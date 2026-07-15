# Hilbert Specialization

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `hilbert-specialization`
- **Legacy kernel ID:** `kernel-hilbert-specialization-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Hilbert irreducibility with simultaneous weak approximation.
- **Provider facts:** `hilbert_irreducibility_weak_approximation_available`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- rational parameter space
- thin bad covers
- local open conditions

### Conclusions / outputs

- rational specialization outside thin sets satisfying every local open

### Existing finite compiler interfaces

- `thin_set_profiles_v19`
- `local_open_constraints_v19`
- `crt_specialization_certificates_v19`

## Next target

Internalize universal thin-set avoidance.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
