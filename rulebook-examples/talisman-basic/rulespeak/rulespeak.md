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
| Is Stale | True when the months since modified is greater than the staleness threshold months. | _TRUE iff the workflow's compliance documentation has not been reviewed (Modified) within its policy window — i.e. MonthsSinceModified exceeds the configurable StalenessThresholdMonths. The metadata half of CQ5 ('which workflows haven't been reviewed in twelve months'): with the default threshold of 12 this is exactly the article's question, but the threshold is now a fact you can change, so the staleness verdict tracks the org's actual review cadence rather than a hardcoded constant._ |
| Is Stale and Has AI Agent | True when all of the following hold: the is stale flag is set and the has AI agent step flag is set. | _The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column._ |
| Count Total Plan Minutes | The total step duration minutes across the workflow steps related to the workflow. | _Total planned runtime of the workflow: the sum of StepDurationMinutes over all of its steps. The live consumer of the per-step duration literal. EXTENSION beyond the source article (which records stepDurationMinutes but never sums it). Compared against MaxPlanMinutes to derive IsOverTimeBudget._ |
| Is Over Time Budget | True when the count total plan minutes is greater than the max plan minutes. | _TRUE iff the workflow's total planned runtime (CountTotalPlanMinutes) exceeds its configured budget (MaxPlanMinutes). EXTENSION beyond the source article — a third compliance input alongside staleness and AI-execution, giving the step-duration literal a visible derived consequence. Editing any step's StepDurationMinutes recomputes the sum and can flip this boolean, which folds into IsAtComplianceRisk._ |
| Count Unmet Gate Signoffs | The number of the workflow's workflow steps that are gate signoff unmet. | _Number of this workflow's steps whose approval gate is NOT satisfied (an off-hours run where the gate requires dual sign-off but two human approvers are not available along the delegation chain). Rollup over WorkflowSteps.GateSignoffUnmet. A clean run holds this at 0._ |
| Has Unmet Gate Signoff | True when the count unmet gate signoffs is greater than 0. | _TRUE iff any approval gate in this workflow is unsatisfied (CountUnmetGateSignoffs > 0). The gate's contribution to the compliance verdict: a required off-hours dual-signoff gate without a second human approver is itself a compliance risk. Folds into IsAtComplianceRisk, so toggling a gate's dual-signoff policy, flagging the run off-hours, or breaking the delegation chain can flip the verdict._ |
| Count Derivation Links | The number of the workflow's workflow artifacts that have a derivation parent. | _Number of prov:wasDerivedFrom links among this workflow's artifacts (rollup over WorkflowArtifacts.HasDerivationParent). Answers the lineage half of CQ4: 5 artifacts form a chain with 4 derivation links._ |
| Count Legal Owned Steps | The number of the workflow's workflow steps that are legal owned. | _Number of steps in this workflow whose owning department is Legal (rollup over WorkflowSteps.IsLegalOwned). CQ7: exactly one Legal-owned step in the Production Deployment workflow._ |
| Count Engineering Owned Steps | The number of the workflow's workflow steps that are engineering owned. | _Number of steps whose owning department is Engineering (rollup over WorkflowSteps.IsEngineeringOwned). Feeds CQ7's Engineering-involvement check._ |
| Involves Engineering and Legal | True when all of the following hold: the count engineering owned steps is greater than 0 and the count legal owned steps is greater than 0. | _TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step)._ |
| Count Inferred Precedence Pairs | The number of vw step precedence closure related to the workflow. | _Number of step-ordering pairs that the transitive closure of ntwf:precedesStep INFERRED (rollup over the closure view vw_step_precedence_closure where is_inferred = TRUE). The article's signature count: 6 of the 10 closure pairs were never asserted — including step-1 -> step-5. NOTE: this single-workflow model has exactly one Workflow, so the global closure view is wholly this workflow's; the COUNTIFS is unfiltered because every precedence edge belongs to the Production Deployment DAG._ |
| Count Asserted Precedence Pairs | The number of vw step precedence closure related to the workflow. | _Number of step-ordering pairs that were directly ASSERTED as ntwf:precedesStep edges (rollup over vw_step_precedence_closure where is_inferred = FALSE) — the hop-1 rows. The article's 4 asserted edges. Together with CountInferredPrecedencePairs (6) this sums to the 10-pair closure, making CountOfPrecedenceClosurePairs an honest asserted+inferred total rather than an unconditional count. Single-workflow note as on CountInferredPrecedencePairs: the global closure view is this workflow's._ |
| Count of Precedence Closure Pairs | Computed as the count asserted precedence pairs plus the count inferred precedence pairs. | _Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them._ |
| Count Roles With Bad Filler Cardinality | The number of roles related to the workflow. | _Number of roles that do NOT have exactly one filledBy arm set (rollup over Roles.HasExactlyOneFiller = FALSE). The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has 0 such roles — this is the Suite-1 functional/disjointness witness as a single integer. A non-zero value is the relational signal of the Suite-4 disjointness violation (a role filled by two agent classes, or by none). NOTE: this single-workflow model has exactly one Workflow and every Role participates in it, so the count is over all roles; a multi-workflow model would scope it through a role→workflow path._ |
| **Workflow Step** | A workflow step is identified by its name and is related to optionally a workflow, optionally a role (its assigned role), optionally a dataset (its consumes dataset), optionally a workflow artifact (its produces artifacts), optionally an approval gate, optionally a step precedence (its precedes), and optionally a step precedence (its preceded by). | — |
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
| Is Off Hours Deployment | True when the workflow step's workflow is off hours deployment. | _Whether this step's workflow is flagged as an off-hours deployment (pulled from Workflows.IsOffHoursDeployment). Brought down to the step so an ApprovalGate specializing this step can read the off-hours bit for its dual-signoff rule._ |
| Gate Dual Signoff Satisfied | True when the workflow step's approval gate is dual signoff satisfied. | _The DualSignoffSatisfied verdict of the approval gate specializing this step, if any (gate → ApprovalGates.DualSignoffSatisfied). Blank for ordinary steps that have no gate. The bridge that brings the gate's verdict up to the step so it can be rolled up to the workflow._ |
| Gate Signoff Unmet | True when all of the following hold: the approval gate has a value and it is not the case that the gate dual signoff satisfied flag is set. | _TRUE iff this step has an approval gate AND that gate's dual-signoff policy is NOT satisfied. FALSE for steps with no gate, and for gates that are satisfied. Counted by Workflows.CountUnmetGateSignoffs to fold the gate into the compliance verdict._ |
| **Approval Gate** | An approval gate is identified by its name and is related to optionally a workflow step. | — |
| Parent Path | The relative path of the approval gate's workflow step. | _Helper: the WorkflowSteps parent's RelativePath, pulled across the WorkflowStep FK. Exists so RelativePath can concatenate the '/approval-gates/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)._ |
| Relative Path | Computed as the parent path, followed by the literal “/approval-gates/”, followed by the approval gate ID. | _Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | — |
| Gate Role | The assigned role of the approval gate's workflow step. | _The role responsible for this gate's underlying step, resolved through WorkflowStep → WorkflowSteps.AssignedRole. First hop of the CQ2 chain (gate → role → approver)._ |
| Gate Approver Human | The filled by human agent of the approval gate's gate role. | _The human agent who approves at this gate, resolved through the two-hop chain gate → GateRole → Roles.FilledByHumanAgent. Answers CQ2 ('who is responsible for approving a production deployment') directly: the release-approval gate resolves to the Release Manager role, filled by Maria Gonzalez._ |
| Is Off Hours Deployment | True when the approval gate's workflow step is off hours deployment. | _Whether the workflow this gate belongs to is flagged as an off-hours deployment. Pulled from Workflows.IsOffHoursDeployment via the gate's step → workflow. The off-hours bit is an editable workflow fact; this lookup brings it down to the gate so the dual-signoff rule can combine the two toggles._ |
| Gate Delegate Role | The delegates to of the approval gate's gate role. | _The role the gate's role escalates to (gate → GateRole → Roles.DelegatesTo). The second approver in the dual-signoff rule comes from here. Ties the gate directly to the delegation/escalation hierarchy._ |
| Gate Delegate Human | The filled by human agent of the approval gate's gate delegate role. | _The human agent filling the gate's delegate role (gate → GateDelegateRole → Roles.FilledByHumanAgent). Empty when the delegate role is unfilled or filled by a non-human — which is exactly the condition that makes an off-hours dual-signoff gate fail._ |
| Has Two Human Approvers | True when all of the following hold: the gate approver human has a value and the gate delegate human has a value. | _TRUE when BOTH the gate's role and its delegate role are filled by (non-empty) human agents. The structural precondition for a second sign-off: two distinct humans are available along the escalation chain._ |
| Dual Signoff Satisfied | True when the has two human approvers if all of the following hold: the requires dual signoff off hours flag is set and the is off hours deployment flag is set, otherwise the TRUE. | _The gate's verdict witness. TRUE unless the gate requires dual sign-off AND the run is off-hours AND two human approvers are NOT available along the delegation chain. In other words: a required off-hours dual-signoff gate is satisfied only when both the gate role and its delegate are human-filled. Drives Workflows.HasUnmetGateSignoff and, through it, the compliance verdict._ |
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
| **Department** | A department is identified by its name. | — |
| Relative Path | Computed as the literal “departments/”, followed by the department ID. | _Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Human-readable display name of the department. Should match organizational terminology for stakeholder communication._ |
| **Human Agent** | A human agent is identified by its name. | — |
| Relative Path | Computed as the literal “human-agents/”, followed by the human agent ID. | _Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **AI Agent** | An AI agent is identified by its name. | — |
| Relative Path | Computed as the literal “ai-agents/”, followed by the AI agent ID. | _Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Automated Pipeline** | An automated pipeline is identified by its name. | — |
| Relative Path | Computed as the literal “automated-pipelines/”, followed by the automated pipeline ID. | _Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Workflow Status Concept** | SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows. | — |
| Relative Path | Computed as the literal “concepts/workflow-status/”, followed by the concept ID. | _Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| **Compliance Verdict Concept** | SKOS controlled vocabulary for the two possible compliance-verdict outcomes (ntwf:ComplianceVerdictScheme). Part of the CBox. ComplianceVerdicts.VerdictConcept is a derived FK that resolves to exactly one of these concepts based on IsAtComplianceRisk; the human-readable Verdict string is then a lookup of the chosen concept's PrefLabel. Promoting the verdict outcomes from inline string literals to a first-class option set makes the choice auditable and lets the RuleSpeak narrative cite the defined options instead of magic strings. | — |
| Relative Path | Computed as the literal “concepts/compliance-verdict/”, followed by the concept ID. | _Stable, DAG-derived location for this ComplianceVerdictConcept row. Root segment 'concepts/compliance-verdict' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique._ |
| **Agent Capability Concept** | SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox. | — |
| Relative Path | Computed as the literal “concepts/agent-capability/”, followed by the concept ID. | _Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
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
| **Compliance Verdict** | A compliance verdict is identified by its name and is related to a workflow and optionally a compliance verdict concept (its verdict concept). | — |
| Parent Path | The relative path of the compliance verdict's workflow. | _Helper: the Workflows parent's RelativePath, pulled across the Workflow FK. Exists so RelativePath can concatenate the '/verdicts/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat)._ |
| Relative Path | Computed as the parent path, followed by the literal “/verdicts/”, followed by the compliance verdict ID. | _Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/verdicts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions._ |
| Name | Computed as the lower-cased workflow title with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Machine-friendly slug, mirrors the workflow._ |
| Workflow Title | The title of the compliance verdict's workflow. | _The workflow's dct:title, for human-readable display._ |
| Months Since Review | The months since modified of the compliance verdict's workflow. | _Whole months since the workflow's dct:modified, pulled from Workflows.MonthsSinceModified. The metadata layer of the verdict._ |
| Is Stale | True when the compliance verdict's workflow is a stale. | _Whether the workflow has not been reviewed in twelve months (dct:modified > 12mo). Pulled from Workflows.IsStale. The article's CQ5 condition._ |
| AI Step Count | The count AI steps of the compliance verdict's workflow. | _How many steps an AI agent executes in this workflow, pulled from Workflows.CountAISteps. The accountability layer._ |
| Has AI Executed Step | True when the compliance verdict's workflow has an AI agent step. | _Whether at least one step is executed by an AIAgent (filledBy -> AIAgent). Pulled from Workflows.HasAIAgentStep._ |
| Total Plan Minutes | The count total plan minutes of the compliance verdict's workflow. | _The workflow's total planned runtime (sum of step durations), pulled from Workflows.CountTotalPlanMinutes. EXTENSION beyond the source article. The time layer of the verdict._ |
| Time Budget Minutes | The max plan minutes of the compliance verdict's workflow. | _The workflow's configured runtime budget, pulled from Workflows.MaxPlanMinutes. EXTENSION beyond the source article._ |
| Is Over Time Budget | True when the compliance verdict's workflow is an over time budget. | _Whether total planned runtime exceeds the workflow's budget. Pulled from Workflows.IsOverTimeBudget. EXTENSION beyond the source article — the third compliance input alongside staleness and AI-execution._ |
| Has Consistency Violation | True when the compliance verdict's workflow has a consistency violation. | _Whether any step breaks the human-approval consistency rule. Pulled from Workflows.HasConsistencyViolation. This is the 'no broken rules' input to the verdict: a structural inconsistency (an approval step not filled by a human) is itself a compliance risk, independent of staleness or time budget._ |
| Has Unmet Gate Signoff | True when the compliance verdict's workflow has an unmet gate signoff. | _Whether any approval gate in this workflow is unsatisfied. Pulled from Workflows.HasUnmetGateSignoff. The gate's input to the verdict: an off-hours run through a gate that requires dual sign-off, without a second human approver available along the delegation chain, is a compliance risk in its own right. This is what gives the ApprovalGate subtype a measurable consequence._ |
| Is At Compliance Risk | True when at least one of the following holds: all of the following hold: the is stale flag is set and the has AI executed step flag is set; the is over time budget flag is set; the has consistency violation flag is set; or the has unmet gate signoff flag is set. | _THE VERDICT. True when the workflow is (stale AND has an AI agent executing a step) — the article's closing business question — OR over its planned-runtime budget OR has any human-approval consistency violation OR has any unmet approval-gate sign-off. The first disjunct is the article's headline finale verbatim; the second gives the step-duration literal a live compliance consequence; the third enforces an implicit no-broken-rules rule; the fourth gives the approval gate its first measurable consequence (a required off-hours dual sign-off with no second human approver). A workflow with any of these can never read COMPLIANT. Fully derived: flip the Modified date, the role's filledBy agent, any step's duration, the off-hours flag, or the gate's dual-signoff policy, and this recomputes on the next read._ |
| Verdict | The pref label of the compliance verdict's verdict concept. | _Human-readable rendering of the verdict for stakeholders — the PrefLabel of the resolved ComplianceVerdictConcepts option (VerdictConcept). No longer an inline string literal: the two possible values are defined rows in the ComplianceVerdictConcepts vocabulary, and this field looks up whichever one VerdictConcept selected._ |
| **Scenario** | A scenario is identified by its name. | — |
| Relative Path | Computed as the literal “scenarios/”, followed by the scenario ID. | _DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key._ |
| Iri | Computed as the relative path with every a slash replaced by a hyphen. | _Opaque stable identifier (dash-form of RelativePath)._ |
| Name | Computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug form of the human label._ |

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
- a **dataset** may reference one **workflow step**
- a **workflow artifact** may reference one **workflow step**
- a **workflow artifact** may reference one **workflow artifact**
- a **workflow artifact** may reference one **human agent**
- a **workflow artifact** may reference one **AI agent**
- a **workflow artifact** may reference one **automated pipeline**
- a **compliance verdict** references exactly one **workflow**
- a **compliance verdict** may reference one **compliance verdict concept**

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
- A workflow status concept **must** have a pref label.
- A compliance verdict concept **must** have a pref label.
- An agent capability concept **must** have a pref label.
- A dataset **must** have a title.
- A workflow artifact **must** have a title.
- A compliance verdict **must** reference exactly one workflow.
- A scenario **must** have a label and an edits.

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
| **DR-15 Count Total Plan Minutes** | A workflow's count total plan minutes is the total step duration minutes across the workflow steps related to the workflow. |
| **DR-16 Is Over Time Budget** | A workflow is considered an over time budget if the count total plan minutes is greater than the max plan minutes. |
| **DR-17 Count Unmet Gate Signoffs** | A workflow's count unmet gate signoffs is the number of the workflow's workflow steps that are gate signoff unmet. |
| **DR-18 Has Unmet Gate Signoff** | A workflow is considered to have an unmet gate signoff if the count unmet gate signoffs is greater than 0. |
| **DR-19 Count Derivation Links** | A workflow's count derivation links is the number of the workflow's workflow artifacts that have a derivation parent. |
| **DR-20 Count Legal Owned Steps** | A workflow's count legal owned steps is the number of the workflow's workflow steps that are legal owned. |
| **DR-21 Count Engineering Owned Steps** | A workflow's count engineering owned steps is the number of the workflow's workflow steps that are engineering owned. |
| **DR-22 Involves Engineering and Legal** | A workflow is considered to involve engineering and legal if all of the following hold: the count engineering owned steps is greater than 0 and the count legal owned steps is greater than 0. |
| **DR-23 Count Inferred Precedence Pairs** | A workflow's count inferred precedence pairs is the number of vw step precedence closure related to the workflow. |
| **DR-24 Count Asserted Precedence Pairs** | A workflow's count asserted precedence pairs is the number of vw step precedence closure related to the workflow. |
| **DR-25 Count of Precedence Closure Pairs** | A workflow's count of precedence closure pairs is computed as the count asserted precedence pairs plus the count inferred precedence pairs. |
| **DR-26 Count Roles With Bad Filler Cardinality** | A workflow's count roles with bad filler cardinality is the number of roles related to the workflow. |
| **DR-27 Parent Path** | A workflow step's parent path is the relative path of the workflow step's workflow. |
| **DR-28 Relative Path** | A workflow step's relative path is computed as the parent path, followed by the literal “/steps/”, followed by the workflow step ID. |
| **DR-29 Iri** | A workflow step's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-30 Name** | A workflow step's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-31 Executing Human Agent** | A workflow step's executing human agent is the filled by human agent of the workflow step's assigned role. |
| **DR-32 Executing AI Agent** | A workflow step's executing AI agent is the filled by AI agent of the workflow step's assigned role. |
| **DR-33 Executing Automated Pipeline** | A workflow step's executing automated pipeline is the filled by automated pipeline of the workflow step's assigned role. |
| **DR-34 Executing Agent Type** | The workflow step's executing agent type is determined by the following priority:<br>1. the literal “HumanAgent”, if the executing human agent has a value;<br>2. the literal “AIAgent”, if the executing AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the executing automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-35 Is Executed by AI** | A workflow step is considered an executed by AI if the executing AI agent has a value. |
| **DR-36 Is Executed by Human** | A workflow step is considered an executed by human if the executing human agent has a value. |
| **DR-37 Approval Consistency Violation** | A workflow step is flagged approval consistency violation if all of the following hold: the requires human approval flag is set and the executing human agent is blank. |
| **DR-38 Approval is Human Filled** | A workflow step is flagged approval is human filled if the executing human agent has a value if the requires human approval flag is set, otherwise the TRUE. |
| **DR-39 Owning Department** | A workflow step's owning department is the owned by of the workflow step's assigned role. |
| **DR-40 Is Legal Owned** | A workflow step is considered legal owned if the owning department is the literal “ntwf-legal-dept”. |
| **DR-41 Is Engineering Owned** | A workflow step is considered engineering owned if the owning department is the literal “ntwf-engineering”. |
| **DR-42 Is Off Hours Deployment** | A workflow step's is off hours deployment is true when the workflow step's workflow is off hours deployment. |
| **DR-43 Gate Dual Signoff Satisfied** | A workflow step's gate dual signoff satisfied is true when the workflow step's approval gate is dual signoff satisfied. |
| **DR-44 Gate Signoff Unmet** | A workflow step is flagged gate signoff unmet if all of the following hold: the approval gate has a value and it is not the case that the gate dual signoff satisfied flag is set. |
| **DR-45 Parent Path** | An approval gate's parent path is the relative path of the approval gate's workflow step. |
| **DR-46 Relative Path** | An approval gate's relative path is computed as the parent path, followed by the literal “/approval-gates/”, followed by the approval gate ID. |
| **DR-47 Iri** | An approval gate's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-48 Name** | An approval gate's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-49 Gate Role** | An approval gate's gate role is the assigned role of the approval gate's workflow step. |
| **DR-50 Gate Approver Human** | An approval gate's gate approver human is the filled by human agent of the approval gate's gate role. |
| **DR-51 Is Off Hours Deployment** | An approval gate's is off hours deployment is true when the approval gate's workflow step is off hours deployment. |
| **DR-52 Gate Delegate Role** | An approval gate's gate delegate role is the delegates to of the approval gate's gate role. |
| **DR-53 Gate Delegate Human** | An approval gate's gate delegate human is the filled by human agent of the approval gate's gate delegate role. |
| **DR-54 Has Two Human Approvers** | An approval gate is considered to have a two human approvers if all of the following hold: the gate approver human has a value and the gate delegate human has a value. |
| **DR-55 Dual Signoff Satisfied** | An approval gate is flagged dual signoff satisfied if the has two human approvers if all of the following hold: the requires dual signoff off hours flag is set and the is off hours deployment flag is set, otherwise the TRUE. |
| **DR-56 Parent Path** | A step precedence's parent path is the relative path of the step precedence's from step. |
| **DR-57 Relative Path** | A step precedence's relative path is computed as the parent path, followed by the literal “/precedence/”, followed by the step precedence ID. |
| **DR-58 Iri** | A step precedence's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-59 Name** | A step precedence's name is computed as the from step, followed by the literal “ -> ”, followed by the to step. |
| **DR-60 Relative Path** | A role's relative path is computed as the literal “roles/”, followed by the role ID. |
| **DR-61 Iri** | A role's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-62 Name** | A role's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-63 Filled by Arm Count** | A role's filled by arm count is computed as the count of the following that hold: the filled by human agent has a value; the filled by AI agent has a value; and the filled by automated pipeline has a value. |
| **DR-64 Has Exactly One Filler** | A role is considered to have an exactly one filler if the filled by arm count is 1. |
| **DR-65 Filler Type** | The role's filler type is determined by the following priority:<br>1. the literal “HumanAgent”, if the filled by human agent has a value;<br>2. the literal “AIAgent”, if the filled by AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the filled by automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-66 Relative Path** | A department's relative path is computed as the literal “departments/”, followed by the department ID. |
| **DR-67 Iri** | A department's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-68 Name** | A department's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-69 Relative Path** | A human agent's relative path is computed as the literal “human-agents/”, followed by the human agent ID. |
| **DR-70 Iri** | A human agent's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-71 Relative Path** | An AI agent's relative path is computed as the literal “ai-agents/”, followed by the AI agent ID. |
| **DR-72 Iri** | An AI agent's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-73 Relative Path** | An automated pipeline's relative path is computed as the literal “automated-pipelines/”, followed by the automated pipeline ID. |
| **DR-74 Iri** | An automated pipeline's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-75 Relative Path** | A workflow status concept's relative path is computed as the literal “concepts/workflow-status/”, followed by the concept ID. |
| **DR-76 Iri** | A workflow status concept's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-77 Relative Path** | A compliance verdict concept's relative path is computed as the literal “concepts/compliance-verdict/”, followed by the concept ID. |
| **DR-78 Iri** | A compliance verdict concept's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-79 Relative Path** | An agent capability concept's relative path is computed as the literal “concepts/agent-capability/”, followed by the concept ID. |
| **DR-80 Iri** | An agent capability concept's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-81 Relative Path** | A dataset's relative path is computed as the literal “datasets/”, followed by the dataset ID. |
| **DR-82 Iri** | A dataset's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-83 Parent Path** | A workflow artifact's parent path is the relative path of the workflow artifact's produced by step. |
| **DR-84 Relative Path** | A workflow artifact's relative path is computed as the parent path, followed by the literal “/artifacts/”, followed by the artifact ID. |
| **DR-85 Iri** | A workflow artifact's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-86 Producing Agent Type** | The workflow artifact's producing agent type is determined by the following priority:<br>1. the literal “HumanAgent”, if the attributed to human agent has a value;<br>2. the literal “AIAgent”, if the attributed to AI agent has a value;<br>3. the literal “AutomatedPipeline”, if the attributed to automated pipeline has a value;<br>4. otherwise an empty string. |
| **DR-87 Has Derivation Parent** | A workflow artifact is considered to have a derivation parent if the derived from artifact has a value. |
| **DR-88 Produced by Workflow** | A workflow artifact's produced by workflow is the workflow of the workflow artifact's produced by step. |
| **DR-89 Parent Path** | A compliance verdict's parent path is the relative path of the compliance verdict's workflow. |
| **DR-90 Relative Path** | A compliance verdict's relative path is computed as the parent path, followed by the literal “/verdicts/”, followed by the compliance verdict ID. |
| **DR-91 Iri** | A compliance verdict's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-92 Name** | A compliance verdict's name is computed as the lower-cased workflow title with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-93 Workflow Title** | A compliance verdict's workflow title is the title of the compliance verdict's workflow. |
| **DR-94 Months Since Review** | A compliance verdict's months since review is the months since modified of the compliance verdict's workflow. |
| **DR-95 Is Stale** | A compliance verdict's is stale is true when the compliance verdict's workflow is a stale. |
| **DR-96 AI Step Count** | A compliance verdict's AI step count is the count AI steps of the compliance verdict's workflow. |
| **DR-97 Has AI Executed Step** | A compliance verdict's has AI executed step is true when the compliance verdict's workflow has an AI agent step. |
| **DR-98 Total Plan Minutes** | A compliance verdict's total plan minutes is the count total plan minutes of the compliance verdict's workflow. |
| **DR-99 Time Budget Minutes** | A compliance verdict's time budget minutes is the max plan minutes of the compliance verdict's workflow. |
| **DR-100 Is Over Time Budget** | A compliance verdict's is over time budget is true when the compliance verdict's workflow is an over time budget. |
| **DR-101 Has Consistency Violation** | A compliance verdict's has consistency violation is true when the compliance verdict's workflow has a consistency violation. |
| **DR-102 Has Unmet Gate Signoff** | A compliance verdict's has unmet gate signoff is true when the compliance verdict's workflow has an unmet gate signoff. |
| **DR-103 Is At Compliance Risk** | A compliance verdict is considered at compliance risk if at least one of the following holds: all of the following hold: the is stale flag is set and the has AI executed step flag is set; the is over time budget flag is set; the has consistency violation flag is set; or the has unmet gate signoff flag is set. |
| **DR-104 Verdict** | A compliance verdict's verdict is the pref label of the compliance verdict's verdict concept. |
| **DR-105 Relative Path** | A scenario's relative path is computed as the literal “scenarios/”, followed by the scenario ID. |
| **DR-106 Iri** | A scenario's iri is computed as the relative path with every a slash replaced by a hyphen. |
| **DR-107 Name** | A scenario's name is computed as the lower-cased label with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |

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
| **Workflows.CountTotalPlanMinutes** | rollup | `Sum(WorkflowSteps.StepDurationMinutes via Workflow)` |
| **Workflows.IsOverTimeBudget** | formula | `CountTotalPlanMinutes > MaxPlanMinutes` |
| **Workflows.CountUnmetGateSignoffs** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.HasUnmetGateSignoff** | formula | `CountUnmetGateSignoffs > 0` |
| **Workflows.CountDerivationLinks** | rollup | `Count(WorkflowArtifacts via ProducedByWorkflow)` |
| **Workflows.CountLegalOwnedSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.CountEngineeringOwnedSteps** | rollup | `Count(WorkflowSteps via Workflow)` |
| **Workflows.InvolvesEngineeringAndLegal** | formula | `And(CountEngineeringOwnedSteps > 0, CountLegalOwnedSteps > 0)` |
| **Workflows.CountInferredPrecedencePairs** | rollup | `Count(vw_step_precedence_closure via IsInferred)` |
| **Workflows.CountAssertedPrecedencePairs** | rollup | `Count(vw_step_precedence_closure via IsInferred)` |
| **Workflows.CountOfPrecedenceClosurePairs** | formula | `CountAssertedPrecedencePairs + CountInferredPrecedencePairs` |
| **Workflows.CountRolesWithBadFillerCardinality** | rollup | `Count(Roles via HasExactlyOneFiller)` |
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
| **WorkflowSteps.IsOffHoursDeployment** | lookup | `Lookup(Workflows.IsOffHoursDeployment via Workflow)` |
| **WorkflowSteps.GateDualSignoffSatisfied** | lookup | `Lookup(ApprovalGates.DualSignoffSatisfied via ApprovalGate)` |
| **WorkflowSteps.GateSignoffUnmet** | formula | `And(Not(Isblank(ApprovalGate)), Not(GateDualSignoffSatisfied))` |
| **ApprovalGates.ParentPath** | lookup | `Lookup(WorkflowSteps.RelativePath via WorkflowStep)` |
| **ApprovalGates.RelativePath** | formula | `ParentPath & "/approval-gates/" & ApprovalGateId` |
| **ApprovalGates.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ApprovalGates.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **ApprovalGates.GateRole** | lookup | `Lookup(WorkflowSteps.AssignedRole via WorkflowStep)` |
| **ApprovalGates.GateApproverHuman** | lookup | `Lookup(Roles.FilledByHumanAgent via GateRole)` |
| **ApprovalGates.IsOffHoursDeployment** | lookup | `Lookup(WorkflowSteps.IsOffHoursDeployment via WorkflowStep)` |
| **ApprovalGates.GateDelegateRole** | lookup | `Lookup(Roles.DelegatesTo via GateRole)` |
| **ApprovalGates.GateDelegateHuman** | lookup | `Lookup(Roles.FilledByHumanAgent via GateDelegateRole)` |
| **ApprovalGates.HasTwoHumanApprovers** | formula | `And(Not(Isblank(GateApproverHuman)), Not(Isblank(GateDelegateHuman)))` |
| **ApprovalGates.DualSignoffSatisfied** | formula | `If(And(RequiresDualSignoffOffHours, IsOffHoursDeployment), HasTwoHumanApprovers, TRUE)` |
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
| **Departments.RelativePath** | formula | `"departments/" & DepartmentId` |
| **Departments.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Departments.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **HumanAgents.RelativePath** | formula | `"human-agents/" & HumanAgentId` |
| **HumanAgents.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **AIAgents.RelativePath** | formula | `"ai-agents/" & AIAgentId` |
| **AIAgents.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **AutomatedPipelines.RelativePath** | formula | `"automated-pipelines/" & AutomatedPipelineId` |
| **AutomatedPipelines.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowStatusConcepts.RelativePath** | formula | `"concepts/workflow-status/" & ConceptId` |
| **WorkflowStatusConcepts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ComplianceVerdictConcepts.RelativePath** | formula | `"concepts/compliance-verdict/" & ConceptId` |
| **ComplianceVerdictConcepts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **AgentCapabilityConcepts.RelativePath** | formula | `"concepts/agent-capability/" & ConceptId` |
| **AgentCapabilityConcepts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Datasets.RelativePath** | formula | `"datasets/" & DatasetId` |
| **Datasets.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowArtifacts.ParentPath** | lookup | `Lookup(WorkflowSteps.RelativePath via ProducedByStep)` |
| **WorkflowArtifacts.RelativePath** | formula | `ParentPath & "/artifacts/" & ArtifactId` |
| **WorkflowArtifacts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **WorkflowArtifacts.ProducingAgentType** | formula | `If(Not(Isblank(AttributedToHumanAgent)), "HumanAgent", If(Not(Isblank(AttributedToAIAgent)), "AIAgent", If(Not(Isblank(AttributedToAutomatedPipeline)), "AutomatedPipeline", "")))` |
| **WorkflowArtifacts.HasDerivationParent** | formula | `Not(Isblank(DerivedFromArtifact))` |
| **WorkflowArtifacts.ProducedByWorkflow** | lookup | `Lookup(WorkflowSteps.Workflow via ProducedByStep)` |
| **ComplianceVerdicts.ParentPath** | lookup | `Lookup(Workflows.RelativePath via Workflow)` |
| **ComplianceVerdicts.RelativePath** | formula | `ParentPath & "/verdicts/" & ComplianceVerdictId` |
| **ComplianceVerdicts.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **ComplianceVerdicts.Name** | formula | `Replace(Lower(WorkflowTitle), " ", "-")` |
| **ComplianceVerdicts.WorkflowTitle** | lookup | `Lookup(Workflows.Title via Workflow)` |
| **ComplianceVerdicts.MonthsSinceReview** | lookup | `Lookup(Workflows.MonthsSinceModified via Workflow)` |
| **ComplianceVerdicts.IsStale** | lookup | `Lookup(Workflows.IsStale via Workflow)` |
| **ComplianceVerdicts.AIStepCount** | lookup | `Lookup(Workflows.CountAISteps via Workflow)` |
| **ComplianceVerdicts.HasAIExecutedStep** | lookup | `Lookup(Workflows.HasAIAgentStep via Workflow)` |
| **ComplianceVerdicts.TotalPlanMinutes** | lookup | `Lookup(Workflows.CountTotalPlanMinutes via Workflow)` |
| **ComplianceVerdicts.TimeBudgetMinutes** | lookup | `Lookup(Workflows.MaxPlanMinutes via Workflow)` |
| **ComplianceVerdicts.IsOverTimeBudget** | lookup | `Lookup(Workflows.IsOverTimeBudget via Workflow)` |
| **ComplianceVerdicts.HasConsistencyViolation** | lookup | `Lookup(Workflows.HasConsistencyViolation via Workflow)` |
| **ComplianceVerdicts.HasUnmetGateSignoff** | lookup | `Lookup(Workflows.HasUnmetGateSignoff via Workflow)` |
| **ComplianceVerdicts.IsAtComplianceRisk** | formula | `Or(And(IsStale, HasAIExecutedStep), IsOverTimeBudget, HasConsistencyViolation, HasUnmetGateSignoff)` |
| **ComplianceVerdicts.Verdict** | lookup | `Lookup(ComplianceVerdictConcepts.PrefLabel via VerdictConcept)` |
| **Scenarios.RelativePath** | formula | `"scenarios/" & ScenarioId` |
| **Scenarios.Iri** | formula | `Replace(RelativePath, "/", "-")` |
| **Scenarios.Name** | formula | `Replace(Lower(Label), " ", "-")` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
