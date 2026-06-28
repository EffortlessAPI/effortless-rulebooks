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

-- Treatments
ALTER TABLE treatments DROP CONSTRAINT IF EXISTS fk_treatments_study;
ALTER TABLE treatments ADD CONSTRAINT fk_treatments_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- Strata
ALTER TABLE strata DROP CONSTRAINT IF EXISTS fk_strata_study;
ALTER TABLE strata ADD CONSTRAINT fk_strata_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- CaseCells
ALTER TABLE case_cells DROP CONSTRAINT IF EXISTS fk_case_cells_study;
ALTER TABLE case_cells ADD CONSTRAINT fk_case_cells_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- StratumSummaries
ALTER TABLE stratum_summaries DROP CONSTRAINT IF EXISTS fk_stratum_summaries_study;
ALTER TABLE stratum_summaries ADD CONSTRAINT fk_stratum_summaries_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- StratumVariables
ALTER TABLE stratum_variables DROP CONSTRAINT IF EXISTS fk_stratum_variables_study;
ALTER TABLE stratum_variables ADD CONSTRAINT fk_stratum_variables_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- TreatmentRankings
ALTER TABLE treatment_rankings DROP CONSTRAINT IF EXISTS fk_treatment_rankings_study;
ALTER TABLE treatment_rankings ADD CONSTRAINT fk_treatment_rankings_study
  FOREIGN KEY (study) REFERENCES studies (study_id);

-- InvariantChecks
ALTER TABLE invariant_checks DROP CONSTRAINT IF EXISTS fk_invariant_checks_protects_conclusion;
ALTER TABLE invariant_checks ADD CONSTRAINT fk_invariant_checks_protects_conclusion
  FOREIGN KEY (protects_conclusion) REFERENCES conclusions (conclusion_id);

-- Methodology
ALTER TABLE methodology DROP CONSTRAINT IF EXISTS fk_methodology_pioneering_researcher;
ALTER TABLE methodology ADD CONSTRAINT fk_methodology_pioneering_researcher
  FOREIGN KEY (pioneering_researcher) REFERENCES researchers (researcher_id);

-- Conclusions
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_witnessed_in_loop;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_witnessed_in_loop
  FOREIGN KEY (witnessed_in_loop) REFERENCES loops (loop_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_target_loop;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_target_loop
  FOREIGN KEY (target_loop) REFERENCES loops (loop_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_challenges_researcher;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_challenges_researcher
  FOREIGN KEY (challenges_researcher) REFERENCES researchers (researcher_id);

-- UIScreens
ALTER TABLE ui_screens DROP CONSTRAINT IF EXISTS fk_ui_screens_primary_conclusion;
ALTER TABLE ui_screens ADD CONSTRAINT fk_ui_screens_primary_conclusion
  FOREIGN KEY (primary_conclusion) REFERENCES conclusions (conclusion_id);

-- ResearchTraditions
ALTER TABLE research_traditions DROP CONSTRAINT IF EXISTS fk_research_traditions_illustrated_by_study;
ALTER TABLE research_traditions ADD CONSTRAINT fk_research_traditions_illustrated_by_study
  FOREIGN KEY (illustrated_by_study) REFERENCES studies (study_id);
ALTER TABLE research_traditions DROP CONSTRAINT IF EXISTS fk_research_traditions_supporting_conclusion;
ALTER TABLE research_traditions ADD CONSTRAINT fk_research_traditions_supporting_conclusion
  FOREIGN KEY (supporting_conclusion) REFERENCES conclusions (conclusion_id);

-- Researchers
ALTER TABLE researchers DROP CONSTRAINT IF EXISTS fk_researchers_illustrates_conclusion;
ALTER TABLE researchers ADD CONSTRAINT fk_researchers_illustrates_conclusion
  FOREIGN KEY (illustrates_conclusion) REFERENCES conclusions (conclusion_id);

-- 15 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
