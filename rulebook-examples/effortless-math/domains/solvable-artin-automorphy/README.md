# Solvable Artin Automorphy

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `solvable-artin-automorphy`
- **Legacy kernel ID:** `kernel-solvable-automorphy-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Solvable base change, automorphic induction, and converse theorem.
- **Provider facts:** `solvable_base_change_converse_available`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- odd finite image
- dihedral/tetrahedral/octahedral router
- local factors

### Conclusions / outputs

- weight-one cuspidal automorphic representation

### Existing finite compiler interfaces

- `artin_projective_type_profiles_v19`
- `automorphic_induction_state_machines_v19`
- `artin_local_factor_certificates_v19`

## Next target

Internalize the analytic Langlands-Tunnell kernel.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
