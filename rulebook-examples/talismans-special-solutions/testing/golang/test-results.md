# Test Results: golang

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 691 |
| Passed | 72 |
| Failed | 619 |
| Score | 10.4% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 51 | 609 | 8.4% |
| Lookup (INDEX/MATCH) | 10 | 48 | 20.8% |
| Aggregation (COUNTIFS/SUMIFS) | 11 | 34 | 32.4% |

## Results by Entity

### roles

- Fields: 13/56 (23.2%)
- Computed columns: relative_path, iri, name, filled_by_arm_count, has_exactly_one_filler, filler_type, fills_approval_gate, escalation_violation

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-ci-executor-role | relative_path | roles/ntwf-ci-executor-role | None |
| ntwf-ci-executor-role | iri | roles-ntwf-ci-executor-role | None |
| ntwf-ci-executor-role | name | ci/cd-executor | None |
| ntwf-ci-executor-role | filled_by_arm_count | 1 | None |
| ntwf-ci-executor-role | has_exactly_one_filler | True | None |
| ntwf-ci-executor-role | filler_type | AutomatedPipeline | None |
| ntwf-cto-role | relative_path | roles/ntwf-cto-role | None |
| ntwf-cto-role | iri | roles-ntwf-cto-role | None |
| ntwf-cto-role | name | chief-technology-officer | None |
| ntwf-cto-role | filled_by_arm_count | 1 | None |
| ntwf-cto-role | has_exactly_one_filler | True | None |
| ntwf-cto-role | filler_type | HumanAgent | None |
| ntwf-deployment-health-role | relative_path | roles/ntwf-deployment-health-r | None |
| ntwf-deployment-health-role | iri | roles-ntwf-deployment-health-r | None |
| ntwf-deployment-health-role | name | deployment-health-agent | None |
| ntwf-deployment-health-role | filled_by_arm_count | 1 | None |
| ntwf-deployment-health-role | has_exactly_one_filler | True | None |
| ntwf-deployment-health-role | filler_type | AIAgent | None |
| ntwf-legal-compliance-role | relative_path | roles/ntwf-legal-compliance-ro | None |
| ntwf-legal-compliance-role | iri | roles-ntwf-legal-compliance-ro | None |
| ... | ... | (23 more) | ... |

### workflow_steps

- Fields: 31/95 (32.6%)
- Computed columns: parent_path, relative_path, iri, name, preceding_step_count, inferred_sequence_position, sequence_position, executing_human_agent, executing_ai_agent, executing_automated_pipeline, executing_agent_type, is_executed_by_ai, is_executed_by_human, is_approval_gate, approval_consistency_violation, approval_is_human_filled, owning_department, is_legal_owned, is_engineering_owned

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| prod-deploy-step-1 | parent_path | workflows/production-deploymen | None |
| prod-deploy-step-1 | relative_path | workflows/production-deploymen | None |
| prod-deploy-step-1 | iri | workflows-production-deploymen | None |
| prod-deploy-step-1 | name | ai-risk-assessment | None |
| prod-deploy-step-1 | inferred_sequence_position | 1 | None |
| prod-deploy-step-1 | sequence_position | 1 | None |
| prod-deploy-step-1 | executing_ai_agent | ntwf-risk-ai | None |
| prod-deploy-step-1 | executing_agent_type | AIAgent | None |
| prod-deploy-step-1 | is_executed_by_ai | True | None |
| prod-deploy-step-1 | approval_is_human_filled | True | None |
| prod-deploy-step-1 | owning_department | ntwf-engineering | None |
| prod-deploy-step-1 | is_engineering_owned | True | None |
| prod-deploy-step-2 | parent_path | workflows/production-deploymen | None |
| prod-deploy-step-2 | relative_path | workflows/production-deploymen | None |
| prod-deploy-step-2 | iri | workflows-production-deploymen | None |
| prod-deploy-step-2 | name | legal-compliance-review | None |
| prod-deploy-step-2 | preceding_step_count | 1 | None |
| prod-deploy-step-2 | inferred_sequence_position | 2 | None |
| prod-deploy-step-2 | sequence_position | 2 | None |
| prod-deploy-step-2 | executing_human_agent | ntwf-james-okafor | None |
| ... | ... | (44 more) | ... |

### artifact_type_concepts

- Fields: 0/6 (0.0%)
- Computed columns: relative_path, iri

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| type-decision-record | relative_path | concepts/artifact-type/type-de | None |
| type-decision-record | iri | concepts-artifact-type-type-de | None |
| type-log | relative_path | concepts/artifact-type/type-lo | None |
| type-log | iri | concepts-artifact-type-type-lo | None |
| type-report | relative_path | concepts/artifact-type/type-re | None |
| type-report | iri | concepts-artifact-type-type-re | None |

### vocabulary_reconciliations

- Fields: 0/6 (0.0%)
- Computed columns: relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| recon-dcat-v3 | relative_path | reconciliations/recon-dcat-v3 | None |
| recon-dcat-v3 | iri | reconciliations-recon-dcat-v3 | None |
| recon-dcat-v3 | name | dcat:Dataset (v2) owl:sameAs d | None |
| recon-foaf-name | relative_path | reconciliations/recon-foaf-nam | None |
| recon-foaf-name | iri | reconciliations-recon-foaf-nam | None |
| recon-foaf-name | name | foaf:name owl:sameAs ntwf:name | None |

### change_log

- Fields: 2/10 (20.0%)
- Computed columns: relative_path, iri, name, is_breaking_change, is_backward_compatible

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| change-1-0-0 | relative_path | change-log/change-1-0-0 | None |
| change-1-0-0 | iri | change-log-change-1-0-0 | None |
| change-1-0-0 | name | 1.0.0 (2026-01-15) | None |
| change-1-0-0 | is_breaking_change | True | None |
| change-1-1-0 | relative_path | change-log/change-1-1-0 | None |
| change-1-1-0 | iri | change-log-change-1-1-0 | None |
| change-1-1-0 | name | 1.1.0 (2026-03-20) | None |
| change-1-1-0 | is_backward_compatible | True | None |

### scenario_cq_effects

- Fields: 0/36 (0.0%)
- Computed columns: relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ai-at-gate-cq-3 | relative_path | scenario-cq-effects/ai-at-gate | None |
| ai-at-gate-cq-3 | iri | scenario-cq-effects-ai-at-gate | None |
| ai-at-gate-cq-3 | name | ai-at-gate-cq-3 | None |
| ai-release-manager-cq-2 | relative_path | scenario-cq-effects/ai-release | None |
| ai-release-manager-cq-2 | iri | scenario-cq-effects-ai-release | None |
| ai-release-manager-cq-2 | name | ai-release-manager-cq-2 | None |
| ai-release-manager-cq-3 | relative_path | scenario-cq-effects/ai-release | None |
| ai-release-manager-cq-3 | iri | scenario-cq-effects-ai-release | None |
| ai-release-manager-cq-3 | name | ai-release-manager-cq-3 | None |
| all-human-cq-3 | relative_path | scenario-cq-effects/all-human- | None |
| all-human-cq-3 | iri | scenario-cq-effects-all-human- | None |
| all-human-cq-3 | name | all-human-cq-3 | None |
| break-delegation-cq-6 | relative_path | scenario-cq-effects/break-dele | None |
| break-delegation-cq-6 | iri | scenario-cq-effects-break-dele | None |
| break-delegation-cq-6 | name | break-delegation-cq-6 | None |
| break-derivation-cq-4 | relative_path | scenario-cq-effects/break-deri | None |
| break-derivation-cq-4 | iri | scenario-cq-effects-break-deri | None |
| break-derivation-cq-4 | name | break-derivation-cq-4 | None |
| dataset-unconsumed-cq-8 | relative_path | scenario-cq-effects/dataset-un | None |
| dataset-unconsumed-cq-8 | iri | scenario-cq-effects-dataset-un | None |
| ... | ... | (16 more) | ... |

### agent_capability_concepts

- Fields: 0/12 (0.0%)
- Computed columns: relative_path, iri

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cap-ci-execution | relative_path | concepts/agent-capability/cap- | None |
| cap-ci-execution | iri | concepts-agent-capability-cap- | None |
| cap-deployment-health | relative_path | concepts/agent-capability/cap- | None |
| cap-deployment-health | iri | concepts-agent-capability-cap- | None |
| cap-executive-authorization | relative_path | concepts/agent-capability/cap- | None |
| cap-executive-authorization | iri | concepts-agent-capability-cap- | None |
| cap-human-judgment | relative_path | concepts/agent-capability/cap- | None |
| cap-human-judgment | iri | concepts-agent-capability-cap- | None |
| cap-legal-review | relative_path | concepts/agent-capability/cap- | None |
| cap-legal-review | iri | concepts-agent-capability-cap- | None |
| cap-risk-analysis | relative_path | concepts/agent-capability/cap- | None |
| cap-risk-analysis | iri | concepts-agent-capability-cap- | None |

### ai_agents

- Fields: 0/8 (0.0%)
- Computed columns: relative_path, iri, count_attributed_artifacts, count_impacted_workflows

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-health-ai | relative_path | ai-agents/ntwf-health-ai | None |
| ntwf-health-ai | iri | ai-agents-ntwf-health-ai | None |
| ntwf-health-ai | count_attributed_artifacts | 1 | None |
| ntwf-health-ai | count_impacted_workflows | 1 | None |
| ntwf-risk-ai | relative_path | ai-agents/ntwf-risk-ai | None |
| ntwf-risk-ai | iri | ai-agents-ntwf-risk-ai | None |
| ntwf-risk-ai | count_attributed_artifacts | 1 | None |
| ntwf-risk-ai | count_impacted_workflows | 1 | None |

### governance_roles

- Fields: 1/8 (12.5%)
- Computed columns: relative_path, iri, name, can_approve_changes

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| gov-authority | relative_path | governance-roles/gov-authority | None |
| gov-authority | iri | governance-roles-gov-authority | None |
| gov-authority | name | ontology-authority | None |
| gov-authority | can_approve_changes | True | None |
| gov-steward | relative_path | governance-roles/gov-steward | None |
| gov-steward | iri | governance-roles-gov-steward | None |
| gov-steward | name | ontology-steward | None |

### workflows

- Fields: 8/37 (21.6%)
- Computed columns: relative_path, iri, name, count_of_non_proposed_steps, has_more_than1_step, count_ai_steps, count_human_steps, count_human_required_steps, count_approval_consistency_violations, has_consistency_violation, has_ai_agent_step, months_since_modified, is_stale, is_stale_and_has_ai_agent, count_derivation_links, count_legal_owned_steps, count_engineering_owned_steps, involves_engineering_and_legal, count_inferred_precedence_pairs, count_asserted_precedence_pairs, count_of_precedence_closure_pairs, count_roles_with_bad_filler_cardinality, count_agent_type_changes, count_compliance_audit_changes, count_approval_gate_steps, count_gates_without_human_approver, count_workflow_artifacts, count_roles_with_escalation_violation, count_unconsumed_datasets, cq1_satisfied, cq2_satisfied, cq3_satisfied, cq4_satisfied, cq5_satisfied, cq6_satisfied, cq7_satisfied, cq8_satisfied

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| production-deployment | relative_path | workflows/production-deploymen | None |
| production-deployment | iri | workflows-production-deploymen | None |
| production-deployment | name | production-deployment | None |
| production-deployment | count_of_non_proposed_steps | 5 | None |
| production-deployment | has_more_than1_step | True | None |
| production-deployment | count_ai_steps | 2 | None |
| production-deployment | count_human_steps | 2 | None |
| production-deployment | count_human_required_steps | 2 | None |
| production-deployment | has_ai_agent_step | True | None |
| production-deployment | months_since_modified | 5 | None |
| production-deployment | count_derivation_links | 4 | None |
| production-deployment | count_legal_owned_steps | 1 | None |
| production-deployment | count_engineering_owned_steps | 4 | None |
| production-deployment | involves_engineering_and_legal | True | None |
| production-deployment | count_inferred_precedence_pairs | 6 | None |
| production-deployment | count_asserted_precedence_pairs | 4 | None |
| production-deployment | count_of_precedence_closure_pairs | 10 | None |
| production-deployment | count_agent_type_changes | 2 | None |
| production-deployment | count_compliance_audit_changes | 1 | None |
| production-deployment | count_approval_gate_steps | 1 | None |
| ... | ... | (9 more) | ... |

### automated_pipelines

- Fields: 0/2 (0.0%)
- Computed columns: relative_path, iri

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-ci-pipeline | relative_path | automated-pipelines/ntwf-ci-pi | None |
| ntwf-ci-pipeline | iri | automated-pipelines-ntwf-ci-pi | None |

### workflow_status_concepts

- Fields: 0/8 (0.0%)
- Computed columns: relative_path, iri

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| status-active | relative_path | concepts/workflow-status/statu | None |
| status-active | iri | concepts-workflow-status-statu | None |
| status-archived | relative_path | concepts/workflow-status/statu | None |
| status-archived | iri | concepts-workflow-status-statu | None |
| status-deprecated | relative_path | concepts/workflow-status/statu | None |
| status-deprecated | iri | concepts-workflow-status-statu | None |
| status-draft | relative_path | concepts/workflow-status/statu | None |
| status-draft | iri | concepts-workflow-status-statu | None |

### departments

- Fields: 0/6 (0.0%)
- Computed columns: relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-engineering | relative_path | departments/ntwf-engineering | None |
| ntwf-engineering | iri | departments-ntwf-engineering | None |
| ntwf-engineering | name | engineering | None |
| ntwf-legal-dept | relative_path | departments/ntwf-legal-dept | None |
| ntwf-legal-dept | iri | departments-ntwf-legal-dept | None |
| ntwf-legal-dept | name | legal | None |

### datasets

- Fields: 1/3 (33.3%)
- Computed columns: relative_path, iri, is_consumed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ds-q1-2026-risk-metrics | relative_path | datasets/ds-q1-2026-risk-metri | None |
| ds-q1-2026-risk-metrics | iri | datasets-ds-q1-2026-risk-metri | None |

### step_precedence

- Fields: 0/16 (0.0%)
- Computed columns: parent_path, relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| prec-1-2 | parent_path | workflows/production-deploymen | None |
| prec-1-2 | relative_path | workflows/production-deploymen | None |
| prec-1-2 | iri | workflows-production-deploymen | None |
| prec-1-2 | name | prod-deploy-step-1 -> prod-dep | None |
| prec-2-3 | parent_path | workflows/production-deploymen | None |
| prec-2-3 | relative_path | workflows/production-deploymen | None |
| prec-2-3 | iri | workflows-production-deploymen | None |
| prec-2-3 | name | prod-deploy-step-2 -> prod-dep | None |
| prec-3-4 | parent_path | workflows/production-deploymen | None |
| prec-3-4 | relative_path | workflows/production-deploymen | None |
| prec-3-4 | iri | workflows-production-deploymen | None |
| prec-3-4 | name | prod-deploy-step-3 -> prod-dep | None |
| prec-4-5 | parent_path | workflows/production-deploymen | None |
| prec-4-5 | relative_path | workflows/production-deploymen | None |
| prec-4-5 | iri | workflows-production-deploymen | None |
| prec-4-5 | name | prod-deploy-step-4 -> prod-dep | None |

### workflow_artifacts

- Fields: 1/35 (2.9%)
- Computed columns: parent_path, relative_path, iri, producing_agent_type, has_derivation_parent, produced_by_workflow, has_producing_workflow

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| artifact-deployment-log | parent_path | workflows/production-deploymen | None |
| artifact-deployment-log | relative_path | workflows/production-deploymen | None |
| artifact-deployment-log | iri | workflows-production-deploymen | None |
| artifact-deployment-log | producing_agent_type | AutomatedPipeline | None |
| artifact-deployment-log | has_derivation_parent | True | None |
| artifact-deployment-log | produced_by_workflow | production-deployment | None |
| artifact-deployment-log | has_producing_workflow | True | None |
| artifact-legal-clearance | parent_path | workflows/production-deploymen | None |
| artifact-legal-clearance | relative_path | workflows/production-deploymen | None |
| artifact-legal-clearance | iri | workflows-production-deploymen | None |
| artifact-legal-clearance | producing_agent_type | HumanAgent | None |
| artifact-legal-clearance | has_derivation_parent | True | None |
| artifact-legal-clearance | produced_by_workflow | production-deployment | None |
| artifact-legal-clearance | has_producing_workflow | True | None |
| artifact-post-deploy-report | parent_path | workflows/production-deploymen | None |
| artifact-post-deploy-report | relative_path | workflows/production-deploymen | None |
| artifact-post-deploy-report | iri | workflows-production-deploymen | None |
| artifact-post-deploy-report | producing_agent_type | AIAgent | None |
| artifact-post-deploy-report | has_derivation_parent | True | None |
| artifact-post-deploy-report | produced_by_workflow | production-deployment | None |
| ... | ... | (14 more) | ... |

### competency_questions

- Fields: 0/24 (0.0%)
- Computed columns: relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cq-1 | relative_path | competency-questions/cq-1 | None |
| cq-1 | iri | competency-questions-cq-1 | None |
| cq-1 | name | steps-and-order | None |
| cq-2 | relative_path | competency-questions/cq-2 | None |
| cq-2 | iri | competency-questions-cq-2 | None |
| cq-2 | name | who-approves | None |
| cq-3 | relative_path | competency-questions/cq-3 | None |
| cq-3 | iri | competency-questions-cq-3 | None |
| cq-3 | name | ai-vs-human-steps | None |
| cq-4 | relative_path | competency-questions/cq-4 | None |
| cq-4 | iri | competency-questions-cq-4 | None |
| cq-4 | name | artifact-lineage | None |
| cq-5 | relative_path | competency-questions/cq-5 | None |
| cq-5 | iri | competency-questions-cq-5 | None |
| cq-5 | name | stale-over-12-months | None |
| cq-6 | relative_path | competency-questions/cq-6 | None |
| cq-6 | iri | competency-questions-cq-6 | None |
| cq-6 | name | escalation-path | None |
| cq-7 | relative_path | competency-questions/cq-7 | None |
| cq-7 | iri | competency-questions-cq-7 | None |
| ... | ... | (4 more) | ... |

### human_agents

- Fields: 0/10 (0.0%)
- Computed columns: relative_path, iri

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-david-chen | relative_path | human-agents/ntwf-david-chen | None |
| ntwf-david-chen | iri | human-agents-ntwf-david-chen | None |
| ntwf-james-okafor | relative_path | human-agents/ntwf-james-okafor | None |
| ntwf-james-okafor | iri | human-agents-ntwf-james-okafor | None |
| ntwf-maria-gonzalez | relative_path | human-agents/ntwf-maria-gonzal | None |
| ntwf-maria-gonzalez | iri | human-agents-ntwf-maria-gonzal | None |
| ntwf-priya-nair | relative_path | human-agents/ntwf-priya-nair | None |
| ntwf-priya-nair | iri | human-agents-ntwf-priya-nair | None |
| ntwf-sarah-kim | relative_path | human-agents/ntwf-sarah-kim | None |
| ntwf-sarah-kim | iri | human-agents-ntwf-sarah-kim | None |

### role_assignments

- Fields: 15/54 (27.8%)
- Computed columns: parent_path, relative_path, iri, name, filler_type, is_current, was_active_as_of_audit_date, is_agent_type_change, requires_compliance_audit

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| asn-deploy-health-ai-current | parent_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-ai-current | relative_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-ai-current | iri | roles-ntwf-deployment-health-r | None |
| asn-deploy-health-ai-current | name | ntwf-deployment-health-role [2 | None |
| asn-deploy-health-ai-current | filler_type | AIAgent | None |
| asn-deploy-health-ai-current | is_current | True | None |
| asn-deploy-health-ai-current | was_active_as_of_audit_date | True | None |
| asn-deploy-health-ai-current | is_agent_type_change | True | None |
| asn-deploy-health-ai-initial | parent_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-ai-initial | relative_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-ai-initial | iri | roles-ntwf-deployment-health-r | None |
| asn-deploy-health-ai-initial | name | ntwf-deployment-health-role [2 | None |
| asn-deploy-health-ai-initial | filler_type | AIAgent | None |
| asn-deploy-health-human-audit | parent_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-human-audit | relative_path | roles/ntwf-deployment-health-r | None |
| asn-deploy-health-human-audit | iri | roles-ntwf-deployment-health-r | None |
| asn-deploy-health-human-audit | name | ntwf-deployment-health-role [2 | None |
| asn-deploy-health-human-audit | filler_type | HumanAgent | None |
| asn-deploy-health-human-audit | is_agent_type_change | True | None |
| asn-deploy-health-human-audit | requires_compliance_audit | True | None |
| ... | ... | (19 more) | ... |

### approval_gates

- Fields: 0/7 (0.0%)
- Computed columns: parent_path, relative_path, iri, name, gate_role, gate_approver_human, has_human_approver

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ntwf-release-approval-gate | parent_path | workflows/production-deploymen | None |
| ntwf-release-approval-gate | relative_path | workflows/production-deploymen | None |
| ntwf-release-approval-gate | iri | workflows-production-deploymen | None |
| ntwf-release-approval-gate | name | release-approval-gate | None |
| ntwf-release-approval-gate | gate_role | ntwf-release-manager-role | None |
| ntwf-release-approval-gate | gate_approver_human | ntwf-maria-gonzalez | None |
| ntwf-release-approval-gate | has_human_approver | True | None |

### scenarios

- Fields: 0/36 (0.0%)
- Computed columns: relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ai-at-gate | relative_path | scenarios/ai-at-gate | None |
| ai-at-gate | iri | scenarios-ai-at-gate | None |
| ai-at-gate | name | ai-at-the-approval-gate | None |
| ai-release-manager | relative_path | scenarios/ai-release-manager | None |
| ai-release-manager | iri | scenarios-ai-release-manager | None |
| ai-release-manager | name | ai-fills-the-release-gate | None |
| all-human | relative_path | scenarios/all-human | None |
| all-human | iri | scenarios-all-human | None |
| all-human | name | all-human-release | None |
| break-delegation | relative_path | scenarios/break-delegation | None |
| break-delegation | iri | scenarios-break-delegation | None |
| break-delegation | name | cut-the-escalation-chain | None |
| break-derivation | relative_path | scenarios/break-derivation | None |
| break-derivation | iri | scenarios-break-derivation | None |
| break-derivation | name | break-the-provenance-chain | None |
| dataset-unconsumed | relative_path | scenarios/dataset-unconsumed | None |
| dataset-unconsumed | iri | scenarios-dataset-unconsumed | None |
| dataset-unconsumed | name | detach-the-risk-dataset | None |
| drop-final-edge | relative_path | scenarios/drop-final-edge | None |
| drop-final-edge | iri | scenarios-drop-final-edge | None |
| ... | ... | (16 more) | ... |

### conformance_tests

- Fields: 0/216 (0.0%)
- Computed columns: relative_path, iri, name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| agent-type-change | relative_path | conformance-tests/agent-type-c | None |
| agent-type-change | iri | conformance-tests-agent-type-c | None |
| agent-type-change | name | agent-type-change-detected | None |
| approval-human-filled | relative_path | conformance-tests/approval-hum | None |
| approval-human-filled | iri | conformance-tests-approval-hum | None |
| approval-human-filled | name | gate-step-is-human-filled-at-s | None |
| approval-violation-false | relative_path | conformance-tests/approval-vio | None |
| approval-violation-false | iri | conformance-tests-approval-vio | None |
| approval-violation-false | name | no-approval-violation-at-seed | None |
| artifact-attribution | relative_path | conformance-tests/artifact-att | None |
| artifact-attribution | iri | conformance-tests-artifact-att | None |
| artifact-attribution | name | risk-report-attributed-to-the- | None |
| artifact-derivation | relative_path | conformance-tests/artifact-der | None |
| artifact-derivation | iri | conformance-tests-artifact-der | None |
| artifact-derivation | name | release-authorization-derives- | None |
| artifact-has-parent | relative_path | conformance-tests/artifact-has | None |
| artifact-has-parent | iri | conformance-tests-artifact-has | None |
| artifact-has-parent | name | post-deploy-report-has-a-deriv | None |
| asserted-pair-count | relative_path | conformance-tests/asserted-pai | None |
| asserted-pair-count | iri | conformance-tests-asserted-pai | None |
| ... | ... | (196 more) | ... |
