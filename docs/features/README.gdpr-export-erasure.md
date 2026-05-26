# GDPR Export and Erasure — Compliance as a Structural Consequence

> **GDPR's right to portability and right to erasure both require knowing
> exactly what the system holds about a person and how it computed it.
> An Effortless project knows this by construction — the DAG shape that
> enables explainability is the same shape the regulation demands.**

---

## What GDPR actually requires

The right to data portability (Article 20) requires that a controller provide,
on request, a structured, commonly-used, machine-readable copy of all personal
data it holds about a subject — not just raw records, but the data "in a format
that allows [the subject] to transmit it to another controller."

The right to erasure (Article 17) requires that the controller delete all
personal data about a subject, across all systems, completely.

Both rights share a prerequisite: the system must be able to answer
"what do we hold about this person, and where?" That question is harder than it
sounds in most applications, because personal data is scattered across tables,
derived by code that was never designed to be audited, and often entangled with
non-personal data in the same records.

---

## Why this is trivial in an Effortless project

An Effortless rulebook declares, for every entity, every field, and every
relationship, exactly what data exists and how it is derived. The DAG is not
reconstructed from running code — it is a build artifact, generated before any
production code runs, that completely describes the information structure.

This has three direct consequences for compliance:

**1. The scope of an account's data is computable from the rulebook.**
Every table that carries a foreign key pointing toward an account entity is
identified in the rulebook's relationship graph. The account-scoped query is
not a bespoke audit script — it is a mechanical traversal of the DAG from
the account node outward.

**2. The export is already live-structured.**
The [Live Excel Export](README.live-excel-export.md) produces a self-contained
workbook with the data and the rules that govern it. That workbook satisfies
the "structured, machine-readable" portability requirement by construction,
not by custom engineering.

**3. Erasure is a DAG traversal, not a manual checklist.**
Because every relationship is declared in the rulebook, every cascade of
a deletion is known at build time. Row-level security (RLS) enforces the
account boundary; the deletion query follows the same graph the export query
does. There is no "did we remember to delete from the audit_log table too?"
question, because the audit graph is explicit.

---

## Row-level security makes the boundary enforcement structural

In a Postgres substrate with RLS enabled, every query runs as a role, and
every policy is derived from the rulebook's account-entity relationships.
The account holder's export runs as that account's role: it returns exactly
the data the RLS policies permit — no more, no less.

This means the account-scoped export and the account-scoped delete share the
same enforcement boundary. The rulebook defines the boundary once. The RLS
policies implement it once. Compliance is not a layer added on top; it is a
consequence of the data model.

---

## What this looks like in practice

A user requests their data. The system:

1. Queries the account-scoped DAG using the account's RLS role
2. Projects the result through the [Live Excel Export](README.live-excel-export.md) substrate
3. Returns a self-contained workbook: data + live formulas + relationship graph

A user requests erasure. The system:

1. Traverses the rulebook's foreign-key graph from the account node
2. Deletes in dependency order (leaves first, root last), enforced by RLS
3. Confirms that no account-scoped data remains — verifiable by re-running the export query, which returns empty

Neither flow requires a bespoke compliance script. Both are structural
consequences of the rulebook.

---

## See also

- [Live Excel Export](README.live-excel-export.md) — the portability artifact; the rulebook's ontology ships with the data
- [React ExplainDAG](README.react-explain-dag.md) — in-app provenance for every value; the same DAG shape surfaced in the UI
- [ExplainDAG — Full Inference Tracing](README.explain-dag.md) — the substrate that generates the witnessed derivation proof underlying all three
- [Hub-and-spoke topology](README.hub-and-spoke.md) — why account-scoped queries and exports follow the same graph as everything else
