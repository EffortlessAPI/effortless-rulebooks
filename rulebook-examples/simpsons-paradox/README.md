# Simpson's Paradox — Witnessed DAG Demo

A **witnessed dependency graph** that turns Simpson's paradox from a textbook curiosity into a computational object. **96 published and synthetic studies** — spanning medicine, epidemiology, law, sports, education, economics, and social science — share one entity model. The instrument was built across **61+ Leopold loops**. Every derived value falls out of formulas declared in the rulebook; modeling choices are centralized there, not scattered in app code.

**Start at `/conclusions`.** Formal epistemic claims live in the rulebook `Conclusions` table — tiered by claim type (theorem · instrument · corpus snapshot). Loop-61 `DiscoveryHypotheses` split into **consistency checks** (definition-linked, e.g. H-purity) vs **corpus hypotheses** (contingent, provisional). SSoT is the rulebook JSON; the explorer reads `vw_*` views only. Run `./start.sh` → [Conclusions & Findings](http://localhost:5173/conclusions).

[`simpsons-paradox-summary.pdf`](simpsons-paradox-summary.pdf) is a static export of the same tiered state (regenerated on `./init-db.sh`). For framing caveats — geometric vs causal, deductive vs empirical, convenience-sample limits — see [What this is not](#what-this-is-not) below.

---

## What this is

An **Effortless Rulebook (ERB)** domain. The SSoT is `effortless-rulebook/simpsons-paradox-rulebook.json`. All other artifacts — Postgres SQL, views, the explorer UI — are mechanically derived from it.

The paradox **emerges** from the model. It is not modeled directly. `IsReversal` and `IsSignFlip` are derived booleans that fall out of comparing pooled rates to per-stratum rates. There is no `ReversalDetection` entity.

The `Loops` table is the build history — each row documents what domain concept was introduced, which natural-language question it answers, and what was witnessed.

---

## Recent loops (55–59)

| Loop | What landed |
|---|---|
| **loop-55** | Bulk import: 24 `CandidateStudyCatalog` rows encoded in one session → **90+ studies** (37 real, 3 synthetic). Catalog discipline: imported vs candidate vs blocked. |
| **loop-56** | Corpus-wide **AllocationSweep**: `SweepStudyConfig` (one row per study), **400 sweep rows** (10 allocation steps × 90+ studies). `inv-corrected-gap-invariant` proven for the full corpus. |
| **loop-57** | Wave 2: six canonical meta-analysis / composition studies (Berkeley six-dept, rosiglitazone naive pool, ironman gender-age, panama-sweden, hanley power-lines, coffee-smoking) → **90+ studies**. |
| **loop-58** | Wave 3: **50 candidate studies** from a curation worksheet — collider/selection, cross-design HRT, composition-over-time, Robinson ecological, non-collapsibility. **17 DOC+SYNTH** fully encoded immediately; **32 REAL?** queued with provenance flags → **90+ studies**. |
| **loop-59** | REAL? backlog drain: all **33** remaining queued candidates imported with **cited approximate 2×K tables** (same provenance pattern as existing rows like `hannan-1994`) → **90+ studies**. Import backlog now **empty** (`candidate_count = 0`). |

**Next up (loop-60):** `TypePredictionMatch` audit — how often does catalog `ExpectedDistortionType` match observed `DistortionType` across the full corpus?

---

## The entity model

```
Studies ──< Strata ──< CaseCells            ← raw leaves: (successes, cases) per cell
   └──< Treatments
              │
              └──> StratumSummaries          per-(stratum, treatment) rates, StratumGap,
                                             AllocationBias, WeightedStratumRate
              └──> TreatmentRankings         IsReversal, IsSignFlip, AllocationDistortion,
                                             DistortionType, SignalPurity, AllocationDirection,
                                             CorrectedWinner, PolicyImplication
   └──< StratumVariables                     IsConfounder, CausalRole, AdjustmentAppropriate
   └──  ModelSummary                         epistemic rollup across all 90+ studies
   └──  InstrumentSpec                       input fields, derived coordinates, adapter contract
   └──  InvariantChecks                      21 algebraic self-consistency assertions (all critical PASS)
   └──  Conclusions / Methodology / Loops    witnessed claims and build narrative
   └──  CandidateStudyCatalog               import backlog (93 imported, 2 blocked, 0 queued)
   └──  CorpusCatalogSummary                backlog readiness metrics
   └──  AllocationSweep / SyntheticPhase     parameter-space exploration (960 sweep rows)
```

`CaseCells` is the ground truth. The Postgres view chain is:
`vw_case_cells` → `vw_stratum_summaries` → `vw_treatment_rankings` → `vw_model_summary`.

---

## The five distortion types

Beyond the binary `IsReversal`, every study is classified on a continuous distortion plane (allocation bias × paradox strength). Type C was split in loop-43 into **C+** (amplification) and **C−** (compression) — epistemically opposite errors with opposite clinical implications.

| Type | Criterion | Count | Representative studies |
|---|---|---|---|
| **A — Full reversal** | `IsSignFlip=true`, ratio ∈ (−1, 0) | 7 | kidney-1986, reintjes-2000, radelet-1981, jeter-justice-1997, phe-covid-2021, rosiglitazone-mi-pool-2007, flu-vaccine-elderly-mortality |
| **B — Sign flip** | `IsSignFlip=true`, ratio < −1 or partial unanimity | 11 | berkeley-1973, berkeley-six-dept-1973, appleton-1996, birth-weight-paradox, cochran-smoking-1968, sat-wainer-1986, … |
| **C+ — Amplification** | `IsSignFlip=false`, `DistortionRatio > 1` | 8 | titanic-1912, melanoma-altman-1991, cesarean-birth-weight-2006, coffee-smoking-lung-1968, israel-covid-severe-2021, covid-italy-china-cfr-2020, … |
| **C− — Compression** | `IsSignFlip=false`, `DistortionRatio ∈ (0,1)` | 6 | hannan-1994, compressed-synthetic, hanley-power-lines-2000, gender-pay-gap-industry, nba-shooting-shot-mix, … |
| **D — Neutral** | `AllocationDistortion < 0.01` (same winner, low distortion) | 64 | kidney-balanced, balanced-synthetic, coffee-tverdal-2020, measles-outbreak-vaccinated, air-pollution-mortality-city, … |

Full roster: query `SELECT study, distortion_type FROM vw_treatment_rankings ORDER BY distortion_type, study` or browse the rulebook `TreatmentRankings` table.

`AllocationDistortion = |WeightedStratumGapSum − SignedPooledGap|` — the scalar distance between what equal-weight strata would show and what the actual allocation-weighted pooled number shows. Berkeley still scores **0.193** (nearly twice kidney-1986's 0.099), despite partial stratum unanimity.

`SignalPurity = |CorrectedGap| / (|CorrectedGap| + AllocationDistortion)` — when below 0.5, allocation noise exceeds true signal; all type-A/B reversal studies satisfy this (witnessed in loop-44).

---

## The 90+ study corpus

**90+ real and synthetic** structural controls (collider-only, non-collapsibility, balanced, compressed, plus domain-tagged synthetics).

| Domain | Real studies | Types present |
|---|---|---|
| epidemiology | 22 | A, B, C+, C−, D |
| medicine | 21 | A, B, C+, C−, D |
| social-science | 13 | B, C+, D |
| sports | 11 | A, B, C−, D |
| education | 9 | B, D |
| economics | 8 | C−, D |
| legal | 7 | A, B, D |

Wave 3–4 expansion added underrepresented **mechanisms**: collider/selection bias, cross-design confounding (HRT observational vs RCT), composition-over-time, Robinson/ecological aggregation, vaccination base-rate structures, and non-collapsibility — not just classic 2×2 sign-flip tables.

**Import provenance:** catalog rows carry `DataSourceNote` provenance (`Prov: DOC`, `Prov: SYNTH`, `Prov: REAL?`). REAL? imports use approximate cell counts from cited primary/secondary literature; they are labeled as such in `Source` and `DataSourceNote`. Nothing is silently invented.

**Catalog status:** 95 catalog rows — **93 imported**, **2 blocked** (duplicates), **0 candidates** queued.

Synthetic and counterfactual studies are structural controls — they prove impossibility claims (balanced allocation → reversal structurally impossible) and isolate single causal factors. All imported catalog rows are linked to live studies (`inv-catalog-imported-linked`).

---

## What the model witnesses

All **21 critical algebraic invariants** pass across the full corpus (**0 critical failures** — any critical `FailCount > 0` breaks the build):

- Every row has a `DistortionType` in {A, B, C+, C−, D}
- Types A and B always have `IsSignFlip=TRUE`; C+, C−, and D always have `IsSignFlip=FALSE`
- When `IsSignFlip=TRUE`, corrected and pooled winners always disagree
- When `DistortionType=D`, corrected and pooled winners always agree
- Type-D rows satisfy ratio-near-unity or degenerate-gap exceptions
- `SIGN(CorrectedGap) = SIGN(WeightedStratumGapSum)` for all rows
- C+ rows have `DistortionRatio > 1`; C− rows have `DistortionRatio ∈ (0,1)`
- All reversal studies (types A/B) have `SignalPurity < 0.5`
- …plus ingestion-contract, phase-diagram, catalog-linkage, and import-session-ready checks

One **warning-only** invariant (`inv-pooled-gap-wanders`) may flag studies with narrow sweep range — expected for small-magnitude reversals.

The `ModelSummary` row witnesses epistemic coverage in one query:

| Metric | Value |
|---|---|
| StudyCount | 90+ |
| RealStudyCount | 91 |
| ReversalCount (unanimity) | 18 |
| SignFlipCount | 18 |
| Type A / B / C+ / C− / D | 7 / 11 / 8 / 6 / 64 |
| ExplainedCount | 15 |
| AvgSignalPurity | 0.79 |
| AllocationSweep rows | 960 (90+ studies × 10 steps) |

**Reversal recovery is free.** `CorrectedGap = WeightedStratumGapSum` is already derived from the allocation arithmetic. The allocation-corrected winner (`CorrectedWinner`) and its policy implication (`CorrectedPolicyImplication`) cost zero new data.

**17 conclusions** are stored as first-class rows in `Conclusions` — from "Simpson's paradox is a derived fact" (loop-04) through loop-61 corpus patterns. The PDF and `/conclusions` UI group them by epistemic tier (proved · instrument · corpus snapshot), not a single headline tally.

---

## Explorer UI

Run `./start.sh` to boot the backend (`:3001`) and Vite frontend (`:5173`).

| Route | View |
|---|---|
| `/overview` | **Study Overview** — allocation-distortion plane; each dot is a study, colored by type |
| `/stratum` | **Stratum Breakdown** — per-stratum rates vs pooled rates for any selected study |
| `/weights` | **Allocation Weights** — case distribution across strata vs stratum success rate |
| `/sandbox` | **Interactive Sandbox** — adjust raw counts via sliders; derived fields update live via the same Postgres views (rollback transaction) |
| `/model` | **Model Summary** — rollup across all 90+ studies: type distribution, paradox strength, definition-delta table |
| `/sweep` | **Allocation Sweep** — parameter-space exploration of allocation geometry |
| `/phase` | **Phase Diagram** — five-type taxonomy in synthetic parameter space |
| `/catalog` | **Import Backlog** — candidate catalog (import session complete; backlog drained) |
| `/instrument` | **Instrument Dashboard** — `InstrumentSpec` adapter contract and screening coordinates |

The sandbox presets load kidney-1986, Berkeley, and the neutral control.

## Corpus summary (PDF)

**[`simpsons-paradox-summary.pdf`](simpsons-paradox-summary.pdf)** — offline export of the same data as `/conclusions` (witnessed conclusions, loop-61 discovery PASS/FAIL, corpus counts). Regenerated on `./init-db.sh`. With `./start.sh`: [localhost:3001/simpsons-paradox-summary.pdf](http://localhost:3001/simpsons-paradox-summary.pdf) or `GET /api/export/summary-pdf`.

Standalone HTML: [`simpsons-paradox-explorer.html`](simpsons-paradox-explorer.html).

---

## What this is not

**Geometric, not causal.** The instrument classifies allocation distortion and flags the allocation-corrected winner from arithmetic alone. Whether it is safe to act on that correction as a causal claim is answered by `AdjustmentAppropriate`, which gates on `ConditioningRisk` and `CausalClaimStatus`. Berkeley is the proof: it has the highest `AllocationDistortion` (0.193) but `CausalRole=contested` — the instrument classifies the geometry; the researcher supplies the causal account.

**Deductive vs. empirical.** `Conclusions` rows tagged **theorem** (e.g. SignalPurity bound) are true by the definitions — invariant checks on them are regression tests for the transpiler, not independent discoveries about nature. Rows tagged **domain** and loop-61 **DiscoveryHypotheses** (e.g. H-econ-zero, H-latent-d) are corpus statistics on a convenience sample — pre-registered before expansion, still provisional. Do not treat H-purity PASS the same way you treat a theorem; the UI separates them for a reason.

Catalog `ExpectedDistortionType` is a **pre-encoding guess**. Observed `DistortionType` is what the DAG computes from cell counts. Mismatches are data for pattern analysis, not bugs — unless they violate an algebraic invariant.

---

## Build & run

**Prerequisites:** [Effortless CLI](https://effortlessapi.com), local Postgres, Node.js.

Transpilers are registered in `effortless.json` and run on hosted Effortless infrastructure (Control Plane). No local transpiler bus is required.

```bash
git status                               # always check first — rulebook JSON is sacred
effortless build                         # hosted transpilers → postgres/, rulespeak/, owl/, summary PDF
cd effortless-postgres && ./init-db.sh   # drop and recreate local DB; writes simpsons-paradox-summary.pdf
./start.sh                               # explorer UI on :5173 (API on :3001)
```

No migrations. Edit rulebook → `effortless build` → `./init-db.sh` → DB reflects it. The build pipeline also regenerates `simpsons-paradox-summary.pdf` at the project root (from live `vw_*` views after Postgres is loaded).

Bulk import scripts live under `scripts/` (`bulk-import-candidates.py`, `import-wave3-candidates.py`, `import-wave4-real33.py`, `generate-allocation-sweep-all.py`). Run allocation-sweep regeneration after any import that adds studies.
