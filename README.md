# Effortlessly Invariant Rulebooks (ERB)

One declarative rulebook, many execution substrates, all conforming to the same answer key.

Write your business rules once in `effortless-rulebook.json`. ERB mechanically projects them into 14+ substrates (Postgres, Python, Go, COBOL, Excel, OWL, English, UML, …) and runs a conformance harness that proves every substrate returns identical results for the same inputs.

This repo wraps a catalog of independent demo rulebooks under `[rulebook-examples/](rulebook-examples/)` ([acme-llc](rulebook-examples/acme-llc/), [star-trek](rulebook-examples/star-trek/), [jessica-advanced](rulebook-examples/jessica-advanced/), …). Each project owns its own rulebook and picks the subset of substrates it needs. The platform itself (`[effortless-platform/](effortless-platform/)`) is one such project — ERB describing its own admin portal, orchestration tool, and ssotme-proxy.

See [CLAUDE.md](CLAUDE.md) for the architectural rules (rulebook-as-SSoT, project vs. demo rulebook category split).

---

## Headline features

1. **[Abstract Derivative Percentage (ADP)](docs/features/README.ADP.md)** — a first-class, measurable percentage of any ERB project that is derivative (mechanically rebuildable) vs. hand-written. `effortless -clean` plus a rebuild gives you the number.
2. `**[effortless -clean` / implicit clean before build](docs/features/README.clean.md)** — every build runs an implicit clean first. Derivative Code is whatever clean removes; Hand Code is whatever survives.
3. **[Rulebook as IR / Hub-and-Spoke topology](docs/features/README.hub-and-spoke.md)** — the rulebook JSON is the hub; every input and every output is a spoke. Spokes never talk to each other.
4. **[Convergent builds (not additive codegen)](docs/features/README.convergent-build.md)** — additions appear, removals disappear, renames propagate. The rulebook stays authoritative in both directions.
5. **[No privileged substrate / portability](docs/features/README.substrate-equivalence.md)** — every substrate is a peer projection. Languages not yet invented can be plugged in down the road.
6. **[Conformance testing across substrates](docs/features/README.conformance.md)** — substrate agreement is the empirical receipt that the rulebook is the SSoT.
7. **[Locally-designated SSoT](docs/features/README.local-ssot.md)** — the answer key for a run is whichever spoke the user designates (Airtable export, Excel workbook, JSON, Postgres dump).
8. **[Portal / CLI parity](docs/features/README.portal-cli-parity.md)** — the admin portal and `./start.sh --cli` are peer interfaces to the same pipeline; portal mutations shell out to the same `effortless` CLI commands.
9. **[Write-through invariant](docs/features/README.write-through.md)** — every portal save writes to Postgres AND the rulebook JSON in the same logical transaction. Drop Postgres at any time and rebuild from JSON.

## Additional features

- **[Rulebook is a complete spec](docs/features/README.complete-spec.md)** — a rulebook alone is sufficient for any frontier LLM to answer any question about the domain.
- **[Fail loudly, never fall back](docs/features/README.fail-loud.md)** — coding discipline: missing paths fail with the exact expected path; defaults derived from the SSoT are fine, guess-defaults are forbidden.
- **[Per-rulebook formula dialect](docs/features/README.dialect-binding.md)** — each rulebook declares its dialect (Excel, Airtable, …); substrates honor whatever is declared.
- **[Project rulebook ≠ demo rulebook](docs/features/README.project-vs-demo.md)** — the platform rulebook and the domain rulebooks never mix.
- **[Admin portal ≠ a domain](docs/features/README.portal-vs-domain.md)** — `erb_admin_portal` (the app) and `erb_<domain>` (the document) are categorically different; `erb_admin_<domain>` is a category-error red flag.
- **[ssotme-proxy transpiler bus](docs/features/README.ssotme-proxy.md)** — `localhost:4242` exposes every injector as a first-class `ssotme://` route.
- **[Self-hosting platform](docs/features/README.self-hosting.md)** — the orchestrator, admin portal, and ssotme-proxy are themselves generated from the platform rulebook.

---

> The [platform rulebook](effortless-platform/effortless-rulebook/effortless-rulebook.json) is the formal SSoT for this repository's own tooling, including this feature list. The `PlatformFeatures` table is authoritative; the READMEs linked above are hand-maintained but MUST conform to those rows. See `[LEGACY_PLANs/](LEGACY_PLANs/)` for foundational concepts not yet extracted into the main docs.

