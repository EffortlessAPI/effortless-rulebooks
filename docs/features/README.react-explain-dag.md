# React ExplainDAG — In-App Provenance for Every Value

> **Any value displayed in an Effortless UI can answer the question "where did
> this come from?" — all the way down to ground truth — because the UI reads
> from the same DAG that the rulebook already proved.**

---

## The problem most apps can't solve

In a typical application — even a well-built one — a number on the screen is
opaque. You see `$142,300`. You do not know whether that came from a formula, a
manual override, a rollup of child records, or a raw database field. Tracing it
requires a developer, a debugger, and probably a conversation with whoever wrote
the query.

Vibe-coded apps have it worse: the provenance often doesn't exist anywhere. The
value is the value.

---

## What Effortless does instead

An Effortless project's UI is almost exclusively a read surface over the
ExplainDAG. Every field displayed in the React layer has a known type — raw,
formula, lookup, aggregation — declared in the rulebook. The React ExplainDAG
feature surfaces that structure in-app: any displayed value can be expanded to
show its full derivation graph, the same witnessed proof that the ExplainDAG
substrate generates at build time.

This is not a tooltip added by hand. It is a consequence of the architecture:
because every field's provenance is encoded in the rulebook, and the rulebook
drives the UI, the UI already *has* the provenance. Surfacing it is a rendering
decision, not an engineering project.

---

## What a user actually sees

Clicking a calculated field in the UI opens its derivation panel:

- The field's declared type (`formula`, `aggregation`, `lookup`)
- The formula or aggregation definition from the rulebook
- The upstream fields it depends on, each with their own witnessed values
- The chain traced back to raw fields — the ground truth inputs

A loan officer looking at a risk-grade migration sees not just the current grade,
but the covenant tests that drove it, the thresholds declared in the rulebook,
and the raw values that triggered each test. No developer required.

---

## Why this is only possible with ERB

This feature exists *because* the business logic lives in the rulebook, not
scattered across services, stored procedures, and component state. The React
layer does not need to reconstruct provenance — it reads it from the same
structure that the Python, SQL, and ExplainDAG substrates already consume.

Add a new derived field to the rulebook. Rebuild. The UI's provenance panel
covers it automatically. There is nothing to instrument.

---

## See also

- [ExplainDAG — Full Inference Tracing](README.explain-dag.md) — the substrate that generates the witnessed derivation proof
- [Rulebook is a complete spec](README.complete-spec.md) — why the rulebook alone is sufficient to answer any question about the domain
- [Hub-and-spoke topology](README.hub-and-spoke.md) — why all substrates, including the UI, read from the same source
- [What is non-linguistic?](../what-is-non-linguistic.md) — why structural encoding makes full provenance tracing possible without custom instrumentation
