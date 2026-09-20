# Test Results: effortless-owl

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 273809 |
| Passed | 114943 |
| Failed | 158866 |
| Score | 42.0% |
| Duration | 10m 0s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 81906 | 172087 | 47.6% |
| Lookup (INDEX/MATCH) | 17362 | 78017 | 22.3% |
| Aggregation (COUNTIFS/SUMIFS) | 15675 | 23705 | 66.1% |

## Error

```
Script timed out
```

## Results by Entity

### rulebook_releases

- Fields: 345/522 (66.1%)
- Computed columns: name, prev_major, prev_minor, prev_patch, prev_issued_at, model_current_release, expected_major, expected_minor, expected_patch, is_increment_inconsistent_with_scale, days_since_previous_release, is_long_release_cycle, is_declared_current_release, log_entry_count, logical_change_count, non_additive_change_count, class_removal_or_rename_count, invalidating_domain_range_count, inconsistent_disjointness_count, schema_addition_count, suite_update_count, consumer_count, notified_consumer_count, revalidated_consumer_count, validation_run_count, validation_failure_total, consistent_run_count, cq_run_count, answerable_cq_run_count, regressed_baseline_count, cq_coverage_percent, prev_cq_coverage_percent, prev_cq_run_count, scored_criterion_count, stated_criterion_count, patch_alters_logical_model, minor_is_not_backward_compatible, class_removal_without_major, invalidating_domain_range_without_major, inconsistent_disjointness_without_major, is_breaking_release, breaking_release_with_unrevalidated_consumers, breaking_release_without_migration_plan, is_undocumented_version_decision, is_unannounced_to_dependents, is_release_without_recorded_changes, suite_lags_release, is_untagged_release, published_without_approval, released_despite_failed_validation, released_without_consistency_check, passed_validation_at_release, cq_coverage_declined, has_baseline_regression, released_without_cq_task_test, well_formed_but_requirements_unshown, is_not_scored_against_criteria, is_released_without_licence_or_permanent_id

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pko-release-0.10.0 | name | 0.10.0 / PKO https://w3id.org/ | None |
| pko-release-0.10.0 | prev_minor | 9 | None |
| pko-release-0.10.0 | prev_issued_at | 2026-04-20T12:00:00-05:00 | None |
| pko-release-0.10.0 | model_current_release | pko-release-1.0.0 | None |
| pko-release-0.10.0 | expected_minor | 10 | None |
| pko-release-0.10.0 | days_since_previous_release | 51 | None |
| pko-release-0.10.0 | log_entry_count | 3 | None |
| pko-release-0.10.0 | logical_change_count | 3 | None |
| pko-release-0.10.0 | non_additive_change_count | 2 | None |
| pko-release-0.10.0 | class_removal_or_rename_count | 1 | None |
| pko-release-0.10.0 | invalidating_domain_range_count | 1 | None |
| pko-release-0.10.0 | consumer_count | 4 | None |
| pko-release-0.10.0 | notified_consumer_count | 4 | None |
| pko-release-0.10.0 | validation_run_count | 2 | None |
| pko-release-0.10.0 | validation_failure_total | 2 | None |
| pko-release-0.10.0 | cq_run_count | 3 | None |
| pko-release-0.10.0 | answerable_cq_run_count | 2 | None |
| pko-release-0.10.0 | regressed_baseline_count | 1 | None |
| pko-release-0.10.0 | cq_coverage_percent | 66.7 | None |
| pko-release-0.10.0 | prev_cq_coverage_percent | 100.0 | None |
| ... | ... | (157 more) | ... |

### ontology_profiles

- Fields: 258/405 (63.7%)
- Computed columns: name, mapping_count, as_of_instant, days_since_last_revision, days_since_major_revision, days_since_dependency_reviewed, recent_deprecation_count, requires_frequent_review, change_rate_profile, is_review_overdue_for_change_rate, prerequisite_adopted_at, skips_adoption_path, namespace_is_http, namespace_dereferences, publishes_following_linked_data_principles

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| adms-2 | name | Asset Description Metadata Sch | None |
| adms-2 | mapping_count | 1 | None |
| adms-2 | change_rate_profile | Unassessed | None |
| adms-2 | namespace_is_http | True | None |
| bibo | name | Bibliographic Ontology 1.3 | None |
| bibo | mapping_count | 1 | None |
| bibo | change_rate_profile | Unassessed | None |
| bibo | namespace_is_http | True | None |
| bpmn-2-0 | name | Business Process Model and Not | None |
| bpmn-2-0 | mapping_count | 2 | None |
| bpmn-2-0 | change_rate_profile | Unassessed | None |
| bpmn-2-0 | namespace_is_http | True | None |
| dcat-3 | name | DCAT 3 | None |
| dcat-3 | mapping_count | 3 | None |
| dcat-3 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| dcat-3 | days_since_last_revision | 696 | None |
| dcat-3 | days_since_major_revision | 696 | None |
| dcat-3 | days_since_dependency_reviewed | 39 | None |
| dcat-3 | recent_deprecation_count | 1 | None |
| dcat-3 | requires_frequent_review | True | None |
| ... | ... | (127 more) | ... |

### evaluation_contexts

- Fields: 0/16 (0.0%)
- Computed columns: name, explicit_fragment_count, tacit_fragment_count, implicit_fragment_count, situated_judgment_fragment_count, model_reasoned_answer_count, otherwise_reasoned_answer_count, assistant_answer_count, model_reasoned_failed_answer_count, out_of_order_step_execution_count, in_order_step_execution_count, step_execution_count, early_start_step_execution_count, exact_mapping_count, aligned_mapping_count, extension_mapping_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| eval-current | name | Post-close evaluation @ 2026-0 | None |
| eval-current | explicit_fragment_count | 3 | None |
| eval-current | tacit_fragment_count | 4 | None |
| eval-current | implicit_fragment_count | 3 | None |
| eval-current | situated_judgment_fragment_count | 2 | None |
| eval-current | model_reasoned_answer_count | 4 | None |
| eval-current | otherwise_reasoned_answer_count | 9 | None |
| eval-current | assistant_answer_count | 13 | None |
| eval-current | model_reasoned_failed_answer_count | 4 | None |
| eval-current | out_of_order_step_execution_count | 6 | None |
| eval-current | in_order_step_execution_count | 64 | None |
| eval-current | step_execution_count | 70 | None |
| eval-current | early_start_step_execution_count | 3 | None |
| eval-current | exact_mapping_count | 69 | None |
| eval-current | aligned_mapping_count | 85 | None |
| eval-current | extension_mapping_count | 203 | None |

### organizations

- Fields: 385/468 (82.3%)
- Computed columns: name, failed_ai_initiative_count, owned_procedure_count, ai_fails_for_lack_of_captured_knowledge, product_delivery_function_count, provider_held_delivery_method_count, is_hollowed_out_firm, audit_finding_count, filled_knowledge_position_count, has_knowledge_findings_without_knowledge_staff, departed_holder_know_how_count, lost_departed_know_how_count, retained_departed_know_how_percent, memory_leaves_with_staff, captured_own_know_how_count, documentation_entry_count, unallocated_documentation_count, transfer_given_count, unallocated_transfer_count, treats_knowledge_work_as_unvalued, person_carried_know_how_count, facility_carried_know_how_count, system_carried_know_how_count, holds_know_how_in_people_plants_and_systems, staged_decline_count, eroded_in_stages

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| acme-corp | name | ACME Corporation | None |
| acme-corp | owned_procedure_count | 1 | None |
| acme-corp | retained_departed_know_how_percent | 100.0 | None |
| acme-customer-support | name | ACME Customer Support | None |
| acme-customer-support | retained_departed_know_how_percent | 100.0 | None |
| acme-engineering | name | ACME Platform Engineering | None |
| acme-engineering | failed_ai_initiative_count | 1 | None |
| acme-engineering | owned_procedure_count | 2 | None |
| acme-engineering | product_delivery_function_count | 1 | None |
| acme-engineering | audit_finding_count | 1 | None |
| acme-engineering | filled_knowledge_position_count | 1 | None |
| acme-engineering | departed_holder_know_how_count | 3 | None |
| acme-engineering | lost_departed_know_how_count | 2 | None |
| acme-engineering | retained_departed_know_how_percent | 33.0 | None |
| acme-engineering | memory_leaves_with_staff | True | None |
| acme-engineering | captured_own_know_how_count | 2 | None |
| acme-engineering | documentation_entry_count | 3 | None |
| acme-engineering | unallocated_documentation_count | 2 | None |
| acme-engineering | transfer_given_count | 2 | None |
| acme-engineering | unallocated_transfer_count | 2 | None |
| ... | ... | (63 more) | ... |

### agents

- Fields: 2461/2695 (91.3%)
- Computed columns: name, count_of_current_role_assignments, is_still_engaged, decision_count, overridden_decision_count, override_rate_percent, is_non_human, boundary_violation_count, is_operating_outside_boundary, draft_decision_count, overridden_draft_count, draft_rewrite_rate_percent, times_named_as_broker, is_recognized_broker, at_risk_reliance_count, has_at_risk_knowledge_reliance, is_organization_agent, answer_count, ai_task_completed_count, ai_task_completion_percent, is_below_task_completion_target, runtime_integration_count, lacks_runtime_knowledge_integration, search_event_count, is_untracked_ai_consumer, accountability_assertion_count, inferred_category_count, category_not_available_as_inference, is_unclassified_agent, is_ai_agent_without_model_version, attributed_artifact_count, has_produced_artifacts, has_artifact_blast_radius, registry_version_match_count, is_ai_agent_not_filled_from_registry, current_accountable_human_count, is_ai_agent_without_accountable_human, community_count, is_boundary_spanner, sna_identification_count, is_unidentified_boundary_spanner, located_know_how_count, required_mentoring_hours_per_week, lacks_time_to_mentor, transfers_given_count, recognition_count, is_unrewarded_sharer, downstream_of_held_steps_count, artifact_blast_radius_step_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| acme-plant-org-agent | name | ACME Appliance Plant (as an ag | None |
| acme-plant-org-agent | is_non_human | True | None |
| acme-plant-org-agent | is_organization_agent | True | None |
| aisha-bello | name | Aisha Bello | None |
| aisha-bello | count_of_current_role_assignments | 1 | None |
| aisha-bello | is_still_engaged | True | None |
| aisha-bello | search_event_count | 1 | None |
| amina-yusuf | name | Amina Yusuf | None |
| amina-yusuf | count_of_current_role_assignments | 1 | None |
| amina-yusuf | is_still_engaged | True | None |
| anil-kapoor | name | Anil Kapoor | None |
| bea-okonkwo | name | Bea Okonkwo | None |
| bea-okonkwo | count_of_current_role_assignments | 1 | None |
| bea-okonkwo | is_still_engaged | True | None |
| carlos-mendez | name | Carlos Mendez | None |
| carlos-mendez | count_of_current_role_assignments | 1 | None |
| carlos-mendez | is_still_engaged | True | None |
| checklist-bot-2019 | name | Checklist Bot (2019 SOP) | None |
| checklist-bot-2019 | is_non_human | True | None |
| checklist-bot-2019 | answer_count | 1 | None |
| ... | ... | (214 more) | ... |

### roles

- Fields: 882/1188 (74.2%)
- Computed columns: name, current_agent_kind, active_assignment_count, currently_covered_assignment_count, has_no_current_holder, count_of_awaited_decisions, current_assignment_valid_from, is_non_human_held, is_ungoverned_non_human_role, departed_assignment_count, has_lost_a_holder, is_vacated_role, ungrounded_boundary_count, is_governed_by_lapsed_authority, unescalated_refusal_count, unauthorized_enforcement_assignment_count, is_ungoverned_enforcement_role, specialized_role_family, specialization_count, has_specializations, is_senior_variant_not_specialization, organization_type, is_not_housed_in_department, capability_tag_count, compliance_review_tag_count, has_compliance_review_capability, role_mention_count, unresolved_role_mention_count, is_missed_by_phrase_query, current_holder_name, backup_role_holder, has_escalation_backup, has_unfilled_escalation_backup, release_approval_step_count, is_production_release_approver, approval_step_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ai-enablement-lead | name | AI Enablement Lead | None |
| ai-enablement-lead | current_agent_kind | Human | None |
| ai-enablement-lead | active_assignment_count | 1.0 | None |
| ai-enablement-lead | currently_covered_assignment_count | 1.0 | None |
| ai-enablement-lead | current_assignment_valid_from | 2025-10-01T00:00:00-05:00 | None |
| ai-enablement-lead | organization_type | Company | None |
| ai-enablement-lead | is_not_housed_in_department | True | None |
| ai-enablement-lead | current_holder_name | Farah Nasser | None |
| build-release-engineer | name | Build and Release Engineer | None |
| build-release-engineer | active_assignment_count | 1.0 | None |
| build-release-engineer | has_no_current_holder | True | None |
| build-release-engineer | is_non_human_held | True | None |
| build-release-engineer | is_ungoverned_non_human_role | True | None |
| build-release-engineer | departed_assignment_count | 1.0 | None |
| build-release-engineer | has_lost_a_holder | True | None |
| build-release-engineer | is_vacated_role | True | None |
| build-release-engineer | organization_type | Division | None |
| build-release-engineer | is_not_housed_in_department | True | None |
| cfo | name | Chief Financial Officer | None |
| cfo | current_agent_kind | Human | None |
| ... | ... | (286 more) | ... |

### role_assignments

- Fields: 1415/2016 (70.2%)
- Computed columns: name, as_of_instant, is_current, current_agent_key, is_currently_valid, agent_role_key, has_departed, covers_now, role_when_covering, agent_kind, is_non_human_assignment, predecessor_agent_kind, is_human_to_non_human_handover, is_unauthorized_non_human_assignment, was_authorized_by_change_request, decision_count, overridden_decision_count, override_rate_percent, predecessor_override_rate_percent, quality_regressed_vs_predecessor, departed_role_key, predecessor_decision_count, has_sufficient_sample, predecessor_has_sufficient_sample, comparison_is_evidentially_sound, single_override_swing_percent, quality_verdict_is_unsupported, is_unmeasured_automation_handover, error_correction_count, error_rate_percent, has_dated_authorization, days_since_authorization_review, authorization_is_overdue_for_review, is_standing_unreviewed_automation, is_unconditioned_automation_handover, exceeds_tolerable_error_rate, boundary_violation_count_for_assignment, has_any_boundary_violation, has_ungrounded_governing_boundary, suspension_condition_met, is_operating_under_met_suspension_condition, has_declared_suspension_condition, has_approving_authority, has_authorizing_change_request, is_unauthorized_enforcement_agent, governance_evidence_count, unauthorized_enforcement_role_key, scoped_version_status, is_scoped_to_retired_version, predecessor_valid_to, predecessor_lacks_validity_end, agent_version_key, agent_role_pair_key, is_open_ended, role_approval_step_count, receives_approval_notices_now

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ra-ai-lead-farah | name | ai-enablement-lead @ 2025-10-0 | None |
| ra-ai-lead-farah | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ra-ai-lead-farah | is_current | True | None |
| ra-ai-lead-farah | current_agent_key | farah-nasser | None |
| ra-ai-lead-farah | is_currently_valid | True | None |
| ra-ai-lead-farah | agent_role_key | farah-nasser|ai-enablement-lea | None |
| ra-ai-lead-farah | covers_now | True | None |
| ra-ai-lead-farah | role_when_covering | ai-enablement-lead | None |
| ra-ai-lead-farah | agent_kind | Human | None |
| ra-ai-lead-farah | has_sufficient_sample | True | None |
| ra-ai-lead-farah | predecessor_has_sufficient_sample | True | None |
| ra-ai-lead-farah | comparison_is_evidentially_sound | True | None |
| ra-ai-lead-farah | days_since_authorization_review | 291 | None |
| ra-ai-lead-farah | agent_version_key | ai-lead-2025 | None |
| ra-ai-lead-farah | agent_role_pair_key | farah-nasser|ai-enablement-lea | None |
| ra-ai-lead-farah | is_open_ended | True | None |
| ra-authority-2026 | name | knowledge-authority @ 2026-01- | None |
| ra-authority-2026 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ra-authority-2026 | is_current | True | None |
| ra-authority-2026 | current_agent_key | priya-raman | None |
| ... | ... | (581 more) | ... |

### communities_of_practice

- Fields: 81/119 (68.1%)
- Computed columns: name, has_own_vocabulary_and_norms, sharing_event_count, is_mandated_without_sharing_norm, interconnected_know_how_count, physical_know_how_count, digital_know_how_count, person_carried_know_how_count, spans_physical_and_digital_with_humans, external_member_count, specialist_member_count, employer_move_count, is_cross_firm_practice_cluster, recent_apprenticeship_count, is_circulation_ending_for_lack_of_apprentices, ambient_absorption_count, has_ambient_trade_know_how

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-practice-guild | name | Financial Close Community of P | None |
| eastvale-packaging-cluster | name | Eastvale Packaging Machinery C | None |
| eastvale-packaging-cluster | sharing_event_count | 1 | None |
| eastvale-packaging-cluster | physical_know_how_count | 1 | None |
| eastvale-packaging-cluster | person_carried_know_how_count | 1 | None |
| eastvale-packaging-cluster | external_member_count | 1 | None |
| eastvale-packaging-cluster | employer_move_count | 1 | None |
| eastvale-packaging-cluster | is_circulation_ending_for_lack_of_apprentices | True | None |
| km-portal-community | name | Knowledge Sharing Portal Commu | None |
| km-portal-community | is_mandated_without_sharing_norm | True | None |
| km-portal-community | external_member_count | 1 | None |
| plant-maintenance-guild | name | Plant Maintenance Guild | None |
| plant-maintenance-guild | has_own_vocabulary_and_norms | True | None |
| plant-maintenance-guild | sharing_event_count | 3 | None |
| plant-maintenance-guild | interconnected_know_how_count | 2 | None |
| plant-maintenance-guild | physical_know_how_count | 5 | None |
| plant-maintenance-guild | digital_know_how_count | 1 | None |
| plant-maintenance-guild | person_carried_know_how_count | 4 | None |
| plant-maintenance-guild | spans_physical_and_digital_with_humans | True | None |
| plant-maintenance-guild | specialist_member_count | 1 | None |
| ... | ... | (18 more) | ... |

### mentorships

- Fields: 5/30 (16.7%)
- Computed columns: name, as_of_instant, is_active, days_since_started, is_recent_apprenticeship, community_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| mentor-close-2026 | name | devon-okafor -> maria-chen | None |
| mentor-close-2026 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| mentor-close-2026 | is_active | True | None |
| mentor-close-2026 | days_since_started | 185 | None |
| mentor-close-2026 | community_label | Financial Close Community of P | None |
| ms12-eastvale-2014 | name | joao-ferreira -> petra-lindqvi | None |
| ms12-eastvale-2014 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ms12-eastvale-2014 | days_since_started | 4339 | None |
| ms12-eastvale-2014 | community_label | Eastvale Packaging Machinery C | None |
| ms12-tomas-aisha | name | tomas-reyes -> aisha-bello | None |
| ms12-tomas-aisha | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ms12-tomas-aisha | is_active | True | None |
| ms12-tomas-aisha | days_since_started | 48 | None |
| ms12-tomas-aisha | is_recent_apprenticeship | True | None |
| ms12-tomas-aisha | community_label | Plant Maintenance Guild | None |
| ms12-tomas-bea | name | tomas-reyes -> bea-okonkwo | None |
| ms12-tomas-bea | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ms12-tomas-bea | is_active | True | None |
| ms12-tomas-bea | days_since_started | 18 | None |
| ms12-tomas-bea | is_recent_apprenticeship | True | None |
| ... | ... | (5 more) | ... |

### procedure_types

- Fields: 95/153 (62.1%)
- Computed columns: name, narrower_type_count, has_narrower_types, is_detached_from_taxonomy, broader_is_detached, is_unreachable_by_navigation, member_count, members_lacking_distinction_count, is_arbitrary_grouping

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| calibration-procedure | name | Instrument Calibration Procedu | None |
| calibration-procedure | broader_is_detached | True | None |
| calibration-procedure | is_unreachable_by_navigation | True | None |
| communication-policy | name | Employee Communication Policy  | None |
| communication-policy | member_count | 1 | None |
| domain-customer-service | name | Customer Service | None |
| domain-customer-service | narrower_type_count | 1 | None |
| domain-customer-service | has_narrower_types | True | None |
| domain-customer-service | broader_is_detached | True | None |
| domain-finance | name | Finance | None |
| domain-finance | narrower_type_count | 1 | None |
| domain-finance | has_narrower_types | True | None |
| domain-finance | broader_is_detached | True | None |
| domain-human-resources | name | Human Resources | None |
| domain-human-resources | narrower_type_count | 1 | None |
| domain-human-resources | has_narrower_types | True | None |
| domain-human-resources | broader_is_detached | True | None |
| domain-operations | name | Operations | None |
| domain-operations | narrower_type_count | 1 | None |
| domain-operations | has_narrower_types | True | None |
| ... | ... | (38 more) | ... |

### procedures

- Fields: 454/660 (68.8%)
- Computed columns: name, execution_count, is_template_instance, target_count, adoption_count, specified_step_total, has_no_explicit_steps, called_by_step_count, is_nested_procedure, failure_criterion_count, has_no_failure_criterion, lens_view_count, privileges_single_stakeholder_view, step_level_view_count, category_level_view_count, procedure_type_rank, procedure_type_definition, has_step_and_category_resolutions, is_missing_demanded_resolution, is_inconsistently_categorized, strategic_alignment_count, has_no_stated_reason_for_existing, outcome_measure_count, unlinked_measure_count, measures_performance_without_business_link, hindered_by_count, is_hindered_by_another_operation, type_distinguishing_facet, type_distinguishing_value, matching_distinction_count, lacks_type_distinction, lacks_distinction_type_key, managed_vocabulary_count, lacks_managed_controlled_vocabulary, compliance_review_step_count, involves_compliance_review_role, is_regulated_but_unformalized, agent_intended_count, is_tacit_only_agent_target, repository_entry_count, stale_entry_count, departed_only_know_how_count, has_decayed_transfer_channel, execution_feedback_entry_count, is_executed_without_feedback_loop, machine_authored_entry_count, practitioner_relationship_count, is_captured_by_automation_alone, unengaged_stakeholder_count, has_unengaged_stakeholder, recent_starter_count, untransferred_veteran_know_how_count, departing_veteran_know_how_count, has_transfer_shortfall_exposure, proficient_with_capture_count, proficient_without_capture_count, days_with_capture_total, days_without_capture_total, avg_days_to_proficiency_with_capture, avg_days_to_proficiency_without_capture, formalization_does_not_ease_onboarding, collected_material_count, in_work_capture_count, is_capture_separate_from_work, expert_acquisition_hours, compliance_document_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| conveyor-maintenance | name | Conveyor Line Preventive Maint | None |
| conveyor-maintenance | specified_step_total | 3 | None |
| conveyor-maintenance | has_no_failure_criterion | True | None |
| conveyor-maintenance | lens_view_count | 2 | None |
| conveyor-maintenance | step_level_view_count | 1 | None |
| conveyor-maintenance | procedure_type_rank | ProcessType | None |
| conveyor-maintenance | procedure_type_definition | A regulated procedure that pro | None |
| conveyor-maintenance | has_step_and_category_resolutions | True | None |
| conveyor-maintenance | strategic_alignment_count | 1 | None |
| conveyor-maintenance | outcome_measure_count | 1 | None |
| conveyor-maintenance | unlinked_measure_count | 1 | None |
| conveyor-maintenance | measures_performance_without_business_link | True | None |
| conveyor-maintenance | type_distinguishing_facet | facet-hazard | None |
| conveyor-maintenance | type_distinguishing_value | HazardousEnergy | None |
| conveyor-maintenance | matching_distinction_count | 1 | None |
| conveyor-maintenance | lacks_managed_controlled_vocabulary | True | None |
| conveyor-maintenance | agent_intended_count | 1 | None |
| conveyor-maintenance | repository_entry_count | 1 | None |
| conveyor-maintenance | machine_authored_entry_count | 1 | None |
| conveyor-maintenance | is_captured_by_automation_alone | True | None |
| ... | ... | (186 more) | ... |

### procedure_versions

- Fields: 1090/1611 (67.7%)
- Computed columns: name, count_of_steps, count_of_open_knowledge_gaps, is_ready_for_execution, specified_step_count, overdue_review_count, open_change_request_count, open_high_severity_gap_count, is_fit_to_execute, steward_review_cadence_days, count_of_stewardship_assignments, has_any_steward, is_live, is_unstewarded, is_live_and_unstewarded, count_of_open_blocking_gaps, has_open_blocking_gap, is_live_with_blocking_gap, should_not_be_executable, count_of_unapproved_reliance_fragments, runs_on_unapproved_knowledge, count_of_overdue_gaps, count_of_change_requests, count_of_review_events, has_governance_record, as_of_instant, days_since_modified, days_since_last_review, was_modified_since_last_review, modifier_is_authority, has_unwitnessed_change, count_of_stale_fragments, knowledge_is_staler_than_cadence, compound_fragile_fragment_count, rests_on_compound_fragile_knowledge, concentrated_witness_session_count, knowledge_base_is_concentrated, machine_consumed_unapproved_count, feeds_unapproved_knowledge_to_machines, genuinely_overdue_fragment_count, awaited_decision_count, scoped_open_blocking_gap_count, is_blocked_on_pending_decision, unexercised_human_gate_count, ai_boundary_is_unevidenced, load_bearing_unapproved_count, unlanded_decision_count, unrehearsed_control_entry_count, has_unrehearsed_control_entry, is_live_with_unrehearsed_control, cadence_breach_count, is_in_cadence_breach, has_decision_in_flight, is_unremediated_cadence_breach, is_managed_cadence_breach, governance_is_silent, valid_fragment_count, still_owns_valid_knowledge, incoming_supersession_count, is_still_referenced, is_load_bearing_orphan, is_cleanly_retired, stalled_implementation_count, is_held_unfit_by_landed_decisions, undeclared_control_kind_count, control_taxonomy_is_incomplete, has_approved_change_request, approved_change_request_count, unwatched_unowned_control_count, mining_run_count, drifted_mining_run_count, has_unresolved_mining_drift, entry_step_id, execution_count, status_is_pko, uses_non_pko_status, exception_count, fallback_transition_count, alternative_transition_count, has_no_exception_handling, latest_source_document_modified_at, days_document_trails_version, document_lags_practice, human_step_count, non_human_step_count, mixes_human_and_software_steps, day_run_count, night_run_count, day_deviating_run_count, night_deviating_run_count, is_inconsistent_across_shifts, overlaps_relation_count, enables_relation_count, prevents_relation_count, steps_without_ontology_type_count, rests_on_notation_only, is_current_without_motivation, conditionless_step_count, is_under_specified_for_execution, coarse_top_level_step_count, fine_top_level_step_count, mixes_granularity_at_one_level, procedure_type_of_version, created_by_agent_kind, elicitation_session_count, expert_capture_count, elicitation_evidence_count, indexed_segment_count, search_count, successful_search_count, search_success_percent, open_question_annotation_count, is_inadequate_for_use, served_assertion_count, lacks_machine_interpretable_encoding, structured_query_count, profile_validated_submission_count, reasoned_assertion_count, is_not_query_validate_reason_ready, published_projection_count, consumer_sync_count, is_unreachable_knowledge, human_sync_count, machine_sync_count, human_channel_count, machine_channel_count, serves_only_humans_or_only_machines, fresh_mining_run_count, lacks_continuous_drift_detection, outcome_measurement_count, is_disconnected_from_outcomes, ai_contribution_count, ai_consumption_count, uses_ai_in_one_direction_only, is_unmodified_for_twelve_months, design_decision_count, is_live_without_recorded_decisions, ai_artifact_consuming_input_count, contains_steps_affected_by_ai_agent_change, interview_session_count, observation_session_count, workshop_session_count, protocol_session_count, incident_session_count, reconciled_divergence_count, complementary_method_count, relies_on_single_method, misses_a_required_elicitation_mode, critical_incident_count, judgment_unprobed_by_incidents, sme_approval_count, sme_ai_evaluation_count, is_approved_without_sme_signoff, experts_evaluate_ai_not_representation, ke_session_count, ke_field_session_count, is_studied_only_from_the_desk, judgment_held_outside_sop_count, tacit_holding_count, explicit_holding_count, tacit_share_exceeds_explicit, hands_held_count, negotiated_practice_count, lives_in_hands_silence_and_negotiation, process_model_trace_count, is_live_model_untraced, trailing_practice_trace_count, is_documented_behind_practice, tacit_fragment_count, is_standardized_without_tacit_capture, tacit_form_fragment_count, situated_judgment_fragment_count, tacit_judgment_fragment_count, first_step, fallback_step, first_step_disagrees_with_graph, declared_first_step_count, graph_entry_step_count, owner_organization

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-v1.0.0 | name | Quarter-End Financial Close v1 | None |
| close-v1.0.0 | is_unstewarded | True | None |
| close-v1.0.0 | count_of_open_blocking_gaps | 1 | None |
| close-v1.0.0 | has_open_blocking_gap | True | None |
| close-v1.0.0 | count_of_unapproved_reliance_fragments | 2 | None |
| close-v1.0.0 | runs_on_unapproved_knowledge | True | None |
| close-v1.0.0 | count_of_overdue_gaps | 2 | None |
| close-v1.0.0 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| close-v1.0.0 | days_since_modified | 110 | None |
| close-v1.0.0 | modifier_is_authority | Human | None |
| close-v1.0.0 | count_of_stale_fragments | 12 | None |
| close-v1.0.0 | knowledge_is_staler_than_cadence | True | None |
| close-v1.0.0 | incoming_supersession_count | 1 | None |
| close-v1.0.0 | is_still_referenced | True | None |
| close-v1.0.0 | is_load_bearing_orphan | True | None |
| close-v1.0.0 | unwatched_unowned_control_count | 9 | None |
| close-v1.0.0 | mining_run_count | 1 | None |
| close-v1.0.0 | status_is_pko | True | None |
| close-v1.0.0 | is_under_specified_for_execution | True | None |
| close-v1.0.0 | procedure_type_of_version | financial-sop | None |
| ... | ... | (501 more) | ... |

### procedure_version_links

- Fields: 0/4 (0.0%)
- Computed columns: name, superseded_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-v1.0.0-to-v1.1.0 | name | close-v1.0.0 -> close-v1.1.0 | None |
| close-v1.0.0-to-v1.1.0 | superseded_version_key | close-v1.0.0 | None |
| loto-v1.0.0-to-v2.0.0 | name | loto-v1.0.0 -> loto-v2.0.0 | None |
| loto-v1.0.0-to-v2.0.0 | superseded_version_key | loto-v1.0.0 | None |

### procedure_status_changes

- Fields: 24/39 (61.5%)
- Computed columns: name, is_unattributed_change, change_kind_contradicts_target

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| status-close-approval-approved | name | close-v1.1.0: Approval -> Appr | None |
| status-close-draft-validation | name | close-v1.1.0: Draft -> Validat | None |
| status-close-validation-approval | name | close-v1.1.0: Validation -> Ap | None |
| status-convmaint-publish | name | convmaint-v1.0.0: Draft -> Pub | None |
| status-convmaint-publish | is_unattributed_change | True | None |
| status-convmaint-publish | change_kind_contradicts_target | True | None |
| status-exec-loto-0716-pause | name | loto-v2.0.0: InProgress -> Pau | None |
| status-loto1-archive | name | loto-v1.0.0: Approved -> Depre | None |
| status-loto2-approve | name | loto-v2.0.0: Approval -> Appro | None |
| status-loto2-create | name | loto-v2.0.0:  -> Draft | None |
| status-loto2-extract | name | loto-v2.0.0: Draft -> Draft | None |
| status-loto2-to-approval | name | loto-v2.0.0: Validation -> App | None |
| status-loto2-validate | name | loto-v2.0.0: Draft -> Validati | None |
| status-policy-draft-validation | name | policy-v1.0.0: Draft -> Valida | None |
| status-policy-validation-approved | name | policy-v1.0.0: Validation -> A | None |

### steps

- Fields: 3386/4469 (75.8%)
- Computed columns: name, assigned_role_label, assigned_agent_kind, blocking_requirement_count, stale_binding_count, authoritative_stale_count, available_exception_count, declared_verification_count, is_preparation_step, is_approval_step, stale_authoritative_binding_count, inputs_are_fresh, is_software_assigned, is_human_approval_gate, gate_held_by_human, binding_boundary_count, assigned_role_is_ungoverned, unusable_binding_count, all_sources_usable, unwarranted_boundary_count, is_governed_by_unwarranted_boundary, software_execution_count, has_been_approached_by_software, is_unexercised_human_gate, is_demonstrated_human_gate, unexercised_gate_version_key, has_declared_control_kind, undeclared_control_version_key, approval_step_is_software_assigned, unwitnessed_blocking_count, reachable_step_count, reached_from_step_count, self_reach_count, is_on_rework_loop, is_blocking_control_on_rework_loop, incoming_transition_count, is_entry_step, entry_step_key, version_entry_step_id, gate_free_reach_from_entry_count, is_reachable_from_entry_without_human_gate, is_gate_bypassed_publication, child_step_count, parent_step_kind, is_composite_without_children, precondition_count, postcondition_count, invariant_count, safety_critical_condition_count, failure_mode_count, cue_count, danger_cue_count, decision_point_count, knowledge_fragment_count, has_instruction_only, input_variable_count, output_variable_count, required_lock_count, required_protective_equipment_count, is_isolation_without_lock, referenced_resource_count, untyped_version_key, is_accountable_to_software, has_downstream_steps, incompleteness_cue_count, has_incompleteness_cue, has_postcondition, accountable_agent, has_no_accountable_agent, conditionless_version_key, prerequisite_downstream_count, prerequisite_is_downstream, states_operational_knowledge, bottleneck_allocation_count, is_bottleneck_step, coarse_top_level_version_key, fine_top_level_version_key, context_sensitivity_count, unscoped_sensitivity_count, is_context_sensitive_but_unscoped, version_procedure, assigned_role_does_compliance_review, compliance_review_procedure_key, step_procedure_type, is_release_approval_gate, regulatory_requirement_count, tool_function_count, parseable_condition_count, dmn_decision_count, tool_use_rules_only_in_prose, open_outdated_flag_count, has_reported_reality_mismatch, deviated_run_count, is_drifted_from_practice, ai_failure_count, is_ai_failure_point, ai_artifact_input_count, consumes_ai_agent_artifact, collection_evidence_count, has_collection_evidence, activity_origin_trace_count, version_model_trace_count, is_untraced_activity_in_traced_model, elicited_validation_count, elicited_extension_count, downstream_artifact_step_count, declared_first_step_key, declared_fallback_step_key, owner_organization

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-01 | name | 01. Freeze transaction entry | None |
| close-01 | assigned_role_label | Close Automation Operator | None |
| close-01 | assigned_agent_kind | AutomatedPipeline | None |
| close-01 | blocking_requirement_count | 1 | None |
| close-01 | declared_verification_count | 1 | None |
| close-01 | inputs_are_fresh | True | None |
| close-01 | is_software_assigned | True | None |
| close-01 | all_sources_usable | True | None |
| close-01 | software_execution_count | 1 | None |
| close-01 | has_been_approached_by_software | True | None |
| close-01 | has_declared_control_kind | True | None |
| close-01 | reachable_step_count | 7 | None |
| close-01 | is_entry_step | True | None |
| close-01 | entry_step_key | close-01 | None |
| close-01 | version_entry_step_id | close-01 | None |
| close-01 | has_instruction_only | True | None |
| close-01 | is_accountable_to_software | True | None |
| close-01 | has_downstream_steps | True | None |
| close-01 | accountable_agent | close-pipeline | None |
| close-01 | conditionless_version_key | close-v1.1.0 | None |
| ... | ... | (1063 more) | ... |

### step_transitions

- Fields: 541/860 (62.9%)
- Computed columns: name, is_recovery_path, count_of_from_step_executions, count_of_to_step_executions, has_reachable_origin, has_reachable_target, is_never_exercised, is_untested_recovery_path, count_of_observed_traversals, has_been_traversed, is_unwalked_recovery_path, target_blocking_requirement_count, target_carries_blocking_control, is_unrehearsed_control_entry, unrehearsed_control_version_key, from_step_is_human_approval_gate, to_step_is_human_approval_gate, avoids_human_approval_gate, decision_point_count, is_undocumented_branch

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-01-to-close-02 | name | close-01 -> close-02 | None |
| close-01-to-close-02 | count_of_from_step_executions | 1 | None |
| close-01-to-close-02 | count_of_to_step_executions | 1 | None |
| close-01-to-close-02 | has_reachable_origin | True | None |
| close-01-to-close-02 | has_reachable_target | True | None |
| close-01-to-close-02 | count_of_observed_traversals | 1 | None |
| close-01-to-close-02 | has_been_traversed | True | None |
| close-01-to-close-02 | avoids_human_approval_gate | True | None |
| close-02-to-close-03 | name | close-02 -> close-03 | None |
| close-02-to-close-03 | count_of_from_step_executions | 1 | None |
| close-02-to-close-03 | count_of_to_step_executions | 1 | None |
| close-02-to-close-03 | has_reachable_origin | True | None |
| close-02-to-close-03 | has_reachable_target | True | None |
| close-02-to-close-03 | count_of_observed_traversals | 1 | None |
| close-02-to-close-03 | has_been_traversed | True | None |
| close-02-to-close-03 | target_blocking_requirement_count | 2 | None |
| close-02-to-close-03 | target_carries_blocking_control | True | None |
| close-02-to-close-03 | avoids_human_approval_gate | True | None |
| close-03-to-close-04 | name | close-03 -> close-04 | None |
| close-03-to-close-04 | count_of_from_step_executions | 1 | None |
| ... | ... | (299 more) | ... |

### actions

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| apply-lock | name | Apply personal lock and tag | None |
| approve-close | name | Approve close | None |
| approve-policy | name | Approve employment policy | None |
| freeze-ledgers | name | Freeze ledgers | None |
| interview-practitioner | name | Interview practitioner | None |
| investigate-variance | name | Investigate variance | None |
| reconcile-account | name | Reconcile account | None |
| review-close-package | name | Review close package | None |
| review-feedback | name | Review procedural feedback | None |
| review-policy | name | Review employment policy | None |

### functions

- Fields: 0/11 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| classify-change-risk | name | Classify change risk | None |
| classify-variance | name | Classify variance | None |
| collect-acknowledgement | name | Collect acknowledgement | None |
| draft-policy | name | Draft policy | None |
| extract-trial-balance | name | Extract trial balance | None |
| fetch-incident-history | name | Fetch incident history | None |
| post-close-entries | name | Post close entries | None |
| render-email | name | Render email | None |
| render-sms | name | Render SMS | None |
| reserve-padlocks | name | Reserve padlocks | None |
| send-notification | name | Send notification | None |

### tools

- Fields: 0/9 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-workpaper | name | Close Workpaper Workbook | None |
| consent-registry | name | Employee Consent Registry | None |
| email-gateway | name | Email Gateway | None |
| erp-ledger | name | ERP General Ledger | None |
| knowledge-capture-form | name | Knowledge Capture Form | None |
| policy-registry | name | Policy Registry | None |
| pressure-gauge | name | Pressure gauge | None |
| sms-gateway | name | SMS Gateway | None |
| variance-model | name | Variance AI Console | None |

### step_actions

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sa-close01 | name | close-01 / freeze-ledgers | None |
| sa-close03 | name | close-03 / reconcile-account | None |
| sa-close04 | name | close-04 / investigate-varianc | None |
| sa-close05 | name | close-05 / review-close-packag | None |
| sa-close06 | name | close-06 / approve-close | None |
| sa-loto05-applylock | name | loto-05 / apply-lock | None |
| sa-policy02 | name | policy-02 / interview-practiti | None |
| sa-policy04 | name | policy-04 / review-policy | None |
| sa-policy05 | name | policy-05 / approve-policy | None |
| sa-policy09 | name | policy-09 / review-feedback | None |

### step_functions

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sf-close02 | name | close-02 / extract-trial-balan | None |
| sf-close04 | name | close-04 / classify-variance | None |
| sf-close07 | name | close-07 / post-close-entries | None |
| sf-deploy02-classify | name | deploy-02 / classify-change-ri | None |
| sf-loto05-reserve-padlocks | name | loto-05 / reserve-padlocks | None |
| sf-policy03 | name | policy-03 / draft-policy | None |
| sf-policy06-email | name | policy-06 / render-email | None |
| sf-policy06-sms | name | policy-06 / render-sms | None |
| sf-policy07 | name | policy-07 / send-notification | None |
| sf-policy08 | name | policy-08 / collect-acknowledg | None |

### step_tools

- Fields: 0/11 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| st-close01 | name | close-01 / erp-ledger | None |
| st-close02 | name | close-02 / erp-ledger | None |
| st-close03 | name | close-03 / close-workpaper | None |
| st-close04a | name | close-04 / variance-model | None |
| st-close04b | name | close-04 / close-workpaper | None |
| st-loto06-gauge | name | loto-06 / pressure-gauge | None |
| st-policy02 | name | policy-02 / knowledge-capture- | None |
| st-policy03 | name | policy-03 / policy-registry | None |
| st-policy07a | name | policy-07 / consent-registry | None |
| st-policy07b | name | policy-07 / email-gateway | None |
| st-policy07c | name | policy-07 / sms-gateway | None |

### requirements

- Fields: 292/544 (53.7%)
- Computed columns: name, satisfaction_record_count, step_binding_count, is_bound_to_any_step, has_ever_been_evaluated, negative_outcome_count, is_inoperative_control, is_decorative_control, has_ever_produced_negative, is_unfalsified_control, claims_a_witness_field, named_witness_field_exists, derived_has_computed_witness, witness_claim_is_unverified, is_unwitnessed_blocking_control, witness_fire_count, witness_has_never_fired, evaluation_sample_size, has_meaningful_sample, is_untested_witness, is_evidenced_holding_control, control_assurance_state, unexercised_binding_count, witness_is_partially_scoped, accountable_agent, has_named_owner, is_orphaned_blocking_control, is_unwatched_and_unowned, attestation_exposure_note, unwatched_unowned_flag, uses_controlled_vocabulary, is_regulatory_requirement, constraint_trace_count, is_untraced_bound_constraint

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| req-close-balance | name | Reconciled balance integrity | None |
| req-close-balance | satisfaction_record_count | 1 | None |
| req-close-balance | step_binding_count | 1 | None |
| req-close-balance | is_bound_to_any_step | True | None |
| req-close-balance | has_ever_been_evaluated | True | None |
| req-close-balance | is_unfalsified_control | True | None |
| req-close-balance | claims_a_witness_field | True | None |
| req-close-balance | named_witness_field_exists | True | None |
| req-close-balance | derived_has_computed_witness | True | None |
| req-close-balance | witness_has_never_fired | True | None |
| req-close-balance | evaluation_sample_size | 1 | None |
| req-close-balance | has_meaningful_sample | True | None |
| req-close-balance | is_evidenced_holding_control | True | None |
| req-close-balance | control_assurance_state | Holding | None |
| req-close-balance | unexercised_binding_count | 1 | None |
| req-close-balance | witness_is_partially_scoped | True | None |
| req-close-balance | is_orphaned_blocking_control | True | None |
| req-close-balance | attestation_exposure_note | Witnessed but unowned: no name | None |
| req-close-balance | uses_controlled_vocabulary | True | None |
| req-close-balance | is_untraced_bound_constraint | True | None |
| ... | ... | (232 more) | ... |

### step_requirements

- Fields: 61/198 (30.8%)
- Computed columns: name, requirement_is_blocking, blocking_step_key, step_when_blocking, requirement_lacks_witness, unwitnessed_step_key, satisfaction_count_for_binding, binding_was_ever_exercised, is_unexercised_blocking_binding, unexercised_binding_requirement_key, requirement_is_regulatory

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sr-c01 | name | close-01 / req-close-cutoff | None |
| sr-c01 | requirement_is_blocking | True | None |
| sr-c01 | blocking_step_key | close-01 | None |
| sr-c01 | step_when_blocking | close-01 | None |
| sr-c01 | is_unexercised_blocking_binding | True | None |
| sr-c01 | unexercised_binding_requirement_key | req-close-cutoff | None |
| sr-c03a | name | close-03 / req-close-balance | None |
| sr-c03a | requirement_is_blocking | True | None |
| sr-c03a | blocking_step_key | close-03 | None |
| sr-c03a | step_when_blocking | close-03 | None |
| sr-c03a | is_unexercised_blocking_binding | True | None |
| sr-c03a | unexercised_binding_requirement_key | req-close-balance | None |
| sr-c03b | name | close-03 / req-close-evidence | None |
| sr-c03b | requirement_is_blocking | True | None |
| sr-c03b | blocking_step_key | close-03 | None |
| sr-c03b | step_when_blocking | close-03 | None |
| sr-c03b | is_unexercised_blocking_binding | True | None |
| sr-c03b | unexercised_binding_requirement_key | req-close-evidence | None |
| sr-c05 | name | close-05 / req-close-separatio | None |
| sr-c05 | requirement_is_blocking | True | None |
| ... | ... | (117 more) | ... |

### step_verifications

- Fields: 0/11 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| verify-ack | name | policy-08 / SignalVerification | None |
| verify-cfo | name | close-06 / ApprovalVerificatio | None |
| verify-close-cutoff | name | close-01 / SignalVerification | None |
| verify-controller | name | close-05 / ApprovalVerificatio | None |
| verify-feed-time | name | close-02 / SignalVerification | None |
| verify-legal | name | policy-04 / ApprovalVerificati | None |
| verify-policy-owner | name | policy-05 / ApprovalVerificati | None |
| verify-policy-source | name | policy-03 / ProvenanceVerifica | None |
| verify-quiet-hours | name | policy-07 / SignalVerification | None |
| verify-reconciliation | name | close-03 / SignalVerification | None |
| verify-sms-consent | name | policy-07 / SignalVerification | None |

### rationales

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rat-close-ai | name | Why AI ranks but does not clos | None |
| rat-close-separation | name | Why the preparer cannot approv | None |
| rat-policy-ai | name | Why AI may draft but not appro | None |
| rat-policy-multichannel | name | Why both email and SMS exist | None |

### exceptions

- Fields: 0/8 (0.0%)
- Computed columns: name, active_exception_step_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exc-cfo-unavailable | name | CFO unavailable during close | None |
| exc-cfo-unavailable | active_exception_step_key | close-06 | None |
| exc-ledger-outage | name | ERP source unavailable | None |
| exc-ledger-outage | active_exception_step_key | close-02 | None |
| exc-no-sms-consent | name | Recipient lacks SMS consent | None |
| exc-no-sms-consent | active_exception_step_key | policy-07 | None |
| exc-unreachable | name | Recipient unreachable | None |
| exc-unreachable | active_exception_step_key | policy-08 | None |

### resources

- Fields: 123/190 (64.7%)
- Computed columns: name, is_approved_source, source_modified_at, is_stale_extraction, referencing_step_count, referencing_version_count, is_unused_resource, is_content_without_organization, trailing_practice_count, is_behind_current_practice

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| res-change-ticket | name | Policy Change Ticket HR-4821 | None |
| res-change-ticket | referencing_version_count | 1 | None |
| res-change-ticket | is_content_without_organization | True | None |
| res-close-checklist | name | Close Control Checklist | None |
| res-close-checklist | referencing_version_count | 1 | None |
| res-close-checklist | is_content_without_organization | True | None |
| res-close-sop-pdf | name | Quarter-End Close SOP PDF | None |
| res-close-sop-pdf | referencing_version_count | 1 | None |
| res-close-sop-pdf | is_content_without_organization | True | None |
| res-consent-registry | name | Employee Communication Consent | None |
| res-consent-registry | referencing_version_count | 1 | None |
| res-consent-registry | is_content_without_organization | True | None |
| res-deploy-runbook | name | Production deployment runbook | None |
| res-deploy-runbook | is_approved_source | True | None |
| res-deploy-runbook | referencing_version_count | 1 | None |
| res-deploy-runbook | trailing_practice_count | 1 | None |
| res-deploy-runbook | is_behind_current_practice | True | None |
| res-email-template | name | Employee Policy Email Template | None |
| res-email-template | referencing_version_count | 1 | None |
| res-email-template | is_content_without_organization | True | None |
| ... | ... | (47 more) | ... |

### procedure_resources

- Fields: 0/36 (0.0%)
- Computed columns: name, relation_iri, resource_modified_at

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pr-close-api | name | close-v1.1.0 / res-erp-api | None |
| pr-close-api | relation_iri | http://purl.org/dc/terms/refer | None |
| pr-close-api | resource_modified_at | 2026-07-19T08:00:00-05:00 | None |
| pr-close-check | name | close-v1.1.0 / res-close-check | None |
| pr-close-check | relation_iri | http://purl.org/dc/terms/refer | None |
| pr-close-check | resource_modified_at | 2026-07-02T15:00:00-05:00 | None |
| pr-close-source | name | close-v1.1.0 / res-close-sop-p | None |
| pr-close-source | relation_iri | https://w3id.org/pko#wasExtrac | None |
| pr-close-source | resource_modified_at | 2026-04-01T09:00:00-05:00 | None |
| pr-deploy-runbook | name | deploy-v3.2.0 / res-deploy-run | None |
| pr-deploy-runbook | relation_iri | https://w3id.org/pko#wasExtrac | None |
| pr-deploy-runbook | resource_modified_at | 2026-01-02T09:00:00-05:00 | None |
| pr-loto2-sop | name | loto-v2.0.0 / res-loto-sop-201 | None |
| pr-loto2-sop | relation_iri | https://w3id.org/pko#wasExtrac | None |
| pr-loto2-sop | resource_modified_at | 2019-03-01T09:00:00-05:00 | None |
| pr-loto2-spec | name | loto-v2.0.0 / res-loto-v2-spec | None |
| pr-loto2-spec | relation_iri | http://purl.org/dc/terms/refer | None |
| pr-loto2-spec | resource_modified_at | 2026-05-20T09:00:00-05:00 | None |
| pr-policy-consent | name | policy-v1.0.0 / res-consent-re | None |
| pr-policy-consent | relation_iri | http://purl.org/dc/terms/refer | None |
| ... | ... | (16 more) | ... |

### elicitation_sessions

- Fields: 234/377 (62.1%)
- Computed columns: name, as_of_instant, days_since_elicited, is_single_witness_method, practitioner_is_still_engaged, valid_fragments_produced, is_high_yield_session, is_concentrated_single_witness, is_stale_concentrated_witness, concentrated_session_version_key, elicitation_mode, is_interview, is_workshop, why_probe_count, shortfall_probe_count, is_interview_without_why_probe, is_interview_without_shortfall_probe, initiator_count, executor_count, dependent_count, uninvited_participant_count, gathers_whole_process_chain, is_workshop_without_usual_outsiders, method_family, facilitator_knowledge_engineer_role_count, facilitator_is_knowledge_engineer, is_generic_or_unskilled_capture, is_ke_field_session, is_reviewed_recording

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| el7-deploy-interview | name | PractitionerInterview / 2026-0 | None |
| el7-deploy-interview | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| el7-deploy-interview | days_since_elicited | 27 | None |
| el7-deploy-interview | is_single_witness_method | True | None |
| el7-deploy-interview | elicitation_mode | Interview | None |
| el7-deploy-interview | is_interview | True | None |
| el7-deploy-interview | why_probe_count | 1 | None |
| el7-deploy-interview | is_interview_without_shortfall_probe | True | None |
| el7-deploy-interview | method_family | Elicitation | None |
| el7-deploy-interview | facilitator_knowledge_engineer_role_count | 1 | None |
| el7-deploy-interview | facilitator_is_knowledge_engineer | True | None |
| el7-loto-cit | name | CriticalIncidentTechnique / 20 | None |
| el7-loto-cit | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| el7-loto-cit | days_since_elicited | 117 | None |
| el7-loto-cit | valid_fragments_produced | 1.0 | None |
| el7-loto-cit | elicitation_mode | Incident | None |
| el7-loto-cit | method_family | Elicitation | None |
| el7-loto-cit | facilitator_knowledge_engineer_role_count | 1 | None |
| el7-loto-cit | facilitator_is_knowledge_engineer | True | None |
| el7-loto-cit | is_ke_field_session | True | None |
| ... | ... | (123 more) | ... |

### knowledge_fragments

- Fields: 436/828 (52.7%)
- Computed columns: name, as_of_instant, is_currently_valid, source_agent_is_still_engaged, source_agent_kind, has_human_source, has_orphaned_provenance, is_undefendable_tacit_claim, is_approved, is_within_validity_window, is_relied_upon, step_procedure_version_status, is_attached_to_live_version, is_unapproved_but_relied_on, evidence_age_days, has_recorded_elicitation, is_from_single_witness, evidence_expiry_days, evidence_has_expired, owner_agent, is_awaiting_approval, owner_is_me, is_my_unfinished_approval, is_invoked_by_an_exception, has_operational_reliance, is_unapproved_and_operationally_live, age_days, is_low_confidence, owning_version_cadence_days, exceeds_owning_cadence, is_aging_low_confidence_claim, owner_role_agent_kind, is_human_owned, is_ai_validated_by_ai, review_cadence_days, is_overdue_for_review, predates_current_role_holder, owner_role_assignment_valid_from, fragility_signal_count, is_compound_fragile, is_single_point_of_failure, is_expiring_single_point_of_failure, compound_fragile_version_key, valid_fragment_session_key, consuming_step_is_software_assigned, consuming_step_agent_kind, is_unapproved_and_machine_consumed, is_unapproved_and_human_consumed, machine_consumed_unapproved_version_key, has_review_record, days_since_actual_review, is_unreviewed_since_authoring, is_genuinely_overdue, review_recency_is_inferred, inference_disagrees_with_record, genuinely_overdue_version_key, ratified_boundary_count, reliance_surface_count, days_awaiting_my_approval, is_high_blast_radius_unapproved, is_long_unapproved, unapproved_load_bearing_version_key, owner_role_is_vacated, is_orphaned_by_role, valid_fragment_version_key, is_flattened_to_brittle_rule, corroboration_count, rests_on_single_data_point, owner_organization

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kf-close-fx-time | name | Tacit: When FX variance spikes | None |
| kf-close-fx-time | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kf-close-fx-time | is_currently_valid | True | None |
| kf-close-fx-time | source_agent_is_still_engaged | True | None |
| kf-close-fx-time | source_agent_kind | Human | None |
| kf-close-fx-time | has_human_source | True | None |
| kf-close-fx-time | is_approved | True | None |
| kf-close-fx-time | is_within_validity_window | True | None |
| kf-close-fx-time | is_relied_upon | True | None |
| kf-close-fx-time | step_procedure_version_status | close-v1.1.0 | None |
| kf-close-fx-time | is_attached_to_live_version | True | None |
| kf-close-fx-time | evidence_age_days | 107 | None |
| kf-close-fx-time | has_recorded_elicitation | True | None |
| kf-close-fx-time | is_from_single_witness | True | None |
| kf-close-fx-time | evidence_expiry_days | 180 | None |
| kf-close-fx-time | owner_agent | devon-okafor | None |
| kf-close-fx-time | age_days | 107 | None |
| kf-close-fx-time | owning_version_cadence_days | 90 | None |
| kf-close-fx-time | exceeds_owning_cadence | True | None |
| kf-close-fx-time | owner_role_agent_kind | Human | None |
| ... | ... | (372 more) | ... |

### knowledge_gaps

- Fields: 204/345 (59.1%)
- Computed columns: name, is_open, open_gap_version_key, is_blocking, is_open_and_blocking, as_of_instant, days_open, tolerance_days, is_overdue_gap, owner_agent, owner_is_still_engaged, has_resolution_plan, is_abandoned_unknown, open_blocking_gap_version_key, owner_role_is_vacated, is_ownerless_open_gap, is_known_and_unresolved, is_gatekeeping_or_sabotage, is_unattributed_gatekeeping, is_required_gatekept_uncodified, owner_organization, answering_change_title, answering_change_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gap-classifier-training-dependency | name | Medium: Retraining and evaluat | None |
| gap-classifier-training-dependency | is_open | True | None |
| gap-classifier-training-dependency | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| gap-classifier-training-dependency | days_open | 39 | None |
| gap-classifier-training-dependency | tolerance_days | 90 | None |
| gap-classifier-training-dependency | owner_agent | omar-haddad | None |
| gap-classifier-training-dependency | owner_is_still_engaged | True | None |
| gap-classifier-training-dependency | is_known_and_unresolved | True | None |
| gap-close-cadence-unmeasured | name | Medium: StewardshipAssignments | None |
| gap-close-cadence-unmeasured | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| gap-close-cadence-unmeasured | tolerance_days | 90 | None |
| gap-close-cadence-unmeasured | owner_agent | elena-garcia | None |
| gap-close-cadence-unmeasured | owner_is_still_engaged | True | None |
| gap-close-cadence-unmeasured | has_resolution_plan | True | None |
| gap-close-cadence-unmeasured | owner_organization | acme-finance | None |
| gap-close-dual-outage | name | High: No validated fallback ex | None |
| gap-close-dual-outage | is_blocking | True | None |
| gap-close-dual-outage | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| gap-close-dual-outage | tolerance_days | 30 | None |
| gap-close-dual-outage | owner_agent | devon-okafor | None |
| ... | ... | (121 more) | ... |

### fa_qs

- Fields: 4/12 (33.3%)
- Computed columns: name, resolution_count, is_unused_faq

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| faq-close-workpapers | name | Where are close workpapers sto | None |
| faq-close-workpapers | resolution_count | 1 | None |
| faq-loto-gauge | name | What if the gauge reads slight | None |
| faq-loto-gauge | resolution_count | 1 | None |
| faq-policy-ai | name | Can the drafting AI approve a  | None |
| faq-policy-ai | resolution_count | 1 | None |
| faq-policy-optout | name | What happens when an employee  | None |
| faq-policy-optout | is_unused_faq | True | None |

### explanations

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exp-close-variance | name | Variance escalation explanatio | None |
| exp-policy-channel | name | Channel eligibility explanatio | None |

### procedure_executions

- Fields: 621/870 (71.4%)
- Computed columns: name, expected_step_count, completed_step_count, control_breach_count, late_step_count, is_structurally_complete, diverged_from_specification, all_blocking_controls_evaluated, unevaluated_blocking_total, separation_of_duties_held, separation_violation_count, is_attestation_ready, attestation_blocker_summary, executed_version_is_fit, signed_against_unfit_version, asserted_only_control_count, assurance_is_mostly_asserted, unreachable_handling_failure_count, retention_breach_count, cleared_legal_review_count, has_cleared_legal_review, abandoned_failure_count, delivered_count, total_delivery_attempt_count, has_abandoned_failures, mishandled_refusal_count, unclean_step_count, ran_clean, count_of_approval_executions, has_human_approval, count_of_delivery_executions, has_delivered, delivered_without_approval, invalid_approval_count, approval_chain_is_complete, vacuously_clean_step_count, preparation_step_count, approval_step_count, separation_was_testable, separation_held_under_test, separation_is_vacuously_green, separation_assurance_note, ungoverned_divergence_count, divergence_was_fully_governed, computedly_witnessed_control_count, evaluated_control_count, computed_assurance_ratio, interested_party_assertion_count, assurance_grade, attestation_would_be_weakly_based, independent_human_observation_count, has_any_independent_observation, self_attested_approval_count, assurance_chain_is_circular, latest_attestation_instant, has_been_attested, attestation_count, post_attestation_score_count, basis_changed_after_signature, requires_re_attestation, intended_recipient_count, reached_recipient_count, silently_dropped_count, delivery_yield_percent, campaign_silently_lost_audience, unrecorded_refusal_count, has_unrecorded_refusals, independently_confirmed_intent_count, send_decisions_are_entirely_self_witnessed, participant_count, deviating_step_count, has_step_deviation, deviating_facility_key, clean_facility_key, deviating_day_version_key, deviating_night_version_key, status_change_count, claims_completion_without_all_steps, is_unconfirmed_completion, has_no_recorded_outcome, feedback_count, is_unreported_mistake, owner_organization, stopped_at_gap_statement, stopped_at_gap_status, stopped_at_gap_change_title, stopped_at_gap_change_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exec-close-2026-q2 | name | close-v1.1.0 / Quarter ended 2 | None |
| exec-close-2026-q2 | expected_step_count | 8 | None |
| exec-close-2026-q2 | completed_step_count | 8 | None |
| exec-close-2026-q2 | control_breach_count | 2 | None |
| exec-close-2026-q2 | late_step_count | 5 | None |
| exec-close-2026-q2 | is_structurally_complete | True | None |
| exec-close-2026-q2 | diverged_from_specification | True | None |
| exec-close-2026-q2 | all_blocking_controls_evaluated | True | None |
| exec-close-2026-q2 | separation_of_duties_held | True | None |
| exec-close-2026-q2 | attestation_blocker_summary | Control breach recorded on one | None |
| exec-close-2026-q2 | signed_against_unfit_version | True | None |
| exec-close-2026-q2 | asserted_only_control_count | 1 | None |
| exec-close-2026-q2 | assurance_is_mostly_asserted | True | None |
| exec-close-2026-q2 | unclean_step_count | 5 | None |
| exec-close-2026-q2 | count_of_approval_executions | 1 | None |
| exec-close-2026-q2 | has_human_approval | True | None |
| exec-close-2026-q2 | approval_chain_is_complete | True | None |
| exec-close-2026-q2 | vacuously_clean_step_count | 1 | None |
| exec-close-2026-q2 | preparation_step_count | 2 | None |
| exec-close-2026-q2 | approval_step_count | 2 | None |
| ... | ... | (229 more) | ... |

### step_executions

- Fields: 6208/8820 (70.4%)
- Computed columns: name, actual_duration_minutes, expected_duration_minutes, is_late, blocking_unmet_count, blocking_unmet_count_safe, proceeded_past_blocking_control, expected_blocking_count, evaluated_blocking_count, unevaluated_blocking_count, has_unevaluated_blocking_control, stale_authoritative_source_count, ran_on_stale_authoritative_source, has_deviation_note, is_late_and_unexplained, available_exception_count_for_step, had_uninvoked_exception_available, expected_verification_count, performed_verification_count, skipped_verification_count, has_skipped_verification, claims_pass_without_evidence, step_is_preparation, step_is_approval, preparer_agent_key, approver_agent_key, prepared_by_this_agent_count, violates_separation_of_duties, required_role_for_step, executor_role_key, executor_authority_count, executor_held_required_role, is_unauthorized_approval, completed_execution_key, control_breach_execution_key, late_execution_key, executor_agent_kind, executor_is_human, step_requires_human_confirmation, non_human_ran_human_step, non_human_approval, unevaluated_blocking_execution_key, separation_violation_execution_key, self_witnessed_verification_count, unbacked_verification_count, approval_rests_on_self_attestation, exception_invocation_count, ran_under_exception, is_completed, is_verification_passed, is_legal_review_step, cleared_legal_review_key, assigned_role, role_current_agent, executor_is_designated_agent, inputs_were_fresh_at_run, ran_on_stale_inputs, unresolved_issue_count, has_deviation, is_clean, procedure_execution_when_unclean, evaluated_requirement_count, required_blocking_count, has_unevaluated_blocking_requirement, executing_agent_kind, was_executed_by_software, step_is_software_assigned, software_did_human_work, is_approval_execution, is_verified, unconfirmed_non_human_decision_count, requires_human_confirmation, human_confirmation_missing, drafted_from_unusable_source, inputs_were_usable, software_execution_step_key, step_control_kind, unfalsified_clearance_count, all_clearances_are_unfalsified, stale_at_run_count, was_stale_when_i_ran_it, staleness_answer_is_tense_dependent, has_any_declared_check, performed_check_count, declared_check_count, is_unchecked_by_design, is_vacuously_clean, is_substantively_clean, vacuously_clean_execution_key, uncorroborated_pass_count, evidence_position_is_weak, preparation_execution_key, approval_execution_key, has_governing_instrument, has_approved_change_coverage, version_of_step, is_ungoverned_divergence, ungoverned_divergence_execution_key, self_attested_approval_execution_key, previous_executed_step, specified_transition_from_previous_count, is_out_of_specified_order, execution_version, executes_step_of_other_version, repetition_count, step_max_repetitions, exceeds_max_repetitions, lacks_required_confirmation, failed_precondition_count, violated_invariant_count, proceeded_despite_failed_precondition, unescalated_danger_cue_count, ignored_danger_cue, used_entity_count, generated_entity_count, step_input_variable_count, ran_without_declared_inputs, step_prerequisite, completed_prerequisite_run_count, ran_before_prerequisite_completed, deviation_execution_key, broke_invariant, is_blocked_by_incomplete_prerequisite, owner_organization, incomplete_cue_observation_count, is_blocked_by_observed_cue

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| se-close01 | name | exec-close-2026-q2 / close-01 | None |
| se-close01 | actual_duration_minutes | 4 | None |
| se-close01 | expected_duration_minutes | 5 | None |
| se-close01 | expected_blocking_count | 1 | None |
| se-close01 | evaluated_blocking_count | 1 | None |
| se-close01 | expected_verification_count | 1 | None |
| se-close01 | performed_verification_count | 1 | None |
| se-close01 | prepared_by_this_agent_count | 68.0 | None |
| se-close01 | required_role_for_step | close-automation | None |
| se-close01 | executor_role_key | close-pipeline|close-automatio | None |
| se-close01 | executor_authority_count | 1 | None |
| se-close01 | executor_held_required_role | True | None |
| se-close01 | completed_execution_key | exec-close-2026-q2 | None |
| se-close01 | executor_agent_kind | AutomatedPipeline | None |
| se-close01 | is_completed | True | None |
| se-close01 | is_verification_passed | True | None |
| se-close01 | assigned_role | close-automation | None |
| se-close01 | role_current_agent | close-pipeline | None |
| se-close01 | executor_is_designated_agent | True | None |
| se-close01 | inputs_were_fresh_at_run | True | None |
| ... | ... | (2592 more) | ... |

### requirement_satisfactions

- Fields: 130/304 (42.8%)
- Computed columns: name, requirement_is_blocking, is_fully_satisfied, is_blocking_and_unmet, blocking_unmet_step_key, blocking_satisfaction_step_key, negative_outcome_requirement_key, evaluator_agent_kind, non_human_evaluated_human_control, requirement_has_computed_witness, is_asserted_only, asserted_only_execution_key, parent_procedure_execution, step_execution_when_scored, is_human_evaluated, requirement_is_approval_type, is_invalid_approval, procedure_execution_of_satisfaction, run_when_invalid_approval, requirement_is_unfalsified, is_clearance_by_unfalsified_control, unfalsified_clearance_step_key, spec_step_of_execution, binding_key, scored_step_executor_agent, evaluator_is_step_executor, run_owner_agent, evaluator_owns_the_run, is_interested_party_assertion, has_written_evidence, is_bare_assertion, interested_assertion_execution_key, is_computedly_witnessed, computed_witness_execution_key, step_executor_agent, was_scored_after_attestation, attestation_instant_for_run, post_attestation_score_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sat-close-balance | name | req-close-balance / Satisfied | None |
| sat-close-balance | requirement_is_blocking | True | None |
| sat-close-balance | is_fully_satisfied | True | None |
| sat-close-balance | blocking_satisfaction_step_key | se-close03 | None |
| sat-close-balance | evaluator_agent_kind | Human | None |
| sat-close-balance | requirement_has_computed_witness | True | None |
| sat-close-balance | parent_procedure_execution | exec-close-2026-q2 | None |
| sat-close-balance | step_execution_when_scored | se-close03 | None |
| sat-close-balance | is_human_evaluated | True | None |
| sat-close-balance | requirement_is_approval_type | Verification | None |
| sat-close-balance | procedure_execution_of_satisfaction | exec-close-2026-q2 | None |
| sat-close-balance | requirement_is_unfalsified | True | None |
| sat-close-balance | is_clearance_by_unfalsified_control | True | None |
| sat-close-balance | unfalsified_clearance_step_key | se-close03 | None |
| sat-close-balance | spec_step_of_execution | close-03 | None |
| sat-close-balance | scored_step_executor_agent | maria-chen | None |
| sat-close-balance | run_owner_agent | devon-okafor | None |
| sat-close-balance | evaluator_owns_the_run | True | None |
| sat-close-balance | has_written_evidence | True | None |
| sat-close-balance | is_computedly_witnessed | True | None |
| ... | ... | (154 more) | ... |

### errors

- Fields: 4/16 (25.0%)
- Computed columns: name, remedy_step_count, occurrence_count, has_no_remedy_step

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| err-feed-stale | name | FEED-STALE - Source feed times | None |
| err-feed-stale | occurrence_count | 1 | None |
| err-feed-stale | has_no_remedy_step | True | None |
| err-lock-missing | name | LOTO-LM - Personal lock missin | None |
| err-lock-missing | occurrence_count | 1 | None |
| err-lock-missing | has_no_remedy_step | True | None |
| err-residual-energy | name | LOTO-RE - Residual stored ener | None |
| err-residual-energy | remedy_step_count | 1 | None |
| err-residual-energy | occurrence_count | 1 | None |
| err-sms-throttle | name | SMS-429 - SMS provider throttl | None |
| err-sms-throttle | occurrence_count | 1 | None |
| err-sms-throttle | has_no_remedy_step | True | None |

### issue_occurrences

- Fields: 20/40 (50.0%)
- Computed columns: name, is_unresolved, step_execution_when_unresolved, executed_step, failed_condition_count_on_run, coincided_with_failed_condition, has_no_recorded_solution, redesign_implemented_at, is_failure_without_landed_redesign, improvement_cycle_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| issue-0709-residual | name | err-residual-energy @ 2026-07- | None |
| issue-0709-residual | executed_step | loto-07 | None |
| issue-0709-residual | failed_condition_count_on_run | 1 | None |
| issue-0709-residual | coincided_with_failed_condition | True | None |
| issue-0709-residual | redesign_implemented_at | 2026-07-13T09:00:00-05:00 | None |
| issue-0709-residual | improvement_cycle_path | Observed at loto-07 by ken-wat | None |
| issue-0714-lockmissing | name | err-lock-missing @ 2026-07-14T | None |
| issue-0714-lockmissing | is_unresolved | True | None |
| issue-0714-lockmissing | step_execution_when_unresolved | se-loto0714-05 | None |
| issue-0714-lockmissing | executed_step | loto-05 | None |
| issue-0714-lockmissing | has_no_recorded_solution | True | None |
| issue-0714-lockmissing | is_failure_without_landed_redesign | True | None |
| issue-close-feed | name | err-feed-stale @ 2026-06-30T22 | None |
| issue-close-feed | executed_step | close-02 | None |
| issue-close-feed | is_failure_without_landed_redesign | True | None |
| issue-policy-sms | name | err-sms-throttle @ 2026-07-19T | None |
| issue-policy-sms | is_unresolved | True | None |
| issue-policy-sms | step_execution_when_unresolved | se-policy04 | None |
| issue-policy-sms | executed_step | policy-04 | None |
| issue-policy-sms | is_failure_without_landed_redesign | True | None |

### user_questions

- Fields: 3/8 (37.5%)
- Computed columns: name, is_unaddressed_question

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| q-close-fx | name | Why did the FX variance appear | None |
| q-policy-ai | name | Can the drafting AI add a new  | None |
| uq-loto-gauge | name | The gauge flickers just above  | None |
| uq-loto-press7 | name | Does the press-7 retrofit use  | None |
| uq-loto-press7 | is_unaddressed_question | True | None |

### user_feedback

- Fields: 15/24 (62.5%)
- Computed columns: name, is_unactioned_procedure_critique, collection_follow_up_count, is_tacit_signal_not_fed_into_collection

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| fb-close-retro | name | Accepted: The feed timestamp c | None |
| fb-loto-0708 | name | Accepted: Smooth run. | None |
| fb-loto-0709 | name | UnderReview: Press-7 map is mi | None |
| fb-loto-0709 | is_unactioned_procedure_critique | True | None |
| fb-policy-sms | name | UnderReview: Add a secondary p | None |
| uf13-deploy-cache-warm | name | Acknowledged: Before switching | None |
| uf13-deploy-cache-warm | is_tacit_signal_not_fed_into_collection | True | None |
| uf13-loto-hiss-cue | name | Accepted: On press-7 we wait f | None |
| uf13-loto-hiss-cue | collection_follow_up_count | 1 | None |

### stewardship_assignments

- Fields: 0/10 (0.0%)
- Computed columns: name, count_of_review_events, has_ever_been_reviewed, as_of_instant, is_current_assignment

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| stew-close | name | close-v1.1.0 / steward=process | None |
| stew-close | count_of_review_events | 2 | None |
| stew-close | has_ever_been_reviewed | True | None |
| stew-close | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| stew-close | is_current_assignment | True | None |
| stew-policy | name | policy-v1.0.0 / steward=proces | None |
| stew-policy | count_of_review_events | 1 | None |
| stew-policy | has_ever_been_reviewed | True | None |
| stew-policy | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| stew-policy | is_current_assignment | True | None |

### change_requests

- Fields: 76/132 (57.6%)
- Computed columns: name, is_open, open_change_version_key, is_decided, as_of_instant, days_pending, is_still_pending, is_stalled, authority_agent, requester_is_authority, awaits_authority_decision, authority_role_label, touches_live_version, is_live_decision_backlog, blocks_an_open_gap, backlog_version_key, is_my_pending_decision, is_my_blocking_backlog, is_my_overdue_backlog, is_implemented, is_my_decided_request, is_my_decided_but_unlanded, decision_latency_days, implementation_latency_days, delay_is_downstream_of_me, unlanded_version_key, is_approved_not_implemented, days_since_approval, is_stalled_implementation, stalled_implementation_version_key, approved_version_key, is_approved_decision, owner_organization

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cr-close-timestamp | name | Add source timestamp as blocki | None |
| cr-close-timestamp | is_decided | True | None |
| cr-close-timestamp | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| cr-close-timestamp | days_pending | 1 | None |
| cr-close-timestamp | authority_agent | priya-raman | None |
| cr-close-timestamp | authority_role_label | Procedural Knowledge Authority | None |
| cr-close-timestamp | touches_live_version | True | None |
| cr-close-timestamp | is_implemented | True | None |
| cr-close-timestamp | decision_latency_days | 1 | None |
| cr-close-timestamp | days_since_approval | 16 | None |
| cr-close-timestamp | approved_version_key | close-v1.1.0 | None |
| cr-close-timestamp | is_approved_decision | True | None |
| cr-close-timestamp | owner_organization | acme-finance | None |
| cr-enc-loto2-gauge-tap | name | Tap the gauge glass before a z | None |
| cr-enc-loto2-gauge-tap | is_open | True | None |
| cr-enc-loto2-gauge-tap | open_change_version_key | loto-v2.0.0 | None |
| cr-enc-loto2-gauge-tap | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| cr-enc-loto2-gauge-tap | days_pending | 3 | None |
| cr-enc-loto2-gauge-tap | is_still_pending | True | None |
| cr-enc-loto2-gauge-tap | authority_agent | lin-zhao | None |
| ... | ... | (36 more) | ... |

### review_events

- Fields: 13/36 (36.1%)
- Computed columns: name, as_of_instant, is_overdue, overdue_version_key, promised_cadence_days, days_since_reviewed, exceeds_promised_cadence, cadence_drift_days, promise_and_behavior_disagree, cadence_breach_version_key, version_modified_at, review_did_not_refresh_modified

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| review-close-q1 | name | close-v1.1.0 / QuarterlySemant | None |
| review-close-q1 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| review-close-q1 | is_overdue | True | None |
| review-close-q1 | overdue_version_key | close-v1.1.0 | None |
| review-close-q1 | promised_cadence_days | 90 | None |
| review-close-q1 | days_since_reviewed | 121 | None |
| review-close-q1 | exceeds_promised_cadence | True | None |
| review-close-q1 | cadence_drift_days | 31 | None |
| review-close-q1 | cadence_breach_version_key | close-v1.1.0 | None |
| review-close-q1 | version_modified_at | 2026-07-02T15:00:00-05:00 | None |
| review-close-q2 | name | close-v1.1.0 / QuarterlySemant | None |
| review-close-q2 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| review-close-q2 | promised_cadence_days | 90 | None |
| review-close-q2 | days_since_reviewed | 17 | None |
| review-close-q2 | cadence_drift_days | -73 | None |
| review-close-q2 | version_modified_at | 2026-07-02T15:00:00-05:00 | None |
| review-close-q2 | review_did_not_refresh_modified | True | None |
| review-policy-prelaunch | name | policy-v1.0.0 / PreLaunchRevie | None |
| review-policy-prelaunch | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| review-policy-prelaunch | promised_cadence_days | 60 | None |
| ... | ... | (3 more) | ... |

### learning_activities

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| learn-close-retro | name | Retrospective / 2026-07-02T20: | None |
| learn-policy-tabletop | name | TabletopExercise / 2026-07-15T | None |

### operational_bindings

- Fields: 21/55 (38.2%)
- Computed columns: name, as_of_instant, age_minutes, is_fresh, stale_binding_step_key, authoritative_stale_step_key, is_stale_and_authoritative, step_when_stale, resource_is_approved, is_usable_for_drafting, step_when_unusable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bind-close-erp | name | close-02 / GeneralLedger.Trial | None |
| bind-close-erp | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| bind-close-erp | age_minutes | 17 | None |
| bind-close-erp | stale_binding_step_key | close-02 | None |
| bind-close-erp | authoritative_stale_step_key | close-02 | None |
| bind-close-erp | is_stale_and_authoritative | True | None |
| bind-close-erp | step_when_stale | close-02 | None |
| bind-close-erp | step_when_unusable | close-02 | None |
| bind-policy-consent | name | policy-07 / HR.CommunicationCo | None |
| bind-policy-consent | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| bind-policy-consent | age_minutes | 75 | None |
| bind-policy-consent | stale_binding_step_key | policy-07 | None |
| bind-policy-consent | authoritative_stale_step_key | policy-07 | None |
| bind-policy-consent | is_stale_and_authoritative | True | None |
| bind-policy-consent | step_when_stale | policy-07 | None |
| bind-policy-consent | step_when_unusable | policy-07 | None |
| bind-policy-email | name | policy-07 / People.EmailTempla | None |
| bind-policy-email | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| bind-policy-email | age_minutes | 1230 | None |
| bind-policy-email | is_fresh | True | None |
| ... | ... | (14 more) | ... |

### communication_policies

- Fields: 3/8 (37.5%)
- Computed columns: name, consent_violation_count, quiet_hours_violation_count, is_active_policy

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| comm-email-policy | name | Email policy / policy-v1.0.0 | None |
| comm-email-policy | is_active_policy | True | None |
| comm-sms-policy | name | SMS policy / policy-v1.0.0 | None |
| comm-sms-policy | quiet_hours_violation_count | 1 | None |
| comm-sms-policy | is_active_policy | True | None |

### message_templates

- Fields: 9/32 (28.1%)
- Computed columns: name, policy_max_message_length, policy_max_segments, body_template_length, is_template_over_length, valid_approval_count, has_valid_approval, is_claiming_unbacked_approval, last_approved_body_hash, has_body_drifted, is_sendable_under_approval, drifted_send_count, unanswered_delivery_count, transmitted_delivery_count, template_draws_no_response, last_approval_at

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tmpl-policy-email | name | comm-email-policy / en-US | None |
| tmpl-policy-email | policy_max_message_length | 100000 | None |
| tmpl-policy-email | policy_max_segments | 1 | None |
| tmpl-policy-email | body_template_length | 164 | None |
| tmpl-policy-email | valid_approval_count | 1 | None |
| tmpl-policy-email | has_valid_approval | True | None |
| tmpl-policy-email | last_approved_body_hash | h-email-v1 | None |
| tmpl-policy-email | is_sendable_under_approval | True | None |
| tmpl-policy-email | unanswered_delivery_count | 1 | None |
| tmpl-policy-email | transmitted_delivery_count | 2 | None |
| tmpl-policy-email | last_approval_at | 2026-07-17T11:00:00-05:00 | None |
| tmpl-policy-sms | name | comm-sms-policy / en-US | None |
| tmpl-policy-sms | policy_max_message_length | 160 | None |
| tmpl-policy-sms | policy_max_segments | 3 | None |
| tmpl-policy-sms | body_template_length | 123 | None |
| tmpl-policy-sms | valid_approval_count | 1 | None |
| tmpl-policy-sms | has_valid_approval | True | None |
| tmpl-policy-sms | last_approved_body_hash | h-sms-v1 | None |
| tmpl-policy-sms | is_sendable_under_approval | True | None |
| tmpl-policy-sms | unanswered_delivery_count | 2 | None |
| ... | ... | (3 more) | ... |

### semantic_mappings

- Fields: 712/1428 (49.9%)
- Computed columns: name, profile_namespace_dereferences, reinvents_standard_term, is_non_resolvable_term_iri

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| map-AbundantKnowledgeGaps | name | AbundantKnowledgeGaps -> https | None |
| map-AbundantKnowledgeGaps | is_non_resolvable_term_iri | True | None |
| map-AccessDenialTests | name | AccessDenialTests -> https://e | None |
| map-AccessDenialTests | is_non_resolvable_term_iri | True | None |
| map-AccessPolicies | name | AccessPolicies -> https://effo | None |
| map-AccessPolicies | is_non_resolvable_term_iri | True | None |
| map-AccessPrincipals | name | AccessPrincipals -> https://ef | None |
| map-AccessPrincipals | is_non_resolvable_term_iri | True | None |
| map-ActivityRelations | name | ActivityRelations -> https://e | None |
| map-ActivityRelations | is_non_resolvable_term_iri | True | None |
| map-AgentDecisionRecords | name | AgentDecisionRecords -> https: | None |
| map-AgentDecisionRecords | is_non_resolvable_term_iri | True | None |
| map-AgentIntegrations | name | AgentIntegrations -> https://e | None |
| map-AgentIntegrations | is_non_resolvable_term_iri | True | None |
| map-AgentUpgradeAssessments | name | AgentUpgradeAssessments -> htt | None |
| map-AgentUpgradeAssessments | is_non_resolvable_term_iri | True | None |
| map-Agents-Organization | name | Agents -> http://www.w3.org/ns | None |
| map-Agents-Organization | is_non_resolvable_term_iri | True | None |
| map-Agents-SoftwareAgent | name | Agents -> http://www.w3.org/ns | None |
| map-Agents-SoftwareAgent | is_non_resolvable_term_iri | True | None |
| ... | ... | (696 more) | ... |

### witness_loops

- Fields: 14/48 (29.2%)
- Computed columns: name, question_count, is_complete

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| loop-01 | name | Loop 1: The founding questions | None |
| loop-01 | question_count | 60 | None |
| loop-01 | is_complete | True | None |
| loop-02 | name | Loop 2: Questions only loop 1  | None |
| loop-02 | question_count | 45 | None |
| loop-02 | is_complete | True | None |
| loop-03 | name | Loop 3: Loop 3: the three gaps | None |
| loop-03 | question_count | 3 | None |
| loop-04 | name | Loop 4: Structure the executio | None |
| loop-04 | question_count | 2.0 | None |
| loop-06 | name | Loop 6: Loop 6: the full PKO c | None |
| loop-06 | question_count | 49.0 | None |
| loop-07 | name | Loop 7: Loop 7: elicitation me | None |
| loop-07 | question_count | 27.0 | None |
| loop-08 | name | Loop 8: Loop 8: organizing col | None |
| loop-08 | question_count | 36.0 | None |
| loop-09 | name | Loop 9: Loop 9: encoded knowle | None |
| loop-09 | question_count | 45.0 | None |
| loop-10 | name | Loop 10: Loop 10: the model go | None |
| loop-10 | question_count | 63.0 | None |
| ... | ... | (14 more) | ... |

### role_questions

- Fields: 0/1326 (0.0%)
- Computed columns: name, predicate_count, is_answered

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| aq-ont4-q01 | name | change-risk-classifier: Which  | None |
| aq-ont4-q01 | predicate_count | 6.0 | None |
| aq-ont4-q01 | is_answered | True | None |
| aq-ont4-q02 | name | release-manager: Which role, a | None |
| aq-ont4-q02 | predicate_count | 4.0 | None |
| aq-ont4-q02 | is_answered | True | None |
| aq-ont4-q03 | name | release-manager: Which AI agen | None |
| aq-ont4-q03 | predicate_count | 12.0 | None |
| aq-ont4-q03 | is_answered | True | None |
| aq-ont4-q04 | name | vp-engineering: If this agent  | None |
| aq-ont4-q04 | predicate_count | 13.0 | None |
| aq-ont4-q04 | is_answered | True | None |
| aq-ont4-q05 | name | process-steward: Which workflo | None |
| aq-ont4-q05 | predicate_count | 1.0 | None |
| aq-ont4-q05 | is_answered | True | None |
| aq-ont4-q06 | name | release-manager: To whom does  | None |
| aq-ont4-q06 | predicate_count | 3.0 | None |
| aq-ont4-q06 | is_answered | True | None |
| aq-ont4-q07 | name | release-manager: Which agent i | None |
| aq-ont4-q07 | predicate_count | 2.0 | None |
| ... | ... | (1306 more) | ... |

### rulebook_fields

- Fields: 15587/36295 (42.9%)
- Computed columns: name, is_derived, is_witness, disagreeing_substrate_count, is_substrate_contested, has_measured_data, is_discriminating

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| AbundantKnowledgeGaps.AbundantKnowledgeGapId | name | AbundantKnowledgeGaps.Abundant | None |
| AbundantKnowledgeGaps.AbundantKnowledgeGapId | has_measured_data | True | None |
| AbundantKnowledgeGaps.AbundantKnowledgeGapId | is_discriminating | True | None |
| AbundantKnowledgeGaps.Description | name | AbundantKnowledgeGaps.Descript | None |
| AbundantKnowledgeGaps.Description | is_witness | True | None |
| AbundantKnowledgeGaps.Description | has_measured_data | True | None |
| AbundantKnowledgeGaps.Description | is_discriminating | True | None |
| AbundantKnowledgeGaps.GapKind | name | AbundantKnowledgeGaps.GapKind | None |
| AbundantKnowledgeGaps.GapKind | is_witness | True | None |
| AbundantKnowledgeGaps.GapKind | has_measured_data | True | None |
| AbundantKnowledgeGaps.GapKind | is_discriminating | True | None |
| AbundantKnowledgeGaps.Label | name | AbundantKnowledgeGaps.Label | None |
| AbundantKnowledgeGaps.Label | is_witness | True | None |
| AbundantKnowledgeGaps.Label | has_measured_data | True | None |
| AbundantKnowledgeGaps.Label | is_discriminating | True | None |
| AbundantKnowledgeGaps.Name | name | AbundantKnowledgeGaps.Name | None |
| AbundantKnowledgeGaps.Name | is_derived | True | None |
| AbundantKnowledgeGaps.Name | has_measured_data | True | None |
| AbundantKnowledgeGaps.Name | is_discriminating | True | None |
| AbundantKnowledgeGaps.RepresentedByTable | name | AbundantKnowledgeGaps.Represen | None |
| ... | ... | (20688 more) | ... |

### test_suites

- Fields: 6/30 (20.0%)
- Computed columns: name, test_count, pass_count, blocking_fail_count, is_green

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| suite-integrity | name | Referential integrity | None |
| suite-integrity | test_count | 160 | None |
| suite-integrity | pass_count | 160 | None |
| suite-integrity | is_green | True | None |
| suite-invariant | name | Domain invariants | None |
| suite-invariant | test_count | 7 | None |
| suite-invariant | pass_count | 7 | None |
| suite-invariant | is_green | True | None |
| suite-provenance | name | Question provenance | None |
| suite-provenance | test_count | 107 | None |
| suite-provenance | pass_count | 107 | None |
| suite-provenance | is_green | True | None |
| suite-structure | name | Model structure | None |
| suite-structure | test_count | 69 | None |
| suite-structure | pass_count | 69 | None |
| suite-structure | is_green | True | None |
| suite-substrate | name | Substrate translation | None |
| suite-substrate | test_count | 993 | None |
| suite-substrate | pass_count | 991 | None |
| suite-substrate | is_green | True | None |
| ... | ... | (4 more) | ... |

### test_cases

- Fields: 6310/12558 (50.2%)
- Computed columns: name, is_blocking, is_passing, is_failing, needs_attention, passing_suite_key, needs_attention_suite_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tc-answered-q-analyst-blocking-never-evaluated | name | question-answered: q-analyst-b | None |
| tc-answered-q-analyst-blocking-never-evaluated | is_blocking | True | None |
| tc-answered-q-analyst-blocking-never-evaluated | is_passing | True | None |
| tc-answered-q-analyst-blocking-never-evaluated | passing_suite_key | suite-provenance | None |
| tc-answered-q-analyst-blocking-unmet-on-my-step | name | question-answered: q-analyst-b | None |
| tc-answered-q-analyst-blocking-unmet-on-my-step | is_blocking | True | None |
| tc-answered-q-analyst-blocking-unmet-on-my-step | is_passing | True | None |
| tc-answered-q-analyst-blocking-unmet-on-my-step | passing_suite_key | suite-provenance | None |
| tc-answered-q-analyst-evidence-binding-stale | name | question-answered: q-analyst-e | None |
| tc-answered-q-analyst-evidence-binding-stale | is_blocking | True | None |
| tc-answered-q-analyst-evidence-binding-stale | is_passing | True | None |
| tc-answered-q-analyst-evidence-binding-stale | passing_suite_key | suite-provenance | None |
| tc-answered-q-analyst-my-late-steps-unexplained | name | question-answered: q-analyst-m | None |
| tc-answered-q-analyst-my-late-steps-unexplained | is_blocking | True | None |
| tc-answered-q-analyst-my-late-steps-unexplained | is_passing | True | None |
| tc-answered-q-analyst-my-late-steps-unexplained | passing_suite_key | suite-provenance | None |
| tc-answered-q-analyst-verification-declared-never-performed | name | question-answered: q-analyst-v | None |
| tc-answered-q-analyst-verification-declared-never-performed | is_blocking | True | None |
| tc-answered-q-analyst-verification-declared-never-performed | is_passing | True | None |
| tc-answered-q-analyst-verification-declared-never-performed | passing_suite_key | suite-provenance | None |
| ... | ... | (6228 more) | ... |

### exception_invocations

- Fields: 4/13 (30.8%)
- Computed columns: name, expected_handling, required_approval_role, required_approval_role_holder, approval_role_matches, is_approved, is_improperly_approved, invoker_agent_kind, invoker_also_prepared_key, parent_procedure_execution, approver_prepared_count, delegated_to_preparer, is_ungoverned_invocation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exc-inv-close02 | name | se-close02 / exc-ledger-outage | None |
| exc-inv-close02 | expected_handling | Use a digitally signed, timest | None |
| exc-inv-close02 | required_approval_role | controller | None |
| exc-inv-close02 | required_approval_role_holder | devon-okafor | None |
| exc-inv-close02 | approval_role_matches | True | None |
| exc-inv-close02 | is_approved | True | None |
| exc-inv-close02 | invoker_agent_kind | AutomatedPipeline | None |
| exc-inv-close02 | invoker_also_prepared_key | exec-close-2026-q2|devon-okafo | None |
| exc-inv-close02 | parent_procedure_execution | exec-close-2026-q2 | None |

### verification_outcomes

- Fields: 32/72 (44.4%)
- Computed columns: name, expected_signal_value, signal_identifier, signal_matches_expected, has_evidence, is_unbacked_observation, is_self_witnessed, step_executor_agent, self_witnessed_step_key, unbacked_step_key, is_self_witnessed_and_unbacked, is_uncorroborated_pass, uncorroborated_pass_step_key, observer_is_non_human, observer_is_independent_of_executor, is_independent_human_observation, independent_observation_execution_key, parent_procedure_execution_of_outcome

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| vo-close01 | name | se-close01 / verify-close-cuto | None |
| vo-close01 | expected_signal_value | LOCKED | None |
| vo-close01 | signal_identifier | ledger-lock-state | None |
| vo-close01 | signal_matches_expected | True | None |
| vo-close01 | has_evidence | True | None |
| vo-close01 | step_executor_agent | close-pipeline | None |
| vo-close01 | observer_is_independent_of_executor | True | None |
| vo-close01 | is_independent_human_observation | True | None |
| vo-close01 | independent_observation_execution_key | exec-close-2026-q2 | None |
| vo-close01 | parent_procedure_execution_of_outcome | exec-close-2026-q2 | None |
| vo-close02 | name | se-close02 / verify-feed-time | None |
| vo-close02 | expected_signal_value | 15 | None |
| vo-close02 | signal_identifier | max-feed-age-minutes | None |
| vo-close02 | has_evidence | True | None |
| vo-close02 | is_self_witnessed | True | None |
| vo-close02 | step_executor_agent | close-pipeline | None |
| vo-close02 | self_witnessed_step_key | se-close02 | None |
| vo-close02 | observer_is_non_human | True | None |
| vo-close02 | parent_procedure_execution_of_outcome | exec-close-2026-q2 | None |
| vo-close03 | name | se-close03 / verify-reconcilia | None |
| ... | ... | (20 more) | ... |

### observed_transitions

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| obs-close-01-02-q2 | name | close-01-to-close-02 @ 2026-06 | None |
| obs-close-02-03-q2 | name | close-02-to-close-03 @ 2026-06 | None |
| obs-close-03-04-q2 | name | close-03-to-close-04 @ 2026-07 | None |
| obs-close-04-05-q2 | name | close-04-to-close-05 @ 2026-07 | None |
| obs-close-05-06-q2 | name | close-05-to-close-06 @ 2026-07 | None |
| obs-close-06-07-q2 | name | close-06-to-close-07 @ 2026-07 | None |
| obs-close-07-08-q2 | name | close-07-to-close-08 @ 2026-07 | None |
| obs-policy-01-02-hr4821 | name | policy-01-to-policy-02 @ 2026- | None |
| obs-policy-02-03-hr4821 | name | policy-02-to-policy-03 @ 2026- | None |
| obs-policy-03-04-hr4821 | name | policy-03-to-policy-04 @ 2026- | None |

### recipients

- Fields: 13/30 (43.3%)
- Computed columns: name, has_sms_consent, is_email_reachable, is_sms_reachable, is_unreachable, is_communicationally_stranded

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rec-001 | name | Jordan Ellis | None |
| rec-001 | has_sms_consent | True | None |
| rec-001 | is_email_reachable | True | None |
| rec-001 | is_sms_reachable | True | None |
| rec-002 | name | Sam Rivera | None |
| rec-002 | has_sms_consent | True | None |
| rec-002 | is_email_reachable | True | None |
| rec-002 | is_sms_reachable | True | None |
| rec-003 | name | Alex Kim | None |
| rec-003 | has_sms_consent | True | None |
| rec-003 | is_email_reachable | True | None |
| rec-003 | is_sms_reachable | True | None |
| rec-004 | name | Riley Chen | None |
| rec-004 | is_email_reachable | True | None |
| rec-004 | is_sms_reachable | True | None |
| rec-005 | name | Morgan Diaz | None |
| rec-005 | is_email_reachable | True | None |

### message_deliveries

- Fields: 243/444 (54.7%)
- Computed columns: name, policy_channel, channel_name, policy_requires_consent, recipient_has_sms_consent, was_actually_transmitted, is_consent_violation, consent_violation_policy_key, policy_quiet_hours_start_hour, policy_quiet_hours_end_hour, policy_has_quiet_hours, quiet_window_wraps_midnight, is_inside_quiet_window, is_quiet_hours_violation, quiet_hours_violation_policy_key, recipient_is_unreachable, is_acknowledged, invoked_exception_condition, has_unreachable_exception_invoked, is_fabricated_acknowledgement, is_unhandled_unreachable, unreachable_failure_key, policy_retention_days, as_of_instant, age_days, is_within_retention_window, has_rendered_body, is_evidence_required, is_retention_breach, retention_breach_execution_key, sending_step_execution_step, execution_has_cleared_legal_review, is_unreviewed_send, rendered_body_length, policy_max_message_length_at_send, segment_count, policy_max_segments_at_send, is_over_segment_limit, template_has_valid_approval, is_unapproved_send, policy_required_opt_out_phrase, policy_requires_opt_out, opt_out_phrase_position, has_opt_out_phrase, is_opt_out_in_first_segment, is_missing_required_opt_out, is_opt_out_at_risk_of_truncation, is_failed_delivery, is_suppressed, is_triaged, is_abandoned_failure, abandoned_failure_execution_key, reached_execution_key, template_was_sendable, is_drifted_send, drifted_send_template_key, was_sent_outside_business_hours, was_delivered_and_unanswered, is_poorly_timed_unanswered, is_well_timed_unanswered, unanswered_template_key, transmitted_template_key, approval_preceded_send, has_frozen_approval_evidence, provenance_is_live_derived, current_last_approval_at, template_reapproved_since_send, is_unprovable_approval_claim, has_sent_reminder, acknowledgement_is_outstanding, outstanding_age_days, is_unchased_acknowledgement, is_exhausted_follow_up, needs_human_escalation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| md-001 | name | rec-001 / tmpl-policy-email /  | None |
| md-001 | policy_channel | comm-email-policy | None |
| md-001 | channel_name | Email | None |
| md-001 | recipient_has_sms_consent | True | None |
| md-001 | was_actually_transmitted | True | None |
| md-001 | is_acknowledged | True | None |
| md-001 | policy_retention_days | 2555 | None |
| md-001 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| md-001 | age_days | 1 | None |
| md-001 | is_within_retention_window | True | None |
| md-001 | has_rendered_body | True | None |
| md-001 | is_evidence_required | True | None |
| md-001 | sending_step_execution_step | policy-04 | None |
| md-001 | execution_has_cleared_legal_review | True | None |
| md-001 | rendered_body_length | 107 | None |
| md-001 | policy_max_message_length_at_send | 100000 | None |
| md-001 | segment_count | 1 | None |
| md-001 | policy_max_segments_at_send | 1 | None |
| md-001 | template_has_valid_approval | True | None |
| md-001 | reached_execution_key | exec-policy-hr4821 | None |
| ... | ... | (181 more) | ... |

### template_approvals

- Fields: 0/12 (0.0%)
- Computed columns: name, is_approval_decision, template_policy, required_approval_role, is_decided_by_required_role, valid_approval_template_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tap-email-001 | name | tmpl-policy-email / Approved / | None |
| tap-email-001 | is_approval_decision | True | None |
| tap-email-001 | template_policy | comm-email-policy | None |
| tap-email-001 | required_approval_role | communications-manager | None |
| tap-email-001 | is_decided_by_required_role | True | None |
| tap-email-001 | valid_approval_template_key | tmpl-policy-email | None |
| tap-sms-001 | name | tmpl-policy-sms / Approved / 2 | None |
| tap-sms-001 | is_approval_decision | True | None |
| tap-sms-001 | template_policy | comm-sms-policy | None |
| tap-sms-001 | required_approval_role | communications-manager | None |
| tap-sms-001 | is_decided_by_required_role | True | None |
| tap-sms-001 | valid_approval_template_key | tmpl-policy-sms | None |

### send_intents

- Fields: 247/553 (44.7%)
- Computed columns: name, intent_policy, intent_channel, policy_is_active, intent_requires_consent, recipient_has_channel_consent, consent_gate_passed, recipient_is_sms_reachable, recipient_is_email_reachable, reachability_gate_passed, permission_gate_passed, intent_quiet_start_hour, intent_quiet_end_hour, intent_policy_has_quiet_hours, intent_quiet_window_wraps, intent_is_inside_quiet_window, timing_gate_passed, hours_until_window_opens, intent_max_message_length, intent_max_segments, length_gate_passed, intent_required_opt_out_phrase, opt_out_gate_passed, content_gate_passed, template_is_sendable, execution_has_legal_clearance, intent_approval_role, approval_role_agent_kind, approval_is_human, authorization_gate_passed, is_cleared_to_send, blocking_gate_name, has_resulting_delivery, resulting_delivery_was_transmitted, is_overridden_refusal, is_silently_dropped, resulting_delivery_exception, refusal_cited_an_exception, is_properly_handled_refusal, refusal_failure_execution_key, intent_execution_key, delivered_intent_execution_key, dropped_intent_execution_key, my_approval_was_in_force, refused_on_approved_content, refused_on_opt_out_only, refusal_was_on_my_rules, refusal_was_outside_my_control, is_unreported_refusal_on_my_rules, is_approval_overridden_silently, has_alternate_channel_attempt, alternate_attempt_was_cleared, is_refused_with_no_alternative, exception_prescribed_an_alternative, prescribed_handling_was_performed, is_suppression_without_remedy, has_durable_refusal_record, refusal_was_escalated, is_unrecorded_refusal, is_unescalated_refusal, unescalated_refusal_role_key, unrecorded_refusal_execution_key, was_deferred_on_timing, as_of_instant, window_has_since_reopened, has_retry_attempt, retry_was_cleared, is_abandoned_deferral, deferral_age_hours, is_stale_deferral, enforced_by_unauthorized_agent, consent_input_was_resolvable, recipient_consent_status_raw, policy_input_was_resolvable, all_gate_inputs_resolved, is_unevaluable_refusal, is_self_witnessed_decision, is_independently_confirmed, independently_confirmed_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| si-001 | name | rec-001 / tmpl-policy-sms / in | None |
| si-001 | intent_policy | comm-sms-policy | None |
| si-001 | intent_channel | SMS | None |
| si-001 | policy_is_active | True | None |
| si-001 | intent_requires_consent | True | None |
| si-001 | recipient_has_channel_consent | True | None |
| si-001 | consent_gate_passed | True | None |
| si-001 | recipient_is_sms_reachable | True | None |
| si-001 | recipient_is_email_reachable | True | None |
| si-001 | reachability_gate_passed | True | None |
| si-001 | permission_gate_passed | True | None |
| si-001 | intent_quiet_start_hour | 20 | None |
| si-001 | intent_quiet_end_hour | 8 | None |
| si-001 | intent_policy_has_quiet_hours | True | None |
| si-001 | intent_quiet_window_wraps | True | None |
| si-001 | timing_gate_passed | True | None |
| si-001 | intent_max_message_length | 160 | None |
| si-001 | intent_max_segments | 3 | None |
| si-001 | length_gate_passed | True | None |
| si-001 | intent_required_opt_out_phrase | Reply STOP to opt out | None |
| ... | ... | (286 more) | ... |

### agent_decision_records

- Fields: 51/81 (63.0%)
- Computed columns: name, was_overridden, was_reviewed, deciding_agent_kind, deciding_agent_when_overridden, role_assignment_when_scored, role_assignment_when_overridden, step_of_decision, boundary_match_key, matching_boundary_count, violated_authority_boundary, reviewer_agent_kind, has_human_confirmation, needs_human_confirmation, is_unconfirmed_non_human_decision, step_execution_when_unconfirmed, agent_when_boundary_violated, review_latency_minutes, is_draft_kind, agent_when_draft_overridden, agent_when_draft, is_error_correction, is_reserved_judgment_override, override_reason_is_recorded, is_unexplained_override, error_correction_role_assignment_key, boundary_violation_role_assignment_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| adr-close-extract-q2 | name | close-pipeline: Accepted a tri | None |
| adr-close-extract-q2 | was_overridden | True | None |
| adr-close-extract-q2 | was_reviewed | True | None |
| adr-close-extract-q2 | deciding_agent_kind | AutomatedPipeline | None |
| adr-close-extract-q2 | deciding_agent_when_overridden | close-pipeline | None |
| adr-close-extract-q2 | step_of_decision | close-02 | None |
| adr-close-extract-q2 | boundary_match_key | close-02|AutomatedPipeline|Pos | None |
| adr-close-extract-q2 | reviewer_agent_kind | Human | None |
| adr-close-extract-q2 | has_human_confirmation | True | None |
| adr-close-extract-q2 | needs_human_confirmation | True | None |
| adr-close-extract-q2 | review_latency_minutes | 4.0 | None |
| adr-close-extract-q2 | is_unexplained_override | True | None |
| adr-close-freeze-q2 | name | close-pipeline: Locked subledg | None |
| adr-close-freeze-q2 | was_reviewed | True | None |
| adr-close-freeze-q2 | deciding_agent_kind | AutomatedPipeline | None |
| adr-close-freeze-q2 | step_of_decision | close-01 | None |
| adr-close-freeze-q2 | boundary_match_key | close-01|AutomatedPipeline|Pos | None |
| adr-close-freeze-q2 | reviewer_agent_kind | Human | None |
| adr-close-freeze-q2 | has_human_confirmation | True | None |
| adr-close-freeze-q2 | needs_human_confirmation | True | None |
| ... | ... | (10 more) | ... |

### delivered_communications

- Fields: 3/6 (50.0%)
- Computed columns: name, has_authorization, content_matches_approval, authorized_at, was_approved_before_sending, is_defensible

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dc-hr4821-email-001 | name | Email -> emp-anon-0001 @  | None |
| dc-hr4821-email-001 | content_matches_approval | True | None |
| dc-hr4821-email-001 | was_approved_before_sending | True | None |

### authority_boundaries

- Fields: 28/63 (44.4%)
- Computed columns: name, as_of_instant, is_currently_binding, ratifying_fragment_is_valid, step_when_binding, boundary_match_key, violation_count, is_untested, has_ratifying_fragment, is_unwarranted, ratifying_fragment_is_overdue, ratifying_fragment_is_single_witness, warrant_is_thin, is_unwarranted_and_untested, unwarranted_boundary_step_key, ratifying_fragment_key, ratifying_fragment_status, ratification_lapsed, binds_despite_lapsed_ratification, is_ungrounded_and_untested, constrained_role_assignment_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bnd-close-04-no-ai-escalation-waiver | name | AIAgent may not Suppression | None |
| bnd-close-04-no-ai-escalation-waiver | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| bnd-close-04-no-ai-escalation-waiver | is_currently_binding | True | None |
| bnd-close-04-no-ai-escalation-waiver | ratifying_fragment_is_valid | True | None |
| bnd-close-04-no-ai-escalation-waiver | step_when_binding | close-04 | None |
| bnd-close-04-no-ai-escalation-waiver | boundary_match_key | close-04|AIAgent|Suppression | None |
| bnd-close-04-no-ai-escalation-waiver | is_untested | True | None |
| bnd-close-04-no-ai-escalation-waiver | has_ratifying_fragment | True | None |
| bnd-close-04-no-ai-escalation-waiver | ratifying_fragment_is_overdue | True | None |
| bnd-close-04-no-ai-escalation-waiver | ratifying_fragment_is_single_witness | True | None |
| bnd-close-04-no-ai-escalation-waiver | warrant_is_thin | True | None |
| bnd-close-04-no-ai-escalation-waiver | ratifying_fragment_key | kf-close-judgment | None |
| bnd-close-04-no-ai-escalation-waiver | ratifying_fragment_status | Approved | None |
| bnd-close-06-no-nonhuman-approval | name | AIAgent may not Approval | None |
| bnd-close-06-no-nonhuman-approval | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| bnd-close-06-no-nonhuman-approval | is_currently_binding | True | None |
| bnd-close-06-no-nonhuman-approval | step_when_binding | close-06 | None |
| bnd-close-06-no-nonhuman-approval | boundary_match_key | close-06|AIAgent|Approval | None |
| bnd-close-06-no-nonhuman-approval | is_untested | True | None |
| bnd-close-06-no-nonhuman-approval | is_unwarranted | True | None |
| ... | ... | (15 more) | ... |

### app_role_profiles

- Fields: 8/40 (20.0%)
- Computed columns: name, route_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| profile-cfo | name | Chief Financial Officer (human | None |
| profile-cfo | route_count | 12 | None |
| profile-close-automation | name | Close Automation Operator (sof | None |
| profile-close-automation | route_count | 6 | None |
| profile-communications-manager | name | Employee Communications Manage | None |
| profile-communications-manager | route_count | 11 | None |
| profile-controller | name | Corporate Controller (human) | None |
| profile-controller | route_count | 10 | None |
| profile-employment-counsel | name | Employment Counsel (human) | None |
| profile-employment-counsel | route_count | 6 | None |
| profile-finance-analyst | name | Finance Analyst (human) | None |
| profile-finance-analyst | route_count | 11 | None |
| profile-hr-policy-owner | name | People Policy Owner (human) | None |
| profile-hr-policy-owner | route_count | 10 | None |
| profile-knowledge-authority | name | Procedural Knowledge Authority | None |
| profile-knowledge-authority | route_count | 11 | None |
| profile-knowledge-engineer | name | Knowledge Engineer (human) | None |
| profile-maintenance-technician | name | Maintenance Technician (human) | None |
| profile-notification-publisher | name | Notification Publisher (softwa | None |
| profile-notification-publisher | route_count | 11 | None |
| ... | ... | (12 more) | ... |

### app_nav_groups

- Fields: 0/46 (0.0%)
- Computed columns: name, route_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| assurance | name | Assurance | None |
| assurance | route_count | 5 | None |
| attest | name | Attestation | None |
| attest | route_count | 4 | None |
| audience | name | Audience | None |
| audience | route_count | 2 | None |
| authority | name | Authority | None |
| authority | route_count | 4 | None |
| backlog | name | My Backlog | None |
| backlog | route_count | 3 | None |
| cadence | name | Cadence | None |
| cadence | route_count | 3 | None |
| compliance | name | Compliance | None |
| compliance | route_count | 8 | None |
| conformance | name | Conformance | None |
| conformance | route_count | 2 | None |
| controls | name | Controls | None |
| controls | route_count | 4 | None |
| delivery | name | Delivery | None |
| delivery | route_count | 5 | None |
| ... | ... | (26 more) | ... |

### app_routes

- Fields: 469/1043 (45.0%)
- Computed columns: name, is_in_nav, is_shared, is_maintainer, question_count, reference_count, answers_no_question

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| admin-board | name | Test Board — /admin/conformanc | None |
| admin-board | is_in_nav | True | None |
| admin-board | is_maintainer | True | None |
| admin-board | reference_count | 2 | None |
| admin-catalog-drift | name | Catalog Drift — /admin/model/c | None |
| admin-catalog-drift | is_in_nav | True | None |
| admin-catalog-drift | is_maintainer | True | None |
| admin-catalog-drift | reference_count | 2 | None |
| admin-checks | name | Checks — /admin/conformance/ch | None |
| admin-checks | is_in_nav | True | None |
| admin-checks | is_maintainer | True | None |
| admin-checks | reference_count | 2 | None |
| admin-loops | name | Loops & Questions — /admin/loo | None |
| admin-loops | is_in_nav | True | None |
| admin-loops | is_maintainer | True | None |
| admin-loops | reference_count | 2 | None |
| admin-provenance | name | Provenance Tracer — /admin/pro | None |
| admin-provenance | is_in_nav | True | None |
| admin-provenance | is_maintainer | True | None |
| admin-provenance | reference_count | 2 | None |
| ... | ... | (554 more) | ... |

### app_route_questions

- Fields: 0/151 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rq-analyst-blocking-unmet--q-analyst-blocking-unmet-on-my-step | name | analyst-blocking-unmet answers | None |
| rq-analyst-clean-but-uncovered--q2-fa-clean-but-uncovered | name | analyst-clean-but-uncovered an | None |
| rq-analyst-evidence-quality--q2-fa-my-own-evidence-quality | name | analyst-evidence-quality answe | None |
| rq-analyst-home--q-analyst-blocking-unmet-on-my-step | name | analyst-home answers q-analyst | None |
| rq-analyst-home--q2-fa-clean-but-uncovered | name | analyst-home answers q2-fa-cle | None |
| rq-analyst-late-steps--q-analyst-my-late-steps-unexplained | name | analyst-late-steps answers q-a | None |
| rq-analyst-mute-clearance--q2-fa-cleared-by-mute-control | name | analyst-mute-clearance answers | None |
| rq-analyst-never-evaluated--q-analyst-blocking-never-evaluated | name | analyst-never-evaluated answer | None |
| rq-analyst-source-freshness--q-analyst-evidence-binding-stale | name | analyst-source-freshness answe | None |
| rq-analyst-stale-then-vs-now--q2-fa-stale-then-vs-stale-now | name | analyst-stale-then-vs-now answ | None |
| rq-analyst-verification-performed--q-analyst-verification-declared-never-performed | name | analyst-verification-performed | None |
| rq-analyst-witness-coverage--q2-fa-witness-coverage-of-my-steps | name | analyst-witness-coverage answe | None |
| rq-authority-elicitation-monoculture--q2-authority-elicitation-method-monoculture | name | authority-elicitation-monocult | None |
| rq-authority-expired-ratifier--q2-authority-boundary-rests-on-expired-claim | name | authority-expired-ratifier ans | None |
| rq-authority-fragment-decay--q2-authority-compound-fragility | name | authority-fragment-decay answe | None |
| rq-authority-home--q-authority-non-approved-dependency | name | authority-home answers q-autho | None |
| rq-authority-home--q-authority-stale-elicitation | name | authority-home answers q-autho | None |
| rq-authority-known-unknowns--q-authority-known-unknowns | name | authority-known-unknowns answe | None |
| rq-authority-machine-consumers--q2-authority-unapproved-knowledge-reaching-machines | name | authority-machine-consumers an | None |
| rq-authority-orphaned-provenance--q-authority-orphaned-provenance | name | authority-orphaned-provenance  | None |
| ... | ... | (131 more) | ... |

### app_route_references

- Fields: 0/315 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rr-admin-board--admin-checks | name | admin-board -> admin-checks | None |
| rr-admin-board--admin-witnesses | name | admin-board -> admin-witnesses | None |
| rr-admin-catalog-drift--admin-board | name | admin-catalog-drift -> admin-b | None |
| rr-admin-catalog-drift--explorer-inferences | name | admin-catalog-drift -> explore | None |
| rr-admin-checks--admin-board | name | admin-checks -> admin-board | None |
| rr-admin-checks--admin-question-defence | name | admin-checks -> admin-question | None |
| rr-admin-loops--admin-question-defence | name | admin-loops -> admin-question- | None |
| rr-admin-loops--shared-field-provenance | name | admin-loops -> shared-field-pr | None |
| rr-admin-provenance--admin-loops | name | admin-provenance -> admin-loop | None |
| rr-admin-provenance--shared-field-provenance | name | admin-provenance -> shared-fie | None |
| rr-admin-question-defence--admin-checks | name | admin-question-defence -> admi | None |
| rr-admin-question-defence--admin-loops | name | admin-question-defence -> admi | None |
| rr-admin-question-defence--admin-route-map | name | admin-question-defence -> admi | None |
| rr-admin-route-map--admin-question-defence | name | admin-route-map -> admin-quest | None |
| rr-admin-route-map--explorer-tables | name | admin-route-map -> explorer-ta | None |
| rr-admin-vacuous--admin-loops | name | admin-vacuous -> admin-loops | None |
| rr-admin-vacuous--admin-witnesses | name | admin-vacuous -> admin-witness | None |
| rr-admin-witnesses--admin-vacuous | name | admin-witnesses -> admin-vacuo | None |
| rr-admin-witnesses--shared-field-provenance | name | admin-witnesses -> shared-fiel | None |
| rr-analyst-blocking-unmet--analyst-never-evaluated | name | analyst-blocking-unmet -> anal | None |
| ... | ... | (295 more) | ... |

### rulebook_tables

- Fields: 2705/4752 (56.9%)
- Computed columns: name, field_count, policy_count, is_unsecured, disagreeing_substrate_count, has_measured_rows, semantic_mapping_count, meaning_is_only_tabular, exact_mapping_count, aligned_mapping_count, is_unaligned_to_standard, semantic_type_iri_field_count, lacks_semantic_type_convention, is_unsecured_governance_record, unrestricted_non_admin_policy_count, restricted_non_admin_policy_count, is_readable_in_full_by_non_admin, is_controlled_for_every_non_admin

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| AbundantKnowledgeGaps | name | AbundantKnowledgeGaps | None |
| AbundantKnowledgeGaps | field_count | 9.0 | None |
| AbundantKnowledgeGaps | policy_count | 2.0 | None |
| AbundantKnowledgeGaps | has_measured_rows | True | None |
| AbundantKnowledgeGaps | semantic_mapping_count | 1 | None |
| AbundantKnowledgeGaps | is_unaligned_to_standard | True | None |
| AbundantKnowledgeGaps | semantic_type_iri_field_count | 1 | None |
| AccessDenialTests | name | AccessDenialTests | None |
| AccessDenialTests | field_count | 18 | None |
| AccessDenialTests | policy_count | 2.0 | None |
| AccessDenialTests | has_measured_rows | True | None |
| AccessDenialTests | semantic_mapping_count | 1 | None |
| AccessDenialTests | is_unaligned_to_standard | True | None |
| AccessDenialTests | semantic_type_iri_field_count | 1 | None |
| AccessPolicies | name | AccessPolicies | None |
| AccessPolicies | field_count | 16 | None |
| AccessPolicies | policy_count | 2.0 | None |
| AccessPolicies | has_measured_rows | True | None |
| AccessPolicies | semantic_mapping_count | 1 | None |
| AccessPolicies | is_unaligned_to_standard | True | None |
| ... | ... | (2027 more) | ... |

### access_principals

- Fields: 40/160 (25.0%)
- Computed columns: name, organization_scope, role_label, policy_count, grant_count, visible_table_count, has_no_access, is_over_privileged

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| principal-cfo | name | Chief Financial Officer | None |
| principal-cfo | organization_scope | acme-finance | None |
| principal-cfo | role_label | Chief Financial Officer | None |
| principal-cfo | policy_count | 5 | None |
| principal-cfo | grant_count | 37 | None |
| principal-cfo | visible_table_count | 5 | None |
| principal-close-automation | name | Close Automation Operator | None |
| principal-close-automation | organization_scope | acme-finance | None |
| principal-close-automation | role_label | Close Automation Operator | None |
| principal-close-automation | policy_count | 3 | None |
| principal-close-automation | grant_count | 27.0 | None |
| principal-close-automation | visible_table_count | 3 | None |
| principal-communications-manager | name | Employee Communications Manage | None |
| principal-communications-manager | organization_scope | acme-people | None |
| principal-communications-manager | role_label | Employee Communications Manage | None |
| principal-communications-manager | policy_count | 6 | None |
| principal-communications-manager | grant_count | 57 | None |
| principal-communications-manager | visible_table_count | 6 | None |
| principal-controller | name | Corporate Controller | None |
| principal-controller | organization_scope | acme-finance | None |
| ... | ... | (100 more) | ... |

### access_policies

- Fields: 3213/5467 (58.8%)
- Computed columns: name, is_write_command, is_unrestricted, principal_is_admin, is_unrestricted_non_admin_grant, is_unwitnessed_write, denial_test_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pol-cfo-authority_boundaries-select | name | principal-cfo SELECT Authority | None |
| pol-cfo-authority_boundaries-select | is_unrestricted | True | None |
| pol-cfo-authority_boundaries-select | is_unrestricted_non_admin_grant | True | None |
| pol-cfo-change_requests-select | name | principal-cfo SELECT ChangeReq | None |
| pol-cfo-exceptions-select | name | principal-cfo SELECT Exception | None |
| pol-cfo-exceptions-select | is_unrestricted | True | None |
| pol-cfo-exceptions-select | is_unrestricted_non_admin_grant | True | None |
| pol-cfo-procedure_executions-select | name | principal-cfo SELECT Procedure | None |
| pol-cfo-procedure_executions-select | is_unrestricted | True | None |
| pol-cfo-procedure_executions-select | is_unrestricted_non_admin_grant | True | None |
| pol-cfo-procedures-select | name | principal-cfo SELECT Procedure | None |
| pol-cfo-procedures-select | is_unrestricted | True | None |
| pol-cfo-procedures-select | is_unrestricted_non_admin_grant | True | None |
| pol-closeautomation-errors-select | name | principal-close-automation SEL | None |
| pol-closeautomation-errors-select | is_unrestricted | True | None |
| pol-closeautomation-errors-select | is_unrestricted_non_admin_grant | True | None |
| pol-closeautomation-step_executions-select | name | principal-close-automation SEL | None |
| pol-closeautomation-step_executions-select | is_unrestricted | True | None |
| pol-closeautomation-step_executions-select | is_unrestricted_non_admin_grant | True | None |
| pol-closeautomation-steps-select | name | principal-close-automation SEL | None |
| ... | ... | (2234 more) | ... |

### field_grants

- Fields: 44243/129269 (34.2%)
- Computed columns: name, field_table, field_name, field_is_derived, is_writable_derived_field, is_masked, grant_key_when_readable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | name | principal-cfo -> AuthorityBoun | None |
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | field_name | AuthorityBoundaryId | None |
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | grant_key_when_readable | principal-cfo|AuthorityBoundar | None |
| fg-cfo-AuthorityBoundaries.AuthorityRole | name | principal-cfo -> AuthorityBoun | None |
| fg-cfo-AuthorityBoundaries.AuthorityRole | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.AuthorityRole | field_name | AuthorityRole | None |
| fg-cfo-AuthorityBoundaries.AuthorityRole | grant_key_when_readable | principal-cfo|AuthorityBoundar | None |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | name | principal-cfo -> AuthorityBoun | None |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | field_name | ForbiddenAgentKind | None |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | grant_key_when_readable | principal-cfo|AuthorityBoundar | None |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | name | principal-cfo -> AuthorityBoun | None |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | field_name | ForbiddenDecisionKind | None |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | grant_key_when_readable | principal-cfo|AuthorityBoundar | None |
| fg-cfo-AuthorityBoundaries.Name | name | principal-cfo -> AuthorityBoun | None |
| fg-cfo-AuthorityBoundaries.Name | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.Name | field_name | Name | None |
| fg-cfo-AuthorityBoundaries.Name | field_is_derived | True | None |
| ... | ... | (85006 more) | ... |

### role_schemas

- Fields: 20/80 (25.0%)
- Computed columns: name, search_path, view_count, is_empty_schema

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| schema-cfo | name | pko_cfo | None |
| schema-cfo | search_path | pko_cfo | None |
| schema-cfo | view_count | 5 | None |
| schema-close-automation | name | pko_close_automation | None |
| schema-close-automation | search_path | pko_close_automation | None |
| schema-close-automation | view_count | 3 | None |
| schema-communications-manager | name | pko_communications_manager | None |
| schema-communications-manager | search_path | pko_communications_manager | None |
| schema-communications-manager | view_count | 6 | None |
| schema-controller | name | pko_controller | None |
| schema-controller | search_path | pko_controller | None |
| schema-controller | view_count | 8 | None |
| schema-employment-counsel | name | pko_employment_counsel | None |
| schema-employment-counsel | search_path | pko_employment_counsel | None |
| schema-employment-counsel | view_count | 5 | None |
| schema-finance-analyst | name | pko_finance_analyst | None |
| schema-finance-analyst | search_path | pko_finance_analyst | None |
| schema-finance-analyst | view_count | 7 | None |
| schema-hr-policy-owner | name | pko_hr_policy_owner | None |
| schema-hr-policy-owner | search_path | pko_hr_policy_owner | None |
| ... | ... | (40 more) | ... |

### role_schema_views

- Fields: 810/6096 (13.3%)
- Computed columns: name, schema_name, source_view, grant_key, column_count, table_field_count, is_full_width, is_degenerate_view

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rsv-cfo-authority_boundaries | name | pko_cfo.authority_boundaries | None |
| rsv-cfo-authority_boundaries | schema_name | pko_cfo | None |
| rsv-cfo-authority_boundaries | source_view | vw_authority_boundaries | None |
| rsv-cfo-authority_boundaries | grant_key | principal-cfo|AuthorityBoundar | None |
| rsv-cfo-authority_boundaries | column_count | 7.0 | None |
| rsv-cfo-authority_boundaries | table_field_count | 33.0 | None |
| rsv-cfo-change_requests | name | pko_cfo.change_requests | None |
| rsv-cfo-change_requests | schema_name | pko_cfo | None |
| rsv-cfo-change_requests | source_view | vw_change_requests | None |
| rsv-cfo-change_requests | grant_key | principal-cfo|ChangeRequests | None |
| rsv-cfo-change_requests | column_count | 9.0 | None |
| rsv-cfo-change_requests | table_field_count | 46.0 | None |
| rsv-cfo-exceptions | name | pko_cfo.exceptions | None |
| rsv-cfo-exceptions | schema_name | pko_cfo | None |
| rsv-cfo-exceptions | source_view | vw_exceptions | None |
| rsv-cfo-exceptions | grant_key | principal-cfo|Exceptions | None |
| rsv-cfo-exceptions | column_count | 11.0 | None |
| rsv-cfo-exceptions | table_field_count | 11.0 | None |
| rsv-cfo-exceptions | is_full_width | True | None |
| rsv-cfo-procedure_executions | name | pko_cfo.procedure_executions | None |
| ... | ... | (5266 more) | ... |

### jwt_claim_mappings

- Fields: 4/8 (50.0%)
- Computed columns: name, usage_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| claim-email | name | email -> app.jwt_email() | None |
| claim-org | name | organization -> app.jwt_organi | None |
| claim-principal | name | principal -> app.jwt_principal | None |
| claim-tenant | name | tenant_id -> app.jwt_tenant_id | None |

### access_denial_tests

- Fields: 47/102 (46.1%)
- Computed columns: name, has_run, is_passing, is_leak, is_unproven, is_positive_control

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| allow-analyst-current-version | name | principal-finance-analyst must | None |
| allow-analyst-current-version | has_run | True | None |
| allow-analyst-current-version | is_passing | True | None |
| allow-analyst-current-version | is_positive_control | True | None |
| allow-controller-open-change-request | name | principal-controller must not  | None |
| allow-controller-open-change-request | has_run | True | None |
| allow-controller-open-change-request | is_passing | True | None |
| allow-controller-open-change-request | is_positive_control | True | None |
| allow-counsel-own-fragment | name | principal-employment-counsel m | None |
| allow-counsel-own-fragment | has_run | True | None |
| allow-counsel-own-fragment | is_passing | True | None |
| allow-counsel-own-fragment | is_positive_control | True | None |
| allow-hr-open-gap | name | principal-hr-policy-owner must | None |
| allow-hr-open-gap | has_run | True | None |
| allow-hr-open-gap | is_passing | True | None |
| allow-hr-open-gap | is_positive_control | True | None |
| deny-analyst-superseded-version | name | principal-finance-analyst must | None |
| deny-analyst-superseded-version | has_run | True | None |
| deny-analyst-superseded-version | is_passing | True | None |
| deny-cfo-send-intents | name | principal-cfo must not see  | None |
| ... | ... | (35 more) | ... |

### app_users

- Fields: 53/140 (37.9%)
- Computed columns: name, agent_kind, organization, assignment_count, has_no_principal, holds_multiple_principals, is_non_human_sign_in

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| user-aisha-bello | name | Aisha Bello | None |
| user-aisha-bello | agent_kind | Human | None |
| user-aisha-bello | organization | acme-plant | None |
| user-aisha-bello | assignment_count | 1.0 | None |
| user-amina-yusuf | name | Amina Yusuf | None |
| user-amina-yusuf | agent_kind | Human | None |
| user-amina-yusuf | organization | acme-people | None |
| user-amina-yusuf | assignment_count | 1 | None |
| user-claire-dubois | name | Claire Dubois | None |
| user-claire-dubois | agent_kind | Human | None |
| user-claire-dubois | organization | acme-home-brands | None |
| user-claire-dubois | assignment_count | 1.0 | None |
| user-close-pipeline | name | Close Pipeline | None |
| user-close-pipeline | agent_kind | AutomatedPipeline | None |
| user-close-pipeline | organization | acme-finance | None |
| user-close-pipeline | assignment_count | 1 | None |
| user-close-pipeline | is_non_human_sign_in | True | None |
| user-devon-okafor | name | Devon Okafor | None |
| user-devon-okafor | agent_kind | Human | None |
| user-devon-okafor | organization | acme-finance | None |
| ... | ... | (67 more) | ... |

### principal_assignments

- Fields: 42/110 (38.2%)
- Computed columns: name, principal_is_admin, user_organization, principal_organization, is_cross_organization_grant

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pa-aisha-bello-maintenance-technician | name | user-aisha-bello as principal- | None |
| pa-aisha-bello-maintenance-technician | user_organization | acme-plant | None |
| pa-aisha-bello-maintenance-technician | principal_organization | acme-plant | None |
| pa-amina-yusuf-communications-manager | name | user-amina-yusuf as principal- | None |
| pa-amina-yusuf-communications-manager | user_organization | acme-people | None |
| pa-amina-yusuf-communications-manager | principal_organization | acme-people | None |
| pa-claire-dubois-sourcing-manager | name | user-claire-dubois as principa | None |
| pa-claire-dubois-sourcing-manager | user_organization | acme-home-brands | None |
| pa-claire-dubois-sourcing-manager | principal_organization | acme-home-brands | None |
| pa-close-pipeline-close-automation | name | user-close-pipeline as princip | None |
| pa-close-pipeline-close-automation | user_organization | acme-finance | None |
| pa-close-pipeline-close-automation | principal_organization | acme-finance | None |
| pa-devon-okafor-controller | name | user-devon-okafor as principal | None |
| pa-devon-okafor-controller | user_organization | acme-finance | None |
| pa-devon-okafor-controller | principal_organization | acme-finance | None |
| pa-elena-garcia-hr-policy-owner | name | user-elena-garcia as principal | None |
| pa-elena-garcia-hr-policy-owner | user_organization | acme-people | None |
| pa-elena-garcia-hr-policy-owner | principal_organization | acme-people | None |
| pa-elena-garcia-process-steward | name | user-elena-garcia as principal | None |
| pa-elena-garcia-process-steward | principal_is_admin | True | None |
| ... | ... | (48 more) | ... |

### process_mining_runs

- Fields: 32/75 (42.7%)
- Computed columns: name, as_of_instant, conformance_rate, is_conformant, has_major_drift_from_documentation, days_since_mined, is_stale_mining_evidence, procedure_version_is_live, is_drift_on_live_version, drifted_mining_run_key, people_capture_complement_count, is_deviation_unexplained_by_people, undocumented_path_count, has_undocumented_enacted_path, conformance_percent

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pmr-close-v10-archived | name | ERP General Ledger Audit Log / | None |
| pmr-close-v10-archived | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| pmr-close-v10-archived | conformance_rate | 0.2 | None |
| pmr-close-v10-archived | has_major_drift_from_documentation | True | None |
| pmr-close-v10-archived | days_since_mined | 199 | None |
| pmr-close-v10-archived | is_stale_mining_evidence | True | None |
| pmr-close-v10-archived | is_deviation_unexplained_by_people | True | None |
| pmr-close-v10-archived | conformance_percent | 20 | None |
| pmr-close-v11-cutoff-bypass | name | ERP General Ledger Audit Log / | None |
| pmr-close-v11-cutoff-bypass | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| pmr-close-v11-cutoff-bypass | conformance_rate | 0.375 | None |
| pmr-close-v11-cutoff-bypass | has_major_drift_from_documentation | True | None |
| pmr-close-v11-cutoff-bypass | days_since_mined | 2 | None |
| pmr-close-v11-cutoff-bypass | procedure_version_is_live | True | None |
| pmr-close-v11-cutoff-bypass | is_drift_on_live_version | True | None |
| pmr-close-v11-cutoff-bypass | drifted_mining_run_key | close-v1.1.0 | None |
| pmr-close-v11-cutoff-bypass | is_deviation_unexplained_by_people | True | None |
| pmr-close-v11-cutoff-bypass | conformance_percent | 38 | None |
| pmr-close-v11-q3 | name | ERP General Ledger Audit Log / | None |
| pmr-close-v11-q3 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ... | ... | (23 more) | ... |

### vocabularies

- Fields: 138/216 (63.9%)
- Computed columns: name, term_count, orphan_term_count, has_orphan_terms, is_machine_accessible, managed_scheme_procedure_key, organized_transcript_count, organized_field_notes_count, organized_process_map_count, organized_mined_event_trace_count, organized_document_excerpt_count, organized_material_count, organized_kind_count, is_single_kind_frame, latest_organized_material_at, refinement_count, is_frozen_despite_new_collection, ontology_preceded_vocabulary_control

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| voc-agent-capabilities | name | Agent Capability Scheme | None |
| voc-agent-capabilities | term_count | 5.0 | None |
| voc-agent-capabilities | orphan_term_count | 5.0 | None |
| voc-agent-capabilities | has_orphan_terms | True | None |
| voc-agent-capabilities | is_machine_accessible | True | None |
| voc-artifact-types | name | Artifact Type Scheme | None |
| voc-artifact-types | term_count | 5.0 | None |
| voc-artifact-types | orphan_term_count | 5.0 | None |
| voc-artifact-types | has_orphan_terms | True | None |
| voc-artifact-types | is_machine_accessible | True | None |
| voc-close-controls | name | Close Control Categories | None |
| voc-close-controls | term_count | 5 | None |
| voc-close-controls | is_machine_accessible | True | None |
| voc-close-controls | managed_scheme_procedure_key | quarter-end-close | None |
| voc-conveyor-terms | name | Conveyor Maintenance Glossary | None |
| voc-conveyor-terms | organized_document_excerpt_count | 2 | None |
| voc-conveyor-terms | organized_material_count | 2 | None |
| voc-conveyor-terms | organized_kind_count | 1 | None |
| voc-conveyor-terms | is_single_kind_frame | True | None |
| voc-conveyor-terms | latest_organized_material_at | 2026-01-20T09:00:00-06:00 | None |
| ... | ... | (58 more) | ... |

### vocabulary_terms

- Fields: 580/792 (73.2%)
- Computed columns: name, usage_count, is_orphan_term, is_widely_adopted_term, orphan_term_vocabulary_key, broader_term_parent, scheme_governed_dimension, introduced_release_issued_at, latest_meaning_change_at, has_stale_definition, structural_shift_count, has_structural_sense_shift_across_years, pref_label_practitioner_mention_count, alt_label_practitioner_mention_count, is_organized_around_official_term, source_phrasing_count, unreconciled_phrasing_count, has_unreconciled_variant_phrasings

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| vt-accessibility | name | Accessibility Requirement | None |
| vt-accessibility | usage_count | 1 | None |
| vt-accessibility | scheme_governed_dimension | PolicyDomain | None |
| vt-artifact-drawing | name | engineering drawing | None |
| vt-artifact-drawing | is_orphan_term | True | None |
| vt-artifact-drawing | orphan_term_vocabulary_key | voc-artifact-types | None |
| vt-artifact-drawing | scheme_governed_dimension | ArtifactType | None |
| vt-artifact-register-entry | name | register entry | None |
| vt-artifact-register-entry | is_orphan_term | True | None |
| vt-artifact-register-entry | orphan_term_vocabulary_key | voc-artifact-types | None |
| vt-artifact-register-entry | scheme_governed_dimension | ArtifactType | None |
| vt-artifact-runbook | name | runbook | None |
| vt-artifact-runbook | is_orphan_term | True | None |
| vt-artifact-runbook | orphan_term_vocabulary_key | voc-artifact-types | None |
| vt-artifact-runbook | scheme_governed_dimension | ArtifactType | None |
| vt-artifact-sop | name | standard operating procedure | None |
| vt-artifact-sop | is_orphan_term | True | None |
| vt-artifact-sop | orphan_term_vocabulary_key | voc-artifact-types | None |
| vt-artifact-sop | scheme_governed_dimension | ArtifactType | None |
| vt-artifact-training-media | name | training media | None |
| ... | ... | (192 more) | ... |

### knowledge_broker_links

- Fields: 36/88 (40.9%)
- Computed columns: name, as_of_instant, days_since_consulted, is_active_reliance, broker_is_still_engaged, is_at_risk_reliance, active_reliance_broker_key, at_risk_broker_key, pointed_holder, locates_other_holder, translation_between_vocabularies

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kbl-amina-jordan-sod | name | amina-yusuf -> jordan-park | None |
| kbl-amina-jordan-sod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kbl-amina-jordan-sod | days_since_consulted | 18 | None |
| kbl-amina-jordan-sod | is_active_reliance | True | None |
| kbl-amina-jordan-sod | is_at_risk_reliance | True | None |
| kbl-amina-jordan-sod | active_reliance_broker_key | jordan-park | None |
| kbl-amina-jordan-sod | at_risk_broker_key | jordan-park | None |
| kbl-devon-priya-recon | name | devon-okafor -> priya-raman | None |
| kbl-devon-priya-recon | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kbl-devon-priya-recon | days_since_consulted | 29 | None |
| kbl-devon-priya-recon | is_active_reliance | True | None |
| kbl-devon-priya-recon | broker_is_still_engaged | True | None |
| kbl-devon-priya-recon | active_reliance_broker_key | priya-raman | None |
| kbl-elena-priya-sod | name | elena-garcia -> priya-raman | None |
| kbl-elena-priya-sod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kbl-elena-priya-sod | days_since_consulted | 14 | None |
| kbl-elena-priya-sod | is_active_reliance | True | None |
| kbl-elena-priya-sod | broker_is_still_engaged | True | None |
| kbl-elena-priya-sod | active_reliance_broker_key | priya-raman | None |
| kbl-maria-priya-recon | name | maria-chen -> priya-raman | None |
| ... | ... | (32 more) | ... |

### conformance_substrates

- Fields: 36/88 (40.9%)
- Computed columns: name, is_graded, run_count, latest_cells_tested, latest_cells_passed, latest_harness_errors, latest_cells_failed, latest_score, disagreeing_field_count, disagreeing_table_count, is_fully_conformant

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| compile-rulebook | name | compile-rulebook | None |
| effortless-entity-framework | name | C# / Entity Framework | None |
| effortless-entity-framework | is_graded | True | None |
| effortless-entity-framework | run_count | 5.0 | None |
| effortless-entity-framework | latest_cells_tested | 67616.0 | None |
| effortless-entity-framework | latest_cells_passed | 67616.0 | None |
| effortless-entity-framework | latest_score | 100.0 | None |
| effortless-entity-framework | is_fully_conformant | True | None |
| effortless-golang | name | Go | None |
| effortless-golang | is_graded | True | None |
| effortless-golang | run_count | 5.0 | None |
| effortless-golang | latest_cells_tested | 67616.0 | None |
| effortless-golang | latest_cells_passed | 67616.0 | None |
| effortless-golang | latest_score | 100.0 | None |
| effortless-golang | is_fully_conformant | True | None |
| effortless-owl | name | OWL / SHACL | None |
| effortless-owl | is_graded | True | None |
| effortless-owl | run_count | 5.0 | None |
| effortless-owl | latest_cells_tested | 67616.0 | None |
| effortless-owl | latest_cells_passed | 67616.0 | None |
| ... | ... | (32 more) | ... |

### conformance_runs

- Fields: 7/45 (15.6%)
- Computed columns: name, substrate_count, perfect_substrate_count, cells_tested, cells_passed, cells_failed, overall_score, imperfect_substrate_count, is_fully_conformant

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| run-20260913-165336 | name | run-20260913-165336 | None |
| run-20260913-165336 | substrate_count | 7 | None |
| run-20260913-165336 | cells_tested | 419146.0 | None |
| run-20260913-165336 | cells_passed | 412274.0 | None |
| run-20260913-165336 | cells_failed | 6872.0 | None |
| run-20260913-165336 | overall_score | 98.36 | None |
| run-20260913-165336 | imperfect_substrate_count | 7 | None |
| run-20260913-203118 | name | run-20260913-203118 | None |
| run-20260913-203118 | substrate_count | 7 | None |
| run-20260913-203118 | cells_tested | 508137.0 | None |
| run-20260913-203118 | cells_passed | 498748.0 | None |
| run-20260913-203118 | cells_failed | 9389.0 | None |
| run-20260913-203118 | overall_score | 98.15 | None |
| run-20260913-203118 | imperfect_substrate_count | 7 | None |
| run-20260914-083430 | name | run-20260914-083430 | None |
| run-20260914-083430 | substrate_count | 7.0 | None |
| run-20260914-083430 | perfect_substrate_count | 4.0 | None |
| run-20260914-083430 | cells_tested | 518238.0 | None |
| run-20260914-083430 | cells_passed | 515942.0 | None |
| run-20260914-083430 | cells_failed | 2296.0 | None |
| ... | ... | (18 more) | ... |

### substrate_run_scores

- Fields: 174/455 (38.2%)
- Computed columns: name, cells_failed, score, calculated_score, lookup_score, aggregation_score, is_perfect, perfect_run_key, is_in_latest_run, latest_cells_tested, latest_cells_passed, latest_error_flag, substrate_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| run-20260913-165336|effortless-entity-framework | name | run-20260913-165336 / effortle | None |
| run-20260913-165336|effortless-entity-framework | cells_failed | 114 | None |
| run-20260913-165336|effortless-entity-framework | score | 99.81 | None |
| run-20260913-165336|effortless-entity-framework | calculated_score | 99.74 | None |
| run-20260913-165336|effortless-entity-framework | lookup_score | 99.98 | None |
| run-20260913-165336|effortless-entity-framework | aggregation_score | 100.0 | None |
| run-20260913-165336|effortless-entity-framework | substrate_label | C# / Entity Framework | None |
| run-20260913-165336|effortless-golang | name | run-20260913-165336 / effortle | None |
| run-20260913-165336|effortless-golang | cells_failed | 114 | None |
| run-20260913-165336|effortless-golang | score | 99.81 | None |
| run-20260913-165336|effortless-golang | calculated_score | 99.74 | None |
| run-20260913-165336|effortless-golang | lookup_score | 99.98 | None |
| run-20260913-165336|effortless-golang | aggregation_score | 100.0 | None |
| run-20260913-165336|effortless-golang | substrate_label | Go | None |
| run-20260913-165336|effortless-owl | name | run-20260913-165336 / effortle | None |
| run-20260913-165336|effortless-owl | cells_failed | 291 | None |
| run-20260913-165336|effortless-owl | score | 99.51 | None |
| run-20260913-165336|effortless-owl | calculated_score | 99.48 | None |
| run-20260913-165336|effortless-owl | lookup_score | 99.95 | None |
| run-20260913-165336|effortless-owl | aggregation_score | 98.51 | None |
| ... | ... | (261 more) | ... |

### table_conformance

- Fields: 2469/5607 (44.0%)
- Computed columns: name, cells_failed, score, is_perfect, imperfect_substrate_key, imperfect_table_key, disagreeing_field_count, substrate_label, subject_area

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| effortless-entity-framework|AccessDenialTests | name | effortless-entity-framework /  | None |
| effortless-entity-framework|AccessDenialTests | score | 100.0 | None |
| effortless-entity-framework|AccessDenialTests | is_perfect | True | None |
| effortless-entity-framework|AccessDenialTests | substrate_label | C# / Entity Framework | None |
| effortless-entity-framework|AccessDenialTests | subject_area | access-control | None |
| effortless-entity-framework|AccessPolicies | name | effortless-entity-framework /  | None |
| effortless-entity-framework|AccessPolicies | score | 100.0 | None |
| effortless-entity-framework|AccessPolicies | is_perfect | True | None |
| effortless-entity-framework|AccessPolicies | substrate_label | C# / Entity Framework | None |
| effortless-entity-framework|AccessPolicies | subject_area | access-control | None |
| effortless-entity-framework|AccessPrincipals | name | effortless-entity-framework /  | None |
| effortless-entity-framework|AccessPrincipals | score | 100.0 | None |
| effortless-entity-framework|AccessPrincipals | is_perfect | True | None |
| effortless-entity-framework|AccessPrincipals | substrate_label | C# / Entity Framework | None |
| effortless-entity-framework|AccessPrincipals | subject_area | access-control | None |
| effortless-entity-framework|Actions | name | effortless-entity-framework /  | None |
| effortless-entity-framework|Actions | score | 100.0 | None |
| effortless-entity-framework|Actions | is_perfect | True | None |
| effortless-entity-framework|Actions | substrate_label | C# / Entity Framework | None |
| effortless-entity-framework|Actions | subject_area | specification | None |
| ... | ... | (3118 more) | ... |

### field_disagreements

- Fields: 0/135 (0.0%)
- Computed columns: name, sampled_cell_count, is_fully_sampled, formula, substrate_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes | name | effortless-xlsx / AgentDecisio | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes | sampled_cell_count | 3.0 | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes | is_fully_sampled | True | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes | formula | =IF({{ReviewedAt}} = "", 0, DA | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes | substrate_label | Excel | None |
| effortless-xlsx|FAQs.Name | name | effortless-xlsx / FAQs.Name | None |
| effortless-xlsx|FAQs.Name | sampled_cell_count | 3.0 | None |
| effortless-xlsx|FAQs.Name | is_fully_sampled | True | None |
| effortless-xlsx|FAQs.Name | formula | ={{Question}} | None |
| effortless-xlsx|FAQs.Name | substrate_label | Excel | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase | name | effortless-xlsx / MessageDeliv | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase | sampled_cell_count | 3.0 | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase | is_fully_sampled | True | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase | formula | ={{OptOutPhrasePosition}} > 0 | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase | substrate_label | Excel | None |
| effortless-xlsx|MessageDeliveries.IsMissingRequiredOptOut | name | effortless-xlsx / MessageDeliv | None |
| effortless-xlsx|MessageDeliveries.IsMissingRequiredOptOut | sampled_cell_count | 1.0 | None |
| effortless-xlsx|MessageDeliveries.IsMissingRequiredOptOut | is_fully_sampled | True | None |
| effortless-xlsx|MessageDeliveries.IsMissingRequiredOptOut | formula | =AND({{WasActuallyTransmitted} | None |
| effortless-xlsx|MessageDeliveries.IsMissingRequiredOptOut | substrate_label | Excel | None |
| ... | ... | (115 more) | ... |

### cell_disagreements

- Fields: 0/615 (0.0%)
- Computed columns: name, substrate, rulebook_field

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-extract-q2 | name | effortless-xlsx|AgentDecisionR | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-extract-q2 | substrate | effortless-xlsx | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-extract-q2 | rulebook_field | AgentDecisionRecords.ReviewLat | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-freeze-q2 | name | effortless-xlsx|AgentDecisionR | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-freeze-q2 | substrate | effortless-xlsx | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-freeze-q2 | rulebook_field | AgentDecisionRecords.ReviewLat | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-post-q2 | name | effortless-xlsx|AgentDecisionR | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-post-q2 | substrate | effortless-xlsx | None |
| effortless-xlsx|AgentDecisionRecords.ReviewLatencyMinutes|adr-close-post-q2 | rulebook_field | AgentDecisionRecords.ReviewLat | None |
| effortless-xlsx|FAQs.Name|faq-close-workpapers | name | effortless-xlsx|FAQs.Name @ fa | None |
| effortless-xlsx|FAQs.Name|faq-close-workpapers | substrate | effortless-xlsx | None |
| effortless-xlsx|FAQs.Name|faq-close-workpapers | rulebook_field | FAQs.Name | None |
| effortless-xlsx|FAQs.Name|faq-policy-ai | name | effortless-xlsx|FAQs.Name @ fa | None |
| effortless-xlsx|FAQs.Name|faq-policy-ai | substrate | effortless-xlsx | None |
| effortless-xlsx|FAQs.Name|faq-policy-ai | rulebook_field | FAQs.Name | None |
| effortless-xlsx|FAQs.Name|faq-policy-optout | name | effortless-xlsx|FAQs.Name @ fa | None |
| effortless-xlsx|FAQs.Name|faq-policy-optout | substrate | effortless-xlsx | None |
| effortless-xlsx|FAQs.Name|faq-policy-optout | rulebook_field | FAQs.Name | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase|md-001 | name | effortless-xlsx|MessageDeliver | None |
| effortless-xlsx|MessageDeliveries.HasOptOutPhrase|md-001 | substrate | effortless-xlsx | None |
| ... | ... | (595 more) | ... |

### knowledge_methods

- Fields: 29/130 (22.3%)
- Computed columns: name, elicitation_use_count, application_count, usage_count, is_applied

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| AgentIntegrationProtocols | name | Model Context Protocol and age | None |
| AgentIntegrationProtocols | application_count | 2 | None |
| AgentIntegrationProtocols | usage_count | 2 | None |
| AgentIntegrationProtocols | is_applied | True | None |
| ConceptLaddering | name | Concept laddering | None |
| ConceptLaddering | elicitation_use_count | 1 | None |
| ConceptLaddering | usage_count | 1 | None |
| ConceptLaddering | is_applied | True | None |
| CriticalIncidentTechnique | name | Critical incident technique | None |
| CriticalIncidentTechnique | elicitation_use_count | 1 | None |
| CriticalIncidentTechnique | usage_count | 1 | None |
| CriticalIncidentTechnique | is_applied | True | None |
| DocumentAnalysis | name | Document analysis | None |
| DocumentAnalysis | application_count | 1 | None |
| DocumentAnalysis | usage_count | 1 | None |
| DocumentAnalysis | is_applied | True | None |
| EnterpriseArchitecturePractice | name | Enterprise architecture practi | None |
| EnterpriseArchitecturePractice | application_count | 1 | None |
| EnterpriseArchitecturePractice | usage_count | 1 | None |
| EnterpriseArchitecturePractice | is_applied | True | None |
| ... | ... | (81 more) | ... |

### source_articles

- Fields: 5/40 (12.5%)
- Computed columns: name, claim_count, covered_claim_count, agreed_claim_count, uncovered_claim_count, coverage_percent, agreed_coverage_percent, is_fully_covered

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ont-4 | name | Ontology Series, Part IV: Gove | None |
| ont-4 | claim_count | 230 | None |
| ont-4 | covered_claim_count | 230 | None |
| ont-4 | agreed_claim_count | 229 | None |
| ont-4 | coverage_percent | 100.0 | None |
| ont-4 | agreed_coverage_percent | 99.6 | None |
| ont-4 | is_fully_covered | True | None |
| pkm-1 | name | Process Knowledge Management,  | None |
| pkm-1 | claim_count | 110 | None |
| pkm-1 | covered_claim_count | 109 | None |
| pkm-1 | agreed_claim_count | 108 | None |
| pkm-1 | uncovered_claim_count | 1 | None |
| pkm-1 | coverage_percent | 99.1 | None |
| pkm-1 | agreed_coverage_percent | 98.2 | None |
| pkm-2 | name | Process Knowledge Management,  | None |
| pkm-2 | claim_count | 283 | None |
| pkm-2 | covered_claim_count | 283 | None |
| pkm-2 | agreed_claim_count | 283 | None |
| pkm-2 | coverage_percent | 100.0 | None |
| pkm-2 | agreed_coverage_percent | 100.0 | None |
| ... | ... | (15 more) | ... |

### article_claims

- Fields: 910/7200 (12.6%)
- Computed columns: name, required_evidence, evidence_count, valid_evidence_count, agreed_evidence_count, is_covered, is_agreed, has_rejected_evidence

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ont4-c01 | name | ont4-c01: Drift: the gap that  | None |
| ont4-c01 | required_evidence | a table with rows or a field w | None |
| ont4-c01 | evidence_count | 1 | None |
| ont4-c01 | valid_evidence_count | 1 | None |
| ont4-c01 | agreed_evidence_count | 1 | None |
| ont4-c01 | is_covered | True | None |
| ont4-c01 | is_agreed | True | None |
| ont4-c02 | name | ont4-c02: A rule set stating w | None |
| ont4-c02 | required_evidence | a table with rows or a field w | None |
| ont4-c02 | evidence_count | 1 | None |
| ont4-c02 | valid_evidence_count | 1 | None |
| ont4-c02 | agreed_evidence_count | 1 | None |
| ont4-c02 | is_covered | True | None |
| ont4-c02 | is_agreed | True | None |
| ont4-c03 | name | ont4-c03: A defined response f | None |
| ont4-c03 | required_evidence | a table with rows or a field w | None |
| ont4-c03 | evidence_count | 1 | None |
| ont4-c03 | valid_evidence_count | 1 | None |
| ont4-c03 | agreed_evidence_count | 1 | None |
| ont4-c03 | is_covered | True | None |
| ... | ... | (6270 more) | ... |

### claim_evidence

- Fields: 10893/19800 (55.0%)
- Computed columns: name, claim_kind, field_catalog_name, field_is_witness, field_has_data, field_is_discriminating, field_is_contested, table_has_rows, question_is_answered, question_witnessed_answer, profile_mapping_count, method_is_applied, procedure_execution_count, has_justification, is_witness_proof, is_structural_proof, is_question_proof, is_standard_proof, is_scenario_proof, is_valid, is_contested, is_agreed_evidence

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ont4-c01-e1 | name | ont4-c01 <- Table | None |
| ont4-c01-e1 | claim_kind | Concept | None |
| ont4-c01-e1 | table_has_rows | True | None |
| ont4-c01-e1 | has_justification | True | None |
| ont4-c01-e1 | is_structural_proof | True | None |
| ont4-c01-e1 | is_valid | True | None |
| ont4-c01-e1 | is_agreed_evidence | True | None |
| ont4-c02-e1 | name | ont4-c02 <- Table | None |
| ont4-c02-e1 | claim_kind | Concept | None |
| ont4-c02-e1 | table_has_rows | True | None |
| ont4-c02-e1 | has_justification | True | None |
| ont4-c02-e1 | is_structural_proof | True | None |
| ont4-c02-e1 | is_valid | True | None |
| ont4-c02-e1 | is_agreed_evidence | True | None |
| ont4-c03-e1 | name | ont4-c03 <- Field | None |
| ont4-c03-e1 | claim_kind | Concept | None |
| ont4-c03-e1 | field_catalog_name | BreakageResponse | None |
| ont4-c03-e1 | field_is_witness | True | None |
| ont4-c03-e1 | field_has_data | True | None |
| ont4-c03-e1 | field_is_discriminating | True | None |
| ... | ... | (8887 more) | ... |

### method_applications

- Fields: 0/20 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ma-graph-retrieval-copilot | name | GraphRetrieval applied: Plant  | None |
| ma-owl-reasoning-0715 | name | OwlReasoning applied: Retrieva | None |
| ma-owl-reasoning-0718 | name | OwlReasoning applied: Hotfix r | None |
| ma10-lot-maintenance | name | LinkedOpenTerms applied: Maint | None |
| ma10-lot-requirements | name | LinkedOpenTerms applied: Gover | None |
| ma10-semver-releases | name | SemanticVersioning applied: Ru | None |
| ma12-sna-plant | name | SocialNetworkAnalysis applied: | None |
| ma13-document-analysis-press7 | name | DocumentAnalysis applied: Pres | None |
| ma13-process-mining-deploy | name | ProcessMining applied: CI/CD p | None |
| ma14-loto-process-map | name | ProcessMapping applied: Lockou | None |
| ma15-ea-repository-structure | name | EnterpriseArchitecturePractice | None |
| ma7-loto-polanyi | name | PolanyiTacitExplicitDistinctio | None |
| ma7-loto-seci | name | SeciConversion applied: Knowle | None |
| ma7-loto-value-stream | name | ValueStreamAnalysis applied: L | None |
| ma8-faceted-procedure-types | name | FacetedClassification applied: | None |
| ma8-layered-lockout | name | LayeredVocabularyToOntology ap | None |
| ma9-a2a-risk-classifier | name | AgentIntegrationProtocols appl | None |
| ma9-assistant-benchmarks | name | LlmEvaluationBenchmarks applie | None |
| ma9-embedding-tuning | name | ProcessDomainEmbeddingTuning a | None |
| ma9-mcp-copilot | name | AgentIntegrationProtocols appl | None |

### lifecycle_statuses

- Fields: 24/44 (54.5%)
- Computed columns: name, version_use_count, execution_use_count, is_non_pko_status_in_use

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Approval | name | Approval | None |
| Approved | name | Approved | None |
| Approved | version_use_count | 6 | None |
| Archived | name | Archived | None |
| Archived | version_use_count | 1 | None |
| Cancelled | name | Cancelled | None |
| Cancelled | execution_use_count | 1 | None |
| Completed | name | Completed | None |
| Completed | execution_use_count | 7 | None |
| Deprecated | name | Deprecated | None |
| Deprecated | version_use_count | 1 | None |
| Draft | name | Draft | None |
| InProgress | name | InProgress | None |
| InProgress | execution_use_count | 1 | None |
| Paused | name | Paused | None |
| Paused | execution_use_count | 1 | None |
| Published | name | Published | None |
| Published | version_use_count | 1 | None |
| Published | is_non_pko_status_in_use | True | None |
| Validation | name | Validation | None |

### facilities

- Fields: 6/12 (50.0%)
- Computed columns: name, deviating_run_count, clean_run_count, is_deviation_only_facility

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| acme-plant-site | name | ACME appliance plant site | None |
| plant-north | name | North assembly hall | None |
| plant-north | clean_run_count | 4 | None |
| plant-south | name | South press hall | None |
| plant-south | deviating_run_count | 1 | None |
| plant-south | is_deviation_only_facility | True | None |

### machine_types

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| belt-conveyor | name | Belt conveyor | None |
| hydraulic-press | name | Hydraulic press | None |
| industrial-mixer | name | Industrial mixer | None |

### energy_sources

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ElectricalEnergy | name | Electrical | None |
| GravitationalEnergy | name | Gravitational | None |
| HydraulicEnergy | name | Hydraulic | None |
| PneumaticEnergy | name | Pneumatic | None |

### machines

- Fields: 5/15 (33.3%)
- Computed columns: name, energy_source_count, unisolated_energy_source_count, is_non_standard_configuration, has_unisolated_energy_source

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| conveyor-3 | name | Conveyor line 3 | None |
| conveyor-3 | energy_source_count | 3 | None |
| mixer-2 | name | Mixer 2 (twin-drive conversion | None |
| mixer-2 | energy_source_count | 1 | None |
| mixer-2 | is_non_standard_configuration | True | None |
| press-7 | name | Press 7 (accumulator retrofit) | None |
| press-7 | energy_source_count | 2 | None |
| press-7 | unisolated_energy_source_count | 1 | None |
| press-7 | is_non_standard_configuration | True | None |
| press-7 | has_unisolated_energy_source | True | None |

### machine_energy_sources

- Fields: 11/30 (36.7%)
- Computed columns: name, machine_procedure_version, isolation_step_count, is_unisolated_energy_source, unisolated_machine_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| mes-conveyor3-electrical | name | conveyor-3 / ElectricalEnergy | None |
| mes-conveyor3-electrical | machine_procedure_version | loto-v2.0.0 | None |
| mes-conveyor3-electrical | isolation_step_count | 1 | None |
| mes-conveyor3-gravitational | name | conveyor-3 / GravitationalEner | None |
| mes-conveyor3-gravitational | machine_procedure_version | loto-v2.0.0 | None |
| mes-conveyor3-gravitational | isolation_step_count | 1 | None |
| mes-conveyor3-pneumatic | name | conveyor-3 / PneumaticEnergy | None |
| mes-conveyor3-pneumatic | machine_procedure_version | loto-v2.0.0 | None |
| mes-conveyor3-pneumatic | isolation_step_count | 1 | None |
| mes-mixer2-electrical | name | mixer-2 / ElectricalEnergy | None |
| mes-mixer2-electrical | machine_procedure_version | loto-v2.0.0 | None |
| mes-mixer2-electrical | isolation_step_count | 1 | None |
| mes-press7-electrical | name | press-7 / ElectricalEnergy | None |
| mes-press7-electrical | machine_procedure_version | loto-v2.0.0 | None |
| mes-press7-electrical | isolation_step_count | 1 | None |
| mes-press7-hydraulic | name | press-7 / HydraulicEnergy | None |
| mes-press7-hydraulic | machine_procedure_version | loto-v2.0.0 | None |
| mes-press7-hydraulic | is_unisolated_energy_source | True | None |
| mes-press7-hydraulic | unisolated_machine_key | press-7 | None |

### lock_devices

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| BallValveLockout | name | Ball valve lockout | None |
| CircuitBreakerLockout | name | Circuit breaker lockout | None |
| StandardPadlock | name | Standard padlock | None |

### protective_equipment

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Gloves | name | Gloves | None |
| HardHat | name | Hard hat | None |
| SafetyGlasses | name | Safety glasses | None |

### step_lock_requirements

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| slr-loto04a-breaker | name | loto-04a / CircuitBreakerLocko | None |
| slr-loto04b-valve | name | loto-04b / BallValveLockout | None |
| slr-loto05-padlock | name | loto-05 / StandardPadlock | None |

### step_protective_equipment

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| spe-loto04b-glasses | name | loto-04b / SafetyGlasses | None |
| spe-loto07-gloves | name | loto-07 / Gloves | None |
| spe-loto07-hardhat | name | loto-07 / HardHat | None |

### regulatory_frameworks

- Fields: 0/6 (0.0%)
- Computed columns: name, requirement_count, required_procedure_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| reg-change-control | name | Production change control poli | None |
| reg-change-control | requirement_count | 1 | None |
| reg-change-control | required_procedure_count | 1 | None |
| reg-hazardous-energy | name | Hazardous energy control stand | None |
| reg-hazardous-energy | requirement_count | 2 | None |
| reg-hazardous-energy | required_procedure_count | 2 | None |

### procedure_targets

- Fields: 14/24 (58.3%)
- Computed columns: name, machine_is_non_standard, handling_failure_mode_count, is_unhandled_non_standard_target

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| target-close-ledger | name | Corporate general ledger | None |
| target-deploy-platform | name | Customer-facing platform | None |
| target-loto-conveyor3 | name | Conveyor line 3 | None |
| target-loto-mixer2 | name | Mixer 2 | None |
| target-loto-mixer2 | machine_is_non_standard | True | None |
| target-loto-mixer2 | is_unhandled_non_standard_target | True | None |
| target-loto-press7 | name | Press 7 | None |
| target-loto-press7 | machine_is_non_standard | True | None |
| target-loto-press7 | handling_failure_mode_count | 1 | None |
| target-policy-workforce | name | Affected workforce | None |

### procedure_adoptions

- Fields: 2/6 (33.3%)
- Computed columns: name, is_adoption_mode_unstated

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| adopt-close-corp | name | acme-corp adopts quarter-end-c | None |
| adopt-deploy-plant | name | acme-plant adopts production-d | None |
| adopt-deploy-plant | is_adoption_mode_unstated | True | None |
| adopt-loto-corp | name | acme-corp adopts lockout-tagou | None |

### procedure_outcome_criteria

- Fields: 3/10 (30.0%)
- Computed columns: name, failure_procedure_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| crit-close-success | name | quarter-end-close Success | None |
| crit-deploy-failure | name | production-deployment Failure | None |
| crit-deploy-failure | failure_procedure_key | production-deployment | None |
| crit-deploy-success | name | production-deployment Success | None |
| crit-loto-failure | name | lockout-tagout Failure | None |
| crit-loto-failure | failure_procedure_key | lockout-tagout | None |
| crit-loto-success | name | lockout-tagout Success | None |

### relation_types

- Fields: 7/22 (31.8%)
- Computed columns: name, usage_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Blocks | name | blocks | None |
| Blocks | usage_count | 1 | None |
| Enables | name | enables | None |
| Enables | usage_count | 2 | None |
| GovernedBy | name | governed by | None |
| Overlaps | name | overlaps | None |
| Overlaps | usage_count | 1 | None |
| PartOf | name | part of | None |
| PerformedBy | name | performed by | None |
| Precedes | name | precedes | None |
| Prevents | name | prevents | None |
| Prevents | usage_count | 1 | None |
| Produces | name | produces | None |
| Requires | name | requires | None |
| ResponsibleFor | name | responsible for | None |

### activity_relations

- Fields: 16/35 (45.7%)
- Computed columns: name, relation_type_is_defined, uses_undefined_relation_type, from_step_version, overlaps_version_key, enables_version_key, prevents_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ar-deploy02-enables-03 | name | deploy-02 Enables deploy-03 | None |
| ar-deploy02-enables-03 | relation_type_is_defined | True | None |
| ar-deploy02-enables-03 | from_step_version | deploy-v3.2.0 | None |
| ar-deploy02-enables-03 | enables_version_key | deploy-v3.2.0 | None |
| ar-loto02-overlaps-03 | name | loto-02 Overlaps loto-03 | None |
| ar-loto02-overlaps-03 | relation_type_is_defined | True | None |
| ar-loto02-overlaps-03 | from_step_version | loto-v2.0.0 | None |
| ar-loto02-overlaps-03 | overlaps_version_key | loto-v2.0.0 | None |
| ar-loto04c-blocks-07 | name | loto-04c Blocks loto-07 | None |
| ar-loto04c-blocks-07 | uses_undefined_relation_type | True | None |
| ar-loto04c-blocks-07 | from_step_version | loto-v2.0.0 | None |
| ar-loto05-prevents-08 | name | loto-05 Prevents loto-08 | None |
| ar-loto05-prevents-08 | relation_type_is_defined | True | None |
| ar-loto05-prevents-08 | from_step_version | loto-v2.0.0 | None |
| ar-loto05-prevents-08 | prevents_version_key | loto-v2.0.0 | None |
| ar-loto06-enables-07 | name | loto-06 Enables loto-07 | None |
| ar-loto06-enables-07 | relation_type_is_defined | True | None |
| ar-loto06-enables-07 | from_step_version | loto-v2.0.0 | None |
| ar-loto06-enables-07 | enables_version_key | loto-v2.0.0 | None |

### step_variables

- Fields: 175/357 (49.0%)
- Computed columns: name, source_direction, source_step, is_dangling_input, is_miswired_source, consumer_count, input_step_key, output_step_key, step_accountable_agent, step_agent_kind, consumer_role, consumer_workflow, source_step_agent, source_step_agent_kind, is_input_from_ai_artifact, ai_artifact_consumer_has_no_accountable_agent, ai_blast_radius_path

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| var-deploy01-out-candidate | name | deploy-01 Output Release candi | None |
| var-deploy01-out-candidate | consumer_count | 2 | None |
| var-deploy01-out-candidate | output_step_key | deploy-01 | None |
| var-deploy01-out-candidate | step_accountable_agent | deploy-pipeline | None |
| var-deploy01-out-candidate | step_agent_kind | AutomatedPipeline | None |
| var-deploy01-out-candidate | consumer_role | deployment-automation | None |
| var-deploy01-out-candidate | consumer_workflow | deploy-v3.2.0 | None |
| var-deploy02-in-candidate | name | deploy-02 Input Release candid | None |
| var-deploy02-in-candidate | source_direction | Output | None |
| var-deploy02-in-candidate | source_step | deploy-01 | None |
| var-deploy02-in-candidate | consumer_count | 1 | None |
| var-deploy02-in-candidate | input_step_key | deploy-02 | None |
| var-deploy02-in-candidate | step_accountable_agent | risk-classifier-2-4-1 | None |
| var-deploy02-in-candidate | step_agent_kind | AIAgent | None |
| var-deploy02-in-candidate | consumer_role | change-risk-classifier | None |
| var-deploy02-in-candidate | consumer_workflow | deploy-v3.2.0 | None |
| var-deploy02-in-candidate | source_step_agent | deploy-pipeline | None |
| var-deploy02-in-candidate | source_step_agent_kind | AutomatedPipeline | None |
| var-deploy02-out-risk | name | deploy-02 Output Change risk c | None |
| var-deploy02-out-risk | consumer_count | 1 | None |
| ... | ... | (162 more) | ... |

### execution_entities

- Fields: 71/195 (36.4%)
- Computed columns: name, variable_direction, variable_step, executed_step, is_usage_direction_mismatch, is_foreign_variable, used_execution_key, generated_execution_key, variable_datatype, variable_expected_format, violates_declared_datatype_or_format, generating_agent, attributed_to_agent

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ee-0708-03-wo | name | se-loto0708-03 Used Work order | None |
| ee-0708-03-wo | variable_direction | Input | None |
| ee-0708-03-wo | variable_step | loto-03 | None |
| ee-0708-03-wo | executed_step | loto-03 | None |
| ee-0708-03-wo | used_execution_key | se-loto0708-03 | None |
| ee-0708-03-wo | variable_datatype | WorkOrder | None |
| ee-0708-03-wo | variable_expected_format | text/uri-list | None |
| ee-0708-03-wo | generating_agent | tomas-reyes | None |
| ee-0708-04-iso | name | se-loto0708-04 Generated Isola | None |
| ee-0708-04-iso | variable_direction | Output | None |
| ee-0708-04-iso | variable_step | loto-04 | None |
| ee-0708-04-iso | executed_step | loto-04 | None |
| ee-0708-04-iso | generated_execution_key | se-loto0708-04 | None |
| ee-0708-04-iso | variable_datatype | IsolationList | None |
| ee-0708-04-iso | generating_agent | tomas-reyes | None |
| ee-0708-04-iso | attributed_to_agent | tomas-reyes | None |
| ee-0708-06-iso | name | se-loto0708-06 Used Isolation  | None |
| ee-0708-06-iso | variable_direction | Input | None |
| ee-0708-06-iso | variable_step | loto-06 | None |
| ee-0708-06-iso | executed_step | loto-06 | None |
| ... | ... | (104 more) | ... |

### step_conditions

- Fields: 29/56 (51.8%)
- Computed columns: name, check_count, is_never_checked, precondition_step_key, postcondition_step_key, invariant_step_key, safety_critical_step_key, is_machine_parseable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cond-deploy04-pre-approved | name | deploy-04 Precondition | None |
| cond-deploy04-pre-approved | check_count | 1 | None |
| cond-deploy04-pre-approved | precondition_step_key | deploy-04 | None |
| cond-loto03-pre-notified | name | loto-03 Precondition | None |
| cond-loto03-pre-notified | check_count | 2 | None |
| cond-loto03-pre-notified | precondition_step_key | loto-03 | None |
| cond-loto05-inv-inventory | name | loto-05 Invariant | None |
| cond-loto05-inv-inventory | is_never_checked | True | None |
| cond-loto05-inv-inventory | invariant_step_key | loto-05 | None |
| cond-loto05-inv-inventory | safety_critical_step_key | loto-05 | None |
| cond-loto05-inv-inventory | is_machine_parseable | True | None |
| cond-loto06-post-zero | name | loto-06 Postcondition | None |
| cond-loto06-post-zero | is_never_checked | True | None |
| cond-loto06-post-zero | postcondition_step_key | loto-06 | None |
| cond-loto06-post-zero | safety_critical_step_key | loto-06 | None |
| cond-loto07-inv-locks | name | loto-07 Invariant | None |
| cond-loto07-inv-locks | check_count | 2 | None |
| cond-loto07-inv-locks | invariant_step_key | loto-07 | None |
| cond-loto07-inv-locks | safety_critical_step_key | loto-07 | None |
| cond-loto07-pre-zeroenergy | name | loto-07 Precondition | None |
| ... | ... | (7 more) | ... |

### condition_checks

- Fields: 24/42 (57.1%)
- Computed columns: name, condition_kind, is_failed_precondition, is_violated_invariant, failed_precondition_execution_key, violated_invariant_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cc-0708-03-notified | name | se-loto0708-03 / cond-loto03-p | None |
| cc-0708-03-notified | condition_kind | Precondition | None |
| cc-0708-07-locks | name | se-loto0708-07 / cond-loto07-i | None |
| cc-0708-07-locks | condition_kind | Invariant | None |
| cc-0708-07-zero | name | se-loto0708-07 / cond-loto07-p | None |
| cc-0708-07-zero | condition_kind | Precondition | None |
| cc-0709-03-notified | name | se-loto0709-03 / cond-loto03-p | None |
| cc-0709-03-notified | condition_kind | Precondition | None |
| cc-0709-07-locks | name | se-loto0709-07 / cond-loto07-i | None |
| cc-0709-07-locks | condition_kind | Invariant | None |
| cc-0709-07-locks | is_violated_invariant | True | None |
| cc-0709-07-locks | violated_invariant_execution_key | se-loto0709-07 | None |
| cc-0709-07-zero | name | se-loto0709-07 / cond-loto07-p | None |
| cc-0709-07-zero | condition_kind | Precondition | None |
| cc-0709-07-zero | is_failed_precondition | True | None |
| cc-0709-07-zero | failed_precondition_execution_key | se-loto0709-07 | None |
| cc-dep0301-04-approved | name | se-dep0301-04 / cond-deploy04- | None |
| cc-dep0301-04-approved | condition_kind | Precondition | None |

### failure_modes

- Fields: 10/20 (50.0%)
- Computed columns: name, escalation_role_has_no_holder, escalates_to_vacant_role, has_no_response

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| fm-deploy02-unscorable | name | deploy-02: The classifier cann | None |
| fm-deploy02-unscorable | escalation_role_has_no_holder | True | None |
| fm-deploy02-unscorable | has_no_response | True | None |
| fm-loto03-press7 | name | loto-03: The retrofitted press | None |
| fm-loto04b-repressurize | name | loto-04b: The pneumatic line r | None |
| fm-loto04b-repressurize | escalation_role_has_no_holder | True | None |
| fm-loto06-night | name | loto-06: Residual energy found | None |
| fm-loto06-night | escalation_role_has_no_holder | True | None |
| fm-loto06-night | escalates_to_vacant_role | True | None |
| fm-loto06-residual | name | loto-06: Residual stored energ | None |

### step_cues

- Fields: 14/28 (50.0%)
- Computed columns: name, observation_count, unescalated_observation_count, danger_cue_step_key, incomplete_cue_step_key, failure_mode_response, is_unanswerable_sign

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cue-loto03-rundown | name | loto-03 Visual | None |
| cue-loto04b-hiss | name | loto-04b Sound | None |
| cue-loto04b-hiss | observation_count | 1 | None |
| cue-loto04b-hiss | unescalated_observation_count | 1 | None |
| cue-loto04b-hiss | danger_cue_step_key | loto-04b | None |
| cue-loto04b-hiss | incomplete_cue_step_key | loto-04b | None |
| cue-loto04b-hiss | is_unanswerable_sign | True | None |
| cue-loto06-gauge | name | loto-06 Indicator | None |
| cue-loto06-gauge | observation_count | 1 | None |
| cue-loto06-gauge | danger_cue_step_key | loto-06 | None |
| cue-loto06-gauge | incomplete_cue_step_key | loto-06 | None |
| cue-loto06-gauge | failure_mode_response | Keep every lock on, stop, esca | None |
| cue-loto06-tryout | name | loto-06 Confirmation | None |
| cue-loto06-tryout | observation_count | 1 | None |

### cue_observations

- Fields: 10/24 (41.7%)
- Computed columns: name, cue_requires_escalation, is_unescalated_danger_cue, unescalated_cue_key, unescalated_execution_key, owner_organization, cue_signals_incomplete_step, is_awaiting_acknowledgement

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| obs-0708-gauge | name | se-loto0708-06 / cue-loto06-ga | None |
| obs-0708-gauge | cue_requires_escalation | True | None |
| obs-0708-gauge | owner_organization | acme-plant | None |
| obs-0708-gauge | cue_signals_incomplete_step | True | None |
| obs-0708-gauge | is_awaiting_acknowledgement | True | None |
| obs-0709-hiss | name | se-loto0709-04b / cue-loto04b- | None |
| obs-0709-hiss | cue_requires_escalation | True | None |
| obs-0709-hiss | is_unescalated_danger_cue | True | None |
| obs-0709-hiss | unescalated_cue_key | cue-loto04b-hiss | None |
| obs-0709-hiss | unescalated_execution_key | se-loto0709-04b | None |
| obs-0709-hiss | owner_organization | acme-plant | None |
| obs-0709-hiss | cue_signals_incomplete_step | True | None |
| obs-0709-tryout | name | se-loto0709-06 / cue-loto06-tr | None |
| obs-0709-tryout | owner_organization | acme-plant | None |

### decision_points

- Fields: 5/15 (33.3%)
- Computed columns: name, has_no_deciding_factors, is_dmn_encoded

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dp-close05-expedite | name | close-05: Is aggregate varianc | None |
| dp-close05-expedite | has_no_deciding_factors | True | None |
| dp-close05-expedite | is_dmn_encoded | True | None |
| dp-deploy02-unscorable | name | deploy-02: Can the classifier  | None |
| dp-deploy03-approve | name | deploy-03: Approve the release | None |
| dp-deploy03-approve | is_dmn_encoded | True | None |
| dp-loto06-reverify | name | loto-06: Is a reading doubtful | None |
| dp-loto06-reverify | is_dmn_encoded | True | None |
| dp-loto06-zero | name | loto-06: Does every isolation  | None |
| dp-loto06-zero | is_dmn_encoded | True | None |

### execution_participants

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ep-0708-lin | name | exec-loto-0708-north-day / lin | None |
| ep-0714-lin | name | exec-loto-0714-north-day / lin | None |
| ep-0714-tomas | name | exec-loto-0714-north-day / tom | None |
| ep-dep0301-omar | name | exec-deploy-2026-03-01 / omar- | None |

### step_resources

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sres-loto03-press7map | name | loto-03 / res-loto-isolation-m | None |
| sres-loto07-video | name | loto-07 / res-loto-training-vi | None |

### faq_categories

- Fields: 0/8 (0.0%)
- Computed columns: name, faq_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Authority | name | Authority | None |
| Authority | faq_count | 1 | None |
| Consent | name | Consent | None |
| Consent | faq_count | 1 | None |
| Safety | name | Safety | None |
| Safety | faq_count | 1 | None |
| Storage | name | Storage | None |
| Storage | faq_count | 1 | None |

### faq_targets

- Fields: 0/4 (0.0%)
- Computed columns: name, faq_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Procedure | name | Procedure | None |
| Procedure | faq_count | 2 | None |
| Step | name | Step | None |
| Step | faq_count | 2 | None |

### authoring_submissions

- Fields: 4/9 (44.4%)
- Computed columns: name, is_expert_authored_conforming, is_non_conforming_accepted

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sub-convmaint-ken | name | convmaint-v1.0.0 by ken-watana | None |
| sub-convmaint-ken | is_non_conforming_accepted | True | None |
| sub-deploy-sam | name | deploy-v3.2.0 by sam-adeyemi | None |
| sub-loto2-tomas | name | loto-v2.0.0 by tomas-reyes | None |
| sub-loto2-tomas | is_expert_authored_conforming | True | None |

### process_knowledge_levels

- Fields: 3/12 (25.0%)
- Computed columns: name, tacit_strategy_count, explicit_strategy_count, lacks_capture_strategy_for_either_form

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| operational | name | Operational | None |
| operational | tacit_strategy_count | 1 | None |
| operational | explicit_strategy_count | 1 | None |
| strategic | name | Strategic | None |
| strategic | explicit_strategy_count | 1 | None |
| strategic | lacks_capture_strategy_for_either_form | True | None |
| tactical | name | Tactical | None |
| tactical | tacit_strategy_count | 1 | None |
| tactical | explicit_strategy_count | 1 | None |

### level_capture_strategies

- Fields: 4/10 (40.0%)
- Computed columns: name, contradicts_knowledge_form

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| lcs-operational-explicit | name | operational Explicit: Steps, c | None |
| lcs-operational-tacit | name | operational Tacit: New technic | None |
| lcs-strategic-explicit | name | strategic Explicit: Annual saf | None |
| lcs-tactical-explicit | name | tactical Explicit: Waiting tim | None |
| lcs-tactical-tacit | name | tactical Tacit: Shift planners | None |
| lcs-tactical-tacit | contradicts_knowledge_form | True | None |

### level_pyramid_questions

- Fields: 0/5 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| operational|how | name | operational answers how | None |
| operational|where | name | operational answers where | None |
| strategic|who | name | strategic answers who | None |
| strategic|why | name | strategic answers why | None |
| tactical|what | name | tactical answers what | None |

### process_level_statements

- Fields: 8/32 (25.0%)
- Computed columns: name, level_question_key, pyramid_match_count, is_filed_at_wrong_level

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pls-deploy-how | name | production-deployment operatio | None |
| pls-deploy-how | level_question_key | operational|how | None |
| pls-deploy-how | pyramid_match_count | 1 | None |
| pls-deploy-what | name | production-deployment tactical | None |
| pls-deploy-what | level_question_key | tactical|what | None |
| pls-deploy-what | pyramid_match_count | 1 | None |
| pls-deploy-why-in-steps | name | production-deployment operatio | None |
| pls-deploy-why-in-steps | level_question_key | operational|why | None |
| pls-deploy-why-in-steps | is_filed_at_wrong_level | True | None |
| pls-loto-how | name | lockout-tagout operational how | None |
| pls-loto-how | level_question_key | operational|how | None |
| pls-loto-how | pyramid_match_count | 1 | None |
| pls-loto-what | name | lockout-tagout tactical what | None |
| pls-loto-what | level_question_key | tactical|what | None |
| pls-loto-what | pyramid_match_count | 1 | None |
| pls-loto-where | name | lockout-tagout operational whe | None |
| pls-loto-where | level_question_key | operational|where | None |
| pls-loto-where | pyramid_match_count | 1 | None |
| pls-loto-who | name | lockout-tagout strategic who | None |
| pls-loto-who | level_question_key | strategic|who | None |
| ... | ... | (4 more) | ... |

### tactical_resource_allocations

- Fields: 3/15 (20.0%)
- Computed columns: name, utilization_percent, is_bottleneck

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tra-deploy01-runners | name | deploy-01 / CI runner hours | None |
| tra-deploy01-runners | utilization_percent | 80.0 | None |
| tra-deploy03-approver | name | deploy-03 / Release manager ap | None |
| tra-deploy03-approver | utilization_percent | 60.0 | None |
| tra-loto05-padlocks-north | name | loto-05 / Personal padlocks at | None |
| tra-loto05-padlocks-north | utilization_percent | 115.0 | None |
| tra-loto05-padlocks-north | is_bottleneck | True | None |
| tra-loto06-gauges | name | loto-06 / Calibrated pressure  | None |
| tra-loto06-gauges | utilization_percent | 60.0 | None |
| tra-loto06-night-verifier | name | loto-06 / Night-shift zero-ene | None |
| tra-loto06-night-verifier | utilization_percent | 300.0 | None |
| tra-loto06-night-verifier | is_bottleneck | True | None |

### process_strategic_alignments

- Fields: 3/8 (37.5%)
- Computed columns: name, states_no_trade_off

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| psa-close | name | quarter-end-close serves acme- | None |
| psa-convmaint | name | conveyor-maintenance serves ac | None |
| psa-convmaint | states_no_trade_off | True | None |
| psa-deploy | name | production-deployment serves a | None |
| psa-loto | name | lockout-tagout serves acme-pla | None |

### business_outcomes

- Fields: 0/6 (0.0%)
- Computed columns: name, linked_measure_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bo-customer-delivery | name | Features delivered to customer | None |
| bo-customer-delivery | linked_measure_count | 1 | None |
| bo-injury-free-plant | name | Recordable injury rate | None |
| bo-injury-free-plant | linked_measure_count | 2 | None |
| bo-platform-availability | name | Platform availability | None |
| bo-platform-availability | linked_measure_count | 1 | None |

### process_outcome_measures

- Fields: 4/10 (40.0%)
- Computed columns: name, is_unlinked_to_business_outcome

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pom-convmaint-rollers | name | conveyor-maintenance: Rollers  | None |
| pom-convmaint-rollers | is_unlinked_to_business_outcome | True | None |
| pom-deploy-change-failure | name | production-deployment: Change  | None |
| pom-deploy-lead-time | name | production-deployment: Lead ti | None |
| pom-loto-deviation-free | name | lockout-tagout: Lockouts compl | None |
| pom-loto-first-pass | name | lockout-tagout: Zero-energy ve | None |

### process_stages

- Fields: 12/28 (42.9%)
- Computed columns: name, step_count, owner_has_no_holder, is_unowned_or_empty_stage

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| stage-deploy-approve | name | deploy-v3.2.0 stage Approve | None |
| stage-deploy-approve | step_count | 1 | None |
| stage-deploy-build-classify | name | deploy-v3.2.0 stage Build and  | None |
| stage-deploy-build-classify | step_count | 2 | None |
| stage-deploy-rollout-verify | name | deploy-v3.2.0 stage Roll out a | None |
| stage-deploy-rollout-verify | step_count | 2 | None |
| stage-deploy-rollout-verify | owner_has_no_holder | True | None |
| stage-deploy-rollout-verify | is_unowned_or_empty_stage | True | None |
| stage-loto-group-lockbox | name | loto-v2.0.0 stage Group lockbo | None |
| stage-loto-group-lockbox | is_unowned_or_empty_stage | True | None |
| stage-loto-isolate-verify | name | loto-v2.0.0 stage Isolate and  | None |
| stage-loto-isolate-verify | step_count | 7 | None |
| stage-loto-prepare | name | loto-v2.0.0 stage Prepare and  | None |
| stage-loto-prepare | step_count | 3 | None |
| stage-loto-service-restore | name | loto-v2.0.0 stage Service and  | None |
| stage-loto-service-restore | step_count | 2 | None |

### process_interdependencies

- Fields: 4/12 (33.3%)
- Computed columns: name, is_hindering_dependency, hindered_procedure_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pi-convmaint-requires-loto | name | conveyor-maintenance Requires  | None |
| pi-deploy-hinders-close | name | production-deployment Hinders  | None |
| pi-deploy-hinders-close | is_hindering_dependency | True | None |
| pi-deploy-hinders-close | hindered_procedure_key | quarter-end-close | None |
| pi-loto-helps-convmaint | name | lockout-tagout Helps conveyor- | None |
| pi-loto-hinders-press-changeover | name | lockout-tagout Hinders press-c | None |
| pi-loto-hinders-press-changeover | is_hindering_dependency | True | None |
| pi-loto-hinders-press-changeover | hindered_procedure_key | press-changeover | None |

### stakeholder_lenses

- Fields: 0/18 (0.0%)
- Computed columns: name, granularity_rank

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| lens-ai-system | name | AI system | None |
| lens-ai-system | granularity_rank | 3 | None |
| lens-compliance-auditor | name | Compliance auditor | None |
| lens-compliance-auditor | granularity_rank | 3 | None |
| lens-customer-service | name | Customer service agent | None |
| lens-customer-service | granularity_rank | 3 | None |
| lens-executive | name | Executive | None |
| lens-executive | granularity_rank | 1 | None |
| lens-infrastructure-engineer | name | Infrastructure engineer | None |
| lens-infrastructure-engineer | granularity_rank | 2 | None |
| lens-operations-manager | name | Operations manager | None |
| lens-operations-manager | granularity_rank | 2 | None |
| lens-product-manager | name | Product manager | None |
| lens-product-manager | granularity_rank | 2 | None |
| lens-strategic-planner | name | Strategic planner | None |
| lens-strategic-planner | granularity_rank | 1 | None |
| lens-trainer | name | Trainer | None |
| lens-trainer | granularity_rank | 3 | None |

### procedure_lens_views

- Fields: 43/120 (35.8%)
- Computed columns: name, procedure_current_version, lens_granularity_rank, view_granularity_rank, is_granularity_misfit, is_disconnected_silo, step_level_procedure_key, category_level_procedure_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| plv-convmaint-opsmgr | name | conveyor-maintenance for lens- | None |
| plv-convmaint-opsmgr | procedure_current_version | convmaint-v1.0.0 | None |
| plv-convmaint-opsmgr | lens_granularity_rank | 2 | None |
| plv-convmaint-opsmgr | view_granularity_rank | 2 | None |
| plv-convmaint-trainer | name | conveyor-maintenance for lens- | None |
| plv-convmaint-trainer | procedure_current_version | convmaint-v1.0.0 | None |
| plv-convmaint-trainer | lens_granularity_rank | 3 | None |
| plv-convmaint-trainer | view_granularity_rank | 3 | None |
| plv-convmaint-trainer | step_level_procedure_key | conveyor-maintenance | None |
| plv-deploy-ai | name | production-deployment for lens | None |
| plv-deploy-ai | procedure_current_version | deploy-v3.2.0 | None |
| plv-deploy-ai | lens_granularity_rank | 3 | None |
| plv-deploy-ai | view_granularity_rank | 3 | None |
| plv-deploy-ai | step_level_procedure_key | production-deployment | None |
| plv-deploy-executive | name | production-deployment for lens | None |
| plv-deploy-executive | procedure_current_version | deploy-v3.2.0 | None |
| plv-deploy-executive | lens_granularity_rank | 1 | None |
| plv-deploy-executive | view_granularity_rank | 1 | None |
| plv-deploy-executive | is_disconnected_silo | True | None |
| plv-deploy-executive | category_level_procedure_key | production-deployment | None |
| ... | ... | (57 more) | ... |

### applicability_scopes

- Fields: 1/15 (6.7%)
- Computed columns: name, dimension_count, states_conditions

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| scope-deploy-enterprise-eu | name | EU enterprise tenants | None |
| scope-deploy-enterprise-eu | dimension_count | 4 | None |
| scope-deploy-enterprise-eu | states_conditions | True | None |
| scope-deploy-hotfix | name | Hotfix deployments | None |
| scope-deploy-hotfix | dimension_count | 1 | None |
| scope-plant-north-day | name | North assembly hall, day shift | None |
| scope-plant-north-day | dimension_count | 3 | None |
| scope-plant-north-day | states_conditions | True | None |
| scope-plant-south-night | name | South press hall, night shift | None |
| scope-plant-south-night | dimension_count | 3 | None |
| scope-plant-south-night | states_conditions | True | None |
| scope-press7-retrofit | name | Press 7 with accumulator retro | None |
| scope-press7-retrofit | dimension_count | 3 | None |
| scope-press7-retrofit | states_conditions | True | None |

### step_context_sensitivities

- Fields: 3/8 (37.5%)
- Computed columns: name, unscoped_step_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| scs-deploy03-freeze | name | deploy-03 / Circumstance | None |
| scs-loto03-press7 | name | loto-03 / Circumstance | None |
| scs-loto03-press7 | unscoped_step_key | loto-03 | None |
| scs-loto05-padlocks | name | loto-05 / Resource | None |
| scs-loto06-night-performer | name | loto-06 / Performer | None |

### situational_variants

- Fields: 8/20 (40.0%)
- Computed columns: name, scope_dimension_count, scope_states_conditions, has_no_applicability_dimension, diverges_without_stated_conditions

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| var-deploy-eu-freeze | name | EU change-freeze hold | None |
| var-deploy-eu-freeze | scope_dimension_count | 4 | None |
| var-deploy-eu-freeze | scope_states_conditions | True | None |
| var-deploy-hotfix-no-canary | name | Hotfix without canary | None |
| var-deploy-hotfix-no-canary | scope_dimension_count | 1 | None |
| var-deploy-hotfix-no-canary | diverges_without_stated_conditions | True | None |
| var-loto-night-self-verify | name | Night self-verification | None |
| var-loto-night-self-verify | has_no_applicability_dimension | True | None |
| var-loto-night-self-verify | diverges_without_stated_conditions | True | None |
| var-loto-press7-accumulator | name | Press 7 accumulator bleed-down | None |
| var-loto-press7-accumulator | scope_dimension_count | 3 | None |
| var-loto-press7-accumulator | scope_states_conditions | True | None |

### collected_source_materials

- Fields: 115/165 (69.7%)
- Computed columns: name, is_modeled_before_organized, source_document_revised_at, is_document_source, is_people_capture, is_practice_evidence, is_captured_in_flow_of_work, dependent_trace_count, is_dependency_invisible_to_change, changed_dependent_count, has_knowledge_affected_by_source_change

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| csm-convmaint-doc-excerpt | name | Excerpt: conveyor maintenance  | None |
| csm-convmaint-doc-excerpt | is_modeled_before_organized | True | None |
| csm-convmaint-doc-excerpt | is_document_source | True | None |
| csm-convmaint-doc-excerpt | is_dependency_invisible_to_change | True | None |
| csm-convmaint-doc-excerpt-2 | name | Excerpt: conveyor maintenance  | None |
| csm-convmaint-doc-excerpt-2 | is_document_source | True | None |
| csm-deploy-event-trace | name | Mined event trace: pipeline ru | None |
| csm-deploy-event-trace | is_practice_evidence | True | None |
| csm-deploy-runbook-excerpt | name | Excerpt: deployment runbook, a | None |
| csm-deploy-runbook-excerpt | source_document_revised_at | 2026-01-02T09:00:00-05:00 | None |
| csm-deploy-runbook-excerpt | is_document_source | True | None |
| csm-deploy-runbook-excerpt | dependent_trace_count | 4 | None |
| csm-deploy-runbook-excerpt | changed_dependent_count | 4 | None |
| csm-deploy-runbook-excerpt | has_knowledge_affected_by_source_change | True | None |
| csm-deploy-transcript-grace | name | Interview transcript: Grace Ho | None |
| csm-deploy-transcript-grace | is_people_capture | True | None |
| csm-deploy-transcript-grace | dependent_trace_count | 2 | None |
| csm-loto-fieldnotes-night | name | Field notes: south hall night  | None |
| csm-loto-fieldnotes-night | is_people_capture | True | None |
| csm-loto-fieldnotes-night | is_practice_evidence | True | None |
| ... | ... | (30 more) | ... |

### scheme_refinements

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sr-lockout-night-notes | name | voc-lockout-activities refined | None |
| sr-lockout-process-map | name | voc-lockout-activities refined | None |

### term_label_variants

- Fields: 190/583 (32.6%)
- Computed columns: name, term_scheme, term_pref_label, wording_key, pref_wording_key, concepts_sharing_wording, is_ambiguous_label, practitioner_mention_count, pref_wording, same_pref_wording_count, is_cross_scheme_duplicate_pref

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| lbl-artifact-drawing-pref | name | vt-artifact-drawing pref: engi | None |
| lbl-artifact-drawing-pref | term_scheme | voc-artifact-types | None |
| lbl-artifact-drawing-pref | term_pref_label | engineering drawing | None |
| lbl-artifact-drawing-pref | wording_key | voc-artifact-types|engineering | None |
| lbl-artifact-drawing-pref | pref_wording_key | voc-artifact-types|engineering | None |
| lbl-artifact-drawing-pref | concepts_sharing_wording | 1 | None |
| lbl-artifact-drawing-pref | pref_wording | engineering drawing | None |
| lbl-artifact-drawing-pref | same_pref_wording_count | 1 | None |
| lbl-artifact-register-entry-pref | name | vt-artifact-register-entry pre | None |
| lbl-artifact-register-entry-pref | term_scheme | voc-artifact-types | None |
| lbl-artifact-register-entry-pref | term_pref_label | register entry | None |
| lbl-artifact-register-entry-pref | wording_key | voc-artifact-types|register en | None |
| lbl-artifact-register-entry-pref | pref_wording_key | voc-artifact-types|register en | None |
| lbl-artifact-register-entry-pref | concepts_sharing_wording | 1 | None |
| lbl-artifact-register-entry-pref | pref_wording | register entry | None |
| lbl-artifact-register-entry-pref | same_pref_wording_count | 1 | None |
| lbl-artifact-runbook-pref | name | vt-artifact-runbook pref: runb | None |
| lbl-artifact-runbook-pref | term_scheme | voc-artifact-types | None |
| lbl-artifact-runbook-pref | term_pref_label | runbook | None |
| lbl-artifact-runbook-pref | wording_key | voc-artifact-types|runbook | None |
| ... | ... | (373 more) | ... |

### ai_labeling_runs

- Fields: 7/18 (38.9%)
- Computed columns: name, output_count, non_canonical_output_count, grounding_scheme_is_machine_accessible, is_ungrounded_synonym_sprawl, grounded_in_non_machine_readable_scheme

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| run-status-grounded | name | status-tagger-ai 2026-06-21T15 | None |
| run-status-grounded | output_count | 3 | None |
| run-status-grounded | grounding_scheme_is_machine_accessible | True | None |
| run-status-ungrounded | name | status-tagger-ai 2026-06-20T15 | None |
| run-status-ungrounded | output_count | 4 | None |
| run-status-ungrounded | non_canonical_output_count | 4 | None |
| run-status-ungrounded | is_ungrounded_synonym_sprawl | True | None |
| run-support-glossary | name | status-tagger-ai 2026-06-22T15 | None |
| run-support-glossary | output_count | 2 | None |
| run-support-glossary | non_canonical_output_count | 1 | None |
| run-support-glossary | grounded_in_non_machine_readable_scheme | True | None |

### source_term_mentions

- Fields: 129/225 (57.3%)
- Computed columns: name, wording_key, matching_label_count, matching_pref_label_count, is_uncontrolled_wording, is_non_canonical_generated_value, intended_term_role, unresolved_intended_term_key, unresolved_role_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| m-ai-01 | name | voc-workflow-status: live | None |
| m-ai-01 | wording_key | voc-workflow-status|live | None |
| m-ai-01 | matching_label_count | 1 | None |
| m-ai-01 | is_non_canonical_generated_value | True | None |
| m-ai-02 | name | voc-workflow-status: shipped | None |
| m-ai-02 | wording_key | voc-workflow-status|shipped | None |
| m-ai-02 | is_uncontrolled_wording | True | None |
| m-ai-02 | is_non_canonical_generated_value | True | None |
| m-ai-02 | unresolved_intended_term_key | vt-status-active | None |
| m-ai-03 | name | voc-workflow-status: work in p | None |
| m-ai-03 | wording_key | voc-workflow-status|work in pr | None |
| m-ai-03 | matching_label_count | 1 | None |
| m-ai-03 | is_non_canonical_generated_value | True | None |
| m-ai-04 | name | voc-workflow-status: retired | None |
| m-ai-04 | wording_key | voc-workflow-status|retired | None |
| m-ai-04 | matching_label_count | 1 | None |
| m-ai-04 | is_non_canonical_generated_value | True | None |
| m-ai-05 | name | voc-workflow-status: active | None |
| m-ai-05 | wording_key | voc-workflow-status|active | None |
| m-ai-05 | matching_label_count | 1 | None |
| ... | ... | (76 more) | ... |

### term_relations

- Fields: 3/12 (25.0%)
- Computed columns: name, from_term_grandparent, asserts_indirect_link_as_direct

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tr-bleed-broader-lockout | name | vt-hydraulic-bleed broader vt- | None |
| tr-bleed-broader-lockout | from_term_grandparent | vt-lockout | None |
| tr-bleed-broader-lockout | asserts_indirect_link_as_direct | True | None |
| tr-risk-related-approval | name | vt-risk-classification related | None |
| tr-risk-related-approval | from_term_grandparent | vt-change-control | None |
| tr-rollback-related-approval | name | vt-rollback related vt-release | None |
| tr-rollback-related-approval | from_term_grandparent | vt-change-control | None |
| tr-zero-related-isolation | name | vt-zero-energy-verification re | None |
| tr-zero-related-isolation | from_term_grandparent | vt-energy-control | None |

### term_meaning_changes

- Fields: 0/6 (0.0%)
- Computed columns: name, span_days

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tmc-release-2026 | name | vt-release meaning changed 202 | None |
| tmc-release-2026 | span_days | 718 | None |
| tmc-rollback-2026 | name | vt-rollback meaning changed 20 | None |
| tmc-rollback-2026 | span_days | 273 | None |
| tmc-zero-energy-2026 | name | vt-zero-energy-verification me | None |
| tmc-zero-energy-2026 | span_days | 2637 | None |

### external_standard_terms

- Fields: 88/153 (57.5%)
- Computed columns: name, as_of_instant, rehomed_term_same_as, rehomed_term_namespace, rehomed_term_release_issued_at, profile_namespace_iri, days_since_deprecated, is_recent_deprecation, using_mapping_count, stale_identifier_mapping_count, is_adopted_but_deprecated, is_alignment_stale_after_deprecation, is_needed_deprecated_term_not_rehomed, is_rehomed_without_identity_link, is_rehomed_without_new_release, has_unpropagated_identifier_change, rehoming_kept_external_namespace

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| est-dct-has-version | name | http://purl.org/dc/terms/hasVe | None |
| est-dct-has-version | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| est-dct-has-version | rehomed_term_same_as | http://purl.org/dc/terms/hasVe | None |
| est-dct-has-version | rehomed_term_namespace | https://effortlessapi.github.i | None |
| est-dct-has-version | profile_namespace_iri | http://www.w3.org/ns/dcat# | None |
| est-dct-has-version | days_since_deprecated | 696 | None |
| est-dct-has-version | is_recent_deprecation | True | None |
| est-dct-has-version | using_mapping_count | 2 | None |
| est-dct-has-version | is_adopted_but_deprecated | True | None |
| est-dct-has-version | is_rehomed_without_new_release | True | None |
| est-foaf-family-name | name | http://xmlns.com/foaf/0.1/fami | None |
| est-foaf-family-name | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| est-foaf-family-name | rehomed_term_same_as | http://xmlns.com/foaf/0.1/fami | None |
| est-foaf-family-name | rehomed_term_namespace | http://xmlns.com/foaf/0.1/ | None |
| est-foaf-family-name | rehomed_term_release_issued_at | 2026-05-04T12:00:00-05:00 | None |
| est-foaf-family-name | profile_namespace_iri | http://xmlns.com/foaf/0.1/ | None |
| est-foaf-family-name | days_since_deprecated | 4569 | None |
| est-foaf-family-name | is_adopted_but_deprecated | True | None |
| est-foaf-family-name | rehoming_kept_external_namespace | True | None |
| est-foaf-givenname | name | http://xmlns.com/foaf/0.1/give | None |
| ... | ... | (45 more) | ... |

### role_capability_tags

- Fields: 8/27 (29.6%)
- Computed columns: name, capability_scheme_dimension, is_tag_outside_capability_scheme

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rct-auditor-compliance | name | internal-compliance-auditor ca | None |
| rct-auditor-compliance | capability_scheme_dimension | AgentCapability | None |
| rct-controller-compliance | name | controller can vt-cap-complian | None |
| rct-controller-compliance | capability_scheme_dimension | AgentCapability | None |
| rct-pipeline-deployment | name | deployment-automation can vt-c | None |
| rct-pipeline-deployment | capability_scheme_dimension | AgentCapability | None |
| rct-release-compliance | name | release-manager can vt-cap-com | None |
| rct-release-compliance | capability_scheme_dimension | AgentCapability | None |
| rct-risk-scoring | name | change-risk-classifier can vt- | None |
| rct-risk-scoring | capability_scheme_dimension | AgentCapability | None |
| rct-safety-compliance | name | plant-safety-officer can vt-ca | None |
| rct-safety-compliance | capability_scheme_dimension | AgentCapability | None |
| rct-safety-validation | name | plant-safety-officer can vt-ca | None |
| rct-safety-validation | capability_scheme_dimension | AgentCapability | None |
| rct-sre-validation | name | site-reliability-engineer can  | None |
| rct-sre-validation | capability_scheme_dimension | AgentCapability | None |
| rct-tech-plant-quality | name | maintenance-technician can vt- | None |
| rct-tech-plant-quality | capability_scheme_dimension | QualityConcern | None |
| rct-tech-plant-quality | is_tag_outside_capability_scheme | True | None |

### classification_facets

- Fields: 0/8 (0.0%)
- Computed columns: name, assignment_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| facet-audience | name | Audience | None |
| facet-audience | assignment_count | 2 | None |
| facet-change-type | name | Change type | None |
| facet-change-type | assignment_count | 1 | None |
| facet-functional-domain | name | Functional domain | None |
| facet-functional-domain | assignment_count | 4 | None |
| facet-hazard | name | Hazard | None |
| facet-hazard | assignment_count | 5 | None |

### procedure_facet_assignments

- Fields: 0/12 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pfa-cir-audience | name | customer-incident-response fac | None |
| pfa-close-domain | name | quarter-end-close facet-functi | None |
| pfa-convmaint-hazard | name | conveyor-maintenance facet-haz | None |
| pfa-deploy-change | name | production-deployment facet-ch | None |
| pfa-deploy-domain | name | production-deployment facet-fu | None |
| pfa-forklift-domain | name | forklift-daily-inspection face | None |
| pfa-forklift-hazard | name | forklift-daily-inspection face | None |
| pfa-loto-domain | name | lockout-tagout facet-functiona | None |
| pfa-loto-hazard | name | lockout-tagout facet-hazard=Ha | None |
| pfa-loto-template-hazard | name | loto-template facet-hazard=Haz | None |
| pfa-policy-audience | name | workforce-policy-notification  | None |
| pfa-press-hazard | name | press-changeover facet-hazard= | None |

### encoding_lifecycle_stages

- Fields: 6/18 (33.3%)
- Computed columns: name, annotation_count, is_stage_without_feedback

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| codify | name | Codify | None |
| codify | is_stage_without_feedback | True | None |
| collect | name | Collect | None |
| collect | annotation_count | 2 | None |
| document | name | Document | None |
| document | annotation_count | 2 | None |
| encode | name | Encode | None |
| encode | annotation_count | 3 | None |
| operationalize | name | Operationalize | None |
| operationalize | annotation_count | 2 | None |
| store | name | Store | None |
| store | is_stage_without_feedback | True | None |

### knowledge_consumer_systems

- Fields: 48/80 (60.0%)
- Computed columns: name, model_sync_count, integration_count, is_knowledge_silo, is_unlinked_toolchain_component, semantic_layer_component_count, platform_capability_count, is_immature_graph_platform, holds_computationally_encoded_procedure_knowledge, stores_procedure_knowledge_without_computational_access

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sys-bpmn-modeler | name | Process modeling workbench | None |
| sys-bpmn-modeler | is_unlinked_toolchain_component | True | None |
| sys-bpmn-modeler | stores_procedure_knowledge_without_computational_access | True | None |
| sys-copilot-platform | name | Plant copilot AI platform | None |
| sys-copilot-platform | model_sync_count | 2 | None |
| sys-copilot-platform | platform_capability_count | 1 | None |
| sys-docvault | name | ACME DocVault (legacy content  | None |
| sys-docvault | integration_count | 1 | None |
| sys-docvault | is_knowledge_silo | True | None |
| sys-docvault | semantic_layer_component_count | 1 | None |
| sys-docvault | stores_procedure_knowledge_without_computational_access | True | None |
| sys-maintenance-tablet | name | Plant maintenance tablet app | None |
| sys-maintenance-tablet | model_sync_count | 1 | None |
| sys-maintenance-tablet | integration_count | 1 | None |
| sys-maintenance-tablet | stores_procedure_knowledge_without_computational_access | True | None |
| sys-procedure-graph | name | ACME procedural knowledge grap | None |
| sys-procedure-graph | model_sync_count | 2 | None |
| sys-procedure-graph | integration_count | 3 | None |
| sys-procedure-graph | semantic_layer_component_count | 5 | None |
| sys-procedure-graph | platform_capability_count | 3 | None |
| ... | ... | (12 more) | ... |

### consumer_system_syncs

- Fields: 24/72 (33.3%)
- Computed columns: name, system_audience, loaded_procedure, canonical_version, canonical_version_modified_at, loaded_version_creator, is_behind_canonical_version, predates_version_change, is_ai_fed_from_forked_copy, drops_provenance_in_transit, reaches_humans, reaches_machines

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sync-copilot-loto2 | name | sys-copilot-platform holds lot | None |
| sync-copilot-loto2 | system_audience | AI | None |
| sync-copilot-loto2 | loaded_procedure | lockout-tagout | None |
| sync-copilot-loto2 | canonical_version | loto-v2.0.0 | None |
| sync-copilot-loto2 | canonical_version_modified_at | 2026-07-10T09:00:00-05:00 | None |
| sync-copilot-loto2 | loaded_version_creator | sam-adeyemi | None |
| sync-copilot-loto2 | reaches_machines | True | None |
| sync-copilot-press | name | sys-copilot-platform holds pre | None |
| sync-copilot-press | system_audience | AI | None |
| sync-copilot-press | loaded_procedure | press-changeover | None |
| sync-copilot-press | canonical_version | press-changeover-v1.0.0 | None |
| sync-copilot-press | canonical_version_modified_at | 2026-06-15T09:00:00-05:00 | None |
| sync-copilot-press | loaded_version_creator | tomas-reyes | None |
| sync-copilot-press | predates_version_change | True | None |
| sync-copilot-press | reaches_machines | True | None |
| sync-graph-deploy | name | sys-procedure-graph holds depl | None |
| sync-graph-deploy | system_audience | Both | None |
| sync-graph-deploy | loaded_procedure | production-deployment | None |
| sync-graph-deploy | canonical_version | deploy-v3.2.0 | None |
| sync-graph-deploy | canonical_version_modified_at | 2026-06-30T09:00:00-05:00 | None |
| ... | ... | (28 more) | ... |

### integration_pathways

- Fields: 0/8 (0.0%)
- Computed columns: name, integration_count_on_pathway

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pathway-a2a-gateway | name | Agent-to-agent gateway to the  | None |
| pathway-a2a-gateway | integration_count_on_pathway | 3 | None |
| pathway-mcp-register | name | Model Context Protocol server  | None |
| pathway-mcp-register | integration_count_on_pathway | 2 | None |
| pathway-oneoff-export | name | One-off nightly export script | None |
| pathway-oneoff-export | integration_count_on_pathway | 1 | None |
| pathway-prompt-paste | name | Pasted into the system prompt | None |
| pathway-prompt-paste | integration_count_on_pathway | 1 | None |

### agent_integrations

- Fields: 21/49 (42.9%)
- Computed columns: name, pathway_integration_count, observed_answer_count, is_shadow_integration, is_one_off_connection, snapshot_is_governed, is_deployed_on_ungoverned_graph

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| int-checklistbot-paste | name | checklist-bot-2019 via pathway | None |
| int-checklistbot-paste | pathway_integration_count | 1 | None |
| int-checklistbot-paste | observed_answer_count | 1 | None |
| int-checklistbot-paste | is_one_off_connection | True | None |
| int-copilot-mcp | name | plant-copilot-1-2 via pathway- | None |
| int-copilot-mcp | pathway_integration_count | 2 | None |
| int-copilot-mcp | observed_answer_count | 7 | None |
| int-copilot-mcp | snapshot_is_governed | True | None |
| int-copilot-oneoff | name | plant-copilot-1-2 via pathway- | None |
| int-copilot-oneoff | pathway_integration_count | 1 | None |
| int-copilot-oneoff | is_one_off_connection | True | None |
| int-copilot-oneoff | snapshot_is_governed | True | None |
| int-releaseasst-a2a-shadow | name | release-assistant-0-9 via path | None |
| int-releaseasst-a2a-shadow | pathway_integration_count | 3 | None |
| int-releaseasst-a2a-shadow | observed_answer_count | 1 | None |
| int-releaseasst-a2a-shadow | is_shadow_integration | True | None |
| int-releaseasst-a2a-shadow | snapshot_is_governed | True | None |
| int-releaseasst-mcp | name | release-assistant-0-9 via path | None |
| int-releaseasst-mcp | pathway_integration_count | 2 | None |
| int-releaseasst-mcp | observed_answer_count | 3 | None |
| ... | ... | (8 more) | ... |

### grounding_snapshots

- Fields: 12/36 (33.3%)
- Computed columns: name, is_governed, reasoner_run_count, consistent_reasoner_run_count, latest_materialized_at, served_without_consistency_check, served_before_materialization, assertion_count, stale_assignment_assertion_count, serves_stale_role_assignments, deprecated_as_current_count, serves_deprecated_as_current

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| snap-2026-06-01 | name | Retrieval graph 2026-06-01 (ta | None |
| snap-2026-06-01 | reasoner_run_count | 1 | None |
| snap-2026-06-01 | served_without_consistency_check | True | None |
| snap-2026-06-01 | served_before_materialization | True | None |
| snap-2026-06-01 | assertion_count | 3 | None |
| snap-2026-06-01 | stale_assignment_assertion_count | 1 | None |
| snap-2026-06-01 | serves_stale_role_assignments | True | None |
| snap-2026-06-01 | deprecated_as_current_count | 1 | None |
| snap-2026-06-01 | serves_deprecated_as_current | True | None |
| snap-2026-07-15 | name | Retrieval graph 2026-07-15 | None |
| snap-2026-07-15 | is_governed | True | None |
| snap-2026-07-15 | reasoner_run_count | 3 | None |
| snap-2026-07-15 | consistent_reasoner_run_count | 2 | None |
| snap-2026-07-15 | latest_materialized_at | 2026-07-15T05:00:00-05:00 | None |
| snap-2026-07-15 | assertion_count | 10 | None |
| snap-2026-07-18-hotfix | name | Retrieval graph 2026-07-18 (ho | None |
| snap-2026-07-18-hotfix | is_governed | True | None |
| snap-2026-07-18-hotfix | reasoner_run_count | 1 | None |
| snap-2026-07-18-hotfix | consistent_reasoner_run_count | 1 | None |
| snap-2026-07-18-hotfix | latest_materialized_at | 2026-07-18T09:00:00-05:00 | None |
| ... | ... | (4 more) | ... |

### reasoner_runs

- Fields: 7/15 (46.7%)
- Computed columns: name, is_richness_tractability_failure, passes_schema_but_fails_reasoner

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rr-0601-rl | name | snap-2026-06-01 / OWL 2 RL | None |
| rr-0601-rl | passes_schema_but_fails_reasoner | True | None |
| rr-0715-dl | name | snap-2026-07-15 / OWL 2 DL | None |
| rr-0715-dl | is_richness_tractability_failure | True | None |
| rr-0715-rdfs | name | snap-2026-07-15 / RDFS | None |
| rr-0715-rdfs | is_richness_tractability_failure | True | None |
| rr-0715-rl | name | snap-2026-07-15 / OWL 2 RL | None |
| rr-0718-rl | name | snap-2026-07-18-hotfix / OWL 2 | None |

### snapshot_assertions

- Fields: 84/150 (56.0%)
- Computed columns: name, source_version_status, source_assignment_is_current, snapshot_consistent_run_count, snapshot_is_reasoned, is_stale_role_assertion, presents_deprecated_as_current, lacks_provenance, has_opaque_identifier, lacks_dublin_core

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sa-0601-approver-text | name | _:b17 rdfs:comment approvals b | None |
| sa-0601-approver-text | source_assignment_is_current | True | None |
| sa-0601-approver-text | lacks_provenance | True | None |
| sa-0601-approver-text | has_opaque_identifier | True | None |
| sa-0601-approver-text | lacks_dublin_core | True | None |
| sa-0601-loto1-current | name | loto-v1.0.0 adms:status Curren | None |
| sa-0601-loto1-current | source_version_status | Deprecated | None |
| sa-0601-loto1-current | source_assignment_is_current | True | None |
| sa-0601-loto1-current | presents_deprecated_as_current | True | None |
| sa-0601-risk240-holds | name | ra-risk-240 pro:isHeldBy risk- | None |
| sa-0601-risk240-holds | is_stale_role_assertion | True | None |
| sa-0715-deploy03-role | name | deploy-03 pko:hasRole release- | None |
| sa-0715-deploy03-role | source_version_status | Approved | None |
| sa-0715-deploy03-role | source_assignment_is_current | True | None |
| sa-0715-deploy03-role | snapshot_consistent_run_count | 2 | None |
| sa-0715-deploy03-role | snapshot_is_reasoned | True | None |
| sa-0715-grace-holds | name | ra-release-grace pro:isHeldBy  | None |
| sa-0715-grace-holds | source_assignment_is_current | True | None |
| sa-0715-grace-holds | snapshot_consistent_run_count | 2 | None |
| sa-0715-grace-holds | snapshot_is_reasoned | True | None |
| ... | ... | (46 more) | ... |

### retrieval_segments

- Fields: 37/63 (58.7%)
- Computed columns: name, author_agent_kind, related_from_count, is_isolated_chunk, contradicted_is_indexed, is_inconsistent_grounding, is_machine_held_knowledge

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| seg-deploy-03-gate | name | seg-deploy-03-gate: Releases a | None |
| seg-deploy-03-gate | author_agent_kind | Human | None |
| seg-deploy-04-rollout | name | seg-deploy-04-rollout: Roll th | None |
| seg-deploy-04-rollout | author_agent_kind | Human | None |
| seg-deploy-04-rollout | related_from_count | 1 | None |
| seg-loto1-02-locks | name | seg-loto1-02-locks: Apply lock | None |
| seg-loto1-02-locks | author_agent_kind | Human | None |
| seg-loto1-02-locks | contradicted_is_indexed | True | None |
| seg-loto1-02-locks | is_inconsistent_grounding | True | None |
| seg-loto2-04b-bleed | name | seg-loto2-04b-bleed: Close the | None |
| seg-loto2-04b-bleed | author_agent_kind | AIAgent | None |
| seg-loto2-04b-bleed | is_machine_held_knowledge | True | None |
| seg-loto2-05-locks | name | seg-loto2-05-locks: Isolate fi | None |
| seg-loto2-05-locks | author_agent_kind | Human | None |
| seg-loto2-06-verify | name | seg-loto2-06-verify: Read ever | None |
| seg-loto2-06-verify | author_agent_kind | Human | None |
| seg-loto2-06-verify | related_from_count | 1 | None |
| seg-loto2-09-escalate | name | seg-loto2-09-escalate: Keep lo | None |
| seg-loto2-09-escalate | author_agent_kind | Human | None |
| seg-loto2-09-escalate | related_from_count | 1 | None |
| ... | ... | (6 more) | ... |

### knowledge_query_definitions

- Fields: 6/16 (37.5%)
- Computed columns: name, source_system_count, misses_a_layer, consolidated_distributed_sources

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kq-agent-kind-flat | name | Is the agent accountable for d | None |
| kq-agent-kind-flat | source_system_count | 1 | None |
| kq-agent-kind-flat | misses_a_layer | True | None |
| kq-approver-sparql | name | Who holds the approving role f | None |
| kq-approver-sparql | source_system_count | 2 | None |
| kq-loto-energy-sql | name | Which energy sources does the  | None |
| kq-loto-energy-sql | source_system_count | 1 | None |
| kq-plantwide-lockouts | name | Which lockouts across both hal | None |
| kq-plantwide-lockouts | source_system_count | 2 | None |
| kq-plantwide-lockouts | consolidated_distributed_sources | True | None |

### knowledge_query_sources

- Fields: 0/6 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kqs-agentkind-register | name | kq-agent-kind-flat reads sys-p | None |
| kqs-approver-graph | name | kq-approver-sparql reads sys-p | None |
| kqs-approver-register | name | kq-approver-sparql reads sys-p | None |
| kqs-energy-register | name | kq-loto-energy-sql reads sys-p | None |
| kqs-plantwide-register | name | kq-plantwide-lockouts reads sy | None |
| kqs-plantwide-tablet | name | kq-plantwide-lockouts reads sy | None |

### assistant_answers

- Fields: 304/429 (70.9%)
- Computed columns: name, grounding_count, own_knowledge_grounding_count, cited_grounding_count, stale_grounding_count, is_not_from_own_knowledge, is_untraceable_to_source, context_unescalated_danger_cue_count, stayed_silent_on_safety_problem, context_step_ended_at, arrived_after_step_ended, execution_of_context, context_step, assumed_step_completed_count, lost_track_of_state, specified_transition_count, contradicts_shared_model, is_explicitly_grounded_recommendation, recommendation_rests_on_nothing_explicit, recommended_step_regulatory_count, requirement_check_count, conflict_count, conflicts_with_regulation, is_unchecked_regulated_recommendation, delivered_despite_conflict, recommended_step_needs_human, acted_on_without_human_judgment, answered_compliance_question_from_documents, document_interpretation_erred, model_did_the_reasoning, wrong_because_graph_was_stale, owner_organization, model_reasoned_and_task_failed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ans-0301-risk | name | risk-classifier-2-4-1: Risk cl | None |
| ans-0301-risk | grounding_count | 1 | None |
| ans-0301-risk | own_knowledge_grounding_count | 1 | None |
| ans-0301-risk | cited_grounding_count | 1 | None |
| ans-0301-risk | context_step_ended_at | 2026-03-01T07:12:00-05:00 | None |
| ans-0301-risk | execution_of_context | exec-deploy-2026-03-01 | None |
| ans-0301-risk | context_step | deploy-02 | None |
| ans-0301-risk | assumed_step_completed_count | 1 | None |
| ans-0301-risk | specified_transition_count | 1 | None |
| ans-0301-risk | is_explicitly_grounded_recommendation | True | None |
| ans-0301-risk | recommended_step_regulatory_count | 1 | None |
| ans-0301-risk | requirement_check_count | 1 | None |
| ans-0301-risk | recommended_step_needs_human | True | None |
| ans-0301-risk | owner_organization | acme-engineering | None |
| ans-0702-current-loto | name | plant-copilot-1-2: Which locko | None |
| ans-0702-current-loto | grounding_count | 1 | None |
| ans-0702-current-loto | own_knowledge_grounding_count | 1 | None |
| ans-0702-current-loto | cited_grounding_count | 1 | None |
| ans-0702-current-loto | stale_grounding_count | 1 | None |
| ans-0702-current-loto | wrong_because_graph_was_stale | True | None |
| ... | ... | (105 more) | ... |

### answer_groundings

- Fields: 39/70 (55.7%)
- Computed columns: name, is_from_own_knowledge, assertion_is_stale, assertion_presents_deprecated, grounds_on_stale_assertion

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ag-0301-risk-gate | name | ans-0301-risk <- seg-deploy-03 | None |
| ag-0301-risk-gate | is_from_own_knowledge | True | None |
| ag-0702-current-loto1 | name | ans-0702-current-loto <- sa-06 | None |
| ag-0702-current-loto1 | is_from_own_knowledge | True | None |
| ag-0702-current-loto1 | assertion_presents_deprecated | True | None |
| ag-0702-current-loto1 | grounds_on_stale_assertion | True | None |
| ag-0708-late-verify | name | ans-0708-late <- seg-loto2-06- | None |
| ag-0708-late-verify | is_from_own_knowledge | True | None |
| ag-0709-hiss-bleed | name | ans-0709-hiss <- seg-loto2-04b | None |
| ag-0709-hiss-bleed | is_from_own_knowledge | True | None |
| ag-0709-skip-verify | name | ans-0709-skipverify <- seg-lot | None |
| ag-0709-skip-verify | is_from_own_knowledge | True | None |
| ag-0710-stale-risk240 | name | ans-0710-stale <- sa-0601-risk | None |
| ag-0710-stale-risk240 | is_from_own_knowledge | True | None |
| ag-0710-stale-risk240 | assertion_is_stale | True | None |
| ag-0710-stale-risk240 | grounds_on_stale_assertion | True | None |
| ag-0712-doc-gate | name | ans-0712-doc <- seg-deploy-03- | None |
| ag-0712-doc-gate | is_from_own_knowledge | True | None |
| ag-0714-deploy-forum | name | ans-0714-deploy <- https://for | None |
| ag-0714-gauge-dp | name | ans-0714-gauge <- seg-loto2-dp | None |
| ... | ... | (11 more) | ... |

### answer_requirement_checks

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| arc-0301-approval | name | ans-0301-risk vs req-deploy-ap | None |
| arc-0714-approval | name | ans-0714-deploy vs req-deploy- | None |

### ai_tool_invocations

- Fields: 5/18 (27.8%)
- Computed columns: name, executed_step, declared_function_count, is_undeclared_tool_use, declared_input_count, acted_without_declared_context

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ati-0716-reserve-locks | name | plant-copilot-1-2 -> reserve-p | None |
| ati-0716-reserve-locks | executed_step | loto-05 | None |
| ati-0716-reserve-locks | declared_function_count | 1 | None |
| ati-0716-reserve-locks | declared_input_count | 2 | None |
| ati-0716-reserve-locks | acted_without_declared_context | True | None |
| ati-dep0301-classify | name | risk-classifier-2-4-1 -> class | None |
| ati-dep0301-classify | executed_step | deploy-02 | None |
| ati-dep0301-classify | declared_function_count | 1 | None |
| ati-dep0301-classify | declared_input_count | 1 | None |
| ati-dep0714-incidents | name | risk-classifier-2-4-1 -> fetch | None |
| ati-dep0714-incidents | executed_step | deploy-02 | None |
| ati-dep0714-incidents | is_undeclared_tool_use | True | None |
| ati-dep0714-incidents | declared_input_count | 1 | None |

### embedding_probes

- Fields: 10/18 (55.6%)
- Computed columns: name, synonyms_not_similar, opposites_not_opposed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| emb-general-approve-authorize | name | approve / authorize @ general- | None |
| emb-general-approve-reject | name | approve / reject @ general-emb | None |
| emb-general-approve-reject | opposites_not_opposed | True | None |
| emb-general-lockout-isolate | name | lock out / isolate @ general-e | None |
| emb-general-lockout-isolate | synonyms_not_similar | True | None |
| emb-tuned-approve-reject | name | approve / reject @ procedure-e | None |
| emb-tuned-escalate-standdown | name | escalate / stand down @ proced | None |
| emb-tuned-lockout-isolate | name | lock out / isolate @ procedure | None |

### prompt_templates

- Fields: 22/35 (62.9%)
- Computed columns: name, child_template_count, is_disconnected_prompt_knowledge, is_unmanaged_prompt_in_use, spends_budget_on_rare_detail, condensation_percent, is_verbatim_dump

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pt-loto2-detail | name | Lockout/Tagout 2.0.0 condensed | None |
| pt-loto2-detail | condensation_percent | 13.4 | None |
| pt-plant-framework | name | Plant assistant framework guid | None |
| pt-plant-framework | child_template_count | 2 | None |
| pt-press7-fullmap | name | Press-7 isolation detail with  | None |
| pt-press7-fullmap | spends_budget_on_rare_detail | True | None |
| pt-press7-fullmap | condensation_percent | 34.7 | None |
| pt-release-hotfix-note | name | Hotfix rollout note typed into | None |
| pt-release-hotfix-note | is_disconnected_prompt_knowledge | True | None |
| pt-sop2019-dump | name | 2019 SOP pasted into the check | None |
| pt-sop2019-dump | is_unmanaged_prompt_in_use | True | None |
| pt-sop2019-dump | condensation_percent | 95.1 | None |
| pt-sop2019-dump | is_verbatim_dump | True | None |

### knowledge_projections

- Fields: 23/49 (46.9%)
- Computed columns: name, version_modified_at, open_count, diagram_can_diverge_from_model, narrative_not_generated_from_model, narrative_unreachable, is_published

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| proj-deploy-mermaid | name | Production deployment flow | None |
| proj-deploy-mermaid | version_modified_at | 2026-06-30T09:00:00-05:00 | None |
| proj-deploy-mermaid | diagram_can_diverge_from_model | True | None |
| proj-deploy-mermaid | is_published | True | None |
| proj-deploy-runbook-prose | name | Hand-maintained deployment run | None |
| proj-deploy-runbook-prose | version_modified_at | 2026-06-30T09:00:00-05:00 | None |
| proj-deploy-runbook-prose | open_count | 1 | None |
| proj-deploy-runbook-prose | narrative_not_generated_from_model | True | None |
| proj-deploy-runbook-prose | is_published | True | None |
| proj-loto2-bpmn | name | Lockout/Tagout 2.0.0 BPMN diag | None |
| proj-loto2-bpmn | version_modified_at | 2026-07-10T09:00:00-05:00 | None |
| proj-loto2-bpmn | open_count | 1 | None |
| proj-loto2-bpmn | is_published | True | None |
| proj-loto2019-visio | name | 2019 lockout flowchart (hand-d | None |
| proj-loto2019-visio | version_modified_at | 2025-11-30T09:00:00-05:00 | None |
| proj-loto2019-visio | diagram_can_diverge_from_model | True | None |
| proj-loto2019-visio | is_published | True | None |
| proj-nl-docs | name | Natural-language lockout docum | None |
| proj-nl-docs | version_modified_at | 2026-07-10T09:00:00-05:00 | None |
| proj-nl-docs | is_published | True | None |
| ... | ... | (6 more) | ... |

### model_annotations

- Fields: 39/54 (72.2%)
- Computed columns: name, is_discussion_in_authoritative_model, flag_bypassed_annotation_interface, is_lost_new_knowledge, is_friction_without_gap, is_open_outdated_flag

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ann-0710-bleed-email | name | FlagOutdated: The bleed instru | None |
| ann-0710-bleed-email | flag_bypassed_annotation_interface | True | None |
| ann-0710-bleed-email | is_open_outdated_flag | True | None |
| ann-0710-press7-map | name | FlagOutdated: The press-7 isol | None |
| ann-0710-press7-map | is_open_outdated_flag | True | None |
| ann-0712-hotfix-comment | name | Comment: Should hotfixes get a | None |
| ann-0712-hotfix-comment | is_discussion_in_authoritative_model | True | None |
| ann-0713-belt-question | name | Question: Does 'perform the ma | None |
| ann-0715-tomas-tap | name | NewKnowledge: Tap the gauge gl | None |
| ann-0715-tomas-tap | is_lost_new_knowledge | True | None |
| ann-0716-risk-factors | name | Suggestion: Show the classifie | None |
| ann-0718-mixer2-missing | name | MissingKnowledge: No isolation | None |
| ann-0718-mixer2-missing | is_friction_without_gap | True | None |
| ann-close-fx-lag | name | NewKnowledge: FX feed timestam | None |
| ann-policy-receipts | name | MissingKnowledge: Nothing says | None |

### knowledge_search_events

- Fields: 14/24 (58.3%)
- Computed columns: name, found_nothing_useful, gave_up_after_seeing_results, is_unlinked_failed_search

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kse-aisha-gauge | name | aisha-bello: gauge above zero | None |
| kse-copilot-residual | name | plant-copilot-1-2: loto-06 res | None |
| kse-grace-hotfix | name | grace-holloway: hotfix approva | None |
| kse-ken-press7 | name | ken-watanabe: press 7 accumula | None |
| kse-ken-press7 | found_nothing_useful | True | None |
| kse-ken-press7 | is_unlinked_failed_search | True | None |
| kse-lin-flow | name | lin-zhao: lockout flow diagram | None |
| kse-tomas-mixer2 | name | tomas-reyes: mixer 2 twin driv | None |
| kse-tomas-mixer2 | found_nothing_useful | True | None |
| kse-tomas-mixer2 | gave_up_after_seeing_results | True | None |

### ai_adoption_initiatives

- Fields: 143/225 (63.6%)
- Computed columns: name, as_of_instant, target_has_no_explicit_steps, target_version_issued_at, target_version_under_specified, target_version_notation_only, target_elicitation_evidence_count, reviewed_output_count, preceding_reviewed_output_count, last_outcome_measured_at, days_since_outcome_measured, ai_insight_count, hands_tacit_procedure_to_agent, agent_on_under_specified_procedure, agent_on_notation_only_procedure, calls_for_process_knowledge_framework, redesigned_before_documented, breaks_elicit_encode_connect_order, went_full_without_reviewed_partial_stage, underinvests_knowledge_layer, not_anchored_in_process_knowledge, agentic_without_knowledge_capture, is_ai_outcome_unmeasured, adopted_without_bottom_line_result, failed_without_formalized_knowledge

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ai-init-checklist-bot | name | 2019 checklist bot | None |
| ai-init-checklist-bot | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ai-init-checklist-bot | target_version_issued_at | 2019-03-01T09:00:00-05:00 | None |
| ai-init-checklist-bot | target_version_under_specified | True | None |
| ai-init-checklist-bot | last_outcome_measured_at | 2026-03-01T09:00:00-06:00 | None |
| ai-init-checklist-bot | days_since_outcome_measured | 140 | None |
| ai-init-checklist-bot | agent_on_under_specified_procedure | True | None |
| ai-init-checklist-bot | calls_for_process_knowledge_framework | True | None |
| ai-init-checklist-bot | breaks_elicit_encode_connect_order | True | None |
| ai-init-checklist-bot | underinvests_knowledge_layer | True | None |
| ai-init-checklist-bot | agentic_without_knowledge_capture | True | None |
| ai-init-checklist-bot | is_ai_outcome_unmeasured | True | None |
| ai-init-checklist-bot | adopted_without_bottom_line_result | True | None |
| ai-init-convmaint-agent | name | Conveyor maintenance schedulin | None |
| ai-init-convmaint-agent | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ai-init-convmaint-agent | target_version_issued_at | 2026-02-01T09:00:00-05:00 | None |
| ai-init-convmaint-agent | target_version_under_specified | True | None |
| ai-init-convmaint-agent | target_elicitation_evidence_count | 1 | None |
| ai-init-convmaint-agent | preceding_reviewed_output_count | 2 | None |
| ai-init-convmaint-agent | ai_insight_count | 1 | None |
| ... | ... | (62 more) | ... |

### knowledge_outcome_measurements

- Fields: 48/70 (68.6%)
- Computed columns: name, baseline_access_score, baseline_error_rate, baseline_minutes_per_run, baseline_satisfaction, higher_access_fewer_errors, higher_access_more_efficient, higher_access_more_satisfied, is_unacted_adverse_outcome, is_gain_outside_procedural_scope

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kom-checklist-bot-0301 | name | Checklist bot lockout outcomes | None |
| kom-checklist-bot-0301 | is_unacted_adverse_outcome | True | None |
| kom-north-day-q2 | name | North hall day-shift lockouts, | None |
| kom-north-day-q2 | baseline_access_score | 35.0 | None |
| kom-north-day-q2 | baseline_error_rate | 9.0 | None |
| kom-north-day-q2 | baseline_minutes_per_run | 81.0 | None |
| kom-north-day-q2 | baseline_satisfaction | 3.1 | None |
| kom-north-day-q2 | higher_access_fewer_errors | True | None |
| kom-north-day-q2 | higher_access_more_efficient | True | None |
| kom-north-day-q2 | higher_access_more_satisfied | True | None |
| kom-north-night-q2 | name | North hall night-shift lockout | None |
| kom-north-night-q2 | baseline_access_score | 35.0 | None |
| kom-north-night-q2 | baseline_error_rate | 9.0 | None |
| kom-north-night-q2 | baseline_minutes_per_run | 81.0 | None |
| kom-north-night-q2 | baseline_satisfaction | 3.1 | None |
| kom-north-night-q2 | higher_access_fewer_errors | True | None |
| kom-plant-copilot-0715 | name | Plant copilot lockout outcomes | None |
| kom-quality-graph-0630 | name | Defect analysis with the quali | None |
| kom-quality-graph-0630 | is_gain_outside_procedural_scope | True | None |
| kom-risk-classifier-0710 | name | Risk classifier release outcom | None |
| ... | ... | (2 more) | ... |

### ai_insight_proposals

- Fields: 10/24 (41.7%)
- Computed columns: name, target_creator_kind, is_unvalidated_or_stranded_insight, grew_model_without_human_seed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| aip-convmaint-step | name | plant-copilot-1-2: Add a belt- | None |
| aip-convmaint-step | target_creator_kind | Human | None |
| aip-copilot-gauge-pattern | name | plant-copilot-1-2: Cold-mornin | None |
| aip-copilot-gauge-pattern | target_creator_kind | Human | None |
| aip-loto2-accumulator | name | segment-summarizer-ai: Add hyd | None |
| aip-loto2-accumulator | target_creator_kind | Human | None |
| aip-new-procedure | name | segment-summarizer-ai: Propose | None |
| aip-new-procedure | grew_model_without_human_seed | True | None |
| aip-risk-hotfix-pattern | name | risk-classifier-2-4-1: Hotfixe | None |
| aip-risk-hotfix-pattern | target_creator_kind | Human | None |
| aip-risk-hotfix-pattern | is_unvalidated_or_stranded_insight | True | None |
| aip-variance-folded | name | variance-ai: Timestamp mismatc | None |
| aip-variance-folded | target_creator_kind | Human | None |
| aip-variance-folded | is_unvalidated_or_stranded_insight | True | None |

### assistant_benchmarks

- Fields: 6/16 (37.5%)
- Computed columns: name, accuracy_lift_points, shows_spatial_lift_from_graph_queries, shows_geospatial_retrieval_lift

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bench-loto-sequence | name | Lockout step order and fallbac | None |
| bench-loto-sequence | accuracy_lift_points | 27.0 | None |
| bench-plant-layout | name | Which isolation points are wit | None |
| bench-plant-layout | accuracy_lift_points | 47.0 | None |
| bench-plant-layout | shows_spatial_lift_from_graph_queries | True | None |
| bench-plant-layout-docs | name | Press-7 platform reach, answer | None |
| bench-plant-layout-docs | accuracy_lift_points | 7.0 | None |
| bench-site-stations | name | Nearest lockout station and mu | None |
| bench-site-stations | accuracy_lift_points | 43.0 | None |
| bench-site-stations | shows_geospatial_retrieval_lift | True | None |

### governed_models

- Fields: 181/350 (51.7%)
- Computed columns: name, as_of_instant, days_since_registered, charter_count, current_charter_count, current_steward_role, current_steward_agent, current_authority_role, current_authority_agent, is_ownerless, is_ownerless_past_a_year, governance_lapsed, has_no_current_steward, has_no_current_authority, is_procedure_without_change_authority, last_steward_activity_at, days_since_steward_activity, is_unmaintained, documents_behind_count, is_not_kept_current, open_practice_drift_count, has_open_practice_drift, is_neglected_and_drifting, expert_found_drift_count, cq_review_count, last_cq_review_at, days_since_cq_review, cq_review_overdue, degradation_hidden_until_wrong_answer, baseline_question_count, requirements_spec_incomplete, collection_control_count, stewardship_control_count, retrieval_control_count, use_control_count, continuous_pipeline_stage_count, lacks_lifecycle_stage_control, pipeline_not_continuous, procedure_has_no_explicit_steps, is_unmanageable_undocumented_work, is_without_originating_use_case, pilot_count, is_adopted_without_pilot, requirements_expert_count, implementation_expert_count, publication_expert_count, maintenance_expert_count, experts_not_involved_throughout, real_data_mapping_run_count, is_implemented_without_real_data

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gm-close-controls-vocabulary | name | Close control categories vocab | None |
| gm-close-controls-vocabulary | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| gm-close-controls-vocabulary | days_since_registered | 48 | None |
| gm-close-controls-vocabulary | charter_count | 1 | None |
| gm-close-controls-vocabulary | is_ownerless | True | None |
| gm-close-controls-vocabulary | governance_lapsed | True | None |
| gm-close-controls-vocabulary | has_no_current_steward | True | None |
| gm-close-controls-vocabulary | has_no_current_authority | True | None |
| gm-close-controls-vocabulary | days_since_steward_activity | 48 | None |
| gm-close-controls-vocabulary | days_since_cq_review | 48 | None |
| gm-close-controls-vocabulary | requirements_spec_incomplete | True | None |
| gm-close-controls-vocabulary | procedure_has_no_explicit_steps | True | None |
| gm-lockout-tagout | name | Lockout/tagout procedure famil | None |
| gm-lockout-tagout | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| gm-lockout-tagout | days_since_registered | 68 | None |
| gm-lockout-tagout | charter_count | 2 | None |
| gm-lockout-tagout | current_charter_count | 1 | None |
| gm-lockout-tagout | current_steward_role | knowledge-engineer | None |
| gm-lockout-tagout | current_steward_agent | sam-adeyemi | None |
| gm-lockout-tagout | current_authority_role | plant-safety-officer | None |
| ... | ... | (149 more) | ... |

### model_charters

- Fields: 84/208 (40.4%)
- Computed columns: name, as_of_instant, is_current, steward_agent, authority_agent, authority_organization, model_kind, model_domain_owner, model_headcount, model_tooling_owner_role, model_first_control_adopted_at, is_steward_unwritten, is_authority_scope_unstated, conflates_steward_and_authority, is_sanctioned_dual_holding, is_authority_outside_domain_owner, controls_precede_ownership, steward_activity_count, last_drift_watch_at, days_since_drift_watch, is_drift_watch_lapsed, procedure_decision_count, model_review_count, is_named_but_unexercised, steward_is_outside_tooling, adopted_template_without_adaptation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| charter-close-2026-04 | name | gm-quarter-end-close / process | None |
| charter-close-2026-04 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| charter-close-2026-04 | is_current | True | None |
| charter-close-2026-04 | steward_agent | elena-garcia | None |
| charter-close-2026-04 | authority_agent | priya-raman | None |
| charter-close-2026-04 | authority_organization | acme-finance | None |
| charter-close-2026-04 | model_kind | ProcedureFamily | None |
| charter-close-2026-04 | model_domain_owner | acme-finance | None |
| charter-close-2026-04 | model_headcount | 900 | None |
| charter-close-2026-04 | model_tooling_owner_role | knowledge-engineer | None |
| charter-close-2026-04 | model_first_control_adopted_at | 2026-04-15T09:00:00-05:00 | None |
| charter-close-2026-04 | steward_activity_count | 1 | None |
| charter-close-2026-04 | last_drift_watch_at | 2026-04-16T09:00:00-05:00 | None |
| charter-close-2026-04 | days_since_drift_watch | 94 | None |
| charter-close-2026-04 | is_drift_watch_lapsed | True | None |
| charter-close-2026-04 | procedure_decision_count | 1 | None |
| charter-deploy-2026-02 | name | gm-production-deployment / rel | None |
| charter-deploy-2026-02 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| charter-deploy-2026-02 | is_current | True | None |
| charter-deploy-2026-02 | steward_agent | grace-holloway | None |
| ... | ... | (104 more) | ... |

### steward_activities

- Fields: 0/20 (0.0%)
- Computed columns: name, activity_model

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sa-close-drift-0416 | name | charter-close-2026-04 DriftWat | None |
| sa-close-drift-0416 | activity_model | gm-quarter-end-close | None |
| sa-loto07-drift-0710 | name | charter-loto-2026-07 DriftWatc | None |
| sa-loto07-drift-0710 | activity_model | gm-lockout-tagout | None |
| sa-pko01-drift-0301 | name | charter-pko-2026-01 DriftWatch | None |
| sa-pko01-drift-0301 | activity_model | gm-pko-rulebook | None |
| sa-pko01-suite-0419 | name | charter-pko-2026-01 Validation | None |
| sa-pko01-suite-0419 | activity_model | gm-pko-rulebook | None |
| sa-pko05-deps-0701 | name | charter-pko-2026-05 Dependency | None |
| sa-pko05-deps-0701 | activity_model | gm-pko-rulebook | None |
| sa-pko05-docs-0620 | name | charter-pko-2026-05 Documentat | None |
| sa-pko05-docs-0620 | activity_model | gm-pko-rulebook | None |
| sa-pko05-drift-0615 | name | charter-pko-2026-05 DriftWatch | None |
| sa-pko05-drift-0615 | activity_model | gm-pko-rulebook | None |
| sa-pko05-question-0716 | name | charter-pko-2026-05 UserQuesti | None |
| sa-pko05-question-0716 | activity_model | gm-pko-rulebook | None |
| sa-pko05-suite-0718 | name | charter-pko-2026-05 Validation | None |
| sa-pko05-suite-0718 | activity_model | gm-pko-rulebook | None |
| sa-policy-drift-0710 | name | charter-policy-2026-06 DriftWa | None |
| sa-policy-drift-0710 | activity_model | gm-workforce-policy | None |

### model_change_requests

- Fields: 804/1054 (76.3%)
- Computed columns: name, steward_agent, authority_agent, rule_key, required_route, is_accepted, is_modeling_change, is_schema_change, has_authority_review, requires_authority_review, is_minor_scope, steward_own_change_unreviewed, steward_approval_out_of_bounds, authority_review_skipped, placement_not_decided_by_authority, is_misrouted, lacks_motivating_question, unmotivated_and_not_returned, motivated_by_failing_question, is_accepted_without_named_approver, deployed_without_target_release, is_misclassified_agent_swap, ai_to_human_move_without_compliance_review, ai_to_human_move_unaudited, is_ai_to_human_move, lifecycle_change_by_unauthorized_agent, assessed_inconsistent_count, post_deploy_inconsistent_count, assessed_inference_count, post_deploy_inference_count, assessed_query_result_count, post_deploy_query_result_count, assessed_coverage_gap_count, would_make_instances_inconsistent, would_alter_inferences, would_alter_query_results, would_leave_coverage_incomplete, missed_inconsistent_instances, missed_altered_inferences, missed_altered_query_results, leaves_coverage_unchecked, domain_change_spread_wrong_inferences, intuitive_disjointness_broke_individuals, validation_run_count, acceptance_failure_total, structural_pass_count, vocabulary_pass_count, accepted_without_test_run, accepted_with_failing_suite, accepted_without_structural_check, accepted_without_vocabulary_check, integrity_check_count, human_integrity_check_count, disjointness_check_count, domain_inference_check_count, range_consistency_check_count, integrity_decided_without_human, skipped_disjointness_review, skipped_domain_inference_review, skipped_range_review, unresolved_objection_count, approved_over_unresolved_objection

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| mcr-01 | name | Add the lockout/tagout class m | None |
| mcr-01 | steward_agent | sam-adeyemi | None |
| mcr-01 | authority_agent | nadia-petrova | None |
| mcr-01 | rule_key | gm-pko-rulebook|Schema | None |
| mcr-01 | required_route | Governance | None |
| mcr-01 | is_accepted | True | None |
| mcr-01 | is_modeling_change | True | None |
| mcr-01 | is_schema_change | True | None |
| mcr-01 | has_authority_review | True | None |
| mcr-01 | is_minor_scope | True | None |
| mcr-01 | assessed_inference_count | 1 | None |
| mcr-01 | would_alter_inferences | True | None |
| mcr-01 | validation_run_count | 1 | None |
| mcr-01 | structural_pass_count | 1 | None |
| mcr-01 | vocabulary_pass_count | 1 | None |
| mcr-01 | integrity_check_count | 3 | None |
| mcr-01 | human_integrity_check_count | 3 | None |
| mcr-01 | disjointness_check_count | 1 | None |
| mcr-01 | domain_inference_check_count | 1 | None |
| mcr-01 | range_consistency_check_count | 1 | None |
| ... | ... | (230 more) | ... |

### change_authority_rules

- Fields: 6/12 (50.0%)
- Computed columns: name, misrouted_request_count, is_rule_bypassed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gm-pko-rulebook|Documentation | name | gm-pko-rulebook Documentation | None |
| gm-pko-rulebook|Instance | name | gm-pko-rulebook Instance | None |
| gm-pko-rulebook|Schema | name | gm-pko-rulebook Schema | None |
| gm-pko-rulebook|Schema | misrouted_request_count | 1 | None |
| gm-pko-rulebook|Schema | is_rule_bypassed | True | None |
| gm-pko-rulebook|Vocabulary | name | gm-pko-rulebook Vocabulary | None |

### change_impact_findings

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cif-01-inference | name | mcr-01 AlteredInference Assess | None |
| cif-02-inference | name | mcr-02 AlteredInference PostDe | None |
| cif-02-query | name | mcr-02 AlteredQueryResult Post | None |
| cif-03-inference | name | mcr-03 AlteredInference PostDe | None |
| cif-03-instance | name | mcr-03 InconsistentInstance As | None |
| cif-04-instance | name | mcr-04 InconsistentInstance Po | None |
| cif-07-coverage | name | mcr-07 CoverageGap Assessment | None |
| cif-09-query | name | mcr-09 AlteredQueryResult Asse | None |
| cif-15-instance | name | mcr-15 InconsistentInstance Po | None |
| cif-18-query | name | mcr-18 AlteredQueryResult Asse | None |

### change_integrity_checks

- Fields: 0/30 (0.0%)
- Computed columns: name, checker_kind

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cic-01-disjointness | name | mcr-01 Disjointness | None |
| cic-01-disjointness | checker_kind | Human | None |
| cic-01-domaininference | name | mcr-01 DomainInference | None |
| cic-01-domaininference | checker_kind | Human | None |
| cic-01-rangeconsistency | name | mcr-01 RangeConsistency | None |
| cic-01-rangeconsistency | checker_kind | Human | None |
| cic-03-disjointness | name | mcr-03 Disjointness | None |
| cic-03-disjointness | checker_kind | Human | None |
| cic-03-domaininference | name | mcr-03 DomainInference | None |
| cic-03-domaininference | checker_kind | AIAgent | None |
| cic-03-rangeconsistency | name | mcr-03 RangeConsistency | None |
| cic-03-rangeconsistency | checker_kind | Human | None |
| cic-04-disjointness | name | mcr-04 Disjointness | None |
| cic-04-disjointness | checker_kind | AIAgent | None |
| cic-05-disjointness | name | mcr-05 Disjointness | None |
| cic-05-disjointness | checker_kind | Human | None |
| cic-05-domaininference | name | mcr-05 DomainInference | None |
| cic-05-domaininference | checker_kind | Human | None |
| cic-15-disjointness | name | mcr-15 Disjointness | None |
| cic-15-disjointness | checker_kind | Human | None |
| ... | ... | (10 more) | ... |

### change_objections

- Fields: 1/4 (25.0%)
- Computed columns: name, is_unresolved

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| obj-03-lin | name | mcr-03 objection by lin-zhao | None |
| obj-04-lin | name | mcr-04 objection by lin-zhao | None |
| obj-04-lin | is_unresolved | True | None |

### change_validation_runs

- Fields: 33/56 (58.9%)
- Computed columns: name, release, expected_chain_count, unproduced_chain_count, has_uninspected_failures, is_unconfirmed_consistency, misses_expected_inference

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| vr-01 | name | mcr-01 Acceptance run | None |
| vr-01 | release | pko-release-0.9.0 | None |
| vr-01 | expected_chain_count | 2 | None |
| vr-03a | name | mcr-03 Exploratory run | None |
| vr-03a | release | pko-release-0.10.0 | None |
| vr-03a | is_unconfirmed_consistency | True | None |
| vr-03b | name | mcr-03 Acceptance run | None |
| vr-03b | release | pko-release-0.10.0 | None |
| vr-03b | has_uninspected_failures | True | None |
| vr-03b | is_unconfirmed_consistency | True | None |
| vr-04 | name | mcr-04 Acceptance run | None |
| vr-04 | release | pko-release-0.10.1 | None |
| vr-04 | is_unconfirmed_consistency | True | None |
| vr-05 | name | mcr-05 Acceptance run | None |
| vr-05 | release | pko-release-0.10.1 | None |
| vr-05 | expected_chain_count | 2 | None |
| vr-05 | unproduced_chain_count | 1 | None |
| vr-05 | misses_expected_inference | True | None |
| vr-14 | name | mcr-14 Acceptance run | None |
| vr-15 | name | mcr-15 Acceptance run | None |
| ... | ... | (3 more) | ... |

### expected_inference_checks

- Fields: 3/8 (37.5%)
- Computed columns: name, is_unproduced

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| eic-01-children | name | vr-01: loto-04 is a multi-step | None |
| eic-01-software | name | vr-01: Classifier 2.4.1 holds  | None |
| eic-05-doclag | name | vr-05: The deployment runbook  | None |
| eic-05-doclag | is_unproduced | True | None |
| eic-05-duration | name | vr-05: loto-v2.0.0 declares a  | None |

### model_consumers

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cons-deploy-risk-context | name | Risk classifier grounding cont | None |
| cons-owl | name | OWL/SHACL substrate | None |
| cons-postgres | name | Postgres substrate | None |
| cons-procedure-register | name | Procedure Register app | None |

### consumer_revalidations

- Fields: 0/26 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| crv-0.10.0-cons-deploy-risk-context | name | pko-release-0.10.0 / cons-depl | None |
| crv-0.10.0-cons-owl | name | pko-release-0.10.0 / cons-owl | None |
| crv-0.10.0-cons-postgres | name | pko-release-0.10.0 / cons-post | None |
| crv-0.10.0-cons-procedure-register | name | pko-release-0.10.0 / cons-proc | None |
| crv-0.10.1-cons-deploy-risk-context | name | pko-release-0.10.1 / cons-depl | None |
| crv-0.10.1-cons-owl | name | pko-release-0.10.1 / cons-owl | None |
| crv-0.10.1-cons-postgres | name | pko-release-0.10.1 / cons-post | None |
| crv-0.10.1-cons-procedure-register | name | pko-release-0.10.1 / cons-proc | None |
| crv-0.8.0-cons-deploy-risk-context | name | pko-release-0.8.0 / cons-deplo | None |
| crv-0.8.0-cons-owl | name | pko-release-0.8.0 / cons-owl | None |
| crv-0.8.0-cons-postgres | name | pko-release-0.8.0 / cons-postg | None |
| crv-0.8.0-cons-procedure-register | name | pko-release-0.8.0 / cons-proce | None |
| crv-0.8.1-cons-deploy-risk-context | name | pko-release-0.8.1 / cons-deplo | None |
| crv-0.8.1-cons-owl | name | pko-release-0.8.1 / cons-owl | None |
| crv-0.8.1-cons-postgres | name | pko-release-0.8.1 / cons-postg | None |
| crv-0.8.1-cons-procedure-register | name | pko-release-0.8.1 / cons-proce | None |
| crv-0.8.2-cons-owl | name | pko-release-0.8.2 / cons-owl | None |
| crv-0.8.2-cons-postgres | name | pko-release-0.8.2 / cons-postg | None |
| crv-0.9.0-cons-deploy-risk-context | name | pko-release-0.9.0 / cons-deplo | None |
| crv-0.9.0-cons-owl | name | pko-release-0.9.0 / cons-owl | None |
| ... | ... | (6 more) | ... |

### model_documents

- Fields: 3/15 (20.0%)
- Computed columns: name, model_current_release, is_behind_current_release

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| doc-article-coverage | name | ARTICLE-COVERAGE.md | None |
| doc-article-coverage | model_current_release | pko-release-1.0.0 | None |
| doc-changelog | name | CHANGELOG.md | None |
| doc-changelog | model_current_release | pko-release-1.0.0 | None |
| doc-changelog | is_behind_current_release | True | None |
| doc-pko-alignment | name | PKO-ALIGNMENT.md | None |
| doc-pko-alignment | model_current_release | pko-release-1.0.0 | None |
| doc-pko-alignment | is_behind_current_release | True | None |
| doc-readme | name | README.md | None |
| doc-readme | model_current_release | pko-release-1.0.0 | None |
| doc-witness-loops | name | WITNESS-LOOPS.md | None |
| doc-witness-loops | model_current_release | pko-release-1.0.0 | None |

### staleness_query_runs

- Fields: 2/9 (22.2%)
- Computed columns: name, runner_kind, not_surfaced_to_steward

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sqr-loto-0715 | name | gm-lockout-tagout staleness ru | None |
| sqr-loto-0715 | runner_kind | AIAgent | None |
| sqr-loto-0715 | not_surfaced_to_steward | True | None |
| sqr-pko-0601 | name | gm-pko-rulebook staleness run | None |
| sqr-pko-0601 | runner_kind | AIAgent | None |
| sqr-pko-0701 | name | gm-pko-rulebook staleness run | None |
| sqr-pko-0701 | runner_kind | AIAgent | None |

### external_dependency_revisions

- Fields: 2/15 (13.3%)
- Computed columns: name, affected_mapping_count, as_of_instant, days_since_published, is_untracked_revision

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rev-dcat-3-keyword-guidance | name | dcat-3 DCAT 3 keyword guidance | None |
| rev-dcat-3-keyword-guidance | affected_mapping_count | 3 | None |
| rev-dcat-3-keyword-guidance | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| rev-dcat-3-keyword-guidance | days_since_published | 70 | None |
| rev-dcat-3-keyword-guidance | is_untracked_revision | True | None |
| rev-pko-core-2-0-1 | name | pko-core-2.0.0 PKO 2.0.1 patch | None |
| rev-pko-core-2-0-1 | affected_mapping_count | 49 | None |
| rev-pko-core-2-0-1 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| rev-pko-core-2-0-1 | days_since_published | 9 | None |
| rev-prov-o-errata | name | prov-o PROV-O errata 2026 | None |
| rev-prov-o-errata | affected_mapping_count | 20 | None |
| rev-prov-o-errata | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| rev-prov-o-errata | days_since_published | 29 | None |

### stakeholder_questions

- Fields: 30/66 (45.5%)
- Computed columns: name, triager_kind, result_route, as_of_instant, days_open, is_unanswered_past_due, is_unanswerable_today, is_untriaged_unanswerable, needs_structural_change, unanswerable_without_scope_request, is_misrouted_after_triage

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| shq-01 | name | Which release approvals were g | None |
| shq-01 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| shq-01 | days_open | 1 | None |
| shq-02 | name | Can the model record the outco | None |
| shq-02 | triager_kind | AIAgent | None |
| shq-02 | result_route | Governance | None |
| shq-02 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| shq-02 | days_open | 1 | None |
| shq-02 | is_unanswerable_today | True | None |
| shq-03 | name | Which benchmarks gated each ri | None |
| shq-03 | triager_kind | Human | None |
| shq-03 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| shq-03 | days_open | 7 | None |
| shq-03 | is_unanswerable_today | True | None |
| shq-03 | needs_structural_change | True | None |
| shq-03 | unanswerable_without_scope_request | True | None |
| shq-04 | name | Which machines were retrofitte | None |
| shq-04 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| shq-04 | days_open | 29 | None |
| shq-04 | is_unanswered_past_due | True | None |
| ... | ... | (16 more) | ... |

### model_expansion_requests

- Fields: 5/21 (23.8%)
- Computed columns: name, model_domain_owner, concept_count, uncovered_concept_count, is_cross_function_expansion, requires_schema_extension, fit_decision_contradicts_concept_fit

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exp-ai-eval-pipeline | name | AI systems team's evaluation p | None |
| exp-ai-eval-pipeline | model_domain_owner | acme-corp | None |
| exp-ai-eval-pipeline | concept_count | 3 | None |
| exp-ai-eval-pipeline | uncovered_concept_count | 1 | None |
| exp-ai-eval-pipeline | is_cross_function_expansion | True | None |
| exp-ai-eval-pipeline | requires_schema_extension | True | None |
| exp-hr-onboarding | name | HR onboarding workflows: check | None |
| exp-hr-onboarding | model_domain_owner | acme-corp | None |
| exp-hr-onboarding | concept_count | 3 | None |
| exp-hr-onboarding | is_cross_function_expansion | True | None |
| exp-internal-audit | name | Internal audit workflows: audi | None |
| exp-internal-audit | model_domain_owner | acme-corp | None |
| exp-internal-audit | concept_count | 3 | None |
| exp-internal-audit | uncovered_concept_count | 1 | None |
| exp-internal-audit | requires_schema_extension | True | None |
| exp-internal-audit | fit_decision_contradicts_concept_fit | True | None |

### expansion_concept_fits

- Fields: 7/18 (38.9%)
- Computed columns: name, is_uncovered

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ecf-ai-benchmark | name | exp-ai-eval-pipeline: Benchmar | None |
| ecf-ai-benchmark | is_uncovered | True | None |
| ecf-ai-candidate | name | exp-ai-eval-pipeline: Candidat | None |
| ecf-ai-run | name | exp-ai-eval-pipeline: Evaluati | None |
| ecf-audit-finding | name | exp-internal-audit: Audit find | None |
| ecf-audit-procedure | name | exp-internal-audit: Audit proc | None |
| ecf-audit-severity | name | exp-internal-audit: Finding se | None |
| ecf-audit-severity | is_uncovered | True | None |
| ecf-hr-buddy | name | exp-hr-onboarding: Buddy assig | None |
| ecf-hr-checklist | name | exp-hr-onboarding: Onboarding  | None |
| ecf-hr-orientation | name | exp-hr-onboarding: Orientation | None |

### competency_question_set_entries

- Fields: 19/56 (33.9%)
- Computed columns: name, as_of_instant, days_since_added, is_outgrown_baseline_question, irrelevant_but_still_active, governance_use_count, serves_every_governance_use

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cqe-close-binding | name | gm-quarter-end-close / q-analy | None |
| cqe-close-binding | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| cqe-close-binding | days_since_added | 505 | None |
| cqe-close-binding | is_outgrown_baseline_question | True | None |
| cqe-close-binding | irrelevant_but_still_active | True | None |
| cqe-close-binding | governance_use_count | 2 | None |
| cqe-close-sign | name | gm-quarter-end-close / q-cfo-c | None |
| cqe-close-sign | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| cqe-close-sign | days_since_added | 505 | None |
| cqe-close-sign | governance_use_count | 4 | None |
| cqe-close-sign | serves_every_governance_use | True | None |
| cqe-close-told | name | gm-quarter-end-close / q-cfo-a | None |
| cqe-close-told | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| cqe-close-told | days_since_added | 505 | None |
| cqe-close-told | is_outgrown_baseline_question | True | None |
| cqe-close-told | governance_use_count | 2 | None |
| cqe-loto-q22 | name | gm-lockout-tagout / aq-pkm2-q2 | None |
| cqe-loto-q22 | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| cqe-loto-q22 | days_since_added | 68 | None |
| cqe-loto-q22 | governance_use_count | 3 | None |
| ... | ... | (17 more) | ... |

### competency_question_runs

- Fields: 24/50 (48.0%)
- Computed columns: name, entry_is_original, prior_was_answerable, is_baseline_regression, is_unfixed_wrong_answer

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cqr-0100-q07 | name | cqe-pko-q07 @ pko-release-0.10 | None |
| cqr-0100-q07 | entry_is_original | True | None |
| cqr-0100-q07 | prior_was_answerable | True | None |
| cqr-0100-q08 | name | cqe-pko-q08 @ pko-release-0.10 | None |
| cqr-0100-q08 | entry_is_original | True | None |
| cqr-0100-q08 | prior_was_answerable | True | None |
| cqr-0100-q08 | is_baseline_regression | True | None |
| cqr-0100-q19 | name | cqe-pko-q19 @ pko-release-0.10 | None |
| cqr-0100-q19 | entry_is_original | True | None |
| cqr-0100-q19 | prior_was_answerable | True | None |
| cqr-090-q07 | name | cqe-pko-q07 @ pko-release-0.9. | None |
| cqr-090-q07 | entry_is_original | True | None |
| cqr-090-q08 | name | cqe-pko-q08 @ pko-release-0.9. | None |
| cqr-090-q08 | entry_is_original | True | None |
| cqr-090-q19 | name | cqe-pko-q19 @ pko-release-0.9. | None |
| cqr-090-q19 | entry_is_original | True | None |
| cqr-100-doclag | name | cqe-pko-doclag @ pko-release-1 | None |
| cqr-100-q07 | name | cqe-pko-q07 @ pko-release-1.0. | None |
| cqr-100-q07 | entry_is_original | True | None |
| cqr-100-q07 | prior_was_answerable | True | None |
| ... | ... | (6 more) | ... |

### competency_question_reviews

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cqrev-close-0315 | name | gm-quarter-end-close question  | None |
| cqrev-pko-0410 | name | gm-pko-rulebook question revie | None |
| cqrev-pko-0630 | name | gm-pko-rulebook question revie | None |
| cqrev-policy-0705 | name | gm-workforce-policy question r | None |

### quality_criteria

- Fields: 0/16 (0.0%)
- Computed columns: name, assessment_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| qc-accuracy | name | Accuracy | None |
| qc-accuracy | assessment_count | 2 | None |
| qc-adaptability | name | Adaptability | None |
| qc-adaptability | assessment_count | 1 | None |
| qc-clarity | name | Clarity | None |
| qc-clarity | assessment_count | 1 | None |
| qc-completeness | name | Completeness | None |
| qc-completeness | assessment_count | 2 | None |
| qc-conciseness | name | Conciseness | None |
| qc-conciseness | assessment_count | 1 | None |
| qc-consistency | name | Consistency | None |
| qc-consistency | assessment_count | 2 | None |
| qc-efficiency | name | Computational efficiency | None |
| qc-efficiency | assessment_count | 1 | None |
| qc-organizational-fitness | name | Organizational fitness | None |
| qc-organizational-fitness | assessment_count | 1 | None |

### quality_assessments

- Fields: 0/11 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| qa-090-accuracy | name | pko-release-0.9.0 / qc-accurac | None |
| qa-090-completeness | name | pko-release-0.9.0 / qc-complet | None |
| qa-090-consistency | name | pko-release-0.9.0 / qc-consist | None |
| qa-100-accuracy | name | pko-release-1.0.0 / qc-accurac | None |
| qa-100-adaptability | name | pko-release-1.0.0 / qc-adaptab | None |
| qa-100-clarity | name | pko-release-1.0.0 / qc-clarity | None |
| qa-100-completeness | name | pko-release-1.0.0 / qc-complet | None |
| qa-100-conciseness | name | pko-release-1.0.0 / qc-concise | None |
| qa-100-consistency | name | pko-release-1.0.0 / qc-consist | None |
| qa-100-efficiency | name | pko-release-1.0.0 / qc-efficie | None |
| qa-100-organizational-fitness | name | pko-release-1.0.0 / qc-organiz | None |

### term_definitions

- Fields: 2/9 (22.2%)
- Computed columns: name, drafter_kind, ai_draft_adopted_unrevised

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| td-drift-observations | name | DriftObservations definition | None |
| td-drift-observations | drafter_kind | AIAgent | None |
| td-drift-observations | ai_draft_adopted_unrevised | True | None |
| td-model-charters | name | ModelCharters definition | None |
| td-model-charters | drafter_kind | AIAgent | None |
| td-procedure-versions | name | ProcedureVersions definition | None |
| td-procedure-versions | drafter_kind | Human | None |

### model_proposals

- Fields: 143/220 (65.0%)
- Computed columns: name, proposer_kind, reviewer_kind, committer_kind, model_steward_agent, is_ai_candidate, is_only_proposed, committed_without_expert_review, entered_without_quality_check, approver_unknown, committed_outside_any_version, ai_question_adopted_unvetted, ai_alignment_decided_by_ai, ai_axiom_without_human_review, adoption_not_answered_by_person, has_no_human_touchpoint, ai_instance_data_loaded_without_steward, awaits_engineer_vetting, is_pending_alignment_decision, awaits_steward_approval

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| mp-01 | name | CompetencyQuestion: Who is esc | None |
| mp-01 | proposer_kind | AIAgent | None |
| mp-01 | model_steward_agent | sam-adeyemi | None |
| mp-01 | is_ai_candidate | True | None |
| mp-01 | is_only_proposed | True | None |
| mp-01 | awaits_engineer_vetting | True | None |
| mp-02 | name | CompetencyQuestion: Who owns e | None |
| mp-02 | proposer_kind | AIAgent | None |
| mp-02 | reviewer_kind | Human | None |
| mp-02 | committer_kind | Human | None |
| mp-02 | model_steward_agent | sam-adeyemi | None |
| mp-02 | is_ai_candidate | True | None |
| mp-03 | name | CompetencyQuestion: Which AI d | None |
| mp-03 | proposer_kind | AIAgent | None |
| mp-03 | committer_kind | Human | None |
| mp-03 | model_steward_agent | sam-adeyemi | None |
| mp-03 | is_ai_candidate | True | None |
| mp-03 | committed_without_expert_review | True | None |
| mp-03 | approver_unknown | True | None |
| mp-03 | committed_outside_any_version | True | None |
| ... | ... | (57 more) | ... |

### assignment_instant_checks

- Fields: 4/32 (12.5%)
- Computed columns: name, step_role, assignment_role, assignment_valid_from, assignment_valid_to, assignment_agent, assignment_agent_version, held_step_at_instant

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| aic-0105-240 | name | deploy-02 / ra-risk-240 | None |
| aic-0105-240 | step_role | change-risk-classifier | None |
| aic-0105-240 | assignment_role | change-risk-classifier | None |
| aic-0105-240 | assignment_valid_from | 2025-11-01T00:00:00-05:00 | None |
| aic-0105-240 | assignment_valid_to | 2026-01-10T00:00:00-06:00 | None |
| aic-0105-240 | assignment_agent | risk-classifier-2-4-0 | None |
| aic-0105-240 | assignment_agent_version | 2.4.0 | None |
| aic-0105-240 | held_step_at_instant | True | None |
| aic-0105-241 | name | deploy-02 / ra-risk-241 | None |
| aic-0105-241 | step_role | change-risk-classifier | None |
| aic-0105-241 | assignment_role | change-risk-classifier | None |
| aic-0105-241 | assignment_valid_from | 2026-01-10T00:00:00-06:00 | None |
| aic-0105-241 | assignment_agent | risk-classifier-2-4-1 | None |
| aic-0105-241 | assignment_agent_version | 2.4.1 | None |
| aic-0301-240 | name | deploy-02 / ra-risk-240 | None |
| aic-0301-240 | step_role | change-risk-classifier | None |
| aic-0301-240 | assignment_role | change-risk-classifier | None |
| aic-0301-240 | assignment_valid_from | 2025-11-01T00:00:00-05:00 | None |
| aic-0301-240 | assignment_valid_to | 2026-01-10T00:00:00-06:00 | None |
| aic-0301-240 | assignment_agent | risk-classifier-2-4-0 | None |
| ... | ... | (8 more) | ... |

### instance_data_versions

- Fields: 0/4 (0.0%)
- Computed columns: name, logged_change_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| idv-2026-07-01 | name | data-2026.07.01 | None |
| idv-2026-07-01 | logged_change_count | 1 | None |
| idv-2026-07-15 | name | data-2026.07.15 | None |
| idv-2026-07-15 | logged_change_count | 1 | None |

### domain_coverage_areas

- Fields: 3/8 (37.5%)
- Computed columns: name, is_uncovered_area

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dca-executions | name | Procedure executions | None |
| dca-governance | name | Model change control | None |
| dca-onboarding | name | Onboarding orientation surveys | None |
| dca-onboarding | is_uncovered_area | True | None |
| dca-procedures | name | Procedure specifications | None |

### governance_stage_controls

- Fields: 12/40 (30.0%)
- Computed columns: name, is_continuous_pipeline_stage

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gsc-lockout-tagout-collection | name | gm-lockout-tagout Collection | None |
| gsc-lockout-tagout-collection | is_continuous_pipeline_stage | True | None |
| gsc-lockout-tagout-encoding | name | gm-lockout-tagout Encoding | None |
| gsc-lockout-tagout-encoding | is_continuous_pipeline_stage | True | None |
| gsc-lockout-tagout-organization | name | gm-lockout-tagout Organization | None |
| gsc-lockout-tagout-organization | is_continuous_pipeline_stage | True | None |
| gsc-lockout-tagout-retrieval | name | gm-lockout-tagout Retrieval | None |
| gsc-lockout-tagout-stewardship | name | gm-lockout-tagout Stewardship | None |
| gsc-lockout-tagout-use | name | gm-lockout-tagout Use | None |
| gsc-production-deployment-collection | name | gm-production-deployment Colle | None |
| gsc-production-deployment-use | name | gm-production-deployment Use | None |
| gsc-quarter-end-close-collection | name | gm-quarter-end-close Collectio | None |
| gsc-quarter-end-close-collection | is_continuous_pipeline_stage | True | None |
| gsc-quarter-end-close-encoding | name | gm-quarter-end-close Encoding | None |
| gsc-quarter-end-close-organization | name | gm-quarter-end-close Organizat | None |
| gsc-quarter-end-close-organization | is_continuous_pipeline_stage | True | None |
| gsc-quarter-end-close-retrieval | name | gm-quarter-end-close Retrieval | None |
| gsc-quarter-end-close-stewardship | name | gm-quarter-end-close Stewardsh | None |
| gsc-quarter-end-close-use | name | gm-quarter-end-close Use | None |
| gsc-workforce-policy-collection | name | gm-workforce-policy Collection | None |
| ... | ... | (8 more) | ... |

### process_design_decisions

- Fields: 5/12 (41.7%)
- Computed columns: name, is_commitment_without_rationale, days_before_recorded, is_recorded_after_the_fact

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pdd-deploy-gate | name | Classify risk before the human | None |
| pdd-deploy-gate | days_before_recorded | 1 | None |
| pdd-loto-night-deputy | name | Escalate night-shift residual  | None |
| pdd-loto-night-deputy | is_commitment_without_rationale | True | None |
| pdd-loto-night-deputy | days_before_recorded | 53 | None |
| pdd-loto-night-deputy | is_recorded_after_the_fact | True | None |
| pdd-loto-verify-cap | name | Cap zero-energy verification a | None |

### model_change_log_entries

- Fields: 217/306 (70.9%)
- Computed columns: name, motivating_question, release_version, release_decision_undocumented, alters_logical_model, is_class_removal_or_rename, is_invalidating_domain_range_change, is_inconsistent_disjointness, is_backward_incompatible, is_additive_schema_change, is_unexplained_modification, rationale_without_question, is_schema_change_without_increment, is_instance_change_in_schema_release, schema_change_without_request, cannot_be_audited, cannot_be_rolled_back, is_untraceable_breaking_change

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| le-01 | name | AddClass: Initial PKO class mo | None |
| le-01 | release_version | 0.8.0 | None |
| le-01 | alters_logical_model | True | None |
| le-01 | is_additive_schema_change | True | None |
| le-01 | rationale_without_question | True | None |
| le-01 | schema_change_without_request | True | None |
| le-02 | name | EditLabel: Clarify step labels | None |
| le-02 | release_version | 0.8.1 | None |
| le-02 | rationale_without_question | True | None |
| le-03 | name | AddInstances: Seed quarter-end | None |
| le-03 | release_version | 0.8.1 | None |
| le-03 | is_unexplained_modification | True | None |
| le-03 | is_instance_change_in_schema_release | True | None |
| le-04 | name | AddClass: Lockout/tagout class | None |
| le-04 | motivating_question | aq-pkm2-q19 | None |
| le-04 | release_version | 0.9.0 | None |
| le-04 | alters_logical_model | True | None |
| le-04 | is_additive_schema_change | True | None |
| le-05 | name | AddProperty: First and fallbac | None |
| le-05 | motivating_question | aq-pkm2-q19 | None |
| ... | ... | (69 more) | ... |

### drift_observations

- Fields: 10/24 (41.7%)
- Computed columns: name, release_passed_validation, release_issued_at, is_open_practice_mismatch, went_undetected_by_passing_suite, drift_follows_clean_release

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| drift-close-competitor-cadence | name | gm-quarter-end-close: A faster | None |
| drift-deploy-sre | name | gm-production-deployment: The  | None |
| drift-deploy-sre | release_passed_validation | True | None |
| drift-deploy-sre | release_issued_at | 2026-06-24T12:00:00-05:00 | None |
| drift-deploy-sre | is_open_practice_mismatch | True | None |
| drift-deploy-sre | went_undetected_by_passing_suite | True | None |
| drift-deploy-sre | drift_follows_clean_release | True | None |
| drift-loto-night-shutdown | name | gm-lockout-tagout: The night s | None |
| drift-loto-night-shutdown | release_issued_at | 2026-06-10T12:00:00-05:00 | None |
| drift-loto-night-shutdown | is_open_practice_mismatch | True | None |
| drift-pko-isolation-map | name | gm-pko-rulebook: The press-7 i | None |
| drift-pko-isolation-map | release_passed_validation | True | None |
| drift-pko-isolation-map | release_issued_at | 2026-04-20T12:00:00-05:00 | None |
| drift-pko-isolation-map | drift_follows_clean_release | True | None |

### sourcing_functions

- Fields: 163/252 (64.7%)
- Computed columns: name, is_outsourced, is_business_process_outsourcing, is_knowledge_process_outsourcing, is_vital_expertise_classed_non_core, is_what_how_split, is_method_knowledge_held_outside, claims_how_without_doing, is_vital_expertise_process, is_outsourced_vital_expertise_process, audit_item_count, dependency_count, coverage_gap_count, specification_audit_count, specification_shortfall_count, method_shortfall_count, provider_ip_documentation_count, designs_what_it_cannot_build, is_unaudited_function, capture_initiative_count, is_uncaptured_priority_process

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sf-corp-deal-analysis | name | acme-corp: Acquisition financi | None |
| sf-corp-deal-analysis | is_outsourced | True | None |
| sf-corp-deal-analysis | is_knowledge_process_outsourcing | True | None |
| sf-corp-deal-analysis | is_vital_expertise_classed_non_core | True | None |
| sf-corp-deal-analysis | is_what_how_split | True | None |
| sf-corp-deal-analysis | is_method_knowledge_held_outside | True | None |
| sf-corp-deal-analysis | is_vital_expertise_process | True | None |
| sf-corp-deal-analysis | is_outsourced_vital_expertise_process | True | None |
| sf-corp-deal-analysis | audit_item_count | 1 | None |
| sf-corp-deal-analysis | capture_initiative_count | 1 | None |
| sf-eng-classifier-training | name | acme-engineering: Change-risk  | None |
| sf-eng-classifier-training | is_outsourced | True | None |
| sf-eng-classifier-training | is_knowledge_process_outsourcing | True | None |
| sf-eng-classifier-training | claims_how_without_doing | True | None |
| sf-eng-classifier-training | is_vital_expertise_process | True | None |
| sf-eng-classifier-training | is_outsourced_vital_expertise_process | True | None |
| sf-eng-classifier-training | audit_item_count | 1 | None |
| sf-eng-classifier-training | dependency_count | 1 | None |
| sf-eng-classifier-training | method_shortfall_count | 1 | None |
| sf-eng-classifier-training | provider_ip_documentation_count | 1 | None |
| ... | ... | (69 more) | ... |

### knowledge_audits

- Fields: 4/12 (33.3%)
- Computed columns: name, finding_count, treats_deficit_as_cost_problem

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| audit-corp-2024 | name | acme-corp: Shared services cos | None |
| audit-eng-2026 | name | acme-engineering: Release and  | None |
| audit-eng-2026 | finding_count | 1 | None |
| audit-hb-2025 | name | acme-home-brands: Supplier cos | None |
| audit-hb-2025 | finding_count | 2 | None |
| audit-hb-2025 | treats_deficit_as_cost_problem | True | None |
| audit-plant-2026 | name | acme-plant: Plant process know | None |
| audit-plant-2026 | finding_count | 2 | None |

### knowledge_audit_items

- Fields: 47/84 (56.0%)
- Computed columns: name, client_organization, has_internal_shortfall, is_knowledge_dependency, is_coverage_gap, is_unnamed_finding, is_single_team_silo

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kai-corp-deal-method | name | audit-corp-2024 / Acquisition  | None |
| kai-corp-deal-method | client_organization | acme-corp | None |
| kai-eng-deploy-method | name | audit-eng-2026 / Release gatin | None |
| kai-eng-deploy-method | client_organization | acme-engineering | None |
| kai-eng-training-method | name | audit-eng-2026 / Retraining an | None |
| kai-eng-training-method | client_organization | acme-engineering | None |
| kai-eng-training-method | has_internal_shortfall | True | None |
| kai-eng-training-method | is_knowledge_dependency | True | None |
| kai-hb-assembly-method | name | audit-hb-2025 / Assembly seque | None |
| kai-hb-assembly-method | client_organization | acme-home-brands | None |
| kai-hb-assembly-method | has_internal_shortfall | True | None |
| kai-hb-assembly-method | is_knowledge_dependency | True | None |
| kai-hb-assembly-method | is_unnamed_finding | True | None |
| kai-hb-assembly-spec | name | audit-hb-2025 / Product specif | None |
| kai-hb-assembly-spec | client_organization | acme-home-brands | None |
| kai-hb-customers | name | audit-hb-2025 / Retail partner | None |
| kai-hb-customers | client_organization | acme-home-brands | None |
| kai-hb-safety-method | name | audit-hb-2025 / Interpreting p | None |
| kai-hb-safety-method | client_organization | acme-home-brands | None |
| kai-hb-safety-method | has_internal_shortfall | True | None |
| ... | ... | (17 more) | ... |

### knowledge_capture_initiatives

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kci-corp-deal-pairing | name | sf-corp-deal-analysis by Pract | None |
| kci-plant-mixer2-incidents | name | sf-plant-lockout by CriticalIn | None |
| kci-plant-press-shadowing | name | sf-plant-press-overhaul by Sha | None |

### knowledge_workforce_positions

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kwp-eng-ia | name | acme-engineering InformationAr | None |
| kwp-eng-ontologist | name | acme-engineering Ontologist | None |
| kwp-hb-ke | name | acme-home-brands KnowledgeEngi | None |
| kwp-plant-ke | name | acme-plant KnowledgeEngineer | None |

### provider_engagements

- Fields: 48/98 (49.0%)
- Computed columns: name, is_active, relied_dependency_count, is_unplanned_knowledge_return, is_knowledge_access_unsecured, required_deliverable_count, to_client_required_count, to_client_delivered_count, to_provider_delivered_count, joint_deliverable_count, lacks_knowledge_deliverables, is_one_way_learning, is_short_term_without_joint_knowledge, obliges_knowledge_flow_back

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pe-corp-lattice | name | acme-corp <- lattice-analytics | None |
| pe-corp-lattice | is_active | True | None |
| pe-corp-lattice | required_deliverable_count | 3 | None |
| pe-corp-lattice | to_client_required_count | 1 | None |
| pe-corp-lattice | to_client_delivered_count | 1 | None |
| pe-corp-lattice | to_provider_delivered_count | 1 | None |
| pe-corp-lattice | joint_deliverable_count | 1 | None |
| pe-corp-lattice | obliges_knowledge_flow_back | True | None |
| pe-eng-northstar | name | acme-engineering <- northstar- | None |
| pe-eng-northstar | is_active | True | None |
| pe-eng-northstar | relied_dependency_count | 1 | None |
| pe-eng-northstar | is_unplanned_knowledge_return | True | None |
| pe-eng-northstar | is_knowledge_access_unsecured | True | None |
| pe-eng-northstar | required_deliverable_count | 1 | None |
| pe-eng-northstar | to_provider_delivered_count | 1 | None |
| pe-eng-northstar | is_one_way_learning | True | None |
| pe-eng-northstar | is_short_term_without_joint_knowledge | True | None |
| pe-hb-harborline | name | acme-home-brands <- harborline | None |
| pe-hb-harborline | is_active | True | None |
| pe-hb-harborline | required_deliverable_count | 2 | None |
| ... | ... | (30 more) | ... |

### knowledge_deliverables

- Fields: 1/20 (5.0%)
- Computed columns: name, is_delivered

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kd-baxter-failure-catalog | name | pe-plant-baxter Joint: Shared  | None |
| kd-baxter-incidents | name | pe-plant-baxter ToProvider: Pr | None |
| kd-baxter-incidents | is_delivered | True | None |
| kd-baxter-procedures | name | pe-plant-baxter ToClient: Pres | None |
| kd-baxter-procedures | is_delivered | True | None |
| kd-harborline-changelog | name | pe-hb-harborline ToClient: Mon | None |
| kd-harborline-changelog | is_delivered | True | None |
| kd-harborline-policy | name | pe-hb-harborline ToProvider: P | None |
| kd-harborline-policy | is_delivered | True | None |
| kd-lattice-context | name | pe-corp-lattice ToProvider: In | None |
| kd-lattice-context | is_delivered | True | None |
| kd-lattice-models | name | pe-corp-lattice ToClient: Valu | None |
| kd-lattice-models | is_delivered | True | None |
| kd-lattice-playbook | name | pe-corp-lattice Joint: Joint d | None |
| kd-lattice-playbook | is_delivered | True | None |
| kd-northstar-history | name | pe-eng-northstar ToProvider: L | None |
| kd-northstar-history | is_delivered | True | None |
| kd-quillstone-digest | name | pe-hb-quillstone ToClient: Qua | None |
| kd-quillstone-digest | is_delivered | True | None |

### corporate_governance_programs

- Fields: 2/6 (33.3%)
- Computed columns: name, is_compliance_only

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gp-hb-compliance-2016 | name | Enterprise compliance and reco | None |
| gp-hb-compliance-2016 | is_compliance_only | True | None |
| gp-hb-engineering-records-2004 | name | Engineering records and knowle | None |
| gp-plant-information-2021 | name | Plant information and knowledg | None |

### records_retention_policies

- Fields: 3/12 (25.0%)
- Computed columns: name, program_sponsor_discipline, is_legal_led_knowledge_destruction

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rrp-hb-chat-2016 | name | Email and chat / ComplianceRis | None |
| rrp-hb-chat-2016 | program_sponsor_discipline | Legal | None |
| rrp-hb-design-history-2004 | name | Design history files / Knowled | None |
| rrp-hb-design-history-2004 | program_sponsor_discipline | Engineering | None |
| rrp-hb-design-history-2016 | name | Design history and engineering | None |
| rrp-hb-design-history-2016 | program_sponsor_discipline | Legal | None |
| rrp-hb-design-history-2016 | is_legal_led_knowledge_destruction | True | None |
| rrp-plant-maintenance-logs | name | Maintenance and lockout logs / | None |
| rrp-plant-maintenance-logs | program_sponsor_discipline | Engineering | None |

### ai_registry_model_versions

- Fields: 11/24 (45.8%)
- Computed columns: name, graph_individual_count, live_production_deployment_count, is_live_but_unregistered_in_graph

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| reg-health-check-1.2.0 | name | Post-deployment health checker | None |
| reg-health-check-1.2.0 | graph_individual_count | 1 | None |
| reg-release-notes-1.1.0 | name | Release notes writer 1.1.0 | None |
| reg-release-notes-1.1.0 | live_production_deployment_count | 1 | None |
| reg-release-notes-1.1.0 | is_live_but_unregistered_in_graph | True | None |
| reg-risk-classifier-2.3.0 | name | Change risk classifier 2.3.0 | None |
| reg-risk-classifier-2.4.0 | name | Change risk classifier 2.4.0 | None |
| reg-risk-classifier-2.4.0 | graph_individual_count | 1 | None |
| reg-risk-classifier-2.4.1 | name | Change risk classifier 2.4.1 | None |
| reg-risk-classifier-2.4.1 | graph_individual_count | 1 | None |
| reg-risk-classifier-2.4.1 | live_production_deployment_count | 1 | None |
| reg-risk-classifier-2.5.0-rc3 | name | Change risk classifier 2.5.0-r | None |
| reg-risk-classifier-2.5.0-rc3 | graph_individual_count | 1 | None |

### ai_model_deployments

- Fields: 16/42 (38.1%)
- Computed columns: name, as_of_instant, is_live_in_production, agent_identifier, assignment_link_count, artifact_link_count, is_isolated_registry_fact

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dep-health-1.2.0-prod | name | reg-health-check-1.2.0 @ Produ | None |
| dep-health-1.2.0-prod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| dep-health-1.2.0-prod | agent_identifier | health-check-ai-1-2-0 | None |
| dep-health-1.2.0-prod | assignment_link_count | 1 | None |
| dep-release-notes-1.1.0-prod | name | reg-release-notes-1.1.0 @ Prod | None |
| dep-release-notes-1.1.0-prod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| dep-release-notes-1.1.0-prod | is_live_in_production | True | None |
| dep-release-notes-1.1.0-prod | agent_identifier | release-notes-writer-1-1-0 | None |
| dep-release-notes-1.1.0-prod | is_isolated_registry_fact | True | None |
| dep-risk-2.3.0-prod | name | reg-risk-classifier-2.3.0 @ Pr | None |
| dep-risk-2.3.0-prod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| dep-risk-2.3.0-prod | agent_identifier | risk-classifier-2-3-0 | None |
| dep-risk-2.4.0-prod | name | reg-risk-classifier-2.4.0 @ Pr | None |
| dep-risk-2.4.0-prod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| dep-risk-2.4.0-prod | agent_identifier | risk-classifier-2-4-0 | None |
| dep-risk-2.4.0-prod | assignment_link_count | 1 | None |
| dep-risk-2.4.0-prod | artifact_link_count | 1 | None |
| dep-risk-2.4.1-prod | name | reg-risk-classifier-2.4.1 @ Pr | None |
| dep-risk-2.4.1-prod | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| dep-risk-2.4.1-prod | is_live_in_production | True | None |
| ... | ... | (6 more) | ... |

### ai_model_evaluations

- Fields: 1/6 (16.7%)
- Computed columns: name, passed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| eval-health-1.2.0-replay | name | reg-health-check-1.2.0 Degrada | None |
| eval-risk-2.4.1-holdout | name | reg-risk-classifier-2.4.1 High | None |
| eval-risk-2.4.1-holdout | passed | True | None |
| eval-risk-2.5.0-holdout | name | reg-risk-classifier-2.5.0-rc3  | None |
| eval-risk-2.5.0-holdout | passed | True | None |

### ai_agent_accountabilities

- Fields: 8/24 (33.3%)
- Computed columns: name, as_of_instant, is_current, accountable_agent_kind, is_accountable_to_non_person, is_current_human_accountability

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| aaa-health-120-omar | name | health-check-ai-1-2-0 accounta | None |
| aaa-health-120-omar | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| aaa-health-120-omar | accountable_agent_kind | Human | None |
| aaa-risk-240-omar | name | risk-classifier-2-4-0 accounta | None |
| aaa-risk-240-omar | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| aaa-risk-240-omar | accountable_agent_kind | Human | None |
| aaa-risk-241-omar | name | risk-classifier-2-4-1 accounta | None |
| aaa-risk-241-omar | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| aaa-risk-241-omar | is_current | True | None |
| aaa-risk-241-omar | accountable_agent_kind | Human | None |
| aaa-risk-241-omar | is_current_human_accountability | True | None |
| aaa-risk-250-pipeline | name | risk-classifier-2-5-0 accounta | None |
| aaa-risk-250-pipeline | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| aaa-risk-250-pipeline | is_current | True | None |
| aaa-risk-250-pipeline | accountable_agent_kind | AutomatedPipeline | None |
| aaa-risk-250-pipeline | is_accountable_to_non_person | True | None |

### agent_upgrade_assessments

- Fields: 1/8 (12.5%)
- Computed columns: name, attributed_artifact_count, traversed_downstream_step_count, missed_traversed_impact

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| upg-risk-241-250-graph | name | risk-classifier-2-4-1 -> risk- | None |
| upg-risk-241-250-graph | attributed_artifact_count | 2 | None |
| upg-risk-241-250-graph | traversed_downstream_step_count | 3 | None |
| upg-risk-241-250-ticket | name | risk-classifier-2-4-1 -> risk- | None |
| upg-risk-241-250-ticket | attributed_artifact_count | 2 | None |
| upg-risk-241-250-ticket | traversed_downstream_step_count | 3 | None |
| upg-risk-241-250-ticket | missed_traversed_impact | True | None |

### assignment_update_policies

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| aup-ai-agents | name | AI agent role fills | None |
| aup-eng-operations | name | Engineering operations rotas ( | None |
| aup-release-approvers | name | Release approval roles | None |

### role_assignment_update_tasks

- Fields: 37/102 (36.3%)
- Computed columns: name, as_of_instant, policy_trigger_owner_role, trigger_owner_cover_count, lacks_named_trigger_owner, policy_sla_hours, elapsed_minutes, exceeded_update_sla, ending_role, replacement_role, changed_role_instead_of_assignment, dependent_run_version, dependent_run_started_at, dependent_run_role_step_count, missed_next_dependent_run, failed_notice_count, stale_assignment_broke_routing

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rau-build-to-pipeline | name | build-release-engineer HumanTo | None |
| rau-build-to-pipeline | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| rau-build-to-pipeline | lacks_named_trigger_owner | True | None |
| rau-build-to-pipeline | elapsed_minutes | 44100.0 | None |
| rau-build-to-pipeline | exceeded_update_sla | True | None |
| rau-build-to-pipeline | ending_role | build-release-engineer | None |
| rau-build-to-pipeline | replacement_role | deployment-automation | None |
| rau-build-to-pipeline | changed_role_instead_of_assignment | True | None |
| rau-deploy-kwame-departure | name | deployment-automation Departur | None |
| rau-deploy-kwame-departure | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| rau-deploy-kwame-departure | lacks_named_trigger_owner | True | None |
| rau-deploy-kwame-departure | elapsed_minutes | 11100.0 | None |
| rau-deploy-kwame-departure | exceeded_update_sla | True | None |
| rau-deploy-kwame-departure | ending_role | deployment-automation | None |
| rau-deploy-kwame-departure | dependent_run_version | deploy-v3.2.0 | None |
| rau-deploy-kwame-departure | dependent_run_started_at | 2026-07-14T16:00:00-05:00 | None |
| rau-deploy-kwame-departure | dependent_run_role_step_count | 2 | None |
| rau-deploy-kwame-departure | missed_next_dependent_run | True | None |
| rau-release-dmitri-departure | name | release-manager Departure @ 20 | None |
| rau-release-dmitri-departure | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ... | ... | (45 more) | ... |

### assignment_routed_notices

- Fields: 23/60 (38.3%)
- Computed columns: name, notice_role, recipient_role_key, recipient_pair_assignment_count, recipient_valid_from, recipient_latest_valid_to, recipient_open_ended_count, recipient_held_role_when_sent, reached_wrong_person_or_nobody, routed_around_model

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| arn-dep0105-03 | name | exec-deploy-2026-01-05 / deplo | None |
| arn-dep0105-03 | notice_role | release-manager | None |
| arn-dep0105-03 | recipient_role_key | grace-holloway|release-manager | None |
| arn-dep0105-03 | recipient_pair_assignment_count | 1 | None |
| arn-dep0105-03 | recipient_valid_from | 2024-02-01T00:00:00-06:00 | None |
| arn-dep0105-03 | recipient_open_ended_count | 1 | None |
| arn-dep0105-03 | recipient_held_role_when_sent | True | None |
| arn-dep0105-05 | name | exec-deploy-2026-01-05 / deplo | None |
| arn-dep0105-05 | notice_role | site-reliability-engineer | None |
| arn-dep0105-05 | recipient_role_key | leo-marchetti|site-reliability | None |
| arn-dep0105-05 | recipient_pair_assignment_count | 1 | None |
| arn-dep0105-05 | recipient_valid_from | 2026-01-01T00:00:00-06:00 | None |
| arn-dep0105-05 | recipient_latest_valid_to | 2026-06-30T17:00:00-05:00 | None |
| arn-dep0105-05 | recipient_held_role_when_sent | True | None |
| arn-dep0301-03 | name | exec-deploy-2026-03-01 / deplo | None |
| arn-dep0301-03 | notice_role | release-manager | None |
| arn-dep0301-03 | recipient_role_key | grace-holloway|release-manager | None |
| arn-dep0301-03 | recipient_pair_assignment_count | 1 | None |
| arn-dep0301-03 | recipient_valid_from | 2024-02-01T00:00:00-06:00 | None |
| arn-dep0301-03 | recipient_open_ended_count | 1 | None |
| ... | ... | (17 more) | ... |

### practitioner_expertise

- Fields: 8/15 (53.3%)
- Computed columns: name, knows_more_than_can_say, is_unexplained_foresight

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pe7-aisha-bleed | name | aisha-bello: A bleed that did  | None |
| pe7-ken-gauge | name | ken-watanabe: Residual pressur | None |
| pe7-omar-canary | name | omar-haddad: Canary stages wit | None |
| pe7-ravi-build-cache | name | ravi-menon: The shared build c | None |
| pe7-ravi-build-cache | is_unexplained_foresight | True | None |
| pe7-tomas-bleed | name | tomas-reyes: A bleed that did  | None |
| pe7-tomas-bleed | knows_more_than_can_say | True | None |

### critical_incidents

- Fields: 2/9 (22.2%)
- Computed columns: name, is_adverse_or_improvised, has_surfaced_judgment

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ci7-conveyor5-handover | name | WentWell: A lockout spanning t | None |
| ci7-mixer2-second-drive | name | WentBadly: The second drive of | None |
| ci7-mixer2-second-drive | is_adverse_or_improvised | True | None |
| ci7-mixer2-second-drive | has_surfaced_judgment | True | None |
| ci7-press3-gauge-lied | name | Improvised: The gauge read zer | None |
| ci7-press3-gauge-lied | is_adverse_or_improvised | True | None |
| ci7-press3-gauge-lied | has_surfaced_judgment | True | None |

### interview_probes

- Fields: 5/12 (41.7%)
- Computed columns: name, why_answer, shortfall_answer

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ip7-deploy-why-canary | name | Why: Why do you hold the canar | None |
| ip7-deploy-why-canary | why_answer | Weekend traffic shifts the err | None |
| ip7-loto-short-zero | name | ProcedureFallsShort: What do y | None |
| ip7-loto-short-zero | shortfall_answer | On a retrofit with a capacitor | None |
| ip7-loto-whatif-gauge | name | WhatIf: What if the line had a | None |
| ip7-loto-why-wait | name | Why: Why do you wait before bl | None |
| ip7-loto-why-wait | why_answer | The press 3 accumulator refill | None |

### observed_actions

- Fields: 21/40 (52.5%)
- Computed columns: name, is_small_choice, is_unofficial_workaround, is_omitted_from_own_account, is_missed_step_left_uncaptured, is_watched_not_questioned, has_recorded_reason, has_counterfactual_answer

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| oa7-close-fx-check | name | SmallChoice: Rechecks the FX r | None |
| oa7-close-fx-check | is_small_choice | True | None |
| oa7-close-fx-check | is_omitted_from_own_account | True | None |
| oa7-close-fx-check | has_recorded_reason | True | None |
| oa7-hand-on-valve | name | SmallChoice: Rests a hand on t | None |
| oa7-hand-on-valve | is_small_choice | True | None |
| oa7-hand-on-valve | is_omitted_from_own_account | True | None |
| oa7-hand-on-valve | has_recorded_reason | True | None |
| oa7-hand-on-valve | has_counterfactual_answer | True | None |
| oa7-open-disconnect | name | DocumentedStep: Opens and lock | None |
| oa7-open-disconnect | has_recorded_reason | True | None |
| oa7-spare-lock-cart | name | Workaround: Hangs a spare pers | None |
| oa7-spare-lock-cart | is_unofficial_workaround | True | None |
| oa7-spare-lock-cart | is_watched_not_questioned | True | None |
| oa7-tryout-button | name | OmittedObviousStep: Presses th | None |
| oa7-tryout-button | is_omitted_from_own_account | True | None |
| oa7-tryout-button | is_missed_step_left_uncaptured | True | None |
| oa7-tryout-button | has_recorded_reason | True | None |
| oa7-tryout-button | has_counterfactual_answer | True | None |

### elicitation_participants

- Fields: 18/42 (42.9%)
- Computed columns: name, is_practitioner, is_subject_matter_expert, is_knowledge_engineer, is_knowledge_producer, is_knowledge_consumer

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ep7-lw-hana | name | hana-kowalski in el7-loto-work | None |
| ep7-lw-hana | is_subject_matter_expert | True | None |
| ep7-lw-hana | is_knowledge_consumer | True | None |
| ep7-lw-lin | name | lin-zhao in el7-loto-workshop | None |
| ep7-lw-lin | is_subject_matter_expert | True | None |
| ep7-lw-lin | is_knowledge_producer | True | None |
| ep7-lw-lin | is_knowledge_consumer | True | None |
| ep7-lw-rosa | name | rosa-delgado in el7-loto-works | None |
| ep7-lw-rosa | is_practitioner | True | None |
| ep7-lw-rosa | is_knowledge_producer | True | None |
| ep7-lw-rosa | is_knowledge_consumer | True | None |
| ep7-lw-sam | name | sam-adeyemi in el7-loto-worksh | None |
| ep7-lw-sam | is_knowledge_engineer | True | None |
| ep7-lw-sam | is_knowledge_consumer | True | None |
| ep7-lw-tomas | name | tomas-reyes in el7-loto-worksh | None |
| ep7-lw-tomas | is_practitioner | True | None |
| ep7-lw-tomas | is_knowledge_producer | True | None |
| ep7-pw-amina | name | amina-yusuf in elicit-policy-w | None |
| ep7-pw-amina | is_practitioner | True | None |
| ep7-pw-amina | is_knowledge_producer | True | None |
| ... | ... | (4 more) | ... |

### representation_reviews

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rr7-deploy-omar | name | RepresentationApproval: deploy | None |
| rr7-deploy-ravi-ai | name | AiEvaluation: deploy-v3.2.0 by | None |
| rr7-loto-lin | name | RepresentationApproval: loto-v | None |
| rr7-loto-tomas | name | RepresentationApproval: loto-v | None |

### workflow_view_divergences

- Fields: 2/6 (33.3%)
- Computed columns: name, is_reconciled, is_surfaced_but_unreconciled

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| wd7-loto-last-tag | name | loto-08: The day crew removes  | None |
| wd7-loto-last-tag | is_reconciled | True | None |
| wd7-loto-notify-timing | name | loto-02: Operators are notifie | None |
| wd7-loto-notify-timing | is_surfaced_but_unreconciled | True | None |

### expert_cognitions

- Fields: 5/12 (41.7%)
- Computed columns: name, is_mental_model, is_automatic_heuristic, is_heuristic_oversimplified

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ec7-energy-reservoirs | name | MentalModel: A machine is a se | None |
| ec7-energy-reservoirs | is_mental_model | True | None |
| ec7-needle-stops | name | DecisionHeuristic: Trust zero  | None |
| ec7-needle-stops | is_automatic_heuristic | True | None |
| ec7-retrofit-two-machines | name | DecisionHeuristic: If a machin | None |
| ec7-retrofit-two-machines | is_automatic_heuristic | True | None |
| ec7-retrofit-two-machines | is_heuristic_oversimplified | True | None |

### concept_ladder_rungs

- Fields: 6/20 (30.0%)
- Computed columns: name, step_top_level, is_ultimate_goal, is_decomposition_rung

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cl7-bleed-down1 | name | loto-04b Subprocess: Close the | None |
| cl7-bleed-down1 | step_top_level | 2 | None |
| cl7-bleed-down1 | is_decomposition_rung | True | None |
| cl7-bleed-down2 | name | loto-04b Condition: Only once  | None |
| cl7-bleed-down2 | step_top_level | 2 | None |
| cl7-bleed-down2 | is_decomposition_rung | True | None |
| cl7-bleed-up1 | name | loto-04b Goal: No trapped air  | None |
| cl7-bleed-up1 | step_top_level | 2 | None |
| cl7-bleed-up2 | name | loto-04b Value: Nobody's hand  | None |
| cl7-bleed-up2 | step_top_level | 2 | None |
| cl7-bleed-up2 | is_ultimate_goal | True | None |
| cl7-lock-up1 | name | loto-05 Value: Each person kee | None |
| cl7-lock-up1 | step_top_level | 1 | None |
| cl7-lock-up1 | is_ultimate_goal | True | None |

### repertory_grid_constructs

- Fields: 2/12 (16.7%)
- Computed columns: name, dimension, is_never_stated_dimension, is_recorded_discriminating_dimension

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rg7-audible-silent | name | You can hear it bleed vs It bl | None |
| rg7-audible-silent | dimension | You can hear it bleed vs It bl | None |
| rg7-audible-silent | is_never_stated_dimension | True | None |
| rg7-audible-silent | is_recorded_discriminating_dimension | True | None |
| rg7-large-small | name | Large machine vs Small machine | None |
| rg7-large-small | dimension | Large machine vs Small machine | None |
| rg7-retrofit-original | name | Retrofitted drive vs Original  | None |
| rg7-retrofit-original | dimension | Retrofitted drive vs Original  | None |
| rg7-retrofit-original | is_never_stated_dimension | True | None |
| rg7-retrofit-original | is_recorded_discriminating_dimension | True | None |

### knowledge_conversions

- Fields: 4/10 (40.0%)
- Computed columns: name, is_mode_inconsistent_with_forms

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kc7-combine-into-v2 | name | Combination: The hiss fragment | None |
| kc7-externalize-hiss | name | Externalization: Tomas's bleed | None |
| kc7-internalize-aisha | name | Internalization: Aisha rehears | None |
| kc7-retyped-note | name | Externalization: The press 3 m | None |
| kc7-retyped-note | is_mode_inconsistent_with_forms | True | None |
| kc7-socialize-hand-on-valve | name | Socialization: Ken picked up t | None |

### knowledge_holdings

- Fields: 38/70 (54.3%)
- Computed columns: name, formalized_fragment_session, is_unformalized_process_knowledge, is_procedural_knowledge, is_judgment_outside_document, is_veteran_discretion, is_formalized_without_elicitation_work

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kh7-after-bleed-cue | name | After ProcedureRulebook: Bleed | None |
| kh7-after-bleed-cue | formalized_fragment_session | el7-loto-interview | None |
| kh7-after-bleed-cue | is_procedural_knowledge | True | None |
| kh7-after-last-tag | name | After ProcedureRulebook: Walkd | None |
| kh7-after-last-tag | formalized_fragment_session | el7-loto-workshop | None |
| kh7-after-last-tag | is_procedural_knowledge | True | None |
| kh7-after-margin-note | name | After ProcedureRulebook: Bleed | None |
| kh7-after-margin-note | is_procedural_knowledge | True | None |
| kh7-after-margin-note | is_formalized_without_elicitation_work | True | None |
| kh7-after-retrofit-zero | name | After OperatorMemory: When to  | None |
| kh7-after-retrofit-zero | is_unformalized_process_knowledge | True | None |
| kh7-after-retrofit-zero | is_judgment_outside_document | True | None |
| kh7-after-retrofit-zero | is_veteran_discretion | True | None |
| kh7-after-twin-drive | name | After ProcedureRulebook: Mixer | None |
| kh7-after-twin-drive | formalized_fragment_session | el7-loto-cit | None |
| kh7-after-twin-drive | is_procedural_knowledge | True | None |
| kh7-before-bleed-cue | name | Before OperatorMemory: How to  | None |
| kh7-before-bleed-cue | is_unformalized_process_knowledge | True | None |
| kh7-before-bleed-cue | is_judgment_outside_document | True | None |
| kh7-before-bleed-cue | is_veteran_discretion | True | None |
| ... | ... | (12 more) | ... |

### fragment_corroborations

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| fc7-hiss-incident | name | kf7-loto-hiss via el7-loto-cit | None |
| fc7-hiss-shadow | name | kf7-loto-hiss via el7-loto-sha | None |
| fc7-last-tag-workshop | name | kf7-loto-tag-handover via el7- | None |
| fc7-mixer2-ken-disagrees | name | kf7-loto-mixer2 via el7-loto-t | None |

### knowledge_test_outcomes

- Fields: 1/10 (10.0%)
- Computed columns: name, is_improved

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kt7-loto-competency-gaps | name | lockout-tagout: CompetencyGaps | None |
| kt7-loto-competency-gaps | is_improved | True | None |
| kt7-loto-consistency | name | lockout-tagout: Consistency | None |
| kt7-loto-consistency | is_improved | True | None |
| kt7-loto-efficiency | name | lockout-tagout: Efficiency | None |
| kt7-loto-efficiency | is_improved | True | None |
| kt7-loto-quality | name | lockout-tagout: ProcessQuality | None |
| kt7-loto-quality | is_improved | True | None |
| kt7-loto-satisfaction | name | lockout-tagout: JobSatisfactio | None |

### know_how_carriers

- Fields: 225/364 (61.8%)
- Computed columns: name, as_of_instant, holder_is_still_engaged, holder_service_started_at, holder_departure_at, days_until_holder_departure, days_served_to_as_of, days_served_to_departure, holder_tenure_years, is_veteran_held, is_holder_leaving_soon, transfer_count, repository_entry_count, source_relationship_count, is_held_in_both_forms, is_held_by_current_practitioner, is_untransferred_veteran_know_how, is_at_risk_of_imminent_loss, is_held_only_by_departed, is_held_by_departed_holder, is_captured, must_be_relearned_if_holder_leaves, is_overlooked_living_holder, transfer_stops_without_veteran, dependency_community, builds_on_same_community_know_how, lost_accumulation_years, is_delegated_to_unfit_source

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| khc12-dmitri-triage | name | Customer incident triage heuri | None |
| khc12-dmitri-triage | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| khc12-dmitri-triage | holder_service_started_at | 2014-05-01T00:00:00-05:00 | None |
| khc12-dmitri-triage | holder_departure_at | 2026-07-03T17:00:00-05:00 | None |
| khc12-dmitri-triage | days_until_holder_departure | -16 | None |
| khc12-dmitri-triage | days_served_to_as_of | 4462 | None |
| khc12-dmitri-triage | days_served_to_departure | 4446 | None |
| khc12-dmitri-triage | holder_tenure_years | 12.2 | None |
| khc12-dmitri-triage | is_veteran_held | True | None |
| khc12-dmitri-triage | is_held_only_by_departed | True | None |
| khc12-dmitri-triage | is_held_by_departed_holder | True | None |
| khc12-dmitri-triage | lost_accumulation_years | 12.2 | None |
| khc12-grace-gonogo | name | Go/no-go judgment when the ris | None |
| khc12-grace-gonogo | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| khc12-grace-gonogo | holder_is_still_engaged | True | None |
| khc12-grace-gonogo | holder_service_started_at | 2012-02-01T00:00:00-06:00 | None |
| khc12-grace-gonogo | days_until_holder_departure | 99999 | None |
| khc12-grace-gonogo | days_served_to_as_of | 5282 | None |
| khc12-grace-gonogo | holder_tenure_years | 14.5 | None |
| khc12-grace-gonogo | is_veteran_held | True | None |
| ... | ... | (119 more) | ... |

### knowledge_transfers

- Fields: 24/63 (38.1%)
- Computed columns: name, from_organization, recipient_role_count, is_traditional_channel, is_social_network_channel, is_ambient_absorption_by_non_practitioner, know_how_topic

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kt12-grace-leo-doc | name | khc12-grace-gonogo via Documen | None |
| kt12-grace-leo-doc | from_organization | acme-engineering | None |
| kt12-grace-leo-doc | is_traditional_channel | True | None |
| kt12-grace-leo-doc | know_how_topic | Go/no-go judgment when the ris | None |
| kt12-grace-ravi-pairing | name | khc12-grace-gonogo via Collabo | None |
| kt12-grace-ravi-pairing | from_organization | acme-engineering | None |
| kt12-grace-ravi-pairing | is_social_network_channel | True | None |
| kt12-grace-ravi-pairing | know_how_topic | Go/no-go judgment when the ris | None |
| kt12-joao-move | name | khc12-joao-servo via EmployerC | None |
| kt12-joao-move | from_organization | baxter-hydraulics | None |
| kt12-joao-move | is_social_network_channel | True | None |
| kt12-joao-move | know_how_topic | Servo cam timing for high-spee | None |
| kt12-joao-nina-ambient | name | khc12-joao-servo via AmbientEx | None |
| kt12-joao-nina-ambient | from_organization | baxter-hydraulics | None |
| kt12-joao-nina-ambient | is_ambient_absorption_by_non_practitioner | True | None |
| kt12-joao-nina-ambient | know_how_topic | Servo cam timing for high-spee | None |
| kt12-joao-petra-iteration | name | khc12-joao-servo via DesignPro | None |
| kt12-joao-petra-iteration | from_organization | baxter-hydraulics | None |
| kt12-joao-petra-iteration | is_social_network_channel | True | None |
| kt12-joao-petra-iteration | know_how_topic | Servo cam timing for high-spee | None |
| ... | ... | (19 more) | ... |

### knowledge_repository_entries

- Fields: 34/88 (38.6%)
- Computed columns: name, as_of_instant, author_is_still_engaged, author_agent_kind, owner_organization, days_since_updated, is_stale, is_execution_feedback, is_machine_authored, outlives_author_tenure, is_uncredited_expert_know_how

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kre12-build-runbook | name | Hand-building a release candid | None |
| kre12-build-runbook | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kre12-build-runbook | author_agent_kind | Human | None |
| kre12-build-runbook | owner_organization | acme-engineering | None |
| kre12-build-runbook | days_since_updated | 60 | None |
| kre12-build-runbook | outlives_author_tenure | True | None |
| kre12-cache-purge-fix | name | Purge the CDN cache after a de | None |
| kre12-cache-purge-fix | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kre12-cache-purge-fix | author_is_still_engaged | True | None |
| kre12-cache-purge-fix | author_agent_kind | Human | None |
| kre12-cache-purge-fix | owner_organization | acme-engineering | None |
| kre12-cache-purge-fix | days_since_updated | 68 | None |
| kre12-close-fx-retro | name | Q2 close retrospective: FX rat | None |
| kre12-close-fx-retro | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| kre12-close-fx-retro | author_is_still_engaged | True | None |
| kre12-close-fx-retro | author_agent_kind | Human | None |
| kre12-close-fx-retro | owner_organization | acme-finance | None |
| kre12-close-fx-retro | days_since_updated | 16 | None |
| kre12-close-fx-retro | is_execution_feedback | True | None |
| kre12-conveyor-ai-summary | name | Conveyor maintenance summary g | None |
| ... | ... | (34 more) | ... |

### community_memberships

- Fields: 6/36 (16.7%)
- Computed columns: name, member_organization, community_organization, is_external_member

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cm12-grace-release | name | grace-holloway in release-engi | None |
| cm12-grace-release | member_organization | acme-engineering | None |
| cm12-grace-release | community_organization | acme-engineering | None |
| cm12-joao-eastvale | name | joao-ferreira in eastvale-pack | None |
| cm12-joao-eastvale | member_organization | baxter-hydraulics | None |
| cm12-joao-eastvale | is_external_member | True | None |
| cm12-joao-riverbend | name | joao-ferreira in riverbend-pre | None |
| cm12-joao-riverbend | member_organization | baxter-hydraulics | None |
| cm12-joao-riverbend | community_organization | acme-plant | None |
| cm12-joao-riverbend | is_external_member | True | None |
| cm12-ken-plant | name | ken-watanabe in plant-maintena | None |
| cm12-ken-plant | member_organization | acme-plant | None |
| cm12-ken-plant | community_organization | acme-plant | None |
| cm12-ken-riverbend | name | ken-watanabe in riverbend-pres | None |
| cm12-ken-riverbend | member_organization | acme-plant | None |
| cm12-ken-riverbend | community_organization | acme-plant | None |
| cm12-lin-plant | name | lin-zhao in plant-maintenance- | None |
| cm12-lin-plant | member_organization | acme-plant | None |
| cm12-lin-plant | community_organization | acme-plant | None |
| cm12-lin-portal | name | lin-zhao in km-portal-communit | None |
| ... | ... | (10 more) | ... |

### source_relationships

- Fields: 6/12 (50.0%)
- Computed columns: name, is_unnegotiated_power_gap, is_extractive_relationship

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sr12-ines-grace | name | ines-moreau with grace-hollowa | None |
| sr12-ines-grace | is_extractive_relationship | True | None |
| sr12-sam-ken | name | sam-adeyemi with ken-watanabe | None |
| sr12-sam-ken | is_unnegotiated_power_gap | True | None |
| sr12-sam-lin | name | sam-adeyemi with lin-zhao | None |
| sr12-sam-tomas | name | sam-adeyemi with tomas-reyes | None |

### department_process_accounts

- Fields: 9/25 (36.0%)
- Computed columns: name, conflicting_department, is_awaiting_engagement, is_conflicting_account, is_unresolved_disagreement

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dpa12-deploy-compliance | name | acme-corp on production-deploy | None |
| dpa12-deploy-compliance | is_awaiting_engagement | True | None |
| dpa12-deploy-engineering | name | acme-engineering on production | None |
| dpa12-deploy-engineering | conflicting_department | acme-customer-support | None |
| dpa12-deploy-engineering | is_conflicting_account | True | None |
| dpa12-deploy-engineering | is_unresolved_disagreement | True | None |
| dpa12-deploy-support | name | acme-customer-support on produ | None |
| dpa12-deploy-support | conflicting_department | acme-engineering | None |
| dpa12-deploy-support | is_conflicting_account | True | None |
| dpa12-deploy-support | is_unresolved_disagreement | True | None |
| dpa12-loto-maintenance | name | acme-plant on lockout-tagout | None |
| dpa12-loto-maintenance | conflicting_department | acme-plant-production | None |
| dpa12-loto-maintenance | is_conflicting_account | True | None |
| dpa12-loto-production | name | acme-plant-production on locko | None |
| dpa12-loto-production | conflicting_department | acme-plant | None |
| dpa12-loto-production | is_conflicting_account | True | None |

### problem_occurrences

- Fields: 30/60 (50.0%)
- Computed columns: name, solver_is_still_engaged, is_solved, prior_was_solved, prior_solution_entry, prior_solver, prior_solver_is_still_engaged, is_solved_without_recorded_solution, has_been_solved_before, has_consultable_specialist, is_relearned_solved_problem, is_turnover_regression

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| po12-bleed-2026-02 | name | Press 7 accumulator bleed stal | None |
| po12-bleed-2026-02 | solver_is_still_engaged | True | None |
| po12-bleed-2026-02 | is_solved | True | None |
| po12-bleed-2026-06 | name | Press 7 accumulator bleed stal | None |
| po12-bleed-2026-06 | solver_is_still_engaged | True | None |
| po12-bleed-2026-06 | is_solved | True | None |
| po12-bleed-2026-06 | prior_was_solved | True | None |
| po12-bleed-2026-06 | prior_solution_entry | kre12-press7-bleed-fix | None |
| po12-bleed-2026-06 | prior_solver | tomas-reyes | None |
| po12-bleed-2026-06 | prior_solver_is_still_engaged | True | None |
| po12-bleed-2026-06 | has_been_solved_before | True | None |
| po12-bleed-2026-06 | has_consultable_specialist | True | None |
| po12-cache-2025-03 | name | Stale CDN assets after a deplo | None |
| po12-cache-2025-03 | is_solved | True | None |
| po12-cache-2025-03 | is_solved_without_recorded_solution | True | None |
| po12-cache-2026-05 | name | Stale CDN assets after a deplo | None |
| po12-cache-2026-05 | solver_is_still_engaged | True | None |
| po12-cache-2026-05 | is_solved | True | None |
| po12-cache-2026-05 | prior_was_solved | True | None |
| po12-cache-2026-05 | prior_solver | anil-kapoor | None |
| ... | ... | (10 more) | ... |

### onboarding_records

- Fields: 19/63 (30.2%)
- Computed columns: name, as_of_instant, is_proficient, days_to_proficiency, days_since_start, is_recent_start, procedure_repository_entry_count, procedure_departed_only_count, is_starting_from_nothing

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ob12-aisha-loto | name | aisha-bello on lockout-tagout | None |
| ob12-aisha-loto | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ob12-aisha-loto | is_proficient | True | None |
| ob12-aisha-loto | days_to_proficiency | 21 | None |
| ob12-aisha-loto | days_since_start | 48 | None |
| ob12-aisha-loto | is_recent_start | True | None |
| ob12-aisha-loto | procedure_repository_entry_count | 3 | None |
| ob12-bea-loto | name | bea-okonkwo on lockout-tagout | None |
| ob12-bea-loto | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ob12-bea-loto | days_since_start | 18 | None |
| ob12-bea-loto | is_recent_start | True | None |
| ob12-bea-loto | procedure_repository_entry_count | 3 | None |
| ob12-carlos-loto | name | carlos-mendez on lockout-tagou | None |
| ob12-carlos-loto | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ob12-carlos-loto | days_since_start | 65 | None |
| ob12-carlos-loto | is_recent_start | True | None |
| ob12-carlos-loto | procedure_repository_entry_count | 3 | None |
| ob12-ken-loto | name | ken-watanabe on lockout-tagout | None |
| ob12-ken-loto | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ob12-ken-loto | is_proficient | True | None |
| ... | ... | (24 more) | ... |

### sharing_recognitions

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rec12-joao-cluster | name | PeerAward: joao-ferreira | None |
| rec12-tomas-teaching | name | TeachingAward: tomas-reyes | None |

### capability_declines

- Fields: 6/16 (37.5%)
- Computed columns: name, preceding_stage, preceding_decline_started_at, follows_preceding_stage_decline

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cd12-eng-training | name | acme-engineering: EngineerTrai | None |
| cd12-plant-facility | name | acme-plant: FacilityInvestment | None |
| cd12-plant-training | name | acme-plant: EngineerTraining | None |
| cd12-plant-training | preceding_stage | FacilityInvestment | None |
| cd12-plant-training | preceding_decline_started_at | 2011-01-01T00:00:00-06:00 | None |
| cd12-plant-training | follows_preceding_stage_decline | True | None |
| cd12-plant-upkeep | name | acme-plant: KnowHowUpkeep | None |
| cd12-plant-upkeep | preceding_stage | EngineerTraining | None |
| cd12-plant-upkeep | preceding_decline_started_at | 2015-01-01T00:00:00-06:00 | None |
| cd12-plant-upkeep | follows_preceding_stage_decline | True | None |

### knowledge_traces

- Fields: 215/360 (59.7%)
- Computed columns: name, source_material_kind, source_collected_at, source_revised_at, source_is_document, source_is_people_capture, source_is_practice_evidence, is_source_changed_since_taken, modeled_duration_minutes, is_unfaithful_to_source, derived_by_agent_kind, is_machine_derived, is_self_validated, has_incomplete_provenance, provenance_statement, is_aspect_unsupported_by_source_kind, is_document_origin, step_elicited_validation_count, step_elicited_extension_count, is_document_start_never_validated, is_document_start_never_extended, contradicted_document_revised_at, is_document_trailing_practice, prescribed_versus_enacted

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kt13-deploy-01 | name | ActivityDefinition <- csm-depl | None |
| kt13-deploy-01 | source_material_kind | DocumentExcerpt | None |
| kt13-deploy-01 | source_collected_at | 2025-11-01T10:00:00-05:00 | None |
| kt13-deploy-01 | source_revised_at | 2026-01-02T09:00:00-05:00 | None |
| kt13-deploy-01 | source_is_document | True | None |
| kt13-deploy-01 | is_source_changed_since_taken | True | None |
| kt13-deploy-01 | modeled_duration_minutes | 20 | None |
| kt13-deploy-01 | derived_by_agent_kind | Human | None |
| kt13-deploy-01 | provenance_statement | From csm-deploy-runbook-excerp | None |
| kt13-deploy-01 | is_document_origin | True | None |
| kt13-deploy-01 | is_document_start_never_validated | True | None |
| kt13-deploy-01 | is_document_start_never_extended | True | None |
| kt13-deploy-02 | name | ActivityDefinition <- csm-depl | None |
| kt13-deploy-02 | source_material_kind | Transcript | None |
| kt13-deploy-02 | source_collected_at | 2025-11-20T10:00:00-06:00 | None |
| kt13-deploy-02 | source_is_people_capture | True | None |
| kt13-deploy-02 | modeled_duration_minutes | 2 | None |
| kt13-deploy-02 | derived_by_agent_kind | AIAgent | None |
| kt13-deploy-02 | is_machine_derived | True | None |
| kt13-deploy-02 | has_incomplete_provenance | True | None |
| ... | ... | (125 more) | ... |

### mined_flow_edges

- Fields: 16/36 (44.4%)
- Computed columns: name, documented_transition_count, is_undocumented_path, to_step_expected_minutes, is_bottleneck, is_mined_path_recorded_as_intent_without_decision

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| mfe13-deploy-01-02 | name | deploy-01 -> deploy-02 | None |
| mfe13-deploy-01-02 | documented_transition_count | 1 | None |
| mfe13-deploy-01-02 | to_step_expected_minutes | 2 | None |
| mfe13-deploy-02-03 | name | deploy-02 -> deploy-03 | None |
| mfe13-deploy-02-03 | documented_transition_count | 1 | None |
| mfe13-deploy-02-03 | to_step_expected_minutes | 15 | None |
| mfe13-deploy-02-03 | is_bottleneck | True | None |
| mfe13-deploy-02-04-hotfix | name | deploy-02 -> deploy-04 | None |
| mfe13-deploy-02-04-hotfix | is_undocumented_path | True | None |
| mfe13-deploy-02-04-hotfix | to_step_expected_minutes | 30 | None |
| mfe13-deploy-03-04 | name | deploy-03 -> deploy-04 | None |
| mfe13-deploy-03-04 | documented_transition_count | 1 | None |
| mfe13-deploy-03-04 | to_step_expected_minutes | 30 | None |
| mfe13-deploy-04-05 | name | deploy-04 -> deploy-05 | None |
| mfe13-deploy-04-05 | documented_transition_count | 1 | None |
| mfe13-deploy-04-05 | to_step_expected_minutes | 30 | None |
| mfe13-deploy-05-04-retry | name | deploy-05 -> deploy-04 | None |
| mfe13-deploy-05-04-retry | is_undocumented_path | True | None |
| mfe13-deploy-05-04-retry | to_step_expected_minutes | 30 | None |
| mfe13-deploy-05-04-retry | is_mined_path_recorded_as_intent_without_decision | True | None |

### collection_occasions

- Fields: 5/18 (27.8%)
- Computed columns: name, as_of_instant, days_since_held, is_lapsed, captured_material_count, is_held_without_capture

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| co13-close-improvement | name | ImprovementInitiative: Close c | None |
| co13-close-improvement | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| co13-close-improvement | days_since_held | 126 | None |
| co13-close-improvement | is_lapsed | True | None |
| co13-close-improvement | is_held_without_capture | True | None |
| co13-deploy-release-retro | name | Retrospective: Monthly release | None |
| co13-deploy-release-retro | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| co13-deploy-release-retro | days_since_held | 1 | None |
| co13-deploy-release-retro | captured_material_count | 1 | None |
| co13-loto-quarterly-review | name | ProcessReview: Quarterly locko | None |
| co13-loto-quarterly-review | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| co13-loto-quarterly-review | days_since_held | 19 | None |
| co13-loto-quarterly-review | captured_material_count | 1 | None |

### stakeholder_perspectives

- Fields: 9/25 (36.0%)
- Computed columns: name, source_material_kind, conflict_partner_count, is_in_conflict, is_dissenting_view_not_kept_with_source

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sp13-deploy03-audit | name | internal-compliance-auditor on | None |
| sp13-deploy03-audit | source_material_kind | DocumentExcerpt | None |
| sp13-deploy03-audit | conflict_partner_count | 1 | None |
| sp13-deploy03-audit | is_in_conflict | True | None |
| sp13-deploy03-release | name | release-manager on deploy-03 | None |
| sp13-deploy03-release | source_material_kind | Transcript | None |
| sp13-deploy03-release | is_in_conflict | True | None |
| sp13-deploy03-release | is_dissenting_view_not_kept_with_source | True | None |
| sp13-deploy04-sre | name | site-reliability-engineer on d | None |
| sp13-loto06-safety | name | plant-safety-officer on loto-0 | None |
| sp13-loto06-safety | source_material_kind | DocumentExcerpt | None |
| sp13-loto06-safety | conflict_partner_count | 1 | None |
| sp13-loto06-safety | is_in_conflict | True | None |
| sp13-loto06-technician | name | senior-maintenance-technician  | None |
| sp13-loto06-technician | source_material_kind | Transcript | None |
| sp13-loto06-technician | is_in_conflict | True | None |

### model_pilots

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pilot-close-q1 | name | gm-quarter-end-close pilot: Q1 | None |
| pilot-loto-north | name | gm-lockout-tagout pilot: North | None |
| pilot-pko-register | name | gm-pko-rulebook pilot: Finance | None |

### model_activity_experts

- Fields: 0/16 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| mae-close-implementation | name | gm-quarter-end-close Implement | None |
| mae-close-publication | name | gm-quarter-end-close Publicati | None |
| mae-close-requirementsspecification | name | gm-quarter-end-close Requireme | None |
| mae-deploy-implementation | name | gm-production-deployment Imple | None |
| mae-loto-implementation | name | gm-lockout-tagout Implementati | None |
| mae-loto-maintenance | name | gm-lockout-tagout Maintenance  | None |
| mae-loto-publication | name | gm-lockout-tagout Publication  | None |
| mae-loto-requirementsspecification | name | gm-lockout-tagout Requirements | None |
| mae-pko-implementation | name | gm-pko-rulebook Implementation | None |
| mae-pko-maintenance | name | gm-pko-rulebook Maintenance de | None |
| mae-pko-publication | name | gm-pko-rulebook Publication de | None |
| mae-pko-requirementsspecification | name | gm-pko-rulebook RequirementsSp | None |
| mae-policy-implementation | name | gm-workforce-policy Implementa | None |
| mae-policy-maintenance | name | gm-workforce-policy Maintenanc | None |
| mae-policy-publication | name | gm-workforce-policy Publicatio | None |
| mae-policy-requirementsspecification | name | gm-workforce-policy Requiremen | None |

### model_data_mapping_runs

- Fields: 0/4 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| map-close-2026-07 | name | gm-quarter-end-close mapping R | None |
| map-loto-2026-07 | name | gm-lockout-tagout mapping Real | None |
| map-pko-2026-07 | name | gm-pko-rulebook mapping Real | None |
| map-policy-synthetic | name | gm-workforce-policy mapping Sy | None |

### artifact_handoffs

- Fields: 11/48 (22.9%)
- Computed columns: name, declared_source_step, declared_consumer_step, disagrees_with_declared_variable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ah-deploy02-candidate | name | deploy-01 -> deploy-02 | None |
| ah-deploy02-candidate | declared_source_step | deploy-01 | None |
| ah-deploy02-candidate | declared_consumer_step | deploy-02 | None |
| ah-deploy03-risk | name | deploy-02 -> deploy-03 | None |
| ah-deploy03-risk | declared_source_step | deploy-02 | None |
| ah-deploy03-risk | declared_consumer_step | deploy-03 | None |
| ah-deploy04-approval | name | deploy-03 -> deploy-04 | None |
| ah-deploy04-approval | declared_source_step | deploy-03 | None |
| ah-deploy04-approval | declared_consumer_step | deploy-04 | None |
| ah-deploy04-candidate | name | deploy-01 -> deploy-04 | None |
| ah-deploy04-candidate | declared_source_step | deploy-01 | None |
| ah-deploy04-candidate | declared_consumer_step | deploy-04 | None |
| ah-deploy05-deployment | name | deploy-04 -> deploy-05 | None |
| ah-deploy05-deployment | declared_source_step | deploy-04 | None |
| ah-deploy05-deployment | declared_consumer_step | deploy-05 | None |
| ah-deploy05-deployment-misrecorded | name | deploy-03 -> deploy-05 | None |
| ah-deploy05-deployment-misrecorded | declared_source_step | deploy-04 | None |
| ah-deploy05-deployment-misrecorded | declared_consumer_step | deploy-05 | None |
| ah-deploy05-deployment-misrecorded | disagrees_with_declared_variable | True | None |
| ah-deploy05-risk | name | deploy-02 -> deploy-05 | None |
| ... | ... | (17 more) | ... |

### app_actions

- Fields: 61/160 (38.1%)
- Computed columns: name, policy_command, policy_denial_test_count, watched_field_is_witness, input_field_count, is_unpermitted, policy_command_disagrees, is_unproven_write

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| act-admin-move-instant | name | Move the date | None |
| act-admin-move-instant | policy_command | UPDATE | None |
| act-admin-move-instant | watched_field_is_witness | True | None |
| act-admin-move-instant | input_field_count | 1 | None |
| act-admin-move-instant | is_unproven_write | True | None |
| act-authority-record-review | name | Record authority review | None |
| act-authority-record-review | policy_command | UPDATE | None |
| act-authority-record-review | watched_field_is_witness | True | None |
| act-authority-record-review | input_field_count | 1 | None |
| act-authority-record-review | is_unproven_write | True | None |
| act-floor-decide | name | Decide | None |
| act-floor-decide | policy_command | UPDATE | None |
| act-floor-decide | watched_field_is_witness | True | None |
| act-floor-decide | input_field_count | 2 | None |
| act-floor-decide | is_unproven_write | True | None |
| act-ke-add-warning-sign | name | Add a warning sign | None |
| act-ke-add-warning-sign | policy_command | INSERT | None |
| act-ke-add-warning-sign | watched_field_is_witness | True | None |
| act-ke-add-warning-sign | input_field_count | 8 | None |
| act-ke-add-warning-sign | is_unproven_write | True | None |
| ... | ... | (79 more) | ... |

### app_action_fields

- Fields: 100/300 (33.3%)
- Computed columns: name, target_field_type, writes_derived_field

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| act-admin-move-instant.AsOfInstant | name | act-admin-move-instant / Judge | None |
| act-admin-move-instant.AsOfInstant | target_field_type | raw | None |
| act-authority-record-review.AuthorityReviewedAt | name | act-authority-record-review /  | None |
| act-authority-record-review.AuthorityReviewedAt | target_field_type | raw | None |
| act-floor-decide.DecidedAt | name | act-floor-decide / Decided | None |
| act-floor-decide.DecidedAt | target_field_type | raw | None |
| act-floor-decide.Status | name | act-floor-decide / Decision | None |
| act-floor-decide.Status | target_field_type | raw | None |
| act-ke-add-warning-sign.CueKind | name | act-ke-add-warning-sign / Kind | None |
| act-ke-add-warning-sign.CueKind | target_field_type | raw | None |
| act-ke-add-warning-sign.Description | name | act-ke-add-warning-sign / What | None |
| act-ke-add-warning-sign.Description | target_field_type | raw | None |
| act-ke-add-warning-sign.EscalateToRole | name | act-ke-add-warning-sign / Esca | None |
| act-ke-add-warning-sign.EscalateToRole | target_field_type | relationship | None |
| act-ke-add-warning-sign.RequiresEscalation | name | act-ke-add-warning-sign / Must | None |
| act-ke-add-warning-sign.RequiresEscalation | target_field_type | raw | None |
| act-ke-add-warning-sign.SemanticTypeIri | name | act-ke-add-warning-sign / Type | None |
| act-ke-add-warning-sign.SemanticTypeIri | target_field_type | raw | None |
| act-ke-add-warning-sign.SignalsIncompleteStep | name | act-ke-add-warning-sign / Mean | None |
| act-ke-add-warning-sign.SignalsIncompleteStep | target_field_type | raw | None |
| ... | ... | (180 more) | ... |

### abundant_knowledge_gaps

- Fields: 0/6 (0.0%)
- Computed columns: name, representing_table_row_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| curated-to-user-and-use-case | name | Knowledge curated to a user an | None |
| curated-to-user-and-use-case | representing_table_row_count | 15 | None |
| human-judgment | name | Human judgment | None |
| human-judgment | representing_table_row_count | 3 | None |
| undiscovered-tacit-knowledge | name | Tacit knowledge nobody has unc | None |
| undiscovered-tacit-knowledge | representing_table_row_count | 12 | None |

### ontology_support_programmes

- Fields: 4/14 (28.6%)
- Computed columns: name, as_of_instant, supported_profile_count, has_ended, days_until_programme_ends, is_ended_with_no_steward_named, is_ending_soon_with_no_steward_named

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ontocommons | name | Ontology-driven data documenta | None |
| ontocommons | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| ontocommons | has_ended | True | None |
| ontocommons | days_until_programme_ends | -992 | None |
| ontocommons | is_ended_with_no_steward_named | True | None |
| perks | name | Eliciting and Exploiting Proce | None |
| perks | as_of_instant | 2026-07-19T13:00:00-05:00 | None |
| perks | supported_profile_count | 2 | None |
| perks | days_until_programme_ends | 73 | None |
| perks | is_ending_soon_with_no_steward_named | True | None |
