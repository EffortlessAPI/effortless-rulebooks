# Build Plan / Discussion Starter: A Witnessed DAG for Stratification Reversals (Simpson's Paradox)

I want to build a small, complete, reproducible domain that turns Simpson's paradox from a textbook curiosity into a **computational object** — a witnessed dependency graph where the reversal is a derived, inspectable fact rather than a story someone tells over a chart. Before I start building, I want your read on the plan, your pushback, and your help sharpening the schema. Treat this as a design review, not a spec to implement silently.

## The methodology I'm building on

The approach is "one rulebook, many runtimes." A single structured **rulebook** (JSON) defines a domain as tables, where every field is one of five first-class kinds:

- **Schema** — the tables and their shapes
- **Data** — raw, entered facts (the leaves; never inferences)
- **Lookups** — pull a value from a related row (INDEX/MATCH-style)
- **Aggregations** — COUNTIFS/SUMIFS/AVERAGEIFS over related rows
- **Functions** — pure calculated cells over other fields

Everything above the raw leaves is a pure, inspectable formula DAG. The same rulebook is then mechanically compiled into multiple execution substrates (e.g. Postgres, Python, Go, and others) that **provably return identical results** against a shared answer key. That cross-substrate agreement is the conformance guarantee: a defect either shows up as divergence across substrates (caught) or as a defect in the specification itself (visible in the rulebook, where a human can see it). No inference is ever hand-entered. The trust boundary is literally a line in the dependency graph: anything with a formula is derived; anything raw is an input.

The build process is an iterative loop:

1. Look at the rulebook.
2. Hypothesize a change (a new concept to make first-class).
3. Update the schema.
4. Push the model into the substrate(s).
5. The substrate computes all higher-order inferences.
6. That computed data is imported back into the rulebook.
7. Repeat — now with the new concept witnessed and first-class.

Each turn of the loop adds one concept as a citizen of the model. Once concepts are first-class columns, you can group, sort, filter, pivot, chart, and cluster **at the level of the inferences**, not just the raw facts — and patterns that are invisible from any single angle become legible once enough angles coexist in the same substrate.

## Why Simpson's paradox is the ideal first domain

Simpson's paradox is the cleanest possible demonstration of the core thesis: **the same fixed data, viewed from one angle, says X; viewed from another, says not-X; and only building both witnesses reveals it.** An aggregate trend points one way; the trend within every stratum points the other way. The reversal is fully latent in the rows the entire time — no new measurement is needed to "discover" it. It becomes knowable only when someone instantiates the specific projection (the stratified angle) alongside the aggregate one and looks.

Three properties make it the strongest candidate for a defensible, uncontroversial, genuinely solid build:

- **The result is a proof, not a prediction.** A witnessed reversal over a real dataset is deductively true given the rows — arithmetic, not a fitted model that "happens to land." Reality can't veto it; no threshold is up for argument. This is the rare case where a finding can be both *novel* (a previously unnoticed reversal in a fresh dataset) and *deductively certain* on the spot, with no replication required.
- **The mechanism is tiny.** At its heart it's two aggregations over the same rows — the pooled trend and the per-stratum trend — plus a comparison that flags when their directions disagree. That makes it tractable to build correctly and easy to audit.
- **The punchline is visceral.** "The trend reverses inside every group" is surprising to nearly everyone the first time they see it and undeniable once shown. It demonstrates the method to a non-specialist audience without requiring them to care about any particular domain.

## The build approach

I want to follow a synthetic-first, real-data-last protocol, and I want to be disciplined about what each phase is allowed to claim:

- **Synthetic phase (build and harden the instrument).** Start with *realistic synthetic data* — raw leaves only, never inferences — for which the correct higher-order results are already known. Add angles one loop at a time: the pooled aggregate, the stratified aggregate, group sizes, weighting, the reversal flag, the deciding stratum. At each loop, confirm the rulebook computes the known-correct inference. The synthetic data is **not** the carrier of any finding; it is scaffolding to prove the inference machinery is sound, independent of any claim about the world.
- **Real-data step (make the one empirical claim).** Once the witness apparatus passes every synthetic angle, pour a *real* dataset through the identical DAG. Any reversal it surfaces is then a witnessed, traceable, deductively certain fact about that real data — with every node drillable down to the raw counts that produced it. Cite the source dataset after this step, not before.

The distinction I care about: the synthetic phase only ever answers "is the inference machinery sound?" The world-claim is made exactly once, at the end, through a structure already shown to be correct — not "train on synthetic and hope it transfers."

## Where I want your input

I'd rather have the plan stress-tested than agreed with. Specifically:

1. **Schema.** What are the right first-class tables and fields? My starting instinct is something like: `Populations` (or `Datasets`), `Strata`, `Observations` (raw counts per cell), and derived layers for pooled rates, per-stratum rates, and a `ReversalDetected` flag with a `DecidingStratum` / deciding-factor field that names *why* the reversal occurs (e.g. which confounder's uneven distribution drives it). What am I missing or over-modeling?

2. **The reversal predicate.** Simpson's reversals come in more than one flavor (rate reversal across two groups; trend-sign reversal in a regression across strata; the aggregation-paradox in weighted means). Should the first build target the cleanest case (two groups, a binary outcome, one stratifying variable) and generalize later, or should the schema be built to hold the general case from loop 1?

3. **What "realistic synthetic" must mean here.** Since the eventual real-data run is the only empirical claim, the realism that matters is in the *raw leaves* — the count structure and the confounder distribution — not the inferences. What properties must the synthetic data share with real data for the hardened instrument to transfer honestly?

4. **Good real datasets for step 7.** Candidates that are open, well-understood, and where a reversal is either famously present (as a validation) or plausibly lurking and unnoticed (as a discovery). The classic admissions and treatment-efficacy examples are validations; I'm also interested in fresh datasets with many strata where a reversal might not yet be documented.

5. **The witnesses.** The whole value is in making the reversal *legible*. What are the right visualizations and tables — the pooled view next to the stratified view, the deciding-stratum drill-down, the group-size weighting that drives the flip — so that a non-technical reader sees exactly what's happening and can trace it to ground truth?

6. **The compounding angle.** Once datasets are rows with reversal-flags as columns, a second-order question appears for free: *what structural property of a dataset predicts whether it harbors a reversal?* Is that worth designing toward from the start, or a distraction from the clean first build?

Start by telling me where this plan is wrong or weak before telling me where it's right. Then let's converge on the loop-1 schema.