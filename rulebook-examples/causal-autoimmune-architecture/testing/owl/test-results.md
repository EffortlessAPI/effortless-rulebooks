# Test Results: owl

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 4731 |
| Passed | 3310 |
| Failed | 1421 |
| Score | 70.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 2651 | 3771 | 70.3% |
| Lookup (INDEX/MATCH) | 448 | 615 | 72.8% |
| Aggregation (COUNTIFS/SUMIFS) | 211 | 345 | 61.2% |

## Results by Entity

### autoimmune_diseases

- Fields: 12/12 (100.0%)
- Computed columns: name, relative_path, count_of_disease_stages, count_of_intervention_targets

### disease_stages

- Fields: 18/30 (60.0%)
- Computed columns: name, parent_path, relative_path, autoimmune_disease_disease_label, is_presymptomatic

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| psa-active | name | Psoriatic Arthritis — Active |  — Active |
| psa-active | autoimmune_disease_disease_label | Psoriatic Arthritis | None |
| ra-active | name | Rheumatoid Arthritis — Active |  — Active |
| ra-active | autoimmune_disease_disease_label | Rheumatoid Arthritis | None |
| ra-remission | name | Rheumatoid Arthritis — Remissi |  — Remission |
| ra-remission | autoimmune_disease_disease_label | Rheumatoid Arthritis | None |
| sle-active | name | Systemic Lupus Erythematosus — |  — Active |
| sle-active | autoimmune_disease_disease_label | Systemic Lupus Erythematosus | None |
| sle-presymptomatic | name | Systemic Lupus Erythematosus — |  — Presymptomatic |
| sle-presymptomatic | autoimmune_disease_disease_label | Systemic Lupus Erythematosus | None |
| sle-remission | name | Systemic Lupus Erythematosus — |  — Remission |
| sle-remission | autoimmune_disease_disease_label | Systemic Lupus Erythematosus | None |

### tissues

- Fields: 15/15 (100.0%)
- Computed columns: name, relative_path, count_of_omics_assays

### omics_modalities

- Fields: 24/24 (100.0%)
- Computed columns: name, relative_path, count_of_omics_assays

### federated_datasets

- Fields: 12/12 (100.0%)
- Computed columns: name, relative_path, count_of_individuals

### variant_types

- Fields: 15/15 (100.0%)
- Computed columns: name, relative_path, count_of_genomic_variants

### individuals

- Fields: 273/396 (68.9%)
- Computed columns: name, slug, relative_path, federated_dataset_node_label, count_of_genomic_variants, count_of_causal_mechanisms, count_of_epistatic_interactions, rare_variant_burden_score, causal_architecture_score, is_development_window, is_aging_window, count_confirmed_causal_nodes, sum_confirmed_causal_confidence, count_cross_ancestry_confirmed_nodes, max_severity_score, count_high_severity_phenotypes, has_high_severity_phenotype, count_predicted_treatment_responses, has_predicted_treatment_response, count_serology_panels, count_pre_nephritic_signature_panels, is_in_pre_nephritic_signature_cluster, signature_strength, max_progression_state_order, latest_sledai_score, nephritis_progression_state_key, activity_tier, is_high_disease_activity, is_disease_progressing, target_pathway_code, target_pathway, current_progression_state_id, reachable_states_ahead

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ind-a-reyes | federated_dataset_node_label | European Reference Node | None |
| ind-a-reyes | count_confirmed_causal_nodes | 1 | 0 |
| ind-a-reyes | sum_confirmed_causal_confidence | 0.7583333333333333 | 0 |
| ind-a-reyes | count_cross_ancestry_confirmed_nodes | 1 | 0 |
| ind-a-reyes | count_high_severity_phenotypes | 1 | 0 |
| ind-a-reyes | has_high_severity_phenotype | True | False |
| ind-a-reyes | count_predicted_treatment_responses | 1 | 0 |
| ind-a-reyes | has_predicted_treatment_response | True | False |
| ind-a-reyes | count_pre_nephritic_signature_panels | 1 | 0 |
| ind-a-reyes | is_in_pre_nephritic_signature_cluster | True | False |
| ind-a-reyes | signature_strength | 1 | 0 |
| ind-a-reyes | max_progression_state_order | 2 | 1 |
| ind-a-reyes | latest_sledai_score | 4 | 0 |
| ind-a-reyes | nephritis_progression_state_key | SerologicActive | PresymptomaticAutoimmunity |
| ind-a-reyes | activity_tier | Mild | Quiescent |
| ind-a-reyes | current_progression_state_id | lupus-nephritis-progression--s | lupus-nephritis-progression--p |
| ind-a-reyes | reachable_states_ahead | 4 | None |
| ind-b-okafor | federated_dataset_node_label | European Reference Node | None |
| ind-b-okafor | count_confirmed_causal_nodes | 1 | 0 |
| ind-b-okafor | sum_confirmed_causal_confidence | 0.7583333333333333 | 0 |
| ... | ... | (103 more) | ... |

### genomic_variants

- Fields: 60/96 (62.5%)
- Computed columns: name, parent_path, relative_path, variant_type_label, variant_class_is_rare, individual_ancestry_label, is_rare_variant, is_causal_candidate

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| var-a-irf5 | variant_type_label | Regulatory Variant | None |
| var-a-irf5 | individual_ancestry_label | European | None |
| var-a-irf5 | is_rare_variant | True | False |
| var-b-irf5 | variant_type_label | Regulatory Variant | None |
| var-b-irf5 | individual_ancestry_label | European | None |
| var-b-irf5 | is_rare_variant | True | False |
| var-c-stat4 | variant_type_label | Regulatory Variant | None |
| var-c-stat4 | individual_ancestry_label | East Asian | None |
| var-c-stat4 | is_rare_variant | True | False |
| var-d-irf5 | variant_type_label | Regulatory Variant | None |
| var-d-irf5 | individual_ancestry_label | European | None |
| var-d-irf5 | is_rare_variant | True | False |
| var-e-ctla4 | variant_type_label | Regulatory Variant | None |
| var-e-ctla4 | individual_ancestry_label | African | None |
| var-e-ctla4 | is_rare_variant | True | False |
| var-f-il23r | variant_type_label | Regulatory Variant | None |
| var-f-il23r | individual_ancestry_label | Indigenous American | None |
| var-f-il23r | is_rare_variant | True | False |
| var-g-il23r | variant_type_label | Regulatory Variant | None |
| var-g-il23r | individual_ancestry_label | Indigenous American | None |
| ... | ... | (16 more) | ... |

### omics_assays

- Fields: 144/252 (57.1%)
- Computed columns: name, parent_path, relative_path, modality_label, tissue_label, has_batch_effect_risk, is_high_quality_assay

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assay-a-atac | modality_label | ATAC-seq | None |
| assay-a-atac | tissue_label | Blood | None |
| assay-a-atac | is_high_quality_assay | True | False |
| assay-a-prot | modality_label | Proteomics | None |
| assay-a-prot | tissue_label | Blood | None |
| assay-a-prot | is_high_quality_assay | True | False |
| assay-a-rna | modality_label | RNA-seq | None |
| assay-a-rna | tissue_label | Blood | None |
| assay-a-rna | is_high_quality_assay | True | False |
| assay-b-atac | modality_label | ATAC-seq | None |
| assay-b-atac | tissue_label | Blood | None |
| assay-b-atac | is_high_quality_assay | True | False |
| assay-b-prot | modality_label | Proteomics | None |
| assay-b-prot | tissue_label | Blood | None |
| assay-b-prot | is_high_quality_assay | True | False |
| assay-b-rna | modality_label | RNA-seq | None |
| assay-b-rna | tissue_label | Blood | None |
| assay-b-rna | is_high_quality_assay | True | False |
| assay-c-atac | modality_label | ATAC-seq | None |
| assay-c-atac | tissue_label | Blood | None |
| ... | ... | (88 more) | ... |

### evidence_items

- Fields: 144/252 (57.1%)
- Computed columns: name, parent_path, relative_path, assay_is_high_quality, z_stat, is_confound_controlled, is_qualified_evidence

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ev-a-1 | assay_is_high_quality | True | False |
| ev-a-1 | z_stat | 4.55 | 0 |
| ev-a-1 | is_qualified_evidence | True | False |
| ev-a-2 | assay_is_high_quality | True | False |
| ev-a-2 | z_stat | 3.8421052631578947 | 0 |
| ev-a-2 | is_qualified_evidence | True | False |
| ev-a-3 | assay_is_high_quality | True | False |
| ev-a-3 | z_stat | 3.2195121951219514 | 0 |
| ev-a-3 | is_qualified_evidence | True | False |
| ev-b-1 | assay_is_high_quality | True | False |
| ev-b-1 | z_stat | 4 | 0 |
| ev-b-1 | is_qualified_evidence | True | False |
| ev-b-2 | assay_is_high_quality | True | False |
| ev-b-2 | z_stat | 3.8536585365853657 | 0 |
| ev-b-2 | is_qualified_evidence | True | False |
| ev-b-3 | assay_is_high_quality | True | False |
| ev-b-3 | z_stat | 2.8292682926829267 | 0 |
| ev-b-3 | is_qualified_evidence | True | False |
| ev-c-1 | assay_is_high_quality | True | False |
| ev-c-1 | z_stat | 4.533333333333333 | 0 |
| ... | ... | (88 more) | ... |

### cohort_replications

- Fields: 158/252 (62.7%)
- Computed columns: name, parent_path, relative_path, replicated_at_nominal_sig, mechanism_primary_ancestry, is_different_ancestry_replication, is_cross_ancestry_concordant

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rep-a-1 | replicated_at_nominal_sig | True | False |
| rep-a-1 | mechanism_primary_ancestry | European | None |
| rep-a-1 | is_cross_ancestry_concordant | True | False |
| rep-a-2 | replicated_at_nominal_sig | True | False |
| rep-a-2 | mechanism_primary_ancestry | European | None |
| rep-a-2 | is_cross_ancestry_concordant | True | False |
| rep-a-3 | mechanism_primary_ancestry | European | None |
| rep-a-3 | is_different_ancestry_replication | False | True |
| rep-b-1 | replicated_at_nominal_sig | True | False |
| rep-b-1 | mechanism_primary_ancestry | European | None |
| rep-b-1 | is_cross_ancestry_concordant | True | False |
| rep-b-2 | replicated_at_nominal_sig | True | False |
| rep-b-2 | mechanism_primary_ancestry | European | None |
| rep-b-2 | is_cross_ancestry_concordant | True | False |
| rep-b-3 | mechanism_primary_ancestry | European | None |
| rep-b-3 | is_different_ancestry_replication | False | True |
| rep-c-1 | replicated_at_nominal_sig | True | False |
| rep-c-1 | mechanism_primary_ancestry | East Asian | None |
| rep-c-1 | is_different_ancestry_replication | False | True |
| rep-c-2 | mechanism_primary_ancestry | East Asian | None |
| ... | ... | (74 more) | ... |

### negative_control_tests

- Fields: 96/96 (100.0%)
- Computed columns: name, parent_path, relative_path, is_survived

### environmental_exposures

- Fields: 4/5 (80.0%)
- Computed columns: name, parent_path, relative_path, individual_ancestry_label, is_high_exposure

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exp-context | individual_ancestry_label | European | None |

### treatments

- Fields: 60/96 (62.5%)
- Computed columns: name, parent_path, relative_path, autoimmune_disease_label, is_effective_treatment, is_mechanism_matched, is_treatment_response_predicted, treatment_response_deciding_factor

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tx-a | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-a | is_mechanism_matched | True | None |
| tx-a | is_treatment_response_predicted | True | False |
| tx-a | treatment_response_deciding_factor | EffectiveOnConfirmedMechanism | Undetermined |
| tx-b | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-b | is_mechanism_matched | True | None |
| tx-b | is_treatment_response_predicted | True | False |
| tx-b | treatment_response_deciding_factor | EffectiveOnConfirmedMechanism | Undetermined |
| tx-c | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-c | treatment_response_deciding_factor | NoConfirmedMechanism | Undetermined |
| tx-d | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-d | is_mechanism_matched | True | None |
| tx-e | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-e | treatment_response_deciding_factor | NoConfirmedMechanism | NoResponse |
| tx-f | autoimmune_disease_label | Psoriatic Arthritis | None |
| tx-f | is_mechanism_matched | True | None |
| tx-g | autoimmune_disease_label | Psoriatic Arthritis | None |
| tx-g | is_mechanism_matched | True | None |
| tx-g | is_treatment_response_predicted | True | False |
| tx-g | treatment_response_deciding_factor | EffectiveOnConfirmedMechanism | Undetermined |
| ... | ... | (16 more) | ... |

### clinical_phenotypes

- Fields: 53/72 (73.6%)
- Computed columns: name, parent_path, relative_path, disease_stage_label, is_high_severity, is_presymptomatic_phenotype

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ph-sev-a | disease_stage_label | Active | None |
| ph-sev-a | is_high_severity | True | False |
| ph-sev-b | disease_stage_label | Active | None |
| ph-sev-b | is_high_severity | True | False |
| ph-sev-c | disease_stage_label | Active | None |
| ph-sev-c | is_high_severity | True | False |
| ph-sev-d | disease_stage_label | Active | None |
| ph-sev-d | is_high_severity | True | False |
| ph-sev-e | disease_stage_label | Active | None |
| ph-sev-f | disease_stage_label | Active | None |
| ph-sev-g | disease_stage_label | Active | None |
| ph-sev-h | disease_stage_label | Active | None |
| ph-sev-h | is_high_severity | True | False |
| ph-sev-i | disease_stage_label | Active | None |
| ph-sev-i | is_high_severity | True | False |
| ph-sev-j | disease_stage_label | Active | None |
| ph-sev-j | is_high_severity | True | False |
| ph-sev-k | disease_stage_label | Active | None |
| ph-sev-l | disease_stage_label | Active | None |

### causal_mechanisms

- Fields: 120/264 (45.5%)
- Computed columns: target_pathway_code, name, parent_path, relative_path, individual_ancestry_label, count_qualified_evidence, count_modalities_supporting, count_intervention_targets, is_experimentally_falsifiable, count_replications, count_concordant_replications, count_cross_ancestry_concordant, replication_fraction, replicates_across_cohorts, count_neg_control_tests, count_neg_control_survived, survives_negative_controls, is_spurious_derived, causal_confidence, variant_is_causal_candidate, is_causal_architecture_node, is_ancestry_transportable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cm-a | individual_ancestry_label | European | None |
| cm-a | count_qualified_evidence | 3 | 0 |
| cm-a | count_modalities_supporting | 2 | 0 |
| cm-a | is_experimentally_falsifiable | True | False |
| cm-a | count_concordant_replications | 2 | 0 |
| cm-a | count_cross_ancestry_concordant | 2 | 0 |
| cm-a | replication_fraction | 0.6666666666666666 | 0 |
| cm-a | replicates_across_cohorts | True | False |
| cm-a | is_spurious_derived | False | True |
| cm-a | causal_confidence | 0.7583333333333333 | 0.2 |
| cm-a | variant_is_causal_candidate | True | None |
| cm-a | is_causal_architecture_node | True | False |
| cm-a | is_ancestry_transportable | True | False |
| cm-b | individual_ancestry_label | European | None |
| cm-b | count_qualified_evidence | 3 | 0 |
| cm-b | count_modalities_supporting | 2 | 0 |
| cm-b | is_experimentally_falsifiable | True | False |
| cm-b | count_concordant_replications | 2 | 0 |
| cm-b | count_cross_ancestry_concordant | 2 | 0 |
| cm-b | replication_fraction | 0.6666666666666666 | 0 |
| ... | ... | (124 more) | ... |

### epistatic_interactions

- Fields: 4/4 (100.0%)
- Computed columns: name, parent_path, relative_path, is_high_order_epistasis

### counterfactual_trajectories

- Fields: 4/5 (80.0%)
- Computed columns: name, parent_path, relative_path, autoimmune_disease_label, is_worsening_trajectory

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cf-context | autoimmune_disease_label | Systemic Lupus Erythematosus | None |

### individual_predictions

- Fields: 223/504 (44.2%)
- Computed columns: name, parent_path, relative_path, individual_ancestry_label, is_ancestry_holdout, individual_causal_mass, individual_confirmed_node_count, individual_cross_ancestry_node_count, individual_has_cryptic_relatedness, individual_max_severity_score, individual_has_high_severity_phenotype, individual_has_predicted_treatment_response, predicted_value, count_bins, count_well_calibrated_bins, sum_bin_abs_error, mean_bin_abs_error, well_calibrated_fraction, calibrated_uncertainty, rests_on_confirmed_mechanism, has_spurious_correlation_flag, is_falsifiability_backed, is_transportable_to_absent_ancestry, is_ancestry_transport_safe, transport_gate_status, is_high_confidence_prediction, patient_stratification_tier, predicted_severity_value, severity_tier, is_severity_actionable, severity_deciding_factor, is_treatment_response_actionable, treatment_response_deciding_factor, is_clinically_actionable, lifecycle_state_key, deciding_gate, individual_target_pathway, individual_progression_state_key, individual_is_disease_progressing, recommended_treatment_line, treatment_line_deciding_factor, progression_vs_actionability_disagree

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pred-a | individual_ancestry_label | European | None |
| pred-a | individual_causal_mass | 0.7583333333333333 | None |
| pred-a | individual_confirmed_node_count | 1 | None |
| pred-a | individual_cross_ancestry_node_count | 1 | None |
| pred-a | individual_max_severity_score | 9 | None |
| pred-a | individual_has_high_severity_phenotype | True | None |
| pred-a | individual_has_predicted_treatment_response | True | None |
| pred-a | predicted_value | 3.0166666666666666 | None |
| pred-a | count_well_calibrated_bins | 5 | 0 |
| pred-a | sum_bin_abs_error | 0.06 | 0 |
| pred-a | mean_bin_abs_error | 0.012 | 0 |
| pred-a | well_calibrated_fraction | 1 | 0 |
| pred-a | calibrated_uncertainty | 0.988 | 0 |
| pred-a | rests_on_confirmed_mechanism | True | False |
| pred-a | has_spurious_correlation_flag | False | True |
| pred-a | is_falsifiability_backed | True | False |
| pred-a | transport_gate_status | NotApplicable | FAIL |
| pred-a | is_high_confidence_prediction | True | False |
| pred-a | predicted_severity_value | 9 | None |
| pred-a | severity_tier | Severe | Mild |
| ... | ... | (261 more) | ... |

### calibration_bins

- Fields: 198/300 (66.0%)
- Computed columns: name, parent_path, relative_path, bin_abs_error, is_well_calibrated_bin

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cb-a-0 | bin_abs_error | 0.01 | None |
| cb-a-0 | is_well_calibrated_bin | True | False |
| cb-a-1 | bin_abs_error | 0.01 | None |
| cb-a-1 | is_well_calibrated_bin | True | False |
| cb-a-2 | bin_abs_error | 0.01 | None |
| cb-a-2 | is_well_calibrated_bin | True | False |
| cb-a-3 | bin_abs_error | 0.01 | None |
| cb-a-3 | is_well_calibrated_bin | True | False |
| cb-a-4 | bin_abs_error | 0.02 | None |
| cb-a-4 | is_well_calibrated_bin | True | False |
| cb-b-0 | bin_abs_error | 0.03 | None |
| cb-b-1 | bin_abs_error | 0.03 | None |
| cb-b-2 | bin_abs_error | 0.05 | None |
| cb-b-3 | bin_abs_error | 0.04 | None |
| cb-b-4 | bin_abs_error | 0.03 | None |
| cb-c-0 | bin_abs_error | 0.01 | None |
| cb-c-0 | is_well_calibrated_bin | True | False |
| cb-c-1 | bin_abs_error | 0.01 | None |
| cb-c-1 | is_well_calibrated_bin | True | False |
| cb-c-2 | bin_abs_error | 0.03 | None |
| ... | ... | (82 more) | ... |

### intervention_targets

- Fields: 55/66 (83.3%)
- Computed columns: name, parent_path, relative_path, causal_mechanism_label, is_gene_based_therapy, is_cell_based_therapy

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| it-a | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-b | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-c | causal_mechanism_label | STAT4 regulatory -> Th1/IFN -> | None |
| it-d | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-f | causal_mechanism_label | IL23R regulatory -> IL-17 axis | None |
| it-g | causal_mechanism_label | IL23R regulatory -> IL-17 axis | None |
| it-h | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-i | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-j | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-k | causal_mechanism_label | IL23R regulatory -> IL-17 axis | None |
| it-l | causal_mechanism_label | IL23R regulatory -> IL-17 axis | None |

### axioms

- Fields: 30/30 (100.0%)
- Computed columns: name, relative_path

### tests_for_success

- Fields: 20/20 (100.0%)
- Computed columns: name, relative_path

### features

- Fields: 72/72 (100.0%)
- Computed columns: name, relative_path, meta_line

### inference_kinds

- Fields: 14/14 (100.0%)
- Computed columns: name, relative_path

### open_questions

- Fields: 8/8 (100.0%)
- Computed columns: name, relative_path

### non_goals

- Fields: 10/10 (100.0%)
- Computed columns: name, relative_path

### glossary_terms

- Fields: 26/26 (100.0%)
- Computed columns: name, relative_path

### effortless_loops

- Fields: 48/64 (75.0%)
- Computed columns: name, relative_path, completedness, is_in_current_plan

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| loop-0 | name | Loop 0 — Solve-by-inference ru | None |
| loop-0-5 | name | Loop 0.5 — Test Harness First  | None |
| loop-1 | name | Loop 1 — Intake app skeleton | None |
| loop-2 | name | Loop 2 — Patient intake (facts | None |
| loop-3 | name | Loop 3 — 3-panel witness — pro | None |
| loop-4 | name | Loop 4 — Second prediction typ | None |
| loop-5 | name | Loop 5 — Treatment-response pr | None |
| loop-cohort-discovery | name | Loop 9 — Cohort discovery boar | None |
| loop-disease-state | name | Loop 7 — Disease-state simulat | None |
| loop-equity | name | Loop 14 — Cohort-level equity  | None |
| loop-llm-intake | name | Loop 13 — LLM intake clerk + s | None |
| loop-progression-admin | name | Loop 10 — Progression machine  | None |
| loop-prune-nonloadbearing | name | Loop 12 — Prune excess enginee | None |
| loop-serology-discovery | name | Loop 11 — Serology-signature d | None |
| loop-treatment-line | name | Loop 8 — Treatment-line recomm | None |
| loop-vocab-completeness | name | Loop 6 — Vocabulary completene | None |

### routing_and_navigation

- Fields: 342/504 (67.9%)
- Computed columns: name, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, intake_clinician_can_create, intake_clinician_can_read, intake_clinician_can_update, intake_clinician_can_delete, diagnosing_doctor_can_create, diagnosing_doctor_can_read, diagnosing_doctor_can_update, diagnosing_doctor_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete, depth, full_path, handler_base_name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| nav-admin | admin_can_create | True | None |
| nav-admin | admin_can_read | True | None |
| nav-admin | admin_can_update | True | None |
| nav-admin | admin_can_delete | True | None |
| nav-admin | handler_base_name | admin |       |
| nav-admin-cohort | admin_can_create | True | None |
| nav-admin-cohort | admin_can_read | True | None |
| nav-admin-cohort | admin_can_update | True | None |
| nav-admin-cohort | admin_can_delete | True | None |
| nav-admin-cohort | handler_base_name | admin cohort |              |
| nav-admin-explainer | admin_can_create | True | None |
| nav-admin-explainer | admin_can_read | True | None |
| nav-admin-explainer | admin_can_update | True | None |
| nav-admin-explainer | admin_can_delete | True | None |
| nav-admin-explainer | depth | 1 | None |
| nav-admin-explainer | handler_base_name | admin explainer |                 |
| nav-admin-harness | admin_can_create | True | None |
| nav-admin-harness | admin_can_read | True | None |
| nav-admin-harness | admin_can_update | True | None |
| nav-admin-harness | admin_can_delete | True | None |
| ... | ... | (142 more) | ... |

### state_machines

- Fields: 8/8 (100.0%)
- Computed columns: name, relative_path, state_count, transition_rule_count

### machine_states

- Fields: 30/39 (76.9%)
- Computed columns: name, relative_path, reachable_state_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| diagnosis-lifecycle--calibrationchecked | reachable_state_count | 3 | None |
| diagnosis-lifecycle--evidenceassessed | reachable_state_count | 5 | None |
| diagnosis-lifecycle--intake | reachable_state_count | 6 | None |
| diagnosis-lifecycle--mechanismconfirmed | reachable_state_count | 4 | None |
| diagnosis-lifecycle--transportchecked | reachable_state_count | 2 | None |
| lupus-nephritis-progression--earlynephritis | reachable_state_count | 2 | None |
| lupus-nephritis-progression--presymptomaticautoimmunity | reachable_state_count | 5 | None |
| lupus-nephritis-progression--renalflarerisk | reachable_state_count | 1 | None |
| lupus-nephritis-progression--serologicactive | reachable_state_count | 4 | None |

### state_transition_rules

- Fields: 70/70 (100.0%)
- Computed columns: name, relative_path, from_state_key, to_state_key, is_forward_edge

### state_transitions

- Fields: 216/216 (100.0%)
- Computed columns: name, relative_path, is_forward

### subject_state_instances

- Fields: 297/395 (75.2%)
- Computed columns: name, relative_path, is_current, has_complete_lineage, is_long_dwell

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ssi-ind-a-reyes-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-a-reyes-presymptomaticautoimmunity-1 | is_long_dwell | True | False |
| ssi-ind-a-reyes-serologicactive-2 | has_complete_lineage | True | None |
| ssi-ind-a-reyes-serologicactive-2 | is_long_dwell | True | False |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | is_long_dwell | True | False |
| ssi-ind-c-chen-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-c-chen-presymptomaticautoimmunity-1 | is_long_dwell | True | False |
| ssi-ind-c-chen-serologicactive-2 | has_complete_lineage | True | None |
| ssi-ind-c-chen-serologicactive-2 | is_long_dwell | True | False |
| ssi-ind-d-santos-biopsyindicated-3 | has_complete_lineage | True | None |
| ssi-ind-d-santos-biopsyindicated-3 | is_long_dwell | True | False |
| ssi-ind-d-santos-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-d-santos-presymptomaticautoimmunity-1 | is_long_dwell | True | False |
| ssi-ind-d-santos-renalflarerisk-2 | has_complete_lineage | True | None |
| ssi-ind-d-santos-renalflarerisk-2 | is_long_dwell | True | False |
| ssi-ind-e-mensah-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-e-mensah-presymptomaticautoimmunity-1 | is_long_dwell | True | False |
| ssi-ind-f-haidar-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-f-haidar-presymptomaticautoimmunity-1 | is_long_dwell | True | False |
| ... | ... | (78 more) | ... |

### disease_domain_concepts

- Fields: 104/104 (100.0%)
- Computed columns: name, relative_path, is_deeply_modeled, is_schema_modeled

### serology_observations

- Fields: 315/375 (84.0%)
- Computed columns: prior_anti_ds_dna_iu, prior_c3, prior_c4, anti_ds_dna_trend, complement_trend, is_pre_nephritic_signature_panel, is_significant_proteinuria, is_nephrotic_range_proteinuria, sledai_renal_points, sledai_serology_points, sledai_score, progression_state_key, progression_state_order, name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sero-ind-a-reyes-2 | anti_ds_dna_trend | Rising | Stable |
| sero-ind-a-reyes-2 | complement_trend | Falling | Stable |
| sero-ind-a-reyes-2 | is_pre_nephritic_signature_panel | True | False |
| sero-ind-a-reyes-2 | sledai_serology_points | 4 | 0 |
| sero-ind-a-reyes-2 | sledai_score | 4 | 0 |
| sero-ind-a-reyes-2 | progression_state_key | SerologicActive | PresymptomaticAutoimmunity |
| sero-ind-a-reyes-2 | progression_state_order | 2 | 1 |
| sero-ind-c-chen-2 | anti_ds_dna_trend | Rising | Stable |
| sero-ind-c-chen-2 | complement_trend | Falling | Stable |
| sero-ind-c-chen-2 | is_pre_nephritic_signature_panel | True | False |
| sero-ind-c-chen-2 | sledai_serology_points | 4 | 0 |
| sero-ind-c-chen-2 | sledai_score | 4 | 0 |
| sero-ind-c-chen-2 | progression_state_key | SerologicActive | PresymptomaticAutoimmunity |
| sero-ind-c-chen-2 | progression_state_order | 2 | 1 |
| sero-ind-d-santos-2 | anti_ds_dna_trend | Rising | Stable |
| sero-ind-d-santos-2 | complement_trend | Falling | Stable |
| sero-ind-d-santos-2 | is_pre_nephritic_signature_panel | True | False |
| sero-ind-d-santos-2 | is_significant_proteinuria | True | False |
| sero-ind-d-santos-2 | sledai_renal_points | 4 | 0 |
| sero-ind-d-santos-2 | sledai_serology_points | 4 | 0 |
| ... | ... | (40 more) | ... |

### therapy_options

- Fields: 8/8 (100.0%)
- Computed columns: name, relative_path
