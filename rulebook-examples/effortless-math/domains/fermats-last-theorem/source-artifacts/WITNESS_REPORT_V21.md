# Fermat Witness Lab v21 — Attempting the Final Nine Imports

## Build summary

```json
{
  "loops": 571,
  "new_loops": 70,
  "remaining_parent_imports_attempted": 9,
  "parent_imports_removed": 9,
  "foundation_kernels_added": 7,
  "active_imports_before": 9,
  "active_imports_after": 7,
  "parents_fully_internalized": 0,
  "hard_failures": 0,
  "derived_contradictions": 1
}
```

All nine v20 parent imports are removed from the live proof trace. They are
not claimed to have disappeared mathematically. Their irreducible content is
consolidated into seven explicit foundation kernels.

## Why nine become seven

Two pairs share a common kernel:

- Mazur representability and Poitou–Tate use one global deformation/duality kernel;
- good-prime geometric comparison and semistable nearby cycles use one modular-curve comparison kernel.

The other five parents retain distinct foundation kernels.

## Rewritten parent facts

```json
[
  {
    "step_no": 26,
    "derived_fact": "mazur_formal_immersion_eisenstein_kernel_available",
    "derivation_kind": "derived-from-mazur-formal-kernel"
  },
  {
    "step_no": 36,
    "derived_fact": "hilbert_irreducibility_weak_approximation_available",
    "derivation_kind": "derived-from-hilbert-specialization-kernel"
  },
  {
    "step_no": 38,
    "derived_fact": "solvable_base_change_converse_available",
    "derivation_kind": "derived-from-solvable-automorphy-kernel"
  },
  {
    "step_no": 58,
    "derived_fact": "analytic_chebotarev_density_available",
    "derivation_kind": "derived-from-prime-distribution-kernel"
  },
  {
    "step_no": 66,
    "derived_fact": "deformation_problem_represented",
    "derivation_kind": "derived-from-deformation-duality-kernel"
  },
  {
    "step_no": 68,
    "derived_fact": "modular_curve_geometric_comparison_available",
    "derivation_kind": "derived-from-modular-curve-comparison-kernel"
  },
  {
    "step_no": 69,
    "derived_fact": "semistable_nearby_cycles_comparison_available",
    "derivation_kind": "derived-from-modular-curve-comparison-kernel"
  },
  {
    "step_no": 74,
    "derived_fact": "poitou_tate_balance_available",
    "derivation_kind": "derived-from-deformation-duality-kernel"
  },
  {
    "step_no": 112,
    "derived_fact": "level_2_weight_2_newform_exists",
    "derivation_kind": "derived-from-universal-level-lowering-kernel"
  }
]
```

## Seven-kernel frontier

```json
[
  {
    "kernel_order": 1,
    "kernel_id": "kernel-prime-distribution-v21",
    "title": "Analytic prime distribution",
    "mathematical_scope": "Chebotarev density for finite Galois quotients.",
    "input_contract_json": "[\"finite quotient\", \"complete conjugacy classes\", \"Frobenius classifier\"]",
    "output_contract_json": "[\"positive density and representatives in every class\"]",
    "finite_compiler_tables_json": "[\"finite_galois_class_profiles_v19\", \"prime_count_density_stages_v19\"]",
    "source_id": "src-prime-distribution-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize the analytic prime-ideal theorem and Artin-L-function argument."
  },
  {
    "kernel_order": 2,
    "kernel_id": "kernel-hilbert-specialization-v21",
    "title": "Hilbert specialization",
    "mathematical_scope": "Hilbert irreducibility with simultaneous weak approximation.",
    "input_contract_json": "[\"rational parameter space\", \"thin bad covers\", \"local open conditions\"]",
    "output_contract_json": "[\"rational specialization outside thin sets satisfying every local open\"]",
    "finite_compiler_tables_json": "[\"thin_set_profiles_v19\", \"local_open_constraints_v19\", \"crt_specialization_certificates_v19\"]",
    "source_id": "src-hilbert-specialization-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize universal thin-set avoidance."
  },
  {
    "kernel_order": 3,
    "kernel_id": "kernel-mazur-formal-immersion-v21",
    "title": "Mazur modular-curve arithmetic",
    "mathematical_scope": "Formal immersion at the cusp plus finiteness of the Eisenstein quotient.",
    "input_contract_json": "[\"cyclic isogeny degree ledger\", \"cusp specialization\", \"tangent rank\"]",
    "output_contract_json": "[\"rational point forced to the cusp\", \"rational isogeny exclusion\"]",
    "finite_compiler_tables_json": "[\"cyclic_isogeny_degree_ledger_v19\", \"formal_immersion_tangent_certificates_v19\", \"frey_local_specialization_profiles_v19\"]",
    "source_id": "src-mazur-modular-curve-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize the Eisenstein quotient and formal-immersion theorem."
  },
  {
    "kernel_order": 4,
    "kernel_id": "kernel-global-deformation-duality-v21",
    "title": "Global deformation and duality",
    "mathematical_scope": "Schlessinger/Mazur representability and Poitou-Tate global-local duality.",
    "input_contract_json": "[\"absolutely irreducible residual representation\", \"finite tangent/obstruction spaces\", \"local conditions\"]",
    "output_contract_json": "[\"universal deformation ring\", \"Selmer/dual-Selmer exact balance\"]",
    "finite_compiler_tables_json": "[\"deformation_problem_contracts_v10\", \"tangent_obstruction_interfaces_v10\", \"poitou_tate_profiles_v10\"]",
    "source_id": "src-global-deformation-duality-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize actual Galois cochain realization, representability, and duality."
  },
  {
    "kernel_order": 5,
    "kernel_id": "kernel-modular-curve-comparison-v21",
    "title": "Modular-curve cohomological comparison",
    "mathematical_scope": "Betti-etale, de Rham/Dieudonne, Eichler-Shimura, and nearby-cycles comparison.",
    "input_contract_json": "[\"finite coefficient towers\", \"Hecke correspondences\", \"monodromy models\"]",
    "output_contract_json": "[\"actual geometric realization at good and bad primes\"]",
    "finite_compiler_tables_json": "[\"betti_etale_stage_certificates_v17\", \"hecke_comparison_squares_v17\", \"eichler_shimura_stage_certificates_v15\", \"monodromy_comparison_certificates_v17\"]",
    "source_id": "src-modular-curve-comparison-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize functorial comparison for the actual modular curve and Jacobian."
  },
  {
    "kernel_order": 6,
    "kernel_id": "kernel-universal-level-lowering-v21",
    "title": "Universal Ribet level lowering",
    "mathematical_scope": "Uniform one-prime descent for every admissible hypothetical FLT level.",
    "input_contract_json": "[\"modularity\", \"irreducibility\", \"finite/local profile\", \"Ihara/component/multiplicity/lift generators\"]",
    "output_contract_json": "[\"weight-two newform at the lowered level\"]",
    "finite_compiler_tables_json": "[\"edge_closure_audit_v9\", \"edge_closure_audit_v8\", \"edge_closure_audit_v7\", \"lower_level_eigenform_certificates\"]",
    "source_id": "src-universal-level-lowering-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize the universal quantifier over all admissible levels."
  },
  {
    "kernel_order": 7,
    "kernel_id": "kernel-solvable-automorphy-v21",
    "title": "Solvable Artin automorphy",
    "mathematical_scope": "Solvable base change, automorphic induction, and converse theorem.",
    "input_contract_json": "[\"odd finite image\", \"dihedral/tetrahedral/octahedral router\", \"local factors\"]",
    "output_contract_json": "[\"weight-one cuspidal automorphic representation\"]",
    "finite_compiler_tables_json": "[\"artin_projective_type_profiles_v19\", \"automorphic_induction_state_machines_v19\", \"artin_local_factor_certificates_v19\"]",
    "source_id": "src-solvable-automorphy-kernel-v21",
    "theorem_content_internalized": 0,
    "status": "ACTIVE-FOUNDATION-KERNEL",
    "next_target": "Internalize the analytic Langlands-Tunnell kernel."
  }
]
```

## Nine-parent audit

```json
[
  {
    "edge_order": 1,
    "edge_id": "import-analytic-chebotarev-density-v19",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-prime-distribution-v21",
    "kernel_shared_with_parent_count": 1,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize the analytic prime-ideal theorem and Artin-L-function argument."
  },
  {
    "edge_order": 2,
    "edge_id": "import-hilbert-irreducibility-weak-approximation-v19",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-hilbert-specialization-v21",
    "kernel_shared_with_parent_count": 1,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize universal thin-set avoidance."
  },
  {
    "edge_order": 3,
    "edge_id": "import-mazur-formal-immersion-eisenstein-v19",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-mazur-formal-immersion-v21",
    "kernel_shared_with_parent_count": 1,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize the Eisenstein quotient and formal-immersion theorem."
  },
  {
    "edge_order": 4,
    "edge_id": "import-mazur-representability",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-global-deformation-duality-v21",
    "kernel_shared_with_parent_count": 2,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize actual Galois cochain realization, representability, and duality."
  },
  {
    "edge_order": 5,
    "edge_id": "import-poitou-tate-rt",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-global-deformation-duality-v21",
    "kernel_shared_with_parent_count": 2,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize actual Galois cochain realization, representability, and duality."
  },
  {
    "edge_order": 6,
    "edge_id": "import-modular-curve-geometric-comparison-v17",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-modular-curve-comparison-v21",
    "kernel_shared_with_parent_count": 2,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize functorial comparison for the actual modular curve and Jacobian."
  },
  {
    "edge_order": 7,
    "edge_id": "import-semistable-nearby-cycles-v17",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-modular-curve-comparison-v21",
    "kernel_shared_with_parent_count": 2,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize functorial comparison for the actual modular curve and Jacobian."
  },
  {
    "edge_order": 8,
    "edge_id": "import-ribet-level-lowering",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-universal-level-lowering-v21",
    "kernel_shared_with_parent_count": 1,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize the universal quantifier over all admissible levels."
  },
  {
    "edge_order": 9,
    "edge_id": "import-solvable-base-change-converse-v19",
    "previous_status": "IMPORTED",
    "current_status": "DERIVED-FROM-FOUNDATION-KERNEL",
    "parent_removed": 1,
    "replacement_kernel_id": "kernel-solvable-automorphy-v21",
    "kernel_shared_with_parent_count": 1,
    "theorem_content_fully_internalized": 0,
    "status": "PARENT-REMOVED-FOUNDATION-KERNEL-REMAINS",
    "next_target": "Internalize the analytic Langlands-Tunnell kernel."
  }
]
```

## Active import surface

```json
[
  {
    "edge_id": "import-kernel-global-deformation-duality-v21",
    "title": "Global deformation and Poitou-Tate foundation kernel",
    "next_decomposition_target": "Internalize actual Galois deformation representability and global duality."
  },
  {
    "edge_id": "import-kernel-hilbert-specialization-v21",
    "title": "Hilbert specialization foundation kernel",
    "next_decomposition_target": "Internalize universal thin-set avoidance."
  },
  {
    "edge_id": "import-kernel-mazur-formal-immersion-v21",
    "title": "Mazur formal-immersion/Eisenstein foundation kernel",
    "next_decomposition_target": "Internalize formal immersion and Eisenstein quotient finiteness."
  },
  {
    "edge_id": "import-kernel-modular-curve-comparison-v21",
    "title": "Modular-curve cohomological comparison foundation kernel",
    "next_decomposition_target": "Internalize good- and bad-prime geometric comparison."
  },
  {
    "edge_id": "import-kernel-prime-distribution-v21",
    "title": "Analytic prime-distribution foundation kernel",
    "next_decomposition_target": "Internalize analytic Chebotarev."
  },
  {
    "edge_id": "import-kernel-solvable-automorphy-v21",
    "title": "Solvable Artin automorphy foundation kernel",
    "next_decomposition_target": "Internalize the Langlands-Tunnell analytic kernel."
  },
  {
    "edge_id": "import-kernel-universal-level-lowering-v21",
    "title": "Universal Ribet level-lowering foundation kernel",
    "next_decomposition_target": "Internalize universal one-prime descent."
  }
]
```

## Interpretation

The conditional end-to-end contradiction still replays with zero hard
invariant failures. The current active count is seven, but these are deeper
foundation-level theorems rather than ordinary parent edges. v21 therefore
marks `theorem_content_fully_internalized = false` for all nine replaced
parents and `theorem_content_internalized = false` for all seven kernels.

## Invariant health

```json
[
  {
    "tier": "adic-certificate",
    "status": "PASS",
    "check_count": 8,
    "pass_rows": 90,
    "fail_rows": 0,
    "universe_rows": 90
  },
  {
    "tier": "adic-local-model",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "adversarial-invariant",
    "status": "PASS",
    "check_count": 15,
    "pass_rows": 81,
    "fail_rows": 0,
    "universe_rows": 81
  },
  {
    "tier": "adversarial-phase-diagram",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "algebraic",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 22,
    "fail_rows": 0,
    "universe_rows": 22
  },
  {
    "tier": "algebraic-local",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 12,
    "fail_rows": 0,
    "universe_rows": 12
  },
  {
    "tier": "algebraic-proof-rule",
    "status": "PASS",
    "check_count": 7,
    "pass_rows": 23,
    "fail_rows": 0,
    "universe_rows": 23
  },
  {
    "tier": "assembled-with-imports",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "automorphy-contract",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "bitemporal-correction",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "bitemporal-edge-closure",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "bounded-computation",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 5,
    "fail_rows": 0,
    "universe_rows": 5
  },
  {
    "tier": "commutative-algebra",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 5,
    "fail_rows": 0,
    "universe_rows": 5
  },
  {
    "tier": "comparison-contract",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "computational-theorem",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "conditional-algebraic",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 6,
    "fail_rows": 0,
    "universe_rows": 6
  },
  {
    "tier": "conditional-certificate",
    "status": "PASS",
    "check_count": 5,
    "pass_rows": 15,
    "fail_rows": 0,
    "universe_rows": 15
  },
  {
    "tier": "conditional-derivation",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "conditional-inference",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 48,
    "fail_rows": 0,
    "universe_rows": 48
  },
  {
    "tier": "conditional-on-eta-quotient-theorem",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "conditional-on-ribet",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "conditional-on-sturm",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "conditional-state-machine",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 7,
    "fail_rows": 0,
    "universe_rows": 7
  },
  {
    "tier": "conditional-with-imported-local-criterion",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 20,
    "fail_rows": 0,
    "universe_rows": 20
  },
  {
    "tier": "deformation-contract",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "derived-algebra",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "derived-local-theorem",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 12,
    "fail_rows": 0,
    "universe_rows": 12
  },
  {
    "tier": "duality-contract",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "epistemic-routing",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 6,
    "fail_rows": 0,
    "universe_rows": 6
  },
  {
    "tier": "exact-group-law",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 5,
    "fail_rows": 0,
    "universe_rows": 5
  },
  {
    "tier": "exact-local-arithmetic",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 29,
    "fail_rows": 0,
    "universe_rows": 29
  },
  {
    "tier": "exact-modular-curve-arithmetic",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "exact-series",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 41,
    "fail_rows": 0,
    "universe_rows": 41
  },
  {
    "tier": "exact-symbolic-reduction",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 15,
    "fail_rows": 0,
    "universe_rows": 15
  },
  {
    "tier": "explicit-characteristic-zero-lift",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "finite-algebra",
    "status": "PASS",
    "check_count": 12,
    "pass_rows": 66,
    "fail_rows": 0,
    "universe_rows": 66
  },
  {
    "tier": "finite-artin-router",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "finite-character-theory",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "finite-coefficient-certificate",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 19,
    "fail_rows": 0,
    "universe_rows": 19
  },
  {
    "tier": "finite-commutative-algebra",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 56,
    "fail_rows": 0,
    "universe_rows": 56
  },
  {
    "tier": "finite-comparison",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "finite-computation",
    "status": "PASS",
    "check_count": 10,
    "pass_rows": 689,
    "fail_rows": 0,
    "universe_rows": 689
  },
  {
    "tier": "finite-correspondence",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 32,
    "fail_rows": 0,
    "universe_rows": 32
  },
  {
    "tier": "finite-density-evidence",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "finite-double-coset",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 8,
    "fail_rows": 0,
    "universe_rows": 8
  },
  {
    "tier": "finite-exhaustion",
    "status": "PASS",
    "check_count": 4,
    "pass_rows": 4092,
    "fail_rows": 0,
    "universe_rows": 4092
  },
  {
    "tier": "finite-field-exhaustion",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "finite-field-factorization",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 9,
    "fail_rows": 0,
    "universe_rows": 9
  },
  {
    "tier": "finite-flat-group-scheme",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 9,
    "fail_rows": 0,
    "universe_rows": 9
  },
  {
    "tier": "finite-galois-classes",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "finite-group",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 23,
    "fail_rows": 0,
    "universe_rows": 23
  },
  {
    "tier": "finite-group-cohomology",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 5,
    "fail_rows": 0,
    "universe_rows": 5
  },
  {
    "tier": "finite-group-computation",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "finite-group-exhaustion",
    "status": "PASS",
    "check_count": 4,
    "pass_rows": 12,
    "fail_rows": 0,
    "universe_rows": 12
  },
  {
    "tier": "finite-group-proof",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 39,
    "fail_rows": 0,
    "universe_rows": 39
  },
  {
    "tier": "finite-group-scheme-law",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "finite-hecke-module",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "finite-homology",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 58,
    "fail_rows": 0,
    "universe_rows": 58
  },
  {
    "tier": "finite-isogeny-ledger",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 48,
    "fail_rows": 0,
    "universe_rows": 48
  },
  {
    "tier": "finite-linear-algebra",
    "status": "PASS",
    "check_count": 18,
    "pass_rows": 205,
    "fail_rows": 0,
    "universe_rows": 205
  },
  {
    "tier": "finite-local-algebra",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 24,
    "fail_rows": 0,
    "universe_rows": 24
  },
  {
    "tier": "finite-local-arithmetic",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 124,
    "fail_rows": 0,
    "universe_rows": 124
  },
  {
    "tier": "finite-local-factors",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 44,
    "fail_rows": 0,
    "universe_rows": 44
  },
  {
    "tier": "finite-module",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 8,
    "fail_rows": 0,
    "universe_rows": 8
  },
  {
    "tier": "finite-moduli-enumeration",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "finite-monodromy",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "finite-prime-search",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "finite-prime-selection",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 8,
    "fail_rows": 0,
    "universe_rows": 8
  },
  {
    "tier": "finite-proof-certificate",
    "status": "PASS",
    "check_count": 5,
    "pass_rows": 11,
    "fail_rows": 0,
    "universe_rows": 11
  },
  {
    "tier": "finite-pseudocharacter",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 28,
    "fail_rows": 0,
    "universe_rows": 28
  },
  {
    "tier": "finite-quotient-selection",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "finite-rational-points",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 8,
    "fail_rows": 0,
    "universe_rows": 8
  },
  {
    "tier": "finite-representation-profile",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "finite-representation-theory",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 17,
    "fail_rows": 0,
    "universe_rows": 17
  },
  {
    "tier": "finite-semistability-filter",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "finite-specialization",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "finite-toy",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "formal-geometry-schema",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "foundation-contract",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "foundation-frontier-rewrite",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "foundation-import",
    "status": "IMPORTED",
    "check_count": 7,
    "pass_rows": 0,
    "fail_rows": 0,
    "universe_rows": 0
  },
  {
    "tier": "generator-coverage",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "graph-derivation",
    "status": "PASS",
    "check_count": 6,
    "pass_rows": 6,
    "fail_rows": 0,
    "universe_rows": 6
  },
  {
    "tier": "graph-rewrite",
    "status": "PASS",
    "check_count": 5,
    "pass_rows": 5,
    "fail_rows": 0,
    "universe_rows": 5
  },
  {
    "tier": "inference-compiler",
    "status": "PASS",
    "check_count": 6,
    "pass_rows": 23,
    "fail_rows": 0,
    "universe_rows": 23
  },
  {
    "tier": "integer-arithmetic",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "integer-homological-algebra",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 15,
    "fail_rows": 0,
    "universe_rows": 15
  },
  {
    "tier": "integer-lattice",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 8,
    "fail_rows": 0,
    "universe_rows": 8
  },
  {
    "tier": "integer-linear-algebra",
    "status": "PASS",
    "check_count": 3,
    "pass_rows": 13,
    "fail_rows": 0,
    "universe_rows": 13
  },
  {
    "tier": "integral-hecke",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "integral-linear-algebra",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 9,
    "fail_rows": 0,
    "universe_rows": 9
  },
  {
    "tier": "integral-q-expansion",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "integral-reduction",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "level-lowering-contract",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "local-arithmetic",
    "status": "PASS",
    "check_count": 7,
    "pass_rows": 125,
    "fail_rows": 0,
    "universe_rows": 125
  },
  {
    "tier": "local-kummer-criterion",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 15,
    "fail_rows": 0,
    "universe_rows": 15
  },
  {
    "tier": "local-representation-schema",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 6,
    "fail_rows": 0,
    "universe_rows": 6
  },
  {
    "tier": "logic-repair",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "logical-kernel",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "methodology",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "module-rank-transfer",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "parent-edge-replacement",
    "status": "PASS",
    "check_count": 6,
    "pass_rows": 7,
    "fail_rows": 0,
    "universe_rows": 7
  },
  {
    "tier": "patching-descent",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "proof-by-cases",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "rational-linear-algebra",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "sourced-classification",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "sourced-import",
    "status": "IMPORTED",
    "check_count": 47,
    "pass_rows": 0,
    "fail_rows": 0,
    "universe_rows": 0
  },
  {
    "tier": "symbolic-algebra",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 3,
    "fail_rows": 0,
    "universe_rows": 3
  },
  {
    "tier": "symbolic-qexp",
    "status": "PASS",
    "check_count": 2,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "theta-modularity",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  },
  {
    "tier": "theta-series",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 101,
    "fail_rows": 0,
    "universe_rows": 101
  },
  {
    "tier": "thin-set-schema",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 2,
    "fail_rows": 0,
    "universe_rows": 2
  },
  {
    "tier": "toy-patching",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 4,
    "fail_rows": 0,
    "universe_rows": 4
  },
  {
    "tier": "two-isogeny-descent",
    "status": "PASS",
    "check_count": 1,
    "pass_rows": 1,
    "fail_rows": 0,
    "universe_rows": 1
  }
]
```
