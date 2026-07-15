# Mazur Modular-Curve Arithmetic

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `mazur-modular-curve-arithmetic`
- **Legacy kernel ID:** `kernel-mazur-formal-immersion-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Formal immersion at the cusp plus finiteness of the Eisenstein quotient.
- **Provider facts:** `mazur_formal_immersion_eisenstein_kernel_available`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- cyclic isogeny degree ledger
- cusp specialization
- tangent rank

### Conclusions / outputs

- rational point forced to the cusp
- rational isogeny exclusion

### Existing finite compiler interfaces

- `cyclic_isogeny_degree_ledger_v19`
- `formal_immersion_tangent_certificates_v19`
- `frey_local_specialization_profiles_v19`

## Next target

Internalize the Eisenstein quotient and formal-immersion theorem.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
