# 📘 cmcc-core-r0 — RuleSpeak®

_The runnable companion to the CMCC-Core Representation Theorem, retrofitted into ERB shape: the R0 warehouse-network spec's own structure, its conformance matrix (answer key x reflection), and the resulting per-substrate roll-ups, all as rulebook data._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **EDB Relation** | The R0 spec's extensional database relations (r0/spec.json's 'edb' map): the fixed vocabulary every rule and command is built from. | — |
| Fields | A defined attribute. | _JSON array of the relation's field names, verbatim from spec.json (e.g. '["id"]')._ |
| Field Count | A defined attribute. | _Number of fields, counted once at transcription time._ |
| Description | A defined attribute. | _What this EDB relation models in the warehouse-network domain._ |
| Name | The same as its relation ID. | _Display alias._ |
| Seed Facts | A defined attribute. | _h0 seed facts whose Relation is this EDB relation._ |
| Transition Effects | A defined attribute. | _Transition effects that assert/retract into this relation._ |

<details open>
<summary>Example data</summary>

| Fields | Field Count | Description | Name ƒ |
|---|---|---|---|
| ["id"] | 1 | A node in the network (the hub, satellite depots, or ordinary warehouses). | Warehouse |
| ["src", "dst", "cap"] | 3 | A directed shipping lane between two warehouses with a capacity. | Route |
| ["id", "wh", "day"] | 3 | A shipment currently sitting at a warehouse on a given day. | Shipment |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Command** | The R0 spec's commands (r0/spec.json's 'commands' map): the only way an input history can act on the state. | — |
| Params | A defined attribute. | _JSON array of the command's parameter names, verbatim from spec.json._ |
| Param Count | A defined attribute. | _Number of parameters, counted once at transcription time._ |
| Description | A defined attribute. | _What issuing this command means in the warehouse-network domain._ |
| Name | The same as its command ID. | _Display alias._ |
| Transitions | A defined attribute. | _Transitions this command can fire._ |

<details open>
<summary>Example data</summary>

| Params | Param Count | Description | Name ƒ |
|---|---|---|---|
| ["s", "d", "c"] | 3 | Open a new directed route s -> d with capacity c. | AddRoute |
| ["s", "d"] | 2 | Close (retract) an existing route s -> d. | CloseRoute |
| ["id", "newday", "at"] | 3 | Bitemporal correction: retract the recorded day for a shipment and re-assert a new day, both visible at knowledge time 'at'. | CorrectShipment |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rule** | Every stratified Datalog-with-negation-and-aggregation rule in the R0 spec (r0/spec.json's 'rules' array), including guard/effect/event rules for transitions and the two integrity-constraint rules. | — |
| Head Predicate | A defined attribute. | _The rule's head predicate name (head[0] in spec.json), e.g. 'Reachable'._ |
| Kind | A defined attribute. | _'rule' for a body-clause rule, 'aggregation' for an agg-clause rule (spec.json's 'agg' key)._ |
| Head JSON | A defined attribute. | _The rule's head tuple, verbatim JSON from spec.json._ |
| Body JSON | A defined attribute. | _The rule's body clause list, verbatim JSON from spec.json (null for aggregation rules)._ |
| Agg JSON | A defined attribute. | _The rule's agg clause, verbatim JSON from spec.json (null for body rules)._ |
| Paper Section Ref | A defined attribute. | _CMCC-Core Representation Theorem section(s) this rule exercises._ |
| Description | A defined attribute. | _What the rule computes and, for reflection.py, which G_R node it constructs._ |
| Name | The same as its rule ID. | _Display alias._ |
| Guarded Transitions | A defined attribute. | _Transitions whose Guard is this rule._ |
| Transition Effects | A defined attribute. | _Transition effects whose FromRule is this rule._ |
| Transition Events | A defined attribute. | _Transition events whose FromRule is this rule._ |
| Integrity Constraints | A defined attribute. | _Integrity constraints whose Rule is this rule._ |

<details open>
<summary>Example data</summary>

| Head Predicate | Kind | Head JSON | Body JSON | Agg JSON | Paper Section Ref | Description | Name ƒ |
|---|---|---|---|---|---|---|---|
| Reachable | rule | ["Reachable", "x", "y"] | [{"atom": ["Route", "x", "y", "_"]}] | — | §4, §12 | Base case of positive recursion: a direct route makes y reachable from x. | reach_base |
| Reachable | rule | ["Reachable", "x", "y"] | [{"atom": ["Reachable", "x", "z"]}, {"atom": ["Route", "z", "y", "_"]}] | — | §4, §12 | Recursive step of positive recursion: Reachable composes with one more Route hop. | reach_step |
| Isolated | rule | ["Isolated", "x"] | [{"atom": ["Warehouse", "x"]}, {"cmp": ["!=", "x", {"$": "hub"}]}, {"not": ["Reachable", {"$": "hub"}, "x"]}] | — | §7 (negation), §12 | Negation over a strictly lower stratum: a warehouse not reachable from the hub. | isolated |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Transition** | The R0 spec's transitions (r0/spec.json's 'transitions' array): command + guard pairs that may fire, each with its effects and events. | — |
| Command | A defined attribute. | _The command that can fire this transition._ |
| Guard | A defined attribute. | _The rule whose head predicate gates this transition._ |
| Description | A defined attribute. | _What this transition does when its guard is satisfied._ |
| Name | The same as its transition ID. | _Display alias._ |
| Effects | A defined attribute. | _This transition's ordered assert/retract effects._ |
| Events | A defined attribute. | _This transition's emitted events._ |

<details open>
<summary>Example data</summary>

| Description | Name ƒ |
|---|---|
| Opens a new route; the sole enabled transition for command AddRoute. | AddRoute |
| Closes an existing route; the sole enabled transition for command CloseRoute. | CloseRoute |
| Bitemporal correction of a shipment's recorded day: retracts the old fact and asserts the new one under one knowledge time. | CorrectShipment |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Transition Effect** | One row per (transition, effect) pair: an ordered assert/retract of a rule's derived tuples into an EDB relation. | — |
| Transition | A defined attribute. | _The transition this effect belongs to._ |
| Op | A defined attribute. | _'assert' or 'retract'._ |
| Relation | A defined attribute. | _The EDB relation this effect asserts into or retracts from._ |
| From Rule | A defined attribute. | _The rule whose derived tuples this effect applies._ |
| Name | The same as its effect ID. | _Display alias._ |

<details open>
<summary>Example data</summary>

| Op | Name ƒ |
|---|---|
| assert | AddRoute#effect0 |
| retract | CloseRoute#effect0 |
| retract | CorrectShipment#effect0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Transition Event** | One row per (transition, event) pair: an event relation emitted when the transition fires. | — |
| Transition | A defined attribute. | _The transition this event belongs to._ |
| Event Relation | A defined attribute. | _The event relation name emitted (e.g. 'RouteAdded'); not an EDB relation._ |
| From Rule | A defined attribute. | _The rule whose derived tuples produce this event._ |
| Name | The same as its event ID. | _Display alias._ |

<details open>
<summary>Example data</summary>

| Event Relation | Name ƒ |
|---|---|
| RouteAdded | AddRoute#event0 |
| ShipmentCorrected | CorrectShipment#event0 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Integrity Constraint** | The R0 spec's integrity constraints (r0/spec.json's 'constraints' array): predicates whose nonempty derivation rejects a transition. Named IntegrityConstraints, not Constraints -- rulebook-to-rulespeak v2026.09.17.2011 treats a table literally named 'Constraints' as a reserved word and fails the whole build with "Entity '' does not resolve to any table." | — |
| Rule | A defined attribute. | _The rule that derives this constraint's violations._ |
| Description | A defined attribute. | _What a nonempty derivation of this predicate means, and which transition it rejects._ |
| Name | The same as its constraint ID. | _Display alias._ |

<details open>
<summary>Example data</summary>

| Description | Name ƒ |
|---|---|
| Rejects any transition whose effect would leave a route with negative capacity. | NegativeCap |
| Rejects any transition whose effect would leave a route from a warehouse to itself. | SelfRoute |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Observable** | The R0 spec's observables (r0/spec.json's 'observables' map): the predicates exposed in Obs_R0(c, i, h), section 9. | — |
| Exposed Predicate | A defined attribute. | _The predicate this observable exposes (e.g. 'Reachable')._ |
| Description | A defined attribute. | _Why this predicate is exposed as an observable in Obs_R0(c, i, h)._ |
| Name | The same as its observable ID. | _Display alias._ |

<details open>
<summary>Example data</summary>

| Exposed Predicate | Description | Name ƒ |
|---|---|---|
| Reachable | Positive recursive closure over Route; the paper's running example of Lemma 1's finite lattice. | reachable |
| Isolated | Negation over Reachable: witnesses the stratum operator's monotonicity (§7). | isolated |
| Fanout | COUNT aggregation over Route; witnesses the empty-group case (§5). | fanout |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Seed Fact** | The R0 spec's initial history h0 (r0/spec.json's 'h0' array): the seed facts every input history starts from. | — |
| N | A defined attribute. | _The fact's 'n' (insertion order) in spec.json's h0 array._ |
| Op | A defined attribute. | _'assert' (h0 is assert-only)._ |
| Relation | A defined attribute. | _The EDB relation this seed fact populates._ |
| Tuple JSON | A defined attribute. | _The seeded tuple, verbatim JSON from spec.json._ |
| K | A defined attribute. | _Knowledge time of the seed fact (0 for all of h0)._ |
| Name | The same as its seed fact ID. | _Display alias._ |

<details open>
<summary>Example data</summary>

| N | Op | Tuple JSON | K | Name ƒ |
|---|---|---|---|---|
| 0 | assert | ["hub"] | 0 | h0-0 |
| 1 | assert | ["a"] | 0 | h0-1 |
| 2 | assert | ["b"] | 0 | h0-2 |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Input History** | The two input histories this example runs (r0/inputs/*.json): what each is designed to exercise. | — |
| Source File | A defined attribute. | _Path to the input history file, relative to the project root._ |
| Event Count | A defined attribute. | _Number of command events in the input history, counted once at transcription time._ |
| Description | A defined attribute. | _What this input history is designed to exercise._ |
| Name | The same as its input history ID. | _Display alias._ |
| Answer Key Results | A defined attribute. | _Per-substrate answer-key results computed against this input history._ |

<details open>
<summary>Example data</summary>

| Source File | Event Count | Description | Name ƒ |
|---|---|---|---|
| r0/inputs/i1-routes.json | 3 | AddRoute/CloseRoute/Reroute traffic over the warehouse network; exercises recursion, negation, aggregation and the nondeterministic Reroute command. | i1-routes |
| r0/inputs/i2-bitemporal.json | 3 | Observes one shipment at three (valid time, knowledge time) pairs around a CorrectShipment command; exercises the bitemporal snapshot rules tau generates (§7). | i2-bitemporal |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Reflection Condition** | The six conditions of constructor reflection, section 13 of the paper. | — |
| Condition Number | A defined attribute. | _The condition's number in §13 (1-6)._ |
| Title | A defined attribute. | _Short name of the condition._ |
| Paper Section Ref | A defined attribute. | _Paper section this condition is defined in._ |
| Description | A defined attribute. | _What the condition checks on the substrate artifact's call graph._ |
| Name | The same as its condition ID. | _Display alias._ |
| Reflection Results | A defined attribute. | _Per-substrate pass/fail/delegated outcomes for this condition._ |

<details open>
<summary>Example data</summary>

| Condition Number | Title | Paper Section Ref | Description | Name ƒ |
|---|---|---|---|---|
| 1 | Constructor preservation | §13 | Every G_R node has a function of its kind in the artifact. | 1_constructor_preservation |
| 2 | Dependency preservation | §13 | Every source edge in G_R is a call edge in the artifact. | 2_dependency_preservation |
| 3 | Addressability | §13 | Every G_R node has an image (a named function) in the artifact. | 3_addressability |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Substrate** | The three substrates checked against the R0 spec: transparent (the real representation) and two controls, tangled and interpreter, that are expected to fail reflection on purpose. | — |
| Description | A defined attribute. | _What this substrate is and how it is generated._ |
| Paper Section Ref | A defined attribute. | _Paper section(s) this substrate demonstrates._ |
| Is Control | True when an empty string. | _True for the two substrates that are deliberately expected to fail reflection (tangled, interpreter)._ |
| Name | The same as its substrate ID. | _Display alias._ |
| Answer Key Results | A defined attribute. | _This substrate's per-input-history answer-key results._ |
| Reflection Results | A defined attribute. | _This substrate's per-condition reflection results._ |
| Answer Key Result Count | The number of answer key results related to the substrate. | _Number of answer-key results recorded for this substrate._ |
| Answer Key Fail Count | The number of the substrate's answer key results that have a outcome of “fail”. | _Number of answer-key results recorded as a failure for this substrate._ |
| Answer Key Ok | True when all of the following hold: the answer key result count is greater than 0 and the answer key fail count is 0. | _True when every recorded answer-key result for this substrate passed (§11)._ |
| Reflection Result Count | The number of reflection results related to the substrate. | _Number of reflection results recorded for this substrate._ |
| Reflection Fail Count | The number of the substrate's reflection results that have a outcome of “fail”. | _Number of reflection conditions recorded as a failure for this substrate._ |
| Reflection Ok | True when all of the following hold: the reflection result count is greater than 0 and the reflection fail count is 0. | _True when no recorded reflection condition failed for this substrate (§13). Delegated conditions never count as a failure._ |
| Conformant | True when all of the following hold: the answer key ok flag is set and the reflection ok flag is set. | _True only when this substrate passes both gates: the answer key and reflection._ |

<details open>
<summary>Example data</summary>

| Description | Paper Section Ref | Is Control | Name ƒ | Answer Key Result Count ƒ | Answer Key Fail Count ƒ | Answer Key Ok ƒ | Reflection Result Count ƒ | Reflection Fail Count ƒ | Reflection Ok ƒ | Conformant ƒ |
|---|---|---|---|---|---|---|---|---|---|---|
| transpile.py emits one function per G_R node: a rule body as explicit nested loops, a non-recursive predicate as the union of its rules, a recursive stratum as a c_mu_* fixed-point driver. runtime.py never reads the spec JSON. | §12-13 | false | transparent | 0 | 0 | false | 0 | 0 | false | false |
| The transparent module with every predicate read inside a rule routed through _dispatch(name, S), which resolves the target by name at run time. Same computation, same answers; dependency preservation and bounded locality fail because dependencies are now strings resolved dynamically. | §23 | true | tangled | 0 | 0 | false | 0 | 0 | false | false |
| The rulebook loaded as inert JSON and handed to one universal evaluator (the SQLite oracle, compiling at request time). Zero image functions; constructor preservation, dependency preservation and addressability all fail. | §6, §13 | true | interpreter | 0 | 0 | false | 0 | 0 | false | false |

_ƒ marks a computed column._

</details>

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Answer Key Result** | Per (substrate, input history) answer-key outcomes, ingested from a fresh run of conformance/run.py (conformance/matrix.json). Witnessed data, never recomputed here. | — |
| Substrate | A defined attribute. | _The substrate this result was computed for._ |
| Input History | A defined attribute. | _The input history this result was computed against._ |
| Traces Produced | A defined attribute. | _Number of distinct traces this substrate produced for this input history (witnessed by conformance/run.py)._ |
| Permitted | A defined attribute. | _Size of the permitted (expected) trace set for this input history (witnessed by tools/gen_expected.py)._ |
| Outcome | A defined attribute. | _'pass' if every produced trace was inside the permitted set, else 'fail'. Witnessed by conformance/run.py; never recomputed here._ |
| Name | The same as its answer key result ID. | _Display alias._ |

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Reflection Result** | Per (substrate, reflection condition) outcomes, ingested from a fresh run of conformance/run.py (conformance/matrix.json). Witnessed data, never recomputed here. | — |
| Substrate | A defined attribute. | _The substrate this reflection outcome was computed for._ |
| Reflection Condition | A defined attribute. | _The §13 condition this outcome is for._ |
| Outcome | A defined attribute. | _'pass', 'fail' or 'delegated' (condition 4 is always delegated to the answer key by design). Witnessed by tools/reflection_check.py; never recomputed here._ |
| Name | The same as its reflection result ID. | _Display alias._ |

## 2 Fact Types

- a **rule** references exactly one **transition**
- a **transition** references exactly one **command**
- a **transition** references exactly one **rule**
- a **transition** references exactly one **transition effect**
- a **transition** references exactly one **transition event**
- a **transition effect** references exactly one **transition**
- a **transition effect** references exactly one **EDB relation**
- a **transition effect** references exactly one **rule**
- a **transition event** references exactly one **transition**
- a **transition event** references exactly one **rule**
- an **integrity constraint** references exactly one **rule**
- a **seed fact** references exactly one **EDB relation**
- an **answer key result** references exactly one **substrate**
- an **answer key result** references exactly one **input history**
- a **reflection result** references exactly one **substrate**
- a **reflection result** references exactly one **reflection condition**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- An EDB relation **must** reference exactly one seed fact.
- An EDB relation **must** reference exactly one transition effect.
- An EDB relation **must** have a fields; a field count; and a description.
- A command **must** reference exactly one transition.
- A command **must** have a params; a param count; and a description.
- A rule **must** reference exactly one transition as its guarded transitions.
- A rule **must** reference exactly one transition effect.
- A rule **must** reference exactly one transition event.
- A rule **must** reference exactly one integrity constraint.
- A rule **must** have a head predicate; a kind; a head JSON; a paper section ref; and a description.
- A transition **must** reference exactly one command.
- A transition **must** reference exactly one rule as its guard.
- A transition **must** reference exactly one transition effect as its effects.
- A transition **must** reference exactly one transition event as its events.
- A transition **must** have a description.
- A transition effect **must** reference exactly one transition.
- A transition effect **must** reference exactly one EDB relation as its relation.
- A transition effect **must** reference exactly one rule as its from rule.
- A transition effect **must** have an op.
- A transition event **must** reference exactly one transition.
- A transition event **must** reference exactly one rule as its from rule.
- A transition event **must** have an event relation.
- An integrity constraint **must** reference exactly one rule.
- An integrity constraint **must** have a description.
- An observable **must** have an exposed predicate and a description.
- A seed fact **must** reference exactly one EDB relation as its relation.
- A seed fact **must** have a n; an op; a tuple JSON; and a k.
- An input history **must** reference exactly one answer key result.
- An input history **must** have a source file; an event count; and a description.
- A reflection condition **must** reference exactly one reflection result.
- A reflection condition **must** have a condition number; a title; a paper section ref; and a description.
- A substrate **must** reference exactly one answer key result.
- A substrate **must** reference exactly one reflection result.
- A substrate **must** have a description and a paper section ref, and record whether it is a control.
- An answer key result **must** reference exactly one substrate.
- An answer key result **must** reference exactly one input history.
- An answer key result **must** have a traces produced; a permitted; and an outcome.
- A reflection result **must** reference exactly one substrate.
- A reflection result **must** reference exactly one reflection condition.
- A reflection result **must** have an outcome.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | An EDB relation's name is the same as its relation ID. |
| **DR-2 Name** | A command's name is the same as its command ID. |
| **DR-3 Name** | A rule's name is the same as its rule ID. |
| **DR-4 Name** | A transition's name is the same as its transition ID. |
| **DR-5 Name** | A transition effect's name is the same as its effect ID. |
| **DR-6 Name** | A transition event's name is the same as its event ID. |
| **DR-7 Name** | An integrity constraint's name is the same as its constraint ID. |
| **DR-8 Name** | An observable's name is the same as its observable ID. |
| **DR-9 Name** | A seed fact's name is the same as its seed fact ID. |
| **DR-10 Name** | An input history's name is the same as its input history ID. |
| **DR-11 Name** | A reflection condition's name is the same as its condition ID. |
| **DR-12 Name** | A substrate's name is the same as its substrate ID. |
| **DR-13 Answer Key Result Count** | A substrate's answer key result count is the number of answer key results related to the substrate. |
| **DR-14 Answer Key Fail Count** | A substrate's answer key fail count is the number of the substrate's answer key results that have a outcome of “fail”. |
| **DR-15 Answer Key Ok** | A substrate is flagged answer key ok if all of the following hold: the answer key result count is greater than 0 and the answer key fail count is 0. |
| **DR-16 Reflection Result Count** | A substrate's reflection result count is the number of reflection results related to the substrate. |
| **DR-17 Reflection Fail Count** | A substrate's reflection fail count is the number of the substrate's reflection results that have a outcome of “fail”. |
| **DR-18 Reflection Ok** | A substrate is flagged reflection ok if all of the following hold: the reflection result count is greater than 0 and the reflection fail count is 0. |
| **DR-19 Conformant** | A substrate is flagged conformant if all of the following hold: the answer key ok flag is set and the reflection ok flag is set. |
| **DR-20 Name** | An answer key result's name is the same as its answer key result ID. |
| **DR-21 Name** | A reflection result's name is the same as its reflection result ID. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **EDBRelations.Name** | formula | `RelationId` |
| **Commands.Name** | formula | `CommandId` |
| **Rules.Name** | formula | `RuleId` |
| **Transitions.Name** | formula | `TransitionId` |
| **TransitionEffects.Name** | formula | `EffectId` |
| **TransitionEvents.Name** | formula | `EventId` |
| **IntegrityConstraints.Name** | formula | `ConstraintId` |
| **Observables.Name** | formula | `ObservableId` |
| **SeedFacts.Name** | formula | `SeedFactId` |
| **InputHistories.Name** | formula | `InputHistoryId` |
| **ReflectionConditions.Name** | formula | `ConditionId` |
| **Substrates.Name** | formula | `SubstrateId` |
| **Substrates.AnswerKeyResultCount** | rollup | `Count(AnswerKeyResults via Substrate)` |
| **Substrates.AnswerKeyFailCount** | rollup | `Count(AnswerKeyResults via Substrate)` |
| **Substrates.AnswerKeyOk** | formula | `And(AnswerKeyResultCount > 0, AnswerKeyFailCount = 0)` |
| **Substrates.ReflectionResultCount** | rollup | `Count(ReflectionResults via Substrate)` |
| **Substrates.ReflectionFailCount** | rollup | `Count(ReflectionResults via Substrate)` |
| **Substrates.ReflectionOk** | formula | `And(ReflectionResultCount > 0, ReflectionFailCount = 0)` |
| **Substrates.Conformant** | formula | `And(AnswerKeyOk, ReflectionOk)` |
| **AnswerKeyResults.Name** | formula | `AnswerKeyResultId` |
| **ReflectionResults.Name** | formula | `ReflectionResultId` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
