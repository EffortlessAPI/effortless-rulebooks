# Test Results: effortless-xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 59878 |
| Passed | 53868 |
| Failed | 6010 |
| Score | 90.0% |
| Duration | 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 40792 | 42928 | 95.0% |
| Lookup (INDEX/MATCH) | 9334 | 12929 | 72.2% |
| Aggregation (COUNTIFS/SUMIFS) | 3742 | 4021 | 93.1% |

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

- Fields: 192/204 (94.1%)
- Computed columns: name, current_agent_kind, active_assignment_count, currently_covered_assignment_count, has_no_current_holder, count_of_awaited_decisions, current_assignment_valid_from, is_non_human_held, is_ungoverned_non_human_role, departed_assignment_count, has_lost_a_holder, is_vacated_role, ungrounded_boundary_count, is_governed_by_lapsed_authority, unescalated_refusal_count, unauthorized_enforcement_assignment_count, is_ungoverned_enforcement_role

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cfo | current_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| close-automation | current_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| communications-manager | current_assignment_valid_from | 2026-02-01T00:00:00-06:00 | 2026-02-01 06:00:00 |
| controller | current_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| employment-counsel | current_assignment_valid_from | 2026-01-15T00:00:00-06:00 | 2026-01-15 06:00:00 |
| finance-analyst | current_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| hr-policy-owner | current_assignment_valid_from | 2026-02-01T00:00:00-06:00 | 2026-02-01 06:00:00 |
| knowledge-authority | current_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| notification-publisher | current_assignment_valid_from | 2026-03-01T00:00:00-06:00 | 2026-03-01 06:00:00 |
| policy-drafting-agent | current_assignment_valid_from | 2026-05-01T00:00:00-05:00 | 2026-05-01 05:00:00 |
| process-steward | current_assignment_valid_from | 2026-04-15T09:00:00-05:00 | 2026-04-15 14:00:00 |
| variance-review-agent | current_assignment_valid_from | 2026-04-01T00:00:00-05:00 | 2026-04-01 05:00:00 |

### role_assignments

- Fields: 527/611 (86.3%)
- Computed columns: name, as_of_instant, is_current, current_agent_key, is_currently_valid, agent_role_key, has_departed, covers_now, role_when_covering, agent_kind, is_non_human_assignment, predecessor_agent_kind, is_human_to_non_human_handover, is_unauthorized_non_human_assignment, was_authorized_by_change_request, decision_count, overridden_decision_count, override_rate_percent, predecessor_override_rate_percent, quality_regressed_vs_predecessor, departed_role_key, predecessor_decision_count, has_sufficient_sample, predecessor_has_sufficient_sample, comparison_is_evidentially_sound, single_override_swing_percent, quality_verdict_is_unsupported, is_unmeasured_automation_handover, error_correction_count, error_rate_percent, has_dated_authorization, days_since_authorization_review, authorization_is_overdue_for_review, is_standing_unreviewed_automation, is_unconditioned_automation_handover, exceeds_tolerable_error_rate, boundary_violation_count_for_assignment, has_any_boundary_violation, has_ungrounded_governing_boundary, suspension_condition_met, is_operating_under_met_suspension_condition, has_declared_suspension_condition, has_approving_authority, has_authorizing_change_request, is_unauthorized_enforcement_agent, governance_evidence_count, unauthorized_enforcement_role_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ra-authority-2026 | name | knowledge-authority @ 2026-01- | knowledge-authority @ 2026-01- |
| ra-authority-2026 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| ra-authority-2026 | has_sufficient_sample | False | True |
| ra-authority-2026 | predecessor_has_sufficient_sample | False | True |
| ra-authority-2026 | comparison_is_evidentially_sound | False | True |
| ra-authority-2026 | quality_verdict_is_unsupported | True | False |
| ra-cfo-2026 | name | cfo @ 2026-01-01T00:00:00-06:0 | cfo @ 2026-01-01 06:00:00+00 |
| ra-cfo-2026 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| ra-cfo-2026 | has_sufficient_sample | False | True |
| ra-cfo-2026 | predecessor_has_sufficient_sample | False | True |
| ra-cfo-2026 | comparison_is_evidentially_sound | False | True |
| ra-cfo-2026 | quality_verdict_is_unsupported | True | False |
| ra-close-pipeline-2026 | name | close-automation @ 2026-01-01T | close-automation @ 2026-01-01  |
| ra-close-pipeline-2026 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| ra-close-pipeline-2026 | has_sufficient_sample | False | True |
| ra-close-pipeline-2026 | predecessor_has_sufficient_sample | False | True |
| ra-close-pipeline-2026 | comparison_is_evidentially_sound | False | True |
| ra-close-pipeline-2026 | quality_verdict_is_unsupported | True | False |
| ra-close-pipeline-2026 | is_unauthorized_enforcement_agent | False | True |
| ra-comms-2026 | name | communications-manager @ 2026- | communications-manager @ 2026- |
| ... | ... | (64 more) | ... |

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

- Fields: 204/216 (94.4%)
- Computed columns: name, count_of_steps, count_of_open_knowledge_gaps, is_ready_for_execution, specified_step_count, overdue_review_count, open_change_request_count, open_high_severity_gap_count, is_fit_to_execute, steward_review_cadence_days, count_of_stewardship_assignments, has_any_steward, is_live, is_unstewarded, is_live_and_unstewarded, count_of_open_blocking_gaps, has_open_blocking_gap, is_live_with_blocking_gap, should_not_be_executable, count_of_unapproved_reliance_fragments, runs_on_unapproved_knowledge, count_of_overdue_gaps, count_of_change_requests, count_of_review_events, has_governance_record, as_of_instant, days_since_modified, days_since_last_review, was_modified_since_last_review, modifier_is_authority, has_unwitnessed_change, count_of_stale_fragments, knowledge_is_staler_than_cadence, compound_fragile_fragment_count, rests_on_compound_fragile_knowledge, concentrated_witness_session_count, knowledge_base_is_concentrated, machine_consumed_unapproved_count, feeds_unapproved_knowledge_to_machines, genuinely_overdue_fragment_count, awaited_decision_count, scoped_open_blocking_gap_count, is_blocked_on_pending_decision, unexercised_human_gate_count, ai_boundary_is_unevidenced, load_bearing_unapproved_count, unlanded_decision_count, unrehearsed_control_entry_count, has_unrehearsed_control_entry, is_live_with_unrehearsed_control, cadence_breach_count, is_in_cadence_breach, has_decision_in_flight, is_unremediated_cadence_breach, is_managed_cadence_breach, governance_is_silent, valid_fragment_count, still_owns_valid_knowledge, incoming_supersession_count, is_still_referenced, is_load_bearing_orphan, is_cleanly_retired, stalled_implementation_count, is_held_unfit_by_landed_decisions, undeclared_control_kind_count, control_taxonomy_is_incomplete, has_approved_change_request, approved_change_request_count, unwatched_unowned_control_count, mining_run_count, drifted_mining_run_count, has_unresolved_mining_drift

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-v1.0.0 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| close-v1.0.0 | days_since_modified | 109 | 110 |
| close-v1.0.0 | days_since_last_review | None | 46222 |
| close-v1.0.0 | was_modified_since_last_review | False | True |
| close-v1.1.0 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| close-v1.1.0 | days_since_modified | 16 | 17 |
| close-v1.1.0 | was_modified_since_last_review | True | False |
| close-v1.1.0 | has_unwitnessed_change | True | False |
| policy-v1.0.0 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
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

- Fields: 490/510 (96.1%)
- Computed columns: name, assigned_role_label, assigned_agent_kind, blocking_requirement_count, stale_binding_count, authoritative_stale_count, available_exception_count, declared_verification_count, is_preparation_step, is_approval_step, stale_authoritative_binding_count, inputs_are_fresh, is_software_assigned, is_human_approval_gate, gate_held_by_human, binding_boundary_count, assigned_role_is_ungoverned, unusable_binding_count, all_sources_usable, unwarranted_boundary_count, is_governed_by_unwarranted_boundary, software_execution_count, has_been_approached_by_software, is_unexercised_human_gate, is_demonstrated_human_gate, unexercised_gate_version_key, has_declared_control_kind, undeclared_control_version_key, approval_step_is_software_assigned, unwitnessed_blocking_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| close-02 | stale_binding_count | 1 | 0 |
| close-02 | authoritative_stale_count | 1 | 0 |
| close-02 | stale_authoritative_binding_count | 1 | 0 |
| close-02 | inputs_are_fresh | False | True |
| close-02 | unusable_binding_count | 1 | 0 |
| close-02 | all_sources_usable | False | True |
| close-06 | unwarranted_boundary_count | 1 | 0 |
| close-06 | is_governed_by_unwarranted_boundary | True | False |
| policy-01 | stale_binding_count | 1 | 0 |
| policy-01 | authoritative_stale_count | 1 | 0 |
| policy-01 | stale_authoritative_binding_count | 1 | 0 |
| policy-01 | inputs_are_fresh | False | True |
| policy-01 | unusable_binding_count | 1 | 0 |
| policy-01 | all_sources_usable | False | True |
| policy-07 | stale_binding_count | 1 | 0 |
| policy-07 | authoritative_stale_count | 1 | 0 |
| policy-07 | stale_authoritative_binding_count | 1 | 0 |
| policy-07 | inputs_are_fresh | False | True |
| policy-07 | unusable_binding_count | 3 | 0 |
| policy-07 | all_sources_usable | False | True |

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

- Fields: 379/403 (94.0%)
- Computed columns: name, satisfaction_record_count, step_binding_count, is_bound_to_any_step, has_ever_been_evaluated, negative_outcome_count, is_inoperative_control, is_decorative_control, has_ever_produced_negative, is_unfalsified_control, claims_a_witness_field, named_witness_field_exists, derived_has_computed_witness, witness_claim_is_unverified, is_unwitnessed_blocking_control, witness_fire_count, witness_has_never_fired, evaluation_sample_size, has_meaningful_sample, is_untested_witness, is_evidenced_holding_control, control_assurance_state, unexercised_binding_count, witness_is_partially_scoped, accountable_agent, has_named_owner, is_orphaned_blocking_control, is_unwatched_and_unowned, attestation_exposure_note, unwatched_unowned_flag, uses_controlled_vocabulary

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| req-close-balance | named_witness_field_exists | True | None |
| req-close-balance | has_meaningful_sample | False | True |
| req-close-balance | is_untested_witness | True | False |
| req-close-balance | is_evidenced_holding_control | False | True |
| req-close-balance | control_assurance_state | Untested | Holding |
| req-close-cutoff | named_witness_field_exists | True | None |
| req-close-cutoff | has_meaningful_sample | False | True |
| req-close-cutoff | is_untested_witness | True | False |
| req-close-cutoff | is_evidenced_holding_control | False | True |
| req-close-cutoff | control_assurance_state | Untested | Holding |
| req-close-evidence | has_meaningful_sample | False | True |
| req-close-human-approval | has_meaningful_sample | False | True |
| req-close-separation | has_meaningful_sample | False | True |
| req-close-separation | is_untested_witness | True | False |
| req-close-separation | is_evidenced_holding_control | False | True |
| req-close-separation | control_assurance_state | Untested | Holding |
| req-policy-accessibility | has_meaningful_sample | False | True |
| req-policy-consent | has_meaningful_sample | False | True |
| req-policy-human-approval | has_meaningful_sample | False | True |
| req-policy-legal | has_meaningful_sample | False | True |
| ... | ... | (4 more) | ... |

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

- Fields: 22/30 (73.3%)
- Computed columns: name, as_of_instant, days_since_elicited, is_single_witness_method, practitioner_is_still_engaged, valid_fragments_produced, is_high_yield_session, is_concentrated_single_witness, is_stale_concentrated_witness, concentrated_session_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| elicit-close-shadow | name | Shadowing / 2026-04-03T09:00:0 | Shadowing / 2026-04-03 14:00:0 |
| elicit-close-shadow | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| elicit-policy-interview | name | PractitionerInterview / 2026-0 | PractitionerInterview / 2026-0 |
| elicit-policy-interview | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| elicit-policy-interview | days_since_elicited | 67 | 68 |
| elicit-policy-workshop | name | FacilitatedWorkshop / 2026-05- | FacilitatedWorkshop / 2026-05- |
| elicit-policy-workshop | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| elicit-policy-workshop | days_since_elicited | 69 | 70 |

### knowledge_fragments

- Fields: 426/455 (93.6%)
- Computed columns: name, as_of_instant, is_currently_valid, source_agent_is_still_engaged, source_agent_kind, has_human_source, has_orphaned_provenance, is_undefendable_tacit_claim, is_approved, is_within_validity_window, is_relied_upon, step_procedure_version_status, is_attached_to_live_version, is_unapproved_but_relied_on, evidence_age_days, has_recorded_elicitation, is_from_single_witness, evidence_expiry_days, evidence_has_expired, owner_agent, is_awaiting_approval, owner_is_me, is_my_unfinished_approval, is_invoked_by_an_exception, has_operational_reliance, is_unapproved_and_operationally_live, age_days, is_low_confidence, owning_version_cadence_days, exceeds_owning_cadence, is_aging_low_confidence_claim, owner_role_agent_kind, is_human_owned, is_ai_validated_by_ai, review_cadence_days, is_overdue_for_review, predates_current_role_holder, owner_role_assignment_valid_from, fragility_signal_count, is_compound_fragile, is_single_point_of_failure, is_expiring_single_point_of_failure, compound_fragile_version_key, valid_fragment_session_key, consuming_step_is_software_assigned, consuming_step_agent_kind, is_unapproved_and_machine_consumed, is_unapproved_and_human_consumed, machine_consumed_unapproved_version_key, has_review_record, days_since_actual_review, is_unreviewed_since_authoring, is_genuinely_overdue, review_recency_is_inferred, inference_disagrees_with_record, genuinely_overdue_version_key, ratified_boundary_count, reliance_surface_count, days_awaiting_my_approval, is_high_blast_radius_unapproved, is_long_unapproved, unapproved_load_bearing_version_key, owner_role_is_vacated, is_orphaned_by_role, valid_fragment_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kf-close-fx-time | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kf-close-fx-time | owner_role_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| kf-close-fx-time | days_since_actual_review | 16 | 17 |
| kf-close-judgment | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kf-close-judgment | owner_role_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| kf-close-retention | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kf-close-retention | evidence_expiry_days | 365 | #VALUE! |
| kf-close-retention | evidence_has_expired | False | #VALUE! |
| kf-close-retention | owner_role_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| kf-close-retention | fragility_signal_count | 1 | #VALUE! |
| kf-close-retention | is_compound_fragile | False | #VALUE! |
| kf-close-retention | compound_fragile_version_key | None | #VALUE! |
| kf-close-top-ten | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kf-close-top-ten | owner_role_assignment_valid_from | 2026-01-01T00:00:00-06:00 | 2026-01-01 06:00:00 |
| kf-close-top-ten | days_since_actual_review | 16 | 17 |
| kf-policy-ai-boundary | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kf-policy-ai-boundary | evidence_age_days | 67 | 68 |
| kf-policy-ai-boundary | age_days | 67 | 68 |
| kf-policy-ai-boundary | owner_role_assignment_valid_from | 2026-01-15T00:00:00-06:00 | 2026-01-15 06:00:00 |
| kf-policy-channel | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| ... | ... | (9 more) | ... |

### knowledge_gaps

- Fields: 119/128 (93.0%)
- Computed columns: name, is_open, open_gap_version_key, is_blocking, is_open_and_blocking, as_of_instant, days_open, tolerance_days, is_overdue_gap, owner_agent, owner_is_still_engaged, has_resolution_plan, is_abandoned_unknown, open_blocking_gap_version_key, owner_role_is_vacated, is_ownerless_open_gap

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gap-close-cadence-unmeasured | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-close-dual-outage | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-close-sod-unwitnessed | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-policy-consent-unenforced | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-policy-delivery-receipts | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-policy-delivery-receipts | days_open | 0 | 1 |
| gap-policy-quiet-hours-unenforced | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-policy-unauthorized-sends | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| gap-roles-unauditable-holders | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |

### fa_qs

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| faq-close-workpapers | name | Where are close workpapers sto | None |
| faq-policy-ai | name | Can the drafting AI approve a  | None |
| faq-policy-optout | name | What happens when an employee  | None |

### explanations

- Fields: 2/2 (100.0%)
- Computed columns: name

### procedure_executions

- Fields: 125/138 (90.6%)
- Computed columns: name, expected_step_count, completed_step_count, control_breach_count, late_step_count, is_structurally_complete, diverged_from_specification, all_blocking_controls_evaluated, unevaluated_blocking_total, separation_of_duties_held, separation_violation_count, is_attestation_ready, attestation_blocker_summary, executed_version_is_fit, signed_against_unfit_version, asserted_only_control_count, assurance_is_mostly_asserted, unreachable_handling_failure_count, retention_breach_count, cleared_legal_review_count, has_cleared_legal_review, abandoned_failure_count, delivered_count, total_delivery_attempt_count, has_abandoned_failures, mishandled_refusal_count, unclean_step_count, ran_clean, count_of_approval_executions, has_human_approval, count_of_delivery_executions, has_delivered, delivered_without_approval, invalid_approval_count, approval_chain_is_complete, vacuously_clean_step_count, preparation_step_count, approval_step_count, separation_was_testable, separation_held_under_test, separation_is_vacuously_green, separation_assurance_note, ungoverned_divergence_count, divergence_was_fully_governed, computedly_witnessed_control_count, evaluated_control_count, computed_assurance_ratio, interested_party_assertion_count, assurance_grade, attestation_would_be_weakly_based, independent_human_observation_count, has_any_independent_observation, self_attested_approval_count, assurance_chain_is_circular, latest_attestation_instant, has_been_attested, attestation_count, post_attestation_score_count, basis_changed_after_signature, requires_re_attestation, intended_recipient_count, reached_recipient_count, silently_dropped_count, delivery_yield_percent, campaign_silently_lost_audience, unrecorded_refusal_count, has_unrecorded_refusals, independently_confirmed_intent_count, send_decisions_are_entirely_self_witnessed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| exec-close-2026-q2 | late_step_count | 5 | 0 |
| exec-close-2026-q2 | unclean_step_count | 5 | 0 |
| exec-close-2026-q2 | ran_clean | False | True |
| exec-close-2026-q2 | vacuously_clean_step_count | 1 | 0 |
| exec-close-2026-q2 | latest_attestation_instant | None | 00:00:00 |
| exec-policy-hr4821 | late_step_count | 2 | 0 |
| exec-policy-hr4821 | mishandled_refusal_count | 4 | 5 |
| exec-policy-hr4821 | unclean_step_count | 2 | 0 |
| exec-policy-hr4821 | ran_clean | False | True |
| exec-policy-hr4821 | vacuously_clean_step_count | 1 | 0 |
| exec-policy-hr4821 | ungoverned_divergence_count | 2 | 0 |
| exec-policy-hr4821 | divergence_was_fully_governed | False | True |
| exec-policy-hr4821 | latest_attestation_instant | None | 00:00:00 |

### step_executions

- Fields: 1030/1188 (86.7%)
- Computed columns: name, actual_duration_minutes, expected_duration_minutes, is_late, blocking_unmet_count, blocking_unmet_count_safe, proceeded_past_blocking_control, expected_blocking_count, evaluated_blocking_count, unevaluated_blocking_count, has_unevaluated_blocking_control, stale_authoritative_source_count, ran_on_stale_authoritative_source, has_deviation_note, is_late_and_unexplained, available_exception_count_for_step, had_uninvoked_exception_available, expected_verification_count, performed_verification_count, skipped_verification_count, has_skipped_verification, claims_pass_without_evidence, step_is_preparation, step_is_approval, preparer_agent_key, approver_agent_key, prepared_by_this_agent_count, violates_separation_of_duties, required_role_for_step, executor_role_key, executor_authority_count, executor_held_required_role, is_unauthorized_approval, completed_execution_key, control_breach_execution_key, late_execution_key, executor_agent_kind, executor_is_human, step_requires_human_confirmation, non_human_ran_human_step, non_human_approval, unevaluated_blocking_execution_key, separation_violation_execution_key, self_witnessed_verification_count, unbacked_verification_count, approval_rests_on_self_attestation, exception_invocation_count, ran_under_exception, is_completed, is_verification_passed, is_legal_review_step, cleared_legal_review_key, assigned_role, role_current_agent, executor_is_designated_agent, inputs_were_fresh_at_run, ran_on_stale_inputs, unresolved_issue_count, has_deviation, is_clean, procedure_execution_when_unclean, evaluated_requirement_count, required_blocking_count, has_unevaluated_blocking_requirement, executing_agent_kind, was_executed_by_software, step_is_software_assigned, software_did_human_work, is_approval_execution, is_verified, unconfirmed_non_human_decision_count, requires_human_confirmation, human_confirmation_missing, drafted_from_unusable_source, inputs_were_usable, software_execution_step_key, step_control_kind, unfalsified_clearance_count, all_clearances_are_unfalsified, stale_at_run_count, was_stale_when_i_ran_it, staleness_answer_is_tense_dependent, has_any_declared_check, performed_check_count, declared_check_count, is_unchecked_by_design, is_vacuously_clean, is_substantively_clean, vacuously_clean_execution_key, uncorroborated_pass_count, evidence_position_is_weak, preparation_execution_key, approval_execution_key, has_governing_instrument, has_approved_change_coverage, version_of_step, is_ungoverned_divergence, ungoverned_divergence_execution_key, self_attested_approval_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| se-close01 | actual_duration_minutes | 4 | #NAME? |
| se-close01 | is_late | False | #NAME? |
| se-close01 | is_late_and_unexplained | False | #NAME? |
| se-close01 | had_uninvoked_exception_available | False | #NAME? |
| se-close01 | late_execution_key | None | #NAME? |
| se-close01 | is_clean | True | #NAME? |
| se-close01 | procedure_execution_when_unclean | None | #NAME? |
| se-close01 | is_vacuously_clean | False | #NAME? |
| se-close01 | is_substantively_clean | True | #NAME? |
| se-close01 | vacuously_clean_execution_key | None | #NAME? |
| se-close01 | is_ungoverned_divergence | False | #NAME? |
| se-close01 | ungoverned_divergence_execution_key | None | #NAME? |
| se-close02 | actual_duration_minutes | 14 | #NAME? |
| se-close02 | is_late | True | #NAME? |
| se-close02 | stale_authoritative_source_count | 1 | 0 |
| se-close02 | ran_on_stale_authoritative_source | True | False |
| se-close02 | is_late_and_unexplained | False | #NAME? |
| se-close02 | had_uninvoked_exception_available | False | #NAME? |
| se-close02 | late_execution_key | exec-close-2026-q2 | #NAME? |
| se-close02 | inputs_were_fresh_at_run | False | True |
| ... | ... | (138 more) | ... |

### requirement_satisfactions

- Fields: 280/304 (92.1%)
- Computed columns: name, requirement_is_blocking, is_fully_satisfied, is_blocking_and_unmet, blocking_unmet_step_key, blocking_satisfaction_step_key, negative_outcome_requirement_key, evaluator_agent_kind, non_human_evaluated_human_control, requirement_has_computed_witness, is_asserted_only, asserted_only_execution_key, parent_procedure_execution, step_execution_when_scored, is_human_evaluated, requirement_is_approval_type, is_invalid_approval, procedure_execution_of_satisfaction, run_when_invalid_approval, requirement_is_unfalsified, is_clearance_by_unfalsified_control, unfalsified_clearance_step_key, spec_step_of_execution, binding_key, scored_step_executor_agent, evaluator_is_step_executor, run_owner_agent, evaluator_owns_the_run, is_interested_party_assertion, has_written_evidence, is_bare_assertion, interested_assertion_execution_key, is_computedly_witnessed, computed_witness_execution_key, step_executor_agent, was_scored_after_attestation, attestation_instant_for_run, post_attestation_score_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| sat-close-balance | was_scored_after_attestation | False | #NAME? |
| sat-close-balance | attestation_instant_for_run | None | 00:00:00 |
| sat-close-balance | post_attestation_score_execution_key | None | #NAME? |
| sat-close-cutoff | was_scored_after_attestation | False | #NAME? |
| sat-close-cutoff | attestation_instant_for_run | None | 00:00:00 |
| sat-close-cutoff | post_attestation_score_execution_key | None | #NAME? |
| sat-close-evidence | was_scored_after_attestation | False | #NAME? |
| sat-close-evidence | attestation_instant_for_run | None | 00:00:00 |
| sat-close-evidence | post_attestation_score_execution_key | None | #NAME? |
| sat-close-human | was_scored_after_attestation | False | #NAME? |
| sat-close-human | attestation_instant_for_run | None | 00:00:00 |
| sat-close-human | post_attestation_score_execution_key | None | #NAME? |
| sat-close-separation | was_scored_after_attestation | False | #NAME? |
| sat-close-separation | attestation_instant_for_run | None | 00:00:00 |
| sat-close-separation | post_attestation_score_execution_key | None | #NAME? |
| sat-close08-evidence | was_scored_after_attestation | False | #NAME? |
| sat-close08-evidence | attestation_instant_for_run | None | 00:00:00 |
| sat-close08-evidence | post_attestation_score_execution_key | None | #NAME? |
| sat-policy-legal | was_scored_after_attestation | False | #NAME? |
| sat-policy-legal | attestation_instant_for_run | None | 00:00:00 |
| ... | ... | (4 more) | ... |

### errors

- Fields: 2/2 (100.0%)
- Computed columns: name

### issue_occurrences

- Fields: 4/6 (66.7%)
- Computed columns: name, is_unresolved, step_execution_when_unresolved

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| issue-close-feed | name | err-feed-stale @ 2026-06-30T17 | err-feed-stale @ 2026-06-30 22 |
| issue-policy-sms | name | err-sms-throttle @ 2026-07-19T | err-sms-throttle @ 2026-07-19  |

### user_questions

- Fields: 2/2 (100.0%)
- Computed columns: name

### user_feedback

- Fields: 2/2 (100.0%)
- Computed columns: name

### stewardship_assignments

- Fields: 8/10 (80.0%)
- Computed columns: name, count_of_review_events, has_ever_been_reviewed, as_of_instant, is_current_assignment

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| stew-close | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| stew-policy | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |

### change_requests

- Fields: 60/64 (93.8%)
- Computed columns: name, is_open, open_change_version_key, is_decided, as_of_instant, days_pending, is_still_pending, is_stalled, authority_agent, requester_is_authority, awaits_authority_decision, authority_role_label, touches_live_version, is_live_decision_backlog, blocks_an_open_gap, backlog_version_key, is_my_pending_decision, is_my_blocking_backlog, is_my_overdue_backlog, is_implemented, is_my_decided_request, is_my_decided_but_unlanded, decision_latency_days, implementation_latency_days, delay_is_downstream_of_me, unlanded_version_key, is_approved_not_implemented, days_since_approval, is_stalled_implementation, stalled_implementation_version_key, approved_version_key, is_approved_decision

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cr-close-timestamp | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| cr-close-timestamp | days_pending | 0 | 1 |
| cr-close-timestamp | decision_latency_days | 0 | 1 |
| cr-policy-delivery | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |

### review_events

- Fields: 21/30 (70.0%)
- Computed columns: name, as_of_instant, is_overdue, overdue_version_key, promised_cadence_days, days_since_reviewed, exceeds_promised_cadence, cadence_drift_days, promise_and_behavior_disagree, cadence_breach_version_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| review-close-q1 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| review-close-q1 | days_since_reviewed | 120 | 121 |
| review-close-q1 | cadence_drift_days | 30.0 | 31 |
| review-close-q2 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| review-close-q2 | days_since_reviewed | 16 | 17 |
| review-close-q2 | cadence_drift_days | -74.0 | -73 |
| review-policy-prelaunch | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| review-policy-prelaunch | days_since_reviewed | 0 | 1 |
| review-policy-prelaunch | cadence_drift_days | -60.0 | -59 |

### learning_activities

- Fields: 0/2 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| learn-close-retro | name | Retrospective / 2026-07-02T15: | Retrospective / 2026-07-02 20: |
| learn-policy-tabletop | name | TabletopExercise / 2026-07-15T | TabletopExercise / 2026-07-15  |

### operational_bindings

- Fields: 10/55 (18.2%)
- Computed columns: name, as_of_instant, age_minutes, is_fresh, stale_binding_step_key, authoritative_stale_step_key, is_stale_and_authoritative, step_when_stale, resource_is_approved, is_usable_for_drafting, step_when_unusable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bind-close-erp | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| bind-close-erp | age_minutes | 17 | #NAME? |
| bind-close-erp | is_fresh | False | #NAME? |
| bind-close-erp | stale_binding_step_key | close-02 | #NAME? |
| bind-close-erp | authoritative_stale_step_key | close-02 | #NAME? |
| bind-close-erp | is_stale_and_authoritative | True | #NAME? |
| bind-close-erp | step_when_stale | close-02 | #NAME? |
| bind-close-erp | is_usable_for_drafting | False | #NAME? |
| bind-close-erp | step_when_unusable | close-02 | #NAME? |
| bind-policy-consent | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| bind-policy-consent | age_minutes | 75 | #NAME? |
| bind-policy-consent | is_fresh | False | #NAME? |
| bind-policy-consent | stale_binding_step_key | policy-07 | #NAME? |
| bind-policy-consent | authoritative_stale_step_key | policy-07 | #NAME? |
| bind-policy-consent | is_stale_and_authoritative | True | #NAME? |
| bind-policy-consent | step_when_stale | policy-07 | #NAME? |
| bind-policy-consent | is_usable_for_drafting | False | #NAME? |
| bind-policy-consent | step_when_unusable | policy-07 | #NAME? |
| bind-policy-email | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| bind-policy-email | age_minutes | 1230 | #NAME? |
| ... | ... | (25 more) | ... |

### communication_policies

- Fields: 8/8 (100.0%)
- Computed columns: name, consent_violation_count, quiet_hours_violation_count, is_active_policy

### message_templates

- Fields: 30/32 (93.8%)
- Computed columns: name, policy_max_message_length, policy_max_segments, body_template_length, is_template_over_length, valid_approval_count, has_valid_approval, is_claiming_unbacked_approval, last_approved_body_hash, has_body_drifted, is_sendable_under_approval, drifted_send_count, unanswered_delivery_count, transmitted_delivery_count, template_draws_no_response, last_approval_at

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tmpl-policy-email | last_approval_at | 2026-07-17T11:00:00-05:00 | 2026-07-17 16:00:00 |
| tmpl-policy-sms | last_approval_at | 2026-07-19T10:00:00-05:00 | 2026-07-19 15:00:00 |

### semantic_mappings

- Fields: 47/47 (100.0%)
- Computed columns: name

### witness_loops

- Fields: 9/9 (100.0%)
- Computed columns: name, question_count, is_complete

### role_questions

- Fields: 262/327 (80.1%)
- Computed columns: name, predicate_count, is_answered

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| q-auditor-substrate-agreement | predicate_count | 47 | 0 |
| q-auditor-substrate-agreement | is_answered | True | False |
| q-authority-controlled-vocabulary | predicate_count | 22 | 2 |
| q-authority-knowledge-broker | predicate_count | 20 | 4 |
| q-cfo-approval-was-actually-verified | predicate_count | 7 | 3 |
| q-cfo-who-actually-held-authority | predicate_count | 8 | 2 |
| q-close-auto-human-override-drift | predicate_count | 8 | 4 |
| q-comms-approval-authority | predicate_count | 10 | 0 |
| q-comms-approval-authority | is_answered | True | False |
| q-comms-deliverability | predicate_count | 10 | 4 |
| q-comms-optout-present | predicate_count | 8 | 0 |
| q-comms-optout-present | is_answered | True | False |
| q-comms-oversized-sms | predicate_count | 9 | 0 |
| q-comms-oversized-sms | is_answered | True | False |
| q-comms-template-drift | predicate_count | 9 | 0 |
| q-comms-template-drift | is_answered | True | False |
| q-counsel-legal-review-gated-send | predicate_count | 8 | 5 |
| q-counsel-quiet-hours-breach | predicate_count | 10 | 0 |
| q-counsel-quiet-hours-breach | is_answered | True | False |
| q-counsel-retention-evidence | predicate_count | 8 | 1 |
| ... | ... | (45 more) | ... |

### rulebook_fields

- Fields: 9395/9395 (100.0%)
- Computed columns: name, is_derived, is_witness, disagreeing_substrate_count, is_substrate_contested

### test_suites

- Fields: 20/30 (66.7%)
- Computed columns: name, test_count, pass_count, blocking_fail_count, is_green

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| suite-integrity | test_count | 160 | 0 |
| suite-integrity | pass_count | 160 | 0 |
| suite-invariant | test_count | 7 | 0 |
| suite-invariant | pass_count | 7 | 0 |
| suite-provenance | test_count | 107 | 0 |
| suite-provenance | pass_count | 107 | 0 |
| suite-substrate | test_count | 993 | 929 |
| suite-substrate | pass_count | 991 | 929 |
| suite-witness | test_count | 458 | 0 |
| suite-witness | pass_count | 225 | 0 |

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

- Fields: 393/444 (88.5%)
- Computed columns: name, policy_channel, channel_name, policy_requires_consent, recipient_has_sms_consent, was_actually_transmitted, is_consent_violation, consent_violation_policy_key, policy_quiet_hours_start_hour, policy_quiet_hours_end_hour, policy_has_quiet_hours, quiet_window_wraps_midnight, is_inside_quiet_window, is_quiet_hours_violation, quiet_hours_violation_policy_key, recipient_is_unreachable, is_acknowledged, invoked_exception_condition, has_unreachable_exception_invoked, is_fabricated_acknowledgement, is_unhandled_unreachable, unreachable_failure_key, policy_retention_days, as_of_instant, age_days, is_within_retention_window, has_rendered_body, is_evidence_required, is_retention_breach, retention_breach_execution_key, sending_step_execution_step, execution_has_cleared_legal_review, is_unreviewed_send, rendered_body_length, policy_max_message_length_at_send, segment_count, policy_max_segments_at_send, is_over_segment_limit, template_has_valid_approval, is_unapproved_send, policy_required_opt_out_phrase, policy_requires_opt_out, opt_out_phrase_position, has_opt_out_phrase, is_opt_out_in_first_segment, is_missing_required_opt_out, is_opt_out_at_risk_of_truncation, is_failed_delivery, is_suppressed, is_triaged, is_abandoned_failure, abandoned_failure_execution_key, reached_execution_key, template_was_sendable, is_drifted_send, drifted_send_template_key, was_sent_outside_business_hours, was_delivered_and_unanswered, is_poorly_timed_unanswered, is_well_timed_unanswered, unanswered_template_key, transmitted_template_key, approval_preceded_send, has_frozen_approval_evidence, provenance_is_live_derived, current_last_approval_at, template_reapproved_since_send, is_unprovable_approval_claim, has_sent_reminder, acknowledgement_is_outstanding, outstanding_age_days, is_unchased_acknowledgement, is_exhausted_follow_up, needs_human_escalation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| md-001 | name | rec-001 / tmpl-policy-email /  | rec-001 / tmpl-policy-email /  |
| md-001 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| md-001 | age_days | 0 | 1 |
| md-001 | segment_count | None | 1 |
| md-001 | policy_requires_opt_out | False | True |
| md-001 | opt_out_phrase_position | None | #VALUE! |
| md-001 | has_opt_out_phrase | False | #VALUE! |
| md-001 | is_opt_out_in_first_segment | False | #VALUE! |
| md-001 | is_missing_required_opt_out | False | #VALUE! |
| md-001 | is_opt_out_at_risk_of_truncation | False | #VALUE! |
| md-001 | current_last_approval_at | 2026-07-17T11:00:00-05:00 | 2026-07-17 16:00:00 |
| md-002 | name | rec-002 / tmpl-policy-email /  | rec-002 / tmpl-policy-email /  |
| md-002 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| md-002 | age_days | 0 | 1 |
| md-002 | segment_count | None | 1 |
| md-002 | policy_requires_opt_out | False | True |
| md-002 | opt_out_phrase_position | None | #VALUE! |
| md-002 | has_opt_out_phrase | False | #VALUE! |
| md-002 | is_opt_out_in_first_segment | False | #VALUE! |
| md-002 | is_missing_required_opt_out | False | #VALUE! |
| ... | ... | (31 more) | ... |

### template_approvals

- Fields: 10/12 (83.3%)
- Computed columns: name, is_approval_decision, template_policy, required_approval_role, is_decided_by_required_role, valid_approval_template_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| tap-email-001 | name | tmpl-policy-email / Approved / | tmpl-policy-email / Approved / |
| tap-sms-001 | name | tmpl-policy-sms / Approved / 2 | tmpl-policy-sms / Approved / 2 |

### send_intents

- Fields: 475/553 (85.9%)
- Computed columns: name, intent_policy, intent_channel, policy_is_active, intent_requires_consent, recipient_has_channel_consent, consent_gate_passed, recipient_is_sms_reachable, recipient_is_email_reachable, reachability_gate_passed, permission_gate_passed, intent_quiet_start_hour, intent_quiet_end_hour, intent_policy_has_quiet_hours, intent_quiet_window_wraps, intent_is_inside_quiet_window, timing_gate_passed, hours_until_window_opens, intent_max_message_length, intent_max_segments, length_gate_passed, intent_required_opt_out_phrase, opt_out_gate_passed, content_gate_passed, template_is_sendable, execution_has_legal_clearance, intent_approval_role, approval_role_agent_kind, approval_is_human, authorization_gate_passed, is_cleared_to_send, blocking_gate_name, has_resulting_delivery, resulting_delivery_was_transmitted, is_overridden_refusal, is_silently_dropped, resulting_delivery_exception, refusal_cited_an_exception, is_properly_handled_refusal, refusal_failure_execution_key, intent_execution_key, delivered_intent_execution_key, dropped_intent_execution_key, my_approval_was_in_force, refused_on_approved_content, refused_on_opt_out_only, refusal_was_on_my_rules, refusal_was_outside_my_control, is_unreported_refusal_on_my_rules, is_approval_overridden_silently, has_alternate_channel_attempt, alternate_attempt_was_cleared, is_refused_with_no_alternative, exception_prescribed_an_alternative, prescribed_handling_was_performed, is_suppression_without_remedy, has_durable_refusal_record, refusal_was_escalated, is_unrecorded_refusal, is_unescalated_refusal, unescalated_refusal_role_key, unrecorded_refusal_execution_key, was_deferred_on_timing, as_of_instant, window_has_since_reopened, has_retry_attempt, retry_was_cleared, is_abandoned_deferral, deferral_age_hours, is_stale_deferral, enforced_by_unauthorized_agent, consent_input_was_resolvable, recipient_consent_status_raw, policy_input_was_resolvable, all_gate_inputs_resolved, is_unevaluable_refusal, is_self_witnessed_decision, is_independently_confirmed, independently_confirmed_execution_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| si-001 | refusal_cited_an_exception | False | True |
| si-001 | is_unreported_refusal_on_my_rules | False | #VALUE! |
| si-001 | is_approval_overridden_silently | False | #VALUE! |
| si-001 | exception_prescribed_an_alternative | False | True |
| si-001 | is_suppression_without_remedy | False | True |
| si-001 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| si-001 | window_has_since_reopened | False | #NAME? |
| si-001 | is_abandoned_deferral | False | #NAME? |
| si-001 | deferral_age_hours | 22 | #NAME? |
| si-001 | is_stale_deferral | False | #NAME? |
| si-001 | is_self_witnessed_decision | True | #VALUE! |
| si-002 | is_properly_handled_refusal | False | #VALUE! |
| si-002 | is_unreported_refusal_on_my_rules | False | #VALUE! |
| si-002 | is_approval_overridden_silently | False | #VALUE! |
| si-002 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| si-002 | window_has_since_reopened | False | #NAME? |
| si-002 | is_abandoned_deferral | False | #NAME? |
| si-002 | deferral_age_hours | 22 | #NAME? |
| si-002 | is_stale_deferral | False | #NAME? |
| si-002 | is_self_witnessed_decision | True | #VALUE! |
| ... | ... | (58 more) | ... |

### agent_decision_records

- Fields: 78/81 (96.3%)
- Computed columns: name, was_overridden, was_reviewed, deciding_agent_kind, deciding_agent_when_overridden, role_assignment_when_scored, role_assignment_when_overridden, step_of_decision, boundary_match_key, matching_boundary_count, violated_authority_boundary, reviewer_agent_kind, has_human_confirmation, needs_human_confirmation, is_unconfirmed_non_human_decision, step_execution_when_unconfirmed, agent_when_boundary_violated, review_latency_minutes, is_draft_kind, agent_when_draft_overridden, agent_when_draft, is_error_correction, is_reserved_judgment_override, override_reason_is_recorded, is_unexplained_override, error_correction_role_assignment_key, boundary_violation_role_assignment_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| adr-close-extract-q2 | review_latency_minutes | 4.0 | #NAME? |
| adr-close-freeze-q2 | review_latency_minutes | 18.0 | #NAME? |
| adr-close-post-q2 | review_latency_minutes | 30.0 | #NAME? |

### delivered_communications

- Fields: 4/6 (66.7%)
- Computed columns: name, has_authorization, content_matches_approval, authorized_at, was_approved_before_sending, is_defensible

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| dc-hr4821-email-001 | name | Email -> emp-anon-0001 @  | Email -> emp-anon-0001 @ 1899- |
| dc-hr4821-email-001 | was_approved_before_sending | False | True |

### authority_boundaries

- Fields: 52/63 (82.5%)
- Computed columns: name, as_of_instant, is_currently_binding, ratifying_fragment_is_valid, step_when_binding, boundary_match_key, violation_count, is_untested, has_ratifying_fragment, is_unwarranted, ratifying_fragment_is_overdue, ratifying_fragment_is_single_witness, warrant_is_thin, is_unwarranted_and_untested, unwarranted_boundary_step_key, ratifying_fragment_key, ratifying_fragment_status, ratification_lapsed, binds_despite_lapsed_ratification, is_ungrounded_and_untested, constrained_role_assignment_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bnd-close-04-no-ai-escalation-waiver | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| bnd-close-06-no-nonhuman-approval | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| bnd-close-06-no-nonhuman-approval | is_unwarranted | True | #VALUE! |
| bnd-close-06-no-nonhuman-approval | warrant_is_thin | False | #VALUE! |
| bnd-close-06-no-nonhuman-approval | is_unwarranted_and_untested | True | #VALUE! |
| bnd-close-06-no-nonhuman-approval | unwarranted_boundary_step_key | close-06 | #VALUE! |
| bnd-close-06-no-nonhuman-approval | ratification_lapsed | False | #VALUE! |
| bnd-close-06-no-nonhuman-approval | binds_despite_lapsed_ratification | False | #VALUE! |
| bnd-close-06-no-nonhuman-approval | is_ungrounded_and_untested | False | #VALUE! |
| bnd-close-06-no-nonhuman-approval | constrained_role_assignment_key | None | #VALUE! |
| bnd-policy-03-no-ai-commitment | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |

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

- Fields: 413/460 (89.8%)
- Computed columns: name, field_count, policy_count, is_unsecured, disagreeing_substrate_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| AccessDenialTests | field_count | 18 | 0 |
| AccessPolicies | field_count | 16 | 0 |
| AccessPrincipals | field_count | 15 | 0 |
| AgentDecisionRecords | field_count | 40 | 0 |
| AppNavGroups | field_count | 5 | 0 |
| AppRoleProfiles | field_count | 12 | 0 |
| AppRouteQuestions | field_count | 5 | 0 |
| AppRouteReferences | field_count | 5 | 0 |
| AppRoutes | field_count | 18 | 0 |
| Attestations | field_count | 12 | 0 |
| AuthorityBoundaries | field_count | 33 | 0 |
| BindingObservations | field_count | 11 | 0 |
| CellDisagreements | field_count | 10 | 0 |
| CommunicationPolicies | field_count | 22 | 0 |
| ConformanceRuns | field_count | 16 | 0 |
| ConformanceSubstrates | field_count | 20 | 0 |
| DeliveredCommunications | field_count | 18 | 0 |
| ERBCustomizations | field_count | 6 | 0 |
| ERBVersions | field_count | 7 | 0 |
| ExceptionInvocations | field_count | 21 | 0 |
| ... | ... | (27 more) | ... |

### access_principals

- Fields: 94/96 (97.9%)
- Computed columns: name, organization_scope, role_label, policy_count, grant_count, visible_table_count, has_no_access, is_over_privileged

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| principal-knowledge-authority | grant_count | 1603 | 0 |
| principal-process-steward | grant_count | 1603 | 565 |

### access_policies

- Fields: 1414/1414 (100.0%)
- Computed columns: name, is_write_command, is_unrestricted, principal_is_admin, is_unrestricted_non_admin_grant, is_unwitnessed_write, denial_test_count

### field_grants

- Fields: 20725/25473 (81.4%)
- Computed columns: name, field_table, field_name, field_is_derived, is_writable_derived_field, is_masked, grant_key_when_readable

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | field_name | AuthorityBoundaryId | None |
| fg-cfo-AuthorityBoundaries.AuthorityBoundaryId | grant_key_when_readable | principal-cfo|AuthorityBoundar | principal-cfo| |
| fg-cfo-AuthorityBoundaries.AuthorityRole | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.AuthorityRole | field_name | AuthorityRole | None |
| fg-cfo-AuthorityBoundaries.AuthorityRole | grant_key_when_readable | principal-cfo|AuthorityBoundar | principal-cfo| |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | field_name | ForbiddenAgentKind | None |
| fg-cfo-AuthorityBoundaries.ForbiddenAgentKind | grant_key_when_readable | principal-cfo|AuthorityBoundar | principal-cfo| |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | field_name | ForbiddenDecisionKind | None |
| fg-cfo-AuthorityBoundaries.ForbiddenDecisionKind | grant_key_when_readable | principal-cfo|AuthorityBoundar | principal-cfo| |
| fg-cfo-AuthorityBoundaries.Name | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.Name | field_name | Name | None |
| fg-cfo-AuthorityBoundaries.Name | field_is_derived | True | None |
| fg-cfo-AuthorityBoundaries.Name | grant_key_when_readable | principal-cfo|AuthorityBoundar | principal-cfo| |
| fg-cfo-AuthorityBoundaries.Status | field_table | AuthorityBoundaries | None |
| fg-cfo-AuthorityBoundaries.Status | field_name | Status | None |
| fg-cfo-AuthorityBoundaries.Status | grant_key_when_readable | principal-cfo|AuthorityBoundar | principal-cfo| |
| fg-cfo-AuthorityBoundaries.Step | field_table | AuthorityBoundaries | None |
| ... | ... | (4728 more) | ... |

### role_schemas

- Fields: 48/48 (100.0%)
- Computed columns: name, search_path, view_count, is_empty_schema

### role_schema_views

- Fields: 1111/1616 (68.8%)
- Computed columns: name, schema_name, source_view, grant_key, column_count, table_field_count, is_full_width, is_degenerate_view

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| rsv-cfo-authority_boundaries | column_count | 7 | 0 |
| rsv-cfo-authority_boundaries | table_field_count | 33 | 0 |
| rsv-cfo-authority_boundaries | is_degenerate_view | False | True |
| rsv-communications-manager-communication_policies | column_count | 22 | 0 |
| rsv-communications-manager-communication_policies | table_field_count | 22 | 0 |
| rsv-communications-manager-communication_policies | is_full_width | True | False |
| rsv-communications-manager-communication_policies | is_degenerate_view | False | True |
| rsv-communications-manager-message_deliveries | column_count | 8 | 0 |
| rsv-communications-manager-message_deliveries | table_field_count | 94 | 0 |
| rsv-communications-manager-message_deliveries | is_degenerate_view | False | True |
| rsv-communications-manager-message_templates | column_count | 7 | 0 |
| rsv-communications-manager-message_templates | table_field_count | 26 | 0 |
| rsv-communications-manager-message_templates | is_degenerate_view | False | True |
| rsv-communications-manager-recipients | column_count | 6 | 0 |
| rsv-communications-manager-recipients | table_field_count | 15 | 0 |
| rsv-communications-manager-recipients | is_degenerate_view | False | True |
| rsv-communications-manager-send_intents | column_count | 7 | 0 |
| rsv-communications-manager-send_intents | table_field_count | 100 | 0 |
| rsv-communications-manager-send_intents | is_degenerate_view | False | True |
| rsv-employment-counsel-authority_boundaries | column_count | 6 | 0 |
| ... | ... | (485 more) | ... |

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

- Fields: 32/40 (80.0%)
- Computed columns: name, as_of_instant, conformance_rate, is_conformant, has_major_drift_from_documentation, days_since_mined, is_stale_mining_evidence, procedure_version_is_live, is_drift_on_live_version, drifted_mining_run_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pmr-close-v10-archived | name | ERP General Ledger Audit Log / | ERP General Ledger Audit Log / |
| pmr-close-v10-archived | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| pmr-close-v11-cutoff-bypass | name | ERP General Ledger Audit Log / | ERP General Ledger Audit Log / |
| pmr-close-v11-cutoff-bypass | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| pmr-close-v11-q3 | name | ERP General Ledger Audit Log / | ERP General Ledger Audit Log / |
| pmr-close-v11-q3 | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| pmr-policy-v1-notify | name | Notification Pipeline Delivery | Notification Pipeline Delivery |
| pmr-policy-v1-notify | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |

### vocabularies

- Fields: 8/8 (100.0%)
- Computed columns: name, term_count, orphan_term_count, has_orphan_terms

### vocabulary_terms

- Fields: 60/60 (100.0%)
- Computed columns: name, usage_count, is_orphan_term, is_widely_adopted_term, orphan_term_vocabulary_key

### knowledge_broker_links

- Fields: 35/40 (87.5%)
- Computed columns: name, as_of_instant, days_since_consulted, is_active_reliance, broker_is_still_engaged, is_at_risk_reliance, active_reliance_broker_key, at_risk_broker_key

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| kbl-amina-jordan-sod | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kbl-devon-priya-recon | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kbl-elena-priya-sod | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kbl-maria-priya-recon | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
| kbl-noah-elena-quiet | as_of_instant | 2026-07-19T13:00:00-05:00 | 2026-07-19 18:00:00 |
