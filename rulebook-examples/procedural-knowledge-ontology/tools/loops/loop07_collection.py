"""Loop 7 (theme B): elicitation methods and tacit knowledge.

Part II of the PKM series treats elicitation as a lossy translation of lived practice into explicit
statements, and names the methods that limit the loss: interviews that probe why and what happens when
the procedure falls short, critical incidents, observation and shadowing, workshops that bring in people
who are not normally invited, think-aloud and retrospective protocols, laddering and repertory grids.
Parts I, III and IV add the tacit/explicit distinction, SECI conversion, the deliberate work of turning
process knowledge into procedural knowledge, and the plant's before-and-after knowledge locations.

The scenario is the lockout/tagout procedure of loop 6. A knowledge engineer (Sam Adeyemi) elicits the
veterans' handling of pneumatic bleeds, twin-drive retrofits and the zero-energy check; a retiring
technician (Victor Hale) had held the twin-drive handling alone. Seeded breaches: an observed workaround
nobody asked about, an omitted obvious step never captured, a sabotaged override card still uncodified,
a gatekeeping signal left unattributed, a heuristic captured without its exceptions, a manual note
retyped into the rulebook without any elicitation, a workshop disagreement left unreconciled, and a
deployment procedure studied only from the desk and approved without its SMEs.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import agg, calc, idx, lookup, raw, rel  # noqa: E402

EXT = "https://effortlessapi.github.io/effortless-rulebooks/ns/pko-extension#"
EV = "eval-current"

LOOP = {
    "WitnessLoopId": "loop-07",
    "LoopNumber": 7,
    "Title": "Loop 7: elicitation methods and tacit knowledge",
    "Premise": "Loop 6 gave the model a lockout/tagout procedure encoded from veterans' workshops, with the "
               "executions that show where practice departs from the SOP. What the model still could not say "
               "was how that knowledge was drawn out: which methods were combined, whether interviews asked why "
               "and what happens when the procedure falls short, what observation saw that the practitioner "
               "never mentioned, who was in the workshop, which disagreements were settled, what an expert "
               "knows but cannot say, where the knowledge lived before and after, and whether gatekept "
               "knowledge was ever drawn out. This loop adds each of those with seeded breaches.",
}

KE, AUTH, STEWARD, SAFETY, OPS = ("knowledge-engineer", "knowledge-authority", "process-steward",
                                  "plant-safety-officer", "plant-operations-manager")

AGENTS = [
    {"AgentId": "victor-hale", "DisplayName": "Victor Hale", "AgentKind": "Human", "Organization": "acme-plant",
     "ContactAddress": "victor.hale@example.com", "VersionOrEmploymentKey": "technician-1988", "SemanticTypeIri": "prov:Agent"},
    {"AgentId": "rosa-delgado", "DisplayName": "Rosa Delgado", "AgentKind": "Human", "Organization": "acme-plant",
     "ContactAddress": "rosa.delgado@example.com", "VersionOrEmploymentKey": "material-handler-2021", "SemanticTypeIri": "prov:Agent"},
]


def T(name, desc, area, *fields):
    return (name, desc, area, list(fields))


def pk(name):
    return raw(name, "string", "Stored identifier.", nullable=False)


def iri():
    return raw("SemanticTypeIri", "string", "Semantic type IRI.")


def R(table_iri, cols, *rows):
    """Rows as dicts from a column list; every row gets SemanticTypeIri."""
    return [dict(zip(cols, r), SemanticTypeIri=table_iri) for r in rows]


def PV(n):
    return f"=COUNTIFS(ElicitationSessions!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, ElicitationSessions!{{{{ElicitationMode}}}}, \"{n}\")"


TABLES = [
    T("PractitionerExpertise",
      "Expertise a practitioner demonstrably holds, with the basis they could not put into words recorded as a property of that "
      "knowledge source.", "elicitation",
      pk("PractitionerExpertiseId"),
      calc("Name", "string", "Display name.", '={{Agent}} & ": " & {{Cue}}'),
      rel("Agent", "Agents", "The practitioner who holds the expertise."),
      rel("Procedure", "Procedures", "The procedure the expertise is exercised in."),
      rel("Step", "Steps", "The step the expertise is exercised at."),
      rel("ElicitationSession", "ElicitationSessions", "The session in which the expertise was probed."),
      raw("ExpertKind", "string", "Detection (sensing a condition in the moment) or DesignForesight (sensing a future cost)."),
      raw("Cue", "string", "What the practitioner senses or judges."),
      raw("UnstatedBasis", "string", "The basis the practitioner could not articulate, in the elicitor's words; blank when the basis was stated."),
      raw("ReliableCallCount", "integer", "Observed occasions on which the practitioner's call proved right."),
      raw("ForesightConfirmed", "boolean", "For design foresight: TRUE when later experience confirmed the call."),
      calc("KnowsMoreThanCanSay", "boolean", "TRUE when a practitioner reliably detects a condition whose basis they cannot state.",
           '=AND({{ExpertKind}} = "Detection", {{UnstatedBasis}} <> "", {{ReliableCallCount}} >= 3)'),
      calc("IsUnexplainedForesight", "boolean", "TRUE when a design judgment later proved right although its holder could not say why.",
           '=AND({{ExpertKind}} = "DesignForesight", {{UnstatedBasis}} <> "", {{ForesightConfirmed}})'),
      iri()),

    T("CriticalIncidents",
      "Specific past episodes of a procedure that went well, went badly or needed improvising, collected with the critical "
      "incident technique, each linked to the judgment call it brought out.", "elicitation",
      pk("CriticalIncidentId"),
      calc("Name", "string", "Display name.", '={{Outcome}} & ": " & LEFT({{Account}}, 60)'),
      rel("ProcedureVersion", "ProcedureVersions", "The version the episode is elicited for."),
      rel("Step", "Steps", "The step where the episode turned."),
      rel("ElicitationSession", "ElicitationSessions", "The critical incident session that collected it."),
      rel("Narrator", "Agents", "The practitioner who recounted the episode."),
      raw("OccurredAt", "datetime", "When the episode happened."),
      raw("Outcome", "string", "WentWell, WentBadly or Improvised."),
      raw("Account", "string", "The practitioner's account of the episode."),
      raw("RevealedJudgment", "string", "The judgment call the episode brought out that the written procedure hides; blank when none."),
      rel("JudgmentFragment", "KnowledgeFragments", "The knowledge fragment the revealed judgment was recorded as."),
      calc("IsAdverseOrImprovised", "boolean", "TRUE for an episode that went badly or had to be improvised.",
           '=OR({{Outcome}} = "WentBadly", {{Outcome}} = "Improvised")'),
      calc("HasSurfacedJudgment", "boolean", "TRUE when the episode revealed a judgment call.", '={{RevealedJudgment}} <> ""'),
      iri()),

    T("InterviewProbes",
      "Questions put in an elicitation interview, typed by what they probe: why a choice is made, what the practitioner would do "
      "if a condition changed, or what happens when the procedure falls short.", "elicitation",
      pk("InterviewProbeId"),
      calc("Name", "string", "Display name.", '={{ProbeKind}} & ": " & LEFT({{Prompt}}, 60)'),
      rel("ElicitationSession", "ElicitationSessions", "The interview the probe was asked in."),
      rel("Step", "Steps", "The step the probe is about."),
      raw("ProbeKind", "string", "Why, WhatIf or ProcedureFallsShort."),
      raw("Prompt", "string", "The question asked."),
      raw("Answer", "string", "The practitioner's answer."),
      calc("WhyAnswer", "string", "The reason a practitioner gave for a choice.", '=IF({{ProbeKind}} = "Why", {{Answer}}, "")'),
      calc("ShortfallAnswer", "string", "What the practitioner does when the standard procedure proves insufficient.",
           '=IF({{ProbeKind}} = "ProcedureFallsShort", {{Answer}}, "")'),
      iri()),

    T("ObservedActions",
      "Actions an observer saw during shadowing or observation, typed as small choices, unofficial workarounds, steps omitted "
      "from the practitioner's own account, or documented steps, with the reason given when the observer asked.", "elicitation",
      pk("ObservedActionId"),
      calc("Name", "string", "Display name.", '={{ActionKind}} & ": " & LEFT({{ActionDescription}}, 60)'),
      rel("ElicitationSession", "ElicitationSessions", "The observation session."),
      rel("Step", "Steps", "The step the action belongs to."),
      rel("Practitioner", "Agents", "The practitioner observed."),
      raw("ActionDescription", "string", "What the observer saw."),
      raw("ActionKind", "string", "SmallChoice, Workaround, OmittedObviousStep or DocumentedStep."),
      raw("MentionedInOwnAccount", "boolean", "TRUE when the practitioner's own interview account included the action."),
      raw("StatedReason", "string", "The reason the practitioner gave when the observer asked; blank when nobody asked."),
      raw("CounterfactualCondition", "string", "The different condition the observer asked about."),
      raw("CounterfactualAnswer", "string", "What the practitioner said they would do under that condition."),
      rel("CapturedAsFragment", "KnowledgeFragments", "The knowledge fragment the action was captured as."),
      calc("IsSmallChoice", "boolean", "TRUE for a small choice made during actual practice.", '={{ActionKind}} = "SmallChoice"'),
      calc("IsUnofficialWorkaround", "boolean", "TRUE for an unofficial workaround.", '={{ActionKind}} = "Workaround"'),
      calc("IsOmittedFromOwnAccount", "boolean", "TRUE when observation saw an action the practitioner's own account left out.",
           '=AND({{ElicitationSession}} <> "", NOT({{MentionedInOwnAccount}}))'),
      calc("IsMissedStepLeftUncaptured", "boolean", "TRUE when an action the interview missed was observed but never captured as knowledge.",
           '=AND({{IsOmittedFromOwnAccount}}, {{CapturedAsFragment}} = "")'),
      calc("IsWatchedNotQuestioned", "boolean", "TRUE when a small choice or workaround was watched but no reason was asked for and recorded.",
           '=AND(OR({{ActionKind}} = "SmallChoice", {{ActionKind}} = "Workaround"), {{StatedReason}} = "")'),
      calc("HasRecordedReason", "boolean", "TRUE when a reason is recorded against the observed action.", '={{StatedReason}} <> ""'),
      calc("HasCounterfactualAnswer", "boolean", "TRUE when the practitioner said what they would do if a condition were different.",
           '=AND({{CounterfactualCondition}} <> "", {{CounterfactualAnswer}} <> "")'),
      iri()),

    T("ElicitationParticipants",
      "Who took part in an elicitation session: their part in the session, whether they produce or consume the knowledge, their "
      "stake in the process, and whether people in their position are normally invited.", "elicitation",
      pk("ElicitationParticipantId"),
      calc("Name", "string", "Display name.", '={{Agent}} & " in " & {{ElicitationSession}}'),
      rel("ElicitationSession", "ElicitationSessions", "The session."),
      rel("Agent", "Agents", "The participant."),
      raw("ParticipationRole", "string", "Practitioner, SubjectMatterExpert or KnowledgeEngineer."),
      raw("KnowledgeFlow", "string", "Producer, Consumer or Both."),
      raw("ProcessStake", "string", "Initiates, Executes or DependsOnResults; blank for a facilitator with no stake."),
      raw("IsUsuallyInvited", "boolean", "FALSE for a participant whose position is normally left out of such sessions."),
      calc("IsPractitioner", "boolean", "TRUE for a practitioner who holds process knowledge.", '={{ParticipationRole}} = "Practitioner"'),
      calc("IsSubjectMatterExpert", "boolean", "TRUE for a subject matter expert.", '={{ParticipationRole}} = "SubjectMatterExpert"'),
      calc("IsKnowledgeEngineer", "boolean", "TRUE for the knowledge engineer collecting the knowledge.", '={{ParticipationRole}} = "KnowledgeEngineer"'),
      calc("IsKnowledgeProducer", "boolean", "TRUE for a participant who produces the knowledge.",
           '=OR({{KnowledgeFlow}} = "Producer", {{KnowledgeFlow}} = "Both")'),
      calc("IsKnowledgeConsumer", "boolean", "TRUE for a participant who consumes the knowledge.",
           '=OR({{KnowledgeFlow}} = "Consumer", {{KnowledgeFlow}} = "Both")'),
      iri()),

    T("RepresentationReviews",
      "Expert reviews of a procedure version: approval of the process representation itself, or evaluation of AI output built "
      "on it, with whether the reviewer is a subject matter expert whose work the procedure affects.", "elicitation",
      pk("RepresentationReviewId"),
      calc("Name", "string", "Display name.", '={{ReviewPurpose}} & ": " & {{ProcedureVersion}} & " by " & {{ReviewerAgent}}'),
      rel("ProcedureVersion", "ProcedureVersions", "The version reviewed."),
      rel("ReviewerAgent", "Agents", "The reviewer."),
      raw("ReviewPurpose", "string", "RepresentationApproval or AiEvaluation."),
      raw("ReviewerIsAffectedSme", "boolean", "TRUE when the reviewer is a subject matter expert whose work the procedure affects."),
      raw("Decision", "string", "Approved, Rejected or Pending."),
      raw("ReviewedAt", "datetime", "When the review was given."),
      iri()),

    T("WorkflowViewDivergences",
      "Differing views of how work flows, brought into the open in an elicitation session, and the reconciled account "
      "when one was reached.", "elicitation",
      pk("WorkflowViewDivergenceId"),
      calc("Name", "string", "Display name.", '={{Step}} & ": " & LEFT({{ViewA}}, 40) & " / " & LEFT({{ViewB}}, 40)'),
      rel("ElicitationSession", "ElicitationSessions", "The session where the views surfaced."),
      rel("ProcedureVersion", "ProcedureVersions", "The version the divergence is about."),
      rel("Step", "Steps", "The step the views differ on."),
      raw("ViewA", "string", "One account of how the work flows."),
      rel("HolderA", "Agents", "Who holds the first account."),
      raw("ViewB", "string", "The differing account."),
      rel("HolderB", "Agents", "Who holds the second account."),
      raw("ReconciledStatement", "string", "The account the participants settled on; blank while unsettled."),
      raw("ReconciledAt", "datetime", "When it was settled."),
      rel("ReconciledIntoFragment", "KnowledgeFragments", "The fragment recording the settled account."),
      calc("IsReconciled", "boolean", "TRUE when a settled account is recorded.", '={{ReconciledStatement}} <> ""'),
      calc("IsSurfacedButUnreconciled", "boolean", "TRUE when two differing views were brought into the open but never reconciled.",
           '=AND({{ViewA}} <> "", {{ViewB}} <> "", {{ReconciledStatement}} = "")'),
      iri()),

    T("ExpertCognitions",
      "Mental models and decision heuristics experts apply, drawn out by protocols and incident interviews, with any exceptions "
      "the expert stated and whether the capture kept them.", "elicitation",
      pk("ExpertCognitionId"),
      calc("Name", "string", "Display name.", '={{CognitionKind}} & ": " & LEFT({{Statement}}, 60)'),
      rel("Agent", "Agents", "The expert."),
      rel("Step", "Steps", "The step the cognition is applied at."),
      rel("ElicitationSession", "ElicitationSessions", "The session that drew it out."),
      raw("CognitionKind", "string", "MentalModel or DecisionHeuristic."),
      raw("Statement", "string", "The model or heuristic as captured."),
      raw("AppliedAutomatically", "boolean", "TRUE when the expert applies it without deliberating."),
      raw("ExpertStatedExceptions", "boolean", "TRUE when the expert said when the heuristic does not hold."),
      raw("CapturedExceptions", "string", "The exceptions as captured; blank when the capture dropped them."),
      calc("IsMentalModel", "boolean", "TRUE for a mental model an expert applies.", '={{CognitionKind}} = "MentalModel"'),
      calc("IsAutomaticHeuristic", "boolean", "TRUE for a decision heuristic the expert applies automatically.",
           '=AND({{CognitionKind}} = "DecisionHeuristic", {{AppliedAutomatically}})'),
      calc("IsHeuristicOversimplified", "boolean", "TRUE when an expert stated when a heuristic does not hold and the capture kept only the bare rule.",
           '=AND({{CognitionKind}} = "DecisionHeuristic", {{ExpertStatedExceptions}}, {{CapturedExceptions}} = "")'),
      iri()),

    T("ConceptLadderRungs",
      "Rungs of a concept ladder built around a concrete action: positive levels climb by asking why to the goals and values it "
      "serves, negative levels descend by asking how to its subprocesses and conditions.", "elicitation",
      pk("ConceptLadderRungId"),
      calc("Name", "string", "Display name.", '={{Step}} & " " & {{RungKind}} & ": " & LEFT({{Statement}}, 50)'),
      rel("ElicitationSession", "ElicitationSessions", "The laddering session."),
      rel("Step", "Steps", "The concrete action the ladder starts from."),
      raw("LadderLevel", "integer", "Rung height: positive above the action, negative below."),
      raw("RungKind", "string", "Goal, Value, Subprocess or Condition."),
      raw("Statement", "string", "The practitioner's answer at this rung."),
      agg("StepTopLevel", "integer", "The highest rung laddered from the same action.",
          "=MAXIFS(ConceptLadderRungs!{{LadderLevel}}, ConceptLadderRungs!{{Step}}, {{Step}})"),
      calc("IsUltimateGoal", "boolean", "TRUE for the top rung above an action: the goal or value it ultimately serves.",
           "=AND({{LadderLevel}} > 0, {{LadderLevel}} = {{StepTopLevel}})"),
      calc("IsDecompositionRung", "boolean", "TRUE for a rung below the action: a subprocess or condition that makes it up.",
           "={{LadderLevel}} < 0"),
      iri()),

    T("RepertoryGridConstructs",
      "Constructs elicited with a repertory grid: the bipolar dimensions an expert uses to tell compared situations apart.", "elicitation",
      pk("RepertoryGridConstructId"),
      calc("Name", "string", "Display name.", "={{Dimension}}"),
      rel("ElicitationSession", "ElicitationSessions", "The repertory grid session."),
      rel("Agent", "Agents", "The expert."),
      raw("SituationsCompared", "string", "The situations laid side by side."),
      raw("PoleA", "string", "One end of the dimension."),
      raw("PoleB", "string", "The other end."),
      raw("WasStatedUnprompted", "boolean", "TRUE when the expert named the dimension before the grid comparison."),
      raw("SeparatesSituations", "boolean", "TRUE when rating the compared situations on the dimension actually split them."),
      calc("Dimension", "string", "The dimension as recorded.", '={{PoleA}} & " vs " & {{PoleB}}'),
      calc("IsNeverStatedDimension", "boolean", "TRUE for a dimension the expert uses but only the grid comparison brought out.",
           '=AND({{PoleA}} <> "", NOT({{WasStatedUnprompted}}))'),
      calc("IsRecordedDiscriminatingDimension", "boolean", "TRUE for a recorded dimension that tells the compared situations apart.",
           '=AND({{PoleA}} <> "", {{SeparatesSituations}})'),
      iri()),

    T("KnowledgeConversions",
      "Recorded shifts of process knowledge between tacit and explicit form, typed by SECI conversion mode.", "elicitation",
      pk("KnowledgeConversionId"),
      calc("Name", "string", "Display name.", '={{ConversionMode}} & ": " & LEFT({{Description}}, 60)'),
      rel("ElicitationSession", "ElicitationSessions", "The session in which the conversion happened, when there was one."),
      rel("ResultFragment", "KnowledgeFragments", "The fragment the conversion produced or used."),
      raw("ConversionMode", "string", "Socialization, Externalization, Combination or Internalization."),
      raw("FromForm", "string", "Tacit or Explicit."),
      raw("ToForm", "string", "Tacit or Explicit."),
      raw("Description", "string", "What moved and how."),
      raw("OccurredAt", "datetime", "When."),
      calc("IsModeInconsistentWithForms", "boolean",
           "TRUE when the recorded SECI mode does not match the forms it converts between (socialization tacit to tacit, "
           "externalization tacit to explicit, combination explicit to explicit, internalization explicit to tacit).",
           '=NOT(OR(AND({{ConversionMode}} = "Socialization", {{FromForm}} = "Tacit", {{ToForm}} = "Tacit"), '
           'AND({{ConversionMode}} = "Externalization", {{FromForm}} = "Tacit", {{ToForm}} = "Explicit"), '
           'AND({{ConversionMode}} = "Combination", {{FromForm}} = "Explicit", {{ToForm}} = "Explicit"), '
           'AND({{ConversionMode}} = "Internalization", {{FromForm}} = "Explicit", {{ToForm}} = "Tacit")))'),
      iri()),

    T("KnowledgeHoldings",
      "Where a piece of a procedure's knowledge lives at one point in time (before and after encoding): operators' memory, "
      "formal documents, notes in outdated manuals, the memory of staff who may leave, unspoken crew agreements, or the "
      "procedure rulebook.", "elicitation",
      pk("KnowledgeHoldingId"),
      calc("Name", "string", "Display name.", '={{Snapshot}} & " " & {{Carrier}} & ": " & LEFT({{Statement}}, 50)'),
      rel("ProcedureVersion", "ProcedureVersions", "The version whose knowledge this is."),
      rel("Step", "Steps", "The step it concerns."),
      raw("Snapshot", "string", "Before or After encoding."),
      raw("KnowledgeForm", "string", "Tacit or Explicit."),
      raw("Carrier", "string", "OperatorMemory, FormalDocument, OutdatedManualNote, DepartingStaffMemory, UnspokenCrewAgreement or ProcedureRulebook."),
      rel("HolderAgent", "Agents", "The person who holds it, when a person does."),
      raw("Statement", "string", "What is held."),
      raw("IsUnsaidInSop", "boolean", "TRUE when the written SOP says nothing about it."),
      raw("HolderIsVeteran", "boolean", "TRUE when the holder is a long-tenured practitioner."),
      raw("HandlesExceptionOrDiscretion", "boolean", "TRUE when the knowledge resolves an exception or a discretionary decision."),
      rel("FormalizedAs", "KnowledgeFragments", "The fragment it was formalized into, once formalized."),
      lookup("FormalizedFragmentSession", "string", "The elicitation session that produced the formalizing fragment.",
             idx("KnowledgeFragments", "ElicitationSession", "FormalizedAs")),
      calc("IsUnformalizedProcessKnowledge", "boolean", "TRUE for lived, unformalized knowledge of how the work gets done.",
           '=AND({{FormalizedAs}} = "", {{Carrier}} <> "ProcedureRulebook")'),
      calc("IsProceduralKnowledge", "boolean", "TRUE once the knowledge is formalized as a typed fragment of the procedure ontology.",
           '={{FormalizedAs}} <> ""'),
      calc("IsJudgmentOutsideDocument", "boolean", "TRUE for unformalized knowledge the written SOP says nothing about.",
           "=AND({{IsUnsaidInSop}}, {{IsUnformalizedProcessKnowledge}})"),
      calc("IsVeteranDiscretion", "boolean", "TRUE when a veteran resolves an exception or discretionary decision that nothing written encodes.",
           "=AND({{HolderIsVeteran}}, {{HandlesExceptionOrDiscretion}}, {{IsUnformalizedProcessKnowledge}})"),
      calc("IsFormalizedWithoutElicitationWork", "boolean",
           "TRUE when knowledge was formalized into a fragment that no observation, interview or other elicitation produced.",
           '=AND({{FormalizedAs}} <> "", {{FormalizedFragmentSession}} = "")'),
      iri()),

    T("FragmentCorroborations",
      "Independent data points (another session or another practitioner) that bear on a knowledge fragment.", "elicitation",
      pk("FragmentCorroborationId"),
      calc("Name", "string", "Display name.", '={{KnowledgeFragment}} & " via " & {{ElicitationSession}}'),
      rel("KnowledgeFragment", "KnowledgeFragments", "The fragment."),
      rel("ElicitationSession", "ElicitationSessions", "The corroborating session."),
      rel("Agent", "Agents", "The corroborating practitioner."),
      raw("Agrees", "boolean", "TRUE when the data point supports the fragment."),
      iri()),

    T("KnowledgeTestOutcomes",
      "Outcomes a process knowledge test programme is expected to improve, measured before and after testing.", "elicitation",
      pk("KnowledgeTestOutcomeId"),
      calc("Name", "string", "Display name.", '={{Procedure}} & ": " & {{OutcomeArea}}'),
      rel("Procedure", "Procedures", "The procedure tested."),
      raw("OutcomeArea", "string", "CompetencyGaps, ProcessQuality, Efficiency, JobSatisfaction or Consistency."),
      raw("Measure", "string", "What was measured."),
      raw("BaselineValue", "number", "Measured before the tests."),
      raw("AfterTestValue", "number", "Measured after the tests."),
      raw("HigherIsBetter", "boolean", "TRUE when a higher value is an improvement."),
      calc("IsImproved", "boolean", "TRUE when the outcome moved in the better direction after testing.",
           "=OR(AND({{HigherIsBetter}}, {{AfterTestValue}} > {{BaselineValue}}), AND(NOT({{HigherIsBetter}}), {{AfterTestValue}} < {{BaselineValue}}))"),
      iri()),
]

ES = "ElicitationSessions"
FIELDS = {
    "ElicitationSessions": [
        raw("ObserverStance", "string", "For observation: NonParticipantObservation, Shadowing or LegitimatePeripheralParticipation."),
        raw("Setting", "string", "InSitu (where the work is done), Office or Remote."),
        raw("RecordingReference", "string", "The recording of the practitioner's work made during the session, when one was made."),
        raw("PractitionerReviewedAt", "datetime", "When the practitioner reviewed the recording afterward to explain their reasoning."),
        calc("ElicitationMode", "string", "The family of elicitation the session's method belongs to.",
             '=IF({{Method}} = "PractitionerInterview", "Interview", IF({{Method}} = "Shadowing", "Observation", '
             'IF({{Method}} = "FacilitatedWorkshop", "Workshop", IF({{Method}} = "CriticalIncidentTechnique", "Incident", '
             'IF(OR({{Method}} = "ThinkAloudProtocol", {{Method}} = "RetrospectiveProtocol", {{Method}} = "ConceptLaddering", '
             '{{Method}} = "RepertoryGrid"), "Protocol", "Other")))))'),
        calc("IsInterview", "boolean", "TRUE for an interview session.", '={{ElicitationMode}} = "Interview"'),
        calc("IsWorkshop", "boolean", "TRUE for a workshop session.", '={{ElicitationMode}} = "Workshop"'),
        agg("WhyProbeCount", "integer", "Probes asking why a choice is made.",
            '=COUNTIFS(InterviewProbes!{{ElicitationSession}}, {{ElicitationSessionId}}, InterviewProbes!{{ProbeKind}}, "Why")'),
        agg("ShortfallProbeCount", "integer", "Probes asking what happens when the procedure falls short.",
            '=COUNTIFS(InterviewProbes!{{ElicitationSession}}, {{ElicitationSessionId}}, InterviewProbes!{{ProbeKind}}, "ProcedureFallsShort")'),
        calc("IsInterviewWithoutWhyProbe", "boolean", "TRUE for an interview that never asked why a choice is made.",
             "=AND({{IsInterview}}, {{WhyProbeCount}} = 0)"),
        calc("IsInterviewWithoutShortfallProbe", "boolean", "TRUE for an interview that never asked what happens when the procedure falls short.",
             "=AND({{IsInterview}}, {{ShortfallProbeCount}} = 0)"),
        agg("InitiatorCount", "integer", "Participants who start the process.",
            '=COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{ProcessStake}}, "Initiates")'),
        agg("ExecutorCount", "integer", "Participants who carry the process out.",
            '=COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{ProcessStake}}, "Executes")'),
        agg("DependentCount", "integer", "Participants who depend on the process's results.",
            '=COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{ProcessStake}}, "DependsOnResults")'),
        agg("UninvitedParticipantCount", "integer", "Participants whose position is normally not invited.",
            "=COUNTIFS(ElicitationParticipants!{{ElicitationSession}}, {{ElicitationSessionId}}, ElicitationParticipants!{{IsUsuallyInvited}}, FALSE)"),
        calc("GathersWholeProcessChain", "boolean", "TRUE for a workshop that gathered people who start, carry out and depend on the process.",
             "=AND({{IsWorkshop}}, {{InitiatorCount}} > 0, {{ExecutorCount}} > 0, {{DependentCount}} > 0)"),
        calc("IsWorkshopWithoutUsualOutsiders", "boolean", "TRUE for a workshop in which nobody normally left out took part.",
             "=AND({{IsWorkshop}}, {{UninvitedParticipantCount}} = 0)"),
        lookup("MethodFamily", "string", "The family of the session's method.", idx("KnowledgeMethods", "MethodFamily", "Method")),
        agg("FacilitatorKnowledgeEngineerRoleCount", "integer", "Knowledge engineer roles the facilitator currently holds.",
            '=COUNTIFS(Roles!{{CurrentAgent}}, {{FacilitatorAgent}}, Roles!{{RoleId}}, "knowledge-engineer")'),
        calc("FacilitatorIsKnowledgeEngineer", "boolean", "TRUE when the session was run by the organization's knowledge engineer.",
             "={{FacilitatorKnowledgeEngineerRoleCount}} > 0"),
        calc("IsGenericOrUnskilledCapture", "boolean",
             "TRUE when a session captured knowledge with a method outside the elicitation family or without a skilled elicitor running it.",
             '=AND({{Method}} <> "", OR({{MethodFamily}} <> "Elicitation", NOT({{FacilitatorIsKnowledgeEngineer}})))'),
        calc("IsKeFieldSession", "boolean", "TRUE for a session the knowledge engineer ran where the work is done.",
             '=AND({{FacilitatorIsKnowledgeEngineer}}, {{Setting}} = "InSitu")'),
        calc("IsReviewedRecording", "boolean", "TRUE when the practitioner reviewed a recording of their own work afterward to explain it.",
             '=AND({{RecordingReference}} <> "", {{PractitionerReviewedAt}} <> "")'),
    ],
    "KnowledgeFragments": [
        raw("CognitiveBasis", "string", "Intuition, PatternRecognition or Judgment for tacit knowledge; Articulated for knowledge stated outright."),
        raw("TacitnessDegree", "integer", "Position on the tacit-to-explicit spectrum: 1 fully articulated to 5 not articulable."),
        raw("LostInTranslation", "string", "What the explicit statement fails to carry of the practice it was drawn from."),
        raw("EncodedAs", "string", "HardRule, ConditionedGuidance, CueNarrative or Statement."),
        raw("StatedConditions", "string", "The conditions under which the fragment holds, as captured."),
        calc("IsFlattenedToBrittleRule", "boolean",
             "TRUE when tacit or situated knowledge was encoded as a hard rule with no conditions saying when it holds.",
             '=AND(OR({{KnowledgeForm}} = "Tacit", {{KnowledgeForm}} = "SituatedJudgment"), {{EncodedAs}} = "HardRule", {{StatedConditions}} = "")'),
        agg("CorroborationCount", "integer", "Independent supporting data points.",
            "=COUNTIFS(FragmentCorroborations!{{KnowledgeFragment}}, {{KnowledgeFragmentId}}, FragmentCorroborations!{{Agrees}}, TRUE)"),
        calc("RestsOnSingleDataPoint", "boolean", "TRUE for approved knowledge no second session or practitioner supports.",
             '=AND({{Status}} = "Approved", {{CorroborationCount}} = 0)'),
    ],
    "KnowledgeGaps": [
        raw("GapCause", "string", "Gatekeeping, Sabotage, Undocumented or Unknown; blank when never classified."),
        raw("HolderDeclinedToShare", "boolean", "TRUE when an elicitation recorded the one person who knows declining to share it."),
        raw("SiloedWithin", "string", "The group the knowledge stays inside."),
        rel("DrawnOutBySession", "ElicitationSessions", "The session that drew the gatekept knowledge out."),
        rel("CodifiedAsFragment", "KnowledgeFragments", "The fragment it was codified as."),
        calc("IsKnownAndUnresolved", "boolean", "TRUE for an identified gap that is still open.", '=AND({{IdentifiedAt}} <> "", {{IsOpen}})'),
        calc("IsGatekeepingOrSabotage", "boolean", "TRUE for a gap caused by gatekeeping or knowledge sabotage.",
             '=OR({{GapCause}} = "Gatekeeping", {{GapCause}} = "Sabotage")'),
        calc("IsUnattributedGatekeeping", "boolean", "TRUE when a holder was recorded declining to share but the gap is not identified as gatekeeping or sabotage.",
             "=AND({{HolderDeclinedToShare}}, NOT({{IsGatekeepingOrSabotage}}))"),
        calc("IsRequiredGatekeptUncodified", "boolean", "TRUE for blocking gatekept or sabotaged knowledge never drawn out and codified.",
             '=AND({{IsGatekeepingOrSabotage}}, {{BlockingKind}} = "Blocking", {{CodifiedAsFragment}} = "")'),
    ],
    "KnowledgeMethods": [
        raw("ElicitationTradeoff", "string", "What the method gives up (accuracy or depth) and what it gains (practicality or reach)."),
    ],
}

KH = "KnowledgeHoldings"
FIELDS["ProcedureVersions"] = [
    agg("InterviewSessionCount", "integer", "Interview sessions for the version.", PV("Interview")),
    agg("ObservationSessionCount", "integer", "Observation sessions for the version.", PV("Observation")),
    agg("WorkshopSessionCount", "integer", "Workshop sessions for the version.", PV("Workshop")),
    agg("ProtocolSessionCount", "integer", "Think-aloud, retrospective, laddering and grid sessions for the version.", PV("Protocol")),
    agg("IncidentSessionCount", "integer", "Critical incident sessions for the version.", PV("Incident")),
    agg("ReconciledDivergenceCount", "integer", "Workflow disagreements settled for the version.",
        "=COUNTIFS(WorkflowViewDivergences!{{ProcedureVersion}}, {{ProcedureVersionId}}, WorkflowViewDivergences!{{IsReconciled}}, TRUE)"),
    calc("ComplementaryMethodCount", "integer", "Distinct families of elicitation used for the version.",
         "=IF({{InterviewSessionCount}} > 0, 1, 0) + IF({{ObservationSessionCount}} > 0, 1, 0) + IF({{WorkshopSessionCount}} > 0, 1, 0)"
         " + IF({{ProtocolSessionCount}} > 0, 1, 0) + IF({{IncidentSessionCount}} > 0, 1, 0)"),
    calc("ReliesOnSingleMethod", "boolean", "TRUE when the version's knowledge was elicited with one family of method only.",
         "={{ComplementaryMethodCount}} = 1"),
    calc("MissesARequiredElicitationMode", "boolean",
         "TRUE for an elicited version lacking a workshop, a structured interview, observation, or a session that settled a disagreement.",
         "=AND({{ComplementaryMethodCount}} > 0, OR({{WorkshopSessionCount}} = 0, {{InterviewSessionCount}} = 0, "
         "{{ObservationSessionCount}} = 0, {{ReconciledDivergenceCount}} = 0))"),
    agg("TacitFormFragmentCount", "integer", "Tacit fragments attached to the version.",
        '=COUNTIFS(KnowledgeFragments!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeFragments!{{KnowledgeForm}}, "Tacit")'),
    agg("SituatedJudgmentFragmentCount", "integer", "Situated-judgment fragments attached to the version.",
        '=COUNTIFS(KnowledgeFragments!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeFragments!{{KnowledgeForm}}, "SituatedJudgment")'),
    calc("TacitJudgmentFragmentCount", "integer", "Tacit or situated-judgment fragments attached to the version.",
         "={{TacitFormFragmentCount}} + {{SituatedJudgmentFragmentCount}}"),
    agg("CriticalIncidentCount", "integer", "Critical incidents collected for the version.",
        "=COUNTIFS(CriticalIncidents!{{ProcedureVersion}}, {{ProcedureVersionId}})"),
    calc("JudgmentUnprobedByIncidents", "boolean",
         "TRUE when a version relies on tacit or situated judgment yet no critical incident was collected to surface it.",
         "=AND({{TacitJudgmentFragmentCount}} > 0, {{CriticalIncidentCount}} = 0)"),
    agg("SmeApprovalCount", "integer", "Approvals of the representation by subject matter experts it affects.",
        '=COUNTIFS(RepresentationReviews!{{ProcedureVersion}}, {{ProcedureVersionId}}, RepresentationReviews!{{ReviewPurpose}}, "RepresentationApproval", '
        'RepresentationReviews!{{ReviewerIsAffectedSme}}, TRUE, RepresentationReviews!{{Decision}}, "Approved")'),
    agg("SmeAiEvaluationCount", "integer", "Evaluations of AI output on this version by affected subject matter experts.",
        '=COUNTIFS(RepresentationReviews!{{ProcedureVersion}}, {{ProcedureVersionId}}, RepresentationReviews!{{ReviewPurpose}}, "AiEvaluation", '
        "RepresentationReviews!{{ReviewerIsAffectedSme}}, TRUE)"),
    calc("IsApprovedWithoutSmeSignoff", "boolean", "TRUE for an approved version no affected subject matter expert approved the representation of.",
         '=AND({{Status}} = "Approved", {{SmeApprovalCount}} = 0)'),
    calc("ExpertsEvaluateAiNotRepresentation", "boolean",
         "TRUE when experts were recruited to evaluate AI output on a version but none to approve the representation it rests on.",
         "=AND({{SmeAiEvaluationCount}} > 0, {{SmeApprovalCount}} = 0)"),
    agg("KeSessionCount", "integer", "Sessions the knowledge engineer ran for the version.",
        "=COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{FacilitatorIsKnowledgeEngineer}}, TRUE)"),
    agg("KeFieldSessionCount", "integer", "Sessions the knowledge engineer ran where the work is done.",
        "=COUNTIFS(ElicitationSessions!{{ProcedureVersion}}, {{ProcedureVersionId}}, ElicitationSessions!{{IsKeFieldSession}}, TRUE)"),
    calc("IsStudiedOnlyFromTheDesk", "boolean", "TRUE when the knowledge engineer elicited the version but never went where the work happens.",
         "=AND({{KeSessionCount}} > 0, {{KeFieldSessionCount}} = 0)"),
    agg("JudgmentHeldOutsideSopCount", "integer", "Unformalized knowledge of the version its SOP says nothing about.",
        f"=COUNTIFS({KH}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KH}!{{{{IsJudgmentOutsideDocument}}}}, TRUE)"),
    agg("TacitHoldingCount", "integer", "Holdings of the version in tacit form.",
        f'=COUNTIFS({KH}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KH}!{{{{KnowledgeForm}}}}, "Tacit")'),
    agg("ExplicitHoldingCount", "integer", "Holdings of the version in explicit form.",
        f'=COUNTIFS({KH}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KH}!{{{{KnowledgeForm}}}}, "Explicit")'),
    calc("TacitShareExceedsExplicit", "boolean", "TRUE when more of the version's knowledge is held tacitly than in explicit carriers.",
         "={{TacitHoldingCount}} > {{ExplicitHoldingCount}}"),
    agg("HandsHeldCount", "integer", "Unformalized knowledge held only in operators' memory.",
        f'=COUNTIFS({KH}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KH}!{{{{Carrier}}}}, "OperatorMemory", {KH}!{{{{IsUnformalizedProcessKnowledge}}}}, TRUE)'),
    agg("NegotiatedPracticeCount", "integer", "Unformalized knowledge held as unspoken crew agreements.",
        f'=COUNTIFS({KH}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KH}!{{{{Carrier}}}}, "UnspokenCrewAgreement", {KH}!{{{{IsUnformalizedProcessKnowledge}}}}, TRUE)'),
    calc("LivesInHandsSilenceAndNegotiation", "boolean",
         "TRUE when the version's knowledge lives at once in experienced hands, in what its SOP leaves unsaid, and in unspoken crew agreements.",
         "=AND({{HandsHeldCount}} > 0, {{JudgmentHeldOutsideSopCount}} > 0, {{NegotiatedPracticeCount}} > 0)"),
]


def F(table, *names):
    return [f"{table}.{n}" for n in names]


OA, IP, EP, RR, WD = "ObservedActions", "InterviewProbes", "ElicitationParticipants", "RepresentationReviews", "WorkflowViewDivergences"
PVS, KF, KG = "ProcedureVersions", "KnowledgeFragments", "KnowledgeGaps"

QUESTIONS = [
    ("aq-pkm2-q01", KE, "For each action I observed, what reason did the practitioner give, and is it recorded against the action?",
     "An observed action without its reason is a habit nobody can judge; the reason is what makes it transferable.",
     F(OA, "ElicitationSession", "Step", "Practitioner", "ActionDescription", "StatedReason", "HasRecordedReason")),
    ("aq-pkm2-q02", KE, "When I asked what they would do if a condition were different, what did the practitioner say?",
     "The what-if answer exposes the conditions a practice depends on, which the practitioner rarely volunteers.",
     F(OA, "CounterfactualCondition", "CounterfactualAnswer", "HasCounterfactualAnswer")),
    ("aq-pkm2-q03", SAFETY, "What goal or value does this lockout action ultimately serve, according to the technician who does it?",
     "A technician who knows what a step protects can adapt it safely; one who only knows the step cannot.",
     F("ConceptLadderRungs", "ElicitationSession", "Step", "LadderLevel", "RungKind", "Statement", "StepTopLevel", "IsUltimateGoal")),
    ("aq-pkm2-q04", KE, "According to the practitioner, which subprocesses and conditions make up this higher-level activity?",
     "Laddering down finds the sub-steps and preconditions a step title hides.",
     F("ConceptLadderRungs", "IsDecompositionRung")),
    ("aq-pkm2-q05", KE, "Along which recorded dimensions do our experts tell these machines' lockout situations apart?",
     "The dimensions an expert discriminates on are the ones a newcomer cannot see and the SOP never names.",
     F("RepertoryGridConstructs", "ElicitationSession", "Agent", "SituationsCompared", "PoleA", "PoleB", "WasStatedUnprompted",
       "SeparatesSituations", "Dimension", "IsNeverStatedDimension", "IsRecordedDiscriminatingDimension")),
    ("aq-pkm2-q10", AUTH, "Which knowledge gaps do we know about and have not yet closed?",
     "A known unknown is manageable; the authority must see the list, not rediscover it in an incident.",
     F(KG, "IsKnownAndUnresolved")),
    ("aq-pkm2-q11", AUTH, "Which of our knowledge gaps come from gatekeeping or sabotage?",
     "Gatekept gaps do not close by asking again; they need a different method and a different conversation.",
     F(KG, "GapCause", "SiloedWithin", "IsGatekeepingOrSabotage")),
    ("aq-pkm2-q17", SAFETY, "Which past lockout episodes went well, went badly, or had to be improvised?",
     "The episodes are where the procedure met the machine; the bad and improvised ones carry the lessons.",
     F("CriticalIncidents", "ProcedureVersion", "Step", "ElicitationSession", "Narrator", "OccurredAt", "Outcome", "Account",
       "IsAdverseOrImprovised")),
    ("aq-pkm2-q18", SAFETY, "What judgment call did each of those episodes reveal, and where is it recorded?",
     "An episode retold without the judgment it revealed is an anecdote, not knowledge.",
     F("CriticalIncidents", "RevealedJudgment", "JudgmentFragment", "HasSurfacedJudgment")),
    ("q7-ke-knows-more-than-can-say", KE, "Which of our practitioners reliably know something they cannot put into words?",
     "Expertise without a statable basis is the knowledge most at risk and the hardest to elicit; it must be recorded as such.",
     F("PractitionerExpertise", "Agent", "Procedure", "Step", "ElicitationSession", "ExpertKind", "Cue", "UnstatedBasis",
       "ReliableCallCount", "ForesightConfirmed", "KnowsMoreThanCanSay", "IsUnexplainedForesight")),
    ("q7-authority-incidents-unprobed", AUTH, "Which versions rely on tacit judgment that no critical incident was ever collected to surface?",
     "Documentation hides judgment calls; without incidents the version encodes the steps and misses the calls.",
     F(PVS, "TacitFormFragmentCount", "SituatedJudgmentFragmentCount", "TacitJudgmentFragmentCount", "CriticalIncidentCount", "JudgmentUnprobedByIncidents")),
    ("q7-authority-gatekept-gaps", AUTH, "Where did someone decline to share knowledge, and was the gatekept knowledge we need ever drawn out and codified?",
     "A gap caused by withholding must be named as such, and required knowledge behind it must be drawn out before the holder leaves.",
     F(KG, "HolderDeclinedToShare", "DrawnOutBySession", "CodifiedAsFragment", "IsUnattributedGatekeeping", "IsRequiredGatekeptUncodified")),
    ("q7-ke-interview-probes", KE, "Did each interview ask why choices are made and what happens when the procedure falls short?",
     "An interview that only walks the steps records what the SOP already says.",
     F(IP, "ElicitationSession", "Step", "ProbeKind", "Prompt", "Answer", "WhyAnswer", "ShortfallAnswer")
     + F(ES, "ElicitationMode", "IsInterview", "WhyProbeCount", "ShortfallProbeCount", "IsInterviewWithoutWhyProbe",
         "IsInterviewWithoutShortfallProbe")),
    ("q7-ke-observation-captures", KE, "What did observation see that the practitioner never mentioned, and did we capture it and ask why?",
     "Observation exists to catch the obvious and the unreachable; seeing it and not recording it wastes the method.",
     F(OA, "ActionKind", "MentionedInOwnAccount", "CapturedAsFragment", "IsSmallChoice", "IsUnofficialWorkaround",
       "IsOmittedFromOwnAccount", "IsMissedStepLeftUncaptured", "IsWatchedNotQuestioned") + F(ES, "ObserverStance")),
    ("q7-steward-workshop-composition", STEWARD, "Who was in each workshop: did it include everyone who starts, does and depends on the process, and people normally left out?",
     "A workshop of the usual invitees reproduces the official picture of the work.",
     F(EP, "ElicitationSession", "Agent", "ParticipationRole", "KnowledgeFlow", "ProcessStake", "IsUsuallyInvited", "IsPractitioner",
       "IsSubjectMatterExpert", "IsKnowledgeEngineer", "IsKnowledgeProducer", "IsKnowledgeConsumer")
     + F(ES, "IsWorkshop", "InitiatorCount", "ExecutorCount", "DependentCount", "UninvitedParticipantCount",
         "GathersWholeProcessChain", "IsWorkshopWithoutUsualOutsiders")),
    ("q7-steward-sme-approval", STEWARD, "Did the subject matter experts whose work a procedure affects approve its representation?",
     "A representation nobody affected approved is the knowledge engineer's reading of the work, not the work.",
     F(RR, "ProcedureVersion", "ReviewerAgent", "ReviewPurpose", "ReviewerIsAffectedSme", "Decision", "ReviewedAt")
     + F(PVS, "SmeApprovalCount", "SmeAiEvaluationCount", "IsApprovedWithoutSmeSignoff", "ExpertsEvaluateAiNotRepresentation")),
    ("q7-steward-reconciled-views", STEWARD, "Which differing accounts of how work flows came out in sessions, and were they reconciled?",
     "Surfacing a disagreement without settling it leaves two procedures in practice and one on paper.",
     F(WD, "ElicitationSession", "ProcedureVersion", "Step", "ViewA", "HolderA", "ViewB", "HolderB", "ReconciledStatement",
       "ReconciledAt", "ReconciledIntoFragment", "IsReconciled", "IsSurfacedButUnreconciled")),
    ("q7-authority-blended-elicitation", AUTH, "Was each version's knowledge elicited with complementary methods, including workshops, interviews, observation and a settled disagreement?",
     "Every method sees part of the practice; one method alone gives a partial picture that looks complete.",
     F(PVS, "InterviewSessionCount", "ObservationSessionCount", "WorkshopSessionCount", "ProtocolSessionCount", "IncidentSessionCount",
       "ReconciledDivergenceCount", "ComplementaryMethodCount", "ReliesOnSingleMethod", "MissesARequiredElicitationMode")),
    ("q7-ke-skilled-specific-capture", KE, "Was each session run with an elicitation method by someone skilled at capturing process knowledge, and what did each method trade away?",
     "Generic knowledge handling by whoever is available loses exactly the tacit part the session was for.",
     F(ES, "MethodFamily", "FacilitatorKnowledgeEngineerRoleCount", "FacilitatorIsKnowledgeEngineer", "IsGenericOrUnskilledCapture")
     + F("KnowledgeMethods", "ElicitationTradeoff")),
    ("q7-ke-field-researcher", KE, "For each version I elicited, did I go where the work is done, like a field researcher, or only interview from the desk?",
     "Process knowledge lives in the setting; a knowledge engineer who never visits studies the account, not the practice.",
     F(ES, "Setting", "IsKeFieldSession") + F(PVS, "KeSessionCount", "KeFieldSessionCount", "IsStudiedOnlyFromTheDesk")),
    ("q7-ke-tacit-protocols", KE, "What did the protocols draw out: mental models, automatic heuristics, reviewed recordings, and how tacit and how lossy was each fragment?",
     "Tacit knowledge is captured as models, heuristics and reasons, and the capture must record what it could not carry.",
     F("ExpertCognitions", "Agent", "Step", "ElicitationSession", "CognitionKind", "Statement", "AppliedAutomatically", "IsMentalModel",
       "IsAutomaticHeuristic") + F(ES, "RecordingReference", "PractitionerReviewedAt", "IsReviewedRecording")
     + F(KF, "CognitiveBasis", "TacitnessDegree", "LostInTranslation")),
    ("q7-authority-flattened-rules", AUTH, "Which tacit knowledge did we flatten into bare rules, dropping the conditions and exceptions the expert gave?",
     "A rule stripped of its conditions fails on the machine it was never meant for, and does so with authority.",
     F(KF, "EncodedAs", "StatedConditions", "IsFlattenedToBrittleRule")
     + F("ExpertCognitions", "ExpertStatedExceptions", "CapturedExceptions", "IsHeuristicOversimplified")),
    ("q7-authority-corroboration", AUTH, "Which approved knowledge rests on a single data point that no second session or practitioner supports?",
     "Process knowledge is non-linear; one person's account on one day is an anecdote until corroborated.",
     F("FragmentCorroborations", "KnowledgeFragment", "ElicitationSession", "Agent", "Agrees")
     + F(KF, "CorroborationCount", "RestsOnSingleDataPoint")),
    ("q7-ke-seci-conversions", KE, "How did the lockout knowledge move between tacit and explicit form, and is each conversion typed consistently?",
     "Mislabelled conversions hide that a retyped document was treated as captured expertise.",
     F("KnowledgeConversions", "ElicitationSession", "ResultFragment", "ConversionMode", "FromForm", "ToForm", "Description",
       "OccurredAt", "IsModeInconsistentWithForms")),
    ("q7-opsmgr-knowledge-locations", OPS, "Where did the plant's lockout knowledge live before we encoded it, and where does it live now?",
     "Knowledge in operators' heads, manual margins and departing staff leaves with them; the move to the rulebook is the point.",
     F(KH, "ProcedureVersion", "Step", "Snapshot", "KnowledgeForm", "Carrier", "HolderAgent", "Statement", "IsUnsaidInSop",
       "HolderIsVeteran", "HandlesExceptionOrDiscretion", "FormalizedAs", "IsUnformalizedProcessKnowledge", "IsProceduralKnowledge",
       "IsJudgmentOutsideDocument", "IsVeteranDiscretion")
     + F(PVS, "JudgmentHeldOutsideSopCount", "TacitHoldingCount", "ExplicitHoldingCount", "TacitShareExceedsExplicit",
         "HandsHeldCount", "NegotiatedPracticeCount", "LivesInHandsSilenceAndNegotiation")),
    ("q7-authority-formalized-without-work", AUTH, "Which knowledge did we formalize without anyone observing, interviewing or extracting it?",
     "Formalizing is deliberate work; a note retyped into the rulebook inherits the rulebook's authority without earning it.",
     F(KH, "FormalizedFragmentSession", "IsFormalizedWithoutElicitationWork")),
    ("q7-opsmgr-knowledge-tests", OPS, "Did the lockout knowledge tests improve competency gaps, quality, efficiency, satisfaction and consistency?",
     "Tests are justified by what they improve; an outcome that did not move says where the programme falls short.",
     F("KnowledgeTestOutcomes", "Procedure", "OutcomeArea", "Measure", "BaselineValue", "AfterTestValue", "HigherIsBetter", "IsImproved")),
]

# ---------------------------------------------------------------------------------------------- seed data
X = EXT
ES_COLS = ["ElicitationSessionId", "ProcedureVersion", "Method", "StartedAt", "EndedAt", "PractitionerAgent", "FacilitatorAgent",
           "Summary", "Status", "EvaluationContext", "ObserverStance", "Setting", "RecordingReference", "PractitionerReviewedAt"]
L2, D3 = "loto-v2.0.0", "deploy-v3.2.0"
SAM = "sam-adeyemi"


def ts(d, h="09:00"):
    return f"2026-{d}T{h}:00-05:00"


ROWS = {
    "ElicitationSessions": R(f"{X}ElicitationSession", ES_COLS,
        ("el7-loto-interview", L2, "PractitionerInterview", ts("03-10"), ts("03-10", "11:00"), "tomas-reyes", SAM,
         "Structured interview on isolating and bleeding the pneumatic supply and the zero-energy check.", "Approved", EV, None, "InSitu", None, None),
        ("el7-loto-shadow", L2, "Shadowing", ts("03-17", "06:00"), ts("03-17", "14:00"), "ken-watanabe", SAM,
         "Full day shift shadowing lockouts on press line 3.", "Approved", EV, "Shadowing", "InSitu", None, None),
        ("el7-loto-lpp", L2, "Shadowing", ts("03-19", "06:00"), ts("03-19", "14:00"), "tomas-reyes", SAM,
         "Sam fetched locks, held tags and staged tools for Tomas's crew for a shift, asking about each move.", "Approved", EV,
         "LegitimatePeripheralParticipation", "InSitu", None, None),
        ("el7-loto-workshop", L2, "FacilitatedWorkshop", ts("04-15", "13:00"), ts("04-15", "17:00"), "tomas-reyes", SAM,
         "Lockout workshop with technicians, the safety officer, the operations manager and a material handler.", "Approved", EV, None, "InSitu", None, None),
        ("el7-loto-cit", L2, "CriticalIncidentTechnique", ts("03-24"), ts("03-24", "12:00"), "victor-hale", SAM,
         "Critical incidents recounted by the two veteran technicians before Victor Hale's retirement.", "Approved", EV, None, "InSitu", None, None),
        ("el7-loto-thinkaloud", L2, "ThinkAloudProtocol", ts("04-02", "07:00"), ts("04-02", "08:00"), "ken-watanabe", SAM,
         "Ken verbalizes his reasoning while locking out mixer 2.", "Approved", EV, None, "InSitu", "video: mixer-2 lockout think-aloud", None),
        ("el7-loto-retro", L2, "RetrospectiveProtocol", ts("04-03", "07:00"), ts("04-03", "08:00"), "tomas-reyes", SAM,
         "Tomas's press 3 lockout recorded without interruption, then reviewed with him the next day.", "Approved", EV, None, "InSitu",
         "video: press-3 lockout 2026-04-03", ts("04-04", "10:00")),
        ("el7-loto-ladder", L2, "ConceptLaddering", ts("04-08"), ts("04-08", "10:30"), "tomas-reyes", SAM,
         "Laddering up and down from bleeding the pneumatic supply and applying personal locks.", "Approved", EV, None, "InSitu", None, None),
        ("el7-loto-grid", L2, "RepertoryGrid", ts("04-09"), ts("04-09", "10:30"), "tomas-reyes", SAM,
         "Repertory grid over mixer 2, press 3 and conveyor 5 lockouts.", "Approved", EV, None, "InSitu", None, None),
        ("el7-deploy-interview", D3, "PractitionerInterview", ts("06-22", "15:00"), ts("06-22", "16:00"), "ravi-menon", SAM,
         "Video-call interview on building and rolling out release candidates.", "Approved", EV, None, "Remote", None, None),
    ) + [
        {"ElicitationSessionId": "elicit-close-shadow", "ObserverStance": "NonParticipantObservation", "Setting": "InSitu"},
        {"ElicitationSessionId": "elicit-policy-workshop", "Setting": "Office"},
        {"ElicitationSessionId": "elicit-policy-interview", "Setting": "Remote"},
    ],
    "KnowledgeMethods": [
        {"KnowledgeMethodId": "PolanyiTacitExplicitDistinction", "Label": "Polanyi's tacit/explicit distinction",
         "MethodFamily": "Classification", "Summary": "Classifies each knowledge item by whether its holder can articulate it (we know more than we can tell).",
         "OriginReference": "Polanyi, The Tacit Dimension, 1966", "SemanticTypeIri": f"{X}KnowledgeMethod",
         "ElicitationTradeoff": "Gives up nuance for tractability: a binary label on what is really a spectrum."},
        {"KnowledgeMethodId": "PractitionerInterview", "ElicitationTradeoff": "Gives up accuracy for practicality: an hour of talk, but practitioners describe what they believe they do."},
        {"KnowledgeMethodId": "Shadowing", "ElicitationTradeoff": "Gives up reach for depth and accuracy: one practitioner per shift, seen doing the real work."},
        {"KnowledgeMethodId": "FacilitatedWorkshop", "ElicitationTradeoff": "Gives up depth for reach: many roles at once, each heard briefly."},
        {"KnowledgeMethodId": "CriticalIncidentTechnique", "ElicitationTradeoff": "Gives up accuracy for practicality: rare events are recalled and reconstructed rather than witnessed."},
        {"KnowledgeMethodId": "ThinkAloudProtocol", "ElicitationTradeoff": "Gives up practicality and some accuracy for depth: speaking while working slows and alters the work."},
        {"KnowledgeMethodId": "RetrospectiveProtocol", "ElicitationTradeoff": "Gives up some accuracy for undisturbed practice: reasons are reconstructed after the fact."},
    ],
    "MethodApplications": R(f"{X}MethodApplication", ["MethodApplicationId", "KnowledgeMethod", "AppliedTo", "AppliedAt", "AppliedByAgent"],
        ("ma7-loto-value-stream", "ValueStreamAnalysis", "Lockout mapped end to end in el7-loto-workshop: day-to-night crew hand-off, waiting on operator release, re-isolation loop.", ts("04-15", "16:00"), SAM),
        ("ma7-loto-seci", "SeciConversion", "KnowledgeConversions for the lockout bleed cue typed by SECI mode, from shadowing to the v2 rulebook.", ts("05-18"), SAM),
        ("ma7-loto-polanyi", "PolanyiTacitExplicitDistinction", "Lockout fragments and knowledge holdings classified tacit or explicit before encoding loto-v2.0.0.", ts("05-19"), SAM),
    ),
}

KF_COLS = ["KnowledgeFragmentId", "ProcedureVersion", "Step", "KnowledgeForm", "Statement", "ElicitationSession", "SourceAgent",
           "Confidence", "ValidFrom", "Status", "OwnerRole", "EvaluationContext", "LastReviewedAt",
           "CognitiveBasis", "TacitnessDegree", "LostInTranslation", "EncodedAs", "StatedConditions"]
V2 = ts("05-20")
ROWS["KnowledgeFragments"] = R(f"{X}KnowledgeFragment", KF_COLS,
    ("kf7-loto-hiss", L2, "loto-04b", "Tacit", "Keep the valve closed and listen until the hiss dies completely before trusting the gauge.",
     "el7-loto-interview", "tomas-reyes", "High", V2, "Approved", SAFETY, EV, ts("07-01"), "PatternRecognition", 4,
     "The change in pitch that tells Tomas the bleed took cannot be written; the fragment carries only 'until the hiss dies'.",
     "ConditionedGuidance", "On pneumatic lines where the gauge sits downstream of the accumulator (press line 3 and retrofits)."),
    ("kf7-loto-mixer2", L2, "loto-04", "SituatedJudgment", "Always isolate both drives of mixer 2.",
     "el7-loto-cit", "victor-hale", "High", V2, "Approved", SAFETY, EV, None, "Judgment", 3,
     "Victor's account of why a retrofit counts as two machines was reduced to the name of one machine.", "HardRule", None),
    ("kf7-loto-bleed-twice", L2, "loto-04b", "Explicit", "Bleed the press 3 line twice.",
     None, SAM, "Medium", V2, "Approved", SAFETY, EV, None, "Articulated", 1,
     "The margin note gave no reason and none was sought.", "HardRule", None),
    ("kf7-loto-tag-handover", L2, "loto-08", "Implicit", "When a lockout spans a shift change, the crew that performs the walkdown removes the last tag and records the hand-off.",
     "el7-loto-workshop", "tomas-reyes", "High", V2, "Approved", SAFETY, EV, ts("07-01"), "Judgment", 2,
     "The crews' sense of whose job is finished when was settled by agreement, not observed.", "ConditionedGuidance", "Lockouts spanning a shift change."),
    ("kf7-loto-retrofit-override", L2, "loto-06", "Tacit", "On retrofits with a capacitor bank, a zero reading right after isolation is not yet zero; wait and re-test.",
     "el7-loto-retro", "tomas-reyes", "Medium", V2, "Reviewed", SAFETY, EV, None, "Intuition", 5,
     "Tomas could say when he distrusts the reading but not what makes him distrust it.", "CueNarrative", "Retrofitted machines with a capacitor bank."),
)

KG_COLS = ["KnowledgeGapId", "ProcedureVersion", "Step", "Statement", "Severity", "BlockingKind", "Status", "OwnerRole", "IdentifiedAt",
           "ResolutionPlan", "EvaluationContext", "GapCause", "HolderDeclinedToShare", "SiloedWithin", "DrawnOutBySession", "CodifiedAsFragment"]
ROWS["KnowledgeGaps"] = R(f"{X}KnowledgeGap", KG_COLS,
    ("gap7-press3-gatekept-bleed", L2, "loto-04b", "Only the press 3 day crew knew how to tell a bleed had taken, and would not show outsiders.",
     "High", "Blocking", "Resolved", SAFETY, ts("03-02"), "Draw it out through incidents the crew already tells.", EV,
     "Gatekeeping", True, "Press line 3 day crew", "el7-loto-cit", "kf7-loto-hiss"),
    ("gap7-retrofit-override-sabotage", L2, "loto-06", "The laminated override card for capacitor-bank retrofits was removed from the lockout binder after a dispute.",
     "High", "Blocking", "Open", SAFETY, ts("04-20"), "Recover the card's content from the technicians who used it.", EV,
     "Sabotage", True, "Retrofit maintenance crew", None, None),
    ("gap7-night-crew-tag-habit", L2, "loto-08", "The night crew will not say how they decide a day-crew tag can come off.",
     "Medium", "NonBlocking", "Investigating", SAFETY, ts("04-16"), "Shadow a night-shift walkdown.", EV,
     None, True, None, None, None),
    ("gap7-deploy-rollback-feel", D3, "deploy-04", "When to abandon a slow rollout is judged by feel and never written down.",
     "Medium", "NonBlocking", "Open", "release-manager", ts("06-22"), "Collect incidents from the last four rollouts.", EV,
     "Undocumented", False, None, None, None),
)

ROWS["PractitionerExpertise"] = R(f"{X}PractitionerExpertise",
    ["PractitionerExpertiseId", "Agent", "Procedure", "Step", "ElicitationSession", "ExpertKind", "Cue", "UnstatedBasis",
     "ReliableCallCount", "ForesightConfirmed"],
    ("pe7-tomas-bleed", "tomas-reyes", "lockout-tagout", "loto-04b", "el7-loto-interview", "Detection", "A bleed that did not take",
     "Knows before the gauge settles; cannot say what in the sound tells him.", 14, None),
    ("pe7-aisha-bleed", "aisha-bello", "lockout-tagout", "loto-04b", "el7-loto-shadow", "Detection", "A bleed that did not take",
     "Says the line 'sounds wrong' but cannot say how.", 1, None),
    ("pe7-ken-gauge", "ken-watanabe", "lockout-tagout", "loto-06", "el7-loto-thinkaloud", "Detection", "Residual pressure on mixer 2",
     None, 9, None),
    ("pe7-ravi-build-cache", "ravi-menon", "production-deployment", "deploy-01", "el7-deploy-interview", "DesignForesight",
     "The shared build cache design will be costly to maintain", "Said the design 'will fight us' but could not say why.", 0, True),
    ("pe7-omar-canary", "omar-haddad", "production-deployment", "deploy-04", "el7-deploy-interview", "DesignForesight",
     "Canary stages without a rollback owner will double on-call load", None, 0, True),
)

ROWS["CriticalIncidents"] = R(f"{X}CriticalIncident",
    ["CriticalIncidentId", "ProcedureVersion", "Step", "ElicitationSession", "Narrator", "OccurredAt", "Outcome", "Account",
     "RevealedJudgment", "JudgmentFragment"],
    ("ci7-mixer2-second-drive", L2, "loto-04", "el7-loto-cit", "victor-hale", "2025-11-12T02:40:00-05:00", "WentBadly",
     "The second drive of mixer 2 restarted the auger while the first was locked out.",
     "Treat any machine with a retrofitted drive as two machines to isolate.", "kf7-loto-mixer2"),
    ("ci7-press3-gauge-lied", L2, "loto-04b", "el7-loto-cit", "tomas-reyes", "2024-08-03T10:15:00-05:00", "Improvised",
     "The gauge read zero but the ram drifted; Tomas closed the valve again and waited for the hiss to stop.",
     "Do not trust the downstream gauge on press 3; listen for the bleed.", "kf7-loto-hiss"),
    ("ci7-conveyor5-handover", L2, "loto-08", "el7-loto-cit", "tomas-reyes", "2026-02-10T22:00:00-05:00", "WentWell",
     "A lockout spanning the shift change was handed over and released without incident.", None, None),
)

ROWS["InterviewProbes"] = R(f"{X}InterviewProbe",
    ["InterviewProbeId", "ElicitationSession", "Step", "ProbeKind", "Prompt", "Answer"],
    ("ip7-loto-why-wait", "el7-loto-interview", "loto-04b", "Why", "Why do you wait before bleeding the line a second time?",
     "The press 3 accumulator refills from the dryer for a few seconds after the valve closes."),
    ("ip7-loto-short-zero", "el7-loto-interview", "loto-06", "ProcedureFallsShort", "What do you do when the zero-energy check is not enough?",
     "On a retrofit with a capacitor bank I wait five minutes and test again before anyone reaches in."),
    ("ip7-loto-whatif-gauge", "el7-loto-interview", "loto-04b", "WhatIf", "What if the line had a working bleed indicator?",
     "I would still listen; indicators stick."),
    ("ip7-deploy-why-canary", "el7-deploy-interview", "deploy-04", "Why", "Why do you hold the canary longer on Fridays?",
     "Weekend traffic shifts the error baseline and a short canary hides regressions."),
)

ROWS["ObservedActions"] = R(f"{X}ObservedAction",
    ["ObservedActionId", "ElicitationSession", "Step", "Practitioner", "ActionDescription", "ActionKind", "MentionedInOwnAccount",
     "StatedReason", "CounterfactualCondition", "CounterfactualAnswer", "CapturedAsFragment"],
    ("oa7-hand-on-valve", "el7-loto-shadow", "loto-04b", "ken-watanabe", "Rests a hand on the valve body for a few seconds after bleeding.",
     "SmallChoice", False, "Feeling for vibration; the gauge lags the line.", "If the line had a working bleed indicator",
     "Would still wait ten seconds.", "kf7-loto-hiss"),
    ("oa7-tryout-button", "el7-loto-shadow", "loto-06", "ken-watanabe", "Presses the local start button to try the machine after locking out.",
     "OmittedObviousStep", False, "Everybody tries it; it is how you know.", "If the start button were remote",
     "Would radio the operator to try it.", None),
    ("oa7-spare-lock-cart", "el7-loto-shadow", "loto-05", "ken-watanabe", "Hangs a spare personal lock on the tool cart for a colleague who forgot his.",
     "Workaround", True, None, None, None, None),
    ("oa7-open-disconnect", "el7-loto-shadow", "loto-04a", "ken-watanabe", "Opens and locks the electrical disconnect.",
     "DocumentedStep", True, "It is the procedure.", None, None, None),
    ("oa7-close-fx-check", "elicit-close-shadow", "close-04", "devon-okafor", "Rechecks the FX rate before posting the adjustment.",
     "SmallChoice", False, "Rates move after four o'clock.", None, None, "kf-close-fx-time"),
)

ROWS["ElicitationParticipants"] = R(f"{X}ElicitationParticipant",
    ["ElicitationParticipantId", "ElicitationSession", "Agent", "ParticipationRole", "KnowledgeFlow", "ProcessStake", "IsUsuallyInvited"],
    ("ep7-lw-tomas", "el7-loto-workshop", "tomas-reyes", "Practitioner", "Producer", "Executes", True),
    ("ep7-lw-lin", "el7-loto-workshop", "lin-zhao", "SubjectMatterExpert", "Both", "Initiates", True),
    ("ep7-lw-sam", "el7-loto-workshop", SAM, "KnowledgeEngineer", "Consumer", None, True),
    ("ep7-lw-hana", "el7-loto-workshop", "hana-kowalski", "SubjectMatterExpert", "Consumer", "DependsOnResults", True),
    ("ep7-lw-rosa", "el7-loto-workshop", "rosa-delgado", "Practitioner", "Both", "DependsOnResults", False),
    ("ep7-pw-elena", "elicit-policy-workshop", "elena-garcia", "SubjectMatterExpert", "Producer", "Initiates", True),
    ("ep7-pw-amina", "elicit-policy-workshop", "amina-yusuf", "Practitioner", "Both", "Executes", True),
)

ROWS["RepresentationReviews"] = R(f"{X}RepresentationReview",
    ["RepresentationReviewId", "ProcedureVersion", "ReviewerAgent", "ReviewPurpose", "ReviewerIsAffectedSme", "Decision", "ReviewedAt"],
    ("rr7-loto-tomas", L2, "tomas-reyes", "RepresentationApproval", True, "Approved", ts("05-19", "10:00")),
    ("rr7-loto-lin", L2, "lin-zhao", "RepresentationApproval", True, "Approved", ts("05-19", "14:00")),
    ("rr7-deploy-ravi-ai", D3, "ravi-menon", "AiEvaluation", True, "Approved", ts("06-30", "11:00")),
    ("rr7-deploy-omar", D3, "omar-haddad", "RepresentationApproval", False, "Approved", ts("07-01", "09:00")),
)

ROWS["WorkflowViewDivergences"] = R(f"{X}WorkflowViewDivergence",
    ["WorkflowViewDivergenceId", "ElicitationSession", "ProcedureVersion", "Step", "ViewA", "HolderA", "ViewB", "HolderB",
     "ReconciledStatement", "ReconciledAt", "ReconciledIntoFragment"],
    ("wd7-loto-last-tag", "el7-loto-workshop", L2, "loto-08", "The day crew removes all of its tags at the end of its shift.", "tomas-reyes",
     "The night crew removes the last day-crew tag after its own walkdown.", "rosa-delgado",
     "The crew that performs the walkdown removes the last tag and records the hand-off.", ts("04-15", "16:30"), "kf7-loto-tag-handover"),
    ("wd7-loto-notify-timing", "el7-loto-workshop", L2, "loto-02", "Operators are notified before the machine is shut down.", "lin-zhao",
     "Operators are notified only once shutdown starts, so the line is not stopped early.", "hana-kowalski", None, None, None),
)

ROWS["ExpertCognitions"] = R(f"{X}ExpertCognition",
    ["ExpertCognitionId", "Agent", "Step", "ElicitationSession", "CognitionKind", "Statement", "AppliedAutomatically",
     "ExpertStatedExceptions", "CapturedExceptions"],
    ("ec7-energy-reservoirs", "tomas-reyes", "loto-04", "el7-loto-ladder", "MentalModel",
     "A machine is a set of energy reservoirs to drain, not a switch to turn off.", False, False, None),
    ("ec7-retrofit-two-machines", "victor-hale", "loto-04", "el7-loto-cit", "DecisionHeuristic",
     "If a machine was retrofitted, lock it out as two machines.", True, True, None),
    ("ec7-needle-stops", "ken-watanabe", "loto-06", "el7-loto-thinkaloud", "DecisionHeuristic",
     "Trust zero only once the gauge needle has stopped moving.", True, True,
     "Unless it is the old press 3 downstream gauge; then listen for the bleed instead."),
)

ROWS["ConceptLadderRungs"] = R(f"{X}ConceptLadderRung",
    ["ConceptLadderRungId", "ElicitationSession", "Step", "LadderLevel", "RungKind", "Statement"],
    ("cl7-bleed-up1", "el7-loto-ladder", "loto-04b", 1, "Goal", "No trapped air can move the clamp while hands are in the die."),
    ("cl7-bleed-up2", "el7-loto-ladder", "loto-04b", 2, "Value", "Nobody's hand is ever inside a machine that can move."),
    ("cl7-bleed-down1", "el7-loto-ladder", "loto-04b", -1, "Subprocess", "Close the supply valve at the drop, then open the bleed."),
    ("cl7-bleed-down2", "el7-loto-ladder", "loto-04b", -1, "Condition", "Only once the accumulator gauge reads below five psi."),
    ("cl7-lock-up1", "el7-loto-ladder", "loto-05", 1, "Value", "Each person keeps control of their own safety."),
)

ROWS["RepertoryGridConstructs"] = R(f"{X}RepertoryGridConstruct",
    ["RepertoryGridConstructId", "ElicitationSession", "Agent", "SituationsCompared", "PoleA", "PoleB", "WasStatedUnprompted",
     "SeparatesSituations"],
    ("rg7-retrofit-original", "el7-loto-grid", "tomas-reyes", "Mixer 2, press 3, conveyor 5 lockouts", "Retrofitted drive",
     "Original drive", False, True),
    ("rg7-audible-silent", "el7-loto-grid", "tomas-reyes", "Mixer 2, press 3, conveyor 5 lockouts", "You can hear it bleed",
     "It bleeds silently", False, True),
    ("rg7-large-small", "el7-loto-grid", "tomas-reyes", "Mixer 2, press 3, conveyor 5 lockouts", "Large machine", "Small machine",
     True, False),
)

ROWS["KnowledgeConversions"] = R(f"{X}KnowledgeConversion",
    ["KnowledgeConversionId", "ElicitationSession", "ResultFragment", "ConversionMode", "FromForm", "ToForm", "Description", "OccurredAt"],
    ("kc7-socialize-hand-on-valve", "el7-loto-shadow", None, "Socialization", "Tacit", "Tacit",
     "Ken picked up the hand-on-valve pause by working beside Tomas for years.", ts("03-17", "12:00")),
    ("kc7-externalize-hiss", "el7-loto-interview", "kf7-loto-hiss", "Externalization", "Tacit", "Explicit",
     "Tomas's bleed cue stated as a conditioned fragment.", ts("03-10", "11:00")),
    ("kc7-combine-into-v2", None, "kf7-loto-hiss", "Combination", "Explicit", "Explicit",
     "The hiss fragment combined with SOP step 4b into loto-v2.0.0.", ts("05-20")),
    ("kc7-internalize-aisha", None, "kf7-loto-hiss", "Internalization", "Explicit", "Tacit",
     "Aisha rehearsed the v2 bleed check until it became habit.", ts("06-15")),
    ("kc7-retyped-note", None, "kf7-loto-bleed-twice", "Externalization", "Explicit", "Explicit",
     "The press 3 manual margin note retyped into the rulebook was logged as an externalization.", ts("05-20")),
)

L1 = "loto-v1.0.0"
ROWS["KnowledgeHoldings"] = R(f"{X}KnowledgeHolding",
    ["KnowledgeHoldingId", "ProcedureVersion", "Step", "Snapshot", "KnowledgeForm", "Carrier", "HolderAgent", "Statement",
     "IsUnsaidInSop", "HolderIsVeteran", "HandlesExceptionOrDiscretion", "FormalizedAs"],
    ("kh7-before-bleed-cue", L1, "loto1-03", "Before", "Tacit", "OperatorMemory", "tomas-reyes",
     "How to tell a pneumatic bleed has taken.", True, True, True, None),
    ("kh7-before-twin-drive", L1, "loto1-03", "Before", "Tacit", "DepartingStaffMemory", "victor-hale",
     "Retrofitted mixers have a second drive that must be isolated separately.", True, True, True, None),
    ("kh7-before-margin-note", L1, "loto1-03", "Before", "Explicit", "OutdatedManualNote", None,
     "Handwritten note in the 2009 press manual: bleed line 3 twice.", True, False, False, None),
    ("kh7-before-sop", L1, "loto1-01", "Before", "Explicit", "FormalDocument", None,
     "Four-step lockout SOP: shut down, isolate, lock, verify.", False, False, False, None),
    ("kh7-before-last-tag", L1, "loto1-04", "Before", "Tacit", "UnspokenCrewAgreement", None,
     "The night crew takes the last day-crew tag off after its walkdown; nobody ever agreed to it aloud.", True, False, True, None),
    ("kh7-after-bleed-cue", L2, "loto-04b", "After", "Explicit", "ProcedureRulebook", "tomas-reyes",
     "Bleed cue with its conditions, in the v2 rulebook.", False, True, True, "kf7-loto-hiss"),
    ("kh7-after-twin-drive", L2, "loto-04", "After", "Explicit", "ProcedureRulebook", "victor-hale",
     "Mixer 2 twin-drive isolation, in the v2 rulebook.", False, True, True, "kf7-loto-mixer2"),
    ("kh7-after-margin-note", L2, "loto-04b", "After", "Explicit", "ProcedureRulebook", None,
     "Bleed press 3 twice, retyped from the manual note.", False, False, False, "kf7-loto-bleed-twice"),
    ("kh7-after-last-tag", L2, "loto-08", "After", "Explicit", "ProcedureRulebook", None,
     "Walkdown crew removes the last tag and records the hand-off.", False, False, True, "kf7-loto-tag-handover"),
    ("kh7-after-retrofit-zero", L2, "loto-06", "After", "Tacit", "OperatorMemory", "tomas-reyes",
     "When to distrust a zero reading on a capacitor-bank retrofit.", True, True, True, None),
)

ROWS["FragmentCorroborations"] = R(f"{X}FragmentCorroboration",
    ["FragmentCorroborationId", "KnowledgeFragment", "ElicitationSession", "Agent", "Agrees"],
    ("fc7-hiss-shadow", "kf7-loto-hiss", "el7-loto-shadow", "ken-watanabe", True),
    ("fc7-hiss-incident", "kf7-loto-hiss", "el7-loto-cit", "tomas-reyes", True),
    ("fc7-last-tag-workshop", "kf7-loto-tag-handover", "el7-loto-workshop", "rosa-delgado", True),
    ("fc7-mixer2-ken-disagrees", "kf7-loto-mixer2", "el7-loto-thinkaloud", "ken-watanabe", False),
)

ROWS["KnowledgeTestOutcomes"] = R(f"{X}KnowledgeTestOutcome",
    ["KnowledgeTestOutcomeId", "Procedure", "OutcomeArea", "Measure", "BaselineValue", "AfterTestValue", "HigherIsBetter"],
    ("kt7-loto-competency-gaps", "lockout-tagout", "CompetencyGaps", "Technicians failing at least one lockout question", 7, 2, False),
    ("kt7-loto-quality", "lockout-tagout", "ProcessQuality", "Lockout audit pass rate (%)", 82, 95, True),
    ("kt7-loto-efficiency", "lockout-tagout", "Efficiency", "Median minutes per lockout", 41, 33, False),
    ("kt7-loto-satisfaction", "lockout-tagout", "JobSatisfaction", "Technician survey score (1-5)", 3.6, 3.6, True),
    ("kt7-loto-consistency", "lockout-tagout", "Consistency", "Distinct step orders observed across technicians", 5, 2, False),
)

MAPPINGS = [
    ("map7-kh-holder", "KnowledgeHoldings.HolderAgent", "property", "http://www.w3.org/ns/prov#wasAttributedTo", "aligned",
     "prov-o", "Loop 7: who holds a piece of process knowledge."),
    ("map7-ci-session", "CriticalIncidents.ElicitationSession", "property", "http://www.w3.org/ns/prov#wasGeneratedBy", "aligned",
     "prov-o", "Loop 7: the incident record was generated by the elicitation session."),
]
