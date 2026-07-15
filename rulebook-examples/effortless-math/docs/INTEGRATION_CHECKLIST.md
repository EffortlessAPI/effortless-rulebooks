# Integration Checklist

## Before editing

- [ ] Read the host repo root `CLAUDE.md`.
- [ ] Confirm whether this becomes a standalone sibling repo or a temporary `rulebook-examples/effortless-math` domain.
- [ ] Do not mix platform/admin tables into this mathematical domain.
- [ ] Verify the v21 source ZIP and SQLite hashes.
- [ ] Run `scripts/validate_starter.py`.

## Canonical data

- [ ] `effortless-rulebook/effortless-math-rulebook.json` is the sole semantic source of truth.
- [ ] All generated SQL, prose, OWL, UI, and test artifacts are disposable projections.
- [ ] `__meta__` remains a first-class table.
- [ ] Every new theorem or status concept is represented as table data, not a bespoke JSON side key on unrelated field objects.

## Host-repo requirements

- [ ] `effortless.json` contains `rulebooktorulespeak`.
- [ ] Use `ERB_DOMAIN=effortless-math` explicitly for CLI commands.
- [ ] Fail loudly on missing files or incompatible rulebook shapes.
- [ ] Do not add locks around `effortless build`.
- [ ] Do not add ad-hoc caches; materialization must be rulebook data with an invalidation contract.
- [ ] Generated apps read from `vw_*` views and never re-evaluate formulas.

## Mathematical integrity

- [ ] `parent_removed` and `theorem_content_fully_internalized` remain separate.
- [ ] Imported theorem status cannot become PASS through renaming.
- [ ] Conditional branch certificates are not asserted as simultaneously realized facts.
- [ ] Counterfactuals remain in the model and are rebuilt.
- [ ] The contradiction must be derived, never seeded.
- [ ] Provider dependencies bind hypotheses explicitly.
- [ ] A provider update invalidates stale consumer certificates bitemporally.

## Definition of done for the first integration PR

- [ ] Blank build succeeds.
- [ ] RuleSpeak projection succeeds.
- [ ] Postgres projection succeeds.
- [ ] ExplainDAG contains the eight theorem nodes and seven FLT dependencies.
- [ ] 571 loops are present.
- [ ] 113 proof facts are present.
- [ ] 305 invariant rows are present.
- [ ] One contradiction is derived.
- [ ] Seven provider dependencies remain imported.
- [ ] No provider is marked fully internalized.
- [ ] Cross-substrate conformance is green for theorem IDs, statuses, dependency count, and contradiction count.
