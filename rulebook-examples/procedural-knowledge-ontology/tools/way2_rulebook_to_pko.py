#!/usr/bin/env python3
"""Example adapter from a simple "Way2" process JSON into the canonical ERB-PKO rulebook.

The adapter demonstrates the n+1 economics: once a source representation maps
to the canonical PKO rulebook, every existing PKO projection becomes available.
"""
from __future__ import annotations

import argparse
import copy
import json
import re
import sys
from pathlib import Path
from typing import Any, Mapping, Sequence

sys.path.insert(0, str(Path(__file__).resolve().parent))
from pko_rulebook_tool import load_rulebook, validate_rulebook, RulebookError  # noqa: E402


def slug(value: str) -> str:
    out = re.sub(r"[^a-z0-9]+", "-", value.strip().lower()).strip("-")
    return out or "item"


def rows(rulebook: dict[str, Any], table: str) -> list[dict[str, Any]]:
    return rulebook[table]["data"]


def id_name(rulebook: Mapping[str, Any], table: str) -> str:
    return str(rulebook[table]["schema"][0]["name"])


def ids(rulebook: Mapping[str, Any], table: str) -> set[str]:
    key = id_name(rulebook, table)
    return {str(row[key]) for row in rulebook[table]["data"]}


def append_unique(rulebook: dict[str, Any], table: str, row: dict[str, Any]) -> None:
    key = id_name(rulebook, table)
    existing = ids(rulebook, table)
    value = str(row[key])
    if value in existing:
        return
    rows(rulebook, table).append(row)


def ensure_org(rulebook: dict[str, Any], value: Mapping[str, Any]) -> str:
    org_id = str(value.get("id") or slug(str(value.get("name") or "organization")))
    append_unique(
        rulebook,
        "Organizations",
        {
            "OrganizationId": org_id,
            "DisplayName": str(value.get("name") or org_id),
            "OrganizationType": str(value.get("type") or "Organization"),
            "ExternalIdentifier": str(value.get("external_identifier") or org_id),
            "SemanticTypeIri": "http://www.w3.org/ns/prov#Organization",
        },
    )
    return org_id


def ensure_agent(rulebook: dict[str, Any], value: Mapping[str, Any], default_org: str) -> str:
    agent_id = str(value.get("id") or slug(str(value.get("name") or "agent")))
    kind = str(value.get("kind") or "Human")
    semantic = "http://www.w3.org/ns/prov#SoftwareAgent" if kind in {"AIAgent", "AutomatedPipeline"} else "http://www.w3.org/ns/prov#Agent"
    append_unique(
        rulebook,
        "Agents",
        {
            "AgentId": agent_id,
            "DisplayName": str(value.get("name") or agent_id),
            "AgentKind": kind,
            "Organization": str(value.get("organization") or default_org),
            "ContactAddress": str(value.get("contact") or ""),
            "VersionOrEmploymentKey": str(value.get("version_or_assignment") or agent_id),
            "SemanticTypeIri": semantic,
        },
    )
    return agent_id


def ensure_role(rulebook: dict[str, Any], value: Mapping[str, Any], default_org: str) -> str:
    role_id = str(value.get("id") or slug(str(value.get("label") or "role")))
    agent_value = value.get("agent")
    if not isinstance(agent_value, Mapping):
        agent_value = {
            "id": f"{role_id}-unassigned-agent",
            "name": f"Unassigned agent for {value.get('label') or role_id}",
            "kind": "Human",
        }
    agent_id = ensure_agent(rulebook, agent_value, default_org)
    append_unique(
        rulebook,
        "Roles",
        {
            "RoleId": role_id,
            "Label": str(value.get("label") or role_id),
            "Organization": str(value.get("organization") or default_org),
            "CurrentAgent": agent_id,
            "Responsibility": str(value.get("responsibility") or "Imported Way2 process responsibility."),
            "SemanticTypeIri": "http://purl.org/spar/pro/Role",
        },
    )
    append_unique(
        rulebook,
        "RoleAssignments",
        {
            "RoleAssignmentId": f"way2-{role_id}-assignment",
            "Role": role_id,
            "Agent": agent_id,
            "ValidFrom": str(value.get("valid_from") or "2026-01-01T00:00:00Z"),
            "ValidTo": value.get("valid_to"),
            "Reason": "Imported from Way2 current assignment.",
            "Status": "Active",
            "SemanticTypeIri": "http://purl.org/spar/pro/RoleInTime",
        },
    )
    return role_id


def import_process(rulebook: dict[str, Any], process: Mapping[str, Any], default_org: str) -> None:
    procedure_id = str(process.get("id") or slug(str(process.get("title") or "procedure")))
    type_id = str(process.get("type") or "imported-procedure")
    if type_id not in ids(rulebook, "ProcedureTypes"):
        append_unique(
            rulebook,
            "ProcedureTypes",
            {
                "ProcedureTypeId": type_id,
                "Label": str(process.get("type_label") or type_id.replace("-", " ").title()),
                "Definition": "Procedure type imported from Way2.",
                "SemanticTypeIri": "https://w3id.org/pko#ProcedureType",
            },
        )

    owner_org = str(process.get("owner_organization") or default_org)
    version_number = str(process.get("version") or "1.0.0")
    version_id = str(process.get("version_id") or f"{procedure_id}-v{version_number}")
    append_unique(
        rulebook,
        "Procedures",
        {
            "ProcedureId": procedure_id,
            "Title": str(process.get("title") or procedure_id),
            "ProcedureType": type_id,
            "OwnerOrganization": owner_org,
            "AdoptedByOrganization": str(process.get("adopted_by") or default_org),
            "Purpose": str(process.get("purpose") or ""),
            "Target": str(process.get("target") or ""),
            "IsTemplate": bool(process.get("is_template", False)),
            "CurrentVersionKey": version_id,
            "SemanticTypeIri": "https://w3id.org/pko#Procedure",
        },
    )
    append_unique(
        rulebook,
        "ProcedureVersions",
        {
            "ProcedureVersionId": version_id,
            "Procedure": procedure_id,
            "VersionNumber": version_number,
            "Title": f"{process.get('title') or procedure_id} v{version_number}",
            "Status": str(process.get("status") or "Draft"),
            "IssuedAt": str(process.get("issued_at") or "2026-07-19T12:00:00Z"),
            "ModifiedAt": str(process.get("modified_at") or "2026-07-19T12:00:00Z"),
            "CreatedByAgent": str(process.get("created_by_agent") or next(iter(ids(rulebook, "Agents")))),
            "ModifiedByAgent": str(process.get("modified_by_agent") or process.get("created_by_agent") or next(iter(ids(rulebook, "Agents")))),
            "NewVersionMotivation": str(process.get("version_motivation") or "Imported from Way2."),
            "ChangelogDescription": str(process.get("changelog") or "Initial Way2 import."),
            "IsCurrent": True,
            "SemanticTypeIri": "https://w3id.org/pko#Procedure",
        },
    )

    role_map: dict[str, str] = {}
    for role in process.get("roles", []):
        if isinstance(role, Mapping):
            role_id = ensure_role(rulebook, role, owner_org)
            role_map[str(role.get("id") or role_id)] = role_id

    imported_steps: list[str] = []
    for ordinal, step in enumerate(process.get("steps", []), start=1):
        if not isinstance(step, Mapping):
            continue
        step_local = str(step.get("id") or f"step-{ordinal:02d}")
        step_id = f"{procedure_id}-{step_local}" if not step_local.startswith(procedure_id) else step_local
        role_key = str(step.get("role") or "")
        role_id = role_map.get(role_key, role_key)
        if not role_id:
            role_id = ensure_role(
                rulebook,
                {"id": f"{procedure_id}-owner", "label": "Procedure Owner", "responsibility": "Imported procedure owner."},
                owner_org,
            )
        imported_steps.append(step_id)
        append_unique(
            rulebook,
            "Steps",
            {
                "StepId": step_id,
                "ProcedureVersion": version_id,
                "StepNumber": str(step.get("number") or f"{ordinal:02d}").zfill(2),
                "Title": str(step.get("title") or step_local.replace("-", " ").title()),
                "StepKind": str(step.get("kind") or "Atomic"),
                "AssignedRole": role_id,
                "Instruction": str(step.get("instruction") or ""),
                "ExpectedDurationMinutes": int(step.get("expected_minutes") or 0),
                "ExpertiseLevel": str(step.get("expertise") or "Senior"),
                "RequiresHumanConfirmation": bool(step.get("requires_human_confirmation", False)),
                "SemanticTypeIri": (
                    "http://purl.org/net/p-plan#MultiStep"
                    if str(step.get("kind") or "Atomic") == "MultiStep"
                    else "http://purl.org/net/p-plan#Step"
                ),
            },
        )

        for req in step.get("requirements", []):
            if isinstance(req, str):
                req = {"statement": req}
            if not isinstance(req, Mapping):
                continue
            req_id = str(req.get("id") or f"{step_id}-requirement-{slug(str(req.get('statement') or 'required'))[:36]}")
            append_unique(
                rulebook,
                "Requirements",
                {
                    "RequirementId": req_id,
                    "Label": str(req.get("label") or req.get("statement") or req_id)[:120],
                    "RequirementType": str(req.get("type") or "Imported"),
                    "Statement": str(req.get("statement") or ""),
                    "Rationale": str(req.get("rationale") or "Imported Way2 requirement."),
                    "IsBlocking": bool(req.get("blocking", True)),
                    "SemanticTypeIri": "https://w3id.org/pko#Requirement",
                },
            )
            append_unique(
                rulebook,
                "StepRequirements",
                {
                    "StepRequirementId": f"{step_id}-{req_id}",
                    "Step": step_id,
                    "Requirement": req_id,
                },
            )

    transitions = process.get("transitions", [])
    if transitions:
        for ordinal, transition in enumerate(transitions, start=1):
            if not isinstance(transition, Mapping):
                continue
            from_local = str(transition.get("from"))
            to_local = str(transition.get("to"))
            from_id = f"{procedure_id}-{from_local}" if not from_local.startswith(procedure_id) else from_local
            to_id = f"{procedure_id}-{to_local}" if not to_local.startswith(procedure_id) else to_local
            append_unique(
                rulebook,
                "StepTransitions",
                {
                    "StepTransitionId": str(transition.get("id") or f"{from_id}-to-{to_id}-{ordinal}"),
                    "ProcedureVersion": version_id,
                    "FromStep": from_id,
                    "ToStep": to_id,
                    "TransitionKind": str(transition.get("kind") or "Next"),
                    "Condition": str(transition.get("condition") or "Default sequence"),
                    "Priority": int(transition.get("priority") or 1),
                    "SemanticTypeIri": "https://w3id.org/pko#Transition",
                },
            )
    else:
        for ordinal, (from_id, to_id) in enumerate(zip(imported_steps, imported_steps[1:]), start=1):
            append_unique(
                rulebook,
                "StepTransitions",
                {
                    "StepTransitionId": f"{from_id}-to-{to_id}",
                    "ProcedureVersion": version_id,
                    "FromStep": from_id,
                    "ToStep": to_id,
                    "TransitionKind": "Next",
                    "Condition": "Default sequence",
                    "Priority": 1,
                    "SemanticTypeIri": "https://w3id.org/pko#Transition",
                },
            )

    for ordinal, rationale in enumerate(process.get("rationales", []), start=1):
        if isinstance(rationale, str):
            rationale = {"statement": rationale}
        if not isinstance(rationale, Mapping):
            continue
        step_local = rationale.get("step")
        step_id = None
        if step_local:
            step_id = str(step_local)
            if not step_id.startswith(procedure_id):
                step_id = f"{procedure_id}-{step_id}"
        append_unique(
            rulebook,
            "Rationales",
            {
                "RationaleId": str(rationale.get("id") or f"{procedure_id}-rationale-{ordinal}"),
                "ProcedureVersion": version_id,
                "Step": step_id,
                "Title": str(rationale.get("title") or f"Imported rationale {ordinal}"),
                "Statement": str(rationale.get("statement") or ""),
                "Status": str(rationale.get("status") or "Reviewed"),
                "AuthorityRole": str(rationale.get("authority_role") or next(iter(role_map.values()), "")),
                "SemanticTypeIri": "urn:effortless:pko-extension#Rationale",
            },
        )

    for ordinal, exception in enumerate(process.get("exceptions", []), start=1):
        if not isinstance(exception, Mapping):
            continue
        trigger_local = str(exception.get("step") or imported_steps[-1])
        trigger_id = (
            trigger_local
            if trigger_local.startswith(procedure_id)
            else f"{procedure_id}-{trigger_local}"
        )
        approval = str(exception.get("approval_role") or next(iter(role_map.values()), ""))
        fallback = str(exception.get("fallback_role") or approval)
        append_unique(
            rulebook,
            "Exceptions",
            {
                "ExceptionId": str(exception.get("id") or f"{procedure_id}-exception-{ordinal}"),
                "ProcedureVersion": version_id,
                "TriggerStep": trigger_id,
                "Condition": str(exception.get("condition") or ""),
                "Handling": str(exception.get("handling") or ""),
                "ApprovalRole": approval,
                "FallbackRole": fallback,
                "Status": str(exception.get("status") or "Active"),
                "SemanticTypeIri": "urn:effortless:pko-extension#Exception",
            },
        )


def transform(source: Mapping[str, Any], base_rulebook: Mapping[str, Any]) -> dict[str, Any]:
    rulebook = copy.deepcopy(dict(base_rulebook))
    standard = source.get("standard", {})
    if not isinstance(standard, Mapping):
        standard = {}
    organization = source.get("organization", {"id": "imported-org", "name": "Imported Organization"})
    if not isinstance(organization, Mapping):
        raise RulebookError("Way2 organization must be an object.")
    default_org = ensure_org(rulebook, organization)

    processes = source.get("processes", [])
    if not isinstance(processes, list) or not processes:
        raise RulebookError("Way2 input must contain at least one process.")
    for process in processes:
        if not isinstance(process, Mapping):
            raise RulebookError("Each Way2 process must be an object.")
        import_process(rulebook, process, default_org)

    meta = rulebook.setdefault("_meta", {})
    if isinstance(meta, dict):
        meta.setdefault("adapter_imports", []).append(
            {
                "source_standard": str(standard.get("name") or "way2"),
                "source_version": str(standard.get("version") or "unknown"),
                "adapter": "way2-rulebook-to-pko/1.0.0",
                "pko_target": "https://w3id.org/pko/2.0.0",
            }
        )
    errors = validate_rulebook(rulebook)
    if errors:
        raise RulebookError("Transformed rulebook failed validation:\n- " + "\n- ".join(errors))
    return rulebook


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Convert an example Way2 process model into the canonical ERB-PKO rulebook.")
    parser.add_argument("-i", "--input", required=True, help="Way2 JSON input.")
    parser.add_argument("-o", "--output", required=True, help="PKO rulebook JSON output.")
    parser.add_argument(
        "--base",
        default=str(Path(__file__).resolve().parents[1] / "effortless-rulebook" / "procedural-knowledge-ontology-rulebook.json"),
        help="Canonical ERB-PKO base rulebook to extend.",
    )
    args = parser.parse_args(argv)
    try:
        source = json.loads(Path(args.input).read_text(encoding="utf-8"))
        base_rulebook = load_rulebook(args.base)
        transformed = transform(source, base_rulebook)
        output = Path(args.output)
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(transformed, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        print(f"Wrote PKO rulebook: {output}")
        return 0
    except (OSError, json.JSONDecodeError, RulebookError, ValueError) as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())
