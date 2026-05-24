# Project Rulebook ≠ Demo Rulebook

> **The platform rulebook describes ERB itself; the demo rulebooks under `rulebook-examples/` describe one business domain each. They never mix — portal config lives only in the platform rulebook; business tables live only in their demo rulebook.**

Mixing platform-meta with business-data couples the wrapper to a domain, which destroys portability. Two file reads, two concerns, never merged.

---

This is a stub README. The formal source of truth for this feature is row `feature-013` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Admin portal ≠ a domain](README.portal-vs-domain.md) — the parallel category boundary at the DB level
- [Rulebook Domains (derived)](../derived/domains.md) — the catalog of demo domains, each with its own rulebook
- [CLAUDE.md](../../CLAUDE.md) — the project-level rules that enforce this split for every agent and contributor
