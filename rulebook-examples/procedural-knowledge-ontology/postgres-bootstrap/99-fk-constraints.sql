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

-- RulebookReleases
ALTER TABLE rulebook_releases DROP CONSTRAINT IF EXISTS fk_rulebook_releases_governed_model;
ALTER TABLE rulebook_releases ADD CONSTRAINT fk_rulebook_releases_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE rulebook_releases DROP CONSTRAINT IF EXISTS fk_rulebook_releases_previous_release;
ALTER TABLE rulebook_releases ADD CONSTRAINT fk_rulebook_releases_previous_release
  FOREIGN KEY (previous_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE rulebook_releases DROP CONSTRAINT IF EXISTS fk_rulebook_releases_version_decided_by_agent;
ALTER TABLE rulebook_releases ADD CONSTRAINT fk_rulebook_releases_version_decided_by_agent
  FOREIGN KEY (version_decided_by_agent) REFERENCES agents (agent_id);
ALTER TABLE rulebook_releases DROP CONSTRAINT IF EXISTS fk_rulebook_releases_approved_by_agent;
ALTER TABLE rulebook_releases ADD CONSTRAINT fk_rulebook_releases_approved_by_agent
  FOREIGN KEY (approved_by_agent) REFERENCES agents (agent_id);

-- OntologyProfiles
ALTER TABLE ontology_profiles DROP CONSTRAINT IF EXISTS fk_ontology_profiles_evaluation_context;
ALTER TABLE ontology_profiles ADD CONSTRAINT fk_ontology_profiles_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);
ALTER TABLE ontology_profiles DROP CONSTRAINT IF EXISTS fk_ontology_profiles_prerequisite_profile;
ALTER TABLE ontology_profiles ADD CONSTRAINT fk_ontology_profiles_prerequisite_profile
  FOREIGN KEY (prerequisite_profile) REFERENCES ontology_profiles (ontology_profile_id);

-- Agents
ALTER TABLE agents DROP CONSTRAINT IF EXISTS fk_agents_organization;
ALTER TABLE agents ADD CONSTRAINT fk_agents_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE agents DROP CONSTRAINT IF EXISTS fk_agents_represents_organization;
ALTER TABLE agents ADD CONSTRAINT fk_agents_represents_organization
  FOREIGN KEY (represents_organization) REFERENCES organizations (organization_id);

-- Roles
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_organization;
ALTER TABLE roles ADD CONSTRAINT fk_roles_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_current_agent;
ALTER TABLE roles ADD CONSTRAINT fk_roles_current_agent
  FOREIGN KEY (current_agent) REFERENCES agents (agent_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_current_assignment;
ALTER TABLE roles ADD CONSTRAINT fk_roles_current_assignment
  FOREIGN KEY (current_assignment) REFERENCES role_assignments (role_assignment_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_specializes_role;
ALTER TABLE roles ADD CONSTRAINT fk_roles_specializes_role
  FOREIGN KEY (specializes_role) REFERENCES roles (role_id);
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_escalation_backup_role;
ALTER TABLE roles ADD CONSTRAINT fk_roles_escalation_backup_role
  FOREIGN KEY (escalation_backup_role) REFERENCES roles (role_id);

-- RoleAssignments
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_role;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_role
  FOREIGN KEY (role) REFERENCES roles (role_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_agent;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_evaluation_context;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_supersedes_assignment;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_supersedes_assignment
  FOREIGN KEY (supersedes_assignment) REFERENCES role_assignments (role_assignment_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_approving_authority_role;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_approving_authority_role
  FOREIGN KEY (approving_authority_role) REFERENCES roles (role_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_authorizing_change_request;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_authorizing_change_request
  FOREIGN KEY (authorizing_change_request) REFERENCES change_requests (change_request_id);
ALTER TABLE role_assignments DROP CONSTRAINT IF EXISTS fk_role_assignments_for_procedure_version;
ALTER TABLE role_assignments ADD CONSTRAINT fk_role_assignments_for_procedure_version
  FOREIGN KEY (for_procedure_version) REFERENCES procedure_versions (procedure_version_id);

-- CommunitiesOfPractice
ALTER TABLE communities_of_practice DROP CONSTRAINT IF EXISTS fk_communities_of_practice_organization;
ALTER TABLE communities_of_practice ADD CONSTRAINT fk_communities_of_practice_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE communities_of_practice DROP CONSTRAINT IF EXISTS fk_communities_of_practice_steward_role;
ALTER TABLE communities_of_practice ADD CONSTRAINT fk_communities_of_practice_steward_role
  FOREIGN KEY (steward_role) REFERENCES roles (role_id);
ALTER TABLE communities_of_practice DROP CONSTRAINT IF EXISTS fk_communities_of_practice_own_vocabulary;
ALTER TABLE communities_of_practice ADD CONSTRAINT fk_communities_of_practice_own_vocabulary
  FOREIGN KEY (own_vocabulary) REFERENCES vocabularies (vocabulary_id);

-- Mentorships
ALTER TABLE mentorships DROP CONSTRAINT IF EXISTS fk_mentorships_community_of_practice;
ALTER TABLE mentorships ADD CONSTRAINT fk_mentorships_community_of_practice
  FOREIGN KEY (community_of_practice) REFERENCES communities_of_practice (community_of_practice_id);
ALTER TABLE mentorships DROP CONSTRAINT IF EXISTS fk_mentorships_mentor_agent;
ALTER TABLE mentorships ADD CONSTRAINT fk_mentorships_mentor_agent
  FOREIGN KEY (mentor_agent) REFERENCES agents (agent_id);
ALTER TABLE mentorships DROP CONSTRAINT IF EXISTS fk_mentorships_learner_agent;
ALTER TABLE mentorships ADD CONSTRAINT fk_mentorships_learner_agent
  FOREIGN KEY (learner_agent) REFERENCES agents (agent_id);
ALTER TABLE mentorships DROP CONSTRAINT IF EXISTS fk_mentorships_evaluation_context;
ALTER TABLE mentorships ADD CONSTRAINT fk_mentorships_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- ProcedureTypes
ALTER TABLE procedure_types DROP CONSTRAINT IF EXISTS fk_procedure_types_broader_procedure_type;
ALTER TABLE procedure_types ADD CONSTRAINT fk_procedure_types_broader_procedure_type
  FOREIGN KEY (broader_procedure_type) REFERENCES procedure_types (procedure_type_id);
ALTER TABLE procedure_types DROP CONSTRAINT IF EXISTS fk_procedure_types_distinguishing_facet;
ALTER TABLE procedure_types ADD CONSTRAINT fk_procedure_types_distinguishing_facet
  FOREIGN KEY (distinguishing_facet) REFERENCES classification_facets (classification_facet_id);

-- Procedures
ALTER TABLE procedures DROP CONSTRAINT IF EXISTS fk_procedures_procedure_type;
ALTER TABLE procedures ADD CONSTRAINT fk_procedures_procedure_type
  FOREIGN KEY (procedure_type) REFERENCES procedure_types (procedure_type_id);
ALTER TABLE procedures DROP CONSTRAINT IF EXISTS fk_procedures_owner_organization;
ALTER TABLE procedures ADD CONSTRAINT fk_procedures_owner_organization
  FOREIGN KEY (owner_organization) REFERENCES organizations (organization_id);
ALTER TABLE procedures DROP CONSTRAINT IF EXISTS fk_procedures_adopted_by_organization;
ALTER TABLE procedures ADD CONSTRAINT fk_procedures_adopted_by_organization
  FOREIGN KEY (adopted_by_organization) REFERENCES organizations (organization_id);
ALTER TABLE procedures DROP CONSTRAINT IF EXISTS fk_procedures_current_version_key;
ALTER TABLE procedures ADD CONSTRAINT fk_procedures_current_version_key
  FOREIGN KEY (current_version_key) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE procedures DROP CONSTRAINT IF EXISTS fk_procedures_template_procedure;
ALTER TABLE procedures ADD CONSTRAINT fk_procedures_template_procedure
  FOREIGN KEY (template_procedure) REFERENCES procedures (procedure_id);
ALTER TABLE procedures DROP CONSTRAINT IF EXISTS fk_procedures_required_by_regulation;
ALTER TABLE procedures ADD CONSTRAINT fk_procedures_required_by_regulation
  FOREIGN KEY (required_by_regulation) REFERENCES regulatory_frameworks (regulatory_framework_id);

-- ProcedureVersions
ALTER TABLE procedure_versions DROP CONSTRAINT IF EXISTS fk_procedure_versions_procedure;
ALTER TABLE procedure_versions ADD CONSTRAINT fk_procedure_versions_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE procedure_versions DROP CONSTRAINT IF EXISTS fk_procedure_versions_status;
ALTER TABLE procedure_versions ADD CONSTRAINT fk_procedure_versions_status
  FOREIGN KEY (status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE procedure_versions DROP CONSTRAINT IF EXISTS fk_procedure_versions_created_by_agent;
ALTER TABLE procedure_versions ADD CONSTRAINT fk_procedure_versions_created_by_agent
  FOREIGN KEY (created_by_agent) REFERENCES agents (agent_id);
ALTER TABLE procedure_versions DROP CONSTRAINT IF EXISTS fk_procedure_versions_modified_by_agent;
ALTER TABLE procedure_versions ADD CONSTRAINT fk_procedure_versions_modified_by_agent
  FOREIGN KEY (modified_by_agent) REFERENCES agents (agent_id);
ALTER TABLE procedure_versions DROP CONSTRAINT IF EXISTS fk_procedure_versions_evaluation_context;
ALTER TABLE procedure_versions ADD CONSTRAINT fk_procedure_versions_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- ProcedureVersionLinks
ALTER TABLE procedure_version_links DROP CONSTRAINT IF EXISTS fk_procedure_version_links_previous_procedure_version;
ALTER TABLE procedure_version_links ADD CONSTRAINT fk_procedure_version_links_previous_procedure_version
  FOREIGN KEY (previous_procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE procedure_version_links DROP CONSTRAINT IF EXISTS fk_procedure_version_links_next_procedure_version;
ALTER TABLE procedure_version_links ADD CONSTRAINT fk_procedure_version_links_next_procedure_version
  FOREIGN KEY (next_procedure_version) REFERENCES procedure_versions (procedure_version_id);

-- ProcedureStatusChanges
ALTER TABLE procedure_status_changes DROP CONSTRAINT IF EXISTS fk_procedure_status_changes_procedure_version;
ALTER TABLE procedure_status_changes ADD CONSTRAINT fk_procedure_status_changes_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE procedure_status_changes DROP CONSTRAINT IF EXISTS fk_procedure_status_changes_from_status;
ALTER TABLE procedure_status_changes ADD CONSTRAINT fk_procedure_status_changes_from_status
  FOREIGN KEY (from_status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE procedure_status_changes DROP CONSTRAINT IF EXISTS fk_procedure_status_changes_to_status;
ALTER TABLE procedure_status_changes ADD CONSTRAINT fk_procedure_status_changes_to_status
  FOREIGN KEY (to_status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE procedure_status_changes DROP CONSTRAINT IF EXISTS fk_procedure_status_changes_changed_by_agent;
ALTER TABLE procedure_status_changes ADD CONSTRAINT fk_procedure_status_changes_changed_by_agent
  FOREIGN KEY (changed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE procedure_status_changes DROP CONSTRAINT IF EXISTS fk_procedure_status_changes_procedure_execution;
ALTER TABLE procedure_status_changes ADD CONSTRAINT fk_procedure_status_changes_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);

-- Steps
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_procedure_version;
ALTER TABLE steps ADD CONSTRAINT fk_steps_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_assigned_role;
ALTER TABLE steps ADD CONSTRAINT fk_steps_assigned_role
  FOREIGN KEY (assigned_role) REFERENCES roles (role_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_parent_step;
ALTER TABLE steps ADD CONSTRAINT fk_steps_parent_step
  FOREIGN KEY (parent_step) REFERENCES steps (step_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_first_child_step;
ALTER TABLE steps ADD CONSTRAINT fk_steps_first_child_step
  FOREIGN KEY (first_child_step) REFERENCES steps (step_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_verifies_step;
ALTER TABLE steps ADD CONSTRAINT fk_steps_verifies_step
  FOREIGN KEY (verifies_step) REFERENCES steps (step_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_remedy_for_error;
ALTER TABLE steps ADD CONSTRAINT fk_steps_remedy_for_error
  FOREIGN KEY (remedy_for_error) REFERENCES errors (error_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_calls_procedure;
ALTER TABLE steps ADD CONSTRAINT fk_steps_calls_procedure
  FOREIGN KEY (calls_procedure) REFERENCES procedures (procedure_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_isolates_energy_source;
ALTER TABLE steps ADD CONSTRAINT fk_steps_isolates_energy_source
  FOREIGN KEY (isolates_energy_source) REFERENCES energy_sources (energy_source_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_prerequisite_step;
ALTER TABLE steps ADD CONSTRAINT fk_steps_prerequisite_step
  FOREIGN KEY (prerequisite_step) REFERENCES steps (step_id);
ALTER TABLE steps DROP CONSTRAINT IF EXISTS fk_steps_stage;
ALTER TABLE steps ADD CONSTRAINT fk_steps_stage
  FOREIGN KEY (stage) REFERENCES process_stages (process_stage_id);

-- StepTransitions
ALTER TABLE step_transitions DROP CONSTRAINT IF EXISTS fk_step_transitions_procedure_version;
ALTER TABLE step_transitions ADD CONSTRAINT fk_step_transitions_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE step_transitions DROP CONSTRAINT IF EXISTS fk_step_transitions_from_step;
ALTER TABLE step_transitions ADD CONSTRAINT fk_step_transitions_from_step
  FOREIGN KEY (from_step) REFERENCES steps (step_id);
ALTER TABLE step_transitions DROP CONSTRAINT IF EXISTS fk_step_transitions_to_step;
ALTER TABLE step_transitions ADD CONSTRAINT fk_step_transitions_to_step
  FOREIGN KEY (to_step) REFERENCES steps (step_id);

-- StepActions
ALTER TABLE step_actions DROP CONSTRAINT IF EXISTS fk_step_actions_step;
ALTER TABLE step_actions ADD CONSTRAINT fk_step_actions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_actions DROP CONSTRAINT IF EXISTS fk_step_actions_action;
ALTER TABLE step_actions ADD CONSTRAINT fk_step_actions_action
  FOREIGN KEY ("action") REFERENCES actions (action_id);

-- StepFunctions
ALTER TABLE step_functions DROP CONSTRAINT IF EXISTS fk_step_functions_step;
ALTER TABLE step_functions ADD CONSTRAINT fk_step_functions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_functions DROP CONSTRAINT IF EXISTS fk_step_functions_function;
ALTER TABLE step_functions ADD CONSTRAINT fk_step_functions_function
  FOREIGN KEY (function) REFERENCES functions (function_id);

-- StepTools
ALTER TABLE step_tools DROP CONSTRAINT IF EXISTS fk_step_tools_step;
ALTER TABLE step_tools ADD CONSTRAINT fk_step_tools_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_tools DROP CONSTRAINT IF EXISTS fk_step_tools_tool;
ALTER TABLE step_tools ADD CONSTRAINT fk_step_tools_tool
  FOREIGN KEY (tool) REFERENCES tools (tool_id);

-- Requirements
ALTER TABLE requirements DROP CONSTRAINT IF EXISTS fk_requirements_witness_field_name;
ALTER TABLE requirements ADD CONSTRAINT fk_requirements_witness_field_name
  FOREIGN KEY (witness_field_name) REFERENCES rulebook_fields (rulebook_field_id);
ALTER TABLE requirements DROP CONSTRAINT IF EXISTS fk_requirements_accountable_role;
ALTER TABLE requirements ADD CONSTRAINT fk_requirements_accountable_role
  FOREIGN KEY (accountable_role) REFERENCES roles (role_id);
ALTER TABLE requirements DROP CONSTRAINT IF EXISTS fk_requirements_controlled_term;
ALTER TABLE requirements ADD CONSTRAINT fk_requirements_controlled_term
  FOREIGN KEY (controlled_term) REFERENCES vocabulary_terms (vocabulary_term_id);
ALTER TABLE requirements DROP CONSTRAINT IF EXISTS fk_requirements_regulatory_framework;
ALTER TABLE requirements ADD CONSTRAINT fk_requirements_regulatory_framework
  FOREIGN KEY (regulatory_framework) REFERENCES regulatory_frameworks (regulatory_framework_id);

-- StepRequirements
ALTER TABLE step_requirements DROP CONSTRAINT IF EXISTS fk_step_requirements_step;
ALTER TABLE step_requirements ADD CONSTRAINT fk_step_requirements_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_requirements DROP CONSTRAINT IF EXISTS fk_step_requirements_requirement;
ALTER TABLE step_requirements ADD CONSTRAINT fk_step_requirements_requirement
  FOREIGN KEY (requirement) REFERENCES requirements (requirement_id);

-- StepVerifications
ALTER TABLE step_verifications DROP CONSTRAINT IF EXISTS fk_step_verifications_step;
ALTER TABLE step_verifications ADD CONSTRAINT fk_step_verifications_step
  FOREIGN KEY (step) REFERENCES steps (step_id);

-- Rationales
ALTER TABLE rationales DROP CONSTRAINT IF EXISTS fk_rationales_procedure_version;
ALTER TABLE rationales ADD CONSTRAINT fk_rationales_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE rationales DROP CONSTRAINT IF EXISTS fk_rationales_step;
ALTER TABLE rationales ADD CONSTRAINT fk_rationales_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE rationales DROP CONSTRAINT IF EXISTS fk_rationales_status;
ALTER TABLE rationales ADD CONSTRAINT fk_rationales_status
  FOREIGN KEY (status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE rationales DROP CONSTRAINT IF EXISTS fk_rationales_authority_role;
ALTER TABLE rationales ADD CONSTRAINT fk_rationales_authority_role
  FOREIGN KEY (authority_role) REFERENCES roles (role_id);

-- Exceptions
ALTER TABLE exceptions DROP CONSTRAINT IF EXISTS fk_exceptions_procedure_version;
ALTER TABLE exceptions ADD CONSTRAINT fk_exceptions_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE exceptions DROP CONSTRAINT IF EXISTS fk_exceptions_trigger_step;
ALTER TABLE exceptions ADD CONSTRAINT fk_exceptions_trigger_step
  FOREIGN KEY (trigger_step) REFERENCES steps (step_id);
ALTER TABLE exceptions DROP CONSTRAINT IF EXISTS fk_exceptions_approval_role;
ALTER TABLE exceptions ADD CONSTRAINT fk_exceptions_approval_role
  FOREIGN KEY (approval_role) REFERENCES roles (role_id);
ALTER TABLE exceptions DROP CONSTRAINT IF EXISTS fk_exceptions_fallback_role;
ALTER TABLE exceptions ADD CONSTRAINT fk_exceptions_fallback_role
  FOREIGN KEY (fallback_role) REFERENCES roles (role_id);

-- Resources
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_approval_status;
ALTER TABLE resources ADD CONSTRAINT fk_resources_approval_status
  FOREIGN KEY (approval_status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_created_by_agent;
ALTER TABLE resources ADD CONSTRAINT fk_resources_created_by_agent
  FOREIGN KEY (created_by_agent) REFERENCES agents (agent_id);
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_modified_by_agent;
ALTER TABLE resources ADD CONSTRAINT fk_resources_modified_by_agent
  FOREIGN KEY (modified_by_agent) REFERENCES agents (agent_id);
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_extracted_from_resource;
ALTER TABLE resources ADD CONSTRAINT fk_resources_extracted_from_resource
  FOREIGN KEY (extracted_from_resource) REFERENCES resources (resource_id);
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_artifact_type_concept;
ALTER TABLE resources ADD CONSTRAINT fk_resources_artifact_type_concept
  FOREIGN KEY (artifact_type_concept) REFERENCES vocabulary_terms (vocabulary_term_id);
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_catalog_entry_for;
ALTER TABLE resources ADD CONSTRAINT fk_resources_catalog_entry_for
  FOREIGN KEY (catalog_entry_for) REFERENCES procedures (procedure_id);
ALTER TABLE resources DROP CONSTRAINT IF EXISTS fk_resources_compliance_record_for;
ALTER TABLE resources ADD CONSTRAINT fk_resources_compliance_record_for
  FOREIGN KEY (compliance_record_for) REFERENCES procedures (procedure_id);

-- ProcedureResources
ALTER TABLE procedure_resources DROP CONSTRAINT IF EXISTS fk_procedure_resources_procedure_version;
ALTER TABLE procedure_resources ADD CONSTRAINT fk_procedure_resources_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE procedure_resources DROP CONSTRAINT IF EXISTS fk_procedure_resources_resource;
ALTER TABLE procedure_resources ADD CONSTRAINT fk_procedure_resources_resource
  FOREIGN KEY (resource) REFERENCES resources (resource_id);

-- ElicitationSessions
ALTER TABLE elicitation_sessions DROP CONSTRAINT IF EXISTS fk_elicitation_sessions_procedure_version;
ALTER TABLE elicitation_sessions ADD CONSTRAINT fk_elicitation_sessions_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE elicitation_sessions DROP CONSTRAINT IF EXISTS fk_elicitation_sessions_method;
ALTER TABLE elicitation_sessions ADD CONSTRAINT fk_elicitation_sessions_method
  FOREIGN KEY (method) REFERENCES knowledge_methods (knowledge_method_id);
ALTER TABLE elicitation_sessions DROP CONSTRAINT IF EXISTS fk_elicitation_sessions_practitioner_agent;
ALTER TABLE elicitation_sessions ADD CONSTRAINT fk_elicitation_sessions_practitioner_agent
  FOREIGN KEY (practitioner_agent) REFERENCES agents (agent_id);
ALTER TABLE elicitation_sessions DROP CONSTRAINT IF EXISTS fk_elicitation_sessions_facilitator_agent;
ALTER TABLE elicitation_sessions ADD CONSTRAINT fk_elicitation_sessions_facilitator_agent
  FOREIGN KEY (facilitator_agent) REFERENCES agents (agent_id);
ALTER TABLE elicitation_sessions DROP CONSTRAINT IF EXISTS fk_elicitation_sessions_status;
ALTER TABLE elicitation_sessions ADD CONSTRAINT fk_elicitation_sessions_status
  FOREIGN KEY (status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE elicitation_sessions DROP CONSTRAINT IF EXISTS fk_elicitation_sessions_evaluation_context;
ALTER TABLE elicitation_sessions ADD CONSTRAINT fk_elicitation_sessions_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- KnowledgeFragments
ALTER TABLE knowledge_fragments DROP CONSTRAINT IF EXISTS fk_knowledge_fragments_procedure_version;
ALTER TABLE knowledge_fragments ADD CONSTRAINT fk_knowledge_fragments_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_fragments DROP CONSTRAINT IF EXISTS fk_knowledge_fragments_step;
ALTER TABLE knowledge_fragments ADD CONSTRAINT fk_knowledge_fragments_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE knowledge_fragments DROP CONSTRAINT IF EXISTS fk_knowledge_fragments_elicitation_session;
ALTER TABLE knowledge_fragments ADD CONSTRAINT fk_knowledge_fragments_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE knowledge_fragments DROP CONSTRAINT IF EXISTS fk_knowledge_fragments_source_agent;
ALTER TABLE knowledge_fragments ADD CONSTRAINT fk_knowledge_fragments_source_agent
  FOREIGN KEY (source_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_fragments DROP CONSTRAINT IF EXISTS fk_knowledge_fragments_owner_role;
ALTER TABLE knowledge_fragments ADD CONSTRAINT fk_knowledge_fragments_owner_role
  FOREIGN KEY (owner_role) REFERENCES roles (role_id);
ALTER TABLE knowledge_fragments DROP CONSTRAINT IF EXISTS fk_knowledge_fragments_evaluation_context;
ALTER TABLE knowledge_fragments ADD CONSTRAINT fk_knowledge_fragments_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- KnowledgeGaps
ALTER TABLE knowledge_gaps DROP CONSTRAINT IF EXISTS fk_knowledge_gaps_procedure_version;
ALTER TABLE knowledge_gaps ADD CONSTRAINT fk_knowledge_gaps_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_gaps DROP CONSTRAINT IF EXISTS fk_knowledge_gaps_step;
ALTER TABLE knowledge_gaps ADD CONSTRAINT fk_knowledge_gaps_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE knowledge_gaps DROP CONSTRAINT IF EXISTS fk_knowledge_gaps_owner_role;
ALTER TABLE knowledge_gaps ADD CONSTRAINT fk_knowledge_gaps_owner_role
  FOREIGN KEY (owner_role) REFERENCES roles (role_id);
ALTER TABLE knowledge_gaps DROP CONSTRAINT IF EXISTS fk_knowledge_gaps_evaluation_context;
ALTER TABLE knowledge_gaps ADD CONSTRAINT fk_knowledge_gaps_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);
ALTER TABLE knowledge_gaps DROP CONSTRAINT IF EXISTS fk_knowledge_gaps_drawn_out_by_session;
ALTER TABLE knowledge_gaps ADD CONSTRAINT fk_knowledge_gaps_drawn_out_by_session
  FOREIGN KEY (drawn_out_by_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE knowledge_gaps DROP CONSTRAINT IF EXISTS fk_knowledge_gaps_codified_as_fragment;
ALTER TABLE knowledge_gaps ADD CONSTRAINT fk_knowledge_gaps_codified_as_fragment
  FOREIGN KEY (codified_as_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);

-- FAQs
ALTER TABLE faqs DROP CONSTRAINT IF EXISTS fk_faqs_procedure_version;
ALTER TABLE faqs ADD CONSTRAINT fk_faqs_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE faqs DROP CONSTRAINT IF EXISTS fk_faqs_step;
ALTER TABLE faqs ADD CONSTRAINT fk_faqs_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE faqs DROP CONSTRAINT IF EXISTS fk_faqs_category;
ALTER TABLE faqs ADD CONSTRAINT fk_faqs_category
  FOREIGN KEY (category) REFERENCES faq_categories (faq_category_id);
ALTER TABLE faqs DROP CONSTRAINT IF EXISTS fk_faqs_target_kind;
ALTER TABLE faqs ADD CONSTRAINT fk_faqs_target_kind
  FOREIGN KEY (target_kind) REFERENCES faq_targets (faq_target_id);
ALTER TABLE faqs DROP CONSTRAINT IF EXISTS fk_faqs_resource;
ALTER TABLE faqs ADD CONSTRAINT fk_faqs_resource
  FOREIGN KEY (resource) REFERENCES resources (resource_id);

-- Explanations
ALTER TABLE explanations DROP CONSTRAINT IF EXISTS fk_explanations_procedure_version;
ALTER TABLE explanations ADD CONSTRAINT fk_explanations_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE explanations DROP CONSTRAINT IF EXISTS fk_explanations_step;
ALTER TABLE explanations ADD CONSTRAINT fk_explanations_step
  FOREIGN KEY (step) REFERENCES steps (step_id);

-- ProcedureExecutions
ALTER TABLE procedure_executions DROP CONSTRAINT IF EXISTS fk_procedure_executions_procedure_version;
ALTER TABLE procedure_executions ADD CONSTRAINT fk_procedure_executions_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE procedure_executions DROP CONSTRAINT IF EXISTS fk_procedure_executions_execution_status;
ALTER TABLE procedure_executions ADD CONSTRAINT fk_procedure_executions_execution_status
  FOREIGN KEY (execution_status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE procedure_executions DROP CONSTRAINT IF EXISTS fk_procedure_executions_executed_by_agent;
ALTER TABLE procedure_executions ADD CONSTRAINT fk_procedure_executions_executed_by_agent
  FOREIGN KEY (executed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE procedure_executions DROP CONSTRAINT IF EXISTS fk_procedure_executions_confirmed_by_agent;
ALTER TABLE procedure_executions ADD CONSTRAINT fk_procedure_executions_confirmed_by_agent
  FOREIGN KEY (confirmed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE procedure_executions DROP CONSTRAINT IF EXISTS fk_procedure_executions_facility;
ALTER TABLE procedure_executions ADD CONSTRAINT fk_procedure_executions_facility
  FOREIGN KEY (facility) REFERENCES facilities (facility_id);
ALTER TABLE procedure_executions DROP CONSTRAINT IF EXISTS fk_procedure_executions_executed_on_machine;
ALTER TABLE procedure_executions ADD CONSTRAINT fk_procedure_executions_executed_on_machine
  FOREIGN KEY (executed_on_machine) REFERENCES machines (machine_id);

-- StepExecutions
ALTER TABLE step_executions DROP CONSTRAINT IF EXISTS fk_step_executions_procedure_execution;
ALTER TABLE step_executions ADD CONSTRAINT fk_step_executions_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE step_executions DROP CONSTRAINT IF EXISTS fk_step_executions_step;
ALTER TABLE step_executions ADD CONSTRAINT fk_step_executions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_executions DROP CONSTRAINT IF EXISTS fk_step_executions_executed_by_agent;
ALTER TABLE step_executions ADD CONSTRAINT fk_step_executions_executed_by_agent
  FOREIGN KEY (executed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE step_executions DROP CONSTRAINT IF EXISTS fk_step_executions_execution_status;
ALTER TABLE step_executions ADD CONSTRAINT fk_step_executions_execution_status
  FOREIGN KEY (execution_status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE step_executions DROP CONSTRAINT IF EXISTS fk_step_executions_previous_step_execution;
ALTER TABLE step_executions ADD CONSTRAINT fk_step_executions_previous_step_execution
  FOREIGN KEY (previous_step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE step_executions DROP CONSTRAINT IF EXISTS fk_step_executions_confirmed_by_agent;
ALTER TABLE step_executions ADD CONSTRAINT fk_step_executions_confirmed_by_agent
  FOREIGN KEY (confirmed_by_agent) REFERENCES agents (agent_id);

-- RequirementSatisfactions
ALTER TABLE requirement_satisfactions DROP CONSTRAINT IF EXISTS fk_requirement_satisfactions_step_execution;
ALTER TABLE requirement_satisfactions ADD CONSTRAINT fk_requirement_satisfactions_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE requirement_satisfactions DROP CONSTRAINT IF EXISTS fk_requirement_satisfactions_requirement;
ALTER TABLE requirement_satisfactions ADD CONSTRAINT fk_requirement_satisfactions_requirement
  FOREIGN KEY (requirement) REFERENCES requirements (requirement_id);
ALTER TABLE requirement_satisfactions DROP CONSTRAINT IF EXISTS fk_requirement_satisfactions_evaluated_by_agent;
ALTER TABLE requirement_satisfactions ADD CONSTRAINT fk_requirement_satisfactions_evaluated_by_agent
  FOREIGN KEY (evaluated_by_agent) REFERENCES agents (agent_id);

-- IssueOccurrences
ALTER TABLE issue_occurrences DROP CONSTRAINT IF EXISTS fk_issue_occurrences_step_execution;
ALTER TABLE issue_occurrences ADD CONSTRAINT fk_issue_occurrences_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE issue_occurrences DROP CONSTRAINT IF EXISTS fk_issue_occurrences_error;
ALTER TABLE issue_occurrences ADD CONSTRAINT fk_issue_occurrences_error
  FOREIGN KEY (error) REFERENCES errors (error_id);
ALTER TABLE issue_occurrences DROP CONSTRAINT IF EXISTS fk_issue_occurrences_encountered_by_agent;
ALTER TABLE issue_occurrences ADD CONSTRAINT fk_issue_occurrences_encountered_by_agent
  FOREIGN KEY (encountered_by_agent) REFERENCES agents (agent_id);
ALTER TABLE issue_occurrences DROP CONSTRAINT IF EXISTS fk_issue_occurrences_redesign_change_request;
ALTER TABLE issue_occurrences ADD CONSTRAINT fk_issue_occurrences_redesign_change_request
  FOREIGN KEY (redesign_change_request) REFERENCES change_requests (change_request_id);

-- UserQuestions
ALTER TABLE user_questions DROP CONSTRAINT IF EXISTS fk_user_questions_step_execution;
ALTER TABLE user_questions ADD CONSTRAINT fk_user_questions_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE user_questions DROP CONSTRAINT IF EXISTS fk_user_questions_asked_by_agent;
ALTER TABLE user_questions ADD CONSTRAINT fk_user_questions_asked_by_agent
  FOREIGN KEY (asked_by_agent) REFERENCES agents (agent_id);
ALTER TABLE user_questions DROP CONSTRAINT IF EXISTS fk_user_questions_resolved_by_faq;
ALTER TABLE user_questions ADD CONSTRAINT fk_user_questions_resolved_by_faq
  FOREIGN KEY (resolved_by_faq) REFERENCES faqs (faq_id);
ALTER TABLE user_questions DROP CONSTRAINT IF EXISTS fk_user_questions_addressed_by_resource;
ALTER TABLE user_questions ADD CONSTRAINT fk_user_questions_addressed_by_resource
  FOREIGN KEY (addressed_by_resource) REFERENCES resources (resource_id);

-- UserFeedback
ALTER TABLE user_feedback DROP CONSTRAINT IF EXISTS fk_user_feedback_procedure_execution;
ALTER TABLE user_feedback ADD CONSTRAINT fk_user_feedback_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE user_feedback DROP CONSTRAINT IF EXISTS fk_user_feedback_provided_by_agent;
ALTER TABLE user_feedback ADD CONSTRAINT fk_user_feedback_provided_by_agent
  FOREIGN KEY (provided_by_agent) REFERENCES agents (agent_id);
ALTER TABLE user_feedback DROP CONSTRAINT IF EXISTS fk_user_feedback_change_request_key;
ALTER TABLE user_feedback ADD CONSTRAINT fk_user_feedback_change_request_key
  FOREIGN KEY (change_request_key) REFERENCES change_requests (change_request_id);

-- StewardshipAssignments
ALTER TABLE stewardship_assignments DROP CONSTRAINT IF EXISTS fk_stewardship_assignments_procedure_version;
ALTER TABLE stewardship_assignments ADD CONSTRAINT fk_stewardship_assignments_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE stewardship_assignments DROP CONSTRAINT IF EXISTS fk_stewardship_assignments_steward_role;
ALTER TABLE stewardship_assignments ADD CONSTRAINT fk_stewardship_assignments_steward_role
  FOREIGN KEY (steward_role) REFERENCES roles (role_id);
ALTER TABLE stewardship_assignments DROP CONSTRAINT IF EXISTS fk_stewardship_assignments_authority_role;
ALTER TABLE stewardship_assignments ADD CONSTRAINT fk_stewardship_assignments_authority_role
  FOREIGN KEY (authority_role) REFERENCES roles (role_id);
ALTER TABLE stewardship_assignments DROP CONSTRAINT IF EXISTS fk_stewardship_assignments_evaluation_context;
ALTER TABLE stewardship_assignments ADD CONSTRAINT fk_stewardship_assignments_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- ChangeRequests
ALTER TABLE change_requests DROP CONSTRAINT IF EXISTS fk_change_requests_procedure_version;
ALTER TABLE change_requests ADD CONSTRAINT fk_change_requests_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE change_requests DROP CONSTRAINT IF EXISTS fk_change_requests_requested_by_agent;
ALTER TABLE change_requests ADD CONSTRAINT fk_change_requests_requested_by_agent
  FOREIGN KEY (requested_by_agent) REFERENCES agents (agent_id);
ALTER TABLE change_requests DROP CONSTRAINT IF EXISTS fk_change_requests_authority_role;
ALTER TABLE change_requests ADD CONSTRAINT fk_change_requests_authority_role
  FOREIGN KEY (authority_role) REFERENCES roles (role_id);
ALTER TABLE change_requests DROP CONSTRAINT IF EXISTS fk_change_requests_evaluation_context;
ALTER TABLE change_requests ADD CONSTRAINT fk_change_requests_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- ReviewEvents
ALTER TABLE review_events DROP CONSTRAINT IF EXISTS fk_review_events_procedure_version;
ALTER TABLE review_events ADD CONSTRAINT fk_review_events_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE review_events DROP CONSTRAINT IF EXISTS fk_review_events_reviewed_by_agent;
ALTER TABLE review_events ADD CONSTRAINT fk_review_events_reviewed_by_agent
  FOREIGN KEY (reviewed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE review_events DROP CONSTRAINT IF EXISTS fk_review_events_related_change_request;
ALTER TABLE review_events ADD CONSTRAINT fk_review_events_related_change_request
  FOREIGN KEY (related_change_request) REFERENCES change_requests (change_request_id);
ALTER TABLE review_events DROP CONSTRAINT IF EXISTS fk_review_events_evaluation_context;
ALTER TABLE review_events ADD CONSTRAINT fk_review_events_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- LearningActivities
ALTER TABLE learning_activities DROP CONSTRAINT IF EXISTS fk_learning_activities_community_of_practice;
ALTER TABLE learning_activities ADD CONSTRAINT fk_learning_activities_community_of_practice
  FOREIGN KEY (community_of_practice) REFERENCES communities_of_practice (community_of_practice_id);
ALTER TABLE learning_activities DROP CONSTRAINT IF EXISTS fk_learning_activities_procedure_version;
ALTER TABLE learning_activities ADD CONSTRAINT fk_learning_activities_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE learning_activities DROP CONSTRAINT IF EXISTS fk_learning_activities_facilitator_agent;
ALTER TABLE learning_activities ADD CONSTRAINT fk_learning_activities_facilitator_agent
  FOREIGN KEY (facilitator_agent) REFERENCES agents (agent_id);
ALTER TABLE learning_activities DROP CONSTRAINT IF EXISTS fk_learning_activities_evidence_resource;
ALTER TABLE learning_activities ADD CONSTRAINT fk_learning_activities_evidence_resource
  FOREIGN KEY (evidence_resource) REFERENCES resources (resource_id);

-- OperationalBindings
ALTER TABLE operational_bindings DROP CONSTRAINT IF EXISTS fk_operational_bindings_procedure_version;
ALTER TABLE operational_bindings ADD CONSTRAINT fk_operational_bindings_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE operational_bindings DROP CONSTRAINT IF EXISTS fk_operational_bindings_step;
ALTER TABLE operational_bindings ADD CONSTRAINT fk_operational_bindings_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE operational_bindings DROP CONSTRAINT IF EXISTS fk_operational_bindings_resource;
ALTER TABLE operational_bindings ADD CONSTRAINT fk_operational_bindings_resource
  FOREIGN KEY (resource) REFERENCES resources (resource_id);
ALTER TABLE operational_bindings DROP CONSTRAINT IF EXISTS fk_operational_bindings_evaluation_context;
ALTER TABLE operational_bindings ADD CONSTRAINT fk_operational_bindings_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- CommunicationPolicies
ALTER TABLE communication_policies DROP CONSTRAINT IF EXISTS fk_communication_policies_procedure_version;
ALTER TABLE communication_policies ADD CONSTRAINT fk_communication_policies_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE communication_policies DROP CONSTRAINT IF EXISTS fk_communication_policies_approval_role;
ALTER TABLE communication_policies ADD CONSTRAINT fk_communication_policies_approval_role
  FOREIGN KEY (approval_role) REFERENCES roles (role_id);

-- MessageTemplates
ALTER TABLE message_templates DROP CONSTRAINT IF EXISTS fk_message_templates_communication_policy;
ALTER TABLE message_templates ADD CONSTRAINT fk_message_templates_communication_policy
  FOREIGN KEY (communication_policy) REFERENCES communication_policies (communication_policy_id);
ALTER TABLE message_templates DROP CONSTRAINT IF EXISTS fk_message_templates_resource;
ALTER TABLE message_templates ADD CONSTRAINT fk_message_templates_resource
  FOREIGN KEY (resource) REFERENCES resources (resource_id);
ALTER TABLE message_templates DROP CONSTRAINT IF EXISTS fk_message_templates_status;
ALTER TABLE message_templates ADD CONSTRAINT fk_message_templates_status
  FOREIGN KEY (status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE message_templates DROP CONSTRAINT IF EXISTS fk_message_templates_last_valid_approval;
ALTER TABLE message_templates ADD CONSTRAINT fk_message_templates_last_valid_approval
  FOREIGN KEY (last_valid_approval) REFERENCES template_approvals (template_approval_id);

-- SemanticMappings
ALTER TABLE semantic_mappings DROP CONSTRAINT IF EXISTS fk_semantic_mappings_ontology_profile;
ALTER TABLE semantic_mappings ADD CONSTRAINT fk_semantic_mappings_ontology_profile
  FOREIGN KEY (ontology_profile) REFERENCES ontology_profiles (ontology_profile_id);

-- RoleQuestions
ALTER TABLE role_questions DROP CONSTRAINT IF EXISTS fk_role_questions_asking_role;
ALTER TABLE role_questions ADD CONSTRAINT fk_role_questions_asking_role
  FOREIGN KEY (asking_role) REFERENCES roles (role_id);
ALTER TABLE role_questions DROP CONSTRAINT IF EXISTS fk_role_questions_witness_loop;
ALTER TABLE role_questions ADD CONSTRAINT fk_role_questions_witness_loop
  FOREIGN KEY (witness_loop) REFERENCES witness_loops (witness_loop_id);

-- RulebookFields
ALTER TABLE rulebook_fields DROP CONSTRAINT IF EXISTS fk_rulebook_fields_target_table;
ALTER TABLE rulebook_fields ADD CONSTRAINT fk_rulebook_fields_target_table
  FOREIGN KEY (target_table) REFERENCES rulebook_tables (rulebook_table_id);
ALTER TABLE rulebook_fields DROP CONSTRAINT IF EXISTS fk_rulebook_fields_invented_for_question;
ALTER TABLE rulebook_fields ADD CONSTRAINT fk_rulebook_fields_invented_for_question
  FOREIGN KEY (invented_for_question) REFERENCES role_questions (role_question_id);

-- TestCases
ALTER TABLE test_cases DROP CONSTRAINT IF EXISTS fk_test_cases_target_table;
ALTER TABLE test_cases ADD CONSTRAINT fk_test_cases_target_table
  FOREIGN KEY (target_table) REFERENCES rulebook_tables (rulebook_table_id);
ALTER TABLE test_cases DROP CONSTRAINT IF EXISTS fk_test_cases_defends_question;
ALTER TABLE test_cases ADD CONSTRAINT fk_test_cases_defends_question
  FOREIGN KEY (defends_question) REFERENCES role_questions (role_question_id);
ALTER TABLE test_cases DROP CONSTRAINT IF EXISTS fk_test_cases_suite;
ALTER TABLE test_cases ADD CONSTRAINT fk_test_cases_suite
  FOREIGN KEY (suite) REFERENCES test_suites (test_suite_id);

-- ExceptionInvocations
ALTER TABLE exception_invocations DROP CONSTRAINT IF EXISTS fk_exception_invocations_step_execution;
ALTER TABLE exception_invocations ADD CONSTRAINT fk_exception_invocations_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE exception_invocations DROP CONSTRAINT IF EXISTS fk_exception_invocations_exception;
ALTER TABLE exception_invocations ADD CONSTRAINT fk_exception_invocations_exception
  FOREIGN KEY (exception) REFERENCES exceptions (exception_id);
ALTER TABLE exception_invocations DROP CONSTRAINT IF EXISTS fk_exception_invocations_invoked_by_agent;
ALTER TABLE exception_invocations ADD CONSTRAINT fk_exception_invocations_invoked_by_agent
  FOREIGN KEY (invoked_by_agent) REFERENCES agents (agent_id);
ALTER TABLE exception_invocations DROP CONSTRAINT IF EXISTS fk_exception_invocations_approved_by_agent;
ALTER TABLE exception_invocations ADD CONSTRAINT fk_exception_invocations_approved_by_agent
  FOREIGN KEY (approved_by_agent) REFERENCES agents (agent_id);

-- VerificationOutcomes
ALTER TABLE verification_outcomes DROP CONSTRAINT IF EXISTS fk_verification_outcomes_step_execution;
ALTER TABLE verification_outcomes ADD CONSTRAINT fk_verification_outcomes_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE verification_outcomes DROP CONSTRAINT IF EXISTS fk_verification_outcomes_step_verification;
ALTER TABLE verification_outcomes ADD CONSTRAINT fk_verification_outcomes_step_verification
  FOREIGN KEY (step_verification) REFERENCES step_verifications (step_verification_id);
ALTER TABLE verification_outcomes DROP CONSTRAINT IF EXISTS fk_verification_outcomes_observed_by_agent;
ALTER TABLE verification_outcomes ADD CONSTRAINT fk_verification_outcomes_observed_by_agent
  FOREIGN KEY (observed_by_agent) REFERENCES agents (agent_id);

-- ObservedTransitions
ALTER TABLE observed_transitions DROP CONSTRAINT IF EXISTS fk_observed_transitions_procedure_execution;
ALTER TABLE observed_transitions ADD CONSTRAINT fk_observed_transitions_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE observed_transitions DROP CONSTRAINT IF EXISTS fk_observed_transitions_step_transition;
ALTER TABLE observed_transitions ADD CONSTRAINT fk_observed_transitions_step_transition
  FOREIGN KEY (step_transition) REFERENCES step_transitions (step_transition_id);
ALTER TABLE observed_transitions DROP CONSTRAINT IF EXISTS fk_observed_transitions_arriving_step_execution;
ALTER TABLE observed_transitions ADD CONSTRAINT fk_observed_transitions_arriving_step_execution
  FOREIGN KEY (arriving_step_execution) REFERENCES step_executions (step_execution_id);

-- Recipients
ALTER TABLE recipients DROP CONSTRAINT IF EXISTS fk_recipients_organization;
ALTER TABLE recipients ADD CONSTRAINT fk_recipients_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE recipients DROP CONSTRAINT IF EXISTS fk_recipients_consent_binding;
ALTER TABLE recipients ADD CONSTRAINT fk_recipients_consent_binding
  FOREIGN KEY (consent_binding) REFERENCES operational_bindings (operational_binding_id);

-- MessageDeliveries
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_procedure_execution;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_step_execution;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_recipient;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_recipient
  FOREIGN KEY (recipient) REFERENCES recipients (recipient_id);
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_message_template;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_message_template
  FOREIGN KEY (message_template) REFERENCES message_templates (message_template_id);
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_sent_by_agent;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_sent_by_agent
  FOREIGN KEY (sent_by_agent) REFERENCES agents (agent_id);
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_invoked_exception;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_invoked_exception
  FOREIGN KEY (invoked_exception) REFERENCES exceptions (exception_id);
ALTER TABLE message_deliveries DROP CONSTRAINT IF EXISTS fk_message_deliveries_evaluation_context;
ALTER TABLE message_deliveries ADD CONSTRAINT fk_message_deliveries_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- TemplateApprovals
ALTER TABLE template_approvals DROP CONSTRAINT IF EXISTS fk_template_approvals_message_template;
ALTER TABLE template_approvals ADD CONSTRAINT fk_template_approvals_message_template
  FOREIGN KEY (message_template) REFERENCES message_templates (message_template_id);
ALTER TABLE template_approvals DROP CONSTRAINT IF EXISTS fk_template_approvals_decided_by_agent;
ALTER TABLE template_approvals ADD CONSTRAINT fk_template_approvals_decided_by_agent
  FOREIGN KEY (decided_by_agent) REFERENCES agents (agent_id);
ALTER TABLE template_approvals DROP CONSTRAINT IF EXISTS fk_template_approvals_decided_in_role;
ALTER TABLE template_approvals ADD CONSTRAINT fk_template_approvals_decided_in_role
  FOREIGN KEY (decided_in_role) REFERENCES roles (role_id);
ALTER TABLE template_approvals DROP CONSTRAINT IF EXISTS fk_template_approvals_decision;
ALTER TABLE template_approvals ADD CONSTRAINT fk_template_approvals_decision
  FOREIGN KEY (decision) REFERENCES lifecycle_statuses (lifecycle_status_id);

-- SendIntents
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_procedure_execution;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_step_execution;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_recipient;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_recipient
  FOREIGN KEY (recipient) REFERENCES recipients (recipient_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_message_template;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_message_template
  FOREIGN KEY (message_template) REFERENCES message_templates (message_template_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_resulting_delivery;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_resulting_delivery
  FOREIGN KEY (resulting_delivery) REFERENCES message_deliveries (message_delivery_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_alternate_channel_intent;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_alternate_channel_intent
  FOREIGN KEY (alternate_channel_intent) REFERENCES send_intents (send_intent_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_refusal_notified_role;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_refusal_notified_role
  FOREIGN KEY (refusal_notified_role) REFERENCES roles (role_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_retry_intent;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_retry_intent
  FOREIGN KEY (retry_intent) REFERENCES send_intents (send_intent_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_evaluation_context;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);
ALTER TABLE send_intents DROP CONSTRAINT IF EXISTS fk_send_intents_evaluating_role_assignment;
ALTER TABLE send_intents ADD CONSTRAINT fk_send_intents_evaluating_role_assignment
  FOREIGN KEY (evaluating_role_assignment) REFERENCES role_assignments (role_assignment_id);

-- AgentDecisionRecords
ALTER TABLE agent_decision_records DROP CONSTRAINT IF EXISTS fk_agent_decision_records_step_execution;
ALTER TABLE agent_decision_records ADD CONSTRAINT fk_agent_decision_records_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE agent_decision_records DROP CONSTRAINT IF EXISTS fk_agent_decision_records_deciding_agent;
ALTER TABLE agent_decision_records ADD CONSTRAINT fk_agent_decision_records_deciding_agent
  FOREIGN KEY (deciding_agent) REFERENCES agents (agent_id);
ALTER TABLE agent_decision_records DROP CONSTRAINT IF EXISTS fk_agent_decision_records_reviewed_by_agent;
ALTER TABLE agent_decision_records ADD CONSTRAINT fk_agent_decision_records_reviewed_by_agent
  FOREIGN KEY (reviewed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE agent_decision_records DROP CONSTRAINT IF EXISTS fk_agent_decision_records_under_role_assignment;
ALTER TABLE agent_decision_records ADD CONSTRAINT fk_agent_decision_records_under_role_assignment
  FOREIGN KEY (under_role_assignment) REFERENCES role_assignments (role_assignment_id);

-- DeliveredCommunications
ALTER TABLE delivered_communications DROP CONSTRAINT IF EXISTS fk_delivered_communications_procedure_execution;
ALTER TABLE delivered_communications ADD CONSTRAINT fk_delivered_communications_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE delivered_communications DROP CONSTRAINT IF EXISTS fk_delivered_communications_sending_step_execution;
ALTER TABLE delivered_communications ADD CONSTRAINT fk_delivered_communications_sending_step_execution
  FOREIGN KEY (sending_step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE delivered_communications DROP CONSTRAINT IF EXISTS fk_delivered_communications_authorizing_step_execution;
ALTER TABLE delivered_communications ADD CONSTRAINT fk_delivered_communications_authorizing_step_execution
  FOREIGN KEY (authorizing_step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE delivered_communications DROP CONSTRAINT IF EXISTS fk_delivered_communications_message_template;
ALTER TABLE delivered_communications ADD CONSTRAINT fk_delivered_communications_message_template
  FOREIGN KEY (message_template) REFERENCES message_templates (message_template_id);

-- AuthorityBoundaries
ALTER TABLE authority_boundaries DROP CONSTRAINT IF EXISTS fk_authority_boundaries_step;
ALTER TABLE authority_boundaries ADD CONSTRAINT fk_authority_boundaries_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE authority_boundaries DROP CONSTRAINT IF EXISTS fk_authority_boundaries_ratified_by_knowledge_fragment;
ALTER TABLE authority_boundaries ADD CONSTRAINT fk_authority_boundaries_ratified_by_knowledge_fragment
  FOREIGN KEY (ratified_by_knowledge_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);
ALTER TABLE authority_boundaries DROP CONSTRAINT IF EXISTS fk_authority_boundaries_enforcing_requirement;
ALTER TABLE authority_boundaries ADD CONSTRAINT fk_authority_boundaries_enforcing_requirement
  FOREIGN KEY (enforcing_requirement) REFERENCES requirements (requirement_id);
ALTER TABLE authority_boundaries DROP CONSTRAINT IF EXISTS fk_authority_boundaries_authority_role;
ALTER TABLE authority_boundaries ADD CONSTRAINT fk_authority_boundaries_authority_role
  FOREIGN KEY (authority_role) REFERENCES roles (role_id);
ALTER TABLE authority_boundaries DROP CONSTRAINT IF EXISTS fk_authority_boundaries_status;
ALTER TABLE authority_boundaries ADD CONSTRAINT fk_authority_boundaries_status
  FOREIGN KEY (status) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE authority_boundaries DROP CONSTRAINT IF EXISTS fk_authority_boundaries_evaluation_context;
ALTER TABLE authority_boundaries ADD CONSTRAINT fk_authority_boundaries_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- BindingObservations
ALTER TABLE binding_observations DROP CONSTRAINT IF EXISTS fk_binding_observations_step_execution;
ALTER TABLE binding_observations ADD CONSTRAINT fk_binding_observations_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE binding_observations DROP CONSTRAINT IF EXISTS fk_binding_observations_operational_binding;
ALTER TABLE binding_observations ADD CONSTRAINT fk_binding_observations_operational_binding
  FOREIGN KEY (operational_binding) REFERENCES operational_bindings (operational_binding_id);

-- Attestations
ALTER TABLE attestations DROP CONSTRAINT IF EXISTS fk_attestations_procedure_execution;
ALTER TABLE attestations ADD CONSTRAINT fk_attestations_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE attestations DROP CONSTRAINT IF EXISTS fk_attestations_signed_by_agent;
ALTER TABLE attestations ADD CONSTRAINT fk_attestations_signed_by_agent
  FOREIGN KEY (signed_by_agent) REFERENCES agents (agent_id);

-- AppRoleProfiles
ALTER TABLE app_role_profiles DROP CONSTRAINT IF EXISTS fk_app_role_profiles_role;
ALTER TABLE app_role_profiles ADD CONSTRAINT fk_app_role_profiles_role
  FOREIGN KEY (role) REFERENCES roles (role_id);

-- AppRoutes
ALTER TABLE app_routes DROP CONSTRAINT IF EXISTS fk_app_routes_owning_role;
ALTER TABLE app_routes ADD CONSTRAINT fk_app_routes_owning_role
  FOREIGN KEY (owning_role) REFERENCES roles (role_id);
ALTER TABLE app_routes DROP CONSTRAINT IF EXISTS fk_app_routes_nav_group;
ALTER TABLE app_routes ADD CONSTRAINT fk_app_routes_nav_group
  FOREIGN KEY (nav_group) REFERENCES app_nav_groups (app_nav_group_id);

-- AppRouteQuestions
ALTER TABLE app_route_questions DROP CONSTRAINT IF EXISTS fk_app_route_questions_route;
ALTER TABLE app_route_questions ADD CONSTRAINT fk_app_route_questions_route
  FOREIGN KEY (route) REFERENCES app_routes (app_route_id);
ALTER TABLE app_route_questions DROP CONSTRAINT IF EXISTS fk_app_route_questions_question;
ALTER TABLE app_route_questions ADD CONSTRAINT fk_app_route_questions_question
  FOREIGN KEY (question) REFERENCES role_questions (role_question_id);

-- AppRouteReferences
ALTER TABLE app_route_references DROP CONSTRAINT IF EXISTS fk_app_route_references_from_route;
ALTER TABLE app_route_references ADD CONSTRAINT fk_app_route_references_from_route
  FOREIGN KEY (from_route) REFERENCES app_routes (app_route_id);
ALTER TABLE app_route_references DROP CONSTRAINT IF EXISTS fk_app_route_references_to_route;
ALTER TABLE app_route_references ADD CONSTRAINT fk_app_route_references_to_route
  FOREIGN KEY (to_route) REFERENCES app_routes (app_route_id);

-- RulebookTables
ALTER TABLE rulebook_tables DROP CONSTRAINT IF EXISTS fk_rulebook_tables_table_name;
ALTER TABLE rulebook_tables ADD CONSTRAINT fk_rulebook_tables_table_name
  FOREIGN KEY (table_name) REFERENCES rulebook_tables (rulebook_table_id);

-- AccessPrincipals
ALTER TABLE access_principals DROP CONSTRAINT IF EXISTS fk_access_principals_domain_role;
ALTER TABLE access_principals ADD CONSTRAINT fk_access_principals_domain_role
  FOREIGN KEY (domain_role) REFERENCES roles (role_id);

-- AccessPolicies
ALTER TABLE access_policies DROP CONSTRAINT IF EXISTS fk_access_policies_principal;
ALTER TABLE access_policies ADD CONSTRAINT fk_access_policies_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);
ALTER TABLE access_policies DROP CONSTRAINT IF EXISTS fk_access_policies_target_table;
ALTER TABLE access_policies ADD CONSTRAINT fk_access_policies_target_table
  FOREIGN KEY (target_table) REFERENCES rulebook_tables (rulebook_table_id);

-- FieldGrants
ALTER TABLE field_grants DROP CONSTRAINT IF EXISTS fk_field_grants_principal;
ALTER TABLE field_grants ADD CONSTRAINT fk_field_grants_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);
ALTER TABLE field_grants DROP CONSTRAINT IF EXISTS fk_field_grants_target_field;
ALTER TABLE field_grants ADD CONSTRAINT fk_field_grants_target_field
  FOREIGN KEY (target_field) REFERENCES rulebook_fields (rulebook_field_id);

-- RoleSchemas
ALTER TABLE role_schemas DROP CONSTRAINT IF EXISTS fk_role_schemas_principal;
ALTER TABLE role_schemas ADD CONSTRAINT fk_role_schemas_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);

-- RoleSchemaViews
ALTER TABLE role_schema_views DROP CONSTRAINT IF EXISTS fk_role_schema_views_role_schema;
ALTER TABLE role_schema_views ADD CONSTRAINT fk_role_schema_views_role_schema
  FOREIGN KEY (role_schema) REFERENCES role_schemas (role_schema_id);
ALTER TABLE role_schema_views DROP CONSTRAINT IF EXISTS fk_role_schema_views_principal;
ALTER TABLE role_schema_views ADD CONSTRAINT fk_role_schema_views_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);
ALTER TABLE role_schema_views DROP CONSTRAINT IF EXISTS fk_role_schema_views_target_table;
ALTER TABLE role_schema_views ADD CONSTRAINT fk_role_schema_views_target_table
  FOREIGN KEY (target_table) REFERENCES rulebook_tables (rulebook_table_id);

-- AccessDenialTests
ALTER TABLE access_denial_tests DROP CONSTRAINT IF EXISTS fk_access_denial_tests_target_policy;
ALTER TABLE access_denial_tests ADD CONSTRAINT fk_access_denial_tests_target_policy
  FOREIGN KEY (target_policy) REFERENCES access_policies (access_policy_id);
ALTER TABLE access_denial_tests DROP CONSTRAINT IF EXISTS fk_access_denial_tests_principal;
ALTER TABLE access_denial_tests ADD CONSTRAINT fk_access_denial_tests_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);
ALTER TABLE access_denial_tests DROP CONSTRAINT IF EXISTS fk_access_denial_tests_target_table;
ALTER TABLE access_denial_tests ADD CONSTRAINT fk_access_denial_tests_target_table
  FOREIGN KEY (target_table) REFERENCES rulebook_tables (rulebook_table_id);

-- AppUsers
ALTER TABLE app_users DROP CONSTRAINT IF EXISTS fk_app_users_linked_agent;
ALTER TABLE app_users ADD CONSTRAINT fk_app_users_linked_agent
  FOREIGN KEY (linked_agent) REFERENCES agents (agent_id);

-- PrincipalAssignments
ALTER TABLE principal_assignments DROP CONSTRAINT IF EXISTS fk_principal_assignments_app_user;
ALTER TABLE principal_assignments ADD CONSTRAINT fk_principal_assignments_app_user
  FOREIGN KEY (app_user) REFERENCES app_users (app_user_id);
ALTER TABLE principal_assignments DROP CONSTRAINT IF EXISTS fk_principal_assignments_principal;
ALTER TABLE principal_assignments ADD CONSTRAINT fk_principal_assignments_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);

-- IssuedTokens
ALTER TABLE issued_tokens DROP CONSTRAINT IF EXISTS fk_issued_tokens_app_user;
ALTER TABLE issued_tokens ADD CONSTRAINT fk_issued_tokens_app_user
  FOREIGN KEY (app_user) REFERENCES app_users (app_user_id);
ALTER TABLE issued_tokens DROP CONSTRAINT IF EXISTS fk_issued_tokens_principal;
ALTER TABLE issued_tokens ADD CONSTRAINT fk_issued_tokens_principal
  FOREIGN KEY (principal) REFERENCES access_principals (access_principal_id);

-- ProcessMiningRuns
ALTER TABLE process_mining_runs DROP CONSTRAINT IF EXISTS fk_process_mining_runs_procedure_version;
ALTER TABLE process_mining_runs ADD CONSTRAINT fk_process_mining_runs_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE process_mining_runs DROP CONSTRAINT IF EXISTS fk_process_mining_runs_evaluation_context;
ALTER TABLE process_mining_runs ADD CONSTRAINT fk_process_mining_runs_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- Vocabularies
ALTER TABLE vocabularies DROP CONSTRAINT IF EXISTS fk_vocabularies_governing_role;
ALTER TABLE vocabularies ADD CONSTRAINT fk_vocabularies_governing_role
  FOREIGN KEY (governing_role) REFERENCES roles (role_id);
ALTER TABLE vocabularies DROP CONSTRAINT IF EXISTS fk_vocabularies_governs_procedure;
ALTER TABLE vocabularies ADD CONSTRAINT fk_vocabularies_governs_procedure
  FOREIGN KEY (governs_procedure) REFERENCES procedures (procedure_id);

-- VocabularyTerms
ALTER TABLE vocabulary_terms DROP CONSTRAINT IF EXISTS fk_vocabulary_terms_vocabulary;
ALTER TABLE vocabulary_terms ADD CONSTRAINT fk_vocabulary_terms_vocabulary
  FOREIGN KEY (vocabulary) REFERENCES vocabularies (vocabulary_id);
ALTER TABLE vocabulary_terms DROP CONSTRAINT IF EXISTS fk_vocabulary_terms_broader_term;
ALTER TABLE vocabulary_terms ADD CONSTRAINT fk_vocabulary_terms_broader_term
  FOREIGN KEY (broader_term) REFERENCES vocabulary_terms (vocabulary_term_id);
ALTER TABLE vocabulary_terms DROP CONSTRAINT IF EXISTS fk_vocabulary_terms_introduced_in_release;
ALTER TABLE vocabulary_terms ADD CONSTRAINT fk_vocabulary_terms_introduced_in_release
  FOREIGN KEY (introduced_in_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE vocabulary_terms DROP CONSTRAINT IF EXISTS fk_vocabulary_terms_represents_role;
ALTER TABLE vocabulary_terms ADD CONSTRAINT fk_vocabulary_terms_represents_role
  FOREIGN KEY (represents_role) REFERENCES roles (role_id);

-- KnowledgeBrokerLinks
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_seeker;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_seeker
  FOREIGN KEY (seeker) REFERENCES agents (agent_id);
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_broker;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_broker
  FOREIGN KEY (broker) REFERENCES agents (agent_id);
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_topic;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_topic
  FOREIGN KEY (topic) REFERENCES vocabulary_terms (vocabulary_term_id);
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_evaluation_context;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_points_to_know_how;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_points_to_know_how
  FOREIGN KEY (points_to_know_how) REFERENCES know_how_carriers (know_how_carrier_id);
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_seeker_vocabulary;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_seeker_vocabulary
  FOREIGN KEY (seeker_vocabulary) REFERENCES vocabularies (vocabulary_id);
ALTER TABLE knowledge_broker_links DROP CONSTRAINT IF EXISTS fk_knowledge_broker_links_holder_vocabulary;
ALTER TABLE knowledge_broker_links ADD CONSTRAINT fk_knowledge_broker_links_holder_vocabulary
  FOREIGN KEY (holder_vocabulary) REFERENCES vocabularies (vocabulary_id);

-- ConformanceRuns
ALTER TABLE conformance_runs DROP CONSTRAINT IF EXISTS fk_conformance_runs_answer_key_author;
ALTER TABLE conformance_runs ADD CONSTRAINT fk_conformance_runs_answer_key_author
  FOREIGN KEY (answer_key_author) REFERENCES conformance_substrates (conformance_substrate_id);

-- SubstrateRunScores
ALTER TABLE substrate_run_scores DROP CONSTRAINT IF EXISTS fk_substrate_run_scores_run;
ALTER TABLE substrate_run_scores ADD CONSTRAINT fk_substrate_run_scores_run
  FOREIGN KEY (run) REFERENCES conformance_runs (conformance_run_id);
ALTER TABLE substrate_run_scores DROP CONSTRAINT IF EXISTS fk_substrate_run_scores_substrate;
ALTER TABLE substrate_run_scores ADD CONSTRAINT fk_substrate_run_scores_substrate
  FOREIGN KEY (substrate) REFERENCES conformance_substrates (conformance_substrate_id);

-- TableConformance
ALTER TABLE table_conformance DROP CONSTRAINT IF EXISTS fk_table_conformance_run;
ALTER TABLE table_conformance ADD CONSTRAINT fk_table_conformance_run
  FOREIGN KEY (run) REFERENCES conformance_runs (conformance_run_id);
ALTER TABLE table_conformance DROP CONSTRAINT IF EXISTS fk_table_conformance_substrate;
ALTER TABLE table_conformance ADD CONSTRAINT fk_table_conformance_substrate
  FOREIGN KEY (substrate) REFERENCES conformance_substrates (conformance_substrate_id);
ALTER TABLE table_conformance DROP CONSTRAINT IF EXISTS fk_table_conformance_rulebook_table;
ALTER TABLE table_conformance ADD CONSTRAINT fk_table_conformance_rulebook_table
  FOREIGN KEY (rulebook_table) REFERENCES rulebook_tables (rulebook_table_id);

-- FieldDisagreements
ALTER TABLE field_disagreements DROP CONSTRAINT IF EXISTS fk_field_disagreements_substrate;
ALTER TABLE field_disagreements ADD CONSTRAINT fk_field_disagreements_substrate
  FOREIGN KEY (substrate) REFERENCES conformance_substrates (conformance_substrate_id);
ALTER TABLE field_disagreements DROP CONSTRAINT IF EXISTS fk_field_disagreements_rulebook_field;
ALTER TABLE field_disagreements ADD CONSTRAINT fk_field_disagreements_rulebook_field
  FOREIGN KEY (rulebook_field) REFERENCES rulebook_fields (rulebook_field_id);
ALTER TABLE field_disagreements DROP CONSTRAINT IF EXISTS fk_field_disagreements_table_conformance;
ALTER TABLE field_disagreements ADD CONSTRAINT fk_field_disagreements_table_conformance
  FOREIGN KEY (table_conformance) REFERENCES table_conformance (table_conformance_id);

-- CellDisagreements
ALTER TABLE cell_disagreements DROP CONSTRAINT IF EXISTS fk_cell_disagreements_field_disagreement;
ALTER TABLE cell_disagreements ADD CONSTRAINT fk_cell_disagreements_field_disagreement
  FOREIGN KEY (field_disagreement) REFERENCES field_disagreements (field_disagreement_id);

-- ArticleClaims
ALTER TABLE article_claims DROP CONSTRAINT IF EXISTS fk_article_claims_source_article;
ALTER TABLE article_claims ADD CONSTRAINT fk_article_claims_source_article
  FOREIGN KEY (source_article) REFERENCES source_articles (source_article_id);

-- ClaimEvidence
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_article_claim;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_article_claim
  FOREIGN KEY (article_claim) REFERENCES article_claims (article_claim_id);
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_rulebook_field;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_rulebook_field
  FOREIGN KEY (rulebook_field) REFERENCES rulebook_fields (rulebook_field_id);
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_rulebook_table;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_rulebook_table
  FOREIGN KEY (rulebook_table) REFERENCES rulebook_tables (rulebook_table_id);
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_role_question;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_role_question
  FOREIGN KEY (role_question) REFERENCES role_questions (role_question_id);
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_ontology_profile;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_ontology_profile
  FOREIGN KEY (ontology_profile) REFERENCES ontology_profiles (ontology_profile_id);
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_knowledge_method;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_knowledge_method
  FOREIGN KEY (knowledge_method) REFERENCES knowledge_methods (knowledge_method_id);
ALTER TABLE claim_evidence DROP CONSTRAINT IF EXISTS fk_claim_evidence_procedure;
ALTER TABLE claim_evidence ADD CONSTRAINT fk_claim_evidence_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);

-- MethodApplications
ALTER TABLE method_applications DROP CONSTRAINT IF EXISTS fk_method_applications_knowledge_method;
ALTER TABLE method_applications ADD CONSTRAINT fk_method_applications_knowledge_method
  FOREIGN KEY (knowledge_method) REFERENCES knowledge_methods (knowledge_method_id);
ALTER TABLE method_applications DROP CONSTRAINT IF EXISTS fk_method_applications_applied_by_agent;
ALTER TABLE method_applications ADD CONSTRAINT fk_method_applications_applied_by_agent
  FOREIGN KEY (applied_by_agent) REFERENCES agents (agent_id);
ALTER TABLE method_applications DROP CONSTRAINT IF EXISTS fk_method_applications_applied_to_grounding_snapshot;
ALTER TABLE method_applications ADD CONSTRAINT fk_method_applications_applied_to_grounding_snapshot
  FOREIGN KEY (applied_to_grounding_snapshot) REFERENCES grounding_snapshots (grounding_snapshot_id);
ALTER TABLE method_applications DROP CONSTRAINT IF EXISTS fk_method_applications_identified_broker;
ALTER TABLE method_applications ADD CONSTRAINT fk_method_applications_identified_broker
  FOREIGN KEY (identified_broker) REFERENCES agents (agent_id);

-- LifecycleStatuses
ALTER TABLE lifecycle_statuses DROP CONSTRAINT IF EXISTS fk_lifecycle_statuses_label;
ALTER TABLE lifecycle_statuses ADD CONSTRAINT fk_lifecycle_statuses_label
  FOREIGN KEY (label) REFERENCES lifecycle_statuses (lifecycle_status_id);
ALTER TABLE lifecycle_statuses DROP CONSTRAINT IF EXISTS fk_lifecycle_statuses_workflow_status_concept;
ALTER TABLE lifecycle_statuses ADD CONSTRAINT fk_lifecycle_statuses_workflow_status_concept
  FOREIGN KEY (workflow_status_concept) REFERENCES vocabulary_terms (vocabulary_term_id);

-- Facilities
ALTER TABLE facilities DROP CONSTRAINT IF EXISTS fk_facilities_organization;
ALTER TABLE facilities ADD CONSTRAINT fk_facilities_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE facilities DROP CONSTRAINT IF EXISTS fk_facilities_parent_facility;
ALTER TABLE facilities ADD CONSTRAINT fk_facilities_parent_facility
  FOREIGN KEY (parent_facility) REFERENCES facilities (facility_id);

-- Machines
ALTER TABLE machines DROP CONSTRAINT IF EXISTS fk_machines_machine_type;
ALTER TABLE machines ADD CONSTRAINT fk_machines_machine_type
  FOREIGN KEY (machine_type) REFERENCES machine_types (machine_type_id);
ALTER TABLE machines DROP CONSTRAINT IF EXISTS fk_machines_facility;
ALTER TABLE machines ADD CONSTRAINT fk_machines_facility
  FOREIGN KEY (facility) REFERENCES facilities (facility_id);
ALTER TABLE machines DROP CONSTRAINT IF EXISTS fk_machines_manufactured_by;
ALTER TABLE machines ADD CONSTRAINT fk_machines_manufactured_by
  FOREIGN KEY (manufactured_by) REFERENCES organizations (organization_id);
ALTER TABLE machines DROP CONSTRAINT IF EXISTS fk_machines_governing_procedure_version;
ALTER TABLE machines ADD CONSTRAINT fk_machines_governing_procedure_version
  FOREIGN KEY (governing_procedure_version) REFERENCES procedure_versions (procedure_version_id);

-- MachineEnergySources
ALTER TABLE machine_energy_sources DROP CONSTRAINT IF EXISTS fk_machine_energy_sources_machine;
ALTER TABLE machine_energy_sources ADD CONSTRAINT fk_machine_energy_sources_machine
  FOREIGN KEY (machine) REFERENCES machines (machine_id);
ALTER TABLE machine_energy_sources DROP CONSTRAINT IF EXISTS fk_machine_energy_sources_energy_source;
ALTER TABLE machine_energy_sources ADD CONSTRAINT fk_machine_energy_sources_energy_source
  FOREIGN KEY (energy_source) REFERENCES energy_sources (energy_source_id);

-- StepLockRequirements
ALTER TABLE step_lock_requirements DROP CONSTRAINT IF EXISTS fk_step_lock_requirements_step;
ALTER TABLE step_lock_requirements ADD CONSTRAINT fk_step_lock_requirements_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_lock_requirements DROP CONSTRAINT IF EXISTS fk_step_lock_requirements_lock_device;
ALTER TABLE step_lock_requirements ADD CONSTRAINT fk_step_lock_requirements_lock_device
  FOREIGN KEY (lock_device) REFERENCES lock_devices (lock_device_id);

-- StepProtectiveEquipment
ALTER TABLE step_protective_equipment DROP CONSTRAINT IF EXISTS fk_step_protective_equipment_step;
ALTER TABLE step_protective_equipment ADD CONSTRAINT fk_step_protective_equipment_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_protective_equipment DROP CONSTRAINT IF EXISTS fk_step_protective_equipment_protective_equipment;
ALTER TABLE step_protective_equipment ADD CONSTRAINT fk_step_protective_equipment_protective_equipment
  FOREIGN KEY (protective_equipment) REFERENCES protective_equipment (protective_equipment_id);

-- ProcedureTargets
ALTER TABLE procedure_targets DROP CONSTRAINT IF EXISTS fk_procedure_targets_procedure;
ALTER TABLE procedure_targets ADD CONSTRAINT fk_procedure_targets_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE procedure_targets DROP CONSTRAINT IF EXISTS fk_procedure_targets_machine;
ALTER TABLE procedure_targets ADD CONSTRAINT fk_procedure_targets_machine
  FOREIGN KEY (machine) REFERENCES machines (machine_id);

-- ProcedureAdoptions
ALTER TABLE procedure_adoptions DROP CONSTRAINT IF EXISTS fk_procedure_adoptions_procedure;
ALTER TABLE procedure_adoptions ADD CONSTRAINT fk_procedure_adoptions_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE procedure_adoptions DROP CONSTRAINT IF EXISTS fk_procedure_adoptions_organization;
ALTER TABLE procedure_adoptions ADD CONSTRAINT fk_procedure_adoptions_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);

-- ProcedureOutcomeCriteria
ALTER TABLE procedure_outcome_criteria DROP CONSTRAINT IF EXISTS fk_procedure_outcome_criteria_procedure;
ALTER TABLE procedure_outcome_criteria ADD CONSTRAINT fk_procedure_outcome_criteria_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);

-- ActivityRelations
ALTER TABLE activity_relations DROP CONSTRAINT IF EXISTS fk_activity_relations_from_step;
ALTER TABLE activity_relations ADD CONSTRAINT fk_activity_relations_from_step
  FOREIGN KEY (from_step) REFERENCES steps (step_id);
ALTER TABLE activity_relations DROP CONSTRAINT IF EXISTS fk_activity_relations_relation_type;
ALTER TABLE activity_relations ADD CONSTRAINT fk_activity_relations_relation_type
  FOREIGN KEY (relation_type) REFERENCES relation_types (relation_type_id);
ALTER TABLE activity_relations DROP CONSTRAINT IF EXISTS fk_activity_relations_to_step;
ALTER TABLE activity_relations ADD CONSTRAINT fk_activity_relations_to_step
  FOREIGN KEY (to_step) REFERENCES steps (step_id);

-- StepVariables
ALTER TABLE step_variables DROP CONSTRAINT IF EXISTS fk_step_variables_step;
ALTER TABLE step_variables ADD CONSTRAINT fk_step_variables_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_variables DROP CONSTRAINT IF EXISTS fk_step_variables_source_variable;
ALTER TABLE step_variables ADD CONSTRAINT fk_step_variables_source_variable
  FOREIGN KEY (source_variable) REFERENCES step_variables (step_variable_id);

-- ExecutionEntities
ALTER TABLE execution_entities DROP CONSTRAINT IF EXISTS fk_execution_entities_step_execution;
ALTER TABLE execution_entities ADD CONSTRAINT fk_execution_entities_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE execution_entities DROP CONSTRAINT IF EXISTS fk_execution_entities_step_variable;
ALTER TABLE execution_entities ADD CONSTRAINT fk_execution_entities_step_variable
  FOREIGN KEY (step_variable) REFERENCES step_variables (step_variable_id);

-- StepConditions
ALTER TABLE step_conditions DROP CONSTRAINT IF EXISTS fk_step_conditions_step;
ALTER TABLE step_conditions ADD CONSTRAINT fk_step_conditions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);

-- ConditionChecks
ALTER TABLE condition_checks DROP CONSTRAINT IF EXISTS fk_condition_checks_step_execution;
ALTER TABLE condition_checks ADD CONSTRAINT fk_condition_checks_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE condition_checks DROP CONSTRAINT IF EXISTS fk_condition_checks_step_condition;
ALTER TABLE condition_checks ADD CONSTRAINT fk_condition_checks_step_condition
  FOREIGN KEY (step_condition) REFERENCES step_conditions (step_condition_id);
ALTER TABLE condition_checks DROP CONSTRAINT IF EXISTS fk_condition_checks_checked_by_agent;
ALTER TABLE condition_checks ADD CONSTRAINT fk_condition_checks_checked_by_agent
  FOREIGN KEY (checked_by_agent) REFERENCES agents (agent_id);

-- FailureModes
ALTER TABLE failure_modes DROP CONSTRAINT IF EXISTS fk_failure_modes_step;
ALTER TABLE failure_modes ADD CONSTRAINT fk_failure_modes_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE failure_modes DROP CONSTRAINT IF EXISTS fk_failure_modes_procedure_target;
ALTER TABLE failure_modes ADD CONSTRAINT fk_failure_modes_procedure_target
  FOREIGN KEY (procedure_target) REFERENCES procedure_targets (procedure_target_id);
ALTER TABLE failure_modes DROP CONSTRAINT IF EXISTS fk_failure_modes_escalate_to_role;
ALTER TABLE failure_modes ADD CONSTRAINT fk_failure_modes_escalate_to_role
  FOREIGN KEY (escalate_to_role) REFERENCES roles (role_id);

-- StepCues
ALTER TABLE step_cues DROP CONSTRAINT IF EXISTS fk_step_cues_step;
ALTER TABLE step_cues ADD CONSTRAINT fk_step_cues_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_cues DROP CONSTRAINT IF EXISTS fk_step_cues_escalate_to_role;
ALTER TABLE step_cues ADD CONSTRAINT fk_step_cues_escalate_to_role
  FOREIGN KEY (escalate_to_role) REFERENCES roles (role_id);

-- CueObservations
ALTER TABLE cue_observations DROP CONSTRAINT IF EXISTS fk_cue_observations_step_execution;
ALTER TABLE cue_observations ADD CONSTRAINT fk_cue_observations_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE cue_observations DROP CONSTRAINT IF EXISTS fk_cue_observations_step_cue;
ALTER TABLE cue_observations ADD CONSTRAINT fk_cue_observations_step_cue
  FOREIGN KEY (step_cue) REFERENCES step_cues (step_cue_id);
ALTER TABLE cue_observations DROP CONSTRAINT IF EXISTS fk_cue_observations_observed_by_agent;
ALTER TABLE cue_observations ADD CONSTRAINT fk_cue_observations_observed_by_agent
  FOREIGN KEY (observed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE cue_observations DROP CONSTRAINT IF EXISTS fk_cue_observations_escalated_to_agent;
ALTER TABLE cue_observations ADD CONSTRAINT fk_cue_observations_escalated_to_agent
  FOREIGN KEY (escalated_to_agent) REFERENCES agents (agent_id);

-- DecisionPoints
ALTER TABLE decision_points DROP CONSTRAINT IF EXISTS fk_decision_points_step;
ALTER TABLE decision_points ADD CONSTRAINT fk_decision_points_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE decision_points DROP CONSTRAINT IF EXISTS fk_decision_points_governing_transition;
ALTER TABLE decision_points ADD CONSTRAINT fk_decision_points_governing_transition
  FOREIGN KEY (governing_transition) REFERENCES step_transitions (step_transition_id);

-- ExecutionParticipants
ALTER TABLE execution_participants DROP CONSTRAINT IF EXISTS fk_execution_participants_procedure_execution;
ALTER TABLE execution_participants ADD CONSTRAINT fk_execution_participants_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE execution_participants DROP CONSTRAINT IF EXISTS fk_execution_participants_agent;
ALTER TABLE execution_participants ADD CONSTRAINT fk_execution_participants_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);

-- StepResources
ALTER TABLE step_resources DROP CONSTRAINT IF EXISTS fk_step_resources_step;
ALTER TABLE step_resources ADD CONSTRAINT fk_step_resources_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_resources DROP CONSTRAINT IF EXISTS fk_step_resources_resource;
ALTER TABLE step_resources ADD CONSTRAINT fk_step_resources_resource
  FOREIGN KEY (resource) REFERENCES resources (resource_id);

-- FaqCategories
ALTER TABLE faq_categories DROP CONSTRAINT IF EXISTS fk_faq_categories_label;
ALTER TABLE faq_categories ADD CONSTRAINT fk_faq_categories_label
  FOREIGN KEY (label) REFERENCES faq_categories (faq_category_id);

-- FaqTargets
ALTER TABLE faq_targets DROP CONSTRAINT IF EXISTS fk_faq_targets_label;
ALTER TABLE faq_targets ADD CONSTRAINT fk_faq_targets_label
  FOREIGN KEY (label) REFERENCES faq_targets (faq_target_id);

-- AuthoringSubmissions
ALTER TABLE authoring_submissions DROP CONSTRAINT IF EXISTS fk_authoring_submissions_procedure_version;
ALTER TABLE authoring_submissions ADD CONSTRAINT fk_authoring_submissions_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE authoring_submissions DROP CONSTRAINT IF EXISTS fk_authoring_submissions_submitted_by_agent;
ALTER TABLE authoring_submissions ADD CONSTRAINT fk_authoring_submissions_submitted_by_agent
  FOREIGN KEY (submitted_by_agent) REFERENCES agents (agent_id);
ALTER TABLE authoring_submissions DROP CONSTRAINT IF EXISTS fk_authoring_submissions_authoring_tool;
ALTER TABLE authoring_submissions ADD CONSTRAINT fk_authoring_submissions_authoring_tool
  FOREIGN KEY (authoring_tool) REFERENCES tools (tool_id);

-- ProcessKnowledgeLevels
ALTER TABLE process_knowledge_levels DROP CONSTRAINT IF EXISTS fk_process_knowledge_levels_label;
ALTER TABLE process_knowledge_levels ADD CONSTRAINT fk_process_knowledge_levels_label
  FOREIGN KEY (label) REFERENCES process_knowledge_levels (process_knowledge_level_id);

-- LevelCaptureStrategies
ALTER TABLE level_capture_strategies DROP CONSTRAINT IF EXISTS fk_level_capture_strategies_level;
ALTER TABLE level_capture_strategies ADD CONSTRAINT fk_level_capture_strategies_level
  FOREIGN KEY (level) REFERENCES process_knowledge_levels (process_knowledge_level_id);
ALTER TABLE level_capture_strategies DROP CONSTRAINT IF EXISTS fk_level_capture_strategies_knowledge_method;
ALTER TABLE level_capture_strategies ADD CONSTRAINT fk_level_capture_strategies_knowledge_method
  FOREIGN KEY (knowledge_method) REFERENCES knowledge_methods (knowledge_method_id);

-- LevelPyramidQuestions
ALTER TABLE level_pyramid_questions DROP CONSTRAINT IF EXISTS fk_level_pyramid_questions_level;
ALTER TABLE level_pyramid_questions ADD CONSTRAINT fk_level_pyramid_questions_level
  FOREIGN KEY (level) REFERENCES process_knowledge_levels (process_knowledge_level_id);

-- ProcessLevelStatements
ALTER TABLE process_level_statements DROP CONSTRAINT IF EXISTS fk_process_level_statements_procedure;
ALTER TABLE process_level_statements ADD CONSTRAINT fk_process_level_statements_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE process_level_statements DROP CONSTRAINT IF EXISTS fk_process_level_statements_level;
ALTER TABLE process_level_statements ADD CONSTRAINT fk_process_level_statements_level
  FOREIGN KEY (level) REFERENCES process_knowledge_levels (process_knowledge_level_id);

-- TacticalResourceAllocations
ALTER TABLE tactical_resource_allocations DROP CONSTRAINT IF EXISTS fk_tactical_resource_allocations_step;
ALTER TABLE tactical_resource_allocations ADD CONSTRAINT fk_tactical_resource_allocations_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE tactical_resource_allocations DROP CONSTRAINT IF EXISTS fk_tactical_resource_allocations_facility;
ALTER TABLE tactical_resource_allocations ADD CONSTRAINT fk_tactical_resource_allocations_facility
  FOREIGN KEY (facility) REFERENCES facilities (facility_id);

-- ProcessStrategicAlignments
ALTER TABLE process_strategic_alignments DROP CONSTRAINT IF EXISTS fk_process_strategic_alignments_procedure;
ALTER TABLE process_strategic_alignments ADD CONSTRAINT fk_process_strategic_alignments_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE process_strategic_alignments DROP CONSTRAINT IF EXISTS fk_process_strategic_alignments_organization;
ALTER TABLE process_strategic_alignments ADD CONSTRAINT fk_process_strategic_alignments_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);

-- BusinessOutcomes
ALTER TABLE business_outcomes DROP CONSTRAINT IF EXISTS fk_business_outcomes_owner_role;
ALTER TABLE business_outcomes ADD CONSTRAINT fk_business_outcomes_owner_role
  FOREIGN KEY (owner_role) REFERENCES roles (role_id);

-- ProcessOutcomeMeasures
ALTER TABLE process_outcome_measures DROP CONSTRAINT IF EXISTS fk_process_outcome_measures_procedure;
ALTER TABLE process_outcome_measures ADD CONSTRAINT fk_process_outcome_measures_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE process_outcome_measures DROP CONSTRAINT IF EXISTS fk_process_outcome_measures_business_outcome;
ALTER TABLE process_outcome_measures ADD CONSTRAINT fk_process_outcome_measures_business_outcome
  FOREIGN KEY (business_outcome) REFERENCES business_outcomes (business_outcome_id);

-- ProcessStages
ALTER TABLE process_stages DROP CONSTRAINT IF EXISTS fk_process_stages_procedure_version;
ALTER TABLE process_stages ADD CONSTRAINT fk_process_stages_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE process_stages DROP CONSTRAINT IF EXISTS fk_process_stages_owner_role;
ALTER TABLE process_stages ADD CONSTRAINT fk_process_stages_owner_role
  FOREIGN KEY (owner_role) REFERENCES roles (role_id);

-- ProcessInterdependencies
ALTER TABLE process_interdependencies DROP CONSTRAINT IF EXISTS fk_process_interdependencies_from_procedure;
ALTER TABLE process_interdependencies ADD CONSTRAINT fk_process_interdependencies_from_procedure
  FOREIGN KEY (from_procedure) REFERENCES procedures (procedure_id);
ALTER TABLE process_interdependencies DROP CONSTRAINT IF EXISTS fk_process_interdependencies_to_procedure;
ALTER TABLE process_interdependencies ADD CONSTRAINT fk_process_interdependencies_to_procedure
  FOREIGN KEY (to_procedure) REFERENCES procedures (procedure_id);

-- StakeholderLenses
ALTER TABLE stakeholder_lenses DROP CONSTRAINT IF EXISTS fk_stakeholder_lenses_exemplar_role;
ALTER TABLE stakeholder_lenses ADD CONSTRAINT fk_stakeholder_lenses_exemplar_role
  FOREIGN KEY (exemplar_role) REFERENCES roles (role_id);

-- ProcedureLensViews
ALTER TABLE procedure_lens_views DROP CONSTRAINT IF EXISTS fk_procedure_lens_views_procedure;
ALTER TABLE procedure_lens_views ADD CONSTRAINT fk_procedure_lens_views_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE procedure_lens_views DROP CONSTRAINT IF EXISTS fk_procedure_lens_views_stakeholder_lens;
ALTER TABLE procedure_lens_views ADD CONSTRAINT fk_procedure_lens_views_stakeholder_lens
  FOREIGN KEY (stakeholder_lens) REFERENCES stakeholder_lenses (stakeholder_lens_id);
ALTER TABLE procedure_lens_views DROP CONSTRAINT IF EXISTS fk_procedure_lens_views_projects_version;
ALTER TABLE procedure_lens_views ADD CONSTRAINT fk_procedure_lens_views_projects_version
  FOREIGN KEY (projects_version) REFERENCES procedure_versions (procedure_version_id);

-- ApplicabilityScopes
ALTER TABLE applicability_scopes DROP CONSTRAINT IF EXISTS fk_applicability_scopes_business_unit;
ALTER TABLE applicability_scopes ADD CONSTRAINT fk_applicability_scopes_business_unit
  FOREIGN KEY (business_unit) REFERENCES organizations (organization_id);
ALTER TABLE applicability_scopes DROP CONSTRAINT IF EXISTS fk_applicability_scopes_regulatory_regime;
ALTER TABLE applicability_scopes ADD CONSTRAINT fk_applicability_scopes_regulatory_regime
  FOREIGN KEY (regulatory_regime) REFERENCES regulatory_frameworks (regulatory_framework_id);

-- StepContextSensitivities
ALTER TABLE step_context_sensitivities DROP CONSTRAINT IF EXISTS fk_step_context_sensitivities_step;
ALTER TABLE step_context_sensitivities ADD CONSTRAINT fk_step_context_sensitivities_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE step_context_sensitivities DROP CONSTRAINT IF EXISTS fk_step_context_sensitivities_applicability_scope;
ALTER TABLE step_context_sensitivities ADD CONSTRAINT fk_step_context_sensitivities_applicability_scope
  FOREIGN KEY (applicability_scope) REFERENCES applicability_scopes (applicability_scope_id);

-- SituationalVariants
ALTER TABLE situational_variants DROP CONSTRAINT IF EXISTS fk_situational_variants_procedure;
ALTER TABLE situational_variants ADD CONSTRAINT fk_situational_variants_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE situational_variants DROP CONSTRAINT IF EXISTS fk_situational_variants_applicability_scope;
ALTER TABLE situational_variants ADD CONSTRAINT fk_situational_variants_applicability_scope
  FOREIGN KEY (applicability_scope) REFERENCES applicability_scopes (applicability_scope_id);
ALTER TABLE situational_variants DROP CONSTRAINT IF EXISTS fk_situational_variants_expert_agent;
ALTER TABLE situational_variants ADD CONSTRAINT fk_situational_variants_expert_agent
  FOREIGN KEY (expert_agent) REFERENCES agents (agent_id);

-- CollectedSourceMaterials
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_procedure;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_organized_into_scheme;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_organized_into_scheme
  FOREIGN KEY (organized_into_scheme) REFERENCES vocabularies (vocabulary_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_encoded_into_version;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_encoded_into_version
  FOREIGN KEY (encoded_into_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_source_document;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_source_document
  FOREIGN KEY (source_document) REFERENCES resources (resource_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_complements_mining_run;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_complements_mining_run
  FOREIGN KEY (complements_mining_run) REFERENCES process_mining_runs (process_mining_run_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_captured_during_execution;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_captured_during_execution
  FOREIGN KEY (captured_during_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_collected_at_occasion;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_collected_at_occasion
  FOREIGN KEY (collected_at_occasion) REFERENCES collection_occasions (collection_occasion_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_prompted_by_feedback;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_prompted_by_feedback
  FOREIGN KEY (prompted_by_feedback) REFERENCES user_feedback (user_feedback_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_produced_by_method_application;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_produced_by_method_application
  FOREIGN KEY (produced_by_method_application) REFERENCES method_applications (method_application_id);
ALTER TABLE collected_source_materials DROP CONSTRAINT IF EXISTS fk_collected_source_materials_contributing_expert;
ALTER TABLE collected_source_materials ADD CONSTRAINT fk_collected_source_materials_contributing_expert
  FOREIGN KEY (contributing_expert) REFERENCES agents (agent_id);

-- SchemeRefinements
ALTER TABLE scheme_refinements DROP CONSTRAINT IF EXISTS fk_scheme_refinements_vocabulary;
ALTER TABLE scheme_refinements ADD CONSTRAINT fk_scheme_refinements_vocabulary
  FOREIGN KEY (vocabulary) REFERENCES vocabularies (vocabulary_id);
ALTER TABLE scheme_refinements DROP CONSTRAINT IF EXISTS fk_scheme_refinements_triggered_by_material;
ALTER TABLE scheme_refinements ADD CONSTRAINT fk_scheme_refinements_triggered_by_material
  FOREIGN KEY (triggered_by_material) REFERENCES collected_source_materials (collected_source_material_id);

-- TermLabelVariants
ALTER TABLE term_label_variants DROP CONSTRAINT IF EXISTS fk_term_label_variants_vocabulary_term;
ALTER TABLE term_label_variants ADD CONSTRAINT fk_term_label_variants_vocabulary_term
  FOREIGN KEY (vocabulary_term) REFERENCES vocabulary_terms (vocabulary_term_id);

-- AiLabelingRuns
ALTER TABLE ai_labeling_runs DROP CONSTRAINT IF EXISTS fk_ai_labeling_runs_agent;
ALTER TABLE ai_labeling_runs ADD CONSTRAINT fk_ai_labeling_runs_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE ai_labeling_runs DROP CONSTRAINT IF EXISTS fk_ai_labeling_runs_grounding_scheme;
ALTER TABLE ai_labeling_runs ADD CONSTRAINT fk_ai_labeling_runs_grounding_scheme
  FOREIGN KEY (grounding_scheme) REFERENCES vocabularies (vocabulary_id);

-- SourceTermMentions
ALTER TABLE source_term_mentions DROP CONSTRAINT IF EXISTS fk_source_term_mentions_source_material;
ALTER TABLE source_term_mentions ADD CONSTRAINT fk_source_term_mentions_source_material
  FOREIGN KEY (source_material) REFERENCES collected_source_materials (collected_source_material_id);
ALTER TABLE source_term_mentions DROP CONSTRAINT IF EXISTS fk_source_term_mentions_ai_labeling_run;
ALTER TABLE source_term_mentions ADD CONSTRAINT fk_source_term_mentions_ai_labeling_run
  FOREIGN KEY (ai_labeling_run) REFERENCES ai_labeling_runs (ai_labeling_run_id);
ALTER TABLE source_term_mentions DROP CONSTRAINT IF EXISTS fk_source_term_mentions_concept_scheme;
ALTER TABLE source_term_mentions ADD CONSTRAINT fk_source_term_mentions_concept_scheme
  FOREIGN KEY (concept_scheme) REFERENCES vocabularies (vocabulary_id);
ALTER TABLE source_term_mentions DROP CONSTRAINT IF EXISTS fk_source_term_mentions_intended_term;
ALTER TABLE source_term_mentions ADD CONSTRAINT fk_source_term_mentions_intended_term
  FOREIGN KEY (intended_term) REFERENCES vocabulary_terms (vocabulary_term_id);

-- TermRelations
ALTER TABLE term_relations DROP CONSTRAINT IF EXISTS fk_term_relations_from_term;
ALTER TABLE term_relations ADD CONSTRAINT fk_term_relations_from_term
  FOREIGN KEY (from_term) REFERENCES vocabulary_terms (vocabulary_term_id);
ALTER TABLE term_relations DROP CONSTRAINT IF EXISTS fk_term_relations_to_term;
ALTER TABLE term_relations ADD CONSTRAINT fk_term_relations_to_term
  FOREIGN KEY (to_term) REFERENCES vocabulary_terms (vocabulary_term_id);

-- TermMeaningChanges
ALTER TABLE term_meaning_changes DROP CONSTRAINT IF EXISTS fk_term_meaning_changes_vocabulary_term;
ALTER TABLE term_meaning_changes ADD CONSTRAINT fk_term_meaning_changes_vocabulary_term
  FOREIGN KEY (vocabulary_term) REFERENCES vocabulary_terms (vocabulary_term_id);
ALTER TABLE term_meaning_changes DROP CONSTRAINT IF EXISTS fk_term_meaning_changes_recorded_by_agent;
ALTER TABLE term_meaning_changes ADD CONSTRAINT fk_term_meaning_changes_recorded_by_agent
  FOREIGN KEY (recorded_by_agent) REFERENCES agents (agent_id);

-- ExternalStandardTerms
ALTER TABLE external_standard_terms DROP CONSTRAINT IF EXISTS fk_external_standard_terms_ontology_profile;
ALTER TABLE external_standard_terms ADD CONSTRAINT fk_external_standard_terms_ontology_profile
  FOREIGN KEY (ontology_profile) REFERENCES ontology_profiles (ontology_profile_id);
ALTER TABLE external_standard_terms DROP CONSTRAINT IF EXISTS fk_external_standard_terms_evaluation_context;
ALTER TABLE external_standard_terms ADD CONSTRAINT fk_external_standard_terms_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);
ALTER TABLE external_standard_terms DROP CONSTRAINT IF EXISTS fk_external_standard_terms_rehomed_as_term;
ALTER TABLE external_standard_terms ADD CONSTRAINT fk_external_standard_terms_rehomed_as_term
  FOREIGN KEY (rehomed_as_term) REFERENCES vocabulary_terms (vocabulary_term_id);

-- RoleCapabilityTags
ALTER TABLE role_capability_tags DROP CONSTRAINT IF EXISTS fk_role_capability_tags_role;
ALTER TABLE role_capability_tags ADD CONSTRAINT fk_role_capability_tags_role
  FOREIGN KEY (role) REFERENCES roles (role_id);
ALTER TABLE role_capability_tags DROP CONSTRAINT IF EXISTS fk_role_capability_tags_capability_term;
ALTER TABLE role_capability_tags ADD CONSTRAINT fk_role_capability_tags_capability_term
  FOREIGN KEY (capability_term) REFERENCES vocabulary_terms (vocabulary_term_id);

-- ProcedureFacetAssignments
ALTER TABLE procedure_facet_assignments DROP CONSTRAINT IF EXISTS fk_procedure_facet_assignments_procedure;
ALTER TABLE procedure_facet_assignments ADD CONSTRAINT fk_procedure_facet_assignments_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE procedure_facet_assignments DROP CONSTRAINT IF EXISTS fk_procedure_facet_assignments_facet;
ALTER TABLE procedure_facet_assignments ADD CONSTRAINT fk_procedure_facet_assignments_facet
  FOREIGN KEY (facet) REFERENCES classification_facets (classification_facet_id);

-- EncodingLifecycleStages
ALTER TABLE encoding_lifecycle_stages DROP CONSTRAINT IF EXISTS fk_encoding_lifecycle_stages_label;
ALTER TABLE encoding_lifecycle_stages ADD CONSTRAINT fk_encoding_lifecycle_stages_label
  FOREIGN KEY (label) REFERENCES encoding_lifecycle_stages (encoding_lifecycle_stage_id);

-- KnowledgeConsumerSystems
ALTER TABLE knowledge_consumer_systems DROP CONSTRAINT IF EXISTS fk_knowledge_consumer_systems_organization;
ALTER TABLE knowledge_consumer_systems ADD CONSTRAINT fk_knowledge_consumer_systems_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);

-- ConsumerSystemSyncs
ALTER TABLE consumer_system_syncs DROP CONSTRAINT IF EXISTS fk_consumer_system_syncs_consumer_system;
ALTER TABLE consumer_system_syncs ADD CONSTRAINT fk_consumer_system_syncs_consumer_system
  FOREIGN KEY (consumer_system) REFERENCES knowledge_consumer_systems (knowledge_consumer_system_id);
ALTER TABLE consumer_system_syncs DROP CONSTRAINT IF EXISTS fk_consumer_system_syncs_loaded_version;
ALTER TABLE consumer_system_syncs ADD CONSTRAINT fk_consumer_system_syncs_loaded_version
  FOREIGN KEY (loaded_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE consumer_system_syncs DROP CONSTRAINT IF EXISTS fk_consumer_system_syncs_source_resource;
ALTER TABLE consumer_system_syncs ADD CONSTRAINT fk_consumer_system_syncs_source_resource
  FOREIGN KEY (source_resource) REFERENCES resources (resource_id);

-- AgentIntegrations
ALTER TABLE agent_integrations DROP CONSTRAINT IF EXISTS fk_agent_integrations_agent;
ALTER TABLE agent_integrations ADD CONSTRAINT fk_agent_integrations_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE agent_integrations DROP CONSTRAINT IF EXISTS fk_agent_integrations_knowledge_system;
ALTER TABLE agent_integrations ADD CONSTRAINT fk_agent_integrations_knowledge_system
  FOREIGN KEY (knowledge_system) REFERENCES knowledge_consumer_systems (knowledge_consumer_system_id);
ALTER TABLE agent_integrations DROP CONSTRAINT IF EXISTS fk_agent_integrations_pathway;
ALTER TABLE agent_integrations ADD CONSTRAINT fk_agent_integrations_pathway
  FOREIGN KEY (pathway) REFERENCES integration_pathways (integration_pathway_id);
ALTER TABLE agent_integrations DROP CONSTRAINT IF EXISTS fk_agent_integrations_serves_snapshot;
ALTER TABLE agent_integrations ADD CONSTRAINT fk_agent_integrations_serves_snapshot
  FOREIGN KEY (serves_snapshot) REFERENCES grounding_snapshots (grounding_snapshot_id);

-- GroundingSnapshots
ALTER TABLE grounding_snapshots DROP CONSTRAINT IF EXISTS fk_grounding_snapshots_steward_role;
ALTER TABLE grounding_snapshots ADD CONSTRAINT fk_grounding_snapshots_steward_role
  FOREIGN KEY (steward_role) REFERENCES roles (role_id);

-- ReasonerRuns
ALTER TABLE reasoner_runs DROP CONSTRAINT IF EXISTS fk_reasoner_runs_snapshot;
ALTER TABLE reasoner_runs ADD CONSTRAINT fk_reasoner_runs_snapshot
  FOREIGN KEY (snapshot) REFERENCES grounding_snapshots (grounding_snapshot_id);

-- SnapshotAssertions
ALTER TABLE snapshot_assertions DROP CONSTRAINT IF EXISTS fk_snapshot_assertions_snapshot;
ALTER TABLE snapshot_assertions ADD CONSTRAINT fk_snapshot_assertions_snapshot
  FOREIGN KEY (snapshot) REFERENCES grounding_snapshots (grounding_snapshot_id);
ALTER TABLE snapshot_assertions DROP CONSTRAINT IF EXISTS fk_snapshot_assertions_source_procedure_version;
ALTER TABLE snapshot_assertions ADD CONSTRAINT fk_snapshot_assertions_source_procedure_version
  FOREIGN KEY (source_procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE snapshot_assertions DROP CONSTRAINT IF EXISTS fk_snapshot_assertions_source_step;
ALTER TABLE snapshot_assertions ADD CONSTRAINT fk_snapshot_assertions_source_step
  FOREIGN KEY (source_step) REFERENCES steps (step_id);
ALTER TABLE snapshot_assertions DROP CONSTRAINT IF EXISTS fk_snapshot_assertions_source_role_assignment;
ALTER TABLE snapshot_assertions ADD CONSTRAINT fk_snapshot_assertions_source_role_assignment
  FOREIGN KEY (source_role_assignment) REFERENCES role_assignments (role_assignment_id);
ALTER TABLE snapshot_assertions DROP CONSTRAINT IF EXISTS fk_snapshot_assertions_about_agent;
ALTER TABLE snapshot_assertions ADD CONSTRAINT fk_snapshot_assertions_about_agent
  FOREIGN KEY (about_agent) REFERENCES agents (agent_id);

-- RetrievalSegments
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_procedure_version;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_step;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_decision_point;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_decision_point
  FOREIGN KEY (decision_point) REFERENCES decision_points (decision_point_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_related_segment;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_related_segment
  FOREIGN KEY (related_segment) REFERENCES retrieval_segments (retrieval_segment_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_source_resource;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_source_resource
  FOREIGN KEY (source_resource) REFERENCES resources (resource_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_authored_by_agent;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_authored_by_agent
  FOREIGN KEY (authored_by_agent) REFERENCES agents (agent_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_accountable_role;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_accountable_role
  FOREIGN KEY (accountable_role) REFERENCES roles (role_id);
ALTER TABLE retrieval_segments DROP CONSTRAINT IF EXISTS fk_retrieval_segments_contradicts_segment;
ALTER TABLE retrieval_segments ADD CONSTRAINT fk_retrieval_segments_contradicts_segment
  FOREIGN KEY (contradicts_segment) REFERENCES retrieval_segments (retrieval_segment_id);

-- KnowledgeQueryDefinitions
ALTER TABLE knowledge_query_definitions DROP CONSTRAINT IF EXISTS fk_knowledge_query_definitions_target_procedure_version;
ALTER TABLE knowledge_query_definitions ADD CONSTRAINT fk_knowledge_query_definitions_target_procedure_version
  FOREIGN KEY (target_procedure_version) REFERENCES procedure_versions (procedure_version_id);

-- KnowledgeQuerySources
ALTER TABLE knowledge_query_sources DROP CONSTRAINT IF EXISTS fk_knowledge_query_sources_query_definition;
ALTER TABLE knowledge_query_sources ADD CONSTRAINT fk_knowledge_query_sources_query_definition
  FOREIGN KEY (query_definition) REFERENCES knowledge_query_definitions (knowledge_query_definition_id);
ALTER TABLE knowledge_query_sources DROP CONSTRAINT IF EXISTS fk_knowledge_query_sources_consumer_system;
ALTER TABLE knowledge_query_sources ADD CONSTRAINT fk_knowledge_query_sources_consumer_system
  FOREIGN KEY (consumer_system) REFERENCES knowledge_consumer_systems (knowledge_consumer_system_id);

-- AssistantAnswers
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_answering_agent;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_answering_agent
  FOREIGN KEY (answering_agent) REFERENCES agents (agent_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_asked_by_agent;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_asked_by_agent
  FOREIGN KEY (asked_by_agent) REFERENCES agents (agent_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_via_integration;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_via_integration
  FOREIGN KEY (via_integration) REFERENCES agent_integrations (agent_integration_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_step_execution;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_assumed_current_step;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_assumed_current_step
  FOREIGN KEY (assumed_current_step) REFERENCES steps (step_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_asserted_next_step;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_asserted_next_step
  FOREIGN KEY (asserted_next_step) REFERENCES steps (step_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_recommended_step;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_recommended_step
  FOREIGN KEY (recommended_step) REFERENCES steps (step_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_human_reviewed_by;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_human_reviewed_by
  FOREIGN KEY (human_reviewed_by) REFERENCES agents (agent_id);
ALTER TABLE assistant_answers DROP CONSTRAINT IF EXISTS fk_assistant_answers_reviewed_for_initiative;
ALTER TABLE assistant_answers ADD CONSTRAINT fk_assistant_answers_reviewed_for_initiative
  FOREIGN KEY (reviewed_for_initiative) REFERENCES ai_adoption_initiatives (ai_adoption_initiative_id);

-- AnswerGroundings
ALTER TABLE answer_groundings DROP CONSTRAINT IF EXISTS fk_answer_groundings_assistant_answer;
ALTER TABLE answer_groundings ADD CONSTRAINT fk_answer_groundings_assistant_answer
  FOREIGN KEY (assistant_answer) REFERENCES assistant_answers (assistant_answer_id);
ALTER TABLE answer_groundings DROP CONSTRAINT IF EXISTS fk_answer_groundings_snapshot_assertion;
ALTER TABLE answer_groundings ADD CONSTRAINT fk_answer_groundings_snapshot_assertion
  FOREIGN KEY (snapshot_assertion) REFERENCES snapshot_assertions (snapshot_assertion_id);
ALTER TABLE answer_groundings DROP CONSTRAINT IF EXISTS fk_answer_groundings_retrieval_segment;
ALTER TABLE answer_groundings ADD CONSTRAINT fk_answer_groundings_retrieval_segment
  FOREIGN KEY (retrieval_segment) REFERENCES retrieval_segments (retrieval_segment_id);

-- AnswerRequirementChecks
ALTER TABLE answer_requirement_checks DROP CONSTRAINT IF EXISTS fk_answer_requirement_checks_assistant_answer;
ALTER TABLE answer_requirement_checks ADD CONSTRAINT fk_answer_requirement_checks_assistant_answer
  FOREIGN KEY (assistant_answer) REFERENCES assistant_answers (assistant_answer_id);
ALTER TABLE answer_requirement_checks DROP CONSTRAINT IF EXISTS fk_answer_requirement_checks_requirement;
ALTER TABLE answer_requirement_checks ADD CONSTRAINT fk_answer_requirement_checks_requirement
  FOREIGN KEY (requirement) REFERENCES requirements (requirement_id);
ALTER TABLE answer_requirement_checks DROP CONSTRAINT IF EXISTS fk_answer_requirement_checks_checked_by_agent;
ALTER TABLE answer_requirement_checks ADD CONSTRAINT fk_answer_requirement_checks_checked_by_agent
  FOREIGN KEY (checked_by_agent) REFERENCES agents (agent_id);

-- AiToolInvocations
ALTER TABLE ai_tool_invocations DROP CONSTRAINT IF EXISTS fk_ai_tool_invocations_invoking_agent;
ALTER TABLE ai_tool_invocations ADD CONSTRAINT fk_ai_tool_invocations_invoking_agent
  FOREIGN KEY (invoking_agent) REFERENCES agents (agent_id);
ALTER TABLE ai_tool_invocations DROP CONSTRAINT IF EXISTS fk_ai_tool_invocations_step_execution;
ALTER TABLE ai_tool_invocations ADD CONSTRAINT fk_ai_tool_invocations_step_execution
  FOREIGN KEY (step_execution) REFERENCES step_executions (step_execution_id);
ALTER TABLE ai_tool_invocations DROP CONSTRAINT IF EXISTS fk_ai_tool_invocations_function;
ALTER TABLE ai_tool_invocations ADD CONSTRAINT fk_ai_tool_invocations_function
  FOREIGN KEY (function) REFERENCES functions (function_id);

-- PromptTemplates
ALTER TABLE prompt_templates DROP CONSTRAINT IF EXISTS fk_prompt_templates_parent_template;
ALTER TABLE prompt_templates ADD CONSTRAINT fk_prompt_templates_parent_template
  FOREIGN KEY (parent_template) REFERENCES prompt_templates (prompt_template_id);
ALTER TABLE prompt_templates DROP CONSTRAINT IF EXISTS fk_prompt_templates_source_procedure_version;
ALTER TABLE prompt_templates ADD CONSTRAINT fk_prompt_templates_source_procedure_version
  FOREIGN KEY (source_procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE prompt_templates DROP CONSTRAINT IF EXISTS fk_prompt_templates_maintained_by_role;
ALTER TABLE prompt_templates ADD CONSTRAINT fk_prompt_templates_maintained_by_role
  FOREIGN KEY (maintained_by_role) REFERENCES roles (role_id);
ALTER TABLE prompt_templates DROP CONSTRAINT IF EXISTS fk_prompt_templates_used_by_agent;
ALTER TABLE prompt_templates ADD CONSTRAINT fk_prompt_templates_used_by_agent
  FOREIGN KEY (used_by_agent) REFERENCES agents (agent_id);

-- KnowledgeProjections
ALTER TABLE knowledge_projections DROP CONSTRAINT IF EXISTS fk_knowledge_projections_procedure_version;
ALTER TABLE knowledge_projections ADD CONSTRAINT fk_knowledge_projections_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_projections DROP CONSTRAINT IF EXISTS fk_knowledge_projections_audience_role;
ALTER TABLE knowledge_projections ADD CONSTRAINT fk_knowledge_projections_audience_role
  FOREIGN KEY (audience_role) REFERENCES roles (role_id);

-- ModelAnnotations
ALTER TABLE model_annotations DROP CONSTRAINT IF EXISTS fk_model_annotations_procedure_version;
ALTER TABLE model_annotations ADD CONSTRAINT fk_model_annotations_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE model_annotations DROP CONSTRAINT IF EXISTS fk_model_annotations_step;
ALTER TABLE model_annotations ADD CONSTRAINT fk_model_annotations_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE model_annotations DROP CONSTRAINT IF EXISTS fk_model_annotations_annotated_by_agent;
ALTER TABLE model_annotations ADD CONSTRAINT fk_model_annotations_annotated_by_agent
  FOREIGN KEY (annotated_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_annotations DROP CONSTRAINT IF EXISTS fk_model_annotations_lifecycle_stage;
ALTER TABLE model_annotations ADD CONSTRAINT fk_model_annotations_lifecycle_stage
  FOREIGN KEY (lifecycle_stage) REFERENCES encoding_lifecycle_stages (encoding_lifecycle_stage_id);
ALTER TABLE model_annotations DROP CONSTRAINT IF EXISTS fk_model_annotations_promoted_to_fragment;
ALTER TABLE model_annotations ADD CONSTRAINT fk_model_annotations_promoted_to_fragment
  FOREIGN KEY (promoted_to_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);
ALTER TABLE model_annotations DROP CONSTRAINT IF EXISTS fk_model_annotations_raised_knowledge_gap;
ALTER TABLE model_annotations ADD CONSTRAINT fk_model_annotations_raised_knowledge_gap
  FOREIGN KEY (raised_knowledge_gap) REFERENCES knowledge_gaps (knowledge_gap_id);

-- KnowledgeSearchEvents
ALTER TABLE knowledge_search_events DROP CONSTRAINT IF EXISTS fk_knowledge_search_events_searched_by_agent;
ALTER TABLE knowledge_search_events ADD CONSTRAINT fk_knowledge_search_events_searched_by_agent
  FOREIGN KEY (searched_by_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_search_events DROP CONSTRAINT IF EXISTS fk_knowledge_search_events_sought_procedure_version;
ALTER TABLE knowledge_search_events ADD CONSTRAINT fk_knowledge_search_events_sought_procedure_version
  FOREIGN KEY (sought_procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_search_events DROP CONSTRAINT IF EXISTS fk_knowledge_search_events_opened_segment;
ALTER TABLE knowledge_search_events ADD CONSTRAINT fk_knowledge_search_events_opened_segment
  FOREIGN KEY (opened_segment) REFERENCES retrieval_segments (retrieval_segment_id);
ALTER TABLE knowledge_search_events DROP CONSTRAINT IF EXISTS fk_knowledge_search_events_opened_projection;
ALTER TABLE knowledge_search_events ADD CONSTRAINT fk_knowledge_search_events_opened_projection
  FOREIGN KEY (opened_projection) REFERENCES knowledge_projections (knowledge_projection_id);
ALTER TABLE knowledge_search_events DROP CONSTRAINT IF EXISTS fk_knowledge_search_events_linked_knowledge_gap;
ALTER TABLE knowledge_search_events ADD CONSTRAINT fk_knowledge_search_events_linked_knowledge_gap
  FOREIGN KEY (linked_knowledge_gap) REFERENCES knowledge_gaps (knowledge_gap_id);

-- AiAdoptionInitiatives
ALTER TABLE ai_adoption_initiatives DROP CONSTRAINT IF EXISTS fk_ai_adoption_initiatives_organization;
ALTER TABLE ai_adoption_initiatives ADD CONSTRAINT fk_ai_adoption_initiatives_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE ai_adoption_initiatives DROP CONSTRAINT IF EXISTS fk_ai_adoption_initiatives_agent;
ALTER TABLE ai_adoption_initiatives ADD CONSTRAINT fk_ai_adoption_initiatives_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE ai_adoption_initiatives DROP CONSTRAINT IF EXISTS fk_ai_adoption_initiatives_target_procedure;
ALTER TABLE ai_adoption_initiatives ADD CONSTRAINT fk_ai_adoption_initiatives_target_procedure
  FOREIGN KEY (target_procedure) REFERENCES procedures (procedure_id);
ALTER TABLE ai_adoption_initiatives DROP CONSTRAINT IF EXISTS fk_ai_adoption_initiatives_target_version;
ALTER TABLE ai_adoption_initiatives ADD CONSTRAINT fk_ai_adoption_initiatives_target_version
  FOREIGN KEY (target_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE ai_adoption_initiatives DROP CONSTRAINT IF EXISTS fk_ai_adoption_initiatives_preceding_initiative;
ALTER TABLE ai_adoption_initiatives ADD CONSTRAINT fk_ai_adoption_initiatives_preceding_initiative
  FOREIGN KEY (preceding_initiative) REFERENCES ai_adoption_initiatives (ai_adoption_initiative_id);
ALTER TABLE ai_adoption_initiatives DROP CONSTRAINT IF EXISTS fk_ai_adoption_initiatives_evaluation_context;
ALTER TABLE ai_adoption_initiatives ADD CONSTRAINT fk_ai_adoption_initiatives_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- KnowledgeOutcomeMeasurements
ALTER TABLE knowledge_outcome_measurements DROP CONSTRAINT IF EXISTS fk_knowledge_outcome_measurements_facility;
ALTER TABLE knowledge_outcome_measurements ADD CONSTRAINT fk_knowledge_outcome_measurements_facility
  FOREIGN KEY (facility) REFERENCES facilities (facility_id);
ALTER TABLE knowledge_outcome_measurements DROP CONSTRAINT IF EXISTS fk_knowledge_outcome_measurements_procedure_version;
ALTER TABLE knowledge_outcome_measurements ADD CONSTRAINT fk_knowledge_outcome_measurements_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_outcome_measurements DROP CONSTRAINT IF EXISTS fk_knowledge_outcome_measurements_ai_initiative;
ALTER TABLE knowledge_outcome_measurements ADD CONSTRAINT fk_knowledge_outcome_measurements_ai_initiative
  FOREIGN KEY (ai_initiative) REFERENCES ai_adoption_initiatives (ai_adoption_initiative_id);
ALTER TABLE knowledge_outcome_measurements DROP CONSTRAINT IF EXISTS fk_knowledge_outcome_measurements_informed_change_request;
ALTER TABLE knowledge_outcome_measurements ADD CONSTRAINT fk_knowledge_outcome_measurements_informed_change_request
  FOREIGN KEY (informed_change_request) REFERENCES change_requests (change_request_id);
ALTER TABLE knowledge_outcome_measurements DROP CONSTRAINT IF EXISTS fk_knowledge_outcome_measurements_comparison_baseline;
ALTER TABLE knowledge_outcome_measurements ADD CONSTRAINT fk_knowledge_outcome_measurements_comparison_baseline
  FOREIGN KEY (comparison_baseline) REFERENCES knowledge_outcome_measurements (knowledge_outcome_measurement_id);

-- AiInsightProposals
ALTER TABLE ai_insight_proposals DROP CONSTRAINT IF EXISTS fk_ai_insight_proposals_proposing_agent;
ALTER TABLE ai_insight_proposals ADD CONSTRAINT fk_ai_insight_proposals_proposing_agent
  FOREIGN KEY (proposing_agent) REFERENCES agents (agent_id);
ALTER TABLE ai_insight_proposals DROP CONSTRAINT IF EXISTS fk_ai_insight_proposals_source_initiative;
ALTER TABLE ai_insight_proposals ADD CONSTRAINT fk_ai_insight_proposals_source_initiative
  FOREIGN KEY (source_initiative) REFERENCES ai_adoption_initiatives (ai_adoption_initiative_id);
ALTER TABLE ai_insight_proposals DROP CONSTRAINT IF EXISTS fk_ai_insight_proposals_target_procedure_version;
ALTER TABLE ai_insight_proposals ADD CONSTRAINT fk_ai_insight_proposals_target_procedure_version
  FOREIGN KEY (target_procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE ai_insight_proposals DROP CONSTRAINT IF EXISTS fk_ai_insight_proposals_validated_by_agent;
ALTER TABLE ai_insight_proposals ADD CONSTRAINT fk_ai_insight_proposals_validated_by_agent
  FOREIGN KEY (validated_by_agent) REFERENCES agents (agent_id);
ALTER TABLE ai_insight_proposals DROP CONSTRAINT IF EXISTS fk_ai_insight_proposals_folded_into_change_request;
ALTER TABLE ai_insight_proposals ADD CONSTRAINT fk_ai_insight_proposals_folded_into_change_request
  FOREIGN KEY (folded_into_change_request) REFERENCES change_requests (change_request_id);

-- AssistantBenchmarks
ALTER TABLE assistant_benchmarks DROP CONSTRAINT IF EXISTS fk_assistant_benchmarks_agent;
ALTER TABLE assistant_benchmarks ADD CONSTRAINT fk_assistant_benchmarks_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE assistant_benchmarks DROP CONSTRAINT IF EXISTS fk_assistant_benchmarks_grounding_snapshot;
ALTER TABLE assistant_benchmarks ADD CONSTRAINT fk_assistant_benchmarks_grounding_snapshot
  FOREIGN KEY (grounding_snapshot) REFERENCES grounding_snapshots (grounding_snapshot_id);

-- GovernedModels
ALTER TABLE governed_models DROP CONSTRAINT IF EXISTS fk_governed_models_procedure;
ALTER TABLE governed_models ADD CONSTRAINT fk_governed_models_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE governed_models DROP CONSTRAINT IF EXISTS fk_governed_models_domain_owning_organization;
ALTER TABLE governed_models ADD CONSTRAINT fk_governed_models_domain_owning_organization
  FOREIGN KEY (domain_owning_organization) REFERENCES organizations (organization_id);
ALTER TABLE governed_models DROP CONSTRAINT IF EXISTS fk_governed_models_tooling_owner_role;
ALTER TABLE governed_models ADD CONSTRAINT fk_governed_models_tooling_owner_role
  FOREIGN KEY (tooling_owner_role) REFERENCES roles (role_id);
ALTER TABLE governed_models DROP CONSTRAINT IF EXISTS fk_governed_models_current_charter;
ALTER TABLE governed_models ADD CONSTRAINT fk_governed_models_current_charter
  FOREIGN KEY (current_charter) REFERENCES model_charters (model_charter_id);
ALTER TABLE governed_models DROP CONSTRAINT IF EXISTS fk_governed_models_current_release;
ALTER TABLE governed_models ADD CONSTRAINT fk_governed_models_current_release
  FOREIGN KEY (current_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE governed_models DROP CONSTRAINT IF EXISTS fk_governed_models_evaluation_context;
ALTER TABLE governed_models ADD CONSTRAINT fk_governed_models_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- ModelCharters
ALTER TABLE model_charters DROP CONSTRAINT IF EXISTS fk_model_charters_governed_model;
ALTER TABLE model_charters ADD CONSTRAINT fk_model_charters_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_charters DROP CONSTRAINT IF EXISTS fk_model_charters_steward_role;
ALTER TABLE model_charters ADD CONSTRAINT fk_model_charters_steward_role
  FOREIGN KEY (steward_role) REFERENCES roles (role_id);
ALTER TABLE model_charters DROP CONSTRAINT IF EXISTS fk_model_charters_authority_role;
ALTER TABLE model_charters ADD CONSTRAINT fk_model_charters_authority_role
  FOREIGN KEY (authority_role) REFERENCES roles (role_id);
ALTER TABLE model_charters DROP CONSTRAINT IF EXISTS fk_model_charters_supersedes_charter;
ALTER TABLE model_charters ADD CONSTRAINT fk_model_charters_supersedes_charter
  FOREIGN KEY (supersedes_charter) REFERENCES model_charters (model_charter_id);
ALTER TABLE model_charters DROP CONSTRAINT IF EXISTS fk_model_charters_evaluation_context;
ALTER TABLE model_charters ADD CONSTRAINT fk_model_charters_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- StewardActivities
ALTER TABLE steward_activities DROP CONSTRAINT IF EXISTS fk_steward_activities_model_charter;
ALTER TABLE steward_activities ADD CONSTRAINT fk_steward_activities_model_charter
  FOREIGN KEY (model_charter) REFERENCES model_charters (model_charter_id);
ALTER TABLE steward_activities DROP CONSTRAINT IF EXISTS fk_steward_activities_performed_by_agent;
ALTER TABLE steward_activities ADD CONSTRAINT fk_steward_activities_performed_by_agent
  FOREIGN KEY (performed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE steward_activities DROP CONSTRAINT IF EXISTS fk_steward_activities_release;
ALTER TABLE steward_activities ADD CONSTRAINT fk_steward_activities_release
  FOREIGN KEY (release) REFERENCES rulebook_releases (rulebook_release_id);

-- ModelChangeRequests
ALTER TABLE model_change_requests DROP CONSTRAINT IF EXISTS fk_model_change_requests_governed_model;
ALTER TABLE model_change_requests ADD CONSTRAINT fk_model_change_requests_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_change_requests DROP CONSTRAINT IF EXISTS fk_model_change_requests_requested_by_agent;
ALTER TABLE model_change_requests ADD CONSTRAINT fk_model_change_requests_requested_by_agent
  FOREIGN KEY (requested_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_change_requests DROP CONSTRAINT IF EXISTS fk_model_change_requests_motivating_question;
ALTER TABLE model_change_requests ADD CONSTRAINT fk_model_change_requests_motivating_question
  FOREIGN KEY (motivating_question) REFERENCES role_questions (role_question_id);
ALTER TABLE model_change_requests DROP CONSTRAINT IF EXISTS fk_model_change_requests_approved_by_agent;
ALTER TABLE model_change_requests ADD CONSTRAINT fk_model_change_requests_approved_by_agent
  FOREIGN KEY (approved_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_change_requests DROP CONSTRAINT IF EXISTS fk_model_change_requests_placement_decided_by_agent;
ALTER TABLE model_change_requests ADD CONSTRAINT fk_model_change_requests_placement_decided_by_agent
  FOREIGN KEY (placement_decided_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_change_requests DROP CONSTRAINT IF EXISTS fk_model_change_requests_target_release;
ALTER TABLE model_change_requests ADD CONSTRAINT fk_model_change_requests_target_release
  FOREIGN KEY (target_release) REFERENCES rulebook_releases (rulebook_release_id);

-- ChangeAuthorityRules
ALTER TABLE change_authority_rules DROP CONSTRAINT IF EXISTS fk_change_authority_rules_governed_model;
ALTER TABLE change_authority_rules ADD CONSTRAINT fk_change_authority_rules_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE change_authority_rules DROP CONSTRAINT IF EXISTS fk_change_authority_rules_permitted_role;
ALTER TABLE change_authority_rules ADD CONSTRAINT fk_change_authority_rules_permitted_role
  FOREIGN KEY (permitted_role) REFERENCES roles (role_id);
ALTER TABLE change_authority_rules DROP CONSTRAINT IF EXISTS fk_change_authority_rules_approval_role;
ALTER TABLE change_authority_rules ADD CONSTRAINT fk_change_authority_rules_approval_role
  FOREIGN KEY (approval_role) REFERENCES roles (role_id);

-- ChangeImpactFindings
ALTER TABLE change_impact_findings DROP CONSTRAINT IF EXISTS fk_change_impact_findings_model_change_request;
ALTER TABLE change_impact_findings ADD CONSTRAINT fk_change_impact_findings_model_change_request
  FOREIGN KEY (model_change_request) REFERENCES model_change_requests (model_change_request_id);
ALTER TABLE change_impact_findings DROP CONSTRAINT IF EXISTS fk_change_impact_findings_affected_table;
ALTER TABLE change_impact_findings ADD CONSTRAINT fk_change_impact_findings_affected_table
  FOREIGN KEY (affected_table) REFERENCES rulebook_tables (rulebook_table_id);
ALTER TABLE change_impact_findings DROP CONSTRAINT IF EXISTS fk_change_impact_findings_found_by_agent;
ALTER TABLE change_impact_findings ADD CONSTRAINT fk_change_impact_findings_found_by_agent
  FOREIGN KEY (found_by_agent) REFERENCES agents (agent_id);

-- ChangeIntegrityChecks
ALTER TABLE change_integrity_checks DROP CONSTRAINT IF EXISTS fk_change_integrity_checks_model_change_request;
ALTER TABLE change_integrity_checks ADD CONSTRAINT fk_change_integrity_checks_model_change_request
  FOREIGN KEY (model_change_request) REFERENCES model_change_requests (model_change_request_id);
ALTER TABLE change_integrity_checks DROP CONSTRAINT IF EXISTS fk_change_integrity_checks_checked_by_agent;
ALTER TABLE change_integrity_checks ADD CONSTRAINT fk_change_integrity_checks_checked_by_agent
  FOREIGN KEY (checked_by_agent) REFERENCES agents (agent_id);

-- ChangeObjections
ALTER TABLE change_objections DROP CONSTRAINT IF EXISTS fk_change_objections_model_change_request;
ALTER TABLE change_objections ADD CONSTRAINT fk_change_objections_model_change_request
  FOREIGN KEY (model_change_request) REFERENCES model_change_requests (model_change_request_id);
ALTER TABLE change_objections DROP CONSTRAINT IF EXISTS fk_change_objections_raised_by_agent;
ALTER TABLE change_objections ADD CONSTRAINT fk_change_objections_raised_by_agent
  FOREIGN KEY (raised_by_agent) REFERENCES agents (agent_id);
ALTER TABLE change_objections DROP CONSTRAINT IF EXISTS fk_change_objections_resolved_by_agent;
ALTER TABLE change_objections ADD CONSTRAINT fk_change_objections_resolved_by_agent
  FOREIGN KEY (resolved_by_agent) REFERENCES agents (agent_id);

-- ChangeValidationRuns
ALTER TABLE change_validation_runs DROP CONSTRAINT IF EXISTS fk_change_validation_runs_model_change_request;
ALTER TABLE change_validation_runs ADD CONSTRAINT fk_change_validation_runs_model_change_request
  FOREIGN KEY (model_change_request) REFERENCES model_change_requests (model_change_request_id);
ALTER TABLE change_validation_runs DROP CONSTRAINT IF EXISTS fk_change_validation_runs_test_suite;
ALTER TABLE change_validation_runs ADD CONSTRAINT fk_change_validation_runs_test_suite
  FOREIGN KEY (test_suite) REFERENCES test_suites (test_suite_id);

-- ExpectedInferenceChecks
ALTER TABLE expected_inference_checks DROP CONSTRAINT IF EXISTS fk_expected_inference_checks_change_validation_run;
ALTER TABLE expected_inference_checks ADD CONSTRAINT fk_expected_inference_checks_change_validation_run
  FOREIGN KEY (change_validation_run) REFERENCES change_validation_runs (change_validation_run_id);
ALTER TABLE expected_inference_checks DROP CONSTRAINT IF EXISTS fk_expected_inference_checks_expected_field;
ALTER TABLE expected_inference_checks ADD CONSTRAINT fk_expected_inference_checks_expected_field
  FOREIGN KEY (expected_field) REFERENCES rulebook_fields (rulebook_field_id);

-- ModelConsumers
ALTER TABLE model_consumers DROP CONSTRAINT IF EXISTS fk_model_consumers_depends_on_model;
ALTER TABLE model_consumers ADD CONSTRAINT fk_model_consumers_depends_on_model
  FOREIGN KEY (depends_on_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_consumers DROP CONSTRAINT IF EXISTS fk_model_consumers_conformance_substrate;
ALTER TABLE model_consumers ADD CONSTRAINT fk_model_consumers_conformance_substrate
  FOREIGN KEY (conformance_substrate) REFERENCES conformance_substrates (conformance_substrate_id);
ALTER TABLE model_consumers DROP CONSTRAINT IF EXISTS fk_model_consumers_owner_role;
ALTER TABLE model_consumers ADD CONSTRAINT fk_model_consumers_owner_role
  FOREIGN KEY (owner_role) REFERENCES roles (role_id);

-- ConsumerRevalidations
ALTER TABLE consumer_revalidations DROP CONSTRAINT IF EXISTS fk_consumer_revalidations_rulebook_release;
ALTER TABLE consumer_revalidations ADD CONSTRAINT fk_consumer_revalidations_rulebook_release
  FOREIGN KEY (rulebook_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE consumer_revalidations DROP CONSTRAINT IF EXISTS fk_consumer_revalidations_model_consumer;
ALTER TABLE consumer_revalidations ADD CONSTRAINT fk_consumer_revalidations_model_consumer
  FOREIGN KEY (model_consumer) REFERENCES model_consumers (model_consumer_id);

-- ModelDocuments
ALTER TABLE model_documents DROP CONSTRAINT IF EXISTS fk_model_documents_governed_model;
ALTER TABLE model_documents ADD CONSTRAINT fk_model_documents_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_documents DROP CONSTRAINT IF EXISTS fk_model_documents_documented_release;
ALTER TABLE model_documents ADD CONSTRAINT fk_model_documents_documented_release
  FOREIGN KEY (documented_release) REFERENCES rulebook_releases (rulebook_release_id);

-- StalenessQueryRuns
ALTER TABLE staleness_query_runs DROP CONSTRAINT IF EXISTS fk_staleness_query_runs_governed_model;
ALTER TABLE staleness_query_runs ADD CONSTRAINT fk_staleness_query_runs_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE staleness_query_runs DROP CONSTRAINT IF EXISTS fk_staleness_query_runs_ran_by_agent;
ALTER TABLE staleness_query_runs ADD CONSTRAINT fk_staleness_query_runs_ran_by_agent
  FOREIGN KEY (ran_by_agent) REFERENCES agents (agent_id);

-- ExternalDependencyRevisions
ALTER TABLE external_dependency_revisions DROP CONSTRAINT IF EXISTS fk_external_dependency_revisions_ontology_profile;
ALTER TABLE external_dependency_revisions ADD CONSTRAINT fk_external_dependency_revisions_ontology_profile
  FOREIGN KEY (ontology_profile) REFERENCES ontology_profiles (ontology_profile_id);
ALTER TABLE external_dependency_revisions DROP CONSTRAINT IF EXISTS fk_external_dependency_revisions_tracked_by_agent;
ALTER TABLE external_dependency_revisions ADD CONSTRAINT fk_external_dependency_revisions_tracked_by_agent
  FOREIGN KEY (tracked_by_agent) REFERENCES agents (agent_id);
ALTER TABLE external_dependency_revisions DROP CONSTRAINT IF EXISTS fk_external_dependency_revisions_evaluation_context;
ALTER TABLE external_dependency_revisions ADD CONSTRAINT fk_external_dependency_revisions_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- StakeholderQuestions
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_governed_model;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_asked_by_agent;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_asked_by_agent
  FOREIGN KEY (asked_by_agent) REFERENCES agents (agent_id);
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_answering_role_question;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_answering_role_question
  FOREIGN KEY (answering_role_question) REFERENCES role_questions (role_question_id);
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_answered_by_agent;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_answered_by_agent
  FOREIGN KEY (answered_by_agent) REFERENCES agents (agent_id);
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_triaged_by_agent;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_triaged_by_agent
  FOREIGN KEY (triaged_by_agent) REFERENCES agents (agent_id);
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_resulting_change_request;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_resulting_change_request
  FOREIGN KEY (resulting_change_request) REFERENCES model_change_requests (model_change_request_id);
ALTER TABLE stakeholder_questions DROP CONSTRAINT IF EXISTS fk_stakeholder_questions_evaluation_context;
ALTER TABLE stakeholder_questions ADD CONSTRAINT fk_stakeholder_questions_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- ModelExpansionRequests
ALTER TABLE model_expansion_requests DROP CONSTRAINT IF EXISTS fk_model_expansion_requests_governed_model;
ALTER TABLE model_expansion_requests ADD CONSTRAINT fk_model_expansion_requests_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_expansion_requests DROP CONSTRAINT IF EXISTS fk_model_expansion_requests_requesting_organization;
ALTER TABLE model_expansion_requests ADD CONSTRAINT fk_model_expansion_requests_requesting_organization
  FOREIGN KEY (requesting_organization) REFERENCES organizations (organization_id);
ALTER TABLE model_expansion_requests DROP CONSTRAINT IF EXISTS fk_model_expansion_requests_requested_by_agent;
ALTER TABLE model_expansion_requests ADD CONSTRAINT fk_model_expansion_requests_requested_by_agent
  FOREIGN KEY (requested_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_expansion_requests DROP CONSTRAINT IF EXISTS fk_model_expansion_requests_decided_by_agent;
ALTER TABLE model_expansion_requests ADD CONSTRAINT fk_model_expansion_requests_decided_by_agent
  FOREIGN KEY (decided_by_agent) REFERENCES agents (agent_id);

-- ExpansionConceptFits
ALTER TABLE expansion_concept_fits DROP CONSTRAINT IF EXISTS fk_expansion_concept_fits_model_expansion_request;
ALTER TABLE expansion_concept_fits ADD CONSTRAINT fk_expansion_concept_fits_model_expansion_request
  FOREIGN KEY (model_expansion_request) REFERENCES model_expansion_requests (model_expansion_request_id);
ALTER TABLE expansion_concept_fits DROP CONSTRAINT IF EXISTS fk_expansion_concept_fits_covering_table;
ALTER TABLE expansion_concept_fits ADD CONSTRAINT fk_expansion_concept_fits_covering_table
  FOREIGN KEY (covering_table) REFERENCES rulebook_tables (rulebook_table_id);

-- CompetencyQuestionSetEntries
ALTER TABLE competency_question_set_entries DROP CONSTRAINT IF EXISTS fk_competency_question_set_entries_governed_model;
ALTER TABLE competency_question_set_entries ADD CONSTRAINT fk_competency_question_set_entries_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE competency_question_set_entries DROP CONSTRAINT IF EXISTS fk_competency_question_set_entries_role_question;
ALTER TABLE competency_question_set_entries ADD CONSTRAINT fk_competency_question_set_entries_role_question
  FOREIGN KEY (role_question) REFERENCES role_questions (role_question_id);
ALTER TABLE competency_question_set_entries DROP CONSTRAINT IF EXISTS fk_competency_question_set_entries_evaluation_context;
ALTER TABLE competency_question_set_entries ADD CONSTRAINT fk_competency_question_set_entries_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- CompetencyQuestionRuns
ALTER TABLE competency_question_runs DROP CONSTRAINT IF EXISTS fk_competency_question_runs_cq_set_entry;
ALTER TABLE competency_question_runs ADD CONSTRAINT fk_competency_question_runs_cq_set_entry
  FOREIGN KEY (cq_set_entry) REFERENCES competency_question_set_entries (competency_question_set_entry_id);
ALTER TABLE competency_question_runs DROP CONSTRAINT IF EXISTS fk_competency_question_runs_rulebook_release;
ALTER TABLE competency_question_runs ADD CONSTRAINT fk_competency_question_runs_rulebook_release
  FOREIGN KEY (rulebook_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE competency_question_runs DROP CONSTRAINT IF EXISTS fk_competency_question_runs_prior_run;
ALTER TABLE competency_question_runs ADD CONSTRAINT fk_competency_question_runs_prior_run
  FOREIGN KEY (prior_run) REFERENCES competency_question_runs (competency_question_run_id);
ALTER TABLE competency_question_runs DROP CONSTRAINT IF EXISTS fk_competency_question_runs_defect_change_request;
ALTER TABLE competency_question_runs ADD CONSTRAINT fk_competency_question_runs_defect_change_request
  FOREIGN KEY (defect_change_request) REFERENCES model_change_requests (model_change_request_id);

-- CompetencyQuestionReviews
ALTER TABLE competency_question_reviews DROP CONSTRAINT IF EXISTS fk_competency_question_reviews_governed_model;
ALTER TABLE competency_question_reviews ADD CONSTRAINT fk_competency_question_reviews_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE competency_question_reviews DROP CONSTRAINT IF EXISTS fk_competency_question_reviews_reviewed_by_agent;
ALTER TABLE competency_question_reviews ADD CONSTRAINT fk_competency_question_reviews_reviewed_by_agent
  FOREIGN KEY (reviewed_by_agent) REFERENCES agents (agent_id);

-- QualityAssessments
ALTER TABLE quality_assessments DROP CONSTRAINT IF EXISTS fk_quality_assessments_rulebook_release;
ALTER TABLE quality_assessments ADD CONSTRAINT fk_quality_assessments_rulebook_release
  FOREIGN KEY (rulebook_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE quality_assessments DROP CONSTRAINT IF EXISTS fk_quality_assessments_quality_criterion;
ALTER TABLE quality_assessments ADD CONSTRAINT fk_quality_assessments_quality_criterion
  FOREIGN KEY (quality_criterion) REFERENCES quality_criteria (quality_criterion_id);
ALTER TABLE quality_assessments DROP CONSTRAINT IF EXISTS fk_quality_assessments_assessed_by_agent;
ALTER TABLE quality_assessments ADD CONSTRAINT fk_quality_assessments_assessed_by_agent
  FOREIGN KEY (assessed_by_agent) REFERENCES agents (agent_id);

-- TermDefinitions
ALTER TABLE term_definitions DROP CONSTRAINT IF EXISTS fk_term_definitions_rulebook_table;
ALTER TABLE term_definitions ADD CONSTRAINT fk_term_definitions_rulebook_table
  FOREIGN KEY (rulebook_table) REFERENCES rulebook_tables (rulebook_table_id);
ALTER TABLE term_definitions DROP CONSTRAINT IF EXISTS fk_term_definitions_drafted_by_agent;
ALTER TABLE term_definitions ADD CONSTRAINT fk_term_definitions_drafted_by_agent
  FOREIGN KEY (drafted_by_agent) REFERENCES agents (agent_id);
ALTER TABLE term_definitions DROP CONSTRAINT IF EXISTS fk_term_definitions_revised_by_agent;
ALTER TABLE term_definitions ADD CONSTRAINT fk_term_definitions_revised_by_agent
  FOREIGN KEY (revised_by_agent) REFERENCES agents (agent_id);

-- ModelProposals
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_governed_model;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_proposed_by_agent;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_proposed_by_agent
  FOREIGN KEY (proposed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_source_document;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_source_document
  FOREIGN KEY (source_document) REFERENCES resources (resource_id);
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_reviewed_by_agent;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_reviewed_by_agent
  FOREIGN KEY (reviewed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_committed_by_agent;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_committed_by_agent
  FOREIGN KEY (committed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_adopted_in_release;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_adopted_in_release
  FOREIGN KEY (adopted_in_release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE model_proposals DROP CONSTRAINT IF EXISTS fk_model_proposals_adopted_in_data_version;
ALTER TABLE model_proposals ADD CONSTRAINT fk_model_proposals_adopted_in_data_version
  FOREIGN KEY (adopted_in_data_version) REFERENCES instance_data_versions (instance_data_version_id);

-- AssignmentInstantChecks
ALTER TABLE assignment_instant_checks DROP CONSTRAINT IF EXISTS fk_assignment_instant_checks_step;
ALTER TABLE assignment_instant_checks ADD CONSTRAINT fk_assignment_instant_checks_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE assignment_instant_checks DROP CONSTRAINT IF EXISTS fk_assignment_instant_checks_role_assignment;
ALTER TABLE assignment_instant_checks ADD CONSTRAINT fk_assignment_instant_checks_role_assignment
  FOREIGN KEY (role_assignment) REFERENCES role_assignments (role_assignment_id);

-- InstanceDataVersions
ALTER TABLE instance_data_versions DROP CONSTRAINT IF EXISTS fk_instance_data_versions_governed_model;
ALTER TABLE instance_data_versions ADD CONSTRAINT fk_instance_data_versions_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE instance_data_versions DROP CONSTRAINT IF EXISTS fk_instance_data_versions_conforms_to_release;
ALTER TABLE instance_data_versions ADD CONSTRAINT fk_instance_data_versions_conforms_to_release
  FOREIGN KEY (conforms_to_release) REFERENCES rulebook_releases (rulebook_release_id);

-- DomainCoverageAreas
ALTER TABLE domain_coverage_areas DROP CONSTRAINT IF EXISTS fk_domain_coverage_areas_governed_model;
ALTER TABLE domain_coverage_areas ADD CONSTRAINT fk_domain_coverage_areas_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE domain_coverage_areas DROP CONSTRAINT IF EXISTS fk_domain_coverage_areas_covering_table;
ALTER TABLE domain_coverage_areas ADD CONSTRAINT fk_domain_coverage_areas_covering_table
  FOREIGN KEY (covering_table) REFERENCES rulebook_tables (rulebook_table_id);

-- GovernanceStageControls
ALTER TABLE governance_stage_controls DROP CONSTRAINT IF EXISTS fk_governance_stage_controls_governed_model;
ALTER TABLE governance_stage_controls ADD CONSTRAINT fk_governance_stage_controls_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);

-- ProcessDesignDecisions
ALTER TABLE process_design_decisions DROP CONSTRAINT IF EXISTS fk_process_design_decisions_procedure_version;
ALTER TABLE process_design_decisions ADD CONSTRAINT fk_process_design_decisions_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE process_design_decisions DROP CONSTRAINT IF EXISTS fk_process_design_decisions_step;
ALTER TABLE process_design_decisions ADD CONSTRAINT fk_process_design_decisions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE process_design_decisions DROP CONSTRAINT IF EXISTS fk_process_design_decisions_decided_by_agent;
ALTER TABLE process_design_decisions ADD CONSTRAINT fk_process_design_decisions_decided_by_agent
  FOREIGN KEY (decided_by_agent) REFERENCES agents (agent_id);

-- ModelChangeLogEntries
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_governed_model;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_model_change_request;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_model_change_request
  FOREIGN KEY (model_change_request) REFERENCES model_change_requests (model_change_request_id);
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_release;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_release
  FOREIGN KEY (release) REFERENCES rulebook_releases (rulebook_release_id);
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_instance_data_version;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_instance_data_version
  FOREIGN KEY (instance_data_version) REFERENCES instance_data_versions (instance_data_version_id);
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_affected_table;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_affected_table
  FOREIGN KEY (affected_table) REFERENCES rulebook_tables (rulebook_table_id);
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_changed_by_agent;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_changed_by_agent
  FOREIGN KEY (changed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE model_change_log_entries DROP CONSTRAINT IF EXISTS fk_model_change_log_entries_reverts_entry;
ALTER TABLE model_change_log_entries ADD CONSTRAINT fk_model_change_log_entries_reverts_entry
  FOREIGN KEY (reverts_entry) REFERENCES model_change_log_entries (model_change_log_entry_id);

-- DriftObservations
ALTER TABLE drift_observations DROP CONSTRAINT IF EXISTS fk_drift_observations_governed_model;
ALTER TABLE drift_observations ADD CONSTRAINT fk_drift_observations_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE drift_observations DROP CONSTRAINT IF EXISTS fk_drift_observations_procedure_version;
ALTER TABLE drift_observations ADD CONSTRAINT fk_drift_observations_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE drift_observations DROP CONSTRAINT IF EXISTS fk_drift_observations_observed_by_agent;
ALTER TABLE drift_observations ADD CONSTRAINT fk_drift_observations_observed_by_agent
  FOREIGN KEY (observed_by_agent) REFERENCES agents (agent_id);
ALTER TABLE drift_observations DROP CONSTRAINT IF EXISTS fk_drift_observations_since_release;
ALTER TABLE drift_observations ADD CONSTRAINT fk_drift_observations_since_release
  FOREIGN KEY (since_release) REFERENCES rulebook_releases (rulebook_release_id);

-- SourcingFunctions
ALTER TABLE sourcing_functions DROP CONSTRAINT IF EXISTS fk_sourcing_functions_client_organization;
ALTER TABLE sourcing_functions ADD CONSTRAINT fk_sourcing_functions_client_organization
  FOREIGN KEY (client_organization) REFERENCES organizations (organization_id);
ALTER TABLE sourcing_functions DROP CONSTRAINT IF EXISTS fk_sourcing_functions_procedure;
ALTER TABLE sourcing_functions ADD CONSTRAINT fk_sourcing_functions_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE sourcing_functions DROP CONSTRAINT IF EXISTS fk_sourcing_functions_executing_organization;
ALTER TABLE sourcing_functions ADD CONSTRAINT fk_sourcing_functions_executing_organization
  FOREIGN KEY (executing_organization) REFERENCES organizations (organization_id);
ALTER TABLE sourcing_functions DROP CONSTRAINT IF EXISTS fk_sourcing_functions_specification_holder;
ALTER TABLE sourcing_functions ADD CONSTRAINT fk_sourcing_functions_specification_holder
  FOREIGN KEY (specification_holder) REFERENCES organizations (organization_id);
ALTER TABLE sourcing_functions DROP CONSTRAINT IF EXISTS fk_sourcing_functions_method_holder;
ALTER TABLE sourcing_functions ADD CONSTRAINT fk_sourcing_functions_method_holder
  FOREIGN KEY (method_holder) REFERENCES organizations (organization_id);

-- KnowledgeAudits
ALTER TABLE knowledge_audits DROP CONSTRAINT IF EXISTS fk_knowledge_audits_organization;
ALTER TABLE knowledge_audits ADD CONSTRAINT fk_knowledge_audits_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE knowledge_audits DROP CONSTRAINT IF EXISTS fk_knowledge_audits_conducted_by_agent;
ALTER TABLE knowledge_audits ADD CONSTRAINT fk_knowledge_audits_conducted_by_agent
  FOREIGN KEY (conducted_by_agent) REFERENCES agents (agent_id);

-- KnowledgeAuditItems
ALTER TABLE knowledge_audit_items DROP CONSTRAINT IF EXISTS fk_knowledge_audit_items_knowledge_audit;
ALTER TABLE knowledge_audit_items ADD CONSTRAINT fk_knowledge_audit_items_knowledge_audit
  FOREIGN KEY (knowledge_audit) REFERENCES knowledge_audits (knowledge_audit_id);
ALTER TABLE knowledge_audit_items DROP CONSTRAINT IF EXISTS fk_knowledge_audit_items_sourcing_function;
ALTER TABLE knowledge_audit_items ADD CONSTRAINT fk_knowledge_audit_items_sourcing_function
  FOREIGN KEY (sourcing_function) REFERENCES sourcing_functions (sourcing_function_id);
ALTER TABLE knowledge_audit_items DROP CONSTRAINT IF EXISTS fk_knowledge_audit_items_provider_holding_knowledge;
ALTER TABLE knowledge_audit_items ADD CONSTRAINT fk_knowledge_audit_items_provider_holding_knowledge
  FOREIGN KEY (provider_holding_knowledge) REFERENCES organizations (organization_id);
ALTER TABLE knowledge_audit_items DROP CONSTRAINT IF EXISTS fk_knowledge_audit_items_named_knowledge_gap;
ALTER TABLE knowledge_audit_items ADD CONSTRAINT fk_knowledge_audit_items_named_knowledge_gap
  FOREIGN KEY (named_knowledge_gap) REFERENCES knowledge_gaps (knowledge_gap_id);

-- KnowledgeCaptureInitiatives
ALTER TABLE knowledge_capture_initiatives DROP CONSTRAINT IF EXISTS fk_knowledge_capture_initiatives_sourcing_function;
ALTER TABLE knowledge_capture_initiatives ADD CONSTRAINT fk_knowledge_capture_initiatives_sourcing_function
  FOREIGN KEY (sourcing_function) REFERENCES sourcing_functions (sourcing_function_id);
ALTER TABLE knowledge_capture_initiatives DROP CONSTRAINT IF EXISTS fk_knowledge_capture_initiatives_knowledge_method;
ALTER TABLE knowledge_capture_initiatives ADD CONSTRAINT fk_knowledge_capture_initiatives_knowledge_method
  FOREIGN KEY (knowledge_method) REFERENCES knowledge_methods (knowledge_method_id);
ALTER TABLE knowledge_capture_initiatives DROP CONSTRAINT IF EXISTS fk_knowledge_capture_initiatives_lead_agent;
ALTER TABLE knowledge_capture_initiatives ADD CONSTRAINT fk_knowledge_capture_initiatives_lead_agent
  FOREIGN KEY (lead_agent) REFERENCES agents (agent_id);

-- KnowledgeWorkforcePositions
ALTER TABLE knowledge_workforce_positions DROP CONSTRAINT IF EXISTS fk_knowledge_workforce_positions_organization;
ALTER TABLE knowledge_workforce_positions ADD CONSTRAINT fk_knowledge_workforce_positions_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE knowledge_workforce_positions DROP CONSTRAINT IF EXISTS fk_knowledge_workforce_positions_role;
ALTER TABLE knowledge_workforce_positions ADD CONSTRAINT fk_knowledge_workforce_positions_role
  FOREIGN KEY (role) REFERENCES roles (role_id);
ALTER TABLE knowledge_workforce_positions DROP CONSTRAINT IF EXISTS fk_knowledge_workforce_positions_filled_by_agent;
ALTER TABLE knowledge_workforce_positions ADD CONSTRAINT fk_knowledge_workforce_positions_filled_by_agent
  FOREIGN KEY (filled_by_agent) REFERENCES agents (agent_id);

-- ProviderEngagements
ALTER TABLE provider_engagements DROP CONSTRAINT IF EXISTS fk_provider_engagements_client_organization;
ALTER TABLE provider_engagements ADD CONSTRAINT fk_provider_engagements_client_organization
  FOREIGN KEY (client_organization) REFERENCES organizations (organization_id);
ALTER TABLE provider_engagements DROP CONSTRAINT IF EXISTS fk_provider_engagements_provider;
ALTER TABLE provider_engagements ADD CONSTRAINT fk_provider_engagements_provider
  FOREIGN KEY (provider) REFERENCES organizations (organization_id);
ALTER TABLE provider_engagements DROP CONSTRAINT IF EXISTS fk_provider_engagements_sourcing_function;
ALTER TABLE provider_engagements ADD CONSTRAINT fk_provider_engagements_sourcing_function
  FOREIGN KEY (sourcing_function) REFERENCES sourcing_functions (sourcing_function_id);

-- KnowledgeDeliverables
ALTER TABLE knowledge_deliverables DROP CONSTRAINT IF EXISTS fk_knowledge_deliverables_provider_engagement;
ALTER TABLE knowledge_deliverables ADD CONSTRAINT fk_knowledge_deliverables_provider_engagement
  FOREIGN KEY (provider_engagement) REFERENCES provider_engagements (provider_engagement_id);

-- CorporateGovernancePrograms
ALTER TABLE corporate_governance_programs DROP CONSTRAINT IF EXISTS fk_corporate_governance_programs_organization;
ALTER TABLE corporate_governance_programs ADD CONSTRAINT fk_corporate_governance_programs_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);

-- RecordsRetentionPolicies
ALTER TABLE records_retention_policies DROP CONSTRAINT IF EXISTS fk_records_retention_policies_governance_program;
ALTER TABLE records_retention_policies ADD CONSTRAINT fk_records_retention_policies_governance_program
  FOREIGN KEY (governance_program) REFERENCES corporate_governance_programs (corporate_governance_program_id);

-- AiModelDeployments
ALTER TABLE ai_model_deployments DROP CONSTRAINT IF EXISTS fk_ai_model_deployments_model_version;
ALTER TABLE ai_model_deployments ADD CONSTRAINT fk_ai_model_deployments_model_version
  FOREIGN KEY (model_version) REFERENCES ai_registry_model_versions (ai_registry_model_version_id);
ALTER TABLE ai_model_deployments DROP CONSTRAINT IF EXISTS fk_ai_model_deployments_evaluation_context;
ALTER TABLE ai_model_deployments ADD CONSTRAINT fk_ai_model_deployments_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- AiModelEvaluations
ALTER TABLE ai_model_evaluations DROP CONSTRAINT IF EXISTS fk_ai_model_evaluations_model_version;
ALTER TABLE ai_model_evaluations ADD CONSTRAINT fk_ai_model_evaluations_model_version
  FOREIGN KEY (model_version) REFERENCES ai_registry_model_versions (ai_registry_model_version_id);

-- AiAgentAccountabilities
ALTER TABLE ai_agent_accountabilities DROP CONSTRAINT IF EXISTS fk_ai_agent_accountabilities_ai_agent;
ALTER TABLE ai_agent_accountabilities ADD CONSTRAINT fk_ai_agent_accountabilities_ai_agent
  FOREIGN KEY (ai_agent) REFERENCES agents (agent_id);
ALTER TABLE ai_agent_accountabilities DROP CONSTRAINT IF EXISTS fk_ai_agent_accountabilities_accountable_agent;
ALTER TABLE ai_agent_accountabilities ADD CONSTRAINT fk_ai_agent_accountabilities_accountable_agent
  FOREIGN KEY (accountable_agent) REFERENCES agents (agent_id);
ALTER TABLE ai_agent_accountabilities DROP CONSTRAINT IF EXISTS fk_ai_agent_accountabilities_evaluation_context;
ALTER TABLE ai_agent_accountabilities ADD CONSTRAINT fk_ai_agent_accountabilities_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- AgentUpgradeAssessments
ALTER TABLE agent_upgrade_assessments DROP CONSTRAINT IF EXISTS fk_agent_upgrade_assessments_current_agent;
ALTER TABLE agent_upgrade_assessments ADD CONSTRAINT fk_agent_upgrade_assessments_current_agent
  FOREIGN KEY (current_agent) REFERENCES agents (agent_id);
ALTER TABLE agent_upgrade_assessments DROP CONSTRAINT IF EXISTS fk_agent_upgrade_assessments_candidate_agent;
ALTER TABLE agent_upgrade_assessments ADD CONSTRAINT fk_agent_upgrade_assessments_candidate_agent
  FOREIGN KEY (candidate_agent) REFERENCES agents (agent_id);
ALTER TABLE agent_upgrade_assessments DROP CONSTRAINT IF EXISTS fk_agent_upgrade_assessments_assessed_by_agent;
ALTER TABLE agent_upgrade_assessments ADD CONSTRAINT fk_agent_upgrade_assessments_assessed_by_agent
  FOREIGN KEY (assessed_by_agent) REFERENCES agents (agent_id);

-- AssignmentUpdatePolicies
ALTER TABLE assignment_update_policies DROP CONSTRAINT IF EXISTS fk_assignment_update_policies_trigger_owner_role;
ALTER TABLE assignment_update_policies ADD CONSTRAINT fk_assignment_update_policies_trigger_owner_role
  FOREIGN KEY (trigger_owner_role) REFERENCES roles (role_id);

-- RoleAssignmentUpdateTasks
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_role;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_role
  FOREIGN KEY (role) REFERENCES roles (role_id);
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_governing_policy;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_governing_policy
  FOREIGN KEY (governing_policy) REFERENCES assignment_update_policies (assignment_update_policy_id);
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_triggered_by_agent;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_triggered_by_agent
  FOREIGN KEY (triggered_by_agent) REFERENCES agents (agent_id);
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_ending_assignment;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_ending_assignment
  FOREIGN KEY (ending_assignment) REFERENCES role_assignments (role_assignment_id);
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_replacement_assignment;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_replacement_assignment
  FOREIGN KEY (replacement_assignment) REFERENCES role_assignments (role_assignment_id);
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_dependent_execution;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_dependent_execution
  FOREIGN KEY (dependent_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE role_assignment_update_tasks DROP CONSTRAINT IF EXISTS fk_role_assignment_update_tasks_evaluation_context;
ALTER TABLE role_assignment_update_tasks ADD CONSTRAINT fk_role_assignment_update_tasks_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- AssignmentRoutedNotices
ALTER TABLE assignment_routed_notices DROP CONSTRAINT IF EXISTS fk_assignment_routed_notices_procedure_execution;
ALTER TABLE assignment_routed_notices ADD CONSTRAINT fk_assignment_routed_notices_procedure_execution
  FOREIGN KEY (procedure_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE assignment_routed_notices DROP CONSTRAINT IF EXISTS fk_assignment_routed_notices_notice_step;
ALTER TABLE assignment_routed_notices ADD CONSTRAINT fk_assignment_routed_notices_notice_step
  FOREIGN KEY (notice_step) REFERENCES steps (step_id);
ALTER TABLE assignment_routed_notices DROP CONSTRAINT IF EXISTS fk_assignment_routed_notices_routed_to_agent;
ALTER TABLE assignment_routed_notices ADD CONSTRAINT fk_assignment_routed_notices_routed_to_agent
  FOREIGN KEY (routed_to_agent) REFERENCES agents (agent_id);

-- PractitionerExpertise
ALTER TABLE practitioner_expertise DROP CONSTRAINT IF EXISTS fk_practitioner_expertise_agent;
ALTER TABLE practitioner_expertise ADD CONSTRAINT fk_practitioner_expertise_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE practitioner_expertise DROP CONSTRAINT IF EXISTS fk_practitioner_expertise_procedure;
ALTER TABLE practitioner_expertise ADD CONSTRAINT fk_practitioner_expertise_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE practitioner_expertise DROP CONSTRAINT IF EXISTS fk_practitioner_expertise_step;
ALTER TABLE practitioner_expertise ADD CONSTRAINT fk_practitioner_expertise_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE practitioner_expertise DROP CONSTRAINT IF EXISTS fk_practitioner_expertise_elicitation_session;
ALTER TABLE practitioner_expertise ADD CONSTRAINT fk_practitioner_expertise_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);

-- CriticalIncidents
ALTER TABLE critical_incidents DROP CONSTRAINT IF EXISTS fk_critical_incidents_procedure_version;
ALTER TABLE critical_incidents ADD CONSTRAINT fk_critical_incidents_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE critical_incidents DROP CONSTRAINT IF EXISTS fk_critical_incidents_step;
ALTER TABLE critical_incidents ADD CONSTRAINT fk_critical_incidents_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE critical_incidents DROP CONSTRAINT IF EXISTS fk_critical_incidents_elicitation_session;
ALTER TABLE critical_incidents ADD CONSTRAINT fk_critical_incidents_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE critical_incidents DROP CONSTRAINT IF EXISTS fk_critical_incidents_narrator;
ALTER TABLE critical_incidents ADD CONSTRAINT fk_critical_incidents_narrator
  FOREIGN KEY (narrator) REFERENCES agents (agent_id);
ALTER TABLE critical_incidents DROP CONSTRAINT IF EXISTS fk_critical_incidents_judgment_fragment;
ALTER TABLE critical_incidents ADD CONSTRAINT fk_critical_incidents_judgment_fragment
  FOREIGN KEY (judgment_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);

-- InterviewProbes
ALTER TABLE interview_probes DROP CONSTRAINT IF EXISTS fk_interview_probes_elicitation_session;
ALTER TABLE interview_probes ADD CONSTRAINT fk_interview_probes_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE interview_probes DROP CONSTRAINT IF EXISTS fk_interview_probes_step;
ALTER TABLE interview_probes ADD CONSTRAINT fk_interview_probes_step
  FOREIGN KEY (step) REFERENCES steps (step_id);

-- ObservedActions
ALTER TABLE observed_actions DROP CONSTRAINT IF EXISTS fk_observed_actions_elicitation_session;
ALTER TABLE observed_actions ADD CONSTRAINT fk_observed_actions_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE observed_actions DROP CONSTRAINT IF EXISTS fk_observed_actions_step;
ALTER TABLE observed_actions ADD CONSTRAINT fk_observed_actions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE observed_actions DROP CONSTRAINT IF EXISTS fk_observed_actions_practitioner;
ALTER TABLE observed_actions ADD CONSTRAINT fk_observed_actions_practitioner
  FOREIGN KEY (practitioner) REFERENCES agents (agent_id);
ALTER TABLE observed_actions DROP CONSTRAINT IF EXISTS fk_observed_actions_captured_as_fragment;
ALTER TABLE observed_actions ADD CONSTRAINT fk_observed_actions_captured_as_fragment
  FOREIGN KEY (captured_as_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);

-- ElicitationParticipants
ALTER TABLE elicitation_participants DROP CONSTRAINT IF EXISTS fk_elicitation_participants_elicitation_session;
ALTER TABLE elicitation_participants ADD CONSTRAINT fk_elicitation_participants_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE elicitation_participants DROP CONSTRAINT IF EXISTS fk_elicitation_participants_agent;
ALTER TABLE elicitation_participants ADD CONSTRAINT fk_elicitation_participants_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);

-- RepresentationReviews
ALTER TABLE representation_reviews DROP CONSTRAINT IF EXISTS fk_representation_reviews_procedure_version;
ALTER TABLE representation_reviews ADD CONSTRAINT fk_representation_reviews_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE representation_reviews DROP CONSTRAINT IF EXISTS fk_representation_reviews_reviewer_agent;
ALTER TABLE representation_reviews ADD CONSTRAINT fk_representation_reviews_reviewer_agent
  FOREIGN KEY (reviewer_agent) REFERENCES agents (agent_id);
ALTER TABLE representation_reviews DROP CONSTRAINT IF EXISTS fk_representation_reviews_decision;
ALTER TABLE representation_reviews ADD CONSTRAINT fk_representation_reviews_decision
  FOREIGN KEY (decision) REFERENCES lifecycle_statuses (lifecycle_status_id);

-- WorkflowViewDivergences
ALTER TABLE workflow_view_divergences DROP CONSTRAINT IF EXISTS fk_workflow_view_divergences_elicitation_session;
ALTER TABLE workflow_view_divergences ADD CONSTRAINT fk_workflow_view_divergences_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE workflow_view_divergences DROP CONSTRAINT IF EXISTS fk_workflow_view_divergences_procedure_version;
ALTER TABLE workflow_view_divergences ADD CONSTRAINT fk_workflow_view_divergences_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE workflow_view_divergences DROP CONSTRAINT IF EXISTS fk_workflow_view_divergences_step;
ALTER TABLE workflow_view_divergences ADD CONSTRAINT fk_workflow_view_divergences_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE workflow_view_divergences DROP CONSTRAINT IF EXISTS fk_workflow_view_divergences_holder_a;
ALTER TABLE workflow_view_divergences ADD CONSTRAINT fk_workflow_view_divergences_holder_a
  FOREIGN KEY (holder_a) REFERENCES agents (agent_id);
ALTER TABLE workflow_view_divergences DROP CONSTRAINT IF EXISTS fk_workflow_view_divergences_holder_b;
ALTER TABLE workflow_view_divergences ADD CONSTRAINT fk_workflow_view_divergences_holder_b
  FOREIGN KEY (holder_b) REFERENCES agents (agent_id);
ALTER TABLE workflow_view_divergences DROP CONSTRAINT IF EXISTS fk_workflow_view_divergences_reconciled_into_fragment;
ALTER TABLE workflow_view_divergences ADD CONSTRAINT fk_workflow_view_divergences_reconciled_into_fragment
  FOREIGN KEY (reconciled_into_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);

-- ExpertCognitions
ALTER TABLE expert_cognitions DROP CONSTRAINT IF EXISTS fk_expert_cognitions_agent;
ALTER TABLE expert_cognitions ADD CONSTRAINT fk_expert_cognitions_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);
ALTER TABLE expert_cognitions DROP CONSTRAINT IF EXISTS fk_expert_cognitions_step;
ALTER TABLE expert_cognitions ADD CONSTRAINT fk_expert_cognitions_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE expert_cognitions DROP CONSTRAINT IF EXISTS fk_expert_cognitions_elicitation_session;
ALTER TABLE expert_cognitions ADD CONSTRAINT fk_expert_cognitions_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);

-- ConceptLadderRungs
ALTER TABLE concept_ladder_rungs DROP CONSTRAINT IF EXISTS fk_concept_ladder_rungs_elicitation_session;
ALTER TABLE concept_ladder_rungs ADD CONSTRAINT fk_concept_ladder_rungs_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE concept_ladder_rungs DROP CONSTRAINT IF EXISTS fk_concept_ladder_rungs_step;
ALTER TABLE concept_ladder_rungs ADD CONSTRAINT fk_concept_ladder_rungs_step
  FOREIGN KEY (step) REFERENCES steps (step_id);

-- RepertoryGridConstructs
ALTER TABLE repertory_grid_constructs DROP CONSTRAINT IF EXISTS fk_repertory_grid_constructs_elicitation_session;
ALTER TABLE repertory_grid_constructs ADD CONSTRAINT fk_repertory_grid_constructs_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE repertory_grid_constructs DROP CONSTRAINT IF EXISTS fk_repertory_grid_constructs_agent;
ALTER TABLE repertory_grid_constructs ADD CONSTRAINT fk_repertory_grid_constructs_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);

-- KnowledgeConversions
ALTER TABLE knowledge_conversions DROP CONSTRAINT IF EXISTS fk_knowledge_conversions_elicitation_session;
ALTER TABLE knowledge_conversions ADD CONSTRAINT fk_knowledge_conversions_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE knowledge_conversions DROP CONSTRAINT IF EXISTS fk_knowledge_conversions_result_fragment;
ALTER TABLE knowledge_conversions ADD CONSTRAINT fk_knowledge_conversions_result_fragment
  FOREIGN KEY (result_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);

-- KnowledgeHoldings
ALTER TABLE knowledge_holdings DROP CONSTRAINT IF EXISTS fk_knowledge_holdings_procedure_version;
ALTER TABLE knowledge_holdings ADD CONSTRAINT fk_knowledge_holdings_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_holdings DROP CONSTRAINT IF EXISTS fk_knowledge_holdings_step;
ALTER TABLE knowledge_holdings ADD CONSTRAINT fk_knowledge_holdings_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE knowledge_holdings DROP CONSTRAINT IF EXISTS fk_knowledge_holdings_holder_agent;
ALTER TABLE knowledge_holdings ADD CONSTRAINT fk_knowledge_holdings_holder_agent
  FOREIGN KEY (holder_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_holdings DROP CONSTRAINT IF EXISTS fk_knowledge_holdings_formalized_as;
ALTER TABLE knowledge_holdings ADD CONSTRAINT fk_knowledge_holdings_formalized_as
  FOREIGN KEY (formalized_as) REFERENCES knowledge_fragments (knowledge_fragment_id);

-- FragmentCorroborations
ALTER TABLE fragment_corroborations DROP CONSTRAINT IF EXISTS fk_fragment_corroborations_knowledge_fragment;
ALTER TABLE fragment_corroborations ADD CONSTRAINT fk_fragment_corroborations_knowledge_fragment
  FOREIGN KEY (knowledge_fragment) REFERENCES knowledge_fragments (knowledge_fragment_id);
ALTER TABLE fragment_corroborations DROP CONSTRAINT IF EXISTS fk_fragment_corroborations_elicitation_session;
ALTER TABLE fragment_corroborations ADD CONSTRAINT fk_fragment_corroborations_elicitation_session
  FOREIGN KEY (elicitation_session) REFERENCES elicitation_sessions (elicitation_session_id);
ALTER TABLE fragment_corroborations DROP CONSTRAINT IF EXISTS fk_fragment_corroborations_agent;
ALTER TABLE fragment_corroborations ADD CONSTRAINT fk_fragment_corroborations_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);

-- KnowledgeTestOutcomes
ALTER TABLE knowledge_test_outcomes DROP CONSTRAINT IF EXISTS fk_knowledge_test_outcomes_procedure;
ALTER TABLE knowledge_test_outcomes ADD CONSTRAINT fk_knowledge_test_outcomes_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);

-- KnowHowCarriers
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_organization;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_procedure;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_community_of_practice;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_community_of_practice
  FOREIGN KEY (community_of_practice) REFERENCES communities_of_practice (community_of_practice_id);
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_holder_agent;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_holder_agent
  FOREIGN KEY (holder_agent) REFERENCES agents (agent_id);
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_holder_facility;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_holder_facility
  FOREIGN KEY (holder_facility) REFERENCES facilities (facility_id);
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_builds_on_know_how;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_builds_on_know_how
  FOREIGN KEY (builds_on_know_how) REFERENCES know_how_carriers (know_how_carrier_id);
ALTER TABLE know_how_carriers DROP CONSTRAINT IF EXISTS fk_know_how_carriers_evaluation_context;
ALTER TABLE know_how_carriers ADD CONSTRAINT fk_know_how_carriers_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- KnowledgeTransfers
ALTER TABLE knowledge_transfers DROP CONSTRAINT IF EXISTS fk_knowledge_transfers_know_how;
ALTER TABLE knowledge_transfers ADD CONSTRAINT fk_knowledge_transfers_know_how
  FOREIGN KEY (know_how) REFERENCES know_how_carriers (know_how_carrier_id);
ALTER TABLE knowledge_transfers DROP CONSTRAINT IF EXISTS fk_knowledge_transfers_from_agent;
ALTER TABLE knowledge_transfers ADD CONSTRAINT fk_knowledge_transfers_from_agent
  FOREIGN KEY (from_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_transfers DROP CONSTRAINT IF EXISTS fk_knowledge_transfers_recipient_agent;
ALTER TABLE knowledge_transfers ADD CONSTRAINT fk_knowledge_transfers_recipient_agent
  FOREIGN KEY (recipient_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_transfers DROP CONSTRAINT IF EXISTS fk_knowledge_transfers_community_of_practice;
ALTER TABLE knowledge_transfers ADD CONSTRAINT fk_knowledge_transfers_community_of_practice
  FOREIGN KEY (community_of_practice) REFERENCES communities_of_practice (community_of_practice_id);

-- KnowledgeRepositoryEntries
ALTER TABLE knowledge_repository_entries DROP CONSTRAINT IF EXISTS fk_knowledge_repository_entries_procedure;
ALTER TABLE knowledge_repository_entries ADD CONSTRAINT fk_knowledge_repository_entries_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE knowledge_repository_entries DROP CONSTRAINT IF EXISTS fk_knowledge_repository_entries_know_how;
ALTER TABLE knowledge_repository_entries ADD CONSTRAINT fk_knowledge_repository_entries_know_how
  FOREIGN KEY (know_how) REFERENCES know_how_carriers (know_how_carrier_id);
ALTER TABLE knowledge_repository_entries DROP CONSTRAINT IF EXISTS fk_knowledge_repository_entries_author_agent;
ALTER TABLE knowledge_repository_entries ADD CONSTRAINT fk_knowledge_repository_entries_author_agent
  FOREIGN KEY (author_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_repository_entries DROP CONSTRAINT IF EXISTS fk_knowledge_repository_entries_source_expert;
ALTER TABLE knowledge_repository_entries ADD CONSTRAINT fk_knowledge_repository_entries_source_expert
  FOREIGN KEY (source_expert) REFERENCES agents (agent_id);
ALTER TABLE knowledge_repository_entries DROP CONSTRAINT IF EXISTS fk_knowledge_repository_entries_fed_from_execution;
ALTER TABLE knowledge_repository_entries ADD CONSTRAINT fk_knowledge_repository_entries_fed_from_execution
  FOREIGN KEY (fed_from_execution) REFERENCES procedure_executions (procedure_execution_id);
ALTER TABLE knowledge_repository_entries DROP CONSTRAINT IF EXISTS fk_knowledge_repository_entries_evaluation_context;
ALTER TABLE knowledge_repository_entries ADD CONSTRAINT fk_knowledge_repository_entries_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- CommunityMemberships
ALTER TABLE community_memberships DROP CONSTRAINT IF EXISTS fk_community_memberships_community_of_practice;
ALTER TABLE community_memberships ADD CONSTRAINT fk_community_memberships_community_of_practice
  FOREIGN KEY (community_of_practice) REFERENCES communities_of_practice (community_of_practice_id);
ALTER TABLE community_memberships DROP CONSTRAINT IF EXISTS fk_community_memberships_agent;
ALTER TABLE community_memberships ADD CONSTRAINT fk_community_memberships_agent
  FOREIGN KEY (agent) REFERENCES agents (agent_id);

-- SourceRelationships
ALTER TABLE source_relationships DROP CONSTRAINT IF EXISTS fk_source_relationships_knowledge_engineer;
ALTER TABLE source_relationships ADD CONSTRAINT fk_source_relationships_knowledge_engineer
  FOREIGN KEY (knowledge_engineer) REFERENCES agents (agent_id);
ALTER TABLE source_relationships DROP CONSTRAINT IF EXISTS fk_source_relationships_source_agent;
ALTER TABLE source_relationships ADD CONSTRAINT fk_source_relationships_source_agent
  FOREIGN KEY (source_agent) REFERENCES agents (agent_id);
ALTER TABLE source_relationships DROP CONSTRAINT IF EXISTS fk_source_relationships_procedure;
ALTER TABLE source_relationships ADD CONSTRAINT fk_source_relationships_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);

-- DepartmentProcessAccounts
ALTER TABLE department_process_accounts DROP CONSTRAINT IF EXISTS fk_department_process_accounts_procedure;
ALTER TABLE department_process_accounts ADD CONSTRAINT fk_department_process_accounts_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE department_process_accounts DROP CONSTRAINT IF EXISTS fk_department_process_accounts_department;
ALTER TABLE department_process_accounts ADD CONSTRAINT fk_department_process_accounts_department
  FOREIGN KEY (department) REFERENCES organizations (organization_id);
ALTER TABLE department_process_accounts DROP CONSTRAINT IF EXISTS fk_department_process_accounts_stakeholder_agent;
ALTER TABLE department_process_accounts ADD CONSTRAINT fk_department_process_accounts_stakeholder_agent
  FOREIGN KEY (stakeholder_agent) REFERENCES agents (agent_id);
ALTER TABLE department_process_accounts DROP CONSTRAINT IF EXISTS fk_department_process_accounts_conflicts_with_account;
ALTER TABLE department_process_accounts ADD CONSTRAINT fk_department_process_accounts_conflicts_with_account
  FOREIGN KEY (conflicts_with_account) REFERENCES department_process_accounts (department_process_account_id);

-- ProblemOccurrences
ALTER TABLE problem_occurrences DROP CONSTRAINT IF EXISTS fk_problem_occurrences_procedure;
ALTER TABLE problem_occurrences ADD CONSTRAINT fk_problem_occurrences_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE problem_occurrences DROP CONSTRAINT IF EXISTS fk_problem_occurrences_solved_by_agent;
ALTER TABLE problem_occurrences ADD CONSTRAINT fk_problem_occurrences_solved_by_agent
  FOREIGN KEY (solved_by_agent) REFERENCES agents (agent_id);
ALTER TABLE problem_occurrences DROP CONSTRAINT IF EXISTS fk_problem_occurrences_solution_entry;
ALTER TABLE problem_occurrences ADD CONSTRAINT fk_problem_occurrences_solution_entry
  FOREIGN KEY (solution_entry) REFERENCES knowledge_repository_entries (knowledge_repository_entry_id);
ALTER TABLE problem_occurrences DROP CONSTRAINT IF EXISTS fk_problem_occurrences_prior_occurrence;
ALTER TABLE problem_occurrences ADD CONSTRAINT fk_problem_occurrences_prior_occurrence
  FOREIGN KEY (prior_occurrence) REFERENCES problem_occurrences (problem_occurrence_id);

-- OnboardingRecords
ALTER TABLE onboarding_records DROP CONSTRAINT IF EXISTS fk_onboarding_records_new_starter;
ALTER TABLE onboarding_records ADD CONSTRAINT fk_onboarding_records_new_starter
  FOREIGN KEY (new_starter) REFERENCES agents (agent_id);
ALTER TABLE onboarding_records DROP CONSTRAINT IF EXISTS fk_onboarding_records_procedure;
ALTER TABLE onboarding_records ADD CONSTRAINT fk_onboarding_records_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE onboarding_records DROP CONSTRAINT IF EXISTS fk_onboarding_records_evaluation_context;
ALTER TABLE onboarding_records ADD CONSTRAINT fk_onboarding_records_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- SharingRecognitions
ALTER TABLE sharing_recognitions DROP CONSTRAINT IF EXISTS fk_sharing_recognitions_recognized_agent;
ALTER TABLE sharing_recognitions ADD CONSTRAINT fk_sharing_recognitions_recognized_agent
  FOREIGN KEY (recognized_agent) REFERENCES agents (agent_id);
ALTER TABLE sharing_recognitions DROP CONSTRAINT IF EXISTS fk_sharing_recognitions_organization;
ALTER TABLE sharing_recognitions ADD CONSTRAINT fk_sharing_recognitions_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);

-- CapabilityDeclines
ALTER TABLE capability_declines DROP CONSTRAINT IF EXISTS fk_capability_declines_organization;
ALTER TABLE capability_declines ADD CONSTRAINT fk_capability_declines_organization
  FOREIGN KEY (organization) REFERENCES organizations (organization_id);
ALTER TABLE capability_declines DROP CONSTRAINT IF EXISTS fk_capability_declines_preceding_stage_decline;
ALTER TABLE capability_declines ADD CONSTRAINT fk_capability_declines_preceding_stage_decline
  FOREIGN KEY (preceding_stage_decline) REFERENCES capability_declines (capability_decline_id);

-- KnowledgeTraces
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_procedure_version;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_step;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_requirement;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_requirement
  FOREIGN KEY (requirement) REFERENCES requirements (requirement_id);
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_source_material;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_source_material
  FOREIGN KEY (source_material) REFERENCES collected_source_materials (collected_source_material_id);
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_derived_by_agent;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_derived_by_agent
  FOREIGN KEY (derived_by_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_validated_by_agent;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_validated_by_agent
  FOREIGN KEY (validated_by_agent) REFERENCES agents (agent_id);
ALTER TABLE knowledge_traces DROP CONSTRAINT IF EXISTS fk_knowledge_traces_contradicted_document;
ALTER TABLE knowledge_traces ADD CONSTRAINT fk_knowledge_traces_contradicted_document
  FOREIGN KEY (contradicted_document) REFERENCES resources (resource_id);

-- MinedFlowEdges
ALTER TABLE mined_flow_edges DROP CONSTRAINT IF EXISTS fk_mined_flow_edges_process_mining_run;
ALTER TABLE mined_flow_edges ADD CONSTRAINT fk_mined_flow_edges_process_mining_run
  FOREIGN KEY (process_mining_run) REFERENCES process_mining_runs (process_mining_run_id);
ALTER TABLE mined_flow_edges DROP CONSTRAINT IF EXISTS fk_mined_flow_edges_from_step;
ALTER TABLE mined_flow_edges ADD CONSTRAINT fk_mined_flow_edges_from_step
  FOREIGN KEY (from_step) REFERENCES steps (step_id);
ALTER TABLE mined_flow_edges DROP CONSTRAINT IF EXISTS fk_mined_flow_edges_to_step;
ALTER TABLE mined_flow_edges ADD CONSTRAINT fk_mined_flow_edges_to_step
  FOREIGN KEY (to_step) REFERENCES steps (step_id);
ALTER TABLE mined_flow_edges DROP CONSTRAINT IF EXISTS fk_mined_flow_edges_intent_decision_by;
ALTER TABLE mined_flow_edges ADD CONSTRAINT fk_mined_flow_edges_intent_decision_by
  FOREIGN KEY (intent_decision_by) REFERENCES agents (agent_id);

-- CollectionOccasions
ALTER TABLE collection_occasions DROP CONSTRAINT IF EXISTS fk_collection_occasions_procedure;
ALTER TABLE collection_occasions ADD CONSTRAINT fk_collection_occasions_procedure
  FOREIGN KEY (procedure) REFERENCES procedures (procedure_id);
ALTER TABLE collection_occasions DROP CONSTRAINT IF EXISTS fk_collection_occasions_evaluation_context;
ALTER TABLE collection_occasions ADD CONSTRAINT fk_collection_occasions_evaluation_context
  FOREIGN KEY (evaluation_context) REFERENCES evaluation_contexts (evaluation_context_id);

-- StakeholderPerspectives
ALTER TABLE stakeholder_perspectives DROP CONSTRAINT IF EXISTS fk_stakeholder_perspectives_procedure_version;
ALTER TABLE stakeholder_perspectives ADD CONSTRAINT fk_stakeholder_perspectives_procedure_version
  FOREIGN KEY (procedure_version) REFERENCES procedure_versions (procedure_version_id);
ALTER TABLE stakeholder_perspectives DROP CONSTRAINT IF EXISTS fk_stakeholder_perspectives_step;
ALTER TABLE stakeholder_perspectives ADD CONSTRAINT fk_stakeholder_perspectives_step
  FOREIGN KEY (step) REFERENCES steps (step_id);
ALTER TABLE stakeholder_perspectives DROP CONSTRAINT IF EXISTS fk_stakeholder_perspectives_holder_role;
ALTER TABLE stakeholder_perspectives ADD CONSTRAINT fk_stakeholder_perspectives_holder_role
  FOREIGN KEY (holder_role) REFERENCES roles (role_id);
ALTER TABLE stakeholder_perspectives DROP CONSTRAINT IF EXISTS fk_stakeholder_perspectives_source_material;
ALTER TABLE stakeholder_perspectives ADD CONSTRAINT fk_stakeholder_perspectives_source_material
  FOREIGN KEY (source_material) REFERENCES collected_source_materials (collected_source_material_id);
ALTER TABLE stakeholder_perspectives DROP CONSTRAINT IF EXISTS fk_stakeholder_perspectives_conflicts_with_perspective;
ALTER TABLE stakeholder_perspectives ADD CONSTRAINT fk_stakeholder_perspectives_conflicts_with_perspective
  FOREIGN KEY (conflicts_with_perspective) REFERENCES stakeholder_perspectives (stakeholder_perspective_id);

-- ModelPilots
ALTER TABLE model_pilots DROP CONSTRAINT IF EXISTS fk_model_pilots_governed_model;
ALTER TABLE model_pilots ADD CONSTRAINT fk_model_pilots_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);

-- ModelActivityExperts
ALTER TABLE model_activity_experts DROP CONSTRAINT IF EXISTS fk_model_activity_experts_governed_model;
ALTER TABLE model_activity_experts ADD CONSTRAINT fk_model_activity_experts_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);
ALTER TABLE model_activity_experts DROP CONSTRAINT IF EXISTS fk_model_activity_experts_expert;
ALTER TABLE model_activity_experts ADD CONSTRAINT fk_model_activity_experts_expert
  FOREIGN KEY (expert) REFERENCES agents (agent_id);

-- ModelDataMappingRuns
ALTER TABLE model_data_mapping_runs DROP CONSTRAINT IF EXISTS fk_model_data_mapping_runs_governed_model;
ALTER TABLE model_data_mapping_runs ADD CONSTRAINT fk_model_data_mapping_runs_governed_model
  FOREIGN KEY (governed_model) REFERENCES governed_models (governed_model_id);

-- ArtifactHandoffs
ALTER TABLE artifact_handoffs DROP CONSTRAINT IF EXISTS fk_artifact_handoffs_step_variable;
ALTER TABLE artifact_handoffs ADD CONSTRAINT fk_artifact_handoffs_step_variable
  FOREIGN KEY (step_variable) REFERENCES step_variables (step_variable_id);
ALTER TABLE artifact_handoffs DROP CONSTRAINT IF EXISTS fk_artifact_handoffs_from_step;
ALTER TABLE artifact_handoffs ADD CONSTRAINT fk_artifact_handoffs_from_step
  FOREIGN KEY (from_step) REFERENCES steps (step_id);
ALTER TABLE artifact_handoffs DROP CONSTRAINT IF EXISTS fk_artifact_handoffs_to_step;
ALTER TABLE artifact_handoffs ADD CONSTRAINT fk_artifact_handoffs_to_step
  FOREIGN KEY (to_step) REFERENCES steps (step_id);

-- 727 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
