# Live Excel Export — The Ontology Ships With the Data

> **An Effortless Excel export is not a CSV with column headers. It is a live
> spreadsheet where the data and the business logic that defines it — formulas,
> relationships, derivation chains — travel together, because the rulebook
> already is a spreadsheet model.**

---

## What most exports look like

A typical "export to Excel" feature dumps rows and columns. The spreadsheet
arrives flat: the recipient sees numbers, but not what the numbers mean, how
they were derived, or how they relate to anything else. If they want to
recalculate, they rebuild the model from scratch.

This is not an Excel problem. It is a provenance problem: the business logic
never left the server.

---

## What an Effortless export looks like

An ERB rulebook defines fields as typed, named cells — raw values, formulas,
lookups, aggregations. That structure maps directly onto Excel's own model: raw
fields become data cells, formula fields become Excel formulas over those cells,
lookups become cross-sheet references, aggregations become SUMIF/COUNTIF ranges.

The export is not a serialization of results. It is a faithful projection of the
rulebook's ontology into Excel's native format. The recipient gets:

- **The data** — every record, every field, at export time
- **The live formulas** — calculated fields as actual Excel formulas, not frozen
  values, so the spreadsheet recalculates correctly if raw inputs change
- **The relational structure** — related tables on separate sheets, with
  cross-sheet references that preserve the foreign-key graph
- **The derivation chain** — the same shape the ExplainDAG substrate traces,
  now expressed as Excel's own dependency tree

Open the export and change a raw field. The downstream calculated fields
update — because the formula was always there.

---

## Why this is possible with ERB

Most applications separate "the model" from "the data." The model lives in code;
the data lives in a database. Exporting data without the model produces a flat
file. Exporting the model requires custom engineering per export format.

In an Effortless project, the model *is* the rulebook, and the rulebook is
already a typed grid. Excel is a typed grid. The projection is mechanical, not
bespoke. The same transpiler mechanism that generates SQL from the rulebook
generates the Excel workbook structure from the rulebook — the data fills in
afterward.

This means the Excel substrate is a first-class citizen of the conformance
harness: it is testable, deterministic, and provably equivalent to Postgres and
Python for any field it can express.

---

## The account-level variant

For customer-facing exports — an account holder downloading their own data — the
export scope is bounded by row-level security. The query runs as the account,
not as an admin. What comes back is exactly the account's data and the rules
that govern it, nothing more.

This is the same shape as the GDPR portability requirement: a self-contained,
human-readable, machine-processable record of what the system holds about a
person and how it computed it.

---

## See also

- [GDPR Export and Erasure](README.gdpr-export-erasure.md) — how the same DAG shape satisfies the right to portability and the right to erasure
- [ExplainDAG — Full Inference Tracing](README.explain-dag.md) — the derivation graph that the Excel export makes navigable in spreadsheet form
- [Conformance testing](README.conformance.md) — Excel as a conformance substrate, not just a reporting format
- [Hub-and-spoke topology](README.hub-and-spoke.md) — why Excel is a spoke, not a special case
