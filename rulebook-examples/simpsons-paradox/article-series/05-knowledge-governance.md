# Trust the Artifact, Not the Autocomplete
## Article 5: A Knowledge Governance Guide to Institutional Memory

**Series position:** 5 of 10
**Audience:** CDOs, data stewards, knowledge managers, enterprise architects, semantic layer owners
**Central metaphor:** Institutional memory with version control.
**Tone:** Practitioner-level, speaks to vocabulary drift, concept lifecycle, provenance

---

## Original Outline

**Title:** *The Organization That Can Remember Why It Believes Things*

**Central metaphor:** Institutional memory with version control.

**Focus:** Shared vocabulary, provenance, SSoT, lifecycle of concepts, semantic governance.

**Translate terms this way:**

| Technical term      | Governance version      |
| ------------------- | ----------------------- |
| Predicate invention | Vocabulary evolution    |
| Rulebook            | Governed semantic layer |
| Loop history        | Institutional memory    |
| Invariant           | Semantic control        |
| Corpus summary      | Knowledge dashboard     |

**Article angle:**
Organizations constantly invent new concepts: churn risk, qualified lead, strategic account, safety incident, customer health, policy exception. Usually those definitions drift. This architecture treats new concepts as governed objects with definitions, dependencies, tests, and history.

**Key lesson:**
"Knowledge governance is not just storing definitions. It is governing how definitions are born, tested, revised, and reused."

---

## Source Material to Weave In

### The concept drift problem every organization has

An organization defines "at-risk customer" in 2019. By 2023, three teams are using the term differently: the CS team means "any customer who opened a support ticket in the last 30 days," the sales team means "any customer with renewal probability below 60%," and the analytics team means "any customer whose usage dropped more than 20% quarter-over-quarter." The definition exists in every system — but as three incompatible things.

This is not an AI problem. But AI makes it dramatically worse: when an LLM proposes a concept name, the prose sounds authoritative and gets copy-pasted into docs, decks, and dashboards before anyone has asked what it actually computes.

### The vocabulary lifecycle: birth, test, revision, archive

This project treats every concept as having a governed lifecycle:

**Birth**: The concept is proposed with a name, a formula, and the domain question it answers. It goes into the rulebook as a typed field definition.

**Test**: Invariant checks verify algebraic consequences hold across the full corpus. The concept is not considered valid until all tests pass.

**Revision**: If a corpus expansion breaks a prior finding, the `Conclusions` table records the revision. The old finding is tagged `corpus-pattern-superseded` with a link to the loop where it changed.

**Reuse**: Once a concept is in the rulebook, it's referenced by downstream fields. `SignalPurity` depends on `CorrectedGap` and `AllocationDistortion`. `PolicyImplication` depends on `DistortionType`. The dependency graph is explicit and machine-verifiable.

### The governed vocabulary of Simpson's Paradox

In this domain, the following concepts were invented and governed through this lifecycle:

**Core domain objects**: Study, Treatment, Stratum, CaseCell (raw inputs)

**First-order derived concepts**: StratumSuccessRateA, StratumSuccessRateB, StratumGap — computed from case counts

**Allocation concepts**: TreatmentExposureFraction, StratumFraction, WeightedStratumGapSum — the mechanism vocabulary

**Classification concepts**: IsSignFlip, IsStratumUnanimous, DistortionType (A/B/C+/C-/D) — taxonomy

**Policy concepts**: PolicyImplication, CorrectedWinner, AdjustmentAppropriate — actionable outputs

**Epistemic concepts**: SignalPurity, AllocationDistortion, DistortionRatio — measurement vocabulary

Every one of these is defined once, in one place (the rulebook JSON), with a formula, a description, and a dependency path. No parallel definitions. No team-specific variants. One concept, one formula, one test.

### The loop history as institutional memory

The loop table (78 rows and growing) is the complete institutional memory of how this vocabulary was built. Each row records:

- What question the organization didn't know how to ask before this loop
- What concept was introduced to let them ask it
- What was actually witnessed when the build ran (the WitnessNote)
- The git commit hash anchoring the record to a reproducible state

Loop 6 (excerpt): "A: 263/350=75% in large-stone stratum. B: 80/350=23% in large-stone stratum. The 52-point gap IS the mechanism — now a first-class derived number (TreatmentExposureFraction)."

Loop 17 (excerpt): "kidney-1986: WSGSUM=+0.0537, SignedPooledGap=−0.0457 → IsSignFlip=TRUE. berkeley-1973: IsSignFlip=TRUE. Per-stratum breakdown shows type-B partial reversal."

Loop 44 (excerpt): "All type-A/B studies (AllocationDirection=reversal) have SignalPurity < 0.5 — the confound is doing more than half the work. AvgSignalPurityReversal ≈ 0.35, AvgSignalPurityNonReversal ≈ 0.70."

A new team member reading the loop history has the full intellectual genealogy: every concept, when it was introduced, what evidence justified it, and what question it was designed to answer.

### The three-tier epistemic system

Not all knowledge has the same status. The Conclusions table uses three tiers:

**Theorem**: algebraic consequence of the definitions. True by construction. Cannot be falsified by more data.
- Example: "CorrectedGap is allocation-invariant" — this follows from the formula, not from the corpus.

**Discovery-finding**: a pattern found in the corpus after testing a pre-registered hypothesis.
- Example: "Economics studies have 0% sign-flip rate; epidemiology has ~27%." More data could revise this.

**Corpus-pattern-superseded**: a finding that was true at a smaller N and broke when the corpus expanded.
- Examples: conc-17, conc-22 — findings from N=64 that needed revision at N=238.

This three-tier system is how the organization knows what it knows — and how confident to be in each piece of knowledge. Most knowledge governance systems have no analog. Every claim lives in the same undifferentiated soup of "things we believe." This project has explicit epistemic metadata on every conclusion.

### What the knowledge dashboard looks like

The ModelSummary table provides a single-row self-portrait of the corpus:

- StudyCount: 238
- ReversalCount: 86 (36%)
- TypeACount: 78, TypeBCount: 8, TypeCPlusCount: 8, TypeCMinusCount: 7, TypeDCount: 137
- WeightedAvgAllocationDistortion: 0.0456
- AvgSignalPurityReversal: 0.35 vs. AvgSignalPurityNonReversal: 0.70
- TheoremCount: 5

This is a knowledge dashboard. It summarizes what the governed semantic layer knows, across the full corpus, at the current build. It updates deterministically every time the corpus is extended.

### The IngestionProtocol as semantic governance

The 17-item adapter contract for encoding new studies is knowledge governance at the acquisition layer:

Required inputs: StudyId, StratumId per cell, TreatmentLabel, Cases, Successes.
Structural constraints: binary treatment, ≥2 strata, paired A/B cells, ≥10 cases per cell.
Inclusion criteria: outcome direction pre-registered, stratum variable named before classification, no post-hoc splitting.
Exclusion criteria: missing stratification variable, observational studies without causal claim.

A study that fails any of these conditions is excluded. The exclusion is documented. The system knows why a study is missing — it's not just absent.

---

## Structural Outline for the Article

1. **Hook (2 paragraphs):** The "at-risk customer" example. Three definitions, three teams, zero governance.
2. **What concept drift costs (2 paragraphs):** Decision inconsistency, onboarding failure, audit exposure.
3. **The vocabulary lifecycle (3 paragraphs):** Birth, test, revision, archive. Each phase with a concrete example from the project.
4. **The governed vocabulary (2 paragraphs):** Walk through the concept hierarchy from domain objects to policy outputs. Show that every concept has one definition.
5. **The loop history as institutional memory (3 paragraphs):** Quote three loop witness notes. Show what a new team member gets from reading them.
6. **The three-tier epistemic system (3 paragraphs):** Theorem vs. discovery-finding vs. superseded. Why the distinction matters for decision-making.
7. **The knowledge dashboard (2 paragraphs):** ModelSummary as a machine-maintained state-of-knowledge report.
8. **The IngestionProtocol as semantic governance (2 paragraphs):** Acquisition governance. Exclusion with documentation.
9. **Close (1 paragraph):** "Knowledge governance is not just storing definitions. It is governing how definitions are born, tested, revised, and reused."

**Target length:** 1400–1700 words
**Suggested Medium tags:** Knowledge Management, Data Governance, Semantic Layer, Enterprise Architecture, AI Governance
