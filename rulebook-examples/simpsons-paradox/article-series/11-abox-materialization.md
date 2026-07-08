# Trust the Artifact, Not the Autocomplete
## Article 11 (Bonus): Pre-Materialize the ABox — Stop Reasoning What You Already Know

**Series position:** 11 (bonus)
**Audience:** PhD ontologists, semantic web engineers, knowledge graph practitioners
**Central metaphor:** A reference library that prints its answers before the reader walks in.
**Tone:** Technical, peer-to-peer, engages directly with OWL/SHACL practitioner assumptions

---

*This is a bonus article in the "Trust the Artifact, Not the Autocomplete" series, prompted by a concrete engineering decision made during the Simpson's Paradox conformance work.*

# Pre-Materialize the ABox — Stop Reasoning What You Already Know

The one-sentence version of what this article is about:

> **Pre-materialize your ABox from a relational substrate instead of deriving it at query time via a TBox reasoner.**

If that sentence clicked immediately, you have probably hit the same wall and are looking for permission to do the obvious thing. This article is that permission, plus the argument for why it is not a compromise — it is the correct architecture.

---

## The classical path and where it breaks

The standard OWL conformance path for a rich ontology looks like this:

1. Generate a sparse `individuals.ttl` from your seed data — one individual per entity, raw field values asserted, forward-FK edges as object properties.
2. Run a TBox reasoner (or SPARQL-CONSTRUCT rules under a SHACL fixpoint loop) to derive the ABox extensions: calculated fields, lookup joins, aggregations, closures.
3. Compare the resulting graph against the expected answers.

This works. It is also, for any ontology of meaningful complexity, very slow. For the Simpson's Paradox corpus — 238 studies, ~7,600 individuals, 366 SPARQL CONSTRUCT rules — the fixpoint loop runs for several minutes on a workstation. Every conformance check is a minutes-long wait.

The slowness is a symptom, not the root problem. The root problem is that the reasoner is being asked to compute values that have *already been computed*. Every derived field in this ontology — `IsSignFlip`, `DistortionType`, `SignalPurity`, `PolicyImplication`, `AllocationDistortion`, the full `TreatmentRankings` geometry — is the output of a formula transpiled to SQL and sitting in a Postgres view. The Postgres engine computed those values in sub-second time. The fixpoint loop then re-derives the same values from TBox rules, taking minutes, and the conformance check confirms that the two computations agree.

The fixpoint loop is redundant. Every cycle of it is proving that the reasoner re-derives what Postgres already knows.

---

## The alternative: hydrate the ABox before you load it

Instead of giving the reasoner a sparse ABox and asking it to fill in the derived triples, give it a **hydrated ABox** — one that already contains the computed values as asserted facts.

The architecture shifts from:

```
sparse individuals.ttl  →  fixpoint reasoner  →  full ABox  →  conformance diff
```

to:

```
Postgres vw_* views  →  hydrated individuals.ttl  →  SHACL shape check  →  conformance diff
```

The reasoner's role changes entirely. It is no longer being asked to *derive* — it is being asked to *validate shape*. Does this pre-asserted graph satisfy the SHACL shapes that describe the domain? That is a single-pass check, not a fixpoint computation. It takes seconds.

Conformance is now: load the hydrated graph, run shape validation, diff against Postgres. If they match — and they will, because Postgres *is* the source — the ontology is confirmed consistent. If they don't, you have found a transpiler bug.

---

## Why this is not cheating

The obvious objection: "you are asserting derived facts rather than deriving them — you have short-circuited the reasoning."

This objection assumes that the purpose of reasoning in conformance testing is to verify the TBox's derivation rules. It is not. The purpose is to verify that the ontology's vocabulary is consistent with the domain it describes. The derivation rules are not the hypothesis under test — the rulebook formulas are. The formulas were transpiled to SQL and tested there. Cross-substrate conformance is checking that the OWL substrate computes the *same answers* as Postgres, not that SPARQL-CONSTRUCT rules can independently re-derive the same answers from first principles.

Put differently: if you are testing a bridge by driving trucks across it, you are testing the bridge's load capacity. You are not required to also test the metallurgy of the steel from atomic first principles on every crossing. The metallurgy was tested when the steel was certified. Cross-substrate conformance is the truck, not the metallurgy lab.

The intensional definitions (the formulas) live in the rulebook — the single source of truth. They are compiled to SQL once, verified once, and then *used* by every substrate. A substrate that pre-asserts the Postgres-computed values is using the verified answer, not bypassing verification.

---

## What the OWL substrate becomes

Under hydrated-ABox architecture, the OWL layer serves three distinct and useful purposes that were previously conflated with "reasoning":

**1. Vocabulary certification.** The TBox declares the classes, properties, domain/range constraints, and cardinality restrictions. Loading the hydrated ABox under those declarations confirms that the pre-asserted values are consistent with the schema. An `xsd:boolean` property receiving a string value would fail here. A property asserted on an individual of the wrong class would fail here. This is genuine validation that the Postgres→OWL pipeline is type-safe.

**2. Structural completeness.** SHACL `sh:minCount` / `sh:maxCount` shapes confirm that every individual that should have a derived property does. If the hydrated export omitted a field due to a mapping bug, the shape check fails. This catches export bugs that a pure Postgres-to-Postgres diff would not catch.

**3. Query portability.** The hydrated graph is a complete, self-contained snapshot of the domain. Any SPARQL query that runs over it returns the same answers as a SQL query over the Postgres views — without requiring a running database. The graph is portable to environments where Postgres is not available: a SPARQL endpoint, a reasoner notebook, a linked-data publication pipeline.

None of these three purposes require re-deriving calculated fields from TBox rules. The derivation has already happened. The OWL layer receives the artifact, certifies its shape, and makes it portable.

---

## The deeper principle

This engineering decision is an instance of a more general rule: **don't re-compute inside a validation substrate what a dedicated computation substrate already computed.**

Classical OWL practice evolved in a world where the ontology was the *only* substrate — there was no Postgres, no transpiler, no formula DAG. In that world, reasoning was the computation layer; the TBox rules *were* the operational semantics. Deriving the ABox extensions by fixpoint was not redundant; it was the only way to get them.

In a multi-substrate architecture where the intensional definitions are in a rulebook, transpiled to multiple independent execution engines, and tested for cross-substrate consistency, the reasoning layer is no longer the computation layer. It is one of several verification layers. Treating it as a computation layer — running fixpoint derivation on every conformance check — is applying a solution from a single-substrate world to a multi-substrate problem. The result is slowness and redundancy where there should be speed and clarity.

The fix is architectural, not implementational: move the computation where it belongs (the relational substrate, where GROUP BY and window functions run natively) and keep the reasoning where it belongs (structural certification and query portability).

---

## Concrete mechanics for the Simpson's Paradox case

The implementation follows from the principle:

1. **`rulebook-to-postgres`** generates SQL schema, seed data, and `vw_*` view definitions from the rulebook JSON. This is an existing transpiler.

2. **`rulebook-to-hydrated-triples`** (new) runs the generated SQL in a temporary Postgres instance, queries every `vw_*` view, and emits `individuals-hydrated.ttl` — one individual per row, one triple per non-null computed column, using the same IRI and property conventions as the existing `rulebook-to-triples` tool.

3. **Conformance** loads `individuals-hydrated.ttl`, runs SHACL shape validation (not CONSTRUCT derivation), and diffs the asserted property values against the Postgres view output. Sub-second load; no fixpoint loop.

The hydrated file is a generated artifact — it changes every time the corpus or formulas change, the same as any other generated output. It is not hand-edited. It is not the source of truth. It is the OWL substrate's view of what the relational substrate computed, expressed in the vocabulary the TBox declared.

The result: conformance testing drops from minutes to seconds. The OWL ontology covers the full derived geometry of the domain. SPARQL queries run over a complete, self-consistent graph. And the fixpoint derivation loop — which was always a workaround for the absence of a relational substrate, not a feature — is retired.

---

## Summary

The reasoning loop is the right tool when reasoning is the computation. When computation has already happened in a dedicated substrate, the right tool is export and validation. The distinction matters for performance, for architectural clarity, and for what the ontology layer is actually being asked to do.

Pre-materialize the ABox from the relational substrate. Let the reasoner certify the shape. Trust the artifact.
