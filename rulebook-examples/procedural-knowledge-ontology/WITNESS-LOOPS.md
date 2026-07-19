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

## VERIFIED TRANSPILER DEFECTS (read before writing any formula)

1. **`IIF` is not supported.** It emits a warning comment, returns NULL, and the
   build still reports SUCCESS. Seven committed predicates were silently dead
   this way, and four witnesses read "vacuously false" when in fact the formula
   never ran. Use `IF(cond, a, b)`. `verify_witnesses.sh` now hard-fails on any
   "Formula translation failed" in the generated SQL.
2. **A lookup whose MATCH key is a string literal generates no function** while
   the view that calls it is still emitted — green build, dead database. Match
   on a relationship column.
3. **Multi-criteria `COUNTIFS` silently drops the 2nd+ criteria.** Use the
   composite-key echo: `IF(cond, {{ParentFk}}, "")` on the child, then a
   single-criterion COUNTIFS against that column.
4. `INDEX/MATCH` only matches the target table's primary key.

The lesson generalizes: **a green build is not evidence that a formula ran.**
Always confirm the column discriminates in Postgres.

## Progress

| Role | Predicates | Committed |
|---|---|---|
| finance-analyst | 33 | yes |
| controller | 40 | yes |
| cfo | 40 | yes |
| process-steward | 37 | yes |
| knowledge-authority | 40 | yes |
| employment-counsel | 43 | yes |
| communications-manager | 46 | yes |
| notification-publisher | 44 | yes |
| close-automation | 35 | yes |
| hr-policy-owner | 34 | yes |
| variance-review-agent | 40 | yes |
| policy-drafting-agent | 31 | yes |

**LOOP 1 COMPLETE.** 12 roles, 60 questions, 462 witnessed predicates,
65 tables, 1096 fields, 559 derived, 753 functions.
Non-vacuity: 10 discriminating booleans at baseline -> 135.

## Findings the model surfaced (all from data that was already there)

- `se-close03` closed with `req-close-evidence` (blocking, 7-year retention)
  unsatisfied. Zero satisfaction records existed for it across a completed close.
- Six blocking policy controls — consent, quiet-hours, opt-out, retention,
  human-approval, accessibility — are bound to steps and have NEVER been
  evaluated. They are structurally incapable of failing.
- `vo-close03`: reconciliation reported PASS with no workpaper attached.
- Segregation of duties was prose; now computed, and proven to fire.
- Per the comms spec: no template has ever had an approval record, so every
  send intent computes `IsClearedToSend = FALSE`. The pipeline was never
  authorized to send.

## Deferred to loop 2 (structural, flagged by the drafting agents)

- `OperationalBindings` points at `Steps` (spec) not `StepExecutions` (event), so
  staleness answers "is it stale now" rather than "was it stale when I ran."
  Wrong tense for an attestation.
- `MessageDeliveries.IsDriftedSend` would recompute live, retroactively flipping
  historical sends when a template is re-approved. Delivery-time truth should
  freeze on the delivery row.
- `ChangeRequests.IsOpen` counts `Approved` as open; needs an Implemented/Closed
  terminal state.
- `Recipients.HasSmsConsent` is per-channel-as-column; multi-channel consent
  wants per-channel rows.
- Composite string keys encode joins in delimiter-separated strings. If an id
  ever contains a `|`, the join silently returns wrong answers. Junction tables
  are the principled fix once the COUNTIFS defect is addressed.
- `Steps` has no field expressing what KIND of control a step is, so two
  questions hardcode step ids. A `Steps.ControlKind` column would fix both.

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
