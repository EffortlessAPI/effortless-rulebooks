# v21 → Effortless Math Mapping

| v21 concept | New canonical concept | Notes |
|---|---|---|
| `foundation_kernels_v21` | `FoundationKernels` + provider `Theorems` | Preserve kernel IDs as legacy aliases. |
| `import_surface WHERE load_bearing=1` | `TheoremDependencies` | Convert direct imports into provider/consumer edges. |
| `proof_trace` | `ProofFacts` | One row per current derived fact. |
| `loops` | `Loops` | All 571 rows are first-class history. |
| `invariant_checks` | `InvariantChecks` | Keep IMPORTED statuses distinct from PASS. |
| `sources` | `Sources` | Preserve IDs and URLs. |
| `nine_parent_audit_v21` | `LegacyParentAudits` | Provenance only; does not close providers. |
| v21 files and SQLite | `ArtifactRegistry` | Hash every migration source. |

## Deliberately not migrated as authoritative meaning

The 364-table v21 SQLite schema records many historical implementation stages. It remains available as evidence and migration source, but the starter does not copy every legacy table into the new canonical rulebook. Claude should migrate a table only when its concept belongs in the stable cross-theorem meta-model or in a theorem-specific provider domain.
