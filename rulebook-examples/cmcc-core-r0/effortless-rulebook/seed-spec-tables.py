#!/usr/bin/env python3
"""One-time builder: transcribe r0/spec.json into the cmcc-core-r0 rulebook.

Structural spec tables (EDBRelations, Commands, Rules, Transitions,
TransitionEffects, TransitionEvents, IntegrityConstraints, Observables,
SeedFacts, InputHistories, ReflectionConditions, Substrates) are seeded here
as a faithful, deterministic transcription of r0/spec.json -- not a
computation. (The constraints table is named IntegrityConstraints, not
Constraints -- see CLAUDE.md "Known transpiler gotcha".)

AnswerKeyResults and ReflectionResults are left with schema but empty data;
they are populated by conformance/ingest_conformance.py reading
conformance/matrix.json, which is the actual output of the independent
Python/SQLite pipeline.

This is a one-time authoring script, not part of the routine build/start
loop: re-run it by hand only if r0/spec.json's structure changes (new
relations/rules/transitions/etc.), then re-run `effortless build`. It
overwrites effortless-rulebook/effortless-rulebook.json in full, so commit
or stash any hand-edits to that file before re-running.

Run from anywhere: python3 effortless-rulebook/seed-spec-tables.py
"""
import json
import os

PROJECT_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SPEC_PATH = os.path.join(PROJECT_DIR, "r0", "spec.json")
RULEBOOK_PATH = os.path.join(PROJECT_DIR, "effortless-rulebook", "effortless-rulebook.json")

spec = json.load(open(SPEC_PATH))

def field(name, datatype, ftype, description, nullable=False, formula=None, related_to=None):
    f = {"name": name, "datatype": datatype, "type": ftype, "nullable": nullable, "Description": description}
    if formula is not None:
        f["formula"] = formula
    if related_to is not None:
        f["RelatedTo"] = related_to
    return f

def name_field(source_field="Name"):
    return field("Name", "string", "calculated", "Display alias.", formula="={{%s}}" % source_field)

# ---------------------------------------------------------------------------
# EDBRelations
# ---------------------------------------------------------------------------
edb_relations_schema = [
    field("RelationId", "string", "raw", "Stored identity: the EDB relation name from r0/spec.json's edb map (e.g. 'Warehouse')."),
    field("Fields", "string", "raw", "JSON array of the relation's field names, verbatim from spec.json (e.g. '[\"id\"]')."),
    field("FieldCount", "integer", "raw", "Number of fields, counted once at transcription time."),
    field("Description", "string", "raw", "What this EDB relation models in the warehouse-network domain."),
    name_field("RelationId"),
    field("SeedFacts", "string", "relationship", "h0 seed facts whose Relation is this EDB relation.", related_to="SeedFacts"),
    field("TransitionEffects", "string", "relationship", "Transition effects that assert/retract into this relation.", related_to="TransitionEffects"),
]
edb_relation_descriptions = {
    "Warehouse": "A node in the network (the hub, satellite depots, or ordinary warehouses).",
    "Route": "A directed shipping lane between two warehouses with a capacity.",
    "Shipment": "A shipment currently sitting at a warehouse on a given day.",
}
edb_relations_data = []
for rel_name, fields in spec["edb"].items():
    edb_relations_data.append({
        "RelationId": rel_name,
        "Fields": json.dumps(fields),
        "FieldCount": len(fields),
        "Description": edb_relation_descriptions.get(rel_name, ""),
    })

# ---------------------------------------------------------------------------
# Commands
# ---------------------------------------------------------------------------
commands_schema = [
    field("CommandId", "string", "raw", "Stored identity: the command name from r0/spec.json's commands map (e.g. 'AddRoute')."),
    field("Params", "string", "raw", "JSON array of the command's parameter names, verbatim from spec.json."),
    field("ParamCount", "integer", "raw", "Number of parameters, counted once at transcription time."),
    field("Description", "string", "raw", "What issuing this command means in the warehouse-network domain."),
    name_field("CommandId"),
    field("Transitions", "string", "relationship", "Transitions this command can fire.", related_to="Transitions"),
]
command_descriptions = {
    "AddRoute": "Open a new directed route s -> d with capacity c.",
    "CloseRoute": "Close (retract) an existing route s -> d.",
    "CorrectShipment": "Bitemporal correction: retract the recorded day for a shipment and re-assert a new day, both visible at knowledge time 'at'.",
    "Reroute": "Ask that a warehouse gain a fallback route, either via the hub or via the depot (two enabled transitions -> nondeterminism).",
    "Observe": "A no-op command that only emits an observation tag; asserts and retracts nothing.",
}
commands_data = []
for cmd_name, params in spec["commands"].items():
    commands_data.append({
        "CommandId": cmd_name,
        "Params": json.dumps(params),
        "ParamCount": len(params),
        "Description": command_descriptions.get(cmd_name, ""),
    })

# ---------------------------------------------------------------------------
# Rules
# ---------------------------------------------------------------------------
rules_schema = [
    field("RuleId", "string", "raw", "Stored identity: the rule's 'id' in r0/spec.json's rules array (e.g. 'reach_base')."),
    field("HeadPredicate", "string", "raw", "The rule's head predicate name (head[0] in spec.json), e.g. 'Reachable'."),
    field("Kind", "string", "raw", "'rule' for a body-clause rule, 'aggregation' for an agg-clause rule (spec.json's 'agg' key)."),
    field("HeadJson", "string", "raw", "The rule's head tuple, verbatim JSON from spec.json."),
    field("BodyJson", "string", "raw", "The rule's body clause list, verbatim JSON from spec.json (null for aggregation rules).", nullable=True),
    field("AggJson", "string", "raw", "The rule's agg clause, verbatim JSON from spec.json (null for body rules).", nullable=True),
    field("PaperSectionRef", "string", "raw", "CMCC-Core Representation Theorem section(s) this rule exercises."),
    field("Description", "string", "raw", "What the rule computes and, for reflection.py, which G_R node it constructs."),
    name_field("RuleId"),
    field("GuardedTransitions", "string", "relationship", "Transitions whose Guard is this rule.", related_to="Transitions"),
    field("TransitionEffects", "string", "relationship", "Transition effects whose FromRule is this rule.", related_to="TransitionEffects"),
    field("TransitionEvents", "string", "relationship", "Transition events whose FromRule is this rule.", related_to="TransitionEvents"),
    field("IntegrityConstraints", "string", "relationship", "Integrity constraints whose Rule is this rule.", related_to="IntegrityConstraints"),
]
rule_notes = {
    "reach_base": ("§4, §12", "Base case of positive recursion: a direct route makes y reachable from x."),
    "reach_step": ("§4, §12", "Recursive step of positive recursion: Reachable composes with one more Route hop."),
    "isolated": ("§7 (negation), §12", "Negation over a strictly lower stratum: a warehouse not reachable from the hub."),
    "fanout": ("§5 (aggregation), §12", "COUNT aggregation: out-degree of a warehouse; the empty group gives 0."),
    "maxcap": ("§5 (aggregation), §12", "MAX aggregation: largest outgoing route capacity; the empty group derives nothing."),
    "big": ("§12", "Comparison over an aggregation result: warehouses whose MaxCap is at least 50."),
    "double": ("§12", "A 'let' binding: DoubleCap re-derives a value via arithmetic, not just projection."),
    "late": ("§12", "Comparison over an EDB field: shipments recorded past day 10."),
    "known_at": ("§7 (bitemporal)", "Reads Hist_Shipment at a given knowledge time k, exercising the bitemporal snapshot rules tau generates."),
    "g_addroute": ("§8 (transitions)", "Guard for AddRoute: both endpoints exist and no route already connects them."),
    "e_addroute": ("§8 (transitions)", "Effect for AddRoute: the asserted Route tuple, valid from now to +inf."),
    "ev_addroute": ("§8 (transitions)", "Event for AddRoute: emits RouteAdded."),
    "g_closeroute": ("§8 (transitions)", "Guard for CloseRoute: the route currently exists."),
    "e_closeroute": ("§7, §8", "Effect for CloseRoute: retracts the live Hist_Route tuple for s -> d."),
    "g_correct": ("§8 (transitions)", "Guard for CorrectShipment: the shipment currently exists."),
    "e_correct_retract": ("§7 (bitemporal correction)", "Retract half of the correction: the live Hist_Shipment tuple for this shipment."),
    "e_correct_assert": ("§7 (bitemporal correction)", "Assert half of the correction: the new day, valid from the given knowledge time 'at'."),
    "ev_corrected": ("§8 (transitions)", "Event for CorrectShipment: emits ShipmentCorrected."),
    "g_reroute_hub": ("§8, §20 (nondeterminism)", "Guard for RerouteViaHub: enabled whenever x has no route to the hub."),
    "e_reroute_hub": ("§8, §20", "Effect for RerouteViaHub: asserts a route from x to the hub."),
    "g_reroute_depot": ("§8, §20 (nondeterminism)", "Guard for RerouteViaDepot: enabled whenever x has no route to the depot."),
    "e_reroute_depot": ("§8, §20", "Effect for RerouteViaDepot: asserts a route from x to the depot."),
    "g_observe": ("§8 (transitions)", "Guard for Observe: always enabled when the command is issued."),
    "c_negcap": ("§8 (integrity constraints)", "Constraint: a route with negative capacity, which rejects the transition that would create it."),
    "c_selfroute": ("§8 (integrity constraints)", "Constraint: a route from a warehouse to itself, which rejects the transition that would create it."),
}
rules_data = []
for r in spec["rules"]:
    rid = r["id"]
    kind = "aggregation" if "agg" in r else "rule"
    section, desc = rule_notes.get(rid, ("", ""))
    rules_data.append({
        "RuleId": rid,
        "HeadPredicate": r["head"][0],
        "Kind": kind,
        "HeadJson": json.dumps(r["head"]),
        "BodyJson": json.dumps(r["body"]) if "body" in r else None,
        "AggJson": json.dumps(r["agg"]) if "agg" in r else None,
        "PaperSectionRef": section,
        "Description": desc,
    })

def rule_id_for_predicate(pred):
    matches = [r["RuleId"] for r in rules_data if r["HeadPredicate"] == pred]
    if not matches:
        raise SystemExit(f"no rule found with head predicate {pred!r}")
    if len(matches) > 1:
        # Multiple rules can share a head predicate (recursive predicates like
        # Reachable). For guard/constraint resolution we want the rule that is
        # actually referenced as a single guard/constraint; if ambiguous here
        # it means the caller should pass the exact rule id instead.
        raise SystemExit(f"ambiguous head predicate {pred!r}: {matches}")
    return matches[0]

# ---------------------------------------------------------------------------
# Transitions / TransitionEffects / TransitionEvents
# ---------------------------------------------------------------------------
transitions_schema = [
    field("TransitionId", "string", "raw", "Stored identity: the transition's 'name' in r0/spec.json's transitions array (e.g. 'AddRoute')."),
    field("Command", "string", "relationship", "The command that can fire this transition.", related_to="Commands"),
    field("Guard", "string", "relationship", "The rule whose head predicate gates this transition.", related_to="Rules"),
    field("Description", "string", "raw", "What this transition does when its guard is satisfied."),
    name_field("TransitionId"),
    field("Effects", "string", "relationship", "This transition's ordered assert/retract effects.", related_to="TransitionEffects"),
    field("Events", "string", "relationship", "This transition's emitted events.", related_to="TransitionEvents"),
]
transition_descriptions = {
    "AddRoute": "Opens a new route; the sole enabled transition for command AddRoute.",
    "CloseRoute": "Closes an existing route; the sole enabled transition for command CloseRoute.",
    "CorrectShipment": "Bitemporal correction of a shipment's recorded day: retracts the old fact and asserts the new one under one knowledge time.",
    "RerouteViaHub": "One of two transitions enabled by command Reroute: adds a fallback route via the hub.",
    "RerouteViaDepot": "The other transition enabled by command Reroute: adds a fallback route via the depot. Together with RerouteViaHub this is the spec's one genuinely nondeterministic command (§20).",
    "Observe": "A no-op transition: enabled whenever Observe is issued, asserts and retracts nothing.",
}

transitions_data = []
transition_effects_data = []
transition_events_data = []
for t in spec["transitions"]:
    tname = t["name"]
    transitions_data.append({
        "TransitionId": tname,
        "Command": t["command"],
        "Guard": rule_id_for_predicate(t["guard"]),
        "Description": transition_descriptions.get(tname, ""),
    })
    for i, eff in enumerate(t.get("effects", [])):
        transition_effects_data.append({
            "EffectId": f"{tname}#effect{i}",
            "Transition": tname,
            "Op": eff["op"],
            "Relation": eff["rel"],
            "FromRule": rule_id_for_predicate(eff["from"]),
        })
    for i, ev in enumerate(t.get("events", [])):
        transition_events_data.append({
            "EventId": f"{tname}#event{i}",
            "Transition": tname,
            "EventRelation": ev["rel"],
            "FromRule": rule_id_for_predicate(ev["from"]),
        })

transition_effects_schema = [
    field("EffectId", "string", "raw", "Stored identity: '<TransitionId>#effect<index>', ordered as in spec.json."),
    field("Transition", "string", "relationship", "The transition this effect belongs to.", related_to="Transitions"),
    field("Op", "string", "raw", "'assert' or 'retract'."),
    field("Relation", "string", "relationship", "The EDB relation this effect asserts into or retracts from.", related_to="EDBRelations"),
    field("FromRule", "string", "relationship", "The rule whose derived tuples this effect applies.", related_to="Rules"),
    name_field("EffectId"),
]
transition_events_schema = [
    field("EventId", "string", "raw", "Stored identity: '<TransitionId>#event<index>', ordered as in spec.json."),
    field("Transition", "string", "relationship", "The transition this event belongs to.", related_to="Transitions"),
    field("EventRelation", "string", "raw", "The event relation name emitted (e.g. 'RouteAdded'); not an EDB relation."),
    field("FromRule", "string", "relationship", "The rule whose derived tuples produce this event.", related_to="Rules"),
    name_field("EventId"),
]

# ---------------------------------------------------------------------------
# Constraints
# ---------------------------------------------------------------------------
constraints_schema = [
    field("ConstraintId", "string", "raw", "Stored identity: the constraint's head predicate name, verbatim from spec.json's constraints array (e.g. 'NegativeCap')."),
    field("Rule", "string", "relationship", "The rule that derives this constraint's violations.", related_to="Rules"),
    field("Description", "string", "raw", "What a nonempty derivation of this predicate means, and which transition it rejects."),
    name_field("ConstraintId"),
]
constraint_descriptions = {
    "NegativeCap": "Rejects any transition whose effect would leave a route with negative capacity.",
    "SelfRoute": "Rejects any transition whose effect would leave a route from a warehouse to itself.",
}
constraints_data = []
for pred in spec["constraints"]:
    constraints_data.append({
        "ConstraintId": pred,
        "Rule": rule_id_for_predicate(pred),
        "Description": constraint_descriptions.get(pred, ""),
    })

# ---------------------------------------------------------------------------
# Observables
# ---------------------------------------------------------------------------
observables_schema = [
    field("ObservableId", "string", "raw", "Stored identity: the observable's key in spec.json's observables map (e.g. 'reachable')."),
    field("ExposedPredicate", "string", "raw", "The predicate this observable exposes (e.g. 'Reachable')."),
    field("Description", "string", "raw", "Why this predicate is exposed as an observable in Obs_R0(c, i, h)."),
    name_field("ObservableId"),
]
observable_descriptions = {
    "reachable": "Positive recursive closure over Route; the paper's running example of Lemma 1's finite lattice.",
    "isolated": "Negation over Reachable: witnesses the stratum operator's monotonicity (§7).",
    "fanout": "COUNT aggregation over Route; witnesses the empty-group case (§5).",
    "maxcap": "MAX aggregation over Route; witnesses the empty-group case (§5).",
    "big": "Comparison over an aggregation result.",
    "double": "A 'let'-derived value, not a bare projection.",
    "shipment": "Raw EDB relation, exposed directly so bitemporal corrections are visible in the answer key.",
    "late": "Comparison over an EDB field.",
    "known_at": "Bitemporal snapshot read of Hist_Shipment at a given knowledge time (§7)."
}
observables_data = []
for key, pred in spec["observables"].items():
    observables_data.append({
        "ObservableId": key,
        "ExposedPredicate": pred,
        "Description": observable_descriptions.get(key, ""),
    })

# ---------------------------------------------------------------------------
# SeedFacts (h0)
# ---------------------------------------------------------------------------
seed_facts_schema = [
    field("SeedFactId", "string", "raw", "Stored identity: 'h0-<n>', where n is the fact's position in spec.json's h0 array."),
    field("N", "integer", "raw", "The fact's 'n' (insertion order) in spec.json's h0 array."),
    field("Op", "string", "raw", "'assert' (h0 is assert-only)."),
    field("Relation", "string", "relationship", "The EDB relation this seed fact populates.", related_to="EDBRelations"),
    field("TupleJson", "string", "raw", "The seeded tuple, verbatim JSON from spec.json."),
    field("K", "integer", "raw", "Knowledge time of the seed fact (0 for all of h0)."),
    name_field("SeedFactId"),
]
seed_facts_data = []
for h in spec["h0"]:
    seed_facts_data.append({
        "SeedFactId": f"h0-{h['n']}",
        "N": h["n"],
        "Op": h["op"],
        "Relation": h["rel"],
        "TupleJson": json.dumps(h["tuple"]),
        "K": h["k"],
    })

# ---------------------------------------------------------------------------
# InputHistories
# ---------------------------------------------------------------------------
def count_events(path):
    data = json.load(open(path))
    return len(data) if isinstance(data, list) else len(data.get("events", data))

input_histories_schema = [
    field("InputHistoryId", "string", "raw", "Stored identity: the input file's stem under r0/inputs/ (e.g. 'i1-routes')."),
    field("SourceFile", "string", "raw", "Path to the input history file, relative to the project root."),
    field("EventCount", "integer", "raw", "Number of command events in the input history, counted once at transcription time."),
    field("Description", "string", "raw", "What this input history is designed to exercise."),
    name_field("InputHistoryId"),
    field("AnswerKeyResults", "string", "relationship", "Per-substrate answer-key results computed against this input history.", related_to="AnswerKeyResults"),
]
input_history_descriptions = {
    "i1-routes": "AddRoute/CloseRoute/Reroute traffic over the warehouse network; exercises recursion, negation, aggregation and the nondeterministic Reroute command.",
    "i2-bitemporal": "Observes one shipment at three (valid time, knowledge time) pairs around a CorrectShipment command; exercises the bitemporal snapshot rules tau generates (§7).",
}
input_histories_data = []
for stem in ["i1-routes", "i2-bitemporal"]:
    p = os.path.join(PROJECT_DIR, "r0", "inputs", f"{stem}.json")
    input_histories_data.append({
        "InputHistoryId": stem,
        "SourceFile": f"r0/inputs/{stem}.json",
        "EventCount": count_events(p),
        "Description": input_history_descriptions[stem],
    })

# ---------------------------------------------------------------------------
# ReflectionConditions (paper §13, six conditions)
# ---------------------------------------------------------------------------
reflection_conditions_schema = [
    field("ConditionId", "string", "raw", "Stored identity, matching conformance/matrix.json's condition keys (e.g. '1_constructor_preservation')."),
    field("ConditionNumber", "integer", "raw", "The condition's number in §13 (1-6)."),
    field("Title", "string", "raw", "Short name of the condition."),
    field("PaperSectionRef", "string", "raw", "Paper section this condition is defined in."),
    field("Description", "string", "raw", "What the condition checks on the substrate artifact's call graph."),
    name_field("ConditionId"),
    field("ReflectionResults", "string", "relationship", "Per-substrate pass/fail/delegated outcomes for this condition.", related_to="ReflectionResults"),
]
reflection_conditions_data = [
    {"ConditionId": "1_constructor_preservation", "ConditionNumber": 1, "Title": "Constructor preservation",
     "PaperSectionRef": "§13", "Description": "Every G_R node has a function of its kind in the artifact."},
    {"ConditionId": "2_dependency_preservation", "ConditionNumber": 2, "Title": "Dependency preservation",
     "PaperSectionRef": "§13", "Description": "Every source edge in G_R is a call edge in the artifact."},
    {"ConditionId": "3_addressability", "ConditionNumber": 3, "Title": "Addressability",
     "PaperSectionRef": "§13", "Description": "Every G_R node has an image (a named function) in the artifact."},
    {"ConditionId": "4_local_adequacy", "ConditionNumber": 4, "Title": "Local adequacy",
     "PaperSectionRef": "§11, §13", "Description": "Delegated to the answer key by design (§13); this condition is always recorded as delegated, never pass/fail."},
    {"ConditionId": "5_injectivity", "ConditionNumber": 5, "Title": "Injectivity",
     "PaperSectionRef": "§13", "Description": "Distinct G_R nodes map to distinct functions."},
    {"ConditionId": "6_bounded_locality", "ConditionNumber": 6, "Title": "Bounded locality",
     "PaperSectionRef": "§13", "Description": "No public function that is not an image; no call between images that is not a source edge; no run-time name resolution."},
]

# ---------------------------------------------------------------------------
# Substrates
# ---------------------------------------------------------------------------
substrates_schema = [
    field("SubstrateId", "string", "raw", "Stored identity, matching conformance/matrix.json's substrate keys."),
    field("Description", "string", "raw", "What this substrate is and how it is generated."),
    field("PaperSectionRef", "string", "raw", "Paper section(s) this substrate demonstrates."),
    field("IsControl", "boolean", "raw", "True for the two substrates that are deliberately expected to fail reflection (tangled, interpreter)."),
    name_field("SubstrateId"),
    field("AnswerKeyResults", "string", "relationship", "This substrate's per-input-history answer-key results.", related_to="AnswerKeyResults"),
    field("ReflectionResults", "string", "relationship", "This substrate's per-condition reflection results.", related_to="ReflectionResults"),
    field("AnswerKeyResultCount", "integer", "aggregation", "Number of answer-key results recorded for this substrate.",
          formula="=COUNTIFS(AnswerKeyResults!{{Substrate}}, {{SubstrateId}})"),
    field("AnswerKeyFailCount", "integer", "aggregation", "Number of answer-key results recorded as a failure for this substrate.",
          formula='=COUNTIFS(AnswerKeyResults!{{Substrate}}, {{SubstrateId}}, AnswerKeyResults!{{Outcome}}, "fail")'),
    field("AnswerKeyOk", "boolean", "calculated", "True when every recorded answer-key result for this substrate passed (§11).",
          formula="=AND({{AnswerKeyResultCount}} > 0, {{AnswerKeyFailCount}} = 0)"),
    field("ReflectionResultCount", "integer", "aggregation", "Number of reflection results recorded for this substrate.",
          formula="=COUNTIFS(ReflectionResults!{{Substrate}}, {{SubstrateId}})"),
    field("ReflectionFailCount", "integer", "aggregation", "Number of reflection conditions recorded as a failure for this substrate.",
          formula='=COUNTIFS(ReflectionResults!{{Substrate}}, {{SubstrateId}}, ReflectionResults!{{Outcome}}, "fail")'),
    field("ReflectionOk", "boolean", "calculated", "True when no recorded reflection condition failed for this substrate (§13). Delegated conditions never count as a failure.",
          formula="=AND({{ReflectionResultCount}} > 0, {{ReflectionFailCount}} = 0)"),
    field("Conformant", "boolean", "calculated", "True only when this substrate passes both gates: the answer key and reflection.",
          formula="=AND({{AnswerKeyOk}}, {{ReflectionOk}})"),
]
substrates_data = [
    {"SubstrateId": "transparent", "IsControl": False, "PaperSectionRef": "§12-13",
     "Description": "transpile.py emits one function per G_R node: a rule body as explicit nested loops, a non-recursive predicate as the union of its rules, a recursive stratum as a c_mu_* fixed-point driver. runtime.py never reads the spec JSON."},
    {"SubstrateId": "tangled", "IsControl": True, "PaperSectionRef": "§23",
     "Description": "The transparent module with every predicate read inside a rule routed through _dispatch(name, S), which resolves the target by name at run time. Same computation, same answers; dependency preservation and bounded locality fail because dependencies are now strings resolved dynamically."},
    {"SubstrateId": "interpreter", "IsControl": True, "PaperSectionRef": "§6, §13",
     "Description": "The rulebook loaded as inert JSON and handed to one universal evaluator (the SQLite oracle, compiling at request time). Zero image functions; constructor preservation, dependency preservation and addressability all fail."},
]

# ---------------------------------------------------------------------------
# AnswerKeyResults / ReflectionResults -- schema only, no seed data.
# Populated at runtime by ingest_conformance.py from conformance/matrix.json.
# ---------------------------------------------------------------------------
answer_key_results_schema = [
    field("AnswerKeyResultId", "string", "raw", "Stored identity: '<SubstrateId>__<InputHistoryId>'."),
    field("Substrate", "string", "relationship", "The substrate this result was computed for.", related_to="Substrates"),
    field("InputHistory", "string", "relationship", "The input history this result was computed against.", related_to="InputHistories"),
    field("TracesProduced", "integer", "raw", "Number of distinct traces this substrate produced for this input history (witnessed by conformance/run.py)."),
    field("Permitted", "integer", "raw", "Size of the permitted (expected) trace set for this input history (witnessed by tools/gen_expected.py)."),
    field("Outcome", "string", "raw", "'pass' if every produced trace was inside the permitted set, else 'fail'. Witnessed by conformance/run.py; never recomputed here."),
    name_field("AnswerKeyResultId"),
]
reflection_results_schema = [
    field("ReflectionResultId", "string", "raw", "Stored identity: '<SubstrateId>__<ConditionId>'."),
    field("Substrate", "string", "relationship", "The substrate this reflection outcome was computed for.", related_to="Substrates"),
    field("ReflectionCondition", "string", "relationship", "The §13 condition this outcome is for.", related_to="ReflectionConditions"),
    field("Outcome", "string", "raw", "'pass', 'fail' or 'delegated' (condition 4 is always delegated to the answer key by design). Witnessed by tools/reflection_check.py; never recomputed here."),
    name_field("ReflectionResultId"),
]

# ---------------------------------------------------------------------------
# __meta__
# ---------------------------------------------------------------------------
meta_schema = [
    field("MetaKey", "string", "raw", "The metadata key. Unique within the table."),
    name_field("MetaKey"),
    field("ValueType", "string", "raw", "How to interpret the value columns: 'string', 'object' or 'array'."),
    field("StringValue", "string", "raw", "Plain string value. Populated when ValueType == 'string'; null otherwise.", nullable=True),
    field("JsonValue", "string", "raw", "JSON-encoded value. Populated when ValueType == 'object' or 'array'; null when ValueType == 'string'.", nullable=True),
]
meta_data = [
    {"MetaKey": "tagline", "ValueType": "string",
     "StringValue": "The CMCC-Core Representation Theorem, made runnable: two gates, three substrates, one divergence.",
     "JsonValue": None},
    {"MetaKey": "description_rich", "ValueType": "string",
     "StringValue": ("Runnable companion to the CMCC-Core Representation Theorem paper. An R0 warehouse-network "
                      "spec is checked two ways -- an answer key (adequacy, section 11) and reflection "
                      "(structural transparency, section 13) -- across three substrates. All three pass the "
                      "answer key; only the transparent substrate passes reflection. This rulebook models the "
                      "spec's own structure (relations, rules, transitions, constraints, observables, seed facts) "
                      "and the conformance matrix as first-class data; it does not re-implement R0's stratified "
                      "Datalog semantics, which stays the exclusive job of the reference evaluator and the "
                      "SQLite oracle."),
     "JsonValue": None},
    {"MetaKey": "cmcc_summary", "ValueType": "string",
     "StringValue": ("Direction of truth: r0/spec.json is transcribed into EDBRelations/Commands/Rules/"
                      "Transitions/.../SeedFacts as static structural data (a mirror, not a computation). "
                      "conformance/matrix.json -- produced fresh by conformance/run.py from the reference "
                      "evaluator and the independent SQLite oracle -- is ingested into AnswerKeyResults and "
                      "ReflectionResults. Substrates.AnswerKeyOk / ReflectionOk / Conformant are the one "
                      "legitimate rulebook computation here: an AND/COUNT rollup of those already-witnessed "
                      "rows, never a re-derivation of R0 semantics itself."),
     "JsonValue": None},
    {"MetaKey": "source_paper", "ValueType": "string",
     "StringValue": "CMCC-Core Representation Theorem (section references throughout this rulebook are to that paper).",
     "JsonValue": None},
]

# ---------------------------------------------------------------------------
# Assemble
# ---------------------------------------------------------------------------
def table(description, schema, data):
    return {"Description": description, "schema": schema, "data": data}

rulebook = {
    "$schema": "https://effortlessapi.com/schemas/effortless-rulebook.schema.json",
    "Name": "cmcc-core-r0",
    "Description": ("The runnable companion to the CMCC-Core Representation Theorem, retrofitted into ERB "
                     "shape: the R0 warehouse-network spec's own structure, its conformance matrix (answer key "
                     "x reflection), and the resulting per-substrate roll-ups, all as rulebook data."),
    "EDBRelations": table(
        "The R0 spec's extensional database relations (r0/spec.json's 'edb' map): the fixed vocabulary every rule and command is built from.",
        edb_relations_schema, edb_relations_data),
    "Commands": table(
        "The R0 spec's commands (r0/spec.json's 'commands' map): the only way an input history can act on the state.",
        commands_schema, commands_data),
    "Rules": table(
        "Every stratified Datalog-with-negation-and-aggregation rule in the R0 spec (r0/spec.json's 'rules' array), including guard/effect/event rules for transitions and the two integrity-constraint rules.",
        rules_schema, rules_data),
    "Transitions": table(
        "The R0 spec's transitions (r0/spec.json's 'transitions' array): command + guard pairs that may fire, each with its effects and events.",
        transitions_schema, transitions_data),
    "TransitionEffects": table(
        "One row per (transition, effect) pair: an ordered assert/retract of a rule's derived tuples into an EDB relation.",
        transition_effects_schema, transition_effects_data),
    "TransitionEvents": table(
        "One row per (transition, event) pair: an event relation emitted when the transition fires.",
        transition_events_schema, transition_events_data),
    "IntegrityConstraints": table(
        "The R0 spec's integrity constraints (r0/spec.json's 'constraints' array): predicates whose nonempty derivation rejects a transition. "
        "Named IntegrityConstraints, not Constraints -- rulebook-to-rulespeak v2026.09.17.2011 treats a table literally named "
        "'Constraints' as a reserved word and fails the whole build with \"Entity '' does not resolve to any table.\"",
        constraints_schema, constraints_data),
    "Observables": table(
        "The R0 spec's observables (r0/spec.json's 'observables' map): the predicates exposed in Obs_R0(c, i, h), section 9.",
        observables_schema, observables_data),
    "SeedFacts": table(
        "The R0 spec's initial history h0 (r0/spec.json's 'h0' array): the seed facts every input history starts from.",
        seed_facts_schema, seed_facts_data),
    "InputHistories": table(
        "The two input histories this example runs (r0/inputs/*.json): what each is designed to exercise.",
        input_histories_schema, input_histories_data),
    "ReflectionConditions": table(
        "The six conditions of constructor reflection, section 13 of the paper.",
        reflection_conditions_schema, reflection_conditions_data),
    "Substrates": table(
        "The three substrates checked against the R0 spec: transparent (the real representation) and two controls, tangled and interpreter, that are expected to fail reflection on purpose.",
        substrates_schema, substrates_data),
    "AnswerKeyResults": table(
        "Per (substrate, input history) answer-key outcomes, ingested from a fresh run of conformance/run.py (conformance/matrix.json). Witnessed data, never recomputed here.",
        answer_key_results_schema, []),
    "ReflectionResults": table(
        "Per (substrate, reflection condition) outcomes, ingested from a fresh run of conformance/run.py (conformance/matrix.json). Witnessed data, never recomputed here.",
        reflection_results_schema, []),
    "__meta__": table(
        "Project-level metadata as typed rows (MetaKey, ValueType, StringValue, JsonValue). The only home for metadata in this rulebook.",
        meta_schema, meta_data),
}

os.makedirs(os.path.dirname(RULEBOOK_PATH), exist_ok=True)
with open(RULEBOOK_PATH, "w", encoding="utf-8") as f:
    json.dump(rulebook, f, indent=1, ensure_ascii=False)
    f.write("\n")

print("wrote", RULEBOOK_PATH)
print("tables:", [k for k in rulebook if k not in ("$schema", "Name", "Description")])
for k, v in rulebook.items():
    if isinstance(v, dict) and "schema" in v:
        print(f"  {k}: {len(v['schema'])} fields, {len(v.get('data', []))} rows")
