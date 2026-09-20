"""Loop 20 — a "no" cannot become a "done".

Premise: loops 18 and 19 read the night of July 9 to the bottom. At 01:00 the technician
answered the step's own precondition check, "zero energy has been verified at every isolation
point", with no (Held = FALSE). At 01:56 the same technician marked the maintenance step
started, and later done, and the register accepted it. A checklist that writes answers down and
never reads them is what let a no be followed by a done. The rule that existed to stop it
("no maintenance starts until zero energy is verified") was owned by the safety officer, and
nothing on the safety officer's desk ever said "this job broke your rule": the register only
scored a rule when a person evaluated a run against it, and nobody ever had.

Two things follow, both askable only now:

- The technician's question. When a check on my step says no, the register must stop offering
  "Done" and offer only the way out (stop, escalate, re-isolate). Step 07 had no such way out
  in the book at all (only step 06 fell back to 09); it does now.
- The safety officer's question. Once a rule has a computed witness (loop 18's "name the
  column"), a failed check of the condition that enforces it is a breach the register can count
  on its own, without waiting for anyone to look. Those are the rules broken "by the register's
  own count", and they belong on the safety desk the minute they are recorded.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import calc, idx, lookup  # noqa: E402

LOOP = {
    "WitnessLoopId": "loop-20",
    "LoopNumber": 20,
    "Title": "A no cannot become a done",
    "Premise": (
        "Loop 18 joined a failed check to the control it breached and loop 19 recorded the near "
        "miss that followed. Reading those together with the technician's runner showed the exact "
        "moment the register failed the technician: it accepted 'done' on a step whose own check "
        "had just recorded 'no', and the safety officer's desk had no place where a breach the "
        "register could already compute would appear. Both questions only became askable once a "
        "check could name the rule it enforces and a rule could name the column that computes it."
    ),
}

FIELDS = {
    "StepExecutions": [
        calc("IsBlockedByFailedPrecondition", "boolean",
             "A check on this step recorded that a precondition did not hold. The step may not be "
             "marked done; only a fallback is offered, exactly as for an observed warning sign.",
             "={{FailedPreconditionCount}} > 0"),
    ],
    "StepConditions": [
        lookup("EnforcedRequirementIsComputed", "boolean",
               "Whether the requirement this condition enforces has a computed witness (loop 18): a "
               "failed check of it is then a breach the register counts on its own.",
               idx("Requirements", "DerivedHasComputedWitness", "EnforcesRequirement")),
        lookup("EnforcedRequirementStatement", "string",
               "The statement of the requirement this condition enforces, for the desk that shows a breach.",
               idx("Requirements", "Statement", "EnforcesRequirement")),
    ],
    "ConditionChecks": [
        lookup("EnforcedRequirementIsComputed", "boolean",
               "Copied from the checked condition: the rule it enforces has a computed witness.",
               idx("StepConditions", "EnforcedRequirementIsComputed", "StepCondition")),
        lookup("EnforcedRequirementStatement", "string",
               "Copied from the checked condition: the rule it enforces, in its own words.",
               idx("StepConditions", "EnforcedRequirementStatement", "StepCondition")),
        calc("IsScoredBreach", "boolean",
             "This check recorded that the condition did not hold, and the rule it enforces is computed "
             "by the register: a breach counted by the register itself, not by a person looking.",
             "=AND({{Held}} = FALSE, {{EnforcedRequirementIsComputed}})"),
    ],
}

ROWS = {
    "StepTransitions": [
        # step 07 had no way out when its own precondition failed; step 06 already fell back to 09
        {"StepTransitionId": "loto-07-fallback-loto-09", "ProcedureVersion": "loto-v2.0.0",
         "FromStep": "loto-07", "ToStep": "loto-09", "TransitionKind": "Fallback",
         "Condition": "Zero energy not verified: stop, keep the locks on, escalate", "Priority": 3,
         "SemanticTypeIri": "https://w3id.org/pko#Transition"},
    ],
}

QUESTIONS = [
    ("q20-maintenance-technician-no-then-done", "maintenance-technician",
     "When a check on the step I am on says no, does the register stop offering me Done?",
     "On July ninth I answered the zero-energy check with no at one in the morning and the register "
     "let me mark the maintenance step done anyway. If the only thing that stops me is my own "
     "judgement at two in the morning, the checklist is a notebook. When I say no, the only way "
     "forward should be the way out.",
     ["StepExecutions.IsBlockedByFailedPrecondition"]),
    ("q20-plant-safety-officer-scored-breaches", "plant-safety-officer",
     "Which of my rules has the register itself counted as broken, without waiting for me to look?",
     "A rule I answer for is only ever marked broken when I evaluate a run against it, and I had "
     "never evaluated one. Once the book knows which column computes a rule, a failed check of the "
     "condition that enforces it is a breach the register can count the minute it is recorded, and "
     "it belongs on my desk that minute, not in a card I read on Monday.",
     ["StepConditions.EnforcedRequirementIsComputed", "StepConditions.EnforcedRequirementStatement",
      "ConditionChecks.EnforcedRequirementIsComputed", "ConditionChecks.EnforcedRequirementStatement",
      "ConditionChecks.IsScoredBreach"]),
]
