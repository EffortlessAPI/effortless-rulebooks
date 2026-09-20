"""Loop 17 — what the series reads aloud, the app must show.

Premise: the video series filmed in the role experiences reads numbers and links aloud
("three explicit, four tacit", "all thirteen answers", "the register holds that as an open
gap"). Rehearsing the series against the built app found seven places where the sentence
was true of the rows and no page showed it, so the shot plan had to DRAW it beside the
app. A viewer who signs in cannot find a drawn number. The rule of the app is that a value
on screen is a column of a view, never something counted in the client, so each of those
sentences becomes a field here and the page reads it.

Register-wide totals live on EvaluationContexts: that table has exactly one current row,
and "the register, as judged at this instant" is what each total is. They use the
literal-criterion COUNTIFS shape already proven on RulebookReleases.StatedCriterionCount
and ProcedureVersions.CountOfOverdueGaps. A total over every row is the sum of the TRUE
count and the FALSE count of a boolean that is never blank on that table.

The run that stopped is linked to the gap it stopped at, and the gap to the model change
that answers it, as relationships; the run then reads both through one-hop lookups, so the
technician sees them on their own run without being granted two more tables.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import agg, calc, idx, lookup, rel  # noqa: E402

LOOP = {
    "WitnessLoopId": "loop-17",
    "LoopNumber": 17,
    "Title": "What is read aloud must be on a page",
    "Premise": (
        "Loop 15 gave each role an app to work in, and rehearsing the video series in that app "
        "showed the difference between a fact the rows support and a fact a person can find. "
        "Seven sentences of narration were true and visible nowhere: totals by kind of knowledge, "
        "by who did an assistant's reasoning, by execution order and by mapping relation; the "
        "community a mentorship belongs to; and the path from a cancelled run to the gap it "
        "stopped at and the change that answers it. Those questions only became askable once "
        "people could sign in and look."
    ),
}


def _count(table, field, literal):
    return f"=COUNTIFS({table}!{{{{{field}}}}}, {literal})"


FIELDS = {
    "AssistantAnswers": [
        calc("ModelReasonedAndTaskFailed", "boolean",
             "The language model did the reasoning for this answer itself, and the task it was asked "
             "about ended in failure.",
             '=AND({{ModelDidTheReasoning}}, {{TaskOutcome}} = "Failed")'),
    ],
    "EvaluationContexts": [
        agg("ExplicitFragmentCount", "integer",
            "Pieces of knowledge on record, judged at this instant, that are written down.",
            _count("KnowledgeFragments", "KnowledgeForm", '"Explicit"')),
        agg("TacitFragmentCount", "integer",
            "Pieces of knowledge on record that live in practiced hands and were drawn out of a person.",
            _count("KnowledgeFragments", "KnowledgeForm", '"Tacit"')),
        agg("ImplicitFragmentCount", "integer",
            "Pieces of knowledge on record that everybody acts on and nobody had ever stated.",
            _count("KnowledgeFragments", "KnowledgeForm", '"Implicit"')),
        agg("SituatedJudgmentFragmentCount", "integer",
            "Pieces of knowledge on record that are a judgment call for one situation.",
            _count("KnowledgeFragments", "KnowledgeForm", '"SituatedJudgment"')),
        agg("ModelReasonedAnswerCount", "integer",
            "Assistant answers on record where the language model did the reasoning itself.",
            _count("AssistantAnswers", "ModelDidTheReasoning", "TRUE")),
        agg("OtherwiseReasonedAnswerCount", "integer",
            "Assistant answers on record where the reasoning was done by a query or a reasoner, or "
            "none was needed.",
            _count("AssistantAnswers", "ModelDidTheReasoning", "FALSE")),
        calc("AssistantAnswerCount", "integer",
             "Every assistant answer on record.",
             "={{ModelReasonedAnswerCount}} + {{OtherwiseReasonedAnswerCount}}"),
        agg("ModelReasonedFailedAnswerCount", "integer",
            "Of the answers the language model reasoned out itself, how many of the tasks failed.",
            _count("AssistantAnswers", "ModelReasonedAndTaskFailed", "TRUE")),
        agg("OutOfOrderStepExecutionCount", "integer",
            "Step executions on record that were carried out of the order the procedure specifies.",
            _count("StepExecutions", "IsOutOfSpecifiedOrder", "TRUE")),
        agg("InOrderStepExecutionCount", "integer",
            "Step executions on record that followed the order the procedure specifies.",
            _count("StepExecutions", "IsOutOfSpecifiedOrder", "FALSE")),
        calc("StepExecutionCount", "integer",
             "Every step execution on record.",
             "={{OutOfOrderStepExecutionCount}} + {{InOrderStepExecutionCount}}"),
        agg("EarlyStartStepExecutionCount", "integer",
            "Step executions on record that began before the step they depend on had finished.",
            _count("StepExecutions", "RanBeforePrerequisiteCompleted", "TRUE")),
        agg("ExactMappingCount", "integer",
            "Concepts of this book that map exactly onto a term PKO itself defines.",
            _count("SemanticMappings", "MappingRelation", '"exact"')),
        agg("AlignedMappingCount", "integer",
            "Concepts of this book that map onto one of the older standards PKO reuses.",
            _count("SemanticMappings", "MappingRelation", '"aligned"')),
        agg("ExtensionMappingCount", "integer",
            "Concepts of this book that PKO does not define: the book's own extensions, labelled as such.",
            _count("SemanticMappings", "MappingRelation", '"extension"')),
    ],
    "Mentorships": [
        lookup("CommunityLabel", "string",
               "The name of the community of practice this mentorship belongs to.",
               idx("CommunitiesOfPractice", "Label", "CommunityOfPractice", "CommunityOfPracticeId")),
    ],
    "KnowledgeTransfers": [
        lookup("KnowHowTopic", "string", "What was handed over, in words: the topic of the know-how.",
               idx("KnowHowCarriers", "Topic", "KnowHow", "KnowHowCarrierId")),
    ],
    "ProcessMiningRuns": [
        calc("ConformancePercent", "integer",
             "The share of the paths people really took that match the documented procedure, as a whole percent.",
             "=ROUND({{ConformanceRate}} * 100, 0)"),
    ],
    "KnowledgeGaps": [
        rel("AnsweredByModelChangeRequest", "ModelChangeRequests",
            "The change to the book that was raised to close this gap, when there is one."),
        lookup("AnsweringChangeTitle", "string", "What the change that answers this gap asks for.",
               idx("ModelChangeRequests", "Title", "AnsweredByModelChangeRequest", "ModelChangeRequestId")),
        lookup("AnsweringChangeStatus", "string", "Where the change that answers this gap has got to.",
               idx("ModelChangeRequests", "Status", "AnsweredByModelChangeRequest", "ModelChangeRequestId")),
    ],
    "ProcedureExecutions": [
        rel("StoppedAtKnowledgeGap", "KnowledgeGaps",
            "The knowledge gap this run stopped at: the procedure had no answer, so the run ended "
            "instead of someone guessing."),
        lookup("StoppedAtGapStatement", "string", "What nobody knows, in the gap this run stopped at.",
               idx("KnowledgeGaps", "Statement", "StoppedAtKnowledgeGap", "KnowledgeGapId")),
        lookup("StoppedAtGapStatus", "string", "Whether the gap this run stopped at is still open.",
               idx("KnowledgeGaps", "Status", "StoppedAtKnowledgeGap", "KnowledgeGapId")),
        lookup("StoppedAtGapChangeTitle", "string",
               "The change to the book that answers the gap this run stopped at.",
               idx("KnowledgeGaps", "AnsweringChangeTitle", "StoppedAtKnowledgeGap", "KnowledgeGapId")),
        lookup("StoppedAtGapChangeStatus", "string",
               "Where the change that answers this run's gap has got to.",
               idx("KnowledgeGaps", "AnsweringChangeStatus", "StoppedAtKnowledgeGap", "KnowledgeGapId")),
    ],
}

ROWS = {
    "KnowledgeGaps": [{"KnowledgeGapId": "gap-mixer2-twin-drive-isolation",
                       "AnsweredByModelChangeRequest": "mcr-16"}],
    "ProcedureExecutions": [{"ProcedureExecutionId": "exec-loto-0718-north-day",
                             "StoppedAtKnowledgeGap": "gap-mixer2-twin-drive-isolation"}],
}

_EC = "EvaluationContexts."
QUESTIONS = [
    ("q17-maintenance-technician-kinds-of-knowing", "maintenance-technician",
     "Of everything this register knows, how much is written down, and how much is the kind that "
     "only lives in somebody's hands?",
     "I am told the book holds what the veterans know and not only what the manual says. I want to "
     "see that as four numbers on the page where I read the procedure, not take it on trust.",
     [_EC + "ExplicitFragmentCount", _EC + "TacitFragmentCount", _EC + "ImplicitFragmentCount",
      _EC + "SituatedJudgmentFragmentCount"]),
    ("q17-maintenance-technician-who-did-the-reasoning", "maintenance-technician",
     "When the assistant answers one of us, how often did the language model work the answer out by "
     "itself, and how did those jobs end?",
     "One answer told me to proceed while the locks were still going on. I need to know whether that "
     "was one bad day or the pattern whenever the model reasons alone, across every answer and not "
     "only the two I can see.",
     ["AssistantAnswers.ModelReasonedAndTaskFailed", _EC + "ModelReasonedAnswerCount",
      _EC + "OtherwiseReasonedAnswerCount", _EC + "AssistantAnswerCount",
      _EC + "ModelReasonedFailedAnswerCount"]),
    ("q17-maintenance-technician-order-across-every-run", "maintenance-technician",
     "Across every run on record, how many steps were carried out of the specified order, and how "
     "many began before the step they depend on had finished?",
     "The ledger compares my one run with the procedure. Whether my run is unusual depends on "
     "every other run, and nobody fills in a report for that.",
     [_EC + "OutOfOrderStepExecutionCount", _EC + "InOrderStepExecutionCount",
      _EC + "StepExecutionCount", _EC + "EarlyStartStepExecutionCount"]),
    ("q17-maintenance-technician-where-my-run-stopped", "maintenance-technician",
     "I cancelled a run because the procedure had no answer for that machine. Does the register "
     "hold that as a gap, and is anyone changing the book to close it?",
     "A cancelled job looks like a failure on my record unless the book shows what it stopped at. "
     "I should be able to see the gap and the change that answers it from my own run, without "
     "signing in as somebody else.",
     ["KnowledgeGaps.AnsweredByModelChangeRequest", "KnowledgeGaps.AnsweringChangeTitle",
      "KnowledgeGaps.AnsweringChangeStatus", "ProcedureExecutions.StoppedAtKnowledgeGap",
      "ProcedureExecutions.StoppedAtGapStatement", "ProcedureExecutions.StoppedAtGapStatus",
      "ProcedureExecutions.StoppedAtGapChangeTitle", "ProcedureExecutions.StoppedAtGapChangeStatus"]),
    ("q17-plant-operations-manager-community-of-a-mentorship", "plant-operations-manager",
     "Which community of practice does each mentorship on my floor belong to?",
     "A mentorship is one line between two people. What I lose when the work is sent away is the "
     "community around those lines, and I cannot see it if the line does not name it.",
     ["Mentorships.CommunityLabel", "KnowledgeTransfers.KnowHowTopic"]),
    ("q17-process-steward-where-the-book-stands", "process-steward",
     "How much of this book is PKO's own vocabulary, how much is a standard PKO reuses, and how "
     "much is our own extension? And for each mined log, what percent of the paths people took "
     "match the plan?",
     "I answer for this book against an open standard. If I cannot state the three counts from a "
     "page, somebody will mistake an extension for the standard. The conformance figure is the one "
     "I am asked for as a percentage every quarter.",
     [_EC + "ExactMappingCount", _EC + "AlignedMappingCount", _EC + "ExtensionMappingCount",
      "ProcessMiningRuns.ConformancePercent"]),
]
