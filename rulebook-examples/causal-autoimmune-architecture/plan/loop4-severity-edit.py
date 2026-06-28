#!/usr/bin/env python3
"""Loop 4 — Second prediction type (severity).

One coherent rule-change to effortless-rulebook.json:
  (1) Individuals: roll up MaxSeverityScore + HasHighSeverityPhenotype from ClinicalPhenotypes.
  (2) IndividualPredictions: derive a severity chain that is GATED on the existing
      onset mechanism gates (RestsOnConfirmedMechanism ∧ NOT HasSpuriousCorrelationFlag)
      AND a high-severity phenotype — so severity can never be "actionable" on a
      debunked/spurious mechanism.
  (3) ClinicalPhenotypes data: seed all 7 patients with the confirmed severity matrix
      A=9 B=8 C=9 D=8 E=3 F=2 G=6 (high-severity threshold is >7, per existing IsHighSeverity).

Idempotent: re-running adds nothing already present. Raw inputs only on seed rows;
calculated fields (ParentPath/RelativePath/IsHighSeverity/tiers) are filled by `effortless build`.
"""
import json, sys, collections

RB_PATH = 'effortless-rulebook/effortless-rulebook.json'
rb = json.load(open(RB_PATH), object_pairs_hook=collections.OrderedDict)

def field(name, datatype, ftype, desc, formula=None, nullable=True):
    d = collections.OrderedDict()
    d['name'] = name
    d['datatype'] = datatype
    d['type'] = ftype
    d['nullable'] = nullable
    d['Description'] = desc
    if formula is not None:
        d['formula'] = formula
    return d

def has_field(schema, name):
    return any(f.get('name') == name for f in schema)

def insert_after(schema, after_name, new_field):
    """Insert new_field right after the field named after_name (or append)."""
    if has_field(schema, new_field['name']):
        return False
    for i, f in enumerate(schema):
        if f.get('name') == after_name:
            schema.insert(i + 1, new_field)
            return True
    schema.append(new_field)
    return True

added = []

# ---------------------------------------------------------------------------
# (1) Individuals rollup hub: severity aggregations over ClinicalPhenotypes.
# ---------------------------------------------------------------------------
ind_schema = rb['Individuals']['schema']

if insert_after(ind_schema, 'CountCrossAncestryConfirmedNodes',
    field('MaxSeverityScore', 'number', 'aggregation',
          "Highest SeverityScore across this individual's clinical phenotypes (0 if none).",
          formula='=MAXIFS(ClinicalPhenotypes!{{SeverityScore}}, ClinicalPhenotypes!{{Individual}}, {{IndividualId}})')):
    added.append('Individuals.MaxSeverityScore')

if insert_after(ind_schema, 'MaxSeverityScore',
    field('CountHighSeverityPhenotypes', 'integer', 'aggregation',
          "Count of this individual's high-severity phenotypes (SeverityScore > 7).",
          formula='=COUNTIFS(ClinicalPhenotypes!{{Individual}}, {{IndividualId}}, ClinicalPhenotypes!{{IsHighSeverity}}, TRUE())')):
    added.append('Individuals.CountHighSeverityPhenotypes')

if insert_after(ind_schema, 'CountHighSeverityPhenotypes',
    field('HasHighSeverityPhenotype', 'boolean', 'calculated',
          "True when the individual has at least one high-severity phenotype.",
          formula='=IF({{CountHighSeverityPhenotypes}} >= 1, TRUE(), FALSE())')):
    added.append('Individuals.HasHighSeverityPhenotype')

# ---------------------------------------------------------------------------
# (2) IndividualPredictions: the severity chain, gated on the onset mechanism gates.
# ---------------------------------------------------------------------------
ip_schema = rb['IndividualPredictions']['schema']

# Lookups from Individuals (empty-guarded, matching IndividualCausalMass idiom).
if insert_after(ip_schema, 'IndividualHasCrypticRelatedness',
    field('IndividualMaxSeverityScore', 'number', 'lookup',
          "This individual's max clinical SeverityScore (empty-guarded).",
          formula='=IF({{Individual}} = "", 0, INDEX(Individuals!{{MaxSeverityScore}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))')):
    added.append('IndividualPredictions.IndividualMaxSeverityScore')

if insert_after(ip_schema, 'IndividualMaxSeverityScore',
    field('IndividualHasHighSeverityPhenotype', 'boolean', 'lookup',
          "Whether this individual has a high-severity phenotype (empty-guarded).",
          formula='=IF({{Individual}} = "", FALSE(), INDEX(Individuals!{{HasHighSeverityPhenotype}}, MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))')):
    added.append('IndividualPredictions.IndividualHasHighSeverityPhenotype')

# Derived severity value + tier (parallel to PredictedValue / PatientStratificationTier).
if insert_after(ip_schema, 'PatientStratificationTier',
    field('PredictedSeverityValue', 'number', 'calculated',
          "Derived severity prediction grounded in the individual's max clinical SeverityScore.",
          formula='={{IndividualMaxSeverityScore}}')):
    added.append('IndividualPredictions.PredictedSeverityValue')

if insert_after(ip_schema, 'PredictedSeverityValue',
    field('SeverityTier', 'string', 'calculated',
          "Derived severity band from the predicted severity value.",
          formula='=IF({{PredictedSeverityValue}} > 7, "Severe", IF({{PredictedSeverityValue}} >= 4, "Moderate", "Mild"))')):
    added.append('IndividualPredictions.SeverityTier')

# The chained gate: severity is actionable ONLY when the onset mechanism gates already
# hold (rests on a confirmed, non-spurious node) AND a high-severity phenotype is present.
if insert_after(ip_schema, 'SeverityTier',
    field('IsSeverityActionable', 'boolean', 'calculated',
          "Derived: a high-severity phenotype on a confirmed, non-spurious mechanism. "
          "Chained to the onset gates so severity can never be actionable on a debunked mechanism.",
          formula='=IF(AND({{IndividualHasHighSeverityPhenotype}}, {{RestsOnConfirmedMechanism}}, NOT({{HasSpuriousCorrelationFlag}})), TRUE(), FALSE())')):
    added.append('IndividualPredictions.IsSeverityActionable')

# A one-line reason string for the witness/diagnosis (mirrors DecidingGate's style).
if insert_after(ip_schema, 'IsSeverityActionable',
    field('SeverityDecidingFactor', 'string', 'calculated',
          "Why severity is/ isn't actionable — the single deciding reason.",
          formula='=IF({{IsSeverityActionable}}, "HighSeverityOnConfirmedMechanism", '
                  'IF(NOT({{IndividualHasHighSeverityPhenotype}}), "NotHighSeverity", '
                  'IF(NOT({{RestsOnConfirmedMechanism}}), "NoValidatedMechanism", '
                  'IF({{HasSpuriousCorrelationFlag}}, "SpuriousFlag", "Undetermined"))))')):
    added.append('IndividualPredictions.SeverityDecidingFactor')

# ---------------------------------------------------------------------------
# (3) ClinicalPhenotypes data: seed all 7 patients with the severity matrix.
# ---------------------------------------------------------------------------
SEVERITY = {  # confirmed matrix
    'ind-a-reyes':  ('sle', 'sle-active', 'blood', 9, 'reyes-ana',   'Active SLE — high disease-activity (SLEDAI-equiv) severity'),
    'ind-b-okafor': ('sle', 'sle-active', 'blood', 8, 'okafor-bili', 'Active SLE — serositis + high disease activity'),
    'ind-c-chen':   ('sle', 'sle-active', 'blood', 9, 'wei-chen',    'Active SLE — arthritis + anti-dsDNA high activity'),
    'ind-d-santos': ('sle', 'sle-active', 'kidney',8, 'santos-diego','Active SLE — biopsy-confirmed nephritis (high severity)'),
    'ind-e-mensah': ('sle', 'sle-active', 'blood', 3, 'mensah-esi',  'SLE — mild cytopenias, low disease activity'),
    'ind-f-haidar': ('psa', 'psa-active', 'synovium',2,'haidar-faisal','PsA — minimal joint involvement, mild'),
    'ind-g-lin':    ('psa', 'psa-active', 'skin',   6, 'lin-grace',   'PsA — moderate plaque + oligoarthritis'),
}

cp = rb['ClinicalPhenotypes']['data']
existing_ids = {r.get('ClinicalPhenotypeId') for r in cp}

# Replace the single "context only" row with one real severity phenotype per patient.
# (Keep ph-context out: it was patient-A context with SeverityScore 4; the new ph-sev-a
#  is the load-bearing severity row. Drop ph-context to avoid two A phenotypes muddying
#  MAXIFS — A's max must be 9, and ph-context's 4 wouldn't change that, but the witness
#  reads cleaner with exactly one severity row per patient.)
new_cp = []
for iid, (disease, stage, tissue, score, slug, label) in SEVERITY.items():
    pid = f"ph-sev-{iid.split('-')[1]}"  # ph-sev-a ... ph-sev-g
    if pid in existing_ids:
        continue
    row = collections.OrderedDict()
    row['ClinicalPhenotypeId'] = pid
    row['PhenotypeLabel'] = label
    row['Individual'] = iid
    row['AutoimmuneDisease'] = disease
    row['DiseaseStage'] = stage
    row['Tissue'] = tissue
    row['SeverityScore'] = score
    row['MeasurementDate'] = "2024-03-01"
    row['HasImmuneDysfunction'] = True
    new_cp.append(row)
    added.append(f'ClinicalPhenotypes.{pid} (severity {score})')

if new_cp:
    # Drop the old context-only row; seed the 7 real severity rows.
    rb['ClinicalPhenotypes']['data'] = new_cp

# ---------------------------------------------------------------------------
json.dump(rb, open(RB_PATH, 'w'), indent=2, ensure_ascii=False)  # keep literal UTF-8 (em-dashes) — file convention
open(RB_PATH, 'a').write('\n')  # match trailing newline convention

print("RULE CHANGE applied. Added/updated:")
for a in added:
    print("  +", a)
if not added:
    print("  (nothing — already applied; idempotent no-op)")
