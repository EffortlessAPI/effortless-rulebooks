# Simpson's Paradox — Witnessed DAG Demo

A **witnessed dependency graph** that turns Simpson's paradox from a textbook curiosity into a computational object. **238 studies** (233 real, 5 synthetic) — spanning medicine, epidemiology, law, sports, education, economics, public health, and social science — share one entity model. The instrument was built across **80 loop iterations** (through **loop-80** complete; **loop-87** and **loop-88** planned). Every derived value falls out of formulas declared in the rulebook; modeling choices are centralized there, not scattered in app code.

**Start at `/discovery` or `/conclusions`.** Formal epistemic claims live in the rulebook `Conclusions` table — tiered by claim type (theorem · instrument · corpus snapshot). Loop-61+ `DiscoveryHypotheses` split into **consistency checks** (definition-linked, e.g. H-purity) vs **corpus hypotheses** (contingent, provisional). SSoT is the rulebook JSON; the explorer reads `vw_*` views only. Run `./start.sh` → [Discovery Research](http://localhost:5173/discovery) · [Conclusions & Findings](http://localhost:5173/conclusions).

[`simpsons-paradox-summary.pdf`](simpsons-paradox-summary.pdf) is a static export of the same tiered state (regenerated on `./init-db.sh`). For framing caveats — geometric vs causal, deductive vs empirical, convenience-sample limits — see [What this is not](#what-this-is-not) below.

---

## What this is

An **Effortless Rulebook (ERB)** domain. The SSoT is `effortless-rulebook/simpsons-paradox-rulebook.json`. All other artifacts — Postgres SQL, views, OWL ontology, RuleSpeak, Explainer DAG, the explorer UI — are mechanically derived from it.

The paradox **emerges** from the model. It is not modeled directly. `IsReversal` equals `IsSignFlip` (loop-60 prune); both are derived booleans comparing pooled rates to per-stratum rates. There is no `ReversalDetection` entity.

The `Loops` table is the build history — each row documents what domain concept was introduced, which natural-language question it answers, and what was witnessed.

---

## Recent loops (60–80)

| Phase | Loops | What landed |
|---|---|---|
| **Legacy prune** | **loop-60** | `IsReversal` redefined as `IsSignFlip`; dropped v2 meta fields (`DefinitionDelta`, `InstrumentScore`, etc.). Unanimous-vs-partial distinction lives in `DistortionType` A vs B only. |
| **Discovery research sweep** | **loop-61–66** | `DiscoveryHypotheses` + `DiscoveryFindings`; `/discovery` UI; causal-role manifest vs latent flips; `IsParadoxExplained` ↔ confounder biconditional; signal-purity law promoted to invariant; catalog type-prediction audit; domain heterogeneity synthesis. |
| **Corpus expansion plan** | **loop-67** | `CorpusDomains` (11 expansion targets); ~142 new `CandidateStudyCatalog` rows from `corpus-expansion-plan.md`; `DataAcquisitionStatus` pipeline fields; `/catalog` acquisition columns. |
| **Expansion wave 1** | **loop-68–69** | 8-study stress batch (adversarial collider, sports unanimous-stratum, economics Expected-A falsification); discovery findings wired; conclusions conc-24..27. |
| **Mechanistic vocabulary** | **loop-70** | `IsStratumUnanimous`, `IsSweepFragile`; Type A/B now keyed on stratum unanimity (not `DistortionRatio` bands). |
| **Expansion wave 2** | **loop-71–72** | 12 studies (criminal-justice, public-health-smoking, sports); corpus → 111 real; domain-profile stability re-tested at scale. |
| **Theorem wave** | **loop-73–76** | Four **Category=theorem** conclusions promoted: CorrectedGap invariance, Explained↔Confounder biconditional, collider-no-manifest (conditional), theorem portfolio synthesis. |
| **Expansion wave 3** | **loop-77–78** | 122 studies bulk-encoded; corpus → **238**; 6 corpus patterns superseded at N=238 (`conc-32`); type-prediction re-audit (~39.1% exact match on imported catalog rows). |
| **Vocabulary prune** | **loop-79** | Retired 6 superseded discovery predicates from loop-78 (archived in `conc-32` evidence); pruned ModelSummary rollup fields; `conc-17` / `conc-22` / `conc-27` marked historical-only. |
| **Confounder identity** | **loop-80** | `ConfounderIdentities` + `StratumVariableIdentityMaps` + `IdentityClusterSummaries`; 15 canonical confounding archetypes; 238 stratum-variable identity maps; `conc-33`. |

**Next up:** **loop-87** — methods-paper export on the pruned instrument (theorem portfolio + `InstrumentSpec` packaging). **loop-88** — ALLHAT source verification (REAL? template cells → published-table encode).

---

## The entity model

```
Studies ──< Strata ──< CaseCells            ← raw leaves: (successes, cases) per cell
   └──< Treatments
              │
              └──> StratumSummaries          per-(stratum, treatment) rates, StratumGap,
                                             AllocationBias, WeightedStratumRate
              └──> TreatmentRankings         IsReversal, IsSignFlip, IsStratumUnanimous,
                                             IsSweepFragile, AllocationDistortion,
                                             DistortionType, SignalPurity, LatentFlipPotential,
                                             CorrectedWinner, PolicyImplication
   └──< StratumVariables                     IsConfounder, CausalRole, AdjustmentAppropriate
   └──  ModelSummary                         epistemic rollup across all 238 studies
   └──  InstrumentSpec                       input fields, derived coordinates, theorem catalog
   └──  InvariantChecks                      29 algebraic self-consistency assertions (24 critical PASS; 5 warning-only)
   └──  Conclusions / Methodology / Loops    witnessed claims and build narrative
   └──  DiscoveryHypotheses / DiscoveryFindings   18 active pre-registered corpus experiments
   └──  ConfounderIdentities / StratumVariableIdentityMaps / IdentityClusterSummaries
   └──  CandidateStudyCatalog               import catalog (235 imported, 2 blocked, 0 queued)
   └──  CorpusCatalogSummary / CorpusDomains / DomainExpansionTargets
   └──  AllocationSweep / SweepStudyConfig   parameter-space exploration (2,380 sweep rows)
   └──  SubstrateConformanceFields          OWL-SHACL cross-substrate receipt field list
```

`CaseCells` is the ground truth. The Postgres view chain is:
`vw_case_cells` → `vw_stratum_summaries` → `vw_treatment_rankings` → `vw_model_summary`.

---

## The five distortion types

Beyond the binary `IsSignFlip`, every study is classified on a continuous distortion plane (allocation bias × paradox strength). Type C was split in loop-43 into **C+** (amplification) and **C−** (compression). Loop-70 re-keyed A/B on **stratum unanimity** rather than legacy ratio bands.

| Type | Criterion | Count | Notes |
|---|---|---|---|
| **A — Unanimous reversal** | `IsSignFlip=true` AND `IsStratumUnanimous=true` | 78 | Canonical Simpson's paradox — every stratum agrees, pooled winner flips |
| **B — Heterogeneous reversal** | `IsSignFlip=true` AND `IsStratumUnanimous=false` | 8 | Sign flip with mixed stratum directions (e.g. rosiglitazone-mi-pool-2007) |
| **C+ — Amplification** | `IsSignFlip=false`, allocation distorts gap upward | 8 | Pooled winner correct but effect overstated |
| **C− — Compression** | `IsSignFlip=false`, allocation distorts gap downward | 7 | Pooled winner correct but effect understated |
| **D — Neutral** | `IsSignFlip=false`, low distortion | 137 | Pooled analysis trustworthy at observed allocation |

Full roster: `SELECT study, distortion_type FROM vw_treatment_rankings ORDER BY distortion_type, study` or browse the rulebook `TreatmentRankings` table.

`AllocationDistortion = |WeightedStratumGapSum − SignedPooledGap|` — the scalar distance between equal-weight strata and allocation-weighted pooled numbers.

`SignalPurity = |CorrectedGap| / (|CorrectedGap| + AllocationDistortion)` — when below 0.5, allocation noise exceeds true signal. All sign-flip studies satisfy this (witnessed as **theorem** in loops 64/76).

`IsSweepFragile` (loop-70) names Type-D studies whose pooled gap crosses zero under allocation sweep despite no manifest sign-flip at observed weights.

---

## The 238-study corpus

**233 real + 5 synthetic** structural controls (collider-only, non-collapsibility, balanced, compressed, plus domain-tagged synthetics).

| Domain (Studies tag) | Real studies |
|---|---|
| epidemiology | 44 |
| economics | 39 |
| education | 35 |
| medicine | 34 |
| social-science | 33 |
| sports | 24 |
| legal | 20 |
| public-health-smoking | 4 |

Loop-67 also registered **11 expansion target domains** in `CorpusDomains` (criminal-justice, clinical-trials, financial-lending, hiring-promotion, online-ab-testing, etc.) — used for catalog prioritization and gap tracking via `DomainExpansionTargets`.

Expansion waves added underrepresented **mechanisms**: collider/selection bias, cross-design confounding (HRT observational vs RCT), composition-over-time, Robinson/ecological aggregation, vaccination base-rate structures, and non-collapsibility — not just classic 2×2 sign-flip tables.

**Import provenance:** catalog rows carry `DataSourceNote` provenance (`Prov: DOC`, `Prov: SYNTH`, `Prov: REAL?`). REAL? imports use approximate cell counts from cited primary/secondary literature; they are labeled as such in `Source` and `DataSourceNote`. Nothing is silently invented.

**Catalog status:** 237 catalog rows — **235 imported**, **2 blocked** (duplicates), **0 candidates** queued. All imported rows link to live studies (`inv-catalog-imported-linked`).

**Data acquisition (out-of-channel):** bulk download artifacts for expansion-wave-1 candidates live under `data/raw/<study-id>/` with manifests in `data/acquisition/`. See `data/acquisition/README.md` for the acquisition loop and merge scripts (`merge-acquisition-manifest.py`, `merge-pdf-extraction-manifest.py`).

Synthetic and counterfactual studies are structural controls — they prove impossibility claims (balanced allocation → reversal structurally impossible) and isolate single causal factors.

---

## What the model witnesses

All **29 algebraic invariants** pass across the full corpus (**0 critical failures** — any critical `FailCount > 0` breaks the build):

- Every row has a `DistortionType` in {A, B, C+, C−, D}
- Types A and B always have `IsSignFlip=TRUE`; C+, C−, and D always have `IsSignFlip=FALSE`
- Type A rows satisfy `IsStratumUnanimous=TRUE` (loop-70)
- When `IsSignFlip=TRUE`, corrected and pooled winners always disagree
- When `DistortionType=D`, corrected and pooled winners always agree
- `SIGN(CorrectedGap) = SIGN(WeightedStratumGapSum)` for all rows
- Sign-flip rows have `SignalPurity < 0.5` (`inv-sign-flip-signal-purity-max`)
- `CorrectedGap` invariant under allocation sweep (`inv-corrected-gap-invariant`)
- Confounder sign-flips ⟺ `IsParadoxExplained=TRUE` (biconditional, loop-74)
- Collider/selection studies: zero manifest sign-flips at observed allocation (conditional theorem, loop-75)
- …plus ingestion-contract, phase-diagram, catalog-linkage, and discovery-rollup checks

Five **warning-only** invariants retain legacy ratio-geometry witnesses from pre-loop-70 taxonomy.

The `ModelSummary` row witnesses epistemic coverage in one query:

| Metric | Value |
|---|---|
| StudyCount | 238 |
| RealStudyCount | 233 |
| SignFlipCount (= ReversalCount) | 86 |
| Type A / B / C+ / C− / D | 78 / 8 / 8 / 7 / 137 |
| ExplainedCount | 83 |
| AllocationSweep rows | 2,380 (238 studies × 10 steps) |
| DiscoveryHypotheses | 18 active (6 superseded predicates retired in loop-79) |
| ConfounderIdentities | 15 canonical archetypes; 238 stratum-variable maps |
| TheoremCount | 5 Category=theorem conclusions |

**Reversal recovery is free.** `CorrectedGap = WeightedStratumGapSum` is already derived from the allocation arithmetic. The allocation-corrected winner (`CorrectedWinner`) and its policy implication (`CorrectedPolicyImplication`) cost zero new data — now promoted to **theorem** (conc-28).

**34 conclusions** are stored as first-class rows in `Conclusions` — from "Simpson's paradox is a derived fact" (loop-04) through loop-80 confounder-identity layer (`conc-33`). Five carry **Category=theorem** (signal purity ceiling, CorrectedGap invariance, Explained↔Confounder, collider-no-manifest). The PDF and `/conclusions` UI group them by epistemic tier, not a single headline tally.

---

## Explorer UI

Run `./start.sh` to boot the backend (`:3001`) and Vite frontend (`:5173`). Default landing route is `/overview`; sidebar prioritizes Discovery Research.

| Route | View |
|---|---|
| `/discovery` | **Discovery Research** — pre-registered hypotheses tested against live views |
| `/conclusions` | **Conclusions & Findings** — tiered epistemic claims, discovery PASS/FAIL, scope boundaries |
| `/overview` | **Study Overview** — allocation-distortion plane; each dot is a study, colored by type |
| `/stratum` | **Stratum Breakdown** — per-stratum rates vs pooled rates for any selected study |
| `/weights` | **Allocation Weights** — case distribution across strata vs stratum success rate |
| `/sandbox` | **Interactive Sandbox** — adjust raw counts via sliders; derived fields update live via the same Postgres views (rollback transaction) |
| `/model` | **Model Summary** — rollup across all 238 studies: type distribution, paradox strength, definition-delta table |
| `/sweep` | **Allocation Sweep** — parameter-space exploration of allocation geometry |
| `/phase` | **Phase Diagram** — five-type taxonomy in synthetic parameter space |
| `/catalog` | **Import Backlog** — candidate catalog with acquisition vs ingestion status |
| `/instrument` | **Instrument Dashboard** — `InstrumentSpec` adapter contract and screening coordinates |
| `/loops` | **Leopold Loops** — build history from the rulebook `Loops` table; on-demand OWL-SHACL conformance runner |
| `/dag` | **Rulebook DAG** (via ⬇ download menu) — Explainer DAG over every table and derived field |

**Download menu (⬇):** RuleSpeak HTML/PDF, Excel export (live `vw_*`), corpus summary PDF, email-ready standalone HTML, Rulebook DAG link.

Standalone HTML: [`simpsons-paradox-explorer.html`](simpsons-paradox-explorer.html).

---

## Corpus summary (PDF)

**[`simpsons-paradox-summary.pdf`](simpsons-paradox-summary.pdf)** — offline export of the same data as `/conclusions` (witnessed conclusions, discovery PASS/FAIL, corpus counts). Regenerated on `./init-db.sh`. With `./start.sh`: [localhost:3001/simpsons-paradox-summary.pdf](http://localhost:3001/simpsons-paradox-summary.pdf) or `GET /api/export/summary-pdf`.

---

## Cross-substrate conformance (OWL)

`effortless build` generates an OWL ontology + SHACL shapes under `owl/` via `rulebook-to-owl`. A **Postgres vs OWL-SHACL** receipt diff runs on demand (not during build — ~10 min at 238 studies):

- **UI:** `/loops` → **Run conformance test**
- **CLI:** `./owl/run-conformance.sh` (after `effortless build` + `./init-db.sh`)

The `owl-conformance` transpiler is disabled in `effortless.json`; conformance is intentionally opt-in.

---

## What this is not

**Geometric, not causal.** The instrument classifies allocation distortion and flags the allocation-corrected winner from arithmetic alone. Whether it is safe to act on that correction as a causal claim is answered by `AdjustmentAppropriate`, which gates on `ConditioningRisk` and `CausalClaimStatus`. Berkeley is the proof: high `AllocationDistortion` but `CausalRole=contested` — the instrument classifies the geometry; the researcher supplies the causal account.

**Deductive vs. empirical.** `Conclusions` rows tagged **theorem** (e.g. SignalPurity bound, CorrectedGap invariance) are true by the definitions — invariant checks on them are regression tests for the transpiler, not independent discoveries about nature. Rows tagged **domain** and active **DiscoveryHypotheses** (e.g. H-latent-d, H-catalog-exact-match) are corpus statistics on a convenience sample — pre-registered before expansion, still provisional. Six domain-pattern hypotheses superseded at N=238 were retired from the active DAG in loop-79 (evidence preserved in `conc-32`). Do not treat a discovery PASS the same way you treat a theorem; the UI separates them for a reason.

Catalog `ExpectedDistortionType` is a **pre-encoding guess**. Observed `DistortionType` is what the DAG computes from cell counts. Mismatches are data for pattern analysis, not bugs — unless they violate an algebraic invariant (~39.1% exact match on 235 imported catalog rows at N=238; sign-flip prediction ~54.1% per loop-78 audit).

---

## Build & run

**Prerequisites:** [Effortless CLI](https://effortlessapi.com), local Postgres, Node.js.

Transpilers are registered in `effortless.json` and run on hosted Effortless infrastructure (Control Plane). No local transpiler bus is required for a routine build.

```bash
git status                               # always check first — rulebook JSON is sacred
effortless build                         # hosted transpilers → postgres/, rulespeak/, owl/, explainer DAG, summary PDF
cd effortless-postgres && ./init-db.sh   # drop and recreate local DB; writes simpsons-paradox-summary.pdf
./start.sh                               # explorer UI on :5173 (API on :3001)
```

**Build outputs** (from `effortless.json`):

| Transpiler | Output |
|---|---|
| `rulebook-to-postgres` | `effortless-postgres/` SQL + seed data |
| `rulebook-to-rulespeak` | `rulespeak/rulespeak.html` (+ optional PDF via download menu) |
| `rulebook-to-owl` | `owl/src/` ontology + SHACL |
| `rulebook-to-explainer-dag` | `app/frontend/public/rulebook-explainer-dag/` |
| `rulebook-to-xlsx` | on-demand Excel export via API |
| `generate-summary-pdf.sh` | `simpsons-paradox-summary.pdf` |

No migrations. Edit rulebook → `effortless build` → `./init-db.sh` → DB reflects it.

**Bulk encode / import scripts** live under `scripts/`:

| Script | Purpose |
|---|---|
| `loop-67-corpus-expansion.py` | Ingest `corpus-expansion-plan.md` into catalog |
| `loop-68-expansion-wave1.py` | Wave 1 stress-batch encode |
| `loop-71-expansion-wave2.py` | Wave 2 domain-gap encode |
| `loop-77-expansion-wave3-bulk-encode.py` | Wave 3 bulk encode (122 studies) |
| `loop-69-expansion-discovery.py`, `loop-72-expansion-discovery.py` | Wire discovery findings after encode waves |
| `loop-70-mechanistic-vocabulary.py` | IsStratumUnanimous / IsSweepFragile |
| `loop-73-76-theorem-formalization-plan.py` | Theorem promotion scaffolding |
| `apply-loops-79-80.py` | Vocabulary prune (loop-79) + confounder identity layer (loop-80) |
| `generate-allocation-sweep-all.py` | Regenerate sweep rows after corpus changes |
| `acquisition-loop-expansion-wave-1.py` | Out-of-channel data download queue |
| `merge-acquisition-manifest.py`, `merge-pdf-extraction-manifest.py` | Merge acquisition metadata into catalog |

Run allocation-sweep regeneration after any import that adds studies.

---

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.
