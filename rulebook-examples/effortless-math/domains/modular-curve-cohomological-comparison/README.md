# Modular-Curve Cohomological Comparison

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `modular-curve-cohomological-comparison`
- **Legacy kernel ID:** `kernel-modular-curve-comparison-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Betti-etale, de Rham/Dieudonne, Eichler-Shimura, and nearby-cycles comparison.
- **Provider facts:** `modular_curve_geometric_comparison_available`, `semistable_nearby_cycles_comparison_available`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- finite coefficient towers
- Hecke correspondences
- monodromy models

### Conclusions / outputs

- actual geometric realization at good and bad primes

### Existing finite compiler interfaces

- `betti_etale_stage_certificates_v17`
- `hecke_comparison_squares_v17`
- `eichler_shimura_stage_certificates_v15`
- `monodromy_comparison_certificates_v17`

## Next target

Internalize functorial comparison for the actual modular curve and Jacobian.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
