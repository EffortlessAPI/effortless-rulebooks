#!/usr/bin/env python3
"""Loop-67: ingest corpus-expansion-plan.md into CandidateStudyCatalog."""

from __future__ import annotations

import json
import re
import unicodedata
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
RULEBOOK = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"
PLAN = ROOT / "corpus-expansion-plan.md"

DOMAIN_MAP = {
    "economics": "economics",
    "criminal justice": "criminal-justice",
    "higher education": "higher-education",
    "clinical trials": "clinical-trials",
    "sports analytics": "sports-analytics",
    "epidemiology / vaccines": "epidemiology-vaccines",
    "epidemiology/vaccines": "epidemiology-vaccines",
    "hiring & promotion": "hiring-promotion",
    "online a/b testing": "online-ab-testing",
    "public health / smoking": "public-health-smoking",
    "education outcomes": "education-outcomes",
    "financial lending": "financial-lending",
}

DOMAIN_TARGETS = {
    "economics": 25,
    "criminal-justice": 15,
    "higher-education": 15,
    "clinical-trials": 15,
    "sports-analytics": 15,
    "epidemiology-vaccines": 20,
    "hiring-promotion": 12,
    "online-ab-testing": 10,
    "public-health-smoking": 12,
    "education-outcomes": 15,
    "financial-lending": 12,
}

LEGACY_DOMAIN_ALIAS = {
    "criminal-justice": "legal",
    "higher-education": "education",
    "sports-analytics": "sports",
    "epidemiology-vaccines": "epidemiology",
    "education-outcomes": "education",
    "clinical-trials": "medicine",
}

CLASSIC_MARKERS = ("CLASSIC", "classic", "foundational", "canonical")

# Manual dedup: expansion-plan study key -> existing study/catalog id
KNOWN_DEDUP = {
    "jeter": "jeter-justice-1997",
    "justice": "jeter-justice-1997",
    "bickel": "berkeley-1973",
    "berkeley": "berkeley-1973",
    "charig": "kidney-1986",
    "kidney stone": "kidney-1986",
    "cochran": "cochran-smoking-1968",
    "surgeon general": "cochran-smoking-1968",
    "radelet": "radelet-1981",
    "ross 2007": "jeter-justice-1997",
    "rogers": "rogers-nicewander-1988",
    "nicewander": "rogers-nicewander-1988",
    "appleton": "appleton-1996",
    "whickham": "appleton-1996",
    "phe": "phe-covid-2021",
    "goldberg y": "israel-covid-severe-2021",
    "israel covid": "israel-covid-severe-2021",
    "pisa": "pisa-immigration-2015",
    "open university": "open-university-1975",
    "wainer 1986": "sat-wainer-1986",
    "wainer h": "sat-wainer-1986",
    "titanic": "titanic-1912",
    "reintjes": "reintjes-2000",
    "birth weight paradox": "birth-weight-paradox",
    "yerushalmy": "birth-weight-paradox",
    "nissen": "rosiglitazone-mi-pool-2007",
    "rosiglitazone": "rosiglitazone-mi-pool-2007",
    "hannan": "hannan-1994",
    "folic acid": "folic-acid-neural-tube-1991",
    "czeizel": "folic-acid-neural-tube-1991",
    "coffee-smoking": "coffee-smoking-lung-1968",
    "coffee and lung": "coffee-smoking-lung-1968",
    "panama": "panama-sweden-mortality-1975",
    "sweden": "panama-sweden-mortality-1975",
    "hanley": "hanley-power-lines-2000",
    "power lines": "hanley-power-lines-2000",
    "compas": "compas-recidivism-base",
    "chouldechova": "compas-recidivism-base",
    "ridgeway": "nypd-stop-frisk-hitrate",
    "stop-and-frisk": "nypd-stop-frisk-hitrate",
    "star class": "star-class-size",
    "tennessee star": "star-class-size",
    "hrt": "hrt-whi-2002",
    "whi": "hrt-whi-2002",
    "snow": "snow-cholera-water-1855",
    "cholera": "snow-cholera-water-1855",
    "robinson 1950": "robinson-1950-literacy",
    "nba": "nba-shooting-shot-mix",
    "nfl completion": "nfl-completion-depth",
    "field goal": "field-goal-distance",
    "ironman": "ironman-gender-age-2010",
    "triathlon": "ironman-gender-age-2010",
    "borjas": "borjas-immigrant-cohort",
    "minimum wage": "minimum-wage-region",
    "dube": "minimum-wage-region",
    "housing audit": "housing-discrimination-audit",
    "hud": "housing-discrimination-audit",
    "bertrand": "housing-discrimination-audit",
    "emily and greg": "housing-discrimination-audit",
    "goldin": "gender-pay-gap-industry",
    "gender wage": "gender-pay-gap-industry",
    "blind audition": "gender-pay-gap-industry",
    "goldin & rouse": "gender-pay-gap-industry",
}


def slugify(text: str) -> str:
    text = unicodedata.normalize("NFKD", text).encode("ascii", "ignore").decode("ascii")
    text = text.lower()
    text = re.sub(r"[^a-z0-9]+", "-", text).strip("-")
    return text[:60] or "study"


def extract_url(line: str) -> str:
    m = re.search(r"\((https?://[^)]+)\)", line)
    return m.group(1) if m else ""


def stratum_slug(text: str) -> str:
    base = slugify(text.split("/")[0].split("×")[0].strip())
    return base[:40] or "stratum"


def guess_distortion(reversal: str, title: str) -> str:
    t = (reversal + " " + title).lower()
    if "does not reverse" in t or "type-c compression" in t or "illustrating a type-c" in t:
        return "C-"
    if "compress" in t or "inflate" in t or "amplification" in t:
        return "C+"
    if "partial" in t or "mixed" in t or "contested" in t:
        return "B"
    if "no distortion" in t or "null" in t and "reversal" not in t:
        return "D"
    if "revers" in t or "paradox" in t or "flip" in t or "invert" in t:
        return "A"
    return "unknown"


def guess_confirmation(title: str, reversal: str) -> str:
    t = title + " " + reversal
    if any(m in t for m in CLASSIC_MARKERS):
        return "confirmed"
    if "revers" in reversal.lower() or "paradox" in reversal.lower():
        return "plausible"
    return "pending"


def guess_acquisition(data_line: str, source_url: str) -> str:
    dl = data_line.lower()
    if "contact authors" in dl or "summarized in" in dl and not source_url:
        return "manual_only"
    if source_url or "replication" in dl or "openintro" in dl or "ipums" in dl or "public" in dl:
        return "not_started"
    return "manual_only"


def priority_for(confirmation: str) -> int:
    return {"confirmed": 1, "plausible": 2, "pending": 3, "not_applicable": 4}.get(confirmation, 3)


def parse_plan(text: str) -> list[dict]:
    studies: list[dict] = []
    current_domain = ""
    current: dict | None = None

    for raw in text.splitlines():
        line = raw.strip()
        dm = re.match(r"^## Domain \d+:\s*(.+?)\s*\(\d+ studies\)", line)
        if dm:
            current_domain = DOMAIN_MAP.get(dm.group(1).strip().lower(), slugify(dm.group(1)))
            continue
        if line.startswith("## Summary") or line.startswith("## Next Steps"):
            break
        sm = re.match(r"^(\d+)\.\s+\*\*(.+?)\*\*(.*)$", line)
        if sm:
            if current:
                studies.append(current)
            title_core = sm.group(2).strip()
            rest = sm.group(3).strip()
            if "listed in" in title_core.lower() or title_core.startswith("*"):
                current = None
                continue
            year_m = re.search(r"\((\d{4})", title_core + rest)
            year = int(year_m.group(1)) if year_m else None
            full_title = title_core
            if rest.startswith("—") or rest.startswith("-"):
                full_title = f"{title_core} {rest.lstrip('—- ')}"
            citation = full_title
            if '"' in rest:
                qm = re.search(r'"([^"]+)"', rest)
                if qm:
                    citation = f"{title_core}. \"{qm.group(1)}.\""
            current = {
                "domain": current_domain,
                "title": full_title[:240],
                "citation": citation[:400],
                "publication_year": year,
                "stratum_variable_name": "stratum",
                "data_source_note": "",
                "reversal_mechanism": "",
                "source_url": "",
                "measures": "",
            }
            continue
        if not current:
            continue
        if line.startswith("- **Measures:**"):
            current["measures"] = line.split(":", 1)[1].strip()
        elif line.startswith("- **Stratum:**"):
            current["stratum_variable_name"] = stratum_slug(line.split(":", 1)[1].strip())
        elif line.startswith("- **Data:**"):
            data = line.split(":", 1)[1].strip()
            current["data_source_note"] = data[:500]
            current["source_url"] = extract_url(line)
        elif line.startswith("- **Reversal:**"):
            current["reversal_mechanism"] = line.split(":", 1)[1].strip()[:500]

    if current:
        studies.append(current)
    return [s for s in studies if s.get("domain")]


def proposed_id(study: dict, used: set[str]) -> str:
    title = study["title"].lower()
    for key, existing in KNOWN_DEDUP.items():
        if key in title:
            return existing
    author = study["title"].split("(")[0].split("—")[0].strip()
    year = study.get("publication_year") or ""
    base = slugify(f"{author}-{year}") if year else slugify(author)
    if not base:
        base = slugify(study["title"][:40])
    pid = base
    n = 2
    while pid in used:
        pid = f"{base}-{n}"
        n += 1
    used.add(pid)
    return pid


def is_duplicate(study: dict, pid: str, existing_ids: set[str], existing_titles: set[str]) -> bool:
    if pid in existing_ids:
        return True
    title_l = study["title"].lower()
    for key, eid in KNOWN_DEDUP.items():
        if key in title_l and eid in existing_ids:
            return True
    norm = re.sub(r"[^a-z0-9]", "", title_l)[:50]
    for t in existing_titles:
        if norm and norm in re.sub(r"[^a-z0-9]", "", t.lower()):
            return True
    return False


def compact_json(obj: dict) -> str:
    lines = ["{"]
    top_keys = list(obj.keys())
    for ki, key in enumerate(top_keys):
        val = obj[key]
        trail = "," if ki < len(top_keys) - 1 else ""
        if isinstance(val, dict) and "data" in val:
            head = json.dumps({k: v for k, v in val.items() if k != "data"}, indent=2, ensure_ascii=False)
            if head.endswith("\n}"):
                head = head[:-2]
            elif head.endswith("}"):
                head = head[:-1]
            data_rows = ",\n".join("      " + json.dumps(r, ensure_ascii=False) for r in val["data"])
            block = f'  "{key}": {head},\n    "data": [\n{data_rows}\n    ]\n  }}'
            lines.append(block + trail)
        else:
            chunk = json.dumps(val, indent=2, ensure_ascii=False)
            lines.append(f'  "{key}": {chunk}{trail}')
    lines.append("}")
    return "\n".join(lines) + "\n"


def main() -> None:
    rb = json.loads(RULEBOOK.read_text(encoding="utf-8"))
    plan_studies = parse_plan(PLAN.read_text(encoding="utf-8"))

    existing_study_ids = {r["StudyId"] for r in rb["Studies"]["data"]}
    existing_catalog = rb["CandidateStudyCatalog"]["data"]
    existing_pids = {r["ProposedStudyId"] for r in existing_catalog}
    existing_titles = {r["Title"] for r in existing_catalog} | {r["Title"] for r in rb["Studies"]["data"]}
    existing_ids = existing_study_ids | existing_pids

    for loop in rb["Loops"]["data"]:
        if loop["LoopId"] in ("loop-65", "loop-66"):
            loop["Status"] = "complete"

    rb["CorpusDomains"] = {
        "Description": "Table: CorpusDomains — canonical expansion-domain slugs from corpus-expansion-plan.md (loop-67).",
        "schema": [
            {"name": "DomainId", "datatype": "string", "type": "raw", "nullable": False, "Description": "Unique slug (matches CandidateStudyCatalog.Domain for expansion wave)."},
            {"name": "Name", "datatype": "string", "type": "calculated", "nullable": False, "Description": "Display label.", "formula": "={{DomainId}}"},
            {"name": "DisplayName", "datatype": "string", "type": "raw", "nullable": False, "Description": "Human-readable domain name."},
            {"name": "ExpansionTarget", "datatype": "integer", "type": "raw", "nullable": False, "Description": "Target study count from corpus-expansion-plan.md."},
            {"name": "LegacyStudiesDomain", "datatype": "string", "type": "raw", "nullable": True, "Description": "Studies.Domain alias for counting already-imported corpus rows."},
            {"name": "PlanSourceSection", "datatype": "string", "type": "raw", "nullable": False, "Description": "Section heading in corpus-expansion-plan.md."},
        ],
        "data": [
            {"DomainId": "economics", "DisplayName": "Economics", "ExpansionTarget": 25, "LegacyStudiesDomain": "economics", "PlanSourceSection": "Domain 1: Economics"},
            {"DomainId": "criminal-justice", "DisplayName": "Criminal Justice", "ExpansionTarget": 15, "LegacyStudiesDomain": "legal", "PlanSourceSection": "Domain 2: Criminal Justice"},
            {"DomainId": "higher-education", "DisplayName": "Higher Education", "ExpansionTarget": 15, "LegacyStudiesDomain": "education", "PlanSourceSection": "Domain 3: Higher Education"},
            {"DomainId": "clinical-trials", "DisplayName": "Clinical Trials", "ExpansionTarget": 15, "LegacyStudiesDomain": "medicine", "PlanSourceSection": "Domain 4: Clinical Trials"},
            {"DomainId": "sports-analytics", "DisplayName": "Sports Analytics", "ExpansionTarget": 15, "LegacyStudiesDomain": "sports", "PlanSourceSection": "Domain 5: Sports Analytics"},
            {"DomainId": "epidemiology-vaccines", "DisplayName": "Epidemiology / Vaccines", "ExpansionTarget": 20, "LegacyStudiesDomain": "epidemiology", "PlanSourceSection": "Domain 6: Epidemiology / Vaccines"},
            {"DomainId": "hiring-promotion", "DisplayName": "Hiring & Promotion", "ExpansionTarget": 12, "LegacyStudiesDomain": None, "PlanSourceSection": "Domain 7: Hiring & Promotion"},
            {"DomainId": "online-ab-testing", "DisplayName": "Online A/B Testing", "ExpansionTarget": 10, "LegacyStudiesDomain": "social-science", "PlanSourceSection": "Domain 8: Online A/B Testing"},
            {"DomainId": "public-health-smoking", "DisplayName": "Public Health / Smoking", "ExpansionTarget": 12, "LegacyStudiesDomain": "epidemiology", "PlanSourceSection": "Domain 9: Public Health / Smoking"},
            {"DomainId": "education-outcomes", "DisplayName": "Education Outcomes", "ExpansionTarget": 15, "LegacyStudiesDomain": "education", "PlanSourceSection": "Domain 10: Education Outcomes"},
            {"DomainId": "financial-lending", "DisplayName": "Financial Lending", "ExpansionTarget": 12, "LegacyStudiesDomain": None, "PlanSourceSection": "Domain 11: Financial Lending"},
        ],
    }

    cat_schema = rb["CandidateStudyCatalog"]["schema"]
    new_fields = [
        {"name": "DataAcquisitionStatus", "datatype": "string", "type": "raw", "nullable": False, "Description": "not_started | downloaded | manual_only | blocked"},
        {"name": "ReversalMechanism", "datatype": "string", "type": "raw", "nullable": False, "Description": "Plain-language reversal mechanism from expansion plan."},
        {"name": "ParadoxConfirmation", "datatype": "string", "type": "raw", "nullable": False, "Description": "confirmed | plausible | pending | not_applicable"},
        {"name": "ExpansionWave", "datatype": "string", "type": "raw", "nullable": True, "Description": "Catalog wave tag; loop-67 expansion rows = expansion-wave-1."},
        {"name": "IsDataReady", "datatype": "boolean", "type": "calculated", "nullable": False, "Description": "TRUE when stratified data is downloaded or available from published tables.", "formula": "=OR({{DataAcquisitionStatus}} = \"downloaded\", {{DataAcquisitionStatus}} = \"manual_only\")"},
    ]
    names = {f["name"] for f in cat_schema}
    for f in new_fields:
        if f["name"] not in names:
            cat_schema.append(f)

    for row in existing_catalog:
        if row.get("IngestionStatus") == "imported":
            row["DataAcquisitionStatus"] = "manual_only"
            row["ReversalMechanism"] = row.get("ReversalMechanism") or "Encoded in prior import wave."
            row["ParadoxConfirmation"] = "confirmed" if row.get("ExpectedDistortionType") in ("A", "B") else "plausible"
            row["ExpansionWave"] = None
        elif row.get("IngestionStatus") == "blocked":
            row["DataAcquisitionStatus"] = "blocked"
            row["ReversalMechanism"] = row.get("ReversalMechanism") or "Blocked duplicate."
            row["ParadoxConfirmation"] = "not_applicable"
            row["ExpansionWave"] = None
        else:
            row["DataAcquisitionStatus"] = row.get("DataAcquisitionStatus", "not_started")
            row["ReversalMechanism"] = row.get("ReversalMechanism", "")
            row["ParadoxConfirmation"] = row.get("ParadoxConfirmation", "pending")
            row["ExpansionWave"] = row.get("ExpansionWave")

    used_pids = set(existing_pids)
    added = 0
    skipped = 0
    for study in plan_studies:
        pid = proposed_id(study, used_pids)
        if is_duplicate(study, pid, existing_ids, existing_titles):
            skipped += 1
            continue
        confirmation = guess_confirmation(study["title"], study["reversal_mechanism"])
        row = {
            "CandidateId": f"cand-exp-{pid}",
            "ProposedStudyId": pid,
            "Title": study["title"],
            "Citation": study["citation"],
            "SourceUrl": study["source_url"] or None,
            "Domain": study["domain"],
            "StratumVariableName": study["stratum_variable_name"],
            "ExpectedDistortionType": guess_distortion(study["reversal_mechanism"], study["title"]),
            "IngestionStatus": "candidate",
            "Priority": priority_for(confirmation),
            "StratumCountEstimate": 2,
            "DataSourceNote": study["data_source_note"] or study["measures"] or "See corpus-expansion-plan.md",
            "LinkedStudyId": None,
            "PublicationYear": study["publication_year"],
            "DataAcquisitionStatus": guess_acquisition(study["data_source_note"], study["source_url"]),
            "ReversalMechanism": study["reversal_mechanism"] or "See expansion plan.",
            "ParadoxConfirmation": confirmation,
            "ExpansionWave": "expansion-wave-1",
        }
        existing_catalog.append(row)
        existing_ids.add(pid)
        existing_titles.add(study["title"])
        added += 1

    summary_schema = rb["CorpusCatalogSummary"]["schema"]
    extra_summary = [
        {"name": "DataReadyCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Candidates with IsDataReady=TRUE.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IsDataReady}}, TRUE(), CandidateStudyCatalog!{{IngestionStatus}}, \"candidate\")"},
        {"name": "EncodePipelineReadyCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Candidates both data-ready and encode-ready.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{IsReadyToEncode}}, TRUE(), CandidateStudyCatalog!{{IsDataReady}}, TRUE())"},
        {"name": "ExpansionCandidateCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Loop-67 expansion-wave-1 candidates.", "formula": "=COUNTIFS(CandidateStudyCatalog!{{ExpansionWave}}, \"expansion-wave-1\")"},
    ]
    snames = {f["name"] for f in summary_schema}
    for f in extra_summary:
        if f["name"] not in snames:
            summary_schema.append(f)

    for f in summary_schema:
        if f["name"] == "CatalogWitnessNote":
            f["formula"] = "=IF({{ImportSessionReady}}, CONCAT(\"READY: \", {{ReadyToEncodeCount}}, \" encode-ready (\", {{DataReadyCount}}, \" data-ready) of \", {{CandidateCount}}, \" candidates\"), CONCAT(\"BUILDING: \", {{CandidateCount}}, \" candidates — \", {{DataReadyCount}}, \" data-ready, \", {{ReadyToEncodeCount}}, \" encode-ready\"))"

    det_schema = rb["DomainExpansionTargets"]["schema"]
    if not any(f["name"] == "LegacyDomainAlias" for f in det_schema):
        det_schema.insert(3, {"name": "LegacyDomainAlias", "datatype": "string", "type": "raw", "nullable": True, "Description": "Studies.Domain slug for legacy imported corpus rows."})
    for f in det_schema:
        if f["name"] == "CurrentImportedCount":
            f["Description"] = "Real studies in corpus matching LegacyDomainAlias (or Domain when no alias)."
            f["formula"] = "=COUNTIFS(Studies!{{Domain}}, IF({{LegacyDomainAlias}}<>\"\", {{LegacyDomainAlias}}, {{Domain}}), Studies!{{IsSynthetic}}, FALSE())"

    rb["DomainExpansionTargets"]["data"] = [
        {"DomainTargetId": f"domain-target-{slug}", "Domain": slug, "LegacyDomainAlias": LEGACY_DOMAIN_ALIAS.get(slug, slug), "TargetMinCount": n}
        for slug, n in DOMAIN_TARGETS.items()
    ]

    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-import-session-ready":
            inv["Name"] = "inv-expansion-catalog-ready"
            inv["AlgebraicStatement"] = "ImportSessionReady = TRUE when CandidateCount >= 20 and ReadyToEncodeCount >= 15"
            inv["NaturalLanguage"] = "Loop-67+: expansion backlog loaded; bulk encode session ready when >=20 candidates and >=15 encode-ready."
            inv["AssertionExpression"] = "ImportSessionReady = TRUE"
            inv["SqlAssertion"] = "import_session_ready = TRUE"
            inv["Severity"] = "warning"

    for loop in rb["Loops"]["data"]:
        if loop["LoopId"] == "loop-67":
            loop["Status"] = "complete"
            loop["MockDataNote"] = (
                f"Witnessed: parsed {len(plan_studies)} plan entries; added {added} expansion candidates "
                f"(skipped {skipped} duplicates); CorpusDomains=11; CandidateStudyCatalog={len(existing_catalog)} total; "
                f"expansion-wave-1 candidates={added}. No CaseCells encoded — catalog + pipeline schema only."
            )
            loop["CommitDate"] = "2026-07-06"

    RULEBOOK.write_text(compact_json(rb), encoding="utf-8")
    print(f"plan_entries={len(plan_studies)} added={added} skipped={skipped} catalog_total={len(existing_catalog)}")


if __name__ == "__main__":
    main()
