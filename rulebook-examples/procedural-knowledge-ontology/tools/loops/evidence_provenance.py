"""Evidence for theme H (loop-13): provenance, traceability, process mining and the document-practice gap.

(kind, target, justification). Validity is computed by ClaimEvidence.IsValid from Postgres
measurements; nothing here asserts coverage by itself.
"""


def fld(target, why):
    return ("Field", target, why)


def tbl(target, why):
    return ("Table", target, why)


def q(target, why):
    return ("RoleQuestion", target, why)


def method(target, why):
    return ("KnowledgeMethod", target, why)


EVIDENCE = {
    # ---------------------------------------------------------------- pkm-1
    "pkm1-c30": [fld("KnowledgeTraces.ProvenanceStatement",
                     "Every trace states in one line the material the item came from (origin), the derivation route and "
                     "deriving agent (route), and the agent who confirmed it (confirmation), e.g. loto-04b from the press-7 "
                     "drawing excerpt, paraphrased by the knowledge engineer, confirmed by the plant safety officer.")],
    "pkm1-c32": [fld("CollectedSourceMaterials.ExpertEffortHours",
                     "Each capture records the hours of domain-expert time it consumed and whose (ContributingExpert): "
                     "technicians and the release manager, not knowledge modelers, spent 0.5 to 3 hours per capture; "
                     "Procedures.ExpertAcquisitionHours totals the cost per procedure.")],
    "pkm1-p25": [fld("ProcessMiningRuns.IsDeviationUnexplainedByPeople",
                     "TRUE when a mining run found non-conforming variants and no capture from people that holds the "
                     "reasoning behind them complements the run. It fires on the close cutoff-bypass run (3 of 8 variants "
                     "conform, nobody asked why) and not on the deployment run, whose hotfix bypass Grace Holloway explained "
                     "in an interview; a mining finding left uncomplemented is exactly the breach.")],
    "pkm1-p26": [fld("MinedFlowEdges.IsMinedPathRecordedAsIntentWithoutDecision",
                     "TRUE when a hand-off reconstructed from the event log is recorded as intended process logic with nobody "
                     "having decided it should be. It fires on the deploy-05 -> deploy-04 retry loop, silently promoted from "
                     "the log, and not on deploy-04 -> deploy-05 (decided by an engineer) or edges kept as Observed; the "
                     "breach is mined behavior becoming indistinguishable from intent.")],
    "pkm1-p29": [fld("StakeholderPerspectives.IsDissentingViewNotKeptWithSource",
                     "TRUE when a perspective that conflicts with another was overridden into one agreed version or is kept "
                     "without the material it comes from. It fires on the release manager's hotfix view, overridden by the "
                     "auditor's, and not on the technician and safety officer views of zero-energy verification, both "
                     "retained with their sources.")],
    "pkm1-q08": [q("aq-pkm1-q08",
                   "The auditor's question is answered per knowledge item by ProvenanceStatement (origin, route, confirmation), "
                   "and HasIncompleteProvenance flags the item missing one of the three: the deploy-02 definition the wiki "
                   "extractor produced, which nobody confirmed.")],
    "pkm1-q09": [q("aq-pkm1-q09",
                   "The steward's question is answered by StakeholderPerspectives: IsInConflict marks the four views that "
                   "conflict (zero-energy verification, hotfix approval), and SourceMaterial / SourceMaterialKind say which "
                   "transcript, excerpt or interview each comes from.")],
    "pkm1-q13": [q("aq-pkm1-q13",
                   "IsUndocumentedPath on each mined hand-off answers where log behavior departs from intended logic: the "
                   "hotfix path deploy-02 -> deploy-04 (no approval) and the deploy-05 -> deploy-04 retry, neither in "
                   "StepTransitions.")],
    "pkm1-s05": [method("ProcessMining",
                        "Applied by MethodApplications ma13-process-mining-deploy to the CI/CD event log; its output is the "
                        "mined event trace material and the MinedFlowEdges, and IsDeviationUnexplainedByPeople records that it "
                        "is a partial aid that needs capture from people.")],
}

EVIDENCE.update({
    # ---------------------------------------------------------------- pkm-2: concepts
    "pkm2-c19": [tbl("MinedFlowEdges",
                     "One row per step-to-step hand-off rebuilt from the CI/CD event log by a mining run, with the cases that "
                     "took it and their wait times: the actual flow of the deployment process.")],
    "pkm2-c20": [fld("MinedFlowEdges.IsUndocumentedPath",
                     "TRUE for a reconstructed hand-off that no documented transition matches: the deviation between the "
                     "documented deployment procedure and the flow rebuilt from its event log.")],
    "pkm2-c21": [fld("MinedFlowEdges.IsBottleneck",
                     "TRUE where mined cases wait more than four times the entered step's documented duration: cases sit a "
                     "day (1440 minutes) before the fifteen-minute release approval gate.")],
    "pkm2-c22": [fld("CollectedSourceMaterials.HoldsReasoningOrTacitKnowledge",
                     "Each collected material records whether it holds the reasoning or tacit knowledge behind the actions. "
                     "The mined deployment event trace reads FALSE (it holds only system events) while the interview and "
                     "field-note captures read TRUE; the mining-complement witness counts only the latter.")],
    "pkm2-c24": [fld("CollectedSourceMaterials.SourceDocument",
                     "Collected excerpts name the existing document they were taken from: the deployment runbook, the 2019 "
                     "lockout SOP and the press-7 retrofit drawing notes.")],
    "pkm2-c25": [fld("KnowledgeTraces.PrescribedVersusEnacted",
                     "For a trace where practice evidence contradicts a document, states what the document prescribes against "
                     "what was enacted: the runbook requires recorded approval, the event log shows three hotfixes rolled out "
                     "with no approval event.")],
    "pkm2-c26": [fld("Resources.IsBehindCurrentPractice",
                     "TRUE for a document contradicted by practice evidence collected after its last revision: the deployment "
                     "runbook, last revised 2026-01-02, contradicted by the event trace of 2026-07-18.")],
    "pkm2-c102": [tbl("CollectionOccasions",
                      "Recurring collection occasions with a cadence: the monthly release retrospective, the quarterly lockout "
                      "process review and the close improvement initiative, with the materials each produced.")],
    "pkm2-c103": [fld("KnowledgeTraces.DerivationRoute",
                      "Every trace records how the knowledge was derived from its source: paraphrased from a runbook section, "
                      "extracted by the wiki extractor from a transcript, mined from the event log, observed and written up.")],
    "pkm2-c104": [fld("KnowledgeTraces.ValidatedByAgent",
                      "Every trace records the agent who validated the derived knowledge (plant safety officer, release "
                      "manager, auditor, senior technician), or blank when nobody has.")],
    # ---------------------------------------------------------------- pkm-2: prescriptions
    "pkm2-p13": [fld("KnowledgeTraces.IsDocumentStartNeverValidated",
                     "TRUE when a step first taken from a document has no validation captured from people. It fires on deploy-01 "
                     "and loto-04, taken from the runbook and the SOP and never confirmed, and not on deploy-03 or loto-04b, "
                     "confirmed at the release retrospective and the quarterly review; document knowledge treated as final is "
                     "the breach.")],
    "pkm2-p14": [fld("KnowledgeTraces.IsDocumentStartNeverExtended",
                     "TRUE when a step first taken from a document has never been extended by capture from people. It fires on "
                     "deploy-01, deploy-03 and loto-04, and not on loto-04b, extended by night-shift field notes with the "
                     "accumulator bleed the drawing omits.")],
    "pkm2-p29": [fld("ProcedureVersions.IsLiveModelUntraced",
                     "TRUE when an executable process model has no origin trace to a collection activity. It fires on the live "
                     "close, policy, conveyor, press-changeover and forklift models and not on the lockout and deployment "
                     "models, which trace to the SOP and runbook excerpts.")],
    "pkm2-p30": [fld("Steps.IsUntracedActivityInTracedModel",
                     "TRUE when a step of a traced process model has no origin trace of its own definition. It fires on "
                     "deploy-05 and nine lockout steps and not on deploy-01..04, loto-04, loto-04b or loto-06.")],
    "pkm2-p31": [fld("Requirements.IsUntracedBoundConstraint",
                     "TRUE when a constraint bound to a step has no trace to a collection activity. It fires on the one-person-"
                     "one-lock requirement and the close and policy constraints, and not on zero-energy verification or recorded "
                     "release approval, traced to the SOP and runbook.")],
    "pkm2-p32": [fld("CollectedSourceMaterials.IsDependencyInvisibleToChange",
                     "TRUE when a material was encoded into a model but no trace records which knowledge rests on it, so a "
                     "change to it reveals nothing. It fires on the conveyor manual roller excerpt encoded into convmaint-v1.0.0 "
                     "and not on the lockout and deployment materials, whose dependents HasKnowledgeAffectedBySourceChange "
                     "reveals when their document is revised.")],
    "pkm2-p33": [fld("KnowledgeTraces.IsUnfaithfulToSource",
                     "Uses the trace to check the model against its source: TRUE when the modeled step's duration differs from "
                     "the duration the source states. It fires on loto-06 (Tomas: twenty minutes; model: ten) and not on the "
                     "five traces whose stated durations match.")],
    "pkm2-p56": [fld("Procedures.IsCaptureSeparateFromWork",
                     "TRUE when knowledge about a procedure is collected but never in the course of executing it. It fires on "
                     "production deployment and conveyor maintenance and not on lockout/tagout, whose field notes were written "
                     "during the 07-09 night lockout.")],
    "pkm2-p76": [fld("UserFeedback.IsTacitSignalNotFedIntoCollection",
                     "TRUE when practitioner feedback that reveals tacit know-how prompted no collection. It fires on Grace "
                     "Holloway's cache-warming remark and not on Tomas Reyes's hiss cue, which prompted a follow-up transcript.")],
    "pkm2-i11": [fld("KnowledgeTraces.IsAspectUnsupportedBySourceKind",
                     "Enforces the illustrated trace targets: a workaround must trace to observation notes and actual execution "
                     "to an event log. It fires on deploy-04's claim that rollout always follows approval, cited only to an "
                     "interview, and not on the workaround traced to field notes or the hotfix execution traced to the log.")],
})

EVIDENCE.update({
    # ---------------------------------------------------------------- pkm-2: competency questions and standards
    "pkm2-q12": [q("aq-pkm2-q12",
                   "KnowledgeTraces lists, for each step, the collected material it traces to and in what role; "
                   "Steps.HasCollectionEvidence marks the seven steps that trace to any evidence and the 34 that trace to none.")],
    "pkm2-q13": [q("aq-pkm2-q13",
                   "IsSourceChangedSinceTaken compares the source document's last revision with when the knowledge was taken: "
                   "TRUE for the four traces from the runbook (revised after excerpting) and loto-04b (drawing revised "
                   "2026-07-15), FALSE for the 2019 SOP and the transcripts.")],
    "pkm2-q14": [q("aq-pkm2-q14",
                   "HasKnowledgeAffectedBySourceChange marks the runbook excerpt and the press-7 drawing excerpt, and the traces "
                   "with IsSourceChangedSinceTaken name the process model, steps and constraint each revision affects.")],
    "pkm2-q15": [q("aq-pkm2-q15",
                   "HasUndocumentedEnactedPath marks the deployment mining run, whose reconstructed flow contains two hand-offs "
                   "the documented model lacks, and no other run.")],
    "pkm2-q16": [q("aq-pkm2-q16",
                   "IsBottleneck on MinedFlowEdges marks the hand-off into the release approval gate, where cases wait 1440 "
                   "minutes against a fifteen-minute step.")],
    "pkm2-q38": [q("aq-pkm2-q38",
                   "DerivationRoute and DerivedByAgent answer how each item was derived; IsMachineDerived marks the deploy-02 "
                   "definition extracted by the wiki extractor AI rather than paraphrased by a person.")],
    "pkm2-q39": [q("aq-pkm2-q39",
                   "ValidatedByAgent answers who validated each item; IsSelfValidated marks deploy-03, confirmed only by the "
                   "knowledge engineer who derived it.")],
    "pkm2-s14": [method("DocumentAnalysis",
                        "Applied by MethodApplications ma13-document-analysis-press7 to the press-7 retrofit drawing notes and "
                        "the 2019 SOP; it produced the drawing excerpt that loto-04b traces to.")],
    "pkm2-s16": [method("ProcessMining",
                        "Applied by MethodApplications ma13-process-mining-deploy to the enterprise CI/CD event log, producing "
                        "the mined event trace and the reconstructed hand-offs in MinedFlowEdges.")],
    # ---------------------------------------------------------------- pkm-3, pkm-4
    "pkm3-c15": [fld("IssueOccurrences.ImprovementCyclePath",
                     "For a failure fed into a redesign, states where it was observed, how it was recorded and the redesign that "
                     "followed: the residual-energy failure on the 07-09 night lockout, recorded as err-residual-energy, "
                     "redesigned by cr13-loto2-accumulator-bleed, implemented 2026-07-13 before the next run on 07-14.")],
    "pkm3-p04": [fld("ProcedureVersions.IsStandardizedWithoutTacitCapture",
                     "TRUE when a version standardized for compliance has no tacit knowledge fragment captured behind it. It "
                     "fires on the forklift daily inspection and not on the compliance-driven close, which carries a tacit "
                     "fragment, nor on versions standardized from practice.")],
    "pkm3-q11": [q("aq-pkm3-q11",
                   "IssueOccurrences answer how the procedure failed and how it was recorded (Error, IssueCause, encountering "
                   "agent); RedesignChangeRequest and ImprovementCyclePath give the redesign; IsFailureWithoutLandedRedesign "
                   "marks the three failures with no landed redesign.")],
    "pkm4-q13": [q("aq-pkm4-q13",
                   "IsDocumentedBehindPractice marks deploy-v3.2.0, whose runbook is contradicted by practice evidence collected "
                   "after its last revision; PrescribedVersusEnacted says how.")],
})
