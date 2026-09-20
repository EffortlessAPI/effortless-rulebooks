#!/usr/bin/env python3
"""Seed everything loop 15's role experiences need that is DATA, then re-seed access.

  1. AccessPrincipals + RoleSchemas for the roles that work in the app
  2. AppUsers + PrincipalAssignments for the people the video series follows
  3. AppRoleProfiles (pitch, device, home) for those roles; device + home for the twelve older ones
  4. AppActions + AppActionFields: what each role may change, field by field
  5. tools/reseed_role_profiles.py, which turns role_profiles.py + the actions into
     AccessPolicies (SELECT, INSERT, UPDATE), FieldGrants (CanWrite) and RoleSchemaViews

Idempotent: every row is upserted by key. Re-run after editing this file or role_profiles.py,
then `bash tools/quick_build.sh`.
"""
import json
import os
import re
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
RB = os.path.join(ROOT, "effortless-rulebook", "procedural-knowledge-ontology-rulebook.json")
IRI = "urn:effortless:pko-extension#"

# agent -> the role they sign in as. Signing in is independent of Roles.CurrentAgent: Aisha and
# Ken work as maintenance technicians without being the role's single recorded holder.
PEOPLE = [
    ("tomas-reyes", "maintenance-technician"),
    ("aisha-bello", "maintenance-technician"),
    ("ken-watanabe", "maintenance-technician"),
    ("lin-zhao", "plant-safety-officer"),
    ("hana-kowalski", "plant-operations-manager"),
    ("sam-adeyemi", "knowledge-engineer"),
    ("claire-dubois", "sourcing-manager"),
    ("grace-holloway", "release-manager"),
    ("nadia-petrova", "ontology-authority"),
    ("plant-copilot-1-2", "plant-assistant"),
]

# role -> (kind, accent, icon, device, home route, home title, pitch in the role's own voice)
PROFILES = {
    "maintenance-technician": ("human", "#0f766e", "padlock", "phone", "/maintenance-technician/my-lockout",
                               "My Lockout", "Lock it out, step by step, with what the veterans know beside each step."),
    "plant-assistant": ("software", "#7c3aed", "spark", "phone", "/plant-assistant/ask",
                        "Ask the Copilot", "Answer from the book, show the rows I used, and never guess the next step."),
    "plant-safety-officer": ("human", "#b91c1c", "shield", "phone", "/plant-safety-officer/desk",
                             "Safety Desk", "See every warning sign that reached me, and every one that should have."),
    "plant-operations-manager": ("human", "#b45309", "factory", "phone", "/plant-operations-manager/floor",
                                 "The Floor, This Quarter", "Know who is proficient, what is about to walk out of the door, and what is waiting on me."),
    "knowledge-engineer": ("human", "#1d4ed8", "notebook", "tablet", "/knowledge-engineer/workbench",
                           "Capture Workbench", "Get it out of people's heads: collect it, organize it, encode it, and prove where it came from."),
    "sourcing-manager": ("human", "#0369a1", "handshake", "tablet", "/sourcing-manager/what-we-still-know",
                         "What We Still Know", "For every company we depend on: who owns the know-how, and can we get it back?"),
    "release-manager": ("human", "#4338ca", "rocket", "tablet", "/release-manager/release-gate",
                        "Release Gate", "Before I upgrade an agent, show me everything it touches, not what I remembered."),
    "ontology-authority": ("human", "#6d28d9", "scales", "tablet", "/ontology-authority/change-board",
                           "Change Board", "Be the second pair of eyes on anything that changes what the book concludes."),
}

S, A, I, IDK = "server", "agent", "instant", "id"


def F(field, label, kind, fixed="", choices=""):
    return dict(field=field, label=label, kind=kind, fixed=fixed, choices=choices)


# (id, role, route, label, table, operation, watched field, episode, description, [fields])
ACTIONS = [
    ("act-tech-start-lockout", "maintenance-technician", "/maintenance-technician/my-lockout", "Start a lockout",
     "ProcedureExecutions", "INSERT", "ProcedureExecutions.OwnerOrganization", 4,
     "Begin a new run of the live lockout procedure on one machine. The run is a record of its own, linked to the procedure version.",
     [F("ProcedureExecutionId", "Run", S, IDK), F("ProcedureVersion", "Procedure version", "fixed", "loto-v2.0.0"),
      F("ExecutionStatus", "Status", "fixed", "InProgress"), F("StartedAt", "Started", S, I),
      F("ExecutedByAgent", "Technician", S, A), F("Title", "Title", S, "title"),
      F("ExecutedOnMachine", "Machine", "choice", choices="Machines"),
      F("Facility", "Hall", "choice", choices="Facilities"), F("Shift", "Shift", "choice", choices="Day|Night"),
      F("SemanticTypeIri", "Type", S, "iri")]),
    ("act-tech-begin-step", "maintenance-technician", "/maintenance-technician/my-lockout", "Begin step",
     "StepExecutions", "INSERT", "StepExecutions.IsOutOfSpecifiedOrder", 4,
     "I am starting this step now. The server links it to the step I did before.",
     [F("StepExecutionId", "Step run", S, IDK), F("ProcedureExecution", "Run", "context"), F("Step", "Step", "context"),
      F("ExecutedByAgent", "Technician", S, A), F("ExecutionStatus", "Status", "fixed", "InProgress"),
      F("StartedAt", "Started", S, I), F("PreviousStepExecution", "Previous step run", S, "previous"),
      F("SemanticTypeIri", "Type", S, "iri")]),
    ("act-tech-complete-step", "maintenance-technician", "/maintenance-technician/my-lockout", "Done",
     "StepExecutions", "UPDATE", "StepExecutions.IsCompleted", 4,
     "I have carried out this step. PASS if it went as written, WARN if I am leaving it by a fallback.",
     [F("ExecutionStatus", "Status", "fixed", "Completed"), F("EndedAt", "Ended", S, I),
      F("VerificationResult", "Verification", "context")]),
    ("act-tech-observe-cue", "maintenance-technician", "/maintenance-technician/my-lockout", "I see this",
     "CueObservations", "INSERT", "StepExecutions.IsBlockedByObservedCue", 4,
     "Record that I saw this warning sign on the step I am on.",
     [F("CueObservationId", "Observation", S, IDK), F("StepExecution", "Step run", "context"),
      F("StepCue", "Warning sign", "context"), F("ObservedAt", "Seen at", S, I),
      F("ObservedByAgent", "Seen by", S, A), F("WasEscalated", "Escalated", "fixed", "false"),
      F("SemanticTypeIri", "Type", S, "iri")]),
    ("act-tech-escalate", "maintenance-technician", "/maintenance-technician/my-lockout", "Escalate",
     "CueObservations", "UPDATE", "CueObservations.IsAwaitingAcknowledgement", 4,
     "Send what I saw to the role the warning sign says it must go to.",
     [F("WasEscalated", "Escalated", "fixed", "true"), F("EscalatedToAgent", "Escalated to", S, "cue-escalation-holder")]),
    ("act-tech-close-run", "maintenance-technician", "/maintenance-technician/my-lockout", "Finish this run",
     "ProcedureExecutions", "UPDATE", "ProcedureExecutions.OwnerOrganization", 4,
     "Finish, pause or cancel my own run, with what I noticed.",
     [F("ExecutionStatus", "Status", "choice", choices="Completed|Paused|Cancelled"), F("EndedAt", "Ended", S, I),
      F("Outcome", "Outcome", "text"), F("Observations", "What I noticed", "longtext")]),
    ("act-tech-ask", "maintenance-technician", "/maintenance-technician/my-lockout", "Ask the Copilot",
     "AssistantAnswers", "INSERT", "AssistantAnswers.OwnerOrganization", 4,
     "Ask one of the questions the book can answer about the step I am on. The answer is worked out by "
     "structured query, as the Copilot's own narrow sign-in, and recorded against me.",
     [F("AssistantAnswerId", "Answer", S, IDK), F("AnsweringAgent", "Answered by", "fixed", "plant-copilot-1-2"),
      F("AskedByAgent", "Asked by", S, A), F("StepExecution", "Step run", "context"),
      F("AskedAt", "Asked", S, I), F("AnsweredAt", "Answered", S, I), F("AnswerKind", "Kind", "fixed", "Guidance"),
      F("QuestionTopic", "Topic", S, "answer"), F("QuestionText", "Question", S, "answer"),
      F("AnswerText", "Answer", S, "answer"), F("RetrievalMode", "Retrieval", "fixed", "StructuredQuery"),
      F("DerivationPerformedBy", "Reasoning by", "fixed", "StructuredQuery"),
      F("AssumedCurrentStep", "Current step", S, "answer"), F("AssertedNextStep", "Next step", S, "answer"),
      F("RaisedSafetyConcern", "Safety concern", S, "answer"),
      F("DeliveryDisposition", "Delivery", "fixed", "Delivered"), F("SemanticTypeIri", "Type", S, "iri")]),

    ("act-safety-acknowledge", "plant-safety-officer", "/plant-safety-officer/desk", "Acknowledge",
     "CueObservations", "UPDATE", "CueObservations.IsAwaitingAcknowledgement", 4,
     "I have seen this escalation and I am dealing with it.",
     [F("AcknowledgedAt", "Acknowledged", S, I)]),
    ("act-safety-validate-insight", "plant-safety-officer", "/plant-safety-officer/desk", "Record my verdict",
     "AiInsightProposals", "UPDATE", "AiInsightProposals.ValidationVerdict", 5,
     "A machine proposed this pattern. A person checks it against the floor and commits to a verdict, in their own name.",
     [F("ValidationVerdict", "Verdict", "choice", choices="Valid|Invalid"), F("ValidatedByAgent", "Validated by", S, A)]),
    ("act-safety-hand-up", "plant-safety-officer", "/plant-safety-officer/desk", "Hand the decision up",
     "ChangeRequests", "UPDATE", "ChangeRequests.RequesterIsAuthority", 5,
     "I raised this request, so I must not decide it. Pass the decision to another authority.",
     [F("AuthorityRole", "Decided by", "choice", choices="Roles")]),
    ("act-safety-decide", "plant-safety-officer", "/plant-safety-officer/desk", "Decide",
     "ChangeRequests", "UPDATE", "ChangeRequests.IsDecided", 5,
     "Approve or reject a request somebody else raised. The database refuses a decision on my own request.",
     [F("Status", "Decision", "choice", choices="Approved|Rejected"), F("DecidedAt", "Decided", S, I)]),
    ("act-floor-decide", "plant-operations-manager", "/plant-operations-manager/floor", "Decide",
     "ChangeRequests", "UPDATE", "ChangeRequests.IsDecided", 5,
     "Approve or reject a request that was handed up to me.",
     [F("Status", "Decision", "choice", choices="Approved|Rejected"), F("DecidedAt", "Decided", S, I)]),

    ("act-ke-write-down", "knowledge-engineer", "/knowledge-engineer/workbench", "Write this down",
     "KnowledgeRepositoryEntries", "INSERT", "KnowHowCarriers.IsAtRiskOfImminentLoss", 2,
     "Put a skill that lives in a person into the repository, crediting the person it came from.",
     [F("KnowledgeRepositoryEntryId", "Entry", S, IDK), F("Title", "Title", "text"),
      F("KnowHow", "Skill", "context"), F("Procedure", "Procedure", "context"),
      F("AuthorAgent", "Author", S, A), F("SourceExpert", "Source expert", "choice", choices="Agents"),
      F("CreditsSourceExpert", "Credit the source expert by name", "toggle"),
      F("WrittenForAudience", "Written for", "text"),
      F("AuthoredOnAllocatedTime", "Written on time set aside for it", "toggle"),
      F("CreatedAt", "Created", S, I), F("LastUpdatedAt", "Updated", S, I),
      F("ReviewIntervalDays", "Review every (days)", "fixed", "180"),
      F("EvaluationContext", "Judged as of", "fixed", "eval-current"), F("SemanticTypeIri", "Type", S, "iri")]),
    ("act-ke-hand-over", "knowledge-engineer", "/knowledge-engineer/workbench", "Record a hand-over",
     "KnowledgeTransfers", "INSERT", "KnowHowCarriers.TransferCount", 2,
     "Record that one person has shown this skill to another, and how.",
     [F("KnowledgeTransferId", "Hand-over", S, IDK), F("KnowHow", "Skill", "context"),
      F("FromAgent", "From", "context"), F("RecipientAgent", "To", "choice", choices="Agents"),
      F("Channel", "How", "choice", choices="Mentoring|TrainingProgram|Document|Collaboration"),
      F("CommunityOfPractice", "Community", "context"), F("OccurredAt", "When", S, I),
      F("OnAllocatedTime", "On time set aside for it", "toggle"), F("SemanticTypeIri", "Type", S, "iri")]),
    ("act-ke-add-warning-sign", "knowledge-engineer", "/knowledge-engineer/workbench", "Add a warning sign",
     "StepCues", "INSERT", "Steps.HasIncompletenessCue", 5,
     "Encode something a veteran watches for as a warning sign on the step, so the runner and the assistant can use it.",
     [F("StepCueId", "Warning sign", S, IDK), F("Step", "Step", "context"),
      F("CueKind", "Kind", "choice", choices="Visual|Indicator|Sound|Confirmation"), F("Description", "What to watch for", "longtext"),
      F("SignalsIncompleteStep", "Means the step is not finished", "toggle"),
      F("RequiresEscalation", "Must be escalated", "toggle"), F("EscalateToRole", "Escalate to", "choice", choices="Roles"),
      F("SemanticTypeIri", "Type", S, "iri")]),
    ("act-ke-mark-implemented", "knowledge-engineer", "/knowledge-engineer/workbench", "Mark implemented",
     "ChangeRequests", "UPDATE", "ChangeRequests.IsOpen", 5,
     "The decided change has now actually been made. Only then does the book call the request closed.",
     [F("ImplementedAt", "Implemented", S, I)]),

    ("act-sourcing-set-clause", "sourcing-manager", "/sourcing-manager/what-we-still-know", "Save contract",
     "ProviderEngagements", "UPDATE", "ProviderEngagements.IsKnowledgeAccessUnsecured", 3,
     "Record whether this contract gives us access to the provider's knowledge of how our work is done.",
     [F("HasKnowledgeAccessClause", "Knowledge access clause", "toggle")]),
    ("act-sourcing-require-deliverable", "sourcing-manager", "/sourcing-manager/what-we-still-know", "Require a deliverable",
     "KnowledgeDeliverables", "INSERT", "ProviderEngagements.LacksKnowledgeDeliverables", 3,
     "Require the provider to hand over a named piece of knowledge, so the book tracks a promise instead of a hole.",
     [F("KnowledgeDeliverableId", "Deliverable", S, IDK), F("ProviderEngagement", "Contract", "context"),
      F("Direction", "Delivered to", "fixed", "ToClient"), F("Title", "What must be delivered", "text"),
      F("DueAt", "Due", "date"), F("SemanticTypeIri", "Type", S, "iri")]),

    ("act-authority-record-review", "ontology-authority", "/ontology-authority/change-board", "Record authority review",
     "ModelChangeRequests", "UPDATE", "ModelChangeRequests.AuthorityReviewSkipped", 5,
     "I have read what this change to the model would alter, before it ships.",
     [F("AuthorityReviewedAt", "Reviewed", S, I)]),

    ("act-admin-move-instant", "process-steward", "/admin/context", "Move the date",
     "EvaluationContexts", "UPDATE", "KnowHowCarriers.DaysUntilHolderDeparture", 5,
     "Every time-dependent answer is judged against this instant. Moving it re-judges all of them at once.",
     [F("AsOfInstant", "Judge every answer as of", "date")]),
]


def snake(n):
    return re.sub(r"_+", "_", re.sub(r"(?<!^)(?=[A-Z])", "_", n).lower())


def upsert(rows, key, new):
    by = {r[key]: r for r in rows}
    for n in new:
        if n[key] in by:
            by[n[key]].update(n)
        else:
            rows.append(n)
            by[n[key]] = n


def fill_physical_names(rb):
    """RulebookTables.PhysicalTable / PhysicalView were only ever set for the first 80 tables, so
    the access generator silently skipped every policy and view on the 180 tables added since
    (no physical name = `continue`). Fill them from what Postgres actually has: the live
    database is the oracle for what a table is called, not a naming rule applied in Python."""
    db = os.environ.get("DATABASE_URL", "postgresql://postgres@localhost:5432/erb_procedural_knowledge_ontology")
    r = subprocess.run(["psql", db, "-tA", "-c",
                        "select table_name from information_schema.tables where table_schema='public'"],
                       capture_output=True, text=True)
    if r.returncode:
        raise SystemExit(f"FATAL: cannot read live table names: {r.stderr}")
    live = set(r.stdout.split())
    filled, absent = 0, []
    for t in rb["RulebookTables"]["data"]:
        if t.get("PhysicalTable") and t.get("PhysicalView"):
            continue
        phys = snake(t["TableName"])
        if phys in live and f"vw_{phys}" in live:
            t["PhysicalTable"], t["PhysicalView"] = phys, f"vw_{phys}"
            filled += 1
        else:
            absent.append(t["TableName"])
    print(f"physical names filled for {filled} tables; not in the live database under their snake name: {absent}")


def main():
    rb = json.load(open(RB))
    roles = {r["RoleId"]: r for r in rb["Roles"]["data"]}
    agents = {a["AgentId"]: a for a in rb["Agents"]["data"]}
    fields = {f["RulebookFieldId"] for f in rb["RulebookFields"]["data"]}
    tables = {k for k, v in rb.items() if isinstance(v, dict) and "schema" in v}

    errors = []
    for agent, role in PEOPLE:
        if agent not in agents:
            errors.append(f"PEOPLE: no agent {agent}")
        if role not in roles or role not in PROFILES:
            errors.append(f"PEOPLE: {agent} signs in as {role}, which has no role or no profile")
    for aid, role, _route, _label, table, op, watched, _ep, _desc, flds in ACTIONS:
        if role not in roles:
            errors.append(f"{aid}: no role {role}")
        if table not in tables:
            errors.append(f"{aid}: no table {table}")
        if op not in ("INSERT", "UPDATE"):
            errors.append(f"{aid}: operation {op}")
        if watched not in fields:
            errors.append(f"{aid}: watched field {watched} is not in the field catalog")
        for f in flds:
            if f"{table}.{f['field']}" not in fields:
                errors.append(f"{aid}: {table}.{f['field']} is not in the field catalog")
            if f["choices"] and "|" not in f["choices"] and f["choices"] not in tables:
                errors.append(f"{aid}: choices table {f['choices']} does not exist")
    if errors:
        print("REFUSING TO SEED:", file=sys.stderr)
        for e in errors:
            print("  - " + e, file=sys.stderr)
        sys.exit(1)

    principals, schemas, profiles = [], [], []
    for n, (role, (kind, accent, icon, device, home, title, pitch)) in enumerate(PROFILES.items(), 1):
        pg = "pko_" + role.replace("-", "_")
        principals.append({"AccessPrincipalId": f"principal-{role}", "Label": roles[role]["Label"],
                           "DomainRole": role, "PgRoleName": pg, "SchemaName": pg,
                           "IsAdministrator": False, "SemanticTypeIri": IRI + "AccessPrincipal"})
        schemas.append({"RoleSchemaId": f"schema-{role}", "Principal": f"principal-{role}",
                        "SchemaName": pg, "IsSealed": True, "SemanticTypeIri": IRI + "RoleSchema"})
        profiles.append({"AppRoleProfileId": f"profile-{role}", "Role": role,
                         "DisplayLabel": roles[role]["Label"], "RoleKind": kind, "AccentColor": accent,
                         "IconMark": icon, "Pitch": pitch, "SortOrder": 20 + n, "Device": device,
                         "HomeRoute": home, "HomeTitle": title, "SemanticTypeIri": IRI + "AppRoleProfile"})

    users, assigns = [], []
    for agent, role in PEOPLE:
        a = agents[agent]
        users.append({"AppUserId": f"user-{agent}", "EmailAddress": a.get("ContactAddress") or f"{agent}@agents.internal",
                      "DisplayName": a.get("Name") or agent.replace("-", " ").title(), "LinkedAgent": agent,
                      "IsEnabled": True, "SemanticTypeIri": IRI + "AppUser"})
        assigns.append({"PrincipalAssignmentId": f"pa-{agent}-{role}", "AppUser": f"user-{agent}",
                        "Principal": f"principal-{role}", "IsDefault": True,
                        "GrantedRationale": f"{agent} works as {role} in the domain model, so may act as its principal.",
                        "SemanticTypeIri": IRI + "PrincipalAssignment"})

    actions, action_fields = [], []
    for aid, role, route, label, table, op, watched, ep, desc, flds in ACTIONS:
        short = role.replace("-", "")
        actions.append({"AppActionId": aid, "Label": label, "OwningRole": role, "RoutePath": route,
                        "TargetTable": table, "Operation": op,
                        "Policy": f"pol-{short}-{snake(table)}-{op.lower()}",
                        "WatchedField": watched, "StoryEpisode": ep, "Description": desc,
                        "SemanticTypeIri": IRI + "AppAction"})
        for i, f in enumerate(flds, 1):
            action_fields.append({"AppActionFieldId": f"{aid}.{f['field']}", "AppAction": aid,
                                  "TargetField": f"{table}.{f['field']}", "FieldLabel": f["label"],
                                  "InputKind": f["kind"], "FixedValue": f["fixed"], "ChoicesFrom": f["choices"],
                                  "SortOrder": i, "SemanticTypeIri": IRI + "AppActionField"})

    rb = json.load(open(RB))  # re-read: contended file
    fill_physical_names(rb)
    upsert(rb["AccessPrincipals"]["data"], "AccessPrincipalId", principals)
    upsert(rb["RoleSchemas"]["data"], "RoleSchemaId", schemas)
    upsert(rb["AppRoleProfiles"]["data"], "AppRoleProfileId", profiles)
    for p in rb["AppRoleProfiles"]["data"]:       # the twelve older profiles: a generated home, on a tablet
        if not p.get("HomeRoute"):
            p["Device"], p["HomeRoute"], p["HomeTitle"] = "tablet", f"/{p['Role']}", p["DisplayLabel"]
    upsert(rb["AppUsers"]["data"], "AppUserId", users)
    upsert(rb["PrincipalAssignments"]["data"], "PrincipalAssignmentId", assigns)
    rb["AppActions"]["data"] = actions                # owned entirely by this script
    rb["AppActionFields"]["data"] = action_fields
    tmp = RB + ".tmp"
    json.dump(rb, open(tmp, "w"), indent=1, ensure_ascii=False)
    os.replace(tmp, RB)
    print(f"principals +{len(principals)}  users +{len(users)}  profiles +{len(profiles)}  "
          f"actions {len(actions)}  action fields {len(action_fields)}")

    subprocess.run([sys.executable, os.path.join(HERE, "reseed_role_profiles.py")], check=True)


if __name__ == "__main__":
    main()
