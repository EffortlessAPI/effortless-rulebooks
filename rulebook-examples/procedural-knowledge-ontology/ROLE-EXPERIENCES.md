# Role experiences — the app the video series is filmed in

This is the durable plan for turning the Procedure Register from a read-only console into
an app where **each role has its own experience, and a viewer can do everything they watched
someone do in the video series**. Read it first after any context loss. Written 2026-09-19 as
a plan; **built 2026-09-20 through phase 5** (see "Where it stands" at the end). The sections
between here and there are the design as planned; where the build departed from it, "Where it
stands" says how.

The series is `../../../effortless-videos/series/17-effortless-acme-corp-pko/` (five
episodes; its `SERIES-SPEC.md` is the companion to this file). Each episode mirrors one of
the five source articles without naming it, so a
reader of the articles recognises the argument and everyone else just sees a plant, a
sourcing desk and a release gate. The scripts there are the acceptance spec for this app:
every `Visualization` field names a screen and an action defined below.

## The contract between the series and the app

1. **Anything a person does on screen, the viewer can do.** Every click, sign-in and edit in
   an episode is a real route and a real action in this app, against the seed data a fresh
   `bash init-db.sh` loads. No mock screens, no video-only states.
2. **Every number spoken is a `vw_*` column.** The scripts quote values read from Postgres on
   2026-09-19 (listed per episode below). Re-verify them when the states are captured; if
   seed data moved, the script changes, never the screen.
3. **The story resets.** `bash init-db.sh` returns the database to the modelled instant
   (2026-07-19 13:00 -05:00, `EvaluationContexts`). A viewer who has played through can
   start again with one command.

## What exists today (verified 2026-09-19)

| Thing | State |
|---|---|
| `app/` | Vanilla JS + Vite front end, Express back end. One shell, ten generic "role" tabs (`operator`, `author`, `approver`, `steward`, `publisher`, `auditor`, `admin`, `explorer`, `conformance`, `access`). Reads whole tables through `/api/register` as the signed-in principal. |
| Writes | Only the Access Control console writes (rulebook JSON, then rebuild). All 202 `AccessPolicies` rows are `SELECT`. `tools/generate_access_ddl.py` already emits `FOR INSERT … WITH CHECK` and `FOR UPDATE`; it only ever `GRANT`s `SELECT`. |
| The blueprint | `AppRoleProfiles` (12), `AppNavGroups` (23), `AppRoutes` (149), `AppRouteQuestions` (151), `AppRouteReferences` (315). A complete per-role route map with purpose and layout hints per route, already keyed to the `RoleQuestions` each route answers. **Nothing in `app/` reads it.** |
| Coverage of the blueprint | Only the original twelve roles (finance close, policy notification). The 21 roles added in loops 06 to 14 (plant, engineering, sourcing, knowledge engineer, ontology authority) have answered questions (knowledge-engineer 91, release-manager 28, ontology-authority 26, plant-safety-officer 24, maintenance-technician 18, plant-operations-manager 15, sourcing-manager 6) and no profile, no routes, no app user and no principal. |
| Sign-ins | 10 `AppUsers`, 12 `AccessPrincipals`. None for the plant, engineering or sourcing people the series follows. |
| Article coverage | 895 of 900 claims witnessed (`vw_source_articles`). No page shows it. |
| Conformance | Latest recorded run is `run-20260914-231633` (67,616 cells), which predates loops 06 to 14. It must be re-run before episode 4 films its conformance scene. |

## Target shape

```
/                         sign-in: one card per person, grouped by organization
/<role>/…                 that role's own experience (bespoke for the eight hero roles,
                          generated from AppRoutes for the rest)
/admin/register/…         the six generic views that are today's whole app, READ-ONLY
/admin/coverage           source articles -> claims -> evidence (new)
/admin/witnesses|loops|provenance|route-map   (exist as tabs today)
/explorer/…  /admin/conformance  /admin/access (exist today)
```

Today's `operator / author / approver / steward / publisher / auditor` tabs move under
`/admin/register` unchanged. They stay useful as the neutral, whole-register view; they stop
being the front door.

## Principles (each one is existing doctrine applied here)

- **The view is the contract.** A role page `SELECT`s from `vw_*` as the signed-in principal.
  No client-side filtering to fake a role's scope; the role schema and RLS already do that.
- **Routes are rulebook data.** The router reads `vw_app_routes`, `vw_app_nav_groups` and
  `vw_app_role_profiles`. A route that is not a row does not exist. Bespoke pages register
  against a `route_path`; every other route renders through the generic workspace renderer.
- **Two write paths, and they are different on purpose.**
  - *Instance data* (an execution, an observation, a repository entry, a decision on a change
    request): written live to the Postgres base table, as the principal, through an RLS
    `INSERT`/`UPDATE` policy. The views recompute at once. This is what makes the app
    interactive.
  - *The model* (a formula, a field, a table, a policy): edited in the rulebook, then
    `effortless build && bash init-db.sh`. This path stays long. Episode 2 films it once.
  The fifth source article draws exactly this line (governed schema change versus data
  operations); the app should make it visible rather than blur it.
- **Every write is an `AppActions` row and an `AccessPolicies` row.** No hand-coded endpoint
  per action. The rulebook already flags a write policy with no denial test
  (`AccessPolicies.IsUnwitnessedWrite`); each new write policy ships with a positive and a
  negative `AccessDenialTests` row.
- **Writes are stamped from the modelled instant, never the wall clock.** The server reads
  `vw_evaluation_contexts.as_of_instant` and stamps `StartedAt`, `ObservedAt`, `DecidedAt`
  and the rest from it (plus a per-session offset in minutes so order is preserved). A
  wall-clock stamp would put new rows two months after the instant the witnesses judge
  against and quietly break every `Days…` column.
- **Live writes are a working copy.** They are lost on the next `init-db.sh`, by design (that
  is the reset). Whether a session can also be published back into the rulebook is an open
  decision at the end of this file; nothing below depends on it.
- **No fallbacks.** A route whose view is missing, or an action whose policy is missing,
  raises with the name of what was expected.

## The hero experiences

Eight roles carry the series. Each gets a hand-built home page and the listed actions. All
other roles (including the original twelve) render from `AppRoutes` through the generic
renderer, which is enough for the episode 1 finance scene.

Notation: **reads** = views the page selects from; **action** = `AppActions` row (base table,
operation); **moves** = the witness column the viewer watches change.

### 1. Maintenance Technician — "My Lockout" (Tomas Reyes, Aisha Bello) · episodes 1, 4
- **Home:** my executions (open, paused, done) and "start a lockout" by machine.
- **Runner:** the procedure as a vertical lane. One step open at a time; each step shows its
  requirements, tools, locks, protective equipment, conditions, cues and the knowledge
  fragments attached to it. Child steps (04a, 04b, 04c) nest under 04. The "next" buttons are
  the step's outgoing `vw_step_transitions`, so a fallback (06 to 09) is a visible choice and
  a blocked path is absent rather than greyed.
- **Reads:** `vw_procedure_versions`, `vw_steps`, `vw_step_transitions`, `vw_step_cues`,
  `vw_step_conditions`, `vw_step_lock_requirements`, `vw_step_protective_equipment`,
  `vw_machine_energy_sources`, `vw_procedure_executions`, `vw_step_executions`,
  `vw_knowledge_fragments`, `vw_know_how_carriers`.
- **Actions:** start execution (`ProcedureExecutions` insert); complete step
  (`StepExecutions` insert, `PreviousStepExecution` set by the server from the open run);
  record a cue (`CueObservations` insert); escalate (`CueObservations` update
  `WasEscalated`, `EscalatedToAgent`).
- **Moves:** `vw_step_cues.unescalated_observation_count`,
  `vw_step_executions.is_out_of_specified_order`, `ran_before_prerequisite_completed`.
- **The path that is no longer offered** (episode 4 scene 8): after a cue with
  `SignalsIncompleteStep` is observed on the open step, the normal transition (06 to 07) must
  not be offered, only the fallback (06 to 09). That decision is a rulebook formula, not UI
  logic: a new calculated `StepExecutions.IsBlockedByObservedCue` (the technician's question:
  "may I carry on?"), and the runner offers a `vw_step_transitions` row of kind Normal only
  when it is false. It needs a role question behind it and a seeded execution where it fires.
- **Know-how card** (episode 1): one `vw_know_how_carriers` row rendered as a card: topic,
  holder, held since, in written procedure, transfers, repository entries, days until
  departure, `is_at_risk_of_imminent_loss`.

### 2. Plant Assistant — the "Ask" panel docked in the runner · episodes 1, 4
- **No language model in the app.** The panel offers the questions the book can answer for the
  open step (from `vw_knowledge_query_definitions` plus templated step questions: what is
  next, what does this cue mean, which energy sources apply) and answers by structured query
  over the views, listing the rows it used. That matches `RetrievalMode = StructuredQuery`
  on the recorded answers and runs for any viewer with no API key.
- **History tab:** the 13 recorded `vw_assistant_answers` with `vw_answer_groundings`,
  including the five where the language model did the reasoning and the task failed.
- **Action:** ask (`AssistantAnswers` insert with `DerivationPerformedBy = StructuredQuery`,
  plus its `AnswerGroundings`).
- **Needs:** an `AccessPrincipals` row for `plant-assistant`, scoped to acme-plant.

### 3. Knowledge Engineer — "Capture Workbench" (Sam Adeyemi) · episode 2
- **Home:** the at-risk board (every `vw_know_how_carriers` row, sorted by
  `is_at_risk_of_imminent_loss`, `is_held_only_by_departed`, `days_until_holder_departure`).
- **Sessions:** timeline of `vw_elicitation_sessions` by method, each opening to what it
  produced: `vw_knowledge_fragments`, `vw_critical_incidents`, `vw_concept_ladder_rungs`,
  `vw_repertory_grid_constructs`, `vw_expert_cognitions`, `vw_workflow_view_divergences`.
- **Gaps:** `vw_knowledge_gaps` with cause (`gap_cause`, `is_gatekeeping_or_sabotage`,
  `drawn_out_by_session`, `codified_as_fragment`).
- **Organize:** vocabulary (`vw_vocabulary_terms`, `vw_term_label_variants`), lenses
  (`vw_stakeholder_lenses`, `vw_procedure_lens_views`), brokers (`vw_knowledge_broker_links`).
- **Encode:** one step opened as structure: `vw_step_conditions`, `vw_failure_modes`,
  `vw_decision_points`, `vw_step_cues`.
- **Trace:** any fragment back to session, source material and practitioner
  (`vw_knowledge_traces`, `vw_collected_source_materials`).
- **Actions:** add repository entry (`KnowledgeRepositoryEntries` insert); record transfer
  (`KnowledgeTransfers` insert); add fragment (`KnowledgeFragments` insert); attach cue
  (`StepCues` insert).
- **Moves:** `vw_know_how_carriers.repository_entry_count`, `is_captured`,
  `is_at_risk_of_imminent_loss` on `khc12-tomas-bleed` (true to false with one insert).

### 4. Plant Safety Officer — "Safety Desk" (Lin Zhao) · episodes 4, 5
- **Inbox:** escalated and unescalated cue observations, insights awaiting validation
  (`vw_ai_insight_proposals`), change requests where she is the authority.
- **Actions:** acknowledge an escalation (`CueObservations` update); validate an insight
  (`AiInsightProposals` update); hand a decision up (`ChangeRequests` update
  `AuthorityRole`).
- **Moves:** `vw_change_requests.requester_is_authority` on `cr-enc-loto2-gauge-tap`.

### 5. Plant Operations Manager — "The Floor, This Quarter" (Hana Kowalski) · episodes 1, 3, 5
- **Reads:** `vw_onboarding_records`, `vw_mentorships`, `vw_communities_of_practice`,
  `vw_know_how_carriers` by line, `vw_process_knowledge_levels`,
  `vw_process_level_statements`, change requests awaiting her.
- **Actions:** decide a change request (`ChangeRequests` update `Status`, `DecidedAt`). Marking
  it implemented (`ImplementedAt`) belongs to the steward who makes the change, on the workbench.
- **Moves:** `is_open`, `is_decided`, `is_implemented`, `is_live_decision_backlog`.

### 6. Strategic Sourcing Manager — "What We Still Know" (Claire Dubois) · episode 3
- **Reads:** `vw_provider_engagements`, `vw_knowledge_deliverables`, `vw_knowledge_audits`,
  `vw_knowledge_audit_items`, `vw_sourcing_functions`, `vw_knowledge_workforce_positions`.
- **Page idea:** each sourcing function as two bars, held inside versus held by the provider,
  against the level needed.
- **Scope:** sourcing reads every ACME organization's engagements and audits (episode 3 shows
  the plant's and engineering's contracts from Claire's page) and writes only Home Brands'.
- **Actions:** set the knowledge access clause (`ProviderEngagements` update); require a
  deliverable (`KnowledgeDeliverables` insert); mark one delivered (update `DeliveredAt`).
- **Moves:** `is_knowledge_access_unsecured`, `lacks_knowledge_deliverables`,
  `to_client_required_count`, `to_client_delivered_count` on `pe-hb-meridian`.

### 7. Release Manager — "Release Gate" (Grace Holloway) · episode 5
- **Reads:** `vw_procedure_executions` for `deploy-v3.2.0`, `vw_ai_registry_model_versions`,
  `vw_agent_upgrade_assessments`, `vw_artifact_handoffs_closure`, `vw_term_meaning_changes`,
  `vw_role_assignments` (history, never deleted).
- **Page idea:** the blast radius as a graph walked from the agent: artifacts attributed,
  steps consuming them, roles holding those steps. The ticket's list beside the traversal's.
- **Action:** none required for the series (read-only hero page).

### 8. Ontology Authority and Steward — "Change Board" (Nadia Petrova, Sam Adeyemi) · episode 5
- **Reads:** `vw_model_change_requests`, `vw_change_validation_runs`,
  `vw_change_integrity_checks`, `vw_competency_question_runs` (as a release by question grid),
  `vw_competency_question_reviews`, `vw_rulebook_releases`, `vw_instance_data_versions`,
  `vw_external_dependency_revisions`, `vw_model_expansion_requests`.
- **Actions:** authority review (`ModelChangeRequests` update `AuthorityReviewedAt`,
  `Status`).
- **Moves:** `authority_review_skipped`, `steward_own_change_unreviewed`.

### Admin additions
- **`/admin/coverage`:** `vw_source_articles` (five cards with `coverage_percent`), drilling to
  `vw_article_claims` and `vw_claim_evidence` with each justification. Claims stay the
  paraphrases already in git; no article text.
- **`/admin/context`:** the modelled instant, editable by an administrator
  (`EvaluationContexts` update). Episode 5 moves it to Tomas's last day.
- **`/admin/register/*`:** today's six generic views, read-only.

## Rulebook changes

All authored in the rulebook, then built. New tables lead with their `<Entity>Id`. Run
`tools/reconcile_field_catalog.py` and `tools/check_rulebook_integrity.py` after each.

1. **People who can sign in.** `AppUsers`, `AccessPrincipals`, `PrincipalAssignments`,
   `RoleSchemas`, `RoleSchemaViews` and `FieldGrants` for: tomas-reyes, aisha-bello,
   ken-watanabe (maintenance-technician), sam-adeyemi (knowledge-engineer), lin-zhao
   (plant-safety-officer), hana-kowalski (plant-operations-manager), claire-dubois
   (sourcing-manager), grace-holloway (release-manager), nadia-petrova (ontology-authority),
   plant-copilot-1-2 (plant-assistant), risk-classifier-2-4-1 (change-risk-classifier).
   Row policies scope each to its organization; the knowledge engineer and the ontology
   authority are acme-corp and read across.
2. **Profiles and routes for the new roles.** `AppRoleProfiles` rows with a pitch in the
   role's own voice; `AppRoutes` + `AppRouteQuestions` generated from each role's answered
   `RoleQuestions` (one workspace route per question cluster), then hand-edited for the eight
   hero homes. `AppRoutes.AnswersNoQuestion` must stay false for every new route.
3. **`AppActions` (new table).** `AppActionId`, `Route`, `OwningRole`, `Label`, `TargetTable`,
   `Operation` (Insert | Update), `EditableFields` (one row per field in a child table
   `AppActionFields`, not a delimited list), `Policy` (FK to the `AccessPolicies` row that
   permits it), `WatchedTable` + `WatchedField` (the witness the page highlights after the
   write). Derived: `IsUnpermitted` (no policy), `IsUnwitnessed` (policy has no denial test),
   `WatchedFieldIsWitness` (lookup to `RulebookFields.IsWitness`).
4. **Write policies.** One `AccessPolicies` row per action with `Command` INSERT or UPDATE and
   a `CheckPredicate` (own executions only; own organization; authority role matches
   `app.jwt_role()`). Two `AccessDenialTests` per policy: one refused, one allowed.
5. **`tools/generate_access_ddl.py`:** grant `INSERT`/`UPDATE` on exactly the tables named by
   write policies (today it grants `SELECT` only). Everything else in it already handles
   write commands.

## App changes

**Back end**
- `GET /api/app/shell` — profile, nav groups and routes for the signed-in principal, from
  `vw_app_role_profiles`, `vw_app_nav_groups`, `vw_app_routes`.
- `GET /api/app/route?path=…` — the route row, its questions with `witnessed_answer`, and for
  each question the fields invented for it (`vw_rulebook_fields.invented_for_question`),
  which name the views and columns the generic renderer shows.
- `POST /api/app/action/:actionId` — loads the `vw_app_actions` row, accepts only its
  editable fields, stamps instants from the evaluation context, runs the insert or update
  inside `asPrincipal()`, then returns the watched row before and after. RLS is the control;
  the endpoint adds none.
- `POST /api/admin/story/reset` — runs `bash init-db.sh` (administrators only, streamed like
  the existing access rebuild).

**Front end**
- A small hash router replaces the role/tab switch. `main.js` becomes the shell; today's tab
  renderers move to `register/` untouched.
- `workspace.js` — the generic renderer: route purpose as the heading, each question as a
  panel with its witnessed answer, the rows behind it with witness columns coloured, every
  cell linking to the existing Explorer record view.
- One module per hero experience (`lockout.js`, `ask.js`, `workbench.js`, `safety-desk.js`,
  `floor.js`, `sourcing.js`, `release-gate.js`, `change-board.js`), plus `coverage.js`.
- After any action the page re-selects the watched row and flashes what changed, so the
  result is visible where the viewer is already looking.
- The sign-in page groups people by organization and shows each profile's pitch.

## The story states (one script serves the video and the tests)

`tools/story_states.py` drives the running app over HTTP as each person, in episode order,
and after every step writes the watched rows to `story-states/<NN-name>.json`. The video
stage reads those captures, so no frame can disagree with the app; the same script run with
`--check` is the end-to-end test that everything in the series is still doable.

| State | Who | Action | Watched column (before to after) |
|---|---|---|---|
| 00 | — | fresh `init-db.sh` | `khc12-tomas-bleed.is_at_risk_of_imminent_loss` = true, 73 days |
| 10 | Sam | edit `IsHolderLeavingSoon` 120 to 60, build, reload | at-risk true to false (then revert: back to true) |
| 11 | Sam | add repository entry for `khc12-tomas-bleed` | `is_captured` false to true, at-risk true to false |
| 12 | Sam | record transfer Tomas to Aisha | `transfer_count` 0 to 1 |
| 20 | Claire | set clause on `pe-hb-meridian` | `is_knowledge_access_unsecured` true to false |
| 21 | Claire | require a deliverable, to client | `lacks_knowledge_deliverables` true to false, `to_client_required_count` 0 to 1 |
| 30 | Aisha | start lockout, conveyor 3 | new execution row |
| 31 | Aisha | complete 01 to 05, observe gauge cue at 06 | `cue-loto06-gauge.observation_count` 1 to 2, unescalated 0 to 1 |
| 32 | Aisha | escalate, take fallback 06 to 09 | unescalated 1 to 0 |
| 33 | Lin | acknowledge | inbox empties |
| 40 | Lin | hand decision on `cr-enc-loto2-gauge-tap` to plant-operations-manager | `requester_is_authority` true to false |
| 41 | Hana | approve | `is_decided` false to true; `is_open` stays true |
| 41b | Sam | add the tap as a warning sign on `loto-06` (`StepCues` insert), mark the request implemented | `is_implemented` false to true, `is_open` true to false |
| 42 | Nadia | authority review on `mcr-13` (approved, not yet deployed; never a retroactive review of the already-deployed `mcr-02`) | `authority_review_skipped` true to false |
| 50 | Admin | move the modelled instant to 2026-09-30 | `days_until_holder_departure` 73 to 0, nothing at risk |

## Phases

| # | Phase | Done when |
|---|---|---|
| 1 | Shell, router, admin move, `/admin/coverage` | Existing tabs work under `/admin/register`; coverage shows 895 of 900 from the views; `verify_access_control.sh` still 11/11. |
| 2 | Sign-ins for the eleven new people and agents | Each can sign in; each sees only its organization; denial witnesses extended and green. |
| 3 | Profiles and routes for the new roles; generic renderer | Every `vw_app_routes` row renders; `/admin/route-map` shows zero routes answering no question. |
| 4 | `AppActions`, write policies, grants, action endpoint, instant stamping | State 11 works end to end as Sam and is refused as Aisha; `init-db.sh` resets it. |
| 5 | Hero pages in episode order: lockout + ask, workbench, sourcing, safety desk + floor, release gate + change board, context editor | `story_states.py --check` passes all states. |
| 6 | Re-run conformance over the grown model and record it | A latest `ConformanceRuns` row dated after phase 5. |
| 7 | Capture states for the video stage | `story-states/` committed; episode 1 can film. |

Phases 1 to 3 are read-only and can ship before any write exists.

## Facts the scripts quote (re-verify at capture)

Tomas Reyes: 32.2 years' tenure, know-how held since 1998, departs 2026-09-30, 73 days from
the modelled instant, 0 transfers, 0 repository entries. 13 know-how carriers; 1 at risk of
imminent loss; 2 held only by people who have left (Dmitri Volkov, Leo Marchetti). Walter
Grieg: 45.6 years, left 475 days before the instant, mentored Ken Watanabe. Knowledge
fragments: 3 explicit, 3 implicit, 4 tacit, 2 situated judgment. 13 elicitation sessions, 9
on lockout. Assistant answers: 13; language model did the reasoning in 5, all 5 tasks
failed. Close v1.1.0 mined run: 3 of 8 paths conform. Onboarding to proficiency: Aisha 21
days with captured knowledge, Ken 70 without, Leah Brooks 105 starting from nothing. Home
Brands assembly know-how: needed 3, held inside 0, held by provider 3. Semantic mappings: 61
exact, 81 aligned, 157 extension. Copilot insight: gauge clears after a tap in 9 of 11 runs.
Risk classifier 2.4.1 to 2.5.0: ticket listed 1 affected step, traversal found 3. "Release"
changed meaning after 718 days. PROV-O errata touches 17 mappings. Competency question q07:
correct at 0.9.0 and 0.10.0, wrong at 1.0.0. Coverage: 895 of 900.

## Open decisions

1. **Publishing a session back to the book.** Live writes vanish on reset. If a viewer should
   be able to keep their changes, the principled route is an administrator action that exports
   the changed base rows into the rulebook (the repo-local `oss-postgres-calculated-to-rulebook`
   tool exists) and records an `InstanceDataVersions` row. Not needed for the series; adds a
   second way for the database and the rulebook to disagree if done carelessly.
2. **A quick build path for episode 2's formula edit.** The full project build regenerates
   seven substrates and takes about seven minutes. The film compresses time, but a viewer
   following along waits. `tools/loops/probe_specs.py` already builds Postgres only in
   seconds; exposing that as a documented "quick check" is an option.
3. **Naming the essays on the coverage page and in the episode 5 credits** needs Jessica
   Talisman's explicit yes before anything is published. PKO's own attribution (CC BY 4.0) is
   required regardless and is already in `NOTICE.md`.

## Where it stands (2026-09-20)

Decisions taken with the owner: a session saves to Postgres and is carried into the rulebook by
an explicit admin **Save**; the role experiences are a new React + TypeScript app (`app/mobile`)
and the older console stays as the admin section; floor roles are designed for a phone and desk
roles for a landscape tablet (rendered inside a device frame on a desktop); the Copilot answers
from the book only, with no language model; development runs against Postgres only
(`tools/quick_build.sh`, about 30 seconds) and the other six substrates are rebuilt and graded
once at the end.

| Phase | State |
|---|---|
| 1 Shell, router, admin, coverage | **Built, differently.** The old console was left intact on `:5174` as the admin's register rather than moved under a new router; the new app links to it. Coverage, "what each role may change", the modelled date, and Save/Reset are the new app's `/admin`. |
| 2 Sign-ins | **Built.** 10 new sign-ins, 8 new principals (20 in all), organization scoping through derived `OwnerOrganization` lookups. |
| 3 Profiles and routes; generic renderer | **Partly.** `AppRoleProfiles` carries `Device`, `HomeRoute`, `HomeTitle` for all 20 roles and the app routes on it. Roles without a hand-built home land on a page generated from the tables in their schema. The 149 `AppRoutes` rows of the twelve office roles are still not rendered. |
| 4 `AppActions`, write policies, grants, action endpoint, instant stamping | **Built.** 20 actions, 100 action fields, 17 write policies with column-level grants, one generic endpoint. |
| 5 Hero pages | **Built**, all eight, plus `/admin`. `story_states.py --check`: 28 of 28. |
| 6 Conformance re-run over the grown model | **Not done.** Needs the full `effortless build`; deliberately last. |
| 7 Capture states for the video stage | **Tool built** (`story_states.py` writes `story-states/`); not yet captured for filming. |

Departures from the plan above, all made because the build showed they were right:

- **A step is begun and completed separately** (`act-tech-begin-step` inserts it `InProgress`,
  `act-tech-complete-step` updates it). A warning sign has to attach to a step that is open.
- **Step 06 has three ways forward, not two** (07, "verify once more", and the fallback to 09).
  Once a warning sign is recorded only the fallback is offered. Episode 4 scene 8 says so.
- **The Copilot's answer is composed from the failure mode the warning sign points at**
  (`StepCues.SignalsFailureMode` and `OperatorQuestion`, loop 15), not from a canned sentence.
  The hiss on 04b is deliberately linked to no failure mode, so `IsUnanswerableSign` fires.
- **Nadia reviews `mcr-13`**, which is approved and not yet deployed. A retroactive review of the
  already-deployed `mcr-02` would have been dishonest governance.
- **The decision rule is in the database.** `NOT (requested_by_agent = app.jwt_agent() AND
  decided_at IS NOT NULL)` is the `WITH CHECK` on both authorities' update policy, so Lin approving
  Lin's own request is refused by Postgres even if the app's button were there.

Still open: rendering the office roles' `AppRoutes`; denial tests for the write policies in
`AccessDenialTests` (every action shows `IsUnprovenWrite` until then; `story_states.py` proves
the refusals over HTTP meanwhile); the narrow per-field audit of the new roles' grants (they
read every column of their tables today); phase 6.

## What is read aloud must be on a page (loop 17, 2026-09-20)

Rehearsing the series found seven sentences of narration that were true of the rows and shown
nowhere, so the shot plan drew them beside the app. They are now fields, and pages read them
(`tools/loops/loop17_read_aloud.py`):

- **Register-wide totals are columns of the one current `EvaluationContexts` row**, read through
  `app/mobile/src/ui/register.tsx` (`<Totals>`): knowledge by kind (The procedure tab), who did an
  assistant's reasoning and how it ended (Copilot tab), execution order across every run (under the
  ledger), mappings against PKO (the steward's Against the standard tab). To put another total on
  a page, add an aggregation to that table and name its column in a `<Totals>`; never count rows in
  the client. A sign-in that reads five fragments still reads the total for all twelve, because a
  `calc_*` function runs over the whole dataset and the total is a value on a row it may read.
  The floor roles were granted `EvaluationContexts` for this.
- A total over every row of a table is the TRUE count plus the FALSE count of a boolean that is
  never blank there (`AssistantAnswerCount`, `StepExecutionCount`).
- A cancelled run reads the gap it stopped at and the change that answers it from its own row
  (`ProcedureExecutions.StoppedAtKnowledgeGap` and one-hop lookups), so the technician needs no
  grant on `KnowledgeGaps` or `ModelChangeRequests`.
- The process steward's home is hand-built (`/process-steward/plan-versus-reality`). An
  administrator's home is named in `ADMIN_HOMES` in `tools/seed_role_experiences.py`, not in
  `PROFILES`, which would make the principal a non-administrator.
- `InputKind` `longtext` must be filled before Save; `note` is the long text that may be left
  empty. "Add a warning sign" could be saved blank before this.

The series asserts every number it reads aloud against the page, so its rehearsal
(`effortless-videos/series/17-effortless-acme-corp-pko/shotplan/rehearse.mjs`) fails when a total
moves. The mapping totals move whenever `SemanticMappings` is edited; that failure means "re-read
the page and update the narration", not "the app is broken".
