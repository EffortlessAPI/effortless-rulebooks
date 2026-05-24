# Effortlessly Invariant Rulebooks (ERB)

One declarative rulebook, many execution substrates, all conforming to the same answer key.

Write your business rules once in `effortless-rulebook.json`. ERB mechanically projects them into 14+ substrates (Postgres, Python, Go, COBOL, Excel, OWL, English, UML, …) and runs a conformance harness that proves every substrate returns identical results for the same inputs.

This repo wraps a catalog of independent demo rulebooks under [`rulebook-examples/`](rulebook-examples/) ([acme-llc](rulebook-examples/acme-llc/), [star-trek](rulebook-examples/star-trek/), [jessica-advanced](rulebook-examples/jessica-advanced/), …). Each project owns its own rulebook and picks the subset of substrates it needs. The platform itself ([`effortless-platform/`](effortless-platform/)) is one such project — ERB describing its own admin portal, orchestration tool, and ssotme-proxy.

→ [What is non-linguistic?](docs/what-is-non-linguistic.md) · [GitHub](https://github.com/effortlessapi/effortless-rulebooks)

---

## Key features

- **[ExplainDAG](docs/features/README.explain-dag.md)** — for every derived value, a complete witnessed derivation graph showing which inputs, which operations, and what value at each step. Auditable, LLM-readable, generated before any production code runs.
- **[Conformance testing](docs/features/README.conformance.md)** — every build runs every substrate's test and shows the pass/fail matrix. There is no "build without testing."
- **[Hub-and-spoke topology](docs/features/README.hub-and-spoke.md)** — the rulebook is the hub; every input and output is a spoke. Spokes never talk to each other, eliminating the n×n integration problem.
- **[Convergent builds](docs/features/README.convergent-build.md)** — additions appear, removals disappear, renames propagate. Not additive codegen — the rulebook stays authoritative in both directions.
- **[Abstract Derivative Percentage (ADP)](docs/features/README.ADP.md)** — a first-class, measurable percentage of any project that is derivative (rebuildable) vs. hand-written. Typical ERB projects land at 60–80%.
- **[Rulebook is a complete spec](docs/features/README.complete-spec.md)** — sufficient for any frontier LLM to answer any question about the domain or produce a faithful implementation in any language.

→ [Full feature list with tiers and status](docs/derived/features.md)

---

## Demo domains

Five ready-to-run domains show the same pattern across wildly different problems — from a one-table Hello World to a philosophical meta-ontology:

| Domain | Complexity | What it demonstrates |
|---|---|---|
| [customer-fullname](rulebook-examples/customer-fullname/) | minimal | Hello World — string concat formula |
| [jessica-basic](rulebook-examples/jessica-basic/) | moderate | Relationships, aggregations, role-agent separation |
| [jessica-advanced](rulebook-examples/jessica-advanced/) | advanced | Cross-entity lookups, conditional IF logic |
| [star-trek](rulebook-examples/star-trek/) | moderate | Hierarchical rollups, polymorphic foreign keys |
| [is-everything-a-language](rulebook-examples/is-everything-a-language/) | philosophical | 8-predicate AND logic, formal argument modeling |

---

## Derived documentation

These pages are generated from the platform rulebook by `effortless build` — edit the rulebook, rebuild, and they update automatically. They are the authoritative reference, not hand-maintained summaries.

- **[Platform Features](docs/derived/features.md)** — the full catalog of all 16 features with tier, priority, and status. The README above is a curated excerpt.
- **[Execution Substrates](docs/derived/substrates.md)** — all 10 substrates with maturity rating, determinism, and whether they can serve as an answer key.
- **[Ontology Axioms](docs/derived/axioms.md)** — the 13 load-bearing claims the methodology rests on. If any one drops, the approach no longer holds.
- **[Rulebook Domains](docs/derived/domains.md)** — the full domain catalog including the ACME and self-referential demos not listed above.
- **[Substrate Contract](docs/derived/substrate-contract.md)** — the inject / execute / grade protocol every substrate must implement to participate in the conformance harness.

→ [Full derived docs index](docs/derived/README.md)

---

## Further reading

- [What is non-linguistic?](docs/what-is-non-linguistic.md) — why structural encoding beats linguistic representation, and why it matters for substrates
- [CLAUDE.md](CLAUDE.md) — the architectural rules (rulebook-as-SSoT, project vs. demo split, portal vs. domain) enforced across this repo
- [CMCC — the theoretical foundation](https://zenodo.org/records/15252466) — the conjecture that Schema, Data, Lookups, Aggregations, and Formulas over a bitemporal ACID DAG are sufficient for any finitely-computable design-time semantic

> The [platform rulebook](effortless-platform/effortless-rulebook/effortless-rulebook.json) is the formal SSoT for this repo's own tooling, including the feature and substrate catalogs above. Run `cd effortless-platform && effortless build` to regenerate.
