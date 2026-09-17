"""Evidence for theme F (loop-11): sourcing, the knowledge audit, the AI system registry and ground-truth routing.

(kind, target, justification). Validity is computed by ClaimEvidence.IsValid from Postgres
measurements; nothing here asserts coverage by itself.
"""


def fld(target, why):
    return ("Field", target, why)


def tbl(target, why):
    return ("Table", target, why)


def q(target, why):
    return ("RoleQuestion", target, why)


EVIDENCE = {
    # ---------------------------------------------------------------- pkm-3: concepts
    "pkm3-c06": [fld("SourcingFunctions.IsBusinessProcessOutsourcing",
                     "TRUE for a function a provider performs whose work is routine and transactional; it fires on accounts payable invoice "
                     "processing sent to Harborline and on nothing expertise-heavy.")],
    "pkm3-c07": [fld("SourcingFunctions.IsKnowledgeProcessOutsourcing",
                     "TRUE for a function a provider performs whose work is expertise-heavy: appliance process engineering, safety regulatory "
                     "research, acquisition due-diligence analysis, press overhaul and classifier training.")],
    "pkm3-c08": [fld("SourcingFunctions.SourcingClass",
                     "Every function carries its classification as RetainedCore or SentOutExecution, separately from who actually executes it.")],
    "pkm3-c09": [fld("SourcingFunctions.ClaimsHowWithoutDoing",
                     "The model records who holds the how of a function separately from who executes it, and flags a declaration that the "
                     "client holds the method while a provider does the work: acme-engineering says it holds classifier training know-how "
                     "while Northstar trains the model, and its own audit rates the in-house level 1 of the 3 needed.")],
    "pkm3-c17": [fld("RecordsRetentionPolicies.RetentionDriver",
                     "Each retention rule states whether its period is set by compliance risk or by the value of the knowledge in the records.")],
    "pkm3-c18": [fld("CorporateGovernancePrograms.IsComplianceOnly",
                     "Each governance programme records which of compliance, data quality, information management and knowledge management "
                     "it covers; this field is TRUE for the programme whose scope is compliance alone.")],
    "pkm3-c23": [fld("SourcingFunctions.IsWhatHowSplit",
                     "Compares the party holding the specification of what is built with the party holding the method of how it is built, "
                     "and is TRUE where different parties hold them.")],
    "pkm3-c24": [fld("Organizations.IsHollowedOutFirm",
                     "TRUE for an organization every one of whose product-delivering functions depends on method knowledge a provider holds; "
                     "ACME Home Brands keeps its brand, customers and specifications while Meridian holds how its appliances are assembled.")],
    "pkm3-c25": [fld("ProviderEngagements.DocumentationOwnership",
                     "Records per engagement whether the documentation the provider produces stays the provider's intellectual property "
                     "(ProviderIP) or belongs to the client.")],
    "pkm3-c28": [fld("ProviderEngagements.KnowledgeDutyTerms",
                     "States the duty, if any, an engagement puts on the provider to build up the client's knowledge, such as Baxter training "
                     "two plant technicians per overhaul; blank where the engagement carries none.")],
    "pkm3-c29": [fld("ProviderEngagements.ProviderTreatsKnowHowAsDifferentiator",
                     "Records whether the provider positions its procedural know-how as its competitive differentiator, the incentive to "
                     "withhold it from the client.")],
    "pkm3-c34": [fld("KnowledgeWorkforcePositions.Discipline",
                     "Each dedicated position is typed KnowledgeEngineer, InformationArchitect or Ontologist, with the organization it serves "
                     "and whether it is filled.")],
    "pkm3-c35": [fld("SourcingFunctions.IsVitalExpertiseProcess",
                     "TRUE for a function that is both strategically vital and expertise-heavy.")],
    "pkm3-c36": [fld("KnowledgeAuditItems.IsKnowledgeDependency",
                     "TRUE for an audited knowledge area where the organization holds less than it needs and relies on a provider that holds "
                     "it at the needed level.")],
    "pkm3-c37": [fld("KnowledgeAuditItems.IsCoverageGap",
                     "TRUE for an audited knowledge area where the organization falls short and no provider available to it holds the "
                     "knowledge either, such as the isolation sequence for mixer 2's twin-drive conversion.")],
    "pkm3-c38": [fld("KnowledgeAuditItems.IsSingleTeamSilo",
                     "Knowledge held at the needed level is classified by how many internal teams hold it; TRUE where exactly one team does "
                     "(the standard-line isolation know-how), the silo single-team ownership creates, against areas already spread across "
                     "two or three teams.")],
    # ---------------------------------------------------------------- pkm-3: prescriptions
    "pkm3-p03": [fld("ProviderEngagements.IsUnplannedKnowledgeReturn",
                     "Fires when an audit shows the client relies on a provider's knowledge, the provider keeps the documentation as its own "
                     "property, and the engagement records no plan for how the knowledge comes back: Meridian, Quillstone and Northstar; it "
                     "does not fire on Baxter, whose documentation is joint and whose return plan is recorded.")],
    "pkm3-p11": [fld("KnowledgeAudits.TreatsDeficitAsCostProblem",
                     "Fires when an audit finds knowledge shortfalls but frames them as a cost and efficiency problem rather than a knowledge "
                     "deficit: the 2025 Home Brands supplier review found two dependencies and filed them as cost; the plant and engineering "
                     "audits acknowledge a deficit, and the 2024 corporate cost review found no shortfall.")],
    "pkm3-p12": [fld("SourcingFunctions.IsUnauditedFunction",
                     "Fires for an outsourced or strategically vital function that no audit has compared held against needed knowledge for: "
                     "invoice processing at Harborline and the heating element R&D; audited functions do not fire.")],
    "pkm3-p13": [fld("KnowledgeAuditItems.IsUnnamedFinding",
                     "Fires for an audit finding (gap or dependency) that no knowledge gap row names: the two Home Brands dependencies; the "
                     "plant and engineering findings are each named as a knowledge gap.")],
    "pkm3-p14": [fld("Organizations.HasKnowledgeFindingsWithoutKnowledgeStaff",
                     "Fires for an organization whose audits found shortfalls while it has no filled knowledge engineer, information architect "
                     "or ontologist position: ACME Home Brands, whose knowledge engineer position was never budgeted; the plant and engineering "
                     "have filled positions.")],
    "pkm3-p15": [fld("SourcingFunctions.IsUncapturedPriorityProcess",
                     "Fires for a vital, expertise-heavy function that is outsourced or has a coverage gap and has no deliberate capture "
                     "initiative: appliance assembly, safety research and classifier training; the press overhaul (shadowing), the lockout "
                     "coverage gap (critical incidents) and deal analysis (practitioner pairing) have one.")],
    "pkm3-p16": [fld("ProviderEngagements.IsKnowledgeAccessUnsecured",
                     "Fires for a running engagement the client depends on for knowledge whose contract gives the client no access to that "
                     "knowledge: Meridian and Northstar; Quillstone and Baxter depend on provider knowledge but grant access.")],
    "pkm3-p20": [fld("ProviderEngagements.LacksKnowledgeDeliverables",
                     "Fires for a running engagement that requires no knowledge management deliverable at all: the Meridian assembly contract; "
                     "an ended engagement does not fire.")],
    "pkm3-p21": [fld("ProviderEngagements.IsOneWayLearning",
                     "Fires for a running engagement with knowledge deliverables where knowledge has actually been delivered in only one "
                     "direction: Quillstone (digests to the client only) and Northstar (change history to the provider only); Harborline, "
                     "Baxter and Lattice deliver both ways.")],
    "pkm3-p22": [fld("ProviderEngagements.IsShortTermWithoutJointKnowledge",
                     "Fires for a running engagement contracted for under three years that builds no knowledge asset jointly, i.e. a provider "
                     "treated transactionally rather than as a long-term partner: Meridian, Quillstone and Northstar; Baxter (60 months, shared "
                     "failure-mode catalog) and Lattice (48 months, joint playbook) do not.")],
    # ---------------------------------------------------------------- pkm-3: competency questions
    "pkm3-q02": [q("aq-pkm3-q02", "Asks, per audited knowledge area, what is needed against what is held, and whether a shortfall is a "
                                  "provider dependency or a coverage gap.")],
    "pkm3-q03": [q("aq-pkm3-q03", "Asks which functions are strategically vital and expertise-heavy and which of those a provider performs.")],
    "pkm3-q07": [q("aq-pkm3-q07", "Asks, per function, whether the method knowledge of how it is really performed is held by the organization "
                                  "or by an outside provider.")],
    "pkm3-q12": [q("aq-pkm3-q12", "Asks, per provider engagement, whether it requires knowledge deliverables to the client and plans how the "
                                  "knowledge comes back.")],
    # ---------------------------------------------------------------- pkm-3: illustrations
    "pkm3-i01": [fld("SourcingFunctions.IsVitalExpertiseClassedNonCore",
                     "Fires for strategically vital, expertise-heavy work classified as execution to send out rather than retained "
                     "core: safety regulatory research (legal research), acquisition due-diligence analysis (financial "
                     "analysis), appliance process engineering, the press overhaul and heating element R&D, the work that builds deep process "
                     "knowledge; routine invoice processing and retained functions do not fire.")],
    "pkm3-i02": [fld("SourcingFunctions.DesignsWhatItCannotBuild",
                     "Fires where a firm keeps complete specification knowledge of a product it delivers, a provider performs the production "
                     "and keeps the documentation as its property, and the firm's own method knowledge falls short: Home Brands designs the "
                     "countertop range and cannot assemble it.")],
    "pkm3-i03": [fld("RecordsRetentionPolicies.IsLegalLedKnowledgeDestruction",
                     "Fires where a programme sponsored by Legal set a compliance-risk retention period shorter than the useful life of the "
                     "knowledge in the records: the 2016 rule keeping design history three years for a fifteen-year product; the earlier "
                     "engineering-sponsored rule kept it twenty-five.")],
    # ---------------------------------------------------------------- ont-4: registry concepts
    "ont4-c40": [fld("Agents.AgentKind",
                     "Every agent is typed Human, AIAgent, AutomatedPipeline or Organization, and AI agents carry the prov:SoftwareAgent type "
                     "while persons do not.")],
    "ont4-c41": [tbl("AiRegistryModelVersions",
                     "Rows are model versions delivered by the acme-model-registry feed, each with its Dublin Core identifier, title, creator, "
                     "date, version and description.")],
    "ont4-c42": [tbl("AiModelDeployments",
                     "Rows are registry deployment records: which model version was deployed to which environment, when, and when retired.")],
    "ont4-c43": [tbl("AiModelEvaluations",
                     "Rows are registry evaluation results for model versions: suite, metric, score, threshold and whether it passed.")],
    "ont4-c44": [fld("ExecutionEntities.AttributedToAgent",
                     "For every generated entity, the agent that executed the step that generated it (prov:wasAttributedTo), e.g. risk reports "
                     "attributed to risk-classifier-2-4-0 and 2-4-1.")],
    "ont4-c46": [fld("StepVariables.SourceStep",
                     "On each input variable, the step whose output feeds it; the row's own step is the downstream step taking that artifact "
                     "as an input.")],
    "ont4-c47": [fld("AiAgentAccountabilities.AccountableAgent",
                     "Links an AI agent to the agent accountable for it over a validity period; risk-classifier-2-4-1 is accountable to Omar "
                     "Haddad.")],
    "ont4-c48": [fld("StepVariables.AiBlastRadiusPath",
                     "For every input fed by an AI agent's artifact the value names the agent, the artifact, the downstream step consuming it, "
                     "that step's role, the agent holding it and the workflow containing it, e.g. risk-classifier-2-4-1 produces Change risk "
                     "classification for deploy-05; role site-reliability-engineer; agent (none); workflow deploy-v3.2.0.")],
    "ont4-c49": [tbl("RoleAssignments",
                     "A role is filled by an agent only through an assignment row with a validity period; steps name the role, never the agent.")],
    "ont4-c50": [fld("AssignmentUpdatePolicies.UpdateSlaHours",
                     "Each assignment-update policy for roles operational systems route from states the hours within which an update must be "
                     "completed (24 for release approvers, 72 for AI agent fills, 0 where none is defined).")],
    # ---------------------------------------------------------------- ont-4: prescriptions
    "ont4-p70": [fld("AiRegistryModelVersions.IsLiveButUnregisteredInGraph",
                     "Fires for a model version running in production that no agent individual in the graph represents: release-notes-writer "
                     "1.1.0; the risk classifier versions and health checker are individuals, and the retired 2.3.0 is not live.")],
    "ont4-p71": [fld("AgentUpgradeAssessments.MissedTraversedImpact",
                     "Fires when an upgrade assessment lists fewer affected steps than the traversal of the artifact-flow closure finds: the "
                     "hand-written change ticket for 2.4.1 -> 2.5.0 listed only the release gate while traversal reaches deploy-03, deploy-04 "
                     "and deploy-05; the traversal-based assessment does not fire.")],
    "ont4-p72": [fld("Agents.IsAiAgentNotFilledFromRegistry",
                     "Fires for an AI agent individual whose identifier and version are not supplied by a registry record through dct:identifier "
                     "and dct:hasVersion: the 2.5.0 candidate typed in as 2.5.0 while the registry says 2.5.0-rc3, and two AI agents entered by "
                     "hand; the 2.4.0, 2.4.1 and health-check individuals match the feed.")],
    "ont4-p73": [fld("RoleAssignmentUpdateTasks.LacksNamedTriggerOwner",
                     "Fires when the governing policy names no role responsible for triggering an assignment update, or names one nobody holds: "
                     "every update under the engineering operations policy, including the SRE departure nobody triggered; updates under the "
                     "release approver and AI agent policies (owner: VP of engineering) do not fire.")],
    "ont4-p74": [fld("RoleAssignmentUpdateTasks.ExceededUpdateSla",
                     "Fires when no service level is defined or the update took longer than it allows: the hotfix release manager's departure "
                     "was recorded 307 hours after he left against a 24-hour service level; the 2.4.1 classifier swap finished in 39 of 72 "
                     "hours and does not fire.")],
    "ont4-p75": [fld("RoleAssignmentUpdateTasks.ChangedRoleInsteadOfAssignment",
                     "Fires when a change of who does the work replaced an assignment of one role with an assignment of a different role, so "
                     "the steps had to be re-pointed: build and rollout moved from Hector Ruiz's Build and Release Engineer role to a new "
                     "Deployment Automation role; the classifier swap and the AI-to-human SRE handover changed only the assignment.")],
    "ont4-p76": [fld("AssignmentRoutedNotices.RoutedAroundModel",
                     "Fires for a notice routed from a separately maintained list that reached someone the model's assignments did not name for "
                     "the step's role when it was sent: the ops spreadsheet sent the 2026-03-01 health verification notice to the VP while Leo "
                     "Marchetti held the SRE role; notices read from assignments do not fire.")],
    "ont4-p77": [fld("RoleAssignmentUpdateTasks.MissedNextDependentRun",
                     "Fires when a run needing the role started after the person left and before the assignment change was recorded: the "
                     "2026-07-14 hotfix ran after Dmitri Volkov (release manager), Leo Marchetti (SRE) and Kwame Asante (rollout) left and "
                     "before any of those updates; the classifier swap and the SRE handover were recorded before their next runs.")],
    # ---------------------------------------------------------------- ont-4: competency questions
    "ont4-q04": [q("aq-ont4-q04", "Asks which steps are affected, transitively along the artifact flow, if the agent moves to a newer model "
                                  "version.")],
    "ont4-q06": [q("aq-ont4-q06", "Asks which current assignment covers a role that approval steps are assigned to, i.e. who approval "
                                  "notices go to now.")],
    "ont4-q21": [q("aq-ont4-q21", "Asks which generated artifacts are attributed to each agent.")],
    "ont4-q22": [q("aq-ont4-q22", "Asks which steps take as an input an artifact produced by an AI agent's step, naming the agent on each "
                                  "input.")],
    "ont4-q23": [q("aq-ont4-q23", "Asks which role and agent are responsible for each step consuming an AI agent's artifact, and whether "
                                  "any such role is held by nobody.")],
    "ont4-q24": [q("aq-ont4-q24", "Asks which workflows contain steps consuming an AI agent's artifacts.")],
    "ont4-q27": [q("aq-ont4-q27", "Asks which human person is currently accountable for each AI agent and flags AI agents with none.")],
    # ---------------------------------------------------------------- ont-4: illustrations
    "ont4-i05": [fld("RoleAssignmentUpdateTasks.StaleAssignmentBrokeRouting",
                     "Separates a stale assignment that only made the model inaccurate from one that broke operations: it fires when the "
                     "assignment was still stale at a run that needed it and that run's notice for the role reached the wrong person or nobody "
                     "(the release manager and SRE departures before the 2026-07-14 hotfix), and not for the rollout operator's departure, "
                     "equally stale at that run but routed nothing.")],
    "ont4-i23": [fld("AiModelDeployments.IsIsolatedRegistryFact",
                     "Fires for a live production deployment whose agent fills no role and has no attributed artifact in the graph: the "
                     "release-notes writer is a registry row and nothing more, while the live 2.4.1 classifier deployment is connected to its "
                     "role assignment and risk reports.")],
    "ont4-i24": [fld("AssignmentRoutedNotices.ReachedWrongPersonOrNobody",
                     "Fires when a notice went to someone who did not hold the step's role when it was sent, or to nobody: the hotfix approval "
                     "notice went to Dmitri Volkov eleven days after he left, the hotfix health verification notice went to nobody, and the "
                     "spreadsheet sent one to the VP; notices to Grace Holloway and to Leo Marchetti while he held the role do not fire.")],
}
