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


-- ============================================================================
-- JETER / JUSTICE 1995-1997 — Batting Averages (Simpson's Paradox in sport)
-- Source: Ross KA. "Repeating effects in baseball." College Mathematics Journal
--         2007;38(3):205-210. Data: Baseball Almanac (primary MLB records).
-- Raw data: https://www.baseball-almanac.com/players/player.php?p=jeterde01
--           https://www.baseball-almanac.com/players/player.php?p=justida01
-- N=2,330 plate appearances (Jeter 1284, Justice 1046) over 3 seasons.
-- Strata: season year (1995, 1996, 1997).
-- Groups: A=Derek Jeter (Yankees), B=David Justice (Braves/Indians).
-- Outcome: hit (success = H, cases = AB).
-- Justice has higher batting average in EVERY year; Jeter wins the 3-year combined.
-- Mechanism: Justice bulk ABs fell in his weakest year (1995: 411 AB at .253);
--   Jeter's bulk fell in his stronger years (1996: 582 AB at .314, 1997: 654 at .291).
-- ============================================================================

INSERT INTO studies (study_id, title, source, source_url)
VALUES (
  'jeter-justice-1997',
  'Batting Averages: Derek Jeter vs David Justice 1995-1997 (Ross 2007)',
  'Ross KA. Repeating effects in baseball. College Mathematics Journal. 2007;38(3):205-210. Raw data from Baseball Almanac (official MLB records).',
  'https://www.baseball-almanac.com/players/player.php?p=jeterde01'
)
ON CONFLICT (study_id) DO UPDATE SET
  title      = EXCLUDED.title,
  source     = EXCLUDED.source,
  source_url = EXCLUDED.source_url;

INSERT INTO treatments (treatment_id, study, treatment_label, description)
VALUES
  ('jeter-justice-1997-A', 'jeter-justice-1997', 'A',
   'Derek Jeter (New York Yankees). Higher combined batting average (.300) despite losing to Justice in every individual season. Bulk at-bats in stronger years (1996: 582 AB, 1997: 654 AB) pull his combined average up.'),
  ('jeter-justice-1997-B', 'jeter-justice-1997', 'B',
   'David Justice (Atlanta Braves 1995-96, Cleveland Indians 1997). Higher batting average in each individual year (1995: .253, 1996: .321, 1997: .329) but lower combined (.298). Bulk at-bats in his weakest year (1995: 411 AB at .253) drag his combined average down.')
ON CONFLICT (treatment_id) DO UPDATE SET
  study            = EXCLUDED.study,
  treatment_label  = EXCLUDED.treatment_label,
  description      = EXCLUDED.description;

INSERT INTO strata (stratum_id, study, stratum_label, description)
VALUES
  ('jeter-justice-1997-1995', 'jeter-justice-1997', '1995',
   '1995 season. Jeter: 12/48=.250. Justice: 104/411=.253. Justice wins by 3 points. Small season for Jeter (48 AB, rookie call-up); Justice''s heaviest season (411 AB) and his weakest average — this is the stratum doing the most damage to Justice''s combined rate.'),
  ('jeter-justice-1997-1996', 'jeter-justice-1997', '1996',
   '1996 season. Jeter: 183/582=.314. Justice: 45/140=.321. Justice wins by 7 points. Jeter''s breakout full season (582 AB). Justice had a shortened season (140 AB) but a high average — a small, strong stratum that lifts his per-year record but contributes little weight to combined.'),
  ('jeter-justice-1997-1997', 'jeter-justice-1997', '1997',
   '1997 season. Jeter: 190/654=.291. Justice: 163/495=.329. Justice wins by 38 points — the largest per-stratum gap. Jeter''s largest season (654 AB). The allocation imbalance is clear: Jeter has 654 AB in a .291 year vs Justice''s 495 at .329.')
ON CONFLICT (stratum_id) DO UPDATE SET
  study         = EXCLUDED.study,
  stratum_label = EXCLUDED.stratum_label,
  description   = EXCLUDED.description;

-- Raw counts from Baseball Almanac (official MLB records). Verified via
-- Ross (2007) College Mathematics Journal and multiple independent sources.
-- successes = hits (higher = better outcome; A = Jeter, B = Justice).
INSERT INTO case_cells (case_cell_id, study, stratum_label, treatment_label, successes, cases)
VALUES
  ('jeter-justice-1997-1995-A', 'jeter-justice-1997', '1995', 'A',  12,  48),
  ('jeter-justice-1997-1995-B', 'jeter-justice-1997', '1995', 'B', 104, 411),
  ('jeter-justice-1997-1996-A', 'jeter-justice-1997', '1996', 'A', 183, 582),
  ('jeter-justice-1997-1996-B', 'jeter-justice-1997', '1996', 'B',  45, 140),
  ('jeter-justice-1997-1997-A', 'jeter-justice-1997', '1997', 'A', 190, 654),
  ('jeter-justice-1997-1997-B', 'jeter-justice-1997', '1997', 'B', 163, 495)
ON CONFLICT (case_cell_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label,
  successes       = EXCLUDED.successes,
  cases           = EXCLUDED.cases;

INSERT INTO stratum_summaries (stratum_summary_id, study, stratum_label, treatment_label)
VALUES
  ('jeter-justice-1997-1995-A', 'jeter-justice-1997', '1995', 'A'),
  ('jeter-justice-1997-1995-B', 'jeter-justice-1997', '1995', 'B'),
  ('jeter-justice-1997-1996-A', 'jeter-justice-1997', '1996', 'A'),
  ('jeter-justice-1997-1996-B', 'jeter-justice-1997', '1996', 'B'),
  ('jeter-justice-1997-1997-A', 'jeter-justice-1997', '1997', 'A'),
  ('jeter-justice-1997-1997-B', 'jeter-justice-1997', '1997', 'B')
ON CONFLICT (stratum_summary_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label;

INSERT INTO stratum_variables (stratum_variable_id, study, variable_name, causal_role,
  affects_treatment_assignment, affects_outcome, mechanism_note)
VALUES (
  'jeter-justice-1997-season', 'jeter-justice-1997', 'season_year', 'confounder',
  true, true,
  'The season year affects both the player''s at-bat count (treatment assignment proxy — Justice''s heaviest season was 1995 when his average was lowest; Jeter''s heaviest were 1996-97 when his average was higher) and the outcome (players hit differently in different years). Justice front-loaded his ABs into his worst year (411 of 1046 = 39% in 1995 at .253); Jeter back-loaded into his stronger years (1236 of 1284 = 96% in 1996-97 at .300-.314). This differential allocation of AB-weight across years flips the combined average despite Justice winning every year individually.'
)
ON CONFLICT (stratum_variable_id) DO UPDATE SET
  study                        = EXCLUDED.study,
  variable_name                = EXCLUDED.variable_name,
  causal_role                  = EXCLUDED.causal_role,
  affects_treatment_assignment = EXCLUDED.affects_treatment_assignment,
  affects_outcome              = EXCLUDED.affects_outcome,
  mechanism_note               = EXCLUDED.mechanism_note;

INSERT INTO treatment_rankings (treatment_ranking_id, study, treatment_a, treatment_b)
VALUES ('jeter-justice-1997-A-vs-B', 'jeter-justice-1997', 'A', 'B')
ON CONFLICT (treatment_ranking_id) DO UPDATE SET
  study       = EXCLUDED.study,
  treatment_a = EXCLUDED.treatment_a,
  treatment_b = EXCLUDED.treatment_b;


-- ============================================================================
-- APPLETON et al. 1996 — Whickham Smoking Cohort
-- Source: Appleton DR, French JM, Vanderpump MPJ. "Ignoring a covariate:
--         An example of Simpson's paradox." The American Statistician.
--         1996;50(4):340-341. DOI: 10.2307/2684931 / JSTOR stable/2684931.
-- Raw data: Whickham (England) health survey 1972-74, 20-year follow-up.
--   Dataset: https://vincentarelbundock.github.io/Rdatasets/csv/mosaicData/Whickham.csv
--   Documentation: https://vincentarelbundock.github.io/Rdatasets/doc/mosaicData/Whickham.html
-- N=1,314 women. Strata: age group at baseline (7 groups, 10-year bands).
-- Groups: A=smoker, B=non-smoker.
-- Outcome: dead at 20-year follow-up.
-- Smokers have HIGHER mortality in every age group; pooled rate INVERTS (smokers appear healthier).
-- Mechanism: smokers were younger on average at baseline; younger women have lower 20-year
--   mortality regardless of smoking. Age confounds the pooled comparison.
-- ============================================================================

INSERT INTO studies (study_id, title, source, source_url)
VALUES (
  'appleton-1996',
  'Whickham Smoking Cohort — 20-year Mortality by Smoking Status (Appleton et al. 1996)',
  'Appleton DR, French JM, Vanderpump MPJ. Ignoring a covariate: An example of Simpson''s paradox. The American Statistician. 1996;50(4):340-341. DOI: 10.2307/2684931. Data: Whickham (England) health survey 1972-74, 20-year follow-up.',
  'https://doi.org/10.2307/2684931'
)
ON CONFLICT (study_id) DO UPDATE SET
  title      = EXCLUDED.title,
  source     = EXCLUDED.source,
  source_url = EXCLUDED.source_url;

INSERT INTO treatments (treatment_id, study, treatment_label, description)
VALUES
  ('appleton-1996-A', 'appleton-1996', 'A',
   'Smoker at baseline (1972-74). Higher mortality within every age stratum. Pooled mortality appears LOWER (23.9%) than non-smokers (31.4%) — the reversal. Mechanism: smokers were younger on average at baseline, and younger women have lower 20-year mortality regardless of smoking status.'),
  ('appleton-1996-B', 'appleton-1996', 'B',
   'Non-smoker at baseline. Lower mortality within every age stratum. Pooled mortality appears HIGHER (31.4%) — misleadingly so. Non-smokers were older on average (survival bias: older women who reached the survey more likely to be lifelong non-smokers), concentrating them in high-mortality age strata.')
ON CONFLICT (treatment_id) DO UPDATE SET
  study            = EXCLUDED.study,
  treatment_label  = EXCLUDED.treatment_label,
  description      = EXCLUDED.description;

INSERT INTO strata (stratum_id, study, stratum_label, description)
VALUES
  ('appleton-1996-18-24', 'appleton-1996', '18-24',
   'Age 18-24 at baseline. Smokers: 2/55=3.6% mortality. Non-smokers: 1/72=1.4%. Smokers worse. Very low mortality overall; small absolute differences.'),
  ('appleton-1996-25-34', 'appleton-1996', '25-34',
   'Age 25-34 at baseline. Smokers: 3/125=2.4% mortality. Non-smokers: 4/156=2.6%. Effectively tied (smokers marginally better — near-zero stratum gap). Very low mortality stratum.'),
  ('appleton-1996-35-44', 'appleton-1996', '35-44',
   'Age 35-44 at baseline. Smokers: 10/105=9.5% mortality. Non-smokers: 7/111=6.3%. Smokers clearly worse.'),
  ('appleton-1996-45-54', 'appleton-1996', '45-54',
   'Age 45-54 at baseline. Smokers: 28/131=21.4% mortality. Non-smokers: 12/78=15.4%. Smokers worse; gap widening with age.'),
  ('appleton-1996-55-64', 'appleton-1996', '55-64',
   'Age 55-64 at baseline. Smokers: 52/116=44.8% mortality. Non-smokers: 41/122=33.6%. Smokers clearly worse; largest absolute gap in the middle strata.'),
  ('appleton-1996-65-74', 'appleton-1996', '65-74',
   'Age 65-74 at baseline. Smokers: 31/37=83.8% mortality. Non-smokers: 101/129=78.3%. Smokers worse. Small smoker group (37) — most heavy smokers in this cohort had already died before reaching 65 at baseline (survivorship selection).'),
  ('appleton-1996-75plus', 'appleton-1996', '75+',
   'Age 75+ at baseline. Smokers: 13/13=100% mortality. Non-smokers: 64/64=100%. Both groups reach 100% at 20-year follow-up — everyone in this stratum died. StratumGap=0. Heavy survivorship selection: only 13 smokers survived to 75+ at baseline vs 64 non-smokers.')
ON CONFLICT (stratum_id) DO UPDATE SET
  study         = EXCLUDED.study,
  stratum_label = EXCLUDED.stratum_label,
  description   = EXCLUDED.description;

-- Raw counts from Whickham.csv (mosaicData R package), exact row counts verified
-- against the Appleton (1996) paper.
-- Source CSV: https://vincentarelbundock.github.io/Rdatasets/csv/mosaicData/Whickham.csv
-- successes = dead at 20-year follow-up (higher = worse outcome).
INSERT INTO case_cells (case_cell_id, study, stratum_label, treatment_label, successes, cases)
VALUES
  ('appleton-1996-18-24-A',  'appleton-1996', '18-24',  'A',  2,  55),
  ('appleton-1996-18-24-B',  'appleton-1996', '18-24',  'B',  1,  72),
  ('appleton-1996-25-34-A',  'appleton-1996', '25-34',  'A',  3, 125),
  ('appleton-1996-25-34-B',  'appleton-1996', '25-34',  'B',  4, 156),
  ('appleton-1996-35-44-A',  'appleton-1996', '35-44',  'A', 10, 105),
  ('appleton-1996-35-44-B',  'appleton-1996', '35-44',  'B',  7, 111),
  ('appleton-1996-45-54-A',  'appleton-1996', '45-54',  'A', 28, 131),
  ('appleton-1996-45-54-B',  'appleton-1996', '45-54',  'B', 12,  78),
  ('appleton-1996-55-64-A',  'appleton-1996', '55-64',  'A', 52, 116),
  ('appleton-1996-55-64-B',  'appleton-1996', '55-64',  'B', 41, 122),
  ('appleton-1996-65-74-A',  'appleton-1996', '65-74',  'A', 31,  37),
  ('appleton-1996-65-74-B',  'appleton-1996', '65-74',  'B',101, 129),
  ('appleton-1996-75plus-A', 'appleton-1996', '75+',    'A', 13,  13),
  ('appleton-1996-75plus-B', 'appleton-1996', '75+',    'B', 64,  64)
ON CONFLICT (case_cell_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label,
  successes       = EXCLUDED.successes,
  cases           = EXCLUDED.cases;

INSERT INTO stratum_summaries (stratum_summary_id, study, stratum_label, treatment_label)
VALUES
  ('appleton-1996-18-24-A',  'appleton-1996', '18-24',  'A'),
  ('appleton-1996-18-24-B',  'appleton-1996', '18-24',  'B'),
  ('appleton-1996-25-34-A',  'appleton-1996', '25-34',  'A'),
  ('appleton-1996-25-34-B',  'appleton-1996', '25-34',  'B'),
  ('appleton-1996-35-44-A',  'appleton-1996', '35-44',  'A'),
  ('appleton-1996-35-44-B',  'appleton-1996', '35-44',  'B'),
  ('appleton-1996-45-54-A',  'appleton-1996', '45-54',  'A'),
  ('appleton-1996-45-54-B',  'appleton-1996', '45-54',  'B'),
  ('appleton-1996-55-64-A',  'appleton-1996', '55-64',  'A'),
  ('appleton-1996-55-64-B',  'appleton-1996', '55-64',  'B'),
  ('appleton-1996-65-74-A',  'appleton-1996', '65-74',  'A'),
  ('appleton-1996-65-74-B',  'appleton-1996', '65-74',  'B'),
  ('appleton-1996-75plus-A', 'appleton-1996', '75+',    'A'),
  ('appleton-1996-75plus-B', 'appleton-1996', '75+',    'B')
ON CONFLICT (stratum_summary_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label;

INSERT INTO stratum_variables (stratum_variable_id, study, variable_name, causal_role,
  affects_treatment_assignment, affects_outcome, mechanism_note)
VALUES (
  'appleton-1996-age', 'appleton-1996', 'age_group_at_baseline', 'confounder',
  true, true,
  'Age at baseline confounds both smoking status and 20-year mortality. Smoking affects age distribution in the cohort (survivorship bias: people who smoked heavily and were older had already died before the 1972-74 baseline survey, so the surviving smokers skew younger). Age strongly determines 20-year mortality regardless of smoking. The result: smokers are over-represented in low-mortality age strata (18-44), pulling the pooled smoker mortality rate below the true within-stratum risk. This is the textbook epidemiological confounding by age pattern, often called the "healthy smoker" fallacy in this particular dataset.'
)
ON CONFLICT (stratum_variable_id) DO UPDATE SET
  study                        = EXCLUDED.study,
  variable_name                = EXCLUDED.variable_name,
  causal_role                  = EXCLUDED.causal_role,
  affects_treatment_assignment = EXCLUDED.affects_treatment_assignment,
  affects_outcome              = EXCLUDED.affects_outcome,
  mechanism_note               = EXCLUDED.mechanism_note;

INSERT INTO treatment_rankings (treatment_ranking_id, study, treatment_a, treatment_b)
VALUES ('appleton-1996-A-vs-B', 'appleton-1996', 'A', 'B')
ON CONFLICT (treatment_ranking_id) DO UPDATE SET
  study       = EXCLUDED.study,
  treatment_a = EXCLUDED.treatment_a,
  treatment_b = EXCLUDED.treatment_b;


-- ============================================================================
-- PHE COVID-19 DELTA VARIANT — UK 2021 (Vaccinated vs Unvaccinated CFR by Age)
-- Source: Public Health England. COVID-19 vaccine surveillance report: Week 42.
--         Technical Briefing 20, August 2021. Dataset packaged by OpenIntro:
--   https://www.openintro.org/data/index.php?data=simpsons_paradox_covid
-- Raw CSV: https://vincentarelbundock.github.io/Rdatasets/csv/openintro/simpsons_paradox_covid.csv
-- N=268,166 cases. Strata: age group (under 50, 50+).
-- Groups: A=vaccinated (includes partially vaccinated), B=unvaccinated.
-- Outcome: death from COVID-19 Delta variant.
-- Vaccinated have LOWER death rate within EACH age group; pooled rate INVERTS
--   (vaccinated appear to have HIGHER death rate).
-- Mechanism: vaccinated individuals were disproportionately older (27,307 of 117,114
--   vaccinated are 50+, vs only 3,440 of 151,052 unvaccinated). The 50+ group has
--   ~40× higher CFR than under-50, so the vaccinated pool is dominated by high-risk
--   patients regardless of vaccine effect.
-- ============================================================================

INSERT INTO studies (study_id, title, source, source_url)
VALUES (
  'phe-covid-2021',
  'COVID-19 Delta Variant CFR: Vaccinated vs Unvaccinated by Age Group, UK 2021 (PHE Technical Briefing 20)',
  'Public Health England. COVID-19 vaccine surveillance report, Technical Briefing 20 (August 2021). Dataset: OpenIntro simpsons_paradox_covid, contributed by Matthew T. Brenneman, Embry-Riddle Aeronautical University.',
  'https://www.openintro.org/data/index.php?data=simpsons_paradox_covid'
)
ON CONFLICT (study_id) DO UPDATE SET
  title      = EXCLUDED.title,
  source     = EXCLUDED.source,
  source_url = EXCLUDED.source_url;

INSERT INTO treatments (treatment_id, study, treatment_label, description)
VALUES
  ('phe-covid-2021-A', 'phe-covid-2021', 'A',
   'Vaccinated (includes partially vaccinated). Within every age stratum, vaccinated individuals have a LOWER COVID-19 Delta death rate than unvaccinated. Pooled death rate appears HIGHER (0.41%) than unvaccinated (0.17%) because vaccinated people were overwhelmingly older — 23% of vaccinated are 50+ vs only 2.3% of unvaccinated, and the 50+ group has ~100× higher CFR.'),
  ('phe-covid-2021-B', 'phe-covid-2021', 'B',
   'Unvaccinated. Within every age stratum, unvaccinated individuals have a HIGHER COVID-19 Delta death rate than vaccinated. Pooled death rate appears LOWER (0.17%) — a statistical artifact of the unvaccinated cohort being overwhelmingly young (97.7% under 50).')
ON CONFLICT (treatment_id) DO UPDATE SET
  study            = EXCLUDED.study,
  treatment_label  = EXCLUDED.treatment_label,
  description      = EXCLUDED.description;

INSERT INTO strata (stratum_id, study, stratum_label, description)
VALUES
  ('phe-covid-2021-under-50', 'phe-covid-2021', 'under 50',
   'Age under 50. Vaccinated CFR: 21/89,807=0.023%. Unvaccinated CFR: 48/147,612=0.033%. Vaccinated clearly better. Low absolute mortality in both groups — under-50 COVID Delta deaths are rare regardless of vaccination status.'),
  ('phe-covid-2021-50plus', 'phe-covid-2021', '50 +',
   'Age 50 and over. Vaccinated CFR: 460/27,307=1.68%. Unvaccinated CFR: 205/3,440=5.96%. Vaccinated dramatically better (3.5× lower death rate). The enormous relative risk reduction in this high-mortality stratum is the true vaccine signal — but it is hidden in the pooled comparison because so few unvaccinated people are in this stratum (3,440 vs 27,307 vaccinated).')
ON CONFLICT (stratum_id) DO UPDATE SET
  study         = EXCLUDED.study,
  stratum_label = EXCLUDED.stratum_label,
  description   = EXCLUDED.description;

-- Raw counts from CSV: https://vincentarelbundock.github.io/Rdatasets/csv/openintro/simpsons_paradox_covid.csv
-- Exact row counts (268,166 total rows) verified by direct CSV enumeration.
-- successes = death (higher = worse outcome).
INSERT INTO case_cells (case_cell_id, study, stratum_label, treatment_label, successes, cases)
VALUES
  ('phe-covid-2021-under-50-A', 'phe-covid-2021', 'under 50', 'A',  21,  89807),
  ('phe-covid-2021-under-50-B', 'phe-covid-2021', 'under 50', 'B',  48, 147612),
  ('phe-covid-2021-50plus-A',   'phe-covid-2021', '50 +',     'A', 460,  27307),
  ('phe-covid-2021-50plus-B',   'phe-covid-2021', '50 +',     'B', 205,   3440)
ON CONFLICT (case_cell_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label,
  successes       = EXCLUDED.successes,
  cases           = EXCLUDED.cases;

INSERT INTO stratum_summaries (stratum_summary_id, study, stratum_label, treatment_label)
VALUES
  ('phe-covid-2021-under-50-A', 'phe-covid-2021', 'under 50', 'A'),
  ('phe-covid-2021-under-50-B', 'phe-covid-2021', 'under 50', 'B'),
  ('phe-covid-2021-50plus-A',   'phe-covid-2021', '50 +',     'A'),
  ('phe-covid-2021-50plus-B',   'phe-covid-2021', '50 +',     'B')
ON CONFLICT (stratum_summary_id) DO UPDATE SET
  study           = EXCLUDED.study,
  stratum_label   = EXCLUDED.stratum_label,
  treatment_label = EXCLUDED.treatment_label;

INSERT INTO stratum_variables (stratum_variable_id, study, variable_name, causal_role,
  affects_treatment_assignment, affects_outcome, mechanism_note)
VALUES (
  'phe-covid-2021-age', 'phe-covid-2021', 'age_group', 'confounder',
  true, true,
  'Age group is a powerful confounder. It affects vaccine uptake (older people were prioritised and had much higher vaccination rates in the UK''s 2021 rollout — 23% of vaccinated are 50+ vs only 2.3% of unvaccinated) and strongly determines COVID-19 mortality (50+ CFR is ~40× higher than under-50 CFR in this dataset regardless of vaccination). The vaccinated cohort is therefore loaded with high-risk patients, inflating the pooled vaccinated death rate to 0.41% vs 0.17% for unvaccinated — reversing the within-stratum relationship where vaccines reduce death rates by 3× to 4× in every age group.'
)
ON CONFLICT (stratum_variable_id) DO UPDATE SET
  study                        = EXCLUDED.study,
  variable_name                = EXCLUDED.variable_name,
  causal_role                  = EXCLUDED.causal_role,
  affects_treatment_assignment = EXCLUDED.affects_treatment_assignment,
  affects_outcome              = EXCLUDED.affects_outcome,
  mechanism_note               = EXCLUDED.mechanism_note;

INSERT INTO treatment_rankings (treatment_ranking_id, study, treatment_a, treatment_b)
VALUES ('phe-covid-2021-A-vs-B', 'phe-covid-2021', 'A', 'B')
ON CONFLICT (treatment_ranking_id) DO UPDATE SET
  study       = EXCLUDED.study,
  treatment_a = EXCLUDED.treatment_a,
  treatment_b = EXCLUDED.treatment_b;
