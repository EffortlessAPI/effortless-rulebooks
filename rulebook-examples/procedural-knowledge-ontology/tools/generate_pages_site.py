#!/usr/bin/env python3
"""Generate the static GitHub Pages site under docs/ from the live Postgres views.

Every value on every page is SELECTed from a generated ``vw_*`` view. Nothing is
computed here, nothing is read from the rulebook JSON, and there is no fallback:
a missing view or a missing column raises, naming exactly what was expected.

    python3 tools/generate_pages_site.py

The site is a section registry (see SECTIONS). The first section is ``roles`` —
one page per row of ``vw_roles``. Adding a section means adding a Section row and
a builder function; the landing page and the manifest follow automatically.
"""

from __future__ import annotations

import argparse
import html
import json
import os
import re
from dataclasses import dataclass
from decimal import Decimal
from datetime import date, datetime
from pathlib import Path
from typing import Callable, Iterable, Sequence

import psycopg2
import psycopg2.extras

PROJECT_ROOT = Path(__file__).resolve().parent.parent
DOCS_ROOT = PROJECT_ROOT / "docs"
MANIFEST_PATH = DOCS_ROOT / ".pages-site-manifest.json"

# docs/ns/ is a separate workstream (a published ontology vocabulary). The
# generator never reads, writes, or prunes anything beneath it.
RESERVED_PREFIXES = ("ns/",)

# Closure views materialize transitive closure; they are not entities and must
# never appear in an enumeration of the model's views.
CLOSURE_MARKER = "_closure"

SITE_TITLE = "Procedural Knowledge Ontology"
SITE_TAGLINE = "A rulebook-derived register of procedural knowledge at ACME, LLC."


# --------------------------------------------------------------------------
# Strict view access. No fallbacks: every failure names what was expected.
# --------------------------------------------------------------------------


class ViewContractError(RuntimeError):
    """A view or column the site depends on is not present in the database."""


class ViewReader:
    """Reads the generated ``vw_*`` views, and only those."""

    def __init__(self, dsn: str) -> None:
        self.dsn = dsn
        self.conn = psycopg2.connect(dsn)
        self._catalog = self._load_catalog()

    def _load_catalog(self) -> dict[str, list[str]]:
        catalog: dict[str, list[str]] = {}
        with self.conn.cursor() as cur:
            cur.execute(
                """
                SELECT c.table_name, c.column_name
                  FROM information_schema.columns c
                  JOIN information_schema.views v
                    ON v.table_schema = c.table_schema
                   AND v.table_name = c.table_name
                 WHERE c.table_schema = 'public'
                   AND c.table_name LIKE 'vw\\_%'
                 ORDER BY c.table_name, c.ordinal_position
                """
            )
            for view_name, column_name in cur.fetchall():
                catalog.setdefault(view_name, []).append(column_name)
        if not catalog:
            raise ViewContractError(
                f"expected generated vw_* views in schema 'public' of {self.dsn}; found none. "
                "Load the database before generating the site."
            )
        return catalog

    def entity_views(self) -> list[str]:
        """Every entity view, with the closure views excluded by name."""
        return sorted(n for n in self._catalog if CLOSURE_MARKER not in n)

    def require(self, view: str, columns: Sequence[str]) -> None:
        if CLOSURE_MARKER in view:
            raise ViewContractError(
                f"{view} is a closure view, not an entity view; it must never be read as one"
            )
        available = self._catalog.get(view)
        if available is None:
            raise ViewContractError(
                f"expected view public.{view} in {self.dsn}; it does not exist. "
                f"Views present: {len(self._catalog)}."
            )
        missing = [c for c in columns if c not in available]
        if missing:
            raise ViewContractError(
                f"expected column(s) {', '.join(missing)} on public.{view}; "
                f"it has: {', '.join(available)}"
            )

    def select(
        self,
        view: str,
        columns: Sequence[str],
        *,
        where_column: str | None = None,
        where_value: object = None,
        order_by: Sequence[str] = (),
    ) -> list[dict]:
        needed = list(columns)
        if where_column:
            needed.append(where_column)
        needed.extend(order_by)
        self.require(view, needed)

        select_list = ", ".join(f'"{c}"' for c in columns)
        sql = f'SELECT {select_list} FROM public."{view}"'
        params: list[object] = []
        if where_column:
            sql += f' WHERE "{where_column}" = %s'
            params.append(where_value)
        if order_by:
            sql += " ORDER BY " + ", ".join(f'"{c}"' for c in order_by)

        with self.conn.cursor(cursor_factory=psycopg2.extras.RealDictCursor) as cur:
            cur.execute(sql, params)
            return [dict(r) for r in cur.fetchall()]

    def index_by(
        self, view: str, key_column: str, columns: Sequence[str]
    ) -> dict[object, dict]:
        rows = self.select(view, list({key_column, *columns}))
        return {r[key_column]: r for r in rows}

    def group_by(
        self, view: str, key_column: str, columns: Sequence[str], order_by: Sequence[str] = ()
    ) -> dict[object, list[dict]]:
        rows = self.select(view, list({key_column, *columns}), order_by=order_by)
        grouped: dict[object, list[dict]] = {}
        for r in rows:
            grouped.setdefault(r[key_column], []).append(r)
        return grouped


# --------------------------------------------------------------------------
# Templating layer. Small on purpose, and shared by every section.
# --------------------------------------------------------------------------


def esc(value: object) -> str:
    return html.escape("" if value is None else str(value), quote=True)


BLANK = '<span class="blank" title="no value in the view">&mdash;</span>'


def cell(value: object) -> str:
    """Render one view value. Presentation only; never substitutes a value."""
    if value is None or value == "":
        return BLANK
    if isinstance(value, bool):
        word = "yes" if value else "no"
        return f'<span class="bool bool-{word}">{word}</span>'
    if isinstance(value, Decimal):
        normalized = value.normalize()
        text = format(normalized, "f")
        return esc(text)
    if isinstance(value, (datetime, date)):
        return f'<time datetime="{esc(value.isoformat())}">{esc(value.isoformat())}</time>'
    return esc(value)


def source(view: str, column: str) -> str:
    return f'<span class="source">{esc(view)}.{esc(column)}</span>'


@dataclass
class Metric:
    label: str
    view: str
    column: str
    value: object


def metric_grid(metrics: Iterable[Metric]) -> str:
    items = []
    for m in metrics:
        items.append(
            "<div class=\"metric\">"
            f'<div class="metric-value">{cell(m.value)}</div>'
            f'<div class="metric-label">{esc(m.label)}</div>'
            f"<div class=\"metric-source\">{source(m.view, m.column)}</div>"
            "</div>"
        )
    if not items:
        return ""
    return f'<div class="metric-grid">{"".join(items)}</div>'


def flag_grid(view: str, row: dict, columns: Sequence[tuple[str, str]]) -> str:
    items = []
    for column, label in columns:
        value = row[column]
        state = "on" if value else "off"
        items.append(
            f'<div class="flag flag-{state}">'
            f'<span class="flag-dot"></span>'
            f'<span class="flag-label">{esc(label)}</span>'
            f"{source(view, column)}"
            "</div>"
        )
    return f'<div class="flag-grid">{"".join(items)}</div>'


def data_table(
    headers: Sequence[str], rows: Sequence[Sequence[str]], *, empty: str, classes: str = ""
) -> str:
    if not rows:
        return f'<p class="empty">{esc(empty)}</p>'
    head = "".join(f"<th>{h}</th>" for h in headers)
    body = "".join("<tr>" + "".join(f"<td>{c}</td>" for c in r) + "</tr>" for r in rows)
    cls = f' class="{classes}"' if classes else ""
    return (
        f'<div class="table-wrap"><table{cls}>'
        f"<thead><tr>{head}</tr></thead><tbody>{body}</tbody>"
        "</table></div>"
    )


def definition_list(items: Sequence[tuple[str, str, str, object]]) -> str:
    """items: (label, view, column, value)."""
    parts = []
    for label, view, column, value in items:
        parts.append(
            f"<div class=\"def\"><dt>{esc(label)}{source(view, column)}</dt>"
            f"<dd>{cell(value)}</dd></div>"
        )
    return f'<dl class="deflist">{"".join(parts)}</dl>'


def section_block(anchor: str, title: str, lede: str, body: str) -> str:
    lede_html = f'<p class="lede">{esc(lede)}</p>' if lede else ""
    return (
        f'<section id="{esc(anchor)}">'
        f'<h2><a class="anchor" href="#{esc(anchor)}">{esc(title)}</a></h2>'
        f"{lede_html}{body}</section>"
    )


def note(text: str) -> str:
    return f'<p class="note">{esc(text)}</p>'


def page(
    *,
    depth: int,
    title: str,
    heading: str,
    kicker: str,
    lede: str,
    breadcrumbs: Sequence[tuple[str, str | None]],
    body: str,
    toc: Sequence[tuple[str, str]] = (),
) -> str:
    up = "../" * depth
    crumb_html = " <span>/</span> ".join(
        f'<a href="{esc(href)}">{esc(label)}</a>' if href else f"<span>{esc(label)}</span>"
        for label, href in breadcrumbs
    )
    toc_html = ""
    if toc:
        links = "".join(f'<li><a href="#{esc(a)}">{esc(t)}</a></li>' for a, t in toc)
        toc_html = f'<nav class="toc"><h2>On this page</h2><ul>{links}</ul></nav>'
    return f"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{esc(title)}</title>
<link rel="stylesheet" href="{up}assets/site.css">
</head>
<body>
<header class="site-header">
  <a class="site-mark" href="{up}index.html">{esc(SITE_TITLE)}</a>
  <nav class="crumbs">{crumb_html}</nav>
</header>
<main>
  <div class="page-head">
    <p class="kicker">{esc(kicker)}</p>
    <h1>{esc(heading)}</h1>
    <p class="lede">{esc(lede)}</p>
  </div>
  {toc_html}
  {body}
</main>
<footer class="site-footer">
  <p>Every value on this page was selected from a generated <code>vw_*</code> view of
  <code>erb_procedural_knowledge_ontology</code>. Nothing here is computed by the site
  generator. Regenerate with <code>python3 tools/generate_pages_site.py</code>.</p>
  <p>The Procedural Knowledge Ontology was created by Valentina Anita Carriero, Mario Scrocca,
  Ilaria Baroni, Antonia Azzini and Irene Celino (CC BY 4.0). This project aligns to PKO; it is
  not an official PKO distribution and implies no endorsement.</p>
</footer>
</body>
</html>
"""


# --------------------------------------------------------------------------
# The roles section
# --------------------------------------------------------------------------

ROLE_COLUMNS = [
    "role_id",
    "label",
    "responsibility",
    "organization",
    "organization_type",
    "current_agent",
    "current_agent_kind",
    "current_holder_name",
    "current_assignment",
    "current_assignment_valid_from",
    "specializes_role",
    "seniority_level",
    "role_family",
    "specialized_role_family",
    "preferred_knowledge_form",
    "escalation_backup_role",
    "backup_role_holder",
    "semantic_type_iri",
    "active_assignment_count",
    "currently_covered_assignment_count",
    "departed_assignment_count",
    "count_of_awaited_decisions",
    "approval_step_count",
    "release_approval_step_count",
    "specialization_count",
    "capability_tag_count",
    "compliance_review_tag_count",
    "role_mention_count",
    "unresolved_role_mention_count",
    "unescalated_refusal_count",
    "ungrounded_boundary_count",
    "unauthorized_enforcement_assignment_count",
    "has_no_current_holder",
    "is_vacated_role",
    "has_lost_a_holder",
    "is_non_human_held",
    "is_ungoverned_non_human_role",
    "is_governed_by_lapsed_authority",
    "is_ungoverned_enforcement_role",
    "is_not_housed_in_department",
    "has_specializations",
    "is_senior_variant_not_specialization",
    "has_compliance_review_capability",
    "is_missed_by_phrase_query",
    "has_escalation_backup",
    "has_unfilled_escalation_backup",
    "is_production_release_approver",
]

ROLE_FLAGS = [
    ("has_no_current_holder", "No current holder"),
    ("is_vacated_role", "Vacated role"),
    ("has_lost_a_holder", "Has lost a holder"),
    ("is_non_human_held", "Held by a non-human agent"),
    ("is_ungoverned_non_human_role", "Ungoverned non-human role"),
    ("is_governed_by_lapsed_authority", "Governed by lapsed authority"),
    ("is_ungoverned_enforcement_role", "Ungoverned enforcement role"),
    ("is_not_housed_in_department", "Not housed in a department"),
    ("has_specializations", "Has specializations"),
    ("is_senior_variant_not_specialization", "Senior variant, not a specialization"),
    ("has_compliance_review_capability", "Compliance-review capability"),
    ("is_missed_by_phrase_query", "Missed by phrase query"),
    ("has_escalation_backup", "Has an escalation backup"),
    ("has_unfilled_escalation_backup", "Escalation backup unfilled"),
    ("is_production_release_approver", "Production release approver"),
]

ASSIGNMENT_COLUMNS = [
    "role_assignment_id",
    "role",
    "agent",
    "valid_from",
    "valid_to",
    "status",
    "reason",
    "is_current",
    "is_currently_valid",
    "covers_now",
    "has_departed",
    "is_open_ended",
    "agent_kind",
    "for_procedure_version",
]

QUESTION_COLUMNS = [
    "role_question_id",
    "asking_role",
    "witness_loop",
    "question_text",
    "why_it_matters",
    "answerable_before",
    "predicate_count",
    "is_answered",
    "witnessed_answer",
]

ROUTE_COLUMNS = [
    "app_route_id",
    "owning_role",
    "route_path",
    "route_name",
    "surface",
    "nav_group",
    "nav_order",
    "route_kind",
    "purpose",
    "layout_hints",
    "is_in_nav",
    "is_shared",
    "is_maintainer",
    "question_count",
    "reference_count",
    "answers_no_question",
]

PROFILE_COLUMNS = [
    "app_role_profile_id",
    "role",
    "display_label",
    "role_kind",
    "device",
    "pitch",
    "accent_color",
    "icon_mark",
    "sort_order",
    "route_count",
    "home_route",
    "home_title",
]

PRINCIPAL_COLUMNS = [
    "access_principal_id",
    "domain_role",
    "label",
    "pg_role_name",
    "schema_name",
    "is_administrator",
    "organization_scope",
    "policy_count",
    "grant_count",
    "visible_table_count",
    "has_no_access",
    "is_over_privileged",
]

POLICY_COLUMNS = [
    "access_policy_id",
    "principal",
    "target_table",
    "command",
    "row_predicate",
    "check_predicate",
    "rationale",
    "references_inference",
    "is_write_command",
    "is_unrestricted",
    "is_unwitnessed_write",
    "denial_test_count",
]

SCHEMA_COLUMNS = [
    "role_schema_id",
    "principal",
    "schema_name",
    "search_path",
    "is_sealed",
    "view_count",
    "is_empty_schema",
]


@dataclass
class RoleBundle:
    role: dict
    assignments: list[dict]
    questions: list[dict]
    profile: dict | None
    routes: list[dict]
    principals: list[dict]
    policies: dict[str, list[dict]]
    schemas: dict[str, list[dict]]


def slug_ok(value: str) -> bool:
    return bool(re.fullmatch(r"[A-Za-z0-9._-]+", value))


def build_roles(db: ViewReader, out: "SiteWriter") -> dict:
    roles = db.select("vw_roles", ROLE_COLUMNS, order_by=["label", "role_id"])
    if not roles:
        raise ViewContractError("expected at least one row in public.vw_roles; found none")

    agents = db.index_by(
        "vw_agents",
        "agent_id",
        ["display_name", "agent_kind", "organization", "is_still_engaged"],
    )
    orgs = db.index_by(
        "vw_organizations", "organization_id", ["display_name", "organization_type"]
    )
    nav_groups = db.index_by("vw_app_nav_groups", "app_nav_group_id", ["group_label"])
    loops = db.index_by("vw_witness_loops", "witness_loop_id", ["loop_number", "title"])

    assignments = db.group_by(
        "vw_role_assignments", "role", ASSIGNMENT_COLUMNS, order_by=["valid_from"]
    )
    questions = db.group_by(
        "vw_role_questions", "asking_role", QUESTION_COLUMNS, order_by=["role_question_id"]
    )
    routes = db.group_by(
        "vw_app_routes", "owning_role", ROUTE_COLUMNS, order_by=["nav_order", "route_path"]
    )
    profiles = db.index_by("vw_app_role_profiles", "role", PROFILE_COLUMNS)
    principals = db.group_by("vw_access_principals", "domain_role", PRINCIPAL_COLUMNS)
    policies = db.group_by(
        "vw_access_policies", "principal", POLICY_COLUMNS, order_by=["target_table", "command"]
    )
    schemas = db.group_by("vw_role_schemas", "principal", SCHEMA_COLUMNS)

    route_questions = db.group_by(
        "vw_app_route_questions", "route", ["app_route_question_id", "route", "question"]
    )
    question_text = {
        r["role_question_id"]: r["question_text"]
        for r in db.select("vw_role_questions", ["role_question_id", "question_text"])
    }

    emitted = []
    for role in roles:
        role_id = role["role_id"]
        if not slug_ok(role_id):
            raise ViewContractError(
                f"vw_roles.role_id {role_id!r} is not usable as a URL path segment; "
                "the site emits docs/roles/<role_id>/index.html"
            )
        bundle = RoleBundle(
            role=role,
            assignments=assignments.get(role_id, []),
            questions=questions.get(role_id, []),
            profile=profiles.get(role_id),
            routes=routes.get(role_id, []),
            principals=principals.get(role_id, []),
            policies=policies,
            schemas=schemas,
        )
        out.write(
            f"roles/{role_id}/index.html",
            render_role_page(bundle, agents, orgs, nav_groups, loops, route_questions, question_text),
        )
        emitted.append(role_id)

    out.write("roles/index.html", render_role_index(roles, orgs))
    return {"pages": len(emitted), "keys": emitted}


def render_role_index(roles: list[dict], orgs: dict[object, dict]) -> str:
    headers = [
        f"Role{source('vw_roles', 'label')}",
        f"Organization{source('vw_organizations', 'display_name')}",
        f"Current holder{source('vw_roles', 'current_holder_name')}",
        f"Kind{source('vw_roles', 'current_agent_kind')}",
        f"Active assignments{source('vw_roles', 'active_assignment_count')}",
        f"Awaited decisions{source('vw_roles', 'count_of_awaited_decisions')}",
        f"No holder{source('vw_roles', 'has_no_current_holder')}",
        f"Approval steps{source('vw_roles', 'approval_step_count')}",
    ]
    rows = []
    for r in roles:
        org = orgs.get(r["organization"])
        org_cell = cell(org["display_name"]) if org else cell(r["organization"])
        rows.append(
            [
                f'<a href="{esc(r["role_id"])}/index.html"><strong>{esc(r["label"])}</strong></a>'
                f'<br><code>{esc(r["role_id"])}</code>',
                org_cell,
                cell(r["current_holder_name"]),
                cell(r["current_agent_kind"]),
                cell(r["active_assignment_count"]),
                cell(r["count_of_awaited_decisions"]),
                cell(r["has_no_current_holder"]),
                cell(r["approval_step_count"]),
            ]
        )
    body = section_block(
        "all-roles",
        "Every role in the model",
        "One row per row of vw_roles. Follow a role to its page for the questions it asks, "
        "the people who have held it, the screens it works in, and what it is allowed to see.",
        data_table(headers, rows, empty="vw_roles returned no rows.", classes="role-index"),
    )
    return page(
        depth=1,
        title=f"Roles — {SITE_TITLE}",
        heading="Roles",
        kicker="Section",
        lede=(
            f"{len(roles)} roles, each one a row of vw_roles. A role is who the procedure "
            "expects to act, who is answerable for a decision, and whose questions the model "
            "must be able to answer."
        ),
        breadcrumbs=[("Home", "../index.html"), ("Roles", None)],
        body=body,
    )


def render_role_page(
    b: RoleBundle,
    agents: dict[object, dict],
    orgs: dict[object, dict],
    nav_groups: dict[object, dict],
    loops: dict[object, dict],
    route_questions: dict[object, list[dict]],
    question_text: dict[str, str],
) -> str:
    r = b.role
    org = orgs.get(r["organization"])
    blocks: list[str] = []
    toc: list[tuple[str, str]] = []

    def add(anchor: str, title: str, lede: str, body: str) -> None:
        blocks.append(section_block(anchor, title, lede, body))
        toc.append((anchor, title))

    # --- Identity -----------------------------------------------------
    identity = definition_list(
        [
            ("Role key", "vw_roles", "role_id", r["role_id"]),
            ("Responsibility", "vw_roles", "responsibility", r["responsibility"]),
            (
                "Organization",
                "vw_organizations",
                "display_name",
                org["display_name"] if org else r["organization"],
            ),
            ("Organization type", "vw_roles", "organization_type", r["organization_type"]),
            ("Current holder", "vw_roles", "current_holder_name", r["current_holder_name"]),
            ("Holder kind", "vw_roles", "current_agent_kind", r["current_agent_kind"]),
            ("Current assignment", "vw_roles", "current_assignment", r["current_assignment"]),
            (
                "Holding since",
                "vw_roles",
                "current_assignment_valid_from",
                r["current_assignment_valid_from"],
            ),
            ("Specializes", "vw_roles", "specializes_role", r["specializes_role"]),
            ("Seniority", "vw_roles", "seniority_level", r["seniority_level"]),
            ("Role family", "vw_roles", "role_family", r["role_family"]),
            (
                "Preferred knowledge form",
                "vw_roles",
                "preferred_knowledge_form",
                r["preferred_knowledge_form"],
            ),
            (
                "Escalation backup",
                "vw_roles",
                "escalation_backup_role",
                r["escalation_backup_role"],
            ),
            ("Backup holder", "vw_roles", "backup_role_holder", r["backup_role_holder"]),
            ("Semantic type", "vw_roles", "semantic_type_iri", r["semantic_type_iri"]),
        ]
    )
    add("identity", "Identity", "", identity)

    # --- Role metrics -------------------------------------------------
    metrics = [
        Metric("Active assignments", "vw_roles", "active_assignment_count", r["active_assignment_count"]),
        Metric(
            "Assignments covering now",
            "vw_roles",
            "currently_covered_assignment_count",
            r["currently_covered_assignment_count"],
        ),
        Metric("Departed assignments", "vw_roles", "departed_assignment_count", r["departed_assignment_count"]),
        Metric("Awaited decisions", "vw_roles", "count_of_awaited_decisions", r["count_of_awaited_decisions"]),
        Metric("Approval steps owned", "vw_roles", "approval_step_count", r["approval_step_count"]),
        Metric(
            "Release-approval steps",
            "vw_roles",
            "release_approval_step_count",
            r["release_approval_step_count"],
        ),
        Metric("Specializations", "vw_roles", "specialization_count", r["specialization_count"]),
        Metric("Capability tags", "vw_roles", "capability_tag_count", r["capability_tag_count"]),
        Metric(
            "Compliance-review tags",
            "vw_roles",
            "compliance_review_tag_count",
            r["compliance_review_tag_count"],
        ),
        Metric("Mentions in sources", "vw_roles", "role_mention_count", r["role_mention_count"]),
        Metric(
            "Unresolved mentions",
            "vw_roles",
            "unresolved_role_mention_count",
            r["unresolved_role_mention_count"],
        ),
        Metric(
            "Unescalated refusals",
            "vw_roles",
            "unescalated_refusal_count",
            r["unescalated_refusal_count"],
        ),
        Metric(
            "Ungrounded authority boundaries",
            "vw_roles",
            "ungrounded_boundary_count",
            r["ungrounded_boundary_count"],
        ),
        Metric(
            "Unauthorized enforcement assignments",
            "vw_roles",
            "unauthorized_enforcement_assignment_count",
            r["unauthorized_enforcement_assignment_count"],
        ),
    ]
    add(
        "metrics",
        "Role metrics",
        "Each number is one column of vw_roles, computed by the SQL the transpiler emitted "
        "from the rulebook's formulas.",
        metric_grid(metrics) + flag_grid("vw_roles", r, ROLE_FLAGS),
    )

    # --- Holders ------------------------------------------------------
    holder_rows = []
    for a in b.assignments:
        agent = agents.get(a["agent"])
        holder_rows.append(
            [
                (
                    f'<strong>{esc(agent["display_name"])}</strong><br><code>{esc(a["agent"])}</code>'
                    if agent
                    else cell(a["agent"])
                ),
                cell(a["agent_kind"]),
                cell(a["valid_from"]),
                cell(a["valid_to"]),
                cell(a["status"]),
                cell(a["is_current"]),
                cell(a["covers_now"]),
                cell(a["has_departed"]),
                cell(a["is_open_ended"]),
                cell(a["reason"]),
            ]
        )
    holders = data_table(
        [
            f"Agent{source('vw_agents', 'display_name')}",
            f"Kind{source('vw_role_assignments', 'agent_kind')}",
            f"Valid from{source('vw_role_assignments', 'valid_from')}",
            f"Valid to{source('vw_role_assignments', 'valid_to')}",
            f"Status{source('vw_role_assignments', 'status')}",
            f"Current{source('vw_role_assignments', 'is_current')}",
            f"Covers now{source('vw_role_assignments', 'covers_now')}",
            f"Departed{source('vw_role_assignments', 'has_departed')}",
            f"Open-ended{source('vw_role_assignments', 'is_open_ended')}",
            f"Reason{source('vw_role_assignments', 'reason')}",
        ],
        holder_rows,
        empty="vw_role_assignments has no row whose role is this role.",
        classes="holders",
    )
    add(
        "holders",
        "Who holds it, and when",
        "Role assignments are first-class and time-bounded: the model can answer who held "
        "this role on a given day, not only who holds it today.",
        holders,
    )

    # --- Questions ----------------------------------------------------
    question_blocks = []
    for q in b.questions:
        loop = loops.get(q["witness_loop"])
        loop_label = (
            f'Loop {cell(loop["loop_number"])} &mdash; {esc(loop["title"])}' if loop else BLANK
        )
        answered = q["is_answered"]
        state = "answered" if answered else "unanswered"
        question_blocks.append(
            f'<article class="question question-{state}" id="q-{esc(q["role_question_id"])}">'
            f'<h3>{esc(q["question_text"])}</h3>'
            f'<p class="question-why">{esc(q["why_it_matters"])}</p>'
            '<div class="question-meta">'
            f'<span class="chip">Answered: {cell(answered)}{source("vw_role_questions", "is_answered")}</span>'
            f'<span class="chip">Predicates: {cell(q["predicate_count"])}'
            f'{source("vw_role_questions", "predicate_count")}</span>'
            f'<span class="chip">Answerable before: {cell(q["answerable_before"])}'
            f'{source("vw_role_questions", "answerable_before")}</span>'
            f'<span class="chip">{loop_label}{source("vw_witness_loops", "title")}</span>'
            "</div>"
            f'<div class="witnessed">{source("vw_role_questions", "witnessed_answer")}'
            f'<pre>{esc(q["witnessed_answer"])}</pre></div>'
            f'<p class="question-key"><code>{esc(q["role_question_id"])}</code></p>'
            "</article>"
        )
    questions_body = (
        "".join(question_blocks)
        if question_blocks
        else '<p class="empty">vw_role_questions has no row whose asking_role is this role.</p>'
    )
    questions_body += note(
        "No vw_roles column holds a per-role tally of questions asked or answered, so this "
        "page does not state one. Each question's own vw_role_questions.is_answered column is "
        "shown instead."
    )
    add(
        "questions",
        "The questions this role asks",
        "Each question is in the role's own voice. The witnessed answer is the substrate's "
        "reading of the predicates that were invented to answer it.",
        questions_body,
    )

    # --- App experience -----------------------------------------------
    profile_body = ""
    if b.profile:
        p = b.profile
        profile_body += definition_list(
            [
                ("App label", "vw_app_role_profiles", "display_label", p["display_label"]),
                ("Role kind", "vw_app_role_profiles", "role_kind", p["role_kind"]),
                ("Device", "vw_app_role_profiles", "device", p["device"]),
                ("Pitch", "vw_app_role_profiles", "pitch", p["pitch"]),
                ("Home route", "vw_app_role_profiles", "home_route", p["home_route"]),
                ("Home title", "vw_app_role_profiles", "home_title", p["home_title"]),
            ]
        )
        profile_body += metric_grid(
            [
                Metric("Routes", "vw_app_role_profiles", "route_count", p["route_count"]),
                Metric("Sort order", "vw_app_role_profiles", "sort_order", p["sort_order"]),
            ]
        )
    else:
        profile_body += (
            '<p class="empty">vw_app_role_profiles has no row whose role is this role: '
            "this role has no app experience in the model.</p>"
        )

    route_rows = []
    for rt in b.routes:
        group = nav_groups.get(rt["nav_group"])
        linked = route_questions.get(rt["app_route_id"], [])
        qlinks = "".join(
            f'<li><a href="#q-{esc(rq["question"])}">{esc(question_text.get(rq["question"], rq["question"]))}</a></li>'
            for rq in linked
        )
        route_rows.append(
            [
                f'<code>{esc(rt["route_path"])}</code><br><strong>{esc(rt["route_name"])}</strong>',
                cell(rt["route_kind"]),
                cell(group["group_label"]) if group else cell(rt["nav_group"]),
                cell(rt["nav_order"]),
                cell(rt["purpose"]),
                cell(rt["question_count"]) + (f"<ul class=\"qlinks\">{qlinks}</ul>" if qlinks else ""),
                cell(rt["reference_count"]),
                cell(rt["is_in_nav"]),
                cell(rt["answers_no_question"]),
            ]
        )
    profile_body += data_table(
        [
            f"Route{source('vw_app_routes', 'route_path')}",
            f"Kind{source('vw_app_routes', 'route_kind')}",
            f"Nav group{source('vw_app_nav_groups', 'group_label')}",
            f"Order{source('vw_app_routes', 'nav_order')}",
            f"Purpose{source('vw_app_routes', 'purpose')}",
            f"Questions{source('vw_app_routes', 'question_count')}",
            f"References{source('vw_app_routes', 'reference_count')}",
            f"In nav{source('vw_app_routes', 'is_in_nav')}",
            f"Answers nothing{source('vw_app_routes', 'answers_no_question')}",
        ],
        route_rows,
        empty="vw_app_routes has no row whose owning_role is this role.",
        classes="routes",
    )
    add(
        "app",
        "The app this role works in",
        "The per-role experience is modelled, not hand-built: a profile row, its routes, and "
        "the questions each route exists to answer.",
        profile_body,
    )

    # --- Access -------------------------------------------------------
    access_body = ""
    if b.principals:
        for pr in b.principals:
            pid = pr["access_principal_id"]
            access_body += (
                f'<h3 class="principal-head"><code>{esc(pid)}</code> &mdash; {esc(pr["label"])}</h3>'
            )
            access_body += definition_list(
                [
                    ("Postgres role", "vw_access_principals", "pg_role_name", pr["pg_role_name"]),
                    ("Schema", "vw_access_principals", "schema_name", pr["schema_name"]),
                    (
                        "Administrator",
                        "vw_access_principals",
                        "is_administrator",
                        pr["is_administrator"],
                    ),
                    (
                        "Organization scope",
                        "vw_access_principals",
                        "organization_scope",
                        pr["organization_scope"],
                    ),
                ]
            )
            schema_metrics = []
            for sc in b.schemas.get(pid, []):
                schema_metrics.extend(
                    [
                        Metric("Views in role schema", "vw_role_schemas", "view_count", sc["view_count"]),
                        Metric("Schema sealed", "vw_role_schemas", "is_sealed", sc["is_sealed"]),
                        Metric(
                            "Schema is empty",
                            "vw_role_schemas",
                            "is_empty_schema",
                            sc["is_empty_schema"],
                        ),
                    ]
                )
            access_body += metric_grid(
                [
                    Metric("Row policies", "vw_access_principals", "policy_count", pr["policy_count"]),
                    Metric("Field grants", "vw_access_principals", "grant_count", pr["grant_count"]),
                    Metric(
                        "Visible tables",
                        "vw_access_principals",
                        "visible_table_count",
                        pr["visible_table_count"],
                    ),
                    Metric("No access at all", "vw_access_principals", "has_no_access", pr["has_no_access"]),
                    Metric(
                        "Over-privileged",
                        "vw_access_principals",
                        "is_over_privileged",
                        pr["is_over_privileged"],
                    ),
                    *schema_metrics,
                ]
            )
            policy_rows = [
                [
                    f'<code>{esc(po["target_table"])}</code>',
                    cell(po["command"]),
                    f'<code class="predicate">{esc(po["row_predicate"])}</code>',
                    cell(po["rationale"]),
                    cell(po["denial_test_count"]),
                    cell(po["is_unrestricted"]),
                    cell(po["references_inference"]),
                    cell(po["is_unwitnessed_write"]),
                ]
                for po in b.policies.get(pid, [])
            ]
            access_body += data_table(
                [
                    f"Table{source('vw_access_policies', 'target_table')}",
                    f"Command{source('vw_access_policies', 'command')}",
                    f"Row predicate{source('vw_access_policies', 'row_predicate')}",
                    f"Rationale{source('vw_access_policies', 'rationale')}",
                    f"Denial tests{source('vw_access_policies', 'denial_test_count')}",
                    f"Unrestricted{source('vw_access_policies', 'is_unrestricted')}",
                    f"Calls an inference{source('vw_access_policies', 'references_inference')}",
                    f"Unwitnessed write{source('vw_access_policies', 'is_unwitnessed_write')}",
                ],
                policy_rows,
                empty="vw_access_policies has no row for this principal.",
                classes="policies",
            )
    else:
        access_body = (
            '<p class="empty">vw_access_principals has no row whose domain_role is this role: '
            "this role is not a database principal.</p>"
        )
    add(
        "access",
        "What this role may see",
        "Security is rulebook data. A policy decides which rows; the role's own schema decides "
        "which tables and columns. A field with no grant is absent from the view, not blanked.",
        access_body,
    )

    holder = r["current_holder_name"]
    lede = r["responsibility"] or ""
    kicker = f"Role · {org['display_name'] if org else r['organization']}"
    if holder:
        kicker += f" · held by {holder}"
    return page(
        depth=2,
        title=f"{r['label']} — Roles — {SITE_TITLE}",
        heading=r["label"],
        kicker=kicker,
        lede=lede,
        breadcrumbs=[
            ("Home", "../../index.html"),
            ("Roles", "../index.html"),
            (r["label"], None),
        ],
        body="".join(blocks),
        toc=toc,
    )


# --------------------------------------------------------------------------
# Section registry — the growth seam
# --------------------------------------------------------------------------


@dataclass
class Section:
    slug: str
    title: str
    blurb: str
    build: Callable[["ViewReader", "SiteWriter"], dict]
    index_href: str


SECTIONS: list[Section] = [
    Section(
        slug="roles",
        title="Roles",
        blurb=(
            "One page per role: who holds it and since when, the questions it asks and the "
            "witnessed answer to each, the app routes it owns, and exactly what it is allowed "
            "to see in the database."
        ),
        build=build_roles,
        index_href="roles/index.html",
    ),
]

# Named here so the landing page shows where the site is going without inventing
# a link to a page that does not exist.
PLANNED_SECTIONS: list[tuple[str, str]] = [
    ("Procedures", "The specifications: steps, transitions, requirements, verifications and exceptions."),
    ("Executions", "What actually happened, and every witness that fired against the specification."),
    ("Witness loops", "Each expansion round, its premise, and the questions it made askable."),
    ("Conformance", "Every installed substrate, graded cell by cell against the same answer keys."),
    ("Access control", "The principals, policies, grants and denial witnesses as one picture."),
    ("PKO alignment", "Exact, aligned and extension mappings for every table in the model."),
]


# --------------------------------------------------------------------------
# Writing the site
# --------------------------------------------------------------------------


class SiteWriter:
    """Writes files under docs/, and prunes only what a previous run emitted."""

    def __init__(self, root: Path, manifest_path: Path) -> None:
        self.root = root
        self.manifest_path = manifest_path
        self.written: list[str] = []
        self.previous: list[str] = self._read_manifest()

    def _read_manifest(self) -> list[str]:
        if not self.manifest_path.exists():
            return []
        data = json.loads(self.manifest_path.read_text(encoding="utf-8"))
        return list(data["files"])

    def _check(self, rel: str) -> None:
        if rel.startswith("/") or ".." in Path(rel).parts:
            raise ValueError(f"refusing to write outside docs/: {rel}")
        for reserved in RESERVED_PREFIXES:
            if rel.startswith(reserved):
                raise ValueError(
                    f"docs/{reserved} is reserved for another workstream; refusing to write {rel}"
                )

    def write(self, rel: str, text: str) -> None:
        self._check(rel)
        path = self.root / rel
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text, encoding="utf-8")
        self.written.append(rel)

    def finish(self) -> list[str]:
        """Remove files this generator emitted last run and did not emit now."""
        stale = [f for f in self.previous if f not in set(self.written)]
        removed = []
        for rel in stale:
            try:
                self._check(rel)
            except ValueError:
                continue
            path = self.root / rel
            if path.exists():
                path.unlink()
                removed.append(rel)
            parent = path.parent
            while parent != self.root and parent.is_dir() and not any(parent.iterdir()):
                parent.rmdir()
                parent = parent.parent
        self.manifest_path.parent.mkdir(parents=True, exist_ok=True)
        self.manifest_path.write_text(
            json.dumps(
                {
                    "generator": "tools/generate_pages_site.py",
                    "files": sorted(self.written),
                },
                indent=1,
                ensure_ascii=False,
            )
            + "\n",
            encoding="utf-8",
        )
        return removed


def render_landing(db: ViewReader, results: dict[str, dict]) -> str:
    entity_views = db.entity_views()
    cards = []
    for s in SECTIONS:
        count = results[s.slug]["pages"]
        badge = f'<span class="card-count">{count} pages</span>'
        cards.append(
            f'<a class="card card-live" href="{esc(s.index_href)}">'
            f"<h3>{esc(s.title)}{badge}</h3><p>{esc(s.blurb)}</p>"
            '<span class="card-cta">Open &rarr;</span></a>'
        )
    for title, blurb in PLANNED_SECTIONS:
        cards.append(
            f'<div class="card card-planned"><h3>{esc(title)}'
            '<span class="card-count">planned</span></h3>'
            f"<p>{esc(blurb)}</p></div>"
        )

    body = section_block(
        "sections",
        "Sections",
        "The site is generated section by section. Live sections are links; the rest name "
        "what this documentation will carry as it grows.",
        f'<div class="cards">{"".join(cards)}</div>',
    )
    body += section_block(
        "contract",
        "How this site is built",
        "",
        "<p>The rulebook is the single source of truth. <code>effortless build</code> compiles it "
        "into a Postgres schema in which every rulebook formula becomes a column of a generated "
        f"<code>vw_*</code> view &mdash; {len(entity_views)} entity views in this model. This site "
        "reads those views and nothing else: no value on any page was computed by the site "
        "generator, and no value was read from the rulebook JSON. Each metric carries the "
        "<code>view.column</code> it came from, so any number here can be checked with one "
        "<code>SELECT</code>.</p>"
        "<pre class=\"cmd\">python3 tools/generate_pages_site.py</pre>",
    )
    return page(
        depth=0,
        title=SITE_TITLE,
        heading=SITE_TITLE,
        kicker="Effortless Rulebook project",
        lede=SITE_TAGLINE,
        breadcrumbs=[("Home", None)],
        body=body,
    )


STYLESHEET = """/* Generated by tools/generate_pages_site.py — edit the generator, not this file. */
:root {
  --ink: #16181d;
  --ink-soft: #545a66;
  --ink-faint: #8b93a1;
  --rule: #e2e5ea;
  --bg: #ffffff;
  --bg-soft: #f6f7f9;
  --accent: #1f5fd0;
  --good: #167a4a;
  --bad: #b23030;
  --mono: ui-monospace, SFMono-Regular, "SF Mono", Menlo, Consolas, monospace;
}
* { box-sizing: border-box; }
html { -webkit-text-size-adjust: 100%; }
body {
  margin: 0;
  background: var(--bg);
  color: var(--ink);
  font: 16px/1.6 -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
}
a { color: var(--accent); text-decoration: none; }
a:hover { text-decoration: underline; }
code, pre { font-family: var(--mono); font-size: 0.86em; }

.site-header {
  display: flex; flex-wrap: wrap; gap: 0.5rem 1.25rem; align-items: baseline;
  padding: 0.9rem 2rem; border-bottom: 1px solid var(--rule); background: var(--bg-soft);
  position: sticky; top: 0; z-index: 10;
}
.site-mark { font-weight: 650; color: var(--ink); letter-spacing: -0.01em; }
.crumbs { color: var(--ink-faint); font-size: 0.85rem; }
.crumbs span { margin: 0 0.15rem; }

main { max-width: 1120px; margin: 0 auto; padding: 2.5rem 2rem 4rem; }
.page-head { border-bottom: 1px solid var(--rule); padding-bottom: 1.5rem; margin-bottom: 2rem; }
.kicker { margin: 0 0 0.4rem; font-size: 0.78rem; letter-spacing: 0.08em;
  text-transform: uppercase; color: var(--ink-faint); }
h1 { margin: 0 0 0.6rem; font-size: 2.1rem; line-height: 1.15; letter-spacing: -0.02em; }
h2 { font-size: 1.3rem; letter-spacing: -0.01em; margin: 0 0 0.4rem; }
h3 { font-size: 1.02rem; margin: 0 0 0.35rem; }
.lede { margin: 0; color: var(--ink-soft); max-width: 68ch; }
section { margin: 2.75rem 0; }
section > h2 .anchor { color: var(--ink); }

.toc { background: var(--bg-soft); border: 1px solid var(--rule); border-radius: 8px;
  padding: 0.9rem 1.2rem; }
.toc h2 { font-size: 0.78rem; text-transform: uppercase; letter-spacing: 0.08em;
  color: var(--ink-faint); margin-bottom: 0.4rem; }
.toc ul { list-style: none; margin: 0; padding: 0; display: flex; flex-wrap: wrap; gap: 0.35rem 1.1rem; }

.source { display: block; font-family: var(--mono); font-size: 0.68rem;
  color: var(--ink-faint); font-weight: 400; letter-spacing: 0; margin-top: 0.15rem;
  white-space: nowrap; }
.metric-source .source, .flag .source { white-space: normal; overflow-wrap: anywhere; }
.blank { color: var(--ink-faint); }

.deflist { display: grid; grid-template-columns: repeat(auto-fill, minmax(270px, 1fr));
  gap: 0.9rem 1.5rem; margin: 1.1rem 0 0; }
.deflist dt { font-size: 0.8rem; font-weight: 600; color: var(--ink-soft); }
.deflist dd { margin: 0.2rem 0 0; }

.metric-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(190px, 1fr));
  gap: 0.75rem; margin: 1.1rem 0; }
.metric { border: 1px solid var(--rule); border-radius: 8px; padding: 0.8rem 0.9rem; }
.metric-value { font-size: 1.55rem; font-weight: 600; letter-spacing: -0.02em; line-height: 1.1; }
.metric-label { font-size: 0.82rem; color: var(--ink-soft); margin-top: 0.2rem; }
.metric-source { margin-top: 0.3rem; }

.flag-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
  gap: 0.4rem 1rem; margin: 1.2rem 0 0; }
.flag { display: grid; grid-template-columns: 0.7rem 1fr; column-gap: 0.55rem;
  align-items: start; font-size: 0.85rem; }
.flag .source { grid-column: 2; }
.flag-dot { width: 0.6rem; height: 0.6rem; border-radius: 50%; margin-top: 0.42rem;
  background: var(--rule); }
.flag-on .flag-dot { background: var(--bad); }
.flag-on .flag-label { font-weight: 600; }
.flag-off .flag-label { color: var(--ink-soft); }

.bool { font-weight: 600; }
.bool-yes { color: var(--good); }
.bool-no { color: var(--ink-faint); font-weight: 400; }

.table-wrap { overflow-x: auto; margin: 1.1rem 0; border: 1px solid var(--rule); border-radius: 8px; }
table { border-collapse: collapse; width: 100%; font-size: 0.86rem; }
th, td { text-align: left; padding: 0.55rem 0.7rem; border-bottom: 1px solid var(--rule);
  vertical-align: top; }
th { background: var(--bg-soft); font-size: 0.78rem; font-weight: 600; color: var(--ink-soft);
  white-space: nowrap; }
tbody tr:last-child td { border-bottom: 0; }
tbody tr:hover { background: #fafbfc; }
td .predicate { display: block; max-width: 40ch; white-space: pre-wrap; word-break: break-word; }
.routes td:nth-child(5) { max-width: 34ch; }
.qlinks { margin: 0.35rem 0 0; padding-left: 1rem; font-size: 0.82em; }

.question { border: 1px solid var(--rule); border-left: 3px solid var(--rule);
  border-radius: 8px; padding: 1rem 1.15rem; margin: 0.85rem 0; }
.question-answered { border-left-color: var(--good); }
.question-unanswered { border-left-color: var(--bad); }
.question h3 { font-size: 1.02rem; }
.question-why { margin: 0.35rem 0 0.7rem; color: var(--ink-soft); font-size: 0.9rem; max-width: 76ch; }
.question-meta { display: flex; flex-wrap: wrap; gap: 0.45rem; }
.chip { background: var(--bg-soft); border: 1px solid var(--rule); border-radius: 999px;
  padding: 0.22rem 0.65rem; font-size: 0.78rem; }
.chip .source { display: inline; margin-left: 0.4rem; }
.witnessed { margin-top: 0.8rem; }
.witnessed pre { background: var(--bg-soft); border: 1px solid var(--rule); border-radius: 6px;
  padding: 0.6rem 0.75rem; margin: 0.25rem 0 0; overflow-x: auto; white-space: pre-wrap;
  word-break: break-word; }
.question-key { margin: 0.6rem 0 0; color: var(--ink-faint); }
.principal-head { margin-top: 1.8rem; }

.empty, .note { color: var(--ink-soft); font-size: 0.88rem; background: var(--bg-soft);
  border: 1px solid var(--rule); border-radius: 6px; padding: 0.6rem 0.8rem; }
.note { border-left: 3px solid var(--ink-faint); }

.cards { display: grid; grid-template-columns: repeat(auto-fill, minmax(290px, 1fr));
  gap: 1rem; margin-top: 1.2rem; }
.card { display: block; border: 1px solid var(--rule); border-radius: 10px; padding: 1.1rem 1.2rem;
  color: var(--ink); }
.card h3 { display: flex; align-items: baseline; justify-content: space-between; gap: 0.6rem; }
.card p { margin: 0; color: var(--ink-soft); font-size: 0.89rem; }
.card-count { font-size: 0.72rem; text-transform: uppercase; letter-spacing: 0.06em;
  color: var(--ink-faint); font-weight: 500; white-space: nowrap; }
.card-live:hover { border-color: var(--accent); text-decoration: none; }
.card-cta { display: inline-block; margin-top: 0.7rem; font-size: 0.85rem; color: var(--accent); }
.card-planned { background: var(--bg-soft); border-style: dashed; }
.card-planned h3 { color: var(--ink-soft); }

.cmd { background: var(--bg-soft); border: 1px solid var(--rule); border-radius: 6px;
  padding: 0.6rem 0.8rem; overflow-x: auto; }
.role-index td:first-child { white-space: nowrap; }

.site-footer { border-top: 1px solid var(--rule); background: var(--bg-soft);
  padding: 1.6rem 2rem 2.4rem; color: var(--ink-faint); font-size: 0.82rem; }
.site-footer p { max-width: 90ch; margin: 0 auto 0.6rem; }

@media (max-width: 640px) {
  main { padding: 1.75rem 1.1rem 3rem; }
  .site-header { padding: 0.8rem 1.1rem; }
  h1 { font-size: 1.6rem; }
}
"""


def main() -> None:
    # GitHub Pages serves one directory per repository, and this project is one of several
    # in it, so the published tree is named on the command line. The default is the
    # project's own docs/, which keeps the project self-contained; publication passes the
    # repository's Pages root. There is one generator either way -- never a second copy
    # kept in step by hand.
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--out", type=Path, default=DOCS_ROOT,
                    help="directory to write the site into (default: this project's docs/)")
    args = ap.parse_args()

    dsn = (
        os.environ.get("DATABASE_URL")
        or "postgresql://postgres@localhost:5432/erb_procedural_knowledge_ontology"
    )
    db = ViewReader(dsn)
    out = SiteWriter(args.out, args.out / MANIFEST_PATH.name)

    out.write("assets/site.css", STYLESHEET)
    out.write(".nojekyll", "")

    results: dict[str, dict] = {}
    for s in SECTIONS:
        results[s.slug] = s.build(db, out)

    out.write("index.html", render_landing(db, results))
    removed = out.finish()

    print(f"read {len(db.entity_views())} entity views from {dsn}")
    for s in SECTIONS:
        print(f"section {s.slug}: {results[s.slug]['pages']} pages")
    print(f"wrote {len(out.written)} files under {args.out}")
    if removed:
        print(f"pruned {len(removed)} stale file(s): {', '.join(removed)}")


if __name__ == "__main__":
    main()
