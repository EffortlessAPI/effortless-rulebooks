# PKO 2.0.0 Alignment

## Version contract

| Artifact | Version |
|---|---|
| PKO core | `https://w3id.org/pko/2.0.0` |
| PKO industry | `https://w3id.org/pko/industry/2.0.0` |
| ERB-PKO profile | `1.0.0` |
| Canonical rulebook | `1.0.0` |

The canonical file keeps the required Effortless `$schema` value and records the PKO profile/version in `RulebookReleases`, `OntologyProfiles`, and `SemanticMappings`.

## Exact or directly reused terms

| Rulebook structure | Semantic term |
|---|---|
| `Procedures` | `pko:Procedure` |
| `ProcedureVersions` | versioned `pko:Procedure` / `dcat:Resource` |
| `ProcedureVersions.VersionNumber` | `pko:versionNumber` |
| `ProcedureVersions.NewVersionMotivation` | `pko:newVersionMotivation` |
| `ProcedureVersions.ChangelogDescription` | `pko:changelogDescription` |
| `ProcedureStatusChanges` | `pko:ChangeOfStatus` |
| `ProcedureStatusChanges.FromStatus` | `pko:fromStatus` |
| `ProcedureStatusChanges.ToStatus` | `pko:toStatus` |
| atomic `Steps` | `pplan:Step` |
| composite `Steps` | `pplan:MultiStep` |
| `StepTransitions` | `pko:Transition` |
| `StepTransitions.FromStep` | `pko:fromStep` |
| `StepTransitions.ToStep` | `pko:toStep` |
| default transition | `pko:nextStep` |
| alternative transition | `pko:nextAlternativeStep` |
| fallback behavior | `pko:hasFallbackStep` / `pko:addressesError` |
| `Actions` | `pko:Action` |
| `Functions` | `pko:Function` |
| `Tools` | `m4ing:Tool` |
| `StepActions` | `pko:requiresAction` |
| `StepFunctions` | `pko:requiresFunction` |
| `StepTools` | `pko:requiresTool` |
| `Requirements` | `pko:Requirement` |
| `StepRequirements` | `pko:hasRequirement` |
| `StepVerifications` | `pko:StepVerification` |
| signal-verification rows | `pko:SignalVerification` |
| `ProcedureExecutions` | `pko:ProcedureExecution` |
| `StepExecutions` | `pko:StepExecution` |
| execution -> specification | `pko:hasExecutedProcedure` / `pko:hasExecutedStep` |
| executing agent | `pko:wasExecutedBy` |
| human confirmation | `pko:wasConfirmedBy` |
| `RequirementSatisfactions` | `pko:RequirementSatisfaction` |
| `Errors` | `pko:Error` |
| `IssueOccurrences` | `pko:IssueOccurrence` |
| `UserQuestions` | `pko:UserQuestionOccurrence` |
| `UserFeedback` | `pko:UserFeedbackOccurrence` |
| `FAQs` | `pko:FrequentlyAskedQuestion` |
| `Explanations` | `pko:Explanation` |
| `Resources` | `dcat:Resource` |
| extracted source | `pko:wasExtractedFrom` |
| referenced resource | `dcterms:references` |
| `Agents` | `prov:Agent` |
| software agents | `prov:SoftwareAgent` |
| `Organizations` | `prov:Organization` |
| `Roles` | `pro:Role` |
| `RoleAssignments` | `pro:RoleInTime` |
| durations | `time:Duration` |
| `CommunicationPolicies` | `odrl:Policy` |

## Explicit ERB-PKO extension

The following concepts are not represented as native PKO 2.0.0 classes. They are declared as an extension so a later PKO revision, another ontology, or an organizational profile can replace or align them without changing the rest of the rulebook.

| Rulebook structure | Extension intent |
|---|---|
| `KnowledgeFragments` | Captured tacit, implicit, explicit, situated, and lesson-learned knowledge |
| `ElicitationSessions` | Interviews, shadowing, workshops, and document analysis |
| `KnowledgeGaps` | Known unknowns, severity, blocking status, ownership, and resolution |
| `Rationales` | First-class design and control rationale |
| `Exceptions` | Governed exception record surrounding PKO fallback/alternative flow |
| `StewardshipAssignments` | Distinct steward and semantic authority assignments |
| `ChangeRequests` | Governed proposed changes and impact assessments |
| `ReviewEvents` | Periodic semantic and competency-coverage reviews |
| `CommunitiesOfPractice` | Socio-technical community maintaining a practice |
| `Mentorships` | Time-bounded apprenticeship and knowledge transfer |
| `LearningActivities` | Retrospectives, drills, onboarding, and tabletop exercises |
| `OperationalBindings` | Live links to operational records and freshness contracts |
| channel constraints on `CommunicationPolicies` | Consent, quiet hours, retention, length, and authority rules |

Each extension row carries an explicit `SemanticTypeIri` under `urn:effortless:pko-extension#`, and `SemanticMappings.MappingRelation` is `extension`, not `exact`.

## Why the relational shape is not a semantic downgrade

PKO is graph-shaped; an Effortless Rulebook is an acyclic structural graph. Repeated and many-to-many semantics are promoted to first-class junction or event entities:

- step/action -> `StepActions`
- step/function -> `StepFunctions`
- step/tool -> `StepTools`
- step/requirement -> `StepRequirements`
- version/version -> `ProcedureVersionLinks`
- step/step -> `StepTransitions`
- agent/role/time -> `RoleAssignments`

This preserves the semantic relationship while keeping the canonical ERB model a DAG. OWL/RDF, SQL, code, prose, and other projections can recover their native surface forms from the same structure.
