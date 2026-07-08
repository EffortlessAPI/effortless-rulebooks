# Trust the Artifact, Not the Autocomplete
## Article 5: A Knowledge Governance Guide to Institutional Memory

**Series position:** 5 of 10
**Audience:** CDOs, data stewards, knowledge managers, enterprise architects, semantic layer owners
**Central metaphor:** Institutional memory with version control.
**Tone:** Practitioner-level, speaks to vocabulary drift, concept lifecycle, provenance

---

*This is part 5 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# The Organization That Can Remember Why It Believes Things

Knowledge governance is not just storing definitions. It is governing how definitions are born, tested, revised, and reused.

That distinction sounds abstract until you watch a concept fail in the field. The failure mode is almost always the same: a term that everyone uses and nobody defined the same way. The governance gap is not in the repository — it is in the lifecycle. Organizations know how to store things. They rarely know how to govern the conditions under which a stored thing can be trusted.

---

## The concept drift problem every organization has

An organization defines "at-risk customer" in 2019. By 2023, three teams use the term differently. The customer success team means any customer who opened a support ticket in the last 30 days. The sales team means any customer with renewal probability below 60%. The analytics team means any customer whose usage dropped more than 20% quarter-over-quarter. Each team can defend its definition. No team is using the same definition. Every cross-functional report that includes "at-risk customer" is quietly wrong.

This is not an AI problem. But AI makes it dramatically worse. When a language model proposes a concept name, the prose sounds authoritative. It gets copy-pasted into documentation, presentations, and dashboards before anyone has asked what it actually computes. The definition exists — in a dozen places, in slightly different forms, none of them governed.

The symptom that surfaces years later is familiar to every data steward: a critical metric that no two systems compute the same way, an audit that cannot find a canonical formula, a new team member who spends three weeks figuring out which definition to use.

---

## The vocabulary lifecycle: birth, test, revision, archive

A governed concept has a lifecycle with four phases, each with explicit conditions that must be satisfied before the concept advances.

**Birth**: the concept is proposed with a name, a formula, and the domain question it answers. It goes into the governed artifact — the rulebook — as a typed field definition with a precise formula. Not a description. A formula. If it cannot be written as a computable expression, it is not a governed concept — it is a label. Labels drift.

**Test**: invariant checks verify that algebraic consequences of the definition hold across the full dataset. A concept that cannot be validated algebraically has not earned its place in the model. The concept is not considered valid until all critical invariants pass.

**Revision**: when a corpus expansion breaks a prior finding, the Conclusions table records the revision. The old finding is tagged `corpus-pattern-superseded` with a link to the build iteration where it changed. Nothing is deleted. The organization can see what it used to believe.

**Reuse**: once a concept is in the rulebook, it becomes available for downstream formulas to reference. `SignalPurity` depends on `CorrectedGap` and `AllocationDistortion`. `PolicyImplication` depends on `DistortionType`. The dependency graph is explicit and machine-verifiable. A change to a foundational concept propagates automatically to every formula that references it — and the invariant suite confirms the propagation is consistent.

---

## The governed vocabulary of Simpson's Paradox

In this domain, every concept in the vocabulary passed through this lifecycle. The result is a layered concept hierarchy:

**Core domain objects** — Study, Treatment, Stratum, CaseCell. These are the raw inputs: the published study metadata, the treatment arms being compared, the subgroups within each study, and the raw counts of successes and cases.

**First-order derived concepts** — StratumSuccessRateA, StratumSuccessRateB, StratumGap. These are computed from the case counts and form the foundation of every higher-level concept.

**Allocation concepts** — TreatmentExposureFraction, StratumFraction, WeightedStratumGapSum. This is the mechanism vocabulary: the concepts needed to describe *how* allocation distorts pooled results.

**Classification concepts** — IsSignFlip, IsStratumUnanimous, DistortionType (A/B/C+/C−/D). The taxonomy that categorizes what kind of distortion each study exhibits.

**Epistemic concepts** — SignalPurity, AllocationDistortion, DistortionRatio. The measurement vocabulary that quantifies how much noise is present and how confident the pooled result should be trusted.

**Policy concepts** — PolicyImplication, CorrectedWinner, AdjustmentAppropriate. The actionable outputs that tell a practitioner what to do with a given study.

Every concept in this hierarchy is defined once, in one place, with a formula, a description, and a dependency path. There are no team-specific variants. No parallel definitions that diverged when the analytics team moved to a new tool. One concept, one formula, one test. That is what "governed" means at the semantic layer.

---

## The loop history as institutional memory

The loop table — 78 rows at last count, and growing — is the complete intellectual genealogy of how this vocabulary was built. Each row records four things: the question the organization could not ask before this loop, the concept introduced to let them ask it, what was actually witnessed when the build ran, and the git commit that anchors the record to a reproducible state.

Three excerpts:

**Loop 6**: "A: 263/350=75% in large-stone stratum. B: 80/350=23% in large-stone stratum. The 52-point gap IS the mechanism — now a first-class derived number (TreatmentExposureFraction)." This is the loop where allocation became a first-class concept rather than background context. A new team member reading this row understands not just that `TreatmentExposureFraction` exists, but *why it had to exist* — because the 52-point allocation gap in the kidney stone study is the entire story.

**Loop 17**: "kidney-1986: WSGSUM=+0.0537, SignedPooledGap=−0.0457 → IsSignFlip=TRUE. berkeley-1973: IsSignFlip=TRUE. Per-stratum breakdown shows type-B partial reversal." This is when the core phenomenon was formalized. The loop record is both a design note and a test fixture: here are the two studies that were used to verify the definition, with exact numbers.

**Loop 44**: "All type-A/B studies (AllocationDirection=reversal) have SignalPurity < 0.5 — the confound is doing more than half the work. AvgSignalPurityReversal ≈ 0.35, AvgSignalPurityNonReversal ≈ 0.70." This is when SignalPurity became more than a proposed metric — it became an invariant. The average reversal study has only 35% real signal.

A new team member reading the loop history from row 1 to row 78 acquires the full context: every concept, when it was introduced, what evidence justified it, and what question it was designed to answer. This is institutional memory in a form that survives personnel turnover. The knowledge is in the table, not in the heads of the people who were in the room.

---

## The three-tier epistemic system

Not all knowledge has the same status, and a governed semantic layer should know the difference. The Conclusions table uses three epistemic tiers:

**Theorem**: an algebraic consequence of the definitions. True by construction. Cannot be falsified by more data — only by changing a definition. Example: "CorrectedGap is allocation-invariant." This follows from the formula: CorrectedGap is defined as the weighted average of per-stratum gaps, with weights that sum to one regardless of how cases are distributed. The invariance is built into the definition. No additional data can make it false.

**Discovery-finding**: a pattern found in the corpus after testing a pre-registered hypothesis. Example: confounder-labeled studies tend to produce manifest sign-flips while collider/selection studies tend toward latent-only geometry — confirmed at N=238 for the current corpus. Contingent on this collection; more studies could change the rates.

**Corpus-pattern-superseded**: a finding that was valid at a smaller sample size and required revision when the corpus expanded. Examples include economics "zero sign-flip rate" (held at N≈8, broke at N=238 with 10 flips) and domain distortion comparisons that changed after expansion wave 3. Retired from the active DAG in loop-79; evidence preserved in `conc-32`.

Most governance systems have no analog to these tiers. Every claim lives in the same undifferentiated repository: things we believe. When someone asks "how confident are we in this?", there is no structural answer. The three-tier system provides a structural answer: theorems are as confident as the definitions themselves; discovery findings are confident contingent on this corpus; superseded findings represent beliefs that have been revised and should not be acted on without awareness of the revision.

Decision-making quality depends on knowing which tier a given piece of knowledge belongs to.

---

## The knowledge dashboard

The `ModelSummary` table provides a single-row self-portrait of the corpus. At N=238:

- StudyCount: 238
- ReversalCount: 86 (36%)
- TypeACount: 78, TypeBCount: 8, TypeCPlusCount: 8, TypeCMinusCount: 7, TypeDCount: 137
- WeightedAvgAllocationDistortion: 0.0456
- AvgSignalPurityReversal: 0.35 vs. AvgSignalPurityNonReversal: 0.70
- TheoremCount: 5

This is a knowledge dashboard. It summarizes the state of what the governed semantic layer knows, at the current build, across the full corpus. It is not a report that someone wrote. It is computed deterministically from the same rulebook and corpus that drive everything else. Every time a new study is added to the corpus, the dashboard updates automatically. Every time a new theorem is promoted, the count increments. The dashboard cannot be out of date — it is generated by the same build that generates the database, the documentation, and the invariant check results.

This is what "governed" looks like at the knowledge layer: a dashboard that the organization did not write and cannot manipulate, because it is a derived fact from the source of truth.

---

## The IngestionProtocol as semantic governance at the acquisition layer

A governed semantic layer is only as good as what it admits. The project's 17-item ingestion protocol defines the conditions a study must meet before its data enters the corpus.

Required inputs: StudyId, a StratumId per CaseCell, TreatmentLabel, Cases, Successes. Structural constraints: binary treatment, at least two strata, paired A/B cells, at least 10 cases per cell. Inclusion criteria: outcome direction pre-registered, stratum variable named before classification, no post-hoc splitting. Exclusion criteria: missing stratification variable, observational studies without a causal claim.

A study that fails any of these conditions is excluded. The exclusion is documented. The system knows why a study is absent — it is not just absent.

This is the acquisition layer of semantic governance: the conditions under which raw data is admitted to the governed layer. Most organizations have no such layer. Data arrives, gets processed, and the downstream model reflects whatever came in. When a finding looks strange, there is no systematic way to ask whether the underlying data met the conditions the analysis assumes.

The IngestionProtocol is not a filter applied to existing data. It is a contract that defines what the concept of "a study in this corpus" means. Violating the contract would change the meaning of every concept that depends on the corpus — and the invariant suite would catch it.

---

## What the organization gains

An organization that governs its vocabulary through this lifecycle gains something rare: the ability to know why it believes things.

Not just what it believes — every organization has claims it acts on. But why: the formula that defines the concept, the test that verified the algebraic consequence, the loop number where the concept was introduced and the witness note that justified it, the tier that distinguishes a theorem from a contingent finding, the archive that records what was believed before and when it changed.

Knowledge governance is not just storing definitions. It is governing how definitions are born, tested, revised, and reused. Organizations that do this well do not just have more reliable analytics. They have a foundation for durable institutional learning — the kind that survives the departure of the person who built the original model.

---

**Also in this series:**
- *Part 2: A Boardroom Guide to Governed AI Knowledge* — the ROI of governed knowledge assets and the three governance risks
- *Part 6: A Legal and Compliance Guide to Auditable AI* — the provenance, traceability, and documentation requirements for regulated AI outputs

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*
