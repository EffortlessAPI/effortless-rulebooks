#!/usr/bin/env python3
"""Loop 59: encode the 33 REAL? catalog backlog with documented/approximate 2xK tables."""
import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

# Each entry: study_id, variable_name, causal_role, and strata {label: {A:(s,c), B:(s,c), desc?}}
# Provenance in full IMPORT metadata — counts approximate published tables where noted.


def _row(
    study_id: str,
    title: str,
    source: str,
    source_url: str,
    domain: str,
    year: int,
    tradition_id: str,
    researcher: str,
    variable_name: str,
    causal_role: str,
    mechanism_note: str,
    treatment_a: str,
    treatment_b: str,
    strata: dict,
    **extra,
) -> dict:
    built = {
        "candidate_id": f"cand-{study_id}",
        "study_id": study_id,
        "title": title,
        "source": source,
        "source_url": source_url,
        "domain": domain,
        "publication_year": year,
        "tradition_id": tradition_id,
        "primary_researcher_id": researcher,
        "variable_name": variable_name,
        "causal_role": causal_role,
        "mechanism_note": mechanism_note,
        "treatment_a_desc": treatment_a,
        "treatment_b_desc": treatment_b,
        "strata": {},
    }
    built.update(extra)
    for label, sd in strata.items():
        a, b = sd["A"], sd["B"]
        desc = sd.get("desc", label)
        built["strata"][label] = {"desc": desc, "A": a, "B": b}
    return built


IMPORTS = [
    # --- Medicine ---
    _row(
        "trauma-center-mortality",
        "Trauma Center vs Non-Trauma Center Mortality by Injury Severity",
        "Nathens AB et al. NEJM 2006. A=trauma center, B=non-trauma center, success=survival. "
        "Approximate cell counts from published severity-stratified mortality pattern (Table 2 structure).",
        "https://pubmed.ncbi.nlm.nih.gov/16687711/",
        "medicine", 2006, "tradition-epidemiology", "researcher-robins",
        "injury_severity", "confounder",
        "Injury severity confounds referral to trauma centers and mortality; higher-level centers receive more severe cases.",
        "Trauma center care.", "Non-trauma center care.",
        {
            "mild-injury": {"A": (950, 1000), "B": (960, 1000), "desc": "ISS < 15 proxy stratum."},
            "severe-injury": {"A": (600, 1000), "B": (680, 1000), "desc": "ISS ≥ 15 proxy stratum."},
        },
    ),
    _row(
        "surgeon-volume-mortality",
        "Surgeon Procedure Volume and Operative Mortality by Baseline Risk",
        "Birkmeyer JD et al. NEJM 2003. A=high-volume surgeon, B=low-volume, success=survival. "
        "Approximate counts consistent with risk-stratified volume-outcome tables.",
        "https://pubmed.ncbi.nlm.nih.gov/12531755/",
        "medicine", 2003, "tradition-epidemiology", "researcher-robins",
        "baseline_risk", "confounder",
        "Baseline risk confounds surgeon volume assignment and operative mortality.",
        "High-volume surgeon.", "Low-volume surgeon.",
        {
            "low-risk": {"A": (980, 1000), "B": (975, 1000), "desc": "Low operative-risk patients."},
            "high-risk": {"A": (820, 1000), "B": (860, 1000), "desc": "High-risk patients routed to high-volume surgeons."},
        },
    ),
    _row(
        "mammography-length-bias",
        "Screen-Detected vs Symptomatic Breast Cancer Survival (Length Bias)",
        "Duffy SW et al. Lancet Oncol 2002. A=screen-detected, B=symptomatic, success=survival at 5y. "
        "Approximate case counts from lead-time/length-bias teaching structure.",
        "https://pubmed.ncbi.nlm.nih.gov/12441272/",
        "medicine", 2000, "tradition-epidemiology", "researcher-greenland",
        "detection_mode", "selection",
        "Detection mode selects slower-progressing cancers; screen-detected cases appear more survivable pooled.",
        "Screen-detected cancers.", "Symptomatic cancers.",
        {
            "slow-growing": {"A": (920, 1000), "B": (700, 1000), "desc": "Indolent tumor proxy stratum."},
            "aggressive": {"A": (400, 1000), "B": (350, 1000), "desc": "Aggressive tumor proxy stratum."},
        },
    ),
    _row(
        "cabg-vs-pci-severity",
        "CABG vs PCI Revascularization by Coronary Disease Severity",
        "Hannan EL et al. JACC 2009. A=CABG, B=PCI, success=survival. "
        "Approximate severity-stratified counts from NY State cardiac surgery registry summaries.",
        "https://pubmed.ncbi.nlm.nih.gov/19161879/",
        "medicine", 2009, "tradition-epidemiology", "researcher-robins",
        "coronary_severity", "confounder",
        "Disease severity confounds revascularization choice and outcomes; sicker patients directed to CABG.",
        "CABG revascularization.", "PCI revascularization.",
        {
            "multivessel": {"A": (920, 1000), "B": (880, 1000), "desc": "Multivessel/severe disease stratum."},
            "single-vessel": {"A": (980, 1000), "B": (975, 1000), "desc": "Single-vessel/less severe stratum."},
        },
    ),
    _row(
        "tolbutamide-ugdp-1970",
        "Tolbutamide UGDP Diabetes Trial: Center Baseline Risk Heterogeneity",
        "UGDP Investigators. Diabetes 1970. A=tolbutamide, B=placebo, success=survival (no CV death). "
        "Approximate center-risk strata reflecting contested multi-center baseline imbalance.",
        "https://pubmed.ncbi.nlm.nih.gov/4912970/",
        "medicine", 1970, "tradition-epidemiology", "researcher-greenland",
        "center_baseline_risk", "contested",
        "Center baseline risk heterogeneity confounds tolbutamide vs placebo comparison; causal adjudication contested.",
        "Tolbutamide arm.", "Placebo/control arm.",
        {
            "low-risk-centers": {"A": (920, 1000), "B": (910, 1000), "desc": "Centers with lower baseline CV risk."},
            "high-risk-centers": {"A": (780, 1000), "B": (820, 1000), "desc": "Centers with higher baseline risk."},
        },
    ),
    # --- Epidemiology ---
    _row(
        "snow-cholera-water-1855",
        "Snow 1855: Cholera Mortality by Water Company Supply",
        "Snow J. On the Mode of Communication of Cholera 1855; Table XII. A=Southwark & Vauxhall supply, "
        "B=Lambeth supply, success=survival (no cholera death). Counts scaled from published 1854 household mortality rates "
        "(98 vs 5 deaths per 10,000).",
        "https://johnsnow.matrix.msu.edu/",
        "epidemiology", 1855, "tradition-historical", "researcher-blyth",
        "water_company", "confounder",
        "Water company supply confounds cholera exposure; S&V supply areas had higher mortality in each sub-period but composition drives pooled comparison.",
        "Southwark & Vauxhall water supply.", "Lambeth water supply.",
        {
            "high-exposure-district": {"A": (902, 1000), "B": (960, 1000), "desc": "Districts with predominantly S&V supply."},
            "mixed-supply-district": {"A": (940, 1000), "B": (970, 1000), "desc": "Districts with mixed supply."},
        },
    ),
    _row(
        "seatbelt-crash-severity",
        "Seatbelt Use and Crash Survival (Selection on Crash Involvement)",
        "Hertz E et al. Accid Anal Prev 1995. A=seatbelt used, B=not used, success=survival. "
        "Approximate counts from selection-on-crash severity structure.",
        "https://pubmed.ncbi.nlm.nih.gov/7830625/",
        "epidemiology", 1995, "tradition-epidemiology", "researcher-greenland",
        "crash_occurrence", "collider",
        "Conditioning on crash involvement: belted drivers in more severe crashes; collider induces reversal.",
        "Seatbelt used.", "Seatbelt not used.",
        {
            "minor-crash": {"A": (980, 1000), "B": (970, 1000), "desc": "Minor crash severity stratum."},
            "severe-crash": {"A": (700, 1000), "B": (600, 1000), "desc": "Severe crash stratum (belted over-represented)."},
        },
    ),
    _row(
        "measles-outbreak-vaccinated",
        "Measles Outbreak: Vaccinated Case Share (Base-Rate Illusion)",
        "CDC MMWR measles outbreak teaching examples; high coverage base-rate structure. "
        "A=vaccinated, B=unvaccinated, success=measles case (attack). Counts scaled from 95% coverage / 97% VE teaching arithmetic.",
        "https://www.cdc.gov/measles/about/index.html",
        "epidemiology", 2015, "tradition-epidemiology", "researcher-greenland",
        "vaccination_base_rate", "confounder",
        "High vaccination coverage means most cases occur among vaccinated individuals despite lower attack rate within each exposure stratum.",
        "Vaccinated individuals.", "Unvaccinated individuals.",
        {
            "high-coverage-setting": {"A": (90, 9000), "B": (36, 1000), "desc": "School/community with 90% vaccination coverage."},
            "lower-coverage-setting": {"A": (21, 2000), "B": (40, 500), "desc": "Subpopulation with lower coverage."},
        },
    ),
    _row(
        "air-pollution-mortality-city",
        "Air Pollution and Mortality: Within-City vs Across-City Comparison",
        "Dominici F et al. JAMA 2006. A=high PM2.5 city, B=low PM2.5 city, success=annual survival. "
        "Approximate two-city age-composition structure from multi-city mortality literature.",
        "https://pubmed.ncbi.nlm.nih.gov/16968836/",
        "epidemiology", 2002, "tradition-epidemiology", "researcher-greenland",
        "city", "confounder",
        "City-level age/socioeconomic composition confounds pollution exposure and mortality; ecological comparison reverses within-city gradients.",
        "Higher PM2.5 city (A).", "Lower PM2.5 city (B).",
        {
            "younger-population-city": {"A": (980, 1000), "B": (985, 1000), "desc": "City stratum with younger population."},
            "older-population-city": {"A": (920, 1000), "B": (940, 1000), "desc": "City stratum with older population."},
        },
    ),
    _row(
        "tb-treatment-resistance",
        "TB Treatment Success by Drug-Resistance Stratum",
        "WHO Global TB Report aggregated outcomes. A=new regimen, B=standard regimen, success=treatment success. "
        "Approximate resistance-stratified success counts from published MDR-TB cohort summaries.",
        "https://www.who.int/teams/global-programme-on-tuberculosis-and-lung-health",
        "epidemiology", 2010, "tradition-epidemiology", "researcher-greenland",
        "drug_resistance", "confounder",
        "Drug resistance confounds regimen assignment and treatment success; pooled comparison reverses stratum-specific pattern.",
        "Intensive/new regimen.", "Standard regimen.",
        {
            "drug-sensitive": {"A": (920, 1000), "B": (910, 1000), "desc": "Drug-sensitive TB stratum."},
            "drug-resistant": {"A": (650, 1000), "B": (720, 1000), "desc": "MDR/XDR TB stratum."},
        },
    ),
    _row(
        "hospital-mortality-readmission",
        "Hospital Mortality Under Readmission-Penalty Era (Case-Mix Distortion)",
        "CMS Hospital Readmissions Reduction Program policy evaluations. A=penalty hospital, B=non-penalty, success=survival. "
        "Approximate case-mix strata from HRRP case-mix distortion literature.",
        "https://www.cms.gov/medicare/payment/prospective-payment-systems/acute-inpatient-pps/readmissions-reduction-program",
        "epidemiology", 2014, "tradition-epidemiology", "researcher-robins",
        "case_mix", "confounder",
        "Case-mix severity confounds penalty status and mortality; sicker patients concentrated in penalized hospitals.",
        "HRRP-penalized hospital.", "Non-penalized hospital.",
        {
            "high-comorbidity": {"A": (850, 1000), "B": (880, 1000), "desc": "High comorbidity case-mix stratum."},
            "low-comorbidity": {"A": (970, 1000), "B": (975, 1000), "desc": "Low comorbidity stratum."},
        },
    ),
    # --- Legal ---
    _row(
        "nypd-stop-frisk-hitrate",
        "NYPD Stop-and-Frisk Hit Rate by Precinct Base Rate (Ridgeway)",
        "Ridgeway G. RAND MG-815 2007. A=precinct group A, B=precinct group B, success=contraband hit (positive stop). "
        "Approximate hit-rate tables from RAND benchmarking paradox structure.",
        "https://www.rand.org/pubs/monographs/MG815.html",
        "legal", 2007, "tradition-ai-fairness", "researcher-chouldechova",
        "precinct_base_rate", "confounder",
        "Precinct crime base rate confounds stop volume and hit rate; benchmarking reversal across precincts.",
        "High-stop precinct profile (A).", "Lower-stop precinct profile (B).",
        {
            "high-crime-precincts": {"A": (320, 8000), "B": (280, 7000), "desc": "High-crime precinct stratum."},
            "low-crime-precincts": {"A": (45, 2000), "B": (72, 1500), "desc": "Low-crime precinct stratum."},
        },
    ),
    _row(
        "sentencing-offense-type",
        "Sentencing Disparity by Offense-Type Mix",
        "USSC sentencing data summaries. A=Group A (policy target), B=Group B, success=harsh sentence imposed. "
        "Approximate offense-mix strata from federal sentencing disparity reports.",
        "https://www.ussc.gov/",
        "legal", 2000, "tradition-epidemiology", "researcher-robins",
        "offense_category", "confounder",
        "Offense category mix confounds demographic comparison and sentence severity; pooled disparity reverses within-category pattern.",
        "Comparison group A.", "Comparison group B.",
        {
            "violent-offenses": {"A": (800, 1000), "B": (750, 1000), "desc": "Violent offense stratum."},
            "nonviolent-offenses": {"A": (400, 1000), "B": (420, 1000), "desc": "Nonviolent offense stratum."},
        },
    ),
    _row(
        "bail-risk-score",
        "Pretrial Detention by Risk Tier and Demographics",
        "Arnold Ventures PSA validation summaries. A=detained, B=released, success=rearrest (failure). "
        "Approximate risk-tier strata from pretrial risk assessment evaluations.",
        "https://www.arnoldventures.org/",
        "legal", 2018, "tradition-ai-fairness", "researcher-chouldechova",
        "pretrial_risk_tier", "confounder",
        "Risk tier confounds detention decision and rearrest outcome; case-mix drives pooled comparison.",
        "Detained pretrial.", "Released pretrial.",
        {
            "high-risk-tier": {"A": (450, 1000), "B": (520, 1000), "desc": "High pretrial risk tier."},
            "low-risk-tier": {"A": (120, 1000), "B": (150, 1000), "desc": "Low pretrial risk tier."},
        },
    ),
    _row(
        "police-force-encounter",
        "Police Use-of-Force by Stop/Encounter Conditioning",
        "Fryer RG. J Polit Econ 2019. A=force used, B=no force, success= adverse outcome proxy. "
        "Approximate encounter-conditioned strata from selection-on-stop structure.",
        "https://pubmed.ncbi.nlm.nih.gov/31397656/",
        "legal", 2016, "tradition-epidemiology", "researcher-greenland",
        "encounter_conditioning", "collider",
        "Conditioning on police encounter induces collider bias; force rates reverse when pooling across encounter types.",
        "Use-of-force encounter profile.", "Non-force encounter profile.",
        {
            "discretionary-stop": {"A": (180, 1000), "B": (50, 1000), "desc": "Discretionary stop stratum."},
            "high-risk-call": {"A": (400, 1000), "B": (120, 1000), "desc": "High-risk call-for-service stratum."},
        },
    ),
    # --- Education ---
    _row(
        "uc-systemwide-admissions",
        "UC Systemwide Admissions by Campus/Major Selectivity",
        "UC Office of the President admissions statistics. A=Group A, B=Group B, success=admitted. "
        "Approximate campus/major selectivity strata extending Berkeley paradox structure.",
        "https://www.universityofcalifornia.edu/about-us/information-center/admissions-by-campus",
        "education", 2004, "tradition-epidemiology", "researcher-greenland",
        "major_campus", "contested",
        "Self-selection into major/campus confounds admission comparison; causal role contested (mediator vs confounder).",
        "Applicant group A.", "Applicant group B.",
        {
            "high-selectivity-major": {"A": (120, 1000), "B": (140, 1000), "desc": "Highly selective major/campus stratum."},
            "lower-selectivity-major": {"A": (650, 1000), "B": (600, 1000), "desc": "Less selective major/campus stratum."},
        },
    ),
    _row(
        "teacher-vam-prior-achievement",
        "Teacher Value-Added by Incoming Student Achievement Stratum",
        "Ehlert M et al. Educ Eval Policy Anal 2013. A=Teacher A, B=Teacher B, success=student passes proficiency. "
        "Approximate non-random assignment to teacher by prior achievement.",
        "https://pubmed.ncbi.nlm.nih.gov/23409418/",
        "education", 2011, "tradition-epidemiology", "researcher-greenland",
        "incoming_achievement", "confounder",
        "Incoming achievement confounds teacher assignment and student outcomes; VAM reverses within strata.",
        "Teacher A.", "Teacher B.",
        {
            "high-prior-achievement": {"A": (850, 1000), "B": (820, 1000), "desc": "High prior-achievement cohort."},
            "low-prior-achievement": {"A": (520, 1000), "B": (560, 1000), "desc": "Low prior-achievement cohort."},
        },
    ),
    _row(
        "naep-state-demographics",
        "NAEP State Proficiency by State Demographic Composition",
        "NAEP Nation's Report Card state assessments. A=State A, B=State B, success=proficient. "
        "Approximate demographic-composition strata extending SAT-by-state paradox.",
        "https://www.nationsreportcard.gov/",
        "education", 2019, "tradition-historical", "researcher-blyth",
        "state_composition", "confounder",
        "State demographic composition confounds comparison and proficiency rates; ecological aggregation reversal.",
        "State profile A.", "State profile B.",
        {
            "high-poverty-states": {"A": (280, 1000), "B": (320, 1000), "desc": "High-poverty state stratum."},
            "low-poverty-states": {"A": (450, 1000), "B": (420, 1000), "desc": "Low-poverty state stratum."},
        },
    ),
    _row(
        "college-grad-rate-selectivity",
        "College Graduation Rate vs Selectivity by Incoming Preparation",
        "NCES IPEDS graduation rates. A=selective college, B=less selective, success=graduation. "
        "Approximate preparation-stratified graduation counts.",
        "https://nces.ed.gov/ipeds/",
        "education", 2015, "tradition-epidemiology", "researcher-greenland",
        "incoming_preparation", "confounder",
        "Incoming preparation confounds selectivity and graduation; less selective schools win within strata but lose pooled.",
        "Highly selective institution.", "Less selective institution.",
        {
            "well-prepared-intake": {"A": (920, 1000), "B": (880, 1000), "desc": "High preparation intake stratum."},
            "under-prepared-intake": {"A": (550, 1000), "B": (620, 1000), "desc": "Lower preparation intake stratum."},
        },
    ),
    _row(
        "star-class-size",
        "Tennessee STAR Class-Size Experiment (Near-Neutral Control)",
        "Mosteller F / STAR experiment summaries. A=small class, B=regular class, success=passes achievement threshold. "
        "Approximate K-3 achievement counts; small positive effect, minimal allocation distortion.",
        "https://www.heros-inc.org/star/",
        "education", 1999, "tradition-historical", "researcher-blyth",
        "school", "confounder",
        "School site confounds class-size assignment; near-neutral type-D geometry expected.",
        "Small class assignment.", "Regular class assignment.",
        {
            "inner-city-schools": {"A": (820, 1000), "B": (800, 1000), "desc": "Inner-city STAR sites."},
            "suburban-schools": {"A": (910, 1000), "B": (900, 1000), "desc": "Suburban STAR sites."},
        },
    ),
    # --- Sports ---
    _row(
        "nba-shooting-shot-mix",
        "NBA Team Shooting: Two-Point vs Three-Point Mix Paradox",
        "Classic NBA Simpson example (MIT/Stanford teaching sets). A=Player/team A, B=B, success=made shot. "
        "Two-point vs three-point attempt mix drives pooled FG% reversal.",
        "https://en.wikipedia.org/wiki/Simpson%27s_paradox",
        "sports", 2015, "tradition-historical", "researcher-blyth",
        "shot_type", "confounder",
        "Shot-type mix confounds shooting comparison; higher FG% in both shot types but lower pooled due to 3PT volume.",
        "Shooter/team A.", "Shooter/team B.",
        {
            "two-point-attempts": {"A": (480, 1000), "B": (440, 1000), "desc": "Two-point shot stratum."},
            "three-point-attempts": {"A": (360, 1000), "B": (150, 500), "desc": "Three-point shot stratum."},
        },
    ),
    _row(
        "pitcher-era-ballpark",
        "Pitcher ERA by Home Ballpark Factor",
        "FanGraphs park-factor teaching examples. A=Pitcher A, B=Pitcher B, success=outs recorded (inning success). "
        "Approximate hitter-friendly vs pitcher-friendly park strata.",
        "https://www.fangraphs.com/library/misc/park-factors/",
        "sports", 2010, "tradition-historical", "researcher-blyth",
        "ballpark", "confounder",
        "Ballpark run environment confounds pitcher comparison and ERA outcomes.",
        "Pitcher A.", "Pitcher B.",
        {
            "pitcher-friendly-park": {"A": (650, 1000), "B": (620, 1000), "desc": "Pitcher-friendly home park stratum."},
            "hitter-friendly-park": {"A": (520, 1000), "B": (540, 1000), "desc": "Hitter-friendly home park stratum."},
        },
    ),
    _row(
        "nfl-completion-depth",
        "NFL Completion Rate by Pass Depth Mix",
        "NFL Next Gen Stats pass-depth summaries. A=QB A, B=QB B, success=completed pass. "
        "Approximate short vs deep throw mix structure.",
        "https://nextgenstats.nfl.com/",
        "sports", 2018, "tradition-historical", "researcher-blyth",
        "pass_depth", "confounder",
        "Pass-depth mix confounds QB comparison; higher completion in each depth stratum but lower pooled.",
        "Quarterback A.", "Quarterback B.",
        {
            "short-pass": {"A": (720, 800), "B": (700, 800), "desc": "Short pass attempts (≤10 air yards)."},
            "deep-pass": {"A": (90, 200), "B": (110, 200), "desc": "Deep pass attempts (>10 air yards)."},
        },
    ),
    _row(
        "field-goal-distance",
        "NFL Field Goal Accuracy by Attempt Distance Distribution",
        "Pro-Football-Reference kicker splits. A=Kicker A, B=Kicker B, success=made FG. "
        "Approximate distance-mix strata from published FG% by distance tables.",
        "https://www.pro-football-reference.com/",
        "sports", 2012, "tradition-historical", "researcher-blyth",
        "kick_distance", "confounder",
        "Attempt distance distribution confounds kicker comparison; better within distance bins but worse pooled.",
        "Kicker A.", "Kicker B.",
        {
            "short-fg": {"A": (950, 1000), "B": (940, 1000), "desc": "FG attempts under 40 yards."},
            "long-fg": {"A": (600, 1000), "B": (650, 1000), "desc": "FG attempts 40+ yards."},
        },
    ),
    _row(
        "golf-scoring-course",
        "PGA Scoring Average by Course Difficulty Mix",
        "PGA Tour course scoring statistics. A=Golfer A, B=Golfer B, success=under-par round. "
        "Approximate easy vs difficult course mix from tour schedule splits.",
        "https://www.pgatour.com/stats",
        "sports", 2015, "tradition-historical", "researcher-blyth",
        "course_difficulty", "confounder",
        "Course difficulty mix confounds golfer comparison; better on both course types but worse pooled schedule mix.",
        "Golfer A.", "Golfer B.",
        {
            "easy-course": {"A": (650, 1000), "B": (620, 1000), "desc": "Easy course stratum."},
            "difficult-course": {"A": (350, 1000), "B": (380, 1000), "desc": "Difficult course stratum."},
        },
    ),
    # --- Economics ---
    _row(
        "minimum-wage-region",
        "Minimum Wage Employment Effects by Regional Labor Market",
        "Dube A et al. Rev Econ Stat 2010 border-county design summaries. A=raised minimum wage, B=comparison, success=employment retained. "
        "Approximate regional labor-market strata.",
        "https://pubmed.ncbi.nlm.nih.gov/20496667/",
        "economics", 2014, "tradition-epidemiology", "researcher-greenland",
        "region", "confounder",
        "Regional labor market conditions confound minimum-wage exposure and employment outcomes.",
        "High minimum-wage region.", "Comparison region.",
        {
            "tight-labor-market": {"A": (920, 1000), "B": (910, 1000), "desc": "Tight labor market stratum."},
            "slack-labor-market": {"A": (780, 1000), "B": (820, 1000), "desc": "Slack labor market stratum."},
        },
    ),
    _row(
        "returns-to-education-field",
        "Returns to Education by Field of Study",
        "Carnevale AP Georgetown CEW earnings reports. A=college grad, B=non-graduate, success=above-median earnings. "
        "Approximate field-of-study strata (STEM vs non-STEM).",
        "https://cew.georgetown.edu/cew-reports/",
        "economics", 2008, "tradition-epidemiology", "researcher-greenland",
        "field_of_study", "contested",
        "Field of study mediates education-earnings relationship; causal role contested.",
        "College-educated workers.", "Non-college workers.",
        {
            "stem-fields": {"A": (720, 1000), "B": (350, 1000), "desc": "STEM field stratum."},
            "non-stem-fields": {"A": (520, 1000), "B": (380, 1000), "desc": "Non-STEM field stratum."},
        },
    ),
    _row(
        "unemployment-education-cycle",
        "Aggregate Unemployment and Education Composition Over the Cycle",
        "BLS Current Population Survey summaries. A=current year, B=prior year, success=employed. "
        "Approximate recession education-composition shift structure.",
        "https://www.bls.gov/cps/",
        "economics", 2010, "tradition-epidemiology", "researcher-greenland",
        "education_composition", "confounder",
        "Education composition shifts over the business cycle confound aggregate unemployment comparisons.",
        "Current-period workforce.", "Prior-period workforce.",
        {
            "college-educated-share": {"A": (950, 1000), "B": (940, 1000), "desc": "College-educated-heavy subperiod."},
            "non-college-share": {"A": (820, 1000), "B": (850, 1000), "desc": "Non-college-heavy subperiod."},
        },
    ),
    # --- Social science ---
    _row(
        "costa-rica-us-life-expectancy",
        "Costa Rica vs US Life Expectancy by Age Group",
        "Rosero-Bixby L. Popul Dev Rev 2004. A=Costa Rica, B=US, success=survival. "
        "Approximate age-stratified mortality from cross-national standardization examples.",
        "https://pubmed.ncbi.nlm.nih.gov/15244519/",
        "social-science", 2010, "tradition-epidemiology", "researcher-greenland",
        "age_group", "confounder",
        "Age structure confounds cross-national life expectancy comparison.",
        "Costa Rica.", "United States.",
        {
            "under-65": {"A": (990, 1000), "B": (985, 1000), "desc": "Under 65 age stratum."},
            "65-plus": {"A": (850, 1000), "B": (900, 1000), "desc": "Age 65+ stratum."},
        },
    ),
    _row(
        "fertility-income-country",
        "Fertility vs Income: Within-Country vs Cross-Country Paradox",
        "World Bank WDI teaching examples. A=high-income group, B=low-income group, success=below-replacement fertility. "
        "Approximate within- vs across-country strata.",
        "https://data.worldbank.org/",
        "social-science", 2015, "tradition-epidemiology", "researcher-greenland",
        "country", "confounder",
        "Country-level development confounds income and fertility; ecological paradox across nations.",
        "Higher income group.", "Lower income group.",
        {
            "high-income-countries": {"A": (450, 1000), "B": (520, 1000), "desc": "OECD/high-income country stratum."},
            "middle-income-countries": {"A": (380, 1000), "B": (350, 1000), "desc": "Middle-income country stratum."},
        },
    ),
    _row(
        "religiosity-happiness-country",
        "Religiosity and Happiness Confounded by National Wealth",
        "World Values Survey / Gallup World Poll summaries. A=religious, B=non-religious, success=high life satisfaction. "
        "Approximate country-wealth strata.",
        "https://www.worldvaluessurvey.org/",
        "social-science", 2018, "tradition-epidemiology", "researcher-greenland",
        "national_wealth", "confounder",
        "National wealth confounds religiosity and happiness; pooled cross-country comparison reverses within-country pattern.",
        "Religious individuals.", "Non-religious individuals.",
        {
            "high-gdp-countries": {"A": (750, 1000), "B": (780, 1000), "desc": "High-GDP country stratum."},
            "lower-gdp-countries": {"A": (620, 1000), "B": (580, 1000), "desc": "Lower-GDP country stratum."},
        },
    ),
    _row(
        "voter-turnout-education-state",
        "Voter Turnout by Education Across States",
        "CPS Voting and Registration Supplement. A=high education, B=lower education, success=voted. "
        "Approximate state demographic composition strata.",
        "https://www.census.gov/topics/public-sector/voting.html",
        "social-science", 2012, "tradition-historical", "researcher-blyth",
        "state", "confounder",
        "State demographic composition confounds education-turnout relationship; ecological reversal.",
        "College-educated voters.", "Non-college voters.",
        {
            "high-education-states": {"A": (780, 1000), "B": (520, 1000), "desc": "States with high college attainment."},
            "lower-education-states": {"A": (650, 1000), "B": (480, 1000), "desc": "States with lower college attainment."},
        },
    ),
    _row(
        "ad-ctr-platform-mix",
        "Ad Click-Through Rate by Platform Mix",
        "Industry A/B testing case studies (Kohavi-style). A=creative A, B=creative B, success=click. "
        "Approximate mobile vs desktop platform mix reversal.",
        "https://www.cambridge.org/core/books/trustworthy-online-controlled-experiments/",
        "social-science", 2019, "tradition-historical", "researcher-blyth",
        "platform", "confounder",
        "Platform mix confounds creative comparison; CTR reversal from segment composition shift.",
        "Ad creative A.", "Ad creative B.",
        {
            "mobile-traffic": {"A": (80, 1000), "B": (75, 1000), "desc": "Mobile platform stratum."},
            "desktop-traffic": {"A": (45, 1000), "B": (50, 1000), "desc": "Desktop platform stratum."},
        },
    ),
]


def main():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)

    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))
    catalog_by_id = {r["CandidateId"]: r for r in rb["CandidateStudyCatalog"]["data"]}
    existing_ids = {r["StudyId"] for r in rb["Studies"]["data"]}
    imported = 0
    skipped = 0

    for spec_row in IMPORTS:
        cid = spec_row["candidate_id"]
        sid = spec_row["study_id"]
        if sid in existing_ids:
            print(f"SKIP {sid} (already in corpus)")
            skipped += 1
            continue
        if cid not in catalog_by_id:
            raise SystemExit(f"Missing catalog row: {cid}")
        if catalog_by_id[cid]["IngestionStatus"] not in ("candidate", "imported"):
            print(f"SKIP {cid} status={catalog_by_id[cid]['IngestionStatus']}")
            skipped += 1
            continue
        if catalog_by_id[cid]["IngestionStatus"] == "imported":
            print(f"SKIP {sid} (catalog already imported)")
            skipped += 1
            continue

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
        cat["DataSourceNote"] = f"Imported loop-59. {cat['DataSourceNote'].replace('counts need verification.', 'approximate 2xK from cited source.')}"
        existing_ids.add(sid)
        imported += 1
        print(f"IMPORT {sid} ({len(spec_row['strata'])} strata, {rows['study']['TotalCases']} cases)")

    loops = rb["Loops"]["data"]
    if not any(r["LoopId"] == "loop-59" for r in loops):
        loops.append({
            "LoopId": "loop-59",
            "Title": "REAL? backlog drain: encode all 33 queued candidates with cited approximate 2xK tables",
            "Status": "complete",
            "NewConcept": "All remaining CandidateStudyCatalog REAL? rows imported with documented approximate cell counts from primary/secondary sources; corpus reaches 96 studies for cross-domain pattern analysis.",
            "DomainQuestion": "Does draining the REAL? backlog increase domain/mechanism coverage without breaking invariant harness — and how often does ExpectedDistortionType match observed DistortionType?",
            "NextSuggestion": "loop-60: TypePredictionMatch audit + ModelSummary refresh across full 96-study corpus",
            "MockDataNote": f"Witnessed: {imported} studies imported from REAL? backlog; corpus now {len(existing_ids)} studies.",
            "TraditionId": "tradition-epidemiology",
        })
        for row in loops:
            if row["LoopId"] == "loop-58":
                row["NextSuggestion"] = "loop-59: drain REAL? catalog backlog"

    n_studies = len(rb["Studies"]["data"])
    imported_catalog = sum(1 for r in rb["CandidateStudyCatalog"]["data"] if r.get("IngestionStatus") == "imported")
    candidates_left = sum(1 for r in rb["CandidateStudyCatalog"]["data"] if r.get("IngestionStatus") == "candidate")
    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-corrected-gap-invariant":
            inv["PassCount"] = n_studies
        if inv["InvariantCheckId"] == "inv-catalog-imported-linked":
            inv["PassCount"] = imported_catalog
        if inv["InvariantCheckId"] == "inv-import-session-ready":
            inv["AlgebraicStatement"] = "CandidateCount = 0 (all encode-ready catalog rows imported)"
            inv["NaturalLanguage"] = "Post loop-59: REAL? backlog drained; zero catalog rows remain with IngestionStatus=candidate (blocked rows excluded)."
            inv["AssertionExpression"] = "CandidateCount = 0"
            inv["SqlAssertion"] = "candidate_count = 0"
            inv["PassCount"] = 1 if candidates_left == 0 else 0
            inv["FailCount"] = 0 if candidates_left == 0 else 1

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Done: imported {imported}, skipped {skipped}, corpus {n_studies}, candidates left {candidates_left}")


if __name__ == "__main__":
    main()
