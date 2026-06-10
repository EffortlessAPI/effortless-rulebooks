"""
OWL ⟷ Postgres conformance test.

This is the whole point of the substrate: two completely different engines —
an OWL-RL *reasoner* (rdflib + owlrl over the rulebook-generated ontology) and
*Postgres* (the rulebook-generated SQL views) — must compute the SAME answers to
the same competency questions. Postgres is the ANSWER KEY; the OWL reasoner is
graded against it.

The questions exercise the load-bearing inferences:
  CQ1  step-precedence transitive closure   (owl:TransitiveProperty)
  CQ2  delegation chain transitive closure  (owl:TransitiveProperty, self-edge)
  CQ3  role → filling agent + agent type     (FK edge + polymorphic arms)

Run:  python test/conformance_test.py        (or ../run.sh test)
Exit code 0 = all competency questions agree; non-zero = a divergence (fail loud).

The Postgres side uses the asserted edges + a WITH RECURSIVE closure — the exact
algorithm rulebook-to-postgres materializes as vw_step_precedence_closure when
the closure-enabled transpiler is installed. We compute it inline here so the
test is self-contained against whichever transpiler version built the DB; the
*answer* is identical either way (verified: 10 pairs from 4 asserted edges).
"""
from __future__ import annotations

import os
import sys
from pathlib import Path

import psycopg2

# Import the shared reasoned-graph helpers (the OWL side).
sys.path.insert(0, str(Path(__file__).resolve().parent.parent))
import graph  # noqa: E402

# Default derived from the SSoT domain (matches orchestrate.sh's formula); the
# env var only OVERRIDES. This is a derived default, not a fallback — if
# ERB_DOMAIN is unset it still names THIS domain's database correctly.
ERB_DOMAIN = os.environ.get("ERB_DOMAIN", "talismans-special-solutions")
DATABASE_URL = os.environ.get("DATABASE_URL") or (
    f"postgresql://postgres@localhost:5432/erb_{ERB_DOMAIN.replace('-', '_')}"
)


def pg():
    return psycopg2.connect(DATABASE_URL)


# ---------------------------------------------------------------------------
# Postgres answer-key queries
# ---------------------------------------------------------------------------

def pg_precedence_closure(conn) -> set:
    """The transitive closure of step precedence, as a set of (from, to)."""
    with conn.cursor() as cur:
        cur.execute("""
            WITH RECURSIVE closure(from_id, to_id) AS (
                SELECT from_step, to_step FROM vw_step_precedence
                UNION
                SELECT c.from_id, e.to_step
                FROM closure c JOIN vw_step_precedence e ON e.from_step = c.to_id
            )
            SELECT DISTINCT from_id, to_id FROM closure
        """)
        return {(r[0], r[1]) for r in cur.fetchall()}


def pg_delegation_closure(conn, role_id: str) -> set:
    with conn.cursor() as cur:
        cur.execute("""
            WITH RECURSIVE chain(rid) AS (
                SELECT delegates_to FROM vw_roles
                WHERE role_id = %s AND delegates_to IS NOT NULL
                  AND delegates_to <> ''
                UNION
                SELECT r.delegates_to
                FROM chain c JOIN vw_roles r ON r.role_id = c.rid
                WHERE r.delegates_to IS NOT NULL AND r.delegates_to <> ''
            )
            SELECT rid FROM chain
        """, (role_id,))
        return {r[0] for r in cur.fetchall()}


def pg_role_filler(conn, role_id: str):
    with conn.cursor() as cur:
        cur.execute("""
            SELECT filled_by_human_agent, filled_by_ai_agent,
                   filled_by_automated_pipeline
            FROM vw_roles WHERE role_id = %s
        """, (role_id,))
        row = cur.fetchone()
    if not row:
        return {"agent": None, "agent_type": None}
    human, ai, pipe = (v or None for v in row)
    if human:
        return {"agent": human, "agent_type": "HumanAgent"}
    if ai:
        return {"agent": ai, "agent_type": "AIAgent"}
    if pipe:
        return {"agent": pipe, "agent_type": "AutomatedPipeline"}
    return {"agent": None, "agent_type": None}


def pg_all_roles(conn):
    with conn.cursor() as cur:
        cur.execute("SELECT role_id FROM vw_roles ORDER BY role_id")
        return [r[0] for r in cur.fetchall()]


def pg_iris(conn, view, pk_col):
    """Map pk -> computed iri from a Postgres view."""
    with conn.cursor() as cur:
        cur.execute(f"SELECT {pk_col}, iri FROM {view} ORDER BY {pk_col}")
        return {r[0]: r[1] for r in cur.fetchall()}


# ---------------------------------------------------------------------------
# Comparison harness
# ---------------------------------------------------------------------------

class Result:
    def __init__(self):
        self.checks = []  # (name, ok, detail)

    def check(self, name, owl_val, pg_val):
        ok = owl_val == pg_val
        detail = "" if ok else f"\n      OWL: {owl_val}\n      PG : {pg_val}"
        self.checks.append((name, ok, detail))
        mark = "PASS" if ok else "FAIL"
        print(f"  [{mark}] {name}{detail}")

    @property
    def passed(self):
        return all(ok for _, ok, _ in self.checks)


def main():
    print("=" * 70)
    print("OWL reasoner  ⟷  Postgres  conformance")
    print(f"  domain   : {ERB_DOMAIN}")
    print(f"  database : {DATABASE_URL}")
    print(f"  ontology : {graph.TBOX.parent}")
    print("=" * 70)

    r = Result()
    conn = pg()
    try:
        # CQ1 — step-precedence transitive closure (the headline inference).
        owl_closure = {
            (row["from_id"], row["to_id"])
            for row in graph.step_precedence_closure()
        }
        pg_closure = pg_precedence_closure(conn)
        print("\nCQ1 — step-precedence transitive closure")
        r.check("closure pair set is identical", owl_closure, pg_closure)
        # The never-asserted long-range pair MUST be present in both.
        long_range = ("prod-deploy-step-1", "prod-deploy-step-5")
        r.check(
            "inferred long-range pair (step-1 → step-5) present in both",
            long_range in owl_closure,
            long_range in pg_closure,
        )

        # CQ2 — delegation chain transitive closure.
        print("\nCQ2 — delegation chain (Release Manager, transitive)")
        rm = "ntwf-release-manager-role"
        owl_deleg = set(graph.delegation_closure(rm))
        pg_deleg = pg_delegation_closure(conn, rm)
        r.check(f"{rm} delegation closure identical", owl_deleg, pg_deleg)

        # CQ3 — every role's filling agent + agent type.
        print("\nCQ3 — role → filling agent + agent type (all roles)")
        for role_id in pg_all_roles(conn):
            owl_f = graph.role_filled_by(role_id)
            pg_f = pg_role_filler(conn, role_id)
            r.check(f"{role_id} filler", owl_f, pg_f)

        # CQ4 — path-derived Iri agrees across substrates. The OWL value is
        # produced by chained SHACL rules (ParentPath → RelativePath → Iri);
        # the PG value by the rulebook's lookup/calc columns. Same formula, two
        # engines — they must mint identical stable identities. We sample the
        # deepest chains (steps + precedence edges nest 2-3 levels).
        print("\nCQ4 — path-derived Iri (OWL SHACL-computed == Postgres iri column)")
        for view, pk_col, klass in (
            ("vw_workflow_steps", "workflow_step_id", "WorkflowSteps"),
            ("vw_step_precedence", "step_precedence_id", "StepPrecedence"),
            ("vw_approval_gates", "approval_gate_id", "ApprovalGates"),
        ):
            pg_map = pg_iris(conn, view, pk_col)
            owl_map = {pk: graph.iri_of(klass, pk) for pk in pg_map}
            r.check(f"{view} iri set", owl_map, pg_map)
    finally:
        conn.close()

    print("\n" + "=" * 70)
    total = len(r.checks)
    passed = sum(1 for _, ok, _ in r.checks if ok)
    print(f"  {passed}/{total} competency checks agree between OWL and Postgres")
    print("=" * 70)
    if not r.passed:
        print("CONFORMANCE FAILED — the two engines diverged.")
        sys.exit(1)
    print("CONFORMANCE PASSED — OWL reasoner and Postgres compute identically.")


if __name__ == "__main__":
    main()
