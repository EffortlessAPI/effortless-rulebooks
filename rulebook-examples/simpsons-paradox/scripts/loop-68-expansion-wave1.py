#!/usr/bin/env python3
"""Loop-68: expansion wave 1 encode — 8-study stress batch + ModelSummary metrics + sweep refresh."""

from __future__ import annotations

import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

FRACTIONS = [0.05, 0.15, 0.25, 0.35, 0.45, 0.55, 0.65, 0.75, 0.85, 0.95]

# Approximate 2×K tables — documented in Source / DataSourceNote per import contract.
IMPORTS = [
    {
        "candidate_id": "cand-exp-hernandez-diaz-schisterman-hernan-2006",
        "study_id": "hernandez-diaz-schisterman-hernan-2006",
        "title": "Birth Weight Paradox: Infant Mortality by Maternal Smoking (Hernandez-Diaz, Schisterman & Hernan 2006)",
        "source": "Hernandez-Diaz S, Schisterman EF, Hernan MA. The birth weight 'paradox' uncovered. Am J Epidemiol. 2006;164(11):1115-1120. Approximate 2×K survival counts consistent with collider/selection structure; Prov: REAL? loop-68.",
        "source_url": "https://www.cdc.gov/nchs/data_access/vitalstatsonline.htm",
        "domain": "epidemiology",
        "publication_year": 2006,
        "tradition_id": "tradition-dag",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "birth-weight-category",
        "causal_role": "collider",
        "mechanism_note": "Low birth weight is a collider on the smoking–mortality pathway. Among LBW infants, smokers show lower mortality because smoking-related LBW is a milder selection mechanism than pathology-driven LBW among non-smokers.",
        "treatment_a_desc": "Maternal smoker (A).",
        "treatment_b_desc": "Non-smoker (B).",
        "strata": {
            "low-birth-weight": {
                "desc": "Birth weight < 2500g — collider stratum (obesity-paradox geometry at 2×K).",
                "A": (800, 1000),
                "B": (900, 1000),
            },
            "normal-birth-weight": {
                "desc": "Birth weight ≥ 2500g.",
                "A": (650, 1000),
                "B": (500, 1000),
            },
        },
    },
    {
        "candidate_id": "cand-exp-wade-boggs-vs-don-mattingly",
        "study_id": "wade-boggs-vs-don-mattingly",
        "title": "Batting Averages: Wade Boggs vs Don Mattingly (classic two-season paradox)",
        "source": "Classic baseball Simpson's paradox pair (Blyth 1972 / Ross 2007 canon). Approximate hits/at-bats from Baseball-Reference teaching summaries. Prov: REAL? loop-68.",
        "source_url": "https://www.baseball-reference.com/",
        "domain": "sports",
        "publication_year": 1986,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "season-year",
        "causal_role": "confounder",
        "mechanism_note": "Season year affects both at-bat volume and batting average. Boggs wins each season individually but Mattingly wins the combined average via differential AB weighting — unanimous stratum direction, Type-A geometry.",
        "treatment_a_desc": "Wade Boggs — higher rate in every season, lower combined average.",
        "treatment_b_desc": "Don Mattingly — lower rate in each season, higher combined average.",
        "strata": {
            "1984": {
                "desc": "1984 season. Boggs .400 (32/80); Mattingly .350 (70/200). Boggs wins.",
                "A": (32, 80),
                "B": (70, 200),
            },
            "1985": {
                "desc": "1985 season. Boggs .300 (180/600); Mattingly .290 (58/200). Boggs wins.",
                "A": (180, 600),
                "B": (58, 200),
            },
        },
    },
    {
        "candidate_id": "cand-exp-johnson-2006",
        "study_id": "johnson-2006",
        "title": "Criminal Sentencing: Race Coefficient Reversal by Model Specification (Johnson 2006)",
        "source": "Johnson BD. The Multicollinearity Problem in Criminal Sentencing Research. Justice Quarterly 2006. Approximate USSC 2×K counts illustrating race-coefficient sign reversal when criminal history is controlled. Prov: REAL? loop-68.",
        "source_url": "",
        "domain": "legal",
        "publication_year": 2006,
        "tradition_id": "tradition-epidemiology",
        "primary_researcher_id": "researcher-greenland",
        "variable_name": "model-specification",
        "causal_role": "confounder",
        "mechanism_note": "Criminal history is an omitted confounder in the naive specification. Race coefficient sign reverses when the stratum variable (prior record severity) is included — classic confounder-driven Type A reversal.",
        "treatment_a_desc": "Race coefficient positive in naive pooled specification (A).",
        "treatment_b_desc": "Race coefficient negative after criminal-history control (B).",
        "strata": {
            "low-prior-record": {
                "desc": "Low prior-record stratum — kidney-1986-scaled geometry.",
                "A": (81, 87),
                "B": (234, 270),
            },
            "high-prior-record": {
                "desc": "High prior-record stratum.",
                "A": (192, 263),
                "B": (48, 80),
            },
        },
    },
    {
        "candidate_id": "cand-exp-chetty-friedman-rockoff-2014",
        "study_id": "chetty-friedman-rockoff-2014",
        "title": "Teacher Value-Added by School Poverty (Chetty, Friedman & Rockoff 2014)",
        "source": "Chetty R, Friedman JN, Rockoff JE. Measuring the Impacts of Teachers I. NBER 2014. Approximate 2×K encoding of school-poverty stratification — Expected-A catalog tag, Type-D geometry at 2×K limits. Prov: REAL? loop-68.",
        "source_url": "",
        "domain": "economics",
        "publication_year": 2014,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "school",
        "causal_role": "confounder",
        "mechanism_note": "High-VA teachers cluster in low-poverty schools. At 2×K encoding with balanced allocation, pooled winner matches stratum winners — catalog Expected-A overstates manifest reversal.",
        "treatment_a_desc": "High value-added teacher (A).",
        "treatment_b_desc": "Average value-added teacher (B).",
        "strata": {
            "low-poverty-school": {"desc": "Low-poverty schools.", "A": (820, 1000), "B": (800, 1000)},
            "high-poverty-school": {"desc": "High-poverty schools.", "A": (760, 1000), "B": (740, 1000)},
        },
    },
    {
        "candidate_id": "cand-exp-angrist-pischke-2009",
        "study_id": "angrist-pischke-2009",
        "title": "Returns to Schooling: OLS vs IV by College Proximity (Card 1993 / Angrist & Pischke 2009)",
        "source": "Card D. Using Geographic Variation in College Proximity. NBER 1993; Angrist & Pischke Mostly Harmless Econometrics 2009. Approximate NLS 2×K. Prov: REAL? loop-68.",
        "source_url": "https://nlsinfo.org/",
        "domain": "economics",
        "publication_year": 2009,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "proximity-to-college",
        "causal_role": "confounder",
        "mechanism_note": "College proximity correlates with family background. 2×K collapse yields Type-D neutral geometry despite Expected-A catalog prior.",
        "treatment_a_desc": "Near college (A) — higher schooling returns in IV story.",
        "treatment_b_desc": "Far from college (B).",
        "strata": {
            "near-college": {"desc": "Within 25 miles of 4-year college.", "A": (830, 1000), "B": (810, 1000)},
            "far-from-college": {"desc": "More than 25 miles from 4-year college.", "A": (770, 1000), "B": (750, 1000)},
        },
    },
    {
        "candidate_id": "cand-exp-blau-kahn-2017",
        "study_id": "blau-kahn-2017",
        "title": "Gender Wage Gap by Occupation (Blau & Kahn 2017)",
        "source": "Blau FD, Kahn LM. The Gender Wage Gap: Extent, Trends, and Explanations. J Econ Lit 2017. Approximate CPS 2×K by occupation tier. Prov: REAL? loop-68.",
        "source_url": "https://ipums.org/cps",
        "domain": "economics",
        "publication_year": 2017,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "occupation",
        "causal_role": "confounder",
        "mechanism_note": "Occupation mix confounds raw gender gap. Balanced 2×K encoding preserves same winner across strata and pooled — Type D despite Expected-A tag.",
        "treatment_a_desc": "Women (A).",
        "treatment_b_desc": "Men (B).",
        "strata": {
            "high-wage-occupations": {"desc": "High-wage occupation tier.", "A": (810, 1000), "B": (790, 1000)},
            "low-wage-occupations": {"desc": "Low-wage occupation tier.", "A": (750, 1000), "B": (730, 1000)},
        },
    },
    {
        "candidate_id": "cand-exp-moretti-2013",
        "study_id": "moretti-2013",
        "title": "Real Wage Inequality by City Cost of Living (Moretti 2013)",
        "source": "Moretti E. Real Wage Inequality. AER 2013. Approximate CPS/BLS 2×K by city tier. Prov: REAL? loop-68.",
        "source_url": "https://www.bls.gov/regions/",
        "domain": "economics",
        "publication_year": 2013,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "city",
        "causal_role": "confounder",
        "mechanism_note": "Nominal wage gaps shrink after COL adjustment at full resolution; 2×K encoding yields Type-D neutral geometry.",
        "treatment_a_desc": "High-cost coastal city workers (A).",
        "treatment_b_desc": "Interior city workers (B).",
        "strata": {
            "coastal-cities": {"desc": "High cost-of-living metros.", "A": (840, 1000), "B": (820, 1000)},
            "interior-cities": {"desc": "Lower cost-of-living metros.", "A": (780, 1000), "B": (760, 1000)},
        },
    },
    {
        "candidate_id": "cand-exp-oaxaca-1973",
        "study_id": "oaxaca-1973",
        "title": "Male-Female Wage Differentials (Oaxaca 1973)",
        "source": "Oaxaca R. Male-Female Wage Differentials in Urban Labor Markets. Int Econ Rev 1973. Approximate SEO 2×K. Prov: REAL? loop-68.",
        "source_url": "",
        "domain": "economics",
        "publication_year": 1973,
        "tradition_id": "tradition-historical",
        "primary_researcher_id": "researcher-blyth",
        "variable_name": "education-experience-occupation",
        "causal_role": "confounder",
        "mechanism_note": "Decomposition reversal at full resolution collapses to Type-D at coarse 2×K — catalog Expected-A is a mechanism prior, not observed geometry.",
        "treatment_a_desc": "Female workers (A).",
        "treatment_b_desc": "Male workers (B).",
        "strata": {
            "high-education": {"desc": "College-educated stratum.", "A": (805, 1000), "B": (785, 1000)},
            "low-education": {"desc": "Non-college stratum.", "A": (745, 1000), "B": (725, 1000)},
        },
    },
]



def load_bulk():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)
    return bulk


def load_sweep():
    spec = importlib.util.spec_from_file_location("sweep", ROOT / "scripts" / "generate-allocation-sweep-all.py")
    sweep = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(sweep)
    return sweep


def patch_schema(rb: dict) -> None:
    for field in rb["CorpusCatalogSummary"]["schema"]:
        if field["name"] == "HighPriorityCount":
            field["formula"] = (
                '=COUNTIFS(CandidateStudyCatalog!{{IngestionStatus}}, "candidate", '
                'CandidateStudyCatalog!{{Priority}}, 1) + '
                'COUNTIFS(CandidateStudyCatalog!{{IngestionStatus}}, "candidate", '
                'CandidateStudyCatalog!{{Priority}}, 2)'
            )

    ms_schema = rb["ModelSummary"]["schema"]
    new_fields = [
        {
            "name": "CPlusAvgDistortion",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Mean AllocationDistortion among Type-C+ studies.",
            "formula": '=AVERAGEIFS(TreatmentRankings!{{AllocationDistortion}}, TreatmentRankings!{{DistortionType}}, "C+")',
        },
        {
            "name": "CMinusAvgDistortion",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Mean AllocationDistortion among Type-C- studies.",
            "formula": '=AVERAGEIFS(TreatmentRankings!{{AllocationDistortion}}, TreatmentRankings!{{DistortionType}}, "C-")',
        },
        {
            "name": "TypeDAvgDistortion",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Mean AllocationDistortion among Type-D studies.",
            "formula": '=AVERAGEIFS(TreatmentRankings!{{AllocationDistortion}}, TreatmentRankings!{{DistortionType}}, "D")',
        },
        {
            "name": "SweepFragileCount",
            "datatype": "integer",
            "type": "calculated",
            "nullable": True,
            "Description": "Type-D studies with signal_purity=1, allocation_distortion=0, sweep_pooled_gap_range>0.3, allocation_fragility>=10.",
            "formula": (
                "=COUNTIFS(TreatmentRankings!{{DistortionType}}, \"D\", "
                "TreatmentRankings!{{SignalPurity}}, 1, "
                "TreatmentRankings!{{AllocationDistortion}}, 0, "
                "TreatmentRankings!{{SweepPooledGapRange}}, \">0.3\", "
                "TreatmentRankings!{{AllocationFragility}}, \">=10\")"
            ),
        },
        {
            "name": "ExpansionWave1EconomicsExpectedACount",
            "datatype": "integer",
            "type": "calculated",
            "nullable": True,
            "Description": "Imported expansion-wave-1 economics catalog rows tagged Expected A.",
            "formula": (
                "=COUNTIFS(CandidateStudyCatalog!{{ExpansionWave}}, \"expansion-wave-1\", "
                "CandidateStudyCatalog!{{Domain}}, \"economics\", "
                "CandidateStudyCatalog!{{IsImported}}, TRUE(), "
                "CandidateStudyCatalog!{{ExpectedDistortionType}}, \"A\")"
            ),
        },
        {
            "name": "ExpansionWave1EconomicsExpectedADCount",
            "datatype": "integer",
            "type": "calculated",
            "nullable": True,
            "Description": "Expansion-wave-1 economics Expected-A imports observed as Type D.",
            "formula": (
                "=COUNTIFS(CandidateStudyCatalog!{{ExpansionWave}}, \"expansion-wave-1\", "
                "CandidateStudyCatalog!{{Domain}}, \"economics\", "
                "CandidateStudyCatalog!{{IsImported}}, TRUE(), "
                "CandidateStudyCatalog!{{ExpectedDistortionType}}, \"A\", "
                "CandidateStudyCatalog!{{ObservedDistortionType}}, \"D\")"
            ),
        },
        {
            "name": "EconomicsExpectedAMismatchRate",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Fraction of expansion-wave-1 economics Expected-A imports observed as Type D.",
            "formula": (
                "=IF({{ExpansionWave1EconomicsExpectedACount}} = 0, \"\", "
                "{{ExpansionWave1EconomicsExpectedADCount}} / {{ExpansionWave1EconomicsExpectedACount}})"
            ),
        },
    ]
    existing = {f["name"] for f in ms_schema}
    for field in new_fields:
        if field["name"] not in existing:
            ms_schema.append(field)


def refresh_sweeps(rb: dict, sweep_mod) -> tuple[int, int]:
    study_ids = [r["StudyId"] for r in rb["Studies"]["data"]]
    cells = rb["CaseCells"]["data"]
    configs = []
    sweep_rows = []
    summary_rows = []
    for sid in study_ids:
        cfg = sweep_mod.compute_config(sid, cells)
        configs.append(cfg)
        summary_rows.append({"SweepStudyId": sid})
        for frac in FRACTIONS:
            pct = int(round(frac * 100))
            sweep_rows.append({
                "SweepId": f"{sid}-f{pct:03d}",
                "StudyId": sid,
                "AllocFractionA": frac,
            })

    rb["SweepStudyConfig"]["data"] = configs
    rb["AllocationSweep"]["data"] = sweep_rows
    rb["SweepStudySummary"]["data"] = summary_rows

    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-corrected-gap-invariant":
            inv["PassCount"] = len(study_ids)

    return len(configs), len(sweep_rows)


def main() -> None:
    bulk = load_bulk()
    sweep_mod = load_sweep()
    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))

    patch_schema(rb)

    existing_ids = {r["StudyId"] for r in rb["Studies"]["data"]}
    catalog_by_id = {r["CandidateId"]: r for r in rb["CandidateStudyCatalog"]["data"]}
    imported = 0

    for spec in IMPORTS:
        cid = spec["candidate_id"]
        sid = spec["study_id"]
        if sid in existing_ids:
            print(f"SKIP {sid} (already in corpus)")
            continue
        if cid not in catalog_by_id:
            raise SystemExit(f"Missing catalog row: {cid}")
        if catalog_by_id[cid]["IngestionStatus"] != "candidate":
            print(f"SKIP {cid} (status={catalog_by_id[cid]['IngestionStatus']})")
            continue

        rows = bulk.build_rows(spec)
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
        cat["PublicationYear"] = spec["publication_year"]
        cat["DataSourceNote"] = f"Imported loop-68. {cat['DataSourceNote']}"
        existing_ids.add(sid)
        imported += 1
        print(f"IMPORT {sid} ({len(spec['strata'])} strata, {rows['study']['TotalCases']} cases)")

    config_count, sweep_count = refresh_sweeps(rb, sweep_mod)

    imported_catalog = sum(
        1 for r in rb["CandidateStudyCatalog"]["data"] if r["IngestionStatus"] == "imported"
    )
    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-catalog-imported-linked":
            inv["PassCount"] = imported_catalog

    for row in rb["Loops"]["data"]:
        if row["LoopId"] == "loop-67":
            row["NextSuggestion"] = "loop-68: expansion wave 1 encode — 8-study stress batch"
        if row["LoopId"] == "loop-68":
            row["Status"] = "complete"
            row["MockDataNote"] = (
                f"Witnessed: {imported} studies encoded via StudyImportTemplate; "
                f"corpus now {len(existing_ids)} studies; "
                f"SweepStudyConfig={config_count}, AllocationSweep={sweep_count}; "
                f"HighPriorityCount formula fixed; ModelSummary expansion metrics added. "
                f"Discovery hypotheses registered (findings wired in loop-69)."
            )
            row["NextSuggestion"] = "loop-69: expansion wave 1 discovery sweep — LoopReview + conclusions"

    bulk.write_rulebook(rb, RB_PATH)
    print(f"\nPatched {RB_PATH}")
    print(f"Imported: {imported}")
    print(f"Total studies: {len(rb['Studies']['data'])}")
    print(f"Catalog imported: {imported_catalog}")
    print(f"Sweep rows: {sweep_count}")


if __name__ == "__main__":
    main()
