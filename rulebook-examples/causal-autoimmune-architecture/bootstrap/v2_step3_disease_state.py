#!/usr/bin/env python3
"""
v2 Step 3 — the disease-state simulator (implements bootstrap/v2-disease-state-spec.md).

Adds to the rulebook hub:
  * SerologyObservations  — RAW longitudinal serology leaves + derived trend/SLEDAI calcs
  * TherapyOptions        — small therapy lookup (MMF / belimumab / anifrolumab)
  * CausalMechanisms.TargetPathway        (NEW RAW LEAF: which druggable pathway)
  * Individuals.NephritisProgressionStateKey (DERIVED current disease state, from labs)
  * Individuals dwell rollups + latest-serology lookups
  * IndividualPredictions.RecommendedTreatmentLine + TreatmentLineDecidingFactor (DERIVED)
  * lupus-nephritis-progression state machine (StateMachines/MachineStates/Rules) + per-patient
    SubjectStateInstances occupancy (bitemporal dwell) + StateTransitions log
  * seeds serology series for the 7 + 2 new patients; Diego = the disagreement counter-example

Trust boundary: every NEW raw field is a lab observation (the LLM/intake layer). Every
disease state, score, trend, and recommendation is a FORMULA. Same line in the DAG as v1.

Idempotent: re-running replaces v2 tables/fields/rows rather than duplicating.
NOTE: dwell-time is stored as a transparent raw `DwellDays` on occupancy rows (the transpiler
has no NOW()/date-diff formula support here), plus a derived long-dwell flag — keeping it
witnessed + editable without an unsupported date formula.
"""
import json, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RB_PATH = os.path.join(ROOT, "effortless-rulebook", "effortless-rulebook.json")

AUDIT = {"CreatedBy": "v2-step3", "CreatedAt": "2026-06-20T00:00:00Z",
         "ModifiedBy": "v2-step3", "ModifiedAt": "2026-06-20T00:00:00Z",
         "ModifiedByModel": "claude-opus-4-8"}


# ---------------------------------------------------------------------------
# Per-patient serology design (fake but transparent). Each row's raw numbers are
# chosen so the DERIVED NephritisProgressionStateKey lands on the intended state.
# (ind, seq, observed_at, dsDNA, C3, C4, proteinuria g/d, eGFR, activeSediment)
# Intended target state per patient is documented inline.
# ---------------------------------------------------------------------------
SEROLOGY = [
    # Ana Reyes (A) — actionable; serologically active but NOT nephritic. -> SerologicActive
    ("ind-a-reyes", 1, "2025-09-01", 40,  110, 22, 0.1, 95, False),
    ("ind-a-reyes", 2, "2025-12-01", 95,  78,  12, 0.2, 94, False),   # dsDNA rising, complement falling
    # Bili Okafor (B) — calibration-fail; mild serology, stable. -> PresymptomaticAutoimmunity
    ("ind-b-okafor", 1, "2025-08-15", 30, 120, 25, 0.05, 99, False),
    ("ind-b-okafor", 2, "2025-11-15", 33, 118, 24, 0.08, 98, False),  # stable
    # Chen Wei (C) — spurious mechanism; serologically active. -> SerologicActive
    ("ind-c-chen", 1, "2025-07-01", 50, 100, 20, 0.2, 92, False),
    ("ind-c-chen", 2, "2025-10-01", 120, 70, 10, 0.3, 90, False),     # rising/falling
    # Diego Santos (D) — THE DISAGREEMENT CASE: progresses to BiopsyIndicated, gate=cryptic FALSE.
    ("ind-d-santos", 1, "2025-06-01", 60, 95, 18, 0.4, 88, False),
    ("ind-d-santos", 2, "2025-09-01", 140, 60, 9, 1.2, 80, False),    # rising/falling + proteinuria up
    ("ind-d-santos", 3, "2025-12-01", 180, 45, 6, 3.6, 68, True),     # nephrotic-range + active sediment -> BiopsyIndicated
    # Esi Mensah (E) — falsifiability-fail; low activity. -> PresymptomaticAutoimmunity
    ("ind-e-mensah", 1, "2025-08-01", 25, 125, 26, 0.05, 100, False),
    ("ind-e-mensah", 2, "2025-11-01", 24, 124, 27, 0.04, 101, False),
    # Faisal Haidar (F) — ancestry-transport fail (PsA); minimal renal. -> PresymptomaticAutoimmunity
    ("ind-f-haidar", 1, "2025-09-10", 20, 130, 28, 0.0, 100, False),
    ("ind-f-haidar", 2, "2025-12-10", 22, 128, 27, 0.0, 99, False),
    # Grace Lin (G) — actionable (PsA); minimal renal. -> PresymptomaticAutoimmunity
    ("ind-g-lin", 1, "2025-09-05", 18, 132, 29, 0.0, 100, False),
    ("ind-g-lin", 2, "2025-12-05", 19, 131, 28, 0.0, 100, False),
    # NEW patient H "Hana Yamamoto" — clean EARLY-NEPHRITIS demo, actionable-style serology.
    ("ind-h-yamamoto", 1, "2025-07-20", 55, 98, 19, 0.3, 90, False),
    ("ind-h-yamamoto", 2, "2025-10-20", 110, 72, 11, 0.7, 85, False), # significant proteinuria -> EarlyNephritis
    # NEW patient I "Ibrahim Conteh" — RENAL-FLARE-RISK demo.
    ("ind-i-conteh", 1, "2025-06-15", 70, 90, 16, 0.5, 84, False),
    ("ind-i-conteh", 2, "2025-09-15", 150, 55, 8, 1.5, 76, False),    # proteinuria 1.5 -> RenalFlareRisk
]

# Dwell days per (ind, state) occupancy — transparent raw values (months in state).
# Built from the serology cadence above; the LATEST state is "current" (ExitedAt null).

# New individuals to add (H, I) — minimal rows; they carry the progression demo only.
NEW_INDIVIDUALS = [
    {"IndividualId": "ind-h-yamamoto", "GivenName": "Hana", "FamilyName": "Yamamoto",
     "AncestryLabel": "East Asian", "AgeYears": 33, "IsAncestryAbsentFromTraining": False,
     "FederatedDataset": None, "EnrollmentDate": "2025-07-01", "HasCrypticRelatednessFlag": False,
     "CaseNarrative": "33yo East Asian woman; rising anti-dsDNA with falling complement and new "
                      "significant proteinuria (0.7 g/day) over one quarter — early lupus nephritis. "
                      "Progression-demo case (no v1 keystone mechanism attached)."},
    {"IndividualId": "ind-i-conteh", "GivenName": "Ibrahim", "FamilyName": "Conteh",
     "AncestryLabel": "African", "AgeYears": 45, "IsAncestryAbsentFromTraining": False,
     "FederatedDataset": None, "EnrollmentDate": "2025-06-01", "HasCrypticRelatednessFlag": False,
     "CaseNarrative": "45yo African man; brisk serologic activity and proteinuria climbing to "
                      "1.5 g/day — at risk of renal flare. Progression-demo case (no v1 keystone "
                      "mechanism attached)."},
]

# Target pathway per patient mechanism (NEW raw leaf on CausalMechanisms).
# Chosen so the treatment-line example differentiates: IFN -> anifrolumab, B-cell -> belimumab.
TARGET_PATHWAY = {
    "cm-a": "type-I-IFN", "cm-b": "type-I-IFN", "cm-c": "B-cell/autoantibody",
    "cm-d": "B-cell/autoantibody", "cm-e": "T-cell-costim", "cm-f": "IL-17/23",
    "cm-g": "IL-17/23",
}


def fmt(*parts):
    return "".join(parts)


def add_serology(rb):
    schema = [
        {"name": "SerologyObservationId", "datatype": "string", "type": "raw", "nullable": False,
         "Description": "Slug PK: sero-{ind}-{seq}."},
        {"name": "Individual", "datatype": "string", "type": "relationship", "nullable": False,
         "Description": "FK -> Individuals.IndividualId.", "RelatedTo": "Individuals",
         "prefersSingleRecordLink": True, "isReversed": False, "InverseField": ""},
        {"name": "ObservedAt", "datatype": "datetime", "type": "raw", "nullable": False,
         "Description": "Valid-time of this serology panel (RAW leaf)."},
        {"name": "SequenceIndex", "datatype": "number", "type": "raw", "nullable": False,
         "Description": "1-based order within the individual's panel series (RAW)."},
        {"name": "AntiDsDnaIU", "datatype": "number", "type": "raw", "nullable": True,
         "Description": "anti-dsDNA titre IU/mL (RAW lab)."},
        {"name": "ComplementC3", "datatype": "number", "type": "raw", "nullable": True,
         "Description": "C3 mg/dL (RAW lab)."},
        {"name": "ComplementC4", "datatype": "number", "type": "raw", "nullable": True,
         "Description": "C4 mg/dL (RAW lab)."},
        {"name": "ProteinuriaGPerDay", "datatype": "number", "type": "raw", "nullable": True,
         "Description": "urine protein g/day (RAW lab)."},
        {"name": "EgfrMlMin", "datatype": "number", "type": "raw", "nullable": True,
         "Description": "eGFR mL/min (RAW lab)."},
        {"name": "HasActiveUrinarySediment", "datatype": "boolean", "type": "raw", "nullable": True,
         "Description": "active casts/hematuria (RAW observation)."},
        {"name": "PriorObservation", "datatype": "string", "type": "relationship", "nullable": True,
         "Description": "Self-FK -> the prior panel in this individual's series (seq-1). NULL for the first.",
         "RelatedTo": "SerologyObservations", "prefersSingleRecordLink": True, "isReversed": False,
         "InverseField": ""},
        {"name": "PriorAntiDsDnaIU", "datatype": "number", "type": "lookup", "nullable": True,
         "Description": "Prior panel's dsDNA via PriorObservation FK, for trend.",
         "RelatedTo": "SerologyObservations", "LookupField": "AntiDsDnaIU", "ViaField": "PriorObservation",
         "formula": '=INDEX(SerologyObservations!{{AntiDsDnaIU}}, MATCH({{PriorObservation}}, SerologyObservations!{{SerologyObservationId}}, 0))'},
        {"name": "PriorC3", "datatype": "number", "type": "lookup", "nullable": True,
         "Description": "Prior C3 via PriorObservation FK.", "RelatedTo": "SerologyObservations",
         "LookupField": "ComplementC3", "ViaField": "PriorObservation",
         "formula": '=INDEX(SerologyObservations!{{ComplementC3}}, MATCH({{PriorObservation}}, SerologyObservations!{{SerologyObservationId}}, 0))'},
        {"name": "PriorC4", "datatype": "number", "type": "lookup", "nullable": True,
         "Description": "Prior C4 via PriorObservation FK.", "RelatedTo": "SerologyObservations",
         "LookupField": "ComplementC4", "ViaField": "PriorObservation",
         "formula": '=INDEX(SerologyObservations!{{ComplementC4}}, MATCH({{PriorObservation}}, SerologyObservations!{{SerologyObservationId}}, 0))'},
        {"name": "AntiDsDnaTrend", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Rising/Falling/Stable vs prior panel (derived from raw).",
         "formula": '=IF(ISBLANK({{PriorAntiDsDnaIU}}),"Stable",IF({{AntiDsDnaIU}}>{{PriorAntiDsDnaIU}}*1.25,"Rising",IF({{AntiDsDnaIU}}<{{PriorAntiDsDnaIU}}*0.8,"Falling","Stable")))'},
        {"name": "ComplementTrend", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Rising/Falling/Stable on C3+C4 vs prior (derived).",
         "formula": '=IF(ISBLANK({{PriorC3}}),"Stable",IF(({{ComplementC3}}+{{ComplementC4}})<({{PriorC3}}+{{PriorC4}})*0.85,"Falling",IF(({{ComplementC3}}+{{ComplementC4}})>({{PriorC3}}+{{PriorC4}})*1.15,"Rising","Stable")))'},
        {"name": "IsSignificantProteinuria", "datatype": "boolean", "type": "calculated", "nullable": True,
         "Description": "proteinuria >= 0.5 g/day.",
         "formula": '=IF({{ProteinuriaGPerDay}}>=0.5,TRUE(),FALSE())'},
        {"name": "IsNephroticRangeProteinuria", "datatype": "boolean", "type": "calculated", "nullable": True,
         "Description": "proteinuria >= 3.0 g/day.",
         "formula": '=IF({{ProteinuriaGPerDay}}>=3.0,TRUE(),FALSE())'},
        {"name": "SledaiRenalPoints", "datatype": "number", "type": "calculated", "nullable": True,
         "Description": "SLEDAI-style renal sub-score (0/4/8) from proteinuria + sediment.",
         "formula": '=IF(OR({{IsNephroticRangeProteinuria}},{{HasActiveUrinarySediment}}),8,IF({{IsSignificantProteinuria}},4,0))'},
        {"name": "SledaiSerologyPoints", "datatype": "number", "type": "calculated", "nullable": True,
         "Description": "SLEDAI-style serology sub-score (0/2/4) from low-complement + raised dsDNA.",
         "formula": '=IF(AND({{ComplementTrend}}="Falling",{{AntiDsDnaTrend}}="Rising"),4,IF(OR({{ComplementTrend}}="Falling",{{AntiDsDnaTrend}}="Rising"),2,0))'},
        {"name": "SledaiScore", "datatype": "number", "type": "calculated", "nullable": True,
         "Description": "Derived SLEDAI-style activity score = renal + serology points.",
         "formula": '={{SledaiRenalPoints}}+{{SledaiSerologyPoints}}'},
        {"name": "ProgressionStateKey", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Disease state IMPLIED by THIS panel (derived purely from raw leaves).",
         "formula": '=IF(OR({{IsNephroticRangeProteinuria}},{{HasActiveUrinarySediment}}),"BiopsyIndicated",IF({{ProteinuriaGPerDay}}>=1.0,"RenalFlareRisk",IF({{IsSignificantProteinuria}},"EarlyNephritis",IF(AND({{AntiDsDnaTrend}}="Rising",{{ComplementTrend}}="Falling"),"SerologicActive","PresymptomaticAutoimmunity"))))'},
        {"name": "ProgressionStateOrder", "datatype": "number", "type": "calculated", "nullable": True,
         "Description": "Numeric severity order of THIS panel's implied state (0..5). Lets the "
                        "individual's current state be a MAXIFS over panels (highest state reached).",
         "formula": '=IF({{ProgressionStateKey}}="BiopsyIndicated",5,IF({{ProgressionStateKey}}="RenalFlareRisk",4,IF({{ProgressionStateKey}}="EarlyNephritis",3,IF({{ProgressionStateKey}}="SerologicActive",2,1))))'},
        {"name": "Name", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Echoes id.", "formula": "={{SerologyObservationId}}"},
        {"name": "RelativePath", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Path.", "formula": '="/admin/serology/" & {{SerologyObservationId}}'},
    ]
    data = []
    for ind, seq, at, dsdna, c3, c4, prot, egfr, sed in SEROLOGY:
        sid = f"sero-{ind}-{seq}"
        prior = f"sero-{ind}-{seq-1}" if seq > 1 else None
        data.append({
            "SerologyObservationId": sid, "Individual": ind, "ObservedAt": at + "T00:00:00Z",
            "SequenceIndex": seq, "AntiDsDnaIU": dsdna, "ComplementC3": c3, "ComplementC4": c4,
            "ProteinuriaGPerDay": prot, "EgfrMlMin": egfr, "HasActiveUrinarySediment": sed,
            "PriorObservation": prior, "Name": sid,
            "RelativePath": f"/admin/serology/{sid}", **AUDIT,
        })
    rb["SerologyObservations"] = {
        "Description": "RAW longitudinal serology panels (the lab/LLM layer) + derived trend, "
                       "SLEDAI sub-scores, and the disease state IMPLIED by each panel. The leaves "
                       "that drive the lupus-nephritis progression machine.",
        "schema": schema, "data": data,
    }


def add_therapy_options(rb):
    schema = [
        {"name": "TherapyOptionId", "datatype": "string", "type": "raw", "nullable": False,
         "Description": "Slug PK."},
        {"name": "TherapyLabel", "datatype": "string", "type": "raw", "nullable": False,
         "Description": "Therapy name."},
        {"name": "TargetsPathway", "datatype": "string", "type": "raw", "nullable": True,
         "Description": "Pathway this therapy targets (matches CausalMechanisms.TargetPathway)."},
        {"name": "LineOrdinal", "datatype": "number", "type": "raw", "nullable": True,
         "Description": "Treatment-line ordinal (1=first-line/induction)."},
        {"name": "PreferredWhen", "datatype": "string", "type": "raw", "nullable": True,
         "Description": "Clinical context where preferred."},
        {"name": "Name", "datatype": "string", "type": "calculated", "nullable": True,
         "formula": "={{TherapyLabel}}", "Description": "Echo."},
        {"name": "RelativePath", "datatype": "string", "type": "calculated", "nullable": True,
         "formula": '="/admin/therapy-options/" & {{TherapyOptionId}}', "Description": "Path."},
    ]
    rows = [
        ("mmf", "Mycophenolate (MMF)", "organ-induction", 1, "active nephritis (proteinuria-driven)"),
        ("belimumab", "Belimumab (anti-BLyS)", "B-cell/autoantibody", 2, "serologically active, autoantibody-driven"),
        ("anifrolumab", "Anifrolumab (anti-IFNAR1)", "type-I-IFN", 2, "type-I-IFN-signature mechanism"),
        ("secukinumab", "Secukinumab (anti-IL-17)", "IL-17/23", 2, "IL-17/23-axis disease (PsA/SpA)"),
    ]
    data = [{"TherapyOptionId": i, "TherapyLabel": l, "TargetsPathway": p, "LineOrdinal": o,
             "PreferredWhen": w, "Name": l, "RelativePath": f"/admin/therapy-options/{i}", **AUDIT}
            for (i, l, p, o, w) in rows]
    rb["TherapyOptions"] = {
        "Description": "Therapy lookup for the treatment-line-selection DAG (MMF/belimumab/anifrolumab/secukinumab).",
        "schema": schema, "data": data,
    }


def add_mechanism_pathway(rb):
    cm = rb["CausalMechanisms"]
    names = {f["name"] for f in cm["schema"]}
    if "TargetPathway" not in names:
        # insert after MechanismType
        idx = next((i for i, f in enumerate(cm["schema"]) if f["name"] == "MechanismType"), 0) + 1
        cm["schema"].insert(idx, {
            "name": "TargetPathway", "datatype": "string", "type": "raw", "nullable": True,
            "Description": "NEW RAW LEAF: druggable pathway this confirmed mechanism implicates "
                           "(type-I-IFN / B-cell/autoantibody / T-cell-costim / IL-17/23). Drives "
                           "treatment-line selection. An observation about the mechanism, not derived."})
    if "TargetPathwayCode" not in names:
        idx = next((i for i, f in enumerate(cm["schema"]) if f["name"] == "TargetPathway"), 0) + 1
        cm["schema"].insert(idx, {
            "name": "TargetPathwayCode", "datatype": "number", "type": "calculated", "nullable": True,
            "Description": "Numeric encoding of TargetPathway so the individual can resolve it via a "
                           "MAXIFS aggregation (transpiler-friendly). 1=type-I-IFN 2=B-cell 3=T-cell 4=IL-17/23.",
            "formula": '=IF({{TargetPathway}}="type-I-IFN",1,IF({{TargetPathway}}="B-cell/autoantibody",2,IF({{TargetPathway}}="T-cell-costim",3,IF({{TargetPathway}}="IL-17/23",4,0))))'})
    for r in cm["data"]:
        r["TargetPathway"] = TARGET_PATHWAY.get(r["CausalMechanismId"], "")


def add_individual_fields(rb):
    ind = rb["Individuals"]
    names = {f["name"] for f in ind["schema"]}
    add = []
    if "CountSerologyPanels" not in names:
        add.append({"name": "CountSerologyPanels", "datatype": "number", "type": "aggregation",
                    "nullable": True, "Description": "Number of serology panels for this individual.",
                    "formula": "=COUNTIFS(SerologyObservations!{{Individual}}, {{IndividualId}})"})
    # Current state = the highest progression-state ORDER reached across this individual's panels
    # (MAXIFS — transpiler-friendly, avoids fragile computed-key lookups). For the seeded monotonic
    # paths, max-order == latest panel's state.
    if "MaxProgressionStateOrder" not in names:
        add.append({"name": "MaxProgressionStateOrder", "datatype": "number", "type": "aggregation",
                    "nullable": True,
                    "Description": "Highest progression-state order reached across this individual's "
                                   "serology panels (worst/current state).",
                    "formula": "=MAXIFS(SerologyObservations!{{ProgressionStateOrder}}, SerologyObservations!{{Individual}}, {{IndividualId}})"})
    if "LatestSledaiScore" not in names:
        add.append({"name": "LatestSledaiScore", "datatype": "number", "type": "aggregation",
                    "nullable": True,
                    "Description": "Peak SLEDAI activity score across this individual's panels (derived).",
                    "formula": "=MAXIFS(SerologyObservations!{{SledaiScore}}, SerologyObservations!{{Individual}}, {{IndividualId}})"})
    if "NephritisProgressionStateKey" not in names:
        add.append({"name": "NephritisProgressionStateKey", "datatype": "string", "type": "calculated",
                    "nullable": True,
                    "Description": "DERIVED current disease state for the lupus-nephritis-progression "
                                   "machine: decoded from the highest state order reached. The subject- "
                                   "state column of the state machine; never hand-set.",
                    "formula": '=IF({{MaxProgressionStateOrder}}>=5,"BiopsyIndicated",IF({{MaxProgressionStateOrder}}>=4,"RenalFlareRisk",IF({{MaxProgressionStateOrder}}>=3,"EarlyNephritis",IF({{MaxProgressionStateOrder}}>=2,"SerologicActive","PresymptomaticAutoimmunity"))))'})
    if "ActivityTier" not in names:
        add.append({"name": "ActivityTier", "datatype": "string", "type": "calculated", "nullable": True,
                    "Description": "Derived disease-activity tier from peak SLEDAI score.",
                    "formula": '=IF({{LatestSledaiScore}}>=12,"High / flare",IF({{LatestSledaiScore}}>=6,"Moderate",IF({{LatestSledaiScore}}>=1,"Mild","Quiescent")))'})
    if "IsHighDiseaseActivity" not in names:
        add.append({"name": "IsHighDiseaseActivity", "datatype": "boolean", "type": "calculated",
                    "nullable": True, "Description": "TRUE when peak SLEDAI >= 12.",
                    "formula": '=IF({{LatestSledaiScore}}>=12,TRUE(),FALSE())'})
    if "IsDiseaseProgressing" not in names:
        add.append({"name": "IsDiseaseProgressing", "datatype": "boolean", "type": "calculated",
                    "nullable": True,
                    "Description": "TRUE when the disease-state simulator places the patient in an "
                                   "active/worsening renal state (independent of the actionability gate).",
                    "formula": '=IF(OR({{NephritisProgressionStateKey}}="EarlyNephritis",{{NephritisProgressionStateKey}}="RenalFlareRisk",{{NephritisProgressionStateKey}}="BiopsyIndicated"),TRUE(),FALSE())'})
    if "TargetPathwayCode" not in names:
        add.append({"name": "TargetPathwayCode", "datatype": "number", "type": "aggregation",
                    "nullable": True,
                    "Description": "Resolved pathway code from this individual's causal mechanism (MAXIFS).",
                    "formula": "=MAXIFS(CausalMechanisms!{{TargetPathwayCode}}, CausalMechanisms!{{Individual}}, {{IndividualId}})"})
    if "TargetPathway" not in names:
        add.append({"name": "TargetPathway", "datatype": "string", "type": "calculated", "nullable": True,
                    "Description": "Decoded druggable pathway implicated by this individual's mechanism.",
                    "formula": '=IF({{TargetPathwayCode}}=1,"type-I-IFN",IF({{TargetPathwayCode}}=2,"B-cell/autoantibody",IF({{TargetPathwayCode}}=3,"T-cell-costim",IF({{TargetPathwayCode}}=4,"IL-17/23",""))))'})
    # insert the aggregation/lookups in dependency order before the calc that uses them:
    # CountSerologyPanels first, then lookups, then calcs. Simplest: append all; transpiler resolves DAG.
    ind["schema"].extend(add)
    # add new individuals
    existing_ids = {r["IndividualId"] for r in ind["data"]}
    # build a template row from an existing one to satisfy all raw columns
    for ni in NEW_INDIVIDUALS:
        if ni["IndividualId"] in existing_ids:
            continue
        row = dict(ni)
        row["Name"] = f'{ni["GivenName"]} {ni["FamilyName"]}'
        row["Slug"] = ni["IndividualId"]
        row.update(AUDIT)
        ind["data"].append(row)


def add_prediction_treatment_line(rb):
    ip = rb["IndividualPredictions"]
    names = {f["name"] for f in ip["schema"]}
    add = []
    if "IndividualTargetPathway" not in names:
        add.append({"name": "IndividualTargetPathway", "datatype": "string", "type": "lookup",
                    "nullable": True, "RelatedTo": "Individuals", "LookupField": "TargetPathway",
                    "ViaField": "Individual",
                    "Description": "TargetPathway resolved on the individual (decoded from the mechanism).",
                    "formula": '=INDEX(Individuals!{{TargetPathway}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0))'})
    if "IndividualProgressionStateKey" not in names:
        add.append({"name": "IndividualProgressionStateKey", "datatype": "lookup", "type": "lookup",
                    "nullable": True, "RelatedTo": "Individuals",
                    "Description": "Lookup: the individual's derived disease state.",
                    "formula": '=INDEX(Individuals!{{NephritisProgressionStateKey}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0))'})
    if "IndividualIsDiseaseProgressing" not in names:
        add.append({"name": "IndividualIsDiseaseProgressing", "datatype": "boolean", "type": "lookup",
                    "nullable": True, "RelatedTo": "Individuals",
                    "Description": "Lookup: is the individual's disease progressing (simulator).",
                    "formula": '=INDEX(Individuals!{{IsDiseaseProgressing}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0))'})
    if "RecommendedTreatmentLine" not in names:
        add.append({"name": "RecommendedTreatmentLine", "datatype": "string", "type": "calculated",
                    "nullable": True,
                    "Description": "DERIVED treatment-line recommendation from confirmed-mechanism "
                                   "TargetPathway + disease state. The audit's MMF/belimumab/anifrolumab example.",
                    "formula": '=IF(NOT({{RestsOnConfirmedMechanism}}),"No targeted line — mechanism unconfirmed",IF(OR({{IndividualProgressionStateKey}}="RenalFlareRisk",{{IndividualProgressionStateKey}}="BiopsyIndicated"),"Mycophenolate (induction)",IF({{IndividualTargetPathway}}="type-I-IFN","Anifrolumab",IF({{IndividualTargetPathway}}="B-cell/autoantibody","Belimumab",IF({{IndividualTargetPathway}}="IL-17/23","Secukinumab","Standard-of-care (no mechanism-matched targeted line)")))))'})
    if "TreatmentLineDecidingFactor" not in names:
        add.append({"name": "TreatmentLineDecidingFactor", "datatype": "string", "type": "calculated",
                    "nullable": True,
                    "Description": "The single deciding reason for the recommended line (mirrors DecidingGate style).",
                    "formula": '=IF(NOT({{RestsOnConfirmedMechanism}}),"MechanismUnconfirmed",IF(OR({{IndividualProgressionStateKey}}="RenalFlareRisk",{{IndividualProgressionStateKey}}="BiopsyIndicated"),"ActiveNephritis-Induction",IF({{IndividualTargetPathway}}="type-I-IFN","IFNSignature-Anifrolumab",IF({{IndividualTargetPathway}}="B-cell/autoantibody","AutoantibodyDriven-Belimumab",IF({{IndividualTargetPathway}}="IL-17/23","IL17Axis-Secukinumab","NoMechanismMatch")))))'})
    if "ProgressionVsActionabilityDisagree" not in names:
        add.append({"name": "ProgressionVsActionabilityDisagree", "datatype": "boolean", "type": "calculated",
                    "nullable": True,
                    "Description": "THE COUNTER-EXAMPLE FLAG: TRUE when disease is progressing (simulator) "
                                   "but the prediction is NOT clinically actionable (gate). Proves the two "
                                   "layers are independent.",
                    "formula": '=IF(AND({{IndividualIsDiseaseProgressing}},NOT({{IsClinicallyActionable}})),TRUE(),FALSE())'})
    ip["schema"].extend(add)


def add_progression_machine(rb):
    SM = "lupus-nephritis-progression"
    # StateMachines row
    sm = rb["StateMachines"]
    sm["data"] = [r for r in sm["data"] if r["StateMachineId"] != SM]
    sm["data"].append({
        "StateMachineId": SM, "Name": SM, "RelativePath": f"/admin/state-machine/{SM}",
        "Title": "Lupus-Nephritis Progression",
        "Description": "Disease-state simulator: a patient's lupus-nephritis course derived from raw "
                       "serology trajectories. Presymptomatic -> SerologicActive -> EarlyNephritis -> "
                       "RenalFlareRisk -> BiopsyIndicated (+ Quiescent). Current state is the derived "
                       "Individuals.NephritisProgressionStateKey; never hand-set.",
        "SubjectTableName": "Individuals", "SubjectStateColumn": "NephritisProgressionStateKey",
        **AUDIT})
    # MachineStates
    states = [
        ("Quiescent", "Quiescent / in remission", 0, False, True),
        ("PresymptomaticAutoimmunity", "Presymptomatic autoimmunity (serology only)", 1, True, False),
        ("SerologicActive", "Serologically active (rising dsDNA / falling complement)", 2, False, False),
        ("EarlyNephritis", "Early nephritis (significant proteinuria)", 3, False, False),
        ("RenalFlareRisk", "At risk of renal flare", 4, False, False),
        ("BiopsyIndicated", "Biopsy indicated", 5, False, True),
    ]
    ms = rb["MachineStates"]
    ms["data"] = [r for r in ms["data"] if r.get("StateMachine") != SM]
    for key, title, order, init, term in states:
        sid = f"{SM}--{key.lower()}"
        ms["data"].append({
            "MachineStateId": sid, "Name": sid, "RelativePath": f"/admin/state-machine/states/{sid}",
            "StateMachine": SM, "StateKey": key, "Title": title, "OrderIndex": order,
            "IsInitial": init, "IsTerminal": term, **AUDIT})
    # StateTransitionRules
    rules = [
        ("PresymptomaticAutoimmunity", "SerologicActive", "anti-dsDNA Rising AND complement Falling", "SERO-ACTIVE"),
        ("SerologicActive", "EarlyNephritis", "proteinuria >= 0.5 g/day", "NEPH-ONSET"),
        ("EarlyNephritis", "RenalFlareRisk", "proteinuria >= 1.0 g/day", "FLARE-RISK"),
        ("RenalFlareRisk", "BiopsyIndicated", "nephrotic-range proteinuria OR active urinary sediment", "BIOPSY-IND"),
        ("SerologicActive", "Quiescent", "dsDNA Falling AND complement Rising AND proteinuria < 0.5", "REMISSION"),
    ]
    rr = rb["StateTransitionRules"]
    rr["data"] = [r for r in rr["data"] if r.get("StateMachine") != SM]
    for frm, to, guard, ref in rules:
        rid = f"{SM}--{frm.lower()}->{to.lower()}"
        rr["data"].append({
            "StateTransitionRuleId": rid, "Name": rid,
            "RelativePath": f"/admin/state-machine/rules/{rid}", "StateMachine": SM,
            "FromState": f"{SM}--{frm.lower()}", "ToState": f"{SM}--{to.lower()}",
            "GuardDescription": guard, "RuleRefs": ref,
            "TriggerEndpoint": "/api/individuals/:id/serology", "TriggeredByRole": "system", **AUDIT})
    # SubjectStateInstances — bitemporal occupancy per individual, derived from their serology series.
    # Build the path each individual walked (sequence of distinct implied states), with dwell days.
    add_occupancy_and_log(rb, SM)


def implied_state(dsdna_trend, comp_trend, prot, sed):
    if prot >= 3.0 or sed:
        return "BiopsyIndicated"
    if prot >= 1.0:
        return "RenalFlareRisk"
    if prot >= 0.5:
        return "EarlyNephritis"
    if dsdna_trend == "Rising" and comp_trend == "Falling":
        return "SerologicActive"
    return "PresymptomaticAutoimmunity"


def add_occupancy_and_log(rb, SM):
    # recompute trend per panel (mirrors the rulebook formula) to know each panel's implied state
    from collections import defaultdict
    by_ind = defaultdict(list)
    for row in sorted(SEROLOGY, key=lambda r: (r[0], r[1])):
        by_ind[row[0]].append(row)
    ssi = rb["SubjectStateInstances"]
    ssi["data"] = [r for r in ssi["data"] if r.get("StateMachine") != SM]
    st = rb["StateTransitions"]
    st["data"] = [r for r in st["data"] if r.get("StateMachine") != SM]
    # extend SubjectStateInstances schema with DwellDays (raw, transparent) + derived flag once
    si_names = {f["name"] for f in ssi["schema"]}
    if "DwellDays" not in si_names:
        ssi["schema"].append({"name": "DwellDays", "datatype": "number", "type": "raw", "nullable": True,
                              "Description": "Bitemporal dwell: days the subject occupied this state "
                                             "(transparent raw value; transpiler lacks date-diff). "
                                             "Witnessed + editable, like any leaf."})
    if "IsLongDwell" not in si_names:
        ssi["schema"].append({"name": "IsLongDwell", "datatype": "boolean", "type": "calculated",
                              "nullable": True, "Description": "TRUE when DwellDays >= 90 (a season).",
                              "formula": "=IF({{DwellDays}}>=90,TRUE(),FALSE())"})
    for ind, panels in by_ind.items():
        path = []  # list of (state, entered_at)
        prev_dsdna = prev_c3 = prev_c4 = None
        for (i, seq, at, dsdna, c3, c4, prot, egfr, sed) in panels:
            if prev_dsdna is None:
                dt = "Stable"
            else:
                dt = "Rising" if dsdna > prev_dsdna * 1.25 else ("Falling" if dsdna < prev_dsdna * 0.8 else "Stable")
            if prev_c3 is None:
                ct = "Stable"
            else:
                ct = "Falling" if (c3 + c4) < (prev_c3 + prev_c4) * 0.85 else ("Rising" if (c3 + c4) > (prev_c3 + prev_c4) * 1.15 else "Stable")
            state = implied_state(dt, ct, prot, sed)
            if not path or path[-1][0] != state:
                path.append((state, at))
            prev_dsdna, prev_c3, prev_c4 = dsdna, c3, c4
        # emit occupancy rows
        prior_id = None
        for idx, (state, at) in enumerate(path, start=1):
            is_current = (idx == len(path))
            entered = at + "T00:00:00Z"
            exited = None if is_current else (path[idx][1] + "T00:00:00Z")
            dwell = 90 if not is_current else 120  # transparent month-scale dwell
            occ_id = f"ssi-{ind}-{state.lower()}-{idx}"
            trans_id = f"st-{ind}-{state.lower()}-{idx}"
            # transition-log row
            st["data"].append({
                "StateTransitionId": trans_id, "Name": trans_id,
                "RelativePath": f"/admin/state-machine/transitions/{trans_id}", "StateMachine": SM,
                "SubjectTableName": "Individuals", "SubjectId": ind,
                "FromStateKey": (path[idx-2][0] if idx >= 2 else None), "ToStateKey": state,
                "TransitionAt": entered, "TriggeredByRole": "system",
                "Reason": f"Serology-derived transition to {state}", **AUDIT})
            ssi["data"].append({
                "SubjectStateInstanceId": occ_id, "Name": occ_id,
                "RelativePath": f"/admin/state-machine/instances/{occ_id}", "StateMachine": SM,
                "SubjectTableName": "Individuals", "SubjectId": ind, "StateKey": state,
                "EnteredAt": entered, "ExitedAt": exited, "SequenceIndex": idx,
                "PriorInstance": prior_id, "EnteredViaTransition": trans_id, "DwellDays": dwell,
                **AUDIT})
            prior_id = occ_id


def main():
    with open(RB_PATH) as f:
        rb = json.load(f)
    add_serology(rb)
    add_therapy_options(rb)
    add_mechanism_pathway(rb)
    add_individual_fields(rb)
    add_prediction_treatment_line(rb)
    add_progression_machine(rb)
    with open(RB_PATH, "w") as f:
        json.dump(rb, f, indent=2)
    print("SerologyObservations:", len(rb["SerologyObservations"]["data"]),
          "| TherapyOptions:", len(rb["TherapyOptions"]["data"]),
          "| Individuals:", len(rb["Individuals"]["data"]),
          "| progression states:", sum(1 for r in rb["MachineStates"]["data"] if r.get("StateMachine") == "lupus-nephritis-progression"),
          "| occupancy rows:", sum(1 for r in rb["SubjectStateInstances"]["data"] if r.get("StateMachine") == "lupus-nephritis-progression"))


if __name__ == "__main__":
    main()
