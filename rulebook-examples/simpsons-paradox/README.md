# Simpson's Paradox — Witnessed DAG Demo

A **witnessed dependency graph** that turns Simpson's paradox from a textbook curiosity into a computational object. Every derived value — pooled rates, stratified rates, reversal flags, paradox strength, allocation bias — falls out of formulas declared in a single rulebook. No inference is ever hand-entered.

---

## What this is

An **Effortless Rulebook (ERB)** domain. The SSoT is `effortless-rulebook/simpsons-paradox-rulebook.json`. All other artifacts — Postgres SQL, views, the explorer UI — are mechanically derived from it.

The paradox **emerges** from the model. It is not modeled directly. `IsReversal` is a derived boolean that falls out of comparing `PooledWinner` to `PerStratumWinner`. There is no `ReversalDetection` entity.

The methodology: one rulebook, compiled into multiple execution substrates (Postgres, the browser-side explorer) that provably return identical results. The trust boundary is a line in the dependency graph — anything with a formula is derived; anything raw is an input.

---

## The entity model

```
Studies ──< Strata ──< CaseCells   ← raw leaves: (successes, cases) per cell
   └──< Treatments
              │
              └──> StratumSummaries    per-(stratum, treatment) rates, StratumGap,
                                       AllocationBias, WeightedStratumRate
              └──> TreatmentRankings   IsReversal, IsSignFlip, AllocationDistortion,
                                       ParadoxStrength, DistortionType, PolicyImplication
   └──< StratumVariables               IsConfounder, CausalRole
   └──  ModelSummary                   epistemic rollup across all studies
```

`CaseCells` is the ground truth. The Postgres view chain is: `vw_case_cells` → `vw_stratum_summaries` → `vw_treatment_rankings` → `vw_model_summary`.

---

## The four distortion types

Beyond the binary `IsReversal`, the model classifies every study on a continuous distortion plane (allocation bias × paradox strength):

| Type | Criterion | Example |
|---|---|---|
| **A — Full reversal** | `IsSignFlip=true`, `ReversalIntensity=1.0` | kidney-1986 |
| **B — Sign flip** | `IsSignFlip=true`, `ReversalIntensity<1.0` | berkeley-1973 |
| **C — Compressed** | `IsSignFlip=false`, `AllocationDistortion>0` | compressed-synthetic |
| **D — Neutral** | `AllocationDistortion≈0` | balanced-synthetic, kidney-balanced |

`AllocationDistortion = |WeightedStratumGapSum − SignedPooledGap|` — the scalar distance between what equal-weight strata would show and what the actual allocation-weighted pooled number shows. Berkeley scores 0.193 (nearly twice kidney-1986's 0.099), despite `IsReversal=false`.

---

## Studies loaded

| Study | Type | Source |
|---|---|---|
| `kidney-1986` | A | Charig et al., BMJ 1986 |
| `berkeley-1973` | B | Bickel, Hammel & O'Connell, Science 1975 |
| `compressed-synthetic` | C | Synthetic — imbalanced allocation, no sign flip |
| `balanced-synthetic` | D | Synthetic — equal allocation, constant rates |
| `kidney-balanced` | D | Counterfactual — kidney-1986 rates, equal allocation |

Synthetic and counterfactual studies are scaffolding. They prove the inference machinery is sound, not claims about the world.

---

## Explorer UI

`simpsons-paradox-explorer.html` — self-contained, open in any browser. Four views:

- **Study Overview** — scatter plot on the distortion plane; per-study metric cards
- **Stratum Breakdown** — per-stratum bars vs pooled dashed lines (the paradox made visual)
- **Allocation Weights** — how each treatment's cases are distributed across strata; `AllocationBias` per stratum
- **Interactive Sandbox** — sliders for raw counts; `IsReversal`, `IsSignFlip`, `AllocationDistortion` update live

---

## How this compares to existing academic tools

A 110-agent adversarially-verified research sweep found no existing tool that combines all three properties:

| Property | OMOP CDM | metafor (R) | PRIME-IPD | psHarmonize | This model |
|---|---|---|---|---|---|
| Fixed canonical entity schema declared upfront | ✓ | — | — | — | ✓ |
| Derived values declared as formulas in the schema | — | partial | — | — | ✓ |
| Same formulas applied uniformly to all loaded studies | partial | ✓ (aggregates only) | — | — | ✓ |
| Raw individual-level data as input | ✓ | — | ✓ | ✓ | ✓ |

The field has two separate traditions — structural harmonization (OMOP, CDISC SEND) and statistical synthesis (metafor, IPD meta-analysis) — that don't overlap. The rulebook-first DAG is a third thing: a declarative formula layer that is part of the schema definition itself.

---

## Loop plan

The `Loops` table in the rulebook IS the build plan. Each row documents what domain concept was introduced and what natural-language question it answers. Currently at loop 19 complete, loop 20+ planned:

- **Loops 1–4**: Core entities, pooled rates, stratum rates, `IsReversal`
- **Loops 5–8**: `StratumVariables`, `IsConfounder`, exposure fractions, balanced counterfactual, Berkeley 1973
- **Loops 9–12**: `ParadoxStrength`, `ModelSummary`, `StratumFraction`, `WeightedStratumRate` mechanism equation
- **Loops 13–16**: `AllocationBias`, kidney-balanced counterfactual, `ReversalThreshold`, `StratumGap` signed
- **Loops 17–19**: `WeightedStratumGapSum`, `IsSignFlip`, `AllocationDistortion`, `DistortionType` taxonomy (all four types populated)
- **Loop 20**: Hydrator regression — write kidney-1986 through an explicit adapter, diff computed output against known-correct witnessed values (the hydrator is validated when the diff passes, not when the visualizer changes)
- **Loop 20b**: First new real study — pour one non-canonical published dataset through the validated hydrator; result is informative regardless of DistortionType
- **Loops 21–23**: `ScreeningApproximation`, third real dataset, `CausalStructureSignal`
- **Loops 24–26**: `DistortionType` as first-class DAG field, type-C real-data witness, `PolicyImplication`
- **Loops 27–30**: Retrospective validation, literature screen, prospective validation, `InstrumentSpec`
- **Loop 31**: `IsReversal_v2 = IsSignFlip`, retire unanimity criterion as primary definition

---

## Build discipline

```bash
git status                           # always check first
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
