# Leopold Loops — Planar Unit-Distance Discovery

Each loop = one named **CHANGE-RULE → `effortless build` → CONSUME-VIEWS** turn through the rulebook. The hub is [effortless-rulebook/planar-unit-discovery-rulebook.json](effortless-rulebook/planar-unit-discovery-rulebook.json).

---

## Completed

### [x] Loop 1 — v0.1 Bootstrap Sketch

**Where:** [bootstrap/ERB_euclidean_unit_distance_neighborhood_v0_1.json](bootstrap/ERB_euclidean_unit_distance_neighborhood_v0_1.json) (87 KB)

**Shape:** Meta-schema design — `object_types`, `relation_types`, `math_objects`, `math_relations`, `property_types`, `formula_definitions`, `semantic_routes`, `semantic_route_steps`, `model_tests`. Concepts encoded as rows of generic typing tables. Inert in ERB because schema-as-data doesn't let the build pipeline compute per-table calculated fields.

**Outcome:** the planning sketch that names every concept Sawin's chain touches.

---

### [x] Loop 2 — Wholesale Rewrite to First-Class Tables

**What changed:** scrapped the meta-schema; promoted every named concept to a first-class ERB table with `schema.fields` and `data`. Tables: `Domains`, `Contexts`, `Points`, `PointSets`, `PointSetMembers`, `PointPairs`, `UnitDistanceGraphs`, `NumberFields`, `PrimeIdeals`, `MinkowskiLattices`, `ShortVectors`, `ConstructionFamilies`, `ConstructionInstances`, `GrowthSequences`, `AsymptoticFunctions`, `AsymptoticLowerBounds`, `Theorems`, plus `__meta__`. 18 tables, ~253 fields, ~68 rows.

**Outcome:** runnable ERB rulebook; the geometric layer (Pythagoras → unit-distance predicate → induced graph) and the asymptotic layer (growth sequence → bound → theorem) are both fully wired. The middle (algebraic-number-theoretic source feeding lattice → projection → planar geometry) was named but light on machinery.

---

### [x] Loop 3 — Facelift: Missing Machinery + Bridges

**What changed:** added 13 new tables — `Metrics`, `FieldEmbeddings`, `MinkowskiEmbeddings`, `GramMatrices`, `PlanarProjections`, `ProjectedShortVectors`, `GolodShafarevichCriteria`, `SemanticBridges`, `SemanticRoutes`, `SemanticRouteSteps`, `SourceReferences`, `Lemmas`, `MirrorContract`. Extended existing tables with chain-closure rollups: `NumberFields.SatisfiesGolodShafarevich`, `NumberFields.IsAlgebraicSourceCandidate`, `MinkowskiLattices.IsLoadBearingForUnitDistanceConstruction`, `ConstructionFamilies.IsAlgebraicConstruction`, `ConstructionInstances.IsAlgebraicSuperlinearWitness`, `AsymptoticLowerBounds.IsAlgebraicallyAnchored`, `Theorems.AlgebraicChainClosed`. Added 4 more `Theorems` rows for multi-anchor coverage (Erdős baseline, Szemerédi–Trotter, Spencer-S-T, conjectural-future slot).

**Outcome:** 31 tables, 340 fields, 143 rows. The full inference DAG is wired top to bottom — five orders of inference (1st = same-row math; 5th = `Theorems.AlgebraicChainClosed`) all named explicitly. Every nullable boolean carries null/true/false coverage rows.

---

### [x] Loop 4 — Q(ζ_7) Chain Materialization + Curation Layer

**What changed:** materialized the second algebraic source end to end — `lat-q-zeta-7` (det = √16807, dim 6), real cyclotomic `PrimeIdeals` data (ramified prime above 7 of norm 7, two primes above 2 of norm 8 each, six primes above 29 of norm 29), `GramMatrices.gram-lat-q-zeta-7`, 6 `ShortVectors` (the powers of ζ_7 under Minkowski, squared norm 3), `proj-q-zeta-7-sigma1` `PlanarProjection`, 6 `ProjectedShortVectors`, a `heptagon-with-center` `PointSet` with 8 points / 7 unit-distance pairs, `zeta7-sigma1-projection` `ConstructionFamily`, an under-claimed `AsymptoticLowerBound` (exp 0.9) that resolves `WitnessConsistent = TRUE` honestly. Added 4 curation-layer tables: `Conjectures`, `ProofObligations`, `CitationLinks`, `AnswerKey`. Added cross-link rollups on `Lemmas.ObligationCount`, `AsymptoticLowerBounds.AllObligationsSatisfied`, `Theorems.FullyAuditedAndClosed`, `SourceReferences.OutboundCitationCount` / `InboundCitationCount` / `LemmaCount`.

**Outcome:** 35 tables, 403 fields, 233 rows. FK integrity verified across all tables. The Sawin theorem row honestly reports `AlgebraicChainClosed = FALSE` and `FullyAuditedAndClosed = FALSE` because the actual paper content hasn't been loaded — exactly the gap Loop 5 closes.

---

## Next

### [ ] Loop 5 — Load Actual Paper Content

**CHANGE-RULE:** extract the named lemmas from whichever paper carries the current state-of-the-art lower bound on U(n) (the placeholder `arXiv:2605.20579` is not a real ID). For each `Lemmas` row, fill in real `StatementText`, flip `IsLoaded` to TRUE. Add at least one `ConstructionInstance` whose `(ParamN, EdgeCount)` resolves `DensityExponentEstimate ≥ 1.014`. Flip each `ProofObligations.IsSatisfied` row by row.

**BUILD propagates:** Postgres calculated functions recompute; `GrowthSequences.MaxObservedDensityExponentEstimate` shifts upward.

**VIEWS consume:** `Theorems.AlgebraicChainClosed` and `FullyAuditedAndClosed` flip TRUE for `thm-superlinear-unit-distance`. The honest "info"-level `AnswerKey` rows can be re-pinned to `GateLevel = blocking` with `IsCurrentlyMatched = true`.

**Blocker:** requires the actual paper. If the arXiv ID stays a placeholder, substitute the latest published result with this same shape of loop.

---

### [ ] Loop 6 — `AsymptoticUpperBounds` (Sandwich U(n))

**CHANGE-RULE:** add top-level table `AsymptoticUpperBounds` mirroring `AsymptoticLowerBounds`. Load `spencer-st-n43` (Exponent = 4/3). Add `Theorems.AnchoredUpperBound` FK linking `thm-spencer-szemeredi-trotter` and `thm-szemeredi-trotter` to the upper-bound rows. Add calc fields `AsymptoticFunctions.BestKnownUpperBoundExponent` (MIN aggregation) and `AsymptoticFunctions.GapBetweenBounds` = upper − lower.

**BUILD propagates:** new table generated everywhere (Postgres tables/views, Python, OWL). The substrate transpilers add `vw_asymptoticupperbounds` and the corresponding base classes.

**VIEWS consume:** any dashboard showing U(n) now displays the gap `1.014 ≤ exponent ≤ 1.333…`. New `AnswerKey` rows pin the upper-bound side. `Conjectures.conj-erdos-43-tightness` can reference both ends.

**Closes:** the model currently has only the lower-bound side of the open question. U(n) is defined by both — sandwich is the natural shape.

---

### [ ] Loop 7 — `SubstrateRuns` + `ConformanceResults`

**CHANGE-RULE:** add two new tables:
- `SubstrateRuns` — one row per (substrate × build timestamp): `SubstrateKind`, `RanAt`, `RulebookCommitSha`
- `ConformanceResults` — one row per (`SubstrateRun` × `AnswerKey`): `ObservedValue`, `Matched` boolean

Change `AnswerKey.IsCurrentlyMatched` from raw to **lookup** — `LOOKUP(ConformanceResults!{{Matched}}, …)` against the latest `SubstrateRun`. Add `LastRunFailureCount` aggregation on `SubstrateRuns`.

**BUILD propagates:** the conformance harness (`orchestration/test-orchestrator.py`) writes rows into `ConformanceResults` after each substrate run. Generated views update.

**VIEWS consume:** the orchestration HTML report reads from `vw_substrateruns` and `vw_answerkey`; the §14 gate is now self-reporting instead of curator-set.

**Closes:** today `IsCurrentlyMatched` is hand-pinned. After this loop it's *derived from the actual run* and stale pins can't lie.

---

### [ ] Loop 8 — `ProofAssistantArtifacts`

**CHANGE-RULE:** add `ProofAssistantArtifacts` — one row per (Lemma × proof-assistant artifact): `Assistant` (lean4 / coq / isabelle), `ArtifactURI`, `LastCheckedAt`, `IsMachineVerified`. Aggregate onto `Lemmas.MachineVerifiedArtifactCount` and `Lemmas.IsMachineVerified` = (count > 0). Change `ProofObligations.IsSatisfied` to be calculated from `IsLemmaMachineVerified` when an artifact exists, falling back to the curator-set raw value when null.

**BUILD propagates:** new table generated; all existing aggregations gain a machine-verifiable signal path. The Postgres substrate exposes `vw_proofassistantartifacts`.

**VIEWS consume:** theorems with `AllObligationsSatisfied = TRUE` now carry the stronger meaning — *machine-verified, not just curator-asserted*. `FullyAuditedAndClosed` carries different epistemic weight after this loop.

**Closes:** bridges the one part previously called "different in kind" from structure-mirroring (proof verification) by giving the rulebook a place to carry the verification *pointer*.

---

### [ ] Loop 9 — Competing Algebraic Source (Head-to-Head)

**CHANGE-RULE:** load a second non-cyclotomic `NumberField` — e.g., a real-quadratic `Q(√k)` with `IsTotallyReal = TRUE` and large class number — plus its `MinkowskiEmbedding`, `MinkowskiLattice`, `ShortVectors`, `PlanarProjection`, `ProjectedShortVectors`, `ConstructionFamily`, `ConstructionInstances`, `GrowthSequence`, and an `AsymptoticLowerBound`. Add `ConstructionFamilies.RankAgainstSiblings` (calculated or aggregated ordering by `MaxObservedDensityExponentEstimate`).

**BUILD propagates:** purely additive. No schema changes — just more rows of types already present.

**VIEWS consume:** `AsymptoticFunctions.BestKnownLowerBoundExponent` aggregates over more bounds and picks the winning family automatically. Head-to-head comparison is now a query, not a discussion.

**Closes:** today the entire algebraic side has one loaded field (Q(√−3)) and one candidate (Q(ζ_7)). The CMCC claim that the model is the invariant gets stronger when two structurally-different algebraic sources produce comparable witnesses in the same schema.

---

## After Loop 9

The rulebook describes U(n) with: known lower bounds + known upper bounds + the gap + the conjectures targeting it + at least two algebraically-distinct witnessing families + machine-verified lemmas + multi-substrate conformance results. The naked-Claude alternative would need ~30 hand-coded files updated in lockstep per loop and would be wrong before the second one shipped.
