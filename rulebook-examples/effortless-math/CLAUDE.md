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

Do not start by building a custom UI. First make the global rulebook build and reproduce:

```text
8 theorems
7 load-bearing provider dependencies
571 loops
113 proof facts
305 invariant rows
1 contradiction
0 provider theorems fully internalized
```

## Host conventions

- Include `rulebooktorulespeak` in `effortless.json`.
- Set `ERB_DOMAIN=effortless-math` explicitly.
- Read generated values from `vw_*` views.
- Do not add defensive locks around `effortless build`.
- Do not add bespoke caches. Any materialization must be first-class rulebook data with a refresh contract.
