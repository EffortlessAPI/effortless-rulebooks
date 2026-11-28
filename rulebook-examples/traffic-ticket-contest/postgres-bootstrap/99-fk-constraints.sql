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

-- BusinessRules
ALTER TABLE business_rules DROP CONSTRAINT IF EXISTS fk_business_rules_category;
ALTER TABLE business_rules ADD CONSTRAINT fk_business_rules_category
  FOREIGN KEY (category) REFERENCES business_rule_categories (business_rule_category_id);

-- GlossaryTerms
ALTER TABLE glossary_terms DROP CONSTRAINT IF EXISTS fk_glossary_terms_category;
ALTER TABLE glossary_terms ADD CONSTRAINT fk_glossary_terms_category
  FOREIGN KEY (category) REFERENCES glossary_categories (glossary_category_id);

-- AuditLogEntries
ALTER TABLE audit_log_entries DROP CONSTRAINT IF EXISTS fk_audit_log_entries_citation;
ALTER TABLE audit_log_entries ADD CONSTRAINT fk_audit_log_entries_citation
  FOREIGN KEY (citation) REFERENCES citations (citation_id);
ALTER TABLE audit_log_entries DROP CONSTRAINT IF EXISTS fk_audit_log_entries_actor;
ALTER TABLE audit_log_entries ADD CONSTRAINT fk_audit_log_entries_actor
  FOREIGN KEY (actor) REFERENCES app_users (app_user_id);

-- PlatformNaviation
ALTER TABLE platform_naviation DROP CONSTRAINT IF EXISTS fk_platform_naviation_erb_package;
ALTER TABLE platform_naviation ADD CONSTRAINT fk_platform_naviation_erb_package
  FOREIGN KEY (erb_package) REFERENCES erb_packages (erb_package_id);

-- Jurisdictions
ALTER TABLE jurisdictions DROP CONSTRAINT IF EXISTS fk_jurisdictions_parent_jurisdiction;
ALTER TABLE jurisdictions ADD CONSTRAINT fk_jurisdictions_parent_jurisdiction
  FOREIGN KEY (parent_jurisdiction) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE jurisdictions DROP CONSTRAINT IF EXISTS fk_jurisdictions_child_jurisdictions;
ALTER TABLE jurisdictions ADD CONSTRAINT fk_jurisdictions_child_jurisdictions
  FOREIGN KEY (child_jurisdictions) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE jurisdictions DROP CONSTRAINT IF EXISTS fk_jurisdictions_jurisdiction_rules;
ALTER TABLE jurisdictions ADD CONSTRAINT fk_jurisdictions_jurisdiction_rules
  FOREIGN KEY (jurisdiction_rules) REFERENCES jurisdiction_rules (jurisdiction_rule_id);
ALTER TABLE jurisdictions DROP CONSTRAINT IF EXISTS fk_jurisdictions_source_documents;
ALTER TABLE jurisdictions ADD CONSTRAINT fk_jurisdictions_source_documents
  FOREIGN KEY (source_documents) REFERENCES jurisdiction_source_documents (jurisdiction_source_document_id);

-- JurisdictionSourceDocuments
ALTER TABLE jurisdiction_source_documents DROP CONSTRAINT IF EXISTS fk_jurisdiction_source_documents_jurisdiction;
ALTER TABLE jurisdiction_source_documents ADD CONSTRAINT fk_jurisdiction_source_documents_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE jurisdiction_source_documents DROP CONSTRAINT IF EXISTS fk_jurisdiction_source_documents_jurisdiction_rules;
ALTER TABLE jurisdiction_source_documents ADD CONSTRAINT fk_jurisdiction_source_documents_jurisdiction_rules
  FOREIGN KEY (jurisdiction_rules) REFERENCES jurisdiction_rules (jurisdiction_rule_id);

-- JurisdictionRules
ALTER TABLE jurisdiction_rules DROP CONSTRAINT IF EXISTS fk_jurisdiction_rules_route;
ALTER TABLE jurisdiction_rules ADD CONSTRAINT fk_jurisdiction_rules_route
  FOREIGN KEY (route) REFERENCES platform_naviation (platform_naviation_id);
ALTER TABLE jurisdiction_rules DROP CONSTRAINT IF EXISTS fk_jurisdiction_rules_source_document;
ALTER TABLE jurisdiction_rules ADD CONSTRAINT fk_jurisdiction_rules_source_document
  FOREIGN KEY (source_document) REFERENCES jurisdiction_source_documents (jurisdiction_source_document_id);
ALTER TABLE jurisdiction_rules DROP CONSTRAINT IF EXISTS fk_jurisdiction_rules_jurisdiction;
ALTER TABLE jurisdiction_rules ADD CONSTRAINT fk_jurisdiction_rules_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);

-- AppUsers
ALTER TABLE app_users DROP CONSTRAINT IF EXISTS fk_app_users_role;
ALTER TABLE app_users ADD CONSTRAINT fk_app_users_role
  FOREIGN KEY (role) REFERENCES roles (role_id);

-- ReferenceDocuments
ALTER TABLE reference_documents DROP CONSTRAINT IF EXISTS fk_reference_documents_jurisdiction;
ALTER TABLE reference_documents ADD CONSTRAINT fk_reference_documents_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);

-- StateMachines
ALTER TABLE state_machines DROP CONSTRAINT IF EXISTS fk_state_machines_erb_package;
ALTER TABLE state_machines ADD CONSTRAINT fk_state_machines_erb_package
  FOREIGN KEY (erb_package) REFERENCES erb_packages (erb_package_id);

-- MachineStates
ALTER TABLE machine_states DROP CONSTRAINT IF EXISTS fk_machine_states_state_machine;
ALTER TABLE machine_states ADD CONSTRAINT fk_machine_states_state_machine
  FOREIGN KEY (state_machine) REFERENCES state_machines (state_machine_id);

-- StateTransitionRules
ALTER TABLE state_transition_rules DROP CONSTRAINT IF EXISTS fk_state_transition_rules_state_machine;
ALTER TABLE state_transition_rules ADD CONSTRAINT fk_state_transition_rules_state_machine
  FOREIGN KEY (state_machine) REFERENCES state_machines (state_machine_id);
ALTER TABLE state_transition_rules DROP CONSTRAINT IF EXISTS fk_state_transition_rules_from_state;
ALTER TABLE state_transition_rules ADD CONSTRAINT fk_state_transition_rules_from_state
  FOREIGN KEY (from_state) REFERENCES machine_states (machine_state_id);
ALTER TABLE state_transition_rules DROP CONSTRAINT IF EXISTS fk_state_transition_rules_to_state;
ALTER TABLE state_transition_rules ADD CONSTRAINT fk_state_transition_rules_to_state
  FOREIGN KEY (to_state) REFERENCES machine_states (machine_state_id);

-- StateTransitions
ALTER TABLE state_transitions DROP CONSTRAINT IF EXISTS fk_state_transitions_state_machine;
ALTER TABLE state_transitions ADD CONSTRAINT fk_state_transitions_state_machine
  FOREIGN KEY (state_machine) REFERENCES state_machines (state_machine_id);
ALTER TABLE state_transitions DROP CONSTRAINT IF EXISTS fk_state_transitions_triggered_by;
ALTER TABLE state_transitions ADD CONSTRAINT fk_state_transitions_triggered_by
  FOREIGN KEY (triggered_by) REFERENCES app_users (app_user_id);

-- WorkQueueItems
ALTER TABLE work_queue_items DROP CONSTRAINT IF EXISTS fk_work_queue_items_assigned_to;
ALTER TABLE work_queue_items ADD CONSTRAINT fk_work_queue_items_assigned_to
  FOREIGN KEY (assigned_to) REFERENCES app_users (app_user_id);

-- ModelPricingVersions
ALTER TABLE model_pricing_versions DROP CONSTRAINT IF EXISTS fk_model_pricing_versions_ai_model;
ALTER TABLE model_pricing_versions ADD CONSTRAINT fk_model_pricing_versions_ai_model
  FOREIGN KEY (ai_model) REFERENCES ai_models (ai_model_id);

-- AssistantTurns
ALTER TABLE assistant_turns DROP CONSTRAINT IF EXISTS fk_assistant_turns_citation;
ALTER TABLE assistant_turns ADD CONSTRAINT fk_assistant_turns_citation
  FOREIGN KEY (citation) REFERENCES citations (citation_id);
ALTER TABLE assistant_turns DROP CONSTRAINT IF EXISTS fk_assistant_turns_ai_model;
ALTER TABLE assistant_turns ADD CONSTRAINT fk_assistant_turns_ai_model
  FOREIGN KEY (ai_model) REFERENCES ai_models (ai_model_id);
ALTER TABLE assistant_turns DROP CONSTRAINT IF EXISTS fk_assistant_turns_model_pricing_version;
ALTER TABLE assistant_turns ADD CONSTRAINT fk_assistant_turns_model_pricing_version
  FOREIGN KEY (model_pricing_version) REFERENCES model_pricing_versions (model_pricing_version_id);

-- ERBPackages
ALTER TABLE erb_packages DROP CONSTRAINT IF EXISTS fk_erb_packages_erb_features;
ALTER TABLE erb_packages ADD CONSTRAINT fk_erb_packages_erb_features
  FOREIGN KEY (erb_features) REFERENCES erb_features (erb_feature_id);

-- ERBFeatures
ALTER TABLE erb_features DROP CONSTRAINT IF EXISTS fk_erb_features_route;
ALTER TABLE erb_features ADD CONSTRAINT fk_erb_features_route
  FOREIGN KEY (route) REFERENCES platform_naviation (platform_naviation_id);
ALTER TABLE erb_features DROP CONSTRAINT IF EXISTS fk_erb_features_erb_package;
ALTER TABLE erb_features ADD CONSTRAINT fk_erb_features_erb_package
  FOREIGN KEY (erb_package) REFERENCES erb_packages (erb_package_id);
ALTER TABLE erb_features DROP CONSTRAINT IF EXISTS fk_erb_features_category;
ALTER TABLE erb_features ADD CONSTRAINT fk_erb_features_category
  FOREIGN KEY (category) REFERENCES erb_feature_categories (erb_feature_category_id);
ALTER TABLE erb_features DROP CONSTRAINT IF EXISTS fk_erb_features_status;
ALTER TABLE erb_features ADD CONSTRAINT fk_erb_features_status
  FOREIGN KEY (status) REFERENCES erb_feature_statuses (erb_feature_status_id);

-- ERBTables
ALTER TABLE erb_tables DROP CONSTRAINT IF EXISTS fk_erb_tables_erb_package;
ALTER TABLE erb_tables ADD CONSTRAINT fk_erb_tables_erb_package
  FOREIGN KEY (erb_package) REFERENCES erb_packages (erb_package_id);
ALTER TABLE erb_tables DROP CONSTRAINT IF EXISTS fk_erb_tables_platform;
ALTER TABLE erb_tables ADD CONSTRAINT fk_erb_tables_platform
  FOREIGN KEY (platform) REFERENCES platforms (platform_id);

-- ERBFields
ALTER TABLE erb_fields DROP CONSTRAINT IF EXISTS fk_erb_fields_erb_table;
ALTER TABLE erb_fields ADD CONSTRAINT fk_erb_fields_erb_table
  FOREIGN KEY (erb_table) REFERENCES erb_tables (erb_table_id);

-- SubjectStateInstances
ALTER TABLE subject_state_instances DROP CONSTRAINT IF EXISTS fk_subject_state_instances_state_machine;
ALTER TABLE subject_state_instances ADD CONSTRAINT fk_subject_state_instances_state_machine
  FOREIGN KEY (state_machine) REFERENCES state_machines (state_machine_id);
ALTER TABLE subject_state_instances DROP CONSTRAINT IF EXISTS fk_subject_state_instances_prior_instance;
ALTER TABLE subject_state_instances ADD CONSTRAINT fk_subject_state_instances_prior_instance
  FOREIGN KEY (prior_instance) REFERENCES subject_state_instances (subject_state_instance_id);
ALTER TABLE subject_state_instances DROP CONSTRAINT IF EXISTS fk_subject_state_instances_entered_via_transition;
ALTER TABLE subject_state_instances ADD CONSTRAINT fk_subject_state_instances_entered_via_transition
  FOREIGN KEY (entered_via_transition) REFERENCES state_transitions (state_transition_id);

-- ViolationTypes
ALTER TABLE violation_types DROP CONSTRAINT IF EXISTS fk_violation_types_jurisdiction;
ALTER TABLE violation_types ADD CONSTRAINT fk_violation_types_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE violation_types DROP CONSTRAINT IF EXISTS fk_violation_types_citations;
ALTER TABLE violation_types ADD CONSTRAINT fk_violation_types_citations
  FOREIGN KEY (citations) REFERENCES citations (citation_id);

-- Drivers
ALTER TABLE drivers DROP CONSTRAINT IF EXISTS fk_drivers_home_jurisdiction;
ALTER TABLE drivers ADD CONSTRAINT fk_drivers_home_jurisdiction
  FOREIGN KEY (home_jurisdiction) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE drivers DROP CONSTRAINT IF EXISTS fk_drivers_citations;
ALTER TABLE drivers ADD CONSTRAINT fk_drivers_citations
  FOREIGN KEY (citations) REFERENCES citations (citation_id);

-- Citations
ALTER TABLE citations DROP CONSTRAINT IF EXISTS fk_citations_driver;
ALTER TABLE citations ADD CONSTRAINT fk_citations_driver
  FOREIGN KEY (driver) REFERENCES drivers (driver_id);
ALTER TABLE citations DROP CONSTRAINT IF EXISTS fk_citations_violation_type;
ALTER TABLE citations ADD CONSTRAINT fk_citations_violation_type
  FOREIGN KEY (violation_type) REFERENCES violation_types (violation_type_id);
ALTER TABLE citations DROP CONSTRAINT IF EXISTS fk_citations_jurisdiction;
ALTER TABLE citations ADD CONSTRAINT fk_citations_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE citations DROP CONSTRAINT IF EXISTS fk_citations_hearings;
ALTER TABLE citations ADD CONSTRAINT fk_citations_hearings
  FOREIGN KEY (hearings) REFERENCES hearings (hearing_id);
ALTER TABLE citations DROP CONSTRAINT IF EXISTS fk_citations_payments;
ALTER TABLE citations ADD CONSTRAINT fk_citations_payments
  FOREIGN KEY (payments) REFERENCES payments (payment_id);
ALTER TABLE citations DROP CONSTRAINT IF EXISTS fk_citations_case_events;
ALTER TABLE citations ADD CONSTRAINT fk_citations_case_events
  FOREIGN KEY (case_events) REFERENCES case_events (case_event_id);

-- Hearings
ALTER TABLE hearings DROP CONSTRAINT IF EXISTS fk_hearings_citation;
ALTER TABLE hearings ADD CONSTRAINT fk_hearings_citation
  FOREIGN KEY (citation) REFERENCES citations (citation_id);

-- Payments
ALTER TABLE payments DROP CONSTRAINT IF EXISTS fk_payments_citation;
ALTER TABLE payments ADD CONSTRAINT fk_payments_citation
  FOREIGN KEY (citation) REFERENCES citations (citation_id);

-- CaseEvents
ALTER TABLE case_events DROP CONSTRAINT IF EXISTS fk_case_events_citation;
ALTER TABLE case_events ADD CONSTRAINT fk_case_events_citation
  FOREIGN KEY (citation) REFERENCES citations (citation_id);

-- TestCase
ALTER TABLE test_case DROP CONSTRAINT IF EXISTS fk_test_case_test_category;
ALTER TABLE test_case ADD CONSTRAINT fk_test_case_test_category
  FOREIGN KEY (test_category) REFERENCES test_category (test_category_id);
ALTER TABLE test_case DROP CONSTRAINT IF EXISTS fk_test_case_test_surface;
ALTER TABLE test_case ADD CONSTRAINT fk_test_case_test_surface
  FOREIGN KEY (test_surface) REFERENCES test_surface (test_surface_id);
ALTER TABLE test_case DROP CONSTRAINT IF EXISTS fk_test_case_test_technology;
ALTER TABLE test_case ADD CONSTRAINT fk_test_case_test_technology
  FOREIGN KEY (test_technology) REFERENCES test_technology (test_technology_id);

-- TestExpectation
ALTER TABLE test_expectation DROP CONSTRAINT IF EXISTS fk_test_expectation_test_case;
ALTER TABLE test_expectation ADD CONSTRAINT fk_test_expectation_test_case
  FOREIGN KEY (test_case) REFERENCES test_case (test_case_id);

-- TestResult
ALTER TABLE test_result DROP CONSTRAINT IF EXISTS fk_test_result_test_run;
ALTER TABLE test_result ADD CONSTRAINT fk_test_result_test_run
  FOREIGN KEY (test_run) REFERENCES test_run (test_run_id);
ALTER TABLE test_result DROP CONSTRAINT IF EXISTS fk_test_result_test_case;
ALTER TABLE test_result ADD CONSTRAINT fk_test_result_test_case
  FOREIGN KEY (test_case) REFERENCES test_case (test_case_id);

-- TestResultAssertion
ALTER TABLE test_result_assertion DROP CONSTRAINT IF EXISTS fk_test_result_assertion_test_result;
ALTER TABLE test_result_assertion ADD CONSTRAINT fk_test_result_assertion_test_result
  FOREIGN KEY (test_result) REFERENCES test_result (test_result_id);

-- ScreenLayouts
ALTER TABLE screen_layouts DROP CONSTRAINT IF EXISTS fk_screen_layouts_nav;
ALTER TABLE screen_layouts ADD CONSTRAINT fk_screen_layouts_nav
  FOREIGN KEY (nav) REFERENCES platform_naviation (platform_naviation_id);

-- ScreenSections
ALTER TABLE screen_sections DROP CONSTRAINT IF EXISTS fk_screen_sections_screen_layout;
ALTER TABLE screen_sections ADD CONSTRAINT fk_screen_sections_screen_layout
  FOREIGN KEY (screen_layout) REFERENCES screen_layouts (screen_layout_id);

-- FieldDisplayHints
ALTER TABLE field_display_hints DROP CONSTRAINT IF EXISTS fk_field_display_hints_erb_table;
ALTER TABLE field_display_hints ADD CONSTRAINT fk_field_display_hints_erb_table
  FOREIGN KEY (erb_table) REFERENCES erb_tables (erb_table_id);

-- FeeSchedules
ALTER TABLE fee_schedules DROP CONSTRAINT IF EXISTS fk_fee_schedules_jurisdiction;
ALTER TABLE fee_schedules ADD CONSTRAINT fk_fee_schedules_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);
ALTER TABLE fee_schedules DROP CONSTRAINT IF EXISTS fk_fee_schedules_violation_type;
ALTER TABLE fee_schedules ADD CONSTRAINT fk_fee_schedules_violation_type
  FOREIGN KEY (violation_type) REFERENCES violation_types (violation_type_id);

-- DeadlineRules
ALTER TABLE deadline_rules DROP CONSTRAINT IF EXISTS fk_deadline_rules_jurisdiction;
ALTER TABLE deadline_rules ADD CONSTRAINT fk_deadline_rules_jurisdiction
  FOREIGN KEY (jurisdiction) REFERENCES jurisdictions (jurisdiction_id);

-- ContestGrounds
ALTER TABLE contest_grounds DROP CONSTRAINT IF EXISTS fk_contest_grounds_violation_type;
ALTER TABLE contest_grounds ADD CONSTRAINT fk_contest_grounds_violation_type
  FOREIGN KEY (violation_type) REFERENCES violation_types (violation_type_id);

-- DriverLicensePoints
ALTER TABLE driver_license_points DROP CONSTRAINT IF EXISTS fk_driver_license_points_driver;
ALTER TABLE driver_license_points ADD CONSTRAINT fk_driver_license_points_driver
  FOREIGN KEY (driver) REFERENCES drivers (driver_id);
ALTER TABLE driver_license_points DROP CONSTRAINT IF EXISTS fk_driver_license_points_citation;
ALTER TABLE driver_license_points ADD CONSTRAINT fk_driver_license_points_citation
  FOREIGN KEY (citation) REFERENCES citations (citation_id);

-- 68 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
