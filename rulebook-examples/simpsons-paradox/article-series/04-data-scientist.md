# Trust the Artifact, Not the Autocomplete
## Article 4: A Data Scientist's Guide to Predicate Invention

**Series position:** 4 of 10
**Audience:** Data scientists, statisticians, ML researchers, feature engineers
**Central metaphor:** Feature engineering with an audit trail.
**Tone:** Methodologically serious, speaks the language of reproducibility and hypothesis testing

---

*This is part 4 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# Predicate Invention as Hypothesis Generation Under Test

LLMs do not eliminate statistical discipline. They can expand the hypothesis space, but only disciplined pipelines can shrink it back down.

This is a claim about methodology, not technology. Feature engineering — the process of creating derived variables that encode domain knowledge — has always been an exercise in judgment: what concept matters, how to measure it, whether the measurement is algebraically consistent and empirically meaningful. LLMs can accelerate the proposal phase of that process dramatically. They cannot replace the validation phase. The pipeline that takes a proposed predicate and decides whether it earns a place in the model is exactly the same discipline it has always been.

What changes is how explicit that discipline becomes.

---

## The feature engineering problem

A data scientist builds a feature called "customer health score." It is a weighted combination of recency, frequency, monetary value, support tickets, and product usage. The weights were chosen by intuition, adjusted after model performance improved, and never documented as a principled formula. Six months later, nobody knows why the feature is defined the way it is, and a new teammate accidentally reweights it during a pipeline refactor.

This is not an AI problem. It is a documentation and reproducibility problem that LLMs make significantly worse — because now the intuition step can be outsourced to a model that generates confident-sounding feature names with no grounding at all. The model proposes "customer health score" with a formula. The formula looks plausible. It goes into the pipeline. Nobody checked whether it is algebraically consistent with the other features, or whether it means what the name implies, or whether there is a simpler formula that provably captures the same quantity.

The problem is not that the LLM proposed something wrong. It is that no system exists to find out.

---

## Predicate invention as a research methodology

This project used LLMs as predicate inventors — generators of candidate concepts for a domain model. The domain: Simpson's Paradox. The question: what vocabulary do you need to fully characterize when and why aggregated data misleads?

The LLM suggested: `IsSignFlip`, `AllocationDistortion`, `DistortionType`, `SignalPurity`, `CorrectedWinner`, `AllocationDirection`, `DistortionRatio`. These are proposed predicates — candidate features with names but no standing until tested.

Each proposed predicate passes through three gates before it becomes a governing concept.

**Gate 1 — Formal definition**: the concept is written as an explicit formula in the rulebook. Not "a measure of how much the allocation matters" but the precise algebraic expression that computes it from the leaf inputs. If the formula cannot be written, the concept is not defined.

**Gate 2 — Algebraic test**: invariant checks verify that mathematical consequences hold across the full corpus. If SignalPurity is supposed to measure signal-to-noise, then reversal studies — where the noise is definitionally larger than the signal — should have SignalPurity below 0.5. That is an algebraic consequence, not a corpus observation. The invariant verifies it.

**Gate 3 — Corpus test**: the predicate is computed against all 238 studies. Patterns are recorded in the Conclusions table under an epistemic tier. What fraction of studies are Type A? What is the average AllocationDistortion by domain? These are corpus findings — contingent on this dataset, revisable with more data.

Only after all three gates does a predicate become a governing concept.

---

## The five-type taxonomy: a predicate invented and verified

`DistortionType` classifies every study in the corpus into one of five categories. The classification was proposed by an LLM and accepted after passing all three gates.

The five types:
- **A**: sign-flip, all strata agree — the confounding is unanimous and manifest
- **B**: sign-flip, strata disagree — the confounding is partial
- **C+**: no flip, but the pooled result is amplified by allocation (DistortionRatio > 1)
- **C−**: no flip, but the pooled result is compressed (DistortionRatio in (0,1))
- **D**: no flip, no meaningful distortion — the pooled analysis is trustworthy

The formula (verbatim from the rulebook):

```
=IF({{IsSignFlip}},
  IF({{IsStratumUnanimous}}, "A", "B"),
  IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
         ABS({{SignedPooledGap}}) > ABS({{CorrectedGap}}) + 0.001), "C+",
  IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
         ABS({{SignedPooledGap}}) < ABS({{CorrectedGap}}) - 0.001), "C-",
  "D")))
```

Corpus distribution at N=238: A=78, B=8, C+=8, C−=7, D=137.

Invariant `inv-taxonomy-complete` verifies that every study in the corpus receives exactly one classification (PassCount=40, FailCount=0). The taxonomy is complete over the domain. No study falls through the cracks, no study is doubly classified. If you change the formula in a way that violates this, the build fails.

---

## Signal purity: from feature to theorem

`SignalPurity` was proposed as a measure of how much of the observed gap between treatments is attributable to a real treatment difference versus allocation noise:

```
SignalPurity = ABS(CorrectedGap) / (ABS(CorrectedGap) + AllocationDistortion)
```

The formula is simple: real signal divided by total signal, where total signal equals real plus noise. After computing this across all 238 studies, the corpus showed: reversal studies (AllocationDirection='reversal') had SignalPurity below 0.5 in every case.

This is not a corpus finding. It is an algebraic consequence.

The reason: in a reversal study, by definition, the allocation noise is large enough to flip the sign of the pooled result. That means the noise must be larger than the signal. Which means the noise-to-total ratio exceeds 0.5. Which means SignalPurity is below 0.5. The formula, the definition of a reversal, and the result are all algebraically locked together. You do not need the corpus to confirm this — you need the corpus to discover that the formula was the right formula to propose.

Invariant `inv-signal-purity-sign-flip`: "AllocationDirection='reversal' → SignalPurity < 0.5." PassCount: 12, FailCount: 0.

The corpus averages add something the algebra alone cannot: `AvgSignalPurityReversal ≈ 0.35, AvgSignalPurityNonReversal ≈ 0.70`. The gap is 0.35. Reversal studies, on average, have only 35% real signal. That is a discovery finding — true of this corpus, potentially different in another domain, subject to revision if more studies are encoded. The difference between the theorem and the finding matters epistemically. The invariant is a theorem. The corpus averages are evidence.

---

## Hypothesis pre-registration

The project uses a `DiscoveryHypotheses` table to pre-register predictions before encoding new studies. The pattern should be familiar to anyone who has done clinical research or preregistered an analysis plan. You commit to a prediction before you see the data. The verdict is computed by the build.

Three examples from Loop 62:

**H-causal-manifest**: "All sign-flip studies with StratumCausalRole='confounder' will be manifest (not latent)." Registered before encoding 15 confounder studies. Result: PASS.

**H-econ-zero**: "Economics studies will have 0% sign-flip rate." Registered before expanding the economics subset. Result at N=238: 0 of 8 economics studies show a sign flip. PASS.

**H-domain-dist**: "Epidemiology will have higher AllocationDistortion than education." Registered before encoding either domain fully. Result: epidemiology average 0.050, education average 0.004. PASS.

Pre-registration prevents the most common form of confirmation bias in exploratory data science: noticing a pattern and then claiming you predicted it. In an LLM-assisted workflow, this risk is amplified — the model can generate a post-hoc explanation for any pattern you show it, in convincing prose, faster than you can write down whether you actually predicted that pattern before you looked. The hypothesis table is a technical countermeasure to a methodological failure mode.

---

## Superseded findings: the epistemically honest archive

At N=64, the corpus showed what appeared to be a near-universal dominance of Type D studies — cases where the pooled analysis is trustworthy and no meaningful distortion is present. At N=238, that pattern broke. More studies from medicine and epidemiology expanded the Type A and Type B counts substantially.

The project's `Conclusions` table has an explicit category for this: `corpus-pattern-superseded`. Findings tagged this way are not deleted. They are archived with the loop number where the revision happened and a note explaining what changed.

Two archived findings:
- **conc-17**: finding about Type-D dominance — held at N=64, required revision at N=238
- **conc-22**: domain-level flip-rate characterization that changed when the economics subset was added

Every working data scientist has experienced the moment when a finding that was stable at smaller N collapses as the dataset grows. The epistemically honest response is to record what you used to believe, when you changed your mind, and why. Most pipelines delete the old finding and move on. This architecture keeps the record.

---

## The CorrectedGap invariance: an allocation-free feature

`CorrectedGap = WeightedStratumGapSum = Σ(StratumFraction × StratumGap)` is the allocation-invariant estimate of the true treatment difference. It answers: if the case load were distributed equally across strata, what would the pooled gap be?

Loop 34 produced a sweep across 10 allocation scenarios and witnessed: "SweepCorrectedGap = 0.05367 across all 10 allocation rows (range = 0.0000). SweepPooledGap ranges from +0.130 to −0.084 (range ≈ 0.214), crossing zero near f_L=0.50."

The pooled gap varied by 0.214 — it crossed zero and changed sign — purely as a function of how cases were distributed across strata. The CorrectedGap did not move. It stayed at 0.05367 across all 10 scenarios.

Invariant `inv-corrected-gap-invariant` (PassCount: 238, FailCount: 0) confirms this algebraic property holds across the entire corpus. CorrectedGap is not a better estimate of the treatment effect — it is the allocation-invariant estimate, by construction. The invariant does not discover this; it verifies it.

This is the kind of feature a data scientist might spend weeks trying to engineer through trial and error. The LLM proposed the name; the formula definition made it precise; the sweep witnessed the invariance; the invariant check locked it in. The whole cycle took one build iteration.

---

## What the methodology adds up to

Feature engineering is not going away. The judgment about which concepts to encode and how to measure them is still human work — or, increasingly, LLM-assisted human work. What this architecture adds is the infrastructure to make that judgment explicit, testable, and auditable.

The LLM expands the hypothesis space: more candidate features, faster, with better names. The three-gate pipeline shrinks it back down: only predicates that can be formally defined, algebraically tested, and empirically meaningful on the actual corpus survive. The loop table records every decision. The invariants enforce every consequence. The supersession archive records every revision.

LLMs do not eliminate statistical discipline. They can expand the hypothesis space. Only disciplined pipelines can shrink it back down.

---

**Also in this series:**
- *Part 3: A Developer Guide to Rulebooks, Builds, and Invariants* — the technical infrastructure: transpilers, SQL views, and the hallucination boundary
- *Part 5: A Knowledge Governance Guide to Institutional Memory* — vocabulary lifecycle, provenance, and the three-tier epistemic system

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*
