#!/usr/bin/env python3
"""Validate and project a PKO-native Effortless Rulebook.

No third-party packages are required. The canonical asset is the rulebook;
Markdown documents are deterministic projections and can be deleted/rebuilt.
"""
from __future__ import annotations

import argparse
import copy
import json
import sys
from collections import defaultdict
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Iterable, Mapping, Sequence


PKO_VERSION_IRI = "https://w3id.org/pko/2.0.0"
PKO_INDUSTRY_VERSION_IRI = "https://w3id.org/pko/industry/2.0.0"

REQUIRED_TABLES = {
    "RulebookReleases",
    "OntologyProfiles",
    "SemanticMappings",
    "Organizations",
    "Agents",
    "Roles",
    "RoleAssignments",
    "ProcedureTypes",
    "Procedures",
    "ProcedureVersions",
    "ProcedureStatusChanges",
    "Steps",
    "StepTransitions",
    "Actions",
    "Functions",
    "Tools",
    "Requirements",
    "StepVerifications",
    "Resources",
    "ProcedureExecutions",
    "StepExecutions",
    "RequirementSatisfactions",
    "Errors",
    "IssueOccurrences",
    "UserQuestions",
    "UserFeedback",
}

CORE_MAPPING_EXPECTATIONS = {
    "Procedures": "https://w3id.org/pko#Procedure",
    "Steps": "http://purl.org/net/p-plan#Step",
    "StepTransitions": "https://w3id.org/pko#Transition",
    "Requirements": "https://w3id.org/pko#Requirement",
    "ProcedureExecutions": "https://w3id.org/pko#ProcedureExecution",
    "StepExecutions": "https://w3id.org/pko#StepExecution",
    "IssueOccurrences": "https://w3id.org/pko#IssueOccurrence",
    "UserQuestions": "https://w3id.org/pko#UserQuestionOccurrence",
    "UserFeedback": "https://w3id.org/pko#UserFeedbackOccurrence",
    "FAQs": "https://w3id.org/pko#FrequentlyAskedQuestion",
}

COVERAGE_GOALS = {
    "versioned PKO profile": {"RulebookReleases", "OntologyProfiles", "SemanticMappings"},
    "procedure specification": {"Procedures", "ProcedureVersions", "Steps", "StepTransitions"},
    "procedure execution": {"ProcedureExecutions", "StepExecutions"},
    "actions, functions, tools": {"Actions", "Functions", "Tools", "StepActions", "StepFunctions", "StepTools"},
    "requirements and verification": {"Requirements", "StepRequirements", "StepVerifications", "RequirementSatisfactions"},
    "rationale": {"Rationales"},
    "exceptions and fallback": {"Exceptions", "StepTransitions"},
    "troubleshooting": {"Errors", "IssueOccurrences"},
    "questions, feedback, FAQ": {"UserQuestions", "UserFeedback", "FAQs", "Explanations"},
    "tacit, implicit, explicit knowledge": {"KnowledgeFragments"},
    "knowledge elicitation": {"ElicitationSessions"},
    "knowledge gaps": {"KnowledgeGaps"},
    "stewardship and authority": {"StewardshipAssignments", "ChangeRequests", "ReviewEvents"},
    "role history and valid time": {"Roles", "RoleAssignments"},
    "organizational learning": {"CommunitiesOfPractice", "Mentorships", "LearningActivities"},
    "live operational data": {"OperationalBindings", "Resources"},
    "financial SOP projection": {"ProcedureTypes", "Requirements", "Procedures"},
    "email/SMS policy projection": {"CommunicationPolicies", "MessageTemplates"},
}


class RulebookError(ValueError):
    """Raised for an invalid PKO rulebook."""


def load_rulebook(path: str | Path) -> dict[str, Any]:
    file_path = Path(path)
    try:
        value = json.loads(file_path.read_text(encoding="utf-8"))
    except FileNotFoundError as exc:
        raise RulebookError(f"Rulebook not found: {file_path}") from exc
    except json.JSONDecodeError as exc:
        raise RulebookError(f"Invalid JSON in {file_path}: {exc}") from exc
    if not isinstance(value, dict):
        raise RulebookError("Rulebook root must be a JSON object.")
    return value


def write_text(path: str | Path, text: str) -> None:
    out = Path(path)
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(text.rstrip() + "\n", encoding="utf-8")


def table_rows(rulebook: Mapping[str, Any], table: str) -> list[dict[str, Any]]:
    value = rulebook.get(table, {})
    rows = value.get("data", []) if isinstance(value, Mapping) else []
    return [dict(row) for row in rows if isinstance(row, Mapping)]


def table_schema(rulebook: Mapping[str, Any], table: str) -> list[dict[str, Any]]:
    value = rulebook.get(table, {})
    schema = value.get("schema", []) if isinstance(value, Mapping) else []
    return [dict(field) for field in schema if isinstance(field, Mapping)]


def id_field(rulebook: Mapping[str, Any], table: str) -> str:
    schema = table_schema(rulebook, table)
    if not schema:
        raise RulebookError(f"Table {table} has no schema.")
    return str(schema[0].get("name", ""))


def row_index(rulebook: Mapping[str, Any], table: str) -> dict[str, dict[str, Any]]:
    key = id_field(rulebook, table)
    return {str(row[key]): row for row in table_rows(rulebook, table) if key in row}


def display(row: Mapping[str, Any] | None, fallback: str = "") -> str:
    if not row:
        return fallback
    for key in ("Title", "Label", "DisplayName", "Question", "QuestionText", "Statement", "Name"):
        value = row.get(key)
        if value not in (None, ""):
            return str(value)
    for key, value in row.items():
        if key.endswith("Id") and value:
            return str(value)
    return fallback


def scalar(value: Any, blank: str = "—") -> str:
    if value in (None, ""):
        return blank
    if isinstance(value, bool):
        return "Yes" if value else "No"
    return str(value)


def markdown_escape(text: Any) -> str:
    return str(text if text is not None else "").replace("|", r"\|").replace("\n", " ")


def rows_for(rulebook: Mapping[str, Any], table: str, field: str, value: Any) -> list[dict[str, Any]]:
    return [row for row in table_rows(rulebook, table) if row.get(field) == value]


def current_versions(rulebook: Mapping[str, Any]) -> list[dict[str, Any]]:
    return [row for row in table_rows(rulebook, "ProcedureVersions") if row.get("IsCurrent") is True]


def sort_steps(rows: Iterable[Mapping[str, Any]]) -> list[dict[str, Any]]:
    def key(row: Mapping[str, Any]) -> tuple[int, str]:
        raw = str(row.get("StepNumber", "999999"))
        digits = "".join(ch for ch in raw if ch.isdigit())
        return (int(digits or 999999), raw)
    return [dict(row) for row in sorted(rows, key=key)]


def validate_rulebook(rulebook: Mapping[str, Any]) -> list[str]:
    errors: list[str] = []

    if rulebook.get("$schema") != "https://example.com/cmcc-schema/v1":
        errors.append("$schema must be https://example.com/cmcc-schema/v1.")

    missing = sorted(REQUIRED_TABLES - set(rulebook))
    if missing:
        errors.append("Missing required tables: " + ", ".join(missing))

    table_names = {
        name
        for name, value in rulebook.items()
        if isinstance(value, Mapping) and "schema" in value and "data" in value
    }

    ids_by_table: dict[str, set[str]] = {}
    relation_graph: dict[str, set[str]] = defaultdict(set)

    for table in sorted(table_names):
        schema = table_schema(rulebook, table)
        rows = table_rows(rulebook, table)
        if not schema:
            errors.append(f"{table}: schema is empty.")
            continue
        if not rulebook[table].get("Description"):
            errors.append(f"{table}: table Description is required.")

        first = schema[0]
        primary_name = str(first.get("name", ""))
        # Reserved tables are repo-standard structures, not domain entities: ERBVersions /
        # ERBCustomizations are ERB bookkeeping, and __meta__ is the canonical project-metadata
        # bag whose primary key is MetaKey (not a *Id). They are exempt from the *Id and
        # calculated-Name conventions that every domain table must follow.
        reserved = table in {"ERBVersions", "ERBCustomizations", "__meta__"}
        if not reserved and (
            first.get("type") != "raw"
            or first.get("nullable") is not False
            or not primary_name.endswith("Id")
        ):
            errors.append(f"{table}: first field must be a non-null raw *Id field.")

        if not reserved:
            name_fields = [field for field in schema if field.get("name") == "Name"]
            if not name_fields or name_fields[0].get("type") != "calculated":
                errors.append(f"{table}: must contain a calculated Name display alias.")

        seen_fields: set[str] = set()
        for field in schema:
            fname = str(field.get("name", ""))
            if not fname:
                errors.append(f"{table}: field without name.")
                continue
            if fname in seen_fields:
                errors.append(f"{table}: duplicate field {fname}.")
            seen_fields.add(fname)
            if not field.get("Description"):
                errors.append(f"{table}.{fname}: Description is required.")
            ftype = field.get("type")
            if ftype == "relationship":
                target = field.get("RelatedTo")
                if not target or target not in table_names:
                    errors.append(f"{table}.{fname}: relationship target {target!r} does not exist.")
                elif target != table:
                    relation_graph[table].add(str(target))
            if ftype in {"calculated", "lookup", "aggregation"} and not field.get("formula"):
                errors.append(f"{table}.{fname}: {ftype} field requires formula.")

        ids: set[str] = set()
        for row_number, row in enumerate(rows, start=1):
            if primary_name not in row or row.get(primary_name) in (None, ""):
                errors.append(f"{table} row {row_number}: missing {primary_name}.")
                continue
            row_id = str(row[primary_name])
            if row_id in ids:
                errors.append(f"{table}: duplicate {primary_name} {row_id!r}.")
            ids.add(row_id)
        ids_by_table[table] = ids

    # FK referential integrity
    for table in sorted(table_names):
        schema = table_schema(rulebook, table)
        rel_fields = [field for field in schema if field.get("type") == "relationship"]
        for row in table_rows(rulebook, table):
            row_label = row.get(id_field(rulebook, table), "<unknown>")
            for field in rel_fields:
                fname = str(field["name"])
                target = str(field["RelatedTo"])
                value = row.get(fname)
                if value in (None, ""):
                    if field.get("nullable") is False:
                        errors.append(f"{table}[{row_label}].{fname}: required relationship is blank.")
                    continue
                if str(value) not in ids_by_table.get(target, set()):
                    errors.append(
                        f"{table}[{row_label}].{fname}: {value!r} not found in "
                        f"{target}.{id_field(rulebook, target)}."
                    )

    # Detect table-level cycles in explicit relationship edges.
    visiting: set[str] = set()
    visited: set[str] = set()

    def visit(node: str, trail: list[str]) -> None:
        if node in visiting:
            cycle_start = trail.index(node) if node in trail else 0
            errors.append("Relationship DAG cycle: " + " -> ".join(trail[cycle_start:] + [node]))
            return
        if node in visited:
            return
        visiting.add(node)
        for parent in relation_graph.get(node, set()):
            visit(parent, trail + [node])
        visiting.remove(node)
        visited.add(node)

    for table in sorted(table_names):
        visit(table, [])

    profiles = row_index(rulebook, "OntologyProfiles") if "OntologyProfiles" in table_names else {}
    pko = profiles.get("pko-core-2.0.0")
    if not pko:
        errors.append("OntologyProfiles must include pko-core-2.0.0.")
    else:
        if pko.get("Version") != "2.0.0" or pko.get("VersionIri") != PKO_VERSION_IRI:
            errors.append("pko-core-2.0.0 must reference PKO 2.0.0 exactly.")
    pko_ind = profiles.get("pko-industry-2.0.0")
    if pko_ind and pko_ind.get("VersionIri") != PKO_INDUSTRY_VERSION_IRI:
        errors.append("pko-industry-2.0.0 has the wrong version IRI.")

    mappings = table_rows(rulebook, "SemanticMappings") if "SemanticMappings" in table_names else []
    mapping_pairs = {(str(m.get("SourcePath")), str(m.get("TargetIri"))) for m in mappings}
    for source, target in CORE_MAPPING_EXPECTATIONS.items():
        if (source, target) not in mapping_pairs:
            errors.append(f"Missing exact semantic mapping: {source} -> {target}")

    # Load-bearing specification/execution distinction.
    if "ProcedureVersions" in table_names and "ProcedureExecutions" in table_names:
        version_ids = ids_by_table["ProcedureVersions"]
        execution_ids = ids_by_table["ProcedureExecutions"]
        if version_ids & execution_ids:
            errors.append("Procedure specification and execution identifiers must not overlap.")
        for execution in table_rows(rulebook, "ProcedureExecutions"):
            if execution.get("ProcedureVersion") not in version_ids:
                errors.append(
                    f"ProcedureExecution {execution.get('ProcedureExecutionId')} does not reference a versioned specification."
                )
    if "Steps" in table_names and "StepExecutions" in table_names:
        step_ids = ids_by_table["Steps"]
        step_execution_ids = ids_by_table["StepExecutions"]
        if step_ids & step_execution_ids:
            errors.append("Step specification and execution identifiers must not overlap.")

    # Coverage contract.
    for goal, needed in COVERAGE_GOALS.items():
        absent = sorted(needed - table_names)
        if absent:
            errors.append(f"Coverage goal {goal!r} lacks tables: {', '.join(absent)}")

    return errors


def validation_report(rulebook: Mapping[str, Any]) -> str:
    errors = validate_rulebook(rulebook)
    lines = [
        "# PKO Rulebook Validation",
        "",
        f"- Rulebook: **{rulebook.get('Name', 'Unnamed')}**",
        f"- PKO core: **{PKO_VERSION_IRI}**",
        f"- Result: **{'PASS' if not errors else 'FAIL'}**",
        "",
        "## Coverage",
        "",
        "| Goal | Required tables | Status |",
        "|---|---|---|",
    ]
    present = set(rulebook)
    for goal, tables in COVERAGE_GOALS.items():
        ok = tables <= present
        lines.append(
            f"| {markdown_escape(goal)} | {markdown_escape(', '.join(sorted(tables)))} | "
            f"{'PASS' if ok else 'FAIL'} |"
        )
    lines += ["", "## Findings", ""]
    if errors:
        lines.extend(f"- {error}" for error in errors)
    else:
        lines.append("- No structural, referential, DAG, PKO-version, or coverage errors found.")
    return "\n".join(lines)


def _indexes(rulebook: Mapping[str, Any]) -> dict[str, dict[str, dict[str, Any]]]:
    output: dict[str, dict[str, dict[str, Any]]] = {}
    for name, value in rulebook.items():
        if isinstance(value, Mapping) and "schema" in value and "data" in value and table_schema(rulebook, name):
            output[name] = row_index(rulebook, name)
    return output


def _step_assets(rulebook: Mapping[str, Any], step_id: str, indexes: Mapping[str, Any]) -> dict[str, list[str]]:
    assets: dict[str, list[str]] = {"actions": [], "functions": [], "tools": [], "requirements": []}
    for link in rows_for(rulebook, "StepActions", "Step", step_id):
        assets["actions"].append(display(indexes["Actions"].get(str(link.get("Action"))), str(link.get("Action"))))
    for link in rows_for(rulebook, "StepFunctions", "Step", step_id):
        assets["functions"].append(display(indexes["Functions"].get(str(link.get("Function"))), str(link.get("Function"))))
    for link in rows_for(rulebook, "StepTools", "Step", step_id):
        assets["tools"].append(display(indexes["Tools"].get(str(link.get("Tool"))), str(link.get("Tool"))))
    for link in rows_for(rulebook, "StepRequirements", "Step", step_id):
        assets["requirements"].append(
            display(indexes["Requirements"].get(str(link.get("Requirement"))), str(link.get("Requirement")))
        )
    return assets


def _procedure_context(rulebook: Mapping[str, Any], version: Mapping[str, Any]) -> dict[str, Any]:
    indexes = _indexes(rulebook)
    procedure = indexes["Procedures"].get(str(version.get("Procedure")), {})
    procedure_type = indexes["ProcedureTypes"].get(str(procedure.get("ProcedureType")), {})
    steps = sort_steps(rows_for(rulebook, "Steps", "ProcedureVersion", version.get("ProcedureVersionId")))
    return {"indexes": indexes, "procedure": procedure, "procedure_type": procedure_type, "steps": steps}


def render_natural_language_docs(rulebook: Mapping[str, Any]) -> str:
    indexes = _indexes(rulebook)
    releases = table_rows(rulebook, "RulebookReleases")
    current_release = next((row for row in releases if row.get("IsCurrent") is True), releases[0] if releases else {})
    lines = [
        f"# {rulebook.get('Name', 'PKO Procedural Knowledge')}",
        "",
        rulebook.get("Description", ""),
        "",
        "## Semantic contract",
        "",
        f"- Canonical rulebook release: **{scalar(current_release.get('RulebookVersion'))}**",
        f"- ERB-PKO profile: **{scalar(current_release.get('ProfileVersion'))}**",
        f"- PKO core version IRI: **{scalar(current_release.get('PkoCoreVersionIri'))}**",
        f"- PKO industry version IRI: **{scalar(current_release.get('PkoIndustryVersionIri'))}**",
        "- Procedure specifications and executions are represented as different entities.",
        "- Native PKO mappings and ERB-PKO extensions are distinguished in `SemanticMappings`.",
        "",
    ]

    for version in current_versions(rulebook):
        ctx = _procedure_context(rulebook, version)
        procedure = ctx["procedure"]
        ptype = ctx["procedure_type"]
        steps = ctx["steps"]
        version_id = str(version.get("ProcedureVersionId"))
        lines += [
            f"## {display(procedure)} — v{scalar(version.get('VersionNumber'))}",
            "",
            f"**Type:** {display(ptype)}  ",
            f"**Status:** {scalar(version.get('Status'))}  ",
            f"**Purpose:** {scalar(procedure.get('Purpose'))}  ",
            f"**Target:** {scalar(procedure.get('Target'))}  ",
            f"**Owner:** {display(indexes['Organizations'].get(str(procedure.get('OwnerOrganization'))), str(procedure.get('OwnerOrganization')))}  ",
            f"**Version motivation:** {scalar(version.get('NewVersionMotivation'))}",
            "",
            "### Procedure",
            "",
            "| # | Step | Role | Current agent type | Expected | Human confirmation |",
            "|---:|---|---|---|---:|---|",
        ]
        for step in steps:
            role = indexes["Roles"].get(str(step.get("AssignedRole")), {})
            lines.append(
                f"| {markdown_escape(step.get('StepNumber'))} | {markdown_escape(step.get('Title'))} | "
                f"{markdown_escape(display(role, str(step.get('AssignedRole'))))} | "
                f"{markdown_escape(role.get('CurrentAgentKind', ''))} | "
                f"{markdown_escape(step.get('ExpectedDurationMinutes'))} min | "
                f"{'Yes' if step.get('RequiresHumanConfirmation') else 'No'} |"
            )
        lines.append("")

        for step in steps:
            step_id = str(step.get("StepId"))
            role = indexes["Roles"].get(str(step.get("AssignedRole")), {})
            current_agent = indexes["Agents"].get(str(role.get("CurrentAgent")), {})
            assets = _step_assets(rulebook, step_id, indexes)
            lines += [
                f"#### {scalar(step.get('StepNumber'))}. {scalar(step.get('Title'))}",
                "",
                scalar(step.get("Instruction")),
                "",
                f"- **Responsible role:** {display(role, str(step.get('AssignedRole')))}",
                f"- **Current role filler:** {display(current_agent, str(role.get('CurrentAgent')))} "
                f"({scalar(current_agent.get('AgentKind'))})",
                f"- **Expertise:** {scalar(step.get('ExpertiseLevel'))}",
            ]
            for label, values in (
                ("Human actions", assets["actions"]),
                ("Software functions", assets["functions"]),
                ("Tools", assets["tools"]),
                ("Requirements", assets["requirements"]),
            ):
                if values:
                    lines.append(f"- **{label}:** " + "; ".join(values))

            checks = rows_for(rulebook, "StepVerifications", "Step", step_id)
            for check in checks:
                lines.append(
                    f"- **Verification:** {scalar(check.get('Instruction'))} "
                    f"(`{scalar(check.get('SignalIdentifier'))}` = `{scalar(check.get('ExpectedSignalValue'))}`)"
                )
            exceptions = rows_for(rulebook, "Exceptions", "TriggerStep", step_id)
            for exc in exceptions:
                lines.append(f"- **Exception — {scalar(exc.get('Condition'))}:** {scalar(exc.get('Handling'))}")
            rationales = rows_for(rulebook, "Rationales", "Step", step_id)
            for rationale in rationales:
                lines.append(f"- **Rationale:** {scalar(rationale.get('Statement'))}")
            fragments = rows_for(rulebook, "KnowledgeFragments", "Step", step_id)
            for fragment in fragments:
                lines.append(
                    f"- **{scalar(fragment.get('KnowledgeForm'))} knowledge:** {scalar(fragment.get('Statement'))} "
                    f"(confidence: {scalar(fragment.get('Confidence'))}, status: {scalar(fragment.get('Status'))})"
                )
            lines.append("")

        resources = [
            indexes["Resources"].get(str(link.get("Resource")), {})
            for link in rows_for(rulebook, "ProcedureResources", "ProcedureVersion", version_id)
        ]
        if resources:
            lines += ["### Referenced resources", ""]
            for resource in resources:
                lines.append(
                    f"- **{display(resource)}** — {scalar(resource.get('ResourceKind'))}; "
                    f"`{scalar(resource.get('ExternalUri'))}`"
                )
            lines.append("")

        gaps = rows_for(rulebook, "KnowledgeGaps", "ProcedureVersion", version_id)
        stewards = rows_for(rulebook, "StewardshipAssignments", "ProcedureVersion", version_id)
        changes = rows_for(rulebook, "ChangeRequests", "ProcedureVersion", version_id)
        reviews = rows_for(rulebook, "ReviewEvents", "ProcedureVersion", version_id)
        lines += ["### Governance and knowledge health", ""]
        for assignment in stewards:
            lines.append(
                f"- **Steward:** {display(indexes['Roles'].get(str(assignment.get('StewardRole'))), str(assignment.get('StewardRole')))}; "
                f"**authority:** {display(indexes['Roles'].get(str(assignment.get('AuthorityRole'))), str(assignment.get('AuthorityRole')))}; "
                f"review every {scalar(assignment.get('ReviewCadenceDays'))} days."
            )
        for gap in gaps:
            lines.append(
                f"- **Knowledge gap ({scalar(gap.get('Status'))}, {scalar(gap.get('Severity'))}):** "
                f"{scalar(gap.get('Statement'))}"
            )
        for change in changes:
            lines.append(f"- **Change request ({scalar(change.get('Status'))}):** {scalar(change.get('Title'))}")
        for review in reviews:
            lines.append(
                f"- **Review:** {scalar(review.get('ReviewKind'))} on {scalar(review.get('ReviewedAt'))} — "
                f"{scalar(review.get('Outcome'))}"
            )
        lines.append("")

        executions = rows_for(rulebook, "ProcedureExecutions", "ProcedureVersion", version_id)
        if executions:
            lines += ["### Execution witnesses", ""]
            for execution in executions:
                lines.append(
                    f"- **{scalar(execution.get('Context'))}** — {scalar(execution.get('ExecutionStatus'))}, "
                    f"{scalar(execution.get('StartedAt'))} → {scalar(execution.get('EndedAt'))}; "
                    f"`{scalar(execution.get('OperationalRecordUri'))}`"
                )
            lines.append("")

    lines += [
        "## Cross-cutting knowledge infrastructure",
        "",
        f"- Elicitation sessions: **{len(table_rows(rulebook, 'ElicitationSessions'))}**",
        f"- Captured knowledge fragments: **{len(table_rows(rulebook, 'KnowledgeFragments'))}**",
        f"- Communities of practice: **{len(table_rows(rulebook, 'CommunitiesOfPractice'))}**",
        f"- Historical role assignments: **{len(table_rows(rulebook, 'RoleAssignments'))}**",
        f"- Live operational bindings: **{len(table_rows(rulebook, 'OperationalBindings'))}**",
        f"- Semantic mappings: **{len(table_rows(rulebook, 'SemanticMappings'))}**",
        "",
        "This document is derivative. Change the canonical rulebook, then regenerate it.",
    ]
    return "\n".join(lines)


def render_financial_sops(rulebook: Mapping[str, Any]) -> str:
    indexes = _indexes(rulebook)
    financial_types = {
        row["ProcedureTypeId"]
        for row in table_rows(rulebook, "ProcedureTypes")
        if "financial" in (str(row.get("ProcedureTypeId", "")) + " " + str(row.get("Label", ""))).lower()
    }
    versions = []
    for version in current_versions(rulebook):
        procedure = indexes["Procedures"].get(str(version.get("Procedure")), {})
        if procedure.get("ProcedureType") in financial_types:
            versions.append(version)

    lines = [
        "# Financial Standard Operating Procedures",
        "",
        f"Generated from `{rulebook.get('Name')}`; PKO profile {PKO_VERSION_IRI}.",
        "",
    ]
    if not versions:
        lines.append("_No financial procedures were found._")
        return "\n".join(lines)

    for version in versions:
        ctx = _procedure_context(rulebook, version)
        procedure, steps = ctx["procedure"], ctx["steps"]
        version_id = str(version.get("ProcedureVersionId"))
        lines += [
            f"## {display(procedure)} — version {scalar(version.get('VersionNumber'))}",
            "",
            f"**Control objective:** {scalar(procedure.get('Purpose'))}",
            "",
            "### Roles and segregation of duties",
            "",
        ]
        role_ids = []
        for step in steps:
            rid = str(step.get("AssignedRole"))
            if rid not in role_ids:
                role_ids.append(rid)
        for rid in role_ids:
            role = indexes["Roles"].get(rid, {})
            agent = indexes["Agents"].get(str(role.get("CurrentAgent")), {})
            lines.append(
                f"- **{display(role, rid)}** — {scalar(role.get('Responsibility'))}; current filler: "
                f"{display(agent, str(role.get('CurrentAgent')))} ({scalar(agent.get('AgentKind'))})."
            )
        lines += ["", "### Required controls", ""]
        requirement_ids = {
            link.get("Requirement")
            for link in table_rows(rulebook, "StepRequirements")
            if link.get("Step") in {step.get("StepId") for step in steps}
        }
        for requirement_id in sorted(str(x) for x in requirement_ids if x):
            req = indexes["Requirements"].get(requirement_id, {})
            lines.append(
                f"- **{display(req, requirement_id)}:** {scalar(req.get('Statement'))} "
                f"_Rationale: {scalar(req.get('Rationale'))}_"
            )
        lines += ["", "### Procedure", ""]
        for step in steps:
            assets = _step_assets(rulebook, str(step.get("StepId")), indexes)
            lines += [
                f"{scalar(step.get('StepNumber'))}. **{scalar(step.get('Title'))}** — {scalar(step.get('Instruction'))}",
                f"   - Owner: {display(indexes['Roles'].get(str(step.get('AssignedRole'))), str(step.get('AssignedRole')))}",
                f"   - Expected duration: {scalar(step.get('ExpectedDurationMinutes'))} minutes",
            ]
            if assets["tools"]:
                lines.append("   - Tools: " + "; ".join(assets["tools"]))
            checks = rows_for(rulebook, "StepVerifications", "Step", step.get("StepId"))
            for check in checks:
                lines.append(f"   - Control evidence: {scalar(check.get('Instruction'))}")
            for exc in rows_for(rulebook, "Exceptions", "TriggerStep", step.get("StepId")):
                lines.append(f"   - Exception: {scalar(exc.get('Condition'))} → {scalar(exc.get('Handling'))}")
        lines += ["", "### Evidence, audit, and change control", ""]
        for link in rows_for(rulebook, "ProcedureResources", "ProcedureVersion", version_id):
            resource = indexes["Resources"].get(str(link.get("Resource")), {})
            lines.append(f"- {display(resource)} — `{scalar(resource.get('ExternalUri'))}`")
        for change in rows_for(rulebook, "ChangeRequests", "ProcedureVersion", version_id):
            lines.append(f"- Change request **{scalar(change.get('Title'))}**: {scalar(change.get('Status'))}")
        lines.append("")
    return "\n".join(lines)


def render_text_email_policies(rulebook: Mapping[str, Any]) -> str:
    indexes = _indexes(rulebook)
    lines = [
        "# Employee Email and Text-Message Policies",
        "",
        f"Generated from the same canonical PKO rulebook as the SOP and execution model. PKO profile: {PKO_VERSION_IRI}.",
        "",
    ]
    for policy in table_rows(rulebook, "CommunicationPolicies"):
        role = indexes["Roles"].get(str(policy.get("ApprovalRole")), {})
        lines += [
            f"## {scalar(policy.get('Channel'))}",
            "",
            f"- **Status:** {scalar(policy.get('Status'))}",
            f"- **Audience:** {scalar(policy.get('AudienceRule'))}",
            f"- **Consent required:** {scalar(policy.get('ConsentRequired'))}",
            f"- **Quiet hours:** {scalar(policy.get('QuietHoursStart'))}–{scalar(policy.get('QuietHoursEnd'))} recipient-local time",
            f"- **Maximum length:** {scalar(policy.get('MaxMessageLength'))} characters; "
            f"{scalar(policy.get('MaxSegments'))} segment(s)",
            f"- **Retention:** {scalar(policy.get('RetentionDays'))} days",
            f"- **Approval role:** {display(role, str(policy.get('ApprovalRole')))}",
            f"- **Required content:** {scalar(policy.get('RequiredContent'))}",
            f"- **Authority:** {scalar(policy.get('AuthorityStatement'))}",
            "",
            "### Approved templates",
            "",
        ]
        templates = rows_for(rulebook, "MessageTemplates", "CommunicationPolicy", policy.get("CommunicationPolicyId"))
        for template in templates:
            lines += [
                f"#### {scalar(template.get('Locale'))}",
                "",
            ]
            if template.get("SubjectTemplate"):
                lines.append(f"**Subject:** `{template.get('SubjectTemplate')}`")
                lines.append("")
            lines.append(str(template.get("BodyTemplate", "")))
            lines.append("")

        policy_version = policy.get("ProcedureVersion")
        policy_steps = sort_steps(rows_for(rulebook, "Steps", "ProcedureVersion", policy_version))
        related = [step for step in policy_steps if step.get("StepNumber") in {"04", "05", "06", "07", "08"}]
        if related:
            lines += ["### Governing procedure steps", ""]
            for step in related:
                lines.append(f"- **{scalar(step.get('StepNumber'))}. {scalar(step.get('Title'))}:** {scalar(step.get('Instruction'))}")
            lines.append("")
    return "\n".join(lines)


def render_governance(rulebook: Mapping[str, Any]) -> str:
    indexes = _indexes(rulebook)
    lines = [
        "# Procedural Knowledge Governance",
        "",
        f"Canonical PKO rulebook: **{rulebook.get('Name')}**",
        "",
        "## Stewardship and authority",
        "",
    ]
    for row in table_rows(rulebook, "StewardshipAssignments"):
        version = indexes["ProcedureVersions"].get(str(row.get("ProcedureVersion")), {})
        steward = indexes["Roles"].get(str(row.get("StewardRole")), {})
        authority = indexes["Roles"].get(str(row.get("AuthorityRole")), {})
        lines.append(
            f"- **{display(version, str(row.get('ProcedureVersion')))}** — steward: "
            f"{display(steward, str(row.get('StewardRole')))}; authority: "
            f"{display(authority, str(row.get('AuthorityRole')))}; cadence: "
            f"{scalar(row.get('ReviewCadenceDays'))} days."
        )

    lines += ["", "## Change requests", ""]
    for row in table_rows(rulebook, "ChangeRequests"):
        lines.append(
            f"- **{scalar(row.get('Title'))}** ({scalar(row.get('ChangeKind'))}, {scalar(row.get('Status'))}) — "
            f"{scalar(row.get('ImpactAssessment'))}"
        )

    lines += ["", "## Knowledge gaps", ""]
    for row in table_rows(rulebook, "KnowledgeGaps"):
        owner = indexes["Roles"].get(str(row.get("OwnerRole")), {})
        lines.append(
            f"- **{scalar(row.get('Severity'))} / {scalar(row.get('Status'))}:** "
            f"{scalar(row.get('Statement'))} Owner: {display(owner, str(row.get('OwnerRole')))}. "
            f"Plan: {scalar(row.get('ResolutionPlan'))}"
        )

    lines += ["", "## Elicitation and learning", ""]
    for row in table_rows(rulebook, "ElicitationSessions"):
        practitioner = indexes["Agents"].get(str(row.get("PractitionerAgent")), {})
        facilitator = indexes["Agents"].get(str(row.get("FacilitatorAgent")), {})
        lines.append(
            f"- **{scalar(row.get('Method'))}** — practitioner {display(practitioner)}; "
            f"facilitator {display(facilitator)}. {scalar(row.get('Summary'))}"
        )
    for row in table_rows(rulebook, "LearningActivities"):
        lines.append(f"- **{scalar(row.get('ActivityKind'))}:** {scalar(row.get('Outcome'))}")

    lines += ["", "## Role history", ""]
    for row in table_rows(rulebook, "RoleAssignments"):
        role = indexes["Roles"].get(str(row.get("Role")), {})
        agent = indexes["Agents"].get(str(row.get("Agent")), {})
        lines.append(
            f"- {display(role)} ← {display(agent)}; valid {scalar(row.get('ValidFrom'))} to "
            f"{scalar(row.get('ValidTo'))}; {scalar(row.get('Status'))}. {scalar(row.get('Reason'))}"
        )

    lines += ["", "## Operational bindings", ""]
    for row in table_rows(rulebook, "OperationalBindings"):
        resource = indexes["Resources"].get(str(row.get("Resource")), {})
        lines.append(
            f"- **{scalar(row.get('RecordOrSchemaKey'))}** ({scalar(row.get('AccessMode'))}) — "
            f"{display(resource)}; authoritative: {scalar(row.get('IsAuthoritative'))}; "
            f"freshness SLA: {scalar(row.get('FreshnessSlaMinutes'))} minutes."
        )
    return "\n".join(lines)


RENDERERS = {
    "natural-language-docs": render_natural_language_docs,
    "financial-sops": render_financial_sops,
    "text-email-policies": render_text_email_policies,
    "governance": render_governance,
    "validation-report": validation_report,
}


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Validate or project a PKO 2.0.0-aligned Effortless Rulebook."
    )
    subparsers = parser.add_subparsers(dest="command", required=True)

    validate = subparsers.add_parser("validate", help="Validate ERB structure, DAG, FKs, PKO version, and coverage.")
    validate.add_argument("-i", "--input", required=True, help="Input procedural-knowledge-ontology-rulebook.json")
    validate.add_argument("-o", "--output", help="Optional Markdown validation report.")

    for command in ("natural-language-docs", "financial-sops", "text-email-policies", "governance", "validation-report"):
        sub = subparsers.add_parser(command, help=f"Generate {command}.")
        sub.add_argument("-i", "--input", required=True, help="Input procedural-knowledge-ontology-rulebook.json")
        sub.add_argument("-o", "--output", required=True, help="Output Markdown path.")

    return parser


def main(argv: Sequence[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    try:
        rulebook = load_rulebook(args.input)
        errors = validate_rulebook(rulebook)
        if args.command == "validate":
            report = validation_report(rulebook)
            if args.output:
                write_text(args.output, report)
            else:
                print(report)
            return 1 if errors else 0

        if errors:
            print(validation_report(rulebook), file=sys.stderr)
            return 1
        renderer = RENDERERS[args.command]
        write_text(args.output, renderer(rulebook))
        print(f"Wrote {args.command}: {args.output}")
        return 0
    except RulebookError as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
