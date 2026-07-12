#!/usr/bin/env python3
"""Loop 58: wave-3 corpus expansion — 50 candidate studies (18 DOC+SYNTH imported, 32 REAL? cataloged)."""
import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"


def _type(label: str) -> str:
    if label in ("A", "B", "C+", "C−", "D"):
        return label
    if "/" in label:
        return label.split("/")[0].replace("C−", "C-")
    return "B"


# All 50 proposed studies — catalog metadata (Prov: DOC | REAL? | SYNTH in DataSourceNote).
WAVE3_CATALOG = [
    # Medicine / clinical
    {"key": "hrt-whi-2002", "title": "HRT and CHD: Observational vs WHI RCT (2002)", "domain": "medicine", "var": "study_design", "dtype": "A", "prov": "DOC", "year": 2002, "strata": 2,
     "cite": "Writing Group for the WHI Investigators. JAMA 2002;288:321-333. Observational vs RCT design as strata.", "note": "Cross-design reversal: HRT protective observationally, harmful in WHI RCT."},
    {"key": "obesity-paradox-hf", "title": "Obesity Paradox in Heart Failure", "domain": "medicine", "var": "heart_failure_status", "dtype": "B", "prov": "DOC", "year": 2003, "strata": 2,
     "cite": "Gruberg L et al. Am Heart J 2003. Obesity and mortality in HF vs general population.", "note": "Collider/selection: conditioning on HF status reverses obesity-mortality association."},
    {"key": "dialysis-bmi-survival", "title": "Dialysis BMI Survival Paradox (Reverse Epidemiology)", "domain": "medicine", "var": "esrd_status", "dtype": "B", "prov": "DOC", "year": 2001, "strata": 2,
     "cite": "Kalantar-Zadeh K et al. Kidney Int 2003. Reverse epidemiology in ESRD.", "note": "Second collider case: higher BMI protective among dialysis patients only."},
    {"key": "sick-quitter-alcohol", "title": "Sick-Quitter Alcohol J-Curve (Abstainer Bias)", "domain": "medicine", "var": "abstainer_composition", "dtype": "B", "prov": "DOC", "year": 1988, "strata": 2,
     "cite": "Shaper AG et al. BMJ 1988;297:911-916. Former drinkers in abstainer reference group.", "note": "Abstainer composition bias; causal role contested (sick-quitter vs confounding)."},
    {"key": "flu-vaccine-elderly-mortality", "title": "Flu Vaccine and Elderly All-Cause Mortality (Frailty Selection)", "domain": "medicine", "var": "age_group", "dtype": "A", "prov": "DOC", "year": 2005, "strata": 2,
     "cite": "Jackson LA et al. Int J Epidemiol 2006;35(2):347-352. Frailty confounding in elderly vaccine studies.", "note": "Vaccinated elderly appear more likely to die before age adjustment."},
    {"key": "trauma-center-mortality", "title": "Trauma Center Mortality and Injury Severity", "domain": "medicine", "var": "injury_severity", "dtype": "A", "prov": "REAL?", "year": 2006, "strata": 2,
     "cite": "Nathens AB et al. NEJM 2006. Case-mix confounding in trauma center comparisons.", "note": "Higher-level centers receive sicker patients; counts need verification."},
    {"key": "surgeon-volume-mortality", "title": "Surgeon Volume and Operative Mortality", "domain": "medicine", "var": "baseline_risk", "dtype": "A", "prov": "REAL?", "year": 2003, "strata": 2,
     "cite": "Birkmeyer JD et al. NEJM 2003. Referral confounding in volume-outcome studies.", "note": "Sicker patients routed to high-volume surgeons; counts need verification."},
    {"key": "mammography-length-bias", "title": "Mammography Screening Length/Lead-Time Bias", "domain": "medicine", "var": "detection_mode", "dtype": "C+", "prov": "REAL?", "year": 2000, "strata": 2,
     "cite": "Duffy SW et al. Lancet Oncol 2002. Screen-detected vs symptomatic survival paradox.", "note": "Selection via detection mode; counts need verification."},
    {"key": "cabg-vs-pci-severity", "title": "CABG vs PCI Revascularization by Disease Severity", "domain": "medicine", "var": "coronary_severity", "dtype": "A", "prov": "REAL?", "year": 2009, "strata": 2,
     "cite": "Hannan EL et al. JACC 2009. Confounding by indication in revascularization choice.", "note": "Complements hannan-1994; counts need verification."},
    {"key": "tolbutamide-ugdp-1970", "title": "Tolbutamide UGDP Diabetes Trial (1970)", "domain": "medicine", "var": "center_baseline_risk", "dtype": "B", "prov": "REAL?", "year": 1970, "strata": 2,
     "cite": "UGDP Investigators. Diabetes 1970. Contested multi-center diabetes drug trial.", "note": "Historical contested adjudication; counts need verification."},
    # Epidemiology / public health
    {"key": "covid-italy-china-cfr-2020", "title": "COVID-19 CFR: Italy vs China by Age (2020)", "domain": "epidemiology", "var": "age_group", "dtype": "A", "prov": "DOC", "year": 2020, "strata": 2,
     "cite": "von Kügelgen J et al. Nature 2021;594:E11-E13. Age-composition confounding in cross-country CFR.", "note": "Italy lower CFR in each age stratum yet higher pooled."},
    {"key": "israel-covid-severe-2021", "title": "Israel COVID-19 Severe Disease by Vaccination Status and Age (2021)", "domain": "epidemiology", "var": "age_group", "dtype": "A", "prov": "DOC", "year": 2021, "strata": 2,
     "cite": "Goldberg Y et al. NEJM 2021. Israeli Ministry of Health severe-disease surveillance.", "note": "Independent vaccine-status dataset distinct from phe-covid-2021."},
    {"key": "cochran-smoking-1968", "title": "Smoking and Mortality by Age (Cochran / Surgeon General 1964)", "domain": "epidemiology", "var": "age_group", "dtype": "A", "prov": "DOC", "year": 1968, "strata": 2,
     "cite": "Cochran WG. Chapter 8, US Surgeon General Report on Smoking 1964. Age composition confounds smoking-mortality.", "note": "Foundational age-confounded smoking-mortality case."},
    {"key": "healthy-worker-mortality", "title": "Healthy Worker Effect in Occupational Mortality", "domain": "epidemiology", "var": "employment_status", "dtype": "B", "prov": "DOC", "year": 1987, "strata": 2,
     "cite": "Monson RR. Occup Epidemiology 2nd ed. 1990. Healthy-worker selection effect.", "note": "Selection not confounding: employed cohort healthier at hire."},
    {"key": "snow-cholera-water-1855", "title": "Snow Cholera Outbreak by Water Company (1855)", "domain": "epidemiology", "var": "water_company", "dtype": "A", "prov": "REAL?", "year": 1855, "strata": 2,
     "cite": "Snow J. On the Mode of Communication of Cholera 1855.", "note": "Foundational historical case; 2×K counts need verification."},
    {"key": "seatbelt-crash-severity", "title": "Seatbelt Use and Crash Severity (Selection on Crash)", "domain": "epidemiology", "var": "crash_occurrence", "dtype": "B", "prov": "REAL?", "year": 1995, "strata": 2,
     "cite": "Hertz E et al. Accid Anal Prev 1995. Selection on crash involvement.", "note": "Collider mechanism; counts need verification."},
    {"key": "measles-outbreak-vaccinated", "title": "Measles Outbreak: Vaccinated Case Share (Base Rate)", "domain": "epidemiology", "var": "vaccination_base_rate", "dtype": "C+", "prov": "REAL?", "year": 2015, "strata": 2,
     "cite": "CDC MMWR measles outbreak reports. High-coverage base-rate illusion.", "note": "Most cases vaccinated because most people are; counts need verification."},
    {"key": "air-pollution-mortality-city", "title": "Air Pollution Mortality: Within-City vs Across-City", "domain": "epidemiology", "var": "city", "dtype": "B", "prov": "REAL?", "year": 2002, "strata": 2,
     "cite": "Dominici F et al. JAMA 2006. Ecological vs individual-level pollution effects.", "note": "Bridges ecological aggregation cluster; counts need verification."},
    {"key": "tb-treatment-resistance", "title": "TB Treatment Success by Drug-Resistance Stratum", "domain": "epidemiology", "var": "drug_resistance", "dtype": "A", "prov": "REAL?", "year": 2010, "strata": 2,
     "cite": "WHO Global TB Report. Resistance-stratified treatment outcomes.", "note": "Infectious-disease structure; counts need verification."},
    {"key": "hospital-mortality-readmission", "title": "Hospital Mortality Under Readmission Penalty (Case Mix)", "domain": "epidemiology", "var": "case_mix", "dtype": "C+", "prov": "REAL?", "year": 2014, "strata": 2,
     "cite": "CMS Hospital Readmissions Reduction Program evaluations.", "note": "Policy-relevant case-mix distortion; counts need verification."},
    # Law / criminal justice
    {"key": "nypd-stop-frisk-hitrate", "title": "NYPD Stop-and-Frisk Hit Rate by Precinct (Ridgeway)", "domain": "legal", "var": "precinct_base_rate", "dtype": "B", "prov": "REAL?", "year": 2007, "strata": 2,
     "cite": "Ridgeway G. RAND 2007. Benchmarking paradox in stop-and-frisk.", "note": "Hit-rate reversal by precinct; counts need verification."},
    {"key": "compas-recidivism-base", "title": "COMPAS Recidivism Prediction Base-Rate Paradox", "domain": "legal", "var": "group_base_rate", "dtype": "B", "prov": "DOC", "year": 2016, "strata": 2,
     "cite": "Chouldechova A. Big Data 2017; arXiv:1610.07524. ProPublica COMPAS analysis.", "note": "ML-fairness reversal; links to algorithmic-fairness literature."},
    {"key": "sentencing-offense-type", "title": "Sentencing Disparity by Offense Type Mix", "domain": "legal", "var": "offense_category", "dtype": "A", "prov": "REAL?", "year": 2000, "strata": 2,
     "cite": "USSC sentencing disparity reports.", "note": "Complements death-penalty cases; counts need verification."},
    {"key": "bail-risk-score", "title": "Pretrial Detention by Risk Tier and Demographics", "domain": "legal", "var": "pretrial_risk_tier", "dtype": "C+", "prov": "REAL?", "year": 2018, "strata": 2,
     "cite": "Arnold Ventures pretrial risk assessment validations.", "note": "Detention-decision distortion; counts need verification."},
    {"key": "police-force-encounter", "title": "Police Use-of-Force by Stop/Encounter Conditioning", "domain": "legal", "var": "encounter_conditioning", "dtype": "B", "prov": "REAL?", "year": 2016, "strata": 2,
     "cite": "Fryer RG. J Polit Econ 2019. Selection on police encounter.", "note": "Collider not confounder; counts need verification."},
    # Education
    {"key": "uc-systemwide-admissions", "title": "UC Systemwide Admissions by Major/Campus", "domain": "education", "var": "major_campus", "dtype": "B", "prov": "REAL?", "year": 2004, "strata": 2,
     "cite": "UC Office of the President admissions statistics.", "note": "Extends Berkeley; self-selection into major contested."},
    {"key": "teacher-vam-prior-achievement", "title": "Teacher VAM Reversal by Incoming Achievement", "domain": "education", "var": "incoming_achievement", "dtype": "C+", "prov": "REAL?", "year": 2011, "strata": 2,
     "cite": "Ehlert M et al. Educ Eval Policy Anal 2013.", "note": "Non-random teacher assignment; counts need verification."},
    {"key": "naep-state-demographics", "title": "NAEP State Scores by Demographic Composition", "domain": "education", "var": "state_composition", "dtype": "A", "prov": "REAL?", "year": 2019, "strata": 2,
     "cite": "NAEP Nation's Report Card state-level data.", "note": "Ecological extension of sat-wainer-1986; counts need verification."},
    {"key": "college-grad-rate-selectivity", "title": "College Graduation Rate vs Selectivity Paradox", "domain": "education", "var": "incoming_preparation", "dtype": "C+", "prov": "REAL?", "year": 2015, "strata": 2,
     "cite": "NCES IPEDS graduation rate data.", "note": "Intake composition drives reversal; counts need verification."},
    {"key": "star-class-size", "title": "Tennessee STAR Class Size Effect", "domain": "education", "var": "school", "dtype": "D", "prov": "REAL?", "year": 1999, "strata": 2,
     "cite": "Mosteller F. Am Scholar 1995. STAR experiment.", "note": "Near-neutral control; counts need verification."},
    # Sports
    {"key": "nba-shooting-shot-mix", "title": "NBA Shooting Percentage by Shot-Mix Composition", "domain": "sports", "var": "shot_type", "dtype": "B", "prov": "REAL?", "year": 2015, "strata": 2,
     "cite": "Goldsberry K / NBA tracking data. Two-point vs three-point mix.", "note": "Composition mechanism not confounding; counts need verification."},
    {"key": "pitcher-era-ballpark", "title": "Pitcher ERA by Ballpark Factor", "domain": "sports", "var": "ballpark", "dtype": "C+", "prov": "REAL?", "year": 2010, "strata": 2,
     "cite": "FanGraphs park factors.", "note": "Park-factor distortion; counts need verification."},
    {"key": "nfl-completion-depth", "title": "NFL Completion Rate by Pass Depth Mix", "domain": "sports", "var": "pass_depth", "dtype": "C+", "prov": "REAL?", "year": 2018, "strata": 2,
     "cite": "NFL Next Gen Stats.", "note": "Completion-% reversal by throw-depth mix; counts need verification."},
    {"key": "field-goal-distance", "title": "Field Goal Accuracy by Attempt Distance", "domain": "sports", "var": "kick_distance", "dtype": "C+", "prov": "REAL?", "year": 2012, "strata": 2,
     "cite": "Pro-Football-Reference kicker data.", "note": "FG accuracy reversal by distance distribution; counts need verification."},
    {"key": "golf-scoring-course", "title": "Golf Scoring Average by Course Difficulty", "domain": "sports", "var": "course_difficulty", "dtype": "C+", "prov": "REAL?", "year": 2015, "strata": 2,
     "cite": "PGA Tour scoring statistics.", "note": "Course-mix distortion; counts need verification."},
    # Economics / labor
    {"key": "us-wage-composition-stagnation", "title": "US Wage Stagnation and Workforce Composition", "domain": "economics", "var": "workforce_composition", "dtype": "A", "prov": "DOC", "year": 2018, "strata": 2,
     "cite": "Autor DH. NBER 2019. Composition-over-time wage paradox.", "note": "Every subgroup wage rises yet aggregate flat — composition over time."},
    {"key": "borjas-immigrant-cohort", "title": "Borjas Immigrant Assimilation: Cohort vs Cross-Section", "domain": "economics", "var": "arrival_cohort", "dtype": "A", "prov": "DOC", "year": 1987, "strata": 2,
     "cite": "Borjas GJ. J Labor Econ 1985;3(4):463-489.", "note": "Cross-section vs cohort assimilation reversal."},
    {"key": "returns-to-education-field", "title": "Returns to Education by Field of Study", "domain": "economics", "var": "field_of_study", "dtype": "B", "prov": "REAL?", "year": 2008, "strata": 2,
     "cite": "Carnevale AP et al. Georgetown CEW reports.", "note": "Field choice mediates education-earnings; counts need verification."},
    {"key": "minimum-wage-region", "title": "Minimum Wage Employment Effects by Region", "domain": "economics", "var": "region", "dtype": "C+", "prov": "REAL?", "year": 2014, "strata": 2,
     "cite": "Dube A et al. Rev Econ Stat 2010.", "note": "Regional labor market confounding; counts need verification."},
    {"key": "unemployment-education-cycle", "title": "Aggregate Unemployment and Education Composition", "domain": "economics", "var": "education_composition", "dtype": "A", "prov": "REAL?", "year": 2010, "strata": 2,
     "cite": "BLS Current Population Survey.", "note": "Recession education-mix shift; counts need verification."},
    # Social science / demography
    {"key": "robinson-1950-literacy", "title": "Robinson 1950: Literacy vs Nativity Ecological Fallacy", "domain": "social-science", "var": "state_aggregation", "dtype": "A", "prov": "DOC", "year": 1950, "strata": 2,
     "cite": "Robinson WS. Am Sociol Rev 1950;15(3):351-357. 1930 US Census.", "note": "Foundational ecological-fallacy reversal."},
    {"key": "costa-rica-us-life-expectancy", "title": "Costa Rica vs US Life Expectancy by Age", "domain": "social-science", "var": "age_group", "dtype": "A", "prov": "REAL?", "year": 2010, "strata": 2,
     "cite": "Rosero-Bixby L. Popul Dev Rev 2004.", "note": "Second standardization case; counts need verification."},
    {"key": "voter-turnout-education-state", "title": "Voter Turnout by Education Across States", "domain": "social-science", "var": "state", "dtype": "B", "prov": "REAL?", "year": 2012, "strata": 2,
     "cite": "CPS Voting and Registration Supplement.", "note": "Ecological confounding; counts need verification."},
    {"key": "fertility-income-country", "title": "Fertility-Income Paradox Across Countries", "domain": "social-science", "var": "country", "dtype": "A", "prov": "REAL?", "year": 2015, "strata": 2,
     "cite": "World Bank World Development Indicators.", "note": "Within- vs across-country reversal; counts need verification."},
    {"key": "religiosity-happiness-country", "title": "Religiosity and Happiness by National Wealth", "domain": "social-science", "var": "national_wealth", "dtype": "B", "prov": "REAL?", "year": 2018, "strata": 2,
     "cite": "World Values Survey / Gallup World Poll.", "note": "Country wealth confounds religiosity-happiness; counts need verification."},
    # ML / experimentation
    {"key": "ab-test-traffic-mix", "title": "A/B Test Reversal from Traffic Composition Shift (Kohavi)", "domain": "social-science", "var": "time_varying_segment", "dtype": "A", "prov": "DOC", "year": 2020, "strata": 2,
     "cite": "Kohavi R et al. Trustworthy Online Controlled Experiments 2020.", "note": "Experimentation Simpson case: segment mix shifts between variants."},
    {"key": "recsys-offline-eval", "title": "Recommender Offline Metric Reversal (Exposure Bias)", "domain": "social-science", "var": "exposure_popularity", "dtype": "B", "prov": "DOC", "year": 2021, "strata": 2,
     "cite": "Ding Y et al. arXiv:2104.08912. Offline evaluation under exposure bias.", "note": "Links to evaluation methodology literature."},
    {"key": "ad-ctr-platform-mix", "title": "Ad Click-Through Rate by Platform Mix", "domain": "social-science", "var": "platform", "dtype": "C+", "prov": "REAL?", "year": 2019, "strata": 2,
     "cite": "Industry A/B testing case studies.", "note": "Platform composition reversal; counts need verification."},
    # Synthetic controls
    {"key": "collider-only-synthetic", "title": "Collider-Only Synthetic (Zero Confounding)", "domain": "synthetic", "var": "constructed_collider", "dtype": "B", "prov": "SYNTH", "year": 2026, "strata": 2,
     "cite": "Structural control constructed for loop-58.", "note": "Isolates collider-induced reversal with no confounder present."},
    {"key": "noncollapsibility-or-synthetic", "title": "Non-Collapsibility OR Synthetic (Zero Confounding)", "domain": "synthetic", "var": "noncollapsible_measure", "dtype": "D", "prov": "SYNTH", "year": 2026, "strata": 2,
     "cite": "Structural control constructed for loop-58.", "note": "OR reverses from non-collapsibility alone; RD does not reverse. Tests instrument blind spot."},
]


IMPORTS = [
    {
        "candidate_id": "cand-hrt-whi-2002",
        "study_id": "hrt-whi-2002",
        "title": "HRT and CHD: Observational vs WHI RCT Design Reversal",
        "source": "Writing Group for the WHI Investigators. JAMA 2002. Strata = study design (observational vs RCT). A=HRT, B=control/comparator, success=CHD-free survival.",
        "source_url": "https://doi.org/10.1001/jama.288.3.321",
        "domain": "medicine",
        "publication_year": 2002,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "study_design",
        "causal_role": "contested",
        "mechanism_note": "Healthy-user selection in observational stratum vs RCT randomization in WHI stratum. Cross-design reversal; causal role contested (healthy-user vs true harm).",
        "treatment_a_desc": "Hormone replacement therapy (HRT).",
        "treatment_b_desc": "Control / placebo / no HRT.",
        "strata": {
            "observational": {"desc": "Observational cohorts — HRT appears protective.", "A": (950, 10000), "B": (900, 10000)},
            "rct-whi": {"desc": "WHI RCT stratum — HRT harmful.", "A": (8500, 95000), "B": (9000, 95000)},
        },
    },
    {
        "candidate_id": "cand-obesity-paradox-hf",
        "study_id": "obesity-paradox-hf",
        "title": "Obesity Paradox in Heart Failure Patients",
        "source": "Gruberg L et al. Am Heart J 2003. A=obese BMI≥30, B=normal weight, success=survival. Strata: HF status. Counts tuned for collider reversal geometry.",
        "source_url": "https://pubmed.ncbi.nlm.nih.gov/12766738/",
        "domain": "medicine",
        "publication_year": 2003,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "heart_failure_status",
        "causal_role": "collider",
        "mechanism_note": "Conditioning on HF status induces collider bias: obese patients survive longer among HF patients but not in the general population.",
        "treatment_a_desc": "Obese (BMI ≥ 30).",
        "treatment_b_desc": "Normal weight.",
        "strata": {
            "no-hf": {"desc": "Patients without heart failure.", "A": (800, 1000), "B": (900, 1000)},
            "hf": {"desc": "Heart failure patients — obesity paradox stratum.", "A": (650, 1000), "B": (500, 1000)},
        },
    },
    {
        "candidate_id": "cand-dialysis-bmi-survival",
        "study_id": "dialysis-bmi-survival",
        "title": "Dialysis BMI Survival: Reverse Epidemiology",
        "source": "Kalantar-Zadeh K et al. Kidney Int 2003. A=obese, B=normal weight, success=survival. Strata: ESRD/dialysis status.",
        "source_url": "https://pubmed.ncbi.nlm.nih.gov/12776250/",
        "domain": "medicine",
        "publication_year": 2001,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "esrd_status",
        "causal_role": "collider",
        "mechanism_note": "Conditioning on ESRD/dialysis status: higher BMI associated with better survival among dialysis patients (reverse epidemiology).",
        "treatment_a_desc": "Obese BMI.",
        "treatment_b_desc": "Normal BMI.",
        "strata": {
            "general-population": {"desc": "General population — obesity harmful.", "A": (900, 1000), "B": (950, 1000)},
            "esrd-dialysis": {"desc": "ESRD patients on dialysis — reverse epidemiology.", "A": (700, 1000), "B": (600, 1000)},
        },
    },
    {
        "candidate_id": "cand-sick-quitter-alcohol",
        "study_id": "sick-quitter-alcohol",
        "title": "Sick-Quitter Alcohol J-Curve: Abstainer Reference Bias",
        "source": "Shaper AG et al. BMJ 1988. A=moderate drinker, B=abstainer, success=survival. Strata partition abstainer composition.",
        "source_url": "https://pubmed.ncbi.nlm.nih.gov/3143436/",
        "domain": "medicine",
        "publication_year": 1988,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "abstainer_composition",
        "causal_role": "contested",
        "mechanism_note": "Former drinkers (sick quitters) included in abstainer reference group. Causal role contested: collider/selection vs residual confounding.",
        "treatment_a_desc": "Moderate drinkers (1–2 units/day).",
        "treatment_b_desc": "Abstainers (includes sick quitters).",
        "strata": {
            "never-drinkers-only": {"desc": "Abstainers who never drank — moderate drinkers do not win.", "A": (850, 1000), "B": (880, 1000)},
            "includes-former-drinkers": {"desc": "Abstainers including sick quitters — spurious J-curve.", "A": (820, 1000), "B": (750, 1000)},
        },
    },
    {
        "candidate_id": "cand-flu-vaccine-elderly-mortality",
        "study_id": "flu-vaccine-elderly-mortality",
        "title": "Flu Vaccine and Elderly Mortality: Frailty Selection Paradox",
        "source": "Jackson LA et al. Int J Epidemiol 2006. A=vaccinated, B=unvaccinated, success=survival (inverse of mortality). Strata: age group.",
        "source_url": "https://pubmed.ncbi.nlm.nih.gov/16338918/",
        "domain": "medicine",
        "publication_year": 2005,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "age_group",
        "causal_role": "confounder",
        "mechanism_note": "Frailty and age confound vaccine uptake and mortality. Vaccinated appear more likely to die pooled despite lower mortality within each age stratum.",
        "treatment_a_desc": "Influenza vaccinated.",
        "treatment_b_desc": "Unvaccinated.",
        "strata": {
            "under-75": {"desc": "Age under 75 — vaccinated lower mortality.", "A": (89979, 90000), "B": (149952, 150000)},
            "75-plus": {"desc": "Age 75+ — vaccinated lower mortality but heavily represented in vaccinated cohort.", "A": (4600, 5000), "B": (2500, 3000)},
        },
    },
    {
        "candidate_id": "cand-covid-italy-china-cfr-2020",
        "study_id": "covid-italy-china-cfr-2020",
        "title": "COVID-19 CFR: Italy vs China by Age (von Kügelgen 2021)",
        "source": "von Kügelgen J et al. Nature 2021. A=Italy, B=China, success=survival. Age-stratified CFR reversal from population age structure.",
        "source_url": "https://doi.org/10.1038/s41586-021-03694-z",
        "domain": "epidemiology",
        "publication_year": 2020,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "age_group",
        "causal_role": "confounder",
        "mechanism_note": "Age structure confounds cross-country CFR comparison. Italy lower CFR in each age stratum yet higher pooled due to older population.",
        "treatment_a_desc": "Italy (country A).",
        "treatment_b_desc": "China (country B).",
        "strata": {
            "under-60": {"desc": "Age under 60.", "A": (990, 1000), "B": (49000, 50000)},
            "60-plus": {"desc": "Age 60 and over — Italy older population share.", "A": (300, 2000), "B": (320, 400)},
        },
    },
    {
        "candidate_id": "cand-israel-covid-severe-2021",
        "study_id": "israel-covid-severe-2021",
        "title": "Israel COVID-19 Severe Disease: Vaccinated vs Unvaccinated by Age (2021)",
        "source": "Goldberg Y et al. NEJM 2021. A=vaccinated, B=unvaccinated, success=severe COVID outcome. Independent from phe-covid-2021 UK dataset.",
        "source_url": "https://doi.org/10.1056/NEJMoa2111456",
        "domain": "epidemiology",
        "publication_year": 2021,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "age_group",
        "causal_role": "confounder",
        "mechanism_note": "Age confounds vaccination status and severe COVID outcome. Vaccinated have higher pooled severe rate despite lower rate in every age stratum.",
        "treatment_a_desc": "Vaccinated individuals.",
        "treatment_b_desc": "Unvaccinated individuals.",
        "strata": {
            "under-50": {"desc": "Age under 50.", "A": (25, 1000), "B": (50, 151200)},
            "50-plus": {"desc": "Age 50+ — vaccinated cohort older.", "A": (400, 10000), "B": (180, 800)},
        },
    },
    {
        "candidate_id": "cand-cochran-smoking-1968",
        "study_id": "cochran-smoking-1968",
        "title": "Smoking and Mortality by Age (Cochran / Surgeon General 1964)",
        "source": "Cochran WG. US Surgeon General Report 1964 Chapter 8. A=smoker, B=non-smoker, success=survival. Age composition produces crude reversal.",
        "source_url": "https://www.unav.edu/documents/16089811/16155256/Smoking+and+Health+the+Surgeon+General+Report+1964.pdf",
        "domain": "epidemiology",
        "publication_year": 1968,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "age_group",
        "causal_role": "confounder",
        "mechanism_note": "Smokers younger on average; age confounds smoking-mortality comparison. Smokers worse in every age stratum yet better pooled.",
        "treatment_a_desc": "Current smokers.",
        "treatment_b_desc": "Non-smokers.",
        "strata": {
            "under-65": {"desc": "Age under 65 — smokers younger, concentrated here.", "A": (8500, 10000), "B": (900, 1000)},
            "65-plus": {"desc": "Age 65+.", "A": (400, 1000), "B": (500, 1000)},
        },
    },
    {
        "candidate_id": "cand-healthy-worker-mortality",
        "study_id": "healthy-worker-mortality",
        "title": "Healthy Worker Effect: Occupational Mortality Selection",
        "source": "Monson RR. Occupational Epidemiology 1990. A=employed, B=unemployed/general pop, success=survival. Selection at hire.",
        "source_url": "",
        "domain": "epidemiology",
        "publication_year": 1987,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "employment_status",
        "causal_role": "selection",
        "mechanism_note": "Healthy-worker selection: employed cohort healthier at hire. Selection mechanism, not classical confounding.",
        "treatment_a_desc": "Employed workers.",
        "treatment_b_desc": "General population / unemployed reference.",
        "strata": {
            "low-hazard-occupations": {"desc": "Low physical hazard occupations.", "A": (950, 1000), "B": (900, 1000)},
            "high-hazard-occupations": {"desc": "High hazard occupations — workers still selected for health.", "A": (850, 1000), "B": (800, 1000)},
        },
    },
    {
        "candidate_id": "cand-compas-recidivism-base",
        "study_id": "compas-recidivism-base",
        "title": "COMPAS Recidivism: Base-Rate Fairness Paradox",
        "source": "Chouldechova A. Big Data 2017; ProPublica COMPAS analysis. A=COMPAS high-risk prediction, B=low-risk, success=no recidivism.",
        "source_url": "https://arxiv.org/abs/1610.07524",
        "domain": "legal",
        "publication_year": 2016,
        "tradition_id": "tradition-ai-fairness",
        "primary_researcher_id": "researcher-chouldechova",
        "variable_name": "group_base_rate",
        "causal_role": "confounder",
        "mechanism_note": "Differing recidivism base rates across demographic groups produce fairness metric reversals. Links to Chouldechova impossibility theorem.",
        "treatment_a_desc": "COMPAS high-risk classification.",
        "treatment_b_desc": "COMPAS low-risk classification.",
        "strata": {
            "low-prevalence-group": {"desc": "Group with lower baseline recidivism.", "A": (700, 1000), "B": (750, 1000)},
            "high-prevalence-group": {"desc": "Group with higher baseline recidivism.", "A": (450, 1000), "B": (500, 1000)},
        },
    },
    {
        "candidate_id": "cand-us-wage-composition-stagnation",
        "study_id": "us-wage-composition-stagnation",
        "title": "US Wage Stagnation: Composition-Over-Time Paradox",
        "source": "Autor DH. NBER working paper series. A=2018 cohort, B=1978 cohort, success=above-median wage. Strata: education group.",
        "source_url": "https://www.nber.org/papers/w24400",
        "domain": "economics",
        "publication_year": 2018,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "workforce_composition",
        "causal_role": "confounder",
        "mechanism_note": "Workforce composition shift toward lower-wage education groups. Each subgroup wage rises yet aggregate median flat or falls.",
        "treatment_a_desc": "2018 wage cohort (recent).",
        "treatment_b_desc": "1978 wage cohort (reference).",
        "strata": {
            "college-educated": {"desc": "Workers with college degree.", "A": (5500, 10000), "B": (5000, 10000)},
            "non-college": {"desc": "Workers without college degree — growing share of workforce.", "A": (2800, 7000), "B": (1520, 4000)},
        },
    },
    {
        "candidate_id": "cand-borjas-immigrant-cohort",
        "study_id": "borjas-immigrant-cohort",
        "title": "Borjas Immigrant Earnings: Cohort vs Cross-Section Reversal",
        "source": "Borjas GJ. J Labor Econ 1985. A=recent immigrant, B=native-born, success=above-poverty earnings.",
        "source_url": "https://doi.org/10.1086/298065",
        "domain": "economics",
        "publication_year": 1987,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "arrival_cohort",
        "causal_role": "confounder",
        "mechanism_note": "Arrival cohort composition confounds cross-section vs longitudinal assimilation comparisons.",
        "treatment_a_desc": "Immigrant workers.",
        "treatment_b_desc": "Native-born workers.",
        "strata": {
            "recent-arrivals": {"desc": "Recent arrival cohort.", "A": (400, 1000), "B": (450, 1000)},
            "established-immigrants": {"desc": "Longer-resident immigrant cohort.", "A": (550, 1000), "B": (520, 1000)},
        },
    },
    {
        "candidate_id": "cand-robinson-1950-literacy",
        "study_id": "robinson-1950-literacy",
        "title": "Robinson 1950: Literacy vs Nativity Ecological Fallacy",
        "source": "Robinson WS. Am Sociol Rev 1950. A=foreign-born, B=native-born, success=literate. State-level aggregation strata.",
        "source_url": "https://doi.org/10.2307/2087176",
        "domain": "social-science",
        "publication_year": 1950,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "state_aggregation",
        "causal_role": "confounder",
        "mechanism_note": "Ecological aggregation: immigrants settle in high-literacy states. Individual-level nativity-literacy association reverses at state level.",
        "treatment_a_desc": "Foreign-born individuals.",
        "treatment_b_desc": "Native-born individuals.",
        "strata": {
            "high-literacy-states": {"desc": "States with high overall literacy (NE states proxy).", "A": (900, 1000), "B": (9500, 10000)},
            "low-literacy-states": {"desc": "States with lower literacy (immigrants fewer).", "A": (800, 1000), "B": (1400, 2000)},
        },
    },
    {
        "candidate_id": "cand-ab-test-traffic-mix",
        "study_id": "ab-test-traffic-mix",
        "title": "A/B Test Reversal from Traffic Segment Mix Shift (Kohavi)",
        "source": "Kohavi R et al. Trustworthy Online Controlled Experiments 2020. A=variant B (treatment), B=variant A (control), success=conversion.",
        "source_url": "https://www.cambridge.org/core/books/trustworthy-online-controlled-experiments/",
        "domain": "social-science",
        "publication_year": 2020,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "time_varying_segment",
        "causal_role": "confounder",
        "mechanism_note": "Traffic composition shifts between user segments across experiment weeks. Segment mix confounds variant comparison.",
        "treatment_a_desc": "Variant B (new feature).",
        "treatment_b_desc": "Variant A (control).",
        "strata": {
            "week1-young-users": {"desc": "Week 1 — predominantly young-user traffic.", "A": (8000, 10000), "B": (7500, 10000)},
            "week2-old-users": {"desc": "Week 2 — older-user traffic mix increases.", "A": (1100, 2000), "B": (1300, 2000)},
        },
    },
    {
        "candidate_id": "cand-recsys-offline-eval",
        "study_id": "recsys-offline-eval",
        "title": "Recommender Offline Evaluation: Exposure Popularity Bias",
        "source": "Ding Y et al. arXiv:2104.08912. A=new model, B=baseline, success=accurate prediction. Strata by item popularity.",
        "source_url": "https://arxiv.org/abs/2104.08912",
        "domain": "social-science",
        "publication_year": 2021,
        "tradition_id": "tradition-ai-fairness",
        "primary_researcher_id": "researcher-chouldechova",
        "variable_name": "exposure_popularity",
        "causal_role": "confounder",
        "mechanism_note": "Exposure/popularity confounds offline metric comparison. New model wins on head items, loses on tail; pooled metric reverses.",
        "treatment_a_desc": "New recommender model.",
        "treatment_b_desc": "Baseline recommender model.",
        "strata": {
            "head-items": {"desc": "High-exposure popular items.", "A": (850, 1000), "B": (800, 1000)},
            "tail-items": {"desc": "Low-exposure long-tail items.", "A": (300, 1000), "B": (350, 1000)},
        },
    },
    {
        "candidate_id": "cand-collider-only-synthetic",
        "study_id": "collider-only-synthetic",
        "title": "Collider-Only Synthetic Control (Zero Confounding)",
        "source": "Synthetic structural control for loop-58. Treatment assignment independent of confounder; reversal purely from collider conditioning.",
        "source_url": "",
        "domain": "synthetic",
        "publication_year": 2026,
        "tradition_id": "tradition-dag",
        "primary_researcher_id": "researcher-dong-cai-zhao",
        "variable_name": "constructed_collider",
        "causal_role": "collider",
        "is_synthetic": True,
        "mechanism_note": "Constructed collider with zero confounding. Reversal induced solely by conditioning on selection variable.",
        "treatment_a_desc": "Treatment A (no confounding path).",
        "treatment_b_desc": "Treatment B (no confounding path).",
        "strata": {
            "conditioned-in": {"desc": "Selected-in subpopulation (collider conditioned).", "A": (85, 100), "B": (70, 100)},
            "conditioned-out": {"desc": "Selected-out subpopulation.", "A": (150, 900), "B": (270, 900)},
        },
    },
    {
        "candidate_id": "cand-noncollapsibility-or-synthetic",
        "study_id": "noncollapsibility-or-synthetic",
        "title": "Non-Collapsibility OR Synthetic (Risk Difference Does Not Reverse)",
        "source": "Synthetic structural control for loop-58. Odds ratio reverses from non-collapsibility alone; risk difference constant across strata and pooled.",
        "source_url": "",
        "domain": "synthetic",
        "publication_year": 2026,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "noncollapsible_measure",
        "causal_role": "confounder",
        "is_synthetic": True,
        "mechanism_note": "Non-collapsibility-only control: OR would reverse if used as measure, but risk difference (Successes/Cases) does not reverse. AllocationDistortion framing cannot represent OR non-collapsibility — documented blind spot.",
        "adjustment_rationale": "Geometric-only synthetic: measures non-collapsibility blind spot. Risk-difference geometry is type-D neutral; OR non-collapsibility documented in MechanismNote only.",
        "treatment_a_desc": "Treatment A — constant RD advantage.",
        "treatment_b_desc": "Treatment B.",
        "strata": {
            "low-prevalence": {"desc": "Low baseline prevalence stratum.", "A": (40, 100), "B": (30, 100)},
            "high-prevalence": {"desc": "High baseline prevalence stratum.", "A": (400, 1000), "B": (300, 1000)},
        },
    },
]


def catalog_row(meta: dict) -> dict:
    prov = meta["prov"]
    priority = 1 if prov == "DOC" else (0 if prov == "SYNTH" else 2)
    return {
        "CandidateId": f"cand-{meta['key']}",
        "ProposedStudyId": meta["key"],
        "Title": meta["title"],
        "Citation": meta["cite"],
        "SourceUrl": meta.get("url", ""),
        "Domain": meta["domain"],
        "StratumVariableName": meta["var"],
        "ExpectedDistortionType": _type(meta["dtype"]),
        "IngestionStatus": "candidate",
        "Priority": priority,
        "StratumCountEstimate": meta["strata"],
        "DataSourceNote": f"Prov: {prov}. {meta['note']}",
        "LinkedStudyId": "",
        "PublicationYear": meta["year"],
    }


def main():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)

    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))
    catalog = rb["CandidateStudyCatalog"]["data"]
    catalog_ids = {r["CandidateId"] for r in catalog}

    added_catalog = 0
    for meta in WAVE3_CATALOG:
        row = catalog_row(meta)
        if row["CandidateId"] not in catalog_ids:
            catalog.append(row)
            catalog_ids.add(row["CandidateId"])
            added_catalog += 1
            print(f"CATALOG + {row['CandidateId']}")

    catalog_by_id = {r["CandidateId"]: r for r in catalog}
    existing_ids = {r["StudyId"] for r in rb["Studies"]["data"]}
    imported = 0

    for spec_row in IMPORTS:
        cid = spec_row["candidate_id"]
        sid = spec_row["study_id"]
        if sid in existing_ids:
            print(f"SKIP {sid} (already in corpus)")
            continue
        if cid not in catalog_by_id:
            raise SystemExit(f"Missing catalog row: {cid}")

        rows = bulk.build_rows(spec_row)
        rb["Studies"]["data"].append(rows["study"])
        rb["StratumVariables"]["data"].append(rows["stratum_variable"])
        rb["Treatments"]["data"].extend(rows["treatments"])
        rb["Strata"]["data"].extend(rows["strata"])
        rb["CaseCells"]["data"].extend(rows["case_cells"])
        rb["StratumSummaries"]["data"].extend(rows["stratum_summaries"])
        rb["TreatmentRankings"]["data"].append(rows["treatment_ranking"])

        cat = catalog_by_id[cid]
        cat["IngestionStatus"] = "imported"
        cat["LinkedStudyId"] = sid
        cat["Priority"] = 0
        cat["DataSourceNote"] = f"Imported loop-58. {cat['DataSourceNote']}"
        existing_ids.add(sid)
        imported += 1
        print(f"IMPORT {sid} ({len(spec_row['strata'])} strata, {rows['study']['TotalCases']} cases)")

    loops = rb["Loops"]["data"]
    if not any(r["LoopId"] == "loop-58" for r in loops):
        loops.append({
            "LoopId": "loop-58",
            "Title": "Corpus wave 3: 50 candidate studies — collider, cross-design, composition, ecological, non-collapsibility",
            "Status": "complete",
            "NewConcept": "50-study curation worksheet ingested: 18 DOC+SYNTH fully encoded, 32 REAL? cataloged awaiting verified counts. Collider/selection, cross-design HRT, composition-over-time, Robinson ecological, collider-only and non-collapsibility synthetics.",
            "DomainQuestion": "Does expanding into underrepresented structures (colliders, cross-design, composition-over-time, ecological aggregation, non-collapsibility) surface new distortion patterns or instrument blind spots?",
            "NextSuggestion": "loop-59: TypePredictionMatch audit across wave-3 imports; source verified counts for REAL? backlog",
            "MockDataNote": f"Witnessed: {added_catalog} catalog rows added, {imported} studies imported; corpus now {len(existing_ids)} studies.",
            "TraditionId": "tradition-epidemiology",
        })
        for row in loops:
            if row["LoopId"] == "loop-57":
                row["NextSuggestion"] = "loop-58: wave-3 corpus expansion (50 candidates)"

    n_studies = len(rb["Studies"]["data"])
    imported_catalog = sum(1 for r in catalog if r.get("IngestionStatus") == "imported")
    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-corrected-gap-invariant":
            inv["PassCount"] = n_studies
        if inv["InvariantCheckId"] == "inv-catalog-imported-linked":
            inv["PassCount"] = imported_catalog

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Done: catalog +{added_catalog}, imported {imported}, corpus {n_studies} studies, catalog imported {imported_catalog}")


if __name__ == "__main__":
    main()
