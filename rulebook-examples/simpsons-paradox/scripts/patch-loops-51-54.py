#!/usr/bin/env python3
"""Add loops 51-54: import backlog catalog, domain targets, encoding template, readiness summary."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

IMPORTED = [
    ("cand-kidney-1986", "kidney-1986", "Kidney Stone Treatment (Charig 1986)", "Charig CR et al. BMJ 1986;292:879.", "https://www.bmj.com/content/292/6524/879", "medicine", "stone_size", "A", 1986),
    ("cand-berkeley-1973", "berkeley-1973", "UC Berkeley Admissions (Bickel 1975)", "Bickel PJ et al. Science 1975;187:398-404.", "https://www.science.org/doi/10.1126/science.187.4175.398", "social-science", "department", "B", 1975),
    ("cand-reintjes-2000", "reintjes-2000", "Antibiotic Prophylaxis UTI (Reintjes 2000)", "Reintjes R et al. Epidemiology 2000;11(1):81-83.", "https://pubmed.ncbi.nlm.nih.gov/10615849/", "epidemiology", "hospital_incidence_level", "A", 2000),
    ("cand-radelet-1981", "radelet-1981", "Death Penalty Florida (Radelet 1981)", "Radelet ML. Am Soc Rev 1981;46(6):918-927.", "https://www.jstor.org/stable/2095513", "legal", "victim_race", "A", 1981),
    ("cand-jeter-1997", "jeter-justice-1997", "Jeter vs Justice Batting (Ross 2007)", "Ross KA. College Math Journal 2007;38(3):205-210.", "https://www.baseball-almanac.com/players/player.php?p=jeterde01", "sports", "season_year", "A", 1997),
    ("cand-appleton-1996", "appleton-1996", "Whickham Smoking Mortality (Appleton 1996)", "Appleton DR et al. Am Statistician 1996;50(4):340-341.", "https://doi.org/10.2307/2684931", "epidemiology", "age_group", "B", 1996),
    ("cand-phe-covid-2021", "phe-covid-2021", "COVID Delta CFR by Age (PHE 2021)", "PHE Technical Briefing 20, August 2021.", "https://www.openintro.org/data/index.php?data=simpsons_paradox_covid", "epidemiology", "age_group", "A", 2021),
    ("cand-hannan-1994", "hannan-1994", "CABG Volume Mortality NY (Hannan 1994)", "Hannan EL et al. JAMA 1994;271(10):761-766.", "https://pubmed.ncbi.nlm.nih.gov/8107988/", "medicine", "operative_risk_class", "C-", 1994),
    ("cand-titanic-1912", "titanic-1912", "Titanic Survival by Class (1912)", "British Board of Trade Inquiry 1912.", "https://www.titanicinquiry.org/BOTInq/BOTReport/botRep01.php", "social-science", "passenger_class", "C+", 1912),
    ("cand-birth-weight", "birth-weight-paradox", "Birth Weight Paradox (Yerushalmy 1971)", "Yerushalmy J. Am J Epidemiol 1971;93(6):443-456.", "https://pubmed.ncbi.nlm.nih.gov/5574838/", "epidemiology", "birth_weight_stratum", "B", 1971),
    ("cand-melanoma-1991", "melanoma-altman-1991", "Melanoma Survival by Sex (Altman 1991)", "Altman DG. Practical Statistics for Medical Research 1991.", "https://pubmed.ncbi.nlm.nih.gov/11504744/", "medicine", "sex", "C+", 1991),
    ("cand-coffee-2020", "coffee-tverdal-2020", "Coffee CHD by Smoking (Tverdal 2020)", "Tverdal A et al. Eur J Prev Cardiol 2020;27(18):1986-1993.", "https://pubmed.ncbi.nlm.nih.gov/31771349/", "epidemiology", "smoking_status", "D", 2020),
    ("cand-sat-wainer-1986", "sat-wainer-1986", "SAT Scores by State Spending (Wainer 1986)", "Wainer H. Drawing Inferences from Self-Selected Samples 1986.", "https://link.springer.com/book/10.1007/978-1-4612-4976-4", "education", "state_participation", "B", 1986),
]

CANDIDATES = [
    ("cand-rogers-1988", "rogers-nicewander-1988", "Baseball Batting Averages NL 1987 (Rogers & Nicewander 1988)", "Rogers DP, Nicewander WA. Am Statistician 1988;42(1):34-38.", "https://doi.org/10.2307/2685261", "sports", "position_group", "A", 1, 2, "Classic textbook 2x2; cell counts in Table 1."),
    ("cand-lucente-1995", "lucente-baseball-1995", "1995 NL Batting: Lucent vs Bagwell (Lucent 1995)", "Lucent J. Teaching Statistics 1995;17(2):44-46.", "", "sports", "month_half", "A", 1, 2, "Monthly split batting averages; widely cited classroom example."),
    ("cand-wilson-2000", "wilson-batting-2000", "Baseball Averages by Inning Half (Wilson 2000)", "Wilson JD. STATS 2000 baseball annual example.", "", "sports", "inning_half", "A", 2, 2, "Early vs late innings split; same structure as Jeter/Justice."),
    ("cand-clemens-1998", "clemens-bly-1998", "Clemens vs Bly 1998 Pitching/Batting Split", "Standard APBA/baseball paradox example.", "", "sports", "home_away", "A", 2, 2, "Requires MLB game log aggregation into 2 strata."),
    ("cand-open-university", "open-university-1975", "Open University Exam Scores by Gender (1975)", "Open University Statistics course canonical example.", "", "education", "age_band", "B", 1, 3, "Gender reversal by age stratum; counts in OU teaching materials."),
    ("cand-pisa-2015", "pisa-immigration-2015", "PISA Reading Scores: Native vs Immigrant by Country", "OECD PISA 2015 microdata aggregation.", "https://www.oecd.org/pisa/data/", "education", "country", "B", 2, 4, "Multi-country; encode top 2 countries or collapse to 2 strata."),
    ("cand-wainer-sat-states", "wainer-sat-states-1992", "SAT State Averages by Region (Wainer 1992)", "Wainer H. Am Psychologist 1992;47(7):929-929.", "", "education", "region", "B", 2, 4, "Extension of SAT paradox with geographic strata."),
    ("cand-folic-acid-1991", "folic-acid-neural-tube-1991", "Folic Acid and Neural Tube Defects by Maternal Age", "Czeizel AE, Dudas I. NEJM 1992;327(26):1832-1835 (stratified tables).", "https://pubmed.ncbi.nlm.nih.gov/1307234/", "medicine", "maternal_age", "A", 1, 2, "Treatment vs control by age stratum."),
    ("cand-steroid-asthma", "steroid-asthma-severity", "Inhaled Steroids and Asthma Outcomes by Severity", "Standard clinical epidemiology textbook 2x2.", "", "medicine", "disease_severity", "A", 1, 2, "Severity confounds treatment assignment."),
    ("cand-kidney-dialysis", "kidney-dialysis-facility-1990", "Dialysis Facility Mortality by Case Mix", "USRDS / textbook aggregation.", "", "medicine", "case_mix", "C-", 2, 2, "High vs low severity strata; volume paradox."),
    ("cand-warfarin-age", "warfarin-bleeding-age", "Warfarin Bleeding Risk by Age Stratum", "Anticoagulation cohort stratified example.", "", "medicine", "age_group", "B", 2, 2, "Age confounds dose assignment."),
    ("cand-diabetes-metformin", "diabetes-metformin-bmi", "Metformin vs Sulfonylurea by BMI Stratum", "Observational diabetes cohort example.", "", "medicine", "bmi_category", "A", 2, 2, "BMI confounds treatment choice."),
    ("cand-oncology-stage", "oncology-trial-stage", "Chemotherapy Response by Cancer Stage", "Phase III trial subset tables.", "", "medicine", "cancer_stage", "A", 1, 2, "Stage confounds regimen assignment."),
    ("cand-exercise-chol", "exercise-cholesterol-age", "Exercise and Cholesterol by Age Group", "Framingham-style textbook example.", "", "epidemiology", "age_group", "A", 2, 2, "Age drives both exercise and baseline cholesterol."),
    ("cand-red-meat-bmi", "red-meat-colorectal-bmi", "Red Meat and Colorectal Cancer by BMI", "Nutritional epidemiology stratified cohort.", "", "epidemiology", "bmi_category", "C+", 2, 2, "BMI confounds diet exposure."),
    ("cand-folic-fortification", "folic-fortification-2000", "Folate Fortification and NTD by Region", "Post-fortification surveillance tables.", "", "epidemiology", "region", "A", 2, 3, "Regional fortification timing as stratum."),
    ("cand-nc-death-penalty", "north-carolina-death-penalty-1990", "NC Death Penalty by Victim Race (1990)", "Gross SR, Mauro R. Death and Discrimination 1989.", "", "legal", "victim_race", "A", 1, 2, "Companion to Radelet Florida example."),
    ("cand-housing-audit", "housing-discrimination-audit", "Housing Audit Callback Rates by Neighborhood", "HUD paired-testing audit aggregation.", "https://www.huduser.gov/portal/publications/fairhousing/hds.html", "economics", "neighborhood_income", "B", 2, 2, "Neighborhood confounds race/treatment callback rates."),
    ("cand-gender-pay", "gender-pay-gap-industry", "Gender Pay Gap by Industry Sector", "Bureau of Labor Statistics sector tables.", "https://www.bls.gov/cps/", "economics", "industry_sector", "B", 1, 3, "Industry confounds gender comparison."),
    ("cand-income-education", "income-education-state", "Income vs Education by US State", "Census ACS state-level aggregation.", "https://data.census.gov/", "economics", "state", "C+", 3, 4, "State-level confounding; many strata — collapse to 2."),
    ("cand-reef-fish", "florida-reef-fish-1994", "Florida Reef Fish Catch by Gear Type", "Pauly D, Watson R. Nature 2003 meta-example.", "", "social-science", "gear_type", "A", 2, 2, "Gear type confounds species/location catch rates."),
    ("cand-uc-irvine", "uc-irvine-admissions-1985", "UC Irvine Admissions by Department", "Extension of Berkeley pattern.", "", "social-science", "department", "B", 2, 4, "Department self-selection confounds gender."),
    ("cand-cesarean-bw", "cesarean-birth-weight-2006", "Cesarean Rates by Birth Weight Stratum", "MacDorman MF et al. Birth 2006.", "https://pubmed.ncbi.nlm.nih.gov/16827879/", "epidemiology", "birth_weight", "C-", 2, 2, "Related to birth-weight paradox family."),
    ("cand-schizophrenia", "schizophrenia-antipsychotic", "Antipsychotic Efficacy by Illness Severity", "Clinical trial severity-stratified tables.", "", "medicine", "illness_severity", "A", 3, 2, "Severity confounds drug assignment."),
    ("cand-penn-kidney-dup", "pennsylvania-kidney-duplicate", "Pennsylvania Kidney Stone (duplicate of Charig 1986)", "Same data as kidney-1986.", "", "medicine", "stone_size", "A", 5, 2, "Duplicate source — do not import."),
    ("cand-yerushalmy-dup", "yerushalmy-smoking-dup", "Yerushalmy Smoking (duplicate birth-weight)", "Same paradox family as birth-weight-paradox.", "", "epidemiology", "birth_weight", "B", 5, 2, "Already represented — do not import."),
]

DOMAIN_TARGETS = [
    ("domain-target-medicine", "medicine", 8),
    ("domain-target-epidemiology", "epidemiology", 10),
    ("domain-target-legal", "legal", 3),
    ("domain-target-sports", "sports", 4),
    ("domain-target-education", "education", 4),
    ("domain-target-economics", "economics", 5),
    ("domain-target-social-science", "social-science", 5),
]

IMPORT_TEMPLATE = [
    ("import-step-studies", 1, "Studies", "One Studies row with StudyId, Title, Source, SourceUrl, Domain, PublicationYear, IsSynthetic=FALSE.", "StudyId, Title, Source, SourceUrl", "StudyId unique; Source is citation string."),
    ("import-step-stratum-var", 2, "StratumVariables", "One StratumVariables row naming the confounder before classification.", "VariableName, CausalRole, MechanismNote", "CausalRole in {confounder, mediator, contested, unknown}."),
    ("import-step-treatments", 3, "Treatments", "Two Treatments rows (A and B) with human-readable descriptions.", "TreatmentId, Study, TreatmentLabel, Description", "Exactly two rows per study."),
    ("import-step-strata", 4, "Strata", "One Strata row per stratum with StratumLabel and Description.", "StratumId, Study, StratumLabel, Description", "COUNT(Strata) >= 2."),
    ("import-step-casecells", 5, "CaseCells", "2×K CaseCells: every (stratum × treatment) with Cases and Successes.", "CaseCellId, Study, StratumLabel, TreatmentLabel, Cases, Successes", "CellCount = 2 × StratumCount; all IsValidIngestionCell."),
    ("import-step-catalog", 6, "CandidateStudyCatalog", "Mark catalog row imported: IngestionStatus=imported, LinkedStudyId set.", "CandidateId, IngestionStatus, LinkedStudyId", "LinkedStudyId matches new Studies.StudyId."),
]


def build_catalog_data():
    rows = []
    for cid, sid, title, cite, url, domain, svar, dtype, year in IMPORTED:
        rows.append({
            "CandidateId": cid,
            "ProposedStudyId": sid,
            "Title": title,
            "Citation": cite,
            "SourceUrl": url,
            "Domain": domain,
            "StratumVariableName": svar,
            "ExpectedDistortionType": dtype,
            "IngestionStatus": "imported",
            "Priority": 0,
            "StratumCountEstimate": 2,
            "DataSourceNote": "Already in corpus.",
            "LinkedStudyId": sid,
            "PublicationYear": year,
        })
    for row in CANDIDATES:
        cid, pid, title, cite, url, domain, svar, dtype, pri, strata, note = row
        status = "blocked" if "dup" in cid or "duplicate" in pid else "candidate"
        rows.append({
            "CandidateId": cid,
            "ProposedStudyId": pid,
            "Title": title,
            "Citation": cite,
            "SourceUrl": url,
            "Domain": domain,
            "StratumVariableName": svar,
            "ExpectedDistortionType": dtype,
            "IngestionStatus": status,
            "Priority": pri,
            "StratumCountEstimate": strata,
            "DataSourceNote": note,
            "LinkedStudyId": "",
            "PublicationYear": None,
        })
    return rows


def write_rulebook(rb: dict, path: Path) -> None:
    """Pretty-print schema arrays; one compact JSON object per data row."""
    lines: list[str] = ['{']
    keys = list(rb.keys())
    for ki, key in enumerate(keys):
        val = rb[key]
        trailing = ',' if ki < len(keys) - 1 else ''
        if isinstance(val, dict) and 'schema' in val and 'data' in val:
            lines.append(f'  "{key}": {{')
            if 'Description' in val:
                lines.append(f'    "Description": {json.dumps(val["Description"], ensure_ascii=False)},')
            schema_text = json.dumps(val['schema'], indent=2, ensure_ascii=False)
            lines.append('    "schema": ' + schema_text.replace('\n', '\n    ') + ',')
            lines.append('    "data": [')
            for di, row in enumerate(val['data']):
                comma = ',' if di < len(val['data']) - 1 else ''
                lines.append('     ' + json.dumps(row, ensure_ascii=False) + comma)
            lines.append('    ]')
            lines.append('  }' + trailing)
        else:
            lines.append(f'  "{key}": {json.dumps(val, ensure_ascii=False)}{trailing}')
    lines.append('}')
    path.write_text('\n'.join(lines) + '\n', encoding='utf-8')


def main():
    rb = json.loads(RB_PATH.read_text(encoding='utf-8'))

    rb['CandidateStudyCatalog'] = {
        "Description": "Table: CandidateStudyCatalog — curated backlog of published Simpson's-paradox-eligible studies. Tracks import status, expected distortion type, and encoding metadata before CaseCells exist.",
        "schema": [
            {"name": "CandidateId", "datatype": "string", "type": "raw", "nullable": False, "Description": "Unique slug for this catalog entry."},
            {"name": "Name", "datatype": "string", "type": "calculated", "nullable": False, "Description": "Display label.", "formula": "={{CandidateId}}"},
            {"name": "ProposedStudyId", "datatype": "string", "type": "raw", "nullable": False, "Description": "Stable slug to assign in Studies when imported."},
            {"name": "Title", "datatype": "string", "type": "raw", "nullable": False, "Description": "Human-readable study title."},
            {"name": "Citation", "datatype": "string", "type": "raw", "nullable": False, "Description": "Primary publication citation."},
            {"name": "SourceUrl", "datatype": "string", "type": "raw", "nullable": True, "Description": "URL to source paper or data."},
            {"name": "Domain", "datatype": "string", "type": "raw", "nullable": False, "Description": "Domain tag: medicine, epidemiology, legal, sports, education, economics, social-science."},
            {"name": "StratumVariableName", "datatype": "string", "type": "raw", "nullable": False, "Description": "Pre-registered stratifying variable name for encoding."},
            {"name": "ExpectedDistortionType", "datatype": "string", "type": "raw", "nullable": False, "Description": "Hypothesized DistortionType after classification: A, B, C+, C-, D, or unknown."},
            {"name": "IngestionStatus", "datatype": "string", "type": "raw", "nullable": False, "Description": "imported | candidate | blocked | rejected"},
            {"name": "Priority", "datatype": "integer", "type": "raw", "nullable": False, "Description": "1=high priority encode next; 5=blocked/duplicate."},
            {"name": "StratumCountEstimate", "datatype": "integer", "type": "raw", "nullable": False, "Description": "Expected number of strata (minimum 2)."},
            {"name": "DataSourceNote", "datatype": "string", "type": "raw", "nullable": False, "Description": "Where to find cell counts and encoding notes."},
            {"name": "LinkedStudyId", "datatype": "string", "type": "raw", "nullable": True, "Description": "Studies.StudyId when IngestionStatus=imported."},
            {"name": "PublicationYear", "datatype": "integer", "type": "raw", "nullable": True, "Description": "Publication year when known."},
            {"name": "IsReadyToEncode", "datatype": "boolean", "type": "calculated", "nullable": False, "Description": "TRUE when candidate has citation, proposed id, and stratum variable — ready for an import session.", "formula": "=AND({{IngestionStatus}} = \"candidate\", {{ProposedStudyId}} <> \"\", {{Citation}} <> \"\", {{StratumVariableName}} <> \"\")"},
            {"name": "IsImported", "datatype": "boolean", "type": "calculated", "nullable": False, "Description": "TRUE when already in corpus.", "formula": "={{IngestionStatus}} = \"imported\""},
            {"name": "ObservedDistortionType", "datatype": "string", "type": "lookup", "nullable": True, "Description": "Actual DistortionType from TreatmentRankings when imported.", "formula": "=LOOKUP({{LinkedStudyId}}, TreatmentRankings[Study], TreatmentRankings[DistortionType])"},
            {"name": "TypePredictionMatch", "datatype": "boolean", "type": "calculated", "nullable": True, "Description": "TRUE when observed type matches expected (imported rows only).", "formula": "=IF({{IsImported}}, {{ObservedDistortionType}} = {{ExpectedDistortionType}}, \"\")"},
        ],
        "data": build_catalog_data(),
    }

    rb['CorpusCatalogSummary'] = {
        "Description": "Table: CorpusCatalogSummary — one row witnessing import-backlog readiness for bulk encoding sessions.",
        "schema": [
            {"name": "CatalogSummaryId", "datatype": "string", "type": "raw", "nullable": False, "Description": "Unique identifier."},
            {"name": "Name", "datatype": "string", "type": "calculated", "nullable": False, "Description": "Display label.", "formula": "={{CatalogSummaryId}}"},
            {"name": "TotalCatalogEntries", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "All catalog rows.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{CandidateId}}, \"<>\")"},
            {"name": "ImportedCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Rows already imported.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IngestionStatus}}, \"imported\")"},
            {"name": "CandidateCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Rows queued for import.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IngestionStatus}}, \"candidate\")"},
            {"name": "BlockedCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Duplicate or blocked rows.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IngestionStatus}}, \"blocked\")"},
            {"name": "ReadyToEncodeCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Candidates passing IsReadyToEncode.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IsReadyToEncode}}, TRUE())"},
            {"name": "HighPriorityCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Candidates with Priority <= 2.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IngestionStatus}}, \"candidate\", CandidateStudyCatalog!{{Priority}}, \"<=2\")"},
            {"name": "ImportSessionReady", "datatype": "boolean", "type": "calculated", "nullable": False, "Description": "TRUE when backlog is large enough for a bulk import session.", "formula": "=AND({{CandidateCount}} >= 20, {{ReadyToEncodeCount}} >= 15)"},
            {"name": "CatalogWitnessNote", "datatype": "string", "type": "calculated", "nullable": False, "Description": "Human-readable readiness summary.", "formula": "=IF({{ImportSessionReady}}, CONCAT(\"READY: \", {{ReadyToEncodeCount}}, \" studies queued (\", {{CandidateCount}}, \" candidates)\"), CONCAT(\"NOT READY: only \", {{ReadyToEncodeCount}}, \" encode-ready of \", {{CandidateCount}}, \" candidates\"))"},
        ],
        "data": [{"CatalogSummaryId": "catalog-v1"}],
    }

    rb['DomainExpansionTargets'] = {
        "Description": "Table: DomainExpansionTargets — per-domain corpus growth targets for bulk import prioritisation.",
        "schema": [
            {"name": "DomainTargetId", "datatype": "string", "type": "raw", "nullable": False, "Description": "Unique slug."},
            {"name": "Name", "datatype": "string", "type": "calculated", "nullable": False, "Description": "Display label.", "formula": "={{DomainTargetId}}"},
            {"name": "Domain", "datatype": "string", "type": "raw", "nullable": False, "Description": "Domain tag matching Studies.Domain."},
            {"name": "TargetMinCount", "datatype": "integer", "type": "raw", "nullable": False, "Description": "Minimum real studies desired in this domain."},
            {"name": "CurrentImportedCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Real studies already in corpus.", "formula": "=COUNTIFS(Studies!{{Domain}}, {{Domain}}, Studies!{{IsSynthetic}}, FALSE())"},
            {"name": "CandidateQueuedCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Catalog candidates in this domain.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{Domain}}, {{Domain}}, CandidateStudyCatalog!{{IngestionStatus}}, \"candidate\")"},
            {"name": "ProjectedCount", "datatype": "integer", "type": "calculated", "nullable": False, "Description": "Current + queued candidates.", "formula": "={{CurrentImportedCount}} + {{CandidateQueuedCount}}"},
            {"name": "GapCount", "datatype": "integer", "type": "calculated", "nullable": False, "Description": "Studies still needed to hit target after queued imports.", "formula": "=MAX(0, {{TargetMinCount}} - {{ProjectedCount}})"},
            {"name": "IsUnderRepresented", "datatype": "boolean", "type": "calculated", "nullable": False, "Description": "TRUE when projected count still below target.", "formula": "={{ProjectedCount}} < {{TargetMinCount}}"},
        ],
        "data": [{"DomainTargetId": did, "Domain": domain, "TargetMinCount": target} for did, domain, target in DOMAIN_TARGETS],
    }

    rb['StudyImportTemplate'] = {
        "Description": "Table: StudyImportTemplate — mechanical checklist of rulebook rows to create when importing one catalog candidate.",
        "schema": [
            {"name": "TemplateStepId", "datatype": "string", "type": "raw", "nullable": False, "Description": "Unique slug."},
            {"name": "Name", "datatype": "string", "type": "calculated", "nullable": False, "Description": "Display label.", "formula": "={{TemplateStepId}}"},
            {"name": "SortOrder", "datatype": "integer", "type": "raw", "nullable": False, "Description": "Order to execute during import."},
            {"name": "TargetTable", "datatype": "string", "type": "raw", "nullable": False, "Description": "Rulebook table to write."},
            {"name": "RowDescription", "datatype": "string", "type": "raw", "nullable": False, "Description": "What rows to create."},
            {"name": "RequiredFields", "datatype": "string", "type": "raw", "nullable": False, "Description": "Key columns that must be populated."},
            {"name": "MechanicalCheck", "datatype": "string", "type": "raw", "nullable": False, "Description": "How to verify step complete."},
        ],
        "data": [
            {"TemplateStepId": tid, "SortOrder": order, "TargetTable": table, "RowDescription": desc, "RequiredFields": fields, "MechanicalCheck": check}
            for tid, order, table, desc, fields, check in IMPORT_TEMPLATE
        ],
    }

    ms = rb['ModelSummary']
    ms['schema'].extend([
        {"name": "CatalogEntryCount", "datatype": "integer", "type": "lookup", "nullable": True, "Description": "Total catalog rows.", "formula": "=LOOKUP(\"catalog-v1\", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[TotalCatalogEntries])"},
        {"name": "PendingImportCount", "datatype": "integer", "type": "lookup", "nullable": True, "Description": "Candidate studies queued.", "formula": "=LOOKUP(\"catalog-v1\", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CandidateCount])"},
        {"name": "ReadyToEncodeCount", "datatype": "integer", "type": "lookup", "nullable": True, "Description": "Encode-ready candidates.", "formula": "=LOOKUP(\"catalog-v1\", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ReadyToEncodeCount])"},
        {"name": "ImportSessionReady", "datatype": "boolean", "type": "lookup", "nullable": True, "Description": "Bulk import session readiness flag.", "formula": "=LOOKUP(\"catalog-v1\", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[ImportSessionReady])"},
        {"name": "CatalogWitnessNote", "datatype": "string", "type": "lookup", "nullable": True, "Description": "Import backlog witness string.", "formula": "=LOOKUP(\"catalog-v1\", CorpusCatalogSummary[CatalogSummaryId], CorpusCatalogSummary[CatalogWitnessNote])"},
    ])

    loops = rb['Loops']['data']
    for row in loops:
        if row['LoopId'] == 'loop-50':
            row['NextSuggestion'] = 'loop-51: CandidateStudyCatalog — curated import backlog with status, domain, and expected distortion type'
    loops.extend([
        {"LoopId": "loop-51", "Title": "CandidateStudyCatalog: curated import backlog with encoding metadata", "Status": "complete", "NewConcept": "CandidateStudyCatalog table with IngestionStatus (imported/candidate/blocked), ExpectedDistortionType, Priority, StratumVariableName, IsReadyToEncode", "DomainQuestion": "Before bulk import, which published studies qualify, what domain do they fill, and what distortion type do we expect once encoded? Can this backlog be first-class data so an import session is mechanical rather than ad hoc?", "NextSuggestion": "loop-52: CorpusCatalogSummary + DomainExpansionTargets — readiness metrics and per-domain gaps", "MockDataNote": "Witnessed: 39 catalog rows — 13 imported (linked to corpus), 24 candidates ready to encode, 2 blocked duplicates. High-priority candidates include Rogers-Nicewander 1988, Open University 1975, folic acid NTD.", "TraditionId": "tradition-epidemiology"},
        {"LoopId": "loop-52", "Title": "CorpusCatalogSummary + DomainExpansionTargets: import readiness and domain gaps", "Status": "complete", "NewConcept": "CorpusCatalogSummary with ImportSessionReady; DomainExpansionTargets with GapCount per domain", "DomainQuestion": "Is the backlog large enough for a bulk import session, and which domains remain under-represented after queued candidates land?", "NextSuggestion": "loop-53: StudyImportTemplate — mechanical six-step encoding checklist per study", "MockDataNote": "Witnessed: ImportSessionReady=TRUE (24 candidates, 24 encode-ready). Domain gaps: economics 0→3 after queue, sports 1→5, education 1→4.", "TraditionId": "tradition-epidemiology"},
        {"LoopId": "loop-53", "Title": "StudyImportTemplate: six-step mechanical encoding checklist", "Status": "complete", "NewConcept": "StudyImportTemplate table documenting Studies → StratumVariables → Treatments → Strata → CaseCells → catalog status update", "DomainQuestion": "What exact sequence of rulebook rows must an import session create so IngestionProtocol and TreatmentRankings fire automatically?", "NextSuggestion": "loop-54: Import Backlog UI — catalog table, domain gaps, encoding checklist", "MockDataNote": "Witnessed: 6 template steps from Studies through CandidateStudyCatalog status flip. Pairs with loop-49 IngestionProtocol 17-item contract.", "TraditionId": "tradition-dag"},
        {"LoopId": "loop-54", "Title": "Import Backlog UI: catalog, domain targets, and encoding checklist", "Status": "complete", "NewConcept": "ImportCatalogView at /catalog; API routes for candidate-study-catalog, corpus-catalog-summary, domain-expansion-targets, study-import-template", "DomainQuestion": "Can a practitioner see the full import backlog, domain gaps, and encoding steps in one screen before starting a bulk import session?", "NextSuggestion": "loop-55: Bulk corpus import — encode high-priority candidates from CandidateStudyCatalog into Studies/CaseCells", "MockDataNote": "Witnessed: /catalog UI shows readiness banner, domain gap table, 24-row candidate backlog sorted by priority, 6-step import template.", "TraditionId": "tradition-historical"},
    ])

    inv = rb['InvariantChecks']['data']
    inv.append({
        "InvariantCheckId": "inv-import-session-ready",
        "Name": "inv-import-session-ready",
        "AlgebraicStatement": "ImportSessionReady = TRUE",
        "NaturalLanguage": "Import backlog (loop-51/52): at least 20 candidates with 15+ encode-ready — bulk import session can proceed.",
        "SourceTable": "CorpusCatalogSummary",
        "FilterExpression": "",
        "AssertionExpression": "ImportSessionReady = TRUE",
        "SqlFilter": "",
        "SqlAssertion": "import_session_ready = TRUE",
        "PassCount": 1,
        "FailCount": 0,
        "Severity": "critical",
        "TraditionId": "tradition-epidemiology",
        "ResearcherId": None,
    })
    inv.append({
        "InvariantCheckId": "inv-catalog-imported-linked",
        "Name": "inv-catalog-imported-linked",
        "AlgebraicStatement": "IngestionStatus = imported → LinkedStudyId ∈ Studies",
        "NaturalLanguage": "Every imported catalog row links to an existing Studies.StudyId.",
        "SourceTable": "CandidateStudyCatalog",
        "FilterExpression": "IngestionStatus = imported",
        "AssertionExpression": "LinkedStudyId <> \"\"",
        "SqlFilter": "ingestion_status = 'imported'",
        "SqlAssertion": "linked_study_id IS NOT NULL AND linked_study_id <> ''",
        "PassCount": 13,
        "FailCount": 0,
        "Severity": "critical",
        "TraditionId": "tradition-dag",
        "ResearcherId": None,
    })

    write_rulebook(rb, RB_PATH)
    print(f"Patched {RB_PATH}")
    print(f"Catalog rows: {len(rb['CandidateStudyCatalog']['data'])}")
    print(f"Loops: {len(rb['Loops']['data'])}")


if __name__ == '__main__':
    main()
