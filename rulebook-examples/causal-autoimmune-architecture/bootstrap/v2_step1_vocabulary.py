#!/usr/bin/env python3
"""
v2 Step 1 — Vocabulary completeness.

Adds a `DiseaseDomainConcepts` lookup table to the rulebook hub holding EVERY
disease-domain concept the v1 audit named as missing, plus the modeling status of
each (vocabulary-only vs schema vs deep-DAG). Also adds the load-bearing glossary
terms. Idempotent: re-running replaces the table/terms rather than duplicating.

This is Step 1 of the v2 audit response (see V2-RESPONSE-PROPOSAL.md). It does NOT
build the deep inference DAGs (that's Step 3) — it makes the coverage claim
checkable: `grep` the rulebook and every word the doctor listed is present.
"""
import json, os, sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RB_PATH = os.path.join(ROOT, "effortless-rulebook", "effortless-rulebook.json")

# Every concept the v1 audit named, with the modeling status we commit to in v2.
# status: "deep-dag"      -> carries a witnessed inference DAG (the load-bearing ones)
#         "schema"        -> first-class schema + data rows, derived fields, not a full progression DAG
#         "vocabulary"    -> present as a concept/stressor type so coverage is not "absent"
# Each row also records WHICH original-challenge stressor TYPE it represents, so the
# reconcile step (Step 7) can map concepts -> challenge clauses.
CONCEPTS = [
    # ---- the two LOAD-BEARING deep DAGs (the doctor's own worked examples) ----
    ("lupus-nephritis", "Lupus Nephritis", "SLE", "deep-dag",
     "Renal involvement of SLE; immune-complex glomerulonephritis. v2 models its PROGRESSION as a witnessed state machine driven by rising anti-dsDNA + falling complement + proteinuria.",
     "disease-progression / organ-specific subtype"),
    ("treatment-line-selection", "Treatment-Line Selection", "SLE", "deep-dag",
     "Which therapy to choose given the patient's causal mechanism + disease state: mycophenolate (MMF) vs belimumab vs anifrolumab. v2 derives a recommended line + single deciding reason.",
     "treatment response / patient stratification"),
    # ---- SLE organ-domain subtypes (schema-level: first-class phenotype subtypes) ----
    ("neuropsychiatric-lupus", "Neuropsychiatric Lupus (NPSLE)", "SLE", "schema",
     "CNS/PNS manifestations of SLE (seizure, psychosis, cognitive). Modeled as a phenotype subtype carrying a SeverityScore and organ-domain tag.",
     "tissue-specific / cell-state-specific effects"),
    ("cutaneous-lupus", "Cutaneous Lupus", "SLE", "schema",
     "Skin-limited or skin-predominant lupus (acute/subacute/chronic discoid). Phenotype subtype with organ-domain = skin.",
     "tissue-specific effects"),
    ("organ-damage-slicc", "Organ Damage (SLICC/ACR Damage Index)", "SLE", "schema",
     "Irreversible accumulated damage, SLICC-style. Modeled as a monotonic damage-accrual score distinct from reversible disease activity.",
     "disease progression / feedback between progression and molecular state"),
    # ---- RA subtypes ----
    ("seropositive-ra", "Seropositive RA (ACPA/RF+)", "RA", "schema",
     "RF/anti-CCP-positive RA; stronger HLA shared-epitope + PTPN22 association, more erosive. Phenotype subtype with a serostatus tag.",
     "coding/HLA variants; ancestry-specific risk alleles"),
    ("seronegative-ra", "Seronegative RA (ACPA/RF-)", "RA", "schema",
     "Autoantibody-negative RA; distinct genetic architecture and trajectory. Phenotype subtype.",
     "heterogeneity within one disease"),
    ("erosive-disease", "Erosive Disease", "RA", "schema",
     "Radiographic bone erosion; a structural-damage progression axis for RA, distinct from disease activity.",
     "disease progression / organ damage"),
    # ---- PsA / spondyloarthritis domain ----
    ("axial-psa", "Axial Psoriatic Arthritis", "PsA", "schema",
     "Spinal/sacroiliac involvement of PsA. Phenotype subtype with axial domain tag.",
     "tissue-specific effects"),
    ("enthesitis", "Enthesitis", "PsA", "schema",
     "Inflammation at tendon/ligament insertions; hallmark of PsA/SpA. Phenotype subtype.",
     "tissue-specific effects"),
    ("dactylitis", "Dactylitis", "PsA", "schema",
     "'Sausage digit' — whole-digit swelling. PsA phenotype subtype.",
     "tissue-specific effects"),
    ("uveitis", "Uveitis", "PsA", "schema",
     "Inflammatory eye disease overlapping spondyloarthritis. Phenotype subtype / extra-articular domain.",
     "pleiotropy / shared mechanism across tissues"),
    ("ibd-overlap", "Inflammatory Bowel Disease Overlap", "PsA", "schema",
     "Gut inflammation overlapping spondyloarthritis (IL-23/IL-17 axis shared). Phenotype subtype representing cross-disease pleiotropy.",
     "pleiotropy / gene-environment-microbiome interaction"),
    # ---- cross-cutting clinical-process concepts ----
    ("flare-pattern", "Flare Pattern", "*", "schema",
     "Episodic relapse of disease activity. v2 models flare-risk as a derived state on the progression machine, driven by serology trajectory.",
     "feedback between disease progression and molecular state"),
    ("disease-activity-score", "Disease-Activity Score (SLEDAI / DAS28)", "*", "deep-dag",
     "Composite activity indices (SLEDAI-2K for SLE, DAS28 for RA). v2 DERIVES an activity tier from raw component observations rather than hand-entering it.",
     "calibrated severity / disease stage"),
    ("treatment-line", "Treatment Line", "*", "schema",
     "First-line / second-line / advanced-therapy sequencing. Modeled as an ordinal on therapies feeding treatment-line selection.",
     "treatment histories / treatment-induced changes"),
    ("adverse-event-surveillance", "Adverse-Event Surveillance", "*", "vocabulary",
     "Monitoring for therapy harms. Present as a concept/stressor type; the keystone already carries HasAdverseEffect at the leaf. Deep AE modeling is out-of-scope for the POC.",
     "adverse effects"),
    # ---- the clinical workflow stages the audit said were absent ----
    ("workflow-referral", "Workflow: Referral", "*", "vocabulary",
     "Entry into specialist care. Represented as the initial state of the clinical-workflow machine.",
     "clinical workflow"),
    ("workflow-differential", "Workflow: Differential Diagnosis", "*", "vocabulary",
     "Weighing competing diagnoses. Workflow state.",
     "clinical workflow"),
    ("workflow-workup", "Workflow: Workup", "*", "vocabulary",
     "Ordering the labs/imaging that BECOME the raw leaves. Workflow state.",
     "clinical workflow"),
    ("workflow-classification", "Workflow: Classification", "*", "vocabulary",
     "Applying classification criteria (e.g. EULAR/ACR). Workflow state.",
     "clinical workflow"),
    ("workflow-baseline-severity", "Workflow: Baseline Severity Assessment", "*", "vocabulary",
     "Establishing baseline activity/damage. Workflow state; consumes the derived activity score.",
     "clinical workflow / baseline severity"),
    ("workflow-treatment-choice", "Workflow: Treatment Choice", "*", "vocabulary",
     "Selecting therapy; consumes the treatment-line-selection DAG. Workflow state.",
     "clinical workflow / treatment"),
    ("workflow-monitoring", "Workflow: Monitoring", "*", "vocabulary",
     "Longitudinal follow-up; produces the serology trajectory leaves. Workflow state.",
     "clinical workflow / monitoring"),
    ("workflow-flare-management", "Workflow: Flare Management", "*", "vocabulary",
     "Responding to a flare. Workflow state tied to the flare-risk progression state.",
     "clinical workflow / flare"),
    ("workflow-outcomes", "Workflow: Long-Term Outcomes", "*", "vocabulary",
     "Damage accrual, remission, outcomes over time. Workflow terminal state.",
     "clinical workflow / outcomes"),
]

GLOSSARY_ADDS = [
    ("disease-state-simulator",
     "The v2 layer the audit asked for: disease modeled as a WITNESSED state machine whose transitions fire only on raw longitudinal observations (rising anti-dsDNA, falling C3/C4, proteinuria). Current state and dwell-time are derived & bitemporal — the contrast the audit drew with an 'evidence gate'."),
    ("evidence-gate",
     "The v1 layer: per-patient adjudication of whether a prediction is trustworthy enough to act on (the four gates + IsClinicallyActionable). v2 keeps this and adds the disease-state simulator above it on the same hub."),
    ("disease-progression-machine",
     "A state machine on the polymorphic StateMachines substrate whose subject is a patient's disease course (presymptomatic -> serologic-active -> early-nephritis -> renal-flare-risk -> biopsy-indicated). Transitions are raw-leaf-triggered; current state is the derived IsCurrent occupancy."),
    ("dwell-time",
     "Bitemporal derivation: how long a subject has been in a disease state (now - EnteredAt while current; ExitedAt - EnteredAt once left). The audit asked for 'how long has the patient/disease been in a state' — this is that, as a formula."),
    ("serology-trajectory",
     "The raw longitudinal leaves driving progression: anti-dsDNA titre trend (rising/stable/falling), complement C3/C4 trend, proteinuria. The LLM/lab may emit these; everything above them is derived."),
    ("treatment-line-recommendation",
     "Derived field: given the patient's confirmed causal mechanism + current disease state, the recommended therapy (MMF vs belimumab vs anifrolumab) and the single deciding reason. The audit's second worked example."),
    ("corpus-discovery",
     "The cohort-level surface where causal-hypothesis patterns EMERGE across many patients (vs v1's per-patient verdicts). Discovery is inherently corpus-level; this is the v2 reframe of the audit's 'you don't do discovery' critique."),
]


def upsert_table(rb):
    schema = [
        {"name": "DiseaseDomainConceptId", "datatype": "string", "type": "raw", "nullable": False,
         "Description": "Slug PK."},
        {"name": "ConceptLabel", "datatype": "string", "type": "raw", "nullable": False,
         "Description": "Human-readable concept name (the audit's missing word)."},
        {"name": "RelatedDisease", "datatype": "string", "type": "raw", "nullable": True,
         "Description": "Disease this concept belongs to (SLE/RA/PsA/* for cross-cutting). Convention string, not a FK."},
        {"name": "ModelingStatus", "datatype": "string", "type": "raw", "nullable": False,
         "Description": "deep-dag | schema | vocabulary — how deeply v2 models this concept."},
        {"name": "Definition", "datatype": "string", "type": "raw", "nullable": True,
         "Description": "What the concept is and how v2 represents it."},
        {"name": "ChallengeStressorType", "datatype": "string", "type": "raw", "nullable": True,
         "Description": "Which TYPE of original-challenge stressor this concept instantiates (for the reconcile map)."},
        {"name": "Name", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Echoes ConceptLabel.", "formula": "={{ConceptLabel}}"},
        {"name": "RelativePath", "datatype": "string", "type": "calculated", "nullable": True,
         "Description": "Path to this concept's page.",
         "formula": '="/admin/disease-concepts/" & {{DiseaseDomainConceptId}}'},
        {"name": "IsDeeplyModeled", "datatype": "boolean", "type": "calculated", "nullable": True,
         "Description": "TRUE when this concept carries a witnessed inference DAG (status = deep-dag).",
         "formula": '=IF({{ModelingStatus}}="deep-dag", TRUE(), FALSE())'},
        {"name": "IsSchemaModeled", "datatype": "boolean", "type": "calculated", "nullable": True,
         "Description": "TRUE when first-class schema/data (deep-dag or schema).",
         "formula": '=IF(OR({{ModelingStatus}}="deep-dag",{{ModelingStatus}}="schema"), TRUE(), FALSE())'},
    ]
    data = []
    for cid, label, dis, status, defn, stressor in CONCEPTS:
        data.append({
            "DiseaseDomainConceptId": cid,
            "ConceptLabel": label,
            "RelatedDisease": dis,
            "ModelingStatus": status,
            "Definition": defn,
            "ChallengeStressorType": stressor,
            "Name": label,
            "RelativePath": f"/admin/disease-concepts/{cid}",
            "IsDeeplyModeled": status == "deep-dag",
            "IsSchemaModeled": status in ("deep-dag", "schema"),
        })
    rb["DiseaseDomainConcepts"] = {
        "Description": "v2 vocabulary completeness: every disease-domain concept the v1 audit named, "
                       "with its modeling status and the challenge-stressor TYPE it instantiates. "
                       "Makes the coverage claim checkable by grep, not by trust.",
        "schema": schema,
        "data": data,
    }


def upsert_glossary(rb):
    gt = rb["GlossaryTerms"]
    existing = {r["GlossaryTermId"]: r for r in gt["data"]}
    for term, defn in GLOSSARY_ADDS:
        gid = f"gt-{term}"
        existing[gid] = {
            "GlossaryTermId": gid,
            "Term": term.replace("-", " "),
            "Definition": defn,
            "Name": term.replace("-", " "),
            "RelativePath": f"/admin/glossary/{gid}",
        }
    gt["data"] = list(existing.values())


def main():
    with open(RB_PATH) as f:
        rb = json.load(f)
    upsert_table(rb)
    upsert_glossary(rb)
    with open(RB_PATH, "w") as f:
        json.dump(rb, f, indent=2)
    n = len(rb["DiseaseDomainConcepts"]["data"])
    deep = sum(1 for r in rb["DiseaseDomainConcepts"]["data"] if r["ModelingStatus"] == "deep-dag")
    print(f"DiseaseDomainConcepts: {n} concepts ({deep} deep-dag). GlossaryTerms: {len(rb['GlossaryTerms']['data'])} total.")


if __name__ == "__main__":
    main()
