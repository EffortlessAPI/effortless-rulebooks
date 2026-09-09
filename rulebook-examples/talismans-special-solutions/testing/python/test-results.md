# Test Results: python

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 691 |
| Passed | 563 |
| Failed | 128 |
| Score | 81.5% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 532 | 609 | 87.4% |
| Lookup (INDEX/MATCH) | 10 | 48 | 20.8% |
| Aggregation (COUNTIFS/SUMIFS) | 21 | 34 | 61.8% |

## Results by Entity

### roles

- Fields: 55/56 (98.2%)
- Computed columns: relative_path, iri, name, filled_by_arm_count, has_exactly_one_filler, filler_type, fills_approval_gate, escalation_violation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-release-manager-role | fills_approval_gate | 1 | None |

### workflow_steps

- Fields: 47/95 (49.5%)
- Computed columns: parent_path, relative_path, iri, name, preceding_step_count, inferred_sequence_position, sequence_position, executing_human_agent, executing_ai_agent, executing_automated_pipeline, executing_agent_type, is_executed_by_ai, is_executed_by_human, is_approval_gate, approval_consistency_violation, approval_is_human_filled, owning_department, is_legal_owned, is_engineering_owned

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| prod-deploy-step-1 | parent_path | workflows/production-deploymen | None |
| prod-deploy-step-1 | relative_path | workflows/production-deploymen | /steps/prod-deploy-step-1 |
| prod-deploy-step-1 | iri | workflows-production-deploymen | -steps-prod-deploy-step-1 |
| prod-deploy-step-1 | sequence_position | 1 | None |
| prod-deploy-step-1 | executing_ai_agent | ntwf-risk-ai | None |
| prod-deploy-step-1 | executing_agent_type | AIAgent | None |
| prod-deploy-step-1 | is_executed_by_ai | True | False |
| prod-deploy-step-1 | owning_department | ntwf-engineering | None |
| prod-deploy-step-1 | is_engineering_owned | True | False |
| prod-deploy-step-2 | parent_path | workflows/production-deploymen | None |
| prod-deploy-step-2 | relative_path | workflows/production-deploymen | /steps/prod-deploy-step-2 |
| prod-deploy-step-2 | iri | workflows-production-deploymen | -steps-prod-deploy-step-2 |
| prod-deploy-step-2 | sequence_position | 2 | None |
| prod-deploy-step-2 | executing_human_agent | ntwf-james-okafor | None |
| prod-deploy-step-2 | executing_agent_type | HumanAgent | None |
| prod-deploy-step-2 | is_executed_by_human | True | False |
| prod-deploy-step-2 | approval_consistency_violation | False | True |
| prod-deploy-step-2 | approval_is_human_filled | True | False |
| prod-deploy-step-2 | owning_department | ntwf-legal-dept | None |
| prod-deploy-step-2 | is_legal_owned | True | False |
| ... | ... | (28 more) | ... |

### artifact_type_concepts

- Fields: 6/6 (100.0%)
- Computed columns: relative_path, iri

### vocabulary_reconciliations

- Fields: 6/6 (100.0%)
- Computed columns: relative_path, iri, name

### change_log

- Fields: 10/10 (100.0%)
- Computed columns: relative_path, iri, name, is_breaking_change, is_backward_compatible

### scenario_cq_effects

- Fields: 36/36 (100.0%)
- Computed columns: relative_path, iri, name

### agent_capability_concepts

- Fields: 12/12 (100.0%)
- Computed columns: relative_path, iri

### ai_agents

- Fields: 6/8 (75.0%)
- Computed columns: relative_path, iri, count_attributed_artifacts, count_impacted_workflows

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-health-ai | count_impacted_workflows | 1 | None |
| ntwf-risk-ai | count_impacted_workflows | 1 | None |

### governance_roles

- Fields: 8/8 (100.0%)
- Computed columns: relative_path, iri, name, can_approve_changes

### workflows

- Fields: 21/37 (56.8%)
- Computed columns: relative_path, iri, name, count_of_non_proposed_steps, has_more_than1_step, count_ai_steps, count_human_steps, count_human_required_steps, count_approval_consistency_violations, has_consistency_violation, has_ai_agent_step, months_since_modified, is_stale, is_stale_and_has_ai_agent, count_derivation_links, count_legal_owned_steps, count_engineering_owned_steps, involves_engineering_and_legal, count_inferred_precedence_pairs, count_asserted_precedence_pairs, count_of_precedence_closure_pairs, count_roles_with_bad_filler_cardinality, count_agent_type_changes, count_compliance_audit_changes, count_approval_gate_steps, count_gates_without_human_approver, count_workflow_artifacts, count_roles_with_escalation_violation, count_unconsumed_datasets, cq1_satisfied, cq2_satisfied, cq3_satisfied, cq4_satisfied, cq5_satisfied, cq6_satisfied, cq7_satisfied, cq8_satisfied

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| production-deployment | count_ai_steps | 2 | None |
| production-deployment | count_human_steps | 2 | None |
| production-deployment | count_human_required_steps | 2 | None |
| production-deployment | has_ai_agent_step | True | False |
| production-deployment | count_derivation_links | 4 | None |
| production-deployment | count_legal_owned_steps | 1 | None |
| production-deployment | count_engineering_owned_steps | 4 | None |
| production-deployment | involves_engineering_and_legal | True | False |
| production-deployment | count_agent_type_changes | 2 | None |
| production-deployment | count_compliance_audit_changes | 1 | None |
| production-deployment | count_approval_gate_steps | 1 | None |
| production-deployment | count_unconsumed_datasets | 1 | None |
| production-deployment | cq2_satisfied | True | False |
| production-deployment | cq4_satisfied | True | False |
| production-deployment | cq6_satisfied | True | False |
| production-deployment | cq7_satisfied | True | False |

### automated_pipelines

- Fields: 2/2 (100.0%)
- Computed columns: relative_path, iri

### workflow_status_concepts

- Fields: 8/8 (100.0%)
- Computed columns: relative_path, iri

### departments

- Fields: 6/6 (100.0%)
- Computed columns: relative_path, iri, name

### datasets

- Fields: 3/3 (100.0%)
- Computed columns: relative_path, iri, is_consumed

### step_precedence

- Fields: 4/16 (25.0%)
- Computed columns: parent_path, relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| prec-1-2 | parent_path | workflows/production-deploymen | None |
| prec-1-2 | relative_path | workflows/production-deploymen | /precedence/prec-1-2 |
| prec-1-2 | iri | workflows-production-deploymen | -precedence-prec-1-2 |
| prec-2-3 | parent_path | workflows/production-deploymen | None |
| prec-2-3 | relative_path | workflows/production-deploymen | /precedence/prec-2-3 |
| prec-2-3 | iri | workflows-production-deploymen | -precedence-prec-2-3 |
| prec-3-4 | parent_path | workflows/production-deploymen | None |
| prec-3-4 | relative_path | workflows/production-deploymen | /precedence/prec-3-4 |
| prec-3-4 | iri | workflows-production-deploymen | -precedence-prec-3-4 |
| prec-4-5 | parent_path | workflows/production-deploymen | None |
| prec-4-5 | relative_path | workflows/production-deploymen | /precedence/prec-4-5 |
| prec-4-5 | iri | workflows-production-deploymen | -precedence-prec-4-5 |

### workflow_artifacts

- Fields: 10/35 (28.6%)
- Computed columns: parent_path, relative_path, iri, producing_agent_type, has_derivation_parent, produced_by_workflow, has_producing_workflow

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| artifact-deployment-log | parent_path | workflows/production-deploymen | None |
| artifact-deployment-log | relative_path | workflows/production-deploymen | /artifacts/artifact-deployment |
| artifact-deployment-log | iri | workflows-production-deploymen | -artifacts-artifact-deployment |
| artifact-deployment-log | produced_by_workflow | production-deployment | None |
| artifact-deployment-log | has_producing_workflow | True | False |
| artifact-legal-clearance | parent_path | workflows/production-deploymen | None |
| artifact-legal-clearance | relative_path | workflows/production-deploymen | /artifacts/artifact-legal-clea |
| artifact-legal-clearance | iri | workflows-production-deploymen | -artifacts-artifact-legal-clea |
| artifact-legal-clearance | produced_by_workflow | production-deployment | None |
| artifact-legal-clearance | has_producing_workflow | True | False |
| artifact-post-deploy-report | parent_path | workflows/production-deploymen | None |
| artifact-post-deploy-report | relative_path | workflows/production-deploymen | /artifacts/artifact-post-deplo |
| artifact-post-deploy-report | iri | workflows-production-deploymen | -artifacts-artifact-post-deplo |
| artifact-post-deploy-report | produced_by_workflow | production-deployment | None |
| artifact-post-deploy-report | has_producing_workflow | True | False |
| artifact-release-authorization | parent_path | workflows/production-deploymen | None |
| artifact-release-authorization | relative_path | workflows/production-deploymen | /artifacts/artifact-release-au |
| artifact-release-authorization | iri | workflows-production-deploymen | -artifacts-artifact-release-au |
| artifact-release-authorization | produced_by_workflow | production-deployment | None |
| artifact-release-authorization | has_producing_workflow | True | False |
| ... | ... | (5 more) | ... |

### competency_questions

- Fields: 24/24 (100.0%)
- Computed columns: relative_path, iri, name

### human_agents

- Fields: 10/10 (100.0%)
- Computed columns: relative_path, iri

### role_assignments

- Fields: 36/54 (66.7%)
- Computed columns: parent_path, relative_path, iri, name, filler_type, is_current, was_active_as_of_audit_date, is_agent_type_change, requires_compliance_audit

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| asn-deploy-health-ai-current | parent_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-ai-current | relative_path | roles/ntwf-deployment-health-r | /assignments/asn-deploy-health |
| asn-deploy-health-ai-current | iri | roles-ntwf-deployment-health-r | -assignments-asn-deploy-health |
| asn-deploy-health-ai-initial | parent_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-ai-initial | relative_path | roles/ntwf-deployment-health-r | /assignments/asn-deploy-health |
| asn-deploy-health-ai-initial | iri | roles-ntwf-deployment-health-r | -assignments-asn-deploy-health |
| asn-deploy-health-human-audit | parent_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-human-audit | relative_path | roles/ntwf-deployment-health-r | /assignments/asn-deploy-health |
| asn-deploy-health-human-audit | iri | roles-ntwf-deployment-health-r | -assignments-asn-deploy-health |
| asn-release-manager-maria | parent_path | roles/ntwf-release-manager-rol | None |
| asn-release-manager-maria | relative_path | roles/ntwf-release-manager-rol | /assignments/asn-release-manag |
| asn-release-manager-maria | iri | roles-ntwf-release-manager-rol | -assignments-asn-release-manag |
| asn-vp-eng-david | parent_path | roles/ntwf-vp-engineering-role | None |
| asn-vp-eng-david | relative_path | roles/ntwf-vp-engineering-role | /assignments/asn-vp-eng-david |
| asn-vp-eng-david | iri | roles-ntwf-vp-engineering-role | -assignments-asn-vp-eng-david |
| asn-vp-eng-priya | parent_path | roles/ntwf-vp-engineering-role | None |
| asn-vp-eng-priya | relative_path | roles/ntwf-vp-engineering-role | /assignments/asn-vp-eng-priya |
| asn-vp-eng-priya | iri | roles-ntwf-vp-engineering-role | -assignments-asn-vp-eng-priya |

### approval_gates

- Fields: 1/7 (14.3%)
- Computed columns: parent_path, relative_path, iri, name, gate_role, gate_approver_human, has_human_approver

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-release-approval-gate | parent_path | workflows/production-deploymen | None |
| ntwf-release-approval-gate | relative_path | workflows/production-deploymen | /approval-gates/ntwf-release-a |
| ntwf-release-approval-gate | iri | workflows-production-deploymen | -approval-gates-ntwf-release-a |
| ntwf-release-approval-gate | gate_role | ntwf-release-manager-role | None |
| ntwf-release-approval-gate | gate_approver_human | ntwf-maria-gonzalez | None |
| ntwf-release-approval-gate | has_human_approver | True | False |

### scenarios

- Fields: 36/36 (100.0%)
- Computed columns: relative_path, iri, name

### conformance_tests

- Fields: 216/216 (100.0%)
- Computed columns: relative_path, iri, name
