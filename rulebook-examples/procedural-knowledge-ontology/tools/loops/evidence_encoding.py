"""Evidence for theme D (loop-09): encoding and use.

The model records AI behaviour as data -- answers, groundings, retrievals, prompts, tool calls,
benchmarks -- and judges those records against the shared procedure model. It does not run an AI.
Justifications say so where it matters.
"""

EVIDENCE = {
    # ---- Concepts --------------------------------------------------------------------------
    "pkm1-c37": [("Table", "EncodingLifecycleStages",
                  "One row per stage of the lifecycle that carries process knowledge to AI and reuse -- collect, store, document, codify, "
                  "encode, operationalize -- in order, with the activity at each stage; feedback is recorded against these rows.")],
    "pkm2-c84": [("Field", "RetrievalSegments.BoundaryKind",
                  "Records whether a retrieval segment is cut at an Activity or DecisionPoint boundary or is a CharacterWindow chunk cut by length; "
                  "the lockout segments are cut at steps and at the step-06 decision, the 2019 SOP chunk by character count.")],
    "pkm2-c85": [("Field", "RetrievalSegments.PositionInProcess",
                  "Holds each segment's position in the larger process, e.g. 'Step 06 of Lockout/Tagout 2.0.0: after 05 apply locks, before 07 maintenance'.")],
    "pkm2-c86": [("Field", "RetrievalSegments.ApplicabilityCondition",
                  "Holds when each segment applies, e.g. 'A reading above zero, a flicker, or a hiss' for the zero-energy decision segment.")],
    "pkm2-c87": [("Field", "RetrievalSegments.RelatedSegment",
                  "Links a segment to a related segment: the step-06 verification segment links to its decision segment, which links to the escalation step.")],
    "pkm2-c93": [("Table", "AiToolInvocations",
                  "Each row is a tool an AI agent invoked during a step execution, tied to the Function and StepExecution in the process graph, "
                  "so tool use and agentic behaviour are process knowledge in the graph rather than a log outside it.")],
    "pkm2-c94": [("Table", "PromptTemplates",
                  "Prompts and system instructions that carry process knowledge to AI agents, recorded with their token budget and size, "
                  "which is exactly the lightweight, size-limited channel the article describes.")],
    "pkm2-c95": [("Field", "PromptTemplates.Layer",
                  "Distinguishes the general Framework layer from the ProcedureDetail layers pulled into it (via ParentTemplate) and AdHocContext; "
                  "the plant assistant's framework prompt has lockout and press-7 detail layers beneath it.")],
    "pkm2-c96": [("Field", "KnowledgeProjections.Notation",
                  "Records the visual encoding of each diagram projection: BPMN 2.0, Mermaid flowchart, Visio flowchart.")],
    "pkm2-c97": [("Field", "KnowledgeProjections.ProjectionKind",
                  "Marks projections that are Narrative encodings -- readable procedures in prose (Markdown procedure docs, RuleSpeak) -- as distinct from diagrams.")],
    "pkm2-c98": [("Field", "Roles.PreferredKnowledgeForm",
                  "Records for a stakeholder role whether its people prefer process knowledge as Prose or as a Diagram (technicians Diagram, safety officer Prose).")],
    "pkm2-c99": [("Table", "ModelAnnotations",
                  "Comments, questions, suggestions and alternative interpretations practitioners attach to a procedure version or step, "
                  "held in their own table with where each is stored.")],
    "pkm2-c100": [("Field", "ProcedureVersions.RestsOnNotationOnly",
                   "TRUE for the 2019 lockout version, whose steps came over from a checklist diagram with no ontology type: a purely syntactic "
                   "representation that carries none of the context a reasoner or reader outside the drawing needs.")],
    "pkm2-c105": [("Field", "KnowledgeSearchEvents.FoundNothingUseful",
                   "Each search is a row, and this field records whether it found something: the press-7 accumulator search and the mixer-2 search found nothing useful.")],
    "pkm2-c106": [("Field", "KnowledgeSearchEvents.WasAbandoned",
                   "Records queries users abandoned (the press-7 and mixer-2 searches), with SecondsBeforeAbandoning beside it.")],
    "pkm2-c107": [("Field", "KnowledgeSearchEvents.DwellSeconds",
                   "Records the time each searcher spent with what was found, from 1 second for an assistant retrieval to 240 seconds reading the runbook.")],
    "pkm2-c108": [("Field", "AssistantAnswers.TaskOutcome",
                   "Records each AI task as Completed or Failed; failures include the copilot telling a technician to start maintenance before zero energy was verified.")],
    "pkm2-c109": [("Field", "Agents.AiTaskCompletionPercent",
                   "Each AI agent's task completion rate computed from its recorded answers: 42.9% for the plant copilot, 25% for the release assistant, 100% for the risk classifier.")],
    "pkm2-c110": [("Field", "AssistantAnswers.DocumentedInaccuracy",
                   "The documented inaccuracy in each wrong AI response, e.g. 'Named the superseded classifier 2.4.0; 2.4.1 has held the role since January.'")],
    "pkm2-c111": [("Field", "KnowledgeOutcomeMeasurements.MinutesPerRun",
                   "Efficiency measured as an outcome: minutes per lockout run for each crew and AI initiative.")],
    "pkm2-c112": [("Field", "KnowledgeOutcomeMeasurements.ErrorRatePercent",
                   "Error rate measured as an outcome for each crew and AI initiative (9% south night crew, 2.5% north day crew).")],
    "pkm2-c113": [("Field", "KnowledgeOutcomeMeasurements.SatisfactionScore",
                   "Customer satisfaction measured as an outcome, on a 1-5 scale, for each crew's internal customers.")],
    "pkm2-c114": [("Field", "ModelAnnotations.AnnotationKind",
                   "Practitioners report friction from missing knowledge as MissingKnowledge annotations, e.g. 'No isolation map exists for mixer 2's second drive.'")],
    "pkm2-c115": [("Field", "ProcedureVersions.IndexedSegmentCount",
                   "Adequacy for AI: how many retrieval segments of the version are served to AI; zero for versions too thin to guide an assistant.")],
    "pkm2-c116": [("Field", "ProcedureVersions.SearchSuccessPercent",
                   "Adequacy for search: the share of searches made while working with the version that found something useful (60% for lockout 2.0.0).")],
    "pkm2-c117": [("Field", "ProcedureVersions.OpenQuestionAnnotationCount",
                   "Adequacy for collaboration: open clarifying questions collaborators attached to the version, a direct measure of where it is unclear.")],
    "pkm3-c32": [("Table", "AiAdoptionInitiatives",
                  "Each initiative joins the people (sponsoring organization), the process and procedure it targets, and the AI technology, "
                  "and records when the workflow redesign for AI started, which is the end-to-end workflow the article means.")],
    "pkm3-c33": [("Field", "Procedures.HasNoExplicitSteps",
                  "TRUE for procedures such as the press-7 changeover and accumulator bleed-down that exist only as a title and documents, "
                  "with no formal step representation to ground an AI.")],
    "pkm3-c39": [("Field", "PromptTemplates.LibraryStatus",
                  "Records whether each prompt is a Maintained library asset (with MaintainedByRole) or Unmanaged, like the 2019 SOP pasted into the checklist bot.")],
    "pkm3-c40": [("Table", "AgentIntegrations",
                  "The central registry of agent integrations: agent, knowledge system, pathway, served snapshot, delivery mode and registry entry key.")],
    "pkm3-c41": [("Field", "AgentIntegrations.ServesSnapshot",
                  "Each integration hands its AI agent a governed snapshot of the process graph as its context, so the context comes from process knowledge "
                  "rather than ad hoc runtime text.")],
    "pkm4-c09": [("Table", "AssistantAnswers",
                  "Records each answer the plant copilot, release assistant or risk classifier gave, with the organization's own assertions and segments it was grounded in "
                  "(AnswerGroundings). The model records the assistant's answers; it does not run the assistant.")],
    "pkm4-c17": [("Table", "KnowledgeFragments",
                  "Records situated, tacit and implicit knowledge -- the judgment and operating constraints practitioners bring -- as its own rows attached to steps, "
                  "separate from the Steps list itself.")],
    "ont4-c37": [("Field", "Roles.EscalationBackupRole",
                  "A role names the role that backs it up on escalation: release manager to VP of Engineering, safety officer to the night-shift deputy.")],
    "ont4-c51": [("Field", "ProcedureVersions.Status",
                  "Workflow individuals carry a status, and lockout/tagout 1.0.0 is marked Deprecated.")],
    "ont4-c52": [("Field", "KnowledgeConsumerSystems.SemanticLayerComponentCount",
                  "Counts how many of taxonomy, thesaurus, ontology, metadata schemas and reasoner a knowledge system combines; the procedural knowledge graph combines all five.")],
}

EVIDENCE.update({
    # ---- pkm-1 prescriptions ---------------------------------------------------------------
    "pkm1-p07": [("Field", "ProcedureVersions.IsUnreachableKnowledge",
                  "Fires on a current version that no one can reach: no published document or diagram and no consuming system holds it, so it cannot be shared "
                  "or acted on. Fires on the conveyor maintenance, close and policy versions; not on lockout 2.0.0, deployment 3.2.0 or press changeover, which are published or synced.")],
    "pkm1-p10": [("Field", "ConsumerSystemSyncs.IsBehindCanonicalVersion",
                  "Fires when a consuming system holds a different version from the one the model says is current: the maintenance tablet still holds the deprecated "
                  "lockout 1.0.0. The copilot, release bot and graph syncs hold the current version and do not fire. Encoded knowledge that has not spread consistently is exactly this.")],
    "pkm1-p11": [("Field", "ProcedureVersions.IsNotQueryValidateReasonReady",
                  "Fires on a current version that no structured query reads, that never passed PKO profile validation, or that has no assertion in a snapshot a reasoner "
                  "found consistent. Lockout 2.0.0 and deployment 3.2.0 pass all three; conveyor maintenance (failed validation, never queried or reasoned) and press changeover fire.")],
    "pkm1-p12": [("Field", "AssistantAnswers.ActedOnWithoutHumanJudgment",
                  "Fires when an AI recommendation on a step that requires human confirmation was acted on with no expert review: the release assistant's advice to treat a "
                  "chat message as the release approval was acted on. The risk classifier's recommendation reviewed by the release manager does not fire. Computation standing in "
                  "for the judgment the step reserves to a person is the violation.")],
    "pkm1-p13": [("Field", "RulebookTables.MeaningIsOnlyTabular",
                  "Fires on a table with no semantic mapping to any ontology term, whose meaning is only its column layout (52 tables, e.g. ChangeRequests); the 101 mapped "
                  "tables do not fire. It detects where the representation carries no context or meaning beyond the fixed schema.")],
    "pkm1-p17": [("Field", "KnowledgeQueryDefinitions.MissesALayer",
                  "Fires when a question that needs ontology and instance data together is answered by a query walking only one: 'is the accountable agent software?' was "
                  "run as flat SQL over an agents export, which cannot see that a software agent is a kind of agent. The federated SPARQL approver query walks both and does not fire.")],
    "pkm1-p18": [("Field", "KnowledgeQueryDefinitions.ConsolidatedDistributedSources",
                  "Fires when a query over several knowledge systems only ran after copying them into one store: the plant-wide lockout query reads a nightly consolidated "
                  "warehouse copy of the tablet and register. The approver query federates the graph and register without consolidation and does not fire.")],
    "pkm1-p19": [("Field", "ConsumerSystemSyncs.DropsProvenanceInTransit",
                  "Fires when a version had its author on record and the copy a consuming system received dropped that provenance: the procedural graph's copy of deployment "
                  "3.2.0. The other syncs carried provenance and do not fire.")],
    "pkm1-p20": [("Field", "AiInsightProposals.IsUnvalidatedOrStrandedInsight",
                  "Fires when an AI insight was folded into a change request with nobody validating it (the variance AI's timestamp pattern) or was validated and never folded "
                  "back (the classifier's after-16:00 hotfix pattern). The copilot's gauge pattern, validated and carried into a change request, does not fire.")],
    "pkm1-p21": [("Field", "AssistantAnswers.LostTrackOfState",
                  "Fires when an assistant acted as if the execution were at a step the execution record shows never completed: the copilot assumed locks were on during a paused "
                  "run, and the release assistant assumed the approval gate had run. The record of step executions, not the AI, holds the state; answers consistent with it do not fire.")],
    "pkm1-p22": [("Field", "AssistantAnswers.ContradictsSharedModel",
                  "Checks each AI answer's asserted next step against the procedure's transition graph and fires when no transition allows it: the copilot told a technician "
                  "to go from applying locks (05) straight to maintenance (07). Answers whose next step the graph allows do not fire.")],
    "pkm1-p23": [("Field", "AssistantAnswers.RecommendationRestsOnNothingExplicit",
                  "Fires on a recommendation grounded in none of the organization's explicit process knowledge: the release assistant's hotfix advice rested on an external forum "
                  "thread only. Recommendations grounded in the step-06 or approval-gate segments do not fire.")],
    "pkm1-p24": [("Field", "AssistantAnswers.IsUncheckedRegulatedRecommendation",
                  "Fires on a recommendation acting on a step that carries a regulatory requirement which was never checked against it: the copilot's advice to skip the second "
                  "zero-energy verification (hazardous energy standard). The two recommendations checked against the change-control requirement do not fire.")],
    "pkm1-p31": [("Field", "KnowledgeConsumerSystems.IsUnlinkedToolchainComponent",
                  "Fires on a process modeling tool, semantic repository or AI platform linked to neither the shared model nor any AI integration: the process modeling workbench. "
                  "The procedure register and both AI platforms are linked and do not fire.")],
    "pkm1-p41": [("Field", "ProcedureVersions.ServesOnlyHumansOrOnlyMachines",
                  "Fires on a current version that is conveyed to people or to machines but not both: press changeover reaches only the copilot's AI platform. Lockout 2.0.0 "
                  "and deployment 3.2.0 reach technicians through published projections and AI through syncs and served assertions, and do not fire.")],
    "pkm1-p42": [("Field", "AiAdoptionInitiatives.AgentOnNotationOnlyProcedure",
                  "Fires when an agentic AI carries out a procedure whose steps have no ontology type and so no semantic context: the checklist bot runs on the 2019 lockout "
                  "transcription. Agents on typed versions do not fire.")],
})

EVIDENCE.update({
    "pkm1-q11": [("RoleQuestion", "aq-pkm1-q11",
                  "The release manager's question; IsExplicitlyGroundedRecommendation reads, per recommendation, whether it names explicit process knowledge, and the "
                  "AnswerGroundings rows list which segments and assertions each rests on.")],
    "pkm1-q12": [("RoleQuestion", "aq-pkm1-q12",
                  "The safety officer's question; ConflictsWithRegulation fires on the recommendation a requirement check found in conflict with the change-control requirement.")],
    "pkm1-q15": [("RoleQuestion", "aq-pkm1-q15",
                  "The AI enablement lead's question; CallsForProcessKnowledgeFramework is TRUE for initiatives whose task follows ordered steps and depends on organizational "
                  "knowledge, FALSE for the single-shot risk classification and the routing AI.")],
    "pkm1-s01": [("OntologyProfile", "owl-2",
                  "The OWL 2 profile, whose mappings record that the generated ontology declares tables as owl:Class and relationships as owl:ObjectProperty in RDF.")],
    "pkm1-s02": [("OntologyProfile", "sparql-1-1",
                  "The SPARQL 1.1 profile; its mapping records that the OWL substrate computes step reachability with the SPARQL property path leadsTo+.")],
    "pkm1-s03": [("OntologyProfile", "bpmn-2-0",
                  "The BPMN 2.0 profile, mapping steps to bpmn:Task and transitions to bpmn:SequenceFlow for projected diagrams.")],
    "pkm1-s04": [("OntologyProfile", "prov-o",
                  "The PROV-O profile, mapped to execution entities, participants, served-assertion provenance and answer derivations.")],
    "pkm1-i03": [("Field", "AssistantAnswers.DeliveredDespiteConflict",
                  "Witnesses the illustrated check of a proposed action against regulation doing its job: it fires when the check found the release assistant's hotfix "
                  "recommendation in conflict with the change-control requirement and it was delivered anyway. The recommendation that complied does not fire.")],
    "pkm1-i07": [("Field", "KnowledgeConsumerSystems.IsImmatureGraphPlatform",
                  "Fires on a knowledge graph platform missing one of reasoner, semantic storage, graph algorithms or machine learning: the procedural knowledge graph has no "
                  "machine learning. The defect analysis graph combines all four and does not fire.")],

    # ---- pkm-2 prescriptions ---------------------------------------------------------------
    "pkm2-p39": [("Field", "ProcedureVersions.LacksMachineInterpretableEncoding",
                  "Fires on a current version with no retrieval segments for search or no graph assertions for reasoning and AI integration: conveyor maintenance, press "
                  "changeover, close and policy. Lockout 2.0.0 and deployment 3.2.0 are encoded both ways and do not fire.")],
    "pkm2-p40": [("Field", "ReasonerRuns.IsRichnessTractabilityFailure",
                  "Fires when a reasoning run over the encoding either blew its time budget (the OWL 2 DL run, 3600s of 600) or stayed within budget only by dropping axioms "
                  "(the RDFS run dropped 12). The OWL 2 RL runs kept every axiom within budget and do not fire.")],
    "pkm2-p41": [("Field", "RetrievalSegments.IsIsolatedChunk",
                  "Fires on a segment with no position, no applicability and no link to or from another segment: the 1000-character chunk of the 2019 SOP. Segments cut at "
                  "lockout steps and decisions carry that metadata and do not fire.")],
    "pkm2-p42": [("Field", "EmbeddingProbes.SynonymsNotSimilar",
                  "Fires when an embedding model scores process near-synonyms below 0.7: the general model puts 'lock out' and 'isolate' at 0.52. The tuned model (0.88) and "
                  "approve/authorize (0.91) do not fire. The similarities are recorded probe results; the model does not compute embeddings.")],
    "pkm2-p43": [("Field", "EmbeddingProbes.OppositesNotOpposed",
                  "Fires when an embedding model scores process opposites above 0.3: the general model puts approve and reject at 0.83. The tuned model's 0.12 does not fire.")],
    "pkm2-p44": [("Field", "RetrievalSegments.IsInconsistentGrounding",
                  "Fires when retrieval serves two segments the consistency check found contradictory: the 2019 'apply locks, then disconnect power' segment is indexed alongside "
                  "the 2.0.0 'isolate first, then lock' segment. Segments with no indexed contradiction do not fire.")],
    "pkm2-p45": [("Field", "AiToolInvocations.IsUndeclaredToolUse",
                  "Fires when an AI agent invoked a tool the process graph does not declare on that step: the risk classifier called fetch-incident-history at deploy-02. "
                  "Its declared classify-change-risk call and the copilot's declared padlock reservation do not fire.")],
    "pkm2-p46": [("Field", "Steps.ToolUseRulesOnlyInProse",
                  "Fires on a step where an agent uses tools but no condition is stored as a machine expression and no decision as DMN: deploy-02, whose unscorable-change "
                  "decision exists only as prose, and the close and policy tool steps. loto-05, whose padlock inventory invariant is a parseable expression, does not fire.")],
    "pkm2-p47": [("Field", "PromptTemplates.IsDisconnectedPromptKnowledge",
                  "Fires on a prompt carrying procedure instructions that exist in no procedure version: the hotfix note typed into the release assistant. Prompts drawn from "
                  "lockout 1.0.0 or 2.0.0 do not fire.")],
    "pkm2-p48": [("Field", "AiAdoptionInitiatives.NotAnchoredInProcessKnowledge",
                  "Fires on an AI initiative past proposal that is anchored to no modeled procedure: the warehouse routing AI. Every lockout and deployment initiative targets a "
                  "procedure and does not fire.")],
    "pkm2-p49": [("Field", "PromptTemplates.SpendsBudgetOnRareDetail",
                  "Fires when a prompt inlines rarely needed detail and so exceeds its token budget: the press-7 detail layer with every retrofit note (3400 of 2500 tokens). "
                  "The condensed lockout layer within budget does not fire.")],
    "pkm2-p50": [("Field", "KnowledgeProjections.DiagramCanDivergeFromModel",
                  "Fires on a diagram drawn by hand or generated before the version last changed: the hand-drawn 2019 Visio flowchart and the Mermaid deployment flow generated "
                  "before deployment 3.2.0's June change. The BPMN diagram generated after lockout 2.0.0's last change does not fire.")],
    "pkm2-p51": [("Field", "KnowledgeProjections.NarrativeNotGeneratedFromModel",
                  "Fires on narrative documentation not generated from the model since its last change: the hand-maintained deployment runbook. The generated RuleSpeak and "
                  "natural-language lockout documents do not fire.")],
    "pkm2-p52": [("Field", "KnowledgeProjections.NarrativeUnreachable",
                  "Fires on narrative documentation published nowhere and never opened: the generated financial close SOPs. The published lockout documents and runbook do not fire.")],
    "pkm2-p53": [("Field", "ModelAnnotations.IsDiscussionInAuthoritativeModel",
                  "Fires when a comment, question, suggestion or alternative reading was stored in the authoritative rulebook: the VP's hotfix-gate comment. The question and "
                  "suggestion kept in the annotation layer do not fire.")],
    "pkm2-p54": [("Field", "KnowledgeConsumerSystems.IsKnowledgeSilo",
                  "Fires on a system holding procedures that exports no open format and receives nothing from the shared model: DocVault's proprietary binders. The modeling "
                  "workbench (exports BPMN) and the tablet (synced) do not fire.")],
    "pkm2-p57": [("Field", "ProcedureVersions.LacksContinuousDriftDetection",
                  "Fires on a current, executed version with no process-mining conformance run inside the freshness window: lockout 2.0.0 and the policy version. Deployment "
                  "3.2.0 (mined from the CI log on 18 July) and the close version do not fire.")],
    "pkm2-p58": [("Field", "ModelAnnotations.FlagBypassedAnnotationInterface",
                  "Fires when a practitioner had to flag outdated guidance outside the annotation interface: Ken emailed the steward about the retrofit bleed valve. His press-7 "
                  "map flag entered through the panel does not fire.")],
    "pkm2-p59": [("Field", "ModelAnnotations.IsLostNewKnowledge",
                  "Fires when new knowledge a practitioner added was accepted but never became a knowledge fragment: Tomas's tap-the-gauge tip. The FX-lag tip promoted to a "
                  "fragment does not fire.")],
    "pkm2-p69": [("Field", "ProcedureVersions.IsDisconnectedFromOutcomes",
                  "Fires on a current, executed version with no operational outcome measurement linked to it: the close and policy versions. Lockout 2.0.0 and deployment "
                  "3.2.0 have crew and AI outcome measurements and do not fire.")],
    "pkm2-p70": [("Field", "AiAdoptionInitiatives.IsAiOutcomeUnmeasured",
                  "Fires on a pilot or production AI never measured or not measured for over 30 days as of the evaluation instant: the release assistant, conveyor agent, "
                  "routing AI and the checklist bot (last measured in March). The copilot and risk classifier, measured in July, do not fire.")],
    "pkm2-p71": [("Field", "KnowledgeSearchEvents.IsUnlinkedFailedSearch",
                  "Fires on a search that found nothing useful and is linked to no knowledge gap and no usability barrier: Ken's press-7 accumulator search. The failed mixer-2 "
                  "search is linked to a usability barrier and does not fire.")],
    "pkm2-p72": [("Field", "Agents.IsUntrackedAiConsumer",
                  "Fires on an AI agent that delivers answers from the knowledge but has no retrievals logged, while every technician's search is: the release assistant, "
                  "risk classifier and checklist bot. The copilot's retrievals are logged and it does not fire.")],
    "pkm2-p73": [("Field", "KnowledgeOutcomeMeasurements.IsUnactedAdverseOutcome",
                  "Fires when an outcome worse than tolerated fed no change request and no investment decision: the south night crew's 9% error rate and the checklist bot's "
                  "12%. The north night crew's measurement funded a second verifier and does not fire.")],
    "pkm2-p74": [("Field", "Steps.HasReportedRealityMismatch",
                  "Fires on a step practitioners' open feedback says no longer matches how the work is done: loto-03 (press-7 map omits the accumulator) and loto-04b (retrofit "
                  "valve needs a second bleed). It identifies where the encoded knowledge is wrong; other steps do not fire.")],
    "pkm2-p75": [("Field", "ModelAnnotations.IsFrictionWithoutGap",
                  "Fires when a practitioner reported friction from missing knowledge and it was never recorded as a knowledge gap: the missing mixer-2 isolation map. The "
                  "policy receipt question raised as a gap does not fire.")],
    "pkm2-p77": [("Field", "EncodingLifecycleStages.IsStageWithoutFeedback",
                  "Fires on a lifecycle stage from which no feedback has come back: store and codify. Collect, document, encode and operationalize have annotations and do not fire.")],
})

EVIDENCE.update({
    "pkm2-q32": [("RoleQuestion", "aq-pkm2-q32",
                  "The technician's question; IsOpenOutdatedFlag fires on the two open flags (press-7 map, retrofit bleed valve) and Steps.HasReportedRealityMismatch places them at loto-03 and loto-04b.")],
    "pkm2-q33": [("RoleQuestion", "aq-pkm2-q33",
                  "The steward's question; Steps.IsDriftedFromPractice fires where practice deviates repeatedly, or deviates at a step practitioners flagged outdated (loto-04b).")],
    "pkm2-q40": [("RoleQuestion", "aq-pkm2-q40",
                  "The steward's question; KnowledgeSearchEvents.FoundNothingUseful fires on the press-7 accumulator and mixer-2 searches, with each query text recorded.")],
    "pkm2-q41": [("RoleQuestion", "aq-pkm2-q41",
                  "The steward's question; GaveUpAfterSeeingResults fires on the mixer-2 search abandoned after 75 seconds with results on screen, and SecondsBeforeAbandoning gives the point of giving up.")],
    "pkm2-q42": [("RoleQuestion", "aq-pkm2-q42",
                  "The safety officer's question; HigherAccessFewerErrors compares each crew with its lower-access baseline and is TRUE for both north crews against the south night crew.")],
    "pkm2-q43": [("RoleQuestion", "aq-pkm2-q43",
                  "The safety officer's question; HigherAccessMoreEfficient is TRUE for the north day crew and FALSE for the north night crew, which is slower than its baseline.")],
    "pkm2-q44": [("RoleQuestion", "aq-pkm2-q44",
                  "The safety officer's question; HigherAccessMoreSatisfied is TRUE for the north day crew and FALSE for the north night crew.")],
    "pkm2-q45": [("RoleQuestion", "aq-pkm2-q45",
                  "The AI enablement lead's question; Steps.IsAiFailurePoint fires on the steps where an AI task failed (loto-04b, loto-05, loto-06, deploy-02).")],
    "pkm2-q46": [("RoleQuestion", "aq-pkm2-q46",
                  "The AI enablement lead's question; Agents.AiTaskCompletionPercent gives each agent's rate and IsBelowTaskCompletionTarget fires below 80%.")],
    "pkm2-s10": [("OntologyProfile", "owl-2", "The OWL 2 profile with mappings for the owl:Class and owl:ObjectProperty declarations of the generated ontology.")],
    "pkm2-s11": [("OntologyProfile", "rdfs-1-1", "The RDF Schema profile; tables are rdfs:Class with rdfs:label/rdfs:comment and relationships carry rdfs:range.")],
    "pkm2-s13": [("KnowledgeMethod", "GraphRetrieval",
                  "Applied by MethodApplications row ma-graph-retrieval-copilot: the plant copilot's answers are grounded by structured retrieval over the 2026-07-15 graph snapshot.")],
    "pkm2-s19": [("OntologyProfile", "rdf-1-1", "The RDF 1.1 profile; served assertions are rdf:Statement triples and relationships are emitted as RDF triples.")],
    "pkm2-s20": [("OntologyProfile", "prov-o", "The PROV-O profile, mapped to served-assertion provenance (prov:wasDerivedFrom) and answer groundings (prov:Derivation).")],
    "pkm2-s21": [("OntologyProfile", "bpmn-2-0", "The BPMN 2.0 profile mapping steps and transitions to BPMN tasks and sequence flows.")],
    "pkm2-s26": [("OntologyProfile", "shacl",
                  "The SHACL profile; mappings record the sh:maxCount cardinality shapes emitted in rules.shacl.ttl and parseable step conditions as SHACL SPARQL constraints.")],
    "pkm2-i03": [("Field", "AiToolInvocations.ActedWithoutDeclaredContext",
                  "The meeting-booking point in this domain: an AI must know the context a step declares before acting. Fires when the copilot reserved padlocks for loto-05 "
                  "with one of the two declared inputs (the isolation list, not the lock inventory), and the crew was two locks short. Invocations supplied with every declared input do not fire.")],
    "pkm2-i06": [("Field", "AssistantBenchmarks.ShowsSpatialLiftFromGraphQueries",
                  "Witnesses the illustrated effect on the organization's own recorded benchmarks: TRUE for the spatial press-7 reach benchmark whose graph-querying arm gained "
                  "47 points; FALSE for the same spatial task answered over document chunks (no queries, 7 points) and for other task families. It is a recorded result, not an AI run.")],
    "pkm2-i13": [("Field", "Organizations.AiFailsForLackOfCapturedKnowledge",
                  "Fires on an organization with a failed AI initiative that has captured no procedures: ACME Logistics' routing AI. The plant and engineering divisions also "
                  "had failures but have captured procedures, and do not fire.")],
})

EVIDENCE.update({
    # ---- pkm-3 -----------------------------------------------------------------------------
    "pkm3-p08": [("Field", "AiAdoptionInitiatives.AgentOnUnderSpecifiedProcedure",
                  "Fires when an agent is set to run a version that does not specify preconditions or exception handling: the conveyor maintenance agent, the checklist bot on "
                  "the 2019 transcription, and the press-7 agent. The release and risk agents run deployment 3.2.0, which specifies both, and do not fire.")],
    "pkm3-p09": [("Field", "AiAdoptionInitiatives.HandsTacitProcedureToAgent",
                  "Fires when an agent is assigned a procedure that exists only as tacit knowledge and documents: the proposed press-7 changeover agent. Agents on procedures "
                  "with specified steps do not fire.")],
    "pkm3-p10": [("Field", "AiAdoptionInitiatives.RedesignedBeforeDocumented",
                  "Fires when the workflow redesign for AI started before the workflow was documented as a version: the conveyor agent's redesign began 10 January, conveyor "
                  "maintenance 1.0.0 was issued 1 February. Redesigns that started after the target version was issued do not fire.")],
    "pkm3-p23": [("Field", "AiAdoptionInitiatives.AgenticWithoutKnowledgeCapture",
                  "Fires on an agentic system past proposal that has produced no procedural knowledge back into the model: the release assistant and the checklist bot. The risk "
                  "classifier and conveyor agent each proposed insights and do not fire.")],
    "pkm3-p27": [("Field", "PromptTemplates.IsUnmanagedPromptInUse",
                  "Fires on a prompt an agent uses that is not maintained in the prompt library: the 2019 SOP pasted into the checklist bot. Library prompts with a maintaining role do not fire.")],
    "pkm3-p28": [("Field", "PromptTemplates.IsVerbatimDump",
                  "Fires when a prompt pastes its source at 90% or more of its size instead of codifying and condensing it: the 2019 SOP dump (3900 of 4100 tokens). The "
                  "lockout detail layer condensed to 13% of its source does not fire.")],
    "pkm3-p29": [("Field", "AgentIntegrations.IsShadowIntegration",
                  "Fires on an integration in live use with no entry in the central registry: the release assistant's agent-to-agent connection that delivered the hotfix "
                  "recommendation. Registered integrations do not fire.")],
    "pkm3-p30": [("Field", "AgentIntegrations.IsOneOffConnection",
                  "Fires on an integration whose pathway no other integration shares: the copilot's one-off export script and the checklist bot's pasted prompt. Integrations on "
                  "the shared MCP server and agent-to-agent gateway do not fire.")],
    "pkm3-q08": [("RoleQuestion", "aq-pkm3-q08",
                  "The steward's question; Procedures.IsTacitOnlyAgentTarget fires on press changeover, which an agentic initiative targets and which has no specified steps.")],
    "pkm3-i10": [("Field", "AiAdoptionInitiatives.AdoptedWithoutBottomLineResult",
                  "Witnesses the illustrated gap between adopting AI and seeing results: fires on AI in production with no measured bottom-line result (checklist bot, routing AI). "
                  "The copilot and risk classifier in production with measured results do not fire.")],
    "pkm3-i11": [("Field", "AiAdoptionInitiatives.FailedWithoutFormalizedKnowledge",
                  "Fires where prompting a model about work with no formalized steps failed: the press-7 changeover optimizer and the routing AI. The knowledge-rich lockout "
                  "copilot and risk classifier, grounded in specified procedures, succeeded and do not fire.")],

    # ---- pkm-4 -----------------------------------------------------------------------------
    "pkm4-p06": [("Field", "Procedures.IsRegulatedButUnformalized",
                  "Makes compliance exposure visible: fires on a procedure a regulation requires that exists only as a title and documents, the press-7 accumulator bleed-down "
                  "under the hazardous energy standard. Regulated procedures with specified steps do not fire.")],
    "pkm4-p07": [("Field", "ProcedureExecutions.IsUnreportedMistake",
                  "Fires on an execution that deviated from its steps with no feedback filed about it, so the mistake stays invisible beyond the raw record: the 14 July hotfix "
                  "rolled out without approval. The 9 July press-7 run, which deviated and was reported, does not fire.")],
    "pkm4-p15": [("Field", "AssistantAnswers.IsNotFromOwnKnowledge",
                  "Fires on a delivered AI answer grounded in none of the organization's own assertions or segments: the release assistant's forum-sourced hotfix advice and the "
                  "checklist bot's baked-in prompt answer. Answers grounded in the lockout graph and segments do not fire.")],
    "pkm4-p16": [("Field", "AssistantAnswers.IsUntraceableToSource",
                  "Fires on a delivered answer that was grounded but showed the person none of its sources: the copilot's 'start the maintenance' answer and the release "
                  "assistant's hotfix advice. Answers with cited groundings do not fire.")],
    "pkm4-p17": [("Field", "AiAdoptionInitiatives.WentFullWithoutReviewedPartialStage",
                  "Fires on full automation with no preceding partial stage whose outputs experts reviewed: the release assistant. The conveyor agent follows the copilot pilot "
                  "and the proposed auto-approver follows the reviewed risk classifier; neither fires.")],
    "pkm4-p18": [("Field", "AiAdoptionInitiatives.UnderinvestsKnowledgeLayer",
                  "Fires when an initiative spends less on eliciting, encoding and governing knowledge than on models: the release assistant (30k against 200k) and three others. "
                  "The copilot and risk classifier invest more in the knowledge layer and do not fire.")],
    "pkm4-p34": [("Field", "AssistantAnswers.StayedSilentOnSafetyProblem",
                  "Fires when a danger cue was on record, unescalated, for the step under way and the AI answer raised no safety concern: the copilot told Ken to carry on after "
                  "the hiss at loto-04b. Answers on steps with no unescalated cue, or that raised the concern, do not fire. The answer is recorded data, not a live AI.")],
    "pkm4-p35": [("Field", "AssistantAnswers.ArrivedAfterStepEnded",
                  "Fires when step guidance arrived after the step had already ended, so it could not guide the worker in real time: the copilot's gauge guidance on 8 July. "
                  "Guidance delivered while its step was under way does not fire.")],
    "pkm4-p36": [("Field", "ConsumerSystemSyncs.PredatesVersionChange",
                  "Fires when a machine holds the current version but took it before that version last changed, so its picture predates the workflow: the copilot platform's "
                  "copy of press changeover (synced 1 June, changed 15 June). Syncs taken after the last change do not fire.")],
    "pkm4-p37": [("Field", "ProcedureVersions.IsNotQueryValidateReasonReady",
                  "The same prescription as pkm1-p11 in Part IV's words: fires on a current version not read by any structured query, never validated against the profile, "
                  "or never checked by a reasoner (conveyor maintenance, press changeover); lockout 2.0.0 and deployment 3.2.0 do not fire.")],
    "pkm4-p38": [("Field", "ConsumerSystemSyncs.IsAiFedFromForkedCopy",
                  "Fires when a system read by AI is fed from a separate document instead of the shared model people use: the release bot platform reads the runbook. The "
                  "copilot and graph syncs from the model do not fire.")],
    "pkm4-p39": [("Field", "Agents.LacksRuntimeKnowledgeIntegration",
                  "Fires on an AI agent that answers people but receives no knowledge at run time through any integration: the checklist bot, whose SOP was pasted into its "
                  "prompt once. The copilot and release assistant retrieve at run time and do not fire.")],
    "pkm4-p40": [("Field", "AiAdoptionInitiatives.BreaksElicitEncodeConnectOrder",
                  "Fires when knowledge was connected to AI with no elicitation behind the target version, or before the version was encoded: the release assistant and risk "
                  "classifier (deployment 3.2.0 was authored without expert elicitation), the checklist bot, press optimizer and routing AI. The copilot, connected after lockout "
                  "2.0.0 was elicited from Tomas and issued, does not fire.")],
    "pkm4-s12": [("OntologyProfile", "owl-2", "The OWL 2 profile; its mappings record the procedural knowledge graph's classes and properties as OWL declarations serialized in RDF.")],
    "pkm4-s14": [("KnowledgeMethod", "GraphRetrieval",
                  "Applied by ma-graph-retrieval-copilot: the copilot's answers are retrieved from the knowledge graph snapshot, which AnswerGroundings ties to each answer.")],
    "pkm4-i02": [("Field", "KnowledgeOutcomeMeasurements.IsGainOutsideProceduralScope",
                  "Separates efficiency gains measured on non-procedural knowledge from procedural ones: TRUE for the defect analysis graph's 38% gain (product quality), FALSE for "
                  "the lockout crews and copilot, so the illustrated gain is not counted as evidence about procedures.")],
    "pkm4-i05": [("Field", "AssistantBenchmarks.ShowsGeospatialRetrievalLift",
                  "Witnesses the illustrated effect on the organization's recorded benchmark: TRUE for plant-site station and muster-point retrieval, which gained 43 points once "
                  "linked to the graph; FALSE for the other benchmarks. A recorded result, not an AI run.")],
})

EVIDENCE.update({
    # ---- ont-4 -----------------------------------------------------------------------------
    "ont4-p57": [("Field", "AiInsightProposals.GrewModelWithoutHumanSeed",
                  "Fires when an AI proposed extending the model where no human-authored seed exists: the summarizer's proposed forklift battery procedure, mined from logs with "
                  "no seed version. Its proposal against the human-authored lockout 2.0.0 does not fire.")],
    "ont4-p58": [("Field", "AssistantAnswers.AnsweredComplianceQuestionFromDocuments",
                  "Fires when an authority or safety question was answered from document passages rather than a structured query over the graph: 'who approves production "
                  "releases?' from the runbook, and the checklist bot's mixer-2 isolation answer. Structured-query answers do not fire.")],
    "ont4-p60": [("Field", "GroundingSnapshots.ServedWithoutConsistencyCheck",
                  "Fires when a graph snapshot was served to AI with no reasoner run finding it consistent: the 2026-06-01 tabular export, whose only run found it inconsistent. "
                  "The July snapshots were found consistent by the OWL 2 RL reasoner and do not fire.")],
    "ont4-p61": [("Field", "GroundingSnapshots.ServedBeforeMaterialization",
                  "Fires when a snapshot was served before a reasoner materialized its entailments: the June export (never materialized) and the 18 July hotfix rebuild "
                  "(served 08:00, materialized 09:00). The 15 July snapshot, materialized at 05:00 and served at 06:00, does not fire.")],
    "ont4-p63": [("Field", "Agents.CategoryNotAvailableAsInference",
                  "Fires on an agent named in served accountability assertions whose category the graph never infers: the deployment pipeline and superseded classifier 2.4.0. "
                  "Grace Holloway (inferred prov:Person) and classifier 2.4.1 (inferred prov:SoftwareAgent) do not fire.")],
    "ont4-p78": [("Field", "AgentIntegrations.IsDeployedOnUngovernedGraph",
                  "Fires when an AI agent is deployed against a snapshot with no steward or no review before serving: the segment summarizer on the June export. Integrations "
                  "serving the stewarded, reviewed July snapshot do not fire.")],
    "ont4-p79": [("Field", "SnapshotAssertions.LacksProvenance",
                  "Fires on a served assertion that links back to nothing -- no provenance URI, procedure version, step or role assignment: the blank-node comment "
                  "'approvals by the release lead'. Every other served assertion names its source and does not fire.")],
    "ont4-p80": [("Field", "GroundingSnapshots.ServesStaleRoleAssignments",
                  "Fires when retrieval serves an assertion resting on a role assignment that has ended: classifier 2.4.0's superseded assignment in the June export and again in "
                  "the hotfix rebuild. The 15 July snapshot does not fire.")],
    "ont4-p81": [("Field", "GroundingSnapshots.ServesDeprecatedAsCurrent",
                  "Fires when retrieval presents a deprecated workflow individual as current: the June export states lockout 1.0.0's status as Current. The July snapshots do not fire.")],
    "ont4-p82": [("Field", "SnapshotAssertions.HasOpaqueIdentifier",
                  "Fires on a served assertion whose subject is a UUID URN or blank node that no person or model can read or cite: the urn:uuid tool requirement and the _:b17 "
                  "comment. Assertions about loto-06 or ra-release-grace do not fire.")],
    "ont4-p83": [("Field", "SnapshotAssertions.LacksDublinCore",
                  "Fires on a served assertion whose subject lacks a Dublin Core title or description: loto-05 (no description) and the blank-node comment. The rest carry both and do not fire.")],
    "ont4-q01": [("RoleQuestion", "aq-ont4-q01",
                  "The risk classifier's routing question; Roles.IsProductionReleaseApprover is TRUE only for the release manager, and CurrentHolderName gives Grace Holloway.")],
    "ont4-q02": [("RoleQuestion", "aq-ont4-q02",
                  "The release manager's question; HasEscalationBackup and BackupRoleHolder give the VP of Engineering held by Omar Haddad, and HasUnfilledEscalationBackup fires "
                  "on the safety officer whose night-shift backup is vacant.")],
    "ont4-s01": [("KnowledgeMethod", "OwlReasoning",
                  "Applied by ma-owl-reasoning-0715 and ma-owl-reasoning-0718: the OWL 2 RL rule set checked and materialized the served snapshots (ReasonerRuns rr-0715-rl, rr-0718-rl).")],
    "ont4-s05": [("OntologyProfile", "dcterms",
                  "The DCMI Terms profile, mapped to resource language, format, creator and step descriptions, and to the dcterms:title and dcterms:description of served assertions.")],
    "ont4-s06": [("KnowledgeMethod", "GraphRetrieval",
                  "Applied by ma-graph-retrieval-copilot: retrieval-augmented answers grounded in the served graph snapshot rather than document chunks.")],
    "ont4-i04": [("Field", "ProcedureVersions.UsesAiInOneDirectionOnly",
                  "Fires on a version where AI only consumes the knowledge (lockout 1.0.0 served, never improved) or only contributes to it (conveyor maintenance and close "
                  "receive AI insights but serve nothing to AI). Lockout 2.0.0 and deployment 3.2.0 are both served to AI and improved by AI insights, and do not fire.")],
    "ont4-i06": [("Field", "AssistantAnswers.WrongBecauseGraphWasStale",
                  "Fires when a graph-grounded answer was wrong because the grounding assertion was stale or deprecated: 'classifier 2.4.0' from a superseded assignment and "
                  "'lockout 1.0.0 is current' from a deprecated version. Graph answers grounded on current assertions were correct and do not fire.")],
    "ont4-i07": [("Field", "RetrievalSegments.IsMachineHeldKnowledge",
                  "Fires on knowledge written by software with no human role accountable for it: the summarizer's loto-04b bleed segment. Segments with an accountable human "
                  "role do not fire. Knowledge nobody is responsible for is what a machine cannot hold.")],
    "ont4-i08": [("Field", "ReasonerRuns.PassesSchemaButFailsReasoner",
                  "Fires when an export that validates as a table design is found inconsistent by the reasoner: the June tabular export. Runs whose exports reason consistently, "
                  "or that were never schema-validated, do not fire.")],
    "ont4-i18": [("Field", "AssistantAnswers.ModelDidTheReasoning",
                  "Fires when an answer that needed inference had its logic produced by the language model rather than the reasoner or a structured query: the copilot's "
                  "skip-verification and start-maintenance answers, the runbook-interpreted approver, the hotfix advice -- all wrong. The reasoner-derived approver-and-backup answer does not fire.")],
    "ont4-i25": [("Field", "AssistantAnswers.DocumentInterpretationErred",
                  "Fires when an authority question answered by interpreting a document passage came back wrong: the runbook's 'release lead' read as the VP. The same question "
                  "answered from typed gate-role-holder assertions named Grace Holloway correctly and does not fire.")],
})

EVIDENCE.update({
    "pkm3-s01": [("KnowledgeMethod", "AgentIntegrationProtocols",
                  "Applied by ma9-mcp-copilot (the copilot reads the register through the shared MCP server) and ma9-a2a-risk-classifier (the classifier reads the graph "
                  "through the agent-to-agent gateway); AgentIntegrations records both pathways.")],
    "pkm2-s24": [("KnowledgeMethod", "ProcessDomainEmbeddingTuning",
                  "Applied by ma9-embedding-tuning: procedure-embed-1 was tuned on the process vocabulary, and EmbeddingProbes records it separating approve from reject (0.12) "
                  "where the general model does not (0.83).")],
    "pkm2-s25": [("KnowledgeMethod", "LlmEvaluationBenchmarks",
                  "Applied by ma9-assistant-benchmarks: the plant copilot was evaluated on four recorded benchmarks (AssistantBenchmarks) with and without graph grounding.")],
})
