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
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_requires_artifacts;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_requires_artifacts
  FOREIGN KEY (requires_artifacts) REFERENCES workflow_artifacts (artifact_id);
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
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_role_assignments;
ALTER TABLE roles ADD CONSTRAINT fk_roles_role_assignments
  FOREIGN KEY (role_assignments) REFERENCES role_assignments (role_assignment_id);

-- RoleAssignments
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_role;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_role
  FOREIGN KEY (role) REFERENCES roles (role_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_filled_by_human_agent;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_filled_by_human_agent
  FOREIGN KEY (filled_by_human_agent) REFERENCES human_agents (human_agent_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_filled_by_ai_agent;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_filled_by_ai_agent
  FOREIGN KEY (filled_by_ai_agent) REFERENCES ai_agents (ai_agent_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_filled_by_automated_pipeline;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_filled_by_automated_pipeline
  FOREIGN KEY (filled_by_automated_pipeline) REFERENCES automated_pipelines (automated_pipeline_id);

-- Departments
ALTER TABLE departments DROP CONSTRAINT IF EXISTS fk_departments_roles;
ALTER TABLE departments ADD CONSTRAINT fk_departments_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- HumanAgents
ALTER TABLE human_agents DROP CONSTRAINT IF EXISTS fk_human_agents_roles;
ALTER TABLE human_agents ADD CONSTRAINT fk_human_agents_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);
ALTER TABLE human_agents DROP CONSTRAINT IF EXISTS fk_human_agents_role_assignments;
ALTER TABLE human_agents ADD CONSTRAINT fk_human_agents_role_assignments
  FOREIGN KEY (role_assignments) REFERENCES role_assignments (role_assignment_id);

-- AIAgents
ALTER TABLE ai_agents DROP CONSTRAINT IF EXISTS fk_ai_agents_roles;
ALTER TABLE ai_agents ADD CONSTRAINT fk_ai_agents_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);
ALTER TABLE ai_agents DROP CONSTRAINT IF EXISTS fk_ai_agents_role_assignments;
ALTER TABLE ai_agents ADD CONSTRAINT fk_ai_agents_role_assignments
  FOREIGN KEY (role_assignments) REFERENCES role_assignments (role_assignment_id);
ALTER TABLE ai_agents DROP CONSTRAINT IF EXISTS fk_ai_agents_attributed_artifacts;
ALTER TABLE ai_agents ADD CONSTRAINT fk_ai_agents_attributed_artifacts
  FOREIGN KEY (attributed_artifacts) REFERENCES workflow_artifacts (artifact_id);

-- AutomatedPipelines
ALTER TABLE automated_pipelines DROP CONSTRAINT IF EXISTS fk_automated_pipelines_roles;
ALTER TABLE automated_pipelines ADD CONSTRAINT fk_automated_pipelines_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);
ALTER TABLE automated_pipelines DROP CONSTRAINT IF EXISTS fk_automated_pipelines_role_assignments;
ALTER TABLE automated_pipelines ADD CONSTRAINT fk_automated_pipelines_role_assignments
  FOREIGN KEY (role_assignments) REFERENCES role_assignments (role_assignment_id);

-- WorkflowStatusConcepts
ALTER TABLE workflow_status_concepts DROP CONSTRAINT IF EXISTS fk_workflow_status_concepts_workflows;
ALTER TABLE workflow_status_concepts ADD CONSTRAINT fk_workflow_status_concepts_workflows
  FOREIGN KEY (workflows) REFERENCES workflows (workflow_id);

-- AgentCapabilityConcepts
ALTER TABLE agent_capability_concepts DROP CONSTRAINT IF EXISTS fk_agent_capability_concepts_roles;
ALTER TABLE agent_capability_concepts ADD CONSTRAINT fk_agent_capability_concepts_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- ArtifactTypeConcepts
ALTER TABLE artifact_type_concepts DROP CONSTRAINT IF EXISTS fk_artifact_type_concepts_workflow_artifacts;
ALTER TABLE artifact_type_concepts ADD CONSTRAINT fk_artifact_type_concepts_workflow_artifacts
  FOREIGN KEY (workflow_artifacts) REFERENCES workflow_artifacts (artifact_id);

-- Datasets
ALTER TABLE datasets DROP CONSTRAINT IF EXISTS fk_datasets_consumed_by_steps;
ALTER TABLE datasets ADD CONSTRAINT fk_datasets_consumed_by_steps
  FOREIGN KEY (consumed_by_steps) REFERENCES workflow_steps (workflow_step_id);

-- WorkflowArtifacts
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_artifact_type;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_artifact_type
  FOREIGN KEY (artifact_type) REFERENCES artifact_type_concepts (concept_id);
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_produced_by_step;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_produced_by_step
  FOREIGN KEY (produced_by_step) REFERENCES workflow_steps (workflow_step_id);
ALTER TABLE workflow_artifacts DROP CONSTRAINT IF EXISTS fk_workflow_artifacts_required_by_steps;
ALTER TABLE workflow_artifacts ADD CONSTRAINT fk_workflow_artifacts_required_by_steps
  FOREIGN KEY (required_by_steps) REFERENCES workflow_steps (workflow_step_id);
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

-- GovernanceRoles
ALTER TABLE governance_roles DROP CONSTRAINT IF EXISTS fk_governance_roles_approved_changes;
ALTER TABLE governance_roles ADD CONSTRAINT fk_governance_roles_approved_changes
  FOREIGN KEY (approved_changes) REFERENCES change_log (change_log_id);

-- ChangeLog
ALTER TABLE change_log DROP CONSTRAINT IF EXISTS fk_change_log_approved_by;
ALTER TABLE change_log ADD CONSTRAINT fk_change_log_approved_by
  FOREIGN KEY (approved_by) REFERENCES governance_roles (governance_role_id);

-- 47 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
