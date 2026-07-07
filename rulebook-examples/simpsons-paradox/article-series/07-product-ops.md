# Trust the Artifact, Not the Autocomplete
## Article 7: An Operations Guide to Repeatable AI Workflows

**Series position:** 7 of 10
**Audience:** Product managers, operations leaders, process designers, quality teams
**Central metaphor:** A kitchen recipe that gets tested every time before serving.
**Tone:** Practical, outcome-focused, avoids jargon, uses familiar process analogies

---

## Original Outline

**Title:** *Turning AI Brainstorms into Reliable Operating Procedures*

**Central metaphor:** A kitchen recipe that gets tested every time before serving.

**Focus:** Repeatable workflows, decision support, policy implications, avoiding one-off AI answers.

**Translate terms this way:**

| Technical term       | Product/ops version       |
| -------------------- | ------------------------- |
| Rulebook             | Operating recipe          |
| Predicate            | Decision label            |
| Loop                 | Process improvement cycle |
| Invariant            | Quality gate              |
| Deterministic output | Repeatable procedure      |

**Article angle:**
A product or operations team does not need philosophical debates about whether AI "reasons." They need to know whether the same input produces the same defensible output. This version shows how AI-generated ideas can become standard operating logic instead of ephemeral chat transcripts.

**Key lesson:**
"Useful AI does not just answer questions. It improves the operating system of the organization."

---

## Source Material to Weave In

### The problem: AI answers that can't be turned into procedures

A product team asks an AI: "How should we classify this user complaint?" The model gives a thoughtful answer. Someone screenshots it and pastes it into a doc. A month later, a different team member asks the same question, gets a slightly different answer, and pastes that one instead. Now there are two versions of the procedure, neither of which was tested, and neither of which the organization can audit.

This is not a problem of AI capability. It's a problem of AI output format. A chat response is not a procedure. A procedure is: given input X, produce output Y, and here is how you verify Y is correct.

### The recipe analogy

A recipe has three parts:
1. **Ingredients** (inputs): flour, eggs, sugar — measured precisely
2. **Method** (procedure): combine, bake at 350°, test with toothpick
3. **Quality check** (invariant): toothpick comes out clean; cake is golden brown

If the quality check fails, you do not serve the cake. You find out why the recipe produced a bad result before the next batch.

An operating procedure has the same structure. The problem with AI-generated procedures is they have part 1 and sometimes part 2 — but never part 3. There's no quality gate. The output goes out whether it's right or wrong.

### What the project does differently

The Simpson's Paradox project is, at its operational core, a classification procedure. Given a study with raw case counts, it outputs a decision label: stratify-immediately, investigate-confounder, check-allocation-bias, or pooled-analysis-trustworthy.

That output — `PolicyImplication` — is the operational output of a 78-iteration process improvement cycle. Here's how it was built:

**Loop 4**: First draft of the classification. Just IsReversal — is there a paradox or not?

**Loop 21**: The four-way taxonomy introduced. LLM proposed the names; the formulas were defined; the build ran.

**Loop 27**: CorrectedWinner added — what's the right answer once allocation bias is removed?

**Loop 30**: Causal role boundary layer added — when is stratification appropriate and when is it dangerous?

Each loop is a process improvement cycle. Each one adds a new quality dimension to the output. The loop history is the change log for the operating procedure.

### The quality gates: invariant checks

The project defines quality gates that run on every build:

**Gate 1** — `inv-taxonomy-complete`: Every input study produces a classification. No blanks, no nulls. PassCount: 40, FailCount: 0.

**Gate 2** — `inv-type-ab-sign-flip`: Studies classified Type A or B must show a sign-flip. This ensures the classification is internally consistent. PassCount: 12, FailCount: 0.

**Gate 3** — `inv-corrected-gap-invariant`: The corrected output (CorrectedGap) must not depend on how patients happen to be distributed. PassCount: 238, FailCount: 0.

If any gate returns FailCount > 0, the procedure is not published. The quality check is not optional. It runs before the output reaches any consumer.

### The six-step intake procedure

The project also defines a standard intake procedure for adding new studies. This is not AI-generated — it's a machine-verifiable checklist:

1. Create a Studies row with metadata (StudyId, Title, Source, Year, Domain)
2. Create a StratumVariables row (variable name, causal role)
3. Create Treatment rows (one per arm, A/B)
4. Create Strata rows (one per category)
5. Create CaseCells rows (one per 2×K cell with Cases and Successes)
6. Update CandidateStudyCatalog status → 'imported'

Any study that misses a step produces a build error. The error is specific: "Missing CaseCells for stratum X," not "something went wrong." The procedure is self-auditing.

### Deterministic outputs: same input, same answer

The most important operational property: given the same study data, the build always produces the same classification. This is not about AI reliability in the sense of "the model might hallucinate." It's about reproducibility: the formula is fixed, the data is fixed, the output is fixed.

When the corpus is extended to include a new study, the existing outputs do not change. Adding berkeley-1973 does not change the classification of kidney-1986. The procedure is additive, not recomputed.

This matters for operations: a procedure that changes its historical outputs when new data arrives is not a procedure — it's a model. A procedure's past decisions are stable.

### The PolicyImplication as a decision tool

The operational output of the classification procedure is a four-value decision label:

- **stratify-immediately**: The aggregated result is backwards. Do not use it. The stratum-level analysis is the right one.
- **investigate-confounder**: The aggregated result may be backwards. Examine the confounding variable before deciding.
- **check-allocation-bias**: The aggregated result is in the right direction but the magnitude is wrong. Use the corrected estimate.
- **pooled-analysis-trustworthy**: The aggregated result is reliable. No adjustment needed.

This is a decision support tool, not a research output. A clinician, a policy analyst, or an operations leader can receive this label and know exactly what to do next — without reading a 20-page methodology paper.

---

## Structural Outline for the Article

1. **Hook (2 paragraphs):** The screenshot problem. AI answers that can't be turned into procedures.
2. **The recipe analogy (2 paragraphs):** Ingredients, method, quality check. AI usually skips the third part.
3. **What the project is operationally (2 paragraphs):** A classification procedure. Given raw case counts → decision label. Built over 78 improvement cycles.
4. **The improvement cycles (3 paragraphs):** Walk through loops 4, 21, 27, 30 as process iterations. Each adds a quality dimension.
5. **The quality gates (3 paragraphs):** Quote three invariants. Show that FailCount > 0 means procedure is not published.
6. **The six-step intake procedure (2 paragraphs):** The StudyImportTemplate checklist. Self-auditing — errors are specific.
7. **Deterministic outputs (2 paragraphs):** Same input, same answer. Adding new data doesn't change historical outputs.
8. **The PolicyImplication as a decision tool (2 paragraphs):** Four-value label. What each value tells the decision-maker.
9. **Close (1 paragraph):** "Useful AI does not just answer questions. It improves the operating system of the organization."

**Target length:** 1200–1500 words
**Suggested Medium tags:** Product Management, Operations, AI Implementation, Process Design, Decision Support
