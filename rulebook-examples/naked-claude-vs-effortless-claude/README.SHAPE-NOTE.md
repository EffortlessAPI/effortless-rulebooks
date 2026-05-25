# Shape note: this is an orchestration / comparison project — no rulebook

This folder lives inside `rulebook-examples/` but is **not** itself an Effortless Rulebook (ERB) project. It has no `effortless-rulebook/<name>-rulebook.json`.

It is the **orchestration harness** that ran the Naked-Claude vs. Effortless-Claude comparison — a meta-experiment about the methodology, not an example of a domain. Different shape is **intentional**.

## What it has

- `Naked-Claude-Experiments/` — comparison runs
- `tale-orchestrator.mjs`, `tale-report.mjs`, `tale-token-sampler.mjs` — the runner
- `v1_SPECIFICATION.md` ... `v4_SPECIFICATION.md` — what each variant was supposed to build
- `tale-comparison-*.html` — generated comparison reports
- `TALE_OF_TWO_CLAUDES.md`, `ORCHESTRATION_PLAN.md` — narrative

## What it does NOT have

- No rulebook JSON
- No transpilers; `effortless.json` here is for the orchestrator, not a rulebook build
- No execution substrates

## Why keep it here

It is the canonical "did this approach work?" evidence for the methodology. It is kept alongside the rulebook examples so the receipts are colocated with the domains the methodology was tested against.
