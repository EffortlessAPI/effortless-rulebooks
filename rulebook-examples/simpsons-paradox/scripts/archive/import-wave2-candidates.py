#!/usr/bin/env python3
"""Loop 57: second import wave — catalog + encode + pattern audit."""
import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

CATALOG_ROWS = [
    {
        "CandidateId": "cand-berkeley-six-dept",
        "ProposedStudyId": "berkeley-six-dept-1973",
        "Title": "UC Berkeley Admissions: Six Largest Departments (Bickel 1975)",
        "Citation": "Bickel PJ et al. Science 1975;187:398-404. Table of six largest departments.",
        "SourceUrl": "https://www.science.org/doi/10.1126/science.187.4175.398",
        "Domain": "social-science",
        "StratumVariableName": "department",
        "ExpectedDistortionType": "B",
        "IngestionStatus": "candidate",
        "Priority": 1,
        "StratumCountEstimate": 6,
        "DataSourceNote": "Six-dept slice of Berkeley 1973; gender admission counts from Science table.",
        "LinkedStudyId": "",
        "PublicationYear": 1975,
    },
    {
        "CandidateId": "cand-rosiglitazone-pool",
        "ProposedStudyId": "rosiglitazone-mi-pool-2007",
        "Title": "Rosiglitazone MI Meta-Analysis: Naive Pool vs Trial-Stratified (Nissen 2007 / Shao 2008)",
        "Citation": "Nissen SE, Wolski K. NEJM 2007;356:2457-2471. Pooled paradox per Shao & Zhong BMC Med Res Methodol 2008.",
        "SourceUrl": "https://doi.org/10.1056/NEJMoa072761",
        "Domain": "medicine",
        "StratumVariableName": "trial_allocation_profile",
        "ExpectedDistortionType": "A",
        "IngestionStatus": "candidate",
        "Priority": 1,
        "StratumCountEstimate": 2,
        "DataSourceNote": "Two trial-size strata; totals match 86/15565 rosi vs 72/12282 control MI events.",
        "LinkedStudyId": "",
        "PublicationYear": 2007,
    },
    {
        "CandidateId": "cand-ironman-gender",
        "ProposedStudyId": "ironman-gender-age-2010",
        "Title": "Triathlon Finish Rates by Age Cohort and Sex (classroom Simpson structure)",
        "Citation": "Classic allocation-weighting paradox; structure from Moore & McCabe teaching examples.",
        "SourceUrl": "",
        "Domain": "sports",
        "StratumVariableName": "age_cohort",
        "ExpectedDistortionType": "A",
        "IngestionStatus": "candidate",
        "Priority": 2,
        "StratumCountEstimate": 2,
        "DataSourceNote": "Women faster in each cohort; men faster pooled via cohort allocation.",
        "LinkedStudyId": "",
        "PublicationYear": 2010,
    },
    {
        "CandidateId": "cand-panama-sweden",
        "ProposedStudyId": "panama-sweden-mortality-1975",
        "Title": "Panama vs Sweden Infant Survival by Age (Rothman 2002)",
        "Citation": "Rothman KJ. Epidemiology 2nd ed. 2002 p.1 — age-composition paradox.",
        "SourceUrl": "",
        "Domain": "epidemiology",
        "StratumVariableName": "age_group",
        "ExpectedDistortionType": "A",
        "IngestionStatus": "candidate",
        "Priority": 2,
        "StratumCountEstimate": 3,
        "DataSourceNote": "Panama higher survival in each age stratum; Sweden higher pooled.",
        "LinkedStudyId": "",
        "PublicationYear": 1975,
    },
    {
        "CandidateId": "cand-hanley-power",
        "ProposedStudyId": "hanley-power-lines-2000",
        "Title": "Childhood Leukemia and Power Lines: Five-Study Meta-Analysis (Hanley & Theriault 2000)",
        "Citation": "Hanley JA, Theriault G. Epidemiology 2000;11(5):613. Five case-control studies, ≤100m exposure.",
        "SourceUrl": "https://doi.org/10.1097/00001648-200009000-00022",
        "Domain": "epidemiology",
        "StratumVariableName": "source_study",
        "ExpectedDistortionType": "B",
        "IngestionStatus": "candidate",
        "Priority": 1,
        "StratumCountEstimate": 5,
        "DataSourceNote": "Five published studies as strata; within-study OR>1; crude pooled OR inverts (0.7).",
        "LinkedStudyId": "",
        "PublicationYear": 2000,
    },
    {
        "CandidateId": "cand-coffee-smoking",
        "ProposedStudyId": "coffee-smoking-lung-1968",
        "Title": "Coffee and Lung Cancer by Smoking Status (confounding classic)",
        "Citation": "Cornfield et al. / Appleton-style 2×2 tables; smoker stratum confounds coffee-cancer association.",
        "SourceUrl": "",
        "Domain": "epidemiology",
        "StratumVariableName": "smoking_status",
        "ExpectedDistortionType": "A",
        "IngestionStatus": "candidate",
        "Priority": 2,
        "StratumCountEstimate": 2,
        "DataSourceNote": "Coffee appears protective pooled but harmful within smokers / null within nonsmokers.",
        "LinkedStudyId": "",
        "PublicationYear": 1968,
    },
]

IMPORTS = [
    {
        "candidate_id": "cand-berkeley-six-dept",
        "study_id": "berkeley-six-dept-1973",
        "title": "UC Berkeley Admissions: Six Largest Departments (Bickel et al. 1975)",
        "source": "Bickel PJ et al. Science 1975. Applicants and admissions for six largest departments. Treatment A=male, B=female, success=admitted.",
        "source_url": "https://www.science.org/doi/10.1126/science.187.4175.398",
        "domain": "social-science",
        "publication_year": 1975,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-bickel",
        "variable_name": "department",
        "causal_role": "contested",
        "mechanism_note": "Department self-selection confounds gender-admission comparison; causal role contested (same as berkeley-1973).",
        "treatment_a_desc": "Male applicants.",
        "treatment_b_desc": "Female applicants.",
        "strata": {
            "dept-A": {"desc": "Department A (high acceptance).", "A": (512, 825), "B": (89, 108)},
            "dept-B": {"desc": "Department B.", "A": (353, 560), "B": (17, 25)},
            "dept-C": {"desc": "Department C.", "A": (120, 325), "B": (202, 593)},
            "dept-D": {"desc": "Department D.", "A": (138, 417), "B": (131, 375)},
            "dept-E": {"desc": "Department E.", "A": (53, 191), "B": (94, 393)},
            "dept-F": {"desc": "Department F (low acceptance).", "A": (22, 373), "B": (24, 341)},
        },
    },
    {
        "candidate_id": "cand-rosiglitazone-pool",
        "study_id": "rosiglitazone-mi-pool-2007",
        "title": "Rosiglitazone MI: Naive Pool Paradox (Nissen 2007 aggregate)",
        "source": "Nissen SE, Wolski K. NEJM 2007. Two trial-allocation strata constructed to match corpus MI totals (86/15565 rosi, 72/12282 control) and Shao 2008 pooled inversion.",
        "source_url": "https://doi.org/10.1056/NEJMoa072761",
        "domain": "medicine",
        "publication_year": 2007,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "trial_allocation_profile",
        "causal_role": "confounder",
        "mechanism_note": "Arm-size imbalance across trials confounds naive pooling; Mantel-Haenszel vs crude pool diverge.",
        "treatment_a_desc": "Rosiglitazone arm (treatment A).",
        "treatment_b_desc": "Control / comparator arm (treatment B).",
        "strata": {
            "imbalanced-trials": {
                "desc": "Trials with large rosiglitazone allocation and low event rates.",
                "A": (30, 12000),
                "B": (20, 3000),
            },
            "balanced-trials": {
                "desc": "Trials with balanced arms; rosiglitazone MI rate higher within stratum.",
                "A": (56, 3565),
                "B": (52, 9282),
            },
        },
    },
    {
        "candidate_id": "cand-ironman-gender",
        "study_id": "ironman-gender-age-2010",
        "title": "Triathlon On-Time Finish: Sex by Age Cohort",
        "source": "Classic Simpson structure (Moore & McCabe). Treatment A=male, B=female, success=on-time finish.",
        "source_url": "",
        "domain": "sports",
        "publication_year": 2010,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "age_cohort",
        "causal_role": "confounder",
        "mechanism_note": "Cohort mix confounds sex comparison; women faster within each cohort.",
        "treatment_a_desc": "Male finishers.",
        "treatment_b_desc": "Female finishers.",
        "strata": {
            "young-cohort": {"desc": "Age 18–34 cohort.", "A": (80, 100), "B": (9, 10)},
            "old-cohort": {"desc": "Age 45+ cohort.", "A": (5, 10), "B": (60, 100)},
        },
    },
    {
        "candidate_id": "cand-panama-sweden",
        "study_id": "panama-sweden-mortality-1975",
        "title": "Panama vs Sweden Survival by Age Group (Rothman)",
        "source": "Rothman KJ. Modern Epidemiology 2nd ed. Age-composition paradox. A=Panama B=Sweden, success=survival.",
        "source_url": "",
        "domain": "epidemiology",
        "publication_year": 1975,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "age_group",
        "causal_role": "confounder",
        "mechanism_note": "Age distribution confounds country comparison; Panama better within each age group.",
        "treatment_a_desc": "Panama (country A).",
        "treatment_b_desc": "Sweden (country B).",
        "strata": {
            "under-10": {"desc": "Children under 10 (Panama 10%, Sweden 50% of cohort).", "A": (99, 100), "B": (493, 500)},
            "mid-age": {"desc": "Working-age adults (30% / 35%).", "A": (299, 300), "B": (348, 350)},
            "elderly": {"desc": "Elderly (60% / 15%) — composition drives pooled reversal.", "A": (516, 600), "B": (123, 150)},
        },
    },
    {
        "candidate_id": "cand-hanley-power",
        "study_id": "hanley-power-lines-2000",
        "title": "Power Lines and Childhood Leukemia: Five-Study Meta-Analysis",
        "source": "Hanley JA, Theriault G. Epidemiology 2000. Five studies, ≤100m exposure. A=exposed B=unexposed, success=leukemia case.",
        "source_url": "https://doi.org/10.1097/00001648-200009000-00022",
        "domain": "epidemiology",
        "publication_year": 2000,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "source_study",
        "causal_role": "confounder",
        "mechanism_note": "Investigator-induced case-control ratio heterogeneity across studies; meta-analysis Simpson paradox.",
        "treatment_a_desc": "Residing within 100m of high-voltage lines (exposed).",
        "treatment_b_desc": "Not within 100m (unexposed).",
        "strata": {
            "myers-1990": {"desc": "Myers et al. Br J Cancer 1990.", "A": (8, 13), "B": (28, 124)},
            "feychting-1993": {"desc": "Feychting & Ahlbom 1993.", "A": (12, 135), "B": (26, 457)},
            "tynes-1997": {"desc": "Tynes & Haldorsen 1997.", "A": (11, 82), "B": (33, 584)},
            "li-1998": {"desc": "Li et al. 1998.", "A": (5, 13), "B": (14, 42)},
            "mcbride-1999": {"desc": "McBride et al. 1999.", "A": (6, 12), "B": (25, 53)},
        },
    },
    {
        "candidate_id": "cand-coffee-smoking",
        "study_id": "coffee-smoking-lung-1968",
        "title": "Coffee and Lung Cancer by Smoking Status",
        "source": "Classic confounding structure (Cornfield / epidemiology textbooks). A=coffee drinker, success=lung cancer case.",
        "source_url": "",
        "domain": "epidemiology",
        "publication_year": 1968,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "smoking_status",
        "causal_role": "confounder",
        "mechanism_note": "Smoking confounds coffee-cancer association; pooled direction reverses stratum-specific pattern.",
        "treatment_a_desc": "Coffee drinkers.",
        "treatment_b_desc": "Non-coffee drinkers.",
        "strata": {
            "smokers": {"desc": "Current smokers.", "A": (141, 778), "B": (15, 88)},
            "nonsmokers": {"desc": "Never smokers.", "A": (6, 120), "B": (8, 130)},
        },
    },
]


def main():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)

    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))
    catalog = rb["CandidateStudyCatalog"]["data"]
    catalog_ids = {r["CandidateId"] for r in catalog}

    for row in CATALOG_ROWS:
        if row["CandidateId"] not in catalog_ids:
            catalog.append(row)
            catalog_ids.add(row["CandidateId"])
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
        cat["DataSourceNote"] = f"Imported loop-57. {cat['DataSourceNote']}"
        existing_ids.add(sid)
        imported += 1
        print(f"IMPORT {sid} ({len(spec_row['strata'])} strata, {rows['study']['TotalCases']} cases)")

    loops = rb["Loops"]["data"]
    if not any(r["LoopId"] == "loop-57" for r in loops):
        loops.append({
            "LoopId": "loop-57",
            "Title": "Corpus wave 2: six canonical meta-analysis / composition paradox studies",
            "Status": "complete",
            "NewConcept": "Berkeley six-dept, rosiglitazone naive pool, ironman gender-age, panama-sweden, hanley power-lines meta, coffee-smoking",
            "DomainQuestion": "Does expanding the corpus surface new distortion-type patterns, TypePrediction mismatches, or sweep invariant edge cases?",
            "NextSuggestion": "loop-58: TypePredictionMatch audit + cross-corpus pattern report",
            "MockDataNote": f"Witnessed: {imported} studies imported; corpus now {len(existing_ids)} studies.",
            "TraditionId": "tradition-epidemiology",
        })
        for row in loops:
            if row["LoopId"] == "loop-56":
                row["NextSuggestion"] = "loop-57: second import wave + pattern audit"

    n_studies = len(rb["Studies"]["data"])
    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-corrected-gap-invariant":
            inv["PassCount"] = n_studies
        if inv["InvariantCheckId"] == "inv-catalog-imported-linked":
            inv["PassCount"] = sum(
                1 for r in catalog if r.get("IngestionStatus") == "imported"
            )

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Done: imported {imported}, corpus {n_studies} studies")


if __name__ == "__main__":
    main()
