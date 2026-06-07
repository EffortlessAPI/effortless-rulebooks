-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- Workflows
ALTER TABLE workflows DROP CONSTRAINT IF EXISTS fk_workflows_workflow_status;
ALTER TABLE workflows ADD CONSTRAINT fk_workflows_workflow_status
  FOREIGN KEY (workflow_status) REFERENCES workflow_status_concepts (concept_id);
ALTER TABLE workflows DROP CONSTRAINT IF EXISTS fk_workflows_workflow_steps;
ALTER TABLE workflows ADD CONSTRAINT fk_workflows_workflow_steps
  FOREIGN KEY (workflow_steps) REFERENCES workflow_steps (workflow_step_id);

-- WorkflowSteps
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_workflow;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_workflow
  FOREIGN KEY (workflow) REFERENCES workflows (workflow_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_assigned_role;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_assigned_role
  FOREIGN KEY (assigned_role) REFERENCES roles (role_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_consumes_dataset;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_consumes_dataset
  FOREIGN KEY (consumes_dataset) REFERENCES datasets (dataset_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_produces_artifacts;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_produces_artifacts
  FOREIGN KEY (produces_artifacts) REFERENCES workflow_artifacts (artifact_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_approval_gate;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_approval_gate
  FOREIGN KEY (approval_gate) REFERENCES approval_gates (approval_gate_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_precedes;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_precedes
  FOREIGN KEY (precedes) REFERENCES step_precedence (step_precedence_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_preceded_by;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_preceded_by
  FOREIGN KEY (preceded_by) REFERENCES step_precedence (step_precedence_id);

-- ApprovalGates
ALTER TABLE approval_gates DROP CONSTRAINT IF EXISTS fk_approval_gates_workflow_step;
ALTER TABLE approval_gates ADD CONSTRAINT fk_approval_gates_workflow_step
  FOREIGN KEY (workflow_step) REFERENCES workflow_steps (workflow_step_id);

-- StepPrecedence
ALTER TABLE step_precedence DROP CONSTRAINT IF EXISTS fk_step_precedence_from_step;
ALTER TABLE step_precedence ADD CONSTRAINT fk_step_precedence_from_step
  FOREIGN KEY (from_step) REFERENCES workflow_steps (workflow_step_id);
ALTER TABLE step_precedence DROP CONSTRAINT IF EXISTS fk_step_precedence_to_step;
ALTER TABLE step_precedence ADD CONSTRAINT fk_step_precedence_to_step
  FOREIGN KEY (to_step) REFERENCES workflow_steps (workflow_step_id);

-- Roles
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_has_capability;
ALTER TABLE roles ADD CONSTRAINT fk_roles_has_capability
  FOREIGN KEY (has_capability) REFERENCES agent_capability_concepts (concept_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_filled_by_human_agent;
ALTER TABLE roles ADD CONSTRAINT fk_roles_filled_by_human_agent
  FOREIGN KEY (filled_by_human_agent) REFERENCES human_agents (human_agent_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_filled_by_ai_agent;
ALTER TABLE roles ADD CONSTRAINT fk_roles_filled_by_ai_agent
  FOREIGN KEY (filled_by_ai_agent) REFERENCES ai_agents (ai_agent_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_filled_by_automated_pipeline;
ALTER TABLE roles ADD CONSTRAINT fk_roles_filled_by_automated_pipeline
  FOREIGN KEY (filled_by_automated_pipeline) REFERENCES automated_pipelines (automated_pipeline_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_owned_by;
ALTER TABLE roles ADD CONSTRAINT fk_roles_owned_by
  FOREIGN KEY (owned_by) REFERENCES departments (department_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_delegates_to;
ALTER TABLE roles ADD CONSTRAINT fk_roles_delegates_to
  FOREIGN KEY (delegates_to) REFERENCES roles (role_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_workflow_steps;
ALTER TABLE roles ADD CONSTRAINT fk_roles_workflow_steps
  FOREIGN KEY (workflow_steps) REFERENCES workflow_steps (workflow_step_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_from_delegates_to;
ALTER TABLE roles ADD CONSTRAINT fk_roles_from_delegates_to
  FOREIGN KEY (from_delegates_to) REFERENCES roles (role_id);

-- Departments
ALTER TABLE departments DROP CONSTRAINT IF EXISTS fk_departments_roles;
ALTER TABLE departments ADD CONSTRAINT fk_departments_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- HumanAgents
ALTER TABLE human_agents DROP CONSTRAINT IF EXISTS fk_human_agents_roles;
ALTER TABLE human_agents ADD CONSTRAINT fk_human_agents_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- AIAgents
ALTER TABLE ai_agents DROP CONSTRAINT IF EXISTS fk_ai_agents_roles;
ALTER TABLE ai_agents ADD CONSTRAINT fk_ai_agents_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- AutomatedPipelines
ALTER TABLE automated_pipelines DROP CONSTRAINT IF EXISTS fk_automated_pipelines_roles;
ALTER TABLE automated_pipelines ADD CONSTRAINT fk_automated_pipelines_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- WorkflowStatusConcepts
ALTER TABLE workflow_status_concepts DROP CONSTRAINT IF EXISTS fk_workflow_status_concepts_workflows;
ALTER TABLE workflow_status_concepts ADD CONSTRAINT fk_workflow_status_concepts_workflows
  FOREIGN KEY (workflows) REFERENCES workflows (workflow_id);

-- ComplianceVerdictConcepts
ALTER TABLE compliance_verdict_concepts DROP CONSTRAINT IF EXISTS fk_compliance_verdict_concepts_compliance_verdicts;
ALTER TABLE compliance_verdict_concepts ADD CONSTRAINT fk_compliance_verdict_concepts_compliance_verdicts
  FOREIGN KEY (compliance_verdicts) REFERENCES compliance_verdicts (compliance_verdict_id);

-- AgentCapabilityConcepts
ALTER TABLE agent_capability_concepts DROP CONSTRAINT IF EXISTS fk_agent_capability_concepts_roles;
ALTER TABLE agent_capability_concepts ADD CONSTRAINT fk_agent_capability_concepts_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- Datasets
ALTER TABLE datasets DROP CONSTRAINT IF EXISTS fk_datasets_consumed_by_steps;
ALTER TABLE datasets ADD CONSTRAINT fk_datasets_consumed_by_steps
  FOREIGN KEY (consumed_by_steps) REFERENCES workflow_steps (workflow_step_id);

-- WorkflowArtifacts
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_produced_by_step;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_produced_by_step
  FOREIGN KEY (produced_by_step) REFERENCES workflow_steps (workflow_step_id);
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_derived_from_artifact;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_derived_from_artifact
  FOREIGN KEY (derived_from_artifact) REFERENCES workflow_artifacts (artifact_id);
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_attributed_to_human_agent;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_attributed_to_human_agent
  FOREIGN KEY (attributed_to_human_agent) REFERENCES human_agents (human_agent_id);
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_attributed_to_ai_agent;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_attributed_to_ai_agent
  FOREIGN KEY (attributed_to_ai_agent) REFERENCES ai_agents (ai_agent_id);
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_attributed_to_automated_pipeline;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_attributed_to_automated_pipeline
  FOREIGN KEY (attributed_to_automated_pipeline) REFERENCES automated_pipelines (automated_pipeline_id);

-- ComplianceVerdicts
ALTER TABLE compliance_verdicts DROP CONSTRAINT IF EXISTS fk_compliance_verdicts_workflow;
ALTER TABLE compliance_verdicts ADD CONSTRAINT fk_compliance_verdicts_workflow
  FOREIGN KEY (workflow) REFERENCES workflows (workflow_id);
ALTER TABLE compliance_verdicts DROP CONSTRAINT IF EXISTS fk_compliance_verdicts_verdict_concept;
ALTER TABLE compliance_verdicts ADD CONSTRAINT fk_compliance_verdicts_verdict_concept
  FOREIGN KEY (verdict_concept) REFERENCES compliance_verdict_concepts (concept_id);

-- 35 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
