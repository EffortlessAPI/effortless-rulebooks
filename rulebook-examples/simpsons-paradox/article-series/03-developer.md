# Trust the Artifact, Not the Autocomplete
## Article 3: A Developer Guide to Rulebooks, Builds, and Invariants

**Series position:** 3 of 10
**Audience:** Software engineers, platform teams, API developers, architects
**Central metaphor:** Compiler and CI pipeline. LLM is a high-variance design assistant; build system decides.
**Tone:** Technical, direct, shows actual code and formulas

---

## Original Outline

**Title:** *LLMs Should Write Candidates; Build Systems Should Decide*

**Central metaphor:** Compiler and CI pipeline.

**Focus:** SSoT, deterministic builds, generated views, invariant checks, reproducibility.

**Translate terms this way:**

| Technical term         | Developer version                                    |
| ---------------------- | ---------------------------------------------------- |
| Rulebook               | Source code for knowledge                            |
| Predicate invention    | Schema/view evolution                                |
| Loop                   | Migration plus design note                           |
| Invariant              | Failing test                                         |
| Hallucination boundary | Runtime boundary between prose and compiled artifact |

**Article angle:**
Treat the LLM as a high-variance design assistant. It can propose tables, formulas, derived fields, naming schemes, and test cases. But the CI pipeline decides whether anything survives. The rulebook becomes the single source from which docs, APIs, views, summaries, and checks are generated.

**Key lesson:**
"The safe pattern is not 'LLM in production.' It is 'LLM upstream of a deterministic build.'"

---

## Source Material to Weave In

### The pattern: LLM proposes, build decides

The Simpson's Paradox project uses an LLM at the design phase only. It proposes:
- names for derived fields
- formulas for computing them
- vocabulary for classifying study types
- hypotheses to test against the corpus

Everything the LLM proposes goes into the rulebook JSON. The build runs. The invariant checks run. If anything fails, the loop is not complete. The LLM's suggestion is not a fact until the build says it is.

### The rulebook is the SSoT

The rulebook is a JSON schema-plus-data file. Every table is defined there. Every field has:
- `type`: `raw`, `calculated`, `lookup`, or `aggregation`
- `formula`: an Excel-style expression for non-raw fields
- `nullable`, `datatype`, `Description`

The build transpiles this to Postgres views, Python classes, OWL ontology, Excel exports, PlantUML diagrams, and RuleSpeak prose. You edit the rulebook; the build generates everything else. You never edit the generated files.

### Actual formula strings (verbatim from the rulebook)

```
IsSignFlip:
  =IF({{WeightedStratumGapSum}} = "", "",
    IF({{WeightedStratumGapSum}} > 0,
      {{SignedPooledGap}} < 0,
      {{SignedPooledGap}} > 0))

DistortionType:
  =IF({{AllocationDistortion}} = "", "", 
    IF({{IsSignFlip}},
      IF({{IsStratumUnanimous}}, "A", "B"),
      IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
             ABS({{SignedPooledGap}}) > ABS({{CorrectedGap}}) + 0.001), "C+",
      IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
             ABS({{SignedPooledGap}}) < ABS({{CorrectedGap}}) - 0.001), "C-",
      "D"))))

SignalPurity:
  =IF({{CorrectedGap}} = "", "", 
    IF(ABS({{CorrectedGap}}) + {{AllocationDistortion}} = 0, 1,
      ABS({{CorrectedGap}}) / (ABS({{CorrectedGap}}) + {{AllocationDistortion}})))

PolicyImplication:
  =IF({{DistortionType}} = "", "",
    IF({{DistortionType}} = "A", "stratify-immediately",
    IF({{DistortionType}} = "B", "investigate-confounder",
    IF(OR({{DistortionType}} = "C+", {{DistortionType}} = "C-"), "check-allocation-bias",
    "pooled-analysis-trustworthy"))))
```

The build transpiles these to SQL. The view `vw_treatment_rankings` has columns for all of them. Every downstream consumer reads from the view — never from the JSON, never from a formula interpreter.

### Loops as migrations with design notes

Each loop is a row in the `Loops` table inside the rulebook. It records:
- `LoopId`: `loop-01` through `loop-78+`
- `Title`: what new concept was introduced
- `NewConcept`: the entity, field, or table added
- `DomainQuestion`: the natural-language question it answers
- `WitnessNote`: what was actually observed after the build ran
- `CommitHash`: the git SHA where this loop first appears

This is a migration history with semantic annotations. A developer reading loop-17 sees: "IsSignFlip defined as the sign disagreement between WeightedStratumGapSum and SignedPooledGap. Witnessed: kidney-1986 IsSignFlip=TRUE (WSGSUM=+0.0537, SignedPooledGap=−0.0457); berkeley-1973 IsSignFlip=TRUE." That's a test fixture embedded in the migration record.

### Invariants as failing tests

The `InvariantChecks` table defines algebraic assertions. Each row:
- `InvariantCheckId`: slug
- `NaturalLanguage`: plain-English assertion
- `SourceTable`: which view to query
- `SqlAssertion`: a WHERE clause — if any row matches, the check fails
- `PassCount / FailCount`: populated on every build
- `Severity`: `critical` (build-breaking) or `warning`

Critical invariants (all passing at N=238):

```
inv-type-ab-sign-flip:
  DistortionType ∈ {A,B} → IsSignFlip=TRUE
  PassCount: 12, FailCount: 0

inv-signal-purity-sign-flip:
  AllocationDirection = 'reversal' → SignalPurity < 0.5
  PassCount: 12, FailCount: 0

inv-corrected-gap-invariant:
  SweepCorrectedGapRange < 0.0001 for every study
  PassCount: 238, FailCount: 0

inv-taxonomy-complete:
  DistortionType ∈ {A,B,C+,C-,D} for all rows
  PassCount: 40, FailCount: 0
```

A `FailCount > 0` on any critical invariant is a build-breaking bug. The project has never shipped a release with a failing invariant.

### Cross-substrate conformance: two compilers, same output

The build generates two independent implementations of the same formulas:
1. **Postgres**: SQL views generated from the rulebook formula strings
2. **OWL-SHACL**: SPARQL-CONSTRUCT rules generated from the same formula strings

`owl/reason.py` runs both, asserts the leaf inputs (StratumCasesA, StratumCasesB, etc.) into the OWL graph, derives the formula fields via SHACL fixpoint, and diffs against the Postgres view. Fields tested: IsReversal, DistortionType, CorrectedWinner, PolicyImplication, SignalPurity.

The conformance doc string:
```python
"""Cross-substrate conformance referee for Simpson's Paradox derived fields.

Two independent substrates compute the SAME formula fields from the SAME leaf
inputs... The transpiler could not emit SHACL rules for COUNTIFS/SUMIFS
aggregations (cross-row arithmetic — those require SQL GROUP BY or SPARQL
aggregates). So the conformance set is the fields that are purely formula-based
once aggregation inputs are provided.

Strategy: assert the Postgres-computed aggregation values into the working graph
alongside the raw individuals, then run SHACL fixpoint derivation to let the OWL
reasoner compute the pure formula fields. Compare those against Postgres.

This is the same pattern as the causal-autoimmune-architecture: assert the
leaves, derive the DAG, diff the two substrates."""
```

### The hallucination boundary

The LLM never touches the production path. The boundary is:

```
LLM → proposes field name + formula text
        ↓
    rulebook.json (SSoT, version-controlled)
        ↓
    effortless build (transpiler)
        ↓
    Postgres views + OWL + Python + Excel + ...
        ↓
    invariant checks (build-breaking if FailCount > 0)
        ↓
    reproducible corpus output
```

Prose from the LLM never enters the database. The LLM's suggestion of `SignalPurity = ABS(CorrectedGap) / (ABS(CorrectedGap) + AllocationDistortion)` became a governance artifact only after the formula was written into the rulebook and the invariant `inv-signal-purity-sign-flip` passed 12 times with 0 failures.

---

## Structural Outline for the Article

1. **Hook (2 paragraphs):** The safe pattern. Not "LLM in production" — "LLM upstream of a deterministic build."
2. **The rulebook as source code (3 paragraphs):** Schema, formulas, transpilation to multiple targets. Show the field type taxonomy.
3. **Actual formulas (2 paragraphs):** Quote IsSignFlip and DistortionType verbatim. Explain the Excel-style syntax.
4. **Loops as migrations (2 paragraphs):** Loop table as versioned design notes. CommitHash anchoring.
5. **Invariants as failing tests (3 paragraphs):** The invariant table schema. Quote four critical invariants with PassCount/FailCount.
6. **Cross-substrate conformance (2 paragraphs):** Two compilers, same formulas, same output. The conformance referee pattern.
7. **The hallucination boundary (2 paragraphs):** Draw the pipeline. Show where LLM output stops and artifact begins.
8. **Close (1 paragraph):** "The safe pattern is not 'LLM in production.' It is 'LLM upstream of a deterministic build.'"

**Target length:** 1400–1800 words
**Suggested Medium tags:** Software Engineering, AI Engineering, Data Engineering, Knowledge Graphs, Build Systems
