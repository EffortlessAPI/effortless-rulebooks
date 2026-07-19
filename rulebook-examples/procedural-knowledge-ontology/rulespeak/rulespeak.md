# 📘 PKO-Native Procedural Knowledge Rulebook — RuleSpeak

_A canonical Effortless Rulebook profile aligned to the Procedural Knowledge Ontology (PKO) 2.0.0. It represents procedure specifications separately from executions and includes versioning, status changes, steps, transitions, actions, software functions, tools, requirements, verifications, resources, agents, roles in time, issues, errors, questions, feedback, FAQs, explanations, governance, tacit/implicit/explicit knowledge capture, operational data bindings, learning, and communication policy projections. PKO-native terms are mapped exactly; enterprise knowledge-governance additions are explicitly identified as ERB-PKO extensions._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Rulebook Releas** | Version ledger for the canonical ERB-PKO rulebook itself. This is distinct from PKO Procedure versioning. | — |
| Name | Computed as the rulebook version, followed by “ / PKO ”, followed by the pko core version iri. | _Human-readable calculated display alias for the RulebookReleases row._ |
| **Ontology Profile** | Versioned ontology and vocabulary dependencies. PKO mappings always identify the exact profile and version. | — |
| Name | Computed as the label, followed by a space, followed by the version. | _Human-readable calculated display alias for the OntologyProfiles row._ |
| **Organization** | Organizations that own, adopt, govern, or execute procedures. Maps to prov:Organization. | — |
| Name | The same as its display name. | _Human-readable calculated display alias for the Organizations row._ |
| **Agent** | Human and software agents that create, modify, approve, or execute procedural knowledge. Maps to prov:Agent. | — |
| Name | The same as its display name. | _Human-readable calculated display alias for the Agents row._ |
| **Role** | Stable organizational functions separated from the agents that currently fill them. Maps to pro:Role. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the Roles row._ |
| Current Agent Kind | Taken from the linked current agent. | _Agent category of the current role filler._ |
| **Role Assignment** | Time-bounded records of agents holding roles. Maps to pro:RoleInTime and preserves assignment history instead of overwriting it. | — |
| Name | Computed as the role, followed by “ @ ”, followed by the valid from. | _Human-readable calculated display alias for the RoleAssignments row._ |
| Is Current | True when all of the following hold: the valid from is at most the current date and time and at least one of the following holds: the valid to is blank or the valid to is greater than the current date and time. | _TRUE when the assignment is valid now._ |
| **Community of Practice** | Socio-technical communities that transmit and maintain procedural knowledge. Explicit ERB-PKO extension. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the CommunitiesOfPractice row._ |
| **Mentorship** | Time-bounded apprenticeship relationships that intentionally transfer situated procedural knowledge. Explicit ERB-PKO extension. | — |
| Name | Computed as the mentor agent, followed by “ -> ”, followed by the learner agent. | _Human-readable calculated display alias for the Mentorships row._ |
| **Procedure Type** | Controlled values used by pko:hasProcedureType. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the ProcedureTypes row._ |
| **Procedure** | Abstract, discoverable procedures. Each version is represented separately in ProcedureVersions. Maps to pko:Procedure and dcat:Resource. | — |
| Name | The same as its title. | _Human-readable calculated display alias for the Procedures row._ |
| **Procedure Version** | Versioned procedure specifications. Maps to pko:Procedure plus DCAT version relations and PKO versionNumber/newVersionMotivation/changelogDescription. | — |
| Name | The same as its title. | _Human-readable calculated display alias for the ProcedureVersions row._ |
| Count of Steps | The number of steps related to the procedure version. | _Number of steps in this version._ |
| Count of Open Knowledge Gaps | The number of the procedure version's knowledge gaps that have a status of “Open”. | _Open knowledge gaps for this version._ |
| Is Ready for Execution | True when all of the following hold: the status is “Approved”; the count of steps is greater than 0; and the count of open knowledge gaps is 0. | _TRUE when approved, populated, and free of blocking knowledge gaps._ |
| **Procedure Version Link** | Directed links between versioned procedures. Maps to dcat:previousVersion/dcat:hasVersion and pko:nextVersion. | — |
| Name | Computed as the previous procedure version, followed by “ -> ”, followed by the next procedure version. | _Human-readable calculated display alias for the ProcedureVersionLinks row._ |
| **Procedure Status Change** | Lifecycle events that move a procedure version between PKO statuses. Maps to pko:ChangeOfStatus, fromStatus, toStatus, and prov:atTime. | — |
| Name | Computed as the procedure version, followed by “: ”, followed by the from status, followed by “ -> ”, followed by the to status. | _Human-readable calculated display alias for the ProcedureStatusChanges row._ |
| **Step** | Version-scoped units of work. Atomic steps map to pplan:Step; composite steps map to pplan:MultiStep. The specification is never conflated with execution. | — |
| Name | Computed as the step number, followed by “. ”, followed by the title. | _Human-readable calculated display alias for the Steps row._ |
| Assigned Role Label | Taken from the linked assigned role. | _Resolved role label._ |
| Assigned Agent Kind | The current agent kind of the step's assigned role. | _Current role-filler category._ |
| **Step Transition** | Directed control-flow edges represented as first-class pko:Transition instances with fromStep/toStep and next/alternative/fallback semantics. | — |
| Name | Computed as the from step, followed by “ -> ”, followed by the to step. | _Human-readable calculated display alias for the StepTransitions row._ |
| **Action** | Human actions required by steps. Maps to pko:Action and pko:requiresAction. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the Actions row._ |
| **Function** | Software or algorithmic functions required by steps. Maps to pko:Function and pko:requiresFunction. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the Functions row._ |
| **Tool** | Tools required to execute steps. Maps to m4ing:Tool and pko:requiresTool. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the Tools row._ |
| **Step Action** | Many-to-many Step/Action semantics normalized into a first-class ERB junction table. | — |
| Name | Computed as the step, followed by “ / ”, followed by the action. | _Human-readable calculated display alias for the StepActions row._ |
| **Step Function** | Many-to-many Step/Function semantics normalized into an ERB junction table. | — |
| Name | Computed as the step, followed by “ / ”, followed by the function. | _Human-readable calculated display alias for the StepFunctions row._ |
| **Step Tool** | Many-to-many Step/Tool semantics normalized into an ERB junction table. | — |
| Name | Computed as the step, followed by “ / ”, followed by the tool. | _Human-readable calculated display alias for the StepTools row._ |
| **Requirement** | Normative requirements applied to procedures, steps, or transitions. Maps to pko:Requirement and pko:hasRequirement. | — |
| Name | The same as its label. | _Human-readable calculated display alias for the Requirements row._ |
| **Step Requirement** | Many-to-many Step/Requirement semantics normalized into an ERB junction table. | — |
| Name | Computed as the step, followed by “ / ”, followed by the requirement. | _Human-readable calculated display alias for the StepRequirements row._ |
| **Step Verification** | Verification definitions attached to steps. Maps to pko:StepVerification and pko:SignalVerification. | — |
| Name | Computed as the step, followed by “ / ”, followed by the verification kind. | _Human-readable calculated display alias for the StepVerifications row._ |
| **Rationale** | First-class rationale statements explaining why procedural commitments and design decisions exist. Explicit ERB-PKO extension, represented as prov:Entity/dcat:Resource in projections. | — |
| Name | The same as its title. | _Human-readable calculated display alias for the Rationales row._ |
| **Exception** | Documented exceptions, fallbacks, and alternative handling. Aligns structurally with PKO fallback/alternative steps and requirements; the exception record itself is an ERB-PKO extension. | — |
| Name | The same as its condition. | _Human-readable calculated display alias for the Exceptions row._ |
| **Resource** | Documents, datasets, APIs, templates, images, manuals, and operational records referenced by procedures. Maps to dcat:Resource. | — |
| Name | The same as its title. | _Human-readable calculated display alias for the Resources row._ |
| **Procedure Resource** | Links versioned procedures to supporting resources using PKO/dcterms provenance relations. | — |
| Name | Computed as the procedure version, followed by “ / ”, followed by the resource. | _Human-readable calculated display alias for the ProcedureResources row._ |
| Relation Iri | Determined by priority: “https://w3id.org/pko#wasExtractedFrom” if the relation is “wasExtractedFrom”; in all other cases, “http://purl.org/dc/terms/references”. | _Exact semantic property IRI for the relation._ |
| **Elicitation Session** | Structured knowledge-elicitation events involving practitioners and knowledge engineers. Explicit ERB-PKO extension, modeled as prov:Activity. | — |
| Name | Computed as the method, followed by “ / ”, followed by the started at. | _Human-readable calculated display alias for the ElicitationSessions row._ |
| **Knowledge Fragment** | Explicit records of tacit, implicit, explicit, and situated procedural knowledge. This is an ERB-PKO extension represented as provenance-bearing dcat:Resource instances. | — |
| Name | Computed as the knowledge form, followed by “: ”, followed by the first 60 character(s) of the statement. | _Human-readable calculated display alias for the KnowledgeFragments row._ |
| Is Currently Valid | True when all of the following hold: the valid from is at most the current date and time; at least one of the following holds: the valid to is blank or the valid to is greater than the current date and time; and the status is “Approved”. | _TRUE when the fragment is approved and valid now._ |
| **Knowledge Gap** | Known unknowns and missing procedural coverage. Explicit ERB-PKO extension used to govern scope and prevent silent incompleteness. | — |
| Name | Computed as the severity, followed by “: ”, followed by the first 60 character(s) of the statement. | _Human-readable calculated display alias for the KnowledgeGaps row._ |
| Is Open | True when at least one of the following holds: the status is “Open” or the status is “Investigating”. | _TRUE when the gap remains open._ |
| **FA Q** | Frequently asked procedural questions. Maps to pko:FrequentlyAskedQuestion, question, answer, hasFAQCategory, and hasFAQTarget. | — |
| Name | The same as its question. | _Human-readable calculated display alias for the FAQs row._ |
| **Explanation** | Explainable derivation artifacts associated with procedural decisions. Maps to pko:Explanation and pko:hasExplanation. | — |
| Name | The same as its title. | _Human-readable calculated display alias for the Explanations row._ |
| **Procedure Execution** | Concrete enactments of procedure specifications. Maps to pko:ProcedureExecution and remains separate from ProcedureVersions. | — |
| Name | Computed as the procedure version, followed by “ / ”, followed by the context. | _Human-readable calculated display alias for the ProcedureExecutions row._ |
| **Step Execution** | Concrete executions of specified steps. Maps to pko:StepExecution, hasExecutedStep, includesStepExecution, and nextStepExecution. | — |
| Name | Computed as the procedure execution, followed by “ / ”, followed by the step. | _Human-readable calculated display alias for the StepExecutions row._ |
| Actual Duration Minutes | Determined by priority: 0 if the ended at is blank; in all other cases, the number of minutes from the started at to the ended at. | _Observed duration in minutes._ |
| Expected Duration Minutes | Taken from the linked step. | _Expected duration from the specification._ |
| Is Late | True when the actual duration minutes is greater than the expected duration minutes. | _TRUE when actual duration exceeds expected duration._ |
| **Requirement Satisfaction** | Execution-time evaluations of requirements. Maps to pko:RequirementSatisfaction, refersToRequirement, and hasRequirementSatisfactionLevel. | — |
| Name | Computed as the requirement, followed by “ / ”, followed by the satisfaction level. | _Human-readable calculated display alias for the RequirementSatisfactions row._ |
| **Error** | Reusable error definitions encountered during execution. Maps to pko:Error, errorCode, and errorCause. | — |
| Name | Computed as the error code, followed by “ - ”, followed by the label. | _Human-readable calculated display alias for the Errors row._ |
| **Issue Occurrence** | Concrete issue events during execution. Maps to pko:IssueOccurrence, hasEncounteredError, wasEncounteredBy, issueCause, and issueSolution. | — |
| Name | Computed as the error, followed by “ @ ”, followed by the occurred at. | _Human-readable calculated display alias for the IssueOccurrences row._ |
| **User Question** | Questions asked by agents during execution. Maps to pko:UserQuestionOccurrence, questionByUser, wasAskedBy, and isQuestionAddressedBy. | — |
| Name | Computed as the first 70 character(s) of the question text. | _Human-readable calculated display alias for the UserQuestions row._ |
| **User Feedback** | Feedback supplied by users about a procedure or execution. Maps to pko:UserFeedbackOccurrence, feedbackOnProcedureExecution, and wasProvidedBy. | — |
| Name | Computed as the disposition, followed by “: ”, followed by the first 60 character(s) of the feedback text. | _Human-readable calculated display alias for the UserFeedback row._ |
| **Stewardship Assignment** | Separates ongoing stewardship from authority to approve semantic commitments. Explicit ERB-PKO governance extension. | — |
| Name | Computed as the procedure version, followed by “ / steward=”, followed by the steward role. | _Human-readable calculated display alias for the StewardshipAssignments row._ |
| **Change Request** | Governed requests for semantic or operational change, anchored to a procedure version and authority. Explicit ERB-PKO extension. | — |
| Name | The same as its title. | _Human-readable calculated display alias for the ChangeRequests row._ |
| Is Open | True when at least one of the following holds: the status is “Draft”; the status is “UnderReview”; or the status is “Approved”. | _TRUE while the request remains active._ |
| **Review Event** | Periodic governance reviews that test competency coverage, staleness, and semantic integrity. Explicit ERB-PKO extension represented as prov:Activity. | — |
| Name | Computed as the procedure version, followed by “ / ”, followed by the review kind. | _Human-readable calculated display alias for the ReviewEvents row._ |
| Is Overdue | True when the next review due is less than the current date and time. | _TRUE when review is overdue._ |
| **Learning Activity** | Learning, retrospective, tabletop, and onboarding activities that convert execution experience into maintained knowledge. Explicit ERB-PKO extension represented as prov:Activity. | — |
| Name | Computed as the activity kind, followed by “ / ”, followed by the occurred at. | _Human-readable calculated display alias for the LearningActivities row._ |
| **Operational Binding** | Live bindings between procedural semantics and operational data/resources. Explicit ERB-PKO extension using DCAT/DCMI/PROV identifiers. | — |
| Name | Computed as the step, followed by “ / ”, followed by the record or schema key. | _Human-readable calculated display alias for the OperationalBindings row._ |
| Age Minutes | Computed as the number of minutes from the last observed at to the current date and time. ⚠︎ mechanical <!-- rulespeak:reword --> | _Current observed age._ |
| Is Fresh | True when the age minutes is at most the freshness sla minutes. | _TRUE when within freshness SLA._ |
| **Communication Policy** | Channel-specific communication policy projected from the same canonical procedure. Uses ODRL-style policy semantics plus ERB-PKO channel constraints. | — |
| Name | Computed as the channel, followed by “ policy / ”, followed by the procedure version. | _Human-readable calculated display alias for the CommunicationPolicies row._ |
| **Message Template** | Approved channel templates projected from the canonical rulebook without becoming a second source of policy meaning. | — |
| Name | Computed as the communication policy, followed by “ / ”, followed by the locale. | _Human-readable calculated display alias for the MessageTemplates row._ |
| **Semantic Mapping** | Machine-readable alignment from ERB table/field paths to exact PKO or reused ontology terms. Extension mappings are never presented as native PKO. | — |
| Name | Computed as the source path, followed by “ -> ”, followed by the target iri. | _Human-readable calculated display alias for the SemanticMappings row._ |
| **ERB Version** | Standard ERB semantic version history. | — |
| **ERB Customization** | Explicit customization seams; empty because the canonical model is expressed in the rulebook. | — |

## 2 Fact Types

- an **agent** may reference one **organization**
- a **role** may reference one **organization**
- a **role** may reference one **agent**
- a **role assignment** may reference one **role**
- a **role assignment** may reference one **agent**
- a **community of practice** may reference one **organization**
- a **community of practice** may reference one **role**
- a **mentorship** may reference one **community of practice**
- a **mentorship** may reference one **agent**
- a **procedure** may reference one **procedure type**
- a **procedure** may reference one **organization**
- a **procedure version** may reference one **procedure**
- a **procedure version** may reference one **agent**
- a **procedure version link** may reference one **procedure version**
- a **procedure status change** may reference one **procedure version**
- a **procedure status change** may reference one **agent**
- a **step** may reference one **procedure version**
- a **step** may reference one **role**
- a **step transition** may reference one **procedure version**
- a **step transition** may reference one **step**
- a **step action** may reference one **step**
- a **step action** may reference one **action**
- a **step function** may reference one **step**
- a **step function** may reference one **function**
- a **step tool** may reference one **step**
- a **step tool** may reference one **tool**
- a **step requirement** may reference one **step**
- a **step requirement** may reference one **requirement**
- a **step verification** may reference one **step**
- a **rationale** may reference one **procedure version**
- a **rationale** may reference one **step**
- a **rationale** may reference one **role**
- an **exception** may reference one **procedure version**
- an **exception** may reference one **step**
- an **exception** may reference one **role**
- a **procedure resource** may reference one **procedure version**
- a **procedure resource** may reference one **resource**
- an **elicitation session** may reference one **procedure version**
- an **elicitation session** may reference one **agent**
- a **knowledge fragment** may reference one **procedure version**
- a **knowledge fragment** may reference one **step**
- a **knowledge fragment** may reference one **elicitation session**
- a **knowledge fragment** may reference one **agent**
- a **knowledge fragment** may reference one **role**
- a **knowledge gap** may reference one **procedure version**
- a **knowledge gap** may reference one **step**
- a **knowledge gap** may reference one **role**
- an **FA q** may reference one **procedure version**
- an **FA q** may reference one **step**
- an **explanation** may reference one **procedure version**
- an **explanation** may reference one **step**
- a **procedure execution** may reference one **procedure version**
- a **procedure execution** may reference one **agent**
- a **step execution** may reference one **procedure execution**
- a **step execution** may reference one **step**
- a **step execution** may reference one **agent**
- a **requirement satisfaction** may reference one **step execution**
- a **requirement satisfaction** may reference one **requirement**
- a **requirement satisfaction** may reference one **agent**
- an **issue occurrence** may reference one **step execution**
- an **issue occurrence** may reference one **error**
- an **issue occurrence** may reference one **agent**
- a **user question** may reference one **step execution**
- a **user question** may reference one **agent**
- a **user question** may reference one **FA q**
- a **user question** may reference one **resource**
- a **user feedback** may reference one **procedure execution**
- a **user feedback** may reference one **agent**
- a **stewardship assignment** may reference one **procedure version**
- a **stewardship assignment** may reference one **role**
- a **change request** may reference one **procedure version**
- a **change request** may reference one **agent**
- a **change request** may reference one **role**
- a **review event** may reference one **procedure version**
- a **review event** may reference one **agent**
- a **review event** may reference one **change request**
- a **learning activity** may reference one **community of practice**
- a **learning activity** may reference one **procedure version**
- a **learning activity** may reference one **agent**
- a **learning activity** may reference one **resource**
- an **operational binding** may reference one **procedure version**
- an **operational binding** may reference one **step**
- an **operational binding** may reference one **resource**
- a **communication policy** may reference one **procedure version**
- a **communication policy** may reference one **role**
- a **message template** may reference one **communication policy**
- a **message template** may reference one **resource**
- a **semantic mapping** may reference one **ontology profile**

## 3 Operative Rules

_No operative rules yet. Required fields and foreign keys imply structural
`must`-rules automatically; to declare semantic obligations (`must` / `must not` / `should`), add a **Constraints** table whose rows point at
boolean calculated fields. See the tool README for the column contract._

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | A rulebook releas's name is computed as the rulebook version, followed by “ / PKO ”, followed by the pko core version iri. |
| **DR-2 Name** | An ontology profile's name is computed as the label, followed by a space, followed by the version. |
| **DR-3 Name** | An organization's name is the same as its display name. |
| **DR-4 Name** | An agent's name is the same as its display name. |
| **DR-5 Name** | A role's name is the same as its label. |
| **DR-6 Current Agent Kind** | A role's current agent kind — taken from the linked current agent. |
| **DR-7 Name** | A role assignment's name is computed as the role, followed by “ @ ”, followed by the valid from. |
| **DR-8 Is Current** | A role assignment is considered a current if all of the following hold: the valid from is at most the current date and time and at least one of the following holds: the valid to is blank or the valid to is greater than the current date and time. |
| **DR-9 Name** | A community of practice's name is the same as its label. |
| **DR-10 Name** | A mentorship's name is computed as the mentor agent, followed by “ -> ”, followed by the learner agent. |
| **DR-11 Name** | A procedure type's name is the same as its label. |
| **DR-12 Name** | A procedure's name is the same as its title. |
| **DR-13 Name** | A procedure version's name is the same as its title. |
| **DR-14 Count of Steps** | A procedure version's count of steps is the number of steps related to the procedure version. |
| **DR-15 Count of Open Knowledge Gaps** | A procedure version's count of open knowledge gaps is the number of the procedure version's knowledge gaps that have a status of “Open”. |
| **DR-16 Is Ready for Execution** | A procedure version is considered a ready for execution if all of the following hold: the status is “Approved”; the count of steps is greater than 0; and the count of open knowledge gaps is 0. |
| **DR-17 Name** | A procedure version link's name is computed as the previous procedure version, followed by “ -> ”, followed by the next procedure version. |
| **DR-18 Name** | A procedure status change's name is computed as the procedure version, followed by “: ”, followed by the from status, followed by “ -> ”, followed by the to status. |
| **DR-19 Name** | A step's name is computed as the step number, followed by “. ”, followed by the title. |
| **DR-20 Assigned Role Label** | A step's assigned role label — taken from the linked assigned role. |
| **DR-21 Assigned Agent Kind** | A step's assigned agent kind is the current agent kind of the step's assigned role. |
| **DR-22 Name** | A step transition's name is computed as the from step, followed by “ -> ”, followed by the to step. |
| **DR-23 Name** | An action's name is the same as its label. |
| **DR-24 Name** | A function's name is the same as its label. |
| **DR-25 Name** | A tool's name is the same as its label. |
| **DR-26 Name** | A step action's name is computed as the step, followed by “ / ”, followed by the action. |
| **DR-27 Name** | A step function's name is computed as the step, followed by “ / ”, followed by the function. |
| **DR-28 Name** | A step tool's name is computed as the step, followed by “ / ”, followed by the tool. |
| **DR-29 Name** | A requirement's name is the same as its label. |
| **DR-30 Name** | A step requirement's name is computed as the step, followed by “ / ”, followed by the requirement. |
| **DR-31 Name** | A step verification's name is computed as the step, followed by “ / ”, followed by the verification kind. |
| **DR-32 Name** | A rationale's name is the same as its title. |
| **DR-33 Name** | An exception's name is the same as its condition. |
| **DR-34 Name** | A resource's name is the same as its title. |
| **DR-35 Name** | A procedure resource's name is computed as the procedure version, followed by “ / ”, followed by the resource. |
| **DR-36 Relation Iri** | The procedure resource's relation iri is determined by the following priority:<br>1. “https://w3id.org/pko#wasExtractedFrom”, if the relation is “wasExtractedFrom”;<br>2. in all other cases, “http://purl.org/dc/terms/references”. |
| **DR-37 Name** | An elicitation session's name is computed as the method, followed by “ / ”, followed by the started at. |
| **DR-38 Name** | A knowledge fragment's name is computed as the knowledge form, followed by “: ”, followed by the first 60 character(s) of the statement. |
| **DR-39 Is Currently Valid** | A knowledge fragment is considered currently-valid if all of the following hold: the valid from is at most the current date and time; at least one of the following holds: the valid to is blank or the valid to is greater than the current date and time; and the status is “Approved”. |
| **DR-40 Name** | A knowledge gap's name is computed as the severity, followed by “: ”, followed by the first 60 character(s) of the statement. |
| **DR-41 Is Open** | A knowledge gap is considered open if at least one of the following holds: the status is “Open” or the status is “Investigating”. |
| **DR-42 Name** | An FA q's name is the same as its question. |
| **DR-43 Name** | An explanation's name is the same as its title. |
| **DR-44 Name** | A procedure execution's name is computed as the procedure version, followed by “ / ”, followed by the context. |
| **DR-45 Name** | A step execution's name is computed as the procedure execution, followed by “ / ”, followed by the step. |
| **DR-46 Actual Duration Minutes** | The step execution's actual duration minutes is determined by the following priority:<br>1. 0, if the ended at is blank;<br>2. in all other cases, the number of minutes from the started at to the ended at. |
| **DR-47 Expected Duration Minutes** | A step execution's expected duration minutes — taken from the linked step. |
| **DR-48 Is Late** | A step execution is considered a late if the actual duration minutes is greater than the expected duration minutes. |
| **DR-49 Name** | A requirement satisfaction's name is computed as the requirement, followed by “ / ”, followed by the satisfaction level. |
| **DR-50 Name** | An error's name is computed as the error code, followed by “ - ”, followed by the label. |
| **DR-51 Name** | An issue occurrence's name is computed as the error, followed by “ @ ”, followed by the occurred at. |
| **DR-52 Name** | A user question's name is computed as the first 70 character(s) of the question text. |
| **DR-53 Name** | A user feedback's name is computed as the disposition, followed by “: ”, followed by the first 60 character(s) of the feedback text. |
| **DR-54 Name** | A stewardship assignment's name is computed as the procedure version, followed by “ / steward=”, followed by the steward role. |
| **DR-55 Name** | A change request's name is the same as its title. |
| **DR-56 Is Open** | A change request is considered open if at least one of the following holds: the status is “Draft”; the status is “UnderReview”; or the status is “Approved”. |
| **DR-57 Name** | A review event's name is computed as the procedure version, followed by “ / ”, followed by the review kind. |
| **DR-58 Is Overdue** | A review event is considered an overdue if the next review due is less than the current date and time. |
| **DR-59 Name** | A learning activity's name is computed as the activity kind, followed by “ / ”, followed by the occurred at. |
| **DR-60 Name** | An operational binding's name is computed as the step, followed by “ / ”, followed by the record or schema key. |
| **DR-61 Age Minutes** | An operational binding's age minutes is computed as the number of minutes from the last observed at to the current date and time. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-62 Is Fresh** | An operational binding is considered a fresh if the age minutes is at most the freshness sla minutes. |
| **DR-63 Name** | A communication policy's name is computed as the channel, followed by “ policy / ”, followed by the procedure version. |
| **DR-64 Name** | A message template's name is computed as the communication policy, followed by “ / ”, followed by the locale. |
| **DR-65 Name** | A semantic mapping's name is computed as the source path, followed by “ -> ”, followed by the target iri. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **RulebookReleases.Name** | formula | `RulebookVersion & " / PKO " & PkoCoreVersionIri` |
| **OntologyProfiles.Name** | formula | `Label & " " & Version` |
| **Organizations.Name** | formula | `DisplayName` |
| **Agents.Name** | formula | `DisplayName` |
| **Roles.Name** | formula | `Label` |
| **Roles.CurrentAgentKind** | lookup | `Lookup(Agents.AgentKind via CurrentAgent)` |
| **RoleAssignments.Name** | formula | `Role & " @ " & ValidFrom` |
| **RoleAssignments.IsCurrent** | formula | `And(ValidFrom <= Now(), Or(ValidTo = "", ValidTo > Now()))` |
| **CommunitiesOfPractice.Name** | formula | `Label` |
| **Mentorships.Name** | formula | `MentorAgent & " -> " & LearnerAgent` |
| **ProcedureTypes.Name** | formula | `Label` |
| **Procedures.Name** | formula | `Title` |
| **ProcedureVersions.Name** | formula | `Title` |
| **ProcedureVersions.CountOfSteps** | rollup | `Count(Steps via ProcedureVersion)` |
| **ProcedureVersions.CountOfOpenKnowledgeGaps** | rollup | `Count(KnowledgeGaps via ProcedureVersion)` |
| **ProcedureVersions.IsReadyForExecution** | formula | `And(Status = "Approved", CountOfSteps > 0, CountOfOpenKnowledgeGaps = 0)` |
| **ProcedureVersionLinks.Name** | formula | `PreviousProcedureVersion & " -> " & NextProcedureVersion` |
| **ProcedureStatusChanges.Name** | formula | `ProcedureVersion & ": " & FromStatus & " -> " & ToStatus` |
| **Steps.Name** | formula | `StepNumber & ". " & Title` |
| **Steps.AssignedRoleLabel** | lookup | `Lookup(Roles.Label via AssignedRole)` |
| **Steps.AssignedAgentKind** | lookup | `Lookup(Roles.CurrentAgentKind via AssignedRole)` |
| **StepTransitions.Name** | formula | `FromStep & " -> " & ToStep` |
| **Actions.Name** | formula | `Label` |
| **Functions.Name** | formula | `Label` |
| **Tools.Name** | formula | `Label` |
| **StepActions.Name** | formula | `Step & " / " & Action` |
| **StepFunctions.Name** | formula | `Step & " / " & Function` |
| **StepTools.Name** | formula | `Step & " / " & Tool` |
| **Requirements.Name** | formula | `Label` |
| **StepRequirements.Name** | formula | `Step & " / " & Requirement` |
| **StepVerifications.Name** | formula | `Step & " / " & VerificationKind` |
| **Rationales.Name** | formula | `Title` |
| **Exceptions.Name** | formula | `Condition` |
| **Resources.Name** | formula | `Title` |
| **ProcedureResources.Name** | formula | `ProcedureVersion & " / " & Resource` |
| **ProcedureResources.RelationIri** | formula | `If(Relation = "wasExtractedFrom", "https://w3id.org/pko#wasExtractedFrom", "http://purl.org/dc/terms/references")` |
| **ElicitationSessions.Name** | formula | `Method & " / " & StartedAt` |
| **KnowledgeFragments.Name** | formula | `KnowledgeForm & ": " & Left(Statement, 60)` |
| **KnowledgeFragments.IsCurrentlyValid** | formula | `And(ValidFrom <= Now(), Or(ValidTo = "", ValidTo > Now()), Status = "Approved")` |
| **KnowledgeGaps.Name** | formula | `Severity & ": " & Left(Statement, 60)` |
| **KnowledgeGaps.IsOpen** | formula | `Or(Status = "Open", Status = "Investigating")` |
| **FAQs.Name** | formula | `Question` |
| **Explanations.Name** | formula | `Title` |
| **ProcedureExecutions.Name** | formula | `ProcedureVersion & " / " & Context` |
| **StepExecutions.Name** | formula | `ProcedureExecution & " / " & Step` |
| **StepExecutions.ActualDurationMinutes** | formula | `If(EndedAt = "", 0, DaysBetween(EndedAt, StartedAt))` |
| **StepExecutions.ExpectedDurationMinutes** | lookup | `Lookup(Steps.ExpectedDurationMinutes via Step)` |
| **StepExecutions.IsLate** | formula | `ActualDurationMinutes > ExpectedDurationMinutes` |
| **RequirementSatisfactions.Name** | formula | `Requirement & " / " & SatisfactionLevel` |
| **Errors.Name** | formula | `ErrorCode & " - " & Label` |
| **IssueOccurrences.Name** | formula | `Error & " @ " & OccurredAt` |
| **UserQuestions.Name** | formula | `Left(QuestionText, 70)` |
| **UserFeedback.Name** | formula | `Disposition & ": " & Left(FeedbackText, 60)` |
| **StewardshipAssignments.Name** | formula | `ProcedureVersion & " / steward=" & StewardRole` |
| **ChangeRequests.Name** | formula | `Title` |
| **ChangeRequests.IsOpen** | formula | `Or(Status = "Draft", Status = "UnderReview", Status = "Approved")` |
| **ReviewEvents.Name** | formula | `ProcedureVersion & " / " & ReviewKind` |
| **ReviewEvents.IsOverdue** | formula | `NextReviewDue < Now()` |
| **LearningActivities.Name** | formula | `ActivityKind & " / " & OccurredAt` |
| **OperationalBindings.Name** | formula | `Step & " / " & RecordOrSchemaKey` |
| **OperationalBindings.AgeMinutes** | formula | `DaysBetween(Now(), LastObservedAt)` |
| **OperationalBindings.IsFresh** | formula | `AgeMinutes <= FreshnessSlaMinutes` |
| **CommunicationPolicies.Name** | formula | `Channel & " policy / " & ProcedureVersion` |
| **MessageTemplates.Name** | formula | `CommunicationPolicy & " / " & Locale` |
| **SemanticMappings.Name** | formula | `SourcePath & " -> " & TargetIri` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
