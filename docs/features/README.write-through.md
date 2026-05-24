# Write-Through Invariant

> **The portal's editor Postgres DB is live; the rulebook JSON is durable. Every save writes to BOTH in the same logical transaction — drop Postgres at any time and rebuild from JSON, never the other direction.**

The rulebook JSON is the only file that travels with the repo. Postgres is convenience; JSON is truth. Lazy sync is forbidden.

---

This is a stub README. The formal source of truth for this feature is row `feature-009` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Portal/CLI parity](README.portal-cli-parity.md) — the complementary guarantee that portal and CLI use the same pipeline
- [Admin portal ≠ a domain](README.portal-vs-domain.md) — why the portal has its own DB separate from domain data
- [Ontology Axioms (derived)](../derived/axioms.md) — the "write-through invariant" axiom (ax-010)
