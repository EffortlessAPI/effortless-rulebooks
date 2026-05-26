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

-- WorkflowSteps
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_workflow;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_workflow
  FOREIGN KEY (workflow) REFERENCES workflows (workflow_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_assigned_role;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_assigned_role
  FOREIGN KEY (assigned_role) REFERENCES roles (role_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_approval_gate;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_approval_gate
  FOREIGN KEY (approval_gate) REFERENCES approval_gates (approval_gate_id);
ALTER TABLE workflow_steps DROP CONSTRAINT IF EXISTS fk_workflow_steps_preceded_by_steps;
ALTER TABLE workflow_steps ADD CONSTRAINT fk_workflow_steps_preceded_by_steps
  FOREIGN KEY (preceded_by_steps) REFERENCES precedes_steps (precedes_step_id);

-- PrecedesSteps
ALTER TABLE precedes_steps DROP CONSTRAINT IF EXISTS fk_precedes_steps_workflow_step;
ALTER TABLE precedes_steps ADD CONSTRAINT fk_precedes_steps_workflow_step
  FOREIGN KEY (workflow_step) REFERENCES workflow_steps (workflow_step_id);

-- Roles
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_display_name;
ALTER TABLE roles ADD CONSTRAINT fk_roles_display_name
  FOREIGN KEY (display_name) REFERENCES roles (role_id);
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
ALTER TABLE ai_agents DROP CONSTRAINT IF EXISTS fk_ai_agents_display_name;
ALTER TABLE ai_agents ADD CONSTRAINT fk_ai_agents_display_name
  FOREIGN KEY (display_name) REFERENCES ai_agents (ai_agent_id);
ALTER TABLE ai_agents DROP CONSTRAINT IF EXISTS fk_ai_agents_roles;
ALTER TABLE ai_agents ADD CONSTRAINT fk_ai_agents_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- AutomatedPipelines
ALTER TABLE automated_pipelines DROP CONSTRAINT IF EXISTS fk_automated_pipelines_roles;
ALTER TABLE automated_pipelines ADD CONSTRAINT fk_automated_pipelines_roles
  FOREIGN KEY (roles) REFERENCES roles (role_id);

-- 17 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
