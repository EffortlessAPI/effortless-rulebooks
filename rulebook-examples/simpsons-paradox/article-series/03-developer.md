# Trust the Artifact, Not the Autocomplete
## Article 3: A Developer Guide to Rulebooks, Builds, and Invariants

**Series position:** 3 of 10
**Audience:** Software engineers, platform teams, API developers, architects
**Central metaphor:** Compiler and CI pipeline. LLM is a high-variance design assistant; build system decides.
**Tone:** Technical, direct, shows actual code and formulas

---

*This is part 3 of a 10-part series exploring what it looks like to build trustworthy AI systems with verifiable outputs.*

# LLMs Should Write Candidates; Build Systems Should Decide

The safe pattern is not "LLM in production." It is "LLM upstream of a deterministic build."

This is a distinction with consequences. An LLM in production means the model's output is the result — whatever it says, that's what the system returns. LLM upstream of a build means the model's output is a proposal: a candidate field name, a candidate formula, a candidate schema — that goes through a compiler, a test suite, and a quality gate before it becomes part of any artifact. The build decides. The model does not.

This article is about how that pipeline works in practice, using a knowledge model of Simpson's Paradox as the concrete example.

---

## The rulebook as source code

In this project, all knowledge lives in a single JSON file: the rulebook. It is the single source of truth from which every other artifact is generated. You edit the rulebook; the build generates Postgres views, Python classes, OWL ontology, Excel exports, PlantUML diagrams, and RuleSpeak prose. You never edit the generated files.

The rulebook defines tables (entities), and each table defines fields. Every field has:
- `type`: one of `raw`, `calculated`, `lookup`, or `aggregation`
- `formula`: an Excel-style expression for non-raw fields
- `nullable`, `datatype`, `description`

The entity model for Simpson's Paradox looks like this:

- **Study** — metadata for a published study (StudyId, Domain, Year, Source)
- **Treatment** — a treatment arm within a study (TreatmentLabel, TreatmentId)
- **Stratum** — a subgroup within a study (StratumLabel, StratumId)
- **CaseCell** — raw counts: SuccessCount and CaseCount for one treatment in one stratum
- **TreatmentRankings** — derived view per treatment pair: every formula field lives here

Raw fields are leaf nodes. Every derived field — `IsSignFlip`, `DistortionType`, `SignalPurity`, `PolicyImplication` — is computed by formula from the leaves. The build transpiles those formulas to SQL. No formula interpreter runs at query time. You read from the view; the view already has the answer.

---

## Actual formulas from the rulebook

The formulas are Excel-style expressions stored as strings in the rulebook JSON. The transpiler converts them to SQL. Here is what they look like verbatim:

```
IsSignFlip:
  =IF({{WeightedStratumGapSum}} = "", "",
    IF({{WeightedStratumGapSum}} > 0,
      {{SignedPooledGap}} < 0,
      {{SignedPooledGap}} > 0))
```

This says: if the allocation-weighted average of per-stratum gaps points positive but the pooled gap is negative (or vice versa), the study is a sign flip. That's the mathematical core of Simpson's Paradox, expressed as a formula, version-controlled, transpiled to SQL, tested on every build.

```
DistortionType:
  =IF({{AllocationDistortion}} = "", "", 
    IF({{IsSignFlip}},
      IF({{IsStratumUnanimous}}, "A", "B"),
      IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
             ABS({{SignedPooledGap}}) > ABS({{CorrectedGap}}) + 0.001), "C+",
      IF(AND(NOT({{IsSignFlip}}), {{AllocationDistortion}} > 0.01,
             ABS({{SignedPooledGap}}) < ABS({{CorrectedGap}}) - 0.001), "C-",
      "D"))))
```

Five types. Every study in the corpus gets exactly one. The LLM proposed the names and the initial taxonomy structure. The build verified that the taxonomy is complete — that every study is assigned, that no study is assigned twice, and that the algebraic invariants downstream of the taxonomy hold.

```
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

These four formulas form a DAG. `PolicyImplication` depends on `DistortionType`, which depends on `IsSignFlip` and `IsStratumUnanimous`, which depend on `WeightedStratumGapSum` and `SignedPooledGap`, which are aggregations over `CaseCell` rows. The transpiler walks the DAG and emits SQL that computes the full chain from leaves to policy output. The view `vw_treatment_rankings` has columns for all of them.

---

## Loops as migrations with design notes

Each build iteration is a row in the `Loops` table inside the rulebook. Think of it as a migration history with semantic annotations:

- `LoopId`: `loop-01` through `loop-85` complete (loops 86–90 next; 97–107 queued)
- `Title`: what new concept was introduced
- `NewConcept`: the entity, field, or table added
- `DomainQuestion`: the natural-language question it answers
- `WitnessNote`: what was actually observed after the build ran
- `CommitHash`: the git SHA where this loop first appears

Loop 17, for example: "IsSignFlip defined as the sign disagreement between WeightedStratumGapSum and SignedPooledGap. Witnessed: kidney-1986 IsSignFlip=TRUE (WSGSUM=+0.0537, SignedPooledGap=−0.0457); berkeley-1973 IsSignFlip=TRUE." That's a test fixture embedded in the migration record. Anyone reading the loop table at loop 17 knows exactly what evidence justified the definition and can reproduce it.

Loop 34: "SweepCorrectedGap = 0.05367 across all 10 allocation rows (range = 0.0000). SweepPooledGap ranges from +0.130 to −0.084 (range ≈ 0.214), crossing zero near f_L=0.50."

These are not analyst notes. They are machine-witnessed observations recorded as governed data. The loop table is a versioned design document with cryptographic anchoring.

---

## Invariants as failing tests

The `InvariantChecks` table is the test suite. Each row defines an algebraic assertion that must hold across the full corpus. The schema:

- `InvariantCheckId`: slug identifier
- `NaturalLanguage`: plain-English statement of what must be true
- `SourceTable`: which view to query
- `SqlAssertion`: a WHERE clause — if any row matches, the check fails
- `PassCount / FailCount`: populated on every build
- `Severity`: `critical` (build-breaking) or `warning`

Four critical invariants, all passing at N=238:

```
inv-type-ab-sign-flip:
  "All Type A and B studies must have IsSignFlip = TRUE"
  PassCount: 12, FailCount: 0

inv-signal-purity-sign-flip:
  "AllocationDirection = 'reversal' → SignalPurity < 0.5"
  PassCount: 12, FailCount: 0

inv-corrected-gap-invariant:
  "SweepCorrectedGapRange < 0.0001 for every study"
  PassCount: 238, FailCount: 0

inv-taxonomy-complete:
  "DistortionType ∈ {A, B, C+, C-, D} for all rows"
  PassCount: 40, FailCount: 0
```

A `FailCount > 0` on any critical invariant is a build-breaking bug. The project has never shipped a release with a failing critical invariant. This is not a policy — it is enforced by the build system. If you change a formula in a way that breaks an invariant, the build fails. There is no "we'll fix it later."

This is the same discipline you enforce in a well-run CI pipeline for application code. The novelty is that it applies to knowledge artifacts — formulas, taxonomies, derived fields — not just functions and classes.

---

## Cross-substrate conformance: two compilers, same output

The build generates two independent implementations of the same formulas. The first is Postgres: SQL views generated from the rulebook formula strings. The second is OWL-SHACL: SPARQL-CONSTRUCT rules generated from the same formula strings.

`owl/reason.py` runs both. It asserts the leaf inputs — StratumCasesA, StratumCasesB, TreatmentExposureFraction — into the OWL graph, derives the formula fields via SHACL fixpoint, and diffs against the Postgres view. The conformance set covers: IsReversal, DistortionType, CorrectedWinner, PolicyImplication, SignalPurity.

The module docstring is worth quoting directly:

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

Two completely different execution engines — a relational database and a description logic reasoner — read the same rulebook and produce the same answers. When they disagree, the rulebook is the arbiter. This is what cross-substrate conformance means: one source of truth, multiple implementations, one test that runs both.

---

## The hallucination boundary

The LLM never touches the production path. The boundary is explicit:

```
LLM → proposes field name + formula text
        ↓
    rulebook.json  (SSoT, version-controlled)
        ↓
    effortless build  (transpiler)
        ↓
    Postgres views + OWL + Python + Excel + ...
        ↓
    invariant checks  (build-breaking if FailCount > 0)
        ↓
    reproducible corpus output
```

Prose from the LLM never enters the database. The LLM's suggestion of `SignalPurity = ABS(CorrectedGap) / (ABS(CorrectedGap) + AllocationDistortion)` became a governance artifact only after the formula was written into the rulebook and the invariant `inv-signal-purity-sign-flip` passed 12 times with 0 failures. Before that, it was a candidate. A proposal. A hypothesis.

The model is a high-variance design assistant. It generates candidates quickly and fluently. It cannot verify them. It cannot test them. It does not know whether its suggestion is algebraically consistent with the rest of the DAG, whether it produces meaningful results on the actual corpus, or whether it breaks any downstream invariants. The build knows all of those things.

---

## What this means for AI engineering

The architecture described here is not exotic. It is the discipline engineers already apply to software: write code (now, with LLM assistance), run tests, ship only what passes. The extension is applying that same discipline to knowledge artifacts — the concepts, formulas, and taxonomies that encode domain understanding.

The LLM makes that work faster. It can propose `IsSignFlip` before you have thought of the exact mathematical formulation. It can suggest five distortion types before you have run any studies. It can generate a candidate formula for `SignalPurity` in seconds. That speed is real and valuable.

What makes it trustworthy is everything that comes after: the rulebook as SSoT, the transpiler that generates consistent implementations, the invariant table as a failing test suite, the cross-substrate conformance check as a second opinion. The LLM is upstream of all of that. The build is downstream. The build decides.

The safe pattern is not LLM in production. It is LLM upstream of a deterministic build.

---

**Also in this series:**
- *Part 4: A Data Scientist's Guide to Predicate Invention* — feature engineering with an audit trail, hypothesis pre-registration, and the difference between a theorem and a finding
- *Part 10: A PhD Ontologist's Guide to Formal Knowledge Representation* — the OWL-SHACL conformance architecture in formal terms

*Full series overview: Trust the Artifact, Not the Autocomplete (10 articles)*
