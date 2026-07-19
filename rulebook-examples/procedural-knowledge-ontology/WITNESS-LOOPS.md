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

## Loop log

### Loop 1 — the founding questions
Status: in progress
The 12 roles each ask 5 questions. Predicates decomposed to atomic.

### Loop 2+ — derivative questions
Planned only after loop 1's answers are materialized and written back.
Each loop records, in its own provenance field, *why* its questions became
askable — naming the loop-N-1 predicates that made them possible.
