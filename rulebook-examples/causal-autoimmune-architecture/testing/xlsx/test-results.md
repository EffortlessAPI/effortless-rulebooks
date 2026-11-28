# Test Results: xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 4731 |
| Passed | 1008 |
| Failed | 3723 |
| Score | 21.3% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 865 | 3771 | 22.9% |
| Lookup (INDEX/MATCH) | 77 | 615 | 12.5% |
| Aggregation (COUNTIFS/SUMIFS) | 66 | 345 | 19.1% |

## Results by Entity

### autoimmune_diseases

- Fields: 1/12 (8.3%)
- Computed columns: name, relative_path, count_of_disease_stages, count_of_intervention_targets

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| psa | name | Psoriatic Arthritis | None |
| psa | relative_path | /diseases/psa | None |
| psa | count_of_disease_stages | 1 | None |
| psa | count_of_intervention_targets | 4 | None |
| ra | name | Rheumatoid Arthritis | None |
| ra | relative_path | /diseases/ra | None |
| ra | count_of_disease_stages | 2 | None |
| sle | name | Systemic Lupus Erythematosus | None |
| sle | relative_path | /diseases/sle | None |
| sle | count_of_disease_stages | 3 | None |
| sle | count_of_intervention_targets | 7 | None |

### disease_stages

- Fields: 5/30 (16.7%)
- Computed columns: name, parent_path, relative_path, autoimmune_disease_disease_label, is_presymptomatic

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| psa-active | name | Psoriatic Arthritis — Active | None |
| psa-active | parent_path | /diseases/psa | None |
| psa-active | relative_path | /diseases/psa/stages/psa-activ | None |
| psa-active | autoimmune_disease_disease_label | Psoriatic Arthritis | None |
| ra-active | name | Rheumatoid Arthritis — Active | None |
| ra-active | parent_path | /diseases/ra | None |
| ra-active | relative_path | /diseases/ra/stages/ra-active | None |
| ra-active | autoimmune_disease_disease_label | Rheumatoid Arthritis | None |
| ra-remission | name | Rheumatoid Arthritis — Remissi | None |
| ra-remission | parent_path | /diseases/ra | None |
| ra-remission | relative_path | /diseases/ra/stages/ra-remissi | None |
| ra-remission | autoimmune_disease_disease_label | Rheumatoid Arthritis | None |
| sle-active | name | Systemic Lupus Erythematosus — | None |
| sle-active | parent_path | /diseases/sle | None |
| sle-active | relative_path | /diseases/sle/stages/sle-activ | None |
| sle-active | autoimmune_disease_disease_label | Systemic Lupus Erythematosus | None |
| sle-presymptomatic | name | Systemic Lupus Erythematosus — | None |
| sle-presymptomatic | parent_path | /diseases/sle | None |
| sle-presymptomatic | relative_path | /diseases/sle/stages/sle-presy | None |
| sle-presymptomatic | autoimmune_disease_disease_label | Systemic Lupus Erythematosus | None |
| ... | ... | (5 more) | ... |

### tissues

- Fields: 4/15 (26.7%)
- Computed columns: name, relative_path, count_of_omics_assays

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| blood | name | Blood | None |
| blood | relative_path | /tissues/blood | None |
| blood | count_of_omics_assays | 36 | None |
| gut | name | Gut | None |
| gut | relative_path | /tissues/gut | None |
| kidney | name | Kidney | None |
| kidney | relative_path | /tissues/kidney | None |
| skin | name | Skin | None |
| skin | relative_path | /tissues/skin | None |
| synovium | name | Synovium | None |
| synovium | relative_path | /tissues/synovium | None |

### omics_modalities

- Fields: 5/24 (20.8%)
- Computed columns: name, relative_path, count_of_omics_assays

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| atac-seq | name | ATAC-seq | None |
| atac-seq | relative_path | /omics-modalities/atac-seq | None |
| atac-seq | count_of_omics_assays | 12 | None |
| hi-c | name | Chromatin Conformation Hi-C (s | None |
| hi-c | relative_path | /omics-modalities/hi-c | None |
| long-read-genomes | name | Phased Telomere-to-Telomere Lo | None |
| long-read-genomes | relative_path | /omics-modalities/long-read-ge | None |
| methylome | name | Methylome (sparse context) | None |
| methylome | relative_path | /omics-modalities/methylome | None |
| microbiome | name | Microbiome | None |
| microbiome | relative_path | /omics-modalities/microbiome | None |
| proteomics | name | Proteomics | None |
| proteomics | relative_path | /omics-modalities/proteomics | None |
| proteomics | count_of_omics_assays | 12 | None |
| rna-seq | name | RNA-seq | None |
| rna-seq | relative_path | /omics-modalities/rna-seq | None |
| rna-seq | count_of_omics_assays | 12 | None |
| single-cell-rna | name | Single-Cell RNA-seq | None |
| single-cell-rna | relative_path | /omics-modalities/single-cell- | None |

### federated_datasets

- Fields: 0/12 (0.0%)
- Computed columns: name, relative_path, count_of_individuals

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| fed-amazon | name | Amazon Indigenous Cohort | None |
| fed-amazon | relative_path | /datasets/fed-amazon | None |
| fed-amazon | count_of_individuals | 4 | None |
| fed-east-asia | name | East Asia Federated Node | None |
| fed-east-asia | relative_path | /datasets/fed-east-asia | None |
| fed-east-asia | count_of_individuals | 1 | None |
| fed-europe | name | European Reference Node | None |
| fed-europe | relative_path | /datasets/fed-europe | None |
| fed-europe | count_of_individuals | 4 | None |
| fed-west-africa | name | West Africa Federated Node | None |
| fed-west-africa | relative_path | /datasets/fed-west-africa | None |
| fed-west-africa | count_of_individuals | 1 | None |

### variant_types

- Fields: 4/15 (26.7%)
- Computed columns: name, relative_path, count_of_genomic_variants

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| coding | name | Coding Variant | None |
| coding | relative_path | /variant-types/coding | None |
| de-novo | name | De Novo Mutation | None |
| de-novo | relative_path | /variant-types/de-novo | None |
| hla-haplotype | name | HLA Haplotype | None |
| hla-haplotype | relative_path | /variant-types/hla-haplotype | None |
| regulatory | name | Regulatory Variant | None |
| regulatory | relative_path | /variant-types/regulatory | None |
| regulatory | count_of_genomic_variants | 12 | None |
| structural | name | Structural Variant | None |
| structural | relative_path | /variant-types/structural | None |

### individuals

- Fields: 112/396 (28.3%)
- Computed columns: name, slug, relative_path, federated_dataset_node_label, count_of_genomic_variants, count_of_causal_mechanisms, count_of_epistatic_interactions, rare_variant_burden_score, causal_architecture_score, is_development_window, is_aging_window, count_confirmed_causal_nodes, sum_confirmed_causal_confidence, count_cross_ancestry_confirmed_nodes, max_severity_score, count_high_severity_phenotypes, has_high_severity_phenotype, count_predicted_treatment_responses, has_predicted_treatment_response, count_serology_panels, count_pre_nephritic_signature_panels, is_in_pre_nephritic_signature_cluster, signature_strength, max_progression_state_order, latest_sledai_score, nephritis_progression_state_key, activity_tier, is_high_disease_activity, is_disease_progressing, target_pathway_code, target_pathway, current_progression_state_id, reachable_states_ahead

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ind-a-reyes | name | Ana Reyes | None |
| ind-a-reyes | slug | reyes-ana | None |
| ind-a-reyes | relative_path | /intake/new-patient/reyes-ana | None |
| ind-a-reyes | federated_dataset_node_label | European Reference Node | None |
| ind-a-reyes | count_of_genomic_variants | 1 | None |
| ind-a-reyes | count_of_causal_mechanisms | 1 | None |
| ind-a-reyes | count_of_epistatic_interactions | 1 | None |
| ind-a-reyes | rare_variant_burden_score | 0.029411764705882353 | None |
| ind-a-reyes | causal_architecture_score | 15.029411764705882 | None |
| ind-a-reyes | count_confirmed_causal_nodes | 1 | None |
| ind-a-reyes | sum_confirmed_causal_confidence | 0.7583333333333333 | None |
| ind-a-reyes | count_cross_ancestry_confirmed_nodes | 1 | None |
| ind-a-reyes | max_severity_score | 9 | None |
| ind-a-reyes | count_high_severity_phenotypes | 1 | None |
| ind-a-reyes | has_high_severity_phenotype | True | None |
| ind-a-reyes | count_predicted_treatment_responses | 1 | None |
| ind-a-reyes | has_predicted_treatment_response | True | None |
| ind-a-reyes | count_serology_panels | 2 | None |
| ind-a-reyes | count_pre_nephritic_signature_panels | 1 | None |
| ind-a-reyes | is_in_pre_nephritic_signature_cluster | True | None |
| ... | ... | (264 more) | ... |

### genomic_variants

- Fields: 0/96 (0.0%)
- Computed columns: name, parent_path, relative_path, variant_type_label, variant_class_is_rare, individual_ancestry_label, is_rare_variant, is_causal_candidate

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| var-a-irf5 | name | IRF5 regulatory (rs2004640) | None |
| var-a-irf5 | parent_path | /intake/new-patient/reyes-ana | None |
| var-a-irf5 | relative_path | /intake/new-patient/reyes-ana/ | None |
| var-a-irf5 | variant_type_label | Regulatory Variant | None |
| var-a-irf5 | variant_class_is_rare | True | None |
| var-a-irf5 | individual_ancestry_label | European | None |
| var-a-irf5 | is_rare_variant | True | None |
| var-a-irf5 | is_causal_candidate | True | None |
| var-b-irf5 | name | IRF5 regulatory (rs2004640) | None |
| var-b-irf5 | parent_path | /intake/new-patient/okafor-bil | None |
| var-b-irf5 | relative_path | /intake/new-patient/okafor-bil | None |
| var-b-irf5 | variant_type_label | Regulatory Variant | None |
| var-b-irf5 | variant_class_is_rare | True | None |
| var-b-irf5 | individual_ancestry_label | European | None |
| var-b-irf5 | is_rare_variant | True | None |
| var-b-irf5 | is_causal_candidate | True | None |
| var-c-stat4 | name | STAT4 regulatory (rs7574865) | None |
| var-c-stat4 | parent_path | /intake/new-patient/wei-chen | None |
| var-c-stat4 | relative_path | /intake/new-patient/wei-chen/v | None |
| var-c-stat4 | variant_type_label | Regulatory Variant | None |
| ... | ... | (76 more) | ... |

### omics_assays

- Fields: 36/252 (14.3%)
- Computed columns: name, parent_path, relative_path, modality_label, tissue_label, has_batch_effect_risk, is_high_quality_assay

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assay-a-atac | name | assay-a-atac | None |
| assay-a-atac | parent_path | /intake/new-patient/reyes-ana | None |
| assay-a-atac | relative_path | /intake/new-patient/reyes-ana/ | None |
| assay-a-atac | modality_label | ATAC-seq | None |
| assay-a-atac | tissue_label | Blood | None |
| assay-a-atac | is_high_quality_assay | True | None |
| assay-a-prot | name | assay-a-prot | None |
| assay-a-prot | parent_path | /intake/new-patient/reyes-ana | None |
| assay-a-prot | relative_path | /intake/new-patient/reyes-ana/ | None |
| assay-a-prot | modality_label | Proteomics | None |
| assay-a-prot | tissue_label | Blood | None |
| assay-a-prot | is_high_quality_assay | True | None |
| assay-a-rna | name | assay-a-rna | None |
| assay-a-rna | parent_path | /intake/new-patient/reyes-ana | None |
| assay-a-rna | relative_path | /intake/new-patient/reyes-ana/ | None |
| assay-a-rna | modality_label | RNA-seq | None |
| assay-a-rna | tissue_label | Blood | None |
| assay-a-rna | is_high_quality_assay | True | None |
| assay-b-atac | name | assay-b-atac | None |
| assay-b-atac | parent_path | /intake/new-patient/okafor-bil | None |
| ... | ... | (196 more) | ... |

### evidence_items

- Fields: 0/252 (0.0%)
- Computed columns: name, parent_path, relative_path, assay_is_high_quality, z_stat, is_confound_controlled, is_qualified_evidence

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ev-a-1 | name | ev-a-1 | None |
| ev-a-1 | parent_path | /intake/new-patient/reyes-ana/ | None |
| ev-a-1 | relative_path | /intake/new-patient/reyes-ana/ | None |
| ev-a-1 | assay_is_high_quality | True | None |
| ev-a-1 | z_stat | 4.55 | None |
| ev-a-1 | is_confound_controlled | True | None |
| ev-a-1 | is_qualified_evidence | True | None |
| ev-a-2 | name | ev-a-2 | None |
| ev-a-2 | parent_path | /intake/new-patient/reyes-ana/ | None |
| ev-a-2 | relative_path | /intake/new-patient/reyes-ana/ | None |
| ev-a-2 | assay_is_high_quality | True | None |
| ev-a-2 | z_stat | 3.8421052631578947 | None |
| ev-a-2 | is_confound_controlled | True | None |
| ev-a-2 | is_qualified_evidence | True | None |
| ev-a-3 | name | ev-a-3 | None |
| ev-a-3 | parent_path | /intake/new-patient/reyes-ana/ | None |
| ev-a-3 | relative_path | /intake/new-patient/reyes-ana/ | None |
| ev-a-3 | assay_is_high_quality | True | None |
| ev-a-3 | z_stat | 3.2195121951219514 | None |
| ev-a-3 | is_confound_controlled | True | None |
| ... | ... | (232 more) | ... |

### cohort_replications

- Fields: 44/252 (17.5%)
- Computed columns: name, parent_path, relative_path, replicated_at_nominal_sig, mechanism_primary_ancestry, is_different_ancestry_replication, is_cross_ancestry_concordant

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rep-a-1 | name | rep-a-1 | None |
| rep-a-1 | parent_path | /intake/new-patient/reyes-ana/ | None |
| rep-a-1 | relative_path | /intake/new-patient/reyes-ana/ | None |
| rep-a-1 | replicated_at_nominal_sig | True | None |
| rep-a-1 | mechanism_primary_ancestry | European | None |
| rep-a-1 | is_different_ancestry_replication | True | None |
| rep-a-1 | is_cross_ancestry_concordant | True | None |
| rep-a-2 | name | rep-a-2 | None |
| rep-a-2 | parent_path | /intake/new-patient/reyes-ana/ | None |
| rep-a-2 | relative_path | /intake/new-patient/reyes-ana/ | None |
| rep-a-2 | replicated_at_nominal_sig | True | None |
| rep-a-2 | mechanism_primary_ancestry | European | None |
| rep-a-2 | is_different_ancestry_replication | True | None |
| rep-a-2 | is_cross_ancestry_concordant | True | None |
| rep-a-3 | name | rep-a-3 | None |
| rep-a-3 | parent_path | /intake/new-patient/reyes-ana/ | None |
| rep-a-3 | relative_path | /intake/new-patient/reyes-ana/ | None |
| rep-a-3 | mechanism_primary_ancestry | European | None |
| rep-b-1 | name | rep-b-1 | None |
| rep-b-1 | parent_path | /intake/new-patient/okafor-bil | None |
| ... | ... | (188 more) | ... |

### negative_control_tests

- Fields: 1/96 (1.0%)
- Computed columns: name, parent_path, relative_path, is_survived

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| nct-a-1 | name | nct-a-1 | None |
| nct-a-1 | parent_path | /intake/new-patient/reyes-ana/ | None |
| nct-a-1 | relative_path | /intake/new-patient/reyes-ana/ | None |
| nct-a-1 | is_survived | True | None |
| nct-a-2 | name | nct-a-2 | None |
| nct-a-2 | parent_path | /intake/new-patient/reyes-ana/ | None |
| nct-a-2 | relative_path | /intake/new-patient/reyes-ana/ | None |
| nct-a-2 | is_survived | True | None |
| nct-b-1 | name | nct-b-1 | None |
| nct-b-1 | parent_path | /intake/new-patient/okafor-bil | None |
| nct-b-1 | relative_path | /intake/new-patient/okafor-bil | None |
| nct-b-1 | is_survived | True | None |
| nct-b-2 | name | nct-b-2 | None |
| nct-b-2 | parent_path | /intake/new-patient/okafor-bil | None |
| nct-b-2 | relative_path | /intake/new-patient/okafor-bil | None |
| nct-b-2 | is_survived | True | None |
| nct-c-1 | name | nct-c-1 | None |
| nct-c-1 | parent_path | /intake/new-patient/wei-chen/m | None |
| nct-c-1 | relative_path | /intake/new-patient/wei-chen/m | None |
| nct-c-1 | is_survived | True | None |
| ... | ... | (75 more) | ... |

### environmental_exposures

- Fields: 1/5 (20.0%)
- Computed columns: name, parent_path, relative_path, individual_ancestry_label, is_high_exposure

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exp-context | name | Smoking pack-years (context on | None |
| exp-context | parent_path | /intake/new-patient/reyes-ana | None |
| exp-context | relative_path | /intake/new-patient/reyes-ana/ | None |
| exp-context | individual_ancestry_label | European | None |

### treatments

- Fields: 13/96 (13.5%)
- Computed columns: name, parent_path, relative_path, autoimmune_disease_label, is_effective_treatment, is_mechanism_matched, is_treatment_response_predicted, treatment_response_deciding_factor

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tx-a | name | Anifrolumab (anti-IFNAR1) | None |
| tx-a | parent_path | /intake/new-patient/reyes-ana | None |
| tx-a | relative_path | /intake/new-patient/reyes-ana/ | None |
| tx-a | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-a | is_effective_treatment | True | None |
| tx-a | is_mechanism_matched | True | None |
| tx-a | is_treatment_response_predicted | True | None |
| tx-a | treatment_response_deciding_factor | EffectiveOnConfirmedMechanism | None |
| tx-b | name | Anifrolumab (anti-IFNAR1) | None |
| tx-b | parent_path | /intake/new-patient/okafor-bil | None |
| tx-b | relative_path | /intake/new-patient/okafor-bil | None |
| tx-b | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| tx-b | is_effective_treatment | True | None |
| tx-b | is_mechanism_matched | True | None |
| tx-b | is_treatment_response_predicted | True | None |
| tx-b | treatment_response_deciding_factor | EffectiveOnConfirmedMechanism | None |
| tx-c | name | STAT4-pathway inhibitor (trial | None |
| tx-c | parent_path | /intake/new-patient/wei-chen | None |
| tx-c | relative_path | /intake/new-patient/wei-chen/t | None |
| tx-c | autoimmune_disease_label | Systemic Lupus Erythematosus | None |
| ... | ... | (63 more) | ... |

### clinical_phenotypes

- Fields: 17/72 (23.6%)
- Computed columns: name, parent_path, relative_path, disease_stage_label, is_high_severity, is_presymptomatic_phenotype

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ph-sev-a | name | Active SLE — high disease-acti | None |
| ph-sev-a | parent_path | /intake/new-patient/reyes-ana | None |
| ph-sev-a | relative_path | /intake/new-patient/reyes-ana/ | None |
| ph-sev-a | disease_stage_label | Active | None |
| ph-sev-a | is_high_severity | True | None |
| ph-sev-b | name | Active SLE — serositis + high  | None |
| ph-sev-b | parent_path | /intake/new-patient/okafor-bil | None |
| ph-sev-b | relative_path | /intake/new-patient/okafor-bil | None |
| ph-sev-b | disease_stage_label | Active | None |
| ph-sev-b | is_high_severity | True | None |
| ph-sev-c | name | Active SLE — arthritis + anti- | None |
| ph-sev-c | parent_path | /intake/new-patient/wei-chen | None |
| ph-sev-c | relative_path | /intake/new-patient/wei-chen/p | None |
| ph-sev-c | disease_stage_label | Active | None |
| ph-sev-c | is_high_severity | True | None |
| ph-sev-d | name | Active SLE — biopsy-confirmed  | None |
| ph-sev-d | parent_path | /intake/new-patient/santos-die | None |
| ph-sev-d | relative_path | /intake/new-patient/santos-die | None |
| ph-sev-d | disease_stage_label | Active | None |
| ph-sev-d | is_high_severity | True | None |
| ... | ... | (35 more) | ... |

### causal_mechanisms

- Fields: 24/264 (9.1%)
- Computed columns: target_pathway_code, name, parent_path, relative_path, individual_ancestry_label, count_qualified_evidence, count_modalities_supporting, count_intervention_targets, is_experimentally_falsifiable, count_replications, count_concordant_replications, count_cross_ancestry_concordant, replication_fraction, replicates_across_cohorts, count_neg_control_tests, count_neg_control_survived, survives_negative_controls, is_spurious_derived, causal_confidence, variant_is_causal_candidate, is_causal_architecture_node, is_ancestry_transportable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cm-a | target_pathway_code | 1 | None |
| cm-a | name | IRF5 regulatory -> type-I IFN  | None |
| cm-a | parent_path | /intake/new-patient/reyes-ana | None |
| cm-a | relative_path | /intake/new-patient/reyes-ana/ | None |
| cm-a | individual_ancestry_label | European | None |
| cm-a | count_qualified_evidence | 3 | None |
| cm-a | count_modalities_supporting | 2 | None |
| cm-a | count_intervention_targets | 1 | None |
| cm-a | is_experimentally_falsifiable | True | None |
| cm-a | count_replications | 3 | None |
| cm-a | count_concordant_replications | 2 | None |
| cm-a | count_cross_ancestry_concordant | 2 | None |
| cm-a | replication_fraction | 0.6666666666666666 | None |
| cm-a | replicates_across_cohorts | True | None |
| cm-a | count_neg_control_tests | 2 | None |
| cm-a | count_neg_control_survived | 2 | None |
| cm-a | survives_negative_controls | True | None |
| cm-a | causal_confidence | 0.7583333333333333 | None |
| cm-a | variant_is_causal_candidate | True | None |
| cm-a | is_causal_architecture_node | True | None |
| ... | ... | (220 more) | ... |

### epistatic_interactions

- Fields: 1/4 (25.0%)
- Computed columns: name, parent_path, relative_path, is_high_order_epistasis

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| epi-context | name | IRF5 x STAT4 (context only) | None |
| epi-context | parent_path | /intake/new-patient/reyes-ana | None |
| epi-context | relative_path | /intake/new-patient/reyes-ana/ | None |

### counterfactual_trajectories

- Fields: 1/5 (20.0%)
- Computed columns: name, parent_path, relative_path, autoimmune_disease_label, is_worsening_trajectory

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cf-context | name | Untreated SLE projection (cont | None |
| cf-context | parent_path | /intake/new-patient/reyes-ana | None |
| cf-context | relative_path | /intake/new-patient/reyes-ana/ | None |
| cf-context | autoimmune_disease_label | Systemic Lupus Erythematosus | None |

### individual_predictions

- Fields: 116/504 (23.0%)
- Computed columns: name, parent_path, relative_path, individual_ancestry_label, is_ancestry_holdout, individual_causal_mass, individual_confirmed_node_count, individual_cross_ancestry_node_count, individual_has_cryptic_relatedness, individual_max_severity_score, individual_has_high_severity_phenotype, individual_has_predicted_treatment_response, predicted_value, count_bins, count_well_calibrated_bins, sum_bin_abs_error, mean_bin_abs_error, well_calibrated_fraction, calibrated_uncertainty, rests_on_confirmed_mechanism, has_spurious_correlation_flag, is_falsifiability_backed, is_transportable_to_absent_ancestry, is_ancestry_transport_safe, transport_gate_status, is_high_confidence_prediction, patient_stratification_tier, predicted_severity_value, severity_tier, is_severity_actionable, severity_deciding_factor, is_treatment_response_actionable, treatment_response_deciding_factor, is_clinically_actionable, lifecycle_state_key, deciding_gate, individual_target_pathway, individual_progression_state_key, individual_is_disease_progressing, recommended_treatment_line, treatment_line_deciding_factor, progression_vs_actionability_disagree

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pred-a | name | pred-a | None |
| pred-a | parent_path | /intake/new-patient/reyes-ana | None |
| pred-a | relative_path | /intake/new-patient/reyes-ana/ | None |
| pred-a | individual_ancestry_label | European | None |
| pred-a | individual_causal_mass | 0.7583333333333333 | None |
| pred-a | individual_confirmed_node_count | 1 | None |
| pred-a | individual_cross_ancestry_node_count | 1 | None |
| pred-a | individual_max_severity_score | 9 | None |
| pred-a | individual_has_high_severity_phenotype | True | None |
| pred-a | individual_has_predicted_treatment_response | True | None |
| pred-a | predicted_value | 3.0166666666666666 | None |
| pred-a | count_bins | 5 | None |
| pred-a | count_well_calibrated_bins | 5 | None |
| pred-a | sum_bin_abs_error | 0.06 | None |
| pred-a | mean_bin_abs_error | 0.012 | None |
| pred-a | well_calibrated_fraction | 1 | None |
| pred-a | calibrated_uncertainty | 0.988 | None |
| pred-a | rests_on_confirmed_mechanism | True | None |
| pred-a | is_falsifiability_backed | True | None |
| pred-a | is_ancestry_transport_safe | True | None |
| ... | ... | (368 more) | ... |

### calibration_bins

- Fields: 18/300 (6.0%)
- Computed columns: name, parent_path, relative_path, bin_abs_error, is_well_calibrated_bin

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cb-a-0 | name | cb-a-0 | None |
| cb-a-0 | parent_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-0 | relative_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-0 | bin_abs_error | 0.01 | None |
| cb-a-0 | is_well_calibrated_bin | True | None |
| cb-a-1 | name | cb-a-1 | None |
| cb-a-1 | parent_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-1 | relative_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-1 | bin_abs_error | 0.01 | None |
| cb-a-1 | is_well_calibrated_bin | True | None |
| cb-a-2 | name | cb-a-2 | None |
| cb-a-2 | parent_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-2 | relative_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-2 | bin_abs_error | 0.01 | None |
| cb-a-2 | is_well_calibrated_bin | True | None |
| cb-a-3 | name | cb-a-3 | None |
| cb-a-3 | parent_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-3 | relative_path | /intake/new-patient/reyes-ana/ | None |
| cb-a-3 | bin_abs_error | 0.01 | None |
| cb-a-3 | is_well_calibrated_bin | True | None |
| ... | ... | (262 more) | ... |

### intervention_targets

- Fields: 11/66 (16.7%)
- Computed columns: name, parent_path, relative_path, causal_mechanism_label, is_gene_based_therapy, is_cell_based_therapy

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| it-a | name | IRF5 Regulatory Modulation | None |
| it-a | parent_path | /intake/new-patient/reyes-ana/ | None |
| it-a | relative_path | /intake/new-patient/reyes-ana/ | None |
| it-a | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-a | is_gene_based_therapy | True | None |
| it-b | name | IRF5 Regulatory Modulation | None |
| it-b | parent_path | /intake/new-patient/okafor-bil | None |
| it-b | relative_path | /intake/new-patient/okafor-bil | None |
| it-b | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-b | is_gene_based_therapy | True | None |
| it-c | name | STAT4 Pathway Modulation | None |
| it-c | parent_path | /intake/new-patient/wei-chen/m | None |
| it-c | relative_path | /intake/new-patient/wei-chen/m | None |
| it-c | causal_mechanism_label | STAT4 regulatory -> Th1/IFN -> | None |
| it-c | is_gene_based_therapy | True | None |
| it-d | name | IRF5 Regulatory Modulation | None |
| it-d | parent_path | /intake/new-patient/santos-die | None |
| it-d | relative_path | /intake/new-patient/santos-die | None |
| it-d | causal_mechanism_label | IRF5 regulatory -> type-I IFN  | None |
| it-d | is_gene_based_therapy | True | None |
| ... | ... | (35 more) | ... |

### axioms

- Fields: 0/30 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ax-both-verdicts | name | The same deterministic machine | None |
| ax-both-verdicts | relative_path | /admin/axioms/ax-both-verdicts | None |
| ax-every-choice-knob | name | Every choice the LLM makes is  | None |
| ax-every-choice-knob | relative_path | /admin/axioms/ax-every-choice- | None |
| ax-formula-local-edit | name | Every DAG formula is inspectab | None |
| ax-formula-local-edit | relative_path | /admin/axioms/ax-formula-local | None |
| ax-independent-witness | name | Extraction and derivation are  | None |
| ax-independent-witness | relative_path | /admin/axioms/ax-independent-w | None |
| ax-keystone-computed | name | IsClinicallyActionable is comp | None |
| ax-keystone-computed | relative_path | /admin/axioms/ax-keystone-comp | None |
| ax-line-in-dag | name | The trust boundary is a line i | None |
| ax-line-in-dag | relative_path | /admin/axioms/ax-line-in-dag | None |
| ax-llm-leaves-only | name | The LLM does exactly two thing | None |
| ax-llm-leaves-only | relative_path | /admin/axioms/ax-llm-leaves-on | None |
| ax-llm-no-vote | name | The LLM never gets a vote on t | None |
| ax-llm-no-vote | relative_path | /admin/axioms/ax-llm-no-vote | None |
| ax-report-cell-simple | name | No single piece of report lang | None |
| ax-report-cell-simple | relative_path | /admin/axioms/ax-report-cell-s | None |
| ax-report-derived-not-prose | name | The diagnosis report is derive | None |
| ax-report-derived-not-prose | relative_path | /admin/axioms/ax-report-derive | None |
| ... | ... | (10 more) | ... |

### tests_for_success

- Fields: 0/20 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tfs-all-seven | name | All seven oracle patients reso | None |
| tfs-all-seven | relative_path | /admin/tests-for-success/tfs-a | None |
| tfs-both-sides | name | The same machine produces TRUE | None |
| tfs-both-sides | relative_path | /admin/tests-for-success/tfs-b | None |
| tfs-extraction-faithful | name | Extracted leaf facts are check | None |
| tfs-extraction-faithful | relative_path | /admin/tests-for-success/tfs-e | None |
| tfs-no-hand-entry | name | Raw facts alone derive the key | None |
| tfs-no-hand-entry | relative_path | /admin/tests-for-success/tfs-n | None |
| tfs-one-gate-each | name | Five of seven patients fail, e | None |
| tfs-one-gate-each | relative_path | /admin/tests-for-success/tfs-o | None |
| tfs-report-cell-simple | name | No narrative fragment field tr | None |
| tfs-report-cell-simple | relative_path | /admin/tests-for-success/tfs-r | None |
| tfs-report-covers-every-dag-node | name | Every DAG node linked to a pat | None |
| tfs-report-covers-every-dag-node | relative_path | /admin/tests-for-success/tfs-r | None |
| tfs-report-covers-every-word | name | Every word the patient gave us | None |
| tfs-report-covers-every-word | relative_path | /admin/tests-for-success/tfs-r | None |
| tfs-report-deterministic | name | The same patient row always re | None |
| tfs-report-deterministic | relative_path | /admin/tests-for-success/tfs-r | None |
| tfs-witnessed-reasoning | name | Each derived node carries an e | None |
| tfs-witnessed-reasoning | relative_path | /admin/tests-for-success/tfs-w | None |

### features

- Fields: 0/72 (0.0%)
- Computed columns: name, relative_path, meta_line

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| feat-cohort-discovery-board | name | Cohort discovery board (corpus | None |
| feat-cohort-discovery-board | relative_path | /admin/features/feat-cohort-di | None |
| feat-cohort-discovery-board | meta_line | **Category:** Corpus-level dis | None |
| feat-coverage-map-honesty | name | Honest coverage map (modeled v | None |
| feat-coverage-map-honesty | relative_path | /admin/features/feat-coverage- | None |
| feat-coverage-map-honesty | meta_line | **Category:** Platform infrast | None |
| feat-derived-report-fragments | name | Derived narrative fragment fie | None |
| feat-derived-report-fragments | relative_path | /admin/features/feat-derived-r | None |
| feat-derived-report-fragments | meta_line | **Category:** Reporting - **Pr | None |
| feat-diagnosis-writeup | name | Per-patient diagnosis writeup | None |
| feat-diagnosis-writeup | relative_path | /admin/features/feat-diagnosis | None |
| feat-diagnosis-writeup | meta_line | **Category:** Reporting - **Pr | None |
| feat-disease-state-machine | name | Disease-state machine derived  | None |
| feat-disease-state-machine | relative_path | /admin/features/feat-disease-s | None |
| feat-disease-state-machine | meta_line | **Category:** Disease-state si | None |
| feat-emergent-serology-cluster | name | Emergent serology-signature cl | None |
| feat-emergent-serology-cluster | relative_path | /admin/features/feat-emergent- | None |
| feat-emergent-serology-cluster | meta_line | **Category:** Corpus-level dis | None |
| feat-evidence-disease-disagreement | name | Evidence-gate <-> disease-stat | None |
| feat-evidence-disease-disagreement | relative_path | /admin/features/feat-evidence- | None |
| ... | ... | (52 more) | ... |

### inference_kinds

- Fields: 0/14 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ik-aggregation-rollup | name | Aggregation rollups | None |
| ik-aggregation-rollup | relative_path | /admin/inference-kinds/ik-aggr | None |
| ik-cross-substrate-conformance | name | Cross-substrate conformance | None |
| ik-cross-substrate-conformance | relative_path | /admin/inference-kinds/ik-cros | None |
| ik-higher-order | name | Higher-order inference (gates  | None |
| ik-higher-order | relative_path | /admin/inference-kinds/ik-high | None |
| ik-lookup-fk | name | Lookup / FK resolution | None |
| ik-lookup-fk | relative_path | /admin/inference-kinds/ik-look | None |
| ik-predicate-narrative | name | Predicate-gated narrative | None |
| ik-predicate-narrative | relative_path | /admin/inference-kinds/ik-pred | None |
| ik-state-machine | name | State-machine derivation | None |
| ik-state-machine | relative_path | /admin/inference-kinds/ik-stat | None |
| ik-transitive-closure | name | Transitive closure (reachabili | None |
| ik-transitive-closure | relative_path | /admin/inference-kinds/ik-tran | None |

### open_questions

- Fields: 0/8 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| oq-extract-vs-invent | name | Does the LLM extract facts pre | None |
| oq-extract-vs-invent | relative_path | /admin/open-questions/oq-extra | None |
| oq-fragment-fields-where | name | On which tables do the narrati | None |
| oq-fragment-fields-where | relative_path | /admin/open-questions/oq-fragm | None |
| oq-publish-now | name | Publish README now as a design | None |
| oq-publish-now | relative_path | /admin/open-questions/oq-publi | None |
| oq-reworded-prompt | name | Confirm the reworded README pr | None |
| oq-reworded-prompt | relative_path | /admin/open-questions/oq-rewor | None |

### non_goals

- Fields: 0/10 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ng-clinical-use | name | This is not validated clinical | None |
| ng-clinical-use | relative_path | /admin/non-goals/ng-clinical-u | None |
| ng-crud-ui | name | This is not a CRUD UI. | None |
| ng-crud-ui | relative_path | /admin/non-goals/ng-crud-ui | None |
| ng-full-breadth | name | The full omics breadth (counte | None |
| ng-full-breadth | relative_path | /admin/non-goals/ng-full-bread | None |
| ng-llm-reasoning | name | The LLM does not perform highe | None |
| ng-llm-reasoning | relative_path | /admin/non-goals/ng-llm-reason | None |
| ng-publish-verbatim | name | Never publish the prospect's v | None |
| ng-publish-verbatim | relative_path | /admin/non-goals/ng-publish-ve | None |

### glossary_terms

- Fields: 0/26 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gt-corpus-discovery | name | corpus discovery | None |
| gt-corpus-discovery | relative_path | /admin/glossary/gt-corpus-disc | None |
| gt-disease-progression-machine | name | disease progression machine | None |
| gt-disease-progression-machine | relative_path | /admin/glossary/gt-disease-pro | None |
| gt-disease-state-simulator | name | disease state simulator | None |
| gt-disease-state-simulator | relative_path | /admin/glossary/gt-disease-sta | None |
| gt-dwell-time | name | dwell time | None |
| gt-dwell-time | relative_path | /admin/glossary/gt-dwell-time | None |
| gt-evidence-gate | name | evidence gate | None |
| gt-evidence-gate | relative_path | /admin/glossary/gt-evidence-ga | None |
| gt-fake-but-transparent | name | fake-but-transparent | None |
| gt-fake-but-transparent | relative_path | /admin/glossary/gt-fake-but-tr | None |
| gt-intake-clerk-lab | name | intake clerk + lab | None |
| gt-intake-clerk-lab | relative_path | /admin/glossary/gt-intake-cler | None |
| gt-knob | name | knob | None |
| gt-knob | relative_path | /admin/glossary/gt-knob | None |
| gt-laundering-objection | name | laundering objection | None |
| gt-laundering-objection | relative_path | /admin/glossary/gt-laundering- | None |
| gt-line-in-dag | name | line in the DAG | None |
| gt-line-in-dag | relative_path | /admin/glossary/gt-line-in-dag | None |
| ... | ... | (6 more) | ... |

### effortless_loops

- Fields: 14/64 (21.9%)
- Computed columns: name, relative_path, completedness, is_in_current_plan

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| loop-0 | name | Loop 0 — Solve-by-inference ru | None |
| loop-0 | relative_path | /admin/effortless-loops/loop-0 | None |
| loop-0 | completedness | done | None |
| loop-0-5 | name | Loop 0.5 — Test Harness First  | None |
| loop-0-5 | relative_path | /admin/effortless-loops/loop-0 | None |
| loop-0-5 | completedness | done | None |
| loop-1 | name | Loop 1 — Intake app skeleton | None |
| loop-1 | relative_path | /admin/effortless-loops/loop-1 | None |
| loop-1 | completedness | done | None |
| loop-2 | name | Loop 2 — Patient intake (facts | None |
| loop-2 | relative_path | /admin/effortless-loops/loop-2 | None |
| loop-2 | completedness | done | None |
| loop-3 | name | Loop 3 — 3-panel witness — pro | None |
| loop-3 | relative_path | /admin/effortless-loops/loop-3 | None |
| loop-3 | completedness | done | None |
| loop-4 | name | Loop 4 — Second prediction typ | None |
| loop-4 | relative_path | /admin/effortless-loops/loop-4 | None |
| loop-4 | completedness | done | None |
| loop-5 | name | Loop 5 — Treatment-response pr | None |
| loop-5 | relative_path | /admin/effortless-loops/loop-5 | None |
| ... | ... | (30 more) | ... |

### routing_and_navigation

- Fields: 270/504 (53.6%)
- Computed columns: name, admin_can_create, admin_can_read, admin_can_update, admin_can_delete, intake_clinician_can_create, intake_clinician_can_read, intake_clinician_can_update, intake_clinician_can_delete, diagnosing_doctor_can_create, diagnosing_doctor_can_read, diagnosing_doctor_can_update, diagnosing_doctor_can_delete, external_llm_can_create, external_llm_can_read, external_llm_can_update, external_llm_can_delete, depth, full_path, handler_base_name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| nav-admin | name | admin | None |
| nav-admin | admin_can_create | True | None |
| nav-admin | admin_can_read | True | None |
| nav-admin | admin_can_update | True | None |
| nav-admin | admin_can_delete | True | None |
| nav-admin | full_path | /admin | None |
| nav-admin | handler_base_name | admin | None |
| nav-admin | relative_path | /admin/routing/nav-admin | None |
| nav-admin-cohort | name | cohort-discovery | None |
| nav-admin-cohort | admin_can_create | True | None |
| nav-admin-cohort | admin_can_read | True | None |
| nav-admin-cohort | admin_can_update | True | None |
| nav-admin-cohort | admin_can_delete | True | None |
| nav-admin-cohort | full_path | /cohort | None |
| nav-admin-cohort | handler_base_name | admin cohort | None |
| nav-admin-cohort | relative_path | /admin/routing/nav-admin-cohor | None |
| nav-admin-explainer | name | explainer-dag | None |
| nav-admin-explainer | admin_can_create | True | None |
| nav-admin-explainer | admin_can_read | True | None |
| nav-admin-explainer | admin_can_update | True | None |
| ... | ... | (214 more) | ... |

### state_machines

- Fields: 0/8 (0.0%)
- Computed columns: name, relative_path, state_count, transition_rule_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| diagnosis-lifecycle | name | diagnosis-lifecycle | None |
| diagnosis-lifecycle | relative_path | /admin/state-machine/diagnosis | None |
| diagnosis-lifecycle | state_count | 7 | None |
| diagnosis-lifecycle | transition_rule_count | 9 | None |
| lupus-nephritis-progression | name | lupus-nephritis-progression | None |
| lupus-nephritis-progression | relative_path | /admin/state-machine/lupus-nep | None |
| lupus-nephritis-progression | state_count | 6 | None |
| lupus-nephritis-progression | transition_rule_count | 5 | None |

### machine_states

- Fields: 4/39 (10.3%)
- Computed columns: name, relative_path, reachable_state_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| diagnosis-lifecycle--actionable | name | diagnosis-lifecycle--actionabl | None |
| diagnosis-lifecycle--actionable | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--calibrationchecked | name | diagnosis-lifecycle--calibrati | None |
| diagnosis-lifecycle--calibrationchecked | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--calibrationchecked | reachable_state_count | 3 | None |
| diagnosis-lifecycle--evidenceassessed | name | diagnosis-lifecycle--evidencea | None |
| diagnosis-lifecycle--evidenceassessed | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--evidenceassessed | reachable_state_count | 5 | None |
| diagnosis-lifecycle--intake | name | diagnosis-lifecycle--intake | None |
| diagnosis-lifecycle--intake | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--intake | reachable_state_count | 6 | None |
| diagnosis-lifecycle--mechanismconfirmed | name | diagnosis-lifecycle--mechanism | None |
| diagnosis-lifecycle--mechanismconfirmed | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--mechanismconfirmed | reachable_state_count | 4 | None |
| diagnosis-lifecycle--notactionable | name | diagnosis-lifecycle--notaction | None |
| diagnosis-lifecycle--notactionable | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--transportchecked | name | diagnosis-lifecycle--transport | None |
| diagnosis-lifecycle--transportchecked | relative_path | /admin/state-machine/states/di | None |
| diagnosis-lifecycle--transportchecked | reachable_state_count | 2 | None |
| lupus-nephritis-progression--biopsyindicated | name | lupus-nephritis-progression--b | None |
| ... | ... | (15 more) | ... |

### state_transition_rules

- Fields: 0/70 (0.0%)
- Computed columns: name, relative_path, from_state_key, to_state_key, is_forward_edge

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| diagnosis-lifecycle--calibrationchecked->notactionable | name | diagnosis-lifecycle--calibrati | None |
| diagnosis-lifecycle--calibrationchecked->notactionable | relative_path | /admin/state-machine/rules/dia | None |
| diagnosis-lifecycle--calibrationchecked->notactionable | from_state_key | CalibrationChecked | None |
| diagnosis-lifecycle--calibrationchecked->notactionable | to_state_key | NotActionable | None |
| diagnosis-lifecycle--calibrationchecked->notactionable | is_forward_edge | True | None |
| diagnosis-lifecycle--calibrationchecked->transportchecked | name | diagnosis-lifecycle--calibrati | None |
| diagnosis-lifecycle--calibrationchecked->transportchecked | relative_path | /admin/state-machine/rules/dia | None |
| diagnosis-lifecycle--calibrationchecked->transportchecked | from_state_key | CalibrationChecked | None |
| diagnosis-lifecycle--calibrationchecked->transportchecked | to_state_key | TransportChecked | None |
| diagnosis-lifecycle--calibrationchecked->transportchecked | is_forward_edge | True | None |
| diagnosis-lifecycle--evidenceassessed->mechanismconfirmed | name | diagnosis-lifecycle--evidencea | None |
| diagnosis-lifecycle--evidenceassessed->mechanismconfirmed | relative_path | /admin/state-machine/rules/dia | None |
| diagnosis-lifecycle--evidenceassessed->mechanismconfirmed | from_state_key | EvidenceAssessed | None |
| diagnosis-lifecycle--evidenceassessed->mechanismconfirmed | to_state_key | MechanismConfirmed | None |
| diagnosis-lifecycle--evidenceassessed->mechanismconfirmed | is_forward_edge | True | None |
| diagnosis-lifecycle--evidenceassessed->notactionable | name | diagnosis-lifecycle--evidencea | None |
| diagnosis-lifecycle--evidenceassessed->notactionable | relative_path | /admin/state-machine/rules/dia | None |
| diagnosis-lifecycle--evidenceassessed->notactionable | from_state_key | EvidenceAssessed | None |
| diagnosis-lifecycle--evidenceassessed->notactionable | to_state_key | NotActionable | None |
| diagnosis-lifecycle--evidenceassessed->notactionable | is_forward_edge | True | None |
| ... | ... | (50 more) | ... |

### state_transitions

- Fields: 5/216 (2.3%)
- Computed columns: name, relative_path, is_forward

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| st-a-02-intake-to-evidenceassessed | name | st-a-02-intake-to-evidenceasse | None |
| st-a-02-intake-to-evidenceassessed | relative_path | /admin/state-machine/transitio | None |
| st-a-02-intake-to-evidenceassessed | is_forward | True | None |
| st-a-03-evidenceassessed-to-mechanismconfirmed | name | st-a-03-evidenceassessed-to-me | None |
| st-a-03-evidenceassessed-to-mechanismconfirmed | relative_path | /admin/state-machine/transitio | None |
| st-a-03-evidenceassessed-to-mechanismconfirmed | is_forward | True | None |
| st-a-04-mechanismconfirmed-to-calibrationchecked | name | st-a-04-mechanismconfirmed-to- | None |
| st-a-04-mechanismconfirmed-to-calibrationchecked | relative_path | /admin/state-machine/transitio | None |
| st-a-04-mechanismconfirmed-to-calibrationchecked | is_forward | True | None |
| st-a-05-calibrationchecked-to-transportchecked | name | st-a-05-calibrationchecked-to- | None |
| st-a-05-calibrationchecked-to-transportchecked | relative_path | /admin/state-machine/transitio | None |
| st-a-05-calibrationchecked-to-transportchecked | is_forward | True | None |
| st-a-06-transportchecked-to-actionable | name | st-a-06-transportchecked-to-ac | None |
| st-a-06-transportchecked-to-actionable | relative_path | /admin/state-machine/transitio | None |
| st-a-06-transportchecked-to-actionable | is_forward | True | None |
| st-b-02-intake-to-evidenceassessed | name | st-b-02-intake-to-evidenceasse | None |
| st-b-02-intake-to-evidenceassessed | relative_path | /admin/state-machine/transitio | None |
| st-b-02-intake-to-evidenceassessed | is_forward | True | None |
| st-b-03-evidenceassessed-to-mechanismconfirmed | name | st-b-03-evidenceassessed-to-me | None |
| st-b-03-evidenceassessed-to-mechanismconfirmed | relative_path | /admin/state-machine/transitio | None |
| ... | ... | (191 more) | ... |

### subject_state_instances

- Fields: 115/395 (29.1%)
- Computed columns: name, relative_path, is_current, has_complete_lineage, is_long_dwell

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ssi-ind-a-reyes-presymptomaticautoimmunity-1 | name | ssi-ind-a-reyes-presymptomatic | None |
| ssi-ind-a-reyes-presymptomaticautoimmunity-1 | relative_path | /admin/state-machine/instances | None |
| ssi-ind-a-reyes-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-a-reyes-presymptomaticautoimmunity-1 | is_long_dwell | True | None |
| ssi-ind-a-reyes-serologicactive-2 | name | ssi-ind-a-reyes-serologicactiv | None |
| ssi-ind-a-reyes-serologicactive-2 | relative_path | /admin/state-machine/instances | None |
| ssi-ind-a-reyes-serologicactive-2 | is_current | True | None |
| ssi-ind-a-reyes-serologicactive-2 | has_complete_lineage | True | None |
| ssi-ind-a-reyes-serologicactive-2 | is_long_dwell | True | None |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | name | ssi-ind-b-okafor-presymptomati | None |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | relative_path | /admin/state-machine/instances | None |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | is_current | True | None |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-b-okafor-presymptomaticautoimmunity-1 | is_long_dwell | True | None |
| ssi-ind-c-chen-presymptomaticautoimmunity-1 | name | ssi-ind-c-chen-presymptomatica | None |
| ssi-ind-c-chen-presymptomaticautoimmunity-1 | relative_path | /admin/state-machine/instances | None |
| ssi-ind-c-chen-presymptomaticautoimmunity-1 | has_complete_lineage | True | None |
| ssi-ind-c-chen-presymptomaticautoimmunity-1 | is_long_dwell | True | None |
| ssi-ind-c-chen-serologicactive-2 | name | ssi-ind-c-chen-serologicactive | None |
| ssi-ind-c-chen-serologicactive-2 | relative_path | /admin/state-machine/instances | None |
| ... | ... | (260 more) | ... |

### disease_domain_concepts

- Fields: 33/104 (31.7%)
- Computed columns: name, relative_path, is_deeply_modeled, is_schema_modeled

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| adverse-event-surveillance | name | Adverse-Event Surveillance | None |
| adverse-event-surveillance | relative_path | /admin/disease-concepts/advers | None |
| axial-psa | name | Axial Psoriatic Arthritis | None |
| axial-psa | relative_path | /admin/disease-concepts/axial- | None |
| axial-psa | is_schema_modeled | True | None |
| cutaneous-lupus | name | Cutaneous Lupus | None |
| cutaneous-lupus | relative_path | /admin/disease-concepts/cutane | None |
| cutaneous-lupus | is_schema_modeled | True | None |
| dactylitis | name | Dactylitis | None |
| dactylitis | relative_path | /admin/disease-concepts/dactyl | None |
| dactylitis | is_schema_modeled | True | None |
| disease-activity-score | name | Disease-Activity Score (SLEDAI | None |
| disease-activity-score | relative_path | /admin/disease-concepts/diseas | None |
| disease-activity-score | is_deeply_modeled | True | None |
| disease-activity-score | is_schema_modeled | True | None |
| enthesitis | name | Enthesitis | None |
| enthesitis | relative_path | /admin/disease-concepts/enthes | None |
| enthesitis | is_schema_modeled | True | None |
| erosive-disease | name | Erosive Disease | None |
| erosive-disease | relative_path | /admin/disease-concepts/erosiv | None |
| ... | ... | (51 more) | ... |

### serology_observations

- Fields: 153/375 (40.8%)
- Computed columns: prior_anti_ds_dna_iu, prior_c3, prior_c4, anti_ds_dna_trend, complement_trend, is_pre_nephritic_signature_panel, is_significant_proteinuria, is_nephrotic_range_proteinuria, sledai_renal_points, sledai_serology_points, sledai_score, progression_state_key, progression_state_order, name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sero-ind-a-reyes-1 | anti_ds_dna_trend | Stable | None |
| sero-ind-a-reyes-1 | complement_trend | Stable | None |
| sero-ind-a-reyes-1 | progression_state_key | PresymptomaticAutoimmunity | None |
| sero-ind-a-reyes-1 | progression_state_order | 1 | None |
| sero-ind-a-reyes-1 | name | sero-ind-a-reyes-1 | None |
| sero-ind-a-reyes-1 | relative_path | /admin/serology/sero-ind-a-rey | None |
| sero-ind-a-reyes-2 | prior_anti_ds_dna_iu | 40 | None |
| sero-ind-a-reyes-2 | prior_c3 | 110 | None |
| sero-ind-a-reyes-2 | prior_c4 | 22 | None |
| sero-ind-a-reyes-2 | anti_ds_dna_trend | Rising | None |
| sero-ind-a-reyes-2 | complement_trend | Falling | None |
| sero-ind-a-reyes-2 | is_pre_nephritic_signature_panel | True | None |
| sero-ind-a-reyes-2 | sledai_serology_points | 4 | None |
| sero-ind-a-reyes-2 | sledai_score | 4 | None |
| sero-ind-a-reyes-2 | progression_state_key | SerologicActive | None |
| sero-ind-a-reyes-2 | progression_state_order | 2 | None |
| sero-ind-a-reyes-2 | name | sero-ind-a-reyes-2 | None |
| sero-ind-a-reyes-2 | relative_path | /admin/serology/sero-ind-a-rey | None |
| sero-ind-b-okafor-1 | anti_ds_dna_trend | Stable | None |
| sero-ind-b-okafor-1 | complement_trend | Stable | None |
| ... | ... | (202 more) | ... |

### therapy_options

- Fields: 0/8 (0.0%)
- Computed columns: name, relative_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| anifrolumab | name | Anifrolumab (anti-IFNAR1) | None |
| anifrolumab | relative_path | /admin/therapy-options/anifrol | None |
| belimumab | name | Belimumab (anti-BLyS) | None |
| belimumab | relative_path | /admin/therapy-options/belimum | None |
| mmf | name | Mycophenolate (MMF) | None |
| mmf | relative_path | /admin/therapy-options/mmf | None |
| secukinumab | name | Secukinumab (anti-IL-17) | None |
| secukinumab | relative_path | /admin/therapy-options/secukin | None |
