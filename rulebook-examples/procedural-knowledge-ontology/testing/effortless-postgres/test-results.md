# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 273809 |
| Passed | 273803 |
| Failed | 6 |
| Score | 100.0% |
| Duration | 41s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 172087 | 172087 | 100.0% |
| Lookup (INDEX/MATCH) | 78011 | 78017 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 23705 | 23705 | 100.0% |

## Results by Entity

### rulebook_releases

- Fields: 522/522 (100.0%)
- Computed columns: name, prev_major, prev_minor, prev_patch, prev_issued_at, model_current_release, expected_major, expected_minor, expected_patch, is_increment_inconsistent_with_scale, days_since_previous_release, is_long_release_cycle, is_declared_current_release, log_entry_count, logical_change_count, non_additive_change_count, class_removal_or_rename_count, invalidating_domain_range_count, inconsistent_disjointness_count, schema_addition_count, suite_update_count, consumer_count, notified_consumer_count, revalidated_consumer_count, validation_run_count, validation_failure_total, consistent_run_count, cq_run_count, answerable_cq_run_count, regressed_baseline_count, cq_coverage_percent, prev_cq_coverage_percent, prev_cq_run_count, scored_criterion_count, stated_criterion_count, patch_alters_logical_model, minor_is_not_backward_compatible, class_removal_without_major, invalidating_domain_range_without_major, inconsistent_disjointness_without_major, is_breaking_release, breaking_release_with_unrevalidated_consumers, breaking_release_without_migration_plan, is_undocumented_version_decision, is_unannounced_to_dependents, is_release_without_recorded_changes, suite_lags_release, is_untagged_release, published_without_approval, released_despite_failed_validation, released_without_consistency_check, passed_validation_at_release, cq_coverage_declined, has_baseline_regression, released_without_cq_task_test, well_formed_but_requirements_unshown, is_not_scored_against_criteria, is_released_without_licence_or_permanent_id

### ontology_profiles

- Fields: 405/405 (100.0%)
- Computed columns: name, mapping_count, as_of_instant, days_since_last_revision, days_since_major_revision, days_since_dependency_reviewed, recent_deprecation_count, requires_frequent_review, change_rate_profile, is_review_overdue_for_change_rate, prerequisite_adopted_at, skips_adoption_path, namespace_is_http, namespace_dereferences, publishes_following_linked_data_principles

### evaluation_contexts

- Fields: 16/16 (100.0%)
- Computed columns: name, explicit_fragment_count, tacit_fragment_count, implicit_fragment_count, situated_judgment_fragment_count, model_reasoned_answer_count, otherwise_reasoned_answer_count, assistant_answer_count, model_reasoned_failed_answer_count, out_of_order_step_execution_count, in_order_step_execution_count, step_execution_count, early_start_step_execution_count, exact_mapping_count, aligned_mapping_count, extension_mapping_count

### organizations

- Fields: 468/468 (100.0%)
- Computed columns: name, failed_ai_initiative_count, owned_procedure_count, ai_fails_for_lack_of_captured_knowledge, product_delivery_function_count, provider_held_delivery_method_count, is_hollowed_out_firm, audit_finding_count, filled_knowledge_position_count, has_knowledge_findings_without_knowledge_staff, departed_holder_know_how_count, lost_departed_know_how_count, retained_departed_know_how_percent, memory_leaves_with_staff, captured_own_know_how_count, documentation_entry_count, unallocated_documentation_count, transfer_given_count, unallocated_transfer_count, treats_knowledge_work_as_unvalued, person_carried_know_how_count, facility_carried_know_how_count, system_carried_know_how_count, holds_know_how_in_people_plants_and_systems, staged_decline_count, eroded_in_stages

### agents

- Fields: 2695/2695 (100.0%)
- Computed columns: name, count_of_current_role_assignments, is_still_engaged, decision_count, overridden_decision_count, override_rate_percent, is_non_human, boundary_violation_count, is_operating_outside_boundary, draft_decision_count, overridden_draft_count, draft_rewrite_rate_percent, times_named_as_broker, is_recognized_broker, at_risk_reliance_count, has_at_risk_knowledge_reliance, is_organization_agent, answer_count, ai_task_completed_count, ai_task_completion_percent, is_below_task_completion_target, runtime_integration_count, lacks_runtime_knowledge_integration, search_event_count, is_untracked_ai_consumer, accountability_assertion_count, inferred_category_count, category_not_available_as_inference, is_unclassified_agent, is_ai_agent_without_model_version, attributed_artifact_count, has_produced_artifacts, has_artifact_blast_radius, registry_version_match_count, is_ai_agent_not_filled_from_registry, current_accountable_human_count, is_ai_agent_without_accountable_human, community_count, is_boundary_spanner, sna_identification_count, is_unidentified_boundary_spanner, located_know_how_count, required_mentoring_hours_per_week, lacks_time_to_mentor, transfers_given_count, recognition_count, is_unrewarded_sharer, downstream_of_held_steps_count, artifact_blast_radius_step_count

### roles

- Fields: 1188/1188 (100.0%)
- Computed columns: name, current_agent_kind, active_assignment_count, currently_covered_assignment_count, has_no_current_holder, count_of_awaited_decisions, current_assignment_valid_from, is_non_human_held, is_ungoverned_non_human_role, departed_assignment_count, has_lost_a_holder, is_vacated_role, ungrounded_boundary_count, is_governed_by_lapsed_authority, unescalated_refusal_count, unauthorized_enforcement_assignment_count, is_ungoverned_enforcement_role, specialized_role_family, specialization_count, has_specializations, is_senior_variant_not_specialization, organization_type, is_not_housed_in_department, capability_tag_count, compliance_review_tag_count, has_compliance_review_capability, role_mention_count, unresolved_role_mention_count, is_missed_by_phrase_query, current_holder_name, backup_role_holder, has_escalation_backup, has_unfilled_escalation_backup, release_approval_step_count, is_production_release_approver, approval_step_count

### role_assignments

- Fields: 2016/2016 (100.0%)
- Computed columns: name, as_of_instant, is_current, current_agent_key, is_currently_valid, agent_role_key, has_departed, covers_now, role_when_covering, agent_kind, is_non_human_assignment, predecessor_agent_kind, is_human_to_non_human_handover, is_unauthorized_non_human_assignment, was_authorized_by_change_request, decision_count, overridden_decision_count, override_rate_percent, predecessor_override_rate_percent, quality_regressed_vs_predecessor, departed_role_key, predecessor_decision_count, has_sufficient_sample, predecessor_has_sufficient_sample, comparison_is_evidentially_sound, single_override_swing_percent, quality_verdict_is_unsupported, is_unmeasured_automation_handover, error_correction_count, error_rate_percent, has_dated_authorization, days_since_authorization_review, authorization_is_overdue_for_review, is_standing_unreviewed_automation, is_unconditioned_automation_handover, exceeds_tolerable_error_rate, boundary_violation_count_for_assignment, has_any_boundary_violation, has_ungrounded_governing_boundary, suspension_condition_met, is_operating_under_met_suspension_condition, has_declared_suspension_condition, has_approving_authority, has_authorizing_change_request, is_unauthorized_enforcement_agent, governance_evidence_count, unauthorized_enforcement_role_key, scoped_version_status, is_scoped_to_retired_version, predecessor_valid_to, predecessor_lacks_validity_end, agent_version_key, agent_role_pair_key, is_open_ended, role_approval_step_count, receives_approval_notices_now

### communities_of_practice

- Fields: 119/119 (100.0%)
- Computed columns: name, has_own_vocabulary_and_norms, sharing_event_count, is_mandated_without_sharing_norm, interconnected_know_how_count, physical_know_how_count, digital_know_how_count, person_carried_know_how_count, spans_physical_and_digital_with_humans, external_member_count, specialist_member_count, employer_move_count, is_cross_firm_practice_cluster, recent_apprenticeship_count, is_circulation_ending_for_lack_of_apprentices, ambient_absorption_count, has_ambient_trade_know_how

### mentorships

- Fields: 30/30 (100.0%)
- Computed columns: name, as_of_instant, is_active, days_since_started, is_recent_apprenticeship, community_label

### procedure_types

- Fields: 147/153 (96.1%)
- Computed columns: name, narrower_type_count, has_narrower_types, is_detached_from_taxonomy, broader_is_detached, is_unreachable_by_navigation, member_count, members_lacking_distinction_count, is_arbitrary_grouping

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| domain-customer-service | broader_is_detached | True | None |
| domain-finance | broader_is_detached | True | None |
| domain-human-resources | broader_is_detached | True | None |
| domain-operations | broader_is_detached | True | None |
| domain-technology | broader_is_detached | True | None |
| group-metrology | broader_is_detached | True | None |

### procedures

- Fields: 660/660 (100.0%)
- Computed columns: name, execution_count, is_template_instance, target_count, adoption_count, specified_step_total, has_no_explicit_steps, called_by_step_count, is_nested_procedure, failure_criterion_count, has_no_failure_criterion, lens_view_count, privileges_single_stakeholder_view, step_level_view_count, category_level_view_count, procedure_type_rank, procedure_type_definition, has_step_and_category_resolutions, is_missing_demanded_resolution, is_inconsistently_categorized, strategic_alignment_count, has_no_stated_reason_for_existing, outcome_measure_count, unlinked_measure_count, measures_performance_without_business_link, hindered_by_count, is_hindered_by_another_operation, type_distinguishing_facet, type_distinguishing_value, matching_distinction_count, lacks_type_distinction, lacks_distinction_type_key, managed_vocabulary_count, lacks_managed_controlled_vocabulary, compliance_review_step_count, involves_compliance_review_role, is_regulated_but_unformalized, agent_intended_count, is_tacit_only_agent_target, repository_entry_count, stale_entry_count, departed_only_know_how_count, has_decayed_transfer_channel, execution_feedback_entry_count, is_executed_without_feedback_loop, machine_authored_entry_count, practitioner_relationship_count, is_captured_by_automation_alone, unengaged_stakeholder_count, has_unengaged_stakeholder, recent_starter_count, untransferred_veteran_know_how_count, departing_veteran_know_how_count, has_transfer_shortfall_exposure, proficient_with_capture_count, proficient_without_capture_count, days_with_capture_total, days_without_capture_total, avg_days_to_proficiency_with_capture, avg_days_to_proficiency_without_capture, formalization_does_not_ease_onboarding, collected_material_count, in_work_capture_count, is_capture_separate_from_work, expert_acquisition_hours, compliance_document_count

### procedure_versions

- Fields: 1611/1611 (100.0%)
- Computed columns: name, count_of_steps, count_of_open_knowledge_gaps, is_ready_for_execution, specified_step_count, overdue_review_count, open_change_request_count, open_high_severity_gap_count, is_fit_to_execute, steward_review_cadence_days, count_of_stewardship_assignments, has_any_steward, is_live, is_unstewarded, is_live_and_unstewarded, count_of_open_blocking_gaps, has_open_blocking_gap, is_live_with_blocking_gap, should_not_be_executable, count_of_unapproved_reliance_fragments, runs_on_unapproved_knowledge, count_of_overdue_gaps, count_of_change_requests, count_of_review_events, has_governance_record, as_of_instant, days_since_modified, days_since_last_review, was_modified_since_last_review, modifier_is_authority, has_unwitnessed_change, count_of_stale_fragments, knowledge_is_staler_than_cadence, compound_fragile_fragment_count, rests_on_compound_fragile_knowledge, concentrated_witness_session_count, knowledge_base_is_concentrated, machine_consumed_unapproved_count, feeds_unapproved_knowledge_to_machines, genuinely_overdue_fragment_count, awaited_decision_count, scoped_open_blocking_gap_count, is_blocked_on_pending_decision, unexercised_human_gate_count, ai_boundary_is_unevidenced, load_bearing_unapproved_count, unlanded_decision_count, unrehearsed_control_entry_count, has_unrehearsed_control_entry, is_live_with_unrehearsed_control, cadence_breach_count, is_in_cadence_breach, has_decision_in_flight, is_unremediated_cadence_breach, is_managed_cadence_breach, governance_is_silent, valid_fragment_count, still_owns_valid_knowledge, incoming_supersession_count, is_still_referenced, is_load_bearing_orphan, is_cleanly_retired, stalled_implementation_count, is_held_unfit_by_landed_decisions, undeclared_control_kind_count, control_taxonomy_is_incomplete, has_approved_change_request, approved_change_request_count, unwatched_unowned_control_count, mining_run_count, drifted_mining_run_count, has_unresolved_mining_drift, entry_step_id, execution_count, status_is_pko, uses_non_pko_status, exception_count, fallback_transition_count, alternative_transition_count, has_no_exception_handling, latest_source_document_modified_at, days_document_trails_version, document_lags_practice, human_step_count, non_human_step_count, mixes_human_and_software_steps, day_run_count, night_run_count, day_deviating_run_count, night_deviating_run_count, is_inconsistent_across_shifts, overlaps_relation_count, enables_relation_count, prevents_relation_count, steps_without_ontology_type_count, rests_on_notation_only, is_current_without_motivation, conditionless_step_count, is_under_specified_for_execution, coarse_top_level_step_count, fine_top_level_step_count, mixes_granularity_at_one_level, procedure_type_of_version, created_by_agent_kind, elicitation_session_count, expert_capture_count, elicitation_evidence_count, indexed_segment_count, search_count, successful_search_count, search_success_percent, open_question_annotation_count, is_inadequate_for_use, served_assertion_count, lacks_machine_interpretable_encoding, structured_query_count, profile_validated_submission_count, reasoned_assertion_count, is_not_query_validate_reason_ready, published_projection_count, consumer_sync_count, is_unreachable_knowledge, human_sync_count, machine_sync_count, human_channel_count, machine_channel_count, serves_only_humans_or_only_machines, fresh_mining_run_count, lacks_continuous_drift_detection, outcome_measurement_count, is_disconnected_from_outcomes, ai_contribution_count, ai_consumption_count, uses_ai_in_one_direction_only, is_unmodified_for_twelve_months, design_decision_count, is_live_without_recorded_decisions, ai_artifact_consuming_input_count, contains_steps_affected_by_ai_agent_change, interview_session_count, observation_session_count, workshop_session_count, protocol_session_count, incident_session_count, reconciled_divergence_count, complementary_method_count, relies_on_single_method, misses_a_required_elicitation_mode, critical_incident_count, judgment_unprobed_by_incidents, sme_approval_count, sme_ai_evaluation_count, is_approved_without_sme_signoff, experts_evaluate_ai_not_representation, ke_session_count, ke_field_session_count, is_studied_only_from_the_desk, judgment_held_outside_sop_count, tacit_holding_count, explicit_holding_count, tacit_share_exceeds_explicit, hands_held_count, negotiated_practice_count, lives_in_hands_silence_and_negotiation, process_model_trace_count, is_live_model_untraced, trailing_practice_trace_count, is_documented_behind_practice, tacit_fragment_count, is_standardized_without_tacit_capture, tacit_form_fragment_count, situated_judgment_fragment_count, tacit_judgment_fragment_count, first_step, fallback_step, first_step_disagrees_with_graph, declared_first_step_count, graph_entry_step_count, owner_organization

### procedure_version_links

- Fields: 4/4 (100.0%)
- Computed columns: name, superseded_version_key

### procedure_status_changes

- Fields: 39/39 (100.0%)
- Computed columns: name, is_unattributed_change, change_kind_contradicts_target

### steps

- Fields: 4469/4469 (100.0%)
- Computed columns: name, assigned_role_label, assigned_agent_kind, blocking_requirement_count, stale_binding_count, authoritative_stale_count, available_exception_count, declared_verification_count, is_preparation_step, is_approval_step, stale_authoritative_binding_count, inputs_are_fresh, is_software_assigned, is_human_approval_gate, gate_held_by_human, binding_boundary_count, assigned_role_is_ungoverned, unusable_binding_count, all_sources_usable, unwarranted_boundary_count, is_governed_by_unwarranted_boundary, software_execution_count, has_been_approached_by_software, is_unexercised_human_gate, is_demonstrated_human_gate, unexercised_gate_version_key, has_declared_control_kind, undeclared_control_version_key, approval_step_is_software_assigned, unwitnessed_blocking_count, reachable_step_count, reached_from_step_count, self_reach_count, is_on_rework_loop, is_blocking_control_on_rework_loop, incoming_transition_count, is_entry_step, entry_step_key, version_entry_step_id, gate_free_reach_from_entry_count, is_reachable_from_entry_without_human_gate, is_gate_bypassed_publication, child_step_count, parent_step_kind, is_composite_without_children, precondition_count, postcondition_count, invariant_count, safety_critical_condition_count, failure_mode_count, cue_count, danger_cue_count, decision_point_count, knowledge_fragment_count, has_instruction_only, input_variable_count, output_variable_count, required_lock_count, required_protective_equipment_count, is_isolation_without_lock, referenced_resource_count, untyped_version_key, is_accountable_to_software, has_downstream_steps, incompleteness_cue_count, has_incompleteness_cue, has_postcondition, accountable_agent, has_no_accountable_agent, conditionless_version_key, prerequisite_downstream_count, prerequisite_is_downstream, states_operational_knowledge, bottleneck_allocation_count, is_bottleneck_step, coarse_top_level_version_key, fine_top_level_version_key, context_sensitivity_count, unscoped_sensitivity_count, is_context_sensitive_but_unscoped, version_procedure, assigned_role_does_compliance_review, compliance_review_procedure_key, step_procedure_type, is_release_approval_gate, regulatory_requirement_count, tool_function_count, parseable_condition_count, dmn_decision_count, tool_use_rules_only_in_prose, open_outdated_flag_count, has_reported_reality_mismatch, deviated_run_count, is_drifted_from_practice, ai_failure_count, is_ai_failure_point, ai_artifact_input_count, consumes_ai_agent_artifact, collection_evidence_count, has_collection_evidence, activity_origin_trace_count, version_model_trace_count, is_untraced_activity_in_traced_model, elicited_validation_count, elicited_extension_count, downstream_artifact_step_count, declared_first_step_key, declared_fallback_step_key, owner_organization

### step_transitions

- Fields: 860/860 (100.0%)
- Computed columns: name, is_recovery_path, count_of_from_step_executions, count_of_to_step_executions, has_reachable_origin, has_reachable_target, is_never_exercised, is_untested_recovery_path, count_of_observed_traversals, has_been_traversed, is_unwalked_recovery_path, target_blocking_requirement_count, target_carries_blocking_control, is_unrehearsed_control_entry, unrehearsed_control_version_key, from_step_is_human_approval_gate, to_step_is_human_approval_gate, avoids_human_approval_gate, decision_point_count, is_undocumented_branch

### actions

- Fields: 10/10 (100.0%)
- Computed columns: name

### functions

- Fields: 11/11 (100.0%)
- Computed columns: name

### tools

- Fields: 9/9 (100.0%)
- Computed columns: name

### step_actions

- Fields: 10/10 (100.0%)
- Computed columns: name

### step_functions

- Fields: 10/10 (100.0%)
- Computed columns: name

### step_tools

- Fields: 11/11 (100.0%)
- Computed columns: name

### requirements

- Fields: 544/544 (100.0%)
- Computed columns: name, satisfaction_record_count, step_binding_count, is_bound_to_any_step, has_ever_been_evaluated, negative_outcome_count, is_inoperative_control, is_decorative_control, has_ever_produced_negative, is_unfalsified_control, claims_a_witness_field, named_witness_field_exists, derived_has_computed_witness, witness_claim_is_unverified, is_unwitnessed_blocking_control, witness_fire_count, witness_has_never_fired, evaluation_sample_size, has_meaningful_sample, is_untested_witness, is_evidenced_holding_control, control_assurance_state, unexercised_binding_count, witness_is_partially_scoped, accountable_agent, has_named_owner, is_orphaned_blocking_control, is_unwatched_and_unowned, attestation_exposure_note, unwatched_unowned_flag, uses_controlled_vocabulary, is_regulatory_requirement, constraint_trace_count, is_untraced_bound_constraint

### step_requirements

- Fields: 198/198 (100.0%)
- Computed columns: name, requirement_is_blocking, blocking_step_key, step_when_blocking, requirement_lacks_witness, unwitnessed_step_key, satisfaction_count_for_binding, binding_was_ever_exercised, is_unexercised_blocking_binding, unexercised_binding_requirement_key, requirement_is_regulatory

### step_verifications

- Fields: 11/11 (100.0%)
- Computed columns: name

### rationales

- Fields: 4/4 (100.0%)
- Computed columns: name

### exceptions

- Fields: 8/8 (100.0%)
- Computed columns: name, active_exception_step_key

### resources

- Fields: 190/190 (100.0%)
- Computed columns: name, is_approved_source, source_modified_at, is_stale_extraction, referencing_step_count, referencing_version_count, is_unused_resource, is_content_without_organization, trailing_practice_count, is_behind_current_practice

### procedure_resources

- Fields: 36/36 (100.0%)
- Computed columns: name, relation_iri, resource_modified_at

### elicitation_sessions

- Fields: 377/377 (100.0%)
- Computed columns: name, as_of_instant, days_since_elicited, is_single_witness_method, practitioner_is_still_engaged, valid_fragments_produced, is_high_yield_session, is_concentrated_single_witness, is_stale_concentrated_witness, concentrated_session_version_key, elicitation_mode, is_interview, is_workshop, why_probe_count, shortfall_probe_count, is_interview_without_why_probe, is_interview_without_shortfall_probe, initiator_count, executor_count, dependent_count, uninvited_participant_count, gathers_whole_process_chain, is_workshop_without_usual_outsiders, method_family, facilitator_knowledge_engineer_role_count, facilitator_is_knowledge_engineer, is_generic_or_unskilled_capture, is_ke_field_session, is_reviewed_recording

### knowledge_fragments

- Fields: 828/828 (100.0%)
- Computed columns: name, as_of_instant, is_currently_valid, source_agent_is_still_engaged, source_agent_kind, has_human_source, has_orphaned_provenance, is_undefendable_tacit_claim, is_approved, is_within_validity_window, is_relied_upon, step_procedure_version_status, is_attached_to_live_version, is_unapproved_but_relied_on, evidence_age_days, has_recorded_elicitation, is_from_single_witness, evidence_expiry_days, evidence_has_expired, owner_agent, is_awaiting_approval, owner_is_me, is_my_unfinished_approval, is_invoked_by_an_exception, has_operational_reliance, is_unapproved_and_operationally_live, age_days, is_low_confidence, owning_version_cadence_days, exceeds_owning_cadence, is_aging_low_confidence_claim, owner_role_agent_kind, is_human_owned, is_ai_validated_by_ai, review_cadence_days, is_overdue_for_review, predates_current_role_holder, owner_role_assignment_valid_from, fragility_signal_count, is_compound_fragile, is_single_point_of_failure, is_expiring_single_point_of_failure, compound_fragile_version_key, valid_fragment_session_key, consuming_step_is_software_assigned, consuming_step_agent_kind, is_unapproved_and_machine_consumed, is_unapproved_and_human_consumed, machine_consumed_unapproved_version_key, has_review_record, days_since_actual_review, is_unreviewed_since_authoring, is_genuinely_overdue, review_recency_is_inferred, inference_disagrees_with_record, genuinely_overdue_version_key, ratified_boundary_count, reliance_surface_count, days_awaiting_my_approval, is_high_blast_radius_unapproved, is_long_unapproved, unapproved_load_bearing_version_key, owner_role_is_vacated, is_orphaned_by_role, valid_fragment_version_key, is_flattened_to_brittle_rule, corroboration_count, rests_on_single_data_point, owner_organization

### knowledge_gaps

- Fields: 345/345 (100.0%)
- Computed columns: name, is_open, open_gap_version_key, is_blocking, is_open_and_blocking, as_of_instant, days_open, tolerance_days, is_overdue_gap, owner_agent, owner_is_still_engaged, has_resolution_plan, is_abandoned_unknown, open_blocking_gap_version_key, owner_role_is_vacated, is_ownerless_open_gap, is_known_and_unresolved, is_gatekeeping_or_sabotage, is_unattributed_gatekeeping, is_required_gatekept_uncodified, owner_organization, answering_change_title, answering_change_status

### fa_qs

- Fields: 12/12 (100.0%)
- Computed columns: name, resolution_count, is_unused_faq

### explanations

- Fields: 2/2 (100.0%)
- Computed columns: name

### procedure_executions

- Fields: 870/870 (100.0%)
- Computed columns: name, expected_step_count, completed_step_count, control_breach_count, late_step_count, is_structurally_complete, diverged_from_specification, all_blocking_controls_evaluated, unevaluated_blocking_total, separation_of_duties_held, separation_violation_count, is_attestation_ready, attestation_blocker_summary, executed_version_is_fit, signed_against_unfit_version, asserted_only_control_count, assurance_is_mostly_asserted, unreachable_handling_failure_count, retention_breach_count, cleared_legal_review_count, has_cleared_legal_review, abandoned_failure_count, delivered_count, total_delivery_attempt_count, has_abandoned_failures, mishandled_refusal_count, unclean_step_count, ran_clean, count_of_approval_executions, has_human_approval, count_of_delivery_executions, has_delivered, delivered_without_approval, invalid_approval_count, approval_chain_is_complete, vacuously_clean_step_count, preparation_step_count, approval_step_count, separation_was_testable, separation_held_under_test, separation_is_vacuously_green, separation_assurance_note, ungoverned_divergence_count, divergence_was_fully_governed, computedly_witnessed_control_count, evaluated_control_count, computed_assurance_ratio, interested_party_assertion_count, assurance_grade, attestation_would_be_weakly_based, independent_human_observation_count, has_any_independent_observation, self_attested_approval_count, assurance_chain_is_circular, latest_attestation_instant, has_been_attested, attestation_count, post_attestation_score_count, basis_changed_after_signature, requires_re_attestation, intended_recipient_count, reached_recipient_count, silently_dropped_count, delivery_yield_percent, campaign_silently_lost_audience, unrecorded_refusal_count, has_unrecorded_refusals, independently_confirmed_intent_count, send_decisions_are_entirely_self_witnessed, participant_count, deviating_step_count, has_step_deviation, deviating_facility_key, clean_facility_key, deviating_day_version_key, deviating_night_version_key, status_change_count, claims_completion_without_all_steps, is_unconfirmed_completion, has_no_recorded_outcome, feedback_count, is_unreported_mistake, owner_organization, stopped_at_gap_statement, stopped_at_gap_status, stopped_at_gap_change_title, stopped_at_gap_change_status

### step_executions

- Fields: 8820/8820 (100.0%)
- Computed columns: name, actual_duration_minutes, expected_duration_minutes, is_late, blocking_unmet_count, blocking_unmet_count_safe, proceeded_past_blocking_control, expected_blocking_count, evaluated_blocking_count, unevaluated_blocking_count, has_unevaluated_blocking_control, stale_authoritative_source_count, ran_on_stale_authoritative_source, has_deviation_note, is_late_and_unexplained, available_exception_count_for_step, had_uninvoked_exception_available, expected_verification_count, performed_verification_count, skipped_verification_count, has_skipped_verification, claims_pass_without_evidence, step_is_preparation, step_is_approval, preparer_agent_key, approver_agent_key, prepared_by_this_agent_count, violates_separation_of_duties, required_role_for_step, executor_role_key, executor_authority_count, executor_held_required_role, is_unauthorized_approval, completed_execution_key, control_breach_execution_key, late_execution_key, executor_agent_kind, executor_is_human, step_requires_human_confirmation, non_human_ran_human_step, non_human_approval, unevaluated_blocking_execution_key, separation_violation_execution_key, self_witnessed_verification_count, unbacked_verification_count, approval_rests_on_self_attestation, exception_invocation_count, ran_under_exception, is_completed, is_verification_passed, is_legal_review_step, cleared_legal_review_key, assigned_role, role_current_agent, executor_is_designated_agent, inputs_were_fresh_at_run, ran_on_stale_inputs, unresolved_issue_count, has_deviation, is_clean, procedure_execution_when_unclean, evaluated_requirement_count, required_blocking_count, has_unevaluated_blocking_requirement, executing_agent_kind, was_executed_by_software, step_is_software_assigned, software_did_human_work, is_approval_execution, is_verified, unconfirmed_non_human_decision_count, requires_human_confirmation, human_confirmation_missing, drafted_from_unusable_source, inputs_were_usable, software_execution_step_key, step_control_kind, unfalsified_clearance_count, all_clearances_are_unfalsified, stale_at_run_count, was_stale_when_i_ran_it, staleness_answer_is_tense_dependent, has_any_declared_check, performed_check_count, declared_check_count, is_unchecked_by_design, is_vacuously_clean, is_substantively_clean, vacuously_clean_execution_key, uncorroborated_pass_count, evidence_position_is_weak, preparation_execution_key, approval_execution_key, has_governing_instrument, has_approved_change_coverage, version_of_step, is_ungoverned_divergence, ungoverned_divergence_execution_key, self_attested_approval_execution_key, previous_executed_step, specified_transition_from_previous_count, is_out_of_specified_order, execution_version, executes_step_of_other_version, repetition_count, step_max_repetitions, exceeds_max_repetitions, lacks_required_confirmation, failed_precondition_count, violated_invariant_count, proceeded_despite_failed_precondition, unescalated_danger_cue_count, ignored_danger_cue, used_entity_count, generated_entity_count, step_input_variable_count, ran_without_declared_inputs, step_prerequisite, completed_prerequisite_run_count, ran_before_prerequisite_completed, deviation_execution_key, broke_invariant, is_blocked_by_incomplete_prerequisite, owner_organization, incomplete_cue_observation_count, is_blocked_by_observed_cue

### requirement_satisfactions

- Fields: 304/304 (100.0%)
- Computed columns: name, requirement_is_blocking, is_fully_satisfied, is_blocking_and_unmet, blocking_unmet_step_key, blocking_satisfaction_step_key, negative_outcome_requirement_key, evaluator_agent_kind, non_human_evaluated_human_control, requirement_has_computed_witness, is_asserted_only, asserted_only_execution_key, parent_procedure_execution, step_execution_when_scored, is_human_evaluated, requirement_is_approval_type, is_invalid_approval, procedure_execution_of_satisfaction, run_when_invalid_approval, requirement_is_unfalsified, is_clearance_by_unfalsified_control, unfalsified_clearance_step_key, spec_step_of_execution, binding_key, scored_step_executor_agent, evaluator_is_step_executor, run_owner_agent, evaluator_owns_the_run, is_interested_party_assertion, has_written_evidence, is_bare_assertion, interested_assertion_execution_key, is_computedly_witnessed, computed_witness_execution_key, step_executor_agent, was_scored_after_attestation, attestation_instant_for_run, post_attestation_score_execution_key

### errors

- Fields: 16/16 (100.0%)
- Computed columns: name, remedy_step_count, occurrence_count, has_no_remedy_step

### issue_occurrences

- Fields: 40/40 (100.0%)
- Computed columns: name, is_unresolved, step_execution_when_unresolved, executed_step, failed_condition_count_on_run, coincided_with_failed_condition, has_no_recorded_solution, redesign_implemented_at, is_failure_without_landed_redesign, improvement_cycle_path

### user_questions

- Fields: 8/8 (100.0%)
- Computed columns: name, is_unaddressed_question

### user_feedback

- Fields: 24/24 (100.0%)
- Computed columns: name, is_unactioned_procedure_critique, collection_follow_up_count, is_tacit_signal_not_fed_into_collection

### stewardship_assignments

- Fields: 10/10 (100.0%)
- Computed columns: name, count_of_review_events, has_ever_been_reviewed, as_of_instant, is_current_assignment

### change_requests

- Fields: 132/132 (100.0%)
- Computed columns: name, is_open, open_change_version_key, is_decided, as_of_instant, days_pending, is_still_pending, is_stalled, authority_agent, requester_is_authority, awaits_authority_decision, authority_role_label, touches_live_version, is_live_decision_backlog, blocks_an_open_gap, backlog_version_key, is_my_pending_decision, is_my_blocking_backlog, is_my_overdue_backlog, is_implemented, is_my_decided_request, is_my_decided_but_unlanded, decision_latency_days, implementation_latency_days, delay_is_downstream_of_me, unlanded_version_key, is_approved_not_implemented, days_since_approval, is_stalled_implementation, stalled_implementation_version_key, approved_version_key, is_approved_decision, owner_organization

### review_events

- Fields: 36/36 (100.0%)
- Computed columns: name, as_of_instant, is_overdue, overdue_version_key, promised_cadence_days, days_since_reviewed, exceeds_promised_cadence, cadence_drift_days, promise_and_behavior_disagree, cadence_breach_version_key, version_modified_at, review_did_not_refresh_modified

### learning_activities

- Fields: 2/2 (100.0%)
- Computed columns: name

### operational_bindings

- Fields: 55/55 (100.0%)
- Computed columns: name, as_of_instant, age_minutes, is_fresh, stale_binding_step_key, authoritative_stale_step_key, is_stale_and_authoritative, step_when_stale, resource_is_approved, is_usable_for_drafting, step_when_unusable

### communication_policies

- Fields: 8/8 (100.0%)
- Computed columns: name, consent_violation_count, quiet_hours_violation_count, is_active_policy

### message_templates

- Fields: 32/32 (100.0%)
- Computed columns: name, policy_max_message_length, policy_max_segments, body_template_length, is_template_over_length, valid_approval_count, has_valid_approval, is_claiming_unbacked_approval, last_approved_body_hash, has_body_drifted, is_sendable_under_approval, drifted_send_count, unanswered_delivery_count, transmitted_delivery_count, template_draws_no_response, last_approval_at

### semantic_mappings

- Fields: 1428/1428 (100.0%)
- Computed columns: name, profile_namespace_dereferences, reinvents_standard_term, is_non_resolvable_term_iri

### witness_loops

- Fields: 48/48 (100.0%)
- Computed columns: name, question_count, is_complete

### role_questions

- Fields: 1326/1326 (100.0%)
- Computed columns: name, predicate_count, is_answered

### rulebook_fields

- Fields: 36295/36295 (100.0%)
- Computed columns: name, is_derived, is_witness, disagreeing_substrate_count, is_substrate_contested, has_measured_data, is_discriminating

### test_suites

- Fields: 30/30 (100.0%)
- Computed columns: name, test_count, pass_count, blocking_fail_count, is_green

### test_cases

- Fields: 12558/12558 (100.0%)
- Computed columns: name, is_blocking, is_passing, is_failing, needs_attention, passing_suite_key, needs_attention_suite_key

### exception_invocations

- Fields: 13/13 (100.0%)
- Computed columns: name, expected_handling, required_approval_role, required_approval_role_holder, approval_role_matches, is_approved, is_improperly_approved, invoker_agent_kind, invoker_also_prepared_key, parent_procedure_execution, approver_prepared_count, delegated_to_preparer, is_ungoverned_invocation

### verification_outcomes

- Fields: 72/72 (100.0%)
- Computed columns: name, expected_signal_value, signal_identifier, signal_matches_expected, has_evidence, is_unbacked_observation, is_self_witnessed, step_executor_agent, self_witnessed_step_key, unbacked_step_key, is_self_witnessed_and_unbacked, is_uncorroborated_pass, uncorroborated_pass_step_key, observer_is_non_human, observer_is_independent_of_executor, is_independent_human_observation, independent_observation_execution_key, parent_procedure_execution_of_outcome

### observed_transitions

- Fields: 10/10 (100.0%)
- Computed columns: name

### recipients

- Fields: 30/30 (100.0%)
- Computed columns: name, has_sms_consent, is_email_reachable, is_sms_reachable, is_unreachable, is_communicationally_stranded

### message_deliveries

- Fields: 444/444 (100.0%)
- Computed columns: name, policy_channel, channel_name, policy_requires_consent, recipient_has_sms_consent, was_actually_transmitted, is_consent_violation, consent_violation_policy_key, policy_quiet_hours_start_hour, policy_quiet_hours_end_hour, policy_has_quiet_hours, quiet_window_wraps_midnight, is_inside_quiet_window, is_quiet_hours_violation, quiet_hours_violation_policy_key, recipient_is_unreachable, is_acknowledged, invoked_exception_condition, has_unreachable_exception_invoked, is_fabricated_acknowledgement, is_unhandled_unreachable, unreachable_failure_key, policy_retention_days, as_of_instant, age_days, is_within_retention_window, has_rendered_body, is_evidence_required, is_retention_breach, retention_breach_execution_key, sending_step_execution_step, execution_has_cleared_legal_review, is_unreviewed_send, rendered_body_length, policy_max_message_length_at_send, segment_count, policy_max_segments_at_send, is_over_segment_limit, template_has_valid_approval, is_unapproved_send, policy_required_opt_out_phrase, policy_requires_opt_out, opt_out_phrase_position, has_opt_out_phrase, is_opt_out_in_first_segment, is_missing_required_opt_out, is_opt_out_at_risk_of_truncation, is_failed_delivery, is_suppressed, is_triaged, is_abandoned_failure, abandoned_failure_execution_key, reached_execution_key, template_was_sendable, is_drifted_send, drifted_send_template_key, was_sent_outside_business_hours, was_delivered_and_unanswered, is_poorly_timed_unanswered, is_well_timed_unanswered, unanswered_template_key, transmitted_template_key, approval_preceded_send, has_frozen_approval_evidence, provenance_is_live_derived, current_last_approval_at, template_reapproved_since_send, is_unprovable_approval_claim, has_sent_reminder, acknowledgement_is_outstanding, outstanding_age_days, is_unchased_acknowledgement, is_exhausted_follow_up, needs_human_escalation

### template_approvals

- Fields: 12/12 (100.0%)
- Computed columns: name, is_approval_decision, template_policy, required_approval_role, is_decided_by_required_role, valid_approval_template_key

### send_intents

- Fields: 553/553 (100.0%)
- Computed columns: name, intent_policy, intent_channel, policy_is_active, intent_requires_consent, recipient_has_channel_consent, consent_gate_passed, recipient_is_sms_reachable, recipient_is_email_reachable, reachability_gate_passed, permission_gate_passed, intent_quiet_start_hour, intent_quiet_end_hour, intent_policy_has_quiet_hours, intent_quiet_window_wraps, intent_is_inside_quiet_window, timing_gate_passed, hours_until_window_opens, intent_max_message_length, intent_max_segments, length_gate_passed, intent_required_opt_out_phrase, opt_out_gate_passed, content_gate_passed, template_is_sendable, execution_has_legal_clearance, intent_approval_role, approval_role_agent_kind, approval_is_human, authorization_gate_passed, is_cleared_to_send, blocking_gate_name, has_resulting_delivery, resulting_delivery_was_transmitted, is_overridden_refusal, is_silently_dropped, resulting_delivery_exception, refusal_cited_an_exception, is_properly_handled_refusal, refusal_failure_execution_key, intent_execution_key, delivered_intent_execution_key, dropped_intent_execution_key, my_approval_was_in_force, refused_on_approved_content, refused_on_opt_out_only, refusal_was_on_my_rules, refusal_was_outside_my_control, is_unreported_refusal_on_my_rules, is_approval_overridden_silently, has_alternate_channel_attempt, alternate_attempt_was_cleared, is_refused_with_no_alternative, exception_prescribed_an_alternative, prescribed_handling_was_performed, is_suppression_without_remedy, has_durable_refusal_record, refusal_was_escalated, is_unrecorded_refusal, is_unescalated_refusal, unescalated_refusal_role_key, unrecorded_refusal_execution_key, was_deferred_on_timing, as_of_instant, window_has_since_reopened, has_retry_attempt, retry_was_cleared, is_abandoned_deferral, deferral_age_hours, is_stale_deferral, enforced_by_unauthorized_agent, consent_input_was_resolvable, recipient_consent_status_raw, policy_input_was_resolvable, all_gate_inputs_resolved, is_unevaluable_refusal, is_self_witnessed_decision, is_independently_confirmed, independently_confirmed_execution_key

### agent_decision_records

- Fields: 81/81 (100.0%)
- Computed columns: name, was_overridden, was_reviewed, deciding_agent_kind, deciding_agent_when_overridden, role_assignment_when_scored, role_assignment_when_overridden, step_of_decision, boundary_match_key, matching_boundary_count, violated_authority_boundary, reviewer_agent_kind, has_human_confirmation, needs_human_confirmation, is_unconfirmed_non_human_decision, step_execution_when_unconfirmed, agent_when_boundary_violated, review_latency_minutes, is_draft_kind, agent_when_draft_overridden, agent_when_draft, is_error_correction, is_reserved_judgment_override, override_reason_is_recorded, is_unexplained_override, error_correction_role_assignment_key, boundary_violation_role_assignment_key

### delivered_communications

- Fields: 6/6 (100.0%)
- Computed columns: name, has_authorization, content_matches_approval, authorized_at, was_approved_before_sending, is_defensible

### authority_boundaries

- Fields: 63/63 (100.0%)
- Computed columns: name, as_of_instant, is_currently_binding, ratifying_fragment_is_valid, step_when_binding, boundary_match_key, violation_count, is_untested, has_ratifying_fragment, is_unwarranted, ratifying_fragment_is_overdue, ratifying_fragment_is_single_witness, warrant_is_thin, is_unwarranted_and_untested, unwarranted_boundary_step_key, ratifying_fragment_key, ratifying_fragment_status, ratification_lapsed, binds_despite_lapsed_ratification, is_ungrounded_and_untested, constrained_role_assignment_key

### app_role_profiles

- Fields: 40/40 (100.0%)
- Computed columns: name, route_count

### app_nav_groups

- Fields: 46/46 (100.0%)
- Computed columns: name, route_count

### app_routes

- Fields: 1043/1043 (100.0%)
- Computed columns: name, is_in_nav, is_shared, is_maintainer, question_count, reference_count, answers_no_question

### app_route_questions

- Fields: 151/151 (100.0%)
- Computed columns: name

### app_route_references

- Fields: 315/315 (100.0%)
- Computed columns: name

### rulebook_tables

- Fields: 4752/4752 (100.0%)
- Computed columns: name, field_count, policy_count, is_unsecured, disagreeing_substrate_count, has_measured_rows, semantic_mapping_count, meaning_is_only_tabular, exact_mapping_count, aligned_mapping_count, is_unaligned_to_standard, semantic_type_iri_field_count, lacks_semantic_type_convention, is_unsecured_governance_record, unrestricted_non_admin_policy_count, restricted_non_admin_policy_count, is_readable_in_full_by_non_admin, is_controlled_for_every_non_admin

### access_principals

- Fields: 160/160 (100.0%)
- Computed columns: name, organization_scope, role_label, policy_count, grant_count, visible_table_count, has_no_access, is_over_privileged

### access_policies

- Fields: 5467/5467 (100.0%)
- Computed columns: name, is_write_command, is_unrestricted, principal_is_admin, is_unrestricted_non_admin_grant, is_unwitnessed_write, denial_test_count

### field_grants

- Fields: 129269/129269 (100.0%)
- Computed columns: name, field_table, field_name, field_is_derived, is_writable_derived_field, is_masked, grant_key_when_readable

### role_schemas

- Fields: 80/80 (100.0%)
- Computed columns: name, search_path, view_count, is_empty_schema

### role_schema_views

- Fields: 6096/6096 (100.0%)
- Computed columns: name, schema_name, source_view, grant_key, column_count, table_field_count, is_full_width, is_degenerate_view

### jwt_claim_mappings

- Fields: 8/8 (100.0%)
- Computed columns: name, usage_count

### access_denial_tests

- Fields: 102/102 (100.0%)
- Computed columns: name, has_run, is_passing, is_leak, is_unproven, is_positive_control

### app_users

- Fields: 140/140 (100.0%)
- Computed columns: name, agent_kind, organization, assignment_count, has_no_principal, holds_multiple_principals, is_non_human_sign_in

### principal_assignments

- Fields: 110/110 (100.0%)
- Computed columns: name, principal_is_admin, user_organization, principal_organization, is_cross_organization_grant

### process_mining_runs

- Fields: 75/75 (100.0%)
- Computed columns: name, as_of_instant, conformance_rate, is_conformant, has_major_drift_from_documentation, days_since_mined, is_stale_mining_evidence, procedure_version_is_live, is_drift_on_live_version, drifted_mining_run_key, people_capture_complement_count, is_deviation_unexplained_by_people, undocumented_path_count, has_undocumented_enacted_path, conformance_percent

### vocabularies

- Fields: 216/216 (100.0%)
- Computed columns: name, term_count, orphan_term_count, has_orphan_terms, is_machine_accessible, managed_scheme_procedure_key, organized_transcript_count, organized_field_notes_count, organized_process_map_count, organized_mined_event_trace_count, organized_document_excerpt_count, organized_material_count, organized_kind_count, is_single_kind_frame, latest_organized_material_at, refinement_count, is_frozen_despite_new_collection, ontology_preceded_vocabulary_control

### vocabulary_terms

- Fields: 792/792 (100.0%)
- Computed columns: name, usage_count, is_orphan_term, is_widely_adopted_term, orphan_term_vocabulary_key, broader_term_parent, scheme_governed_dimension, introduced_release_issued_at, latest_meaning_change_at, has_stale_definition, structural_shift_count, has_structural_sense_shift_across_years, pref_label_practitioner_mention_count, alt_label_practitioner_mention_count, is_organized_around_official_term, source_phrasing_count, unreconciled_phrasing_count, has_unreconciled_variant_phrasings

### knowledge_broker_links

- Fields: 88/88 (100.0%)
- Computed columns: name, as_of_instant, days_since_consulted, is_active_reliance, broker_is_still_engaged, is_at_risk_reliance, active_reliance_broker_key, at_risk_broker_key, pointed_holder, locates_other_holder, translation_between_vocabularies

### conformance_substrates

- Fields: 88/88 (100.0%)
- Computed columns: name, is_graded, run_count, latest_cells_tested, latest_cells_passed, latest_harness_errors, latest_cells_failed, latest_score, disagreeing_field_count, disagreeing_table_count, is_fully_conformant

### conformance_runs

- Fields: 45/45 (100.0%)
- Computed columns: name, substrate_count, perfect_substrate_count, cells_tested, cells_passed, cells_failed, overall_score, imperfect_substrate_count, is_fully_conformant

### substrate_run_scores

- Fields: 455/455 (100.0%)
- Computed columns: name, cells_failed, score, calculated_score, lookup_score, aggregation_score, is_perfect, perfect_run_key, is_in_latest_run, latest_cells_tested, latest_cells_passed, latest_error_flag, substrate_label

### table_conformance

- Fields: 5607/5607 (100.0%)
- Computed columns: name, cells_failed, score, is_perfect, imperfect_substrate_key, imperfect_table_key, disagreeing_field_count, substrate_label, subject_area

### field_disagreements

- Fields: 135/135 (100.0%)
- Computed columns: name, sampled_cell_count, is_fully_sampled, formula, substrate_label

### cell_disagreements

- Fields: 615/615 (100.0%)
- Computed columns: name, substrate, rulebook_field

### knowledge_methods

- Fields: 130/130 (100.0%)
- Computed columns: name, elicitation_use_count, application_count, usage_count, is_applied

### source_articles

- Fields: 40/40 (100.0%)
- Computed columns: name, claim_count, covered_claim_count, agreed_claim_count, uncovered_claim_count, coverage_percent, agreed_coverage_percent, is_fully_covered

### article_claims

- Fields: 7200/7200 (100.0%)
- Computed columns: name, required_evidence, evidence_count, valid_evidence_count, agreed_evidence_count, is_covered, is_agreed, has_rejected_evidence

### claim_evidence

- Fields: 19800/19800 (100.0%)
- Computed columns: name, claim_kind, field_catalog_name, field_is_witness, field_has_data, field_is_discriminating, field_is_contested, table_has_rows, question_is_answered, question_witnessed_answer, profile_mapping_count, method_is_applied, procedure_execution_count, has_justification, is_witness_proof, is_structural_proof, is_question_proof, is_standard_proof, is_scenario_proof, is_valid, is_contested, is_agreed_evidence

### method_applications

- Fields: 20/20 (100.0%)
- Computed columns: name

### lifecycle_statuses

- Fields: 44/44 (100.0%)
- Computed columns: name, version_use_count, execution_use_count, is_non_pko_status_in_use

### facilities

- Fields: 12/12 (100.0%)
- Computed columns: name, deviating_run_count, clean_run_count, is_deviation_only_facility

### machine_types

- Fields: 3/3 (100.0%)
- Computed columns: name

### energy_sources

- Fields: 4/4 (100.0%)
- Computed columns: name

### machines

- Fields: 15/15 (100.0%)
- Computed columns: name, energy_source_count, unisolated_energy_source_count, is_non_standard_configuration, has_unisolated_energy_source

### machine_energy_sources

- Fields: 30/30 (100.0%)
- Computed columns: name, machine_procedure_version, isolation_step_count, is_unisolated_energy_source, unisolated_machine_key

### lock_devices

- Fields: 3/3 (100.0%)
- Computed columns: name

### protective_equipment

- Fields: 3/3 (100.0%)
- Computed columns: name

### step_lock_requirements

- Fields: 3/3 (100.0%)
- Computed columns: name

### step_protective_equipment

- Fields: 3/3 (100.0%)
- Computed columns: name

### regulatory_frameworks

- Fields: 6/6 (100.0%)
- Computed columns: name, requirement_count, required_procedure_count

### procedure_targets

- Fields: 24/24 (100.0%)
- Computed columns: name, machine_is_non_standard, handling_failure_mode_count, is_unhandled_non_standard_target

### procedure_adoptions

- Fields: 6/6 (100.0%)
- Computed columns: name, is_adoption_mode_unstated

### procedure_outcome_criteria

- Fields: 10/10 (100.0%)
- Computed columns: name, failure_procedure_key

### relation_types

- Fields: 22/22 (100.0%)
- Computed columns: name, usage_count

### activity_relations

- Fields: 35/35 (100.0%)
- Computed columns: name, relation_type_is_defined, uses_undefined_relation_type, from_step_version, overlaps_version_key, enables_version_key, prevents_version_key

### step_variables

- Fields: 357/357 (100.0%)
- Computed columns: name, source_direction, source_step, is_dangling_input, is_miswired_source, consumer_count, input_step_key, output_step_key, step_accountable_agent, step_agent_kind, consumer_role, consumer_workflow, source_step_agent, source_step_agent_kind, is_input_from_ai_artifact, ai_artifact_consumer_has_no_accountable_agent, ai_blast_radius_path

### execution_entities

- Fields: 195/195 (100.0%)
- Computed columns: name, variable_direction, variable_step, executed_step, is_usage_direction_mismatch, is_foreign_variable, used_execution_key, generated_execution_key, variable_datatype, variable_expected_format, violates_declared_datatype_or_format, generating_agent, attributed_to_agent

### step_conditions

- Fields: 56/56 (100.0%)
- Computed columns: name, check_count, is_never_checked, precondition_step_key, postcondition_step_key, invariant_step_key, safety_critical_step_key, is_machine_parseable

### condition_checks

- Fields: 42/42 (100.0%)
- Computed columns: name, condition_kind, is_failed_precondition, is_violated_invariant, failed_precondition_execution_key, violated_invariant_execution_key

### failure_modes

- Fields: 20/20 (100.0%)
- Computed columns: name, escalation_role_has_no_holder, escalates_to_vacant_role, has_no_response

### step_cues

- Fields: 28/28 (100.0%)
- Computed columns: name, observation_count, unescalated_observation_count, danger_cue_step_key, incomplete_cue_step_key, failure_mode_response, is_unanswerable_sign

### cue_observations

- Fields: 24/24 (100.0%)
- Computed columns: name, cue_requires_escalation, is_unescalated_danger_cue, unescalated_cue_key, unescalated_execution_key, owner_organization, cue_signals_incomplete_step, is_awaiting_acknowledgement

### decision_points

- Fields: 15/15 (100.0%)
- Computed columns: name, has_no_deciding_factors, is_dmn_encoded

### execution_participants

- Fields: 4/4 (100.0%)
- Computed columns: name

### step_resources

- Fields: 2/2 (100.0%)
- Computed columns: name

### faq_categories

- Fields: 8/8 (100.0%)
- Computed columns: name, faq_count

### faq_targets

- Fields: 4/4 (100.0%)
- Computed columns: name, faq_count

### authoring_submissions

- Fields: 9/9 (100.0%)
- Computed columns: name, is_expert_authored_conforming, is_non_conforming_accepted

### process_knowledge_levels

- Fields: 12/12 (100.0%)
- Computed columns: name, tacit_strategy_count, explicit_strategy_count, lacks_capture_strategy_for_either_form

### level_capture_strategies

- Fields: 10/10 (100.0%)
- Computed columns: name, contradicts_knowledge_form

### level_pyramid_questions

- Fields: 5/5 (100.0%)
- Computed columns: name

### process_level_statements

- Fields: 32/32 (100.0%)
- Computed columns: name, level_question_key, pyramid_match_count, is_filed_at_wrong_level

### tactical_resource_allocations

- Fields: 15/15 (100.0%)
- Computed columns: name, utilization_percent, is_bottleneck

### process_strategic_alignments

- Fields: 8/8 (100.0%)
- Computed columns: name, states_no_trade_off

### business_outcomes

- Fields: 6/6 (100.0%)
- Computed columns: name, linked_measure_count

### process_outcome_measures

- Fields: 10/10 (100.0%)
- Computed columns: name, is_unlinked_to_business_outcome

### process_stages

- Fields: 28/28 (100.0%)
- Computed columns: name, step_count, owner_has_no_holder, is_unowned_or_empty_stage

### process_interdependencies

- Fields: 12/12 (100.0%)
- Computed columns: name, is_hindering_dependency, hindered_procedure_key

### stakeholder_lenses

- Fields: 18/18 (100.0%)
- Computed columns: name, granularity_rank

### procedure_lens_views

- Fields: 120/120 (100.0%)
- Computed columns: name, procedure_current_version, lens_granularity_rank, view_granularity_rank, is_granularity_misfit, is_disconnected_silo, step_level_procedure_key, category_level_procedure_key

### applicability_scopes

- Fields: 15/15 (100.0%)
- Computed columns: name, dimension_count, states_conditions

### step_context_sensitivities

- Fields: 8/8 (100.0%)
- Computed columns: name, unscoped_step_key

### situational_variants

- Fields: 20/20 (100.0%)
- Computed columns: name, scope_dimension_count, scope_states_conditions, has_no_applicability_dimension, diverges_without_stated_conditions

### collected_source_materials

- Fields: 165/165 (100.0%)
- Computed columns: name, is_modeled_before_organized, source_document_revised_at, is_document_source, is_people_capture, is_practice_evidence, is_captured_in_flow_of_work, dependent_trace_count, is_dependency_invisible_to_change, changed_dependent_count, has_knowledge_affected_by_source_change

### scheme_refinements

- Fields: 2/2 (100.0%)
- Computed columns: name

### term_label_variants

- Fields: 583/583 (100.0%)
- Computed columns: name, term_scheme, term_pref_label, wording_key, pref_wording_key, concepts_sharing_wording, is_ambiguous_label, practitioner_mention_count, pref_wording, same_pref_wording_count, is_cross_scheme_duplicate_pref

### ai_labeling_runs

- Fields: 18/18 (100.0%)
- Computed columns: name, output_count, non_canonical_output_count, grounding_scheme_is_machine_accessible, is_ungrounded_synonym_sprawl, grounded_in_non_machine_readable_scheme

### source_term_mentions

- Fields: 225/225 (100.0%)
- Computed columns: name, wording_key, matching_label_count, matching_pref_label_count, is_uncontrolled_wording, is_non_canonical_generated_value, intended_term_role, unresolved_intended_term_key, unresolved_role_key

### term_relations

- Fields: 12/12 (100.0%)
- Computed columns: name, from_term_grandparent, asserts_indirect_link_as_direct

### term_meaning_changes

- Fields: 6/6 (100.0%)
- Computed columns: name, span_days

### external_standard_terms

- Fields: 153/153 (100.0%)
- Computed columns: name, as_of_instant, rehomed_term_same_as, rehomed_term_namespace, rehomed_term_release_issued_at, profile_namespace_iri, days_since_deprecated, is_recent_deprecation, using_mapping_count, stale_identifier_mapping_count, is_adopted_but_deprecated, is_alignment_stale_after_deprecation, is_needed_deprecated_term_not_rehomed, is_rehomed_without_identity_link, is_rehomed_without_new_release, has_unpropagated_identifier_change, rehoming_kept_external_namespace

### role_capability_tags

- Fields: 27/27 (100.0%)
- Computed columns: name, capability_scheme_dimension, is_tag_outside_capability_scheme

### classification_facets

- Fields: 8/8 (100.0%)
- Computed columns: name, assignment_count

### procedure_facet_assignments

- Fields: 12/12 (100.0%)
- Computed columns: name

### encoding_lifecycle_stages

- Fields: 18/18 (100.0%)
- Computed columns: name, annotation_count, is_stage_without_feedback

### knowledge_consumer_systems

- Fields: 80/80 (100.0%)
- Computed columns: name, model_sync_count, integration_count, is_knowledge_silo, is_unlinked_toolchain_component, semantic_layer_component_count, platform_capability_count, is_immature_graph_platform, holds_computationally_encoded_procedure_knowledge, stores_procedure_knowledge_without_computational_access

### consumer_system_syncs

- Fields: 72/72 (100.0%)
- Computed columns: name, system_audience, loaded_procedure, canonical_version, canonical_version_modified_at, loaded_version_creator, is_behind_canonical_version, predates_version_change, is_ai_fed_from_forked_copy, drops_provenance_in_transit, reaches_humans, reaches_machines

### integration_pathways

- Fields: 8/8 (100.0%)
- Computed columns: name, integration_count_on_pathway

### agent_integrations

- Fields: 49/49 (100.0%)
- Computed columns: name, pathway_integration_count, observed_answer_count, is_shadow_integration, is_one_off_connection, snapshot_is_governed, is_deployed_on_ungoverned_graph

### grounding_snapshots

- Fields: 36/36 (100.0%)
- Computed columns: name, is_governed, reasoner_run_count, consistent_reasoner_run_count, latest_materialized_at, served_without_consistency_check, served_before_materialization, assertion_count, stale_assignment_assertion_count, serves_stale_role_assignments, deprecated_as_current_count, serves_deprecated_as_current

### reasoner_runs

- Fields: 15/15 (100.0%)
- Computed columns: name, is_richness_tractability_failure, passes_schema_but_fails_reasoner

### snapshot_assertions

- Fields: 150/150 (100.0%)
- Computed columns: name, source_version_status, source_assignment_is_current, snapshot_consistent_run_count, snapshot_is_reasoned, is_stale_role_assertion, presents_deprecated_as_current, lacks_provenance, has_opaque_identifier, lacks_dublin_core

### retrieval_segments

- Fields: 63/63 (100.0%)
- Computed columns: name, author_agent_kind, related_from_count, is_isolated_chunk, contradicted_is_indexed, is_inconsistent_grounding, is_machine_held_knowledge

### knowledge_query_definitions

- Fields: 16/16 (100.0%)
- Computed columns: name, source_system_count, misses_a_layer, consolidated_distributed_sources

### knowledge_query_sources

- Fields: 6/6 (100.0%)
- Computed columns: name

### assistant_answers

- Fields: 429/429 (100.0%)
- Computed columns: name, grounding_count, own_knowledge_grounding_count, cited_grounding_count, stale_grounding_count, is_not_from_own_knowledge, is_untraceable_to_source, context_unescalated_danger_cue_count, stayed_silent_on_safety_problem, context_step_ended_at, arrived_after_step_ended, execution_of_context, context_step, assumed_step_completed_count, lost_track_of_state, specified_transition_count, contradicts_shared_model, is_explicitly_grounded_recommendation, recommendation_rests_on_nothing_explicit, recommended_step_regulatory_count, requirement_check_count, conflict_count, conflicts_with_regulation, is_unchecked_regulated_recommendation, delivered_despite_conflict, recommended_step_needs_human, acted_on_without_human_judgment, answered_compliance_question_from_documents, document_interpretation_erred, model_did_the_reasoning, wrong_because_graph_was_stale, owner_organization, model_reasoned_and_task_failed

### answer_groundings

- Fields: 70/70 (100.0%)
- Computed columns: name, is_from_own_knowledge, assertion_is_stale, assertion_presents_deprecated, grounds_on_stale_assertion

### answer_requirement_checks

- Fields: 2/2 (100.0%)
- Computed columns: name

### ai_tool_invocations

- Fields: 18/18 (100.0%)
- Computed columns: name, executed_step, declared_function_count, is_undeclared_tool_use, declared_input_count, acted_without_declared_context

### embedding_probes

- Fields: 18/18 (100.0%)
- Computed columns: name, synonyms_not_similar, opposites_not_opposed

### prompt_templates

- Fields: 35/35 (100.0%)
- Computed columns: name, child_template_count, is_disconnected_prompt_knowledge, is_unmanaged_prompt_in_use, spends_budget_on_rare_detail, condensation_percent, is_verbatim_dump

### knowledge_projections

- Fields: 49/49 (100.0%)
- Computed columns: name, version_modified_at, open_count, diagram_can_diverge_from_model, narrative_not_generated_from_model, narrative_unreachable, is_published

### model_annotations

- Fields: 54/54 (100.0%)
- Computed columns: name, is_discussion_in_authoritative_model, flag_bypassed_annotation_interface, is_lost_new_knowledge, is_friction_without_gap, is_open_outdated_flag

### knowledge_search_events

- Fields: 24/24 (100.0%)
- Computed columns: name, found_nothing_useful, gave_up_after_seeing_results, is_unlinked_failed_search

### ai_adoption_initiatives

- Fields: 225/225 (100.0%)
- Computed columns: name, as_of_instant, target_has_no_explicit_steps, target_version_issued_at, target_version_under_specified, target_version_notation_only, target_elicitation_evidence_count, reviewed_output_count, preceding_reviewed_output_count, last_outcome_measured_at, days_since_outcome_measured, ai_insight_count, hands_tacit_procedure_to_agent, agent_on_under_specified_procedure, agent_on_notation_only_procedure, calls_for_process_knowledge_framework, redesigned_before_documented, breaks_elicit_encode_connect_order, went_full_without_reviewed_partial_stage, underinvests_knowledge_layer, not_anchored_in_process_knowledge, agentic_without_knowledge_capture, is_ai_outcome_unmeasured, adopted_without_bottom_line_result, failed_without_formalized_knowledge

### knowledge_outcome_measurements

- Fields: 70/70 (100.0%)
- Computed columns: name, baseline_access_score, baseline_error_rate, baseline_minutes_per_run, baseline_satisfaction, higher_access_fewer_errors, higher_access_more_efficient, higher_access_more_satisfied, is_unacted_adverse_outcome, is_gain_outside_procedural_scope

### ai_insight_proposals

- Fields: 24/24 (100.0%)
- Computed columns: name, target_creator_kind, is_unvalidated_or_stranded_insight, grew_model_without_human_seed

### assistant_benchmarks

- Fields: 16/16 (100.0%)
- Computed columns: name, accuracy_lift_points, shows_spatial_lift_from_graph_queries, shows_geospatial_retrieval_lift

### governed_models

- Fields: 350/350 (100.0%)
- Computed columns: name, as_of_instant, days_since_registered, charter_count, current_charter_count, current_steward_role, current_steward_agent, current_authority_role, current_authority_agent, is_ownerless, is_ownerless_past_a_year, governance_lapsed, has_no_current_steward, has_no_current_authority, is_procedure_without_change_authority, last_steward_activity_at, days_since_steward_activity, is_unmaintained, documents_behind_count, is_not_kept_current, open_practice_drift_count, has_open_practice_drift, is_neglected_and_drifting, expert_found_drift_count, cq_review_count, last_cq_review_at, days_since_cq_review, cq_review_overdue, degradation_hidden_until_wrong_answer, baseline_question_count, requirements_spec_incomplete, collection_control_count, stewardship_control_count, retrieval_control_count, use_control_count, continuous_pipeline_stage_count, lacks_lifecycle_stage_control, pipeline_not_continuous, procedure_has_no_explicit_steps, is_unmanageable_undocumented_work, is_without_originating_use_case, pilot_count, is_adopted_without_pilot, requirements_expert_count, implementation_expert_count, publication_expert_count, maintenance_expert_count, experts_not_involved_throughout, real_data_mapping_run_count, is_implemented_without_real_data

### model_charters

- Fields: 208/208 (100.0%)
- Computed columns: name, as_of_instant, is_current, steward_agent, authority_agent, authority_organization, model_kind, model_domain_owner, model_headcount, model_tooling_owner_role, model_first_control_adopted_at, is_steward_unwritten, is_authority_scope_unstated, conflates_steward_and_authority, is_sanctioned_dual_holding, is_authority_outside_domain_owner, controls_precede_ownership, steward_activity_count, last_drift_watch_at, days_since_drift_watch, is_drift_watch_lapsed, procedure_decision_count, model_review_count, is_named_but_unexercised, steward_is_outside_tooling, adopted_template_without_adaptation

### steward_activities

- Fields: 20/20 (100.0%)
- Computed columns: name, activity_model

### model_change_requests

- Fields: 1054/1054 (100.0%)
- Computed columns: name, steward_agent, authority_agent, rule_key, required_route, is_accepted, is_modeling_change, is_schema_change, has_authority_review, requires_authority_review, is_minor_scope, steward_own_change_unreviewed, steward_approval_out_of_bounds, authority_review_skipped, placement_not_decided_by_authority, is_misrouted, lacks_motivating_question, unmotivated_and_not_returned, motivated_by_failing_question, is_accepted_without_named_approver, deployed_without_target_release, is_misclassified_agent_swap, ai_to_human_move_without_compliance_review, ai_to_human_move_unaudited, is_ai_to_human_move, lifecycle_change_by_unauthorized_agent, assessed_inconsistent_count, post_deploy_inconsistent_count, assessed_inference_count, post_deploy_inference_count, assessed_query_result_count, post_deploy_query_result_count, assessed_coverage_gap_count, would_make_instances_inconsistent, would_alter_inferences, would_alter_query_results, would_leave_coverage_incomplete, missed_inconsistent_instances, missed_altered_inferences, missed_altered_query_results, leaves_coverage_unchecked, domain_change_spread_wrong_inferences, intuitive_disjointness_broke_individuals, validation_run_count, acceptance_failure_total, structural_pass_count, vocabulary_pass_count, accepted_without_test_run, accepted_with_failing_suite, accepted_without_structural_check, accepted_without_vocabulary_check, integrity_check_count, human_integrity_check_count, disjointness_check_count, domain_inference_check_count, range_consistency_check_count, integrity_decided_without_human, skipped_disjointness_review, skipped_domain_inference_review, skipped_range_review, unresolved_objection_count, approved_over_unresolved_objection

### change_authority_rules

- Fields: 12/12 (100.0%)
- Computed columns: name, misrouted_request_count, is_rule_bypassed

### change_impact_findings

- Fields: 10/10 (100.0%)
- Computed columns: name

### change_integrity_checks

- Fields: 30/30 (100.0%)
- Computed columns: name, checker_kind

### change_objections

- Fields: 4/4 (100.0%)
- Computed columns: name, is_unresolved

### change_validation_runs

- Fields: 56/56 (100.0%)
- Computed columns: name, release, expected_chain_count, unproduced_chain_count, has_uninspected_failures, is_unconfirmed_consistency, misses_expected_inference

### expected_inference_checks

- Fields: 8/8 (100.0%)
- Computed columns: name, is_unproduced

### model_consumers

- Fields: 4/4 (100.0%)
- Computed columns: name

### consumer_revalidations

- Fields: 26/26 (100.0%)
- Computed columns: name

### model_documents

- Fields: 15/15 (100.0%)
- Computed columns: name, model_current_release, is_behind_current_release

### staleness_query_runs

- Fields: 9/9 (100.0%)
- Computed columns: name, runner_kind, not_surfaced_to_steward

### external_dependency_revisions

- Fields: 15/15 (100.0%)
- Computed columns: name, affected_mapping_count, as_of_instant, days_since_published, is_untracked_revision

### stakeholder_questions

- Fields: 66/66 (100.0%)
- Computed columns: name, triager_kind, result_route, as_of_instant, days_open, is_unanswered_past_due, is_unanswerable_today, is_untriaged_unanswerable, needs_structural_change, unanswerable_without_scope_request, is_misrouted_after_triage

### model_expansion_requests

- Fields: 21/21 (100.0%)
- Computed columns: name, model_domain_owner, concept_count, uncovered_concept_count, is_cross_function_expansion, requires_schema_extension, fit_decision_contradicts_concept_fit

### expansion_concept_fits

- Fields: 18/18 (100.0%)
- Computed columns: name, is_uncovered

### competency_question_set_entries

- Fields: 56/56 (100.0%)
- Computed columns: name, as_of_instant, days_since_added, is_outgrown_baseline_question, irrelevant_but_still_active, governance_use_count, serves_every_governance_use

### competency_question_runs

- Fields: 50/50 (100.0%)
- Computed columns: name, entry_is_original, prior_was_answerable, is_baseline_regression, is_unfixed_wrong_answer

### competency_question_reviews

- Fields: 4/4 (100.0%)
- Computed columns: name

### quality_criteria

- Fields: 16/16 (100.0%)
- Computed columns: name, assessment_count

### quality_assessments

- Fields: 11/11 (100.0%)
- Computed columns: name

### term_definitions

- Fields: 9/9 (100.0%)
- Computed columns: name, drafter_kind, ai_draft_adopted_unrevised

### model_proposals

- Fields: 220/220 (100.0%)
- Computed columns: name, proposer_kind, reviewer_kind, committer_kind, model_steward_agent, is_ai_candidate, is_only_proposed, committed_without_expert_review, entered_without_quality_check, approver_unknown, committed_outside_any_version, ai_question_adopted_unvetted, ai_alignment_decided_by_ai, ai_axiom_without_human_review, adoption_not_answered_by_person, has_no_human_touchpoint, ai_instance_data_loaded_without_steward, awaits_engineer_vetting, is_pending_alignment_decision, awaits_steward_approval

### assignment_instant_checks

- Fields: 32/32 (100.0%)
- Computed columns: name, step_role, assignment_role, assignment_valid_from, assignment_valid_to, assignment_agent, assignment_agent_version, held_step_at_instant

### instance_data_versions

- Fields: 4/4 (100.0%)
- Computed columns: name, logged_change_count

### domain_coverage_areas

- Fields: 8/8 (100.0%)
- Computed columns: name, is_uncovered_area

### governance_stage_controls

- Fields: 40/40 (100.0%)
- Computed columns: name, is_continuous_pipeline_stage

### process_design_decisions

- Fields: 12/12 (100.0%)
- Computed columns: name, is_commitment_without_rationale, days_before_recorded, is_recorded_after_the_fact

### model_change_log_entries

- Fields: 306/306 (100.0%)
- Computed columns: name, motivating_question, release_version, release_decision_undocumented, alters_logical_model, is_class_removal_or_rename, is_invalidating_domain_range_change, is_inconsistent_disjointness, is_backward_incompatible, is_additive_schema_change, is_unexplained_modification, rationale_without_question, is_schema_change_without_increment, is_instance_change_in_schema_release, schema_change_without_request, cannot_be_audited, cannot_be_rolled_back, is_untraceable_breaking_change

### drift_observations

- Fields: 24/24 (100.0%)
- Computed columns: name, release_passed_validation, release_issued_at, is_open_practice_mismatch, went_undetected_by_passing_suite, drift_follows_clean_release

### sourcing_functions

- Fields: 252/252 (100.0%)
- Computed columns: name, is_outsourced, is_business_process_outsourcing, is_knowledge_process_outsourcing, is_vital_expertise_classed_non_core, is_what_how_split, is_method_knowledge_held_outside, claims_how_without_doing, is_vital_expertise_process, is_outsourced_vital_expertise_process, audit_item_count, dependency_count, coverage_gap_count, specification_audit_count, specification_shortfall_count, method_shortfall_count, provider_ip_documentation_count, designs_what_it_cannot_build, is_unaudited_function, capture_initiative_count, is_uncaptured_priority_process

### knowledge_audits

- Fields: 12/12 (100.0%)
- Computed columns: name, finding_count, treats_deficit_as_cost_problem

### knowledge_audit_items

- Fields: 84/84 (100.0%)
- Computed columns: name, client_organization, has_internal_shortfall, is_knowledge_dependency, is_coverage_gap, is_unnamed_finding, is_single_team_silo

### knowledge_capture_initiatives

- Fields: 3/3 (100.0%)
- Computed columns: name

### knowledge_workforce_positions

- Fields: 4/4 (100.0%)
- Computed columns: name

### provider_engagements

- Fields: 98/98 (100.0%)
- Computed columns: name, is_active, relied_dependency_count, is_unplanned_knowledge_return, is_knowledge_access_unsecured, required_deliverable_count, to_client_required_count, to_client_delivered_count, to_provider_delivered_count, joint_deliverable_count, lacks_knowledge_deliverables, is_one_way_learning, is_short_term_without_joint_knowledge, obliges_knowledge_flow_back

### knowledge_deliverables

- Fields: 20/20 (100.0%)
- Computed columns: name, is_delivered

### corporate_governance_programs

- Fields: 6/6 (100.0%)
- Computed columns: name, is_compliance_only

### records_retention_policies

- Fields: 12/12 (100.0%)
- Computed columns: name, program_sponsor_discipline, is_legal_led_knowledge_destruction

### ai_registry_model_versions

- Fields: 24/24 (100.0%)
- Computed columns: name, graph_individual_count, live_production_deployment_count, is_live_but_unregistered_in_graph

### ai_model_deployments

- Fields: 42/42 (100.0%)
- Computed columns: name, as_of_instant, is_live_in_production, agent_identifier, assignment_link_count, artifact_link_count, is_isolated_registry_fact

### ai_model_evaluations

- Fields: 6/6 (100.0%)
- Computed columns: name, passed

### ai_agent_accountabilities

- Fields: 24/24 (100.0%)
- Computed columns: name, as_of_instant, is_current, accountable_agent_kind, is_accountable_to_non_person, is_current_human_accountability

### agent_upgrade_assessments

- Fields: 8/8 (100.0%)
- Computed columns: name, attributed_artifact_count, traversed_downstream_step_count, missed_traversed_impact

### assignment_update_policies

- Fields: 3/3 (100.0%)
- Computed columns: name

### role_assignment_update_tasks

- Fields: 102/102 (100.0%)
- Computed columns: name, as_of_instant, policy_trigger_owner_role, trigger_owner_cover_count, lacks_named_trigger_owner, policy_sla_hours, elapsed_minutes, exceeded_update_sla, ending_role, replacement_role, changed_role_instead_of_assignment, dependent_run_version, dependent_run_started_at, dependent_run_role_step_count, missed_next_dependent_run, failed_notice_count, stale_assignment_broke_routing

### assignment_routed_notices

- Fields: 60/60 (100.0%)
- Computed columns: name, notice_role, recipient_role_key, recipient_pair_assignment_count, recipient_valid_from, recipient_latest_valid_to, recipient_open_ended_count, recipient_held_role_when_sent, reached_wrong_person_or_nobody, routed_around_model

### practitioner_expertise

- Fields: 15/15 (100.0%)
- Computed columns: name, knows_more_than_can_say, is_unexplained_foresight

### critical_incidents

- Fields: 9/9 (100.0%)
- Computed columns: name, is_adverse_or_improvised, has_surfaced_judgment

### interview_probes

- Fields: 12/12 (100.0%)
- Computed columns: name, why_answer, shortfall_answer

### observed_actions

- Fields: 40/40 (100.0%)
- Computed columns: name, is_small_choice, is_unofficial_workaround, is_omitted_from_own_account, is_missed_step_left_uncaptured, is_watched_not_questioned, has_recorded_reason, has_counterfactual_answer

### elicitation_participants

- Fields: 42/42 (100.0%)
- Computed columns: name, is_practitioner, is_subject_matter_expert, is_knowledge_engineer, is_knowledge_producer, is_knowledge_consumer

### representation_reviews

- Fields: 4/4 (100.0%)
- Computed columns: name

### workflow_view_divergences

- Fields: 6/6 (100.0%)
- Computed columns: name, is_reconciled, is_surfaced_but_unreconciled

### expert_cognitions

- Fields: 12/12 (100.0%)
- Computed columns: name, is_mental_model, is_automatic_heuristic, is_heuristic_oversimplified

### concept_ladder_rungs

- Fields: 20/20 (100.0%)
- Computed columns: name, step_top_level, is_ultimate_goal, is_decomposition_rung

### repertory_grid_constructs

- Fields: 12/12 (100.0%)
- Computed columns: name, dimension, is_never_stated_dimension, is_recorded_discriminating_dimension

### knowledge_conversions

- Fields: 10/10 (100.0%)
- Computed columns: name, is_mode_inconsistent_with_forms

### knowledge_holdings

- Fields: 70/70 (100.0%)
- Computed columns: name, formalized_fragment_session, is_unformalized_process_knowledge, is_procedural_knowledge, is_judgment_outside_document, is_veteran_discretion, is_formalized_without_elicitation_work

### fragment_corroborations

- Fields: 4/4 (100.0%)
- Computed columns: name

### knowledge_test_outcomes

- Fields: 10/10 (100.0%)
- Computed columns: name, is_improved

### know_how_carriers

- Fields: 364/364 (100.0%)
- Computed columns: name, as_of_instant, holder_is_still_engaged, holder_service_started_at, holder_departure_at, days_until_holder_departure, days_served_to_as_of, days_served_to_departure, holder_tenure_years, is_veteran_held, is_holder_leaving_soon, transfer_count, repository_entry_count, source_relationship_count, is_held_in_both_forms, is_held_by_current_practitioner, is_untransferred_veteran_know_how, is_at_risk_of_imminent_loss, is_held_only_by_departed, is_held_by_departed_holder, is_captured, must_be_relearned_if_holder_leaves, is_overlooked_living_holder, transfer_stops_without_veteran, dependency_community, builds_on_same_community_know_how, lost_accumulation_years, is_delegated_to_unfit_source

### knowledge_transfers

- Fields: 63/63 (100.0%)
- Computed columns: name, from_organization, recipient_role_count, is_traditional_channel, is_social_network_channel, is_ambient_absorption_by_non_practitioner, know_how_topic

### knowledge_repository_entries

- Fields: 88/88 (100.0%)
- Computed columns: name, as_of_instant, author_is_still_engaged, author_agent_kind, owner_organization, days_since_updated, is_stale, is_execution_feedback, is_machine_authored, outlives_author_tenure, is_uncredited_expert_know_how

### community_memberships

- Fields: 36/36 (100.0%)
- Computed columns: name, member_organization, community_organization, is_external_member

### source_relationships

- Fields: 12/12 (100.0%)
- Computed columns: name, is_unnegotiated_power_gap, is_extractive_relationship

### department_process_accounts

- Fields: 25/25 (100.0%)
- Computed columns: name, conflicting_department, is_awaiting_engagement, is_conflicting_account, is_unresolved_disagreement

### problem_occurrences

- Fields: 60/60 (100.0%)
- Computed columns: name, solver_is_still_engaged, is_solved, prior_was_solved, prior_solution_entry, prior_solver, prior_solver_is_still_engaged, is_solved_without_recorded_solution, has_been_solved_before, has_consultable_specialist, is_relearned_solved_problem, is_turnover_regression

### onboarding_records

- Fields: 63/63 (100.0%)
- Computed columns: name, as_of_instant, is_proficient, days_to_proficiency, days_since_start, is_recent_start, procedure_repository_entry_count, procedure_departed_only_count, is_starting_from_nothing

### sharing_recognitions

- Fields: 2/2 (100.0%)
- Computed columns: name

### capability_declines

- Fields: 16/16 (100.0%)
- Computed columns: name, preceding_stage, preceding_decline_started_at, follows_preceding_stage_decline

### knowledge_traces

- Fields: 360/360 (100.0%)
- Computed columns: name, source_material_kind, source_collected_at, source_revised_at, source_is_document, source_is_people_capture, source_is_practice_evidence, is_source_changed_since_taken, modeled_duration_minutes, is_unfaithful_to_source, derived_by_agent_kind, is_machine_derived, is_self_validated, has_incomplete_provenance, provenance_statement, is_aspect_unsupported_by_source_kind, is_document_origin, step_elicited_validation_count, step_elicited_extension_count, is_document_start_never_validated, is_document_start_never_extended, contradicted_document_revised_at, is_document_trailing_practice, prescribed_versus_enacted

### mined_flow_edges

- Fields: 36/36 (100.0%)
- Computed columns: name, documented_transition_count, is_undocumented_path, to_step_expected_minutes, is_bottleneck, is_mined_path_recorded_as_intent_without_decision

### collection_occasions

- Fields: 18/18 (100.0%)
- Computed columns: name, as_of_instant, days_since_held, is_lapsed, captured_material_count, is_held_without_capture

### stakeholder_perspectives

- Fields: 25/25 (100.0%)
- Computed columns: name, source_material_kind, conflict_partner_count, is_in_conflict, is_dissenting_view_not_kept_with_source

### model_pilots

- Fields: 3/3 (100.0%)
- Computed columns: name

### model_activity_experts

- Fields: 16/16 (100.0%)
- Computed columns: name

### model_data_mapping_runs

- Fields: 4/4 (100.0%)
- Computed columns: name

### artifact_handoffs

- Fields: 48/48 (100.0%)
- Computed columns: name, declared_source_step, declared_consumer_step, disagrees_with_declared_variable

### app_actions

- Fields: 160/160 (100.0%)
- Computed columns: name, policy_command, policy_denial_test_count, watched_field_is_witness, input_field_count, is_unpermitted, policy_command_disagrees, is_unproven_write

### app_action_fields

- Fields: 300/300 (100.0%)
- Computed columns: name, target_field_type, writes_derived_field

### abundant_knowledge_gaps

- Fields: 6/6 (100.0%)
- Computed columns: name, representing_table_row_count

### ontology_support_programmes

- Fields: 14/14 (100.0%)
- Computed columns: name, as_of_instant, supported_profile_count, has_ended, days_until_programme_ends, is_ended_with_no_steward_named, is_ending_soon_with_no_steward_named
