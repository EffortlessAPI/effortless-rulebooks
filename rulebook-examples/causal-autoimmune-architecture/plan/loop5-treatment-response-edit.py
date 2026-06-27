#!/usr/bin/env python3
"""Loop 5 — Treatment-response prediction (third prediction type).

One coherent rule-change to effortless-rulebook.json. Mirrors Loop 4 (severity), but
the source table is Treatments and the grounding is a MECHANISM MATCH:

  Treatment --TargetsMechanism(FK)--> CausalMechanism --IsCausalArchitectureNode--> confirmed?

A treatment-response prediction is trustworthy only when the treatment targets a
CONFIRMED causal mechanism (IsMechanismMatched) AND the treatment is effective
(response in {Complete,Partial} and not adverse — the existing IsEffectiveTreatment).
So a drug aimed at a debunked mechanism, or one that didn't work / was adverse, is
never "predicted to respond".

  (1) Treatments: TargetsMechanism (raw FK), IsMechanismMatched (lookup of the target
      mechanism's IsCausalArchitectureNode), IsTreatmentResponsePredicted (calc:
      IsEffectiveTreatment ∧ IsMechanismMatched), TreatmentResponseDecidingFactor.
  (2) Individuals: CountPredictedTreatmentResponses (agg) + HasPredictedTreatmentResponse.
  (3) IndividualPredictions: IndividualHasPredictedTreatmentResponse (lookup),
      IsTreatmentResponseActionable + TreatmentResponseDecidingFactor at prediction level.
  (4) Treatments data: seed all 7 patients (replace the single tx-context row).

Onset keystone + severity contracts untouched — this is purely additive.
Idempotent. ensure_ascii=False to preserve literal em-dashes (file convention).
"""
import json, collections

RB_PATH = 'effortless-rulebook/effortless-rulebook.json'
rb = json.load(open(RB_PATH), object_pairs_hook=collections.OrderedDict)

def field(name, datatype, ftype, desc, formula=None, nullable=True):
    d = collections.OrderedDict()
    d['name'] = name; d['datatype'] = datatype; d['type'] = ftype
    d['nullable'] = nullable; d['Description'] = desc
    if formula is not None:
        d['formula'] = formula
    return d

def has_field(schema, name):
    return any(f.get('name') == name for f in schema)

def insert_after(schema, after_name, new_field):
    if has_field(schema, new_field['name']):
        return False
    for i, f in enumerate(schema):
        if f.get('name') == after_name:
            schema.insert(i + 1, new_field); return True
    schema.append(new_field); return True

added = []

# ---------------------------------------------------------------------------
# (1) Treatments: the mechanism-match chain.
# ---------------------------------------------------------------------------
tx = rb['Treatments']['schema']

if insert_after(tx, 'AutoimmuneDisease',
    field('TargetsMechanism', 'string', 'raw',
          "FK to the CausalMechanism this treatment acts on (the drug's target mechanism). "
          "Raw leaf: which mechanism the clinician/LLM says this therapy targets.", nullable=True)):
    added.append('Treatments.TargetsMechanism (raw FK)')

if insert_after(tx, 'IsEffectiveTreatment',
    field('IsMechanismMatched', 'boolean', 'lookup',
          "True when the treatment's target mechanism is a CONFIRMED causal-architecture node "
          "(empty-guarded). This is the 'mechanism match'.",
          formula='=IF({{TargetsMechanism}} = "", FALSE(), '
                  'INDEX(CausalMechanisms!{{IsCausalArchitectureNode}}, '
                  'MATCH({{TargetsMechanism}}, CausalMechanisms!{{CausalMechanismId}}, 0)))')):
    added.append('Treatments.IsMechanismMatched (lookup)')

if insert_after(tx, 'IsMechanismMatched',
    field('IsTreatmentResponsePredicted', 'boolean', 'calculated',
          "Derived: the treatment is effective AND targets a confirmed mechanism. A drug aimed "
          "at a debunked mechanism, or one that didn't respond / was adverse, is NOT predicted.",
          formula='=IF(AND({{IsEffectiveTreatment}}, {{IsMechanismMatched}}), TRUE(), FALSE())')):
    added.append('Treatments.IsTreatmentResponsePredicted (calc)')

if insert_after(tx, 'IsTreatmentResponsePredicted',
    field('TreatmentResponseDecidingFactor', 'string', 'calculated',
          "Why response is/ isn't predicted — the single deciding reason.",
          formula='=IF({{IsTreatmentResponsePredicted}}, "EffectiveOnConfirmedMechanism", '
                  'IF(NOT({{IsMechanismMatched}}), "NoConfirmedMechanism", '
                  'IF({{HasAdverseEffect}}, "AdverseEffect", '
                  'IF(OR({{TreatmentResponse}} = "None", {{TreatmentResponse}} = "Adverse"), "NoResponse", '
                  '"Undetermined"))))')):
    added.append('Treatments.TreatmentResponseDecidingFactor (calc)')

# ---------------------------------------------------------------------------
# (2) Individuals: roll up predicted treatment responses.
# ---------------------------------------------------------------------------
ind = rb['Individuals']['schema']

if insert_after(ind, 'HasHighSeverityPhenotype',
    field('CountPredictedTreatmentResponses', 'integer', 'aggregation',
          "Count of this individual's treatments predicted to respond (effective ∧ mechanism-matched).",
          formula='=COUNTIFS(Treatments!{{Individual}}, {{IndividualId}}, '
                  'Treatments!{{IsTreatmentResponsePredicted}}, TRUE())')):
    added.append('Individuals.CountPredictedTreatmentResponses')

if insert_after(ind, 'CountPredictedTreatmentResponses',
    field('HasPredictedTreatmentResponse', 'boolean', 'calculated',
          "True when the individual has at least one treatment predicted to respond.",
          formula='=IF({{CountPredictedTreatmentResponses}} >= 1, TRUE(), FALSE())')):
    added.append('Individuals.HasPredictedTreatmentResponse')

# ---------------------------------------------------------------------------
# (3) IndividualPredictions: surface treatment-response at the prediction level.
# ---------------------------------------------------------------------------
ip = rb['IndividualPredictions']['schema']

if insert_after(ip, 'IndividualHasHighSeverityPhenotype',
    field('IndividualHasPredictedTreatmentResponse', 'boolean', 'lookup',
          "Whether this individual has a treatment predicted to respond (empty-guarded).",
          formula='=IF({{Individual}} = "", FALSE(), '
                  'INDEX(Individuals!{{HasPredictedTreatmentResponse}}, '
                  'MATCH({{Individual}}, Individuals!{{IndividualId}}, 0)))')):
    added.append('IndividualPredictions.IndividualHasPredictedTreatmentResponse')

if insert_after(ip, 'SeverityDecidingFactor',
    field('IsTreatmentResponseActionable', 'boolean', 'calculated',
          "Derived: the individual has a treatment predicted to respond (effective therapy on a "
          "confirmed mechanism). The third prediction type — independent of onset/severity.",
          formula='=IF({{IndividualHasPredictedTreatmentResponse}}, TRUE(), FALSE())')):
    added.append('IndividualPredictions.IsTreatmentResponseActionable')

if insert_after(ip, 'IsTreatmentResponseActionable',
    field('TreatmentResponseDecidingFactor', 'string', 'calculated',
          "Why treatment-response is/ isn't actionable for this individual.",
          formula='=IF({{IsTreatmentResponseActionable}}, "EffectiveOnConfirmedMechanism", '
                  'IF({{RestsOnConfirmedMechanism}}, "NoEffectiveTreatmentOnMechanism", '
                  '"NoConfirmedMechanism"))')):
    added.append('IndividualPredictions.TreatmentResponseDecidingFactor')

# ---------------------------------------------------------------------------
# (4) Treatments data: seed all 7 patients.
#   Matrix (confirmed nodes: a,b,d,f,g; debunked: c,e):
#     A cm-a confirmed, Partial,  no AE  -> PREDICTED
#     B cm-b confirmed, Complete, no AE  -> PREDICTED  (onset failed on calibration — independence)
#     C cm-c debunked,  Partial,  no AE  -> not (NoConfirmedMechanism)
#     D cm-d confirmed, Complete, AE     -> not (AdverseEffect)
#     E cm-e debunked,  None,     no AE  -> not (NoConfirmedMechanism)
#     F cm-f confirmed, None,     no AE  -> not (NoResponse)
#     G cm-g confirmed, Partial,  no AE  -> PREDICTED
# ---------------------------------------------------------------------------
TREATMENTS = collections.OrderedDict([
    ('ind-a-reyes',  ('sle', 'cm-a', 'Anifrolumab (anti-IFNAR1)',        'Partial',  False, 'IRF5/IFN-axis therapy targeting the confirmed mechanism')),
    ('ind-b-okafor', ('sle', 'cm-b', 'Anifrolumab (anti-IFNAR1)',        'Complete', False, 'IRF5/IFN-axis therapy targeting the confirmed mechanism')),
    ('ind-c-chen',   ('sle', 'cm-c', 'STAT4-pathway inhibitor (trial)',  'Partial',  False, 'targets a mechanism that failed validation')),
    ('ind-d-santos', ('sle', 'cm-d', 'Anifrolumab (anti-IFNAR1)',        'Complete', True,  'effective on a confirmed mechanism but adverse event observed')),
    ('ind-e-mensah', ('sle', 'cm-e', 'CTLA4-Ig (abatacept, off-target)', 'None',     False, 'targets a non-falsifiable / unvalidated mechanism, no response')),
    ('ind-f-haidar', ('psa', 'cm-f', 'Secukinumab (anti-IL-17)',         'None',     False, 'targets the confirmed mechanism but the patient did not respond')),
    ('ind-g-lin',    ('psa', 'cm-g', 'Secukinumab (anti-IL-17)',         'Partial',  False, 'IL-17 therapy targeting the confirmed mechanism')),
])

txdata = rb['Treatments']['data']
existing_ids = {r.get('TreatmentId') for r in txdata}
new_tx = []
for iid, (disease, mech, label, response, adverse, _why) in TREATMENTS.items():
    tid = f"tx-{iid.split('-')[1]}"  # tx-a ... tx-g
    if tid in existing_ids:
        continue
    row = collections.OrderedDict()
    row['TreatmentId'] = tid
    row['TreatmentLabel'] = label
    row['Individual'] = iid
    row['AutoimmuneDisease'] = disease
    row['TargetsMechanism'] = mech
    row['TreatmentResponse'] = response
    row['HasTreatmentInducedChange'] = response in ('Complete', 'Partial')
    row['HasAdverseEffect'] = adverse
    row['StartDate'] = "2024-04-01"
    new_tx.append(row)
    added.append(f'Treatments.{tid} ({response}{"/AE" if adverse else ""}, targets {mech})')

if new_tx:
    rb['Treatments']['data'] = new_tx  # drop tx-context; seed the 7 real rows

# ---------------------------------------------------------------------------
json.dump(rb, open(RB_PATH, 'w'), indent=2, ensure_ascii=False)
open(RB_PATH, 'a').write('\n')

print("RULE CHANGE applied. Added/updated:")
for a in added:
    print("  +", a)
if not added:
    print("  (nothing — already applied; idempotent no-op)")
