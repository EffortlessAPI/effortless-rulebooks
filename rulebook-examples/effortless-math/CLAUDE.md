# CLAUDE.md — Effortless Math Domain

Read and obey the host repository's root `CLAUDE.md` first. This file adds mathematical-domain constraints.

## Category boundary

`effortless-math` is domain data. Do not put admin-portal, orchestration, user-role, or platform configuration tables in its rulebook. Do not put theorem-domain entities in the platform rulebook.

## SSoT

The canonical semantic asset is:

```text
effortless-rulebook/effortless-math-rulebook.json
```

Do not create a second theorem-status registry in Python, SQL, TypeScript, YAML, or prose. Generate projections from the rulebook.

## Rulebook JSON formatting (non-negotiable)

The rulebook JSON is **always standard JSON with single-line leaves**:

- Standard JSON — no comments, no trailing commas, valid `json.load`-able.
- **Single-line leaves:** every leaf record is emitted on exactly ONE line. A leaf record is an individual `data[]` row object and an individual `schema[]` field object. Container keys (`Domains`, `Theorems`, the `schema`/`data` arrays, top-level metadata) stay pretty-printed and indented; only the innermost objects collapse to one line each.
- The reason: leaf-per-line keeps `git diff` legible (one changed row = one changed line) and keeps the file greppable (`grep '"TheoremId": "2-plus-2..."'` finds the whole row).

Do NOT hand-produce fully-expanded (every-field-on-its-own-line) rulebook JSON. After any edit, normalize with the project formatter so schema field objects and data row objects are each one line. The canonical normalizer is `scripts/format-rulebook.py` (single-line-leaves pretty-printer); run it after every rulebook edit and never commit a rulebook that has not been through it.

**`effortless build` re-expands the rulebook.** The `rulebook-to-postgres` transpiler rewrites `effortless-math-rulebook.json` in place during a build (FK normalization etc.) and emits it fully expanded. So the formatter must run **after every build**, not just after hand edits — the last thing you do before committing is `python3 scripts/format-rulebook.py`. The formatter is idempotent and asserts the reformatted file parses to an identical object, so it is always safe to run.

## Fail loudly

- Missing v21 files or hash mismatch: raise with the exact path.
- Unknown theorem/provider ID: raise.
- Unsupported rulebook field shape: stop and migrate the rulebook; do not silently ignore it.
- Conformance disagreement: fail the build.
- Do not look for a legacy path if the declared path is missing.

## Proof-status doctrine

Never conflate:

```text
parent removed
parent derived
theorem fully internalized for scope
zero imports above a declared trust boundary
```

They are different data fields and different claims.

A finite calibration, replayable computation, or green invariant does not establish a universal theorem unless the universal inference is represented and its child frontier is closed.

## Migration doctrine

The v21 archive is immutable evidence. Preserve IDs, hashes, and bitemporal history. The new rulebook becomes authoritative only after the host conformance harness reproduces the frozen answer key from a blank build.

## Provider theorem doctrine

Provider theorem folders export contracts and eventually provider certificates. FLT consumes only versioned contracts/certificates, never provider-private tables or code.

A provider certificate must state:

- theorem ID and version;
- bound hypotheses;
- conclusions supplied;
- proof status;
- remaining imports;
- artifact hashes;
- generated timestamp.

## First integration objective

Do not start by building a custom UI. First make the global rulebook build and reproduce the frozen **v21 witness** (these numbers come from the immutable v21 SQLite and never change):

```text
8 migrated v21 theorems (FLT + 7 foundation providers)
7 load-bearing provider dependencies
571 v21 loops
113 v21 proof facts
305 v21 invariant rows
1 contradiction
0 provider theorems fully internalized
```

### natural-number-arithmetic addendum (loops 572–576)

The `natural-number-arithmetic` (Peano) domain adds the network's first fully internalized, zero-import theorems on top of the frozen v21 witness. It does NOT alter any v21 count above. After it is present the rulebook totals are:

```text
12 theorems          (8 v21 + 4 natural-number-arithmetic: + − × ÷)
7 provider dependencies   (unchanged; the new theorems are zero-import)
575 loops            (571 v21 + loops 572–575; loop 576 is the cleanup loop)
135 proof facts      (113 v21 + 22 Peano reduction steps)
313 invariant rows   (305 v21 + 8 peano-reduction-certificate checks)
4 conclusions        (2 v21 + 2 natural-number-arithmetic)
9 domains, 9 trust boundaries, 8 foundation kernels
1 kernel with theorem_content_internalized = 1  (kernel-peano-naturals-v1; the 7 v21 kernels remain 0)
4 theorems FULLY_INTERNALIZED_FOR_SCOPE with ActiveImportCount = 0
```

`scripts/validate_starter.py` asserts the 12-theorem / 7-dependency shape and the frozen v21 SHA-256 + SQLite counts; keep both in sync with this block.

## Host conventions

- Include `rulebooktorulespeak` in `effortless.json`.
- Set `ERB_DOMAIN=effortless-math` explicitly.
- Read generated values from `vw_*` views.
- Do not add defensive locks around `effortless build`.
- Do not add bespoke caches. Any materialization must be first-class rulebook data with a refresh contract.
