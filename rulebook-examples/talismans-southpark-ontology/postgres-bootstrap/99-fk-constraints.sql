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

-- FiringKinds
ALTER TABLE firing_kinds DROP CONSTRAINT IF EXISTS fk_firing_kinds_rule;
ALTER TABLE firing_kinds ADD CONSTRAINT fk_firing_kinds_rule
  FOREIGN KEY (rule) REFERENCES rules (rule_id);

-- Things
ALTER TABLE things DROP CONSTRAINT IF EXISTS fk_things_portrait;
ALTER TABLE things ADD CONSTRAINT fk_things_portrait
  FOREIGN KEY (portrait) REFERENCES portraits (portrait_id);

-- PortraitAliases
ALTER TABLE portrait_aliases DROP CONSTRAINT IF EXISTS fk_portrait_aliases_portrait;
ALTER TABLE portrait_aliases ADD CONSTRAINT fk_portrait_aliases_portrait
  FOREIGN KEY (portrait) REFERENCES portraits (portrait_id);

-- Properties
ALTER TABLE properties DROP CONSTRAINT IF EXISTS fk_properties_domain_class;
ALTER TABLE properties ADD CONSTRAINT fk_properties_domain_class
  FOREIGN KEY (domain_class) REFERENCES things (thing_id);
ALTER TABLE properties DROP CONSTRAINT IF EXISTS fk_properties_range_class;
ALTER TABLE properties ADD CONSTRAINT fk_properties_range_class
  FOREIGN KEY (range_class) REFERENCES things (thing_id);

-- PropertyAxioms
ALTER TABLE property_axioms DROP CONSTRAINT IF EXISTS fk_property_axioms_kind;
ALTER TABLE property_axioms ADD CONSTRAINT fk_property_axioms_kind
  FOREIGN KEY (kind) REFERENCES firing_kinds (firing_kind_id);
ALTER TABLE property_axioms DROP CONSTRAINT IF EXISTS fk_property_axioms_property;
ALTER TABLE property_axioms ADD CONSTRAINT fk_property_axioms_property
  FOREIGN KEY (property) REFERENCES properties (property_id);
ALTER TABLE property_axioms DROP CONSTRAINT IF EXISTS fk_property_axioms_first;
ALTER TABLE property_axioms ADD CONSTRAINT fk_property_axioms_first
  FOREIGN KEY (first) REFERENCES properties (property_id);
ALTER TABLE property_axioms DROP CONSTRAINT IF EXISTS fk_property_axioms_second;
ALTER TABLE property_axioms ADD CONSTRAINT fk_property_axioms_second
  FOREIGN KEY (second) REFERENCES properties (property_id);

-- Statements
ALTER TABLE statements DROP CONSTRAINT IF EXISTS fk_statements_graph;
ALTER TABLE statements ADD CONSTRAINT fk_statements_graph
  FOREIGN KEY (graph) REFERENCES graphs (graph_id);
ALTER TABLE statements DROP CONSTRAINT IF EXISTS fk_statements_subject;
ALTER TABLE statements ADD CONSTRAINT fk_statements_subject
  FOREIGN KEY (subject) REFERENCES things (thing_id);
ALTER TABLE statements DROP CONSTRAINT IF EXISTS fk_statements_property;
ALTER TABLE statements ADD CONSTRAINT fk_statements_property
  FOREIGN KEY (property) REFERENCES properties (property_id);
ALTER TABLE statements DROP CONSTRAINT IF EXISTS fk_statements_object_thing;
ALTER TABLE statements ADD CONSTRAINT fk_statements_object_thing
  FOREIGN KEY (object_thing) REFERENCES things (thing_id);

-- Firings
ALTER TABLE firings DROP CONSTRAINT IF EXISTS fk_firings_kind;
ALTER TABLE firings ADD CONSTRAINT fk_firings_kind
  FOREIGN KEY (kind) REFERENCES firing_kinds (firing_kind_id);
ALTER TABLE firings DROP CONSTRAINT IF EXISTS fk_firings_rule;
ALTER TABLE firings ADD CONSTRAINT fk_firings_rule
  FOREIGN KEY (rule) REFERENCES rules (rule_id);
ALTER TABLE firings DROP CONSTRAINT IF EXISTS fk_firings_target;
ALTER TABLE firings ADD CONSTRAINT fk_firings_target
  FOREIGN KEY (target) REFERENCES statements (statement_id);
ALTER TABLE firings DROP CONSTRAINT IF EXISTS fk_firings_first;
ALTER TABLE firings ADD CONSTRAINT fk_firings_first
  FOREIGN KEY (first) REFERENCES statements (statement_id);
ALTER TABLE firings DROP CONSTRAINT IF EXISTS fk_firings_second;
ALTER TABLE firings ADD CONSTRAINT fk_firings_second
  FOREIGN KEY (second) REFERENCES statements (statement_id);

-- DisjointChecks
ALTER TABLE disjoint_checks DROP CONSTRAINT IF EXISTS fk_disjoint_checks_graph;
ALTER TABLE disjoint_checks ADD CONSTRAINT fk_disjoint_checks_graph
  FOREIGN KEY (graph) REFERENCES graphs (graph_id);
ALTER TABLE disjoint_checks DROP CONSTRAINT IF EXISTS fk_disjoint_checks_rule;
ALTER TABLE disjoint_checks ADD CONSTRAINT fk_disjoint_checks_rule
  FOREIGN KEY (rule) REFERENCES rules (rule_id);
ALTER TABLE disjoint_checks DROP CONSTRAINT IF EXISTS fk_disjoint_checks_first;
ALTER TABLE disjoint_checks ADD CONSTRAINT fk_disjoint_checks_first
  FOREIGN KEY (first) REFERENCES statements (statement_id);
ALTER TABLE disjoint_checks DROP CONSTRAINT IF EXISTS fk_disjoint_checks_second;
ALTER TABLE disjoint_checks ADD CONSTRAINT fk_disjoint_checks_second
  FOREIGN KEY (second) REFERENCES statements (statement_id);

-- Scenarios
ALTER TABLE scenarios DROP CONSTRAINT IF EXISTS fk_scenarios_graph;
ALTER TABLE scenarios ADD CONSTRAINT fk_scenarios_graph
  FOREIGN KEY (graph) REFERENCES graphs (graph_id);

-- ScenarioFactChanges
ALTER TABLE scenario_fact_changes DROP CONSTRAINT IF EXISTS fk_scenario_fact_changes_scenario;
ALTER TABLE scenario_fact_changes ADD CONSTRAINT fk_scenario_fact_changes_scenario
  FOREIGN KEY (scenario) REFERENCES scenarios (scenario_id);
ALTER TABLE scenario_fact_changes DROP CONSTRAINT IF EXISTS fk_scenario_fact_changes_subject;
ALTER TABLE scenario_fact_changes ADD CONSTRAINT fk_scenario_fact_changes_subject
  FOREIGN KEY (subject) REFERENCES things (thing_id);
ALTER TABLE scenario_fact_changes DROP CONSTRAINT IF EXISTS fk_scenario_fact_changes_property;
ALTER TABLE scenario_fact_changes ADD CONSTRAINT fk_scenario_fact_changes_property
  FOREIGN KEY (property) REFERENCES properties (property_id);

-- ScenarioRuleSettings
ALTER TABLE scenario_rule_settings DROP CONSTRAINT IF EXISTS fk_scenario_rule_settings_scenario;
ALTER TABLE scenario_rule_settings ADD CONSTRAINT fk_scenario_rule_settings_scenario
  FOREIGN KEY (scenario) REFERENCES scenarios (scenario_id);
ALTER TABLE scenario_rule_settings DROP CONSTRAINT IF EXISTS fk_scenario_rule_settings_rule;
ALTER TABLE scenario_rule_settings ADD CONSTRAINT fk_scenario_rule_settings_rule
  FOREIGN KEY (rule) REFERENCES rules (rule_id);

-- 28 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
