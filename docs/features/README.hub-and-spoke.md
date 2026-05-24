# Rulebook as IR / Hub-and-Spoke Topology

> **The rulebook JSON is the hub; every input (Airtable, LLM, admin portal, hand edits) and every output (Postgres, Python, Go, Excel, OWL, …) is a spoke. Spokes never talk to each other — they all route through the rulebook, eliminating the n×n integration problem.**

The topology IS the architecture: one durable IR, N peer spokes. Generated artifacts are projections of the rulebook, never accumulations.

---

This is a stub README. The formal source of truth for this feature is row `feature-003` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [No privileged substrate](README.substrate-equivalence.md) — every spoke is a peer
- [Convergent builds](README.convergent-build.md) — why spokes stay in sync with the hub
- [What is non-linguistic?](../what-is-non-linguistic.md) — why structural encoding makes the hub pattern tractable
- [Execution Substrates (derived)](../derived/substrates.md) — the full spoke catalog
