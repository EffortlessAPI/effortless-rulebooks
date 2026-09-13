# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 59878 |
| Passed | 59763 |
| Failed | 115 |
| Score | 99.8% |
| Duration | 24s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 42816 | 42928 | 99.7% |
| Lookup (INDEX/MATCH) | 12926 | 12929 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 4021 | 4021 | 100.0% |

## Results by Entity

### rulebook_releases

- Fields: 1/1 (100.0%)
- Computed columns: name

### ontology_profiles

- Fields: 11/11 (100.0%)
- Computed columns: name

### evaluation_contexts

- Fields: 0/1 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| eval-current | name | Post-close evaluation @ 2026-0 | Post-close evaluation @ 2026-0 |

### organizations

- Fields: 4/4 (100.0%)
- Computed columns: name

### agents

- Fields: 176/176 (100.0%)
- Computed columns: name, count_of_current_role_assignments, is_still_engaged, decision_count, overridden_decision_count, override_rate_percent, is_non_human, boundary_violation_count, is_operating_outside_boundary, draft_decision_count, overridden_draft_count, draft_rewrite_rate_percent, times_named_as_broker, is_recognized_broker, at_risk_reliance_count, has_at_risk_knowledge_reliance

### roles

- Fields: 204/204 (100.0%)
- Computed columns: name, current_agent_kind, active_assignment_count, currently_covered_assignment_count, has_no_current_holder, count_of_awaited_decisions, current_assignment_valid_from, is_non_human_held, is_ungoverned_non_human_role, departed_assignment_count, has_lost_a_holder, is_vacated_role, ungrounded_boundary_count, is_governed_by_lapsed_authority, unescalated_refusal_count, unauthorized_enforcement_assignment_count, is_ungoverned_enforcement_role

### role_assignments

- Fields: 584/611 (95.6%)
- Computed columns: name, as_of_instant, is_current, current_agent_key, is_currently_valid, agent_role_key, has_departed, covers_now, role_when_covering, agent_kind, is_non_human_assignment, predecessor_agent_kind, is_human_to_non_human_handover, is_unauthorized_non_human_assignment, was_authorized_by_change_request, decision_count, overridden_decision_count, override_rate_percent, predecessor_override_rate_percent, quality_regressed_vs_predecessor, departed_role_key, predecessor_decision_count, has_sufficient_sample, predecessor_has_sufficient_sample, comparison_is_evidentially_sound, single_override_swing_percent, quality_verdict_is_unsupported, is_unmeasured_automation_handover, error_correction_count, error_rate_percent, has_dated_authorization, days_since_authorization_review, authorization_is_overdue_for_review, is_standing_unreviewed_automation, is_unconditioned_automation_handover, exceeds_tolerable_error_rate, boundary_violation_count_for_assignment, has_any_boundary_violation, has_ungrounded_governing_boundary, suspension_condition_met, is_operating_under_met_suspension_condition, has_declared_suspension_condition, has_approving_authority, has_authorizing_change_request, is_unauthorized_enforcement_agent, governance_evidence_count, unauthorized_enforcement_role_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ra-authority-2026 | name | knowledge-authority @ 2026-01- | knowledge-authority @ 2026-01- |
| ra-authority-2026 | quality_verdict_is_unsupported | True | None |
| ra-cfo-2026 | name | cfo @ 2026-01-01T00:00:00-06:0 | cfo @ 2026-01-01 00:00:00-06 |
| ra-cfo-2026 | quality_verdict_is_unsupported | True | None |
| ra-close-pipeline-2026 | name | close-automation @ 2026-01-01T | close-automation @ 2026-01-01  |
| ra-close-pipeline-2026 | quality_verdict_is_unsupported | True | None |
| ra-comms-2026 | name | communications-manager @ 2026- | communications-manager @ 2026- |
| ra-comms-2026 | quality_verdict_is_unsupported | True | None |
| ra-controller-2026 | name | controller @ 2026-01-01T00:00: | controller @ 2026-01-01 00:00: |
| ra-controller-2026 | quality_verdict_is_unsupported | True | None |
| ra-counsel-2026 | name | employment-counsel @ 2026-01-1 | employment-counsel @ 2026-01-1 |
| ra-counsel-2026 | quality_verdict_is_unsupported | True | None |
| ra-finance-2026 | name | finance-analyst @ 2026-01-01T0 | finance-analyst @ 2026-01-01 0 |
| ra-finance-2026 | quality_verdict_is_unsupported | True | None |
| ra-new-variance | name | variance-review-agent @ 2026-0 | variance-review-agent @ 2026-0 |
| ra-new-variance | quality_verdict_is_unsupported | True | None |
| ra-new-variance | is_unmeasured_automation_handover | True | None |
| ra-notify-pipeline-2026 | name | notification-publisher @ 2026- | notification-publisher @ 2026- |
| ra-notify-pipeline-2026 | quality_verdict_is_unsupported | True | None |
| ra-old-variance | name | variance-review-agent @ 2025-0 | variance-review-agent @ 2025-0 |
| ... | ... | (7 more) | ... |

### communities_of_practice

- Fields: 2/2 (100.0%)
- Computed columns: name

### mentorships

- Fields: 1/1 (100.0%)
- Computed columns: name

### procedure_types

- Fields: 2/2 (100.0%)
- Computed columns: name

### procedures

- Fields: 2/2 (100.0%)
- Computed columns: name

### procedure_versions

- Fields: 209/216 (96.8%)
- Computed columns: name, count_of_steps, count_of_open_knowledge_gaps, is_ready_for_execution, specified_step_count, overdue_review_count, open_change_request_count, open_high_severity_gap_count, is_fit_to_execute, steward_review_cadence_days, count_of_stewardship_assignments, has_any_steward, is_live, is_unstewarded, is_live_and_unstewarded, count_of_open_blocking_gaps, has_open_blocking_gap, is_live_with_blocking_gap, should_not_be_executable, count_of_unapproved_reliance_fragments, runs_on_unapproved_knowledge, count_of_overdue_gaps, count_of_change_requests, count_of_review_events, has_governance_record, as_of_instant, days_since_modified, days_since_last_review, was_modified_since_last_review, modifier_is_authority, has_unwitnessed_change, count_of_stale_fragments, knowledge_is_staler_than_cadence, compound_fragile_fragment_count, rests_on_compound_fragile_knowledge, concentrated_witness_session_count, knowledge_base_is_concentrated, machine_consumed_unapproved_count, feeds_unapproved_knowledge_to_machines, genuinely_overdue_fragment_count, awaited_decision_count, scoped_open_blocking_gap_count, is_blocked_on_pending_decision, unexercised_human_gate_count, ai_boundary_is_unevidenced, load_bearing_unapproved_count, unlanded_decision_count, unrehearsed_control_entry_count, has_unrehearsed_control_entry, is_live_with_unrehearsed_control, cadence_breach_count, is_in_cadence_breach, has_decision_in_flight, is_unremediated_cadence_breach, is_managed_cadence_breach, governance_is_silent, valid_fragment_count, still_owns_valid_knowledge, incoming_supersession_count, is_still_referenced, is_load_bearing_orphan, is_cleanly_retired, stalled_implementation_count, is_held_unfit_by_landed_decisions, undeclared_control_kind_count, control_taxonomy_is_incomplete, has_approved_change_request, approved_change_request_count, unwatched_unowned_control_count, mining_run_count, drifted_mining_run_count, has_unresolved_mining_drift

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-v1.0.0 | days_since_modified | 109 | 110 |
| close-v1.1.0 | days_since_modified | 16 | 17 |
| close-v1.1.0 | was_modified_since_last_review | True | False |
| close-v1.1.0 | has_unwitnessed_change | True | False |
| policy-v1.0.0 | days_since_modified | 0 | 1 |
| policy-v1.0.0 | was_modified_since_last_review | True | False |
| policy-v1.0.0 | has_unwitnessed_change | True | False |

### procedure_version_links

- Fields: 2/2 (100.0%)
- Computed columns: name, superseded_version_key

### procedure_status_changes

- Fields: 5/5 (100.0%)
- Computed columns: name

### steps

- Fields: 510/510 (100.0%)
- Computed columns: name, assigned_role_label, assigned_agent_kind, blocking_requirement_count, stale_binding_count, authoritative_stale_count, available_exception_count, declared_verification_count, is_preparation_step, is_approval_step, stale_authoritative_binding_count, inputs_are_fresh, is_software_assigned, is_human_approval_gate, gate_held_by_human, binding_boundary_count, assigned_role_is_ungoverned, unusable_binding_count, all_sources_usable, unwarranted_boundary_count, is_governed_by_unwarranted_boundary, software_execution_count, has_been_approached_by_software, is_unexercised_human_gate, is_demonstrated_human_gate, unexercised_gate_version_key, has_declared_control_kind, undeclared_control_version_key, approval_step_is_software_assigned, unwitnessed_blocking_count

### step_transitions

- Fields: 285/285 (100.0%)
- Computed columns: name, is_recovery_path, count_of_from_step_executions, count_of_to_step_executions, has_reachable_origin, has_reachable_target, is_never_exercised, is_untested_recovery_path, count_of_observed_traversals, has_been_traversed, is_unwalked_recovery_path, target_blocking_requirement_count, target_carries_blocking_control, is_unrehearsed_control_entry, unrehearsed_control_version_key

### actions

- Fields: 9/9 (100.0%)
- Computed columns: name

### functions

- Fields: 8/8 (100.0%)
- Computed columns: name

### tools

- Fields: 8/8 (100.0%)
- Computed columns: name

### step_actions

- Fields: 9/9 (100.0%)
- Computed columns: name

### step_functions

- Fields: 8/8 (100.0%)
- Computed columns: name

### step_tools

- Fields: 10/10 (100.0%)
- Computed columns: name

### requirements

- Fields: 400/403 (99.3%)
- Computed columns: name, satisfaction_record_count, step_binding_count, is_bound_to_any_step, has_ever_been_evaluated, negative_outcome_count, is_inoperative_control, is_decorative_control, has_ever_produced_negative, is_unfalsified_control, claims_a_witness_field, named_witness_field_exists, derived_has_computed_witness, witness_claim_is_unverified, is_unwitnessed_blocking_control, witness_fire_count, witness_has_never_fired, evaluation_sample_size, has_meaningful_sample, is_untested_witness, is_evidenced_holding_control, control_assurance_state, unexercised_binding_count, witness_is_partially_scoped, accountable_agent, has_named_owner, is_orphaned_blocking_control, is_unwatched_and_unowned, attestation_exposure_note, unwatched_unowned_flag, uses_controlled_vocabulary

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| req-close-balance | is_untested_witness | True | None |
| req-close-cutoff | is_untested_witness | True | None |
| req-close-separation | is_untested_witness | True | None |

### step_requirements

- Fields: 150/150 (100.0%)
- Computed columns: name, requirement_is_blocking, blocking_step_key, step_when_blocking, requirement_lacks_witness, unwitnessed_step_key, satisfaction_count_for_binding, binding_was_ever_exercised, is_unexercised_blocking_binding, unexercised_binding_requirement_key

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

- Fields: 16/16 (100.0%)
- Computed columns: name, is_approved_source

### procedure_resources

- Fields: 16/16 (100.0%)
- Computed columns: name, relation_iri

### elicitation_sessions

- Fields: 25/30 (83.3%)
- Computed columns: name, as_of_instant, days_since_elicited, is_single_witness_method, practitioner_is_still_engaged, valid_fragments_produced, is_high_yield_session, is_concentrated_single_witness, is_stale_concentrated_witness, concentrated_session_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| elicit-close-shadow | name | Shadowing / 2026-04-03T09:00:0 | Shadowing / 2026-04-03 09:00:0 |
| elicit-policy-interview | name | PractitionerInterview / 2026-0 | PractitionerInterview / 2026-0 |
| elicit-policy-interview | days_since_elicited | 67 | 68 |
| elicit-policy-workshop | name | FacilitatedWorkshop / 2026-05- | FacilitatedWorkshop / 2026-05- |
| elicit-policy-workshop | days_since_elicited | 69 | 70 |

### knowledge_fragments

- Fields: 445/455 (97.8%)
- Computed columns: name, as_of_instant, is_currently_valid, source_agent_is_still_engaged, source_agent_kind, has_human_source, has_orphaned_provenance, is_undefendable_tacit_claim, is_approved, is_within_validity_window, is_relied_upon, step_procedure_version_status, is_attached_to_live_version, is_unapproved_but_relied_on, evidence_age_days, has_recorded_elicitation, is_from_single_witness, evidence_expiry_days, evidence_has_expired, owner_agent, is_awaiting_approval, owner_is_me, is_my_unfinished_approval, is_invoked_by_an_exception, has_operational_reliance, is_unapproved_and_operationally_live, age_days, is_low_confidence, owning_version_cadence_days, exceeds_owning_cadence, is_aging_low_confidence_claim, owner_role_agent_kind, is_human_owned, is_ai_validated_by_ai, review_cadence_days, is_overdue_for_review, predates_current_role_holder, owner_role_assignment_valid_from, fragility_signal_count, is_compound_fragile, is_single_point_of_failure, is_expiring_single_point_of_failure, compound_fragile_version_key, valid_fragment_session_key, consuming_step_is_software_assigned, consuming_step_agent_kind, is_unapproved_and_machine_consumed, is_unapproved_and_human_consumed, machine_consumed_unapproved_version_key, has_review_record, days_since_actual_review, is_unreviewed_since_authoring, is_genuinely_overdue, review_recency_is_inferred, inference_disagrees_with_record, genuinely_overdue_version_key, ratified_boundary_count, reliance_surface_count, days_awaiting_my_approval, is_high_blast_radius_unapproved, is_long_unapproved, unapproved_load_bearing_version_key, owner_role_is_vacated, is_orphaned_by_role, valid_fragment_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kf-close-fx-time | days_since_actual_review | 16 | 17 |
| kf-close-top-ten | days_since_actual_review | 16 | 17 |
| kf-policy-ai-boundary | evidence_age_days | 67 | 68 |
| kf-policy-ai-boundary | age_days | 67 | 68 |
| kf-policy-channel | evidence_age_days | 69 | 70 |
| kf-policy-channel | age_days | 69 | 70 |
| kf-policy-channel | days_since_actual_review | 0 | 1 |
| kf-policy-manager-route | evidence_age_days | 69 | 70 |
| kf-policy-manager-route | age_days | 69 | 70 |
| kf-policy-manager-route | days_awaiting_my_approval | 69 | 70 |

### knowledge_gaps

- Fields: 127/128 (99.2%)
- Computed columns: name, is_open, open_gap_version_key, is_blocking, is_open_and_blocking, as_of_instant, days_open, tolerance_days, is_overdue_gap, owner_agent, owner_is_still_engaged, has_resolution_plan, is_abandoned_unknown, open_blocking_gap_version_key, owner_role_is_vacated, is_ownerless_open_gap

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gap-policy-delivery-receipts | days_open | 0 | 1 |

### fa_qs

- Fields: 3/3 (100.0%)
- Computed columns: name

### explanations

- Fields: 2/2 (100.0%)
- Computed columns: name

### procedure_executions

- Fields: 137/138 (99.3%)
- Computed columns: name, expected_step_count, completed_step_count, control_breach_count, late_step_count, is_structurally_complete, diverged_from_specification, all_blocking_controls_evaluated, unevaluated_blocking_total, separation_of_duties_held, separation_violation_count, is_attestation_ready, attestation_blocker_summary, executed_version_is_fit, signed_against_unfit_version, asserted_only_control_count, assurance_is_mostly_asserted, unreachable_handling_failure_count, retention_breach_count, cleared_legal_review_count, has_cleared_legal_review, abandoned_failure_count, delivered_count, total_delivery_attempt_count, has_abandoned_failures, mishandled_refusal_count, unclean_step_count, ran_clean, count_of_approval_executions, has_human_approval, count_of_delivery_executions, has_delivered, delivered_without_approval, invalid_approval_count, approval_chain_is_complete, vacuously_clean_step_count, preparation_step_count, approval_step_count, separation_was_testable, separation_held_under_test, separation_is_vacuously_green, separation_assurance_note, ungoverned_divergence_count, divergence_was_fully_governed, computedly_witnessed_control_count, evaluated_control_count, computed_assurance_ratio, interested_party_assertion_count, assurance_grade, attestation_would_be_weakly_based, independent_human_observation_count, has_any_independent_observation, self_attested_approval_count, assurance_chain_is_circular, latest_attestation_instant, has_been_attested, attestation_count, post_attestation_score_count, basis_changed_after_signature, requires_re_attestation, intended_recipient_count, reached_recipient_count, silently_dropped_count, delivery_yield_percent, campaign_silently_lost_audience, unrecorded_refusal_count, has_unrecorded_refusals, independently_confirmed_intent_count, send_decisions_are_entirely_self_witnessed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exec-policy-hr4821 | computed_assurance_ratio | 0.0 | 0E-20 |

### step_executions

- Fields: 1188/1188 (100.0%)
- Computed columns: name, actual_duration_minutes, expected_duration_minutes, is_late, blocking_unmet_count, blocking_unmet_count_safe, proceeded_past_blocking_control, expected_blocking_count, evaluated_blocking_count, unevaluated_blocking_count, has_unevaluated_blocking_control, stale_authoritative_source_count, ran_on_stale_authoritative_source, has_deviation_note, is_late_and_unexplained, available_exception_count_for_step, had_uninvoked_exception_available, expected_verification_count, performed_verification_count, skipped_verification_count, has_skipped_verification, claims_pass_without_evidence, step_is_preparation, step_is_approval, preparer_agent_key, approver_agent_key, prepared_by_this_agent_count, violates_separation_of_duties, required_role_for_step, executor_role_key, executor_authority_count, executor_held_required_role, is_unauthorized_approval, completed_execution_key, control_breach_execution_key, late_execution_key, executor_agent_kind, executor_is_human, step_requires_human_confirmation, non_human_ran_human_step, non_human_approval, unevaluated_blocking_execution_key, separation_violation_execution_key, self_witnessed_verification_count, unbacked_verification_count, approval_rests_on_self_attestation, exception_invocation_count, ran_under_exception, is_completed, is_verification_passed, is_legal_review_step, cleared_legal_review_key, assigned_role, role_current_agent, executor_is_designated_agent, inputs_were_fresh_at_run, ran_on_stale_inputs, unresolved_issue_count, has_deviation, is_clean, procedure_execution_when_unclean, evaluated_requirement_count, required_blocking_count, has_unevaluated_blocking_requirement, executing_agent_kind, was_executed_by_software, step_is_software_assigned, software_did_human_work, is_approval_execution, is_verified, unconfirmed_non_human_decision_count, requires_human_confirmation, human_confirmation_missing, drafted_from_unusable_source, inputs_were_usable, software_execution_step_key, step_control_kind, unfalsified_clearance_count, all_clearances_are_unfalsified, stale_at_run_count, was_stale_when_i_ran_it, staleness_answer_is_tense_dependent, has_any_declared_check, performed_check_count, declared_check_count, is_unchecked_by_design, is_vacuously_clean, is_substantively_clean, vacuously_clean_execution_key, uncorroborated_pass_count, evidence_position_is_weak, preparation_execution_key, approval_execution_key, has_governing_instrument, has_approved_change_coverage, version_of_step, is_ungoverned_divergence, ungoverned_divergence_execution_key, self_attested_approval_execution_key

### requirement_satisfactions

- Fields: 304/304 (100.0%)
- Computed columns: name, requirement_is_blocking, is_fully_satisfied, is_blocking_and_unmet, blocking_unmet_step_key, blocking_satisfaction_step_key, negative_outcome_requirement_key, evaluator_agent_kind, non_human_evaluated_human_control, requirement_has_computed_witness, is_asserted_only, asserted_only_execution_key, parent_procedure_execution, step_execution_when_scored, is_human_evaluated, requirement_is_approval_type, is_invalid_approval, procedure_execution_of_satisfaction, run_when_invalid_approval, requirement_is_unfalsified, is_clearance_by_unfalsified_control, unfalsified_clearance_step_key, spec_step_of_execution, binding_key, scored_step_executor_agent, evaluator_is_step_executor, run_owner_agent, evaluator_owns_the_run, is_interested_party_assertion, has_written_evidence, is_bare_assertion, interested_assertion_execution_key, is_computedly_witnessed, computed_witness_execution_key, step_executor_agent, was_scored_after_attestation, attestation_instant_for_run, post_attestation_score_execution_key

### errors

- Fields: 2/2 (100.0%)
- Computed columns: name

### issue_occurrences

- Fields: 4/6 (66.7%)
- Computed columns: name, is_unresolved, step_execution_when_unresolved

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| issue-close-feed | name | err-feed-stale @ 2026-06-30T17 | err-feed-stale @ 2026-06-30 17 |
| issue-policy-sms | name | err-sms-throttle @ 2026-07-19T | err-sms-throttle @ 2026-07-19  |

### user_questions

- Fields: 2/2 (100.0%)
- Computed columns: name

### user_feedback

- Fields: 2/2 (100.0%)
- Computed columns: name

### stewardship_assignments

- Fields: 10/10 (100.0%)
- Computed columns: name, count_of_review_events, has_ever_been_reviewed, as_of_instant, is_current_assignment

### change_requests

- Fields: 62/64 (96.9%)
- Computed columns: name, is_open, open_change_version_key, is_decided, as_of_instant, days_pending, is_still_pending, is_stalled, authority_agent, requester_is_authority, awaits_authority_decision, authority_role_label, touches_live_version, is_live_decision_backlog, blocks_an_open_gap, backlog_version_key, is_my_pending_decision, is_my_blocking_backlog, is_my_overdue_backlog, is_implemented, is_my_decided_request, is_my_decided_but_unlanded, decision_latency_days, implementation_latency_days, delay_is_downstream_of_me, unlanded_version_key, is_approved_not_implemented, days_since_approval, is_stalled_implementation, stalled_implementation_version_key, approved_version_key, is_approved_decision

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cr-close-timestamp | days_pending | 0 | 1 |
| cr-close-timestamp | decision_latency_days | 0 | 1 |

### review_events

- Fields: 24/30 (80.0%)
- Computed columns: name, as_of_instant, is_overdue, overdue_version_key, promised_cadence_days, days_since_reviewed, exceeds_promised_cadence, cadence_drift_days, promise_and_behavior_disagree, cadence_breach_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| review-close-q1 | days_since_reviewed | 120 | 121 |
| review-close-q1 | cadence_drift_days | 30.0 | 31 |
| review-close-q2 | days_since_reviewed | 16 | 17 |
| review-close-q2 | cadence_drift_days | -74.0 | -73 |
| review-policy-prelaunch | days_since_reviewed | 0 | 1 |
| review-policy-prelaunch | cadence_drift_days | -60.0 | -59 |

### learning_activities

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| learn-close-retro | name | Retrospective / 2026-07-02T15: | Retrospective / 2026-07-02 15: |
| learn-policy-tabletop | name | TabletopExercise / 2026-07-15T | TabletopExercise / 2026-07-15  |

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

- Fields: 47/47 (100.0%)
- Computed columns: name

### witness_loops

- Fields: 9/9 (100.0%)
- Computed columns: name, question_count, is_complete

### role_questions

- Fields: 327/327 (100.0%)
- Computed columns: name, predicate_count, is_answered

### rulebook_fields

- Fields: 9395/9395 (100.0%)
- Computed columns: name, is_derived, is_witness, disagreeing_substrate_count, is_substrate_contested

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

- Fields: 0/10 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| obs-close-01-02-q2 | name | close-01-to-close-02 @ 2026-06 | close-01-to-close-02 @ 2026-06 |
| obs-close-02-03-q2 | name | close-02-to-close-03 @ 2026-06 | close-02-to-close-03 @ 2026-06 |
| obs-close-03-04-q2 | name | close-03-to-close-04 @ 2026-07 | close-03-to-close-04 @ 2026-07 |
| obs-close-04-05-q2 | name | close-04-to-close-05 @ 2026-07 | close-04-to-close-05 @ 2026-07 |
| obs-close-05-06-q2 | name | close-05-to-close-06 @ 2026-07 | close-05-to-close-06 @ 2026-07 |
| obs-close-06-07-q2 | name | close-06-to-close-07 @ 2026-07 | close-06-to-close-07 @ 2026-07 |
| obs-close-07-08-q2 | name | close-07-to-close-08 @ 2026-07 | close-07-to-close-08 @ 2026-07 |
| obs-policy-01-02-hr4821 | name | policy-01-to-policy-02 @ 2026- | policy-01-to-policy-02 @ 2026- |
| obs-policy-02-03-hr4821 | name | policy-02-to-policy-03 @ 2026- | policy-02-to-policy-03 @ 2026- |
| obs-policy-03-04-hr4821 | name | policy-03-to-policy-04 @ 2026- | policy-03-to-policy-04 @ 2026- |

### recipients

- Fields: 30/30 (100.0%)
- Computed columns: name, has_sms_consent, is_email_reachable, is_sms_reachable, is_unreachable, is_communicationally_stranded

### message_deliveries

- Fields: 420/444 (94.6%)
- Computed columns: name, policy_channel, channel_name, policy_requires_consent, recipient_has_sms_consent, was_actually_transmitted, is_consent_violation, consent_violation_policy_key, policy_quiet_hours_start_hour, policy_quiet_hours_end_hour, policy_has_quiet_hours, quiet_window_wraps_midnight, is_inside_quiet_window, is_quiet_hours_violation, quiet_hours_violation_policy_key, recipient_is_unreachable, is_acknowledged, invoked_exception_condition, has_unreachable_exception_invoked, is_fabricated_acknowledgement, is_unhandled_unreachable, unreachable_failure_key, policy_retention_days, as_of_instant, age_days, is_within_retention_window, has_rendered_body, is_evidence_required, is_retention_breach, retention_breach_execution_key, sending_step_execution_step, execution_has_cleared_legal_review, is_unreviewed_send, rendered_body_length, policy_max_message_length_at_send, segment_count, policy_max_segments_at_send, is_over_segment_limit, template_has_valid_approval, is_unapproved_send, policy_required_opt_out_phrase, policy_requires_opt_out, opt_out_phrase_position, has_opt_out_phrase, is_opt_out_in_first_segment, is_missing_required_opt_out, is_opt_out_at_risk_of_truncation, is_failed_delivery, is_suppressed, is_triaged, is_abandoned_failure, abandoned_failure_execution_key, reached_execution_key, template_was_sendable, is_drifted_send, drifted_send_template_key, was_sent_outside_business_hours, was_delivered_and_unanswered, is_poorly_timed_unanswered, is_well_timed_unanswered, unanswered_template_key, transmitted_template_key, approval_preceded_send, has_frozen_approval_evidence, provenance_is_live_derived, current_last_approval_at, template_reapproved_since_send, is_unprovable_approval_claim, has_sent_reminder, acknowledgement_is_outstanding, outstanding_age_days, is_unchased_acknowledgement, is_exhausted_follow_up, needs_human_escalation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| md-001 | name | rec-001 / tmpl-policy-email /  | rec-001 / tmpl-policy-email /  |
| md-001 | age_days | 0 | 1 |
| md-001 | segment_count | None | 1 |
| md-002 | name | rec-002 / tmpl-policy-email /  | rec-002 / tmpl-policy-email /  |
| md-002 | age_days | 0 | 1 |
| md-002 | segment_count | None | 1 |
| md-002 | outstanding_age_days | 0 | 1 |
| md-003 | name | rec-003 / tmpl-policy-sms / 20 | rec-003 / tmpl-policy-sms / 20 |
| md-003 | age_days | 0 | 1 |
| md-003 | segment_count | None | 1 |
| md-003 | opt_out_phrase_position | True | 86 |
| md-003 | outstanding_age_days | 0 | 1 |
| md-004 | name | rec-004 / tmpl-policy-sms / 20 | rec-004 / tmpl-policy-sms / 20 |
| md-004 | age_days | 0 | 1 |
| md-004 | segment_count | None | 1 |
| md-004 | opt_out_phrase_position | True | 86 |
| md-005 | name | rec-001 / tmpl-policy-sms / 20 | rec-001 / tmpl-policy-sms / 20 |
| md-005 | age_days | 0 | 1 |
| md-005 | segment_count | None | 1 |
| md-005 | outstanding_age_days | 0 | 1 |
| ... | ... | (4 more) | ... |

### template_approvals

- Fields: 10/12 (83.3%)
- Computed columns: name, is_approval_decision, template_policy, required_approval_role, is_decided_by_required_role, valid_approval_template_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tap-email-001 | name | tmpl-policy-email / Approved / | tmpl-policy-email / Approved / |
| tap-sms-001 | name | tmpl-policy-sms / Approved / 2 | tmpl-policy-sms / Approved / 2 |

### send_intents

- Fields: 546/553 (98.7%)
- Computed columns: name, intent_policy, intent_channel, policy_is_active, intent_requires_consent, recipient_has_channel_consent, consent_gate_passed, recipient_is_sms_reachable, recipient_is_email_reachable, reachability_gate_passed, permission_gate_passed, intent_quiet_start_hour, intent_quiet_end_hour, intent_policy_has_quiet_hours, intent_quiet_window_wraps, intent_is_inside_quiet_window, timing_gate_passed, hours_until_window_opens, intent_max_message_length, intent_max_segments, length_gate_passed, intent_required_opt_out_phrase, opt_out_gate_passed, content_gate_passed, template_is_sendable, execution_has_legal_clearance, intent_approval_role, approval_role_agent_kind, approval_is_human, authorization_gate_passed, is_cleared_to_send, blocking_gate_name, has_resulting_delivery, resulting_delivery_was_transmitted, is_overridden_refusal, is_silently_dropped, resulting_delivery_exception, refusal_cited_an_exception, is_properly_handled_refusal, refusal_failure_execution_key, intent_execution_key, delivered_intent_execution_key, dropped_intent_execution_key, my_approval_was_in_force, refused_on_approved_content, refused_on_opt_out_only, refusal_was_on_my_rules, refusal_was_outside_my_control, is_unreported_refusal_on_my_rules, is_approval_overridden_silently, has_alternate_channel_attempt, alternate_attempt_was_cleared, is_refused_with_no_alternative, exception_prescribed_an_alternative, prescribed_handling_was_performed, is_suppression_without_remedy, has_durable_refusal_record, refusal_was_escalated, is_unrecorded_refusal, is_unescalated_refusal, unescalated_refusal_role_key, unrecorded_refusal_execution_key, was_deferred_on_timing, as_of_instant, window_has_since_reopened, has_retry_attempt, retry_was_cleared, is_abandoned_deferral, deferral_age_hours, is_stale_deferral, enforced_by_unauthorized_agent, consent_input_was_resolvable, recipient_consent_status_raw, policy_input_was_resolvable, all_gate_inputs_resolved, is_unevaluable_refusal, is_self_witnessed_decision, is_independently_confirmed, independently_confirmed_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| si-001 | deferral_age_hours | 22 | 23 |
| si-002 | deferral_age_hours | 22 | 23 |
| si-003 | deferral_age_hours | 22 | 23 |
| si-004 | deferral_age_hours | 22 | 23 |
| si-005 | deferral_age_hours | 22 | 23 |
| si-006 | deferral_age_hours | 22 | 23 |
| si-007 | deferral_age_hours | 22 | 23 |

### agent_decision_records

- Fields: 81/81 (100.0%)
- Computed columns: name, was_overridden, was_reviewed, deciding_agent_kind, deciding_agent_when_overridden, role_assignment_when_scored, role_assignment_when_overridden, step_of_decision, boundary_match_key, matching_boundary_count, violated_authority_boundary, reviewer_agent_kind, has_human_confirmation, needs_human_confirmation, is_unconfirmed_non_human_decision, step_execution_when_unconfirmed, agent_when_boundary_violated, review_latency_minutes, is_draft_kind, agent_when_draft_overridden, agent_when_draft, is_error_correction, is_reserved_judgment_override, override_reason_is_recorded, is_unexplained_override, error_correction_role_assignment_key, boundary_violation_role_assignment_key

### delivered_communications

- Fields: 5/6 (83.3%)
- Computed columns: name, has_authorization, content_matches_approval, authorized_at, was_approved_before_sending, is_defensible

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dc-hr4821-email-001 | content_matches_approval | True | None |

### authority_boundaries

- Fields: 63/63 (100.0%)
- Computed columns: name, as_of_instant, is_currently_binding, ratifying_fragment_is_valid, step_when_binding, boundary_match_key, violation_count, is_untested, has_ratifying_fragment, is_unwarranted, ratifying_fragment_is_overdue, ratifying_fragment_is_single_witness, warrant_is_thin, is_unwarranted_and_untested, unwarranted_boundary_step_key, ratifying_fragment_key, ratifying_fragment_status, ratification_lapsed, binds_despite_lapsed_ratification, is_ungrounded_and_untested, constrained_role_assignment_key

### app_role_profiles

- Fields: 24/24 (100.0%)
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

- Fields: 460/460 (100.0%)
- Computed columns: name, field_count, policy_count, is_unsecured, disagreeing_substrate_count

### access_principals

- Fields: 96/96 (100.0%)
- Computed columns: name, organization_scope, role_label, policy_count, grant_count, visible_table_count, has_no_access, is_over_privileged

### access_policies

- Fields: 1414/1414 (100.0%)
- Computed columns: name, is_write_command, is_unrestricted, principal_is_admin, is_unrestricted_non_admin_grant, is_unwitnessed_write, denial_test_count

### field_grants

- Fields: 25473/25473 (100.0%)
- Computed columns: name, field_table, field_name, field_is_derived, is_writable_derived_field, is_masked, grant_key_when_readable

### role_schemas

- Fields: 48/48 (100.0%)
- Computed columns: name, search_path, view_count, is_empty_schema

### role_schema_views

- Fields: 1616/1616 (100.0%)
- Computed columns: name, schema_name, source_view, grant_key, column_count, table_field_count, is_full_width, is_degenerate_view

### jwt_claim_mappings

- Fields: 8/8 (100.0%)
- Computed columns: name, usage_count

### access_denial_tests

- Fields: 102/102 (100.0%)
- Computed columns: name, has_run, is_passing, is_leak, is_unproven, is_positive_control

### app_users

- Fields: 70/70 (100.0%)
- Computed columns: name, agent_kind, organization, assignment_count, has_no_principal, holds_multiple_principals, is_non_human_sign_in

### principal_assignments

- Fields: 60/60 (100.0%)
- Computed columns: name, principal_is_admin, user_organization, principal_organization, is_cross_organization_grant

### process_mining_runs

- Fields: 36/40 (90.0%)
- Computed columns: name, as_of_instant, conformance_rate, is_conformant, has_major_drift_from_documentation, days_since_mined, is_stale_mining_evidence, procedure_version_is_live, is_drift_on_live_version, drifted_mining_run_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pmr-close-v10-archived | name | ERP General Ledger Audit Log / | ERP General Ledger Audit Log / |
| pmr-close-v11-cutoff-bypass | name | ERP General Ledger Audit Log / | ERP General Ledger Audit Log / |
| pmr-close-v11-q3 | name | ERP General Ledger Audit Log / | ERP General Ledger Audit Log / |
| pmr-policy-v1-notify | name | Notification Pipeline Delivery | Notification Pipeline Delivery |

### vocabularies

- Fields: 8/8 (100.0%)
- Computed columns: name, term_count, orphan_term_count, has_orphan_terms

### vocabulary_terms

- Fields: 60/60 (100.0%)
- Computed columns: name, usage_count, is_orphan_term, is_widely_adopted_term, orphan_term_vocabulary_key

### knowledge_broker_links

- Fields: 40/40 (100.0%)
- Computed columns: name, as_of_instant, days_since_consulted, is_active_reliance, broker_is_still_engaged, is_at_risk_reliance, active_reliance_broker_key, at_risk_broker_key
