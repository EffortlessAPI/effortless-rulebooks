"""Evidence for loop-15: the five claims loops 6 to 14 left uncovered.

Each of these was left open rather than stretched, and each is closed here by modelling the
thing the claim is actually about, not by reclassifying the claim or by reusing an unrelated
field that happened to discriminate.
"""

EVIDENCE = {
    # ---- pkm-1 ------------------------------------------------------------------------------
    "pkm1-i06": [
        ("Field", "KnowledgeConsumerSystems.HoldsComputationallyEncodedProcedureKnowledge",
         "The claim is that content management, business process management and knowledge graphs shifted "
         "attention to representations of process knowledge that can be queried, validated and reasoned "
         "over computationally. The model holds one row per system that carries ACME's process knowledge, "
         "across exactly those technology classes -- a content-management vault, a BPM modelling tool, two "
         "knowledge-graph platforms, a semantic repository, a field app and two AI platforms -- and this "
         "field applies the claim's own three tests to each of them. It reads TRUE only for the "
         "knowledge-graph platform, which can do all three, and FALSE for the document vault and the field "
         "app, which hold the same procedure knowledge in a form no machine can interrogate. Its "
         "counterpart StoresProcedureKnowledgeWithoutComputationalAccess names the four systems still on "
         "the wrong side of that shift, so the illustration is not just depicted, it is measured."),
    ],
    "pkm1-s09": [
        ("KnowledgeMethod", "EnterpriseArchitecturePractice",
         "The claim is that enterprise architecture is a precedent for process-knowledge methodology. "
         "EnterpriseArchitecturePractice is recorded as a knowledge method in the Governance family, and it "
         "is applied by a seeded MethodApplications row that names what was taken from the precedent: the "
         "register is built as an EA-style repository rather than a document set, with procedures, roles, "
         "functions, tools and resources as separate governed entities, and a change request against any "
         "one of them assessed for impact on the others before approval. The precedent is therefore "
         "recorded as a method this model follows, with the specific practice it borrowed, rather than as "
         "a reference in prose."),
    ],

    # ---- pkm-4 ------------------------------------------------------------------------------
    "pkm4-c12": [
        ("Table", "AbundantKnowledgeGaps",
         "The claim names three kinds of knowledge an abundance of information leaves wanting. The table "
         "holds exactly three rows, one per kind -- human judgment, tacit knowledge nobody has uncovered "
         "yet, and knowledge curated to a user and a use case -- each saying why more information does not "
         "supply it, and each pointing at the table in this model that represents it: ExpertCognitions, "
         "KnowledgeFragments and ProcedureLensViews respectively. RepresentingTableRowCount reads the "
         "measured row count of the table each one points at -- three, twelve and fifteen -- so a kind "
         "this model only named, and held nothing of, would read zero rather than pass quietly. All three "
         "are covered by one row each, which is why one evidence row proves the whole claim."),
    ],
    "pkm4-i07": [
        ("Field", "OntologySupportProgrammes.IsEndedWithNoStewardNamed",
         "The claim illustrates EU projects that support ontology development in industry. The model holds "
         "them as first-class rows with funder, grant reference, coordinator, dates and the public CORDIS "
         "record: PERKS (Horizon Europe 101120323, Cefriel, 2023-10-01 to 2026-09-30), the programme that "
         "produced the vocabulary this entire register is aligned to, and OntoCommons (Horizon 2020 "
         "958371, 2020-11-01 to 2023-10-31), evaluated but not adopted from. This field applies the "
         "governance consequence of that funding model: it reads TRUE on OntoCommons, which closed before "
         "the modelled instant with nobody here named to maintain what came from it, and FALSE on PERKS, "
         "which is still running. Its counterpart IsEndingSoonWithNoStewardNamed reads TRUE on PERKS, "
         "whose grant ends 73 days after the modelled instant. The model therefore does not merely name "
         "the projects -- it tracks what becomes of industrial ontology work when its funding stops."),
    ],
    "pkm4-s13": [
        ("OntologyProfile", "erb-pko-extension-1.0.0",
         "The claim is the LOT methodology's publication step: making terms available following Linked "
         "Data principles -- unique HTTP URIs and semantic links. Every concept this model needs that PKO "
         "2.0.0 does not define is minted in this profile, which carries the extension mappings for all of "
         "them. The namespace is the permanent identifier https://w3id.org/effortless/pko-extension#, "
         "resolving to a published vocabulary document that serves RDF under content negotiation and links "
         "every term to the PKO, P-Plan, PROV-O, DCAT and SKOS terms it sits beside -- the same way PKO "
         "publishes its own terms at https://w3id.org/pko. That this is true is not asserted here: "
         "PublishesFollowingLinkedDataPrinciples on this profile is computed from an actual HTTP fetch of "
         "the namespace recorded by tools/check_namespace_resolution.py, so if the identifier stopped "
         "resolving, or stopped serving RDF, the column would say so."),
    ],
}
