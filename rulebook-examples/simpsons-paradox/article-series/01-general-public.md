# Trust the Artifact, Not the Autocomplete
## Article 1: A Public Guide to Auditable AI

**Series position:** 1 of 10 (publish first — broadest audience)
**Audience:** Educated lay readers, magazine readers, people who've heard AI is either magic or dangerous
**Central metaphor:** A courtroom. The LLM is a witness, not a judge.
**Tone:** Accessible, slightly skeptical of hype, ultimately constructive

---

## Original Outline

**Title:** *AI Should Not Be Trusted. It Should Be Audited.*

**Central metaphor:** A courtroom. The LLM is a witness, not a judge.

**Focus:** A realistic middle path between AI hype and AI dismissal.

**Translate terms this way:**

| Technical term       | Public version           |
| -------------------- | ------------------------ |
| LLM                  | Pattern witness          |
| Rulebook             | Evidence ledger          |
| Predicate invention  | New explanatory category |
| Deterministic output | Reproducible finding     |
| Governance           | Due process for ideas    |

**Article angle:**
The article opens with the skeptical position: AI can sound smart without being right. Then it introduces the architecture as a way to use AI without trusting AI prose. The model proposes; the rulebook tests; the artifact decides.

**Key lesson:**
"The future of trustworthy AI may not be better chatbots. It may be better systems for forcing chatbots to show their work."

---

## Source Material to Weave In

### The hook: a paradox that fooled everyone

Open with Simpson's Paradox. Berkeley 1973 admissions data: the university looked like it was discriminating against women. The aggregate numbers said so clearly. But when you broke the same numbers down by department, women were admitted at *higher* rates in almost every department. The aggregate conclusion was not just wrong — it was backwards.

This is not an AI story. This is a data story. But it's the perfect setup: a pattern that looks true, sounds true, is argued with confidence — and is the opposite of true once you examine the structure underneath.

### What the LLM does and does not do

In this project, an LLM was used to propose vocabulary. Things like: "What should we call the difference between the pooled winner and the stratum-level winner?" The model suggested terms like `IsSignFlip`, `AllocationDistortion`, `DistortionType`, `SignalPurity`. These are useful names for real phenomena.

What the LLM did *not* do: decide whether those names were correct, whether the formulas attached to them were right, or whether any study in the corpus actually fit the pattern the name implied.

That work was done by the rulebook and the tests.

### The rulebook as evidence ledger

The rulebook is a JSON file. Every concept in the domain — Study, Treatment, Stratum, CaseCell — is defined there. Every derived fact — `IsReversal`, `CorrectedWinner`, `PolicyImplication` — has a formula:

```
IsSignFlip = IF(WeightedStratumGapSum > 0, SignedPooledGap < 0, SignedPooledGap > 0)
```

That formula runs against every study in the corpus. If it breaks, the build fails. If it passes, the output is a reproducible finding — not an AI opinion.

### The invariants: things that must always be true

The project defines algebraic invariants — assertions that must hold across the entire dataset. Examples:

- **inv-type-ab-sign-flip**: If a study is Type A or Type B, it must have IsSignFlip=TRUE. (PassCount: 12, FailCount: 0)
- **inv-signal-purity-sign-flip**: If allocation direction is "reversal," SignalPurity must be < 0.5. (PassCount: 12, FailCount: 0)
- **inv-corrected-gap-invariant**: CorrectedGap must be constant across all possible allocation reweightings. (PassCount: 238, FailCount: 0)

Every one of these passes across the full N=238 corpus. That's not an AI claim. That's a machine-verified algebraic fact.

### The three tiers of knowledge

The project explicitly separates three kinds of conclusions:

1. **Theorems** — algebraic consequences of the definitions. These are true by construction. Example: "CorrectedGap is allocation-invariant" (Loop 34, witnessed with range = 0.0000 across 10 allocation reweightings).

2. **Discovery findings** — patterns found in the corpus after testing hypotheses. Example: "Economics studies have 0% sign-flip rate; epidemiology has ~27%." This is a corpus observation, not a theorem.

3. **Superseded patterns** — findings that held at N=64 but broke at N=238. These are archived, not deleted. The system knows what it used to believe and when it changed its mind.

### The courtroom framing

In a courtroom, a witness can say "I saw X." They cannot say "X is therefore proven." That's the jury's job, and the jury follows rules of evidence.

Here: the LLM is the witness. It can propose `AllocationDistortion` as a concept. But the rulebook is the rules of evidence. The invariant checks are the jury. The build either passes or it doesn't.

The output — a corpus of 238 studies, five taxonomy types, four algebraic theorems, domain-rate breakdowns — is the verdict. It can be reproduced by anyone who runs the build against the same input data.

### The punchline

The future of trustworthy AI isn't better chatbots. It's better infrastructure for treating chatbot outputs as witness testimony rather than expert judgment — and having a system that subjects every proposed concept to machine verification before it becomes part of the record.

---

## Structural Outline for the Article

1. **Hook (3 paragraphs):** Berkeley 1973. The paradox. The lesson: confident aggregate claims can be backwards.
2. **The problem with trusting AI prose (2 paragraphs):** LLMs are trained to produce plausible text. Plausible is not the same as verified. A model that proposes a category can't tell you if the category holds.
3. **A different architecture (3 paragraphs):** The rulebook as evidence ledger. Formulas as commitments. Builds as tests.
4. **What the invariants look like (2 paragraphs):** Pick two or three of the most intuitive invariants. Show that they pass across 238 studies.
5. **Three tiers of knowledge (2 paragraphs):** Theorems vs. corpus findings vs. superseded patterns. The system knows the difference.
6. **The courtroom metaphor (2 paragraphs):** Witness vs. jury. The LLM can propose; the artifact decides.
7. **Conclusion / call to action (2 paragraphs):** "The future of trustworthy AI may not be better chatbots. It may be better systems for forcing chatbots to show their work."

**Target length:** 1200–1500 words
**Suggested Medium tags:** Artificial Intelligence, Data Science, Statistics, Simpson's Paradox, AI Governance
