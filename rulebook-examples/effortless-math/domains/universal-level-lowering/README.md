# Universal Ribet Level Lowering

## Current role

This is a first-class provider theorem in the `effortless-math` network. It begins as a thin contract migrated from Fermat Witness Lab v21. Fermat's Last Theorem consumes the provider facts below; the theorem's universal content is still imported.

## Contract

- **Theorem ID:** `universal-level-lowering`
- **Legacy kernel ID:** `kernel-universal-level-lowering-v21`
- **Current status:** `IMPORTED`
- **Mathematical scope:** Uniform one-prime descent for every admissible hypothetical FLT level.
- **Provider facts:** `level_2_weight_2_newform_exists`
- **Consumer:** `fermats-last-theorem`

### Hypotheses / inputs

- modularity
- irreducibility
- finite/local profile
- Ihara/component/multiplicity/lift generators

### Conclusions / outputs

- weight-two newform at the lowered level

### Existing finite compiler interfaces

- `edge_closure_audit_v9`
- `edge_closure_audit_v8`
- `edge_closure_audit_v7`
- `lower_level_eigenform_certificates`

## Next target

Internalize the universal quantifier over all admissible levels.

## Guardrail

Do not change this theorem to `FULLY_INTERNALIZED_FOR_SCOPE` merely because a parent edge has been decomposed or because finite examples pass. The universal conclusion must be derived above an explicit trust boundary, with every load-bearing child either internalized or declared.
