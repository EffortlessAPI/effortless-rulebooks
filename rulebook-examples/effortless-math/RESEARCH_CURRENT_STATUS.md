# Effortless Math — Current Research Status

This file is the short, current pointer. Read it before the longer historical handoff.

## Repository alignment

As of the latest `main` alignment:

- PR #5 is merged: TSP exact-search and wall-clock provider evidence through the represented 711–912 campaign boundary.
- PR #6 is merged: the long-form `RESEARCH_HANDOFF.md`.
- PR #7 is merged: reconciliation of canonical state and provider-local campaigns through loop 1532.
- No research pull requests remain open.

## Canonical boundary

The canonical generated TSP rulebook remains at:

```text
highest loop       710
loop rows          134
generated views     45
Effortless build   PASS
Postgres/Python    PASS
```

That boundary is deliberate. Provider-local campaign evidence is not silently promoted into the canonical rulebook.

## Where to read

1. `RESEARCH_HANDOFF.md` — full architecture, history, doctrine, and continuation protocol.
2. `../../../research-campaigns/_alignment/README.md` — current reconciliation of GitHub state versus provider-local evidence.
3. `../../../research-campaigns/_alignment/local-artifact-inventory.json` — exact archive names, sizes, and SHA-256 identities for local campaign bundles.
4. `../../../research-campaigns/_alignment/provider-local-result-1033-1532.md` — preserved summary and claim boundary for the persistent-provider campaign.

## Next action

The next substantive work should ingest a small, auditable subset of the orchestration/provider evidence into an appropriate shared rulebook domain and then run the real:

```text
Effortless build
→ generated Postgres
→ peer conformance
→ independent replay
```
