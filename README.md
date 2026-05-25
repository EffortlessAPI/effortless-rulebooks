# Effortlessly Invariant Rulebooks (ERB)

One declarative rulebook → many execution substrates, each conforming to the **same answer key**.

Write your business rules once in `effortless-rulebook.json`. ERB mechanically generates working implementations across [14 execution substrates](execution-substrates/) — Postgres, Python, Go, COBOL, Excel, OWL, English, UML, and more — then runs a conformance harness that proves every substrate returns identical results for the same inputs. Swap to a different domain ([acme-llc](rulebook-examples/acme-llc/), [star-trek](rulebook-examples/star-trek/), [jessica-advanced](rulebook-examples/jessica-advanced/), …) and the whole stack regenerates.

The rulebook is the answer key; substrates pass or fail against it. Substrate agreement is corroboration, not the test.

This repo is itself ERB-shaped. The [`effortless-platform/`](effortless-platform/) folder holds the orchestration tool, admin portal, and ssotme-proxy — built from their own rulebook, eating the same dog food. The [`rulebook-examples/`](rulebook-examples/) folder holds the demo domains the orchestrator manages. See [CLAUDE.md](CLAUDE.md) for the architectural rules (rulebook-as-SSoT, no fallbacks, project vs. demo rulebook category split).

## Features

- **One rulebook, many substrates** — write business rules once in `effortless-rulebook.json` and generate working implementations across 14 targets.
- **Conformance harness** — every substrate is tested against the same answer key, so cross-substrate agreement is mechanically verifiable.
- **SDLAF primitives** — Schema, Data, Lookups, Aggregations, and Formulas cover the full design-time surface with no procedural sidecar.
- **Bitemporal ACID DAG substrate** — every fact carries transaction-time and valid-time, transactions are consistent, and schema relationships are acyclic.
- **Swappable demo domains** — switch the active domain (acme-llc, star-trek, jessica-advanced, …) and the entire stack regenerates against it.
- **Self-hosting platform** — the orchestrator, admin portal, and ssotme-proxy are themselves generated from their own rulebook.
- **ssotme-proxy transpiler bus** — a local HTTP server (`localhost:4242`) exposes each injector as a first-class ssotme:// transpiler route.
- **Fail-loud discipline** — no silent fallbacks anywhere, so a wrong rulebook or wrong database surfaces immediately instead of faking conformance.
- **Project / demo rulebook category split** — the platform's own rulebook and the domain demo rulebooks are kept strictly separate to preserve the conformance claim.
- **Excel-compatible formula semantics** — calculated fields use `IF()`, `AND()`, `OR()`, `CONCAT()`, `LEFT()`, `RIGHT()`, etc., so domain experts can read and write them directly.

---

> See [`LEGACY_PLANs/`](LEGACY_PLANs/) for foundational concepts not yet extracted into the main docs.
