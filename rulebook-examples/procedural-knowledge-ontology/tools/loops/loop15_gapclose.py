"""Loop 15 — the five claims loops 6 to 14 left honestly uncovered.

Premise: after loop 14 the model could describe how ACME's own procedural knowledge is
collected, organized, encoded and governed, but it could say nothing about the machinery
*around* that knowledge — which systems hold it in a form a machine can actually work with,
which funded programmes stand behind the ontologies it reuses, whether its own published
terms resolve, and which kinds of knowledge an abundance of information still leaves
wanting. Those four questions only became askable once the model had consumer systems,
ontology profiles and a field catalog to point at.

Closes pkm1-i06, pkm1-s09, pkm4-c12, pkm4-i07 and pkm4-s13.

External facts seeded here are cited to their public sources in the row's own columns
(CORDIS grant references); no real company appears as an actor in the scenario data.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import EXT, agg, calc, idx, lookup, raw, rel  # noqa: E402

LOOP = {
    "WitnessLoopId": "loop-15",
    "LoopNumber": 15,
    "Title": "The machinery around the knowledge: encoding technologies, funding programmes, published terms",
    "Premise": (
        "Loops 6 to 14 modelled ACME's procedural knowledge itself. Five article claims had no honest "
        "witness afterwards, because each is about the machinery around that knowledge rather than the "
        "knowledge: whether the systems holding it can query, validate and reason over it; whether the "
        "research programmes behind the ontologies it reuses are still running; whether its own extension "
        "terms are published so anyone can look them up; and which kinds of knowledge an abundance of "
        "information leaves wanting. Each became askable only once consumer systems, ontology profiles and "
        "the field catalog existed to point at."
    ),
}

TABLES = [
    (
        "AbundantKnowledgeGaps",
        "The kinds of knowledge that an abundance of information does not supply, each pointing at the "
        "table in this model that represents it. One row per kind named in the source material's "
        "'paradox of abundant knowledge'.",
        "knowledge",
        [
            raw("AbundantKnowledgeGapId", "string", "Readable key for the kind of knowledge.", nullable=False),
            calc("Name", "string", "Display name.", "={{Label}}"),
            raw("Label", "string", "Short name of this kind of knowledge."),
            raw("GapKind", "string", "HumanJudgment | UndiscoveredTacitKnowledge | CuratedToUserAndUseCase."),
            raw("Description", "string", "What this kind of knowledge is."),
            raw("WhyAbundanceDoesNotSupplyIt", "string",
                "Why having more information available does not produce this kind of knowledge."),
            rel("RepresentedByTable", "RulebookTables",
                "The table in this model that holds this kind of knowledge."),
            lookup("RepresentingTableRowCount", "integer",
                   "Rows measured in the representing table, from the field catalog. This is how much of "
                   "this kind of knowledge the model actually holds; a kind that is named but held nowhere "
                   "reads zero. Deliberately a count rather than a yes/no: all three kinds are represented "
                   "here, so a boolean would be all-true and would state nothing, while the counts differ "
                   "and say which kind is thinnest.",
                   idx("RulebookTables", "MeasuredRowCount", "RepresentedByTable")),
            raw("SemanticTypeIri", "string", "Semantic type."),
        ],
    ),
    (
        "OntologySupportProgrammes",
        "Funded research programmes that produced or supported the ontologies this model reuses. Recorded "
        "so the ontology authority can see what happens to a reused vocabulary when the programme behind "
        "it ends.",
        "governance",
        [
            raw("OntologySupportProgrammeId", "string", "Readable key for the programme.", nullable=False),
            calc("Name", "string", "Display name.", "={{Label}}"),
            raw("Label", "string", "Full programme title."),
            raw("Acronym", "string", "Programme acronym."),
            raw("Funder", "string", "Body that funded the programme."),
            raw("GrantReference", "string", "Grant agreement number, as published by the funder."),
            raw("ProgrammeIri", "string", "Public record of the programme."),
            raw("Coordinator", "string", "Organization coordinating the programme."),
            raw("StartedOn", "datetime", "Programme start date, as published by the funder."),
            raw("EndsOn", "datetime", "Programme end date, as published by the funder."),
            raw("IsIndustryFocused", "boolean",
                "Whether the programme's purpose is ontology development for industry."),
            raw("OurSuccessorSteward", "string",
                "Who this organization relies on to maintain the terms it took from this programme once "
                "the programme ends. Blank means nobody has been named."),
            raw("WhyRecorded", "string",
                "Why the ontology authority tracks this programme, whether or not anything has been "
                "adopted from it yet."),
            rel("EvaluationContext", "EvaluationContexts", "The instant this row is judged against."),
            lookup("AsOfInstant", "datetime", "The modelled instant.",
                   idx("EvaluationContexts", "AsOfInstant", "EvaluationContext")),
            agg("SupportedProfileCount", "integer",
                "Ontology profiles in this model that name this programme.",
                "=COUNTIFS(OntologyProfiles!{{SupportingProgramme}}, {{OntologySupportProgrammeId}})"),
            calc("HasEnded", "boolean", "Whether the programme has ended as of the modelled instant.",
                 '=AND({{EndsOn}} <> "", {{EndsOn}} < {{AsOfInstant}})'),
            calc("DaysUntilProgrammeEnds", "integer",
                 "Days from the modelled instant to the programme's end date.",
                 '=IF({{EndsOn}} = "", 0, DATETIME_DIFF({{EndsOn}}, {{AsOfInstant}}, "days"))'),
            calc("IsEndedWithNoStewardNamed", "boolean",
                 "The programme has ended and this organization has named nobody to maintain what it took "
                 "from it.",
                 '=AND({{HasEnded}}, {{OurSuccessorSteward}} = "")'),
            calc("IsEndingSoonWithNoStewardNamed", "boolean",
                 "The programme has not ended yet, ends within six months of the modelled instant, and "
                 "nobody here has been named to maintain what we took from it. This is the window in "
                 "which something can still be done about it.",
                 '=AND({{HasEnded}} = FALSE, {{EndsOn}} <> "", {{DaysUntilProgrammeEnds}} <= 180, '
                 '{{OurSuccessorSteward}} = "")'),
            raw("SemanticTypeIri", "string", "Semantic type."),
        ],
    ),
]

FIELDS = {
    # pkm1-i06 -- the technologies that shifted attention to ENCODED process knowledge are the
    # ones whose holdings can be queried, validated and reasoned over computationally.
    "KnowledgeConsumerSystems": [
        raw("IsComputationallyQueryable", "boolean",
            "Whether the procedure knowledge this system holds can be queried structurally, rather than "
            "only opened and read by a person."),
        raw("IsComputationallyValidatable", "boolean",
            "Whether the procedure knowledge this system holds can be checked automatically against rules."),
        calc("HoldsComputationallyEncodedProcedureKnowledge", "boolean",
             "The system holds procedure knowledge in a form that can be queried, validated and reasoned "
             "over computationally -- the shift that encoded representations made possible.",
             "=AND({{HoldsProcedureKnowledge}}, {{IsComputationallyQueryable}}, "
             "{{IsComputationallyValidatable}}, {{HasReasoner}})"),
        calc("StoresProcedureKnowledgeWithoutComputationalAccess", "boolean",
             "The system holds procedure knowledge that no machine can query, validate or reason over: "
             "storage without encoding.",
             "=AND({{HoldsProcedureKnowledge}}, "
             "{{HoldsComputationallyEncodedProcedureKnowledge}} = FALSE)"),
    ],
    # pkm4-i07 and pkm4-s13 -- where a reused vocabulary came from, and whether ours can be looked up.
    "OntologyProfiles": [
        rel("SupportingProgramme", "OntologySupportProgrammes",
            "The funded programme that produced or supported this vocabulary."),
        raw("NamespaceCheckedAt", "datetime",
            "When the namespace was last fetched by tools/check_namespace_resolution.py."),
        raw("NamespaceHttpStatus", "integer", "HTTP status the namespace returned when last fetched."),
        raw("NamespaceServesRdf", "boolean",
            "Whether fetching the namespace with an RDF Accept header returned RDF."),
        calc("NamespaceIsHttp", "boolean",
             "Whether the namespace is an HTTP(S) URI, the first Linked Data requirement, rather than a "
             "urn: or other non-resolvable identifier.",
             '=OR(LEFT({{NamespaceIri}}, 7) = "http://", LEFT({{NamespaceIri}}, 8) = "https://")'),
        calc("NamespaceDereferences", "boolean",
             "The namespace is an HTTP URI and fetching it actually returned a document.",
             "=AND({{NamespaceIsHttp}}, {{NamespaceHttpStatus}} = 200)"),
        calc("PublishesFollowingLinkedDataPrinciples", "boolean",
             "The vocabulary is published the way Linked Data requires: an HTTP URI that resolves, serves "
             "RDF when asked for it, and carries links to other terms.",
             "=AND({{NamespaceDereferences}}, {{NamespaceServesRdf}}, {{MappingCount}} > 0)"),
    ],
}

QUESTIONS = [
    ("q15-knowledge-engineer-encoded-access", "knowledge-engineer",
     "Which of the systems that hold our procedure knowledge can a machine actually query, validate and "
     "reason over -- and which of them only store it?",
     "Documentation, content management and modelling tools all hold procedure knowledge, but only an "
     "encoded representation can be queried, validated and reasoned over. Knowing which of our systems "
     "cleared that bar tells me where knowledge is usable and where it is merely filed.",
     ["KnowledgeConsumerSystems.IsComputationallyQueryable",
      "KnowledgeConsumerSystems.IsComputationallyValidatable",
      "KnowledgeConsumerSystems.HoldsComputationallyEncodedProcedureKnowledge",
      "KnowledgeConsumerSystems.StoresProcedureKnowledgeWithoutComputationalAccess"]),

    ("q15-knowledge-authority-abundance-gaps", "knowledge-authority",
     "An abundance of information still leaves three things wanting -- human judgment, the tacit knowledge "
     "nobody has uncovered yet, and knowledge curated to a particular person and task. For each of them, "
     "does this model hold anything, or does it only name the problem?",
     "It is easy to build a system that is full of information and still cannot supply the three things "
     "people actually come to it for. I want each of the three named explicitly and pointed at the rows "
     "that answer it, so a gap shows up as an empty table rather than as an unexamined assumption.",
     ["AbundantKnowledgeGaps.Label", "AbundantKnowledgeGaps.GapKind",
      "AbundantKnowledgeGaps.Description", "AbundantKnowledgeGaps.WhyAbundanceDoesNotSupplyIt",
      "AbundantKnowledgeGaps.RepresentedByTable",
      "AbundantKnowledgeGaps.RepresentingTableRowCount"]),

    ("q15-ontology-authority-programme-support", "ontology-authority",
     "Which of the vocabularies we depend on came out of a funded research programme, has that programme "
     "ended, and has anyone here been named to maintain what we took from it?",
     "Ontologies for industry are largely produced by time-limited funded projects. When the funding ends "
     "the terms stay in our model and the maintainers go home. I need to see, before that happens, which "
     "of our dependencies are in that position and which of them nobody here has taken responsibility for.",
     ["OntologySupportProgrammes.Label", "OntologySupportProgrammes.Acronym",
      "OntologySupportProgrammes.Funder", "OntologySupportProgrammes.GrantReference",
      "OntologySupportProgrammes.ProgrammeIri", "OntologySupportProgrammes.Coordinator",
      "OntologySupportProgrammes.StartedOn", "OntologySupportProgrammes.EndsOn",
      "OntologySupportProgrammes.IsIndustryFocused", "OntologySupportProgrammes.OurSuccessorSteward",
      "OntologySupportProgrammes.WhyRecorded",
      "OntologySupportProgrammes.EvaluationContext", "OntologySupportProgrammes.AsOfInstant",
      "OntologySupportProgrammes.SupportedProfileCount", "OntologySupportProgrammes.HasEnded",
      "OntologySupportProgrammes.DaysUntilProgrammeEnds",
      "OntologySupportProgrammes.IsEndedWithNoStewardNamed",
      "OntologySupportProgrammes.IsEndingSoonWithNoStewardNamed",
      "OntologyProfiles.SupportingProgramme"]),

    ("q15-ontology-authority-terms-resolve", "ontology-authority",
     "When someone outside this organization meets one of our own terms in our data, can they look it up? "
     "Does the identifier resolve, and does it return RDF when a machine asks for it?",
     "We mint identifiers for every concept PKO does not define. If those identifiers cannot be "
     "dereferenced they are private strings wearing the costume of an IRI, and our data is not linked to "
     "anything. I want the answer measured by fetching them, not asserted in a document.",
     ["OntologyProfiles.NamespaceCheckedAt", "OntologyProfiles.NamespaceHttpStatus",
      "OntologyProfiles.NamespaceServesRdf", "OntologyProfiles.NamespaceIsHttp",
      "OntologyProfiles.NamespaceDereferences",
      "OntologyProfiles.PublishesFollowingLinkedDataPrinciples"]),
]

ROWS = {
    "KnowledgeMethods": [
        {
            "KnowledgeMethodId": "EnterpriseArchitecturePractice",
            "Label": "Enterprise architecture practice",
            "MethodFamily": "Governance",
            "Summary": (
                "Enterprise architecture's repository discipline, applied to process knowledge: business "
                "processes, the roles that perform them, the functions and tools that support them and the "
                "information they consume are modelled as separate governed entities in one repository, and "
                "a change to any of them is reviewed for its effect on the others."
            ),
            "OriginReference": "Enterprise architecture discipline",
            "ElicitationTradeoff": (
                "Gives a whole-organization view and a governed change path, at the cost of a heavier "
                "repository than a single procedure's authors need."
            ),
            "SemanticTypeIri": f"{EXT}KnowledgeMethod",
        },
    ],
    "MethodApplications": [
        {
            "MethodApplicationId": "ma15-ea-repository-structure",
            "KnowledgeMethod": "EnterpriseArchitecturePractice",
            "AppliedTo": (
                "The procedure register is structured as an enterprise-architecture repository rather than "
                "as a document set: procedures, the roles that execute them, the functions and tools they "
                "require and the resources they consume are separate governed entities, and a change "
                "request against any one of them is assessed for impact on the others before approval."
            ),
            "AppliedAt": "2026-02-16T10:00:00-05:00",
            "AppliedByAgent": "priya-raman",
            "SemanticTypeIri": f"{EXT}MethodApplication",
        },
    ],
    "AbundantKnowledgeGaps": [
        {
            "AbundantKnowledgeGapId": "human-judgment",
            "Label": "Human judgment",
            "GapKind": "HumanJudgment",
            "Description": (
                "The judgment a person supplies that no volume of recorded information contains: what an "
                "experienced worker decides when the readings are ambiguous, and the human feedback that "
                "corrects an automated system."
            ),
            "WhyAbundanceDoesNotSupplyIt": (
                "Judgment is exercised, not retrieved. More documents describing a step do not decide "
                "whether this particular accumulator has finished bleeding down."
            ),
            "RepresentedByTable": "ExpertCognitions",
            "SemanticTypeIri": f"{EXT}AbundantKnowledgeGap",
        },
        {
            "AbundantKnowledgeGapId": "undiscovered-tacit-knowledge",
            "Label": "Tacit knowledge nobody has uncovered yet",
            "GapKind": "UndiscoveredTacitKnowledge",
            "Description": (
                "Knowledge that is held and used but has never been written down, so it is absent from "
                "every repository until somebody deliberately elicits it."
            ),
            "WhyAbundanceDoesNotSupplyIt": (
                "It was never in the corpus. Searching more of what was written cannot return something "
                "that only exists in a practitioner's hands and ears."
            ),
            "RepresentedByTable": "KnowledgeFragments",
            "SemanticTypeIri": f"{EXT}AbundantKnowledgeGap",
        },
        {
            "AbundantKnowledgeGapId": "curated-to-user-and-use-case",
            "Label": "Knowledge curated to a user and a use case",
            "GapKind": "CuratedToUserAndUseCase",
            "Description": (
                "The same procedure shaped for who is asking and what they are doing: the granularity, form "
                "and detail an operator on the floor needs differ from what an auditor or a manager needs."
            ),
            "WhyAbundanceDoesNotSupplyIt": (
                "Abundance makes the curation problem worse, not better. Without a recorded lens, more "
                "material means more to sift at the moment of use."
            ),
            "RepresentedByTable": "ProcedureLensViews",
            "SemanticTypeIri": f"{EXT}AbundantKnowledgeGap",
        },
    ],
    "OntologySupportProgrammes": [
        {
            "OntologySupportProgrammeId": "perks",
            "Label": "Eliciting and Exploiting Procedural Knowledge in Industry 5.0",
            "Acronym": "PERKS",
            "Funder": "European Union, Horizon Europe",
            "GrantReference": "101120323",
            "ProgrammeIri": "https://cordis.europa.eu/project/id/101120323",
            "Coordinator": "Cefriel",
            "StartedOn": "2023-10-01T00:00:00+00:00",
            "EndsOn": "2026-09-30T00:00:00+00:00",
            "IsIndustryFocused": True,
            "OurSuccessorSteward": "",
            "WhyRecorded": (
                "The programme that produced the vocabulary this whole register is aligned to. Everything "
                "the register says about procedures and their executions is expressed in terms this "
                "programme defined."
            ),
            "EvaluationContext": "eval-current",
            "SemanticTypeIri": f"{EXT}OntologySupportProgramme",
        },
        {
            "OntologySupportProgrammeId": "ontocommons",
            "Label": "Ontology-driven data documentation for Industry Commons",
            "Acronym": "OntoCommons",
            "Funder": "European Union, Horizon 2020",
            "GrantReference": "958371",
            "ProgrammeIri": "https://cordis.europa.eu/project/id/958371",
            "Coordinator": "Technische Universitaet Wien",
            "StartedOn": "2020-11-01T00:00:00+00:00",
            "EndsOn": "2023-10-31T00:00:00+00:00",
            "IsIndustryFocused": True,
            "OurSuccessorSteward": "",
            "WhyRecorded": (
                "Evaluated by the ontology authority as a source of reusable industrial vocabulary. "
                "Nothing has been adopted from it, which is why its supported-profile count is zero; it is "
                "kept on the register because a programme that has already closed is the case this "
                "organization most needs to reason about before it adopts anything."
            ),
            "EvaluationContext": "eval-current",
            "SemanticTypeIri": f"{EXT}OntologySupportProgramme",
        },
    ],
    # pkm1-i06: which technology classes hold process knowledge a machine can work with.
    "KnowledgeConsumerSystems": [
        {"KnowledgeConsumerSystemId": "sys-docvault",
         "IsComputationallyQueryable": False, "IsComputationallyValidatable": False},
        {"KnowledgeConsumerSystemId": "sys-bpmn-modeler",
         "IsComputationallyQueryable": True, "IsComputationallyValidatable": True},
        {"KnowledgeConsumerSystemId": "sys-procedure-graph",
         "IsComputationallyQueryable": True, "IsComputationallyValidatable": True},
        {"KnowledgeConsumerSystemId": "sys-procedure-register",
         "IsComputationallyQueryable": True, "IsComputationallyValidatable": True},
        {"KnowledgeConsumerSystemId": "sys-maintenance-tablet",
         "IsComputationallyQueryable": False, "IsComputationallyValidatable": False},
        {"KnowledgeConsumerSystemId": "sys-quality-graph",
         "IsComputationallyQueryable": True, "IsComputationallyValidatable": True},
        {"KnowledgeConsumerSystemId": "sys-copilot-platform",
         "IsComputationallyQueryable": False, "IsComputationallyValidatable": False},
        {"KnowledgeConsumerSystemId": "sys-release-bot-platform",
         "IsComputationallyQueryable": False, "IsComputationallyValidatable": False},
    ],
    # pkm4-i07: the two vocabularies this model is aligned to came out of PERKS.
    "OntologyProfiles": [
        {"OntologyProfileId": "pko-core-2.0.0", "SupportingProgramme": "perks"},
        {"OntologyProfileId": "pko-industry-2.0.0", "SupportingProgramme": "perks"},
    ],
}

# No MAPPINGS. Both new tables are deliberate extensions: PKO defines no term for "the funded
# programme behind a vocabulary" or for "a kind of knowledge an abundance of information leaves
# wanting", and schema:Grant is a grant, not a programme. apply_loop_spec.py therefore records
# each as an `extension` mapping in the ERB-PKO extension profile, which is the honest label.
