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

-- EDBRelations
ALTER TABLE edb_relations DROP CONSTRAINT IF EXISTS fk_edb_relations_seed_facts;
ALTER TABLE edb_relations ADD CONSTRAINT fk_edb_relations_seed_facts
  FOREIGN KEY (seed_facts) REFERENCES seed_facts (seed_fact_id);
ALTER TABLE edb_relations DROP CONSTRAINT IF EXISTS fk_edb_relations_transition_effects;
ALTER TABLE edb_relations ADD CONSTRAINT fk_edb_relations_transition_effects
  FOREIGN KEY (transition_effects) REFERENCES transition_effects (effect_id);

-- Commands
ALTER TABLE commands DROP CONSTRAINT IF EXISTS fk_commands_transitions;
ALTER TABLE commands ADD CONSTRAINT fk_commands_transitions
  FOREIGN KEY (transitions) REFERENCES transitions (transition_id);

-- Rules
ALTER TABLE rules DROP CONSTRAINT IF EXISTS fk_rules_guarded_transitions;
ALTER TABLE rules ADD CONSTRAINT fk_rules_guarded_transitions
  FOREIGN KEY (guarded_transitions) REFERENCES transitions (transition_id);
ALTER TABLE rules DROP CONSTRAINT IF EXISTS fk_rules_transition_effects;
ALTER TABLE rules ADD CONSTRAINT fk_rules_transition_effects
  FOREIGN KEY (transition_effects) REFERENCES transition_effects (effect_id);
ALTER TABLE rules DROP CONSTRAINT IF EXISTS fk_rules_transition_events;
ALTER TABLE rules ADD CONSTRAINT fk_rules_transition_events
  FOREIGN KEY (transition_events) REFERENCES transition_events (event_id);
ALTER TABLE rules DROP CONSTRAINT IF EXISTS fk_rules_integrity_constraints;
ALTER TABLE rules ADD CONSTRAINT fk_rules_integrity_constraints
  FOREIGN KEY (integrity_constraints) REFERENCES integrity_constraints (constraint_id);

-- Transitions
ALTER TABLE transitions DROP CONSTRAINT IF EXISTS fk_transitions_command;
ALTER TABLE transitions ADD CONSTRAINT fk_transitions_command
  FOREIGN KEY (command) REFERENCES commands (command_id);
ALTER TABLE transitions DROP CONSTRAINT IF EXISTS fk_transitions_guard;
ALTER TABLE transitions ADD CONSTRAINT fk_transitions_guard
  FOREIGN KEY (guard) REFERENCES rules (rule_id);
ALTER TABLE transitions DROP CONSTRAINT IF EXISTS fk_transitions_effects;
ALTER TABLE transitions ADD CONSTRAINT fk_transitions_effects
  FOREIGN KEY (effects) REFERENCES transition_effects (effect_id);
ALTER TABLE transitions DROP CONSTRAINT IF EXISTS fk_transitions_events;
ALTER TABLE transitions ADD CONSTRAINT fk_transitions_events
  FOREIGN KEY (events) REFERENCES transition_events (event_id);

-- TransitionEffects
ALTER TABLE transition_effects DROP CONSTRAINT IF EXISTS fk_transition_effects_transition;
ALTER TABLE transition_effects ADD CONSTRAINT fk_transition_effects_transition
  FOREIGN KEY (transition) REFERENCES transitions (transition_id);
ALTER TABLE transition_effects DROP CONSTRAINT IF EXISTS fk_transition_effects_relation;
ALTER TABLE transition_effects ADD CONSTRAINT fk_transition_effects_relation
  FOREIGN KEY (relation) REFERENCES edb_relations (relation_id);
ALTER TABLE transition_effects DROP CONSTRAINT IF EXISTS fk_transition_effects_from_rule;
ALTER TABLE transition_effects ADD CONSTRAINT fk_transition_effects_from_rule
  FOREIGN KEY (from_rule) REFERENCES rules (rule_id);

-- TransitionEvents
ALTER TABLE transition_events DROP CONSTRAINT IF EXISTS fk_transition_events_transition;
ALTER TABLE transition_events ADD CONSTRAINT fk_transition_events_transition
  FOREIGN KEY (transition) REFERENCES transitions (transition_id);
ALTER TABLE transition_events DROP CONSTRAINT IF EXISTS fk_transition_events_from_rule;
ALTER TABLE transition_events ADD CONSTRAINT fk_transition_events_from_rule
  FOREIGN KEY (from_rule) REFERENCES rules (rule_id);

-- IntegrityConstraints
ALTER TABLE integrity_constraints DROP CONSTRAINT IF EXISTS fk_integrity_constraints_rule;
ALTER TABLE integrity_constraints ADD CONSTRAINT fk_integrity_constraints_rule
  FOREIGN KEY (rule) REFERENCES rules (rule_id);

-- SeedFacts
ALTER TABLE seed_facts DROP CONSTRAINT IF EXISTS fk_seed_facts_relation;
ALTER TABLE seed_facts ADD CONSTRAINT fk_seed_facts_relation
  FOREIGN KEY (relation) REFERENCES edb_relations (relation_id);

-- InputHistories
ALTER TABLE input_histories DROP CONSTRAINT IF EXISTS fk_input_histories_answer_key_results;
ALTER TABLE input_histories ADD CONSTRAINT fk_input_histories_answer_key_results
  FOREIGN KEY (answer_key_results) REFERENCES answer_key_results (answer_key_result_id);

-- ReflectionConditions
ALTER TABLE reflection_conditions DROP CONSTRAINT IF EXISTS fk_reflection_conditions_reflection_results;
ALTER TABLE reflection_conditions ADD CONSTRAINT fk_reflection_conditions_reflection_results
  FOREIGN KEY (reflection_results) REFERENCES reflection_results (reflection_result_id);

-- Substrates
ALTER TABLE substrates DROP CONSTRAINT IF EXISTS fk_substrates_answer_key_results;
ALTER TABLE substrates ADD CONSTRAINT fk_substrates_answer_key_results
  FOREIGN KEY (answer_key_results) REFERENCES answer_key_results (answer_key_result_id);
ALTER TABLE substrates DROP CONSTRAINT IF EXISTS fk_substrates_reflection_results;
ALTER TABLE substrates ADD CONSTRAINT fk_substrates_reflection_results
  FOREIGN KEY (reflection_results) REFERENCES reflection_results (reflection_result_id);

-- AnswerKeyResults
ALTER TABLE answer_key_results DROP CONSTRAINT IF EXISTS fk_answer_key_results_substrate;
ALTER TABLE answer_key_results ADD CONSTRAINT fk_answer_key_results_substrate
  FOREIGN KEY (substrate) REFERENCES substrates (substrate_id);
ALTER TABLE answer_key_results DROP CONSTRAINT IF EXISTS fk_answer_key_results_input_history;
ALTER TABLE answer_key_results ADD CONSTRAINT fk_answer_key_results_input_history
  FOREIGN KEY (input_history) REFERENCES input_histories (input_history_id);

-- ReflectionResults
ALTER TABLE reflection_results DROP CONSTRAINT IF EXISTS fk_reflection_results_substrate;
ALTER TABLE reflection_results ADD CONSTRAINT fk_reflection_results_substrate
  FOREIGN KEY (substrate) REFERENCES substrates (substrate_id);
ALTER TABLE reflection_results DROP CONSTRAINT IF EXISTS fk_reflection_results_reflection_condition;
ALTER TABLE reflection_results ADD CONSTRAINT fk_reflection_results_reflection_condition
  FOREIGN KEY (reflection_condition) REFERENCES reflection_conditions (condition_id);

-- 26 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
