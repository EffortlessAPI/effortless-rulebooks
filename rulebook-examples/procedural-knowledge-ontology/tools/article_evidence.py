"""Evidence that the model covers each article claim, with the justification a skeptic audits.

EVIDENCE maps an ArticleClaimId to a list of (EvidenceKind, target key, justification).
Whether a row counts is decided by ClaimEvidence.IsValid in the rulebook, from Postgres
measurements. Listing a row here asserts nothing on its own.

Every claim is atomic: one valid evidence row must prove the whole claim. A claim that
bundles several things is split in article_claims.py rather than covered piecemeal.
"""

# (KnowledgeMethodId, Label, MethodFamily, Summary, OriginReference)
METHODS = [
    ("Shadowing", "Shadowing and observation", "Elicitation",
     "A knowledge engineer observes a practitioner doing the real work and asks about choices in the moment.",
     "Ethnographic observation; legitimate peripheral participation"),
    ("PractitionerInterview", "Structured practitioner interview", "Elicitation",
     "A structured interview that asks why choices are made and what happens when the procedure is not enough.",
     "Knowledge elicitation practice"),
    ("FacilitatedWorkshop", "Facilitated workshop", "Elicitation",
     "A session bringing together people who initiate, execute and receive a process to map it together.",
     "Collaborative process mapping"),
    ("CriticalIncidentTechnique", "Critical incident technique", "Elicitation",
     "Collects specific occasions when a process succeeded, failed or needed improvisation to surface judgment calls.",
     "Flanagan, 1954"),
    ("ThinkAloudProtocol", "Think-aloud protocol", "Elicitation",
     "The practitioner verbalizes reasoning while performing the task.",
     "Cognitive psychology"),
    ("RetrospectiveProtocol", "Retrospective protocol", "Elicitation",
     "The practitioner explains reasoning after the task, avoiding interference with the work.",
     "Cognitive psychology"),
    ("ConceptLaddering", "Concept laddering", "Elicitation",
     "Moves up by asking why something matters and down by asking how it is done, to reach goals and sub-steps.",
     "Knowledge acquisition practice"),
    ("RepertoryGrid", "Repertory grid", "Elicitation",
     "Compares situations pairwise to reveal the dimensions along which experts discriminate.",
     "Personal construct psychology"),
    ("DocumentAnalysis", "Document analysis", "Elicitation",
     "Extracts candidate knowledge from SOPs, training material, decision matrices and system documentation.",
     "Records analysis"),
    ("ProcessMining", "Process mining", "Analysis",
     "Reconstructs actual process flows, deviations and bottlenecks from system event logs.",
     "Process mining discipline"),
    ("SocialNetworkAnalysis", "Social network analysis", "Analysis",
     "Maps who consults whom to find knowledge brokers and concentration risk.",
     "Social network analysis"),
    ("ValueStreamAnalysis", "Process mapping and value stream analysis", "Analysis",
     "Maps a process end to end to expose hand-offs, waiting and bottlenecks.",
     "Lean process improvement"),
    ("ProcessKnowledgeTest", "Process knowledge test", "Analysis",
     "Tests practitioners on a procedure to find competency gaps.",
     "Training assessment practice"),
    ("LinkedOpenTerms", "Linked Open Terms methodology", "Modeling",
     "Lightweight, reuse-based ontology engineering: requirements, implementation, publication, maintenance.",
     "LOT methodology"),
    ("SemanticVersioning", "Semantic versioning", "Governance",
     "Major, minor and patch releases that state compatibility.",
     "semver.org"),
    ("SeciConversion", "SECI knowledge conversion", "Governance",
     "Socialization, externalization, combination and internalization of knowledge.",
     "Nonaka and Takeuchi"),
    ("GraphRetrieval", "Graph-grounded retrieval-augmented generation", "Encoding",
     "Retrieves structured segments from the knowledge graph to ground an AI answer.",
     "GraphRAG"),
    ("OwlReasoning", "OWL reasoning", "Encoding",
     "Materializes inferences with a reasoner before any AI consumes the context.",
     "OWL 2 RL"),
]

# Evidence lives with the theme that built it: tools/loops/evidence_<theme>.py. The ledger merges
# every such module; a claim id may appear in only one of them.
EVIDENCE: dict = {}
