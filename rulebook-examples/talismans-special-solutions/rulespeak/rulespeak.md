# 📘 Talisman's Special Solutions — RuleSpeak

_The NTWF (Talisman's Special Solutions Workflow) ontology from Jessica Talisman's 'Intentional Arrangement' series, modeled as a relational rulebook. One curated worked example — the Production Deployment Workflow — exercises every class, property, and competency question: three disjoint agent types (human / AI / pipeline), role-vs-agent separation, a delegation/escalation chain, an approval gate as a step subtype, step-to-step transitive ordering, a PROV provenance chain over artifacts, and DCAT dataset consumption. Every competency question is answered by a derived column or a transitive-closure view (no sidecar code); a small set of load-bearing INDEX/MATCH lookups resolves the role-to-agent and gate-to-role-to-approver chains the questions depend on._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Workflow** | A workflow is identified by its name and is related to optionally a workflow status concept (its workflow status). | — |
| Relative Path | Computed as the literal “workflows/”, followed by the workflow ID. | _Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation._ |
| Count of Non Proposed Steps | The number of workflow steps related to the workflow. | _Calculated count of workflow steps in this workflow. Useful for workflow complexity analysis and reporting._ |
| Has More Than1 Step | True when the count of non proposed steps is greater than 1. | — |
| Count AI Steps | The number of the workflow's workflow steps that are executed by AI. | _Number of steps in this workflow executed by an AIAgent (rollup over WorkflowSteps.IsExecutedByAI). This is the 'AI-executed' half of the article's CQ3 ('which steps are executed by AI agents') — counted against AIAgent individuals specifically, not the deterministic AutomatedPipeline, which is a disjoint agent type. Also drives the business-payoff query (a workflow is a compliance risk when an AI agent runs a step). Worked example: 2 (the AI risk-assessment step and the AI post-deployment health report)._ |
| Count Human Steps | The number of the workflow's workflow steps that are executed by humans. | _Number of steps executed by a HumanAgent (rollup over WorkflowSteps.IsExecutedByHuman). The 'who actually runs this step' count — distinct from CountHumanRequiredSteps, which counts steps that demand a human decision (requiresHumanApproval). Worked example: 2 (the legal-review step and the release approval gate)._ |
| Count Human Required Steps | The number of the workflow's workflow steps that require human approval. | _Number of steps that require a human decision (rollup over WorkflowSteps.RequiresHumanApproval). This is the 'human-required' half of the article's CQ3 ('which require a human decision'), answered — as the article notes — by a single FILTER on requiresHumanApproval. Worked example: 2 (the legal-review step and the release approval gate)._ |
| Count Approval Consistency Violations | The number of the workflow's workflow steps that are approval consistency violation. | _Number of steps that require human approval but are not human-filled (rollup over WorkflowSteps.ApprovalConsistencyViolation). The clean ABox witness: this is 0 for the Production Deployment workflow. A non-zero value is the relational signal of a Suite-4 consistency violation._ |
| Has Consistency Violation | True when the count approval consistency violations is greater than 0. | _TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT._ |
| Has AI Agent Step | True when the count AI steps is greater than 0. | _TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query._ |
| Months Since Modified | Computed as the number of months from the modified to the current date and time. ⚠︎ mechanical <!-- rulespeak:reword --> | _Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable._ |
| Is Stale | True when the months since modified is greater than the staleness threshold months. | _TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field._ |
| Is Stale and Has AI Agent | True when all of the following hold: the is stale flag is set and the has AI agent step flag is set. | _The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column._ |
| Count Derivation Links | The number of the workflow's workflow artifacts that have a derivation parent. | _Number of prov:wasDerivedFrom links among this workflow's artifacts (rollup over WorkflowArtifacts.HasDerivationParent). Answers the lineage half of CQ4: 5 artifacts form a chain with 4 derivation links._ |
| Count Legal Owned Steps | The number of the workflow's workflow steps that are legal owned. | _Number of steps in this workflow whose owning department is Legal (rollup over WorkflowSteps.IsLegalOwned). CQ7: exactly one Legal-owned step in the Production Deployment workflow._ |
| Count Engineering Owned Steps | The number of the workflow's workflow steps that are engineering owned. | _Number of steps whose owning department is Engineering (rollup over WorkflowSteps.IsEngineeringOwned). Feeds CQ7's Engineering-involvement check._ |
| Involves Engineering and Legal | True when all of the following hold: the count engineering owned steps is greater than 0 and the count legal owned steps is greater than 0. | _TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step)._ |
| Count Inferred Precedence Pairs | The number of vw step precedence closure related to the workflow. | _Number of step-ordering pairs that the transitive closure of ntwf:precedesStep INFERRED (rollup over the closure view vw_step_precedence_closure where is_inferred = TRUE). The article's signature count: 6 of the 10 closure pairs were never asserted — including step-1 -> step-5. NOTE: this single-workflow model has exactly one Workflow, so the global closure view is wholly this workflow's; the COUNTIFS is unfiltered because every precedence edge belongs to the Production Deployment DAG._ |
| Count Asserted Precedence Pairs | The number of vw step precedence closure related to the workflow. | _Number of step-ordering pairs that were directly ASSERTED as ntwf:precedesStep edges (rollup over vw_step_precedence_closure where is_inferred = FALSE) — the hop-1 rows. The article's 4 asserted edges. Together with CountInferredPrecedencePairs (6) this sums to the 10-pair closure, making CountOfPrecedenceClosurePairs an honest asserted+inferred total rather than an unconditional count. Single-workflow note as on CountInferredPrecedencePairs: the global closure view is this workflow's._ |
| Count of Precedence Closure Pairs | Computed as the count asserted precedence pairs plus the count inferred precedence pairs. | _Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them._ |
| Count Roles With Bad Filler Cardinality | The number of roles related to the workflow. | _Number of roles that do NOT have exactly one filledBy arm set (rollup over Roles.HasExactlyOneFiller = FALSE). The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has 0 such roles — this is the Suite-1 functional/disjointness witness as a single integer. A non-zero value is the relational signal of the Suite-4 disjointness violation (a role filled by two agent classes, or by none). NOTE: this single-workflow model has exactly one Workflow and every Role participates in it, so the count is over all roles; a multi-workflow model would scope it through a role→workflow path._ |
| Count Agent Type Changes | The number of role assignments related to the workflow. | _Number of filledBy assignment periods that changed the agent CLASS of a role (rollup over RoleAssignments.IsAgentTypeChange = TRUE). NTWF governance distinguishes a same-class personnel/model swap from an agent-type transition; this counts the latter. NOTE: single-workflow model — every Role participates in the one workflow, so the count is over all assignment history; a multi-workflow model would scope it through a role→workflow path._ |
| Count Compliance Audit Changes | The number of role assignments related to the workflow. | _Number of filledBy assignment periods that took a previously AI-executed binding and reassigned it to a human (rollup over RoleAssignments.RequiresComplianceAudit = TRUE). NTWF governance treats this as a data operation with compliance implications: changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent. Each such row must carry when (ValidFrom) and why (Reason). NOTE: single-workflow scoping as above._ |
| **Workflow Step** | A workflow step is identified by its name and is related to optionally a workflow, optionally a role (its assigned role), optionally a dataset (its consumes dataset), optionally a workflow artifact (its produces artifacts), optionally a workflow artifact (its requires artifacts), optionally an approval gate, optionally a step precedence (its precedes), and optionally a step precedence (its preceded by). | — |
| Parent Path | The relative path of the workflow step's workflow. | _Helper: the Workflows parent's RelativePath, pulled across the Workflow FK. Exists so RelativePath can concatenate the '/steps/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)._ |
| Relative Path | Computed as the parent path, followed by the literal “/steps/”, followed by the workflow step ID. | _Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Executing Human Agent | The filled by human agent of the workflow step's assigned role. | _The HumanAgent (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the HumanAgent arm). Load-bearing lookup: it follows the role→agent indirection the article relies on, so a step knows its executing agent without per-step agent bindings._ |
| Executing AI Agent | The filled by AI agent of the workflow step's assigned role. | _The AIAgent (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the AIAgent arm). One of the three polymorphic filledBy arms._ |
| Executing Automated Pipeline | The filled by automated pipeline of the workflow step's assigned role. | _The AutomatedPipeline (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the AutomatedPipeline arm)._ |
| Executing Agent Type | Determined by priority: the literal “HumanAgent” if the executing human agent has a value; the literal “AIAgent” if the executing AI agent has a value; the literal “AutomatedPipeline” if the executing automated pipeline has a value; otherwise an empty string. | _Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision')._ |
| Is Executed by AI | True when the executing AI agent has a value. | _TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps)._ |
| Is Executed by Human | True when the executing human agent has a value. | _TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split._ |
| Approval Consistency Violation | True when all of the following hold: the requires human approval flag is set and the executing human agent is blank. | _Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check._ |
| Approval is Human Filled | True when the executing human agent has a value if the requires human approval flag is set, otherwise the TRUE. | _Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval._ |
| Owning Department | The owned by of the workflow step's assigned role. | _The department that owns this step's assigned role, resolved through AssignedRole → Roles.OwnedBy. Lets a workflow report which departments its steps touch (CQ7: 'which workflows involve both Engineering and Legal, and at what steps do they intersect')._ |
| Is Legal Owned | True when the owning department is the literal “ntwf-legal-dept”. | _TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow)._ |
| Is Engineering Owned | True when the owning department is the literal “ntwf-engineering”. | _TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check._ |
| **Approval Gate** | An approval gate is identified by its name and is related to optionally a workflow step. | — |
| Parent Path | The relative path of the approval gate's workflow step. | _Helper: the WorkflowSteps parent's RelativePath, pulled across the WorkflowStep FK. Exists so RelativePath can concatenate the '/approval-gates/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)._ |
| Relative Path | Computed as the parent path, followed by the literal “/approval-gates/”, followed by the approval gate ID. | _Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Gate Role | The assigned role of the approval gate's workflow step. | _The role responsible for this gate's underlying step, resolved through WorkflowStep → WorkflowSteps.AssignedRole. First hop of the CQ2 chain (gate → role → approver)._ |
| Gate Approver Human | The filled by human agent of the approval gate's gate role. | _The human agent who approves at this gate, resolved through the two-hop chain gate → GateRole → Roles.FilledByHumanAgent. Answers CQ2 ('who is responsible for approving a production deployment') directly: the release-approval gate resolves to the Release Manager role, filled by Maria Gonzalez._ |
| **Step Precedence** | A step precedence is identified by its name and is related to a workflow step (its from step) and a workflow step (its to step). | — |
| Parent Path | The relative path of the step precedence's from step. | _Helper: the WorkflowSteps parent's RelativePath, pulled across the FromStep FK. Exists so RelativePath can concatenate the '/precedence/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)._ |
| Relative Path | Computed as the parent path, followed by the literal “/precedence/”, followed by the step precedence ID. | _Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the from step, followed by the literal “ -> ”, followed by the to step. | _Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction._ |
| **Role** | A role is identified by its name and is related to optionally an agent capability concept (its has capability), optionally a human agent (its filled by human agent), optionally an AI agent (its filled by AI agent), optionally an automated pipeline (its filled by automated pipeline), optionally a department (its owned by), optionally another role (its delegates to), and optionally another role (its from delegates to). | — |
| Relative Path | Computed as the literal “roles/”, followed by the role ID. | _Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Filled by Arm Count | Computed as the count of the following that hold: the filled by human agent has a value; the filled by AI agent has a value; and the filled by automated pipeline has a value. | _Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint._ |
| Has Exactly One Filler | True when the filled by arm count is 1. | _Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE._ |
| Filler Type | Determined by priority: the literal “HumanAgent” if the filled by human agent has a value; the literal “AIAgent” if the filled by AI agent has a value; the literal “AutomatedPipeline” if the filled by automated pipeline has a value; otherwise an empty string. | _Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'._ |
| **Role Assignment** | A role assignment is identified by its name and is related to a role, optionally a human agent (its filled by human agent), optionally an AI agent (its filled by AI agent), and optionally an automated pipeline (its filled by automated pipeline). | — |
| Parent Path | The relative path of the role assignment's role. | _Helper: the Roles parent's RelativePath, pulled across the Role FK. Exists so RelativePath can concatenate the '/assignments/' segment using only local-field '&' concat._ |
| Relative Path | Computed as the parent path, followed by the literal “/assignments/”, followed by the role assignment ID. | _Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique._ |
| Name | Computed as the role, followed by the literal “ [”, followed by the valid from, followed by the literal “ -> ”, followed by the literal “open” if the valid to is blank, otherwise the valid to, followed by the literal “]”. | _Human-readable label for this assignment period: the role and the validity window._ |
| Filler Type | Determined by priority: the literal “HumanAgent” if the filled by human agent has a value; the literal “AIAgent” if the filled by AI agent has a value; the literal “AutomatedPipeline” if the filled by automated pipeline has a value; otherwise an empty string. | _Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding._ |
| Is Current | True when the valid to is blank. | _TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips._ |
| Was Active As of Audit Date | True when all of the following hold: the valid from is at most the literal “2026-03-01” and at least one of the following holds: the valid to is blank or the valid to is greater than the literal “2026-03-01”. | _NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained._ |
| Is Agent Type Change | True when all of the following hold: the prior filler type has a value and the prior filler type is not the filler type. | _TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight._ |
| Requires Compliance Audit | True when all of the following hold: the prior filler type has a value; the prior filler type is the literal “AIAgent”; and the filler type is the literal “HumanAgent”. | _Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why)._ |
| **Department** | A department is identified by its name. | — |
| Relative Path | Computed as the literal “departments/”, followed by the department ID. | _Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Human-readable display name of the department. Should match organizational terminology for stakeholder communication._ |
| **Human Agent** | A human agent is identified by its name. | — |
| Relative Path | Computed as the literal “human-agents/”, followed by the human agent ID. | _Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **AI Agent** | An AI agent is identified by its name and is related to optionally a workflow artifact (its attributed artifacts). | — |
| Relative Path | Computed as the literal “ai-agents/”, followed by the AI agent ID. | _Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Count Attributed Artifacts | The number of workflow artifacts related to the AI agent. | _'Blast radius', leg 1: how many artifacts are attributed to this AI agent (prov:wasAttributedTo). Counts WorkflowArtifacts whose AttributedToAIAgent is this agent._ |
| Count Impacted Workflows | The number of the AI agent's workflow artifacts that have a producing workflow. | _'Blast radius', summarized: the number of distinct workflows reachable from this agent's attributed artifacts (each artifact is produced by a step that belongs to a workflow). With one workflow in the worked example, an upgrade to an agent that produced any artifact has a blast radius of 1 workflow. Counts artifacts attributed to this agent that resolve to a workflow._ |
| **Automated Pipeline** | An automated pipeline is identified by its name. | — |
| Relative Path | Computed as the literal “automated-pipelines/”, followed by the automated pipeline ID. | _Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Workflow Status Concept** | SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows. | — |
| Relative Path | Computed as the literal “concepts/workflow-status/”, followed by the concept ID. | _Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Agent Capability Concept** | SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox. | — |
| Relative Path | Computed as the literal “concepts/agent-capability/”, followed by the concept ID. | _Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Artifact Type Concept** | SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts. | — |
| Relative Path | Computed as the literal “concepts/artifact-type/”, followed by the concept ID. | _Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value._ |
| **Dataset** | DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?' | — |
| Relative Path | Computed as the literal “datasets/”, followed by the dataset ID. | _Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Workflow Artifact** | Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent. | — |
| Parent Path | The relative path of the workflow artifact's produced by step. | _Helper: the WorkflowSteps parent's RelativePath, pulled across the ProducedByStep FK. Exists so RelativePath can concatenate the '/artifacts/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)._ |
| Relative Path | Computed as the parent path, followed by the literal “/artifacts/”, followed by the artifact ID. | _Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Producing Agent Type | Determined by priority: the literal “HumanAgent” if the attributed to human agent has a value; the literal “AIAgent” if the attributed to AI agent has a value; the literal “AutomatedPipeline” if the attributed to automated pipeline has a value; otherwise an empty string. | _Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from._ |
| Has Derivation Parent | True when the derived from artifact has a value. | _TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent._ |
| Produced by Workflow | The workflow of the workflow artifact's produced by step. | _The workflow this artifact belongs to, resolved through ProducedByStep → WorkflowSteps.Workflow (artifact → producing step → workflow). Lets workflow-level rollups (e.g. CountDerivationLinks) aggregate artifacts without a redundant direct FK._ |
| Has Producing Workflow | True when the produced by workflow has a value. | _TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column._ |
| **Governance Role** | A governance role is identified by its name and is related to optionally a change log (its approved changes). | — |
| Relative Path | Computed as the literal “governance-roles/”, followed by the governance role ID. | _Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug form of the display name._ |
| Can Approve Changes | True when the kind is the literal “Authority”. | _TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure._ |
| **Change Log** | A change log is identified by its name and is related to optionally a governance role (its approved by). | — |
| Relative Path | Computed as the literal “change-log/”, followed by the change log ID. | _Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath)._ |
| Name | Computed as the version, followed by the literal “ (”, followed by the change date, followed by the literal “)”. | _Human-readable label: the version and date of this change._ |
| Is Breaking Change | True when the change kind is the literal “major”. | _TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version._ |
| Is Backward Compatible | True when at least one of the following holds: the change kind is the literal “patch” or the change kind is the literal “minor”. | _TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it._ |
| **Vocabulary Reconciliation** | A vocabulary reconciliation is identified by its name. | — |
| Relative Path | Computed as the literal “reconciliations/”, followed by the reconciliation ID. | _Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath)._ |
| Name | Computed as the deprecated term, followed by the literal “ owl:sameAs ”, followed by the replacement term. | _Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement._ |
| **Scenario** | A scenario is identified by its name. | — |
| Relative Path | Computed as the literal “scenarios/”, followed by the scenario ID. | _DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (dash-form of RelativePath)._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug form of the human label._ |
| **Competency Question** | The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook. | — |
| Relative Path | Computed as the literal “competency-questions/”, followed by the competency question ID. | _Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables._ |
| **Conformance Test** | A conformance test is identified by its name. | — |
| Relative Path | Computed as the literal “conformance-tests/”, followed by the conformance test ID. | _DAG-derived location for this test row: root segment 'conformance-tests' + the primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Slug IRI for this row, derived from RelativePath._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Machine name derived from the display name._ |

## 2 Fact Types

- a **workflow** may reference one **workflow status concept**
- a **workflow step** may reference one **workflow**
- a **workflow step** may reference one **role**
- a **workflow step** may reference one **dataset**
- a **workflow step** may reference one **workflow artifact**
- a **workflow step** may reference one **approval gate**
- a **workflow step** may reference one **step precedence**
- an **approval gate** may reference one **workflow step**
- a **step precedence** references exactly one **workflow step**
- a **role** may reference one **agent capability concept**
- a **role** may reference one **human agent**
- a **role** may reference one **AI agent**
- a **role** may reference one **automated pipeline**
- a **role** may reference one **department**
- a **role** may reference one **role**
- a **role assignment** references exactly one **role**
- a **role assignment** may reference one **human agent**
- a **role assignment** may reference one **AI agent**
- a **role assignment** may reference one **automated pipeline**
- an **AI agent** may reference one **workflow artifact**
- a **dataset** may reference one **workflow step**
- a **workflow artifact** may reference one **artifact type concept**
- a **workflow artifact** may reference one **workflow step**
- a **workflow artifact** may reference one **workflow artifact**
- a **workflow artifact** may reference one **human agent**
- a **workflow artifact** may reference one **AI agent**
- a **workflow artifact** may reference one **automated pipeline**
- a **governance role** may reference one **change log**
- a **change log** may reference one **governance role**

## 2b Reachability Rules

_A reachability rule is a transitive closure: relationships that hold not only
directly but through any chain of the same relationship. The asserted edges are
the single source of truth; the inferred edges are necessary consequences of them._

- **Precedes Step Closure** — one step precedence is reachable from another by the **precedes step** relationship
  when the second can be reached from the first by following one or more **precedes step** edges
  (from its from step to its to step), whether directly asserted or reached transitively.
  - An edge is **asserted** when it exists directly in the step precedence; it is **inferred**
    when no direct edge states it but it follows from a chain of asserted edges.
  - The **hop distance** of a reachable pair is the length of the shortest such chain
    (1 for a directly-asserted edge).
  - _Transitive closure of ntwf:precedesStep (an owl:TransitiveProperty). The 4 asserted edges (1→2, 2→3, 3→4, 4→5) imply the full 10-pair ordering closure — including the never-asserted step-1 → step-5. Materialized by the transpiler as the view vw_step_precedence_closure(from_id, to_id, hop_distance, is_inferred): 4 asserted (hop 1) + 6 inferred rows. This is the article's headline inference made to fire, not seeded._
- **Delegation Closure** — one role is reachable from another by the **delegation** relationship
  when the second can be reached from the first by following one or more **delegation** edges
  (from its source to its delegates to), whether directly asserted or reached transitively.
  - An edge is **asserted** when it exists directly in the roles; it is **inferred**
    when no direct edge states it but it follows from a chain of asserted edges.
  - The **hop distance** of a reachable pair is the length of the shortest such chain
    (1 for a directly-asserted edge).
  - _Transitive closure of ntwf:delegatesTo over the self-referential DelegatesTo FK. The asserted escalation edges (Release Manager → VP Engineering, VP Engineering → CTO) imply the never-asserted reachability Release Manager → CTO. Materialized as vw_roles_closure(from_id, to_id, hop_distance, is_inferred). This is the SQL equivalent of the SPARQL delegatesTo+ property path._

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A step precedence **must** reference exactly one workflow step as its from step.
- A step precedence **must** reference exactly one workflow step as its to step.
- A role assignment **must** reference exactly one role.
- A role assignment **must** have a valid from.
- A workflow status concept **must** have a pref label.
- An agent capability concept **must** have a pref label.
- An artifact type concept **must** have a pref label.
- A dataset **must** have a title.
- A workflow artifact **must** have a title.
- A scenario **must** have a label and an edits.
- A competency question **must** have a number, a display name, a question text, a target table, a target field, an answer kind, and an expected answer.
- A conformance test **must** have a display name, a section, a test kind, and a sort order, and record whether it is enabled.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Relative Path** | A workflow's relative path is computed as the literal “workflows/”, followed by the workflow ID. |
| **DR-2 Iri** | A workflow's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-3 Name** | A workflow's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-4 Count of Non Proposed Steps** | A workflow's count of non proposed steps is the number of workflow steps related to the workflow. |
| **DR-5 Has More Than1 Step** | A workflow is considered to have a more than1 step if the count of non proposed steps is greater than 1. |
| **DR-6 Count AI Steps** | A workflow's count AI steps is the number of the workflow's workflow steps that are executed by AI. |
| **DR-7 Count Human Steps** | A workflow's count human steps is the number of the workflow's workflow steps that are executed by humans. |
| **DR-8 Count Human Required Steps** | A workflow's count human required steps is the number of the workflow's workflow steps that require human approval. |
| **DR-9 Count Approval Consistency Violations** | A workflow's count approval consistency violations is the number of the workflow's workflow steps that are approval consistency violation. |
| **DR-10 Has Consistency Violation** | A workflow is considered to have a consistency violation if the count approval consistency violations is greater than 0. |
| **DR-11 Has AI Agent Step** | A workflow is considered to have an AI agent step if the count AI steps is greater than 0. |
| **DR-12 Months Since Modified** | A workflow's months since modified is computed as the number of months from the modified to the current date and time. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-13 Is Stale** | A workflow is considered a stale if the months since modified is greater than the staleness threshold months. |
| **DR-14 Is Stale and Has AI Agent** | A workflow is considered a stale and has AI agent if all of the following hold: the is stale flag is set and the has AI agent step flag is set. |
| **DR-15 Count Derivation Links** | A workflow's count derivation links is the number of the workflow's workflow artifacts that have a derivation parent. |
| **DR-16 Count Legal Owned Steps** | A workflow's count legal owned steps is the number of the workflow's workflow steps that are legal owned. |
| **DR-17 Count Engineering Owned Steps** | A workflow's count engineering owned steps is the number of the workflow's workflow steps that are engineering owned. |
| **DR-18 Involves Engineering and Legal** | A workflow is considered to involve engineering and legal if all of the following hold: the count engineering owned steps is greater than 0 and the count legal owned steps is greater than 0. |
| **DR-19 Count Inferred Precedence Pairs** | A workflow's count inferred precedence pairs is the number of vw step precedence closure related to the workflow. |
| **DR-20 Count Asserted Precedence Pairs** | A workflow's count asserted precedence pairs is the number of vw step precedence closure related to the workflow. |
| **DR-21 Count of Precedence Closure Pairs** | A workflow's count of precedence closure pairs is computed as the count asserted precedence pairs plus the count inferred precedence pairs. |
| **DR-22 Count Roles With Bad Filler Cardinality** | A workflow's count roles with bad filler cardinality is the number of roles related to the workflow. |
| **DR-23 Count Agent Type Changes** | A workflow's count agent type changes is the number of role assignments related to the workflow. |
| **DR-24 Count Compliance Audit Changes** | A workflow's count compliance audit changes is the number of role assignments related to the workflow. |
| **DR-25 Parent Path** | A workflow step's parent path is the relative path of the workflow step's workflow. |
| **DR-26 Relative Path** | A workflow step's relative path is computed as the parent path, followed by the literal “/steps/”, followed by the workflow step ID. |
| **DR-27 Iri** | A workflow step's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-28 Name** | A workflow step's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-29 Executing Human Agent** | A workflow step's executing human agent is the filled by human agent of the workflow step's assigned role. |
| **DR-30 Executing AI Agent** | A workflow step's executing AI agent is the filled by AI agent of the workflow step's assigned role. |
| **DR-31 Executing Automated Pipeline** | A workflow step's executing automated pipeline is the filled by automated pipeline of the workflow step's assigned role. |
| **DR-32 Executing Agent Type** | The workflow step's executing agent type is determined by the following priority:<br>1. the literal “HumanAgent”, if the executing human agent has a value;<br>2. the literal “AIAgent”, if the executing AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the executing automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-33 Is Executed by AI** | A workflow step is considered an executed by AI if the executing AI agent has a value. |
| **DR-34 Is Executed by Human** | A workflow step is considered an executed by human if the executing human agent has a value. |
| **DR-35 Approval Consistency Violation** | A workflow step is flagged approval consistency violation if all of the following hold: the requires human approval flag is set and the executing human agent is blank. |
| **DR-36 Approval is Human Filled** | A workflow step is flagged approval is human filled if the executing human agent has a value if the requires human approval flag is set, otherwise the TRUE. |
| **DR-37 Owning Department** | A workflow step's owning department is the owned by of the workflow step's assigned role. |
| **DR-38 Is Legal Owned** | A workflow step is considered legal owned if the owning department is the literal “ntwf-legal-dept”. |
| **DR-39 Is Engineering Owned** | A workflow step is considered engineering owned if the owning department is the literal “ntwf-engineering”. |
| **DR-40 Parent Path** | An approval gate's parent path is the relative path of the approval gate's workflow step. |
| **DR-41 Relative Path** | An approval gate's relative path is computed as the parent path, followed by the literal “/approval-gates/”, followed by the approval gate ID. |
| **DR-42 Iri** | An approval gate's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-43 Name** | An approval gate's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-44 Gate Role** | An approval gate's gate role is the assigned role of the approval gate's workflow step. |
| **DR-45 Gate Approver Human** | An approval gate's gate approver human is the filled by human agent of the approval gate's gate role. |
| **DR-46 Parent Path** | A step precedence's parent path is the relative path of the step precedence's from step. |
| **DR-47 Relative Path** | A step precedence's relative path is computed as the parent path, followed by the literal “/precedence/”, followed by the step precedence ID. |
| **DR-48 Iri** | A step precedence's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-49 Name** | A step precedence's name is computed as the from step, followed by the literal “ -> ”, followed by the to step. |
| **DR-50 Relative Path** | A role's relative path is computed as the literal “roles/”, followed by the role ID. |
| **DR-51 Iri** | A role's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-52 Name** | A role's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-53 Filled by Arm Count** | A role's filled by arm count is computed as the count of the following that hold: the filled by human agent has a value; the filled by AI agent has a value; and the filled by automated pipeline has a value. |
| **DR-54 Has Exactly One Filler** | A role is considered to have an exactly one filler if the filled by arm count is 1. |
| **DR-55 Filler Type** | The role's filler type is determined by the following priority:<br>1. the literal “HumanAgent”, if the filled by human agent has a value;<br>2. the literal “AIAgent”, if the filled by AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the filled by automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-56 Parent Path** | A role assignment's parent path is the relative path of the role assignment's role. |
| **DR-57 Relative Path** | A role assignment's relative path is computed as the parent path, followed by the literal “/assignments/”, followed by the role assignment ID. |
| **DR-58 Iri** | A role assignment's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-59 Name** | A role assignment's name is computed as the role, followed by the literal “ [”, followed by the valid from, followed by the literal “ -> ”, followed by the literal “open” if the valid to is blank, otherwise the valid to, followed by the literal “]”. |
| **DR-60 Filler Type** | The role assignment's filler type is determined by the following priority:<br>1. the literal “HumanAgent”, if the filled by human agent has a value;<br>2. the literal “AIAgent”, if the filled by AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the filled by automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-61 Is Current** | A role assignment is considered a current if the valid to is blank. |
| **DR-62 Was Active As of Audit Date** | A role assignment is considered to have been active as of audit date if all of the following hold: the valid from is at most the literal “2026-03-01” and at least one of the following holds: the valid to is blank or the valid to is greater than the literal “2026-03-01”. |
| **DR-63 Is Agent Type Change** | A role assignment is considered an agent type change if all of the following hold: the prior filler type has a value and the prior filler type is not the filler type. |
| **DR-64 Requires Compliance Audit** | A role assignment is considered to require compliance audit if all of the following hold: the prior filler type has a value; the prior filler type is the literal “AIAgent”; and the filler type is the literal “HumanAgent”. |
| **DR-65 Relative Path** | A department's relative path is computed as the literal “departments/”, followed by the department ID. |
| **DR-66 Iri** | A department's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-67 Name** | A department's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-68 Relative Path** | A human agent's relative path is computed as the literal “human-agents/”, followed by the human agent ID. |
| **DR-69 Iri** | A human agent's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-70 Relative Path** | An AI agent's relative path is computed as the literal “ai-agents/”, followed by the AI agent ID. |
| **DR-71 Iri** | An AI agent's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-72 Count Attributed Artifacts** | An AI agent's count attributed artifacts is the number of workflow artifacts related to the AI agent. |
| **DR-73 Count Impacted Workflows** | An AI agent's count impacted workflows is the number of the AI agent's workflow artifacts that have a producing workflow. |
| **DR-74 Relative Path** | An automated pipeline's relative path is computed as the literal “automated-pipelines/”, followed by the automated pipeline ID. |
| **DR-75 Iri** | An automated pipeline's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-76 Relative Path** | A workflow status concept's relative path is computed as the literal “concepts/workflow-status/”, followed by the concept ID. |
| **DR-77 Iri** | A workflow status concept's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-78 Relative Path** | An agent capability concept's relative path is computed as the literal “concepts/agent-capability/”, followed by the concept ID. |
| **DR-79 Iri** | An agent capability concept's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-80 Relative Path** | An artifact type concept's relative path is computed as the literal “concepts/artifact-type/”, followed by the concept ID. |
| **DR-81 Iri** | An artifact type concept's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-82 Relative Path** | A dataset's relative path is computed as the literal “datasets/”, followed by the dataset ID. |
| **DR-83 Iri** | A dataset's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-84 Parent Path** | A workflow artifact's parent path is the relative path of the workflow artifact's produced by step. |
| **DR-85 Relative Path** | A workflow artifact's relative path is computed as the parent path, followed by the literal “/artifacts/”, followed by the artifact ID. |
| **DR-86 Iri** | A workflow artifact's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-87 Producing Agent Type** | The workflow artifact's producing agent type is determined by the following priority:<br>1. the literal “HumanAgent”, if the attributed to human agent has a value;<br>2. the literal “AIAgent”, if the attributed to AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the attributed to automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-88 Has Derivation Parent** | A workflow artifact is considered to have a derivation parent if the derived from artifact has a value. |
| **DR-89 Produced by Workflow** | A workflow artifact's produced by workflow is the workflow of the workflow artifact's produced by step. |
| **DR-90 Has Producing Workflow** | A workflow artifact is considered to have a producing workflow if the produced by workflow has a value. |
| **DR-91 Relative Path** | A governance role's relative path is computed as the literal “governance-roles/”, followed by the governance role ID. |
| **DR-92 Iri** | A governance role's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-93 Name** | A governance role's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-94 Can Approve Changes** | A governance role is considered able to approve changes if the kind is the literal “Authority”. |
| **DR-95 Relative Path** | A change log's relative path is computed as the literal “change-log/”, followed by the change log ID. |
| **DR-96 Iri** | A change log's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-97 Name** | A change log's name is computed as the version, followed by the literal “ (”, followed by the change date, followed by the literal “)”. |
| **DR-98 Is Breaking Change** | A change log is considered a breaking change if the change kind is the literal “major”. |
| **DR-99 Is Backward Compatible** | A change log is considered a backward compatible if at least one of the following holds: the change kind is the literal “patch” or the change kind is the literal “minor”. |
| **DR-100 Relative Path** | A vocabulary reconciliation's relative path is computed as the literal “reconciliations/”, followed by the reconciliation ID. |
| **DR-101 Iri** | A vocabulary reconciliation's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-102 Name** | A vocabulary reconciliation's name is computed as the deprecated term, followed by the literal “ owl:sameAs ”, followed by the replacement term. |
| **DR-103 Relative Path** | A scenario's relative path is computed as the literal “scenarios/”, followed by the scenario ID. |
| **DR-104 Iri** | A scenario's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-105 Name** | A scenario's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-106 Relative Path** | A competency question's relative path is computed as the literal “competency-questions/”, followed by the competency question ID. |
| **DR-107 Iri** | A competency question's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-108 Name** | A competency question's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-109 Relative Path** | A conformance test's relative path is computed as the literal “conformance-tests/”, followed by the conformance test ID. |
| **DR-110 Iri** | A conformance test's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-111 Name** | A conformance test's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **Workflows.RelativePath** | formula | `"workflows/" & WorkflowId` |
| **Workflows.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Workflows.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **Workflows.CountOfNonProposedSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.HasMoreThan1Step** | formula | `CountOfNonProposedSteps > 1` |
| **Workflows.CountAISteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.CountHumanSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.CountHumanRequiredSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.CountApprovalConsistencyViolations** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.HasConsistencyViolation** | formula | `CountApprovalConsistencyViolations > 0` |
| **Workflows.HasAIAgentStep** | formula | `CountAISteps > 0` |
| **Workflows.MonthsSinceModified** | formula | `DaysBetween(Now(), Modified)` |
| **Workflows.IsStale** | formula | `MonthsSinceModified > StalenessThresholdMonths` |
| **Workflows.IsStaleAndHasAIAgent** | formula | `And(IsStale, HasAIAgentStep)` |
| **Workflows.CountDerivationLinks** | rollup | `Count(WorkflowArtifacts via ProducedByWorkflow)` |
| **Workflows.CountLegalOwnedSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.CountEngineeringOwnedSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.InvolvesEngineeringAndLegal** | formula | `And(CountEngineeringOwnedSteps > 0, CountLegalOwnedSteps > 0)` |
| **Workflows.CountInferredPrecedencePairs** | rollup | `Count(vw_step_precedence_closure via IsInferred)` |
| **Workflows.CountAssertedPrecedencePairs** | rollup | `Count(vw_step_precedence_closure via IsInferred)` |
| **Workflows.CountOfPrecedenceClosurePairs** | formula | `CountAssertedPrecedencePairs + CountInferredPrecedencePairs` |
| **Workflows.CountRolesWithBadFillerCardinality** | rollup | `Count(Roles via HasExactlyOneFiller)` |
| **Workflows.CountAgentTypeChanges** | rollup | `Count(RoleAssignments via IsAgentTypeChange)` |
| **Workflows.CountComplianceAuditChanges** | rollup | `Count(RoleAssignments via RequiresComplianceAudit)` |
| **WorkflowSteps.ParentPath** | lookup | `Lookup(Workflows.RelativePath via Workflow)` |
| **WorkflowSteps.RelativePath** | formula | `ParentPath & "/steps/" & WorkflowStepId` |
| **WorkflowSteps.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowSteps.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **WorkflowSteps.ExecutingHumanAgent** | lookup | `Lookup(Roles.FilledByHumanAgent via AssignedRole)` |
| **WorkflowSteps.ExecutingAIAgent** | lookup | `Lookup(Roles.FilledByAIAgent via AssignedRole)` |
| **WorkflowSteps.ExecutingAutomatedPipeline** | lookup | `Lookup(Roles.FilledByAutomatedPipeline via AssignedRole)` |
| **WorkflowSteps.ExecutingAgentType** | formula | `If(Not(Isblank(ExecutingHumanAgent)), "HumanAgent", If(Not(Isblank(ExecutingAIAgent)), "AIAgent", If(Not(Isblank(ExecutingAutomatedPipeline)), "AutomatedPipeline", "")))` |
| **WorkflowSteps.IsExecutedByAI** | formula | `Not(Isblank(ExecutingAIAgent))` |
| **WorkflowSteps.IsExecutedByHuman** | formula | `Not(Isblank(ExecutingHumanAgent))` |
| **WorkflowSteps.ApprovalConsistencyViolation** | formula | `And(RequiresHumanApproval, Isblank(ExecutingHumanAgent))` |
| **WorkflowSteps.ApprovalIsHumanFilled** | formula | `If(RequiresHumanApproval, Not(Isblank(ExecutingHumanAgent)), TRUE)` |
| **WorkflowSteps.OwningDepartment** | lookup | `Lookup(Roles.OwnedBy via AssignedRole)` |
| **WorkflowSteps.IsLegalOwned** | formula | `OwningDepartment = "ntwf-legal-dept"` |
| **WorkflowSteps.IsEngineeringOwned** | formula | `OwningDepartment = "ntwf-engineering"` |
| **ApprovalGates.ParentPath** | lookup | `Lookup(WorkflowSteps.RelativePath via WorkflowStep)` |
| **ApprovalGates.RelativePath** | formula | `ParentPath & "/approval-gates/" & ApprovalGateId` |
| **ApprovalGates.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ApprovalGates.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **ApprovalGates.GateRole** | lookup | `Lookup(WorkflowSteps.AssignedRole via WorkflowStep)` |
| **ApprovalGates.GateApproverHuman** | lookup | `Lookup(Roles.FilledByHumanAgent via GateRole)` |
| **StepPrecedence.ParentPath** | lookup | `Lookup(WorkflowSteps.RelativePath via FromStep)` |
| **StepPrecedence.RelativePath** | formula | `ParentPath & "/precedence/" & StepPrecedenceId` |
| **StepPrecedence.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **StepPrecedence.Name** | formula | `FromStep & " -> " & ToStep` |
| **Roles.RelativePath** | formula | `"roles/" & RoleId` |
| **Roles.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Roles.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **Roles.FilledByArmCount** | formula | `If(Not(Isblank(FilledByHumanAgent)), 1, 0) + If(Not(Isblank(FilledByAIAgent)), 1, 0) + If(Not(Isblank(FilledByAutomatedPipeline)), 1, 0)` |
| **Roles.HasExactlyOneFiller** | formula | `FilledByArmCount = 1` |
| **Roles.FillerType** | formula | `If(Not(Isblank(FilledByHumanAgent)), "HumanAgent", If(Not(Isblank(FilledByAIAgent)), "AIAgent", If(Not(Isblank(FilledByAutomatedPipeline)), "AutomatedPipeline", "")))` |
| **RoleAssignments.ParentPath** | lookup | `Lookup(Roles.RelativePath via Role)` |
| **RoleAssignments.RelativePath** | formula | `ParentPath & "/assignments/" & RoleAssignmentId` |
| **RoleAssignments.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **RoleAssignments.Name** | formula | `Role & " [" & ValidFrom & " -> " & If(Isblank(ValidTo), "open", ValidTo) & "]"` |
| **RoleAssignments.FillerType** | formula | `If(Not(Isblank(FilledByHumanAgent)), "HumanAgent", If(Not(Isblank(FilledByAIAgent)), "AIAgent", If(Not(Isblank(FilledByAutomatedPipeline)), "AutomatedPipeline", "")))` |
| **RoleAssignments.IsCurrent** | formula | `Isblank(ValidTo)` |
| **RoleAssignments.WasActiveAsOfAuditDate** | formula | `And(ValidFrom <= "2026-03-01", Or(Isblank(ValidTo), ValidTo > "2026-03-01"))` |
| **RoleAssignments.IsAgentTypeChange** | formula | `And(Not(Isblank(PriorFillerType)), PriorFillerType <> FillerType)` |
| **RoleAssignments.RequiresComplianceAudit** | formula | `And(Not(Isblank(PriorFillerType)), PriorFillerType = "AIAgent", FillerType = "HumanAgent")` |
| **Departments.RelativePath** | formula | `"departments/" & DepartmentId` |
| **Departments.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Departments.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **HumanAgents.RelativePath** | formula | `"human-agents/" & HumanAgentId` |
| **HumanAgents.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **AIAgents.RelativePath** | formula | `"ai-agents/" & AIAgentId` |
| **AIAgents.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **AIAgents.CountAttributedArtifacts** | rollup | `Count(WorkflowArtifacts via AttributedToAIAgent)` |
| **AIAgents.CountImpactedWorkflows** | rollup | `Count(WorkflowArtifacts via AttributedToAIAgent)` |
| **AutomatedPipelines.RelativePath** | formula | `"automated-pipelines/" & AutomatedPipelineId` |
| **AutomatedPipelines.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowStatusConcepts.RelativePath** | formula | `"concepts/workflow-status/" & ConceptId` |
| **WorkflowStatusConcepts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **AgentCapabilityConcepts.RelativePath** | formula | `"concepts/agent-capability/" & ConceptId` |
| **AgentCapabilityConcepts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ArtifactTypeConcepts.RelativePath** | formula | `"concepts/artifact-type/" & ConceptId` |
| **ArtifactTypeConcepts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Datasets.RelativePath** | formula | `"datasets/" & DatasetId` |
| **Datasets.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowArtifacts.ParentPath** | lookup | `Lookup(WorkflowSteps.RelativePath via ProducedByStep)` |
| **WorkflowArtifacts.RelativePath** | formula | `ParentPath & "/artifacts/" & ArtifactId` |
| **WorkflowArtifacts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowArtifacts.ProducingAgentType** | formula | `If(Not(Isblank(AttributedToHumanAgent)), "HumanAgent", If(Not(Isblank(AttributedToAIAgent)), "AIAgent", If(Not(Isblank(AttributedToAutomatedPipeline)), "AutomatedPipeline", "")))` |
| **WorkflowArtifacts.HasDerivationParent** | formula | `Not(Isblank(DerivedFromArtifact))` |
| **WorkflowArtifacts.ProducedByWorkflow** | lookup | `Lookup(WorkflowSteps.Workflow via ProducedByStep)` |
| **WorkflowArtifacts.HasProducingWorkflow** | formula | `Not(Isblank(ProducedByWorkflow))` |
| **GovernanceRoles.RelativePath** | formula | `"governance-roles/" & GovernanceRoleId` |
| **GovernanceRoles.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **GovernanceRoles.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **GovernanceRoles.CanApproveChanges** | formula | `Kind = "Authority"` |
| **ChangeLog.RelativePath** | formula | `"change-log/" & ChangeLogId` |
| **ChangeLog.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ChangeLog.Name** | formula | `Version & " (" & ChangeDate & ")"` |
| **ChangeLog.IsBreakingChange** | formula | `ChangeKind = "major"` |
| **ChangeLog.IsBackwardCompatible** | formula | `Or(ChangeKind = "patch", ChangeKind = "minor")` |
| **VocabularyReconciliations.RelativePath** | formula | `"reconciliations/" & ReconciliationId` |
| **VocabularyReconciliations.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **VocabularyReconciliations.Name** | formula | `DeprecatedTerm & " owl:sameAs " & ReplacementTerm` |
| **Scenarios.RelativePath** | formula | `"scenarios/" & ScenarioId` |
| **Scenarios.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Scenarios.Name** | formula | `Replace(Lower(Label), " ", "-")` |
| **CompetencyQuestions.RelativePath** | formula | `"competency-questions/" & CompetencyQuestionId` |
| **CompetencyQuestions.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **CompetencyQuestions.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **ConformanceTests.RelativePath** | formula | `"conformance-tests/" & ConformanceTestId` |
| **ConformanceTests.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ConformanceTests.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
