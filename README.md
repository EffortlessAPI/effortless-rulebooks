# Effortlessly Invariant Rulebooks (ERB)

One declarative rulebook → many execution substrates that all compute the **same** answers.

Write your business rules once in `effortless-rulebook.json`. ERB mechanically generates working implementations across [14 execution substrates](execution-substrates/) — Postgres, Python, Go, COBOL, Excel, OWL, English, UML, and more — then runs a conformance harness that proves every substrate returns identical results for the same inputs. Swap to a different domain ([acme-llc](rulebook-examples/acme-llc/), [star-trek](rulebook-examples/star-trek/), [jessica-advanced](rulebook-examples/jessica-advanced/), …) and the whole stack regenerates.

This repo is itself ERB-shaped. The [`effortless-platform/`](effortless-platform/) folder holds the orchestration tool, admin portal, and ssotme-proxy — built from their own rulebook, eating the same dog food. The [`rulebook-examples/`](rulebook-examples/) folder holds the demo domains the orchestrator manages. See [CLAUDE.md](CLAUDE.md) for the architectural rules (rulebook-as-SSoT, no fallbacks, project vs. demo rulebook category split).

---

> See [`LEGACY_PLANs/`](LEGACY_PLANs/) for foundational concepts not yet extracted into the main docs.
