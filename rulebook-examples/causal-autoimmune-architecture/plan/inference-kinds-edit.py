#!/usr/bin/env python3
"""Add an InferenceKinds capture table: the FAMILIES of derivation the platform
performs, so PLATFORM_FEATURES.md can open with a short overview of the *kinds*
of inference (Transitive closure, Higher-order inference, Aggregation rollups, ...)
without enumerating all 24 individual features.

This is a reasoning/capture table, sibling to Axioms / Features / OpenQuestions /
NonGoals / GlossaryTerms — NOT a domain table. Every row is grounded in field types
that actually exist in this rulebook (verified: 35 lookup, 33 aggregation, 200
calculated, 1 closure field; INDEX/MATCH x49, COUNTIFS x27; the closure view
vw_state_transition_rules_closure is live with 18 inferred reachability pairs).

Each row: a Title, a one-line factual Description, a concrete ExampleField that
realizes the kind in the model, an EvidenceCount (how many such fields/uses exist,
for honest weight), and a Maturity (Live | Partial). SortOrder fixes display order.

Idempotent (replaces the table cleanly). ascii-only strings (the json-hbars-transform
engine HTML-entity-escapes non-ASCII), ensure_ascii=False on write to match file convention.
"""
import json, collections

RB_PATH = 'effortless-rulebook/effortless-rulebook.json'
rb = json.load(open(RB_PATH), object_pairs_hook=collections.OrderedDict)
OD = collections.OrderedDict


def field(name, datatype, ftype, desc, formula=None, nullable=True):
    d = OD()
    d['name'] = name; d['datatype'] = datatype; d['type'] = ftype
    d['nullable'] = nullable; d['Description'] = desc
    if formula is not None:
        d['formula'] = formula
    return d


# The families. Order = display order. Descriptions are factual and grounded.
KINDS = [
    dict(
        id="ik-lookup-fk",
        title="Lookup / FK resolution",
        maturity="Live",
        evidence="35 lookup fields + 42 relationships; INDEX/MATCH used 49x",
        description=(
            "Pull a related row's value across a foreign key so a derived field can reason over it "
            "in place (no application-side JOIN). The relationship is declared once; the value resolves "
            "wherever it is needed."
        ),
        example="Individuals.FederatedDatasetNodeLabel (resolves the dataset's label by FK)",
    ),
    dict(
        id="ik-aggregation-rollup",
        title="Aggregation rollups",
        maturity="Live",
        evidence="33 aggregation fields; COUNTIFS/MAXIFS/SUMIFS used 33x",
        description=(
            "Collapse many child rows into one parent value - counts, maxima, sums, fractions. This is "
            "how per-row facts become corpus-level patterns: a population signature is a rollup over each "
            "person's raw series, with no label assigned."
        ),
        example="Individuals.MaxProgressionStateOrder; the emergent IsInPreNephriticSignatureCluster rollup",
    ),
    dict(
        id="ik-higher-order",
        title="Higher-order inference (gates over gates)",
        maturity="Live",
        evidence="200 calculated fields; the keystone is an AND of four derived gates",
        description=(
            "Calculated fields that take other calculated fields as inputs, stacked into a multi-level "
            "DAG. The keystone is the apex: a boolean computed over four gates, each themselves computed "
            "over lower derivations - inference about inferences, never a hand-entered answer."
        ),
        example="IndividualPredictions.IsClinicallyActionable (AND of the four derived gates)",
    ),
    dict(
        id="ik-state-machine",
        title="State-machine derivation",
        maturity="Live",
        evidence="2 state machines, 13 states, 14 transition rules; current state is computed",
        description=(
            "A subject's current state is derived from raw longitudinal inputs against a declared "
            "transition graph, not entered. Dwell time per state and the progression key fall out of the "
            "same derivation."
        ),
        example="Individuals.NephritisProgressionStateKey (from rising anti-dsDNA + falling complement)",
    ),
    dict(
        id="ik-transitive-closure",
        title="Transitive closure (reachability inference)",
        maturity="Live",
        evidence="1 closure field -> vw_state_transition_rules_closure: 14 asserted edges, 18 inferred pairs (to hop 5)",
        description=(
            "Sparsely-asserted FromState->ToState edges imply the full reachability ordering, including "
            "never-asserted long-range pairs. The disease trajectory is inferred from the transition "
            "topology (e.g. intake -> actionable at hop 5, is_inferred=true) rather than hand-typed as an "
            "integer ladder - the autoimmune analogue of a step-1 -> step-5 ordering."
        ),
        example="StateTransitionRules.ProgressionClosure (presymptomatic -> biopsy-indicated, inferred)",
    ),
    dict(
        id="ik-predicate-narrative",
        title="Predicate-gated narrative",
        maturity="Live",
        evidence="IF used 166x; per-row narrative fragment fields band raw values into language",
        description=(
            "Each raw fact is turned into a phrase by a per-row formula that selects wording from its "
            "own value and a shared __meta__ phrasebook (e.g. a temperature becomes 'normal' | 'light "
            "fever' | 'severe fever'). The diagnosis writeup is assembled from these inspectable cells - "
            "no free-text generated at render time."
        ),
        example="Individuals.ActivityTier / TemperatureNarrative (banded from the row's own value)",
    ),
    dict(
        id="ik-cross-substrate-conformance",
        title="Cross-substrate conformance",
        maturity="Live",
        evidence="OWL-RL closure == Postgres closure: 14 asserted edges, 18 inferred pairs, 32 total - identical sets, verified by owl/reason.py",
        description=(
            "The same declared model is projected to a second substrate (OWL) and an independent reasoner "
            "closes it on its own; agreement with the Postgres recursive view is the conformance receipt. "
            "rulebook-to-owl emits the transition relation as an owl:TransitiveProperty, owlrl deduces every "
            "reachable pair, and owl/reason.py diffs that set against vw_state_transition_rules_closure - "
            "they match exactly (32 reachability pairs, incl. the inferred long-range intake -> actionable at "
            "hop 5). A recursive SQL view and an OWL-RL deductive closure compute the same reachability from "
            "one model: the reasoning is substrate-independent, not SQL-specific."
        ),
        example="owl/reason.py: owlrl closure of `progression` (TransitiveProperty) == vw_state_transition_rules_closure",
    ),
]


# Build the table object and insert it right after Features (with the other
# reasoning tables), preserving any prior Assigned... values if re-run.
ik = OD()
ik['Description'] = (
    "Families of derivation the platform performs - the KINDS of inference in the DAG (lookups, "
    "aggregations, higher-order gates, state machines, transitive closure, predicate-gated narrative, "
    "cross-substrate conformance), independent of any one feature. Lets PLATFORM_FEATURES.md open with a "
    "short overview of the reasoning machinery before the per-feature catalog. Grounded in field types "
    "that actually exist in this rulebook; Maturity is Live unless an upstream step is still in flight."
)
ik['schema'] = [
    field('InferenceKindId', 'string', 'raw', "Stable identifier.", nullable=False),
    field('Title', 'string', 'raw', "Short name of the inference family.", nullable=False),
    field('Description', 'string', 'raw', "One-line factual description of the kind.", nullable=False),
    field('ExampleField', 'string', 'raw', "A concrete field in the model that realizes this kind.", nullable=False),
    field('EvidenceCount', 'string', 'raw', "How much of it exists (field counts / usage), for honest weight.", nullable=False),
    field('Maturity', 'string', 'raw', "Live | Partial (Partial = an upstream step is still in flight).", nullable=False),
    field('SortOrder', 'number', 'raw', "Display order in the overview.", nullable=False),
    field('Name', 'string', 'calculated', "Display label.", formula="={{Title}}"),
    field('RelativePath', 'string', 'calculated',
          "Path to this entry: /admin/inference-kinds/<id>.",
          formula="=\"/admin/inference-kinds/\" & {{InferenceKindId}}"),
]
data = []
for i, k in enumerate(KINDS):
    row = OD()
    row['InferenceKindId'] = k['id']
    row['Title'] = k['title']
    row['Description'] = k['description']
    row['ExampleField'] = k['example']
    row['EvidenceCount'] = k['evidence']
    row['Maturity'] = k['maturity']
    row['SortOrder'] = i + 1
    row['Name'] = k['title']
    row['RelativePath'] = "/admin/inference-kinds/" + k['id']
    data.append(row)
ik['data'] = data

# Insert after 'Features' in the top-level key order. Skip any pre-existing
# InferenceKinds key so a fresh build cleanly REPLACES it (no orphan keys carried
# over from an earlier run's snapshot round-trip).
new_rb = OD()
for key, val in rb.items():
    if key == 'InferenceKinds':
        continue  # replaced wholesale below, not copied
    new_rb[key] = val
    if key == 'Features':
        new_rb['InferenceKinds'] = ik
# If Features somehow absent, append at end.
if 'InferenceKinds' not in new_rb:
    new_rb['InferenceKinds'] = ik

with open(RB_PATH, 'w') as fh:
    json.dump(new_rb, fh, indent=2, ensure_ascii=False)
    fh.write("\n")

print(f"InferenceKinds table written: {len(data)} kinds.")
for k in KINDS:
    print(f"  - {k['title']}  [{k['maturity']}]")
