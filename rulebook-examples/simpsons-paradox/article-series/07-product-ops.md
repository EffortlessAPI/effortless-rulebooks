# Trust the Artifact, Not the Autocomplete
## Article 7: An Operations Guide to Repeatable AI Workflows

**Series position:** 7 of 10
**Audience:** Product managers, operations leaders, process designers, quality teams
**Central metaphor:** A kitchen recipe that gets tested every time before serving.
**Tone:** Practical, outcome-focused, avoids jargon, uses familiar process analogies

---

*This is part 7 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# Turning AI Brainstorms into Reliable Operating Procedures

A product team asks an AI: "How should we classify this user complaint?" The model gives a thoughtful answer. Someone screenshots it and pastes it into a document. A month later, a different team member asks the same question, gets a slightly different answer, and pastes that one instead. Now there are two versions of the procedure. Neither was tested. Neither can be audited. Neither is reliably reproducible.

This is not a problem of AI capability. The model was probably helpful both times. It is a problem of output format. A chat response is not a procedure. A procedure is: given input X, produce output Y, and here is how you verify Y is correct. AI generates the first two parts fluently and skips the third entirely.

The third part is the only part that makes a procedure reliable.

---

## The recipe that gets tested before serving

A recipe has three parts. Ingredients: flour, eggs, sugar, measured precisely. Method: combine in order, bake at 350°, check at 25 minutes. Quality check: toothpick comes out clean; top is golden brown.

If the quality check fails, you do not serve the result. You find out what went wrong before the next batch.

An operating procedure has the same structure. The problem with AI-generated procedures is they reliably cover parts one and two. The AI can describe inputs and steps clearly, often better than a human could draft on first attempt. But part three — the quality gate — is absent. The output ships whether it is right or wrong because nothing in the pipeline checks.

The question for any operations team adopting AI-generated workflows is not "is the AI good at producing outputs?" It is "does the system test those outputs before anyone acts on them?"

---

## What this project is, operationally

The Simpson's Paradox knowledge model is, at its operational core, a classification procedure. Given a published study with raw case counts, it produces a decision label: one of four values that tells a practitioner exactly what to do with the aggregated result.

That output — `PolicyImplication` — is the final product of a 78-iteration process improvement cycle. Each iteration added a new quality dimension. The cycle was not driven by the AI; it was driven by the question "what does the output need to be good enough to act on?" The AI accelerated individual steps. The iteration structure is what made the procedure reliable.

This is the operational pattern worth borrowing: not "use AI to generate a procedure" but "use AI inside a structured improvement cycle where each iteration earns its way in through a quality gate."

---

## How the procedure was built, iteration by iteration

**Iteration 4** introduced the first draft of the classification: a binary flag, IsReversal — does this study show a statistical reversal or not? That is the minimal viable procedure. It answered one question: is the aggregate result trustworthy?

**Iteration 21** extended the classification to five types. The LLM proposed the names and the rough taxonomy. The build ran. The quality gates checked that every study received exactly one classification, that the classification was internally consistent, and that no study fell through the cracks. The five-type taxonomy passed. Now the procedure answered a richer question: *what kind* of distortion is present?

**Iteration 27** added `CorrectedWinner` — the answer to "which treatment actually wins once you remove the allocation bias?" The procedure now answered not just whether the aggregate was misleading, but what the right answer was.

**Iteration 30** added a causal role boundary layer — the conditions under which stratification is appropriate and the conditions under which it introduces different problems. The procedure now answered: not just what the right answer is, but when it is safe to act on it.

Each iteration is a process improvement cycle. Each one added a quality dimension that the previous version lacked. The loop history is the change log for the operating procedure — exactly the kind of documentation that process improvement frameworks require and that AI-only workflows never produce.

---

## The quality gates that ship the procedure

Three quality gates run on every build. If any gate fails, the procedure is not published.

**Gate 1 — Completeness** (`inv-taxonomy-complete`): Every input study must produce a classification. No blanks. No nulls. No "could not determine." PassCount: 40, FailCount: 0. A procedure that fails to classify some inputs is not a procedure — it is a procedure with undocumented exceptions. This gate enforces completeness.

**Gate 2 — Consistency** (`inv-type-ab-sign-flip`): Studies classified as Type A or B must show a sign-flip. This ensures the classification is internally consistent — that the taxonomy labels mean what the taxonomy says they mean. PassCount: 12, FailCount: 0. A procedure whose outputs contradict its own definitions has a logic error that will cause downstream decisions to be wrong in ways that are hard to detect.

**Gate 3 — Stability** (`inv-corrected-gap-invariant`): The corrected output — the real answer once allocation bias is removed — must not depend on how patients happen to be distributed between groups. PassCount: 238, FailCount: 0. A procedure that gives different answers depending on arbitrary distributional details is not reliable in operational settings where those distributions vary.

If any gate returns FailCount > 0, the build fails. The procedure is not published in that state. The quality check is not optional, and it is not deferred to a review step that might get skipped when the team is busy. It is enforced by the build system before the output reaches any consumer.

---

## The standard intake procedure

The project also defines a standard intake procedure for adding new studies to the corpus. This is not AI-generated — it is a machine-verifiable checklist:

1. Create a Studies row with metadata: StudyId, Title, Source, Year, Domain
2. Create a StratumVariables row: the variable name, and whether its causal role is confirmed
3. Create Treatment rows: one per arm, labeled A and B
4. Create Strata rows: one per category of the stratification variable
5. Create CaseCell rows: one per treatment-stratum combination, with Cases and Successes counts
6. Update the CandidateStudyCatalog status to 'imported'

Any study that misses a step produces a specific build error: "Missing CaseCells for stratum X," not "something went wrong." The procedure is self-auditing. An incomplete intake produces a traceable failure, not a silent gap in the corpus.

This is the operational property that makes the procedure scalable: anyone who follows the six steps produces a valid result. The procedure does not require the person who designed it to check every entry. The build checks every entry automatically.

---

## Same input, same answer

The most important operational property of a procedure is determinism: given the same inputs, the procedure always produces the same output. This sounds obvious, but it is precisely the property that AI-only workflows lack. Ask the same question on two different days and you may get meaningfully different answers, for reasons that are not visible in the output.

In this procedure, the formula is fixed. The data is fixed. The output is fixed. When the corpus is extended with a new study, the existing outputs do not change. Adding one study does not recompute the classification of any other study. The procedure is additive: new inputs produce new outputs; old inputs continue to produce the same outputs they always have.

This matters for operations because decisions are made on a timeline. A decision made in January using the classification from January should not retroactively change because new data was added in March. A procedure that rewrites historical outputs is not a procedure — it is a continuously updating model, and that model's past outputs cannot be relied upon as a stable record.

---

## The decision label as an operational output

The operational output of the classification procedure is a four-value decision label designed for a practitioner who does not have time to read the methodology:

**stratify-immediately**: The aggregated result is backwards. Do not use it. The stratum-level analysis is the correct one. Stop acting on the aggregate.

**investigate-confounder**: The aggregated result may be backwards. Before making any decision based on it, examine the confounding variable to determine whether the reversal applies.

**check-allocation-bias**: The aggregated result points in the right direction but the magnitude is wrong — it is either amplified or compressed by unequal allocation. Use the corrected estimate rather than the raw pooled figure.

**pooled-analysis-trustworthy**: The aggregated result is reliable. No adjustment is needed. Act on the pooled number.

A clinician, a policy analyst, or an operations leader can receive one of these four labels and know exactly what to do next — without reading a statistical methodology paper, without consulting the team that built the model, and without asking the AI to explain itself again. The label is the output of a procedure that has already done the explanation, formalized it, tested it, and published only what passed.

---

## What this means for AI in operations

The pattern is transferable. The specific domain — Simpson's Paradox, medical studies, allocation distortion — is not what matters. What matters is the structure:

AI participates in the early stages of procedure design: proposing names, suggesting taxonomies, drafting classification rules. Human judgment decides which proposals are worth formalizing. The build system tests every formalized proposal against quality gates before it ships. The procedure's history is a documented improvement cycle, not a sequence of chat sessions.

The result is a procedure that operations teams can rely on: same input, same output, known quality gates, auditable history, and a decision label that tells practitioners what to do without requiring them to understand the methodology.

Useful AI does not just answer questions. It improves the operating system of the organization — and the improvement is documented, tested, and owned by the organization, not locked inside a model that might answer differently tomorrow.

---

**Also in this series:**
- *Part 2: A Boardroom Guide to Governed AI Knowledge* — why AI pilots fail to become durable assets, and what changes that
- *Part 3: A Developer Guide to Rulebooks, Builds, and Invariants* — the technical infrastructure behind the quality gates

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*
