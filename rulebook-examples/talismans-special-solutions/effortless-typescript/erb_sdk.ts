// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/talisman-s-special-solutions-rulebook.json
//
// One interface per table, a calc<Table><Field>() function per calculated field,
// and the table registry main.ts runs. Formulas compute through erb_runtime.ts;
// nothing here parses a formula, reads the rulebook, or calls a database.

/* eslint-disable */
import {
  Null,
  Value,
  vB,
  vS,
  vI,
  vF,
  vStr,
  vStrPlain,
  vBool,
  vBoolPlain,
  vInt,
  vIntPlain,
  vNum,
  vNumPlain,
  vAny,
  toStringPtr,
  strPlain,
  toBoolPtr,
  boolPlain,
  toIntPtr,
  intPlain,
  toFloatPtr,
  floatPlain,
  anyPlain,
  erbTextOr,
  erbTextNotNull,
  erbTimestamptzText,
  erbConcat,
  erbNeg,
  erbAdd,
  erbSub,
  erbMul,
  erbDiv,
  erbInteger,
  erbRound,
  erbRoundup,
  erbAbs,
  erbPower,
  erbSqrt,
  erbTan,
  erbLog,
  erbLog10,
  erbMaxMin,
  erbSum,
  erbPi,
  erbBool3,
  erbIsTrue,
  erbHasValue,
  erbAnd,
  erbOr,
  erbNot,
  erbIf,
  erbIsBlank,
  erbIsNotBlank,
  erbNullif,
  erbEq,
  erbNe,
  erbCmp,
  erbCoalesce,
  erbTry,
  erbIsError,
  erbLower,
  erbUpper,
  erbTrim,
  erbLen,
  erbLeft,
  erbRight,
  erbMid,
  erbSubstitute,
  erbFind,
  erbCast,
  erbDatetimeDiff,
  erbNow,
  calcGuard,
  loadRows,
} from "./erb_runtime.js";
import type { ClosureSpec, FieldType, TableSpec } from "./erb_runtime.js";

// =============================================================================
// WORKFLOWS TABLE
// Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
// =============================================================================

/** A row in the Workflows table. */
export interface WorkflowsRow {
  workflow_id: string;
  /** Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation. */
  name: string | null;
  display_name: string | null;
  /** Human-readable title of the workflow. Maps to dct:title from Dublin Core. Example: 'Production Deployment Workflow'. */
  title: string | null;
  /** Detailed description of the workflow's purpose and scope. Maps to dct:description from Dublin Core. Should explain what business goal the workflow achieves. */
  description: string | null;
  /** External system identifier for cross-referencing. Maps to dct:identifier from Dublin Core. This is the join key back to document management systems, ticket systems, or other operational systems. */
  identifier: string | null;
  /** Last modification timestamp. Maps to dct:modified from Dublin Core. Critical for answering CQ5: 'Which workflows haven't been reviewed or updated in twelve months?' */
  modified: string | null;
  /** Creation timestamp. Maps to dct:created from Dublin Core. Records when the workflow was first defined. */
  created: string | null;
  /** The governance POLICY (in months): the full review cadence after which this workflow's compliance documentation is formally out of date. The docs go stale exactly when this review age is exceeded — IsStale fires the instant MonthsSinceModified passes this policy line, with no deferral. The article hardcodes the CQ5 question at twelve months ('which workflows haven't been reviewed in twelve months'); promoting that threshold to a raw, editable field makes the policy itself a fact in the SSoT rather than a constant buried in the IsStale formula — so an org can set a 6-month or 18-month review cadence and the staleness verdict recomputes. Defaults to 12 to match the article. */
  staleness_threshold_months: number | null;
  /** FK to WorkflowStatusConcepts. Captures the current lifecycle state of the workflow (draft, active, deprecated, archived). Maps to the SKOS CBox status vocabulary. */
  workflow_status: string | null;
  /** Reference to workflow steps. Represents the ntwf:hasStep relationship linking workflows to their constituent steps. */
  workflow_steps: string | null;
  /** Calculated count of workflow steps in this workflow. Useful for workflow complexity analysis and reporting. */
  count_of_non_proposed_steps: number | null;
  has_more_than1_step: boolean | null;
  /** Number of steps in this workflow executed by an AIAgent (rollup over WorkflowSteps.IsExecutedByAI). This is the 'AI-executed' half of the article's CQ3 ('which steps are executed by AI agents') — counted against AIAgent individuals specifically, not the deterministic AutomatedPipeline, which is a disjoint agent type. Also drives the business-payoff query (a workflow is a compliance risk when an AI agent runs a step). Worked example: 2 (the AI risk-assessment step and the AI post-deployment health report). */
  count_ai_steps: number | null;
  /** Number of steps executed by a HumanAgent (rollup over WorkflowSteps.IsExecutedByHuman). The 'who actually runs this step' count — distinct from CountHumanRequiredSteps, which counts steps that demand a human decision (requiresHumanApproval). Worked example: 2 (the legal-review step and the release approval gate). */
  count_human_steps: number | null;
  /** Number of steps that require a human decision (rollup over WorkflowSteps.RequiresHumanApproval). This is the 'human-required' half of the article's CQ3 ('which require a human decision'), answered — as the article notes — by a single FILTER on requiresHumanApproval. Worked example: 2 (the legal-review step and the release approval gate). */
  count_human_required_steps: number | null;
  /** Number of steps that require human approval but are not human-filled (rollup over WorkflowSteps.ApprovalConsistencyViolation). The clean ABox witness: this is 0 for the Production Deployment workflow. A non-zero value is the relational signal of a Suite-4 consistency violation. */
  count_approval_consistency_violations: number | null;
  /** TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT. */
  has_consistency_violation: boolean | null;
  /** TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query. */
  has_ai_agent_step: boolean | null;
  /** Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable. */
  months_since_modified: number | null;
  /** TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field. */
  is_stale: boolean | null;
  /** The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column. */
  is_stale_and_has_ai_agent: boolean | null;
  /** Number of prov:wasDerivedFrom links among this workflow's artifacts (rollup over WorkflowArtifacts.HasDerivationParent). Answers the lineage half of CQ4: 5 artifacts form a chain with 4 derivation links. */
  count_derivation_links: number | null;
  /** Number of steps in this workflow whose owning department is Legal (rollup over WorkflowSteps.IsLegalOwned). CQ7: exactly one Legal-owned step in the Production Deployment workflow. */
  count_legal_owned_steps: number | null;
  /** Number of steps whose owning department is Engineering (rollup over WorkflowSteps.IsEngineeringOwned). Feeds CQ7's Engineering-involvement check. */
  count_engineering_owned_steps: number | null;
  /** TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step). */
  involves_engineering_and_legal: boolean | null;
  /** Number of step-ordering pairs that the transitive closure of ntwf:precedesStep INFERRED (rollup over the closure view vw_step_precedence_closure where is_inferred = TRUE). The article's signature count: 6 of the 10 closure pairs were never asserted — including step-1 -> step-5. NOTE: this single-workflow model has exactly one Workflow, so the global closure view is wholly this workflow's; the COUNTIFS is unfiltered because every precedence edge belongs to the Production Deployment DAG. */
  count_inferred_precedence_pairs: number | null;
  /** Number of step-ordering pairs that were directly ASSERTED as ntwf:precedesStep edges (rollup over vw_step_precedence_closure where is_inferred = FALSE) — the hop-1 rows. The article's 4 asserted edges. Together with CountInferredPrecedencePairs (6) this sums to the 10-pair closure, making CountOfPrecedenceClosurePairs an honest asserted+inferred total rather than an unconditional count. Single-workflow note as on CountInferredPrecedencePairs: the global closure view is this workflow's. */
  count_asserted_precedence_pairs: number | null;
  /** Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them. */
  count_of_precedence_closure_pairs: number | null;
  /** Number of roles that do NOT have exactly one filledBy arm set (rollup over Roles.HasExactlyOneFiller = FALSE). The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has 0 such roles — this is the Suite-1 functional/disjointness witness as a single integer. A non-zero value is the relational signal of the Suite-4 disjointness violation (a role filled by two agent classes, or by none). NOTE: this single-workflow model has exactly one Workflow and every Role participates in it, so the count is over all roles; a multi-workflow model would scope it through a role→workflow path. */
  count_roles_with_bad_filler_cardinality: number | null;
  /** Number of filledBy assignment periods that changed the agent CLASS of a role (rollup over RoleAssignments.IsAgentTypeChange = TRUE). NTWF governance distinguishes a same-class personnel/model swap from an agent-type transition; this counts the latter. NOTE: single-workflow model — every Role participates in the one workflow, so the count is over all assignment history; a multi-workflow model would scope it through a role→workflow path. */
  count_agent_type_changes: number | null;
  /** Number of filledBy assignment periods that took a previously AI-executed binding and reassigned it to a human (rollup over RoleAssignments.RequiresComplianceAudit = TRUE). NTWF governance treats this as a data operation with compliance implications: changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent. Each such row must carry when (ValidFrom) and why (Reason). NOTE: single-workflow scoping as above. */
  count_compliance_audit_changes: number | null;
  /** Number of this workflow's steps that are approval gates. >0 means the workflow has a blocking approval checkpoint; used by Cq2Satisfied to require that the gate exists before asking whether it has a human approver. */
  count_approval_gate_steps: number | null;
  /** Number of approval gates with no resolved human approver (gate role not filled by a HumanAgent). Single-workflow model, so this global count is wholly this workflow's. Drives Cq2Satisfied (= a gate exists AND none lack a human approver). */
  count_gates_without_human_approver: number | null;
  /** Total artifacts produced by this workflow. With CountDerivationLinks (artifacts that have a wasDerivedFrom parent) this lets Cq4Satisfied check the provenance chain is intact: every artifact but the single origin has a parent. */
  count_workflow_artifacts: number | null;
  /** Number of roles that own an approval gate yet escalate to no one (Roles.EscalationViolation). Single-workflow model, so this global count applies to this workflow. Drives Cq6Satisfied (=0): the model's own native escalation-completeness invariant, replacing any hardcoded 'must reach the CTO' check. */
  count_roles_with_escalation_violation: number | null;
  /** Number of datasets not consumed by any step (Datasets.IsConsumed = FALSE). Single-workflow model, so this global count applies to this workflow. Drives Cq8Satisfied (=0). */
  count_unconsumed_datasets: number | null;
  /** CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal. */
  cq1_satisfied: boolean | null;
  /** CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name. */
  cq2_satisfied: boolean | null;
  /** CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant. */
  cq3_satisfied: boolean | null;
  /** CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut. */
  cq4_satisfied: boolean | null;
  /** CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths. */
  cq5_satisfied: boolean | null;
  /** CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name. */
  cq6_satisfied: boolean | null;
  /** CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean. */
  cq7_satisfied: boolean | null;
  /** CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached. */
  cq8_satisfied: boolean | null;
  _erb_errors?: Record<string, string>;
}

const workflowsFieldTypes: Record<string, FieldType> = {
  workflow_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "*string",
  title: "*string",
  description: "*string",
  identifier: "*string",
  modified: "*string",
  created: "*string",
  staleness_threshold_months: "*int",
  workflow_status: "*string",
  workflow_steps: "*string",
  count_of_non_proposed_steps: "*int",
  has_more_than1_step: "*bool",
  count_ai_steps: "*int",
  count_human_steps: "*int",
  count_human_required_steps: "*int",
  count_approval_consistency_violations: "*int",
  has_consistency_violation: "*bool",
  has_ai_agent_step: "*bool",
  months_since_modified: "*int",
  is_stale: "*bool",
  is_stale_and_has_ai_agent: "*bool",
  count_derivation_links: "*int",
  count_legal_owned_steps: "*int",
  count_engineering_owned_steps: "*int",
  involves_engineering_and_legal: "*bool",
  count_inferred_precedence_pairs: "*int",
  count_asserted_precedence_pairs: "*int",
  count_of_precedence_closure_pairs: "*int",
  count_roles_with_bad_filler_cardinality: "*int",
  count_agent_type_changes: "*int",
  count_compliance_audit_changes: "*int",
  count_approval_gate_steps: "*int",
  count_gates_without_human_approver: "*int",
  count_workflow_artifacts: "*int",
  count_roles_with_escalation_violation: "*int",
  count_unconsumed_datasets: "*int",
  cq1_satisfied: "*bool",
  cq2_satisfied: "*bool",
  cq3_satisfied: "*bool",
  cq4_satisfied: "*bool",
  cq5_satisfied: "*bool",
  cq6_satisfied: "*bool",
  cq7_satisfied: "*bool",
  cq8_satisfied: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="workflows/" & {{WorkflowId}} */
export function calcWorkflowsRelativePath(tc: WorkflowsRow): string | null {
  return toStringPtr(erbConcat(vS("workflows/"), erbTextOr(vStrPlain(tc.workflow_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcWorkflowsIri(tc: WorkflowsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcWorkflowsName(tc: WorkflowsRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStr(tc.display_name)), vS(" "), vS("-")));
}

/** Computes the HasMoreThan1Step calculated field.
 *  Formula: ={{CountOfNonProposedSteps}} > 1 */
export function calcWorkflowsHasMoreThan1Step(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_non_proposed_steps), ">", vI(1)));
}

/** Computes the HasConsistencyViolation calculated field.
 *  TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
 *  Formula: ={{CountApprovalConsistencyViolations}} > 0 */
export function calcWorkflowsHasConsistencyViolation(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_approval_consistency_violations), ">", vI(0)));
}

/** Computes the HasAIAgentStep calculated field.
 *  TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
 *  Formula: ={{CountAISteps}} > 0 */
export function calcWorkflowsHasAIAgentStep(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_ai_steps), ">", vI(0)));
}

/** Computes the MonthsSinceModified calculated field.
 *  Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
 *  Formula: =DATETIME_DIFF(NOW(), {{Modified}}, "months") */
export function calcWorkflowsMonthsSinceModified(tc: WorkflowsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(erbNow(), vStr(tc.modified), vS("months"))));
}

/** Computes the IsStale calculated field.
 *  TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field.
 *  Formula: ={{MonthsSinceModified}} > {{StalenessThresholdMonths}} */
export function calcWorkflowsIsStale(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.months_since_modified), ">", erbNullif(vInt(tc.staleness_threshold_months))));
}

/** Computes the IsStaleAndHasAIAgent calculated field.
 *  The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
 *  Formula: =AND({{IsStale}}, {{HasAIAgentStep}}) */
export function calcWorkflowsIsStaleAndHasAIAgent(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_stale)), erbBool3(vBool(tc.has_ai_agent_step))));
}

/** Computes the InvolvesEngineeringAndLegal calculated field.
 *  TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
 *  Formula: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0) */
export function calcWorkflowsInvolvesEngineeringAndLegal(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.count_engineering_owned_steps), ">", vI(0))), erbBool3(erbCmp(vInt(tc.count_legal_owned_steps), ">", vI(0)))));
}

/** Computes the CountOfPrecedenceClosurePairs calculated field.
 *  Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
 *  Formula: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}} */
export function calcWorkflowsCountOfPrecedenceClosurePairs(tc: WorkflowsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.count_asserted_precedence_pairs), vInt(tc.count_inferred_precedence_pairs))));
}

/** Computes the Cq1Satisfied calculated field.
 *  CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
 *  Formula: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2 */
export function calcWorkflowsCq1Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.count_of_precedence_closure_pairs), erbDiv(erbMul(vInt(tc.count_of_non_proposed_steps), erbSub(vInt(tc.count_of_non_proposed_steps), vI(1))), vI(2))));
}

/** Computes the Cq2Satisfied calculated field.
 *  CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
 *  Formula: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0) */
export function calcWorkflowsCq2Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.count_approval_gate_steps), ">", vI(0))), erbBool3(erbEq(vInt(tc.count_gates_without_human_approver), vI(0)))));
}

/** Computes the Cq3Satisfied calculated field.
 *  CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
 *  Formula: =NOT({{HasConsistencyViolation}}) */
export function calcWorkflowsCq3Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(vBool(tc.has_consistency_violation))));
}

/** Computes the Cq4Satisfied calculated field.
 *  CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
 *  Formula: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1 */
export function calcWorkflowsCq4Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.count_derivation_links), erbSub(vInt(tc.count_workflow_artifacts), vI(1))));
}

/** Computes the Cq5Satisfied calculated field.
 *  CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
 *  Formula: =NOT({{IsStale}}) */
export function calcWorkflowsCq5Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(vBool(tc.is_stale))));
}

/** Computes the Cq6Satisfied calculated field.
 *  CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
 *  Formula: ={{CountRolesWithEscalationViolation}} = 0 */
export function calcWorkflowsCq6Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.count_roles_with_escalation_violation), vI(0)));
}

/** Computes the Cq7Satisfied calculated field.
 *  CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
 *  Formula: ={{InvolvesEngineeringAndLegal}} */
export function calcWorkflowsCq7Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(vBool(tc.involves_engineering_and_legal));
}

/** Computes the Cq8Satisfied calculated field.
 *  CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
 *  Formula: ={{CountUnconsumedDatasets}} = 0 */
export function calcWorkflowsCq8Satisfied(tc: WorkflowsRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.count_unconsumed_datasets), vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeWorkflows(tc: WorkflowsRow): WorkflowsRow {
  // Level 1
  calcGuard(tc, workflowsFieldTypes, "relative_path", () => { tc.relative_path = calcWorkflowsRelativePath(tc); });
  calcGuard(tc, workflowsFieldTypes, "name", () => { tc.name = calcWorkflowsName(tc); });
  calcGuard(tc, workflowsFieldTypes, "has_more_than1_step", () => { tc.has_more_than1_step = calcWorkflowsHasMoreThan1Step(tc); });
  calcGuard(tc, workflowsFieldTypes, "has_consistency_violation", () => { tc.has_consistency_violation = calcWorkflowsHasConsistencyViolation(tc); });
  calcGuard(tc, workflowsFieldTypes, "has_ai_agent_step", () => { tc.has_ai_agent_step = calcWorkflowsHasAIAgentStep(tc); });
  calcGuard(tc, workflowsFieldTypes, "months_since_modified", () => { tc.months_since_modified = calcWorkflowsMonthsSinceModified(tc); });
  calcGuard(tc, workflowsFieldTypes, "involves_engineering_and_legal", () => { tc.involves_engineering_and_legal = calcWorkflowsInvolvesEngineeringAndLegal(tc); });
  calcGuard(tc, workflowsFieldTypes, "count_of_precedence_closure_pairs", () => { tc.count_of_precedence_closure_pairs = calcWorkflowsCountOfPrecedenceClosurePairs(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq2_satisfied", () => { tc.cq2_satisfied = calcWorkflowsCq2Satisfied(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq4_satisfied", () => { tc.cq4_satisfied = calcWorkflowsCq4Satisfied(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq6_satisfied", () => { tc.cq6_satisfied = calcWorkflowsCq6Satisfied(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq8_satisfied", () => { tc.cq8_satisfied = calcWorkflowsCq8Satisfied(tc); });
  // Level 2
  calcGuard(tc, workflowsFieldTypes, "iri", () => { tc.iri = calcWorkflowsIri(tc); });
  calcGuard(tc, workflowsFieldTypes, "is_stale", () => { tc.is_stale = calcWorkflowsIsStale(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq1_satisfied", () => { tc.cq1_satisfied = calcWorkflowsCq1Satisfied(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq3_satisfied", () => { tc.cq3_satisfied = calcWorkflowsCq3Satisfied(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq7_satisfied", () => { tc.cq7_satisfied = calcWorkflowsCq7Satisfied(tc); });
  // Level 3
  calcGuard(tc, workflowsFieldTypes, "is_stale_and_has_ai_agent", () => { tc.is_stale_and_has_ai_agent = calcWorkflowsIsStaleAndHasAIAgent(tc); });
  calcGuard(tc, workflowsFieldTypes, "cq5_satisfied", () => { tc.cq5_satisfied = calcWorkflowsCq5Satisfied(tc); });
  return tc;
}

/** Reads Workflows rows from a JSON array file. */
export function loadWorkflowsRows(file: string): WorkflowsRow[] {
  return loadRows(file, { fields: workflowsFieldTypes }) as unknown as WorkflowsRow[];
}

// =============================================================================
// WORKFLOWSTEPS TABLE
// Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
// =============================================================================

/** A row in the WorkflowSteps table. */
export interface WorkflowStepsRow {
  workflow_step_id: string;
  /** Helper: the Workflows parent's RelativePath, pulled across the Workflow FK. Exists so RelativePath can concatenate the '/steps/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat). */
  parent_path: string | null;
  /** Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  name: string | null;
  display_name: string | null;
  /** Forward foreign key to the parent workflow (ntwf:isStepOf) — the authoritative stored link from step to its containing workflow; every per-workflow rollup (CountOfNonProposedSteps, the department-owned counts, etc.) reads it. This IS the stored column, not a derived inverse: isReversed is false. */
  workflow: string | null;
  /** Number of steps that TRANSITIVELY precede this step in the ntwf:precedesStep ordering — a rollup over the closure view vw_step_precedence_closure counting rows whose to_id is this step (i.e. this step's ancestors). On the linear Production Deployment chain: 0,1,2,3,4. Derived purely from the asserted StepPrecedence edges via their transitive closure; nothing is hand-entered. SequencePosition is this + 1. */
  preceding_step_count: number | null;
  /** The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering). */
  inferred_sequence_position: number | null;
  /** OPTIONAL hand-asserted ordinal position — the article's pure ntwf:sequencePosition functional datatype property, preserved as an override slot. NULL on the linear Production Deployment chain (the inference is unambiguous, so nothing is pinned). When set, it wins over InferredSequencePosition in the resolved SequencePosition — this is how a modeler recovers the owl:FunctionalProperty 'exactly one distinct position per step' guarantee on a partial order / branch where the inferred rank would tie. Maps to ntwf:sequencePosition (the article's asserted functional property). */
  sequence_position_override: number | null;
  /** The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers. */
  sequence_position: number | null;
  /** Foreign key to the Role responsible for executing this step. Maps to ntwf:assignedRole (owl:FunctionalProperty — exactly one role per step). Critical for implementing Heuristic 2 (role-agent separation): steps point to roles, not directly to agents. */
  assigned_role: string | null;
  /** Boolean flag indicating whether a human agent must fill the assigned role. Maps to ntwf:requiresHumanApproval. Enables answering CQ3: 'Which steps require human decisions vs. AI execution?' */
  requires_human_approval: boolean | null;
  /** Expected duration of this step in minutes. Maps to ntwf:stepDurationMinutes (datatype property, not functional). Enables SLA and throughput analysis. */
  step_duration_minutes: number | null;
  /** FK to Datasets. Records which DCAT dataset this step consumes as input. Kept separate from artifact consumption to preserve DCAT metadata semantics (consumesDataset vs. requiresArtifact). */
  consumes_dataset: string | null;
  /** Back-reference to WorkflowArtifacts produced by this step. Inverse of WorkflowArtifacts.ProducedByStep (ntwf:producesArtifact / prov:wasGeneratedBy). */
  produces_artifacts: string | null;
  /** FK to WorkflowArtifact(s) this step CONSUMES as input. Maps to ntwf:requiresArtifact (aligned to prov:used). Kept distinct from producesArtifact (prov:generated) and from consumesDataset (dcat:Dataset) so the input/output and artifact/dataset semantics stay separate. Inverse is WorkflowArtifacts.RequiredBySteps. */
  requires_artifacts: string | null;
  /** Back-reference to the ApprovalGate subtype row that specializes this step, if any. Inverse of ApprovalGates.WorkflowStep. A step has zero or one approval gate; when present, the gate adds escalationThresholdHours and marks the step as a blocking decision checkpoint. This models ntwf:ApprovalGate rdfs:subClassOf WorkflowStep as a shared-key 1:1 specialization rather than collapsing two DAG nodes into one. */
  approval_gate: string | null;
  /** Back-reference to StepPrecedence edges where this step is the FromStep (the predecessor). Inverse of StepPrecedence.FromStep. Together with PrecededBy, lets you walk the ntwf:precedesStep ordering in both directions. */
  precedes: string | null;
  /** Back-reference to StepPrecedence edges where this step is the ToStep (the successor). Inverse of StepPrecedence.ToStep. Part of the ntwf:precedesStep transitive ordering relationship. */
  preceded_by: string | null;
  /** The HumanAgent (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the HumanAgent arm). Load-bearing lookup: it follows the role→agent indirection the article relies on, so a step knows its executing agent without per-step agent bindings. */
  executing_human_agent: string | null;
  /** The AIAgent (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the AIAgent arm). One of the three polymorphic filledBy arms. */
  executing_ai_agent: string | null;
  /** The AutomatedPipeline (if any) that executes this step, resolved through ntwf:assignedRole → ntwf:filledBy (the AutomatedPipeline arm). */
  executing_automated_pipeline: string | null;
  /** Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision'). */
  executing_agent_type: string | null;
  /** TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps). */
  is_executed_by_ai: boolean | null;
  /** TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split. */
  is_executed_by_human: boolean | null;
  /** TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6). */
  is_approval_gate: boolean | null;
  /** Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check. */
  approval_consistency_violation: boolean | null;
  /** Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval. */
  approval_is_human_filled: boolean | null;
  /** The department that owns this step's assigned role, resolved through AssignedRole → Roles.OwnedBy. Lets a workflow report which departments its steps touch (CQ7: 'which workflows involve both Engineering and Legal, and at what steps do they intersect'). */
  owning_department: string | null;
  /** TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow). */
  is_legal_owned: boolean | null;
  /** TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check. */
  is_engineering_owned: boolean | null;
  _erb_errors?: Record<string, string>;
}

const workflowStepsFieldTypes: Record<string, FieldType> = {
  workflow_step_id: "string",
  parent_path: "*string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "*string",
  workflow: "*string",
  preceding_step_count: "*int",
  inferred_sequence_position: "*int",
  sequence_position_override: "*int",
  sequence_position: "*int",
  assigned_role: "*string",
  requires_human_approval: "*bool",
  step_duration_minutes: "*int",
  consumes_dataset: "*string",
  produces_artifacts: "*string",
  requires_artifacts: "*string",
  approval_gate: "*string",
  precedes: "*string",
  preceded_by: "*string",
  executing_human_agent: "*string",
  executing_ai_agent: "*string",
  executing_automated_pipeline: "*string",
  executing_agent_type: "*string",
  is_executed_by_ai: "*bool",
  is_executed_by_human: "*bool",
  is_approval_gate: "*bool",
  approval_consistency_violation: "*bool",
  approval_is_human_filled: "*bool",
  owning_department: "*string",
  is_legal_owned: "*bool",
  is_engineering_owned: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
 *  Formula: ={{ParentPath}} & "/steps/" & {{WorkflowStepId}} */
export function calcWorkflowStepsRelativePath(tc: WorkflowStepsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.parent_path)), vS("/steps/"), erbTextOr(vStrPlain(tc.workflow_step_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcWorkflowStepsIri(tc: WorkflowStepsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcWorkflowStepsName(tc: WorkflowStepsRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStr(tc.display_name)), vS(" "), vS("-")));
}

/** Computes the InferredSequencePosition calculated field.
 *  The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering).
 *  Formula: ={{PrecedingStepCount}} + 1 */
export function calcWorkflowStepsInferredSequencePosition(tc: WorkflowStepsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.preceding_step_count), vI(1))));
}

/** Computes the SequencePosition calculated field.
 *  The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
 *  Formula: =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, {{InferredSequencePosition}}) */
export function calcWorkflowStepsSequencePosition(tc: WorkflowStepsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbIsNotBlank(vInt(tc.sequence_position_override))), () => vInt(tc.sequence_position_override), () => vInt(tc.inferred_sequence_position))));
}

/** Computes the ExecutingAgentType calculated field.
 *  Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
 *  Formula: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", ""))) */
export function calcWorkflowStepsExecutingAgentType(tc: WorkflowStepsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.executing_human_agent))))), () => vS("HumanAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.executing_ai_agent))))), () => vS("AIAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.executing_automated_pipeline))))), () => vS("AutomatedPipeline"), () => vS("")))));
}

/** Computes the IsExecutedByAI calculated field.
 *  TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
 *  Formula: =NOT(ISBLANK({{ExecutingAIAgent}})) */
export function calcWorkflowStepsIsExecutedByAI(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.executing_ai_agent)))));
}

/** Computes the IsExecutedByHuman calculated field.
 *  TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
 *  Formula: =NOT(ISBLANK({{ExecutingHumanAgent}})) */
export function calcWorkflowStepsIsExecutedByHuman(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.executing_human_agent)))));
}

/** Computes the IsApprovalGate calculated field.
 *  TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
 *  Formula: =NOT(ISBLANK({{ApprovalGate}})) */
export function calcWorkflowStepsIsApprovalGate(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.approval_gate)))));
}

/** Computes the ApprovalConsistencyViolation calculated field.
 *  Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
 *  Formula: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}})) */
export function calcWorkflowStepsApprovalConsistencyViolation(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.requires_human_approval)), erbBool3(erbIsBlank(vStr(tc.executing_human_agent)))));
}

/** Computes the ApprovalIsHumanFilled calculated field.
 *  Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
 *  Formula: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE) */
export function calcWorkflowStepsApprovalIsHumanFilled(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbIf(erbIsTrue(vBool(tc.requires_human_approval)), () => erbNot(erbBool3(erbIsBlank(vStr(tc.executing_human_agent)))), () => vB(true)));
}

/** Computes the IsLegalOwned calculated field.
 *  TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
 *  Formula: ={{OwningDepartment}} = "ntwf-legal-dept" */
export function calcWorkflowStepsIsLegalOwned(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.owning_department), vS("ntwf-legal-dept")));
}

/** Computes the IsEngineeringOwned calculated field.
 *  TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
 *  Formula: ={{OwningDepartment}} = "ntwf-engineering" */
export function calcWorkflowStepsIsEngineeringOwned(tc: WorkflowStepsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.owning_department), vS("ntwf-engineering")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeWorkflowSteps(tc: WorkflowStepsRow): WorkflowStepsRow {
  // Level 1
  calcGuard(tc, workflowStepsFieldTypes, "relative_path", () => { tc.relative_path = calcWorkflowStepsRelativePath(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "name", () => { tc.name = calcWorkflowStepsName(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "inferred_sequence_position", () => { tc.inferred_sequence_position = calcWorkflowStepsInferredSequencePosition(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "executing_agent_type", () => { tc.executing_agent_type = calcWorkflowStepsExecutingAgentType(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "is_executed_by_ai", () => { tc.is_executed_by_ai = calcWorkflowStepsIsExecutedByAI(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "is_executed_by_human", () => { tc.is_executed_by_human = calcWorkflowStepsIsExecutedByHuman(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "is_approval_gate", () => { tc.is_approval_gate = calcWorkflowStepsIsApprovalGate(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "approval_consistency_violation", () => { tc.approval_consistency_violation = calcWorkflowStepsApprovalConsistencyViolation(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "approval_is_human_filled", () => { tc.approval_is_human_filled = calcWorkflowStepsApprovalIsHumanFilled(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "is_legal_owned", () => { tc.is_legal_owned = calcWorkflowStepsIsLegalOwned(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "is_engineering_owned", () => { tc.is_engineering_owned = calcWorkflowStepsIsEngineeringOwned(tc); });
  // Level 2
  calcGuard(tc, workflowStepsFieldTypes, "iri", () => { tc.iri = calcWorkflowStepsIri(tc); });
  calcGuard(tc, workflowStepsFieldTypes, "sequence_position", () => { tc.sequence_position = calcWorkflowStepsSequencePosition(tc); });
  return tc;
}

/** Reads WorkflowSteps rows from a JSON array file. */
export function loadWorkflowStepsRows(file: string): WorkflowStepsRow[] {
  return loadRows(file, { fields: workflowStepsFieldTypes }) as unknown as WorkflowStepsRow[];
}

// =============================================================================
// APPROVALGATES TABLE
// Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
// =============================================================================

/** A row in the ApprovalGates table. */
export interface ApprovalGatesRow {
  approval_gate_id: string;
  /** Helper: the WorkflowSteps parent's RelativePath, pulled across the WorkflowStep FK. Exists so RelativePath can concatenate the '/approval-gates/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat). */
  parent_path: string | null;
  /** Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  name: string | null;
  display_name: string | null;
  /** 1:1 FK to the WorkflowStep this gate specializes (subtype shared key). This is the relational expression of ntwf:ApprovalGate rdfs:subClassOf WorkflowStep: the gate row adds escalationThresholdHours to its step. Inverse is WorkflowSteps.ApprovalGate. */
  workflow_step: string | null;
  /** Integer number of hours that may elapse on a pending gate before the ntwf:delegatesTo chain activates. Maps to ntwf:escalationThresholdHours. Domain applies only to ApprovalGate individuals — which is exactly why the gate is its own subtype table and this attribute does not live on every WorkflowStep. */
  escalation_threshold_hours: number | null;
  /** The role responsible for this gate's underlying step, resolved through WorkflowStep → WorkflowSteps.AssignedRole. First hop of the CQ2 chain (gate → role → approver). */
  gate_role: string | null;
  /** The human agent who approves at this gate, resolved through the two-hop chain gate → GateRole → Roles.FilledByHumanAgent. Answers CQ2 ('who is responsible for approving a production deployment') directly: the release-approval gate resolves to the Release Manager role, filled by Maria Gonzalez. */
  gate_approver_human: string | null;
  /** TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads. */
  has_human_approver: boolean | null;
  _erb_errors?: Record<string, string>;
}

const approvalGatesFieldTypes: Record<string, FieldType> = {
  approval_gate_id: "string",
  parent_path: "*string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "*string",
  workflow_step: "*string",
  escalation_threshold_hours: "*int",
  gate_role: "*string",
  gate_approver_human: "*string",
  has_human_approver: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
 *  Formula: ={{ParentPath}} & "/approval-gates/" & {{ApprovalGateId}} */
export function calcApprovalGatesRelativePath(tc: ApprovalGatesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.parent_path)), vS("/approval-gates/"), erbTextOr(vStrPlain(tc.approval_gate_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcApprovalGatesIri(tc: ApprovalGatesRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcApprovalGatesName(tc: ApprovalGatesRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStr(tc.display_name)), vS(" "), vS("-")));
}

/** Computes the HasHumanApprover calculated field.
 *  TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads.
 *  Formula: =NOT(ISBLANK({{GateApproverHuman}})) */
export function calcApprovalGatesHasHumanApprover(tc: ApprovalGatesRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.gate_approver_human)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeApprovalGates(tc: ApprovalGatesRow): ApprovalGatesRow {
  // Level 1
  calcGuard(tc, approvalGatesFieldTypes, "relative_path", () => { tc.relative_path = calcApprovalGatesRelativePath(tc); });
  calcGuard(tc, approvalGatesFieldTypes, "name", () => { tc.name = calcApprovalGatesName(tc); });
  calcGuard(tc, approvalGatesFieldTypes, "has_human_approver", () => { tc.has_human_approver = calcApprovalGatesHasHumanApprover(tc); });
  // Level 2
  calcGuard(tc, approvalGatesFieldTypes, "iri", () => { tc.iri = calcApprovalGatesIri(tc); });
  return tc;
}

/** Reads ApprovalGates rows from a JSON array file. */
export function loadApprovalGatesRows(file: string): ApprovalGatesRow[] {
  return loadRows(file, { fields: approvalGatesFieldTypes }) as unknown as ApprovalGatesRow[];
}

// =============================================================================
// STEPPRECEDENCE TABLE
// Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
// =============================================================================

/** A row in the StepPrecedence table. */
export interface StepPrecedenceRow {
  step_precedence_id: string;
  /** Helper: the WorkflowSteps parent's RelativePath, pulled across the FromStep FK. Exists so RelativePath can concatenate the '/precedence/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat). */
  parent_path: string | null;
  /** Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction. */
  name: string | null;
  /** FK to the predecessor WorkflowStep — the step that comes BEFORE. The source of the ntwf:precedesStep edge. Inverse is WorkflowSteps.Precedes. */
  from_step: string;
  /** FK to the successor WorkflowStep — the step that comes AFTER. The target of the ntwf:precedesStep edge. Inverse is WorkflowSteps.PrecededBy. */
  to_step: string;
  /** Transitive closure of ntwf:precedesStep (an owl:TransitiveProperty). The 4 asserted edges (1→2, 2→3, 3→4, 4→5) imply the full 10-pair ordering closure — including the never-asserted step-1 → step-5. Materialized by the transpiler as the view vw_step_precedence_closure(from_id, to_id, hop_distance, is_inferred): 4 asserted (hop 1) + 6 inferred rows. This is the article's headline inference made to fire, not seeded. */
  precedes_step_closure: unknown;
  _erb_errors?: Record<string, string>;
}

const stepPrecedenceFieldTypes: Record<string, FieldType> = {
  step_precedence_id: "string",
  parent_path: "*string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  from_step: "string",
  to_step: "string",
  precedes_step_closure: "any",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
 *  Formula: ={{ParentPath}} & "/precedence/" & {{StepPrecedenceId}} */
export function calcStepPrecedenceRelativePath(tc: StepPrecedenceRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.parent_path)), vS("/precedence/"), erbTextOr(vStrPlain(tc.step_precedence_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcStepPrecedenceIri(tc: StepPrecedenceRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
 *  Formula: ={{FromStep}} & " -> " & {{ToStep}} */
export function calcStepPrecedenceName(tc: StepPrecedenceRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStrPlain(tc.from_step)), vS(" -> "), erbTextOr(vStrPlain(tc.to_step))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepPrecedence(tc: StepPrecedenceRow): StepPrecedenceRow {
  // Level 1
  calcGuard(tc, stepPrecedenceFieldTypes, "relative_path", () => { tc.relative_path = calcStepPrecedenceRelativePath(tc); });
  calcGuard(tc, stepPrecedenceFieldTypes, "name", () => { tc.name = calcStepPrecedenceName(tc); });
  // Level 2
  calcGuard(tc, stepPrecedenceFieldTypes, "iri", () => { tc.iri = calcStepPrecedenceIri(tc); });
  return tc;
}

/** Reads StepPrecedence rows from a JSON array file. */
export function loadStepPrecedenceRows(file: string): StepPrecedenceRow[] {
  return loadRows(file, { fields: stepPrecedenceFieldTypes }) as unknown as StepPrecedenceRow[];
}

// =============================================================================
// ROLES TABLE
// Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
// =============================================================================

/** A row in the Roles table. */
export interface RolesRow {
  role_id: string;
  /** Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  name: string | null;
  display_name: string | null;
  /** Human-readable display name. Maps to rdfs:label. Per Heuristic 6: if you cannot write a clear label, you do not yet understand the concept well enough to model it. */
  label: string | null;
  /** Detailed description of the role's responsibilities and scope. Maps to rdfs:comment. Should define what the role covers, what it excludes, and how it differs from adjacent roles. */
  comment: string | null;
  /** FK to AgentCapabilityConcepts. Declares the capability this role requires of its filler. Maps to ntwf:hasCapability. Enables CQ: 'Which roles require AI-specific capabilities?' */
  has_capability: string | null;
  /** One arm of the polymorphic ntwf:filledBy relationship: FK to the HumanAgent that fills this role today. Exactly one of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline is set per role (filledBy is functional; the three agent types are owl:disjointWith each other). Roles whose capability requires human judgment or legal review must use this arm. */
  filled_by_human_agent: string | null;
  /** One arm of the polymorphic ntwf:filledBy relationship: FK to the AIAgent that fills this role today. Exactly one filledBy arm is set per role. An AIAgent may fill probabilistic-capability roles (e.g. risk analysis) but never a role whose step has requiresHumanApproval. */
  filled_by_ai_agent: string | null;
  /** One arm of the polymorphic ntwf:filledBy relationship: FK to the AutomatedPipeline that fills this role today. Exactly one filledBy arm is set per role. Pipelines fill deterministic execution roles (e.g. CI/CD). */
  filled_by_automated_pipeline: string | null;
  /** Foreign key to the Department that owns this role. Maps to ntwf:ownedBy (owl:FunctionalProperty). Enables answering CQ7: 'Which workflows involve both Engineering and Legal?' */
  owned_by: string | null;
  /** Foreign key to the next Role in the escalation chain. Maps to ntwf:delegatesTo (traversable via the SPARQL property path delegatesTo+). Enables answering CQ6: 'What happens when the Release Manager / VP of Engineering is unavailable?' */
  delegates_to: string | null;
  /** Back-reference to workflow steps assigned to this role. Inverse of WorkflowSteps.AssignedRole. */
  workflow_steps: string | null;
  /** Back-reference: the Role that delegates TO this role (one step up the escalation chain). Inverse of Roles.DelegatesTo. */
  from_delegates_to: string | null;
  /** Back-reference to the temporal filledBy history for this role (every validity period, current and retained). Inverse of RoleAssignments.Role. */
  role_assignments: string | null;
  /** Transitive closure of ntwf:delegatesTo over the self-referential DelegatesTo FK. The asserted escalation edges (Release Manager → VP Engineering, VP Engineering → CTO) imply the never-asserted reachability Release Manager → CTO. Materialized as vw_roles_closure(from_id, to_id, hop_distance, is_inferred). This is the SQL equivalent of the SPARQL delegatesTo+ property path. */
  delegation_closure: unknown;
  /** Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint. */
  filled_by_arm_count: number | null;
  /** Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE. */
  has_exactly_one_filler: boolean | null;
  /** Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'. */
  filler_type: string | null;
  /** Number of this role's assigned WorkflowSteps that are approval gates (rollup over WorkflowSteps.IsApprovalGate). Greater than zero marks a role that owns a blocking decision checkpoint and therefore MUST have a complete delegatesTo escalation path — the precondition for EscalationViolation. Worked example: 1 for the Release Manager (who fills the Release Approval Gate), 0 for every other role. */
  fills_approval_gate: number | null;
  /** Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on. */
  escalation_violation: boolean | null;
  _erb_errors?: Record<string, string>;
}

const rolesFieldTypes: Record<string, FieldType> = {
  role_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "*string",
  label: "*string",
  comment: "*string",
  has_capability: "*string",
  filled_by_human_agent: "*string",
  filled_by_ai_agent: "*string",
  filled_by_automated_pipeline: "*string",
  owned_by: "*string",
  delegates_to: "*string",
  workflow_steps: "*string",
  from_delegates_to: "*string",
  role_assignments: "*string",
  delegation_closure: "any",
  filled_by_arm_count: "*int",
  has_exactly_one_filler: "*bool",
  filler_type: "*string",
  fills_approval_gate: "*int",
  escalation_violation: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="roles/" & {{RoleId}} */
export function calcRolesRelativePath(tc: RolesRow): string | null {
  return toStringPtr(erbConcat(vS("roles/"), erbTextOr(vStrPlain(tc.role_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcRolesIri(tc: RolesRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcRolesName(tc: RolesRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStr(tc.display_name)), vS(" "), vS("-")));
}

/** Computes the FilledByArmCount calculated field.
 *  Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint.
 *  Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0) */
export function calcRolesFilledByArmCount(tc: RolesRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbAdd(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_human_agent))))), () => vI(1), () => vI(0)), erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_ai_agent))))), () => vI(1), () => vI(0))), erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_automated_pipeline))))), () => vI(1), () => vI(0)))));
}

/** Computes the HasExactlyOneFiller calculated field.
 *  Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
 *  Formula: ={{FilledByArmCount}} = 1 */
export function calcRolesHasExactlyOneFiller(tc: RolesRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.filled_by_arm_count), vI(1)));
}

/** Computes the FillerType calculated field.
 *  Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
 *  Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", ""))) */
export function calcRolesFillerType(tc: RolesRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_human_agent))))), () => vS("HumanAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_ai_agent))))), () => vS("AIAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_automated_pipeline))))), () => vS("AutomatedPipeline"), () => vS("")))));
}

/** Computes the EscalationViolation calculated field.
 *  Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
 *  Formula: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}})) */
export function calcRolesEscalationViolation(tc: RolesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.fills_approval_gate), ">", vI(0))), erbBool3(erbIsBlank(vStr(tc.delegates_to)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoles(tc: RolesRow): RolesRow {
  // Level 1
  calcGuard(tc, rolesFieldTypes, "relative_path", () => { tc.relative_path = calcRolesRelativePath(tc); });
  calcGuard(tc, rolesFieldTypes, "name", () => { tc.name = calcRolesName(tc); });
  calcGuard(tc, rolesFieldTypes, "filled_by_arm_count", () => { tc.filled_by_arm_count = calcRolesFilledByArmCount(tc); });
  calcGuard(tc, rolesFieldTypes, "filler_type", () => { tc.filler_type = calcRolesFillerType(tc); });
  calcGuard(tc, rolesFieldTypes, "escalation_violation", () => { tc.escalation_violation = calcRolesEscalationViolation(tc); });
  // Level 2
  calcGuard(tc, rolesFieldTypes, "iri", () => { tc.iri = calcRolesIri(tc); });
  calcGuard(tc, rolesFieldTypes, "has_exactly_one_filler", () => { tc.has_exactly_one_filler = calcRolesHasExactlyOneFiller(tc); });
  return tc;
}

/** Reads Roles rows from a JSON array file. */
export function loadRolesRows(file: string): RolesRow[] {
  return loadRows(file, { fields: rolesFieldTypes }) as unknown as RolesRow[];
}

// =============================================================================
// ROLEASSIGNMENTS TABLE
// Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
// =============================================================================

/** A row in the RoleAssignments table. */
export interface RoleAssignmentsRow {
  role_assignment_id: string;
  /** Helper: the Roles parent's RelativePath, pulled across the Role FK. Exists so RelativePath can concatenate the '/assignments/' segment using only local-field '&' concat. */
  parent_path: string | null;
  /** Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique. */
  iri: string | null;
  /** Human-readable label for this assignment period: the role and the validity window. */
  name: string | null;
  /** FK to the Role this assignment binds an agent to. The subject of the historical ntwf:filledBy triple. */
  role: string;
  /** One arm of the polymorphic filledBy binding for this assignment period: FK to the HumanAgent who filled the role during this window. Exactly one filler arm is set per assignment. */
  filled_by_human_agent: string | null;
  /** One arm of the polymorphic filledBy binding: FK to the AIAgent who filled the role during this window. Exactly one filler arm is set per assignment. */
  filled_by_ai_agent: string | null;
  /** One arm of the polymorphic filledBy binding: FK to the AutomatedPipeline that filled the role during this window. Exactly one filler arm is set per assignment. */
  filled_by_automated_pipeline: string | null;
  /** Start of the validity period for this filledBy binding (inclusive). A retained/versioned triple carries the validity period. ISO date. */
  valid_from: string;
  /** End of the validity period for this filledBy binding (exclusive). Blank means the binding is still current — this is the live ntwf:filledBy value mirrored on Roles. A non-blank value means the binding was superseded; the row is retained (not deleted) to preserve provenance. */
  valid_to: string | null;
  /** The WHY of the change: the audit record must reflect when that transition happened and why. e.g. 'initial assignment', 'departure / backfill', 'model upgrade', 'compliance reassignment to human'. */
  reason: string | null;
  /** The agent class (HumanAgent / AIAgent / AutomatedPipeline) of the binding this assignment SUPERSEDED, or blank for the first assignment of a role. Lets the agent-type-change audit (AIAgent -> HumanAgent) be witnessed without re-deriving from the prior row. */
  prior_filler_type: string | null;
  /** Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding. */
  filler_type: string | null;
  /** TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips. */
  is_current: boolean | null;
  /** NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained. */
  was_active_as_of_audit_date: boolean | null;
  /** TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight. */
  is_agent_type_change: boolean | null;
  /** Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why). */
  requires_compliance_audit: boolean | null;
  _erb_errors?: Record<string, string>;
}

const roleAssignmentsFieldTypes: Record<string, FieldType> = {
  role_assignment_id: "string",
  parent_path: "*string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  role: "string",
  filled_by_human_agent: "*string",
  filled_by_ai_agent: "*string",
  filled_by_automated_pipeline: "*string",
  valid_from: "string",
  valid_to: "*string",
  reason: "*string",
  prior_filler_type: "*string",
  filler_type: "*string",
  is_current: "*bool",
  was_active_as_of_audit_date: "*bool",
  is_agent_type_change: "*bool",
  requires_compliance_audit: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
 *  Formula: ={{ParentPath}} & "/assignments/" & {{RoleAssignmentId}} */
export function calcRoleAssignmentsRelativePath(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.parent_path)), vS("/assignments/"), erbTextOr(vStrPlain(tc.role_assignment_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcRoleAssignmentsIri(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Human-readable label for this assignment period: the role and the validity window.
 *  Formula: ={{Role}} & " [" & {{ValidFrom}} & " -> " & IF(ISBLANK({{ValidTo}}), "open", {{ValidTo}}) & "]" */
export function calcRoleAssignmentsName(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStrPlain(tc.role)), vS(" ["), erbTextOr(vStrPlain(tc.valid_from)), vS(" -> "), erbTextNotNull(erbIf(erbBool3(erbIsBlank(vStr(tc.valid_to))), () => vS("open"), () => vStr(tc.valid_to))), vS("]")));
}

/** Computes the FillerType calculated field.
 *  Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
 *  Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", ""))) */
export function calcRoleAssignmentsFillerType(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_human_agent))))), () => vS("HumanAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_ai_agent))))), () => vS("AIAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.filled_by_automated_pipeline))))), () => vS("AutomatedPipeline"), () => vS("")))));
}

/** Computes the IsCurrent calculated field.
 *  TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips.
 *  Formula: =ISBLANK({{ValidTo}}) */
export function calcRoleAssignmentsIsCurrent(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbIsBlank(vStr(tc.valid_to)));
}

/** Computes the WasActiveAsOfAuditDate calculated field.
 *  NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained.
 *  Formula: =AND({{ValidFrom}} <= "2026-03-01", OR(ISBLANK({{ValidTo}}), {{ValidTo}} > "2026-03-01")) */
export function calcRoleAssignmentsWasActiveAsOfAuditDate(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vStrPlain(tc.valid_from)), "<=", vS("2026-03-01"))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vS("2026-03-01")))))));
}

/** Computes the IsAgentTypeChange calculated field.
 *  TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight.
 *  Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} <> {{FillerType}}) */
export function calcRoleAssignmentsIsAgentTypeChange(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.prior_filler_type))))), erbBool3(erbNe(erbNullif(vStr(tc.prior_filler_type)), vStr(tc.filler_type)))));
}

/** Computes the RequiresComplianceAudit calculated field.
 *  Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
 *  Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent") */
export function calcRoleAssignmentsRequiresComplianceAudit(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.prior_filler_type))))), erbBool3(erbEq(erbNullif(vStr(tc.prior_filler_type)), vS("AIAgent"))), erbBool3(erbEq(vStr(tc.filler_type), vS("HumanAgent")))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoleAssignments(tc: RoleAssignmentsRow): RoleAssignmentsRow {
  // Level 1
  calcGuard(tc, roleAssignmentsFieldTypes, "relative_path", () => { tc.relative_path = calcRoleAssignmentsRelativePath(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "name", () => { tc.name = calcRoleAssignmentsName(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "filler_type", () => { tc.filler_type = calcRoleAssignmentsFillerType(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_current", () => { tc.is_current = calcRoleAssignmentsIsCurrent(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "was_active_as_of_audit_date", () => { tc.was_active_as_of_audit_date = calcRoleAssignmentsWasActiveAsOfAuditDate(tc); });
  // Level 2
  calcGuard(tc, roleAssignmentsFieldTypes, "iri", () => { tc.iri = calcRoleAssignmentsIri(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_agent_type_change", () => { tc.is_agent_type_change = calcRoleAssignmentsIsAgentTypeChange(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "requires_compliance_audit", () => { tc.requires_compliance_audit = calcRoleAssignmentsRequiresComplianceAudit(tc); });
  return tc;
}

/** Reads RoleAssignments rows from a JSON array file. */
export function loadRoleAssignmentsRows(file: string): RoleAssignmentsRow[] {
  return loadRows(file, { fields: roleAssignmentsFieldTypes }) as unknown as RoleAssignmentsRow[];
}

// =============================================================================
// DEPARTMENTS TABLE
// Table: Departments. The NTWF Department class — schema:Organization. First-class entity that enables cross-department intersection queries (CQ7: which workflows involve both Engineering and Legal?). Roles are ownedBy a department.
// =============================================================================

/** A row in the Departments table. */
export interface DepartmentsRow {
  department_id: string;
  /** Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Human-readable display name of the department. Should match organizational terminology for stakeholder communication. */
  name: string | null;
  /** Formal organizational title of the department. Maps to schema:name / dct:title. */
  title: string | null;
  /** Machine-friendly name for programmatic reference. */
  display_name: string | null;
  /** Back-reference to roles owned by this department. Inverse of Roles.OwnedBy. */
  roles: string | null;
  _erb_errors?: Record<string, string>;
}

const departmentsFieldTypes: Record<string, FieldType> = {
  department_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  title: "*string",
  display_name: "*string",
  roles: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="departments/" & {{DepartmentId}} */
export function calcDepartmentsRelativePath(tc: DepartmentsRow): string | null {
  return toStringPtr(erbConcat(vS("departments/"), erbTextOr(vStrPlain(tc.department_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcDepartmentsIri(tc: DepartmentsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Human-readable display name of the department. Should match organizational terminology for stakeholder communication.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcDepartmentsName(tc: DepartmentsRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStr(tc.display_name)), vS(" "), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeDepartments(tc: DepartmentsRow): DepartmentsRow {
  // Level 1
  calcGuard(tc, departmentsFieldTypes, "relative_path", () => { tc.relative_path = calcDepartmentsRelativePath(tc); });
  calcGuard(tc, departmentsFieldTypes, "name", () => { tc.name = calcDepartmentsName(tc); });
  // Level 2
  calcGuard(tc, departmentsFieldTypes, "iri", () => { tc.iri = calcDepartmentsIri(tc); });
  return tc;
}

/** Reads Departments rows from a JSON array file. */
export function loadDepartmentsRows(file: string): DepartmentsRow[] {
  return loadRows(file, { fields: departmentsFieldTypes }) as unknown as DepartmentsRow[];
}

// =============================================================================
// HUMANAGENTS TABLE
// Table: HumanAgents. The NTWF HumanAgent class — foaf:Person + prov:Agent. The only agent type permitted to fill roles whose step has requiresHumanApproval. Disjoint with AIAgent and AutomatedPipeline.
// =============================================================================

/** A row in the HumanAgents table. */
export interface HumanAgentsRow {
  human_agent_id: string;
  /** Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Full name of the person. Maps to foaf:name. Note: FOAF's name property is appropriate for persons, not for software systems (which use schema:name). */
  name: string | null;
  display_name: string | null;
  /** Email address of the person. Maps to foaf:mbox. Used for notifications and organizational directory integration. */
  mbox: string | null;
  /** Back-reference to roles currently filled by this agent. Inverse of Roles.FilledByHumanAgent. */
  roles: string | null;
  /** Back-reference to historical filledBy assignment periods in which this human filled a role. Inverse of RoleAssignments.FilledByHumanAgent. */
  role_assignments: string | null;
  _erb_errors?: Record<string, string>;
}

const humanAgentsFieldTypes: Record<string, FieldType> = {
  human_agent_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "*string",
  mbox: "*string",
  roles: "*string",
  role_assignments: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="human-agents/" & {{HumanAgentId}} */
export function calcHumanAgentsRelativePath(tc: HumanAgentsRow): string | null {
  return toStringPtr(erbConcat(vS("human-agents/"), erbTextOr(vStrPlain(tc.human_agent_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcHumanAgentsIri(tc: HumanAgentsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeHumanAgents(tc: HumanAgentsRow): HumanAgentsRow {
  // Level 1
  calcGuard(tc, humanAgentsFieldTypes, "relative_path", () => { tc.relative_path = calcHumanAgentsRelativePath(tc); });
  // Level 2
  calcGuard(tc, humanAgentsFieldTypes, "iri", () => { tc.iri = calcHumanAgentsIri(tc); });
  return tc;
}

/** Reads HumanAgents rows from a JSON array file. */
export function loadHumanAgentsRows(file: string): HumanAgentsRow[] {
  return loadRows(file, { fields: humanAgentsFieldTypes }) as unknown as HumanAgentsRow[];
}

// =============================================================================
// AIAGENTS TABLE
// Table: AIAgents. The NTWF AIAgent class — prov:SoftwareAgent + ntwf:modelVersion. Distinguished from AutomatedPipeline by probabilistic (vs. deterministic) output semantics. Disjoint with HumanAgent and AutomatedPipeline. May never fill a role whose step has requiresHumanApproval.
// =============================================================================

/** A row in the AIAgents table. */
export interface AIAgentsRow {
  ai_agent_id: string;
  /** Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Display name of the AI agent. Maps to schema:name (not foaf:name, which is for persons). */
  name: string | null;
  /** Descriptive title of the AI agent's function. */
  title: string | null;
  display_name: string | null;
  /** Version string of the AI model. Maps to ntwf:modelVersion. Makes AI-produced artifacts auditable at the version level. The domain declaration means this property applies only to AIAgent individuals. */
  model_version: string | null;
  /** Deployment date of this AI model version. The NTWF graph doubles as an AI system registry: 'risk-classifier-v2.4.1 was deployed on 2026-01-10'. Sourced from the AI system registry feed via the shared Dublin Core contract (dct:date). */
  deployed_on: string | null;
  /** Back-reference to roles currently filled by this AI agent. Inverse of Roles.FilledByAIAgent. */
  roles: string | null;
  /** Back-reference to historical filledBy assignment periods filled by this AI agent. Inverse of RoleAssignments.FilledByAIAgent. */
  role_assignments: string | null;
  /** Back-reference to WorkflowArtifacts attributed to this AI agent (prov:wasAttributedTo). Inverse of WorkflowArtifacts.AttributedToAIAgent. First leg of the 'blast radius' traversal. */
  attributed_artifacts: string | null;
  /** 'Blast radius', leg 1: how many artifacts are attributed to this AI agent (prov:wasAttributedTo). Counts WorkflowArtifacts whose AttributedToAIAgent is this agent. */
  count_attributed_artifacts: number | null;
  /** 'Blast radius', summarized: the number of distinct workflows reachable from this agent's attributed artifacts (each artifact is produced by a step that belongs to a workflow). With one workflow in the worked example, an upgrade to an agent that produced any artifact has a blast radius of 1 workflow. Counts artifacts attributed to this agent that resolve to a workflow. */
  count_impacted_workflows: number | null;
  _erb_errors?: Record<string, string>;
}

const aIAgentsFieldTypes: Record<string, FieldType> = {
  ai_agent_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  title: "*string",
  display_name: "*string",
  model_version: "*string",
  deployed_on: "*string",
  roles: "*string",
  role_assignments: "*string",
  attributed_artifacts: "*string",
  count_attributed_artifacts: "*int",
  count_impacted_workflows: "*int",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="ai-agents/" & {{AIAgentId}} */
export function calcAIAgentsRelativePath(tc: AIAgentsRow): string | null {
  return toStringPtr(erbConcat(vS("ai-agents/"), erbTextOr(vStrPlain(tc.ai_agent_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcAIAgentsIri(tc: AIAgentsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAIAgents(tc: AIAgentsRow): AIAgentsRow {
  // Level 1
  calcGuard(tc, aIAgentsFieldTypes, "relative_path", () => { tc.relative_path = calcAIAgentsRelativePath(tc); });
  // Level 2
  calcGuard(tc, aIAgentsFieldTypes, "iri", () => { tc.iri = calcAIAgentsIri(tc); });
  return tc;
}

/** Reads AIAgents rows from a JSON array file. */
export function loadAIAgentsRows(file: string): AIAgentsRow[] {
  return loadRows(file, { fields: aIAgentsFieldTypes }) as unknown as AIAgentsRow[];
}

// =============================================================================
// AUTOMATEDPIPELINES TABLE
// Table: AutomatedPipelines. The NTWF AutomatedPipeline class — prov:SoftwareAgent + schema:SoftwareApplication. Distinguished from AIAgent by deterministic (vs. probabilistic) output semantics. Disjoint with HumanAgent and AIAgent. Carries schema:name, not foaf:name.
// =============================================================================

/** A row in the AutomatedPipelines table. */
export interface AutomatedPipelinesRow {
  automated_pipeline_id: string;
  /** Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Display name of the pipeline. Maps to schema:name (appropriate for software systems, unlike foaf:name which is for persons). */
  name: string | null;
  /** Description of what the pipeline does and its execution semantics (deterministic, no probabilistic output). */
  description: string | null;
  display_name: string | null;
  /** Back-reference to roles currently filled by this pipeline. Inverse of Roles.FilledByAutomatedPipeline. */
  roles: string | null;
  /** Back-reference to historical filledBy assignment periods filled by this pipeline. Inverse of RoleAssignments.FilledByAutomatedPipeline. */
  role_assignments: string | null;
  _erb_errors?: Record<string, string>;
}

const automatedPipelinesFieldTypes: Record<string, FieldType> = {
  automated_pipeline_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  description: "*string",
  display_name: "*string",
  roles: "*string",
  role_assignments: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="automated-pipelines/" & {{AutomatedPipelineId}} */
export function calcAutomatedPipelinesRelativePath(tc: AutomatedPipelinesRow): string | null {
  return toStringPtr(erbConcat(vS("automated-pipelines/"), erbTextOr(vStrPlain(tc.automated_pipeline_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcAutomatedPipelinesIri(tc: AutomatedPipelinesRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAutomatedPipelines(tc: AutomatedPipelinesRow): AutomatedPipelinesRow {
  // Level 1
  calcGuard(tc, automatedPipelinesFieldTypes, "relative_path", () => { tc.relative_path = calcAutomatedPipelinesRelativePath(tc); });
  // Level 2
  calcGuard(tc, automatedPipelinesFieldTypes, "iri", () => { tc.iri = calcAutomatedPipelinesIri(tc); });
  return tc;
}

/** Reads AutomatedPipelines rows from a JSON array file. */
export function loadAutomatedPipelinesRows(file: string): AutomatedPipelinesRow[] {
  return loadRows(file, { fields: automatedPipelinesFieldTypes }) as unknown as AutomatedPipelinesRow[];
}

// =============================================================================
// WORKFLOWSTATUSCONCEPTS TABLE
// SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows.
// =============================================================================

/** A row in the WorkflowStatusConcepts table. */
export interface WorkflowStatusConceptsRow {
  concept_id: string;
  /** Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Preferred human-readable label. Maps to skos:prefLabel. */
  pref_label: string;
  /** Alternative label or synonym. Maps to skos:altLabel. */
  alt_label: string | null;
  /** Formal definition of the concept. Maps to skos:definition. */
  definition: string | null;
  /** Usage guidance for the concept. Maps to skos:scopeNote. */
  scope_note: string | null;
  /** Back-reference to workflows currently in this status. Inverse of Workflows.WorkflowStatus. */
  workflows: string | null;
  _erb_errors?: Record<string, string>;
}

const workflowStatusConceptsFieldTypes: Record<string, FieldType> = {
  concept_id: "string",
  relative_path: "*string",
  iri: "*string",
  pref_label: "string",
  alt_label: "*string",
  definition: "*string",
  scope_note: "*string",
  workflows: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="concepts/workflow-status/" & {{ConceptId}} */
export function calcWorkflowStatusConceptsRelativePath(tc: WorkflowStatusConceptsRow): string | null {
  return toStringPtr(erbConcat(vS("concepts/workflow-status/"), erbTextOr(vStrPlain(tc.concept_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcWorkflowStatusConceptsIri(tc: WorkflowStatusConceptsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeWorkflowStatusConcepts(tc: WorkflowStatusConceptsRow): WorkflowStatusConceptsRow {
  // Level 1
  calcGuard(tc, workflowStatusConceptsFieldTypes, "relative_path", () => { tc.relative_path = calcWorkflowStatusConceptsRelativePath(tc); });
  // Level 2
  calcGuard(tc, workflowStatusConceptsFieldTypes, "iri", () => { tc.iri = calcWorkflowStatusConceptsIri(tc); });
  return tc;
}

/** Reads WorkflowStatusConcepts rows from a JSON array file. */
export function loadWorkflowStatusConceptsRows(file: string): WorkflowStatusConceptsRow[] {
  return loadRows(file, { fields: workflowStatusConceptsFieldTypes }) as unknown as WorkflowStatusConceptsRow[];
}

// =============================================================================
// AGENTCAPABILITYCONCEPTS TABLE
// SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox.
// =============================================================================

/** A row in the AgentCapabilityConcepts table. */
export interface AgentCapabilityConceptsRow {
  concept_id: string;
  /** Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Preferred label. Maps to skos:prefLabel. */
  pref_label: string;
  /** Alternative label. Maps to skos:altLabel. */
  alt_label: string | null;
  /** Formal definition. Maps to skos:definition. */
  definition: string | null;
  /** Usage guidance. Maps to skos:scopeNote. */
  scope_note: string | null;
  /** Back-reference to roles requiring this capability. Inverse of Roles.HasCapability. */
  roles: string | null;
  _erb_errors?: Record<string, string>;
}

const agentCapabilityConceptsFieldTypes: Record<string, FieldType> = {
  concept_id: "string",
  relative_path: "*string",
  iri: "*string",
  pref_label: "string",
  alt_label: "*string",
  definition: "*string",
  scope_note: "*string",
  roles: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="concepts/agent-capability/" & {{ConceptId}} */
export function calcAgentCapabilityConceptsRelativePath(tc: AgentCapabilityConceptsRow): string | null {
  return toStringPtr(erbConcat(vS("concepts/agent-capability/"), erbTextOr(vStrPlain(tc.concept_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcAgentCapabilityConceptsIri(tc: AgentCapabilityConceptsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAgentCapabilityConcepts(tc: AgentCapabilityConceptsRow): AgentCapabilityConceptsRow {
  // Level 1
  calcGuard(tc, agentCapabilityConceptsFieldTypes, "relative_path", () => { tc.relative_path = calcAgentCapabilityConceptsRelativePath(tc); });
  // Level 2
  calcGuard(tc, agentCapabilityConceptsFieldTypes, "iri", () => { tc.iri = calcAgentCapabilityConceptsIri(tc); });
  return tc;
}

/** Reads AgentCapabilityConcepts rows from a JSON array file. */
export function loadAgentCapabilityConceptsRows(file: string): AgentCapabilityConceptsRow[] {
  return loadRows(file, { fields: agentCapabilityConceptsFieldTypes }) as unknown as AgentCapabilityConceptsRow[];
}

// =============================================================================
// ARTIFACTTYPECONCEPTS TABLE
// SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts.
// =============================================================================

/** A row in the ArtifactTypeConcepts table. */
export interface ArtifactTypeConceptsRow {
  concept_id: string;
  /** Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value. */
  iri: string | null;
  /** Preferred human-readable label. Maps to skos:prefLabel. */
  pref_label: string;
  /** Alternative label or synonym. Maps to skos:altLabel. */
  alt_label: string | null;
  /** Formal definition of the concept. Maps to skos:definition. */
  definition: string | null;
  /** Usage note clarifying boundaries. Maps to skos:scopeNote. */
  scope_note: string | null;
  /** Back-reference to WorkflowArtifacts classified under this concept. Inverse of WorkflowArtifacts.ArtifactType. */
  workflow_artifacts: string | null;
  _erb_errors?: Record<string, string>;
}

const artifactTypeConceptsFieldTypes: Record<string, FieldType> = {
  concept_id: "string",
  relative_path: "*string",
  iri: "*string",
  pref_label: "string",
  alt_label: "*string",
  definition: "*string",
  scope_note: "*string",
  workflow_artifacts: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key.
 *  Formula: ="concepts/artifact-type/" & {{ConceptId}} */
export function calcArtifactTypeConceptsRelativePath(tc: ArtifactTypeConceptsRow): string | null {
  return toStringPtr(erbConcat(vS("concepts/artifact-type/"), erbTextOr(vStrPlain(tc.concept_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcArtifactTypeConceptsIri(tc: ArtifactTypeConceptsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeArtifactTypeConcepts(tc: ArtifactTypeConceptsRow): ArtifactTypeConceptsRow {
  // Level 1
  calcGuard(tc, artifactTypeConceptsFieldTypes, "relative_path", () => { tc.relative_path = calcArtifactTypeConceptsRelativePath(tc); });
  // Level 2
  calcGuard(tc, artifactTypeConceptsFieldTypes, "iri", () => { tc.iri = calcArtifactTypeConceptsIri(tc); });
  return tc;
}

/** Reads ArtifactTypeConcepts rows from a JSON array file. */
export function loadArtifactTypeConceptsRows(file: string): ArtifactTypeConceptsRow[] {
  return loadRows(file, { fields: artifactTypeConceptsFieldTypes }) as unknown as ArtifactTypeConceptsRow[];
}

// =============================================================================
// DATASETS TABLE
// DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
// =============================================================================

/** A row in the Datasets table. */
export interface DatasetsRow {
  dataset_id: string;
  /** Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Human-readable dataset name. Maps to dct:title. */
  title: string;
  /** External system identifier. Maps to dct:identifier. Used for cross-referencing with data catalogs. */
  identifier: string | null;
  /** Last modification timestamp. Maps to dct:modified. */
  modified: string | null;
  /** URL of the data distribution. Maps to dcat:Distribution. The access endpoint for the dataset. */
  distribution_url: string | null;
  /** Back-reference to WorkflowSteps that consume this dataset. Inverse of WorkflowSteps.ConsumesDataset. Marked isReversed so every substrate DERIVES it from the forward FK (a reverse lookup over WorkflowSteps.ConsumesDataset) instead of storing it — keeping the two sides from drifting when the forward FK is edited. */
  consumed_by_steps: string | null;
  /** TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads. */
  is_consumed: boolean | null;
  _erb_errors?: Record<string, string>;
}

const datasetsFieldTypes: Record<string, FieldType> = {
  dataset_id: "string",
  relative_path: "*string",
  iri: "*string",
  title: "string",
  identifier: "*string",
  modified: "*string",
  distribution_url: "*string",
  consumed_by_steps: "*string",
  is_consumed: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
 *  Formula: ="datasets/" & {{DatasetId}} */
export function calcDatasetsRelativePath(tc: DatasetsRow): string | null {
  return toStringPtr(erbConcat(vS("datasets/"), erbTextOr(vStrPlain(tc.dataset_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcDatasetsIri(tc: DatasetsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the IsConsumed calculated field.
 *  TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads.
 *  Formula: =NOT(ISBLANK({{ConsumedBySteps}})) */
export function calcDatasetsIsConsumed(tc: DatasetsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.consumed_by_steps)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeDatasets(tc: DatasetsRow): DatasetsRow {
  // Level 1
  calcGuard(tc, datasetsFieldTypes, "relative_path", () => { tc.relative_path = calcDatasetsRelativePath(tc); });
  calcGuard(tc, datasetsFieldTypes, "is_consumed", () => { tc.is_consumed = calcDatasetsIsConsumed(tc); });
  // Level 2
  calcGuard(tc, datasetsFieldTypes, "iri", () => { tc.iri = calcDatasetsIri(tc); });
  return tc;
}

/** Reads Datasets rows from a JSON array file. */
export function loadDatasetsRows(file: string): DatasetsRow[] {
  return loadRows(file, { fields: datasetsFieldTypes }) as unknown as DatasetsRow[];
}

// =============================================================================
// WORKFLOWARTIFACTS TABLE
// Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
// =============================================================================

/** A row in the WorkflowArtifacts table. */
export interface WorkflowArtifactsRow {
  artifact_id: string;
  /** Helper: the WorkflowSteps parent's RelativePath, pulled across the ProducedByStep FK. Exists so RelativePath can concatenate the '/artifacts/' segment using only local-field '&' concat (the transpiler compiles a lookup as a pure passthrough, not a lookup+concat). */
  parent_path: string | null;
  /** Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions. */
  iri: string | null;
  /** Human-readable artifact name. Maps to dct:title. */
  title: string;
  /** External system identifier. Maps to dct:identifier. */
  identifier: string | null;
  /** FK to the ArtifactTypeConcepts SKOS concept classifying this artifact. Maps to dct:type. The CBox defines a concept scheme for artifact types. */
  artifact_type: string | null;
  /** Creation timestamp. Maps to dct:created. */
  created: string | null;
  /** FK to the WorkflowStep that produced this artifact. Maps to prov:wasGeneratedBy. Inverse of WorkflowSteps.ProducesArtifacts. */
  produced_by_step: string | null;
  /** Back-reference to the WorkflowStep(s) that consume this artifact as input (ntwf:requiresArtifact / prov:used). Inverse of WorkflowSteps.RequiresArtifacts. */
  required_by_steps: string | null;
  /** Self-FK to the artifact this one was derived from. Maps to prov:wasDerivedFrom. Enables the full provenance chain query (CQ4). */
  derived_from_artifact: string | null;
  /** FK to HumanAgent responsible for this artifact. One arm of prov:wasAttributedTo (exactly one AttributedTo arm is set per artifact, mirroring the disjoint agent types). */
  attributed_to_human_agent: string | null;
  /** FK to AIAgent responsible for this artifact. One arm of prov:wasAttributedTo. */
  attributed_to_ai_agent: string | null;
  /** FK to AutomatedPipeline responsible for this artifact. One arm of prov:wasAttributedTo. */
  attributed_to_automated_pipeline: string | null;
  /** Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from. */
  producing_agent_type: string | null;
  /** TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent. */
  has_derivation_parent: boolean | null;
  /** The workflow this artifact belongs to, resolved through ProducedByStep → WorkflowSteps.Workflow (artifact → producing step → workflow). Lets workflow-level rollups (e.g. CountDerivationLinks) aggregate artifacts without a redundant direct FK. */
  produced_by_workflow: string | null;
  /** TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column. */
  has_producing_workflow: boolean | null;
  /** Transitive closure of prov:wasDerivedFrom over the self-referential DerivedFromArtifact FK. The asserted single-step derivation edges (Legal Clearance was derived from Risk Report, Release Authorization from Legal Clearance, …) imply the never-asserted reachability (Post-Deployment Report transitively wasDerivedFrom Risk Report). Materialized as vw_workflow_artifacts_closure(from_id, to_id, hop_distance, is_inferred). This is the artifact-lineage analogue of vw_step_precedence_closure and vw_roles_closure — the SAME closure construct as step ordering and role escalation, just over a different relation, so a broken link surfaces as a missing reachability pair exactly like a dropped precedence edge. */
  derivation_closure: unknown;
  _erb_errors?: Record<string, string>;
}

const workflowArtifactsFieldTypes: Record<string, FieldType> = {
  artifact_id: "string",
  parent_path: "*string",
  relative_path: "*string",
  iri: "*string",
  title: "string",
  identifier: "*string",
  artifact_type: "*string",
  created: "*string",
  produced_by_step: "*string",
  required_by_steps: "*string",
  derived_from_artifact: "*string",
  attributed_to_human_agent: "*string",
  attributed_to_ai_agent: "*string",
  attributed_to_automated_pipeline: "*string",
  producing_agent_type: "*string",
  has_derivation_parent: "*bool",
  produced_by_workflow: "*string",
  has_producing_workflow: "*bool",
  derivation_closure: "any",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
 *  Formula: ={{ParentPath}} & "/artifacts/" & {{ArtifactId}} */
export function calcWorkflowArtifactsRelativePath(tc: WorkflowArtifactsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.parent_path)), vS("/artifacts/"), erbTextOr(vStrPlain(tc.artifact_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcWorkflowArtifactsIri(tc: WorkflowArtifactsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the ProducingAgentType calculated field.
 *  Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from.
 *  Formula: =IF(NOT(ISBLANK({{AttributedToHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{AttributedToAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{AttributedToAutomatedPipeline}})), "AutomatedPipeline", ""))) */
export function calcWorkflowArtifactsProducingAgentType(tc: WorkflowArtifactsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.attributed_to_human_agent))))), () => vS("HumanAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.attributed_to_ai_agent))))), () => vS("AIAgent"), () => erbIf(erbBool3(erbNot(erbBool3(erbIsBlank(vStr(tc.attributed_to_automated_pipeline))))), () => vS("AutomatedPipeline"), () => vS("")))));
}

/** Computes the HasDerivationParent calculated field.
 *  TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
 *  Formula: =NOT(ISBLANK({{DerivedFromArtifact}})) */
export function calcWorkflowArtifactsHasDerivationParent(tc: WorkflowArtifactsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.derived_from_artifact)))));
}

/** Computes the HasProducingWorkflow calculated field.
 *  TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
 *  Formula: =NOT(ISBLANK({{ProducedByWorkflow}})) */
export function calcWorkflowArtifactsHasProducingWorkflow(tc: WorkflowArtifactsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbIsBlank(vStr(tc.produced_by_workflow)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeWorkflowArtifacts(tc: WorkflowArtifactsRow): WorkflowArtifactsRow {
  // Level 1
  calcGuard(tc, workflowArtifactsFieldTypes, "relative_path", () => { tc.relative_path = calcWorkflowArtifactsRelativePath(tc); });
  calcGuard(tc, workflowArtifactsFieldTypes, "producing_agent_type", () => { tc.producing_agent_type = calcWorkflowArtifactsProducingAgentType(tc); });
  calcGuard(tc, workflowArtifactsFieldTypes, "has_derivation_parent", () => { tc.has_derivation_parent = calcWorkflowArtifactsHasDerivationParent(tc); });
  calcGuard(tc, workflowArtifactsFieldTypes, "has_producing_workflow", () => { tc.has_producing_workflow = calcWorkflowArtifactsHasProducingWorkflow(tc); });
  // Level 2
  calcGuard(tc, workflowArtifactsFieldTypes, "iri", () => { tc.iri = calcWorkflowArtifactsIri(tc); });
  return tc;
}

/** Reads WorkflowArtifacts rows from a JSON array file. */
export function loadWorkflowArtifactsRows(file: string): WorkflowArtifactsRow[] {
  return loadRows(file, { fields: workflowArtifactsFieldTypes }) as unknown as WorkflowArtifactsRow[];
}

// =============================================================================
// GOVERNANCEROLES TABLE
// Table: GovernanceRoles. NTWF governance names two distinct ontology-governance roles: a Steward (responsible for the ontology's health — monitors drift, tracks external dependency updates, fields user questions, maintains docs, keeps the validation suite current; identifies that a change is needed but has no approval power) and an Authority (the power to approve changes to the CBox, ABox, and TBox; decides how and where a change is made; sits with the function that owns the domain). 'A steward who can make TBox or ABox changes without authority review is a single point of failure.' For an organization under 500 people a single person may hold both roles. This table models the maintenance discipline itself, as data, so the change log can attribute approvals to a named authority.
// =============================================================================

/** A row in the GovernanceRoles table. */
export interface GovernanceRolesRow {
  governance_role_id: string;
  /** Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value. */
  iri: string | null;
  /** Slug form of the display name. */
  name: string | null;
  /** Human-readable name of the governance role (e.g. 'Steward', 'Authority'). */
  display_name: string | null;
  /** Which of the two NTWF governance kinds this is: 'Steward' or 'Authority'. */
  kind: string | null;
  /** What this role is responsible for. Steward: monitor drift, track external dependency updates, field user questions, maintain documentation, keep the validation suite current. Authority: approve changes to CBox/ABox/TBox; decide how and where a change is made. */
  responsibilities: string | null;
  /** The boxes this role may approve changes to (CBox/ABox/TBox), or 'none' for a Steward — who can identify that a change is needed but cannot approve it. */
  approval_scope: string | null;
  /** The person or function holding this role. The steward is naturally whoever owns the engineering knowledge infrastructure; authority sits with the workflow governance function that owns the modeled domain. Under 500 people, one person may hold both. */
  held_by: string | null;
  /** TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure. */
  can_approve_changes: boolean | null;
  /** Back-reference to ChangeLog entries this governance role approved. Inverse of ChangeLog.ApprovedBy. */
  approved_changes: string | null;
  _erb_errors?: Record<string, string>;
}

const governanceRolesFieldTypes: Record<string, FieldType> = {
  governance_role_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "*string",
  kind: "*string",
  responsibilities: "*string",
  approval_scope: "*string",
  held_by: "*string",
  can_approve_changes: "*bool",
  approved_changes: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key.
 *  Formula: ="governance-roles/" & {{GovernanceRoleId}} */
export function calcGovernanceRolesRelativePath(tc: GovernanceRolesRow): string | null {
  return toStringPtr(erbConcat(vS("governance-roles/"), erbTextOr(vStrPlain(tc.governance_role_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcGovernanceRolesIri(tc: GovernanceRolesRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Slug form of the display name.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcGovernanceRolesName(tc: GovernanceRolesRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStr(tc.display_name)), vS(" "), vS("-")));
}

/** Computes the CanApproveChanges calculated field.
 *  TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure.
 *  Formula: ={{Kind}} = "Authority" */
export function calcGovernanceRolesCanApproveChanges(tc: GovernanceRolesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.kind)), vS("Authority")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeGovernanceRoles(tc: GovernanceRolesRow): GovernanceRolesRow {
  // Level 1
  calcGuard(tc, governanceRolesFieldTypes, "relative_path", () => { tc.relative_path = calcGovernanceRolesRelativePath(tc); });
  calcGuard(tc, governanceRolesFieldTypes, "name", () => { tc.name = calcGovernanceRolesName(tc); });
  calcGuard(tc, governanceRolesFieldTypes, "can_approve_changes", () => { tc.can_approve_changes = calcGovernanceRolesCanApproveChanges(tc); });
  // Level 2
  calcGuard(tc, governanceRolesFieldTypes, "iri", () => { tc.iri = calcGovernanceRolesIri(tc); });
  return tc;
}

/** Reads GovernanceRoles rows from a JSON array file. */
export function loadGovernanceRolesRows(file: string): GovernanceRolesRow[] {
  return loadRows(file, { fields: governanceRolesFieldTypes }) as unknown as GovernanceRolesRow[];
}

// =============================================================================
// CHANGELOG TABLE
// Table: ChangeLog. NTWF's minimum governance artifact: 'a change log that records every TBox and ABox modification, with its rationale.' Each entry records the four facts NTWF governance enumerates — the competency question that motivated the change, the terms affected, the version number of the release, and the date — plus the rationale and the Authority who approved it. Semantic-versioning discipline (MAJOR.MINOR.PATCH) is captured per entry via ChangeKind.
// =============================================================================

/** A row in the ChangeLog table. */
export interface ChangeLogRow {
  change_log_id: string;
  /** Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). */
  iri: string | null;
  /** Human-readable label: the version and date of this change. */
  name: string | null;
  /** The release version number this change shipped in (semantic versioning MAJOR.MINOR.PATCH). NTWF is currently at 1.1.0. */
  version: string | null;
  /** The date of the change. One of the four facts NTWF governance requires every change-log entry to record. */
  change_date: string | null;
  /** Semantic-versioning class of the change: 'patch' (documentation/label/comment only, formal model unchanged), 'minor' (additive — new classes/properties/CBox concepts, backward compatible), or 'major' (breaking — class removed/renamed, domain/range change invalidating ABox triples, or a new disjointness axiom). */
  change_kind: string | null;
  /** The competency question that motivated the change. One of the four facts NTWF governance requires. Empty if the change was driven by an external-dependency update rather than a CQ. */
  motivating_question: string | null;
  /** The ontology terms (classes/properties/concepts) the change added, removed, or modified. One of the four facts NTWF governance requires. */
  terms_affected: string | null;
  /** Why the change was made. NTWF governance requires every TBox/ABox modification to be logged with its rationale. */
  rationale: string | null;
  /** FK to the GovernanceRole (an Authority) that approved this change. Changes to CBox/ABox/TBox require authority review; a steward identifying a need is not enough. */
  approved_by: string | null;
  /** TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version. */
  is_breaking_change: boolean | null;
  /** TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it. */
  is_backward_compatible: boolean | null;
  _erb_errors?: Record<string, string>;
}

const changeLogFieldTypes: Record<string, FieldType> = {
  change_log_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  version: "*string",
  change_date: "*string",
  change_kind: "*string",
  motivating_question: "*string",
  terms_affected: "*string",
  rationale: "*string",
  approved_by: "*string",
  is_breaking_change: "*bool",
  is_backward_compatible: "*bool",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key.
 *  Formula: ="change-log/" & {{ChangeLogId}} */
export function calcChangeLogRelativePath(tc: ChangeLogRow): string | null {
  return toStringPtr(erbConcat(vS("change-log/"), erbTextOr(vStrPlain(tc.change_log_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath).
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcChangeLogIri(tc: ChangeLogRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Human-readable label: the version and date of this change.
 *  Formula: ={{Version}} & " (" & {{ChangeDate}} & ")" */
export function calcChangeLogName(tc: ChangeLogRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.version)), vS(" ("), erbTextOr(vStr(tc.change_date)), vS(")")));
}

/** Computes the IsBreakingChange calculated field.
 *  TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version.
 *  Formula: ={{ChangeKind}} = "major" */
export function calcChangeLogIsBreakingChange(tc: ChangeLogRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.change_kind)), vS("major")));
}

/** Computes the IsBackwardCompatible calculated field.
 *  TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
 *  Formula: =OR({{ChangeKind}} = "patch", {{ChangeKind}} = "minor") */
export function calcChangeLogIsBackwardCompatible(tc: ChangeLogRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.change_kind)), vS("patch"))), erbBool3(erbEq(erbNullif(vStr(tc.change_kind)), vS("minor")))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeChangeLog(tc: ChangeLogRow): ChangeLogRow {
  // Level 1
  calcGuard(tc, changeLogFieldTypes, "relative_path", () => { tc.relative_path = calcChangeLogRelativePath(tc); });
  calcGuard(tc, changeLogFieldTypes, "name", () => { tc.name = calcChangeLogName(tc); });
  calcGuard(tc, changeLogFieldTypes, "is_breaking_change", () => { tc.is_breaking_change = calcChangeLogIsBreakingChange(tc); });
  calcGuard(tc, changeLogFieldTypes, "is_backward_compatible", () => { tc.is_backward_compatible = calcChangeLogIsBackwardCompatible(tc); });
  // Level 2
  calcGuard(tc, changeLogFieldTypes, "iri", () => { tc.iri = calcChangeLogIri(tc); });
  return tc;
}

/** Reads ChangeLog rows from a JSON array file. */
export function loadChangeLogRows(file: string): ChangeLogRow[] {
  return loadRows(file, { fields: changeLogFieldTypes }) as unknown as ChangeLogRow[];
}

// =============================================================================
// VOCABULARYRECONCILIATIONS TABLE
// Table: VocabularyReconciliations. External dependency change: when a borrowed term from a living standard (PROV-O, FOAF, Dublin Core, DCAT, Schema.org) is deprecated and re-homed into the NTWF namespace, the edit triggers a version bump and an owl:sameAs reconciliation relation declaring the old and new terms equivalent. The worked example: deprecating foaf:name, prepending the ntwf prefix to get ntwf:name, and asserting foaf:name owl:sameAs ntwf:name. Each row is one reconciliation, with the standard it came from and the version in which the reconciliation shipped.
// =============================================================================

/** A row in the VocabularyReconciliations table. */
export interface VocabularyReconciliationsRow {
  reconciliation_id: string;
  /** Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). */
  iri: string | null;
  /** Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement. */
  name: string | null;
  /** The borrowed/deprecated term being reconciled (e.g. foaf:name). */
  deprecated_term: string | null;
  /** The NTWF-namespaced replacement term (e.g. ntwf:name). */
  replacement_term: string | null;
  /** The OWL relation asserting equivalence. NTWF uses owl:sameAs. */
  reconciliation_relation: string | null;
  /** The external standard the deprecated term came from (PROV-O, FOAF, Dublin Core, DCAT, Schema.org). */
  source_standard: string | null;
  /** The NTWF release version in which this reconciliation shipped. Re-homing a term triggers a version bump. */
  introduced_in_version: string | null;
  /** Why the term was re-homed (e.g. upstream deprecation; semantic-alignment shift). */
  rationale: string | null;
  _erb_errors?: Record<string, string>;
}

const vocabularyReconciliationsFieldTypes: Record<string, FieldType> = {
  reconciliation_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  deprecated_term: "*string",
  replacement_term: "*string",
  reconciliation_relation: "*string",
  source_standard: "*string",
  introduced_in_version: "*string",
  rationale: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key.
 *  Formula: ="reconciliations/" & {{ReconciliationId}} */
export function calcVocabularyReconciliationsRelativePath(tc: VocabularyReconciliationsRow): string | null {
  return toStringPtr(erbConcat(vS("reconciliations/"), erbTextOr(vStrPlain(tc.reconciliation_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath).
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcVocabularyReconciliationsIri(tc: VocabularyReconciliationsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement.
 *  Formula: ={{DeprecatedTerm}} & " owl:sameAs " & {{ReplacementTerm}} */
export function calcVocabularyReconciliationsName(tc: VocabularyReconciliationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.deprecated_term)), vS(" owl:sameAs "), erbTextOr(vStr(tc.replacement_term))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeVocabularyReconciliations(tc: VocabularyReconciliationsRow): VocabularyReconciliationsRow {
  // Level 1
  calcGuard(tc, vocabularyReconciliationsFieldTypes, "relative_path", () => { tc.relative_path = calcVocabularyReconciliationsRelativePath(tc); });
  calcGuard(tc, vocabularyReconciliationsFieldTypes, "name", () => { tc.name = calcVocabularyReconciliationsName(tc); });
  // Level 2
  calcGuard(tc, vocabularyReconciliationsFieldTypes, "iri", () => { tc.iri = calcVocabularyReconciliationsIri(tc); });
  return tc;
}

/** Reads VocabularyReconciliations rows from a JSON array file. */
export function loadVocabularyReconciliationsRows(file: string): VocabularyReconciliationsRow[] {
  return loadRows(file, { fields: vocabularyReconciliationsFieldTypes }) as unknown as VocabularyReconciliationsRow[];
}

// =============================================================================
// SCENARIOS TABLE
// =============================================================================

/** A row in the Scenarios table. */
export interface ScenariosRow {
  /** Stable identifier for a curated demo scenario (a named set of raw-fact edits applied at once). */
  scenario_id: string;
  /** DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key. */
  relative_path: string | null;
  /** Opaque stable identifier (dash-form of RelativePath). */
  iri: string | null;
  /** Slug form of the human label. */
  name: string | null;
  /** Human-readable button label for this scenario in the picker. */
  label: string;
  /** A single emoji shown beside the label in the picker. */
  icon: string | null;
  /** Plain-language description of what this scenario changes and what the reasoner will derive as a result. Shown in the floating scenario picker so the user knows what each preset does before applying it. */
  explanation: string | null;
  /** Display order in the picker (ascending). */
  sort_order: number | null;
  /** True for the single 'restore baseline' scenario; the picker styles it as a secondary action. */
  is_reset: boolean | null;
  /** JSON-encoded ordered list of raw-fact assignments this scenario applies. Each item is {class, id|match, set:{field:value,...}} where 'class' is a rulebook table (camelCase keys on the raw store), 'id' targets a row by its *Id primary key (or 'match':'first' for the singleton Workflow), and 'set' is the raw fields to assign. The backend replays this list against the active raw store, then re-reasons — the scenario NEVER sets a derived field. This is the single source of truth for what each demo scenario does; the app's picker and the apply endpoint both read it from here (same JSON-on-a-first-class-table pattern as __meta__'s JsonValue). */
  edits: string;
  _erb_errors?: Record<string, string>;
}

const scenariosFieldTypes: Record<string, FieldType> = {
  scenario_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  label: "string",
  icon: "*string",
  explanation: "*string",
  sort_order: "*int",
  is_reset: "*bool",
  edits: "string",
};

/** Computes the RelativePath calculated field.
 *  DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key.
 *  Formula: ="scenarios/" & {{ScenarioId}} */
export function calcScenariosRelativePath(tc: ScenariosRow): string | null {
  return toStringPtr(erbConcat(vS("scenarios/"), erbTextOr(vStrPlain(tc.scenario_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (dash-form of RelativePath).
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcScenariosIri(tc: ScenariosRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Slug form of the human label.
 *  Formula: =SUBSTITUTE(LOWER({{Label}}), " ", "-") */
export function calcScenariosName(tc: ScenariosRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.label)), vS(" "), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeScenarios(tc: ScenariosRow): ScenariosRow {
  // Level 1
  calcGuard(tc, scenariosFieldTypes, "relative_path", () => { tc.relative_path = calcScenariosRelativePath(tc); });
  calcGuard(tc, scenariosFieldTypes, "name", () => { tc.name = calcScenariosName(tc); });
  // Level 2
  calcGuard(tc, scenariosFieldTypes, "iri", () => { tc.iri = calcScenariosIri(tc); });
  return tc;
}

/** Reads Scenarios rows from a JSON array file. */
export function loadScenariosRows(file: string): ScenariosRow[] {
  return loadRows(file, { fields: scenariosFieldTypes }) as unknown as ScenariosRow[];
}

// =============================================================================
// COMPETENCYQUESTIONS TABLE
// The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook.
// =============================================================================

/** A row in the CompetencyQuestions table. */
export interface CompetencyQuestionsRow {
  /** Primary key. Stable slug for the competency question (cq-1 .. cq-8). */
  competency_question_id: string;
  /** Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. */
  relative_path: string | null;
  /** Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique. */
  iri: string | null;
  /** Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables. */
  name: string | null;
  /** The canonical 1-8 ordering of the competency questions as listed in the article / README. */
  number: number;
  /** Short human label for the question (e.g. 'Steps and order'). */
  display_name: string;
  /** The full competency question, verbatim from the article's acceptance suite. */
  question_text: string;
  /** The entity whose computed field answers this question. Together with TargetField it pins the answer to a real column in the substrate, so the scoreboard reads the answer (never recomputes it) and the explainer-DAG drilldown lands on the exact derivation. */
  target_table: string;
  /** The substrate-computed field on TargetTable that answers this question (calc / lookup / aggregation / closure). The CQ scoreboard wraps the live answer in a DagCell(TargetTable, TargetField) so a click opens its inference graph. */
  target_field: string;
  /** 'scalar' when the answer is a single value graded by equality with ExpectedAnswer; 'list' when the answer is a collection graded as answerable (non-empty / matches the asserted shape). */
  answer_kind: string;
  /** The asserted correct answer for the seed worked example. For scalar questions the live computed value must equal this to score a pass; for list questions this is the canonical summary the rendered collection is checked against. Authored here so pass/fail is (substrate-computed value) vs (rulebook-asserted expectation) — a real conformance check, not UI logic. */
  expected_answer: string;
  /** Name of the boolean column on Workflows that computes whether this CQ is satisfied (e.g. Cq6Satisfied). The scoreboard reads pass/fail straight from this substrate-computed column — the acceptance criterion lives in the rulebook as a derived field, never as app-side logic. Mirrors TargetTable/TargetField for the answer. */
  satisfied_field: string | null;
  /** One-sentence note on how this question resolves through the model — the FK / formula chain a presenter can narrate. */
  explanation: string | null;
  /** Display order in the scoreboard. Mirrors Number for now; kept separate so the list can be re-sequenced without renumbering the canonical CQ ids. */
  sort_order: number | null;
  /** Whether this competency question is shown in the scoreboard. All eight are active in the worked example. */
  is_active: boolean | null;
  /** FK to the Scenario the card's 'Simulate' button applies to demonstrate this competency question live. Points at the minimal raw-fact edit that moves THIS question's answer in isolation where one exists; for cq-2 it points at 'ai-release-manager', which also ripples to cq-3 (the gate approver is itself a step executor, so the two answers cannot be perturbed independently). The full set of questions each scenario moves — trigger vs ripple — is enumerated in the ScenarioCQEffects junction; this is just the one the button fires. Inverse-ish of ScenarioCQEffects but kept as a direct FK so the UI has a single answer. */
  simulate_scenario: string | null;
  _erb_errors?: Record<string, string>;
}

const competencyQuestionsFieldTypes: Record<string, FieldType> = {
  competency_question_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  number: "float64",
  display_name: "string",
  question_text: "string",
  target_table: "string",
  target_field: "string",
  answer_kind: "string",
  expected_answer: "string",
  satisfied_field: "*string",
  explanation: "*string",
  sort_order: "*float64",
  is_active: "*bool",
  simulate_scenario: "*string",
};

/** Computes the RelativePath calculated field.
 *  Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution.
 *  Formula: ="competency-questions/" & {{CompetencyQuestionId}} */
export function calcCompetencyQuestionsRelativePath(tc: CompetencyQuestionsRow): string | null {
  return toStringPtr(erbConcat(vS("competency-questions/"), erbTextOr(vStrPlain(tc.competency_question_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcCompetencyQuestionsIri(tc: CompetencyQuestionsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcCompetencyQuestionsName(tc: CompetencyQuestionsRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.display_name)), vS(" "), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeCompetencyQuestions(tc: CompetencyQuestionsRow): CompetencyQuestionsRow {
  // Level 1
  calcGuard(tc, competencyQuestionsFieldTypes, "relative_path", () => { tc.relative_path = calcCompetencyQuestionsRelativePath(tc); });
  calcGuard(tc, competencyQuestionsFieldTypes, "name", () => { tc.name = calcCompetencyQuestionsName(tc); });
  // Level 2
  calcGuard(tc, competencyQuestionsFieldTypes, "iri", () => { tc.iri = calcCompetencyQuestionsIri(tc); });
  return tc;
}

/** Reads CompetencyQuestions rows from a JSON array file. */
export function loadCompetencyQuestionsRows(file: string): CompetencyQuestionsRow[] {
  return loadRows(file, { fields: competencyQuestionsFieldTypes }) as unknown as CompetencyQuestionsRow[];
}

// =============================================================================
// SCENARIOCQEFFECTS TABLE
// Table: ScenarioCQEffects. Names the many-to-many between Scenarios and CompetencyQuestions as two 1:M foreign keys (Scenario, CompetencyQuestion) plus the detail of the relationship (EffectKind, Note). Each row asserts 'applying this scenario moves this competency question's live answer'. 'trigger' rows are the intended demonstration; 'ripple' rows record answers that move as an unavoidable consequence of the same raw edit (e.g. ai-release-manager moves cq-2 AND cq-3 because the gate approver is itself a step executor). The answers themselves are never stored here — they are read live from each substrate after the scenario applies.
// =============================================================================

/** A row in the ScenarioCQEffects table. */
export interface ScenarioCQEffectsRow {
  /** Primary key. '<scenario>-<cq>' — names one (scenario moves this competency question) edge. */
  scenario_cq_effect_id: string;
  /** DAG-derived location: 'scenario-cq-effects/' + the row's primary key. */
  relative_path: string | null;
  /** Opaque stable identifier (dash-form of RelativePath). */
  iri: string | null;
  /** Slug label, mirrors the primary key. */
  name: string | null;
  /** FK to the Scenario whose raw-fact edits cause this effect. The 'many effects belong to one scenario' side: a single scenario can move several competency questions. */
  scenario: string;
  /** FK to the CompetencyQuestion whose live answer this scenario moves. The 'many effects belong to one question' side: a question can be exercised by several scenarios. */
  competency_question: string;
  /** 'trigger' = this scenario was authored to move this question (the point of the demo). 'ripple' = the question also moves as an unavoidable side effect of the same raw edit. The ripple rows are the pedagogical payload: they show answers that are structurally coupled and cannot be perturbed independently. */
  effect_kind: string;
  /** One-line, human-readable account of how this scenario moves this question's answer (qualitative — the actual value is read live from the substrate, never stored here). */
  note: string | null;
  /** Display order. */
  sort_order: number | null;
  _erb_errors?: Record<string, string>;
}

const scenarioCQEffectsFieldTypes: Record<string, FieldType> = {
  scenario_cq_effect_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  scenario: "string",
  competency_question: "string",
  effect_kind: "string",
  note: "*string",
  sort_order: "*int",
};

/** Computes the RelativePath calculated field.
 *  DAG-derived location: 'scenario-cq-effects/' + the row's primary key.
 *  Formula: ="scenario-cq-effects/" & {{ScenarioCQEffectId}} */
export function calcScenarioCQEffectsRelativePath(tc: ScenarioCQEffectsRow): string | null {
  return toStringPtr(erbConcat(vS("scenario-cq-effects/"), erbTextOr(vStrPlain(tc.scenario_cq_effect_id))));
}

/** Computes the Iri calculated field.
 *  Opaque stable identifier (dash-form of RelativePath).
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcScenarioCQEffectsIri(tc: ScenarioCQEffectsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Slug label, mirrors the primary key.
 *  Formula: =SUBSTITUTE(LOWER({{ScenarioCQEffectId}}), " ", "-") */
export function calcScenarioCQEffectsName(tc: ScenarioCQEffectsRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.scenario_cq_effect_id)), vS(" "), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeScenarioCQEffects(tc: ScenarioCQEffectsRow): ScenarioCQEffectsRow {
  // Level 1
  calcGuard(tc, scenarioCQEffectsFieldTypes, "relative_path", () => { tc.relative_path = calcScenarioCQEffectsRelativePath(tc); });
  calcGuard(tc, scenarioCQEffectsFieldTypes, "name", () => { tc.name = calcScenarioCQEffectsName(tc); });
  // Level 2
  calcGuard(tc, scenarioCQEffectsFieldTypes, "iri", () => { tc.iri = calcScenarioCQEffectsIri(tc); });
  return tc;
}

/** Reads ScenarioCQEffects rows from a JSON array file. */
export function loadScenarioCQEffectsRows(file: string): ScenarioCQEffectsRow[] {
  return loadRows(file, { fields: scenarioCQEffectsFieldTypes }) as unknown as ScenarioCQEffectsRow[];
}

// =============================================================================
// CONFORMANCETESTS TABLE
// =============================================================================

/** A row in the ConformanceTests table. */
export interface ConformanceTestsRow {
  /** Stable identifier for one conformance test — one assertion the harness runs against every execution substrate. */
  conformance_test_id: string;
  /** DAG-derived location for this test row: root segment 'conformance-tests' + the primary key. */
  relative_path: string | null;
  /** Slug IRI for this row, derived from RelativePath. */
  iri: string | null;
  /** Machine name derived from the display name. */
  name: string | null;
  /** Human-readable test title shown in the admin console and run logs. */
  display_name: string;
  /** Comma-separated FEATURE-COVERAGE.md ids this test witnesses (e.g. 'II-3,CQ2'). The traceability link from the article's feature inventory to an executable assertion. */
  feature_ref: string | null;
  /** Grouping for display: 'Sweep', 'Part I'..'Part IV', 'Closure', 'Mutation'. */
  section: string;
  /** How the harness executes this test. 'sweep' = every row+field of TargetRef table vs the answer key; 'field-match' = one row's field vs the answer key; 'closure-contains' = a from→to pair must appear in the engine's computed transitive closure (Expect names the closure and pair); 'engines-agree' = zero value-class disagreements between the two engines; 'mutation' = apply Expect.edits to an in-memory copy of the seed facts, re-reason, and check Expect.assert — the store is NEVER written. */
  test_kind: string;
  /** What the test reads: 'Entity' (whole table), 'Entity/pk' (one row) or 'Entity/pk#Field' (one value). Slash/hash form on purpose — these are spec references, not foreign keys. */
  target_ref: string | null;
  /** Kind-specific JSON spec. Empty for sweep/field-match/engines-agree — there the ORACLE is the answer key (testing/answer-keys), never a value hardcoded here (a literal would go stale; the key regenerates). closure-contains: {closure, from, to}. mutation: {edits:[{class,id,set:{camelRawField:value}}], assert:[{class,id,field,equals}]} — same edits shape as Scenarios.Edits. */
  expect: string | null;
  /** Why this test exists — what feature of the model it flexes, in one sentence. */
  explanation: string | null;
  /** Display/run order within the suite. */
  sort_order: number;
  /** Disabled tests are listed but not executed (parked, not deleted — the list stays the complete spec). */
  is_enabled: boolean;
  _erb_errors?: Record<string, string>;
}

const conformanceTestsFieldTypes: Record<string, FieldType> = {
  conformance_test_id: "string",
  relative_path: "*string",
  iri: "*string",
  name: "*string",
  display_name: "string",
  feature_ref: "*string",
  section: "string",
  test_kind: "string",
  target_ref: "*string",
  expect: "*string",
  explanation: "*string",
  sort_order: "int",
  is_enabled: "bool",
};

/** Computes the RelativePath calculated field.
 *  DAG-derived location for this test row: root segment 'conformance-tests' + the primary key.
 *  Formula: ="conformance-tests/" & {{ConformanceTestId}} */
export function calcConformanceTestsRelativePath(tc: ConformanceTestsRow): string | null {
  return toStringPtr(erbConcat(vS("conformance-tests/"), erbTextOr(vStrPlain(tc.conformance_test_id))));
}

/** Computes the Iri calculated field.
 *  Slug IRI for this row, derived from RelativePath.
 *  Formula: =SUBSTITUTE({{RelativePath}}, "/", "-") */
export function calcConformanceTestsIri(tc: ConformanceTestsRow): string | null {
  return toStringPtr(erbSubstitute(vStr(tc.relative_path), vS("/"), vS("-")));
}

/** Computes the Name calculated field.
 *  Machine name derived from the display name.
 *  Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-") */
export function calcConformanceTestsName(tc: ConformanceTestsRow): string | null {
  return toStringPtr(erbSubstitute(erbLower(vStrPlain(tc.display_name)), vS(" "), vS("-")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeConformanceTests(tc: ConformanceTestsRow): ConformanceTestsRow {
  // Level 1
  calcGuard(tc, conformanceTestsFieldTypes, "relative_path", () => { tc.relative_path = calcConformanceTestsRelativePath(tc); });
  calcGuard(tc, conformanceTestsFieldTypes, "name", () => { tc.name = calcConformanceTestsName(tc); });
  // Level 2
  calcGuard(tc, conformanceTestsFieldTypes, "iri", () => { tc.iri = calcConformanceTestsIri(tc); });
  return tc;
}

/** Reads ConformanceTests rows from a JSON array file. */
export function loadConformanceTestsRows(file: string): ConformanceTestsRow[] {
  return loadRows(file, { fields: conformanceTestsFieldTypes }) as unknown as ConformanceTestsRow[];
}

// =============================================================================
// __META__ TABLE
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
// =============================================================================

/** A row in the __meta__ table. */
export interface __meta__Row {
  /** The metadata key (e.g. 'tagline', 'motif_palette', 'substrates'). Unique within the table. */
  meta_key: string;
  /** Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table. */
  name: string | null;
  /** How to interpret the value columns: 'string' (use StringValue), 'object' (parse JsonValue as JSON object), 'array' (parse JsonValue as JSON array). */
  value_type: string;
  /** Plain string value. Populated when ValueType == 'string'; null otherwise. */
  string_value: string | null;
  /** JSON-encoded value. Populated when ValueType == 'object' or 'array'; null when ValueType == 'string'. */
  json_value: string | null;
  _erb_errors?: Record<string, string>;
}

const __meta__FieldTypes: Record<string, FieldType> = {
  meta_key: "string",
  name: "*string",
  value_type: "string",
  string_value: "*string",
  json_value: "*string",
};

/** Computes the Name calculated field.
 *  Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
 *  Formula: ={{MetaKey}} */
export function calc__meta__Name(tc: __meta__Row): string | null {
  return toStringPtr(vStrPlain(tc.meta_key));
}

/** Computes every calculated field of the row in dependency order. */
export function compute__meta__(tc: __meta__Row): __meta__Row {
  // Level 1
  calcGuard(tc, __meta__FieldTypes, "name", () => { tc.name = calc__meta__Name(tc); });
  return tc;
}

/** Reads __meta__ rows from a JSON array file. */
export function load__meta__Rows(file: string): __meta__Row[] {
  return loadRows(file, { fields: __meta__FieldTypes }) as unknown as __meta__Row[];
}

/** Bounds the runner's passes over the dataset. */
export const calculatedFieldCount = 102;

/** Every table, in rulebook order. */
export const erbTables: TableSpec[] = [
  { name: "Workflows", file: "workflows", rulebookRows: 1, fields: workflowsFieldTypes,
    compute: (row: any) => computeWorkflows(row as WorkflowsRow),
    lookups: [],
    aggregations: [
      { field: "count_of_non_proposed_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }] },
      { field: "count_ai_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "is_executed_by_ai", kind: "literal", literal: vB(true) }] },
      { field: "count_human_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "is_executed_by_human", kind: "literal", literal: vB(true) }] },
      { field: "count_human_required_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "requires_human_approval", kind: "literal", literal: vB(true) }] },
      { field: "count_approval_consistency_violations", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "approval_consistency_violation", kind: "literal", literal: vB(true) }] },
      { field: "count_derivation_links", op: "COUNTIFS", table: "workflow_artifacts", criteria: [{ range: "produced_by_workflow", kind: "field", field: "workflow_id" }, { range: "has_derivation_parent", kind: "literal", literal: vB(true) }] },
      { field: "count_legal_owned_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "is_legal_owned", kind: "literal", literal: vB(true) }] },
      { field: "count_engineering_owned_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "is_engineering_owned", kind: "literal", literal: vB(true) }] },
      { field: "count_inferred_precedence_pairs", op: "COUNTIFS", table: "vw_step_precedence_closure", criteria: [{ range: "is_inferred", kind: "literal", literal: vB(true) }] },
      { field: "count_asserted_precedence_pairs", op: "COUNTIFS", table: "vw_step_precedence_closure", criteria: [{ range: "is_inferred", kind: "literal", literal: vB(false) }] },
      { field: "count_roles_with_bad_filler_cardinality", op: "COUNTIFS", table: "roles", criteria: [{ range: "has_exactly_one_filler", kind: "literal", literal: vB(false) }] },
      { field: "count_agent_type_changes", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "is_agent_type_change", kind: "literal", literal: vB(true) }] },
      { field: "count_compliance_audit_changes", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "requires_compliance_audit", kind: "literal", literal: vB(true) }] },
      { field: "count_approval_gate_steps", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "workflow", kind: "field", field: "workflow_id" }, { range: "is_approval_gate", kind: "literal", literal: vB(true) }] },
      { field: "count_gates_without_human_approver", op: "COUNTIFS", table: "approval_gates", criteria: [{ range: "has_human_approver", kind: "literal", literal: vB(false) }] },
      { field: "count_workflow_artifacts", op: "COUNTIFS", table: "workflow_artifacts", criteria: [{ range: "produced_by_workflow", kind: "field", field: "workflow_id" }] },
      { field: "count_roles_with_escalation_violation", op: "COUNTIFS", table: "roles", criteria: [{ range: "escalation_violation", kind: "literal", literal: vB(true) }] },
      { field: "count_unconsumed_datasets", op: "COUNTIFS", table: "datasets", criteria: [{ range: "is_consumed", kind: "literal", literal: vB(false) }] },] },
  { name: "WorkflowSteps", file: "workflow_steps", rulebookRows: 5, fields: workflowStepsFieldTypes,
    compute: (row: any) => computeWorkflowSteps(row as WorkflowStepsRow),
    lookups: [
      { field: "parent_path", target: "workflows", ret: "relative_path", key: "workflow", match: "workflow_id" },
      { field: "executing_human_agent", target: "roles", ret: "filled_by_human_agent", key: "assigned_role", match: "role_id" },
      { field: "executing_ai_agent", target: "roles", ret: "filled_by_ai_agent", key: "assigned_role", match: "role_id" },
      { field: "executing_automated_pipeline", target: "roles", ret: "filled_by_automated_pipeline", key: "assigned_role", match: "role_id" },
      { field: "owning_department", target: "roles", ret: "owned_by", key: "assigned_role", match: "role_id" },],
    aggregations: [
      { field: "preceding_step_count", op: "COUNTIFS", table: "vw_step_precedence_closure", criteria: [{ range: "to_id", kind: "field", field: "workflow_step_id" }] },] },
  { name: "ApprovalGates", file: "approval_gates", rulebookRows: 1, fields: approvalGatesFieldTypes,
    compute: (row: any) => computeApprovalGates(row as ApprovalGatesRow),
    lookups: [
      { field: "parent_path", target: "workflow_steps", ret: "relative_path", key: "workflow_step", match: "workflow_step_id" },
      { field: "gate_role", target: "workflow_steps", ret: "assigned_role", key: "workflow_step", match: "workflow_step_id" },
      { field: "gate_approver_human", target: "roles", ret: "filled_by_human_agent", key: "gate_role", match: "role_id" },],
    aggregations: [] },
  { name: "StepPrecedence", file: "step_precedence", rulebookRows: 4, fields: stepPrecedenceFieldTypes,
    compute: (row: any) => computeStepPrecedence(row as StepPrecedenceRow),
    lookups: [
      { field: "parent_path", target: "workflow_steps", ret: "relative_path", key: "from_step", match: "workflow_step_id" },],
    aggregations: [] },
  { name: "Roles", file: "roles", rulebookRows: 7, fields: rolesFieldTypes,
    compute: (row: any) => computeRoles(row as RolesRow),
    lookups: [],
    aggregations: [
      { field: "fills_approval_gate", op: "COUNTIFS", table: "workflow_steps", criteria: [{ range: "assigned_role", kind: "field", field: "role_id" }, { range: "is_approval_gate", kind: "literal", literal: vB(true) }] },] },
  { name: "RoleAssignments", file: "role_assignments", rulebookRows: 6, fields: roleAssignmentsFieldTypes,
    compute: (row: any) => computeRoleAssignments(row as RoleAssignmentsRow),
    lookups: [
      { field: "parent_path", target: "roles", ret: "relative_path", key: "role", match: "role_id" },],
    aggregations: [] },
  { name: "Departments", file: "departments", rulebookRows: 2, fields: departmentsFieldTypes,
    compute: (row: any) => computeDepartments(row as DepartmentsRow),
    lookups: [],
    aggregations: [] },
  { name: "HumanAgents", file: "human_agents", rulebookRows: 5, fields: humanAgentsFieldTypes,
    compute: (row: any) => computeHumanAgents(row as HumanAgentsRow),
    lookups: [],
    aggregations: [] },
  { name: "AIAgents", file: "ai_agents", rulebookRows: 2, fields: aIAgentsFieldTypes,
    compute: (row: any) => computeAIAgents(row as AIAgentsRow),
    lookups: [],
    aggregations: [
      { field: "count_attributed_artifacts", op: "COUNTIFS", table: "workflow_artifacts", criteria: [{ range: "attributed_to_ai_agent", kind: "field", field: "ai_agent_id" }] },
      { field: "count_impacted_workflows", op: "COUNTIFS", table: "workflow_artifacts", criteria: [{ range: "attributed_to_ai_agent", kind: "field", field: "ai_agent_id" }, { range: "has_producing_workflow", kind: "literal", literal: vB(true) }] },] },
  { name: "AutomatedPipelines", file: "automated_pipelines", rulebookRows: 1, fields: automatedPipelinesFieldTypes,
    compute: (row: any) => computeAutomatedPipelines(row as AutomatedPipelinesRow),
    lookups: [],
    aggregations: [] },
  { name: "WorkflowStatusConcepts", file: "workflow_status_concepts", rulebookRows: 4, fields: workflowStatusConceptsFieldTypes,
    compute: (row: any) => computeWorkflowStatusConcepts(row as WorkflowStatusConceptsRow),
    lookups: [],
    aggregations: [] },
  { name: "AgentCapabilityConcepts", file: "agent_capability_concepts", rulebookRows: 6, fields: agentCapabilityConceptsFieldTypes,
    compute: (row: any) => computeAgentCapabilityConcepts(row as AgentCapabilityConceptsRow),
    lookups: [],
    aggregations: [] },
  { name: "ArtifactTypeConcepts", file: "artifact_type_concepts", rulebookRows: 3, fields: artifactTypeConceptsFieldTypes,
    compute: (row: any) => computeArtifactTypeConcepts(row as ArtifactTypeConceptsRow),
    lookups: [],
    aggregations: [] },
  { name: "Datasets", file: "datasets", rulebookRows: 1, fields: datasetsFieldTypes,
    compute: (row: any) => computeDatasets(row as DatasetsRow),
    lookups: [],
    aggregations: [] },
  { name: "WorkflowArtifacts", file: "workflow_artifacts", rulebookRows: 5, fields: workflowArtifactsFieldTypes,
    compute: (row: any) => computeWorkflowArtifacts(row as WorkflowArtifactsRow),
    lookups: [
      { field: "parent_path", target: "workflow_steps", ret: "relative_path", key: "produced_by_step", match: "workflow_step_id" },
      { field: "produced_by_workflow", target: "workflow_steps", ret: "workflow", key: "produced_by_step", match: "workflow_step_id" },],
    aggregations: [] },
  { name: "GovernanceRoles", file: "governance_roles", rulebookRows: 2, fields: governanceRolesFieldTypes,
    compute: (row: any) => computeGovernanceRoles(row as GovernanceRolesRow),
    lookups: [],
    aggregations: [] },
  { name: "ChangeLog", file: "change_log", rulebookRows: 2, fields: changeLogFieldTypes,
    compute: (row: any) => computeChangeLog(row as ChangeLogRow),
    lookups: [],
    aggregations: [] },
  { name: "VocabularyReconciliations", file: "vocabulary_reconciliations", rulebookRows: 2, fields: vocabularyReconciliationsFieldTypes,
    compute: (row: any) => computeVocabularyReconciliations(row as VocabularyReconciliationsRow),
    lookups: [],
    aggregations: [] },
  { name: "Scenarios", file: "scenarios", rulebookRows: 12, fields: scenariosFieldTypes,
    compute: (row: any) => computeScenarios(row as ScenariosRow),
    lookups: [],
    aggregations: [] },
  { name: "CompetencyQuestions", file: "competency_questions", rulebookRows: 8, fields: competencyQuestionsFieldTypes,
    compute: (row: any) => computeCompetencyQuestions(row as CompetencyQuestionsRow),
    lookups: [],
    aggregations: [] },
  { name: "ScenarioCQEffects", file: "scenario_cq_effects", rulebookRows: 12, fields: scenarioCQEffectsFieldTypes,
    compute: (row: any) => computeScenarioCQEffects(row as ScenarioCQEffectsRow),
    lookups: [],
    aggregations: [] },
  { name: "ConformanceTests", file: "conformance_tests", rulebookRows: 72, fields: conformanceTestsFieldTypes,
    compute: (row: any) => computeConformanceTests(row as ConformanceTestsRow),
    lookups: [],
    aggregations: [] },
  { name: "__meta__", file: "__meta__", rulebookRows: 12, fields: __meta__FieldTypes,
    compute: (row: any) => compute__meta__(row as __meta__Row),
    lookups: [],
    aggregations: [] },
];

/** Materializes each vw_<entity>_closure view aggregations read. */
export const erbClosures: ClosureSpec[] = [
  { view: "vw_roles_closure", source: "roles", from: "role_id", to: "delegates_to" },
  { view: "vw_step_precedence_closure", source: "step_precedence", from: "from_step", to: "to_step" },
  { view: "vw_workflow_artifacts_closure", source: "workflow_artifacts", from: "artifact_id", to: "derived_from_artifact" },
];
