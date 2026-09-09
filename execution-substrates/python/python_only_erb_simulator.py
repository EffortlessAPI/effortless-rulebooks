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

from orchestration import formula_parser as _erb

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

def calc_workflows_has_more_than1_step(count_of_non_proposed_steps):
    """Formula: ={{CountOfNonProposedSteps}} > 1"""
    return (False if (count_of_non_proposed_steps) is None else (count_of_non_proposed_steps) > (1))

def calc_workflows_has_consistency_violation(count_approval_consistency_violations):
    """
    TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
    
    Formula: ={{CountApprovalConsistencyViolations}} > 0
    """
    return (False if (count_approval_consistency_violations) is None else (count_approval_consistency_violations) > (0))

def calc_workflows_has_ai_agent_step(count_ai_steps):
    """
    TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
    
    Formula: ={{CountAISteps}} > 0
    """
    return (False if (count_ai_steps) is None else (count_ai_steps) > (0))

def calc_workflows_months_since_modified(modified):
    """
    Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
    
    Formula: =DATETIME_DIFF(NOW(), {{Modified}}, "months")
    """
    return _erb.erb_datetime_diff(_erb.erb_now(), modified, 'months')

def calc_workflows_involves_engineering_and_legal(count_engineering_owned_steps, count_legal_owned_steps):
    """
    TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
    
    Formula: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0)
    """
    return ((False if (count_engineering_owned_steps) is None else (count_engineering_owned_steps) > (0)) and (False if (count_legal_owned_steps) is None else (count_legal_owned_steps) > (0)))

def calc_workflows_count_of_precedence_closure_pairs(count_asserted_precedence_pairs, count_inferred_precedence_pairs):
    """
    Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
    
    Formula: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}}
    """
    return ((count_asserted_precedence_pairs) or 0) + ((count_inferred_precedence_pairs) or 0)

def calc_workflows_cq2_satisfied(count_approval_gate_steps, count_gates_without_human_approver):
    """
    CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
    
    Formula: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0)
    """
    return ((False if (count_approval_gate_steps) is None else (count_approval_gate_steps) > (0)) and (count_gates_without_human_approver == 0))

def calc_workflows_cq4_satisfied(count_derivation_links, count_workflow_artifacts):
    """
    CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
    
    Formula: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1
    """
    return (count_derivation_links == ((count_workflow_artifacts) or 0) - ((1) or 0))

def calc_workflows_cq6_satisfied(count_roles_with_escalation_violation):
    """
    CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
    
    Formula: ={{CountRolesWithEscalationViolation}} = 0
    """
    return (count_roles_with_escalation_violation == 0)

def calc_workflows_cq8_satisfied(count_unconsumed_datasets):
    """
    CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
    
    Formula: ={{CountUnconsumedDatasets}} = 0
    """
    return (count_unconsumed_datasets == 0)

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
    return (False if (months_since_modified) is None or (staleness_threshold_months) is None else (months_since_modified) > (staleness_threshold_months))

def calc_workflows_cq1_satisfied(count_of_precedence_closure_pairs, count_of_non_proposed_steps):
    """
    CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
    
    Formula: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2
    """
    return (count_of_precedence_closure_pairs == ((((count_of_non_proposed_steps) or 0) * ((((count_of_non_proposed_steps) or 0) - ((1) or 0)) or 0)) or 0) / ((2) or 0))

def calc_workflows_cq3_satisfied(has_consistency_violation):
    """
    CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
    
    Formula: =NOT({{HasConsistencyViolation}})
    """
    return (has_consistency_violation is not True)

def calc_workflows_cq7_satisfied(involves_engineering_and_legal):
    """
    CQ7 satisfied: the workflow involves BOTH Engineering-owned and Legal-owned steps. Reads the existing InvolvesEngineeringAndLegal boolean.
    
    Formula: ={{InvolvesEngineeringAndLegal}}
    """
    return involves_engineering_and_legal

# Level 3

def calc_workflows_is_stale_and_has_ai_agent(is_stale, has_ai_agent_step):
    """
    The article's headline business question, as one boolean: a workflow that is BOTH stale (not reviewed in 12 months) AND has an AI-executed step — the highest compliance risk. Joins the metadata layer (dct:modified) with the accountability layer (filledBy → AIAgent) the way the closing SPARQL demo does, but as a single derived column.
    
    Formula: =AND({{IsStale}}, {{HasAIAgentStep}})
    """
    return ((is_stale is True) and (has_ai_agent_step is True))

def calc_workflows_cq5_satisfied(is_stale):
    """
    CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
    
    Formula: =NOT({{IsStale}})
    """
    return (is_stale is not True)


def compute_workflows_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Workflows.
    
    Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_workflows_relative_path(result.get('workflow_id'))
    result['name'] = calc_workflows_name(result.get('display_name'))
    result['has_more_than1_step'] = calc_workflows_has_more_than1_step(result.get('count_of_non_proposed_steps'))
    result['has_consistency_violation'] = calc_workflows_has_consistency_violation(result.get('count_approval_consistency_violations'))
    result['has_ai_agent_step'] = calc_workflows_has_ai_agent_step(result.get('count_ai_steps'))
    result['months_since_modified'] = calc_workflows_months_since_modified(result.get('modified'))
    result['involves_engineering_and_legal'] = calc_workflows_involves_engineering_and_legal(result.get('count_engineering_owned_steps'), result.get('count_legal_owned_steps'))
    result['count_of_precedence_closure_pairs'] = calc_workflows_count_of_precedence_closure_pairs(result.get('count_asserted_precedence_pairs'), result.get('count_inferred_precedence_pairs'))
    result['cq2_satisfied'] = calc_workflows_cq2_satisfied(result.get('count_approval_gate_steps'), result.get('count_gates_without_human_approver'))
    result['cq4_satisfied'] = calc_workflows_cq4_satisfied(result.get('count_derivation_links'), result.get('count_workflow_artifacts'))
    result['cq6_satisfied'] = calc_workflows_cq6_satisfied(result.get('count_roles_with_escalation_violation'))
    result['cq8_satisfied'] = calc_workflows_cq8_satisfied(result.get('count_unconsumed_datasets'))

    # Level 2 calculations
    result['iri'] = calc_workflows_iri(result.get('relative_path'))
    result['is_stale'] = calc_workflows_is_stale(result.get('months_since_modified'), result.get('staleness_threshold_months'))
    result['cq1_satisfied'] = calc_workflows_cq1_satisfied(result.get('count_of_precedence_closure_pairs'), result.get('count_of_non_proposed_steps'))
    result['cq3_satisfied'] = calc_workflows_cq3_satisfied(result.get('has_consistency_violation'))
    result['cq7_satisfied'] = calc_workflows_cq7_satisfied(result.get('involves_engineering_and_legal'))

    # Level 3 calculations
    result['is_stale_and_has_ai_agent'] = calc_workflows_is_stale_and_has_ai_agent(result.get('is_stale'), result.get('has_ai_agent_step'))
    result['cq5_satisfied'] = calc_workflows_cq5_satisfied(result.get('is_stale'))

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

def calc_workflow_steps_relative_path(parent_path, workflow_step_id):
    """
    Stable, DAG-derived location: this row nests under its Workflows parent. Concatenates the parent's path (ParentPath) with '/steps/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
    
    Formula: ={{ParentPath}} & "/steps/" & {{WorkflowStepId}}
    """
    return (str(parent_path or "") + '/steps/' + str(workflow_step_id or ""))

def calc_workflow_steps_name(display_name):
    """Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")"""
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

def calc_workflow_steps_inferred_sequence_position(preceding_step_count):
    """
    The step's ordinal position INFERRED purely from the StepPrecedence edges: 1 + PrecedingStepCount (one plus the number of steps that transitively precede it in vw_step_precedence_closure). On the linear Production Deployment chain: 1,2,3,4,5 — no integer is typed; it is a projection of the asserted ordering edges. This is the DEFAULT position; SequencePositionOverride can pin a different value where the inference is ambiguous (e.g. a branch produces ties). Maps to ntwf:inferredSequencePosition (an effortless extension of the article's ordering).
    
    Formula: ={{PrecedingStepCount}} + 1
    """
    return ((preceding_step_count) or 0) + ((1) or 0)

def calc_workflow_steps_executing_agent_type(executing_human_agent, executing_ai_agent, executing_automated_pipeline):
    """
    Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
    
    Formula: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if (not (executing_human_agent is None or executing_human_agent == "")) else ('AIAgent' if (not (executing_ai_agent is None or executing_ai_agent == "")) else ('AutomatedPipeline' if (not (executing_automated_pipeline is None or executing_automated_pipeline == "")) else '')))

def calc_workflow_steps_is_executed_by_ai(executing_ai_agent):
    """
    TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
    
    Formula: =NOT(ISBLANK({{ExecutingAIAgent}}))
    """
    return (not (executing_ai_agent is None or executing_ai_agent == ""))

def calc_workflow_steps_is_executed_by_human(executing_human_agent):
    """
    TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
    
    Formula: =NOT(ISBLANK({{ExecutingHumanAgent}}))
    """
    return (not (executing_human_agent is None or executing_human_agent == ""))

def calc_workflow_steps_is_approval_gate(approval_gate):
    """
    TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
    
    Formula: =NOT(ISBLANK({{ApprovalGate}}))
    """
    return (not (approval_gate is None or approval_gate == ""))

def calc_workflow_steps_approval_consistency_violation(requires_human_approval, executing_human_agent):
    """
    Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
    
    Formula: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}}))
    """
    return ((requires_human_approval is True) and ((executing_human_agent is None or executing_human_agent == "") is True))

def calc_workflow_steps_approval_is_human_filled(requires_human_approval, executing_human_agent):
    """
    Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
    
    Formula: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE)
    """
    return ((not (executing_human_agent is None or executing_human_agent == "")) if requires_human_approval else True)

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

# Level 2

def calc_workflow_steps_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

def calc_workflow_steps_sequence_position(sequence_position_override, inferred_sequence_position):
    """
    The effective ordinal position used everywhere (views, UI, competency questions): the hand-asserted SequencePositionOverride when present, otherwise the edge-derived InferredSequencePosition. IF(SequencePositionOverride <> "", SequencePositionOverride, InferredSequencePosition). This is the honest resolution of the two ways order can be stated: the inference is the default computed from the SSoT (the StepPrecedence edges), and an explicit override only overrides — never a silent guess. On the Production Deployment chain all overrides are null, so this equals InferredSequencePosition = 1,2,3,4,5. Maps to ntwf:sequencePosition for consumers.
    
    Formula: =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, {{InferredSequencePosition}})
    """
    return (sequence_position_override if (sequence_position_override != '') else inferred_sequence_position)


def compute_workflow_steps_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WorkflowSteps.
    
    Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_workflow_steps_relative_path(result.get('parent_path'), result.get('workflow_step_id'))
    result['name'] = calc_workflow_steps_name(result.get('display_name'))
    result['inferred_sequence_position'] = calc_workflow_steps_inferred_sequence_position(result.get('preceding_step_count'))
    result['executing_agent_type'] = calc_workflow_steps_executing_agent_type(result.get('executing_human_agent'), result.get('executing_ai_agent'), result.get('executing_automated_pipeline'))
    result['is_executed_by_ai'] = calc_workflow_steps_is_executed_by_ai(result.get('executing_ai_agent'))
    result['is_executed_by_human'] = calc_workflow_steps_is_executed_by_human(result.get('executing_human_agent'))
    result['is_approval_gate'] = calc_workflow_steps_is_approval_gate(result.get('approval_gate'))
    result['approval_consistency_violation'] = calc_workflow_steps_approval_consistency_violation(result.get('requires_human_approval'), result.get('executing_human_agent'))
    result['approval_is_human_filled'] = calc_workflow_steps_approval_is_human_filled(result.get('requires_human_approval'), result.get('executing_human_agent'))
    result['is_legal_owned'] = calc_workflow_steps_is_legal_owned(result.get('owning_department'))
    result['is_engineering_owned'] = calc_workflow_steps_is_engineering_owned(result.get('owning_department'))

    # Level 2 calculations
    result['iri'] = calc_workflow_steps_iri(result.get('relative_path'))
    result['sequence_position'] = calc_workflow_steps_sequence_position(result.get('sequence_position_override'), result.get('inferred_sequence_position'))

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

def calc_approval_gates_relative_path(parent_path, approval_gate_id):
    """
    Stable, DAG-derived location: this row nests under its WorkflowSteps parent. Concatenates the parent's path (ParentPath) with '/approval-gates/' + this row's primary key. The DAG performs the recursion — one hop per table via ParentPath — so the full ancestry is encoded without a recursive formula. Unique by construction.
    
    Formula: ={{ParentPath}} & "/approval-gates/" & {{ApprovalGateId}}
    """
    return (str(parent_path or "") + '/approval-gates/' + str(approval_gate_id or ""))

def calc_approval_gates_name(display_name):
    """Formula: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-")"""
    return ((((display_name or "").lower()) or "").replace(' ', '-'))

def calc_approval_gates_has_human_approver(gate_approver_human):
    """
    TRUE iff this approval gate resolves to a human approver (its gate role is filled by a HumanAgent). Rolls up into Workflows.CountGatesWithoutHumanApprover, which CQ2's satisfaction reads.
    
    Formula: =NOT(ISBLANK({{GateApproverHuman}}))
    """
    return (not (gate_approver_human is None or gate_approver_human == ""))

# Level 2

def calc_approval_gates_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_approval_gates_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ApprovalGates.
    
    Table: ApprovalGates. The NTWF ApprovalGate class — rdfs:subClassOf WorkflowStep. Modeled as a class-table-inheritance subtype: each gate row shares identity with exactly one WorkflowStep (via the WorkflowStep 1:1 FK) and carries only the gate-specific attribute, escalationThresholdHours. The step it specializes keeps the common attributes (requiresHumanApproval, assigned role, etc.). This preserves the article's double-typing — a gate IS a step — without collapsing two DAG nodes into one.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_approval_gates_relative_path(result.get('parent_path'), result.get('approval_gate_id'))
    result['name'] = calc_approval_gates_name(result.get('display_name'))
    result['has_human_approver'] = calc_approval_gates_has_human_approver(result.get('gate_approver_human'))

    # Level 2 calculations
    result['iri'] = calc_approval_gates_iri(result.get('relative_path'))

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

def calc_step_precedence_name(from_step, to_step):
    """
    Human-readable edge label derived from its endpoints. Mirrors the FromStep -> ToStep direction.
    
    Formula: ={{FromStep}} & " -> " & {{ToStep}}
    """
    return (str(from_step or "") + ' -> ' + str(to_step or ""))

# Level 2

def calc_step_precedence_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_step_precedence_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepPrecedence.
    
    Table: StepPrecedence. The NTWF ntwf:precedesStep ordering relationship, modeled as a first-class step-to-step junction. Each row is one directed edge: FromStep precedes ToStep. ntwf:precedesStep is an owl:TransitiveProperty — the four asserted edges (1->2, 2->3, 3->4, 4->5) imply the full closure of ten ordering pairs (including 1->5, which is never asserted). Each edge is a first-class node in the DAG, never a 'helper' integer.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_step_precedence_relative_path(result.get('parent_path'), result.get('step_precedence_id'))
    result['name'] = calc_step_precedence_name(result.get('from_step'), result.get('to_step'))

    # Level 2 calculations
    result['iri'] = calc_step_precedence_iri(result.get('relative_path'))

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

def calc_roles_filled_by_arm_count(filled_by_human_agent, filled_by_ai_agent, filled_by_automated_pipeline):
    """
    Number of polymorphic ntwf:filledBy arms set on this role (of FilledByHumanAgent / FilledByAIAgent / FilledByAutomatedPipeline). Should always be exactly 1 — mirroring filledBy being functional and the three agent types being mutually disjoint.
    
    Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0)
    """
    return (((((1 if (not (filled_by_human_agent is None or filled_by_human_agent == "")) else 0)) or 0) + (((1 if (not (filled_by_ai_agent is None or filled_by_ai_agent == "")) else 0)) or 0)) or 0) + (((1 if (not (filled_by_automated_pipeline is None or filled_by_automated_pipeline == "")) else 0)) or 0)

def calc_roles_filler_type(filled_by_human_agent, filled_by_ai_agent, filled_by_automated_pipeline):
    """
    Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
    
    Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if (not (filled_by_human_agent is None or filled_by_human_agent == "")) else ('AIAgent' if (not (filled_by_ai_agent is None or filled_by_ai_agent == "")) else ('AutomatedPipeline' if (not (filled_by_automated_pipeline is None or filled_by_automated_pipeline == "")) else '')))

def calc_roles_escalation_violation(fills_approval_gate, delegates_to):
    """
    Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
    
    Formula: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}}))
    """
    return ((False if (fills_approval_gate) is None else (fills_approval_gate) > (0)) and ((delegates_to is None or delegates_to == "") is True))

# Level 2

def calc_roles_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

def calc_roles_has_exactly_one_filler(filled_by_arm_count):
    """
    Disjointness/functional witness: TRUE iff exactly one filledBy arm is set. The three agent classes are owl:disjointWith one another and ntwf:filledBy is functional, so a clean ABox has this TRUE for every role. Setting two arms (a role filled by both a human and an AI) is the Suite-4 disjointness violation — here it flips this to FALSE.
    
    Formula: ={{FilledByArmCount}} = 1
    """
    return (filled_by_arm_count == 1)


def compute_roles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Roles.
    
    Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_roles_relative_path(result.get('role_id'))
    result['name'] = calc_roles_name(result.get('display_name'))
    result['filled_by_arm_count'] = calc_roles_filled_by_arm_count(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    result['filler_type'] = calc_roles_filler_type(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    result['escalation_violation'] = calc_roles_escalation_violation(result.get('fills_approval_gate'), result.get('delegates_to'))

    # Level 2 calculations
    result['iri'] = calc_roles_iri(result.get('relative_path'))
    result['has_exactly_one_filler'] = calc_roles_has_exactly_one_filler(result.get('filled_by_arm_count'))

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

def calc_role_assignments_relative_path(parent_path, role_assignment_id):
    """
    Stable, DAG-derived location: this assignment nests under its Role parent. Concatenates the parent's path (ParentPath) with '/assignments/' + this row's primary key. Unique by construction.
    
    Formula: ={{ParentPath}} & "/assignments/" & {{RoleAssignmentId}}
    """
    return (str(parent_path or "") + '/assignments/' + str(role_assignment_id or ""))

def calc_role_assignments_name(role, valid_from, valid_to):
    """
    Human-readable label for this assignment period: the role and the validity window.
    
    Formula: ={{Role}} & " [" & {{ValidFrom}} & " -> " & IF(ISBLANK({{ValidTo}}), "open", {{ValidTo}}) & "]" 
    """
    return (str(role or "") + ' [' + str(valid_from or "") + ' -> ' + str(('open' if (valid_to is None or valid_to == "") else valid_to) if ('open' if (valid_to is None or valid_to == "") else valid_to) is not None else "") + ']')

def calc_role_assignments_filler_type(filled_by_human_agent, filled_by_ai_agent, filled_by_automated_pipeline):
    """
    Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
    
    Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if (not (filled_by_human_agent is None or filled_by_human_agent == "")) else ('AIAgent' if (not (filled_by_ai_agent is None or filled_by_ai_agent == "")) else ('AutomatedPipeline' if (not (filled_by_automated_pipeline is None or filled_by_automated_pipeline == "")) else '')))

def calc_role_assignments_is_current(valid_to):
    """
    TRUE iff this is the live binding (ValidTo is blank). The set of IsCurrent rows reproduces exactly the current Roles.FilledBy* values; the rest are retained history. The old triple is never deleted — closed rows stay, only IsCurrent flips.
    
    Formula: =ISBLANK({{ValidTo}})
    """
    return (valid_to is None or valid_to == "")

def calc_role_assignments_was_active_as_of_audit_date(valid_from, valid_to):
    """
    NTWF's signature temporal query: 'which agent was executing this step on March 1, 2026?'. TRUE iff this binding's validity period contains 2026-03-01 (ValidFrom <= the date AND (ValidTo blank OR ValidTo > the date)). ISO dates compare lexically. The single row that is TRUE for a given role names the agent active on the audit date — answerable only because history is retained.
    
    Formula: =AND({{ValidFrom}} <= "2026-03-01", OR(ISBLANK({{ValidTo}}), {{ValidTo}} > "2026-03-01"))
    """
    return ((False if (valid_from) is None else (valid_from) <= ('2026-03-01')) and (((valid_to is None or valid_to == "") is True) or (False if (valid_to) is None else (valid_to) > ('2026-03-01'))))

# Level 2

def calc_role_assignments_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))

def calc_role_assignments_is_agent_type_change(prior_filler_type, filler_type):
    """
    TRUE iff this assignment changed the agent CLASS of the role (PriorFillerType set and different from FillerType). NTWF distinguishes a plain personnel/model swap (same class) from an agent-type transition, which carries compliance weight.
    
    Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} <> {{FillerType}})
    """
    return ((not (prior_filler_type is None or prior_filler_type == "")) and (prior_filler_type != filler_type))

def calc_role_assignments_requires_compliance_audit(prior_filler_type, filler_type):
    """
    Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
    
    Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent")
    """
    return ((not (prior_filler_type is None or prior_filler_type == "")) and (prior_filler_type == 'AIAgent') and (filler_type == 'HumanAgent'))


def compute_role_assignments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleAssignments.
    
    Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_role_assignments_relative_path(result.get('parent_path'), result.get('role_assignment_id'))
    result['name'] = calc_role_assignments_name(result.get('role'), result.get('valid_from'), result.get('valid_to'))
    result['filler_type'] = calc_role_assignments_filler_type(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    result['is_current'] = calc_role_assignments_is_current(result.get('valid_to'))
    result['was_active_as_of_audit_date'] = calc_role_assignments_was_active_as_of_audit_date(result.get('valid_from'), result.get('valid_to'))

    # Level 2 calculations
    result['iri'] = calc_role_assignments_iri(result.get('relative_path'))
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

def calc_datasets_is_consumed(consumed_by_steps):
    """
    TRUE iff some workflow step consumes this dataset (ConsumedBySteps is set). Rolls up into Workflows.CountUnconsumedDatasets, which CQ8's satisfaction reads.
    
    Formula: =NOT(ISBLANK({{ConsumedBySteps}}))
    """
    return (not (consumed_by_steps is None or consumed_by_steps == ""))

# Level 2

def calc_datasets_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_datasets_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Datasets.
    
    DCAT datasets consumed by workflow steps. The NTWF mapping of dcat:Dataset. Kept separate from WorkflowArtifacts to preserve DCAT metadata semantics (dcat:Dataset vs. prov:Entity). Answers CQ8: 'What datasets does the review consume, and which AI processed them?'
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_datasets_relative_path(result.get('dataset_id'))
    result['is_consumed'] = calc_datasets_is_consumed(result.get('consumed_by_steps'))

    # Level 2 calculations
    result['iri'] = calc_datasets_iri(result.get('relative_path'))

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

def calc_workflow_artifacts_producing_agent_type(attributed_to_human_agent, attributed_to_ai_agent, attributed_to_automated_pipeline):
    """
    Which disjoint agent class produced this artifact (HumanAgent / AIAgent / AutomatedPipeline), from whichever prov:wasAttributedTo arm is set. Lets CQ4 report which kind of agent each artifact in the lineage came from.
    
    Formula: =IF(NOT(ISBLANK({{AttributedToHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{AttributedToAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{AttributedToAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if (not (attributed_to_human_agent is None or attributed_to_human_agent == "")) else ('AIAgent' if (not (attributed_to_ai_agent is None or attributed_to_ai_agent == "")) else ('AutomatedPipeline' if (not (attributed_to_automated_pipeline is None or attributed_to_automated_pipeline == "")) else '')))

def calc_workflow_artifacts_has_derivation_parent(derived_from_artifact):
    """
    TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
    
    Formula: =NOT(ISBLANK({{DerivedFromArtifact}}))
    """
    return (not (derived_from_artifact is None or derived_from_artifact == ""))

def calc_workflow_artifacts_has_producing_workflow(produced_by_workflow):
    """
    TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
    
    Formula: =NOT(ISBLANK({{ProducedByWorkflow}}))
    """
    return (not (produced_by_workflow is None or produced_by_workflow == ""))

# Level 2

def calc_workflow_artifacts_iri(relative_path):
    """
    Opaque stable identifier (the dash-form of RelativePath). Because RelativePath has no leading slash, this is a clean SUBSTITUTE of '/' for '-'. The OWL transpiler mints each individual's IRI from this value (erb:<Iri>), so identity is path-derived and globally unique — no cross-table primary-key collisions.
    
    Formula: =SUBSTITUTE({{RelativePath}}, "/", "-")
    """
    return ((relative_path or "").replace('/', '-'))


def compute_workflow_artifacts_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WorkflowArtifacts.
    
    Artifacts produced and consumed by workflow steps. The NTWF WorkflowArtifact class — prov:Entity + schema:CreativeWork. The DerivedFromArtifact self-FK encodes the prov:wasDerivedFrom provenance chain; ProducedByStep maps prov:wasGeneratedBy; the AttributedTo* arms map prov:wasAttributedTo to the responsible agent.
    """
    result = dict(record)

    # Level 1 calculations
    result['relative_path'] = calc_workflow_artifacts_relative_path(result.get('parent_path'), result.get('artifact_id'))
    result['producing_agent_type'] = calc_workflow_artifacts_producing_agent_type(result.get('attributed_to_human_agent'), result.get('attributed_to_ai_agent'), result.get('attributed_to_automated_pipeline'))
    result['has_derivation_parent'] = calc_workflow_artifacts_has_derivation_parent(result.get('derived_from_artifact'))
    result['has_producing_workflow'] = calc_workflow_artifacts_has_producing_workflow(result.get('produced_by_workflow'))

    # Level 2 calculations
    result['iri'] = calc_workflow_artifacts_iri(result.get('relative_path'))

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
    # The MATCH key is a field on the record being computed, so the rulebook
    # writes it bare ({{WorkflowStep}}); an explicit table prefix
    # (ApprovalGates!{{WorkflowStep}}) means the same thing. Both spellings
    # must parse — requiring the prefix silently nulled every bare lookup.
    pattern = (
        r"=\s*INDEX\(\s*(\w+)!\{\{(\w+)\}\}\s*,"
        r"\s*MATCH\(\s*(?:\w+!)?\{\{(\w+)\}\}\s*,"
        r"\s*(\w+)!\{\{(\w+)\}\}\s*,\s*0\s*\)\s*\)"
    )
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


def parse_countifs_literal_formula(formula: str) -> tuple:
    """Parse =COUNTIFS(Table!{{Field}}, TRUE()) / FALSE().

    Distinct from parse_countifs_formula, whose second argument is another
    table's field rather than a literal. Returns (table, field, bool).
    """
    pattern = r"=COUNTIFS\((\w+)!\{\{(\w+)\}\},\s*(TRUE|FALSE)\(\)\)"
    match = re.match(pattern, formula)
    if match:
        return (match.group(1), match.group(2), match.group(3) == 'TRUE')
    return (None, None, None)


def count_closure_rows(closure_rows: list, column: str, expected) -> int:
    """Count materialized closure rows whose column equals expected."""
    return sum(1 for row in closure_rows if row.get(column) == expected)


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


# =============================================================================
# TRANSITIVE CLOSURE ENGINE (PYTHON SIMULATOR — DO NOT CALL FROM OTHER SUBSTRATES)
# =============================================================================


def compute_closure_relation(rows: list, to_field: str,
                             pk_field: str = None, from_field: str = None) -> list:
    """Cycle-safe transitive closure, matching Postgres vw_<entity>_closure.

    Two edge shapes: pass from_field for an edge/junction table, or pk_field
    for a self-referential FK on the entity's own rows.

    Returns dicts of from_id, to_id, hop_distance (shortest derivation) and
    is_inferred (TRUE iff no directly-asserted hop-1 edge states the pair).
    A NULL or empty-string endpoint is not an edge — the transpiler stores
    absent relationships as '' rather than NULL, so both must be excluded.
    """
    source_field = from_field or pk_field
    if source_field is None:
        raise ValueError("compute_closure_relation requires from_field or pk_field")

    edges = []
    for row in rows:
        src = row.get(source_field)
        dst = row.get(to_field)
        if src is None or src == '' or dst is None or dst == '':
            continue
        edges.append((src, dst))

    if not edges:
        return []

    asserted = set(edges)
    adjacency = {}
    for src, dst in edges:
        adjacency.setdefault(src, []).append(dst)

    shortest = {}
    for origin in {src for src, _ in edges}:
        # BFS keeps the first arrival shortest; the path set makes it cycle-safe.
        frontier = [(origin, (origin,))]
        hop = 0
        while frontier:
            hop += 1
            next_frontier = []
            for node, path in frontier:
                for neighbor in adjacency.get(node, []):
                    pair = (origin, neighbor)
                    if pair not in shortest:
                        shortest[pair] = hop
                    if neighbor not in path:
                        next_frontier.append((neighbor, path + (neighbor,)))
            frontier = next_frontier

    return [
        {
            'from_id': from_id,
            'to_id': to_id,
            'hop_distance': hop_distance,
            'is_inferred': (from_id, to_id) not in asserted,
        }
        for (from_id, to_id), hop_distance in sorted(shortest.items())
    ]


def compute_closures(rulebook: dict, project_root: Path) -> dict:
    """Materialize every closure field in the rulebook as a pseudo-table.

    Aggregations address these by view name, e.g.
    =COUNTIFS(vw_step_precedence_closure!{{IsInferred}}, TRUE()) — so the
    result is keyed by vw_<entity>_closure and joins the related-data lookup
    path alongside real tables.
    """
    from orchestration.shared import (
        discover_entities,
        discover_primary_key,
        get_closure_fields,
        closure_view_name,
    )

    materialized = {}

    for entity_name in discover_entities(rulebook):
        schema = get_entity_schema(rulebook, entity_name)
        for field in get_closure_fields(schema):
            edge_table = field.get('EdgeTable')
            to_column = field.get('ToColumn')
            if not to_column:
                continue

            to_field = to_snake_case(to_column)

            if edge_table and field.get('FromColumn'):
                source_entity = edge_table
                source_rows = load_related_data(project_root, edge_table)
                kwargs = {'from_field': to_snake_case(field['FromColumn'])}
            else:
                source_entity = entity_name
                source_rows = load_related_data(project_root, entity_name)
                kwargs = {'pk_field': to_snake_case(
                    discover_primary_key(rulebook, entity_name))}

            materialized[closure_view_name(source_entity)] = compute_closure_relation(
                source_rows, to_field=to_field, **kwargs)

    return materialized


def compute_aggregations(records: list, entity_name: str, rulebook: dict, project_root: Path,
                         closures: dict = None) -> list:
    """COUNTIFS / SUMIFS aggregation interpreter. PYTHON SIMULATOR ONLY."""
    schema = get_entity_schema(rulebook, entity_name)
    agg_fields = get_aggregation_fields(schema)

    if not agg_fields:
        return records

    related_data_cache = {}
    closures = closures or {}

    for field in agg_fields:
        field_name = field.get("name")
        formula = field.get("formula", "")
        snake_field_name = to_snake_case(field_name)

        # COUNTIFS with a literal criteria counts rows of the whole target
        # table, so the count is the same for every record. The target is
        # either a materialized closure or an ordinary table.
        literal_table, literal_column, literal = parse_countifs_literal_formula(formula)
        if literal_table:
            if literal_table in closures:
                target_rows = closures[literal_table]
            else:
                if literal_table not in related_data_cache:
                    related_data_cache[literal_table] = load_related_data(
                        project_root, literal_table)
                target_rows = related_data_cache[literal_table]
            count = count_closure_rows(
                target_rows, to_snake_case(literal_column), literal)
            for record in records:
                record[snake_field_name] = count
            continue

        related_table, lookup_field, match_field = parse_countifs_formula(formula)

        if related_table in closures:
            closure_rows = closures[related_table]
            snake_lookup_field = to_snake_case(lookup_field)
            snake_match_field = to_snake_case(match_field)
            for record in records:
                record[snake_field_name] = count_closure_rows(
                    closure_rows, snake_lookup_field, record.get(snake_match_field))
            continue

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
