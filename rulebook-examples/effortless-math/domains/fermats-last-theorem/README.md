# Fermat's Last Theorem — Flagship Domain

This folder is the migration target for the deeply developed Fermat Witness Lab v21. It is not a claim that FLT has been independently re-proved without imported mathematics.

## Authoritative migration inputs

- `source-artifacts/fermat_witness_v21.sqlite`
- `source-artifacts/WITNESS_REPORT_V21.md`
- `source-artifacts/FINAL_NINE_ATTEMPT_V21.md`
- the complete frozen archive at `../../source-artifacts/fermat_witness_lab_v21.zip`

The SQLite database is authoritative for the legacy live state. The new rulebook becomes authoritative only after Claude has migrated the data, rebuilt every target substrate, and demonstrated conformance against the frozen v21 answer key.

## Current state

- 571 historical loops
- 113 current proof-trace facts
- 305 invariant rows
- 1 derived contradiction
- 7 active foundation-provider dependencies
- 0 providers marked fully internalized

## Non-negotiable status rule

`parent_removed = true` does **not** mean `theorem_content_fully_internalized = true`.

The migrated theorem remains `DERIVED_WITH_IMPORTED_CHILDREN` until provider certificates close the seven foundation contracts.
