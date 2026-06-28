# Simpson's Paradox — Witnessed DAG Demo

A **witnessed dependency graph** that turns Simpson's paradox from a textbook curiosity into a computational object. **46 published and synthetic studies** — spanning medicine, epidemiology, law, sports, education, economics, and social science — are poured into a single entity model. The instrument was built across **57 Leopold loops** (loop-01 through loop-57, all complete; 52 rows documented in the `Loops` table). Every derived value falls out of formulas declared in the rulebook. No inference is ever hand-entered.

---

## What this is

An **Effortless Rulebook (ERB)** domain. The SSoT is `effortless-rulebook/simpsons-paradox-rulebook.json`. All other artifacts — Postgres SQL, views, the explorer UI — are mechanically derived from it.

The paradox **emerges** from the model. It is not modeled directly. `IsReversal` and `IsSignFlip` are derived booleans that fall out of comparing pooled rates to per-stratum rates. There is no `ReversalDetection` entity.

The `Loops` table is the build history — each row documents what domain concept was introduced, which natural-language question it answers, and what was witnessed.

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
   └──  ModelSummary                         epistemic rollup across all 46 studies
   └──  InstrumentSpec                       input fields, derived coordinates, adapter contract
   └──  InvariantChecks                      21 algebraic self-consistency assertions (all PASS)
   └──  Conclusions / Methodology / Loops    witnessed claims and build narrative
   └──  CandidateStudyCatalog               import backlog for corpus expansion
   └──  AllocationSweep / SyntheticPhase     parameter-space exploration
```

`CaseCells` is the ground truth. The Postgres view chain is:
`vw_case_cells` → `vw_stratum_summaries` → `vw_treatment_rankings` → `vw_model_summary`.

---

## The five distortion types

Beyond the binary `IsReversal`, every study is classified on a continuous distortion plane (allocation bias × paradox strength). Type C was split in loop-43 into **C+** (amplification) and **C−** (compression) — epistemically opposite errors with opposite clinical implications.

| Type | Criterion | Count | Representative studies |
|---|---|---|---|
| **A — Full reversal** | `IsSignFlip=true`, unanimity across all strata | 6 | kidney-1986, reintjes-2000, radelet-1981, jeter-justice-1997, phe-covid-2021, rosiglitazone-mi-pool-2007 |
| **B — Sign flip** | `IsSignFlip=true`, not unanimous | 10 | berkeley-1973, berkeley-six-dept-1973, appleton-1996, birth-weight-paradox, sat-wainer-1986, … |
| **C+ — Amplification** | `IsSignFlip=false`, `DistortionRatio > 1` | 5 | titanic-1912, melanoma-altman-1991, cesarean-birth-weight-2006, coffee-smoking-lung-1968, uc-irvine-admissions-1985 |
| **C− — Compression** | `IsSignFlip=false`, `DistortionRatio ∈ (0,1)` | 4 | hannan-1994, compressed-synthetic, hanley-power-lines-2000, gender-pay-gap-industry |
| **D — Neutral** | `AllocationDistortion < 0.01` | 21 | balanced-synthetic, kidney-balanced, coffee-tverdal-2020, folic-fortification-2000, … |

`AllocationDistortion = |WeightedStratumGapSum − SignedPooledGap|` — the scalar distance between what equal-weight strata would show and what the actual allocation-weighted pooled number shows. Berkeley still scores **0.193** (nearly twice kidney-1986's 0.099), despite `IsReversal=false`.

`SignalPurity = |CorrectedGap| / (|CorrectedGap| + AllocationDistortion)` — when below 0.5, allocation noise exceeds true signal; all type-A/B reversal studies satisfy this (witnessed in loop-44).

---

## The 46-study corpus

| Domain | Studies | Types present |
|---|---|---|
| epidemiology | 12 | A, B, C+, C−, D |
| medicine | 11 | A, B, C+, C−, D |
| sports | 6 | A, B, D |
| social-science | 5 | B, C+, D |
| education | 4 | B, D |
| economics | 3 | C−, D |
| legal | 2 | A, B |
| synthetic | 3 | C−, D (structural controls) |

**Type A (6):** jeter-justice-1997, kidney-1986, phe-covid-2021, radelet-1981, reintjes-2000, rosiglitazone-mi-pool-2007

**Type B (10):** appleton-1996, berkeley-1973, berkeley-six-dept-1973, birth-weight-paradox, ironman-gender-age-2010, north-carolina-death-penalty-1990, panama-sweden-mortality-1975, rogers-nicewander-1988, sat-wainer-1986, steroid-asthma-severity

**Type C+ (5):** cesarean-birth-weight-2006, coffee-smoking-lung-1968, melanoma-altman-1991, titanic-1912, uc-irvine-admissions-1985

**Type C− (4):** compressed-synthetic, gender-pay-gap-industry, hanley-power-lines-2000, hannan-1994

**Type D (21):** balanced-synthetic, clemens-bly-1998, coffee-tverdal-2020, diabetes-metformin-bmi, exercise-cholesterol-age, florida-reef-fish-1994, folic-acid-neural-tube-1991, folic-fortification-2000, housing-discrimination-audit, income-education-state, kidney-balanced, kidney-dialysis-facility-1990, lucente-baseball-1995, oncology-trial-stage, open-university-1975, pisa-immigration-2015, red-meat-colorectal-bmi, schizophrenia-antipsychotic, wainer-sat-states-1992, warfarin-bleeding-age, wilson-batting-2000

Synthetic and counterfactual studies are structural controls — they prove impossibility claims (balanced allocation → reversal structurally impossible) and isolate single causal factors. **43 of 45 catalog candidates** are linked to live studies (`inv-catalog-imported-linked`).

---

## What the model witnesses

All **21 algebraic invariants** pass across the full corpus (**0 failures** — any `FailCount > 0` breaks the build):

- Every row has a `DistortionType` in {A, B, C+, C−, D}
- Types A and B always have `IsSignFlip=TRUE`; C+, C−, and D always have `IsSignFlip=FALSE`
- When `IsSignFlip=TRUE`, corrected and pooled winners always disagree
- When `DistortionType=D`, corrected and pooled winners always agree
- Type-D rows always have `AllocationDistortion < 0.01`
- `SIGN(CorrectedGap) = SIGN(WeightedStratumGapSum)` for all rows
- C+ rows have `DistortionRatio > 1`; C− rows have `DistortionRatio ∈ (0,1)`
- All reversal studies (types A/B) have `SignalPurity < 0.5`
- …plus ingestion-contract, phase-diagram, and catalog-linkage checks

The `ModelSummary` row witnesses epistemic coverage in one query:

| Metric | Value |
|---|---|
| StudyCount | 46 |
| ReversalCount (unanimity) | 8 |
| ReversalCountV2 (sign-flip) | 16 |
| Type A / B / C / D | 6 / 10 / 9 / 21 |
| ExplainedCountV2 | 13 |

**Reversal recovery is free.** `CorrectedGap = WeightedStratumGapSum` is already derived from the allocation arithmetic. The allocation-corrected winner (`CorrectedWinner`) and its policy implication (`CorrectedPolicyImplication`) cost zero new data.

**14 witnessed conclusions** are stored as first-class rows in `Conclusions` — from "Simpson's paradox is a derived fact" (loop-04) through the SignalPurity theorem (loop-44) and the five-type instrument spec (loop-26).

---

## Explorer UI

Run `./start.sh` to boot the backend (`:3001`) and Vite frontend (`:5173`).

| Route | View |
|---|---|
| `/overview` | **Study Overview** — allocation-distortion plane; each dot is a study, colored by type |
| `/stratum` | **Stratum Breakdown** — per-stratum rates vs pooled rates for any selected study |
| `/weights` | **Allocation Weights** — case distribution across strata vs stratum success rate |
| `/sandbox` | **Interactive Sandbox** — adjust raw counts via sliders; derived fields update live via the same Postgres views (rollback transaction) |
| `/model` | **Model Summary** — rollup across all 46 studies: type distribution, paradox strength, definition-delta table |
| `/sweep` | **Allocation Sweep** — parameter-space exploration of allocation geometry |
| `/phase` | **Phase Diagram** — five-type taxonomy in synthetic parameter space |
| `/catalog` | **Import Backlog** — candidate studies awaiting ingestion |
| `/instrument` | **Instrument Dashboard** — `InstrumentSpec` adapter contract and screening coordinates |

The sandbox presets load kidney-1986, Berkeley, and the neutral control. A standalone email-ready HTML export lives at `simpsons-paradox-explorer.html`.

---

## What this is not

The instrument is **geometric**: it classifies allocation distortion and flags the allocation-corrected winner from arithmetic alone. Whether it is safe to act on that correction as a causal claim is answered by `AdjustmentAppropriate`, which gates on `ConditioningRisk` and `CausalClaimStatus`. Berkeley is the proof: it has the highest `AllocationDistortion` (0.193) but `CausalRole=contested` — the instrument classifies the geometry; the researcher supplies the causal account.

---

## Build discipline

```bash
git status                               # always check first
cd effortless-postgres && ./init-db.sh   # drop and recreate from rulebook
```

No migrations. Edit rulebook → build → DB reflects it.

---

## Local transpiler bus (`localhost:4242`)

> **All 13 local transpilers live on `localhost:4242`.** Once you run
> `./start.sh` from the repo root, the ssotme-proxy exposes every repo-local
> transpiler — `rulebook-to-postgres`, `rulebook-to-python`, `rulebook-to-golang`,
> `rulebook-to-cobol`, `rulebook-to-owl`, and more — as first-class `ssotme://`
> routes any `effortless build` can call.
