-- ============================================================================
-- REAL STUDY HYDRATION — data/raw/<study-id>/provenance.md for source notes
-- Idempotent upserts; safe to re-run on every init-db.sh invocation.
-- The rulebook SSoT defines the schema; these rows enter through the
-- customization file so real data never lives in the rulebook JSON.
-- ============================================================================


-- ============================================================================
-- REINTJES et al. 2000 — Antibiotic Prophylaxis and UTI in Dutch Hospitals
-- Source: Epidemiology 2000;11(1):81-83. PubMed PMID: 10615849
-- N=3519 patients. Strata: hospital baseline UTI incidence (low/high).
-- Groups: A=prophylaxis given, B=control (no prophylaxis).
-- Outcome: UTI acquired. A is WORSE in every stratum; aggregate inverts.
-- ============================================================================

INSERT INTO studies (study_id, title, source, source_url)
VALUES (
  'reintjes-2000',
  'Antibiotic Prophylaxis and UTI Rates in Dutch Hospitals (Reintjes et al. 2000)',
  'Reintjes R, de Boer A, van Pelt W, Mintjes-de Groot J. Simpson''s paradox: an example from hospital epidemiology. Epidemiology. 2000;11(1):81-83. PMID: 10615849.',
  'https://pubmed.ncbi.nlm.nih.gov/10615849/'
)
ON CONFLICT (study_id) DO UPDATE SET
  title      = EXCLUDED.title,
  source     = EXCLUDED.source,
  source_url = EXCLUDED.source_url;

INSERT INTO treatments (treatment_id, study, treatment_label, description)
VALUES
  ('reintjes-2000-A', 'reintjes-2000', 'A',
   'Antibiotic prophylaxis given. Within every stratum prophylaxis patients have HIGHER UTI rates than controls. Aggregate inverts due to confounding by hospital incidence level.'),
  ('reintjes-2000-B', 'reintjes-2000', 'B',
   'No antibiotic prophylaxis (control). Within every stratum controls have LOWER UTI rates. Aggregate inverts: controls appear to have higher rates pooled.')
ON CONFLICT (treatment_id) DO UPDATE SET
  study            = EXCLUDED.study,
  treatment_label  = EXCLUDED.treatment_label,
  description      = EXCLUDED.description;

INSERT INTO strata (stratum_id, study, stratum_label, description)
VALUES
  ('reintjes-2000-low', 'reintjes-2000', 'low',
   'Low-incidence hospitals (baseline UTI rate ≤2.5%). Prophylaxis rate: 20/1113=1.80%. Control rate: 5/720=0.69%. Prophylaxis is harmful here. Most prophylaxis patients land in this stratum (87%), pulling the aggregate prophylaxis rate down and fabricating an apparent protective effect pooled.'),
  ('reintjes-2000-high', 'reintjes-2000', 'high',
   'High-incidence hospitals (baseline UTI rate >2.5%). Prophylaxis rate: 22/166=13.25%. Control rate: 99/1520=6.51%. Prophylaxis is again harmful. Controls are heavily concentrated here (1520 of 2240=68%), inflating the aggregate control rate.')
ON CONFLICT (stratum_id) DO UPDATE SET
  study         = EXCLUDED.study,
  stratum_label = EXCLUDED.stratum_label,
  description   = EXCLUDED.description;

-- Raw counts from Table 1 of Reintjes et al. 2000.
-- successes = UTI events (higher = worse outcome; direction noted in provenance).
INSERT INTO case_cells (case_cell_id, study, stratum_label, treatment_label, successes, cases)
VALUES
  ('reintjes-2000-low-A',  'reintjes-2000', 'low',  'A',  20,  1113),
  ('reintjes-2000-low-B',  'reintjes-2000', 'low',  'B',   5,   720),
  ('reintjes-2000-high-A', 'reintjes-2000', 'high', 'A',  22,   166),
  ('reintjes-2000-high-B', 'reintjes-2000', 'high', 'B',  99,  1520)
ON CONFLICT (case_cell_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label,
  successes       = EXCLUDED.successes,
  cases           = EXCLUDED.cases;

INSERT INTO stratum_summaries (stratum_summary_id, study, stratum_label, treatment_label)
VALUES
  ('reintjes-2000-low-A',  'reintjes-2000', 'low',  'A'),
  ('reintjes-2000-low-B',  'reintjes-2000', 'low',  'B'),
  ('reintjes-2000-high-A', 'reintjes-2000', 'high', 'A'),
  ('reintjes-2000-high-B', 'reintjes-2000', 'high', 'B')
ON CONFLICT (stratum_summary_id) DO UPDATE SET
  study         = EXCLUDED.study,
  stratum_label = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label;

INSERT INTO stratum_variables (stratum_variable_id, study, variable_name, causal_role,
  affects_treatment_assignment, affects_outcome, mechanism_note)
VALUES (
  'reintjes-2000-hospital-incidence', 'reintjes-2000', 'hospital_incidence_level', 'confounder',
  true, true,
  'Hospital baseline UTI incidence affects both whether a patient received prophylaxis (high-incidence hospitals were more likely to forgo it, concentrating controls there) and the outcome (high-incidence hospitals have higher UTI rates regardless of treatment). Most prophylaxis patients are in low-incidence hospitals (1113/1279=87%), which depresses the aggregate prophylaxis UTI rate below the aggregate control rate — inverting the within-stratum finding.'
)
ON CONFLICT (stratum_variable_id) DO UPDATE SET
  study                        = EXCLUDED.study,
  variable_name                = EXCLUDED.variable_name,
  causal_role                  = EXCLUDED.causal_role,
  affects_treatment_assignment = EXCLUDED.affects_treatment_assignment,
  affects_outcome              = EXCLUDED.affects_outcome,
  mechanism_note               = EXCLUDED.mechanism_note;

INSERT INTO treatment_rankings (treatment_ranking_id, study, treatment_a, treatment_b)
VALUES ('reintjes-2000-A-vs-B', 'reintjes-2000', 'A', 'B')
ON CONFLICT (treatment_ranking_id) DO UPDATE SET
  study       = EXCLUDED.study,
  treatment_a = EXCLUDED.treatment_a,
  treatment_b = EXCLUDED.treatment_b;


-- ============================================================================
-- RADELET 1981 — Racial Characteristics and the Death Penalty in Florida
-- Source: American Sociological Review 1981;46(6):918-927. JSTOR 2095513.
-- N=326 homicide cases, 20 Florida counties, 1976-1977.
-- Strata: victim race (white-victim / black-victim).
-- Groups: A=white defendant, B=black defendant.
-- Outcome: death penalty imposed.
-- B is MORE likely to get death penalty in BOTH strata; aggregate inverts.
-- ============================================================================

INSERT INTO studies (study_id, title, source, source_url)
VALUES (
  'radelet-1981',
  'Racial Characteristics and the Death Penalty in Florida (Radelet 1981)',
  'Radelet ML. Racial characteristics and the imposition of the death penalty. American Sociological Review. 1981;46(6):918-927.',
  'https://www.jstor.org/stable/2095513'
)
ON CONFLICT (study_id) DO UPDATE SET
  title      = EXCLUDED.title,
  source     = EXCLUDED.source,
  source_url = EXCLUDED.source_url;

INSERT INTO treatments (treatment_id, study, treatment_label, description)
VALUES
  ('radelet-1981-A', 'radelet-1981', 'A',
   'White defendant. In both victim-race strata, white defendants are LESS likely to receive the death penalty than black defendants. Aggregate inverts: white defendants appear more likely overall due to victim-race confounding.'),
  ('radelet-1981-B', 'radelet-1981', 'B',
   'Black defendant. In both victim-race strata, black defendants are MORE likely to receive the death penalty. Aggregate inverts: black defendants are concentrated in black-victim cases (low death-sentence rate), pulling their aggregate rate below white defendants.')
ON CONFLICT (treatment_id) DO UPDATE SET
  study            = EXCLUDED.study,
  treatment_label  = EXCLUDED.treatment_label,
  description      = EXCLUDED.description;

INSERT INTO strata (stratum_id, study, stratum_label, description)
VALUES
  ('radelet-1981-white-victim', 'radelet-1981', 'white-victim',
   'Cases with a white victim. Death penalty issued far more often here (~17% pooled). White def: 19/151=12.58%. Black def: 11/63=17.46%. Black defendants MORE likely within this stratum.'),
  ('radelet-1981-black-victim', 'radelet-1981', 'black-victim',
   'Cases with a black victim. Death penalty rare here (~3.3% pooled). White def: 0/9=0.00%. Black def: 6/103=5.83%. Black defendants STILL more likely. Most black defendants are in this low-rate stratum (103/166=62%), dragging their aggregate rate down.')
ON CONFLICT (stratum_id) DO UPDATE SET
  study         = EXCLUDED.study,
  stratum_label = EXCLUDED.stratum_label,
  description   = EXCLUDED.description;

-- Raw counts from Table 1 of Radelet 1981.
-- successes = death penalty imposed.
-- Note: white-defendant, black-victim cell is 0/9 — exact published count, not imputed.
INSERT INTO case_cells (case_cell_id, study, stratum_label, treatment_label, successes, cases)
VALUES
  ('radelet-1981-white-victim-A', 'radelet-1981', 'white-victim', 'A', 19, 151),
  ('radelet-1981-white-victim-B', 'radelet-1981', 'white-victim', 'B', 11,  63),
  ('radelet-1981-black-victim-A', 'radelet-1981', 'black-victim', 'A',  0,   9),
  ('radelet-1981-black-victim-B', 'radelet-1981', 'black-victim', 'B',  6, 103)
ON CONFLICT (case_cell_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label,
  successes       = EXCLUDED.successes,
  cases           = EXCLUDED.cases;

INSERT INTO stratum_summaries (stratum_summary_id, study, stratum_label, treatment_label)
VALUES
  ('radelet-1981-white-victim-A', 'radelet-1981', 'white-victim', 'A'),
  ('radelet-1981-white-victim-B', 'radelet-1981', 'white-victim', 'B'),
  ('radelet-1981-black-victim-A', 'radelet-1981', 'black-victim', 'A'),
  ('radelet-1981-black-victim-B', 'radelet-1981', 'black-victim', 'B')
ON CONFLICT (stratum_summary_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label;

INSERT INTO stratum_variables (stratum_variable_id, study, variable_name, causal_role,
  affects_treatment_assignment, affects_outcome, mechanism_note)
VALUES (
  'radelet-1981-victim-race', 'radelet-1981', 'victim_race', 'confounder',
  true, true,
  'Victim race affects both defendant race (cross-race homicides are rare — most black defendants killed black victims, most white defendants killed white victims) and sentencing outcome (white-victim cases receive the death penalty ~5× more often: ~17.7% vs ~3.3%). This is classic confounding. Black defendants are disproportionately concentrated in the black-victim stratum (low sentence rate), pulling their aggregate death penalty rate below white defendants'' rate — even though black defendants are MORE likely to be sentenced to death within each victim-race stratum.'
)
ON CONFLICT (stratum_variable_id) DO UPDATE SET
  study                        = EXCLUDED.study,
  variable_name                = EXCLUDED.variable_name,
  causal_role                  = EXCLUDED.causal_role,
  affects_treatment_assignment = EXCLUDED.affects_treatment_assignment,
  affects_outcome              = EXCLUDED.affects_outcome,
  mechanism_note               = EXCLUDED.mechanism_note;

INSERT INTO treatment_rankings (treatment_ranking_id, study, treatment_a, treatment_b)
VALUES ('radelet-1981-A-vs-B', 'radelet-1981', 'A', 'B')
ON CONFLICT (treatment_ranking_id) DO UPDATE SET
  study       = EXCLUDED.study,
  treatment_a = EXCLUDED.treatment_a,
  treatment_b = EXCLUDED.treatment_b;
