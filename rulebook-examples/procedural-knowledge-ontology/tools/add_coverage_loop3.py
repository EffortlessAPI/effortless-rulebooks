#!/usr/bin/env python3
"""Loop 3: close the three gaps found when comparing this rulebook against the
four "Intentional Arrangement" source articles (process mining, controlled
vocabulary/taxonomy, and the informal knowledge-broker network).

Idempotent: safe to re-run. Skips any top-level table that already exists.

Writes with indent=1, ensure_ascii=False to match this file's on-disk format
(1-space indent; see project CLAUDE.md "Concurrent writes to the rulebook").
"""
from __future__ import annotations

import json
from collections import OrderedDict
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
EXT = "urn:effortless:pko-extension#"


def load():
    with RB.open() as fh:
        return json.load(fh, object_pairs_hook=OrderedDict)


def dump(rb):
    with RB.open("w") as fh:
        json.dump(rb, fh, indent=1, ensure_ascii=False)
        fh.write("\n")


def raw(name, datatype, desc, nullable=True):
    return OrderedDict([("name", name), ("datatype", datatype), ("type", "raw"),
                         ("nullable", nullable), ("Description", desc)])


def calc(name, datatype, desc, formula):
    return OrderedDict([("name", name), ("datatype", datatype), ("type", "calculated"),
                         ("nullable", True), ("Description", desc), ("formula", formula)])


def agg(name, datatype, desc, formula):
    return OrderedDict([("name", name), ("datatype", datatype), ("type", "aggregation"),
                         ("nullable", True), ("Description", desc), ("formula", formula)])


def lookup(name, datatype, desc, formula):
    return OrderedDict([("name", name), ("datatype", datatype), ("type", "lookup"),
                         ("nullable", True), ("Description", desc), ("formula", formula)])


def rel(name, target, desc, nullable=True):
    return OrderedDict([("name", name), ("datatype", "string"), ("type", "relationship"),
                         ("nullable", nullable), ("Description", desc), ("RelatedTo", target)])


def main() -> int:
    rb = load()
    if "ProcessMiningRuns" in rb:
        print("already applied (ProcessMiningRuns exists) — nothing to do")
        return 0

    # ---- 1. ProcessMiningRuns (process-steward: does the live system agree
    # with what we documented?) ------------------------------------------
    rb["ProcessMiningRuns"] = OrderedDict([
        ("Description", "One conformance-checking run of a mined event log against a "
                         "documented procedure version — a third kind of evidence, "
                         "distinct from an elicitation session (someone's account) or a "
                         "knowledge fragment (a claim): what the system of record actually "
                         "did, discovered by process mining rather than told to us."),
        ("important", True),
        ("schema", [
            raw("ProcessMiningRunId", "string", "Stored logical identifier for one ProcessMiningRuns row.", nullable=False),
            calc("Name", "string", "Human-readable calculated display alias for the ProcessMiningRuns row.",
                 '={{EventLogSource}} & " / " & {{MinedAt}}'),
            rel("ProcedureVersion", "ProcedureVersions", "Procedure version this mined event log is checked against."),
            raw("EventLogSource", "string", "System or log the event log was extracted from (e.g. an ERP audit log)."),
            raw("MinedAt", "datetime", "When this mining/conformance-checking run was performed."),
            raw("DiscoveredVariantCount", "integer", "Distinct process variants discovered in the mined event log."),
            raw("ConformingVariantCount", "integer", "How many of the discovered variants exactly match a documented path through StepTransitions."),
            raw("DeviationDescription", "string", "Narrative of the most significant discovered deviation, if any."),
            rel("EvaluationContext", "EvaluationContexts", "The evaluation context this row's time-dependent witnesses are judged under."),
            lookup("AsOfInstant", "datetime", "The evaluation instant this row's time-dependent witnesses are judged under.",
                   "=INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0))"),
            calc("ConformanceRate", "number", "Share of discovered variants that conform to the documented procedure.",
                 "=IF({{DiscoveredVariantCount}} = 0, 0, {{ConformingVariantCount}} / {{DiscoveredVariantCount}})"),
            calc("IsConformant", "boolean", "TRUE when at least 80% of what actually happened matches what was documented.",
                 "={{ConformanceRate}} >= 0.8"),
            calc("HasMajorDriftFromDocumentation", "boolean", "TRUE when less than half of what actually happened matches what was documented.",
                 "={{ConformanceRate}} < 0.5"),
            calc("DaysSinceMined", "integer", "Days elapsed since this event log was mined.",
                 '=DATETIME_DIFF({{AsOfInstant}}, {{MinedAt}}, "days")'),
            calc("IsStaleMiningEvidence", "boolean", "TRUE when this mining evidence is more than 180 days old.",
                 "={{DaysSinceMined}} > 180"),
            lookup("ProcedureVersionIsLive", "boolean", "Whether the procedure version this run was checked against is currently live.",
                   "=INDEX(ProcedureVersions!{{IsLive}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0))"),
            calc("IsDriftOnLiveVersion", "boolean", "TRUE when a major, real, mined deviation exists against a procedure version people are actually executing right now.",
                 "=AND({{HasMajorDriftFromDocumentation}}, {{ProcedureVersionIsLive}})"),
            calc("DriftedMiningRunKey", "string", "Composite-key echo: this run's procedure version when the run drifted on a live version, else blank.",
                 '=IF({{IsDriftOnLiveVersion}}, {{ProcedureVersion}}, "")'),
            raw("SemanticTypeIri", "string", "Extension class IRI."),
        ]),
        ("data", [
            OrderedDict([
                ("ProcessMiningRunId", "pmr-close-v11-q3"), ("ProcedureVersion", "close-v1.1.0"),
                ("EventLogSource", "ERP General Ledger Audit Log"), ("MinedAt", "2026-07-15T09:00:00-05:00"),
                ("DiscoveredVariantCount", 6), ("ConformingVariantCount", 5),
                ("DeviationDescription", "One variant closes a day early with no recorded controller approval; otherwise matches the documented path."),
                ("EvaluationContext", "eval-current"), ("SemanticTypeIri", f"{EXT}ProcessMiningRun"),
            ]),
            OrderedDict([
                ("ProcessMiningRunId", "pmr-close-v11-cutoff-bypass"), ("ProcedureVersion", "close-v1.1.0"),
                ("EventLogSource", "ERP General Ledger Audit Log"), ("MinedAt", "2026-07-17T09:00:00-05:00"),
                ("DiscoveredVariantCount", 8), ("ConformingVariantCount", 3),
                ("DeviationDescription", "Ledger entries posted after the documented cutoff step with no recorded reopening approval — a path StepTransitions never modeled."),
                ("EvaluationContext", "eval-current"), ("SemanticTypeIri", f"{EXT}ProcessMiningRun"),
            ]),
            OrderedDict([
                ("ProcessMiningRunId", "pmr-policy-v1-notify"), ("ProcedureVersion", "policy-v1.0.0"),
                ("EventLogSource", "Notification Pipeline Delivery Log"), ("MinedAt", "2026-01-10T09:00:00-05:00"),
                ("DiscoveredVariantCount", 4), ("ConformingVariantCount", 4),
                ("DeviationDescription", ""),
                ("EvaluationContext", "eval-current"), ("SemanticTypeIri", f"{EXT}ProcessMiningRun"),
            ]),
            OrderedDict([
                ("ProcessMiningRunId", "pmr-close-v10-archived"), ("ProcedureVersion", "close-v1.0.0"),
                ("EventLogSource", "ERP General Ledger Audit Log"), ("MinedAt", "2026-01-01T09:00:00-05:00"),
                ("DiscoveredVariantCount", 5), ("ConformingVariantCount", 1),
                ("DeviationDescription", "Heavily drifted, but this version was archived before anyone mined it — nobody executes this path anymore."),
                ("EvaluationContext", "eval-current"), ("SemanticTypeIri", f"{EXT}ProcessMiningRun"),
            ]),
        ]),
    ])

    pv_schema = rb["ProcedureVersions"]["schema"]
    idx = [f["name"] for f in pv_schema].index("SemanticTypeIri")
    pv_schema[idx:idx] = [
        agg("MiningRunCount", "number", "How many process-mining conformance runs exist for this procedure version.",
            "=COUNTIFS(ProcessMiningRuns!{{ProcedureVersion}}, {{ProcedureVersionId}})"),
        agg("DriftedMiningRunCount", "number", "How many mining runs found major, real drift while this version was live.",
            "=COUNTIFS(ProcessMiningRuns!{{DriftedMiningRunKey}}, {{ProcedureVersionId}})"),
        calc("HasUnresolvedMiningDrift", "boolean", "TRUE when mined operational evidence contradicts this live version's documented path.",
             "={{DriftedMiningRunCount}} > 0"),
    ]

    # ---- 2. Vocabularies / VocabularyTerms (knowledge-authority: are we
    # naming the same thing the same way everywhere?) ---------------------
    rb["Vocabularies"] = OrderedDict([
        ("Description", "A controlled vocabulary / SKOS-style concept scheme — one facet of "
                         "standardized terminology (e.g. a family of control categories) that "
                         "Requirements and other rows can point at instead of restating the "
                         "concept in free text each time."),
        ("important", True),
        ("schema", [
            raw("VocabularyId", "string", "Stored logical identifier for one Vocabularies row.", nullable=False),
            calc("Name", "string", "Human-readable calculated display alias for the Vocabularies row.", "={{Title}}"),
            raw("Title", "string", "Human-facing name of this controlled vocabulary."),
            raw("SchemeUri", "string", "SKOS ConceptScheme IRI for this vocabulary."),
            rel("GoverningRole", "Roles", "Role accountable for defining and maintaining terms in this vocabulary."),
            agg("TermCount", "number", "How many terms belong to this vocabulary.",
                "=COUNTIFS(VocabularyTerms!{{Vocabulary}}, {{VocabularyId}})"),
            agg("OrphanTermCount", "number", "How many terms in this vocabulary have never actually been used.",
                "=COUNTIFS(VocabularyTerms!{{OrphanTermVocabularyKey}}, {{VocabularyId}})"),
            calc("HasOrphanTerms", "boolean", "TRUE when this vocabulary has at least one defined-but-unused term.",
                 "={{OrphanTermCount}} > 0"),
            raw("SemanticTypeIri", "string", "Extension class IRI."),
        ]),
        ("data", [
            OrderedDict([("VocabularyId", "voc-close-controls"), ("Title", "Close Control Categories"),
                         ("SchemeUri", "urn:effortless:pko-extension:vocab#close-controls"),
                         ("GoverningRole", "knowledge-authority"), ("SemanticTypeIri", f"{EXT}Vocabulary")]),
            OrderedDict([("VocabularyId", "voc-policy-domains"), ("Title", "Policy Compliance Domains"),
                         ("SchemeUri", "urn:effortless:pko-extension:vocab#policy-domains"),
                         ("GoverningRole", "knowledge-authority"), ("SemanticTypeIri", f"{EXT}Vocabulary")]),
        ]),
    ])

    rb["VocabularyTerms"] = OrderedDict([
        ("Description", "One controlled, defined term (a SKOS Concept) within a Vocabulary. "
                         "Requirements point at a VocabularyTerm instead of free-texting the "
                         "same concept in a new Statement every time."),
        ("important", True),
        ("schema", [
            raw("VocabularyTermId", "string", "Stored logical identifier for one VocabularyTerms row.", nullable=False),
            calc("Name", "string", "Human-readable calculated display alias for the VocabularyTerms row.", "={{PrefLabel}}"),
            rel("Vocabulary", "Vocabularies", "Vocabulary this term belongs to."),
            raw("PrefLabel", "string", "SKOS preferred label — the controlled term itself."),
            raw("AltLabels", "string", "Comma-separated synonyms this term should also match."),
            raw("Definition", "string", "SKOS definition of what this term means."),
            agg("UsageCount", "number", "How many Requirements point at this exact term instead of free-texting the concept.",
                "=COUNTIFS(Requirements!{{ControlledTerm}}, {{VocabularyTermId}})"),
            calc("IsOrphanTerm", "boolean", "TRUE when this term has been defined but nothing in the model actually uses it yet.",
                 "={{UsageCount}} = 0"),
            calc("IsWidelyAdoptedTerm", "boolean", "TRUE when more than one Requirement shares this exact controlled term.",
                 "={{UsageCount}} >= 2"),
            calc("OrphanTermVocabularyKey", "string", "Composite-key echo: this term's vocabulary when the term is an orphan, else blank.",
                 '=IF({{IsOrphanTerm}}, {{Vocabulary}}, "")'),
            raw("SemanticTypeIri", "string", "Extension class IRI."),
        ]),
        ("data", [
            OrderedDict([("VocabularyTermId", "vt-cutoff"), ("Vocabulary", "voc-close-controls"),
                         ("PrefLabel", "Cutoff Control"), ("AltLabels", "posting cutoff, period cutoff"),
                         ("Definition", "A control preventing ledger entries after the period close boundary without an approved reopening."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-reconciliation"), ("Vocabulary", "voc-close-controls"),
                         ("PrefLabel", "Reconciliation Control"), ("AltLabels", "account reconciliation, balance control"),
                         ("Definition", "A control requiring a material account to reconcile or carry an approved exception."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-segregation"), ("Vocabulary", "voc-close-controls"),
                         ("PrefLabel", "Segregation of Duties"), ("AltLabels", "SoD, maker-checker"),
                         ("Definition", "A control requiring the preparer and the final approver of an action to be different agents."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-evidence-retention"), ("Vocabulary", "voc-close-controls"),
                         ("PrefLabel", "Evidence Retention"), ("AltLabels", "records retention, retention period"),
                         ("Definition", "A requirement that evidence of an action remain retrievable for a fixed retention window."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-human-approval-gate"), ("Vocabulary", "voc-close-controls"),
                         ("PrefLabel", "Human Approval Gate"), ("AltLabels", "human-in-the-loop, human confirmation"),
                         ("Definition", "A control requiring a human, not an automated agent, to confirm an approval."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-consent"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Consent Requirement"), ("AltLabels", "opt-in, active consent"),
                         ("Definition", "A requirement that a communication only be sent to a recipient with active, recorded consent."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-quiet-hours"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Quiet Hours Restriction"), ("AltLabels", "do-not-disturb window"),
                         ("Definition", "A requirement restricting delivery to a defined local-time window."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-optout"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Opt-Out Requirement"), ("AltLabels", "unsubscribe language"),
                         ("Definition", "A requirement that a message include approved opt-out language."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-accessibility"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Accessibility Requirement"), ("AltLabels", "readable content, alternate contact"),
                         ("Definition", "A requirement that content be readable, accessible, and offer an alternate contact method."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-legal-review"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Legal Review Gate"), ("AltLabels", "counsel approval"),
                         ("Definition", "A requirement that employment counsel approve a legal or privacy commitment before it ships."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-privacy-impact"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Data Privacy Impact"), ("AltLabels", "DPIA, privacy impact assessment"),
                         ("Definition", "An assessment of how a change affects personal data handling. Defined, not yet applied to any Requirement."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
            OrderedDict([("VocabularyTermId", "vt-third-party-attestation"), ("Vocabulary", "voc-policy-domains"),
                         ("PrefLabel", "Third-Party Attestation"), ("AltLabels", "vendor attestation"),
                         ("Definition", "A requirement that an external vendor attest to a control. Defined, not yet applied to any Requirement."),
                         ("SemanticTypeIri", f"{EXT}VocabularyTerm")]),
        ]),
    ])

    req_schema = rb["Requirements"]["schema"]
    idx = [f["name"] for f in req_schema].index("SemanticTypeIri")
    req_schema[idx:idx] = [
        rel("ControlledTerm", "VocabularyTerms", "Controlled vocabulary term this requirement's concept maps to, if any."),
        calc("UsesControlledVocabulary", "boolean", "TRUE when this requirement points at a controlled term instead of only free-texting the concept.",
             '={{ControlledTerm}} <> ""'),
    ]
    controlled_term_by_requirement = {
        "req-close-cutoff": "vt-cutoff",
        "req-close-balance": "vt-reconciliation",
        "req-close-separation": "vt-segregation",
        "req-close-evidence": "vt-evidence-retention",
        "req-close-human-approval": "vt-human-approval-gate",
        "req-policy-consent": "vt-consent",
        "req-policy-quiet-hours": "vt-quiet-hours",
        "req-policy-optout": "vt-optout",
        "req-policy-retention": "vt-evidence-retention",
        "req-policy-accessibility": "vt-accessibility",
        "req-policy-legal": "vt-legal-review",
    }
    for r in rb["Requirements"]["data"]:
        term = controlled_term_by_requirement.get(r.get("RequirementId"))
        if term:
            r["ControlledTerm"] = term

    # ---- 3. KnowledgeBrokerLinks (knowledge-authority: who do people
    # actually go to?) -----------------------------------------------------
    rb["Agents"]["data"].append(OrderedDict([
        ("AgentId", "jordan-park"), ("DisplayName", "Jordan Park"), ("AgentKind", "Human"),
        ("Organization", "acme-finance"), ("ContactAddress", ""), ("VersionOrEmploymentKey", ""),
        ("SemanticTypeIri", "prov:Agent"),
    ]))

    rb["KnowledgeBrokerLinks"] = OrderedDict([
        ("Description", "An informal expertise-network edge: one agent naming another as who "
                         "they actually go to for a topic, independent of any formal Role or "
                         "RoleAssignment. RoleAssignments and CommunitiesOfPractice record who is "
                         "SUPPOSED to know something; this records who people ACTUALLY rely on."),
        ("important", True),
        ("schema", [
            raw("KnowledgeBrokerLinkId", "string", "Stored logical identifier for one KnowledgeBrokerLinks row.", nullable=False),
            calc("Name", "string", "Human-readable calculated display alias for the KnowledgeBrokerLinks row.",
                 '={{Seeker}} & " -> " & {{Broker}}'),
            rel("Seeker", "Agents", "The agent who goes to someone else for this topic."),
            rel("Broker", "Agents", "The agent who is actually consulted, whether or not they hold a formal role for it."),
            rel("Topic", "VocabularyTerms", "The controlled topic this reliance is about."),
            raw("Frequency", "string", "Rarely, Sometimes, or Often."),
            raw("LastConsultedAt", "datetime", "When the seeker last actually consulted the broker on this topic."),
            rel("EvaluationContext", "EvaluationContexts", "The evaluation context this row's time-dependent witnesses are judged under."),
            lookup("AsOfInstant", "datetime", "The evaluation instant this row's time-dependent witnesses are judged under.",
                   "=INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0))"),
            calc("DaysSinceConsulted", "integer", "Days elapsed since the seeker last consulted the broker.",
                 '=DATETIME_DIFF({{AsOfInstant}}, {{LastConsultedAt}}, "days")'),
            calc("IsActiveReliance", "boolean", "TRUE when this is a live, ongoing informal dependency rather than a one-off or stale contact.",
                 '=AND(NOT({{Frequency}} = "Rarely"), {{DaysSinceConsulted}} <= 180)'),
            lookup("BrokerIsStillEngaged", "boolean", "Whether the broker being relied on still holds any current role at all.",
                   "=INDEX(Agents!{{IsStillEngaged}}, MATCH({{Broker}}, Agents!{{AgentId}}, 0))"),
            calc("IsAtRiskReliance", "boolean", "TRUE when someone is actively relying on a broker who has already left every role they held.",
                 "=AND({{IsActiveReliance}}, NOT({{BrokerIsStillEngaged}}))"),
            calc("ActiveRelianceBrokerKey", "string", "Composite-key echo: the broker this link names when the reliance is active, else blank.",
                 '=IF({{IsActiveReliance}}, {{Broker}}, "")'),
            calc("AtRiskBrokerKey", "string", "Composite-key echo: the broker this link names when the reliance is at risk, else blank.",
                 '=IF({{IsAtRiskReliance}}, {{Broker}}, "")'),
            raw("SemanticTypeIri", "string", "Extension class IRI."),
        ]),
        ("data", [
            OrderedDict([("KnowledgeBrokerLinkId", "kbl-maria-priya-recon"), ("Seeker", "maria-chen"),
                         ("Broker", "priya-raman"), ("Topic", "vt-reconciliation"), ("Frequency", "Often"),
                         ("LastConsultedAt", "2026-07-10T09:00:00-05:00"), ("EvaluationContext", "eval-current"),
                         ("SemanticTypeIri", f"{EXT}KnowledgeBrokerLink")]),
            OrderedDict([("KnowledgeBrokerLinkId", "kbl-devon-priya-recon"), ("Seeker", "devon-okafor"),
                         ("Broker", "priya-raman"), ("Topic", "vt-reconciliation"), ("Frequency", "Sometimes"),
                         ("LastConsultedAt", "2026-06-20T09:00:00-05:00"), ("EvaluationContext", "eval-current"),
                         ("SemanticTypeIri", f"{EXT}KnowledgeBrokerLink")]),
            OrderedDict([("KnowledgeBrokerLinkId", "kbl-elena-priya-sod"), ("Seeker", "elena-garcia"),
                         ("Broker", "priya-raman"), ("Topic", "vt-segregation"), ("Frequency", "Often"),
                         ("LastConsultedAt", "2026-07-05T09:00:00-05:00"), ("EvaluationContext", "eval-current"),
                         ("SemanticTypeIri", f"{EXT}KnowledgeBrokerLink")]),
            OrderedDict([("KnowledgeBrokerLinkId", "kbl-amina-jordan-sod"), ("Seeker", "amina-yusuf"),
                         ("Broker", "jordan-park"), ("Topic", "vt-segregation"), ("Frequency", "Often"),
                         ("LastConsultedAt", "2026-07-01T09:00:00-05:00"), ("EvaluationContext", "eval-current"),
                         ("SemanticTypeIri", f"{EXT}KnowledgeBrokerLink")]),
            OrderedDict([("KnowledgeBrokerLinkId", "kbl-noah-elena-quiet"), ("Seeker", "noah-williams"),
                         ("Broker", "elena-garcia"), ("Topic", "vt-quiet-hours"), ("Frequency", "Rarely"),
                         ("LastConsultedAt", "2025-01-01T09:00:00-05:00"), ("EvaluationContext", "eval-current"),
                         ("SemanticTypeIri", f"{EXT}KnowledgeBrokerLink")]),
        ]),
    ])

    ag_schema = rb["Agents"]["schema"]
    idx = [f["name"] for f in ag_schema].index("SemanticTypeIri")
    ag_schema[idx:idx] = [
        agg("TimesNamedAsBroker", "number", "How many active informal-reliance links name this agent as the one actually consulted.",
            "=COUNTIFS(KnowledgeBrokerLinks!{{ActiveRelianceBrokerKey}}, {{AgentId}})"),
        calc("IsRecognizedBroker", "boolean", "TRUE when at least three people actively rely on this agent as an informal knowledge broker.",
             "={{TimesNamedAsBroker}} >= 3"),
        agg("AtRiskRelianceCount", "number", "How many active informal-reliance links name this agent while this agent holds no current role.",
            "=COUNTIFS(KnowledgeBrokerLinks!{{AtRiskBrokerKey}}, {{AgentId}})"),
        calc("HasAtRiskKnowledgeReliance", "boolean", "TRUE when people are actively relying on this agent even though they have already left every role.",
             "={{AtRiskRelianceCount}} > 0"),
    ]

    # ---- 4. WitnessLoops / RoleQuestions provenance ----------------------
    rb["WitnessLoops"]["data"].append(OrderedDict([
        ("WitnessLoopId", "loop-03"), ("LoopNumber", 3),
        ("Title", "Loop 3: the three gaps the source articles named and the model didn't have"),
        ("Premise", "Comparing this rulebook against the four \"Intentional Arrangement\" "
                    "Process Knowledge Management articles surfaced three concepts the essays "
                    "argue for at length that had no table: process mining as an elicitation "
                    "channel, controlled vocabulary/taxonomy, and the informal knowledge-broker "
                    "network. Unlike loops 1-2, this loop is not a full 12-role sweep — it is "
                    "exactly these three named gaps, each owned by the role that would actually "
                    "ask the question."),
        ("SemanticTypeIri", f"{EXT}WitnessLoop"),
    ]))

    questions = [
        OrderedDict([
            ("RoleQuestionId", "q-steward-mining-drift"), ("AskingRole", "process-steward"),
            ("WitnessLoop", "loop-03"),
            ("QuestionText", "The event log from the ERP shows close-v1.1.0 running with 8 discovered "
                              "variants and only 3 conforming — has anyone recorded that the live "
                              "procedure and the live system actually disagree?"),
            ("WhyItMatters", "Every other witness in this model reasons from documents we wrote "
                              "(StepTransitions, Requirements) or claims someone told us "
                              "(KnowledgeFragments). None of them ever check what the system of "
                              "record actually did. Process mining is a third, independent kind of "
                              "evidence, and until now there was no table letting 'what we mined' "
                              "contradict 'what we modeled' — a live version could drift from its "
                              "own StepTransitions and nothing here would ever notice."),
            ("AnswerableBefore", False), ("WitnessedAnswer", ""),
            ("SemanticTypeIri", f"{EXT}RoleQuestion"),
        ]),
        OrderedDict([
            ("RoleQuestionId", "q-authority-controlled-vocabulary"), ("AskingRole", "knowledge-authority"),
            ("WitnessLoop", "loop-03"),
            ("QuestionText", "req-close-evidence and req-policy-retention both restate the same "
                              "7-year retention obligation in their own free-text Statement — is "
                              "there anything stopping one of them from quietly drifting from the "
                              "other?"),
            ("WhyItMatters", "Thirteen Requirements each free-text their own Statement with no "
                              "shared identifier between rows that mean the same thing, so a later "
                              "edit to one can never be checked against the other, and a search or "
                              "an AI grounding on 'evidence retention' cannot find both at once. "
                              "There was no controlled term either Requirement could point at — "
                              "exactly the organize-layer gap the source articles argue is where "
                              "most process knowledge quietly becomes unfindable."),
            ("AnswerableBefore", False), ("WitnessedAnswer", ""),
            ("SemanticTypeIri", f"{EXT}RoleQuestion"),
        ]),
        OrderedDict([
            ("RoleQuestionId", "q-authority-knowledge-broker"), ("AskingRole", "knowledge-authority"),
            ("WitnessLoop", "loop-03"),
            ("QuestionText", "RoleAssignments says who is FORMALLY responsible for reconciliation "
                              "and segregation-of-duties questions — but Maria, Devon, and Elena all "
                              "actually go to Priya Raman, and Amina goes to someone who no longer "
                              "holds any role at all. Does the model know that?"),
            ("WhyItMatters", "CommunitiesOfPractice and Mentorships already record the FORMAL "
                              "structures we teach people through. Nothing recorded the INFORMAL "
                              "one — who people actually walk over and ask — which is exactly the "
                              "knowledge-broker network the articles describe. A broker more relied "
                              "on than any Role holder is invisible risk; one who has already left "
                              "is a risk nobody is watching."),
            ("AnswerableBefore", False), ("WitnessedAnswer", ""),
            ("SemanticTypeIri", f"{EXT}RoleQuestion"),
        ]),
    ]
    rb["RoleQuestions"]["data"].extend(questions)

    # ---- 5. RulebookFields provenance stubs (reconcile_field_catalog.py
    # rebuilds every other column; only InventedForQuestion is authored) --
    provenance = {}
    for f in rb["ProcessMiningRuns"]["schema"]:
        provenance[f"ProcessMiningRuns.{f['name']}"] = "q-steward-mining-drift"
    for fname in ("MiningRunCount", "DriftedMiningRunCount", "HasUnresolvedMiningDrift"):
        provenance[f"ProcedureVersions.{fname}"] = "q-steward-mining-drift"
    for f in rb["Vocabularies"]["schema"]:
        provenance[f"Vocabularies.{f['name']}"] = "q-authority-controlled-vocabulary"
    for f in rb["VocabularyTerms"]["schema"]:
        provenance[f"VocabularyTerms.{f['name']}"] = "q-authority-controlled-vocabulary"
    for fname in ("ControlledTerm", "UsesControlledVocabulary"):
        provenance[f"Requirements.{fname}"] = "q-authority-controlled-vocabulary"
    for f in rb["KnowledgeBrokerLinks"]["schema"]:
        provenance[f"KnowledgeBrokerLinks.{f['name']}"] = "q-authority-knowledge-broker"
    for fname in ("TimesNamedAsBroker", "IsRecognizedBroker", "AtRiskRelianceCount", "HasAtRiskKnowledgeReliance"):
        provenance[f"Agents.{fname}"] = "q-authority-knowledge-broker"

    catalog = rb["RulebookFields"]["data"]
    have = {r["RulebookFieldId"] for r in catalog}
    for fid, qid in provenance.items():
        if fid in have:
            continue
        tbl, fname = fid.split(".", 1)
        catalog.append(OrderedDict([
            ("RulebookFieldId", fid), ("TargetTable", tbl), ("FieldName", fname),
            ("InventedForQuestion", qid),
        ]))

    # ---- 6. SemanticMappings — these are all pko-extensions --------------
    ext_rows = [
        ("map-process-mining-runs", "ProcessMiningRuns", "class", f"{EXT}ProcessMiningRun",
         "Process-mining conformance evidence — not part of PKO 2.0.0."),
        ("map-vocabularies", "Vocabularies", "class", f"{EXT}Vocabulary",
         "A controlled-vocabulary concept scheme — not part of PKO 2.0.0."),
        ("map-vocabulary-terms", "VocabularyTerms", "class", f"{EXT}VocabularyTerm",
         "A SKOS-style controlled term — not part of PKO 2.0.0."),
        ("map-knowledge-broker-links", "KnowledgeBrokerLinks", "class", f"{EXT}KnowledgeBrokerLink",
         "An informal expertise-network edge — not part of PKO 2.0.0."),
    ]
    sm = rb["SemanticMappings"]["data"]
    have_sm = {r["SemanticMappingId"] for r in sm}
    for sm_id, path, kind, iri, notes in ext_rows:
        if sm_id in have_sm:
            continue
        sm.append(OrderedDict([
            ("SemanticMappingId", sm_id), ("SourcePath", path), ("MappingKind", kind),
            ("TargetIri", iri), ("MappingRelation", "extension"),
            ("OntologyProfile", "erb-pko-extension-1.0.0"), ("Notes", notes),
        ]))

    dump(rb)
    print("applied loop-03: ProcessMiningRuns, Vocabularies, VocabularyTerms, "
          "KnowledgeBrokerLinks + patched ProcedureVersions/Requirements/Agents.")
    print("next: python3 tools/reconcile_field_catalog.py")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
