# Trust the Artifact, Not the Autocomplete
## Article 4: A Data Scientist's Guide to Predicate Invention

**Series position:** 4 of 10
**Audience:** Data scientists, statisticians, ML researchers, feature engineers
**Central metaphor:** Feature engineering with an audit trail.
**Tone:** Methodologically serious, speaks the language of reproducibility and hypothesis testing

---

## Original Outline

**Title:** *Predicate Invention as Hypothesis Generation Under Test*

**Central metaphor:** Feature engineering with an audit trail.

**Focus:** LLM-assisted abstraction discovery, measurable derived variables, validation, failure cases.

**Translate terms this way:**

| Technical term      | Data science version                 |
| ------------------- | ------------------------------------ |
| Predicate invention | Feature/concept discovery            |
| Rulebook            | Reproducible feature pipeline        |
| Invariant           | Unit test on derived statistics      |
| Corpus hypothesis   | Non-inferential exploratory finding  |
| Theorem claim       | Algebraic consequence of definitions |

**Article angle:**
This version should be the most direct bridge from familiar data science practice. It says: feature engineering often hides judgment. Here, the judgment is externalized, named, computed, tested, and replayed. The LLM proposes candidate features; the corpus and invariants punish bad abstractions.

**Key lesson:**
"LLMs do not eliminate statistical discipline. They can expand the hypothesis space, but only disciplined pipelines can shrink it back down."

---

## Source Material to Weave In

### The classic feature engineering problem

A data scientist builds a feature called "customer health score." It's a weighted combination of recency, frequency, monetary value, support tickets, and product usage. The weights were chosen by intuition, adjusted after model performance improved, and never documented as a principled formula. Six months later, nobody knows why the feature is defined the way it is, and a new teammate accidentally reweights it during a pipeline refactor.

This is not an AI problem. It's a documentation and reproducibility problem that LLMs make worse — because now the "intuition" step can be outsourced to a model that generates confident-sounding feature names with no grounding at all.

### Predicate invention as a research methodology

This project used LLMs as predicate inventors — generators of candidate concepts for a domain model. The domain: Simpson's Paradox. The question: what vocabulary do you need to fully describe when and why aggregated data misleads?

The LLM suggested: `IsSignFlip`, `AllocationDistortion`, `DistortionType`, `SignalPurity`, `CorrectedWinner`, `AllocationDirection`, `DistortionRatio`. These are proposed predicates — candidate features with names but no standing until tested.

Each proposed predicate goes through three gates:

1. **Formal definition**: written as an explicit formula in the rulebook
2. **Algebraic test**: invariant checks verify mathematical consequences hold
3. **Corpus test**: the predicate is computed against all 238 studies; patterns are recorded in the Conclusions table under an epistemic tier

Only after all three gates does a predicate become a governing concept.

### The five-type taxonomy: a predicate invented and verified

`DistortionType` classifies every study into one of five categories:
- **A**: sign-flip, all strata agree (confounding is unanimous and manifest)
- **B**: sign-flip, strata disagree (confounding is partial)
- **C+**: no flip, but pooled result is amplified (DistortionRatio > 1)
- **C-**: no flip, but pooled result is compressed (DistortionRatio in (0,1))
- **D**: no flip, no meaningful distortion (pooled analysis trustworthy)

The formula (verbatim):
```
=IF({{IsSignFlip}},
  IF({{IsStratumUnanimous}}, "A", "B"),
  IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
         ABS({{SignedPooledGap}}) > ABS({{CorrectedGap}}) + 0.001), "C+",
  IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
         ABS({{SignedPooledGap}}) < ABS({{CorrectedGap}}) - 0.001), "C-",
  "D")))
```

Corpus distribution at N=238: A=78, B=8, C+=8, C-=7, D=137.

Invariant `inv-taxonomy-complete` verifies every row is assigned (PassCount=40, FailCount=0). No uncategorized studies. The taxonomy is complete over the domain.

### The signal purity theorem: a feature that became a theorem

`SignalPurity` was proposed as a measure of how much of the observed gap between treatments is "real" (the corrected gap) versus "noise" (the allocation distortion):

```
SignalPurity = ABS(CorrectedGap) / (ABS(CorrectedGap) + AllocationDistortion)
```

After computing this across all studies, the corpus showed: reversal studies (AllocationDirection='reversal') had SignalPurity < 0.5 in every case. That's not a statistical finding — it's an algebraic consequence. The noise exceeds the signal, by definition, whenever the allocation distortion is large enough to flip the sign.

Invariant `inv-signal-purity-sign-flip`: "AllocationDirection='reversal' → SignalPurity < 0.5." PassCount: 12, FailCount: 0.

Corpus summary: AvgSignalPurityReversal ≈ 0.35, AvgSignalPurityNonReversal ≈ 0.70. The gap is 0.35 — reversal studies are on average using only 35% real signal.

This is the difference between a theorem and a finding. The invariant is a theorem. The corpus averages are a finding. The epistemic tier matters.

### Hypothesis pre-registration: the DiscoveryHypotheses pattern

The project uses a `DiscoveryHypotheses` table to pre-register predictions before encoding new studies. Examples from Loop 62:

- **H-causal-manifest**: "All sign-flip studies with StratumCausalRole='confounder' will be manifest (not latent)" — registered before encoding 15 confounder studies. Result: PASS.
- **H-econ-zero**: "Economics studies will have 0% sign-flip rate" — registered before expanding the economics subset. Result at N=238: 0/8 economics sign-flips. PASS.
- **H-domain-dist**: "Epidemiology will have higher AllocationDistortion than education" — registered before encoding either domain fully. Result: epi avg 0.050, education avg 0.004. PASS.

Pre-registration prevents the most common form of confirmation bias in exploratory data science: noticing a pattern and then claiming you predicted it. The hypothesis is in the table before the data is encoded. The verdict is computed by the build.

### Superseded findings: the epistemically honest archive

At N=64, the corpus showed what appeared to be a near-universal dominance of Type D studies. At N=238, that pattern broke. The project's `Conclusions` table has an explicit category: `corpus-pattern-superseded`. Examples:

- **conc-17**: Archived finding about Type-D dominance — held at N=64, broken at N=238
- **conc-22**: Domain flip-rate characterization that was revised when economics studies were added

These are not deleted. They're tagged, dated, and traceable to the loop number where the revision happened. A data scientist reading the corpus history can see exactly what was believed, when it changed, and why.

### The CorrectedGap invariance: an allocation-free feature

`CorrectedGap = WeightedStratumGapSum = Σ(StratumFraction × StratumGap)` is the allocation-invariant estimate of the true treatment difference. Loop 34 witnessed:

"SweepCorrectedGap = 0.05367 across all 10 allocation rows (range = 0.0000). SweepPooledGap ranges from +0.130 to −0.084 (range ≈ 0.214), crossing zero near f_L=0.50 — the paradox appears and disappears as a pure artifact of allocation."

The invariant `inv-corrected-gap-invariant` (PassCount: 238, FailCount: 0) confirms this algebraic property holds across the entire corpus. CorrectedGap is not a better estimate — it is the allocation-invariant estimate, by construction.

---

## Structural Outline for the Article

1. **Hook (2 paragraphs):** The feature engineering problem. Hidden judgment, no documentation, accumulating debt.
2. **LLMs as predicate proposers (2 paragraphs):** What the model can do (name things, propose formulas) vs. what it cannot do (verify they hold).
3. **The three gates (2 paragraphs):** Formal definition, algebraic test, corpus test. Each gate eliminates candidate features.
4. **The five-type taxonomy (3 paragraphs):** Show the DistortionType formula. Show the corpus distribution. Show the invariant.
5. **Signal purity: from feature to theorem (3 paragraphs):** The formula. The corpus pattern. The distinction between theorem and finding.
6. **Pre-registration (2 paragraphs):** DiscoveryHypotheses table. Three examples from Loop 62. Why this prevents confirmation bias.
7. **Superseded findings (2 paragraphs):** The epistemic honesty of archiving. What the system knew at N=64 vs. N=238.
8. **CorrectedGap invariance (2 paragraphs):** The sweep result. The invariant. What allocation-invariance means.
9. **Close (1 paragraph):** "LLMs do not eliminate statistical discipline. They can expand the hypothesis space, but only disciplined pipelines can shrink it back down."

**Target length:** 1500–2000 words
**Suggested Medium tags:** Data Science, Feature Engineering, Simpson's Paradox, Causal Inference, Reproducibility
