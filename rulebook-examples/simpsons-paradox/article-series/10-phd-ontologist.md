# Trust the Artifact, Not the Autocomplete
## Article 10: An Ontologist's Guide to Executable Vocabulary Evolution

**Series position:** 10 of 10
**Audience:** PhD ontologists, knowledge representation researchers, semantic web veterans, DL reasoner practitioners
**Central metaphor:** Ontology as a laboratory instrument — not a static schema, but an evolving, executable theory.
**Tone:** Technical, precise, engages with KR literature, treats the reader as a peer

---

## Original Outline

**Title:** *Predicate Invention Beyond Constraint Satisfaction: Executable Ontology Evolution in an LLM-Assisted Loop*

**Central metaphor:** Ontology as a laboratory instrument, not a static schema.

**Focus:** Difference from classical constraint ontologies, derived predicates, epistemic typing, executable semantics, abductive proposal plus deterministic adjudication.

**Translate terms this way:**

| Technical term      | Ontologist version                                   |
| ------------------- | ---------------------------------------------------- |
| Rulebook            | Executable intensional theory                        |
| Predicate invention | Vocabulary extension under operational semantics     |
| Loop                | Epistemic revision event                             |
| Invariant           | Model-theoretic or algebraic preservation constraint |
| Corpus finding      | Empirical interpretation over encoded instances      |

**Article angle:**
This version should explicitly distinguish the architecture from traditional constraint-based ontologies. A constraint ontology validates instance conformance against a vocabulary. This system also records the evolution of the vocabulary itself: why a predicate was introduced, what it computes, what invariants protect it, what conclusions it supports, and whether later corpus evidence demotes or strengthens it.

**Key lesson:**
"The interesting object is not merely the ontology. It is the controlled evolution of the ontology under executable tests."

---

## Source Material to Weave In

### Position relative to classical KR

Classical OWL ontologies (OWL-DL, OWL-RL) operate as constraint-satisfaction systems: a vocabulary of classes and properties is declared; instances are asserted; a reasoner derives consequences under the closed or open world assumption. The vocabulary itself is treated as given — stable between revision cycles, typically authored by domain experts, and not subject to systematic empirical testing.

Two limitations follow: (1) the vocabulary evolution is either undocumented (ad hoc editing of the OWL file) or externalized to a separate process (e.g., ontology learning, ontology alignment) that is decoupled from the reasoning machinery; and (2) derived predicates are either not representable (OWL-DL expressivity constraints) or representable only as SWRL rules or SPARQL-CONSTRUCT patterns with no integrated test harness.

This project takes a different stance: the vocabulary IS the thing under evolution, the formulas ARE the operational semantics, and the test harness IS part of the model.

### The architecture: intensional theory as executable SSoT

The rulebook is an intensional theory over domain individuals. Every field definition is either:
- **Raw**: an extensional assertion (StratumCasesA, StratumCasesB are counted from CaseCells)
- **Calculated**: an intensional definition via an Excel-style formula (IsSignFlip, DistortionType)
- **Lookup**: an intensional JOIN (TraditionName = LOOKUP(TraditionId, ResearchTraditions))
- **Aggregation**: an intensional GROUP-BY (StrataWonByA = COUNTIFS across CaseCells)

The build transpiles these definitions to multiple substrates: Postgres SQL (views), OWL-SHACL (SPARQL-CONSTRUCT rules), Python (class methods), Excel. Two of these are used for conformance testing: the Postgres view and the OWL-SHACL derivation. The conformance test asserts the leaf aggregates into the OWL graph, runs the SHACL fixpoint, and diffs the derived fields against the Postgres view.

This is cross-substrate model-theoretic consistency: the formula's intended semantics are realized independently in two computational substrates, and the realizability is tested on every build.

### Predicate invention as abductive proposal under adjudication

Classical predicate invention in ILP (Inductive Logic Programming) operates by generating candidate predicates that improve a target hypothesis's coverage. The adjudication criterion is predictive accuracy or compression.

This project's predicate invention operates differently:

1. **Abduction** (LLM-mediated): the model is prompted with the domain structure and proposes candidate predicates — names, informal definitions, hypothesized algebraic relationships. Example: "Call the difference between pooled gap and corrected gap `AllocationDistortion`. Hypothesis: when this exceeds 0.01 with a sign change, you have a Type A or B study."

2. **Formalization**: the candidate is given an operational definition — an Excel-style formula over the existing DAG nodes. The formula is written into the rulebook.

3. **Adjudication** (deterministic): invariant checks verify algebraic consequences. If the proposed predicate entails `AllocationDirection='reversal' → SignalPurity < 0.5`, that entailment is stated as an invariant and checked across all 238 instances.

4. **Classification**: the predicate is assigned to a `Conclusions` row with an epistemic tier:
   - **theorem**: the entailment follows from the formula definitions alone
   - **discovery-finding**: the entailment was pre-registered and confirmed empirically
   - **corpus-pattern**: observed without pre-registration
   - **corpus-pattern-superseded**: observed at N=64, broken at N=238

The adjudication criterion is not predictive accuracy (there is no prediction task) — it is algebraic closure under the operational semantics, plus empirical support across the corpus.

### The DAG as a dependency-typed derivation graph

The rulebook's calculated and aggregation fields form a directed acyclic graph where edges represent formula dependencies. The key theorems concern invariants of this graph:

**Theorem (CorrectedGap allocation-invariance)**: WeightedStratumGapSum = Σ(StratumFraction_k × StratumGap_k). The allocation variable (TreatmentExposureFraction) does not appear in this formula. Therefore CorrectedGap is algebraically independent of allocation. Witnessed numerically: SweepCorrectedGapRange = 0.0000 across 10 parametric sweep points per study, 238 studies, invariant `inv-corrected-gap-invariant` PassCount: 238, FailCount: 0.

**Theorem (SignalPurity < 0.5 for reversals)**: SignalPurity = ABS(CorrectedGap) / (ABS(CorrectedGap) + AllocationDistortion). When AllocationDirection='reversal', by definition AllocationDistortion > ABS(CorrectedGap) (the allocation distortion is large enough to flip the sign). Therefore ABS(CorrectedGap) < AllocationDistortion, which implies SignalPurity < 0.5. This is an algebraic consequence of the DistortionType taxonomy definition, not a corpus finding. Invariant `inv-signal-purity-sign-flip` PassCount: 12, FailCount: 0.

**Theorem (IsParadoxExplained biconditional)**: IsParadoxExplained=TRUE ↔ IsSignFlip=TRUE ∧ StratumCausalRole='confounder'. This is definitional — IsParadoxExplained is defined by this biconditional. The invariant confirms the definition is encoded correctly across all instances.

**Corpus finding (confounder-manifest conditional)**: IsSignFlip=TRUE ∧ StratumCausalRole='confounder' → AllocationDirection='reversal' (manifest, not latent). Pre-registered as H-causal-manifest. Confirmed at N=238. This is NOT a theorem — it is an empirical generalization over the corpus. Additional instances from different domains could falsify it.

### The epistemic type system

The Conclusions table implements a typed epistemic record:

```
theorem            — algebraic consequence of formula definitions
discovery-finding  — pre-registered hypothesis, empirically confirmed
corpus-pattern     — post-hoc observation over the current corpus
corpus-pattern-superseded — finding archived due to corpus expansion
```

This is a partial ordering of epistemic warrant: theorem > discovery-finding > corpus-pattern. The superseded category records the evolution of the corpus-pattern level, not the theorem level (theorems are revision-protected by construction — they follow from the definitions regardless of new instances).

Most ontologies lack this type system. Every class and property assertion in an OWL file has the same epistemic status. The result: it is impossible to distinguish what the ontology knows by construction, what it has confirmed empirically, and what it used to believe before the most recent revision.

### Cross-substrate conformance as model-theoretic consistency testing

The OWL-SHACL conformance test (`owl/reason.py`) provides a form of model-theoretic consistency check that is unusual in ontology engineering practice:

- The leaf aggregates are asserted into the OWL graph as data property assertions
- SHACL SPARQL-CONSTRUCT rules derived from the same rulebook formula strings are applied iteratively (fixpoint)
- The derived values are compared against the Postgres view, which is independently derived from the same formulas

Fields in the conformance set: IsReversal, DistortionType, CorrectedWinner, AdjustmentAppropriate, PolicyImplication, SignalPurity.

The conformance test is NOT a substitute for formal model-theoretic proof. It is an empirical witness: two independent computational realizations of the same intensional theory agree on the test corpus. The failure mode it catches is transpiler divergence — the formula was correctly specified in the rulebook but incorrectly compiled to one of the two substrates.

This is analogous to checking that two theorem provers agree on the validity of a sentence: neither constitutes a proof, but disagreement is a bug signal that warrants investigation.

### What distinguishes this from DOLCE, BFO, and foundational ontologies

Foundational ontologies (DOLCE, BFO, GFO, etc.) provide upper-level vocabulary with formal axioms and principled parthood/dependence relations. They do not typically address:
- The lifecycle of new predicates added post-publication
- The empirical validation of derived predicates against instances
- The epistemic status of axioms relative to the corpus that motivated them
- The cross-substrate realizability of formula semantics

This project is not a foundational ontology. It is a domain-specific executable theory that incorporates the meta-epistemic machinery that foundational ontologies assume is handled outside the formalism. The Loops table is the formalism's theory of its own evolution.

---

## Structural Outline for the Article

1. **Position statement (2 paragraphs):** Classical OWL as constraint-satisfaction. The two gaps: undocumented vocabulary evolution and no integrated test harness.
2. **The architecture (3 paragraphs):** Intensional theory as executable SSoT. Field type taxonomy (raw, calculated, lookup, aggregation). Multi-substrate transpilation.
3. **Predicate invention as abductive adjudication (3 paragraphs):** Compare to ILP predicate invention. The four steps: abduction, formalization, adjudication, classification. The distinction between algebraic and empirical adjudication criteria.
4. **The DAG and its theorems (4 paragraphs):** CorrectedGap allocation-invariance. SignalPurity < 0.5 for reversals. IsParadoxExplained biconditional. Confounder-manifest as corpus finding (NOT theorem). Quote the invariant PassCounts.
5. **The epistemic type system (3 paragraphs):** The four-tier type system. Partial ordering of warrant. Contrast with flat OWL assertion epistemics.
6. **Cross-substrate conformance (3 paragraphs):** The OWL-SHACL test as empirical model-theoretic consistency check. Fixpoint SHACL derivation. Conformance set fields. Failure mode (transpiler divergence).
7. **Position relative to foundational ontologies (2 paragraphs):** What DOLCE/BFO/GFO don't address. The Loops table as a formalism's theory of its own evolution.
8. **Close (1 paragraph):** "The interesting object is not merely the ontology. It is the controlled evolution of the ontology under executable tests."

**Target length:** 2000–2500 words
**Suggested Medium tags:** Knowledge Representation, Ontology Engineering, Semantic Web, OWL, Description Logics, AI
**Note:** This article can cite Muggleton (ILP), Guarino (formal ontology), the OWL 2 specification, and SHACL documentation. It is the only article in the series that should engage with primary literature.
