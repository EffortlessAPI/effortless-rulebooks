"""Loop 18 — a check you have never seen fail is a check you have never seen work.

Premise: loops 10 and 11 gave every blocking requirement a control-health verdict
(ControlAssuranceState: Decorative, Inoperative, Asserted, Demonstrated, Holding, Untested),
and loop 15 let people sign in and act. Reading the lockout procedure through those two
together found something neither could say alone.

On the night of July 9, on Press 7, the execution record holds everything a safety officer
would need: a hiss after the pneumatic valve closed (a cue that requires escalation, not
escalated), a precondition check at 01:00 on "Zero energy has been verified at every
isolation point" that recorded Held = FALSE, maintenance completed anyway
(StepExecutions.ProceededDespiteFailedPrecondition), and a lock invariant broken at 01:30.
Every one of those step executions says PASS.

And the requirement that exists to stop exactly that — req-loto-zero-energy, "No
maintenance starts until zero energy is verified at every isolation point" — reads
Inoperative: bound to a step, never once evaluated. Its record says it has never failed,
because nobody has ever asked it. The book held the breach in ConditionChecks and the
control in Requirements, and nothing connected the two.

This loop connects them. A step condition may name the requirement it enforces; a failed
check then counts against that requirement; and a control whose own record says it has
never failed, while the book holds a failed check of the condition that enforces it, is
witnessed as such. The PASS marks recorded on a control's steps are counted beside its
evaluations, so "six PASS, zero evaluations" is a number on a page and not a sentence.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import agg, calc, idx, lookup, rel  # noqa: E402

LOOP = {
    "WitnessLoopId": "loop-18",
    "LoopNumber": 18,
    "Title": "A check you have never seen fail is a check you have never seen work",
    "Premise": (
        "Loops 10 and 11 gave each blocking control a health verdict and loop 15 let people act "
        "in the app. Read together, they showed the lockout control that stands between a person "
        "and a live machine reading Inoperative while the execution record held a failed check of "
        "the very condition it exists to enforce, every step still marked PASS. The question "
        "'has this control ever been seen to fail, when the book says it did' only became askable "
        "once both halves existed."
    ),
}

FIELDS = {
    "StepConditions": [
        rel("EnforcesRequirement", "Requirements",
            "The blocking requirement this condition is the physical check of, when it is one. A "
            "failed check of the condition is then a failure of that requirement."),
    ],
    "ConditionChecks": [
        lookup("EnforcedRequirement", "string",
               "The requirement the checked condition enforces, copied from the condition.",
               idx("StepConditions", "EnforcesRequirement", "StepCondition")),
        calc("FailedCheckRequirementKey", "string",
             "Echoes the enforced requirement's id when this check recorded that the condition did "
             "not hold; blank otherwise.",
             '=IF({{Held}} = FALSE, {{EnforcedRequirement}}, "")'),
    ],
    "StepRequirements": [
        agg("PassCountOnStep", "integer",
            "How many executions of this binding's step were recorded as PASS.",
            '=COUNTIFS(StepExecutions!{{Step}}, {{Step}}, StepExecutions!{{VerificationResult}}, "PASS")'),
    ],
    "Requirements": [
        agg("RecordedPassCount", "integer",
            "PASS marks recorded on every step this control is bound to. Set beside "
            "SatisfactionRecordCount, it says how often somebody passed the step against how often "
            "anybody asked whether the control held.",
            "=SUMIFS(StepRequirements!{{PassCountOnStep}}, StepRequirements!{{Requirement}}, {{RequirementId}})"),
        calc("IsPassedButNeverEvaluated", "boolean",
             "TRUE for a blocking control whose steps carry PASS marks while the control itself has "
             "never once been evaluated. The green on those steps says nothing about the control.",
             "=AND({{IsInoperativeControl}}, {{RecordedPassCount}} > 0)"),
        agg("FailedCheckCount", "integer",
            "Checks on record, of a condition that enforces this control, that found the condition "
            "did not hold.",
            "=COUNTIFS(ConditionChecks!{{FailedCheckRequirementKey}}, {{RequirementId}})"),
        calc("IsBreachedButNeverFailed", "boolean",
             "TRUE when the book holds a failed check of a condition this control enforces, and the "
             "control's own record still says it has never failed. The breach is on record; the "
             "control never heard about it.",
             "=AND({{FailedCheckCount}} > 0, NOT({{HasEverProducedNegative}}))"),
    ],
}

# The July 9 night run on Press 7 (exec-loto-0709-south-night) started at 22:00 and ended at 04:30
# the next morning, but its thirteen step executions were seeded at 07:00 to 13:29 on July 9: before
# the run began. Every step is moved by the same fifteen hours, so each one falls inside the run and
# every duration and every ordering the rulebook derives from them is unchanged. Its condition checks
# (22:30, 01:00, 01:30) and cue observations (23:10, 02:00) were already on the night.
_NIGHT_OF_JULY_9 = [
        {"StepExecutionId": "se-loto0709-01", "StartedAt": "2026-07-09T22:00:00-05:00", "EndedAt": "2026-07-09T22:05:00-05:00"},
        {"StepExecutionId": "se-loto0709-02", "StartedAt": "2026-07-09T22:07:00-05:00", "EndedAt": "2026-07-09T22:12:00-05:00"},
        {"StepExecutionId": "se-loto0709-03", "StartedAt": "2026-07-09T23:14:00-05:00", "EndedAt": "2026-07-09T23:19:00-05:00"},
        {"StepExecutionId": "se-loto0709-04", "StartedAt": "2026-07-09T23:21:00-05:00", "EndedAt": "2026-07-09T23:26:00-05:00"},
        {"StepExecutionId": "se-loto0709-04a", "StartedAt": "2026-07-10T00:28:00-05:00", "EndedAt": "2026-07-10T00:33:00-05:00"},
        {"StepExecutionId": "se-loto0709-04b", "StartedAt": "2026-07-10T00:35:00-05:00", "EndedAt": "2026-07-10T00:40:00-05:00"},
        {"StepExecutionId": "se-loto0709-04c", "StartedAt": "2026-07-10T01:42:00-05:00", "EndedAt": "2026-07-10T01:47:00-05:00"},
        {"StepExecutionId": "se-loto0709-05", "StartedAt": "2026-07-10T01:49:00-05:00", "EndedAt": "2026-07-10T01:54:00-05:00"},
        {"StepExecutionId": "se-loto0709-07", "StartedAt": "2026-07-10T02:56:00-05:00", "EndedAt": "2026-07-10T02:01:00-05:00"},
        {"StepExecutionId": "se-loto0709-06", "StartedAt": "2026-07-10T02:03:00-05:00", "EndedAt": "2026-07-10T02:08:00-05:00"},
        {"StepExecutionId": "se-loto0709-06b", "StartedAt": "2026-07-10T03:10:00-05:00", "EndedAt": "2026-07-10T03:15:00-05:00"},
        {"StepExecutionId": "se-loto0709-06c", "StartedAt": "2026-07-10T03:17:00-05:00", "EndedAt": "2026-07-10T03:22:00-05:00"},
        {"StepExecutionId": "se-loto0709-08", "StartedAt": "2026-07-10T04:24:00-05:00", "EndedAt": "2026-07-10T04:29:00-05:00"},
]

# The lockout conditions are the physical checks of the two lockout controls. Nothing else in
# the seed has a condition that is the check of a blocking requirement.
ROWS = {
    "StepConditions": [
        {"StepConditionId": "cond-loto06-post-zero", "EnforcesRequirement": "req-loto-zero-energy"},
        {"StepConditionId": "cond-loto07-pre-zeroenergy", "EnforcesRequirement": "req-loto-zero-energy"},
        {"StepConditionId": "cond-loto07-inv-locks", "EnforcesRequirement": "req-loto-personal-lock"},
    ],
    "StepExecutions": _NIGHT_OF_JULY_9,
}

QUESTIONS = [
    ("q18-plant-safety-officer-passed-never-evaluated", "plant-safety-officer",
     "Of the controls I answer for, which carry PASS marks on their steps and have never once been "
     "evaluated themselves?",
     "A PASS on the step tells me somebody finished it. It does not tell me the control held. If a "
     "control has only ever been passed and never asked, its clean record is not evidence of "
     "anything, and I need to see the two numbers side by side to know which of mine that is.",
     ["StepRequirements.PassCountOnStep", "Requirements.RecordedPassCount",
      "Requirements.IsPassedButNeverEvaluated"]),
    ("q18-plant-safety-officer-breached-never-failed", "plant-safety-officer",
     "Has the book recorded a condition failing that one of my controls exists to enforce, while "
     "that control's own record still says it has never failed?",
     "A failed check of 'zero energy has been verified' is the zero-energy control failing. If the "
     "check sits in the execution record and the control still reads as never having failed, the "
     "two halves of the book have never met, and I am the person who has to be told.",
     ["StepConditions.EnforcesRequirement", "ConditionChecks.EnforcedRequirement",
      "ConditionChecks.FailedCheckRequirementKey", "Requirements.FailedCheckCount",
      "Requirements.IsBreachedButNeverFailed"]),
]
