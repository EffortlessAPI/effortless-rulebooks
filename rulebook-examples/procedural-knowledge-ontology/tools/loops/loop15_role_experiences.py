"""Loop 15: the register becomes an app people work in.

Loops 1 to 14 made the model answer questions. This loop makes it something a technician, a
knowledge engineer or a sourcing manager can act in: each role gets a home built for its
job, what a role may change is declared as data, and the vertical cut that keeps a plant
sign-in from ever seeing the finance close is derived from who owns each procedure.

See ROLE-EXPERIENCES.md. Applied with tools/apply_loop_spec.py; the write policies, the new
sign-ins and the AppActions rows that reference them are seeded afterwards by
tools/seed_role_experiences.py (they depend on policy ids that script generates).
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import EXT, agg, calc, idx, lookup, raw, rel  # noqa: E402

LOOP = {
    "WitnessLoopId": "loop-15",
    "LoopNumber": 15,
    "Title": "Loop 15: the register becomes an app people work in",
    "Premise": ("Every earlier loop asked what the model could witness. None asked what a person could DO "
                "in it: all 202 access policies were SELECT, the twelve role profiles covered only the two "
                "office procedures, and nothing said which sign-in may record an execution, write down a "
                "skill, or decide a change. These questions only became askable once the plant, sourcing "
                "and governance roles of loops 6 to 14 existed."),
}

OWNER_ORG_DESC = ("The organization that owns the procedure this row belongs to, one hop at a time from "
                  "Procedures.OwnerOrganization. A row policy compares it to the caller's organization, so a "
                  "plant sign-in never sees the finance close.")

FIELDS = {
    "ProcedureVersions": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                                 idx("Procedures", "OwnerOrganization", "Procedure"))],
    "Steps": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                     idx("ProcedureVersions", "OwnerOrganization", "ProcedureVersion"))],
    "ProcedureExecutions": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                                   idx("ProcedureVersions", "OwnerOrganization", "ProcedureVersion"))],
    "KnowledgeFragments": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                                  idx("ProcedureVersions", "OwnerOrganization", "ProcedureVersion"))],
    "ChangeRequests": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                              idx("ProcedureVersions", "OwnerOrganization", "ProcedureVersion"))],
    "KnowledgeGaps": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                             idx("ProcedureVersions", "OwnerOrganization", "ProcedureVersion"))],
    "StepExecutions": [
        lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
               idx("ProcedureExecutions", "OwnerOrganization", "ProcedureExecution")),
        agg("IncompleteCueObservationCount", "integer",
            "Warning signs observed on this step execution that mean the step is not finished.",
            "=COUNTIFS(CueObservations!{{StepExecution}}, {{StepExecutionId}}, "
            "CueObservations!{{CueSignalsIncompleteStep}}, TRUE)"),
        calc("IsBlockedByObservedCue", "boolean",
             "A warning sign that means 'not finished' was seen on this step execution, so the normal "
             "next step must not be offered; only a fallback is.",
             "={{IncompleteCueObservationCount}} > 0"),
    ],
    "CueObservations": [
        lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
               idx("StepExecutions", "OwnerOrganization", "StepExecution")),
        lookup("CueSignalsIncompleteStep", "boolean",
               "Whether the observed warning sign means the step is not finished.",
               idx("StepCues", "SignalsIncompleteStep", "StepCue")),
        raw("AcknowledgedAt", "datetime",
            "When the person it was escalated to acknowledged it. Blank until they do."),
        calc("IsAwaitingAcknowledgement", "boolean",
             "Escalated to someone who has not acknowledged it yet: the safety desk's inbox.",
             "=AND({{WasEscalated}} = TRUE, {{AcknowledgedAt}} = \"\")"),
    ],
    "AssistantAnswers": [lookup("OwnerOrganization", "string", OWNER_ORG_DESC,
                                idx("StepExecutions", "OwnerOrganization", "StepExecution"))],
    "StepCues": [
        raw("OperatorQuestion", "string",
            "How a technician on the floor would ask about this warning sign. The assistant offers it verbatim."),
        rel("SignalsFailureMode", "FailureModes",
            "The failure mode this warning sign is evidence of. Its recorded response is the assistant's answer."),
        lookup("FailureModeResponse", "string", "What to do, from the failure mode this sign points at.",
               idx("FailureModes", "Response", "SignalsFailureMode")),
        calc("IsUnanswerableSign", "boolean",
             "A sign that means 'not finished' but points at no failure mode, so neither the runner nor the "
             "assistant can say what to do about it.",
             "=AND({{SignalsIncompleteStep}} = TRUE, {{SignalsFailureMode}} = \"\")"),
    ],
    "AppRoleProfiles": [
        raw("Device", "string", "The device this role's experience is designed for: phone or tablet."),
        raw("HomeRoute", "string", "The route this role lands on after signing in."),
        raw("HomeTitle", "string", "The title of that home page, as the role would say it."),
    ],
}

TABLES = [
    ("AppActions",
     "One row per thing a role may DO in the app: the base table it writes, the operation, the access "
     "policy that permits it, and the derived value the page highlights afterwards. The app has no "
     "per-action endpoint; it executes these rows.",
     "application",
     [raw("AppActionId", "string", "Stable key, e.g. act-tech-start-lockout.", nullable=False),
      calc("Name", "string", "Display name.", "={{Label}}"),
      raw("Label", "string", "The button's words."),
      rel("OwningRole", "Roles", "The role that performs this action."),
      raw("RoutePath", "string", "The app route the action appears on."),
      rel("TargetTable", "RulebookTables", "The base table written."),
      raw("Operation", "string", "INSERT or UPDATE, spelled as the policy's Command is."),
      rel("Policy", "AccessPolicies", "The write policy that permits it. Blank means nothing permits it."),
      rel("WatchedField", "RulebookFields", "The derived value the page highlights after the write."),
      raw("StoryEpisode", "integer", "The episode of the video series that performs this action on camera."),
      raw("Description", "string", "What the action means, in the role's words."),
      lookup("PolicyCommand", "string", "The permitting policy's SQL command.",
             idx("AccessPolicies", "Command", "Policy", "AccessPolicyId")),
      lookup("PolicyDenialTestCount", "integer", "Denial tests that exercise the permitting policy.",
             idx("AccessPolicies", "DenialTestCount", "Policy", "AccessPolicyId")),
      lookup("WatchedFieldIsWitness", "boolean", "Whether the highlighted value is a boolean witness.",
             idx("RulebookFields", "IsWitness", "WatchedField", "RulebookFieldId")),
      agg("InputFieldCount", "integer", "Fields this action writes.",
          "=COUNTIFS(AppActionFields!{{AppAction}}, {{AppActionId}})"),
      calc("IsUnpermitted", "boolean", "No access policy permits this action, so the database would refuse it.",
           "={{Policy}} = \"\""),
      calc("PolicyCommandDisagrees", "boolean",
           "The permitting policy is for a different SQL command than the action performs.",
           "=AND({{Policy}} <> \"\", {{Operation}} <> {{PolicyCommand}})"),
      calc("IsUnprovenWrite", "boolean",
           "Permitted, but no denial test proves the policy refuses anyone else.",
           "=AND({{Policy}} <> \"\", {{PolicyDenialTestCount}} = 0)"),
      raw("SemanticTypeIri", "string", "Extension IRI.")]),
    ("AppActionFields",
     "The fields one action writes, one row each, and where each value comes from: typed by the person, "
     "picked from a list, fixed by the action, or stamped by the server.",
     "application",
     [raw("AppActionFieldId", "string", "Stable key.", nullable=False),
      calc("Name", "string", "Display name.", "={{AppAction}} & \" / \" & {{FieldLabel}}"),
      rel("AppAction", "AppActions", "The action."),
      rel("TargetField", "RulebookFields", "The field written."),
      raw("FieldLabel", "string", "The label shown beside the input."),
      raw("InputKind", "string", "text, longtext, note (a long text that may be left empty), choice, toggle, date, fixed, or server."),
      raw("FixedValue", "string", "For fixed: the value. For server: what is stamped (agent, instant, id, iri)."),
      raw("ChoicesFrom", "string", "For choice: the table whose rows are offered."),
      raw("SortOrder", "integer", "Position in the form."),
      lookup("TargetFieldType", "string", "raw, relationship, calculated, lookup or aggregation.",
             idx("RulebookFields", "FieldType", "TargetField", "RulebookFieldId")),
      calc("WritesDerivedField", "boolean",
           "The action tries to write a value the model works out for itself. That is never allowed.",
           "=AND({{TargetField}} <> \"\", {{TargetFieldType}} <> \"raw\", {{TargetFieldType}} <> \"relationship\")"),
      raw("SemanticTypeIri", "string", "Extension IRI.")]),
]

QUESTIONS = [
    ("q15-tech-may-i-carry-on", "maintenance-technician",
     "I have seen a warning sign on the step I am on. May I carry on to the next step, or is the only way "
     "forward the fallback?",
     "A checklist that lets you tick past a gauge that is not at zero is a record of an accident, not a "
     "control. The way forward has to change the moment the sign is recorded.",
     ["CueObservations.CueSignalsIncompleteStep", "StepExecutions.IncompleteCueObservationCount",
      "StepExecutions.IsBlockedByObservedCue"]),
    ("q15-tech-what-does-this-sign-mean", "maintenance-technician",
     "When I see one of the warning signs on a step and ask about it in my own words, what does the book say "
     "I should do, and is there any sign it has no answer for?",
     "An assistant that answers from the book can only be as good as the link between what a technician sees "
     "and what the procedure says to do about it.",
     ["StepCues.OperatorQuestion", "StepCues.SignalsFailureMode", "StepCues.FailureModeResponse",
      "StepCues.IsUnanswerableSign"]),
    ("q15-safety-what-is-waiting-on-me", "plant-safety-officer",
     "Which warning signs have been escalated to me that I have not acknowledged yet?",
     "An escalation nobody has picked up is no better than one nobody sent.",
     ["CueObservations.AcknowledgedAt", "CueObservations.IsAwaitingAcknowledgement"]),
    ("q15-safety-whose-procedure-is-this", "plant-safety-officer",
     "Which procedures, runs, observations, knowledge and change requests belong to my part of the company, "
     "so that someone signed in at the plant never sees the finance close and the reverse?",
     "Tenancy that is written into each screen gets forgotten on the next screen. Derived once from who owns "
     "the procedure, a one-line row policy carries it everywhere.",
     ["ProcedureVersions.OwnerOrganization", "Steps.OwnerOrganization", "ProcedureExecutions.OwnerOrganization",
      "StepExecutions.OwnerOrganization", "CueObservations.OwnerOrganization",
      "AssistantAnswers.OwnerOrganization", "KnowledgeFragments.OwnerOrganization",
      "ChangeRequests.OwnerOrganization", "KnowledgeGaps.OwnerOrganization"]),
    ("q15-steward-what-may-each-role-change", "process-steward",
     "What can each role change from the app, which policy permits it, is that policy for the right command, "
     "has anyone proven it refuses everybody else, and does any action try to write a value the model derives?",
     "The moment an app can write, the access model is either data you can audit or code you have to trust.",
     ["AppActions.Label", "AppActions.OwningRole", "AppActions.RoutePath", "AppActions.TargetTable",
      "AppActions.Operation", "AppActions.Policy", "AppActions.WatchedField", "AppActions.StoryEpisode",
      "AppActions.Description", "AppActions.PolicyCommand", "AppActions.PolicyDenialTestCount",
      "AppActions.WatchedFieldIsWitness", "AppActions.InputFieldCount", "AppActions.IsUnpermitted",
      "AppActions.PolicyCommandDisagrees", "AppActions.IsUnprovenWrite",
      "AppActionFields.AppAction", "AppActionFields.TargetField", "AppActionFields.FieldLabel",
      "AppActionFields.InputKind", "AppActionFields.FixedValue", "AppActionFields.ChoicesFrom",
      "AppActionFields.SortOrder", "AppActionFields.TargetFieldType", "AppActionFields.WritesDerivedField"]),
    ("q15-steward-where-does-each-role-land", "process-steward",
     "When each role signs in, which page do they land on, what is it called, and is it built for a phone in "
     "a hand or a tablet on a desk?",
     "A technician at a press and a knowledge engineer at a desk do not want the same screen.",
     ["AppRoleProfiles.Device", "AppRoleProfiles.HomeRoute", "AppRoleProfiles.HomeTitle"]),
]

ROWS = {
    "StepCues": [
        {"StepCueId": "cue-loto06-gauge", "OperatorQuestion": "The gauge is just above zero. Is that zero?",
         "SignalsFailureMode": "fm-loto06-residual"},
        # Deliberately left without a failure mode: the hiss is a real sign with no recorded response,
        # so IsUnanswerableSign fires on it. Closing that is knowledge-engineering work, not seeding.
        {"StepCueId": "cue-loto04b-hiss", "OperatorQuestion": "I can still hear a hiss after closing the valve. Has it bled?"},
        {"StepCueId": "cue-loto06-tryout", "OperatorQuestion": "I pressed start and nothing happened. Is that what I want?"},
        {"StepCueId": "cue-loto03-rundown", "OperatorQuestion": "The belt has stopped. Can I start isolating?"},
    ],
}
