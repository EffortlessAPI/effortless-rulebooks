#!/usr/bin/env python3
"""
One-shot rulebook editor: add a universal RelativePath + Iri to every table.

RATIONALE (see the OWL substrate work): every row should carry a stable,
DAG-derived identity. RelativePath is the routable location (slash-separated,
NO leading slash so the Iri swap is clean); Iri is the opaque token form
(dashes), which the OWL transpiler uses to mint each individual's IRI —
eliminating the bare-PK collisions that broke conformance.

This is the CMCC-native expression: per-table calculated/lookup fields that
reach up the DAG. Root tables compute their own segment; child tables INDEX/
MATCH the parent's already-computed RelativePath (the DAG does the recursion,
one hop per table). Excel dialect only uses functions the substrates support
(INDEX, MATCH, SUBSTITUTE, & concat) — deliberately NO MID/LEFT/RIGHT.

Run once; idempotent (skips a table that already has the fields).
"""
import json
from pathlib import Path

RB = Path(__file__).resolve().parent.parent / "effortless-rulebook" / "talisman-basic-rulebook.json"

# table -> (parent_spec, segment)
#   parent_spec = None                       → root table (path = "<segment>/" & {{pk}})
#   parent_spec = (ParentTable, ParentFkField) → child (INDEX/MATCH up to parent)
DESIGN = {
    # Roots — each owns a top-level collection segment
    "Workflows":                (None, "workflows"),
    "Roles":                    (None, "roles"),
    "Departments":              (None, "departments"),
    "HumanAgents":              (None, "human-agents"),
    "AIAgents":                 (None, "ai-agents"),
    "AutomatedPipelines":       (None, "automated-pipelines"),
    "Datasets":                 (None, "datasets"),
    "WorkflowStatusConcepts":   (None, "concepts/workflow-status"),
    "AgentCapabilityConcepts":  (None, "concepts/agent-capability"),
    # Children — nested under their structural ("contains") parent
    "WorkflowSteps":      (("Workflows", "Workflow"), "steps"),
    "ApprovalGates":      (("WorkflowSteps", "WorkflowStep"), "approval-gates"),
    "StepPrecedence":     (("WorkflowSteps", "FromStep"), "precedence"),
    "ComplianceVerdicts": (("Workflows", "Workflow"), "verdicts"),
    "WorkflowArtifacts":  (("WorkflowSteps", "ProducedByStep"), "artifacts"),
}


def pk_field(schema):
    for c in schema:
        if c.get("type", "raw") == "raw":
            return c["name"]
    return schema[0]["name"]


def pk_field_of(rb, table):
    return pk_field(rb[table]["schema"])


def make_parentpath_field(rb, table, parent_spec, segment):
    """Child tables only: a pure-passthrough LOOKUP of the parent's RelativePath.

    The deployed rulebook-to-postgres compiles a `lookup` as a verbatim copy of
    the related field (it does NOT support post-concatenation on a lookup). So we
    split: ParentPath does the cross-table reach (passthrough, which DOES
    compile), and RelativePath below concatenates using only & on LOCAL fields
    (which also compiles — same shape as the existing StepPrecedence.Name).
    """
    parent_table, parent_fk = parent_spec
    parent_pk = pk_field_of(rb, parent_table)
    return {
        "name": "ParentPath",
        "datatype": "string",
        "type": "lookup",
        "nullable": True,
        "RelatedTo": parent_table,
        "Description": (
            f"Helper: the {parent_table} parent's RelativePath, pulled across the {parent_fk} FK. "
            f"Exists so RelativePath can concatenate the '/{segment}/' segment using only local-field "
            f"'&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)."
        ),
        "formula": (
            f'=INDEX({parent_table}!{{{{RelativePath}}}}, '
            f'MATCH({{{{{parent_fk}}}}}, {parent_table}!{{{{{parent_pk}}}}}, 0))'
        ),
    }


def make_relpath_field(rb, table, parent_spec, segment):
    pk = pk_field_of(rb, table)
    if parent_spec is None:
        return {
            "name": "RelativePath",
            "datatype": "string",
            "type": "calculated",
            "nullable": True,
            "Description": (
                f"Stable, DAG-derived location for this {table[:-1] if table.endswith('s') else table} "
                f"row. Root segment '{segment}' + the row's primary key. No leading slash so the Iri "
                f"swap is a clean 1:1 substitution. The relational analogue of a REST resource path; "
                f"unique by construction across the whole model."
            ),
            "formula": f'="{segment}/" & {{{{{pk}}}}}',
        }
    # Child: concatenate the (already passthrough-resolved) ParentPath with this
    # row's own segment + pk. Only LOCAL fields in the '&' chain → compiles.
    return {
        "name": "RelativePath",
        "datatype": "string",
        "type": "calculated",
        "nullable": True,
        "Description": (
            f"Stable, DAG-derived location: this row nests under its {parent_spec[0]} parent. "
            f"Concatenates the parent's path (ParentPath) with '/{segment}/' + this row's primary key. "
            f"The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry "
            f"is encoded without a recursive formula. Unique by construction."
        ),
        "formula": f'={{{{ParentPath}}}} & "/{segment}/" & {{{{{pk}}}}}',
    }


def make_iri_field(table):
    return {
        "name": "Iri",
        "datatype": "string",
        "type": "calculated",
        "nullable": True,
        "Description": (
            "Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no "
            "leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each "
            "individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally "
            "unique — no cross-table primary-key collisions."
        ),
        "formula": '=SUBSTITUTE({{RelativePath}}, "/", "-")',
    }


def insert_after_pk(schema, *new_fields):
    """Insert new fields immediately after the PK (first raw) field."""
    pk_idx = 0
    for i, c in enumerate(schema):
        if c.get("type", "raw") == "raw":
            pk_idx = i
            break
    out = schema[: pk_idx + 1] + list(new_fields) + schema[pk_idx + 1:]
    return out


def main():
    rb = json.loads(RB.read_text())
    changed = []
    for table, (parent_spec, segment) in DESIGN.items():
        if table not in rb:
            raise SystemExit(f"Table {table} not in rulebook — design map is stale.")
        schema = rb[table]["schema"]
        existing = {c.get("name") for c in schema}
        if "RelativePath" in existing or "Iri" in existing:
            print(f"  SKIP {table} (already has RelativePath/Iri)")
            continue
        relpath = make_relpath_field(rb, table, parent_spec, segment)
        iri = make_iri_field(table)
        if parent_spec is None:
            new_fields = (relpath, iri)
            shape = "RelativePath(calc) + Iri"
        else:
            parentpath = make_parentpath_field(rb, table, parent_spec, segment)
            new_fields = (parentpath, relpath, iri)
            shape = "ParentPath(lookup) + RelativePath(calc) + Iri"
        rb[table]["schema"] = insert_after_pk(schema, *new_fields)
        changed.append(table)
        print(f"  ADD  {table}: {shape}")

    if not changed:
        print("Nothing to do.")
        return

    # Pretty-print with 2-space indent to match the existing file style.
    RB.write_text(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print(f"\nUpdated {len(changed)} tables: {', '.join(changed)}")


if __name__ == "__main__":
    main()
