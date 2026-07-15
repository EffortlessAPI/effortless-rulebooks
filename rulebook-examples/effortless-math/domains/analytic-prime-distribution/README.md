# Analytic Prime Distribution / Chebotarev

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `analytic-prime-distribution`
- **Legacy kernel ID:** `kernel-prime-distribution-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Chebotarev density for finite Galois quotients.
- **Provider facts:** `analytic_chebotarev_density_available`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- finite quotient
- complete conjugacy classes
- Frobenius classifier

### Conclusions / outputs

- positive density and representatives in every class

### Existing finite compiler interfaces

- `finite_galois_class_profiles_v19`
- `prime_count_density_stages_v19`

## Next target

Internalize the analytic prime-ideal theorem and Artin-L-function argument.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
