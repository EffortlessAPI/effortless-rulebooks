"""
ERB Calculation Library (GENERATED - DO NOT EDIT)
=================================================
Generated from: effortless-rulebook/talismans-special-solutions-rulebook.json

PYTHON SUBSTRATE ONLY. Importing this module from any other substrate
is cheating. That substrate must execute the rulebook in its own native
semantics (its $ENGINE). The whole point of ERB conformance is that each
substrate computes calculated, lookup, and aggregation fields by
interpreting/compiling the rulebook itself — not by calling out to a
Python simulator. If a substrate cannot natively compute a field, it must
leave it null and accept the 0 score for that field.

This file contains:
  - Generated calc_* functions for calculated (scalar) fields
  - Generated compute_*_fields(record) dispatchers per entity
  - The compute_all_calculated_fields(record, entity_name) entry point
  - Hand-written compute_lookups() and compute_aggregations() — the
    INDEX/MATCH and COUNTIFS/SUMIFS interpreters. These used to live in
    orchestration/shared.py where any substrate could import them, which
    let 7 substrates report 100% without executing anything native. They
    now live inside the Python-only fence by design.
"""

import json
import re
from pathlib import Path
from typing import Optional, Any

from orchestration.shared import (
    to_snake_case,
    get_entity_schema,
    get_lookup_fields,
    get_aggregation_fields,
)


# =============================================================================
# WORKFLOWS CALCULATIONS
# Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
# =============================================================================

# Level 1

def calc_workflows_relative_path(workflow_id):
    """
    Stable, DAG-derived location for this Workflow row. Root segment 'workflows' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="workflows/" & {{WorkflowId}}
    """
    return ('workflows/' + str(workflow_id or ""))

def calc_workflows_name(display_name):
    """
    Short machine-friendly name for the workflow. Used for programmatic reference and URL slug generation.
    
    Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
    """
    return ((((display_name or "").lower()) or "").replace(' ', '-'))


def calc_workflows_months_since_modified():
    """ERROR: Could not parse formula: =DATETIME_DIFF(NOW(), {{Modified}}, "months")
    Error: Unknown function: DATETIME_DIFF
    """
    raise NotImplementedError("Formula parsing failed")


# Level 2

def calc_workflows_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

def calc_workflows_is_stale(months_since_modified, staleness_threshold_months):
    """
    TRUE iff the workflow's compliance documentation is past its review policy — i.e. the review age in months exceeds the policy line: MonthsSinceModified > StalenessThresholdMonths. With the default the docs go stale at 12 months. Staleness fires the instant the review comes due — there is no renewal window or deferral. This is the article's CQ5 condition ('which workflows haven't been reviewed in twelve months') stated directly against the editable policy field.
    
    Formula: ={{MonthsSinceModified}} > {{StalenessThresholdMonths}}
    """
    return (months_since_modified > staleness_threshold_months)

# Level 3

def calc_workflows_cq5_satisfied(is_stale):
    """
    CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
    
    Formula: =NOT({{IsStale}})
    """
    return (is_stale is not True)

# Level 4

def calc_workflows_has_more_than1_step(count_of_non_proposed_steps):
    """Formula: ={{CountOfNonProposedSteps}} > 1"""
    return (count_of_non_proposed_steps > 1)

def calc_workflows_has_consistency_violation(count_approval_consistency_violations):
    """
    TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
    
    Formula: ={{CountApprovalConsistencyViolations}} > 0
    """
    return (count_approval_consistency_violations > 0)

def calc_workflows_has_ai_agent_step(count_ai_steps):
    """
    TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
    
    Formula: ={{CountAISteps}} > 0
    """
    return (count_ai_steps > 0)

def calc_workflows_is_stale_and_has_ai_agent(is_stale, has_ai_agent_step):
    """
    The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
    
    Formula: =AND({{IsStale}}, {{HasAIAgentStep}})
    """
    return ((is_stale is True) and (has_ai_agent_step is True))

def calc_workflows_involves_engineering_and_legal(count_engineering_owned_steps, count_legal_owned_steps):
    """
    TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
    
    Formula: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0)
    """
    return ((count_engineering_owned_steps > 0) and (count_legal_owned_steps > 0))


def calc_workflows_count_of_precedence_closure_pairs():
    """ERROR: Could not parse formula: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}}
    Error: '+'
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflows_cq1_satisfied():
    """ERROR: Could not parse formula: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2
    Error: '-'
    """
    raise NotImplementedError("Formula parsing failed")


def calc_workflows_cq2_satisfied(count_approval_gate_steps, count_gates_without_human_approver):
    """
    CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
    
    Formula: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0)
    """
    return ((count_approval_gate_steps > 0) and (count_gates_without_human_approver == 0))

def calc_workflows_cq3_satisfied(has_consistency_violation):
    """
    CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
    
    Formula: =NOT({{HasConsistencyViolation}})
    """
    return (has_consistency_violation is not True)


def calc_workflows_cq4_satisfied():
    """ERROR: Could not parse formula: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1
    Error: '-'
    """
    raise NotImplementedError("Formula parsing failed")


def calc_workflows_cq6_satisfied(count_roles_with_escalation_violation):
    """
    CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
    
    Formula: ={{CountRolesWithEscalationViolation}} = 0
    """
    return (count_roles_with_escalation_violation == 0)

def calc_workflows_cq7_satisfied(involves_engineering_and_legal):
    """
    CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
    
    Formula: ={{InvolvesEngineeringAndLegal}}
    """
    return involves_engineering_and_legal

def calc_workflows_cq8_satisfied(count_unconsumed_datasets):
    """
    CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
    
    Formula: ={{CountUnconsumedDatasets}} = 0
    """
    return (count_unconsumed_datasets == 0)


def compute_workflows_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Workflows.
    
    Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_workflows_relative_path(result.get('workflow_id'))
    result['name'] = calc_workflows_name(result.get('display_name'))
    result['months_since_modified'] = calc_workflows_months_since_modified(result.get('modified'))

    # Level 2 calculations
    result['iri'] = calc_workflows_iri(result.get('relative_path'))
    result['is_stale'] = calc_workflows_is_stale(result.get('months_since_modified'), result.get('staleness_threshold_months'))

    # Level 3 calculations
    result['cq5_satisfied'] = calc_workflows_cq5_satisfied(result.get('is_stale'))

    # Level 4 calculations
    result['has_more_than1_step'] = calc_workflows_has_more_than1_step(result.get('count_of_non_proposed_steps'))
    result['has_consistency_violation'] = calc_workflows_has_consistency_violation(result.get('count_approval_consistency_violations'))
    result['has_ai_agent_step'] = calc_workflows_has_ai_agent_step(result.get('count_ai_steps'))
    result['is_stale_and_has_ai_agent'] = calc_workflows_is_stale_and_has_ai_agent(result.get('is_stale'), result.get('has_ai_agent_step'))
    result['involves_engineering_and_legal'] = calc_workflows_involves_engineering_and_legal(result.get('count_engineering_owned_steps'), result.get('count_legal_owned_steps'))
    result['count_of_precedence_closure_pairs'] = calc_workflows_count_of_precedence_closure_pairs(result.get('count_asserted_precedence_pairs'), result.get('count_inferred_precedence_pairs'))
    result['cq1_satisfied'] = calc_workflows_cq1_satisfied(result.get('count_of_precedence_closure_pairs'), result.get('count_of_non_proposed_steps'))
    result['cq2_satisfied'] = calc_workflows_cq2_satisfied(result.get('count_approval_gate_steps'), result.get('count_gates_without_human_approver'))
    result['cq3_satisfied'] = calc_workflows_cq3_satisfied(result.get('has_consistency_violation'))
    result['cq4_satisfied'] = calc_workflows_cq4_satisfied(result.get('count_derivation_links'), result.get('count_workflow_artifacts'))
    result['cq6_satisfied'] = calc_workflows_cq6_satisfied(result.get('count_roles_with_escalation_violation'))
    result['cq7_satisfied'] = calc_workflows_cq7_satisfied(result.get('involves_engineering_and_legal'))
    result['cq8_satisfied'] = calc_workflows_cq8_satisfied(result.get('count_unconsumed_datasets'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WORKFLOWSTEPS CALCULATIONS
# Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
# =============================================================================

# Level 1

def calc_workflow_steps_name(display_name):
    """Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")"""
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_workflow_steps_relative_path(parent_path, workflow_step_id):
    """
    Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
    
    Formula: ={{ParentPath}} & "/steps/" & {{WorkflowStepId}}
    """
    return (str(parent_path or "") + '/steps/' + str(workflow_step_id or ""))

def calc_workflow_steps_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def calc_workflow_steps_inferred_sequence_position():
    """ERROR: Could not parse formula: ={{PrecedingStepCount}} + 1
    Error: '+'
    """
    raise NotImplementedError("Formula parsing failed")


def calc_workflow_steps_sequence_position(sequence_position_override, inferred_sequence_position):
    """
    The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
    
    Formula: =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, {{InferredSequencePosition}})
    """
    return (sequence_position_override if (sequence_position_override != '') else inferred_sequence_position)


def calc_workflow_steps_executing_agent_type():
    """ERROR: Could not parse formula: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", "")))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_steps_is_executed_by_ai():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{ExecutingAIAgent}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_steps_is_executed_by_human():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{ExecutingHumanAgent}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_steps_is_approval_gate():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{ApprovalGate}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_steps_approval_consistency_violation():
    """ERROR: Could not parse formula: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_steps_approval_is_human_filled():
    """ERROR: Could not parse formula: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE)
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")


def calc_workflow_steps_is_legal_owned(owning_department):
    """
    TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
    
    Formula: ={{OwningDepartment}} = "ntwf-legal-dept" 
    """
    return (owning_department == 'ntwf-legal-dept')

def calc_workflow_steps_is_engineering_owned(owning_department):
    """
    TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
    
    Formula: ={{OwningDepartment}} = "ntwf-engineering" 
    """
    return (owning_department == 'ntwf-engineering')


def compute_workflow_steps_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WorkflowSteps.
    
    Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
    """
    result = dict(record)

    # Level 1 calculations
    result['name'] = calc_workflow_steps_name(result.get('display_name'))

    # Level 2 calculations
    result['relative_path'] = calc_workflow_steps_relative_path(result.get('parent_path'), result.get('workflow_step_id'))
    result['iri'] = calc_workflow_steps_iri(result.get('relative_path'))
    result['inferred_sequence_position'] = calc_workflow_steps_inferred_sequence_position(result.get('preceding_step_count'))
    result['sequence_position'] = calc_workflow_steps_sequence_position(result.get('sequence_position_override'), result.get('inferred_sequence_position'))
    result['executing_agent_type'] = calc_workflow_steps_executing_agent_type(result.get('executing_human_agent'), result.get('executing_ai_agent'), result.get('executing_automated_pipeline'))
    result['is_executed_by_ai'] = calc_workflow_steps_is_executed_by_ai(result.get('executing_ai_agent'))
    result['is_executed_by_human'] = calc_workflow_steps_is_executed_by_human(result.get('executing_human_agent'))
    result['is_approval_gate'] = calc_workflow_steps_is_approval_gate(result.get('approval_gate'))
    result['approval_consistency_violation'] = calc_workflow_steps_approval_consistency_violation(result.get('requires_human_approval'), result.get('executing_human_agent'))
    result['approval_is_human_filled'] = calc_workflow_steps_approval_is_human_filled(result.get('requires_human_approval'), result.get('executing_human_agent'))
    result['is_legal_owned'] = calc_workflow_steps_is_legal_owned(result.get('owning_department'))
    result['is_engineering_owned'] = calc_workflow_steps_is_engineering_owned(result.get('owning_department'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name', 'executing_agent_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPROVALGATES CALCULATIONS
# Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
# =============================================================================

# Level 1

def calc_approval_gates_name(display_name):
    """Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")"""
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_approval_gates_relative_path(parent_path, approval_gate_id):
    """
    Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
    
    Formula: ={{ParentPath}} & "/approval-gates/" & {{ApprovalGateId}}
    """
    return (str(parent_path or "") + '/approval-gates/' + str(approval_gate_id or ""))

def calc_approval_gates_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def calc_approval_gates_has_human_approver():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{GateApproverHuman}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def compute_approval_gates_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ApprovalGates.
    
    Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
    """
    result = dict(record)

    # Level 1 calculations
    result['name'] = calc_approval_gates_name(result.get('display_name'))

    # Level 2 calculations
    result['relative_path'] = calc_approval_gates_relative_path(result.get('parent_path'), result.get('approval_gate_id'))
    result['iri'] = calc_approval_gates_iri(result.get('relative_path'))
    result['has_human_approver'] = calc_approval_gates_has_human_approver(result.get('gate_approver_human'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPPRECEDENCE CALCULATIONS
# Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
# =============================================================================

# Level 1

def calc_step_precedence_relative_path(parent_path, step_precedence_id):
    """
    Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/precedence/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
    
    Formula: ={{ParentPath}} & "/precedence/" & {{StepPrecedenceId}}
    """
    return (str(parent_path or "") + '/precedence/' + str(step_precedence_id or ""))

def calc_step_precedence_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

def calc_step_precedence_name(from_step, to_step):
    """
    Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
    
    Formula: ={{FromStep}} & " -> " & {{ToStep}}
    """
    return (str(from_step or "") + ' -> ' + str(to_step or ""))


def compute_step_precedence_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepPrecedence.
    
    Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_step_precedence_relative_path(result.get('parent_path'), result.get('step_precedence_id'))
    result['iri'] = calc_step_precedence_iri(result.get('relative_path'))
    result['name'] = calc_step_precedence_name(result.get('from_step'), result.get('to_step'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLES CALCULATIONS
# Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
# =============================================================================

# Level 1

def calc_roles_relative_path(role_id):
    """
    Stable, DAG-derived location for this Role row. Root segment 'roles' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="roles/" & {{RoleId}}
    """
    return ('roles/' + str(role_id or ""))

def calc_roles_name(display_name):
    """Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")"""
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_roles_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

# Level 3


def calc_roles_filled_by_arm_count():
    """ERROR: Could not parse formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0)
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")


def calc_roles_has_exactly_one_filler(filled_by_arm_count):
    """
    Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
    
    Formula: ={{FilledByArmCount}} = 1
    """
    return (filled_by_arm_count == 1)


def calc_roles_filler_type():
    """ERROR: Could not parse formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_roles_escalation_violation():
    """ERROR: Could not parse formula: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def compute_roles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Roles.
    
    Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_roles_relative_path(result.get('role_id'))
    result['name'] = calc_roles_name(result.get('display_name'))

    # Level 2 calculations
    result['iri'] = calc_roles_iri(result.get('relative_path'))

    # Level 3 calculations
    result['filled_by_arm_count'] = calc_roles_filled_by_arm_count(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    result['has_exactly_one_filler'] = calc_roles_has_exactly_one_filler(result.get('filled_by_arm_count'))
    result['filler_type'] = calc_roles_filler_type(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    result['escalation_violation'] = calc_roles_escalation_violation(result.get('fills_approval_gate'), result.get('delegates_to'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name', 'filler_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLEASSIGNMENTS CALCULATIONS
# Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
# =============================================================================

# Level 1


def calc_role_assignments_is_current():
    """ERROR: Could not parse formula: =ISBLANK({{ValidTo}})
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_role_assignments_was_active_as_of_audit_date():
    """ERROR: Could not parse formula: =AND({{ValidFrom}} <= "2026-03-01", OR(ISBLANK({{ValidTo}}), {{ValidTo}} > "2026-03-01"))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")


# Level 2

def calc_role_assignments_relative_path(parent_path, role_assignment_id):
    """
    Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
    
    Formula: ={{ParentPath}} & "/assignments/" & {{RoleAssignmentId}}
    """
    return (str(parent_path or "") + '/assignments/' + str(role_assignment_id or ""))

def calc_role_assignments_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def calc_role_assignments_name():
    """ERROR: Could not parse formula: ={{Role}} & " [" & {{ValidFrom}} & " -> " & IF(ISBLANK({{ValidTo}}), "open", {{ValidTo}}) & "]"
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_role_assignments_filler_type():
    """ERROR: Could not parse formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_role_assignments_is_agent_type_change():
    """ERROR: Could not parse formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} <> {{FillerType}})
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_role_assignments_requires_compliance_audit():
    """ERROR: Could not parse formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent")
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def compute_role_assignments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleAssignments.
    
    Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
    """
    result = dict(record)

    # Level 1 calculations
    result['is_current'] = calc_role_assignments_is_current(result.get('valid_to'))
    result['was_active_as_of_audit_date'] = calc_role_assignments_was_active_as_of_audit_date(result.get('valid_from'), result.get('valid_to'))

    # Level 2 calculations
    result['relative_path'] = calc_role_assignments_relative_path(result.get('parent_path'), result.get('role_assignment_id'))
    result['iri'] = calc_role_assignments_iri(result.get('relative_path'))
    result['name'] = calc_role_assignments_name(result.get('role'), result.get('valid_from'), result.get('valid_to'))
    result['filler_type'] = calc_role_assignments_filler_type(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    result['is_agent_type_change'] = calc_role_assignments_is_agent_type_change(result.get('prior_filler_type'), result.get('filler_type'))
    result['requires_compliance_audit'] = calc_role_assignments_requires_compliance_audit(result.get('prior_filler_type'), result.get('filler_type'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name', 'filler_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# DEPARTMENTS CALCULATIONS
# Table: Departments. The NTWF Department class — schema:Organization. First-class entity that enables cross-department intersection queries (CQ7: which workflows involve both Engineering and Legal?). Roles are ownedBy a department.
# =============================================================================

# Level 1

def calc_departments_relative_path(department_id):
    """
    Stable, DAG-derived location for this Department row. Root segment 'departments' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="departments/" & {{DepartmentId}}
    """
    return ('departments/' + str(department_id or ""))

def calc_departments_name(display_name):
    """
    Human-readable display name of the department. Should match organizational terminology for stakeholder communication.
    
    Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
    """
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_departments_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_departments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Departments.
    
    Table: Departments. The NTWF Department class — schema:Organization. First-class entity that enables cross-department intersection queries (CQ7: which workflows involve both Engineering and Legal?). Roles are ownedBy a department.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_departments_relative_path(result.get('department_id'))
    result['name'] = calc_departments_name(result.get('display_name'))

    # Level 2 calculations
    result['iri'] = calc_departments_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# HUMANAGENTS CALCULATIONS
# Table: HumanAgents. The NTWF HumanAgent class — foaf:Person + prov:Agent. The only agent type permitted to fill roles whose step has requiresHumanApproval. Disjoint with AIAgent and AutomatedPipeline.
# =============================================================================

# Level 1

def calc_human_agents_relative_path(human_agent_id):
    """
    Stable, DAG-derived location for this HumanAgent row. Root segment 'human-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="human-agents/" & {{HumanAgentId}}
    """
    return ('human-agents/' + str(human_agent_id or ""))

# Level 2

def calc_human_agents_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_human_agents_fields(record: dict) -> dict:
    """
    Compute all calculated fields for HumanAgents.
    
    Table: HumanAgents. The NTWF HumanAgent class — foaf:Person + prov:Agent. The only agent type permitted to fill roles whose step has requiresHumanApproval. Disjoint with AIAgent and AutomatedPipeline.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_human_agents_relative_path(result.get('human_agent_id'))

    # Level 2 calculations
    result['iri'] = calc_human_agents_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AIAGENTS CALCULATIONS
# Table: AIAgents. The NTWF AIAgent class — prov:SoftwareAgent + ntwf:modelVersion. Distinguished from AutomatedPipeline by probabilistic (vs. deterministic) output semantics. Disjoint with HumanAgent and AutomatedPipeline. May never fill a role whose step has requiresHumanApproval.
# =============================================================================

# Level 1

def calc_ai_agents_relative_path(ai_agent_id):
    """
    Stable, DAG-derived location for this AIAgent row. Root segment 'ai-agents' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="ai-agents/" & {{AIAgentId}}
    """
    return ('ai-agents/' + str(ai_agent_id or ""))

# Level 2

def calc_ai_agents_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_ai_agents_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AIAgents.
    
    Table: AIAgents. The NTWF AIAgent class — prov:SoftwareAgent + ntwf:modelVersion. Distinguished from AutomatedPipeline by probabilistic (vs. deterministic) output semantics. Disjoint with HumanAgent and AutomatedPipeline. May never fill a role whose step has requiresHumanApproval.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_ai_agents_relative_path(result.get('ai_agent_id'))

    # Level 2 calculations
    result['iri'] = calc_ai_agents_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AUTOMATEDPIPELINES CALCULATIONS
# Table: AutomatedPipelines. The NTWF AutomatedPipeline class — prov:SoftwareAgent + schema:SoftwareApplication. Distinguished from AIAgent by deterministic (vs. probabilistic) output semantics. Disjoint with HumanAgent and AIAgent. Carries schema:name, not foaf:name.
# =============================================================================

# Level 1

def calc_automated_pipelines_relative_path(automated_pipeline_id):
    """
    Stable, DAG-derived location for this AutomatedPipeline row. Root segment 'automated-pipelines' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="automated-pipelines/" & {{AutomatedPipelineId}}
    """
    return ('automated-pipelines/' + str(automated_pipeline_id or ""))

# Level 2

def calc_automated_pipelines_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_automated_pipelines_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AutomatedPipelines.
    
    Table: AutomatedPipelines. The NTWF AutomatedPipeline class — prov:SoftwareAgent + schema:SoftwareApplication. Distinguished from AIAgent by deterministic (vs. probabilistic) output semantics. Disjoint with HumanAgent and AIAgent. Carries schema:name, not foaf:name.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_automated_pipelines_relative_path(result.get('automated_pipeline_id'))

    # Level 2 calculations
    result['iri'] = calc_automated_pipelines_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WORKFLOWSTATUSCONCEPTS CALCULATIONS
# SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows.
# =============================================================================

# Level 1

def calc_workflow_status_concepts_relative_path(concept_id):
    """
    Stable, DAG-derived location for this WorkflowStatusConcept row. Root segment 'concepts/workflow-status' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="concepts/workflow-status/" & {{ConceptId}}
    """
    return ('concepts/workflow-status/' + str(concept_id or ""))

# Level 2

def calc_workflow_status_concepts_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_workflow_status_concepts_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WorkflowStatusConcepts.
    
    SKOS controlled vocabulary for workflow lifecycle states (ntwf:WorkflowStatusScheme). Part of the CBox. Concepts are shared across all workflows.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_workflow_status_concepts_relative_path(result.get('concept_id'))

    # Level 2 calculations
    result['iri'] = calc_workflow_status_concepts_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AGENTCAPABILITYCONCEPTS CALCULATIONS
# SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox.
# =============================================================================

# Level 1

def calc_agent_capability_concepts_relative_path(concept_id):
    """
    Stable, DAG-derived location for this AgentCapabilityConcept row. Root segment 'concepts/agent-capability' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="concepts/agent-capability/" & {{ConceptId}}
    """
    return ('concepts/agent-capability/' + str(concept_id or ""))

# Level 2

def calc_agent_capability_concepts_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_agent_capability_concepts_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AgentCapabilityConcepts.
    
    SKOS controlled vocabulary for agent capability types (ntwf:AgentCapabilityScheme). Roles declare which capability their filler must have (ntwf:hasCapability). Part of the CBox.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_agent_capability_concepts_relative_path(result.get('concept_id'))

    # Level 2 calculations
    result['iri'] = calc_agent_capability_concepts_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ARTIFACTTYPECONCEPTS CALCULATIONS
# SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts.
# =============================================================================

# Level 1

def calc_artifact_type_concepts_relative_path(concept_id):
    """
    Stable, DAG-derived location for this concept row. Root segment 'concepts/artifact-type' + the row's primary key.
    
    Formula: ="concepts/artifact-type/" & {{ConceptId}}
    """
    return ('concepts/artifact-type/' + str(concept_id or ""))

# Level 2

def calc_artifact_type_concepts_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_artifact_type_concepts_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ArtifactTypeConcepts.
    
    SKOS controlled vocabulary for artifact type (ntwf artifact-type scheme). Part of the CBox; NTWF names a CBox concept scheme for artifact types alongside workflow status and agent capabilities. Each artifact is classified via dct:type into one of these concepts.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_artifact_type_concepts_relative_path(result.get('concept_id'))

    # Level 2 calculations
    result['iri'] = calc_artifact_type_concepts_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# DATASETS CALCULATIONS
# DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
# =============================================================================

# Level 1

def calc_datasets_relative_path(dataset_id):
    """
    Stable, DAG-derived location for this Dataset row. Root segment 'datasets' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution. The relational analogue of a REST resource path; unique by construction across the whole model.
    
    Formula: ="datasets/" & {{DatasetId}}
    """
    return ('datasets/' + str(dataset_id or ""))

# Level 2

def calc_datasets_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

# Level 3


def calc_datasets_is_consumed():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{ConsumedBySteps}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def compute_datasets_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Datasets.
    
    DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_datasets_relative_path(result.get('dataset_id'))

    # Level 2 calculations
    result['iri'] = calc_datasets_iri(result.get('relative_path'))

    # Level 3 calculations
    result['is_consumed'] = calc_datasets_is_consumed(result.get('consumed_by_steps'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WORKFLOWARTIFACTS CALCULATIONS
# Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
# =============================================================================

# Level 1

def calc_workflow_artifacts_relative_path(parent_path, artifact_id):
    """
    Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/artifacts/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
    
    Formula: ={{ParentPath}} & "/artifacts/" & {{ArtifactId}}
    """
    return (str(parent_path or "") + '/artifacts/' + str(artifact_id or ""))

def calc_workflow_artifacts_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def calc_workflow_artifacts_producing_agent_type():
    """ERROR: Could not parse formula: =IF(NOT(ISBLANK({{AttributedToHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{AttributedToAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{AttributedToAutomatedPipeline}})), "AutomatedPipeline", "")))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_artifacts_has_derivation_parent():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{DerivedFromArtifact}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def calc_workflow_artifacts_has_producing_workflow():
    """ERROR: Could not parse formula: =NOT(ISBLANK({{ProducedByWorkflow}}))
    Error: Unknown function: ISBLANK
    """
    raise NotImplementedError("Formula parsing failed")



def compute_workflow_artifacts_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WorkflowArtifacts.
    
    Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_workflow_artifacts_relative_path(result.get('parent_path'), result.get('artifact_id'))
    result['iri'] = calc_workflow_artifacts_iri(result.get('relative_path'))
    result['producing_agent_type'] = calc_workflow_artifacts_producing_agent_type(result.get('attributed_to_human_agent'), result.get('attributed_to_ai_agent'), result.get('attributed_to_automated_pipeline'))
    result['has_derivation_parent'] = calc_workflow_artifacts_has_derivation_parent(result.get('derived_from_artifact'))
    result['has_producing_workflow'] = calc_workflow_artifacts_has_producing_workflow(result.get('produced_by_workflow'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'producing_agent_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# GOVERNANCEROLES CALCULATIONS
# Table: GovernanceRoles. NTWF governance names two distinct ontology-governance roles: a Steward (responsible for the ontology's health — monitors drift, tracks external dependency updates, fields user questions, maintains docs, keeps the validation suite current; identifies that a change is needed but has no approval power) and an Authority (the power to approve changes to the CBox, ABox, and TBox; decides how and where a change is made; sits with the function that owns the domain). 'A steward who can make TBox or ABox changes without authority review is a single point of failure.' For an organization under 500 people a single person may hold both roles. This table models the maintenance discipline itself, as data, so the change log can attribute approvals to a named authority.
# =============================================================================

# Level 1

def calc_governance_roles_relative_path(governance_role_id):
    """
    Stable, DAG-derived location for this GovernanceRole row. Root segment 'governance-roles' + the row's primary key.
    
    Formula: ="governance-roles/" & {{GovernanceRoleId}}
    """
    return ('governance-roles/' + str(governance_role_id or ""))

def calc_governance_roles_name(display_name):
    """
    Slug form of the display name.
    
    Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
    """
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

def calc_governance_roles_can_approve_changes(kind):
    """
    TRUE iff this governance role carries approval power (Kind = 'Authority'). A Steward returns FALSE — a steward making TBox/ABox changes without authority review is a single point of failure.
    
    Formula: ={{Kind}} = "Authority" 
    """
    return (kind == 'Authority')

# Level 2

def calc_governance_roles_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_governance_roles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for GovernanceRoles.
    
    Table: GovernanceRoles. NTWF governance names two distinct ontology-governance roles: a Steward (responsible for the ontology's health — monitors drift, tracks external dependency updates, fields user questions, maintains docs, keeps the validation suite current; identifies that a change is needed but has no approval power) and an Authority (the power to approve changes to the CBox, ABox, and TBox; decides how and where a change is made; sits with the function that owns the domain). 'A steward who can make TBox or ABox changes without authority review is a single point of failure.' For an organization under 500 people a single person may hold both roles. This table models the maintenance discipline itself, as data, so the change log can attribute approvals to a named authority.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_governance_roles_relative_path(result.get('governance_role_id'))
    result['name'] = calc_governance_roles_name(result.get('display_name'))
    result['can_approve_changes'] = calc_governance_roles_can_approve_changes(result.get('kind'))

    # Level 2 calculations
    result['iri'] = calc_governance_roles_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# CHANGELOG CALCULATIONS
# Table: ChangeLog. NTWF's minimum governance artifact: 'a change log that records every TBox and ABox modification, with its rationale.' Each entry records the four facts NTWF governance enumerates — the competency question that motivated the change, the terms affected, the version number of the release, and the date — plus the rationale and the Authority who approved it. Semantic-versioning discipline (MAJOR.MINOR.PATCH) is captured per entry via ChangeKind.
# =============================================================================

# Level 1

def calc_change_log_relative_path(change_log_id):
    """
    Stable, DAG-derived location for this ChangeLog row. Root segment 'change-log' + the row's primary key.
    
    Formula: ="change-log/" & {{ChangeLogId}}
    """
    return ('change-log/' + str(change_log_id or ""))

def calc_change_log_name(version, change_date):
    """
    Human-readable label: the version and date of this change.
    
    Formula: ={{Version}} & " (" & {{ChangeDate}} & ")" 
    """
    return (str(version or "") + ' (' + str(change_date or "") + ')')

def calc_change_log_is_breaking_change(change_kind):
    """
    TRUE iff this is a major (breaking) change (ChangeKind = 'major') — requires explicit update, re-validation, and migration planning for any system on the prior version.
    
    Formula: ={{ChangeKind}} = "major" 
    """
    return (change_kind == 'major')

def calc_change_log_is_backward_compatible(change_kind):
    """
    TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
    
    Formula: =OR({{ChangeKind}} = "patch", {{ChangeKind}} = "minor")
    """
    return ((change_kind == 'patch') or (change_kind == 'minor'))

# Level 2

def calc_change_log_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath).
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_change_log_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ChangeLog.
    
    Table: ChangeLog. NTWF's minimum governance artifact: 'a change log that records every TBox and ABox modification, with its rationale.' Each entry records the four facts NTWF governance enumerates — the competency question that motivated the change, the terms affected, the version number of the release, and the date — plus the rationale and the Authority who approved it. Semantic-versioning discipline (MAJOR.MINOR.PATCH) is captured per entry via ChangeKind.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_change_log_relative_path(result.get('change_log_id'))
    result['name'] = calc_change_log_name(result.get('version'), result.get('change_date'))
    result['is_breaking_change'] = calc_change_log_is_breaking_change(result.get('change_kind'))
    result['is_backward_compatible'] = calc_change_log_is_backward_compatible(result.get('change_kind'))

    # Level 2 calculations
    result['iri'] = calc_change_log_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# VOCABULARYRECONCILIATIONS CALCULATIONS
# Table: VocabularyReconciliations. External dependency change: when a borrowed term from a living standard (PROV-O, FOAF, Dublin Core, DCAT, Schema.org) is deprecated and re-homed into the NTWF namespace, the edit triggers a version bump and an owl:sameAs reconciliation relation declaring the old and new terms equivalent. The worked example: deprecating foaf:name, prepending the ntwf prefix to get ntwf:name, and asserting foaf:name owl:sameAs ntwf:name. Each row is one reconciliation, with the standard it came from and the version in which the reconciliation shipped.
# =============================================================================

# Level 1

def calc_vocabulary_reconciliations_relative_path(reconciliation_id):
    """
    Stable, DAG-derived location for this reconciliation row. Root segment 'reconciliations' + the row's primary key.
    
    Formula: ="reconciliations/" & {{ReconciliationId}}
    """
    return ('reconciliations/' + str(reconciliation_id or ""))

def calc_vocabulary_reconciliations_name(deprecated_term, replacement_term):
    """
    Human-readable label: the sameAs relation between the deprecated term and its NTWF replacement.
    
    Formula: ={{DeprecatedTerm}} & " owl:sameAs " & {{ReplacementTerm}}
    """
    return (str(deprecated_term or "") + ' owl:sameAs ' + str(replacement_term or ""))

# Level 2

def calc_vocabulary_reconciliations_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath).
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_vocabulary_reconciliations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for VocabularyReconciliations.
    
    Table: VocabularyReconciliations. External dependency change: when a borrowed term from a living standard (PROV-O, FOAF, Dublin Core, DCAT, Schema.org) is deprecated and re-homed into the NTWF namespace, the edit triggers a version bump and an owl:sameAs reconciliation relation declaring the old and new terms equivalent. The worked example: deprecating foaf:name, prepending the ntwf prefix to get ntwf:name, and asserting foaf:name owl:sameAs ntwf:name. Each row is one reconciliation, with the standard it came from and the version in which the reconciliation shipped.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_vocabulary_reconciliations_relative_path(result.get('reconciliation_id'))
    result['name'] = calc_vocabulary_reconciliations_name(result.get('deprecated_term'), result.get('replacement_term'))

    # Level 2 calculations
    result['iri'] = calc_vocabulary_reconciliations_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# SCENARIOS CALCULATIONS
# =============================================================================

# Level 1

def calc_scenarios_relative_path(scenario_id):
    """
    DAG-derived location for this Scenario row: root segment 'scenarios' + the primary key.
    
    Formula: ="scenarios/" & {{ScenarioId}}
    """
    return ('scenarios/' + str(scenario_id or ""))

def calc_scenarios_name(label):
    """
    Slug form of the human label.
    
    Formula: =SUBSTITUTE(LOWER({{Label}}), " ", "-")
    """
    return ((((label or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_scenarios_iri(relative_path):
    """
    Opaque stable identifier (dash-form of RelativePath).
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_scenarios_fields(record: dict) -> dict:
    """Compute all calculated fields for Scenarios."""
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_scenarios_relative_path(result.get('scenario_id'))
    result['name'] = calc_scenarios_name(result.get('label'))

    # Level 2 calculations
    result['iri'] = calc_scenarios_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# COMPETENCYQUESTIONS CALCULATIONS
# The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook.
# =============================================================================

# Level 1

def calc_competency_questions_relative_path(competency_question_id):
    """
    Stable, DAG-derived location for this CompetencyQuestion row. Root segment 'competency-questions' + the row's primary key. No leading slash so the Iri swap is a clean 1:1 substitution.
    
    Formula: ="competency-questions/" & {{CompetencyQuestionId}}
    """
    return ('competency-questions/' + str(competency_question_id or ""))

def calc_competency_questions_name(display_name):
    """
    Slug form of the DisplayName, for stable cross-reference. Mirrors the Name idiom used by the controlled-vocabulary tables.
    
    Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
    """
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_competency_questions_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_competency_questions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for CompetencyQuestions.
    
    The article's literal acceptance suite — the eight leadership/competency questions the NTWF worked example must answer (Talisman, Intentional Arrangement, CQ1-CQ8). First-class data, not hardcoded UI strings: each row names the question, the substrate-computed field that ANSWERS it (TargetTable/TargetField, for cross-substrate traceability and the explainer-DAG drilldown), the answer kind, and the asserted ExpectedAnswer used to grade pass/fail. The live answer is always READ from the named computed column — never recomputed — so the CQ scoreboard is a projection of the model like every other lens. This is the CMCC-native home for the competency questions: the article treats them as acceptance criteria traceable to the rulebook, so they live in the rulebook.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_competency_questions_relative_path(result.get('competency_question_id'))
    result['name'] = calc_competency_questions_name(result.get('display_name'))

    # Level 2 calculations
    result['iri'] = calc_competency_questions_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# SCENARIOCQEFFECTS CALCULATIONS
# Table: ScenarioCQEffects. Names the many-to-many between Scenarios and CompetencyQuestions as two 1:M foreign keys (Scenario, CompetencyQuestion) plus the detail of the relationship (EffectKind, Note). Each row asserts 'applying this scenario moves this competency question's live answer'. 'trigger' rows are the intended demonstration; 'ripple' rows record answers that move as an unavoidable consequence of the same raw edit (e.g. ai-release-manager moves cq-2 AND cq-3 because the gate approver is itself a step executor). The answers themselves are never stored here — they are read live from each substrate after the scenario applies.
# =============================================================================

# Level 1

def calc_scenario_cq_effects_relative_path(scenario_cq_effect_id):
    """
    DAG-derived location: 'scenario-cq-effects/' + the row's primary key.
    
    Formula: ="scenario-cq-effects/" & {{ScenarioCQEffectId}}
    """
    return ('scenario-cq-effects/' + str(scenario_cq_effect_id or ""))

def calc_scenario_cq_effects_name(scenario_cq_effect_id):
    """
    Slug label, mirrors the primary key.
    
    Formula: =SUBSTITUTE(LOWER({{ScenarioCQEffectId}}), " ", "-")
    """
    return ((((scenario_cq_effect_id or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_scenario_cq_effects_iri(relative_path):
    """
    Opaque stable identifier (dash-form of RelativePath).
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_scenario_cq_effects_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ScenarioCQEffects.
    
    Table: ScenarioCQEffects. Names the many-to-many between Scenarios and CompetencyQuestions as two 1:M foreign keys (Scenario, CompetencyQuestion) plus the detail of the relationship (EffectKind, Note). Each row asserts 'applying this scenario moves this competency question's live answer'. 'trigger' rows are the intended demonstration; 'ripple' rows record answers that move as an unavoidable consequence of the same raw edit (e.g. ai-release-manager moves cq-2 AND cq-3 because the gate approver is itself a step executor). The answers themselves are never stored here — they are read live from each substrate after the scenario applies.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_scenario_cq_effects_relative_path(result.get('scenario_cq_effect_id'))
    result['name'] = calc_scenario_cq_effects_name(result.get('scenario_cq_effect_id'))

    # Level 2 calculations
    result['iri'] = calc_scenario_cq_effects_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# CONFORMANCETESTS CALCULATIONS
# =============================================================================

# Level 1

def calc_conformance_tests_relative_path(conformance_test_id):
    """
    DAG-derived location for this test row: root segment 'conformance-tests' + the primary key.
    
    Formula: ="conformance-tests/" & {{ConformanceTestId}}
    """
    return ('conformance-tests/' + str(conformance_test_id or ""))

def calc_conformance_tests_name(display_name):
    """
    Machine name derived from the display name.
    
    Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")
    """
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

# Level 2

def calc_conformance_tests_iri(relative_path):
    """
    Slug IRI for this row, derived from RelativePath.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_conformance_tests_fields(record: dict) -> dict:
    """Compute all calculated fields for ConformanceTests."""
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_conformance_tests_relative_path(result.get('conformance_test_id'))
    result['name'] = calc_conformance_tests_name(result.get('display_name'))

    # Level 2 calculations
    result['iri'] = calc_conformance_tests_iri(result.get('relative_path'))

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# __META__ CALCULATIONS
# Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
# =============================================================================

# Level 1

def calc___meta___name(meta_key):
    """
    Identifier for this metadata entry. Mirrors MetaKey so the row is addressable by Name like every other table.
    
    Formula: ={{MetaKey}}
    """
    return meta_key


def compute___meta___fields(record: dict) -> dict:
    """
    Compute all calculated fields for __meta__.
    
    Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
    """
    result = dict(record)

    # Level 1 calculations
    result['name'] = calc___meta___name(result.get('meta_key'))

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result


# =============================================================================
# DISPATCHER FUNCTION
# =============================================================================

def compute_all_calculated_fields(record: dict, entity_name: str = None) -> dict:
    """
    Compute all calculated fields for a record.
    
    This is the main entry point for computing calculated fields.
    It routes to the appropriate entity-specific compute function.
    
    Args:
        record: The record dict with raw field values
        entity_name: Entity name (snake_case or PascalCase)
    
    Returns:
        Record dict with calculated fields filled in
    """
    if entity_name is None:
        # No entity specified - return record unchanged
        return dict(record)

    # Normalize to snake_case to support "LineItem", "line_item", "line-item"
    entity_lower = entity_name.lower().replace('-', '_')

    if entity_lower == 'workflows':
        return compute_workflows_fields(record)
    elif entity_lower == 'workflow_steps':
        return compute_workflow_steps_fields(record)
    elif entity_lower == 'approval_gates':
        return compute_approval_gates_fields(record)
    elif entity_lower == 'step_precedence':
        return compute_step_precedence_fields(record)
    elif entity_lower == 'roles':
        return compute_roles_fields(record)
    elif entity_lower == 'role_assignments':
        return compute_role_assignments_fields(record)
    elif entity_lower == 'departments':
        return compute_departments_fields(record)
    elif entity_lower == 'human_agents':
        return compute_human_agents_fields(record)
    elif entity_lower == 'ai_agents':
        return compute_ai_agents_fields(record)
    elif entity_lower == 'automated_pipelines':
        return compute_automated_pipelines_fields(record)
    elif entity_lower == 'workflow_status_concepts':
        return compute_workflow_status_concepts_fields(record)
    elif entity_lower == 'agent_capability_concepts':
        return compute_agent_capability_concepts_fields(record)
    elif entity_lower == 'artifact_type_concepts':
        return compute_artifact_type_concepts_fields(record)
    elif entity_lower == 'datasets':
        return compute_datasets_fields(record)
    elif entity_lower == 'workflow_artifacts':
        return compute_workflow_artifacts_fields(record)
    elif entity_lower == 'governance_roles':
        return compute_governance_roles_fields(record)
    elif entity_lower == 'change_log':
        return compute_change_log_fields(record)
    elif entity_lower == 'vocabulary_reconciliations':
        return compute_vocabulary_reconciliations_fields(record)
    elif entity_lower == 'scenarios':
        return compute_scenarios_fields(record)
    elif entity_lower == 'competency_questions':
        return compute_competency_questions_fields(record)
    elif entity_lower == 'scenario_cq_effects':
        return compute_scenario_cq_effects_fields(record)
    elif entity_lower == 'conformance_tests':
        return compute_conformance_tests_fields(record)
    elif entity_lower == '__meta__':
        return compute___meta___fields(record)
    else:
        raise KeyError(f"compute_all_calculated_fields called with unknown entity {entity_name!r}. "f"Known entities in this generated erb_calc.py: ['workflows', 'workflow_steps', 'approval_gates', 'step_precedence', 'roles', 'role_assignments', 'departments', 'human_agents', 'ai_agents', 'automated_pipelines', 'workflow_status_concepts', 'agent_capability_concepts', 'artifact_type_concepts', 'datasets', 'workflow_artifacts', 'governance_roles', 'change_log', 'vocabulary_reconciliations', 'scenarios', 'competency_questions', 'scenario_cq_effects', 'conformance_tests', '__meta__']. "f"Check that the rulebook used to generate this file matches the data being computed.")

# =============================================================================
# INDEX/MATCH LOOKUP INTERPRETER (PYTHON SIMULATOR — DO NOT CALL FROM OTHER SUBSTRATES)
# =============================================================================


def parse_index_match_formula(formula: str) -> tuple:
    """
    Parse an INDEX/MATCH formula to extract the lookup components.

    Formula format: =INDEX(Table!{{FieldToReturn}}, MATCH(CurrentTable!{{KeyField}}, Table!{{PrimaryKeyField}}, 0))
    Returns: (lookup_table, return_field, key_field, pk_field) or all None.
    """
    pattern = r"=INDEX\((\w+)!\{\{(\w+)\}\},\s*MATCH\(\w+!\{\{(\w+)\}\},\s*(\w+)!\{\{(\w+)\}\},\s*0\)\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3), match.group(5))
    return (None, None, None, None)


def parse_countifs_formula(formula: str) -> tuple:
    """Parse =COUNTIFS(RelatedTable!{{LookupField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=COUNTIFS\((\w+)!\{\{(\w+)\}\},\s*\w+!\{\{(\w+)\}\}\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3))
    return (None, None, None)


def parse_sumifs_formula(formula: str) -> tuple:
    """Parse =SUMIFS(RelatedTable!{{SumField}}, RelatedTable!{{CriteriaField}}, CurrentTable!{{MatchField}})."""
    pattern = r"=SUMIFS\((\w+)!\{\{(\w+)\}\},\s*(\w+)!\{\{(\w+)\}\},\s*\w+!\{\{(\w+)\}\}\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(4), match.group(5))
    return (None, None, None, None)


def _get_testing_dir(project_root: Path) -> Path:
    """Return the active domain's testing/ dir. ERB_TESTING_DIR is required.

    The injector runs at build time and must operate on the same domain the
    orchestrator chose. There is no implicit per-substrate testing dir.
    """
    import os
    erb = os.environ.get("ERB_TESTING_DIR")
    if not erb:
        raise RuntimeError(
            "ERB_TESTING_DIR is not set. inject-into-python.py must be invoked "
            "by the orchestrator with ERB_TESTING_DIR pointing at the active "
            "domain's testing/ directory."
        )
    return Path(erb)


def load_related_data(project_root: Path, related_table: str) -> list:
    """
    Load data from testing/answer-keys for a related table; falls back to blank-tests.
    Prefers answer-keys so that aggregations referencing computed fields in related
    tables resolve correctly.
    """
    snake_name = to_snake_case(related_table)
    testing_dir = _get_testing_dir(project_root)

    answer_keys_path = testing_dir / "answer-keys" / f"{snake_name}.json"
    if answer_keys_path.exists():
        with open(answer_keys_path, "r", encoding="utf-8") as f:
            return json.load(f)

    blank_tests_path = testing_dir / "blank-tests" / f"{snake_name}.json"
    if blank_tests_path.exists():
        with open(blank_tests_path, "r", encoding="utf-8") as f:
            return json.load(f)

    return []


def compute_lookups(records: list, entity_name: str, rulebook: dict, project_root: Path) -> list:
    """INDEX/MATCH lookup interpreter. PYTHON SIMULATOR ONLY."""
    schema = get_entity_schema(rulebook, entity_name)
    lookup_fields = get_lookup_fields(schema)

    if not lookup_fields:
        return records

    related_data_cache = {}

    for field in lookup_fields:
        field_name = field.get("name")
        formula = field.get("formula", "")
        snake_field_name = to_snake_case(field_name)

        lookup_table, return_field, key_field, pk_field = parse_index_match_formula(formula)

        if not lookup_table:
            continue

        if lookup_table not in related_data_cache:
            related_data_cache[lookup_table] = load_related_data(project_root, lookup_table)

        related_records = related_data_cache[lookup_table]
        snake_return_field = to_snake_case(return_field)
        snake_key_field = to_snake_case(key_field)
        snake_pk_field = to_snake_case(pk_field)

        lookup_map = {}
        for related_record in related_records:
            pk_value = related_record.get(snake_pk_field)
            if pk_value is not None:
                lookup_map[pk_value] = related_record.get(snake_return_field)

        for record in records:
            key_value = record.get(snake_key_field)
            if key_value is not None and key_value in lookup_map:
                record[snake_field_name] = lookup_map[key_value]
            else:
                record[snake_field_name] = None

    return records


def compute_aggregations(records: list, entity_name: str, rulebook: dict, project_root: Path) -> list:
    """COUNTIFS / SUMIFS aggregation interpreter. PYTHON SIMULATOR ONLY."""
    schema = get_entity_schema(rulebook, entity_name)
    agg_fields = get_aggregation_fields(schema)

    if not agg_fields:
        return records

    related_data_cache = {}

    for field in agg_fields:
        field_name = field.get("name")
        formula = field.get("formula", "")
        snake_field_name = to_snake_case(field_name)

        related_table, lookup_field, match_field = parse_countifs_formula(formula)

        if related_table:
            if related_table not in related_data_cache:
                related_data_cache[related_table] = load_related_data(project_root, related_table)

            related_records = related_data_cache[related_table]
            snake_lookup_field = to_snake_case(lookup_field)
            snake_match_field = to_snake_case(match_field)

            count_map = {}
            for related_record in related_records:
                lookup_value = related_record.get(snake_lookup_field)
                if lookup_value is not None:
                    count_map[lookup_value] = count_map.get(lookup_value, 0) + 1

            for record in records:
                match_value = record.get(snake_match_field)
                if match_value is not None:
                    record[snake_field_name] = count_map.get(match_value, 0)
                else:
                    record[snake_field_name] = 0
            continue

        related_table, sum_field, criteria_field, match_field = parse_sumifs_formula(formula)

        if related_table:
            if related_table not in related_data_cache:
                related_data_cache[related_table] = load_related_data(project_root, related_table)

            related_records = related_data_cache[related_table]
            snake_sum_field = to_snake_case(sum_field)
            snake_criteria_field = to_snake_case(criteria_field)
            snake_match_field = to_snake_case(match_field)

            is_distinct = "distinct" in field_name.lower()

            related_pk_field = to_snake_case(related_table[:-1] + "Id")
            pk_to_record = {}
            for rec in related_records:
                pk_val = rec.get(related_pk_field)
                if pk_val:
                    pk_to_record[pk_val] = rec

            relationship_field = None
            for f in schema:
                if f.get("type") == "relationship" and f.get("RelatedTo") == related_table:
                    relationship_field = to_snake_case(f.get("name"))
                    break

            for record in records:
                match_value = record.get(snake_match_field)
                values = []
                has_any_match = False

                if relationship_field and relationship_field in record and record[relationship_field]:
                    rel_ids = [rid.strip() for rid in str(record[relationship_field]).split(",") if rid.strip()]
                    for rel_id in rel_ids:
                        rel_rec = pk_to_record.get(rel_id)
                        if rel_rec:
                            criteria_value = rel_rec.get(snake_criteria_field)
                            if criteria_value == match_value:
                                has_any_match = True
                                sum_value = rel_rec.get(snake_sum_field)
                                if sum_value is not None and sum_value != "" and sum_value != 0:
                                    str_value = str(sum_value)
                                    if is_distinct:
                                        if str_value not in values:
                                            values.append(str_value)
                                    else:
                                        values.append(str_value)
                                else:
                                    values.append("")
                else:
                    sorted_records = sorted(
                        related_records,
                        key=lambda r: r.get("sequence_position", 0) if r.get("sequence_position") is not None else 0,
                    )
                    for related_record in sorted_records:
                        criteria_value = related_record.get(snake_criteria_field)
                        if criteria_value == match_value:
                            has_any_match = True
                            sum_value = related_record.get(snake_sum_field)
                            if sum_value is not None and sum_value != "" and sum_value != 0:
                                str_value = str(sum_value)
                                if is_distinct:
                                    if str_value not in values:
                                        values.append(str_value)
                                else:
                                    values.append(str_value)

                non_empty_values = [v for v in values if v]
                if non_empty_values:
                    if is_distinct:
                        record[snake_field_name] = ", ".join(non_empty_values)
                    else:
                        record[snake_field_name] = ", ".join(values)
                elif has_any_match:
                    record[snake_field_name] = 0
                elif is_distinct:
                    record[snake_field_name] = ""
                else:
                    record[snake_field_name] = 0

    return records
