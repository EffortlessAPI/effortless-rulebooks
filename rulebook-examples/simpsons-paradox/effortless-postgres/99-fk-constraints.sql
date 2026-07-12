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

-- Loops
ALTER TABLE loops DROP CONSTRAINT IF EXISTS fk_loops_tradition_id;
ALTER TABLE loops ADD CONSTRAINT fk_loops_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);

-- Studies
ALTER TABLE studies DROP CONSTRAINT IF EXISTS fk_studies_tradition_id;
ALTER TABLE studies ADD CONSTRAINT fk_studies_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);
ALTER TABLE studies DROP CONSTRAINT IF EXISTS fk_studies_primary_researcher_id;
ALTER TABLE studies ADD CONSTRAINT fk_studies_primary_researcher_id
  FOREIGN KEY (primary_researcher_id) REFERENCES researchers (researcher_id);

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
ALTER TABLE invariant_checks DROP CONSTRAINT IF EXISTS fk_invariant_checks_tradition_id;
ALTER TABLE invariant_checks ADD CONSTRAINT fk_invariant_checks_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);
ALTER TABLE invariant_checks DROP CONSTRAINT IF EXISTS fk_invariant_checks_protects_conclusion;
ALTER TABLE invariant_checks ADD CONSTRAINT fk_invariant_checks_protects_conclusion
  FOREIGN KEY (protects_conclusion) REFERENCES conclusions (conclusion_id);

-- Methodology
ALTER TABLE methodology DROP CONSTRAINT IF EXISTS fk_methodology_tradition_id;
ALTER TABLE methodology ADD CONSTRAINT fk_methodology_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);
ALTER TABLE methodology DROP CONSTRAINT IF EXISTS fk_methodology_pioneering_researcher;
ALTER TABLE methodology ADD CONSTRAINT fk_methodology_pioneering_researcher
  FOREIGN KEY (pioneering_researcher) REFERENCES researchers (researcher_id);

-- Conclusions
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_validating_hypothesis;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_validating_hypothesis
  FOREIGN KEY (validating_hypothesis) REFERENCES discovery_findings (finding_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_witnessed_in_loop;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_witnessed_in_loop
  FOREIGN KEY (witnessed_in_loop) REFERENCES loops (loop_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_target_loop;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_target_loop
  FOREIGN KEY (target_loop) REFERENCES loops (loop_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_tradition_id;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_researcher_id;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_researcher_id
  FOREIGN KEY (researcher_id) REFERENCES researchers (researcher_id);
ALTER TABLE conclusions DROP CONSTRAINT IF EXISTS fk_conclusions_challenges_researcher;
ALTER TABLE conclusions ADD CONSTRAINT fk_conclusions_challenges_researcher
  FOREIGN KEY (challenges_researcher) REFERENCES researchers (researcher_id);

-- UIScreens
ALTER TABLE ui_screens DROP CONSTRAINT IF EXISTS fk_ui_screens_tradition_id;
ALTER TABLE ui_screens ADD CONSTRAINT fk_ui_screens_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);
ALTER TABLE ui_screens DROP CONSTRAINT IF EXISTS fk_ui_screens_primary_conclusion;
ALTER TABLE ui_screens ADD CONSTRAINT fk_ui_screens_primary_conclusion
  FOREIGN KEY (primary_conclusion) REFERENCES conclusions (conclusion_id);

-- AllocationSweep
ALTER TABLE allocation_sweep DROP CONSTRAINT IF EXISTS fk_allocation_sweep_study_id;
ALTER TABLE allocation_sweep ADD CONSTRAINT fk_allocation_sweep_study_id
  FOREIGN KEY (study_id) REFERENCES sweep_study_config (config_id);

-- ResearchTraditions
ALTER TABLE research_traditions DROP CONSTRAINT IF EXISTS fk_research_traditions_illustrated_by_study;
ALTER TABLE research_traditions ADD CONSTRAINT fk_research_traditions_illustrated_by_study
  FOREIGN KEY (illustrated_by_study) REFERENCES studies (study_id);
ALTER TABLE research_traditions DROP CONSTRAINT IF EXISTS fk_research_traditions_supporting_conclusion;
ALTER TABLE research_traditions ADD CONSTRAINT fk_research_traditions_supporting_conclusion
  FOREIGN KEY (supporting_conclusion) REFERENCES conclusions (conclusion_id);

-- Researchers
ALTER TABLE researchers DROP CONSTRAINT IF EXISTS fk_researchers_tradition_id;
ALTER TABLE researchers ADD CONSTRAINT fk_researchers_tradition_id
  FOREIGN KEY (tradition_id) REFERENCES research_traditions (tradition_id);
ALTER TABLE researchers DROP CONSTRAINT IF EXISTS fk_researchers_canonical_study_id;
ALTER TABLE researchers ADD CONSTRAINT fk_researchers_canonical_study_id
  FOREIGN KEY (canonical_study_id) REFERENCES studies (study_id);
ALTER TABLE researchers DROP CONSTRAINT IF EXISTS fk_researchers_illustrates_conclusion;
ALTER TABLE researchers ADD CONSTRAINT fk_researchers_illustrates_conclusion
  FOREIGN KEY (illustrates_conclusion) REFERENCES conclusions (conclusion_id);

-- CandidateStudyCatalog
ALTER TABLE candidate_study_catalog DROP CONSTRAINT IF EXISTS fk_candidate_study_catalog_linked_study_id;
ALTER TABLE candidate_study_catalog ADD CONSTRAINT fk_candidate_study_catalog_linked_study_id
  FOREIGN KEY (linked_study_id) REFERENCES treatment_rankings (treatment_ranking_id);

-- DomainExpansionTargets
ALTER TABLE domain_expansion_targets DROP CONSTRAINT IF EXISTS fk_domain_expansion_targets_domain;
ALTER TABLE domain_expansion_targets ADD CONSTRAINT fk_domain_expansion_targets_domain
  FOREIGN KEY (domain) REFERENCES corpus_domains (domain_id);

-- DiscoveryHypotheses
ALTER TABLE discovery_hypotheses DROP CONSTRAINT IF EXISTS fk_discovery_hypotheses_registered_in_loop;
ALTER TABLE discovery_hypotheses ADD CONSTRAINT fk_discovery_hypotheses_registered_in_loop
  FOREIGN KEY (registered_in_loop) REFERENCES loops (loop_id);

-- DiscoveryFindings
ALTER TABLE discovery_findings DROP CONSTRAINT IF EXISTS fk_discovery_findings_hypothesis_id;
ALTER TABLE discovery_findings ADD CONSTRAINT fk_discovery_findings_hypothesis_id
  FOREIGN KEY (hypothesis_id) REFERENCES discovery_hypotheses (hypothesis_id);
ALTER TABLE discovery_findings DROP CONSTRAINT IF EXISTS fk_discovery_findings_witnessed_in_loop;
ALTER TABLE discovery_findings ADD CONSTRAINT fk_discovery_findings_witnessed_in_loop
  FOREIGN KEY (witnessed_in_loop) REFERENCES loops (loop_id);

-- StratumVariableIdentityMaps
ALTER TABLE stratum_variable_identity_maps DROP CONSTRAINT IF EXISTS fk_stratum_variable_identity_maps_stratum_variable;
ALTER TABLE stratum_variable_identity_maps ADD CONSTRAINT fk_stratum_variable_identity_maps_stratum_variable
  FOREIGN KEY (stratum_variable) REFERENCES stratum_variables (stratum_variable_id);
ALTER TABLE stratum_variable_identity_maps DROP CONSTRAINT IF EXISTS fk_stratum_variable_identity_maps_confounder_identity;
ALTER TABLE stratum_variable_identity_maps ADD CONSTRAINT fk_stratum_variable_identity_maps_confounder_identity
  FOREIGN KEY (confounder_identity) REFERENCES confounder_identities (confounder_identity_id);

-- IdentityClusterSummaries
ALTER TABLE identity_cluster_summaries DROP CONSTRAINT IF EXISTS fk_identity_cluster_summaries_confounder_identity;
ALTER TABLE identity_cluster_summaries ADD CONSTRAINT fk_identity_cluster_summaries_confounder_identity
  FOREIGN KEY (confounder_identity) REFERENCES confounder_identities (confounder_identity_id);

-- IdentityDomainCells
ALTER TABLE identity_domain_cells DROP CONSTRAINT IF EXISTS fk_identity_domain_cells_confounder_identity;
ALTER TABLE identity_domain_cells ADD CONSTRAINT fk_identity_domain_cells_confounder_identity
  FOREIGN KEY (confounder_identity) REFERENCES confounder_identities (confounder_identity_id);

-- ConfounderDistortionTimeline
ALTER TABLE confounder_distortion_timeline DROP CONSTRAINT IF EXISTS fk_confounder_distortion_timeline_confounder_identity;
ALTER TABLE confounder_distortion_timeline ADD CONSTRAINT fk_confounder_distortion_timeline_confounder_identity
  FOREIGN KEY (confounder_identity) REFERENCES confounder_identities (confounder_identity_id);

-- 37 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
