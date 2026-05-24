# Locally-Designated SSoT

> **The 'answer key' for a conformance run is whichever spoke the user designates — Airtable export, Excel workbook, hand-edited JSON, Postgres dump. The same harness validates Airtable-driven, Excel-driven, and JSON-driven projects.**

SSoT is a per-run designation, not a global property. The rulebook is the portable IR; the SSoT for a given run is whichever substrate the user nominates as the oracle.

---

This is a stub README. The formal source of truth for this feature is row `feature-007` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Conformance testing](README.conformance.md) — the harness that uses the locally-designated SSoT as answer key
- [No privileged substrate](README.substrate-equivalence.md) — why the SSoT designation is not fixed to a specific substrate
- [Ontology Axioms (derived)](../derived/axioms.md) — the "SSoT is locally designated" axiom (ax-003)
