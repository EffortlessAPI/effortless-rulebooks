# Claude Bootstrap Prompt

You are integrating the attached `effortless-math` starter into my EffortlessAPI repositories.

First, inspect the current repository and read its root `CLAUDE.md` in full. The host repository's category boundaries and fail-loud rules take precedence over guesses.

## Intent

Create a new mathematical theorem-network project using the same hub-and-spoke architecture as `effortless-rulebooks`:

- one canonical structural rulebook is the source of truth;
- all SQL, Python, RuleSpeak, OWL, ExplainDAG, spreadsheets, and UI artifacts are projections;
- conformance must prove the projections agree;
- FLT is the deeply modeled flagship consumer theorem;
- seven foundation theorems are first-class provider contracts, initially imported;
- provider theorem status changes must rebuild all consumers.

## Starting package

The payload is in `payload/effortless-math/` and contains:

- `effortless-rulebook/effortless-math-rulebook.json`;
- FLT and seven provider theorem contracts;
- a frozen v21 source archive and SQLite database;
- schemas, trust model, architecture, migration plan, validation scripts, and expected-state tests.

Run `python payload/effortless-math/scripts/validate_starter.py` before editing.

## Required design

1. Prefer a standalone sibling repo named `effortless-math`. A temporary integration under `rulebook-examples/effortless-math` is acceptable for bootstrapping if you document the extraction path.
2. Keep the global theorem catalog and the individual theorem domains categorically distinct but compiled into one dependency graph.
3. Keep theorem contracts versioned and content-addressed.
4. Add a generic provider-certificate resolver; do not hard-code Chebotarev, FLT, or any provider into orchestration.
5. Preserve these proof-status distinctions:
   - IMPORTED
   - DECOMPOSED
   - DERIVED_WITH_IMPORTED_CHILDREN
   - DERIVED_WITH_SHARED_KERNEL
   - FULLY_INTERNALIZED_FOR_SCOPE
   - FALSIFIED
   - SUPERSEDED
   - NOT_EVALUABLE
6. Never infer `FULLY_INTERNALIZED_FOR_SCOPE` from `parent_removed`, finite examples, or a green build.
7. Keep the v21 source archive immutable and preserve all IDs and hashes.
8. Every new example must include `rulebooktorulespeak`, use `ERB_DOMAIN=effortless-math`, read generated values from `vw_*` views, and fail loudly on missing or incompatible inputs.
9. Do not add bespoke caches or locks around `effortless build`.
10. Do not build a custom app until the canonical rulebook and cross-substrate conformance are green.

## First acceptance milestone

From a blank build, reproduce:

```text
8 theorem rows
7 load-bearing FLT provider dependencies
571 loop rows
113 proof-fact rows
305 invariant rows
1 derived contradiction
0 provider theorems fully internalized
0 critical invariant failures
```

The same theorem IDs, statuses, dependency count, and contradiction count must agree across all enabled substrates.

## Deliverables

- integrated project/repo layout;
- migrated canonical rulebook;
- build and conformance commands;
- provider-certificate resolver;
- a migration report listing every transformation from v21;
- tests proving the answer-key counts and row identities;
- explicit list of anything blocked by the current transpiler/schema, with no silent fallback.

Work incrementally. Commit semantic changes as named loops, not as one batch rewrite.
