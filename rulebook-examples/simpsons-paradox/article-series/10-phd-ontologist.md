# Trust the Artifact, Not the Autocomplete
## Article 10: An Ontologist's Guide to Executable Vocabulary Evolution

**Series position:** 10 of 10
**Audience:** PhD ontologists, knowledge representation researchers, semantic web veterans, DL reasoner practitioners
**Central metaphor:** Ontology as a laboratory instrument — not a static schema, but an evolving, executable theory.
**Tone:** Technical, precise, engages with KR literature, treats the reader as a peer

---

*This is part 10 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# Predicate Invention Beyond Constraint Satisfaction: Executable Ontology Evolution in an LLM-Assisted Loop

The interesting object in this project is not the ontology. It is the controlled evolution of the ontology under executable tests.

That framing distinguishes what follows from classical OWL engineering in a specific and important way. Classical OWL practice treats the vocabulary as the stable artifact and the reasoner as the operational tool: you declare classes and properties, assert instances, and the reasoner derives consequences. The vocabulary is assumed to be given — authored by domain experts, revised through editorial processes external to the reasoning machinery, and not itself subject to systematic empirical testing. The lifecycle of the vocabulary is, in this sense, outside the formalism.

This project inverts that assumption. The vocabulary IS the thing under evolution. The formulas ARE the operational semantics. The test harness IS part of the model. The Loops table IS the formalism's theory of its own development.

---

## Two gaps in classical constraint-based ontology

Two structural limitations follow from treating the vocabulary as given.

**Gap 1: Undocumented vocabulary evolution.** OWL files are typically maintained in version control, but the version history records *what changed*, not *why the predicate was introduced*, *what corpus evidence motivated it*, *what invariants protect it*, or *what epistemic tier it occupies*. Ontology learning and ontology alignment methods address the acquisition problem but remain decoupled from the reasoning machinery — the learned vocabulary is injected, but the adjudication process that decided to inject it is not represented in the formalism.

**Gap 2: No integrated test harness for derived predicates.** OWL-DL expressivity constraints exclude full SWRL rule coverage for many practically important derived predicates. OWL-RL and SPARQL-CONSTRUCT patterns extend coverage but are not natively integrated with instance-level invariant checking. There is no standard mechanism for stating "this derived predicate must satisfy invariant φ across all current instances" as a first-class element of the ontology, running that assertion on every build, and treating failure as a publication-blocking error.

Both gaps matter because they allow the vocabulary to drift away from the domain it is supposed to model without producing an observable signal. Predicates are introduced, their algebraic consequences are never verified, their epistemic basis is never recorded, and their revision history is never distinguished from simple editorial cleanup.

---

## The architecture: intensional theory as executable SSoT

The rulebook is an intensional theory over domain individuals. The type system for field definitions distinguishes four modes of predicate introduction:

- **Raw**: an extensional assertion grounded in observation (StratumCasesA, StratumCasesB are counts derived from CaseCells)
- **Calculated**: an intensional definition via Excel-style formula (IsSignFlip, DistortionType, SignalPurity)
- **Lookup**: an intensional JOIN (treatment metadata retrieved by FK from the Treatments table)
- **Aggregation**: an intensional GROUP-BY (StrataWonByA = COUNTIFS across CaseCells for a given treatment pair)

The build transpiles these definitions to multiple substrates: Postgres SQL (materialized as views), OWL-SHACL (SPARQL-CONSTRUCT derivation rules), Python (class methods), Excel. Two substrates participate in conformance testing. The conformance test asserts the Postgres-computed aggregation values — which require SQL GROUP BY and cannot be expressed as SHACL pure-formula rules — into the OWL graph alongside the raw individuals. SHACL fixpoint derivation then computes the purely formula-based derived fields. The results are diffed against the Postgres view.

This is cross-substrate model-theoretic consistency: the formula's intended semantics are realized in two independent computational substrates, and realizability is tested on every build. The failure mode being detected is transpiler divergence — the formula was correctly specified in the rulebook but incorrectly compiled to one substrate. This is the ontology engineering equivalent of checking that two independent theorem provers agree on a sentence's validity: neither constitutes a proof, but disagreement is a deterministic bug signal warranting investigation.

---

## Predicate invention as abductive proposal under deterministic adjudication

Classical predicate invention in ILP (Muggleton, 1991) operates by generating candidate predicates that improve the coverage or compression of a target hypothesis. The adjudication criterion is predictive accuracy over a training corpus. The process is tightly coupled to a specific learning task.

The predicate invention process in this project differs along three axes.

**Abduction is LLM-mediated rather than syntactically generated.** The LLM is prompted with the domain structure — the existing DAG, the raw field types, the current corpus distribution — and proposes candidate predicates by natural language description. The proposals include informal definitions, hypothesized algebraic relationships, and suggested naming conventions. Example: "Call the difference between pooled gap and corrected gap `AllocationDistortion`. Hypothesis: when this exceeds 0.01 with a sign change, you have a Type A or B study." The LLM is functioning as an abductive generator: it proposes candidate hypotheses consistent with the observed domain structure without committing to their validity.

**Formalization precedes adjudication.** The candidate is given an operational definition — an Excel-style formula over the existing DAG nodes — before any adjudication occurs. This formalization step is non-trivial: informal descriptions are often vague enough to be operationalized in multiple inconsistent ways. The formula makes the commitment explicit and makes the adjudication deterministic. The formula is written into the rulebook.

**Adjudication is deterministic, not statistical.** Invariant checks verify algebraic consequences. If the proposed predicate entails `AllocationDirection='reversal' → SignalPurity < 0.5`, that entailment is stated as an InvariantCheck row and checked across all 238 instances. PassCount and FailCount are populated by the build. FailCount > 0 on a critical invariant is build-breaking. The adjudication criterion is not predictive accuracy over a held-out set — there is no prediction task. It is algebraic closure under the operational semantics, plus empirical support across the full corpus. A predicate that produces algebraically incoherent results (DistortionType distributes non-exhaustively) fails regardless of how well it would perform in a classification task.

**Epistemic classification follows adjudication.** The predicate is assigned a Conclusions row with one of four epistemic tiers (described below), distinguishing what the formalism knows by construction from what it has confirmed empirically.

---

## The DAG and its theorems

The rulebook's calculated and aggregation fields form a directed acyclic graph where edges represent formula dependencies. The key structural results concern invariants of this graph under specific substitutions.

**Theorem — CorrectedGap allocation-invariance.** `WeightedStratumGapSum = Σ_k(StratumFraction_k × StratumGap_k)`. The allocation variable (TreatmentExposureFraction) does not appear in this formula. The formula's value is therefore independent of allocation by construction — reweighting the stratum fractions changes PooledGap but leaves WeightedStratumGapSum invariant. This is established algebraically, not empirically. Invariant `inv-corrected-gap-invariant` (PassCount: 238, FailCount: 0) provides numerical witness across a parametric sweep of 10 allocation scenarios per study, confirming that the formula was correctly transpiled to both substrates, but the theorem does not depend on the witness count.

**Theorem — SignalPurity < 0.5 for reversals.** `SignalPurity = ABS(CorrectedGap) / (ABS(CorrectedGap) + AllocationDistortion)`. When AllocationDirection='reversal', the definition of reversal entails `AllocationDistortion > ABS(CorrectedGap)` — the distortion is large enough to flip the sign of the pooled result, which requires it to dominate the corrected gap. Therefore `ABS(CorrectedGap) < AllocationDistortion`, which directly implies `SignalPurity < 0.5`. This is an algebraic consequence of the DistortionType taxonomy definition and the SignalPurity formula together. It is not a corpus finding; it is a derived theorem. Invariant `inv-signal-purity-sign-flip` (PassCount: 12, FailCount: 0) provides numerical witness.

**Theorem — IsParadoxExplained biconditional.** `IsParadoxExplained = TRUE ↔ IsSignFlip = TRUE ∧ StratumCausalRole = 'confounder'`. This is definitional — IsParadoxExplained is defined by this biconditional. The invariant `inv-explained-sign-flip-confounder` confirms the biconditional is encoded correctly in both substrates across all conforming instances.

**Corpus finding (not theorem) — confounder-manifest conditional.** `IsSignFlip = TRUE ∧ StratumCausalRole = 'confounder' → AllocationDirection = 'reversal'` (manifest, not latent). Pre-registered as H-causal-manifest before encoding 15 confounder studies. Confirmed at N=238. This is an empirical generalization over the current corpus, not an algebraic consequence of the definitions. Additional instances from domains not yet represented could falsify it. Its epistemic tier is discovery-finding, not theorem — a distinction the type system makes explicit and which cannot be represented in a flat OWL assertion file.

---

## The epistemic type system

The Conclusions table implements a typed epistemic record with four types:

```
theorem                      — algebraic consequence of formula definitions
discovery-finding            — pre-registered hypothesis, empirically confirmed
corpus-pattern               — post-hoc observation over the current corpus
corpus-pattern-superseded    — prior finding archived due to corpus expansion
```

This constitutes a partial ordering of epistemic warrant: `theorem > discovery-finding > corpus-pattern`. The superseded category records revision events at the corpus-pattern level, not the theorem level. Theorems are revision-protected by construction — they follow from the formula definitions regardless of what new instances are added. Corpus patterns are revision-exposed — they are contingent on the current corpus distribution and may be superseded as the corpus grows (as occurred with conc-17 and conc-22 between N=64 and N=238).

The critical property of this type system is that it makes epistemic heterogeneity a first-class element of the formalism. In a standard OWL file, every class axiom and property assertion has identical epistemic status. There is no structural representation of "this axiom is a model-theoretic consequence of the other axioms" versus "this axiom was pre-registered before the corpus was encoded" versus "this axiom held at an earlier corpus size and has since been superseded." This collapse of epistemic categories is not merely inconvenient — it prevents a reader of the ontology from knowing what the ontology knows by construction and what it happens to have observed.

The Conclusions table's type system addresses this directly, and the distinction is machine-enforced: theorems must reference a SupportingInvariantId that documents their algebraic basis; discovery-findings must reference the DiscoveryHypotheses row where they were pre-registered; superseded patterns must carry the loop number of their revision.

---

## Cross-substrate conformance as empirical model-theoretic consistency

The OWL-SHACL conformance test (`owl/reason.py`) provides a form of model-theoretic consistency check that is unusual in current ontology engineering practice.

The operational sequence: (1) assert the Postgres-computed aggregation values — StrataWonByA, WeightedStratumGapSum, SignedPooledGap — into the OWL working graph as data property assertions alongside the raw CaseCell individuals; (2) apply SPARQL-CONSTRUCT rules derived from the same rulebook formula strings iteratively until fixpoint; (3) compare the derived values against the Postgres view for the conformance set fields: IsReversal, DistortionType, CorrectedWinner, AdjustmentAppropriate, PolicyImplication, SignalPurity.

The conformance set is bounded by the expressivity boundary between the two substrates. COUNTIFS and SUMIFS aggregations require SQL GROUP BY or SPARQL aggregates — they cannot be expressed as pure SHACL rules over individual triples. The conformance test therefore asserts the Postgres-computed aggregates as inputs and tests only the fields that are purely formula-based once those inputs are provided. This is the documented strategy from `owl/reason.py`:

> *"Assert the Postgres-computed aggregation values into the working graph alongside the raw individuals, then run SHACL fixpoint derivation to let the OWL reasoner compute the pure formula fields. Compare those against Postgres. This is the same pattern as the causal-autoimmune-architecture: assert the leaves, derive the DAG, diff the two substrates."*

The failure mode being detected is transpiler divergence: the formula was correctly specified in the rulebook but incorrectly compiled to one of the two target representations. This is empirical, not formal — it witnesses that the two substrate compilers agree on the test corpus, not that they agree in general. The OWL-SHACL derivation and the Postgres SQL are both outputs of transpilers reading the same source; when they disagree, at least one transpiler is wrong, and the disagreement is a diagnostic signal rather than a proof.

---

## Position relative to foundational ontologies

Foundational ontologies — DOLCE (Gangemi et al., 2002), BFO (Arp, Smith, and Spear, 2015), GFO (Herre et al., 2006) — provide upper-level vocabulary with formal axioms, principled parthood and dependence relations, and ontologically motivated distinctions between universals and particulars. Their contribution is the principled scaffolding on which domain ontologies are erected.

What they do not address, and do not attempt to address, is the lifecycle of predicates added post-publication. A foundational ontology assumes that the vocabulary of a well-formed domain ontology was derived from principled upper-level commitments by qualified ontologists. It does not represent:

- The process by which a new predicate was introduced
- The empirical corpus that motivated the introduction
- The algebraic invariants that protect the predicate's semantics
- The epistemic tier that distinguishes its status relative to other predicates
- The revision history that records when its corpus-level support was superseded

These are not failures of foundational ontologies. They are not the problem foundational ontologies were designed to solve. They are the problem this architecture is designed to solve.

The Loops table is the formalism's representation of its own epistemic development — not as external documentation, but as governed data within the same system that stores the predicates themselves. Each loop row is a typed epistemic event: a predicate introduced, a formula committed, a witness note produced, a CommitHash anchoring the event to a reproducible build state. The Loops table is the theory of the ontology's evolution, expressed in the ontology's own representational vocabulary.

This is the property that distinguishes executable ontology evolution from both classical OWL engineering and ILP-style predicate invention: the formalism includes a representation of how it came to be, what it witnessed at each step, and what it is still uncertain about. The controlled evolution is not external to the model — it IS the model.

---

**Also in this series:**
- *Part 3: A Developer Guide to Rulebooks, Builds, and Invariants* — the build infrastructure for non-specialist readers
- *Part 4: A Data Scientist's Guide to Predicate Invention* — the feature engineering perspective on the same predicate lifecycle

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*
