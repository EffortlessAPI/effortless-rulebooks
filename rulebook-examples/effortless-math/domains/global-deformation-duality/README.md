# Global Deformation and Duality

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `global-deformation-duality`
- **Legacy kernel ID:** `kernel-global-deformation-duality-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Schlessinger/Mazur representability and Poitou-Tate global-local duality.
- **Provider facts:** `deformation_problem_represented`, `poitou_tate_balance_available`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- absolutely irreducible residual representation
- finite tangent/obstruction spaces
- local conditions

### Conclusions / outputs

- universal deformation ring
- Selmer/dual-Selmer exact balance

### Existing finite compiler interfaces

- `deformation_problem_contracts_v10`
- `tangent_obstruction_interfaces_v10`
- `poitou_tate_profiles_v10`

## Next target

Internalize actual Galois cochain realization, representability, and duality.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
