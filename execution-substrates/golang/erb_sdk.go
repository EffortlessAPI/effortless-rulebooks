// ERB SDK - Go Implementation (GENERATED - DO NOT EDIT)
// ======================================================
// Generated from: effortless-rulebook/talismans-special-solutions-rulebook.json
//
// This file contains structs and calculation functions
// for all tables defined in the rulebook.

package main

import (
	"encoding/json"
	"fmt"
	"os"
	"strconv"
	"strings"
)

// =============================================================================
// HELPER FUNCTIONS
// =============================================================================

// boolVal safely dereferences a *bool, returning false if nil
func boolVal(b *bool) bool {
	if b == nil {
		return false
	}
	return *b
}

// stringVal safely dereferences a *string, returning "" if nil
func stringVal(s *string) string {
	if s == nil {
		return ""
	}
	return *s
}

// nilIfEmpty returns nil for empty strings, otherwise a pointer to the string
func nilIfEmpty(s string) *string {
	if s == "" {
		return nil
	}
	return &s
}

// intToString safely converts a *int to string, returning "" if nil
func intToString(i *int) string {
	if i == nil {
		return ""
	}
	return strconv.Itoa(*i)
}

// boolToString converts a bool to "true" or "false"
func boolToString(b bool) string {
	if b {
		return "true"
	}
	return "false"
}

// FlexibleString is a type that can unmarshal from both string and number JSON values
// This is needed for aggregation fields that return 0 (int) when empty or string values
type FlexibleString string

func (f *FlexibleString) UnmarshalJSON(data []byte) error {
	// First try as string
	var s string
	if err := json.Unmarshal(data, &s); err == nil {
		*f = FlexibleString(s)
		return nil
	}
	// Try as number
	var n float64
	if err := json.Unmarshal(data, &n); err == nil {
		// Convert number to string, but treat 0 as empty
		if n == 0 {
			*f = FlexibleString("0")
		} else {
			*f = FlexibleString(fmt.Sprintf("%v", n))
		}
		return nil
	}
	return fmt.Errorf("cannot unmarshal %s into FlexibleString", string(data))
}

// String returns the underlying string value
func (f FlexibleString) String() string {
	return string(f)
}

// =============================================================================
// WORKFLOWS TABLE
// Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
// =============================================================================

// Workflow represents a row in the Workflows table
// Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
type Workflow struct {
	WorkflowId string `json:"workflow_id"`
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
	CountAISteps *int `json:"count_ai_steps"` // Number of steps in this workflow executed by an AIAgent (rollup over WorkflowSteps.IsExecutedByAI). This is the 'AI-executed' half of the article's CQ3 ('which steps are executed by AI agents') — counted against AIAgent individuals specifically, not the deterministic AutomatedPipeline, which is a disjoint agent type. Also drives the business-payoff query (a workflow is a compliance risk when an AI agent runs a step). Worked example: 2 (the AI risk-assessment step and the AI post-deployment health report).
	CountHumanSteps *int `json:"count_human_steps"` // Number of steps executed by a HumanAgent (rollup over WorkflowSteps.IsExecutedByHuman). The 'who actually runs this step' count — distinct from CountHumanRequiredSteps, which counts steps that demand a human decision (requiresHumanApproval). Worked example: 2 (the legal-review step and the release approval gate).
	CountHumanRequiredSteps *int `json:"count_human_required_steps"` // Number of steps that require a human decision (rollup over WorkflowSteps.RequiresHumanApproval). This is the 'human-required' half of the article's CQ3 ('which require a human decision'), answered — as the article notes — by a single FILTER on requiresHumanApproval. Worked example: 2 (the legal-review step and the release approval gate).
	CountApprovalConsistencyViolations *int `json:"count_approval_consistency_violations"` // Number of steps that require human approval but are not human-filled (rollup over WorkflowSteps.ApprovalConsistencyViolation). The clean ABox witness: this is 0 for the Production Deployment workflow. A non-zero value is the relational signal of a Suite-4 consistency violation.
	CountDerivationLinks *int `json:"count_derivation_links"` // Number of prov:wasDerivedFrom links among this workflow's artifacts (rollup over WorkflowArtifacts.HasDerivationParent). Answers the lineage half of CQ4: 5 artifacts form a chain with 4 derivation links.
	CountLegalOwnedSteps *int `json:"count_legal_owned_steps"` // Number of steps in this workflow whose owning department is Legal (rollup over WorkflowSteps.IsLegalOwned). CQ7: exactly one Legal-owned step in the Production Deployment workflow.
	CountEngineeringOwnedSteps *int `json:"count_engineering_owned_steps"` // Number of steps whose owning department is Engineering (rollup over WorkflowSteps.IsEngineeringOwned). Feeds CQ7's Engineering-involvement check.
	CountInferredPrecedencePairs *int `json:"count_inferred_precedence_pairs"` // Number of step-ordering pairs that the transitive closure of ntwf:precedesStep INFERRED (rollup over the closure view vw_step_precedence_closure where is_inferred = TRUE). The article's signature count: 6 of the 10 closure pairs were never asserted — including step-1 -> step-5. NOTE: this single-workflow model has exactly one Workflow, so the global closure view is wholly this workflow's; the COUNTIFS is unfiltered because every precedence edge belongs to the Production Deployment DAG.
	CountAssertedPrecedencePairs *int `json:"count_asserted_precedence_pairs"` // Number of step-ordering pairs that were directly ASSERTED as ntwf:precedesStep edges (rollup over vw_step_precedence_closure where is_inferred = FALSE) — the hop-1 rows. The article's 4 asserted edges. Together with CountInferredPrecedencePairs (6) this sums to the 10-pair closure, making CountOfPrecedenceClosurePairs an honest asserted+inferred total rather than an unconditional count. Single-workflow note as on CountInferredPrecedencePairs: the global closure view is this workflow's.
	CountRolesWithBadFillerCardinality *int `json:"count_roles_with_bad_filler_cardinality"` // Number of roles that do NOT have exactly one filledBy arm set (rollup over Roles.HasExactlyOneFiller = FALSE). The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has 0 such roles — this is the Suite-1 functional/disjointness witness as a single integer. A non-zero value is the relational signal of the Suite-4 disjointness violation (a role filled by two agent classes, or by none). NOTE: this single-workflow model has exactly one Workflow and every Role participates in it, so the count is over all roles; a multi-workflow model would scope it through a role→workflow path.
	CountAgentTypeChanges *int `json:"count_agent_type_changes"` // Number of filledBy assignment periods that changed the agent CLASS of a role (rollup over RoleAssignments.IsAgentTypeChange = TRUE). NTWF governance distinguishes a same-class personnel/model swap from an agent-type transition; this counts the latter. NOTE: single-workflow model — every Role participates in the one workflow, so the count is over all assignment history; a multi-workflow model would scope it through a role→workflow path.
	CountComplianceAuditChanges *int `json:"count_compliance_audit_changes"` // Number of filledBy assignment periods that took a previously AI-executed binding and reassigned it to a human (rollup over RoleAssignments.RequiresComplianceAudit = TRUE). NTWF governance treats this as a data operation with compliance implications: changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent. Each such row must carry when (ValidFrom) and why (Reason). NOTE: single-workflow scoping as above.
	CountApprovalGateSteps *int `json:"count_approval_gate_steps"` // Number of this workflow's steps that are approval gates. >0 means the workflow has a blocking approval checkpoint; used by Cq2Satisfied to require that the gate exists before asking whether it has a human approver.
	CountGatesWithoutHumanApprover *int `json:"count_gates_without_human_approver"` // Number of approval gates with no resolved human approver (gate role not filled by a HumanAgent). Single-workflow model, so this global count is wholly this workflow's. Drives Cq2Satisfied (= a gate exists AND none lack a human approver).
	CountWorkflowArtifacts *int `json:"count_workflow_artifacts"` // Total artifacts produced by this workflow. With CountDerivationLinks (artifacts that have a wasDerivedFrom parent) this lets Cq4Satisfied check the provenance chain is intact: every artifact but the single origin has a parent.
	CountRolesWithEscalationViolation *int `json:"count_roles_with_escalation_violation"` // Number of roles that own an approval gate yet escalate to no one (Roles.EscalationViolation). Single-workflow model, so this global count applies to this workflow. Drives Cq6Satisfied (=0): the model's own native escalation-completeness invariant, replacing any hardcoded 'must reach the CTO' check.
	CountUnconsumedDatasets *int `json:"count_unconsumed_datasets"` // Number of datasets not consumed by any step (Datasets.IsConsumed = FALSE). Single-workflow model, so this global count applies to this workflow. Drives Cq8Satisfied (=0).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation.
	HasMoreThan1Step *bool `json:"has_more_than1_step"`
	HasConsistencyViolation *bool `json:"has_consistency_violation"` // TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
	HasAIAgentStep *bool `json:"has_ai_agent_step"` // TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
	MonthsSinceModified *int `json:"months_since_modified"` // Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
	IsStale *bool `json:"is_stale"` // TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field.
	IsStaleAndHasAIAgent *bool `json:"is_stale_and_has_ai_agent"` // The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
	InvolvesEngineeringAndLegal *bool `json:"involves_engineering_and_legal"` // TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
	CountOfPrecedenceClosurePairs *int `json:"count_of_precedence_closure_pairs"` // Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
	Cq1Satisfied *bool `json:"cq1_satisfied"` // CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
	Cq2Satisfied *bool `json:"cq2_satisfied"` // CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
	Cq3Satisfied *bool `json:"cq3_satisfied"` // CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
	Cq4Satisfied *bool `json:"cq4_satisfied"` // CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
	Cq5Satisfied *bool `json:"cq5_satisfied"` // CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
	Cq6Satisfied *bool `json:"cq6_satisfied"` // CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
	Cq7Satisfied *bool `json:"cq7_satisfied"` // CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
	Cq8Satisfied *bool `json:"cq8_satisfied"` // CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="workflows/" & {{WorkflowId}}
func (tc *Workflow) CalcRelativePath() string {
	return "workflows/" + stringVal(tc.WorkflowId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Workflow) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Workflow) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// CalcHasMoreThan1Step computes the HasMoreThan1Step calculated field
// Formula: ={{CountOfNonProposedSteps}} > 1
func (tc *Workflow) CalcHasMoreThan1Step() bool {
	return (tc.CountOfNonProposedSteps != nil && *tc.CountOfNonProposedSteps > 1)
}

// CalcHasConsistencyViolation computes the HasConsistencyViolation calculated field
// TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
// Formula: ={{CountApprovalConsistencyViolations}} > 0
func (tc *Workflow) CalcHasConsistencyViolation() bool {
	return (tc.CountApprovalConsistencyViolations != nil && *tc.CountApprovalConsistencyViolations > 0)
}

// CalcHasAIAgentStep computes the HasAIAgentStep calculated field
// TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
// Formula: ={{CountAISteps}} > 0
func (tc *Workflow) CalcHasAIAgentStep() bool {
	return (tc.CountAISteps != nil && *tc.CountAISteps > 0)
}

// CalcMonthsSinceModified computes the MonthsSinceModified calculated field
// Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
// Formula: =DATETIME_DIFF(NOW(), {{Modified}}, "months")
func (tc *Workflow) CalcMonthsSinceModified() int {
	return func() interface{} { panic("Formula parse error: Unknown function: DATETIME_DIFF") }()
}

// CalcIsStale computes the IsStale calculated field
// TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field.
// Formula: ={{MonthsSinceModified}} > {{StalenessThresholdMonths}}
func (tc *Workflow) CalcIsStale() bool {
	return (boolVal(tc.MonthsSinceModified) > boolVal(tc.StalenessThresholdMonths))
}

// CalcIsStaleAndHasAIAgent computes the IsStaleAndHasAIAgent calculated field
// The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
// Formula: =AND({{IsStale}}, {{HasAIAgentStep}})
func (tc *Workflow) CalcIsStaleAndHasAIAgent() bool {
	return (boolVal(tc.IsStale) && boolVal(tc.HasAIAgentStep))
}

// CalcInvolvesEngineeringAndLegal computes the InvolvesEngineeringAndLegal calculated field
// TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
// Formula: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0)
func (tc *Workflow) CalcInvolvesEngineeringAndLegal() bool {
	return ((tc.CountEngineeringOwnedSteps != nil && *tc.CountEngineeringOwnedSteps > 0) && (tc.CountLegalOwnedSteps != nil && *tc.CountLegalOwnedSteps > 0))
}

// CalcCountOfPrecedenceClosurePairs computes the CountOfPrecedenceClosurePairs calculated field
// Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
// Formula: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}}
func (tc *Workflow) CalcCountOfPrecedenceClosurePairs() int {
	return func() interface{} { panic("Formula parse error: '+'") }()
}

// CalcCq1Satisfied computes the Cq1Satisfied calculated field
// CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
// Formula: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2
func (tc *Workflow) CalcCq1Satisfied() bool {
	return func() interface{} { panic("Formula parse error: '-'") }()
}

// CalcCq2Satisfied computes the Cq2Satisfied calculated field
// CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
// Formula: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0)
func (tc *Workflow) CalcCq2Satisfied() bool {
	return ((tc.CountApprovalGateSteps != nil && *tc.CountApprovalGateSteps > 0) && (tc.CountGatesWithoutHumanApprover != nil && *tc.CountGatesWithoutHumanApprover == 0))
}

// CalcCq3Satisfied computes the Cq3Satisfied calculated field
// CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
// Formula: =NOT({{HasConsistencyViolation}})
func (tc *Workflow) CalcCq3Satisfied() bool {
	return !boolVal(tc.HasConsistencyViolation)
}

// CalcCq4Satisfied computes the Cq4Satisfied calculated field
// CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
// Formula: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1
func (tc *Workflow) CalcCq4Satisfied() bool {
	return func() interface{} { panic("Formula parse error: '-'") }()
}

// CalcCq5Satisfied computes the Cq5Satisfied calculated field
// CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
// Formula: =NOT({{IsStale}})
func (tc *Workflow) CalcCq5Satisfied() bool {
	return !boolVal(tc.IsStale)
}

// CalcCq6Satisfied computes the Cq6Satisfied calculated field
// CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
// Formula: ={{CountRolesWithEscalationViolation}} = 0
func (tc *Workflow) CalcCq6Satisfied() bool {
	return (tc.CountRolesWithEscalationViolation != nil && *tc.CountRolesWithEscalationViolation == 0)
}

// CalcCq7Satisfied computes the Cq7Satisfied calculated field
// CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
// Formula: ={{InvolvesEngineeringAndLegal}}
func (tc *Workflow) CalcCq7Satisfied() bool {
	return tc.InvolvesEngineeringAndLegal
}

// CalcCq8Satisfied computes the Cq8Satisfied calculated field
// CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
// Formula: ={{CountUnconsumedDatasets}} = 0
func (tc *Workflow) CalcCq8Satisfied() bool {
	return (tc.CountUnconsumedDatasets != nil && *tc.CountUnconsumedDatasets == 0)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Workflow) ComputeAll() *Workflow {
	// Level 1 calculations
	relativePath := "workflows/" + stringVal(tc.WorkflowId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
	hasMoreThan1Step := (tc.CountOfNonProposedSteps != nil && *tc.CountOfNonProposedSteps > 1)
	hasConsistencyViolation := (tc.CountApprovalConsistencyViolations != nil && *tc.CountApprovalConsistencyViolations > 0)
	hasAIAgentStep := (tc.CountAISteps != nil && *tc.CountAISteps > 0)
	monthsSinceModified := func() interface{} { panic("Formula parse error: Unknown function: DATETIME_DIFF") }()
	involvesEngineeringAndLegal := ((tc.CountEngineeringOwnedSteps != nil && *tc.CountEngineeringOwnedSteps > 0) && (tc.CountLegalOwnedSteps != nil && *tc.CountLegalOwnedSteps > 0))
	countOfPrecedenceClosurePairs := func() interface{} { panic("Formula parse error: '+'") }()
	cq2Satisfied := ((tc.CountApprovalGateSteps != nil && *tc.CountApprovalGateSteps > 0) && (tc.CountGatesWithoutHumanApprover != nil && *tc.CountGatesWithoutHumanApprover == 0))
	cq4Satisfied := func() interface{} { panic("Formula parse error: '-'") }()
	cq6Satisfied := (tc.CountRolesWithEscalationViolation != nil && *tc.CountRolesWithEscalationViolation == 0)
	cq8Satisfied := (tc.CountUnconsumedDatasets != nil && *tc.CountUnconsumedDatasets == 0)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")
	isStale := (monthsSinceModified > boolVal(tc.StalenessThresholdMonths))
	cq1Satisfied := func() interface{} { panic("Formula parse error: '-'") }()
	cq3Satisfied := !hasConsistencyViolation
	cq7Satisfied := involvesEngineeringAndLegal

	// Level 3 calculations
	isStaleAndHasAIAgent := (isStale && hasAIAgentStep)
	cq5Satisfied := !isStale

	return &Workflow{
		WorkflowId: tc.WorkflowId,
		DisplayName: tc.DisplayName,
		Title: tc.Title,
		Description: tc.Description,
		Identifier: tc.Identifier,
		Modified: tc.Modified,
		Created: tc.Created,
		StalenessThresholdMonths: tc.StalenessThresholdMonths,
		WorkflowStatus: tc.WorkflowStatus,
		WorkflowSteps: tc.WorkflowSteps,
		CountOfNonProposedSteps: tc.CountOfNonProposedSteps,
		CountAISteps: tc.CountAISteps,
		CountHumanSteps: tc.CountHumanSteps,
		CountHumanRequiredSteps: tc.CountHumanRequiredSteps,
		CountApprovalConsistencyViolations: tc.CountApprovalConsistencyViolations,
		CountDerivationLinks: tc.CountDerivationLinks,
		CountLegalOwnedSteps: tc.CountLegalOwnedSteps,
		CountEngineeringOwnedSteps: tc.CountEngineeringOwnedSteps,
		CountInferredPrecedencePairs: tc.CountInferredPrecedencePairs,
		CountAssertedPrecedencePairs: tc.CountAssertedPrecedencePairs,
		CountRolesWithBadFillerCardinality: tc.CountRolesWithBadFillerCardinality,
		CountAgentTypeChanges: tc.CountAgentTypeChanges,
		CountComplianceAuditChanges: tc.CountComplianceAuditChanges,
		CountApprovalGateSteps: tc.CountApprovalGateSteps,
		CountGatesWithoutHumanApprover: tc.CountGatesWithoutHumanApprover,
		CountWorkflowArtifacts: tc.CountWorkflowArtifacts,
		CountRolesWithEscalationViolation: tc.CountRolesWithEscalationViolation,
		CountUnconsumedDatasets: tc.CountUnconsumedDatasets,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		HasMoreThan1Step: &hasMoreThan1Step,
		HasConsistencyViolation: &hasConsistencyViolation,
		HasAIAgentStep: &hasAIAgentStep,
		MonthsSinceModified: &monthsSinceModified,
		IsStale: &isStale,
		IsStaleAndHasAIAgent: &isStaleAndHasAIAgent,
		InvolvesEngineeringAndLegal: &involvesEngineeringAndLegal,
		CountOfPrecedenceClosurePairs: &countOfPrecedenceClosurePairs,
		Cq1Satisfied: &cq1Satisfied,
		Cq2Satisfied: &cq2Satisfied,
		Cq3Satisfied: &cq3Satisfied,
		Cq4Satisfied: &cq4Satisfied,
		Cq5Satisfied: &cq5Satisfied,
		Cq6Satisfied: &cq6Satisfied,
		Cq7Satisfied: &cq7Satisfied,
		Cq8Satisfied: &cq8Satisfied,
	}
}

// =============================================================================
// WORKFLOWSTEPS TABLE
// Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
// =============================================================================

// WorkflowStep represents a row in the WorkflowSteps table
// Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
type WorkflowStep struct {
	WorkflowStepId string `json:"workflow_step_id"`
	ParentPath *string `json:"parent_path"` // Helper: the Workflows parent's RelativePath, pulled across the Workflow FK. Exists so RelativePath can concatenate the '/steps/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	DisplayName *string `json:"display_name"`
	Workflow *string `json:"workflow"` // Forward foreign key to the parent workflow (ntwf:isStepOf) — the authoritative stored link from step to its containing workflow; every per-workflow rollup (CountOfNonProposedSteps, the department-owned counts, etc.) reads it. This IS the stored column, not a derived inverse: isReversed is false.
	PrecedingStepCount *int `json:"preceding_step_count"` // Number of steps that TRANSITIVELY precede this step in the ntwf:precedesStep ordering — a rollup over the closure view vw_step_precedence_closure counting rows whose to_id is this step (i.e. this step's ancestors). On the linear Production Deployment chain: 0,1,2,3,4. Derived purely from the asserted StepPrecedence edges via their transitive closure; nothing is hand-entered. SequencePosition is this + 1.
	SequencePositionOverride *int `json:"sequence_position_override"` // OPTIONAL hand-asserted ordinal position — the article's pure ntwf:sequencePosition functional datatype property, preserved as an override slot. NULL on the linear Production Deployment chain (the inference is unambiguous, so nothing is pinned). When set, it wins over InferredSequencePosition in the resolved SequencePosition — this is how a modeler recovers the owl:FunctionalProperty 'exactly one distinct position per step' guarantee on a partial order / branch where the inferred rank would tie. Maps to ntwf:sequencePosition (the article's asserted functional property).
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
	OwningDepartment *string `json:"owning_department"` // The department that owns this step's assigned role, resolved through AssignedRole → Roles.OwnedBy. Lets a workflow report which departments its steps touch (CQ7: 'which workflows involve both Engineering and Legal, and at what steps do they intersect').
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"`
	InferredSequencePosition *int `json:"inferred_sequence_position"` // The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering).
	SequencePosition *int `json:"sequence_position"` // The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
	ExecutingAgentType *string `json:"executing_agent_type"` // Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
	IsExecutedByAI *bool `json:"is_executed_by_ai"` // TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
	IsExecutedByHuman *bool `json:"is_executed_by_human"` // TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
	IsApprovalGate *bool `json:"is_approval_gate"` // TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
	ApprovalConsistencyViolation *bool `json:"approval_consistency_violation"` // Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
	ApprovalIsHumanFilled *bool `json:"approval_is_human_filled"` // Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
	IsLegalOwned *bool `json:"is_legal_owned"` // TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
	IsEngineeringOwned *bool `json:"is_engineering_owned"` // TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/steps/" & {{WorkflowStepId}}
func (tc *WorkflowStep) CalcRelativePath() string {
	return stringVal(tc.ParentPath) + "/steps/" + stringVal(tc.WorkflowStepId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *WorkflowStep) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *WorkflowStep) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// CalcInferredSequencePosition computes the InferredSequencePosition calculated field
// The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering).
// Formula: ={{PrecedingStepCount}} + 1
func (tc *WorkflowStep) CalcInferredSequencePosition() int {
	return func() interface{} { panic("Formula parse error: '+'") }()
}

// CalcSequencePosition computes the SequencePosition calculated field
// The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
// Formula: =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, {{InferredSequencePosition}})
func (tc *WorkflowStep) CalcSequencePosition() int {
	return func() string { if (stringVal(tc.SequencePositionOverride) != "") { return tc.SequencePositionOverride }; return tc.InferredSequencePosition }()
}

// CalcExecutingAgentType computes the ExecutingAgentType calculated field
// Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
// Formula: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *WorkflowStep) CalcExecutingAgentType() string {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcIsExecutedByAI computes the IsExecutedByAI calculated field
// TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
// Formula: =NOT(ISBLANK({{ExecutingAIAgent}}))
func (tc *WorkflowStep) CalcIsExecutedByAI() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcIsExecutedByHuman computes the IsExecutedByHuman calculated field
// TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
// Formula: =NOT(ISBLANK({{ExecutingHumanAgent}}))
func (tc *WorkflowStep) CalcIsExecutedByHuman() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcIsApprovalGate computes the IsApprovalGate calculated field
// TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
// Formula: =NOT(ISBLANK({{ApprovalGate}}))
func (tc *WorkflowStep) CalcIsApprovalGate() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcApprovalConsistencyViolation computes the ApprovalConsistencyViolation calculated field
// Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
// Formula: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}}))
func (tc *WorkflowStep) CalcApprovalConsistencyViolation() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcApprovalIsHumanFilled computes the ApprovalIsHumanFilled calculated field
// Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
// Formula: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE)
func (tc *WorkflowStep) CalcApprovalIsHumanFilled() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcIsLegalOwned computes the IsLegalOwned calculated field
// TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
// Formula: ={{OwningDepartment}} = "ntwf-legal-dept"
func (tc *WorkflowStep) CalcIsLegalOwned() bool {
	return (stringVal(tc.OwningDepartment) == "ntwf-legal-dept")
}

// CalcIsEngineeringOwned computes the IsEngineeringOwned calculated field
// TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
// Formula: ={{OwningDepartment}} = "ntwf-engineering"
func (tc *WorkflowStep) CalcIsEngineeringOwned() bool {
	return (stringVal(tc.OwningDepartment) == "ntwf-engineering")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *WorkflowStep) ComputeAll() *WorkflowStep {
	// Level 1 calculations
	relativePath := stringVal(tc.ParentPath) + "/steps/" + stringVal(tc.WorkflowStepId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
	inferredSequencePosition := func() interface{} { panic("Formula parse error: '+'") }()
	executingAgentType := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	isExecutedByAI := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	isExecutedByHuman := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	isApprovalGate := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	approvalConsistencyViolation := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	approvalIsHumanFilled := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	isLegalOwned := (stringVal(tc.OwningDepartment) == "ntwf-legal-dept")
	isEngineeringOwned := (stringVal(tc.OwningDepartment) == "ntwf-engineering")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")
	sequencePosition := func() string { if (stringVal(tc.SequencePositionOverride) != "") { return tc.SequencePositionOverride }; return inferredSequencePosition }()

	return &WorkflowStep{
		WorkflowStepId: tc.WorkflowStepId,
		ParentPath: tc.ParentPath,
		DisplayName: tc.DisplayName,
		Workflow: tc.Workflow,
		PrecedingStepCount: tc.PrecedingStepCount,
		SequencePositionOverride: tc.SequencePositionOverride,
		AssignedRole: tc.AssignedRole,
		RequiresHumanApproval: tc.RequiresHumanApproval,
		StepDurationMinutes: tc.StepDurationMinutes,
		ConsumesDataset: tc.ConsumesDataset,
		ProducesArtifacts: tc.ProducesArtifacts,
		RequiresArtifacts: tc.RequiresArtifacts,
		ApprovalGate: tc.ApprovalGate,
		Precedes: tc.Precedes,
		PrecededBy: tc.PrecededBy,
		ExecutingHumanAgent: tc.ExecutingHumanAgent,
		ExecutingAIAgent: tc.ExecutingAIAgent,
		ExecutingAutomatedPipeline: tc.ExecutingAutomatedPipeline,
		OwningDepartment: tc.OwningDepartment,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		InferredSequencePosition: &inferredSequencePosition,
		SequencePosition: &sequencePosition,
		ExecutingAgentType: nilIfEmpty(executingAgentType),
		IsExecutedByAI: &isExecutedByAI,
		IsExecutedByHuman: &isExecutedByHuman,
		IsApprovalGate: &isApprovalGate,
		ApprovalConsistencyViolation: &approvalConsistencyViolation,
		ApprovalIsHumanFilled: &approvalIsHumanFilled,
		IsLegalOwned: &isLegalOwned,
		IsEngineeringOwned: &isEngineeringOwned,
	}
}

// =============================================================================
// APPROVALGATES TABLE
// Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
// =============================================================================

// ApprovalGate represents a row in the ApprovalGates table
// Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
type ApprovalGate struct {
	ApprovalGateId string `json:"approval_gate_id"`
	ParentPath *string `json:"parent_path"` // Helper: the WorkflowSteps parent's RelativePath, pulled across the WorkflowStep FK. Exists so RelativePath can concatenate the '/approval-gates/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	DisplayName *string `json:"display_name"`
	WorkflowStep *string `json:"workflow_step"` // 1:1 FK to the WorkflowStep this gate specializes (subtype shared key). This is the relational expression of ntwf:ApprovalGate rdfs:subClassOf WorkflowStep: the gate row adds escalationThresholdHours to its step. Inverse is WorkflowSteps.ApprovalGate.
	EscalationThresholdHours *int `json:"escalation_threshold_hours"` // Integer number of hours that may elapse on a pending gate before the ntwf:delegatesTo chain activates. Maps to ntwf:escalationThresholdHours. Domain applies only to ApprovalGate individuals — which is exactly why the gate is its own subtype table and this attribute does not live on every WorkflowStep.
	GateRole *string `json:"gate_role"` // The role responsible for this gate's underlying step, resolved through WorkflowStep → WorkflowSteps.AssignedRole. First hop of the CQ2 chain (gate → role → approver).
	GateApproverHuman *string `json:"gate_approver_human"` // The human agent who approves at this gate, resolved through the two-hop chain gate → GateRole → Roles.FilledByHumanAgent. Answers CQ2 ('who is responsible for approving a production deployment') directly: the release-approval gate resolves to the Release Manager role, filled by Maria Gonzalez.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"`
	HasHumanApprover *bool `json:"has_human_approver"` // TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/approval-gates/" & {{ApprovalGateId}}
func (tc *ApprovalGate) CalcRelativePath() string {
	return stringVal(tc.ParentPath) + "/approval-gates/" + stringVal(tc.ApprovalGateId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ApprovalGate) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *ApprovalGate) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// CalcHasHumanApprover computes the HasHumanApprover calculated field
// TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads.
// Formula: =NOT(ISBLANK({{GateApproverHuman}}))
func (tc *ApprovalGate) CalcHasHumanApprover() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *ApprovalGate) ComputeAll() *ApprovalGate {
	// Level 1 calculations
	relativePath := stringVal(tc.ParentPath) + "/approval-gates/" + stringVal(tc.ApprovalGateId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
	hasHumanApprover := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &ApprovalGate{
		ApprovalGateId: tc.ApprovalGateId,
		ParentPath: tc.ParentPath,
		DisplayName: tc.DisplayName,
		WorkflowStep: tc.WorkflowStep,
		EscalationThresholdHours: tc.EscalationThresholdHours,
		GateRole: tc.GateRole,
		GateApproverHuman: tc.GateApproverHuman,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		HasHumanApprover: &hasHumanApprover,
	}
}

// =============================================================================
// STEPPRECEDENCE TABLE
// Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
// =============================================================================

// StepPrecedence represents a row in the StepPrecedence table
// Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
type StepPrecedence struct {
	StepPrecedenceId string `json:"step_precedence_id"`
	ParentPath *string `json:"parent_path"` // Helper: the WorkflowSteps parent's RelativePath, pulled across the FromStep FK. Exists so RelativePath can concatenate the '/precedence/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
	FromStep string `json:"from_step"` // FK to the predecessor WorkflowStep — the step that comes BEFORE. The source of the ntwf:precedesStep edge. Inverse is WorkflowSteps.Precedes.
	ToStep string `json:"to_step"` // FK to the successor WorkflowStep — the step that comes AFTER. The target of the ntwf:precedesStep edge. Inverse is WorkflowSteps.PrecededBy.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/precedence/" & {{StepPrecedenceId}}
func (tc *StepPrecedence) CalcRelativePath() string {
	return stringVal(tc.ParentPath) + "/precedence/" + stringVal(tc.StepPrecedenceId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *StepPrecedence) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
// Formula: ={{FromStep}} & " -> " & {{ToStep}}
func (tc *StepPrecedence) CalcName() string {
	return stringVal(tc.FromStep) + " -> " + stringVal(tc.ToStep)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *StepPrecedence) ComputeAll() *StepPrecedence {
	// Level 1 calculations
	relativePath := stringVal(tc.ParentPath) + "/precedence/" + stringVal(tc.StepPrecedenceId)
	name := stringVal(tc.FromStep) + " -> " + stringVal(tc.ToStep)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &StepPrecedence{
		StepPrecedenceId: tc.StepPrecedenceId,
		ParentPath: tc.ParentPath,
		FromStep: tc.FromStep,
		ToStep: tc.ToStep,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// ROLES TABLE
// Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
// =============================================================================

// Role represents a row in the Roles table
// Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
type Role struct {
	RoleId string `json:"role_id"`
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
	FillsApprovalGate *int `json:"fills_approval_gate"` // Number of this role's assigned WorkflowSteps that are approval gates (rollup over WorkflowSteps.IsApprovalGate). Greater than zero marks a role that owns a blocking decision checkpoint and therefore MUST have a complete delegatesTo escalation path — the precondition for EscalationViolation. Worked example: 1 for the Release Manager (who fills the Release Approval Gate), 0 for every other role.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"`
	FilledByArmCount *int `json:"filled_by_arm_count"` // Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint.
	HasExactlyOneFiller *bool `json:"has_exactly_one_filler"` // Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
	FillerType *string `json:"filler_type"` // Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
	EscalationViolation *bool `json:"escalation_violation"` // Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="roles/" & {{RoleId}}
func (tc *Role) CalcRelativePath() string {
	return "roles/" + stringVal(tc.RoleId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Role) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Role) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// CalcFilledByArmCount computes the FilledByArmCount calculated field
// Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint.
// Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0)
func (tc *Role) CalcFilledByArmCount() int {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcHasExactlyOneFiller computes the HasExactlyOneFiller calculated field
// Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
// Formula: ={{FilledByArmCount}} = 1
func (tc *Role) CalcHasExactlyOneFiller() bool {
	return (tc.FilledByArmCount != nil && *tc.FilledByArmCount == 1)
}

// CalcFillerType computes the FillerType calculated field
// Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
// Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *Role) CalcFillerType() string {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcEscalationViolation computes the EscalationViolation calculated field
// Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
// Formula: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}}))
func (tc *Role) CalcEscalationViolation() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Role) ComputeAll() *Role {
	// Level 1 calculations
	relativePath := "roles/" + stringVal(tc.RoleId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
	filledByArmCount := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	fillerType := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	escalationViolation := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")
	hasExactlyOneFiller := (filledByArmCount == 1)

	return &Role{
		RoleId: tc.RoleId,
		DisplayName: tc.DisplayName,
		Label: tc.Label,
		Comment: tc.Comment,
		HasCapability: tc.HasCapability,
		FilledByHumanAgent: tc.FilledByHumanAgent,
		FilledByAIAgent: tc.FilledByAIAgent,
		FilledByAutomatedPipeline: tc.FilledByAutomatedPipeline,
		OwnedBy: tc.OwnedBy,
		DelegatesTo: tc.DelegatesTo,
		WorkflowSteps: tc.WorkflowSteps,
		FromDelegatesTo: tc.FromDelegatesTo,
		RoleAssignments: tc.RoleAssignments,
		FillsApprovalGate: tc.FillsApprovalGate,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		FilledByArmCount: &filledByArmCount,
		HasExactlyOneFiller: &hasExactlyOneFiller,
		FillerType: nilIfEmpty(fillerType),
		EscalationViolation: &escalationViolation,
	}
}

// =============================================================================
// ROLEASSIGNMENTS TABLE
// Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
// =============================================================================

// RoleAssignment represents a row in the RoleAssignments table
// Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
type RoleAssignment struct {
	RoleAssignmentId string `json:"role_assignment_id"`
	ParentPath *string `json:"parent_path"` // Helper: the Roles parent's RelativePath, pulled across the Role FK. Exists so RelativePath can concatenate the '/assignments/' segment using only local-field '&' concat.
	Role string `json:"role"` // FK to the Role this assignment binds an agent to. The subject of the historical ntwf:filledBy triple.
	FilledByHumanAgent *string `json:"filled_by_human_agent"` // One arm of the polymorphic filledBy binding for this assignment period: FK to the HumanAgent who filled the role during this window. Exactly one filler arm is set per assignment.
	FilledByAIAgent *string `json:"filled_by_ai_agent"` // One arm of the polymorphic filledBy binding: FK to the AIAgent who filled the role during this window. Exactly one filler arm is set per assignment.
	FilledByAutomatedPipeline *string `json:"filled_by_automated_pipeline"` // One arm of the polymorphic filledBy binding: FK to the AutomatedPipeline that filled the role during this window. Exactly one filler arm is set per assignment.
	ValidFrom string `json:"valid_from"` // Start of the validity period for this filledBy binding (inclusive). A retained/versioned triple carries the validity period. ISO date.
	ValidTo *string `json:"valid_to"` // End of the validity period for this filledBy binding (exclusive). Blank means the binding is still current — this is the live ntwf:filledBy value mirrored on Roles. A non-blank value means the binding was superseded; the row is retained (not deleted) to preserve provenance.
	Reason *string `json:"reason"` // The WHY of the change: the audit record must reflect when that transition happened and why. e.g. 'initial assignment', 'departure / backfill', 'model upgrade', 'compliance reassignment to human'.
	PriorFillerType *string `json:"prior_filler_type"` // The agent class (HumanAgent / AIAgent / AutomatedPipeline) of the binding this assignment SUPERSEDED, or blank for the first assignment of a role. Lets the agent-type-change audit (AIAgent -> HumanAgent) be witnessed without re-deriving from the prior row.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
	Name *string `json:"name"` // Human-readable label for this assignment period: the role and the validity window.
	FillerType *string `json:"filler_type"` // Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
	IsCurrent *bool `json:"is_current"` // TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips.
	WasActiveAsOfAuditDate *bool `json:"was_active_as_of_audit_date"` // NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained.
	IsAgentTypeChange *bool `json:"is_agent_type_change"` // TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight.
	RequiresComplianceAudit *bool `json:"requires_compliance_audit"` // Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
// Formula: ={{ParentPath}} & "/assignments/" & {{RoleAssignmentId}}
func (tc *RoleAssignment) CalcRelativePath() string {
	return stringVal(tc.ParentPath) + "/assignments/" + stringVal(tc.RoleAssignmentId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *RoleAssignment) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Human-readable label for this assignment period: the role and the validity window.
// Formula: ={{Role}} & " [" & {{ValidFrom}} & " -> " & IF(ISBLANK({{ValidTo}}), "open", {{ValidTo}}) & "]"
func (tc *RoleAssignment) CalcName() string {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcFillerType computes the FillerType calculated field
// Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
// Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *RoleAssignment) CalcFillerType() string {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcIsCurrent computes the IsCurrent calculated field
// TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips.
// Formula: =ISBLANK({{ValidTo}})
func (tc *RoleAssignment) CalcIsCurrent() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcWasActiveAsOfAuditDate computes the WasActiveAsOfAuditDate calculated field
// NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained.
// Formula: =AND({{ValidFrom}} <= "2026-03-01", OR(ISBLANK({{ValidTo}}), {{ValidTo}} > "2026-03-01"))
func (tc *RoleAssignment) CalcWasActiveAsOfAuditDate() bool {
	return func() interface{} { panic("Formula parse error: '<='") }()
}

// CalcIsAgentTypeChange computes the IsAgentTypeChange calculated field
// TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight.
// Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} <> {{FillerType}})
func (tc *RoleAssignment) CalcIsAgentTypeChange() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcRequiresComplianceAudit computes the RequiresComplianceAudit calculated field
// Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
// Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent")
func (tc *RoleAssignment) CalcRequiresComplianceAudit() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *RoleAssignment) ComputeAll() *RoleAssignment {
	// Level 1 calculations
	relativePath := stringVal(tc.ParentPath) + "/assignments/" + stringVal(tc.RoleAssignmentId)
	name := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	fillerType := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	isCurrent := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	wasActiveAsOfAuditDate := func() interface{} { panic("Formula parse error: '<='") }()

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")
	isAgentTypeChange := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	requiresComplianceAudit := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()

	return &RoleAssignment{
		RoleAssignmentId: tc.RoleAssignmentId,
		ParentPath: tc.ParentPath,
		Role: tc.Role,
		FilledByHumanAgent: tc.FilledByHumanAgent,
		FilledByAIAgent: tc.FilledByAIAgent,
		FilledByAutomatedPipeline: tc.FilledByAutomatedPipeline,
		ValidFrom: tc.ValidFrom,
		ValidTo: tc.ValidTo,
		Reason: tc.Reason,
		PriorFillerType: tc.PriorFillerType,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		FillerType: nilIfEmpty(fillerType),
		IsCurrent: &isCurrent,
		WasActiveAsOfAuditDate: &wasActiveAsOfAuditDate,
		IsAgentTypeChange: &isAgentTypeChange,
		RequiresComplianceAudit: &requiresComplianceAudit,
	}
}

// =============================================================================
// DEPARTMENTS TABLE
// Table: Departments. The NTWF Department class — schema:Organization. First-class entity that enables cross-department intersection queries (CQ7: which workflows involve both Engineering and Legal?). Roles are ownedBy a department.
// =============================================================================

// Department represents a row in the Departments table
// Table: Departments. The NTWF Department class — schema:Organization. First-class entity that enables cross-department intersection queries (CQ7: which workflows involve both Engineering and Legal?). Roles are ownedBy a department.
type Department struct {
	DepartmentId string `json:"department_id"`
	Title *string `json:"title"` // Formal organizational title of the department. Maps to schema:name / dct:title.
	DisplayName *string `json:"display_name"` // Machine-friendly name for programmatic reference.
	Roles *string `json:"roles"` // Back-reference to roles owned by this department. Inverse of Roles.OwnedBy.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	Name *string `json:"name"` // Human-readable display name of the department. Should match organizational terminology for stakeholder communication.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="departments/" & {{DepartmentId}}
func (tc *Department) CalcRelativePath() string {
	return "departments/" + stringVal(tc.DepartmentId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Department) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Human-readable display name of the department. Should match organizational terminology for stakeholder communication.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *Department) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Department) ComputeAll() *Department {
	// Level 1 calculations
	relativePath := "departments/" + stringVal(tc.DepartmentId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &Department{
		DepartmentId: tc.DepartmentId,
		Title: tc.Title,
		DisplayName: tc.DisplayName,
		Roles: tc.Roles,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// HUMANAGENTS TABLE
// Table: HumanAgents. The NTWF HumanAgent class — foaf:Person + prov:Agent. The only agent type permitted to fill roles whose step has requiresHumanApproval. Disjoint with AIAgent and AutomatedPipeline.
// =============================================================================

// HumanAgent represents a row in the HumanAgents table
// Table: HumanAgents. The NTWF HumanAgent class — foaf:Person + prov:Agent. The only agent type permitted to fill roles whose step has requiresHumanApproval. Disjoint with AIAgent and AutomatedPipeline.
type HumanAgent struct {
	HumanAgentId string `json:"human_agent_id"`
	Name *string `json:"name"` // Full name of the person. Maps to foaf:name. Note: FOAF's name property is appropriate for persons, not for software systems (which use schema:name).
	DisplayName *string `json:"display_name"`
	Mbox *string `json:"mbox"` // Email address of the person. Maps to foaf:mbox. Used for notifications and organizational directory integration.
	Roles *string `json:"roles"` // Back-reference to roles currently filled by this agent. Inverse of Roles.FilledByHumanAgent.
	RoleAssignments *string `json:"role_assignments"` // Back-reference to historical filledBy assignment periods in which this human filled a role. Inverse of RoleAssignments.FilledByHumanAgent.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="human-agents/" & {{HumanAgentId}}
func (tc *HumanAgent) CalcRelativePath() string {
	return "human-agents/" + stringVal(tc.HumanAgentId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *HumanAgent) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *HumanAgent) ComputeAll() *HumanAgent {
	// Level 1 calculations
	relativePath := "human-agents/" + stringVal(tc.HumanAgentId)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &HumanAgent{
		HumanAgentId: tc.HumanAgentId,
		Name: tc.Name,
		DisplayName: tc.DisplayName,
		Mbox: tc.Mbox,
		Roles: tc.Roles,
		RoleAssignments: tc.RoleAssignments,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
	}
}

// =============================================================================
// AIAGENTS TABLE
// Table: AIAgents. The NTWF AIAgent class — prov:SoftwareAgent + ntwf:modelVersion. Distinguished from AutomatedPipeline by probabilistic (vs. deterministic) output semantics. Disjoint with HumanAgent and AutomatedPipeline. May never fill a role whose step has requiresHumanApproval.
// =============================================================================

// AIAgent represents a row in the AIAgents table
// Table: AIAgents. The NTWF AIAgent class — prov:SoftwareAgent + ntwf:modelVersion. Distinguished from AutomatedPipeline by probabilistic (vs. deterministic) output semantics. Disjoint with HumanAgent and AutomatedPipeline. May never fill a role whose step has requiresHumanApproval.
type AIAgent struct {
	AIAgentId string `json:"ai_agent_id"`
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
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="ai-agents/" & {{AIAgentId}}
func (tc *AIAgent) CalcRelativePath() string {
	return "ai-agents/" + stringVal(tc.AIAgentId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *AIAgent) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *AIAgent) ComputeAll() *AIAgent {
	// Level 1 calculations
	relativePath := "ai-agents/" + stringVal(tc.AIAgentId)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &AIAgent{
		AIAgentId: tc.AIAgentId,
		Name: tc.Name,
		Title: tc.Title,
		DisplayName: tc.DisplayName,
		ModelVersion: tc.ModelVersion,
		DeployedOn: tc.DeployedOn,
		Roles: tc.Roles,
		RoleAssignments: tc.RoleAssignments,
		AttributedArtifacts: tc.AttributedArtifacts,
		CountAttributedArtifacts: tc.CountAttributedArtifacts,
		CountImpactedWorkflows: tc.CountImpactedWorkflows,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
	}
}

// =============================================================================
// AUTOMATEDPIPELINES TABLE
// Table: AutomatedPipelines. The NTWF AutomatedPipeline class — prov:SoftwareAgent + schema:SoftwareApplication. Distinguished from AIAgent by deterministic (vs. probabilistic) output semantics. Disjoint with HumanAgent and AIAgent. Carries schema:name, not foaf:name.
// =============================================================================

// AutomatedPipeline represents a row in the AutomatedPipelines table
// Table: AutomatedPipelines. The NTWF AutomatedPipeline class — prov:SoftwareAgent + schema:SoftwareApplication. Distinguished from AIAgent by deterministic (vs. probabilistic) output semantics. Disjoint with HumanAgent and AIAgent. Carries schema:name, not foaf:name.
type AutomatedPipeline struct {
	AutomatedPipelineId string `json:"automated_pipeline_id"`
	Name *string `json:"name"` // Display name of the pipeline. Maps to schema:name (appropriate for software systems, unlike foaf:name which is for persons).
	Description *string `json:"description"` // Description of what the pipeline does and its execution semantics (deterministic, no probabilistic output).
	DisplayName *string `json:"display_name"`
	Roles *string `json:"roles"` // Back-reference to roles currently filled by this pipeline. Inverse of Roles.FilledByAutomatedPipeline.
	RoleAssignments *string `json:"role_assignments"` // Back-reference to historical filledBy assignment periods filled by this pipeline. Inverse of RoleAssignments.FilledByAutomatedPipeline.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="automated-pipelines/" & {{AutomatedPipelineId}}
func (tc *AutomatedPipeline) CalcRelativePath() string {
	return "automated-pipelines/" + stringVal(tc.AutomatedPipelineId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *AutomatedPipeline) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *AutomatedPipeline) ComputeAll() *AutomatedPipeline {
	// Level 1 calculations
	relativePath := "automated-pipelines/" + stringVal(tc.AutomatedPipelineId)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &AutomatedPipeline{
		AutomatedPipelineId: tc.AutomatedPipelineId,
		Name: tc.Name,
		Description: tc.Description,
		DisplayName: tc.DisplayName,
		Roles: tc.Roles,
		RoleAssignments: tc.RoleAssignments,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
	}
}

// =============================================================================
// WORKFLOWSTATUSCONCEPTS TABLE
// SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows.
// =============================================================================

// WorkflowStatusConcept represents a row in the WorkflowStatusConcepts table
// SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows.
type WorkflowStatusConcept struct {
	ConceptId string `json:"concept_id"`
	PrefLabel string `json:"pref_label"` // Preferred human-readable label. Maps to skos:prefLabel.
	AltLabel *string `json:"alt_label"` // Alternative label or synonym. Maps to skos:altLabel.
	Definition *string `json:"definition"` // Formal definition of the concept. Maps to skos:definition.
	ScopeNote *string `json:"scope_note"` // Usage guidance for the concept. Maps to skos:scopeNote.
	Workflows *string `json:"workflows"` // Back-reference to workflows currently in this status. Inverse of Workflows.WorkflowStatus.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="concepts/workflow-status/" & {{ConceptId}}
func (tc *WorkflowStatusConcept) CalcRelativePath() string {
	return "concepts/workflow-status/" + stringVal(tc.ConceptId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *WorkflowStatusConcept) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *WorkflowStatusConcept) ComputeAll() *WorkflowStatusConcept {
	// Level 1 calculations
	relativePath := "concepts/workflow-status/" + stringVal(tc.ConceptId)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &WorkflowStatusConcept{
		ConceptId: tc.ConceptId,
		PrefLabel: tc.PrefLabel,
		AltLabel: tc.AltLabel,
		Definition: tc.Definition,
		ScopeNote: tc.ScopeNote,
		Workflows: tc.Workflows,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
	}
}

// =============================================================================
// AGENTCAPABILITYCONCEPTS TABLE
// SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox.
// =============================================================================

// AgentCapabilityConcept represents a row in the AgentCapabilityConcepts table
// SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox.
type AgentCapabilityConcept struct {
	ConceptId string `json:"concept_id"`
	PrefLabel string `json:"pref_label"` // Preferred label. Maps to skos:prefLabel.
	AltLabel *string `json:"alt_label"` // Alternative label. Maps to skos:altLabel.
	Definition *string `json:"definition"` // Formal definition. Maps to skos:definition.
	ScopeNote *string `json:"scope_note"` // Usage guidance. Maps to skos:scopeNote.
	Roles *string `json:"roles"` // Back-reference to roles requiring this capability. Inverse of Roles.HasCapability.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="concepts/agent-capability/" & {{ConceptId}}
func (tc *AgentCapabilityConcept) CalcRelativePath() string {
	return "concepts/agent-capability/" + stringVal(tc.ConceptId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *AgentCapabilityConcept) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *AgentCapabilityConcept) ComputeAll() *AgentCapabilityConcept {
	// Level 1 calculations
	relativePath := "concepts/agent-capability/" + stringVal(tc.ConceptId)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &AgentCapabilityConcept{
		ConceptId: tc.ConceptId,
		PrefLabel: tc.PrefLabel,
		AltLabel: tc.AltLabel,
		Definition: tc.Definition,
		ScopeNote: tc.ScopeNote,
		Roles: tc.Roles,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
	}
}

// =============================================================================
// ARTIFACTTYPECONCEPTS TABLE
// SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts.
// =============================================================================

// ArtifactTypeConcept represents a row in the ArtifactTypeConcepts table
// SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts.
type ArtifactTypeConcept struct {
	ConceptId string `json:"concept_id"`
	PrefLabel string `json:"pref_label"` // Preferred human-readable label. Maps to skos:prefLabel.
	AltLabel *string `json:"alt_label"` // Alternative label or synonym. Maps to skos:altLabel.
	Definition *string `json:"definition"` // Formal definition of the concept. Maps to skos:definition.
	ScopeNote *string `json:"scope_note"` // Usage note clarifying boundaries. Maps to skos:scopeNote.
	WorkflowArtifacts *string `json:"workflow_artifacts"` // Back-reference to WorkflowArtifacts classified under this concept. Inverse of WorkflowArtifacts.ArtifactType.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key.
// Formula: ="concepts/artifact-type/" & {{ConceptId}}
func (tc *ArtifactTypeConcept) CalcRelativePath() string {
	return "concepts/artifact-type/" + stringVal(tc.ConceptId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ArtifactTypeConcept) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *ArtifactTypeConcept) ComputeAll() *ArtifactTypeConcept {
	// Level 1 calculations
	relativePath := "concepts/artifact-type/" + stringVal(tc.ConceptId)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &ArtifactTypeConcept{
		ConceptId: tc.ConceptId,
		PrefLabel: tc.PrefLabel,
		AltLabel: tc.AltLabel,
		Definition: tc.Definition,
		ScopeNote: tc.ScopeNote,
		WorkflowArtifacts: tc.WorkflowArtifacts,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
	}
}

// =============================================================================
// DATASETS TABLE
// DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
// =============================================================================

// Dataset represents a row in the Datasets table
// DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
type Dataset struct {
	DatasetId string `json:"dataset_id"`
	Title string `json:"title"` // Human-readable dataset name. Maps to dct:title.
	Identifier *string `json:"identifier"` // External system identifier. Maps to dct:identifier. Used for cross-referencing with data catalogs.
	Modified *string `json:"modified"` // Last modification timestamp. Maps to dct:modified.
	DistributionUrl *string `json:"distribution_url"` // URL of the data distribution. Maps to dcat:Distribution. The access endpoint for the dataset.
	ConsumedBySteps *string `json:"consumed_by_steps"` // Back-reference to WorkflowSteps that consume this dataset. Inverse of WorkflowSteps.ConsumesDataset. Marked isReversed so every substrate DERIVES it from the forward FK (a reverse lookup over WorkflowSteps.ConsumesDataset) instead of storing it — keeping the two sides from drifting when the forward FK is edited.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	IsConsumed *bool `json:"is_consumed"` // TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
// Formula: ="datasets/" & {{DatasetId}}
func (tc *Dataset) CalcRelativePath() string {
	return "datasets/" + stringVal(tc.DatasetId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Dataset) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcIsConsumed computes the IsConsumed calculated field
// TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads.
// Formula: =NOT(ISBLANK({{ConsumedBySteps}}))
func (tc *Dataset) CalcIsConsumed() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Dataset) ComputeAll() *Dataset {
	// Level 1 calculations
	relativePath := "datasets/" + stringVal(tc.DatasetId)
	isConsumed := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &Dataset{
		DatasetId: tc.DatasetId,
		Title: tc.Title,
		Identifier: tc.Identifier,
		Modified: tc.Modified,
		DistributionUrl: tc.DistributionUrl,
		ConsumedBySteps: tc.ConsumedBySteps,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		IsConsumed: &isConsumed,
	}
}

// =============================================================================
// WORKFLOWARTIFACTS TABLE
// Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
// =============================================================================

// WorkflowArtifact represents a row in the WorkflowArtifacts table
// Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
type WorkflowArtifact struct {
	ArtifactId string `json:"artifact_id"`
	ParentPath *string `json:"parent_path"` // Helper: the WorkflowSteps parent's RelativePath, pulled across the ProducedByStep FK. Exists so RelativePath can concatenate the '/artifacts/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat).
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
	ProducedByWorkflow *string `json:"produced_by_workflow"` // The workflow this artifact belongs to, resolved through ProducedByStep → WorkflowSteps.Workflow (artifact → producing step → workflow). Lets workflow-level rollups (e.g. CountDerivationLinks) aggregate artifacts without a redundant direct FK.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
	ProducingAgentType *string `json:"producing_agent_type"` // Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from.
	HasDerivationParent *bool `json:"has_derivation_parent"` // TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
	HasProducingWorkflow *bool `json:"has_producing_workflow"` // TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
// Formula: ={{ParentPath}} & "/artifacts/" & {{ArtifactId}}
func (tc *WorkflowArtifact) CalcRelativePath() string {
	return stringVal(tc.ParentPath) + "/artifacts/" + stringVal(tc.ArtifactId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *WorkflowArtifact) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcProducingAgentType computes the ProducingAgentType calculated field
// Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from.
// Formula: =IF(NOT(ISBLANK({{AttributedToHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{AttributedToAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{AttributedToAutomatedPipeline}})), "AutomatedPipeline", "")))
func (tc *WorkflowArtifact) CalcProducingAgentType() string {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcHasDerivationParent computes the HasDerivationParent calculated field
// TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
// Formula: =NOT(ISBLANK({{DerivedFromArtifact}}))
func (tc *WorkflowArtifact) CalcHasDerivationParent() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// CalcHasProducingWorkflow computes the HasProducingWorkflow calculated field
// TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
// Formula: =NOT(ISBLANK({{ProducedByWorkflow}}))
func (tc *WorkflowArtifact) CalcHasProducingWorkflow() bool {
	return func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *WorkflowArtifact) ComputeAll() *WorkflowArtifact {
	// Level 1 calculations
	relativePath := stringVal(tc.ParentPath) + "/artifacts/" + stringVal(tc.ArtifactId)
	producingAgentType := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	hasDerivationParent := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()
	hasProducingWorkflow := func() interface{} { panic("Formula parse error: Unknown function: ISBLANK") }()

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &WorkflowArtifact{
		ArtifactId: tc.ArtifactId,
		ParentPath: tc.ParentPath,
		Title: tc.Title,
		Identifier: tc.Identifier,
		ArtifactType: tc.ArtifactType,
		Created: tc.Created,
		ProducedByStep: tc.ProducedByStep,
		RequiredBySteps: tc.RequiredBySteps,
		DerivedFromArtifact: tc.DerivedFromArtifact,
		AttributedToHumanAgent: tc.AttributedToHumanAgent,
		AttributedToAIAgent: tc.AttributedToAIAgent,
		AttributedToAutomatedPipeline: tc.AttributedToAutomatedPipeline,
		ProducedByWorkflow: tc.ProducedByWorkflow,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		ProducingAgentType: nilIfEmpty(producingAgentType),
		HasDerivationParent: &hasDerivationParent,
		HasProducingWorkflow: &hasProducingWorkflow,
	}
}

// =============================================================================
// GOVERNANCEROLES TABLE
// Table: GovernanceRoles. NTWF governance names two distinct ontology-governance roles: a Steward (responsible for the ontology's health — monitors drift, tracks external dependency updates, fields user questions, maintains docs, keeps the validation suite current; identifies that a change is needed but has no approval power) and an Authority (the power to approve changes to the CBox, ABox, and TBox; decides how and where a change is made; sits with the function that owns the domain). 'A steward who can make TBox or ABox changes without authority review is a single point of failure.' For an organization under 500 people a single person may hold both roles. This table models the maintenance discipline itself, as data, so the change log can attribute approvals to a named authority.
// =============================================================================

// GovernanceRole represents a row in the GovernanceRoles table
// Table: GovernanceRoles. NTWF governance names two distinct ontology-governance roles: a Steward (responsible for the ontology's health — monitors drift, tracks external dependency updates, fields user questions, maintains docs, keeps the validation suite current; identifies that a change is needed but has no approval power) and an Authority (the power to approve changes to the CBox, ABox, and TBox; decides how and where a change is made; sits with the function that owns the domain). 'A steward who can make TBox or ABox changes without authority review is a single point of failure.' For an organization under 500 people a single person may hold both roles. This table models the maintenance discipline itself, as data, so the change log can attribute approvals to a named authority.
type GovernanceRole struct {
	GovernanceRoleId string `json:"governance_role_id"`
	DisplayName *string `json:"display_name"` // Human-readable name of the governance role (e.g. 'Steward', 'Authority').
	Kind *string `json:"kind"` // Which of the two NTWF governance kinds this is: 'Steward' or 'Authority'.
	Responsibilities *string `json:"responsibilities"` // What this role is responsible for. Steward: monitor drift, track external dependency updates, field user questions, maintain documentation, keep the validation suite current. Authority: approve changes to CBox/ABox/TBox; decide how and where a change is made.
	ApprovalScope *string `json:"approval_scope"` // The boxes this role may approve changes to (CBox/ABox/TBox), or 'none' for a Steward — who can identify that a change is needed but cannot approve it.
	HeldBy *string `json:"held_by"` // The person or function holding this role. The steward is naturally whoever owns the engineering knowledge infrastructure; authority sits with the workflow governance function that owns the modeled domain. Under 500 people, one person may hold both.
	ApprovedChanges *string `json:"approved_changes"` // Back-reference to ChangeLog entries this governance role approved. Inverse of ChangeLog.ApprovedBy.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
	Name *string `json:"name"` // Slug form of the display name.
	CanApproveChanges *bool `json:"can_approve_changes"` // TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key.
// Formula: ="governance-roles/" & {{GovernanceRoleId}}
func (tc *GovernanceRole) CalcRelativePath() string {
	return "governance-roles/" + stringVal(tc.GovernanceRoleId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *GovernanceRole) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Slug form of the display name.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *GovernanceRole) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// CalcCanApproveChanges computes the CanApproveChanges calculated field
// TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure.
// Formula: ={{Kind}} = "Authority"
func (tc *GovernanceRole) CalcCanApproveChanges() bool {
	return (stringVal(tc.Kind) == "Authority")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *GovernanceRole) ComputeAll() *GovernanceRole {
	// Level 1 calculations
	relativePath := "governance-roles/" + stringVal(tc.GovernanceRoleId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
	canApproveChanges := (stringVal(tc.Kind) == "Authority")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &GovernanceRole{
		GovernanceRoleId: tc.GovernanceRoleId,
		DisplayName: tc.DisplayName,
		Kind: tc.Kind,
		Responsibilities: tc.Responsibilities,
		ApprovalScope: tc.ApprovalScope,
		HeldBy: tc.HeldBy,
		ApprovedChanges: tc.ApprovedChanges,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		CanApproveChanges: &canApproveChanges,
	}
}

// =============================================================================
// CHANGELOG TABLE
// Table: ChangeLog. NTWF's minimum governance artifact: 'a change log that records every TBox and ABox modification, with its rationale.' Each entry records the four facts NTWF governance enumerates — the competency question that motivated the change, the terms affected, the version number of the release, and the date — plus the rationale and the Authority who approved it. Semantic-versioning discipline (MAJOR.MINOR.PATCH) is captured per entry via ChangeKind.
// =============================================================================

// ChangeLog represents a row in the ChangeLog table
// Table: ChangeLog. NTWF's minimum governance artifact: 'a change log that records every TBox and ABox modification, with its rationale.' Each entry records the four facts NTWF governance enumerates — the competency question that motivated the change, the terms affected, the version number of the release, and the date — plus the rationale and the Authority who approved it. Semantic-versioning discipline (MAJOR.MINOR.PATCH) is captured per entry via ChangeKind.
type ChangeLog struct {
	ChangeLogId string `json:"change_log_id"`
	Version *string `json:"version"` // The release version number this change shipped in (semantic versioning MAJOR.MINOR.PATCH). NTWF is currently at 1.1.0.
	ChangeDate *string `json:"change_date"` // The date of the change. One of the four facts NTWF governance requires every change-log entry to record.
	ChangeKind *string `json:"change_kind"` // Semantic-versioning class of the change: 'patch' (documentation/label/comment only, formal model unchanged), 'minor' (additive — new classes/properties/CBox concepts, backward compatible), or 'major' (breaking — class removed/renamed, domain/range change invalidating ABox triples, or a new disjointness axiom).
	MotivatingQuestion *string `json:"motivating_question"` // The competency question that motivated the change. One of the four facts NTWF governance requires. Empty if the change was driven by an external-dependency update rather than a CQ.
	TermsAffected *string `json:"terms_affected"` // The ontology terms (classes/properties/concepts) the change added, removed, or modified. One of the four facts NTWF governance requires.
	Rationale *string `json:"rationale"` // Why the change was made. NTWF governance requires every TBox/ABox modification to be logged with its rationale.
	ApprovedBy *string `json:"approved_by"` // FK to the GovernanceRole (an Authority) that approved this change. Changes to CBox/ABox/TBox require authority review; a steward identifying a need is not enough.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath).
	Name *string `json:"name"` // Human-readable label: the version and date of this change.
	IsBreakingChange *bool `json:"is_breaking_change"` // TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version.
	IsBackwardCompatible *bool `json:"is_backward_compatible"` // TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key.
// Formula: ="change-log/" & {{ChangeLogId}}
func (tc *ChangeLog) CalcRelativePath() string {
	return "change-log/" + stringVal(tc.ChangeLogId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ChangeLog) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Human-readable label: the version and date of this change.
// Formula: ={{Version}} & " (" & {{ChangeDate}} & ")"
func (tc *ChangeLog) CalcName() string {
	return stringVal(tc.Version) + " (" + stringVal(tc.ChangeDate) + ")"
}

// CalcIsBreakingChange computes the IsBreakingChange calculated field
// TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version.
// Formula: ={{ChangeKind}} = "major"
func (tc *ChangeLog) CalcIsBreakingChange() bool {
	return (stringVal(tc.ChangeKind) == "major")
}

// CalcIsBackwardCompatible computes the IsBackwardCompatible calculated field
// TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
// Formula: =OR({{ChangeKind}} = "patch", {{ChangeKind}} = "minor")
func (tc *ChangeLog) CalcIsBackwardCompatible() bool {
	return ((stringVal(tc.ChangeKind) == "patch") || (stringVal(tc.ChangeKind) == "minor"))
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *ChangeLog) ComputeAll() *ChangeLog {
	// Level 1 calculations
	relativePath := "change-log/" + stringVal(tc.ChangeLogId)
	name := stringVal(tc.Version) + " (" + stringVal(tc.ChangeDate) + ")"
	isBreakingChange := (stringVal(tc.ChangeKind) == "major")
	isBackwardCompatible := ((stringVal(tc.ChangeKind) == "patch") || (stringVal(tc.ChangeKind) == "minor"))

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &ChangeLog{
		ChangeLogId: tc.ChangeLogId,
		Version: tc.Version,
		ChangeDate: tc.ChangeDate,
		ChangeKind: tc.ChangeKind,
		MotivatingQuestion: tc.MotivatingQuestion,
		TermsAffected: tc.TermsAffected,
		Rationale: tc.Rationale,
		ApprovedBy: tc.ApprovedBy,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
		IsBreakingChange: &isBreakingChange,
		IsBackwardCompatible: &isBackwardCompatible,
	}
}

// =============================================================================
// VOCABULARYRECONCILIATIONS TABLE
// Table: VocabularyReconciliations. External dependency change: when a borrowed term from a living standard (PROV-O, FOAF, Dublin Core, DCAT, Schema.org) is deprecated and re-homed into the NTWF namespace, the edit triggers a version bump and an owl:sameAs reconciliation relation declaring the old and new terms equivalent. The worked example: deprecating foaf:name, prepending the ntwf prefix to get ntwf:name, and asserting foaf:name owl:sameAs ntwf:name. Each row is one reconciliation, with the standard it came from and the version in which the reconciliation shipped.
// =============================================================================

// VocabularyReconciliation represents a row in the VocabularyReconciliations table
// Table: VocabularyReconciliations. External dependency change: when a borrowed term from a living standard (PROV-O, FOAF, Dublin Core, DCAT, Schema.org) is deprecated and re-homed into the NTWF namespace, the edit triggers a version bump and an owl:sameAs reconciliation relation declaring the old and new terms equivalent. The worked example: deprecating foaf:name, prepending the ntwf prefix to get ntwf:name, and asserting foaf:name owl:sameAs ntwf:name. Each row is one reconciliation, with the standard it came from and the version in which the reconciliation shipped.
type VocabularyReconciliation struct {
	ReconciliationId string `json:"reconciliation_id"`
	DeprecatedTerm *string `json:"deprecated_term"` // The borrowed/deprecated term being reconciled (e.g. foaf:name).
	ReplacementTerm *string `json:"replacement_term"` // The NTWF-namespaced replacement term (e.g. ntwf:name).
	ReconciliationRelation *string `json:"reconciliation_relation"` // The OWL relation asserting equivalence. NTWF uses owl:sameAs.
	SourceStandard *string `json:"source_standard"` // The external standard the deprecated term came from (PROV-O, FOAF, Dublin Core, DCAT, Schema.org).
	IntroducedInVersion *string `json:"introduced_in_version"` // The NTWF release version in which this reconciliation shipped. Re-homing a term triggers a version bump.
	Rationale *string `json:"rationale"` // Why the term was re-homed (e.g. upstream deprecation; semantic-alignment shift).
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath).
	Name *string `json:"name"` // Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key.
// Formula: ="reconciliations/" & {{ReconciliationId}}
func (tc *VocabularyReconciliation) CalcRelativePath() string {
	return "reconciliations/" + stringVal(tc.ReconciliationId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *VocabularyReconciliation) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement.
// Formula: ={{DeprecatedTerm}} & " owl:sameAs " & {{ReplacementTerm}}
func (tc *VocabularyReconciliation) CalcName() string {
	return stringVal(tc.DeprecatedTerm) + " owl:sameAs " + stringVal(tc.ReplacementTerm)
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *VocabularyReconciliation) ComputeAll() *VocabularyReconciliation {
	// Level 1 calculations
	relativePath := "reconciliations/" + stringVal(tc.ReconciliationId)
	name := stringVal(tc.DeprecatedTerm) + " owl:sameAs " + stringVal(tc.ReplacementTerm)

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &VocabularyReconciliation{
		ReconciliationId: tc.ReconciliationId,
		DeprecatedTerm: tc.DeprecatedTerm,
		ReplacementTerm: tc.ReplacementTerm,
		ReconciliationRelation: tc.ReconciliationRelation,
		SourceStandard: tc.SourceStandard,
		IntroducedInVersion: tc.IntroducedInVersion,
		Rationale: tc.Rationale,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// SCENARIOS TABLE
// =============================================================================

// Scenario represents a row in the Scenarios table
type Scenario struct {
	ScenarioId string `json:"scenario_id"` // Stable identifier for a curated demo scenario (a named set of raw-fact edits applied at once).
	Label string `json:"label"` // Human-readable button label for this scenario in the picker.
	Icon *string `json:"icon"` // A single emoji shown beside the label in the picker.
	Explanation *string `json:"explanation"` // Plain-language description of what this scenario changes and what the reasoner will derive as a result. Shown in the floating scenario picker so the user knows what each preset does before applying it.
	SortOrder *int `json:"sort_order"` // Display order in the picker (ascending).
	IsReset *bool `json:"is_reset"` // True for the single 'restore baseline' scenario; the picker styles it as a secondary action.
	Edits string `json:"edits"` // JSON-encoded ordered list of raw-fact assignments this scenario applies. Each item is {class, id|match, set:{field:value,...}} where 'class' is a rulebook table (camelCase keys on the raw store), 'id' targets a row by its *Id primary key (or 'match':'first' for the singleton Workflow), and 'set' is the raw fields to assign. The backend replays this list against the active raw store, then re-reasons — the scenario NEVER sets a derived field. This is the single source of truth for what each demo scenario does; the app's picker and the apply endpoint both read it from here (same JSON-on-a-first-class-table pattern as __meta__'s JsonValue).
	RelativePath *string `json:"relative_path"` // DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (dash-form of RelativePath).
	Name *string `json:"name"` // Slug form of the human label.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key.
// Formula: ="scenarios/" & {{ScenarioId}}
func (tc *Scenario) CalcRelativePath() string {
	return "scenarios/" + stringVal(tc.ScenarioId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *Scenario) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Slug form of the human label.
// Formula: =SUBSTITUTE(LOWER({{Label}}), " ", "-")
func (tc *Scenario) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.Label)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *Scenario) ComputeAll() *Scenario {
	// Level 1 calculations
	relativePath := "scenarios/" + stringVal(tc.ScenarioId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.Label)), " ", "-")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &Scenario{
		ScenarioId: tc.ScenarioId,
		Label: tc.Label,
		Icon: tc.Icon,
		Explanation: tc.Explanation,
		SortOrder: tc.SortOrder,
		IsReset: tc.IsReset,
		Edits: tc.Edits,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// COMPETENCYQUESTIONS TABLE
// The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook.
// =============================================================================

// CompetencyQuestion represents a row in the CompetencyQuestions table
// The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook.
type CompetencyQuestion struct {
	CompetencyQuestionId string `json:"competency_question_id"` // Primary key. Stable slug for the competency question (cq-1 .. cq-8).
	Number string `json:"number"` // The canonical 1-8 ordering of the competency questions as listed in the article / README.
	DisplayName string `json:"display_name"` // Short human label for the question (e.g. 'Steps and order').
	QuestionText string `json:"question_text"` // The full competency question, verbatim from the article's acceptance suite.
	TargetTable string `json:"target_table"` // The entity whose computed field answers this question. Together with TargetField it pins the answer to a real column in the substrate, so the scoreboard reads the answer (never recomputes it) and the explainer-DAG drilldown lands on the exact derivation.
	TargetField string `json:"target_field"` // The substrate-computed field on TargetTable that answers this question (calc / lookup / aggregation / closure). The CQ scoreboard wraps the live answer in a DagCell(TargetTable, TargetField) so a click opens its inference graph.
	AnswerKind string `json:"answer_kind"` // 'scalar' when the answer is a single value graded by equality with ExpectedAnswer; 'list' when the answer is a collection graded as answerable (non-empty / matches the asserted shape).
	ExpectedAnswer string `json:"expected_answer"` // The asserted correct answer for the seed worked example. For scalar questions the live computed value must equal this to score a pass; for list questions this is the canonical summary the rendered collection is checked against. Authored here so pass/fail is (substrate-computed value) vs (rulebook-asserted expectation) — a real conformance check, not UI logic.
	SatisfiedField *string `json:"satisfied_field"` // Name of the boolean column on Workflows that computes whether this CQ is satisfied (e.g. Cq6Satisfied). The scoreboard reads pass/fail straight from this substrate-computed column — the acceptance criterion lives in the rulebook as a derived field, never as app-side logic. Mirrors TargetTable/TargetField for the answer.
	Explanation *string `json:"explanation"` // One-sentence note on how this question resolves through the model — the FK / formula chain a presenter can narrate.
	SortOrder *string `json:"sort_order"` // Display order in the scoreboard. Mirrors Number for now; kept separate so the list can be re-sequenced without renumbering the canonical CQ ids.
	IsActive *bool `json:"is_active"` // Whether this competency question is shown in the scoreboard. All eight are active in the worked example.
	SimulateScenario *string `json:"simulate_scenario"` // FK to the Scenario the card's 'Simulate' button applies to demonstrate this competency question live. Points at the minimal raw-fact edit that moves THIS question's answer in isolation where one exists; for cq-2 it points at 'ai-release-manager', which also ripples to cq-3 (the gate approver is itself a step executor, so the two answers cannot be perturbed independently). The full set of questions each scenario moves — trigger vs ripple — is enumerated in the ScenarioCQEffects junction; this is just the one the button fires. Inverse-ish of ScenarioCQEffects but kept as a direct FK so the UI has a single answer.
	RelativePath *string `json:"relative_path"` // Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution.
	Iri *string `json:"iri"` // Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
	Name *string `json:"name"` // Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution.
// Formula: ="competency-questions/" & {{CompetencyQuestionId}}
func (tc *CompetencyQuestion) CalcRelativePath() string {
	return "competency-questions/" + stringVal(tc.CompetencyQuestionId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *CompetencyQuestion) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *CompetencyQuestion) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *CompetencyQuestion) ComputeAll() *CompetencyQuestion {
	// Level 1 calculations
	relativePath := "competency-questions/" + stringVal(tc.CompetencyQuestionId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &CompetencyQuestion{
		CompetencyQuestionId: tc.CompetencyQuestionId,
		Number: tc.Number,
		DisplayName: tc.DisplayName,
		QuestionText: tc.QuestionText,
		TargetTable: tc.TargetTable,
		TargetField: tc.TargetField,
		AnswerKind: tc.AnswerKind,
		ExpectedAnswer: tc.ExpectedAnswer,
		SatisfiedField: tc.SatisfiedField,
		Explanation: tc.Explanation,
		SortOrder: tc.SortOrder,
		IsActive: tc.IsActive,
		SimulateScenario: tc.SimulateScenario,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// SCENARIOCQEFFECTS TABLE
// Table: ScenarioCQEffects. Names the many-to-many between Scenarios and CompetencyQuestions as two 1:M foreign keys (Scenario, CompetencyQuestion) plus the detail of the relationship (EffectKind, Note). Each row asserts 'applying this scenario moves this competency question's live answer'. 'trigger' rows are the intended demonstration; 'ripple' rows record answers that move as an unavoidable consequence of the same raw edit (e.g. ai-release-manager moves cq-2 AND cq-3 because the gate approver is itself a step executor). The answers themselves are never stored here — they are read live from each substrate after the scenario applies.
// =============================================================================

// ScenarioCQEffect represents a row in the ScenarioCQEffects table
// Table: ScenarioCQEffects. Names the many-to-many between Scenarios and CompetencyQuestions as two 1:M foreign keys (Scenario, CompetencyQuestion) plus the detail of the relationship (EffectKind, Note). Each row asserts 'applying this scenario moves this competency question's live answer'. 'trigger' rows are the intended demonstration; 'ripple' rows record answers that move as an unavoidable consequence of the same raw edit (e.g. ai-release-manager moves cq-2 AND cq-3 because the gate approver is itself a step executor). The answers themselves are never stored here — they are read live from each substrate after the scenario applies.
type ScenarioCQEffect struct {
	ScenarioCQEffectId string `json:"scenario_cq_effect_id"` // Primary key. '<scenario>-<cq>' — names one (scenario moves this competency question) edge.
	Scenario string `json:"scenario"` // FK to the Scenario whose raw-fact edits cause this effect. The 'many effects belong to one scenario' side: a single scenario can move several competency questions.
	CompetencyQuestion string `json:"competency_question"` // FK to the CompetencyQuestion whose live answer this scenario moves. The 'many effects belong to one question' side: a question can be exercised by several scenarios.
	EffectKind string `json:"effect_kind"` // 'trigger' = this scenario was authored to move this question (the point of the demo). 'ripple' = the question also moves as an unavoidable side effect of the same raw edit. The ripple rows are the pedagogical payload: they show answers that are structurally coupled and cannot be perturbed independently.
	Note *string `json:"note"` // One-line, human-readable account of how this scenario moves this question's answer (qualitative — the actual value is read live from the substrate, never stored here).
	SortOrder *int `json:"sort_order"` // Display order.
	RelativePath *string `json:"relative_path"` // DAG-derived location: 'scenario-cq-effects/' + the row's primary key.
	Iri *string `json:"iri"` // Opaque stable identifier (dash-form of RelativePath).
	Name *string `json:"name"` // Slug label, mirrors the primary key.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// DAG-derived location: 'scenario-cq-effects/' + the row's primary key.
// Formula: ="scenario-cq-effects/" & {{ScenarioCQEffectId}}
func (tc *ScenarioCQEffect) CalcRelativePath() string {
	return "scenario-cq-effects/" + stringVal(tc.ScenarioCQEffectId)
}

// CalcIri computes the Iri calculated field
// Opaque stable identifier (dash-form of RelativePath).
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ScenarioCQEffect) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Slug label, mirrors the primary key.
// Formula: =SUBSTITUTE(LOWER({{ScenarioCQEffectId}}), " ", "-")
func (tc *ScenarioCQEffect) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.ScenarioCQEffectId)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *ScenarioCQEffect) ComputeAll() *ScenarioCQEffect {
	// Level 1 calculations
	relativePath := "scenario-cq-effects/" + stringVal(tc.ScenarioCQEffectId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.ScenarioCQEffectId)), " ", "-")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &ScenarioCQEffect{
		ScenarioCQEffectId: tc.ScenarioCQEffectId,
		Scenario: tc.Scenario,
		CompetencyQuestion: tc.CompetencyQuestion,
		EffectKind: tc.EffectKind,
		Note: tc.Note,
		SortOrder: tc.SortOrder,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// CONFORMANCETESTS TABLE
// =============================================================================

// ConformanceTest represents a row in the ConformanceTests table
type ConformanceTest struct {
	ConformanceTestId string `json:"conformance_test_id"` // Stable identifier for one conformance test — one assertion the harness runs against every execution substrate.
	DisplayName string `json:"display_name"` // Human-readable test title shown in the admin console and run logs.
	FeatureRef *string `json:"feature_ref"` // Comma-separated FEATURE-COVERAGE.md ids this test witnesses (e.g. 'II-3,CQ2'). The traceability link from the article's feature inventory to an executable assertion.
	Section string `json:"section"` // Grouping for display: 'Sweep', 'Part I'..'Part IV', 'Closure', 'Mutation'.
	TestKind string `json:"test_kind"` // How the harness executes this test. 'sweep' = every row+field of TargetRef table vs the answer key; 'field-match' = one row's field vs the answer key; 'closure-contains' = a from→to pair must appear in the engine's computed transitive closure (Expect names the closure and pair); 'engines-agree' = zero value-class disagreements between the two engines; 'mutation' = apply Expect.edits to an in-memory copy of the seed facts, re-reason, and check Expect.assert — the store is NEVER written.
	TargetRef *string `json:"target_ref"` // What the test reads: 'Entity' (whole table), 'Entity/pk' (one row) or 'Entity/pk#Field' (one value). Slash/hash form on purpose — these are spec references, not foreign keys.
	Expect *string `json:"expect"` // Kind-specific JSON spec. Empty for sweep/field-match/engines-agree — there the ORACLE is the answer key (testing/answer-keys), never a value hardcoded here (a literal would go stale; the key regenerates). closure-contains: {closure, from, to}. mutation: {edits:[{class,id,set:{camelRawField:value}}], assert:[{class,id,field,equals}]} — same edits shape as Scenarios.Edits.
	Explanation *string `json:"explanation"` // Why this test exists — what feature of the model it flexes, in one sentence.
	SortOrder int `json:"sort_order"` // Display/run order within the suite.
	IsEnabled bool `json:"is_enabled"` // Disabled tests are listed but not executed (parked, not deleted — the list stays the complete spec).
	RelativePath *string `json:"relative_path"` // DAG-derived location for this test row: root segment 'conformance-tests' + the primary key.
	Iri *string `json:"iri"` // Slug IRI for this row, derived from RelativePath.
	Name *string `json:"name"` // Machine name derived from the display name.
}

// --- Individual Calculation Functions ---

// CalcRelativePath computes the RelativePath calculated field
// DAG-derived location for this test row: root segment 'conformance-tests' + the primary key.
// Formula: ="conformance-tests/" & {{ConformanceTestId}}
func (tc *ConformanceTest) CalcRelativePath() string {
	return "conformance-tests/" + stringVal(tc.ConformanceTestId)
}

// CalcIri computes the Iri calculated field
// Slug IRI for this row, derived from RelativePath.
// Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
func (tc *ConformanceTest) CalcIri() string {
	return strings.ReplaceAll(stringVal(tc.RelativePath), "/", "-")
}

// CalcName computes the Name calculated field
// Machine name derived from the display name.
// Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
func (tc *ConformanceTest) CalcName() string {
	return strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *ConformanceTest) ComputeAll() *ConformanceTest {
	// Level 1 calculations
	relativePath := "conformance-tests/" + stringVal(tc.ConformanceTestId)
	name := strings.ReplaceAll(strings.ToLower(stringVal(tc.DisplayName)), " ", "-")

	// Level 2 calculations
	iri := strings.ReplaceAll(relativePath, "/", "-")

	return &ConformanceTest{
		ConformanceTestId: tc.ConformanceTestId,
		DisplayName: tc.DisplayName,
		FeatureRef: tc.FeatureRef,
		Section: tc.Section,
		TestKind: tc.TestKind,
		TargetRef: tc.TargetRef,
		Expect: tc.Expect,
		Explanation: tc.Explanation,
		SortOrder: tc.SortOrder,
		IsEnabled: tc.IsEnabled,
		RelativePath: nilIfEmpty(relativePath),
		Iri: nilIfEmpty(iri),
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// __META__ TABLE
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
// =============================================================================

// __meta__ represents a row in the __meta__ table
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
type __meta__ struct {
	MetaKey string `json:"meta_key"` // The metadata key (e.g. 'tagline', 'motif_palette', 'substrates'). Unique within the table.
	ValueType string `json:"value_type"` // How to interpret the value columns: 'string' (use StringValue), 'object' (parse JsonValue as JSON object), 'array' (parse JsonValue as JSON array).
	StringValue *string `json:"string_value"` // Plain string value. Populated when ValueType == 'string'; null otherwise.
	JsonValue *string `json:"json_value"` // JSON-encoded value. Populated when ValueType == 'object' or 'array'; null when ValueType == 'string'.
	Name string `json:"name"` // Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
}

// --- Individual Calculation Functions ---

// CalcName computes the Name calculated field
// Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
// Formula: ={{MetaKey}}
func (tc *__meta__) CalcName() string {
	return tc.MetaKey
}

// --- Compute All Calculated Fields ---

// ComputeAll computes all calculated fields and returns an updated struct
func (tc *__meta__) ComputeAll() *__meta__ {
	// Level 1 calculations
	name := tc.MetaKey

	return &__meta__{
		MetaKey: tc.MetaKey,
		ValueType: tc.ValueType,
		StringValue: tc.StringValue,
		JsonValue: tc.JsonValue,
		Name: nilIfEmpty(name),
	}
}

// =============================================================================
// FILE I/O FUNCTIONS
// Load: all tables referenced by main.go (computed + lookup/aggregation targets)
// Save: only tables that have computed fields to write back
// =============================================================================

// LoadWorkflowRecords loads Workflows records from a JSON file
func LoadWorkflowRecords(path string) ([]Workflow, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Workflow
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveWorkflowRecords saves computed Workflows records to a JSON file
func SaveWorkflowRecords(path string, records []Workflow) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadWorkflowStepRecords loads WorkflowSteps records from a JSON file
func LoadWorkflowStepRecords(path string) ([]WorkflowStep, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []WorkflowStep
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveWorkflowStepRecords saves computed WorkflowSteps records to a JSON file
func SaveWorkflowStepRecords(path string, records []WorkflowStep) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadApprovalGateRecords loads ApprovalGates records from a JSON file
func LoadApprovalGateRecords(path string) ([]ApprovalGate, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []ApprovalGate
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveApprovalGateRecords saves computed ApprovalGates records to a JSON file
func SaveApprovalGateRecords(path string, records []ApprovalGate) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadStepPrecedenceRecords loads StepPrecedence records from a JSON file
func LoadStepPrecedenceRecords(path string) ([]StepPrecedence, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []StepPrecedence
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveStepPrecedenceRecords saves computed StepPrecedence records to a JSON file
func SaveStepPrecedenceRecords(path string, records []StepPrecedence) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadRoleRecords loads Roles records from a JSON file
func LoadRoleRecords(path string) ([]Role, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Role
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveRoleRecords saves computed Roles records to a JSON file
func SaveRoleRecords(path string, records []Role) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadRoleAssignmentRecords loads RoleAssignments records from a JSON file
func LoadRoleAssignmentRecords(path string) ([]RoleAssignment, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []RoleAssignment
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveRoleAssignmentRecords saves computed RoleAssignments records to a JSON file
func SaveRoleAssignmentRecords(path string, records []RoleAssignment) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadDepartmentRecords loads Departments records from a JSON file
func LoadDepartmentRecords(path string) ([]Department, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Department
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveDepartmentRecords saves computed Departments records to a JSON file
func SaveDepartmentRecords(path string, records []Department) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadHumanAgentRecords loads HumanAgents records from a JSON file
func LoadHumanAgentRecords(path string) ([]HumanAgent, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []HumanAgent
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveHumanAgentRecords saves computed HumanAgents records to a JSON file
func SaveHumanAgentRecords(path string, records []HumanAgent) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadAIAgentRecords loads AIAgents records from a JSON file
func LoadAIAgentRecords(path string) ([]AIAgent, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []AIAgent
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveAIAgentRecords saves computed AIAgents records to a JSON file
func SaveAIAgentRecords(path string, records []AIAgent) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadAutomatedPipelineRecords loads AutomatedPipelines records from a JSON file
func LoadAutomatedPipelineRecords(path string) ([]AutomatedPipeline, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []AutomatedPipeline
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveAutomatedPipelineRecords saves computed AutomatedPipelines records to a JSON file
func SaveAutomatedPipelineRecords(path string, records []AutomatedPipeline) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadWorkflowStatusConceptRecords loads WorkflowStatusConcepts records from a JSON file
func LoadWorkflowStatusConceptRecords(path string) ([]WorkflowStatusConcept, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []WorkflowStatusConcept
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveWorkflowStatusConceptRecords saves computed WorkflowStatusConcepts records to a JSON file
func SaveWorkflowStatusConceptRecords(path string, records []WorkflowStatusConcept) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadAgentCapabilityConceptRecords loads AgentCapabilityConcepts records from a JSON file
func LoadAgentCapabilityConceptRecords(path string) ([]AgentCapabilityConcept, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []AgentCapabilityConcept
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveAgentCapabilityConceptRecords saves computed AgentCapabilityConcepts records to a JSON file
func SaveAgentCapabilityConceptRecords(path string, records []AgentCapabilityConcept) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadArtifactTypeConceptRecords loads ArtifactTypeConcepts records from a JSON file
func LoadArtifactTypeConceptRecords(path string) ([]ArtifactTypeConcept, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []ArtifactTypeConcept
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveArtifactTypeConceptRecords saves computed ArtifactTypeConcepts records to a JSON file
func SaveArtifactTypeConceptRecords(path string, records []ArtifactTypeConcept) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadDatasetRecords loads Datasets records from a JSON file
func LoadDatasetRecords(path string) ([]Dataset, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Dataset
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveDatasetRecords saves computed Datasets records to a JSON file
func SaveDatasetRecords(path string, records []Dataset) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadWorkflowArtifactRecords loads WorkflowArtifacts records from a JSON file
func LoadWorkflowArtifactRecords(path string) ([]WorkflowArtifact, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []WorkflowArtifact
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveWorkflowArtifactRecords saves computed WorkflowArtifacts records to a JSON file
func SaveWorkflowArtifactRecords(path string, records []WorkflowArtifact) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadGovernanceRoleRecords loads GovernanceRoles records from a JSON file
func LoadGovernanceRoleRecords(path string) ([]GovernanceRole, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []GovernanceRole
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveGovernanceRoleRecords saves computed GovernanceRoles records to a JSON file
func SaveGovernanceRoleRecords(path string, records []GovernanceRole) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadChangeLogRecords loads ChangeLog records from a JSON file
func LoadChangeLogRecords(path string) ([]ChangeLog, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []ChangeLog
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveChangeLogRecords saves computed ChangeLog records to a JSON file
func SaveChangeLogRecords(path string, records []ChangeLog) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadVocabularyReconciliationRecords loads VocabularyReconciliations records from a JSON file
func LoadVocabularyReconciliationRecords(path string) ([]VocabularyReconciliation, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []VocabularyReconciliation
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveVocabularyReconciliationRecords saves computed VocabularyReconciliations records to a JSON file
func SaveVocabularyReconciliationRecords(path string, records []VocabularyReconciliation) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadScenarioRecords loads Scenarios records from a JSON file
func LoadScenarioRecords(path string) ([]Scenario, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []Scenario
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveScenarioRecords saves computed Scenarios records to a JSON file
func SaveScenarioRecords(path string, records []Scenario) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadCompetencyQuestionRecords loads CompetencyQuestions records from a JSON file
func LoadCompetencyQuestionRecords(path string) ([]CompetencyQuestion, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []CompetencyQuestion
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveCompetencyQuestionRecords saves computed CompetencyQuestions records to a JSON file
func SaveCompetencyQuestionRecords(path string, records []CompetencyQuestion) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadScenarioCQEffectRecords loads ScenarioCQEffects records from a JSON file
func LoadScenarioCQEffectRecords(path string) ([]ScenarioCQEffect, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []ScenarioCQEffect
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveScenarioCQEffectRecords saves computed ScenarioCQEffects records to a JSON file
func SaveScenarioCQEffectRecords(path string, records []ScenarioCQEffect) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// LoadConformanceTestRecords loads ConformanceTests records from a JSON file
func LoadConformanceTestRecords(path string) ([]ConformanceTest, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []ConformanceTest
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// SaveConformanceTestRecords saves computed ConformanceTests records to a JSON file
func SaveConformanceTestRecords(path string, records []ConformanceTest) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}

// Load__meta__Records loads __meta__ records from a JSON file
func Load__meta__Records(path string) ([]__meta__, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("failed to read file: %w", err)
	}

	var records []__meta__
	if err := json.Unmarshal(data, &records); err != nil {
		return nil, fmt.Errorf("failed to parse file: %w", err)
	}

	return records, nil
}

// Save__meta__Records saves computed __meta__ records to a JSON file
func Save__meta__Records(path string, records []__meta__) error {
	data, err := json.MarshalIndent(records, "", "  ")
	if err != nil {
		return fmt.Errorf("failed to marshal records: %w", err)
	}

	if err := os.WriteFile(path, data, 0644); err != nil {
		return fmt.Errorf("failed to write records: %w", err)
	}

	return nil
}
