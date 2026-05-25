# Shape note: this is a Naked-Claude baseline — no rulebook

This folder lives inside `rulebook-examples/` but is **not** an Effortless Rulebook (ERB) project. It is the **Naked-Claude baseline** for the v3 comparison — a plain JS app built without a rulebook. Different shape is **intentional**.

## What it has

- `src/`, `public/` — application source
- `package.json`, `package-lock.json` — npm app
- `start.sh` — launch script
- `README.md` — what the v3 naked variant was supposed to build

## What it does NOT have

- No rulebook JSON
- No `effortless.json` (not a rulebook-driven project)
- No execution substrates

## Why keep it here

It is the "no rulebook" control sample for the v3 Naked-Claude vs. Effortless-Claude experiment. The matching ERB-shaped version lives next door at `v3-nakedclaude-demo/`. Keeping both side-by-side preserves the comparison.
