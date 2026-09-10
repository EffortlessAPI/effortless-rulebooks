"""
ERB SDK (GENERATED - DO NOT EDIT)
=================================
Generated from: effortless-rulebook/pko-native-procedural-knowledge-rulebook-rulebook.json

A calc_<table>_<field>() function per calculated field, a
compute_<table>_fields(record) per table, and the table registry main.py
runs. Every formula is compiled; lookups and aggregations are compiled to
the specs in ERB_TABLES, which erb_runtime.erb_run computes over the whole
dataset. Nothing here parses a formula or reads the rulebook.
"""

import erb_runtime as _erb


# =============================================================================
# RULEBOOKRELEASES
# Version ledger for the canonical ERB-PKO rulebook itself. This is distinct from PKO Procedure versioning.
# =============================================================================

# Level 1

def calc_rulebook_releases_name(rulebook_version, pko_core_version_iri):
    """
    Human-readable calculated display alias for the RulebookReleases row.
    
    Formula: ={{RulebookVersion}} & " / PKO " & {{PkoCoreVersionIri}}
    """
    return (str(rulebook_version or "") + ' / PKO ' + str(pko_core_version_iri or ""))


def compute_rulebook_releases_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RulebookReleases.
    
    Version ledger for the canonical ERB-PKO rulebook itself. This is distinct from PKO Procedure versioning.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_rulebook_releases_name(result.get('rulebook_version'), result.get('pko_core_version_iri'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ONTOLOGYPROFILES
# Versioned ontology and vocabulary dependencies. PKO mappings always identify the exact profile and version.
# =============================================================================

# Level 1

def calc_ontology_profiles_name(label, version):
    """
    Human-readable calculated display alias for the OntologyProfiles row.
    
    Formula: ={{Label}} & " " & {{Version}}
    """
    return (str(label or "") + ' ' + str(version or ""))


def compute_ontology_profiles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for OntologyProfiles.
    
    Versioned ontology and vocabulary dependencies. PKO mappings always identify the exact profile and version.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_ontology_profiles_name(result.get('label'), result.get('version'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# EVALUATIONCONTEXTS
# The instant this rulebook's time-dependent witnesses are evaluated against. Modeled as data rather than wall-clock so every freshness, overdue, and validity answer is reproducible and auditable: asking the same question tomorrow yields the same answer. Exactly one row carries IsCurrent.
# =============================================================================

# Level 1

def calc_evaluation_contexts_name(label, as_of_instant):
    """
    Human-readable calculated display alias for the EvaluationContexts row.
    
    Formula: ={{Label}} & " @ " & {{AsOfInstant}}
    """
    return (str(label or "") + ' @ ' + _erb.erb_timestamptz_text(as_of_instant))


def compute_evaluation_contexts_fields(record: dict) -> dict:
    """
    Compute all calculated fields for EvaluationContexts.
    
    The instant this rulebook's time-dependent witnesses are evaluated against. Modeled as data rather than wall-clock so every freshness, overdue, and validity answer is reproducible and auditable: asking the same question tomorrow yields the same answer. Exactly one row carries IsCurrent.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_evaluation_contexts_name(result.get('label'), result.get('as_of_instant'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ORGANIZATIONS
# Organizations that own, adopt, govern, or execute procedures. Maps to prov:Organization.
# =============================================================================

# Level 1

def calc_organizations_name(display_name):
    """
    Human-readable calculated display alias for the Organizations row.
    
    Formula: ={{DisplayName}}
    """
    return display_name


def compute_organizations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Organizations.
    
    Organizations that own, adopt, govern, or execute procedures. Maps to prov:Organization.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_organizations_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AGENTS
# Human and software agents that create, modify, approve, or execute procedural knowledge. Maps to prov:Agent.
# =============================================================================

# Level 1

def calc_agents_name(display_name):
    """
    Human-readable calculated display alias for the Agents row.
    
    Formula: ={{DisplayName}}
    """
    return display_name

def calc_agents_is_still_engaged(count_of_current_role_assignments):
    """
    TRUE when this agent currently holds at least one role in the organization.
    
    Formula: ={{CountOfCurrentRoleAssignments}} > 0
    """
    return _erb.erb_cmp(count_of_current_role_assignments, '>', 0)

def calc_agents_override_rate_percent(decision_count, overridden_decision_count):
    """
    Percentage of this agent's decisions that were overridden by a human.
    
    Formula: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}})
    """
    return (0 if _erb.erb_bool3(_erb.erb_eq(decision_count, 0)) else _erb.erb_div(_erb.erb_mul(overridden_decision_count, 100), decision_count))

def calc_agents_is_non_human(agent_kind):
    """
    TRUE when this agent is an AI agent or an automated pipeline.
    
    Formula: =NOT({{AgentKind}} = "Human")
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(agent_kind), 'Human')))

def calc_agents_is_operating_outside_boundary(boundary_violation_count):
    """
    TRUE when this agent has made at least one decision an authority boundary forbids.
    
    Formula: ={{BoundaryViolationCount}} > 0
    """
    return _erb.erb_cmp(boundary_violation_count, '>', 0)

def calc_agents_draft_rewrite_rate_percent(draft_decision_count, overridden_draft_count):
    """
    Percentage of this agent's drafting output that a human rewrote.
    
    Formula: =IF({{DraftDecisionCount}} = 0, 0, ({{OverriddenDraftCount}} * 100) / {{DraftDecisionCount}})
    """
    return (0 if _erb.erb_bool3(_erb.erb_eq(draft_decision_count, 0)) else _erb.erb_div(_erb.erb_mul(overridden_draft_count, 100), draft_decision_count))

def calc_agents_is_recognized_broker(times_named_as_broker):
    """
    TRUE when at least three people actively rely on this agent as an informal knowledge broker.
    
    Formula: ={{TimesNamedAsBroker}} >= 3
    """
    return _erb.erb_cmp(times_named_as_broker, '>=', 3)

def calc_agents_has_at_risk_knowledge_reliance(at_risk_reliance_count):
    """
    TRUE when people are actively relying on this agent even though they have already left every role.
    
    Formula: ={{AtRiskRelianceCount}} > 0
    """
    return _erb.erb_cmp(at_risk_reliance_count, '>', 0)


def compute_agents_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Agents.
    
    Human and software agents that create, modify, approve, or execute procedural knowledge. Maps to prov:Agent.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_agents_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_still_engaged'] = calc_agents_is_still_engaged(result.get('count_of_current_role_assignments'))
    except Exception as _field_exc:
        result['is_still_engaged'] = None
        result.setdefault('_erb_errors', {})['is_still_engaged'] = str(_field_exc)
    try:
        result['override_rate_percent'] = calc_agents_override_rate_percent(result.get('decision_count'), result.get('overridden_decision_count'))
    except Exception as _field_exc:
        result['override_rate_percent'] = None
        result.setdefault('_erb_errors', {})['override_rate_percent'] = str(_field_exc)
    try:
        result['is_non_human'] = calc_agents_is_non_human(result.get('agent_kind'))
    except Exception as _field_exc:
        result['is_non_human'] = None
        result.setdefault('_erb_errors', {})['is_non_human'] = str(_field_exc)
    try:
        result['is_operating_outside_boundary'] = calc_agents_is_operating_outside_boundary(result.get('boundary_violation_count'))
    except Exception as _field_exc:
        result['is_operating_outside_boundary'] = None
        result.setdefault('_erb_errors', {})['is_operating_outside_boundary'] = str(_field_exc)
    try:
        result['draft_rewrite_rate_percent'] = calc_agents_draft_rewrite_rate_percent(result.get('draft_decision_count'), result.get('overridden_draft_count'))
    except Exception as _field_exc:
        result['draft_rewrite_rate_percent'] = None
        result.setdefault('_erb_errors', {})['draft_rewrite_rate_percent'] = str(_field_exc)
    try:
        result['is_recognized_broker'] = calc_agents_is_recognized_broker(result.get('times_named_as_broker'))
    except Exception as _field_exc:
        result['is_recognized_broker'] = None
        result.setdefault('_erb_errors', {})['is_recognized_broker'] = str(_field_exc)
    try:
        result['has_at_risk_knowledge_reliance'] = calc_agents_has_at_risk_knowledge_reliance(result.get('at_risk_reliance_count'))
    except Exception as _field_exc:
        result['has_at_risk_knowledge_reliance'] = None
        result.setdefault('_erb_errors', {})['has_at_risk_knowledge_reliance'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLES
# Stable organizational functions separated from the agents that currently fill them. Maps to pro:Role.
# =============================================================================

# Level 1

def calc_roles_name(label):
    """
    Human-readable calculated display alias for the Roles row.
    
    Formula: ={{Label}}
    """
    return label

def calc_roles_has_no_current_holder(currently_covered_assignment_count):
    """
    TRUE when no assignment currently covers this role — the role is uncovered.
    
    Formula: ={{CurrentlyCoveredAssignmentCount}} = 0
    """
    return _erb.erb_eq(currently_covered_assignment_count, 0)

def calc_roles_is_non_human_held(current_agent_kind):
    """
    TRUE when the role's current agent is an AI agent or automated pipeline.
    
    Formula: =NOT({{CurrentAgentKind}} = "Human")
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(current_agent_kind, 'Human')))

def calc_roles_has_lost_a_holder(departed_assignment_count):
    """
    Whether anyone has ever departed this role.
    
    Formula: ={{DepartedAssignmentCount}} > 0
    """
    return _erb.erb_cmp(departed_assignment_count, '>', 0)

def calc_roles_is_governed_by_lapsed_authority(ungrounded_boundary_count):
    """
    TRUE when at least one constraint on this role rests on knowledge that is no longer valid.
    
    Formula: =({{UngroundedBoundaryCount}} > 0)
    """
    return _erb.erb_cmp(ungrounded_boundary_count, '>', 0)

def calc_roles_is_ungoverned_enforcement_role(unauthorized_enforcement_assignment_count):
    """
    TRUE when a role that enforces controls on others is held with no recorded authorization.
    
    Formula: =({{UnauthorizedEnforcementAssignmentCount}} > 0)
    """
    return _erb.erb_cmp(unauthorized_enforcement_assignment_count, '>', 0)

# Level 2

def calc_roles_is_ungoverned_non_human_role(is_non_human_held, has_no_current_holder):
    """
    TRUE when a role is pointed at a non-human agent but has no assignment row granting it.
    
    Formula: =AND({{IsNonHumanHeld}}, {{HasNoCurrentHolder}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_non_human_held), _erb.erb_bool3(has_no_current_holder))

def calc_roles_is_vacated_role(has_lost_a_holder, has_no_current_holder):
    """
    A role somebody departed and that nobody currently covers.
    
    Formula: =AND({{HasLostAHolder}}, {{HasNoCurrentHolder}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_lost_a_holder), _erb.erb_bool3(has_no_current_holder))


def compute_roles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Roles.
    
    Stable organizational functions separated from the agents that currently fill them. Maps to pro:Role.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_roles_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_no_current_holder'] = calc_roles_has_no_current_holder(result.get('currently_covered_assignment_count'))
    except Exception as _field_exc:
        result['has_no_current_holder'] = None
        result.setdefault('_erb_errors', {})['has_no_current_holder'] = str(_field_exc)
    try:
        result['is_non_human_held'] = calc_roles_is_non_human_held(result.get('current_agent_kind'))
    except Exception as _field_exc:
        result['is_non_human_held'] = None
        result.setdefault('_erb_errors', {})['is_non_human_held'] = str(_field_exc)
    try:
        result['has_lost_a_holder'] = calc_roles_has_lost_a_holder(result.get('departed_assignment_count'))
    except Exception as _field_exc:
        result['has_lost_a_holder'] = None
        result.setdefault('_erb_errors', {})['has_lost_a_holder'] = str(_field_exc)
    try:
        result['is_governed_by_lapsed_authority'] = calc_roles_is_governed_by_lapsed_authority(result.get('ungrounded_boundary_count'))
    except Exception as _field_exc:
        result['is_governed_by_lapsed_authority'] = None
        result.setdefault('_erb_errors', {})['is_governed_by_lapsed_authority'] = str(_field_exc)
    try:
        result['is_ungoverned_enforcement_role'] = calc_roles_is_ungoverned_enforcement_role(result.get('unauthorized_enforcement_assignment_count'))
    except Exception as _field_exc:
        result['is_ungoverned_enforcement_role'] = None
        result.setdefault('_erb_errors', {})['is_ungoverned_enforcement_role'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_ungoverned_non_human_role'] = calc_roles_is_ungoverned_non_human_role(result.get('is_non_human_held'), result.get('has_no_current_holder'))
    except Exception as _field_exc:
        result['is_ungoverned_non_human_role'] = None
        result.setdefault('_erb_errors', {})['is_ungoverned_non_human_role'] = str(_field_exc)
    try:
        result['is_vacated_role'] = calc_roles_is_vacated_role(result.get('has_lost_a_holder'), result.get('has_no_current_holder'))
    except Exception as _field_exc:
        result['is_vacated_role'] = None
        result.setdefault('_erb_errors', {})['is_vacated_role'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLEASSIGNMENTS
# Time-bounded records of agents holding roles. Maps to pro:RoleInTime and preserves assignment history instead of overwriting it.
# =============================================================================

# Level 1

def calc_role_assignments_name(role, valid_from):
    """
    Human-readable calculated display alias for the RoleAssignments row.
    
    Formula: ={{Role}} & " @ " & {{ValidFrom}}
    """
    return (str(role or "") + ' @ ' + _erb.erb_timestamptz_text(valid_from))

def calc_role_assignments_is_current(valid_from, as_of_instant, valid_to):
    """
    TRUE when the assignment is valid now.
    
    Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', as_of_instant)), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))))

def calc_role_assignments_is_currently_valid(status, valid_to, as_of_instant):
    """
    TRUE when this role assignment is active and has not lapsed.
    
    Formula: =AND({{Status}} = "Active", OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Active')), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))))

def calc_role_assignments_has_departed(valid_to, as_of_instant):
    """
    TRUE when this role assignment has ended — the agent no longer holds the role.
    
    Formula: =AND({{ValidTo}} <> "", {{ValidTo}} <= {{AsOfInstant}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (valid_to is None or valid_to == ""))), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '<=', as_of_instant)))

def calc_role_assignments_covers_now(status, valid_from, as_of_instant, valid_to):
    """
    TRUE when this assignment is both status-Active and inside its valid-time window right now.
    
    Formula: =AND({{Status}} = "Active", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Active')), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', as_of_instant)), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))))

def calc_role_assignments_is_non_human_assignment(agent_kind):
    """
    TRUE when this assignment places a non-human agent into the role.
    
    Formula: =NOT({{AgentKind}} = "Human")
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(agent_kind, 'Human')))

def calc_role_assignments_override_rate_percent(decision_count, overridden_decision_count):
    """
    Percentage of decisions under this assignment that a human overrode.
    
    Formula: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}})
    """
    return (0 if _erb.erb_bool3(_erb.erb_eq(decision_count, 0)) else _erb.erb_div(_erb.erb_mul(overridden_decision_count, 100), decision_count))

def calc_role_assignments_has_sufficient_sample(decision_count, minimum_decisions_for_comparison):
    """
    TRUE when this assignment has produced enough decisions for its override rate to mean anything.
    
    Formula: ={{DecisionCount}} >= {{MinimumDecisionsForComparison}}
    """
    return _erb.erb_cmp(decision_count, '>=', _erb.erb_nullif(minimum_decisions_for_comparison))

def calc_role_assignments_predecessor_has_sufficient_sample(predecessor_decision_count, minimum_decisions_for_comparison):
    """
    TRUE when the predecessor assignment produced enough decisions to compare against.
    
    Formula: ={{PredecessorDecisionCount}} >= {{MinimumDecisionsForComparison}}
    """
    return _erb.erb_cmp(predecessor_decision_count, '>=', _erb.erb_nullif(minimum_decisions_for_comparison))

def calc_role_assignments_single_override_swing_percent(decision_count):
    """
    How many percentage points one additional override would move this assignment's rate. The fragility of the number.
    
    Formula: =IF({{DecisionCount}} > 0, 100 / {{DecisionCount}}, 0)
    """
    return (_erb.erb_div(100, decision_count) if _erb.erb_bool3(_erb.erb_cmp(decision_count, '>', 0)) else 0)

def calc_role_assignments_error_rate_percent(decision_count, error_correction_count):
    """
    Percentage of this assignment's decisions overridden as errors -- the override rate with reserved-judgment overrides removed.
    
    Formula: =IF({{DecisionCount}} > 0, {{ErrorCorrectionCount}} * 100 / {{DecisionCount}}, 0)
    """
    return (_erb.erb_div(_erb.erb_mul(error_correction_count, 100), decision_count) if _erb.erb_bool3(_erb.erb_cmp(decision_count, '>', 0)) else 0)

def calc_role_assignments_has_dated_authorization(approving_authority_role, authorization_decided_at):
    """
    TRUE when this assignment carries both a named approving authority and the date they granted it.
    
    Formula: =AND({{ApprovingAuthorityRole}} <> "", {{AuthorizationDecidedAt}} <> "")
    """
    return _erb.erb_and(_erb.erb_bool3((not (approving_authority_role is None or approving_authority_role == ""))), _erb.erb_bool3((not (authorization_decided_at is None or authorization_decided_at == ""))))

def calc_role_assignments_days_since_authorization_review(authorization_reviewed_at, as_of_instant, valid_from):
    """
    How long since this assignment's authorization was last re-examined.
    
    Formula: =IF({{AuthorizationReviewedAt}} <> "", DATETIME_DIFF({{AsOfInstant}}, {{AuthorizationReviewedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"))
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(as_of_instant, authorization_reviewed_at, 'days') if _erb.erb_bool3((not (authorization_reviewed_at is None or authorization_reviewed_at == ""))) else _erb.erb_datetime_diff(as_of_instant, valid_from, 'days')))

def calc_role_assignments_has_any_boundary_violation(boundary_violation_count_for_assignment):
    """
    TRUE when any decision under this assignment crossed a boundary it was forbidden to cross.
    
    Formula: =({{BoundaryViolationCountForAssignment}} > 0)
    """
    return _erb.erb_cmp(boundary_violation_count_for_assignment, '>', 0)

def calc_role_assignments_has_declared_suspension_condition(max_tolerable_error_rate_percent):
    """
    TRUE when this assignment has any pre-declared condition under which it must stop at all.
    
    Formula: ={{MaxTolerableErrorRatePercent}} > 0
    """
    return _erb.erb_cmp(_erb.erb_nullif(max_tolerable_error_rate_percent), '>', 0)

def calc_role_assignments_has_approving_authority(approving_authority_role):
    """
    TRUE when a role is recorded as having approved this assignment.
    
    Formula: ={{ApprovingAuthorityRole}} <> "" 
    """
    return (not (approving_authority_role is None or approving_authority_role == ""))

def calc_role_assignments_has_authorizing_change_request(authorizing_change_request):
    """
    TRUE when a change request is recorded as the governance vehicle for this assignment.
    
    Formula: ={{AuthorizingChangeRequest}} <> "" 
    """
    return (not (authorizing_change_request is None or authorizing_change_request == ""))

# Level 2

def calc_role_assignments_current_agent_key(is_current, agent):
    """
    Echoes the Agent id only while this assignment is current; empty otherwise. Lets a parent count CURRENT assignments with a single-criterion COUNTIFS, which is the only shape this transpiler translates correctly.
    
    Formula: =IF({{IsCurrent}}, {{Agent}}, "")
    """
    return (agent if _erb.erb_bool3(is_current) else '')

def calc_role_assignments_agent_role_key(is_currently_valid, agent, role):
    """
    Composite agent+role key, emitted only for currently-valid assignments.
    
    Formula: =IF({{IsCurrentlyValid}}, {{Agent}} & "|" & {{Role}}, "")
    """
    return ((str(agent or "") + '|' + str(role or "")) if _erb.erb_bool3(is_currently_valid) else '')

def calc_role_assignments_role_when_covering(covers_now, role):
    """
    Echoes the role id when this assignment is currently in force, blank otherwise.
    
    Formula: =IF({{CoversNow}}, {{Role}}, "")
    """
    return (role if _erb.erb_bool3(covers_now) else '')

def calc_role_assignments_is_human_to_non_human_handover(predecessor_agent_kind, is_non_human_assignment):
    """
    TRUE when this assignment handed a role from a human to a non-human agent.
    
    Formula: =AND({{PredecessorAgentKind}} = "Human", {{IsNonHumanAssignment}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(predecessor_agent_kind, 'Human')), _erb.erb_bool3(is_non_human_assignment))

def calc_role_assignments_is_unauthorized_non_human_assignment(is_non_human_assignment, has_approving_authority):
    """
    TRUE when a non-human agent holds this role with no approving authority recorded at all. An authority named without a change request is still an authority; the separate WasAuthorizedByChangeRequest column carries that weaker distinction.
    
    Formula: =AND({{IsNonHumanAssignment}}, NOT({{HasApprovingAuthority}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_non_human_assignment), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_approving_authority))))

def calc_role_assignments_was_authorized_by_change_request(has_approving_authority, authorizing_change_request):
    """
    TRUE when this assignment's authorization is traceable to a change request, not merely to a named role. The stronger form of authorization, kept separate so the weaker one is not silently reported as unauthorized.
    
    Formula: =AND({{HasApprovingAuthority}}, {{AuthorizingChangeRequest}} <> "")
    """
    return _erb.erb_and(_erb.erb_bool3(has_approving_authority), _erb.erb_bool3((not (authorizing_change_request is None or authorizing_change_request == ""))))

def calc_role_assignments_quality_regressed_vs_predecessor(supersedes_assignment, override_rate_percent, predecessor_override_rate_percent):
    """
    TRUE when this assignment is overridden by humans more often than the assignment it replaced.
    
    Formula: =AND({{SupersedesAssignment}} <> "", {{OverrideRatePercent}} > {{PredecessorOverrideRatePercent}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (supersedes_assignment is None or supersedes_assignment == ""))), _erb.erb_bool3(_erb.erb_cmp(override_rate_percent, '>', predecessor_override_rate_percent)))

def calc_role_assignments_departed_role_key(has_departed, role):
    """
    Composite-key echo: the role this assignment covered when the assignment has ended, blank otherwise.
    
    Formula: =IF({{HasDeparted}}, {{Role}}, "")
    """
    return (role if _erb.erb_bool3(has_departed) else '')

def calc_role_assignments_comparison_is_evidentially_sound(has_sufficient_sample, predecessor_has_sufficient_sample):
    """
    TRUE when both sides of the override-rate comparison rest on adequate samples.
    
    Formula: =AND({{HasSufficientSample}}, {{PredecessorHasSufficientSample}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_sufficient_sample), _erb.erb_bool3(predecessor_has_sufficient_sample))

def calc_role_assignments_authorization_is_overdue_for_review(authorization_review_cadence_days, days_since_authorization_review):
    """
    TRUE when the promised re-examination interval has elapsed without a review.
    
    Formula: =AND({{AuthorizationReviewCadenceDays}} > 0, {{DaysSinceAuthorizationReview}} > {{AuthorizationReviewCadenceDays}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(authorization_review_cadence_days), '>', 0)), _erb.erb_bool3(_erb.erb_cmp(days_since_authorization_review, '>', _erb.erb_nullif(authorization_review_cadence_days))))

def calc_role_assignments_exceeds_tolerable_error_rate(max_tolerable_error_rate_percent, error_rate_percent):
    """
    TRUE when this assignment's error rate has reached the threshold set when it was authorized.
    
    Formula: =AND({{MaxTolerableErrorRatePercent}} > 0, {{ErrorRatePercent}} >= {{MaxTolerableErrorRatePercent}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(max_tolerable_error_rate_percent), '>', 0)), _erb.erb_bool3(_erb.erb_cmp(error_rate_percent, '>=', _erb.erb_nullif(max_tolerable_error_rate_percent))))

def calc_role_assignments_governance_evidence_count(has_approving_authority, has_authorizing_change_request):
    """
    How many independent governance artifacts back this assignment: an approving role, an authorizing change request.
    
    Formula: =IF({{HasApprovingAuthority}}, 1, 0) + IF({{HasAuthorizingChangeRequest}}, 1, 0)
    """
    return _erb.erb_integer(_erb.erb_add((1 if _erb.erb_bool3(has_approving_authority) else 0), (1 if _erb.erb_bool3(has_authorizing_change_request) else 0)))

# Level 3

def calc_role_assignments_quality_verdict_is_unsupported(comparison_is_evidentially_sound, quality_regressed_vs_predecessor):
    """
    TRUE when a quality verdict is being reported for this assignment on a sample too small to support it.
    
    Formula: =AND(NOT({{ComparisonIsEvidentiallySound}}), NOT({{QualityRegressedVsPredecessor}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(comparison_is_evidentially_sound))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(quality_regressed_vs_predecessor))))

def calc_role_assignments_is_unmeasured_automation_handover(is_human_to_non_human_handover, comparison_is_evidentially_sound):
    """
    TRUE when a human-to-machine handover is operating without a statistically meaningful quality comparison behind it.
    
    Formula: =AND({{IsHumanToNonHumanHandover}}, NOT({{ComparisonIsEvidentiallySound}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_human_to_non_human_handover), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(comparison_is_evidentially_sound))))

def calc_role_assignments_is_standing_unreviewed_automation(covers_now, is_non_human_assignment, authorization_is_overdue_for_review):
    """
    TRUE when a currently-active non-human assignment has been running past its authorization review date.
    
    Formula: =AND({{CoversNow}}, AND({{IsNonHumanAssignment}}, {{AuthorizationIsOverdueForReview}}))
    """
    return _erb.erb_and(_erb.erb_bool3(covers_now), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_non_human_assignment), _erb.erb_bool3(authorization_is_overdue_for_review))))

def calc_role_assignments_is_unconditioned_automation_handover(is_human_to_non_human_handover, authorization_review_cadence_days):
    """
    TRUE when a human-to-machine handover was authorized with no promised review cadence at all -- granted once, permanently.
    
    Formula: =AND({{IsHumanToNonHumanHandover}}, {{AuthorizationReviewCadenceDays}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_human_to_non_human_handover), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(authorization_review_cadence_days), 0)))

def calc_role_assignments_suspension_condition_met(exceeds_tolerable_error_rate, has_any_boundary_violation, has_ungrounded_governing_boundary):
    """
    TRUE when any pre-declared condition requiring this assignment to stop deciding unaided has been met.
    
    Formula: =OR({{ExceedsTolerableErrorRate}}, OR({{HasAnyBoundaryViolation}}, {{HasUngroundedGoverningBoundary}}))
    """
    return _erb.erb_or(_erb.erb_bool3(exceeds_tolerable_error_rate), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(has_any_boundary_violation), _erb.erb_bool3(has_ungrounded_governing_boundary))))

def calc_role_assignments_is_unauthorized_enforcement_agent(is_enforcement_role, is_unauthorized_non_human_assignment):
    """
    TRUE when a non-human agent enforces controls on others while holding no recorded authorization of its own.
    
    Formula: =AND({{IsEnforcementRole}}, {{IsUnauthorizedNonHumanAssignment}})
    """
    return _erb.erb_and((is_enforcement_role is True), _erb.erb_bool3(is_unauthorized_non_human_assignment))

def calc_role_assignments_unauthorized_enforcement_role_key(is_unauthorized_non_human_assignment, role):
    """
    Echoes the role only for non-human assignments nobody authorized; empty otherwise.
    
    Formula: =IF({{IsUnauthorizedNonHumanAssignment}}, {{Role}}, "")
    """
    return (role if _erb.erb_bool3(is_unauthorized_non_human_assignment) else '')

# Level 4

def calc_role_assignments_is_operating_under_met_suspension_condition(suspension_condition_met, covers_now, is_non_human_assignment):
    """
    TRUE when a suspension condition has been met and the assignment is nonetheless still active and still deciding.
    
    Formula: =AND({{SuspensionConditionMet}}, AND({{CoversNow}}, {{IsNonHumanAssignment}}))
    """
    return _erb.erb_and(_erb.erb_bool3(suspension_condition_met), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(covers_now), _erb.erb_bool3(is_non_human_assignment))))


def compute_role_assignments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleAssignments.
    
    Time-bounded records of agents holding roles. Maps to pro:RoleInTime and preserves assignment history instead of overwriting it.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_role_assignments_name(result.get('role'), result.get('valid_from'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_current'] = calc_role_assignments_is_current(result.get('valid_from'), result.get('as_of_instant'), result.get('valid_to'))
    except Exception as _field_exc:
        result['is_current'] = None
        result.setdefault('_erb_errors', {})['is_current'] = str(_field_exc)
    try:
        result['is_currently_valid'] = calc_role_assignments_is_currently_valid(result.get('status'), result.get('valid_to'), result.get('as_of_instant'))
    except Exception as _field_exc:
        result['is_currently_valid'] = None
        result.setdefault('_erb_errors', {})['is_currently_valid'] = str(_field_exc)
    try:
        result['has_departed'] = calc_role_assignments_has_departed(result.get('valid_to'), result.get('as_of_instant'))
    except Exception as _field_exc:
        result['has_departed'] = None
        result.setdefault('_erb_errors', {})['has_departed'] = str(_field_exc)
    try:
        result['covers_now'] = calc_role_assignments_covers_now(result.get('status'), result.get('valid_from'), result.get('as_of_instant'), result.get('valid_to'))
    except Exception as _field_exc:
        result['covers_now'] = None
        result.setdefault('_erb_errors', {})['covers_now'] = str(_field_exc)
    try:
        result['is_non_human_assignment'] = calc_role_assignments_is_non_human_assignment(result.get('agent_kind'))
    except Exception as _field_exc:
        result['is_non_human_assignment'] = None
        result.setdefault('_erb_errors', {})['is_non_human_assignment'] = str(_field_exc)
    try:
        result['override_rate_percent'] = calc_role_assignments_override_rate_percent(result.get('decision_count'), result.get('overridden_decision_count'))
    except Exception as _field_exc:
        result['override_rate_percent'] = None
        result.setdefault('_erb_errors', {})['override_rate_percent'] = str(_field_exc)
    try:
        result['has_sufficient_sample'] = calc_role_assignments_has_sufficient_sample(result.get('decision_count'), result.get('minimum_decisions_for_comparison'))
    except Exception as _field_exc:
        result['has_sufficient_sample'] = None
        result.setdefault('_erb_errors', {})['has_sufficient_sample'] = str(_field_exc)
    try:
        result['predecessor_has_sufficient_sample'] = calc_role_assignments_predecessor_has_sufficient_sample(result.get('predecessor_decision_count'), result.get('minimum_decisions_for_comparison'))
    except Exception as _field_exc:
        result['predecessor_has_sufficient_sample'] = None
        result.setdefault('_erb_errors', {})['predecessor_has_sufficient_sample'] = str(_field_exc)
    try:
        result['single_override_swing_percent'] = calc_role_assignments_single_override_swing_percent(result.get('decision_count'))
    except Exception as _field_exc:
        result['single_override_swing_percent'] = None
        result.setdefault('_erb_errors', {})['single_override_swing_percent'] = str(_field_exc)
    try:
        result['error_rate_percent'] = calc_role_assignments_error_rate_percent(result.get('decision_count'), result.get('error_correction_count'))
    except Exception as _field_exc:
        result['error_rate_percent'] = None
        result.setdefault('_erb_errors', {})['error_rate_percent'] = str(_field_exc)
    try:
        result['has_dated_authorization'] = calc_role_assignments_has_dated_authorization(result.get('approving_authority_role'), result.get('authorization_decided_at'))
    except Exception as _field_exc:
        result['has_dated_authorization'] = None
        result.setdefault('_erb_errors', {})['has_dated_authorization'] = str(_field_exc)
    try:
        result['days_since_authorization_review'] = calc_role_assignments_days_since_authorization_review(result.get('authorization_reviewed_at'), result.get('as_of_instant'), result.get('valid_from'))
    except Exception as _field_exc:
        result['days_since_authorization_review'] = None
        result.setdefault('_erb_errors', {})['days_since_authorization_review'] = str(_field_exc)
    try:
        result['has_any_boundary_violation'] = calc_role_assignments_has_any_boundary_violation(result.get('boundary_violation_count_for_assignment'))
    except Exception as _field_exc:
        result['has_any_boundary_violation'] = None
        result.setdefault('_erb_errors', {})['has_any_boundary_violation'] = str(_field_exc)
    try:
        result['has_declared_suspension_condition'] = calc_role_assignments_has_declared_suspension_condition(result.get('max_tolerable_error_rate_percent'))
    except Exception as _field_exc:
        result['has_declared_suspension_condition'] = None
        result.setdefault('_erb_errors', {})['has_declared_suspension_condition'] = str(_field_exc)
    try:
        result['has_approving_authority'] = calc_role_assignments_has_approving_authority(result.get('approving_authority_role'))
    except Exception as _field_exc:
        result['has_approving_authority'] = None
        result.setdefault('_erb_errors', {})['has_approving_authority'] = str(_field_exc)
    try:
        result['has_authorizing_change_request'] = calc_role_assignments_has_authorizing_change_request(result.get('authorizing_change_request'))
    except Exception as _field_exc:
        result['has_authorizing_change_request'] = None
        result.setdefault('_erb_errors', {})['has_authorizing_change_request'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['current_agent_key'] = calc_role_assignments_current_agent_key(result.get('is_current'), result.get('agent'))
    except Exception as _field_exc:
        result['current_agent_key'] = None
        result.setdefault('_erb_errors', {})['current_agent_key'] = str(_field_exc)
    try:
        result['agent_role_key'] = calc_role_assignments_agent_role_key(result.get('is_currently_valid'), result.get('agent'), result.get('role'))
    except Exception as _field_exc:
        result['agent_role_key'] = None
        result.setdefault('_erb_errors', {})['agent_role_key'] = str(_field_exc)
    try:
        result['role_when_covering'] = calc_role_assignments_role_when_covering(result.get('covers_now'), result.get('role'))
    except Exception as _field_exc:
        result['role_when_covering'] = None
        result.setdefault('_erb_errors', {})['role_when_covering'] = str(_field_exc)
    try:
        result['is_human_to_non_human_handover'] = calc_role_assignments_is_human_to_non_human_handover(result.get('predecessor_agent_kind'), result.get('is_non_human_assignment'))
    except Exception as _field_exc:
        result['is_human_to_non_human_handover'] = None
        result.setdefault('_erb_errors', {})['is_human_to_non_human_handover'] = str(_field_exc)
    try:
        result['is_unauthorized_non_human_assignment'] = calc_role_assignments_is_unauthorized_non_human_assignment(result.get('is_non_human_assignment'), result.get('has_approving_authority'))
    except Exception as _field_exc:
        result['is_unauthorized_non_human_assignment'] = None
        result.setdefault('_erb_errors', {})['is_unauthorized_non_human_assignment'] = str(_field_exc)
    try:
        result['was_authorized_by_change_request'] = calc_role_assignments_was_authorized_by_change_request(result.get('has_approving_authority'), result.get('authorizing_change_request'))
    except Exception as _field_exc:
        result['was_authorized_by_change_request'] = None
        result.setdefault('_erb_errors', {})['was_authorized_by_change_request'] = str(_field_exc)
    try:
        result['quality_regressed_vs_predecessor'] = calc_role_assignments_quality_regressed_vs_predecessor(result.get('supersedes_assignment'), result.get('override_rate_percent'), result.get('predecessor_override_rate_percent'))
    except Exception as _field_exc:
        result['quality_regressed_vs_predecessor'] = None
        result.setdefault('_erb_errors', {})['quality_regressed_vs_predecessor'] = str(_field_exc)
    try:
        result['departed_role_key'] = calc_role_assignments_departed_role_key(result.get('has_departed'), result.get('role'))
    except Exception as _field_exc:
        result['departed_role_key'] = None
        result.setdefault('_erb_errors', {})['departed_role_key'] = str(_field_exc)
    try:
        result['comparison_is_evidentially_sound'] = calc_role_assignments_comparison_is_evidentially_sound(result.get('has_sufficient_sample'), result.get('predecessor_has_sufficient_sample'))
    except Exception as _field_exc:
        result['comparison_is_evidentially_sound'] = None
        result.setdefault('_erb_errors', {})['comparison_is_evidentially_sound'] = str(_field_exc)
    try:
        result['authorization_is_overdue_for_review'] = calc_role_assignments_authorization_is_overdue_for_review(result.get('authorization_review_cadence_days'), result.get('days_since_authorization_review'))
    except Exception as _field_exc:
        result['authorization_is_overdue_for_review'] = None
        result.setdefault('_erb_errors', {})['authorization_is_overdue_for_review'] = str(_field_exc)
    try:
        result['exceeds_tolerable_error_rate'] = calc_role_assignments_exceeds_tolerable_error_rate(result.get('max_tolerable_error_rate_percent'), result.get('error_rate_percent'))
    except Exception as _field_exc:
        result['exceeds_tolerable_error_rate'] = None
        result.setdefault('_erb_errors', {})['exceeds_tolerable_error_rate'] = str(_field_exc)
    try:
        result['governance_evidence_count'] = calc_role_assignments_governance_evidence_count(result.get('has_approving_authority'), result.get('has_authorizing_change_request'))
    except Exception as _field_exc:
        result['governance_evidence_count'] = None
        result.setdefault('_erb_errors', {})['governance_evidence_count'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['quality_verdict_is_unsupported'] = calc_role_assignments_quality_verdict_is_unsupported(result.get('comparison_is_evidentially_sound'), result.get('quality_regressed_vs_predecessor'))
    except Exception as _field_exc:
        result['quality_verdict_is_unsupported'] = None
        result.setdefault('_erb_errors', {})['quality_verdict_is_unsupported'] = str(_field_exc)
    try:
        result['is_unmeasured_automation_handover'] = calc_role_assignments_is_unmeasured_automation_handover(result.get('is_human_to_non_human_handover'), result.get('comparison_is_evidentially_sound'))
    except Exception as _field_exc:
        result['is_unmeasured_automation_handover'] = None
        result.setdefault('_erb_errors', {})['is_unmeasured_automation_handover'] = str(_field_exc)
    try:
        result['is_standing_unreviewed_automation'] = calc_role_assignments_is_standing_unreviewed_automation(result.get('covers_now'), result.get('is_non_human_assignment'), result.get('authorization_is_overdue_for_review'))
    except Exception as _field_exc:
        result['is_standing_unreviewed_automation'] = None
        result.setdefault('_erb_errors', {})['is_standing_unreviewed_automation'] = str(_field_exc)
    try:
        result['is_unconditioned_automation_handover'] = calc_role_assignments_is_unconditioned_automation_handover(result.get('is_human_to_non_human_handover'), result.get('authorization_review_cadence_days'))
    except Exception as _field_exc:
        result['is_unconditioned_automation_handover'] = None
        result.setdefault('_erb_errors', {})['is_unconditioned_automation_handover'] = str(_field_exc)
    try:
        result['suspension_condition_met'] = calc_role_assignments_suspension_condition_met(result.get('exceeds_tolerable_error_rate'), result.get('has_any_boundary_violation'), result.get('has_ungrounded_governing_boundary'))
    except Exception as _field_exc:
        result['suspension_condition_met'] = None
        result.setdefault('_erb_errors', {})['suspension_condition_met'] = str(_field_exc)
    try:
        result['is_unauthorized_enforcement_agent'] = calc_role_assignments_is_unauthorized_enforcement_agent(result.get('is_enforcement_role'), result.get('is_unauthorized_non_human_assignment'))
    except Exception as _field_exc:
        result['is_unauthorized_enforcement_agent'] = None
        result.setdefault('_erb_errors', {})['is_unauthorized_enforcement_agent'] = str(_field_exc)
    try:
        result['unauthorized_enforcement_role_key'] = calc_role_assignments_unauthorized_enforcement_role_key(result.get('is_unauthorized_non_human_assignment'), result.get('role'))
    except Exception as _field_exc:
        result['unauthorized_enforcement_role_key'] = None
        result.setdefault('_erb_errors', {})['unauthorized_enforcement_role_key'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['is_operating_under_met_suspension_condition'] = calc_role_assignments_is_operating_under_met_suspension_condition(result.get('suspension_condition_met'), result.get('covers_now'), result.get('is_non_human_assignment'))
    except Exception as _field_exc:
        result['is_operating_under_met_suspension_condition'] = None
        result.setdefault('_erb_errors', {})['is_operating_under_met_suspension_condition'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'current_agent_key', 'agent_role_key', 'role_when_covering', 'departed_role_key', 'unauthorized_enforcement_role_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# COMMUNITIESOFPRACTICE
# Socio-technical communities that transmit and maintain procedural knowledge. Explicit ERB-PKO extension.
# =============================================================================

# Level 1

def calc_communities_of_practice_name(label):
    """
    Human-readable calculated display alias for the CommunitiesOfPractice row.
    
    Formula: ={{Label}}
    """
    return label


def compute_communities_of_practice_fields(record: dict) -> dict:
    """
    Compute all calculated fields for CommunitiesOfPractice.
    
    Socio-technical communities that transmit and maintain procedural knowledge. Explicit ERB-PKO extension.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_communities_of_practice_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# MENTORSHIPS
# Time-bounded apprenticeship relationships that intentionally transfer situated procedural knowledge. Explicit ERB-PKO extension.
# =============================================================================

# Level 1

def calc_mentorships_name(mentor_agent, learner_agent):
    """
    Human-readable calculated display alias for the Mentorships row.
    
    Formula: ={{MentorAgent}} & " -> " & {{LearnerAgent}}
    """
    return (str(mentor_agent or "") + ' -> ' + str(learner_agent or ""))


def compute_mentorships_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Mentorships.
    
    Time-bounded apprenticeship relationships that intentionally transfer situated procedural knowledge. Explicit ERB-PKO extension.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_mentorships_name(result.get('mentor_agent'), result.get('learner_agent'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDURETYPES
# Controlled values used by pko:hasProcedureType.
# =============================================================================

# Level 1

def calc_procedure_types_name(label):
    """
    Human-readable calculated display alias for the ProcedureTypes row.
    
    Formula: ={{Label}}
    """
    return label


def compute_procedure_types_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcedureTypes.
    
    Controlled values used by pko:hasProcedureType.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedure_types_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDURES
# Abstract, discoverable procedures. Each version is represented separately in ProcedureVersions. Maps to pko:Procedure and dcat:Resource.
# =============================================================================

# Level 1

def calc_procedures_name(title):
    """
    Human-readable calculated display alias for the Procedures row.
    
    Formula: ={{Title}}
    """
    return title


def compute_procedures_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Procedures.
    
    Abstract, discoverable procedures. Each version is represented separately in ProcedureVersions. Maps to pko:Procedure and dcat:Resource.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedures_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDUREVERSIONS
# Versioned procedure specifications. Maps to pko:Procedure plus DCAT version relations and PKO versionNumber/newVersionMotivation/changelogDescription.
# =============================================================================

# Level 1

def calc_procedure_versions_name(title):
    """
    Human-readable calculated display alias for the ProcedureVersions row.
    
    Formula: ={{Title}}
    """
    return title

def calc_procedure_versions_is_ready_for_execution(status, count_of_steps, count_of_open_knowledge_gaps):
    """
    TRUE when approved, populated, and free of blocking knowledge gaps.
    
    Formula: =AND({{Status}} = "Approved", {{CountOfSteps}} > 0, {{CountOfOpenKnowledgeGaps}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_cmp(count_of_steps, '>', 0)), _erb.erb_bool3(_erb.erb_eq(count_of_open_knowledge_gaps, 0)))

def calc_procedure_versions_is_fit_to_execute(status, overdue_review_count, open_change_request_count, open_high_severity_gap_count):
    """
    TRUE when this version is approved, current on review, and carries no open change request or high-severity gap.
    
    Formula: =AND({{Status}} = "Approved", {{OverdueReviewCount}} = 0, {{OpenChangeRequestCount}} = 0, {{OpenHighSeverityGapCount}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_eq(overdue_review_count, 0)), _erb.erb_bool3(_erb.erb_eq(open_change_request_count, 0)), _erb.erb_bool3(_erb.erb_eq(open_high_severity_gap_count, 0)))

def calc_procedure_versions_has_any_steward(count_of_stewardship_assignments):
    """
    TRUE if any stewardship assignment has ever named this version.
    
    Formula: ={{CountOfStewardshipAssignments}} > 0
    """
    return _erb.erb_cmp(count_of_stewardship_assignments, '>', 0)

def calc_procedure_versions_is_live(status):
    """
    TRUE when this version is in a state where somebody could execute it.
    
    Formula: =OR({{Status}} = "Approved", {{Status}} = "Published")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Published')))

def calc_procedure_versions_has_open_blocking_gap(count_of_open_blocking_gaps):
    """
    TRUE when at least one open blocking gap stands against this version.
    
    Formula: ={{CountOfOpenBlockingGaps}} > 0
    """
    return _erb.erb_cmp(count_of_open_blocking_gaps, '>', 0)

def calc_procedure_versions_runs_on_unapproved_knowledge(count_of_unapproved_reliance_fragments):
    """
    TRUE when this version depends on at least one claim the knowledge authority has not approved.
    
    Formula: ={{CountOfUnapprovedRelianceFragments}} > 0
    """
    return _erb.erb_cmp(count_of_unapproved_reliance_fragments, '>', 0)

def calc_procedure_versions_has_governance_record(count_of_change_requests, count_of_review_events):
    """
    TRUE when at least one change request or review event exists for this version.
    
    Formula: =OR({{CountOfChangeRequests}} > 0, {{CountOfReviewEvents}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(count_of_change_requests, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(count_of_review_events, '>', 0)))

def calc_procedure_versions_days_since_modified(as_of_instant, modified_at):
    """
    Days since this version's content was last changed.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{ModifiedAt}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, modified_at, 'days'))

def calc_procedure_versions_knowledge_is_staler_than_cadence(count_of_stale_fragments):
    """
    TRUE when this version rests on at least one claim older than its own review cadence.
    
    Formula: ={{CountOfStaleFragments}} > 0
    """
    return _erb.erb_cmp(count_of_stale_fragments, '>', 0)

def calc_procedure_versions_is_blocked_on_pending_decision(awaited_decision_count, scoped_open_blocking_gap_count):
    """
    A live version carrying both an undecided change request and an open blocking gap — the gap cannot close until the decision lands.
    
    Formula: =AND({{AwaitedDecisionCount}} > 0, {{ScopedOpenBlockingGapCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(awaited_decision_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(scoped_open_blocking_gap_count, '>', 0)))

def calc_procedure_versions_has_unrehearsed_control_entry(unrehearsed_control_entry_count):
    """
    Whether this live version has at least one blocking control that is only reachable by a path nobody has ever walked.
    
    Formula: ={{UnrehearsedControlEntryCount}} > 0
    """
    return _erb.erb_cmp(unrehearsed_control_entry_count, '>', 0)

def calc_procedure_versions_is_in_cadence_breach(cadence_breach_count):
    """
    Whether this version currently has at least one review event past the cadence its steward promised.
    
    Formula: ={{CadenceBreachCount}} > 0
    """
    return _erb.erb_cmp(cadence_breach_count, '>', 0)

def calc_procedure_versions_has_decision_in_flight(open_change_request_count):
    """
    Whether this version has at least one change request that is still open.
    
    Formula: ={{OpenChangeRequestCount}} > 0
    """
    return _erb.erb_cmp(open_change_request_count, '>', 0)

def calc_procedure_versions_still_owns_valid_knowledge(valid_fragment_count):
    """
    Whether this version still holds at least one knowledge fragment that is currently valid.
    
    Formula: ={{ValidFragmentCount}} > 0
    """
    return _erb.erb_cmp(valid_fragment_count, '>', 0)

def calc_procedure_versions_is_still_referenced(incoming_supersession_count):
    """
    Whether any other version points at this one through a supersession link.
    
    Formula: ={{IncomingSupersessionCount}} > 0
    """
    return _erb.erb_cmp(incoming_supersession_count, '>', 0)

def calc_procedure_versions_control_taxonomy_is_incomplete(undeclared_control_kind_count):
    """
    Whether this version contains any step whose control kind is unstated — meaning role-based and id-based control predicates cannot be trusted to cover it.
    
    Formula: ={{UndeclaredControlKindCount}} > 0
    """
    return _erb.erb_cmp(undeclared_control_kind_count, '>', 0)

def calc_procedure_versions_has_approved_change_request(approved_change_request_count):
    """
    TRUE when at least one change request against this version has been approved.
    
    Formula: ={{ApprovedChangeRequestCount}} > 0
    """
    return _erb.erb_cmp(approved_change_request_count, '>', 0)

def calc_procedure_versions_has_unresolved_mining_drift(drifted_mining_run_count):
    """
    TRUE when mined operational evidence contradicts this live version's documented path.
    
    Formula: ={{DriftedMiningRunCount}} > 0
    """
    return _erb.erb_cmp(drifted_mining_run_count, '>', 0)

# Level 2

def calc_procedure_versions_is_unstewarded(has_any_steward):
    """
    TRUE when no stewardship assignment names this version.
    
    Formula: =NOT({{HasAnySteward}})
    """
    return _erb.erb_not(_erb.erb_bool3(has_any_steward))

def calc_procedure_versions_is_live_with_blocking_gap(is_live, has_open_blocking_gap):
    """
    TRUE when an executable version carries an unresolved blocking knowledge gap.
    
    Formula: =AND({{IsLive}}, {{HasOpenBlockingGap}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(has_open_blocking_gap))

def calc_procedure_versions_should_not_be_executable(is_ready_for_execution, has_open_blocking_gap):
    """
    TRUE when the model says this version is ready to execute while a blocking gap is open against it.
    
    Formula: =AND({{IsReadyForExecution}}, {{HasOpenBlockingGap}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_ready_for_execution), _erb.erb_bool3(has_open_blocking_gap))

def calc_procedure_versions_was_modified_since_last_review(days_since_modified, days_since_last_review):
    """
    TRUE when the version was edited more recently than it was reviewed.
    
    Formula: ={{DaysSinceModified}} < {{DaysSinceLastReview}}
    """
    return _erb.erb_cmp(days_since_modified, '<', days_since_last_review)

def calc_procedure_versions_rests_on_compound_fragile_knowledge(is_live, compound_fragile_fragment_count):
    """
    A live procedure version resting on at least one knowledge fragment that carries three or more decay signals.
    
    Formula: =AND({{IsLive}}, {{CompoundFragileFragmentCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(_erb.erb_cmp(compound_fragile_fragment_count, '>', 0)))

def calc_procedure_versions_knowledge_base_is_concentrated(is_live, concentrated_witness_session_count):
    """
    A live version where at least one single-witness session alone underwrites three or more of its live claims.
    
    Formula: =AND({{IsLive}}, {{ConcentratedWitnessSessionCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(_erb.erb_cmp(concentrated_witness_session_count, '>', 0)))

def calc_procedure_versions_feeds_unapproved_knowledge_to_machines(is_live, machine_consumed_unapproved_count):
    """
    A live version that hands unapproved knowledge to a step no human is positioned to review.
    
    Formula: =AND({{IsLive}}, {{MachineConsumedUnapprovedCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(_erb.erb_cmp(machine_consumed_unapproved_count, '>', 0)))

def calc_procedure_versions_ai_boundary_is_unevidenced(is_live, unexercised_human_gate_count):
    """
    A live version whose human-only gates rest on assertion rather than on any observed attempt by software.
    
    Formula: =AND({{IsLive}}, {{UnexercisedHumanGateCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(_erb.erb_cmp(unexercised_human_gate_count, '>', 0)))

def calc_procedure_versions_is_live_with_unrehearsed_control(is_live, has_unrehearsed_control_entry):
    """
    A version that is live for execution while carrying at least one never-rehearsed blocking control entry.
    
    Formula: =AND({{IsLive}}, {{HasUnrehearsedControlEntry}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(has_unrehearsed_control_entry))

def calc_procedure_versions_is_unremediated_cadence_breach(is_in_cadence_breach, has_decision_in_flight):
    """
    A cadence breach with no open change request against the version — a broken promise with no response in motion.
    
    Formula: =AND({{IsInCadenceBreach}}, NOT({{HasDecisionInFlight}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_in_cadence_breach), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_decision_in_flight))))

def calc_procedure_versions_is_managed_cadence_breach(is_in_cadence_breach, has_decision_in_flight):
    """
    A cadence breach where a change request is at least open against the version.
    
    Formula: =AND({{IsInCadenceBreach}}, {{HasDecisionInFlight}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_in_cadence_breach), _erb.erb_bool3(has_decision_in_flight))

def calc_procedure_versions_governance_is_silent(is_live, has_governance_record):
    """
    A live version with neither a change request nor a review event ever recorded against it.
    
    Formula: =AND({{IsLive}}, NOT({{HasGovernanceRecord}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_governance_record))))

def calc_procedure_versions_is_held_unfit_by_landed_decisions(is_fit_to_execute, stalled_implementation_count):
    """
    A version reading unfit to execute specifically because approved changes have not been marked implemented.
    
    Formula: =AND(NOT({{IsFitToExecute}}), {{StalledImplementationCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fit_to_execute))), _erb.erb_bool3(_erb.erb_cmp(stalled_implementation_count, '>', 0)))

# Level 3

def calc_procedure_versions_is_live_and_unstewarded(is_live, is_unstewarded):
    """
    TRUE when an executable version has nobody accountable for keeping it healthy.
    
    Formula: =AND({{IsLive}}, {{IsUnstewarded}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(is_unstewarded))

def calc_procedure_versions_has_unwitnessed_change(is_live, was_modified_since_last_review):
    """
    TRUE when a live version's current content postdates every review it has had.
    
    Formula: =AND({{IsLive}}, {{WasModifiedSinceLastReview}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_live), _erb.erb_bool3(was_modified_since_last_review))

def calc_procedure_versions_is_load_bearing_orphan(is_unstewarded, still_owns_valid_knowledge, is_still_referenced):
    """
    An unstewarded version that is still referenced by a supersession link or still owns currently-valid knowledge — nobody is accountable for it and something still depends on it.
    
    Formula: =AND({{IsUnstewarded}}, OR({{StillOwnsValidKnowledge}}, {{IsStillReferenced}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_unstewarded), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(still_owns_valid_knowledge), _erb.erb_bool3(is_still_referenced))))

def calc_procedure_versions_is_cleanly_retired(is_unstewarded, still_owns_valid_knowledge, is_still_referenced):
    """
    An unstewarded version that nothing depends on — a genuine, safe retirement.
    
    Formula: =AND({{IsUnstewarded}}, NOT({{StillOwnsValidKnowledge}}), NOT({{IsStillReferenced}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_unstewarded), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(still_owns_valid_knowledge))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_still_referenced))))


def compute_procedure_versions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcedureVersions.
    
    Versioned procedure specifications. Maps to pko:Procedure plus DCAT version relations and PKO versionNumber/newVersionMotivation/changelogDescription.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedure_versions_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_ready_for_execution'] = calc_procedure_versions_is_ready_for_execution(result.get('status'), result.get('count_of_steps'), result.get('count_of_open_knowledge_gaps'))
    except Exception as _field_exc:
        result['is_ready_for_execution'] = None
        result.setdefault('_erb_errors', {})['is_ready_for_execution'] = str(_field_exc)
    try:
        result['is_fit_to_execute'] = calc_procedure_versions_is_fit_to_execute(result.get('status'), result.get('overdue_review_count'), result.get('open_change_request_count'), result.get('open_high_severity_gap_count'))
    except Exception as _field_exc:
        result['is_fit_to_execute'] = None
        result.setdefault('_erb_errors', {})['is_fit_to_execute'] = str(_field_exc)
    try:
        result['has_any_steward'] = calc_procedure_versions_has_any_steward(result.get('count_of_stewardship_assignments'))
    except Exception as _field_exc:
        result['has_any_steward'] = None
        result.setdefault('_erb_errors', {})['has_any_steward'] = str(_field_exc)
    try:
        result['is_live'] = calc_procedure_versions_is_live(result.get('status'))
    except Exception as _field_exc:
        result['is_live'] = None
        result.setdefault('_erb_errors', {})['is_live'] = str(_field_exc)
    try:
        result['has_open_blocking_gap'] = calc_procedure_versions_has_open_blocking_gap(result.get('count_of_open_blocking_gaps'))
    except Exception as _field_exc:
        result['has_open_blocking_gap'] = None
        result.setdefault('_erb_errors', {})['has_open_blocking_gap'] = str(_field_exc)
    try:
        result['runs_on_unapproved_knowledge'] = calc_procedure_versions_runs_on_unapproved_knowledge(result.get('count_of_unapproved_reliance_fragments'))
    except Exception as _field_exc:
        result['runs_on_unapproved_knowledge'] = None
        result.setdefault('_erb_errors', {})['runs_on_unapproved_knowledge'] = str(_field_exc)
    try:
        result['has_governance_record'] = calc_procedure_versions_has_governance_record(result.get('count_of_change_requests'), result.get('count_of_review_events'))
    except Exception as _field_exc:
        result['has_governance_record'] = None
        result.setdefault('_erb_errors', {})['has_governance_record'] = str(_field_exc)
    try:
        result['days_since_modified'] = calc_procedure_versions_days_since_modified(result.get('as_of_instant'), result.get('modified_at'))
    except Exception as _field_exc:
        result['days_since_modified'] = None
        result.setdefault('_erb_errors', {})['days_since_modified'] = str(_field_exc)
    try:
        result['knowledge_is_staler_than_cadence'] = calc_procedure_versions_knowledge_is_staler_than_cadence(result.get('count_of_stale_fragments'))
    except Exception as _field_exc:
        result['knowledge_is_staler_than_cadence'] = None
        result.setdefault('_erb_errors', {})['knowledge_is_staler_than_cadence'] = str(_field_exc)
    try:
        result['is_blocked_on_pending_decision'] = calc_procedure_versions_is_blocked_on_pending_decision(result.get('awaited_decision_count'), result.get('scoped_open_blocking_gap_count'))
    except Exception as _field_exc:
        result['is_blocked_on_pending_decision'] = None
        result.setdefault('_erb_errors', {})['is_blocked_on_pending_decision'] = str(_field_exc)
    try:
        result['has_unrehearsed_control_entry'] = calc_procedure_versions_has_unrehearsed_control_entry(result.get('unrehearsed_control_entry_count'))
    except Exception as _field_exc:
        result['has_unrehearsed_control_entry'] = None
        result.setdefault('_erb_errors', {})['has_unrehearsed_control_entry'] = str(_field_exc)
    try:
        result['is_in_cadence_breach'] = calc_procedure_versions_is_in_cadence_breach(result.get('cadence_breach_count'))
    except Exception as _field_exc:
        result['is_in_cadence_breach'] = None
        result.setdefault('_erb_errors', {})['is_in_cadence_breach'] = str(_field_exc)
    try:
        result['has_decision_in_flight'] = calc_procedure_versions_has_decision_in_flight(result.get('open_change_request_count'))
    except Exception as _field_exc:
        result['has_decision_in_flight'] = None
        result.setdefault('_erb_errors', {})['has_decision_in_flight'] = str(_field_exc)
    try:
        result['still_owns_valid_knowledge'] = calc_procedure_versions_still_owns_valid_knowledge(result.get('valid_fragment_count'))
    except Exception as _field_exc:
        result['still_owns_valid_knowledge'] = None
        result.setdefault('_erb_errors', {})['still_owns_valid_knowledge'] = str(_field_exc)
    try:
        result['is_still_referenced'] = calc_procedure_versions_is_still_referenced(result.get('incoming_supersession_count'))
    except Exception as _field_exc:
        result['is_still_referenced'] = None
        result.setdefault('_erb_errors', {})['is_still_referenced'] = str(_field_exc)
    try:
        result['control_taxonomy_is_incomplete'] = calc_procedure_versions_control_taxonomy_is_incomplete(result.get('undeclared_control_kind_count'))
    except Exception as _field_exc:
        result['control_taxonomy_is_incomplete'] = None
        result.setdefault('_erb_errors', {})['control_taxonomy_is_incomplete'] = str(_field_exc)
    try:
        result['has_approved_change_request'] = calc_procedure_versions_has_approved_change_request(result.get('approved_change_request_count'))
    except Exception as _field_exc:
        result['has_approved_change_request'] = None
        result.setdefault('_erb_errors', {})['has_approved_change_request'] = str(_field_exc)
    try:
        result['has_unresolved_mining_drift'] = calc_procedure_versions_has_unresolved_mining_drift(result.get('drifted_mining_run_count'))
    except Exception as _field_exc:
        result['has_unresolved_mining_drift'] = None
        result.setdefault('_erb_errors', {})['has_unresolved_mining_drift'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_unstewarded'] = calc_procedure_versions_is_unstewarded(result.get('has_any_steward'))
    except Exception as _field_exc:
        result['is_unstewarded'] = None
        result.setdefault('_erb_errors', {})['is_unstewarded'] = str(_field_exc)
    try:
        result['is_live_with_blocking_gap'] = calc_procedure_versions_is_live_with_blocking_gap(result.get('is_live'), result.get('has_open_blocking_gap'))
    except Exception as _field_exc:
        result['is_live_with_blocking_gap'] = None
        result.setdefault('_erb_errors', {})['is_live_with_blocking_gap'] = str(_field_exc)
    try:
        result['should_not_be_executable'] = calc_procedure_versions_should_not_be_executable(result.get('is_ready_for_execution'), result.get('has_open_blocking_gap'))
    except Exception as _field_exc:
        result['should_not_be_executable'] = None
        result.setdefault('_erb_errors', {})['should_not_be_executable'] = str(_field_exc)
    try:
        result['was_modified_since_last_review'] = calc_procedure_versions_was_modified_since_last_review(result.get('days_since_modified'), result.get('days_since_last_review'))
    except Exception as _field_exc:
        result['was_modified_since_last_review'] = None
        result.setdefault('_erb_errors', {})['was_modified_since_last_review'] = str(_field_exc)
    try:
        result['rests_on_compound_fragile_knowledge'] = calc_procedure_versions_rests_on_compound_fragile_knowledge(result.get('is_live'), result.get('compound_fragile_fragment_count'))
    except Exception as _field_exc:
        result['rests_on_compound_fragile_knowledge'] = None
        result.setdefault('_erb_errors', {})['rests_on_compound_fragile_knowledge'] = str(_field_exc)
    try:
        result['knowledge_base_is_concentrated'] = calc_procedure_versions_knowledge_base_is_concentrated(result.get('is_live'), result.get('concentrated_witness_session_count'))
    except Exception as _field_exc:
        result['knowledge_base_is_concentrated'] = None
        result.setdefault('_erb_errors', {})['knowledge_base_is_concentrated'] = str(_field_exc)
    try:
        result['feeds_unapproved_knowledge_to_machines'] = calc_procedure_versions_feeds_unapproved_knowledge_to_machines(result.get('is_live'), result.get('machine_consumed_unapproved_count'))
    except Exception as _field_exc:
        result['feeds_unapproved_knowledge_to_machines'] = None
        result.setdefault('_erb_errors', {})['feeds_unapproved_knowledge_to_machines'] = str(_field_exc)
    try:
        result['ai_boundary_is_unevidenced'] = calc_procedure_versions_ai_boundary_is_unevidenced(result.get('is_live'), result.get('unexercised_human_gate_count'))
    except Exception as _field_exc:
        result['ai_boundary_is_unevidenced'] = None
        result.setdefault('_erb_errors', {})['ai_boundary_is_unevidenced'] = str(_field_exc)
    try:
        result['is_live_with_unrehearsed_control'] = calc_procedure_versions_is_live_with_unrehearsed_control(result.get('is_live'), result.get('has_unrehearsed_control_entry'))
    except Exception as _field_exc:
        result['is_live_with_unrehearsed_control'] = None
        result.setdefault('_erb_errors', {})['is_live_with_unrehearsed_control'] = str(_field_exc)
    try:
        result['is_unremediated_cadence_breach'] = calc_procedure_versions_is_unremediated_cadence_breach(result.get('is_in_cadence_breach'), result.get('has_decision_in_flight'))
    except Exception as _field_exc:
        result['is_unremediated_cadence_breach'] = None
        result.setdefault('_erb_errors', {})['is_unremediated_cadence_breach'] = str(_field_exc)
    try:
        result['is_managed_cadence_breach'] = calc_procedure_versions_is_managed_cadence_breach(result.get('is_in_cadence_breach'), result.get('has_decision_in_flight'))
    except Exception as _field_exc:
        result['is_managed_cadence_breach'] = None
        result.setdefault('_erb_errors', {})['is_managed_cadence_breach'] = str(_field_exc)
    try:
        result['governance_is_silent'] = calc_procedure_versions_governance_is_silent(result.get('is_live'), result.get('has_governance_record'))
    except Exception as _field_exc:
        result['governance_is_silent'] = None
        result.setdefault('_erb_errors', {})['governance_is_silent'] = str(_field_exc)
    try:
        result['is_held_unfit_by_landed_decisions'] = calc_procedure_versions_is_held_unfit_by_landed_decisions(result.get('is_fit_to_execute'), result.get('stalled_implementation_count'))
    except Exception as _field_exc:
        result['is_held_unfit_by_landed_decisions'] = None
        result.setdefault('_erb_errors', {})['is_held_unfit_by_landed_decisions'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_live_and_unstewarded'] = calc_procedure_versions_is_live_and_unstewarded(result.get('is_live'), result.get('is_unstewarded'))
    except Exception as _field_exc:
        result['is_live_and_unstewarded'] = None
        result.setdefault('_erb_errors', {})['is_live_and_unstewarded'] = str(_field_exc)
    try:
        result['has_unwitnessed_change'] = calc_procedure_versions_has_unwitnessed_change(result.get('is_live'), result.get('was_modified_since_last_review'))
    except Exception as _field_exc:
        result['has_unwitnessed_change'] = None
        result.setdefault('_erb_errors', {})['has_unwitnessed_change'] = str(_field_exc)
    try:
        result['is_load_bearing_orphan'] = calc_procedure_versions_is_load_bearing_orphan(result.get('is_unstewarded'), result.get('still_owns_valid_knowledge'), result.get('is_still_referenced'))
    except Exception as _field_exc:
        result['is_load_bearing_orphan'] = None
        result.setdefault('_erb_errors', {})['is_load_bearing_orphan'] = str(_field_exc)
    try:
        result['is_cleanly_retired'] = calc_procedure_versions_is_cleanly_retired(result.get('is_unstewarded'), result.get('still_owns_valid_knowledge'), result.get('is_still_referenced'))
    except Exception as _field_exc:
        result['is_cleanly_retired'] = None
        result.setdefault('_erb_errors', {})['is_cleanly_retired'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDUREVERSIONLINKS
# Directed links between versioned procedures. Maps to dcat:previousVersion/dcat:hasVersion and pko:nextVersion.
# =============================================================================

# Level 1

def calc_procedure_version_links_name(previous_procedure_version, next_procedure_version):
    """
    Human-readable calculated display alias for the ProcedureVersionLinks row.
    
    Formula: ={{PreviousProcedureVersion}} & " -> " & {{NextProcedureVersion}}
    """
    return (str(previous_procedure_version or "") + ' -> ' + str(next_procedure_version or ""))

def calc_procedure_version_links_superseded_version_key(relation_iri, previous_procedure_version):
    """
    Echoes the superseded (previous) version id for rows that express a next-version relation. Supersession is carried by RelationIri here, not by a separate link-kind column.
    
    Formula: =IF({{RelationIri}} = "https://w3id.org/pko#nextVersion", {{PreviousProcedureVersion}}, "")
    """
    return (previous_procedure_version if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(relation_iri), 'https://w3id.org/pko#nextVersion')) else '')


def compute_procedure_version_links_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcedureVersionLinks.
    
    Directed links between versioned procedures. Maps to dcat:previousVersion/dcat:hasVersion and pko:nextVersion.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedure_version_links_name(result.get('previous_procedure_version'), result.get('next_procedure_version'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['superseded_version_key'] = calc_procedure_version_links_superseded_version_key(result.get('relation_iri'), result.get('previous_procedure_version'))
    except Exception as _field_exc:
        result['superseded_version_key'] = None
        result.setdefault('_erb_errors', {})['superseded_version_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'superseded_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDURESTATUSCHANGES
# Lifecycle events that move a procedure version between PKO statuses. Maps to pko:ChangeOfStatus, fromStatus, toStatus, and prov:atTime.
# =============================================================================

# Level 1

def calc_procedure_status_changes_name(procedure_version, from_status, to_status):
    """
    Human-readable calculated display alias for the ProcedureStatusChanges row.
    
    Formula: ={{ProcedureVersion}} & ": " & {{FromStatus}} & " -> " & {{ToStatus}}
    """
    return (str(procedure_version or "") + ': ' + str(from_status or "") + ' -> ' + str(to_status or ""))


def compute_procedure_status_changes_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcedureStatusChanges.
    
    Lifecycle events that move a procedure version between PKO statuses. Maps to pko:ChangeOfStatus, fromStatus, toStatus, and prov:atTime.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedure_status_changes_name(result.get('procedure_version'), result.get('from_status'), result.get('to_status'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPS
# Version-scoped units of work. Atomic steps map to pplan:Step; composite steps map to pplan:MultiStep. The specification is never conflated with execution.
# =============================================================================

# Level 1

def calc_steps_name(step_number, title):
    """
    Human-readable calculated display alias for the Steps row.
    
    Formula: ={{StepNumber}} & ". " & {{Title}}
    """
    return (str(step_number or "") + '. ' + str(title or ""))

def calc_steps_is_preparation_step(assigned_role):
    """
    TRUE for steps whose assigned role produces the work product rather than reviewing it.
    
    Formula: =OR({{AssignedRole}} = "finance-analyst", {{AssignedRole}} = "variance-review-agent")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(assigned_role), 'finance-analyst')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(assigned_role), 'variance-review-agent')))

def calc_steps_is_approval_step(assigned_role):
    """
    TRUE for steps whose assigned role is an approval authority.
    
    Formula: =OR({{AssignedRole}} = "controller", {{AssignedRole}} = "cfo")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(assigned_role), 'controller')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(assigned_role), 'cfo')))

def calc_steps_inputs_are_fresh(stale_authoritative_binding_count):
    """
    TRUE when no authoritative binding for this step is stale.
    
    Formula: ={{StaleAuthoritativeBindingCount}} = 0
    """
    return _erb.erb_eq(stale_authoritative_binding_count, 0)

def calc_steps_is_software_assigned(assigned_agent_kind):
    """
    TRUE when this step is specified to be performed by software rather than a person.
    
    Formula: =OR({{AssignedAgentKind}} = "AIAgent", {{AssignedAgentKind}} = "AutomatedPipeline")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(assigned_agent_kind, 'AIAgent')), _erb.erb_bool3(_erb.erb_eq(assigned_agent_kind, 'AutomatedPipeline')))

def calc_steps_all_sources_usable(unusable_binding_count):
    """
    TRUE when every binding at this step is an approved, fresh source.
    
    Formula: ={{UnusableBindingCount}} = 0
    """
    return _erb.erb_eq(unusable_binding_count, 0)

def calc_steps_is_governed_by_unwarranted_boundary(unwarranted_boundary_count):
    """
    Whether this step's constraints on machine authority rest on a claim that is no longer valid.
    
    Formula: ={{UnwarrantedBoundaryCount}} > 0
    """
    return _erb.erb_cmp(unwarranted_boundary_count, '>', 0)

def calc_steps_has_been_approached_by_software(software_execution_count):
    """
    Whether any software agent has ever executed this step.
    
    Formula: ={{SoftwareExecutionCount}} > 0
    """
    return _erb.erb_cmp(software_execution_count, '>', 0)

def calc_steps_has_declared_control_kind(control_kind):
    """
    Whether this step declares what kind of control it is.
    
    Formula: ={{ControlKind}} <> "" 
    """
    return (not (control_kind is None or control_kind == ""))

# Level 2

def calc_steps_is_human_approval_gate(is_software_assigned, step_id):
    """
    TRUE for the steps that exist specifically to place a human commitment between drafting and delivery.
    
    Formula: =AND(NOT({{IsSoftwareAssigned}}), OR({{StepId}} = "policy-05", {{StepId}} = "close-06"))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_software_assigned))), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(step_id), 'policy-05')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(step_id), 'close-06')))))

def calc_steps_undeclared_control_version_key(has_declared_control_kind, procedure_version):
    """
    Composite-key echo: this step's procedure version when the step has no declared control kind, blank otherwise.
    
    Formula: =IF({{HasDeclaredControlKind}}, "", {{ProcedureVersion}})
    """
    return ('' if _erb.erb_bool3(has_declared_control_kind) else procedure_version)

def calc_steps_approval_step_is_software_assigned(control_kind, is_software_assigned):
    """
    An approval-kind step whose assigned role is currently held by an AI agent or automated pipeline.
    
    Formula: =AND({{ControlKind}} = "Approval", {{IsSoftwareAssigned}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(control_kind), 'Approval')), _erb.erb_bool3(is_software_assigned))

# Level 3

def calc_steps_gate_held_by_human(is_human_approval_gate, assigned_agent_kind):
    """
    TRUE when a designated approval gate is in fact assigned to a human role.
    
    Formula: =AND({{IsHumanApprovalGate}}, {{AssignedAgentKind}} = "Human")
    """
    return _erb.erb_and(_erb.erb_bool3(is_human_approval_gate), _erb.erb_bool3(_erb.erb_eq(assigned_agent_kind, 'Human')))

def calc_steps_is_unexercised_human_gate(is_human_approval_gate, has_been_approached_by_software):
    """
    A human-only approval gate that no software agent has ever attempted — the control is asserted, not demonstrated.
    
    Formula: =AND({{IsHumanApprovalGate}}, NOT({{HasBeenApproachedBySoftware}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_human_approval_gate), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_been_approached_by_software))))

# Level 4

def calc_steps_is_demonstrated_human_gate(is_human_approval_gate, has_been_approached_by_software, gate_held_by_human):
    """
    A human gate that software has actually reached and that a human nevertheless held.
    
    Formula: =AND({{IsHumanApprovalGate}}, {{HasBeenApproachedBySoftware}}, {{GateHeldByHuman}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_human_approval_gate), _erb.erb_bool3(has_been_approached_by_software), _erb.erb_bool3(gate_held_by_human))

def calc_steps_unexercised_gate_version_key(is_unexercised_human_gate, procedure_version):
    """
    Composite-key echo: this step's procedure version when the step is an unexercised human gate, blank otherwise.
    
    Formula: =IF({{IsUnexercisedHumanGate}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_unexercised_human_gate) else '')


def compute_steps_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Steps.
    
    Version-scoped units of work. Atomic steps map to pplan:Step; composite steps map to pplan:MultiStep. The specification is never conflated with execution.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_steps_name(result.get('step_number'), result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_preparation_step'] = calc_steps_is_preparation_step(result.get('assigned_role'))
    except Exception as _field_exc:
        result['is_preparation_step'] = None
        result.setdefault('_erb_errors', {})['is_preparation_step'] = str(_field_exc)
    try:
        result['is_approval_step'] = calc_steps_is_approval_step(result.get('assigned_role'))
    except Exception as _field_exc:
        result['is_approval_step'] = None
        result.setdefault('_erb_errors', {})['is_approval_step'] = str(_field_exc)
    try:
        result['inputs_are_fresh'] = calc_steps_inputs_are_fresh(result.get('stale_authoritative_binding_count'))
    except Exception as _field_exc:
        result['inputs_are_fresh'] = None
        result.setdefault('_erb_errors', {})['inputs_are_fresh'] = str(_field_exc)
    try:
        result['is_software_assigned'] = calc_steps_is_software_assigned(result.get('assigned_agent_kind'))
    except Exception as _field_exc:
        result['is_software_assigned'] = None
        result.setdefault('_erb_errors', {})['is_software_assigned'] = str(_field_exc)
    try:
        result['all_sources_usable'] = calc_steps_all_sources_usable(result.get('unusable_binding_count'))
    except Exception as _field_exc:
        result['all_sources_usable'] = None
        result.setdefault('_erb_errors', {})['all_sources_usable'] = str(_field_exc)
    try:
        result['is_governed_by_unwarranted_boundary'] = calc_steps_is_governed_by_unwarranted_boundary(result.get('unwarranted_boundary_count'))
    except Exception as _field_exc:
        result['is_governed_by_unwarranted_boundary'] = None
        result.setdefault('_erb_errors', {})['is_governed_by_unwarranted_boundary'] = str(_field_exc)
    try:
        result['has_been_approached_by_software'] = calc_steps_has_been_approached_by_software(result.get('software_execution_count'))
    except Exception as _field_exc:
        result['has_been_approached_by_software'] = None
        result.setdefault('_erb_errors', {})['has_been_approached_by_software'] = str(_field_exc)
    try:
        result['has_declared_control_kind'] = calc_steps_has_declared_control_kind(result.get('control_kind'))
    except Exception as _field_exc:
        result['has_declared_control_kind'] = None
        result.setdefault('_erb_errors', {})['has_declared_control_kind'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_human_approval_gate'] = calc_steps_is_human_approval_gate(result.get('is_software_assigned'), result.get('step_id'))
    except Exception as _field_exc:
        result['is_human_approval_gate'] = None
        result.setdefault('_erb_errors', {})['is_human_approval_gate'] = str(_field_exc)
    try:
        result['undeclared_control_version_key'] = calc_steps_undeclared_control_version_key(result.get('has_declared_control_kind'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['undeclared_control_version_key'] = None
        result.setdefault('_erb_errors', {})['undeclared_control_version_key'] = str(_field_exc)
    try:
        result['approval_step_is_software_assigned'] = calc_steps_approval_step_is_software_assigned(result.get('control_kind'), result.get('is_software_assigned'))
    except Exception as _field_exc:
        result['approval_step_is_software_assigned'] = None
        result.setdefault('_erb_errors', {})['approval_step_is_software_assigned'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['gate_held_by_human'] = calc_steps_gate_held_by_human(result.get('is_human_approval_gate'), result.get('assigned_agent_kind'))
    except Exception as _field_exc:
        result['gate_held_by_human'] = None
        result.setdefault('_erb_errors', {})['gate_held_by_human'] = str(_field_exc)
    try:
        result['is_unexercised_human_gate'] = calc_steps_is_unexercised_human_gate(result.get('is_human_approval_gate'), result.get('has_been_approached_by_software'))
    except Exception as _field_exc:
        result['is_unexercised_human_gate'] = None
        result.setdefault('_erb_errors', {})['is_unexercised_human_gate'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['is_demonstrated_human_gate'] = calc_steps_is_demonstrated_human_gate(result.get('is_human_approval_gate'), result.get('has_been_approached_by_software'), result.get('gate_held_by_human'))
    except Exception as _field_exc:
        result['is_demonstrated_human_gate'] = None
        result.setdefault('_erb_errors', {})['is_demonstrated_human_gate'] = str(_field_exc)
    try:
        result['unexercised_gate_version_key'] = calc_steps_unexercised_gate_version_key(result.get('is_unexercised_human_gate'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['unexercised_gate_version_key'] = None
        result.setdefault('_erb_errors', {})['unexercised_gate_version_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'unexercised_gate_version_key', 'undeclared_control_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPTRANSITIONS
# Directed control-flow edges represented as first-class pko:Transition instances with fromStep/toStep and next/alternative/fallback semantics.
# =============================================================================

# Level 1

def calc_step_transitions_name(from_step, to_step):
    """
    Human-readable calculated display alias for the StepTransitions row.
    
    Formula: ={{FromStep}} & " -> " & {{ToStep}}
    """
    return (str(from_step or "") + ' -> ' + str(to_step or ""))

def calc_step_transitions_is_recovery_path(transition_kind):
    """
    TRUE when this transition is a non-default path taken because something went wrong.
    
    Formula: =OR({{TransitionKind}} = "Fallback", {{TransitionKind}} = "Alternative")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(transition_kind), 'Fallback')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(transition_kind), 'Alternative')))

def calc_step_transitions_has_reachable_origin(count_of_from_step_executions):
    """
    TRUE when the origin step of this transition has been executed at least once.
    
    Formula: ={{CountOfFromStepExecutions}} > 0
    """
    return _erb.erb_cmp(count_of_from_step_executions, '>', 0)

def calc_step_transitions_has_reachable_target(count_of_to_step_executions):
    """
    TRUE when the destination step of this transition has been executed at least once.
    
    Formula: ={{CountOfToStepExecutions}} > 0
    """
    return _erb.erb_cmp(count_of_to_step_executions, '>', 0)

def calc_step_transitions_has_been_traversed(count_of_observed_traversals):
    """
    TRUE when this transition has been walked at least once.
    
    Formula: ={{CountOfObservedTraversals}} > 0
    """
    return _erb.erb_cmp(count_of_observed_traversals, '>', 0)

def calc_step_transitions_target_carries_blocking_control(target_blocking_requirement_count):
    """
    Whether the destination step of this transition carries at least one blocking control.
    
    Formula: ={{TargetBlockingRequirementCount}} > 0
    """
    return _erb.erb_cmp(target_blocking_requirement_count, '>', 0)

# Level 2

def calc_step_transitions_is_never_exercised(has_reachable_origin, has_reachable_target):
    """
    TRUE when at least one end of this transition has never appeared in any step execution.
    
    Formula: =NOT(AND({{HasReachableOrigin}}, {{HasReachableTarget}}))
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_reachable_origin), _erb.erb_bool3(has_reachable_target))))

def calc_step_transitions_is_unwalked_recovery_path(is_recovery_path, has_been_traversed):
    """
    TRUE for a Fallback/Alternative transition with zero recorded traversals.
    
    Formula: =AND({{IsRecoveryPath}}, NOT({{HasBeenTraversed}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_recovery_path), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_been_traversed))))

# Level 3

def calc_step_transitions_is_untested_recovery_path(is_recovery_path, is_never_exercised):
    """
    TRUE for a Fallback or Alternative transition whose endpoints show no execution evidence.
    
    Formula: =AND({{IsRecoveryPath}}, {{IsNeverExercised}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_recovery_path), _erb.erb_bool3(is_never_exercised))

def calc_step_transitions_is_unrehearsed_control_entry(is_unwalked_recovery_path, target_carries_blocking_control):
    """
    A recovery path that has never been traversed and that leads into a step carrying a blocking control.
    
    Formula: =AND({{IsUnwalkedRecoveryPath}}, {{TargetCarriesBlockingControl}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_unwalked_recovery_path), _erb.erb_bool3(target_carries_blocking_control))

# Level 4

def calc_step_transitions_unrehearsed_control_version_key(is_unrehearsed_control_entry, procedure_version):
    """
    Composite-key echo: this transition's procedure version when it is an unrehearsed control entry, blank otherwise.
    
    Formula: =IF({{IsUnrehearsedControlEntry}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_unrehearsed_control_entry) else '')


def compute_step_transitions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepTransitions.
    
    Directed control-flow edges represented as first-class pko:Transition instances with fromStep/toStep and next/alternative/fallback semantics.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_transitions_name(result.get('from_step'), result.get('to_step'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_recovery_path'] = calc_step_transitions_is_recovery_path(result.get('transition_kind'))
    except Exception as _field_exc:
        result['is_recovery_path'] = None
        result.setdefault('_erb_errors', {})['is_recovery_path'] = str(_field_exc)
    try:
        result['has_reachable_origin'] = calc_step_transitions_has_reachable_origin(result.get('count_of_from_step_executions'))
    except Exception as _field_exc:
        result['has_reachable_origin'] = None
        result.setdefault('_erb_errors', {})['has_reachable_origin'] = str(_field_exc)
    try:
        result['has_reachable_target'] = calc_step_transitions_has_reachable_target(result.get('count_of_to_step_executions'))
    except Exception as _field_exc:
        result['has_reachable_target'] = None
        result.setdefault('_erb_errors', {})['has_reachable_target'] = str(_field_exc)
    try:
        result['has_been_traversed'] = calc_step_transitions_has_been_traversed(result.get('count_of_observed_traversals'))
    except Exception as _field_exc:
        result['has_been_traversed'] = None
        result.setdefault('_erb_errors', {})['has_been_traversed'] = str(_field_exc)
    try:
        result['target_carries_blocking_control'] = calc_step_transitions_target_carries_blocking_control(result.get('target_blocking_requirement_count'))
    except Exception as _field_exc:
        result['target_carries_blocking_control'] = None
        result.setdefault('_erb_errors', {})['target_carries_blocking_control'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_never_exercised'] = calc_step_transitions_is_never_exercised(result.get('has_reachable_origin'), result.get('has_reachable_target'))
    except Exception as _field_exc:
        result['is_never_exercised'] = None
        result.setdefault('_erb_errors', {})['is_never_exercised'] = str(_field_exc)
    try:
        result['is_unwalked_recovery_path'] = calc_step_transitions_is_unwalked_recovery_path(result.get('is_recovery_path'), result.get('has_been_traversed'))
    except Exception as _field_exc:
        result['is_unwalked_recovery_path'] = None
        result.setdefault('_erb_errors', {})['is_unwalked_recovery_path'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_untested_recovery_path'] = calc_step_transitions_is_untested_recovery_path(result.get('is_recovery_path'), result.get('is_never_exercised'))
    except Exception as _field_exc:
        result['is_untested_recovery_path'] = None
        result.setdefault('_erb_errors', {})['is_untested_recovery_path'] = str(_field_exc)
    try:
        result['is_unrehearsed_control_entry'] = calc_step_transitions_is_unrehearsed_control_entry(result.get('is_unwalked_recovery_path'), result.get('target_carries_blocking_control'))
    except Exception as _field_exc:
        result['is_unrehearsed_control_entry'] = None
        result.setdefault('_erb_errors', {})['is_unrehearsed_control_entry'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['unrehearsed_control_version_key'] = calc_step_transitions_unrehearsed_control_version_key(result.get('is_unrehearsed_control_entry'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['unrehearsed_control_version_key'] = None
        result.setdefault('_erb_errors', {})['unrehearsed_control_version_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'unrehearsed_control_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ACTIONS
# Human actions required by steps. Maps to pko:Action and pko:requiresAction.
# =============================================================================

# Level 1

def calc_actions_name(label):
    """
    Human-readable calculated display alias for the Actions row.
    
    Formula: ={{Label}}
    """
    return label


def compute_actions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Actions.
    
    Human actions required by steps. Maps to pko:Action and pko:requiresAction.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_actions_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# FUNCTIONS
# Software or algorithmic functions required by steps. Maps to pko:Function and pko:requiresFunction.
# =============================================================================

# Level 1

def calc_functions_name(label):
    """
    Human-readable calculated display alias for the Functions row.
    
    Formula: ={{Label}}
    """
    return label


def compute_functions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Functions.
    
    Software or algorithmic functions required by steps. Maps to pko:Function and pko:requiresFunction.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_functions_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# TOOLS
# Tools required to execute steps. Maps to m4ing:Tool and pko:requiresTool.
# =============================================================================

# Level 1

def calc_tools_name(label):
    """
    Human-readable calculated display alias for the Tools row.
    
    Formula: ={{Label}}
    """
    return label


def compute_tools_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Tools.
    
    Tools required to execute steps. Maps to m4ing:Tool and pko:requiresTool.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_tools_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPACTIONS
# Many-to-many Step/Action semantics normalized into a first-class ERB junction table.
# =============================================================================

# Level 1

def calc_step_actions_name(step, action):
    """
    Human-readable calculated display alias for the StepActions row.
    
    Formula: ={{Step}} & " / " & {{Action}}
    """
    return (str(step or "") + ' / ' + str(action or ""))


def compute_step_actions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepActions.
    
    Many-to-many Step/Action semantics normalized into a first-class ERB junction table.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_actions_name(result.get('step'), result.get('action'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPFUNCTIONS
# Many-to-many Step/Function semantics normalized into an ERB junction table.
# =============================================================================

# Level 1

def calc_step_functions_name(step, function):
    """
    Human-readable calculated display alias for the StepFunctions row.
    
    Formula: ={{Step}} & " / " & {{Function}}
    """
    return (str(step or "") + ' / ' + str(function or ""))


def compute_step_functions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepFunctions.
    
    Many-to-many Step/Function semantics normalized into an ERB junction table.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_functions_name(result.get('step'), result.get('function'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPTOOLS
# Many-to-many Step/Tool semantics normalized into an ERB junction table.
# =============================================================================

# Level 1

def calc_step_tools_name(step, tool):
    """
    Human-readable calculated display alias for the StepTools row.
    
    Formula: ={{Step}} & " / " & {{Tool}}
    """
    return (str(step or "") + ' / ' + str(tool or ""))


def compute_step_tools_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepTools.
    
    Many-to-many Step/Tool semantics normalized into an ERB junction table.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_tools_name(result.get('step'), result.get('tool'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# REQUIREMENTS
# Normative requirements applied to procedures, steps, or transitions. Maps to pko:Requirement and pko:hasRequirement.
# =============================================================================

# Level 1

def calc_requirements_name(label):
    """
    Human-readable calculated display alias for the Requirements row.
    
    Formula: ={{Label}}
    """
    return label

def calc_requirements_is_bound_to_any_step(step_binding_count):
    """
    TRUE when at least one step carries this requirement.
    
    Formula: ={{StepBindingCount}} > 0
    """
    return _erb.erb_cmp(step_binding_count, '>', 0)

def calc_requirements_has_ever_been_evaluated(satisfaction_record_count):
    """
    TRUE when this requirement has at least one satisfaction record in the model.
    
    Formula: ={{SatisfactionRecordCount}} > 0
    """
    return _erb.erb_cmp(satisfaction_record_count, '>', 0)

def calc_requirements_has_ever_produced_negative(negative_outcome_count):
    """
    TRUE when this requirement has at least once been scored as anything other than Satisfied.
    
    Formula: ={{NegativeOutcomeCount}} > 0
    """
    return _erb.erb_cmp(negative_outcome_count, '>', 0)

def calc_requirements_claims_a_witness_field(witness_field_name):
    """
    TRUE when this requirement names a field it claims computes it.
    
    Formula: ={{WitnessFieldName}} <> "" 
    """
    return (not (witness_field_name is None or witness_field_name == ""))

def calc_requirements_witness_fire_count(negative_outcome_count):
    """
    How many times this requirement's evaluation has returned a non-Satisfied result.
    
    Formula: ={{NegativeOutcomeCount}}
    """
    return negative_outcome_count

def calc_requirements_evaluation_sample_size(satisfaction_record_count):
    """
    How many times this requirement has been evaluated at all.
    
    Formula: ={{SatisfactionRecordCount}}
    """
    return satisfaction_record_count

def calc_requirements_witness_is_partially_scoped(has_computed_witness, unexercised_binding_count):
    """
    TRUE when a control has a computed witness but at least one of its step bindings has never been exercised by it.
    
    Formula: =AND({{HasComputedWitness}}, {{UnexercisedBindingCount}} > 0)
    """
    return _erb.erb_and((has_computed_witness is True), _erb.erb_bool3(_erb.erb_cmp(unexercised_binding_count, '>', 0)))

def calc_requirements_has_named_owner(accountable_role):
    """
    TRUE when a role has been named as accountable for this control.
    
    Formula: ={{AccountableRole}} <> "" 
    """
    return (not (accountable_role is None or accountable_role == ""))

def calc_requirements_uses_controlled_vocabulary(controlled_term):
    """
    TRUE when this requirement points at a controlled term instead of only free-texting the concept.
    
    Formula: ={{ControlledTerm}} <> "" 
    """
    return (not (controlled_term is None or controlled_term == ""))

# Level 2

def calc_requirements_is_inoperative_control(is_blocking, is_bound_to_any_step, has_ever_been_evaluated):
    """
    TRUE for a blocking requirement that is attached to a step in the specification but has never once been evaluated on any execution.
    
    Formula: =AND({{IsBlocking}}, {{IsBoundToAnyStep}}, NOT({{HasEverBeenEvaluated}}))
    """
    return _erb.erb_and((is_blocking is True), _erb.erb_bool3(is_bound_to_any_step), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_ever_been_evaluated))))

def calc_requirements_is_decorative_control(is_blocking, is_bound_to_any_step):
    """
    TRUE for a blocking requirement that is not attached to any step at all.
    
    Formula: =AND({{IsBlocking}}, NOT({{IsBoundToAnyStep}}))
    """
    return _erb.erb_and((is_blocking is True), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_bound_to_any_step))))

def calc_requirements_is_unfalsified_control(is_blocking, has_ever_been_evaluated, has_ever_produced_negative):
    """
    TRUE for a blocking control that HAS been evaluated at least once and has never returned a negative result.
    
    Formula: =AND({{IsBlocking}}, {{HasEverBeenEvaluated}}, NOT({{HasEverProducedNegative}}))
    """
    return _erb.erb_and((is_blocking is True), _erb.erb_bool3(has_ever_been_evaluated), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_ever_produced_negative))))

def calc_requirements_derived_has_computed_witness(claims_a_witness_field, named_witness_field_exists):
    """
    Whether this requirement genuinely has a computed witness, derived from the field catalog rather than asserted.
    
    Formula: =AND({{ClaimsAWitnessField}}, {{NamedWitnessFieldExists}})
    """
    return _erb.erb_and(_erb.erb_bool3(claims_a_witness_field), _erb.erb_bool3(named_witness_field_exists))

def calc_requirements_witness_has_never_fired(has_computed_witness, witness_fire_count):
    """
    TRUE for a requirement that has a computed witness which has never once returned a negative result.
    
    Formula: =AND({{HasComputedWitness}}, {{WitnessFireCount}} = 0)
    """
    return _erb.erb_and((has_computed_witness is True), _erb.erb_bool3(_erb.erb_eq(witness_fire_count, 0)))

def calc_requirements_has_meaningful_sample(evaluation_sample_size, minimum_sample_for_assurance):
    """
    TRUE when this requirement has been evaluated often enough that a clean record is informative.
    
    Formula: ={{EvaluationSampleSize}} >= {{MinimumSampleForAssurance}}
    """
    return _erb.erb_cmp(evaluation_sample_size, '>=', _erb.erb_nullif(minimum_sample_for_assurance))

def calc_requirements_is_orphaned_blocking_control(is_blocking, has_named_owner):
    """
    TRUE for a blocking control with nobody named as accountable for it.
    
    Formula: =AND({{IsBlocking}}, NOT({{HasNamedOwner}}))
    """
    return _erb.erb_and((is_blocking is True), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_named_owner))))

def calc_requirements_is_unwatched_and_unowned(is_blocking, has_computed_witness, has_named_owner):
    """
    TRUE for a blocking control that nothing computes and nobody owns.
    
    Formula: =AND({{IsBlocking}}, NOT({{HasComputedWitness}}), NOT({{HasNamedOwner}}))
    """
    return _erb.erb_and((is_blocking is True), _erb.erb_bool3(_erb.erb_not((has_computed_witness is True))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_named_owner))))

# Level 3

def calc_requirements_witness_claim_is_unverified(has_computed_witness, derived_has_computed_witness):
    """
    TRUE when the hand-typed HasComputedWitness flag disagrees with what the field catalog says.
    
    Formula: =NOT({{HasComputedWitness}} = {{DerivedHasComputedWitness}})
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(has_computed_witness), derived_has_computed_witness)))

def calc_requirements_is_unwitnessed_blocking_control(is_blocking, derived_has_computed_witness):
    """
    TRUE for a blocking control with no verified computed witness behind it.
    
    Formula: =AND({{IsBlocking}}, NOT({{DerivedHasComputedWitness}}))
    """
    return _erb.erb_and((is_blocking is True), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(derived_has_computed_witness))))

def calc_requirements_is_untested_witness(witness_has_never_fired, has_meaningful_sample):
    """
    TRUE for a witness that has never fired and has not been exercised enough for that silence to mean anything.
    
    Formula: =AND({{WitnessHasNeverFired}}, NOT({{HasMeaningfulSample}}))
    """
    return _erb.erb_and(_erb.erb_bool3(witness_has_never_fired), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_meaningful_sample))))

def calc_requirements_is_evidenced_holding_control(witness_has_never_fired, has_meaningful_sample):
    """
    TRUE for a witness that has never fired across a sample large enough for that to constitute evidence.
    
    Formula: =AND({{WitnessHasNeverFired}}, {{HasMeaningfulSample}})
    """
    return _erb.erb_and(_erb.erb_bool3(witness_has_never_fired), _erb.erb_bool3(has_meaningful_sample))

def calc_requirements_control_assurance_state(is_bound_to_any_step, has_ever_been_evaluated, has_computed_witness, witness_fire_count, has_meaningful_sample):
    """
    One of Decorative, Inoperative, Asserted, Demonstrated, Holding, or Untested — the single control-health verdict for this requirement.
    
    Formula: =IF(NOT({{IsBoundToAnyStep}}), "Decorative", IF(NOT({{HasEverBeenEvaluated}}), "Inoperative", IF(NOT({{HasComputedWitness}}), "Asserted", IF({{WitnessFireCount}} > 0, "Demonstrated", IF({{HasMeaningfulSample}}, "Holding", "Untested")))))
    """
    return ('Decorative' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_bound_to_any_step))) else ('Inoperative' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_ever_been_evaluated))) else ('Asserted' if _erb.erb_bool3(_erb.erb_not((has_computed_witness is True))) else ('Demonstrated' if _erb.erb_bool3(_erb.erb_cmp(witness_fire_count, '>', 0)) else ('Holding' if _erb.erb_bool3(has_meaningful_sample) else 'Untested')))))

def calc_requirements_attestation_exposure_note(is_blocking, is_unwatched_and_unowned, is_orphaned_blocking_control, has_computed_witness):
    """
    States, per control, what kind of exposure attesting to it creates.
    
    Formula: =IF(NOT({{IsBlocking}}), "", IF({{IsUnwatchedAndUnowned}}, "Unwatched and unowned: exposure defaults to the signatory.", IF({{IsOrphanedBlockingControl}}, "Witnessed but unowned: no named accountability.", IF(NOT({{HasComputedWitness}}), "Owned but unwitnessed: rests on human judgement.", ""))))
    """
    return ('' if _erb.erb_bool3(_erb.erb_not((is_blocking is True))) else ('Unwatched and unowned: exposure defaults to the signatory.' if _erb.erb_bool3(is_unwatched_and_unowned) else ('Witnessed but unowned: no named accountability.' if _erb.erb_bool3(is_orphaned_blocking_control) else ('Owned but unwitnessed: rests on human judgement.' if _erb.erb_bool3(_erb.erb_not((has_computed_witness is True))) else ''))))

def calc_requirements_unwatched_unowned_flag(is_unwatched_and_unowned):
    """
    Constant marker echoed when this control is both unwatched and unowned.
    
    Formula: =IF({{IsUnwatchedAndUnowned}}, "unwatched-unowned", "")
    """
    return ('unwatched-unowned' if _erb.erb_bool3(is_unwatched_and_unowned) else '')


def compute_requirements_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Requirements.
    
    Normative requirements applied to procedures, steps, or transitions. Maps to pko:Requirement and pko:hasRequirement.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_requirements_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_bound_to_any_step'] = calc_requirements_is_bound_to_any_step(result.get('step_binding_count'))
    except Exception as _field_exc:
        result['is_bound_to_any_step'] = None
        result.setdefault('_erb_errors', {})['is_bound_to_any_step'] = str(_field_exc)
    try:
        result['has_ever_been_evaluated'] = calc_requirements_has_ever_been_evaluated(result.get('satisfaction_record_count'))
    except Exception as _field_exc:
        result['has_ever_been_evaluated'] = None
        result.setdefault('_erb_errors', {})['has_ever_been_evaluated'] = str(_field_exc)
    try:
        result['has_ever_produced_negative'] = calc_requirements_has_ever_produced_negative(result.get('negative_outcome_count'))
    except Exception as _field_exc:
        result['has_ever_produced_negative'] = None
        result.setdefault('_erb_errors', {})['has_ever_produced_negative'] = str(_field_exc)
    try:
        result['claims_a_witness_field'] = calc_requirements_claims_a_witness_field(result.get('witness_field_name'))
    except Exception as _field_exc:
        result['claims_a_witness_field'] = None
        result.setdefault('_erb_errors', {})['claims_a_witness_field'] = str(_field_exc)
    try:
        result['witness_fire_count'] = calc_requirements_witness_fire_count(result.get('negative_outcome_count'))
    except Exception as _field_exc:
        result['witness_fire_count'] = None
        result.setdefault('_erb_errors', {})['witness_fire_count'] = str(_field_exc)
    try:
        result['evaluation_sample_size'] = calc_requirements_evaluation_sample_size(result.get('satisfaction_record_count'))
    except Exception as _field_exc:
        result['evaluation_sample_size'] = None
        result.setdefault('_erb_errors', {})['evaluation_sample_size'] = str(_field_exc)
    try:
        result['witness_is_partially_scoped'] = calc_requirements_witness_is_partially_scoped(result.get('has_computed_witness'), result.get('unexercised_binding_count'))
    except Exception as _field_exc:
        result['witness_is_partially_scoped'] = None
        result.setdefault('_erb_errors', {})['witness_is_partially_scoped'] = str(_field_exc)
    try:
        result['has_named_owner'] = calc_requirements_has_named_owner(result.get('accountable_role'))
    except Exception as _field_exc:
        result['has_named_owner'] = None
        result.setdefault('_erb_errors', {})['has_named_owner'] = str(_field_exc)
    try:
        result['uses_controlled_vocabulary'] = calc_requirements_uses_controlled_vocabulary(result.get('controlled_term'))
    except Exception as _field_exc:
        result['uses_controlled_vocabulary'] = None
        result.setdefault('_erb_errors', {})['uses_controlled_vocabulary'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_inoperative_control'] = calc_requirements_is_inoperative_control(result.get('is_blocking'), result.get('is_bound_to_any_step'), result.get('has_ever_been_evaluated'))
    except Exception as _field_exc:
        result['is_inoperative_control'] = None
        result.setdefault('_erb_errors', {})['is_inoperative_control'] = str(_field_exc)
    try:
        result['is_decorative_control'] = calc_requirements_is_decorative_control(result.get('is_blocking'), result.get('is_bound_to_any_step'))
    except Exception as _field_exc:
        result['is_decorative_control'] = None
        result.setdefault('_erb_errors', {})['is_decorative_control'] = str(_field_exc)
    try:
        result['is_unfalsified_control'] = calc_requirements_is_unfalsified_control(result.get('is_blocking'), result.get('has_ever_been_evaluated'), result.get('has_ever_produced_negative'))
    except Exception as _field_exc:
        result['is_unfalsified_control'] = None
        result.setdefault('_erb_errors', {})['is_unfalsified_control'] = str(_field_exc)
    try:
        result['derived_has_computed_witness'] = calc_requirements_derived_has_computed_witness(result.get('claims_a_witness_field'), result.get('named_witness_field_exists'))
    except Exception as _field_exc:
        result['derived_has_computed_witness'] = None
        result.setdefault('_erb_errors', {})['derived_has_computed_witness'] = str(_field_exc)
    try:
        result['witness_has_never_fired'] = calc_requirements_witness_has_never_fired(result.get('has_computed_witness'), result.get('witness_fire_count'))
    except Exception as _field_exc:
        result['witness_has_never_fired'] = None
        result.setdefault('_erb_errors', {})['witness_has_never_fired'] = str(_field_exc)
    try:
        result['has_meaningful_sample'] = calc_requirements_has_meaningful_sample(result.get('evaluation_sample_size'), result.get('minimum_sample_for_assurance'))
    except Exception as _field_exc:
        result['has_meaningful_sample'] = None
        result.setdefault('_erb_errors', {})['has_meaningful_sample'] = str(_field_exc)
    try:
        result['is_orphaned_blocking_control'] = calc_requirements_is_orphaned_blocking_control(result.get('is_blocking'), result.get('has_named_owner'))
    except Exception as _field_exc:
        result['is_orphaned_blocking_control'] = None
        result.setdefault('_erb_errors', {})['is_orphaned_blocking_control'] = str(_field_exc)
    try:
        result['is_unwatched_and_unowned'] = calc_requirements_is_unwatched_and_unowned(result.get('is_blocking'), result.get('has_computed_witness'), result.get('has_named_owner'))
    except Exception as _field_exc:
        result['is_unwatched_and_unowned'] = None
        result.setdefault('_erb_errors', {})['is_unwatched_and_unowned'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['witness_claim_is_unverified'] = calc_requirements_witness_claim_is_unverified(result.get('has_computed_witness'), result.get('derived_has_computed_witness'))
    except Exception as _field_exc:
        result['witness_claim_is_unverified'] = None
        result.setdefault('_erb_errors', {})['witness_claim_is_unverified'] = str(_field_exc)
    try:
        result['is_unwitnessed_blocking_control'] = calc_requirements_is_unwitnessed_blocking_control(result.get('is_blocking'), result.get('derived_has_computed_witness'))
    except Exception as _field_exc:
        result['is_unwitnessed_blocking_control'] = None
        result.setdefault('_erb_errors', {})['is_unwitnessed_blocking_control'] = str(_field_exc)
    try:
        result['is_untested_witness'] = calc_requirements_is_untested_witness(result.get('witness_has_never_fired'), result.get('has_meaningful_sample'))
    except Exception as _field_exc:
        result['is_untested_witness'] = None
        result.setdefault('_erb_errors', {})['is_untested_witness'] = str(_field_exc)
    try:
        result['is_evidenced_holding_control'] = calc_requirements_is_evidenced_holding_control(result.get('witness_has_never_fired'), result.get('has_meaningful_sample'))
    except Exception as _field_exc:
        result['is_evidenced_holding_control'] = None
        result.setdefault('_erb_errors', {})['is_evidenced_holding_control'] = str(_field_exc)
    try:
        result['control_assurance_state'] = calc_requirements_control_assurance_state(result.get('is_bound_to_any_step'), result.get('has_ever_been_evaluated'), result.get('has_computed_witness'), result.get('witness_fire_count'), result.get('has_meaningful_sample'))
    except Exception as _field_exc:
        result['control_assurance_state'] = None
        result.setdefault('_erb_errors', {})['control_assurance_state'] = str(_field_exc)
    try:
        result['attestation_exposure_note'] = calc_requirements_attestation_exposure_note(result.get('is_blocking'), result.get('is_unwatched_and_unowned'), result.get('is_orphaned_blocking_control'), result.get('has_computed_witness'))
    except Exception as _field_exc:
        result['attestation_exposure_note'] = None
        result.setdefault('_erb_errors', {})['attestation_exposure_note'] = str(_field_exc)
    try:
        result['unwatched_unowned_flag'] = calc_requirements_unwatched_unowned_flag(result.get('is_unwatched_and_unowned'))
    except Exception as _field_exc:
        result['unwatched_unowned_flag'] = None
        result.setdefault('_erb_errors', {})['unwatched_unowned_flag'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'control_assurance_state', 'attestation_exposure_note', 'unwatched_unowned_flag']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPREQUIREMENTS
# Many-to-many Step/Requirement semantics normalized into an ERB junction table.
# =============================================================================

# Level 1

def calc_step_requirements_name(step, requirement):
    """
    Human-readable calculated display alias for the StepRequirements row.
    
    Formula: ={{Step}} & " / " & {{Requirement}}
    """
    return (str(step or "") + ' / ' + str(requirement or ""))

def calc_step_requirements_blocking_step_key(requirement_is_blocking, step):
    """
    Echoes the Step id only when the bound requirement is blocking; empty otherwise.
    
    Formula: =IF({{RequirementIsBlocking}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(requirement_is_blocking) else '')

def calc_step_requirements_step_when_blocking(requirement_is_blocking, step):
    """
    Echoes the step id when the bound requirement is blocking, blank otherwise.
    
    Formula: =IF({{RequirementIsBlocking}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(requirement_is_blocking) else '')

def calc_step_requirements_unwitnessed_step_key(requirement_lacks_witness, step):
    """
    Echoes the step id when the requirement bound here has no computed witness.
    
    Formula: =IF({{RequirementLacksWitness}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(requirement_lacks_witness) else '')

def calc_step_requirements_binding_was_ever_exercised(satisfaction_count_for_binding):
    """
    TRUE when this specific (step, requirement) pair has been evaluated at least once.
    
    Formula: ={{SatisfactionCountForBinding}} > 0
    """
    return _erb.erb_cmp(satisfaction_count_for_binding, '>', 0)

# Level 2

def calc_step_requirements_is_unexercised_blocking_binding(requirement_is_blocking, binding_was_ever_exercised):
    """
    TRUE for a blocking control bound to a step where it has never actually been evaluated.
    
    Formula: =AND({{RequirementIsBlocking}}, NOT({{BindingWasEverExercised}}))
    """
    return _erb.erb_and(_erb.erb_bool3(requirement_is_blocking), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(binding_was_ever_exercised))))

# Level 3

def calc_step_requirements_unexercised_binding_requirement_key(is_unexercised_blocking_binding, requirement):
    """
    Echoes the requirement id when this binding has never been exercised.
    
    Formula: =IF({{IsUnexercisedBlockingBinding}}, {{Requirement}}, "")
    """
    return (requirement if _erb.erb_bool3(is_unexercised_blocking_binding) else '')


def compute_step_requirements_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepRequirements.
    
    Many-to-many Step/Requirement semantics normalized into an ERB junction table.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_requirements_name(result.get('step'), result.get('requirement'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['blocking_step_key'] = calc_step_requirements_blocking_step_key(result.get('requirement_is_blocking'), result.get('step'))
    except Exception as _field_exc:
        result['blocking_step_key'] = None
        result.setdefault('_erb_errors', {})['blocking_step_key'] = str(_field_exc)
    try:
        result['step_when_blocking'] = calc_step_requirements_step_when_blocking(result.get('requirement_is_blocking'), result.get('step'))
    except Exception as _field_exc:
        result['step_when_blocking'] = None
        result.setdefault('_erb_errors', {})['step_when_blocking'] = str(_field_exc)
    try:
        result['unwitnessed_step_key'] = calc_step_requirements_unwitnessed_step_key(result.get('requirement_lacks_witness'), result.get('step'))
    except Exception as _field_exc:
        result['unwitnessed_step_key'] = None
        result.setdefault('_erb_errors', {})['unwitnessed_step_key'] = str(_field_exc)
    try:
        result['binding_was_ever_exercised'] = calc_step_requirements_binding_was_ever_exercised(result.get('satisfaction_count_for_binding'))
    except Exception as _field_exc:
        result['binding_was_ever_exercised'] = None
        result.setdefault('_erb_errors', {})['binding_was_ever_exercised'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_unexercised_blocking_binding'] = calc_step_requirements_is_unexercised_blocking_binding(result.get('requirement_is_blocking'), result.get('binding_was_ever_exercised'))
    except Exception as _field_exc:
        result['is_unexercised_blocking_binding'] = None
        result.setdefault('_erb_errors', {})['is_unexercised_blocking_binding'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['unexercised_binding_requirement_key'] = calc_step_requirements_unexercised_binding_requirement_key(result.get('is_unexercised_blocking_binding'), result.get('requirement'))
    except Exception as _field_exc:
        result['unexercised_binding_requirement_key'] = None
        result.setdefault('_erb_errors', {})['unexercised_binding_requirement_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'blocking_step_key', 'step_when_blocking', 'unwitnessed_step_key', 'unexercised_binding_requirement_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPVERIFICATIONS
# Verification definitions attached to steps. Maps to pko:StepVerification and pko:SignalVerification.
# =============================================================================

# Level 1

def calc_step_verifications_name(step, verification_kind):
    """
    Human-readable calculated display alias for the StepVerifications row.
    
    Formula: ={{Step}} & " / " & {{VerificationKind}}
    """
    return (str(step or "") + ' / ' + str(verification_kind or ""))


def compute_step_verifications_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepVerifications.
    
    Verification definitions attached to steps. Maps to pko:StepVerification and pko:SignalVerification.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_verifications_name(result.get('step'), result.get('verification_kind'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# RATIONALES
# First-class rationale statements explaining why procedural commitments and design decisions exist. Explicit ERB-PKO extension, represented as prov:Entity/dcat:Resource in projections.
# =============================================================================

# Level 1

def calc_rationales_name(title):
    """
    Human-readable calculated display alias for the Rationales row.
    
    Formula: ={{Title}}
    """
    return title


def compute_rationales_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Rationales.
    
    First-class rationale statements explaining why procedural commitments and design decisions exist. Explicit ERB-PKO extension, represented as prov:Entity/dcat:Resource in projections.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_rationales_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# EXCEPTIONS
# Documented exceptions, fallbacks, and alternative handling. Aligns structurally with PKO fallback/alternative steps and requirements; the exception record itself is an ERB-PKO extension.
# =============================================================================

# Level 1

def calc_exceptions_name(condition):
    """
    Human-readable calculated display alias for the Exceptions row.
    
    Formula: ={{Condition}}
    """
    return condition

def calc_exceptions_active_exception_step_key(status, trigger_step):
    """
    Echoes the TriggerStep id only for exceptions currently in Active status.
    
    Formula: =IF({{Status}} = "Active", {{TriggerStep}}, "")
    """
    return (trigger_step if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Active')) else '')


def compute_exceptions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Exceptions.
    
    Documented exceptions, fallbacks, and alternative handling. Aligns structurally with PKO fallback/alternative steps and requirements; the exception record itself is an ERB-PKO extension.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_exceptions_name(result.get('condition'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['active_exception_step_key'] = calc_exceptions_active_exception_step_key(result.get('status'), result.get('trigger_step'))
    except Exception as _field_exc:
        result['active_exception_step_key'] = None
        result.setdefault('_erb_errors', {})['active_exception_step_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'active_exception_step_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# RESOURCES
# Documents, datasets, APIs, templates, images, manuals, and operational records referenced by procedures. Maps to dcat:Resource.
# =============================================================================

# Level 1

def calc_resources_name(title):
    """
    Human-readable calculated display alias for the Resources row.
    
    Formula: ={{Title}}
    """
    return title

def calc_resources_is_approved_source(approval_status):
    """
    TRUE when this resource is an approved source.
    
    Formula: ={{ApprovalStatus}} = "Approved" 
    """
    return _erb.erb_eq(_erb.erb_nullif(approval_status), 'Approved')


def compute_resources_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Resources.
    
    Documents, datasets, APIs, templates, images, manuals, and operational records referenced by procedures. Maps to dcat:Resource.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_resources_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_approved_source'] = calc_resources_is_approved_source(result.get('approval_status'))
    except Exception as _field_exc:
        result['is_approved_source'] = None
        result.setdefault('_erb_errors', {})['is_approved_source'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDURERESOURCES
# Links versioned procedures to supporting resources using PKO/dcterms provenance relations.
# =============================================================================

# Level 1

def calc_procedure_resources_name(procedure_version, resource):
    """
    Human-readable calculated display alias for the ProcedureResources row.
    
    Formula: ={{ProcedureVersion}} & " / " & {{Resource}}
    """
    return (str(procedure_version or "") + ' / ' + str(resource or ""))

def calc_procedure_resources_relation_iri(relation):
    """
    Exact semantic property IRI for the relation.
    
    Formula: =IF({{Relation}} = "wasExtractedFrom", "https://w3id.org/pko#wasExtractedFrom", "http://purl.org/dc/terms/references")
    """
    return ('https://w3id.org/pko#wasExtractedFrom' if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(relation), 'wasExtractedFrom')) else 'http://purl.org/dc/terms/references')


def compute_procedure_resources_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcedureResources.
    
    Links versioned procedures to supporting resources using PKO/dcterms provenance relations.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedure_resources_name(result.get('procedure_version'), result.get('resource'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['relation_iri'] = calc_procedure_resources_relation_iri(result.get('relation'))
    except Exception as _field_exc:
        result['relation_iri'] = None
        result.setdefault('_erb_errors', {})['relation_iri'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'relation_iri']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ELICITATIONSESSIONS
# Structured knowledge-elicitation events involving practitioners and knowledge engineers. Explicit ERB-PKO extension, modeled as prov:Activity.
# =============================================================================

# Level 1

def calc_elicitation_sessions_name(method, started_at):
    """
    Human-readable calculated display alias for the ElicitationSessions row.
    
    Formula: ={{Method}} & " / " & {{StartedAt}}
    """
    return (str(method or "") + ' / ' + _erb.erb_timestamptz_text(started_at))

def calc_elicitation_sessions_days_since_elicited(as_of_instant, ended_at):
    """
    Days elapsed since this elicitation session concluded.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{EndedAt}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, ended_at, 'days'))

def calc_elicitation_sessions_is_single_witness_method(method):
    """
    TRUE when this session captured one practitioner's account rather than a group's.
    
    Formula: =OR({{Method}} = "Shadowing", {{Method}} = "PractitionerInterview")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(method), 'Shadowing')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(method), 'PractitionerInterview')))

def calc_elicitation_sessions_is_high_yield_session(valid_fragments_produced):
    """
    A session that alone underwrites three or more currently-valid claims.
    
    Formula: ={{ValidFragmentsProduced}} >= 3
    """
    return _erb.erb_cmp(valid_fragments_produced, '>=', 3)

# Level 2

def calc_elicitation_sessions_is_concentrated_single_witness(is_single_witness_method, is_high_yield_session):
    """
    One unrepeated session with one witness that underwrites three or more live claims.
    
    Formula: =AND({{IsSingleWitnessMethod}}, {{IsHighYieldSession}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_single_witness_method), _erb.erb_bool3(is_high_yield_session))

# Level 3

def calc_elicitation_sessions_is_stale_concentrated_witness(is_concentrated_single_witness, days_since_elicited):
    """
    A concentrated single-witness session more than 180 days old — matching the single-witness expiry horizon loop 1 already established.
    
    Formula: =AND({{IsConcentratedSingleWitness}}, {{DaysSinceElicited}} > 180)
    """
    return _erb.erb_and(_erb.erb_bool3(is_concentrated_single_witness), _erb.erb_bool3(_erb.erb_cmp(days_since_elicited, '>', 180)))

def calc_elicitation_sessions_concentrated_session_version_key(is_concentrated_single_witness, procedure_version):
    """
    Composite-key echo: this session's procedure version when the session is a concentrated single witness, blank otherwise.
    
    Formula: =IF({{IsConcentratedSingleWitness}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_concentrated_single_witness) else '')


def compute_elicitation_sessions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ElicitationSessions.
    
    Structured knowledge-elicitation events involving practitioners and knowledge engineers. Explicit ERB-PKO extension, modeled as prov:Activity.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_elicitation_sessions_name(result.get('method'), result.get('started_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['days_since_elicited'] = calc_elicitation_sessions_days_since_elicited(result.get('as_of_instant'), result.get('ended_at'))
    except Exception as _field_exc:
        result['days_since_elicited'] = None
        result.setdefault('_erb_errors', {})['days_since_elicited'] = str(_field_exc)
    try:
        result['is_single_witness_method'] = calc_elicitation_sessions_is_single_witness_method(result.get('method'))
    except Exception as _field_exc:
        result['is_single_witness_method'] = None
        result.setdefault('_erb_errors', {})['is_single_witness_method'] = str(_field_exc)
    try:
        result['is_high_yield_session'] = calc_elicitation_sessions_is_high_yield_session(result.get('valid_fragments_produced'))
    except Exception as _field_exc:
        result['is_high_yield_session'] = None
        result.setdefault('_erb_errors', {})['is_high_yield_session'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_concentrated_single_witness'] = calc_elicitation_sessions_is_concentrated_single_witness(result.get('is_single_witness_method'), result.get('is_high_yield_session'))
    except Exception as _field_exc:
        result['is_concentrated_single_witness'] = None
        result.setdefault('_erb_errors', {})['is_concentrated_single_witness'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_stale_concentrated_witness'] = calc_elicitation_sessions_is_stale_concentrated_witness(result.get('is_concentrated_single_witness'), result.get('days_since_elicited'))
    except Exception as _field_exc:
        result['is_stale_concentrated_witness'] = None
        result.setdefault('_erb_errors', {})['is_stale_concentrated_witness'] = str(_field_exc)
    try:
        result['concentrated_session_version_key'] = calc_elicitation_sessions_concentrated_session_version_key(result.get('is_concentrated_single_witness'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['concentrated_session_version_key'] = None
        result.setdefault('_erb_errors', {})['concentrated_session_version_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'concentrated_session_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# KNOWLEDGEFRAGMENTS
# Explicit records of tacit, implicit, explicit, and situated procedural knowledge. This is an ERB-PKO extension represented as provenance-bearing dcat:Resource instances.
# =============================================================================

# Level 1

def calc_knowledge_fragments_name(knowledge_form, statement):
    """
    Human-readable calculated display alias for the KnowledgeFragments row.
    
    Formula: ={{KnowledgeForm}} & ": " & LEFT({{Statement}}, 60)
    """
    return (str(knowledge_form or "") + ': ' + str(((statement or "")[:(60 or 0)]) if ((statement or "")[:(60 or 0)]) is not None else ""))

def calc_knowledge_fragments_is_currently_valid(valid_from, as_of_instant, valid_to, status):
    """
    TRUE when the fragment is approved and valid now.
    
    Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}), {{Status}} = "Approved")
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', as_of_instant)), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')))

def calc_knowledge_fragments_has_human_source(source_agent_kind):
    """
    TRUE when the claim originates from a human practitioner rather than software.
    
    Formula: ={{SourceAgentKind}} = "Human" 
    """
    return _erb.erb_eq(source_agent_kind, 'Human')

def calc_knowledge_fragments_is_approved(status):
    """
    TRUE when this claim has passed knowledge-authority approval.
    
    Formula: ={{Status}} = "Approved" 
    """
    return _erb.erb_eq(_erb.erb_nullif(status), 'Approved')

def calc_knowledge_fragments_is_within_validity_window(valid_from, as_of_instant, valid_to):
    """
    TRUE when this claim's stated validity window contains the present moment.
    
    Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', as_of_instant)), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))))

def calc_knowledge_fragments_has_recorded_elicitation(elicitation_session):
    """
    TRUE when this claim traces to a recorded elicitation session.
    
    Formula: ={{ElicitationSession}} <> "" 
    """
    return (not (elicitation_session is None or elicitation_session == ""))

def calc_knowledge_fragments_evidence_expiry_days(is_from_single_witness):
    """
    How many days this claim's evidence is trusted for, given how it was gathered.
    
    Formula: =IF({{IsFromSingleWitness}}, 180, 365)
    """
    return _erb.erb_integer((180 if _erb.erb_bool3(is_from_single_witness) else 365))

def calc_knowledge_fragments_is_awaiting_approval(status):
    """
    TRUE when this claim has been reviewed but not yet approved.
    
    Formula: ={{Status}} = "Reviewed" 
    """
    return _erb.erb_eq(_erb.erb_nullif(status), 'Reviewed')

def calc_knowledge_fragments_owner_is_me(owner_role):
    """
    TRUE when the People Policy Owner role owns this claim.
    
    Formula: ={{OwnerRole}} = "hr-policy-owner" 
    """
    return _erb.erb_eq(_erb.erb_nullif(owner_role), 'hr-policy-owner')

def calc_knowledge_fragments_has_operational_reliance(is_invoked_by_an_exception):
    """
    TRUE when an exception handler exists on the step this claim governs.
    
    Formula: ={{IsInvokedByAnException}} > 0
    """
    return _erb.erb_cmp(is_invoked_by_an_exception, '>', 0)

def calc_knowledge_fragments_age_days(as_of_instant, valid_from):
    """
    Days since this claim became valid.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, valid_from, 'days'))

def calc_knowledge_fragments_is_low_confidence(confidence):
    """
    TRUE when this claim was recorded with less than high confidence.
    
    Formula: =OR({{Confidence}} = "Medium", {{Confidence}} = "Low")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(confidence), 'Medium')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(confidence), 'Low')))

def calc_knowledge_fragments_is_human_owned(owner_role_agent_kind):
    """
    TRUE when the owning role is currently held by a human.
    
    Formula: ={{OwnerRoleAgentKind}} = "Human" 
    """
    return _erb.erb_eq(owner_role_agent_kind, 'Human')

def calc_knowledge_fragments_predates_current_role_holder(owner_role_agent_kind, valid_from, owner_role_assignment_valid_from):
    """
    TRUE when this knowledge became valid before the current holder of its owning role took the role.
    
    Formula: =AND({{OwnerRoleAgentKind}} <> "", {{ValidFrom}} < {{OwnerRoleAssignmentValidFrom}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (owner_role_agent_kind is None or owner_role_agent_kind == ""))), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<', owner_role_assignment_valid_from)))

def calc_knowledge_fragments_has_review_record(last_reviewed_at):
    """
    Whether this fragment has ever had an actual review recorded, as opposed to merely having a ValidFrom date.
    
    Formula: ={{LastReviewedAt}} <> "" 
    """
    return (not (last_reviewed_at is None or last_reviewed_at == ""))

def calc_knowledge_fragments_reliance_surface_count(is_invoked_by_an_exception, ratified_boundary_count):
    """
    Total number of distinct downstream dependents on this claim: exception handlers plus ratified authority boundaries.
    
    Formula: ={{IsInvokedByAnException}} + {{RatifiedBoundaryCount}}
    """
    return _erb.erb_integer(_erb.erb_add(is_invoked_by_an_exception, ratified_boundary_count))

# Level 2

def calc_knowledge_fragments_has_orphaned_provenance(is_currently_valid, source_agent_is_still_engaged):
    """
    TRUE when we still rely on this claim but the agent who gave it to us no longer holds a role here.
    
    Formula: =AND({{IsCurrentlyValid}}, NOT({{SourceAgentIsStillEngaged}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_valid), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(source_agent_is_still_engaged))))

def calc_knowledge_fragments_is_relied_upon(step, is_within_validity_window):
    """
    TRUE when this claim is attached to a specific step and is inside its validity window — i.e. it is operationally in play.
    
    Formula: =AND({{Step}} <> "", {{IsWithinValidityWindow}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (step is None or step == ""))), _erb.erb_bool3(is_within_validity_window))

def calc_knowledge_fragments_evidence_has_expired(has_recorded_elicitation, evidence_age_days, evidence_expiry_days):
    """
    TRUE when the evidence behind this claim is older than we trust evidence of its kind to be.
    
    Formula: =AND({{HasRecordedElicitation}}, {{EvidenceAgeDays}} > {{EvidenceExpiryDays}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_recorded_elicitation), _erb.erb_bool3(_erb.erb_cmp(evidence_age_days, '>', evidence_expiry_days)))

def calc_knowledge_fragments_is_my_unfinished_approval(owner_is_me, is_awaiting_approval):
    """
    TRUE when a claim I own has been reviewed and is waiting on my approval.
    
    Formula: =AND({{OwnerIsMe}}, {{IsAwaitingApproval}})
    """
    return _erb.erb_and(_erb.erb_bool3(owner_is_me), _erb.erb_bool3(is_awaiting_approval))

def calc_knowledge_fragments_exceeds_owning_cadence(age_days, owning_version_cadence_days):
    """
    TRUE when this claim is older than the review cadence promised for the version it supports.
    
    Formula: ={{AgeDays}} > {{OwningVersionCadenceDays}}
    """
    return _erb.erb_cmp(age_days, '>', owning_version_cadence_days)

def calc_knowledge_fragments_is_ai_validated_by_ai(source_agent_kind, is_human_owned):
    """
    TRUE when knowledge came from a non-human source AND is owned by a non-human-held role.
    
    Formula: =AND(NOT({{SourceAgentKind}} = "Human"), NOT({{IsHumanOwned}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(_erb.erb_eq(source_agent_kind, 'Human')))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_human_owned))))

def calc_knowledge_fragments_is_overdue_for_review(is_currently_valid, age_days, review_cadence_days):
    """
    TRUE when a currently-valid fragment has gone longer than its stewardship cadence without review.
    
    Formula: =AND({{IsCurrentlyValid}}, {{AgeDays}} > {{ReviewCadenceDays}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_valid), _erb.erb_bool3(_erb.erb_cmp(age_days, '>', review_cadence_days)))

def calc_knowledge_fragments_is_single_point_of_failure(is_from_single_witness, has_operational_reliance):
    """
    A claim that rests on exactly one person's word and that an active exception handler actually routes cases against.
    
    Formula: =AND({{IsFromSingleWitness}}, {{HasOperationalReliance}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_from_single_witness), _erb.erb_bool3(has_operational_reliance))

def calc_knowledge_fragments_valid_fragment_session_key(is_currently_valid, elicitation_session):
    """
    Composite-key echo: the elicitation session behind this fragment when the fragment is currently valid, blank otherwise.
    
    Formula: =IF({{IsCurrentlyValid}}, {{ElicitationSession}}, "")
    """
    return (elicitation_session if _erb.erb_bool3(is_currently_valid) else '')

def calc_knowledge_fragments_days_since_actual_review(has_review_record, as_of_instant, last_reviewed_at):
    """
    Days elapsed since this fragment was last actually reviewed. Zero when no review has ever been recorded — read this only alongside HasReviewRecord, never on its own.
    
    Formula: =IF({{HasReviewRecord}}, DATETIME_DIFF({{AsOfInstant}}, {{LastReviewedAt}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(as_of_instant, last_reviewed_at, 'days') if _erb.erb_bool3(has_review_record) else 0))

def calc_knowledge_fragments_is_unreviewed_since_authoring(is_currently_valid, has_review_record):
    """
    A currently-valid claim that nobody has ever reviewed since it was written.
    
    Formula: =AND({{IsCurrentlyValid}}, NOT({{HasReviewRecord}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_valid), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_review_record))))

def calc_knowledge_fragments_is_orphaned_by_role(is_currently_valid, owner_role_is_vacated):
    """
    A currently-valid claim whose owning role nobody holds — accountable to a vacancy.
    
    Formula: =AND({{IsCurrentlyValid}}, {{OwnerRoleIsVacated}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_valid), _erb.erb_bool3(owner_role_is_vacated))

def calc_knowledge_fragments_valid_fragment_version_key(is_currently_valid, procedure_version):
    """
    Composite-key echo: this fragment's owning procedure version when the fragment is currently valid, blank otherwise.
    
    Formula: =IF({{IsCurrentlyValid}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_currently_valid) else '')

# Level 3

def calc_knowledge_fragments_is_undefendable_tacit_claim(has_orphaned_provenance, knowledge_form):
    """
    TRUE when an orphaned claim is of a kind that lives in a person's head rather than in a document.
    
    Formula: =AND({{HasOrphanedProvenance}}, OR({{KnowledgeForm}} = "Tacit", {{KnowledgeForm}} = "SituatedJudgment"))
    """
    return _erb.erb_and(_erb.erb_bool3(has_orphaned_provenance), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(knowledge_form), 'Tacit')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(knowledge_form), 'SituatedJudgment')))))

def calc_knowledge_fragments_is_unapproved_but_relied_on(is_relied_upon, is_attached_to_live_version, is_approved):
    """
    TRUE when a live procedure is acting on a claim that has not been approved by the knowledge authority.
    
    Formula: =AND({{IsReliedUpon}}, {{IsAttachedToLiveVersion}}, NOT({{IsApproved}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_relied_upon), _erb.erb_bool3(is_attached_to_live_version), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_approved))))

def calc_knowledge_fragments_is_unapproved_and_operationally_live(is_my_unfinished_approval, has_operational_reliance):
    """
    TRUE when a claim awaiting my approval is already being acted on through a documented exception path.
    
    Formula: =AND({{IsMyUnfinishedApproval}}, {{HasOperationalReliance}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_my_unfinished_approval), _erb.erb_bool3(has_operational_reliance))

def calc_knowledge_fragments_is_aging_low_confidence_claim(exceeds_owning_cadence, is_low_confidence):
    """
    TRUE when a claim we were never sure about has also outlived its version's review cadence.
    
    Formula: =AND({{ExceedsOwningCadence}}, {{IsLowConfidence}})
    """
    return _erb.erb_and(_erb.erb_bool3(exceeds_owning_cadence), _erb.erb_bool3(is_low_confidence))

def calc_knowledge_fragments_fragility_signal_count(is_from_single_witness, is_overdue_for_review, is_low_confidence, has_operational_reliance):
    """
    How many of the four loop-1 decay signals are simultaneously true for this fragment: single witness, overdue for review, low confidence, operational reliance.
    
    Formula: =IF({{IsFromSingleWitness}}, 1, 0) + IF({{IsOverdueForReview}}, 1, 0) + IF({{IsLowConfidence}}, 1, 0) + IF({{HasOperationalReliance}}, 1, 0)
    """
    return _erb.erb_integer(_erb.erb_add(_erb.erb_add(_erb.erb_add((1 if _erb.erb_bool3(is_from_single_witness) else 0), (1 if _erb.erb_bool3(is_overdue_for_review) else 0)), (1 if _erb.erb_bool3(is_low_confidence) else 0)), (1 if _erb.erb_bool3(has_operational_reliance) else 0)))

def calc_knowledge_fragments_is_expiring_single_point_of_failure(is_single_point_of_failure, is_overdue_for_review):
    """
    A single-sourced, operationally relied-upon claim that is also past its review date.
    
    Formula: =AND({{IsSinglePointOfFailure}}, {{IsOverdueForReview}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_single_point_of_failure), _erb.erb_bool3(is_overdue_for_review))

def calc_knowledge_fragments_is_genuinely_overdue(is_currently_valid, has_review_record, days_since_actual_review, review_cadence_days):
    """
    A valid claim whose LAST ACTUAL REVIEW is older than the cadence its owning version promised.
    
    Formula: =AND({{IsCurrentlyValid}}, {{HasReviewRecord}}, {{DaysSinceActualReview}} > {{ReviewCadenceDays}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_valid), _erb.erb_bool3(has_review_record), _erb.erb_bool3(_erb.erb_cmp(days_since_actual_review, '>', review_cadence_days)))

def calc_knowledge_fragments_review_recency_is_inferred(is_overdue_for_review, has_review_record):
    """
    A fragment reported overdue by the loop-1 inference purely because no review has ever been recorded — the number is an artifact of missing data, not evidence of neglect.
    
    Formula: =AND({{IsOverdueForReview}}, NOT({{HasReviewRecord}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_overdue_for_review), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_review_record))))

def calc_knowledge_fragments_days_awaiting_my_approval(is_my_unfinished_approval, as_of_instant, valid_from):
    """
    How long a claim of mine has been sitting at Reviewed without my approval. Zero when the claim is not mine or is already decided.
    
    Formula: =IF({{IsMyUnfinishedApproval}}, DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(as_of_instant, valid_from, 'days') if _erb.erb_bool3(is_my_unfinished_approval) else 0))

# Level 4

def calc_knowledge_fragments_is_compound_fragile(fragility_signal_count):
    """
    A fragment carrying at least three of the four decay signals at once.
    
    Formula: ={{FragilitySignalCount}} >= 3
    """
    return _erb.erb_cmp(fragility_signal_count, '>=', 3)

def calc_knowledge_fragments_is_unapproved_and_machine_consumed(is_unapproved_but_relied_on, consuming_step_is_software_assigned):
    """
    An unapproved claim that a software-assigned step actually relies on — executed literally, with no human in position to notice it is wrong.
    
    Formula: =AND({{IsUnapprovedButReliedOn}}, {{ConsumingStepIsSoftwareAssigned}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_unapproved_but_relied_on), _erb.erb_bool3(consuming_step_is_software_assigned))

def calc_knowledge_fragments_is_unapproved_and_human_consumed(is_unapproved_but_relied_on, consuming_step_is_software_assigned):
    """
    An unapproved claim relied on by a human-assigned step — a reviewable risk rather than a silent one.
    
    Formula: =AND({{IsUnapprovedButReliedOn}}, NOT({{ConsumingStepIsSoftwareAssigned}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_unapproved_but_relied_on), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(consuming_step_is_software_assigned))))

def calc_knowledge_fragments_inference_disagrees_with_record(has_review_record, is_overdue_for_review, is_genuinely_overdue):
    """
    A fragment the ValidFrom inference calls overdue but which was in fact reviewed inside its cadence — a false positive in the loop-1 predicate, now provable.
    
    Formula: =AND({{HasReviewRecord}}, {{IsOverdueForReview}}, NOT({{IsGenuinelyOverdue}}))
    """
    return _erb.erb_and(_erb.erb_bool3(has_review_record), _erb.erb_bool3(is_overdue_for_review), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_genuinely_overdue))))

def calc_knowledge_fragments_genuinely_overdue_version_key(is_genuinely_overdue, procedure_version):
    """
    Composite-key echo: this fragment's procedure version when the fragment is genuinely overdue, blank otherwise.
    
    Formula: =IF({{IsGenuinelyOverdue}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_genuinely_overdue) else '')

def calc_knowledge_fragments_is_high_blast_radius_unapproved(is_unapproved_and_operationally_live, reliance_surface_count):
    """
    An unapproved, operationally live claim of mine with more than one distinct downstream dependent.
    
    Formula: =AND({{IsUnapprovedAndOperationallyLive}}, {{RelianceSurfaceCount}} > 1)
    """
    return _erb.erb_and(_erb.erb_bool3(is_unapproved_and_operationally_live), _erb.erb_bool3(_erb.erb_cmp(reliance_surface_count, '>', 1)))

def calc_knowledge_fragments_is_long_unapproved(is_my_unfinished_approval, days_awaiting_my_approval):
    """
    A claim that has waited on my signature for more than thirty days.
    
    Formula: =AND({{IsMyUnfinishedApproval}}, {{DaysAwaitingMyApproval}} > 30)
    """
    return _erb.erb_and(_erb.erb_bool3(is_my_unfinished_approval), _erb.erb_bool3(_erb.erb_cmp(days_awaiting_my_approval, '>', 30)))

# Level 5

def calc_knowledge_fragments_compound_fragile_version_key(is_compound_fragile, procedure_version):
    """
    Composite-key echo: this fragment's procedure version when the fragment is compound-fragile, blank otherwise.
    
    Formula: =IF({{IsCompoundFragile}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_compound_fragile) else '')

def calc_knowledge_fragments_machine_consumed_unapproved_version_key(is_unapproved_and_machine_consumed, procedure_version):
    """
    Composite-key echo: this fragment's procedure version when the fragment is unapproved and machine-consumed, blank otherwise.
    
    Formula: =IF({{IsUnapprovedAndMachineConsumed}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_unapproved_and_machine_consumed) else '')

def calc_knowledge_fragments_unapproved_load_bearing_version_key(is_high_blast_radius_unapproved, procedure_version):
    """
    Composite-key echo: this fragment's procedure version when it is a high-blast-radius unapproved claim, blank otherwise.
    
    Formula: =IF({{IsHighBlastRadiusUnapproved}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_high_blast_radius_unapproved) else '')


def compute_knowledge_fragments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for KnowledgeFragments.
    
    Explicit records of tacit, implicit, explicit, and situated procedural knowledge. This is an ERB-PKO extension represented as provenance-bearing dcat:Resource instances.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_knowledge_fragments_name(result.get('knowledge_form'), result.get('statement'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_currently_valid'] = calc_knowledge_fragments_is_currently_valid(result.get('valid_from'), result.get('as_of_instant'), result.get('valid_to'), result.get('status'))
    except Exception as _field_exc:
        result['is_currently_valid'] = None
        result.setdefault('_erb_errors', {})['is_currently_valid'] = str(_field_exc)
    try:
        result['has_human_source'] = calc_knowledge_fragments_has_human_source(result.get('source_agent_kind'))
    except Exception as _field_exc:
        result['has_human_source'] = None
        result.setdefault('_erb_errors', {})['has_human_source'] = str(_field_exc)
    try:
        result['is_approved'] = calc_knowledge_fragments_is_approved(result.get('status'))
    except Exception as _field_exc:
        result['is_approved'] = None
        result.setdefault('_erb_errors', {})['is_approved'] = str(_field_exc)
    try:
        result['is_within_validity_window'] = calc_knowledge_fragments_is_within_validity_window(result.get('valid_from'), result.get('as_of_instant'), result.get('valid_to'))
    except Exception as _field_exc:
        result['is_within_validity_window'] = None
        result.setdefault('_erb_errors', {})['is_within_validity_window'] = str(_field_exc)
    try:
        result['has_recorded_elicitation'] = calc_knowledge_fragments_has_recorded_elicitation(result.get('elicitation_session'))
    except Exception as _field_exc:
        result['has_recorded_elicitation'] = None
        result.setdefault('_erb_errors', {})['has_recorded_elicitation'] = str(_field_exc)
    try:
        result['evidence_expiry_days'] = calc_knowledge_fragments_evidence_expiry_days(result.get('is_from_single_witness'))
    except Exception as _field_exc:
        result['evidence_expiry_days'] = None
        result.setdefault('_erb_errors', {})['evidence_expiry_days'] = str(_field_exc)
    try:
        result['is_awaiting_approval'] = calc_knowledge_fragments_is_awaiting_approval(result.get('status'))
    except Exception as _field_exc:
        result['is_awaiting_approval'] = None
        result.setdefault('_erb_errors', {})['is_awaiting_approval'] = str(_field_exc)
    try:
        result['owner_is_me'] = calc_knowledge_fragments_owner_is_me(result.get('owner_role'))
    except Exception as _field_exc:
        result['owner_is_me'] = None
        result.setdefault('_erb_errors', {})['owner_is_me'] = str(_field_exc)
    try:
        result['has_operational_reliance'] = calc_knowledge_fragments_has_operational_reliance(result.get('is_invoked_by_an_exception'))
    except Exception as _field_exc:
        result['has_operational_reliance'] = None
        result.setdefault('_erb_errors', {})['has_operational_reliance'] = str(_field_exc)
    try:
        result['age_days'] = calc_knowledge_fragments_age_days(result.get('as_of_instant'), result.get('valid_from'))
    except Exception as _field_exc:
        result['age_days'] = None
        result.setdefault('_erb_errors', {})['age_days'] = str(_field_exc)
    try:
        result['is_low_confidence'] = calc_knowledge_fragments_is_low_confidence(result.get('confidence'))
    except Exception as _field_exc:
        result['is_low_confidence'] = None
        result.setdefault('_erb_errors', {})['is_low_confidence'] = str(_field_exc)
    try:
        result['is_human_owned'] = calc_knowledge_fragments_is_human_owned(result.get('owner_role_agent_kind'))
    except Exception as _field_exc:
        result['is_human_owned'] = None
        result.setdefault('_erb_errors', {})['is_human_owned'] = str(_field_exc)
    try:
        result['predates_current_role_holder'] = calc_knowledge_fragments_predates_current_role_holder(result.get('owner_role_agent_kind'), result.get('valid_from'), result.get('owner_role_assignment_valid_from'))
    except Exception as _field_exc:
        result['predates_current_role_holder'] = None
        result.setdefault('_erb_errors', {})['predates_current_role_holder'] = str(_field_exc)
    try:
        result['has_review_record'] = calc_knowledge_fragments_has_review_record(result.get('last_reviewed_at'))
    except Exception as _field_exc:
        result['has_review_record'] = None
        result.setdefault('_erb_errors', {})['has_review_record'] = str(_field_exc)
    try:
        result['reliance_surface_count'] = calc_knowledge_fragments_reliance_surface_count(result.get('is_invoked_by_an_exception'), result.get('ratified_boundary_count'))
    except Exception as _field_exc:
        result['reliance_surface_count'] = None
        result.setdefault('_erb_errors', {})['reliance_surface_count'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['has_orphaned_provenance'] = calc_knowledge_fragments_has_orphaned_provenance(result.get('is_currently_valid'), result.get('source_agent_is_still_engaged'))
    except Exception as _field_exc:
        result['has_orphaned_provenance'] = None
        result.setdefault('_erb_errors', {})['has_orphaned_provenance'] = str(_field_exc)
    try:
        result['is_relied_upon'] = calc_knowledge_fragments_is_relied_upon(result.get('step'), result.get('is_within_validity_window'))
    except Exception as _field_exc:
        result['is_relied_upon'] = None
        result.setdefault('_erb_errors', {})['is_relied_upon'] = str(_field_exc)
    try:
        result['evidence_has_expired'] = calc_knowledge_fragments_evidence_has_expired(result.get('has_recorded_elicitation'), result.get('evidence_age_days'), result.get('evidence_expiry_days'))
    except Exception as _field_exc:
        result['evidence_has_expired'] = None
        result.setdefault('_erb_errors', {})['evidence_has_expired'] = str(_field_exc)
    try:
        result['is_my_unfinished_approval'] = calc_knowledge_fragments_is_my_unfinished_approval(result.get('owner_is_me'), result.get('is_awaiting_approval'))
    except Exception as _field_exc:
        result['is_my_unfinished_approval'] = None
        result.setdefault('_erb_errors', {})['is_my_unfinished_approval'] = str(_field_exc)
    try:
        result['exceeds_owning_cadence'] = calc_knowledge_fragments_exceeds_owning_cadence(result.get('age_days'), result.get('owning_version_cadence_days'))
    except Exception as _field_exc:
        result['exceeds_owning_cadence'] = None
        result.setdefault('_erb_errors', {})['exceeds_owning_cadence'] = str(_field_exc)
    try:
        result['is_ai_validated_by_ai'] = calc_knowledge_fragments_is_ai_validated_by_ai(result.get('source_agent_kind'), result.get('is_human_owned'))
    except Exception as _field_exc:
        result['is_ai_validated_by_ai'] = None
        result.setdefault('_erb_errors', {})['is_ai_validated_by_ai'] = str(_field_exc)
    try:
        result['is_overdue_for_review'] = calc_knowledge_fragments_is_overdue_for_review(result.get('is_currently_valid'), result.get('age_days'), result.get('review_cadence_days'))
    except Exception as _field_exc:
        result['is_overdue_for_review'] = None
        result.setdefault('_erb_errors', {})['is_overdue_for_review'] = str(_field_exc)
    try:
        result['is_single_point_of_failure'] = calc_knowledge_fragments_is_single_point_of_failure(result.get('is_from_single_witness'), result.get('has_operational_reliance'))
    except Exception as _field_exc:
        result['is_single_point_of_failure'] = None
        result.setdefault('_erb_errors', {})['is_single_point_of_failure'] = str(_field_exc)
    try:
        result['valid_fragment_session_key'] = calc_knowledge_fragments_valid_fragment_session_key(result.get('is_currently_valid'), result.get('elicitation_session'))
    except Exception as _field_exc:
        result['valid_fragment_session_key'] = None
        result.setdefault('_erb_errors', {})['valid_fragment_session_key'] = str(_field_exc)
    try:
        result['days_since_actual_review'] = calc_knowledge_fragments_days_since_actual_review(result.get('has_review_record'), result.get('as_of_instant'), result.get('last_reviewed_at'))
    except Exception as _field_exc:
        result['days_since_actual_review'] = None
        result.setdefault('_erb_errors', {})['days_since_actual_review'] = str(_field_exc)
    try:
        result['is_unreviewed_since_authoring'] = calc_knowledge_fragments_is_unreviewed_since_authoring(result.get('is_currently_valid'), result.get('has_review_record'))
    except Exception as _field_exc:
        result['is_unreviewed_since_authoring'] = None
        result.setdefault('_erb_errors', {})['is_unreviewed_since_authoring'] = str(_field_exc)
    try:
        result['is_orphaned_by_role'] = calc_knowledge_fragments_is_orphaned_by_role(result.get('is_currently_valid'), result.get('owner_role_is_vacated'))
    except Exception as _field_exc:
        result['is_orphaned_by_role'] = None
        result.setdefault('_erb_errors', {})['is_orphaned_by_role'] = str(_field_exc)
    try:
        result['valid_fragment_version_key'] = calc_knowledge_fragments_valid_fragment_version_key(result.get('is_currently_valid'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['valid_fragment_version_key'] = None
        result.setdefault('_erb_errors', {})['valid_fragment_version_key'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_undefendable_tacit_claim'] = calc_knowledge_fragments_is_undefendable_tacit_claim(result.get('has_orphaned_provenance'), result.get('knowledge_form'))
    except Exception as _field_exc:
        result['is_undefendable_tacit_claim'] = None
        result.setdefault('_erb_errors', {})['is_undefendable_tacit_claim'] = str(_field_exc)
    try:
        result['is_unapproved_but_relied_on'] = calc_knowledge_fragments_is_unapproved_but_relied_on(result.get('is_relied_upon'), result.get('is_attached_to_live_version'), result.get('is_approved'))
    except Exception as _field_exc:
        result['is_unapproved_but_relied_on'] = None
        result.setdefault('_erb_errors', {})['is_unapproved_but_relied_on'] = str(_field_exc)
    try:
        result['is_unapproved_and_operationally_live'] = calc_knowledge_fragments_is_unapproved_and_operationally_live(result.get('is_my_unfinished_approval'), result.get('has_operational_reliance'))
    except Exception as _field_exc:
        result['is_unapproved_and_operationally_live'] = None
        result.setdefault('_erb_errors', {})['is_unapproved_and_operationally_live'] = str(_field_exc)
    try:
        result['is_aging_low_confidence_claim'] = calc_knowledge_fragments_is_aging_low_confidence_claim(result.get('exceeds_owning_cadence'), result.get('is_low_confidence'))
    except Exception as _field_exc:
        result['is_aging_low_confidence_claim'] = None
        result.setdefault('_erb_errors', {})['is_aging_low_confidence_claim'] = str(_field_exc)
    try:
        result['fragility_signal_count'] = calc_knowledge_fragments_fragility_signal_count(result.get('is_from_single_witness'), result.get('is_overdue_for_review'), result.get('is_low_confidence'), result.get('has_operational_reliance'))
    except Exception as _field_exc:
        result['fragility_signal_count'] = None
        result.setdefault('_erb_errors', {})['fragility_signal_count'] = str(_field_exc)
    try:
        result['is_expiring_single_point_of_failure'] = calc_knowledge_fragments_is_expiring_single_point_of_failure(result.get('is_single_point_of_failure'), result.get('is_overdue_for_review'))
    except Exception as _field_exc:
        result['is_expiring_single_point_of_failure'] = None
        result.setdefault('_erb_errors', {})['is_expiring_single_point_of_failure'] = str(_field_exc)
    try:
        result['is_genuinely_overdue'] = calc_knowledge_fragments_is_genuinely_overdue(result.get('is_currently_valid'), result.get('has_review_record'), result.get('days_since_actual_review'), result.get('review_cadence_days'))
    except Exception as _field_exc:
        result['is_genuinely_overdue'] = None
        result.setdefault('_erb_errors', {})['is_genuinely_overdue'] = str(_field_exc)
    try:
        result['review_recency_is_inferred'] = calc_knowledge_fragments_review_recency_is_inferred(result.get('is_overdue_for_review'), result.get('has_review_record'))
    except Exception as _field_exc:
        result['review_recency_is_inferred'] = None
        result.setdefault('_erb_errors', {})['review_recency_is_inferred'] = str(_field_exc)
    try:
        result['days_awaiting_my_approval'] = calc_knowledge_fragments_days_awaiting_my_approval(result.get('is_my_unfinished_approval'), result.get('as_of_instant'), result.get('valid_from'))
    except Exception as _field_exc:
        result['days_awaiting_my_approval'] = None
        result.setdefault('_erb_errors', {})['days_awaiting_my_approval'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['is_compound_fragile'] = calc_knowledge_fragments_is_compound_fragile(result.get('fragility_signal_count'))
    except Exception as _field_exc:
        result['is_compound_fragile'] = None
        result.setdefault('_erb_errors', {})['is_compound_fragile'] = str(_field_exc)
    try:
        result['is_unapproved_and_machine_consumed'] = calc_knowledge_fragments_is_unapproved_and_machine_consumed(result.get('is_unapproved_but_relied_on'), result.get('consuming_step_is_software_assigned'))
    except Exception as _field_exc:
        result['is_unapproved_and_machine_consumed'] = None
        result.setdefault('_erb_errors', {})['is_unapproved_and_machine_consumed'] = str(_field_exc)
    try:
        result['is_unapproved_and_human_consumed'] = calc_knowledge_fragments_is_unapproved_and_human_consumed(result.get('is_unapproved_but_relied_on'), result.get('consuming_step_is_software_assigned'))
    except Exception as _field_exc:
        result['is_unapproved_and_human_consumed'] = None
        result.setdefault('_erb_errors', {})['is_unapproved_and_human_consumed'] = str(_field_exc)
    try:
        result['inference_disagrees_with_record'] = calc_knowledge_fragments_inference_disagrees_with_record(result.get('has_review_record'), result.get('is_overdue_for_review'), result.get('is_genuinely_overdue'))
    except Exception as _field_exc:
        result['inference_disagrees_with_record'] = None
        result.setdefault('_erb_errors', {})['inference_disagrees_with_record'] = str(_field_exc)
    try:
        result['genuinely_overdue_version_key'] = calc_knowledge_fragments_genuinely_overdue_version_key(result.get('is_genuinely_overdue'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['genuinely_overdue_version_key'] = None
        result.setdefault('_erb_errors', {})['genuinely_overdue_version_key'] = str(_field_exc)
    try:
        result['is_high_blast_radius_unapproved'] = calc_knowledge_fragments_is_high_blast_radius_unapproved(result.get('is_unapproved_and_operationally_live'), result.get('reliance_surface_count'))
    except Exception as _field_exc:
        result['is_high_blast_radius_unapproved'] = None
        result.setdefault('_erb_errors', {})['is_high_blast_radius_unapproved'] = str(_field_exc)
    try:
        result['is_long_unapproved'] = calc_knowledge_fragments_is_long_unapproved(result.get('is_my_unfinished_approval'), result.get('days_awaiting_my_approval'))
    except Exception as _field_exc:
        result['is_long_unapproved'] = None
        result.setdefault('_erb_errors', {})['is_long_unapproved'] = str(_field_exc)

    # Level 5 calculations
    try:
        result['compound_fragile_version_key'] = calc_knowledge_fragments_compound_fragile_version_key(result.get('is_compound_fragile'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['compound_fragile_version_key'] = None
        result.setdefault('_erb_errors', {})['compound_fragile_version_key'] = str(_field_exc)
    try:
        result['machine_consumed_unapproved_version_key'] = calc_knowledge_fragments_machine_consumed_unapproved_version_key(result.get('is_unapproved_and_machine_consumed'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['machine_consumed_unapproved_version_key'] = None
        result.setdefault('_erb_errors', {})['machine_consumed_unapproved_version_key'] = str(_field_exc)
    try:
        result['unapproved_load_bearing_version_key'] = calc_knowledge_fragments_unapproved_load_bearing_version_key(result.get('is_high_blast_radius_unapproved'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['unapproved_load_bearing_version_key'] = None
        result.setdefault('_erb_errors', {})['unapproved_load_bearing_version_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'compound_fragile_version_key', 'valid_fragment_session_key', 'machine_consumed_unapproved_version_key', 'genuinely_overdue_version_key', 'unapproved_load_bearing_version_key', 'valid_fragment_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# KNOWLEDGEGAPS
# Known unknowns and missing procedural coverage. Explicit ERB-PKO extension used to govern scope and prevent silent incompleteness.
# =============================================================================

# Level 1

def calc_knowledge_gaps_name(severity, statement):
    """
    Human-readable calculated display alias for the KnowledgeGaps row.
    
    Formula: ={{Severity}} & ": " & LEFT({{Statement}}, 60)
    """
    return (str(severity or "") + ': ' + str(((statement or "")[:(60 or 0)]) if ((statement or "")[:(60 or 0)]) is not None else ""))

def calc_knowledge_gaps_is_open(status):
    """
    TRUE when the gap remains open.
    
    Formula: =OR({{Status}} = "Open", {{Status}} = "Investigating")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Open')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Investigating')))

def calc_knowledge_gaps_is_blocking(blocking_kind):
    """
    TRUE when this gap is declared blocking rather than informational.
    
    Formula: ={{BlockingKind}} = "Blocking" 
    """
    return _erb.erb_eq(_erb.erb_nullif(blocking_kind), 'Blocking')

def calc_knowledge_gaps_tolerance_days(severity):
    """
    How long a gap of this severity may remain open before it becomes a governance failure in its own right.
    
    Formula: =IF({{Severity}} = "High", 30, IF({{Severity}} = "Medium", 90, 180))
    """
    return _erb.erb_integer((30 if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(severity), 'High')) else (90 if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(severity), 'Medium')) else 180)))

def calc_knowledge_gaps_has_resolution_plan(resolution_plan):
    """
    TRUE when someone has written down how this gap would be closed.
    
    Formula: ={{ResolutionPlan}} <> "" 
    """
    return (not (resolution_plan is None or resolution_plan == ""))

# Level 2

def calc_knowledge_gaps_open_gap_version_key(is_open, severity, procedure_version):
    """
    Echoes the ProcedureVersion id for open high-severity knowledge gaps.
    
    Formula: =IF(AND({{IsOpen}}, {{Severity}} = "High"), {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_open), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(severity), 'High')))) else '')

def calc_knowledge_gaps_is_open_and_blocking(is_open, is_blocking):
    """
    TRUE when this gap is both unresolved and declared blocking.
    
    Formula: =AND({{IsOpen}}, {{IsBlocking}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_open), _erb.erb_bool3(is_blocking))

def calc_knowledge_gaps_days_open(is_open, as_of_instant, identified_at):
    """
    Days this gap has been unresolved, or zero once closed.
    
    Formula: =IF({{IsOpen}}, DATETIME_DIFF({{AsOfInstant}}, {{IdentifiedAt}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(as_of_instant, identified_at, 'days') if _erb.erb_bool3(is_open) else 0))

def calc_knowledge_gaps_is_ownerless_open_gap(is_open, owner_role_is_vacated):
    """
    An open gap whose owning role nobody currently holds — an acknowledged unknown with nobody accountable for closing it.
    
    Formula: =AND({{IsOpen}}, {{OwnerRoleIsVacated}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_open), _erb.erb_bool3(owner_role_is_vacated))

# Level 3

def calc_knowledge_gaps_is_overdue_gap(days_open, tolerance_days):
    """
    TRUE when this acknowledged unknown has outlived the tolerance for its severity.
    
    Formula: ={{DaysOpen}} > {{ToleranceDays}}
    """
    return _erb.erb_cmp(days_open, '>', tolerance_days)

def calc_knowledge_gaps_open_blocking_gap_version_key(is_open_and_blocking, procedure_version):
    """
    Composite-key echo: this gap's procedure version when the gap is both open and blocking, blank otherwise.
    
    Formula: =IF({{IsOpenAndBlocking}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_open_and_blocking) else '')

# Level 4

def calc_knowledge_gaps_is_abandoned_unknown(is_overdue_gap, has_resolution_plan, owner_is_still_engaged):
    """
    TRUE when an overdue gap has either no plan or no living owner — an admission of ignorance that nobody is acting on.
    
    Formula: =AND({{IsOverdueGap}}, OR(NOT({{HasResolutionPlan}}), NOT({{OwnerIsStillEngaged}})))
    """
    return _erb.erb_and(_erb.erb_bool3(is_overdue_gap), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_resolution_plan))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(owner_is_still_engaged))))))


def compute_knowledge_gaps_fields(record: dict) -> dict:
    """
    Compute all calculated fields for KnowledgeGaps.
    
    Known unknowns and missing procedural coverage. Explicit ERB-PKO extension used to govern scope and prevent silent incompleteness.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_knowledge_gaps_name(result.get('severity'), result.get('statement'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_open'] = calc_knowledge_gaps_is_open(result.get('status'))
    except Exception as _field_exc:
        result['is_open'] = None
        result.setdefault('_erb_errors', {})['is_open'] = str(_field_exc)
    try:
        result['is_blocking'] = calc_knowledge_gaps_is_blocking(result.get('blocking_kind'))
    except Exception as _field_exc:
        result['is_blocking'] = None
        result.setdefault('_erb_errors', {})['is_blocking'] = str(_field_exc)
    try:
        result['tolerance_days'] = calc_knowledge_gaps_tolerance_days(result.get('severity'))
    except Exception as _field_exc:
        result['tolerance_days'] = None
        result.setdefault('_erb_errors', {})['tolerance_days'] = str(_field_exc)
    try:
        result['has_resolution_plan'] = calc_knowledge_gaps_has_resolution_plan(result.get('resolution_plan'))
    except Exception as _field_exc:
        result['has_resolution_plan'] = None
        result.setdefault('_erb_errors', {})['has_resolution_plan'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['open_gap_version_key'] = calc_knowledge_gaps_open_gap_version_key(result.get('is_open'), result.get('severity'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['open_gap_version_key'] = None
        result.setdefault('_erb_errors', {})['open_gap_version_key'] = str(_field_exc)
    try:
        result['is_open_and_blocking'] = calc_knowledge_gaps_is_open_and_blocking(result.get('is_open'), result.get('is_blocking'))
    except Exception as _field_exc:
        result['is_open_and_blocking'] = None
        result.setdefault('_erb_errors', {})['is_open_and_blocking'] = str(_field_exc)
    try:
        result['days_open'] = calc_knowledge_gaps_days_open(result.get('is_open'), result.get('as_of_instant'), result.get('identified_at'))
    except Exception as _field_exc:
        result['days_open'] = None
        result.setdefault('_erb_errors', {})['days_open'] = str(_field_exc)
    try:
        result['is_ownerless_open_gap'] = calc_knowledge_gaps_is_ownerless_open_gap(result.get('is_open'), result.get('owner_role_is_vacated'))
    except Exception as _field_exc:
        result['is_ownerless_open_gap'] = None
        result.setdefault('_erb_errors', {})['is_ownerless_open_gap'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_overdue_gap'] = calc_knowledge_gaps_is_overdue_gap(result.get('days_open'), result.get('tolerance_days'))
    except Exception as _field_exc:
        result['is_overdue_gap'] = None
        result.setdefault('_erb_errors', {})['is_overdue_gap'] = str(_field_exc)
    try:
        result['open_blocking_gap_version_key'] = calc_knowledge_gaps_open_blocking_gap_version_key(result.get('is_open_and_blocking'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['open_blocking_gap_version_key'] = None
        result.setdefault('_erb_errors', {})['open_blocking_gap_version_key'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['is_abandoned_unknown'] = calc_knowledge_gaps_is_abandoned_unknown(result.get('is_overdue_gap'), result.get('has_resolution_plan'), result.get('owner_is_still_engaged'))
    except Exception as _field_exc:
        result['is_abandoned_unknown'] = None
        result.setdefault('_erb_errors', {})['is_abandoned_unknown'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'open_gap_version_key', 'open_blocking_gap_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# FAQS
# Frequently asked procedural questions. Maps to pko:FrequentlyAskedQuestion, question, answer, hasFAQCategory, and hasFAQTarget.
# =============================================================================

# Level 1

def calc_fa_qs_name(question):
    """
    Human-readable calculated display alias for the FAQs row.
    
    Formula: ={{Question}}
    """
    return question


def compute_fa_qs_fields(record: dict) -> dict:
    """
    Compute all calculated fields for FAQs.
    
    Frequently asked procedural questions. Maps to pko:FrequentlyAskedQuestion, question, answer, hasFAQCategory, and hasFAQTarget.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_fa_qs_name(result.get('question'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# EXPLANATIONS
# Explainable derivation artifacts associated with procedural decisions. Maps to pko:Explanation and pko:hasExplanation.
# =============================================================================

# Level 1

def calc_explanations_name(title):
    """
    Human-readable calculated display alias for the Explanations row.
    
    Formula: ={{Title}}
    """
    return title


def compute_explanations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Explanations.
    
    Explainable derivation artifacts associated with procedural decisions. Maps to pko:Explanation and pko:hasExplanation.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_explanations_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCEDUREEXECUTIONS
# Concrete enactments of procedure specifications. Maps to pko:ProcedureExecution and remains separate from ProcedureVersions.
# =============================================================================

# Level 1

def calc_procedure_executions_name(procedure_version, context):
    """
    Human-readable calculated display alias for the ProcedureExecutions row.
    
    Formula: ={{ProcedureVersion}} & " / " & {{Context}}
    """
    return (str(procedure_version or "") + ' / ' + str(context or ""))

def calc_procedure_executions_is_structurally_complete(completed_step_count, expected_step_count):
    """
    TRUE when every specified step of the procedure version reached Completed in this execution.
    
    Formula: ={{CompletedStepCount}} >= {{ExpectedStepCount}}
    """
    return _erb.erb_cmp(completed_step_count, '>=', expected_step_count)

def calc_procedure_executions_all_blocking_controls_evaluated(unevaluated_blocking_total):
    """
    TRUE when every blocking requirement bound to every step of this execution received a satisfaction record.
    
    Formula: ={{UnevaluatedBlockingTotal}} = 0
    """
    return _erb.erb_eq(unevaluated_blocking_total, 0)

def calc_procedure_executions_separation_of_duties_held(separation_violation_count):
    """
    TRUE when no agent both prepared and approved within this execution.
    
    Formula: ={{SeparationViolationCount}} = 0
    """
    return _erb.erb_eq(separation_violation_count, 0)

def calc_procedure_executions_signed_against_unfit_version(execution_status, executed_version_is_fit):
    """
    TRUE when a completed execution was run against a procedure version the organization no longer stands behind.
    
    Formula: =AND({{ExecutionStatus}} = "Completed", NOT({{ExecutedVersionIsFit}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(execution_status), 'Completed')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(executed_version_is_fit))))

def calc_procedure_executions_assurance_is_mostly_asserted(asserted_only_control_count):
    """
    TRUE when any blocking control in this execution passed without a computed witness behind it.
    
    Formula: ={{AssertedOnlyControlCount}} > 0
    """
    return _erb.erb_cmp(asserted_only_control_count, '>', 0)

def calc_procedure_executions_has_cleared_legal_review(cleared_legal_review_count):
    """
    TRUE when legal review has passed for this execution.
    
    Formula: ={{ClearedLegalReviewCount}} > 0
    """
    return _erb.erb_cmp(cleared_legal_review_count, '>', 0)

def calc_procedure_executions_has_abandoned_failures(abandoned_failure_count):
    """
    TRUE when this run has any untriaged delivery failure.
    
    Formula: ={{AbandonedFailureCount}} > 0
    """
    return _erb.erb_cmp(abandoned_failure_count, '>', 0)

def calc_procedure_executions_ran_clean(unclean_step_count):
    """
    TRUE when every step execution in this run was clean.
    
    Formula: ={{UncleanStepCount}} = 0
    """
    return _erb.erb_eq(unclean_step_count, 0)

def calc_procedure_executions_has_human_approval(count_of_approval_executions):
    """
    TRUE when at least one human approval gate was executed in this run.
    
    Formula: ={{CountOfApprovalExecutions}} > 0
    """
    return _erb.erb_cmp(count_of_approval_executions, '>', 0)

def calc_procedure_executions_has_delivered(count_of_delivery_executions):
    """
    TRUE when this run has sent communications to employees.
    
    Formula: ={{CountOfDeliveryExecutions}} > 0
    """
    return _erb.erb_cmp(count_of_delivery_executions, '>', 0)

def calc_procedure_executions_approval_chain_is_complete(invalid_approval_count):
    """
    TRUE when every approval-type requirement in this run is fully satisfied by a human.
    
    Formula: ={{InvalidApprovalCount}} = 0
    """
    return _erb.erb_eq(invalid_approval_count, 0)

def calc_procedure_executions_separation_was_testable(preparation_step_count, approval_step_count):
    """
    TRUE when this execution contained both a preparation step and an approval step, so segregation of duties had an opportunity to fail.
    
    Formula: =AND({{PreparationStepCount}} > 0, {{ApprovalStepCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(preparation_step_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(approval_step_count, '>', 0)))

def calc_procedure_executions_evaluated_control_count(computedly_witnessed_control_count, asserted_only_control_count):
    """
    Total blocking controls actually evaluated in this run, computed and asserted together.
    
    Formula: ={{ComputedlyWitnessedControlCount}} + {{AssertedOnlyControlCount}}
    """
    return _erb.erb_add(computedly_witnessed_control_count, asserted_only_control_count)

def calc_procedure_executions_has_any_independent_observation(independent_human_observation_count):
    """
    TRUE when at least one verification in this run was independently observed by a human.
    
    Formula: ={{IndependentHumanObservationCount}} > 0
    """
    return _erb.erb_cmp(independent_human_observation_count, '>', 0)

def calc_procedure_executions_has_been_attested(attestation_count):
    """
    TRUE when someone has signed for this execution.
    
    Formula: ={{AttestationCount}} > 0
    """
    return _erb.erb_cmp(attestation_count, '>', 0)

def calc_procedure_executions_delivery_yield_percent(intended_recipient_count, reached_recipient_count):
    """
    Percentage of intended recipients who actually received the communication.
    
    Formula: =IF({{IntendedRecipientCount}} > 0, {{ReachedRecipientCount}} * 100 / {{IntendedRecipientCount}}, 0)
    """
    return (_erb.erb_div(_erb.erb_mul(reached_recipient_count, 100), intended_recipient_count) if _erb.erb_bool3(_erb.erb_cmp(intended_recipient_count, '>', 0)) else 0)

def calc_procedure_executions_campaign_silently_lost_audience(silently_dropped_count):
    """
    TRUE when a campaign lost intended recipients without producing any record of the loss.
    
    Formula: =({{SilentlyDroppedCount}} > 0)
    """
    return _erb.erb_cmp(silently_dropped_count, '>', 0)

def calc_procedure_executions_has_unrecorded_refusals(unrecorded_refusal_count):
    """
    TRUE when this run contains at least one refusal that left no trace anywhere.
    
    Formula: =({{UnrecordedRefusalCount}} > 0)
    """
    return _erb.erb_cmp(unrecorded_refusal_count, '>', 0)

def calc_procedure_executions_send_decisions_are_entirely_self_witnessed(intended_recipient_count, independently_confirmed_intent_count):
    """
    TRUE when no send decision in this run was confirmed by anything other than the pipeline itself.
    
    Formula: =AND({{IntendedRecipientCount}} > 0, {{IndependentlyConfirmedIntentCount}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(intended_recipient_count, '>', 0)), _erb.erb_bool3(_erb.erb_eq(independently_confirmed_intent_count, 0)))

# Level 2

def calc_procedure_executions_diverged_from_specification(is_structurally_complete, control_breach_count):
    """
    TRUE when the execution either skipped specified steps or carried at least one control breach.
    
    Formula: =OR(NOT({{IsStructurallyComplete}}), {{ControlBreachCount}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_structurally_complete))), _erb.erb_bool3(_erb.erb_cmp(control_breach_count, '>', 0)))

def calc_procedure_executions_delivered_without_approval(has_delivered, has_human_approval):
    """
    TRUE when a run sent employee communications without executing a human approval gate.
    
    Formula: =AND({{HasDelivered}}, NOT({{HasHumanApproval}}))
    """
    return _erb.erb_and(_erb.erb_bool3(has_delivered), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_human_approval))))

def calc_procedure_executions_separation_held_under_test(separation_was_testable, separation_of_duties_held):
    """
    TRUE only when segregation of duties both could have failed and did not.
    
    Formula: =AND({{SeparationWasTestable}}, {{SeparationOfDutiesHeld}})
    """
    return _erb.erb_and(_erb.erb_bool3(separation_was_testable), _erb.erb_bool3(separation_of_duties_held))

def calc_procedure_executions_separation_is_vacuously_green(separation_of_duties_held, separation_was_testable):
    """
    TRUE when the segregation control reports as held on a run where it could not have failed.
    
    Formula: =AND({{SeparationOfDutiesHeld}}, NOT({{SeparationWasTestable}}))
    """
    return _erb.erb_and(_erb.erb_bool3(separation_of_duties_held), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(separation_was_testable))))

def calc_procedure_executions_computed_assurance_ratio(evaluated_control_count, computedly_witnessed_control_count):
    """
    The fraction of evaluated blocking controls in this run that rest on a computed witness rather than a human assertion.
    
    Formula: =IF({{EvaluatedControlCount}} = 0, 0, {{ComputedlyWitnessedControlCount}} / {{EvaluatedControlCount}})
    """
    return (0 if _erb.erb_bool3(_erb.erb_eq(evaluated_control_count, 0)) else _erb.erb_div(computedly_witnessed_control_count, evaluated_control_count))

def calc_procedure_executions_assurance_chain_is_circular(self_attested_approval_count, has_any_independent_observation):
    """
    TRUE when every approval in this run rests on self-attestation and no independent human observation exists anywhere in it.
    
    Formula: =AND({{SelfAttestedApprovalCount}} > 0, NOT({{HasAnyIndependentObservation}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(self_attested_approval_count, '>', 0)), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_any_independent_observation))))

def calc_procedure_executions_basis_changed_after_signature(has_been_attested, post_attestation_score_count):
    """
    TRUE when controls this attestation depended on were scored after the signature was given.
    
    Formula: =AND({{HasBeenAttested}}, {{PostAttestationScoreCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(has_been_attested), _erb.erb_bool3(_erb.erb_cmp(post_attestation_score_count, '>', 0)))

# Level 3

def calc_procedure_executions_is_attestation_ready(is_structurally_complete, diverged_from_specification, all_blocking_controls_evaluated, separation_of_duties_held):
    """
    TRUE only when every step completed, no control breached, every blocking control was actually evaluated, and segregation of duties held.
    
    Formula: =AND({{IsStructurallyComplete}}, NOT({{DivergedFromSpecification}}), {{AllBlockingControlsEvaluated}}, {{SeparationOfDutiesHeld}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_structurally_complete), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(diverged_from_specification))), _erb.erb_bool3(all_blocking_controls_evaluated), _erb.erb_bool3(separation_of_duties_held))

def calc_procedure_executions_separation_assurance_note(separation_violation_count, separation_is_vacuously_green):
    """
    The sentence that goes into the control narrative for this execution.
    
    Formula: =IF({{SeparationViolationCount}} > 0, "Violated: same agent prepared and approved.", IF({{SeparationIsVacuouslyGreen}}, "Not tested: this run had no preparation/approval pair.", "Held under test."))
    """
    return ('Violated: same agent prepared and approved.' if _erb.erb_bool3(_erb.erb_cmp(separation_violation_count, '>', 0)) else ('Not tested: this run had no preparation/approval pair.' if _erb.erb_bool3(separation_is_vacuously_green) else 'Held under test.'))

def calc_procedure_executions_divergence_was_fully_governed(diverged_from_specification, ungoverned_divergence_count):
    """
    TRUE when this run departed from specification and every departure was authorised.
    
    Formula: =AND({{DivergedFromSpecification}}, {{UngovernedDivergenceCount}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(diverged_from_specification), _erb.erb_bool3(_erb.erb_eq(ungoverned_divergence_count, 0)))

def calc_procedure_executions_assurance_grade(evaluated_control_count, interested_party_assertion_count, computed_assurance_ratio):
    """
    Names the quality of the assurance behind this execution, worst case first.
    
    Formula: =IF({{EvaluatedControlCount}} = 0, "None: no blocking control was evaluated.", IF({{InterestedPartyAssertionCount}} > 0, "Weak: at least one control rests on an interested-party assertion.", IF({{ComputedAssuranceRatio}} < 0.5, "Thin: most controls rest on human assertion.", IF({{ComputedAssuranceRatio}} < 1, "Mixed: computed and asserted controls.", "Computed: every evaluated control has a witness."))))
    """
    return ('None: no blocking control was evaluated.' if _erb.erb_bool3(_erb.erb_eq(evaluated_control_count, 0)) else ('Weak: at least one control rests on an interested-party assertion.' if _erb.erb_bool3(_erb.erb_cmp(interested_party_assertion_count, '>', 0)) else ('Thin: most controls rest on human assertion.' if _erb.erb_bool3(_erb.erb_cmp(computed_assurance_ratio, '<', 0.5)) else ('Mixed: computed and asserted controls.' if _erb.erb_bool3(_erb.erb_cmp(computed_assurance_ratio, '<', 1)) else 'Computed: every evaluated control has a witness.'))))

# Level 4

def calc_procedure_executions_attestation_blocker_summary(is_attestation_ready, is_structurally_complete, separation_violation_count, unevaluated_blocking_total):
    """
    Names the highest-severity reason the attestation cannot be signed, or empty when it can.
    
    Formula: =IF({{IsAttestationReady}}, "", IF(NOT({{IsStructurallyComplete}}), "Incomplete: specified steps did not all complete.", IF({{SeparationViolationCount}} > 0, "Segregation of duties violated.", IF({{UnevaluatedBlockingTotal}} > 0, "Blocking controls were never evaluated.", "Control breach recorded on one or more steps."))))
    """
    return ('' if _erb.erb_bool3(is_attestation_ready) else ('Incomplete: specified steps did not all complete.' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_structurally_complete))) else ('Segregation of duties violated.' if _erb.erb_bool3(_erb.erb_cmp(separation_violation_count, '>', 0)) else ('Blocking controls were never evaluated.' if _erb.erb_bool3(_erb.erb_cmp(unevaluated_blocking_total, '>', 0)) else 'Control breach recorded on one or more steps.'))))

def calc_procedure_executions_attestation_would_be_weakly_based(is_attestation_ready, interested_party_assertion_count, computed_assurance_ratio):
    """
    TRUE when the model says I may sign, but the basis for that permission is mostly or partly unwitnessed assertion.
    
    Formula: =AND({{IsAttestationReady}}, OR({{InterestedPartyAssertionCount}} > 0, {{ComputedAssuranceRatio}} < 0.5))
    """
    return _erb.erb_and(_erb.erb_bool3(is_attestation_ready), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(interested_party_assertion_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(computed_assurance_ratio, '<', 0.5)))))

def calc_procedure_executions_requires_re_attestation(basis_changed_after_signature, is_attestation_ready):
    """
    TRUE when the basis changed after signature AND the execution no longer reads as attestable.
    
    Formula: =AND({{BasisChangedAfterSignature}}, NOT({{IsAttestationReady}}))
    """
    return _erb.erb_and(_erb.erb_bool3(basis_changed_after_signature), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_attestation_ready))))


def compute_procedure_executions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcedureExecutions.
    
    Concrete enactments of procedure specifications. Maps to pko:ProcedureExecution and remains separate from ProcedureVersions.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_procedure_executions_name(result.get('procedure_version'), result.get('context'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_structurally_complete'] = calc_procedure_executions_is_structurally_complete(result.get('completed_step_count'), result.get('expected_step_count'))
    except Exception as _field_exc:
        result['is_structurally_complete'] = None
        result.setdefault('_erb_errors', {})['is_structurally_complete'] = str(_field_exc)
    try:
        result['all_blocking_controls_evaluated'] = calc_procedure_executions_all_blocking_controls_evaluated(result.get('unevaluated_blocking_total'))
    except Exception as _field_exc:
        result['all_blocking_controls_evaluated'] = None
        result.setdefault('_erb_errors', {})['all_blocking_controls_evaluated'] = str(_field_exc)
    try:
        result['separation_of_duties_held'] = calc_procedure_executions_separation_of_duties_held(result.get('separation_violation_count'))
    except Exception as _field_exc:
        result['separation_of_duties_held'] = None
        result.setdefault('_erb_errors', {})['separation_of_duties_held'] = str(_field_exc)
    try:
        result['signed_against_unfit_version'] = calc_procedure_executions_signed_against_unfit_version(result.get('execution_status'), result.get('executed_version_is_fit'))
    except Exception as _field_exc:
        result['signed_against_unfit_version'] = None
        result.setdefault('_erb_errors', {})['signed_against_unfit_version'] = str(_field_exc)
    try:
        result['assurance_is_mostly_asserted'] = calc_procedure_executions_assurance_is_mostly_asserted(result.get('asserted_only_control_count'))
    except Exception as _field_exc:
        result['assurance_is_mostly_asserted'] = None
        result.setdefault('_erb_errors', {})['assurance_is_mostly_asserted'] = str(_field_exc)
    try:
        result['has_cleared_legal_review'] = calc_procedure_executions_has_cleared_legal_review(result.get('cleared_legal_review_count'))
    except Exception as _field_exc:
        result['has_cleared_legal_review'] = None
        result.setdefault('_erb_errors', {})['has_cleared_legal_review'] = str(_field_exc)
    try:
        result['has_abandoned_failures'] = calc_procedure_executions_has_abandoned_failures(result.get('abandoned_failure_count'))
    except Exception as _field_exc:
        result['has_abandoned_failures'] = None
        result.setdefault('_erb_errors', {})['has_abandoned_failures'] = str(_field_exc)
    try:
        result['ran_clean'] = calc_procedure_executions_ran_clean(result.get('unclean_step_count'))
    except Exception as _field_exc:
        result['ran_clean'] = None
        result.setdefault('_erb_errors', {})['ran_clean'] = str(_field_exc)
    try:
        result['has_human_approval'] = calc_procedure_executions_has_human_approval(result.get('count_of_approval_executions'))
    except Exception as _field_exc:
        result['has_human_approval'] = None
        result.setdefault('_erb_errors', {})['has_human_approval'] = str(_field_exc)
    try:
        result['has_delivered'] = calc_procedure_executions_has_delivered(result.get('count_of_delivery_executions'))
    except Exception as _field_exc:
        result['has_delivered'] = None
        result.setdefault('_erb_errors', {})['has_delivered'] = str(_field_exc)
    try:
        result['approval_chain_is_complete'] = calc_procedure_executions_approval_chain_is_complete(result.get('invalid_approval_count'))
    except Exception as _field_exc:
        result['approval_chain_is_complete'] = None
        result.setdefault('_erb_errors', {})['approval_chain_is_complete'] = str(_field_exc)
    try:
        result['separation_was_testable'] = calc_procedure_executions_separation_was_testable(result.get('preparation_step_count'), result.get('approval_step_count'))
    except Exception as _field_exc:
        result['separation_was_testable'] = None
        result.setdefault('_erb_errors', {})['separation_was_testable'] = str(_field_exc)
    try:
        result['evaluated_control_count'] = calc_procedure_executions_evaluated_control_count(result.get('computedly_witnessed_control_count'), result.get('asserted_only_control_count'))
    except Exception as _field_exc:
        result['evaluated_control_count'] = None
        result.setdefault('_erb_errors', {})['evaluated_control_count'] = str(_field_exc)
    try:
        result['has_any_independent_observation'] = calc_procedure_executions_has_any_independent_observation(result.get('independent_human_observation_count'))
    except Exception as _field_exc:
        result['has_any_independent_observation'] = None
        result.setdefault('_erb_errors', {})['has_any_independent_observation'] = str(_field_exc)
    try:
        result['has_been_attested'] = calc_procedure_executions_has_been_attested(result.get('attestation_count'))
    except Exception as _field_exc:
        result['has_been_attested'] = None
        result.setdefault('_erb_errors', {})['has_been_attested'] = str(_field_exc)
    try:
        result['delivery_yield_percent'] = calc_procedure_executions_delivery_yield_percent(result.get('intended_recipient_count'), result.get('reached_recipient_count'))
    except Exception as _field_exc:
        result['delivery_yield_percent'] = None
        result.setdefault('_erb_errors', {})['delivery_yield_percent'] = str(_field_exc)
    try:
        result['campaign_silently_lost_audience'] = calc_procedure_executions_campaign_silently_lost_audience(result.get('silently_dropped_count'))
    except Exception as _field_exc:
        result['campaign_silently_lost_audience'] = None
        result.setdefault('_erb_errors', {})['campaign_silently_lost_audience'] = str(_field_exc)
    try:
        result['has_unrecorded_refusals'] = calc_procedure_executions_has_unrecorded_refusals(result.get('unrecorded_refusal_count'))
    except Exception as _field_exc:
        result['has_unrecorded_refusals'] = None
        result.setdefault('_erb_errors', {})['has_unrecorded_refusals'] = str(_field_exc)
    try:
        result['send_decisions_are_entirely_self_witnessed'] = calc_procedure_executions_send_decisions_are_entirely_self_witnessed(result.get('intended_recipient_count'), result.get('independently_confirmed_intent_count'))
    except Exception as _field_exc:
        result['send_decisions_are_entirely_self_witnessed'] = None
        result.setdefault('_erb_errors', {})['send_decisions_are_entirely_self_witnessed'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['diverged_from_specification'] = calc_procedure_executions_diverged_from_specification(result.get('is_structurally_complete'), result.get('control_breach_count'))
    except Exception as _field_exc:
        result['diverged_from_specification'] = None
        result.setdefault('_erb_errors', {})['diverged_from_specification'] = str(_field_exc)
    try:
        result['delivered_without_approval'] = calc_procedure_executions_delivered_without_approval(result.get('has_delivered'), result.get('has_human_approval'))
    except Exception as _field_exc:
        result['delivered_without_approval'] = None
        result.setdefault('_erb_errors', {})['delivered_without_approval'] = str(_field_exc)
    try:
        result['separation_held_under_test'] = calc_procedure_executions_separation_held_under_test(result.get('separation_was_testable'), result.get('separation_of_duties_held'))
    except Exception as _field_exc:
        result['separation_held_under_test'] = None
        result.setdefault('_erb_errors', {})['separation_held_under_test'] = str(_field_exc)
    try:
        result['separation_is_vacuously_green'] = calc_procedure_executions_separation_is_vacuously_green(result.get('separation_of_duties_held'), result.get('separation_was_testable'))
    except Exception as _field_exc:
        result['separation_is_vacuously_green'] = None
        result.setdefault('_erb_errors', {})['separation_is_vacuously_green'] = str(_field_exc)
    try:
        result['computed_assurance_ratio'] = calc_procedure_executions_computed_assurance_ratio(result.get('evaluated_control_count'), result.get('computedly_witnessed_control_count'))
    except Exception as _field_exc:
        result['computed_assurance_ratio'] = None
        result.setdefault('_erb_errors', {})['computed_assurance_ratio'] = str(_field_exc)
    try:
        result['assurance_chain_is_circular'] = calc_procedure_executions_assurance_chain_is_circular(result.get('self_attested_approval_count'), result.get('has_any_independent_observation'))
    except Exception as _field_exc:
        result['assurance_chain_is_circular'] = None
        result.setdefault('_erb_errors', {})['assurance_chain_is_circular'] = str(_field_exc)
    try:
        result['basis_changed_after_signature'] = calc_procedure_executions_basis_changed_after_signature(result.get('has_been_attested'), result.get('post_attestation_score_count'))
    except Exception as _field_exc:
        result['basis_changed_after_signature'] = None
        result.setdefault('_erb_errors', {})['basis_changed_after_signature'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_attestation_ready'] = calc_procedure_executions_is_attestation_ready(result.get('is_structurally_complete'), result.get('diverged_from_specification'), result.get('all_blocking_controls_evaluated'), result.get('separation_of_duties_held'))
    except Exception as _field_exc:
        result['is_attestation_ready'] = None
        result.setdefault('_erb_errors', {})['is_attestation_ready'] = str(_field_exc)
    try:
        result['separation_assurance_note'] = calc_procedure_executions_separation_assurance_note(result.get('separation_violation_count'), result.get('separation_is_vacuously_green'))
    except Exception as _field_exc:
        result['separation_assurance_note'] = None
        result.setdefault('_erb_errors', {})['separation_assurance_note'] = str(_field_exc)
    try:
        result['divergence_was_fully_governed'] = calc_procedure_executions_divergence_was_fully_governed(result.get('diverged_from_specification'), result.get('ungoverned_divergence_count'))
    except Exception as _field_exc:
        result['divergence_was_fully_governed'] = None
        result.setdefault('_erb_errors', {})['divergence_was_fully_governed'] = str(_field_exc)
    try:
        result['assurance_grade'] = calc_procedure_executions_assurance_grade(result.get('evaluated_control_count'), result.get('interested_party_assertion_count'), result.get('computed_assurance_ratio'))
    except Exception as _field_exc:
        result['assurance_grade'] = None
        result.setdefault('_erb_errors', {})['assurance_grade'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['attestation_blocker_summary'] = calc_procedure_executions_attestation_blocker_summary(result.get('is_attestation_ready'), result.get('is_structurally_complete'), result.get('separation_violation_count'), result.get('unevaluated_blocking_total'))
    except Exception as _field_exc:
        result['attestation_blocker_summary'] = None
        result.setdefault('_erb_errors', {})['attestation_blocker_summary'] = str(_field_exc)
    try:
        result['attestation_would_be_weakly_based'] = calc_procedure_executions_attestation_would_be_weakly_based(result.get('is_attestation_ready'), result.get('interested_party_assertion_count'), result.get('computed_assurance_ratio'))
    except Exception as _field_exc:
        result['attestation_would_be_weakly_based'] = None
        result.setdefault('_erb_errors', {})['attestation_would_be_weakly_based'] = str(_field_exc)
    try:
        result['requires_re_attestation'] = calc_procedure_executions_requires_re_attestation(result.get('basis_changed_after_signature'), result.get('is_attestation_ready'))
    except Exception as _field_exc:
        result['requires_re_attestation'] = None
        result.setdefault('_erb_errors', {})['requires_re_attestation'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'attestation_blocker_summary', 'separation_assurance_note', 'assurance_grade']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEPEXECUTIONS
# Concrete executions of specified steps. Maps to pko:StepExecution, hasExecutedStep, includesStepExecution, and nextStepExecution.
# =============================================================================

# Level 1

def calc_step_executions_name(procedure_execution, step):
    """
    Human-readable calculated display alias for the StepExecutions row.
    
    Formula: ={{ProcedureExecution}} & " / " & {{Step}}
    """
    return (str(procedure_execution or "") + ' / ' + str(step or ""))

def calc_step_executions_actual_duration_minutes(ended_at, started_at):
    """
    Observed duration in minutes.
    
    Formula: =IF({{EndedAt}} = "", 0, DATETIME_DIFF({{EndedAt}}, {{StartedAt}}, "minutes"))
    """
    return _erb.erb_integer((0 if _erb.erb_bool3((ended_at is None or ended_at == "")) else _erb.erb_datetime_diff(ended_at, started_at, 'minutes')))

def calc_step_executions_proceeded_past_blocking_control(execution_status, blocking_unmet_count_safe):
    """
    TRUE when a step execution reached Completed even though a blocking requirement on it was never fully satisfied.
    
    Formula: =AND({{ExecutionStatus}} = "Completed", {{BlockingUnmetCountSafe}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(execution_status), 'Completed')), _erb.erb_bool3(_erb.erb_cmp(blocking_unmet_count_safe, '>', 0)))

def calc_step_executions_unevaluated_blocking_count(expected_blocking_count, evaluated_blocking_count):
    """
    Blocking requirements that were bound to this step but never assessed on this execution.
    
    Formula: ={{ExpectedBlockingCount}} - {{EvaluatedBlockingCount}}
    """
    return _erb.erb_sub(expected_blocking_count, evaluated_blocking_count)

def calc_step_executions_ran_on_stale_authoritative_source(stale_authoritative_source_count):
    """
    TRUE when this execution's step depends on at least one authoritative source that is outside its freshness SLA.
    
    Formula: ={{StaleAuthoritativeSourceCount}} > 0
    """
    return _erb.erb_cmp(stale_authoritative_source_count, '>', 0)

def calc_step_executions_has_deviation_note(deviation):
    """
    TRUE when a human recorded some deviation narrative on this execution.
    
    Formula: ={{Deviation}} <> "" 
    """
    return (not (deviation is None or deviation == ""))

def calc_step_executions_skipped_verification_count(expected_verification_count, performed_verification_count):
    """
    Declared verifications with no recorded outcome on this execution.
    
    Formula: ={{ExpectedVerificationCount}} - {{PerformedVerificationCount}}
    """
    return _erb.erb_sub(expected_verification_count, performed_verification_count)

def calc_step_executions_preparer_agent_key(step_is_preparation, procedure_execution, executed_by_agent):
    """
    Composite execution+agent key, emitted only for preparation steps.
    
    Formula: =IF({{StepIsPreparation}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, "")
    """
    return ((str(procedure_execution or "") + '|' + str(executed_by_agent or "")) if _erb.erb_bool3(step_is_preparation) else '')

def calc_step_executions_approver_agent_key(step_is_approval, procedure_execution, executed_by_agent):
    """
    Composite execution+agent key, emitted only for approval steps.
    
    Formula: =IF({{StepIsApproval}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, "")
    """
    return ((str(procedure_execution or "") + '|' + str(executed_by_agent or "")) if _erb.erb_bool3(step_is_approval) else '')

def calc_step_executions_violates_separation_of_duties(step_is_approval, prepared_by_this_agent_count):
    """
    TRUE when the agent approving this step also prepared work earlier in the same procedure execution.
    
    Formula: =AND({{StepIsApproval}}, {{PreparedByThisAgentCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(step_is_approval), _erb.erb_bool3(_erb.erb_cmp(prepared_by_this_agent_count, '>', 0)))

def calc_step_executions_executor_role_key(executed_by_agent, required_role_for_step):
    """
    The agent+role pair that would need to exist as a valid assignment for this execution to be properly authorized.
    
    Formula: ={{ExecutedByAgent}} & "|" & {{RequiredRoleForStep}}
    """
    return (str(executed_by_agent or "") + '|' + str(required_role_for_step or ""))

def calc_step_executions_executor_held_required_role(executor_authority_count):
    """
    TRUE when the executing agent holds a currently-valid assignment to the role the step required.
    
    Formula: ={{ExecutorAuthorityCount}} > 0
    """
    return _erb.erb_cmp(executor_authority_count, '>', 0)

def calc_step_executions_completed_execution_key(execution_status, procedure_execution):
    """
    Echoes the parent execution id only for completed steps.
    
    Formula: =IF({{ExecutionStatus}} = "Completed", {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(execution_status), 'Completed')) else '')

def calc_step_executions_executor_is_human(executor_agent_kind):
    """
    TRUE when a human executed this step.
    
    Formula: ={{ExecutorAgentKind}} = "Human" 
    """
    return _erb.erb_eq(executor_agent_kind, 'Human')

def calc_step_executions_ran_under_exception(exception_invocation_count):
    """
    TRUE when this execution invoked at least one specified exception.
    
    Formula: ={{ExceptionInvocationCount}} > 0
    """
    return _erb.erb_cmp(exception_invocation_count, '>', 0)

def calc_step_executions_is_completed(execution_status):
    """
    TRUE when this step execution reached a completed state.
    
    Formula: ={{ExecutionStatus}} = "Completed" 
    """
    return _erb.erb_eq(_erb.erb_nullif(execution_status), 'Completed')

def calc_step_executions_is_verification_passed(verification_result):
    """
    TRUE only when this step execution's verification actually passed. PENDING and FAIL are both not-passed.
    
    Formula: ={{VerificationResult}} = "PASS" 
    """
    return _erb.erb_eq(_erb.erb_nullif(verification_result), 'PASS')

def calc_step_executions_is_legal_review_step(step):
    """
    TRUE for executions of the legal and privacy review step.
    
    Formula: ={{Step}} = "policy-04" 
    """
    return _erb.erb_eq(_erb.erb_nullif(step), 'policy-04')

def calc_step_executions_executor_is_designated_agent(executed_by_agent, role_current_agent):
    """
    TRUE when the agent that executed the step is the agent currently designated for the step's role.
    
    Formula: ={{ExecutedByAgent}} = {{RoleCurrentAgent}}
    """
    return _erb.erb_eq(_erb.erb_nullif(executed_by_agent), role_current_agent)

def calc_step_executions_ran_on_stale_inputs(execution_status, inputs_were_fresh_at_run):
    """
    TRUE when an execution completed even though its step's authoritative inputs are stale.
    
    Formula: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereFreshAtRun}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(execution_status), 'Completed')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(inputs_were_fresh_at_run))))

def calc_step_executions_has_deviation(deviation):
    """
    TRUE when a deviation from the specification was recorded.
    
    Formula: ={{Deviation}} <> "" 
    """
    return (not (deviation is None or deviation == ""))

def calc_step_executions_has_unevaluated_blocking_requirement(evaluated_requirement_count, required_blocking_count):
    """
    TRUE when fewer requirements were scored than the step has blocking requirements.
    
    Formula: ={{EvaluatedRequirementCount}} < {{RequiredBlockingCount}}
    """
    return _erb.erb_cmp(evaluated_requirement_count, '<', required_blocking_count)

def calc_step_executions_was_executed_by_software(executing_agent_kind):
    """
    TRUE when software actually performed this step.
    
    Formula: =OR({{ExecutingAgentKind}} = "AIAgent", {{ExecutingAgentKind}} = "AutomatedPipeline")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(executing_agent_kind, 'AIAgent')), _erb.erb_bool3(_erb.erb_eq(executing_agent_kind, 'AutomatedPipeline')))

def calc_step_executions_is_verified(verification_result):
    """
    TRUE when this execution recorded a positive verification outcome.
    
    Formula: =AND({{VerificationResult}} <> "", {{VerificationResult}} <> "PENDING", {{VerificationResult}} <> "FAIL")
    """
    return _erb.erb_and(_erb.erb_bool3((not (verification_result is None or verification_result == ""))), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(verification_result), 'PENDING')), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(verification_result), 'FAIL')))

def calc_step_executions_human_confirmation_missing(requires_human_confirmation, unconfirmed_non_human_decision_count):
    """
    TRUE when a step requiring human confirmation contains unconfirmed material non-human decisions.
    
    Formula: =AND({{RequiresHumanConfirmation}}, {{UnconfirmedNonHumanDecisionCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(requires_human_confirmation), _erb.erb_bool3(_erb.erb_cmp(unconfirmed_non_human_decision_count, '>', 0)))

def calc_step_executions_drafted_from_unusable_source(execution_status, inputs_were_usable):
    """
    TRUE when a drafting execution completed despite an unapproved or stale source at its step.
    
    Formula: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereUsable}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(execution_status), 'Completed')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(inputs_were_usable))))

def calc_step_executions_all_clearances_are_unfalsified(evaluated_blocking_count, unfalsified_clearance_count):
    """
    TRUE when every blocking control evaluated on this step is one that has never returned a negative result.
    
    Formula: =AND({{EvaluatedBlockingCount}} > 0, {{UnfalsifiedClearanceCount}} >= {{EvaluatedBlockingCount}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(evaluated_blocking_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(unfalsified_clearance_count, '>=', evaluated_blocking_count)))

def calc_step_executions_was_stale_when_i_ran_it(stale_at_run_count):
    """
    TRUE when this execution consumed at least one authoritative source that was stale at the time of reading.
    
    Formula: ={{StaleAtRunCount}} > 0
    """
    return _erb.erb_cmp(stale_at_run_count, '>', 0)

def calc_step_executions_has_any_declared_check(expected_verification_count, expected_blocking_count):
    """
    TRUE when the specification declared at least one verification or one blocking requirement for this step.
    
    Formula: =OR({{ExpectedVerificationCount}} > 0, {{ExpectedBlockingCount}} > 0)
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(expected_verification_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(expected_blocking_count, '>', 0)))

def calc_step_executions_performed_check_count(performed_verification_count, evaluated_blocking_count):
    """
    Total number of checks actually carried out on this execution, verifications plus blocking-control evaluations.
    
    Formula: ={{PerformedVerificationCount}} + {{EvaluatedBlockingCount}}
    """
    return _erb.erb_add(performed_verification_count, evaluated_blocking_count)

def calc_step_executions_declared_check_count(expected_verification_count, expected_blocking_count):
    """
    Total number of checks the specification called for on this step.
    
    Formula: ={{ExpectedVerificationCount}} + {{ExpectedBlockingCount}}
    """
    return _erb.erb_add(expected_verification_count, expected_blocking_count)

def calc_step_executions_evidence_position_is_weak(performed_verification_count, uncorroborated_pass_count):
    """
    TRUE when every verification performed on this step was an uncorroborated self-witnessed pass.
    
    Formula: =AND({{PerformedVerificationCount}} > 0, {{UncorroboratedPassCount}} >= {{PerformedVerificationCount}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(performed_verification_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(uncorroborated_pass_count, '>=', performed_verification_count)))

def calc_step_executions_preparation_execution_key(step_is_preparation, procedure_execution):
    """
    Echoes the parent execution id when this step execution is a preparation step.
    
    Formula: =IF({{StepIsPreparation}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(step_is_preparation) else '')

def calc_step_executions_approval_execution_key(step_is_approval, procedure_execution):
    """
    Echoes the parent execution id when this step execution is an approval step.
    
    Formula: =IF({{StepIsApproval}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(step_is_approval) else '')

# Level 2

def calc_step_executions_is_late(actual_duration_minutes, expected_duration_minutes):
    """
    TRUE when actual duration exceeds expected duration.
    
    Formula: ={{ActualDurationMinutes}} > {{ExpectedDurationMinutes}}
    """
    return _erb.erb_cmp(actual_duration_minutes, '>', expected_duration_minutes)

def calc_step_executions_has_unevaluated_blocking_control(unevaluated_blocking_count):
    """
    TRUE when at least one blocking control bound to this step was never evaluated on this execution.
    
    Formula: ={{UnevaluatedBlockingCount}} > 0
    """
    return _erb.erb_cmp(unevaluated_blocking_count, '>', 0)

def calc_step_executions_has_skipped_verification(skipped_verification_count):
    """
    TRUE when a declared verification was never performed on this execution.
    
    Formula: ={{SkippedVerificationCount}} > 0
    """
    return _erb.erb_cmp(skipped_verification_count, '>', 0)

def calc_step_executions_is_unauthorized_approval(step_is_approval, executor_held_required_role):
    """
    TRUE when an approval step was executed by an agent who does not hold the required approving role.
    
    Formula: =AND({{StepIsApproval}}, NOT({{ExecutorHeldRequiredRole}}))
    """
    return _erb.erb_and(_erb.erb_bool3(step_is_approval), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(executor_held_required_role))))

def calc_step_executions_non_human_ran_human_step(step_requires_human_confirmation, executor_is_human):
    """
    TRUE when a step requiring human confirmation was executed by an AI agent or automated pipeline.
    
    Formula: =AND({{StepRequiresHumanConfirmation}}, NOT({{ExecutorIsHuman}}))
    """
    return _erb.erb_and(_erb.erb_bool3(step_requires_human_confirmation), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(executor_is_human))))

def calc_step_executions_non_human_approval(step_is_approval, executor_is_human):
    """
    TRUE when an approval-authority step was executed by a non-human agent, regardless of the RequiresHumanConfirmation flag.
    
    Formula: =AND({{StepIsApproval}}, NOT({{ExecutorIsHuman}}))
    """
    return _erb.erb_and(_erb.erb_bool3(step_is_approval), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(executor_is_human))))

def calc_step_executions_separation_violation_execution_key(violates_separation_of_duties, procedure_execution):
    """
    Echoes the parent execution id when this step execution violates segregation of duties.
    
    Formula: =IF({{ViolatesSeparationOfDuties}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(violates_separation_of_duties) else '')

def calc_step_executions_cleared_legal_review_key(is_legal_review_step, is_verification_passed, procedure_execution):
    """
    Carries the execution id only when this row is a PASSED legal review; empty string otherwise.
    
    Formula: =IF(AND({{IsLegalReviewStep}}, {{IsVerificationPassed}}), {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_legal_review_step), _erb.erb_bool3(is_verification_passed))) else '')

def calc_step_executions_software_did_human_work(was_executed_by_software, step_is_software_assigned):
    """
    TRUE when software performed a step the procedure specified for a human.
    
    Formula: =AND({{WasExecutedBySoftware}}, NOT({{StepIsSoftwareAssigned}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_executed_by_software), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(step_is_software_assigned))))

def calc_step_executions_software_execution_step_key(was_executed_by_software, step):
    """
    Composite-key echo: the step this execution ran when it was carried out by software, blank otherwise.
    
    Formula: =IF({{WasExecutedBySoftware}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(was_executed_by_software) else '')

def calc_step_executions_staleness_answer_is_tense_dependent(was_stale_when_i_ran_it, ran_on_stale_authoritative_source):
    """
    TRUE when the as-of-now staleness verdict disagrees with the as-of-run verdict for the same step.
    
    Formula: =NOT({{WasStaleWhenIRanIt}} = {{RanOnStaleAuthoritativeSource}})
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(was_stale_when_i_ran_it, ran_on_stale_authoritative_source)))

def calc_step_executions_is_unchecked_by_design(declared_check_count):
    """
    TRUE when the specification asked for no verification and no blocking control on this step at all.
    
    Formula: ={{DeclaredCheckCount}} = 0
    """
    return _erb.erb_eq(declared_check_count, 0)

def calc_step_executions_has_governing_instrument(ran_under_exception, has_approved_change_coverage):
    """
    TRUE when this step's departure from spec is covered by an invoked exception or an approved change request against its procedure version.
    
    Formula: =OR({{RanUnderException}}, {{HasApprovedChangeCoverage}})
    """
    return _erb.erb_or(_erb.erb_bool3(ran_under_exception), _erb.erb_bool3(has_approved_change_coverage))

# Level 3

def calc_step_executions_is_late_and_unexplained(is_late, has_deviation_note):
    """
    TRUE when the execution exceeded its expected duration and no deviation was recorded.
    
    Formula: =AND({{IsLate}}, NOT({{HasDeviationNote}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_late), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_deviation_note))))

def calc_step_executions_claims_pass_without_evidence(verification_result, has_skipped_verification):
    """
    TRUE when an execution asserts PASS while at least one declared verification has no recorded outcome.
    
    Formula: =AND({{VerificationResult}} = "PASS", {{HasSkippedVerification}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(verification_result), 'PASS')), _erb.erb_bool3(has_skipped_verification))

def calc_step_executions_late_execution_key(is_late, procedure_execution):
    """
    Echoes the parent execution id only for steps that ran past their expected duration.
    
    Formula: =IF({{IsLate}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_late) else '')

def calc_step_executions_unevaluated_blocking_execution_key(has_unevaluated_blocking_control, procedure_execution):
    """
    Echoes the parent execution id when this step left a blocking control unevaluated.
    
    Formula: =IF({{HasUnevaluatedBlockingControl}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(has_unevaluated_blocking_control) else '')

def calc_step_executions_approval_rests_on_self_attestation(step_is_approval, self_witnessed_verification_count, has_skipped_verification):
    """
    TRUE when an approval step's verification was either self-witnessed by the approver or never performed at all.
    
    Formula: =AND({{StepIsApproval}}, OR({{SelfWitnessedVerificationCount}} > 0, {{HasSkippedVerification}}))
    """
    return _erb.erb_and(_erb.erb_bool3(step_is_approval), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(self_witnessed_verification_count, '>', 0)), _erb.erb_bool3(has_skipped_verification))))

def calc_step_executions_is_clean(verification_result, has_deviation, unresolved_issue_count, is_late):
    """
    TRUE when this execution passed verification, deviated from nothing, left no open issue, and finished on time.
    
    Formula: =AND({{VerificationResult}} = "PASS", NOT({{HasDeviation}}), {{UnresolvedIssueCount}} = 0, NOT({{IsLate}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(verification_result), 'PASS')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_deviation))), _erb.erb_bool3(_erb.erb_eq(unresolved_issue_count, 0)), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_late))))

def calc_step_executions_is_ungoverned_divergence(has_deviation, is_late, proceeded_past_blocking_control, has_governing_instrument):
    """
    TRUE when this step departed from specification and no exception or approved change covers it.
    
    Formula: =AND(OR({{HasDeviation}}, {{IsLate}}, {{ProceededPastBlockingControl}}), NOT({{HasGoverningInstrument}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(has_deviation), _erb.erb_bool3(is_late), _erb.erb_bool3(proceeded_past_blocking_control))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_governing_instrument))))

# Level 4

def calc_step_executions_had_uninvoked_exception_available(is_late_and_unexplained, available_exception_count_for_step):
    """
    TRUE when an execution ran long with no explanation despite the specification defining an active exception for exactly that situation.
    
    Formula: =AND({{IsLateAndUnexplained}}, {{AvailableExceptionCountForStep}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_late_and_unexplained), _erb.erb_bool3(_erb.erb_cmp(available_exception_count_for_step, '>', 0)))

def calc_step_executions_control_breach_execution_key(proceeded_past_blocking_control, violates_separation_of_duties, is_unauthorized_approval, claims_pass_without_evidence, procedure_execution):
    """
    Echoes the parent execution id when this step execution carries ANY control breach.
    
    Formula: =IF(OR({{ProceededPastBlockingControl}}, {{ViolatesSeparationOfDuties}}, {{IsUnauthorizedApproval}}, {{ClaimsPassWithoutEvidence}}), {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(proceeded_past_blocking_control), _erb.erb_bool3(violates_separation_of_duties), _erb.erb_bool3(is_unauthorized_approval), _erb.erb_bool3(claims_pass_without_evidence))) else '')

def calc_step_executions_procedure_execution_when_unclean(is_clean, procedure_execution):
    """
    Echoes the parent execution id when this step execution is not clean, blank otherwise.
    
    Formula: =IF({{IsClean}}, "", {{ProcedureExecution}})
    """
    return ('' if _erb.erb_bool3(is_clean) else procedure_execution)

def calc_step_executions_is_vacuously_clean(is_clean, is_unchecked_by_design):
    """
    TRUE when this execution reads clean and nothing was ever declared that could have made it read otherwise.
    
    Formula: =AND({{IsClean}}, {{IsUncheckedByDesign}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_clean), _erb.erb_bool3(is_unchecked_by_design))

def calc_step_executions_is_substantively_clean(is_clean, performed_check_count, declared_check_count):
    """
    TRUE when this execution is clean AND every declared check was actually performed.
    
    Formula: =AND({{IsClean}}, {{PerformedCheckCount}} >= {{DeclaredCheckCount}}, {{DeclaredCheckCount}} > 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_clean), _erb.erb_bool3(_erb.erb_cmp(performed_check_count, '>=', declared_check_count)), _erb.erb_bool3(_erb.erb_cmp(declared_check_count, '>', 0)))

def calc_step_executions_ungoverned_divergence_execution_key(is_ungoverned_divergence, procedure_execution):
    """
    Echoes the parent execution id when this step diverged without governance.
    
    Formula: =IF({{IsUngovernedDivergence}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_ungoverned_divergence) else '')

def calc_step_executions_self_attested_approval_execution_key(approval_rests_on_self_attestation, procedure_execution):
    """
    Echoes the parent execution id when this approval rested on self-attestation.
    
    Formula: =IF({{ApprovalRestsOnSelfAttestation}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(approval_rests_on_self_attestation) else '')

# Level 5

def calc_step_executions_vacuously_clean_execution_key(is_vacuously_clean, procedure_execution):
    """
    Echoes the parent execution id when this step is vacuously clean.
    
    Formula: =IF({{IsVacuouslyClean}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_vacuously_clean) else '')


def compute_step_executions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StepExecutions.
    
    Concrete executions of specified steps. Maps to pko:StepExecution, hasExecutedStep, includesStepExecution, and nextStepExecution.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_step_executions_name(result.get('procedure_execution'), result.get('step'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['actual_duration_minutes'] = calc_step_executions_actual_duration_minutes(result.get('ended_at'), result.get('started_at'))
    except Exception as _field_exc:
        result['actual_duration_minutes'] = None
        result.setdefault('_erb_errors', {})['actual_duration_minutes'] = str(_field_exc)
    try:
        result['proceeded_past_blocking_control'] = calc_step_executions_proceeded_past_blocking_control(result.get('execution_status'), result.get('blocking_unmet_count_safe'))
    except Exception as _field_exc:
        result['proceeded_past_blocking_control'] = None
        result.setdefault('_erb_errors', {})['proceeded_past_blocking_control'] = str(_field_exc)
    try:
        result['unevaluated_blocking_count'] = calc_step_executions_unevaluated_blocking_count(result.get('expected_blocking_count'), result.get('evaluated_blocking_count'))
    except Exception as _field_exc:
        result['unevaluated_blocking_count'] = None
        result.setdefault('_erb_errors', {})['unevaluated_blocking_count'] = str(_field_exc)
    try:
        result['ran_on_stale_authoritative_source'] = calc_step_executions_ran_on_stale_authoritative_source(result.get('stale_authoritative_source_count'))
    except Exception as _field_exc:
        result['ran_on_stale_authoritative_source'] = None
        result.setdefault('_erb_errors', {})['ran_on_stale_authoritative_source'] = str(_field_exc)
    try:
        result['has_deviation_note'] = calc_step_executions_has_deviation_note(result.get('deviation'))
    except Exception as _field_exc:
        result['has_deviation_note'] = None
        result.setdefault('_erb_errors', {})['has_deviation_note'] = str(_field_exc)
    try:
        result['skipped_verification_count'] = calc_step_executions_skipped_verification_count(result.get('expected_verification_count'), result.get('performed_verification_count'))
    except Exception as _field_exc:
        result['skipped_verification_count'] = None
        result.setdefault('_erb_errors', {})['skipped_verification_count'] = str(_field_exc)
    try:
        result['preparer_agent_key'] = calc_step_executions_preparer_agent_key(result.get('step_is_preparation'), result.get('procedure_execution'), result.get('executed_by_agent'))
    except Exception as _field_exc:
        result['preparer_agent_key'] = None
        result.setdefault('_erb_errors', {})['preparer_agent_key'] = str(_field_exc)
    try:
        result['approver_agent_key'] = calc_step_executions_approver_agent_key(result.get('step_is_approval'), result.get('procedure_execution'), result.get('executed_by_agent'))
    except Exception as _field_exc:
        result['approver_agent_key'] = None
        result.setdefault('_erb_errors', {})['approver_agent_key'] = str(_field_exc)
    try:
        result['violates_separation_of_duties'] = calc_step_executions_violates_separation_of_duties(result.get('step_is_approval'), result.get('prepared_by_this_agent_count'))
    except Exception as _field_exc:
        result['violates_separation_of_duties'] = None
        result.setdefault('_erb_errors', {})['violates_separation_of_duties'] = str(_field_exc)
    try:
        result['executor_role_key'] = calc_step_executions_executor_role_key(result.get('executed_by_agent'), result.get('required_role_for_step'))
    except Exception as _field_exc:
        result['executor_role_key'] = None
        result.setdefault('_erb_errors', {})['executor_role_key'] = str(_field_exc)
    try:
        result['executor_held_required_role'] = calc_step_executions_executor_held_required_role(result.get('executor_authority_count'))
    except Exception as _field_exc:
        result['executor_held_required_role'] = None
        result.setdefault('_erb_errors', {})['executor_held_required_role'] = str(_field_exc)
    try:
        result['completed_execution_key'] = calc_step_executions_completed_execution_key(result.get('execution_status'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['completed_execution_key'] = None
        result.setdefault('_erb_errors', {})['completed_execution_key'] = str(_field_exc)
    try:
        result['executor_is_human'] = calc_step_executions_executor_is_human(result.get('executor_agent_kind'))
    except Exception as _field_exc:
        result['executor_is_human'] = None
        result.setdefault('_erb_errors', {})['executor_is_human'] = str(_field_exc)
    try:
        result['ran_under_exception'] = calc_step_executions_ran_under_exception(result.get('exception_invocation_count'))
    except Exception as _field_exc:
        result['ran_under_exception'] = None
        result.setdefault('_erb_errors', {})['ran_under_exception'] = str(_field_exc)
    try:
        result['is_completed'] = calc_step_executions_is_completed(result.get('execution_status'))
    except Exception as _field_exc:
        result['is_completed'] = None
        result.setdefault('_erb_errors', {})['is_completed'] = str(_field_exc)
    try:
        result['is_verification_passed'] = calc_step_executions_is_verification_passed(result.get('verification_result'))
    except Exception as _field_exc:
        result['is_verification_passed'] = None
        result.setdefault('_erb_errors', {})['is_verification_passed'] = str(_field_exc)
    try:
        result['is_legal_review_step'] = calc_step_executions_is_legal_review_step(result.get('step'))
    except Exception as _field_exc:
        result['is_legal_review_step'] = None
        result.setdefault('_erb_errors', {})['is_legal_review_step'] = str(_field_exc)
    try:
        result['executor_is_designated_agent'] = calc_step_executions_executor_is_designated_agent(result.get('executed_by_agent'), result.get('role_current_agent'))
    except Exception as _field_exc:
        result['executor_is_designated_agent'] = None
        result.setdefault('_erb_errors', {})['executor_is_designated_agent'] = str(_field_exc)
    try:
        result['ran_on_stale_inputs'] = calc_step_executions_ran_on_stale_inputs(result.get('execution_status'), result.get('inputs_were_fresh_at_run'))
    except Exception as _field_exc:
        result['ran_on_stale_inputs'] = None
        result.setdefault('_erb_errors', {})['ran_on_stale_inputs'] = str(_field_exc)
    try:
        result['has_deviation'] = calc_step_executions_has_deviation(result.get('deviation'))
    except Exception as _field_exc:
        result['has_deviation'] = None
        result.setdefault('_erb_errors', {})['has_deviation'] = str(_field_exc)
    try:
        result['has_unevaluated_blocking_requirement'] = calc_step_executions_has_unevaluated_blocking_requirement(result.get('evaluated_requirement_count'), result.get('required_blocking_count'))
    except Exception as _field_exc:
        result['has_unevaluated_blocking_requirement'] = None
        result.setdefault('_erb_errors', {})['has_unevaluated_blocking_requirement'] = str(_field_exc)
    try:
        result['was_executed_by_software'] = calc_step_executions_was_executed_by_software(result.get('executing_agent_kind'))
    except Exception as _field_exc:
        result['was_executed_by_software'] = None
        result.setdefault('_erb_errors', {})['was_executed_by_software'] = str(_field_exc)
    try:
        result['is_verified'] = calc_step_executions_is_verified(result.get('verification_result'))
    except Exception as _field_exc:
        result['is_verified'] = None
        result.setdefault('_erb_errors', {})['is_verified'] = str(_field_exc)
    try:
        result['human_confirmation_missing'] = calc_step_executions_human_confirmation_missing(result.get('requires_human_confirmation'), result.get('unconfirmed_non_human_decision_count'))
    except Exception as _field_exc:
        result['human_confirmation_missing'] = None
        result.setdefault('_erb_errors', {})['human_confirmation_missing'] = str(_field_exc)
    try:
        result['drafted_from_unusable_source'] = calc_step_executions_drafted_from_unusable_source(result.get('execution_status'), result.get('inputs_were_usable'))
    except Exception as _field_exc:
        result['drafted_from_unusable_source'] = None
        result.setdefault('_erb_errors', {})['drafted_from_unusable_source'] = str(_field_exc)
    try:
        result['all_clearances_are_unfalsified'] = calc_step_executions_all_clearances_are_unfalsified(result.get('evaluated_blocking_count'), result.get('unfalsified_clearance_count'))
    except Exception as _field_exc:
        result['all_clearances_are_unfalsified'] = None
        result.setdefault('_erb_errors', {})['all_clearances_are_unfalsified'] = str(_field_exc)
    try:
        result['was_stale_when_i_ran_it'] = calc_step_executions_was_stale_when_i_ran_it(result.get('stale_at_run_count'))
    except Exception as _field_exc:
        result['was_stale_when_i_ran_it'] = None
        result.setdefault('_erb_errors', {})['was_stale_when_i_ran_it'] = str(_field_exc)
    try:
        result['has_any_declared_check'] = calc_step_executions_has_any_declared_check(result.get('expected_verification_count'), result.get('expected_blocking_count'))
    except Exception as _field_exc:
        result['has_any_declared_check'] = None
        result.setdefault('_erb_errors', {})['has_any_declared_check'] = str(_field_exc)
    try:
        result['performed_check_count'] = calc_step_executions_performed_check_count(result.get('performed_verification_count'), result.get('evaluated_blocking_count'))
    except Exception as _field_exc:
        result['performed_check_count'] = None
        result.setdefault('_erb_errors', {})['performed_check_count'] = str(_field_exc)
    try:
        result['declared_check_count'] = calc_step_executions_declared_check_count(result.get('expected_verification_count'), result.get('expected_blocking_count'))
    except Exception as _field_exc:
        result['declared_check_count'] = None
        result.setdefault('_erb_errors', {})['declared_check_count'] = str(_field_exc)
    try:
        result['evidence_position_is_weak'] = calc_step_executions_evidence_position_is_weak(result.get('performed_verification_count'), result.get('uncorroborated_pass_count'))
    except Exception as _field_exc:
        result['evidence_position_is_weak'] = None
        result.setdefault('_erb_errors', {})['evidence_position_is_weak'] = str(_field_exc)
    try:
        result['preparation_execution_key'] = calc_step_executions_preparation_execution_key(result.get('step_is_preparation'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['preparation_execution_key'] = None
        result.setdefault('_erb_errors', {})['preparation_execution_key'] = str(_field_exc)
    try:
        result['approval_execution_key'] = calc_step_executions_approval_execution_key(result.get('step_is_approval'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['approval_execution_key'] = None
        result.setdefault('_erb_errors', {})['approval_execution_key'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_late'] = calc_step_executions_is_late(result.get('actual_duration_minutes'), result.get('expected_duration_minutes'))
    except Exception as _field_exc:
        result['is_late'] = None
        result.setdefault('_erb_errors', {})['is_late'] = str(_field_exc)
    try:
        result['has_unevaluated_blocking_control'] = calc_step_executions_has_unevaluated_blocking_control(result.get('unevaluated_blocking_count'))
    except Exception as _field_exc:
        result['has_unevaluated_blocking_control'] = None
        result.setdefault('_erb_errors', {})['has_unevaluated_blocking_control'] = str(_field_exc)
    try:
        result['has_skipped_verification'] = calc_step_executions_has_skipped_verification(result.get('skipped_verification_count'))
    except Exception as _field_exc:
        result['has_skipped_verification'] = None
        result.setdefault('_erb_errors', {})['has_skipped_verification'] = str(_field_exc)
    try:
        result['is_unauthorized_approval'] = calc_step_executions_is_unauthorized_approval(result.get('step_is_approval'), result.get('executor_held_required_role'))
    except Exception as _field_exc:
        result['is_unauthorized_approval'] = None
        result.setdefault('_erb_errors', {})['is_unauthorized_approval'] = str(_field_exc)
    try:
        result['non_human_ran_human_step'] = calc_step_executions_non_human_ran_human_step(result.get('step_requires_human_confirmation'), result.get('executor_is_human'))
    except Exception as _field_exc:
        result['non_human_ran_human_step'] = None
        result.setdefault('_erb_errors', {})['non_human_ran_human_step'] = str(_field_exc)
    try:
        result['non_human_approval'] = calc_step_executions_non_human_approval(result.get('step_is_approval'), result.get('executor_is_human'))
    except Exception as _field_exc:
        result['non_human_approval'] = None
        result.setdefault('_erb_errors', {})['non_human_approval'] = str(_field_exc)
    try:
        result['separation_violation_execution_key'] = calc_step_executions_separation_violation_execution_key(result.get('violates_separation_of_duties'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['separation_violation_execution_key'] = None
        result.setdefault('_erb_errors', {})['separation_violation_execution_key'] = str(_field_exc)
    try:
        result['cleared_legal_review_key'] = calc_step_executions_cleared_legal_review_key(result.get('is_legal_review_step'), result.get('is_verification_passed'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['cleared_legal_review_key'] = None
        result.setdefault('_erb_errors', {})['cleared_legal_review_key'] = str(_field_exc)
    try:
        result['software_did_human_work'] = calc_step_executions_software_did_human_work(result.get('was_executed_by_software'), result.get('step_is_software_assigned'))
    except Exception as _field_exc:
        result['software_did_human_work'] = None
        result.setdefault('_erb_errors', {})['software_did_human_work'] = str(_field_exc)
    try:
        result['software_execution_step_key'] = calc_step_executions_software_execution_step_key(result.get('was_executed_by_software'), result.get('step'))
    except Exception as _field_exc:
        result['software_execution_step_key'] = None
        result.setdefault('_erb_errors', {})['software_execution_step_key'] = str(_field_exc)
    try:
        result['staleness_answer_is_tense_dependent'] = calc_step_executions_staleness_answer_is_tense_dependent(result.get('was_stale_when_i_ran_it'), result.get('ran_on_stale_authoritative_source'))
    except Exception as _field_exc:
        result['staleness_answer_is_tense_dependent'] = None
        result.setdefault('_erb_errors', {})['staleness_answer_is_tense_dependent'] = str(_field_exc)
    try:
        result['is_unchecked_by_design'] = calc_step_executions_is_unchecked_by_design(result.get('declared_check_count'))
    except Exception as _field_exc:
        result['is_unchecked_by_design'] = None
        result.setdefault('_erb_errors', {})['is_unchecked_by_design'] = str(_field_exc)
    try:
        result['has_governing_instrument'] = calc_step_executions_has_governing_instrument(result.get('ran_under_exception'), result.get('has_approved_change_coverage'))
    except Exception as _field_exc:
        result['has_governing_instrument'] = None
        result.setdefault('_erb_errors', {})['has_governing_instrument'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_late_and_unexplained'] = calc_step_executions_is_late_and_unexplained(result.get('is_late'), result.get('has_deviation_note'))
    except Exception as _field_exc:
        result['is_late_and_unexplained'] = None
        result.setdefault('_erb_errors', {})['is_late_and_unexplained'] = str(_field_exc)
    try:
        result['claims_pass_without_evidence'] = calc_step_executions_claims_pass_without_evidence(result.get('verification_result'), result.get('has_skipped_verification'))
    except Exception as _field_exc:
        result['claims_pass_without_evidence'] = None
        result.setdefault('_erb_errors', {})['claims_pass_without_evidence'] = str(_field_exc)
    try:
        result['late_execution_key'] = calc_step_executions_late_execution_key(result.get('is_late'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['late_execution_key'] = None
        result.setdefault('_erb_errors', {})['late_execution_key'] = str(_field_exc)
    try:
        result['unevaluated_blocking_execution_key'] = calc_step_executions_unevaluated_blocking_execution_key(result.get('has_unevaluated_blocking_control'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['unevaluated_blocking_execution_key'] = None
        result.setdefault('_erb_errors', {})['unevaluated_blocking_execution_key'] = str(_field_exc)
    try:
        result['approval_rests_on_self_attestation'] = calc_step_executions_approval_rests_on_self_attestation(result.get('step_is_approval'), result.get('self_witnessed_verification_count'), result.get('has_skipped_verification'))
    except Exception as _field_exc:
        result['approval_rests_on_self_attestation'] = None
        result.setdefault('_erb_errors', {})['approval_rests_on_self_attestation'] = str(_field_exc)
    try:
        result['is_clean'] = calc_step_executions_is_clean(result.get('verification_result'), result.get('has_deviation'), result.get('unresolved_issue_count'), result.get('is_late'))
    except Exception as _field_exc:
        result['is_clean'] = None
        result.setdefault('_erb_errors', {})['is_clean'] = str(_field_exc)
    try:
        result['is_ungoverned_divergence'] = calc_step_executions_is_ungoverned_divergence(result.get('has_deviation'), result.get('is_late'), result.get('proceeded_past_blocking_control'), result.get('has_governing_instrument'))
    except Exception as _field_exc:
        result['is_ungoverned_divergence'] = None
        result.setdefault('_erb_errors', {})['is_ungoverned_divergence'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['had_uninvoked_exception_available'] = calc_step_executions_had_uninvoked_exception_available(result.get('is_late_and_unexplained'), result.get('available_exception_count_for_step'))
    except Exception as _field_exc:
        result['had_uninvoked_exception_available'] = None
        result.setdefault('_erb_errors', {})['had_uninvoked_exception_available'] = str(_field_exc)
    try:
        result['control_breach_execution_key'] = calc_step_executions_control_breach_execution_key(result.get('proceeded_past_blocking_control'), result.get('violates_separation_of_duties'), result.get('is_unauthorized_approval'), result.get('claims_pass_without_evidence'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['control_breach_execution_key'] = None
        result.setdefault('_erb_errors', {})['control_breach_execution_key'] = str(_field_exc)
    try:
        result['procedure_execution_when_unclean'] = calc_step_executions_procedure_execution_when_unclean(result.get('is_clean'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['procedure_execution_when_unclean'] = None
        result.setdefault('_erb_errors', {})['procedure_execution_when_unclean'] = str(_field_exc)
    try:
        result['is_vacuously_clean'] = calc_step_executions_is_vacuously_clean(result.get('is_clean'), result.get('is_unchecked_by_design'))
    except Exception as _field_exc:
        result['is_vacuously_clean'] = None
        result.setdefault('_erb_errors', {})['is_vacuously_clean'] = str(_field_exc)
    try:
        result['is_substantively_clean'] = calc_step_executions_is_substantively_clean(result.get('is_clean'), result.get('performed_check_count'), result.get('declared_check_count'))
    except Exception as _field_exc:
        result['is_substantively_clean'] = None
        result.setdefault('_erb_errors', {})['is_substantively_clean'] = str(_field_exc)
    try:
        result['ungoverned_divergence_execution_key'] = calc_step_executions_ungoverned_divergence_execution_key(result.get('is_ungoverned_divergence'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['ungoverned_divergence_execution_key'] = None
        result.setdefault('_erb_errors', {})['ungoverned_divergence_execution_key'] = str(_field_exc)
    try:
        result['self_attested_approval_execution_key'] = calc_step_executions_self_attested_approval_execution_key(result.get('approval_rests_on_self_attestation'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['self_attested_approval_execution_key'] = None
        result.setdefault('_erb_errors', {})['self_attested_approval_execution_key'] = str(_field_exc)

    # Level 5 calculations
    try:
        result['vacuously_clean_execution_key'] = calc_step_executions_vacuously_clean_execution_key(result.get('is_vacuously_clean'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['vacuously_clean_execution_key'] = None
        result.setdefault('_erb_errors', {})['vacuously_clean_execution_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'preparer_agent_key', 'approver_agent_key', 'executor_role_key', 'completed_execution_key', 'control_breach_execution_key', 'late_execution_key', 'unevaluated_blocking_execution_key', 'separation_violation_execution_key', 'cleared_legal_review_key', 'procedure_execution_when_unclean', 'software_execution_step_key', 'vacuously_clean_execution_key', 'preparation_execution_key', 'approval_execution_key', 'ungoverned_divergence_execution_key', 'self_attested_approval_execution_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# REQUIREMENTSATISFACTIONS
# Execution-time evaluations of requirements. Maps to pko:RequirementSatisfaction, refersToRequirement, and hasRequirementSatisfactionLevel.
# =============================================================================

# Level 1

def calc_requirement_satisfactions_name(requirement, satisfaction_level):
    """
    Human-readable calculated display alias for the RequirementSatisfactions row.
    
    Formula: ={{Requirement}} & " / " & {{SatisfactionLevel}}
    """
    return (str(requirement or "") + ' / ' + str(satisfaction_level or ""))

def calc_requirement_satisfactions_is_fully_satisfied(satisfaction_level):
    """
    TRUE only when the requirement is recorded as fully Satisfied. PartiallySatisfied, Unsatisfied, Waived, and blank are all FALSE.
    
    Formula: ={{SatisfactionLevel}} = "Satisfied" 
    """
    return _erb.erb_eq(_erb.erb_nullif(satisfaction_level), 'Satisfied')

def calc_requirement_satisfactions_blocking_satisfaction_step_key(requirement_is_blocking, step_execution):
    """
    Echoes the parent StepExecution id when this satisfaction row concerns a blocking requirement.
    
    Formula: =IF({{RequirementIsBlocking}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(requirement_is_blocking) else '')

def calc_requirement_satisfactions_non_human_evaluated_human_control(requirement_is_blocking, evaluator_agent_kind):
    """
    TRUE when a blocking requirement was evaluated by a non-human agent.
    
    Formula: =AND({{RequirementIsBlocking}}, {{EvaluatorAgentKind}} <> "Human")
    """
    return _erb.erb_and(_erb.erb_bool3(requirement_is_blocking), _erb.erb_bool3(_erb.erb_ne(evaluator_agent_kind, 'Human')))

def calc_requirement_satisfactions_step_execution_when_scored(satisfaction_level, step_execution):
    """
    Echoes the step-execution id when a satisfaction level was actually recorded, blank otherwise.
    
    Formula: =IF({{SatisfactionLevel}} <> "", {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3((not (satisfaction_level is None or satisfaction_level == ""))) else '')

def calc_requirement_satisfactions_is_human_evaluated(evaluator_agent_kind):
    """
    TRUE when a human evaluated this requirement satisfaction.
    
    Formula: ={{EvaluatorAgentKind}} = "Human" 
    """
    return _erb.erb_eq(evaluator_agent_kind, 'Human')

def calc_requirement_satisfactions_evaluator_is_step_executor(evaluated_by_agent, scored_step_executor_agent):
    """
    TRUE when the agent who scored this control is the same agent who performed the step being scored.
    
    Formula: ={{EvaluatedByAgent}} = {{ScoredStepExecutorAgent}}
    """
    return _erb.erb_eq(_erb.erb_nullif(evaluated_by_agent), scored_step_executor_agent)

def calc_requirement_satisfactions_evaluator_owns_the_run(evaluated_by_agent, run_owner_agent):
    """
    TRUE when the agent scoring this control is the agent accountable for the execution it belongs to.
    
    Formula: ={{EvaluatedByAgent}} = {{RunOwnerAgent}}
    """
    return _erb.erb_eq(_erb.erb_nullif(evaluated_by_agent), run_owner_agent)

def calc_requirement_satisfactions_has_written_evidence(evidence):
    """
    TRUE when this satisfaction record carries any evidence text at all.
    
    Formula: ={{Evidence}} <> "" 
    """
    return (not (evidence is None or evidence == ""))

def calc_requirement_satisfactions_is_computedly_witnessed(requirement_is_blocking, requirement_has_computed_witness):
    """
    TRUE when this record scores a blocking control that has a computed witness behind it.
    
    Formula: =AND({{RequirementIsBlocking}}, {{RequirementHasComputedWitness}})
    """
    return _erb.erb_and(_erb.erb_bool3(requirement_is_blocking), _erb.erb_bool3(requirement_has_computed_witness))

def calc_requirement_satisfactions_was_scored_after_attestation(evaluated_at, attestation_instant_for_run):
    """
    TRUE when this control was scored after the run it belongs to had already been attested.
    
    Formula: =DATETIME_DIFF({{EvaluatedAt}}, {{AttestationInstantForRun}}, "minutes") > 0
    """
    return _erb.erb_cmp(_erb.erb_datetime_diff(evaluated_at, attestation_instant_for_run, 'minutes'), '>', 0)

# Level 2

def calc_requirement_satisfactions_is_blocking_and_unmet(requirement_is_blocking, is_fully_satisfied):
    """
    TRUE when a blocking requirement is recorded at anything less than fully Satisfied. This is the control-failure witness at the requirement grain.
    
    Formula: =AND({{RequirementIsBlocking}}, NOT({{IsFullySatisfied}}))
    """
    return _erb.erb_and(_erb.erb_bool3(requirement_is_blocking), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fully_satisfied))))

def calc_requirement_satisfactions_negative_outcome_requirement_key(is_fully_satisfied, requirement):
    """
    Echoes the Requirement id only when this evaluation came out at less than fully Satisfied.
    
    Formula: =IF(NOT({{IsFullySatisfied}}), {{Requirement}}, "")
    """
    return (requirement if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fully_satisfied))) else '')

def calc_requirement_satisfactions_is_asserted_only(requirement_is_blocking, is_fully_satisfied, requirement_has_computed_witness):
    """
    TRUE when a blocking requirement is recorded as Satisfied purely on human assertion, with no computed predicate behind it.
    
    Formula: =AND({{RequirementIsBlocking}}, {{IsFullySatisfied}}, NOT({{RequirementHasComputedWitness}}))
    """
    return _erb.erb_and(_erb.erb_bool3(requirement_is_blocking), _erb.erb_bool3(is_fully_satisfied), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(requirement_has_computed_witness))))

def calc_requirement_satisfactions_is_invalid_approval(requirement_is_approval_type, is_fully_satisfied, is_human_evaluated):
    """
    TRUE when an approval-type requirement is not fully satisfied, or was not evaluated by a human.
    
    Formula: =AND({{RequirementIsApprovalType}} = "Approval", OR(NOT({{IsFullySatisfied}}), NOT({{IsHumanEvaluated}})))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(requirement_is_approval_type, 'Approval')), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fully_satisfied))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_human_evaluated))))))

def calc_requirement_satisfactions_is_clearance_by_unfalsified_control(is_fully_satisfied, requirement_is_blocking, requirement_is_unfalsified):
    """
    TRUE when this record cleared a step against a blocking control that has never produced a negative outcome.
    
    Formula: =AND({{IsFullySatisfied}}, {{RequirementIsBlocking}}, {{RequirementIsUnfalsified}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_fully_satisfied), _erb.erb_bool3(requirement_is_blocking), _erb.erb_bool3(requirement_is_unfalsified))

def calc_requirement_satisfactions_computed_witness_execution_key(is_computedly_witnessed, parent_procedure_execution):
    """
    Echoes the parent execution id when this control was computationally witnessed.
    
    Formula: =IF({{IsComputedlyWitnessed}}, {{ParentProcedureExecution}}, "")
    """
    return (parent_procedure_execution if _erb.erb_bool3(is_computedly_witnessed) else '')

def calc_requirement_satisfactions_post_attestation_score_execution_key(was_scored_after_attestation, parent_procedure_execution):
    """
    Echoes the parent execution id when this control was scored after signature.
    
    Formula: =IF({{WasScoredAfterAttestation}}, {{ParentProcedureExecution}}, "")
    """
    return (parent_procedure_execution if _erb.erb_bool3(was_scored_after_attestation) else '')

# Level 3

def calc_requirement_satisfactions_blocking_unmet_step_key(is_blocking_and_unmet, step_execution):
    """
    Echoes the parent StepExecution id only when this row is a blocking-unmet violation; empty string otherwise.
    
    Formula: =IF({{IsBlockingAndUnmet}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_blocking_and_unmet) else '')

def calc_requirement_satisfactions_asserted_only_execution_key(is_asserted_only, parent_procedure_execution):
    """
    Echoes the grandparent procedure execution id for assertion-only satisfactions.
    
    Formula: =IF({{IsAssertedOnly}}, {{ParentProcedureExecution}}, "")
    """
    return (parent_procedure_execution if _erb.erb_bool3(is_asserted_only) else '')

def calc_requirement_satisfactions_run_when_invalid_approval(is_invalid_approval, procedure_execution_of_satisfaction):
    """
    Echoes the run id when this is an invalid approval, blank otherwise.
    
    Formula: =IF({{IsInvalidApproval}}, {{ProcedureExecutionOfSatisfaction}}, "")
    """
    return (procedure_execution_of_satisfaction if _erb.erb_bool3(is_invalid_approval) else '')

def calc_requirement_satisfactions_unfalsified_clearance_step_key(is_clearance_by_unfalsified_control, step_execution):
    """
    Echoes the step execution id when this clearance came from an unfalsified control.
    
    Formula: =IF({{IsClearanceByUnfalsifiedControl}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_clearance_by_unfalsified_control) else '')

def calc_requirement_satisfactions_is_interested_party_assertion(is_asserted_only, evaluator_is_step_executor, evaluator_owns_the_run):
    """
    TRUE when a blocking control's only evidence is an assertion made by someone with an interest in the outcome.
    
    Formula: =AND({{IsAssertedOnly}}, OR({{EvaluatorIsStepExecutor}}, {{EvaluatorOwnsTheRun}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_asserted_only), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(evaluator_is_step_executor), _erb.erb_bool3(evaluator_owns_the_run))))

def calc_requirement_satisfactions_is_bare_assertion(is_asserted_only, has_written_evidence):
    """
    TRUE when a blocking control was cleared with no computed witness and no written justification.
    
    Formula: =AND({{IsAssertedOnly}}, NOT({{HasWrittenEvidence}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_asserted_only), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_written_evidence))))

# Level 4

def calc_requirement_satisfactions_interested_assertion_execution_key(is_interested_party_assertion, parent_procedure_execution):
    """
    Echoes the parent execution id when this record is an interested-party assertion.
    
    Formula: =IF({{IsInterestedPartyAssertion}}, {{ParentProcedureExecution}}, "")
    """
    return (parent_procedure_execution if _erb.erb_bool3(is_interested_party_assertion) else '')


def compute_requirement_satisfactions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RequirementSatisfactions.
    
    Execution-time evaluations of requirements. Maps to pko:RequirementSatisfaction, refersToRequirement, and hasRequirementSatisfactionLevel.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_requirement_satisfactions_name(result.get('requirement'), result.get('satisfaction_level'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_fully_satisfied'] = calc_requirement_satisfactions_is_fully_satisfied(result.get('satisfaction_level'))
    except Exception as _field_exc:
        result['is_fully_satisfied'] = None
        result.setdefault('_erb_errors', {})['is_fully_satisfied'] = str(_field_exc)
    try:
        result['blocking_satisfaction_step_key'] = calc_requirement_satisfactions_blocking_satisfaction_step_key(result.get('requirement_is_blocking'), result.get('step_execution'))
    except Exception as _field_exc:
        result['blocking_satisfaction_step_key'] = None
        result.setdefault('_erb_errors', {})['blocking_satisfaction_step_key'] = str(_field_exc)
    try:
        result['non_human_evaluated_human_control'] = calc_requirement_satisfactions_non_human_evaluated_human_control(result.get('requirement_is_blocking'), result.get('evaluator_agent_kind'))
    except Exception as _field_exc:
        result['non_human_evaluated_human_control'] = None
        result.setdefault('_erb_errors', {})['non_human_evaluated_human_control'] = str(_field_exc)
    try:
        result['step_execution_when_scored'] = calc_requirement_satisfactions_step_execution_when_scored(result.get('satisfaction_level'), result.get('step_execution'))
    except Exception as _field_exc:
        result['step_execution_when_scored'] = None
        result.setdefault('_erb_errors', {})['step_execution_when_scored'] = str(_field_exc)
    try:
        result['is_human_evaluated'] = calc_requirement_satisfactions_is_human_evaluated(result.get('evaluator_agent_kind'))
    except Exception as _field_exc:
        result['is_human_evaluated'] = None
        result.setdefault('_erb_errors', {})['is_human_evaluated'] = str(_field_exc)
    try:
        result['evaluator_is_step_executor'] = calc_requirement_satisfactions_evaluator_is_step_executor(result.get('evaluated_by_agent'), result.get('scored_step_executor_agent'))
    except Exception as _field_exc:
        result['evaluator_is_step_executor'] = None
        result.setdefault('_erb_errors', {})['evaluator_is_step_executor'] = str(_field_exc)
    try:
        result['evaluator_owns_the_run'] = calc_requirement_satisfactions_evaluator_owns_the_run(result.get('evaluated_by_agent'), result.get('run_owner_agent'))
    except Exception as _field_exc:
        result['evaluator_owns_the_run'] = None
        result.setdefault('_erb_errors', {})['evaluator_owns_the_run'] = str(_field_exc)
    try:
        result['has_written_evidence'] = calc_requirement_satisfactions_has_written_evidence(result.get('evidence'))
    except Exception as _field_exc:
        result['has_written_evidence'] = None
        result.setdefault('_erb_errors', {})['has_written_evidence'] = str(_field_exc)
    try:
        result['is_computedly_witnessed'] = calc_requirement_satisfactions_is_computedly_witnessed(result.get('requirement_is_blocking'), result.get('requirement_has_computed_witness'))
    except Exception as _field_exc:
        result['is_computedly_witnessed'] = None
        result.setdefault('_erb_errors', {})['is_computedly_witnessed'] = str(_field_exc)
    try:
        result['was_scored_after_attestation'] = calc_requirement_satisfactions_was_scored_after_attestation(result.get('evaluated_at'), result.get('attestation_instant_for_run'))
    except Exception as _field_exc:
        result['was_scored_after_attestation'] = None
        result.setdefault('_erb_errors', {})['was_scored_after_attestation'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_blocking_and_unmet'] = calc_requirement_satisfactions_is_blocking_and_unmet(result.get('requirement_is_blocking'), result.get('is_fully_satisfied'))
    except Exception as _field_exc:
        result['is_blocking_and_unmet'] = None
        result.setdefault('_erb_errors', {})['is_blocking_and_unmet'] = str(_field_exc)
    try:
        result['negative_outcome_requirement_key'] = calc_requirement_satisfactions_negative_outcome_requirement_key(result.get('is_fully_satisfied'), result.get('requirement'))
    except Exception as _field_exc:
        result['negative_outcome_requirement_key'] = None
        result.setdefault('_erb_errors', {})['negative_outcome_requirement_key'] = str(_field_exc)
    try:
        result['is_asserted_only'] = calc_requirement_satisfactions_is_asserted_only(result.get('requirement_is_blocking'), result.get('is_fully_satisfied'), result.get('requirement_has_computed_witness'))
    except Exception as _field_exc:
        result['is_asserted_only'] = None
        result.setdefault('_erb_errors', {})['is_asserted_only'] = str(_field_exc)
    try:
        result['is_invalid_approval'] = calc_requirement_satisfactions_is_invalid_approval(result.get('requirement_is_approval_type'), result.get('is_fully_satisfied'), result.get('is_human_evaluated'))
    except Exception as _field_exc:
        result['is_invalid_approval'] = None
        result.setdefault('_erb_errors', {})['is_invalid_approval'] = str(_field_exc)
    try:
        result['is_clearance_by_unfalsified_control'] = calc_requirement_satisfactions_is_clearance_by_unfalsified_control(result.get('is_fully_satisfied'), result.get('requirement_is_blocking'), result.get('requirement_is_unfalsified'))
    except Exception as _field_exc:
        result['is_clearance_by_unfalsified_control'] = None
        result.setdefault('_erb_errors', {})['is_clearance_by_unfalsified_control'] = str(_field_exc)
    try:
        result['computed_witness_execution_key'] = calc_requirement_satisfactions_computed_witness_execution_key(result.get('is_computedly_witnessed'), result.get('parent_procedure_execution'))
    except Exception as _field_exc:
        result['computed_witness_execution_key'] = None
        result.setdefault('_erb_errors', {})['computed_witness_execution_key'] = str(_field_exc)
    try:
        result['post_attestation_score_execution_key'] = calc_requirement_satisfactions_post_attestation_score_execution_key(result.get('was_scored_after_attestation'), result.get('parent_procedure_execution'))
    except Exception as _field_exc:
        result['post_attestation_score_execution_key'] = None
        result.setdefault('_erb_errors', {})['post_attestation_score_execution_key'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['blocking_unmet_step_key'] = calc_requirement_satisfactions_blocking_unmet_step_key(result.get('is_blocking_and_unmet'), result.get('step_execution'))
    except Exception as _field_exc:
        result['blocking_unmet_step_key'] = None
        result.setdefault('_erb_errors', {})['blocking_unmet_step_key'] = str(_field_exc)
    try:
        result['asserted_only_execution_key'] = calc_requirement_satisfactions_asserted_only_execution_key(result.get('is_asserted_only'), result.get('parent_procedure_execution'))
    except Exception as _field_exc:
        result['asserted_only_execution_key'] = None
        result.setdefault('_erb_errors', {})['asserted_only_execution_key'] = str(_field_exc)
    try:
        result['run_when_invalid_approval'] = calc_requirement_satisfactions_run_when_invalid_approval(result.get('is_invalid_approval'), result.get('procedure_execution_of_satisfaction'))
    except Exception as _field_exc:
        result['run_when_invalid_approval'] = None
        result.setdefault('_erb_errors', {})['run_when_invalid_approval'] = str(_field_exc)
    try:
        result['unfalsified_clearance_step_key'] = calc_requirement_satisfactions_unfalsified_clearance_step_key(result.get('is_clearance_by_unfalsified_control'), result.get('step_execution'))
    except Exception as _field_exc:
        result['unfalsified_clearance_step_key'] = None
        result.setdefault('_erb_errors', {})['unfalsified_clearance_step_key'] = str(_field_exc)
    try:
        result['is_interested_party_assertion'] = calc_requirement_satisfactions_is_interested_party_assertion(result.get('is_asserted_only'), result.get('evaluator_is_step_executor'), result.get('evaluator_owns_the_run'))
    except Exception as _field_exc:
        result['is_interested_party_assertion'] = None
        result.setdefault('_erb_errors', {})['is_interested_party_assertion'] = str(_field_exc)
    try:
        result['is_bare_assertion'] = calc_requirement_satisfactions_is_bare_assertion(result.get('is_asserted_only'), result.get('has_written_evidence'))
    except Exception as _field_exc:
        result['is_bare_assertion'] = None
        result.setdefault('_erb_errors', {})['is_bare_assertion'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['interested_assertion_execution_key'] = calc_requirement_satisfactions_interested_assertion_execution_key(result.get('is_interested_party_assertion'), result.get('parent_procedure_execution'))
    except Exception as _field_exc:
        result['interested_assertion_execution_key'] = None
        result.setdefault('_erb_errors', {})['interested_assertion_execution_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'blocking_unmet_step_key', 'blocking_satisfaction_step_key', 'negative_outcome_requirement_key', 'asserted_only_execution_key', 'step_execution_when_scored', 'run_when_invalid_approval', 'unfalsified_clearance_step_key', 'interested_assertion_execution_key', 'computed_witness_execution_key', 'post_attestation_score_execution_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ERRORS
# Reusable error definitions encountered during execution. Maps to pko:Error, errorCode, and errorCause.
# =============================================================================

# Level 1

def calc_errors_name(error_code, label):
    """
    Human-readable calculated display alias for the Errors row.
    
    Formula: ={{ErrorCode}} & " - " & {{Label}}
    """
    return (str(error_code or "") + ' - ' + str(label or ""))


def compute_errors_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Errors.
    
    Reusable error definitions encountered during execution. Maps to pko:Error, errorCode, and errorCause.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_errors_name(result.get('error_code'), result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ISSUEOCCURRENCES
# Concrete issue events during execution. Maps to pko:IssueOccurrence, hasEncounteredError, wasEncounteredBy, issueCause, and issueSolution.
# =============================================================================

# Level 1

def calc_issue_occurrences_name(error, occurred_at):
    """
    Human-readable calculated display alias for the IssueOccurrences row.
    
    Formula: ={{Error}} & " @ " & {{OccurredAt}}
    """
    return (str(error or "") + ' @ ' + _erb.erb_timestamptz_text(occurred_at))

def calc_issue_occurrences_is_unresolved(status):
    """
    TRUE when this issue occurrence has not been closed out.
    
    Formula: =OR({{Status}} = "Open", {{Status}} = "Investigating", {{Status}} = "Monitoring")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Open')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Investigating')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Monitoring')))

# Level 2

def calc_issue_occurrences_step_execution_when_unresolved(is_unresolved, step_execution):
    """
    Echoes the step-execution id when the issue is unresolved, blank otherwise.
    
    Formula: =IF({{IsUnresolved}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_unresolved) else '')


def compute_issue_occurrences_fields(record: dict) -> dict:
    """
    Compute all calculated fields for IssueOccurrences.
    
    Concrete issue events during execution. Maps to pko:IssueOccurrence, hasEncounteredError, wasEncounteredBy, issueCause, and issueSolution.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_issue_occurrences_name(result.get('error'), result.get('occurred_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_unresolved'] = calc_issue_occurrences_is_unresolved(result.get('status'))
    except Exception as _field_exc:
        result['is_unresolved'] = None
        result.setdefault('_erb_errors', {})['is_unresolved'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['step_execution_when_unresolved'] = calc_issue_occurrences_step_execution_when_unresolved(result.get('is_unresolved'), result.get('step_execution'))
    except Exception as _field_exc:
        result['step_execution_when_unresolved'] = None
        result.setdefault('_erb_errors', {})['step_execution_when_unresolved'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'step_execution_when_unresolved']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# USERQUESTIONS
# Questions asked by agents during execution. Maps to pko:UserQuestionOccurrence, questionByUser, wasAskedBy, and isQuestionAddressedBy.
# =============================================================================

# Level 1

def calc_user_questions_name(question_text):
    """
    Human-readable calculated display alias for the UserQuestions row.
    
    Formula: =LEFT({{QuestionText}}, 70)
    """
    return ((question_text or "")[:(70 or 0)])


def compute_user_questions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for UserQuestions.
    
    Questions asked by agents during execution. Maps to pko:UserQuestionOccurrence, questionByUser, wasAskedBy, and isQuestionAddressedBy.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_user_questions_name(result.get('question_text'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# USERFEEDBACK
# Feedback supplied by users about a procedure or execution. Maps to pko:UserFeedbackOccurrence, feedbackOnProcedureExecution, and wasProvidedBy.
# =============================================================================

# Level 1

def calc_user_feedback_name(disposition, feedback_text):
    """
    Human-readable calculated display alias for the UserFeedback row.
    
    Formula: ={{Disposition}} & ": " & LEFT({{FeedbackText}}, 60)
    """
    return (str(disposition or "") + ': ' + str(((feedback_text or "")[:(60 or 0)]) if ((feedback_text or "")[:(60 or 0)]) is not None else ""))


def compute_user_feedback_fields(record: dict) -> dict:
    """
    Compute all calculated fields for UserFeedback.
    
    Feedback supplied by users about a procedure or execution. Maps to pko:UserFeedbackOccurrence, feedbackOnProcedureExecution, and wasProvidedBy.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_user_feedback_name(result.get('disposition'), result.get('feedback_text'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# STEWARDSHIPASSIGNMENTS
# Separates ongoing stewardship from authority to approve semantic commitments. Explicit ERB-PKO governance extension.
# =============================================================================

# Level 1

def calc_stewardship_assignments_name(procedure_version, steward_role):
    """
    Human-readable calculated display alias for the StewardshipAssignments row.
    
    Formula: ={{ProcedureVersion}} & " / steward=" & {{StewardRole}}
    """
    return (str(procedure_version or "") + ' / steward=' + str(steward_role or ""))

def calc_stewardship_assignments_has_ever_been_reviewed(count_of_review_events):
    """
    TRUE if at least one review event exists for the stewarded procedure version.
    
    Formula: ={{CountOfReviewEvents}} > 0
    """
    return _erb.erb_cmp(count_of_review_events, '>', 0)

def calc_stewardship_assignments_is_current_assignment(valid_from, as_of_instant, valid_to):
    """
    TRUE when this stewardship assignment is in force right now.
    
    Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', as_of_instant)), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))))


def compute_stewardship_assignments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for StewardshipAssignments.
    
    Separates ongoing stewardship from authority to approve semantic commitments. Explicit ERB-PKO governance extension.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_stewardship_assignments_name(result.get('procedure_version'), result.get('steward_role'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_ever_been_reviewed'] = calc_stewardship_assignments_has_ever_been_reviewed(result.get('count_of_review_events'))
    except Exception as _field_exc:
        result['has_ever_been_reviewed'] = None
        result.setdefault('_erb_errors', {})['has_ever_been_reviewed'] = str(_field_exc)
    try:
        result['is_current_assignment'] = calc_stewardship_assignments_is_current_assignment(result.get('valid_from'), result.get('as_of_instant'), result.get('valid_to'))
    except Exception as _field_exc:
        result['is_current_assignment'] = None
        result.setdefault('_erb_errors', {})['is_current_assignment'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# CHANGEREQUESTS
# Governed requests for semantic or operational change, anchored to a procedure version and authority. Explicit ERB-PKO extension.
# =============================================================================

# Level 1

def calc_change_requests_name(title):
    """
    Human-readable calculated display alias for the ChangeRequests row.
    
    Formula: ={{Title}}
    """
    return title

def calc_change_requests_is_open(status, implemented_at):
    """
    TRUE while the change request is still outstanding. An Approved request stays open until ImplementedAt records that it actually landed — approval is a decision, not an outcome.
    
    Formula: =AND(OR({{Status}} = "Draft", {{Status}} = "UnderReview", {{Status}} = "Approved"), {{ImplementedAt}} = "")
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Draft')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'UnderReview')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')))), _erb.erb_bool3((implemented_at is None or implemented_at == "")))

def calc_change_requests_is_decided(decided_at):
    """
    TRUE when a decision timestamp has been recorded.
    
    Formula: ={{DecidedAt}} <> "" 
    """
    return (not (decided_at is None or decided_at == ""))

def calc_change_requests_requester_is_authority(requested_by_agent, authority_agent):
    """
    TRUE when the agent who raised the request is also the agent who decides it.
    
    Formula: ={{RequestedByAgent}} = {{AuthorityAgent}}
    """
    return _erb.erb_eq(_erb.erb_nullif(requested_by_agent), authority_agent)

def calc_change_requests_is_implemented(implemented_at):
    """
    Whether the approved change has actually been applied.
    
    Formula: ={{ImplementedAt}} <> "" 
    """
    return (not (implemented_at is None or implemented_at == ""))

def calc_change_requests_is_approved_decision(status):
    """
    TRUE when this change request was decided in the affirmative.
    
    Formula: ={{Status}} = "Approved" 
    """
    return _erb.erb_eq(_erb.erb_nullif(status), 'Approved')

# Level 2

def calc_change_requests_open_change_version_key(is_open, procedure_version):
    """
    Echoes the ProcedureVersion id only for change requests still open.
    
    Formula: =IF({{IsOpen}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_open) else '')

def calc_change_requests_days_pending(is_decided, decided_at, requested_at, as_of_instant):
    """
    Days from request to decision, or to now if still undecided.
    
    Formula: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{RequestedAt}}, "days"))
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(decided_at, requested_at, 'days') if _erb.erb_bool3(is_decided) else _erb.erb_datetime_diff(as_of_instant, requested_at, 'days')))

def calc_change_requests_is_still_pending(is_open, is_decided):
    """
    TRUE when the request is in an open status and has no decision recorded.
    
    Formula: =AND({{IsOpen}}, NOT({{IsDecided}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_open), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_decided))))

def calc_change_requests_awaits_authority_decision(status, is_decided):
    """
    TRUE when this request is formally before its authority and no decision has been recorded.
    
    Formula: =AND({{Status}} = "UnderReview", NOT({{IsDecided}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'UnderReview')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_decided))))

def calc_change_requests_is_my_decided_request(authority_role, is_decided):
    """
    A change request I have personally ruled on.
    
    Formula: =AND({{AuthorityRole}} = "hr-policy-owner", {{IsDecided}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(authority_role), 'hr-policy-owner')), _erb.erb_bool3(is_decided))

def calc_change_requests_decision_latency_days(is_decided, decided_at, requested_at):
    """
    How many days I took to rule, once ruled. Zero when undecided — read only alongside IsDecided.
    
    Formula: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(decided_at, requested_at, 'days') if _erb.erb_bool3(is_decided) else 0))

def calc_change_requests_implementation_latency_days(is_implemented, implemented_at, decided_at):
    """
    How many days elapsed between my ruling and the change actually landing. Zero when not yet implemented.
    
    Formula: =IF({{IsImplemented}}, DATETIME_DIFF({{ImplementedAt}}, {{DecidedAt}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(implemented_at, decided_at, 'days') if _erb.erb_bool3(is_implemented) else 0))

def calc_change_requests_is_approved_not_implemented(status, is_implemented):
    """
    A change request the authority approved but that has not yet been applied.
    
    Formula: =AND({{Status}} = "Approved", NOT({{IsImplemented}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_implemented))))

def calc_change_requests_days_since_approval(is_decided, as_of_instant, decided_at):
    """
    How many days have elapsed since the authority decided this request. Zero when undecided.
    
    Formula: =IF({{IsDecided}}, DATETIME_DIFF({{AsOfInstant}}, {{DecidedAt}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(as_of_instant, decided_at, 'days') if _erb.erb_bool3(is_decided) else 0))

def calc_change_requests_approved_version_key(is_approved_decision, procedure_version):
    """
    Echoes the target version id when this change request was approved.
    
    Formula: =IF({{IsApprovedDecision}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_approved_decision) else '')

# Level 3

def calc_change_requests_is_stalled(is_still_pending, days_pending):
    """
    TRUE when an undecided change request has been pending more than fourteen days.
    
    Formula: =AND({{IsStillPending}}, {{DaysPending}} > 14)
    """
    return _erb.erb_and(_erb.erb_bool3(is_still_pending), _erb.erb_bool3(_erb.erb_cmp(days_pending, '>', 14)))

def calc_change_requests_is_live_decision_backlog(awaits_authority_decision, touches_live_version):
    """
    TRUE when a decision I owe is blocking a change to a procedure currently in production.
    
    Formula: =AND({{AwaitsAuthorityDecision}}, {{TouchesLiveVersion}})
    """
    return _erb.erb_and(_erb.erb_bool3(awaits_authority_decision), _erb.erb_bool3(touches_live_version))

def calc_change_requests_is_my_pending_decision(authority_role, awaits_authority_decision):
    """
    A change request awaiting a decision that is mine personally to make.
    
    Formula: =AND({{AuthorityRole}} = "hr-policy-owner", {{AwaitsAuthorityDecision}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(authority_role), 'hr-policy-owner')), _erb.erb_bool3(awaits_authority_decision))

def calc_change_requests_is_my_decided_but_unlanded(is_my_decided_request, is_implemented):
    """
    A decision I have made that has not yet been applied — still counted against me by the loop-1 open-request measure.
    
    Formula: =AND({{IsMyDecidedRequest}}, NOT({{IsImplemented}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_my_decided_request), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_implemented))))

def calc_change_requests_is_stalled_implementation(is_approved_not_implemented, days_since_approval):
    """
    An approved change request that has sat unimplemented for more than two weeks.
    
    Formula: =AND({{IsApprovedNotImplemented}}, {{DaysSinceApproval}} > 14)
    """
    return _erb.erb_and(_erb.erb_bool3(is_approved_not_implemented), _erb.erb_bool3(_erb.erb_cmp(days_since_approval, '>', 14)))

# Level 4

def calc_change_requests_blocks_an_open_gap(is_live_decision_backlog, change_kind):
    """
    TRUE when an undecided request against a live version is the kind that exists to close a known gap.
    
    Formula: =AND({{IsLiveDecisionBacklog}}, {{ChangeKind}} = "Enhancement")
    """
    return _erb.erb_and(_erb.erb_bool3(is_live_decision_backlog), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(change_kind), 'Enhancement')))

def calc_change_requests_backlog_version_key(is_live_decision_backlog, procedure_version):
    """
    Composite-key echo: this request's procedure version when it is live decision backlog, blank otherwise.
    
    Formula: =IF({{IsLiveDecisionBacklog}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_live_decision_backlog) else '')

def calc_change_requests_delay_is_downstream_of_me(is_my_decided_but_unlanded, decision_latency_days):
    """
    A request I decided promptly that is nevertheless still outstanding because nobody has implemented it.
    
    Formula: =AND({{IsMyDecidedButUnlanded}}, {{DecisionLatencyDays}} <= 14)
    """
    return _erb.erb_and(_erb.erb_bool3(is_my_decided_but_unlanded), _erb.erb_bool3(_erb.erb_cmp(decision_latency_days, '<=', 14)))

def calc_change_requests_unlanded_version_key(is_my_decided_but_unlanded, procedure_version):
    """
    Composite-key echo: this request's procedure version when I have decided it but it has not landed, blank otherwise.
    
    Formula: =IF({{IsMyDecidedButUnlanded}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_my_decided_but_unlanded) else '')

def calc_change_requests_stalled_implementation_version_key(is_stalled_implementation, procedure_version):
    """
    Composite-key echo: this request's procedure version when its implementation is stalled, blank otherwise.
    
    Formula: =IF({{IsStalledImplementation}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_stalled_implementation) else '')

# Level 5

def calc_change_requests_is_my_blocking_backlog(is_my_pending_decision, blocks_an_open_gap):
    """
    A decision waiting on me that is holding an open gap on a live procedure.
    
    Formula: =AND({{IsMyPendingDecision}}, {{BlocksAnOpenGap}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_my_pending_decision), _erb.erb_bool3(blocks_an_open_gap))

# Level 6

def calc_change_requests_is_my_overdue_backlog(is_my_blocking_backlog, days_pending):
    """
    A blocking decision that has waited on me for more than two weeks.
    
    Formula: =AND({{IsMyBlockingBacklog}}, {{DaysPending}} > 14)
    """
    return _erb.erb_and(_erb.erb_bool3(is_my_blocking_backlog), _erb.erb_bool3(_erb.erb_cmp(days_pending, '>', 14)))


def compute_change_requests_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ChangeRequests.
    
    Governed requests for semantic or operational change, anchored to a procedure version and authority. Explicit ERB-PKO extension.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_change_requests_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_open'] = calc_change_requests_is_open(result.get('status'), result.get('implemented_at'))
    except Exception as _field_exc:
        result['is_open'] = None
        result.setdefault('_erb_errors', {})['is_open'] = str(_field_exc)
    try:
        result['is_decided'] = calc_change_requests_is_decided(result.get('decided_at'))
    except Exception as _field_exc:
        result['is_decided'] = None
        result.setdefault('_erb_errors', {})['is_decided'] = str(_field_exc)
    try:
        result['requester_is_authority'] = calc_change_requests_requester_is_authority(result.get('requested_by_agent'), result.get('authority_agent'))
    except Exception as _field_exc:
        result['requester_is_authority'] = None
        result.setdefault('_erb_errors', {})['requester_is_authority'] = str(_field_exc)
    try:
        result['is_implemented'] = calc_change_requests_is_implemented(result.get('implemented_at'))
    except Exception as _field_exc:
        result['is_implemented'] = None
        result.setdefault('_erb_errors', {})['is_implemented'] = str(_field_exc)
    try:
        result['is_approved_decision'] = calc_change_requests_is_approved_decision(result.get('status'))
    except Exception as _field_exc:
        result['is_approved_decision'] = None
        result.setdefault('_erb_errors', {})['is_approved_decision'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['open_change_version_key'] = calc_change_requests_open_change_version_key(result.get('is_open'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['open_change_version_key'] = None
        result.setdefault('_erb_errors', {})['open_change_version_key'] = str(_field_exc)
    try:
        result['days_pending'] = calc_change_requests_days_pending(result.get('is_decided'), result.get('decided_at'), result.get('requested_at'), result.get('as_of_instant'))
    except Exception as _field_exc:
        result['days_pending'] = None
        result.setdefault('_erb_errors', {})['days_pending'] = str(_field_exc)
    try:
        result['is_still_pending'] = calc_change_requests_is_still_pending(result.get('is_open'), result.get('is_decided'))
    except Exception as _field_exc:
        result['is_still_pending'] = None
        result.setdefault('_erb_errors', {})['is_still_pending'] = str(_field_exc)
    try:
        result['awaits_authority_decision'] = calc_change_requests_awaits_authority_decision(result.get('status'), result.get('is_decided'))
    except Exception as _field_exc:
        result['awaits_authority_decision'] = None
        result.setdefault('_erb_errors', {})['awaits_authority_decision'] = str(_field_exc)
    try:
        result['is_my_decided_request'] = calc_change_requests_is_my_decided_request(result.get('authority_role'), result.get('is_decided'))
    except Exception as _field_exc:
        result['is_my_decided_request'] = None
        result.setdefault('_erb_errors', {})['is_my_decided_request'] = str(_field_exc)
    try:
        result['decision_latency_days'] = calc_change_requests_decision_latency_days(result.get('is_decided'), result.get('decided_at'), result.get('requested_at'))
    except Exception as _field_exc:
        result['decision_latency_days'] = None
        result.setdefault('_erb_errors', {})['decision_latency_days'] = str(_field_exc)
    try:
        result['implementation_latency_days'] = calc_change_requests_implementation_latency_days(result.get('is_implemented'), result.get('implemented_at'), result.get('decided_at'))
    except Exception as _field_exc:
        result['implementation_latency_days'] = None
        result.setdefault('_erb_errors', {})['implementation_latency_days'] = str(_field_exc)
    try:
        result['is_approved_not_implemented'] = calc_change_requests_is_approved_not_implemented(result.get('status'), result.get('is_implemented'))
    except Exception as _field_exc:
        result['is_approved_not_implemented'] = None
        result.setdefault('_erb_errors', {})['is_approved_not_implemented'] = str(_field_exc)
    try:
        result['days_since_approval'] = calc_change_requests_days_since_approval(result.get('is_decided'), result.get('as_of_instant'), result.get('decided_at'))
    except Exception as _field_exc:
        result['days_since_approval'] = None
        result.setdefault('_erb_errors', {})['days_since_approval'] = str(_field_exc)
    try:
        result['approved_version_key'] = calc_change_requests_approved_version_key(result.get('is_approved_decision'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['approved_version_key'] = None
        result.setdefault('_erb_errors', {})['approved_version_key'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_stalled'] = calc_change_requests_is_stalled(result.get('is_still_pending'), result.get('days_pending'))
    except Exception as _field_exc:
        result['is_stalled'] = None
        result.setdefault('_erb_errors', {})['is_stalled'] = str(_field_exc)
    try:
        result['is_live_decision_backlog'] = calc_change_requests_is_live_decision_backlog(result.get('awaits_authority_decision'), result.get('touches_live_version'))
    except Exception as _field_exc:
        result['is_live_decision_backlog'] = None
        result.setdefault('_erb_errors', {})['is_live_decision_backlog'] = str(_field_exc)
    try:
        result['is_my_pending_decision'] = calc_change_requests_is_my_pending_decision(result.get('authority_role'), result.get('awaits_authority_decision'))
    except Exception as _field_exc:
        result['is_my_pending_decision'] = None
        result.setdefault('_erb_errors', {})['is_my_pending_decision'] = str(_field_exc)
    try:
        result['is_my_decided_but_unlanded'] = calc_change_requests_is_my_decided_but_unlanded(result.get('is_my_decided_request'), result.get('is_implemented'))
    except Exception as _field_exc:
        result['is_my_decided_but_unlanded'] = None
        result.setdefault('_erb_errors', {})['is_my_decided_but_unlanded'] = str(_field_exc)
    try:
        result['is_stalled_implementation'] = calc_change_requests_is_stalled_implementation(result.get('is_approved_not_implemented'), result.get('days_since_approval'))
    except Exception as _field_exc:
        result['is_stalled_implementation'] = None
        result.setdefault('_erb_errors', {})['is_stalled_implementation'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['blocks_an_open_gap'] = calc_change_requests_blocks_an_open_gap(result.get('is_live_decision_backlog'), result.get('change_kind'))
    except Exception as _field_exc:
        result['blocks_an_open_gap'] = None
        result.setdefault('_erb_errors', {})['blocks_an_open_gap'] = str(_field_exc)
    try:
        result['backlog_version_key'] = calc_change_requests_backlog_version_key(result.get('is_live_decision_backlog'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['backlog_version_key'] = None
        result.setdefault('_erb_errors', {})['backlog_version_key'] = str(_field_exc)
    try:
        result['delay_is_downstream_of_me'] = calc_change_requests_delay_is_downstream_of_me(result.get('is_my_decided_but_unlanded'), result.get('decision_latency_days'))
    except Exception as _field_exc:
        result['delay_is_downstream_of_me'] = None
        result.setdefault('_erb_errors', {})['delay_is_downstream_of_me'] = str(_field_exc)
    try:
        result['unlanded_version_key'] = calc_change_requests_unlanded_version_key(result.get('is_my_decided_but_unlanded'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['unlanded_version_key'] = None
        result.setdefault('_erb_errors', {})['unlanded_version_key'] = str(_field_exc)
    try:
        result['stalled_implementation_version_key'] = calc_change_requests_stalled_implementation_version_key(result.get('is_stalled_implementation'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['stalled_implementation_version_key'] = None
        result.setdefault('_erb_errors', {})['stalled_implementation_version_key'] = str(_field_exc)

    # Level 5 calculations
    try:
        result['is_my_blocking_backlog'] = calc_change_requests_is_my_blocking_backlog(result.get('is_my_pending_decision'), result.get('blocks_an_open_gap'))
    except Exception as _field_exc:
        result['is_my_blocking_backlog'] = None
        result.setdefault('_erb_errors', {})['is_my_blocking_backlog'] = str(_field_exc)

    # Level 6 calculations
    try:
        result['is_my_overdue_backlog'] = calc_change_requests_is_my_overdue_backlog(result.get('is_my_blocking_backlog'), result.get('days_pending'))
    except Exception as _field_exc:
        result['is_my_overdue_backlog'] = None
        result.setdefault('_erb_errors', {})['is_my_overdue_backlog'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'open_change_version_key', 'backlog_version_key', 'unlanded_version_key', 'stalled_implementation_version_key', 'approved_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# REVIEWEVENTS
# Periodic governance reviews that test competency coverage, staleness, and semantic integrity. Explicit ERB-PKO extension represented as prov:Activity.
# =============================================================================

# Level 1

def calc_review_events_name(procedure_version, review_kind):
    """
    Human-readable calculated display alias for the ReviewEvents row.
    
    Formula: ={{ProcedureVersion}} & " / " & {{ReviewKind}}
    """
    return (str(procedure_version or "") + ' / ' + str(review_kind or ""))

def calc_review_events_is_overdue(next_review_due, as_of_instant):
    """
    TRUE when review is overdue.
    
    Formula: ={{NextReviewDue}} < {{AsOfInstant}}
    """
    return _erb.erb_cmp(_erb.erb_nullif(next_review_due), '<', as_of_instant)

def calc_review_events_days_since_reviewed(as_of_instant, reviewed_at):
    """
    Elapsed days since this review actually happened.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{ReviewedAt}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, reviewed_at, 'days'))

# Level 2

def calc_review_events_overdue_version_key(is_overdue, procedure_version):
    """
    Echoes the ProcedureVersion id only when this review is past its next-due date.
    
    Formula: =IF({{IsOverdue}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_overdue) else '')

def calc_review_events_exceeds_promised_cadence(days_since_reviewed, promised_cadence_days):
    """
    TRUE when more days have elapsed since this review than the stewardship assignment promised as a cadence.
    
    Formula: ={{DaysSinceReviewed}} > {{PromisedCadenceDays}}
    """
    return _erb.erb_cmp(days_since_reviewed, '>', promised_cadence_days)

def calc_review_events_cadence_drift_days(days_since_reviewed, promised_cadence_days):
    """
    Signed drift: positive means we are past the promised cadence by this many days; negative means we are still inside it.
    
    Formula: ={{DaysSinceReviewed}} - {{PromisedCadenceDays}}
    """
    return _erb.erb_integer(_erb.erb_sub(days_since_reviewed, promised_cadence_days))

# Level 3

def calc_review_events_promise_and_behavior_disagree(exceeds_promised_cadence, is_overdue):
    """
    TRUE when the promised cadence has been blown but the hand-entered NextReviewDue still says we are fine.
    
    Formula: =AND({{ExceedsPromisedCadence}}, NOT({{IsOverdue}}))
    """
    return _erb.erb_and(_erb.erb_bool3(exceeds_promised_cadence), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_overdue))))

def calc_review_events_cadence_breach_version_key(exceeds_promised_cadence, procedure_version):
    """
    Composite-key echo: this review event's procedure version when the promised cadence has been exceeded, blank otherwise.
    
    Formula: =IF({{ExceedsPromisedCadence}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(exceeds_promised_cadence) else '')


def compute_review_events_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ReviewEvents.
    
    Periodic governance reviews that test competency coverage, staleness, and semantic integrity. Explicit ERB-PKO extension represented as prov:Activity.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_review_events_name(result.get('procedure_version'), result.get('review_kind'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_overdue'] = calc_review_events_is_overdue(result.get('next_review_due'), result.get('as_of_instant'))
    except Exception as _field_exc:
        result['is_overdue'] = None
        result.setdefault('_erb_errors', {})['is_overdue'] = str(_field_exc)
    try:
        result['days_since_reviewed'] = calc_review_events_days_since_reviewed(result.get('as_of_instant'), result.get('reviewed_at'))
    except Exception as _field_exc:
        result['days_since_reviewed'] = None
        result.setdefault('_erb_errors', {})['days_since_reviewed'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['overdue_version_key'] = calc_review_events_overdue_version_key(result.get('is_overdue'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['overdue_version_key'] = None
        result.setdefault('_erb_errors', {})['overdue_version_key'] = str(_field_exc)
    try:
        result['exceeds_promised_cadence'] = calc_review_events_exceeds_promised_cadence(result.get('days_since_reviewed'), result.get('promised_cadence_days'))
    except Exception as _field_exc:
        result['exceeds_promised_cadence'] = None
        result.setdefault('_erb_errors', {})['exceeds_promised_cadence'] = str(_field_exc)
    try:
        result['cadence_drift_days'] = calc_review_events_cadence_drift_days(result.get('days_since_reviewed'), result.get('promised_cadence_days'))
    except Exception as _field_exc:
        result['cadence_drift_days'] = None
        result.setdefault('_erb_errors', {})['cadence_drift_days'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['promise_and_behavior_disagree'] = calc_review_events_promise_and_behavior_disagree(result.get('exceeds_promised_cadence'), result.get('is_overdue'))
    except Exception as _field_exc:
        result['promise_and_behavior_disagree'] = None
        result.setdefault('_erb_errors', {})['promise_and_behavior_disagree'] = str(_field_exc)
    try:
        result['cadence_breach_version_key'] = calc_review_events_cadence_breach_version_key(result.get('exceeds_promised_cadence'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['cadence_breach_version_key'] = None
        result.setdefault('_erb_errors', {})['cadence_breach_version_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'overdue_version_key', 'cadence_breach_version_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# LEARNINGACTIVITIES
# Learning, retrospective, tabletop, and onboarding activities that convert execution experience into maintained knowledge. Explicit ERB-PKO extension represented as prov:Activity.
# =============================================================================

# Level 1

def calc_learning_activities_name(activity_kind, occurred_at):
    """
    Human-readable calculated display alias for the LearningActivities row.
    
    Formula: ={{ActivityKind}} & " / " & {{OccurredAt}}
    """
    return (str(activity_kind or "") + ' / ' + _erb.erb_timestamptz_text(occurred_at))


def compute_learning_activities_fields(record: dict) -> dict:
    """
    Compute all calculated fields for LearningActivities.
    
    Learning, retrospective, tabletop, and onboarding activities that convert execution experience into maintained knowledge. Explicit ERB-PKO extension represented as prov:Activity.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_learning_activities_name(result.get('activity_kind'), result.get('occurred_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# OPERATIONALBINDINGS
# Live bindings between procedural semantics and operational data/resources. Explicit ERB-PKO extension using DCAT/DCMI/PROV identifiers.
# =============================================================================

# Level 1

def calc_operational_bindings_name(step, record_or_schema_key):
    """
    Human-readable calculated display alias for the OperationalBindings row.
    
    Formula: ={{Step}} & " / " & {{RecordOrSchemaKey}}
    """
    return (str(step or "") + ' / ' + str(record_or_schema_key or ""))

def calc_operational_bindings_age_minutes(as_of_instant, last_observed_at):
    """
    Current observed age.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{LastObservedAt}}, "minutes")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, last_observed_at, 'minutes'))

# Level 2

def calc_operational_bindings_is_fresh(age_minutes, freshness_sla_minutes):
    """
    TRUE when within freshness SLA.
    
    Formula: ={{AgeMinutes}} <= {{FreshnessSlaMinutes}}
    """
    return _erb.erb_cmp(age_minutes, '<=', _erb.erb_nullif(freshness_sla_minutes))

# Level 3

def calc_operational_bindings_stale_binding_step_key(is_fresh, step):
    """
    Echoes the bound Step id only when the binding is outside its freshness SLA.
    
    Formula: =IF(NOT({{IsFresh}}), {{Step}}, "")
    """
    return (step if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fresh))) else '')

def calc_operational_bindings_authoritative_stale_step_key(is_fresh, is_authoritative, step):
    """
    Echoes the bound Step id only when an AUTHORITATIVE binding is stale.
    
    Formula: =IF(AND(NOT({{IsFresh}}), {{IsAuthoritative}}), {{Step}}, "")
    """
    return (step if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fresh))), (is_authoritative is True))) else '')

def calc_operational_bindings_is_stale_and_authoritative(is_authoritative, is_fresh):
    """
    TRUE when an authoritative binding has aged past its freshness SLA.
    
    Formula: =AND({{IsAuthoritative}}, NOT({{IsFresh}}))
    """
    return _erb.erb_and((is_authoritative is True), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_fresh))))

def calc_operational_bindings_is_usable_for_drafting(resource_is_approved, is_fresh):
    """
    TRUE when this binding points at an approved source that is still inside its freshness SLA.
    
    Formula: =AND({{ResourceIsApproved}}, {{IsFresh}})
    """
    return _erb.erb_and(_erb.erb_bool3(resource_is_approved), _erb.erb_bool3(is_fresh))

# Level 4

def calc_operational_bindings_step_when_stale(is_stale_and_authoritative, step):
    """
    Echoes the step id when this authoritative binding is stale, blank otherwise.
    
    Formula: =IF({{IsStaleAndAuthoritative}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(is_stale_and_authoritative) else '')

def calc_operational_bindings_step_when_unusable(is_usable_for_drafting, step):
    """
    Echoes the step id when this binding is NOT usable for drafting, blank otherwise.
    
    Formula: =IF({{IsUsableForDrafting}}, "", {{Step}})
    """
    return ('' if _erb.erb_bool3(is_usable_for_drafting) else step)


def compute_operational_bindings_fields(record: dict) -> dict:
    """
    Compute all calculated fields for OperationalBindings.
    
    Live bindings between procedural semantics and operational data/resources. Explicit ERB-PKO extension using DCAT/DCMI/PROV identifiers.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_operational_bindings_name(result.get('step'), result.get('record_or_schema_key'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['age_minutes'] = calc_operational_bindings_age_minutes(result.get('as_of_instant'), result.get('last_observed_at'))
    except Exception as _field_exc:
        result['age_minutes'] = None
        result.setdefault('_erb_errors', {})['age_minutes'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_fresh'] = calc_operational_bindings_is_fresh(result.get('age_minutes'), result.get('freshness_sla_minutes'))
    except Exception as _field_exc:
        result['is_fresh'] = None
        result.setdefault('_erb_errors', {})['is_fresh'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['stale_binding_step_key'] = calc_operational_bindings_stale_binding_step_key(result.get('is_fresh'), result.get('step'))
    except Exception as _field_exc:
        result['stale_binding_step_key'] = None
        result.setdefault('_erb_errors', {})['stale_binding_step_key'] = str(_field_exc)
    try:
        result['authoritative_stale_step_key'] = calc_operational_bindings_authoritative_stale_step_key(result.get('is_fresh'), result.get('is_authoritative'), result.get('step'))
    except Exception as _field_exc:
        result['authoritative_stale_step_key'] = None
        result.setdefault('_erb_errors', {})['authoritative_stale_step_key'] = str(_field_exc)
    try:
        result['is_stale_and_authoritative'] = calc_operational_bindings_is_stale_and_authoritative(result.get('is_authoritative'), result.get('is_fresh'))
    except Exception as _field_exc:
        result['is_stale_and_authoritative'] = None
        result.setdefault('_erb_errors', {})['is_stale_and_authoritative'] = str(_field_exc)
    try:
        result['is_usable_for_drafting'] = calc_operational_bindings_is_usable_for_drafting(result.get('resource_is_approved'), result.get('is_fresh'))
    except Exception as _field_exc:
        result['is_usable_for_drafting'] = None
        result.setdefault('_erb_errors', {})['is_usable_for_drafting'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['step_when_stale'] = calc_operational_bindings_step_when_stale(result.get('is_stale_and_authoritative'), result.get('step'))
    except Exception as _field_exc:
        result['step_when_stale'] = None
        result.setdefault('_erb_errors', {})['step_when_stale'] = str(_field_exc)
    try:
        result['step_when_unusable'] = calc_operational_bindings_step_when_unusable(result.get('is_usable_for_drafting'), result.get('step'))
    except Exception as _field_exc:
        result['step_when_unusable'] = None
        result.setdefault('_erb_errors', {})['step_when_unusable'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'stale_binding_step_key', 'authoritative_stale_step_key', 'step_when_stale', 'step_when_unusable']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# COMMUNICATIONPOLICIES
# Channel-specific communication policy projected from the same canonical procedure. Uses ODRL-style policy semantics plus ERB-PKO channel constraints.
# =============================================================================

# Level 1

def calc_communication_policies_name(channel, procedure_version):
    """
    Human-readable calculated display alias for the CommunicationPolicies row.
    
    Formula: ={{Channel}} & " policy / " & {{ProcedureVersion}}
    """
    return (str(channel or "") + ' policy / ' + str(procedure_version or ""))

def calc_communication_policies_is_active_policy(status):
    """
    TRUE only when this channel policy is in Active status.
    
    Formula: ={{Status}} = "Active" 
    """
    return _erb.erb_eq(_erb.erb_nullif(status), 'Active')


def compute_communication_policies_fields(record: dict) -> dict:
    """
    Compute all calculated fields for CommunicationPolicies.
    
    Channel-specific communication policy projected from the same canonical procedure. Uses ODRL-style policy semantics plus ERB-PKO channel constraints.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_communication_policies_name(result.get('channel'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_active_policy'] = calc_communication_policies_is_active_policy(result.get('status'))
    except Exception as _field_exc:
        result['is_active_policy'] = None
        result.setdefault('_erb_errors', {})['is_active_policy'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# MESSAGETEMPLATES
# Approved channel templates projected from the canonical rulebook without becoming a second source of policy meaning.
# =============================================================================

# Level 1

def calc_message_templates_name(communication_policy, locale):
    """
    Human-readable calculated display alias for the MessageTemplates row.
    
    Formula: ={{CommunicationPolicy}} & " / " & {{Locale}}
    """
    return (str(communication_policy or "") + ' / ' + str(locale or ""))

def calc_message_templates_body_template_length(body_template):
    """
    Character length of the raw template body before variable substitution.
    
    Formula: =LEN({{BodyTemplate}})
    """
    return _erb.erb_integer(len(body_template or ""))

def calc_message_templates_has_valid_approval(valid_approval_count):
    """
    TRUE when at least one properly-authorized approval exists for this template.
    
    Formula: ={{ValidApprovalCount}} > 0
    """
    return _erb.erb_cmp(valid_approval_count, '>', 0)

def calc_message_templates_has_body_drifted(last_approved_body_hash, current_body_hash):
    """
    TRUE when the template body no longer matches what was approved.
    
    Formula: =AND({{LastApprovedBodyHash}} <> "", {{CurrentBodyHash}} <> {{LastApprovedBodyHash}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (last_approved_body_hash is None or last_approved_body_hash == ""))), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(current_body_hash), last_approved_body_hash)))

def calc_message_templates_template_draws_no_response(transmitted_delivery_count, unanswered_delivery_count):
    """
    TRUE when every transmitted message from this template went unacknowledged.
    
    Formula: =AND({{TransmittedDeliveryCount}} > 0, {{UnansweredDeliveryCount}} = {{TransmittedDeliveryCount}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(transmitted_delivery_count, '>', 0)), _erb.erb_bool3(_erb.erb_eq(unanswered_delivery_count, transmitted_delivery_count)))

# Level 2

def calc_message_templates_is_template_over_length(body_template_length, policy_max_message_length):
    """
    TRUE when the template body alone already exceeds the single-segment limit before any variables are substituted in.
    
    Formula: ={{BodyTemplateLength}} > {{PolicyMaxMessageLength}}
    """
    return _erb.erb_cmp(body_template_length, '>', policy_max_message_length)

def calc_message_templates_is_claiming_unbacked_approval(status, has_valid_approval):
    """
    TRUE when a template's Status says Approved but no properly-authorized approval record backs it. The phantom-approval witness.
    
    Formula: =AND({{Status}} = "Approved", NOT({{HasValidApproval}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_valid_approval))))

def calc_message_templates_is_sendable_under_approval(status, has_valid_approval, has_body_drifted):
    """
    TRUE only when the template is marked Approved, has a properly-authorized approval, AND its body still matches what was approved.
    
    Formula: =AND({{Status}} = "Approved", AND({{HasValidApproval}}, NOT({{HasBodyDrifted}})))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_valid_approval), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_body_drifted))))))


def compute_message_templates_fields(record: dict) -> dict:
    """
    Compute all calculated fields for MessageTemplates.
    
    Approved channel templates projected from the canonical rulebook without becoming a second source of policy meaning.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_message_templates_name(result.get('communication_policy'), result.get('locale'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['body_template_length'] = calc_message_templates_body_template_length(result.get('body_template'))
    except Exception as _field_exc:
        result['body_template_length'] = None
        result.setdefault('_erb_errors', {})['body_template_length'] = str(_field_exc)
    try:
        result['has_valid_approval'] = calc_message_templates_has_valid_approval(result.get('valid_approval_count'))
    except Exception as _field_exc:
        result['has_valid_approval'] = None
        result.setdefault('_erb_errors', {})['has_valid_approval'] = str(_field_exc)
    try:
        result['has_body_drifted'] = calc_message_templates_has_body_drifted(result.get('last_approved_body_hash'), result.get('current_body_hash'))
    except Exception as _field_exc:
        result['has_body_drifted'] = None
        result.setdefault('_erb_errors', {})['has_body_drifted'] = str(_field_exc)
    try:
        result['template_draws_no_response'] = calc_message_templates_template_draws_no_response(result.get('transmitted_delivery_count'), result.get('unanswered_delivery_count'))
    except Exception as _field_exc:
        result['template_draws_no_response'] = None
        result.setdefault('_erb_errors', {})['template_draws_no_response'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_template_over_length'] = calc_message_templates_is_template_over_length(result.get('body_template_length'), result.get('policy_max_message_length'))
    except Exception as _field_exc:
        result['is_template_over_length'] = None
        result.setdefault('_erb_errors', {})['is_template_over_length'] = str(_field_exc)
    try:
        result['is_claiming_unbacked_approval'] = calc_message_templates_is_claiming_unbacked_approval(result.get('status'), result.get('has_valid_approval'))
    except Exception as _field_exc:
        result['is_claiming_unbacked_approval'] = None
        result.setdefault('_erb_errors', {})['is_claiming_unbacked_approval'] = str(_field_exc)
    try:
        result['is_sendable_under_approval'] = calc_message_templates_is_sendable_under_approval(result.get('status'), result.get('has_valid_approval'), result.get('has_body_drifted'))
    except Exception as _field_exc:
        result['is_sendable_under_approval'] = None
        result.setdefault('_erb_errors', {})['is_sendable_under_approval'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# SEMANTICMAPPINGS
# Machine-readable alignment from ERB table/field paths to exact PKO or reused ontology terms. Extension mappings are never presented as native PKO.
# =============================================================================

# Level 1

def calc_semantic_mappings_name(source_path, target_iri):
    """
    Human-readable calculated display alias for the SemanticMappings row.
    
    Formula: ={{SourcePath}} & " -> " & {{TargetIri}}
    """
    return (str(source_path or "") + ' -> ' + str(target_iri or ""))


def compute_semantic_mappings_fields(record: dict) -> dict:
    """
    Compute all calculated fields for SemanticMappings.
    
    Machine-readable alignment from ERB table/field paths to exact PKO or reused ontology terms. Extension mappings are never presented as native PKO.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_semantic_mappings_name(result.get('source_path'), result.get('target_iri'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# WITNESSLOOPS
# One row per role-question expansion loop. Each loop poses questions that only became askable because of the previous loop's predicates.
# =============================================================================

# Level 1

def calc_witness_loops_name(loop_number, title):
    """
    Human-readable calculated display alias for the WitnessLoops row.
    
    Formula: ="Loop " & {{LoopNumber}} & ": " & {{Title}}
    """
    return ('Loop ' + str(loop_number or "") + ': ' + str(title or ""))

def calc_witness_loops_is_complete(completed_at):
    """
    TRUE once the loop has been committed.
    
    Formula: ={{CompletedAt}} <> "" 
    """
    return (not (completed_at is None or completed_at == ""))


def compute_witness_loops_fields(record: dict) -> dict:
    """
    Compute all calculated fields for WitnessLoops.
    
    One row per role-question expansion loop. Each loop poses questions that only became askable because of the previous loop's predicates.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_witness_loops_name(result.get('loop_number'), result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_complete'] = calc_witness_loops_is_complete(result.get('completed_at'))
    except Exception as _field_exc:
        result['is_complete'] = None
        result.setdefault('_erb_errors', {})['is_complete'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLEQUESTIONS
# One row per question a named role wants answered. Every invented predicate in this rulebook traces back to one of these.
# =============================================================================

# Level 1

def calc_role_questions_name(asking_role, question_text):
    """
    Human-readable calculated display alias for the RoleQuestions row.
    
    Formula: ={{AskingRole}} & ": " & LEFT({{QuestionText}}, 60)
    """
    return (str(asking_role or "") + ': ' + str(((question_text or "")[:(60 or 0)]) if ((question_text or "")[:(60 or 0)]) is not None else ""))

def calc_role_questions_is_answered(predicate_count):
    """
    TRUE when at least one predicate exists to answer this question.
    
    Formula: ={{PredicateCount}} > 0
    """
    return _erb.erb_cmp(predicate_count, '>', 0)


def compute_role_questions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleQuestions.
    
    One row per question a named role wants answered. Every invented predicate in this rulebook traces back to one of these.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_role_questions_name(result.get('asking_role'), result.get('question_text'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_answered'] = calc_role_questions_is_answered(result.get('predicate_count'))
    except Exception as _field_exc:
        result['is_answered'] = None
        result.setdefault('_erb_errors', {})['is_answered'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# RULEBOOKFIELDS
# A complete census of every field in this rulebook. Reconciled from the real schemas by tools/reconcile_field_catalog.py — never hand-maintained. Fields invented by a witness loop carry an InventedForQuestion FK.
# =============================================================================

# Level 1

def calc_rulebook_fields_name(target_table, field_name):
    """
    Human-readable calculated display alias for the RulebookFields row.
    
    Formula: ={{TargetTable}} & "." & {{FieldName}}
    """
    return (str(target_table or "") + '.' + str(field_name or ""))

def calc_rulebook_fields_is_derived(field_type):
    """
    TRUE when this field is computed rather than stored.
    
    Formula: =OR({{FieldType}} = "calculated", {{FieldType}} = "lookup", {{FieldType}} = "aggregation")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(field_type), 'calculated')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(field_type), 'lookup')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(field_type), 'aggregation')))

def calc_rulebook_fields_is_witness(invented_for_question):
    """
    TRUE when this field exists because a role asked a question. These are the fields the witness loops added.
    
    Formula: ={{InventedForQuestion}} <> "" 
    """
    return (not (invented_for_question is None or invented_for_question == ""))


def compute_rulebook_fields_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RulebookFields.
    
    A complete census of every field in this rulebook. Reconciled from the real schemas by tools/reconcile_field_catalog.py — never hand-maintained. Fields invented by a witness loop carry an InventedForQuestion FK.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_rulebook_fields_name(result.get('target_table'), result.get('field_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_derived'] = calc_rulebook_fields_is_derived(result.get('field_type'))
    except Exception as _field_exc:
        result['is_derived'] = None
        result.setdefault('_erb_errors', {})['is_derived'] = str(_field_exc)
    try:
        result['is_witness'] = calc_rulebook_fields_is_witness(result.get('invented_for_question'))
    except Exception as _field_exc:
        result['is_witness'] = None
        result.setdefault('_erb_errors', {})['is_witness'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# TESTSUITES
# Groups of conformance checks. Rollups here are computed from TestCases, so the board's headline is itself a derived field.
# =============================================================================

# Level 1

def calc_test_suites_name(label):
    """
    Human-readable calculated display alias for the TestSuites row.
    
    Formula: ={{Label}}
    """
    return label

def calc_test_suites_is_green(blocking_fail_count):
    """
    TRUE when no blocking check is failing. Advisory warnings do not break the board.
    
    Formula: ={{BlockingFailCount}} = 0
    """
    return _erb.erb_eq(blocking_fail_count, 0)


def compute_test_suites_fields(record: dict) -> dict:
    """
    Compute all calculated fields for TestSuites.
    
    Groups of conformance checks. Rollups here are computed from TestCases, so the board's headline is itself a derived field.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_test_suites_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_green'] = calc_test_suites_is_green(result.get('blocking_fail_count'))
    except Exception as _field_exc:
        result['is_green'] = None
        result.setdefault('_erb_errors', {})['is_green'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# TESTCASES
# The conformance suite, as data. One row per check, naming what it checks and — where applicable — the role question whose answer it defends. tools/run_test_suite.py executes these rows and writes the outcome back; it invents no checks of its own.
# =============================================================================

# Level 1

def calc_test_cases_name(test_kind, subject):
    """
    Human-readable calculated display alias for the TestCases row.
    
    Formula: ={{TestKind}} & ": " & {{Subject}}
    """
    return (str(test_kind or "") + ': ' + str(subject or ""))

def calc_test_cases_is_blocking(severity):
    """
    TRUE when a failure of this check means the model is asserting something false.
    
    Formula: ={{Severity}} = "blocking" 
    """
    return _erb.erb_eq(_erb.erb_nullif(severity), 'blocking')

def calc_test_cases_is_passing(last_outcome):
    """
    TRUE when the last run passed outright.
    
    Formula: ={{LastOutcome}} = "PASS" 
    """
    return _erb.erb_eq(_erb.erb_nullif(last_outcome), 'PASS')

def calc_test_cases_is_failing(last_outcome):
    """
    TRUE when the last run failed. A blocking failure means the model is asserting something false.
    
    Formula: ={{LastOutcome}} = "FAIL" 
    """
    return _erb.erb_eq(_erb.erb_nullif(last_outcome), 'FAIL')

# Level 2

def calc_test_cases_needs_attention(is_failing, is_blocking):
    """
    TRUE when this check failed and its failure means the model is wrong. This is the number that must be zero.
    
    Formula: =AND({{IsFailing}}, {{IsBlocking}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_failing), _erb.erb_bool3(is_blocking))

def calc_test_cases_passing_suite_key(is_passing, suite):
    """
    Echoes the suite id only for checks that passed; empty otherwise. Single-criterion COUNTIFS key.
    
    Formula: =IF({{IsPassing}}, {{Suite}}, "")
    """
    return (suite if _erb.erb_bool3(is_passing) else '')

# Level 3

def calc_test_cases_needs_attention_suite_key(needs_attention, suite):
    """
    Echoes the suite id only for blocking checks that failed; empty otherwise. Single-criterion COUNTIFS key.
    
    Formula: =IF({{NeedsAttention}}, {{Suite}}, "")
    """
    return (suite if _erb.erb_bool3(needs_attention) else '')


def compute_test_cases_fields(record: dict) -> dict:
    """
    Compute all calculated fields for TestCases.
    
    The conformance suite, as data. One row per check, naming what it checks and — where applicable — the role question whose answer it defends. tools/run_test_suite.py executes these rows and writes the outcome back; it invents no checks of its own.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_test_cases_name(result.get('test_kind'), result.get('subject'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_blocking'] = calc_test_cases_is_blocking(result.get('severity'))
    except Exception as _field_exc:
        result['is_blocking'] = None
        result.setdefault('_erb_errors', {})['is_blocking'] = str(_field_exc)
    try:
        result['is_passing'] = calc_test_cases_is_passing(result.get('last_outcome'))
    except Exception as _field_exc:
        result['is_passing'] = None
        result.setdefault('_erb_errors', {})['is_passing'] = str(_field_exc)
    try:
        result['is_failing'] = calc_test_cases_is_failing(result.get('last_outcome'))
    except Exception as _field_exc:
        result['is_failing'] = None
        result.setdefault('_erb_errors', {})['is_failing'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['needs_attention'] = calc_test_cases_needs_attention(result.get('is_failing'), result.get('is_blocking'))
    except Exception as _field_exc:
        result['needs_attention'] = None
        result.setdefault('_erb_errors', {})['needs_attention'] = str(_field_exc)
    try:
        result['passing_suite_key'] = calc_test_cases_passing_suite_key(result.get('is_passing'), result.get('suite'))
    except Exception as _field_exc:
        result['passing_suite_key'] = None
        result.setdefault('_erb_errors', {})['passing_suite_key'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['needs_attention_suite_key'] = calc_test_cases_needs_attention_suite_key(result.get('needs_attention'), result.get('suite'))
    except Exception as _field_exc:
        result['needs_attention_suite_key'] = None
        result.setdefault('_erb_errors', {})['needs_attention_suite_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'passing_suite_key', 'needs_attention_suite_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ERBVERSIONS
# Standard ERB semantic version history.
# =============================================================================


def compute_erb_versions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ERBVersions.
    
    Standard ERB semantic version history.
    """
    result = dict(record)

    return result

# =============================================================================
# ERBCUSTOMIZATIONS
# Explicit customization seams; empty because the canonical model is expressed in the rulebook.
# =============================================================================


def compute_erb_customizations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ERBCustomizations.
    
    Explicit customization seams; empty because the canonical model is expressed in the rulebook.
    """
    result = dict(record)

    return result

# =============================================================================
# __META__
# Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, PKO version contract, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
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
    
    Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, PKO version contract, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
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
# EXCEPTIONINVOCATIONS
# ExceptionInvocations (added by witness loop 1).
# =============================================================================

# Level 1

def calc_exception_invocations_name(step_execution, exception):
    """
    Human-readable calculated display alias.
    
    Formula: ={{StepExecution}} & " / " & {{Exception}}
    """
    return (str(step_execution or "") + ' / ' + str(exception or ""))

def calc_exception_invocations_approval_role_matches(approved_by_agent, required_approval_role_holder):
    """
    TRUE when the agent who approved is the holder of the role the exception requires.
    
    Formula: ={{ApprovedByAgent}} = {{RequiredApprovalRoleHolder}}
    """
    return _erb.erb_eq(_erb.erb_nullif(approved_by_agent), required_approval_role_holder)

def calc_exception_invocations_is_approved(approved_by_agent):
    """
    TRUE when an approver was recorded at all.
    
    Formula: ={{ApprovedByAgent}} <> "" 
    """
    return (not (approved_by_agent is None or approved_by_agent == ""))

def calc_exception_invocations_invoker_also_prepared_key(parent_procedure_execution, approved_by_agent):
    """
    Composite execution+approver key, in the same key space as StepExecutions.PreparerAgentKey.
    
    Formula: ={{ParentProcedureExecution}} & "|" & {{ApprovedByAgent}}
    """
    return (str(parent_procedure_execution or "") + '|' + str(approved_by_agent or ""))

def calc_exception_invocations_delegated_to_preparer(approver_prepared_count):
    """
    TRUE when an exception routed approval authority to an agent who prepared work in the same execution.
    
    Formula: ={{ApproverPreparedCount}} > 0
    """
    return _erb.erb_cmp(approver_prepared_count, '>', 0)

# Level 2

def calc_exception_invocations_is_improperly_approved(is_approved, approval_role_matches):
    """
    TRUE when an exception was invoked without an approver, or approved by an agent who does not hold the required role.
    
    Formula: =OR(NOT({{IsApproved}}), NOT({{ApprovalRoleMatches}}))
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_approved))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(approval_role_matches))))

# Level 3

def calc_exception_invocations_is_ungoverned_invocation(is_improperly_approved, delegated_to_preparer):
    """
    TRUE when an exception was invoked without proper role authority, or routed authority to the preparer.
    
    Formula: =OR({{IsImproperlyApproved}}, {{DelegatedToPreparer}})
    """
    return _erb.erb_or(_erb.erb_bool3(is_improperly_approved), _erb.erb_bool3(delegated_to_preparer))


def compute_exception_invocations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ExceptionInvocations.
    
    ExceptionInvocations (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_exception_invocations_name(result.get('step_execution'), result.get('exception'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['approval_role_matches'] = calc_exception_invocations_approval_role_matches(result.get('approved_by_agent'), result.get('required_approval_role_holder'))
    except Exception as _field_exc:
        result['approval_role_matches'] = None
        result.setdefault('_erb_errors', {})['approval_role_matches'] = str(_field_exc)
    try:
        result['is_approved'] = calc_exception_invocations_is_approved(result.get('approved_by_agent'))
    except Exception as _field_exc:
        result['is_approved'] = None
        result.setdefault('_erb_errors', {})['is_approved'] = str(_field_exc)
    try:
        result['invoker_also_prepared_key'] = calc_exception_invocations_invoker_also_prepared_key(result.get('parent_procedure_execution'), result.get('approved_by_agent'))
    except Exception as _field_exc:
        result['invoker_also_prepared_key'] = None
        result.setdefault('_erb_errors', {})['invoker_also_prepared_key'] = str(_field_exc)
    try:
        result['delegated_to_preparer'] = calc_exception_invocations_delegated_to_preparer(result.get('approver_prepared_count'))
    except Exception as _field_exc:
        result['delegated_to_preparer'] = None
        result.setdefault('_erb_errors', {})['delegated_to_preparer'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_improperly_approved'] = calc_exception_invocations_is_improperly_approved(result.get('is_approved'), result.get('approval_role_matches'))
    except Exception as _field_exc:
        result['is_improperly_approved'] = None
        result.setdefault('_erb_errors', {})['is_improperly_approved'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_ungoverned_invocation'] = calc_exception_invocations_is_ungoverned_invocation(result.get('is_improperly_approved'), result.get('delegated_to_preparer'))
    except Exception as _field_exc:
        result['is_ungoverned_invocation'] = None
        result.setdefault('_erb_errors', {})['is_ungoverned_invocation'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'invoker_also_prepared_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# VERIFICATIONOUTCOMES
# VerificationOutcomes (added by witness loop 1).
# =============================================================================

# Level 1

def calc_verification_outcomes_name(step_execution, step_verification):
    """
    Human-readable calculated display alias.
    
    Formula: ={{StepExecution}} & " / " & {{StepVerification}}
    """
    return (str(step_execution or "") + ' / ' + str(step_verification or ""))

def calc_verification_outcomes_signal_matches_expected(observed_signal_value, expected_signal_value):
    """
    TRUE when the observed value equals the expected value.
    
    Formula: ={{ObservedSignalValue}} = {{ExpectedSignalValue}}
    """
    return _erb.erb_eq(_erb.erb_nullif(observed_signal_value), expected_signal_value)

def calc_verification_outcomes_has_evidence(evidence_uri):
    """
    TRUE when a retained artifact backs this observation.
    
    Formula: ={{EvidenceUri}} <> "" 
    """
    return (not (evidence_uri is None or evidence_uri == ""))

def calc_verification_outcomes_is_self_witnessed(observed_by_agent, step_executor_agent):
    """
    TRUE when the agent who observed the verification signal is the same agent who executed the step being verified.
    
    Formula: ={{ObservedByAgent}} = {{StepExecutorAgent}}
    """
    return _erb.erb_eq(_erb.erb_nullif(observed_by_agent), step_executor_agent)

# Level 2

def calc_verification_outcomes_is_unbacked_observation(signal_matches_expected, has_evidence):
    """
    TRUE when a verification was recorded as matching but no evidence artifact was retained.
    
    Formula: =AND({{SignalMatchesExpected}}, NOT({{HasEvidence}}))
    """
    return _erb.erb_and(_erb.erb_bool3(signal_matches_expected), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_evidence))))

def calc_verification_outcomes_self_witnessed_step_key(is_self_witnessed, step_execution):
    """
    Echoes the StepExecution id only for self-witnessed verifications.
    
    Formula: =IF({{IsSelfWitnessed}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_self_witnessed) else '')

def calc_verification_outcomes_is_self_witnessed_and_unbacked(is_self_witnessed, has_evidence):
    """
    TRUE when the executor observed their own passing signal and attached no evidence.
    
    Formula: =AND({{IsSelfWitnessed}}, NOT({{HasEvidence}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_self_witnessed), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_evidence))))

def calc_verification_outcomes_observer_is_independent_of_executor(is_self_witnessed):
    """
    TRUE when someone other than the step's executor recorded the observation.
    
    Formula: =NOT({{IsSelfWitnessed}})
    """
    return _erb.erb_not(_erb.erb_bool3(is_self_witnessed))

def calc_verification_outcomes_is_independent_human_observation(observer_is_non_human, is_self_witnessed, has_evidence):
    """
    TRUE when a human other than the step's executor observed this signal and attached evidence.
    
    Formula: =AND(NOT({{ObserverIsNonHuman}}), NOT({{IsSelfWitnessed}}), {{HasEvidence}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(observer_is_non_human))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_self_witnessed))), _erb.erb_bool3(has_evidence))

# Level 3

def calc_verification_outcomes_unbacked_step_key(is_unbacked_observation, step_execution):
    """
    Echoes the StepExecution id when a verification matched but retained no evidence.
    
    Formula: =IF({{IsUnbackedObservation}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_unbacked_observation) else '')

def calc_verification_outcomes_is_uncorroborated_pass(signal_matches_expected, is_self_witnessed_and_unbacked):
    """
    TRUE when a PASSING signal was self-observed with no evidence behind it.
    
    Formula: =AND({{SignalMatchesExpected}}, {{IsSelfWitnessedAndUnbacked}})
    """
    return _erb.erb_and(_erb.erb_bool3(signal_matches_expected), _erb.erb_bool3(is_self_witnessed_and_unbacked))

def calc_verification_outcomes_independent_observation_execution_key(is_independent_human_observation, parent_procedure_execution_of_outcome):
    """
    Echoes the parent execution id when this is an independent human observation.
    
    Formula: =IF({{IsIndependentHumanObservation}}, {{ParentProcedureExecutionOfOutcome}}, "")
    """
    return (parent_procedure_execution_of_outcome if _erb.erb_bool3(is_independent_human_observation) else '')

# Level 4

def calc_verification_outcomes_uncorroborated_pass_step_key(is_uncorroborated_pass, step_execution):
    """
    Echoes the step execution id when this outcome is an uncorroborated pass.
    
    Formula: =IF({{IsUncorroboratedPass}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_uncorroborated_pass) else '')


def compute_verification_outcomes_fields(record: dict) -> dict:
    """
    Compute all calculated fields for VerificationOutcomes.
    
    VerificationOutcomes (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_verification_outcomes_name(result.get('step_execution'), result.get('step_verification'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['signal_matches_expected'] = calc_verification_outcomes_signal_matches_expected(result.get('observed_signal_value'), result.get('expected_signal_value'))
    except Exception as _field_exc:
        result['signal_matches_expected'] = None
        result.setdefault('_erb_errors', {})['signal_matches_expected'] = str(_field_exc)
    try:
        result['has_evidence'] = calc_verification_outcomes_has_evidence(result.get('evidence_uri'))
    except Exception as _field_exc:
        result['has_evidence'] = None
        result.setdefault('_erb_errors', {})['has_evidence'] = str(_field_exc)
    try:
        result['is_self_witnessed'] = calc_verification_outcomes_is_self_witnessed(result.get('observed_by_agent'), result.get('step_executor_agent'))
    except Exception as _field_exc:
        result['is_self_witnessed'] = None
        result.setdefault('_erb_errors', {})['is_self_witnessed'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_unbacked_observation'] = calc_verification_outcomes_is_unbacked_observation(result.get('signal_matches_expected'), result.get('has_evidence'))
    except Exception as _field_exc:
        result['is_unbacked_observation'] = None
        result.setdefault('_erb_errors', {})['is_unbacked_observation'] = str(_field_exc)
    try:
        result['self_witnessed_step_key'] = calc_verification_outcomes_self_witnessed_step_key(result.get('is_self_witnessed'), result.get('step_execution'))
    except Exception as _field_exc:
        result['self_witnessed_step_key'] = None
        result.setdefault('_erb_errors', {})['self_witnessed_step_key'] = str(_field_exc)
    try:
        result['is_self_witnessed_and_unbacked'] = calc_verification_outcomes_is_self_witnessed_and_unbacked(result.get('is_self_witnessed'), result.get('has_evidence'))
    except Exception as _field_exc:
        result['is_self_witnessed_and_unbacked'] = None
        result.setdefault('_erb_errors', {})['is_self_witnessed_and_unbacked'] = str(_field_exc)
    try:
        result['observer_is_independent_of_executor'] = calc_verification_outcomes_observer_is_independent_of_executor(result.get('is_self_witnessed'))
    except Exception as _field_exc:
        result['observer_is_independent_of_executor'] = None
        result.setdefault('_erb_errors', {})['observer_is_independent_of_executor'] = str(_field_exc)
    try:
        result['is_independent_human_observation'] = calc_verification_outcomes_is_independent_human_observation(result.get('observer_is_non_human'), result.get('is_self_witnessed'), result.get('has_evidence'))
    except Exception as _field_exc:
        result['is_independent_human_observation'] = None
        result.setdefault('_erb_errors', {})['is_independent_human_observation'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['unbacked_step_key'] = calc_verification_outcomes_unbacked_step_key(result.get('is_unbacked_observation'), result.get('step_execution'))
    except Exception as _field_exc:
        result['unbacked_step_key'] = None
        result.setdefault('_erb_errors', {})['unbacked_step_key'] = str(_field_exc)
    try:
        result['is_uncorroborated_pass'] = calc_verification_outcomes_is_uncorroborated_pass(result.get('signal_matches_expected'), result.get('is_self_witnessed_and_unbacked'))
    except Exception as _field_exc:
        result['is_uncorroborated_pass'] = None
        result.setdefault('_erb_errors', {})['is_uncorroborated_pass'] = str(_field_exc)
    try:
        result['independent_observation_execution_key'] = calc_verification_outcomes_independent_observation_execution_key(result.get('is_independent_human_observation'), result.get('parent_procedure_execution_of_outcome'))
    except Exception as _field_exc:
        result['independent_observation_execution_key'] = None
        result.setdefault('_erb_errors', {})['independent_observation_execution_key'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['uncorroborated_pass_step_key'] = calc_verification_outcomes_uncorroborated_pass_step_key(result.get('is_uncorroborated_pass'), result.get('step_execution'))
    except Exception as _field_exc:
        result['uncorroborated_pass_step_key'] = None
        result.setdefault('_erb_errors', {})['uncorroborated_pass_step_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'self_witnessed_step_key', 'unbacked_step_key', 'uncorroborated_pass_step_key', 'independent_observation_execution_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# OBSERVEDTRANSITIONS
# The proxy above cannot tell a walked fallback from a happy-path step that happens to share an endpoint. PKO models Transition as a first-class thing; its execution counterpart is missing. Without a table that records WHICH transition a step execution arrived by, 'has this fallback ever been walked' is permanently unanswerable rather than merely unanswered. This is an extension (urn:effortless:pko-extension#ObservedTransition), not a native PKO term.
# =============================================================================

# Level 1

def calc_observed_transitions_name(step_transition, observed_at):
    """
    Human-readable calculated display alias.
    
    Formula: ={{StepTransition}} & " @ " & {{ObservedAt}}
    """
    return (str(step_transition or "") + ' @ ' + _erb.erb_timestamptz_text(observed_at))


def compute_observed_transitions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ObservedTransitions.
    
    The proxy above cannot tell a walked fallback from a happy-path step that happens to share an endpoint. PKO models Transition as a first-class thing; its execution counterpart is missing. Without a table that records WHICH transition a step execution arrived by, 'has this fallback ever been walked' is permanently unanswerable rather than merely unanswered. This is an extension (urn:effortless:pko-extension#ObservedTransition), not a native PKO term.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_observed_transitions_name(result.get('step_transition'), result.get('observed_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# RECIPIENTS
# Recipients (added by witness loop 1).
# =============================================================================

# Level 1

def calc_recipients_name(display_name):
    """
    Human-readable calculated display alias for the Recipients row.
    
    Formula: ={{DisplayName}}
    """
    return display_name

def calc_recipients_has_sms_consent(sms_consent_status):
    """
    TRUE only when the recipient's SMS consent state is Granted.
    
    Formula: ={{SmsConsentStatus}} = "Granted" 
    """
    return _erb.erb_eq(_erb.erb_nullif(sms_consent_status), 'Granted')

def calc_recipients_is_email_reachable(email_address):
    """
    TRUE when a corporate email address is on file for this recipient.
    
    Formula: ={{EmailAddress}} <> "" 
    """
    return (not (email_address is None or email_address == ""))

def calc_recipients_is_sms_reachable(mobile_number):
    """
    TRUE when a mobile number is on file for this recipient.
    
    Formula: ={{MobileNumber}} <> "" 
    """
    return (not (mobile_number is None or mobile_number == ""))

# Level 2

def calc_recipients_is_unreachable(is_email_reachable, is_sms_reachable):
    """
    TRUE when the recipient has neither an email address nor a mobile number -- the exc-unreachable trigger condition.
    
    Formula: =AND(NOT({{IsEmailReachable}}), NOT({{IsSmsReachable}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_email_reachable))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_sms_reachable))))

def calc_recipients_is_communicationally_stranded(is_sms_reachable, is_email_reachable):
    """
    TRUE when every channel we hold for this person is either non-consenting or unreachable, so no lawful route exists at all.
    
    Formula: =AND(NOT({{IsSmsReachable}}), NOT({{IsEmailReachable}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_sms_reachable))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_email_reachable))))


def compute_recipients_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Recipients.
    
    Recipients (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_recipients_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_sms_consent'] = calc_recipients_has_sms_consent(result.get('sms_consent_status'))
    except Exception as _field_exc:
        result['has_sms_consent'] = None
        result.setdefault('_erb_errors', {})['has_sms_consent'] = str(_field_exc)
    try:
        result['is_email_reachable'] = calc_recipients_is_email_reachable(result.get('email_address'))
    except Exception as _field_exc:
        result['is_email_reachable'] = None
        result.setdefault('_erb_errors', {})['is_email_reachable'] = str(_field_exc)
    try:
        result['is_sms_reachable'] = calc_recipients_is_sms_reachable(result.get('mobile_number'))
    except Exception as _field_exc:
        result['is_sms_reachable'] = None
        result.setdefault('_erb_errors', {})['is_sms_reachable'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_unreachable'] = calc_recipients_is_unreachable(result.get('is_email_reachable'), result.get('is_sms_reachable'))
    except Exception as _field_exc:
        result['is_unreachable'] = None
        result.setdefault('_erb_errors', {})['is_unreachable'] = str(_field_exc)
    try:
        result['is_communicationally_stranded'] = calc_recipients_is_communicationally_stranded(result.get('is_sms_reachable'), result.get('is_email_reachable'))
    except Exception as _field_exc:
        result['is_communicationally_stranded'] = None
        result.setdefault('_erb_errors', {})['is_communicationally_stranded'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# MESSAGEDELIVERIES
# MessageDeliveries (added by witness loop 1).
# =============================================================================

# Level 1

def calc_message_deliveries_name(recipient, message_template, sent_at):
    """
    Human-readable calculated display alias for the MessageDeliveries row.
    
    Formula: ={{Recipient}} & " / " & {{MessageTemplate}} & " / " & {{SentAt}}
    """
    return (str(recipient or "") + ' / ' + str(message_template or "") + ' / ' + _erb.erb_timestamptz_text(sent_at))

def calc_message_deliveries_was_actually_transmitted(delivery_status):
    """
    TRUE when the message actually left our systems. Suppressed and Failed messages never reached the carrier.
    
    Formula: =OR({{DeliveryStatus}} = "Sent", OR({{DeliveryStatus}} = "Delivered", {{DeliveryStatus}} = "Bounced"))
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(delivery_status), 'Sent')), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(delivery_status), 'Delivered')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(delivery_status), 'Bounced')))))

def calc_message_deliveries_policy_has_quiet_hours(policy_quiet_hours_start_hour, policy_quiet_hours_end_hour):
    """
    TRUE when the governing policy actually declares a quiet-hours window. comm-email-policy has 00:00-00:00, meaning no window.
    
    Formula: ={{PolicyQuietHoursStartHour}} <> {{PolicyQuietHoursEndHour}}
    """
    return _erb.erb_ne(policy_quiet_hours_start_hour, policy_quiet_hours_end_hour)

def calc_message_deliveries_quiet_window_wraps_midnight(policy_quiet_hours_start_hour, policy_quiet_hours_end_hour):
    """
    TRUE when the quiet window crosses midnight (20:00 -> 08:00 does).
    
    Formula: ={{PolicyQuietHoursStartHour}} > {{PolicyQuietHoursEndHour}}
    """
    return _erb.erb_cmp(policy_quiet_hours_start_hour, '>', policy_quiet_hours_end_hour)

def calc_message_deliveries_is_acknowledged(acknowledged_at):
    """
    TRUE when an acknowledgement timestamp is recorded for this delivery.
    
    Formula: ={{AcknowledgedAt}} <> "" 
    """
    return (not (acknowledged_at is None or acknowledged_at == ""))

def calc_message_deliveries_has_unreachable_exception_invoked(invoked_exception):
    """
    TRUE when the documented unreachable-recipient exception was formally invoked on this delivery.
    
    Formula: ={{InvokedException}} = "exc-unreachable" 
    """
    return _erb.erb_eq(_erb.erb_nullif(invoked_exception), 'exc-unreachable')

def calc_message_deliveries_age_days(as_of_instant, sent_at):
    """
    Days elapsed since transmission.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, sent_at, 'days'))

def calc_message_deliveries_has_rendered_body(rendered_body):
    """
    TRUE when the exact transmitted text is still held on this record.
    
    Formula: ={{RenderedBody}} <> "" 
    """
    return (not (rendered_body is None or rendered_body == ""))

def calc_message_deliveries_rendered_body_length(rendered_body):
    """
    Character length of the exact text that was actually transmitted.
    
    Formula: =LEN({{RenderedBody}})
    """
    return _erb.erb_integer(len(rendered_body or ""))

def calc_message_deliveries_policy_requires_opt_out(policy_required_opt_out_phrase):
    """
    TRUE when the governing channel policy declares a required opt-out phrase at all.
    
    Formula: ={{PolicyRequiredOptOutPhrase}} <> "" 
    """
    return (not (policy_required_opt_out_phrase is None or policy_required_opt_out_phrase == ""))

def calc_message_deliveries_opt_out_phrase_position(policy_required_opt_out_phrase, rendered_body):
    """
    Character position at which the required opt-out phrase appears in the transmitted text; 0 when absent.
    
    Formula: =FIND({{PolicyRequiredOptOutPhrase}}, {{RenderedBody}})
    """
    return _erb.erb_integer(_erb.erb_find(policy_required_opt_out_phrase, _erb.erb_nullif(rendered_body)))

def calc_message_deliveries_is_failed_delivery(delivery_status):
    """
    TRUE when the message left our systems but did not land.
    
    Formula: =OR({{DeliveryStatus}} = "Failed", {{DeliveryStatus}} = "Bounced")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(delivery_status), 'Failed')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(delivery_status), 'Bounced')))

def calc_message_deliveries_is_suppressed(delivery_status):
    """
    TRUE when we deliberately chose not to transmit.
    
    Formula: ={{DeliveryStatus}} = "Suppressed" 
    """
    return _erb.erb_eq(_erb.erb_nullif(delivery_status), 'Suppressed')

def calc_message_deliveries_is_triaged(invoked_exception):
    """
    TRUE when some documented exception was formally invoked on this delivery -- i.e. a human or a rule picked it up.
    
    Formula: ={{InvokedException}} <> "" 
    """
    return (not (invoked_exception is None or invoked_exception == ""))

def calc_message_deliveries_reached_execution_key(delivery_status, procedure_execution):
    """
    Carries the execution id on confirmed-delivered messages; empty string otherwise.
    
    Formula: =IF({{DeliveryStatus}} = "Delivered", {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(delivery_status), 'Delivered')) else '')

def calc_message_deliveries_was_sent_outside_business_hours(sent_at_local_hour):
    """
    TRUE when this message landed before 08:00 or after 18:00 in the recipient's local time.
    
    Formula: =OR({{SentAtLocalHour}} < 8, {{SentAtLocalHour}} > 18)
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at_local_hour), '<', 8)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at_local_hour), '>', 18)))

def calc_message_deliveries_approval_preceded_send(approval_decided_at_send, sent_at):
    """
    TRUE when the approval we relied on was granted before the message went out.
    
    Formula: =AND({{ApprovalDecidedAtSend}} <> "", {{SentAt}} > {{ApprovalDecidedAtSend}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (approval_decided_at_send is None or approval_decided_at_send == ""))), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at), '>', _erb.erb_nullif(approval_decided_at_send))))

def calc_message_deliveries_has_frozen_approval_evidence(approving_agent_at_send, approval_decided_at_send):
    """
    TRUE when this delivery carries a complete frozen record of who authorized it and when.
    
    Formula: =AND({{ApprovingAgentAtSend}} <> "", {{ApprovalDecidedAtSend}} <> "")
    """
    return _erb.erb_and(_erb.erb_bool3((not (approving_agent_at_send is None or approving_agent_at_send == ""))), _erb.erb_bool3((not (approval_decided_at_send is None or approval_decided_at_send == ""))))

def calc_message_deliveries_template_reapproved_since_send(current_last_approval_at, sent_at):
    """
    TRUE when the template has picked up a newer approval since this message was transmitted.
    
    Formula: =AND({{CurrentLastApprovalAt}} <> "", {{CurrentLastApprovalAt}} > {{SentAt}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (current_last_approval_at is None or current_last_approval_at == ""))), _erb.erb_bool3(_erb.erb_cmp(current_last_approval_at, '>', _erb.erb_nullif(sent_at))))

def calc_message_deliveries_has_sent_reminder(reminder_count):
    """
    TRUE when at least one reminder went out for this delivery.
    
    Formula: ={{ReminderCount}} > 0
    """
    return _erb.erb_cmp(_erb.erb_nullif(reminder_count), '>', 0)

# Level 2

def calc_message_deliveries_is_consent_violation(was_actually_transmitted, policy_requires_consent, recipient_has_sms_consent):
    """
    TRUE when a message was actually transmitted on a consent-required channel to a recipient without consent. This is the TCPA witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresConsent}}, NOT({{RecipientHasSmsConsent}})))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(policy_requires_consent), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(recipient_has_sms_consent))))))

def calc_message_deliveries_is_inside_quiet_window(quiet_window_wraps_midnight, sent_at_local_hour, policy_quiet_hours_start_hour, policy_quiet_hours_end_hour):
    """
    TRUE when the recipient-local send hour falls inside the declared quiet window, handling the midnight wrap correctly.
    
    Formula: =IF({{QuietWindowWrapsMidnight}}, OR({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}}), AND({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}}))
    """
    return (_erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at_local_hour), '>=', policy_quiet_hours_start_hour)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at_local_hour), '<', policy_quiet_hours_end_hour))) if _erb.erb_bool3(quiet_window_wraps_midnight) else _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at_local_hour), '>=', policy_quiet_hours_start_hour)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(sent_at_local_hour), '<', policy_quiet_hours_end_hour))))

def calc_message_deliveries_is_fabricated_acknowledgement(recipient_is_unreachable, is_acknowledged):
    """
    TRUE when an acknowledgement is recorded for a recipient we could not reach. This is the misrepresentation witness.
    
    Formula: =AND({{RecipientIsUnreachable}}, {{IsAcknowledged}})
    """
    return _erb.erb_and(_erb.erb_bool3(recipient_is_unreachable), _erb.erb_bool3(is_acknowledged))

def calc_message_deliveries_is_unhandled_unreachable(recipient_is_unreachable, has_unreachable_exception_invoked):
    """
    TRUE when a recipient was unreachable but no documented exception was invoked -- the case fell on the floor silently.
    
    Formula: =AND({{RecipientIsUnreachable}}, NOT({{HasUnreachableExceptionInvoked}}))
    """
    return _erb.erb_and(_erb.erb_bool3(recipient_is_unreachable), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_unreachable_exception_invoked))))

def calc_message_deliveries_is_within_retention_window(age_days, policy_retention_days):
    """
    TRUE while this delivery is still inside its committed retention period.
    
    Formula: ={{AgeDays}} <= {{PolicyRetentionDays}}
    """
    return _erb.erb_cmp(age_days, '<=', policy_retention_days)

def calc_message_deliveries_is_unreviewed_send(was_actually_transmitted, execution_has_cleared_legal_review):
    """
    TRUE when a message was transmitted under an execution whose legal review had not passed. The ungated-send witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, NOT({{ExecutionHasClearedLegalReview}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(execution_has_cleared_legal_review))))

def calc_message_deliveries_segment_count(rendered_body_length, policy_max_message_length_at_send):
    """
    How many channel segments the transmitted text occupied.
    
    Formula: =IF({{RenderedBodyLength}} = 0, 0, IF({{RenderedBodyLength}} <= {{PolicyMaxMessageLengthAtSend}}, 1, ROUNDUP({{RenderedBodyLength}} / {{PolicyMaxMessageLengthAtSend}}, 0)))
    """
    return _erb.erb_integer((0 if _erb.erb_bool3(_erb.erb_eq(rendered_body_length, 0)) else (1 if _erb.erb_bool3(_erb.erb_cmp(rendered_body_length, '<=', policy_max_message_length_at_send)) else _erb.erb_roundup(_erb.erb_div(rendered_body_length, policy_max_message_length_at_send), 0))))

def calc_message_deliveries_is_unapproved_send(was_actually_transmitted, template_has_valid_approval):
    """
    TRUE when a message was transmitted using a template with no valid approval behind it.
    
    Formula: =AND({{WasActuallyTransmitted}}, NOT({{TemplateHasValidApproval}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(template_has_valid_approval))))

def calc_message_deliveries_has_opt_out_phrase(opt_out_phrase_position):
    """
    TRUE when the required opt-out phrase appears anywhere in the transmitted text.
    
    Formula: ={{OptOutPhrasePosition}} > 0
    """
    return _erb.erb_cmp(opt_out_phrase_position, '>', 0)

def calc_message_deliveries_is_abandoned_failure(is_failed_delivery, is_triaged):
    """
    TRUE when a delivery failed and no documented exception was invoked. The abandoned-bounce witness.
    
    Formula: =AND({{IsFailedDelivery}}, NOT({{IsTriaged}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_failed_delivery), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_triaged))))

def calc_message_deliveries_is_drifted_send(was_actually_transmitted, template_was_sendable):
    """
    TRUE when a message was transmitted from a template that was not validly sendable at the time. The drift witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, NOT({{TemplateWasSendable}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(template_was_sendable))))

def calc_message_deliveries_was_delivered_and_unanswered(was_actually_transmitted, is_acknowledged):
    """
    TRUE when a message actually reached someone and no acknowledgement came back.
    
    Formula: =AND({{WasActuallyTransmitted}}, NOT({{IsAcknowledged}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_acknowledged))))

def calc_message_deliveries_transmitted_template_key(was_actually_transmitted, message_template):
    """
    The template id when this delivery actually reached someone.
    
    Formula: =IF({{WasActuallyTransmitted}}, {{MessageTemplate}}, "")
    """
    return (message_template if _erb.erb_bool3(was_actually_transmitted) else '')

def calc_message_deliveries_provenance_is_live_derived(has_frozen_approval_evidence):
    """
    TRUE when we have no frozen evidence and the only available answer comes from recomputing against today's template state.
    
    Formula: =NOT({{HasFrozenApprovalEvidence}})
    """
    return _erb.erb_not(_erb.erb_bool3(has_frozen_approval_evidence))

# Level 3

def calc_message_deliveries_consent_violation_policy_key(is_consent_violation, policy_channel):
    """
    Carries the policy id only on rows that are consent violations; empty string otherwise.
    
    Formula: =IF({{IsConsentViolation}}, {{PolicyChannel}}, "")
    """
    return (policy_channel if _erb.erb_bool3(is_consent_violation) else '')

def calc_message_deliveries_is_quiet_hours_violation(was_actually_transmitted, policy_has_quiet_hours, is_inside_quiet_window):
    """
    TRUE when a message was actually transmitted inside a declared quiet-hours window. The quiet-hours witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyHasQuietHours}}, {{IsInsideQuietWindow}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(policy_has_quiet_hours), _erb.erb_bool3(is_inside_quiet_window))))

def calc_message_deliveries_unreachable_failure_key(is_fabricated_acknowledgement, is_unhandled_unreachable, procedure_execution):
    """
    Carries the execution id on any unreachable-handling failure; empty string otherwise.
    
    Formula: =IF(OR({{IsFabricatedAcknowledgement}}, {{IsUnhandledUnreachable}}), {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(is_fabricated_acknowledgement), _erb.erb_bool3(is_unhandled_unreachable))) else '')

def calc_message_deliveries_is_evidence_required(was_actually_transmitted, is_within_retention_window):
    """
    TRUE when we are still obliged to hold this message's text.
    
    Formula: =AND({{WasActuallyTransmitted}}, {{IsWithinRetentionWindow}})
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(is_within_retention_window))

def calc_message_deliveries_is_over_segment_limit(was_actually_transmitted, segment_count, policy_max_segments_at_send):
    """
    TRUE when a transmitted message split into more segments than the policy permits. The oversize witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, {{SegmentCount}} > {{PolicyMaxSegmentsAtSend}})
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_cmp(segment_count, '>', policy_max_segments_at_send)))

def calc_message_deliveries_is_opt_out_in_first_segment(has_opt_out_phrase, opt_out_phrase_position, policy_max_message_length_at_send):
    """
    TRUE when the opt-out phrase falls inside the first segment, where it is guaranteed to be read.
    
    Formula: =AND({{HasOptOutPhrase}}, {{OptOutPhrasePosition}} <= {{PolicyMaxMessageLengthAtSend}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_opt_out_phrase), _erb.erb_bool3(_erb.erb_cmp(opt_out_phrase_position, '<=', policy_max_message_length_at_send)))

def calc_message_deliveries_is_missing_required_opt_out(was_actually_transmitted, policy_requires_opt_out, has_opt_out_phrase):
    """
    TRUE when a transmitted message on an opt-out-required channel did not contain the phrase at all. The hard witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, NOT({{HasOptOutPhrase}})))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(policy_requires_opt_out), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_opt_out_phrase))))))

def calc_message_deliveries_abandoned_failure_execution_key(is_abandoned_failure, procedure_execution):
    """
    Carries the execution id on abandoned failures; empty string otherwise.
    
    Formula: =IF({{IsAbandonedFailure}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_abandoned_failure) else '')

def calc_message_deliveries_drifted_send_template_key(is_drifted_send, message_template):
    """
    Carries the template id on drifted sends; empty string otherwise.
    
    Formula: =IF({{IsDriftedSend}}, {{MessageTemplate}}, "")
    """
    return (message_template if _erb.erb_bool3(is_drifted_send) else '')

def calc_message_deliveries_is_poorly_timed_unanswered(was_delivered_and_unanswered, was_sent_outside_business_hours):
    """
    TRUE when an unanswered message was delivered outside business hours -- a timing hypothesis for the silence.
    
    Formula: =AND({{WasDeliveredAndUnanswered}}, {{WasSentOutsideBusinessHours}})
    """
    return _erb.erb_and(_erb.erb_bool3(was_delivered_and_unanswered), _erb.erb_bool3(was_sent_outside_business_hours))

def calc_message_deliveries_is_well_timed_unanswered(was_delivered_and_unanswered, was_sent_outside_business_hours):
    """
    TRUE when a message was delivered at a reasonable hour and still drew no response.
    
    Formula: =AND({{WasDeliveredAndUnanswered}}, NOT({{WasSentOutsideBusinessHours}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_delivered_and_unanswered), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(was_sent_outside_business_hours))))

def calc_message_deliveries_unanswered_template_key(was_delivered_and_unanswered, message_template):
    """
    The template id when this delivery went unanswered, otherwise empty string.
    
    Formula: =IF({{WasDeliveredAndUnanswered}}, {{MessageTemplate}}, "")
    """
    return (message_template if _erb.erb_bool3(was_delivered_and_unanswered) else '')

def calc_message_deliveries_is_unprovable_approval_claim(provenance_is_live_derived, template_reapproved_since_send, template_has_valid_approval):
    """
    TRUE when we assert this send was approved, hold no frozen evidence, and the template has been re-approved since. The claim cannot be substantiated from the record.
    
    Formula: =AND({{ProvenanceIsLiveDerived}}, AND({{TemplateReapprovedSinceSend}}, {{TemplateHasValidApproval}}))
    """
    return _erb.erb_and(_erb.erb_bool3(provenance_is_live_derived), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(template_reapproved_since_send), _erb.erb_bool3(template_has_valid_approval))))

# Level 4

def calc_message_deliveries_quiet_hours_violation_policy_key(is_quiet_hours_violation, policy_channel):
    """
    Carries the policy id only on quiet-hours violations; empty string otherwise.
    
    Formula: =IF({{IsQuietHoursViolation}}, {{PolicyChannel}}, "")
    """
    return (policy_channel if _erb.erb_bool3(is_quiet_hours_violation) else '')

def calc_message_deliveries_is_retention_breach(is_evidence_required, has_rendered_body):
    """
    TRUE when we are obliged to hold the message text and do not. The evidentiary-gap witness.
    
    Formula: =AND({{IsEvidenceRequired}}, NOT({{HasRenderedBody}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_evidence_required), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_rendered_body))))

def calc_message_deliveries_is_opt_out_at_risk_of_truncation(was_actually_transmitted, policy_requires_opt_out, has_opt_out_phrase, is_opt_out_in_first_segment):
    """
    TRUE when the opt-out phrase is present but sits beyond the first segment, where carrier truncation or out-of-order delivery can hide it. The soft witness.
    
    Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, AND({{HasOptOutPhrase}}, NOT({{IsOptOutInFirstSegment}}))))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(policy_requires_opt_out), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_opt_out_phrase), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_opt_out_in_first_segment))))))))

def calc_message_deliveries_acknowledgement_is_outstanding(was_actually_transmitted, is_evidence_required, is_acknowledged):
    """
    TRUE when a message actually reached someone, carried an acknowledgement obligation, and has not been acknowledged.
    
    Formula: =AND({{WasActuallyTransmitted}}, AND({{IsEvidenceRequired}}, NOT({{IsAcknowledged}})))
    """
    return _erb.erb_and(_erb.erb_bool3(was_actually_transmitted), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_evidence_required), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_acknowledged))))))

# Level 5

def calc_message_deliveries_retention_breach_execution_key(is_retention_breach, procedure_execution):
    """
    Carries the execution id on retention breaches; empty string otherwise.
    
    Formula: =IF({{IsRetentionBreach}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_retention_breach) else '')

def calc_message_deliveries_outstanding_age_days(acknowledgement_is_outstanding, as_of_instant, sent_at):
    """
    How long this acknowledgement has been outstanding.
    
    Formula: =IF({{AcknowledgementIsOutstanding}}, DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days"), 0)
    """
    return _erb.erb_integer((_erb.erb_datetime_diff(as_of_instant, sent_at, 'days') if _erb.erb_bool3(acknowledgement_is_outstanding) else 0))

def calc_message_deliveries_is_exhausted_follow_up(acknowledgement_is_outstanding, reminder_count):
    """
    TRUE when I have sent three or more reminders and still have no acknowledgement -- the point at which this stops being my work and becomes a human escalation.
    
    Formula: =AND({{AcknowledgementIsOutstanding}}, {{ReminderCount}} >= 3)
    """
    return _erb.erb_and(_erb.erb_bool3(acknowledgement_is_outstanding), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(reminder_count), '>=', 3)))

# Level 6

def calc_message_deliveries_is_unchased_acknowledgement(acknowledgement_is_outstanding, outstanding_age_days, has_sent_reminder):
    """
    TRUE when an acknowledgement has been outstanding for more than 7 days and I have never sent a reminder.
    
    Formula: =AND({{AcknowledgementIsOutstanding}}, AND({{OutstandingAgeDays}} > 7, NOT({{HasSentReminder}})))
    """
    return _erb.erb_and(_erb.erb_bool3(acknowledgement_is_outstanding), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(outstanding_age_days, '>', 7)), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_sent_reminder))))))

def calc_message_deliveries_needs_human_escalation(is_exhausted_follow_up, has_unreachable_exception_invoked):
    """
    TRUE when follow-up is exhausted and no exception has been invoked to close out the obligation.
    
    Formula: =AND({{IsExhaustedFollowUp}}, NOT({{HasUnreachableExceptionInvoked}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_exhausted_follow_up), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_unreachable_exception_invoked))))


def compute_message_deliveries_fields(record: dict) -> dict:
    """
    Compute all calculated fields for MessageDeliveries.
    
    MessageDeliveries (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_message_deliveries_name(result.get('recipient'), result.get('message_template'), result.get('sent_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['was_actually_transmitted'] = calc_message_deliveries_was_actually_transmitted(result.get('delivery_status'))
    except Exception as _field_exc:
        result['was_actually_transmitted'] = None
        result.setdefault('_erb_errors', {})['was_actually_transmitted'] = str(_field_exc)
    try:
        result['policy_has_quiet_hours'] = calc_message_deliveries_policy_has_quiet_hours(result.get('policy_quiet_hours_start_hour'), result.get('policy_quiet_hours_end_hour'))
    except Exception as _field_exc:
        result['policy_has_quiet_hours'] = None
        result.setdefault('_erb_errors', {})['policy_has_quiet_hours'] = str(_field_exc)
    try:
        result['quiet_window_wraps_midnight'] = calc_message_deliveries_quiet_window_wraps_midnight(result.get('policy_quiet_hours_start_hour'), result.get('policy_quiet_hours_end_hour'))
    except Exception as _field_exc:
        result['quiet_window_wraps_midnight'] = None
        result.setdefault('_erb_errors', {})['quiet_window_wraps_midnight'] = str(_field_exc)
    try:
        result['is_acknowledged'] = calc_message_deliveries_is_acknowledged(result.get('acknowledged_at'))
    except Exception as _field_exc:
        result['is_acknowledged'] = None
        result.setdefault('_erb_errors', {})['is_acknowledged'] = str(_field_exc)
    try:
        result['has_unreachable_exception_invoked'] = calc_message_deliveries_has_unreachable_exception_invoked(result.get('invoked_exception'))
    except Exception as _field_exc:
        result['has_unreachable_exception_invoked'] = None
        result.setdefault('_erb_errors', {})['has_unreachable_exception_invoked'] = str(_field_exc)
    try:
        result['age_days'] = calc_message_deliveries_age_days(result.get('as_of_instant'), result.get('sent_at'))
    except Exception as _field_exc:
        result['age_days'] = None
        result.setdefault('_erb_errors', {})['age_days'] = str(_field_exc)
    try:
        result['has_rendered_body'] = calc_message_deliveries_has_rendered_body(result.get('rendered_body'))
    except Exception as _field_exc:
        result['has_rendered_body'] = None
        result.setdefault('_erb_errors', {})['has_rendered_body'] = str(_field_exc)
    try:
        result['rendered_body_length'] = calc_message_deliveries_rendered_body_length(result.get('rendered_body'))
    except Exception as _field_exc:
        result['rendered_body_length'] = None
        result.setdefault('_erb_errors', {})['rendered_body_length'] = str(_field_exc)
    try:
        result['policy_requires_opt_out'] = calc_message_deliveries_policy_requires_opt_out(result.get('policy_required_opt_out_phrase'))
    except Exception as _field_exc:
        result['policy_requires_opt_out'] = None
        result.setdefault('_erb_errors', {})['policy_requires_opt_out'] = str(_field_exc)
    try:
        result['opt_out_phrase_position'] = calc_message_deliveries_opt_out_phrase_position(result.get('policy_required_opt_out_phrase'), result.get('rendered_body'))
    except Exception as _field_exc:
        result['opt_out_phrase_position'] = None
        result.setdefault('_erb_errors', {})['opt_out_phrase_position'] = str(_field_exc)
    try:
        result['is_failed_delivery'] = calc_message_deliveries_is_failed_delivery(result.get('delivery_status'))
    except Exception as _field_exc:
        result['is_failed_delivery'] = None
        result.setdefault('_erb_errors', {})['is_failed_delivery'] = str(_field_exc)
    try:
        result['is_suppressed'] = calc_message_deliveries_is_suppressed(result.get('delivery_status'))
    except Exception as _field_exc:
        result['is_suppressed'] = None
        result.setdefault('_erb_errors', {})['is_suppressed'] = str(_field_exc)
    try:
        result['is_triaged'] = calc_message_deliveries_is_triaged(result.get('invoked_exception'))
    except Exception as _field_exc:
        result['is_triaged'] = None
        result.setdefault('_erb_errors', {})['is_triaged'] = str(_field_exc)
    try:
        result['reached_execution_key'] = calc_message_deliveries_reached_execution_key(result.get('delivery_status'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['reached_execution_key'] = None
        result.setdefault('_erb_errors', {})['reached_execution_key'] = str(_field_exc)
    try:
        result['was_sent_outside_business_hours'] = calc_message_deliveries_was_sent_outside_business_hours(result.get('sent_at_local_hour'))
    except Exception as _field_exc:
        result['was_sent_outside_business_hours'] = None
        result.setdefault('_erb_errors', {})['was_sent_outside_business_hours'] = str(_field_exc)
    try:
        result['approval_preceded_send'] = calc_message_deliveries_approval_preceded_send(result.get('approval_decided_at_send'), result.get('sent_at'))
    except Exception as _field_exc:
        result['approval_preceded_send'] = None
        result.setdefault('_erb_errors', {})['approval_preceded_send'] = str(_field_exc)
    try:
        result['has_frozen_approval_evidence'] = calc_message_deliveries_has_frozen_approval_evidence(result.get('approving_agent_at_send'), result.get('approval_decided_at_send'))
    except Exception as _field_exc:
        result['has_frozen_approval_evidence'] = None
        result.setdefault('_erb_errors', {})['has_frozen_approval_evidence'] = str(_field_exc)
    try:
        result['template_reapproved_since_send'] = calc_message_deliveries_template_reapproved_since_send(result.get('current_last_approval_at'), result.get('sent_at'))
    except Exception as _field_exc:
        result['template_reapproved_since_send'] = None
        result.setdefault('_erb_errors', {})['template_reapproved_since_send'] = str(_field_exc)
    try:
        result['has_sent_reminder'] = calc_message_deliveries_has_sent_reminder(result.get('reminder_count'))
    except Exception as _field_exc:
        result['has_sent_reminder'] = None
        result.setdefault('_erb_errors', {})['has_sent_reminder'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_consent_violation'] = calc_message_deliveries_is_consent_violation(result.get('was_actually_transmitted'), result.get('policy_requires_consent'), result.get('recipient_has_sms_consent'))
    except Exception as _field_exc:
        result['is_consent_violation'] = None
        result.setdefault('_erb_errors', {})['is_consent_violation'] = str(_field_exc)
    try:
        result['is_inside_quiet_window'] = calc_message_deliveries_is_inside_quiet_window(result.get('quiet_window_wraps_midnight'), result.get('sent_at_local_hour'), result.get('policy_quiet_hours_start_hour'), result.get('policy_quiet_hours_end_hour'))
    except Exception as _field_exc:
        result['is_inside_quiet_window'] = None
        result.setdefault('_erb_errors', {})['is_inside_quiet_window'] = str(_field_exc)
    try:
        result['is_fabricated_acknowledgement'] = calc_message_deliveries_is_fabricated_acknowledgement(result.get('recipient_is_unreachable'), result.get('is_acknowledged'))
    except Exception as _field_exc:
        result['is_fabricated_acknowledgement'] = None
        result.setdefault('_erb_errors', {})['is_fabricated_acknowledgement'] = str(_field_exc)
    try:
        result['is_unhandled_unreachable'] = calc_message_deliveries_is_unhandled_unreachable(result.get('recipient_is_unreachable'), result.get('has_unreachable_exception_invoked'))
    except Exception as _field_exc:
        result['is_unhandled_unreachable'] = None
        result.setdefault('_erb_errors', {})['is_unhandled_unreachable'] = str(_field_exc)
    try:
        result['is_within_retention_window'] = calc_message_deliveries_is_within_retention_window(result.get('age_days'), result.get('policy_retention_days'))
    except Exception as _field_exc:
        result['is_within_retention_window'] = None
        result.setdefault('_erb_errors', {})['is_within_retention_window'] = str(_field_exc)
    try:
        result['is_unreviewed_send'] = calc_message_deliveries_is_unreviewed_send(result.get('was_actually_transmitted'), result.get('execution_has_cleared_legal_review'))
    except Exception as _field_exc:
        result['is_unreviewed_send'] = None
        result.setdefault('_erb_errors', {})['is_unreviewed_send'] = str(_field_exc)
    try:
        result['segment_count'] = calc_message_deliveries_segment_count(result.get('rendered_body_length'), result.get('policy_max_message_length_at_send'))
    except Exception as _field_exc:
        result['segment_count'] = None
        result.setdefault('_erb_errors', {})['segment_count'] = str(_field_exc)
    try:
        result['is_unapproved_send'] = calc_message_deliveries_is_unapproved_send(result.get('was_actually_transmitted'), result.get('template_has_valid_approval'))
    except Exception as _field_exc:
        result['is_unapproved_send'] = None
        result.setdefault('_erb_errors', {})['is_unapproved_send'] = str(_field_exc)
    try:
        result['has_opt_out_phrase'] = calc_message_deliveries_has_opt_out_phrase(result.get('opt_out_phrase_position'))
    except Exception as _field_exc:
        result['has_opt_out_phrase'] = None
        result.setdefault('_erb_errors', {})['has_opt_out_phrase'] = str(_field_exc)
    try:
        result['is_abandoned_failure'] = calc_message_deliveries_is_abandoned_failure(result.get('is_failed_delivery'), result.get('is_triaged'))
    except Exception as _field_exc:
        result['is_abandoned_failure'] = None
        result.setdefault('_erb_errors', {})['is_abandoned_failure'] = str(_field_exc)
    try:
        result['is_drifted_send'] = calc_message_deliveries_is_drifted_send(result.get('was_actually_transmitted'), result.get('template_was_sendable'))
    except Exception as _field_exc:
        result['is_drifted_send'] = None
        result.setdefault('_erb_errors', {})['is_drifted_send'] = str(_field_exc)
    try:
        result['was_delivered_and_unanswered'] = calc_message_deliveries_was_delivered_and_unanswered(result.get('was_actually_transmitted'), result.get('is_acknowledged'))
    except Exception as _field_exc:
        result['was_delivered_and_unanswered'] = None
        result.setdefault('_erb_errors', {})['was_delivered_and_unanswered'] = str(_field_exc)
    try:
        result['transmitted_template_key'] = calc_message_deliveries_transmitted_template_key(result.get('was_actually_transmitted'), result.get('message_template'))
    except Exception as _field_exc:
        result['transmitted_template_key'] = None
        result.setdefault('_erb_errors', {})['transmitted_template_key'] = str(_field_exc)
    try:
        result['provenance_is_live_derived'] = calc_message_deliveries_provenance_is_live_derived(result.get('has_frozen_approval_evidence'))
    except Exception as _field_exc:
        result['provenance_is_live_derived'] = None
        result.setdefault('_erb_errors', {})['provenance_is_live_derived'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['consent_violation_policy_key'] = calc_message_deliveries_consent_violation_policy_key(result.get('is_consent_violation'), result.get('policy_channel'))
    except Exception as _field_exc:
        result['consent_violation_policy_key'] = None
        result.setdefault('_erb_errors', {})['consent_violation_policy_key'] = str(_field_exc)
    try:
        result['is_quiet_hours_violation'] = calc_message_deliveries_is_quiet_hours_violation(result.get('was_actually_transmitted'), result.get('policy_has_quiet_hours'), result.get('is_inside_quiet_window'))
    except Exception as _field_exc:
        result['is_quiet_hours_violation'] = None
        result.setdefault('_erb_errors', {})['is_quiet_hours_violation'] = str(_field_exc)
    try:
        result['unreachable_failure_key'] = calc_message_deliveries_unreachable_failure_key(result.get('is_fabricated_acknowledgement'), result.get('is_unhandled_unreachable'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['unreachable_failure_key'] = None
        result.setdefault('_erb_errors', {})['unreachable_failure_key'] = str(_field_exc)
    try:
        result['is_evidence_required'] = calc_message_deliveries_is_evidence_required(result.get('was_actually_transmitted'), result.get('is_within_retention_window'))
    except Exception as _field_exc:
        result['is_evidence_required'] = None
        result.setdefault('_erb_errors', {})['is_evidence_required'] = str(_field_exc)
    try:
        result['is_over_segment_limit'] = calc_message_deliveries_is_over_segment_limit(result.get('was_actually_transmitted'), result.get('segment_count'), result.get('policy_max_segments_at_send'))
    except Exception as _field_exc:
        result['is_over_segment_limit'] = None
        result.setdefault('_erb_errors', {})['is_over_segment_limit'] = str(_field_exc)
    try:
        result['is_opt_out_in_first_segment'] = calc_message_deliveries_is_opt_out_in_first_segment(result.get('has_opt_out_phrase'), result.get('opt_out_phrase_position'), result.get('policy_max_message_length_at_send'))
    except Exception as _field_exc:
        result['is_opt_out_in_first_segment'] = None
        result.setdefault('_erb_errors', {})['is_opt_out_in_first_segment'] = str(_field_exc)
    try:
        result['is_missing_required_opt_out'] = calc_message_deliveries_is_missing_required_opt_out(result.get('was_actually_transmitted'), result.get('policy_requires_opt_out'), result.get('has_opt_out_phrase'))
    except Exception as _field_exc:
        result['is_missing_required_opt_out'] = None
        result.setdefault('_erb_errors', {})['is_missing_required_opt_out'] = str(_field_exc)
    try:
        result['abandoned_failure_execution_key'] = calc_message_deliveries_abandoned_failure_execution_key(result.get('is_abandoned_failure'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['abandoned_failure_execution_key'] = None
        result.setdefault('_erb_errors', {})['abandoned_failure_execution_key'] = str(_field_exc)
    try:
        result['drifted_send_template_key'] = calc_message_deliveries_drifted_send_template_key(result.get('is_drifted_send'), result.get('message_template'))
    except Exception as _field_exc:
        result['drifted_send_template_key'] = None
        result.setdefault('_erb_errors', {})['drifted_send_template_key'] = str(_field_exc)
    try:
        result['is_poorly_timed_unanswered'] = calc_message_deliveries_is_poorly_timed_unanswered(result.get('was_delivered_and_unanswered'), result.get('was_sent_outside_business_hours'))
    except Exception as _field_exc:
        result['is_poorly_timed_unanswered'] = None
        result.setdefault('_erb_errors', {})['is_poorly_timed_unanswered'] = str(_field_exc)
    try:
        result['is_well_timed_unanswered'] = calc_message_deliveries_is_well_timed_unanswered(result.get('was_delivered_and_unanswered'), result.get('was_sent_outside_business_hours'))
    except Exception as _field_exc:
        result['is_well_timed_unanswered'] = None
        result.setdefault('_erb_errors', {})['is_well_timed_unanswered'] = str(_field_exc)
    try:
        result['unanswered_template_key'] = calc_message_deliveries_unanswered_template_key(result.get('was_delivered_and_unanswered'), result.get('message_template'))
    except Exception as _field_exc:
        result['unanswered_template_key'] = None
        result.setdefault('_erb_errors', {})['unanswered_template_key'] = str(_field_exc)
    try:
        result['is_unprovable_approval_claim'] = calc_message_deliveries_is_unprovable_approval_claim(result.get('provenance_is_live_derived'), result.get('template_reapproved_since_send'), result.get('template_has_valid_approval'))
    except Exception as _field_exc:
        result['is_unprovable_approval_claim'] = None
        result.setdefault('_erb_errors', {})['is_unprovable_approval_claim'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['quiet_hours_violation_policy_key'] = calc_message_deliveries_quiet_hours_violation_policy_key(result.get('is_quiet_hours_violation'), result.get('policy_channel'))
    except Exception as _field_exc:
        result['quiet_hours_violation_policy_key'] = None
        result.setdefault('_erb_errors', {})['quiet_hours_violation_policy_key'] = str(_field_exc)
    try:
        result['is_retention_breach'] = calc_message_deliveries_is_retention_breach(result.get('is_evidence_required'), result.get('has_rendered_body'))
    except Exception as _field_exc:
        result['is_retention_breach'] = None
        result.setdefault('_erb_errors', {})['is_retention_breach'] = str(_field_exc)
    try:
        result['is_opt_out_at_risk_of_truncation'] = calc_message_deliveries_is_opt_out_at_risk_of_truncation(result.get('was_actually_transmitted'), result.get('policy_requires_opt_out'), result.get('has_opt_out_phrase'), result.get('is_opt_out_in_first_segment'))
    except Exception as _field_exc:
        result['is_opt_out_at_risk_of_truncation'] = None
        result.setdefault('_erb_errors', {})['is_opt_out_at_risk_of_truncation'] = str(_field_exc)
    try:
        result['acknowledgement_is_outstanding'] = calc_message_deliveries_acknowledgement_is_outstanding(result.get('was_actually_transmitted'), result.get('is_evidence_required'), result.get('is_acknowledged'))
    except Exception as _field_exc:
        result['acknowledgement_is_outstanding'] = None
        result.setdefault('_erb_errors', {})['acknowledgement_is_outstanding'] = str(_field_exc)

    # Level 5 calculations
    try:
        result['retention_breach_execution_key'] = calc_message_deliveries_retention_breach_execution_key(result.get('is_retention_breach'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['retention_breach_execution_key'] = None
        result.setdefault('_erb_errors', {})['retention_breach_execution_key'] = str(_field_exc)
    try:
        result['outstanding_age_days'] = calc_message_deliveries_outstanding_age_days(result.get('acknowledgement_is_outstanding'), result.get('as_of_instant'), result.get('sent_at'))
    except Exception as _field_exc:
        result['outstanding_age_days'] = None
        result.setdefault('_erb_errors', {})['outstanding_age_days'] = str(_field_exc)
    try:
        result['is_exhausted_follow_up'] = calc_message_deliveries_is_exhausted_follow_up(result.get('acknowledgement_is_outstanding'), result.get('reminder_count'))
    except Exception as _field_exc:
        result['is_exhausted_follow_up'] = None
        result.setdefault('_erb_errors', {})['is_exhausted_follow_up'] = str(_field_exc)

    # Level 6 calculations
    try:
        result['is_unchased_acknowledgement'] = calc_message_deliveries_is_unchased_acknowledgement(result.get('acknowledgement_is_outstanding'), result.get('outstanding_age_days'), result.get('has_sent_reminder'))
    except Exception as _field_exc:
        result['is_unchased_acknowledgement'] = None
        result.setdefault('_erb_errors', {})['is_unchased_acknowledgement'] = str(_field_exc)
    try:
        result['needs_human_escalation'] = calc_message_deliveries_needs_human_escalation(result.get('is_exhausted_follow_up'), result.get('has_unreachable_exception_invoked'))
    except Exception as _field_exc:
        result['needs_human_escalation'] = None
        result.setdefault('_erb_errors', {})['needs_human_escalation'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'consent_violation_policy_key', 'quiet_hours_violation_policy_key', 'unreachable_failure_key', 'retention_breach_execution_key', 'abandoned_failure_execution_key', 'reached_execution_key', 'drifted_send_template_key', 'unanswered_template_key', 'transmitted_template_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# TEMPLATEAPPROVALS
# TemplateApprovals (added by witness loop 1).
# =============================================================================

# Level 1

def calc_template_approvals_name(message_template, decision, decided_at):
    """
    Human-readable calculated display alias for the TemplateApprovals row.
    
    Formula: ={{MessageTemplate}} & " / " & {{Decision}} & " / " & {{DecidedAt}}
    """
    return (str(message_template or "") + ' / ' + str(decision or "") + ' / ' + _erb.erb_timestamptz_text(decided_at))

def calc_template_approvals_is_approval_decision(decision):
    """
    TRUE when this decision row is an approval rather than a rejection or withdrawal.
    
    Formula: ={{Decision}} = "Approved" 
    """
    return _erb.erb_eq(_erb.erb_nullif(decision), 'Approved')

def calc_template_approvals_is_decided_by_required_role(decided_in_role, required_approval_role):
    """
    TRUE when the approving role matches the role the policy designates.
    
    Formula: ={{DecidedInRole}} = {{RequiredApprovalRole}}
    """
    return _erb.erb_eq(_erb.erb_nullif(decided_in_role), required_approval_role)

# Level 2

def calc_template_approvals_valid_approval_template_key(is_approval_decision, is_decided_by_required_role, message_template):
    """
    Carries the template id only on approvals made by the correct role; empty string otherwise.
    
    Formula: =IF(AND({{IsApprovalDecision}}, {{IsDecidedByRequiredRole}}), {{MessageTemplate}}, "")
    """
    return (message_template if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_approval_decision), _erb.erb_bool3(is_decided_by_required_role))) else '')


def compute_template_approvals_fields(record: dict) -> dict:
    """
    Compute all calculated fields for TemplateApprovals.
    
    TemplateApprovals (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_template_approvals_name(result.get('message_template'), result.get('decision'), result.get('decided_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_approval_decision'] = calc_template_approvals_is_approval_decision(result.get('decision'))
    except Exception as _field_exc:
        result['is_approval_decision'] = None
        result.setdefault('_erb_errors', {})['is_approval_decision'] = str(_field_exc)
    try:
        result['is_decided_by_required_role'] = calc_template_approvals_is_decided_by_required_role(result.get('decided_in_role'), result.get('required_approval_role'))
    except Exception as _field_exc:
        result['is_decided_by_required_role'] = None
        result.setdefault('_erb_errors', {})['is_decided_by_required_role'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['valid_approval_template_key'] = calc_template_approvals_valid_approval_template_key(result.get('is_approval_decision'), result.get('is_decided_by_required_role'), result.get('message_template'))
    except Exception as _field_exc:
        result['valid_approval_template_key'] = None
        result.setdefault('_erb_errors', {})['valid_approval_template_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'valid_approval_template_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# SENDINTENTS
# SendIntents (added by witness loop 1).
# =============================================================================

# Level 1

def calc_send_intents_name(recipient, message_template):
    """
    Human-readable calculated display alias for the SendIntents row.
    
    Formula: ={{Recipient}} & " / " & {{MessageTemplate}} & " / intent" 
    """
    return (str(recipient or "") + ' / ' + str(message_template or "") + ' / intent')

def calc_send_intents_consent_gate_passed(intent_requires_consent, recipient_has_channel_consent):
    """
    TRUE when either the channel does not require consent, or the recipient has granted it.
    
    Formula: =OR(NOT({{IntentRequiresConsent}}), {{RecipientHasChannelConsent}})
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(intent_requires_consent))), _erb.erb_bool3(recipient_has_channel_consent))

def calc_send_intents_reachability_gate_passed(intent_channel, recipient_is_sms_reachable, recipient_is_email_reachable):
    """
    TRUE when the recipient is reachable on the channel this intent would use.
    
    Formula: =IF({{IntentChannel}} = "SMS", {{RecipientIsSmsReachable}}, {{RecipientIsEmailReachable}})
    """
    return (recipient_is_sms_reachable if _erb.erb_bool3(_erb.erb_eq(intent_channel, 'SMS')) else recipient_is_email_reachable)

def calc_send_intents_intent_policy_has_quiet_hours(intent_quiet_start_hour, intent_quiet_end_hour):
    """
    TRUE when the governing policy declares a real quiet-hours window.
    
    Formula: ={{IntentQuietStartHour}} <> {{IntentQuietEndHour}}
    """
    return _erb.erb_ne(intent_quiet_start_hour, intent_quiet_end_hour)

def calc_send_intents_intent_quiet_window_wraps(intent_quiet_start_hour, intent_quiet_end_hour):
    """
    TRUE when the quiet window crosses midnight.
    
    Formula: ={{IntentQuietStartHour}} > {{IntentQuietEndHour}}
    """
    return _erb.erb_cmp(intent_quiet_start_hour, '>', intent_quiet_end_hour)

def calc_send_intents_length_gate_passed(proposed_body_length, proposed_segment_count, intent_max_segments):
    """
    TRUE when the proposed text is non-empty and fits within the segment ceiling.
    
    Formula: =AND({{ProposedBodyLength}} > 0, {{ProposedSegmentCount}} <= {{IntentMaxSegments}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_body_length), '>', 0)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_segment_count), '<=', intent_max_segments)))

def calc_send_intents_opt_out_gate_passed(intent_required_opt_out_phrase, proposed_opt_out_position, intent_max_message_length):
    """
    TRUE when no opt-out is required, or the required phrase is present within the first segment.
    
    Formula: =OR({{IntentRequiredOptOutPhrase}} = "", AND({{ProposedOptOutPosition}} > 0, {{ProposedOptOutPosition}} <= {{IntentMaxMessageLength}}))
    """
    return _erb.erb_or(_erb.erb_bool3((intent_required_opt_out_phrase is None or intent_required_opt_out_phrase == "")), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_opt_out_position), '>', 0)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_opt_out_position), '<=', intent_max_message_length)))))

def calc_send_intents_approval_is_human(approval_role_agent_kind):
    """
    TRUE when the designated approval role is currently held by a human agent.
    
    Formula: ={{ApprovalRoleAgentKind}} = "Human" 
    """
    return _erb.erb_eq(approval_role_agent_kind, 'Human')

def calc_send_intents_has_resulting_delivery(resulting_delivery):
    """
    TRUE when this intent produced a delivery record of any status.
    
    Formula: ={{ResultingDelivery}} <> "" 
    """
    return (not (resulting_delivery is None or resulting_delivery == ""))

def calc_send_intents_refusal_cited_an_exception(resulting_delivery_exception):
    """
    TRUE when the suppression produced by this refusal cited a documented exception.
    
    Formula: ={{ResultingDeliveryException}} <> "" 
    """
    return (not (resulting_delivery_exception is None or resulting_delivery_exception == ""))

def calc_send_intents_intent_execution_key(procedure_execution):
    """
    The procedure execution id for every intent, used as the campaign rollup key.
    
    Formula: ={{ProcedureExecution}}
    """
    return procedure_execution

def calc_send_intents_my_approval_was_in_force(template_is_sendable):
    """
    TRUE when the template carried a valid approval at the moment this intent was evaluated.
    
    Formula: ={{TemplateIsSendable}}
    """
    return template_is_sendable

def calc_send_intents_has_alternate_channel_attempt(alternate_channel_intent):
    """
    TRUE when a follow-up intent on another channel was raised for this refused send.
    
    Formula: ={{AlternateChannelIntent}} <> "" 
    """
    return (not (alternate_channel_intent is None or alternate_channel_intent == ""))

def calc_send_intents_has_durable_refusal_record(refusal_recorded_at):
    """
    TRUE when this refusal was written down somewhere a human can find it.
    
    Formula: ={{RefusalRecordedAt}} <> "" 
    """
    return (not (refusal_recorded_at is None or refusal_recorded_at == ""))

def calc_send_intents_refusal_was_escalated(refusal_notified_role):
    """
    TRUE when a specific role was notified of this refusal.
    
    Formula: ={{RefusalNotifiedRole}} <> "" 
    """
    return (not (refusal_notified_role is None or refusal_notified_role == ""))

def calc_send_intents_has_retry_attempt(retry_intent):
    """
    TRUE when a retry intent was raised for this deferred send.
    
    Formula: ={{RetryIntent}} <> "" 
    """
    return (not (retry_intent is None or retry_intent == ""))

def calc_send_intents_deferral_age_hours(as_of_instant, evaluated_at):
    """
    How long this deferred intent has been sitting since evaluation.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, evaluated_at, 'hours'))

def calc_send_intents_consent_input_was_resolvable(recipient_consent_status_raw):
    """
    TRUE when the recipient's consent state was actually retrievable, as opposed to absent and read as a refusal.
    
    Formula: ={{RecipientConsentStatusRaw}} <> "" 
    """
    return (not (recipient_consent_status_raw is None or recipient_consent_status_raw == ""))

def calc_send_intents_policy_input_was_resolvable(intent_policy):
    """
    TRUE when a governing communication policy was actually found for this intent.
    
    Formula: ={{IntentPolicy}} <> "" 
    """
    return (not (intent_policy is None or intent_policy == ""))

def calc_send_intents_is_self_witnessed_decision(gate_result_was_independently_confirmed):
    """
    TRUE when the entire decision to send or refuse rests solely on my own computation, unconfirmed by anything else.
    
    Formula: =NOT({{GateResultWasIndependentlyConfirmed}})
    """
    return _erb.erb_not((gate_result_was_independently_confirmed is True))

# Level 2

def calc_send_intents_permission_gate_passed(policy_is_active, consent_gate_passed, reachability_gate_passed):
    """
    TRUE when the policy is active, consent is satisfied, and the recipient is reachable on this channel. The channel-permission gate.
    
    Formula: =AND({{PolicyIsActive}}, AND({{ConsentGatePassed}}, {{ReachabilityGatePassed}}))
    """
    return _erb.erb_and(_erb.erb_bool3(policy_is_active), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(consent_gate_passed), _erb.erb_bool3(reachability_gate_passed))))

def calc_send_intents_intent_is_inside_quiet_window(intent_quiet_window_wraps, proposed_send_at_local_hour, intent_quiet_start_hour, intent_quiet_end_hour):
    """
    TRUE when the proposed recipient-local send hour falls inside the forbidden window.
    
    Formula: =IF({{IntentQuietWindowWraps}}, OR({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}), AND({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}))
    """
    return (_erb.erb_or(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_send_at_local_hour), '>=', intent_quiet_start_hour)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_send_at_local_hour), '<', intent_quiet_end_hour))) if _erb.erb_bool3(intent_quiet_window_wraps) else _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_send_at_local_hour), '>=', intent_quiet_start_hour)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_send_at_local_hour), '<', intent_quiet_end_hour))))

def calc_send_intents_content_gate_passed(length_gate_passed, opt_out_gate_passed):
    """
    TRUE when the proposed text satisfies every content rule of the governing channel policy.
    
    Formula: =AND({{LengthGatePassed}}, {{OptOutGatePassed}})
    """
    return _erb.erb_and(_erb.erb_bool3(length_gate_passed), _erb.erb_bool3(opt_out_gate_passed))

def calc_send_intents_authorization_gate_passed(template_is_sendable, execution_has_legal_clearance, approval_is_human):
    """
    TRUE only when the template is validly approved, legal review has cleared, and the approving role is held by a human. The authorization gate.
    
    Formula: =AND({{TemplateIsSendable}}, AND({{ExecutionHasLegalClearance}}, {{ApprovalIsHuman}}))
    """
    return _erb.erb_and(_erb.erb_bool3(template_is_sendable), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(execution_has_legal_clearance), _erb.erb_bool3(approval_is_human))))

def calc_send_intents_delivered_intent_execution_key(has_resulting_delivery, resulting_delivery_was_transmitted, procedure_execution):
    """
    The execution id when this intent actually reached a person, otherwise empty string.
    
    Formula: =IF(AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}), {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_resulting_delivery), _erb.erb_bool3(resulting_delivery_was_transmitted))) else '')

def calc_send_intents_refused_on_opt_out_only(opt_out_gate_passed, length_gate_passed):
    """
    TRUE when the only content failure was a missing or mispositioned opt-out phrase.
    
    Formula: =AND(NOT({{OptOutGatePassed}}), {{LengthGatePassed}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(opt_out_gate_passed))), _erb.erb_bool3(length_gate_passed))

def calc_send_intents_exception_prescribed_an_alternative(refusal_cited_an_exception, resulting_delivery_exception):
    """
    TRUE when the documented exception for this refusal prescribes a different-channel send as the correct handling.
    
    Formula: =AND({{RefusalCitedAnException}}, {{ResultingDeliveryException}} <> "")
    """
    return _erb.erb_and(_erb.erb_bool3(refusal_cited_an_exception), _erb.erb_bool3((not (resulting_delivery_exception is None or resulting_delivery_exception == ""))))

def calc_send_intents_all_gate_inputs_resolved(consent_input_was_resolvable, policy_input_was_resolvable):
    """
    TRUE when every input my gates depend on was actually retrievable.
    
    Formula: =AND({{ConsentInputWasResolvable}}, {{PolicyInputWasResolvable}})
    """
    return _erb.erb_and(_erb.erb_bool3(consent_input_was_resolvable), _erb.erb_bool3(policy_input_was_resolvable))

def calc_send_intents_is_independently_confirmed(has_resulting_delivery, resulting_delivery_was_transmitted):
    """
    TRUE when this intent's decision was corroborated by an actual delivery record rather than resting solely on the pipeline's own say-so.
    
    Formula: =AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_resulting_delivery), _erb.erb_bool3(resulting_delivery_was_transmitted))

# Level 3

def calc_send_intents_timing_gate_passed(intent_policy_has_quiet_hours, intent_is_inside_quiet_window):
    """
    TRUE when transmitting now is permitted on timing grounds. The timing gate.
    
    Formula: =OR(NOT({{IntentPolicyHasQuietHours}}), NOT({{IntentIsInsideQuietWindow}}))
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(intent_policy_has_quiet_hours))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(intent_is_inside_quiet_window))))

def calc_send_intents_refused_on_approved_content(my_approval_was_in_force, content_gate_passed):
    """
    TRUE when the pipeline refused an intent on content grounds even though the template was approved.
    
    Formula: =AND({{MyApprovalWasInForce}}, NOT({{ContentGatePassed}}))
    """
    return _erb.erb_and(_erb.erb_bool3(my_approval_was_in_force), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(content_gate_passed))))

def calc_send_intents_prescribed_handling_was_performed(exception_prescribed_an_alternative, has_alternate_channel_attempt, alternate_attempt_was_cleared):
    """
    TRUE when the exception prescribed an alternate channel and an alternate intent was actually raised and cleared.
    
    Formula: =AND({{ExceptionPrescribedAnAlternative}}, AND({{HasAlternateChannelAttempt}}, {{AlternateAttemptWasCleared}}))
    """
    return _erb.erb_and(_erb.erb_bool3(exception_prescribed_an_alternative), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_alternate_channel_attempt), _erb.erb_bool3(alternate_attempt_was_cleared))))

def calc_send_intents_independently_confirmed_execution_key(is_independently_confirmed, procedure_execution):
    """
    Echoes the parent execution only for independently confirmed intents; empty otherwise.
    
    Formula: =IF({{IsIndependentlyConfirmed}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_independently_confirmed) else '')

# Level 4

def calc_send_intents_hours_until_window_opens(timing_gate_passed, proposed_send_at_local_hour, intent_quiet_end_hour):
    """
    How many hours the pipeline must defer before transmission becomes permitted; 0 when already permitted.
    
    Formula: =IF({{TimingGatePassed}}, 0, IF({{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}, {{IntentQuietEndHour}} - {{ProposedSendAtLocalHour}}, 24 - {{ProposedSendAtLocalHour}} + {{IntentQuietEndHour}}))
    """
    return _erb.erb_integer((0 if _erb.erb_bool3(timing_gate_passed) else (_erb.erb_sub(intent_quiet_end_hour, proposed_send_at_local_hour) if _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(proposed_send_at_local_hour), '<', intent_quiet_end_hour)) else _erb.erb_add(_erb.erb_sub(24, proposed_send_at_local_hour), intent_quiet_end_hour))))

def calc_send_intents_is_cleared_to_send(permission_gate_passed, timing_gate_passed, content_gate_passed, authorization_gate_passed):
    """
    TRUE only when all four gates pass. THE single column the pipeline reads before transmitting.
    
    Formula: =AND({{PermissionGatePassed}}, AND({{TimingGatePassed}}, AND({{ContentGatePassed}}, {{AuthorizationGatePassed}})))
    """
    return _erb.erb_and(_erb.erb_bool3(permission_gate_passed), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(timing_gate_passed), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(content_gate_passed), _erb.erb_bool3(authorization_gate_passed))))))

def calc_send_intents_is_approval_overridden_silently(refused_on_approved_content, approver_was_notified):
    """
    TRUE when the pipeline overrode a valid human approval on content grounds and told nobody.
    
    Formula: =AND({{RefusedOnApprovedContent}}, NOT({{ApproverWasNotified}}))
    """
    return _erb.erb_and(_erb.erb_bool3(refused_on_approved_content), _erb.erb_bool3(_erb.erb_not((approver_was_notified is True))))

def calc_send_intents_is_suppression_without_remedy(exception_prescribed_an_alternative, prescribed_handling_was_performed):
    """
    TRUE when we cited an exception that prescribed an alternate channel and then never performed it.
    
    Formula: =AND({{ExceptionPrescribedAnAlternative}}, NOT({{PrescribedHandlingWasPerformed}}))
    """
    return _erb.erb_and(_erb.erb_bool3(exception_prescribed_an_alternative), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(prescribed_handling_was_performed))))

def calc_send_intents_was_deferred_on_timing(timing_gate_passed, permission_gate_passed, content_gate_passed):
    """
    TRUE when the timing gate is the reason this intent did not clear, and every other gate passed.
    
    Formula: =AND(NOT({{TimingGatePassed}}), AND({{PermissionGatePassed}}, {{ContentGatePassed}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(timing_gate_passed))), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(permission_gate_passed), _erb.erb_bool3(content_gate_passed))))

# Level 5

def calc_send_intents_blocking_gate_name(is_cleared_to_send, permission_gate_passed, timing_gate_passed, content_gate_passed):
    """
    Names the first gate that refused, for the suppression reason; empty string when cleared.
    
    Formula: =IF({{IsClearedToSend}}, "", IF(NOT({{PermissionGatePassed}}), "Permission", IF(NOT({{TimingGatePassed}}), "Timing", IF(NOT({{ContentGatePassed}}), "Content", "Authorization"))))
    """
    return ('' if _erb.erb_bool3(is_cleared_to_send) else ('Permission' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(permission_gate_passed))) else ('Timing' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(timing_gate_passed))) else ('Content' if _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(content_gate_passed))) else 'Authorization'))))

def calc_send_intents_is_overridden_refusal(is_cleared_to_send, has_resulting_delivery, resulting_delivery_was_transmitted):
    """
    TRUE when a gate refused and the message was transmitted regardless. The override witness.
    
    Formula: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_resulting_delivery), _erb.erb_bool3(resulting_delivery_was_transmitted))))

def calc_send_intents_is_silently_dropped(is_cleared_to_send, has_resulting_delivery):
    """
    TRUE when a gate refused and no delivery record of any kind was produced -- the recipient vanished from the run.
    
    Formula: =AND(NOT({{IsClearedToSend}}), NOT({{HasResultingDelivery}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_resulting_delivery))))

def calc_send_intents_is_properly_handled_refusal(is_cleared_to_send, has_resulting_delivery, resulting_delivery_was_transmitted, refusal_cited_an_exception):
    """
    TRUE when a refusal correctly resulted in a non-transmitted delivery record citing a documented exception. The positive witness.
    
    Formula: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, AND(NOT({{ResultingDeliveryWasTransmitted}}), {{RefusalCitedAnException}})))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(has_resulting_delivery), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(resulting_delivery_was_transmitted))), _erb.erb_bool3(refusal_cited_an_exception))))))

def calc_send_intents_refusal_was_on_my_rules(is_cleared_to_send, content_gate_passed, timing_gate_passed):
    """
    TRUE when the refusal came from a communications rule I own -- content, length, opt-out, quiet hours.
    
    Formula: =AND(NOT({{IsClearedToSend}}), OR(NOT({{ContentGatePassed}}), NOT({{TimingGatePassed}})))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(content_gate_passed))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(timing_gate_passed))))))

def calc_send_intents_refusal_was_outside_my_control(is_cleared_to_send, permission_gate_passed, authorization_gate_passed):
    """
    TRUE when the refusal came from consent, reachability, or authorization -- none of which I can fix by editing a template.
    
    Formula: =AND(NOT({{IsClearedToSend}}), OR(NOT({{PermissionGatePassed}}), NOT({{AuthorizationGatePassed}})))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(permission_gate_passed))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(authorization_gate_passed))))))

def calc_send_intents_is_refused_with_no_alternative(is_cleared_to_send, has_alternate_channel_attempt):
    """
    TRUE when a send was refused and no attempt was ever made on any other channel.
    
    Formula: =AND(NOT({{IsClearedToSend}}), NOT({{HasAlternateChannelAttempt}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_alternate_channel_attempt))))

def calc_send_intents_is_unescalated_refusal(is_cleared_to_send, refusal_was_escalated):
    """
    TRUE when a refusal was recorded but no human role was ever told.
    
    Formula: =AND(NOT({{IsClearedToSend}}), NOT({{RefusalWasEscalated}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(refusal_was_escalated))))

def calc_send_intents_window_has_since_reopened(hours_until_window_opens, as_of_instant, evaluated_at):
    """
    TRUE when enough time has passed since evaluation that the quiet window this intent hit must have closed.
    
    Formula: =AND({{HoursUntilWindowOpens}} > 0, DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours") > {{HoursUntilWindowOpens}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(hours_until_window_opens, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_datetime_diff(as_of_instant, evaluated_at, 'hours'), '>', hours_until_window_opens)))

def calc_send_intents_is_stale_deferral(was_deferred_on_timing, deferral_age_hours):
    """
    TRUE when a deferred send has been waiting more than 24 hours -- longer than any quiet window can justify.
    
    Formula: =AND({{WasDeferredOnTiming}}, {{DeferralAgeHours}} > 24)
    """
    return _erb.erb_and(_erb.erb_bool3(was_deferred_on_timing), _erb.erb_bool3(_erb.erb_cmp(deferral_age_hours, '>', 24)))

def calc_send_intents_is_unevaluable_refusal(is_cleared_to_send, all_gate_inputs_resolved):
    """
    TRUE when I refused a send but at least one gate input could not be resolved -- so I do not actually know whether the rule was violated or merely unreadable.
    
    Formula: =AND(NOT({{IsClearedToSend}}), NOT({{AllGateInputsResolved}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(is_cleared_to_send))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(all_gate_inputs_resolved))))

# Level 6

def calc_send_intents_refusal_failure_execution_key(is_overridden_refusal, is_silently_dropped, procedure_execution):
    """
    Carries the execution id on any mishandled refusal; empty string otherwise.
    
    Formula: =IF(OR({{IsOverriddenRefusal}}, {{IsSilentlyDropped}}), {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(is_overridden_refusal), _erb.erb_bool3(is_silently_dropped))) else '')

def calc_send_intents_dropped_intent_execution_key(is_silently_dropped, procedure_execution):
    """
    The execution id when this intent was refused and left no record at all.
    
    Formula: =IF({{IsSilentlyDropped}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_silently_dropped) else '')

def calc_send_intents_is_unreported_refusal_on_my_rules(refusal_was_on_my_rules, approver_was_notified):
    """
    TRUE when a refusal I own and could have fixed was never surfaced to me.
    
    Formula: =AND({{RefusalWasOnMyRules}}, NOT({{ApproverWasNotified}}))
    """
    return _erb.erb_and(_erb.erb_bool3(refusal_was_on_my_rules), _erb.erb_bool3(_erb.erb_not((approver_was_notified is True))))

def calc_send_intents_is_unrecorded_refusal(is_silently_dropped, has_durable_refusal_record, refusal_cited_an_exception):
    """
    TRUE when I refused a send and produced no delivery record, no refusal record, and no exception. The refusal left no trace of any kind.
    
    Formula: =AND({{IsSilentlyDropped}}, AND(NOT({{HasDurableRefusalRecord}}), NOT({{RefusalCitedAnException}})))
    """
    return _erb.erb_and(_erb.erb_bool3(is_silently_dropped), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_durable_refusal_record))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(refusal_cited_an_exception))))))

def calc_send_intents_is_abandoned_deferral(was_deferred_on_timing, window_has_since_reopened, has_retry_attempt):
    """
    TRUE when a send was deferred for timing, the window has since reopened, and no retry was ever raised. A deferral silently converted into a cancellation.
    
    Formula: =AND({{WasDeferredOnTiming}}, AND({{WindowHasSinceReopened}}, NOT({{HasRetryAttempt}})))
    """
    return _erb.erb_and(_erb.erb_bool3(was_deferred_on_timing), _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(window_has_since_reopened), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_retry_attempt))))))

# Level 7

def calc_send_intents_unescalated_refusal_role_key(is_unrecorded_refusal, refusal_notified_role):
    """
    Echoes the role that should have been told about this refusal, but only when the refusal went unrecorded. Empty otherwise.
    
    Formula: =IF({{IsUnrecordedRefusal}}, {{RefusalNotifiedRole}}, "")
    """
    return (refusal_notified_role if _erb.erb_bool3(is_unrecorded_refusal) else '')

def calc_send_intents_unrecorded_refusal_execution_key(is_unrecorded_refusal, procedure_execution):
    """
    Echoes the parent procedure execution only for refusals nobody recorded; empty otherwise.
    
    Formula: =IF({{IsUnrecordedRefusal}}, {{ProcedureExecution}}, "")
    """
    return (procedure_execution if _erb.erb_bool3(is_unrecorded_refusal) else '')


def compute_send_intents_fields(record: dict) -> dict:
    """
    Compute all calculated fields for SendIntents.
    
    SendIntents (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_send_intents_name(result.get('recipient'), result.get('message_template'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['consent_gate_passed'] = calc_send_intents_consent_gate_passed(result.get('intent_requires_consent'), result.get('recipient_has_channel_consent'))
    except Exception as _field_exc:
        result['consent_gate_passed'] = None
        result.setdefault('_erb_errors', {})['consent_gate_passed'] = str(_field_exc)
    try:
        result['reachability_gate_passed'] = calc_send_intents_reachability_gate_passed(result.get('intent_channel'), result.get('recipient_is_sms_reachable'), result.get('recipient_is_email_reachable'))
    except Exception as _field_exc:
        result['reachability_gate_passed'] = None
        result.setdefault('_erb_errors', {})['reachability_gate_passed'] = str(_field_exc)
    try:
        result['intent_policy_has_quiet_hours'] = calc_send_intents_intent_policy_has_quiet_hours(result.get('intent_quiet_start_hour'), result.get('intent_quiet_end_hour'))
    except Exception as _field_exc:
        result['intent_policy_has_quiet_hours'] = None
        result.setdefault('_erb_errors', {})['intent_policy_has_quiet_hours'] = str(_field_exc)
    try:
        result['intent_quiet_window_wraps'] = calc_send_intents_intent_quiet_window_wraps(result.get('intent_quiet_start_hour'), result.get('intent_quiet_end_hour'))
    except Exception as _field_exc:
        result['intent_quiet_window_wraps'] = None
        result.setdefault('_erb_errors', {})['intent_quiet_window_wraps'] = str(_field_exc)
    try:
        result['length_gate_passed'] = calc_send_intents_length_gate_passed(result.get('proposed_body_length'), result.get('proposed_segment_count'), result.get('intent_max_segments'))
    except Exception as _field_exc:
        result['length_gate_passed'] = None
        result.setdefault('_erb_errors', {})['length_gate_passed'] = str(_field_exc)
    try:
        result['opt_out_gate_passed'] = calc_send_intents_opt_out_gate_passed(result.get('intent_required_opt_out_phrase'), result.get('proposed_opt_out_position'), result.get('intent_max_message_length'))
    except Exception as _field_exc:
        result['opt_out_gate_passed'] = None
        result.setdefault('_erb_errors', {})['opt_out_gate_passed'] = str(_field_exc)
    try:
        result['approval_is_human'] = calc_send_intents_approval_is_human(result.get('approval_role_agent_kind'))
    except Exception as _field_exc:
        result['approval_is_human'] = None
        result.setdefault('_erb_errors', {})['approval_is_human'] = str(_field_exc)
    try:
        result['has_resulting_delivery'] = calc_send_intents_has_resulting_delivery(result.get('resulting_delivery'))
    except Exception as _field_exc:
        result['has_resulting_delivery'] = None
        result.setdefault('_erb_errors', {})['has_resulting_delivery'] = str(_field_exc)
    try:
        result['refusal_cited_an_exception'] = calc_send_intents_refusal_cited_an_exception(result.get('resulting_delivery_exception'))
    except Exception as _field_exc:
        result['refusal_cited_an_exception'] = None
        result.setdefault('_erb_errors', {})['refusal_cited_an_exception'] = str(_field_exc)
    try:
        result['intent_execution_key'] = calc_send_intents_intent_execution_key(result.get('procedure_execution'))
    except Exception as _field_exc:
        result['intent_execution_key'] = None
        result.setdefault('_erb_errors', {})['intent_execution_key'] = str(_field_exc)
    try:
        result['my_approval_was_in_force'] = calc_send_intents_my_approval_was_in_force(result.get('template_is_sendable'))
    except Exception as _field_exc:
        result['my_approval_was_in_force'] = None
        result.setdefault('_erb_errors', {})['my_approval_was_in_force'] = str(_field_exc)
    try:
        result['has_alternate_channel_attempt'] = calc_send_intents_has_alternate_channel_attempt(result.get('alternate_channel_intent'))
    except Exception as _field_exc:
        result['has_alternate_channel_attempt'] = None
        result.setdefault('_erb_errors', {})['has_alternate_channel_attempt'] = str(_field_exc)
    try:
        result['has_durable_refusal_record'] = calc_send_intents_has_durable_refusal_record(result.get('refusal_recorded_at'))
    except Exception as _field_exc:
        result['has_durable_refusal_record'] = None
        result.setdefault('_erb_errors', {})['has_durable_refusal_record'] = str(_field_exc)
    try:
        result['refusal_was_escalated'] = calc_send_intents_refusal_was_escalated(result.get('refusal_notified_role'))
    except Exception as _field_exc:
        result['refusal_was_escalated'] = None
        result.setdefault('_erb_errors', {})['refusal_was_escalated'] = str(_field_exc)
    try:
        result['has_retry_attempt'] = calc_send_intents_has_retry_attempt(result.get('retry_intent'))
    except Exception as _field_exc:
        result['has_retry_attempt'] = None
        result.setdefault('_erb_errors', {})['has_retry_attempt'] = str(_field_exc)
    try:
        result['deferral_age_hours'] = calc_send_intents_deferral_age_hours(result.get('as_of_instant'), result.get('evaluated_at'))
    except Exception as _field_exc:
        result['deferral_age_hours'] = None
        result.setdefault('_erb_errors', {})['deferral_age_hours'] = str(_field_exc)
    try:
        result['consent_input_was_resolvable'] = calc_send_intents_consent_input_was_resolvable(result.get('recipient_consent_status_raw'))
    except Exception as _field_exc:
        result['consent_input_was_resolvable'] = None
        result.setdefault('_erb_errors', {})['consent_input_was_resolvable'] = str(_field_exc)
    try:
        result['policy_input_was_resolvable'] = calc_send_intents_policy_input_was_resolvable(result.get('intent_policy'))
    except Exception as _field_exc:
        result['policy_input_was_resolvable'] = None
        result.setdefault('_erb_errors', {})['policy_input_was_resolvable'] = str(_field_exc)
    try:
        result['is_self_witnessed_decision'] = calc_send_intents_is_self_witnessed_decision(result.get('gate_result_was_independently_confirmed'))
    except Exception as _field_exc:
        result['is_self_witnessed_decision'] = None
        result.setdefault('_erb_errors', {})['is_self_witnessed_decision'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['permission_gate_passed'] = calc_send_intents_permission_gate_passed(result.get('policy_is_active'), result.get('consent_gate_passed'), result.get('reachability_gate_passed'))
    except Exception as _field_exc:
        result['permission_gate_passed'] = None
        result.setdefault('_erb_errors', {})['permission_gate_passed'] = str(_field_exc)
    try:
        result['intent_is_inside_quiet_window'] = calc_send_intents_intent_is_inside_quiet_window(result.get('intent_quiet_window_wraps'), result.get('proposed_send_at_local_hour'), result.get('intent_quiet_start_hour'), result.get('intent_quiet_end_hour'))
    except Exception as _field_exc:
        result['intent_is_inside_quiet_window'] = None
        result.setdefault('_erb_errors', {})['intent_is_inside_quiet_window'] = str(_field_exc)
    try:
        result['content_gate_passed'] = calc_send_intents_content_gate_passed(result.get('length_gate_passed'), result.get('opt_out_gate_passed'))
    except Exception as _field_exc:
        result['content_gate_passed'] = None
        result.setdefault('_erb_errors', {})['content_gate_passed'] = str(_field_exc)
    try:
        result['authorization_gate_passed'] = calc_send_intents_authorization_gate_passed(result.get('template_is_sendable'), result.get('execution_has_legal_clearance'), result.get('approval_is_human'))
    except Exception as _field_exc:
        result['authorization_gate_passed'] = None
        result.setdefault('_erb_errors', {})['authorization_gate_passed'] = str(_field_exc)
    try:
        result['delivered_intent_execution_key'] = calc_send_intents_delivered_intent_execution_key(result.get('has_resulting_delivery'), result.get('resulting_delivery_was_transmitted'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['delivered_intent_execution_key'] = None
        result.setdefault('_erb_errors', {})['delivered_intent_execution_key'] = str(_field_exc)
    try:
        result['refused_on_opt_out_only'] = calc_send_intents_refused_on_opt_out_only(result.get('opt_out_gate_passed'), result.get('length_gate_passed'))
    except Exception as _field_exc:
        result['refused_on_opt_out_only'] = None
        result.setdefault('_erb_errors', {})['refused_on_opt_out_only'] = str(_field_exc)
    try:
        result['exception_prescribed_an_alternative'] = calc_send_intents_exception_prescribed_an_alternative(result.get('refusal_cited_an_exception'), result.get('resulting_delivery_exception'))
    except Exception as _field_exc:
        result['exception_prescribed_an_alternative'] = None
        result.setdefault('_erb_errors', {})['exception_prescribed_an_alternative'] = str(_field_exc)
    try:
        result['all_gate_inputs_resolved'] = calc_send_intents_all_gate_inputs_resolved(result.get('consent_input_was_resolvable'), result.get('policy_input_was_resolvable'))
    except Exception as _field_exc:
        result['all_gate_inputs_resolved'] = None
        result.setdefault('_erb_errors', {})['all_gate_inputs_resolved'] = str(_field_exc)
    try:
        result['is_independently_confirmed'] = calc_send_intents_is_independently_confirmed(result.get('has_resulting_delivery'), result.get('resulting_delivery_was_transmitted'))
    except Exception as _field_exc:
        result['is_independently_confirmed'] = None
        result.setdefault('_erb_errors', {})['is_independently_confirmed'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['timing_gate_passed'] = calc_send_intents_timing_gate_passed(result.get('intent_policy_has_quiet_hours'), result.get('intent_is_inside_quiet_window'))
    except Exception as _field_exc:
        result['timing_gate_passed'] = None
        result.setdefault('_erb_errors', {})['timing_gate_passed'] = str(_field_exc)
    try:
        result['refused_on_approved_content'] = calc_send_intents_refused_on_approved_content(result.get('my_approval_was_in_force'), result.get('content_gate_passed'))
    except Exception as _field_exc:
        result['refused_on_approved_content'] = None
        result.setdefault('_erb_errors', {})['refused_on_approved_content'] = str(_field_exc)
    try:
        result['prescribed_handling_was_performed'] = calc_send_intents_prescribed_handling_was_performed(result.get('exception_prescribed_an_alternative'), result.get('has_alternate_channel_attempt'), result.get('alternate_attempt_was_cleared'))
    except Exception as _field_exc:
        result['prescribed_handling_was_performed'] = None
        result.setdefault('_erb_errors', {})['prescribed_handling_was_performed'] = str(_field_exc)
    try:
        result['independently_confirmed_execution_key'] = calc_send_intents_independently_confirmed_execution_key(result.get('is_independently_confirmed'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['independently_confirmed_execution_key'] = None
        result.setdefault('_erb_errors', {})['independently_confirmed_execution_key'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['hours_until_window_opens'] = calc_send_intents_hours_until_window_opens(result.get('timing_gate_passed'), result.get('proposed_send_at_local_hour'), result.get('intent_quiet_end_hour'))
    except Exception as _field_exc:
        result['hours_until_window_opens'] = None
        result.setdefault('_erb_errors', {})['hours_until_window_opens'] = str(_field_exc)
    try:
        result['is_cleared_to_send'] = calc_send_intents_is_cleared_to_send(result.get('permission_gate_passed'), result.get('timing_gate_passed'), result.get('content_gate_passed'), result.get('authorization_gate_passed'))
    except Exception as _field_exc:
        result['is_cleared_to_send'] = None
        result.setdefault('_erb_errors', {})['is_cleared_to_send'] = str(_field_exc)
    try:
        result['is_approval_overridden_silently'] = calc_send_intents_is_approval_overridden_silently(result.get('refused_on_approved_content'), result.get('approver_was_notified'))
    except Exception as _field_exc:
        result['is_approval_overridden_silently'] = None
        result.setdefault('_erb_errors', {})['is_approval_overridden_silently'] = str(_field_exc)
    try:
        result['is_suppression_without_remedy'] = calc_send_intents_is_suppression_without_remedy(result.get('exception_prescribed_an_alternative'), result.get('prescribed_handling_was_performed'))
    except Exception as _field_exc:
        result['is_suppression_without_remedy'] = None
        result.setdefault('_erb_errors', {})['is_suppression_without_remedy'] = str(_field_exc)
    try:
        result['was_deferred_on_timing'] = calc_send_intents_was_deferred_on_timing(result.get('timing_gate_passed'), result.get('permission_gate_passed'), result.get('content_gate_passed'))
    except Exception as _field_exc:
        result['was_deferred_on_timing'] = None
        result.setdefault('_erb_errors', {})['was_deferred_on_timing'] = str(_field_exc)

    # Level 5 calculations
    try:
        result['blocking_gate_name'] = calc_send_intents_blocking_gate_name(result.get('is_cleared_to_send'), result.get('permission_gate_passed'), result.get('timing_gate_passed'), result.get('content_gate_passed'))
    except Exception as _field_exc:
        result['blocking_gate_name'] = None
        result.setdefault('_erb_errors', {})['blocking_gate_name'] = str(_field_exc)
    try:
        result['is_overridden_refusal'] = calc_send_intents_is_overridden_refusal(result.get('is_cleared_to_send'), result.get('has_resulting_delivery'), result.get('resulting_delivery_was_transmitted'))
    except Exception as _field_exc:
        result['is_overridden_refusal'] = None
        result.setdefault('_erb_errors', {})['is_overridden_refusal'] = str(_field_exc)
    try:
        result['is_silently_dropped'] = calc_send_intents_is_silently_dropped(result.get('is_cleared_to_send'), result.get('has_resulting_delivery'))
    except Exception as _field_exc:
        result['is_silently_dropped'] = None
        result.setdefault('_erb_errors', {})['is_silently_dropped'] = str(_field_exc)
    try:
        result['is_properly_handled_refusal'] = calc_send_intents_is_properly_handled_refusal(result.get('is_cleared_to_send'), result.get('has_resulting_delivery'), result.get('resulting_delivery_was_transmitted'), result.get('refusal_cited_an_exception'))
    except Exception as _field_exc:
        result['is_properly_handled_refusal'] = None
        result.setdefault('_erb_errors', {})['is_properly_handled_refusal'] = str(_field_exc)
    try:
        result['refusal_was_on_my_rules'] = calc_send_intents_refusal_was_on_my_rules(result.get('is_cleared_to_send'), result.get('content_gate_passed'), result.get('timing_gate_passed'))
    except Exception as _field_exc:
        result['refusal_was_on_my_rules'] = None
        result.setdefault('_erb_errors', {})['refusal_was_on_my_rules'] = str(_field_exc)
    try:
        result['refusal_was_outside_my_control'] = calc_send_intents_refusal_was_outside_my_control(result.get('is_cleared_to_send'), result.get('permission_gate_passed'), result.get('authorization_gate_passed'))
    except Exception as _field_exc:
        result['refusal_was_outside_my_control'] = None
        result.setdefault('_erb_errors', {})['refusal_was_outside_my_control'] = str(_field_exc)
    try:
        result['is_refused_with_no_alternative'] = calc_send_intents_is_refused_with_no_alternative(result.get('is_cleared_to_send'), result.get('has_alternate_channel_attempt'))
    except Exception as _field_exc:
        result['is_refused_with_no_alternative'] = None
        result.setdefault('_erb_errors', {})['is_refused_with_no_alternative'] = str(_field_exc)
    try:
        result['is_unescalated_refusal'] = calc_send_intents_is_unescalated_refusal(result.get('is_cleared_to_send'), result.get('refusal_was_escalated'))
    except Exception as _field_exc:
        result['is_unescalated_refusal'] = None
        result.setdefault('_erb_errors', {})['is_unescalated_refusal'] = str(_field_exc)
    try:
        result['window_has_since_reopened'] = calc_send_intents_window_has_since_reopened(result.get('hours_until_window_opens'), result.get('as_of_instant'), result.get('evaluated_at'))
    except Exception as _field_exc:
        result['window_has_since_reopened'] = None
        result.setdefault('_erb_errors', {})['window_has_since_reopened'] = str(_field_exc)
    try:
        result['is_stale_deferral'] = calc_send_intents_is_stale_deferral(result.get('was_deferred_on_timing'), result.get('deferral_age_hours'))
    except Exception as _field_exc:
        result['is_stale_deferral'] = None
        result.setdefault('_erb_errors', {})['is_stale_deferral'] = str(_field_exc)
    try:
        result['is_unevaluable_refusal'] = calc_send_intents_is_unevaluable_refusal(result.get('is_cleared_to_send'), result.get('all_gate_inputs_resolved'))
    except Exception as _field_exc:
        result['is_unevaluable_refusal'] = None
        result.setdefault('_erb_errors', {})['is_unevaluable_refusal'] = str(_field_exc)

    # Level 6 calculations
    try:
        result['refusal_failure_execution_key'] = calc_send_intents_refusal_failure_execution_key(result.get('is_overridden_refusal'), result.get('is_silently_dropped'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['refusal_failure_execution_key'] = None
        result.setdefault('_erb_errors', {})['refusal_failure_execution_key'] = str(_field_exc)
    try:
        result['dropped_intent_execution_key'] = calc_send_intents_dropped_intent_execution_key(result.get('is_silently_dropped'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['dropped_intent_execution_key'] = None
        result.setdefault('_erb_errors', {})['dropped_intent_execution_key'] = str(_field_exc)
    try:
        result['is_unreported_refusal_on_my_rules'] = calc_send_intents_is_unreported_refusal_on_my_rules(result.get('refusal_was_on_my_rules'), result.get('approver_was_notified'))
    except Exception as _field_exc:
        result['is_unreported_refusal_on_my_rules'] = None
        result.setdefault('_erb_errors', {})['is_unreported_refusal_on_my_rules'] = str(_field_exc)
    try:
        result['is_unrecorded_refusal'] = calc_send_intents_is_unrecorded_refusal(result.get('is_silently_dropped'), result.get('has_durable_refusal_record'), result.get('refusal_cited_an_exception'))
    except Exception as _field_exc:
        result['is_unrecorded_refusal'] = None
        result.setdefault('_erb_errors', {})['is_unrecorded_refusal'] = str(_field_exc)
    try:
        result['is_abandoned_deferral'] = calc_send_intents_is_abandoned_deferral(result.get('was_deferred_on_timing'), result.get('window_has_since_reopened'), result.get('has_retry_attempt'))
    except Exception as _field_exc:
        result['is_abandoned_deferral'] = None
        result.setdefault('_erb_errors', {})['is_abandoned_deferral'] = str(_field_exc)

    # Level 7 calculations
    try:
        result['unescalated_refusal_role_key'] = calc_send_intents_unescalated_refusal_role_key(result.get('is_unrecorded_refusal'), result.get('refusal_notified_role'))
    except Exception as _field_exc:
        result['unescalated_refusal_role_key'] = None
        result.setdefault('_erb_errors', {})['unescalated_refusal_role_key'] = str(_field_exc)
    try:
        result['unrecorded_refusal_execution_key'] = calc_send_intents_unrecorded_refusal_execution_key(result.get('is_unrecorded_refusal'), result.get('procedure_execution'))
    except Exception as _field_exc:
        result['unrecorded_refusal_execution_key'] = None
        result.setdefault('_erb_errors', {})['unrecorded_refusal_execution_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'blocking_gate_name', 'refusal_failure_execution_key', 'intent_execution_key', 'delivered_intent_execution_key', 'dropped_intent_execution_key', 'unescalated_refusal_role_key', 'unrecorded_refusal_execution_key', 'independently_confirmed_execution_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AGENTDECISIONRECORDS
# AgentDecisionRecords (added by witness loop 1).
# =============================================================================

# Level 1

def calc_agent_decision_records_name(deciding_agent, decision_summary):
    """
    Human-readable calculated display alias for the AgentDecisionRecords row.
    
    Formula: ={{DecidingAgent}} & ": " & LEFT({{DecisionSummary}}, 60)
    """
    return (str(deciding_agent or "") + ': ' + str(((decision_summary or "")[:(60 or 0)]) if ((decision_summary or "")[:(60 or 0)]) is not None else ""))

def calc_agent_decision_records_was_overridden(human_disposition):
    """
    TRUE when a human corrected or reversed this decision.
    
    Formula: =OR({{HumanDisposition}} = "Corrected", {{HumanDisposition}} = "Reversed")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(human_disposition), 'Corrected')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(human_disposition), 'Reversed')))

def calc_agent_decision_records_was_reviewed(human_disposition):
    """
    TRUE when a human actually dispositioned this decision.
    
    Formula: =AND({{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed")
    """
    return _erb.erb_and(_erb.erb_bool3((not (human_disposition is None or human_disposition == ""))), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(human_disposition), 'NotReviewed')))

def calc_agent_decision_records_role_assignment_when_scored(under_role_assignment):
    """
    Echoes the governing role assignment when one is recorded, blank otherwise.
    
    Formula: =IF({{UnderRoleAssignment}} <> "", {{UnderRoleAssignment}}, "")
    """
    return (under_role_assignment if _erb.erb_bool3((not (under_role_assignment is None or under_role_assignment == ""))) else '')

def calc_agent_decision_records_boundary_match_key(step_of_decision, deciding_agent_kind, decision_kind):
    """
    Composite key of step, deciding agent kind, and decision kind for this decision.
    
    Formula: ={{StepOfDecision}} & "|" & {{DecidingAgentKind}} & "|" & {{DecisionKind}}
    """
    return (str(step_of_decision or "") + '|' + str(deciding_agent_kind or "") + '|' + str(decision_kind or ""))

def calc_agent_decision_records_violated_authority_boundary(matching_boundary_count):
    """
    TRUE when this decision matches an authority boundary that forbids it.
    
    Formula: ={{MatchingBoundaryCount}} > 0
    """
    return _erb.erb_cmp(matching_boundary_count, '>', 0)

def calc_agent_decision_records_has_human_confirmation(reviewer_agent_kind, human_disposition):
    """
    TRUE when a human agent actually dispositioned this decision.
    
    Formula: =AND({{ReviewerAgentKind}} = "Human", {{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed")
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(reviewer_agent_kind, 'Human')), _erb.erb_bool3((not (human_disposition is None or human_disposition == ""))), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(human_disposition), 'NotReviewed')))

def calc_agent_decision_records_needs_human_confirmation(deciding_agent_kind, materiality_band):
    """
    TRUE when a non-human agent made a material or escalated decision.
    
    Formula: =AND(NOT({{DecidingAgentKind}} = "Human"), OR({{MaterialityBand}} = "Material", {{MaterialityBand}} = "Escalated"))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(_erb.erb_eq(deciding_agent_kind, 'Human')))), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(materiality_band), 'Material')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(materiality_band), 'Escalated')))))

def calc_agent_decision_records_review_latency_minutes(reviewed_at, decided_at):
    """
    Minutes between the decision and its human disposition; 0 when never reviewed. Declared number, not integer: DATETIME_DIFF returns a numeric and an integer cast makes the entire view fail on read.
    
    Formula: =IF({{ReviewedAt}} = "", 0, DATETIME_DIFF({{ReviewedAt}}, {{DecidedAt}}, "minutes"))
    """
    return (0 if _erb.erb_bool3((reviewed_at is None or reviewed_at == "")) else _erb.erb_datetime_diff(reviewed_at, decided_at, 'minutes'))

def calc_agent_decision_records_is_draft_kind(decision_kind):
    """
    TRUE when this decision produced text — the drafter's own output class.
    
    Formula: =OR({{DecisionKind}} = "Draft", {{DecisionKind}} = "Commitment")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(decision_kind), 'Draft')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(decision_kind), 'Commitment')))

# Level 2

def calc_agent_decision_records_deciding_agent_when_overridden(was_overridden, deciding_agent):
    """
    Echoes the deciding agent id when the decision was overridden, blank otherwise.
    
    Formula: =IF({{WasOverridden}}, {{DecidingAgent}}, "")
    """
    return (deciding_agent if _erb.erb_bool3(was_overridden) else '')

def calc_agent_decision_records_role_assignment_when_overridden(was_overridden, under_role_assignment):
    """
    Echoes the governing role assignment when the decision was overridden, blank otherwise.
    
    Formula: =IF({{WasOverridden}}, {{UnderRoleAssignment}}, "")
    """
    return (under_role_assignment if _erb.erb_bool3(was_overridden) else '')

def calc_agent_decision_records_is_unconfirmed_non_human_decision(needs_human_confirmation, has_human_confirmation):
    """
    TRUE when a material non-human decision was never confirmed by a human.
    
    Formula: =AND({{NeedsHumanConfirmation}}, NOT({{HasHumanConfirmation}}))
    """
    return _erb.erb_and(_erb.erb_bool3(needs_human_confirmation), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_human_confirmation))))

def calc_agent_decision_records_agent_when_boundary_violated(violated_authority_boundary, deciding_agent):
    """
    Echoes the deciding agent id when the decision violated a boundary, blank otherwise.
    
    Formula: =IF({{ViolatedAuthorityBoundary}}, {{DecidingAgent}}, "")
    """
    return (deciding_agent if _erb.erb_bool3(violated_authority_boundary) else '')

def calc_agent_decision_records_agent_when_draft_overridden(is_draft_kind, was_overridden, deciding_agent):
    """
    Echoes the deciding agent id when a drafting decision was overridden, blank otherwise.
    
    Formula: =IF(AND({{IsDraftKind}}, {{WasOverridden}}), {{DecidingAgent}}, "")
    """
    return (deciding_agent if _erb.erb_bool3(_erb.erb_and(_erb.erb_bool3(is_draft_kind), _erb.erb_bool3(was_overridden))) else '')

def calc_agent_decision_records_agent_when_draft(is_draft_kind, deciding_agent):
    """
    Echoes the deciding agent id when the decision produced text, blank otherwise.
    
    Formula: =IF({{IsDraftKind}}, {{DecidingAgent}}, "")
    """
    return (deciding_agent if _erb.erb_bool3(is_draft_kind) else '')

def calc_agent_decision_records_is_error_correction(was_overridden, override_reason_kind):
    """
    TRUE when the override corrected something I got wrong.
    
    Formula: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "ErrorCorrection")
    """
    return _erb.erb_and(_erb.erb_bool3(was_overridden), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(override_reason_kind), 'ErrorCorrection')))

def calc_agent_decision_records_is_reserved_judgment_override(was_overridden, override_reason_kind):
    """
    TRUE when the override was a human exercising authority the procedure always reserved to them.
    
    Formula: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "JudgmentReserved")
    """
    return _erb.erb_and(_erb.erb_bool3(was_overridden), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(override_reason_kind), 'JudgmentReserved')))

def calc_agent_decision_records_override_reason_is_recorded(was_overridden, override_reason_kind):
    """
    TRUE when an override carries a stated reason.
    
    Formula: =AND({{WasOverridden}}, {{OverrideReasonKind}} <> "")
    """
    return _erb.erb_and(_erb.erb_bool3(was_overridden), _erb.erb_bool3((not (override_reason_kind is None or override_reason_kind == ""))))

def calc_agent_decision_records_boundary_violation_role_assignment_key(violated_authority_boundary, under_role_assignment):
    """
    Echoes the role assignment this decision was made under, but only when the decision violated an authority boundary. Empty otherwise.
    
    Formula: =IF({{ViolatedAuthorityBoundary}}, {{UnderRoleAssignment}}, "")
    """
    return (under_role_assignment if _erb.erb_bool3(violated_authority_boundary) else '')

# Level 3

def calc_agent_decision_records_step_execution_when_unconfirmed(is_unconfirmed_non_human_decision, step_execution):
    """
    Echoes the step-execution id when the decision is an unconfirmed non-human material decision, blank otherwise.
    
    Formula: =IF({{IsUnconfirmedNonHumanDecision}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(is_unconfirmed_non_human_decision) else '')

def calc_agent_decision_records_is_unexplained_override(was_overridden, override_reason_is_recorded):
    """
    TRUE when my output was changed and nobody recorded why.
    
    Formula: =AND({{WasOverridden}}, NOT({{OverrideReasonIsRecorded}}))
    """
    return _erb.erb_and(_erb.erb_bool3(was_overridden), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(override_reason_is_recorded))))

def calc_agent_decision_records_error_correction_role_assignment_key(is_error_correction, under_role_assignment):
    """
    The role assignment id when this decision was overridden as an error correction, otherwise empty.
    
    Formula: =IF({{IsErrorCorrection}}, {{UnderRoleAssignment}}, "")
    """
    return (under_role_assignment if _erb.erb_bool3(is_error_correction) else '')


def compute_agent_decision_records_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AgentDecisionRecords.
    
    AgentDecisionRecords (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_agent_decision_records_name(result.get('deciding_agent'), result.get('decision_summary'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['was_overridden'] = calc_agent_decision_records_was_overridden(result.get('human_disposition'))
    except Exception as _field_exc:
        result['was_overridden'] = None
        result.setdefault('_erb_errors', {})['was_overridden'] = str(_field_exc)
    try:
        result['was_reviewed'] = calc_agent_decision_records_was_reviewed(result.get('human_disposition'))
    except Exception as _field_exc:
        result['was_reviewed'] = None
        result.setdefault('_erb_errors', {})['was_reviewed'] = str(_field_exc)
    try:
        result['role_assignment_when_scored'] = calc_agent_decision_records_role_assignment_when_scored(result.get('under_role_assignment'))
    except Exception as _field_exc:
        result['role_assignment_when_scored'] = None
        result.setdefault('_erb_errors', {})['role_assignment_when_scored'] = str(_field_exc)
    try:
        result['boundary_match_key'] = calc_agent_decision_records_boundary_match_key(result.get('step_of_decision'), result.get('deciding_agent_kind'), result.get('decision_kind'))
    except Exception as _field_exc:
        result['boundary_match_key'] = None
        result.setdefault('_erb_errors', {})['boundary_match_key'] = str(_field_exc)
    try:
        result['violated_authority_boundary'] = calc_agent_decision_records_violated_authority_boundary(result.get('matching_boundary_count'))
    except Exception as _field_exc:
        result['violated_authority_boundary'] = None
        result.setdefault('_erb_errors', {})['violated_authority_boundary'] = str(_field_exc)
    try:
        result['has_human_confirmation'] = calc_agent_decision_records_has_human_confirmation(result.get('reviewer_agent_kind'), result.get('human_disposition'))
    except Exception as _field_exc:
        result['has_human_confirmation'] = None
        result.setdefault('_erb_errors', {})['has_human_confirmation'] = str(_field_exc)
    try:
        result['needs_human_confirmation'] = calc_agent_decision_records_needs_human_confirmation(result.get('deciding_agent_kind'), result.get('materiality_band'))
    except Exception as _field_exc:
        result['needs_human_confirmation'] = None
        result.setdefault('_erb_errors', {})['needs_human_confirmation'] = str(_field_exc)
    try:
        result['review_latency_minutes'] = calc_agent_decision_records_review_latency_minutes(result.get('reviewed_at'), result.get('decided_at'))
    except Exception as _field_exc:
        result['review_latency_minutes'] = None
        result.setdefault('_erb_errors', {})['review_latency_minutes'] = str(_field_exc)
    try:
        result['is_draft_kind'] = calc_agent_decision_records_is_draft_kind(result.get('decision_kind'))
    except Exception as _field_exc:
        result['is_draft_kind'] = None
        result.setdefault('_erb_errors', {})['is_draft_kind'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['deciding_agent_when_overridden'] = calc_agent_decision_records_deciding_agent_when_overridden(result.get('was_overridden'), result.get('deciding_agent'))
    except Exception as _field_exc:
        result['deciding_agent_when_overridden'] = None
        result.setdefault('_erb_errors', {})['deciding_agent_when_overridden'] = str(_field_exc)
    try:
        result['role_assignment_when_overridden'] = calc_agent_decision_records_role_assignment_when_overridden(result.get('was_overridden'), result.get('under_role_assignment'))
    except Exception as _field_exc:
        result['role_assignment_when_overridden'] = None
        result.setdefault('_erb_errors', {})['role_assignment_when_overridden'] = str(_field_exc)
    try:
        result['is_unconfirmed_non_human_decision'] = calc_agent_decision_records_is_unconfirmed_non_human_decision(result.get('needs_human_confirmation'), result.get('has_human_confirmation'))
    except Exception as _field_exc:
        result['is_unconfirmed_non_human_decision'] = None
        result.setdefault('_erb_errors', {})['is_unconfirmed_non_human_decision'] = str(_field_exc)
    try:
        result['agent_when_boundary_violated'] = calc_agent_decision_records_agent_when_boundary_violated(result.get('violated_authority_boundary'), result.get('deciding_agent'))
    except Exception as _field_exc:
        result['agent_when_boundary_violated'] = None
        result.setdefault('_erb_errors', {})['agent_when_boundary_violated'] = str(_field_exc)
    try:
        result['agent_when_draft_overridden'] = calc_agent_decision_records_agent_when_draft_overridden(result.get('is_draft_kind'), result.get('was_overridden'), result.get('deciding_agent'))
    except Exception as _field_exc:
        result['agent_when_draft_overridden'] = None
        result.setdefault('_erb_errors', {})['agent_when_draft_overridden'] = str(_field_exc)
    try:
        result['agent_when_draft'] = calc_agent_decision_records_agent_when_draft(result.get('is_draft_kind'), result.get('deciding_agent'))
    except Exception as _field_exc:
        result['agent_when_draft'] = None
        result.setdefault('_erb_errors', {})['agent_when_draft'] = str(_field_exc)
    try:
        result['is_error_correction'] = calc_agent_decision_records_is_error_correction(result.get('was_overridden'), result.get('override_reason_kind'))
    except Exception as _field_exc:
        result['is_error_correction'] = None
        result.setdefault('_erb_errors', {})['is_error_correction'] = str(_field_exc)
    try:
        result['is_reserved_judgment_override'] = calc_agent_decision_records_is_reserved_judgment_override(result.get('was_overridden'), result.get('override_reason_kind'))
    except Exception as _field_exc:
        result['is_reserved_judgment_override'] = None
        result.setdefault('_erb_errors', {})['is_reserved_judgment_override'] = str(_field_exc)
    try:
        result['override_reason_is_recorded'] = calc_agent_decision_records_override_reason_is_recorded(result.get('was_overridden'), result.get('override_reason_kind'))
    except Exception as _field_exc:
        result['override_reason_is_recorded'] = None
        result.setdefault('_erb_errors', {})['override_reason_is_recorded'] = str(_field_exc)
    try:
        result['boundary_violation_role_assignment_key'] = calc_agent_decision_records_boundary_violation_role_assignment_key(result.get('violated_authority_boundary'), result.get('under_role_assignment'))
    except Exception as _field_exc:
        result['boundary_violation_role_assignment_key'] = None
        result.setdefault('_erb_errors', {})['boundary_violation_role_assignment_key'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['step_execution_when_unconfirmed'] = calc_agent_decision_records_step_execution_when_unconfirmed(result.get('is_unconfirmed_non_human_decision'), result.get('step_execution'))
    except Exception as _field_exc:
        result['step_execution_when_unconfirmed'] = None
        result.setdefault('_erb_errors', {})['step_execution_when_unconfirmed'] = str(_field_exc)
    try:
        result['is_unexplained_override'] = calc_agent_decision_records_is_unexplained_override(result.get('was_overridden'), result.get('override_reason_is_recorded'))
    except Exception as _field_exc:
        result['is_unexplained_override'] = None
        result.setdefault('_erb_errors', {})['is_unexplained_override'] = str(_field_exc)
    try:
        result['error_correction_role_assignment_key'] = calc_agent_decision_records_error_correction_role_assignment_key(result.get('is_error_correction'), result.get('under_role_assignment'))
    except Exception as _field_exc:
        result['error_correction_role_assignment_key'] = None
        result.setdefault('_erb_errors', {})['error_correction_role_assignment_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'deciding_agent_when_overridden', 'role_assignment_when_scored', 'role_assignment_when_overridden', 'boundary_match_key', 'step_execution_when_unconfirmed', 'agent_when_boundary_violated', 'agent_when_draft_overridden', 'agent_when_draft', 'error_correction_role_assignment_key', 'boundary_violation_role_assignment_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# DELIVEREDCOMMUNICATIONS
# Everything above is a proxy for the real question, which is instance-level: THIS message, to THIS recipient, rendered from THIS template, authorized by THIS approval. The model has MessageTemplates and CommunicationPolicies as specifications and no record of a single thing ever sent. Without an instance table, 'can I show that what was sent matched what I approved' is permanently unanswerable rather than merely unanswered — and it is the question a disputing employee actually asks. Extension term (urn:effortless:pko-extension#DeliveredCommunication); PKO has no native class for a delivered artifact instance.
# =============================================================================

# Level 1

def calc_delivered_communications_name(channel, recipient_key, sent_at):
    """
    Human-readable calculated display alias.
    
    Formula: ={{Channel}} & " -> " & {{RecipientKey}} & " @ " & {{SentAt}}
    """
    return (str(channel or "") + ' -> ' + str(recipient_key or "") + ' @ ' + _erb.erb_timestamptz_text(sent_at))

def calc_delivered_communications_has_authorization(authorizing_step_execution):
    """
    TRUE when this delivery names the approval that authorized it.
    
    Formula: ={{AuthorizingStepExecution}} <> "" 
    """
    return (not (authorizing_step_execution is None or authorizing_step_execution == ""))

def calc_delivered_communications_content_matches_approval(rendered_content_hash, approved_content_hash):
    """
    TRUE when the bytes delivered are the bytes approved.
    
    Formula: ={{RenderedContentHash}} = {{ApprovedContentHash}}
    """
    return _erb.erb_eq(_erb.erb_nullif(rendered_content_hash), _erb.erb_nullif(approved_content_hash))

def calc_delivered_communications_was_approved_before_sending(authorized_at, sent_at):
    """
    TRUE when the approval preceded the send.
    
    Formula: ={{AuthorizedAt}} <= {{SentAt}}
    """
    return _erb.erb_cmp(authorized_at, '<=', _erb.erb_nullif(sent_at))

# Level 2

def calc_delivered_communications_is_defensible(has_authorization, content_matches_approval, was_approved_before_sending):
    """
    TRUE when this delivery can be defended in a dispute: authorized, unaltered, and approved beforehand.
    
    Formula: =AND({{HasAuthorization}}, {{ContentMatchesApproval}}, {{WasApprovedBeforeSending}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_authorization), _erb.erb_bool3(content_matches_approval), _erb.erb_bool3(was_approved_before_sending))


def compute_delivered_communications_fields(record: dict) -> dict:
    """
    Compute all calculated fields for DeliveredCommunications.
    
    Everything above is a proxy for the real question, which is instance-level: THIS message, to THIS recipient, rendered from THIS template, authorized by THIS approval. The model has MessageTemplates and CommunicationPolicies as specifications and no record of a single thing ever sent. Without an instance table, 'can I show that what was sent matched what I approved' is permanently unanswerable rather than merely unanswered — and it is the question a disputing employee actually asks. Extension term (urn:effortless:pko-extension#DeliveredCommunication); PKO has no native class for a delivered artifact instance.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_delivered_communications_name(result.get('channel'), result.get('recipient_key'), result.get('sent_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_authorization'] = calc_delivered_communications_has_authorization(result.get('authorizing_step_execution'))
    except Exception as _field_exc:
        result['has_authorization'] = None
        result.setdefault('_erb_errors', {})['has_authorization'] = str(_field_exc)
    try:
        result['content_matches_approval'] = calc_delivered_communications_content_matches_approval(result.get('rendered_content_hash'), result.get('approved_content_hash'))
    except Exception as _field_exc:
        result['content_matches_approval'] = None
        result.setdefault('_erb_errors', {})['content_matches_approval'] = str(_field_exc)
    try:
        result['was_approved_before_sending'] = calc_delivered_communications_was_approved_before_sending(result.get('authorized_at'), result.get('sent_at'))
    except Exception as _field_exc:
        result['was_approved_before_sending'] = None
        result.setdefault('_erb_errors', {})['was_approved_before_sending'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_defensible'] = calc_delivered_communications_is_defensible(result.get('has_authorization'), result.get('content_matches_approval'), result.get('was_approved_before_sending'))
    except Exception as _field_exc:
        result['is_defensible'] = None
        result.setdefault('_erb_errors', {})['is_defensible'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# AUTHORITYBOUNDARIES
# AuthorityBoundaries (added by witness loop 1).
# =============================================================================

# Level 1

def calc_authority_boundaries_name(forbidden_agent_kind, forbidden_decision_kind):
    """
    Human-readable calculated display alias for the AuthorityBoundaries row.
    
    Formula: ={{ForbiddenAgentKind}} & " may not " & {{ForbiddenDecisionKind}}
    """
    return (str(forbidden_agent_kind or "") + ' may not ' + str(forbidden_decision_kind or ""))

def calc_authority_boundaries_is_currently_binding(status, valid_from, as_of_instant, valid_to):
    """
    TRUE when this boundary is approved and inside its valid-time window right now.
    
    Formula: =AND({{Status}} = "Approved", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}))
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(status), 'Approved')), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_from), '<=', as_of_instant)), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3((valid_to is None or valid_to == "")), _erb.erb_bool3(_erb.erb_cmp(_erb.erb_nullif(valid_to), '>', as_of_instant)))))

def calc_authority_boundaries_boundary_match_key(step, forbidden_agent_kind, forbidden_decision_kind):
    """
    Composite key of step, forbidden agent kind, and forbidden decision kind.
    
    Formula: ={{Step}} & "|" & {{ForbiddenAgentKind}} & "|" & {{ForbiddenDecisionKind}}
    """
    return (str(step or "") + '|' + str(forbidden_agent_kind or "") + '|' + str(forbidden_decision_kind or ""))

def calc_authority_boundaries_has_ratifying_fragment(ratified_by_knowledge_fragment):
    """
    TRUE when this boundary names a knowledge fragment as its justification. FALSE means the rule constrains behaviour on nobody's recorded authority — strictly worse than resting on an expired claim, and previously invisible because the ratification lookup returned NULL.
    
    Formula: ={{RatifiedByKnowledgeFragment}} <> "" 
    """
    return (not (ratified_by_knowledge_fragment is None or ratified_by_knowledge_fragment == ""))

# Level 2

def calc_authority_boundaries_step_when_binding(is_currently_binding, step):
    """
    Echoes the step id when this boundary is currently binding, blank otherwise.
    
    Formula: =IF({{IsCurrentlyBinding}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(is_currently_binding) else '')

def calc_authority_boundaries_is_untested(is_currently_binding, violation_count):
    """
    TRUE when a binding boundary has never been triggered by any recorded decision.
    
    Formula: =AND({{IsCurrentlyBinding}}, {{ViolationCount}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_binding), _erb.erb_bool3(_erb.erb_eq(violation_count, 0)))

def calc_authority_boundaries_is_unwarranted(is_currently_binding, has_ratifying_fragment, ratifying_fragment_is_valid):
    """
    TRUE when a binding constraint on authority rests on no ratifying claim at all, or on one that is no longer valid. Either way the rule is being enforced without a live justification.
    
    Formula: =AND({{IsCurrentlyBinding}}, OR(NOT({{HasRatifyingFragment}}), NOT({{RatifyingFragmentIsValid}})))
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_binding), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(has_ratifying_fragment))), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(ratifying_fragment_is_valid))))))

def calc_authority_boundaries_warrant_is_thin(is_currently_binding, ratifying_fragment_is_overdue, ratifying_fragment_is_single_witness):
    """
    A binding boundary whose ratifying knowledge is either overdue for review or single-sourced — still valid, but weakly warranted.
    
    Formula: =AND({{IsCurrentlyBinding}}, OR({{RatifyingFragmentIsOverdue}}, {{RatifyingFragmentIsSingleWitness}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_binding), _erb.erb_bool3(_erb.erb_or(_erb.erb_bool3(ratifying_fragment_is_overdue), _erb.erb_bool3(ratifying_fragment_is_single_witness))))

def calc_authority_boundaries_ratifying_fragment_key(is_currently_binding, ratified_by_knowledge_fragment):
    """
    Composite-key echo: the fragment ratifying this boundary when the boundary is currently binding, blank otherwise.
    
    Formula: =IF({{IsCurrentlyBinding}}, {{RatifiedByKnowledgeFragment}}, "")
    """
    return (ratified_by_knowledge_fragment if _erb.erb_bool3(is_currently_binding) else '')

def calc_authority_boundaries_ratification_lapsed(has_ratifying_fragment, ratifying_fragment_is_valid):
    """
    TRUE when this boundary names a ratifying fragment and that fragment is no longer valid.
    
    Formula: =AND({{HasRatifyingFragment}}, NOT({{RatifyingFragmentIsValid}}))
    """
    return _erb.erb_and(_erb.erb_bool3(has_ratifying_fragment), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(ratifying_fragment_is_valid))))

# Level 3

def calc_authority_boundaries_is_unwarranted_and_untested(is_unwarranted, is_untested):
    """
    A boundary whose ratification has lapsed and which no agent decision has ever been evaluated against — we cannot show it works and we cannot show why it exists.
    
    Formula: =AND({{IsUnwarranted}}, {{IsUntested}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_unwarranted), _erb.erb_bool3(is_untested))

def calc_authority_boundaries_unwarranted_boundary_step_key(is_unwarranted, step):
    """
    Composite-key echo: the step this boundary governs when the boundary is unwarranted, blank otherwise.
    
    Formula: =IF({{IsUnwarranted}}, {{Step}}, "")
    """
    return (step if _erb.erb_bool3(is_unwarranted) else '')

def calc_authority_boundaries_binds_despite_lapsed_ratification(is_currently_binding, ratification_lapsed):
    """
    TRUE when a boundary is still enforced against agents while the knowledge that authorized it has lapsed.
    
    Formula: =AND({{IsCurrentlyBinding}}, {{RatificationLapsed}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_currently_binding), _erb.erb_bool3(ratification_lapsed))

# Level 4

def calc_authority_boundaries_is_ungrounded_and_untested(binds_despite_lapsed_ratification, is_untested):
    """
    TRUE when a boundary has lapsed ratification AND has never once been exercised -- so neither its authority nor its operation has ever been demonstrated.
    
    Formula: =AND({{BindsDespiteLapsedRatification}}, {{IsUntested}})
    """
    return _erb.erb_and(_erb.erb_bool3(binds_despite_lapsed_ratification), _erb.erb_bool3(is_untested))

def calc_authority_boundaries_constrained_role_assignment_key(binds_despite_lapsed_ratification, authority_role):
    """
    The role id this boundary constrains, emitted only when the boundary is ungrounded.
    
    Formula: =IF({{BindsDespiteLapsedRatification}}, {{AuthorityRole}}, "")
    """
    return (authority_role if _erb.erb_bool3(binds_despite_lapsed_ratification) else '')


def compute_authority_boundaries_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AuthorityBoundaries.
    
    AuthorityBoundaries (added by witness loop 1).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_authority_boundaries_name(result.get('forbidden_agent_kind'), result.get('forbidden_decision_kind'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_currently_binding'] = calc_authority_boundaries_is_currently_binding(result.get('status'), result.get('valid_from'), result.get('as_of_instant'), result.get('valid_to'))
    except Exception as _field_exc:
        result['is_currently_binding'] = None
        result.setdefault('_erb_errors', {})['is_currently_binding'] = str(_field_exc)
    try:
        result['boundary_match_key'] = calc_authority_boundaries_boundary_match_key(result.get('step'), result.get('forbidden_agent_kind'), result.get('forbidden_decision_kind'))
    except Exception as _field_exc:
        result['boundary_match_key'] = None
        result.setdefault('_erb_errors', {})['boundary_match_key'] = str(_field_exc)
    try:
        result['has_ratifying_fragment'] = calc_authority_boundaries_has_ratifying_fragment(result.get('ratified_by_knowledge_fragment'))
    except Exception as _field_exc:
        result['has_ratifying_fragment'] = None
        result.setdefault('_erb_errors', {})['has_ratifying_fragment'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['step_when_binding'] = calc_authority_boundaries_step_when_binding(result.get('is_currently_binding'), result.get('step'))
    except Exception as _field_exc:
        result['step_when_binding'] = None
        result.setdefault('_erb_errors', {})['step_when_binding'] = str(_field_exc)
    try:
        result['is_untested'] = calc_authority_boundaries_is_untested(result.get('is_currently_binding'), result.get('violation_count'))
    except Exception as _field_exc:
        result['is_untested'] = None
        result.setdefault('_erb_errors', {})['is_untested'] = str(_field_exc)
    try:
        result['is_unwarranted'] = calc_authority_boundaries_is_unwarranted(result.get('is_currently_binding'), result.get('has_ratifying_fragment'), result.get('ratifying_fragment_is_valid'))
    except Exception as _field_exc:
        result['is_unwarranted'] = None
        result.setdefault('_erb_errors', {})['is_unwarranted'] = str(_field_exc)
    try:
        result['warrant_is_thin'] = calc_authority_boundaries_warrant_is_thin(result.get('is_currently_binding'), result.get('ratifying_fragment_is_overdue'), result.get('ratifying_fragment_is_single_witness'))
    except Exception as _field_exc:
        result['warrant_is_thin'] = None
        result.setdefault('_erb_errors', {})['warrant_is_thin'] = str(_field_exc)
    try:
        result['ratifying_fragment_key'] = calc_authority_boundaries_ratifying_fragment_key(result.get('is_currently_binding'), result.get('ratified_by_knowledge_fragment'))
    except Exception as _field_exc:
        result['ratifying_fragment_key'] = None
        result.setdefault('_erb_errors', {})['ratifying_fragment_key'] = str(_field_exc)
    try:
        result['ratification_lapsed'] = calc_authority_boundaries_ratification_lapsed(result.get('has_ratifying_fragment'), result.get('ratifying_fragment_is_valid'))
    except Exception as _field_exc:
        result['ratification_lapsed'] = None
        result.setdefault('_erb_errors', {})['ratification_lapsed'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_unwarranted_and_untested'] = calc_authority_boundaries_is_unwarranted_and_untested(result.get('is_unwarranted'), result.get('is_untested'))
    except Exception as _field_exc:
        result['is_unwarranted_and_untested'] = None
        result.setdefault('_erb_errors', {})['is_unwarranted_and_untested'] = str(_field_exc)
    try:
        result['unwarranted_boundary_step_key'] = calc_authority_boundaries_unwarranted_boundary_step_key(result.get('is_unwarranted'), result.get('step'))
    except Exception as _field_exc:
        result['unwarranted_boundary_step_key'] = None
        result.setdefault('_erb_errors', {})['unwarranted_boundary_step_key'] = str(_field_exc)
    try:
        result['binds_despite_lapsed_ratification'] = calc_authority_boundaries_binds_despite_lapsed_ratification(result.get('is_currently_binding'), result.get('ratification_lapsed'))
    except Exception as _field_exc:
        result['binds_despite_lapsed_ratification'] = None
        result.setdefault('_erb_errors', {})['binds_despite_lapsed_ratification'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['is_ungrounded_and_untested'] = calc_authority_boundaries_is_ungrounded_and_untested(result.get('binds_despite_lapsed_ratification'), result.get('is_untested'))
    except Exception as _field_exc:
        result['is_ungrounded_and_untested'] = None
        result.setdefault('_erb_errors', {})['is_ungrounded_and_untested'] = str(_field_exc)
    try:
        result['constrained_role_assignment_key'] = calc_authority_boundaries_constrained_role_assignment_key(result.get('binds_despite_lapsed_ratification'), result.get('authority_role'))
    except Exception as _field_exc:
        result['constrained_role_assignment_key'] = None
        result.setdefault('_erb_errors', {})['constrained_role_assignment_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'step_when_binding', 'boundary_match_key', 'unwarranted_boundary_step_key', 'ratifying_fragment_key', 'constrained_role_assignment_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# BINDINGOBSERVATIONS
# BindingObservations (added by witness loop 2).
# =============================================================================

# Level 1

def calc_binding_observations_name(step_execution, binding_observation_id):
    """
    Human-readable calculated display alias for the BindingObservations row.
    
    Formula: ={{StepExecution}} & " / " & {{BindingObservationId}}
    """
    return (str(step_execution or "") + ' / ' + str(binding_observation_id or ""))

def calc_binding_observations_age_at_run_minutes(read_at, observed_source_timestamp):
    """
    How old the source data was at the instant the step read it.
    
    Formula: =DATETIME_DIFF({{ReadAt}}, {{ObservedSourceTimestamp}}, "minutes")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(read_at, observed_source_timestamp, 'minutes'))

# Level 2

def calc_binding_observations_was_stale_at_run(is_authoritative_binding, age_at_run_minutes, sla_minutes_at_run):
    """
    TRUE when an authoritative source was already outside its SLA at the moment the step consumed it.
    
    Formula: =AND({{IsAuthoritativeBinding}}, {{AgeAtRunMinutes}} > {{SlaMinutesAtRun}})
    """
    return _erb.erb_and(_erb.erb_bool3(is_authoritative_binding), _erb.erb_bool3(_erb.erb_cmp(age_at_run_minutes, '>', sla_minutes_at_run)))

# Level 3

def calc_binding_observations_stale_at_run_step_key(was_stale_at_run, step_execution):
    """
    Echoes the step execution id when the source was stale at run time.
    
    Formula: =IF({{WasStaleAtRun}}, {{StepExecution}}, "")
    """
    return (step_execution if _erb.erb_bool3(was_stale_at_run) else '')


def compute_binding_observations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for BindingObservations.
    
    BindingObservations (added by witness loop 2).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_binding_observations_name(result.get('step_execution'), result.get('binding_observation_id'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['age_at_run_minutes'] = calc_binding_observations_age_at_run_minutes(result.get('read_at'), result.get('observed_source_timestamp'))
    except Exception as _field_exc:
        result['age_at_run_minutes'] = None
        result.setdefault('_erb_errors', {})['age_at_run_minutes'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['was_stale_at_run'] = calc_binding_observations_was_stale_at_run(result.get('is_authoritative_binding'), result.get('age_at_run_minutes'), result.get('sla_minutes_at_run'))
    except Exception as _field_exc:
        result['was_stale_at_run'] = None
        result.setdefault('_erb_errors', {})['was_stale_at_run'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['stale_at_run_step_key'] = calc_binding_observations_stale_at_run_step_key(result.get('was_stale_at_run'), result.get('step_execution'))
    except Exception as _field_exc:
        result['stale_at_run_step_key'] = None
        result.setdefault('_erb_errors', {})['stale_at_run_step_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'stale_at_run_step_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ATTESTATIONS
# Attestations (added by witness loop 2).
# =============================================================================

# Level 1

def calc_attestations_name(procedure_execution, attestation_id):
    """
    Human-readable calculated display alias for the Attestations row.
    
    Formula: ={{ProcedureExecution}} & " / " & {{AttestationId}}
    """
    return (str(procedure_execution or "") + ' / ' + str(attestation_id or ""))

def calc_attestations_fitness_verdict_has_drifted(version_was_fit_at_signing, version_is_fit_now):
    """
    TRUE when the fitness of the signed version reads differently today than it did at signature.
    
    Formula: =NOT({{VersionWasFitAtSigning}} = {{VersionIsFitNow}})
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(version_was_fit_at_signing), version_is_fit_now)))

def calc_attestations_assurance_grade_has_drifted(assurance_grade_at_signing, assurance_grade_now):
    """
    TRUE when the assurance behind this signature is described differently now than it was at signature.
    
    Formula: =NOT({{AssuranceGradeAtSigning}} = {{AssuranceGradeNow}})
    """
    return _erb.erb_not(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(assurance_grade_at_signing), assurance_grade_now)))

# Level 2

def calc_attestations_would_not_survive_restatement(fitness_verdict_has_drifted, assurance_grade_has_drifted):
    """
    TRUE when re-deriving this attestation today would not reproduce what the model said when it was signed.
    
    Formula: =OR({{FitnessVerdictHasDrifted}}, {{AssuranceGradeHasDrifted}})
    """
    return _erb.erb_or(_erb.erb_bool3(fitness_verdict_has_drifted), _erb.erb_bool3(assurance_grade_has_drifted))


def compute_attestations_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Attestations.
    
    Attestations (added by witness loop 2).
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_attestations_name(result.get('procedure_execution'), result.get('attestation_id'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['fitness_verdict_has_drifted'] = calc_attestations_fitness_verdict_has_drifted(result.get('version_was_fit_at_signing'), result.get('version_is_fit_now'))
    except Exception as _field_exc:
        result['fitness_verdict_has_drifted'] = None
        result.setdefault('_erb_errors', {})['fitness_verdict_has_drifted'] = str(_field_exc)
    try:
        result['assurance_grade_has_drifted'] = calc_attestations_assurance_grade_has_drifted(result.get('assurance_grade_at_signing'), result.get('assurance_grade_now'))
    except Exception as _field_exc:
        result['assurance_grade_has_drifted'] = None
        result.setdefault('_erb_errors', {})['assurance_grade_has_drifted'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['would_not_survive_restatement'] = calc_attestations_would_not_survive_restatement(result.get('fitness_verdict_has_drifted'), result.get('assurance_grade_has_drifted'))
    except Exception as _field_exc:
        result['would_not_survive_restatement'] = None
        result.setdefault('_erb_errors', {})['would_not_survive_restatement'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPROLEPROFILES
# One row per role, carrying how that role is presented: its accent colour, its 128x128 icon, and the login-card copy. Presentation is data, so the app never hardcodes a colour or a label per role.
# =============================================================================

# Level 1

def calc_app_role_profiles_name(display_label, role_kind):
    """
    Human-readable calculated display alias for the AppRoleProfiles row.
    
    Formula: ={{DisplayLabel}} & " (" & {{RoleKind}} & ")" 
    """
    return (str(display_label or "") + ' (' + str(role_kind or "") + ')')


def compute_app_role_profiles_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AppRoleProfiles.
    
    One row per role, carrying how that role is presented: its accent colour, its 128x128 icon, and the login-card copy. Presentation is data, so the app never hardcodes a colour or a label per role.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_app_role_profiles_name(result.get('display_label'), result.get('role_kind'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPNAVGROUPS
# The left-navigation section headers. A route names the group it appears under; the nav renders the groups its active role actually uses.
# =============================================================================

# Level 1

def calc_app_nav_groups_name(group_label):
    """
    Human-readable calculated display alias for the AppNavGroups row.
    
    Formula: ={{GroupLabel}}
    """
    return group_label


def compute_app_nav_groups_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AppNavGroups.
    
    The left-navigation section headers. A route names the group it appears under; the nav renders the groups its active role actually uses.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_app_nav_groups_name(result.get('group_label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPROUTES
# One row per screen in the role-navigated application, as /{role}/{dashboard}/{entity...}. Each carries its purpose in the owning role's voice and brief layout hints, so a later build session has the brief without re-deriving it. Shared detail routes have no owning role.
# =============================================================================

# Level 1

def calc_app_routes_name(route_name, route_path):
    """
    Human-readable calculated display alias for the AppRoutes row.
    
    Formula: ={{RouteName}} & " — " & {{RoutePath}}
    """
    return (str(route_name or "") + ' — ' + str(route_path or ""))

def calc_app_routes_is_in_nav(nav_group):
    """
    Whether this route appears in the left navigation. Detail routes do not.
    
    Formula: ={{NavGroup}} <> "" 
    """
    return (not (nav_group is None or nav_group == ""))

def calc_app_routes_is_shared(owning_role, surface):
    """
    Whether this route is shared across domain roles rather than owned by one. A maintainer route is not shared — it belongs to a different surface entirely.
    
    Formula: =AND({{OwningRole}} = "", {{Surface}} = "domain")
    """
    return _erb.erb_and(_erb.erb_bool3((owning_role is None or owning_role == "")), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(surface), 'domain')))

def calc_app_routes_is_maintainer(surface):
    """
    Whether this route is model instrumentation rather than a domain workspace. Maintainer routes are reached from the login page's maintainer section, not from a role card.
    
    Formula: ={{Surface}} = "maintainer" 
    """
    return _erb.erb_eq(_erb.erb_nullif(surface), 'maintainer')

# Level 2

def calc_app_routes_answers_no_question(question_count, is_shared, is_maintainer, route_kind):
    """
    A domain route owned by a role that answers no role question. Not automatically wrong, but it should be justified. Maintainer routes are excluded: they answer questions about the model itself, which are not RoleQuestions and must not be fabricated as such.
    
    Formula: =AND({{QuestionCount}} = 0, {{IsShared}} = FALSE, {{IsMaintainer}} = FALSE, {{RouteKind}} <> "index")
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_eq(question_count, 0)), _erb.erb_bool3(_erb.erb_eq(is_shared, False)), _erb.erb_bool3(_erb.erb_eq(is_maintainer, False)), _erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(route_kind), 'index')))


def compute_app_routes_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AppRoutes.
    
    One row per screen in the role-navigated application, as /{role}/{dashboard}/{entity...}. Each carries its purpose in the owning role's voice and brief layout hints, so a later build session has the brief without re-deriving it. Shared detail routes have no owning role.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_app_routes_name(result.get('route_name'), result.get('route_path'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_in_nav'] = calc_app_routes_is_in_nav(result.get('nav_group'))
    except Exception as _field_exc:
        result['is_in_nav'] = None
        result.setdefault('_erb_errors', {})['is_in_nav'] = str(_field_exc)
    try:
        result['is_shared'] = calc_app_routes_is_shared(result.get('owning_role'), result.get('surface'))
    except Exception as _field_exc:
        result['is_shared'] = None
        result.setdefault('_erb_errors', {})['is_shared'] = str(_field_exc)
    try:
        result['is_maintainer'] = calc_app_routes_is_maintainer(result.get('surface'))
    except Exception as _field_exc:
        result['is_maintainer'] = None
        result.setdefault('_erb_errors', {})['is_maintainer'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['answers_no_question'] = calc_app_routes_answers_no_question(result.get('question_count'), result.get('is_shared'), result.get('is_maintainer'), result.get('route_kind'))
    except Exception as _field_exc:
        result['answers_no_question'] = None
        result.setdefault('_erb_errors', {})['answers_no_question'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPROUTEQUESTIONS
# Junction: which RoleQuestions each route helps answer. Deliberately many-to-many — a question is answered across several routes and a route serves several questions. It is not, and should not become, 1:1.
# =============================================================================

# Level 1

def calc_app_route_questions_name(route, question):
    """
    Human-readable calculated display alias for the AppRouteQuestions row.
    
    Formula: ={{Route}} & " answers " & {{Question}}
    """
    return (str(route or "") + ' answers ' + str(question or ""))


def compute_app_route_questions_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AppRouteQuestions.
    
    Junction: which RoleQuestions each route helps answer. Deliberately many-to-many — a question is answered across several routes and a route serves several questions. It is not, and should not become, 1:1.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_app_route_questions_name(result.get('route'), result.get('question'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPROUTEREFERENCES
# Junction: which other routes a route links to. This is the navigation graph between screens, kept as rows rather than embedded lists so the canonical model stays a DAG.
# =============================================================================

# Level 1

def calc_app_route_references_name(from_route, to_route):
    """
    Human-readable calculated display alias for the AppRouteReferences row.
    
    Formula: ={{FromRoute}} & " -> " & {{ToRoute}}
    """
    return (str(from_route or "") + ' -> ' + str(to_route or ""))


def compute_app_route_references_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AppRouteReferences.
    
    Junction: which other routes a route links to. This is the navigation graph between screens, kept as rows rather than embedded lists so the canonical model stays a DAG.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_app_route_references_name(result.get('from_route'), result.get('to_route'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# RULEBOOKTABLES
# Census of every table in this rulebook. The table-level counterpart to RulebookFields, and the anchor every access policy points at. Derived by tools/reconcile_field_catalog.py -- never hand-maintained.
# =============================================================================

# Level 1

def calc_rulebook_tables_name(table_name):
    """
    Human-readable calculated display alias.
    
    Formula: ={{TableName}}
    """
    return table_name

def calc_rulebook_tables_is_unsecured(policy_count):
    """
    True when RLS is enabled but no policy targets the table, so every principal sees zero rows. A fail-closed table nobody has granted access to.
    
    Formula: ={{PolicyCount}} = 0
    """
    return _erb.erb_eq(policy_count, 0)


def compute_rulebook_tables_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RulebookTables.
    
    Census of every table in this rulebook. The table-level counterpart to RulebookFields, and the anchor every access policy points at. Derived by tools/reconcile_field_catalog.py -- never hand-maintained.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_rulebook_tables_name(result.get('table_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_unsecured'] = calc_rulebook_tables_is_unsecured(result.get('policy_count'))
    except Exception as _field_exc:
        result['is_unsecured'] = None
        result.setdefault('_erb_errors', {})['is_unsecured'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ACCESSPRINCIPALS
# Security principals -- the identities policies attach to. A principal is the console persona a person logs in as; it maps many-to-one onto a domain Role, so 'who may see this row' is expressed once against the domain vocabulary while the UI keeps its own persona names. Each principal owns exactly one Postgres role and one Postgres schema.
# =============================================================================

# Level 1

def calc_access_principals_name(label):
    """
    Human-readable calculated display alias.
    
    Formula: ={{Label}}
    """
    return label

def calc_access_principals_has_no_access(policy_count):
    """
    True when the principal holds no policies at all, so its schema is empty and it can read nothing. Fail-closed by construction.
    
    Formula: ={{PolicyCount}} = 0
    """
    return _erb.erb_eq(policy_count, 0)

def calc_access_principals_is_over_privileged(is_administrator, visible_table_count):
    """
    True when a non-administrator principal can reach every table in the rulebook -- an admin-equivalent principal that was never declared as one.
    
    Formula: =AND(NOT({{IsAdministrator}}), {{VisibleTableCount}} >= 74)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not((is_administrator is True))), _erb.erb_bool3(_erb.erb_cmp(visible_table_count, '>=', 74)))


def compute_access_principals_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AccessPrincipals.
    
    Security principals -- the identities policies attach to. A principal is the console persona a person logs in as; it maps many-to-one onto a domain Role, so 'who may see this row' is expressed once against the domain vocabulary while the UI keeps its own persona names. Each principal owns exactly one Postgres role and one Postgres schema.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_access_principals_name(result.get('label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_no_access'] = calc_access_principals_has_no_access(result.get('policy_count'))
    except Exception as _field_exc:
        result['has_no_access'] = None
        result.setdefault('_erb_errors', {})['has_no_access'] = str(_field_exc)
    try:
        result['is_over_privileged'] = calc_access_principals_is_over_privileged(result.get('is_administrator'), result.get('visible_table_count'))
    except Exception as _field_exc:
        result['is_over_privileged'] = None
        result.setdefault('_erb_errors', {})['is_over_privileged'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ACCESSPOLICIES
# Row-level security policies: the VERTICAL cut. One row per principal x table x command, carrying the predicate that decides which rows are visible. RowPredicate is emitted verbatim into a Postgres USING clause, so it may call any SECURITY DEFINER calc_* function and therefore reference inference fields many hops down the DAG.
# =============================================================================

# Level 1

def calc_access_policies_name(principal, command, target_table):
    """
    Human-readable calculated display alias.
    
    Formula: ={{Principal}} & " " & {{Command}} & " " & {{TargetTable}}
    """
    return (str(principal or "") + ' ' + str(command or "") + ' ' + str(target_table or ""))

def calc_access_policies_is_write_command(command):
    """
    True when this policy governs a mutating command.
    
    Formula: =OR({{Command}} = "INSERT", {{Command}} = "UPDATE", {{Command}} = "DELETE", {{Command}} = "ALL")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(command), 'INSERT')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(command), 'UPDATE')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(command), 'DELETE')), _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(command), 'ALL')))

def calc_access_policies_is_unrestricted(row_predicate):
    """
    True when the policy carries no predicate, exposing every row of the target table to the principal.
    
    Formula: ={{RowPredicate}} = "" 
    """
    return (row_predicate is None or row_predicate == "")

# Level 2

def calc_access_policies_is_unrestricted_non_admin_grant(is_unrestricted, principal_is_admin):
    """
    True when a non-administrator principal is granted an unrestricted policy -- a whole-table exposure that no row predicate narrows. The single highest-signal privilege-escalation witness in the model.
    
    Formula: =AND({{IsUnrestricted}}, NOT({{PrincipalIsAdmin}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_unrestricted), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(principal_is_admin))))

def calc_access_policies_is_unwitnessed_write(is_write_command, denial_test_count):
    """
    True when a write policy has no denial test proving it refuses out-of-scope rows. An untested write grant is an assertion, not evidence.
    
    Formula: =AND({{IsWriteCommand}}, {{DenialTestCount}} = 0)
    """
    return _erb.erb_and(_erb.erb_bool3(is_write_command), _erb.erb_bool3(_erb.erb_eq(denial_test_count, 0)))


def compute_access_policies_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AccessPolicies.
    
    Row-level security policies: the VERTICAL cut. One row per principal x table x command, carrying the predicate that decides which rows are visible. RowPredicate is emitted verbatim into a Postgres USING clause, so it may call any SECURITY DEFINER calc_* function and therefore reference inference fields many hops down the DAG.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_access_policies_name(result.get('principal'), result.get('command'), result.get('target_table'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_write_command'] = calc_access_policies_is_write_command(result.get('command'))
    except Exception as _field_exc:
        result['is_write_command'] = None
        result.setdefault('_erb_errors', {})['is_write_command'] = str(_field_exc)
    try:
        result['is_unrestricted'] = calc_access_policies_is_unrestricted(result.get('row_predicate'))
    except Exception as _field_exc:
        result['is_unrestricted'] = None
        result.setdefault('_erb_errors', {})['is_unrestricted'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_unrestricted_non_admin_grant'] = calc_access_policies_is_unrestricted_non_admin_grant(result.get('is_unrestricted'), result.get('principal_is_admin'))
    except Exception as _field_exc:
        result['is_unrestricted_non_admin_grant'] = None
        result.setdefault('_erb_errors', {})['is_unrestricted_non_admin_grant'] = str(_field_exc)
    try:
        result['is_unwitnessed_write'] = calc_access_policies_is_unwitnessed_write(result.get('is_write_command'), result.get('denial_test_count'))
    except Exception as _field_exc:
        result['is_unwitnessed_write'] = None
        result.setdefault('_erb_errors', {})['is_unwitnessed_write'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# FIELDGRANTS
# Field-level grants: the HORIZONTAL cut. One row per principal x field. A field with no grant row is not filtered from the principal's view -- it is absent from it, so the column does not exist as far as that principal's SQL is concerned.
# =============================================================================

# Level 1

def calc_field_grants_name(principal, target_field):
    """
    Human-readable calculated display alias.
    
    Formula: ={{Principal}} & " -> " & {{TargetField}}
    """
    return (str(principal or "") + ' -> ' + str(target_field or ""))

def calc_field_grants_is_writable_derived_field(can_write, field_is_derived):
    """
    True when a derived field has been granted write access. Derived fields are computed by the substrate and cannot be written -- such a grant is incoherent and must be corrected.
    
    Formula: =AND({{CanWrite}}, {{FieldIsDerived}})
    """
    return _erb.erb_and((can_write is True), _erb.erb_bool3(field_is_derived))

def calc_field_grants_is_masked(mask_strategy):
    """
    True when the value is transformed rather than shown verbatim.
    
    Formula: =AND({{MaskStrategy}} <> "plain", {{MaskStrategy}} <> "")
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_ne(_erb.erb_nullif(mask_strategy), 'plain')), _erb.erb_bool3((not (mask_strategy is None or mask_strategy == ""))))

def calc_field_grants_grant_key_when_readable(can_read, principal, field_table):
    """
    Composite echo of principal and table, blank unless readable. Enables single-criterion COUNTIFS rollups of readable columns per principal per table, per the documented multi-criteria COUNTIFS defect.
    
    Formula: =IF({{CanRead}}, {{Principal}} & "|" & {{FieldTable}}, "")
    """
    return ((str(principal or "") + '|' + str(field_table or "")) if (can_read is True) else '')


def compute_field_grants_fields(record: dict) -> dict:
    """
    Compute all calculated fields for FieldGrants.
    
    Field-level grants: the HORIZONTAL cut. One row per principal x field. A field with no grant row is not filtered from the principal's view -- it is absent from it, so the column does not exist as far as that principal's SQL is concerned.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_field_grants_name(result.get('principal'), result.get('target_field'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_writable_derived_field'] = calc_field_grants_is_writable_derived_field(result.get('can_write'), result.get('field_is_derived'))
    except Exception as _field_exc:
        result['is_writable_derived_field'] = None
        result.setdefault('_erb_errors', {})['is_writable_derived_field'] = str(_field_exc)
    try:
        result['is_masked'] = calc_field_grants_is_masked(result.get('mask_strategy'))
    except Exception as _field_exc:
        result['is_masked'] = None
        result.setdefault('_erb_errors', {})['is_masked'] = str(_field_exc)
    try:
        result['grant_key_when_readable'] = calc_field_grants_grant_key_when_readable(result.get('can_read'), result.get('principal'), result.get('field_table'))
    except Exception as _field_exc:
        result['grant_key_when_readable'] = None
        result.setdefault('_erb_errors', {})['grant_key_when_readable'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'grant_key_when_readable']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLESCHEMAS
# One Postgres schema per principal -- the principal's entire visible world. The schema is the only entry on that principal's search_path, so a table absent from it cannot be named at all.
# =============================================================================

# Level 1

def calc_role_schemas_name(schema_name):
    """
    Human-readable calculated display alias.
    
    Formula: ={{SchemaName}}
    """
    return schema_name

def calc_role_schemas_search_path(schema_name):
    """
    search_path set for this principal's sessions. The principal's own schema only -- public is deliberately excluded so base tables cannot be named.
    
    Formula: ={{SchemaName}}
    """
    return schema_name

def calc_role_schemas_is_empty_schema(view_count):
    """
    True when the schema exposes no views, so the principal can read nothing at all.
    
    Formula: ={{ViewCount}} = 0
    """
    return _erb.erb_eq(view_count, 0)


def compute_role_schemas_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleSchemas.
    
    One Postgres schema per principal -- the principal's entire visible world. The schema is the only entry on that principal's search_path, so a table absent from it cannot be named at all.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_role_schemas_name(result.get('schema_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['search_path'] = calc_role_schemas_search_path(result.get('schema_name'))
    except Exception as _field_exc:
        result['search_path'] = None
        result.setdefault('_erb_errors', {})['search_path'] = str(_field_exc)
    try:
        result['is_empty_schema'] = calc_role_schemas_is_empty_schema(result.get('view_count'))
    except Exception as _field_exc:
        result['is_empty_schema'] = None
        result.setdefault('_erb_errors', {})['is_empty_schema'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'search_path']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ROLESCHEMAVIEWS
# The emitted views: one per principal x table. ColumnList is DERIVED from FieldGrants, so toggling a single grant changes the emitted DDL with no second edit anywhere. This is what makes an admin's save reshape the database without touching UI code.
# =============================================================================

# Level 1

def calc_role_schema_views_name(schema_name, view_name):
    """
    Human-readable calculated display alias.
    
    Formula: ={{SchemaName}} & "." & {{ViewName}}
    """
    return (str(schema_name or "") + '.' + str(view_name or ""))

def calc_role_schema_views_grant_key(principal, target_table):
    """
    Composite key matching FieldGrants.GrantKeyWhenReadable, used to roll up this view's readable column count.
    
    Formula: ={{Principal}} & "|" & {{TargetTable}}
    """
    return (str(principal or "") + '|' + str(target_table or ""))

def calc_role_schema_views_is_full_width(column_count, table_field_count):
    """
    True when every field on the table is exposed, so the horizontal cut removes nothing.
    
    Formula: =AND({{ColumnCount}} > 0, {{ColumnCount}} >= {{TableFieldCount}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_cmp(column_count, '>', 0)), _erb.erb_bool3(_erb.erb_cmp(column_count, '>=', table_field_count)))

def calc_role_schema_views_is_degenerate_view(column_count):
    """
    True when the view exposes zero columns -- an emitted view that cannot be selected from. A generator that emits this has produced invalid DDL.
    
    Formula: ={{ColumnCount}} = 0
    """
    return _erb.erb_eq(column_count, 0)


def compute_role_schema_views_fields(record: dict) -> dict:
    """
    Compute all calculated fields for RoleSchemaViews.
    
    The emitted views: one per principal x table. ColumnList is DERIVED from FieldGrants, so toggling a single grant changes the emitted DDL with no second edit anywhere. This is what makes an admin's save reshape the database without touching UI code.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_role_schema_views_name(result.get('schema_name'), result.get('view_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['grant_key'] = calc_role_schema_views_grant_key(result.get('principal'), result.get('target_table'))
    except Exception as _field_exc:
        result['grant_key'] = None
        result.setdefault('_erb_errors', {})['grant_key'] = str(_field_exc)
    try:
        result['is_full_width'] = calc_role_schema_views_is_full_width(result.get('column_count'), result.get('table_field_count'))
    except Exception as _field_exc:
        result['is_full_width'] = None
        result.setdefault('_erb_errors', {})['is_full_width'] = str(_field_exc)
    try:
        result['is_degenerate_view'] = calc_role_schema_views_is_degenerate_view(result.get('column_count'))
    except Exception as _field_exc:
        result['is_degenerate_view'] = None
        result.setdefault('_erb_errors', {})['is_degenerate_view'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'grant_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# JWTCLAIMMAPPINGS
# Maps verified JWT claims onto the SQL accessors row predicates call. Magic-links is the notary: it asserts only that the bearer controls an email address. This table records how that verified email, and any additional claims, become values a policy can test.
# =============================================================================

# Level 1

def calc_jwt_claim_mappings_name(claim_name, sql_accessor):
    """
    Human-readable calculated display alias.
    
    Formula: ={{ClaimName}} & " -> " & {{SqlAccessor}}
    """
    return (str(claim_name or "") + ' -> ' + str(sql_accessor or ""))


def compute_jwt_claim_mappings_fields(record: dict) -> dict:
    """
    Compute all calculated fields for JwtClaimMappings.
    
    Maps verified JWT claims onto the SQL accessors row predicates call. Magic-links is the notary: it asserts only that the bearer controls an email address. This table records how that verified email, and any additional claims, become values a policy can test.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_jwt_claim_mappings_name(result.get('claim_name'), result.get('sql_accessor'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ACCESSDENIALTESTS
# Denial witnesses. A policy with no failing case seeded against it is an assertion, not evidence -- the same acceptance bar the rest of this rulebook holds. Each row names a principal, a query, and the row that MUST NOT come back, so a policy that silently stops enforcing is caught by a red test rather than by an incident.
# =============================================================================

# Level 1

def calc_access_denial_tests_name(principal, forbidden_row_id):
    """
    Human-readable calculated display alias.
    
    Formula: ={{Principal}} & " must not see " & {{ForbiddenRowId}}
    """
    return (str(principal or "") + ' must not see ' + str(forbidden_row_id or ""))

def calc_access_denial_tests_has_run(last_run_at):
    """
    True once the test has been executed at least once.
    
    Formula: ={{LastRunAt}} <> "" 
    """
    return (not (last_run_at is None or last_run_at == ""))

def calc_access_denial_tests_is_passing(observed_visible, expected_visible):
    """
    True when observed visibility matches expectation.
    
    Formula: ={{ObservedVisible}} = {{ExpectedVisible}}
    """
    return _erb.erb_eq(_erb.erb_nullif(observed_visible), _erb.erb_nullif(expected_visible))

def calc_access_denial_tests_is_leak(expected_visible, observed_visible):
    """
    True when a row that must be invisible was returned. A confirmed access-control breach.
    
    Formula: =AND(NOT({{ExpectedVisible}}), {{ObservedVisible}})
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not((expected_visible is True))), (observed_visible is True))

def calc_access_denial_tests_is_positive_control(expected_visible):
    """
    True when this test asserts a row the principal IS entitled to. A denial suite with no positive controls cannot distinguish a working policy from one that denies everything.
    
    Formula: ={{ExpectedVisible}}
    """
    return expected_visible

# Level 2

def calc_access_denial_tests_is_unproven(has_run):
    """
    True when the test has never run, so it proves nothing regardless of how it is written.
    
    Formula: =NOT({{HasRun}})
    """
    return _erb.erb_not(_erb.erb_bool3(has_run))


def compute_access_denial_tests_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AccessDenialTests.
    
    Denial witnesses. A policy with no failing case seeded against it is an assertion, not evidence -- the same acceptance bar the rest of this rulebook holds. Each row names a principal, a query, and the row that MUST NOT come back, so a policy that silently stops enforcing is caught by a red test rather than by an incident.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_access_denial_tests_name(result.get('principal'), result.get('forbidden_row_id'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_run'] = calc_access_denial_tests_has_run(result.get('last_run_at'))
    except Exception as _field_exc:
        result['has_run'] = None
        result.setdefault('_erb_errors', {})['has_run'] = str(_field_exc)
    try:
        result['is_passing'] = calc_access_denial_tests_is_passing(result.get('observed_visible'), result.get('expected_visible'))
    except Exception as _field_exc:
        result['is_passing'] = None
        result.setdefault('_erb_errors', {})['is_passing'] = str(_field_exc)
    try:
        result['is_leak'] = calc_access_denial_tests_is_leak(result.get('expected_visible'), result.get('observed_visible'))
    except Exception as _field_exc:
        result['is_leak'] = None
        result.setdefault('_erb_errors', {})['is_leak'] = str(_field_exc)
    try:
        result['is_positive_control'] = calc_access_denial_tests_is_positive_control(result.get('expected_visible'))
    except Exception as _field_exc:
        result['is_positive_control'] = None
        result.setdefault('_erb_errors', {})['is_positive_control'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_unproven'] = calc_access_denial_tests_is_unproven(result.get('has_run'))
    except Exception as _field_exc:
        result['is_unproven'] = None
        result.setdefault('_erb_errors', {})['is_unproven'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# APPUSERS
# Sign-in identities. One row per person or automation that can authenticate. EmailAddress is what a verified token asserts; everything else about the caller is resolved from here inside the database, never trusted from the token.
# =============================================================================

# Level 1

def calc_app_users_name(display_name):
    """
    Human-readable calculated display alias.
    
    Formula: ={{DisplayName}}
    """
    return display_name

def calc_app_users_has_no_principal(assignment_count):
    """
    True when the user may act as no principal at all, so a successfully verified token still grants nothing. Authentication without authorization.
    
    Formula: ={{AssignmentCount}} = 0
    """
    return _erb.erb_eq(assignment_count, 0)

def calc_app_users_holds_multiple_principals(assignment_count):
    """
    True when the user may act as more than one principal, so the principal cannot be inferred from the email alone and must be chosen explicitly at sign-in.
    
    Formula: ={{AssignmentCount}} > 1
    """
    return _erb.erb_cmp(assignment_count, '>', 1)

def calc_app_users_is_non_human_sign_in(agent_kind):
    """
    True when a non-human agent has a sign-in identity. Pipelines and AI agents authenticate too, and their tokens are scoped exactly like a person's.
    
    Formula: =OR({{AgentKind}} = "AIAgent", {{AgentKind}} = "AutomatedPipeline")
    """
    return _erb.erb_or(_erb.erb_bool3(_erb.erb_eq(agent_kind, 'AIAgent')), _erb.erb_bool3(_erb.erb_eq(agent_kind, 'AutomatedPipeline')))


def compute_app_users_fields(record: dict) -> dict:
    """
    Compute all calculated fields for AppUsers.
    
    Sign-in identities. One row per person or automation that can authenticate. EmailAddress is what a verified token asserts; everything else about the caller is resolved from here inside the database, never trusted from the token.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_app_users_name(result.get('display_name'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_no_principal'] = calc_app_users_has_no_principal(result.get('assignment_count'))
    except Exception as _field_exc:
        result['has_no_principal'] = None
        result.setdefault('_erb_errors', {})['has_no_principal'] = str(_field_exc)
    try:
        result['holds_multiple_principals'] = calc_app_users_holds_multiple_principals(result.get('assignment_count'))
    except Exception as _field_exc:
        result['holds_multiple_principals'] = None
        result.setdefault('_erb_errors', {})['holds_multiple_principals'] = str(_field_exc)
    try:
        result['is_non_human_sign_in'] = calc_app_users_is_non_human_sign_in(result.get('agent_kind'))
    except Exception as _field_exc:
        result['is_non_human_sign_in'] = None
        result.setdefault('_erb_errors', {})['is_non_human_sign_in'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PRINCIPALASSIGNMENTS
# Which principals a user may act as. The authorization half of sign-in: a verified email proves who you are, this table decides what you may become. A user with two assignments picks one at sign-in, and the choice is verified here rather than accepted from the client.
# =============================================================================

# Level 1

def calc_principal_assignments_name(app_user, principal):
    """
    Human-readable calculated display alias.
    
    Formula: ={{AppUser}} & " as " & {{Principal}}
    """
    return (str(app_user or "") + ' as ' + str(principal or ""))

def calc_principal_assignments_is_cross_organization_grant(user_organization, principal_organization):
    """
    True when a user is allowed to act as a principal in a different organization. Legitimate for shared-service roles, but it crosses the tenancy boundary and should be deliberate rather than accidental.
    
    Formula: =AND({{UserOrganization}} <> "", {{PrincipalOrganization}} <> "", {{UserOrganization}} <> {{PrincipalOrganization}})
    """
    return _erb.erb_and(_erb.erb_bool3((not (user_organization is None or user_organization == ""))), _erb.erb_bool3((not (principal_organization is None or principal_organization == ""))), _erb.erb_bool3(_erb.erb_ne(user_organization, principal_organization)))


def compute_principal_assignments_fields(record: dict) -> dict:
    """
    Compute all calculated fields for PrincipalAssignments.
    
    Which principals a user may act as. The authorization half of sign-in: a verified email proves who you are, this table decides what you may become. A user with two assignments picks one at sign-in, and the choice is verified here rather than accepted from the client.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_principal_assignments_name(result.get('app_user'), result.get('principal'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_cross_organization_grant'] = calc_principal_assignments_is_cross_organization_grant(result.get('user_organization'), result.get('principal_organization'))
    except Exception as _field_exc:
        result['is_cross_organization_grant'] = None
        result.setdefault('_erb_errors', {})['is_cross_organization_grant'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# ISSUEDTOKENS
# Audit trail of every token minted. A token records which user signed in, which principal they chose, and the claims that were joined from the database at mint time -- so a later question of 'what could this session see' is answerable from data rather than reconstruction.
# =============================================================================

# Level 1

def calc_issued_tokens_name(app_user, principal, issued_at):
    """
    Human-readable calculated display alias.
    
    Formula: ={{AppUser}} & " as " & {{Principal}} & " @ " & {{IssuedAt}}
    """
    return (str(app_user or "") + ' as ' + str(principal or "") + ' @ ' + _erb.erb_timestamptz_text(issued_at))

def calc_issued_tokens_is_dev_minted(issuer):
    """
    True when issued by the local dev minter rather than a real magic-links tenant. Dev tokens are genuine RS256 tokens with a genuine keypair; they simply skip the email round-trip.
    
    Formula: ={{Issuer}} = "dev-mint" 
    """
    return _erb.erb_eq(_erb.erb_nullif(issuer), 'dev-mint')


def compute_issued_tokens_fields(record: dict) -> dict:
    """
    Compute all calculated fields for IssuedTokens.
    
    Audit trail of every token minted. A token records which user signed in, which principal they chose, and the claims that were joined from the database at mint time -- so a later question of 'what could this session see' is answerable from data rather than reconstruction.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_issued_tokens_name(result.get('app_user'), result.get('principal'), result.get('issued_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_dev_minted'] = calc_issued_tokens_is_dev_minted(result.get('issuer'))
    except Exception as _field_exc:
        result['is_dev_minted'] = None
        result.setdefault('_erb_errors', {})['is_dev_minted'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# PROCESSMININGRUNS
# One conformance-checking run of a mined event log against a documented procedure version — a third kind of evidence, distinct from an elicitation session (someone's account) or a knowledge fragment (a claim): what the system of record actually did, discovered by process mining rather than told to us.
# =============================================================================

# Level 1

def calc_process_mining_runs_name(event_log_source, mined_at):
    """
    Human-readable calculated display alias for the ProcessMiningRuns row.
    
    Formula: ={{EventLogSource}} & " / " & {{MinedAt}}
    """
    return (str(event_log_source or "") + ' / ' + _erb.erb_timestamptz_text(mined_at))

def calc_process_mining_runs_conformance_rate(discovered_variant_count, conforming_variant_count):
    """
    Share of discovered variants that conform to the documented procedure.
    
    Formula: =IF({{DiscoveredVariantCount}} = 0, 0, {{ConformingVariantCount}} / {{DiscoveredVariantCount}})
    """
    return (0 if _erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(discovered_variant_count), 0)) else _erb.erb_div(conforming_variant_count, discovered_variant_count))

def calc_process_mining_runs_days_since_mined(as_of_instant, mined_at):
    """
    Days elapsed since this event log was mined.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{MinedAt}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, mined_at, 'days'))

# Level 2

def calc_process_mining_runs_is_conformant(conformance_rate):
    """
    TRUE when at least 80% of what actually happened matches what was documented.
    
    Formula: ={{ConformanceRate}} >= 0.8
    """
    return _erb.erb_cmp(conformance_rate, '>=', 0.8)

def calc_process_mining_runs_has_major_drift_from_documentation(conformance_rate):
    """
    TRUE when less than half of what actually happened matches what was documented.
    
    Formula: ={{ConformanceRate}} < 0.5
    """
    return _erb.erb_cmp(conformance_rate, '<', 0.5)

def calc_process_mining_runs_is_stale_mining_evidence(days_since_mined):
    """
    TRUE when this mining evidence is more than 180 days old.
    
    Formula: ={{DaysSinceMined}} > 180
    """
    return _erb.erb_cmp(days_since_mined, '>', 180)

# Level 3

def calc_process_mining_runs_is_drift_on_live_version(has_major_drift_from_documentation, procedure_version_is_live):
    """
    TRUE when a major, real, mined deviation exists against a procedure version people are actually executing right now.
    
    Formula: =AND({{HasMajorDriftFromDocumentation}}, {{ProcedureVersionIsLive}})
    """
    return _erb.erb_and(_erb.erb_bool3(has_major_drift_from_documentation), _erb.erb_bool3(procedure_version_is_live))

# Level 4

def calc_process_mining_runs_drifted_mining_run_key(is_drift_on_live_version, procedure_version):
    """
    Composite-key echo: this run's procedure version when the run drifted on a live version, else blank.
    
    Formula: =IF({{IsDriftOnLiveVersion}}, {{ProcedureVersion}}, "")
    """
    return (procedure_version if _erb.erb_bool3(is_drift_on_live_version) else '')


def compute_process_mining_runs_fields(record: dict) -> dict:
    """
    Compute all calculated fields for ProcessMiningRuns.
    
    One conformance-checking run of a mined event log against a documented procedure version — a third kind of evidence, distinct from an elicitation session (someone's account) or a knowledge fragment (a claim): what the system of record actually did, discovered by process mining rather than told to us.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_process_mining_runs_name(result.get('event_log_source'), result.get('mined_at'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['conformance_rate'] = calc_process_mining_runs_conformance_rate(result.get('discovered_variant_count'), result.get('conforming_variant_count'))
    except Exception as _field_exc:
        result['conformance_rate'] = None
        result.setdefault('_erb_errors', {})['conformance_rate'] = str(_field_exc)
    try:
        result['days_since_mined'] = calc_process_mining_runs_days_since_mined(result.get('as_of_instant'), result.get('mined_at'))
    except Exception as _field_exc:
        result['days_since_mined'] = None
        result.setdefault('_erb_errors', {})['days_since_mined'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_conformant'] = calc_process_mining_runs_is_conformant(result.get('conformance_rate'))
    except Exception as _field_exc:
        result['is_conformant'] = None
        result.setdefault('_erb_errors', {})['is_conformant'] = str(_field_exc)
    try:
        result['has_major_drift_from_documentation'] = calc_process_mining_runs_has_major_drift_from_documentation(result.get('conformance_rate'))
    except Exception as _field_exc:
        result['has_major_drift_from_documentation'] = None
        result.setdefault('_erb_errors', {})['has_major_drift_from_documentation'] = str(_field_exc)
    try:
        result['is_stale_mining_evidence'] = calc_process_mining_runs_is_stale_mining_evidence(result.get('days_since_mined'))
    except Exception as _field_exc:
        result['is_stale_mining_evidence'] = None
        result.setdefault('_erb_errors', {})['is_stale_mining_evidence'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_drift_on_live_version'] = calc_process_mining_runs_is_drift_on_live_version(result.get('has_major_drift_from_documentation'), result.get('procedure_version_is_live'))
    except Exception as _field_exc:
        result['is_drift_on_live_version'] = None
        result.setdefault('_erb_errors', {})['is_drift_on_live_version'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['drifted_mining_run_key'] = calc_process_mining_runs_drifted_mining_run_key(result.get('is_drift_on_live_version'), result.get('procedure_version'))
    except Exception as _field_exc:
        result['drifted_mining_run_key'] = None
        result.setdefault('_erb_errors', {})['drifted_mining_run_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'drifted_mining_run_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# VOCABULARIES
# A controlled vocabulary / SKOS-style concept scheme — one facet of standardized terminology (e.g. a family of control categories) that Requirements and other rows can point at instead of restating the concept in free text each time.
# =============================================================================

# Level 1

def calc_vocabularies_name(title):
    """
    Human-readable calculated display alias for the Vocabularies row.
    
    Formula: ={{Title}}
    """
    return title

def calc_vocabularies_has_orphan_terms(orphan_term_count):
    """
    TRUE when this vocabulary has at least one defined-but-unused term.
    
    Formula: ={{OrphanTermCount}} > 0
    """
    return _erb.erb_cmp(orphan_term_count, '>', 0)


def compute_vocabularies_fields(record: dict) -> dict:
    """
    Compute all calculated fields for Vocabularies.
    
    A controlled vocabulary / SKOS-style concept scheme — one facet of standardized terminology (e.g. a family of control categories) that Requirements and other rows can point at instead of restating the concept in free text each time.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_vocabularies_name(result.get('title'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['has_orphan_terms'] = calc_vocabularies_has_orphan_terms(result.get('orphan_term_count'))
    except Exception as _field_exc:
        result['has_orphan_terms'] = None
        result.setdefault('_erb_errors', {})['has_orphan_terms'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# VOCABULARYTERMS
# One controlled, defined term (a SKOS Concept) within a Vocabulary. Requirements point at a VocabularyTerm instead of free-texting the same concept in a new Statement every time.
# =============================================================================

# Level 1

def calc_vocabulary_terms_name(pref_label):
    """
    Human-readable calculated display alias for the VocabularyTerms row.
    
    Formula: ={{PrefLabel}}
    """
    return pref_label

def calc_vocabulary_terms_is_orphan_term(usage_count):
    """
    TRUE when this term has been defined but nothing in the model actually uses it yet.
    
    Formula: ={{UsageCount}} = 0
    """
    return _erb.erb_eq(usage_count, 0)

def calc_vocabulary_terms_is_widely_adopted_term(usage_count):
    """
    TRUE when more than one Requirement shares this exact controlled term.
    
    Formula: ={{UsageCount}} >= 2
    """
    return _erb.erb_cmp(usage_count, '>=', 2)

# Level 2

def calc_vocabulary_terms_orphan_term_vocabulary_key(is_orphan_term, vocabulary):
    """
    Composite-key echo: this term's vocabulary when the term is an orphan, else blank.
    
    Formula: =IF({{IsOrphanTerm}}, {{Vocabulary}}, "")
    """
    return (vocabulary if _erb.erb_bool3(is_orphan_term) else '')


def compute_vocabulary_terms_fields(record: dict) -> dict:
    """
    Compute all calculated fields for VocabularyTerms.
    
    One controlled, defined term (a SKOS Concept) within a Vocabulary. Requirements point at a VocabularyTerm instead of free-texting the same concept in a new Statement every time.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_vocabulary_terms_name(result.get('pref_label'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['is_orphan_term'] = calc_vocabulary_terms_is_orphan_term(result.get('usage_count'))
    except Exception as _field_exc:
        result['is_orphan_term'] = None
        result.setdefault('_erb_errors', {})['is_orphan_term'] = str(_field_exc)
    try:
        result['is_widely_adopted_term'] = calc_vocabulary_terms_is_widely_adopted_term(result.get('usage_count'))
    except Exception as _field_exc:
        result['is_widely_adopted_term'] = None
        result.setdefault('_erb_errors', {})['is_widely_adopted_term'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['orphan_term_vocabulary_key'] = calc_vocabulary_terms_orphan_term_vocabulary_key(result.get('is_orphan_term'), result.get('vocabulary'))
    except Exception as _field_exc:
        result['orphan_term_vocabulary_key'] = None
        result.setdefault('_erb_errors', {})['orphan_term_vocabulary_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'orphan_term_vocabulary_key']:
        if result.get(key) == '':
            result[key] = None

    return result

# =============================================================================
# KNOWLEDGEBROKERLINKS
# An informal expertise-network edge: one agent naming another as who they actually go to for a topic, independent of any formal Role or RoleAssignment. RoleAssignments and CommunitiesOfPractice record who is SUPPOSED to know something; this records who people ACTUALLY rely on.
# =============================================================================

# Level 1

def calc_knowledge_broker_links_name(seeker, broker):
    """
    Human-readable calculated display alias for the KnowledgeBrokerLinks row.
    
    Formula: ={{Seeker}} & " -> " & {{Broker}}
    """
    return (str(seeker or "") + ' -> ' + str(broker or ""))

def calc_knowledge_broker_links_days_since_consulted(as_of_instant, last_consulted_at):
    """
    Days elapsed since the seeker last consulted the broker.
    
    Formula: =DATETIME_DIFF({{AsOfInstant}}, {{LastConsultedAt}}, "days")
    """
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, last_consulted_at, 'days'))

# Level 2

def calc_knowledge_broker_links_is_active_reliance(frequency, days_since_consulted):
    """
    TRUE when this is a live, ongoing informal dependency rather than a one-off or stale contact.
    
    Formula: =AND(NOT({{Frequency}} = "Rarely"), {{DaysSinceConsulted}} <= 180)
    """
    return _erb.erb_and(_erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(_erb.erb_eq(_erb.erb_nullif(frequency), 'Rarely')))), _erb.erb_bool3(_erb.erb_cmp(days_since_consulted, '<=', 180)))

# Level 3

def calc_knowledge_broker_links_is_at_risk_reliance(is_active_reliance, broker_is_still_engaged):
    """
    TRUE when someone is actively relying on a broker who has already left every role they held.
    
    Formula: =AND({{IsActiveReliance}}, NOT({{BrokerIsStillEngaged}}))
    """
    return _erb.erb_and(_erb.erb_bool3(is_active_reliance), _erb.erb_bool3(_erb.erb_not(_erb.erb_bool3(broker_is_still_engaged))))

def calc_knowledge_broker_links_active_reliance_broker_key(is_active_reliance, broker):
    """
    Composite-key echo: the broker this link names when the reliance is active, else blank.
    
    Formula: =IF({{IsActiveReliance}}, {{Broker}}, "")
    """
    return (broker if _erb.erb_bool3(is_active_reliance) else '')

# Level 4

def calc_knowledge_broker_links_at_risk_broker_key(is_at_risk_reliance, broker):
    """
    Composite-key echo: the broker this link names when the reliance is at risk, else blank.
    
    Formula: =IF({{IsAtRiskReliance}}, {{Broker}}, "")
    """
    return (broker if _erb.erb_bool3(is_at_risk_reliance) else '')


def compute_knowledge_broker_links_fields(record: dict) -> dict:
    """
    Compute all calculated fields for KnowledgeBrokerLinks.
    
    An informal expertise-network edge: one agent naming another as who they actually go to for a topic, independent of any formal Role or RoleAssignment. RoleAssignments and CommunitiesOfPractice record who is SUPPOSED to know something; this records who people ACTUALLY rely on.
    """
    result = dict(record)

    # Level 1 calculations
    try:
        result['name'] = calc_knowledge_broker_links_name(result.get('seeker'), result.get('broker'))
    except Exception as _field_exc:
        result['name'] = None
        result.setdefault('_erb_errors', {})['name'] = str(_field_exc)
    try:
        result['days_since_consulted'] = calc_knowledge_broker_links_days_since_consulted(result.get('as_of_instant'), result.get('last_consulted_at'))
    except Exception as _field_exc:
        result['days_since_consulted'] = None
        result.setdefault('_erb_errors', {})['days_since_consulted'] = str(_field_exc)

    # Level 2 calculations
    try:
        result['is_active_reliance'] = calc_knowledge_broker_links_is_active_reliance(result.get('frequency'), result.get('days_since_consulted'))
    except Exception as _field_exc:
        result['is_active_reliance'] = None
        result.setdefault('_erb_errors', {})['is_active_reliance'] = str(_field_exc)

    # Level 3 calculations
    try:
        result['is_at_risk_reliance'] = calc_knowledge_broker_links_is_at_risk_reliance(result.get('is_active_reliance'), result.get('broker_is_still_engaged'))
    except Exception as _field_exc:
        result['is_at_risk_reliance'] = None
        result.setdefault('_erb_errors', {})['is_at_risk_reliance'] = str(_field_exc)
    try:
        result['active_reliance_broker_key'] = calc_knowledge_broker_links_active_reliance_broker_key(result.get('is_active_reliance'), result.get('broker'))
    except Exception as _field_exc:
        result['active_reliance_broker_key'] = None
        result.setdefault('_erb_errors', {})['active_reliance_broker_key'] = str(_field_exc)

    # Level 4 calculations
    try:
        result['at_risk_broker_key'] = calc_knowledge_broker_links_at_risk_broker_key(result.get('is_at_risk_reliance'), result.get('broker'))
    except Exception as _field_exc:
        result['at_risk_broker_key'] = None
        result.setdefault('_erb_errors', {})['at_risk_broker_key'] = str(_field_exc)

    # Convert empty strings to None for string fields
    for key in ['name', 'active_reliance_broker_key', 'at_risk_broker_key']:
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
    'RulebookReleases': compute_rulebook_releases_fields,
    'rulebook_releases': compute_rulebook_releases_fields,
    'OntologyProfiles': compute_ontology_profiles_fields,
    'ontology_profiles': compute_ontology_profiles_fields,
    'EvaluationContexts': compute_evaluation_contexts_fields,
    'evaluation_contexts': compute_evaluation_contexts_fields,
    'Organizations': compute_organizations_fields,
    'organizations': compute_organizations_fields,
    'Agents': compute_agents_fields,
    'agents': compute_agents_fields,
    'Roles': compute_roles_fields,
    'roles': compute_roles_fields,
    'RoleAssignments': compute_role_assignments_fields,
    'role_assignments': compute_role_assignments_fields,
    'CommunitiesOfPractice': compute_communities_of_practice_fields,
    'communities_of_practice': compute_communities_of_practice_fields,
    'Mentorships': compute_mentorships_fields,
    'mentorships': compute_mentorships_fields,
    'ProcedureTypes': compute_procedure_types_fields,
    'procedure_types': compute_procedure_types_fields,
    'Procedures': compute_procedures_fields,
    'procedures': compute_procedures_fields,
    'ProcedureVersions': compute_procedure_versions_fields,
    'procedure_versions': compute_procedure_versions_fields,
    'ProcedureVersionLinks': compute_procedure_version_links_fields,
    'procedure_version_links': compute_procedure_version_links_fields,
    'ProcedureStatusChanges': compute_procedure_status_changes_fields,
    'procedure_status_changes': compute_procedure_status_changes_fields,
    'Steps': compute_steps_fields,
    'steps': compute_steps_fields,
    'StepTransitions': compute_step_transitions_fields,
    'step_transitions': compute_step_transitions_fields,
    'Actions': compute_actions_fields,
    'actions': compute_actions_fields,
    'Functions': compute_functions_fields,
    'functions': compute_functions_fields,
    'Tools': compute_tools_fields,
    'tools': compute_tools_fields,
    'StepActions': compute_step_actions_fields,
    'step_actions': compute_step_actions_fields,
    'StepFunctions': compute_step_functions_fields,
    'step_functions': compute_step_functions_fields,
    'StepTools': compute_step_tools_fields,
    'step_tools': compute_step_tools_fields,
    'Requirements': compute_requirements_fields,
    'requirements': compute_requirements_fields,
    'StepRequirements': compute_step_requirements_fields,
    'step_requirements': compute_step_requirements_fields,
    'StepVerifications': compute_step_verifications_fields,
    'step_verifications': compute_step_verifications_fields,
    'Rationales': compute_rationales_fields,
    'rationales': compute_rationales_fields,
    'Exceptions': compute_exceptions_fields,
    'exceptions': compute_exceptions_fields,
    'Resources': compute_resources_fields,
    'resources': compute_resources_fields,
    'ProcedureResources': compute_procedure_resources_fields,
    'procedure_resources': compute_procedure_resources_fields,
    'ElicitationSessions': compute_elicitation_sessions_fields,
    'elicitation_sessions': compute_elicitation_sessions_fields,
    'KnowledgeFragments': compute_knowledge_fragments_fields,
    'knowledge_fragments': compute_knowledge_fragments_fields,
    'KnowledgeGaps': compute_knowledge_gaps_fields,
    'knowledge_gaps': compute_knowledge_gaps_fields,
    'FAQs': compute_fa_qs_fields,
    'fa_qs': compute_fa_qs_fields,
    'Explanations': compute_explanations_fields,
    'explanations': compute_explanations_fields,
    'ProcedureExecutions': compute_procedure_executions_fields,
    'procedure_executions': compute_procedure_executions_fields,
    'StepExecutions': compute_step_executions_fields,
    'step_executions': compute_step_executions_fields,
    'RequirementSatisfactions': compute_requirement_satisfactions_fields,
    'requirement_satisfactions': compute_requirement_satisfactions_fields,
    'Errors': compute_errors_fields,
    'errors': compute_errors_fields,
    'IssueOccurrences': compute_issue_occurrences_fields,
    'issue_occurrences': compute_issue_occurrences_fields,
    'UserQuestions': compute_user_questions_fields,
    'user_questions': compute_user_questions_fields,
    'UserFeedback': compute_user_feedback_fields,
    'user_feedback': compute_user_feedback_fields,
    'StewardshipAssignments': compute_stewardship_assignments_fields,
    'stewardship_assignments': compute_stewardship_assignments_fields,
    'ChangeRequests': compute_change_requests_fields,
    'change_requests': compute_change_requests_fields,
    'ReviewEvents': compute_review_events_fields,
    'review_events': compute_review_events_fields,
    'LearningActivities': compute_learning_activities_fields,
    'learning_activities': compute_learning_activities_fields,
    'OperationalBindings': compute_operational_bindings_fields,
    'operational_bindings': compute_operational_bindings_fields,
    'CommunicationPolicies': compute_communication_policies_fields,
    'communication_policies': compute_communication_policies_fields,
    'MessageTemplates': compute_message_templates_fields,
    'message_templates': compute_message_templates_fields,
    'SemanticMappings': compute_semantic_mappings_fields,
    'semantic_mappings': compute_semantic_mappings_fields,
    'WitnessLoops': compute_witness_loops_fields,
    'witness_loops': compute_witness_loops_fields,
    'RoleQuestions': compute_role_questions_fields,
    'role_questions': compute_role_questions_fields,
    'RulebookFields': compute_rulebook_fields_fields,
    'rulebook_fields': compute_rulebook_fields_fields,
    'TestSuites': compute_test_suites_fields,
    'test_suites': compute_test_suites_fields,
    'TestCases': compute_test_cases_fields,
    'test_cases': compute_test_cases_fields,
    'ERBVersions': compute_erb_versions_fields,
    'erb_versions': compute_erb_versions_fields,
    'ERBCustomizations': compute_erb_customizations_fields,
    'erb_customizations': compute_erb_customizations_fields,
    '__meta__': compute___meta___fields,
    'ExceptionInvocations': compute_exception_invocations_fields,
    'exception_invocations': compute_exception_invocations_fields,
    'VerificationOutcomes': compute_verification_outcomes_fields,
    'verification_outcomes': compute_verification_outcomes_fields,
    'ObservedTransitions': compute_observed_transitions_fields,
    'observed_transitions': compute_observed_transitions_fields,
    'Recipients': compute_recipients_fields,
    'recipients': compute_recipients_fields,
    'MessageDeliveries': compute_message_deliveries_fields,
    'message_deliveries': compute_message_deliveries_fields,
    'TemplateApprovals': compute_template_approvals_fields,
    'template_approvals': compute_template_approvals_fields,
    'SendIntents': compute_send_intents_fields,
    'send_intents': compute_send_intents_fields,
    'AgentDecisionRecords': compute_agent_decision_records_fields,
    'agent_decision_records': compute_agent_decision_records_fields,
    'DeliveredCommunications': compute_delivered_communications_fields,
    'delivered_communications': compute_delivered_communications_fields,
    'AuthorityBoundaries': compute_authority_boundaries_fields,
    'authority_boundaries': compute_authority_boundaries_fields,
    'BindingObservations': compute_binding_observations_fields,
    'binding_observations': compute_binding_observations_fields,
    'Attestations': compute_attestations_fields,
    'attestations': compute_attestations_fields,
    'AppRoleProfiles': compute_app_role_profiles_fields,
    'app_role_profiles': compute_app_role_profiles_fields,
    'AppNavGroups': compute_app_nav_groups_fields,
    'app_nav_groups': compute_app_nav_groups_fields,
    'AppRoutes': compute_app_routes_fields,
    'app_routes': compute_app_routes_fields,
    'AppRouteQuestions': compute_app_route_questions_fields,
    'app_route_questions': compute_app_route_questions_fields,
    'AppRouteReferences': compute_app_route_references_fields,
    'app_route_references': compute_app_route_references_fields,
    'RulebookTables': compute_rulebook_tables_fields,
    'rulebook_tables': compute_rulebook_tables_fields,
    'AccessPrincipals': compute_access_principals_fields,
    'access_principals': compute_access_principals_fields,
    'AccessPolicies': compute_access_policies_fields,
    'access_policies': compute_access_policies_fields,
    'FieldGrants': compute_field_grants_fields,
    'field_grants': compute_field_grants_fields,
    'RoleSchemas': compute_role_schemas_fields,
    'role_schemas': compute_role_schemas_fields,
    'RoleSchemaViews': compute_role_schema_views_fields,
    'role_schema_views': compute_role_schema_views_fields,
    'JwtClaimMappings': compute_jwt_claim_mappings_fields,
    'jwt_claim_mappings': compute_jwt_claim_mappings_fields,
    'AccessDenialTests': compute_access_denial_tests_fields,
    'access_denial_tests': compute_access_denial_tests_fields,
    'AppUsers': compute_app_users_fields,
    'app_users': compute_app_users_fields,
    'PrincipalAssignments': compute_principal_assignments_fields,
    'principal_assignments': compute_principal_assignments_fields,
    'IssuedTokens': compute_issued_tokens_fields,
    'issued_tokens': compute_issued_tokens_fields,
    'ProcessMiningRuns': compute_process_mining_runs_fields,
    'process_mining_runs': compute_process_mining_runs_fields,
    'Vocabularies': compute_vocabularies_fields,
    'vocabularies': compute_vocabularies_fields,
    'VocabularyTerms': compute_vocabulary_terms_fields,
    'vocabulary_terms': compute_vocabulary_terms_fields,
    'KnowledgeBrokerLinks': compute_knowledge_broker_links_fields,
    'knowledge_broker_links': compute_knowledge_broker_links_fields,
}


# =============================================================================
# AGGREGATE SCALARS — formulas wrapped around aggregate calls
# =============================================================================


def _erb_composite_procedure_versions_days_since_last_review(r):
    as_of_instant = r.get('as_of_instant')
    erb_aggregate0 = r.get('erb_aggregate0')
    return _erb.erb_integer(_erb.erb_datetime_diff(as_of_instant, erb_aggregate0, 'days'))


# calculated_field_count bounds the runner's passes over the dataset.
CALCULATED_FIELD_COUNT = 705

# ERB_TABLES is every table, in rulebook order.
ERB_TABLES = [
    {'name': 'RulebookReleases', 'file': 'rulebook_releases', 'rulebook_rows': 1,
     'compute': compute_rulebook_releases_fields,
     'fields': ['rulebook_release_id', 'name', 'rulebook_version', 'profile_version', 'profile_schema_path', 'pko_core_version_iri', 'pko_industry_version_iri', 'issued_at', 'status', 'changelog', 'is_current'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'OntologyProfiles', 'file': 'ontology_profiles', 'rulebook_rows': 11,
     'compute': compute_ontology_profiles_fields,
     'fields': ['ontology_profile_id', 'name', 'label', 'version', 'version_iri', 'namespace_iri', 'license', 'scope'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'EvaluationContexts', 'file': 'evaluation_contexts', 'rulebook_rows': 1,
     'compute': compute_evaluation_contexts_fields,
     'fields': ['evaluation_context_id', 'name', 'label', 'as_of_instant', 'is_current', 'rationale', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Organizations', 'file': 'organizations', 'rulebook_rows': 4,
     'compute': compute_organizations_fields,
     'fields': ['organization_id', 'name', 'display_name', 'organization_type', 'external_identifier', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Agents', 'file': 'agents', 'rulebook_rows': 11,
     'compute': compute_agents_fields,
     'fields': ['agent_id', 'name', 'display_name', 'agent_kind', 'organization', 'contact_address', 'version_or_employment_key', 'count_of_current_role_assignments', 'is_still_engaged', 'decision_count', 'overridden_decision_count', 'override_rate_percent', 'is_non_human', 'boundary_violation_count', 'is_operating_outside_boundary', 'draft_decision_count', 'overridden_draft_count', 'draft_rewrite_rate_percent', 'times_named_as_broker', 'is_recognized_broker', 'at_risk_reliance_count', 'has_at_risk_knowledge_reliance', 'semantic_type_iri'],
     'calculated': {'is_non_human', 'is_recognized_broker', 'has_at_risk_knowledge_reliance', 'draft_rewrite_rate_percent', 'is_still_engaged', 'override_rate_percent', 'name', 'is_operating_outside_boundary'},
     'lookups': [],
     'aggregations': [
        {'field': 'count_of_current_role_assignments', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('current_agent_key', 'field', 'agent_id')]},
        {'field': 'decision_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('deciding_agent', 'field', 'agent_id')]},
        {'field': 'overridden_decision_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('deciding_agent_when_overridden', 'field', 'agent_id')]},
        {'field': 'boundary_violation_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('agent_when_boundary_violated', 'field', 'agent_id')]},
        {'field': 'draft_decision_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('agent_when_draft', 'field', 'agent_id')]},
        {'field': 'overridden_draft_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('agent_when_draft_overridden', 'field', 'agent_id')]},
        {'field': 'times_named_as_broker', 'op': 'COUNTIFS', 'table': 'knowledge_broker_links', 'criteria': [('active_reliance_broker_key', 'field', 'agent_id')]},
        {'field': 'at_risk_reliance_count', 'op': 'COUNTIFS', 'table': 'knowledge_broker_links', 'criteria': [('at_risk_broker_key', 'field', 'agent_id')]},]},
    {'name': 'Roles', 'file': 'roles', 'rulebook_rows': 12,
     'compute': compute_roles_fields,
     'fields': ['role_id', 'name', 'label', 'organization', 'current_agent', 'current_agent_kind', 'responsibility', 'active_assignment_count', 'currently_covered_assignment_count', 'has_no_current_holder', 'count_of_awaited_decisions', 'current_assignment', 'current_assignment_valid_from', 'is_non_human_held', 'is_ungoverned_non_human_role', 'departed_assignment_count', 'has_lost_a_holder', 'is_vacated_role', 'ungrounded_boundary_count', 'is_governed_by_lapsed_authority', 'unescalated_refusal_count', 'unauthorized_enforcement_assignment_count', 'is_ungoverned_enforcement_role', 'semantic_type_iri'],
     'calculated': {'is_ungoverned_non_human_role', 'is_non_human_held', 'has_lost_a_holder', 'is_vacated_role', 'name', 'is_governed_by_lapsed_authority', 'has_no_current_holder', 'is_ungoverned_enforcement_role'},
     'lookups': [
        {'field': 'current_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'current_agent', 'match': 'agent_id'},
        {'field': 'current_assignment_valid_from', 'target': 'role_assignments', 'return': 'valid_from', 'key': 'current_assignment', 'match': 'role_assignment_id'},],
     'aggregations': [
        {'field': 'active_assignment_count', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('role', 'field', 'role_id')]},
        {'field': 'currently_covered_assignment_count', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('role_when_covering', 'field', 'role_id')]},
        {'field': 'count_of_awaited_decisions', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('authority_role', 'field', 'role_id')]},
        {'field': 'departed_assignment_count', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('departed_role_key', 'field', 'role_id')]},
        {'field': 'ungrounded_boundary_count', 'op': 'COUNTIFS', 'table': 'authority_boundaries', 'criteria': [('constrained_role_assignment_key', 'field', 'role_id')]},
        {'field': 'unescalated_refusal_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('unescalated_refusal_role_key', 'field', 'role_id')]},
        {'field': 'unauthorized_enforcement_assignment_count', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('unauthorized_enforcement_role_key', 'field', 'role_id')]},]},
    {'name': 'RoleAssignments', 'file': 'role_assignments', 'rulebook_rows': 13,
     'compute': compute_role_assignments_fields,
     'fields': ['role_assignment_id', 'name', 'role', 'agent', 'valid_from', 'valid_to', 'reason', 'status', 'evaluation_context', 'as_of_instant', 'is_current', 'current_agent_key', 'is_currently_valid', 'agent_role_key', 'has_departed', 'covers_now', 'role_when_covering', 'agent_kind', 'is_non_human_assignment', 'supersedes_assignment', 'predecessor_agent_kind', 'is_human_to_non_human_handover', 'approving_authority_role', 'authorizing_change_request', 'is_unauthorized_non_human_assignment', 'was_authorized_by_change_request', 'decision_count', 'overridden_decision_count', 'override_rate_percent', 'predecessor_override_rate_percent', 'quality_regressed_vs_predecessor', 'departed_role_key', 'minimum_decisions_for_comparison', 'predecessor_decision_count', 'has_sufficient_sample', 'predecessor_has_sufficient_sample', 'comparison_is_evidentially_sound', 'single_override_swing_percent', 'quality_verdict_is_unsupported', 'is_unmeasured_automation_handover', 'error_correction_count', 'error_rate_percent', 'authorization_decided_at', 'authorization_reviewed_at', 'authorization_review_cadence_days', 'has_dated_authorization', 'days_since_authorization_review', 'authorization_is_overdue_for_review', 'is_standing_unreviewed_automation', 'is_unconditioned_automation_handover', 'max_tolerable_error_rate_percent', 'exceeds_tolerable_error_rate', 'boundary_violation_count_for_assignment', 'has_any_boundary_violation', 'has_ungrounded_governing_boundary', 'suspension_condition_met', 'is_operating_under_met_suspension_condition', 'has_declared_suspension_condition', 'has_approving_authority', 'has_authorizing_change_request', 'is_enforcement_role', 'is_unauthorized_enforcement_agent', 'governance_evidence_count', 'unauthorized_enforcement_role_key', 'semantic_type_iri'],
     'calculated': {'is_operating_under_met_suspension_condition', 'is_unauthorized_non_human_assignment', 'has_sufficient_sample', 'quality_verdict_is_unsupported', 'has_dated_authorization', 'is_currently_valid', 'departed_role_key', 'error_rate_percent', 'exceeds_tolerable_error_rate', 'is_standing_unreviewed_automation', 'name', 'is_unmeasured_automation_handover', 'agent_role_key', 'covers_now', 'authorization_is_overdue_for_review', 'is_non_human_assignment', 'comparison_is_evidentially_sound', 'suspension_condition_met', 'has_any_boundary_violation', 'has_authorizing_change_request', 'predecessor_has_sufficient_sample', 'has_approving_authority', 'is_unauthorized_enforcement_agent', 'was_authorized_by_change_request', 'has_departed', 'is_human_to_non_human_handover', 'is_unconditioned_automation_handover', 'has_declared_suspension_condition', 'days_since_authorization_review', 'single_override_swing_percent', 'role_when_covering', 'override_rate_percent', 'is_current', 'current_agent_key', 'governance_evidence_count', 'quality_regressed_vs_predecessor', 'unauthorized_enforcement_role_key'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'agent', 'match': 'agent_id'},
        {'field': 'predecessor_agent_kind', 'target': 'role_assignments', 'return': 'agent_kind', 'key': 'supersedes_assignment', 'match': 'role_assignment_id'},
        {'field': 'predecessor_override_rate_percent', 'target': 'role_assignments', 'return': 'override_rate_percent', 'key': 'supersedes_assignment', 'match': 'role_assignment_id'},
        {'field': 'predecessor_decision_count', 'target': 'role_assignments', 'return': 'decision_count', 'key': 'supersedes_assignment', 'match': 'role_assignment_id'},
        {'field': 'has_ungrounded_governing_boundary', 'target': 'roles', 'return': 'is_governed_by_lapsed_authority', 'key': 'role', 'match': 'role_id'},],
     'aggregations': [
        {'field': 'decision_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('role_assignment_when_scored', 'field', 'role_assignment_id')]},
        {'field': 'overridden_decision_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('role_assignment_when_overridden', 'field', 'role_assignment_id')]},
        {'field': 'error_correction_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('error_correction_role_assignment_key', 'field', 'role_assignment_id')]},
        {'field': 'boundary_violation_count_for_assignment', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('boundary_violation_role_assignment_key', 'field', 'role_assignment_id')]},]},
    {'name': 'CommunitiesOfPractice', 'file': 'communities_of_practice', 'rulebook_rows': 2,
     'compute': compute_communities_of_practice_fields,
     'fields': ['community_of_practice_id', 'name', 'label', 'organization', 'steward_role', 'purpose', 'cadence', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Mentorships', 'file': 'mentorships', 'rulebook_rows': 1,
     'compute': compute_mentorships_fields,
     'fields': ['mentorship_id', 'name', 'community_of_practice', 'mentor_agent', 'learner_agent', 'valid_from', 'valid_to', 'learning_objective', 'evidence_of_completion', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ProcedureTypes', 'file': 'procedure_types', 'rulebook_rows': 2,
     'compute': compute_procedure_types_fields,
     'fields': ['procedure_type_id', 'name', 'label', 'definition', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Procedures', 'file': 'procedures', 'rulebook_rows': 2,
     'compute': compute_procedures_fields,
     'fields': ['procedure_id', 'name', 'title', 'procedure_type', 'owner_organization', 'adopted_by_organization', 'purpose', 'target', 'is_template', 'current_version_key', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ProcedureVersions', 'file': 'procedure_versions', 'rulebook_rows': 3,
     'compute': compute_procedure_versions_fields,
     'fields': ['procedure_version_id', 'name', 'procedure', 'version_number', 'title', 'status', 'issued_at', 'modified_at', 'created_by_agent', 'modified_by_agent', 'new_version_motivation', 'changelog_description', 'is_current', 'count_of_steps', 'count_of_open_knowledge_gaps', 'is_ready_for_execution', 'specified_step_count', 'overdue_review_count', 'open_change_request_count', 'open_high_severity_gap_count', 'is_fit_to_execute', 'steward_review_cadence_days', 'count_of_stewardship_assignments', 'has_any_steward', 'is_live', 'is_unstewarded', 'is_live_and_unstewarded', 'count_of_open_blocking_gaps', 'has_open_blocking_gap', 'is_live_with_blocking_gap', 'should_not_be_executable', 'count_of_unapproved_reliance_fragments', 'runs_on_unapproved_knowledge', 'count_of_overdue_gaps', 'count_of_change_requests', 'count_of_review_events', 'has_governance_record', 'evaluation_context', 'as_of_instant', 'days_since_modified', 'days_since_last_review', 'was_modified_since_last_review', 'modifier_is_authority', 'has_unwitnessed_change', 'count_of_stale_fragments', 'knowledge_is_staler_than_cadence', 'compound_fragile_fragment_count', 'rests_on_compound_fragile_knowledge', 'concentrated_witness_session_count', 'knowledge_base_is_concentrated', 'machine_consumed_unapproved_count', 'feeds_unapproved_knowledge_to_machines', 'genuinely_overdue_fragment_count', 'awaited_decision_count', 'scoped_open_blocking_gap_count', 'is_blocked_on_pending_decision', 'unexercised_human_gate_count', 'ai_boundary_is_unevidenced', 'load_bearing_unapproved_count', 'unlanded_decision_count', 'unrehearsed_control_entry_count', 'has_unrehearsed_control_entry', 'is_live_with_unrehearsed_control', 'cadence_breach_count', 'is_in_cadence_breach', 'has_decision_in_flight', 'is_unremediated_cadence_breach', 'is_managed_cadence_breach', 'governance_is_silent', 'valid_fragment_count', 'still_owns_valid_knowledge', 'incoming_supersession_count', 'is_still_referenced', 'is_load_bearing_orphan', 'is_cleanly_retired', 'stalled_implementation_count', 'is_held_unfit_by_landed_decisions', 'undeclared_control_kind_count', 'control_taxonomy_is_incomplete', 'has_approved_change_request', 'approved_change_request_count', 'unwatched_unowned_control_count', 'mining_run_count', 'drifted_mining_run_count', 'has_unresolved_mining_drift', 'semantic_type_iri'],
     'calculated': {'has_unresolved_mining_drift', 'has_unwitnessed_change', 'is_cleanly_retired', 'is_managed_cadence_breach', 'control_taxonomy_is_incomplete', 'name', 'is_fit_to_execute', 'has_governance_record', 'feeds_unapproved_knowledge_to_machines', 'governance_is_silent', 'is_held_unfit_by_landed_decisions', 'is_ready_for_execution', 'has_approved_change_request', 'should_not_be_executable', 'knowledge_is_staler_than_cadence', 'knowledge_base_is_concentrated', 'is_live_and_unstewarded', 'is_live', 'has_decision_in_flight', 'is_live_with_blocking_gap', 'is_load_bearing_orphan', 'is_still_referenced', 'is_unremediated_cadence_breach', 'runs_on_unapproved_knowledge', 'is_live_with_unrehearsed_control', 'days_since_modified', 'has_unrehearsed_control_entry', 'has_any_steward', 'is_blocked_on_pending_decision', 'has_open_blocking_gap', 'is_in_cadence_breach', 'rests_on_compound_fragile_knowledge', 'ai_boundary_is_unevidenced', 'is_unstewarded', 'was_modified_since_last_review', 'still_owns_valid_knowledge'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'modifier_is_authority', 'target': 'agents', 'return': 'agent_kind', 'key': 'modified_by_agent', 'match': 'agent_id'},],
     'aggregations': [
        {'field': 'count_of_steps', 'op': 'COUNTIFS', 'table': 'steps', 'criteria': [('procedure_version', 'field', 'procedure_version_id')]},
        {'field': 'count_of_open_knowledge_gaps', 'op': 'COUNTIFS', 'table': 'knowledge_gaps', 'criteria': [('procedure_version', 'field', 'procedure_version_id'), ('status', 'literal', 'Open')]},
        {'field': 'specified_step_count', 'op': 'COUNTIFS', 'table': 'steps', 'criteria': [('procedure_version', 'field', 'procedure_version_id')]},
        {'field': 'overdue_review_count', 'op': 'COUNTIFS', 'table': 'review_events', 'criteria': [('overdue_version_key', 'field', 'procedure_version_id')]},
        {'field': 'open_change_request_count', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('open_change_version_key', 'field', 'procedure_version_id')]},
        {'field': 'open_high_severity_gap_count', 'op': 'COUNTIFS', 'table': 'knowledge_gaps', 'criteria': [('open_gap_version_key', 'field', 'procedure_version_id')]},
        {'field': 'steward_review_cadence_days', 'op': 'SUM', 'table': 'stewardship_assignments', 'target': 'review_cadence_days', 'criteria': [('procedure_version', 'field', 'procedure_version_id')], 'suffix': None},
        {'field': 'count_of_stewardship_assignments', 'op': 'COUNTIFS', 'table': 'stewardship_assignments', 'criteria': [('procedure_version', 'field', 'procedure_version_id')]},
        {'field': 'count_of_open_blocking_gaps', 'op': 'COUNTIFS', 'table': 'knowledge_gaps', 'criteria': [('is_open_and_blocking', 'literal', True)]},
        {'field': 'count_of_unapproved_reliance_fragments', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('is_unapproved_but_relied_on', 'literal', True)]},
        {'field': 'count_of_overdue_gaps', 'op': 'COUNTIFS', 'table': 'knowledge_gaps', 'criteria': [('is_overdue_gap', 'literal', True)]},
        {'field': 'count_of_change_requests', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('procedure_version', 'field', 'procedure_version_id')]},
        {'field': 'count_of_review_events', 'op': 'COUNTIFS', 'table': 'review_events', 'criteria': [('procedure_version', 'field', 'procedure_version_id')]},
        {'field': 'days_since_last_review', 'op': 'COMPOSITE', 'parts': [{'field': 'erb_aggregate0', 'op': 'MAX', 'table': 'review_events', 'target': 'reviewed_at', 'criteria': [('procedure_version', 'field', 'procedure_version_id')], 'suffix': None}], 'scalar': _erb_composite_procedure_versions_days_since_last_review},
        {'field': 'count_of_stale_fragments', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('exceeds_owning_cadence', 'literal', True)]},
        {'field': 'compound_fragile_fragment_count', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('compound_fragile_version_key', 'field', 'procedure_version_id')]},
        {'field': 'concentrated_witness_session_count', 'op': 'COUNTIFS', 'table': 'elicitation_sessions', 'criteria': [('concentrated_session_version_key', 'field', 'procedure_version_id')]},
        {'field': 'machine_consumed_unapproved_count', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('machine_consumed_unapproved_version_key', 'field', 'procedure_version_id')]},
        {'field': 'genuinely_overdue_fragment_count', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('genuinely_overdue_version_key', 'field', 'procedure_version_id')]},
        {'field': 'awaited_decision_count', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('backlog_version_key', 'field', 'procedure_version_id')]},
        {'field': 'scoped_open_blocking_gap_count', 'op': 'COUNTIFS', 'table': 'knowledge_gaps', 'criteria': [('open_blocking_gap_version_key', 'field', 'procedure_version_id')]},
        {'field': 'unexercised_human_gate_count', 'op': 'COUNTIFS', 'table': 'steps', 'criteria': [('unexercised_gate_version_key', 'field', 'procedure_version_id')]},
        {'field': 'load_bearing_unapproved_count', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('unapproved_load_bearing_version_key', 'field', 'procedure_version_id')]},
        {'field': 'unlanded_decision_count', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('unlanded_version_key', 'field', 'procedure_version_id')]},
        {'field': 'unrehearsed_control_entry_count', 'op': 'COUNTIFS', 'table': 'step_transitions', 'criteria': [('unrehearsed_control_version_key', 'field', 'procedure_version_id')]},
        {'field': 'cadence_breach_count', 'op': 'COUNTIFS', 'table': 'review_events', 'criteria': [('cadence_breach_version_key', 'field', 'procedure_version_id')]},
        {'field': 'valid_fragment_count', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('valid_fragment_version_key', 'field', 'procedure_version_id')]},
        {'field': 'incoming_supersession_count', 'op': 'COUNTIFS', 'table': 'procedure_version_links', 'criteria': [('superseded_version_key', 'field', 'procedure_version_id')]},
        {'field': 'stalled_implementation_count', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('stalled_implementation_version_key', 'field', 'procedure_version_id')]},
        {'field': 'undeclared_control_kind_count', 'op': 'COUNTIFS', 'table': 'steps', 'criteria': [('undeclared_control_version_key', 'field', 'procedure_version_id')]},
        {'field': 'approved_change_request_count', 'op': 'COUNTIFS', 'table': 'change_requests', 'criteria': [('approved_version_key', 'field', 'procedure_version_id')]},
        {'field': 'unwatched_unowned_control_count', 'op': 'COUNTIFS', 'table': 'requirements', 'criteria': [('unwatched_unowned_flag', 'literal', 'unwatched-unowned')]},
        {'field': 'mining_run_count', 'op': 'COUNTIFS', 'table': 'process_mining_runs', 'criteria': [('procedure_version', 'field', 'procedure_version_id')]},
        {'field': 'drifted_mining_run_count', 'op': 'COUNTIFS', 'table': 'process_mining_runs', 'criteria': [('drifted_mining_run_key', 'field', 'procedure_version_id')]},]},
    {'name': 'ProcedureVersionLinks', 'file': 'procedure_version_links', 'rulebook_rows': 1,
     'compute': compute_procedure_version_links_fields,
     'fields': ['procedure_version_link_id', 'name', 'previous_procedure_version', 'next_procedure_version', 'relation_iri', 'change_summary', 'superseded_version_key'],
     'calculated': {'superseded_version_key', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ProcedureStatusChanges', 'file': 'procedure_status_changes', 'rulebook_rows': 5,
     'compute': compute_procedure_status_changes_fields,
     'fields': ['procedure_status_change_id', 'name', 'procedure_version', 'from_status', 'to_status', 'changed_at', 'changed_by_agent', 'motivation', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Steps', 'file': 'steps', 'rulebook_rows': 17,
     'compute': compute_steps_fields,
     'fields': ['step_id', 'name', 'procedure_version', 'step_number', 'title', 'step_kind', 'assigned_role', 'assigned_role_label', 'assigned_agent_kind', 'instruction', 'expected_duration_minutes', 'expertise_level', 'requires_human_confirmation', 'blocking_requirement_count', 'stale_binding_count', 'authoritative_stale_count', 'available_exception_count', 'declared_verification_count', 'is_preparation_step', 'is_approval_step', 'stale_authoritative_binding_count', 'inputs_are_fresh', 'is_software_assigned', 'is_human_approval_gate', 'gate_held_by_human', 'binding_boundary_count', 'assigned_role_is_ungoverned', 'unusable_binding_count', 'all_sources_usable', 'control_kind', 'unwarranted_boundary_count', 'is_governed_by_unwarranted_boundary', 'software_execution_count', 'has_been_approached_by_software', 'is_unexercised_human_gate', 'is_demonstrated_human_gate', 'unexercised_gate_version_key', 'has_declared_control_kind', 'undeclared_control_version_key', 'approval_step_is_software_assigned', 'unwitnessed_blocking_count', 'semantic_type_iri'],
     'calculated': {'is_demonstrated_human_gate', 'is_human_approval_gate', 'is_unexercised_human_gate', 'inputs_are_fresh', 'has_declared_control_kind', 'all_sources_usable', 'undeclared_control_version_key', 'unexercised_gate_version_key', 'is_approval_step', 'is_preparation_step', 'name', 'is_governed_by_unwarranted_boundary', 'is_software_assigned', 'gate_held_by_human', 'approval_step_is_software_assigned', 'has_been_approached_by_software'},
     'lookups': [
        {'field': 'assigned_role_label', 'target': 'roles', 'return': 'label', 'key': 'assigned_role', 'match': 'role_id'},
        {'field': 'assigned_agent_kind', 'target': 'roles', 'return': 'current_agent_kind', 'key': 'assigned_role', 'match': 'role_id'},
        {'field': 'assigned_role_is_ungoverned', 'target': 'roles', 'return': 'is_ungoverned_non_human_role', 'key': 'assigned_role', 'match': 'role_id'},],
     'aggregations': [
        {'field': 'blocking_requirement_count', 'op': 'COUNTIFS', 'table': 'step_requirements', 'criteria': [('blocking_step_key', 'field', 'step_id')]},
        {'field': 'stale_binding_count', 'op': 'COUNTIFS', 'table': 'operational_bindings', 'criteria': [('stale_binding_step_key', 'field', 'step_id')]},
        {'field': 'authoritative_stale_count', 'op': 'COUNTIFS', 'table': 'operational_bindings', 'criteria': [('authoritative_stale_step_key', 'field', 'step_id')]},
        {'field': 'available_exception_count', 'op': 'COUNTIFS', 'table': 'exceptions', 'criteria': [('active_exception_step_key', 'field', 'step_id')]},
        {'field': 'declared_verification_count', 'op': 'COUNTIFS', 'table': 'step_verifications', 'criteria': [('step', 'field', 'step_id')]},
        {'field': 'stale_authoritative_binding_count', 'op': 'COUNTIFS', 'table': 'operational_bindings', 'criteria': [('step_when_stale', 'field', 'step_id')]},
        {'field': 'binding_boundary_count', 'op': 'COUNTIFS', 'table': 'authority_boundaries', 'criteria': [('step_when_binding', 'field', 'step_id')]},
        {'field': 'unusable_binding_count', 'op': 'COUNTIFS', 'table': 'operational_bindings', 'criteria': [('step_when_unusable', 'field', 'step_id')]},
        {'field': 'unwarranted_boundary_count', 'op': 'COUNTIFS', 'table': 'authority_boundaries', 'criteria': [('unwarranted_boundary_step_key', 'field', 'step_id')]},
        {'field': 'software_execution_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('software_execution_step_key', 'field', 'step_id')]},
        {'field': 'unwitnessed_blocking_count', 'op': 'COUNTIFS', 'table': 'step_requirements', 'criteria': [('unwitnessed_step_key', 'field', 'step_id')]},]},
    {'name': 'StepTransitions', 'file': 'step_transitions', 'rulebook_rows': 19,
     'compute': compute_step_transitions_fields,
     'fields': ['step_transition_id', 'name', 'procedure_version', 'from_step', 'to_step', 'transition_kind', 'condition', 'priority', 'is_recovery_path', 'count_of_from_step_executions', 'count_of_to_step_executions', 'has_reachable_origin', 'has_reachable_target', 'is_never_exercised', 'is_untested_recovery_path', 'count_of_observed_traversals', 'has_been_traversed', 'is_unwalked_recovery_path', 'target_blocking_requirement_count', 'target_carries_blocking_control', 'is_unrehearsed_control_entry', 'unrehearsed_control_version_key', 'semantic_type_iri'],
     'calculated': {'unrehearsed_control_version_key', 'is_untested_recovery_path', 'is_recovery_path', 'has_reachable_origin', 'has_reachable_target', 'name', 'is_unrehearsed_control_entry', 'is_unwalked_recovery_path', 'target_carries_blocking_control', 'is_never_exercised', 'has_been_traversed'},
     'lookups': [
        {'field': 'target_blocking_requirement_count', 'target': 'steps', 'return': 'blocking_requirement_count', 'key': 'to_step', 'match': 'step_id'},],
     'aggregations': [
        {'field': 'count_of_from_step_executions', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('step', 'field', 'from_step')]},
        {'field': 'count_of_to_step_executions', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('step', 'field', 'to_step')]},
        {'field': 'count_of_observed_traversals', 'op': 'COUNTIFS', 'table': 'observed_transitions', 'criteria': [('step_transition', 'field', 'step_transition_id')]},]},
    {'name': 'Actions', 'file': 'actions', 'rulebook_rows': 9,
     'compute': compute_actions_fields,
     'fields': ['action_id', 'name', 'label', 'definition', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Functions', 'file': 'functions', 'rulebook_rows': 8,
     'compute': compute_functions_fields,
     'fields': ['function_id', 'name', 'label', 'definition', 'implementation_key', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Tools', 'file': 'tools', 'rulebook_rows': 8,
     'compute': compute_tools_fields,
     'fields': ['tool_id', 'name', 'label', 'purpose', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'StepActions', 'file': 'step_actions', 'rulebook_rows': 9,
     'compute': compute_step_actions_fields,
     'fields': ['step_action_id', 'name', 'step', 'action'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'StepFunctions', 'file': 'step_functions', 'rulebook_rows': 8,
     'compute': compute_step_functions_fields,
     'fields': ['step_function_id', 'name', 'step', 'function'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'StepTools', 'file': 'step_tools', 'rulebook_rows': 10,
     'compute': compute_step_tools_fields,
     'fields': ['step_tool_id', 'name', 'step', 'tool'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Requirements', 'file': 'requirements', 'rulebook_rows': 13,
     'compute': compute_requirements_fields,
     'fields': ['requirement_id', 'name', 'label', 'requirement_type', 'statement', 'rationale', 'is_blocking', 'satisfaction_record_count', 'step_binding_count', 'is_bound_to_any_step', 'has_ever_been_evaluated', 'negative_outcome_count', 'is_inoperative_control', 'is_decorative_control', 'has_computed_witness', 'witness_field_name', 'has_ever_produced_negative', 'is_unfalsified_control', 'claims_a_witness_field', 'named_witness_field_exists', 'derived_has_computed_witness', 'witness_claim_is_unverified', 'is_unwitnessed_blocking_control', 'witness_fire_count', 'witness_has_never_fired', 'evaluation_sample_size', 'has_meaningful_sample', 'minimum_sample_for_assurance', 'is_untested_witness', 'is_evidenced_holding_control', 'control_assurance_state', 'unexercised_binding_count', 'witness_is_partially_scoped', 'accountable_role', 'accountable_agent', 'has_named_owner', 'is_orphaned_blocking_control', 'is_unwatched_and_unowned', 'attestation_exposure_note', 'unwatched_unowned_flag', 'controlled_term', 'uses_controlled_vocabulary', 'semantic_type_iri'],
     'calculated': {'unwatched_unowned_flag', 'has_meaningful_sample', 'is_untested_witness', 'witness_fire_count', 'name', 'is_decorative_control', 'derived_has_computed_witness', 'is_unfalsified_control', 'attestation_exposure_note', 'witness_claim_is_unverified', 'is_bound_to_any_step', 'control_assurance_state', 'is_unwitnessed_blocking_control', 'has_ever_produced_negative', 'is_orphaned_blocking_control', 'claims_a_witness_field', 'is_unwatched_and_unowned', 'witness_is_partially_scoped', 'has_named_owner', 'evaluation_sample_size', 'is_inoperative_control', 'uses_controlled_vocabulary', 'has_ever_been_evaluated', 'witness_has_never_fired', 'is_evidenced_holding_control'},
     'lookups': [
        {'field': 'named_witness_field_exists', 'target': 'rulebook_fields', 'return': 'is_derived', 'key': 'witness_field_name', 'match': 'rulebook_field_id'},
        {'field': 'accountable_agent', 'target': 'roles', 'return': 'current_agent', 'key': 'accountable_role', 'match': 'role_id'},],
     'aggregations': [
        {'field': 'satisfaction_record_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('requirement', 'field', 'requirement_id')]},
        {'field': 'step_binding_count', 'op': 'COUNTIFS', 'table': 'step_requirements', 'criteria': [('requirement', 'field', 'requirement_id')]},
        {'field': 'negative_outcome_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('negative_outcome_requirement_key', 'field', 'requirement_id')]},
        {'field': 'unexercised_binding_count', 'op': 'COUNTIFS', 'table': 'step_requirements', 'criteria': [('unexercised_binding_requirement_key', 'field', 'requirement_id')]},]},
    {'name': 'StepRequirements', 'file': 'step_requirements', 'rulebook_rows': 15,
     'compute': compute_step_requirements_fields,
     'fields': ['step_requirement_id', 'name', 'step', 'requirement', 'requirement_is_blocking', 'blocking_step_key', 'step_when_blocking', 'requirement_lacks_witness', 'unwitnessed_step_key', 'satisfaction_count_for_binding', 'binding_was_ever_exercised', 'is_unexercised_blocking_binding', 'unexercised_binding_requirement_key'],
     'calculated': {'unexercised_binding_requirement_key', 'binding_was_ever_exercised', 'is_unexercised_blocking_binding', 'step_when_blocking', 'name', 'blocking_step_key', 'unwitnessed_step_key'},
     'lookups': [
        {'field': 'requirement_is_blocking', 'target': 'requirements', 'return': 'is_blocking', 'key': 'requirement', 'match': 'requirement_id'},
        {'field': 'requirement_lacks_witness', 'target': 'requirements', 'return': 'is_unwitnessed_blocking_control', 'key': 'requirement', 'match': 'requirement_id'},],
     'aggregations': [
        {'field': 'satisfaction_count_for_binding', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('binding_key', 'field', 'step_requirement_id')]},]},
    {'name': 'StepVerifications', 'file': 'step_verifications', 'rulebook_rows': 11,
     'compute': compute_step_verifications_fields,
     'fields': ['step_verification_id', 'name', 'step', 'verification_kind', 'signal_identifier', 'expected_signal_value', 'instruction', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Rationales', 'file': 'rationales', 'rulebook_rows': 4,
     'compute': compute_rationales_fields,
     'fields': ['rationale_id', 'name', 'procedure_version', 'step', 'title', 'statement', 'status', 'authority_role', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Exceptions', 'file': 'exceptions', 'rulebook_rows': 4,
     'compute': compute_exceptions_fields,
     'fields': ['exception_id', 'name', 'procedure_version', 'trigger_step', 'condition', 'handling', 'approval_role', 'fallback_role', 'status', 'active_exception_step_key', 'semantic_type_iri'],
     'calculated': {'active_exception_step_key', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Resources', 'file': 'resources', 'rulebook_rows': 8,
     'compute': compute_resources_fields,
     'fields': ['resource_id', 'name', 'title', 'resource_kind', 'external_uri', 'created_at', 'modified_at', 'description', 'approval_status', 'is_approved_source', 'semantic_type_iri'],
     'calculated': {'is_approved_source', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ProcedureResources', 'file': 'procedure_resources', 'rulebook_rows': 8,
     'compute': compute_procedure_resources_fields,
     'fields': ['procedure_resource_id', 'name', 'procedure_version', 'resource', 'relation', 'relation_iri'],
     'calculated': {'relation_iri', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ElicitationSessions', 'file': 'elicitation_sessions', 'rulebook_rows': 3,
     'compute': compute_elicitation_sessions_fields,
     'fields': ['elicitation_session_id', 'name', 'procedure_version', 'method', 'started_at', 'ended_at', 'practitioner_agent', 'facilitator_agent', 'summary', 'status', 'evaluation_context', 'as_of_instant', 'days_since_elicited', 'is_single_witness_method', 'practitioner_is_still_engaged', 'valid_fragments_produced', 'is_high_yield_session', 'is_concentrated_single_witness', 'is_stale_concentrated_witness', 'concentrated_session_version_key', 'semantic_type_iri'],
     'calculated': {'days_since_elicited', 'is_single_witness_method', 'is_concentrated_single_witness', 'is_high_yield_session', 'is_stale_concentrated_witness', 'concentrated_session_version_key', 'name'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'practitioner_is_still_engaged', 'target': 'agents', 'return': 'is_still_engaged', 'key': 'practitioner_agent', 'match': 'agent_id'},],
     'aggregations': [
        {'field': 'valid_fragments_produced', 'op': 'COUNTIFS', 'table': 'knowledge_fragments', 'criteria': [('valid_fragment_session_key', 'field', 'elicitation_session_id')]},]},
    {'name': 'KnowledgeFragments', 'file': 'knowledge_fragments', 'rulebook_rows': 7,
     'compute': compute_knowledge_fragments_fields,
     'fields': ['knowledge_fragment_id', 'name', 'procedure_version', 'step', 'knowledge_form', 'statement', 'elicitation_session', 'source_agent', 'confidence', 'valid_from', 'valid_to', 'status', 'owner_role', 'evaluation_context', 'as_of_instant', 'is_currently_valid', 'source_agent_is_still_engaged', 'source_agent_kind', 'has_human_source', 'has_orphaned_provenance', 'is_undefendable_tacit_claim', 'is_approved', 'is_within_validity_window', 'is_relied_upon', 'step_procedure_version_status', 'is_attached_to_live_version', 'is_unapproved_but_relied_on', 'evidence_age_days', 'has_recorded_elicitation', 'is_from_single_witness', 'evidence_expiry_days', 'evidence_has_expired', 'owner_agent', 'is_awaiting_approval', 'owner_is_me', 'is_my_unfinished_approval', 'is_invoked_by_an_exception', 'has_operational_reliance', 'is_unapproved_and_operationally_live', 'age_days', 'is_low_confidence', 'owning_version_cadence_days', 'exceeds_owning_cadence', 'is_aging_low_confidence_claim', 'owner_role_agent_kind', 'is_human_owned', 'is_ai_validated_by_ai', 'review_cadence_days', 'is_overdue_for_review', 'predates_current_role_holder', 'owner_role_assignment_valid_from', 'last_reviewed_at', 'fragility_signal_count', 'is_compound_fragile', 'is_single_point_of_failure', 'is_expiring_single_point_of_failure', 'compound_fragile_version_key', 'valid_fragment_session_key', 'consuming_step_is_software_assigned', 'consuming_step_agent_kind', 'is_unapproved_and_machine_consumed', 'is_unapproved_and_human_consumed', 'machine_consumed_unapproved_version_key', 'has_review_record', 'days_since_actual_review', 'is_unreviewed_since_authoring', 'is_genuinely_overdue', 'review_recency_is_inferred', 'inference_disagrees_with_record', 'genuinely_overdue_version_key', 'ratified_boundary_count', 'reliance_surface_count', 'days_awaiting_my_approval', 'is_high_blast_radius_unapproved', 'is_long_unapproved', 'unapproved_load_bearing_version_key', 'owner_role_is_vacated', 'is_orphaned_by_role', 'valid_fragment_version_key', 'semantic_type_iri'],
     'calculated': {'unapproved_load_bearing_version_key', 'has_orphaned_provenance', 'compound_fragile_version_key', 'is_long_unapproved', 'days_awaiting_my_approval', 'owner_is_me', 'has_recorded_elicitation', 'is_unapproved_and_machine_consumed', 'is_currently_valid', 'reliance_surface_count', 'machine_consumed_unapproved_version_key', 'is_unreviewed_since_authoring', 'name', 'is_my_unfinished_approval', 'has_human_source', 'is_relied_upon', 'age_days', 'evidence_has_expired', 'has_review_record', 'is_unapproved_but_relied_on', 'is_approved', 'genuinely_overdue_version_key', 'predates_current_role_holder', 'is_unapproved_and_operationally_live', 'is_overdue_for_review', 'valid_fragment_version_key', 'fragility_signal_count', 'is_ai_validated_by_ai', 'is_single_point_of_failure', 'days_since_actual_review', 'is_human_owned', 'is_compound_fragile', 'is_within_validity_window', 'exceeds_owning_cadence', 'is_expiring_single_point_of_failure', 'has_operational_reliance', 'is_awaiting_approval', 'is_high_blast_radius_unapproved', 'is_low_confidence', 'inference_disagrees_with_record', 'valid_fragment_session_key', 'is_orphaned_by_role', 'is_undefendable_tacit_claim', 'is_aging_low_confidence_claim', 'is_unapproved_and_human_consumed', 'evidence_expiry_days', 'review_recency_is_inferred', 'is_genuinely_overdue'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'source_agent_is_still_engaged', 'target': 'agents', 'return': 'is_still_engaged', 'key': 'source_agent', 'match': 'agent_id'},
        {'field': 'source_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'source_agent', 'match': 'agent_id'},
        {'field': 'step_procedure_version_status', 'target': 'steps', 'return': 'procedure_version', 'key': 'step', 'match': 'step_id'},
        {'field': 'is_attached_to_live_version', 'target': 'procedure_versions', 'return': 'is_live', 'key': 'procedure_version', 'match': 'procedure_version_id'},
        {'field': 'evidence_age_days', 'target': 'elicitation_sessions', 'return': 'days_since_elicited', 'key': 'elicitation_session', 'match': 'elicitation_session_id'},
        {'field': 'is_from_single_witness', 'target': 'elicitation_sessions', 'return': 'is_single_witness_method', 'key': 'elicitation_session', 'match': 'elicitation_session_id'},
        {'field': 'owner_agent', 'target': 'roles', 'return': 'current_agent', 'key': 'owner_role', 'match': 'role_id'},
        {'field': 'owning_version_cadence_days', 'target': 'procedure_versions', 'return': 'steward_review_cadence_days', 'key': 'procedure_version', 'match': 'procedure_version_id'},
        {'field': 'owner_role_agent_kind', 'target': 'roles', 'return': 'current_agent_kind', 'key': 'owner_role', 'match': 'role_id'},
        {'field': 'review_cadence_days', 'target': 'procedure_versions', 'return': 'steward_review_cadence_days', 'key': 'procedure_version', 'match': 'procedure_version_id'},
        {'field': 'owner_role_assignment_valid_from', 'target': 'roles', 'return': 'current_assignment_valid_from', 'key': 'owner_role', 'match': 'role_id'},
        {'field': 'consuming_step_is_software_assigned', 'target': 'steps', 'return': 'is_software_assigned', 'key': 'step', 'match': 'step_id'},
        {'field': 'consuming_step_agent_kind', 'target': 'steps', 'return': 'assigned_agent_kind', 'key': 'step', 'match': 'step_id'},
        {'field': 'owner_role_is_vacated', 'target': 'roles', 'return': 'is_vacated_role', 'key': 'owner_role', 'match': 'role_id'},],
     'aggregations': [
        {'field': 'is_invoked_by_an_exception', 'op': 'COUNTIFS', 'table': 'exceptions', 'criteria': [('trigger_step', 'field', 'step')]},
        {'field': 'ratified_boundary_count', 'op': 'COUNTIFS', 'table': 'authority_boundaries', 'criteria': [('ratifying_fragment_key', 'field', 'knowledge_fragment_id')]},]},
    {'name': 'KnowledgeGaps', 'file': 'knowledge_gaps', 'rulebook_rows': 8,
     'compute': compute_knowledge_gaps_fields,
     'fields': ['knowledge_gap_id', 'name', 'procedure_version', 'step', 'statement', 'severity', 'blocking_kind', 'status', 'owner_role', 'identified_at', 'resolution_plan', 'is_open', 'open_gap_version_key', 'is_blocking', 'is_open_and_blocking', 'evaluation_context', 'as_of_instant', 'days_open', 'tolerance_days', 'is_overdue_gap', 'owner_agent', 'owner_is_still_engaged', 'has_resolution_plan', 'is_abandoned_unknown', 'open_blocking_gap_version_key', 'owner_role_is_vacated', 'is_ownerless_open_gap', 'semantic_type_iri'],
     'calculated': {'is_blocking', 'has_resolution_plan', 'open_gap_version_key', 'is_overdue_gap', 'is_open_and_blocking', 'is_abandoned_unknown', 'is_ownerless_open_gap', 'open_blocking_gap_version_key', 'name', 'days_open', 'tolerance_days', 'is_open'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'owner_agent', 'target': 'roles', 'return': 'current_agent', 'key': 'owner_role', 'match': 'role_id'},
        {'field': 'owner_is_still_engaged', 'target': 'agents', 'return': 'is_still_engaged', 'key': 'owner_agent', 'match': 'agent_id'},
        {'field': 'owner_role_is_vacated', 'target': 'roles', 'return': 'is_vacated_role', 'key': 'owner_role', 'match': 'role_id'},],
     'aggregations': []},
    {'name': 'FAQs', 'file': 'fa_qs', 'rulebook_rows': 3,
     'compute': compute_fa_qs_fields,
     'fields': ['faq_id', 'name', 'procedure_version', 'step', 'category', 'target_kind', 'question', 'answer', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Explanations', 'file': 'explanations', 'rulebook_rows': 2,
     'compute': compute_explanations_fields,
     'fields': ['explanation_id', 'name', 'procedure_version', 'step', 'title', 'description', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ProcedureExecutions', 'file': 'procedure_executions', 'rulebook_rows': 2,
     'compute': compute_procedure_executions_fields,
     'fields': ['procedure_execution_id', 'name', 'procedure_version', 'execution_status', 'started_at', 'ended_at', 'executed_by_agent', 'context', 'operational_record_uri', 'expected_step_count', 'completed_step_count', 'control_breach_count', 'late_step_count', 'is_structurally_complete', 'diverged_from_specification', 'all_blocking_controls_evaluated', 'unevaluated_blocking_total', 'separation_of_duties_held', 'separation_violation_count', 'is_attestation_ready', 'attestation_blocker_summary', 'executed_version_is_fit', 'signed_against_unfit_version', 'asserted_only_control_count', 'assurance_is_mostly_asserted', 'unreachable_handling_failure_count', 'retention_breach_count', 'cleared_legal_review_count', 'has_cleared_legal_review', 'abandoned_failure_count', 'delivered_count', 'total_delivery_attempt_count', 'has_abandoned_failures', 'mishandled_refusal_count', 'unclean_step_count', 'ran_clean', 'count_of_approval_executions', 'has_human_approval', 'count_of_delivery_executions', 'has_delivered', 'delivered_without_approval', 'invalid_approval_count', 'approval_chain_is_complete', 'vacuously_clean_step_count', 'preparation_step_count', 'approval_step_count', 'separation_was_testable', 'separation_held_under_test', 'separation_is_vacuously_green', 'separation_assurance_note', 'ungoverned_divergence_count', 'divergence_was_fully_governed', 'computedly_witnessed_control_count', 'evaluated_control_count', 'computed_assurance_ratio', 'interested_party_assertion_count', 'assurance_grade', 'attestation_would_be_weakly_based', 'independent_human_observation_count', 'has_any_independent_observation', 'self_attested_approval_count', 'assurance_chain_is_circular', 'latest_attestation_instant', 'has_been_attested', 'attestation_count', 'post_attestation_score_count', 'basis_changed_after_signature', 'requires_re_attestation', 'intended_recipient_count', 'reached_recipient_count', 'silently_dropped_count', 'delivery_yield_percent', 'campaign_silently_lost_audience', 'unrecorded_refusal_count', 'has_unrecorded_refusals', 'independently_confirmed_intent_count', 'send_decisions_are_entirely_self_witnessed', 'semantic_type_iri'],
     'calculated': {'ran_clean', 'separation_was_testable', 'has_any_independent_observation', 'has_abandoned_failures', 'all_blocking_controls_evaluated', 'computed_assurance_ratio', 'name', 'attestation_would_be_weakly_based', 'basis_changed_after_signature', 'has_human_approval', 'assurance_grade', 'has_cleared_legal_review', 'has_unrecorded_refusals', 'is_attestation_ready', 'separation_of_duties_held', 'assurance_is_mostly_asserted', 'requires_re_attestation', 'separation_is_vacuously_green', 'evaluated_control_count', 'is_structurally_complete', 'attestation_blocker_summary', 'divergence_was_fully_governed', 'separation_assurance_note', 'signed_against_unfit_version', 'assurance_chain_is_circular', 'delivered_without_approval', 'send_decisions_are_entirely_self_witnessed', 'separation_held_under_test', 'delivery_yield_percent', 'campaign_silently_lost_audience', 'has_delivered', 'diverged_from_specification', 'has_been_attested', 'approval_chain_is_complete'},
     'lookups': [
        {'field': 'expected_step_count', 'target': 'procedure_versions', 'return': 'specified_step_count', 'key': 'procedure_version', 'match': 'procedure_version_id'},
        {'field': 'executed_version_is_fit', 'target': 'procedure_versions', 'return': 'is_fit_to_execute', 'key': 'procedure_version', 'match': 'procedure_version_id'},],
     'aggregations': [
        {'field': 'completed_step_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('completed_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'control_breach_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('control_breach_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'late_step_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('late_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'unevaluated_blocking_total', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('unevaluated_blocking_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'separation_violation_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('separation_violation_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'asserted_only_control_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('asserted_only_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'unreachable_handling_failure_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('unreachable_failure_key', 'field', 'procedure_execution_id')]},
        {'field': 'retention_breach_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('retention_breach_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'cleared_legal_review_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('cleared_legal_review_key', 'field', 'procedure_execution_id')]},
        {'field': 'abandoned_failure_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('abandoned_failure_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'delivered_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('reached_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'total_delivery_attempt_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('procedure_execution', 'field', 'procedure_execution_id')]},
        {'field': 'mishandled_refusal_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('refusal_failure_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'unclean_step_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('procedure_execution_when_unclean', 'field', 'procedure_execution_id')]},
        {'field': 'count_of_approval_executions', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('is_approval_execution', 'literal', True)]},
        {'field': 'count_of_delivery_executions', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('step', 'literal', 'policy-07')]},
        {'field': 'invalid_approval_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('run_when_invalid_approval', 'field', 'procedure_execution_id')]},
        {'field': 'vacuously_clean_step_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('vacuously_clean_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'preparation_step_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('preparation_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'approval_step_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('approval_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'ungoverned_divergence_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('ungoverned_divergence_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'computedly_witnessed_control_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('computed_witness_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'interested_party_assertion_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('interested_assertion_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'independent_human_observation_count', 'op': 'COUNTIFS', 'table': 'verification_outcomes', 'criteria': [('independent_observation_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'self_attested_approval_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('self_attested_approval_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'latest_attestation_instant', 'op': 'MAX', 'table': 'attestations', 'target': 'signed_at', 'criteria': [('procedure_execution', 'field', 'procedure_execution_id')], 'suffix': None},
        {'field': 'attestation_count', 'op': 'COUNTIFS', 'table': 'attestations', 'criteria': [('procedure_execution', 'field', 'procedure_execution_id')]},
        {'field': 'post_attestation_score_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('post_attestation_score_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'intended_recipient_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('intent_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'reached_recipient_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('delivered_intent_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'silently_dropped_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('dropped_intent_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'unrecorded_refusal_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('unrecorded_refusal_execution_key', 'field', 'procedure_execution_id')]},
        {'field': 'independently_confirmed_intent_count', 'op': 'COUNTIFS', 'table': 'send_intents', 'criteria': [('independently_confirmed_execution_key', 'field', 'procedure_execution_id')]},]},
    {'name': 'StepExecutions', 'file': 'step_executions', 'rulebook_rows': 12,
     'compute': compute_step_executions_fields,
     'fields': ['step_execution_id', 'name', 'procedure_execution', 'step', 'executed_by_agent', 'execution_status', 'started_at', 'ended_at', 'verification_result', 'deviation', 'actual_duration_minutes', 'expected_duration_minutes', 'is_late', 'blocking_unmet_count', 'blocking_unmet_count_safe', 'proceeded_past_blocking_control', 'expected_blocking_count', 'evaluated_blocking_count', 'unevaluated_blocking_count', 'has_unevaluated_blocking_control', 'stale_authoritative_source_count', 'ran_on_stale_authoritative_source', 'has_deviation_note', 'is_late_and_unexplained', 'available_exception_count_for_step', 'had_uninvoked_exception_available', 'expected_verification_count', 'performed_verification_count', 'skipped_verification_count', 'has_skipped_verification', 'claims_pass_without_evidence', 'step_is_preparation', 'step_is_approval', 'preparer_agent_key', 'approver_agent_key', 'prepared_by_this_agent_count', 'violates_separation_of_duties', 'required_role_for_step', 'executor_role_key', 'executor_authority_count', 'executor_held_required_role', 'is_unauthorized_approval', 'completed_execution_key', 'control_breach_execution_key', 'late_execution_key', 'executor_agent_kind', 'executor_is_human', 'step_requires_human_confirmation', 'non_human_ran_human_step', 'non_human_approval', 'unevaluated_blocking_execution_key', 'separation_violation_execution_key', 'self_witnessed_verification_count', 'unbacked_verification_count', 'approval_rests_on_self_attestation', 'exception_invocation_count', 'ran_under_exception', 'is_completed', 'is_verification_passed', 'is_legal_review_step', 'cleared_legal_review_key', 'assigned_role', 'role_current_agent', 'executor_is_designated_agent', 'inputs_were_fresh_at_run', 'ran_on_stale_inputs', 'unresolved_issue_count', 'has_deviation', 'is_clean', 'procedure_execution_when_unclean', 'evaluated_requirement_count', 'required_blocking_count', 'has_unevaluated_blocking_requirement', 'executing_agent_kind', 'was_executed_by_software', 'step_is_software_assigned', 'software_did_human_work', 'is_approval_execution', 'is_verified', 'unconfirmed_non_human_decision_count', 'requires_human_confirmation', 'human_confirmation_missing', 'drafted_from_unusable_source', 'inputs_were_usable', 'software_execution_step_key', 'step_control_kind', 'unfalsified_clearance_count', 'all_clearances_are_unfalsified', 'stale_at_run_count', 'was_stale_when_i_ran_it', 'staleness_answer_is_tense_dependent', 'has_any_declared_check', 'performed_check_count', 'declared_check_count', 'is_unchecked_by_design', 'is_vacuously_clean', 'is_substantively_clean', 'vacuously_clean_execution_key', 'uncorroborated_pass_count', 'evidence_position_is_weak', 'preparation_execution_key', 'approval_execution_key', 'has_governing_instrument', 'has_approved_change_coverage', 'version_of_step', 'is_ungoverned_divergence', 'ungoverned_divergence_execution_key', 'self_attested_approval_execution_key', 'semantic_type_iri'],
     'calculated': {'executor_role_key', 'is_completed', 'is_ungoverned_divergence', 'all_clearances_are_unfalsified', 'control_breach_execution_key', 'name', 'has_deviation_note', 'is_clean', 'violates_separation_of_duties', 'separation_violation_execution_key', 'approver_agent_key', 'is_unchecked_by_design', 'has_governing_instrument', 'is_verified', 'drafted_from_unusable_source', 'is_vacuously_clean', 'is_legal_review_step', 'executor_held_required_role', 'approval_rests_on_self_attestation', 'is_unauthorized_approval', 'cleared_legal_review_key', 'has_any_declared_check', 'executor_is_human', 'is_late', 'ran_under_exception', 'unevaluated_blocking_count', 'executor_is_designated_agent', 'approval_execution_key', 'had_uninvoked_exception_available', 'is_substantively_clean', 'is_verification_passed', 'staleness_answer_is_tense_dependent', 'non_human_approval', 'declared_check_count', 'skipped_verification_count', 'non_human_ran_human_step', 'actual_duration_minutes', 'evidence_position_is_weak', 'has_skipped_verification', 'is_late_and_unexplained', 'performed_check_count', 'has_unevaluated_blocking_control', 'preparer_agent_key', 'was_stale_when_i_ran_it', 'procedure_execution_when_unclean', 'claims_pass_without_evidence', 'preparation_execution_key', 'ran_on_stale_inputs', 'vacuously_clean_execution_key', 'has_deviation', 'unevaluated_blocking_execution_key', 'proceeded_past_blocking_control', 'software_did_human_work', 'ungoverned_divergence_execution_key', 'human_confirmation_missing', 'completed_execution_key', 'ran_on_stale_authoritative_source', 'has_unevaluated_blocking_requirement', 'late_execution_key', 'software_execution_step_key', 'was_executed_by_software', 'self_attested_approval_execution_key'},
     'lookups': [
        {'field': 'expected_duration_minutes', 'target': 'steps', 'return': 'expected_duration_minutes', 'key': 'step', 'match': 'step_id'},
        {'field': 'expected_blocking_count', 'target': 'steps', 'return': 'blocking_requirement_count', 'key': 'step', 'match': 'step_id'},
        {'field': 'stale_authoritative_source_count', 'target': 'steps', 'return': 'authoritative_stale_count', 'key': 'step', 'match': 'step_id'},
        {'field': 'available_exception_count_for_step', 'target': 'steps', 'return': 'available_exception_count', 'key': 'step', 'match': 'step_id'},
        {'field': 'expected_verification_count', 'target': 'steps', 'return': 'declared_verification_count', 'key': 'step', 'match': 'step_id'},
        {'field': 'step_is_preparation', 'target': 'steps', 'return': 'is_preparation_step', 'key': 'step', 'match': 'step_id'},
        {'field': 'step_is_approval', 'target': 'steps', 'return': 'is_approval_step', 'key': 'step', 'match': 'step_id'},
        {'field': 'required_role_for_step', 'target': 'steps', 'return': 'assigned_role', 'key': 'step', 'match': 'step_id'},
        {'field': 'executor_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'executed_by_agent', 'match': 'agent_id'},
        {'field': 'step_requires_human_confirmation', 'target': 'steps', 'return': 'requires_human_confirmation', 'key': 'step', 'match': 'step_id'},
        {'field': 'assigned_role', 'target': 'steps', 'return': 'assigned_role', 'key': 'step', 'match': 'step_id'},
        {'field': 'role_current_agent', 'target': 'roles', 'return': 'current_agent', 'key': 'assigned_role', 'match': 'role_id'},
        {'field': 'inputs_were_fresh_at_run', 'target': 'steps', 'return': 'inputs_are_fresh', 'key': 'step', 'match': 'step_id'},
        {'field': 'required_blocking_count', 'target': 'steps', 'return': 'blocking_requirement_count', 'key': 'step', 'match': 'step_id'},
        {'field': 'executing_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'executed_by_agent', 'match': 'agent_id'},
        {'field': 'step_is_software_assigned', 'target': 'steps', 'return': 'is_software_assigned', 'key': 'step', 'match': 'step_id'},
        {'field': 'is_approval_execution', 'target': 'steps', 'return': 'is_human_approval_gate', 'key': 'step', 'match': 'step_id'},
        {'field': 'requires_human_confirmation', 'target': 'steps', 'return': 'requires_human_confirmation', 'key': 'step', 'match': 'step_id'},
        {'field': 'inputs_were_usable', 'target': 'steps', 'return': 'all_sources_usable', 'key': 'step', 'match': 'step_id'},
        {'field': 'step_control_kind', 'target': 'steps', 'return': 'control_kind', 'key': 'step', 'match': 'step_id'},
        {'field': 'has_approved_change_coverage', 'target': 'procedure_versions', 'return': 'has_approved_change_request', 'key': 'version_of_step', 'match': 'procedure_version_id'},
        {'field': 'version_of_step', 'target': 'steps', 'return': 'procedure_version', 'key': 'step', 'match': 'step_id'},],
     'aggregations': [
        {'field': 'blocking_unmet_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('step_execution', 'field', 'step_execution_id'), ('is_blocking_and_unmet', 'literal', True)]},
        {'field': 'blocking_unmet_count_safe', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('blocking_unmet_step_key', 'field', 'step_execution_id')]},
        {'field': 'evaluated_blocking_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('blocking_satisfaction_step_key', 'field', 'step_execution_id')]},
        {'field': 'performed_verification_count', 'op': 'COUNTIFS', 'table': 'verification_outcomes', 'criteria': [('step_execution', 'field', 'step_execution_id')]},
        {'field': 'prepared_by_this_agent_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('preparer_agent_key', 'field', 'approver_agent_key')]},
        {'field': 'executor_authority_count', 'op': 'COUNTIFS', 'table': 'role_assignments', 'criteria': [('agent_role_key', 'field', 'executor_role_key')]},
        {'field': 'self_witnessed_verification_count', 'op': 'COUNTIFS', 'table': 'verification_outcomes', 'criteria': [('self_witnessed_step_key', 'field', 'step_execution_id')]},
        {'field': 'unbacked_verification_count', 'op': 'COUNTIFS', 'table': 'verification_outcomes', 'criteria': [('unbacked_step_key', 'field', 'step_execution_id')]},
        {'field': 'exception_invocation_count', 'op': 'COUNTIFS', 'table': 'exception_invocations', 'criteria': [('step_execution', 'field', 'step_execution_id')]},
        {'field': 'unresolved_issue_count', 'op': 'COUNTIFS', 'table': 'issue_occurrences', 'criteria': [('step_execution_when_unresolved', 'field', 'step_execution_id')]},
        {'field': 'evaluated_requirement_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('step_execution_when_scored', 'field', 'step_execution_id')]},
        {'field': 'unconfirmed_non_human_decision_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('step_execution_when_unconfirmed', 'field', 'step_execution_id')]},
        {'field': 'unfalsified_clearance_count', 'op': 'COUNTIFS', 'table': 'requirement_satisfactions', 'criteria': [('unfalsified_clearance_step_key', 'field', 'step_execution_id')]},
        {'field': 'stale_at_run_count', 'op': 'COUNTIFS', 'table': 'binding_observations', 'criteria': [('stale_at_run_step_key', 'field', 'step_execution_id')]},
        {'field': 'uncorroborated_pass_count', 'op': 'COUNTIFS', 'table': 'verification_outcomes', 'criteria': [('uncorroborated_pass_step_key', 'field', 'step_execution_id')]},]},
    {'name': 'RequirementSatisfactions', 'file': 'requirement_satisfactions', 'rulebook_rows': 8,
     'compute': compute_requirement_satisfactions_fields,
     'fields': ['requirement_satisfaction_id', 'name', 'step_execution', 'requirement', 'satisfaction_level', 'evidence', 'evaluated_by_agent', 'evaluated_at', 'requirement_is_blocking', 'is_fully_satisfied', 'is_blocking_and_unmet', 'blocking_unmet_step_key', 'blocking_satisfaction_step_key', 'negative_outcome_requirement_key', 'evaluator_agent_kind', 'non_human_evaluated_human_control', 'requirement_has_computed_witness', 'is_asserted_only', 'asserted_only_execution_key', 'parent_procedure_execution', 'step_execution_when_scored', 'is_human_evaluated', 'requirement_is_approval_type', 'is_invalid_approval', 'procedure_execution_of_satisfaction', 'run_when_invalid_approval', 'requirement_is_unfalsified', 'is_clearance_by_unfalsified_control', 'unfalsified_clearance_step_key', 'spec_step_of_execution', 'binding_key', 'scored_step_executor_agent', 'evaluator_is_step_executor', 'run_owner_agent', 'evaluator_owns_the_run', 'is_interested_party_assertion', 'has_written_evidence', 'is_bare_assertion', 'interested_assertion_execution_key', 'is_computedly_witnessed', 'computed_witness_execution_key', 'step_executor_agent', 'was_scored_after_attestation', 'attestation_instant_for_run', 'post_attestation_score_execution_key', 'semantic_type_iri'],
     'calculated': {'step_execution_when_scored', 'non_human_evaluated_human_control', 'is_invalid_approval', 'name', 'unfalsified_clearance_step_key', 'blocking_satisfaction_step_key', 'interested_assertion_execution_key', 'evaluator_owns_the_run', 'computed_witness_execution_key', 'has_written_evidence', 'run_when_invalid_approval', 'post_attestation_score_execution_key', 'blocking_unmet_step_key', 'evaluator_is_step_executor', 'is_human_evaluated', 'is_fully_satisfied', 'asserted_only_execution_key', 'was_scored_after_attestation', 'is_interested_party_assertion', 'is_bare_assertion', 'is_clearance_by_unfalsified_control', 'is_asserted_only', 'is_computedly_witnessed', 'is_blocking_and_unmet', 'negative_outcome_requirement_key'},
     'lookups': [
        {'field': 'requirement_is_blocking', 'target': 'requirements', 'return': 'is_blocking', 'key': 'requirement', 'match': 'requirement_id'},
        {'field': 'evaluator_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'evaluated_by_agent', 'match': 'agent_id'},
        {'field': 'requirement_has_computed_witness', 'target': 'requirements', 'return': 'has_computed_witness', 'key': 'requirement', 'match': 'requirement_id'},
        {'field': 'parent_procedure_execution', 'target': 'step_executions', 'return': 'procedure_execution', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'requirement_is_approval_type', 'target': 'requirements', 'return': 'requirement_type', 'key': 'requirement', 'match': 'requirement_id'},
        {'field': 'procedure_execution_of_satisfaction', 'target': 'step_executions', 'return': 'procedure_execution', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'requirement_is_unfalsified', 'target': 'requirements', 'return': 'is_unfalsified_control', 'key': 'requirement', 'match': 'requirement_id'},
        {'field': 'spec_step_of_execution', 'target': 'step_executions', 'return': 'step', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'binding_key', 'target': 'step_requirements', 'return': 'step_requirement_id', 'key': 'requirement_satisfaction_id', 'match': 'step_requirement_id'},
        {'field': 'scored_step_executor_agent', 'target': 'step_executions', 'return': 'executed_by_agent', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'run_owner_agent', 'target': 'procedure_executions', 'return': 'executed_by_agent', 'key': 'parent_procedure_execution', 'match': 'procedure_execution_id'},
        {'field': 'step_executor_agent', 'target': 'step_executions', 'return': 'executed_by_agent', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'attestation_instant_for_run', 'target': 'procedure_executions', 'return': 'latest_attestation_instant', 'key': 'parent_procedure_execution', 'match': 'procedure_execution_id'},],
     'aggregations': []},
    {'name': 'Errors', 'file': 'errors', 'rulebook_rows': 2,
     'compute': compute_errors_fields,
     'fields': ['error_id', 'name', 'label', 'error_code', 'error_cause', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'IssueOccurrences', 'file': 'issue_occurrences', 'rulebook_rows': 2,
     'compute': compute_issue_occurrences_fields,
     'fields': ['issue_occurrence_id', 'name', 'step_execution', 'error', 'encountered_by_agent', 'occurred_at', 'issue_cause', 'issue_solution', 'status', 'is_unresolved', 'step_execution_when_unresolved', 'semantic_type_iri'],
     'calculated': {'step_execution_when_unresolved', 'is_unresolved', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'UserQuestions', 'file': 'user_questions', 'rulebook_rows': 2,
     'compute': compute_user_questions_fields,
     'fields': ['user_question_id', 'name', 'step_execution', 'asked_by_agent', 'asked_at', 'question_text', 'resolved_by_faq', 'addressed_by_resource', 'status', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'UserFeedback', 'file': 'user_feedback', 'rulebook_rows': 2,
     'compute': compute_user_feedback_fields,
     'fields': ['user_feedback_id', 'name', 'procedure_execution', 'provided_by_agent', 'provided_at', 'feedback_text', 'disposition', 'change_request_key', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'StewardshipAssignments', 'file': 'stewardship_assignments', 'rulebook_rows': 2,
     'compute': compute_stewardship_assignments_fields,
     'fields': ['stewardship_assignment_id', 'name', 'procedure_version', 'steward_role', 'authority_role', 'valid_from', 'valid_to', 'review_cadence_days', 'count_of_review_events', 'has_ever_been_reviewed', 'evaluation_context', 'as_of_instant', 'is_current_assignment', 'semantic_type_iri'],
     'calculated': {'name', 'has_ever_been_reviewed', 'is_current_assignment'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},],
     'aggregations': [
        {'field': 'count_of_review_events', 'op': 'COUNTIFS', 'table': 'review_events', 'criteria': [('procedure_version', 'field', 'procedure_version')]},]},
    {'name': 'ChangeRequests', 'file': 'change_requests', 'rulebook_rows': 2,
     'compute': compute_change_requests_fields,
     'fields': ['change_request_id', 'name', 'procedure_version', 'title', 'change_kind', 'status', 'requested_by_agent', 'authority_role', 'requested_at', 'decided_at', 'impact_assessment', 'is_open', 'open_change_version_key', 'is_decided', 'evaluation_context', 'as_of_instant', 'days_pending', 'is_still_pending', 'is_stalled', 'authority_agent', 'requester_is_authority', 'awaits_authority_decision', 'authority_role_label', 'touches_live_version', 'is_live_decision_backlog', 'blocks_an_open_gap', 'implemented_at', 'backlog_version_key', 'is_my_pending_decision', 'is_my_blocking_backlog', 'is_my_overdue_backlog', 'is_implemented', 'is_my_decided_request', 'is_my_decided_but_unlanded', 'decision_latency_days', 'implementation_latency_days', 'delay_is_downstream_of_me', 'unlanded_version_key', 'is_approved_not_implemented', 'days_since_approval', 'is_stalled_implementation', 'stalled_implementation_version_key', 'approved_version_key', 'is_approved_decision', 'semantic_type_iri'],
     'calculated': {'is_my_decided_request', 'is_approved_decision', 'unlanded_version_key', 'is_stalled', 'blocks_an_open_gap', 'name', 'is_my_blocking_backlog', 'is_open', 'decision_latency_days', 'days_since_approval', 'open_change_version_key', 'is_implemented', 'is_still_pending', 'backlog_version_key', 'is_decided', 'implementation_latency_days', 'stalled_implementation_version_key', 'delay_is_downstream_of_me', 'is_my_overdue_backlog', 'is_approved_not_implemented', 'is_live_decision_backlog', 'requester_is_authority', 'is_stalled_implementation', 'is_my_decided_but_unlanded', 'is_my_pending_decision', 'approved_version_key', 'awaits_authority_decision', 'days_pending'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'authority_agent', 'target': 'roles', 'return': 'current_agent', 'key': 'authority_role', 'match': 'role_id'},
        {'field': 'authority_role_label', 'target': 'roles', 'return': 'label', 'key': 'authority_role', 'match': 'role_id'},
        {'field': 'touches_live_version', 'target': 'procedure_versions', 'return': 'is_live', 'key': 'procedure_version', 'match': 'procedure_version_id'},],
     'aggregations': []},
    {'name': 'ReviewEvents', 'file': 'review_events', 'rulebook_rows': 3,
     'compute': compute_review_events_fields,
     'fields': ['review_event_id', 'name', 'procedure_version', 'review_kind', 'reviewed_at', 'reviewed_by_agent', 'outcome', 'related_change_request', 'next_review_due', 'evaluation_context', 'as_of_instant', 'is_overdue', 'overdue_version_key', 'promised_cadence_days', 'days_since_reviewed', 'exceeds_promised_cadence', 'cadence_drift_days', 'promise_and_behavior_disagree', 'cadence_breach_version_key', 'semantic_type_iri'],
     'calculated': {'exceeds_promised_cadence', 'overdue_version_key', 'cadence_breach_version_key', 'is_overdue', 'days_since_reviewed', 'name', 'cadence_drift_days', 'promise_and_behavior_disagree'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'promised_cadence_days', 'target': 'procedure_versions', 'return': 'steward_review_cadence_days', 'key': 'procedure_version', 'match': 'procedure_version_id'},],
     'aggregations': []},
    {'name': 'LearningActivities', 'file': 'learning_activities', 'rulebook_rows': 2,
     'compute': compute_learning_activities_fields,
     'fields': ['learning_activity_id', 'name', 'community_of_practice', 'procedure_version', 'activity_kind', 'occurred_at', 'facilitator_agent', 'outcome', 'evidence_resource', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'OperationalBindings', 'file': 'operational_bindings', 'rulebook_rows': 5,
     'compute': compute_operational_bindings_fields,
     'fields': ['operational_binding_id', 'name', 'procedure_version', 'step', 'resource', 'access_mode', 'record_or_schema_key', 'last_observed_at', 'freshness_sla_minutes', 'is_authoritative', 'evaluation_context', 'as_of_instant', 'age_minutes', 'is_fresh', 'stale_binding_step_key', 'authoritative_stale_step_key', 'is_stale_and_authoritative', 'step_when_stale', 'resource_is_approved', 'is_usable_for_drafting', 'step_when_unusable', 'semantic_type_iri'],
     'calculated': {'is_fresh', 'step_when_stale', 'stale_binding_step_key', 'authoritative_stale_step_key', 'is_stale_and_authoritative', 'name', 'is_usable_for_drafting', 'step_when_unusable', 'age_minutes'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'resource_is_approved', 'target': 'resources', 'return': 'is_approved_source', 'key': 'resource', 'match': 'resource_id'},],
     'aggregations': []},
    {'name': 'CommunicationPolicies', 'file': 'communication_policies', 'rulebook_rows': 2,
     'compute': compute_communication_policies_fields,
     'fields': ['communication_policy_id', 'name', 'procedure_version', 'channel', 'audience_rule', 'consent_required', 'quiet_hours_start', 'quiet_hours_end', 'max_message_length', 'max_segments', 'retention_days', 'approval_role', 'required_content', 'authority_statement', 'status', 'consent_violation_count', 'quiet_hours_start_hour', 'quiet_hours_end_hour', 'quiet_hours_violation_count', 'required_opt_out_phrase', 'is_active_policy', 'semantic_type_iri'],
     'calculated': {'is_active_policy', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'consent_violation_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('policy_channel', 'field', 'communication_policy_id'), ('is_consent_violation', 'literal', True)]},
        {'field': 'quiet_hours_violation_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('quiet_hours_violation_policy_key', 'field', 'communication_policy_id')]},]},
    {'name': 'MessageTemplates', 'file': 'message_templates', 'rulebook_rows': 2,
     'compute': compute_message_templates_fields,
     'fields': ['message_template_id', 'name', 'communication_policy', 'resource', 'subject_template', 'body_template', 'locale', 'status', 'policy_max_message_length', 'policy_max_segments', 'body_template_length', 'is_template_over_length', 'valid_approval_count', 'has_valid_approval', 'is_claiming_unbacked_approval', 'current_body_hash', 'last_approved_body_hash', 'last_valid_approval', 'has_body_drifted', 'is_sendable_under_approval', 'drifted_send_count', 'unanswered_delivery_count', 'transmitted_delivery_count', 'template_draws_no_response', 'last_approval_at', 'semantic_type_iri'],
     'calculated': {'has_valid_approval', 'is_claiming_unbacked_approval', 'has_body_drifted', 'is_sendable_under_approval', 'is_template_over_length', 'name', 'body_template_length', 'template_draws_no_response'},
     'lookups': [
        {'field': 'policy_max_message_length', 'target': 'communication_policies', 'return': 'max_message_length', 'key': 'communication_policy', 'match': 'communication_policy_id'},
        {'field': 'policy_max_segments', 'target': 'communication_policies', 'return': 'max_segments', 'key': 'communication_policy', 'match': 'communication_policy_id'},
        {'field': 'last_approved_body_hash', 'target': 'template_approvals', 'return': 'approved_body_hash', 'key': 'last_valid_approval', 'match': 'template_approval_id'},
        {'field': 'last_approval_at', 'target': 'template_approvals', 'return': 'decided_at', 'key': 'last_valid_approval', 'match': 'template_approval_id'},],
     'aggregations': [
        {'field': 'valid_approval_count', 'op': 'COUNTIFS', 'table': 'template_approvals', 'criteria': [('valid_approval_template_key', 'field', 'message_template_id')]},
        {'field': 'drifted_send_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('drifted_send_template_key', 'field', 'message_template_id')]},
        {'field': 'unanswered_delivery_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('unanswered_template_key', 'field', 'message_template_id')]},
        {'field': 'transmitted_delivery_count', 'op': 'COUNTIFS', 'table': 'message_deliveries', 'criteria': [('transmitted_template_key', 'field', 'message_template_id')]},]},
    {'name': 'SemanticMappings', 'file': 'semantic_mappings', 'rulebook_rows': 41,
     'compute': compute_semantic_mappings_fields,
     'fields': ['semantic_mapping_id', 'name', 'source_path', 'mapping_kind', 'target_iri', 'mapping_relation', 'ontology_profile', 'notes'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'WitnessLoops', 'file': 'witness_loops', 'rulebook_rows': 3,
     'compute': compute_witness_loops_fields,
     'fields': ['witness_loop_id', 'name', 'loop_number', 'title', 'premise', 'started_at', 'completed_at', 'question_count', 'is_complete', 'fields_after', 'derived_after', 'witnessed_after', 'semantic_type_iri'],
     'calculated': {'name', 'is_complete'},
     'lookups': [],
     'aggregations': [
        {'field': 'question_count', 'op': 'COUNTIFS', 'table': 'role_questions', 'criteria': [('witness_loop', 'field', 'witness_loop_id')]},]},
    {'name': 'RoleQuestions', 'file': 'role_questions', 'rulebook_rows': 108,
     'compute': compute_role_questions_fields,
     'fields': ['role_question_id', 'name', 'asking_role', 'witness_loop', 'question_text', 'why_it_matters', 'answerable_before', 'predicate_count', 'is_answered', 'witnessed_answer', 'semantic_type_iri'],
     'calculated': {'is_answered', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'predicate_count', 'op': 'COUNTIFS', 'table': 'rulebook_fields', 'criteria': [('invented_for_question', 'field', 'role_question_id')]},]},
    {'name': 'RulebookFields', 'file': 'rulebook_fields', 'rulebook_rows': 1771,
     'compute': compute_rulebook_fields_fields,
     'fields': ['rulebook_field_id', 'name', 'target_table', 'field_name', 'field_type', 'datatype', 'formula', 'invented_for_question', 'is_derived', 'is_witness', 'semantic_type_iri'],
     'calculated': {'is_witness', 'is_derived', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'TestSuites', 'file': 'test_suites', 'rulebook_rows': 6,
     'compute': compute_test_suites_fields,
     'fields': ['test_suite_id', 'name', 'label', 'test_count', 'pass_count', 'blocking_fail_count', 'is_green', 'semantic_type_iri'],
     'calculated': {'name', 'is_green'},
     'lookups': [],
     'aggregations': [
        {'field': 'test_count', 'op': 'COUNTIFS', 'table': 'test_cases', 'criteria': [('suite', 'field', 'test_suite_id')]},
        {'field': 'pass_count', 'op': 'COUNTIFS', 'table': 'test_cases', 'criteria': [('passing_suite_key', 'field', 'test_suite_id')]},
        {'field': 'blocking_fail_count', 'op': 'COUNTIFS', 'table': 'test_cases', 'criteria': [('needs_attention_suite_key', 'field', 'test_suite_id')]},]},
    {'name': 'TestCases', 'file': 'test_cases', 'rulebook_rows': 1794,
     'compute': compute_test_cases_fields,
     'fields': ['test_case_id', 'name', 'test_kind', 'subject', 'target_table', 'target_field', 'assertion', 'defends_question', 'suite', 'severity', 'is_blocking', 'last_outcome', 'last_detail', 'last_run_at', 'is_passing', 'is_failing', 'needs_attention', 'passing_suite_key', 'needs_attention_suite_key', 'semantic_type_iri'],
     'calculated': {'is_passing', 'needs_attention_suite_key', 'is_blocking', 'is_failing', 'name', 'passing_suite_key', 'needs_attention'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ERBVersions', 'file': 'erb_versions', 'rulebook_rows': 1,
     'compute': compute_erb_versions_fields,
     'fields': ['erb_version_id', 'base_id', 'name', 'message', 'notes', 'commit_date', 'is_published'],
     'calculated': set(),
     'lookups': [],
     'aggregations': []},
    {'name': 'ERBCustomizations', 'file': 'erb_customizations', 'rulebook_rows': 0,
     'compute': compute_erb_customizations_fields,
     'fields': ['erb_customization_id', 'name', 'title', 'sql_code', 'sql_target', 'customization_type'],
     'calculated': set(),
     'lookups': [],
     'aggregations': []},
    {'name': '__meta__', 'file': '__meta__', 'rulebook_rows': 19,
     'compute': compute___meta___fields,
     'fields': ['meta_key', 'name', 'value_type', 'string_value', 'json_value'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ExceptionInvocations', 'file': 'exception_invocations', 'rulebook_rows': 1,
     'compute': compute_exception_invocations_fields,
     'fields': ['exception_invocation_id', 'name', 'step_execution', 'exception', 'invoked_by_agent', 'approved_by_agent', 'invoked_at', 'handling_applied', 'expected_handling', 'required_approval_role', 'required_approval_role_holder', 'approval_role_matches', 'is_approved', 'is_improperly_approved', 'invoker_agent_kind', 'invoker_also_prepared_key', 'parent_procedure_execution', 'approver_prepared_count', 'delegated_to_preparer', 'is_ungoverned_invocation', 'semantic_type_iri'],
     'calculated': {'delegated_to_preparer', 'is_improperly_approved', 'is_approved', 'invoker_also_prepared_key', 'approval_role_matches', 'name', 'is_ungoverned_invocation'},
     'lookups': [
        {'field': 'expected_handling', 'target': 'exceptions', 'return': 'handling', 'key': 'exception', 'match': 'exception_id'},
        {'field': 'required_approval_role', 'target': 'exceptions', 'return': 'approval_role', 'key': 'exception', 'match': 'exception_id'},
        {'field': 'required_approval_role_holder', 'target': 'roles', 'return': 'current_agent', 'key': 'required_approval_role', 'match': 'role_id'},
        {'field': 'invoker_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'invoked_by_agent', 'match': 'agent_id'},
        {'field': 'parent_procedure_execution', 'target': 'step_executions', 'return': 'procedure_execution', 'key': 'step_execution', 'match': 'step_execution_id'},],
     'aggregations': [
        {'field': 'approver_prepared_count', 'op': 'COUNTIFS', 'table': 'step_executions', 'criteria': [('preparer_agent_key', 'field', 'invoker_also_prepared_key')]},]},
    {'name': 'VerificationOutcomes', 'file': 'verification_outcomes', 'rulebook_rows': 4,
     'compute': compute_verification_outcomes_fields,
     'fields': ['verification_outcome_id', 'name', 'step_execution', 'step_verification', 'observed_signal_value', 'observed_by_agent', 'observed_at', 'evidence_uri', 'expected_signal_value', 'signal_identifier', 'signal_matches_expected', 'has_evidence', 'is_unbacked_observation', 'is_self_witnessed', 'step_executor_agent', 'self_witnessed_step_key', 'unbacked_step_key', 'is_self_witnessed_and_unbacked', 'is_uncorroborated_pass', 'uncorroborated_pass_step_key', 'observer_is_non_human', 'observer_is_independent_of_executor', 'is_independent_human_observation', 'independent_observation_execution_key', 'parent_procedure_execution_of_outcome', 'semantic_type_iri'],
     'calculated': {'independent_observation_execution_key', 'is_self_witnessed', 'has_evidence', 'is_independent_human_observation', 'is_self_witnessed_and_unbacked', 'signal_matches_expected', 'is_uncorroborated_pass', 'self_witnessed_step_key', 'is_unbacked_observation', 'observer_is_independent_of_executor', 'name', 'unbacked_step_key', 'uncorroborated_pass_step_key'},
     'lookups': [
        {'field': 'expected_signal_value', 'target': 'step_verifications', 'return': 'expected_signal_value', 'key': 'step_verification', 'match': 'step_verification_id'},
        {'field': 'signal_identifier', 'target': 'step_verifications', 'return': 'signal_identifier', 'key': 'step_verification', 'match': 'step_verification_id'},
        {'field': 'step_executor_agent', 'target': 'step_executions', 'return': 'executed_by_agent', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'observer_is_non_human', 'target': 'agents', 'return': 'is_non_human', 'key': 'observed_by_agent', 'match': 'agent_id'},
        {'field': 'parent_procedure_execution_of_outcome', 'target': 'step_executions', 'return': 'procedure_execution', 'key': 'step_execution', 'match': 'step_execution_id'},],
     'aggregations': []},
    {'name': 'ObservedTransitions', 'file': 'observed_transitions', 'rulebook_rows': 10,
     'compute': compute_observed_transitions_fields,
     'fields': ['observed_transition_id', 'name', 'procedure_execution', 'step_transition', 'arriving_step_execution', 'observed_at', 'trigger_reason', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'Recipients', 'file': 'recipients', 'rulebook_rows': 5,
     'compute': compute_recipients_fields,
     'fields': ['recipient_id', 'name', 'display_name', 'organization', 'email_address', 'mobile_number', 'sms_consent_status', 'sms_consent_at', 'consent_binding', 'has_sms_consent', 'is_email_reachable', 'is_sms_reachable', 'is_unreachable', 'is_communicationally_stranded', 'semantic_type_iri'],
     'calculated': {'is_sms_reachable', 'is_communicationally_stranded', 'name', 'is_email_reachable', 'has_sms_consent', 'is_unreachable'},
     'lookups': [],
     'aggregations': []},
    {'name': 'MessageDeliveries', 'file': 'message_deliveries', 'rulebook_rows': 6,
     'compute': compute_message_deliveries_fields,
     'fields': ['message_delivery_id', 'name', 'procedure_execution', 'step_execution', 'recipient', 'message_template', 'sent_by_agent', 'rendered_body', 'sent_at', 'sent_at_local_hour', 'delivery_status', 'suppression_reason', 'invoked_exception', 'acknowledged_at', 'policy_channel', 'channel_name', 'policy_requires_consent', 'recipient_has_sms_consent', 'was_actually_transmitted', 'is_consent_violation', 'consent_violation_policy_key', 'policy_quiet_hours_start_hour', 'policy_quiet_hours_end_hour', 'policy_has_quiet_hours', 'quiet_window_wraps_midnight', 'is_inside_quiet_window', 'is_quiet_hours_violation', 'quiet_hours_violation_policy_key', 'recipient_is_unreachable', 'is_acknowledged', 'invoked_exception_condition', 'has_unreachable_exception_invoked', 'is_fabricated_acknowledgement', 'is_unhandled_unreachable', 'unreachable_failure_key', 'policy_retention_days', 'evaluation_context', 'as_of_instant', 'age_days', 'is_within_retention_window', 'has_rendered_body', 'is_evidence_required', 'is_retention_breach', 'retention_breach_execution_key', 'sending_step_execution_step', 'execution_has_cleared_legal_review', 'is_unreviewed_send', 'rendered_body_length', 'policy_max_message_length_at_send', 'segment_count', 'policy_max_segments_at_send', 'is_over_segment_limit', 'template_has_valid_approval', 'is_unapproved_send', 'policy_required_opt_out_phrase', 'policy_requires_opt_out', 'opt_out_phrase_position', 'has_opt_out_phrase', 'is_opt_out_in_first_segment', 'is_missing_required_opt_out', 'is_opt_out_at_risk_of_truncation', 'is_failed_delivery', 'is_suppressed', 'is_triaged', 'is_abandoned_failure', 'abandoned_failure_execution_key', 'reached_execution_key', 'template_was_sendable', 'is_drifted_send', 'drifted_send_template_key', 'was_sent_outside_business_hours', 'was_delivered_and_unanswered', 'is_poorly_timed_unanswered', 'is_well_timed_unanswered', 'unanswered_template_key', 'transmitted_template_key', 'approving_agent_at_send', 'approving_role_at_send', 'approval_decided_at_send', 'approval_preceded_send', 'has_frozen_approval_evidence', 'provenance_is_live_derived', 'current_last_approval_at', 'template_reapproved_since_send', 'is_unprovable_approval_claim', 'reminder_sent_at', 'reminder_count', 'has_sent_reminder', 'acknowledgement_is_outstanding', 'outstanding_age_days', 'is_unchased_acknowledgement', 'is_exhausted_follow_up', 'needs_human_escalation', 'semantic_type_iri'],
     'calculated': {'is_unreviewed_send', 'is_quiet_hours_violation', 'is_well_timed_unanswered', 'is_consent_violation', 'is_unchased_acknowledgement', 'quiet_hours_violation_policy_key', 'is_within_retention_window', 'has_sent_reminder', 'is_unhandled_unreachable', 'name', 'has_unreachable_exception_invoked', 'was_delivered_and_unanswered', 'segment_count', 'policy_has_quiet_hours', 'age_days', 'is_retention_breach', 'transmitted_template_key', 'abandoned_failure_execution_key', 'is_inside_quiet_window', 'is_opt_out_at_risk_of_truncation', 'consent_violation_policy_key', 'template_reapproved_since_send', 'unanswered_template_key', 'opt_out_phrase_position', 'is_exhausted_follow_up', 'retention_breach_execution_key', 'has_opt_out_phrase', 'rendered_body_length', 'is_evidence_required', 'is_suppressed', 'needs_human_escalation', 'has_frozen_approval_evidence', 'drifted_send_template_key', 'policy_requires_opt_out', 'is_acknowledged', 'is_triaged', 'provenance_is_live_derived', 'outstanding_age_days', 'is_abandoned_failure', 'acknowledgement_is_outstanding', 'has_rendered_body', 'is_poorly_timed_unanswered', 'was_sent_outside_business_hours', 'unreachable_failure_key', 'is_unapproved_send', 'is_missing_required_opt_out', 'reached_execution_key', 'approval_preceded_send', 'was_actually_transmitted', 'is_drifted_send', 'is_opt_out_in_first_segment', 'is_unprovable_approval_claim', 'is_failed_delivery', 'is_fabricated_acknowledgement', 'quiet_window_wraps_midnight', 'is_over_segment_limit'},
     'lookups': [
        {'field': 'policy_channel', 'target': 'message_templates', 'return': 'communication_policy', 'key': 'message_template', 'match': 'message_template_id'},
        {'field': 'channel_name', 'target': 'communication_policies', 'return': 'channel', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'policy_requires_consent', 'target': 'communication_policies', 'return': 'consent_required', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'recipient_has_sms_consent', 'target': 'recipients', 'return': 'has_sms_consent', 'key': 'recipient', 'match': 'recipient_id'},
        {'field': 'policy_quiet_hours_start_hour', 'target': 'communication_policies', 'return': 'quiet_hours_start_hour', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'policy_quiet_hours_end_hour', 'target': 'communication_policies', 'return': 'quiet_hours_end_hour', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'recipient_is_unreachable', 'target': 'recipients', 'return': 'is_unreachable', 'key': 'recipient', 'match': 'recipient_id'},
        {'field': 'invoked_exception_condition', 'target': 'exceptions', 'return': 'condition', 'key': 'invoked_exception', 'match': 'exception_id'},
        {'field': 'policy_retention_days', 'target': 'communication_policies', 'return': 'retention_days', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'sending_step_execution_step', 'target': 'step_executions', 'return': 'step', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'execution_has_cleared_legal_review', 'target': 'procedure_executions', 'return': 'has_cleared_legal_review', 'key': 'procedure_execution', 'match': 'procedure_execution_id'},
        {'field': 'policy_max_message_length_at_send', 'target': 'communication_policies', 'return': 'max_message_length', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'policy_max_segments_at_send', 'target': 'communication_policies', 'return': 'max_segments', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'template_has_valid_approval', 'target': 'message_templates', 'return': 'has_valid_approval', 'key': 'message_template', 'match': 'message_template_id'},
        {'field': 'policy_required_opt_out_phrase', 'target': 'communication_policies', 'return': 'required_opt_out_phrase', 'key': 'policy_channel', 'match': 'communication_policy_id'},
        {'field': 'template_was_sendable', 'target': 'message_templates', 'return': 'is_sendable_under_approval', 'key': 'message_template', 'match': 'message_template_id'},
        {'field': 'current_last_approval_at', 'target': 'message_templates', 'return': 'last_approval_at', 'key': 'message_template', 'match': 'message_template_id'},],
     'aggregations': []},
    {'name': 'TemplateApprovals', 'file': 'template_approvals', 'rulebook_rows': 2,
     'compute': compute_template_approvals_fields,
     'fields': ['template_approval_id', 'name', 'message_template', 'decided_by_agent', 'decided_in_role', 'decision', 'decided_at', 'approved_body_hash', 'notes', 'is_approval_decision', 'template_policy', 'required_approval_role', 'is_decided_by_required_role', 'valid_approval_template_key', 'semantic_type_iri'],
     'calculated': {'is_approval_decision', 'valid_approval_template_key', 'is_decided_by_required_role', 'name'},
     'lookups': [
        {'field': 'template_policy', 'target': 'message_templates', 'return': 'communication_policy', 'key': 'message_template', 'match': 'message_template_id'},
        {'field': 'required_approval_role', 'target': 'communication_policies', 'return': 'approval_role', 'key': 'template_policy', 'match': 'communication_policy_id'},],
     'aggregations': []},
    {'name': 'SendIntents', 'file': 'send_intents', 'rulebook_rows': 7,
     'compute': compute_send_intents_fields,
     'fields': ['send_intent_id', 'name', 'procedure_execution', 'step_execution', 'recipient', 'message_template', 'proposed_body', 'proposed_send_at_local_hour', 'evaluated_at', 'resulting_delivery', 'intent_policy', 'intent_channel', 'policy_is_active', 'intent_requires_consent', 'recipient_has_channel_consent', 'consent_gate_passed', 'recipient_is_sms_reachable', 'recipient_is_email_reachable', 'reachability_gate_passed', 'permission_gate_passed', 'intent_quiet_start_hour', 'intent_quiet_end_hour', 'intent_policy_has_quiet_hours', 'intent_quiet_window_wraps', 'intent_is_inside_quiet_window', 'timing_gate_passed', 'hours_until_window_opens', 'intent_max_message_length', 'intent_max_segments', 'proposed_body_length', 'proposed_segment_count', 'length_gate_passed', 'intent_required_opt_out_phrase', 'proposed_opt_out_position', 'opt_out_gate_passed', 'content_gate_passed', 'template_is_sendable', 'execution_has_legal_clearance', 'intent_approval_role', 'approval_role_agent_kind', 'approval_is_human', 'authorization_gate_passed', 'is_cleared_to_send', 'blocking_gate_name', 'has_resulting_delivery', 'resulting_delivery_was_transmitted', 'is_overridden_refusal', 'is_silently_dropped', 'resulting_delivery_exception', 'refusal_cited_an_exception', 'is_properly_handled_refusal', 'refusal_failure_execution_key', 'intent_execution_key', 'delivered_intent_execution_key', 'dropped_intent_execution_key', 'my_approval_was_in_force', 'refused_on_approved_content', 'refused_on_opt_out_only', 'refusal_was_on_my_rules', 'refusal_was_outside_my_control', 'approver_was_notified', 'is_unreported_refusal_on_my_rules', 'is_approval_overridden_silently', 'alternate_channel_intent', 'has_alternate_channel_attempt', 'alternate_attempt_was_cleared', 'is_refused_with_no_alternative', 'exception_prescribed_an_alternative', 'prescribed_handling_was_performed', 'is_suppression_without_remedy', 'refusal_recorded_at', 'refusal_notified_role', 'has_durable_refusal_record', 'refusal_was_escalated', 'is_unrecorded_refusal', 'is_unescalated_refusal', 'unescalated_refusal_role_key', 'unrecorded_refusal_execution_key', 'retry_intent', 'was_deferred_on_timing', 'evaluation_context', 'as_of_instant', 'window_has_since_reopened', 'has_retry_attempt', 'retry_was_cleared', 'is_abandoned_deferral', 'deferral_age_hours', 'is_stale_deferral', 'evaluating_role_assignment', 'enforced_by_unauthorized_agent', 'consent_input_was_resolvable', 'recipient_consent_status_raw', 'policy_input_was_resolvable', 'all_gate_inputs_resolved', 'is_unevaluable_refusal', 'gate_result_was_independently_confirmed', 'is_self_witnessed_decision', 'is_independently_confirmed', 'independently_confirmed_execution_key', 'semantic_type_iri'],
     'calculated': {'delivered_intent_execution_key', 'has_alternate_channel_attempt', 'is_unescalated_refusal', 'timing_gate_passed', 'exception_prescribed_an_alternative', 'reachability_gate_passed', 'is_unevaluable_refusal', 'window_has_since_reopened', 'is_overridden_refusal', 'consent_gate_passed', 'was_deferred_on_timing', 'refusal_was_escalated', 'name', 'is_stale_deferral', 'intent_is_inside_quiet_window', 'refused_on_opt_out_only', 'consent_input_was_resolvable', 'is_properly_handled_refusal', 'unescalated_refusal_role_key', 'is_refused_with_no_alternative', 'hours_until_window_opens', 'has_durable_refusal_record', 'is_independently_confirmed', 'approval_is_human', 'is_approval_overridden_silently', 'refusal_was_outside_my_control', 'permission_gate_passed', 'independently_confirmed_execution_key', 'dropped_intent_execution_key', 'is_unreported_refusal_on_my_rules', 'refusal_was_on_my_rules', 'has_resulting_delivery', 'refused_on_approved_content', 'all_gate_inputs_resolved', 'intent_execution_key', 'length_gate_passed', 'unrecorded_refusal_execution_key', 'is_silently_dropped', 'policy_input_was_resolvable', 'blocking_gate_name', 'intent_policy_has_quiet_hours', 'prescribed_handling_was_performed', 'is_abandoned_deferral', 'refusal_failure_execution_key', 'deferral_age_hours', 'intent_quiet_window_wraps', 'content_gate_passed', 'authorization_gate_passed', 'has_retry_attempt', 'is_unrecorded_refusal', 'is_self_witnessed_decision', 'is_suppression_without_remedy', 'opt_out_gate_passed', 'is_cleared_to_send', 'my_approval_was_in_force', 'refusal_cited_an_exception'},
     'lookups': [
        {'field': 'intent_policy', 'target': 'message_templates', 'return': 'communication_policy', 'key': 'message_template', 'match': 'message_template_id'},
        {'field': 'intent_channel', 'target': 'communication_policies', 'return': 'channel', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'policy_is_active', 'target': 'communication_policies', 'return': 'is_active_policy', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'intent_requires_consent', 'target': 'communication_policies', 'return': 'consent_required', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'recipient_has_channel_consent', 'target': 'recipients', 'return': 'has_sms_consent', 'key': 'recipient', 'match': 'recipient_id'},
        {'field': 'recipient_is_sms_reachable', 'target': 'recipients', 'return': 'is_sms_reachable', 'key': 'recipient', 'match': 'recipient_id'},
        {'field': 'recipient_is_email_reachable', 'target': 'recipients', 'return': 'is_email_reachable', 'key': 'recipient', 'match': 'recipient_id'},
        {'field': 'intent_quiet_start_hour', 'target': 'communication_policies', 'return': 'quiet_hours_start_hour', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'intent_quiet_end_hour', 'target': 'communication_policies', 'return': 'quiet_hours_end_hour', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'intent_max_message_length', 'target': 'communication_policies', 'return': 'max_message_length', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'intent_max_segments', 'target': 'communication_policies', 'return': 'max_segments', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'intent_required_opt_out_phrase', 'target': 'communication_policies', 'return': 'required_opt_out_phrase', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'template_is_sendable', 'target': 'message_templates', 'return': 'is_sendable_under_approval', 'key': 'message_template', 'match': 'message_template_id'},
        {'field': 'execution_has_legal_clearance', 'target': 'procedure_executions', 'return': 'has_cleared_legal_review', 'key': 'procedure_execution', 'match': 'procedure_execution_id'},
        {'field': 'intent_approval_role', 'target': 'communication_policies', 'return': 'approval_role', 'key': 'intent_policy', 'match': 'communication_policy_id'},
        {'field': 'approval_role_agent_kind', 'target': 'roles', 'return': 'current_agent_kind', 'key': 'intent_approval_role', 'match': 'role_id'},
        {'field': 'resulting_delivery_was_transmitted', 'target': 'message_deliveries', 'return': 'was_actually_transmitted', 'key': 'resulting_delivery', 'match': 'message_delivery_id'},
        {'field': 'resulting_delivery_exception', 'target': 'message_deliveries', 'return': 'invoked_exception', 'key': 'resulting_delivery', 'match': 'message_delivery_id'},
        {'field': 'alternate_attempt_was_cleared', 'target': 'send_intents', 'return': 'is_cleared_to_send', 'key': 'alternate_channel_intent', 'match': 'send_intent_id'},
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'retry_was_cleared', 'target': 'send_intents', 'return': 'is_cleared_to_send', 'key': 'retry_intent', 'match': 'send_intent_id'},
        {'field': 'enforced_by_unauthorized_agent', 'target': 'role_assignments', 'return': 'is_unauthorized_enforcement_agent', 'key': 'evaluating_role_assignment', 'match': 'role_assignment_id'},
        {'field': 'recipient_consent_status_raw', 'target': 'recipients', 'return': 'sms_consent_status', 'key': 'recipient', 'match': 'recipient_id'},],
     'aggregations': []},
    {'name': 'AgentDecisionRecords', 'file': 'agent_decision_records', 'rulebook_rows': 3,
     'compute': compute_agent_decision_records_fields,
     'fields': ['agent_decision_record_id', 'name', 'step_execution', 'deciding_agent', 'decision_kind', 'decision_summary', 'decided_at', 'materiality_band', 'human_disposition', 'reviewed_by_agent', 'reviewed_at', 'was_overridden', 'was_reviewed', 'deciding_agent_kind', 'deciding_agent_when_overridden', 'under_role_assignment', 'role_assignment_when_scored', 'role_assignment_when_overridden', 'step_of_decision', 'boundary_match_key', 'matching_boundary_count', 'violated_authority_boundary', 'reviewer_agent_kind', 'has_human_confirmation', 'needs_human_confirmation', 'is_unconfirmed_non_human_decision', 'step_execution_when_unconfirmed', 'agent_when_boundary_violated', 'review_latency_minutes', 'is_draft_kind', 'agent_when_draft_overridden', 'agent_when_draft', 'override_reason_kind', 'is_error_correction', 'is_reserved_judgment_override', 'override_reason_is_recorded', 'is_unexplained_override', 'error_correction_role_assignment_key', 'boundary_violation_role_assignment_key', 'semantic_type_iri'],
     'calculated': {'is_draft_kind', 'violated_authority_boundary', 'is_unexplained_override', 'agent_when_draft', 'name', 'role_assignment_when_overridden', 'needs_human_confirmation', 'role_assignment_when_scored', 'was_reviewed', 'boundary_match_key', 'has_human_confirmation', 'review_latency_minutes', 'boundary_violation_role_assignment_key', 'agent_when_draft_overridden', 'is_error_correction', 'was_overridden', 'is_unconfirmed_non_human_decision', 'agent_when_boundary_violated', 'is_reserved_judgment_override', 'deciding_agent_when_overridden', 'override_reason_is_recorded', 'step_execution_when_unconfirmed', 'error_correction_role_assignment_key'},
     'lookups': [
        {'field': 'deciding_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'deciding_agent', 'match': 'agent_id'},
        {'field': 'step_of_decision', 'target': 'step_executions', 'return': 'step', 'key': 'step_execution', 'match': 'step_execution_id'},
        {'field': 'reviewer_agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'reviewed_by_agent', 'match': 'agent_id'},],
     'aggregations': [
        {'field': 'matching_boundary_count', 'op': 'COUNTIFS', 'table': 'authority_boundaries', 'criteria': [('boundary_match_key', 'field', 'boundary_match_key')]},]},
    {'name': 'DeliveredCommunications', 'file': 'delivered_communications', 'rulebook_rows': 1,
     'compute': compute_delivered_communications_fields,
     'fields': ['delivered_communication_id', 'name', 'procedure_execution', 'sending_step_execution', 'authorizing_step_execution', 'message_template', 'channel', 'recipient_key', 'sent_at', 'rendered_content_hash', 'approved_content_hash', 'delivery_status', 'semantic_type_iri', 'has_authorization', 'content_matches_approval', 'authorized_at', 'was_approved_before_sending', 'is_defensible'],
     'calculated': {'has_authorization', 'is_defensible', 'name', 'content_matches_approval', 'was_approved_before_sending'},
     'lookups': [
        {'field': 'authorized_at', 'target': 'step_executions', 'return': 'ended_at', 'key': 'authorizing_step_execution', 'match': 'step_execution_id'},],
     'aggregations': []},
    {'name': 'AuthorityBoundaries', 'file': 'authority_boundaries', 'rulebook_rows': 3,
     'compute': compute_authority_boundaries_fields,
     'fields': ['authority_boundary_id', 'name', 'step', 'forbidden_agent_kind', 'forbidden_decision_kind', 'ratified_by_knowledge_fragment', 'enforcing_requirement', 'authority_role', 'valid_from', 'valid_to', 'status', 'evaluation_context', 'as_of_instant', 'is_currently_binding', 'ratifying_fragment_is_valid', 'step_when_binding', 'boundary_match_key', 'violation_count', 'is_untested', 'has_ratifying_fragment', 'is_unwarranted', 'ratifying_fragment_is_overdue', 'ratifying_fragment_is_single_witness', 'warrant_is_thin', 'is_unwarranted_and_untested', 'unwarranted_boundary_step_key', 'ratifying_fragment_key', 'ratifying_fragment_status', 'ratification_lapsed', 'binds_despite_lapsed_ratification', 'is_ungrounded_and_untested', 'constrained_role_assignment_key', 'semantic_type_iri'],
     'calculated': {'ratifying_fragment_key', 'constrained_role_assignment_key', 'is_ungrounded_and_untested', 'unwarranted_boundary_step_key', 'has_ratifying_fragment', 'warrant_is_thin', 'is_untested', 'boundary_match_key', 'is_unwarranted', 'binds_despite_lapsed_ratification', 'is_unwarranted_and_untested', 'name', 'step_when_binding', 'ratification_lapsed', 'is_currently_binding'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'ratifying_fragment_is_valid', 'target': 'knowledge_fragments', 'return': 'is_currently_valid', 'key': 'ratified_by_knowledge_fragment', 'match': 'knowledge_fragment_id'},
        {'field': 'ratifying_fragment_is_overdue', 'target': 'knowledge_fragments', 'return': 'is_overdue_for_review', 'key': 'ratified_by_knowledge_fragment', 'match': 'knowledge_fragment_id'},
        {'field': 'ratifying_fragment_is_single_witness', 'target': 'knowledge_fragments', 'return': 'is_from_single_witness', 'key': 'ratified_by_knowledge_fragment', 'match': 'knowledge_fragment_id'},
        {'field': 'ratifying_fragment_status', 'target': 'knowledge_fragments', 'return': 'status', 'key': 'ratified_by_knowledge_fragment', 'match': 'knowledge_fragment_id'},],
     'aggregations': [
        {'field': 'violation_count', 'op': 'COUNTIFS', 'table': 'agent_decision_records', 'criteria': [('boundary_match_key', 'field', 'boundary_match_key')]},]},
    {'name': 'BindingObservations', 'file': 'binding_observations', 'rulebook_rows': 0,
     'compute': compute_binding_observations_fields,
     'fields': ['binding_observation_id', 'name', 'step_execution', 'operational_binding', 'observed_source_timestamp', 'read_at', 'sla_minutes_at_run', 'age_at_run_minutes', 'was_stale_at_run', 'is_authoritative_binding', 'stale_at_run_step_key'],
     'calculated': {'was_stale_at_run', 'stale_at_run_step_key', 'age_at_run_minutes', 'name'},
     'lookups': [
        {'field': 'sla_minutes_at_run', 'target': 'operational_bindings', 'return': 'freshness_sla_minutes', 'key': 'operational_binding', 'match': 'operational_binding_id'},
        {'field': 'is_authoritative_binding', 'target': 'operational_bindings', 'return': 'is_authoritative', 'key': 'operational_binding', 'match': 'operational_binding_id'},],
     'aggregations': []},
    {'name': 'Attestations', 'file': 'attestations', 'rulebook_rows': 0,
     'compute': compute_attestations_fields,
     'fields': ['attestation_id', 'name', 'procedure_execution', 'signed_by_agent', 'signed_at', 'assurance_grade_at_signing', 'version_was_fit_at_signing', 'version_is_fit_now', 'fitness_verdict_has_drifted', 'assurance_grade_now', 'assurance_grade_has_drifted', 'would_not_survive_restatement'],
     'calculated': {'fitness_verdict_has_drifted', 'would_not_survive_restatement', 'name', 'assurance_grade_has_drifted'},
     'lookups': [
        {'field': 'version_is_fit_now', 'target': 'procedure_executions', 'return': 'executed_version_is_fit', 'key': 'procedure_execution', 'match': 'procedure_execution_id'},
        {'field': 'assurance_grade_now', 'target': 'procedure_executions', 'return': 'assurance_grade', 'key': 'procedure_execution', 'match': 'procedure_execution_id'},],
     'aggregations': []},
    {'name': 'AppRoleProfiles', 'file': 'app_role_profiles', 'rulebook_rows': 12,
     'compute': compute_app_role_profiles_fields,
     'fields': ['app_role_profile_id', 'name', 'role', 'display_label', 'role_kind', 'accent_color', 'icon_mark', 'icon_png_base64', 'pitch', 'sort_order', 'route_count', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'route_count', 'op': 'COUNTIFS', 'table': 'app_routes', 'criteria': [('owning_role', 'field', 'role')]},]},
    {'name': 'AppNavGroups', 'file': 'app_nav_groups', 'rulebook_rows': 23,
     'compute': compute_app_nav_groups_fields,
     'fields': ['app_nav_group_id', 'name', 'group_label', 'route_count', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'route_count', 'op': 'COUNTIFS', 'table': 'app_routes', 'criteria': [('nav_group', 'field', 'app_nav_group_id')]},]},
    {'name': 'AppRoutes', 'file': 'app_routes', 'rulebook_rows': 149,
     'compute': compute_app_routes_fields,
     'fields': ['app_route_id', 'name', 'route_path', 'route_name', 'surface', 'owning_role', 'nav_group', 'nav_order', 'route_kind', 'purpose', 'layout_hints', 'is_in_nav', 'is_shared', 'is_maintainer', 'question_count', 'reference_count', 'answers_no_question', 'semantic_type_iri'],
     'calculated': {'is_shared', 'answers_no_question', 'is_maintainer', 'is_in_nav', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'question_count', 'op': 'COUNTIFS', 'table': 'app_route_questions', 'criteria': [('route', 'field', 'app_route_id')]},
        {'field': 'reference_count', 'op': 'COUNTIFS', 'table': 'app_route_references', 'criteria': [('from_route', 'field', 'app_route_id')]},]},
    {'name': 'AppRouteQuestions', 'file': 'app_route_questions', 'rulebook_rows': 151,
     'compute': compute_app_route_questions_fields,
     'fields': ['app_route_question_id', 'name', 'route', 'question', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'AppRouteReferences', 'file': 'app_route_references', 'rulebook_rows': 315,
     'compute': compute_app_route_references_fields,
     'fields': ['app_route_reference_id', 'name', 'from_route', 'to_route', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'RulebookTables', 'file': 'rulebook_tables', 'rulebook_rows': 86,
     'compute': compute_rulebook_tables_fields,
     'fields': ['rulebook_table_id', 'table_name', 'name', 'physical_table', 'physical_view', 'subject_area', 'is_extension', 'field_count', 'policy_count', 'is_unsecured', 'semantic_type_iri'],
     'calculated': {'is_unsecured', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'field_count', 'op': 'COUNTIFS', 'table': 'rulebook_fields', 'criteria': [('target_table', 'field', 'rulebook_table_id')]},
        {'field': 'policy_count', 'op': 'COUNTIFS', 'table': 'access_policies', 'criteria': [('target_table', 'field', 'rulebook_table_id')]},]},
    {'name': 'AccessPrincipals', 'file': 'access_principals', 'rulebook_rows': 12,
     'compute': compute_access_principals_fields,
     'fields': ['access_principal_id', 'name', 'label', 'domain_role', 'pg_role_name', 'schema_name', 'is_administrator', 'organization_scope', 'role_label', 'policy_count', 'grant_count', 'visible_table_count', 'has_no_access', 'is_over_privileged', 'semantic_type_iri'],
     'calculated': {'is_over_privileged', 'name', 'has_no_access'},
     'lookups': [
        {'field': 'organization_scope', 'target': 'roles', 'return': 'organization', 'key': 'domain_role', 'match': 'role_id'},
        {'field': 'role_label', 'target': 'roles', 'return': 'label', 'key': 'domain_role', 'match': 'role_id'},],
     'aggregations': [
        {'field': 'policy_count', 'op': 'COUNTIFS', 'table': 'access_policies', 'criteria': [('principal', 'field', 'access_principal_id')]},
        {'field': 'grant_count', 'op': 'COUNTIFS', 'table': 'field_grants', 'criteria': [('principal', 'field', 'access_principal_id')]},
        {'field': 'visible_table_count', 'op': 'COUNTIFS', 'table': 'role_schema_views', 'criteria': [('principal', 'field', 'access_principal_id')]},]},
    {'name': 'AccessPolicies', 'file': 'access_policies', 'rulebook_rows': 202,
     'compute': compute_access_policies_fields,
     'fields': ['access_policy_id', 'name', 'principal', 'target_table', 'command', 'row_predicate', 'check_predicate', 'rationale', 'references_inference', 'is_write_command', 'is_unrestricted', 'principal_is_admin', 'is_unrestricted_non_admin_grant', 'is_unwitnessed_write', 'denial_test_count', 'semantic_type_iri'],
     'calculated': {'is_unwitnessed_write', 'is_write_command', 'is_unrestricted_non_admin_grant', 'name', 'is_unrestricted'},
     'lookups': [
        {'field': 'principal_is_admin', 'target': 'access_principals', 'return': 'is_administrator', 'key': 'principal', 'match': 'access_principal_id'},],
     'aggregations': [
        {'field': 'denial_test_count', 'op': 'COUNTIFS', 'table': 'access_denial_tests', 'criteria': [('target_policy', 'field', 'access_policy_id')]},]},
    {'name': 'FieldGrants', 'file': 'field_grants', 'rulebook_rows': 3639,
     'compute': compute_field_grants_fields,
     'fields': ['field_grant_id', 'name', 'principal', 'target_field', 'can_read', 'can_write', 'mask_strategy', 'field_table', 'field_name', 'field_is_derived', 'is_writable_derived_field', 'is_masked', 'grant_key_when_readable', 'semantic_type_iri'],
     'calculated': {'is_writable_derived_field', 'grant_key_when_readable', 'is_masked', 'name'},
     'lookups': [
        {'field': 'field_table', 'target': 'rulebook_fields', 'return': 'target_table', 'key': 'target_field', 'match': 'rulebook_field_id'},
        {'field': 'field_name', 'target': 'rulebook_fields', 'return': 'field_name', 'key': 'target_field', 'match': 'rulebook_field_id'},
        {'field': 'field_is_derived', 'target': 'rulebook_fields', 'return': 'is_derived', 'key': 'target_field', 'match': 'rulebook_field_id'},],
     'aggregations': []},
    {'name': 'RoleSchemas', 'file': 'role_schemas', 'rulebook_rows': 12,
     'compute': compute_role_schemas_fields,
     'fields': ['role_schema_id', 'name', 'principal', 'schema_name', 'search_path', 'is_sealed', 'view_count', 'is_empty_schema', 'semantic_type_iri'],
     'calculated': {'search_path', 'is_empty_schema', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'view_count', 'op': 'COUNTIFS', 'table': 'role_schema_views', 'criteria': [('role_schema', 'field', 'role_schema_id')]},]},
    {'name': 'RoleSchemaViews', 'file': 'role_schema_views', 'rulebook_rows': 202,
     'compute': compute_role_schema_views_fields,
     'fields': ['role_schema_view_id', 'name', 'role_schema', 'principal', 'target_table', 'view_name', 'schema_name', 'source_view', 'grant_key', 'column_count', 'table_field_count', 'is_full_width', 'is_degenerate_view', 'semantic_type_iri'],
     'calculated': {'grant_key', 'is_full_width', 'is_degenerate_view', 'name'},
     'lookups': [
        {'field': 'schema_name', 'target': 'role_schemas', 'return': 'schema_name', 'key': 'role_schema', 'match': 'role_schema_id'},
        {'field': 'source_view', 'target': 'rulebook_tables', 'return': 'physical_view', 'key': 'target_table', 'match': 'rulebook_table_id'},
        {'field': 'table_field_count', 'target': 'rulebook_tables', 'return': 'field_count', 'key': 'target_table', 'match': 'rulebook_table_id'},],
     'aggregations': [
        {'field': 'column_count', 'op': 'COUNTIFS', 'table': 'field_grants', 'criteria': [('grant_key_when_readable', 'field', 'grant_key')]},]},
    {'name': 'JwtClaimMappings', 'file': 'jwt_claim_mappings', 'rulebook_rows': 4,
     'compute': compute_jwt_claim_mappings_fields,
     'fields': ['jwt_claim_mapping_id', 'name', 'claim_name', 'sql_accessor', 'is_reserved_claim', 'maps_to_principal', 'description2', 'usage_count', 'semantic_type_iri'],
     'calculated': {'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'usage_count', 'op': 'COUNTIFS', 'table': 'access_policies', 'criteria': [('row_predicate', 'field', 'sql_accessor')]},]},
    {'name': 'AccessDenialTests', 'file': 'access_denial_tests', 'rulebook_rows': 17,
     'compute': compute_access_denial_tests_fields,
     'fields': ['access_denial_test_id', 'name', 'target_policy', 'principal', 'target_table', 'forbidden_row_id', 'expected_visible', 'observed_visible', 'last_run_at', 'has_run', 'is_passing', 'is_leak', 'is_unproven', 'rationale', 'is_positive_control', 'forbidden_table', 'forbidden_column', 'semantic_type_iri'],
     'calculated': {'is_passing', 'has_run', 'is_unproven', 'is_positive_control', 'name', 'is_leak'},
     'lookups': [],
     'aggregations': []},
    {'name': 'AppUsers', 'file': 'app_users', 'rulebook_rows': 10,
     'compute': compute_app_users_fields,
     'fields': ['app_user_id', 'name', 'email_address', 'display_name', 'linked_agent', 'is_enabled', 'agent_kind', 'organization', 'assignment_count', 'has_no_principal', 'holds_multiple_principals', 'is_non_human_sign_in', 'semantic_type_iri'],
     'calculated': {'holds_multiple_principals', 'is_non_human_sign_in', 'has_no_principal', 'name'},
     'lookups': [
        {'field': 'agent_kind', 'target': 'agents', 'return': 'agent_kind', 'key': 'linked_agent', 'match': 'agent_id'},
        {'field': 'organization', 'target': 'agents', 'return': 'organization', 'key': 'linked_agent', 'match': 'agent_id'},],
     'aggregations': [
        {'field': 'assignment_count', 'op': 'COUNTIFS', 'table': 'principal_assignments', 'criteria': [('app_user', 'field', 'app_user_id')]},]},
    {'name': 'PrincipalAssignments', 'file': 'principal_assignments', 'rulebook_rows': 12,
     'compute': compute_principal_assignments_fields,
     'fields': ['principal_assignment_id', 'name', 'app_user', 'principal', 'is_default', 'granted_rationale', 'principal_is_admin', 'user_organization', 'principal_organization', 'is_cross_organization_grant', 'semantic_type_iri'],
     'calculated': {'is_cross_organization_grant', 'name'},
     'lookups': [
        {'field': 'principal_is_admin', 'target': 'access_principals', 'return': 'is_administrator', 'key': 'principal', 'match': 'access_principal_id'},
        {'field': 'user_organization', 'target': 'app_users', 'return': 'organization', 'key': 'app_user', 'match': 'app_user_id'},
        {'field': 'principal_organization', 'target': 'access_principals', 'return': 'organization_scope', 'key': 'principal', 'match': 'access_principal_id'},],
     'aggregations': []},
    {'name': 'IssuedTokens', 'file': 'issued_tokens', 'rulebook_rows': 0,
     'compute': compute_issued_tokens_fields,
     'fields': ['issued_token_id', 'name', 'app_user', 'principal', 'issued_at', 'expires_at', 'issuer', 'subject_claim', 'claims_snapshot', 'is_dev_minted', 'semantic_type_iri'],
     'calculated': {'is_dev_minted', 'name'},
     'lookups': [],
     'aggregations': []},
    {'name': 'ProcessMiningRuns', 'file': 'process_mining_runs', 'rulebook_rows': 4,
     'compute': compute_process_mining_runs_fields,
     'fields': ['process_mining_run_id', 'name', 'procedure_version', 'event_log_source', 'mined_at', 'discovered_variant_count', 'conforming_variant_count', 'deviation_description', 'evaluation_context', 'as_of_instant', 'conformance_rate', 'is_conformant', 'has_major_drift_from_documentation', 'days_since_mined', 'is_stale_mining_evidence', 'procedure_version_is_live', 'is_drift_on_live_version', 'drifted_mining_run_key', 'semantic_type_iri'],
     'calculated': {'conformance_rate', 'has_major_drift_from_documentation', 'is_conformant', 'is_drift_on_live_version', 'days_since_mined', 'name', 'is_stale_mining_evidence', 'drifted_mining_run_key'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'procedure_version_is_live', 'target': 'procedure_versions', 'return': 'is_live', 'key': 'procedure_version', 'match': 'procedure_version_id'},],
     'aggregations': []},
    {'name': 'Vocabularies', 'file': 'vocabularies', 'rulebook_rows': 2,
     'compute': compute_vocabularies_fields,
     'fields': ['vocabulary_id', 'name', 'title', 'scheme_uri', 'governing_role', 'term_count', 'orphan_term_count', 'has_orphan_terms', 'semantic_type_iri'],
     'calculated': {'has_orphan_terms', 'name'},
     'lookups': [],
     'aggregations': [
        {'field': 'term_count', 'op': 'COUNTIFS', 'table': 'vocabulary_terms', 'criteria': [('vocabulary', 'field', 'vocabulary_id')]},
        {'field': 'orphan_term_count', 'op': 'COUNTIFS', 'table': 'vocabulary_terms', 'criteria': [('orphan_term_vocabulary_key', 'field', 'vocabulary_id')]},]},
    {'name': 'VocabularyTerms', 'file': 'vocabulary_terms', 'rulebook_rows': 12,
     'compute': compute_vocabulary_terms_fields,
     'fields': ['vocabulary_term_id', 'name', 'vocabulary', 'pref_label', 'alt_labels', 'definition', 'usage_count', 'is_orphan_term', 'is_widely_adopted_term', 'orphan_term_vocabulary_key', 'semantic_type_iri'],
     'calculated': {'orphan_term_vocabulary_key', 'is_orphan_term', 'name', 'is_widely_adopted_term'},
     'lookups': [],
     'aggregations': [
        {'field': 'usage_count', 'op': 'COUNTIFS', 'table': 'requirements', 'criteria': [('controlled_term', 'field', 'vocabulary_term_id')]},]},
    {'name': 'KnowledgeBrokerLinks', 'file': 'knowledge_broker_links', 'rulebook_rows': 5,
     'compute': compute_knowledge_broker_links_fields,
     'fields': ['knowledge_broker_link_id', 'name', 'seeker', 'broker', 'topic', 'frequency', 'last_consulted_at', 'evaluation_context', 'as_of_instant', 'days_since_consulted', 'is_active_reliance', 'broker_is_still_engaged', 'is_at_risk_reliance', 'active_reliance_broker_key', 'at_risk_broker_key', 'semantic_type_iri'],
     'calculated': {'days_since_consulted', 'at_risk_broker_key', 'active_reliance_broker_key', 'is_at_risk_reliance', 'name', 'is_active_reliance'},
     'lookups': [
        {'field': 'as_of_instant', 'target': 'evaluation_contexts', 'return': 'as_of_instant', 'key': 'evaluation_context', 'match': 'evaluation_context_id'},
        {'field': 'broker_is_still_engaged', 'target': 'agents', 'return': 'is_still_engaged', 'key': 'broker', 'match': 'agent_id'},],
     'aggregations': []},
]

# ERB_CLOSURES materializes each vw_<entity>_closure view aggregations read.
ERB_CLOSURES = [
]
