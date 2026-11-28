# Test Results: effortless-python

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 691 |
| Passed | 691 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 2s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 609 | 609 | 100.0% |
| Lookup (INDEX/MATCH) | 48 | 48 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 34 | 34 | 100.0% |

## Results by Entity

### workflows

- Fields: 37/37 (100.0%)
- Computed columns: relative_path, iri, name, count_of_non_proposed_steps, has_more_than1_step, count_ai_steps, count_human_steps, count_human_required_steps, count_approval_consistency_violations, has_consistency_violation, has_ai_agent_step, months_since_modified, is_stale, is_stale_and_has_ai_agent, count_derivation_links, count_legal_owned_steps, count_engineering_owned_steps, involves_engineering_and_legal, count_inferred_precedence_pairs, count_asserted_precedence_pairs, count_of_precedence_closure_pairs, count_roles_with_bad_filler_cardinality, count_agent_type_changes, count_compliance_audit_changes, count_approval_gate_steps, count_gates_without_human_approver, count_workflow_artifacts, count_roles_with_escalation_violation, count_unconsumed_datasets, cq1_satisfied, cq2_satisfied, cq3_satisfied, cq4_satisfied, cq5_satisfied, cq6_satisfied, cq7_satisfied, cq8_satisfied

### workflow_steps

- Fields: 95/95 (100.0%)
- Computed columns: parent_path, relative_path, iri, name, preceding_step_count, inferred_sequence_position, sequence_position, executing_human_agent, executing_ai_agent, executing_automated_pipeline, executing_agent_type, is_executed_by_ai, is_executed_by_human, is_approval_gate, approval_consistency_violation, approval_is_human_filled, owning_department, is_legal_owned, is_engineering_owned

### approval_gates

- Fields: 7/7 (100.0%)
- Computed columns: parent_path, relative_path, iri, name, gate_role, gate_approver_human, has_human_approver

### step_precedence

- Fields: 16/16 (100.0%)
- Computed columns: parent_path, relative_path, iri, name

### roles

- Fields: 56/56 (100.0%)
- Computed columns: relative_path, iri, name, filled_by_arm_count, has_exactly_one_filler, filler_type, fills_approval_gate, escalation_violation

### role_assignments

- Fields: 54/54 (100.0%)
- Computed columns: parent_path, relative_path, iri, name, filler_type, is_current, was_active_as_of_audit_date, is_agent_type_change, requires_compliance_audit

### departments

- Fields: 6/6 (100.0%)
- Computed columns: relative_path, iri, name

### human_agents

- Fields: 10/10 (100.0%)
- Computed columns: relative_path, iri

### ai_agents

- Fields: 8/8 (100.0%)
- Computed columns: relative_path, iri, count_attributed_artifacts, count_impacted_workflows

### automated_pipelines

- Fields: 2/2 (100.0%)
- Computed columns: relative_path, iri

### workflow_status_concepts

- Fields: 8/8 (100.0%)
- Computed columns: relative_path, iri

### agent_capability_concepts

- Fields: 12/12 (100.0%)
- Computed columns: relative_path, iri

### artifact_type_concepts

- Fields: 6/6 (100.0%)
- Computed columns: relative_path, iri

### datasets

- Fields: 3/3 (100.0%)
- Computed columns: relative_path, iri, is_consumed

### workflow_artifacts

- Fields: 35/35 (100.0%)
- Computed columns: parent_path, relative_path, iri, producing_agent_type, has_derivation_parent, produced_by_workflow, has_producing_workflow

### governance_roles

- Fields: 8/8 (100.0%)
- Computed columns: relative_path, iri, name, can_approve_changes

### change_log

- Fields: 10/10 (100.0%)
- Computed columns: relative_path, iri, name, is_breaking_change, is_backward_compatible

### vocabulary_reconciliations

- Fields: 6/6 (100.0%)
- Computed columns: relative_path, iri, name

### scenarios

- Fields: 36/36 (100.0%)
- Computed columns: relative_path, iri, name

### competency_questions

- Fields: 24/24 (100.0%)
- Computed columns: relative_path, iri, name

### scenario_cq_effects

- Fields: 36/36 (100.0%)
- Computed columns: relative_path, iri, name

### conformance_tests

- Fields: 216/216 (100.0%)
- Computed columns: relative_path, iri, name
