# Theme author contract — loops 6 to 11

You own one theme (see `themes.py` and your `claims_<theme>.txt`). You write exactly two
files and nothing else:

- `tools/loops/loopNN_<theme>.py` — the spec, applied by `tools/apply_loop_spec.py`
- `tools/loops/evidence_<theme>.py` — `EVIDENCE = {claim_id: [(kind, target, justification), ...]}`

**Never write the project rulebook, never run `effortless build` in the project, never run
`init-db.sh`.** Validate only with the probe, which works on a scratch copy:

```bash
python3 tools/loops/probe_specs.py --name <theme> tools/loops/loop06_foundation.py tools/loops/loopNN_<theme>.py
```

Iterate until it exits 0. Every theme after A must list `loop06_foundation.py` first,
because A creates the shared scenario data and tables everyone builds on.

## The bar each claim must meet (read ARTICLE-COVERAGE.md)

| Kind | Evidence that counts |
|---|---|
| Concept | `("Table", "T", why)` with rows, or `("Field", "T.F", why)` holding at least one substantive value (not blank, not false, not zero) |
| Prescription, Illustration | `("Field", "T.F", why)` invented for one of your questions AND discriminating (two or more distinct values over the seed data). A boolean that is all-false or all-true does not count. |
| CompetencyQuestion | `("RoleQuestion", "question-id", why)` — the question must own at least one boolean predicate |
| Standard | `("OntologyProfile", "id", why)` with at least one SemanticMappings row, or `("KnowledgeMethod", "Id", why)` applied by a seeded row |
| Scenario | `("Procedure", "id", why)` with at least one execution |

One evidence row must prove the WHOLE claim. The justification says, in plain words, how
that field or question answers that claim. A skeptic reads every justification.

**Anti-party-trick rules.** A witness for a prescription must fire when the prescription
is violated, and the seed data must contain a real violation it fires on, plus rows where
it does not fire. Do not satisfy a prescription with a trivial classifier that merely
restates a column. Do not reuse an unrelated witness because it happens to discriminate.
Existing fields from loops 1-4 may be cited as evidence when they genuinely answer the
claim (search `$SCRATCH/fields.tsv`, column 4 is the question that invented the field),
but you can never list an existing field under your own question.

## Spec format

```python
import sys; from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import raw, calc, agg, lookup, rel, idx

LOOP = {"WitnessLoopId": "loop-07", "LoopNumber": 7, "Title": "...", "Premise": "..."}
ROLES = [ {row for Roles} ]            # only if your questions need a new role
AGENTS = [ {row for Agents} ]
TABLES = [ ("TableName", "description", "subject-area", [fields...]) ]
FIELDS = { "ExistingTable": [fields...] }
RELATE = [ ("Table", "RawField", "TargetTable") ]   # turn a raw column into a reference
QUESTIONS = [ ("q7-...", "asking-role", "question text", "why it matters", ["Table.Field", ...]) ]
ROWS = { "Table": [ {...}, ... ] }      # upserted by primary key
MAPPINGS = [ (id, source_path, kind, iri, relation, profile, notes) ]
```

- Every field your loop adds (except the `<Entity>Id`, `Name`, `SemanticTypeIri`) must be
  listed under one of your questions. The applier refuses orphans.
- Question ids: `q<loop>-<role>-<slug>`. For an article competency question, name the
  question after the claim: `aq-pkm2-q19` and phrase it in the role's own voice.
- New tables lead with `<Entity>Id`, then a calculated `Name`, and end with a raw
  `SemanticTypeIri`. Pass the primary key explicitly to `idx()` when the table name does
  not singularize by dropping one trailing `s` (`Facilities` -> `FacilityId`).
- Seed rows must set `SemanticTypeIri`. Keys are readable slugs (`loto-04`, not UUIDs).
- Fictional organizations only (`acme-*`). Never name a real company in data.
- Time is judged against `EvaluationContexts` row `eval-current` (AsOfInstant
  2026-07-19T13:00:00-05:00). A time-dependent witness uses a lookup of `AsOfInstant`
  through an `EvaluationContext` relationship, never NOW().

## The relationship graph stays acyclic

An ERB rulebook's relationships form a DAG across tables. A relationship that points "back"
(a version naming its first step while every step names its version; a procedure naming a
resource that reaches procedures again through other tables) is refused by
`tools/pko_rulebook_tool.py validate`, which the probe now runs. Put the relationship on the
child side and derive the parent's view of it (`ProcedureVersions.FirstStep` is a MAXIFS over
`Steps.DeclaredFirstStepKey`). Found at merge.

## Formula dialect — verified traps

- No `IIF`, no `ISBLANK`, no `NULLIF`, no bare `LOOKUP`. Blank check: `{{X}} <> ""` / `{{X}} = ""`.
- `IF` produces a value; never nest `IF` as a predicate inside `AND`/`OR`.
- Lookups: `INDEX(T!{{F}}, MATCH({{LocalFk}}, T!{{TId}}, 0))` on the target's primary key only.
- **A blank lookup inside a boolean expression:** when the reference is blank the lookup is
  NULL, and under `erbBlankLogic=coerce` a comparison such as `{{LookedUpFlag}} = FALSE` reads
  TRUE, so an `OR` can fire on rows with no reference at all. Guard with the reference:
  `AND({{Fk}} <> "", {{LookedUpFlag}} = TRUE)`. Found by theme C.
- Every reference is one hop. Need two hops? Add a lookup at the middle table first.
- Aggregations: `COUNTIFS(Child!{{Fk}}, {{Id}})`, multi-criteria COUNTIFS/SUMIFS/MAXIFS work,
  including a literal (`"Night"`, `TRUE`) or a current-row field as the criterion value.
  Self-table aggregations work (`COUNTIFS(Steps!{{ParentStep}}, Steps!{{StepId}})`).
- A conditional key column is the cleanest filtered count:
  `DeviatingRunVersionKey = IF({{HasDeviation}}, {{ProcedureVersion}}, "")`, then
  `COUNTIFS(ProcedureExecutions!{{DeviatingRunVersionKey}}, {{ProcedureVersionId}})`.
- **Performance:** a count over a large or heavily derived table should filter on the raw,
  indexed reference first and put the derived boolean second:
  `COUNTIFS(ClaimEvidence!{{ArticleClaim}}, {{ArticleClaimId}}, ClaimEvidence!{{IsValid}}, TRUE)`.
  A derived key column (`IF({{IsValid}}, {{ArticleClaim}}, "")`) forces Postgres to compute the
  key for every child row of every parent; on the coverage ledger that was 86 seconds versus
  43 milliseconds.
- **A derived empty string is not blank.** `IF(cond, {{X}}, "")` produces an empty string, and a
  MAXIFS over such keys returns one, which every substrate treats as a value: `{{Key}} <> ""` is
  TRUE for it. Test a derived key's presence with a count (`COUNTIFS(Child!{{Fk}}, {{Id}},
  Child!{{Flag}}, TRUE) > 0`), not with a blank check. Blank checks are for raw columns. Found at
  merge (`ProcedureVersions.FirstStepDisagreesWithGraph`).
- **One aggregation per field.** `=COUNTIFS(...) + COUNTIFS(...)` translates in Postgres but the
  Python engine (which is also `compile-rulebook`, the answer-key generator) refuses it and the
  field reads NULL in the answer key while the probe stays green. Put each aggregation in its
  own field and add them in a calculated field. Found at merge (`TacitJudgmentFragmentCount`).
- Strings: `&` or `CONCAT`. `LEFT(x, n)` works. No `a-b` string formulas.
- `DATETIME_DIFF(later, earlier, "days")` is positive. Guard a possibly blank date:
  `IF({{D}} = "", 0, DATETIME_DIFF(...))`.
- Divide safely: `IF({{Den}} = 0, 0, {{Num}} / {{Den}})`. `ROUND(x, n)` takes two args.
- Closure (transitive reachability) is a `closure` field on an edge table: see
  `StepTransitions.LeadsToClosure` and `Steps.ReachableStepCount` in the rulebook.
- **Closure endpoints must be relationship columns.** `rulebook-to-owl` refuses a closure whose
  `FromColumn` or `ToColumn` is anything else ("is not a relationship"), and the CLI reports that
  refusal as SSL errors and a cook timeout. Postgres accepts it, so the Postgres-only probe used to
  pass; `apply_loop_spec.py --check` now refuses it. Mixed node types (steps and agents in one
  closure) cannot be expressed; use a dedicated edge table with relationship columns.
- **Closure over a calculated column:** a plain closure reads the base table, so a
  `FromColumn` or `ToColumn` that is calculated makes the view fail to load ("column does not
  exist"). A closure with an `EdgeFilterColumn` reads the view instead; declare a filter
  (it may be derived) whenever an endpoint is calculated. Found by theme F
  (`StepVariables.ArtifactFlowClosure`).

## Shared facts from theme A (loop06_foundation.py) you can seed against

Read `loop06_foundation.py` itself; its ROWS are the source of truth. It provides the
lockout/tagout procedure (`lockout-tagout`, versions `loto-v1.0.0` deprecated and
`loto-v2.0.0` current, steps `loto-01`..`loto-09` with sub-steps), the production
deployment procedure (`production-deployment`, version `deploy-v3.2.0`, steps
`deploy-01`..`deploy-05`), their executions, facilities `plant-north` and `plant-south`,
and the roles and agents listed there.

## Reporting back

When the probe exits 0, report: the loop id, tables and fields added, questions, how many
claims have evidence, and every claim you could NOT cover with an honest witness and why.
An honest gap is better than a stretched justification.
