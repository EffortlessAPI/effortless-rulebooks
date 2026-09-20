"""Loop 13 (theme H): provenance and traceability.

Part II of the PKM series asks that every process model, activity definition and constraint trace to
the collection that produced it, that a change to a source reveal the organized knowledge resting on
it, and that the trace make fidelity checkable. It treats documents as a starting point that
elicitation validates and extends, and process mining as a partial aid: it rebuilds the actual flow,
its deviations and bottlenecks, but holds no reasoning or tacit knowledge. Part I asks for the
provenance of every knowledge item (origin, route, confirmation) and for conflicting stakeholder
perspectives to be kept with their sources. Part III asks for an improvement cycle in which a failure
is recorded and fed into a redesign, and warns that compliance-driven standardization does not supply
the tacit knowledge a procedure needs.

The scenario: the deployment runbook was revised after it was excerpted, the CI log shows hotfixes
rolling out with no approval event while the release gate waits a day for approvers, a press-7
retrofit drawing changed after the bleed-down step was taken from it, and the forklift inspection is
standardized for compliance with no tacit knowledge captured behind it.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import agg, calc, idx, lookup, raw, rel  # noqa: E402

EXT = "https://effortlessapi.github.io/effortless-rulebooks/ns/pko-extension#"
PROV = "http://www.w3.org/ns/prov#"
EV = "eval-current"

LOOP = {
    "WitnessLoopId": "loop-13",
    "LoopNumber": 13,
    "Title": "Loop 13: provenance, traceability, process mining and the document-practice gap",
    "Premise": "Earlier loops recorded collected source material, elicitation sessions, knowledge fragments with a "
               "source agent, and process-mining runs as variant counts. What the model could not say was which "
               "collected material each process model, step and constraint came from and in what role, whether "
               "that source has changed since, who derived and who confirmed each item, where the mined flow "
               "departs from the documented one and where it waits, whether documents trail practice, and "
               "whether failures, feedback and recurring reviews actually flow back into collection. This loop "
               "adds each of those, with seeded breaches on the deployment and lockout scenarios.",
}

KE, STEWARD, AUTH, SAFETY, RELEASE, AUDITOR, OPS = (
    "knowledge-engineer", "process-steward", "knowledge-authority", "plant-safety-officer", "release-manager",
    "internal-compliance-auditor", "plant-operations-manager")


def T(name, desc, area, *fields):
    return (name, desc, area, list(fields))


def pk(name):
    return raw(name, "string", "Stored identifier.", nullable=False)


def iri():
    return raw("SemanticTypeIri", "string", "Semantic type IRI.")


def F(table, *names):
    return [f"{table}.{n}" for n in names]


CSM = "CollectedSourceMaterials"

TABLES = [
    T("KnowledgeTraces",
      "One trace from a piece of organized knowledge (a process model, an activity definition or a constraint) to the "
      "collected material it rests on, with the role the material plays (origin, validation, extension, contradiction), "
      "how the knowledge was derived, and who confirmed it.", "provenance",
      pk("KnowledgeTraceId"),
      calc("Name", "string", "Display name.", '={{TargetKind}} & " <- " & {{SourceMaterial}} & " (" & {{TraceRole}} & ")"'),
      raw("TargetKind", "string", "ProcessModel, ActivityDefinition or Constraint."),
      rel("ProcedureVersion", "ProcedureVersions", "The process model the traced knowledge belongs to."),
      rel("Step", "Steps", "The activity definition traced, when the target is a step."),
      rel("Requirement", "Requirements", "The constraint traced, when the target is a requirement."),
      rel("SourceMaterial", CSM, "The collected material the knowledge traces to."),
      raw("TraceRole", "string", "Origin (knowledge was taken from it), Validates, Extends, or Contradicts."),
      raw("TracedAspect", "string", "What the source is cited for: Sequence, StepDescription, Workaround, ActualExecution or Constraint."),
      raw("SourceStatement", "string", "Paraphrase of what the source says about the target."),
      raw("DerivationRoute", "string", "How the knowledge was produced from the source."),
      rel("DerivedByAgent", "Agents", "Who (or what) derived the knowledge from the source."),
      rel("ValidatedByAgent", "Agents", "Who confirmed the derived knowledge."),
      raw("ValidatedAt", "datetime", "When it was confirmed."),
      raw("SourceStatedDurationMinutes", "integer", "Duration the source states for the traced step, when it states one."),
      rel("ContradictedDocument", "Resources", "For a contradiction: the document whose prescription practice departs from."),
      lookup("SourceMaterialKind", "string", "Kind of the source material.", idx(CSM, "MaterialKind", "SourceMaterial")),
      lookup("SourceCollectedAt", "datetime", "When the knowledge was taken: the material's collection instant.", idx(CSM, "CollectedAt", "SourceMaterial")),
      lookup("SourceRevisedAt", "datetime", "Last revision of the document the material was excerpted from.", idx(CSM, "SourceDocumentRevisedAt", "SourceMaterial")),
      lookup("SourceIsDocument", "boolean", "TRUE when the source is an excerpt of an existing document.", idx(CSM, "IsDocumentSource", "SourceMaterial")),
      lookup("SourceIsPeopleCapture", "boolean", "TRUE when the source was captured from people (transcript or field notes).", idx(CSM, "IsPeopleCapture", "SourceMaterial")),
      lookup("SourceIsPracticeEvidence", "boolean", "TRUE when the source records work as performed (field notes or an event trace).", idx(CSM, "IsPracticeEvidence", "SourceMaterial")),
      calc("IsSourceChangedSinceTaken", "boolean", "TRUE when the source document was revised after the knowledge was taken from it.",
           '=AND({{SourceRevisedAt}} <> "", {{SourceRevisedAt}} > {{SourceCollectedAt}})'),
      lookup("ModeledDurationMinutes", "integer", "Duration the modeled step carries.", idx("Steps", "ExpectedDurationMinutes", "Step")),
      calc("IsUnfaithfulToSource", "boolean", "TRUE when the modeled step's duration differs from the duration its source states.",
           '=AND({{Step}} <> "", {{SourceStatedDurationMinutes}} > 0, {{ModeledDurationMinutes}} <> {{SourceStatedDurationMinutes}})'),
      lookup("DerivedByAgentKind", "string", "Kind of the deriving agent.", idx("Agents", "AgentKind", "DerivedByAgent")),
      calc("IsMachineDerived", "boolean", "TRUE when a software or AI agent derived the knowledge from its source.",
           '=AND({{DerivedByAgent}} <> "", {{DerivedByAgentKind}} <> "Human")'),
      calc("IsSelfValidated", "boolean", "TRUE when the agent who derived the knowledge is the one who confirmed it.",
           '=AND({{ValidatedByAgent}} <> "", {{ValidatedByAgent}} = {{DerivedByAgent}})'),
      calc("HasIncompleteProvenance", "boolean", "TRUE when the origin, the derivation route or the confirmation is missing.",
           '=OR({{SourceMaterial}} = "", {{DerivationRoute}} = "", {{ValidatedByAgent}} = "")'),
      calc("ProvenanceStatement", "string", "Origin, route and confirmation in one line.",
           '="From " & {{SourceMaterial}} & "; route: " & {{DerivationRoute}} & "; derived by " & {{DerivedByAgent}} & "; confirmed by " & {{ValidatedByAgent}}'),
      calc("IsAspectUnsupportedBySourceKind", "boolean",
           "TRUE when a workaround is cited to something other than observation notes, or actual execution to something other than an event trace.",
           '=OR(AND({{TracedAspect}} = "Workaround", {{SourceMaterialKind}} <> "FieldNotes"), AND({{TracedAspect}} = "ActualExecution", {{SourceMaterialKind}} <> "MinedEventTrace"))'),
      calc("IsDocumentOrigin", "boolean", "TRUE when the knowledge was first taken from a document.",
           '=AND({{TraceRole}} = "Origin", {{SourceIsDocument}})'),
      lookup("StepElicitedValidationCount", "integer", "Validations of the traced step captured from people.", idx("Steps", "ElicitedValidationCount", "Step")),
      lookup("StepElicitedExtensionCount", "integer", "Extensions of the traced step captured from people.", idx("Steps", "ElicitedExtensionCount", "Step")),
      calc("IsDocumentStartNeverValidated", "boolean", "TRUE when a step taken from a document has never been confirmed by capture from people.",
           '=AND({{IsDocumentOrigin}}, {{Step}} <> "", {{StepElicitedValidationCount}} = 0)'),
      calc("IsDocumentStartNeverExtended", "boolean", "TRUE when a step taken from a document has never been extended by capture from people.",
           '=AND({{IsDocumentOrigin}}, {{Step}} <> "", {{StepElicitedExtensionCount}} = 0)'),
      lookup("ContradictedDocumentRevisedAt", "datetime", "Last revision of the contradicted document.", idx("Resources", "ModifiedAt", "ContradictedDocument")),
      calc("IsDocumentTrailingPractice", "boolean", "TRUE when practice evidence collected after the document's last revision contradicts it.",
           '=AND({{TraceRole}} = "Contradicts", {{ContradictedDocument}} <> "", {{SourceIsPracticeEvidence}}, {{SourceCollectedAt}} > {{ContradictedDocumentRevisedAt}})'),
      calc("PrescribedVersusEnacted", "string", "For a contradiction: what the document prescribes against what practice evidence shows.",
           '=IF({{ContradictedDocument}} = "", "", "Prescribed in " & {{ContradictedDocument}} & "; enacted per " & {{SourceMaterial}} & ": " & {{SourceStatement}})'),
      iri()),
]

TABLES += [
    T("MinedFlowEdges",
      "One step-to-step hand-off reconstructed from a system event log by a process-mining run: how many cases took it, "
      "how long cases waited before the next step, and whether it is kept as observed behavior or recorded as intended logic.",
      "provenance",
      pk("MinedFlowEdgeId"),
      calc("Name", "string", "Display name.", '={{FromStep}} & " -> " & {{ToStep}}'),
      rel("ProcessMiningRun", "ProcessMiningRuns", "The mining run that reconstructed the hand-off."),
      rel("FromStep", "Steps", "Step the cases left."),
      rel("ToStep", "Steps", "Step the cases entered next."),
      raw("ObservedCaseCount", "integer", "Cases in the log that took this hand-off."),
      raw("MedianWaitMinutes", "integer", "Median minutes between leaving FromStep and starting ToStep."),
      raw("RecordedStance", "string", "Observed (kept as mined behavior) or Intended (recorded as intended process logic)."),
      rel("IntentDecisionBy", "Agents", "Who decided that mined behavior should become intended logic."),
      agg("DocumentedTransitionCount", "integer", "Documented transitions with the same origin and target.",
          "=COUNTIFS(StepTransitions!{{FromStep}}, {{FromStep}}, StepTransitions!{{ToStep}}, {{ToStep}})"),
      calc("IsUndocumentedPath", "boolean", "TRUE when cases took a hand-off the documented model does not contain.",
           "={{DocumentedTransitionCount}} = 0"),
      lookup("ToStepExpectedMinutes", "integer", "Documented duration of the step entered.", idx("Steps", "ExpectedDurationMinutes", "ToStep")),
      calc("IsBottleneck", "boolean", "TRUE when cases wait more than four times the entered step's documented duration.",
           "=AND({{ToStepExpectedMinutes}} > 0, {{MedianWaitMinutes}} > 4 * {{ToStepExpectedMinutes}})"),
      calc("IsMinedPathRecordedAsIntentWithoutDecision", "boolean",
           "TRUE when a mined hand-off is recorded as intended logic and nobody decided it should be.",
           '=AND({{RecordedStance}} = "Intended", {{IntentDecisionBy}} = "")'),
      iri()),
    T("CollectionOccasions",
      "Recurring occasions at which process knowledge is collected: retrospectives, process reviews and improvement initiatives.",
      "provenance",
      pk("CollectionOccasionId"),
      calc("Name", "string", "Display name.", '={{OccasionKind}} & ": " & {{Label}}'),
      raw("Label", "string", "The occasion."),
      raw("OccasionKind", "string", "Retrospective, ProcessReview or ImprovementInitiative."),
      rel("Procedure", "Procedures", "The procedure the occasion collects knowledge about."),
      raw("CadenceDays", "integer", "Days between occurrences."),
      raw("LastHeldAt", "datetime", "When it was last held."),
      rel("EvaluationContext", "EvaluationContexts", "The instant the row is judged against."),
      lookup("AsOfInstant", "datetime", "The modeled evaluation instant.", idx("EvaluationContexts", "AsOfInstant", "EvaluationContext")),
      calc("DaysSinceHeld", "integer", "Days since the occasion was last held.",
           '=IF({{LastHeldAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{LastHeldAt}}, "days"))'),
      calc("IsLapsed", "boolean", "TRUE when the occasion has not been held within its cadence.", "={{DaysSinceHeld}} > {{CadenceDays}}"),
      agg("CapturedMaterialCount", "integer", "Collected materials captured at the occasion.",
          f"=COUNTIFS({CSM}!{{{{CollectedAtOccasion}}}}, {{{{CollectionOccasionId}}}})"),
      calc("IsHeldWithoutCapture", "boolean", "TRUE when the occasion was held and nothing it surfaced was collected.",
           '=AND({{LastHeldAt}} <> "", {{CapturedMaterialCount}} = 0)'),
      iri()),
    T("StakeholderPerspectives",
      "One stakeholder's view of how a step should be performed, kept with the collected material it comes from and the "
      "perspective it conflicts with, rather than merged into a single agreed version.", "provenance",
      pk("StakeholderPerspectiveId"),
      calc("Name", "string", "Display name.", '={{HolderRole}} & " on " & {{Step}}'),
      rel("ProcedureVersion", "ProcedureVersions", "The process model the view is about."),
      rel("Step", "Steps", "The step the view is about."),
      rel("HolderRole", "Roles", "The role whose view this is."),
      raw("Position", "string", "The view, paraphrased."),
      rel("SourceMaterial", CSM, "The collected material the view comes from."),
      rel("ConflictsWithPerspective", "StakeholderPerspectives", "The perspective this one conflicts with."),
      raw("Disposition", "string", "Retained (kept alongside the other views) or OverriddenByConsensus."),
      lookup("SourceMaterialKind", "string", "Kind of material the view comes from.", idx(CSM, "MaterialKind", "SourceMaterial")),
      agg("ConflictPartnerCount", "integer", "Perspectives that name this one as their conflict.",
          "=COUNTIFS(StakeholderPerspectives!{{ConflictsWithPerspective}}, {{StakeholderPerspectiveId}})"),
      calc("IsInConflict", "boolean", "TRUE when the perspective conflicts with another in either direction.",
           '=OR({{ConflictsWithPerspective}} <> "", {{ConflictPartnerCount}} > 0)'),
      calc("IsDissentingViewNotKeptWithSource", "boolean",
           "TRUE when a conflicting view was overridden into one version or kept without the source it comes from.",
           '=AND({{IsInConflict}}, OR({{Disposition}} = "OverriddenByConsensus", {{SourceMaterial}} = ""))'),
      iri()),
]

KT = "KnowledgeTraces"

FIELDS = {
    CSM: [
        rel("SourceDocument", "Resources", "The existing document the material was excerpted from."),
        lookup("SourceDocumentRevisedAt", "datetime", "Last revision of that document.", idx("Resources", "ModifiedAt", "SourceDocument")),
        calc("IsDocumentSource", "boolean", "TRUE for an excerpt of existing written material.", '={{MaterialKind}} = "DocumentExcerpt"'),
        calc("IsPeopleCapture", "boolean", "TRUE for material captured from people: a transcript or field notes.",
             '=OR({{MaterialKind}} = "Transcript", {{MaterialKind}} = "FieldNotes")'),
        calc("IsPracticeEvidence", "boolean", "TRUE for material that records the work as performed: field notes or a mined event trace.",
             '=OR({{MaterialKind}} = "FieldNotes", {{MaterialKind}} = "MinedEventTrace")'),
        raw("HoldsReasoningOrTacitKnowledge", "boolean", "TRUE when the material holds the reasoning or tacit knowledge behind the actions, not only the actions."),
        rel("ComplementsMiningRun", "ProcessMiningRuns", "The mining run whose findings this capture from people explains."),
        rel("CapturedDuringExecution", "ProcedureExecutions", "The execution during which the material was captured, when captured in the course of work."),
        calc("IsCapturedInFlowOfWork", "boolean", "TRUE when the material was captured while the work was being done.",
             '={{CapturedDuringExecution}} <> ""'),
        rel("CollectedAtOccasion", "CollectionOccasions", "The recurring occasion at which the material was collected."),
        rel("PromptedByFeedback", "UserFeedback", "The practitioner feedback that prompted this collection."),
        rel("ProducedByMethodApplication", "MethodApplications", "The method application that produced the material."),
        rel("ContributingExpert", "Agents", "The domain expert whose time the capture consumed."),
        raw("ExpertEffortHours", "number", "Hours of expert time the capture consumed."),
        agg("DependentTraceCount", "integer", "Organized knowledge traced to this material.",
            f"=COUNTIFS({KT}!{{{{SourceMaterial}}}}, {{{{CollectedSourceMaterialId}}}})"),
        calc("IsDependencyInvisibleToChange", "boolean",
             "TRUE when the material was encoded into a model but no trace records what rests on it, so a change to it reveals nothing.",
             '=AND({{EncodedIntoVersion}} <> "", {{DependentTraceCount}} = 0)'),
        agg("ChangedDependentCount", "integer", "Traced knowledge taken before the source document was last revised.",
            f"=COUNTIFS({KT}!{{{{SourceMaterial}}}}, {{{{CollectedSourceMaterialId}}}}, {KT}!{{{{IsSourceChangedSinceTaken}}}}, TRUE)"),
        calc("HasKnowledgeAffectedBySourceChange", "boolean", "TRUE when a revision of the source affects organized knowledge taken from it.",
             "={{ChangedDependentCount}} > 0"),
    ],
    "Steps": [
        agg("CollectionEvidenceCount", "integer", "Traces from this step to collected material, in any role.",
            f"=COUNTIFS({KT}!{{{{Step}}}}, {{{{StepId}}}})"),
        calc("HasCollectionEvidence", "boolean", "TRUE when the step traces to at least one collected material.", "={{CollectionEvidenceCount}} > 0"),
        agg("ActivityOriginTraceCount", "integer", "Origin traces of this step as an activity definition.",
            f'=COUNTIFS({KT}!{{{{Step}}}}, {{{{StepId}}}}, {KT}!{{{{TargetKind}}}}, "ActivityDefinition", {KT}!{{{{TraceRole}}}}, "Origin")'),
        lookup("VersionModelTraceCount", "integer", "Origin traces of the step's process model.", idx("ProcedureVersions", "ProcessModelTraceCount", "ProcedureVersion")),
        calc("IsUntracedActivityInTracedModel", "boolean",
             "TRUE when the step belongs to a traced process model but its own definition traces to no collection.",
             "=AND({{VersionModelTraceCount}} > 0, {{ActivityOriginTraceCount}} = 0)"),
        agg("ElicitedValidationCount", "integer", "Validations of this step captured from people.",
            f'=COUNTIFS({KT}!{{{{Step}}}}, {{{{StepId}}}}, {KT}!{{{{TraceRole}}}}, "Validates", {KT}!{{{{SourceIsPeopleCapture}}}}, TRUE)'),
        agg("ElicitedExtensionCount", "integer", "Extensions of this step captured from people.",
            f'=COUNTIFS({KT}!{{{{Step}}}}, {{{{StepId}}}}, {KT}!{{{{TraceRole}}}}, "Extends", {KT}!{{{{SourceIsPeopleCapture}}}}, TRUE)'),
    ],
    "ProcedureVersions": [
        agg("ProcessModelTraceCount", "integer", "Origin traces of the version as a process model.",
            f'=COUNTIFS({KT}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KT}!{{{{TargetKind}}}}, "ProcessModel", {KT}!{{{{TraceRole}}}}, "Origin")'),
        calc("IsLiveModelUntraced", "boolean", "TRUE when an executable process model traces to no collection activity.",
             "=AND({{IsLive}}, {{ProcessModelTraceCount}} = 0)"),
        agg("TrailingPracticeTraceCount", "integer", "Contradictions showing a document of this version trailing practice.",
            f"=COUNTIFS({KT}!{{{{ProcedureVersion}}}}, {{{{ProcedureVersionId}}}}, {KT}!{{{{IsDocumentTrailingPractice}}}}, TRUE)"),
        calc("IsDocumentedBehindPractice", "boolean", "TRUE when the version's documentation trails how the work is performed.",
             "={{TrailingPracticeTraceCount}} > 0"),
        raw("StandardizationDriver", "string", "Why the version was standardized: Compliance or Practice."),
        agg("TacitFragmentCount", "integer", "Tacit knowledge fragments captured for the version.",
            '=COUNTIFS(KnowledgeFragments!{{ProcedureVersion}}, {{ProcedureVersionId}}, KnowledgeFragments!{{KnowledgeForm}}, "Tacit")'),
        calc("IsStandardizedWithoutTacitCapture", "boolean",
             "TRUE when a version standardized for compliance has no tacit knowledge captured behind it.",
             '=AND({{StandardizationDriver}} = "Compliance", {{TacitFragmentCount}} = 0)'),
    ],
    "Requirements": [
        agg("ConstraintTraceCount", "integer", "Traces of this requirement as a constraint.",
            f'=COUNTIFS({KT}!{{{{Requirement}}}}, {{{{RequirementId}}}}, {KT}!{{{{TargetKind}}}}, "Constraint")'),
        calc("IsUntracedBoundConstraint", "boolean", "TRUE when a constraint bound to a step traces to no collection activity.",
             "=AND({{IsBoundToAnyStep}}, {{ConstraintTraceCount}} = 0)"),
    ],
    "Resources": [
        agg("TrailingPracticeCount", "integer", "Practice evidence, collected after the document's last revision, that contradicts it.",
            f"=COUNTIFS({KT}!{{{{ContradictedDocument}}}}, {{{{ResourceId}}}}, {KT}!{{{{IsDocumentTrailingPractice}}}}, TRUE)"),
        calc("IsBehindCurrentPractice", "boolean", "TRUE when the document has fallen behind how the work is currently done.",
             "={{TrailingPracticeCount}} > 0"),
    ],
}

FIELDS.update({
    "ProcessMiningRuns": [
        agg("PeopleCaptureComplementCount", "integer", "Captures from people that hold the reasoning behind this run's findings.",
            f"=COUNTIFS({CSM}!{{{{ComplementsMiningRun}}}}, {{{{ProcessMiningRunId}}}}, {CSM}!{{{{HoldsReasoningOrTacitKnowledge}}}}, TRUE)"),
        calc("IsDeviationUnexplainedByPeople", "boolean",
             "TRUE when mining found non-conforming variants and no capture from experienced workers explains them.",
             "=AND({{DiscoveredVariantCount}} > {{ConformingVariantCount}}, {{PeopleCaptureComplementCount}} = 0)"),
        agg("UndocumentedPathCount", "integer", "Mined hand-offs the documented model does not contain.",
            "=COUNTIFS(MinedFlowEdges!{{ProcessMiningRun}}, {{ProcessMiningRunId}}, MinedFlowEdges!{{IsUndocumentedPath}}, TRUE)"),
        calc("HasUndocumentedEnactedPath", "boolean", "TRUE when the enacted flow the run reconstructed contains a path the documentation lacks.",
             "={{UndocumentedPathCount}} > 0"),
    ],
    "IssueOccurrences": [
        rel("RedesignChangeRequest", "ChangeRequests", "The change request that redesigned the procedure in response to the failure."),
        lookup("RedesignImplementedAt", "datetime", "When that redesign was implemented.", idx("ChangeRequests", "ImplementedAt", "RedesignChangeRequest")),
        calc("IsFailureWithoutLandedRedesign", "boolean", "TRUE when a recorded failure has no redesign, or its redesign never landed.",
             '=OR({{RedesignChangeRequest}} = "", {{RedesignImplementedAt}} = "")'),
        calc("ImprovementCyclePath", "string", "Failure observed, how it was recorded, and the redesign that followed.",
             '=IF({{RedesignChangeRequest}} = "", "", "Observed at " & {{ExecutedStep}} & " by " & {{EncounteredByAgent}} & "; recorded as " & {{Error}} & " (" & {{IssueCause}} & "); redesigned by " & {{RedesignChangeRequest}})'),
    ],
    "UserFeedback": [
        raw("RevealsTacitKnowledge", "boolean", "TRUE when the feedback surfaces know-how the procedure does not hold."),
        agg("CollectionFollowUpCount", "integer", "Collected materials this feedback prompted.",
            f"=COUNTIFS({CSM}!{{{{PromptedByFeedback}}}}, {{{{UserFeedbackId}}}})"),
        calc("IsTacitSignalNotFedIntoCollection", "boolean",
             "TRUE when feedback that surfaces tacit know-how prompted no collection.",
             "=AND({{RevealsTacitKnowledge}}, {{CollectionFollowUpCount}} = 0)"),
    ],
    "Procedures": [
        agg("CollectedMaterialCount", "integer", "Materials collected about the procedure.",
            f"=COUNTIFS({CSM}!{{{{Procedure}}}}, {{{{ProcedureId}}}})"),
        agg("InWorkCaptureCount", "integer", "Of those, materials captured in the course of executing the work.",
            f"=COUNTIFS({CSM}!{{{{Procedure}}}}, {{{{ProcedureId}}}}, {CSM}!{{{{IsCapturedInFlowOfWork}}}}, TRUE)"),
        calc("IsCaptureSeparateFromWork", "boolean", "TRUE when knowledge about the procedure is collected, but never in the course of the work.",
             "=AND({{CollectedMaterialCount}} > 0, {{InWorkCaptureCount}} = 0)"),
        agg("ExpertAcquisitionHours", "number", "Expert hours spent capturing knowledge about the procedure.",
            f"=SUMIFS({CSM}!{{{{ExpertEffortHours}}}}, {CSM}!{{{{Procedure}}}}, {{{{ProcedureId}}}})"),
    ],
})

QUESTIONS = [
    # ------------------------------------------------------------------ traceability to collection
    ("aq-pkm2-q12", KE,
     "For this step, which collected material does it trace to: which transcript, field notes, document excerpt or event trace?",
     "A step that traces to nothing cannot be checked, defended or updated when its source changes.",
     F(KT, "TargetKind", "ProcedureVersion", "Step", "Requirement", "SourceMaterial", "TraceRole", "SourceMaterialKind")
     + F("Steps", "CollectionEvidenceCount", "HasCollectionEvidence")),
    ("q13-knowledge-engineer-models-trace-to-collection", KE,
     "Does every process model, every step definition and every constraint we encoded trace back to the collection activity that produced it?",
     "Organized knowledge with no trace to a collection activity is an assertion nobody can audit.",
     F("ProcedureVersions", "ProcessModelTraceCount", "IsLiveModelUntraced")
     + F("Steps", "ActivityOriginTraceCount", "VersionModelTraceCount", "IsUntracedActivityInTracedModel")
     + F("Requirements", "ConstraintTraceCount", "IsUntracedBoundConstraint")),
    ("aq-pkm2-q13", STEWARD,
     "Has the document this knowledge was taken from been revised since we took it?",
     "Knowledge taken from a source that has since changed may no longer say what the source says.",
     F(CSM, "SourceDocumentRevisedAt") + F(KT, "SourceCollectedAt", "SourceRevisedAt", "IsSourceChangedSinceTaken")),
    ("aq-pkm2-q14", STEWARD,
     "This source was revised: which process models, steps and constraints that we organized from it are affected?",
     "Without the dependency, a revised source silently leaves stale knowledge in the model.",
     F(CSM, "ChangedDependentCount", "HasKnowledgeAffectedBySourceChange")),
    ("q13-process-steward-change-visibility", STEWARD,
     "Which collected materials did we encode into a model without recording what rests on them, so that a change to them would reveal nothing?",
     "A dependency that was never recorded cannot be revealed when the source changes.",
     F(CSM, "SourceDocument", "DependentTraceCount", "IsDependencyInvisibleToChange")),
    ("q13-knowledge-authority-fidelity", AUTH,
     "Where does a modeled step say something different from the source it was taken from?",
     "Traceability is only worth keeping if it lets us check the model still represents its sources faithfully.",
     F(KT, "SourceStatedDurationMinutes", "ModeledDurationMinutes", "IsUnfaithfulToSource")),
    ("q13-knowledge-engineer-evidence-kind", KE,
     "Is each claim cited to the kind of evidence that can support it: interviews for how a step is described, observation for workarounds, logs for what really ran?",
     "A claim about real execution that rests on an interview is hearsay, however confident the speaker.",
     F(KT, "TracedAspect", "SourceIsPracticeEvidence", "IsAspectUnsupportedBySourceKind") + F(CSM, "IsPracticeEvidence")),
    # ------------------------------------------------------------------ provenance of each item
    ("aq-pkm1-q08", AUDITOR,
     "For this knowledge item, where did it come from, how was it produced, and who confirmed it?",
     "An auditor cannot rely on an item whose origin, route or confirmation is missing.",
     F(KT, "SourceStatement", "ProvenanceStatement", "HasIncompleteProvenance")),
    ("aq-pkm2-q38", AUTH,
     "How was this piece of knowledge derived, and by a person or by a machine?",
     "Knowledge a machine extracted needs a different kind of review from knowledge a person paraphrased.",
     F(KT, "DerivationRoute", "DerivedByAgent", "DerivedByAgentKind", "IsMachineDerived") + F(CSM, "ProducedByMethodApplication")),
    ("aq-pkm2-q39", AUTH,
     "Who validated this piece of knowledge, and was it someone other than the person who derived it?",
     "A derivation confirmed only by its own author has not been independently validated.",
     F(KT, "ValidatedByAgent", "ValidatedAt", "IsSelfValidated")),
    # ------------------------------------------------------------------ documents and practice
    ("q13-knowledge-engineer-document-starting-point", KE,
     "For steps we took from existing documents, has capture from the people who do the work since confirmed them, and added what the documents leave out?",
     "Documents describe the prescribed process; only people who perform it can confirm it and supply what is missing.",
     F(CSM, "IsDocumentSource", "IsPeopleCapture")
     + F(KT, "SourceIsDocument", "SourceIsPeopleCapture", "IsDocumentOrigin", "StepElicitedValidationCount",
         "StepElicitedExtensionCount", "IsDocumentStartNeverValidated", "IsDocumentStartNeverExtended")
     + F("Steps", "ElicitedValidationCount", "ElicitedExtensionCount")),
    ("aq-pkm4-q13", OPS,
     "Which of our procedures are documented in a way that trails how they are actually performed?",
     "Operators follow the practice; auditors and new hires follow the document. Where they differ, one of them is wrong.",
     F(KT, "ContradictedDocument", "ContradictedDocumentRevisedAt", "IsDocumentTrailingPractice", "PrescribedVersusEnacted")
     + F("Resources", "TrailingPracticeCount", "IsBehindCurrentPractice")
     + F("ProcedureVersions", "TrailingPracticeTraceCount", "IsDocumentedBehindPractice")),
]

QUESTIONS += [
    # ------------------------------------------------------------------ process mining
    ("q13-knowledge-engineer-mining-complement", KE,
     "Where mining found cases that did not follow the procedure, have we asked experienced workers why?",
     "An event log shows what was done, never the reasoning or tacit judgment behind it.",
     F(CSM, "HoldsReasoningOrTacitKnowledge", "ComplementsMiningRun")
     + F("ProcessMiningRuns", "PeopleCaptureComplementCount", "IsDeviationUnexplainedByPeople")),
    ("aq-pkm1-q13", KE,
     "Which hand-offs in the flow mined from our event logs are not in the process logic we intended?",
     "A path the model does not contain is either an undocumented practice or a control being bypassed.",
     F("MinedFlowEdges", "ProcessMiningRun", "FromStep", "ToStep", "ObservedCaseCount", "DocumentedTransitionCount", "IsUndocumentedPath")),
    ("aq-pkm2-q15", RELEASE,
     "In which of our procedures does the process as enacted deviate from the documented one?",
     "A release process that is enacted differently from its documentation is not governed by it.",
     F("ProcessMiningRuns", "UndocumentedPathCount", "HasUndocumentedEnactedPath")),
    ("aq-pkm2-q16", RELEASE,
     "Where in the mined release flow do cases sit waiting far longer than the next step should take?",
     "Waiting is where release lead time goes, and where people start looking for a way around the gate.",
     F("MinedFlowEdges", "MedianWaitMinutes", "ToStepExpectedMinutes", "IsBottleneck")),
    ("q13-knowledge-authority-mined-versus-intended", AUTH,
     "Has any mined behavior been recorded as intended process logic without anyone deciding that it should be?",
     "A log records what happened, not what should happen; promoting it silently makes a workaround normative.",
     F("MinedFlowEdges", "RecordedStance", "IntentDecisionBy", "IsMinedPathRecordedAsIntentWithoutDecision")),
    # ------------------------------------------------------------------ perspectives
    ("aq-pkm1-q09", STEWARD,
     "Which stakeholder perspectives on this process conflict, and which collected material does each come from?",
     "Legitimate disagreement is information about the process; its source tells us whose experience it reflects.",
     F("StakeholderPerspectives", "ProcedureVersion", "Step", "HolderRole", "Position", "SourceMaterial",
       "ConflictsWithPerspective", "SourceMaterialKind", "ConflictPartnerCount", "IsInConflict")),
    ("q13-process-steward-dissent-kept", STEWARD,
     "Did we collapse a legitimate disagreement into one agreed version, or keep a dissenting view without its source?",
     "Forcing consensus discards the knowledge of whoever lost the argument.",
     F("StakeholderPerspectives", "Disposition", "IsDissentingViewNotKeptWithSource")),
    # ------------------------------------------------------------------ collection built into work
    ("q13-process-steward-capture-in-work", STEWARD,
     "For which procedures do we only collect knowledge in separate exercises and never while the work is being done?",
     "Capture that waits for a workshop loses what people notice in the moment.",
     F(CSM, "CapturedDuringExecution", "IsCapturedInFlowOfWork")
     + F("Procedures", "CollectedMaterialCount", "InWorkCaptureCount", "IsCaptureSeparateFromWork")),
    ("q13-process-steward-collection-occasions", STEWARD,
     "Are our retrospectives, process reviews and improvement initiatives held on cadence, and does what they surface get collected?",
     "A recurring occasion that is skipped, or held and not captured, is a collection channel that has quietly closed.",
     F("CollectionOccasions", "Label", "OccasionKind", "Procedure", "CadenceDays", "LastHeldAt", "EvaluationContext",
       "AsOfInstant", "DaysSinceHeld", "IsLapsed", "CapturedMaterialCount", "IsHeldWithoutCapture")
     + F(CSM, "CollectedAtOccasion")),
    ("q13-knowledge-engineer-feedback-into-collection", KE,
     "When a practitioner's feedback reveals know-how the procedure lacks, did it prompt us to go and collect it?",
     "Feedback is the cheapest discovery channel for tacit knowledge; unfollowed, it is lost with the ticket.",
     F(CSM, "PromptedByFeedback") + F("UserFeedback", "RevealsTacitKnowledge", "CollectionFollowUpCount", "IsTacitSignalNotFedIntoCollection")),
    ("q13-knowledge-engineer-acquisition-cost", KE,
     "How many hours of our domain experts' time has capturing each procedure's knowledge taken?",
     "Acquisition is paid for in expert time, by people whose job is not knowledge modeling.",
     F(CSM, "ContributingExpert", "ExpertEffortHours") + F("Procedures", "ExpertAcquisitionHours")),
    # ------------------------------------------------------------------ failure, redesign, tacit knowledge
    ("aq-pkm3-q11", SAFETY,
     "How has this procedure failed before, how was each failure recorded, and what redesign followed?",
     "A failure that is recorded but never fed into a redesign will happen again at the next run.",
     F("IssueOccurrences", "RedesignChangeRequest", "RedesignImplementedAt", "ImprovementCyclePath", "IsFailureWithoutLandedRedesign")),
    ("q13-knowledge-authority-tacit-behind-compliance", AUTH,
     "Which procedures did we standardize for compliance without capturing the tacit knowledge that makes them work?",
     "A compliant document does not carry the judgment of the people who perform it.",
     F("ProcedureVersions", "StandardizationDriver", "TacitFragmentCount", "IsStandardizedWithoutTacitCapture")),
]


def _rows(cols, *values, iri_=None):
    out = []
    for v in values:
        row = dict(zip(cols, v))
        if iri_ is not None:
            row["SemanticTypeIri"] = iri_
        out.append(row)
    return out


_csm_cols = ["CollectedSourceMaterialId", "Label", "MaterialKind", "Procedure", "CollectedAt", "SourceDocument",
             "HoldsReasoningOrTacitKnowledge", "ComplementsMiningRun", "CapturedDuringExecution", "CollectedAtOccasion",
             "PromptedByFeedback", "ProducedByMethodApplication", "ContributingExpert", "ExpertEffortHours"]

_kt_cols = ["KnowledgeTraceId", "TargetKind", "ProcedureVersion", "Step", "Requirement", "SourceMaterial", "TraceRole",
            "TracedAspect", "SourceStatement", "DerivationRoute", "DerivedByAgent", "ValidatedByAgent", "ValidatedAt",
            "SourceStatedDurationMinutes", "ContradictedDocument"]

RB_DOC = "Paraphrased from the deployment runbook, approval section"
LOTO_SOP = "Paraphrased from section 4 of the 2019 lockout SOP"

ROWS = {
    "MethodApplications": _rows(["MethodApplicationId", "KnowledgeMethod", "AppliedTo", "AppliedAt", "AppliedByAgent"],
        ("ma13-process-mining-deploy", "ProcessMining",
         "CI/CD pipeline event log for deploy-v3.2.0 mined into hand-offs, wait times and undocumented paths.",
         "2026-07-18T09:00:00-05:00", "sam-adeyemi"),
        ("ma13-document-analysis-press7", "DocumentAnalysis",
         "Press-7 retrofit drawing notes and the 2019 lockout SOP analysed for isolation and bleed-down steps.",
         "2026-05-01T09:00:00-05:00", "sam-adeyemi"),
        iri_=f"{EXT}MethodApplication"),
    "CollectionOccasions": _rows(["CollectionOccasionId", "Label", "OccasionKind", "Procedure", "CadenceDays", "LastHeldAt", "EvaluationContext"],
        ("co13-deploy-release-retro", "Monthly release retrospective", "Retrospective", "production-deployment", 30, "2026-07-18T14:00:00-05:00", EV),
        ("co13-loto-quarterly-review", "Quarterly lockout process review", "ProcessReview", "lockout-tagout", 90, "2026-06-30T10:00:00-05:00", EV),
        ("co13-close-improvement", "Close cycle-time improvement initiative", "ImprovementInitiative", "quarter-end-close", 90, "2026-03-15T10:00:00-05:00", EV),
        iri_=f"{EXT}CollectionOccasion"),
    "UserFeedback": _rows(["UserFeedbackId", "ProcedureExecution", "ProvidedByAgent", "ProvidedAt", "FeedbackText", "Disposition",
                           "ChangeRequestKey", "FeedbackOnProcedure", "FeedbackOnExecution", "RevealsTacitKnowledge"],
        ("uf13-loto-hiss-cue", "exec-loto-0718-north-day", "tomas-reyes", "2026-07-18T07:30:00-05:00",
         "On press-7 we wait for the accumulator hiss to stop before trusting the gauge; the bleed step does not say so.",
         "Accepted", "", "", "", True),
        ("uf13-deploy-cache-warm", "exec-deploy-2026-03-01", "grace-holloway", "2026-03-01T15:00:00-06:00",
         "Before switching traffic we warm the caches by hand; nothing written says when that is safe.",
         "Acknowledged", "", "", "", True),
        iri_="https://w3id.org/pko#UserFeedbackOccurrence"),
    CSM: _rows(_csm_cols,
        ("csm13-press7-drawing-excerpt", "Excerpt: press-7 retrofit drawing notes, pneumatic isolation", "DocumentExcerpt",
         "lockout-tagout", "2026-05-01T09:00:00-05:00", "res-press7-retrofit-drawing", False, None, None, None, None,
         "ma13-document-analysis-press7", "tomas-reyes", 1.5),
        ("csm13-loto-night-fieldnotes-bleed", "Field notes: hiss after the press-7 valve closed, 07-09 night lockout", "FieldNotes",
         "lockout-tagout", "2026-07-10T03:30:00-05:00", None, True, None, "exec-loto-0709-south-night", None,
         None, None, "ken-watanabe", 1),
        ("csm13-loto-tomas-hiss-followup", "Transcript: follow-up with Tomas Reyes on the accumulator hiss cue", "Transcript",
         "lockout-tagout", "2026-07-18T13:00:00-05:00", None, True, None, None, None,
         "uf13-loto-hiss-cue", None, "tomas-reyes", 1.5),
        ("csm13-loto-review-notes", "Transcript: quarterly lockout process review, June 2026", "Transcript",
         "lockout-tagout", "2026-06-30T12:00:00-05:00", None, True, None, None, "co13-loto-quarterly-review", None, None,
         "tomas-reyes", 3),
        ("csm13-deploy-hotfix-interview", "Interview transcript: Grace Holloway on hotfixes that skip the gate", "Transcript",
         "production-deployment", "2026-07-18T15:00:00-05:00", None, True, "pmr-deploy-v320-ci", None,
         "co13-deploy-release-retro", None, None, "grace-holloway", 1.5),
        ("csm13-deploy-mining-trace-0718", "Mined event trace: deploy-v3.2.0 hand-offs, 2026-07-18", "MinedEventTrace",
         "production-deployment", "2026-07-18T09:00:00-05:00", None, False, None, None, None, None,
         "ma13-process-mining-deploy", None, 0.5),
        iri_=f"{EXT}CollectedSourceMaterial")
    # the sibling theme's excerpts gain only the document they were excerpted from (a column this loop adds)
    + [{"CollectedSourceMaterialId": "csm-deploy-runbook-excerpt", "SourceDocument": "res-deploy-runbook"},
       {"CollectedSourceMaterialId": "csm-loto-sop-excerpt", "SourceDocument": "res-loto-sop-2019"}],
}

ROWS[KT] = _rows(_kt_cols,
    # ---- production deployment: the runbook was revised on 2026-01-02, after it was excerpted on 2025-11-01
    ("kt13-deploy-model", "ProcessModel", "deploy-v3.2.0", None, None, "csm-deploy-runbook-excerpt", "Origin", "Sequence",
     "Build, classify risk, approve, roll out, verify.", "Step sequence transcribed from the runbook", "sam-adeyemi",
     "grace-holloway", "2025-11-10T10:00:00-06:00", None, None),
    ("kt13-deploy-01", "ActivityDefinition", "deploy-v3.2.0", "deploy-01", None, "csm-deploy-runbook-excerpt", "Origin",
     "StepDescription", "Build the release candidate from the tagged commit; about twenty minutes.", RB_DOC, "sam-adeyemi",
     "grace-holloway", "2025-11-10T10:00:00-06:00", 20, None),
    ("kt13-deploy-02", "ActivityDefinition", "deploy-v3.2.0", "deploy-02", None, "csm-deploy-transcript-grace", "Origin",
     "StepDescription", "Every change is classified for risk before it reaches the gate.",
     "Extracted from the interview transcript by the wiki workflow extractor", "wiki-extractor", None, None, None, None),
    ("kt13-deploy-03", "ActivityDefinition", "deploy-v3.2.0", "deploy-03", None, "csm-deploy-runbook-excerpt", "Origin",
     "StepDescription", "The release manager records approval before rollout; fifteen minutes.", RB_DOC, "sam-adeyemi",
     "sam-adeyemi", "2025-11-12T10:00:00-06:00", 15, None),
    ("kt13-deploy-03-validation", "ActivityDefinition", "deploy-v3.2.0", "deploy-03", None, "csm13-deploy-hotfix-interview",
     "Validates", "StepDescription", "Grace confirms approval is recorded for every scheduled release.",
     "Read back to the release manager at the release retrospective", "sam-adeyemi", "grace-holloway",
     "2026-07-18T16:00:00-05:00", None, None),
    ("kt13-deploy-03-hotfix-log", "ActivityDefinition", "deploy-v3.2.0", "deploy-03", None, "csm13-deploy-mining-trace-0718",
     "Contradicts", "ActualExecution", "Three hotfix cases went from risk classification straight to rollout with no approval event.",
     "Mined from the CI/CD event log", "sam-adeyemi", "grace-holloway", "2026-07-18T17:00:00-05:00", None, "res-deploy-runbook"),
    ("kt13-deploy-04", "ActivityDefinition", "deploy-v3.2.0", "deploy-04", None, "csm-deploy-transcript-grace", "Origin",
     "ActualExecution", "Rollout always starts within minutes of approval; thirty minutes to complete.",
     "Paraphrased from the interview transcript", "sam-adeyemi", "grace-holloway", "2025-11-25T10:00:00-06:00", 30, None),
    ("kt13-deploy-approval-constraint", "Constraint", "deploy-v3.2.0", None, "req-deploy-approval", "csm-deploy-runbook-excerpt",
     "Origin", "Constraint", "No rollout starts without a recorded release approval.", RB_DOC, "sam-adeyemi", "marcus-webb",
     "2025-11-12T10:00:00-06:00", None, None),
    # ---- lockout/tagout: the press-7 retrofit drawing changed on 2026-07-15, after the bleed step was taken from it
    ("kt13-loto-model", "ProcessModel", "loto-v2.0.0", None, None, "csm-loto-sop-excerpt", "Origin", "Sequence",
     "Prepare, notify, shut down, isolate, lock, verify, work, restore.",
     "Sequence taken from the 2019 SOP and restructured into isolation sub-steps", "sam-adeyemi", "lin-zhao",
     "2026-05-20T09:00:00-05:00", None, None),
    ("kt13-loto-04", "ActivityDefinition", "loto-v2.0.0", "loto-04", None, "csm-loto-sop-excerpt", "Origin", "StepDescription",
     "Isolate every energy source listed on the work order; twenty minutes.", LOTO_SOP, "sam-adeyemi", "lin-zhao",
     "2026-05-20T09:00:00-05:00", 20, None),
    ("kt13-loto-04b", "ActivityDefinition", "loto-v2.0.0", "loto-04b", None, "csm13-press7-drawing-excerpt", "Origin",
     "StepDescription", "Close the pneumatic supply and bleed the line at the valve on the drawing; five minutes.",
     "Paraphrased from the press-7 retrofit drawing notes", "sam-adeyemi", "lin-zhao", "2026-05-02T09:00:00-05:00", 5, None),
    ("kt13-loto-04b-validation", "ActivityDefinition", "loto-v2.0.0", "loto-04b", None, "csm13-loto-review-notes", "Validates",
     "StepDescription", "Technicians at the quarterly review confirm the bleed valve location.",
     "Read back at the quarterly lockout review", "sam-adeyemi", "lin-zhao", "2026-06-30T13:00:00-05:00", None, None),
    ("kt13-loto-04b-extension", "ActivityDefinition", "loto-v2.0.0", "loto-04b", None, "csm13-loto-night-fieldnotes-bleed",
     "Extends", "Workaround", "The technician waits for the accumulator hiss to stop, then cracks the accumulator bleed valve the drawing omits.",
     "Observed on the 07-09 night lockout and written up during the run", "sam-adeyemi", "tomas-reyes", "2026-07-18T14:00:00-05:00", None, None),
    ("kt13-loto-06", "ActivityDefinition", "loto-v2.0.0", "loto-06", None, "csm-loto-transcript-tomas", "Origin",
     "StepDescription", "A proper zero-energy check on press-7 takes twenty minutes: try the controls, read the gauge, tap the glass.",
     "Paraphrased from the workshop transcript", "sam-adeyemi", "lin-zhao", "2026-04-25T09:00:00-05:00", 20, None),
    ("kt13-loto-zero-energy-constraint", "Constraint", "loto-v2.0.0", None, "req-loto-zero-energy", "csm-loto-sop-excerpt",
     "Origin", "Constraint", "No work begins until zero energy is verified.", LOTO_SOP, "sam-adeyemi", "lin-zhao",
     "2026-05-20T09:00:00-05:00", None, None),
    iri_=f"{PROV}Derivation")

PMR = "pmr-deploy-v320-ci"
ROWS["MinedFlowEdges"] = _rows(["MinedFlowEdgeId", "ProcessMiningRun", "FromStep", "ToStep", "ObservedCaseCount", "MedianWaitMinutes",
                                "RecordedStance", "IntentDecisionBy"],
    ("mfe13-deploy-01-02", PMR, "deploy-01", "deploy-02", 41, 5, "Observed", None),
    ("mfe13-deploy-02-03", PMR, "deploy-02", "deploy-03", 38, 1440, "Observed", None),   # a day waiting for approvers
    ("mfe13-deploy-02-04-hotfix", PMR, "deploy-02", "deploy-04", 3, 4, "Observed", None),  # no approval event
    ("mfe13-deploy-03-04", PMR, "deploy-03", "deploy-04", 37, 20, "Observed", None),
    ("mfe13-deploy-04-05", PMR, "deploy-04", "deploy-05", 40, 3, "Intended", "omar-haddad"),
    ("mfe13-deploy-05-04-retry", PMR, "deploy-05", "deploy-04", 6, 12, "Intended", None),  # retry loop promoted silently
    iri_=f"{EXT}MinedFlowEdge")

ROWS["StakeholderPerspectives"] = _rows(["StakeholderPerspectiveId", "ProcedureVersion", "Step", "HolderRole", "Position",
                                         "SourceMaterial", "ConflictsWithPerspective", "Disposition"],
    ("sp13-loto06-technician", "loto-v2.0.0", "loto-06", "senior-maintenance-technician",
     "Zero energy on press-7 is confirmed by ear and by tapping the gauge glass, not by the gauge reading alone.",
     "csm-loto-transcript-tomas", "sp13-loto06-safety", "Retained"),
    ("sp13-loto06-safety", "loto-v2.0.0", "loto-06", "plant-safety-officer",
     "Zero energy is confirmed by the calibrated gauge reading the SOP names; sensory cues are not a control.",
     "csm-loto-sop-excerpt", None, "Retained"),
    ("sp13-deploy03-release", "deploy-v3.2.0", "deploy-03", "release-manager",
     "During an incident a hotfix may roll out on verbal approval that is recorded afterwards.",
     "csm13-deploy-hotfix-interview", "sp13-deploy03-audit", "OverriddenByConsensus"),
    ("sp13-deploy03-audit", "deploy-v3.2.0", "deploy-03", "internal-compliance-auditor",
     "Every rollout, hotfix or not, needs an approval recorded before it starts.",
     "csm-deploy-runbook-excerpt", None, "Retained"),
    ("sp13-deploy04-sre", "deploy-v3.2.0", "deploy-04", "site-reliability-engineer",
     "Caches are warmed before traffic is switched.", None, None, "Retained"),
    iri_=f"{EXT}StakeholderPerspective")

ROWS["ChangeRequests"] = _rows(["ChangeRequestId", "ProcedureVersion", "Title", "ChangeKind", "Status", "RequestedByAgent",
                                "AuthorityRole", "RequestedAt", "DecidedAt", "ImplementedAt", "ImpactAssessment", "EvaluationContext"],
    ("cr13-loto2-accumulator-bleed", "loto-v2.0.0", "Add the press-7 accumulator bleed-down to pneumatic isolation", "Defect",
     "Approved", "ken-watanabe", "plant-safety-officer", "2026-07-10T08:00:00-05:00", "2026-07-11T10:00:00-05:00",
     "2026-07-13T09:00:00-05:00", "Adds the accumulator to the press-7 isolation map and a bleed-down cue to step 04b before the next run.", EV),
    iri_=f"{EXT}ChangeRequest")

# Columns this loop adds, set on rows other loops own. No other column of those rows is touched.
ROWS["IssueOccurrences"] = [{"IssueOccurrenceId": "issue-0709-residual", "RedesignChangeRequest": "cr13-loto2-accumulator-bleed"}]
ROWS["ProcedureVersions"] = [{"ProcedureVersionId": "forklift-v1.0.0", "StandardizationDriver": "Compliance"},
                             {"ProcedureVersionId": "close-v1.1.0", "StandardizationDriver": "Compliance"},
                             {"ProcedureVersionId": "deploy-v3.2.0", "StandardizationDriver": "Practice"}]

_M = [  # (source path, kind, iri, relation, profile)
    ("KnowledgeTraces", "class", f"{PROV}Derivation", "aligned", "prov-o"),
    ("KnowledgeTraces.SourceMaterial", "property", f"{PROV}entity", "aligned", "prov-o"),
    ("KnowledgeTraces.DerivedByAgent", "property", f"{PROV}agent", "aligned", "prov-o"),
    ("KnowledgeTraces.ValidatedByAgent", "property", f"{EXT}validatedBy", "extension", "erb-pko-extension-1.0.0"),
    ("MinedFlowEdges", "class", f"{EXT}MinedFlowEdge", "extension", "erb-pko-extension-1.0.0"),
    ("CollectionOccasions", "class", f"{EXT}CollectionOccasion", "extension", "erb-pko-extension-1.0.0"),
    ("StakeholderPerspectives", "class", f"{EXT}StakeholderPerspective", "extension", "erb-pko-extension-1.0.0"),
]
MAPPINGS = [(f"map13-{p.lower().replace('.', '-')}", p, k, i, r, prof, "Added by loop-13.") for p, k, i, r, prof in _M]
