# Trust the Artifact, Not the Autocomplete
## Article 6: A Compliance Guide to Machine-Generated Claims

**Series position:** 6 of 10
**Audience:** Lawyers, regulators, auditors, compliance officers, AI governance framework authors
**Central metaphor:** Legal admissibility. The LLM can suggest evidence, but cannot admit it.
**Tone:** Precise, risk-aware, emphasizes provenance and separation of claim types

---

## Original Outline

**Title:** *Due Process for Machine-Generated Knowledge*

**Central metaphor:** Legal admissibility. The LLM can suggest evidence, but cannot admit it.

**Focus:** Accountability, provenance, separation of claim types, audit trails.

**Translate terms this way:**

| Technical term | Legal/compliance version |
| -------------- | ------------------------ |
| LLM output     | Proposed testimony       |
| Rulebook       | Evidentiary record       |
| Invariant      | Compliance control       |
| Epistemic tier | Standard of proof        |
| Loop history   | Chain of custody         |

**Article angle:**
The article emphasizes the architecture's most governance-relevant feature: not all claims are treated equally. The system distinguishes theorem-like consequences, instrument-level classifications, and provisional corpus observations. That is exactly what many AI governance frameworks lack.

**Key lesson:**
"AI governance should not merely restrict outputs. It should classify the legal and epistemic status of every output."

---

## Source Material to Weave In

### The admissibility problem with AI-generated conclusions

When an AI system produces a conclusion — "this patient should receive Treatment A," "this loan applicant is high-risk," "this study shows treatment B is more effective" — the conclusion has the format of a finding but may have no verifiable derivation. Most AI governance frameworks respond to this by restricting which outputs can be acted on. That's necessary but insufficient. The deeper problem is that the architecture produces no record of *how* the conclusion was reached, and therefore no mechanism for auditing it.

A courtroom analogy: a witness can say "I saw X." A lawyer can say "X therefore implies Y." An expert can say "by scientific consensus, Y." These are different evidentiary standards. AI systems typically produce outputs with no indication of which standard applies.

### The Simpson's Paradox case: three standards in one corpus

This project explicitly classifies every conclusion into one of three epistemic tiers:

**Tier 1 — Theorem**: an algebraic consequence of the definitions. True by construction. No additional data can falsify it. Example:

"CorrectedGap is allocation-invariant: it equals WeightedStratumGapSum regardless of how patients are distributed between treatments within a stratum."

This is proven by the formula alone, not by the corpus. Invariant `inv-corrected-gap-invariant` confirms it holds numerically (PassCount: 238, FailCount: 0), but the invariant is a sanity check — the theorem is established by algebra.

**Tier 2 — Discovery finding**: a hypothesis pre-registered before corpus expansion, then confirmed or disconfirmed by machine computation. Example:

"H-causal-manifest: All sign-flip studies with StratumCausalRole='confounder' will be manifest, not latent." Pre-registered in Loop 62 before encoding 15 confounder studies. Verdict: PASS.

This is not a theorem. More data could change it. But it is more than an observation — it was pre-registered before the evidence was collected.

**Tier 3 — Corpus pattern**: an observation at a specific N, explicitly tagged as provisional. Examples: conc-17, conc-22 — findings that held at N=64 and were superseded at N=238. These remain in the record, tagged `corpus-pattern-superseded`, with the loop number of the revision.

### Chain of custody: the loop table

The loop table is the chain of custody for every conclusion in the corpus. Each row records:

- The concept introduced in that build iteration
- The git commit hash where it first appears (`CommitHash`)
- The first 7 characters for display (`CommitShort = LEFT(CommitHash, 7)`)
- What was witnessed when the build ran (`WitnessNote`)
- The next open question (`NextSuggestion`)

An auditor can check out any git SHA from the loop table and reproduce the exact state of the knowledge model at that point. Loop 34: `CommitShort` anchors to the build where `AllocationSweep` was first introduced and witnessed. The witness note is machine-produced, not analyst-written.

### Compliance controls: the invariant table

The invariant checks table defines machine-verifiable compliance controls — assertions that must hold on every build:

**inv-type-ab-sign-flip** (Severity: critical): "DistortionType ∈ {A,B} → IsSignFlip=TRUE."
- This is a compliance control on the classification system. Any study classified Type A or B must show a sign-flip. PassCount: 12, FailCount: 0.

**inv-explained-sign-flip-confounder** (Severity: critical): "IsParadoxExplained=TRUE → IsSignFlip=TRUE AND CorrectedVsPooledAgreement=FALSE."
- A study can only be marked "explained" if it is a sign-flip where the corrected analysis disagrees with the pooled analysis. PassCount: 15, FailCount: 0.

**inv-signal-purity-sign-flip** (Severity: critical): "AllocationDirection='reversal' → SignalPurity < 0.5."
- A compliance control on the measurement system. Reversal studies must have SignalPurity below 0.5. PassCount: 12, FailCount: 0.

Every critical invariant has FailCount = 0 at N=238. A `FailCount > 0` is build-breaking — the corpus cannot be published in that state.

### The IngestionProtocol as due process

The 17-item adapter contract for encoding new studies is the due process layer for acquiring new evidence. Exclusion criteria include:

- Studies with missing stratification variable (excluded, reason documented)
- Observational studies without a causal claim (excluded, reason documented)
- Cells with fewer than 10 cases (structural minimum, enforced)

A study that fails inclusion is not silently dropped — it's recorded in the `CandidateStudyCatalog` table with `ingestion_status = 'excluded'` and a reason. The exclusion is auditable.

### The provenance of every conclusion

Every conclusion in the `Conclusions` table carries:
- `Category`: theorem | discovery-finding | corpus-pattern | corpus-pattern-superseded
- `WitnessedInLoop`: FK to the Loops table row where this conclusion was first established
- `SupportingInvariantId`: FK to the InvariantChecks row that numerically confirms it (for theorems)
- For superseded findings: the loop number of the revision and what replaced it

This means every claim has a provenance chain: the conclusion → the loop where it was witnessed → the git commit → the reproducible build state → the invariant check or hypothesis test that validated it.

### What AI governance frameworks are missing

Most current AI governance frameworks focus on:
1. Output restrictions (don't say X)
2. Bias audits (is the output fair across groups?)
3. Human-in-the-loop requirements (a person must approve)

What they rarely address:
1. Epistemic classification of outputs (is this a theorem, a finding, or an observation?)
2. Provenance of conclusions (what derivation produced this?)
3. Supersession tracking (when did this system change its mind, and why?)

The architecture here addresses all three by treating conclusions as first-class governed objects with explicit epistemic metadata, versioned formulas, and machine-verifiable derivations.

---

## Structural Outline for the Article

1. **Hook (2 paragraphs):** The admissibility problem. AI outputs have the format of findings but no verifiable derivation.
2. **The courtroom analogy (2 paragraphs):** Witness testimony vs. expert opinion vs. logical consequence. Different evidentiary standards.
3. **Three epistemic tiers (4 paragraphs):** Theorem, discovery finding, corpus pattern. One concrete example of each from the project.
4. **Chain of custody: the loop table (2 paragraphs):** CommitHash anchoring. Reproducible build states. Auditor workflow.
5. **Compliance controls: the invariant table (3 paragraphs):** Quote three critical invariants with PassCount/FailCount. Show that FailCount > 0 breaks the build.
6. **Due process at acquisition: IngestionProtocol (2 paragraphs):** Exclusion criteria. Documented exclusions in CandidateStudyCatalog.
7. **The provenance chain (2 paragraphs):** Conclusion → loop → git commit → invariant. Every claim is traceable.
8. **What frameworks are missing (2 paragraphs):** The three gaps. How this architecture fills them.
9. **Close (1 paragraph):** "AI governance should not merely restrict outputs. It should classify the legal and epistemic status of every output."

**Target length:** 1300–1600 words
**Suggested Medium tags:** AI Governance, Compliance, Legal AI, AI Ethics, Data Provenance
