# Trust the Artifact, Not the Autocomplete
## Article 6: A Compliance Guide to Machine-Generated Claims

**Series position:** 6 of 10
**Audience:** Lawyers, regulators, auditors, compliance officers, AI governance framework authors
**Central metaphor:** Legal admissibility. The LLM can suggest evidence, but cannot admit it.
**Tone:** Precise, risk-aware, emphasizes provenance and separation of claim types

---

*This is part 6 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# Due Process for Machine-Generated Knowledge

When an AI system produces a conclusion — "this patient should receive Treatment A," "this loan applicant is high-risk," "this study shows Treatment B is more effective" — the conclusion arrives in the format of a finding. It is stated with the grammar of a fact. It may be supported by reference to training data, statistical patterns, or retrieved documents. But in most architectures, the derivation is opaque: there is no mechanism to audit how the conclusion was reached, no record of what evidence was weighed, and no classification of what kind of claim is being made.

This is the admissibility problem. A courtroom does not merely evaluate whether a statement is true. It asks: what kind of statement is this, what is its basis, and how was it produced? A witness who says "I saw the defendant" is making one kind of claim. An expert who says "in my professional opinion" is making another. A document that says "this contract requires" is making a third. AI systems typically produce all three without distinguishing them.

---

## Different claims require different standards

Legal practice has evolved careful distinctions between evidentiary standards because the cost of conflating them is high. Eyewitness testimony is the most powerful and the most fallible — juries overweight it, and it fails in ways that are hard to detect. Expert opinion is admissible only when the expert is qualified and the methodology is sound. Documentary evidence carries the evidentiary weight of the document itself, not of whoever introduces it.

AI governance frameworks that respond to AI outputs with blanket restrictions — "do not rely on AI for decisions affecting individuals" — are not wrong. They are insufficient. The deeper requirement is differentiation: the ability to classify the legal and epistemic status of every output, so that downstream decision-makers know what kind of claim they are acting on, what its basis is, and how confident they should be.

The architecture described in this article provides exactly that differentiation, built into the structure of the knowledge model rather than added as a policy gloss after the fact.

---

## Three epistemic tiers: different standards of proof

The Simpson's Paradox knowledge model explicitly classifies every conclusion into one of three tiers, each with a different standard of proof.

**Tier 1 — Theorem**: an algebraic consequence of the definitions. True by construction. No additional data can falsify it; it can only be revised by changing a definition, which would trigger a re-derivation of all downstream conclusions.

Example: "CorrectedGap is allocation-invariant: it equals WeightedStratumGapSum regardless of how patients are distributed between treatment arms within a stratum." This is proven by the formula alone. Invariant `inv-corrected-gap-invariant` confirms it numerically (PassCount: 238, FailCount: 0), but the invariant is a sanity check — the theorem is established by algebra. No corpus, however large, can make it false if the definition is correct.

**Tier 2 — Discovery finding**: a hypothesis pre-registered before corpus expansion, then confirmed or disconfirmed by machine computation. The prediction was in the record before the evidence was collected.

Example: "H-causal-manifest: All sign-flip studies with StratumCausalRole='confounder' will be manifest, not latent." Pre-registered in Loop 62 before encoding 15 confounder studies. The build ran. Verdict: PASS. This is not a theorem — more data from different domains could revise it. But it is more than an observation: it was committed before the evidence was collected, which eliminates post-hoc pattern-fitting as an explanation.

**Tier 3 — Corpus pattern**: an observation at a specific corpus size, tagged as provisional. Examples include conc-17 and conc-22 — findings that held at N=64 and were superseded when the corpus grew to N=238. These remain in the record, tagged `corpus-pattern-superseded`, with the loop number where the revision occurred. The organization knows what it used to believe and when it changed its mind.

The critical governance property: these three tiers are not interchangeable. A conclusion classified as Tier 1 can be cited with the same confidence as a mathematical proof. A conclusion classified as Tier 3-superseded should not be cited at all without disclosing that it has been revised.

---

## Chain of custody: the loop table

A chain of custody is a documented record of how evidence was collected, transferred, and handled — establishing that the evidence presented is the same evidence that was originally obtained. It is admissible because it is auditable.

The loop table is the chain of custody for every conclusion in this knowledge model. Each of the 78 rows records:

- The concept introduced in that build iteration
- The `CommitHash`: the git commit where it first appears — a cryptographic anchor to a reproducible build state
- The `CommitShort`: the first 7 characters for display and citation
- The `WitnessNote`: what the build produced when it ran — machine-generated, not analyst-written
- The `NextSuggestion`: the open question that motivated the next iteration

An auditor can check out any `CommitHash` from the loop table, run the build, and reproduce the exact state of the knowledge model at that point in its development. Loop 34's commit short anchors to the build where the AllocationSweep was first introduced and witnessed. The witness note is machine-produced: "SweepCorrectedGap = 0.05367 across all 10 allocation rows (range = 0.0000)." It was not written by a person claiming to remember what happened. It was generated by the same build that produced the result.

This is the property that makes the chain of custody meaningful: each link in the chain is a deterministic output of a versioned process, not a retrospective account.

---

## Compliance controls: the invariant table

The invariant checks table defines machine-verifiable compliance controls — assertions that must hold across the full corpus on every build. Each is a stated requirement, a test procedure, and an observed result. The structure maps cleanly to what compliance officers call a control:

**inv-type-ab-sign-flip** (Severity: critical): "DistortionType ∈ {A,B} → IsSignFlip=TRUE." Any study classified Type A or B must exhibit a sign-flip. If any such study has IsSignFlip=FALSE, the classification system has internally contradicted itself. PassCount: 12, FailCount: 0.

**inv-explained-sign-flip-confounder** (Severity: critical): "IsParadoxExplained=TRUE → IsSignFlip=TRUE AND CorrectedVsPooledAgreement=FALSE." A study can only be marked as explained if it is a genuine sign-flip where the corrected analysis disagrees with the pooled analysis. PassCount: 15, FailCount: 0.

**inv-signal-purity-sign-flip** (Severity: critical): "AllocationDirection='reversal' → SignalPurity < 0.5." Reversal studies must have SignalPurity below 0.5 — the noise is doing more than half the work, by construction. PassCount: 12, FailCount: 0.

Every critical invariant has FailCount = 0 at N=238. A `FailCount > 0` on any critical invariant is build-breaking: the corpus cannot be published or distributed in that state. The control is not monitored after the fact — it is enforced during production. The equivalent in financial systems would be a transaction that cannot close until reconciliation passes.

---

## Due process at acquisition: the IngestionProtocol

Evidence is only as reliable as the process that collected it. The IngestionProtocol defines the conditions a study must satisfy before its data enters the corpus. This is due process at the acquisition layer.

Required inputs: StudyId, a StratumId per CaseCell, TreatmentLabel, Cases, Successes. Structural minimums: binary treatment comparison, at least two strata, paired A/B cells, at least 10 cases per cell. Inclusion criteria: outcome direction pre-registered, stratum variable named and documented before classification, no post-hoc stratum splitting. Exclusion criteria: missing stratification variable, observational studies without a causal claim.

A study that fails any condition is not silently dropped — it is recorded in the `CandidateStudyCatalog` table with `ingestion_status = 'excluded'` and a documented reason. The system knows why a study is absent. An auditor can inspect the exclusion table and verify that the exclusion criteria were applied consistently. This is the compliance equivalent of documented exception handling: the absence of a record is itself a record.

---

## The provenance chain

Every conclusion in the `Conclusions` table carries structured provenance metadata:

- `Category`: theorem | discovery-finding | corpus-pattern | corpus-pattern-superseded
- `WitnessedInLoop`: foreign key to the Loops table row where this conclusion was first established
- `SupportingInvariantId`: foreign key to the InvariantChecks row that numerically confirms it (for theorems)
- For superseded findings: the loop number of the revision and the description of what replaced it

The full provenance chain for any conclusion is therefore: the conclusion → the loop where it was witnessed → the git commit hash → the reproducible build state → the invariant check or hypothesis test that validated it. Each link is machine-generated and auditable. The chain terminates at a versioned artifact that can be replayed.

This is the structure that AI governance frameworks currently do not require and most AI systems do not provide. Most AI outputs have no provenance chain at all: a model produced a response, the response entered a document or decision record, and there is no mechanism to trace the response back to its derivation.

---

## What AI governance frameworks are missing

Most current AI governance frameworks address three concerns: output restrictions (do not produce X), bias audits (are outputs equitable across groups), and human-in-the-loop requirements (a person must review before action). These are necessary responses to real risks.

They miss three things that the architecture here addresses directly.

First: **epistemic classification**. Not all AI outputs have the same standing. A conclusion that follows algebraically from a definition is categorically different from a corpus observation that held at a small sample size. Governance frameworks that treat all outputs identically are governing the wrong variable.

Second: **derivation provenance**. The ability to trace any claim to the formula, data, and build that produced it. Without this, "auditing AI" means auditing the decision that was made using AI output — not auditing the AI output itself. These are different audit targets with different implications.

Third: **supersession tracking**. When the knowledge model changes its conclusions — because new data arrived, because the corpus expanded, because an error was corrected — the governance record should reflect what was believed before and what changed it. A model that silently updates its outputs has no audit trail for the revision. The architecture here tracks every revision as a first-class event in the loop table, with the loop number, the previous finding, and the evidence that displaced it.

AI governance should not merely restrict outputs. It should classify the legal and epistemic status of every output, trace its derivation, and record what the system used to believe. That is the standard the law applies to human testimony and documentary evidence. It is the standard machine-generated knowledge should meet.

---

**Also in this series:**
- *Part 2: A Boardroom Guide to Governed AI Knowledge* — the three governance risks and the ROI of governed knowledge assets
- *Part 5: A Knowledge Governance Guide to Institutional Memory* — vocabulary lifecycle, provenance, and the three-tier epistemic system

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*
