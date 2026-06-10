# Talisman's Special Solutions — Article Inference Coverage Spec

**Goal:** 100% implementation coverage of every *checkable claim* in Jessica Talisman's
NTWF article (Parts I–III), expressed as **first-class inference chains in the rulebook**
(→ Postgres views), with **zero sidecar/imperative logic**, at the **minimum surface area**
that tells the entire story unambiguously. Add nothing that is not load-bearing in the
article (or in direct service of it); leave nothing out.

This spec is for review **before** any edit to the sacred
`effortless-rulebook/talismans-special-solutions-rulebook.json`.

---

## Principle: what is already covered vs. what must be added

The rulebook already faithfully encodes the **T-Box + A-Box** (all 9 classes as tables,
all standard-vocab mappings in field `Description`s, the exact worked-example instances:
5 steps, 6 roles, 4 humans + 1 AI + 1 pipeline, 5-artifact `wasDerivedFrom` chain,
1 dataset, 1 approval gate, RM→VP→CTO delegation).

What is **missing** is the **derived / inferred layer** — the facts the article only obtains
by running an OWL-RL reasoner or by writing imperative SPARQL/Python. Those are exactly the
claims that must become derived columns/tables so the Postgres view *is* the answer.

**Engine capability constraints (verified):**
- Scalar formulas: `IF/AND/OR/NOT/CONCAT/&/LOWER/SUBSTITUTE/comparison/arithmetic/date-diff`.
- Single-hop `lookup` (`=INDEX(T!{{Col}}, MATCH({{FK}}, T!{{PK}}, 0))`); **multi-hop chains compose**
  (a lookup may target another lookup).
- Conditional aggregation: `COUNTIFS` / `SUMIFS` with WHERE criteria.
- **No recursion / transitive closure.** → Any transitive inference must be a **first-class derived table**.

---

## PART A — Two first-class CLOSURE TABLES (the recursive inferences)

These are *new triples* in the article's terms, so they are *new rows* here — exactly how
`StepPrecedence` is itself a first-class table, not a helper.

### A1. `StepPrecedenceClosure`  (materializes `ntwf:precedesStep` as `owl:TransitiveProperty`)

Article claim: 4 asserted edges → **10 ordering pairs** (6 inferred), incl. the never-asserted
`step-1 → step-5`. Full closure of the path 1→2→3→4→5 = all (i<j) pairs.

**Schema** (mirrors `StepPrecedence` + an `IsInferred` witness):
| field | type | notes |
|---|---|---|
| `StepPrecedenceClosureId` | raw/string (PK) | e.g. `closure-1-5` |
| `Name` | calculated/string | `={{FromStep}} & " ->* " & {{ToStep}}` (`->*` = transitive) |
| `FromStep` | relationship → WorkflowSteps | predecessor |
| `ToStep` | relationship → WorkflowSteps | successor |
| `IsInferred` | raw/boolean | `false` for the 4 asserted, `true` for the 6 inferred |
| `HopDistance` | raw/integer | `ToPos − FromPos` (1 = adjacent/asserted) — lets you witness "non-adjacent ⇒ inferred" |

**Data (10 rows):**
```
closure-1-2  step-1->step-2  inferred=false  hop=1
closure-1-3  step-1->step-3  inferred=true   hop=2
closure-1-4  step-1->step-4  inferred=true   hop=3
closure-1-5  step-1->step-5  inferred=true   hop=4   ← the article's signature inference
closure-2-3  step-2->step-3  inferred=false  hop=1
closure-2-4  step-2->step-4  inferred=true   hop=2
closure-2-5  step-2->step-5  inferred=true   hop=3
closure-3-4  step-3->step-4  inferred=false  hop=1
closure-3-5  step-3->step-5  inferred=true   hop=2
closure-4-5  step-4->step-5  inferred=false  hop=1
```
Witness columns that prove the closure fired:
- `Workflows.CountOfPrecedenceClosurePairs` = `COUNTIFS(StepPrecedenceClosure ...)` → **10**
- `Workflows.CountOfInferredPrecedencePairs` = `COUNTIFS(... IsInferred=TRUE)` → **6**

### A2. `RoleDelegationClosure`  (materializes `ntwf:delegatesTo+` reachability)

Article claim: escalation chain RM → VP Engineering → CTO; `delegatesTo+` SPARQL property path
makes RM→CTO reachable though never asserted.

**Schema** (parallels A1):
| field | type | notes |
|---|---|---|
| `RoleDelegationClosureId` | raw/string (PK) | e.g. `deleg-rm-cto` |
| `Name` | calculated/string | `={{FromRole}} & " ->* " & {{ToRole}}` |
| `FromRole` | relationship → Roles | |
| `ToRole` | relationship → Roles | |
| `IsInferred` | raw/boolean | asserted edges false; RM→CTO true |
| `HopDistance` | raw/integer | |

**Data (3 rows):**
```
deleg-rm-vp    release-manager -> vp-engineering   inferred=false  hop=1
deleg-vp-cto   vp-engineering  -> cto              inferred=false  hop=1
deleg-rm-cto   release-manager -> cto              inferred=true   hop=2   ← inferred reachability
```

---

## PART B — LOOKUP CHAINS (read a related row's field; the role→agent indirection)

The article's CQs need a step's / artifact's **executing agent type**. That is the
role→agent indirection resolved one hop — a load-bearing lookup (explicitly welcomed).

### B1. `WorkflowSteps` — executing-agent resolution (lookup chain through AssignedRole)
| new field | type | formula (INDEX/MATCH) | purpose |
|---|---|---|---|
| `ExecutingHumanAgent` | lookup → Roles.FilledByHumanAgent via AssignedRole | `=INDEX(Roles!{{FilledByHumanAgent}}, MATCH({{AssignedRole}}, Roles!{{RoleId}},0))` | which human (if any) |
| `ExecutingAIAgent` | lookup → Roles.FilledByAIAgent | analogous | which AI (if any) |
| `ExecutingAutomatedPipeline` | lookup → Roles.FilledByAutomatedPipeline | analogous | which pipeline (if any) |
| `ExecutingAgentType` | calculated/string | `=IF({{ExecutingHumanAgent}}<>"","HumanAgent",IF({{ExecutingAIAgent}}<>"","AIAgent",IF({{ExecutingAutomatedPipeline}}<>"","AutomatedPipeline","")))` | **CQ3** discriminator |
| `IsExecutedByAI` | calculated/boolean | `={{ExecutingAIAgent}}<>""` | CQ3 / business demo |
| `IsExecutedByHuman` | calculated/boolean | `={{ExecutingHumanAgent}}<>""` | CQ3 |

→ **CQ3 ("2 AI steps, 2 human steps")** becomes two `COUNTIFS` on `WorkflowSteps` grouped by
`ExecutingAgentType` (witnessed at the Workflow level, see Part D).

### B2. `WorkflowSteps` — human-approval consistency (Heuristic / human-only gate rule)
| new field | type | formula | purpose |
|---|---|---|---|
| `ApprovalIsHumanFilled` | calculated/boolean | `=IF({{RequiresHumanApproval}}=TRUE, {{ExecutingHumanAgent}}<>"", TRUE)` | true iff a human-approval step is actually filled by a human (vacuously true when not required) |
| `ApprovalConsistencyViolation` | calculated/boolean | `=AND({{RequiresHumanApproval}}=TRUE, {{ExecutingHumanAgent}}="")` | the article's *detectable error* witness (clean ABox ⇒ all FALSE) |

### B3. `ApprovalGates` — who approves (CQ2 chain: gate → role → human)
| new field | type | formula | purpose |
|---|---|---|---|
| `GateRole` | lookup → WorkflowSteps.AssignedRole via WorkflowStep | INDEX/MATCH on the specialized step | the role at the gate |
| `GateApproverHuman` | lookup (2-hop) → Roles.FilledByHumanAgent via GateRole | INDEX/MATCH | **CQ2**: gate → Release Manager → Maria |

### B4. `WorkflowArtifacts` — producing-agent type (CQ4 attribution)
| new field | type | formula | purpose |
|---|---|---|---|
| `ProducingAgentType` | calculated/string | `=IF({{AttributedToHumanAgent}}<>"","HumanAgent",IF({{AttributedToAIAgent}}<>"","AIAgent",IF({{AttributedToAutomatedPipeline}}<>"","AutomatedPipeline","")))` | which agent class produced each artifact |
| `HasDerivationParent` | calculated/boolean | `={{DerivedFromArtifact}}<>""` | counts the 4 `wasDerivedFrom` links (5 artifacts, 4 links) |

---

## PART C — DISJOINTNESS / FUNCTIONAL witnesses (same-row scalar; no lookup)

### C1. `Roles` — exactly-one-filler (mirrors `filledBy` functional + 3-way disjointness)
| new field | type | formula | purpose |
|---|---|---|---|
| `FilledByArmCount` | calculated/integer | `=IF({{FilledByHumanAgent}}<>"",1,0)+IF({{FilledByAIAgent}}<>"",1,0)+IF({{FilledByAutomatedPipeline}}<>"",1,0)` | how many arms set |
| `HasExactlyOneFiller` | calculated/boolean | `={{FilledByArmCount}}=1` | clean ABox ⇒ all TRUE (disjointness witness) |
| `FillerType` | calculated/string | `=IF({{FilledByHumanAgent}}<>"","HumanAgent",IF({{FilledByAIAgent}}<>"","AIAgent",IF({{FilledByAutomatedPipeline}}<>"","AutomatedPipeline","")))` | role's filler class (feeds CQ6) |

---

## PART D — WORKFLOW-LEVEL ROLLUPS (the CQ answers + business payoff)

Added to `Workflows` (aggregations over the now-typed steps/closure):

| new field | type | formula sketch | article claim it satisfies |
|---|---|---|---|
| `CountAISteps` | aggregation/integer | `COUNTIFS(WorkflowSteps!{{Workflow}}, {{WorkflowId}}, WorkflowSteps!{{IsExecutedByAI}}, TRUE)` | **CQ3 → 2** |
| `CountHumanSteps` | aggregation/integer | analogous on `IsExecutedByHuman` | **CQ3 → 2** |
| `CountPrecedenceClosurePairs` | aggregation/integer | `COUNTIFS(StepPrecedenceClosure ... where FromStep∈workflow)` | closure = **10** |
| `CountInferredPrecedencePairs` | aggregation/integer | `... IsInferred=TRUE` | inferred = **6** |
| `HasAIAgentStep` | calculated/boolean | `={{CountAISteps}}>0` | business demo join |
| `MonthsSinceModified` | calculated/integer | `=DATETIME_DIFF(NOW(), {{Modified}}, "month")` (or day/30) | **CQ5** staleness |
| `IsStale` | calculated/boolean | `={{MonthsSinceModified}} > 12` | **CQ5** "not reviewed in 12 months" |
| `IsStaleAndHasAIAgent` | calculated/boolean | `=AND({{IsStale}}, {{HasAIAgentStep}})` | **the article's headline business SPARQL payoff** |
| `CountDerivationLinks` | aggregation/integer | `COUNTIFS(WorkflowArtifacts ... HasDerivationParent=TRUE)` | **CQ4 → 4 links** |
| `CountLegalOwnedSteps` | aggregation/integer | steps whose role's dept = Legal | **CQ7 → 1** |
| `InvolvesEngineeringAndLegal` | calculated/boolean | both dept counts > 0 | **CQ7** |
| `CountApprovalConsistencyViolations` | aggregation/integer | `COUNTIFS(WorkflowSteps ... ApprovalConsistencyViolation=TRUE)` | clean ABox ⇒ **0** (Suite-4 witness) |
| `CountRolesWithBadFillerCardinality` | aggregation/integer | `COUNTIFS(Roles ... HasExactlyOneFiller=FALSE)` | clean ABox ⇒ **0** (disjointness witness) |

*(CQ7 "Legal-owned step" needs a step→role→dept hop. If a `StepOwningDepartment` lookup chain on
`WorkflowSteps` is added (INDEX/MATCH AssignedRole→OwnedBy), `CountLegalOwnedSteps` is a plain COUNTIFS.
This lookup is load-bearing for CQ7 and otherwise minimal.)*

---

## Coverage matrix (article claim → construct)

| Article (Suite/CQ) | Expected | Construct |
|---|---|---|
| precedesStep closure | 10 pairs, 6 inferred, incl. 1→5 | A1 table + D rollups |
| delegatesTo+ reachability | RM→CTO inferred | A2 table |
| CQ1 ordered steps | 5 in order | existing `SequencePosition` + A1 |
| CQ2 approver | gate→RM→Maria | B3 |
| CQ3 AI vs human steps | 2 / 2 | B1 + D (`CountAISteps`/`CountHumanSteps`) |
| CQ4 artifacts + lineage | 5 artifacts, 4 links | B4 + D (`CountDerivationLinks`) |
| CQ5 staleness | modified queryable / >12mo | D (`MonthsSinceModified`/`IsStale`) |
| CQ6 delegation, 0 AI | 2-link chain, 0 AI | A2 + C1 `FillerType` |
| CQ7 Eng+Legal intersect | 1 workflow, 1 Legal step | D (`InvolvesEngineeringAndLegal`/`CountLegalOwnedSteps`) |
| CQ8 dataset-consuming step | 1 step, risk-ai | existing `ConsumesDataset` + B1 type |
| Suite-1 functional/disjoint | 4 functional, 5 disjoint | C1 witnesses (+ semantic table, Q2 phase) |
| Suite-4 disjointness error | detectable | C1 `HasExactlyOneFiller` (clean ⇒ all true) |
| Suite-4 human-approval | detectable | B2 `ApprovalConsistencyViolation` (clean ⇒ all false) |
| Business payoff | stale + AI workflow | D `IsStaleAndHasAIAgent` |

---

## What is deliberately NOT added (not load-bearing / would be invention)
- No `owl:sameAs` merge simulation for the functional-property error (that's a reasoner behavior;
  the *witness* `HasExactlyOneFiller` is the relational equivalent the article actually checks for).
- No new instances beyond the article's worked example.
- No materialized OWL/TTL here — that is Q2 (the generator), which reads a **semantic-mapping
  first-class table** (per the chosen design) rather than living in business tables.

---

## Open questions for review
1. **Closure tables hand-authored vs. transpiler-computed.** This spec hand-authors the 10 + 3
   closure rows (deterministic, tiny, fully derivable from the asserted edges). The alternative —
   teach `rulebook-to-postgres` to emit a `WITH RECURSIVE` view — is a bigger, shared-transpiler
   change. Hand-authored rows are correct and auditable now; recommend that, and flag the recursive-
   view upgrade as a separate follow-up.
2. **`NOW()` in `MonthsSinceModified`** makes staleness time-dependent. The engine seeds `NOW()`
   deterministically for conformance (`FORMULA_NOW`). Confirm we want a live staleness calc (matches
   the article's "haven't been reviewed in twelve months") vs. a fixed reference date.
3. Confirm field-naming register (the rulebook favors `DisplayName`/`Title`/rich `explanation_rich`
   on important calc fields — I'll add `important: true` + `explanation_rich` to the headline
   inference fields so the story reads in the report).
