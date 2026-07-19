# Onboarding record — prototype → first-class domain

This folder began as a self-contained PKO prototype prepared outside the repo. It has since been onboarded as a first-class domain under `rulebook-examples/`. This file records what changed, so the next reader does not have to reconstruct it from the diff.

The semantic content of the rulebook was **preserved, not refactored.** The prototype's validator, projectors, adapter, tests, and generated artifacts were all kept.

## What changed

| Change | Why |
|---|---|
| Folder renamed `process-knowledge-ontology` → `procedural-knowledge-ontology` | Matches PKO's actual name, and the slug is the naming contract every discovery surface keys on |
| Rulebook renamed `pko-rulebook.json` → `procedural-knowledge-ontology-rulebook.json` | Repo contract is `<slug>-rulebook.json`; without it, orchestration and the admin portal silently skip the domain |
| Added `effortless.json` | Registers the project and its `rulebook-to-rulespeak` transpiler (mandatory for every demo) |
| Added `__meta__` table to the rulebook | Canonical home for project-level metadata (tagline, motif, PKO version contract, use cases, signature rows) |
| Added `start.sh` | Every domain boots the same way. No server here, so it validates → regenerates projections → runs the adapter → tests |
| Added `CLAUDE.md` | Domain-level agent guidance, including the exact/aligned/extension rule |
| README: added `start.sh` + `effortless build` usage and the local transpiler bus block | Repo convention for every demo README |
| `tools/pko_rulebook_tool.py`: exempted `__meta__` from the `*Id` primary-key and calculated-`Name` checks | `__meta__` is a repo-standard structure keyed by `MetaKey`, not a domain entity. The exemption list already existed for `ERBVersions`/`ERBCustomizations`; the `*Id` check was also fixed to actually honor it |

## Where the domain is registered

- `rulebook-examples/README.md` — domain section
- `effortless-platform/effortless-rulebook/effortless-rulebook.json`:
  - `RulebookDomains` → `domain-028`
  - `RulebookFlavors` → `flav-028` (hand-authored; `Flavor` set to `graph-ontology` rather than the reconciler's `computation-heavy` guess, which over-counts `Name` aliases)
  - `FlavorTags` → 5 rows

Everything else in the repo discovers the domain dynamically by globbing `rulebook-examples/*/`.

## Still open

- No `effortless build` has been run here yet, so `rulespeak/` does not exist. Run it to produce the plain-English rendering.
- No Postgres substrate. The rulebook is 50 tables with dense FK structure; wiring `rulebook-to-postgres` is a deliberate next step, not an oversight.
- `RulebookDomains` in the platform rulebook is stale for *other* domains (it still lists demos that moved to `toy-rulebooks/` and omits several current ones). `domain-028` is correct; the surrounding table deserves its own cleanup pass.
