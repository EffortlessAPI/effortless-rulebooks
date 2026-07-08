#!/usr/bin/env python3
"""
inject-loop-88.py
Injects all staged-study.json files from data/loop-88-candidates/ into the rulebook.
Adds:
  - Studies.IsControlStudy (boolean field)
  - Studies.ControlDomain (string field)
  - CorpusBalance singleton table
  - 3 DiscoveryHypotheses rows
  - 33 Study rows + their Treatments, Strata, CaseCells
"""

import json, os, re, glob, sys

RULEBOOK_PATH = "effortless-rulebook/simpsons-paradox-rulebook.json"
CANDIDATES_DIR = "data/loop-88-candidates"


# ── helpers ──────────────────────────────────────────────────────────────────

def compact_data_arrays(text):
    """Re-compact every data array so each object sits on one line."""
    def compact_array(m):
        try:
            arr = json.loads(m.group(0))
            lines = [json.dumps(row, ensure_ascii=False) for row in arr]
            return "[\n    " + ",\n    ".join(lines) + "\n  ]"
        except Exception:
            return m.group(0)
    return re.sub(r'\[\s*\{.*?\}\s*\]', compact_array, text, flags=re.DOTALL)


def load_rb():
    return json.loads(open(RULEBOOK_PATH).read())


def save_rb(rb):
    pretty = json.dumps(rb, indent=2, ensure_ascii=False)
    compacted = compact_data_arrays(pretty)
    open(RULEBOOK_PATH, "w").write(compacted)


def load_staged_studies():
    studies = []
    for path in sorted(glob.glob(f"{CANDIDATES_DIR}/*/staged-study.json")):
        try:
            s = json.load(open(path))
            s["_source_path"] = path
            studies.append(s)
        except Exception as e:
            print(f"  WARN: could not load {path}: {e}", file=sys.stderr)
    return studies


# ── step 1: add schema fields to Studies ─────────────────────────────────────

def add_studies_schema_fields(rb):
    schema = rb["Studies"]["schema"]
    names = {f["name"] for f in schema}

    if "IsControlStudy" not in names:
        schema.append({
            "name": "IsControlStudy",
            "datatype": "boolean",
            "type": "raw",
            "nullable": False,
            "Description": "TRUE if this study is a control (no Simpson's paradox expected/found); FALSE for the main Simpson's Paradox corpus."
        })
        print("  Added Studies.IsControlStudy")

    if "ControlDomain" not in names:
        schema.append({
            "name": "ControlDomain",
            "datatype": "string",
            "type": "raw",
            "nullable": True,
            "Description": "Domain slug for control studies (e.g. medicine-rct, education-higher). NULL for non-control studies."
        })
        print("  Added Studies.ControlDomain")

    # Backfill existing studies with IsControlStudy=false, ControlDomain=null
    for row in rb["Studies"]["data"]:
        if "IsControlStudy" not in row:
            row["IsControlStudy"] = False
        if "ControlDomain" not in row:
            row["ControlDomain"] = None


# ── step 2: add CorpusBalance singleton table ─────────────────────────────────

def add_corpus_balance_table(rb):
    if "CorpusBalance" in rb:
        print("  CorpusBalance table already exists — skipping")
        return

    rb["CorpusBalance"] = {
        "schema": [
            {"name": "BalanceId",         "datatype": "string",  "type": "raw",        "nullable": False, "Description": "Primary key (singleton: 'corpus-balance')"},
            {"name": "Name",              "datatype": "string",  "type": "calculated",  "nullable": False, "Description": "=BalanceId", "Formula": "=BalanceId"},
            {"name": "TotalStudyCount",   "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Total studies in corpus (Simpson + control)", "Formula": "=COUNTA(Studies[StudyId])"},
            {"name": "SimpsonStudyCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Studies where IsControlStudy=FALSE", "Formula": "=COUNTIF(Studies[IsControlStudy],FALSE)"},
            {"name": "ControlStudyCount", "datatype": "integer", "type": "aggregation", "nullable": False, "Description": "Studies where IsControlStudy=TRUE", "Formula": "=COUNTIF(Studies[IsControlStudy],TRUE)"},
            {"name": "ControlFraction",   "datatype": "float",   "type": "calculated",  "nullable": True,  "Description": "ControlStudyCount / TotalStudyCount", "Formula": "=IF(TotalStudyCount>0,ControlStudyCount/TotalStudyCount,NULL)"}
        ],
        "data": [
            {"BalanceId": "corpus-balance"}
        ]
    }
    print("  Added CorpusBalance table")


# ── step 3: add 3 discovery hypotheses ───────────────────────────────────────

NEW_HYPOTHESES = [
    {
        "HypothesisId": "H-control-type-d-predominance",
        "Title": "Control Type-D Predominance",
        "Predicate": "COUNTIF(TreatmentRankings[IsSignFlip],TRUE,Studies[IsControlStudy],TRUE)/COUNTIF(Studies[IsControlStudy],TRUE)<0.3",
        "Description": "≥70% of control studies encode as Type-D (neutral allocation, no sign flip). Type-D is the expected basin for studies selected for absence of paradox.",
        "IsRegistered": True,
        "IsCritical": False,
        "LoopId": "loop-88"
    },
    {
        "HypothesisId": "H-control-no-manifest-flip",
        "Title": "Control Studies Have No Manifest Sign Flip",
        "Predicate": "COUNTIF(TreatmentRankings[IsSignFlip],TRUE,Studies[IsControlStudy],TRUE)=0",
        "Description": "Zero manifest sign flips among control studies. These studies were selected for absence of paradox, so no pooled-vs-stratified reversal should be present.",
        "IsRegistered": True,
        "IsCritical": False,
        "LoopId": "loop-88"
    },
    {
        "HypothesisId": "H-control-safety-corridor",
        "Title": "Control Safety Corridor: No Flip at Low Imbalance",
        "Predicate": "COUNTIF(TreatmentRankings[IsSignFlip],TRUE,TreatmentRankings[MaxStratumImbalance],\"<0.033\",Studies[IsControlStudy],TRUE)=0",
        "Description": "No control study with MaxStratumImbalance < 0.033 (the safe-corridor threshold) has IsSignFlip=TRUE. The safety corridor hypothesis predicts zero flips below this boundary.",
        "IsRegistered": True,
        "IsCritical": False,
        "LoopId": "loop-88"
    }
]

def add_discovery_hypotheses(rb):
    existing_ids = {h["HypothesisId"] for h in rb.get("DiscoveryHypotheses", {}).get("data", [])}
    added = 0
    for h in NEW_HYPOTHESES:
        if h["HypothesisId"] not in existing_ids:
            rb["DiscoveryHypotheses"]["data"].append(h)
            added += 1
    print(f"  Added {added} DiscoveryHypotheses rows")


# ── step 4: inject studies ────────────────────────────────────────────────────

def inject_studies(rb, staged_studies):
    existing_study_ids   = {r["StudyId"]     for r in rb["Studies"]["data"]}
    existing_treatment_ids = {r["TreatmentId"] for r in rb["Treatments"]["data"]}
    existing_stratum_ids   = {r["StratumId"]   for r in rb["Strata"]["data"]}
    existing_cell_ids      = {r["CaseCellId"]  for r in rb["CaseCells"]["data"]}

    studies_added = treatments_added = strata_added = cells_added = 0
    skipped = []

    for s in staged_studies:
        sid = s["StudyId"]
        if sid in existing_study_ids:
            skipped.append(sid)
            continue

        # — Study row —
        rb["Studies"]["data"].append({
            "StudyId":          sid,
            "Title":            s["Title"],
            "Source":           s["Source"],
            "SourceUrl":        s["SourceUrl"],
            "PublicationYear":  s.get("PublicationYear"),
            "Domain":           s.get("Domain", ""),
            "IsSynthetic":      False,
            "IsControlStudy":   True,
            "ControlDomain":    s.get("ControlDomain"),
            "TraditionId":      s.get("TraditionId", "tradition-dag"),
            "PrimaryResearcherId": s.get("PrimaryResearcherId", "researcher-pearl"),
        })
        existing_study_ids.add(sid)
        studies_added += 1

        # — Treatment rows —
        for t in s.get("treatments", []):
            tid = f"{sid}-{t['TreatmentLabel']}"
            if tid in existing_treatment_ids:
                continue
            rb["Treatments"]["data"].append({
                "TreatmentId":    tid,
                "Study":          sid,
                "TreatmentLabel": t["TreatmentLabel"],
                "Description":    t.get("Description", ""),
                "TotalCases":     t.get("TotalCases", 0),
                "TotalSuccesses": t.get("TotalSuccesses", 0),
            })
            existing_treatment_ids.add(tid)
            treatments_added += 1

        # — Stratum rows —
        for st in s.get("strata", []):
            stid = f"{sid}-{st['StratumLabel']}"
            if stid in existing_stratum_ids:
                continue
            rb["Strata"]["data"].append({
                "StratumId":    stid,
                "Study":        sid,
                "StratumLabel": st["StratumLabel"],
                "Description":  st.get("Description", ""),
                "TotalCases":   st.get("TotalCases", 0),
            })
            existing_stratum_ids.add(stid)
            strata_added += 1

        # — CaseCell rows —
        for cell in s.get("cells", []):
            cid = f"{sid}-{cell['StratumLabel']}-{cell['TreatmentLabel']}"
            if cid in existing_cell_ids:
                continue
            cases = cell.get("Cases", 0)
            succs = cell.get("Successes", 0)
            rb["CaseCells"]["data"].append({
                "CaseCellId":    cid,
                "Study":         sid,
                "StratumLabel":  cell["StratumLabel"],
                "TreatmentLabel": cell["TreatmentLabel"],
                "Successes":     succs,
                "Cases":         cases,
            })
            existing_cell_ids.add(cid)
            cells_added += 1

    print(f"  Injected: {studies_added} studies, {treatments_added} treatments, {strata_added} strata, {cells_added} cells")
    if skipped:
        print(f"  Skipped (already present): {skipped}")


# ── step 5: mark loop-88 complete ────────────────────────────────────────────

def mark_loop_complete(rb):
    for row in rb.get("Loops", {}).get("data", []):
        if row.get("LoopId") == "loop-88":
            row["Status"] = "complete"
            row["WhatWasIntroduced"] = (
                "Added IsControlStudy + ControlDomain fields to Studies. "
                "Added CorpusBalance singleton table. "
                "Added 3 control-corpus hypotheses. "
                "Injected 33 control studies (from 153 candidates: 33 viable, 92 skip, 23 portal-required, 1 duplicate, 1 download-failed). "
                "Control domains: medicine-rct, medicine-epidemiology, criminal-justice, education-higher, education-k12, economics-labor, sports, historical, public-health, transportation, sociology."
            )
            print("  Marked loop-88 complete")
            return
    print("  WARN: loop-88 not found in Loops table", file=sys.stderr)


# ── main ──────────────────────────────────────────────────────────────────────

def main():
    print("Loading rulebook…")
    rb = load_rb()

    print("\nStep 1: Studies schema fields")
    add_studies_schema_fields(rb)

    print("\nStep 2: CorpusBalance table")
    add_corpus_balance_table(rb)

    print("\nStep 3: DiscoveryHypotheses")
    add_discovery_hypotheses(rb)

    print("\nStep 4: Loading staged studies…")
    staged = load_staged_studies()
    print(f"  Found {len(staged)} staged-study.json files")
    inject_studies(rb, staged)

    print("\nStep 5: Loop status")
    mark_loop_complete(rb)

    print("\nSaving rulebook…")
    save_rb(rb)
    print("Done.")


if __name__ == "__main__":
    os.chdir(os.path.dirname(os.path.abspath(__file__)) + "/..")
    main()
