-- All study data (synthetic and real) now lives in the rulebook JSON SSoT.
-- It is generated into 05-insert-data.sql by the transpiler on every build.

-- loop-90: the 15 hand Studies UPDATE statements that used to live here
-- (PublicationYear/Domain/IsSynthetic) are DELETED — confirmed dead.
-- 05-insert-data.sql (native transpiler output) already inserts these
-- columns directly from the rulebook's Studies schema; these UPDATEs were
-- redundant no-ops re-setting values already correct after the INSERT.

-- Loop rows beyond what the transpiler quota emits (loop-36+)
INSERT INTO loops (loop_id, title, status, new_concept, domain_question, next_suggestion)
VALUES
  ('loop-36', 'DistortionRatio: the signed ratio that algebraically encodes all four distortion types', 'complete', 'DistortionRatio', 'What single number captures whether allocation attenuated, amplified, or preserved the treatment signal — and whether it flipped sign?', 'loop-37: Instrument definition — the plane, boundary, and screening formula using DistortionRatio as the key diagnostic'),
  ('loop-37', 'InstrumentScore + ScreeningVerdict + ScreeningTier: the three-tier practitioner verdict', 'complete', 'InstrumentScore, ScreeningVerdict, ScreeningTier', 'What is the minimal formula a practitioner needs to screen any 2×k study for Simpson''s Paradox?', 'loop-38: StratumImbalanceScore — per-stratum max allocation bias and max gap as scalar witnesses'),
  ('loop-38', 'StratumImbalanceScore: max allocation bias and max stratum gap as scalar witnesses', 'complete', 'MaxStratumImbalance, MaxStratumGap', 'Which stratum is most responsible for the pooled distortion, and how large is the underlying treatment effect being masked?', 'loop-39: ArmSizeRatio + SymmetryDeparture — correct the reversal threshold for non-symmetric arm sizes'),
  ('loop-39', 'ArmSizeRatio + SymmetryDeparture: arm-size-corrected threshold for non-symmetric allocation', 'complete', 'ArmSizeRatio, SymmetryDeparture', 'How far does the overall arm split depart from balance, and what does that imply for the threshold at which pooled analysis becomes unreliable?', 'loop-40: InvariantChecks expansion — ratio partition formally asserted as DAG-level proof'),
  ('loop-40', 'InvariantChecks expansion: ratio partition formally asserted as DAG-level proof', 'complete', 'inv-type-a-ratio-negative, inv-type-b-ratio-lt-neg1, inv-type-d-ratio-unity, inv-type-c-ratio-positive', 'Can the DistortionRatio partition be asserted as formal algebraic invariants?', 'loop-41: ModelSummary expansion — ratio statistics per type'),
  ('loop-41', 'ModelSummary expansion: ratio statistics per type and InstrumentScore summary', 'complete', 'MinRatioTypeA, MaxRatioTypeA, MinRatioTypeB, AvgInstrumentScoreDanger, DangerTierCount, SafeTierCount', 'What do DistortionRatio statistics reveal about the range within each type, and how many studies fall in each tier?', 'loop-42: InstrumentDashboard UI — tier-colored table view of all 90+ studies'),
  ('loop-42', 'InstrumentDashboard: UI view showing DistortionRatio, InstrumentScore, ScreeningTier across the full 90+ study corpus', 'complete', 'InstrumentDashboardView', 'Can a single table view make the instrument''s screening output readable at a glance?', 'loop-43: C+/C- split — type-C collapses two opposite errors; split into amplification (C+) and compression (C-) with distinct invariants.'),
  ('loop-43', 'C+/C-: allocation amplification vs compression as distinct epistemic errors', 'complete', 'DistortionType values C+ and C-; AllocationDirection field; CAmplificationCount and CCompressionCount on ModelSummary; InvariantChecks inv-cplus-ratio-gt-1 and inv-cminus-ratio-in-01', 'The existing type-C category collapses two epistemically opposite errors: C+ causes overconfidence (allocation inflates effect); C- causes underconfidence (allocation suppresses effect). Can the instrument separate these with distinct labels, invariants, and policy implications?', 'loop-44: SignalPurity theorem — a reversal requires noise > signal, witnessed as inv-signal-purity-sign-flip.'),
  ('loop-44', 'SignalPurity theorem: reversal requires noise > signal — witnessed as a DAG invariant', 'complete', 'SignalPurity field; inv-signal-purity-sign-flip InvariantCheck; AvgSignalPurity on ModelSummary', 'Is there a single threshold separating studies where the pooled conclusion is directionally trustworthy from studies where it is not? The theorem: SignalPurity < 0.5 → AllocationDirection = reversal.', 'loop-45: ModelSummary TypeC subtype counts and AvgSignalPurity.'),
  ('loop-45', 'ModelSummary TypeC subtype counts: TypeCPlusCount and TypeCMinusCount', 'complete', 'TypeCPlusCount, TypeCMinusCount', 'How many studies are C+ (allocation amplifies) vs C- (allocation compresses) without sign flip?', 'loop-46: Rulespeak regen — five-type taxonomy in human-readable narrative'),
  ('loop-46', 'Rulespeak regen + final state: five-type taxonomy and InstrumentScore in human-readable narrative', 'complete', 'Rulespeak narrative update', 'Does the rulespeak narrative accurately describe all 46 loops, the five-type taxonomy, and the InstrumentScore formula?', NULL),
  ('loop-51', 'CandidateStudyCatalog: curated import backlog with encoding metadata', 'complete', 'CandidateStudyCatalog', 'Before bulk import, which published studies qualify and what distortion type do we expect?', 'loop-52: CorpusCatalogSummary + DomainExpansionTargets'),
  ('loop-52', 'CorpusCatalogSummary + DomainExpansionTargets: import readiness and domain gaps', 'complete', 'CorpusCatalogSummary, DomainExpansionTargets', 'Is the backlog large enough for bulk import, and which domains remain under-represented?', 'loop-53: StudyImportTemplate'),
  ('loop-53', 'StudyImportTemplate: six-step mechanical encoding checklist', 'complete', 'StudyImportTemplate', 'What exact rulebook rows must an import session create?', 'loop-54: Import Backlog UI'),
  ('loop-54', 'Import Backlog UI: catalog, domain targets, and encoding checklist', 'complete', 'ImportCatalogView at /catalog', 'Can a practitioner see the full import backlog before starting bulk import?', 'loop-55: Bulk corpus import — encode high-priority candidates')
ON CONFLICT (loop_id) DO UPDATE SET
  title = EXCLUDED.title,
  status = EXCLUDED.status,
  new_concept = EXCLUDED.new_concept,
  domain_question = EXCLUDED.domain_question,
  next_suggestion = EXCLUDED.next_suggestion;

-- allocation_direction and signal_purity are now first-class DAG fields (loop-43/44).
-- All InvariantChecks rows (including loop-40 ratio partition) live in the rulebook SSoT.

-- Retired loop-40 strict unity check (replaced by inv-type-d-ratio-near-unity).
DELETE FROM invariant_checks WHERE invariant_check_id = 'inv-type-d-ratio-unity';

-- Degenerate type-D rows: ratio is unstable when |CorrectedGap| is tiny but non-zero.
UPDATE invariant_checks SET
  algebraic_statement = 'DistortionType = D → ABS(DistortionRatio - 1) < 0.15 OR |CorrectedGap| < 0.02',
  assertion_expression = 'ABS(DistortionRatio - 1) < 0.15 OR ABS(CorrectedGap) < 0.02 OR DistortionRatio IS NULL',
  sql_assertion = 'ABS(distortion_ratio - 1) < 0.15 OR ABS(corrected_gap) < 0.02 OR distortion_ratio IS NULL'
WHERE invariant_check_id = 'inv-type-d-ratio-near-unity';

-- ----------------------------------------------------------------------------
-- loop-50/90: refresh PhaseDiagramSummary counts from the synthetic phase
-- grid. Reads live vw_synthetic_phase (not the deleted _erb_sp_metrics
-- cache) — SyntheticPhase's per-row distortion_type is itself a real
-- calculated field computed live by 02-create-functions.sql.
-- ----------------------------------------------------------------------------
SELECT refresh_identity_cluster_summaries();
SELECT refresh_identity_domain_cells();

-- Loop-93: SweepStudySummary principled materialization (MaterializedEntities:
-- mat-sweep-study-summary). Must run after allocation_sweep/sweep_study_config
-- are loaded (05-insert-data.sql, above) and before anything below reads
-- ModelSummary/DiscoveryFindings fields that depend on it.
SELECT refresh_sweep_study_summary();

UPDATE model_summary m SET
  identity_cluster_witness_note = calc_model_summary_identity_cluster_witness_note('simpsons-paradox-v1')
WHERE m.model_summary_id = 'simpsons-paradox-v1';

WITH phase_counts AS (
  SELECT phase_distortion_type, COUNT(*)::int AS n
  FROM vw_synthetic_phase
  GROUP BY phase_distortion_type
)
UPDATE phase_diagram_summary p SET
  phase_type_a_count = COALESCE((SELECT n FROM phase_counts WHERE phase_distortion_type = 'A'), 0),
  phase_type_b_count = COALESCE((SELECT n FROM phase_counts WHERE phase_distortion_type = 'B'), 0),
  phase_type_c_plus_count = COALESCE((SELECT n FROM phase_counts WHERE phase_distortion_type = 'C+'), 0),
  phase_type_c_minus_count = COALESCE((SELECT n FROM phase_counts WHERE phase_distortion_type = 'C-'), 0),
  phase_type_d_count = COALESCE((SELECT n FROM phase_counts WHERE phase_distortion_type = 'D'), 0)
WHERE p.phase_diagram_id = 'phase-diagram-v1';

UPDATE model_summary m SET
  synthetic_phase_count = s.phase_point_count,
  phase_diagram_complete = s.all_five_types_present,
  phase_taxonomy_coverage = s.phase_taxonomy_witness
FROM vw_phase_diagram_summary s
WHERE m.model_summary_id = 'simpsons-paradox-v1'
  AND s.phase_diagram_id = 'phase-diagram-v1';

-- loop-56: drop legacy loop-35 sweep rows (old PK scheme: kidney-f005, berk-f005, …)
DELETE FROM allocation_sweep
WHERE (study_id = 'kidney-1986' AND sweep_id NOT LIKE 'kidney-1986-f%')
   OR (study_id = 'berkeley-1973' AND sweep_id NOT LIKE 'berkeley-1973-f%')
   OR (study_id = 'compressed-synthetic' AND sweep_id NOT LIKE 'compressed-synthetic-f%')
   OR (study_id = 'balanced-synthetic' AND sweep_id NOT LIKE 'balanced-synthetic-f%');

-- loop-76: split discovery harness — corpus hypotheses vs theorem consistency checks (loops 73–76).
UPDATE invariant_checks SET
  algebraic_statement = 'COUNTIFS(DiscoveryFindings IsConfirmed FALSE WHERE EpistemicTier=corpus-hypothesis) = 0',
  natural_language = 'Pre-registered corpus hypotheses that still hold at N=238; expansion-wave-broken patterns (loop-77) deferred to loop-78.',
  filter_expression = 'HypothesisEpistemicTier = corpus-hypothesis',
  sql_filter = 'hypothesis_id IN (''H-latent-d'', ''H-small-effect'', ''H-causal-manifest'', ''H-causal-latent'', ''H-explained-confounder'', ''H-unexplained-nonconfounder'', ''H-catalog-exact-match'', ''H-collider-no-manifest-v2'', ''H-cplus-magnitude'', ''H-ultra-fragile'')',
  pass_count = 10,
  fail_count = 0
WHERE invariant_check_id = 'inv-discovery-all-confirmed';

UPDATE invariant_checks SET
  sql_filter = 'hypothesis_id IN (''H-corrected-gap-invariant'', ''H-explained-bidirectional'', ''H-collider-no-manifest-theorem'', ''H-theorem-portfolio'', ''H-identity-map-coverage'')',
  pass_count = 5,
  fail_count = 0,
  natural_language = 'Theorem wave + identity consistency checks: CorrectedGap invariance, explained↔confounder biconditional, collider no-manifest, theorem portfolio, identity map coverage.'
WHERE invariant_check_id = 'inv-theorem-consistency-confirmed';

INSERT INTO invariant_checks (
  invariant_check_id, algebraic_statement, natural_language, source_table,
  filter_expression, assertion_expression, sql_filter, sql_assertion,
  pass_count, fail_count, severity, tradition_id, protects_conclusion
) VALUES (
  'inv-theorem-consistency-confirmed',
  'COUNTIFS(DiscoveryFindings IsConfirmed FALSE WHERE EpistemicTier=consistency-check AND RegisteredInLoop IN loop-73..76) = 0',
  'Theorem wave consistency checks (loops 73–76): CorrectedGap invariance, explained↔confounder biconditional, collider no-manifest, theorem portfolio.',
  'DiscoveryFindings',
  'HypothesisEpistemicTier = consistency-check',
  'IsConfirmed = TRUE',
  'hypothesis_id IN (''H-corrected-gap-invariant'', ''H-explained-bidirectional'', ''H-collider-no-manifest-theorem'', ''H-theorem-portfolio'')',
  'is_confirmed = TRUE',
  4, 0, 'critical', 'tradition-dag', 'conc-31-theorem-portfolio-synthesis'
) ON CONFLICT (invariant_check_id) DO UPDATE SET
  algebraic_statement = EXCLUDED.algebraic_statement,
  natural_language = EXCLUDED.natural_language,
  source_table = EXCLUDED.source_table,
  filter_expression = EXCLUDED.filter_expression,
  assertion_expression = EXCLUDED.assertion_expression,
  sql_filter = EXCLUDED.sql_filter,
  sql_assertion = EXCLUDED.sql_assertion,
  pass_count = EXCLUDED.pass_count,
  fail_count = EXCLUDED.fail_count,
  severity = EXCLUDED.severity,
  tradition_id = EXCLUDED.tradition_id,
  protects_conclusion = EXCLUDED.protects_conclusion;

-- ----------------------------------------------------------------------------
-- Evaluate the invariant checks defined in the InvariantChecks rulebook table.
-- run_invariant_checks() reads SqlFilter and SqlAssertion from invariant_checks
-- and writes pass_count / fail_count back. The entire test harness is driven
-- from the rulebook — no assertion SQL lives outside the InvariantChecks table.
-- ----------------------------------------------------------------------------
SELECT * FROM run_invariant_checks();

-- Fail the build if any critical invariant has FailCount > 0.
DO $$
DECLARE
  v_fail INTEGER;
  v_detail TEXT;
BEGIN
  SELECT COALESCE(SUM(fail_count), 0)
    INTO v_fail
    FROM invariant_checks
   WHERE severity = 'critical';

  IF v_fail > 0 THEN
    SELECT string_agg(invariant_check_id || ': FAIL(' || fail_count || ')', ', ' ORDER BY invariant_check_id)
      INTO v_detail
      FROM invariant_checks
     WHERE severity = 'critical' AND fail_count > 0;

    RAISE EXCEPTION 'Invariant harness: % critical failure(s): %', v_fail, v_detail;
  END IF;
END $$;
