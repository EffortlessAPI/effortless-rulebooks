# Witness Loops — role-driven predicate expansion

Working plan for the multi-loop expansion of this rulebook from ~452 fields to
~700-750, where every new field exists because a **named role asked a question**.

This file is the durable plan. It survives context compaction. Read it first.

## The chain

```
Loops -> RoleQuestions -> RulebookFields
  ^          ^                  ^
  |          |                  |
 loop N   asked by a Role   InventedForQuestion FK
```

Every predicate traces to the question that motivated it, to the role that asked
it, to the loop it was invented in. Existing fields carry a null
`InventedForQuestion` — they predate the exercise and we do not fabricate
retroactive motivations for them.

## Decisions (confirmed with the user 2026-07-19, before an unattended run)

| Decision | Answer |
|---|---|
| Quota wall | The quota report is not authoritative. Keep going. |
| Acceptance bar | Postgres-computed AND non-vacuous. A witness that cannot fire is not done. |
| Loop 1 scope | All 12 roles, 5 questions each = 60 questions. |
| Missing seed data | Add the entity, seed real violations to prove the witness fires, **then remediate in-model back to clean.** |
| Violation history | Keep as resolved rows — exception invoked, CR approved, gap Resolved. Green now, with an audit trail showing it fired. |
| Meta->domain link | **Full FK fidelity.** A real `RulebookFields` catalog, not strings. |
| Catalog scope | All fields, existing get null question. |
| Catalog sync | A script reconciles it from the real rulebook every loop and fails loudly on drift. Never hand-maintained. |
| Transpiler bugs | **Decompose** into atomic predicates rather than fight one gnarly formula. 02b is last resort. |
| Parallelism | Heavy multi-agent for *authoring*; serialized rulebook mutation + verification. |
| Commits | One per role. Granular on purpose; squash later if desired. |
| Projectors | Do not update every loop. **Do** update Postgres every loop and extract computed values back into the rulebook. |
| Autonomy | Keep looping until ~250-300 new predicates exist. |

## The write-back rule (load-bearing)

After every loop: build Postgres, compute, then **extract computed values back
into the rulebook**. The rulebook is always fully up to date. This is what makes
loop N+1 genuinely derivative of loop N — questions get planned against
materialized answers, not imagined ones.

Per repo doctrine: the substrate is the oracle. Never recompute a derived value
in Python to "check" it.

## Concurrency hazard

The rulebook is a single JSON file. N agents editing it concurrently clobber each
other regardless of git. So:

- **Parallel**: agents draft question/predicate specs into separate files under
  `.witness-specs/<role>.json`.
- **Serial**: I apply each spec to the rulebook, build, verify, commit.

## Known issues to fix during the run

- `OperationalBindings.IsFresh` is all-false because `AgeMinutes` uses `NOW()`
  against fixed seed timestamps — it decays with wall clock. Vacuous by clock.
  Needs a seeded as-of reference so the witness stays meaningful.
- `RequirementSatisfactions` records a level but nothing flags a step that
  proceeded past an unsatisfied *blocking* requirement.
- `StepVerifications` is spec-side only; no execution-side outcome exists.
- `ProcedureExecutions` has zero derived fields — the parent of the whole
  execution story makes no claim.
- Segregation of duties (`req-close-separation`) is prose; not evaluable.

## The apply protocol (per role — this is the unattended loop)

For each role spec in `.witness-specs/<role>.json`:

1. Apply its predicates to the rulebook via `tools/apply_witness_spec.py`
   (adds RoleQuestions rows + the fields themselves, in dependency order).
2. Create any `needs_table` entities.
3. Seed the violation rows named in `needs_seed` so the witness can fire.
4. `tools/verify_witnesses.sh` — must show the new witnesses DISCRIMINATING.
   Vacuous means not done; fix the formula or the seed.
5. Remediate in-model back to clean: invoke the documented exception, open and
   resolve a KnowledgeGap, approve the ChangeRequest. Keep the resolved rows —
   the arc is the story.
6. `python3 tools/reconcile_field_catalog.py` so the catalog matches reality.
7. Commit, one per role, naming the questions the role can now answer.

## Verification command

```bash
tools/verify_witnesses.sh          # build + reload + non-vacuity report
tools/verify_witnesses.sh --no-build
```

Baseline before loop 1: **496 fields, 79 derived, 0 witnessed, 10 discriminating
booleans / 11 vacuous.** The witnessed count is the number this exercise exists
to move.

## Loop log

### Loop 1 — the founding questions
Status: in progress. 12 roles x 5 questions, ~34 atomic predicates per role
(~410 total, which clears the 250-300 target).

Roles: finance-analyst, controller, cfo | process-steward, knowledge-authority,
hr-policy-owner | employment-counsel, communications-manager,
notification-publisher | close-automation, variance-review-agent,
policy-drafting-agent.

### Loop 2+ — derivative questions
Planned only after loop 1's answers are materialized and written back.
Each loop records, in its own provenance field, *why* its questions became
askable — naming the loop-N-1 predicates that made them possible.
