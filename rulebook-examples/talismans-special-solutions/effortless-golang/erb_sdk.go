// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/talisman-s-special-solutions-rulebook.json
//
// One struct per table, a Calc<Field>() method per calculated field, and the
// table registry main.go runs. Formulas compute through erb_runtime.go.

package main

// =============================================================================
// WORKFLOWS TABLE
// Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
// =============================================================================

// Workflow represents a row in the Workflows table
type Workflow struct {
	WorkflowId string `json:"workflow_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation.
	DisplayName *string `json:"display_name"`
	Title *string `json:"title"` // Human-readable title of the workflow. Maps to dct:title from Dublin Core. Example: 'Production Deployment Workflow'.
	Description *string `json:"description"` // Detailed description of the workflow's purpose and scope. Maps to dct:description from Dublin Core. Should explain what business goal the workflow achieves.
	Identifier *string `json:"identifier"` // External system identifier for cross-referencing. Maps to dct:identifier from Dublin Core. This is the join key back to document management systems, ticket systems, or other operational systems.
	Modified *string `json:"modified"` // Last modification timestamp. Maps to dct:modified from Dublin Core. Critical for answering CQ5: 'Which workflows haven't been reviewed or updated in twelve months?'
	Created *string `json:"created"` // Creation timestamp. Maps to dct:created from Dublin Core. Records when the workflow was first defined.
	StalenessThresholdMonths *int `json:"staleness_threshold_months"` // The governance POLICY (in months): the full review cadence after which this workflow's compliance documentation is formally out of date. The docs go stale exactly when this review age is exceeded — IsStale fires the instant MonthsSinceModified passes this policy line, with no deferral. The article hardcodes the CQ5 question at twelve months ('which workflows haven't been reviewed in twelve months'); promoting that threshold to a raw, editable field makes the policy itself a fact in the SSoT rather than a constant buried in the IsStale formula — so an org can set a 6-month or 18-month review cadence and the staleness verdict recomputes. Defaults to 12 to match the article.
	WorkflowStatus *string `json:"workflow_status"` // FK to WorkflowStatusConcepts. Captures the current lifecycle state of the workflow (draft, active, deprecated, archived). Maps to the SKOS CBox status vocabulary.
	WorkflowSteps *string `json:"workflow_steps"` // Reference to workflow steps. Represents the ntwf:hasStep relationship linking workflows to their constituent steps.
	CountOfNonProposedSteps *int `json:"count_of_non_proposed_steps"` // Calculated count of workflow steps in this workflow. Useful for workflow complexity analysis and reporting.
	HasMoreThan1Step *bool `json:"has_more_than1_step"`
	CountAISteps *int `json:"count_ai_steps"` // Number of steps in this workflow executed by an AIAgent (rollup over WorkflowSteps.IsExecutedByAI). This is the 'AI-executed' half of the article's CQ3 ('which steps are executed by AI agents') — counted against AIAgent individuals specifically, not the deterministic AutomatedPipeline, which is a disjoint agent type. Also drives the business-payoff query (a workflow is a compliance risk when an AI agent runs a step). Worked example: 2 (the AI risk-assessment step and the AI post-deployment health report).
	CountHumanSteps *int `json:"count_human_steps"` // Number of steps executed by a HumanAgent (rollup over WorkflowSteps.IsExecutedByHuman). The 'who actually runs this step' count — distinct from CountHumanRequiredSteps, which counts steps that demand a human decision (requiresHumanApproval). Worked example: 2 (the legal-review step and the release approval gate).
	CountHumanRequiredSteps *int `json:"count_human_required_steps"` // Number of steps that require a human decision (rollup over WorkflowSteps.RequiresHumanApproval). This is the 'human-required' half of the article's CQ3 ('which require a human decision'), answered — as the article notes — by a single FILTER on requiresHumanApproval. Worked example: 2 (the legal-review step and the release approval gate).
	CountApprovalConsistencyViolations *int `json:"count_approval_consistency_violations"` // Number of steps that require human approval but are not human-filled (rollup over WorkflowSteps.ApprovalConsistencyViolation). The clean ABox witness: this is 0 for the Production Deployment workflow. A non-zero value is the relational signal of a Suite-4 consistency violation.
	HasConsistencyViolation *bool `json:"has_consistency_violation"` // TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
	HasAIAgentStep *bool `json:"has_ai_agent_step"` // TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
	MonthsSinceModified *int `json:"months_since_modified"` // Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
	IsStale *bool `json:"is_stale"` // TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field.
	IsStaleAndHasAIAgent *bool `json:"is_stale_and_has_ai_agent"` // The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
	CountDerivationLinks *int `json:"count_derivation_links"` // Number of prov:wasDerivedFrom links among this workflow's artifacts (rollup over WorkflowArtifacts.HasDerivationParent). Answers the lineage half of CQ4: 5 artifacts form a chain with 4 derivation links.
	CountLegalOwnedSteps *int `json:"count_legal_owned_steps"` // Number of steps in this workflow whose owning department is Legal (rollup over WorkflowSteps.IsLegalOwned). CQ7: exactly one Legal-owned step in the Production Deployment workflow.
	CountEngineeringOwnedSteps *int `json:"count_engineering_owned_steps"` // Number of steps whose owning department is Engineering (rollup over WorkflowSteps.IsEngineeringOwned). Feeds CQ7's Engineering-involvement check.
	InvolvesEngineeringAndLegal *bool `json:"involves_engineering_and_legal"` // TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
	CountInferredPrecedencePairs *int `json:"count_inferred_precedence_pairs"` // Number of step-ordering pairs that the transitive closure of ntwf:precedesStep INFERRED (rollup over the closure view vw_step_precedence_closure where is_inferred = TRUE). The article's signature count: 6 of the 10 closure pairs were never asserted — including step-1 -> step-5. NOTE: this single-workflow model has exactly one Workflow, so the global closure view is wholly this workflow's; the COUNTIFS is unfiltered because every precedence edge belongs to the Production Deployment DAG.
	CountAssertedPrecedencePairs *int `json:"count_asserted_precedence_pairs"` // Number of step-ordering pairs that were directly ASSERTED as ntwf:precedesStep edges (rollup over vw_step_precedence_closure where is_inferred = FALSE) — the hop-1 rows. The article's 4 asserted edges. Together with CountInferredPrecedencePairs (6) this sums to the 10-pair closure, making CountOfPrecedenceClosurePairs an honest asserted+inferred total rather than an unconditional count. Single-workflow note as on CountInferredPrecedencePairs: the global closure view is this workflow's.
	CountOfPrecedenceClosurePairs *int `json:"count_of_precedence_closure_pairs"` // Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
	CountRolesWithBadFillerCardinality *int `json:"count_roles_with_bad_filler_cardinality"` // Number of roles that do NOT have exactly one filledBy arm set (rollup over Roles.HasExactlyOneFiller = FALSE). The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has 0 such roles — this is the Suite-1 functional/disjointness witness as a single integer. A non-zero value is the relational signal of the Suite-4 disjointness violation (a role filled by two agent classes, or by none). NOTE: this single-workflow model has exactly one Workflow and every Role participates in it, so the count is over all roles; a multi-workflow model would scope it through a role→workflow path.
	CountAgentTypeChanges *int `json:"count_agent_type_changes"` // Number of filledBy assignment periods that changed the agent CLASS of a role (rollup over RoleAssignments.IsAgentTypeChange = TRUE). NTWF governance distinguishes a same-class personnel/model swap from an agent-type transition; this counts the latter. NOTE: single-workflow model — every Role participates in the one workflow, so the count is over all assignment history; a multi-workflow model would scope it through a role→workflow path.
	CountComplianceAuditChanges *int `json:"count_compliance_audit_changes"` // Number of filledBy assignment periods that took a previously AI-executed binding and reassigned it to a human (rollup over RoleAssignments.RequiresComplianceAudit = TRUE). NTWF governance treats this as a data operation with compliance implications: changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent. Each such row must carry when (ValidFrom) and why (Reason). NOTE: single-workflow scoping as above.
	CountApprovalGateSteps *int `json:"count_approval_gate_steps"` // Number of this workflow's steps that are approval gates. >0 means the workflow has a blocking approval checkpoint; used by Cq2Satisfied to require that the gate exists before asking whether it has a human approver.
	CountGatesWithoutHumanApprover *int `json:"count_gates_without_human_approver"` // Number of approval gates with no resolved human approver (gate role not filled by a HumanAgent). Single-workflow model, so this global count is wholly this workflow's. Drives Cq2Satisfied (= a gate exists AND none lack a human approver).
	CountWorkflowArtifacts *int `json:"count_workflow_artifacts"` // Total artifacts produced by this workflow. With CountDerivationLinks (artifacts that have a wasDerivedFrom parent) this lets Cq4Satisfied check the provenance chain is intact: every artifact but the single origin has a parent.
	CountRolesWithEscalationViolation *int `json:"count_roles_with_escalation_violation"` // Number of roles that own an approval gate yet escalate to no one (Roles.EscalationViolation). Single-workflow model, so this global count applies to this workflow. Drives Cq6Satisfied (=0): the model's own native escalation-completeness invariant, replacing any hardcoded 'must reach the CTO' check.
	CountUnconsumedDatasets *int `json:"count_unconsumed_datasets"` // Number of datasets not consumed by any step (Datasets.IsConsumed = FALSE). Single-workflow model, so this global count applies to this workflow. Drives Cq8Satisfied (=0).
	Cq1Satisfied *bool `json:"cq1_satisfied"` // CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
	Cq2Satisfied *bool `json:"cq2_satisfied"` // CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
	Cq3Satisfied *bool `json:"cq3_satisfied"` // CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
	Cq4Satisfied *bool `json:"cq4_satisfied"` // CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
	Cq5Satisfied *bool `json:"cq5_satisfied"` // CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
	Cq6Satisfied *bool `json:"cq6_satisfied"` // CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
	Cq7Satisfied *bool `json:"cq7_satisfied"` // CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
	Cq8Satisfied *bool `json:"cq8_satisfied"` // CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="workflows/" & {{WorkflowId}}
func (tc *Workflow) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("workflows/"), erbTextOr(vStrPlain(tc.WorkflowId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Workflow) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Workflow) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStr(tc.DisplayName)), vS(" "), vS("-")))
}

// CalcHasMoreThan1Step computes the HasMoreThan1Step calculated field
// Formula: ={{CountOfNonProposedSteps}} > 1
func (tc *Workflow) CalcHasMoreThan1Step() *bool {
	return toBoolPtr(erbCmp(vInt(tc.CountOfNonProposedSteps), ">", vI(1)))
}

// CalcHasConsistencyViolation computes the HasConsistencyViolation calculated field
// TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
// Formula: ={{CountApprovalConsistencyViolations}} > 0
func (tc *Workflow) CalcHasConsistencyViolation() *bool {
	return toBoolPtr(erbCmp(vInt(tc.CountApprovalConsistencyViolations), ">", vI(0)))
}

// CalcHasAIAgentStep computes the HasAIAgentStep calculated field
// TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
// Formula: ={{CountAISteps}} > 0
func (tc *Workflow) CalcHasAIAgentStep() *bool {
	return toBoolPtr(erbCmp(vInt(tc.CountAISteps), ">", vI(0)))
}

// CalcMonthsSinceModified computes the MonthsSinceModified calculated field
// Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
// Formula: =DATETIME_DIFF(NOW(), {{Modified}}, "months")
func (tc *Workflow) CalcMonthsSinceModified() *int {
	return toIntPtr(erbInteger(erbDatetimeDiff(erbNow(), vStr(tc.Modified), vS("months"))))
}

// CalcIsStale computes the IsStale calculated field
// TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field.
// Formula: ={{MonthsSinceModified}} > {{StalenessThresholdMonths}}
func (tc *Workflow) CalcIsStale() *bool {
	return toBoolPtr(erbCmp(vInt(tc.MonthsSinceModified), ">", erbNullif(vInt(tc.StalenessThresholdMonths))))
}

// CalcIsStaleAndHasAIAgent computes the IsStaleAndHasAIAgent calculated field
// The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
// Formula: =AND({{IsStale}}, {{HasAIAgentStep}})
func (tc *Workflow) CalcIsStaleAndHasAIAgent() *bool {
	return toBoolPtr(erbAnd(erbBool3(vBool(tc.IsStale)), erbBool3(vBool(tc.HasAIAgentStep))))
}

// CalcInvolvesEngineeringAndLegal computes the InvolvesEngineeringAndLegal calculated field
// TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
// Formula: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0)
func (tc *Workflow) CalcInvolvesEngineeringAndLegal() *bool {
	return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.CountEngineeringOwnedSteps), ">", vI(0))), erbBool3(erbCmp(vInt(tc.CountLegalOwnedSteps), ">", vI(0)))))
}

// CalcCountOfPrecedenceClosurePairs computes the CountOfPrecedenceClosurePairs calculated field
// Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
// Formula: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}}
func (tc *Workflow) CalcCountOfPrecedenceClosurePairs() *int {
	return toIntPtr(erbInteger(erbAdd(vInt(tc.CountAssertedPrecedencePairs), vInt(tc.CountInferredPrecedencePairs))))
}

// CalcCq1Satisfied computes the Cq1Satisfied calculated field
// CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
// Formula: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2
func (tc *Workflow) CalcCq1Satisfied() *bool {
	return toBoolPtr(erbEq(vInt(tc.CountOfPrecedenceClosurePairs), erbDiv(erbMul(vInt(tc.CountOfNonProposedSteps), erbSub(vInt(tc.CountOfNonProposedSteps), vI(1))), vI(2))))
}

// CalcCq2Satisfied computes the Cq2Satisfied calculated field
// CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
// Formula: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0)
func (tc *Workflow) CalcCq2Satisfied() *bool {
	return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.CountApprovalGateSteps), ">", vI(0))), erbBool3(erbEq(vInt(tc.CountGatesWithoutHumanApprover), vI(0)))))
}

// CalcCq3Satisfied computes the Cq3Satisfied calculated field
// CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
// Formula: =NOT({{HasConsistencyViolation}})
func (tc *Workflow) CalcCq3Satisfied() *bool {
	return toBoolPtr(erbNot(erbBool3(vBool(tc.HasConsistencyViolation))))
}

// CalcCq4Satisfied computes the Cq4Satisfied calculated field
// CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
// Formula: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1
func (tc *Workflow) CalcCq4Satisfied() *bool {
	return toBoolPtr(erbEq(vInt(tc.CountDerivationLinks), erbSub(vInt(tc.CountWorkflowArtifacts), vI(1))))
}

// CalcCq5Satisfied computes the Cq5Satisfied calculated field
// CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
// Formula: =NOT({{IsStale}})
func (tc *Workflow) CalcCq5Satisfied() *bool {
	return toBoolPtr(erbNot(erbBool3(vBool(tc.IsStale))))
}

// CalcCq6Satisfied computes the Cq6Satisfied calculated field
// CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
// Formula: ={{CountRolesWithEscalationViolation}} = 0
func (tc *Workflow) CalcCq6Satisfied() *bool {
	return toBoolPtr(erbEq(vInt(tc.CountRolesWithEscalationViolation), vI(0)))
}

// CalcCq7Satisfied computes the Cq7Satisfied calculated field
// CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
// Formula: ={{InvolvesEngineeringAndLegal}}
func (tc *Workflow) CalcCq7Satisfied() *bool {
	return toBoolPtr(vBool(tc.InvolvesEngineeringAndLegal))
}

// CalcCq8Satisfied computes the Cq8Satisfied calculated field
// CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
// Formula: ={{CountUnconsumedDatasets}} = 0
func (tc *Workflow) CalcCq8Satisfied() *bool {
	return toBoolPtr(erbEq(vInt(tc.CountUnconsumedDatasets), vI(0)))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *Workflow) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "has_more_than1_step", func() { tc.HasMoreThan1Step = tc.CalcHasMoreThan1Step() })
	calcGuard(tc, "has_consistency_violation", func() { tc.HasConsistencyViolation = tc.CalcHasConsistencyViolation() })
	calcGuard(tc, "has_ai_agent_step", func() { tc.HasAIAgentStep = tc.CalcHasAIAgentStep() })
	calcGuard(tc, "months_since_modified", func() { tc.MonthsSinceModified = tc.CalcMonthsSinceModified() })
	calcGuard(tc, "involves_engineering_and_legal", func() { tc.InvolvesEngineeringAndLegal = tc.CalcInvolvesEngineeringAndLegal() })
	calcGuard(tc, "count_of_precedence_closure_pairs", func() { tc.CountOfPrecedenceClosurePairs = tc.CalcCountOfPrecedenceClosurePairs() })
	calcGuard(tc, "cq2_satisfied", func() { tc.Cq2Satisfied = tc.CalcCq2Satisfied() })
	calcGuard(tc, "cq4_satisfied", func() { tc.Cq4Satisfied = tc.CalcCq4Satisfied() })
	calcGuard(tc, "cq6_satisfied", func() { tc.Cq6Satisfied = tc.CalcCq6Satisfied() })
	calcGuard(tc, "cq8_satisfied", func() { tc.Cq8Satisfied = tc.CalcCq8Satisfied() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
	calcGuard(tc, "is_stale", func() { tc.IsStale = tc.CalcIsStale() })
	calcGuard(tc, "cq1_satisfied", func() { tc.Cq1Satisfied = tc.CalcCq1Satisfied() })
	calcGuard(tc, "cq3_satisfied", func() { tc.Cq3Satisfied = tc.CalcCq3Satisfied() })
	calcGuard(tc, "cq7_satisfied", func() { tc.Cq7Satisfied = tc.CalcCq7Satisfied() })
	// Level 3
	calcGuard(tc, "is_stale_and_has_ai_agent", func() { tc.IsStaleAndHasAIAgent = tc.CalcIsStaleAndHasAIAgent() })
	calcGuard(tc, "cq5_satisfied", func() { tc.Cq5Satisfied = tc.CalcCq5Satisfied() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *Workflow) ComputeAll() *Workflow {
	tc.erbComputeCalculations()
	return tc
}

func (tc *Workflow) erbGet(field string) Value {
	switch field {
	case "workflow_id":
		return vStrPlain(tc.WorkflowId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStr(tc.DisplayName)
	case "title":
		return vStr(tc.Title)
	case "description":
		return vStr(tc.Description)
	case "identifier":
		return vStr(tc.Identifier)
	case "modified":
		return vStr(tc.Modified)
	case "created":
		return vStr(tc.Created)
	case "staleness_threshold_months":
		return vInt(tc.StalenessThresholdMonths)
	case "workflow_status":
		return vStr(tc.WorkflowStatus)
	case "workflow_steps":
		return vStr(tc.WorkflowSteps)
	case "count_of_non_proposed_steps":
		return vInt(tc.CountOfNonProposedSteps)
	case "has_more_than1_step":
		return vBool(tc.HasMoreThan1Step)
	case "count_ai_steps":
		return vInt(tc.CountAISteps)
	case "count_human_steps":
		return vInt(tc.CountHumanSteps)
	case "count_human_required_steps":
		return vInt(tc.CountHumanRequiredSteps)
	case "count_approval_consistency_violations":
		return vInt(tc.CountApprovalConsistencyViolations)
	case "has_consistency_violation":
		return vBool(tc.HasConsistencyViolation)
	case "has_ai_agent_step":
		return vBool(tc.HasAIAgentStep)
	case "months_since_modified":
		return vInt(tc.MonthsSinceModified)
	case "is_stale":
		return vBool(tc.IsStale)
	case "is_stale_and_has_ai_agent":
		return vBool(tc.IsStaleAndHasAIAgent)
	case "count_derivation_links":
		return vInt(tc.CountDerivationLinks)
	case "count_legal_owned_steps":
		return vInt(tc.CountLegalOwnedSteps)
	case "count_engineering_owned_steps":
		return vInt(tc.CountEngineeringOwnedSteps)
	case "involves_engineering_and_legal":
		return vBool(tc.InvolvesEngineeringAndLegal)
	case "count_inferred_precedence_pairs":
		return vInt(tc.CountInferredPrecedencePairs)
	case "count_asserted_precedence_pairs":
		return vInt(tc.CountAssertedPrecedencePairs)
	case "count_of_precedence_closure_pairs":
		return vInt(tc.CountOfPrecedenceClosurePairs)
	case "count_roles_with_bad_filler_cardinality":
		return vInt(tc.CountRolesWithBadFillerCardinality)
	case "count_agent_type_changes":
		return vInt(tc.CountAgentTypeChanges)
	case "count_compliance_audit_changes":
		return vInt(tc.CountComplianceAuditChanges)
	case "count_approval_gate_steps":
		return vInt(tc.CountApprovalGateSteps)
	case "count_gates_without_human_approver":
		return vInt(tc.CountGatesWithoutHumanApprover)
	case "count_workflow_artifacts":
		return vInt(tc.CountWorkflowArtifacts)
	case "count_roles_with_escalation_violation":
		return vInt(tc.CountRolesWithEscalationViolation)
	case "count_unconsumed_datasets":
		return vInt(tc.CountUnconsumedDatasets)
	case "cq1_satisfied":
		return vBool(tc.Cq1Satisfied)
	case "cq2_satisfied":
		return vBool(tc.Cq2Satisfied)
	case "cq3_satisfied":
		return vBool(tc.Cq3Satisfied)
	case "cq4_satisfied":
		return vBool(tc.Cq4Satisfied)
	case "cq5_satisfied":
		return vBool(tc.Cq5Satisfied)
	case "cq6_satisfied":
		return vBool(tc.Cq6Satisfied)
	case "cq7_satisfied":
		return vBool(tc.Cq7Satisfied)
	case "cq8_satisfied":
		return vBool(tc.Cq8Satisfied)
	}
	panic("Workflows has no field " + field)
}

func (tc *Workflow) erbSet(field string, v Value) {
	switch field {
	case "workflow_id":
		tc.WorkflowId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "title":
		tc.Title = toStringPtr(v)
	case "description":
		tc.Description = toStringPtr(v)
	case "identifier":
		tc.Identifier = toStringPtr(v)
	case "modified":
		tc.Modified = toStringPtr(v)
	case "created":
		tc.Created = toStringPtr(v)
	case "staleness_threshold_months":
		tc.StalenessThresholdMonths = toIntPtr(v)
	case "workflow_status":
		tc.WorkflowStatus = toStringPtr(v)
	case "workflow_steps":
		tc.WorkflowSteps = toStringPtr(v)
	case "count_of_non_proposed_steps":
		tc.CountOfNonProposedSteps = toIntPtr(v)
	case "has_more_than1_step":
		tc.HasMoreThan1Step = toBoolPtr(v)
	case "count_ai_steps":
		tc.CountAISteps = toIntPtr(v)
	case "count_human_steps":
		tc.CountHumanSteps = toIntPtr(v)
	case "count_human_required_steps":
		tc.CountHumanRequiredSteps = toIntPtr(v)
	case "count_approval_consistency_violations":
		tc.CountApprovalConsistencyViolations = toIntPtr(v)
	case "has_consistency_violation":
		tc.HasConsistencyViolation = toBoolPtr(v)
	case "has_ai_agent_step":
		tc.HasAIAgentStep = toBoolPtr(v)
	case "months_since_modified":
		tc.MonthsSinceModified = toIntPtr(v)
	case "is_stale":
		tc.IsStale = toBoolPtr(v)
	case "is_stale_and_has_ai_agent":
		tc.IsStaleAndHasAIAgent = toBoolPtr(v)
	case "count_derivation_links":
		tc.CountDerivationLinks = toIntPtr(v)
	case "count_legal_owned_steps":
		tc.CountLegalOwnedSteps = toIntPtr(v)
	case "count_engineering_owned_steps":
		tc.CountEngineeringOwnedSteps = toIntPtr(v)
	case "involves_engineering_and_legal":
		tc.InvolvesEngineeringAndLegal = toBoolPtr(v)
	case "count_inferred_precedence_pairs":
		tc.CountInferredPrecedencePairs = toIntPtr(v)
	case "count_asserted_precedence_pairs":
		tc.CountAssertedPrecedencePairs = toIntPtr(v)
	case "count_of_precedence_closure_pairs":
		tc.CountOfPrecedenceClosurePairs = toIntPtr(v)
	case "count_roles_with_bad_filler_cardinality":
		tc.CountRolesWithBadFillerCardinality = toIntPtr(v)
	case "count_agent_type_changes":
		tc.CountAgentTypeChanges = toIntPtr(v)
	case "count_compliance_audit_changes":
		tc.CountComplianceAuditChanges = toIntPtr(v)
	case "count_approval_gate_steps":
		tc.CountApprovalGateSteps = toIntPtr(v)
	case "count_gates_without_human_approver":
		tc.CountGatesWithoutHumanApprover = toIntPtr(v)
	case "count_workflow_artifacts":
		tc.CountWorkflowArtifacts = toIntPtr(v)
	case "count_roles_with_escalation_violation":
		tc.CountRolesWithEscalationViolation = toIntPtr(v)
	case "count_unconsumed_datasets":
		tc.CountUnconsumedDatasets = toIntPtr(v)
	case "cq1_satisfied":
		tc.Cq1Satisfied = toBoolPtr(v)
	case "cq2_satisfied":
		tc.Cq2Satisfied = toBoolPtr(v)
	case "cq3_satisfied":
		tc.Cq3Satisfied = toBoolPtr(v)
	case "cq4_satisfied":
		tc.Cq4Satisfied = toBoolPtr(v)
	case "cq5_satisfied":
		tc.Cq5Satisfied = toBoolPtr(v)
	case "cq6_satisfied":
		tc.Cq6Satisfied = toBoolPtr(v)
	case "cq7_satisfied":
		tc.Cq7Satisfied = toBoolPtr(v)
	case "cq8_satisfied":
		tc.Cq8Satisfied = toBoolPtr(v)
	default:
		panic("Workflows has no field " + field)
	}
}

func (tc *Workflow) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "workflow_id", "relative_path", "iri", "name", "display_name", "title", "description", "identifier", "modified", "created", "staleness_threshold_months", "workflow_status", "workflow_steps", "count_of_non_proposed_steps", "has_more_than1_step", "count_ai_steps", "count_human_steps", "count_human_required_steps", "count_approval_consistency_violations", "has_consistency_violation", "has_ai_agent_step", "months_since_modified", "is_stale", "is_stale_and_has_ai_agent", "count_derivation_links", "count_legal_owned_steps", "count_engineering_owned_steps", "involves_engineering_and_legal", "count_inferred_precedence_pairs", "count_asserted_precedence_pairs", "count_of_precedence_closure_pairs", "count_roles_with_bad_filler_cardinality", "count_agent_type_changes", "count_compliance_audit_changes", "count_approval_gate_steps", "count_gates_without_human_approver", "count_workflow_artifacts", "count_roles_with_escalation_violation", "count_unconsumed_datasets", "cq1_satisfied", "cq2_satisfied", "cq3_satisfied", "cq4_satisfied", "cq5_satisfied", "cq6_satisfied", "cq7_satisfied", "cq8_satisfied":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *Workflow) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *Workflow) erbResetErrors() { tc.ErbErrors = nil }

func (tc *Workflow) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *Workflow) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadWorkflowRecords reads Workflows rows from a JSON array file.
func LoadWorkflowRecords(path string) ([]Workflow, error) {
	records, err := loadRecords(path, func() Record { return &Workflow{} })
	if err != nil {
		return nil, err
	}
	rows := make([]Workflow, len(records))
	for i, r := range records {
		rows[i] = *r.(*Workflow)
	}
	return rows, nil
}

// =============================================================================
// WORKFLOWSTEPS TABLE
// Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
// =============================================================================

// WorkflowStep represents a row in the WorkflowSteps table
type WorkflowStep struct {
	WorkflowStepId string `json:"workflow_step_id"`
	ParentPath *string `json:"parent_path"` // Helper: the Workflows parent's RelativePath, pulled across the Workflow FK. Exists so RelativePath can concatenate the '/steps/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"`
	DisplayName *string `json:"display_name"`
	Workflow *string `json:"workflow"` // Forward foreign key to the parent workflow (ntwf:isStepOf) — the authoritative stored link from step to its containing workflow; every per-workflow rollup (CountOfNonProposedSteps, the department-owned counts, etc.) reads it. This IS the stored column, not a derived inverse: isReversed is false.
	PrecedingStepCount *int `json:"preceding_step_count"` // Number of steps that TRANSITIVELY precede this step in the ntwf:precedesStep ordering — a rollup over the closure view vw_step_precedence_closure counting rows whose to_id is this step (i.e. this step's ancestors). On the linear Production Deployment chain: 0,1,2,3,4. Derived purely from the asserted StepPrecedence edges via their transitive closure; nothing is hand-entered. SequencePosition is this + 1.
	InferredSequencePosition *int `json:"inferred_sequence_position"` // The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering).
	SequencePositionOverride *int `json:"sequence_position_override"` // OPTIONAL hand-asserted ordinal position — the article's pure ntwf:sequencePosition functional datatype property, preserved as an override slot. NULL on the linear Production Deployment chain (the inference is unambiguous, so nothing is pinned). When set, it wins over InferredSequencePosition in the resolved SequencePosition — this is how a modeler recovers the owl:FunctionalProperty 'exactly one distinct position per step' guarantee on a partial order / branch where the inferred rank would tie. Maps to ntwf:sequencePosition (the article's asserted functional property).
	SequencePosition *int `json:"sequence_position"` // The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
	AssignedRole *string `json:"assigned_role"` // Foreign key to the Role responsible for executing this step. Maps to ntwf:assignedRole (owl:FunctionalProperty — exactly one role per step). Critical for implementing Heuristic 2 (role-agent separation): steps point to roles, not directly to agents.
	RequiresHumanApproval *bool `json:"requires_human_approval"` // Boolean flag indicating whether a human agent must fill the assigned role. Maps to ntwf:requiresHumanApproval. Enables answering CQ3: 'Which steps require human decisions vs. AI execution?'
	StepDurationMinutes *int `json:"step_duration_minutes"` // Expected duration of this step in minutes. Maps to ntwf:stepDurationMinutes (datatype property, not functional). Enables SLA and throughput analysis.
	ConsumesDataset *string `json:"consumes_dataset"` // FK to Datasets. Records which DCAT dataset this step consumes as input. Kept separate from artifact consumption to preserve DCAT metadata semantics (consumesDataset vs. requiresArtifact).
	ProducesArtifacts *string `json:"produces_artifacts"` // Back-reference to WorkflowArtifacts produced by this step. Inverse of WorkflowArtifacts.ProducedByStep (ntwf:producesArtifact / prov:wasGeneratedBy).
	RequiresArtifacts *string `json:"requires_artifacts"` // FK to WorkflowArtifact(s) this step CONSUMES as input. Maps to ntwf:requiresArtifact (aligned to prov:used). Kept distinct from producesArtifact (prov:generated) and from consumesDataset (dcat:Dataset) so the input/output and artifact/dataset semantics stay separate. Inverse is WorkflowArtifacts.RequiredBySteps.
	ApprovalGate *string `json:"approval_gate"` // Back-reference to the ApprovalGate subtype row that specializes this step, if any. Inverse of ApprovalGates.WorkflowStep. A step has zero or one approval gate; when present, the gate adds escalationThresholdHours and marks the step as a blocking decision checkpoint. This models ntwf:ApprovalGate rdfs:subClassOf WorkflowStep as a shared-key 1:1 specialization rather than collapsing two DAG nodes into one.
	Precedes *string `json:"precedes"` // Back-reference to StepPrecedence edges where this step is the FromStep (the predecessor). Inverse of StepPrecedence.FromStep. Together with PrecededBy, lets you walk the ntwf:precedesStep ordering in both directions.
	PrecededBy *string `json:"preceded_by"` // Back-reference to StepPrecedence edges where this step is the ToStep (the successor). Inverse of StepPrecedence.ToStep. Part of the ntwf:precedesStep transitive ordering relationship.
	ExecutingHumanAgent *string `json:"executing_human_agent"` // The HumanAgent (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the HumanAgent arm). Load-bearing lookup: it follows the role→agent indirection the article relies on, so a step knows its executing agent without per-step agent bindings.
	ExecutingAIAgent *string `json:"executing_ai_agent"` // The AIAgent (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the AIAgent arm). One of the three polymorphic filledBy arms.
	ExecutingAutomatedPipeline *string `json:"executing_automated_pipeline"` // The AutomatedPipeline (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the AutomatedPipeline arm).
	ExecutingAgentType *string `json:"executing_agent_type"` // Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
	IsExecutedByAI *bool `json:"is_executed_by_ai"` // TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
	IsExecutedByHuman *bool `json:"is_executed_by_human"` // TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
	IsApprovalGate *bool `json:"is_approval_gate"` // TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
	ApprovalConsistencyViolation *bool `json:"approval_consistency_violation"` // Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
	ApprovalIsHumanFilled *bool `json:"approval_is_human_filled"` // Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
	OwningDepartment *string `json:"owning_department"` // The department that owns this step's assigned role, resolved through AssignedRole → Roles.OwnedBy. Lets a workflow report which departments its steps touch (CQ7: 'which workflows involve both Engineering and Legal, and at what steps do they intersect').
	IsLegalOwned *bool `json:"is_legal_owned"` // TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
	IsEngineeringOwned *bool `json:"is_engineering_owned"` // TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/steps/" & {{WorkflowStepId}}
func (tc *WorkflowStep) CalcRelativePath() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.ParentPath)), vS("/steps/"), erbTextOr(vStrPlain(tc.WorkflowStepId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *WorkflowStep) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *WorkflowStep) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStr(tc.DisplayName)), vS(" "), vS("-")))
}

// CalcInferredSequencePosition computes the InferredSequencePosition calculated field
// The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering).
// Formula: ={{PrecedingStepCount}} + 1
func (tc *WorkflowStep) CalcInferredSequencePosition() *int {
	return toIntPtr(erbInteger(erbAdd(vInt(tc.PrecedingStepCount), vI(1))))
}

// CalcSequencePosition computes the SequencePosition calculated field
// The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
// Formula: =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, {{InferredSequencePosition}})
func (tc *WorkflowStep) CalcSequencePosition() *int {
	return toIntPtr(erbInteger(erbIf(erbBool3(erbIsNotBlank(vInt(tc.SequencePositionOverride))), func() Value { return vInt(tc.SequencePositionOverride) }, func() Value { return vInt(tc.InferredSequencePosition) })))
}

// CalcExecutingAgentType computes the ExecutingAgentType calculated field
// Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
// Formula: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *WorkflowStep) CalcExecutingAgentType() *string {
	return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.ExecutingHumanAgent))))), func() Value { return vS("HumanAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.ExecutingAIAgent))))), func() Value { return vS("AIAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.ExecutingAutomatedPipeline))))), func() Value { return vS("AutomatedPipeline") }, func() Value { return vS("") }) }) }))
}

// CalcIsExecutedByAI computes the IsExecutedByAI calculated field
// TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
// Formula: =NOT(ISBLANK({{ExecutingAIAgent}}))
func (tc *WorkflowStep) CalcIsExecutedByAI() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.ExecutingAIAgent)))))
}

// CalcIsExecutedByHuman computes the IsExecutedByHuman calculated field
// TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
// Formula: =NOT(ISBLANK({{ExecutingHumanAgent}}))
func (tc *WorkflowStep) CalcIsExecutedByHuman() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.ExecutingHumanAgent)))))
}

// CalcIsApprovalGate computes the IsApprovalGate calculated field
// TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
// Formula: =NOT(ISBLANK({{ApprovalGate}}))
func (tc *WorkflowStep) CalcIsApprovalGate() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.ApprovalGate)))))
}

// CalcApprovalConsistencyViolation computes the ApprovalConsistencyViolation calculated field
// Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
// Formula: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}}))
func (tc *WorkflowStep) CalcApprovalConsistencyViolation() *bool {
	return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.RequiresHumanApproval)), erbBool3(erbIsBlank(vStr(tc.ExecutingHumanAgent)))))
}

// CalcApprovalIsHumanFilled computes the ApprovalIsHumanFilled calculated field
// Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
// Formula: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE)
func (tc *WorkflowStep) CalcApprovalIsHumanFilled() *bool {
	return toBoolPtr(erbIf(erbIsTrue(vBool(tc.RequiresHumanApproval)), func() Value { return erbNot(erbBool3(erbIsBlank(vStr(tc.ExecutingHumanAgent)))) }, func() Value { return vB(true) }))
}

// CalcIsLegalOwned computes the IsLegalOwned calculated field
// TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
// Formula: ={{OwningDepartment}} = "ntwf-legal-dept"
func (tc *WorkflowStep) CalcIsLegalOwned() *bool {
	return toBoolPtr(erbEq(vStr(tc.OwningDepartment), vS("ntwf-legal-dept")))
}

// CalcIsEngineeringOwned computes the IsEngineeringOwned calculated field
// TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
// Formula: ={{OwningDepartment}} = "ntwf-engineering"
func (tc *WorkflowStep) CalcIsEngineeringOwned() *bool {
	return toBoolPtr(erbEq(vStr(tc.OwningDepartment), vS("ntwf-engineering")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *WorkflowStep) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "inferred_sequence_position", func() { tc.InferredSequencePosition = tc.CalcInferredSequencePosition() })
	calcGuard(tc, "executing_agent_type", func() { tc.ExecutingAgentType = tc.CalcExecutingAgentType() })
	calcGuard(tc, "is_executed_by_ai", func() { tc.IsExecutedByAI = tc.CalcIsExecutedByAI() })
	calcGuard(tc, "is_executed_by_human", func() { tc.IsExecutedByHuman = tc.CalcIsExecutedByHuman() })
	calcGuard(tc, "is_approval_gate", func() { tc.IsApprovalGate = tc.CalcIsApprovalGate() })
	calcGuard(tc, "approval_consistency_violation", func() { tc.ApprovalConsistencyViolation = tc.CalcApprovalConsistencyViolation() })
	calcGuard(tc, "approval_is_human_filled", func() { tc.ApprovalIsHumanFilled = tc.CalcApprovalIsHumanFilled() })
	calcGuard(tc, "is_legal_owned", func() { tc.IsLegalOwned = tc.CalcIsLegalOwned() })
	calcGuard(tc, "is_engineering_owned", func() { tc.IsEngineeringOwned = tc.CalcIsEngineeringOwned() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
	calcGuard(tc, "sequence_position", func() { tc.SequencePosition = tc.CalcSequencePosition() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *WorkflowStep) ComputeAll() *WorkflowStep {
	tc.erbComputeCalculations()
	return tc
}

func (tc *WorkflowStep) erbGet(field string) Value {
	switch field {
	case "workflow_step_id":
		return vStrPlain(tc.WorkflowStepId)
	case "parent_path":
		return vStr(tc.ParentPath)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStr(tc.DisplayName)
	case "workflow":
		return vStr(tc.Workflow)
	case "preceding_step_count":
		return vInt(tc.PrecedingStepCount)
	case "inferred_sequence_position":
		return vInt(tc.InferredSequencePosition)
	case "sequence_position_override":
		return vInt(tc.SequencePositionOverride)
	case "sequence_position":
		return vInt(tc.SequencePosition)
	case "assigned_role":
		return vStr(tc.AssignedRole)
	case "requires_human_approval":
		return vBool(tc.RequiresHumanApproval)
	case "step_duration_minutes":
		return vInt(tc.StepDurationMinutes)
	case "consumes_dataset":
		return vStr(tc.ConsumesDataset)
	case "produces_artifacts":
		return vStr(tc.ProducesArtifacts)
	case "requires_artifacts":
		return vStr(tc.RequiresArtifacts)
	case "approval_gate":
		return vStr(tc.ApprovalGate)
	case "precedes":
		return vStr(tc.Precedes)
	case "preceded_by":
		return vStr(tc.PrecededBy)
	case "executing_human_agent":
		return vStr(tc.ExecutingHumanAgent)
	case "executing_ai_agent":
		return vStr(tc.ExecutingAIAgent)
	case "executing_automated_pipeline":
		return vStr(tc.ExecutingAutomatedPipeline)
	case "executing_agent_type":
		return vStr(tc.ExecutingAgentType)
	case "is_executed_by_ai":
		return vBool(tc.IsExecutedByAI)
	case "is_executed_by_human":
		return vBool(tc.IsExecutedByHuman)
	case "is_approval_gate":
		return vBool(tc.IsApprovalGate)
	case "approval_consistency_violation":
		return vBool(tc.ApprovalConsistencyViolation)
	case "approval_is_human_filled":
		return vBool(tc.ApprovalIsHumanFilled)
	case "owning_department":
		return vStr(tc.OwningDepartment)
	case "is_legal_owned":
		return vBool(tc.IsLegalOwned)
	case "is_engineering_owned":
		return vBool(tc.IsEngineeringOwned)
	}
	panic("WorkflowSteps has no field " + field)
}

func (tc *WorkflowStep) erbSet(field string, v Value) {
	switch field {
	case "workflow_step_id":
		tc.WorkflowStepId = strPlain(v)
	case "parent_path":
		tc.ParentPath = toStringPtr(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "workflow":
		tc.Workflow = toStringPtr(v)
	case "preceding_step_count":
		tc.PrecedingStepCount = toIntPtr(v)
	case "inferred_sequence_position":
		tc.InferredSequencePosition = toIntPtr(v)
	case "sequence_position_override":
		tc.SequencePositionOverride = toIntPtr(v)
	case "sequence_position":
		tc.SequencePosition = toIntPtr(v)
	case "assigned_role":
		tc.AssignedRole = toStringPtr(v)
	case "requires_human_approval":
		tc.RequiresHumanApproval = toBoolPtr(v)
	case "step_duration_minutes":
		tc.StepDurationMinutes = toIntPtr(v)
	case "consumes_dataset":
		tc.ConsumesDataset = toStringPtr(v)
	case "produces_artifacts":
		tc.ProducesArtifacts = toStringPtr(v)
	case "requires_artifacts":
		tc.RequiresArtifacts = toStringPtr(v)
	case "approval_gate":
		tc.ApprovalGate = toStringPtr(v)
	case "precedes":
		tc.Precedes = toStringPtr(v)
	case "preceded_by":
		tc.PrecededBy = toStringPtr(v)
	case "executing_human_agent":
		tc.ExecutingHumanAgent = toStringPtr(v)
	case "executing_ai_agent":
		tc.ExecutingAIAgent = toStringPtr(v)
	case "executing_automated_pipeline":
		tc.ExecutingAutomatedPipeline = toStringPtr(v)
	case "executing_agent_type":
		tc.ExecutingAgentType = toStringPtr(v)
	case "is_executed_by_ai":
		tc.IsExecutedByAI = toBoolPtr(v)
	case "is_executed_by_human":
		tc.IsExecutedByHuman = toBoolPtr(v)
	case "is_approval_gate":
		tc.IsApprovalGate = toBoolPtr(v)
	case "approval_consistency_violation":
		tc.ApprovalConsistencyViolation = toBoolPtr(v)
	case "approval_is_human_filled":
		tc.ApprovalIsHumanFilled = toBoolPtr(v)
	case "owning_department":
		tc.OwningDepartment = toStringPtr(v)
	case "is_legal_owned":
		tc.IsLegalOwned = toBoolPtr(v)
	case "is_engineering_owned":
		tc.IsEngineeringOwned = toBoolPtr(v)
	default:
		panic("WorkflowSteps has no field " + field)
	}
}

func (tc *WorkflowStep) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "workflow_step_id", "parent_path", "relative_path", "iri", "name", "display_name", "workflow", "preceding_step_count", "inferred_sequence_position", "sequence_position_override", "sequence_position", "assigned_role", "requires_human_approval", "step_duration_minutes", "consumes_dataset", "produces_artifacts", "requires_artifacts", "approval_gate", "precedes", "preceded_by", "executing_human_agent", "executing_ai_agent", "executing_automated_pipeline", "executing_agent_type", "is_executed_by_ai", "is_executed_by_human", "is_approval_gate", "approval_consistency_violation", "approval_is_human_filled", "owning_department", "is_legal_owned", "is_engineering_owned":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *WorkflowStep) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *WorkflowStep) erbResetErrors() { tc.ErbErrors = nil }

func (tc *WorkflowStep) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *WorkflowStep) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadWorkflowStepRecords reads WorkflowSteps rows from a JSON array file.
func LoadWorkflowStepRecords(path string) ([]WorkflowStep, error) {
	records, err := loadRecords(path, func() Record { return &WorkflowStep{} })
	if err != nil {
		return nil, err
	}
	rows := make([]WorkflowStep, len(records))
	for i, r := range records {
		rows[i] = *r.(*WorkflowStep)
	}
	return rows, nil
}

// =============================================================================
// APPROVALGATES TABLE
// Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
// =============================================================================

// ApprovalGate represents a row in the ApprovalGates table
type ApprovalGate struct {
	ApprovalGateId string `json:"approval_gate_id"`
	ParentPath *string `json:"parent_path"` // Helper: the WorkflowSteps parent's RelativePath, pulled across the WorkflowStep FK. Exists so RelativePath can concatenate the '/approval-gates/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"`
	DisplayName *string `json:"display_name"`
	WorkflowStep *string `json:"workflow_step"` // 1:1 FK to the WorkflowStep this gate specializes (subtype shared key). This is the relational expression of ntwf:ApprovalGate rdfs:subClassOf WorkflowStep: the gate row adds escalationThresholdHours to its step. Inverse is WorkflowSteps.ApprovalGate.
	EscalationThresholdHours *int `json:"escalation_threshold_hours"` // Integer number of hours that may elapse on a pending gate before the ntwf:delegatesTo chain activates. Maps to ntwf:escalationThresholdHours. Domain applies only to ApprovalGate individuals — which is exactly why the gate is its own subtype table and this attribute does not live on every WorkflowStep.
	GateRole *string `json:"gate_role"` // The role responsible for this gate's underlying step, resolved through WorkflowStep → WorkflowSteps.AssignedRole. First hop of the CQ2 chain (gate → role → approver).
	GateApproverHuman *string `json:"gate_approver_human"` // The human agent who approves at this gate, resolved through the two-hop chain gate → GateRole → Roles.FilledByHumanAgent. Answers CQ2 ('who is responsible for approving a production deployment') directly: the release-approval gate resolves to the Release Manager role, filled by Maria Gonzalez.
	HasHumanApprover *bool `json:"has_human_approver"` // TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/approval-gates/" & {{ApprovalGateId}}
func (tc *ApprovalGate) CalcRelativePath() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.ParentPath)), vS("/approval-gates/"), erbTextOr(vStrPlain(tc.ApprovalGateId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ApprovalGate) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *ApprovalGate) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStr(tc.DisplayName)), vS(" "), vS("-")))
}

// CalcHasHumanApprover computes the HasHumanApprover calculated field
// TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads.
// Formula: =NOT(ISBLANK({{GateApproverHuman}}))
func (tc *ApprovalGate) CalcHasHumanApprover() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.GateApproverHuman)))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *ApprovalGate) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "has_human_approver", func() { tc.HasHumanApprover = tc.CalcHasHumanApprover() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *ApprovalGate) ComputeAll() *ApprovalGate {
	tc.erbComputeCalculations()
	return tc
}

func (tc *ApprovalGate) erbGet(field string) Value {
	switch field {
	case "approval_gate_id":
		return vStrPlain(tc.ApprovalGateId)
	case "parent_path":
		return vStr(tc.ParentPath)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStr(tc.DisplayName)
	case "workflow_step":
		return vStr(tc.WorkflowStep)
	case "escalation_threshold_hours":
		return vInt(tc.EscalationThresholdHours)
	case "gate_role":
		return vStr(tc.GateRole)
	case "gate_approver_human":
		return vStr(tc.GateApproverHuman)
	case "has_human_approver":
		return vBool(tc.HasHumanApprover)
	}
	panic("ApprovalGates has no field " + field)
}

func (tc *ApprovalGate) erbSet(field string, v Value) {
	switch field {
	case "approval_gate_id":
		tc.ApprovalGateId = strPlain(v)
	case "parent_path":
		tc.ParentPath = toStringPtr(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "workflow_step":
		tc.WorkflowStep = toStringPtr(v)
	case "escalation_threshold_hours":
		tc.EscalationThresholdHours = toIntPtr(v)
	case "gate_role":
		tc.GateRole = toStringPtr(v)
	case "gate_approver_human":
		tc.GateApproverHuman = toStringPtr(v)
	case "has_human_approver":
		tc.HasHumanApprover = toBoolPtr(v)
	default:
		panic("ApprovalGates has no field " + field)
	}
}

func (tc *ApprovalGate) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "approval_gate_id", "parent_path", "relative_path", "iri", "name", "display_name", "workflow_step", "escalation_threshold_hours", "gate_role", "gate_approver_human", "has_human_approver":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *ApprovalGate) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *ApprovalGate) erbResetErrors() { tc.ErbErrors = nil }

func (tc *ApprovalGate) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *ApprovalGate) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadApprovalGateRecords reads ApprovalGates rows from a JSON array file.
func LoadApprovalGateRecords(path string) ([]ApprovalGate, error) {
	records, err := loadRecords(path, func() Record { return &ApprovalGate{} })
	if err != nil {
		return nil, err
	}
	rows := make([]ApprovalGate, len(records))
	for i, r := range records {
		rows[i] = *r.(*ApprovalGate)
	}
	return rows, nil
}

// =============================================================================
// STEPPRECEDENCE TABLE
// Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
// =============================================================================

// StepPrecedence represents a row in the StepPrecedence table
type StepPrecedence struct {
	StepPrecedenceId string `json:"step_precedence_id"`
	ParentPath *string `json:"parent_path"` // Helper: the WorkflowSteps parent's RelativePath, pulled across the FromStep FK. Exists so RelativePath can concatenate the '/precedence/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
	FromStep string `json:"from_step"` // FK to the predecessor WorkflowStep — the step that comes BEFORE. The source of the ntwf:precedesStep edge. Inverse is WorkflowSteps.Precedes.
	ToStep string `json:"to_step"` // FK to the successor WorkflowStep — the step that comes AFTER. The target of the ntwf:precedesStep edge. Inverse is WorkflowSteps.PrecededBy.
	PrecedesStepClosure any `json:"precedes_step_closure"` // Transitive closure of ntwf:precedesStep (an owl:TransitiveProperty). The 4 asserted edges (1→2, 2→3, 3→4, 4→5) imply the full 10-pair ordering closure — including the never-asserted step-1 → step-5. Materialized by the transpiler as the view vw_step_precedence_closure(from_id, to_id, hop_distance, is_inferred): 4 asserted (hop 1) + 6 inferred rows. This is the article's headline inference made to fire, not seeded.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/precedence/" & {{StepPrecedenceId}}
func (tc *StepPrecedence) CalcRelativePath() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.ParentPath)), vS("/precedence/"), erbTextOr(vStrPlain(tc.StepPrecedenceId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *StepPrecedence) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
// Formula: ={{FromStep}} & " -> " & {{ToStep}}
func (tc *StepPrecedence) CalcName() *string {
	return toStringPtr(erbConcat(erbTextOr(vStrPlain(tc.FromStep)), vS(" -> "), erbTextOr(vStrPlain(tc.ToStep))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *StepPrecedence) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *StepPrecedence) ComputeAll() *StepPrecedence {
	tc.erbComputeCalculations()
	return tc
}

func (tc *StepPrecedence) erbGet(field string) Value {
	switch field {
	case "step_precedence_id":
		return vStrPlain(tc.StepPrecedenceId)
	case "parent_path":
		return vStr(tc.ParentPath)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "from_step":
		return vStrPlain(tc.FromStep)
	case "to_step":
		return vStrPlain(tc.ToStep)
	case "precedes_step_closure":
		return vAny(tc.PrecedesStepClosure)
	}
	panic("StepPrecedence has no field " + field)
}

func (tc *StepPrecedence) erbSet(field string, v Value) {
	switch field {
	case "step_precedence_id":
		tc.StepPrecedenceId = strPlain(v)
	case "parent_path":
		tc.ParentPath = toStringPtr(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "from_step":
		tc.FromStep = strPlain(v)
	case "to_step":
		tc.ToStep = strPlain(v)
	case "precedes_step_closure":
		tc.PrecedesStepClosure = anyPlain(v)
	default:
		panic("StepPrecedence has no field " + field)
	}
}

func (tc *StepPrecedence) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "step_precedence_id", "parent_path", "relative_path", "iri", "name", "from_step", "to_step", "precedes_step_closure":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *StepPrecedence) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *StepPrecedence) erbResetErrors() { tc.ErbErrors = nil }

func (tc *StepPrecedence) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *StepPrecedence) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadStepPrecedenceRecords reads StepPrecedence rows from a JSON array file.
func LoadStepPrecedenceRecords(path string) ([]StepPrecedence, error) {
	records, err := loadRecords(path, func() Record { return &StepPrecedence{} })
	if err != nil {
		return nil, err
	}
	rows := make([]StepPrecedence, len(records))
	for i, r := range records {
		rows[i] = *r.(*StepPrecedence)
	}
	return rows, nil
}

// =============================================================================
// ROLES TABLE
// Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
// =============================================================================

// Role represents a row in the Roles table
type Role struct {
	RoleId string `json:"role_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"`
	DisplayName *string `json:"display_name"`
	Label *string `json:"label"` // Human-readable display name. Maps to rdfs:label. Per Heuristic 6: if you cannot write a clear label, you do not yet understand the concept well enough to model it.
	Comment *string `json:"comment"` // Detailed description of the role's responsibilities and scope. Maps to rdfs:comment. Should define what the role covers, what it excludes, and how it differs from adjacent roles.
	HasCapability *string `json:"has_capability"` // FK to AgentCapabilityConcepts. Declares the capability this role requires of its filler. Maps to ntwf:hasCapability. Enables CQ: 'Which roles require AI-specific capabilities?'
	FilledByHumanAgent *string `json:"filled_by_human_agent"` // One arm of the polymorphic ntwf:filledBy relationship: FK to the HumanAgent that fills this role today. Exactly one of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline is set per role (filledBy is functional; the three agent types are owl:disjointWith each other). Roles whose capability requires human judgment or legal review must use this arm.
	FilledByAIAgent *string `json:"filled_by_ai_agent"` // One arm of the polymorphic ntwf:filledBy relationship: FK to the AIAgent that fills this role today. Exactly one filledBy arm is set per role. An AIAgent may fill probabilistic-capability roles (e.g. risk analysis) but never a role whose step has requiresHumanApproval.
	FilledByAutomatedPipeline *string `json:"filled_by_automated_pipeline"` // One arm of the polymorphic ntwf:filledBy relationship: FK to the AutomatedPipeline that fills this role today. Exactly one filledBy arm is set per role. Pipelines fill deterministic execution roles (e.g. CI/CD).
	OwnedBy *string `json:"owned_by"` // Foreign key to the Department that owns this role. Maps to ntwf:ownedBy (owl:FunctionalProperty). Enables answering CQ7: 'Which workflows involve both Engineering and Legal?'
	DelegatesTo *string `json:"delegates_to"` // Foreign key to the next Role in the escalation chain. Maps to ntwf:delegatesTo (traversable via the SPARQL property path delegatesTo+). Enables answering CQ6: 'What happens when the Release Manager / VP of Engineering is unavailable?'
	WorkflowSteps *string `json:"workflow_steps"` // Back-reference to workflow steps assigned to this role. Inverse of WorkflowSteps.AssignedRole.
	FromDelegatesTo *string `json:"from_delegates_to"` // Back-reference: the Role that delegates TO this role (one step up the escalation chain). Inverse of Roles.DelegatesTo.
	RoleAssignments *string `json:"role_assignments"` // Back-reference to the temporal filledBy history for this role (every validity period, current and retained). Inverse of RoleAssignments.Role.
	DelegationClosure any `json:"delegation_closure"` // Transitive closure of ntwf:delegatesTo over the self-referential DelegatesTo FK. The asserted escalation edges (Release Manager → VP Engineering, VP Engineering → CTO) imply the never-asserted reachability Release Manager → CTO. Materialized as vw_roles_closure(from_id, to_id, hop_distance, is_inferred). This is the SQL equivalent of the SPARQL delegatesTo+ property path.
	FilledByArmCount *int `json:"filled_by_arm_count"` // Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint.
	HasExactlyOneFiller *bool `json:"has_exactly_one_filler"` // Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
	FillerType *string `json:"filler_type"` // Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
	FillsApprovalGate *int `json:"fills_approval_gate"` // Number of this role's assigned WorkflowSteps that are approval gates (rollup over WorkflowSteps.IsApprovalGate). Greater than zero marks a role that owns a blocking decision checkpoint and therefore MUST have a complete delegatesTo escalation path — the precondition for EscalationViolation. Worked example: 1 for the Release Manager (who fills the Release Approval Gate), 0 for every other role.
	EscalationViolation *bool `json:"escalation_violation"` // Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="roles/" & {{RoleId}}
func (tc *Role) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("roles/"), erbTextOr(vStrPlain(tc.RoleId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Role) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Role) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStr(tc.DisplayName)), vS(" "), vS("-")))
}

// CalcFilledByArmCount computes the FilledByArmCount calculated field
// Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint.
// Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0)
func (tc *Role) CalcFilledByArmCount() *int {
	return toIntPtr(erbInteger(erbAdd(erbAdd(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByHumanAgent))))), func() Value { return vI(1) }, func() Value { return vI(0) }), erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByAIAgent))))), func() Value { return vI(1) }, func() Value { return vI(0) })), erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByAutomatedPipeline))))), func() Value { return vI(1) }, func() Value { return vI(0) }))))
}

// CalcHasExactlyOneFiller computes the HasExactlyOneFiller calculated field
// Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
// Formula: ={{FilledByArmCount}} = 1
func (tc *Role) CalcHasExactlyOneFiller() *bool {
	return toBoolPtr(erbEq(vInt(tc.FilledByArmCount), vI(1)))
}

// CalcFillerType computes the FillerType calculated field
// Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
// Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *Role) CalcFillerType() *string {
	return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByHumanAgent))))), func() Value { return vS("HumanAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByAIAgent))))), func() Value { return vS("AIAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByAutomatedPipeline))))), func() Value { return vS("AutomatedPipeline") }, func() Value { return vS("") }) }) }))
}

// CalcEscalationViolation computes the EscalationViolation calculated field
// Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
// Formula: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}}))
func (tc *Role) CalcEscalationViolation() *bool {
	return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.FillsApprovalGate), ">", vI(0))), erbBool3(erbIsBlank(vStr(tc.DelegatesTo)))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *Role) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "filled_by_arm_count", func() { tc.FilledByArmCount = tc.CalcFilledByArmCount() })
	calcGuard(tc, "filler_type", func() { tc.FillerType = tc.CalcFillerType() })
	calcGuard(tc, "escalation_violation", func() { tc.EscalationViolation = tc.CalcEscalationViolation() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
	calcGuard(tc, "has_exactly_one_filler", func() { tc.HasExactlyOneFiller = tc.CalcHasExactlyOneFiller() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *Role) ComputeAll() *Role {
	tc.erbComputeCalculations()
	return tc
}

func (tc *Role) erbGet(field string) Value {
	switch field {
	case "role_id":
		return vStrPlain(tc.RoleId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStr(tc.DisplayName)
	case "label":
		return vStr(tc.Label)
	case "comment":
		return vStr(tc.Comment)
	case "has_capability":
		return vStr(tc.HasCapability)
	case "filled_by_human_agent":
		return vStr(tc.FilledByHumanAgent)
	case "filled_by_ai_agent":
		return vStr(tc.FilledByAIAgent)
	case "filled_by_automated_pipeline":
		return vStr(tc.FilledByAutomatedPipeline)
	case "owned_by":
		return vStr(tc.OwnedBy)
	case "delegates_to":
		return vStr(tc.DelegatesTo)
	case "workflow_steps":
		return vStr(tc.WorkflowSteps)
	case "from_delegates_to":
		return vStr(tc.FromDelegatesTo)
	case "role_assignments":
		return vStr(tc.RoleAssignments)
	case "delegation_closure":
		return vAny(tc.DelegationClosure)
	case "filled_by_arm_count":
		return vInt(tc.FilledByArmCount)
	case "has_exactly_one_filler":
		return vBool(tc.HasExactlyOneFiller)
	case "filler_type":
		return vStr(tc.FillerType)
	case "fills_approval_gate":
		return vInt(tc.FillsApprovalGate)
	case "escalation_violation":
		return vBool(tc.EscalationViolation)
	}
	panic("Roles has no field " + field)
}

func (tc *Role) erbSet(field string, v Value) {
	switch field {
	case "role_id":
		tc.RoleId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "label":
		tc.Label = toStringPtr(v)
	case "comment":
		tc.Comment = toStringPtr(v)
	case "has_capability":
		tc.HasCapability = toStringPtr(v)
	case "filled_by_human_agent":
		tc.FilledByHumanAgent = toStringPtr(v)
	case "filled_by_ai_agent":
		tc.FilledByAIAgent = toStringPtr(v)
	case "filled_by_automated_pipeline":
		tc.FilledByAutomatedPipeline = toStringPtr(v)
	case "owned_by":
		tc.OwnedBy = toStringPtr(v)
	case "delegates_to":
		tc.DelegatesTo = toStringPtr(v)
	case "workflow_steps":
		tc.WorkflowSteps = toStringPtr(v)
	case "from_delegates_to":
		tc.FromDelegatesTo = toStringPtr(v)
	case "role_assignments":
		tc.RoleAssignments = toStringPtr(v)
	case "delegation_closure":
		tc.DelegationClosure = anyPlain(v)
	case "filled_by_arm_count":
		tc.FilledByArmCount = toIntPtr(v)
	case "has_exactly_one_filler":
		tc.HasExactlyOneFiller = toBoolPtr(v)
	case "filler_type":
		tc.FillerType = toStringPtr(v)
	case "fills_approval_gate":
		tc.FillsApprovalGate = toIntPtr(v)
	case "escalation_violation":
		tc.EscalationViolation = toBoolPtr(v)
	default:
		panic("Roles has no field " + field)
	}
}

func (tc *Role) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "role_id", "relative_path", "iri", "name", "display_name", "label", "comment", "has_capability", "filled_by_human_agent", "filled_by_ai_agent", "filled_by_automated_pipeline", "owned_by", "delegates_to", "workflow_steps", "from_delegates_to", "role_assignments", "delegation_closure", "filled_by_arm_count", "has_exactly_one_filler", "filler_type", "fills_approval_gate", "escalation_violation":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *Role) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *Role) erbResetErrors() { tc.ErbErrors = nil }

func (tc *Role) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *Role) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadRoleRecords reads Roles rows from a JSON array file.
func LoadRoleRecords(path string) ([]Role, error) {
	records, err := loadRecords(path, func() Record { return &Role{} })
	if err != nil {
		return nil, err
	}
	rows := make([]Role, len(records))
	for i, r := range records {
		rows[i] = *r.(*Role)
	}
	return rows, nil
}

// =============================================================================
// ROLEASSIGNMENTS TABLE
// Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
// =============================================================================

// RoleAssignment represents a row in the RoleAssignments table
type RoleAssignment struct {
	RoleAssignmentId string `json:"role_assignment_id"`
	ParentPath *string `json:"parent_path"` // Helper: the Roles parent's RelativePath, pulled across the Role FK. Exists so RelativePath can concatenate the '/assignments/' segment using only local-field '&' concat.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
	Name *string `json:"name"` // Human-readable label for this assignment period: the role and the validity window.
	Role string `json:"role"` // FK to the Role this assignment binds an agent to. The subject of the historical ntwf:filledBy triple.
	FilledByHumanAgent *string `json:"filled_by_human_agent"` // One arm of the polymorphic filledBy binding for this assignment period: FK to the HumanAgent who filled the role during this window. Exactly one filler arm is set per assignment.
	FilledByAIAgent *string `json:"filled_by_ai_agent"` // One arm of the polymorphic filledBy binding: FK to the AIAgent who filled the role during this window. Exactly one filler arm is set per assignment.
	FilledByAutomatedPipeline *string `json:"filled_by_automated_pipeline"` // One arm of the polymorphic filledBy binding: FK to the AutomatedPipeline that filled the role during this window. Exactly one filler arm is set per assignment.
	ValidFrom string `json:"valid_from"` // Start of the validity period for this filledBy binding (inclusive). A retained/versioned triple carries the validity period. ISO date.
	ValidTo *string `json:"valid_to"` // End of the validity period for this filledBy binding (exclusive). Blank means the binding is still current — this is the live ntwf:filledBy value mirrored on Roles. A non-blank value means the binding was superseded; the row is retained (not deleted) to preserve provenance.
	Reason *string `json:"reason"` // The WHY of the change: the audit record must reflect when that transition happened and why. e.g. 'initial assignment', 'departure / backfill', 'model upgrade', 'compliance reassignment to human'.
	PriorFillerType *string `json:"prior_filler_type"` // The agent class (HumanAgent / AIAgent / AutomatedPipeline) of the binding this assignment SUPERSEDED, or blank for the first assignment of a role. Lets the agent-type-change audit (AIAgent -> HumanAgent) be witnessed without re-deriving from the prior row.
	FillerType *string `json:"filler_type"` // Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
	IsCurrent *bool `json:"is_current"` // TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips.
	WasActiveAsOfAuditDate *bool `json:"was_active_as_of_audit_date"` // NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained.
	IsAgentTypeChange *bool `json:"is_agent_type_change"` // TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight.
	RequiresComplianceAudit *bool `json:"requires_compliance_audit"` // Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
// Formula: ={{ParentPath}} & "/assignments/" & {{RoleAssignmentId}}
func (tc *RoleAssignment) CalcRelativePath() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.ParentPath)), vS("/assignments/"), erbTextOr(vStrPlain(tc.RoleAssignmentId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *RoleAssignment) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Human-readable label for this assignment period: the role and the validity window.
// Formula: ={{Role}} & " [" & {{ValidFrom}} & " -> " & IF(ISBLANK({{ValidTo}}), "open", {{ValidTo}}) & "]"
func (tc *RoleAssignment) CalcName() *string {
	return toStringPtr(erbConcat(erbTextOr(vStrPlain(tc.Role)), vS(" ["), erbTextOr(vStrPlain(tc.ValidFrom)), vS(" -> "), erbTextNotNull(erbIf(erbBool3(erbIsBlank(vStr(tc.ValidTo))), func() Value { return vS("open") }, func() Value { return vStr(tc.ValidTo) })), vS("]")))
}

// CalcFillerType computes the FillerType calculated field
// Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
// Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *RoleAssignment) CalcFillerType() *string {
	return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByHumanAgent))))), func() Value { return vS("HumanAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByAIAgent))))), func() Value { return vS("AIAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.FilledByAutomatedPipeline))))), func() Value { return vS("AutomatedPipeline") }, func() Value { return vS("") }) }) }))
}

// CalcIsCurrent computes the IsCurrent calculated field
// TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips.
// Formula: =ISBLANK({{ValidTo}})
func (tc *RoleAssignment) CalcIsCurrent() *bool {
	return toBoolPtr(erbIsBlank(vStr(tc.ValidTo)))
}

// CalcWasActiveAsOfAuditDate computes the WasActiveAsOfAuditDate calculated field
// NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained.
// Formula: =AND({{ValidFrom}} <= "2026-03-01", OR(ISBLANK({{ValidTo}}), {{ValidTo}} > "2026-03-01"))
func (tc *RoleAssignment) CalcWasActiveAsOfAuditDate() *bool {
	return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vStrPlain(tc.ValidFrom)), "<=", vS("2026-03-01"))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.ValidTo))), erbBool3(erbCmp(erbNullif(vStr(tc.ValidTo)), ">", vS("2026-03-01")))))))
}

// CalcIsAgentTypeChange computes the IsAgentTypeChange calculated field
// TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight.
// Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} <> {{FillerType}})
func (tc *RoleAssignment) CalcIsAgentTypeChange() *bool {
	return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.PriorFillerType))))), erbBool3(erbNe(erbNullif(vStr(tc.PriorFillerType)), vStr(tc.FillerType)))))
}

// CalcRequiresComplianceAudit computes the RequiresComplianceAudit calculated field
// Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
// Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent")
func (tc *RoleAssignment) CalcRequiresComplianceAudit() *bool {
	return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.PriorFillerType))))), erbBool3(erbEq(erbNullif(vStr(tc.PriorFillerType)), vS("AIAgent"))), erbBool3(erbEq(vStr(tc.FillerType), vS("HumanAgent")))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *RoleAssignment) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "filler_type", func() { tc.FillerType = tc.CalcFillerType() })
	calcGuard(tc, "is_current", func() { tc.IsCurrent = tc.CalcIsCurrent() })
	calcGuard(tc, "was_active_as_of_audit_date", func() { tc.WasActiveAsOfAuditDate = tc.CalcWasActiveAsOfAuditDate() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
	calcGuard(tc, "is_agent_type_change", func() { tc.IsAgentTypeChange = tc.CalcIsAgentTypeChange() })
	calcGuard(tc, "requires_compliance_audit", func() { tc.RequiresComplianceAudit = tc.CalcRequiresComplianceAudit() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *RoleAssignment) ComputeAll() *RoleAssignment {
	tc.erbComputeCalculations()
	return tc
}

func (tc *RoleAssignment) erbGet(field string) Value {
	switch field {
	case "role_assignment_id":
		return vStrPlain(tc.RoleAssignmentId)
	case "parent_path":
		return vStr(tc.ParentPath)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "role":
		return vStrPlain(tc.Role)
	case "filled_by_human_agent":
		return vStr(tc.FilledByHumanAgent)
	case "filled_by_ai_agent":
		return vStr(tc.FilledByAIAgent)
	case "filled_by_automated_pipeline":
		return vStr(tc.FilledByAutomatedPipeline)
	case "valid_from":
		return vStrPlain(tc.ValidFrom)
	case "valid_to":
		return vStr(tc.ValidTo)
	case "reason":
		return vStr(tc.Reason)
	case "prior_filler_type":
		return vStr(tc.PriorFillerType)
	case "filler_type":
		return vStr(tc.FillerType)
	case "is_current":
		return vBool(tc.IsCurrent)
	case "was_active_as_of_audit_date":
		return vBool(tc.WasActiveAsOfAuditDate)
	case "is_agent_type_change":
		return vBool(tc.IsAgentTypeChange)
	case "requires_compliance_audit":
		return vBool(tc.RequiresComplianceAudit)
	}
	panic("RoleAssignments has no field " + field)
}

func (tc *RoleAssignment) erbSet(field string, v Value) {
	switch field {
	case "role_assignment_id":
		tc.RoleAssignmentId = strPlain(v)
	case "parent_path":
		tc.ParentPath = toStringPtr(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "role":
		tc.Role = strPlain(v)
	case "filled_by_human_agent":
		tc.FilledByHumanAgent = toStringPtr(v)
	case "filled_by_ai_agent":
		tc.FilledByAIAgent = toStringPtr(v)
	case "filled_by_automated_pipeline":
		tc.FilledByAutomatedPipeline = toStringPtr(v)
	case "valid_from":
		tc.ValidFrom = strPlain(v)
	case "valid_to":
		tc.ValidTo = toStringPtr(v)
	case "reason":
		tc.Reason = toStringPtr(v)
	case "prior_filler_type":
		tc.PriorFillerType = toStringPtr(v)
	case "filler_type":
		tc.FillerType = toStringPtr(v)
	case "is_current":
		tc.IsCurrent = toBoolPtr(v)
	case "was_active_as_of_audit_date":
		tc.WasActiveAsOfAuditDate = toBoolPtr(v)
	case "is_agent_type_change":
		tc.IsAgentTypeChange = toBoolPtr(v)
	case "requires_compliance_audit":
		tc.RequiresComplianceAudit = toBoolPtr(v)
	default:
		panic("RoleAssignments has no field " + field)
	}
}

func (tc *RoleAssignment) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "role_assignment_id", "parent_path", "relative_path", "iri", "name", "role", "filled_by_human_agent", "filled_by_ai_agent", "filled_by_automated_pipeline", "valid_from", "valid_to", "reason", "prior_filler_type", "filler_type", "is_current", "was_active_as_of_audit_date", "is_agent_type_change", "requires_compliance_audit":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *RoleAssignment) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *RoleAssignment) erbResetErrors() { tc.ErbErrors = nil }

func (tc *RoleAssignment) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *RoleAssignment) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadRoleAssignmentRecords reads RoleAssignments rows from a JSON array file.
func LoadRoleAssignmentRecords(path string) ([]RoleAssignment, error) {
	records, err := loadRecords(path, func() Record { return &RoleAssignment{} })
	if err != nil {
		return nil, err
	}
	rows := make([]RoleAssignment, len(records))
	for i, r := range records {
		rows[i] = *r.(*RoleAssignment)
	}
	return rows, nil
}

// =============================================================================
// DEPARTMENTS TABLE
// Table: Departments. The NTWF Department class — schema:Organization. First-class entity that enables cross-department intersection queries (CQ7: which workflows involve both Engineering and Legal?). Roles are ownedBy a department.
// =============================================================================

// Department represents a row in the Departments table
type Department struct {
	DepartmentId string `json:"department_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Human-readable display name of the department. Should match organizational terminology for stakeholder communication.
	Title *string `json:"title"` // Formal organizational title of the department. Maps to schema:name / dct:title.
	DisplayName *string `json:"display_name"` // Machine-friendly name for programmatic reference.
	Roles *string `json:"roles"` // Back-reference to roles owned by this department. Inverse of Roles.OwnedBy.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="departments/" & {{DepartmentId}}
func (tc *Department) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("departments/"), erbTextOr(vStrPlain(tc.DepartmentId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Department) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Human-readable display name of the department. Should match organizational terminology for stakeholder communication.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Department) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStr(tc.DisplayName)), vS(" "), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *Department) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *Department) ComputeAll() *Department {
	tc.erbComputeCalculations()
	return tc
}

func (tc *Department) erbGet(field string) Value {
	switch field {
	case "department_id":
		return vStrPlain(tc.DepartmentId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "title":
		return vStr(tc.Title)
	case "display_name":
		return vStr(tc.DisplayName)
	case "roles":
		return vStr(tc.Roles)
	}
	panic("Departments has no field " + field)
}

func (tc *Department) erbSet(field string, v Value) {
	switch field {
	case "department_id":
		tc.DepartmentId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "title":
		tc.Title = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "roles":
		tc.Roles = toStringPtr(v)
	default:
		panic("Departments has no field " + field)
	}
}

func (tc *Department) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "department_id", "relative_path", "iri", "name", "title", "display_name", "roles":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *Department) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *Department) erbResetErrors() { tc.ErbErrors = nil }

func (tc *Department) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *Department) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadDepartmentRecords reads Departments rows from a JSON array file.
func LoadDepartmentRecords(path string) ([]Department, error) {
	records, err := loadRecords(path, func() Record { return &Department{} })
	if err != nil {
		return nil, err
	}
	rows := make([]Department, len(records))
	for i, r := range records {
		rows[i] = *r.(*Department)
	}
	return rows, nil
}

// =============================================================================
// HUMANAGENTS TABLE
// Table: HumanAgents. The NTWF HumanAgent class — foaf:Person + prov:Agent. The only agent type permitted to fill roles whose step has requiresHumanApproval. Disjoint with AIAgent and AutomatedPipeline.
// =============================================================================

// HumanAgent represents a row in the HumanAgents table
type HumanAgent struct {
	HumanAgentId string `json:"human_agent_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Full name of the person. Maps to foaf:name. Note: FOAF's name property is appropriate for persons, not for software systems (which use schema:name).
	DisplayName *string `json:"display_name"`
	Mbox *string `json:"mbox"` // Email address of the person. Maps to foaf:mbox. Used for notifications and organizational directory integration.
	Roles *string `json:"roles"` // Back-reference to roles currently filled by this agent. Inverse of Roles.FilledByHumanAgent.
	RoleAssignments *string `json:"role_assignments"` // Back-reference to historical filledBy assignment periods in which this human filled a role. Inverse of RoleAssignments.FilledByHumanAgent.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="human-agents/" & {{HumanAgentId}}
func (tc *HumanAgent) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("human-agents/"), erbTextOr(vStrPlain(tc.HumanAgentId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *HumanAgent) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *HumanAgent) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *HumanAgent) ComputeAll() *HumanAgent {
	tc.erbComputeCalculations()
	return tc
}

func (tc *HumanAgent) erbGet(field string) Value {
	switch field {
	case "human_agent_id":
		return vStrPlain(tc.HumanAgentId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStr(tc.DisplayName)
	case "mbox":
		return vStr(tc.Mbox)
	case "roles":
		return vStr(tc.Roles)
	case "role_assignments":
		return vStr(tc.RoleAssignments)
	}
	panic("HumanAgents has no field " + field)
}

func (tc *HumanAgent) erbSet(field string, v Value) {
	switch field {
	case "human_agent_id":
		tc.HumanAgentId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "mbox":
		tc.Mbox = toStringPtr(v)
	case "roles":
		tc.Roles = toStringPtr(v)
	case "role_assignments":
		tc.RoleAssignments = toStringPtr(v)
	default:
		panic("HumanAgents has no field " + field)
	}
}

func (tc *HumanAgent) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "human_agent_id", "relative_path", "iri", "name", "display_name", "mbox", "roles", "role_assignments":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *HumanAgent) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *HumanAgent) erbResetErrors() { tc.ErbErrors = nil }

func (tc *HumanAgent) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *HumanAgent) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadHumanAgentRecords reads HumanAgents rows from a JSON array file.
func LoadHumanAgentRecords(path string) ([]HumanAgent, error) {
	records, err := loadRecords(path, func() Record { return &HumanAgent{} })
	if err != nil {
		return nil, err
	}
	rows := make([]HumanAgent, len(records))
	for i, r := range records {
		rows[i] = *r.(*HumanAgent)
	}
	return rows, nil
}

// =============================================================================
// AIAGENTS TABLE
// Table: AIAgents. The NTWF AIAgent class — prov:SoftwareAgent + ntwf:modelVersion. Distinguished from AutomatedPipeline by probabilistic (vs. deterministic) output semantics. Disjoint with HumanAgent and AutomatedPipeline. May never fill a role whose step has requiresHumanApproval.
// =============================================================================

// AIAgent represents a row in the AIAgents table
type AIAgent struct {
	AIAgentId string `json:"ai_agent_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Display name of the AI agent. Maps to schema:name (not foaf:name, which is for persons).
	Title *string `json:"title"` // Descriptive title of the AI agent's function.
	DisplayName *string `json:"display_name"`
	ModelVersion *string `json:"model_version"` // Version string of the AI model. Maps to ntwf:modelVersion. Makes AI-produced artifacts auditable at the version level. The domain declaration means this property applies only to AIAgent individuals.
	DeployedOn *string `json:"deployed_on"` // Deployment date of this AI model version. The NTWF graph doubles as an AI system registry: 'risk-classifier-v2.4.1 was deployed on 2026-01-10'. Sourced from the AI system registry feed via the shared Dublin Core contract (dct:date).
	Roles *string `json:"roles"` // Back-reference to roles currently filled by this AI agent. Inverse of Roles.FilledByAIAgent.
	RoleAssignments *string `json:"role_assignments"` // Back-reference to historical filledBy assignment periods filled by this AI agent. Inverse of RoleAssignments.FilledByAIAgent.
	AttributedArtifacts *string `json:"attributed_artifacts"` // Back-reference to WorkflowArtifacts attributed to this AI agent (prov:wasAttributedTo). Inverse of WorkflowArtifacts.AttributedToAIAgent. First leg of the 'blast radius' traversal.
	CountAttributedArtifacts *int `json:"count_attributed_artifacts"` // 'Blast radius', leg 1: how many artifacts are attributed to this AI agent (prov:wasAttributedTo). Counts WorkflowArtifacts whose AttributedToAIAgent is this agent.
	CountImpactedWorkflows *int `json:"count_impacted_workflows"` // 'Blast radius', summarized: the number of distinct workflows reachable from this agent's attributed artifacts (each artifact is produced by a step that belongs to a workflow). With one workflow in the worked example, an upgrade to an agent that produced any artifact has a blast radius of 1 workflow. Counts artifacts attributed to this agent that resolve to a workflow.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="ai-agents/" & {{AIAgentId}}
func (tc *AIAgent) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("ai-agents/"), erbTextOr(vStrPlain(tc.AIAgentId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *AIAgent) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *AIAgent) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *AIAgent) ComputeAll() *AIAgent {
	tc.erbComputeCalculations()
	return tc
}

func (tc *AIAgent) erbGet(field string) Value {
	switch field {
	case "ai_agent_id":
		return vStrPlain(tc.AIAgentId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "title":
		return vStr(tc.Title)
	case "display_name":
		return vStr(tc.DisplayName)
	case "model_version":
		return vStr(tc.ModelVersion)
	case "deployed_on":
		return vStr(tc.DeployedOn)
	case "roles":
		return vStr(tc.Roles)
	case "role_assignments":
		return vStr(tc.RoleAssignments)
	case "attributed_artifacts":
		return vStr(tc.AttributedArtifacts)
	case "count_attributed_artifacts":
		return vInt(tc.CountAttributedArtifacts)
	case "count_impacted_workflows":
		return vInt(tc.CountImpactedWorkflows)
	}
	panic("AIAgents has no field " + field)
}

func (tc *AIAgent) erbSet(field string, v Value) {
	switch field {
	case "ai_agent_id":
		tc.AIAgentId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "title":
		tc.Title = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "model_version":
		tc.ModelVersion = toStringPtr(v)
	case "deployed_on":
		tc.DeployedOn = toStringPtr(v)
	case "roles":
		tc.Roles = toStringPtr(v)
	case "role_assignments":
		tc.RoleAssignments = toStringPtr(v)
	case "attributed_artifacts":
		tc.AttributedArtifacts = toStringPtr(v)
	case "count_attributed_artifacts":
		tc.CountAttributedArtifacts = toIntPtr(v)
	case "count_impacted_workflows":
		tc.CountImpactedWorkflows = toIntPtr(v)
	default:
		panic("AIAgents has no field " + field)
	}
}

func (tc *AIAgent) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "ai_agent_id", "relative_path", "iri", "name", "title", "display_name", "model_version", "deployed_on", "roles", "role_assignments", "attributed_artifacts", "count_attributed_artifacts", "count_impacted_workflows":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *AIAgent) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *AIAgent) erbResetErrors() { tc.ErbErrors = nil }

func (tc *AIAgent) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *AIAgent) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadAIAgentRecords reads AIAgents rows from a JSON array file.
func LoadAIAgentRecords(path string) ([]AIAgent, error) {
	records, err := loadRecords(path, func() Record { return &AIAgent{} })
	if err != nil {
		return nil, err
	}
	rows := make([]AIAgent, len(records))
	for i, r := range records {
		rows[i] = *r.(*AIAgent)
	}
	return rows, nil
}

// =============================================================================
// AUTOMATEDPIPELINES TABLE
// Table: AutomatedPipelines. The NTWF AutomatedPipeline class — prov:SoftwareAgent + schema:SoftwareApplication. Distinguished from AIAgent by deterministic (vs. probabilistic) output semantics. Disjoint with HumanAgent and AIAgent. Carries schema:name, not foaf:name.
// =============================================================================

// AutomatedPipeline represents a row in the AutomatedPipelines table
type AutomatedPipeline struct {
	AutomatedPipelineId string `json:"automated_pipeline_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Display name of the pipeline. Maps to schema:name (appropriate for software systems, unlike foaf:name which is for persons).
	Description *string `json:"description"` // Description of what the pipeline does and its execution semantics (deterministic, no probabilistic output).
	DisplayName *string `json:"display_name"`
	Roles *string `json:"roles"` // Back-reference to roles currently filled by this pipeline. Inverse of Roles.FilledByAutomatedPipeline.
	RoleAssignments *string `json:"role_assignments"` // Back-reference to historical filledBy assignment periods filled by this pipeline. Inverse of RoleAssignments.FilledByAutomatedPipeline.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="automated-pipelines/" & {{AutomatedPipelineId}}
func (tc *AutomatedPipeline) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("automated-pipelines/"), erbTextOr(vStrPlain(tc.AutomatedPipelineId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *AutomatedPipeline) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *AutomatedPipeline) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *AutomatedPipeline) ComputeAll() *AutomatedPipeline {
	tc.erbComputeCalculations()
	return tc
}

func (tc *AutomatedPipeline) erbGet(field string) Value {
	switch field {
	case "automated_pipeline_id":
		return vStrPlain(tc.AutomatedPipelineId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "description":
		return vStr(tc.Description)
	case "display_name":
		return vStr(tc.DisplayName)
	case "roles":
		return vStr(tc.Roles)
	case "role_assignments":
		return vStr(tc.RoleAssignments)
	}
	panic("AutomatedPipelines has no field " + field)
}

func (tc *AutomatedPipeline) erbSet(field string, v Value) {
	switch field {
	case "automated_pipeline_id":
		tc.AutomatedPipelineId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "description":
		tc.Description = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "roles":
		tc.Roles = toStringPtr(v)
	case "role_assignments":
		tc.RoleAssignments = toStringPtr(v)
	default:
		panic("AutomatedPipelines has no field " + field)
	}
}

func (tc *AutomatedPipeline) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "automated_pipeline_id", "relative_path", "iri", "name", "description", "display_name", "roles", "role_assignments":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *AutomatedPipeline) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *AutomatedPipeline) erbResetErrors() { tc.ErbErrors = nil }

func (tc *AutomatedPipeline) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *AutomatedPipeline) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadAutomatedPipelineRecords reads AutomatedPipelines rows from a JSON array file.
func LoadAutomatedPipelineRecords(path string) ([]AutomatedPipeline, error) {
	records, err := loadRecords(path, func() Record { return &AutomatedPipeline{} })
	if err != nil {
		return nil, err
	}
	rows := make([]AutomatedPipeline, len(records))
	for i, r := range records {
		rows[i] = *r.(*AutomatedPipeline)
	}
	return rows, nil
}

// =============================================================================
// WORKFLOWSTATUSCONCEPTS TABLE
// SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows.
// =============================================================================

// WorkflowStatusConcept represents a row in the WorkflowStatusConcepts table
type WorkflowStatusConcept struct {
	ConceptId string `json:"concept_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	PrefLabel string `json:"pref_label"` // Preferred human-readable label. Maps to skos:prefLabel.
	AltLabel *string `json:"alt_label"` // Alternative label or synonym. Maps to skos:altLabel.
	Definition *string `json:"definition"` // Formal definition of the concept. Maps to skos:definition.
	ScopeNote *string `json:"scope_note"` // Usage guidance for the concept. Maps to skos:scopeNote.
	Workflows *string `json:"workflows"` // Back-reference to workflows currently in this status. Inverse of Workflows.WorkflowStatus.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="concepts/workflow-status/" & {{ConceptId}}
func (tc *WorkflowStatusConcept) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("concepts/workflow-status/"), erbTextOr(vStrPlain(tc.ConceptId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *WorkflowStatusConcept) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *WorkflowStatusConcept) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *WorkflowStatusConcept) ComputeAll() *WorkflowStatusConcept {
	tc.erbComputeCalculations()
	return tc
}

func (tc *WorkflowStatusConcept) erbGet(field string) Value {
	switch field {
	case "concept_id":
		return vStrPlain(tc.ConceptId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "pref_label":
		return vStrPlain(tc.PrefLabel)
	case "alt_label":
		return vStr(tc.AltLabel)
	case "definition":
		return vStr(tc.Definition)
	case "scope_note":
		return vStr(tc.ScopeNote)
	case "workflows":
		return vStr(tc.Workflows)
	}
	panic("WorkflowStatusConcepts has no field " + field)
}

func (tc *WorkflowStatusConcept) erbSet(field string, v Value) {
	switch field {
	case "concept_id":
		tc.ConceptId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "pref_label":
		tc.PrefLabel = strPlain(v)
	case "alt_label":
		tc.AltLabel = toStringPtr(v)
	case "definition":
		tc.Definition = toStringPtr(v)
	case "scope_note":
		tc.ScopeNote = toStringPtr(v)
	case "workflows":
		tc.Workflows = toStringPtr(v)
	default:
		panic("WorkflowStatusConcepts has no field " + field)
	}
}

func (tc *WorkflowStatusConcept) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "concept_id", "relative_path", "iri", "pref_label", "alt_label", "definition", "scope_note", "workflows":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *WorkflowStatusConcept) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *WorkflowStatusConcept) erbResetErrors() { tc.ErbErrors = nil }

func (tc *WorkflowStatusConcept) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *WorkflowStatusConcept) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadWorkflowStatusConceptRecords reads WorkflowStatusConcepts rows from a JSON array file.
func LoadWorkflowStatusConceptRecords(path string) ([]WorkflowStatusConcept, error) {
	records, err := loadRecords(path, func() Record { return &WorkflowStatusConcept{} })
	if err != nil {
		return nil, err
	}
	rows := make([]WorkflowStatusConcept, len(records))
	for i, r := range records {
		rows[i] = *r.(*WorkflowStatusConcept)
	}
	return rows, nil
}

// =============================================================================
// AGENTCAPABILITYCONCEPTS TABLE
// SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox.
// =============================================================================

// AgentCapabilityConcept represents a row in the AgentCapabilityConcepts table
type AgentCapabilityConcept struct {
	ConceptId string `json:"concept_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	PrefLabel string `json:"pref_label"` // Preferred label. Maps to skos:prefLabel.
	AltLabel *string `json:"alt_label"` // Alternative label. Maps to skos:altLabel.
	Definition *string `json:"definition"` // Formal definition. Maps to skos:definition.
	ScopeNote *string `json:"scope_note"` // Usage guidance. Maps to skos:scopeNote.
	Roles *string `json:"roles"` // Back-reference to roles requiring this capability. Inverse of Roles.HasCapability.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="concepts/agent-capability/" & {{ConceptId}}
func (tc *AgentCapabilityConcept) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("concepts/agent-capability/"), erbTextOr(vStrPlain(tc.ConceptId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *AgentCapabilityConcept) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *AgentCapabilityConcept) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *AgentCapabilityConcept) ComputeAll() *AgentCapabilityConcept {
	tc.erbComputeCalculations()
	return tc
}

func (tc *AgentCapabilityConcept) erbGet(field string) Value {
	switch field {
	case "concept_id":
		return vStrPlain(tc.ConceptId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "pref_label":
		return vStrPlain(tc.PrefLabel)
	case "alt_label":
		return vStr(tc.AltLabel)
	case "definition":
		return vStr(tc.Definition)
	case "scope_note":
		return vStr(tc.ScopeNote)
	case "roles":
		return vStr(tc.Roles)
	}
	panic("AgentCapabilityConcepts has no field " + field)
}

func (tc *AgentCapabilityConcept) erbSet(field string, v Value) {
	switch field {
	case "concept_id":
		tc.ConceptId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "pref_label":
		tc.PrefLabel = strPlain(v)
	case "alt_label":
		tc.AltLabel = toStringPtr(v)
	case "definition":
		tc.Definition = toStringPtr(v)
	case "scope_note":
		tc.ScopeNote = toStringPtr(v)
	case "roles":
		tc.Roles = toStringPtr(v)
	default:
		panic("AgentCapabilityConcepts has no field " + field)
	}
}

func (tc *AgentCapabilityConcept) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "concept_id", "relative_path", "iri", "pref_label", "alt_label", "definition", "scope_note", "roles":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *AgentCapabilityConcept) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *AgentCapabilityConcept) erbResetErrors() { tc.ErbErrors = nil }

func (tc *AgentCapabilityConcept) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *AgentCapabilityConcept) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadAgentCapabilityConceptRecords reads AgentCapabilityConcepts rows from a JSON array file.
func LoadAgentCapabilityConceptRecords(path string) ([]AgentCapabilityConcept, error) {
	records, err := loadRecords(path, func() Record { return &AgentCapabilityConcept{} })
	if err != nil {
		return nil, err
	}
	rows := make([]AgentCapabilityConcept, len(records))
	for i, r := range records {
		rows[i] = *r.(*AgentCapabilityConcept)
	}
	return rows, nil
}

// =============================================================================
// ARTIFACTTYPECONCEPTS TABLE
// SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts.
// =============================================================================

// ArtifactTypeConcept represents a row in the ArtifactTypeConcepts table
type ArtifactTypeConcept struct {
	ConceptId string `json:"concept_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
	PrefLabel string `json:"pref_label"` // Preferred human-readable label. Maps to skos:prefLabel.
	AltLabel *string `json:"alt_label"` // Alternative label or synonym. Maps to skos:altLabel.
	Definition *string `json:"definition"` // Formal definition of the concept. Maps to skos:definition.
	ScopeNote *string `json:"scope_note"` // Usage note clarifying boundaries. Maps to skos:scopeNote.
	WorkflowArtifacts *string `json:"workflow_artifacts"` // Back-reference to WorkflowArtifacts classified under this concept. Inverse of WorkflowArtifacts.ArtifactType.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key.
// Formula: ="concepts/artifact-type/" & {{ConceptId}}
func (tc *ArtifactTypeConcept) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("concepts/artifact-type/"), erbTextOr(vStrPlain(tc.ConceptId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ArtifactTypeConcept) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *ArtifactTypeConcept) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *ArtifactTypeConcept) ComputeAll() *ArtifactTypeConcept {
	tc.erbComputeCalculations()
	return tc
}

func (tc *ArtifactTypeConcept) erbGet(field string) Value {
	switch field {
	case "concept_id":
		return vStrPlain(tc.ConceptId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "pref_label":
		return vStrPlain(tc.PrefLabel)
	case "alt_label":
		return vStr(tc.AltLabel)
	case "definition":
		return vStr(tc.Definition)
	case "scope_note":
		return vStr(tc.ScopeNote)
	case "workflow_artifacts":
		return vStr(tc.WorkflowArtifacts)
	}
	panic("ArtifactTypeConcepts has no field " + field)
}

func (tc *ArtifactTypeConcept) erbSet(field string, v Value) {
	switch field {
	case "concept_id":
		tc.ConceptId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "pref_label":
		tc.PrefLabel = strPlain(v)
	case "alt_label":
		tc.AltLabel = toStringPtr(v)
	case "definition":
		tc.Definition = toStringPtr(v)
	case "scope_note":
		tc.ScopeNote = toStringPtr(v)
	case "workflow_artifacts":
		tc.WorkflowArtifacts = toStringPtr(v)
	default:
		panic("ArtifactTypeConcepts has no field " + field)
	}
}

func (tc *ArtifactTypeConcept) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "concept_id", "relative_path", "iri", "pref_label", "alt_label", "definition", "scope_note", "workflow_artifacts":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *ArtifactTypeConcept) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *ArtifactTypeConcept) erbResetErrors() { tc.ErbErrors = nil }

func (tc *ArtifactTypeConcept) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *ArtifactTypeConcept) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadArtifactTypeConceptRecords reads ArtifactTypeConcepts rows from a JSON array file.
func LoadArtifactTypeConceptRecords(path string) ([]ArtifactTypeConcept, error) {
	records, err := loadRecords(path, func() Record { return &ArtifactTypeConcept{} })
	if err != nil {
		return nil, err
	}
	rows := make([]ArtifactTypeConcept, len(records))
	for i, r := range records {
		rows[i] = *r.(*ArtifactTypeConcept)
	}
	return rows, nil
}

// =============================================================================
// DATASETS TABLE
// DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
// =============================================================================

// Dataset represents a row in the Datasets table
type Dataset struct {
	DatasetId string `json:"dataset_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Title string `json:"title"` // Human-readable dataset name. Maps to dct:title.
	Identifier *string `json:"identifier"` // External system identifier. Maps to dct:identifier. Used for cross-referencing with data catalogs.
	Modified *string `json:"modified"` // Last modification timestamp. Maps to dct:modified.
	DistributionUrl *string `json:"distribution_url"` // URL of the data distribution. Maps to dcat:Distribution. The access endpoint for the dataset.
	ConsumedBySteps *string `json:"consumed_by_steps"` // Back-reference to WorkflowSteps that consume this dataset. Inverse of WorkflowSteps.ConsumesDataset. Marked isReversed so every substrate DERIVES it from the forward FK (a reverse lookup over WorkflowSteps.ConsumesDataset) instead of storing it — keeping the two sides from drifting when the forward FK is edited.
	IsConsumed *bool `json:"is_consumed"` // TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="datasets/" & {{DatasetId}}
func (tc *Dataset) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("datasets/"), erbTextOr(vStrPlain(tc.DatasetId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Dataset) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcIsConsumed computes the IsConsumed calculated field
// TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads.
// Formula: =NOT(ISBLANK({{ConsumedBySteps}}))
func (tc *Dataset) CalcIsConsumed() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.ConsumedBySteps)))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *Dataset) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "is_consumed", func() { tc.IsConsumed = tc.CalcIsConsumed() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *Dataset) ComputeAll() *Dataset {
	tc.erbComputeCalculations()
	return tc
}

func (tc *Dataset) erbGet(field string) Value {
	switch field {
	case "dataset_id":
		return vStrPlain(tc.DatasetId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "title":
		return vStrPlain(tc.Title)
	case "identifier":
		return vStr(tc.Identifier)
	case "modified":
		return vStr(tc.Modified)
	case "distribution_url":
		return vStr(tc.DistributionUrl)
	case "consumed_by_steps":
		return vStr(tc.ConsumedBySteps)
	case "is_consumed":
		return vBool(tc.IsConsumed)
	}
	panic("Datasets has no field " + field)
}

func (tc *Dataset) erbSet(field string, v Value) {
	switch field {
	case "dataset_id":
		tc.DatasetId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "title":
		tc.Title = strPlain(v)
	case "identifier":
		tc.Identifier = toStringPtr(v)
	case "modified":
		tc.Modified = toStringPtr(v)
	case "distribution_url":
		tc.DistributionUrl = toStringPtr(v)
	case "consumed_by_steps":
		tc.ConsumedBySteps = toStringPtr(v)
	case "is_consumed":
		tc.IsConsumed = toBoolPtr(v)
	default:
		panic("Datasets has no field " + field)
	}
}

func (tc *Dataset) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "dataset_id", "relative_path", "iri", "title", "identifier", "modified", "distribution_url", "consumed_by_steps", "is_consumed":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *Dataset) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *Dataset) erbResetErrors() { tc.ErbErrors = nil }

func (tc *Dataset) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *Dataset) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadDatasetRecords reads Datasets rows from a JSON array file.
func LoadDatasetRecords(path string) ([]Dataset, error) {
	records, err := loadRecords(path, func() Record { return &Dataset{} })
	if err != nil {
		return nil, err
	}
	rows := make([]Dataset, len(records))
	for i, r := range records {
		rows[i] = *r.(*Dataset)
	}
	return rows, nil
}

// =============================================================================
// WORKFLOWARTIFACTS TABLE
// Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
// =============================================================================

// WorkflowArtifact represents a row in the WorkflowArtifacts table
type WorkflowArtifact struct {
	ArtifactId string `json:"artifact_id"`
	ParentPath *string `json:"parent_path"` // Helper: the WorkflowSteps parent's RelativePath, pulled across the ProducedByStep FK. Exists so RelativePath can concatenate the '/artifacts/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Title string `json:"title"` // Human-readable artifact name. Maps to dct:title.
	Identifier *string `json:"identifier"` // External system identifier. Maps to dct:identifier.
	ArtifactType *string `json:"artifact_type"` // FK to the ArtifactTypeConcepts SKOS concept classifying this artifact. Maps to dct:type. The CBox defines a concept scheme for artifact types.
	Created *string `json:"created"` // Creation timestamp. Maps to dct:created.
	ProducedByStep *string `json:"produced_by_step"` // FK to the WorkflowStep that produced this artifact. Maps to prov:wasGeneratedBy. Inverse of WorkflowSteps.ProducesArtifacts.
	RequiredBySteps *string `json:"required_by_steps"` // Back-reference to the WorkflowStep(s) that consume this artifact as input (ntwf:requiresArtifact / prov:used). Inverse of WorkflowSteps.RequiresArtifacts.
	DerivedFromArtifact *string `json:"derived_from_artifact"` // Self-FK to the artifact this one was derived from. Maps to prov:wasDerivedFrom. Enables the full provenance chain query (CQ4).
	AttributedToHumanAgent *string `json:"attributed_to_human_agent"` // FK to HumanAgent responsible for this artifact. One arm of prov:wasAttributedTo (exactly one AttributedTo arm is set per artifact, mirroring the disjoint agent types).
	AttributedToAIAgent *string `json:"attributed_to_ai_agent"` // FK to AIAgent responsible for this artifact. One arm of prov:wasAttributedTo.
	AttributedToAutomatedPipeline *string `json:"attributed_to_automated_pipeline"` // FK to AutomatedPipeline responsible for this artifact. One arm of prov:wasAttributedTo.
	ProducingAgentType *string `json:"producing_agent_type"` // Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from.
	HasDerivationParent *bool `json:"has_derivation_parent"` // TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
	ProducedByWorkflow *string `json:"produced_by_workflow"` // The workflow this artifact belongs to, resolved through ProducedByStep → WorkflowSteps.Workflow (artifact → producing step → workflow). Lets workflow-level rollups (e.g. CountDerivationLinks) aggregate artifacts without a redundant direct FK.
	HasProducingWorkflow *bool `json:"has_producing_workflow"` // TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
	DerivationClosure any `json:"derivation_closure"` // Transitive closure of prov:wasDerivedFrom over the self-referential DerivedFromArtifact FK. The asserted single-step derivation edges (Legal Clearance was derived from Risk Report, Release Authorization from Legal Clearance, …) imply the never-asserted reachability (Post-Deployment Report transitively wasDerivedFrom Risk Report). Materialized as vw_workflow_artifacts_closure(from_id, to_id, hop_distance, is_inferred). This is the artifact-lineage analogue of vw_step_precedence_closure and vw_roles_closure — the SAME closure construct as step ordering and role escalation, just over a different relation, so a broken link surfaces as a missing reachability pair exactly like a dropped precedence edge.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/artifacts/" & {{ArtifactId}}
func (tc *WorkflowArtifact) CalcRelativePath() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.ParentPath)), vS("/artifacts/"), erbTextOr(vStrPlain(tc.ArtifactId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *WorkflowArtifact) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcProducingAgentType computes the ProducingAgentType calculated field
// Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from.
// Formula: =IF(NOT(ISBLANK({{AttributedToHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{AttributedToAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{AttributedToAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *WorkflowArtifact) CalcProducingAgentType() *string {
	return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.AttributedToHumanAgent))))), func() Value { return vS("HumanAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.AttributedToAIAgent))))), func() Value { return vS("AIAgent") }, func() Value { return erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.AttributedToAutomatedPipeline))))), func() Value { return vS("AutomatedPipeline") }, func() Value { return vS("") }) }) }))
}

// CalcHasDerivationParent computes the HasDerivationParent calculated field
// TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
// Formula: =NOT(ISBLANK({{DerivedFromArtifact}}))
func (tc *WorkflowArtifact) CalcHasDerivationParent() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.DerivedFromArtifact)))))
}

// CalcHasProducingWorkflow computes the HasProducingWorkflow calculated field
// TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
// Formula: =NOT(ISBLANK({{ProducedByWorkflow}}))
func (tc *WorkflowArtifact) CalcHasProducingWorkflow() *bool {
	return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.ProducedByWorkflow)))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *WorkflowArtifact) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "producing_agent_type", func() { tc.ProducingAgentType = tc.CalcProducingAgentType() })
	calcGuard(tc, "has_derivation_parent", func() { tc.HasDerivationParent = tc.CalcHasDerivationParent() })
	calcGuard(tc, "has_producing_workflow", func() { tc.HasProducingWorkflow = tc.CalcHasProducingWorkflow() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *WorkflowArtifact) ComputeAll() *WorkflowArtifact {
	tc.erbComputeCalculations()
	return tc
}

func (tc *WorkflowArtifact) erbGet(field string) Value {
	switch field {
	case "artifact_id":
		return vStrPlain(tc.ArtifactId)
	case "parent_path":
		return vStr(tc.ParentPath)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "title":
		return vStrPlain(tc.Title)
	case "identifier":
		return vStr(tc.Identifier)
	case "artifact_type":
		return vStr(tc.ArtifactType)
	case "created":
		return vStr(tc.Created)
	case "produced_by_step":
		return vStr(tc.ProducedByStep)
	case "required_by_steps":
		return vStr(tc.RequiredBySteps)
	case "derived_from_artifact":
		return vStr(tc.DerivedFromArtifact)
	case "attributed_to_human_agent":
		return vStr(tc.AttributedToHumanAgent)
	case "attributed_to_ai_agent":
		return vStr(tc.AttributedToAIAgent)
	case "attributed_to_automated_pipeline":
		return vStr(tc.AttributedToAutomatedPipeline)
	case "producing_agent_type":
		return vStr(tc.ProducingAgentType)
	case "has_derivation_parent":
		return vBool(tc.HasDerivationParent)
	case "produced_by_workflow":
		return vStr(tc.ProducedByWorkflow)
	case "has_producing_workflow":
		return vBool(tc.HasProducingWorkflow)
	case "derivation_closure":
		return vAny(tc.DerivationClosure)
	}
	panic("WorkflowArtifacts has no field " + field)
}

func (tc *WorkflowArtifact) erbSet(field string, v Value) {
	switch field {
	case "artifact_id":
		tc.ArtifactId = strPlain(v)
	case "parent_path":
		tc.ParentPath = toStringPtr(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "title":
		tc.Title = strPlain(v)
	case "identifier":
		tc.Identifier = toStringPtr(v)
	case "artifact_type":
		tc.ArtifactType = toStringPtr(v)
	case "created":
		tc.Created = toStringPtr(v)
	case "produced_by_step":
		tc.ProducedByStep = toStringPtr(v)
	case "required_by_steps":
		tc.RequiredBySteps = toStringPtr(v)
	case "derived_from_artifact":
		tc.DerivedFromArtifact = toStringPtr(v)
	case "attributed_to_human_agent":
		tc.AttributedToHumanAgent = toStringPtr(v)
	case "attributed_to_ai_agent":
		tc.AttributedToAIAgent = toStringPtr(v)
	case "attributed_to_automated_pipeline":
		tc.AttributedToAutomatedPipeline = toStringPtr(v)
	case "producing_agent_type":
		tc.ProducingAgentType = toStringPtr(v)
	case "has_derivation_parent":
		tc.HasDerivationParent = toBoolPtr(v)
	case "produced_by_workflow":
		tc.ProducedByWorkflow = toStringPtr(v)
	case "has_producing_workflow":
		tc.HasProducingWorkflow = toBoolPtr(v)
	case "derivation_closure":
		tc.DerivationClosure = anyPlain(v)
	default:
		panic("WorkflowArtifacts has no field " + field)
	}
}

func (tc *WorkflowArtifact) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "artifact_id", "parent_path", "relative_path", "iri", "title", "identifier", "artifact_type", "created", "produced_by_step", "required_by_steps", "derived_from_artifact", "attributed_to_human_agent", "attributed_to_ai_agent", "attributed_to_automated_pipeline", "producing_agent_type", "has_derivation_parent", "produced_by_workflow", "has_producing_workflow", "derivation_closure":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *WorkflowArtifact) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *WorkflowArtifact) erbResetErrors() { tc.ErbErrors = nil }

func (tc *WorkflowArtifact) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *WorkflowArtifact) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadWorkflowArtifactRecords reads WorkflowArtifacts rows from a JSON array file.
func LoadWorkflowArtifactRecords(path string) ([]WorkflowArtifact, error) {
	records, err := loadRecords(path, func() Record { return &WorkflowArtifact{} })
	if err != nil {
		return nil, err
	}
	rows := make([]WorkflowArtifact, len(records))
	for i, r := range records {
		rows[i] = *r.(*WorkflowArtifact)
	}
	return rows, nil
}

// =============================================================================
// GOVERNANCEROLES TABLE
// Table: GovernanceRoles. NTWF governance names two distinct ontology-governance roles: a Steward (responsible for the ontology's health — monitors drift, tracks external dependency updates, fields user questions, maintains docs, keeps the validation suite current; identifies that a change is needed but has no approval power) and an Authority (the power to approve changes to the CBox, ABox, and TBox; decides how and where a change is made; sits with the function that owns the domain). 'A steward who can make TBox or ABox changes without authority review is a single point of failure.' For an organization under 500 people a single person may hold both roles. This table models the maintenance discipline itself, as data, so the change log can attribute approvals to a named authority.
// =============================================================================

// GovernanceRole represents a row in the GovernanceRoles table
type GovernanceRole struct {
	GovernanceRoleId string `json:"governance_role_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
	Name *string `json:"name"` // Slug form of the display name.
	DisplayName *string `json:"display_name"` // Human-readable name of the governance role (e.g. 'Steward', 'Authority').
	Kind *string `json:"kind"` // Which of the two NTWF governance kinds this is: 'Steward' or 'Authority'.
	Responsibilities *string `json:"responsibilities"` // What this role is responsible for. Steward: monitor drift, track external dependency updates, field user questions, maintain documentation, keep the validation suite current. Authority: approve changes to CBox/ABox/TBox; decide how and where a change is made.
	ApprovalScope *string `json:"approval_scope"` // The boxes this role may approve changes to (CBox/ABox/TBox), or 'none' for a Steward — who can identify that a change is needed but cannot approve it.
	HeldBy *string `json:"held_by"` // The person or function holding this role. The steward is naturally whoever owns the engineering knowledge infrastructure; authority sits with the workflow governance function that owns the modeled domain. Under 500 people, one person may hold both.
	CanApproveChanges *bool `json:"can_approve_changes"` // TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure.
	ApprovedChanges *string `json:"approved_changes"` // Back-reference to ChangeLog entries this governance role approved. Inverse of ChangeLog.ApprovedBy.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key.
// Formula: ="governance-roles/" & {{GovernanceRoleId}}
func (tc *GovernanceRole) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("governance-roles/"), erbTextOr(vStrPlain(tc.GovernanceRoleId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *GovernanceRole) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Slug form of the display name.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *GovernanceRole) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStr(tc.DisplayName)), vS(" "), vS("-")))
}

// CalcCanApproveChanges computes the CanApproveChanges calculated field
// TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure.
// Formula: ={{Kind}} = "Authority"
func (tc *GovernanceRole) CalcCanApproveChanges() *bool {
	return toBoolPtr(erbEq(erbNullif(vStr(tc.Kind)), vS("Authority")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *GovernanceRole) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "can_approve_changes", func() { tc.CanApproveChanges = tc.CalcCanApproveChanges() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *GovernanceRole) ComputeAll() *GovernanceRole {
	tc.erbComputeCalculations()
	return tc
}

func (tc *GovernanceRole) erbGet(field string) Value {
	switch field {
	case "governance_role_id":
		return vStrPlain(tc.GovernanceRoleId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStr(tc.DisplayName)
	case "kind":
		return vStr(tc.Kind)
	case "responsibilities":
		return vStr(tc.Responsibilities)
	case "approval_scope":
		return vStr(tc.ApprovalScope)
	case "held_by":
		return vStr(tc.HeldBy)
	case "can_approve_changes":
		return vBool(tc.CanApproveChanges)
	case "approved_changes":
		return vStr(tc.ApprovedChanges)
	}
	panic("GovernanceRoles has no field " + field)
}

func (tc *GovernanceRole) erbSet(field string, v Value) {
	switch field {
	case "governance_role_id":
		tc.GovernanceRoleId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = toStringPtr(v)
	case "kind":
		tc.Kind = toStringPtr(v)
	case "responsibilities":
		tc.Responsibilities = toStringPtr(v)
	case "approval_scope":
		tc.ApprovalScope = toStringPtr(v)
	case "held_by":
		tc.HeldBy = toStringPtr(v)
	case "can_approve_changes":
		tc.CanApproveChanges = toBoolPtr(v)
	case "approved_changes":
		tc.ApprovedChanges = toStringPtr(v)
	default:
		panic("GovernanceRoles has no field " + field)
	}
}

func (tc *GovernanceRole) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "governance_role_id", "relative_path", "iri", "name", "display_name", "kind", "responsibilities", "approval_scope", "held_by", "can_approve_changes", "approved_changes":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *GovernanceRole) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *GovernanceRole) erbResetErrors() { tc.ErbErrors = nil }

func (tc *GovernanceRole) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *GovernanceRole) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadGovernanceRoleRecords reads GovernanceRoles rows from a JSON array file.
func LoadGovernanceRoleRecords(path string) ([]GovernanceRole, error) {
	records, err := loadRecords(path, func() Record { return &GovernanceRole{} })
	if err != nil {
		return nil, err
	}
	rows := make([]GovernanceRole, len(records))
	for i, r := range records {
		rows[i] = *r.(*GovernanceRole)
	}
	return rows, nil
}

// =============================================================================
// CHANGELOG TABLE
// Table: ChangeLog. NTWF's minimum governance artifact: 'a change log that records every TBox and ABox modification, with its rationale.' Each entry records the four facts NTWF governance enumerates — the competency question that motivated the change, the terms affected, the version number of the release, and the date — plus the rationale and the Authority who approved it. Semantic-versioning discipline (MAJOR.MINOR.PATCH) is captured per entry via ChangeKind.
// =============================================================================

// ChangeLog represents a row in the ChangeLog table
type ChangeLog struct {
	ChangeLogId string `json:"change_log_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath).
	Name *string `json:"name"` // Human-readable label: the version and date of this change.
	Version *string `json:"version"` // The release version number this change shipped in (semantic versioning MAJOR.MINOR.PATCH). NTWF is currently at 1.1.0.
	ChangeDate *string `json:"change_date"` // The date of the change. One of the four facts NTWF governance requires every change-log entry to record.
	ChangeKind *string `json:"change_kind"` // Semantic-versioning class of the change: 'patch' (documentation/label/comment only, formal model unchanged), 'minor' (additive — new classes/properties/CBox concepts, backward compatible), or 'major' (breaking — class removed/renamed, domain/range change invalidating ABox triples, or a new disjointness axiom).
	MotivatingQuestion *string `json:"motivating_question"` // The competency question that motivated the change. One of the four facts NTWF governance requires. Empty if the change was driven by an external-dependency update rather than a CQ.
	TermsAffected *string `json:"terms_affected"` // The ontology terms (classes/properties/concepts) the change added, removed, or modified. One of the four facts NTWF governance requires.
	Rationale *string `json:"rationale"` // Why the change was made. NTWF governance requires every TBox/ABox modification to be logged with its rationale.
	ApprovedBy *string `json:"approved_by"` // FK to the GovernanceRole (an Authority) that approved this change. Changes to CBox/ABox/TBox require authority review; a steward identifying a need is not enough.
	IsBreakingChange *bool `json:"is_breaking_change"` // TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version.
	IsBackwardCompatible *bool `json:"is_backward_compatible"` // TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key.
// Formula: ="change-log/" & {{ChangeLogId}}
func (tc *ChangeLog) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("change-log/"), erbTextOr(vStrPlain(tc.ChangeLogId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ChangeLog) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Human-readable label: the version and date of this change.
// Formula: ={{Version}} & " (" & {{ChangeDate}} & ")"
func (tc *ChangeLog) CalcName() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.Version)), vS(" ("), erbTextOr(vStr(tc.ChangeDate)), vS(")")))
}

// CalcIsBreakingChange computes the IsBreakingChange calculated field
// TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version.
// Formula: ={{ChangeKind}} = "major"
func (tc *ChangeLog) CalcIsBreakingChange() *bool {
	return toBoolPtr(erbEq(erbNullif(vStr(tc.ChangeKind)), vS("major")))
}

// CalcIsBackwardCompatible computes the IsBackwardCompatible calculated field
// TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
// Formula: =OR({{ChangeKind}} = "patch", {{ChangeKind}} = "minor")
func (tc *ChangeLog) CalcIsBackwardCompatible() *bool {
	return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.ChangeKind)), vS("patch"))), erbBool3(erbEq(erbNullif(vStr(tc.ChangeKind)), vS("minor")))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *ChangeLog) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	calcGuard(tc, "is_breaking_change", func() { tc.IsBreakingChange = tc.CalcIsBreakingChange() })
	calcGuard(tc, "is_backward_compatible", func() { tc.IsBackwardCompatible = tc.CalcIsBackwardCompatible() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *ChangeLog) ComputeAll() *ChangeLog {
	tc.erbComputeCalculations()
	return tc
}

func (tc *ChangeLog) erbGet(field string) Value {
	switch field {
	case "change_log_id":
		return vStrPlain(tc.ChangeLogId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "version":
		return vStr(tc.Version)
	case "change_date":
		return vStr(tc.ChangeDate)
	case "change_kind":
		return vStr(tc.ChangeKind)
	case "motivating_question":
		return vStr(tc.MotivatingQuestion)
	case "terms_affected":
		return vStr(tc.TermsAffected)
	case "rationale":
		return vStr(tc.Rationale)
	case "approved_by":
		return vStr(tc.ApprovedBy)
	case "is_breaking_change":
		return vBool(tc.IsBreakingChange)
	case "is_backward_compatible":
		return vBool(tc.IsBackwardCompatible)
	}
	panic("ChangeLog has no field " + field)
}

func (tc *ChangeLog) erbSet(field string, v Value) {
	switch field {
	case "change_log_id":
		tc.ChangeLogId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "version":
		tc.Version = toStringPtr(v)
	case "change_date":
		tc.ChangeDate = toStringPtr(v)
	case "change_kind":
		tc.ChangeKind = toStringPtr(v)
	case "motivating_question":
		tc.MotivatingQuestion = toStringPtr(v)
	case "terms_affected":
		tc.TermsAffected = toStringPtr(v)
	case "rationale":
		tc.Rationale = toStringPtr(v)
	case "approved_by":
		tc.ApprovedBy = toStringPtr(v)
	case "is_breaking_change":
		tc.IsBreakingChange = toBoolPtr(v)
	case "is_backward_compatible":
		tc.IsBackwardCompatible = toBoolPtr(v)
	default:
		panic("ChangeLog has no field " + field)
	}
}

func (tc *ChangeLog) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "change_log_id", "relative_path", "iri", "name", "version", "change_date", "change_kind", "motivating_question", "terms_affected", "rationale", "approved_by", "is_breaking_change", "is_backward_compatible":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *ChangeLog) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *ChangeLog) erbResetErrors() { tc.ErbErrors = nil }

func (tc *ChangeLog) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *ChangeLog) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadChangeLogRecords reads ChangeLog rows from a JSON array file.
func LoadChangeLogRecords(path string) ([]ChangeLog, error) {
	records, err := loadRecords(path, func() Record { return &ChangeLog{} })
	if err != nil {
		return nil, err
	}
	rows := make([]ChangeLog, len(records))
	for i, r := range records {
		rows[i] = *r.(*ChangeLog)
	}
	return rows, nil
}

// =============================================================================
// VOCABULARYRECONCILIATIONS TABLE
// Table: VocabularyReconciliations. External dependency change: when a borrowed term from a living standard (PROV-O, FOAF, Dublin Core, DCAT, Schema.org) is deprecated and re-homed into the NTWF namespace, the edit triggers a version bump and an owl:sameAs reconciliation relation declaring the old and new terms equivalent. The worked example: deprecating foaf:name, prepending the ntwf prefix to get ntwf:name, and asserting foaf:name owl:sameAs ntwf:name. Each row is one reconciliation, with the standard it came from and the version in which the reconciliation shipped.
// =============================================================================

// VocabularyReconciliation represents a row in the VocabularyReconciliations table
type VocabularyReconciliation struct {
	ReconciliationId string `json:"reconciliation_id"`
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath).
	Name *string `json:"name"` // Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement.
	DeprecatedTerm *string `json:"deprecated_term"` // The borrowed/deprecated term being reconciled (e.g. foaf:name).
	ReplacementTerm *string `json:"replacement_term"` // The NTWF-namespaced replacement term (e.g. ntwf:name).
	ReconciliationRelation *string `json:"reconciliation_relation"` // The OWL relation asserting equivalence. NTWF uses owl:sameAs.
	SourceStandard *string `json:"source_standard"` // The external standard the deprecated term came from (PROV-O, FOAF, Dublin Core, DCAT, Schema.org).
	IntroducedInVersion *string `json:"introduced_in_version"` // The NTWF release version in which this reconciliation shipped. Re-homing a term triggers a version bump.
	Rationale *string `json:"rationale"` // Why the term was re-homed (e.g. upstream deprecation; semantic-alignment shift).
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key.
// Formula: ="reconciliations/" & {{ReconciliationId}}
func (tc *VocabularyReconciliation) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("reconciliations/"), erbTextOr(vStrPlain(tc.ReconciliationId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *VocabularyReconciliation) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement.
// Formula: ={{DeprecatedTerm}} & " owl:sameAs " & {{ReplacementTerm}}
func (tc *VocabularyReconciliation) CalcName() *string {
	return toStringPtr(erbConcat(erbTextOr(vStr(tc.DeprecatedTerm)), vS(" owl:sameAs "), erbTextOr(vStr(tc.ReplacementTerm))))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *VocabularyReconciliation) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *VocabularyReconciliation) ComputeAll() *VocabularyReconciliation {
	tc.erbComputeCalculations()
	return tc
}

func (tc *VocabularyReconciliation) erbGet(field string) Value {
	switch field {
	case "reconciliation_id":
		return vStrPlain(tc.ReconciliationId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "deprecated_term":
		return vStr(tc.DeprecatedTerm)
	case "replacement_term":
		return vStr(tc.ReplacementTerm)
	case "reconciliation_relation":
		return vStr(tc.ReconciliationRelation)
	case "source_standard":
		return vStr(tc.SourceStandard)
	case "introduced_in_version":
		return vStr(tc.IntroducedInVersion)
	case "rationale":
		return vStr(tc.Rationale)
	}
	panic("VocabularyReconciliations has no field " + field)
}

func (tc *VocabularyReconciliation) erbSet(field string, v Value) {
	switch field {
	case "reconciliation_id":
		tc.ReconciliationId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "deprecated_term":
		tc.DeprecatedTerm = toStringPtr(v)
	case "replacement_term":
		tc.ReplacementTerm = toStringPtr(v)
	case "reconciliation_relation":
		tc.ReconciliationRelation = toStringPtr(v)
	case "source_standard":
		tc.SourceStandard = toStringPtr(v)
	case "introduced_in_version":
		tc.IntroducedInVersion = toStringPtr(v)
	case "rationale":
		tc.Rationale = toStringPtr(v)
	default:
		panic("VocabularyReconciliations has no field " + field)
	}
}

func (tc *VocabularyReconciliation) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "reconciliation_id", "relative_path", "iri", "name", "deprecated_term", "replacement_term", "reconciliation_relation", "source_standard", "introduced_in_version", "rationale":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *VocabularyReconciliation) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *VocabularyReconciliation) erbResetErrors() { tc.ErbErrors = nil }

func (tc *VocabularyReconciliation) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *VocabularyReconciliation) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadVocabularyReconciliationRecords reads VocabularyReconciliations rows from a JSON array file.
func LoadVocabularyReconciliationRecords(path string) ([]VocabularyReconciliation, error) {
	records, err := loadRecords(path, func() Record { return &VocabularyReconciliation{} })
	if err != nil {
		return nil, err
	}
	rows := make([]VocabularyReconciliation, len(records))
	for i, r := range records {
		rows[i] = *r.(*VocabularyReconciliation)
	}
	return rows, nil
}

// =============================================================================
// SCENARIOS TABLE
// =============================================================================

// Scenario represents a row in the Scenarios table
type Scenario struct {
	ScenarioId string `json:"scenario_id"` // Stable identifier for a curated demo scenario (a named set of raw-fact edits applied at once).
	RelativePath *string `json:"relative_path"` // DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (dash-form of RelativePath).
	Name *string `json:"name"` // Slug form of the human label.
	Label string `json:"label"` // Human-readable button label for this scenario in the picker.
	Icon *string `json:"icon"` // A single emoji shown beside the label in the picker.
	Explanation *string `json:"explanation"` // Plain-language description of what this scenario changes and what the reasoner will derive as a result. Shown in the floating scenario picker so the user knows what each preset does before applying it.
	SortOrder *int `json:"sort_order"` // Display order in the picker (ascending).
	IsReset *bool `json:"is_reset"` // True for the single 'restore baseline' scenario; the picker styles it as a secondary action.
	Edits string `json:"edits"` // JSON-encoded ordered list of raw-fact assignments this scenario applies. Each item is {class, id|match, set:{field:value,...}} where 'class' is a rulebook table (camelCase keys on the raw store), 'id' targets a row by its *Id primary key (or 'match':'first' for the singleton Workflow), and 'set' is the raw fields to assign. The backend replays this list against the active raw store, then re-reasons — the scenario NEVER sets a derived field. This is the single source of truth for what each demo scenario does; the app's picker and the apply endpoint both read it from here (same JSON-on-a-first-class-table pattern as __meta__'s JsonValue).
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key.
// Formula: ="scenarios/" & {{ScenarioId}}
func (tc *Scenario) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("scenarios/"), erbTextOr(vStrPlain(tc.ScenarioId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Scenario) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Slug form of the human label.
// Formula: =SUBSTITUTE(LOWER({{Label}}), " ", "-")
func (tc *Scenario) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.Label)), vS(" "), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *Scenario) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *Scenario) ComputeAll() *Scenario {
	tc.erbComputeCalculations()
	return tc
}

func (tc *Scenario) erbGet(field string) Value {
	switch field {
	case "scenario_id":
		return vStrPlain(tc.ScenarioId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "label":
		return vStrPlain(tc.Label)
	case "icon":
		return vStr(tc.Icon)
	case "explanation":
		return vStr(tc.Explanation)
	case "sort_order":
		return vInt(tc.SortOrder)
	case "is_reset":
		return vBool(tc.IsReset)
	case "edits":
		return vStrPlain(tc.Edits)
	}
	panic("Scenarios has no field " + field)
}

func (tc *Scenario) erbSet(field string, v Value) {
	switch field {
	case "scenario_id":
		tc.ScenarioId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "label":
		tc.Label = strPlain(v)
	case "icon":
		tc.Icon = toStringPtr(v)
	case "explanation":
		tc.Explanation = toStringPtr(v)
	case "sort_order":
		tc.SortOrder = toIntPtr(v)
	case "is_reset":
		tc.IsReset = toBoolPtr(v)
	case "edits":
		tc.Edits = strPlain(v)
	default:
		panic("Scenarios has no field " + field)
	}
}

func (tc *Scenario) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "scenario_id", "relative_path", "iri", "name", "label", "icon", "explanation", "sort_order", "is_reset", "edits":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *Scenario) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *Scenario) erbResetErrors() { tc.ErbErrors = nil }

func (tc *Scenario) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *Scenario) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadScenarioRecords reads Scenarios rows from a JSON array file.
func LoadScenarioRecords(path string) ([]Scenario, error) {
	records, err := loadRecords(path, func() Record { return &Scenario{} })
	if err != nil {
		return nil, err
	}
	rows := make([]Scenario, len(records))
	for i, r := range records {
		rows[i] = *r.(*Scenario)
	}
	return rows, nil
}

// =============================================================================
// COMPETENCYQUESTIONS TABLE
// The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook.
// =============================================================================

// CompetencyQuestion represents a row in the CompetencyQuestions table
type CompetencyQuestion struct {
	CompetencyQuestionId string `json:"competency_question_id"` // Primary key. Stable slug for the competency question (cq-1 .. cq-8).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
	Name *string `json:"name"` // Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables.
	Number float64 `json:"number"` // The canonical 1-8 ordering of the competency questions as listed in the article / README.
	DisplayName string `json:"display_name"` // Short human label for the question (e.g. 'Steps and order').
	QuestionText string `json:"question_text"` // The full competency question, verbatim from the article's acceptance suite.
	TargetTable string `json:"target_table"` // The entity whose computed field answers this question. Together with TargetField it pins the answer to a real column in the substrate, so the scoreboard reads the answer (never recomputes it) and the explainer-DAG drilldown lands on the exact derivation.
	TargetField string `json:"target_field"` // The substrate-computed field on TargetTable that answers this question (calc / lookup / aggregation / closure). The CQ scoreboard wraps the live answer in a DagCell(TargetTable, TargetField) so a click opens its inference graph.
	AnswerKind string `json:"answer_kind"` // 'scalar' when the answer is a single value graded by equality with ExpectedAnswer; 'list' when the answer is a collection graded as answerable (non-empty / matches the asserted shape).
	ExpectedAnswer string `json:"expected_answer"` // The asserted correct answer for the seed worked example. For scalar questions the live computed value must equal this to score a pass; for list questions this is the canonical summary the rendered collection is checked against. Authored here so pass/fail is (substrate-computed value) vs (rulebook-asserted expectation) — a real conformance check, not UI logic.
	SatisfiedField *string `json:"satisfied_field"` // Name of the boolean column on Workflows that computes whether this CQ is satisfied (e.g. Cq6Satisfied). The scoreboard reads pass/fail straight from this substrate-computed column — the acceptance criterion lives in the rulebook as a derived field, never as app-side logic. Mirrors TargetTable/TargetField for the answer.
	Explanation *string `json:"explanation"` // One-sentence note on how this question resolves through the model — the FK / formula chain a presenter can narrate.
	SortOrder *float64 `json:"sort_order"` // Display order in the scoreboard. Mirrors Number for now; kept separate so the list can be re-sequenced without renumbering the canonical CQ ids.
	IsActive *bool `json:"is_active"` // Whether this competency question is shown in the scoreboard. All eight are active in the worked example.
	SimulateScenario *string `json:"simulate_scenario"` // FK to the Scenario the card's 'Simulate' button applies to demonstrate this competency question live. Points at the minimal raw-fact edit that moves THIS question's answer in isolation where one exists; for cq-2 it points at 'ai-release-manager', which also ripples to cq-3 (the gate approver is itself a step executor, so the two answers cannot be perturbed independently). The full set of questions each scenario moves — trigger vs ripple — is enumerated in the ScenarioCQEffects junction; this is just the one the button fires. Inverse-ish of ScenarioCQEffects but kept as a direct FK so the UI has a single answer.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution.
// Formula: ="competency-questions/" & {{CompetencyQuestionId}}
func (tc *CompetencyQuestion) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("competency-questions/"), erbTextOr(vStrPlain(tc.CompetencyQuestionId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *CompetencyQuestion) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *CompetencyQuestion) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.DisplayName)), vS(" "), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *CompetencyQuestion) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *CompetencyQuestion) ComputeAll() *CompetencyQuestion {
	tc.erbComputeCalculations()
	return tc
}

func (tc *CompetencyQuestion) erbGet(field string) Value {
	switch field {
	case "competency_question_id":
		return vStrPlain(tc.CompetencyQuestionId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "number":
		return vNumPlain(tc.Number)
	case "display_name":
		return vStrPlain(tc.DisplayName)
	case "question_text":
		return vStrPlain(tc.QuestionText)
	case "target_table":
		return vStrPlain(tc.TargetTable)
	case "target_field":
		return vStrPlain(tc.TargetField)
	case "answer_kind":
		return vStrPlain(tc.AnswerKind)
	case "expected_answer":
		return vStrPlain(tc.ExpectedAnswer)
	case "satisfied_field":
		return vStr(tc.SatisfiedField)
	case "explanation":
		return vStr(tc.Explanation)
	case "sort_order":
		return vNum(tc.SortOrder)
	case "is_active":
		return vBool(tc.IsActive)
	case "simulate_scenario":
		return vStr(tc.SimulateScenario)
	}
	panic("CompetencyQuestions has no field " + field)
}

func (tc *CompetencyQuestion) erbSet(field string, v Value) {
	switch field {
	case "competency_question_id":
		tc.CompetencyQuestionId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "number":
		tc.Number = floatPlain(v)
	case "display_name":
		tc.DisplayName = strPlain(v)
	case "question_text":
		tc.QuestionText = strPlain(v)
	case "target_table":
		tc.TargetTable = strPlain(v)
	case "target_field":
		tc.TargetField = strPlain(v)
	case "answer_kind":
		tc.AnswerKind = strPlain(v)
	case "expected_answer":
		tc.ExpectedAnswer = strPlain(v)
	case "satisfied_field":
		tc.SatisfiedField = toStringPtr(v)
	case "explanation":
		tc.Explanation = toStringPtr(v)
	case "sort_order":
		tc.SortOrder = toFloatPtr(v)
	case "is_active":
		tc.IsActive = toBoolPtr(v)
	case "simulate_scenario":
		tc.SimulateScenario = toStringPtr(v)
	default:
		panic("CompetencyQuestions has no field " + field)
	}
}

func (tc *CompetencyQuestion) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "competency_question_id", "relative_path", "iri", "name", "number", "display_name", "question_text", "target_table", "target_field", "answer_kind", "expected_answer", "satisfied_field", "explanation", "sort_order", "is_active", "simulate_scenario":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *CompetencyQuestion) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *CompetencyQuestion) erbResetErrors() { tc.ErbErrors = nil }

func (tc *CompetencyQuestion) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *CompetencyQuestion) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadCompetencyQuestionRecords reads CompetencyQuestions rows from a JSON array file.
func LoadCompetencyQuestionRecords(path string) ([]CompetencyQuestion, error) {
	records, err := loadRecords(path, func() Record { return &CompetencyQuestion{} })
	if err != nil {
		return nil, err
	}
	rows := make([]CompetencyQuestion, len(records))
	for i, r := range records {
		rows[i] = *r.(*CompetencyQuestion)
	}
	return rows, nil
}

// =============================================================================
// SCENARIOCQEFFECTS TABLE
// Table: ScenarioCQEffects. Names the many-to-many between Scenarios and CompetencyQuestions as two 1:M foreign keys (Scenario, CompetencyQuestion) plus the detail of the relationship (EffectKind, Note). Each row asserts 'applying this scenario moves this competency question's live answer'. 'trigger' rows are the intended demonstration; 'ripple' rows record answers that move as an unavoidable consequence of the same raw edit (e.g. ai-release-manager moves cq-2 AND cq-3 because the gate approver is itself a step executor). The answers themselves are never stored here — they are read live from each substrate after the scenario applies.
// =============================================================================

// ScenarioCQEffect represents a row in the ScenarioCQEffects table
type ScenarioCQEffect struct {
	ScenarioCQEffectId string `json:"scenario_cq_effect_id"` // Primary key. '<scenario>-<cq>' — names one (scenario moves this competency question) edge.
	RelativePath *string `json:"relative_path"` // DAG-derived location: 'scenario-cq-effects/' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (dash-form of RelativePath).
	Name *string `json:"name"` // Slug label, mirrors the primary key.
	Scenario string `json:"scenario"` // FK to the Scenario whose raw-fact edits cause this effect. The 'many effects belong to one scenario' side: a single scenario can move several competency questions.
	CompetencyQuestion string `json:"competency_question"` // FK to the CompetencyQuestion whose live answer this scenario moves. The 'many effects belong to one question' side: a question can be exercised by several scenarios.
	EffectKind string `json:"effect_kind"` // 'trigger' = this scenario was authored to move this question (the point of the demo). 'ripple' = the question also moves as an unavoidable side effect of the same raw edit. The ripple rows are the pedagogical payload: they show answers that are structurally coupled and cannot be perturbed independently.
	Note *string `json:"note"` // One-line, human-readable account of how this scenario moves this question's answer (qualitative — the actual value is read live from the substrate, never stored here).
	SortOrder *int `json:"sort_order"` // Display order.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// DAG-derived location: 'scenario-cq-effects/' + the row's primary key.
// Formula: ="scenario-cq-effects/" & {{ScenarioCQEffectId}}
func (tc *ScenarioCQEffect) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("scenario-cq-effects/"), erbTextOr(vStrPlain(tc.ScenarioCQEffectId))))
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ScenarioCQEffect) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Slug label, mirrors the primary key.
// Formula: =SUBSTITUTE(LOWER({{ScenarioCQEffectId}}), " ", "-")
func (tc *ScenarioCQEffect) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.ScenarioCQEffectId)), vS(" "), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *ScenarioCQEffect) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *ScenarioCQEffect) ComputeAll() *ScenarioCQEffect {
	tc.erbComputeCalculations()
	return tc
}

func (tc *ScenarioCQEffect) erbGet(field string) Value {
	switch field {
	case "scenario_cq_effect_id":
		return vStrPlain(tc.ScenarioCQEffectId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "scenario":
		return vStrPlain(tc.Scenario)
	case "competency_question":
		return vStrPlain(tc.CompetencyQuestion)
	case "effect_kind":
		return vStrPlain(tc.EffectKind)
	case "note":
		return vStr(tc.Note)
	case "sort_order":
		return vInt(tc.SortOrder)
	}
	panic("ScenarioCQEffects has no field " + field)
}

func (tc *ScenarioCQEffect) erbSet(field string, v Value) {
	switch field {
	case "scenario_cq_effect_id":
		tc.ScenarioCQEffectId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "scenario":
		tc.Scenario = strPlain(v)
	case "competency_question":
		tc.CompetencyQuestion = strPlain(v)
	case "effect_kind":
		tc.EffectKind = strPlain(v)
	case "note":
		tc.Note = toStringPtr(v)
	case "sort_order":
		tc.SortOrder = toIntPtr(v)
	default:
		panic("ScenarioCQEffects has no field " + field)
	}
}

func (tc *ScenarioCQEffect) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "scenario_cq_effect_id", "relative_path", "iri", "name", "scenario", "competency_question", "effect_kind", "note", "sort_order":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *ScenarioCQEffect) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *ScenarioCQEffect) erbResetErrors() { tc.ErbErrors = nil }

func (tc *ScenarioCQEffect) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *ScenarioCQEffect) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadScenarioCQEffectRecords reads ScenarioCQEffects rows from a JSON array file.
func LoadScenarioCQEffectRecords(path string) ([]ScenarioCQEffect, error) {
	records, err := loadRecords(path, func() Record { return &ScenarioCQEffect{} })
	if err != nil {
		return nil, err
	}
	rows := make([]ScenarioCQEffect, len(records))
	for i, r := range records {
		rows[i] = *r.(*ScenarioCQEffect)
	}
	return rows, nil
}

// =============================================================================
// CONFORMANCETESTS TABLE
// =============================================================================

// ConformanceTest represents a row in the ConformanceTests table
type ConformanceTest struct {
	ConformanceTestId string `json:"conformance_test_id"` // Stable identifier for one conformance test — one assertion the harness runs against every execution substrate.
	RelativePath *string `json:"relative_path"` // DAG-derived location for this test row: root segment 'conformance-tests' + the primary key.
	Iri *string `json:"iri"` // Slug IRI for this row, derived from RelativePath.
	Name *string `json:"name"` // Machine name derived from the display name.
	DisplayName string `json:"display_name"` // Human-readable test title shown in the admin console and run logs.
	FeatureRef *string `json:"feature_ref"` // Comma-separated FEATURE-COVERAGE.md ids this test witnesses (e.g. 'II-3,CQ2'). The traceability link from the article's feature inventory to an executable assertion.
	Section string `json:"section"` // Grouping for display: 'Sweep', 'Part I'..'Part IV', 'Closure', 'Mutation'.
	TestKind string `json:"test_kind"` // How the harness executes this test. 'sweep' = every row+field of TargetRef table vs the answer key; 'field-match' = one row's field vs the answer key; 'closure-contains' = a from→to pair must appear in the engine's computed transitive closure (Expect names the closure and pair); 'engines-agree' = zero value-class disagreements between the two engines; 'mutation' = apply Expect.edits to an in-memory copy of the seed facts, re-reason, and check Expect.assert — the store is NEVER written.
	TargetRef *string `json:"target_ref"` // What the test reads: 'Entity' (whole table), 'Entity/pk' (one row) or 'Entity/pk#Field' (one value). Slash/hash form on purpose — these are spec references, not foreign keys.
	Expect *string `json:"expect"` // Kind-specific JSON spec. Empty for sweep/field-match/engines-agree — there the ORACLE is the answer key (testing/answer-keys), never a value hardcoded here (a literal would go stale; the key regenerates). closure-contains: {closure, from, to}. mutation: {edits:[{class,id,set:{camelRawField:value}}], assert:[{class,id,field,equals}]} — same edits shape as Scenarios.Edits.
	Explanation *string `json:"explanation"` // Why this test exists — what feature of the model it flexes, in one sentence.
	SortOrder int `json:"sort_order"` // Display/run order within the suite.
	IsEnabled bool `json:"is_enabled"` // Disabled tests are listed but not executed (parked, not deleted — the list stays the complete spec).
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcRelativePath computes the RelativePath calculated field
// DAG-derived location for this test row: root segment 'conformance-tests' + the primary key.
// Formula: ="conformance-tests/" & {{ConformanceTestId}}
func (tc *ConformanceTest) CalcRelativePath() *string {
	return toStringPtr(erbConcat(vS("conformance-tests/"), erbTextOr(vStrPlain(tc.ConformanceTestId))))
}

// CalcIri computes the Iri calculated field
// Slug IRI for this row, derived from RelativePath.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ConformanceTest) CalcIri() *string {
	return toStringPtr(erbSubstitute(vStr(tc.RelativePath), vS("/"), vS("-")))
}

// CalcName computes the Name calculated field
// Machine name derived from the display name.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *ConformanceTest) CalcName() *string {
	return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.DisplayName)), vS(" "), vS("-")))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *ConformanceTest) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "relative_path", func() { tc.RelativePath = tc.CalcRelativePath() })
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
	// Level 2
	calcGuard(tc, "iri", func() { tc.Iri = tc.CalcIri() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *ConformanceTest) ComputeAll() *ConformanceTest {
	tc.erbComputeCalculations()
	return tc
}

func (tc *ConformanceTest) erbGet(field string) Value {
	switch field {
	case "conformance_test_id":
		return vStrPlain(tc.ConformanceTestId)
	case "relative_path":
		return vStr(tc.RelativePath)
	case "iri":
		return vStr(tc.Iri)
	case "name":
		return vStr(tc.Name)
	case "display_name":
		return vStrPlain(tc.DisplayName)
	case "feature_ref":
		return vStr(tc.FeatureRef)
	case "section":
		return vStrPlain(tc.Section)
	case "test_kind":
		return vStrPlain(tc.TestKind)
	case "target_ref":
		return vStr(tc.TargetRef)
	case "expect":
		return vStr(tc.Expect)
	case "explanation":
		return vStr(tc.Explanation)
	case "sort_order":
		return vIntPlain(tc.SortOrder)
	case "is_enabled":
		return vBoolPlain(tc.IsEnabled)
	}
	panic("ConformanceTests has no field " + field)
}

func (tc *ConformanceTest) erbSet(field string, v Value) {
	switch field {
	case "conformance_test_id":
		tc.ConformanceTestId = strPlain(v)
	case "relative_path":
		tc.RelativePath = toStringPtr(v)
	case "iri":
		tc.Iri = toStringPtr(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "display_name":
		tc.DisplayName = strPlain(v)
	case "feature_ref":
		tc.FeatureRef = toStringPtr(v)
	case "section":
		tc.Section = strPlain(v)
	case "test_kind":
		tc.TestKind = strPlain(v)
	case "target_ref":
		tc.TargetRef = toStringPtr(v)
	case "expect":
		tc.Expect = toStringPtr(v)
	case "explanation":
		tc.Explanation = toStringPtr(v)
	case "sort_order":
		tc.SortOrder = intPlain(v)
	case "is_enabled":
		tc.IsEnabled = boolPlain(v)
	default:
		panic("ConformanceTests has no field " + field)
	}
}

func (tc *ConformanceTest) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "conformance_test_id", "relative_path", "iri", "name", "display_name", "feature_ref", "section", "test_kind", "target_ref", "expect", "explanation", "sort_order", "is_enabled":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *ConformanceTest) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *ConformanceTest) erbResetErrors() { tc.ErbErrors = nil }

func (tc *ConformanceTest) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *ConformanceTest) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// LoadConformanceTestRecords reads ConformanceTests rows from a JSON array file.
func LoadConformanceTestRecords(path string) ([]ConformanceTest, error) {
	records, err := loadRecords(path, func() Record { return &ConformanceTest{} })
	if err != nil {
		return nil, err
	}
	rows := make([]ConformanceTest, len(records))
	for i, r := range records {
		rows[i] = *r.(*ConformanceTest)
	}
	return rows, nil
}

// =============================================================================
// __META__ TABLE
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
// =============================================================================

// __meta__ represents a row in the __meta__ table
type __meta__ struct {
	MetaKey string `json:"meta_key"` // The metadata key (e.g. 'tagline', 'motif_palette', 'substrates'). Unique within the table.
	Name *string `json:"name"` // Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
	ValueType string `json:"value_type"` // How to interpret the value columns: 'string' (use StringValue), 'object' (parse JsonValue as JSON object), 'array' (parse JsonValue as JSON array).
	StringValue *string `json:"string_value"` // Plain string value. Populated when ValueType == 'string'; null otherwise.
	JsonValue *string `json:"json_value"` // JSON-encoded value. Populated when ValueType == 'object' or 'array'; null when ValueType == 'string'.
	ErbErrors map[string]string `json:"_erb_errors,omitempty"`
	erbAggregates map[string]Value
}

// CalcName computes the Name calculated field
// Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
// Formula: ={{MetaKey}}
func (tc *__meta__) CalcName() *string {
	return toStringPtr(vStrPlain(tc.MetaKey))
}

// erbComputeCalculations computes every calculated field of the row in dependency order.
func (tc *__meta__) erbComputeCalculations() {
	// Level 1
	calcGuard(tc, "name", func() { tc.Name = tc.CalcName() })
}

// ComputeAll computes every calculated field from the row's current inputs.
func (tc *__meta__) ComputeAll() *__meta__ {
	tc.erbComputeCalculations()
	return tc
}

func (tc *__meta__) erbGet(field string) Value {
	switch field {
	case "meta_key":
		return vStrPlain(tc.MetaKey)
	case "name":
		return vStr(tc.Name)
	case "value_type":
		return vStrPlain(tc.ValueType)
	case "string_value":
		return vStr(tc.StringValue)
	case "json_value":
		return vStr(tc.JsonValue)
	}
	panic("__meta__ has no field " + field)
}

func (tc *__meta__) erbSet(field string, v Value) {
	switch field {
	case "meta_key":
		tc.MetaKey = strPlain(v)
	case "name":
		tc.Name = toStringPtr(v)
	case "value_type":
		tc.ValueType = strPlain(v)
	case "string_value":
		tc.StringValue = toStringPtr(v)
	case "json_value":
		tc.JsonValue = toStringPtr(v)
	default:
		panic("__meta__ has no field " + field)
	}
}

func (tc *__meta__) erbLoad(row map[string]any) {
	for key, value := range row {
		switch key {
		case "meta_key", "name", "value_type", "string_value", "json_value":
			tc.erbSet(key, fromJSON(value))
		}
	}
}

func (tc *__meta__) erbErrors() map[string]string {
	if tc.ErbErrors == nil {
		tc.ErbErrors = map[string]string{}
	}
	return tc.ErbErrors
}

func (tc *__meta__) erbResetErrors() { tc.ErbErrors = nil }

func (tc *__meta__) erbAggregate(name string) Value { return tc.erbAggregates[name] }

func (tc *__meta__) erbSetAggregate(name string, v Value) {
	if tc.erbAggregates == nil {
		tc.erbAggregates = map[string]Value{}
	}
	tc.erbAggregates[name] = v
}

// Load__meta__Records reads __meta__ rows from a JSON array file.
func Load__meta__Records(path string) ([]__meta__, error) {
	records, err := loadRecords(path, func() Record { return &__meta__{} })
	if err != nil {
		return nil, err
	}
	rows := make([]__meta__, len(records))
	for i, r := range records {
		rows[i] = *r.(*__meta__)
	}
	return rows, nil
}

// calculatedFieldCount bounds the runner's passes over the dataset.
const calculatedFieldCount = 102

// erbTables is every table, in rulebook order.
var erbTables = []TableSpec{
	{Name: "Workflows", File: "workflows", RulebookRows: 1, New: func() Record { return &Workflow{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{
			{Field: "count_of_non_proposed_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}}},
			{Field: "count_ai_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "is_executed_by_ai", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_human_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "is_executed_by_human", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_human_required_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "requires_human_approval", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_approval_consistency_violations", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "approval_consistency_violation", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_derivation_links", Op: "COUNTIFS", Table: "workflow_artifacts", Criteria: []Criterion{{Range: "produced_by_workflow", Kind: "field", Field: "workflow_id"}, {Range: "has_derivation_parent", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_legal_owned_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "is_legal_owned", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_engineering_owned_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "is_engineering_owned", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_inferred_precedence_pairs", Op: "COUNTIFS", Table: "vw_step_precedence_closure", Criteria: []Criterion{{Range: "is_inferred", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_asserted_precedence_pairs", Op: "COUNTIFS", Table: "vw_step_precedence_closure", Criteria: []Criterion{{Range: "is_inferred", Kind: "literal", Literal: vB(false)}}},
			{Field: "count_roles_with_bad_filler_cardinality", Op: "COUNTIFS", Table: "roles", Criteria: []Criterion{{Range: "has_exactly_one_filler", Kind: "literal", Literal: vB(false)}}},
			{Field: "count_agent_type_changes", Op: "COUNTIFS", Table: "role_assignments", Criteria: []Criterion{{Range: "is_agent_type_change", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_compliance_audit_changes", Op: "COUNTIFS", Table: "role_assignments", Criteria: []Criterion{{Range: "requires_compliance_audit", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_approval_gate_steps", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "workflow", Kind: "field", Field: "workflow_id"}, {Range: "is_approval_gate", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_gates_without_human_approver", Op: "COUNTIFS", Table: "approval_gates", Criteria: []Criterion{{Range: "has_human_approver", Kind: "literal", Literal: vB(false)}}},
			{Field: "count_workflow_artifacts", Op: "COUNTIFS", Table: "workflow_artifacts", Criteria: []Criterion{{Range: "produced_by_workflow", Kind: "field", Field: "workflow_id"}}},
			{Field: "count_roles_with_escalation_violation", Op: "COUNTIFS", Table: "roles", Criteria: []Criterion{{Range: "escalation_violation", Kind: "literal", Literal: vB(true)}}},
			{Field: "count_unconsumed_datasets", Op: "COUNTIFS", Table: "datasets", Criteria: []Criterion{{Range: "is_consumed", Kind: "literal", Literal: vB(false)}}},}},
	{Name: "WorkflowSteps", File: "workflow_steps", RulebookRows: 5, New: func() Record { return &WorkflowStep{} },
		Lookups: []LookupSpec{
			{Field: "parent_path", Target: "workflows", Return: "relative_path", Key: "workflow", Match: "workflow_id"},
			{Field: "executing_human_agent", Target: "roles", Return: "filled_by_human_agent", Key: "assigned_role", Match: "role_id"},
			{Field: "executing_ai_agent", Target: "roles", Return: "filled_by_ai_agent", Key: "assigned_role", Match: "role_id"},
			{Field: "executing_automated_pipeline", Target: "roles", Return: "filled_by_automated_pipeline", Key: "assigned_role", Match: "role_id"},
			{Field: "owning_department", Target: "roles", Return: "owned_by", Key: "assigned_role", Match: "role_id"},},
		Aggregations: []AggregateSpec{
			{Field: "preceding_step_count", Op: "COUNTIFS", Table: "vw_step_precedence_closure", Criteria: []Criterion{{Range: "to_id", Kind: "field", Field: "workflow_step_id"}}},}},
	{Name: "ApprovalGates", File: "approval_gates", RulebookRows: 1, New: func() Record { return &ApprovalGate{} },
		Lookups: []LookupSpec{
			{Field: "parent_path", Target: "workflow_steps", Return: "relative_path", Key: "workflow_step", Match: "workflow_step_id"},
			{Field: "gate_role", Target: "workflow_steps", Return: "assigned_role", Key: "workflow_step", Match: "workflow_step_id"},
			{Field: "gate_approver_human", Target: "roles", Return: "filled_by_human_agent", Key: "gate_role", Match: "role_id"},},
		Aggregations: []AggregateSpec{}},
	{Name: "StepPrecedence", File: "step_precedence", RulebookRows: 4, New: func() Record { return &StepPrecedence{} },
		Lookups: []LookupSpec{
			{Field: "parent_path", Target: "workflow_steps", Return: "relative_path", Key: "from_step", Match: "workflow_step_id"},},
		Aggregations: []AggregateSpec{}},
	{Name: "Roles", File: "roles", RulebookRows: 7, New: func() Record { return &Role{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{
			{Field: "fills_approval_gate", Op: "COUNTIFS", Table: "workflow_steps", Criteria: []Criterion{{Range: "assigned_role", Kind: "field", Field: "role_id"}, {Range: "is_approval_gate", Kind: "literal", Literal: vB(true)}}},}},
	{Name: "RoleAssignments", File: "role_assignments", RulebookRows: 6, New: func() Record { return &RoleAssignment{} },
		Lookups: []LookupSpec{
			{Field: "parent_path", Target: "roles", Return: "relative_path", Key: "role", Match: "role_id"},},
		Aggregations: []AggregateSpec{}},
	{Name: "Departments", File: "departments", RulebookRows: 2, New: func() Record { return &Department{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "HumanAgents", File: "human_agents", RulebookRows: 5, New: func() Record { return &HumanAgent{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "AIAgents", File: "ai_agents", RulebookRows: 2, New: func() Record { return &AIAgent{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{
			{Field: "count_attributed_artifacts", Op: "COUNTIFS", Table: "workflow_artifacts", Criteria: []Criterion{{Range: "attributed_to_ai_agent", Kind: "field", Field: "ai_agent_id"}}},
			{Field: "count_impacted_workflows", Op: "COUNTIFS", Table: "workflow_artifacts", Criteria: []Criterion{{Range: "attributed_to_ai_agent", Kind: "field", Field: "ai_agent_id"}, {Range: "has_producing_workflow", Kind: "literal", Literal: vB(true)}}},}},
	{Name: "AutomatedPipelines", File: "automated_pipelines", RulebookRows: 1, New: func() Record { return &AutomatedPipeline{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "WorkflowStatusConcepts", File: "workflow_status_concepts", RulebookRows: 4, New: func() Record { return &WorkflowStatusConcept{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "AgentCapabilityConcepts", File: "agent_capability_concepts", RulebookRows: 6, New: func() Record { return &AgentCapabilityConcept{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "ArtifactTypeConcepts", File: "artifact_type_concepts", RulebookRows: 3, New: func() Record { return &ArtifactTypeConcept{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "Datasets", File: "datasets", RulebookRows: 1, New: func() Record { return &Dataset{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "WorkflowArtifacts", File: "workflow_artifacts", RulebookRows: 5, New: func() Record { return &WorkflowArtifact{} },
		Lookups: []LookupSpec{
			{Field: "parent_path", Target: "workflow_steps", Return: "relative_path", Key: "produced_by_step", Match: "workflow_step_id"},
			{Field: "produced_by_workflow", Target: "workflow_steps", Return: "workflow", Key: "produced_by_step", Match: "workflow_step_id"},},
		Aggregations: []AggregateSpec{}},
	{Name: "GovernanceRoles", File: "governance_roles", RulebookRows: 2, New: func() Record { return &GovernanceRole{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "ChangeLog", File: "change_log", RulebookRows: 2, New: func() Record { return &ChangeLog{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "VocabularyReconciliations", File: "vocabulary_reconciliations", RulebookRows: 2, New: func() Record { return &VocabularyReconciliation{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "Scenarios", File: "scenarios", RulebookRows: 12, New: func() Record { return &Scenario{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "CompetencyQuestions", File: "competency_questions", RulebookRows: 8, New: func() Record { return &CompetencyQuestion{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "ScenarioCQEffects", File: "scenario_cq_effects", RulebookRows: 12, New: func() Record { return &ScenarioCQEffect{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "ConformanceTests", File: "conformance_tests", RulebookRows: 72, New: func() Record { return &ConformanceTest{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
	{Name: "__meta__", File: "__meta__", RulebookRows: 12, New: func() Record { return &__meta__{} },
		Lookups: []LookupSpec{},
		Aggregations: []AggregateSpec{}},
}

// erbClosures materializes each vw_<entity>_closure view aggregations read.
var erbClosures = []ClosureSpec{
	{View: "vw_roles_closure", Source: "roles", From: "role_id", To: "delegates_to"},
	{View: "vw_step_precedence_closure", Source: "step_precedence", From: "from_step", To: "to_step"},
	{View: "vw_workflow_artifacts_closure", Source: "workflow_artifacts", From: "artifact_id", To: "derived_from_artifact"},
}
