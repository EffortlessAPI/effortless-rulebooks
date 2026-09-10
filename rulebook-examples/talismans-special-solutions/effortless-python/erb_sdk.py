"""
ERB SDK (GENERATED - DO NOT EDIT)
=================================
Generated from: effortless-rulebook/talisman-s-special-solutions-rulebook.json

A calc_<table>_<field>() function per calculated field, a
compute_<table>_fields(record) per table, and the table registry main.py
runs. Every formula is compiled; lookups and aggregations are compiled to
the specs in ERB_TABLES, which erb_runtime.erb_run computes over the whole
dataset. Nothing here parses a formula or reads the rulebook.
"""

import erb_runtime as _erb


# =============================================================================
# WORKFLOWS
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
    return _erb.erb_cmp(count_of_non_proposed_steps, '>', 1)

def calc_workflows_has_consistency_violation(count_approval_consistency_violations):
    """
    TRUE iff at least one step breaks the human-approval consistency rule (CountApprovalConsistencyViolations > 0). The boolean witness of model integrity: a clean ABox holds it FALSE. This is what makes a broken rule a first-class input to the compliance verdict — a workflow with any consistency violation cannot be COMPLIANT.
    
    Formula: ={{CountApprovalConsistencyViolations}} > 0
    """
    return _erb.erb_cmp(count_approval_consistency_violations, '>', 0)

def calc_workflows_has_ai_agent_step(count_ai_steps):
    """
    TRUE iff at least one step in this workflow is executed by an AIAgent. The structural half of the article's business payoff query.
    
    Formula: ={{CountAISteps}} > 0
    """
    return _erb.erb_cmp(count_ai_steps, '>', 0)

def calc_workflows_months_since_modified(modified):
    """
    Whole months since this workflow was last modified (dct:modified), measured live against NOW(). Drives CQ5 staleness. NOW() is seeded deterministically during conformance so test answers stay stable.
    
    Formula: =DATETIME_DIFF(NOW(), {{Modified}}, "months")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(_erb.erb_now(), modified, 'months'))

def calc_workflows_involves_engineering_and_legal(count_engineering_owned_steps, count_legal_owned_steps):
    """
    TRUE iff this workflow has at least one Engineering-owned step AND at least one Legal-owned step. Answers CQ7 ('which workflows involve both engineering and legal') as a single boolean — the Production Deployment workflow qualifies (4 Engineering steps + 1 Legal step).
    
    Formula: =AND({{CountEngineeringOwnedSteps}} > 0, {{CountLegalOwnedSteps}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(count_engineering_owned_steps, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(count_legal_owned_steps, '>', 0)))

def calc_workflows_count_of_precedence_closure_pairs(count_asserted_precedence_pairs, count_inferred_precedence_pairs):
    """
    Total number of step-ordering pairs in the transitive closure of ntwf:precedesStep = asserted (4) + inferred (6) = 10. The article's headline closure cardinality, witnessing that the 4 asserted edges over a 5-step chain close to all 10 (i<j) pairs. Computed as CountAssertedPrecedencePairs + CountInferredPrecedencePairs so the total is provably the sum of the two halves, not a separate unconditional view count that could silently drift from them.
    
    Formula: ={{CountAssertedPrecedencePairs}} + {{CountInferredPrecedencePairs}}
    """
    return _erb.erb_integer(_erb.erb_add(count_asserted_precedence_pairs, count_inferred_precedence_pairs))

def calc_workflows_cq2_satisfied(count_approval_gate_steps, count_gates_without_human_approver):
    """
    CQ2 satisfied: the workflow has an approval gate AND every gate resolves to a human approver. Derived from the gate->role->filler chain; no hardcoded approver name.
    
    Formula: =AND({{CountApprovalGateSteps}} > 0, {{CountGatesWithoutHumanApprover}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(count_approval_gate_steps, '>', 0)), _erb.erb_bool3(_erb.erb_eq(count_gates_without_human_approver, 0)))

def calc_workflows_cq4_satisfied(count_derivation_links, count_workflow_artifacts):
    """
    CQ4 satisfied: the wasDerivedFrom provenance chain is intact — every artifact but the single origin has a derivation parent. Structural; breaks the instant any derivation edge is cut.
    
    Formula: ={{CountDerivationLinks}} = {{CountWorkflowArtifacts}} - 1
    """
    return _erb.erb_eq(count_derivation_links, _erb.erb_sub(count_workflow_artifacts, 1))

def calc_workflows_cq6_satisfied(count_roles_with_escalation_violation):
    """
    CQ6 satisfied: no gate-owning role escalates to nobody — every approval gate has a complete escalation path. Uses the model's native EscalationViolation invariant; 'the top' is the delegation apex, derived, not a hardcoded CTO name.
    
    Formula: ={{CountRolesWithEscalationViolation}} = 0
    """
    return _erb.erb_eq(count_roles_with_escalation_violation, 0)

def calc_workflows_cq8_satisfied(count_unconsumed_datasets):
    """
    CQ8 satisfied: every dataset the workflow declares is actually consumed by a step. Flips when a dataset is detached.
    
    Formula: ={{CountUnconsumedDatasets}} = 0
    """
    return _erb.erb_eq(count_unconsumed_datasets, 0)

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
    return _erb.erb_cmp(months_since_modified, '>', _erb.erb_nullif(staleness_threshold_months))

def calc_workflows_cq1_satisfied(count_of_precedence_closure_pairs, count_of_non_proposed_steps):
    """
    CQ1 satisfied: the step-ordering closure is a TOTAL order — its pair count equals n*(n-1)/2 for n steps, so every pair of steps is comparable and 'the order' is well-defined. Purely structural; no asserted literal.
    
    Formula: ={{CountOfPrecedenceClosurePairs}} = {{CountOfNonProposedSteps}} * ({{CountOfNonProposedSteps}} - 1) / 2
    """
    return _erb.erb_eq(count_of_precedence_closure_pairs, _erb.erb_div(_erb.erb_mul(count_of_non_proposed_steps, _erb.erb_sub(count_of_non_proposed_steps, 1)), 2))

def calc_workflows_cq3_satisfied(has_consistency_violation):
    """
    CQ3 satisfied: the AI-vs-human assignment is consistent — no step that requires a human decision is executed by a non-human (no ApprovalConsistencyViolation). Derived from the model's own consistency invariant.
    
    Formula: =NOT({{HasConsistencyViolation}})
    """
    return _erb.erb_not(_erb.erb_bool3(has_consistency_violation))

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
    return _erb.erb_and(_erb.erb_bool3(is_stale), _erb.erb_bool3(has_ai_agent_step))

def calc_workflows_cq5_satisfied(is_stale):
    """
    CQ5 satisfied: the workflow's compliance docs are within the review policy (not stale). Reads the existing IsStale verdict; flips when the review age passes StalenessThresholdMonths.
    
    Formula: =NOT({{IsStale}})
    """
    return _erb.erb_not(_erb.erb_bool3(is_stale))


def compute_workflows_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Workflows.
    
    Table: Workflows. The NTWF Workflow class — prov:Plan + schema:CreativeWork. Each workflow has Dublin Core metadata (title, description, identifier, created, modified), a lifecycle status from the SKOS scheme, and a collection of WorkflowSteps (ntwf:hasStep).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['relative_path'] = calc_workflows_relative_path(result.get('workflow_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_workflows_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_more_than1_step'] = calc_workflows_has_more_than1_step(result.get('count_of_non_proposed_steps'))
    except Exception as _field_exc:
        result['has_more_than1_step'] = None
        result.setdefault('_erb_errors', {})['has_more_than1_step'] = str(_field_exc)
    try:
        result['has_consistency_violation'] = calc_workflows_has_consistency_violation(result.get('count_approval_consistency_violations'))
    except Exception as _field_exc:
        result['has_consistency_violation'] = None
        result.setdefault('_erb_errors', {})['has_consistency_violation'] = str(_field_exc)
    try:
        result['has_ai_agent_step'] = calc_workflows_has_ai_agent_step(result.get('count_ai_steps'))
    except Exception as _field_exc:
        result['has_ai_agent_step'] = None
        result.setdefault('_erb_errors', {})['has_ai_agent_step'] = str(_field_exc)
    try:
        result['months_since_modified'] = calc_workflows_months_since_modified(result.get('modified'))
    except Exception as _field_exc:
        result['months_since_modified'] = None
        result.setdefault('_erb_errors', {})['months_since_modified'] = str(_field_exc)
    try:
        result['involves_engineering_and_legal'] = calc_workflows_involves_engineering_and_legal(result.get('count_engineering_owned_steps'), result.get('count_legal_owned_steps'))
    except Exception as _field_exc:
        result['involves_engineering_and_legal'] = None
        result.setdefault('_erb_errors', {})['involves_engineering_and_legal'] = str(_field_exc)
    try:
        result['count_of_precedence_closure_pairs'] = calc_workflows_count_of_precedence_closure_pairs(result.get('count_asserted_precedence_pairs'), result.get('count_inferred_precedence_pairs'))
    except Exception as _field_exc:
        result['count_of_precedence_closure_pairs'] = None
        result.setdefault('_erb_errors', {})['count_of_precedence_closure_pairs'] = str(_field_exc)
    try:
        result['cq2_satisfied'] = calc_workflows_cq2_satisfied(result.get('count_approval_gate_steps'), result.get('count_gates_without_human_approver'))
    except Exception as _field_exc:
        result['cq2_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq2_satisfied'] = str(_field_exc)
    try:
        result['cq4_satisfied'] = calc_workflows_cq4_satisfied(result.get('count_derivation_links'), result.get('count_workflow_artifacts'))
    except Exception as _field_exc:
        result['cq4_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq4_satisfied'] = str(_field_exc)
    try:
        result['cq6_satisfied'] = calc_workflows_cq6_satisfied(result.get('count_roles_with_escalation_violation'))
    except Exception as _field_exc:
        result['cq6_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq6_satisfied'] = str(_field_exc)
    try:
        result['cq8_satisfied'] = calc_workflows_cq8_satisfied(result.get('count_unconsumed_datasets'))
    except Exception as _field_exc:
        result['cq8_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq8_satisfied'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_workflows_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)
    try:
        result['is_stale'] = calc_workflows_is_stale(result.get('months_since_modified'), result.get('staleness_threshold_months'))
    except Exception as _field_exc:
        result['is_stale'] = None
        result.setdefault('_erb_errors', {})['is_stale'] = str(_field_exc)
    try:
        result['cq1_satisfied'] = calc_workflows_cq1_satisfied(result.get('count_of_precedence_closure_pairs'), result.get('count_of_non_proposed_steps'))
    except Exception as _field_exc:
        result['cq1_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq1_satisfied'] = str(_field_exc)
    try:
        result['cq3_satisfied'] = calc_workflows_cq3_satisfied(result.get('has_consistency_violation'))
    except Exception as _field_exc:
        result['cq3_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq3_satisfied'] = str(_field_exc)
    try:
        result['cq7_satisfied'] = calc_workflows_cq7_satisfied(result.get('involves_engineering_and_legal'))
    except Exception as _field_exc:
        result['cq7_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq7_satisfied'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_stale_and_has_ai_agent'] = calc_workflows_is_stale_and_has_ai_agent(result.get('is_stale'), result.get('has_ai_agent_step'))
    except Exception as _field_exc:
        result['is_stale_and_has_ai_agent'] = None
        result.setdefault('_erb_errors', {})['is_stale_and_has_ai_agent'] = str(_field_exc)
    try:
        result['cq5_satisfied'] = calc_workflows_cq5_satisfied(result.get('is_stale'))
    except Exception as _field_exc:
        result['cq5_satisfied'] = None
        result.setdefault('_erb_errors', {})['cq5_satisfied'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WORKFLOWSTEPS
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
    return _erb.erb_integer(_erb.erb_add(preceding_step_count, 1))

def calc_workflow_steps_executing_agent_type(executing_human_agent, executing_ai_agent, executing_automated_pipeline):
    """
    Which of the three disjoint agent classes executes this step (HumanAgent / AIAgent / AutomatedPipeline), derived from whichever filledBy arm the assigned role has set. Answers the typing half of CQ3 ('which steps are executed by AI agents, and which require a human decision').
    
    Formula: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((executing_human_agent is None or executing_human_agent == "")))) else ('AIAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((executing_ai_agent is None or executing_ai_agent == "")))) else ('AutomatedPipeline' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((executing_automated_pipeline is None or executing_automated_pipeline == "")))) else '')))

def calc_workflow_steps_is_executed_by_ai(executing_ai_agent):
    """
    TRUE when this step's assigned role is filled by an AIAgent. Feeds CQ3 and the business payoff query (stale workflows with AI-executed steps).
    
    Formula: =NOT(ISBLANK({{ExecutingAIAgent}}))
    """
    return _erb.erb_not(_erb.erb_bool3((executing_ai_agent is None or executing_ai_agent == "")))

def calc_workflow_steps_is_executed_by_human(executing_human_agent):
    """
    TRUE when this step's assigned role is filled by a HumanAgent. Feeds CQ3's human-vs-AI step split.
    
    Formula: =NOT(ISBLANK({{ExecutingHumanAgent}}))
    """
    return _erb.erb_not(_erb.erb_bool3((executing_human_agent is None or executing_human_agent == "")))

def calc_workflow_steps_is_approval_gate(approval_gate):
    """
    TRUE when this step is specialized by an ApprovalGate subtype row (its ApprovalGate back-reference is set). An approval gate carries escalationThresholdHours and, when it stalls, activates the gate role's delegatesTo escalation chain. Rolls up into Roles.FillsApprovalGate, which marks the role that must have a complete escalation path (CQ6).
    
    Formula: =NOT(ISBLANK({{ApprovalGate}}))
    """
    return _erb.erb_not(_erb.erb_bool3((approval_gate is None or approval_gate == "")))

def calc_workflow_steps_approval_consistency_violation(requires_human_approval, executing_human_agent):
    """
    Detectable-error witness: TRUE iff this step requires human approval (RequiresHumanApproval) yet its assigned role is NOT filled by a HumanAgent. In the OWL ABox this is the rule that only a HumanAgent may fill a role on a requiresHumanApproval step; a clean ABox yields FALSE for every step. This is the relational equivalent of the Suite-4 disjointness/consistency check.
    
    Formula: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}}))
    """
    return _erb.erb_and((requires_human_approval is True), _erb.erb_bool3((executing_human_agent is None or executing_human_agent == "")))

def calc_workflow_steps_approval_is_human_filled(requires_human_approval, executing_human_agent):
    """
    Positive form of the human-only-gate rule: TRUE iff this step's human-approval obligation is satisfied — either the step does not require human approval (vacuously satisfied), or it does and its assigned role is filled by a HumanAgent. The clean Production Deployment ABox yields TRUE for every step. This is the affirmative complement of ApprovalConsistencyViolation: the two are always opposite when approval is required, and this one is additionally TRUE on steps that need no approval.
    
    Formula: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE)
    """
    return (_erb.erb_not(_erb.erb_bool3((executing_human_agent is None or executing_human_agent == ""))) if (requires_human_approval is True) else True)

def calc_workflow_steps_is_legal_owned(owning_department):
    """
    TRUE iff this step's owning department is Legal. Rolls up to CQ7's count of Legal-owned steps (exactly one in the Production Deployment workflow).
    
    Formula: ={{OwningDepartment}} = "ntwf-legal-dept" 
    """
    return _erb.erb_eq(owning_department, 'ntwf-legal-dept')

def calc_workflow_steps_is_engineering_owned(owning_department):
    """
    TRUE iff this step's owning department is Engineering. Rolls up to CQ7's Engineering-involvement check.
    
    Formula: ={{OwningDepartment}} = "ntwf-engineering" 
    """
    return _erb.erb_eq(owning_department, 'ntwf-engineering')

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
    return _erb.erb_integer((sequence_position_override if _erb.erb_bool3((not (sequence_position_override is None or sequence_position_override == ""))) else inferred_sequence_position))


def compute_workflow_steps_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WorkflowSteps.
    
    Table: WorkflowSteps. The NTWF WorkflowStep class — prov:Activity. Each step is first-class and individually addressable, belongs to one Workflow (ntwf:isStepOf), and is assigned to exactly one Role (ntwf:assignedRole). Step-to-step ordering is modeled in the StepPrecedence junction; the ApprovalGate subtype specializes a step via a 1:1 FK.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['relative_path'] = calc_workflow_steps_relative_path(result.get('parent_path'), result.get('workflow_step_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_workflow_steps_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['inferred_sequence_position'] = calc_workflow_steps_inferred_sequence_position(result.get('preceding_step_count'))
    except Exception as _field_exc:
        result['inferred_sequence_position'] = None
        result.setdefault('_erb_errors', {})['inferred_sequence_position'] = str(_field_exc)
    try:
        result['executing_agent_type'] = calc_workflow_steps_executing_agent_type(result.get('executing_human_agent'), result.get('executing_ai_agent'), result.get('executing_automated_pipeline'))
    except Exception as _field_exc:
        result['executing_agent_type'] = None
        result.setdefault('_erb_errors', {})['executing_agent_type'] = str(_field_exc)
    try:
        result['is_executed_by_ai'] = calc_workflow_steps_is_executed_by_ai(result.get('executing_ai_agent'))
    except Exception as _field_exc:
        result['is_executed_by_ai'] = None
        result.setdefault('_erb_errors', {})['is_executed_by_ai'] = str(_field_exc)
    try:
        result['is_executed_by_human'] = calc_workflow_steps_is_executed_by_human(result.get('executing_human_agent'))
    except Exception as _field_exc:
        result['is_executed_by_human'] = None
        result.setdefault('_erb_errors', {})['is_executed_by_human'] = str(_field_exc)
    try:
        result['is_approval_gate'] = calc_workflow_steps_is_approval_gate(result.get('approval_gate'))
    except Exception as _field_exc:
        result['is_approval_gate'] = None
        result.setdefault('_erb_errors', {})['is_approval_gate'] = str(_field_exc)
    try:
        result['approval_consistency_violation'] = calc_workflow_steps_approval_consistency_violation(result.get('requires_human_approval'), result.get('executing_human_agent'))
    except Exception as _field_exc:
        result['approval_consistency_violation'] = None
        result.setdefault('_erb_errors', {})['approval_consistency_violation'] = str(_field_exc)
    try:
        result['approval_is_human_filled'] = calc_workflow_steps_approval_is_human_filled(result.get('requires_human_approval'), result.get('executing_human_agent'))
    except Exception as _field_exc:
        result['approval_is_human_filled'] = None
        result.setdefault('_erb_errors', {})['approval_is_human_filled'] = str(_field_exc)
    try:
        result['is_legal_owned'] = calc_workflow_steps_is_legal_owned(result.get('owning_department'))
    except Exception as _field_exc:
        result['is_legal_owned'] = None
        result.setdefault('_erb_errors', {})['is_legal_owned'] = str(_field_exc)
    try:
        result['is_engineering_owned'] = calc_workflow_steps_is_engineering_owned(result.get('owning_department'))
    except Exception as _field_exc:
        result['is_engineering_owned'] = None
        result.setdefault('_erb_errors', {})['is_engineering_owned'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_workflow_steps_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)
    try:
        result['sequence_position'] = calc_workflow_steps_sequence_position(result.get('sequence_position_override'), result.get('inferred_sequence_position'))
    except Exception as _field_exc:
        result['sequence_position'] = None
        result.setdefault('_erb_errors', {})['sequence_position'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name', 'executing_agent_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPROVALGATES
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
    return _erb.erb_not(_erb.erb_bool3((gate_approver_human is None or gate_approver_human == "")))

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
    try:
        result['relative_path'] = calc_approval_gates_relative_path(result.get('parent_path'), result.get('approval_gate_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_approval_gates_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_human_approver'] = calc_approval_gates_has_human_approver(result.get('gate_approver_human'))
    except Exception as _field_exc:
        result['has_human_approver'] = None
        result.setdefault('_erb_errors', {})['has_human_approver'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_approval_gates_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPPRECEDENCE
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
    try:
        result['relative_path'] = calc_step_precedence_relative_path(result.get('parent_path'), result.get('step_precedence_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_step_precedence_name(result.get('from_step'), result.get('to_step'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_step_precedence_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLES
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
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add((1 if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_human_agent is None or filled_by_human_agent == "")))) else 0), (1 if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_ai_agent is None or filled_by_ai_agent == "")))) else 0)), (1 if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_automated_pipeline is None or filled_by_automated_pipeline == "")))) else 0)))

def calc_roles_filler_type(filled_by_human_agent, filled_by_ai_agent, filled_by_automated_pipeline):
    """
    Which disjoint agent class fills this role (HumanAgent / AIAgent / AutomatedPipeline), from whichever filledBy arm is set. Lets the delegation-chain query confirm CQ6's 'zero AI agents in the escalation chain'.
    
    Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_human_agent is None or filled_by_human_agent == "")))) else ('AIAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_ai_agent is None or filled_by_ai_agent == "")))) else ('AutomatedPipeline' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_automated_pipeline is None or filled_by_automated_pipeline == "")))) else '')))

def calc_roles_escalation_violation(fills_approval_gate, delegates_to):
    """
    Detectable-error witness: TRUE iff this role owns an approval gate (FillsApprovalGate > 0) yet has no escalation target (DelegatesTo is blank). A gate can stall and must be escalable up the delegatesTo chain; a gate role with no one to escalate to is a broken escalation. A clean ABox yields FALSE for every role. This is the role-side analogue of WorkflowSteps.ApprovalConsistencyViolation, and the witness CQ6's escalation chain depends on.
    
    Formula: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(fills_approval_gate, '>', 0)), _erb.erb_bool3((delegates_to is None or delegates_to == "")))

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
    return _erb.erb_eq(filled_by_arm_count, 1)


def compute_roles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Roles.
    
    Table: Roles. The NTWF Role class — a custom root with no adequate standard match, declared disjoint with WorkflowStep and WorkflowArtifact. Roles are the heart of Heuristic 2 (role-agent separation): WorkflowSteps point to Roles; Roles point to exactly one agent (human, AI, or pipeline) via the polymorphic filledBy relationship. When personnel or models change, one filledBy triple changes and the workflow structure is untouched.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['relative_path'] = calc_roles_relative_path(result.get('role_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_roles_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['filled_by_arm_count'] = calc_roles_filled_by_arm_count(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    except Exception as _field_exc:
        result['filled_by_arm_count'] = None
        result.setdefault('_erb_errors', {})['filled_by_arm_count'] = str(_field_exc)
    try:
        result['filler_type'] = calc_roles_filler_type(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    except Exception as _field_exc:
        result['filler_type'] = None
        result.setdefault('_erb_errors', {})['filler_type'] = str(_field_exc)
    try:
        result['escalation_violation'] = calc_roles_escalation_violation(result.get('fills_approval_gate'), result.get('delegates_to'))
    except Exception as _field_exc:
        result['escalation_violation'] = None
        result.setdefault('_erb_errors', {})['escalation_violation'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_roles_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)
    try:
        result['has_exactly_one_filler'] = calc_roles_has_exactly_one_filler(result.get('filled_by_arm_count'))
    except Exception as _field_exc:
        result['has_exactly_one_filler'] = None
        result.setdefault('_erb_errors', {})['has_exactly_one_filler'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name', 'filler_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLEASSIGNMENTS
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
    return (str(role or "") + ' [' + str(valid_from or "") + ' -> ' + str(('open' if _erb.erb_bool3((valid_to is None or valid_to == "")) else valid_to) if ('open' if _erb.erb_bool3((valid_to is None or valid_to == "")) else valid_to) is not None else "") + ']')

def calc_role_assignments_filler_type(filled_by_human_agent, filled_by_ai_agent, filled_by_automated_pipeline):
    """
    Which agent class filled the role during this period, derived from the three filler arms. Mirrors Roles.FillerType but for the historical binding.
    
    Formula: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", "")))
    """
    return ('HumanAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_human_agent is None or filled_by_human_agent == "")))) else ('AIAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_ai_agent is None or filled_by_ai_agent == "")))) else ('AutomatedPipeline' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((filled_by_automated_pipeline is None or filled_by_automated_pipeline == "")))) else '')))

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
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', '2026-03-01')), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', '2026-03-01')))))

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
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((prior_filler_type is None or prior_filler_type == "")))), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(prior_filler_type), filler_type)))

def calc_role_assignments_requires_compliance_audit(prior_filler_type, filler_type):
    """
    Changing the agent type of a step from ntwf:AIAgent to ntwf:HumanAgent is a data operation with compliance implications. TRUE iff this assignment took a previously AI-executed binding and reassigned it to a human — the exact transition NTWF governance says the audit record must capture (when + why).
    
    Formula: =AND(NOT(ISBLANK({{PriorFillerType}})), {{PriorFillerType}} = "AIAgent", {{FillerType}} = "HumanAgent")
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((prior_filler_type is None or prior_filler_type == "")))), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(prior_filler_type), 'AIAgent')), _erb.erb_bool3(_erb.erb_eq(filler_type, 'HumanAgent')))


def compute_role_assignments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleAssignments.
    
    Table: RoleAssignments. The temporal history of ntwf:filledBy. NTWF's change-management discipline requires that when a filledBy triple is updated the old triple is NOT deleted — it is timestamped and retained, or replaced with a versioned triple carrying a validity period. Each row is one filledBy binding with a ValidFrom / ValidTo validity period and the reason for the change, so that 'which agent was executing this step on March 1, 2026?' is answerable from the graph. The current binding on Roles.FilledBy* is the row whose ValidTo is blank (IsCurrent = TRUE); closed rows preserve provenance and chain of custody. This is the relational equivalent of the ontology's named-graph / versioned-triple retention practice.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['relative_path'] = calc_role_assignments_relative_path(result.get('parent_path'), result.get('role_assignment_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_role_assignments_name(result.get('role'), result.get('valid_from'), result.get('valid_to'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['filler_type'] = calc_role_assignments_filler_type(result.get('filled_by_human_agent'), result.get('filled_by_ai_agent'), result.get('filled_by_automated_pipeline'))
    except Exception as _field_exc:
        result['filler_type'] = None
        result.setdefault('_erb_errors', {})['filler_type'] = str(_field_exc)
    try:
        result['is_current'] = calc_role_assignments_is_current(result.get('valid_to'))
    except Exception as _field_exc:
        result['is_current'] = None
        result.setdefault('_erb_errors', {})['is_current'] = str(_field_exc)
    try:
        result['was_active_as_of_audit_date'] = calc_role_assignments_was_active_as_of_audit_date(result.get('valid_from'), result.get('valid_to'))
    except Exception as _field_exc:
        result['was_active_as_of_audit_date'] = None
        result.setdefault('_erb_errors', {})['was_active_as_of_audit_date'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_role_assignments_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)
    try:
        result['is_agent_type_change'] = calc_role_assignments_is_agent_type_change(result.get('prior_filler_type'), result.get('filler_type'))
    except Exception as _field_exc:
        result['is_agent_type_change'] = None
        result.setdefault('_erb_errors', {})['is_agent_type_change'] = str(_field_exc)
    try:
        result['requires_compliance_audit'] = calc_role_assignments_requires_compliance_audit(result.get('prior_filler_type'), result.get('filler_type'))
    except Exception as _field_exc:
        result['requires_compliance_audit'] = None
        result.setdefault('_erb_errors', {})['requires_compliance_audit'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name', 'filler_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# DEPARTMENTS
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
    try:
        result['relative_path'] = calc_departments_relative_path(result.get('department_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_departments_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_departments_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# HUMANAGENTS
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
    try:
        result['relative_path'] = calc_human_agents_relative_path(result.get('human_agent_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_human_agents_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AIAGENTS
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
    try:
        result['relative_path'] = calc_ai_agents_relative_path(result.get('ai_agent_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_ai_agents_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AUTOMATEDPIPELINES
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
    try:
        result['relative_path'] = calc_automated_pipelines_relative_path(result.get('automated_pipeline_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_automated_pipelines_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WORKFLOWSTATUSCONCEPTS
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
    try:
        result['relative_path'] = calc_workflow_status_concepts_relative_path(result.get('concept_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_workflow_status_concepts_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AGENTCAPABILITYCONCEPTS
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
    try:
        result['relative_path'] = calc_agent_capability_concepts_relative_path(result.get('concept_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_agent_capability_concepts_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ARTIFACTTYPECONCEPTS
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
    try:
        result['relative_path'] = calc_artifact_type_concepts_relative_path(result.get('concept_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_artifact_type_concepts_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# DATASETS
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
    return _erb.erb_not(_erb.erb_bool3((consumed_by_steps is None or consumed_by_steps == "")))

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
    try:
        result['relative_path'] = calc_datasets_relative_path(result.get('dataset_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['is_consumed'] = calc_datasets_is_consumed(result.get('consumed_by_steps'))
    except Exception as _field_exc:
        result['is_consumed'] = None
        result.setdefault('_erb_errors', {})['is_consumed'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_datasets_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WORKFLOWARTIFACTS
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
    return ('HumanAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((attributed_to_human_agent is None or attributed_to_human_agent == "")))) else ('AIAgent' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((attributed_to_ai_agent is None or attributed_to_ai_agent == "")))) else ('AutomatedPipeline' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3((attributed_to_automated_pipeline is None or attributed_to_automated_pipeline == "")))) else '')))

def calc_workflow_artifacts_has_derivation_parent(derived_from_artifact):
    """
    TRUE iff this artifact was derived from another (prov:wasDerivedFrom is set). Counting these across the chain gives CQ4's '4 derivation links among 5 artifacts' — every artifact except the first has a parent.
    
    Formula: =NOT(ISBLANK({{DerivedFromArtifact}}))
    """
    return _erb.erb_not(_erb.erb_bool3((derived_from_artifact is None or derived_from_artifact == "")))

def calc_workflow_artifacts_has_producing_workflow(produced_by_workflow):
    """
    TRUE iff this artifact resolves to a producing workflow (ProducedByWorkflow is set). Lets the AIAgents blast-radius rollup (CountImpactedWorkflows) count only artifacts that reach a workflow, since COUNTIFS needs a boolean criterion column.
    
    Formula: =NOT(ISBLANK({{ProducedByWorkflow}}))
    """
    return _erb.erb_not(_erb.erb_bool3((produced_by_workflow is None or produced_by_workflow == "")))

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
    try:
        result['relative_path'] = calc_workflow_artifacts_relative_path(result.get('parent_path'), result.get('artifact_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['producing_agent_type'] = calc_workflow_artifacts_producing_agent_type(result.get('attributed_to_human_agent'), result.get('attributed_to_ai_agent'), result.get('attributed_to_automated_pipeline'))
    except Exception as _field_exc:
        result['producing_agent_type'] = None
        result.setdefault('_erb_errors', {})['producing_agent_type'] = str(_field_exc)
    try:
        result['has_derivation_parent'] = calc_workflow_artifacts_has_derivation_parent(result.get('derived_from_artifact'))
    except Exception as _field_exc:
        result['has_derivation_parent'] = None
        result.setdefault('_erb_errors', {})['has_derivation_parent'] = str(_field_exc)
    try:
        result['has_producing_workflow'] = calc_workflow_artifacts_has_producing_workflow(result.get('produced_by_workflow'))
    except Exception as _field_exc:
        result['has_producing_workflow'] = None
        result.setdefault('_erb_errors', {})['has_producing_workflow'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_workflow_artifacts_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'producing_agent_type']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# GOVERNANCEROLES
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
    return _erb.erb_eq(_erb.erb_nullif(kind), 'Authority')

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
    try:
        result['relative_path'] = calc_governance_roles_relative_path(result.get('governance_role_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_governance_roles_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['can_approve_changes'] = calc_governance_roles_can_approve_changes(result.get('kind'))
    except Exception as _field_exc:
        result['can_approve_changes'] = None
        result.setdefault('_erb_errors', {})['can_approve_changes'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_governance_roles_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# CHANGELOG
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
    return _erb.erb_eq(_erb.erb_nullif(change_kind), 'major')

def calc_change_log_is_backward_compatible(change_kind):
    """
    TRUE iff systems on the prior version keep working against this release (ChangeKind is 'patch' or 'minor'). Patch and minor increments preserve backward compatibility; only major breaks it.
    
    Formula: =OR({{ChangeKind}} = "patch", {{ChangeKind}} = "minor")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(change_kind), 'patch')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(change_kind), 'minor')))

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
    try:
        result['relative_path'] = calc_change_log_relative_path(result.get('change_log_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_change_log_name(result.get('version'), result.get('change_date'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_breaking_change'] = calc_change_log_is_breaking_change(result.get('change_kind'))
    except Exception as _field_exc:
        result['is_breaking_change'] = None
        result.setdefault('_erb_errors', {})['is_breaking_change'] = str(_field_exc)
    try:
        result['is_backward_compatible'] = calc_change_log_is_backward_compatible(result.get('change_kind'))
    except Exception as _field_exc:
        result['is_backward_compatible'] = None
        result.setdefault('_erb_errors', {})['is_backward_compatible'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_change_log_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# VOCABULARYRECONCILIATIONS
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
    try:
        result['relative_path'] = calc_vocabulary_reconciliations_relative_path(result.get('reconciliation_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_vocabulary_reconciliations_name(result.get('deprecated_term'), result.get('replacement_term'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_vocabulary_reconciliations_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# SCENARIOS
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
    try:
        result['relative_path'] = calc_scenarios_relative_path(result.get('scenario_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_scenarios_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_scenarios_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# COMPETENCYQUESTIONS
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
    try:
        result['relative_path'] = calc_competency_questions_relative_path(result.get('competency_question_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_competency_questions_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_competency_questions_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# SCENARIOCQEFFECTS
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
    try:
        result['relative_path'] = calc_scenario_cq_effects_relative_path(result.get('scenario_cq_effect_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_scenario_cq_effects_name(result.get('scenario_cq_effect_id'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_scenario_cq_effects_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# CONFORMANCETESTS
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
    try:
        result['relative_path'] = calc_conformance_tests_relative_path(result.get('conformance_test_id'))
    except Exception as _field_exc:
        result['relative_path'] = None
        result.setdefault('_erb_errors', {})['relative_path'] = str(_field_exc)
    try:
        result['name'] = calc_conformance_tests_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['iri'] = calc_conformance_tests_iri(result.get('relative_path'))
    except Exception as _field_exc:
        result['iri'] = None
        result.setdefault('_erb_errors', {})['iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['relative_path', 'iri', 'name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# __META__
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
    try:
        result['name'] = calc___meta___name(result.get('meta_key'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result


# =============================================================================
# DISPATCHER
# =============================================================================

def compute_all_calculated_fields(record: dict, entity_name: str) -> dict:
    """Compute every calculated field of one record of the named entity
    (PascalCase or snake_case). Lookups and aggregations read other rows,
    so they are computed by erb_run over the whole dataset, not here."""
    compute = _ERB_COMPUTE_BY_NAME.get(entity_name)
    if compute is None:
        raise KeyError(
            f"compute_all_calculated_fields called with unknown entity {entity_name!r}. "
            f"Known entities: {sorted(_ERB_COMPUTE_BY_NAME)!r}.")
    return compute(record)


_ERB_COMPUTE_BY_NAME = {
    'Workflows': compute_workflows_fields,
    'workflows': compute_workflows_fields,
    'WorkflowSteps': compute_workflow_steps_fields,
    'workflow_steps': compute_workflow_steps_fields,
    'ApprovalGates': compute_approval_gates_fields,
    'approval_gates': compute_approval_gates_fields,
    'StepPrecedence': compute_step_precedence_fields,
    'step_precedence': compute_step_precedence_fields,
    'Roles': compute_roles_fields,
    'roles': compute_roles_fields,
    'RoleAssignments': compute_role_assignments_fields,
    'role_assignments': compute_role_assignments_fields,
    'Departments': compute_departments_fields,
    'departments': compute_departments_fields,
    'HumanAgents': compute_human_agents_fields,
    'human_agents': compute_human_agents_fields,
    'AIAgents': compute_ai_agents_fields,
    'ai_agents': compute_ai_agents_fields,
    'AutomatedPipelines': compute_automated_pipelines_fields,
    'automated_pipelines': compute_automated_pipelines_fields,
    'WorkflowStatusConcepts': compute_workflow_status_concepts_fields,
    'workflow_status_concepts': compute_workflow_status_concepts_fields,
    'AgentCapabilityConcepts': compute_agent_capability_concepts_fields,
    'agent_capability_concepts': compute_agent_capability_concepts_fields,
    'ArtifactTypeConcepts': compute_artifact_type_concepts_fields,
    'artifact_type_concepts': compute_artifact_type_concepts_fields,
    'Datasets': compute_datasets_fields,
    'datasets': compute_datasets_fields,
    'WorkflowArtifacts': compute_workflow_artifacts_fields,
    'workflow_artifacts': compute_workflow_artifacts_fields,
    'GovernanceRoles': compute_governance_roles_fields,
    'governance_roles': compute_governance_roles_fields,
    'ChangeLog': compute_change_log_fields,
    'change_log': compute_change_log_fields,
    'VocabularyReconciliations': compute_vocabulary_reconciliations_fields,
    'vocabulary_reconciliations': compute_vocabulary_reconciliations_fields,
    'Scenarios': compute_scenarios_fields,
    'scenarios': compute_scenarios_fields,
    'CompetencyQuestions': compute_competency_questions_fields,
    'competency_questions': compute_competency_questions_fields,
    'ScenarioCQEffects': compute_scenario_cq_effects_fields,
    'scenario_cq_effects': compute_scenario_cq_effects_fields,
    'ConformanceTests': compute_conformance_tests_fields,
    'conformance_tests': compute_conformance_tests_fields,
    '__meta__': compute___meta___fields,
}


# =============================================================================
# AGGREGATE SCALARS — formulas wrapped around aggregate calls
# =============================================================================


# calculated_field_count bounds the runner's passes over the dataset.
CALCULATED_FIELD_COUNT = 102

# ERB_TABLES is every table, in rulebook order.
ERB_TABLES = [
    {'name': 'Workflows', 'file': 'workflows', 'rulebook_rows': 1,
     'compute': compute_workflows_fields,
     'fields': ['workflow_id', 'relative_path', 'iri', 'name', 'display_name', 'title', 'description', 'identifier', 'modified', 'created', 'staleness_threshold_months', 'workflow_status', 'workflow_steps', 'count_of_non_proposed_steps', 'has_more_than1_step', 'count_ai_steps', 'count_human_steps', 'count_human_required_steps', 'count_approval_consistency_violations', 'has_consistency_violation', 'has_ai_agent_step', 'months_since_modified', 'is_stale', 'is_stale_and_has_ai_agent', 'count_derivation_links', 'count_legal_owned_steps', 'count_engineering_owned_steps', 'involves_engineering_and_legal', 'count_inferred_precedence_pairs', 'count_asserted_precedence_pairs', 'count_of_precedence_closure_pairs', 'count_roles_with_bad_filler_cardinality', 'count_agent_type_changes', 'count_compliance_audit_changes', 'count_approval_gate_steps', 'count_gates_without_human_approver', 'count_workflow_artifacts', 'count_roles_with_escalation_violation', 'count_unconsumed_datasets', 'cq1_satisfied', 'cq2_satisfied', 'cq3_satisfied', 'cq4_satisfied', 'cq5_satisfied', 'cq6_satisfied', 'cq7_satisfied', 'cq8_satisfied'],
     'calculated': {'iri', 'cq6_satisfied', 'is_stale', 'cq3_satisfied', 'is_stale_and_has_ai_agent', 'cq4_satisfied', 'cq7_satisfied', 'name', 'cq8_satisfied', 'involves_engineering_and_legal', 'has_ai_agent_step', 'months_since_modified', 'cq1_satisfied', 'has_consistency_violation', 'relative_path', 'has_more_than1_step', 'cq5_satisfied', 'cq2_satisfied', 'count_of_precedence_closure_pairs'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_of_non_proposed_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id')]},
        {'field': 'count_ai_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('is_executed_by_ai', 'literal', True)]},
        {'field': 'count_human_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('is_executed_by_human', 'literal', True)]},
        {'field': 'count_human_required_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('requires_human_approval', 'literal', True)]},
        {'field': 'count_approval_consistency_violations', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('approval_consistency_violation', 'literal', True)]},
        {'field': 'count_derivation_links', 'op': 'COUNTIFS', 'table': 'workflow_artifacts', 'criteria': [('produced_by_workflow', 'field', 'workflow_id'), ('has_derivation_parent', 'literal', True)]},
        {'field': 'count_legal_owned_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('is_legal_owned', 'literal', True)]},
        {'field': 'count_engineering_owned_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('is_engineering_owned', 'literal', True)]},
        {'field': 'count_inferred_precedence_pairs', 'op': 'COUNTIFS', 'table': 'vw_step_precedence_closure', 'criteria': [('is_inferred', 'literal', True)]},
        {'field': 'count_asserted_precedence_pairs', 'op': 'COUNTIFS', 'table': 'vw_step_precedence_closure', 'criteria': [('is_inferred', 'literal', False)]},
        {'field': 'count_roles_with_bad_filler_cardinality', 'op': 'COUNTIFS', 'table': 'roles', 'criteria': [('has_exactly_one_filler', 'literal', False)]},
        {'field': 'count_agent_type_changes', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('is_agent_type_change', 'literal', True)]},
        {'field': 'count_compliance_audit_changes', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('requires_compliance_audit', 'literal', True)]},
        {'field': 'count_approval_gate_steps', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('workflow', 'field', 'workflow_id'), ('is_approval_gate', 'literal', True)]},
        {'field': 'count_gates_without_human_approver', 'op': 'COUNTIFS', 'table': 'approval_gates', 'criteria': [('has_human_approver', 'literal', False)]},
        {'field': 'count_workflow_artifacts', 'op': 'COUNTIFS', 'table': 'workflow_artifacts', 'criteria': [('produced_by_workflow', 'field', 'workflow_id')]},
        {'field': 'count_roles_with_escalation_violation', 'op': 'COUNTIFS', 'table': 'roles', 'criteria': [('escalation_violation', 'literal', True)]},
        {'field': 'count_unconsumed_datasets', 'op': 'COUNTIFS', 'table': 'datasets', 'criteria': [('is_consumed', 'literal', False)]},]},
    {'name': 'WorkflowSteps', 'file': 'workflow_steps', 'rulebook_rows': 5,
     'compute': compute_workflow_steps_fields,
     'fields': ['workflow_step_id', 'parent_path', 'relative_path', 'iri', 'name', 'display_name', 'workflow', 'preceding_step_count', 'inferred_sequence_position', 'sequence_position_override', 'sequence_position', 'assigned_role', 'requires_human_approval', 'step_duration_minutes', 'consumes_dataset', 'produces_artifacts', 'requires_artifacts', 'approval_gate', 'precedes', 'preceded_by', 'executing_human_agent', 'executing_ai_agent', 'executing_automated_pipeline', 'executing_agent_type', 'is_executed_by_ai', 'is_executed_by_human', 'is_approval_gate', 'approval_consistency_violation', 'approval_is_human_filled', 'owning_department', 'is_legal_owned', 'is_engineering_owned'],
     'calculated': {'approval_is_human_filled', 'iri', 'is_legal_owned', 'sequence_position', 'inferred_sequence_position', 'name', 'is_engineering_owned', 'approval_consistency_violation', 'is_executed_by_ai', 'relative_path', 'is_executed_by_human', 'is_approval_gate', 'executing_agent_type'},
     'lookups': [
        {'field': 'parent_path', 'target': 'workflows', 'return': 'relative_path', 'key': 'workflow', 'match': 'workflow_id'},
        {'field': 'executing_human_agent', 'target': 'roles', 'return': 'filled_by_human_agent', 'key': 'assigned_role', 'match': 'role_id'},
        {'field': 'executing_ai_agent', 'target': 'roles', 'return': 'filled_by_ai_agent', 'key': 'assigned_role', 'match': 'role_id'},
        {'field': 'executing_automated_pipeline', 'target': 'roles', 'return': 'filled_by_automated_pipeline', 'key': 'assigned_role', 'match': 'role_id'},
        {'field': 'owning_department', 'target': 'roles', 'return': 'owned_by', 'key': 'assigned_role', 'match': 'role_id'},],
     'aggregations': [
        {'field': 'preceding_step_count', 'op': 'COUNTIFS', 'table': 'vw_step_precedence_closure', 'criteria': [('to_id', 'field', 'workflow_step_id')]},]},
    {'name': 'ApprovalGates', 'file': 'approval_gates', 'rulebook_rows': 1,
     'compute': compute_approval_gates_fields,
     'fields': ['approval_gate_id', 'parent_path', 'relative_path', 'iri', 'name', 'display_name', 'workflow_step', 'escalation_threshold_hours', 'gate_role', 'gate_approver_human', 'has_human_approver'],
     'calculated': {'name', 'iri', 'has_human_approver', 'relative_path'},
     'lookups': [
        {'field': 'parent_path', 'target': 'workflow_steps', 'return': 'relative_path', 'key': 'workflow_step', 'match': 'workflow_step_id'},
        {'field': 'gate_role', 'target': 'workflow_steps', 'return': 'assigned_role', 'key': 'workflow_step', 'match': 'workflow_step_id'},
        {'field': 'gate_approver_human', 'target': 'roles', 'return': 'filled_by_human_agent', 'key': 'gate_role', 'match': 'role_id'},],
     'aggregations': []},
    {'name': 'StepPrecedence', 'file': 'step_precedence', 'rulebook_rows': 4,
     'compute': compute_step_precedence_fields,
     'fields': ['step_precedence_id', 'parent_path', 'relative_path', 'iri', 'name', 'from_step', 'to_step', 'precedes_step_closure'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [
        {'field': 'parent_path', 'target': 'workflow_steps', 'return': 'relative_path', 'key': 'from_step', 'match': 'workflow_step_id'},],
     'aggregations': []},
    {'name': 'Roles', 'file': 'roles', 'rulebook_rows': 7,
     'compute': compute_roles_fields,
     'fields': ['role_id', 'relative_path', 'iri', 'name', 'display_name', 'label', 'comment', 'has_capability', 'filled_by_human_agent', 'filled_by_ai_agent', 'filled_by_automated_pipeline', 'owned_by', 'delegates_to', 'workflow_steps', 'from_delegates_to', 'role_assignments', 'delegation_closure', 'filled_by_arm_count', 'has_exactly_one_filler', 'filler_type', 'fills_approval_gate', 'escalation_violation'],
     'calculated': {'has_exactly_one_filler', 'iri', 'name', 'filled_by_arm_count', 'filler_type', 'relative_path', 'escalation_violation'},
     'lookups': [],
     'aggregations': [
        {'field': 'fills_approval_gate', 'op': 'COUNTIFS', 'table': 'workflow_steps', 'criteria': [('assigned_role', 'field', 'role_id'), ('is_approval_gate', 'literal', True)]},]},
    {'name': 'RoleAssignments', 'file': 'role_assignments', 'rulebook_rows': 6,
     'compute': compute_role_assignments_fields,
     'fields': ['role_assignment_id', 'parent_path', 'relative_path', 'iri', 'name', 'role', 'filled_by_human_agent', 'filled_by_ai_agent', 'filled_by_automated_pipeline', 'valid_from', 'valid_to', 'reason', 'prior_filler_type', 'filler_type', 'is_current', 'was_active_as_of_audit_date', 'is_agent_type_change', 'requires_compliance_audit'],
     'calculated': {'iri', 'was_active_as_of_audit_date', 'name', 'requires_compliance_audit', 'is_agent_type_change', 'is_current', 'filler_type', 'relative_path'},
     'lookups': [
        {'field': 'parent_path', 'target': 'roles', 'return': 'relative_path', 'key': 'role', 'match': 'role_id'},],
     'aggregations': []},
    {'name': 'Departments', 'file': 'departments', 'rulebook_rows': 2,
     'compute': compute_departments_fields,
     'fields': ['department_id', 'relative_path', 'iri', 'name', 'title', 'display_name', 'roles'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': 'HumanAgents', 'file': 'human_agents', 'rulebook_rows': 5,
     'compute': compute_human_agents_fields,
     'fields': ['human_agent_id', 'relative_path', 'iri', 'name', 'display_name', 'mbox', 'roles', 'role_assignments'],
     'calculated': {'relative_path', 'iri'},
     'lookups': [],
     'aggregations': []},
    {'name': 'AIAgents', 'file': 'ai_agents', 'rulebook_rows': 2,
     'compute': compute_ai_agents_fields,
     'fields': ['ai_agent_id', 'relative_path', 'iri', 'name', 'title', 'display_name', 'model_version', 'deployed_on', 'roles', 'role_assignments', 'attributed_artifacts', 'count_attributed_artifacts', 'count_impacted_workflows'],
     'calculated': {'relative_path', 'iri'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_attributed_artifacts', 'op': 'COUNTIFS', 'table': 'workflow_artifacts', 'criteria': [('attributed_to_ai_agent', 'field', 'ai_agent_id')]},
        {'field': 'count_impacted_workflows', 'op': 'COUNTIFS', 'table': 'workflow_artifacts', 'criteria': [('attributed_to_ai_agent', 'field', 'ai_agent_id'), ('has_producing_workflow', 'literal', True)]},]},
    {'name': 'AutomatedPipelines', 'file': 'automated_pipelines', 'rulebook_rows': 1,
     'compute': compute_automated_pipelines_fields,
     'fields': ['automated_pipeline_id', 'relative_path', 'iri', 'name', 'description', 'display_name', 'roles', 'role_assignments'],
     'calculated': {'relative_path', 'iri'},
     'lookups': [],
     'aggregations': []},
    {'name': 'WorkflowStatusConcepts', 'file': 'workflow_status_concepts', 'rulebook_rows': 4,
     'compute': compute_workflow_status_concepts_fields,
     'fields': ['concept_id', 'relative_path', 'iri', 'pref_label', 'alt_label', 'definition', 'scope_note', 'workflows'],
     'calculated': {'relative_path', 'iri'},
     'lookups': [],
     'aggregations': []},
    {'name': 'AgentCapabilityConcepts', 'file': 'agent_capability_concepts', 'rulebook_rows': 6,
     'compute': compute_agent_capability_concepts_fields,
     'fields': ['concept_id', 'relative_path', 'iri', 'pref_label', 'alt_label', 'definition', 'scope_note', 'roles'],
     'calculated': {'relative_path', 'iri'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ArtifactTypeConcepts', 'file': 'artifact_type_concepts', 'rulebook_rows': 3,
     'compute': compute_artifact_type_concepts_fields,
     'fields': ['concept_id', 'relative_path', 'iri', 'pref_label', 'alt_label', 'definition', 'scope_note', 'workflow_artifacts'],
     'calculated': {'relative_path', 'iri'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Datasets', 'file': 'datasets', 'rulebook_rows': 1,
     'compute': compute_datasets_fields,
     'fields': ['dataset_id', 'relative_path', 'iri', 'title', 'identifier', 'modified', 'distribution_url', 'consumed_by_steps', 'is_consumed'],
     'calculated': {'relative_path', 'iri', 'is_consumed'},
     'lookups': [],
     'aggregations': []},
    {'name': 'WorkflowArtifacts', 'file': 'workflow_artifacts', 'rulebook_rows': 5,
     'compute': compute_workflow_artifacts_fields,
     'fields': ['artifact_id', 'parent_path', 'relative_path', 'iri', 'title', 'identifier', 'artifact_type', 'created', 'produced_by_step', 'required_by_steps', 'derived_from_artifact', 'attributed_to_human_agent', 'attributed_to_ai_agent', 'attributed_to_automated_pipeline', 'producing_agent_type', 'has_derivation_parent', 'produced_by_workflow', 'has_producing_workflow', 'derivation_closure'],
     'calculated': {'iri', 'has_producing_workflow', 'has_derivation_parent', 'producing_agent_type', 'relative_path'},
     'lookups': [
        {'field': 'parent_path', 'target': 'workflow_steps', 'return': 'relative_path', 'key': 'produced_by_step', 'match': 'workflow_step_id'},
        {'field': 'produced_by_workflow', 'target': 'workflow_steps', 'return': 'workflow', 'key': 'produced_by_step', 'match': 'workflow_step_id'},],
     'aggregations': []},
    {'name': 'GovernanceRoles', 'file': 'governance_roles', 'rulebook_rows': 2,
     'compute': compute_governance_roles_fields,
     'fields': ['governance_role_id', 'relative_path', 'iri', 'name', 'display_name', 'kind', 'responsibilities', 'approval_scope', 'held_by', 'can_approve_changes', 'approved_changes'],
     'calculated': {'name', 'iri', 'relative_path', 'can_approve_changes'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ChangeLog', 'file': 'change_log', 'rulebook_rows': 2,
     'compute': compute_change_log_fields,
     'fields': ['change_log_id', 'relative_path', 'iri', 'name', 'version', 'change_date', 'change_kind', 'motivating_question', 'terms_affected', 'rationale', 'approved_by', 'is_breaking_change', 'is_backward_compatible'],
     'calculated': {'iri', 'is_backward_compatible', 'name', 'is_breaking_change', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': 'VocabularyReconciliations', 'file': 'vocabulary_reconciliations', 'rulebook_rows': 2,
     'compute': compute_vocabulary_reconciliations_fields,
     'fields': ['reconciliation_id', 'relative_path', 'iri', 'name', 'deprecated_term', 'replacement_term', 'reconciliation_relation', 'source_standard', 'introduced_in_version', 'rationale'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Scenarios', 'file': 'scenarios', 'rulebook_rows': 12,
     'compute': compute_scenarios_fields,
     'fields': ['scenario_id', 'relative_path', 'iri', 'name', 'label', 'icon', 'explanation', 'sort_order', 'is_reset', 'edits'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': 'CompetencyQuestions', 'file': 'competency_questions', 'rulebook_rows': 8,
     'compute': compute_competency_questions_fields,
     'fields': ['competency_question_id', 'relative_path', 'iri', 'name', 'number', 'display_name', 'question_text', 'target_table', 'target_field', 'answer_kind', 'expected_answer', 'satisfied_field', 'explanation', 'sort_order', 'is_active', 'simulate_scenario'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ScenarioCQEffects', 'file': 'scenario_cq_effects', 'rulebook_rows': 12,
     'compute': compute_scenario_cq_effects_fields,
     'fields': ['scenario_cq_effect_id', 'relative_path', 'iri', 'name', 'scenario', 'competency_question', 'effect_kind', 'note', 'sort_order'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ConformanceTests', 'file': 'conformance_tests', 'rulebook_rows': 72,
     'compute': compute_conformance_tests_fields,
     'fields': ['conformance_test_id', 'relative_path', 'iri', 'name', 'display_name', 'feature_ref', 'section', 'test_kind', 'target_ref', 'expect', 'explanation', 'sort_order', 'is_enabled'],
     'calculated': {'name', 'iri', 'relative_path'},
     'lookups': [],
     'aggregations': []},
    {'name': '__meta__', 'file': '__meta__', 'rulebook_rows': 12,
     'compute': compute___meta___fields,
     'fields': ['meta_key', 'name', 'value_type', 'string_value', 'json_value'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
]

# ERB_CLOSURES materializes each vw_<entity>_closure view aggregations read.
ERB_CLOSURES = [
    {'view': 'vw_roles_closure', 'source': 'roles', 'from': 'role_id', 'to': 'delegates_to'},
    {'view': 'vw_step_precedence_closure', 'source': 'step_precedence', 'from': 'from_step', 'to': 'to_step'},
    {'view': 'vw_workflow_artifacts_closure', 'source': 'workflow_artifacts', 'from': 'artifact_id', 'to': 'derived_from_artifact'},
]
