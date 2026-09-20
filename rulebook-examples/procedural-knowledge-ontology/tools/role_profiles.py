#!/usr/bin/env python3
"""Job-minimum access profiles: what each role needs, and nothing else.

One entry per principal. Each table lists ONLY the fields that role needs to
do the job named in Roles.Responsibility. The test for including a field is
"would this role's work stall without it", not "might they find it mildly
interesting".

Where a role is NAMED on a table in the seed data (Steps.AssignedRole,
Exceptions.ApprovalRole, ...) that is direct evidence they work with it, and
those tables are always included. The reverse also holds: close-automation
appears nowhere except Steps, and its profile reflects that.

Deliberately excluded from every non-steward profile: the witness/diagnostic
columns (unwarranted_boundary_count, stale_binding_count,
undeclared_control_version_key, has_been_approached_by_software, ...). Those
exist so the process-steward can audit model health. A CFO approving a close
has no use for them, and their presence is what made all twelve roles look
identical.

`ALL` means every field on that table -- used only for small tables where the
whole row is the point (Exceptions has 11 fields and a CFO approving one wants
all of them).
"""

ALL = "*"

# Common shapes, so profiles stay readable and the intent is visible.
STEP_MINIMAL = ["StepId", "StepNumber", "Title", "StepKind", "Instruction",
                "AssignedRole", "AssignedRoleLabel", "ProcedureVersion", "Name"]
STEP_WITH_GATE = STEP_MINIMAL + ["IsApprovalStep", "RequiresHumanConfirmation",
                                 "ExpectedDurationMinutes"]
EXEC_MINIMAL = ["StepExecutionId", "Step", "ExecutionStatus", "StartedAt",
                "EndedAt", "VerificationResult", "Name"]
PROC_MINIMAL = ["ProcedureId", "Title", "Purpose", "Name"]
PV_MINIMAL = ["ProcedureVersionId", "VersionNumber", "Title", "Status",
              "IsCurrent", "Procedure", "Name"]

PROFILES = {

    # ---- Finance close ----------------------------------------------------
    "finance-analyst": {
        "why": "Preparer of reconciliations and variance evidence. Needs the "
               "steps assigned to them, what they recorded, and the "
               "requirements they must satisfy. Not governance, not comms.",
        "tables": {
            "Steps": STEP_WITH_GATE,
            "StepExecutions": EXEC_MINIMAL + ["Deviation", "ActualDurationMinutes",
                                              "IsLate"],
            "ProcedureExecutions": ["ProcedureExecutionId", "ExecutionStatus",
                                    "StartedAt", "EndedAt", "ProcedureVersion",
                                    "Name"],
            "Requirements": ["RequirementId", "Label", "RequirementType",
                             "Statement", "IsBlocking", "Name"],
            "RequirementSatisfactions": ["RequirementSatisfactionId", "Requirement",
                                         "SatisfactionLevel", "Evidence",
                                         "EvaluatedAt", "Name"],
            "Procedures": PROC_MINIMAL,
            "ProcedureVersions": PV_MINIMAL,
        },
    },

    "controller": {
        "why": "Owner of close controls and first approval authority. Needs "
               "the approval surface -- exceptions they approve, change "
               "requests they decide, the knowledge they own -- plus enough "
               "execution state to approve against.",
        "tables": {
            "Steps": STEP_WITH_GATE + ["ControlKind"],
            "StepExecutions": EXEC_MINIMAL + ["Deviation"],
            "ProcedureExecutions": ["ProcedureExecutionId", "ExecutionStatus",
                                    "StartedAt", "EndedAt", "ProcedureVersion",
                                    "Name"],
            "Exceptions": ALL,
            "ChangeRequests": ["ChangeRequestId", "Title", "ChangeKind", "Status",
                               "RequestedAt", "DecidedAt", "ImpactAssessment",
                               "AuthorityRole", "IsOpen", "Name"],
            "KnowledgeFragments": ["KnowledgeFragmentId", "Statement",
                                   "KnowledgeForm", "Status", "Confidence",
                                   "OwnerRole", "LastReviewedAt", "Name"],
            "Requirements": ["RequirementId", "Label", "Statement", "IsBlocking",
                             "AccountableRole", "Name"],
            "ProcedureVersions": PV_MINIMAL,
        },
    },

    "cfo": {
        "why": "Final authority for the financial close. Sees the decision "
               "surface only: what awaits approval, the boundaries on that "
               "authority, and the close's headline state. No preparation "
               "detail, no stewardship diagnostics.",
        "tables": {
            "ProcedureExecutions": ["ProcedureExecutionId", "ExecutionStatus",
                                    "StartedAt", "EndedAt", "ProcedureVersion",
                                    "Name"],
            "Exceptions": ALL,
            "ChangeRequests": ["ChangeRequestId", "Title", "ChangeKind", "Status",
                               "RequestedAt", "DecidedAt", "ImpactAssessment",
                               "IsOpen", "Name"],
            "AuthorityBoundaries": ["AuthorityBoundaryId", "ForbiddenAgentKind",
                                    "ForbiddenDecisionKind", "Status",
                                    "AuthorityRole", "Step", "Name"],
            "Procedures": PROC_MINIMAL,
        },
    },

    # ---- Automation / AI --------------------------------------------------
    "close-automation": {
        "why": "Deterministic extraction, posting and archival operator. A "
               "pipeline. It reads the steps it is assigned and writes what it "
               "did. It has no view of people, policy, or governance at all.",
        "tables": {
            "Steps": STEP_MINIMAL + ["ExpectedDurationMinutes"],
            "StepExecutions": EXEC_MINIMAL,
            "Errors": ALL,
        },
    },

    "variance-review-agent": {
        "why": "AI assistant that classifies and prioritises variances. Reads "
               "execution outcomes and the requirements that define a "
               "variance. No identities, no approvals.",
        "tables": {
            "Steps": STEP_MINIMAL,
            "StepExecutions": EXEC_MINIMAL + ["Deviation", "IsLate"],
            "Requirements": ["RequirementId", "Label", "RequirementType",
                             "Statement", "IsBlocking", "Name"],
            "RequirementSatisfactions": ["RequirementSatisfactionId", "Requirement",
                                         "SatisfactionLevel", "Evidence", "Name"],
            "Errors": ALL,
        },
    },

    "policy-drafting-agent": {
        "why": "AI assistant that drafts policy and channel variants. Reads "
               "the templates and policies it drafts against. It never sees "
               "recipients -- drafting does not require knowing who is "
               "addressed, and consent data is not its business.",
        "tables": {
            "MessageTemplates": ["MessageTemplateId", "SubjectTemplate",
                                 "BodyTemplate", "Locale", "Status", "Name"],
            "CommunicationPolicies": ["CommunicationPolicyId", "Channel",
                                      "AudienceRule", "MaxMessageLength",
                                      "MaxSegments", "RequiredContent",
                                      "AuthorityStatement", "Status", "Name"],
            "KnowledgeFragments": ["KnowledgeFragmentId", "Statement",
                                   "KnowledgeForm", "Status", "Name"],
            "Steps": STEP_MINIMAL,
        },
    },

    # ---- People / policy --------------------------------------------------
    "hr-policy-owner": {
        "why": "Accountable owner for employment policy content. Owns policy "
               "knowledge and the change requests that alter it. Sees the "
               "steps they are assigned, and the exceptions they approve.",
        "tables": {
            "Steps": STEP_WITH_GATE,
            "KnowledgeFragments": ["KnowledgeFragmentId", "Statement",
                                   "KnowledgeForm", "Status", "Confidence",
                                   "OwnerRole", "ValidFrom", "ValidTo",
                                   "LastReviewedAt", "Name"],
            "KnowledgeGaps": ["KnowledgeGapId", "Statement", "Severity",
                              "Status", "OwnerRole", "ResolutionPlan",
                              "IsOpen", "Name"],
            "ChangeRequests": ["ChangeRequestId", "Title", "ChangeKind", "Status",
                               "RequestedAt", "DecidedAt", "ImpactAssessment",
                               "AuthorityRole", "IsOpen", "Name"],
            "Exceptions": ALL,
            "MessageTemplates": ["MessageTemplateId", "SubjectTemplate",
                                 "BodyTemplate", "Status", "Name"],
        },
    },

    "employment-counsel": {
        "why": "Legal reviewer for workforce policy changes. Reads what is "
               "proposed and the boundaries that constrain it. Does not need "
               "execution telemetry or delivery data.",
        "tables": {
            "ChangeRequests": ["ChangeRequestId", "Title", "ChangeKind", "Status",
                               "RequestedAt", "DecidedAt", "ImpactAssessment",
                               "IsOpen", "Name"],
            "KnowledgeFragments": ["KnowledgeFragmentId", "Statement",
                                   "KnowledgeForm", "Status", "OwnerRole",
                                   "ValidFrom", "ValidTo", "Name"],
            "AuthorityBoundaries": ["AuthorityBoundaryId", "ForbiddenAgentKind",
                                    "ForbiddenDecisionKind", "Status",
                                    "AuthorityRole", "Name"],
            "CommunicationPolicies": ["CommunicationPolicyId", "Channel",
                                      "ConsentRequired", "RequiredContent",
                                      "AuthorityStatement", "RetentionDays",
                                      "Status", "Name"],
            "Steps": STEP_MINIMAL,
        },
    },

    # ---- Communications ---------------------------------------------------
    "communications-manager": {
        "why": "Human owner of employee communications. Approves what goes "
               "out and to whom, so this is the one non-admin role that sees "
               "recipients -- including consent state, which is the whole "
               "point of the approval.",
        "tables": {
            "CommunicationPolicies": ALL,
            "MessageTemplates": ["MessageTemplateId", "SubjectTemplate",
                                 "BodyTemplate", "Locale", "Status",
                                 "LastValidApproval", "Name"],
            "SendIntents": ["SendIntentId", "ProposedBody",
                            "ProposedSendAtLocalHour", "EvaluatedAt",
                            "MessageTemplate", "Recipient", "Name"],
            "MessageDeliveries": ["MessageDeliveryId", "RenderedBody", "SentAt",
                                  "DeliveryStatus", "SuppressionReason",
                                  "AcknowledgedAt", "Recipient", "Name"],
            "Recipients": ["RecipientId", "DisplayName", "EmailAddress",
                           "SmsConsentStatus", "SmsConsentAt", "Name"],
            "KnowledgeGaps": ["KnowledgeGapId", "Statement", "Severity",
                              "Status", "OwnerRole", "IsOpen", "Name"],
        },
    },

    "notification-publisher": {
        "why": "Pipeline that applies channel rules and sends approved "
               "messages. Needs the rules and the send queue. Sees a "
               "recipient's ADDRESS but not their consent history or display "
               "name -- it delivers, it does not decide.",
        "tables": {
            "CommunicationPolicies": ["CommunicationPolicyId", "Channel",
                                      "QuietHoursStartHour", "QuietHoursEndHour",
                                      "MaxMessageLength", "MaxSegments",
                                      "ConsentRequired", "Status", "Name"],
            "SendIntents": ["SendIntentId", "ProposedBody",
                            "ProposedSendAtLocalHour", "Recipient",
                            "MessageTemplate", "Name"],
            "MessageDeliveries": ["MessageDeliveryId", "RenderedBody", "SentAt",
                                  "DeliveryStatus", "SuppressionReason",
                                  "Recipient", "Name"],
            "Recipients": ["RecipientId", "EmailAddress", "MobileNumber",
                           "SmsConsentStatus", "Name"],
            "Steps": STEP_MINIMAL,
        },
    },

    # ---- Administrators ---------------------------------------------------
    # Full read is the job, not a shortcut. The diagnostics that were noise for
    # everyone else are the working surface here.
    "process-steward": {
        "why": "Maintains procedural knowledge health and review cadence. The "
               "stewardship diagnostics excluded everywhere else ARE this "
               "role's instrument panel, so it reads everything.",
        "admin": True,
    },

    "knowledge-authority": {
        "why": "Approves semantic changes that alter commitments or controls. "
               "Must be able to see anything a change could touch.",
        "admin": True,
    },
}


# ============================================================================
# Loop 15 -- the roles that WORK in the app (ROLE-EXPERIENCES.md).
#
# Three keys the office profiles above never needed:
#
#   rows      {table: (predicate, why)}  the vertical cut for THIS role on that
#             table, replacing the shared defaults in reseed_role_profiles.py.
#   unscoped  True = this role reads across ACME (the knowledge engineer, the
#             ontology authority); the shared tenancy defaults do not apply.
#   writes    [(table, command, using, check, why)]  what the role may CHANGE.
#             Each becomes an INSERT or UPDATE policy; the columns it may write
#             come from AppActionFields via FieldGrants.CanWrite.
#
# These roles get every column of the tables they work with (ALL). The narrow
# column lists above were earned by an audit of what each office role's work
# stalls without; the same audit for these roles is still to be done, and
# IsOverPrivileged will say so until it is.
# ============================================================================
def _org(table_snake, pk):
    """Rows whose owning procedure belongs to the caller's organization. The
    predicate calls a derived lookup several hops down the DAG; the policy stays
    one line."""
    return (f"public.calc_{table_snake}_owner_organization({pk}) = app.jwt_organization()",
            "Only rows belonging to a procedure this sign-in's organization owns. "
            "OwnerOrganization is derived hop by hop from Procedures; the policy reads one function.")


PLANT_ROWS = {
    "Procedures": ("owner_organization = app.jwt_organization()",
                   "Only procedures this sign-in's organization owns."),
    "ProcedureVersions": _org("procedure_versions", "procedure_version_id"),
    "Steps": _org("steps", "step_id"),
    "ProcedureExecutions": _org("procedure_executions", "procedure_execution_id"),
    "StepExecutions": _org("step_executions", "step_execution_id"),
    "CueObservations": _org("cue_observations", "cue_observation_id"),
    "AssistantAnswers": _org("assistant_answers", "assistant_answer_id"),
    "KnowledgeFragments": _org("knowledge_fragments", "knowledge_fragment_id"),
    "ChangeRequests": _org("change_requests", "change_request_id"),
    "KnowledgeGaps": _org("knowledge_gaps", "knowledge_gap_id"),
    "KnowHowCarriers": ("organization = app.jwt_organization()",
                        "Only know-how held in this sign-in's organization."),
}

_RUNNER = ["Procedures", "ProcedureVersions", "Steps", "StepTransitions", "StepCues",
           "StepConditions", "StepLockRequirements", "StepProtectiveEquipment",
           "LockDevices", "ProtectiveEquipment", "Machines", "MachineEnergySources",
           "EnergySources", "Facilities", "FailureModes", "DecisionPoints",
           "ProcedureExecutions", "StepExecutions", "CueObservations",
           "KnowledgeFragments", "ExpertCognitions", "ConceptLadderRungs",
           "KnowHowCarriers", "AssistantAnswers", "KnowledgeQueryDefinitions",
           "Agents", "Roles",
           # what was checked on a step, and whether it held: a technician reads their own run's checks (loop 18)
           "ConditionChecks",
           # the one current row carries the register-wide totals the floor pages read (loop 17)
           "EvaluationContexts"]

_OWN_AGENT = "executed_by_agent = app.jwt_agent()"
_DECIDE = ("NOT (requested_by_agent = app.jwt_agent() AND decided_at IS NOT NULL)")

EXPERIENCE_PROFILES = {
    "maintenance-technician": {
        "why": "Runs lockouts on the floor. Needs the procedure, its warning signs, the knowledge "
               "attached to each step, their own runs, and the assistant's answers. Plant only.",
        "tables": {t: ALL for t in _RUNNER},
        "rows": PLANT_ROWS,
        "writes": [
            ("ProcedureExecutions", "INSERT", "", _OWN_AGENT, "A technician may start a run in their own name only."),
            ("ProcedureExecutions", "UPDATE", _OWN_AGENT, _OWN_AGENT, "A technician may pause or finish their own run."),
            ("StepExecutions", "INSERT", "", _OWN_AGENT, "A technician records the steps they carried out themselves."),
            ("StepExecutions", "UPDATE", _OWN_AGENT, _OWN_AGENT, "A technician completes a step they began themselves."),
            ("ConditionChecks", "INSERT", "", "checked_by_agent = app.jwt_agent()", "A technician answers a check on their own step, in their own name (loop 20)."),
            ("CueObservations", "INSERT", "", "observed_by_agent = app.jwt_agent()",
             "A warning sign is recorded by the person who saw it."),
            ("CueObservations", "UPDATE", "observed_by_agent = app.jwt_agent()", "observed_by_agent = app.jwt_agent()",
             "Only the observer escalates their own observation."),
            ("AssistantAnswers", "INSERT", "", "asked_by_agent = app.jwt_agent()",
             "A question to the assistant is recorded against the person who asked it."),
        ],
    },
    "plant-assistant": {
        "why": "The Plant Copilot. Reads the procedure, its warning signs, transitions and attached "
               "knowledge, and nothing about people, governance or any other organization. It "
               "answers from these rows; it cannot leak what it cannot read.",
        "tables": {t: ALL for t in ["Procedures", "ProcedureVersions", "Steps", "StepTransitions",
                                    "StepCues", "StepConditions", "MachineEnergySources",
                                    "EnergySources", "Machines", "KnowledgeFragments",
                                    "FailureModes", "DecisionPoints", "KnowledgeQueryDefinitions",
                                    "ProcedureExecutions", "StepExecutions", "CueObservations"]},
        "rows": PLANT_ROWS,
    },
    "plant-safety-officer": {
        "why": "Owns lockout safety: receives escalations, validates what the assistant proposes, "
               "and is the authority on changes to the plant's procedures.",
        # Requirements / StepRequirements / RequirementSatisfactions: the officer cannot judge whether a
        # blocking control is real without seeing the control, the step it is bound to, and every
        # evaluation ever recorded against it.
        "tables": {t: ALL for t in _RUNNER + ["AiInsightProposals", "ChangeRequests", "KnowledgeGaps",
                                              "WorkflowViewDivergences", "StakeholderPerspectives",
                                              "Requirements", "StepRequirements",
                                              "RequirementSatisfactions", "ConditionChecks"]},
        "rows": PLANT_ROWS,
        "writes": [
            ("CueObservations", "UPDATE", "escalated_to_agent = app.jwt_agent()", "escalated_to_agent = app.jwt_agent()",
             "The person an observation was escalated to acknowledges it."),
            ("AiInsightProposals", "UPDATE", "", "validated_by_agent = app.jwt_agent()",
             "A person, never the proposing agent, validates an insight, in their own name."),
            ("ChangeRequests", "UPDATE", "authority_role = app.jwt_role()", _DECIDE,
             "The authority may hand a decision up or decide it, but never decide a request they raised themselves."),
            ("RequirementSatisfactions", "INSERT", "", "evaluated_by_agent = app.jwt_agent()",
             "The safety officer records whether a blocking control held, in their own name. The model "
             "then judges the record itself: EvaluatorIsStepExecutor is TRUE when the person scoring the "
             "control is the person who performed the step, and IsBareAssertion is TRUE when a control "
             "was cleared with no evidence and no computed witness."),
        ],
    },
    "plant-operations-manager": {
        "why": "Runs the floor over weeks and quarters: who is proficient, who mentors whom, what "
               "know-how is about to leave, and the decisions handed up to them.",
        "tables": {t: ALL for t in ["Procedures", "ProcedureVersions", "Steps", "ProcedureExecutions",
                                    "OnboardingRecords", "Mentorships", "CommunitiesOfPractice",
                                    "KnowHowCarriers", "KnowledgeTransfers", "ProcessKnowledgeLevels",
                                    "ProcessLevelStatements", "LevelCaptureStrategies", "ChangeRequests",
                                    "WorkflowViewDivergences", "KnowledgeWorkforcePositions",
                                    "Agents", "Roles"]},
        "rows": PLANT_ROWS,
        "writes": [
            ("ChangeRequests", "UPDATE", "authority_role = app.jwt_role()", _DECIDE,
             "The authority decides a request, but never one they raised themselves."),
        ],
    },
    "knowledge-engineer": {
        "why": "Collects, organizes and encodes procedural knowledge across ACME. The capture "
               "workbench reads every session, fragment, gap, vocabulary and know-how card.",
        "unscoped": True,
        "tables": {t: ALL for t in _RUNNER + [
            "ElicitationSessions", "ElicitationParticipants", "CriticalIncidents",
            "RepertoryGridConstructs", "WorkflowViewDivergences", "StakeholderPerspectives",
            "KnowledgeRepositoryEntries", "KnowledgeTransfers", "Mentorships",
            "KnowledgeBrokerLinks", "Vocabularies", "VocabularyTerms", "TermLabelVariants",
            "StakeholderLenses", "ProcedureLensViews", "KnowledgeTraces",
            "CollectedSourceMaterials", "KnowledgeSearchEvents", "KnowledgeMethods",
            "KnowledgeGaps", "ChangeRequests", "OnboardingRecords", "CommunitiesOfPractice",
            # a control, its binding, its evaluations and the checks that enforce it (loop 18)
            "Requirements", "StepRequirements", "RequirementSatisfactions", "ConditionChecks"]},
        "writes": [
            ("KnowledgeRepositoryEntries", "INSERT", "", "author_agent = app.jwt_agent()",
             "A repository entry is written in its author's own name."),
            ("KnowledgeTransfers", "INSERT", "", "true",
             "The knowledge engineer records a hand-over between two other people."),
            ("StepCues", "INSERT", "", "true",
             "Encoding a warning sign onto a step is the knowledge engineer's job."),
            ("ChangeRequests", "UPDATE", "decided_at IS NOT NULL", "decided_at IS NOT NULL",
             "The steward marks a DECIDED request implemented; it cannot touch an undecided one."),
            ("Requirements", "UPDATE", "is_blocking", "has_computed_witness",
             "The knowledge engineer names the column that computes a blocking control, and in doing "
             "so claims it is computed. Whether that column really exists is the model's judgement "
             "(Requirements.WitnessClaimIsUnverified), not the engineer's."),
        ],
    },
    "sourcing-manager": {
        "why": "Manages the contracts ACME depends on. Reads every organization's engagements and "
               "audits (sourcing is a corporate view) and changes only its own organization's.",
        "unscoped": True,
        "tables": {t: ALL for t in ["ProviderEngagements", "KnowledgeDeliverables", "KnowledgeAudits",
                                    "KnowledgeAuditItems", "SourcingFunctions", "Organizations",
                                    "KnowledgeWorkforcePositions", "Agents"]},
        "writes": [
            ("ProviderEngagements", "UPDATE", "client_organization = app.jwt_organization()",
             "client_organization = app.jwt_organization()",
             "A sourcing manager changes only their own organization's contracts."),
            ("KnowledgeDeliverables", "INSERT", "",
             "provider_engagement IN (SELECT provider_engagement_id FROM public.provider_engagements "
             "WHERE client_organization = app.jwt_organization())",
             "A deliverable may be required only on the caller's own organization's contract."),
        ],
    },
    "release-manager": {
        "why": "Owns the release gate: the deployment procedure, the AI agents that score releases, "
               "what an upgrade would touch, and who held which role when.",
        "tables": {t: ALL for t in ["Procedures", "ProcedureVersions", "Steps", "StepTransitions",
                                    "ProcedureExecutions", "StepExecutions", "AgentUpgradeAssessments",
                                    "AiRegistryModelVersions", "RoleAssignments", "Roles", "Agents",
                                    "TermMeaningChanges", "VocabularyTerms", "ArtifactHandoffs",
                                    "ExecutionEntities"]},
        "rows": {**{k: v for k, v in PLANT_ROWS.items()
                    if k in ("Procedures", "ProcedureVersions", "Steps", "ProcedureExecutions", "StepExecutions")},
                 # Without this the default policy is IsCurrent, and the page "Who held the role that
                 # day" could only ever answer "whoever holds it now" (found rehearsing episode 5).
                 "RoleAssignments": ("true", "Every assignment, open or closed. A question about a past date "
                                             "can only be answered from the rows that were closed, never deleted.")},
    },
    "ontology-authority": {
        "why": "Approves changes to the model itself and must see anything such a change could "
               "touch: requests, validation runs, integrity checks, releases and the questions the "
               "model must still answer.",
        "unscoped": True,
        "tables": {t: ALL for t in ["ModelChangeRequests", "ChangeValidationRuns", "ChangeIntegrityChecks",
                                    "ChangeImpactFindings", "CompetencyQuestionRuns",
                                    "CompetencyQuestionSetEntries", "CompetencyQuestionReviews",
                                    "RulebookReleases", "InstanceDataVersions",
                                    "ExternalDependencyRevisions", "ModelExpansionRequests",
                                    "TermMeaningChanges", "VocabularyTerms", "GovernedModels",
                                    "AiRegistryModelVersions", "RoleAssignments", "Roles", "Agents"]},
        "writes": [
            ("ModelChangeRequests", "UPDATE", "", "authority_reviewed_at IS NOT NULL",
             "The authority records that they reviewed a model change."),
        ],
    },
}
PROFILES.update(EXPERIENCE_PROFILES)

# The process steward is an administrator and owns the modelled instant.
PROFILES["process-steward"]["writes"] = [
    ("EvaluationContexts", "UPDATE", "", "true",
     "An administrator may move the instant every time-dependent answer is judged against."),
]
