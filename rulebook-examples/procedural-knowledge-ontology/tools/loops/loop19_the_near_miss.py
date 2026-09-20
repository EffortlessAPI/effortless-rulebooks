"""Loop 19 — the moment the record only hinted at.

Premise: loop 18 showed that the night of July 9 on Press 7 was already in the register as a
failed zero-energy check, an unescalated hiss, and a deviation note reading "Residual
hydraulic energy released during maintenance; work stopped". That last line is the record's
dry version of the one event a safety officer would most want to know about: the die MOVED
while the machine was locked out, with the technician inside the gap. Nobody was hurt, so
nothing more was written.

This loop records that moment as data, in the vocabulary the register already has for it:
a warning sign on step 07 (the ram or die moving while locked out), observed by the
technician at 03:00 and never escalated, beside the hiss. It also puts the night's timestamps
in the order the run actually happened: the hiss AFTER the valve was closed (loop 06 seeded it
an hour before that step began), the maintenance step running from 01:56 until the work
stopped at 03:01, the three verifications after it, and the lock check during the work.
No field is added; the existing witnesses (IsUnescalatedDangerCue, UnescalatedObservationCount)
now count two signs from that night instead of one.
"""
LOOP = {
    "WitnessLoopId": "loop-19",
    "LoopNumber": 19,
    "Title": "The die moved, and the record said PASS",
    "Premise": (
        "Loop 18 read the July 9 lockout as a failed check nobody joined to its control. Reading the "
        "same run's deviation note, 'residual hydraulic energy released during maintenance; work "
        "stopped', with the safety officer's eyes showed the register was holding a near miss as a "
        "footnote: the die moved while the technician was inside the gap. The record already had a "
        "vocabulary for that (a warning sign that requires escalation) and had never used it for this."
    ),
}

SAFETY = "plant-safety-officer"
EXT = "https://effortlessapi.github.io/effortless-rulebooks/ns/pko-extension#"

ROWS = {
    "StepCues": [
        {"StepCueId": "cue-loto07-movement", "Step": "loto-07", "CueKind": "Visual",
         "Description": "The ram or die moves while the machine is locked out.",
         "SignalsIncompleteStep": True, "RequiresEscalation": True, "EscalateToRole": SAFETY,
         "SemanticTypeIri": f"{EXT}StepCue",
         "OperatorQuestion": "The die just moved. Is this machine really dead?",
         "SignalsFailureMode": "fm-loto06-residual"},
    ],
    "CueObservations": [
        # the hiss was heard after the valve was closed (step 04b, 00:35-00:40), not an hour before it
        {"CueObservationId": "obs-0709-hiss", "ObservedAt": "2026-07-10T00:38:00-05:00"},
        # the near miss: the die dropped at 03:00 with the technician inside the gap; nobody was told
        {"CueObservationId": "obs-0709-die-moved", "StepExecution": "se-loto0709-07",
         "StepCue": "cue-loto07-movement", "ObservedAt": "2026-07-10T03:00:00-05:00",
         "ObservedByAgent": "ken-watanabe", "WasEscalated": False,
         "SemanticTypeIri": f"{EXT}CueObservation"},
        # the start-button try-out belongs to the first verification after the work stopped
        {"CueObservationId": "obs-0709-tryout", "ObservedAt": "2026-07-10T03:05:00-05:00"},
    ],
    "StepExecutions": [
        # maintenance began at 01:56, before zero energy was confirmed, and stopped at 03:01 when the die moved
        {"StepExecutionId": "se-loto0709-07", "StartedAt": "2026-07-10T01:56:00-05:00", "EndedAt": "2026-07-10T03:01:00-05:00",
         "Description": "Residual hydraulic energy released during maintenance; the die dropped; work stopped"},
        # the three verifications all follow the work stopping
        {"StepExecutionId": "se-loto0709-06", "StartedAt": "2026-07-10T03:03:00-05:00", "EndedAt": "2026-07-10T03:08:00-05:00"},
    ],
    "ConditionChecks": [
        # the lock invariant was checked during the work, not before it began
        {"ConditionCheckId": "cc-0709-07-locks", "CheckedAt": "2026-07-10T02:30:00-05:00"},
    ],
    "ProcedureExecutions": [
        {"ProcedureExecutionId": "exec-loto-0709-south-night",
         "Observations": "Hiss after the valve closed; carried on to maintenance. The die dropped during the work; stepped clear; nobody hurt."},
    ],
}
