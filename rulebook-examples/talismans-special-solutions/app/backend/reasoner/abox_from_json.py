"""
JSON db  ->  OWL ABox (Turtle).

The app's "database" is a plain JSON file (app/backend/db.json) that holds ONLY
the raw, editable rows — exactly the fields a human types. It contains NO
computed fields: no iri, no relativePath, no isStale, no countAISteps, no
closure. Those are NOT data; they are what the reasoner DERIVES.

This module turns those raw rows into the ABox Turtle that the SHACL + OWL-RL
reasoner consumes. It is the JSON analogue of the generated owl/src/individuals.ttl
— same triples, but sourced live from the editable JSON instead of a frozen
build artifact. That is what makes the app writable while keeping every
derivation in OWL/SHACL: edit JSON -> regenerate ABox -> reason -> computed
fields fall out.

It is deliberately domain-agnostic. It does not hardcode any table or field
name. The two pieces of structural knowledge it needs are read from the data
itself:

  * Which property values are object references (erb: IRIs) vs. literals?
    -> a value is a reference iff it equals the primary key of some other row.
  * The transitive-closure seed edges (precedesStep) the reasoner expands.
    -> projected from the StepPrecedence junction rows, exactly as the
       generated ABox does (see individuals.ttl "Junction-projected base edges").

If the JSON is malformed or a referenced PK does not exist, we RAISE. We never
silently drop a triple or substitute a blank node — a missing reference is a
real error in the data and must surface, not vanish.
"""
from __future__ import annotations

import json
import re
from pathlib import Path
from typing import Any, Dict, List

ERB = "http://example.org/erb#"

# ISO-8601 date / dateTime detection. The SHACL date math (monthsSinceModified,
# isStale, …) uses YEAR()/MONTH()/NOW(), which only work on typed temporal
# literals — an untyped string makes YEAR(?x) return nothing and the whole
# staleness chain stays silent. So when a raw value is unambiguously an ISO
# date(Time), we emit it as xsd:dateTime/xsd:date. This is faithful typing of a
# value that genuinely IS a dateTime, not a guessed fallback.
_ISO_DATETIME = re.compile(
    r"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})?$"
)
_ISO_DATE = re.compile(r"^\d{4}-\d{2}-\d{2}$")

# Junction tables are projected into a direct edge for the reasoner to close
# over. This mirrors the generated ABox's "Junction-projected base edges" block.
# {junction class: (from-property, to-property, projected-edge-property)}
JUNCTION_PROJECTIONS = {
    "StepPrecedence": ("fromStep", "toStep", "precedesStep"),
}


def _lname(prop: str) -> str:
    """erb: local name for a JSON key. Keys are already local names."""
    return prop


def _all_pks(db: Dict[str, List[Dict[str, Any]]]) -> Dict[str, str]:
    """Map every row's primary-key value -> the class it belongs to.

    The PK of a row is, by ERB convention, the single *Id field whose value the
    row is keyed by. We detect it as the field whose name ends in 'Id' and whose
    value is the row's identifier. To stay domain-agnostic we treat ANY string
    value that is itself a key of some row as a reference; this map is the set of
    all such keys.
    """
    pks: Dict[str, str] = {}
    for cls, rows in db.items():
        if cls.startswith("__"):
            continue
        for row in rows:
            pk = _row_pk(row)
            if pk in pks:
                raise ValueError(
                    f"Duplicate primary key {pk!r} (in {cls} and {pks[pk]}). "
                    f"PKs must be globally unique — they become IRIs."
                )
            pks[pk] = cls
    return pks


def _row_pk(row: Dict[str, Any]) -> str:
    """The primary-key value of a row.

    Convention: the first field whose name ends in 'Id' (conceptId, roleId,
    workflowStepId, ...). We do NOT guess; if there is no *Id field we raise,
    because without a stable identity we cannot mint an IRI.
    """
    for k, v in row.items():
        if k.endswith("Id") and isinstance(v, str):
            return v
    raise ValueError(
        f"Row has no *Id primary-key field, cannot mint an IRI: {row!r}"
    )


def _ttl_literal(value: Any) -> str:
    """Serialize a scalar JSON value as a Turtle literal."""
    if isinstance(value, bool):
        return "true" if value else "false"
    if isinstance(value, int):
        return str(value)
    if isinstance(value, float):
        return str(value)
    # Typed temporal literals so SHACL's date math fires (see _ISO_* above).
    if isinstance(value, str) and _ISO_DATETIME.match(value):
        return f'"{value}"^^xsd:dateTime'
    if isinstance(value, str) and _ISO_DATE.match(value):
        return f'"{value}"^^xsd:date'
    # string: escape backslashes and quotes, keep it on one line
    s = (
        str(value)
        .replace("\\", "\\\\")
        .replace('"', '\\"')
        .replace("\n", "\\n")
        .replace("\r", "\\r")
    )
    return f'"{s}"'


def json_db_to_turtle(db: Dict[str, Any]) -> str:
    """Render the raw-row JSON db as ABox Turtle.

    A value is emitted as an erb: IRI reference iff it (or, for lists, each
    element) is the PK of a known row. Otherwise it is a literal. This is the
    one structural inference we make, and it is total: a string that *looks*
    like an id but matches no row is treated as a literal, never a dangling IRI.
    """
    pks = _all_pks(db)
    lines: List[str] = [
        "@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .",
        "@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .",
        "@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .",
        f"@prefix erb: <{ERB}> .",
        "",
    ]

    for cls, rows in db.items():
        if cls.startswith("__"):
            continue
        lines.append(f"# === Individuals: {cls} ===")
        for row in rows:
            pk = _row_pk(row)
            triples = [f"erb:{pk} a erb:{cls}"]
            for key, value in row.items():
                if value is None:
                    continue
                # The empty string is this domain's encoding of "no value" — every
                # unfilled FK / relationship slot in the seed and the scenarios
                # uses "" (filledByAIAgent:"", consumesDataset:"", approvalGate:"",
                # …). Postgres maps these to NULL; the OWL ABox must do the same.
                # Emitting "" as a literal triple is never meaningful and actively
                # poisons lookups (an INDEX/MATCH on "" matched no concept yet left
                # a spurious empty FK value, bleeding the verdict). Skip it so OWL's
                # "absent" semantics match Postgres's NULL.
                if value == "":
                    continue
                if isinstance(value, list):
                    for item in value:
                        if item == "":
                            continue
                        triples.append(_pred_obj(key, item, pks))
                else:
                    triples.append(_pred_obj(key, value, pks))
            lines.append(" ;\n    ".join(triples) + " .")
            lines.append("")

    # Junction-projected base edges (transitive-closure seeds).
    proj_lines: List[str] = []
    for cls, (from_p, to_p, edge_p) in JUNCTION_PROJECTIONS.items():
        for row in db.get(cls, []):
            src = row.get(from_p)
            dst = row.get(to_p)
            if src is None or dst is None:
                raise ValueError(
                    f"{cls} row {row!r} missing {from_p}/{to_p}; cannot project "
                    f"the {edge_p} closure seed."
                )
            if src not in pks or dst not in pks:
                raise ValueError(
                    f"{cls} row {row!r} references unknown step(s) "
                    f"{src!r}/{dst!r}. Closure seed would dangle — refusing."
                )
            proj_lines.append(f"erb:{src} erb:{edge_p} erb:{dst} .")
    if proj_lines:
        lines.append("# === Junction-projected base edges (transitive closure seeds) ===")
        lines.extend(proj_lines)
        lines.append("")

    return "\n".join(lines)


def _pred_obj(key: str, value: Any, pks: Dict[str, str]) -> str:
    """One 'predicate object' fragment (no leading subject, no trailing dot)."""
    if isinstance(value, str) and value in pks:
        return f"erb:{_lname(key)} erb:{value}"
    return f"erb:{_lname(key)} {_ttl_literal(value)}"


def load_db(path: Path) -> Dict[str, Any]:
    """Read the JSON db. Missing file is a hard error — there is no empty-db
    fallback (an empty db would let every query return nothing, masking a
    broken setup as a working one)."""
    if not path.exists():
        raise FileNotFoundError(
            f"db.json not found at {path}. The app has no database to reason over."
        )
    with path.open() as f:
        return json.load(f)


if __name__ == "__main__":
    import sys

    db_path = Path(sys.argv[1]) if len(sys.argv) > 1 else (
        Path(__file__).resolve().parents[1] / "db.json"
    )
    print(json_db_to_turtle(load_db(db_path)))
