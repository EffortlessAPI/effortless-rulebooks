// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/pko-native-procedural-knowledge-rulebook-rulebook.json
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
// RULEBOOKRELEASES TABLE
// Version ledger for the canonical ERB-PKO rulebook itself. This is distinct from PKO Procedure versioning.
// =============================================================================

/** A row in the RulebookReleases table. */
export interface RulebookReleasesRow {
  /** Stored logical identifier for one RulebookReleases row. */
  rulebook_release_id: string;
  /** Human-readable calculated display alias for the RulebookReleases row. */
  name: string | null;
  /** Semantic version of this canonical rulebook release. */
  rulebook_version: string | null;
  /** Version of the ERB-PKO profile schema. */
  profile_version: string | null;
  /** Repository-relative path to the ERB-PKO JSON profile schema. */
  profile_schema_path: string | null;
  /** Exact version IRI of the PKO core ontology used by this release. */
  pko_core_version_iri: string | null;
  /** Exact version IRI of the PKO industry module used by this release. */
  pko_industry_version_iri: string | null;
  /** Timestamp at which this rulebook release was issued. */
  issued_at: string | null;
  /** Release status such as Draft, Candidate, or Published. */
  status: string | null;
  /** Human-readable semantic changelog for this release. */
  changelog: string | null;
  /** TRUE only for the current rulebook release. */
  is_current: boolean | null;
  _erb_errors?: Record<string, string>;
}

const rulebookReleasesFieldTypes: Record<string, FieldType> = {
  rulebook_release_id: "string",
  name: "*string",
  rulebook_version: "*string",
  profile_version: "*string",
  profile_schema_path: "*string",
  pko_core_version_iri: "*string",
  pko_industry_version_iri: "*string",
  issued_at: "*string",
  status: "*string",
  changelog: "*string",
  is_current: "*bool",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the RulebookReleases row.
 *  Formula: ={{RulebookVersion}} & " / PKO " & {{PkoCoreVersionIri}} */
export function calcRulebookReleasesName(tc: RulebookReleasesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.rulebook_version)), vS(" / PKO "), erbTextOr(vStr(tc.pko_core_version_iri))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRulebookReleases(tc: RulebookReleasesRow): RulebookReleasesRow {
  // Level 1
  calcGuard(tc, rulebookReleasesFieldTypes, "name", () => { tc.name = calcRulebookReleasesName(tc); });
  return tc;
}

/** Reads RulebookReleases rows from a JSON array file. */
export function loadRulebookReleasesRows(file: string): RulebookReleasesRow[] {
  return loadRows(file, { fields: rulebookReleasesFieldTypes }) as unknown as RulebookReleasesRow[];
}

// =============================================================================
// ONTOLOGYPROFILES TABLE
// Versioned ontology and vocabulary dependencies. PKO mappings always identify the exact profile and version.
// =============================================================================

/** A row in the OntologyProfiles table. */
export interface OntologyProfilesRow {
  /** Stored logical identifier for one OntologyProfiles row. */
  ontology_profile_id: string;
  /** Human-readable calculated display alias for the OntologyProfiles row. */
  name: string | null;
  /** Human-readable ontology or vocabulary name. */
  label: string | null;
  /** Referenced release or specification version. */
  version: string | null;
  /** Exact version IRI or normative specification URI. */
  version_iri: string | null;
  /** Namespace used for class/property IRIs. */
  namespace_iri: string | null;
  /** License or standards body attribution. */
  license: string | null;
  /** How the profile is used in this rulebook. */
  scope: string | null;
  _erb_errors?: Record<string, string>;
}

const ontologyProfilesFieldTypes: Record<string, FieldType> = {
  ontology_profile_id: "string",
  name: "*string",
  label: "*string",
  version: "*string",
  version_iri: "*string",
  namespace_iri: "*string",
  license: "*string",
  scope: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the OntologyProfiles row.
 *  Formula: ={{Label}} & " " & {{Version}} */
export function calcOntologyProfilesName(tc: OntologyProfilesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.label)), vS(" "), erbTextOr(vStr(tc.version))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeOntologyProfiles(tc: OntologyProfilesRow): OntologyProfilesRow {
  // Level 1
  calcGuard(tc, ontologyProfilesFieldTypes, "name", () => { tc.name = calcOntologyProfilesName(tc); });
  return tc;
}

/** Reads OntologyProfiles rows from a JSON array file. */
export function loadOntologyProfilesRows(file: string): OntologyProfilesRow[] {
  return loadRows(file, { fields: ontologyProfilesFieldTypes }) as unknown as OntologyProfilesRow[];
}

// =============================================================================
// EVALUATIONCONTEXTS TABLE
// The instant this rulebook's time-dependent witnesses are evaluated against. Modeled as data rather than wall-clock so every freshness, overdue, and validity answer is reproducible and auditable: asking the same question tomorrow yields the same answer. Exactly one row carries IsCurrent.
// =============================================================================

/** A row in the EvaluationContexts table. */
export interface EvaluationContextsRow {
  /** Stored logical identifier for one EvaluationContexts row. */
  evaluation_context_id: string;
  /** Human-readable calculated display alias for the EvaluationContexts row. */
  name: string | null;
  /** What this evaluation instant represents. */
  label: string | null;
  /** The instant all time-dependent witnesses are evaluated against. */
  as_of_instant: string;
  /** TRUE for the single active evaluation context. */
  is_current: boolean | null;
  /** Why this instant was chosen. */
  rationale: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const evaluationContextsFieldTypes: Record<string, FieldType> = {
  evaluation_context_id: "string",
  name: "*string",
  label: "*string",
  as_of_instant: "string",
  is_current: "*bool",
  rationale: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the EvaluationContexts row.
 *  Formula: ={{Label}} & " @ " & {{AsOfInstant}} */
export function calcEvaluationContextsName(tc: EvaluationContextsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.label)), vS(" @ "), erbTimestamptzText(vStrPlain(tc.as_of_instant))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeEvaluationContexts(tc: EvaluationContextsRow): EvaluationContextsRow {
  // Level 1
  calcGuard(tc, evaluationContextsFieldTypes, "name", () => { tc.name = calcEvaluationContextsName(tc); });
  return tc;
}

/** Reads EvaluationContexts rows from a JSON array file. */
export function loadEvaluationContextsRows(file: string): EvaluationContextsRow[] {
  return loadRows(file, { fields: evaluationContextsFieldTypes }) as unknown as EvaluationContextsRow[];
}

// =============================================================================
// ORGANIZATIONS TABLE
// Organizations that own, adopt, govern, or execute procedures. Maps to prov:Organization.
// =============================================================================

/** A row in the Organizations table. */
export interface OrganizationsRow {
  /** Stored logical identifier for one Organizations row. */
  organization_id: string;
  /** Human-readable calculated display alias for the Organizations row. */
  name: string | null;
  /** Organization's human-readable name. */
  display_name: string | null;
  /** Type such as Company, Department, Vendor, or Regulator. */
  organization_type: string | null;
  /** Identifier in an operational directory or master-data system. */
  external_identifier: string | null;
  /** Exact RDF class IRI for the organization. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const organizationsFieldTypes: Record<string, FieldType> = {
  organization_id: "string",
  name: "*string",
  display_name: "*string",
  organization_type: "*string",
  external_identifier: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Organizations row.
 *  Formula: ={{DisplayName}} */
export function calcOrganizationsName(tc: OrganizationsRow): string | null {
  return toStringPtr(vStr(tc.display_name));
}

/** Computes every calculated field of the row in dependency order. */
export function computeOrganizations(tc: OrganizationsRow): OrganizationsRow {
  // Level 1
  calcGuard(tc, organizationsFieldTypes, "name", () => { tc.name = calcOrganizationsName(tc); });
  return tc;
}

/** Reads Organizations rows from a JSON array file. */
export function loadOrganizationsRows(file: string): OrganizationsRow[] {
  return loadRows(file, { fields: organizationsFieldTypes }) as unknown as OrganizationsRow[];
}

// =============================================================================
// AGENTS TABLE
// Human and software agents that create, modify, approve, or execute procedural knowledge. Maps to prov:Agent.
// =============================================================================

/** A row in the Agents table. */
export interface AgentsRow {
  /** Stored logical identifier for one Agents row. */
  agent_id: string;
  /** Human-readable calculated display alias for the Agents row. */
  name: string | null;
  /** Agent's human-readable name. */
  display_name: string | null;
  /** Human, AIAgent, AutomatedPipeline, Organization, or another explicit agent category. */
  agent_kind: string | null;
  /** Organization to which the agent belongs. */
  organization: string | null;
  /** Contact address when applicable. */
  contact_address: string | null;
  /** Model version, pipeline release, or employment assignment key. */
  version_or_employment_key: string | null;
  /** How many role assignments this agent currently holds. Counts only CURRENT assignments via the CurrentAgentKey echo — counting all assignments would report a departed agent as engaged forever. */
  count_of_current_role_assignments: number | null;
  /** TRUE when this agent currently holds at least one role in the organization. */
  is_still_engaged: boolean | null;
  /** Total decisions this agent has recorded. */
  decision_count: number | null;
  /** Decisions by this agent that a human corrected or reversed. */
  overridden_decision_count: number | null;
  /** Percentage of this agent's decisions that were overridden by a human. */
  override_rate_percent: number | null;
  /** TRUE when this agent is an AI agent or an automated pipeline. */
  is_non_human: boolean | null;
  /** Number of decisions by this agent that violated an authority boundary. */
  boundary_violation_count: number | null;
  /** TRUE when this agent has made at least one decision an authority boundary forbids. */
  is_operating_outside_boundary: boolean | null;
  /** Number of drafting decisions this agent has made. */
  draft_decision_count: number | null;
  /** Number of this agent's drafting decisions a human corrected or reversed. */
  overridden_draft_count: number | null;
  /** Percentage of this agent's drafting output that a human rewrote. */
  draft_rewrite_rate_percent: number | null;
  /** How many active informal-reliance links name this agent as the one actually consulted. */
  times_named_as_broker: number | null;
  /** TRUE when at least three people actively rely on this agent as an informal knowledge broker. */
  is_recognized_broker: boolean | null;
  /** How many active informal-reliance links name this agent while this agent holds no current role. */
  at_risk_reliance_count: number | null;
  /** TRUE when people are actively relying on this agent even though they have already left every role. */
  has_at_risk_knowledge_reliance: boolean | null;
  /** Exact semantic type IRI used when projecting the agent. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const agentsFieldTypes: Record<string, FieldType> = {
  agent_id: "string",
  name: "*string",
  display_name: "*string",
  agent_kind: "*string",
  organization: "*string",
  contact_address: "*string",
  version_or_employment_key: "*string",
  count_of_current_role_assignments: "*int",
  is_still_engaged: "*bool",
  decision_count: "*float64",
  overridden_decision_count: "*float64",
  override_rate_percent: "*float64",
  is_non_human: "*bool",
  boundary_violation_count: "*float64",
  is_operating_outside_boundary: "*bool",
  draft_decision_count: "*float64",
  overridden_draft_count: "*float64",
  draft_rewrite_rate_percent: "*float64",
  times_named_as_broker: "*float64",
  is_recognized_broker: "*bool",
  at_risk_reliance_count: "*float64",
  has_at_risk_knowledge_reliance: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Agents row.
 *  Formula: ={{DisplayName}} */
export function calcAgentsName(tc: AgentsRow): string | null {
  return toStringPtr(vStr(tc.display_name));
}

/** Computes the IsStillEngaged calculated field.
 *  TRUE when this agent currently holds at least one role in the organization.
 *  Formula: ={{CountOfCurrentRoleAssignments}} > 0 */
export function calcAgentsIsStillEngaged(tc: AgentsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_current_role_assignments), ">", vI(0)));
}

/** Computes the OverrideRatePercent calculated field.
 *  Percentage of this agent's decisions that were overridden by a human.
 *  Formula: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}) */
export function calcAgentsOverrideRatePercent(tc: AgentsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbEq(vNum(tc.decision_count), vI(0))), () => vI(0), () => erbDiv(erbMul(vNum(tc.overridden_decision_count), vI(100)), vNum(tc.decision_count))));
}

/** Computes the IsNonHuman calculated field.
 *  TRUE when this agent is an AI agent or an automated pipeline.
 *  Formula: =NOT({{AgentKind}} = "Human") */
export function calcAgentsIsNonHuman(tc: AgentsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(erbNullif(vStr(tc.agent_kind)), vS("Human")))));
}

/** Computes the IsOperatingOutsideBoundary calculated field.
 *  TRUE when this agent has made at least one decision an authority boundary forbids.
 *  Formula: ={{BoundaryViolationCount}} > 0 */
export function calcAgentsIsOperatingOutsideBoundary(tc: AgentsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.boundary_violation_count), ">", vI(0)));
}

/** Computes the DraftRewriteRatePercent calculated field.
 *  Percentage of this agent's drafting output that a human rewrote.
 *  Formula: =IF({{DraftDecisionCount}} = 0, 0, ({{OverriddenDraftCount}} * 100) / {{DraftDecisionCount}}) */
export function calcAgentsDraftRewriteRatePercent(tc: AgentsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbEq(vNum(tc.draft_decision_count), vI(0))), () => vI(0), () => erbDiv(erbMul(vNum(tc.overridden_draft_count), vI(100)), vNum(tc.draft_decision_count))));
}

/** Computes the IsRecognizedBroker calculated field.
 *  TRUE when at least three people actively rely on this agent as an informal knowledge broker.
 *  Formula: ={{TimesNamedAsBroker}} >= 3 */
export function calcAgentsIsRecognizedBroker(tc: AgentsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.times_named_as_broker), ">=", vI(3)));
}

/** Computes the HasAtRiskKnowledgeReliance calculated field.
 *  TRUE when people are actively relying on this agent even though they have already left every role.
 *  Formula: ={{AtRiskRelianceCount}} > 0 */
export function calcAgentsHasAtRiskKnowledgeReliance(tc: AgentsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.at_risk_reliance_count), ">", vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAgents(tc: AgentsRow): AgentsRow {
  // Level 1
  calcGuard(tc, agentsFieldTypes, "name", () => { tc.name = calcAgentsName(tc); });
  calcGuard(tc, agentsFieldTypes, "is_still_engaged", () => { tc.is_still_engaged = calcAgentsIsStillEngaged(tc); });
  calcGuard(tc, agentsFieldTypes, "override_rate_percent", () => { tc.override_rate_percent = calcAgentsOverrideRatePercent(tc); });
  calcGuard(tc, agentsFieldTypes, "is_non_human", () => { tc.is_non_human = calcAgentsIsNonHuman(tc); });
  calcGuard(tc, agentsFieldTypes, "is_operating_outside_boundary", () => { tc.is_operating_outside_boundary = calcAgentsIsOperatingOutsideBoundary(tc); });
  calcGuard(tc, agentsFieldTypes, "draft_rewrite_rate_percent", () => { tc.draft_rewrite_rate_percent = calcAgentsDraftRewriteRatePercent(tc); });
  calcGuard(tc, agentsFieldTypes, "is_recognized_broker", () => { tc.is_recognized_broker = calcAgentsIsRecognizedBroker(tc); });
  calcGuard(tc, agentsFieldTypes, "has_at_risk_knowledge_reliance", () => { tc.has_at_risk_knowledge_reliance = calcAgentsHasAtRiskKnowledgeReliance(tc); });
  return tc;
}

/** Reads Agents rows from a JSON array file. */
export function loadAgentsRows(file: string): AgentsRow[] {
  return loadRows(file, { fields: agentsFieldTypes }) as unknown as AgentsRow[];
}

// =============================================================================
// ROLES TABLE
// Stable organizational functions separated from the agents that currently fill them. Maps to pro:Role.
// =============================================================================

/** A row in the Roles table. */
export interface RolesRow {
  /** Stored logical identifier for one Roles row. */
  role_id: string;
  /** Human-readable calculated display alias for the Roles row. */
  name: string | null;
  /** Human-readable role label. */
  label: string | null;
  /** Organization that owns the role. */
  organization: string | null;
  /** Current agent filling the role; history is retained in RoleAssignments. */
  current_agent: string | null;
  /** Agent category of the current role filler. */
  current_agent_kind: string | null;
  /** Accountability and responsibility assigned to the role. */
  responsibility: string | null;
  /** Total number of assignment rows ever recorded against this role. */
  active_assignment_count: number | null;
  /** Number of assignment rows for this role that are in force right now. */
  currently_covered_assignment_count: number | null;
  /** TRUE when no assignment currently covers this role — the role is uncovered. */
  has_no_current_holder: boolean | null;
  /** How many change requests name this role as the deciding authority. */
  count_of_awaited_decisions: number | null;
  /** The RoleAssignments id of the assignment by which this role is currently held. Deliberately a raw identifier, not a relationship: RoleAssignments already points at Roles, so an FK back would make the two mutually dependent and this rulebook must stay acyclic. */
  current_assignment: string | null;
  /** Start of the currently-in-force assignment for this role. */
  current_assignment_valid_from: string | null;
  /** TRUE when the role's current agent is an AI agent or automated pipeline. */
  is_non_human_held: boolean | null;
  /** TRUE when a role is pointed at a non-human agent but has no assignment row granting it. */
  is_ungoverned_non_human_role: boolean | null;
  /** How many assignments to this role have ended. */
  departed_assignment_count: number | null;
  /** Whether anyone has ever departed this role. */
  has_lost_a_holder: boolean | null;
  /** A role somebody departed and that nobody currently covers. */
  is_vacated_role: boolean | null;
  /** How many boundaries constrain this role on lapsed authority. */
  ungrounded_boundary_count: number | null;
  /** TRUE when at least one constraint on this role rests on knowledge that is no longer valid. */
  is_governed_by_lapsed_authority: boolean | null;
  /** How many refusals this role should have been told about and was not. */
  unescalated_refusal_count: number | null;
  /** How many current assignments of this role are unauthorized enforcement assignments. */
  unauthorized_enforcement_assignment_count: number | null;
  /** TRUE when a role that enforces controls on others is held with no recorded authorization. */
  is_ungoverned_enforcement_role: boolean | null;
  /** Exact semantic type IRI for the role. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const rolesFieldTypes: Record<string, FieldType> = {
  role_id: "string",
  name: "*string",
  label: "*string",
  organization: "*string",
  current_agent: "*string",
  current_agent_kind: "*string",
  responsibility: "*string",
  active_assignment_count: "*float64",
  currently_covered_assignment_count: "*float64",
  has_no_current_holder: "*bool",
  count_of_awaited_decisions: "*int",
  current_assignment: "*string",
  current_assignment_valid_from: "*string",
  is_non_human_held: "*bool",
  is_ungoverned_non_human_role: "*bool",
  departed_assignment_count: "*float64",
  has_lost_a_holder: "*bool",
  is_vacated_role: "*bool",
  ungrounded_boundary_count: "*float64",
  is_governed_by_lapsed_authority: "*bool",
  unescalated_refusal_count: "*float64",
  unauthorized_enforcement_assignment_count: "*float64",
  is_ungoverned_enforcement_role: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Roles row.
 *  Formula: ={{Label}} */
export function calcRolesName(tc: RolesRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes the HasNoCurrentHolder calculated field.
 *  TRUE when no assignment currently covers this role — the role is uncovered.
 *  Formula: ={{CurrentlyCoveredAssignmentCount}} = 0 */
export function calcRolesHasNoCurrentHolder(tc: RolesRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.currently_covered_assignment_count), vI(0)));
}

/** Computes the IsNonHumanHeld calculated field.
 *  TRUE when the role's current agent is an AI agent or automated pipeline.
 *  Formula: =NOT({{CurrentAgentKind}} = "Human") */
export function calcRolesIsNonHumanHeld(tc: RolesRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(vStr(tc.current_agent_kind), vS("Human")))));
}

/** Computes the IsUngovernedNonHumanRole calculated field.
 *  TRUE when a role is pointed at a non-human agent but has no assignment row granting it.
 *  Formula: =AND({{IsNonHumanHeld}}, {{HasNoCurrentHolder}}) */
export function calcRolesIsUngovernedNonHumanRole(tc: RolesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_non_human_held)), erbBool3(vBool(tc.has_no_current_holder))));
}

/** Computes the HasLostAHolder calculated field.
 *  Whether anyone has ever departed this role.
 *  Formula: ={{DepartedAssignmentCount}} > 0 */
export function calcRolesHasLostAHolder(tc: RolesRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.departed_assignment_count), ">", vI(0)));
}

/** Computes the IsVacatedRole calculated field.
 *  A role somebody departed and that nobody currently covers.
 *  Formula: =AND({{HasLostAHolder}}, {{HasNoCurrentHolder}}) */
export function calcRolesIsVacatedRole(tc: RolesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_lost_a_holder)), erbBool3(vBool(tc.has_no_current_holder))));
}

/** Computes the IsGovernedByLapsedAuthority calculated field.
 *  TRUE when at least one constraint on this role rests on knowledge that is no longer valid.
 *  Formula: =({{UngroundedBoundaryCount}} > 0) */
export function calcRolesIsGovernedByLapsedAuthority(tc: RolesRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.ungrounded_boundary_count), ">", vI(0)));
}

/** Computes the IsUngovernedEnforcementRole calculated field.
 *  TRUE when a role that enforces controls on others is held with no recorded authorization.
 *  Formula: =({{UnauthorizedEnforcementAssignmentCount}} > 0) */
export function calcRolesIsUngovernedEnforcementRole(tc: RolesRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.unauthorized_enforcement_assignment_count), ">", vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoles(tc: RolesRow): RolesRow {
  // Level 1
  calcGuard(tc, rolesFieldTypes, "name", () => { tc.name = calcRolesName(tc); });
  calcGuard(tc, rolesFieldTypes, "has_no_current_holder", () => { tc.has_no_current_holder = calcRolesHasNoCurrentHolder(tc); });
  calcGuard(tc, rolesFieldTypes, "is_non_human_held", () => { tc.is_non_human_held = calcRolesIsNonHumanHeld(tc); });
  calcGuard(tc, rolesFieldTypes, "has_lost_a_holder", () => { tc.has_lost_a_holder = calcRolesHasLostAHolder(tc); });
  calcGuard(tc, rolesFieldTypes, "is_governed_by_lapsed_authority", () => { tc.is_governed_by_lapsed_authority = calcRolesIsGovernedByLapsedAuthority(tc); });
  calcGuard(tc, rolesFieldTypes, "is_ungoverned_enforcement_role", () => { tc.is_ungoverned_enforcement_role = calcRolesIsUngovernedEnforcementRole(tc); });
  // Level 2
  calcGuard(tc, rolesFieldTypes, "is_ungoverned_non_human_role", () => { tc.is_ungoverned_non_human_role = calcRolesIsUngovernedNonHumanRole(tc); });
  calcGuard(tc, rolesFieldTypes, "is_vacated_role", () => { tc.is_vacated_role = calcRolesIsVacatedRole(tc); });
  return tc;
}

/** Reads Roles rows from a JSON array file. */
export function loadRolesRows(file: string): RolesRow[] {
  return loadRows(file, { fields: rolesFieldTypes }) as unknown as RolesRow[];
}

// =============================================================================
// ROLEASSIGNMENTS TABLE
// Time-bounded records of agents holding roles. Maps to pro:RoleInTime and preserves assignment history instead of overwriting it.
// =============================================================================

/** A row in the RoleAssignments table. */
export interface RoleAssignmentsRow {
  /** Stored logical identifier for one RoleAssignments row. */
  role_assignment_id: string;
  /** Human-readable calculated display alias for the RoleAssignments row. */
  name: string | null;
  /** Role held during the assignment. */
  role: string | null;
  /** Agent holding the role. */
  agent: string | null;
  /** Start of the assignment's valid-time interval. */
  valid_from: string | null;
  /** End of the assignment's valid-time interval; null means open-ended. */
  valid_to: string | null;
  /** Rationale for the assignment or reassignment. */
  reason: string | null;
  /** Active, Superseded, Planned, or Revoked. */
  status: string | null;
  /** The evaluation context this assignment's currency is judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this assignment's currency is judged against. */
  as_of_instant: string | null;
  /** TRUE when the assignment is valid now. */
  is_current: boolean | null;
  /** Echoes the Agent id only while this assignment is current; empty otherwise. Lets a parent count CURRENT assignments with a single-criterion COUNTIFS, which is the only shape this transpiler translates correctly. */
  current_agent_key: string | null;
  /** TRUE when this role assignment is active and has not lapsed. */
  is_currently_valid: boolean | null;
  /** Composite agent+role key, emitted only for currently-valid assignments. */
  agent_role_key: string | null;
  /** TRUE when this role assignment has ended — the agent no longer holds the role. */
  has_departed: boolean | null;
  /** TRUE when this assignment is both status-Active and inside its valid-time window right now. */
  covers_now: boolean | null;
  /** Echoes the role id when this assignment is currently in force, blank otherwise. */
  role_when_covering: string | null;
  /** Whether the agent holding this assignment is Human, AIAgent, or AutomatedPipeline. */
  agent_kind: string | null;
  /** TRUE when this assignment places a non-human agent into the role. */
  is_non_human_assignment: boolean | null;
  /** The assignment this one replaced, if any. */
  supersedes_assignment: string | null;
  /** Agent kind of the assignment this one superseded. */
  predecessor_agent_kind: string | null;
  /** TRUE when this assignment handed a role from a human to a non-human agent. */
  is_human_to_non_human_handover: boolean | null;
  /** Role that approved this assignment. */
  approving_authority_role: string | null;
  /** Change request under which this assignment was approved. */
  authorizing_change_request: string | null;
  /** TRUE when a non-human agent holds this role with no approving authority recorded at all. An authority named without a change request is still an authority; the separate WasAuthorizedByChangeRequest column carries that weaker distinction. */
  is_unauthorized_non_human_assignment: boolean | null;
  /** TRUE when this assignment's authorization is traceable to a change request, not merely to a named role. The stronger form of authorization, kept separate so the weaker one is not silently reported as unauthorized. */
  was_authorized_by_change_request: boolean | null;
  /** Decisions made under this assignment. */
  decision_count: number | null;
  /** Decisions made under this assignment that a human corrected or reversed. */
  overridden_decision_count: number | null;
  /** Percentage of decisions under this assignment that a human overrode. */
  override_rate_percent: number | null;
  /** Override rate of the assignment this one superseded. */
  predecessor_override_rate_percent: number | null;
  /** TRUE when this assignment is overridden by humans more often than the assignment it replaced. */
  quality_regressed_vs_predecessor: boolean | null;
  /** Composite-key echo: the role this assignment covered when the assignment has ended, blank otherwise. */
  departed_role_key: string | null;
  /** The number of decisions below which an override-rate comparison is not considered evidentially meaningful for this role. */
  minimum_decisions_for_comparison: number | null;
  /** How many decisions the superseded assignment produced. */
  predecessor_decision_count: number | null;
  /** TRUE when this assignment has produced enough decisions for its override rate to mean anything. */
  has_sufficient_sample: boolean | null;
  /** TRUE when the predecessor assignment produced enough decisions to compare against. */
  predecessor_has_sufficient_sample: boolean | null;
  /** TRUE when both sides of the override-rate comparison rest on adequate samples. */
  comparison_is_evidentially_sound: boolean | null;
  /** How many percentage points one additional override would move this assignment's rate. The fragility of the number. */
  single_override_swing_percent: number | null;
  /** TRUE when a quality verdict is being reported for this assignment on a sample too small to support it. */
  quality_verdict_is_unsupported: boolean | null;
  /** TRUE when a human-to-machine handover is operating without a statistically meaningful quality comparison behind it. */
  is_unmeasured_automation_handover: boolean | null;
  /** How many of this assignment's decisions were overridden because they were wrong. */
  error_correction_count: number | null;
  /** Percentage of this assignment's decisions overridden as errors -- the override rate with reserved-judgment overrides removed. */
  error_rate_percent: number | null;
  /** When the approving authority actually granted this assignment. Empty when no authorization was ever recorded. */
  authorization_decided_at: string | null;
  /** When the authorization for this assignment was last re-examined. */
  authorization_reviewed_at: string | null;
  /** How often this assignment's authorization is promised to be re-examined. */
  authorization_review_cadence_days: number | null;
  /** TRUE when this assignment carries both a named approving authority and the date they granted it. */
  has_dated_authorization: boolean | null;
  /** How long since this assignment's authorization was last re-examined. */
  days_since_authorization_review: number | null;
  /** TRUE when the promised re-examination interval has elapsed without a review. */
  authorization_is_overdue_for_review: boolean | null;
  /** TRUE when a currently-active non-human assignment has been running past its authorization review date. */
  is_standing_unreviewed_automation: boolean | null;
  /** TRUE when a human-to-machine handover was authorized with no promised review cadence at all -- granted once, permanently. */
  is_unconditioned_automation_handover: boolean | null;
  /** The error rate at or above which this assignment must stop making decisions unaided. */
  max_tolerable_error_rate_percent: number | null;
  /** TRUE when this assignment's error rate has reached the threshold set when it was authorized. */
  exceeds_tolerable_error_rate: boolean | null;
  /** How many decisions under this assignment violated an authority boundary. */
  boundary_violation_count_for_assignment: number | null;
  /** TRUE when any decision under this assignment crossed a boundary it was forbidden to cross. */
  has_any_boundary_violation: boolean | null;
  /** TRUE when at least one boundary constraining this assignment's role rests on lapsed knowledge. */
  has_ungrounded_governing_boundary: boolean | null;
  /** TRUE when any pre-declared condition requiring this assignment to stop deciding unaided has been met. */
  suspension_condition_met: boolean | null;
  /** TRUE when a suspension condition has been met and the assignment is nonetheless still active and still deciding. */
  is_operating_under_met_suspension_condition: boolean | null;
  /** TRUE when this assignment has any pre-declared condition under which it must stop at all. */
  has_declared_suspension_condition: boolean | null;
  /** TRUE when a role is recorded as having approved this assignment. */
  has_approving_authority: boolean | null;
  /** TRUE when a change request is recorded as the governance vehicle for this assignment. */
  has_authorizing_change_request: boolean | null;
  /** Whether this role's function is to enforce controls on other agents' actions. */
  is_enforcement_role: boolean | null;
  /** TRUE when a non-human agent enforces controls on others while holding no recorded authorization of its own. */
  is_unauthorized_enforcement_agent: boolean | null;
  /** How many independent governance artifacts back this assignment: an approving role, an authorizing change request. */
  governance_evidence_count: number | null;
  /** Echoes the role only for non-human assignments nobody authorized; empty otherwise. */
  unauthorized_enforcement_role_key: string | null;
  /** Exact class IRI for the assignment event. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const roleAssignmentsFieldTypes: Record<string, FieldType> = {
  role_assignment_id: "string",
  name: "*string",
  role: "*string",
  agent: "*string",
  valid_from: "*string",
  valid_to: "*string",
  reason: "*string",
  status: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  is_current: "*bool",
  current_agent_key: "*string",
  is_currently_valid: "*bool",
  agent_role_key: "*string",
  has_departed: "*bool",
  covers_now: "*bool",
  role_when_covering: "*string",
  agent_kind: "*string",
  is_non_human_assignment: "*bool",
  supersedes_assignment: "*string",
  predecessor_agent_kind: "*string",
  is_human_to_non_human_handover: "*bool",
  approving_authority_role: "*string",
  authorizing_change_request: "*string",
  is_unauthorized_non_human_assignment: "*bool",
  was_authorized_by_change_request: "*bool",
  decision_count: "*float64",
  overridden_decision_count: "*float64",
  override_rate_percent: "*float64",
  predecessor_override_rate_percent: "*float64",
  quality_regressed_vs_predecessor: "*bool",
  departed_role_key: "*string",
  minimum_decisions_for_comparison: "*int",
  predecessor_decision_count: "*float64",
  has_sufficient_sample: "*bool",
  predecessor_has_sufficient_sample: "*bool",
  comparison_is_evidentially_sound: "*bool",
  single_override_swing_percent: "*float64",
  quality_verdict_is_unsupported: "*bool",
  is_unmeasured_automation_handover: "*bool",
  error_correction_count: "*float64",
  error_rate_percent: "*float64",
  authorization_decided_at: "*string",
  authorization_reviewed_at: "*string",
  authorization_review_cadence_days: "*int",
  has_dated_authorization: "*bool",
  days_since_authorization_review: "*int",
  authorization_is_overdue_for_review: "*bool",
  is_standing_unreviewed_automation: "*bool",
  is_unconditioned_automation_handover: "*bool",
  max_tolerable_error_rate_percent: "*int",
  exceeds_tolerable_error_rate: "*bool",
  boundary_violation_count_for_assignment: "*float64",
  has_any_boundary_violation: "*bool",
  has_ungrounded_governing_boundary: "*bool",
  suspension_condition_met: "*bool",
  is_operating_under_met_suspension_condition: "*bool",
  has_declared_suspension_condition: "*bool",
  has_approving_authority: "*bool",
  has_authorizing_change_request: "*bool",
  is_enforcement_role: "*bool",
  is_unauthorized_enforcement_agent: "*bool",
  governance_evidence_count: "*int",
  unauthorized_enforcement_role_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the RoleAssignments row.
 *  Formula: ={{Role}} & " @ " & {{ValidFrom}} */
export function calcRoleAssignmentsName(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.role)), vS(" @ "), erbTimestamptzText(vStr(tc.valid_from))));
}

/** Computes the IsCurrent calculated field.
 *  TRUE when the assignment is valid now.
 *  Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})) */
export function calcRoleAssignmentsIsCurrent(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<=", vStr(tc.as_of_instant))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant)))))));
}

/** Computes the CurrentAgentKey calculated field.
 *  Echoes the Agent id only while this assignment is current; empty otherwise. Lets a parent count CURRENT assignments with a single-criterion COUNTIFS, which is the only shape this transpiler translates correctly.
 *  Formula: =IF({{IsCurrent}}, {{Agent}}, "") */
export function calcRoleAssignmentsCurrentAgentKey(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_current)), () => vStr(tc.agent), () => vS("")));
}

/** Computes the IsCurrentlyValid calculated field.
 *  TRUE when this role assignment is active and has not lapsed.
 *  Formula: =AND({{Status}} = "Active", OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})) */
export function calcRoleAssignmentsIsCurrentlyValid(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Active"))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant)))))));
}

/** Computes the AgentRoleKey calculated field.
 *  Composite agent+role key, emitted only for currently-valid assignments.
 *  Formula: =IF({{IsCurrentlyValid}}, {{Agent}} & "|" & {{Role}}, "") */
export function calcRoleAssignmentsAgentRoleKey(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_currently_valid)), () => erbConcat(erbTextOr(vStr(tc.agent)), vS("|"), erbTextOr(vStr(tc.role))), () => vS("")));
}

/** Computes the HasDeparted calculated field.
 *  TRUE when this role assignment has ended — the agent no longer holds the role.
 *  Formula: =AND({{ValidTo}} <> "", {{ValidTo}} <= {{AsOfInstant}}) */
export function calcRoleAssignmentsHasDeparted(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), "<=", vStr(tc.as_of_instant)))));
}

/** Computes the CoversNow calculated field.
 *  TRUE when this assignment is both status-Active and inside its valid-time window right now.
 *  Formula: =AND({{Status}} = "Active", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})) */
export function calcRoleAssignmentsCoversNow(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Active"))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<=", vStr(tc.as_of_instant))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant)))))));
}

/** Computes the RoleWhenCovering calculated field.
 *  Echoes the role id when this assignment is currently in force, blank otherwise.
 *  Formula: =IF({{CoversNow}}, {{Role}}, "") */
export function calcRoleAssignmentsRoleWhenCovering(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.covers_now)), () => vStr(tc.role), () => vS("")));
}

/** Computes the IsNonHumanAssignment calculated field.
 *  TRUE when this assignment places a non-human agent into the role.
 *  Formula: =NOT({{AgentKind}} = "Human") */
export function calcRoleAssignmentsIsNonHumanAssignment(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(vStr(tc.agent_kind), vS("Human")))));
}

/** Computes the IsHumanToNonHumanHandover calculated field.
 *  TRUE when this assignment handed a role from a human to a non-human agent.
 *  Formula: =AND({{PredecessorAgentKind}} = "Human", {{IsNonHumanAssignment}}) */
export function calcRoleAssignmentsIsHumanToNonHumanHandover(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(vStr(tc.predecessor_agent_kind), vS("Human"))), erbBool3(vBool(tc.is_non_human_assignment))));
}

/** Computes the IsUnauthorizedNonHumanAssignment calculated field.
 *  TRUE when a non-human agent holds this role with no approving authority recorded at all. An authority named without a change request is still an authority; the separate WasAuthorizedByChangeRequest column carries that weaker distinction.
 *  Formula: =AND({{IsNonHumanAssignment}}, NOT({{HasApprovingAuthority}})) */
export function calcRoleAssignmentsIsUnauthorizedNonHumanAssignment(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_non_human_assignment)), erbBool3(erbNot(erbBool3(vBool(tc.has_approving_authority))))));
}

/** Computes the WasAuthorizedByChangeRequest calculated field.
 *  TRUE when this assignment's authorization is traceable to a change request, not merely to a named role. The stronger form of authorization, kept separate so the weaker one is not silently reported as unauthorized.
 *  Formula: =AND({{HasApprovingAuthority}}, {{AuthorizingChangeRequest}} <> "") */
export function calcRoleAssignmentsWasAuthorizedByChangeRequest(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_approving_authority)), erbBool3(erbIsNotBlank(vStr(tc.authorizing_change_request)))));
}

/** Computes the OverrideRatePercent calculated field.
 *  Percentage of decisions under this assignment that a human overrode.
 *  Formula: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}) */
export function calcRoleAssignmentsOverrideRatePercent(tc: RoleAssignmentsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbEq(vNum(tc.decision_count), vI(0))), () => vI(0), () => erbDiv(erbMul(vNum(tc.overridden_decision_count), vI(100)), vNum(tc.decision_count))));
}

/** Computes the QualityRegressedVsPredecessor calculated field.
 *  TRUE when this assignment is overridden by humans more often than the assignment it replaced.
 *  Formula: =AND({{SupersedesAssignment}} <> "", {{OverrideRatePercent}} > {{PredecessorOverrideRatePercent}}) */
export function calcRoleAssignmentsQualityRegressedVsPredecessor(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.supersedes_assignment))), erbBool3(erbCmp(vNum(tc.override_rate_percent), ">", vNum(tc.predecessor_override_rate_percent)))));
}

/** Computes the DepartedRoleKey calculated field.
 *  Composite-key echo: the role this assignment covered when the assignment has ended, blank otherwise.
 *  Formula: =IF({{HasDeparted}}, {{Role}}, "") */
export function calcRoleAssignmentsDepartedRoleKey(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.has_departed)), () => vStr(tc.role), () => vS("")));
}

/** Computes the HasSufficientSample calculated field.
 *  TRUE when this assignment has produced enough decisions for its override rate to mean anything.
 *  Formula: ={{DecisionCount}} >= {{MinimumDecisionsForComparison}} */
export function calcRoleAssignmentsHasSufficientSample(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.decision_count), ">=", erbNullif(vInt(tc.minimum_decisions_for_comparison))));
}

/** Computes the PredecessorHasSufficientSample calculated field.
 *  TRUE when the predecessor assignment produced enough decisions to compare against.
 *  Formula: ={{PredecessorDecisionCount}} >= {{MinimumDecisionsForComparison}} */
export function calcRoleAssignmentsPredecessorHasSufficientSample(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.predecessor_decision_count), ">=", erbNullif(vInt(tc.minimum_decisions_for_comparison))));
}

/** Computes the ComparisonIsEvidentiallySound calculated field.
 *  TRUE when both sides of the override-rate comparison rest on adequate samples.
 *  Formula: =AND({{HasSufficientSample}}, {{PredecessorHasSufficientSample}}) */
export function calcRoleAssignmentsComparisonIsEvidentiallySound(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_sufficient_sample)), erbBool3(vBool(tc.predecessor_has_sufficient_sample))));
}

/** Computes the SingleOverrideSwingPercent calculated field.
 *  How many percentage points one additional override would move this assignment's rate. The fragility of the number.
 *  Formula: =IF({{DecisionCount}} > 0, 100 / {{DecisionCount}}, 0) */
export function calcRoleAssignmentsSingleOverrideSwingPercent(tc: RoleAssignmentsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbCmp(vNum(tc.decision_count), ">", vI(0))), () => erbDiv(vI(100), vNum(tc.decision_count)), () => vI(0)));
}

/** Computes the QualityVerdictIsUnsupported calculated field.
 *  TRUE when a quality verdict is being reported for this assignment on a sample too small to support it.
 *  Formula: =AND(NOT({{ComparisonIsEvidentiallySound}}), NOT({{QualityRegressedVsPredecessor}})) */
export function calcRoleAssignmentsQualityVerdictIsUnsupported(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.comparison_is_evidentially_sound)))), erbBool3(erbNot(erbBool3(vBool(tc.quality_regressed_vs_predecessor))))));
}

/** Computes the IsUnmeasuredAutomationHandover calculated field.
 *  TRUE when a human-to-machine handover is operating without a statistically meaningful quality comparison behind it.
 *  Formula: =AND({{IsHumanToNonHumanHandover}}, NOT({{ComparisonIsEvidentiallySound}})) */
export function calcRoleAssignmentsIsUnmeasuredAutomationHandover(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_human_to_non_human_handover)), erbBool3(erbNot(erbBool3(vBool(tc.comparison_is_evidentially_sound))))));
}

/** Computes the ErrorRatePercent calculated field.
 *  Percentage of this assignment's decisions overridden as errors -- the override rate with reserved-judgment overrides removed.
 *  Formula: =IF({{DecisionCount}} > 0, {{ErrorCorrectionCount}} * 100 / {{DecisionCount}}, 0) */
export function calcRoleAssignmentsErrorRatePercent(tc: RoleAssignmentsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbCmp(vNum(tc.decision_count), ">", vI(0))), () => erbDiv(erbMul(vNum(tc.error_correction_count), vI(100)), vNum(tc.decision_count)), () => vI(0)));
}

/** Computes the HasDatedAuthorization calculated field.
 *  TRUE when this assignment carries both a named approving authority and the date they granted it.
 *  Formula: =AND({{ApprovingAuthorityRole}} <> "", {{AuthorizationDecidedAt}} <> "") */
export function calcRoleAssignmentsHasDatedAuthorization(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.approving_authority_role))), erbBool3(erbIsNotBlank(vStr(tc.authorization_decided_at)))));
}

/** Computes the DaysSinceAuthorizationReview calculated field.
 *  How long since this assignment's authorization was last re-examined.
 *  Formula: =IF({{AuthorizationReviewedAt}} <> "", DATETIME_DIFF({{AsOfInstant}}, {{AuthorizationReviewedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days")) */
export function calcRoleAssignmentsDaysSinceAuthorizationReview(tc: RoleAssignmentsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbIsNotBlank(vStr(tc.authorization_reviewed_at))), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.authorization_reviewed_at), vS("days")), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.valid_from), vS("days")))));
}

/** Computes the AuthorizationIsOverdueForReview calculated field.
 *  TRUE when the promised re-examination interval has elapsed without a review.
 *  Formula: =AND({{AuthorizationReviewCadenceDays}} > 0, {{DaysSinceAuthorizationReview}} > {{AuthorizationReviewCadenceDays}}) */
export function calcRoleAssignmentsAuthorizationIsOverdueForReview(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vInt(tc.authorization_review_cadence_days)), ">", vI(0))), erbBool3(erbCmp(vInt(tc.days_since_authorization_review), ">", erbNullif(vInt(tc.authorization_review_cadence_days))))));
}

/** Computes the IsStandingUnreviewedAutomation calculated field.
 *  TRUE when a currently-active non-human assignment has been running past its authorization review date.
 *  Formula: =AND({{CoversNow}}, AND({{IsNonHumanAssignment}}, {{AuthorizationIsOverdueForReview}})) */
export function calcRoleAssignmentsIsStandingUnreviewedAutomation(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.covers_now)), erbBool3(erbAnd(erbBool3(vBool(tc.is_non_human_assignment)), erbBool3(vBool(tc.authorization_is_overdue_for_review))))));
}

/** Computes the IsUnconditionedAutomationHandover calculated field.
 *  TRUE when a human-to-machine handover was authorized with no promised review cadence at all -- granted once, permanently.
 *  Formula: =AND({{IsHumanToNonHumanHandover}}, {{AuthorizationReviewCadenceDays}} = 0) */
export function calcRoleAssignmentsIsUnconditionedAutomationHandover(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_human_to_non_human_handover)), erbBool3(erbEq(erbNullif(vInt(tc.authorization_review_cadence_days)), vI(0)))));
}

/** Computes the ExceedsTolerableErrorRate calculated field.
 *  TRUE when this assignment's error rate has reached the threshold set when it was authorized.
 *  Formula: =AND({{MaxTolerableErrorRatePercent}} > 0, {{ErrorRatePercent}} >= {{MaxTolerableErrorRatePercent}}) */
export function calcRoleAssignmentsExceedsTolerableErrorRate(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vInt(tc.max_tolerable_error_rate_percent)), ">", vI(0))), erbBool3(erbCmp(vNum(tc.error_rate_percent), ">=", erbNullif(vInt(tc.max_tolerable_error_rate_percent))))));
}

/** Computes the HasAnyBoundaryViolation calculated field.
 *  TRUE when any decision under this assignment crossed a boundary it was forbidden to cross.
 *  Formula: =({{BoundaryViolationCountForAssignment}} > 0) */
export function calcRoleAssignmentsHasAnyBoundaryViolation(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.boundary_violation_count_for_assignment), ">", vI(0)));
}

/** Computes the SuspensionConditionMet calculated field.
 *  TRUE when any pre-declared condition requiring this assignment to stop deciding unaided has been met.
 *  Formula: =OR({{ExceedsTolerableErrorRate}}, OR({{HasAnyBoundaryViolation}}, {{HasUngroundedGoverningBoundary}})) */
export function calcRoleAssignmentsSuspensionConditionMet(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(vBool(tc.exceeds_tolerable_error_rate)), erbBool3(erbOr(erbBool3(vBool(tc.has_any_boundary_violation)), erbBool3(vBool(tc.has_ungrounded_governing_boundary))))));
}

/** Computes the IsOperatingUnderMetSuspensionCondition calculated field.
 *  TRUE when a suspension condition has been met and the assignment is nonetheless still active and still deciding.
 *  Formula: =AND({{SuspensionConditionMet}}, AND({{CoversNow}}, {{IsNonHumanAssignment}})) */
export function calcRoleAssignmentsIsOperatingUnderMetSuspensionCondition(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.suspension_condition_met)), erbBool3(erbAnd(erbBool3(vBool(tc.covers_now)), erbBool3(vBool(tc.is_non_human_assignment))))));
}

/** Computes the HasDeclaredSuspensionCondition calculated field.
 *  TRUE when this assignment has any pre-declared condition under which it must stop at all.
 *  Formula: ={{MaxTolerableErrorRatePercent}} > 0 */
export function calcRoleAssignmentsHasDeclaredSuspensionCondition(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbCmp(erbNullif(vInt(tc.max_tolerable_error_rate_percent)), ">", vI(0)));
}

/** Computes the HasApprovingAuthority calculated field.
 *  TRUE when a role is recorded as having approved this assignment.
 *  Formula: ={{ApprovingAuthorityRole}} <> "" */
export function calcRoleAssignmentsHasApprovingAuthority(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.approving_authority_role)));
}

/** Computes the HasAuthorizingChangeRequest calculated field.
 *  TRUE when a change request is recorded as the governance vehicle for this assignment.
 *  Formula: ={{AuthorizingChangeRequest}} <> "" */
export function calcRoleAssignmentsHasAuthorizingChangeRequest(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.authorizing_change_request)));
}

/** Computes the IsUnauthorizedEnforcementAgent calculated field.
 *  TRUE when a non-human agent enforces controls on others while holding no recorded authorization of its own.
 *  Formula: =AND({{IsEnforcementRole}}, {{IsUnauthorizedNonHumanAssignment}}) */
export function calcRoleAssignmentsIsUnauthorizedEnforcementAgent(tc: RoleAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_enforcement_role)), erbBool3(vBool(tc.is_unauthorized_non_human_assignment))));
}

/** Computes the GovernanceEvidenceCount calculated field.
 *  How many independent governance artifacts back this assignment: an approving role, an authorizing change request.
 *  Formula: =IF({{HasApprovingAuthority}}, 1, 0) + IF({{HasAuthorizingChangeRequest}}, 1, 0) */
export function calcRoleAssignmentsGovernanceEvidenceCount(tc: RoleAssignmentsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbIf(erbBool3(vBool(tc.has_approving_authority)), () => vI(1), () => vI(0)), erbIf(erbBool3(vBool(tc.has_authorizing_change_request)), () => vI(1), () => vI(0)))));
}

/** Computes the UnauthorizedEnforcementRoleKey calculated field.
 *  Echoes the role only for non-human assignments nobody authorized; empty otherwise.
 *  Formula: =IF({{IsUnauthorizedNonHumanAssignment}}, {{Role}}, "") */
export function calcRoleAssignmentsUnauthorizedEnforcementRoleKey(tc: RoleAssignmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unauthorized_non_human_assignment)), () => vStr(tc.role), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoleAssignments(tc: RoleAssignmentsRow): RoleAssignmentsRow {
  // Level 1
  calcGuard(tc, roleAssignmentsFieldTypes, "name", () => { tc.name = calcRoleAssignmentsName(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_current", () => { tc.is_current = calcRoleAssignmentsIsCurrent(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_currently_valid", () => { tc.is_currently_valid = calcRoleAssignmentsIsCurrentlyValid(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_departed", () => { tc.has_departed = calcRoleAssignmentsHasDeparted(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "covers_now", () => { tc.covers_now = calcRoleAssignmentsCoversNow(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_non_human_assignment", () => { tc.is_non_human_assignment = calcRoleAssignmentsIsNonHumanAssignment(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "override_rate_percent", () => { tc.override_rate_percent = calcRoleAssignmentsOverrideRatePercent(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_sufficient_sample", () => { tc.has_sufficient_sample = calcRoleAssignmentsHasSufficientSample(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "predecessor_has_sufficient_sample", () => { tc.predecessor_has_sufficient_sample = calcRoleAssignmentsPredecessorHasSufficientSample(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "single_override_swing_percent", () => { tc.single_override_swing_percent = calcRoleAssignmentsSingleOverrideSwingPercent(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "error_rate_percent", () => { tc.error_rate_percent = calcRoleAssignmentsErrorRatePercent(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_dated_authorization", () => { tc.has_dated_authorization = calcRoleAssignmentsHasDatedAuthorization(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "days_since_authorization_review", () => { tc.days_since_authorization_review = calcRoleAssignmentsDaysSinceAuthorizationReview(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_any_boundary_violation", () => { tc.has_any_boundary_violation = calcRoleAssignmentsHasAnyBoundaryViolation(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_declared_suspension_condition", () => { tc.has_declared_suspension_condition = calcRoleAssignmentsHasDeclaredSuspensionCondition(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_approving_authority", () => { tc.has_approving_authority = calcRoleAssignmentsHasApprovingAuthority(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "has_authorizing_change_request", () => { tc.has_authorizing_change_request = calcRoleAssignmentsHasAuthorizingChangeRequest(tc); });
  // Level 2
  calcGuard(tc, roleAssignmentsFieldTypes, "current_agent_key", () => { tc.current_agent_key = calcRoleAssignmentsCurrentAgentKey(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "agent_role_key", () => { tc.agent_role_key = calcRoleAssignmentsAgentRoleKey(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "role_when_covering", () => { tc.role_when_covering = calcRoleAssignmentsRoleWhenCovering(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_human_to_non_human_handover", () => { tc.is_human_to_non_human_handover = calcRoleAssignmentsIsHumanToNonHumanHandover(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_unauthorized_non_human_assignment", () => { tc.is_unauthorized_non_human_assignment = calcRoleAssignmentsIsUnauthorizedNonHumanAssignment(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "was_authorized_by_change_request", () => { tc.was_authorized_by_change_request = calcRoleAssignmentsWasAuthorizedByChangeRequest(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "quality_regressed_vs_predecessor", () => { tc.quality_regressed_vs_predecessor = calcRoleAssignmentsQualityRegressedVsPredecessor(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "departed_role_key", () => { tc.departed_role_key = calcRoleAssignmentsDepartedRoleKey(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "comparison_is_evidentially_sound", () => { tc.comparison_is_evidentially_sound = calcRoleAssignmentsComparisonIsEvidentiallySound(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "authorization_is_overdue_for_review", () => { tc.authorization_is_overdue_for_review = calcRoleAssignmentsAuthorizationIsOverdueForReview(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "exceeds_tolerable_error_rate", () => { tc.exceeds_tolerable_error_rate = calcRoleAssignmentsExceedsTolerableErrorRate(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "governance_evidence_count", () => { tc.governance_evidence_count = calcRoleAssignmentsGovernanceEvidenceCount(tc); });
  // Level 3
  calcGuard(tc, roleAssignmentsFieldTypes, "quality_verdict_is_unsupported", () => { tc.quality_verdict_is_unsupported = calcRoleAssignmentsQualityVerdictIsUnsupported(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_unmeasured_automation_handover", () => { tc.is_unmeasured_automation_handover = calcRoleAssignmentsIsUnmeasuredAutomationHandover(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_standing_unreviewed_automation", () => { tc.is_standing_unreviewed_automation = calcRoleAssignmentsIsStandingUnreviewedAutomation(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_unconditioned_automation_handover", () => { tc.is_unconditioned_automation_handover = calcRoleAssignmentsIsUnconditionedAutomationHandover(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "suspension_condition_met", () => { tc.suspension_condition_met = calcRoleAssignmentsSuspensionConditionMet(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "is_unauthorized_enforcement_agent", () => { tc.is_unauthorized_enforcement_agent = calcRoleAssignmentsIsUnauthorizedEnforcementAgent(tc); });
  calcGuard(tc, roleAssignmentsFieldTypes, "unauthorized_enforcement_role_key", () => { tc.unauthorized_enforcement_role_key = calcRoleAssignmentsUnauthorizedEnforcementRoleKey(tc); });
  // Level 4
  calcGuard(tc, roleAssignmentsFieldTypes, "is_operating_under_met_suspension_condition", () => { tc.is_operating_under_met_suspension_condition = calcRoleAssignmentsIsOperatingUnderMetSuspensionCondition(tc); });
  return tc;
}

/** Reads RoleAssignments rows from a JSON array file. */
export function loadRoleAssignmentsRows(file: string): RoleAssignmentsRow[] {
  return loadRows(file, { fields: roleAssignmentsFieldTypes }) as unknown as RoleAssignmentsRow[];
}

// =============================================================================
// COMMUNITIESOFPRACTICE TABLE
// Socio-technical communities that transmit and maintain procedural knowledge. Explicit ERB-PKO extension.
// =============================================================================

/** A row in the CommunitiesOfPractice table. */
export interface CommunitiesOfPracticeRow {
  /** Stored logical identifier for one CommunitiesOfPractice row. */
  community_of_practice_id: string;
  /** Human-readable calculated display alias for the CommunitiesOfPractice row. */
  name: string | null;
  /** Community's human-readable name. */
  label: string | null;
  /** Organization hosting the community. */
  organization: string | null;
  /** Role accountable for convening and maintaining the community. */
  steward_role: string | null;
  /** Shared practice and knowledge-transfer purpose. */
  purpose: string | null;
  /** Meeting or practice cadence. */
  cadence: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const communitiesOfPracticeFieldTypes: Record<string, FieldType> = {
  community_of_practice_id: "string",
  name: "*string",
  label: "*string",
  organization: "*string",
  steward_role: "*string",
  purpose: "*string",
  cadence: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the CommunitiesOfPractice row.
 *  Formula: ={{Label}} */
export function calcCommunitiesOfPracticeName(tc: CommunitiesOfPracticeRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeCommunitiesOfPractice(tc: CommunitiesOfPracticeRow): CommunitiesOfPracticeRow {
  // Level 1
  calcGuard(tc, communitiesOfPracticeFieldTypes, "name", () => { tc.name = calcCommunitiesOfPracticeName(tc); });
  return tc;
}

/** Reads CommunitiesOfPractice rows from a JSON array file. */
export function loadCommunitiesOfPracticeRows(file: string): CommunitiesOfPracticeRow[] {
  return loadRows(file, { fields: communitiesOfPracticeFieldTypes }) as unknown as CommunitiesOfPracticeRow[];
}

// =============================================================================
// MENTORSHIPS TABLE
// Time-bounded apprenticeship relationships that intentionally transfer situated procedural knowledge. Explicit ERB-PKO extension.
// =============================================================================

/** A row in the Mentorships table. */
export interface MentorshipsRow {
  /** Stored logical identifier for one Mentorships row. */
  mentorship_id: string;
  /** Human-readable calculated display alias for the Mentorships row. */
  name: string | null;
  /** Community in which the mentorship operates. */
  community_of_practice: string | null;
  /** Experienced practitioner serving as mentor. */
  mentor_agent: string | null;
  /** Practitioner learning the procedure. */
  learner_agent: string | null;
  /** Mentorship start. */
  valid_from: string | null;
  /** Mentorship end. */
  valid_to: string | null;
  /** Procedural capability to be transferred. */
  learning_objective: string | null;
  /** Evidence showing completion or competence. */
  evidence_of_completion: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const mentorshipsFieldTypes: Record<string, FieldType> = {
  mentorship_id: "string",
  name: "*string",
  community_of_practice: "*string",
  mentor_agent: "*string",
  learner_agent: "*string",
  valid_from: "*string",
  valid_to: "*string",
  learning_objective: "*string",
  evidence_of_completion: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Mentorships row.
 *  Formula: ={{MentorAgent}} & " -> " & {{LearnerAgent}} */
export function calcMentorshipsName(tc: MentorshipsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.mentor_agent)), vS(" -> "), erbTextOr(vStr(tc.learner_agent))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeMentorships(tc: MentorshipsRow): MentorshipsRow {
  // Level 1
  calcGuard(tc, mentorshipsFieldTypes, "name", () => { tc.name = calcMentorshipsName(tc); });
  return tc;
}

/** Reads Mentorships rows from a JSON array file. */
export function loadMentorshipsRows(file: string): MentorshipsRow[] {
  return loadRows(file, { fields: mentorshipsFieldTypes }) as unknown as MentorshipsRow[];
}

// =============================================================================
// PROCEDURETYPES TABLE
// Controlled values used by pko:hasProcedureType.
// =============================================================================

/** A row in the ProcedureTypes table. */
export interface ProcedureTypesRow {
  /** Stored logical identifier for one ProcedureTypes row. */
  procedure_type_id: string;
  /** Human-readable calculated display alias for the ProcedureTypes row. */
  name: string | null;
  /** Procedure type label. */
  label: string | null;
  /** Definition and scope of the procedure type. */
  definition: string | null;
  /** Exact class IRI for procedure types. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const procedureTypesFieldTypes: Record<string, FieldType> = {
  procedure_type_id: "string",
  name: "*string",
  label: "*string",
  definition: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcedureTypes row.
 *  Formula: ={{Label}} */
export function calcProcedureTypesName(tc: ProcedureTypesRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedureTypes(tc: ProcedureTypesRow): ProcedureTypesRow {
  // Level 1
  calcGuard(tc, procedureTypesFieldTypes, "name", () => { tc.name = calcProcedureTypesName(tc); });
  return tc;
}

/** Reads ProcedureTypes rows from a JSON array file. */
export function loadProcedureTypesRows(file: string): ProcedureTypesRow[] {
  return loadRows(file, { fields: procedureTypesFieldTypes }) as unknown as ProcedureTypesRow[];
}

// =============================================================================
// PROCEDURES TABLE
// Abstract, discoverable procedures. Each version is represented separately in ProcedureVersions. Maps to pko:Procedure and dcat:Resource.
// =============================================================================

/** A row in the Procedures table. */
export interface ProceduresRow {
  /** Stored logical identifier for one Procedures row. */
  procedure_id: string;
  /** Human-readable calculated display alias for the Procedures row. */
  name: string | null;
  /** Human-readable title; maps to dcterms:title. */
  title: string | null;
  /** Procedure type; maps to pko:hasProcedureType. */
  procedure_type: string | null;
  /** Organization that owns the procedure. */
  owner_organization: string | null;
  /** Organization that adopts the procedure; maps to pko:isAdoptedBy. */
  adopted_by_organization: string | null;
  /** Desired outcome and scope of the procedure. */
  purpose: string | null;
  /** Thing or population on which the procedure acts; maps to pko:hasProcedureTarget. */
  target: string | null;
  /** TRUE when the procedure is a reusable template; maps to pko:isTemplate. */
  is_template: boolean | null;
  /** Current version identifier, mirrored structurally by ProcedureVersions.IsCurrent. */
  current_version_key: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const proceduresFieldTypes: Record<string, FieldType> = {
  procedure_id: "string",
  name: "*string",
  title: "*string",
  procedure_type: "*string",
  owner_organization: "*string",
  adopted_by_organization: "*string",
  purpose: "*string",
  target: "*string",
  is_template: "*bool",
  current_version_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Procedures row.
 *  Formula: ={{Title}} */
export function calcProceduresName(tc: ProceduresRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedures(tc: ProceduresRow): ProceduresRow {
  // Level 1
  calcGuard(tc, proceduresFieldTypes, "name", () => { tc.name = calcProceduresName(tc); });
  return tc;
}

/** Reads Procedures rows from a JSON array file. */
export function loadProceduresRows(file: string): ProceduresRow[] {
  return loadRows(file, { fields: proceduresFieldTypes }) as unknown as ProceduresRow[];
}

// =============================================================================
// PROCEDUREVERSIONS TABLE
// Versioned procedure specifications. Maps to pko:Procedure plus DCAT version relations and PKO versionNumber/newVersionMotivation/changelogDescription.
// =============================================================================

/** A row in the ProcedureVersions table. */
export interface ProcedureVersionsRow {
  /** Stored logical identifier for one ProcedureVersions row. */
  procedure_version_id: string;
  /** Human-readable calculated display alias for the ProcedureVersions row. */
  name: string | null;
  /** Abstract procedure of which this is a version. */
  procedure: string | null;
  /** Version number; maps to pko:versionNumber. */
  version_number: string | null;
  /** Version-specific title. */
  title: string | null;
  /** PKO procedure status individual: Draft, Validation, Approval, Approved, Deprecated, or Archived. */
  status: string | null;
  /** Issue timestamp; maps to dcterms:issued. */
  issued_at: string | null;
  /** Last semantic modification; maps to dcterms:modified. */
  modified_at: string | null;
  /** Agent that created the version; maps to dcterms:creator. */
  created_by_agent: string | null;
  /** Agent that last modified the version; maps to pko:wasModifiedBy. */
  modified_by_agent: string | null;
  /** Reason for the new version; maps to pko:newVersionMotivation. */
  new_version_motivation: string | null;
  /** Semantic change description; maps to pko:changelogDescription. */
  changelog_description: string | null;
  /** TRUE when this is the current version. */
  is_current: boolean | null;
  /** Number of steps in this version. */
  count_of_steps: number | null;
  /** Open knowledge gaps for this version. */
  count_of_open_knowledge_gaps: number | null;
  /** TRUE when approved, populated, and free of blocking knowledge gaps. */
  is_ready_for_execution: boolean | null;
  /** How many steps the specification defines for this version. */
  specified_step_count: number | null;
  /** How many reviews of this procedure version are past due. */
  overdue_review_count: number | null;
  /** How many change requests are open against this version. */
  open_change_request_count: number | null;
  /** How many high-severity knowledge gaps are open against this version. */
  open_high_severity_gap_count: number | null;
  /** TRUE when this version is approved, current on review, and carries no open change request or high-severity gap. */
  is_fit_to_execute: boolean | null;
  /** The review cadence promised by this version's stewardship assignment, in days. Summed because a version has at most one active steward; this is the hop that lets ReviewEvents resolve the promise via a primary-key match. */
  steward_review_cadence_days: number | null;
  /** How many stewardship assignments of any vintage point at this version. */
  count_of_stewardship_assignments: number | null;
  /** TRUE if any stewardship assignment has ever named this version. */
  has_any_steward: boolean | null;
  /** TRUE when this version is in a state where somebody could execute it. */
  is_live: boolean | null;
  /** TRUE when no stewardship assignment names this version. */
  is_unstewarded: boolean | null;
  /** TRUE when an executable version has nobody accountable for keeping it healthy. */
  is_live_and_unstewarded: boolean | null;
  /** How many open blocking gaps stand against this version. */
  count_of_open_blocking_gaps: number | null;
  /** TRUE when at least one open blocking gap stands against this version. */
  has_open_blocking_gap: boolean | null;
  /** TRUE when an executable version carries an unresolved blocking knowledge gap. */
  is_live_with_blocking_gap: boolean | null;
  /** TRUE when the model says this version is ready to execute while a blocking gap is open against it. */
  should_not_be_executable: boolean | null;
  /** How many unapproved claims this version is relying on. */
  count_of_unapproved_reliance_fragments: number | null;
  /** TRUE when this version depends on at least one claim the knowledge authority has not approved. */
  runs_on_unapproved_knowledge: boolean | null;
  /** How many acknowledged unknowns against this version have outlived their tolerance. */
  count_of_overdue_gaps: number | null;
  /** How many change requests have ever been raised against this version. */
  count_of_change_requests: number | null;
  /** How many review events have ever been recorded against this version. */
  count_of_review_events: number | null;
  /** TRUE when at least one change request or review event exists for this version. */
  has_governance_record: boolean | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** Days since this version's content was last changed. */
  days_since_modified: number | null;
  /** Days since the most recent review of this version. */
  days_since_last_review: number | null;
  /** TRUE when the version was edited more recently than it was reviewed. */
  was_modified_since_last_review: boolean | null;
  /** The kind of agent that last modified this version. */
  modifier_is_authority: string | null;
  /** TRUE when a live version's current content postdates every review it has had. */
  has_unwitnessed_change: boolean | null;
  /** How many supporting claims have outlived this version's review cadence. */
  count_of_stale_fragments: number | null;
  /** TRUE when this version rests on at least one claim older than its own review cadence. */
  knowledge_is_staler_than_cadence: boolean | null;
  /** How many compound-fragile knowledge fragments this version rests on. */
  compound_fragile_fragment_count: number | null;
  /** A live procedure version resting on at least one knowledge fragment that carries three or more decay signals. */
  rests_on_compound_fragile_knowledge: boolean | null;
  /** How many concentrated single-witness sessions underwrite this version's knowledge base. */
  concentrated_witness_session_count: number | null;
  /** A live version where at least one single-witness session alone underwrites three or more of its live claims. */
  knowledge_base_is_concentrated: boolean | null;
  /** How many unapproved claims this version feeds directly to software-assigned steps. */
  machine_consumed_unapproved_count: number | null;
  /** A live version that hands unapproved knowledge to a step no human is positioned to review. */
  feeds_unapproved_knowledge_to_machines: boolean | null;
  /** How many of this version's claims are overdue by actual review record rather than by inference. */
  genuinely_overdue_fragment_count: number | null;
  /** How many decisions are pending against this live version. */
  awaited_decision_count: number | null;
  /** How many open blocking gaps belong to THIS version, correctly scoped. */
  scoped_open_blocking_gap_count: number | null;
  /** A live version carrying both an undecided change request and an open blocking gap — the gap cannot close until the decision lands. */
  is_blocked_on_pending_decision: boolean | null;
  /** How many of this version's human-only gates have never been approached by software. */
  unexercised_human_gate_count: number | null;
  /** A live version whose human-only gates rest on assertion rather than on any observed attempt by software. */
  ai_boundary_is_unevidenced: boolean | null;
  /** How many high-blast-radius unapproved claims this version rests on. */
  load_bearing_unapproved_count: number | null;
  /** How many decided-but-unimplemented change requests hold this version's fitness down. */
  unlanded_decision_count: number | null;
  /** How many unrehearsed control entries exist in this procedure version. */
  unrehearsed_control_entry_count: number | null;
  /** Whether this live version has at least one blocking control that is only reachable by a path nobody has ever walked. */
  has_unrehearsed_control_entry: boolean | null;
  /** A version that is live for execution while carrying at least one never-rehearsed blocking control entry. */
  is_live_with_unrehearsed_control: boolean | null;
  /** How many review events on this version are past their promised cadence. */
  cadence_breach_count: number | null;
  /** Whether this version currently has at least one review event past the cadence its steward promised. */
  is_in_cadence_breach: boolean | null;
  /** Whether this version has at least one change request that is still open. */
  has_decision_in_flight: boolean | null;
  /** A cadence breach with no open change request against the version — a broken promise with no response in motion. */
  is_unremediated_cadence_breach: boolean | null;
  /** A cadence breach where a change request is at least open against the version. */
  is_managed_cadence_breach: boolean | null;
  /** A live version with neither a change request nor a review event ever recorded against it. */
  governance_is_silent: boolean | null;
  /** How many currently-valid knowledge fragments are attached to this version. */
  valid_fragment_count: number | null;
  /** Whether this version still holds at least one knowledge fragment that is currently valid. */
  still_owns_valid_knowledge: boolean | null;
  /** How many other versions declare that they supersede this one. */
  incoming_supersession_count: number | null;
  /** Whether any other version points at this one through a supersession link. */
  is_still_referenced: boolean | null;
  /** An unstewarded version that is still referenced by a supersession link or still owns currently-valid knowledge — nobody is accountable for it and something still depends on it. */
  is_load_bearing_orphan: boolean | null;
  /** An unstewarded version that nothing depends on — a genuine, safe retirement. */
  is_cleanly_retired: boolean | null;
  /** How many approved-but-unimplemented change requests are stalled against this version. */
  stalled_implementation_count: number | null;
  /** A version reading unfit to execute specifically because approved changes have not been marked implemented. */
  is_held_unfit_by_landed_decisions: boolean | null;
  /** How many steps in this version have not declared a control kind. */
  undeclared_control_kind_count: number | null;
  /** Whether this version contains any step whose control kind is unstated — meaning role-based and id-based control predicates cannot be trusted to cover it. */
  control_taxonomy_is_incomplete: boolean | null;
  /** TRUE when at least one change request against this version has been approved. */
  has_approved_change_request: boolean | null;
  /** How many change requests against this version have been approved. */
  approved_change_request_count: number | null;
  /** How many blocking controls in the model are neither computed nor owned. */
  unwatched_unowned_control_count: number | null;
  /** How many process-mining conformance runs exist for this procedure version. */
  mining_run_count: number | null;
  /** How many mining runs found major, real drift while this version was live. */
  drifted_mining_run_count: number | null;
  /** TRUE when mined operational evidence contradicts this live version's documented path. */
  has_unresolved_mining_drift: boolean | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const procedureVersionsFieldTypes: Record<string, FieldType> = {
  procedure_version_id: "string",
  name: "*string",
  procedure: "*string",
  version_number: "*string",
  title: "*string",
  status: "*string",
  issued_at: "*string",
  modified_at: "*string",
  created_by_agent: "*string",
  modified_by_agent: "*string",
  new_version_motivation: "*string",
  changelog_description: "*string",
  is_current: "*bool",
  count_of_steps: "*int",
  count_of_open_knowledge_gaps: "*int",
  is_ready_for_execution: "*bool",
  specified_step_count: "*float64",
  overdue_review_count: "*float64",
  open_change_request_count: "*float64",
  open_high_severity_gap_count: "*float64",
  is_fit_to_execute: "*bool",
  steward_review_cadence_days: "*float64",
  count_of_stewardship_assignments: "*int",
  has_any_steward: "*bool",
  is_live: "*bool",
  is_unstewarded: "*bool",
  is_live_and_unstewarded: "*bool",
  count_of_open_blocking_gaps: "*int",
  has_open_blocking_gap: "*bool",
  is_live_with_blocking_gap: "*bool",
  should_not_be_executable: "*bool",
  count_of_unapproved_reliance_fragments: "*int",
  runs_on_unapproved_knowledge: "*bool",
  count_of_overdue_gaps: "*int",
  count_of_change_requests: "*int",
  count_of_review_events: "*int",
  has_governance_record: "*bool",
  evaluation_context: "*string",
  as_of_instant: "*string",
  days_since_modified: "*int",
  days_since_last_review: "*int",
  was_modified_since_last_review: "*bool",
  modifier_is_authority: "*string",
  has_unwitnessed_change: "*bool",
  count_of_stale_fragments: "*int",
  knowledge_is_staler_than_cadence: "*bool",
  compound_fragile_fragment_count: "*float64",
  rests_on_compound_fragile_knowledge: "*bool",
  concentrated_witness_session_count: "*float64",
  knowledge_base_is_concentrated: "*bool",
  machine_consumed_unapproved_count: "*float64",
  feeds_unapproved_knowledge_to_machines: "*bool",
  genuinely_overdue_fragment_count: "*float64",
  awaited_decision_count: "*float64",
  scoped_open_blocking_gap_count: "*float64",
  is_blocked_on_pending_decision: "*bool",
  unexercised_human_gate_count: "*float64",
  ai_boundary_is_unevidenced: "*bool",
  load_bearing_unapproved_count: "*float64",
  unlanded_decision_count: "*float64",
  unrehearsed_control_entry_count: "*float64",
  has_unrehearsed_control_entry: "*bool",
  is_live_with_unrehearsed_control: "*bool",
  cadence_breach_count: "*float64",
  is_in_cadence_breach: "*bool",
  has_decision_in_flight: "*bool",
  is_unremediated_cadence_breach: "*bool",
  is_managed_cadence_breach: "*bool",
  governance_is_silent: "*bool",
  valid_fragment_count: "*float64",
  still_owns_valid_knowledge: "*bool",
  incoming_supersession_count: "*float64",
  is_still_referenced: "*bool",
  is_load_bearing_orphan: "*bool",
  is_cleanly_retired: "*bool",
  stalled_implementation_count: "*float64",
  is_held_unfit_by_landed_decisions: "*bool",
  undeclared_control_kind_count: "*float64",
  control_taxonomy_is_incomplete: "*bool",
  has_approved_change_request: "*bool",
  approved_change_request_count: "*float64",
  unwatched_unowned_control_count: "*float64",
  mining_run_count: "*float64",
  drifted_mining_run_count: "*float64",
  has_unresolved_mining_drift: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcedureVersions row.
 *  Formula: ={{Title}} */
export function calcProcedureVersionsName(tc: ProcedureVersionsRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes the IsReadyForExecution calculated field.
 *  TRUE when approved, populated, and free of blocking knowledge gaps.
 *  Formula: =AND({{Status}} = "Approved", {{CountOfSteps}} > 0, {{CountOfOpenKnowledgeGaps}} = 0) */
export function calcProcedureVersionsIsReadyForExecution(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbCmp(vInt(tc.count_of_steps), ">", vI(0))), erbBool3(erbEq(vInt(tc.count_of_open_knowledge_gaps), vI(0)))));
}

/** Computes the IsFitToExecute calculated field.
 *  TRUE when this version is approved, current on review, and carries no open change request or high-severity gap.
 *  Formula: =AND({{Status}} = "Approved", {{OverdueReviewCount}} = 0, {{OpenChangeRequestCount}} = 0, {{OpenHighSeverityGapCount}} = 0) */
export function calcProcedureVersionsIsFitToExecute(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbEq(vNum(tc.overdue_review_count), vI(0))), erbBool3(erbEq(vNum(tc.open_change_request_count), vI(0))), erbBool3(erbEq(vNum(tc.open_high_severity_gap_count), vI(0)))));
}

/** Computes the HasAnySteward calculated field.
 *  TRUE if any stewardship assignment has ever named this version.
 *  Formula: ={{CountOfStewardshipAssignments}} > 0 */
export function calcProcedureVersionsHasAnySteward(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_stewardship_assignments), ">", vI(0)));
}

/** Computes the IsLive calculated field.
 *  TRUE when this version is in a state where somebody could execute it.
 *  Formula: =OR({{Status}} = "Approved", {{Status}} = "Published") */
export function calcProcedureVersionsIsLive(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Published")))));
}

/** Computes the IsUnstewarded calculated field.
 *  TRUE when no stewardship assignment names this version.
 *  Formula: =NOT({{HasAnySteward}}) */
export function calcProcedureVersionsIsUnstewarded(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(vBool(tc.has_any_steward))));
}

/** Computes the IsLiveAndUnstewarded calculated field.
 *  TRUE when an executable version has nobody accountable for keeping it healthy.
 *  Formula: =AND({{IsLive}}, {{IsUnstewarded}}) */
export function calcProcedureVersionsIsLiveAndUnstewarded(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(vBool(tc.is_unstewarded))));
}

/** Computes the HasOpenBlockingGap calculated field.
 *  TRUE when at least one open blocking gap stands against this version.
 *  Formula: ={{CountOfOpenBlockingGaps}} > 0 */
export function calcProcedureVersionsHasOpenBlockingGap(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_open_blocking_gaps), ">", vI(0)));
}

/** Computes the IsLiveWithBlockingGap calculated field.
 *  TRUE when an executable version carries an unresolved blocking knowledge gap.
 *  Formula: =AND({{IsLive}}, {{HasOpenBlockingGap}}) */
export function calcProcedureVersionsIsLiveWithBlockingGap(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(vBool(tc.has_open_blocking_gap))));
}

/** Computes the ShouldNotBeExecutable calculated field.
 *  TRUE when the model says this version is ready to execute while a blocking gap is open against it.
 *  Formula: =AND({{IsReadyForExecution}}, {{HasOpenBlockingGap}}) */
export function calcProcedureVersionsShouldNotBeExecutable(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_ready_for_execution)), erbBool3(vBool(tc.has_open_blocking_gap))));
}

/** Computes the RunsOnUnapprovedKnowledge calculated field.
 *  TRUE when this version depends on at least one claim the knowledge authority has not approved.
 *  Formula: ={{CountOfUnapprovedRelianceFragments}} > 0 */
export function calcProcedureVersionsRunsOnUnapprovedKnowledge(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_unapproved_reliance_fragments), ">", vI(0)));
}

/** Computes the HasGovernanceRecord calculated field.
 *  TRUE when at least one change request or review event exists for this version.
 *  Formula: =OR({{CountOfChangeRequests}} > 0, {{CountOfReviewEvents}} > 0) */
export function calcProcedureVersionsHasGovernanceRecord(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbCmp(vInt(tc.count_of_change_requests), ">", vI(0))), erbBool3(erbCmp(vInt(tc.count_of_review_events), ">", vI(0)))));
}

/** Computes the DaysSinceModified calculated field.
 *  Days since this version's content was last changed.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{ModifiedAt}}, "days") */
export function calcProcedureVersionsDaysSinceModified(tc: ProcedureVersionsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.modified_at), vS("days"))));
}

/** Computes the WasModifiedSinceLastReview calculated field.
 *  TRUE when the version was edited more recently than it was reviewed.
 *  Formula: ={{DaysSinceModified}} < {{DaysSinceLastReview}} */
export function calcProcedureVersionsWasModifiedSinceLastReview(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.days_since_modified), "<", vInt(tc.days_since_last_review)));
}

/** Computes the HasUnwitnessedChange calculated field.
 *  TRUE when a live version's current content postdates every review it has had.
 *  Formula: =AND({{IsLive}}, {{WasModifiedSinceLastReview}}) */
export function calcProcedureVersionsHasUnwitnessedChange(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(vBool(tc.was_modified_since_last_review))));
}

/** Computes the KnowledgeIsStalerThanCadence calculated field.
 *  TRUE when this version rests on at least one claim older than its own review cadence.
 *  Formula: ={{CountOfStaleFragments}} > 0 */
export function calcProcedureVersionsKnowledgeIsStalerThanCadence(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_stale_fragments), ">", vI(0)));
}

/** Computes the RestsOnCompoundFragileKnowledge calculated field.
 *  A live procedure version resting on at least one knowledge fragment that carries three or more decay signals.
 *  Formula: =AND({{IsLive}}, {{CompoundFragileFragmentCount}} > 0) */
export function calcProcedureVersionsRestsOnCompoundFragileKnowledge(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(erbCmp(vNum(tc.compound_fragile_fragment_count), ">", vI(0)))));
}

/** Computes the KnowledgeBaseIsConcentrated calculated field.
 *  A live version where at least one single-witness session alone underwrites three or more of its live claims.
 *  Formula: =AND({{IsLive}}, {{ConcentratedWitnessSessionCount}} > 0) */
export function calcProcedureVersionsKnowledgeBaseIsConcentrated(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(erbCmp(vNum(tc.concentrated_witness_session_count), ">", vI(0)))));
}

/** Computes the FeedsUnapprovedKnowledgeToMachines calculated field.
 *  A live version that hands unapproved knowledge to a step no human is positioned to review.
 *  Formula: =AND({{IsLive}}, {{MachineConsumedUnapprovedCount}} > 0) */
export function calcProcedureVersionsFeedsUnapprovedKnowledgeToMachines(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(erbCmp(vNum(tc.machine_consumed_unapproved_count), ">", vI(0)))));
}

/** Computes the IsBlockedOnPendingDecision calculated field.
 *  A live version carrying both an undecided change request and an open blocking gap — the gap cannot close until the decision lands.
 *  Formula: =AND({{AwaitedDecisionCount}} > 0, {{ScopedOpenBlockingGapCount}} > 0) */
export function calcProcedureVersionsIsBlockedOnPendingDecision(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.awaited_decision_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.scoped_open_blocking_gap_count), ">", vI(0)))));
}

/** Computes the AiBoundaryIsUnevidenced calculated field.
 *  A live version whose human-only gates rest on assertion rather than on any observed attempt by software.
 *  Formula: =AND({{IsLive}}, {{UnexercisedHumanGateCount}} > 0) */
export function calcProcedureVersionsAiBoundaryIsUnevidenced(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(erbCmp(vNum(tc.unexercised_human_gate_count), ">", vI(0)))));
}

/** Computes the HasUnrehearsedControlEntry calculated field.
 *  Whether this live version has at least one blocking control that is only reachable by a path nobody has ever walked.
 *  Formula: ={{UnrehearsedControlEntryCount}} > 0 */
export function calcProcedureVersionsHasUnrehearsedControlEntry(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.unrehearsed_control_entry_count), ">", vI(0)));
}

/** Computes the IsLiveWithUnrehearsedControl calculated field.
 *  A version that is live for execution while carrying at least one never-rehearsed blocking control entry.
 *  Formula: =AND({{IsLive}}, {{HasUnrehearsedControlEntry}}) */
export function calcProcedureVersionsIsLiveWithUnrehearsedControl(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(vBool(tc.has_unrehearsed_control_entry))));
}

/** Computes the IsInCadenceBreach calculated field.
 *  Whether this version currently has at least one review event past the cadence its steward promised.
 *  Formula: ={{CadenceBreachCount}} > 0 */
export function calcProcedureVersionsIsInCadenceBreach(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.cadence_breach_count), ">", vI(0)));
}

/** Computes the HasDecisionInFlight calculated field.
 *  Whether this version has at least one change request that is still open.
 *  Formula: ={{OpenChangeRequestCount}} > 0 */
export function calcProcedureVersionsHasDecisionInFlight(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.open_change_request_count), ">", vI(0)));
}

/** Computes the IsUnremediatedCadenceBreach calculated field.
 *  A cadence breach with no open change request against the version — a broken promise with no response in motion.
 *  Formula: =AND({{IsInCadenceBreach}}, NOT({{HasDecisionInFlight}})) */
export function calcProcedureVersionsIsUnremediatedCadenceBreach(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_in_cadence_breach)), erbBool3(erbNot(erbBool3(vBool(tc.has_decision_in_flight))))));
}

/** Computes the IsManagedCadenceBreach calculated field.
 *  A cadence breach where a change request is at least open against the version.
 *  Formula: =AND({{IsInCadenceBreach}}, {{HasDecisionInFlight}}) */
export function calcProcedureVersionsIsManagedCadenceBreach(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_in_cadence_breach)), erbBool3(vBool(tc.has_decision_in_flight))));
}

/** Computes the GovernanceIsSilent calculated field.
 *  A live version with neither a change request nor a review event ever recorded against it.
 *  Formula: =AND({{IsLive}}, NOT({{HasGovernanceRecord}})) */
export function calcProcedureVersionsGovernanceIsSilent(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live)), erbBool3(erbNot(erbBool3(vBool(tc.has_governance_record))))));
}

/** Computes the StillOwnsValidKnowledge calculated field.
 *  Whether this version still holds at least one knowledge fragment that is currently valid.
 *  Formula: ={{ValidFragmentCount}} > 0 */
export function calcProcedureVersionsStillOwnsValidKnowledge(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.valid_fragment_count), ">", vI(0)));
}

/** Computes the IsStillReferenced calculated field.
 *  Whether any other version points at this one through a supersession link.
 *  Formula: ={{IncomingSupersessionCount}} > 0 */
export function calcProcedureVersionsIsStillReferenced(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.incoming_supersession_count), ">", vI(0)));
}

/** Computes the IsLoadBearingOrphan calculated field.
 *  An unstewarded version that is still referenced by a supersession link or still owns currently-valid knowledge — nobody is accountable for it and something still depends on it.
 *  Formula: =AND({{IsUnstewarded}}, OR({{StillOwnsValidKnowledge}}, {{IsStillReferenced}})) */
export function calcProcedureVersionsIsLoadBearingOrphan(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unstewarded)), erbBool3(erbOr(erbBool3(vBool(tc.still_owns_valid_knowledge)), erbBool3(vBool(tc.is_still_referenced))))));
}

/** Computes the IsCleanlyRetired calculated field.
 *  An unstewarded version that nothing depends on — a genuine, safe retirement.
 *  Formula: =AND({{IsUnstewarded}}, NOT({{StillOwnsValidKnowledge}}), NOT({{IsStillReferenced}})) */
export function calcProcedureVersionsIsCleanlyRetired(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unstewarded)), erbBool3(erbNot(erbBool3(vBool(tc.still_owns_valid_knowledge)))), erbBool3(erbNot(erbBool3(vBool(tc.is_still_referenced))))));
}

/** Computes the IsHeldUnfitByLandedDecisions calculated field.
 *  A version reading unfit to execute specifically because approved changes have not been marked implemented.
 *  Formula: =AND(NOT({{IsFitToExecute}}), {{StalledImplementationCount}} > 0) */
export function calcProcedureVersionsIsHeldUnfitByLandedDecisions(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_fit_to_execute)))), erbBool3(erbCmp(vNum(tc.stalled_implementation_count), ">", vI(0)))));
}

/** Computes the ControlTaxonomyIsIncomplete calculated field.
 *  Whether this version contains any step whose control kind is unstated — meaning role-based and id-based control predicates cannot be trusted to cover it.
 *  Formula: ={{UndeclaredControlKindCount}} > 0 */
export function calcProcedureVersionsControlTaxonomyIsIncomplete(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.undeclared_control_kind_count), ">", vI(0)));
}

/** Computes the HasApprovedChangeRequest calculated field.
 *  TRUE when at least one change request against this version has been approved.
 *  Formula: ={{ApprovedChangeRequestCount}} > 0 */
export function calcProcedureVersionsHasApprovedChangeRequest(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.approved_change_request_count), ">", vI(0)));
}

/** Computes the HasUnresolvedMiningDrift calculated field.
 *  TRUE when mined operational evidence contradicts this live version's documented path.
 *  Formula: ={{DriftedMiningRunCount}} > 0 */
export function calcProcedureVersionsHasUnresolvedMiningDrift(tc: ProcedureVersionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.drifted_mining_run_count), ">", vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedureVersions(tc: ProcedureVersionsRow): ProcedureVersionsRow {
  // Level 1
  calcGuard(tc, procedureVersionsFieldTypes, "name", () => { tc.name = calcProcedureVersionsName(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_ready_for_execution", () => { tc.is_ready_for_execution = calcProcedureVersionsIsReadyForExecution(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_fit_to_execute", () => { tc.is_fit_to_execute = calcProcedureVersionsIsFitToExecute(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_any_steward", () => { tc.has_any_steward = calcProcedureVersionsHasAnySteward(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_live", () => { tc.is_live = calcProcedureVersionsIsLive(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_open_blocking_gap", () => { tc.has_open_blocking_gap = calcProcedureVersionsHasOpenBlockingGap(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "runs_on_unapproved_knowledge", () => { tc.runs_on_unapproved_knowledge = calcProcedureVersionsRunsOnUnapprovedKnowledge(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_governance_record", () => { tc.has_governance_record = calcProcedureVersionsHasGovernanceRecord(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "days_since_modified", () => { tc.days_since_modified = calcProcedureVersionsDaysSinceModified(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "knowledge_is_staler_than_cadence", () => { tc.knowledge_is_staler_than_cadence = calcProcedureVersionsKnowledgeIsStalerThanCadence(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_blocked_on_pending_decision", () => { tc.is_blocked_on_pending_decision = calcProcedureVersionsIsBlockedOnPendingDecision(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_unrehearsed_control_entry", () => { tc.has_unrehearsed_control_entry = calcProcedureVersionsHasUnrehearsedControlEntry(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_in_cadence_breach", () => { tc.is_in_cadence_breach = calcProcedureVersionsIsInCadenceBreach(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_decision_in_flight", () => { tc.has_decision_in_flight = calcProcedureVersionsHasDecisionInFlight(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "still_owns_valid_knowledge", () => { tc.still_owns_valid_knowledge = calcProcedureVersionsStillOwnsValidKnowledge(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_still_referenced", () => { tc.is_still_referenced = calcProcedureVersionsIsStillReferenced(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "control_taxonomy_is_incomplete", () => { tc.control_taxonomy_is_incomplete = calcProcedureVersionsControlTaxonomyIsIncomplete(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_approved_change_request", () => { tc.has_approved_change_request = calcProcedureVersionsHasApprovedChangeRequest(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_unresolved_mining_drift", () => { tc.has_unresolved_mining_drift = calcProcedureVersionsHasUnresolvedMiningDrift(tc); });
  // Level 2
  calcGuard(tc, procedureVersionsFieldTypes, "is_unstewarded", () => { tc.is_unstewarded = calcProcedureVersionsIsUnstewarded(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_live_with_blocking_gap", () => { tc.is_live_with_blocking_gap = calcProcedureVersionsIsLiveWithBlockingGap(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "should_not_be_executable", () => { tc.should_not_be_executable = calcProcedureVersionsShouldNotBeExecutable(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "was_modified_since_last_review", () => { tc.was_modified_since_last_review = calcProcedureVersionsWasModifiedSinceLastReview(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "rests_on_compound_fragile_knowledge", () => { tc.rests_on_compound_fragile_knowledge = calcProcedureVersionsRestsOnCompoundFragileKnowledge(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "knowledge_base_is_concentrated", () => { tc.knowledge_base_is_concentrated = calcProcedureVersionsKnowledgeBaseIsConcentrated(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "feeds_unapproved_knowledge_to_machines", () => { tc.feeds_unapproved_knowledge_to_machines = calcProcedureVersionsFeedsUnapprovedKnowledgeToMachines(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "ai_boundary_is_unevidenced", () => { tc.ai_boundary_is_unevidenced = calcProcedureVersionsAiBoundaryIsUnevidenced(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_live_with_unrehearsed_control", () => { tc.is_live_with_unrehearsed_control = calcProcedureVersionsIsLiveWithUnrehearsedControl(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_unremediated_cadence_breach", () => { tc.is_unremediated_cadence_breach = calcProcedureVersionsIsUnremediatedCadenceBreach(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_managed_cadence_breach", () => { tc.is_managed_cadence_breach = calcProcedureVersionsIsManagedCadenceBreach(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "governance_is_silent", () => { tc.governance_is_silent = calcProcedureVersionsGovernanceIsSilent(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_held_unfit_by_landed_decisions", () => { tc.is_held_unfit_by_landed_decisions = calcProcedureVersionsIsHeldUnfitByLandedDecisions(tc); });
  // Level 3
  calcGuard(tc, procedureVersionsFieldTypes, "is_live_and_unstewarded", () => { tc.is_live_and_unstewarded = calcProcedureVersionsIsLiveAndUnstewarded(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "has_unwitnessed_change", () => { tc.has_unwitnessed_change = calcProcedureVersionsHasUnwitnessedChange(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_load_bearing_orphan", () => { tc.is_load_bearing_orphan = calcProcedureVersionsIsLoadBearingOrphan(tc); });
  calcGuard(tc, procedureVersionsFieldTypes, "is_cleanly_retired", () => { tc.is_cleanly_retired = calcProcedureVersionsIsCleanlyRetired(tc); });
  return tc;
}

/** Reads ProcedureVersions rows from a JSON array file. */
export function loadProcedureVersionsRows(file: string): ProcedureVersionsRow[] {
  return loadRows(file, { fields: procedureVersionsFieldTypes }) as unknown as ProcedureVersionsRow[];
}

// =============================================================================
// PROCEDUREVERSIONLINKS TABLE
// Directed links between versioned procedures. Maps to dcat:previousVersion/dcat:hasVersion and pko:nextVersion.
// =============================================================================

/** A row in the ProcedureVersionLinks table. */
export interface ProcedureVersionLinksRow {
  /** Stored logical identifier for one ProcedureVersionLinks row. */
  procedure_version_link_id: string;
  /** Human-readable calculated display alias for the ProcedureVersionLinks row. */
  name: string | null;
  /** Earlier version. */
  previous_procedure_version: string | null;
  /** Later version. */
  next_procedure_version: string | null;
  /** Exact version relation IRI. */
  relation_iri: string | null;
  /** Summary of semantic change across the edge. */
  change_summary: string | null;
  /** Echoes the superseded (previous) version id for rows that express a next-version relation. Supersession is carried by RelationIri here, not by a separate link-kind column. */
  superseded_version_key: string | null;
  _erb_errors?: Record<string, string>;
}

const procedureVersionLinksFieldTypes: Record<string, FieldType> = {
  procedure_version_link_id: "string",
  name: "*string",
  previous_procedure_version: "*string",
  next_procedure_version: "*string",
  relation_iri: "*string",
  change_summary: "*string",
  superseded_version_key: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcedureVersionLinks row.
 *  Formula: ={{PreviousProcedureVersion}} & " -> " & {{NextProcedureVersion}} */
export function calcProcedureVersionLinksName(tc: ProcedureVersionLinksRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.previous_procedure_version)), vS(" -> "), erbTextOr(vStr(tc.next_procedure_version))));
}

/** Computes the SupersededVersionKey calculated field.
 *  Echoes the superseded (previous) version id for rows that express a next-version relation. Supersession is carried by RelationIri here, not by a separate link-kind column.
 *  Formula: =IF({{RelationIri}} = "https://w3id.org/pko#nextVersion", {{PreviousProcedureVersion}}, "") */
export function calcProcedureVersionLinksSupersededVersionKey(tc: ProcedureVersionLinksRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(erbNullif(vStr(tc.relation_iri)), vS("https://w3id.org/pko#nextVersion"))), () => vStr(tc.previous_procedure_version), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedureVersionLinks(tc: ProcedureVersionLinksRow): ProcedureVersionLinksRow {
  // Level 1
  calcGuard(tc, procedureVersionLinksFieldTypes, "name", () => { tc.name = calcProcedureVersionLinksName(tc); });
  calcGuard(tc, procedureVersionLinksFieldTypes, "superseded_version_key", () => { tc.superseded_version_key = calcProcedureVersionLinksSupersededVersionKey(tc); });
  return tc;
}

/** Reads ProcedureVersionLinks rows from a JSON array file. */
export function loadProcedureVersionLinksRows(file: string): ProcedureVersionLinksRow[] {
  return loadRows(file, { fields: procedureVersionLinksFieldTypes }) as unknown as ProcedureVersionLinksRow[];
}

// =============================================================================
// PROCEDURESTATUSCHANGES TABLE
// Lifecycle events that move a procedure version between PKO statuses. Maps to pko:ChangeOfStatus, fromStatus, toStatus, and prov:atTime.
// =============================================================================

/** A row in the ProcedureStatusChanges table. */
export interface ProcedureStatusChangesRow {
  /** Stored logical identifier for one ProcedureStatusChanges row. */
  procedure_status_change_id: string;
  /** Human-readable calculated display alias for the ProcedureStatusChanges row. */
  name: string | null;
  /** Procedure version whose status changed. */
  procedure_version: string | null;
  /** Previous PKO status. */
  from_status: string | null;
  /** New PKO status. */
  to_status: string | null;
  /** Time of change; maps to prov:atTime. */
  changed_at: string | null;
  /** Agent responsible for the change. */
  changed_by_agent: string | null;
  /** Reason for changing status. */
  motivation: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const procedureStatusChangesFieldTypes: Record<string, FieldType> = {
  procedure_status_change_id: "string",
  name: "*string",
  procedure_version: "*string",
  from_status: "*string",
  to_status: "*string",
  changed_at: "*string",
  changed_by_agent: "*string",
  motivation: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcedureStatusChanges row.
 *  Formula: ={{ProcedureVersion}} & ": " & {{FromStatus}} & " -> " & {{ToStatus}} */
export function calcProcedureStatusChangesName(tc: ProcedureStatusChangesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_version)), vS(": "), erbTextOr(vStr(tc.from_status)), vS(" -> "), erbTextOr(vStr(tc.to_status))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedureStatusChanges(tc: ProcedureStatusChangesRow): ProcedureStatusChangesRow {
  // Level 1
  calcGuard(tc, procedureStatusChangesFieldTypes, "name", () => { tc.name = calcProcedureStatusChangesName(tc); });
  return tc;
}

/** Reads ProcedureStatusChanges rows from a JSON array file. */
export function loadProcedureStatusChangesRows(file: string): ProcedureStatusChangesRow[] {
  return loadRows(file, { fields: procedureStatusChangesFieldTypes }) as unknown as ProcedureStatusChangesRow[];
}

// =============================================================================
// STEPS TABLE
// Version-scoped units of work. Atomic steps map to pplan:Step; composite steps map to pplan:MultiStep. The specification is never conflated with execution.
// =============================================================================

/** A row in the Steps table. */
export interface StepsRow {
  /** Stored logical identifier for one Steps row. */
  step_id: string;
  /** Human-readable calculated display alias for the Steps row. */
  name: string | null;
  /** Procedure version containing the step; maps to pko:hasStep/pplan:isStepOfPlan. */
  procedure_version: string | null;
  /** Stable sequence label; maps to pko:stepNumber. */
  step_number: string | null;
  /** Human-readable step title. */
  title: string | null;
  /** Atomic or MultiStep. */
  step_kind: string | null;
  /** Role responsible for the step. */
  assigned_role: string | null;
  /** Resolved role label. */
  assigned_role_label: string | null;
  /** Current role-filler category. */
  assigned_agent_kind: string | null;
  /** Normative step instruction. */
  instruction: string | null;
  /** Expected duration represented as an OWL-Time duration in semantic projections. */
  expected_duration_minutes: number | null;
  /** PKO expertise level: Junior, Senior, Expert, or Master. */
  expertise_level: string | null;
  /** TRUE when a human must confirm the step; maps to pko:wasConfirmedBy at execution. */
  requires_human_confirmation: boolean | null;
  /** How many blocking requirements the specification attaches to this step. */
  blocking_requirement_count: number | null;
  /** How many operational bindings for this step are currently outside their freshness SLA. */
  stale_binding_count: number | null;
  /** How many authoritative bindings for this step are currently stale. */
  authoritative_stale_count: number | null;
  /** How many active exceptions the specification defines for this step. */
  available_exception_count: number | null;
  /** How many verifications the specification declares for this step. */
  declared_verification_count: number | null;
  /** TRUE for steps whose assigned role produces the work product rather than reviewing it. */
  is_preparation_step: boolean | null;
  /** TRUE for steps whose assigned role is an approval authority. */
  is_approval_step: boolean | null;
  /** Number of authoritative bindings for this step that are past their freshness SLA. */
  stale_authoritative_binding_count: number | null;
  /** TRUE when no authoritative binding for this step is stale. */
  inputs_are_fresh: boolean | null;
  /** TRUE when this step is specified to be performed by software rather than a person. */
  is_software_assigned: boolean | null;
  /** TRUE for the steps that exist specifically to place a human commitment between drafting and delivery. */
  is_human_approval_gate: boolean | null;
  /** TRUE when a designated approval gate is in fact assigned to a human role. */
  gate_held_by_human: boolean | null;
  /** Number of authority boundaries currently binding at this step. */
  binding_boundary_count: number | null;
  /** Whether this step's assigned role is a non-human role with no governing assignment. */
  assigned_role_is_ungoverned: boolean | null;
  /** Number of bindings at this step that are unapproved or stale. */
  unusable_binding_count: number | null;
  /** TRUE when every binding at this step is an approved, fresh source. */
  all_sources_usable: boolean | null;
  /** What kind of control this step is: Preparation, Approval, LegalReview, Verification, Extraction, Publication, Retrospective, or None. That policy-04 is a legal review is a property OF policy-04, not of a formula that happens to name it; storing it as data makes downstream predicates portable to procedures that do not exist yet. */
  control_kind: string | null;
  /** How many boundaries governing this step are being enforced without a valid ratifying claim. */
  unwarranted_boundary_count: number | null;
  /** Whether this step's constraints on machine authority rest on a claim that is no longer valid. */
  is_governed_by_unwarranted_boundary: boolean | null;
  /** How many times a software agent has actually executed this step. */
  software_execution_count: number | null;
  /** Whether any software agent has ever executed this step. */
  has_been_approached_by_software: boolean | null;
  /** A human-only approval gate that no software agent has ever attempted — the control is asserted, not demonstrated. */
  is_unexercised_human_gate: boolean | null;
  /** A human gate that software has actually reached and that a human nevertheless held. */
  is_demonstrated_human_gate: boolean | null;
  /** Composite-key echo: this step's procedure version when the step is an unexercised human gate, blank otherwise. */
  unexercised_gate_version_key: string | null;
  /** Whether this step declares what kind of control it is. */
  has_declared_control_kind: boolean | null;
  /** Composite-key echo: this step's procedure version when the step has no declared control kind, blank otherwise. */
  undeclared_control_version_key: string | null;
  /** An approval-kind step whose assigned role is currently held by an AI agent or automated pipeline. */
  approval_step_is_software_assigned: boolean | null;
  /** How many blocking controls bound to this step have no computed witness. */
  unwitnessed_blocking_count: number | null;
  /** Exact P-Plan class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const stepsFieldTypes: Record<string, FieldType> = {
  step_id: "string",
  name: "*string",
  procedure_version: "*string",
  step_number: "*string",
  title: "*string",
  step_kind: "*string",
  assigned_role: "*string",
  assigned_role_label: "*string",
  assigned_agent_kind: "*string",
  instruction: "*string",
  expected_duration_minutes: "*int",
  expertise_level: "*string",
  requires_human_confirmation: "*bool",
  blocking_requirement_count: "*float64",
  stale_binding_count: "*float64",
  authoritative_stale_count: "*float64",
  available_exception_count: "*float64",
  declared_verification_count: "*float64",
  is_preparation_step: "*bool",
  is_approval_step: "*bool",
  stale_authoritative_binding_count: "*float64",
  inputs_are_fresh: "*bool",
  is_software_assigned: "*bool",
  is_human_approval_gate: "*bool",
  gate_held_by_human: "*bool",
  binding_boundary_count: "*float64",
  assigned_role_is_ungoverned: "*bool",
  unusable_binding_count: "*float64",
  all_sources_usable: "*bool",
  control_kind: "*string",
  unwarranted_boundary_count: "*float64",
  is_governed_by_unwarranted_boundary: "*bool",
  software_execution_count: "*float64",
  has_been_approached_by_software: "*bool",
  is_unexercised_human_gate: "*bool",
  is_demonstrated_human_gate: "*bool",
  unexercised_gate_version_key: "*string",
  has_declared_control_kind: "*bool",
  undeclared_control_version_key: "*string",
  approval_step_is_software_assigned: "*bool",
  unwitnessed_blocking_count: "*float64",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Steps row.
 *  Formula: ={{StepNumber}} & ". " & {{Title}} */
export function calcStepsName(tc: StepsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step_number)), vS(". "), erbTextOr(vStr(tc.title))));
}

/** Computes the IsPreparationStep calculated field.
 *  TRUE for steps whose assigned role produces the work product rather than reviewing it.
 *  Formula: =OR({{AssignedRole}} = "finance-analyst", {{AssignedRole}} = "variance-review-agent") */
export function calcStepsIsPreparationStep(tc: StepsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.assigned_role)), vS("finance-analyst"))), erbBool3(erbEq(erbNullif(vStr(tc.assigned_role)), vS("variance-review-agent")))));
}

/** Computes the IsApprovalStep calculated field.
 *  TRUE for steps whose assigned role is an approval authority.
 *  Formula: =OR({{AssignedRole}} = "controller", {{AssignedRole}} = "cfo") */
export function calcStepsIsApprovalStep(tc: StepsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.assigned_role)), vS("controller"))), erbBool3(erbEq(erbNullif(vStr(tc.assigned_role)), vS("cfo")))));
}

/** Computes the InputsAreFresh calculated field.
 *  TRUE when no authoritative binding for this step is stale.
 *  Formula: ={{StaleAuthoritativeBindingCount}} = 0 */
export function calcStepsInputsAreFresh(tc: StepsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.stale_authoritative_binding_count), vI(0)));
}

/** Computes the IsSoftwareAssigned calculated field.
 *  TRUE when this step is specified to be performed by software rather than a person.
 *  Formula: =OR({{AssignedAgentKind}} = "AIAgent", {{AssignedAgentKind}} = "AutomatedPipeline") */
export function calcStepsIsSoftwareAssigned(tc: StepsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(vStr(tc.assigned_agent_kind), vS("AIAgent"))), erbBool3(erbEq(vStr(tc.assigned_agent_kind), vS("AutomatedPipeline")))));
}

/** Computes the IsHumanApprovalGate calculated field.
 *  TRUE for the steps that exist specifically to place a human commitment between drafting and delivery.
 *  Formula: =AND(NOT({{IsSoftwareAssigned}}), OR({{StepId}} = "policy-05", {{StepId}} = "close-06")) */
export function calcStepsIsHumanApprovalGate(tc: StepsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_software_assigned)))), erbBool3(erbOr(erbBool3(erbEq(erbNullif(vStrPlain(tc.step_id)), vS("policy-05"))), erbBool3(erbEq(erbNullif(vStrPlain(tc.step_id)), vS("close-06")))))));
}

/** Computes the GateHeldByHuman calculated field.
 *  TRUE when a designated approval gate is in fact assigned to a human role.
 *  Formula: =AND({{IsHumanApprovalGate}}, {{AssignedAgentKind}} = "Human") */
export function calcStepsGateHeldByHuman(tc: StepsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_human_approval_gate)), erbBool3(erbEq(vStr(tc.assigned_agent_kind), vS("Human")))));
}

/** Computes the AllSourcesUsable calculated field.
 *  TRUE when every binding at this step is an approved, fresh source.
 *  Formula: ={{UnusableBindingCount}} = 0 */
export function calcStepsAllSourcesUsable(tc: StepsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.unusable_binding_count), vI(0)));
}

/** Computes the IsGovernedByUnwarrantedBoundary calculated field.
 *  Whether this step's constraints on machine authority rest on a claim that is no longer valid.
 *  Formula: ={{UnwarrantedBoundaryCount}} > 0 */
export function calcStepsIsGovernedByUnwarrantedBoundary(tc: StepsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.unwarranted_boundary_count), ">", vI(0)));
}

/** Computes the HasBeenApproachedBySoftware calculated field.
 *  Whether any software agent has ever executed this step.
 *  Formula: ={{SoftwareExecutionCount}} > 0 */
export function calcStepsHasBeenApproachedBySoftware(tc: StepsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.software_execution_count), ">", vI(0)));
}

/** Computes the IsUnexercisedHumanGate calculated field.
 *  A human-only approval gate that no software agent has ever attempted — the control is asserted, not demonstrated.
 *  Formula: =AND({{IsHumanApprovalGate}}, NOT({{HasBeenApproachedBySoftware}})) */
export function calcStepsIsUnexercisedHumanGate(tc: StepsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_human_approval_gate)), erbBool3(erbNot(erbBool3(vBool(tc.has_been_approached_by_software))))));
}

/** Computes the IsDemonstratedHumanGate calculated field.
 *  A human gate that software has actually reached and that a human nevertheless held.
 *  Formula: =AND({{IsHumanApprovalGate}}, {{HasBeenApproachedBySoftware}}, {{GateHeldByHuman}}) */
export function calcStepsIsDemonstratedHumanGate(tc: StepsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_human_approval_gate)), erbBool3(vBool(tc.has_been_approached_by_software)), erbBool3(vBool(tc.gate_held_by_human))));
}

/** Computes the UnexercisedGateVersionKey calculated field.
 *  Composite-key echo: this step's procedure version when the step is an unexercised human gate, blank otherwise.
 *  Formula: =IF({{IsUnexercisedHumanGate}}, {{ProcedureVersion}}, "") */
export function calcStepsUnexercisedGateVersionKey(tc: StepsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unexercised_human_gate)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the HasDeclaredControlKind calculated field.
 *  Whether this step declares what kind of control it is.
 *  Formula: ={{ControlKind}} <> "" */
export function calcStepsHasDeclaredControlKind(tc: StepsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.control_kind)));
}

/** Computes the UndeclaredControlVersionKey calculated field.
 *  Composite-key echo: this step's procedure version when the step has no declared control kind, blank otherwise.
 *  Formula: =IF({{HasDeclaredControlKind}}, "", {{ProcedureVersion}}) */
export function calcStepsUndeclaredControlVersionKey(tc: StepsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.has_declared_control_kind)), () => vS(""), () => vStr(tc.procedure_version)));
}

/** Computes the ApprovalStepIsSoftwareAssigned calculated field.
 *  An approval-kind step whose assigned role is currently held by an AI agent or automated pipeline.
 *  Formula: =AND({{ControlKind}} = "Approval", {{IsSoftwareAssigned}}) */
export function calcStepsApprovalStepIsSoftwareAssigned(tc: StepsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.control_kind)), vS("Approval"))), erbBool3(vBool(tc.is_software_assigned))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeSteps(tc: StepsRow): StepsRow {
  // Level 1
  calcGuard(tc, stepsFieldTypes, "name", () => { tc.name = calcStepsName(tc); });
  calcGuard(tc, stepsFieldTypes, "is_preparation_step", () => { tc.is_preparation_step = calcStepsIsPreparationStep(tc); });
  calcGuard(tc, stepsFieldTypes, "is_approval_step", () => { tc.is_approval_step = calcStepsIsApprovalStep(tc); });
  calcGuard(tc, stepsFieldTypes, "inputs_are_fresh", () => { tc.inputs_are_fresh = calcStepsInputsAreFresh(tc); });
  calcGuard(tc, stepsFieldTypes, "is_software_assigned", () => { tc.is_software_assigned = calcStepsIsSoftwareAssigned(tc); });
  calcGuard(tc, stepsFieldTypes, "all_sources_usable", () => { tc.all_sources_usable = calcStepsAllSourcesUsable(tc); });
  calcGuard(tc, stepsFieldTypes, "is_governed_by_unwarranted_boundary", () => { tc.is_governed_by_unwarranted_boundary = calcStepsIsGovernedByUnwarrantedBoundary(tc); });
  calcGuard(tc, stepsFieldTypes, "has_been_approached_by_software", () => { tc.has_been_approached_by_software = calcStepsHasBeenApproachedBySoftware(tc); });
  calcGuard(tc, stepsFieldTypes, "has_declared_control_kind", () => { tc.has_declared_control_kind = calcStepsHasDeclaredControlKind(tc); });
  // Level 2
  calcGuard(tc, stepsFieldTypes, "is_human_approval_gate", () => { tc.is_human_approval_gate = calcStepsIsHumanApprovalGate(tc); });
  calcGuard(tc, stepsFieldTypes, "undeclared_control_version_key", () => { tc.undeclared_control_version_key = calcStepsUndeclaredControlVersionKey(tc); });
  calcGuard(tc, stepsFieldTypes, "approval_step_is_software_assigned", () => { tc.approval_step_is_software_assigned = calcStepsApprovalStepIsSoftwareAssigned(tc); });
  // Level 3
  calcGuard(tc, stepsFieldTypes, "gate_held_by_human", () => { tc.gate_held_by_human = calcStepsGateHeldByHuman(tc); });
  calcGuard(tc, stepsFieldTypes, "is_unexercised_human_gate", () => { tc.is_unexercised_human_gate = calcStepsIsUnexercisedHumanGate(tc); });
  // Level 4
  calcGuard(tc, stepsFieldTypes, "is_demonstrated_human_gate", () => { tc.is_demonstrated_human_gate = calcStepsIsDemonstratedHumanGate(tc); });
  calcGuard(tc, stepsFieldTypes, "unexercised_gate_version_key", () => { tc.unexercised_gate_version_key = calcStepsUnexercisedGateVersionKey(tc); });
  return tc;
}

/** Reads Steps rows from a JSON array file. */
export function loadStepsRows(file: string): StepsRow[] {
  return loadRows(file, { fields: stepsFieldTypes }) as unknown as StepsRow[];
}

// =============================================================================
// STEPTRANSITIONS TABLE
// Directed control-flow edges represented as first-class pko:Transition instances with fromStep/toStep and next/alternative/fallback semantics.
// =============================================================================

/** A row in the StepTransitions table. */
export interface StepTransitionsRow {
  /** Stored logical identifier for one StepTransitions row. */
  step_transition_id: string;
  /** Human-readable calculated display alias for the StepTransitions row. */
  name: string | null;
  /** Procedure version whose control flow owns the transition. */
  procedure_version: string | null;
  /** Source step; maps to pko:fromStep. */
  from_step: string | null;
  /** Destination step; maps to pko:toStep. */
  to_step: string | null;
  /** Next, Alternative, or Fallback. */
  transition_kind: string | null;
  /** Condition under which the edge is taken. */
  condition: string | null;
  /** Evaluation priority when several outgoing edges exist. */
  priority: number | null;
  /** TRUE when this transition is a non-default path taken because something went wrong. */
  is_recovery_path: boolean | null;
  /** How many times the step this transition leaves from has actually been executed. */
  count_of_from_step_executions: number | null;
  /** How many times the step this transition arrives at has actually been executed. */
  count_of_to_step_executions: number | null;
  /** TRUE when the origin step of this transition has been executed at least once. */
  has_reachable_origin: boolean | null;
  /** TRUE when the destination step of this transition has been executed at least once. */
  has_reachable_target: boolean | null;
  /** TRUE when at least one end of this transition has never appeared in any step execution. */
  is_never_exercised: boolean | null;
  /** TRUE for a Fallback or Alternative transition whose endpoints show no execution evidence. */
  is_untested_recovery_path: boolean | null;
  /** How many times this exact transition has actually been traversed. */
  count_of_observed_traversals: number | null;
  /** TRUE when this transition has been walked at least once. */
  has_been_traversed: boolean | null;
  /** TRUE for a Fallback/Alternative transition with zero recorded traversals. */
  is_unwalked_recovery_path: boolean | null;
  /** How many blocking requirements are attached to the step this transition lands on. */
  target_blocking_requirement_count: number | null;
  /** Whether the destination step of this transition carries at least one blocking control. */
  target_carries_blocking_control: boolean | null;
  /** A recovery path that has never been traversed and that leads into a step carrying a blocking control. */
  is_unrehearsed_control_entry: boolean | null;
  /** Composite-key echo: this transition's procedure version when it is an unrehearsed control entry, blank otherwise. */
  unrehearsed_control_version_key: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const stepTransitionsFieldTypes: Record<string, FieldType> = {
  step_transition_id: "string",
  name: "*string",
  procedure_version: "*string",
  from_step: "*string",
  to_step: "*string",
  transition_kind: "*string",
  condition: "*string",
  priority: "*int",
  is_recovery_path: "*bool",
  count_of_from_step_executions: "*int",
  count_of_to_step_executions: "*int",
  has_reachable_origin: "*bool",
  has_reachable_target: "*bool",
  is_never_exercised: "*bool",
  is_untested_recovery_path: "*bool",
  count_of_observed_traversals: "*int",
  has_been_traversed: "*bool",
  is_unwalked_recovery_path: "*bool",
  target_blocking_requirement_count: "*float64",
  target_carries_blocking_control: "*bool",
  is_unrehearsed_control_entry: "*bool",
  unrehearsed_control_version_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepTransitions row.
 *  Formula: ={{FromStep}} & " -> " & {{ToStep}} */
export function calcStepTransitionsName(tc: StepTransitionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.from_step)), vS(" -> "), erbTextOr(vStr(tc.to_step))));
}

/** Computes the IsRecoveryPath calculated field.
 *  TRUE when this transition is a non-default path taken because something went wrong.
 *  Formula: =OR({{TransitionKind}} = "Fallback", {{TransitionKind}} = "Alternative") */
export function calcStepTransitionsIsRecoveryPath(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.transition_kind)), vS("Fallback"))), erbBool3(erbEq(erbNullif(vStr(tc.transition_kind)), vS("Alternative")))));
}

/** Computes the HasReachableOrigin calculated field.
 *  TRUE when the origin step of this transition has been executed at least once.
 *  Formula: ={{CountOfFromStepExecutions}} > 0 */
export function calcStepTransitionsHasReachableOrigin(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_from_step_executions), ">", vI(0)));
}

/** Computes the HasReachableTarget calculated field.
 *  TRUE when the destination step of this transition has been executed at least once.
 *  Formula: ={{CountOfToStepExecutions}} > 0 */
export function calcStepTransitionsHasReachableTarget(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_to_step_executions), ">", vI(0)));
}

/** Computes the IsNeverExercised calculated field.
 *  TRUE when at least one end of this transition has never appeared in any step execution.
 *  Formula: =NOT(AND({{HasReachableOrigin}}, {{HasReachableTarget}})) */
export function calcStepTransitionsIsNeverExercised(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbAnd(erbBool3(vBool(tc.has_reachable_origin)), erbBool3(vBool(tc.has_reachable_target))))));
}

/** Computes the IsUntestedRecoveryPath calculated field.
 *  TRUE for a Fallback or Alternative transition whose endpoints show no execution evidence.
 *  Formula: =AND({{IsRecoveryPath}}, {{IsNeverExercised}}) */
export function calcStepTransitionsIsUntestedRecoveryPath(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_recovery_path)), erbBool3(vBool(tc.is_never_exercised))));
}

/** Computes the HasBeenTraversed calculated field.
 *  TRUE when this transition has been walked at least once.
 *  Formula: ={{CountOfObservedTraversals}} > 0 */
export function calcStepTransitionsHasBeenTraversed(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_observed_traversals), ">", vI(0)));
}

/** Computes the IsUnwalkedRecoveryPath calculated field.
 *  TRUE for a Fallback/Alternative transition with zero recorded traversals.
 *  Formula: =AND({{IsRecoveryPath}}, NOT({{HasBeenTraversed}})) */
export function calcStepTransitionsIsUnwalkedRecoveryPath(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_recovery_path)), erbBool3(erbNot(erbBool3(vBool(tc.has_been_traversed))))));
}

/** Computes the TargetCarriesBlockingControl calculated field.
 *  Whether the destination step of this transition carries at least one blocking control.
 *  Formula: ={{TargetBlockingRequirementCount}} > 0 */
export function calcStepTransitionsTargetCarriesBlockingControl(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.target_blocking_requirement_count), ">", vI(0)));
}

/** Computes the IsUnrehearsedControlEntry calculated field.
 *  A recovery path that has never been traversed and that leads into a step carrying a blocking control.
 *  Formula: =AND({{IsUnwalkedRecoveryPath}}, {{TargetCarriesBlockingControl}}) */
export function calcStepTransitionsIsUnrehearsedControlEntry(tc: StepTransitionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unwalked_recovery_path)), erbBool3(vBool(tc.target_carries_blocking_control))));
}

/** Computes the UnrehearsedControlVersionKey calculated field.
 *  Composite-key echo: this transition's procedure version when it is an unrehearsed control entry, blank otherwise.
 *  Formula: =IF({{IsUnrehearsedControlEntry}}, {{ProcedureVersion}}, "") */
export function calcStepTransitionsUnrehearsedControlVersionKey(tc: StepTransitionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unrehearsed_control_entry)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepTransitions(tc: StepTransitionsRow): StepTransitionsRow {
  // Level 1
  calcGuard(tc, stepTransitionsFieldTypes, "name", () => { tc.name = calcStepTransitionsName(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "is_recovery_path", () => { tc.is_recovery_path = calcStepTransitionsIsRecoveryPath(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "has_reachable_origin", () => { tc.has_reachable_origin = calcStepTransitionsHasReachableOrigin(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "has_reachable_target", () => { tc.has_reachable_target = calcStepTransitionsHasReachableTarget(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "has_been_traversed", () => { tc.has_been_traversed = calcStepTransitionsHasBeenTraversed(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "target_carries_blocking_control", () => { tc.target_carries_blocking_control = calcStepTransitionsTargetCarriesBlockingControl(tc); });
  // Level 2
  calcGuard(tc, stepTransitionsFieldTypes, "is_never_exercised", () => { tc.is_never_exercised = calcStepTransitionsIsNeverExercised(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "is_unwalked_recovery_path", () => { tc.is_unwalked_recovery_path = calcStepTransitionsIsUnwalkedRecoveryPath(tc); });
  // Level 3
  calcGuard(tc, stepTransitionsFieldTypes, "is_untested_recovery_path", () => { tc.is_untested_recovery_path = calcStepTransitionsIsUntestedRecoveryPath(tc); });
  calcGuard(tc, stepTransitionsFieldTypes, "is_unrehearsed_control_entry", () => { tc.is_unrehearsed_control_entry = calcStepTransitionsIsUnrehearsedControlEntry(tc); });
  // Level 4
  calcGuard(tc, stepTransitionsFieldTypes, "unrehearsed_control_version_key", () => { tc.unrehearsed_control_version_key = calcStepTransitionsUnrehearsedControlVersionKey(tc); });
  return tc;
}

/** Reads StepTransitions rows from a JSON array file. */
export function loadStepTransitionsRows(file: string): StepTransitionsRow[] {
  return loadRows(file, { fields: stepTransitionsFieldTypes }) as unknown as StepTransitionsRow[];
}

// =============================================================================
// ACTIONS TABLE
// Human actions required by steps. Maps to pko:Action and pko:requiresAction.
// =============================================================================

/** A row in the Actions table. */
export interface ActionsRow {
  /** Stored logical identifier for one Actions row. */
  action_id: string;
  /** Human-readable calculated display alias for the Actions row. */
  name: string | null;
  /** Action label. */
  label: string | null;
  /** Normative definition of the action. */
  definition: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const actionsFieldTypes: Record<string, FieldType> = {
  action_id: "string",
  name: "*string",
  label: "*string",
  definition: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Actions row.
 *  Formula: ={{Label}} */
export function calcActionsName(tc: ActionsRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeActions(tc: ActionsRow): ActionsRow {
  // Level 1
  calcGuard(tc, actionsFieldTypes, "name", () => { tc.name = calcActionsName(tc); });
  return tc;
}

/** Reads Actions rows from a JSON array file. */
export function loadActionsRows(file: string): ActionsRow[] {
  return loadRows(file, { fields: actionsFieldTypes }) as unknown as ActionsRow[];
}

// =============================================================================
// FUNCTIONS TABLE
// Software or algorithmic functions required by steps. Maps to pko:Function and pko:requiresFunction.
// =============================================================================

/** A row in the Functions table. */
export interface FunctionsRow {
  /** Stored logical identifier for one Functions row. */
  function_id: string;
  /** Human-readable calculated display alias for the Functions row. */
  name: string | null;
  /** Function label. */
  label: string | null;
  /** Deterministic or AI-assisted behavior. */
  definition: string | null;
  /** Operational implementation or model key. */
  implementation_key: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const functionsFieldTypes: Record<string, FieldType> = {
  function_id: "string",
  name: "*string",
  label: "*string",
  definition: "*string",
  implementation_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Functions row.
 *  Formula: ={{Label}} */
export function calcFunctionsName(tc: FunctionsRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeFunctions(tc: FunctionsRow): FunctionsRow {
  // Level 1
  calcGuard(tc, functionsFieldTypes, "name", () => { tc.name = calcFunctionsName(tc); });
  return tc;
}

/** Reads Functions rows from a JSON array file. */
export function loadFunctionsRows(file: string): FunctionsRow[] {
  return loadRows(file, { fields: functionsFieldTypes }) as unknown as FunctionsRow[];
}

// =============================================================================
// TOOLS TABLE
// Tools required to execute steps. Maps to m4ing:Tool and pko:requiresTool.
// =============================================================================

/** A row in the Tools table. */
export interface ToolsRow {
  /** Stored logical identifier for one Tools row. */
  tool_id: string;
  /** Human-readable calculated display alias for the Tools row. */
  name: string | null;
  /** Tool label. */
  label: string | null;
  /** Operational purpose. */
  purpose: string | null;
  /** Exact semantic class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const toolsFieldTypes: Record<string, FieldType> = {
  tool_id: "string",
  name: "*string",
  label: "*string",
  purpose: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Tools row.
 *  Formula: ={{Label}} */
export function calcToolsName(tc: ToolsRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeTools(tc: ToolsRow): ToolsRow {
  // Level 1
  calcGuard(tc, toolsFieldTypes, "name", () => { tc.name = calcToolsName(tc); });
  return tc;
}

/** Reads Tools rows from a JSON array file. */
export function loadToolsRows(file: string): ToolsRow[] {
  return loadRows(file, { fields: toolsFieldTypes }) as unknown as ToolsRow[];
}

// =============================================================================
// STEPACTIONS TABLE
// Many-to-many Step/Action semantics normalized into a first-class ERB junction table.
// =============================================================================

/** A row in the StepActions table. */
export interface StepActionsRow {
  /** Stored logical identifier for one StepActions row. */
  step_action_id: string;
  /** Human-readable calculated display alias for the StepActions row. */
  name: string | null;
  /** Step side of the relationship. */
  step: string | null;
  /** Action side of the relationship. */
  action: string | null;
  _erb_errors?: Record<string, string>;
}

const stepActionsFieldTypes: Record<string, FieldType> = {
  step_action_id: "string",
  name: "*string",
  step: "*string",
  action: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepActions row.
 *  Formula: ={{Step}} & " / " & {{Action}} */
export function calcStepActionsName(tc: StepActionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS(" / "), erbTextOr(vStr(tc.action))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepActions(tc: StepActionsRow): StepActionsRow {
  // Level 1
  calcGuard(tc, stepActionsFieldTypes, "name", () => { tc.name = calcStepActionsName(tc); });
  return tc;
}

/** Reads StepActions rows from a JSON array file. */
export function loadStepActionsRows(file: string): StepActionsRow[] {
  return loadRows(file, { fields: stepActionsFieldTypes }) as unknown as StepActionsRow[];
}

// =============================================================================
// STEPFUNCTIONS TABLE
// Many-to-many Step/Function semantics normalized into an ERB junction table.
// =============================================================================

/** A row in the StepFunctions table. */
export interface StepFunctionsRow {
  /** Stored logical identifier for one StepFunctions row. */
  step_function_id: string;
  /** Human-readable calculated display alias for the StepFunctions row. */
  name: string | null;
  /** Step side of the relationship. */
  step: string | null;
  /** Function side of the relationship. */
  function: string | null;
  _erb_errors?: Record<string, string>;
}

const stepFunctionsFieldTypes: Record<string, FieldType> = {
  step_function_id: "string",
  name: "*string",
  step: "*string",
  function: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepFunctions row.
 *  Formula: ={{Step}} & " / " & {{Function}} */
export function calcStepFunctionsName(tc: StepFunctionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS(" / "), erbTextOr(vStr(tc.function))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepFunctions(tc: StepFunctionsRow): StepFunctionsRow {
  // Level 1
  calcGuard(tc, stepFunctionsFieldTypes, "name", () => { tc.name = calcStepFunctionsName(tc); });
  return tc;
}

/** Reads StepFunctions rows from a JSON array file. */
export function loadStepFunctionsRows(file: string): StepFunctionsRow[] {
  return loadRows(file, { fields: stepFunctionsFieldTypes }) as unknown as StepFunctionsRow[];
}

// =============================================================================
// STEPTOOLS TABLE
// Many-to-many Step/Tool semantics normalized into an ERB junction table.
// =============================================================================

/** A row in the StepTools table. */
export interface StepToolsRow {
  /** Stored logical identifier for one StepTools row. */
  step_tool_id: string;
  /** Human-readable calculated display alias for the StepTools row. */
  name: string | null;
  /** Step side of the relationship. */
  step: string | null;
  /** Tool side of the relationship. */
  tool: string | null;
  _erb_errors?: Record<string, string>;
}

const stepToolsFieldTypes: Record<string, FieldType> = {
  step_tool_id: "string",
  name: "*string",
  step: "*string",
  tool: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepTools row.
 *  Formula: ={{Step}} & " / " & {{Tool}} */
export function calcStepToolsName(tc: StepToolsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS(" / "), erbTextOr(vStr(tc.tool))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepTools(tc: StepToolsRow): StepToolsRow {
  // Level 1
  calcGuard(tc, stepToolsFieldTypes, "name", () => { tc.name = calcStepToolsName(tc); });
  return tc;
}

/** Reads StepTools rows from a JSON array file. */
export function loadStepToolsRows(file: string): StepToolsRow[] {
  return loadRows(file, { fields: stepToolsFieldTypes }) as unknown as StepToolsRow[];
}

// =============================================================================
// REQUIREMENTS TABLE
// Normative requirements applied to procedures, steps, or transitions. Maps to pko:Requirement and pko:hasRequirement.
// =============================================================================

/** A row in the Requirements table. */
export interface RequirementsRow {
  /** Stored logical identifier for one Requirements row. */
  requirement_id: string;
  /** Human-readable calculated display alias for the Requirements row. */
  name: string | null;
  /** Requirement label. */
  label: string | null;
  /** Controlled requirement type; maps to pko:hasRequirementType. */
  requirement_type: string | null;
  /** Normative requirement statement. */
  statement: string | null;
  /** Why the requirement exists. */
  rationale: string | null;
  /** TRUE when unsatisfied requirement blocks execution or approval. */
  is_blocking: boolean | null;
  /** How many times this requirement has ever been evaluated against an execution. */
  satisfaction_record_count: number | null;
  /** How many steps the specification binds this requirement to. */
  step_binding_count: number | null;
  /** TRUE when at least one step carries this requirement. */
  is_bound_to_any_step: boolean | null;
  /** TRUE when this requirement has at least one satisfaction record in the model. */
  has_ever_been_evaluated: boolean | null;
  /** How many times this requirement has ever produced a less-than-satisfied outcome. */
  negative_outcome_count: number | null;
  /** TRUE for a blocking requirement that is attached to a step in the specification but has never once been evaluated on any execution. */
  is_inoperative_control: boolean | null;
  /** TRUE for a blocking requirement that is not attached to any step at all. */
  is_decorative_control: boolean | null;
  /** Whether a computed predicate in this rulebook evaluates this requirement, as opposed to it being satisfied by human assertion only. */
  has_computed_witness: boolean | null;
  /** The fully-qualified Table.Field of the predicate that computes this requirement, when one exists. */
  witness_field_name: string | null;
  /** TRUE when this requirement has at least once been scored as anything other than Satisfied. */
  has_ever_produced_negative: boolean | null;
  /** TRUE for a blocking control that HAS been evaluated at least once and has never returned a negative result. */
  is_unfalsified_control: boolean | null;
  /** TRUE when this requirement names a field it claims computes it. */
  claims_a_witness_field: boolean | null;
  /** TRUE when the field named in WitnessFieldName actually exists in the field catalog AND is a derived (calculated/lookup/aggregation) field. */
  named_witness_field_exists: boolean | null;
  /** Whether this requirement genuinely has a computed witness, derived from the field catalog rather than asserted. */
  derived_has_computed_witness: boolean | null;
  /** TRUE when the hand-typed HasComputedWitness flag disagrees with what the field catalog says. */
  witness_claim_is_unverified: boolean | null;
  /** TRUE for a blocking control with no verified computed witness behind it. */
  is_unwitnessed_blocking_control: boolean | null;
  /** How many times this requirement's evaluation has returned a non-Satisfied result. */
  witness_fire_count: number | null;
  /** TRUE for a requirement that has a computed witness which has never once returned a negative result. */
  witness_has_never_fired: boolean | null;
  /** How many times this requirement has been evaluated at all. */
  evaluation_sample_size: number | null;
  /** TRUE when this requirement has been evaluated often enough that a clean record is informative. */
  has_meaningful_sample: boolean | null;
  /** The number of clean evaluations this control must accumulate before its silence counts as evidence. */
  minimum_sample_for_assurance: number | null;
  /** TRUE for a witness that has never fired and has not been exercised enough for that silence to mean anything. */
  is_untested_witness: boolean | null;
  /** TRUE for a witness that has never fired across a sample large enough for that to constitute evidence. */
  is_evidenced_holding_control: boolean | null;
  /** One of Decorative, Inoperative, Asserted, Demonstrated, Holding, or Untested — the single control-health verdict for this requirement. */
  control_assurance_state: string | null;
  /** How many of this control's step bindings have never been evaluated. */
  unexercised_binding_count: number | null;
  /** TRUE when a control has a computed witness but at least one of its step bindings has never been exercised by it. */
  witness_is_partially_scoped: boolean | null;
  /** The role answerable for this control operating as designed. */
  accountable_role: string | null;
  /** The person currently holding the accountable role for this control. */
  accountable_agent: string | null;
  /** TRUE when a role has been named as accountable for this control. */
  has_named_owner: boolean | null;
  /** TRUE for a blocking control with nobody named as accountable for it. */
  is_orphaned_blocking_control: boolean | null;
  /** TRUE for a blocking control that nothing computes and nobody owns. */
  is_unwatched_and_unowned: boolean | null;
  /** States, per control, what kind of exposure attesting to it creates. */
  attestation_exposure_note: string | null;
  /** Constant marker echoed when this control is both unwatched and unowned. */
  unwatched_unowned_flag: string | null;
  /** Controlled vocabulary term this requirement's concept maps to, if any. */
  controlled_term: string | null;
  /** TRUE when this requirement points at a controlled term instead of only free-texting the concept. */
  uses_controlled_vocabulary: boolean | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const requirementsFieldTypes: Record<string, FieldType> = {
  requirement_id: "string",
  name: "*string",
  label: "*string",
  requirement_type: "*string",
  statement: "*string",
  rationale: "*string",
  is_blocking: "*bool",
  satisfaction_record_count: "*float64",
  step_binding_count: "*float64",
  is_bound_to_any_step: "*bool",
  has_ever_been_evaluated: "*bool",
  negative_outcome_count: "*float64",
  is_inoperative_control: "*bool",
  is_decorative_control: "*bool",
  has_computed_witness: "*bool",
  witness_field_name: "*string",
  has_ever_produced_negative: "*bool",
  is_unfalsified_control: "*bool",
  claims_a_witness_field: "*bool",
  named_witness_field_exists: "*bool",
  derived_has_computed_witness: "*bool",
  witness_claim_is_unverified: "*bool",
  is_unwitnessed_blocking_control: "*bool",
  witness_fire_count: "*float64",
  witness_has_never_fired: "*bool",
  evaluation_sample_size: "*float64",
  has_meaningful_sample: "*bool",
  minimum_sample_for_assurance: "*int",
  is_untested_witness: "*bool",
  is_evidenced_holding_control: "*bool",
  control_assurance_state: "*string",
  unexercised_binding_count: "*float64",
  witness_is_partially_scoped: "*bool",
  accountable_role: "*string",
  accountable_agent: "*string",
  has_named_owner: "*bool",
  is_orphaned_blocking_control: "*bool",
  is_unwatched_and_unowned: "*bool",
  attestation_exposure_note: "*string",
  unwatched_unowned_flag: "*string",
  controlled_term: "*string",
  uses_controlled_vocabulary: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Requirements row.
 *  Formula: ={{Label}} */
export function calcRequirementsName(tc: RequirementsRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes the IsBoundToAnyStep calculated field.
 *  TRUE when at least one step carries this requirement.
 *  Formula: ={{StepBindingCount}} > 0 */
export function calcRequirementsIsBoundToAnyStep(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.step_binding_count), ">", vI(0)));
}

/** Computes the HasEverBeenEvaluated calculated field.
 *  TRUE when this requirement has at least one satisfaction record in the model.
 *  Formula: ={{SatisfactionRecordCount}} > 0 */
export function calcRequirementsHasEverBeenEvaluated(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.satisfaction_record_count), ">", vI(0)));
}

/** Computes the IsInoperativeControl calculated field.
 *  TRUE for a blocking requirement that is attached to a step in the specification but has never once been evaluated on any execution.
 *  Formula: =AND({{IsBlocking}}, {{IsBoundToAnyStep}}, NOT({{HasEverBeenEvaluated}})) */
export function calcRequirementsIsInoperativeControl(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_blocking)), erbBool3(vBool(tc.is_bound_to_any_step)), erbBool3(erbNot(erbBool3(vBool(tc.has_ever_been_evaluated))))));
}

/** Computes the IsDecorativeControl calculated field.
 *  TRUE for a blocking requirement that is not attached to any step at all.
 *  Formula: =AND({{IsBlocking}}, NOT({{IsBoundToAnyStep}})) */
export function calcRequirementsIsDecorativeControl(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_blocking)), erbBool3(erbNot(erbBool3(vBool(tc.is_bound_to_any_step))))));
}

/** Computes the HasEverProducedNegative calculated field.
 *  TRUE when this requirement has at least once been scored as anything other than Satisfied.
 *  Formula: ={{NegativeOutcomeCount}} > 0 */
export function calcRequirementsHasEverProducedNegative(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.negative_outcome_count), ">", vI(0)));
}

/** Computes the IsUnfalsifiedControl calculated field.
 *  TRUE for a blocking control that HAS been evaluated at least once and has never returned a negative result.
 *  Formula: =AND({{IsBlocking}}, {{HasEverBeenEvaluated}}, NOT({{HasEverProducedNegative}})) */
export function calcRequirementsIsUnfalsifiedControl(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_blocking)), erbBool3(vBool(tc.has_ever_been_evaluated)), erbBool3(erbNot(erbBool3(vBool(tc.has_ever_produced_negative))))));
}

/** Computes the ClaimsAWitnessField calculated field.
 *  TRUE when this requirement names a field it claims computes it.
 *  Formula: ={{WitnessFieldName}} <> "" */
export function calcRequirementsClaimsAWitnessField(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.witness_field_name)));
}

/** Computes the DerivedHasComputedWitness calculated field.
 *  Whether this requirement genuinely has a computed witness, derived from the field catalog rather than asserted.
 *  Formula: =AND({{ClaimsAWitnessField}}, {{NamedWitnessFieldExists}}) */
export function calcRequirementsDerivedHasComputedWitness(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.claims_a_witness_field)), erbBool3(vBool(tc.named_witness_field_exists))));
}

/** Computes the WitnessClaimIsUnverified calculated field.
 *  TRUE when the hand-typed HasComputedWitness flag disagrees with what the field catalog says.
 *  Formula: =NOT({{HasComputedWitness}} = {{DerivedHasComputedWitness}}) */
export function calcRequirementsWitnessClaimIsUnverified(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(erbNullif(vBool(tc.has_computed_witness)), vBool(tc.derived_has_computed_witness)))));
}

/** Computes the IsUnwitnessedBlockingControl calculated field.
 *  TRUE for a blocking control with no verified computed witness behind it.
 *  Formula: =AND({{IsBlocking}}, NOT({{DerivedHasComputedWitness}})) */
export function calcRequirementsIsUnwitnessedBlockingControl(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_blocking)), erbBool3(erbNot(erbBool3(vBool(tc.derived_has_computed_witness))))));
}

/** Computes the WitnessFireCount calculated field.
 *  How many times this requirement's evaluation has returned a non-Satisfied result.
 *  Formula: ={{NegativeOutcomeCount}} */
export function calcRequirementsWitnessFireCount(tc: RequirementsRow): number | null {
  return toFloatPtr(vNum(tc.negative_outcome_count));
}

/** Computes the WitnessHasNeverFired calculated field.
 *  TRUE for a requirement that has a computed witness which has never once returned a negative result.
 *  Formula: =AND({{HasComputedWitness}}, {{WitnessFireCount}} = 0) */
export function calcRequirementsWitnessHasNeverFired(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.has_computed_witness)), erbBool3(erbEq(vNum(tc.witness_fire_count), vI(0)))));
}

/** Computes the EvaluationSampleSize calculated field.
 *  How many times this requirement has been evaluated at all.
 *  Formula: ={{SatisfactionRecordCount}} */
export function calcRequirementsEvaluationSampleSize(tc: RequirementsRow): number | null {
  return toFloatPtr(vNum(tc.satisfaction_record_count));
}

/** Computes the HasMeaningfulSample calculated field.
 *  TRUE when this requirement has been evaluated often enough that a clean record is informative.
 *  Formula: ={{EvaluationSampleSize}} >= {{MinimumSampleForAssurance}} */
export function calcRequirementsHasMeaningfulSample(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.evaluation_sample_size), ">=", erbNullif(vInt(tc.minimum_sample_for_assurance))));
}

/** Computes the IsUntestedWitness calculated field.
 *  TRUE for a witness that has never fired and has not been exercised enough for that silence to mean anything.
 *  Formula: =AND({{WitnessHasNeverFired}}, NOT({{HasMeaningfulSample}})) */
export function calcRequirementsIsUntestedWitness(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.witness_has_never_fired)), erbBool3(erbNot(erbBool3(vBool(tc.has_meaningful_sample))))));
}

/** Computes the IsEvidencedHoldingControl calculated field.
 *  TRUE for a witness that has never fired across a sample large enough for that to constitute evidence.
 *  Formula: =AND({{WitnessHasNeverFired}}, {{HasMeaningfulSample}}) */
export function calcRequirementsIsEvidencedHoldingControl(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.witness_has_never_fired)), erbBool3(vBool(tc.has_meaningful_sample))));
}

/** Computes the ControlAssuranceState calculated field.
 *  One of Decorative, Inoperative, Asserted, Demonstrated, Holding, or Untested — the single control-health verdict for this requirement.
 *  Formula: =IF(NOT({{IsBoundToAnyStep}}), "Decorative", IF(NOT({{HasEverBeenEvaluated}}), "Inoperative", IF(NOT({{HasComputedWitness}}), "Asserted", IF({{WitnessFireCount}} > 0, "Demonstrated", IF({{HasMeaningfulSample}}, "Holding", "Untested"))))) */
export function calcRequirementsControlAssuranceState(tc: RequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(vBool(tc.is_bound_to_any_step)))), () => vS("Decorative"), () => erbIf(erbBool3(erbNot(erbBool3(vBool(tc.has_ever_been_evaluated)))), () => vS("Inoperative"), () => erbIf(erbBool3(erbNot(erbIsTrue(vBool(tc.has_computed_witness)))), () => vS("Asserted"), () => erbIf(erbBool3(erbCmp(vNum(tc.witness_fire_count), ">", vI(0))), () => vS("Demonstrated"), () => erbIf(erbBool3(vBool(tc.has_meaningful_sample)), () => vS("Holding"), () => vS("Untested")))))));
}

/** Computes the WitnessIsPartiallyScoped calculated field.
 *  TRUE when a control has a computed witness but at least one of its step bindings has never been exercised by it.
 *  Formula: =AND({{HasComputedWitness}}, {{UnexercisedBindingCount}} > 0) */
export function calcRequirementsWitnessIsPartiallyScoped(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.has_computed_witness)), erbBool3(erbCmp(vNum(tc.unexercised_binding_count), ">", vI(0)))));
}

/** Computes the HasNamedOwner calculated field.
 *  TRUE when a role has been named as accountable for this control.
 *  Formula: ={{AccountableRole}} <> "" */
export function calcRequirementsHasNamedOwner(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.accountable_role)));
}

/** Computes the IsOrphanedBlockingControl calculated field.
 *  TRUE for a blocking control with nobody named as accountable for it.
 *  Formula: =AND({{IsBlocking}}, NOT({{HasNamedOwner}})) */
export function calcRequirementsIsOrphanedBlockingControl(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_blocking)), erbBool3(erbNot(erbBool3(vBool(tc.has_named_owner))))));
}

/** Computes the IsUnwatchedAndUnowned calculated field.
 *  TRUE for a blocking control that nothing computes and nobody owns.
 *  Formula: =AND({{IsBlocking}}, NOT({{HasComputedWitness}}), NOT({{HasNamedOwner}})) */
export function calcRequirementsIsUnwatchedAndUnowned(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_blocking)), erbBool3(erbNot(erbIsTrue(vBool(tc.has_computed_witness)))), erbBool3(erbNot(erbBool3(vBool(tc.has_named_owner))))));
}

/** Computes the AttestationExposureNote calculated field.
 *  States, per control, what kind of exposure attesting to it creates.
 *  Formula: =IF(NOT({{IsBlocking}}), "", IF({{IsUnwatchedAndUnowned}}, "Unwatched and unowned: exposure defaults to the signatory.", IF({{IsOrphanedBlockingControl}}, "Witnessed but unowned: no named accountability.", IF(NOT({{HasComputedWitness}}), "Owned but unwitnessed: rests on human judgement.", "")))) */
export function calcRequirementsAttestationExposureNote(tc: RequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbIsTrue(vBool(tc.is_blocking)))), () => vS(""), () => erbIf(erbBool3(vBool(tc.is_unwatched_and_unowned)), () => vS("Unwatched and unowned: exposure defaults to the signatory."), () => erbIf(erbBool3(vBool(tc.is_orphaned_blocking_control)), () => vS("Witnessed but unowned: no named accountability."), () => erbIf(erbBool3(erbNot(erbIsTrue(vBool(tc.has_computed_witness)))), () => vS("Owned but unwitnessed: rests on human judgement."), () => vS(""))))));
}

/** Computes the UnwatchedUnownedFlag calculated field.
 *  Constant marker echoed when this control is both unwatched and unowned.
 *  Formula: =IF({{IsUnwatchedAndUnowned}}, "unwatched-unowned", "") */
export function calcRequirementsUnwatchedUnownedFlag(tc: RequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unwatched_and_unowned)), () => vS("unwatched-unowned"), () => vS("")));
}

/** Computes the UsesControlledVocabulary calculated field.
 *  TRUE when this requirement points at a controlled term instead of only free-texting the concept.
 *  Formula: ={{ControlledTerm}} <> "" */
export function calcRequirementsUsesControlledVocabulary(tc: RequirementsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.controlled_term)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRequirements(tc: RequirementsRow): RequirementsRow {
  // Level 1
  calcGuard(tc, requirementsFieldTypes, "name", () => { tc.name = calcRequirementsName(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_bound_to_any_step", () => { tc.is_bound_to_any_step = calcRequirementsIsBoundToAnyStep(tc); });
  calcGuard(tc, requirementsFieldTypes, "has_ever_been_evaluated", () => { tc.has_ever_been_evaluated = calcRequirementsHasEverBeenEvaluated(tc); });
  calcGuard(tc, requirementsFieldTypes, "has_ever_produced_negative", () => { tc.has_ever_produced_negative = calcRequirementsHasEverProducedNegative(tc); });
  calcGuard(tc, requirementsFieldTypes, "claims_a_witness_field", () => { tc.claims_a_witness_field = calcRequirementsClaimsAWitnessField(tc); });
  calcGuard(tc, requirementsFieldTypes, "witness_fire_count", () => { tc.witness_fire_count = calcRequirementsWitnessFireCount(tc); });
  calcGuard(tc, requirementsFieldTypes, "evaluation_sample_size", () => { tc.evaluation_sample_size = calcRequirementsEvaluationSampleSize(tc); });
  calcGuard(tc, requirementsFieldTypes, "witness_is_partially_scoped", () => { tc.witness_is_partially_scoped = calcRequirementsWitnessIsPartiallyScoped(tc); });
  calcGuard(tc, requirementsFieldTypes, "has_named_owner", () => { tc.has_named_owner = calcRequirementsHasNamedOwner(tc); });
  calcGuard(tc, requirementsFieldTypes, "uses_controlled_vocabulary", () => { tc.uses_controlled_vocabulary = calcRequirementsUsesControlledVocabulary(tc); });
  // Level 2
  calcGuard(tc, requirementsFieldTypes, "is_inoperative_control", () => { tc.is_inoperative_control = calcRequirementsIsInoperativeControl(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_decorative_control", () => { tc.is_decorative_control = calcRequirementsIsDecorativeControl(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_unfalsified_control", () => { tc.is_unfalsified_control = calcRequirementsIsUnfalsifiedControl(tc); });
  calcGuard(tc, requirementsFieldTypes, "derived_has_computed_witness", () => { tc.derived_has_computed_witness = calcRequirementsDerivedHasComputedWitness(tc); });
  calcGuard(tc, requirementsFieldTypes, "witness_has_never_fired", () => { tc.witness_has_never_fired = calcRequirementsWitnessHasNeverFired(tc); });
  calcGuard(tc, requirementsFieldTypes, "has_meaningful_sample", () => { tc.has_meaningful_sample = calcRequirementsHasMeaningfulSample(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_orphaned_blocking_control", () => { tc.is_orphaned_blocking_control = calcRequirementsIsOrphanedBlockingControl(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_unwatched_and_unowned", () => { tc.is_unwatched_and_unowned = calcRequirementsIsUnwatchedAndUnowned(tc); });
  // Level 3
  calcGuard(tc, requirementsFieldTypes, "witness_claim_is_unverified", () => { tc.witness_claim_is_unverified = calcRequirementsWitnessClaimIsUnverified(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_unwitnessed_blocking_control", () => { tc.is_unwitnessed_blocking_control = calcRequirementsIsUnwitnessedBlockingControl(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_untested_witness", () => { tc.is_untested_witness = calcRequirementsIsUntestedWitness(tc); });
  calcGuard(tc, requirementsFieldTypes, "is_evidenced_holding_control", () => { tc.is_evidenced_holding_control = calcRequirementsIsEvidencedHoldingControl(tc); });
  calcGuard(tc, requirementsFieldTypes, "control_assurance_state", () => { tc.control_assurance_state = calcRequirementsControlAssuranceState(tc); });
  calcGuard(tc, requirementsFieldTypes, "attestation_exposure_note", () => { tc.attestation_exposure_note = calcRequirementsAttestationExposureNote(tc); });
  calcGuard(tc, requirementsFieldTypes, "unwatched_unowned_flag", () => { tc.unwatched_unowned_flag = calcRequirementsUnwatchedUnownedFlag(tc); });
  return tc;
}

/** Reads Requirements rows from a JSON array file. */
export function loadRequirementsRows(file: string): RequirementsRow[] {
  return loadRows(file, { fields: requirementsFieldTypes }) as unknown as RequirementsRow[];
}

// =============================================================================
// STEPREQUIREMENTS TABLE
// Many-to-many Step/Requirement semantics normalized into an ERB junction table.
// =============================================================================

/** A row in the StepRequirements table. */
export interface StepRequirementsRow {
  /** Stored logical identifier for one StepRequirements row. */
  step_requirement_id: string;
  /** Human-readable calculated display alias for the StepRequirements row. */
  name: string | null;
  /** Step side of the relationship. */
  step: string | null;
  /** Requirement side of the relationship. */
  requirement: string | null;
  /** Whether this spec-side step/requirement binding names a blocking control. */
  requirement_is_blocking: boolean | null;
  /** Echoes the Step id only when the bound requirement is blocking; empty otherwise. */
  blocking_step_key: string | null;
  /** Echoes the step id when the bound requirement is blocking, blank otherwise. */
  step_when_blocking: string | null;
  /** Whether the requirement bound at this step is a blocking control with no computed witness. */
  requirement_lacks_witness: boolean | null;
  /** Echoes the step id when the requirement bound here has no computed witness. */
  unwitnessed_step_key: string | null;
  /** How many times this specific (step, requirement) binding has been evaluated in any execution. */
  satisfaction_count_for_binding: number | null;
  /** TRUE when this specific (step, requirement) pair has been evaluated at least once. */
  binding_was_ever_exercised: boolean | null;
  /** TRUE for a blocking control bound to a step where it has never actually been evaluated. */
  is_unexercised_blocking_binding: boolean | null;
  /** Echoes the requirement id when this binding has never been exercised. */
  unexercised_binding_requirement_key: string | null;
  _erb_errors?: Record<string, string>;
}

const stepRequirementsFieldTypes: Record<string, FieldType> = {
  step_requirement_id: "string",
  name: "*string",
  step: "*string",
  requirement: "*string",
  requirement_is_blocking: "*bool",
  blocking_step_key: "*string",
  step_when_blocking: "*string",
  requirement_lacks_witness: "*bool",
  unwitnessed_step_key: "*string",
  satisfaction_count_for_binding: "*float64",
  binding_was_ever_exercised: "*bool",
  is_unexercised_blocking_binding: "*bool",
  unexercised_binding_requirement_key: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepRequirements row.
 *  Formula: ={{Step}} & " / " & {{Requirement}} */
export function calcStepRequirementsName(tc: StepRequirementsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS(" / "), erbTextOr(vStr(tc.requirement))));
}

/** Computes the BlockingStepKey calculated field.
 *  Echoes the Step id only when the bound requirement is blocking; empty otherwise.
 *  Formula: =IF({{RequirementIsBlocking}}, {{Step}}, "") */
export function calcStepRequirementsBlockingStepKey(tc: StepRequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.requirement_is_blocking)), () => vStr(tc.step), () => vS("")));
}

/** Computes the StepWhenBlocking calculated field.
 *  Echoes the step id when the bound requirement is blocking, blank otherwise.
 *  Formula: =IF({{RequirementIsBlocking}}, {{Step}}, "") */
export function calcStepRequirementsStepWhenBlocking(tc: StepRequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.requirement_is_blocking)), () => vStr(tc.step), () => vS("")));
}

/** Computes the UnwitnessedStepKey calculated field.
 *  Echoes the step id when the requirement bound here has no computed witness.
 *  Formula: =IF({{RequirementLacksWitness}}, {{Step}}, "") */
export function calcStepRequirementsUnwitnessedStepKey(tc: StepRequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.requirement_lacks_witness)), () => vStr(tc.step), () => vS("")));
}

/** Computes the BindingWasEverExercised calculated field.
 *  TRUE when this specific (step, requirement) pair has been evaluated at least once.
 *  Formula: ={{SatisfactionCountForBinding}} > 0 */
export function calcStepRequirementsBindingWasEverExercised(tc: StepRequirementsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.satisfaction_count_for_binding), ">", vI(0)));
}

/** Computes the IsUnexercisedBlockingBinding calculated field.
 *  TRUE for a blocking control bound to a step where it has never actually been evaluated.
 *  Formula: =AND({{RequirementIsBlocking}}, NOT({{BindingWasEverExercised}})) */
export function calcStepRequirementsIsUnexercisedBlockingBinding(tc: StepRequirementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.requirement_is_blocking)), erbBool3(erbNot(erbBool3(vBool(tc.binding_was_ever_exercised))))));
}

/** Computes the UnexercisedBindingRequirementKey calculated field.
 *  Echoes the requirement id when this binding has never been exercised.
 *  Formula: =IF({{IsUnexercisedBlockingBinding}}, {{Requirement}}, "") */
export function calcStepRequirementsUnexercisedBindingRequirementKey(tc: StepRequirementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unexercised_blocking_binding)), () => vStr(tc.requirement), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepRequirements(tc: StepRequirementsRow): StepRequirementsRow {
  // Level 1
  calcGuard(tc, stepRequirementsFieldTypes, "name", () => { tc.name = calcStepRequirementsName(tc); });
  calcGuard(tc, stepRequirementsFieldTypes, "blocking_step_key", () => { tc.blocking_step_key = calcStepRequirementsBlockingStepKey(tc); });
  calcGuard(tc, stepRequirementsFieldTypes, "step_when_blocking", () => { tc.step_when_blocking = calcStepRequirementsStepWhenBlocking(tc); });
  calcGuard(tc, stepRequirementsFieldTypes, "unwitnessed_step_key", () => { tc.unwitnessed_step_key = calcStepRequirementsUnwitnessedStepKey(tc); });
  calcGuard(tc, stepRequirementsFieldTypes, "binding_was_ever_exercised", () => { tc.binding_was_ever_exercised = calcStepRequirementsBindingWasEverExercised(tc); });
  // Level 2
  calcGuard(tc, stepRequirementsFieldTypes, "is_unexercised_blocking_binding", () => { tc.is_unexercised_blocking_binding = calcStepRequirementsIsUnexercisedBlockingBinding(tc); });
  // Level 3
  calcGuard(tc, stepRequirementsFieldTypes, "unexercised_binding_requirement_key", () => { tc.unexercised_binding_requirement_key = calcStepRequirementsUnexercisedBindingRequirementKey(tc); });
  return tc;
}

/** Reads StepRequirements rows from a JSON array file. */
export function loadStepRequirementsRows(file: string): StepRequirementsRow[] {
  return loadRows(file, { fields: stepRequirementsFieldTypes }) as unknown as StepRequirementsRow[];
}

// =============================================================================
// STEPVERIFICATIONS TABLE
// Verification definitions attached to steps. Maps to pko:StepVerification and pko:SignalVerification.
// =============================================================================

/** A row in the StepVerifications table. */
export interface StepVerificationsRow {
  /** Stored logical identifier for one StepVerifications row. */
  step_verification_id: string;
  /** Human-readable calculated display alias for the StepVerifications row. */
  name: string | null;
  /** Step whose execution is verified; maps to pko:hasStepVerification. */
  step: string | null;
  /** SignalVerification, ApprovalVerification, ProvenanceVerification, or another documented kind. */
  verification_kind: string | null;
  /** Signal or evidence key; maps to pko:signalIdentifier for signal verifications. */
  signal_identifier: string | null;
  /** Expected value; maps to pko:expectedSignalValue. */
  expected_signal_value: string | null;
  /** How to verify the step. */
  instruction: string | null;
  /** Exact PKO or extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const stepVerificationsFieldTypes: Record<string, FieldType> = {
  step_verification_id: "string",
  name: "*string",
  step: "*string",
  verification_kind: "*string",
  signal_identifier: "*string",
  expected_signal_value: "*string",
  instruction: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepVerifications row.
 *  Formula: ={{Step}} & " / " & {{VerificationKind}} */
export function calcStepVerificationsName(tc: StepVerificationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS(" / "), erbTextOr(vStr(tc.verification_kind))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepVerifications(tc: StepVerificationsRow): StepVerificationsRow {
  // Level 1
  calcGuard(tc, stepVerificationsFieldTypes, "name", () => { tc.name = calcStepVerificationsName(tc); });
  return tc;
}

/** Reads StepVerifications rows from a JSON array file. */
export function loadStepVerificationsRows(file: string): StepVerificationsRow[] {
  return loadRows(file, { fields: stepVerificationsFieldTypes }) as unknown as StepVerificationsRow[];
}

// =============================================================================
// RATIONALES TABLE
// First-class rationale statements explaining why procedural commitments and design decisions exist. Explicit ERB-PKO extension, represented as prov:Entity/dcat:Resource in projections.
// =============================================================================

/** A row in the Rationales table. */
export interface RationalesRow {
  /** Stored logical identifier for one Rationales row. */
  rationale_id: string;
  /** Human-readable calculated display alias for the Rationales row. */
  name: string | null;
  /** Version justified by the rationale. */
  procedure_version: string | null;
  /** Optional step justified by the rationale. */
  step: string | null;
  /** Rationale title. */
  title: string | null;
  /** Reasoned explanation. */
  statement: string | null;
  /** Draft, Reviewed, Approved, or Superseded. */
  status: string | null;
  /** Role authorized to approve the rationale. */
  authority_role: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const rationalesFieldTypes: Record<string, FieldType> = {
  rationale_id: "string",
  name: "*string",
  procedure_version: "*string",
  step: "*string",
  title: "*string",
  statement: "*string",
  status: "*string",
  authority_role: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Rationales row.
 *  Formula: ={{Title}} */
export function calcRationalesName(tc: RationalesRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRationales(tc: RationalesRow): RationalesRow {
  // Level 1
  calcGuard(tc, rationalesFieldTypes, "name", () => { tc.name = calcRationalesName(tc); });
  return tc;
}

/** Reads Rationales rows from a JSON array file. */
export function loadRationalesRows(file: string): RationalesRow[] {
  return loadRows(file, { fields: rationalesFieldTypes }) as unknown as RationalesRow[];
}

// =============================================================================
// EXCEPTIONS TABLE
// Documented exceptions, fallbacks, and alternative handling. Aligns structurally with PKO fallback/alternative steps and requirements; the exception record itself is an ERB-PKO extension.
// =============================================================================

/** A row in the Exceptions table. */
export interface ExceptionsRow {
  /** Stored logical identifier for one Exceptions row. */
  exception_id: string;
  /** Human-readable calculated display alias for the Exceptions row. */
  name: string | null;
  /** Procedure version containing the exception. */
  procedure_version: string | null;
  /** Step at which the exception becomes relevant. */
  trigger_step: string | null;
  /** Exception condition. */
  condition: string | null;
  /** Required handling and guardrails. */
  handling: string | null;
  /** Role that approves use of the exception. */
  approval_role: string | null;
  /** Role or function that executes fallback handling. */
  fallback_role: string | null;
  /** Active, Draft, Retired, or Superseded. */
  status: string | null;
  /** Echoes the TriggerStep id only for exceptions currently in Active status. */
  active_exception_step_key: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const exceptionsFieldTypes: Record<string, FieldType> = {
  exception_id: "string",
  name: "*string",
  procedure_version: "*string",
  trigger_step: "*string",
  condition: "*string",
  handling: "*string",
  approval_role: "*string",
  fallback_role: "*string",
  status: "*string",
  active_exception_step_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Exceptions row.
 *  Formula: ={{Condition}} */
export function calcExceptionsName(tc: ExceptionsRow): string | null {
  return toStringPtr(vStr(tc.condition));
}

/** Computes the ActiveExceptionStepKey calculated field.
 *  Echoes the TriggerStep id only for exceptions currently in Active status.
 *  Formula: =IF({{Status}} = "Active", {{TriggerStep}}, "") */
export function calcExceptionsActiveExceptionStepKey(tc: ExceptionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Active"))), () => vStr(tc.trigger_step), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeExceptions(tc: ExceptionsRow): ExceptionsRow {
  // Level 1
  calcGuard(tc, exceptionsFieldTypes, "name", () => { tc.name = calcExceptionsName(tc); });
  calcGuard(tc, exceptionsFieldTypes, "active_exception_step_key", () => { tc.active_exception_step_key = calcExceptionsActiveExceptionStepKey(tc); });
  return tc;
}

/** Reads Exceptions rows from a JSON array file. */
export function loadExceptionsRows(file: string): ExceptionsRow[] {
  return loadRows(file, { fields: exceptionsFieldTypes }) as unknown as ExceptionsRow[];
}

// =============================================================================
// RESOURCES TABLE
// Documents, datasets, APIs, templates, images, manuals, and operational records referenced by procedures. Maps to dcat:Resource.
// =============================================================================

/** A row in the Resources table. */
export interface ResourcesRow {
  /** Stored logical identifier for one Resources row. */
  resource_id: string;
  /** Human-readable calculated display alias for the Resources row. */
  name: string | null;
  /** Resource title; maps to dcterms:title. */
  title: string | null;
  /** Document, Dataset, API, Template, OperationalRecord, Image, Video, or another declared type. */
  resource_kind: string | null;
  /** Resolvable or organization-internal resource identifier. */
  external_uri: string | null;
  /** Creation time; maps to dcterms:created. */
  created_at: string | null;
  /** Modification time; maps to dcterms:modified. */
  modified_at: string | null;
  /** Resource description; maps to dcterms:description. */
  description: string | null;
  /** Approved, Draft, Deprecated, or Unvetted. */
  approval_status: string | null;
  /** TRUE when this resource is an approved source. */
  is_approved_source: boolean | null;
  /** Exact semantic class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const resourcesFieldTypes: Record<string, FieldType> = {
  resource_id: "string",
  name: "*string",
  title: "*string",
  resource_kind: "*string",
  external_uri: "*string",
  created_at: "*string",
  modified_at: "*string",
  description: "*string",
  approval_status: "*string",
  is_approved_source: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Resources row.
 *  Formula: ={{Title}} */
export function calcResourcesName(tc: ResourcesRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes the IsApprovedSource calculated field.
 *  TRUE when this resource is an approved source.
 *  Formula: ={{ApprovalStatus}} = "Approved" */
export function calcResourcesIsApprovedSource(tc: ResourcesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.approval_status)), vS("Approved")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeResources(tc: ResourcesRow): ResourcesRow {
  // Level 1
  calcGuard(tc, resourcesFieldTypes, "name", () => { tc.name = calcResourcesName(tc); });
  calcGuard(tc, resourcesFieldTypes, "is_approved_source", () => { tc.is_approved_source = calcResourcesIsApprovedSource(tc); });
  return tc;
}

/** Reads Resources rows from a JSON array file. */
export function loadResourcesRows(file: string): ResourcesRow[] {
  return loadRows(file, { fields: resourcesFieldTypes }) as unknown as ResourcesRow[];
}

// =============================================================================
// PROCEDURERESOURCES TABLE
// Links versioned procedures to supporting resources using PKO/dcterms provenance relations.
// =============================================================================

/** A row in the ProcedureResources table. */
export interface ProcedureResourcesRow {
  /** Stored logical identifier for one ProcedureResources row. */
  procedure_resource_id: string;
  /** Human-readable calculated display alias for the ProcedureResources row. */
  name: string | null;
  /** Procedure version. */
  procedure_version: string | null;
  /** Referenced or source resource. */
  resource: string | null;
  /** wasExtractedFrom, references, generated, used, or another declared relation. */
  relation: string | null;
  /** Exact semantic property IRI for the relation. */
  relation_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const procedureResourcesFieldTypes: Record<string, FieldType> = {
  procedure_resource_id: "string",
  name: "*string",
  procedure_version: "*string",
  resource: "*string",
  relation: "*string",
  relation_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcedureResources row.
 *  Formula: ={{ProcedureVersion}} & " / " & {{Resource}} */
export function calcProcedureResourcesName(tc: ProcedureResourcesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_version)), vS(" / "), erbTextOr(vStr(tc.resource))));
}

/** Computes the RelationIri calculated field.
 *  Exact semantic property IRI for the relation.
 *  Formula: =IF({{Relation}} = "wasExtractedFrom", "https://w3id.org/pko#wasExtractedFrom", "http://purl.org/dc/terms/references") */
export function calcProcedureResourcesRelationIri(tc: ProcedureResourcesRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(erbNullif(vStr(tc.relation)), vS("wasExtractedFrom"))), () => vS("https://w3id.org/pko#wasExtractedFrom"), () => vS("http://purl.org/dc/terms/references")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedureResources(tc: ProcedureResourcesRow): ProcedureResourcesRow {
  // Level 1
  calcGuard(tc, procedureResourcesFieldTypes, "name", () => { tc.name = calcProcedureResourcesName(tc); });
  calcGuard(tc, procedureResourcesFieldTypes, "relation_iri", () => { tc.relation_iri = calcProcedureResourcesRelationIri(tc); });
  return tc;
}

/** Reads ProcedureResources rows from a JSON array file. */
export function loadProcedureResourcesRows(file: string): ProcedureResourcesRow[] {
  return loadRows(file, { fields: procedureResourcesFieldTypes }) as unknown as ProcedureResourcesRow[];
}

// =============================================================================
// ELICITATIONSESSIONS TABLE
// Structured knowledge-elicitation events involving practitioners and knowledge engineers. Explicit ERB-PKO extension, modeled as prov:Activity.
// =============================================================================

/** A row in the ElicitationSessions table. */
export interface ElicitationSessionsRow {
  /** Stored logical identifier for one ElicitationSessions row. */
  elicitation_session_id: string;
  /** Human-readable calculated display alias for the ElicitationSessions row. */
  name: string | null;
  /** Procedure version whose knowledge was elicited. */
  procedure_version: string | null;
  /** Interview, shadowing, workshop, observation, document analysis, or another method. */
  method: string | null;
  /** Start time; maps to prov:startedAtTime. */
  started_at: string | null;
  /** End time; maps to prov:endedAtTime. */
  ended_at: string | null;
  /** Domain practitioner providing knowledge. */
  practitioner_agent: string | null;
  /** Person facilitating elicitation. */
  facilitator_agent: string | null;
  /** What was learned and captured. */
  summary: string | null;
  /** Draft, Reviewed, Approved, or Rejected. */
  status: string | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** Days elapsed since this elicitation session concluded. */
  days_since_elicited: number | null;
  /** TRUE when this session captured one practitioner's account rather than a group's. */
  is_single_witness_method: boolean | null;
  /** Whether the practitioner whose knowledge this session captured still holds a role here. */
  practitioner_is_still_engaged: boolean | null;
  /** How many currently-valid knowledge fragments this one session produced. */
  valid_fragments_produced: number | null;
  /** A session that alone underwrites three or more currently-valid claims. */
  is_high_yield_session: boolean | null;
  /** One unrepeated session with one witness that underwrites three or more live claims. */
  is_concentrated_single_witness: boolean | null;
  /** A concentrated single-witness session more than 180 days old — matching the single-witness expiry horizon loop 1 already established. */
  is_stale_concentrated_witness: boolean | null;
  /** Composite-key echo: this session's procedure version when the session is a concentrated single witness, blank otherwise. */
  concentrated_session_version_key: string | null;
  /** Class IRI used in semantic projection. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const elicitationSessionsFieldTypes: Record<string, FieldType> = {
  elicitation_session_id: "string",
  name: "*string",
  procedure_version: "*string",
  method: "*string",
  started_at: "*string",
  ended_at: "*string",
  practitioner_agent: "*string",
  facilitator_agent: "*string",
  summary: "*string",
  status: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  days_since_elicited: "*int",
  is_single_witness_method: "*bool",
  practitioner_is_still_engaged: "*bool",
  valid_fragments_produced: "*float64",
  is_high_yield_session: "*bool",
  is_concentrated_single_witness: "*bool",
  is_stale_concentrated_witness: "*bool",
  concentrated_session_version_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ElicitationSessions row.
 *  Formula: ={{Method}} & " / " & {{StartedAt}} */
export function calcElicitationSessionsName(tc: ElicitationSessionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.method)), vS(" / "), erbTimestamptzText(vStr(tc.started_at))));
}

/** Computes the DaysSinceElicited calculated field.
 *  Days elapsed since this elicitation session concluded.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{EndedAt}}, "days") */
export function calcElicitationSessionsDaysSinceElicited(tc: ElicitationSessionsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.ended_at), vS("days"))));
}

/** Computes the IsSingleWitnessMethod calculated field.
 *  TRUE when this session captured one practitioner's account rather than a group's.
 *  Formula: =OR({{Method}} = "Shadowing", {{Method}} = "PractitionerInterview") */
export function calcElicitationSessionsIsSingleWitnessMethod(tc: ElicitationSessionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.method)), vS("Shadowing"))), erbBool3(erbEq(erbNullif(vStr(tc.method)), vS("PractitionerInterview")))));
}

/** Computes the IsHighYieldSession calculated field.
 *  A session that alone underwrites three or more currently-valid claims.
 *  Formula: ={{ValidFragmentsProduced}} >= 3 */
export function calcElicitationSessionsIsHighYieldSession(tc: ElicitationSessionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.valid_fragments_produced), ">=", vI(3)));
}

/** Computes the IsConcentratedSingleWitness calculated field.
 *  One unrepeated session with one witness that underwrites three or more live claims.
 *  Formula: =AND({{IsSingleWitnessMethod}}, {{IsHighYieldSession}}) */
export function calcElicitationSessionsIsConcentratedSingleWitness(tc: ElicitationSessionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_single_witness_method)), erbBool3(vBool(tc.is_high_yield_session))));
}

/** Computes the IsStaleConcentratedWitness calculated field.
 *  A concentrated single-witness session more than 180 days old — matching the single-witness expiry horizon loop 1 already established.
 *  Formula: =AND({{IsConcentratedSingleWitness}}, {{DaysSinceElicited}} > 180) */
export function calcElicitationSessionsIsStaleConcentratedWitness(tc: ElicitationSessionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_concentrated_single_witness)), erbBool3(erbCmp(vInt(tc.days_since_elicited), ">", vI(180)))));
}

/** Computes the ConcentratedSessionVersionKey calculated field.
 *  Composite-key echo: this session's procedure version when the session is a concentrated single witness, blank otherwise.
 *  Formula: =IF({{IsConcentratedSingleWitness}}, {{ProcedureVersion}}, "") */
export function calcElicitationSessionsConcentratedSessionVersionKey(tc: ElicitationSessionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_concentrated_single_witness)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeElicitationSessions(tc: ElicitationSessionsRow): ElicitationSessionsRow {
  // Level 1
  calcGuard(tc, elicitationSessionsFieldTypes, "name", () => { tc.name = calcElicitationSessionsName(tc); });
  calcGuard(tc, elicitationSessionsFieldTypes, "days_since_elicited", () => { tc.days_since_elicited = calcElicitationSessionsDaysSinceElicited(tc); });
  calcGuard(tc, elicitationSessionsFieldTypes, "is_single_witness_method", () => { tc.is_single_witness_method = calcElicitationSessionsIsSingleWitnessMethod(tc); });
  calcGuard(tc, elicitationSessionsFieldTypes, "is_high_yield_session", () => { tc.is_high_yield_session = calcElicitationSessionsIsHighYieldSession(tc); });
  // Level 2
  calcGuard(tc, elicitationSessionsFieldTypes, "is_concentrated_single_witness", () => { tc.is_concentrated_single_witness = calcElicitationSessionsIsConcentratedSingleWitness(tc); });
  // Level 3
  calcGuard(tc, elicitationSessionsFieldTypes, "is_stale_concentrated_witness", () => { tc.is_stale_concentrated_witness = calcElicitationSessionsIsStaleConcentratedWitness(tc); });
  calcGuard(tc, elicitationSessionsFieldTypes, "concentrated_session_version_key", () => { tc.concentrated_session_version_key = calcElicitationSessionsConcentratedSessionVersionKey(tc); });
  return tc;
}

/** Reads ElicitationSessions rows from a JSON array file. */
export function loadElicitationSessionsRows(file: string): ElicitationSessionsRow[] {
  return loadRows(file, { fields: elicitationSessionsFieldTypes }) as unknown as ElicitationSessionsRow[];
}

// =============================================================================
// KNOWLEDGEFRAGMENTS TABLE
// Explicit records of tacit, implicit, explicit, and situated procedural knowledge. This is an ERB-PKO extension represented as provenance-bearing dcat:Resource instances.
// =============================================================================

/** A row in the KnowledgeFragments table. */
export interface KnowledgeFragmentsRow {
  /** Stored logical identifier for one KnowledgeFragments row. */
  knowledge_fragment_id: string;
  /** Human-readable calculated display alias for the KnowledgeFragments row. */
  name: string | null;
  /** Procedure version informed by the fragment. */
  procedure_version: string | null;
  /** Optional step to which the fragment applies. */
  step: string | null;
  /** Tacit, Implicit, Explicit, SituatedJudgment, LessonLearned, or another declared form. */
  knowledge_form: string | null;
  /** Captured knowledge statement. */
  statement: string | null;
  /** Optional elicitation session that produced the fragment. */
  elicitation_session: string | null;
  /** Practitioner or source agent. */
  source_agent: string | null;
  /** Low, Medium, or High confidence. */
  confidence: string | null;
  /** Start of the fragment's valid-time interval. */
  valid_from: string | null;
  /** End of the fragment's valid-time interval. */
  valid_to: string | null;
  /** Draft, Reviewed, Approved, Rejected, or Superseded. */
  status: string | null;
  /** Role accountable for maintaining the fragment. */
  owner_role: string | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** TRUE when the fragment is approved and valid now. */
  is_currently_valid: boolean | null;
  /** Whether the agent who is the source of this claim still holds a role here. */
  source_agent_is_still_engaged: boolean | null;
  /** Whether the source of this claim is a Human, an AIAgent, or an AutomatedPipeline. */
  source_agent_kind: string | null;
  /** TRUE when the claim originates from a human practitioner rather than software. */
  has_human_source: boolean | null;
  /** TRUE when we still rely on this claim but the agent who gave it to us no longer holds a role here. */
  has_orphaned_provenance: boolean | null;
  /** TRUE when an orphaned claim is of a kind that lives in a person's head rather than in a document. */
  is_undefendable_tacit_claim: boolean | null;
  /** TRUE when this claim has passed knowledge-authority approval. */
  is_approved: boolean | null;
  /** TRUE when this claim's stated validity window contains the present moment. */
  is_within_validity_window: boolean | null;
  /** TRUE when this claim is attached to a specific step and is inside its validity window — i.e. it is operationally in play. */
  is_relied_upon: boolean | null;
  /** The procedure version owning the step this claim is attached to. */
  step_procedure_version_status: string | null;
  /** Whether the procedure version this claim belongs to is currently executable. */
  is_attached_to_live_version: boolean | null;
  /** TRUE when a live procedure is acting on a claim that has not been approved by the knowledge authority. */
  is_unapproved_but_relied_on: boolean | null;
  /** Age in days of the elicitation session this claim came from. */
  evidence_age_days: number | null;
  /** TRUE when this claim traces to a recorded elicitation session. */
  has_recorded_elicitation: boolean | null;
  /** Whether this claim rests on a single practitioner's account. */
  is_from_single_witness: boolean | null;
  /** How many days this claim's evidence is trusted for, given how it was gathered. */
  evidence_expiry_days: number | null;
  /** TRUE when the evidence behind this claim is older than we trust evidence of its kind to be. */
  evidence_has_expired: boolean | null;
  /** The agent currently holding the role that owns this claim. */
  owner_agent: string | null;
  /** TRUE when this claim has been reviewed but not yet approved. */
  is_awaiting_approval: boolean | null;
  /** TRUE when the People Policy Owner role owns this claim. */
  owner_is_me: boolean | null;
  /** TRUE when a claim I own has been reviewed and is waiting on my approval. */
  is_my_unfinished_approval: boolean | null;
  /** How many documented exception handlers trigger on the same step this claim is attached to. */
  is_invoked_by_an_exception: number | null;
  /** TRUE when an exception handler exists on the step this claim governs. */
  has_operational_reliance: boolean | null;
  /** TRUE when a claim awaiting my approval is already being acted on through a documented exception path. */
  is_unapproved_and_operationally_live: boolean | null;
  /** Days since this claim became valid. */
  age_days: number | null;
  /** TRUE when this claim was recorded with less than high confidence. */
  is_low_confidence: boolean | null;
  /** The review cadence promised for the version this claim supports. */
  owning_version_cadence_days: number | null;
  /** TRUE when this claim is older than the review cadence promised for the version it supports. */
  exceeds_owning_cadence: boolean | null;
  /** TRUE when a claim we were never sure about has also outlived its version's review cadence. */
  is_aging_low_confidence_claim: boolean | null;
  /** Agent kind of whoever currently holds the role that owns this knowledge fragment. */
  owner_role_agent_kind: string | null;
  /** TRUE when the owning role is currently held by a human. */
  is_human_owned: boolean | null;
  /** TRUE when knowledge came from a non-human source AND is owned by a non-human-held role. */
  is_ai_validated_by_ai: boolean | null;
  /** The review cadence promised for the procedure version this fragment supports, resolved via ProcedureVersions because INDEX/MATCH only matches a target primary key. */
  review_cadence_days: number | null;
  /** TRUE when a currently-valid fragment has gone longer than its stewardship cadence without review. */
  is_overdue_for_review: boolean | null;
  /** TRUE when this knowledge became valid before the current holder of its owning role took the role. */
  predates_current_role_holder: boolean | null;
  /** When the current holder of the owning role took that role. */
  owner_role_assignment_valid_from: string | null;
  /** When this fragment was last actually reviewed and reaffirmed by its owning role. Null when it has never been reviewed since authoring. IsOverdueForReview infers recency from ValidFrom, which records when the claim became TRUE, not when anyone last looked at it. Those are different events, and the inference is deliberately left in place under its own name rather than silently rewritten to fall back on this column. */
  last_reviewed_at: string | null;
  /** How many of the four loop-1 decay signals are simultaneously true for this fragment: single witness, overdue for review, low confidence, operational reliance. */
  fragility_signal_count: number | null;
  /** A fragment carrying at least three of the four decay signals at once. */
  is_compound_fragile: boolean | null;
  /** A claim that rests on exactly one person's word and that an active exception handler actually routes cases against. */
  is_single_point_of_failure: boolean | null;
  /** A single-sourced, operationally relied-upon claim that is also past its review date. */
  is_expiring_single_point_of_failure: boolean | null;
  /** Composite-key echo: this fragment's procedure version when the fragment is compound-fragile, blank otherwise. */
  compound_fragile_version_key: string | null;
  /** Composite-key echo: the elicitation session behind this fragment when the fragment is currently valid, blank otherwise. */
  valid_fragment_session_key: string | null;
  /** Whether the step that consumes this fragment is assigned to an AI agent or an automated pipeline. */
  consuming_step_is_software_assigned: boolean | null;
  /** The kind of agent currently holding the role assigned to the step that consumes this fragment. */
  consuming_step_agent_kind: string | null;
  /** An unapproved claim that a software-assigned step actually relies on — executed literally, with no human in position to notice it is wrong. */
  is_unapproved_and_machine_consumed: boolean | null;
  /** An unapproved claim relied on by a human-assigned step — a reviewable risk rather than a silent one. */
  is_unapproved_and_human_consumed: boolean | null;
  /** Composite-key echo: this fragment's procedure version when the fragment is unapproved and machine-consumed, blank otherwise. */
  machine_consumed_unapproved_version_key: string | null;
  /** Whether this fragment has ever had an actual review recorded, as opposed to merely having a ValidFrom date. */
  has_review_record: boolean | null;
  /** Days elapsed since this fragment was last actually reviewed. Zero when no review has ever been recorded — read this only alongside HasReviewRecord, never on its own. */
  days_since_actual_review: number | null;
  /** A currently-valid claim that nobody has ever reviewed since it was written. */
  is_unreviewed_since_authoring: boolean | null;
  /** A valid claim whose LAST ACTUAL REVIEW is older than the cadence its owning version promised. */
  is_genuinely_overdue: boolean | null;
  /** A fragment reported overdue by the loop-1 inference purely because no review has ever been recorded — the number is an artifact of missing data, not evidence of neglect. */
  review_recency_is_inferred: boolean | null;
  /** A fragment the ValidFrom inference calls overdue but which was in fact reviewed inside its cadence — a false positive in the loop-1 predicate, now provable. */
  inference_disagrees_with_record: boolean | null;
  /** Composite-key echo: this fragment's procedure version when the fragment is genuinely overdue, blank otherwise. */
  genuinely_overdue_version_key: string | null;
  /** How many currently-binding authority boundaries this fragment ratifies. */
  ratified_boundary_count: number | null;
  /** Total number of distinct downstream dependents on this claim: exception handlers plus ratified authority boundaries. */
  reliance_surface_count: number | null;
  /** How long a claim of mine has been sitting at Reviewed without my approval. Zero when the claim is not mine or is already decided. */
  days_awaiting_my_approval: number | null;
  /** An unapproved, operationally live claim of mine with more than one distinct downstream dependent. */
  is_high_blast_radius_unapproved: boolean | null;
  /** A claim that has waited on my signature for more than thirty days. */
  is_long_unapproved: boolean | null;
  /** Composite-key echo: this fragment's procedure version when it is a high-blast-radius unapproved claim, blank otherwise. */
  unapproved_load_bearing_version_key: string | null;
  /** Whether the role that owns this claim is currently vacated. */
  owner_role_is_vacated: boolean | null;
  /** A currently-valid claim whose owning role nobody holds — accountable to a vacancy. */
  is_orphaned_by_role: boolean | null;
  /** Composite-key echo: this fragment's owning procedure version when the fragment is currently valid, blank otherwise. */
  valid_fragment_version_key: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const knowledgeFragmentsFieldTypes: Record<string, FieldType> = {
  knowledge_fragment_id: "string",
  name: "*string",
  procedure_version: "*string",
  step: "*string",
  knowledge_form: "*string",
  statement: "*string",
  elicitation_session: "*string",
  source_agent: "*string",
  confidence: "*string",
  valid_from: "*string",
  valid_to: "*string",
  status: "*string",
  owner_role: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  is_currently_valid: "*bool",
  source_agent_is_still_engaged: "*bool",
  source_agent_kind: "*string",
  has_human_source: "*bool",
  has_orphaned_provenance: "*bool",
  is_undefendable_tacit_claim: "*bool",
  is_approved: "*bool",
  is_within_validity_window: "*bool",
  is_relied_upon: "*bool",
  step_procedure_version_status: "*string",
  is_attached_to_live_version: "*bool",
  is_unapproved_but_relied_on: "*bool",
  evidence_age_days: "*int",
  has_recorded_elicitation: "*bool",
  is_from_single_witness: "*bool",
  evidence_expiry_days: "*int",
  evidence_has_expired: "*bool",
  owner_agent: "*string",
  is_awaiting_approval: "*bool",
  owner_is_me: "*bool",
  is_my_unfinished_approval: "*bool",
  is_invoked_by_an_exception: "*int",
  has_operational_reliance: "*bool",
  is_unapproved_and_operationally_live: "*bool",
  age_days: "*int",
  is_low_confidence: "*bool",
  owning_version_cadence_days: "*int",
  exceeds_owning_cadence: "*bool",
  is_aging_low_confidence_claim: "*bool",
  owner_role_agent_kind: "*string",
  is_human_owned: "*bool",
  is_ai_validated_by_ai: "*bool",
  review_cadence_days: "*int",
  is_overdue_for_review: "*bool",
  predates_current_role_holder: "*bool",
  owner_role_assignment_valid_from: "*string",
  last_reviewed_at: "*string",
  fragility_signal_count: "*int",
  is_compound_fragile: "*bool",
  is_single_point_of_failure: "*bool",
  is_expiring_single_point_of_failure: "*bool",
  compound_fragile_version_key: "*string",
  valid_fragment_session_key: "*string",
  consuming_step_is_software_assigned: "*bool",
  consuming_step_agent_kind: "*string",
  is_unapproved_and_machine_consumed: "*bool",
  is_unapproved_and_human_consumed: "*bool",
  machine_consumed_unapproved_version_key: "*string",
  has_review_record: "*bool",
  days_since_actual_review: "*int",
  is_unreviewed_since_authoring: "*bool",
  is_genuinely_overdue: "*bool",
  review_recency_is_inferred: "*bool",
  inference_disagrees_with_record: "*bool",
  genuinely_overdue_version_key: "*string",
  ratified_boundary_count: "*float64",
  reliance_surface_count: "*int",
  days_awaiting_my_approval: "*int",
  is_high_blast_radius_unapproved: "*bool",
  is_long_unapproved: "*bool",
  unapproved_load_bearing_version_key: "*string",
  owner_role_is_vacated: "*bool",
  is_orphaned_by_role: "*bool",
  valid_fragment_version_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the KnowledgeFragments row.
 *  Formula: ={{KnowledgeForm}} & ": " & LEFT({{Statement}}, 60) */
export function calcKnowledgeFragmentsName(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.knowledge_form)), vS(": "), erbTextNotNull(erbLeft(vStr(tc.statement), vI(60)))));
}

/** Computes the IsCurrentlyValid calculated field.
 *  TRUE when the fragment is approved and valid now.
 *  Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}}), {{Status}} = "Approved") */
export function calcKnowledgeFragmentsIsCurrentlyValid(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<=", vStr(tc.as_of_instant))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant))))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved")))));
}

/** Computes the HasHumanSource calculated field.
 *  TRUE when the claim originates from a human practitioner rather than software.
 *  Formula: ={{SourceAgentKind}} = "Human" */
export function calcKnowledgeFragmentsHasHumanSource(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.source_agent_kind), vS("Human")));
}

/** Computes the HasOrphanedProvenance calculated field.
 *  TRUE when we still rely on this claim but the agent who gave it to us no longer holds a role here.
 *  Formula: =AND({{IsCurrentlyValid}}, NOT({{SourceAgentIsStillEngaged}})) */
export function calcKnowledgeFragmentsHasOrphanedProvenance(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_valid)), erbBool3(erbNot(erbBool3(vBool(tc.source_agent_is_still_engaged))))));
}

/** Computes the IsUndefendableTacitClaim calculated field.
 *  TRUE when an orphaned claim is of a kind that lives in a person's head rather than in a document.
 *  Formula: =AND({{HasOrphanedProvenance}}, OR({{KnowledgeForm}} = "Tacit", {{KnowledgeForm}} = "SituatedJudgment")) */
export function calcKnowledgeFragmentsIsUndefendableTacitClaim(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_orphaned_provenance)), erbBool3(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.knowledge_form)), vS("Tacit"))), erbBool3(erbEq(erbNullif(vStr(tc.knowledge_form)), vS("SituatedJudgment")))))));
}

/** Computes the IsApproved calculated field.
 *  TRUE when this claim has passed knowledge-authority approval.
 *  Formula: ={{Status}} = "Approved" */
export function calcKnowledgeFragmentsIsApproved(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.status)), vS("Approved")));
}

/** Computes the IsWithinValidityWindow calculated field.
 *  TRUE when this claim's stated validity window contains the present moment.
 *  Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})) */
export function calcKnowledgeFragmentsIsWithinValidityWindow(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<=", vStr(tc.as_of_instant))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant)))))));
}

/** Computes the IsReliedUpon calculated field.
 *  TRUE when this claim is attached to a specific step and is inside its validity window — i.e. it is operationally in play.
 *  Formula: =AND({{Step}} <> "", {{IsWithinValidityWindow}}) */
export function calcKnowledgeFragmentsIsReliedUpon(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.step))), erbBool3(vBool(tc.is_within_validity_window))));
}

/** Computes the IsUnapprovedButReliedOn calculated field.
 *  TRUE when a live procedure is acting on a claim that has not been approved by the knowledge authority.
 *  Formula: =AND({{IsReliedUpon}}, {{IsAttachedToLiveVersion}}, NOT({{IsApproved}})) */
export function calcKnowledgeFragmentsIsUnapprovedButReliedOn(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_relied_upon)), erbBool3(vBool(tc.is_attached_to_live_version)), erbBool3(erbNot(erbBool3(vBool(tc.is_approved))))));
}

/** Computes the HasRecordedElicitation calculated field.
 *  TRUE when this claim traces to a recorded elicitation session.
 *  Formula: ={{ElicitationSession}} <> "" */
export function calcKnowledgeFragmentsHasRecordedElicitation(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.elicitation_session)));
}

/** Computes the EvidenceExpiryDays calculated field.
 *  How many days this claim's evidence is trusted for, given how it was gathered.
 *  Formula: =IF({{IsFromSingleWitness}}, 180, 365) */
export function calcKnowledgeFragmentsEvidenceExpiryDays(tc: KnowledgeFragmentsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_from_single_witness)), () => vI(180), () => vI(365))));
}

/** Computes the EvidenceHasExpired calculated field.
 *  TRUE when the evidence behind this claim is older than we trust evidence of its kind to be.
 *  Formula: =AND({{HasRecordedElicitation}}, {{EvidenceAgeDays}} > {{EvidenceExpiryDays}}) */
export function calcKnowledgeFragmentsEvidenceHasExpired(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_recorded_elicitation)), erbBool3(erbCmp(vInt(tc.evidence_age_days), ">", vInt(tc.evidence_expiry_days)))));
}

/** Computes the IsAwaitingApproval calculated field.
 *  TRUE when this claim has been reviewed but not yet approved.
 *  Formula: ={{Status}} = "Reviewed" */
export function calcKnowledgeFragmentsIsAwaitingApproval(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.status)), vS("Reviewed")));
}

/** Computes the OwnerIsMe calculated field.
 *  TRUE when the People Policy Owner role owns this claim.
 *  Formula: ={{OwnerRole}} = "hr-policy-owner" */
export function calcKnowledgeFragmentsOwnerIsMe(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.owner_role)), vS("hr-policy-owner")));
}

/** Computes the IsMyUnfinishedApproval calculated field.
 *  TRUE when a claim I own has been reviewed and is waiting on my approval.
 *  Formula: =AND({{OwnerIsMe}}, {{IsAwaitingApproval}}) */
export function calcKnowledgeFragmentsIsMyUnfinishedApproval(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.owner_is_me)), erbBool3(vBool(tc.is_awaiting_approval))));
}

/** Computes the HasOperationalReliance calculated field.
 *  TRUE when an exception handler exists on the step this claim governs.
 *  Formula: ={{IsInvokedByAnException}} > 0 */
export function calcKnowledgeFragmentsHasOperationalReliance(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.is_invoked_by_an_exception), ">", vI(0)));
}

/** Computes the IsUnapprovedAndOperationallyLive calculated field.
 *  TRUE when a claim awaiting my approval is already being acted on through a documented exception path.
 *  Formula: =AND({{IsMyUnfinishedApproval}}, {{HasOperationalReliance}}) */
export function calcKnowledgeFragmentsIsUnapprovedAndOperationallyLive(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_my_unfinished_approval)), erbBool3(vBool(tc.has_operational_reliance))));
}

/** Computes the AgeDays calculated field.
 *  Days since this claim became valid.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days") */
export function calcKnowledgeFragmentsAgeDays(tc: KnowledgeFragmentsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.valid_from), vS("days"))));
}

/** Computes the IsLowConfidence calculated field.
 *  TRUE when this claim was recorded with less than high confidence.
 *  Formula: =OR({{Confidence}} = "Medium", {{Confidence}} = "Low") */
export function calcKnowledgeFragmentsIsLowConfidence(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.confidence)), vS("Medium"))), erbBool3(erbEq(erbNullif(vStr(tc.confidence)), vS("Low")))));
}

/** Computes the ExceedsOwningCadence calculated field.
 *  TRUE when this claim is older than the review cadence promised for the version it supports.
 *  Formula: ={{AgeDays}} > {{OwningVersionCadenceDays}} */
export function calcKnowledgeFragmentsExceedsOwningCadence(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.age_days), ">", vInt(tc.owning_version_cadence_days)));
}

/** Computes the IsAgingLowConfidenceClaim calculated field.
 *  TRUE when a claim we were never sure about has also outlived its version's review cadence.
 *  Formula: =AND({{ExceedsOwningCadence}}, {{IsLowConfidence}}) */
export function calcKnowledgeFragmentsIsAgingLowConfidenceClaim(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.exceeds_owning_cadence)), erbBool3(vBool(tc.is_low_confidence))));
}

/** Computes the IsHumanOwned calculated field.
 *  TRUE when the owning role is currently held by a human.
 *  Formula: ={{OwnerRoleAgentKind}} = "Human" */
export function calcKnowledgeFragmentsIsHumanOwned(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.owner_role_agent_kind), vS("Human")));
}

/** Computes the IsAiValidatedByAi calculated field.
 *  TRUE when knowledge came from a non-human source AND is owned by a non-human-held role.
 *  Formula: =AND(NOT({{SourceAgentKind}} = "Human"), NOT({{IsHumanOwned}})) */
export function calcKnowledgeFragmentsIsAiValidatedByAi(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbEq(vStr(tc.source_agent_kind), vS("Human"))))), erbBool3(erbNot(erbBool3(vBool(tc.is_human_owned))))));
}

/** Computes the IsOverdueForReview calculated field.
 *  TRUE when a currently-valid fragment has gone longer than its stewardship cadence without review.
 *  Formula: =AND({{IsCurrentlyValid}}, {{AgeDays}} > {{ReviewCadenceDays}}) */
export function calcKnowledgeFragmentsIsOverdueForReview(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_valid)), erbBool3(erbCmp(vInt(tc.age_days), ">", vInt(tc.review_cadence_days)))));
}

/** Computes the PredatesCurrentRoleHolder calculated field.
 *  TRUE when this knowledge became valid before the current holder of its owning role took the role.
 *  Formula: =AND({{OwnerRoleAgentKind}} <> "", {{ValidFrom}} < {{OwnerRoleAssignmentValidFrom}}) */
export function calcKnowledgeFragmentsPredatesCurrentRoleHolder(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.owner_role_agent_kind))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<", vStr(tc.owner_role_assignment_valid_from)))));
}

/** Computes the FragilitySignalCount calculated field.
 *  How many of the four loop-1 decay signals are simultaneously true for this fragment: single witness, overdue for review, low confidence, operational reliance.
 *  Formula: =IF({{IsFromSingleWitness}}, 1, 0) + IF({{IsOverdueForReview}}, 1, 0) + IF({{IsLowConfidence}}, 1, 0) + IF({{HasOperationalReliance}}, 1, 0) */
export function calcKnowledgeFragmentsFragilitySignalCount(tc: KnowledgeFragmentsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbAdd(erbAdd(erbIf(erbBool3(vBool(tc.is_from_single_witness)), () => vI(1), () => vI(0)), erbIf(erbBool3(vBool(tc.is_overdue_for_review)), () => vI(1), () => vI(0))), erbIf(erbBool3(vBool(tc.is_low_confidence)), () => vI(1), () => vI(0))), erbIf(erbBool3(vBool(tc.has_operational_reliance)), () => vI(1), () => vI(0)))));
}

/** Computes the IsCompoundFragile calculated field.
 *  A fragment carrying at least three of the four decay signals at once.
 *  Formula: ={{FragilitySignalCount}} >= 3 */
export function calcKnowledgeFragmentsIsCompoundFragile(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.fragility_signal_count), ">=", vI(3)));
}

/** Computes the IsSinglePointOfFailure calculated field.
 *  A claim that rests on exactly one person's word and that an active exception handler actually routes cases against.
 *  Formula: =AND({{IsFromSingleWitness}}, {{HasOperationalReliance}}) */
export function calcKnowledgeFragmentsIsSinglePointOfFailure(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_from_single_witness)), erbBool3(vBool(tc.has_operational_reliance))));
}

/** Computes the IsExpiringSinglePointOfFailure calculated field.
 *  A single-sourced, operationally relied-upon claim that is also past its review date.
 *  Formula: =AND({{IsSinglePointOfFailure}}, {{IsOverdueForReview}}) */
export function calcKnowledgeFragmentsIsExpiringSinglePointOfFailure(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_single_point_of_failure)), erbBool3(vBool(tc.is_overdue_for_review))));
}

/** Computes the CompoundFragileVersionKey calculated field.
 *  Composite-key echo: this fragment's procedure version when the fragment is compound-fragile, blank otherwise.
 *  Formula: =IF({{IsCompoundFragile}}, {{ProcedureVersion}}, "") */
export function calcKnowledgeFragmentsCompoundFragileVersionKey(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_compound_fragile)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the ValidFragmentSessionKey calculated field.
 *  Composite-key echo: the elicitation session behind this fragment when the fragment is currently valid, blank otherwise.
 *  Formula: =IF({{IsCurrentlyValid}}, {{ElicitationSession}}, "") */
export function calcKnowledgeFragmentsValidFragmentSessionKey(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_currently_valid)), () => vStr(tc.elicitation_session), () => vS("")));
}

/** Computes the IsUnapprovedAndMachineConsumed calculated field.
 *  An unapproved claim that a software-assigned step actually relies on — executed literally, with no human in position to notice it is wrong.
 *  Formula: =AND({{IsUnapprovedButReliedOn}}, {{ConsumingStepIsSoftwareAssigned}}) */
export function calcKnowledgeFragmentsIsUnapprovedAndMachineConsumed(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unapproved_but_relied_on)), erbBool3(vBool(tc.consuming_step_is_software_assigned))));
}

/** Computes the IsUnapprovedAndHumanConsumed calculated field.
 *  An unapproved claim relied on by a human-assigned step — a reviewable risk rather than a silent one.
 *  Formula: =AND({{IsUnapprovedButReliedOn}}, NOT({{ConsumingStepIsSoftwareAssigned}})) */
export function calcKnowledgeFragmentsIsUnapprovedAndHumanConsumed(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unapproved_but_relied_on)), erbBool3(erbNot(erbBool3(vBool(tc.consuming_step_is_software_assigned))))));
}

/** Computes the MachineConsumedUnapprovedVersionKey calculated field.
 *  Composite-key echo: this fragment's procedure version when the fragment is unapproved and machine-consumed, blank otherwise.
 *  Formula: =IF({{IsUnapprovedAndMachineConsumed}}, {{ProcedureVersion}}, "") */
export function calcKnowledgeFragmentsMachineConsumedUnapprovedVersionKey(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unapproved_and_machine_consumed)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the HasReviewRecord calculated field.
 *  Whether this fragment has ever had an actual review recorded, as opposed to merely having a ValidFrom date.
 *  Formula: ={{LastReviewedAt}} <> "" */
export function calcKnowledgeFragmentsHasReviewRecord(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.last_reviewed_at)));
}

/** Computes the DaysSinceActualReview calculated field.
 *  Days elapsed since this fragment was last actually reviewed. Zero when no review has ever been recorded — read this only alongside HasReviewRecord, never on its own.
 *  Formula: =IF({{HasReviewRecord}}, DATETIME_DIFF({{AsOfInstant}}, {{LastReviewedAt}}, "days"), 0) */
export function calcKnowledgeFragmentsDaysSinceActualReview(tc: KnowledgeFragmentsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.has_review_record)), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.last_reviewed_at), vS("days")), () => vI(0))));
}

/** Computes the IsUnreviewedSinceAuthoring calculated field.
 *  A currently-valid claim that nobody has ever reviewed since it was written.
 *  Formula: =AND({{IsCurrentlyValid}}, NOT({{HasReviewRecord}})) */
export function calcKnowledgeFragmentsIsUnreviewedSinceAuthoring(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_valid)), erbBool3(erbNot(erbBool3(vBool(tc.has_review_record))))));
}

/** Computes the IsGenuinelyOverdue calculated field.
 *  A valid claim whose LAST ACTUAL REVIEW is older than the cadence its owning version promised.
 *  Formula: =AND({{IsCurrentlyValid}}, {{HasReviewRecord}}, {{DaysSinceActualReview}} > {{ReviewCadenceDays}}) */
export function calcKnowledgeFragmentsIsGenuinelyOverdue(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_valid)), erbBool3(vBool(tc.has_review_record)), erbBool3(erbCmp(vInt(tc.days_since_actual_review), ">", vInt(tc.review_cadence_days)))));
}

/** Computes the ReviewRecencyIsInferred calculated field.
 *  A fragment reported overdue by the loop-1 inference purely because no review has ever been recorded — the number is an artifact of missing data, not evidence of neglect.
 *  Formula: =AND({{IsOverdueForReview}}, NOT({{HasReviewRecord}})) */
export function calcKnowledgeFragmentsReviewRecencyIsInferred(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_overdue_for_review)), erbBool3(erbNot(erbBool3(vBool(tc.has_review_record))))));
}

/** Computes the InferenceDisagreesWithRecord calculated field.
 *  A fragment the ValidFrom inference calls overdue but which was in fact reviewed inside its cadence — a false positive in the loop-1 predicate, now provable.
 *  Formula: =AND({{HasReviewRecord}}, {{IsOverdueForReview}}, NOT({{IsGenuinelyOverdue}})) */
export function calcKnowledgeFragmentsInferenceDisagreesWithRecord(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_review_record)), erbBool3(vBool(tc.is_overdue_for_review)), erbBool3(erbNot(erbBool3(vBool(tc.is_genuinely_overdue))))));
}

/** Computes the GenuinelyOverdueVersionKey calculated field.
 *  Composite-key echo: this fragment's procedure version when the fragment is genuinely overdue, blank otherwise.
 *  Formula: =IF({{IsGenuinelyOverdue}}, {{ProcedureVersion}}, "") */
export function calcKnowledgeFragmentsGenuinelyOverdueVersionKey(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_genuinely_overdue)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the RelianceSurfaceCount calculated field.
 *  Total number of distinct downstream dependents on this claim: exception handlers plus ratified authority boundaries.
 *  Formula: ={{IsInvokedByAnException}} + {{RatifiedBoundaryCount}} */
export function calcKnowledgeFragmentsRelianceSurfaceCount(tc: KnowledgeFragmentsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.is_invoked_by_an_exception), vNum(tc.ratified_boundary_count))));
}

/** Computes the DaysAwaitingMyApproval calculated field.
 *  How long a claim of mine has been sitting at Reviewed without my approval. Zero when the claim is not mine or is already decided.
 *  Formula: =IF({{IsMyUnfinishedApproval}}, DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"), 0) */
export function calcKnowledgeFragmentsDaysAwaitingMyApproval(tc: KnowledgeFragmentsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_my_unfinished_approval)), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.valid_from), vS("days")), () => vI(0))));
}

/** Computes the IsHighBlastRadiusUnapproved calculated field.
 *  An unapproved, operationally live claim of mine with more than one distinct downstream dependent.
 *  Formula: =AND({{IsUnapprovedAndOperationallyLive}}, {{RelianceSurfaceCount}} > 1) */
export function calcKnowledgeFragmentsIsHighBlastRadiusUnapproved(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unapproved_and_operationally_live)), erbBool3(erbCmp(vInt(tc.reliance_surface_count), ">", vI(1)))));
}

/** Computes the IsLongUnapproved calculated field.
 *  A claim that has waited on my signature for more than thirty days.
 *  Formula: =AND({{IsMyUnfinishedApproval}}, {{DaysAwaitingMyApproval}} > 30) */
export function calcKnowledgeFragmentsIsLongUnapproved(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_my_unfinished_approval)), erbBool3(erbCmp(vInt(tc.days_awaiting_my_approval), ">", vI(30)))));
}

/** Computes the UnapprovedLoadBearingVersionKey calculated field.
 *  Composite-key echo: this fragment's procedure version when it is a high-blast-radius unapproved claim, blank otherwise.
 *  Formula: =IF({{IsHighBlastRadiusUnapproved}}, {{ProcedureVersion}}, "") */
export function calcKnowledgeFragmentsUnapprovedLoadBearingVersionKey(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_high_blast_radius_unapproved)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsOrphanedByRole calculated field.
 *  A currently-valid claim whose owning role nobody holds — accountable to a vacancy.
 *  Formula: =AND({{IsCurrentlyValid}}, {{OwnerRoleIsVacated}}) */
export function calcKnowledgeFragmentsIsOrphanedByRole(tc: KnowledgeFragmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_valid)), erbBool3(vBool(tc.owner_role_is_vacated))));
}

/** Computes the ValidFragmentVersionKey calculated field.
 *  Composite-key echo: this fragment's owning procedure version when the fragment is currently valid, blank otherwise.
 *  Formula: =IF({{IsCurrentlyValid}}, {{ProcedureVersion}}, "") */
export function calcKnowledgeFragmentsValidFragmentVersionKey(tc: KnowledgeFragmentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_currently_valid)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeKnowledgeFragments(tc: KnowledgeFragmentsRow): KnowledgeFragmentsRow {
  // Level 1
  calcGuard(tc, knowledgeFragmentsFieldTypes, "name", () => { tc.name = calcKnowledgeFragmentsName(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_currently_valid", () => { tc.is_currently_valid = calcKnowledgeFragmentsIsCurrentlyValid(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "has_human_source", () => { tc.has_human_source = calcKnowledgeFragmentsHasHumanSource(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_approved", () => { tc.is_approved = calcKnowledgeFragmentsIsApproved(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_within_validity_window", () => { tc.is_within_validity_window = calcKnowledgeFragmentsIsWithinValidityWindow(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "has_recorded_elicitation", () => { tc.has_recorded_elicitation = calcKnowledgeFragmentsHasRecordedElicitation(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "evidence_expiry_days", () => { tc.evidence_expiry_days = calcKnowledgeFragmentsEvidenceExpiryDays(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_awaiting_approval", () => { tc.is_awaiting_approval = calcKnowledgeFragmentsIsAwaitingApproval(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "owner_is_me", () => { tc.owner_is_me = calcKnowledgeFragmentsOwnerIsMe(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "has_operational_reliance", () => { tc.has_operational_reliance = calcKnowledgeFragmentsHasOperationalReliance(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "age_days", () => { tc.age_days = calcKnowledgeFragmentsAgeDays(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_low_confidence", () => { tc.is_low_confidence = calcKnowledgeFragmentsIsLowConfidence(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_human_owned", () => { tc.is_human_owned = calcKnowledgeFragmentsIsHumanOwned(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "predates_current_role_holder", () => { tc.predates_current_role_holder = calcKnowledgeFragmentsPredatesCurrentRoleHolder(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "has_review_record", () => { tc.has_review_record = calcKnowledgeFragmentsHasReviewRecord(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "reliance_surface_count", () => { tc.reliance_surface_count = calcKnowledgeFragmentsRelianceSurfaceCount(tc); });
  // Level 2
  calcGuard(tc, knowledgeFragmentsFieldTypes, "has_orphaned_provenance", () => { tc.has_orphaned_provenance = calcKnowledgeFragmentsHasOrphanedProvenance(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_relied_upon", () => { tc.is_relied_upon = calcKnowledgeFragmentsIsReliedUpon(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "evidence_has_expired", () => { tc.evidence_has_expired = calcKnowledgeFragmentsEvidenceHasExpired(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_my_unfinished_approval", () => { tc.is_my_unfinished_approval = calcKnowledgeFragmentsIsMyUnfinishedApproval(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "exceeds_owning_cadence", () => { tc.exceeds_owning_cadence = calcKnowledgeFragmentsExceedsOwningCadence(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_ai_validated_by_ai", () => { tc.is_ai_validated_by_ai = calcKnowledgeFragmentsIsAiValidatedByAi(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_overdue_for_review", () => { tc.is_overdue_for_review = calcKnowledgeFragmentsIsOverdueForReview(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_single_point_of_failure", () => { tc.is_single_point_of_failure = calcKnowledgeFragmentsIsSinglePointOfFailure(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "valid_fragment_session_key", () => { tc.valid_fragment_session_key = calcKnowledgeFragmentsValidFragmentSessionKey(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "days_since_actual_review", () => { tc.days_since_actual_review = calcKnowledgeFragmentsDaysSinceActualReview(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_unreviewed_since_authoring", () => { tc.is_unreviewed_since_authoring = calcKnowledgeFragmentsIsUnreviewedSinceAuthoring(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_orphaned_by_role", () => { tc.is_orphaned_by_role = calcKnowledgeFragmentsIsOrphanedByRole(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "valid_fragment_version_key", () => { tc.valid_fragment_version_key = calcKnowledgeFragmentsValidFragmentVersionKey(tc); });
  // Level 3
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_undefendable_tacit_claim", () => { tc.is_undefendable_tacit_claim = calcKnowledgeFragmentsIsUndefendableTacitClaim(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_unapproved_but_relied_on", () => { tc.is_unapproved_but_relied_on = calcKnowledgeFragmentsIsUnapprovedButReliedOn(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_unapproved_and_operationally_live", () => { tc.is_unapproved_and_operationally_live = calcKnowledgeFragmentsIsUnapprovedAndOperationallyLive(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_aging_low_confidence_claim", () => { tc.is_aging_low_confidence_claim = calcKnowledgeFragmentsIsAgingLowConfidenceClaim(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "fragility_signal_count", () => { tc.fragility_signal_count = calcKnowledgeFragmentsFragilitySignalCount(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_expiring_single_point_of_failure", () => { tc.is_expiring_single_point_of_failure = calcKnowledgeFragmentsIsExpiringSinglePointOfFailure(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_genuinely_overdue", () => { tc.is_genuinely_overdue = calcKnowledgeFragmentsIsGenuinelyOverdue(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "review_recency_is_inferred", () => { tc.review_recency_is_inferred = calcKnowledgeFragmentsReviewRecencyIsInferred(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "days_awaiting_my_approval", () => { tc.days_awaiting_my_approval = calcKnowledgeFragmentsDaysAwaitingMyApproval(tc); });
  // Level 4
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_compound_fragile", () => { tc.is_compound_fragile = calcKnowledgeFragmentsIsCompoundFragile(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_unapproved_and_machine_consumed", () => { tc.is_unapproved_and_machine_consumed = calcKnowledgeFragmentsIsUnapprovedAndMachineConsumed(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_unapproved_and_human_consumed", () => { tc.is_unapproved_and_human_consumed = calcKnowledgeFragmentsIsUnapprovedAndHumanConsumed(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "inference_disagrees_with_record", () => { tc.inference_disagrees_with_record = calcKnowledgeFragmentsInferenceDisagreesWithRecord(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "genuinely_overdue_version_key", () => { tc.genuinely_overdue_version_key = calcKnowledgeFragmentsGenuinelyOverdueVersionKey(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_high_blast_radius_unapproved", () => { tc.is_high_blast_radius_unapproved = calcKnowledgeFragmentsIsHighBlastRadiusUnapproved(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "is_long_unapproved", () => { tc.is_long_unapproved = calcKnowledgeFragmentsIsLongUnapproved(tc); });
  // Level 5
  calcGuard(tc, knowledgeFragmentsFieldTypes, "compound_fragile_version_key", () => { tc.compound_fragile_version_key = calcKnowledgeFragmentsCompoundFragileVersionKey(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "machine_consumed_unapproved_version_key", () => { tc.machine_consumed_unapproved_version_key = calcKnowledgeFragmentsMachineConsumedUnapprovedVersionKey(tc); });
  calcGuard(tc, knowledgeFragmentsFieldTypes, "unapproved_load_bearing_version_key", () => { tc.unapproved_load_bearing_version_key = calcKnowledgeFragmentsUnapprovedLoadBearingVersionKey(tc); });
  return tc;
}

/** Reads KnowledgeFragments rows from a JSON array file. */
export function loadKnowledgeFragmentsRows(file: string): KnowledgeFragmentsRow[] {
  return loadRows(file, { fields: knowledgeFragmentsFieldTypes }) as unknown as KnowledgeFragmentsRow[];
}

// =============================================================================
// KNOWLEDGEGAPS TABLE
// Known unknowns and missing procedural coverage. Explicit ERB-PKO extension used to govern scope and prevent silent incompleteness.
// =============================================================================

/** A row in the KnowledgeGaps table. */
export interface KnowledgeGapsRow {
  /** Stored logical identifier for one KnowledgeGaps row. */
  knowledge_gap_id: string;
  /** Human-readable calculated display alias for the KnowledgeGaps row. */
  name: string | null;
  /** Affected procedure version. */
  procedure_version: string | null;
  /** Affected step. */
  step: string | null;
  /** What is not yet known or represented. */
  statement: string | null;
  /** Low, Medium, High, or Critical. */
  severity: string | null;
  /** Blocking or NonBlocking. */
  blocking_kind: string | null;
  /** Open, Investigating, Resolved, AcceptedRisk, or Closed. */
  status: string | null;
  /** Role responsible for resolving the gap. */
  owner_role: string | null;
  /** Time the gap was identified. */
  identified_at: string | null;
  /** Resolution plan or final resolution. */
  resolution_plan: string | null;
  /** TRUE when the gap remains open. */
  is_open: boolean | null;
  /** Echoes the ProcedureVersion id for open high-severity knowledge gaps. */
  open_gap_version_key: string | null;
  /** TRUE when this gap is declared blocking rather than informational. */
  is_blocking: boolean | null;
  /** TRUE when this gap is both unresolved and declared blocking. */
  is_open_and_blocking: boolean | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** Days this gap has been unresolved, or zero once closed. */
  days_open: number | null;
  /** How long a gap of this severity may remain open before it becomes a governance failure in its own right. */
  tolerance_days: number | null;
  /** TRUE when this acknowledged unknown has outlived the tolerance for its severity. */
  is_overdue_gap: boolean | null;
  /** The agent currently holding the role that owns resolving this gap. */
  owner_agent: string | null;
  /** Whether the agent responsible for closing this gap still holds a role here. */
  owner_is_still_engaged: boolean | null;
  /** TRUE when someone has written down how this gap would be closed. */
  has_resolution_plan: boolean | null;
  /** TRUE when an overdue gap has either no plan or no living owner — an admission of ignorance that nobody is acting on. */
  is_abandoned_unknown: boolean | null;
  /** Composite-key echo: this gap's procedure version when the gap is both open and blocking, blank otherwise. */
  open_blocking_gap_version_key: string | null;
  /** Whether the role that owns this gap is currently vacated. */
  owner_role_is_vacated: boolean | null;
  /** An open gap whose owning role nobody currently holds — an acknowledged unknown with nobody accountable for closing it. */
  is_ownerless_open_gap: boolean | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const knowledgeGapsFieldTypes: Record<string, FieldType> = {
  knowledge_gap_id: "string",
  name: "*string",
  procedure_version: "*string",
  step: "*string",
  statement: "*string",
  severity: "*string",
  blocking_kind: "*string",
  status: "*string",
  owner_role: "*string",
  identified_at: "*string",
  resolution_plan: "*string",
  is_open: "*bool",
  open_gap_version_key: "*string",
  is_blocking: "*bool",
  is_open_and_blocking: "*bool",
  evaluation_context: "*string",
  as_of_instant: "*string",
  days_open: "*int",
  tolerance_days: "*int",
  is_overdue_gap: "*bool",
  owner_agent: "*string",
  owner_is_still_engaged: "*bool",
  has_resolution_plan: "*bool",
  is_abandoned_unknown: "*bool",
  open_blocking_gap_version_key: "*string",
  owner_role_is_vacated: "*bool",
  is_ownerless_open_gap: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the KnowledgeGaps row.
 *  Formula: ={{Severity}} & ": " & LEFT({{Statement}}, 60) */
export function calcKnowledgeGapsName(tc: KnowledgeGapsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.severity)), vS(": "), erbTextNotNull(erbLeft(vStr(tc.statement), vI(60)))));
}

/** Computes the IsOpen calculated field.
 *  TRUE when the gap remains open.
 *  Formula: =OR({{Status}} = "Open", {{Status}} = "Investigating") */
export function calcKnowledgeGapsIsOpen(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Open"))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Investigating")))));
}

/** Computes the OpenGapVersionKey calculated field.
 *  Echoes the ProcedureVersion id for open high-severity knowledge gaps.
 *  Formula: =IF(AND({{IsOpen}}, {{Severity}} = "High"), {{ProcedureVersion}}, "") */
export function calcKnowledgeGapsOpenGapVersionKey(tc: KnowledgeGapsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.is_open)), erbBool3(erbEq(erbNullif(vStr(tc.severity)), vS("High"))))), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsBlocking calculated field.
 *  TRUE when this gap is declared blocking rather than informational.
 *  Formula: ={{BlockingKind}} = "Blocking" */
export function calcKnowledgeGapsIsBlocking(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.blocking_kind)), vS("Blocking")));
}

/** Computes the IsOpenAndBlocking calculated field.
 *  TRUE when this gap is both unresolved and declared blocking.
 *  Formula: =AND({{IsOpen}}, {{IsBlocking}}) */
export function calcKnowledgeGapsIsOpenAndBlocking(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_open)), erbBool3(vBool(tc.is_blocking))));
}

/** Computes the DaysOpen calculated field.
 *  Days this gap has been unresolved, or zero once closed.
 *  Formula: =IF({{IsOpen}}, DATETIME_DIFF({{AsOfInstant}}, {{IdentifiedAt}}, "days"), 0) */
export function calcKnowledgeGapsDaysOpen(tc: KnowledgeGapsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_open)), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.identified_at), vS("days")), () => vI(0))));
}

/** Computes the ToleranceDays calculated field.
 *  How long a gap of this severity may remain open before it becomes a governance failure in its own right.
 *  Formula: =IF({{Severity}} = "High", 30, IF({{Severity}} = "Medium", 90, 180)) */
export function calcKnowledgeGapsToleranceDays(tc: KnowledgeGapsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbEq(erbNullif(vStr(tc.severity)), vS("High"))), () => vI(30), () => erbIf(erbBool3(erbEq(erbNullif(vStr(tc.severity)), vS("Medium"))), () => vI(90), () => vI(180)))));
}

/** Computes the IsOverdueGap calculated field.
 *  TRUE when this acknowledged unknown has outlived the tolerance for its severity.
 *  Formula: ={{DaysOpen}} > {{ToleranceDays}} */
export function calcKnowledgeGapsIsOverdueGap(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.days_open), ">", vInt(tc.tolerance_days)));
}

/** Computes the HasResolutionPlan calculated field.
 *  TRUE when someone has written down how this gap would be closed.
 *  Formula: ={{ResolutionPlan}} <> "" */
export function calcKnowledgeGapsHasResolutionPlan(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.resolution_plan)));
}

/** Computes the IsAbandonedUnknown calculated field.
 *  TRUE when an overdue gap has either no plan or no living owner — an admission of ignorance that nobody is acting on.
 *  Formula: =AND({{IsOverdueGap}}, OR(NOT({{HasResolutionPlan}}), NOT({{OwnerIsStillEngaged}}))) */
export function calcKnowledgeGapsIsAbandonedUnknown(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_overdue_gap)), erbBool3(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.has_resolution_plan)))), erbBool3(erbNot(erbBool3(vBool(tc.owner_is_still_engaged))))))));
}

/** Computes the OpenBlockingGapVersionKey calculated field.
 *  Composite-key echo: this gap's procedure version when the gap is both open and blocking, blank otherwise.
 *  Formula: =IF({{IsOpenAndBlocking}}, {{ProcedureVersion}}, "") */
export function calcKnowledgeGapsOpenBlockingGapVersionKey(tc: KnowledgeGapsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_open_and_blocking)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsOwnerlessOpenGap calculated field.
 *  An open gap whose owning role nobody currently holds — an acknowledged unknown with nobody accountable for closing it.
 *  Formula: =AND({{IsOpen}}, {{OwnerRoleIsVacated}}) */
export function calcKnowledgeGapsIsOwnerlessOpenGap(tc: KnowledgeGapsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_open)), erbBool3(vBool(tc.owner_role_is_vacated))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeKnowledgeGaps(tc: KnowledgeGapsRow): KnowledgeGapsRow {
  // Level 1
  calcGuard(tc, knowledgeGapsFieldTypes, "name", () => { tc.name = calcKnowledgeGapsName(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "is_open", () => { tc.is_open = calcKnowledgeGapsIsOpen(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "is_blocking", () => { tc.is_blocking = calcKnowledgeGapsIsBlocking(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "tolerance_days", () => { tc.tolerance_days = calcKnowledgeGapsToleranceDays(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "has_resolution_plan", () => { tc.has_resolution_plan = calcKnowledgeGapsHasResolutionPlan(tc); });
  // Level 2
  calcGuard(tc, knowledgeGapsFieldTypes, "open_gap_version_key", () => { tc.open_gap_version_key = calcKnowledgeGapsOpenGapVersionKey(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "is_open_and_blocking", () => { tc.is_open_and_blocking = calcKnowledgeGapsIsOpenAndBlocking(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "days_open", () => { tc.days_open = calcKnowledgeGapsDaysOpen(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "is_ownerless_open_gap", () => { tc.is_ownerless_open_gap = calcKnowledgeGapsIsOwnerlessOpenGap(tc); });
  // Level 3
  calcGuard(tc, knowledgeGapsFieldTypes, "is_overdue_gap", () => { tc.is_overdue_gap = calcKnowledgeGapsIsOverdueGap(tc); });
  calcGuard(tc, knowledgeGapsFieldTypes, "open_blocking_gap_version_key", () => { tc.open_blocking_gap_version_key = calcKnowledgeGapsOpenBlockingGapVersionKey(tc); });
  // Level 4
  calcGuard(tc, knowledgeGapsFieldTypes, "is_abandoned_unknown", () => { tc.is_abandoned_unknown = calcKnowledgeGapsIsAbandonedUnknown(tc); });
  return tc;
}

/** Reads KnowledgeGaps rows from a JSON array file. */
export function loadKnowledgeGapsRows(file: string): KnowledgeGapsRow[] {
  return loadRows(file, { fields: knowledgeGapsFieldTypes }) as unknown as KnowledgeGapsRow[];
}

// =============================================================================
// FAQS TABLE
// Frequently asked procedural questions. Maps to pko:FrequentlyAskedQuestion, question, answer, hasFAQCategory, and hasFAQTarget.
// =============================================================================

/** A row in the FAQs table. */
export interface FAQsRow {
  /** Stored logical identifier for one FAQs row. */
  faq_id: string;
  /** Human-readable calculated display alias for the FAQs row. */
  name: string | null;
  /** Procedure version addressed by the FAQ. */
  procedure_version: string | null;
  /** Optional step targeted by the FAQ. */
  step: string | null;
  /** FAQ category. */
  category: string | null;
  /** Procedure, Step, Tool, Resource, or Execution. */
  target_kind: string | null;
  /** FAQ question; maps to pko:question. */
  question: string | null;
  /** Approved answer; maps to pko:answer. */
  answer: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const fAQsFieldTypes: Record<string, FieldType> = {
  faq_id: "string",
  name: "*string",
  procedure_version: "*string",
  step: "*string",
  category: "*string",
  target_kind: "*string",
  question: "*string",
  answer: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the FAQs row.
 *  Formula: ={{Question}} */
export function calcFAQsName(tc: FAQsRow): string | null {
  return toStringPtr(vStr(tc.question));
}

/** Computes every calculated field of the row in dependency order. */
export function computeFAQs(tc: FAQsRow): FAQsRow {
  // Level 1
  calcGuard(tc, fAQsFieldTypes, "name", () => { tc.name = calcFAQsName(tc); });
  return tc;
}

/** Reads FAQs rows from a JSON array file. */
export function loadFAQsRows(file: string): FAQsRow[] {
  return loadRows(file, { fields: fAQsFieldTypes }) as unknown as FAQsRow[];
}

// =============================================================================
// EXPLANATIONS TABLE
// Explainable derivation artifacts associated with procedural decisions. Maps to pko:Explanation and pko:hasExplanation.
// =============================================================================

/** A row in the Explanations table. */
export interface ExplanationsRow {
  /** Stored logical identifier for one Explanations row. */
  explanation_id: string;
  /** Human-readable calculated display alias for the Explanations row. */
  name: string | null;
  /** Procedure version explained. */
  procedure_version: string | null;
  /** Step explained. */
  step: string | null;
  /** Explanation title. */
  title: string | null;
  /** What the explanation traces. */
  description: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const explanationsFieldTypes: Record<string, FieldType> = {
  explanation_id: "string",
  name: "*string",
  procedure_version: "*string",
  step: "*string",
  title: "*string",
  description: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Explanations row.
 *  Formula: ={{Title}} */
export function calcExplanationsName(tc: ExplanationsRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes every calculated field of the row in dependency order. */
export function computeExplanations(tc: ExplanationsRow): ExplanationsRow {
  // Level 1
  calcGuard(tc, explanationsFieldTypes, "name", () => { tc.name = calcExplanationsName(tc); });
  return tc;
}

/** Reads Explanations rows from a JSON array file. */
export function loadExplanationsRows(file: string): ExplanationsRow[] {
  return loadRows(file, { fields: explanationsFieldTypes }) as unknown as ExplanationsRow[];
}

// =============================================================================
// PROCEDUREEXECUTIONS TABLE
// Concrete enactments of procedure specifications. Maps to pko:ProcedureExecution and remains separate from ProcedureVersions.
// =============================================================================

/** A row in the ProcedureExecutions table. */
export interface ProcedureExecutionsRow {
  /** Stored logical identifier for one ProcedureExecutions row. */
  procedure_execution_id: string;
  /** Human-readable calculated display alias for the ProcedureExecutions row. */
  name: string | null;
  /** Exact procedure specification executed; maps to pko:hasExecutedProcedure. */
  procedure_version: string | null;
  /** PKO execution status: InProgress, Completed, Paused, or Cancelled. */
  execution_status: string | null;
  /** Execution start; maps to prov:startedAtTime. */
  started_at: string | null;
  /** Execution end; maps to prov:endedAtTime. */
  ended_at: string | null;
  /** Agent accountable for the overall execution; maps to pko:wasExecutedBy. */
  executed_by_agent: string | null;
  /** Execution-specific scope or target. */
  context: string | null;
  /** Identifier in the operational execution system. */
  operational_record_uri: string | null;
  /** How many steps this execution was supposed to perform. */
  expected_step_count: number | null;
  /** How many steps of this execution actually reached Completed. */
  completed_step_count: number | null;
  /** Total step executions in this run that carry at least one control breach. */
  control_breach_count: number | null;
  /** How many steps in this execution ran long. */
  late_step_count: number | null;
  /** TRUE when every specified step of the procedure version reached Completed in this execution. */
  is_structurally_complete: boolean | null;
  /** TRUE when the execution either skipped specified steps or carried at least one control breach. */
  diverged_from_specification: boolean | null;
  /** TRUE when every blocking requirement bound to every step of this execution received a satisfaction record. */
  all_blocking_controls_evaluated: boolean | null;
  /** How many step executions in this run left at least one blocking control unevaluated. */
  unevaluated_blocking_total: number | null;
  /** TRUE when no agent both prepared and approved within this execution. */
  separation_of_duties_held: boolean | null;
  /** Number of segregation-of-duties violations in this execution. */
  separation_violation_count: number | null;
  /** TRUE only when every step completed, no control breached, every blocking control was actually evaluated, and segregation of duties held. */
  is_attestation_ready: boolean | null;
  /** Names the highest-severity reason the attestation cannot be signed, or empty when it can. */
  attestation_blocker_summary: string | null;
  /** Whether the procedure version this execution ran against is currently fit to execute. */
  executed_version_is_fit: boolean | null;
  /** TRUE when a completed execution was run against a procedure version the organization no longer stands behind. */
  signed_against_unfit_version: boolean | null;
  /** How many blocking controls in this execution passed on human assertion alone. */
  asserted_only_control_count: number | null;
  /** TRUE when any blocking control in this execution passed without a computed witness behind it. */
  assurance_is_mostly_asserted: boolean | null;
  /** Number of deliveries in this execution where unreachable handling was wrong -- either fabricated acknowledgement or no exception invoked. */
  unreachable_handling_failure_count: number | null;
  /** Number of transmitted messages in this execution whose text we no longer hold despite an active retention obligation. */
  retention_breach_count: number | null;
  /** How many passed legal reviews exist for this execution. */
  cleared_legal_review_count: number | null;
  /** TRUE when legal review has passed for this execution. */
  has_cleared_legal_review: boolean | null;
  /** Number of failed deliveries in this run that nobody picked up. */
  abandoned_failure_count: number | null;
  /** Number of confirmed-delivered messages in this run. */
  delivered_count: number | null;
  /** Total delivery records of any status for this run. */
  total_delivery_attempt_count: number | null;
  /** TRUE when this run has any untriaged delivery failure. */
  has_abandoned_failures: boolean | null;
  /** Number of gate refusals in this run that were either overridden or silently dropped. */
  mishandled_refusal_count: number | null;
  /** Number of step executions in this run that were not clean. */
  unclean_step_count: number | null;
  /** TRUE when every step execution in this run was clean. */
  ran_clean: boolean | null;
  /** How many human approval gates were executed in this run. */
  count_of_approval_executions: number | null;
  /** TRUE when at least one human approval gate was executed in this run. */
  has_human_approval: boolean | null;
  /** How many times the send step ran in this execution. */
  count_of_delivery_executions: number | null;
  /** TRUE when this run has sent communications to employees. */
  has_delivered: boolean | null;
  /** TRUE when a run sent employee communications without executing a human approval gate. */
  delivered_without_approval: boolean | null;
  /** Number of approval-type requirements in this run that are incomplete or non-human-evaluated. */
  invalid_approval_count: number | null;
  /** TRUE when every approval-type requirement in this run is fully satisfied by a human. */
  approval_chain_is_complete: boolean | null;
  /** How many steps in this run were clean only because nothing checked them. */
  vacuously_clean_step_count: number | null;
  /** How many preparation steps this execution actually ran. */
  preparation_step_count: number | null;
  /** How many approval steps this execution actually ran. */
  approval_step_count: number | null;
  /** TRUE when this execution contained both a preparation step and an approval step, so segregation of duties had an opportunity to fail. */
  separation_was_testable: boolean | null;
  /** TRUE only when segregation of duties both could have failed and did not. */
  separation_held_under_test: boolean | null;
  /** TRUE when the segregation control reports as held on a run where it could not have failed. */
  separation_is_vacuously_green: boolean | null;
  /** The sentence that goes into the control narrative for this execution. */
  separation_assurance_note: string | null;
  /** How many steps in this run departed from spec with nothing authorising the departure. */
  ungoverned_divergence_count: number | null;
  /** TRUE when this run departed from specification and every departure was authorised. */
  divergence_was_fully_governed: boolean | null;
  /** How many blocking controls in this run were cleared by something computed. */
  computedly_witnessed_control_count: number | null;
  /** Total blocking controls actually evaluated in this run, computed and asserted together. */
  evaluated_control_count: number | null;
  /** The fraction of evaluated blocking controls in this run that rest on a computed witness rather than a human assertion. */
  computed_assurance_ratio: number | null;
  /** How many controls in this run were cleared by assertion from someone with an interest in the outcome. */
  interested_party_assertion_count: number | null;
  /** Names the quality of the assurance behind this execution, worst case first. */
  assurance_grade: string | null;
  /** TRUE when the model says I may sign, but the basis for that permission is mostly or partly unwitnessed assertion. */
  attestation_would_be_weakly_based: boolean | null;
  /** How many verifications in this run were independently observed by a human with evidence attached. */
  independent_human_observation_count: number | null;
  /** TRUE when at least one verification in this run was independently observed by a human. */
  has_any_independent_observation: boolean | null;
  /** How many approval steps in this run rested on self-attestation. */
  self_attested_approval_count: number | null;
  /** TRUE when every approval in this run rests on self-attestation and no independent human observation exists anywhere in it. */
  assurance_chain_is_circular: boolean | null;
  /** The most recent signature instant for this execution. */
  latest_attestation_instant: string | null;
  /** TRUE when someone has signed for this execution. */
  has_been_attested: boolean | null;
  /** How many signatures exist against this execution. */
  attestation_count: number | null;
  /** How many controls were scored after this execution was signed. */
  post_attestation_score_count: number | null;
  /** TRUE when controls this attestation depended on were scored after the signature was given. */
  basis_changed_after_signature: boolean | null;
  /** TRUE when the basis changed after signature AND the execution no longer reads as attestable. */
  requires_re_attestation: boolean | null;
  /** How many sends this campaign proposed to make. */
  intended_recipient_count: number | null;
  /** How many of those sends actually left our systems. */
  reached_recipient_count: number | null;
  /** How many intended recipients disappeared from this campaign with no record. */
  silently_dropped_count: number | null;
  /** Percentage of intended recipients who actually received the communication. */
  delivery_yield_percent: number | null;
  /** TRUE when a campaign lost intended recipients without producing any record of the loss. */
  campaign_silently_lost_audience: boolean | null;
  /** How many sends this execution refused without recording the refusal anywhere. */
  unrecorded_refusal_count: number | null;
  /** TRUE when this run contains at least one refusal that left no trace anywhere. */
  has_unrecorded_refusals: boolean | null;
  /** How many of this execution's send decisions were corroborated by a delivery record. */
  independently_confirmed_intent_count: number | null;
  /** TRUE when no send decision in this run was confirmed by anything other than the pipeline itself. */
  send_decisions_are_entirely_self_witnessed: boolean | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const procedureExecutionsFieldTypes: Record<string, FieldType> = {
  procedure_execution_id: "string",
  name: "*string",
  procedure_version: "*string",
  execution_status: "*string",
  started_at: "*string",
  ended_at: "*string",
  executed_by_agent: "*string",
  context: "*string",
  operational_record_uri: "*string",
  expected_step_count: "*float64",
  completed_step_count: "*float64",
  control_breach_count: "*float64",
  late_step_count: "*float64",
  is_structurally_complete: "*bool",
  diverged_from_specification: "*bool",
  all_blocking_controls_evaluated: "*bool",
  unevaluated_blocking_total: "*float64",
  separation_of_duties_held: "*bool",
  separation_violation_count: "*float64",
  is_attestation_ready: "*bool",
  attestation_blocker_summary: "*string",
  executed_version_is_fit: "*bool",
  signed_against_unfit_version: "*bool",
  asserted_only_control_count: "*float64",
  assurance_is_mostly_asserted: "*bool",
  unreachable_handling_failure_count: "*float64",
  retention_breach_count: "*float64",
  cleared_legal_review_count: "*float64",
  has_cleared_legal_review: "*bool",
  abandoned_failure_count: "*float64",
  delivered_count: "*float64",
  total_delivery_attempt_count: "*float64",
  has_abandoned_failures: "*bool",
  mishandled_refusal_count: "*float64",
  unclean_step_count: "*float64",
  ran_clean: "*bool",
  count_of_approval_executions: "*int",
  has_human_approval: "*bool",
  count_of_delivery_executions: "*int",
  has_delivered: "*bool",
  delivered_without_approval: "*bool",
  invalid_approval_count: "*float64",
  approval_chain_is_complete: "*bool",
  vacuously_clean_step_count: "*float64",
  preparation_step_count: "*float64",
  approval_step_count: "*float64",
  separation_was_testable: "*bool",
  separation_held_under_test: "*bool",
  separation_is_vacuously_green: "*bool",
  separation_assurance_note: "*string",
  ungoverned_divergence_count: "*float64",
  divergence_was_fully_governed: "*bool",
  computedly_witnessed_control_count: "*float64",
  evaluated_control_count: "*float64",
  computed_assurance_ratio: "*float64",
  interested_party_assertion_count: "*float64",
  assurance_grade: "*string",
  attestation_would_be_weakly_based: "*bool",
  independent_human_observation_count: "*float64",
  has_any_independent_observation: "*bool",
  self_attested_approval_count: "*float64",
  assurance_chain_is_circular: "*bool",
  latest_attestation_instant: "*string",
  has_been_attested: "*bool",
  attestation_count: "*float64",
  post_attestation_score_count: "*float64",
  basis_changed_after_signature: "*bool",
  requires_re_attestation: "*bool",
  intended_recipient_count: "*float64",
  reached_recipient_count: "*float64",
  silently_dropped_count: "*float64",
  delivery_yield_percent: "*float64",
  campaign_silently_lost_audience: "*bool",
  unrecorded_refusal_count: "*float64",
  has_unrecorded_refusals: "*bool",
  independently_confirmed_intent_count: "*float64",
  send_decisions_are_entirely_self_witnessed: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcedureExecutions row.
 *  Formula: ={{ProcedureVersion}} & " / " & {{Context}} */
export function calcProcedureExecutionsName(tc: ProcedureExecutionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_version)), vS(" / "), erbTextOr(vStr(tc.context))));
}

/** Computes the IsStructurallyComplete calculated field.
 *  TRUE when every specified step of the procedure version reached Completed in this execution.
 *  Formula: ={{CompletedStepCount}} >= {{ExpectedStepCount}} */
export function calcProcedureExecutionsIsStructurallyComplete(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.completed_step_count), ">=", vNum(tc.expected_step_count)));
}

/** Computes the DivergedFromSpecification calculated field.
 *  TRUE when the execution either skipped specified steps or carried at least one control breach.
 *  Formula: =OR(NOT({{IsStructurallyComplete}}), {{ControlBreachCount}} > 0) */
export function calcProcedureExecutionsDivergedFromSpecification(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.is_structurally_complete)))), erbBool3(erbCmp(vNum(tc.control_breach_count), ">", vI(0)))));
}

/** Computes the AllBlockingControlsEvaluated calculated field.
 *  TRUE when every blocking requirement bound to every step of this execution received a satisfaction record.
 *  Formula: ={{UnevaluatedBlockingTotal}} = 0 */
export function calcProcedureExecutionsAllBlockingControlsEvaluated(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.unevaluated_blocking_total), vI(0)));
}

/** Computes the SeparationOfDutiesHeld calculated field.
 *  TRUE when no agent both prepared and approved within this execution.
 *  Formula: ={{SeparationViolationCount}} = 0 */
export function calcProcedureExecutionsSeparationOfDutiesHeld(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.separation_violation_count), vI(0)));
}

/** Computes the IsAttestationReady calculated field.
 *  TRUE only when every step completed, no control breached, every blocking control was actually evaluated, and segregation of duties held.
 *  Formula: =AND({{IsStructurallyComplete}}, NOT({{DivergedFromSpecification}}), {{AllBlockingControlsEvaluated}}, {{SeparationOfDutiesHeld}}) */
export function calcProcedureExecutionsIsAttestationReady(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_structurally_complete)), erbBool3(erbNot(erbBool3(vBool(tc.diverged_from_specification)))), erbBool3(vBool(tc.all_blocking_controls_evaluated)), erbBool3(vBool(tc.separation_of_duties_held))));
}

/** Computes the AttestationBlockerSummary calculated field.
 *  Names the highest-severity reason the attestation cannot be signed, or empty when it can.
 *  Formula: =IF({{IsAttestationReady}}, "", IF(NOT({{IsStructurallyComplete}}), "Incomplete: specified steps did not all complete.", IF({{SeparationViolationCount}} > 0, "Segregation of duties violated.", IF({{UnevaluatedBlockingTotal}} > 0, "Blocking controls were never evaluated.", "Control breach recorded on one or more steps.")))) */
export function calcProcedureExecutionsAttestationBlockerSummary(tc: ProcedureExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_attestation_ready)), () => vS(""), () => erbIf(erbBool3(erbNot(erbBool3(vBool(tc.is_structurally_complete)))), () => vS("Incomplete: specified steps did not all complete."), () => erbIf(erbBool3(erbCmp(vNum(tc.separation_violation_count), ">", vI(0))), () => vS("Segregation of duties violated."), () => erbIf(erbBool3(erbCmp(vNum(tc.unevaluated_blocking_total), ">", vI(0))), () => vS("Blocking controls were never evaluated."), () => vS("Control breach recorded on one or more steps."))))));
}

/** Computes the SignedAgainstUnfitVersion calculated field.
 *  TRUE when a completed execution was run against a procedure version the organization no longer stands behind.
 *  Formula: =AND({{ExecutionStatus}} = "Completed", NOT({{ExecutedVersionIsFit}})) */
export function calcProcedureExecutionsSignedAgainstUnfitVersion(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.execution_status)), vS("Completed"))), erbBool3(erbNot(erbBool3(vBool(tc.executed_version_is_fit))))));
}

/** Computes the AssuranceIsMostlyAsserted calculated field.
 *  TRUE when any blocking control in this execution passed without a computed witness behind it.
 *  Formula: ={{AssertedOnlyControlCount}} > 0 */
export function calcProcedureExecutionsAssuranceIsMostlyAsserted(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.asserted_only_control_count), ">", vI(0)));
}

/** Computes the HasClearedLegalReview calculated field.
 *  TRUE when legal review has passed for this execution.
 *  Formula: ={{ClearedLegalReviewCount}} > 0 */
export function calcProcedureExecutionsHasClearedLegalReview(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.cleared_legal_review_count), ">", vI(0)));
}

/** Computes the HasAbandonedFailures calculated field.
 *  TRUE when this run has any untriaged delivery failure.
 *  Formula: ={{AbandonedFailureCount}} > 0 */
export function calcProcedureExecutionsHasAbandonedFailures(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.abandoned_failure_count), ">", vI(0)));
}

/** Computes the RanClean calculated field.
 *  TRUE when every step execution in this run was clean.
 *  Formula: ={{UncleanStepCount}} = 0 */
export function calcProcedureExecutionsRanClean(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.unclean_step_count), vI(0)));
}

/** Computes the HasHumanApproval calculated field.
 *  TRUE when at least one human approval gate was executed in this run.
 *  Formula: ={{CountOfApprovalExecutions}} > 0 */
export function calcProcedureExecutionsHasHumanApproval(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_approval_executions), ">", vI(0)));
}

/** Computes the HasDelivered calculated field.
 *  TRUE when this run has sent communications to employees.
 *  Formula: ={{CountOfDeliveryExecutions}} > 0 */
export function calcProcedureExecutionsHasDelivered(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_delivery_executions), ">", vI(0)));
}

/** Computes the DeliveredWithoutApproval calculated field.
 *  TRUE when a run sent employee communications without executing a human approval gate.
 *  Formula: =AND({{HasDelivered}}, NOT({{HasHumanApproval}})) */
export function calcProcedureExecutionsDeliveredWithoutApproval(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_delivered)), erbBool3(erbNot(erbBool3(vBool(tc.has_human_approval))))));
}

/** Computes the ApprovalChainIsComplete calculated field.
 *  TRUE when every approval-type requirement in this run is fully satisfied by a human.
 *  Formula: ={{InvalidApprovalCount}} = 0 */
export function calcProcedureExecutionsApprovalChainIsComplete(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.invalid_approval_count), vI(0)));
}

/** Computes the SeparationWasTestable calculated field.
 *  TRUE when this execution contained both a preparation step and an approval step, so segregation of duties had an opportunity to fail.
 *  Formula: =AND({{PreparationStepCount}} > 0, {{ApprovalStepCount}} > 0) */
export function calcProcedureExecutionsSeparationWasTestable(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.preparation_step_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.approval_step_count), ">", vI(0)))));
}

/** Computes the SeparationHeldUnderTest calculated field.
 *  TRUE only when segregation of duties both could have failed and did not.
 *  Formula: =AND({{SeparationWasTestable}}, {{SeparationOfDutiesHeld}}) */
export function calcProcedureExecutionsSeparationHeldUnderTest(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.separation_was_testable)), erbBool3(vBool(tc.separation_of_duties_held))));
}

/** Computes the SeparationIsVacuouslyGreen calculated field.
 *  TRUE when the segregation control reports as held on a run where it could not have failed.
 *  Formula: =AND({{SeparationOfDutiesHeld}}, NOT({{SeparationWasTestable}})) */
export function calcProcedureExecutionsSeparationIsVacuouslyGreen(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.separation_of_duties_held)), erbBool3(erbNot(erbBool3(vBool(tc.separation_was_testable))))));
}

/** Computes the SeparationAssuranceNote calculated field.
 *  The sentence that goes into the control narrative for this execution.
 *  Formula: =IF({{SeparationViolationCount}} > 0, "Violated: same agent prepared and approved.", IF({{SeparationIsVacuouslyGreen}}, "Not tested: this run had no preparation/approval pair.", "Held under test.")) */
export function calcProcedureExecutionsSeparationAssuranceNote(tc: ProcedureExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbCmp(vNum(tc.separation_violation_count), ">", vI(0))), () => vS("Violated: same agent prepared and approved."), () => erbIf(erbBool3(vBool(tc.separation_is_vacuously_green)), () => vS("Not tested: this run had no preparation/approval pair."), () => vS("Held under test."))));
}

/** Computes the DivergenceWasFullyGoverned calculated field.
 *  TRUE when this run departed from specification and every departure was authorised.
 *  Formula: =AND({{DivergedFromSpecification}}, {{UngovernedDivergenceCount}} = 0) */
export function calcProcedureExecutionsDivergenceWasFullyGoverned(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.diverged_from_specification)), erbBool3(erbEq(vNum(tc.ungoverned_divergence_count), vI(0)))));
}

/** Computes the EvaluatedControlCount calculated field.
 *  Total blocking controls actually evaluated in this run, computed and asserted together.
 *  Formula: ={{ComputedlyWitnessedControlCount}} + {{AssertedOnlyControlCount}} */
export function calcProcedureExecutionsEvaluatedControlCount(tc: ProcedureExecutionsRow): number | null {
  return toFloatPtr(erbAdd(vNum(tc.computedly_witnessed_control_count), vNum(tc.asserted_only_control_count)));
}

/** Computes the ComputedAssuranceRatio calculated field.
 *  The fraction of evaluated blocking controls in this run that rest on a computed witness rather than a human assertion.
 *  Formula: =IF({{EvaluatedControlCount}} = 0, 0, {{ComputedlyWitnessedControlCount}} / {{EvaluatedControlCount}}) */
export function calcProcedureExecutionsComputedAssuranceRatio(tc: ProcedureExecutionsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbEq(vNum(tc.evaluated_control_count), vI(0))), () => vI(0), () => erbDiv(vNum(tc.computedly_witnessed_control_count), vNum(tc.evaluated_control_count))));
}

/** Computes the AssuranceGrade calculated field.
 *  Names the quality of the assurance behind this execution, worst case first.
 *  Formula: =IF({{EvaluatedControlCount}} = 0, "None: no blocking control was evaluated.", IF({{InterestedPartyAssertionCount}} > 0, "Weak: at least one control rests on an interested-party assertion.", IF({{ComputedAssuranceRatio}} < 0.5, "Thin: most controls rest on human assertion.", IF({{ComputedAssuranceRatio}} < 1, "Mixed: computed and asserted controls.", "Computed: every evaluated control has a witness.")))) */
export function calcProcedureExecutionsAssuranceGrade(tc: ProcedureExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(vNum(tc.evaluated_control_count), vI(0))), () => vS("None: no blocking control was evaluated."), () => erbIf(erbBool3(erbCmp(vNum(tc.interested_party_assertion_count), ">", vI(0))), () => vS("Weak: at least one control rests on an interested-party assertion."), () => erbIf(erbBool3(erbCmp(vNum(tc.computed_assurance_ratio), "<", vF(0.5))), () => vS("Thin: most controls rest on human assertion."), () => erbIf(erbBool3(erbCmp(vNum(tc.computed_assurance_ratio), "<", vI(1))), () => vS("Mixed: computed and asserted controls."), () => vS("Computed: every evaluated control has a witness."))))));
}

/** Computes the AttestationWouldBeWeaklyBased calculated field.
 *  TRUE when the model says I may sign, but the basis for that permission is mostly or partly unwitnessed assertion.
 *  Formula: =AND({{IsAttestationReady}}, OR({{InterestedPartyAssertionCount}} > 0, {{ComputedAssuranceRatio}} < 0.5)) */
export function calcProcedureExecutionsAttestationWouldBeWeaklyBased(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_attestation_ready)), erbBool3(erbOr(erbBool3(erbCmp(vNum(tc.interested_party_assertion_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.computed_assurance_ratio), "<", vF(0.5)))))));
}

/** Computes the HasAnyIndependentObservation calculated field.
 *  TRUE when at least one verification in this run was independently observed by a human.
 *  Formula: ={{IndependentHumanObservationCount}} > 0 */
export function calcProcedureExecutionsHasAnyIndependentObservation(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.independent_human_observation_count), ">", vI(0)));
}

/** Computes the AssuranceChainIsCircular calculated field.
 *  TRUE when every approval in this run rests on self-attestation and no independent human observation exists anywhere in it.
 *  Formula: =AND({{SelfAttestedApprovalCount}} > 0, NOT({{HasAnyIndependentObservation}})) */
export function calcProcedureExecutionsAssuranceChainIsCircular(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.self_attested_approval_count), ">", vI(0))), erbBool3(erbNot(erbBool3(vBool(tc.has_any_independent_observation))))));
}

/** Computes the HasBeenAttested calculated field.
 *  TRUE when someone has signed for this execution.
 *  Formula: ={{AttestationCount}} > 0 */
export function calcProcedureExecutionsHasBeenAttested(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.attestation_count), ">", vI(0)));
}

/** Computes the BasisChangedAfterSignature calculated field.
 *  TRUE when controls this attestation depended on were scored after the signature was given.
 *  Formula: =AND({{HasBeenAttested}}, {{PostAttestationScoreCount}} > 0) */
export function calcProcedureExecutionsBasisChangedAfterSignature(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_been_attested)), erbBool3(erbCmp(vNum(tc.post_attestation_score_count), ">", vI(0)))));
}

/** Computes the RequiresReAttestation calculated field.
 *  TRUE when the basis changed after signature AND the execution no longer reads as attestable.
 *  Formula: =AND({{BasisChangedAfterSignature}}, NOT({{IsAttestationReady}})) */
export function calcProcedureExecutionsRequiresReAttestation(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.basis_changed_after_signature)), erbBool3(erbNot(erbBool3(vBool(tc.is_attestation_ready))))));
}

/** Computes the DeliveryYieldPercent calculated field.
 *  Percentage of intended recipients who actually received the communication.
 *  Formula: =IF({{IntendedRecipientCount}} > 0, {{ReachedRecipientCount}} * 100 / {{IntendedRecipientCount}}, 0) */
export function calcProcedureExecutionsDeliveryYieldPercent(tc: ProcedureExecutionsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbCmp(vNum(tc.intended_recipient_count), ">", vI(0))), () => erbDiv(erbMul(vNum(tc.reached_recipient_count), vI(100)), vNum(tc.intended_recipient_count)), () => vI(0)));
}

/** Computes the CampaignSilentlyLostAudience calculated field.
 *  TRUE when a campaign lost intended recipients without producing any record of the loss.
 *  Formula: =({{SilentlyDroppedCount}} > 0) */
export function calcProcedureExecutionsCampaignSilentlyLostAudience(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.silently_dropped_count), ">", vI(0)));
}

/** Computes the HasUnrecordedRefusals calculated field.
 *  TRUE when this run contains at least one refusal that left no trace anywhere.
 *  Formula: =({{UnrecordedRefusalCount}} > 0) */
export function calcProcedureExecutionsHasUnrecordedRefusals(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.unrecorded_refusal_count), ">", vI(0)));
}

/** Computes the SendDecisionsAreEntirelySelfWitnessed calculated field.
 *  TRUE when no send decision in this run was confirmed by anything other than the pipeline itself.
 *  Formula: =AND({{IntendedRecipientCount}} > 0, {{IndependentlyConfirmedIntentCount}} = 0) */
export function calcProcedureExecutionsSendDecisionsAreEntirelySelfWitnessed(tc: ProcedureExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.intended_recipient_count), ">", vI(0))), erbBool3(erbEq(vNum(tc.independently_confirmed_intent_count), vI(0)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcedureExecutions(tc: ProcedureExecutionsRow): ProcedureExecutionsRow {
  // Level 1
  calcGuard(tc, procedureExecutionsFieldTypes, "name", () => { tc.name = calcProcedureExecutionsName(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "is_structurally_complete", () => { tc.is_structurally_complete = calcProcedureExecutionsIsStructurallyComplete(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "all_blocking_controls_evaluated", () => { tc.all_blocking_controls_evaluated = calcProcedureExecutionsAllBlockingControlsEvaluated(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "separation_of_duties_held", () => { tc.separation_of_duties_held = calcProcedureExecutionsSeparationOfDutiesHeld(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "signed_against_unfit_version", () => { tc.signed_against_unfit_version = calcProcedureExecutionsSignedAgainstUnfitVersion(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "assurance_is_mostly_asserted", () => { tc.assurance_is_mostly_asserted = calcProcedureExecutionsAssuranceIsMostlyAsserted(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_cleared_legal_review", () => { tc.has_cleared_legal_review = calcProcedureExecutionsHasClearedLegalReview(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_abandoned_failures", () => { tc.has_abandoned_failures = calcProcedureExecutionsHasAbandonedFailures(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "ran_clean", () => { tc.ran_clean = calcProcedureExecutionsRanClean(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_human_approval", () => { tc.has_human_approval = calcProcedureExecutionsHasHumanApproval(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_delivered", () => { tc.has_delivered = calcProcedureExecutionsHasDelivered(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "approval_chain_is_complete", () => { tc.approval_chain_is_complete = calcProcedureExecutionsApprovalChainIsComplete(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "separation_was_testable", () => { tc.separation_was_testable = calcProcedureExecutionsSeparationWasTestable(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "evaluated_control_count", () => { tc.evaluated_control_count = calcProcedureExecutionsEvaluatedControlCount(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_any_independent_observation", () => { tc.has_any_independent_observation = calcProcedureExecutionsHasAnyIndependentObservation(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_been_attested", () => { tc.has_been_attested = calcProcedureExecutionsHasBeenAttested(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "delivery_yield_percent", () => { tc.delivery_yield_percent = calcProcedureExecutionsDeliveryYieldPercent(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "campaign_silently_lost_audience", () => { tc.campaign_silently_lost_audience = calcProcedureExecutionsCampaignSilentlyLostAudience(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "has_unrecorded_refusals", () => { tc.has_unrecorded_refusals = calcProcedureExecutionsHasUnrecordedRefusals(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "send_decisions_are_entirely_self_witnessed", () => { tc.send_decisions_are_entirely_self_witnessed = calcProcedureExecutionsSendDecisionsAreEntirelySelfWitnessed(tc); });
  // Level 2
  calcGuard(tc, procedureExecutionsFieldTypes, "diverged_from_specification", () => { tc.diverged_from_specification = calcProcedureExecutionsDivergedFromSpecification(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "delivered_without_approval", () => { tc.delivered_without_approval = calcProcedureExecutionsDeliveredWithoutApproval(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "separation_held_under_test", () => { tc.separation_held_under_test = calcProcedureExecutionsSeparationHeldUnderTest(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "separation_is_vacuously_green", () => { tc.separation_is_vacuously_green = calcProcedureExecutionsSeparationIsVacuouslyGreen(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "computed_assurance_ratio", () => { tc.computed_assurance_ratio = calcProcedureExecutionsComputedAssuranceRatio(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "assurance_chain_is_circular", () => { tc.assurance_chain_is_circular = calcProcedureExecutionsAssuranceChainIsCircular(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "basis_changed_after_signature", () => { tc.basis_changed_after_signature = calcProcedureExecutionsBasisChangedAfterSignature(tc); });
  // Level 3
  calcGuard(tc, procedureExecutionsFieldTypes, "is_attestation_ready", () => { tc.is_attestation_ready = calcProcedureExecutionsIsAttestationReady(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "separation_assurance_note", () => { tc.separation_assurance_note = calcProcedureExecutionsSeparationAssuranceNote(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "divergence_was_fully_governed", () => { tc.divergence_was_fully_governed = calcProcedureExecutionsDivergenceWasFullyGoverned(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "assurance_grade", () => { tc.assurance_grade = calcProcedureExecutionsAssuranceGrade(tc); });
  // Level 4
  calcGuard(tc, procedureExecutionsFieldTypes, "attestation_blocker_summary", () => { tc.attestation_blocker_summary = calcProcedureExecutionsAttestationBlockerSummary(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "attestation_would_be_weakly_based", () => { tc.attestation_would_be_weakly_based = calcProcedureExecutionsAttestationWouldBeWeaklyBased(tc); });
  calcGuard(tc, procedureExecutionsFieldTypes, "requires_re_attestation", () => { tc.requires_re_attestation = calcProcedureExecutionsRequiresReAttestation(tc); });
  return tc;
}

/** Reads ProcedureExecutions rows from a JSON array file. */
export function loadProcedureExecutionsRows(file: string): ProcedureExecutionsRow[] {
  return loadRows(file, { fields: procedureExecutionsFieldTypes }) as unknown as ProcedureExecutionsRow[];
}

// =============================================================================
// STEPEXECUTIONS TABLE
// Concrete executions of specified steps. Maps to pko:StepExecution, hasExecutedStep, includesStepExecution, and nextStepExecution.
// =============================================================================

/** A row in the StepExecutions table. */
export interface StepExecutionsRow {
  /** Stored logical identifier for one StepExecutions row. */
  step_execution_id: string;
  /** Human-readable calculated display alias for the StepExecutions row. */
  name: string | null;
  /** Parent procedure execution. */
  procedure_execution: string | null;
  /** Specified step executed; maps to pko:hasExecutedStep. */
  step: string | null;
  /** Agent executing the step; maps to pko:wasExecutedBy. */
  executed_by_agent: string | null;
  /** InProgress, Completed, Paused, Cancelled, or Failed. */
  execution_status: string | null;
  /** Execution start. */
  started_at: string | null;
  /** Execution end. */
  ended_at: string | null;
  /** PASS, WARN, FAIL, or PENDING. */
  verification_result: string | null;
  /** Observed deviation from the specification. */
  deviation: string | null;
  /** Observed duration in minutes. */
  actual_duration_minutes: number | null;
  /** Expected duration from the specification. */
  expected_duration_minutes: number | null;
  /** TRUE when actual duration exceeds expected duration. */
  is_late: boolean | null;
  /** How many blocking requirements attached to this step execution are recorded as not fully satisfied. */
  blocking_unmet_count: number | null;
  /** Count of blocking-but-unmet requirements on this step execution, computed via the single-criterion key so no criterion is dropped. */
  blocking_unmet_count_safe: number | null;
  /** TRUE when a step execution reached Completed even though a blocking requirement on it was never fully satisfied. */
  proceeded_past_blocking_control: boolean | null;
  /** How many blocking requirements this execution's step was supposed to have evaluated. */
  expected_blocking_count: number | null;
  /** How many blocking requirements actually received a satisfaction record on this step execution, at any level. */
  evaluated_blocking_count: number | null;
  /** Blocking requirements that were bound to this step but never assessed on this execution. */
  unevaluated_blocking_count: number | null;
  /** TRUE when at least one blocking control bound to this step was never evaluated on this execution. */
  has_unevaluated_blocking_control: boolean | null;
  /** Number of stale authoritative sources bound to the step this execution ran. */
  stale_authoritative_source_count: number | null;
  /** TRUE when this execution's step depends on at least one authoritative source that is outside its freshness SLA. */
  ran_on_stale_authoritative_source: boolean | null;
  /** TRUE when a human recorded some deviation narrative on this execution. */
  has_deviation_note: boolean | null;
  /** TRUE when the execution exceeded its expected duration and no deviation was recorded. */
  is_late_and_unexplained: boolean | null;
  /** How many active exceptions were available to this execution's step. */
  available_exception_count_for_step: number | null;
  /** TRUE when an execution ran long with no explanation despite the specification defining an active exception for exactly that situation. */
  had_uninvoked_exception_available: boolean | null;
  /** How many verifications this execution was supposed to perform. */
  expected_verification_count: number | null;
  /** How many verification outcomes were actually recorded against this execution. */
  performed_verification_count: number | null;
  /** Declared verifications with no recorded outcome on this execution. */
  skipped_verification_count: number | null;
  /** TRUE when a declared verification was never performed on this execution. */
  has_skipped_verification: boolean | null;
  /** TRUE when an execution asserts PASS while at least one declared verification has no recorded outcome. */
  claims_pass_without_evidence: boolean | null;
  /** Whether this execution ran a preparation step. */
  step_is_preparation: boolean | null;
  /** Whether this execution ran an approval step. */
  step_is_approval: boolean | null;
  /** Composite execution+agent key, emitted only for preparation steps. */
  preparer_agent_key: string | null;
  /** Composite execution+agent key, emitted only for approval steps. */
  approver_agent_key: string | null;
  /** On an approval row, how many preparation steps in the SAME procedure execution were run by this same agent. */
  prepared_by_this_agent_count: number | null;
  /** TRUE when the agent approving this step also prepared work earlier in the same procedure execution. */
  violates_separation_of_duties: boolean | null;
  /** The role the specification assigns to this step. */
  required_role_for_step: string | null;
  /** The agent+role pair that would need to exist as a valid assignment for this execution to be properly authorized. */
  executor_role_key: string | null;
  /** How many currently-valid role assignments grant this executor the role their step required. */
  executor_authority_count: number | null;
  /** TRUE when the executing agent holds a currently-valid assignment to the role the step required. */
  executor_held_required_role: boolean | null;
  /** TRUE when an approval step was executed by an agent who does not hold the required approving role. */
  is_unauthorized_approval: boolean | null;
  /** Echoes the parent execution id only for completed steps. */
  completed_execution_key: string | null;
  /** Echoes the parent execution id when this step execution carries ANY control breach. */
  control_breach_execution_key: string | null;
  /** Echoes the parent execution id only for steps that ran past their expected duration. */
  late_execution_key: string | null;
  /** Human, AIAgent, or AutomatedPipeline — what kind of agent actually ran this step. */
  executor_agent_kind: string | null;
  /** TRUE when a human executed this step. */
  executor_is_human: boolean | null;
  /** Whether the specification requires this step to be human-confirmed. */
  step_requires_human_confirmation: boolean | null;
  /** TRUE when a step requiring human confirmation was executed by an AI agent or automated pipeline. */
  non_human_ran_human_step: boolean | null;
  /** TRUE when an approval-authority step was executed by a non-human agent, regardless of the RequiresHumanConfirmation flag. */
  non_human_approval: boolean | null;
  /** Echoes the parent execution id when this step left a blocking control unevaluated. */
  unevaluated_blocking_execution_key: string | null;
  /** Echoes the parent execution id when this step execution violates segregation of duties. */
  separation_violation_execution_key: string | null;
  /** How many of this execution's verifications were observed by the same agent who ran the step. */
  self_witnessed_verification_count: number | null;
  /** How many verifications on this execution recorded a matching signal with no retained evidence artifact. */
  unbacked_verification_count: number | null;
  /** TRUE when an approval step's verification was either self-witnessed by the approver or never performed at all. */
  approval_rests_on_self_attestation: boolean | null;
  /** How many specified exceptions were invoked during this step execution. */
  exception_invocation_count: number | null;
  /** TRUE when this execution invoked at least one specified exception. */
  ran_under_exception: boolean | null;
  /** TRUE when this step execution reached a completed state. */
  is_completed: boolean | null;
  /** TRUE only when this step execution's verification actually passed. PENDING and FAIL are both not-passed. */
  is_verification_passed: boolean | null;
  /** TRUE for executions of the legal and privacy review step. */
  is_legal_review_step: boolean | null;
  /** Carries the execution id only when this row is a PASSED legal review; empty string otherwise. */
  cleared_legal_review_key: string | null;
  /** The role the specification assigns to this step. */
  assigned_role: string | null;
  /** The agent currently designated to hold the step's assigned role. */
  role_current_agent: string | null;
  /** TRUE when the agent that executed the step is the agent currently designated for the step's role. */
  executor_is_designated_agent: boolean | null;
  /** Freshness verdict for the step this execution ran. */
  inputs_were_fresh_at_run: boolean | null;
  /** TRUE when an execution completed even though its step's authoritative inputs are stale. */
  ran_on_stale_inputs: boolean | null;
  /** Number of unresolved issue occurrences recorded against this execution. */
  unresolved_issue_count: number | null;
  /** TRUE when a deviation from the specification was recorded. */
  has_deviation: boolean | null;
  /** TRUE when this execution passed verification, deviated from nothing, left no open issue, and finished on time. */
  is_clean: boolean | null;
  /** Echoes the parent execution id when this step execution is not clean, blank otherwise. */
  procedure_execution_when_unclean: string | null;
  /** Number of requirements actually scored against this execution. */
  evaluated_requirement_count: number | null;
  /** Number of blocking requirements the specification demands for this step. */
  required_blocking_count: number | null;
  /** TRUE when fewer requirements were scored than the step has blocking requirements. */
  has_unevaluated_blocking_requirement: boolean | null;
  /** The kind of agent that actually performed this step execution. */
  executing_agent_kind: string | null;
  /** TRUE when software actually performed this step. */
  was_executed_by_software: boolean | null;
  /** Whether the step being executed was specified as a software step. */
  step_is_software_assigned: boolean | null;
  /** TRUE when software performed a step the procedure specified for a human. */
  software_did_human_work: boolean | null;
  /** Whether this execution is of a designated human approval gate. */
  is_approval_execution: boolean | null;
  /** TRUE when this execution recorded a positive verification outcome. */
  is_verified: boolean | null;
  /** Number of material non-human decisions in this execution that no human confirmed. */
  unconfirmed_non_human_decision_count: number | null;
  /** Whether the specification requires human confirmation for this step. */
  requires_human_confirmation: boolean | null;
  /** TRUE when a step requiring human confirmation contains unconfirmed material non-human decisions. */
  human_confirmation_missing: boolean | null;
  /** TRUE when a drafting execution completed despite an unapproved or stale source at its step. */
  drafted_from_unusable_source: boolean | null;
  /** Source-usability verdict for the step this execution ran. */
  inputs_were_usable: boolean | null;
  /** Composite-key echo: the step this execution ran when it was carried out by software, blank otherwise. */
  software_execution_step_key: string | null;
  /** The control kind of the step this execution ran. */
  step_control_kind: string | null;
  /** How many of this step's blocking clearances came from controls that have never once failed. */
  unfalsified_clearance_count: number | null;
  /** TRUE when every blocking control evaluated on this step is one that has never returned a negative result. */
  all_clearances_are_unfalsified: boolean | null;
  /** How many authoritative sources this step read that were already out of SLA when it read them. */
  stale_at_run_count: number | null;
  /** TRUE when this execution consumed at least one authoritative source that was stale at the time of reading. */
  was_stale_when_i_ran_it: boolean | null;
  /** TRUE when the as-of-now staleness verdict disagrees with the as-of-run verdict for the same step. */
  staleness_answer_is_tense_dependent: boolean | null;
  /** TRUE when the specification declared at least one verification or one blocking requirement for this step. */
  has_any_declared_check: boolean | null;
  /** Total number of checks actually carried out on this execution, verifications plus blocking-control evaluations. */
  performed_check_count: number | null;
  /** Total number of checks the specification called for on this step. */
  declared_check_count: number | null;
  /** TRUE when the specification asked for no verification and no blocking control on this step at all. */
  is_unchecked_by_design: boolean | null;
  /** TRUE when this execution reads clean and nothing was ever declared that could have made it read otherwise. */
  is_vacuously_clean: boolean | null;
  /** TRUE when this execution is clean AND every declared check was actually performed. */
  is_substantively_clean: boolean | null;
  /** Echoes the parent execution id when this step is vacuously clean. */
  vacuously_clean_execution_key: string | null;
  /** How many of this step's passing verifications were observed by the executor with no evidence attached. */
  uncorroborated_pass_count: number | null;
  /** TRUE when every verification performed on this step was an uncorroborated self-witnessed pass. */
  evidence_position_is_weak: boolean | null;
  /** Echoes the parent execution id when this step execution is a preparation step. */
  preparation_execution_key: string | null;
  /** Echoes the parent execution id when this step execution is an approval step. */
  approval_execution_key: string | null;
  /** TRUE when this step's departure from spec is covered by an invoked exception or an approved change request against its procedure version. */
  has_governing_instrument: boolean | null;
  /** Whether the procedure version this step belongs to has an approved change request that could account for a departure. */
  has_approved_change_coverage: boolean | null;
  /** The procedure version this step execution's specification step belongs to. */
  version_of_step: string | null;
  /** TRUE when this step departed from specification and no exception or approved change covers it. */
  is_ungoverned_divergence: boolean | null;
  /** Echoes the parent execution id when this step diverged without governance. */
  ungoverned_divergence_execution_key: string | null;
  /** Echoes the parent execution id when this approval rested on self-attestation. */
  self_attested_approval_execution_key: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const stepExecutionsFieldTypes: Record<string, FieldType> = {
  step_execution_id: "string",
  name: "*string",
  procedure_execution: "*string",
  step: "*string",
  executed_by_agent: "*string",
  execution_status: "*string",
  started_at: "*string",
  ended_at: "*string",
  verification_result: "*string",
  deviation: "*string",
  actual_duration_minutes: "*int",
  expected_duration_minutes: "*int",
  is_late: "*bool",
  blocking_unmet_count: "*float64",
  blocking_unmet_count_safe: "*float64",
  proceeded_past_blocking_control: "*bool",
  expected_blocking_count: "*float64",
  evaluated_blocking_count: "*float64",
  unevaluated_blocking_count: "*float64",
  has_unevaluated_blocking_control: "*bool",
  stale_authoritative_source_count: "*float64",
  ran_on_stale_authoritative_source: "*bool",
  has_deviation_note: "*bool",
  is_late_and_unexplained: "*bool",
  available_exception_count_for_step: "*float64",
  had_uninvoked_exception_available: "*bool",
  expected_verification_count: "*float64",
  performed_verification_count: "*float64",
  skipped_verification_count: "*float64",
  has_skipped_verification: "*bool",
  claims_pass_without_evidence: "*bool",
  step_is_preparation: "*bool",
  step_is_approval: "*bool",
  preparer_agent_key: "*string",
  approver_agent_key: "*string",
  prepared_by_this_agent_count: "*float64",
  violates_separation_of_duties: "*bool",
  required_role_for_step: "*string",
  executor_role_key: "*string",
  executor_authority_count: "*float64",
  executor_held_required_role: "*bool",
  is_unauthorized_approval: "*bool",
  completed_execution_key: "*string",
  control_breach_execution_key: "*string",
  late_execution_key: "*string",
  executor_agent_kind: "*string",
  executor_is_human: "*bool",
  step_requires_human_confirmation: "*bool",
  non_human_ran_human_step: "*bool",
  non_human_approval: "*bool",
  unevaluated_blocking_execution_key: "*string",
  separation_violation_execution_key: "*string",
  self_witnessed_verification_count: "*float64",
  unbacked_verification_count: "*float64",
  approval_rests_on_self_attestation: "*bool",
  exception_invocation_count: "*float64",
  ran_under_exception: "*bool",
  is_completed: "*bool",
  is_verification_passed: "*bool",
  is_legal_review_step: "*bool",
  cleared_legal_review_key: "*string",
  assigned_role: "*string",
  role_current_agent: "*string",
  executor_is_designated_agent: "*bool",
  inputs_were_fresh_at_run: "*bool",
  ran_on_stale_inputs: "*bool",
  unresolved_issue_count: "*float64",
  has_deviation: "*bool",
  is_clean: "*bool",
  procedure_execution_when_unclean: "*string",
  evaluated_requirement_count: "*float64",
  required_blocking_count: "*int",
  has_unevaluated_blocking_requirement: "*bool",
  executing_agent_kind: "*string",
  was_executed_by_software: "*bool",
  step_is_software_assigned: "*bool",
  software_did_human_work: "*bool",
  is_approval_execution: "*bool",
  is_verified: "*bool",
  unconfirmed_non_human_decision_count: "*float64",
  requires_human_confirmation: "*bool",
  human_confirmation_missing: "*bool",
  drafted_from_unusable_source: "*bool",
  inputs_were_usable: "*bool",
  software_execution_step_key: "*string",
  step_control_kind: "*string",
  unfalsified_clearance_count: "*float64",
  all_clearances_are_unfalsified: "*bool",
  stale_at_run_count: "*float64",
  was_stale_when_i_ran_it: "*bool",
  staleness_answer_is_tense_dependent: "*bool",
  has_any_declared_check: "*bool",
  performed_check_count: "*float64",
  declared_check_count: "*float64",
  is_unchecked_by_design: "*bool",
  is_vacuously_clean: "*bool",
  is_substantively_clean: "*bool",
  vacuously_clean_execution_key: "*string",
  uncorroborated_pass_count: "*float64",
  evidence_position_is_weak: "*bool",
  preparation_execution_key: "*string",
  approval_execution_key: "*string",
  has_governing_instrument: "*bool",
  has_approved_change_coverage: "*bool",
  version_of_step: "*string",
  is_ungoverned_divergence: "*bool",
  ungoverned_divergence_execution_key: "*string",
  self_attested_approval_execution_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StepExecutions row.
 *  Formula: ={{ProcedureExecution}} & " / " & {{Step}} */
export function calcStepExecutionsName(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_execution)), vS(" / "), erbTextOr(vStr(tc.step))));
}

/** Computes the ActualDurationMinutes calculated field.
 *  Observed duration in minutes.
 *  Formula: =IF({{EndedAt}} = "", 0, DATETIME_DIFF({{EndedAt}}, {{StartedAt}}, "minutes")) */
export function calcStepExecutionsActualDurationMinutes(tc: StepExecutionsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbIsBlank(vStr(tc.ended_at))), () => vI(0), () => erbDatetimeDiff(vStr(tc.ended_at), vStr(tc.started_at), vS("minutes")))));
}

/** Computes the IsLate calculated field.
 *  TRUE when actual duration exceeds expected duration.
 *  Formula: ={{ActualDurationMinutes}} > {{ExpectedDurationMinutes}} */
export function calcStepExecutionsIsLate(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.actual_duration_minutes), ">", vInt(tc.expected_duration_minutes)));
}

/** Computes the ProceededPastBlockingControl calculated field.
 *  TRUE when a step execution reached Completed even though a blocking requirement on it was never fully satisfied.
 *  Formula: =AND({{ExecutionStatus}} = "Completed", {{BlockingUnmetCountSafe}} > 0) */
export function calcStepExecutionsProceededPastBlockingControl(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.execution_status)), vS("Completed"))), erbBool3(erbCmp(vNum(tc.blocking_unmet_count_safe), ">", vI(0)))));
}

/** Computes the UnevaluatedBlockingCount calculated field.
 *  Blocking requirements that were bound to this step but never assessed on this execution.
 *  Formula: ={{ExpectedBlockingCount}} - {{EvaluatedBlockingCount}} */
export function calcStepExecutionsUnevaluatedBlockingCount(tc: StepExecutionsRow): number | null {
  return toFloatPtr(erbSub(vNum(tc.expected_blocking_count), vNum(tc.evaluated_blocking_count)));
}

/** Computes the HasUnevaluatedBlockingControl calculated field.
 *  TRUE when at least one blocking control bound to this step was never evaluated on this execution.
 *  Formula: ={{UnevaluatedBlockingCount}} > 0 */
export function calcStepExecutionsHasUnevaluatedBlockingControl(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.unevaluated_blocking_count), ">", vI(0)));
}

/** Computes the RanOnStaleAuthoritativeSource calculated field.
 *  TRUE when this execution's step depends on at least one authoritative source that is outside its freshness SLA.
 *  Formula: ={{StaleAuthoritativeSourceCount}} > 0 */
export function calcStepExecutionsRanOnStaleAuthoritativeSource(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.stale_authoritative_source_count), ">", vI(0)));
}

/** Computes the HasDeviationNote calculated field.
 *  TRUE when a human recorded some deviation narrative on this execution.
 *  Formula: ={{Deviation}} <> "" */
export function calcStepExecutionsHasDeviationNote(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.deviation)));
}

/** Computes the IsLateAndUnexplained calculated field.
 *  TRUE when the execution exceeded its expected duration and no deviation was recorded.
 *  Formula: =AND({{IsLate}}, NOT({{HasDeviationNote}})) */
export function calcStepExecutionsIsLateAndUnexplained(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_late)), erbBool3(erbNot(erbBool3(vBool(tc.has_deviation_note))))));
}

/** Computes the HadUninvokedExceptionAvailable calculated field.
 *  TRUE when an execution ran long with no explanation despite the specification defining an active exception for exactly that situation.
 *  Formula: =AND({{IsLateAndUnexplained}}, {{AvailableExceptionCountForStep}} > 0) */
export function calcStepExecutionsHadUninvokedExceptionAvailable(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_late_and_unexplained)), erbBool3(erbCmp(vNum(tc.available_exception_count_for_step), ">", vI(0)))));
}

/** Computes the SkippedVerificationCount calculated field.
 *  Declared verifications with no recorded outcome on this execution.
 *  Formula: ={{ExpectedVerificationCount}} - {{PerformedVerificationCount}} */
export function calcStepExecutionsSkippedVerificationCount(tc: StepExecutionsRow): number | null {
  return toFloatPtr(erbSub(vNum(tc.expected_verification_count), vNum(tc.performed_verification_count)));
}

/** Computes the HasSkippedVerification calculated field.
 *  TRUE when a declared verification was never performed on this execution.
 *  Formula: ={{SkippedVerificationCount}} > 0 */
export function calcStepExecutionsHasSkippedVerification(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.skipped_verification_count), ">", vI(0)));
}

/** Computes the ClaimsPassWithoutEvidence calculated field.
 *  TRUE when an execution asserts PASS while at least one declared verification has no recorded outcome.
 *  Formula: =AND({{VerificationResult}} = "PASS", {{HasSkippedVerification}}) */
export function calcStepExecutionsClaimsPassWithoutEvidence(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.verification_result)), vS("PASS"))), erbBool3(vBool(tc.has_skipped_verification))));
}

/** Computes the PreparerAgentKey calculated field.
 *  Composite execution+agent key, emitted only for preparation steps.
 *  Formula: =IF({{StepIsPreparation}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, "") */
export function calcStepExecutionsPreparerAgentKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.step_is_preparation)), () => erbConcat(erbTextOr(vStr(tc.procedure_execution)), vS("|"), erbTextOr(vStr(tc.executed_by_agent))), () => vS("")));
}

/** Computes the ApproverAgentKey calculated field.
 *  Composite execution+agent key, emitted only for approval steps.
 *  Formula: =IF({{StepIsApproval}}, {{ProcedureExecution}} & "|" & {{ExecutedByAgent}}, "") */
export function calcStepExecutionsApproverAgentKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.step_is_approval)), () => erbConcat(erbTextOr(vStr(tc.procedure_execution)), vS("|"), erbTextOr(vStr(tc.executed_by_agent))), () => vS("")));
}

/** Computes the ViolatesSeparationOfDuties calculated field.
 *  TRUE when the agent approving this step also prepared work earlier in the same procedure execution.
 *  Formula: =AND({{StepIsApproval}}, {{PreparedByThisAgentCount}} > 0) */
export function calcStepExecutionsViolatesSeparationOfDuties(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.step_is_approval)), erbBool3(erbCmp(vNum(tc.prepared_by_this_agent_count), ">", vI(0)))));
}

/** Computes the ExecutorRoleKey calculated field.
 *  The agent+role pair that would need to exist as a valid assignment for this execution to be properly authorized.
 *  Formula: ={{ExecutedByAgent}} & "|" & {{RequiredRoleForStep}} */
export function calcStepExecutionsExecutorRoleKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.executed_by_agent)), vS("|"), erbTextOr(vStr(tc.required_role_for_step))));
}

/** Computes the ExecutorHeldRequiredRole calculated field.
 *  TRUE when the executing agent holds a currently-valid assignment to the role the step required.
 *  Formula: ={{ExecutorAuthorityCount}} > 0 */
export function calcStepExecutionsExecutorHeldRequiredRole(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.executor_authority_count), ">", vI(0)));
}

/** Computes the IsUnauthorizedApproval calculated field.
 *  TRUE when an approval step was executed by an agent who does not hold the required approving role.
 *  Formula: =AND({{StepIsApproval}}, NOT({{ExecutorHeldRequiredRole}})) */
export function calcStepExecutionsIsUnauthorizedApproval(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.step_is_approval)), erbBool3(erbNot(erbBool3(vBool(tc.executor_held_required_role))))));
}

/** Computes the CompletedExecutionKey calculated field.
 *  Echoes the parent execution id only for completed steps.
 *  Formula: =IF({{ExecutionStatus}} = "Completed", {{ProcedureExecution}}, "") */
export function calcStepExecutionsCompletedExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(erbNullif(vStr(tc.execution_status)), vS("Completed"))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the ControlBreachExecutionKey calculated field.
 *  Echoes the parent execution id when this step execution carries ANY control breach.
 *  Formula: =IF(OR({{ProceededPastBlockingControl}}, {{ViolatesSeparationOfDuties}}, {{IsUnauthorizedApproval}}, {{ClaimsPassWithoutEvidence}}), {{ProcedureExecution}}, "") */
export function calcStepExecutionsControlBreachExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbOr(erbBool3(vBool(tc.proceeded_past_blocking_control)), erbBool3(vBool(tc.violates_separation_of_duties)), erbBool3(vBool(tc.is_unauthorized_approval)), erbBool3(vBool(tc.claims_pass_without_evidence)))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the LateExecutionKey calculated field.
 *  Echoes the parent execution id only for steps that ran past their expected duration.
 *  Formula: =IF({{IsLate}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsLateExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_late)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the ExecutorIsHuman calculated field.
 *  TRUE when a human executed this step.
 *  Formula: ={{ExecutorAgentKind}} = "Human" */
export function calcStepExecutionsExecutorIsHuman(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.executor_agent_kind), vS("Human")));
}

/** Computes the NonHumanRanHumanStep calculated field.
 *  TRUE when a step requiring human confirmation was executed by an AI agent or automated pipeline.
 *  Formula: =AND({{StepRequiresHumanConfirmation}}, NOT({{ExecutorIsHuman}})) */
export function calcStepExecutionsNonHumanRanHumanStep(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.step_requires_human_confirmation)), erbBool3(erbNot(erbBool3(vBool(tc.executor_is_human))))));
}

/** Computes the NonHumanApproval calculated field.
 *  TRUE when an approval-authority step was executed by a non-human agent, regardless of the RequiresHumanConfirmation flag.
 *  Formula: =AND({{StepIsApproval}}, NOT({{ExecutorIsHuman}})) */
export function calcStepExecutionsNonHumanApproval(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.step_is_approval)), erbBool3(erbNot(erbBool3(vBool(tc.executor_is_human))))));
}

/** Computes the UnevaluatedBlockingExecutionKey calculated field.
 *  Echoes the parent execution id when this step left a blocking control unevaluated.
 *  Formula: =IF({{HasUnevaluatedBlockingControl}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsUnevaluatedBlockingExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.has_unevaluated_blocking_control)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the SeparationViolationExecutionKey calculated field.
 *  Echoes the parent execution id when this step execution violates segregation of duties.
 *  Formula: =IF({{ViolatesSeparationOfDuties}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsSeparationViolationExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.violates_separation_of_duties)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the ApprovalRestsOnSelfAttestation calculated field.
 *  TRUE when an approval step's verification was either self-witnessed by the approver or never performed at all.
 *  Formula: =AND({{StepIsApproval}}, OR({{SelfWitnessedVerificationCount}} > 0, {{HasSkippedVerification}})) */
export function calcStepExecutionsApprovalRestsOnSelfAttestation(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.step_is_approval)), erbBool3(erbOr(erbBool3(erbCmp(vNum(tc.self_witnessed_verification_count), ">", vI(0))), erbBool3(vBool(tc.has_skipped_verification))))));
}

/** Computes the RanUnderException calculated field.
 *  TRUE when this execution invoked at least one specified exception.
 *  Formula: ={{ExceptionInvocationCount}} > 0 */
export function calcStepExecutionsRanUnderException(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.exception_invocation_count), ">", vI(0)));
}

/** Computes the IsCompleted calculated field.
 *  TRUE when this step execution reached a completed state.
 *  Formula: ={{ExecutionStatus}} = "Completed" */
export function calcStepExecutionsIsCompleted(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.execution_status)), vS("Completed")));
}

/** Computes the IsVerificationPassed calculated field.
 *  TRUE only when this step execution's verification actually passed. PENDING and FAIL are both not-passed.
 *  Formula: ={{VerificationResult}} = "PASS" */
export function calcStepExecutionsIsVerificationPassed(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.verification_result)), vS("PASS")));
}

/** Computes the IsLegalReviewStep calculated field.
 *  TRUE for executions of the legal and privacy review step.
 *  Formula: ={{Step}} = "policy-04" */
export function calcStepExecutionsIsLegalReviewStep(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.step)), vS("policy-04")));
}

/** Computes the ClearedLegalReviewKey calculated field.
 *  Carries the execution id only when this row is a PASSED legal review; empty string otherwise.
 *  Formula: =IF(AND({{IsLegalReviewStep}}, {{IsVerificationPassed}}), {{ProcedureExecution}}, "") */
export function calcStepExecutionsClearedLegalReviewKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.is_legal_review_step)), erbBool3(vBool(tc.is_verification_passed)))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the ExecutorIsDesignatedAgent calculated field.
 *  TRUE when the agent that executed the step is the agent currently designated for the step's role.
 *  Formula: ={{ExecutedByAgent}} = {{RoleCurrentAgent}} */
export function calcStepExecutionsExecutorIsDesignatedAgent(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.executed_by_agent)), vStr(tc.role_current_agent)));
}

/** Computes the RanOnStaleInputs calculated field.
 *  TRUE when an execution completed even though its step's authoritative inputs are stale.
 *  Formula: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereFreshAtRun}})) */
export function calcStepExecutionsRanOnStaleInputs(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.execution_status)), vS("Completed"))), erbBool3(erbNot(erbBool3(vBool(tc.inputs_were_fresh_at_run))))));
}

/** Computes the HasDeviation calculated field.
 *  TRUE when a deviation from the specification was recorded.
 *  Formula: ={{Deviation}} <> "" */
export function calcStepExecutionsHasDeviation(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.deviation)));
}

/** Computes the IsClean calculated field.
 *  TRUE when this execution passed verification, deviated from nothing, left no open issue, and finished on time.
 *  Formula: =AND({{VerificationResult}} = "PASS", NOT({{HasDeviation}}), {{UnresolvedIssueCount}} = 0, NOT({{IsLate}})) */
export function calcStepExecutionsIsClean(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.verification_result)), vS("PASS"))), erbBool3(erbNot(erbBool3(vBool(tc.has_deviation)))), erbBool3(erbEq(vNum(tc.unresolved_issue_count), vI(0))), erbBool3(erbNot(erbBool3(vBool(tc.is_late))))));
}

/** Computes the ProcedureExecutionWhenUnclean calculated field.
 *  Echoes the parent execution id when this step execution is not clean, blank otherwise.
 *  Formula: =IF({{IsClean}}, "", {{ProcedureExecution}}) */
export function calcStepExecutionsProcedureExecutionWhenUnclean(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_clean)), () => vS(""), () => vStr(tc.procedure_execution)));
}

/** Computes the HasUnevaluatedBlockingRequirement calculated field.
 *  TRUE when fewer requirements were scored than the step has blocking requirements.
 *  Formula: ={{EvaluatedRequirementCount}} < {{RequiredBlockingCount}} */
export function calcStepExecutionsHasUnevaluatedBlockingRequirement(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.evaluated_requirement_count), "<", vInt(tc.required_blocking_count)));
}

/** Computes the WasExecutedBySoftware calculated field.
 *  TRUE when software actually performed this step.
 *  Formula: =OR({{ExecutingAgentKind}} = "AIAgent", {{ExecutingAgentKind}} = "AutomatedPipeline") */
export function calcStepExecutionsWasExecutedBySoftware(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(vStr(tc.executing_agent_kind), vS("AIAgent"))), erbBool3(erbEq(vStr(tc.executing_agent_kind), vS("AutomatedPipeline")))));
}

/** Computes the SoftwareDidHumanWork calculated field.
 *  TRUE when software performed a step the procedure specified for a human.
 *  Formula: =AND({{WasExecutedBySoftware}}, NOT({{StepIsSoftwareAssigned}})) */
export function calcStepExecutionsSoftwareDidHumanWork(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_executed_by_software)), erbBool3(erbNot(erbBool3(vBool(tc.step_is_software_assigned))))));
}

/** Computes the IsVerified calculated field.
 *  TRUE when this execution recorded a positive verification outcome.
 *  Formula: =AND({{VerificationResult}} <> "", {{VerificationResult}} <> "PENDING", {{VerificationResult}} <> "FAIL") */
export function calcStepExecutionsIsVerified(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.verification_result))), erbBool3(erbNe(erbNullif(vStr(tc.verification_result)), vS("PENDING"))), erbBool3(erbNe(erbNullif(vStr(tc.verification_result)), vS("FAIL")))));
}

/** Computes the HumanConfirmationMissing calculated field.
 *  TRUE when a step requiring human confirmation contains unconfirmed material non-human decisions.
 *  Formula: =AND({{RequiresHumanConfirmation}}, {{UnconfirmedNonHumanDecisionCount}} > 0) */
export function calcStepExecutionsHumanConfirmationMissing(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.requires_human_confirmation)), erbBool3(erbCmp(vNum(tc.unconfirmed_non_human_decision_count), ">", vI(0)))));
}

/** Computes the DraftedFromUnusableSource calculated field.
 *  TRUE when a drafting execution completed despite an unapproved or stale source at its step.
 *  Formula: =AND({{ExecutionStatus}} = "Completed", NOT({{InputsWereUsable}})) */
export function calcStepExecutionsDraftedFromUnusableSource(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.execution_status)), vS("Completed"))), erbBool3(erbNot(erbBool3(vBool(tc.inputs_were_usable))))));
}

/** Computes the SoftwareExecutionStepKey calculated field.
 *  Composite-key echo: the step this execution ran when it was carried out by software, blank otherwise.
 *  Formula: =IF({{WasExecutedBySoftware}}, {{Step}}, "") */
export function calcStepExecutionsSoftwareExecutionStepKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_executed_by_software)), () => vStr(tc.step), () => vS("")));
}

/** Computes the AllClearancesAreUnfalsified calculated field.
 *  TRUE when every blocking control evaluated on this step is one that has never returned a negative result.
 *  Formula: =AND({{EvaluatedBlockingCount}} > 0, {{UnfalsifiedClearanceCount}} >= {{EvaluatedBlockingCount}}) */
export function calcStepExecutionsAllClearancesAreUnfalsified(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.evaluated_blocking_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.unfalsified_clearance_count), ">=", vNum(tc.evaluated_blocking_count)))));
}

/** Computes the WasStaleWhenIRanIt calculated field.
 *  TRUE when this execution consumed at least one authoritative source that was stale at the time of reading.
 *  Formula: ={{StaleAtRunCount}} > 0 */
export function calcStepExecutionsWasStaleWhenIRanIt(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.stale_at_run_count), ">", vI(0)));
}

/** Computes the StalenessAnswerIsTenseDependent calculated field.
 *  TRUE when the as-of-now staleness verdict disagrees with the as-of-run verdict for the same step.
 *  Formula: =NOT({{WasStaleWhenIRanIt}} = {{RanOnStaleAuthoritativeSource}}) */
export function calcStepExecutionsStalenessAnswerIsTenseDependent(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(vBool(tc.was_stale_when_i_ran_it), vBool(tc.ran_on_stale_authoritative_source)))));
}

/** Computes the HasAnyDeclaredCheck calculated field.
 *  TRUE when the specification declared at least one verification or one blocking requirement for this step.
 *  Formula: =OR({{ExpectedVerificationCount}} > 0, {{ExpectedBlockingCount}} > 0) */
export function calcStepExecutionsHasAnyDeclaredCheck(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbCmp(vNum(tc.expected_verification_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.expected_blocking_count), ">", vI(0)))));
}

/** Computes the PerformedCheckCount calculated field.
 *  Total number of checks actually carried out on this execution, verifications plus blocking-control evaluations.
 *  Formula: ={{PerformedVerificationCount}} + {{EvaluatedBlockingCount}} */
export function calcStepExecutionsPerformedCheckCount(tc: StepExecutionsRow): number | null {
  return toFloatPtr(erbAdd(vNum(tc.performed_verification_count), vNum(tc.evaluated_blocking_count)));
}

/** Computes the DeclaredCheckCount calculated field.
 *  Total number of checks the specification called for on this step.
 *  Formula: ={{ExpectedVerificationCount}} + {{ExpectedBlockingCount}} */
export function calcStepExecutionsDeclaredCheckCount(tc: StepExecutionsRow): number | null {
  return toFloatPtr(erbAdd(vNum(tc.expected_verification_count), vNum(tc.expected_blocking_count)));
}

/** Computes the IsUncheckedByDesign calculated field.
 *  TRUE when the specification asked for no verification and no blocking control on this step at all.
 *  Formula: ={{DeclaredCheckCount}} = 0 */
export function calcStepExecutionsIsUncheckedByDesign(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.declared_check_count), vI(0)));
}

/** Computes the IsVacuouslyClean calculated field.
 *  TRUE when this execution reads clean and nothing was ever declared that could have made it read otherwise.
 *  Formula: =AND({{IsClean}}, {{IsUncheckedByDesign}}) */
export function calcStepExecutionsIsVacuouslyClean(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_clean)), erbBool3(vBool(tc.is_unchecked_by_design))));
}

/** Computes the IsSubstantivelyClean calculated field.
 *  TRUE when this execution is clean AND every declared check was actually performed.
 *  Formula: =AND({{IsClean}}, {{PerformedCheckCount}} >= {{DeclaredCheckCount}}, {{DeclaredCheckCount}} > 0) */
export function calcStepExecutionsIsSubstantivelyClean(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_clean)), erbBool3(erbCmp(vNum(tc.performed_check_count), ">=", vNum(tc.declared_check_count))), erbBool3(erbCmp(vNum(tc.declared_check_count), ">", vI(0)))));
}

/** Computes the VacuouslyCleanExecutionKey calculated field.
 *  Echoes the parent execution id when this step is vacuously clean.
 *  Formula: =IF({{IsVacuouslyClean}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsVacuouslyCleanExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_vacuously_clean)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the EvidencePositionIsWeak calculated field.
 *  TRUE when every verification performed on this step was an uncorroborated self-witnessed pass.
 *  Formula: =AND({{PerformedVerificationCount}} > 0, {{UncorroboratedPassCount}} >= {{PerformedVerificationCount}}) */
export function calcStepExecutionsEvidencePositionIsWeak(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.performed_verification_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.uncorroborated_pass_count), ">=", vNum(tc.performed_verification_count)))));
}

/** Computes the PreparationExecutionKey calculated field.
 *  Echoes the parent execution id when this step execution is a preparation step.
 *  Formula: =IF({{StepIsPreparation}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsPreparationExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.step_is_preparation)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the ApprovalExecutionKey calculated field.
 *  Echoes the parent execution id when this step execution is an approval step.
 *  Formula: =IF({{StepIsApproval}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsApprovalExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.step_is_approval)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the HasGoverningInstrument calculated field.
 *  TRUE when this step's departure from spec is covered by an invoked exception or an approved change request against its procedure version.
 *  Formula: =OR({{RanUnderException}}, {{HasApprovedChangeCoverage}}) */
export function calcStepExecutionsHasGoverningInstrument(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(vBool(tc.ran_under_exception)), erbBool3(vBool(tc.has_approved_change_coverage))));
}

/** Computes the IsUngovernedDivergence calculated field.
 *  TRUE when this step departed from specification and no exception or approved change covers it.
 *  Formula: =AND(OR({{HasDeviation}}, {{IsLate}}, {{ProceededPastBlockingControl}}), NOT({{HasGoverningInstrument}})) */
export function calcStepExecutionsIsUngovernedDivergence(tc: StepExecutionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbOr(erbBool3(vBool(tc.has_deviation)), erbBool3(vBool(tc.is_late)), erbBool3(vBool(tc.proceeded_past_blocking_control)))), erbBool3(erbNot(erbBool3(vBool(tc.has_governing_instrument))))));
}

/** Computes the UngovernedDivergenceExecutionKey calculated field.
 *  Echoes the parent execution id when this step diverged without governance.
 *  Formula: =IF({{IsUngovernedDivergence}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsUngovernedDivergenceExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_ungoverned_divergence)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the SelfAttestedApprovalExecutionKey calculated field.
 *  Echoes the parent execution id when this approval rested on self-attestation.
 *  Formula: =IF({{ApprovalRestsOnSelfAttestation}}, {{ProcedureExecution}}, "") */
export function calcStepExecutionsSelfAttestedApprovalExecutionKey(tc: StepExecutionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.approval_rests_on_self_attestation)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStepExecutions(tc: StepExecutionsRow): StepExecutionsRow {
  // Level 1
  calcGuard(tc, stepExecutionsFieldTypes, "name", () => { tc.name = calcStepExecutionsName(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "actual_duration_minutes", () => { tc.actual_duration_minutes = calcStepExecutionsActualDurationMinutes(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "proceeded_past_blocking_control", () => { tc.proceeded_past_blocking_control = calcStepExecutionsProceededPastBlockingControl(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "unevaluated_blocking_count", () => { tc.unevaluated_blocking_count = calcStepExecutionsUnevaluatedBlockingCount(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "ran_on_stale_authoritative_source", () => { tc.ran_on_stale_authoritative_source = calcStepExecutionsRanOnStaleAuthoritativeSource(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_deviation_note", () => { tc.has_deviation_note = calcStepExecutionsHasDeviationNote(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "skipped_verification_count", () => { tc.skipped_verification_count = calcStepExecutionsSkippedVerificationCount(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "preparer_agent_key", () => { tc.preparer_agent_key = calcStepExecutionsPreparerAgentKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "approver_agent_key", () => { tc.approver_agent_key = calcStepExecutionsApproverAgentKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "violates_separation_of_duties", () => { tc.violates_separation_of_duties = calcStepExecutionsViolatesSeparationOfDuties(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "executor_role_key", () => { tc.executor_role_key = calcStepExecutionsExecutorRoleKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "executor_held_required_role", () => { tc.executor_held_required_role = calcStepExecutionsExecutorHeldRequiredRole(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "completed_execution_key", () => { tc.completed_execution_key = calcStepExecutionsCompletedExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "executor_is_human", () => { tc.executor_is_human = calcStepExecutionsExecutorIsHuman(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "ran_under_exception", () => { tc.ran_under_exception = calcStepExecutionsRanUnderException(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_completed", () => { tc.is_completed = calcStepExecutionsIsCompleted(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_verification_passed", () => { tc.is_verification_passed = calcStepExecutionsIsVerificationPassed(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_legal_review_step", () => { tc.is_legal_review_step = calcStepExecutionsIsLegalReviewStep(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "executor_is_designated_agent", () => { tc.executor_is_designated_agent = calcStepExecutionsExecutorIsDesignatedAgent(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "ran_on_stale_inputs", () => { tc.ran_on_stale_inputs = calcStepExecutionsRanOnStaleInputs(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_deviation", () => { tc.has_deviation = calcStepExecutionsHasDeviation(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_unevaluated_blocking_requirement", () => { tc.has_unevaluated_blocking_requirement = calcStepExecutionsHasUnevaluatedBlockingRequirement(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "was_executed_by_software", () => { tc.was_executed_by_software = calcStepExecutionsWasExecutedBySoftware(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_verified", () => { tc.is_verified = calcStepExecutionsIsVerified(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "human_confirmation_missing", () => { tc.human_confirmation_missing = calcStepExecutionsHumanConfirmationMissing(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "drafted_from_unusable_source", () => { tc.drafted_from_unusable_source = calcStepExecutionsDraftedFromUnusableSource(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "all_clearances_are_unfalsified", () => { tc.all_clearances_are_unfalsified = calcStepExecutionsAllClearancesAreUnfalsified(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "was_stale_when_i_ran_it", () => { tc.was_stale_when_i_ran_it = calcStepExecutionsWasStaleWhenIRanIt(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_any_declared_check", () => { tc.has_any_declared_check = calcStepExecutionsHasAnyDeclaredCheck(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "performed_check_count", () => { tc.performed_check_count = calcStepExecutionsPerformedCheckCount(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "declared_check_count", () => { tc.declared_check_count = calcStepExecutionsDeclaredCheckCount(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "evidence_position_is_weak", () => { tc.evidence_position_is_weak = calcStepExecutionsEvidencePositionIsWeak(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "preparation_execution_key", () => { tc.preparation_execution_key = calcStepExecutionsPreparationExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "approval_execution_key", () => { tc.approval_execution_key = calcStepExecutionsApprovalExecutionKey(tc); });
  // Level 2
  calcGuard(tc, stepExecutionsFieldTypes, "is_late", () => { tc.is_late = calcStepExecutionsIsLate(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_unevaluated_blocking_control", () => { tc.has_unevaluated_blocking_control = calcStepExecutionsHasUnevaluatedBlockingControl(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_skipped_verification", () => { tc.has_skipped_verification = calcStepExecutionsHasSkippedVerification(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_unauthorized_approval", () => { tc.is_unauthorized_approval = calcStepExecutionsIsUnauthorizedApproval(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "non_human_ran_human_step", () => { tc.non_human_ran_human_step = calcStepExecutionsNonHumanRanHumanStep(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "non_human_approval", () => { tc.non_human_approval = calcStepExecutionsNonHumanApproval(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "separation_violation_execution_key", () => { tc.separation_violation_execution_key = calcStepExecutionsSeparationViolationExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "cleared_legal_review_key", () => { tc.cleared_legal_review_key = calcStepExecutionsClearedLegalReviewKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "software_did_human_work", () => { tc.software_did_human_work = calcStepExecutionsSoftwareDidHumanWork(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "software_execution_step_key", () => { tc.software_execution_step_key = calcStepExecutionsSoftwareExecutionStepKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "staleness_answer_is_tense_dependent", () => { tc.staleness_answer_is_tense_dependent = calcStepExecutionsStalenessAnswerIsTenseDependent(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_unchecked_by_design", () => { tc.is_unchecked_by_design = calcStepExecutionsIsUncheckedByDesign(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "has_governing_instrument", () => { tc.has_governing_instrument = calcStepExecutionsHasGoverningInstrument(tc); });
  // Level 3
  calcGuard(tc, stepExecutionsFieldTypes, "is_late_and_unexplained", () => { tc.is_late_and_unexplained = calcStepExecutionsIsLateAndUnexplained(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "claims_pass_without_evidence", () => { tc.claims_pass_without_evidence = calcStepExecutionsClaimsPassWithoutEvidence(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "late_execution_key", () => { tc.late_execution_key = calcStepExecutionsLateExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "unevaluated_blocking_execution_key", () => { tc.unevaluated_blocking_execution_key = calcStepExecutionsUnevaluatedBlockingExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "approval_rests_on_self_attestation", () => { tc.approval_rests_on_self_attestation = calcStepExecutionsApprovalRestsOnSelfAttestation(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_clean", () => { tc.is_clean = calcStepExecutionsIsClean(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_ungoverned_divergence", () => { tc.is_ungoverned_divergence = calcStepExecutionsIsUngovernedDivergence(tc); });
  // Level 4
  calcGuard(tc, stepExecutionsFieldTypes, "had_uninvoked_exception_available", () => { tc.had_uninvoked_exception_available = calcStepExecutionsHadUninvokedExceptionAvailable(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "control_breach_execution_key", () => { tc.control_breach_execution_key = calcStepExecutionsControlBreachExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "procedure_execution_when_unclean", () => { tc.procedure_execution_when_unclean = calcStepExecutionsProcedureExecutionWhenUnclean(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_vacuously_clean", () => { tc.is_vacuously_clean = calcStepExecutionsIsVacuouslyClean(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "is_substantively_clean", () => { tc.is_substantively_clean = calcStepExecutionsIsSubstantivelyClean(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "ungoverned_divergence_execution_key", () => { tc.ungoverned_divergence_execution_key = calcStepExecutionsUngovernedDivergenceExecutionKey(tc); });
  calcGuard(tc, stepExecutionsFieldTypes, "self_attested_approval_execution_key", () => { tc.self_attested_approval_execution_key = calcStepExecutionsSelfAttestedApprovalExecutionKey(tc); });
  // Level 5
  calcGuard(tc, stepExecutionsFieldTypes, "vacuously_clean_execution_key", () => { tc.vacuously_clean_execution_key = calcStepExecutionsVacuouslyCleanExecutionKey(tc); });
  return tc;
}

/** Reads StepExecutions rows from a JSON array file. */
export function loadStepExecutionsRows(file: string): StepExecutionsRow[] {
  return loadRows(file, { fields: stepExecutionsFieldTypes }) as unknown as StepExecutionsRow[];
}

// =============================================================================
// REQUIREMENTSATISFACTIONS TABLE
// Execution-time evaluations of requirements. Maps to pko:RequirementSatisfaction, refersToRequirement, and hasRequirementSatisfactionLevel.
// =============================================================================

/** A row in the RequirementSatisfactions table. */
export interface RequirementSatisfactionsRow {
  /** Stored logical identifier for one RequirementSatisfactions row. */
  requirement_satisfaction_id: string;
  /** Human-readable calculated display alias for the RequirementSatisfactions row. */
  name: string | null;
  /** Execution being evaluated. */
  step_execution: string | null;
  /** Requirement evaluated. */
  requirement: string | null;
  /** NotEvaluated, NotSatisfied, PartiallySatisfied, or Satisfied. */
  satisfaction_level: string | null;
  /** Evidence supporting the evaluation. */
  evidence: string | null;
  /** Agent that evaluated satisfaction. */
  evaluated_by_agent: string | null;
  /** Evaluation timestamp. */
  evaluated_at: string | null;
  /** Whether the requirement being evaluated on this satisfaction row is a blocking control. */
  requirement_is_blocking: boolean | null;
  /** TRUE only when the requirement is recorded as fully Satisfied. PartiallySatisfied, Unsatisfied, Waived, and blank are all FALSE. */
  is_fully_satisfied: boolean | null;
  /** TRUE when a blocking requirement is recorded at anything less than fully Satisfied. This is the control-failure witness at the requirement grain. */
  is_blocking_and_unmet: boolean | null;
  /** Echoes the parent StepExecution id only when this row is a blocking-unmet violation; empty string otherwise. */
  blocking_unmet_step_key: string | null;
  /** Echoes the parent StepExecution id when this satisfaction row concerns a blocking requirement. */
  blocking_satisfaction_step_key: string | null;
  /** Echoes the Requirement id only when this evaluation came out at less than fully Satisfied. */
  negative_outcome_requirement_key: string | null;
  /** What kind of agent evaluated this requirement satisfaction. */
  evaluator_agent_kind: string | null;
  /** TRUE when a blocking requirement was evaluated by a non-human agent. */
  non_human_evaluated_human_control: boolean | null;
  /** Whether the requirement this row evaluates has a computed witness behind it. */
  requirement_has_computed_witness: boolean | null;
  /** TRUE when a blocking requirement is recorded as Satisfied purely on human assertion, with no computed predicate behind it. */
  is_asserted_only: boolean | null;
  /** Echoes the grandparent procedure execution id for assertion-only satisfactions. */
  asserted_only_execution_key: string | null;
  /** The procedure execution this satisfaction ultimately belongs to. */
  parent_procedure_execution: string | null;
  /** Echoes the step-execution id when a satisfaction level was actually recorded, blank otherwise. */
  step_execution_when_scored: string | null;
  /** TRUE when a human evaluated this requirement satisfaction. */
  is_human_evaluated: boolean | null;
  /** The requirement's type, e.g. Approval, Control, Privacy. */
  requirement_is_approval_type: string | null;
  /** TRUE when an approval-type requirement is not fully satisfied, or was not evaluated by a human. */
  is_invalid_approval: boolean | null;
  /** The run this satisfaction belongs to. */
  procedure_execution_of_satisfaction: string | null;
  /** Echoes the run id when this is an invalid approval, blank otherwise. */
  run_when_invalid_approval: string | null;
  /** Whether the requirement behind this satisfaction record has never once returned a negative result. */
  requirement_is_unfalsified: boolean | null;
  /** TRUE when this record cleared a step against a blocking control that has never produced a negative outcome. */
  is_clearance_by_unfalsified_control: boolean | null;
  /** Echoes the step execution id when this clearance came from an unfalsified control. */
  unfalsified_clearance_step_key: string | null;
  /** The specification step behind the step execution this satisfaction was recorded against. */
  spec_step_of_execution: string | null;
  /** The StepRequirements binding this satisfaction record exercised. */
  binding_key: string | null;
  /** The agent who executed the step this satisfaction record scores. */
  scored_step_executor_agent: string | null;
  /** TRUE when the agent who scored this control is the same agent who performed the step being scored. */
  evaluator_is_step_executor: boolean | null;
  /** The agent accountable for the whole procedure execution this satisfaction sits inside. */
  run_owner_agent: string | null;
  /** TRUE when the agent scoring this control is the agent accountable for the execution it belongs to. */
  evaluator_owns_the_run: boolean | null;
  /** TRUE when a blocking control's only evidence is an assertion made by someone with an interest in the outcome. */
  is_interested_party_assertion: boolean | null;
  /** TRUE when this satisfaction record carries any evidence text at all. */
  has_written_evidence: boolean | null;
  /** TRUE when a blocking control was cleared with no computed witness and no written justification. */
  is_bare_assertion: boolean | null;
  /** Echoes the parent execution id when this record is an interested-party assertion. */
  interested_assertion_execution_key: string | null;
  /** TRUE when this record scores a blocking control that has a computed witness behind it. */
  is_computedly_witnessed: boolean | null;
  /** Echoes the parent execution id when this control was computationally witnessed. */
  computed_witness_execution_key: string | null;
  /** The agent who executed the step this satisfaction record scores. */
  step_executor_agent: string | null;
  /** TRUE when this control was scored after the run it belongs to had already been attested. */
  was_scored_after_attestation: boolean | null;
  /** When the parent execution was attested, if it has been. */
  attestation_instant_for_run: string | null;
  /** Echoes the parent execution id when this control was scored after signature. */
  post_attestation_score_execution_key: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const requirementSatisfactionsFieldTypes: Record<string, FieldType> = {
  requirement_satisfaction_id: "string",
  name: "*string",
  step_execution: "*string",
  requirement: "*string",
  satisfaction_level: "*string",
  evidence: "*string",
  evaluated_by_agent: "*string",
  evaluated_at: "*string",
  requirement_is_blocking: "*bool",
  is_fully_satisfied: "*bool",
  is_blocking_and_unmet: "*bool",
  blocking_unmet_step_key: "*string",
  blocking_satisfaction_step_key: "*string",
  negative_outcome_requirement_key: "*string",
  evaluator_agent_kind: "*string",
  non_human_evaluated_human_control: "*bool",
  requirement_has_computed_witness: "*bool",
  is_asserted_only: "*bool",
  asserted_only_execution_key: "*string",
  parent_procedure_execution: "*string",
  step_execution_when_scored: "*string",
  is_human_evaluated: "*bool",
  requirement_is_approval_type: "*string",
  is_invalid_approval: "*bool",
  procedure_execution_of_satisfaction: "*string",
  run_when_invalid_approval: "*string",
  requirement_is_unfalsified: "*bool",
  is_clearance_by_unfalsified_control: "*bool",
  unfalsified_clearance_step_key: "*string",
  spec_step_of_execution: "*string",
  binding_key: "*string",
  scored_step_executor_agent: "*string",
  evaluator_is_step_executor: "*bool",
  run_owner_agent: "*string",
  evaluator_owns_the_run: "*bool",
  is_interested_party_assertion: "*bool",
  has_written_evidence: "*bool",
  is_bare_assertion: "*bool",
  interested_assertion_execution_key: "*string",
  is_computedly_witnessed: "*bool",
  computed_witness_execution_key: "*string",
  step_executor_agent: "*string",
  was_scored_after_attestation: "*bool",
  attestation_instant_for_run: "*string",
  post_attestation_score_execution_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the RequirementSatisfactions row.
 *  Formula: ={{Requirement}} & " / " & {{SatisfactionLevel}} */
export function calcRequirementSatisfactionsName(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.requirement)), vS(" / "), erbTextOr(vStr(tc.satisfaction_level))));
}

/** Computes the IsFullySatisfied calculated field.
 *  TRUE only when the requirement is recorded as fully Satisfied. PartiallySatisfied, Unsatisfied, Waived, and blank are all FALSE.
 *  Formula: ={{SatisfactionLevel}} = "Satisfied" */
export function calcRequirementSatisfactionsIsFullySatisfied(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.satisfaction_level)), vS("Satisfied")));
}

/** Computes the IsBlockingAndUnmet calculated field.
 *  TRUE when a blocking requirement is recorded at anything less than fully Satisfied. This is the control-failure witness at the requirement grain.
 *  Formula: =AND({{RequirementIsBlocking}}, NOT({{IsFullySatisfied}})) */
export function calcRequirementSatisfactionsIsBlockingAndUnmet(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.requirement_is_blocking)), erbBool3(erbNot(erbBool3(vBool(tc.is_fully_satisfied))))));
}

/** Computes the BlockingUnmetStepKey calculated field.
 *  Echoes the parent StepExecution id only when this row is a blocking-unmet violation; empty string otherwise.
 *  Formula: =IF({{IsBlockingAndUnmet}}, {{StepExecution}}, "") */
export function calcRequirementSatisfactionsBlockingUnmetStepKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_blocking_and_unmet)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the BlockingSatisfactionStepKey calculated field.
 *  Echoes the parent StepExecution id when this satisfaction row concerns a blocking requirement.
 *  Formula: =IF({{RequirementIsBlocking}}, {{StepExecution}}, "") */
export function calcRequirementSatisfactionsBlockingSatisfactionStepKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.requirement_is_blocking)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the NegativeOutcomeRequirementKey calculated field.
 *  Echoes the Requirement id only when this evaluation came out at less than fully Satisfied.
 *  Formula: =IF(NOT({{IsFullySatisfied}}), {{Requirement}}, "") */
export function calcRequirementSatisfactionsNegativeOutcomeRequirementKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(vBool(tc.is_fully_satisfied)))), () => vStr(tc.requirement), () => vS("")));
}

/** Computes the NonHumanEvaluatedHumanControl calculated field.
 *  TRUE when a blocking requirement was evaluated by a non-human agent.
 *  Formula: =AND({{RequirementIsBlocking}}, {{EvaluatorAgentKind}} <> "Human") */
export function calcRequirementSatisfactionsNonHumanEvaluatedHumanControl(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.requirement_is_blocking)), erbBool3(erbNe(vStr(tc.evaluator_agent_kind), vS("Human")))));
}

/** Computes the IsAssertedOnly calculated field.
 *  TRUE when a blocking requirement is recorded as Satisfied purely on human assertion, with no computed predicate behind it.
 *  Formula: =AND({{RequirementIsBlocking}}, {{IsFullySatisfied}}, NOT({{RequirementHasComputedWitness}})) */
export function calcRequirementSatisfactionsIsAssertedOnly(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.requirement_is_blocking)), erbBool3(vBool(tc.is_fully_satisfied)), erbBool3(erbNot(erbBool3(vBool(tc.requirement_has_computed_witness))))));
}

/** Computes the AssertedOnlyExecutionKey calculated field.
 *  Echoes the grandparent procedure execution id for assertion-only satisfactions.
 *  Formula: =IF({{IsAssertedOnly}}, {{ParentProcedureExecution}}, "") */
export function calcRequirementSatisfactionsAssertedOnlyExecutionKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_asserted_only)), () => vStr(tc.parent_procedure_execution), () => vS("")));
}

/** Computes the StepExecutionWhenScored calculated field.
 *  Echoes the step-execution id when a satisfaction level was actually recorded, blank otherwise.
 *  Formula: =IF({{SatisfactionLevel}} <> "", {{StepExecution}}, "") */
export function calcRequirementSatisfactionsStepExecutionWhenScored(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbIsNotBlank(vStr(tc.satisfaction_level))), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the IsHumanEvaluated calculated field.
 *  TRUE when a human evaluated this requirement satisfaction.
 *  Formula: ={{EvaluatorAgentKind}} = "Human" */
export function calcRequirementSatisfactionsIsHumanEvaluated(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.evaluator_agent_kind), vS("Human")));
}

/** Computes the IsInvalidApproval calculated field.
 *  TRUE when an approval-type requirement is not fully satisfied, or was not evaluated by a human.
 *  Formula: =AND({{RequirementIsApprovalType}} = "Approval", OR(NOT({{IsFullySatisfied}}), NOT({{IsHumanEvaluated}}))) */
export function calcRequirementSatisfactionsIsInvalidApproval(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(vStr(tc.requirement_is_approval_type), vS("Approval"))), erbBool3(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.is_fully_satisfied)))), erbBool3(erbNot(erbBool3(vBool(tc.is_human_evaluated))))))));
}

/** Computes the RunWhenInvalidApproval calculated field.
 *  Echoes the run id when this is an invalid approval, blank otherwise.
 *  Formula: =IF({{IsInvalidApproval}}, {{ProcedureExecutionOfSatisfaction}}, "") */
export function calcRequirementSatisfactionsRunWhenInvalidApproval(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_invalid_approval)), () => vStr(tc.procedure_execution_of_satisfaction), () => vS("")));
}

/** Computes the IsClearanceByUnfalsifiedControl calculated field.
 *  TRUE when this record cleared a step against a blocking control that has never produced a negative outcome.
 *  Formula: =AND({{IsFullySatisfied}}, {{RequirementIsBlocking}}, {{RequirementIsUnfalsified}}) */
export function calcRequirementSatisfactionsIsClearanceByUnfalsifiedControl(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_fully_satisfied)), erbBool3(vBool(tc.requirement_is_blocking)), erbBool3(vBool(tc.requirement_is_unfalsified))));
}

/** Computes the UnfalsifiedClearanceStepKey calculated field.
 *  Echoes the step execution id when this clearance came from an unfalsified control.
 *  Formula: =IF({{IsClearanceByUnfalsifiedControl}}, {{StepExecution}}, "") */
export function calcRequirementSatisfactionsUnfalsifiedClearanceStepKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_clearance_by_unfalsified_control)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the EvaluatorIsStepExecutor calculated field.
 *  TRUE when the agent who scored this control is the same agent who performed the step being scored.
 *  Formula: ={{EvaluatedByAgent}} = {{ScoredStepExecutorAgent}} */
export function calcRequirementSatisfactionsEvaluatorIsStepExecutor(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.evaluated_by_agent)), vStr(tc.scored_step_executor_agent)));
}

/** Computes the EvaluatorOwnsTheRun calculated field.
 *  TRUE when the agent scoring this control is the agent accountable for the execution it belongs to.
 *  Formula: ={{EvaluatedByAgent}} = {{RunOwnerAgent}} */
export function calcRequirementSatisfactionsEvaluatorOwnsTheRun(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.evaluated_by_agent)), vStr(tc.run_owner_agent)));
}

/** Computes the IsInterestedPartyAssertion calculated field.
 *  TRUE when a blocking control's only evidence is an assertion made by someone with an interest in the outcome.
 *  Formula: =AND({{IsAssertedOnly}}, OR({{EvaluatorIsStepExecutor}}, {{EvaluatorOwnsTheRun}})) */
export function calcRequirementSatisfactionsIsInterestedPartyAssertion(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_asserted_only)), erbBool3(erbOr(erbBool3(vBool(tc.evaluator_is_step_executor)), erbBool3(vBool(tc.evaluator_owns_the_run))))));
}

/** Computes the HasWrittenEvidence calculated field.
 *  TRUE when this satisfaction record carries any evidence text at all.
 *  Formula: ={{Evidence}} <> "" */
export function calcRequirementSatisfactionsHasWrittenEvidence(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.evidence)));
}

/** Computes the IsBareAssertion calculated field.
 *  TRUE when a blocking control was cleared with no computed witness and no written justification.
 *  Formula: =AND({{IsAssertedOnly}}, NOT({{HasWrittenEvidence}})) */
export function calcRequirementSatisfactionsIsBareAssertion(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_asserted_only)), erbBool3(erbNot(erbBool3(vBool(tc.has_written_evidence))))));
}

/** Computes the InterestedAssertionExecutionKey calculated field.
 *  Echoes the parent execution id when this record is an interested-party assertion.
 *  Formula: =IF({{IsInterestedPartyAssertion}}, {{ParentProcedureExecution}}, "") */
export function calcRequirementSatisfactionsInterestedAssertionExecutionKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_interested_party_assertion)), () => vStr(tc.parent_procedure_execution), () => vS("")));
}

/** Computes the IsComputedlyWitnessed calculated field.
 *  TRUE when this record scores a blocking control that has a computed witness behind it.
 *  Formula: =AND({{RequirementIsBlocking}}, {{RequirementHasComputedWitness}}) */
export function calcRequirementSatisfactionsIsComputedlyWitnessed(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.requirement_is_blocking)), erbBool3(vBool(tc.requirement_has_computed_witness))));
}

/** Computes the ComputedWitnessExecutionKey calculated field.
 *  Echoes the parent execution id when this control was computationally witnessed.
 *  Formula: =IF({{IsComputedlyWitnessed}}, {{ParentProcedureExecution}}, "") */
export function calcRequirementSatisfactionsComputedWitnessExecutionKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_computedly_witnessed)), () => vStr(tc.parent_procedure_execution), () => vS("")));
}

/** Computes the WasScoredAfterAttestation calculated field.
 *  TRUE when this control was scored after the run it belongs to had already been attested.
 *  Formula: =DATETIME_DIFF({{EvaluatedAt}}, {{AttestationInstantForRun}}, "minutes") > 0 */
export function calcRequirementSatisfactionsWasScoredAfterAttestation(tc: RequirementSatisfactionsRow): boolean | null {
  return toBoolPtr(erbCmp(erbDatetimeDiff(vStr(tc.evaluated_at), vStr(tc.attestation_instant_for_run), vS("minutes")), ">", vI(0)));
}

/** Computes the PostAttestationScoreExecutionKey calculated field.
 *  Echoes the parent execution id when this control was scored after signature.
 *  Formula: =IF({{WasScoredAfterAttestation}}, {{ParentProcedureExecution}}, "") */
export function calcRequirementSatisfactionsPostAttestationScoreExecutionKey(tc: RequirementSatisfactionsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_scored_after_attestation)), () => vStr(tc.parent_procedure_execution), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRequirementSatisfactions(tc: RequirementSatisfactionsRow): RequirementSatisfactionsRow {
  // Level 1
  calcGuard(tc, requirementSatisfactionsFieldTypes, "name", () => { tc.name = calcRequirementSatisfactionsName(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_fully_satisfied", () => { tc.is_fully_satisfied = calcRequirementSatisfactionsIsFullySatisfied(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "blocking_satisfaction_step_key", () => { tc.blocking_satisfaction_step_key = calcRequirementSatisfactionsBlockingSatisfactionStepKey(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "non_human_evaluated_human_control", () => { tc.non_human_evaluated_human_control = calcRequirementSatisfactionsNonHumanEvaluatedHumanControl(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "step_execution_when_scored", () => { tc.step_execution_when_scored = calcRequirementSatisfactionsStepExecutionWhenScored(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_human_evaluated", () => { tc.is_human_evaluated = calcRequirementSatisfactionsIsHumanEvaluated(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "evaluator_is_step_executor", () => { tc.evaluator_is_step_executor = calcRequirementSatisfactionsEvaluatorIsStepExecutor(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "evaluator_owns_the_run", () => { tc.evaluator_owns_the_run = calcRequirementSatisfactionsEvaluatorOwnsTheRun(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "has_written_evidence", () => { tc.has_written_evidence = calcRequirementSatisfactionsHasWrittenEvidence(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_computedly_witnessed", () => { tc.is_computedly_witnessed = calcRequirementSatisfactionsIsComputedlyWitnessed(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "was_scored_after_attestation", () => { tc.was_scored_after_attestation = calcRequirementSatisfactionsWasScoredAfterAttestation(tc); });
  // Level 2
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_blocking_and_unmet", () => { tc.is_blocking_and_unmet = calcRequirementSatisfactionsIsBlockingAndUnmet(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "negative_outcome_requirement_key", () => { tc.negative_outcome_requirement_key = calcRequirementSatisfactionsNegativeOutcomeRequirementKey(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_asserted_only", () => { tc.is_asserted_only = calcRequirementSatisfactionsIsAssertedOnly(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_invalid_approval", () => { tc.is_invalid_approval = calcRequirementSatisfactionsIsInvalidApproval(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_clearance_by_unfalsified_control", () => { tc.is_clearance_by_unfalsified_control = calcRequirementSatisfactionsIsClearanceByUnfalsifiedControl(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "computed_witness_execution_key", () => { tc.computed_witness_execution_key = calcRequirementSatisfactionsComputedWitnessExecutionKey(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "post_attestation_score_execution_key", () => { tc.post_attestation_score_execution_key = calcRequirementSatisfactionsPostAttestationScoreExecutionKey(tc); });
  // Level 3
  calcGuard(tc, requirementSatisfactionsFieldTypes, "blocking_unmet_step_key", () => { tc.blocking_unmet_step_key = calcRequirementSatisfactionsBlockingUnmetStepKey(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "asserted_only_execution_key", () => { tc.asserted_only_execution_key = calcRequirementSatisfactionsAssertedOnlyExecutionKey(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "run_when_invalid_approval", () => { tc.run_when_invalid_approval = calcRequirementSatisfactionsRunWhenInvalidApproval(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "unfalsified_clearance_step_key", () => { tc.unfalsified_clearance_step_key = calcRequirementSatisfactionsUnfalsifiedClearanceStepKey(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_interested_party_assertion", () => { tc.is_interested_party_assertion = calcRequirementSatisfactionsIsInterestedPartyAssertion(tc); });
  calcGuard(tc, requirementSatisfactionsFieldTypes, "is_bare_assertion", () => { tc.is_bare_assertion = calcRequirementSatisfactionsIsBareAssertion(tc); });
  // Level 4
  calcGuard(tc, requirementSatisfactionsFieldTypes, "interested_assertion_execution_key", () => { tc.interested_assertion_execution_key = calcRequirementSatisfactionsInterestedAssertionExecutionKey(tc); });
  return tc;
}

/** Reads RequirementSatisfactions rows from a JSON array file. */
export function loadRequirementSatisfactionsRows(file: string): RequirementSatisfactionsRow[] {
  return loadRows(file, { fields: requirementSatisfactionsFieldTypes }) as unknown as RequirementSatisfactionsRow[];
}

// =============================================================================
// ERRORS TABLE
// Reusable error definitions encountered during execution. Maps to pko:Error, errorCode, and errorCause.
// =============================================================================

/** A row in the Errors table. */
export interface ErrorsRow {
  /** Stored logical identifier for one Errors row. */
  error_id: string;
  /** Human-readable calculated display alias for the Errors row. */
  name: string | null;
  /** Error label. */
  label: string | null;
  /** Error code; maps to pko:errorCode. */
  error_code: string | null;
  /** Known or suspected cause; maps to pko:errorCause. */
  error_cause: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const errorsFieldTypes: Record<string, FieldType> = {
  error_id: "string",
  name: "*string",
  label: "*string",
  error_code: "*string",
  error_cause: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Errors row.
 *  Formula: ={{ErrorCode}} & " - " & {{Label}} */
export function calcErrorsName(tc: ErrorsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.error_code)), vS(" - "), erbTextOr(vStr(tc.label))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeErrors(tc: ErrorsRow): ErrorsRow {
  // Level 1
  calcGuard(tc, errorsFieldTypes, "name", () => { tc.name = calcErrorsName(tc); });
  return tc;
}

/** Reads Errors rows from a JSON array file. */
export function loadErrorsRows(file: string): ErrorsRow[] {
  return loadRows(file, { fields: errorsFieldTypes }) as unknown as ErrorsRow[];
}

// =============================================================================
// ISSUEOCCURRENCES TABLE
// Concrete issue events during execution. Maps to pko:IssueOccurrence, hasEncounteredError, wasEncounteredBy, issueCause, and issueSolution.
// =============================================================================

/** A row in the IssueOccurrences table. */
export interface IssueOccurrencesRow {
  /** Stored logical identifier for one IssueOccurrences row. */
  issue_occurrence_id: string;
  /** Human-readable calculated display alias for the IssueOccurrences row. */
  name: string | null;
  /** Step execution during which the issue occurred. */
  step_execution: string | null;
  /** Error encountered. */
  error: string | null;
  /** Agent that encountered or reported the issue. */
  encountered_by_agent: string | null;
  /** Occurrence time. */
  occurred_at: string | null;
  /** Execution-specific cause; maps to pko:issueCause. */
  issue_cause: string | null;
  /** Applied solution; maps to pko:issueSolution. */
  issue_solution: string | null;
  /** Open, Monitoring, Resolved, or Closed. */
  status: string | null;
  /** TRUE when this issue occurrence has not been closed out. */
  is_unresolved: boolean | null;
  /** Echoes the step-execution id when the issue is unresolved, blank otherwise. */
  step_execution_when_unresolved: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const issueOccurrencesFieldTypes: Record<string, FieldType> = {
  issue_occurrence_id: "string",
  name: "*string",
  step_execution: "*string",
  error: "*string",
  encountered_by_agent: "*string",
  occurred_at: "*string",
  issue_cause: "*string",
  issue_solution: "*string",
  status: "*string",
  is_unresolved: "*bool",
  step_execution_when_unresolved: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the IssueOccurrences row.
 *  Formula: ={{Error}} & " @ " & {{OccurredAt}} */
export function calcIssueOccurrencesName(tc: IssueOccurrencesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.error)), vS(" @ "), erbTimestamptzText(vStr(tc.occurred_at))));
}

/** Computes the IsUnresolved calculated field.
 *  TRUE when this issue occurrence has not been closed out.
 *  Formula: =OR({{Status}} = "Open", {{Status}} = "Investigating", {{Status}} = "Monitoring") */
export function calcIssueOccurrencesIsUnresolved(tc: IssueOccurrencesRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Open"))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Investigating"))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Monitoring")))));
}

/** Computes the StepExecutionWhenUnresolved calculated field.
 *  Echoes the step-execution id when the issue is unresolved, blank otherwise.
 *  Formula: =IF({{IsUnresolved}}, {{StepExecution}}, "") */
export function calcIssueOccurrencesStepExecutionWhenUnresolved(tc: IssueOccurrencesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unresolved)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeIssueOccurrences(tc: IssueOccurrencesRow): IssueOccurrencesRow {
  // Level 1
  calcGuard(tc, issueOccurrencesFieldTypes, "name", () => { tc.name = calcIssueOccurrencesName(tc); });
  calcGuard(tc, issueOccurrencesFieldTypes, "is_unresolved", () => { tc.is_unresolved = calcIssueOccurrencesIsUnresolved(tc); });
  // Level 2
  calcGuard(tc, issueOccurrencesFieldTypes, "step_execution_when_unresolved", () => { tc.step_execution_when_unresolved = calcIssueOccurrencesStepExecutionWhenUnresolved(tc); });
  return tc;
}

/** Reads IssueOccurrences rows from a JSON array file. */
export function loadIssueOccurrencesRows(file: string): IssueOccurrencesRow[] {
  return loadRows(file, { fields: issueOccurrencesFieldTypes }) as unknown as IssueOccurrencesRow[];
}

// =============================================================================
// USERQUESTIONS TABLE
// Questions asked by agents during execution. Maps to pko:UserQuestionOccurrence, questionByUser, wasAskedBy, and isQuestionAddressedBy.
// =============================================================================

/** A row in the UserQuestions table. */
export interface UserQuestionsRow {
  /** Stored logical identifier for one UserQuestions row. */
  user_question_id: string;
  /** Human-readable calculated display alias for the UserQuestions row. */
  name: string | null;
  /** Execution context for the question. */
  step_execution: string | null;
  /** Agent asking the question. */
  asked_by_agent: string | null;
  /** Question time. */
  asked_at: string | null;
  /** Question text; maps to pko:questionByUser. */
  question_text: string | null;
  /** FAQ used to resolve the question. */
  resolved_by_faq: string | null;
  /** Resource that addressed the question. */
  addressed_by_resource: string | null;
  /** Open, Answered, Escalated, or Closed. */
  status: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const userQuestionsFieldTypes: Record<string, FieldType> = {
  user_question_id: "string",
  name: "*string",
  step_execution: "*string",
  asked_by_agent: "*string",
  asked_at: "*string",
  question_text: "*string",
  resolved_by_faq: "*string",
  addressed_by_resource: "*string",
  status: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the UserQuestions row.
 *  Formula: =LEFT({{QuestionText}}, 70) */
export function calcUserQuestionsName(tc: UserQuestionsRow): string | null {
  return toStringPtr(erbLeft(vStr(tc.question_text), vI(70)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeUserQuestions(tc: UserQuestionsRow): UserQuestionsRow {
  // Level 1
  calcGuard(tc, userQuestionsFieldTypes, "name", () => { tc.name = calcUserQuestionsName(tc); });
  return tc;
}

/** Reads UserQuestions rows from a JSON array file. */
export function loadUserQuestionsRows(file: string): UserQuestionsRow[] {
  return loadRows(file, { fields: userQuestionsFieldTypes }) as unknown as UserQuestionsRow[];
}

// =============================================================================
// USERFEEDBACK TABLE
// Feedback supplied by users about a procedure or execution. Maps to pko:UserFeedbackOccurrence, feedbackOnProcedureExecution, and wasProvidedBy.
// =============================================================================

/** A row in the UserFeedback table. */
export interface UserFeedbackRow {
  /** Stored logical identifier for one UserFeedback row. */
  user_feedback_id: string;
  /** Human-readable calculated display alias for the UserFeedback row. */
  name: string | null;
  /** Execution receiving feedback. */
  procedure_execution: string | null;
  /** Agent providing feedback. */
  provided_by_agent: string | null;
  /** Feedback time. */
  provided_at: string | null;
  /** Feedback statement. */
  feedback_text: string | null;
  /** Accepted, Rejected, UnderReview, or Deferred. */
  disposition: string | null;
  /** Related change request identifier. */
  change_request_key: string | null;
  /** Exact PKO class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const userFeedbackFieldTypes: Record<string, FieldType> = {
  user_feedback_id: "string",
  name: "*string",
  procedure_execution: "*string",
  provided_by_agent: "*string",
  provided_at: "*string",
  feedback_text: "*string",
  disposition: "*string",
  change_request_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the UserFeedback row.
 *  Formula: ={{Disposition}} & ": " & LEFT({{FeedbackText}}, 60) */
export function calcUserFeedbackName(tc: UserFeedbackRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.disposition)), vS(": "), erbTextNotNull(erbLeft(vStr(tc.feedback_text), vI(60)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeUserFeedback(tc: UserFeedbackRow): UserFeedbackRow {
  // Level 1
  calcGuard(tc, userFeedbackFieldTypes, "name", () => { tc.name = calcUserFeedbackName(tc); });
  return tc;
}

/** Reads UserFeedback rows from a JSON array file. */
export function loadUserFeedbackRows(file: string): UserFeedbackRow[] {
  return loadRows(file, { fields: userFeedbackFieldTypes }) as unknown as UserFeedbackRow[];
}

// =============================================================================
// STEWARDSHIPASSIGNMENTS TABLE
// Separates ongoing stewardship from authority to approve semantic commitments. Explicit ERB-PKO governance extension.
// =============================================================================

/** A row in the StewardshipAssignments table. */
export interface StewardshipAssignmentsRow {
  /** Stored logical identifier for one StewardshipAssignments row. */
  stewardship_assignment_id: string;
  /** Human-readable calculated display alias for the StewardshipAssignments row. */
  name: string | null;
  /** Governed procedure version. */
  procedure_version: string | null;
  /** Role responsible for health, review, and maintenance. */
  steward_role: string | null;
  /** Role authorized to approve semantic changes. */
  authority_role: string | null;
  /** Start of stewardship interval. */
  valid_from: string | null;
  /** End of stewardship interval. */
  valid_to: string | null;
  /** Required review cadence. */
  review_cadence_days: number | null;
  /** How many review events have ever been recorded for the procedure version this assignment stewards. */
  count_of_review_events: number | null;
  /** TRUE if at least one review event exists for the stewarded procedure version. */
  has_ever_been_reviewed: boolean | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** TRUE when this stewardship assignment is in force right now. */
  is_current_assignment: boolean | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const stewardshipAssignmentsFieldTypes: Record<string, FieldType> = {
  stewardship_assignment_id: "string",
  name: "*string",
  procedure_version: "*string",
  steward_role: "*string",
  authority_role: "*string",
  valid_from: "*string",
  valid_to: "*string",
  review_cadence_days: "*int",
  count_of_review_events: "*int",
  has_ever_been_reviewed: "*bool",
  evaluation_context: "*string",
  as_of_instant: "*string",
  is_current_assignment: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the StewardshipAssignments row.
 *  Formula: ={{ProcedureVersion}} & " / steward=" & {{StewardRole}} */
export function calcStewardshipAssignmentsName(tc: StewardshipAssignmentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_version)), vS(" / steward="), erbTextOr(vStr(tc.steward_role))));
}

/** Computes the HasEverBeenReviewed calculated field.
 *  TRUE if at least one review event exists for the stewarded procedure version.
 *  Formula: ={{CountOfReviewEvents}} > 0 */
export function calcStewardshipAssignmentsHasEverBeenReviewed(tc: StewardshipAssignmentsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_review_events), ">", vI(0)));
}

/** Computes the IsCurrentAssignment calculated field.
 *  TRUE when this stewardship assignment is in force right now.
 *  Formula: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})) */
export function calcStewardshipAssignmentsIsCurrentAssignment(tc: StewardshipAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<=", vStr(tc.as_of_instant))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant)))))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStewardshipAssignments(tc: StewardshipAssignmentsRow): StewardshipAssignmentsRow {
  // Level 1
  calcGuard(tc, stewardshipAssignmentsFieldTypes, "name", () => { tc.name = calcStewardshipAssignmentsName(tc); });
  calcGuard(tc, stewardshipAssignmentsFieldTypes, "has_ever_been_reviewed", () => { tc.has_ever_been_reviewed = calcStewardshipAssignmentsHasEverBeenReviewed(tc); });
  calcGuard(tc, stewardshipAssignmentsFieldTypes, "is_current_assignment", () => { tc.is_current_assignment = calcStewardshipAssignmentsIsCurrentAssignment(tc); });
  return tc;
}

/** Reads StewardshipAssignments rows from a JSON array file. */
export function loadStewardshipAssignmentsRows(file: string): StewardshipAssignmentsRow[] {
  return loadRows(file, { fields: stewardshipAssignmentsFieldTypes }) as unknown as StewardshipAssignmentsRow[];
}

// =============================================================================
// CHANGEREQUESTS TABLE
// Governed requests for semantic or operational change, anchored to a procedure version and authority. Explicit ERB-PKO extension.
// =============================================================================

/** A row in the ChangeRequests table. */
export interface ChangeRequestsRow {
  /** Stored logical identifier for one ChangeRequests row. */
  change_request_id: string;
  /** Human-readable calculated display alias for the ChangeRequests row. */
  name: string | null;
  /** Procedure version affected. */
  procedure_version: string | null;
  /** Change title. */
  title: string | null;
  /** Defect, Enhancement, NewRequirement, DataOperation, or BreakingChange. */
  change_kind: string | null;
  /** Draft, UnderReview, Approved, Rejected, Implemented, or Closed. */
  status: string | null;
  /** Agent requesting the change. */
  requested_by_agent: string | null;
  /** Role authorized to approve the change. */
  authority_role: string | null;
  /** Request time. */
  requested_at: string | null;
  /** Decision time. */
  decided_at: string | null;
  /** Expected effect on commitments, data, projections, and tests. */
  impact_assessment: string | null;
  /** TRUE while the change request is still outstanding. An Approved request stays open until ImplementedAt records that it actually landed — approval is a decision, not an outcome. */
  is_open: boolean | null;
  /** Echoes the ProcedureVersion id only for change requests still open. */
  open_change_version_key: string | null;
  /** TRUE when a decision timestamp has been recorded. */
  is_decided: boolean | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** Days from request to decision, or to now if still undecided. */
  days_pending: number | null;
  /** TRUE when the request is in an open status and has no decision recorded. */
  is_still_pending: boolean | null;
  /** TRUE when an undecided change request has been pending more than fourteen days. */
  is_stalled: boolean | null;
  /** The agent currently holding the authority role that owes a decision on this request. */
  authority_agent: string | null;
  /** TRUE when the agent who raised the request is also the agent who decides it. */
  requester_is_authority: boolean | null;
  /** TRUE when this request is formally before its authority and no decision has been recorded. */
  awaits_authority_decision: boolean | null;
  /** Human-readable label of the role that owes a decision. */
  authority_role_label: string | null;
  /** Whether the version this request would alter is currently executable. */
  touches_live_version: boolean | null;
  /** TRUE when a decision I owe is blocking a change to a procedure currently in production. */
  is_live_decision_backlog: boolean | null;
  /** TRUE when an undecided request against a live version is the kind that exists to close a known gap. */
  blocks_an_open_gap: boolean | null;
  /** When the approved change was actually applied to the procedure version. Null until it lands. Distinct from DecidedAt, which records only that the authority ruled. Approval and implementation are separate events; conflating them is what left IsOpen with no terminal state. */
  implemented_at: string | null;
  /** Composite-key echo: this request's procedure version when it is live decision backlog, blank otherwise. */
  backlog_version_key: string | null;
  /** A change request awaiting a decision that is mine personally to make. */
  is_my_pending_decision: boolean | null;
  /** A decision waiting on me that is holding an open gap on a live procedure. */
  is_my_blocking_backlog: boolean | null;
  /** A blocking decision that has waited on me for more than two weeks. */
  is_my_overdue_backlog: boolean | null;
  /** Whether the approved change has actually been applied. */
  is_implemented: boolean | null;
  /** A change request I have personally ruled on. */
  is_my_decided_request: boolean | null;
  /** A decision I have made that has not yet been applied — still counted against me by the loop-1 open-request measure. */
  is_my_decided_but_unlanded: boolean | null;
  /** How many days I took to rule, once ruled. Zero when undecided — read only alongside IsDecided. */
  decision_latency_days: number | null;
  /** How many days elapsed between my ruling and the change actually landing. Zero when not yet implemented. */
  implementation_latency_days: number | null;
  /** A request I decided promptly that is nevertheless still outstanding because nobody has implemented it. */
  delay_is_downstream_of_me: boolean | null;
  /** Composite-key echo: this request's procedure version when I have decided it but it has not landed, blank otherwise. */
  unlanded_version_key: string | null;
  /** A change request the authority approved but that has not yet been applied. */
  is_approved_not_implemented: boolean | null;
  /** How many days have elapsed since the authority decided this request. Zero when undecided. */
  days_since_approval: number | null;
  /** An approved change request that has sat unimplemented for more than two weeks. */
  is_stalled_implementation: boolean | null;
  /** Composite-key echo: this request's procedure version when its implementation is stalled, blank otherwise. */
  stalled_implementation_version_key: string | null;
  /** Echoes the target version id when this change request was approved. */
  approved_version_key: string | null;
  /** TRUE when this change request was decided in the affirmative. */
  is_approved_decision: boolean | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const changeRequestsFieldTypes: Record<string, FieldType> = {
  change_request_id: "string",
  name: "*string",
  procedure_version: "*string",
  title: "*string",
  change_kind: "*string",
  status: "*string",
  requested_by_agent: "*string",
  authority_role: "*string",
  requested_at: "*string",
  decided_at: "*string",
  impact_assessment: "*string",
  is_open: "*bool",
  open_change_version_key: "*string",
  is_decided: "*bool",
  evaluation_context: "*string",
  as_of_instant: "*string",
  days_pending: "*int",
  is_still_pending: "*bool",
  is_stalled: "*bool",
  authority_agent: "*string",
  requester_is_authority: "*bool",
  awaits_authority_decision: "*bool",
  authority_role_label: "*string",
  touches_live_version: "*bool",
  is_live_decision_backlog: "*bool",
  blocks_an_open_gap: "*bool",
  implemented_at: "*string",
  backlog_version_key: "*string",
  is_my_pending_decision: "*bool",
  is_my_blocking_backlog: "*bool",
  is_my_overdue_backlog: "*bool",
  is_implemented: "*bool",
  is_my_decided_request: "*bool",
  is_my_decided_but_unlanded: "*bool",
  decision_latency_days: "*int",
  implementation_latency_days: "*int",
  delay_is_downstream_of_me: "*bool",
  unlanded_version_key: "*string",
  is_approved_not_implemented: "*bool",
  days_since_approval: "*int",
  is_stalled_implementation: "*bool",
  stalled_implementation_version_key: "*string",
  approved_version_key: "*string",
  is_approved_decision: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ChangeRequests row.
 *  Formula: ={{Title}} */
export function calcChangeRequestsName(tc: ChangeRequestsRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes the IsOpen calculated field.
 *  TRUE while the change request is still outstanding. An Approved request stays open until ImplementedAt records that it actually landed — approval is a decision, not an outcome.
 *  Formula: =AND(OR({{Status}} = "Draft", {{Status}} = "UnderReview", {{Status}} = "Approved"), {{ImplementedAt}} = "") */
export function calcChangeRequestsIsOpen(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Draft"))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("UnderReview"))), erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))))), erbBool3(erbIsBlank(vStr(tc.implemented_at)))));
}

/** Computes the OpenChangeVersionKey calculated field.
 *  Echoes the ProcedureVersion id only for change requests still open.
 *  Formula: =IF({{IsOpen}}, {{ProcedureVersion}}, "") */
export function calcChangeRequestsOpenChangeVersionKey(tc: ChangeRequestsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_open)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsDecided calculated field.
 *  TRUE when a decision timestamp has been recorded.
 *  Formula: ={{DecidedAt}} <> "" */
export function calcChangeRequestsIsDecided(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.decided_at)));
}

/** Computes the DaysPending calculated field.
 *  Days from request to decision, or to now if still undecided.
 *  Formula: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{RequestedAt}}, "days")) */
export function calcChangeRequestsDaysPending(tc: ChangeRequestsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_decided)), () => erbDatetimeDiff(vStr(tc.decided_at), vStr(tc.requested_at), vS("days")), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.requested_at), vS("days")))));
}

/** Computes the IsStillPending calculated field.
 *  TRUE when the request is in an open status and has no decision recorded.
 *  Formula: =AND({{IsOpen}}, NOT({{IsDecided}})) */
export function calcChangeRequestsIsStillPending(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_open)), erbBool3(erbNot(erbBool3(vBool(tc.is_decided))))));
}

/** Computes the IsStalled calculated field.
 *  TRUE when an undecided change request has been pending more than fourteen days.
 *  Formula: =AND({{IsStillPending}}, {{DaysPending}} > 14) */
export function calcChangeRequestsIsStalled(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_still_pending)), erbBool3(erbCmp(vInt(tc.days_pending), ">", vI(14)))));
}

/** Computes the RequesterIsAuthority calculated field.
 *  TRUE when the agent who raised the request is also the agent who decides it.
 *  Formula: ={{RequestedByAgent}} = {{AuthorityAgent}} */
export function calcChangeRequestsRequesterIsAuthority(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.requested_by_agent)), vStr(tc.authority_agent)));
}

/** Computes the AwaitsAuthorityDecision calculated field.
 *  TRUE when this request is formally before its authority and no decision has been recorded.
 *  Formula: =AND({{Status}} = "UnderReview", NOT({{IsDecided}})) */
export function calcChangeRequestsAwaitsAuthorityDecision(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("UnderReview"))), erbBool3(erbNot(erbBool3(vBool(tc.is_decided))))));
}

/** Computes the IsLiveDecisionBacklog calculated field.
 *  TRUE when a decision I owe is blocking a change to a procedure currently in production.
 *  Formula: =AND({{AwaitsAuthorityDecision}}, {{TouchesLiveVersion}}) */
export function calcChangeRequestsIsLiveDecisionBacklog(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.awaits_authority_decision)), erbBool3(vBool(tc.touches_live_version))));
}

/** Computes the BlocksAnOpenGap calculated field.
 *  TRUE when an undecided request against a live version is the kind that exists to close a known gap.
 *  Formula: =AND({{IsLiveDecisionBacklog}}, {{ChangeKind}} = "Enhancement") */
export function calcChangeRequestsBlocksAnOpenGap(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_live_decision_backlog)), erbBool3(erbEq(erbNullif(vStr(tc.change_kind)), vS("Enhancement")))));
}

/** Computes the BacklogVersionKey calculated field.
 *  Composite-key echo: this request's procedure version when it is live decision backlog, blank otherwise.
 *  Formula: =IF({{IsLiveDecisionBacklog}}, {{ProcedureVersion}}, "") */
export function calcChangeRequestsBacklogVersionKey(tc: ChangeRequestsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_live_decision_backlog)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsMyPendingDecision calculated field.
 *  A change request awaiting a decision that is mine personally to make.
 *  Formula: =AND({{AuthorityRole}} = "hr-policy-owner", {{AwaitsAuthorityDecision}}) */
export function calcChangeRequestsIsMyPendingDecision(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.authority_role)), vS("hr-policy-owner"))), erbBool3(vBool(tc.awaits_authority_decision))));
}

/** Computes the IsMyBlockingBacklog calculated field.
 *  A decision waiting on me that is holding an open gap on a live procedure.
 *  Formula: =AND({{IsMyPendingDecision}}, {{BlocksAnOpenGap}}) */
export function calcChangeRequestsIsMyBlockingBacklog(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_my_pending_decision)), erbBool3(vBool(tc.blocks_an_open_gap))));
}

/** Computes the IsMyOverdueBacklog calculated field.
 *  A blocking decision that has waited on me for more than two weeks.
 *  Formula: =AND({{IsMyBlockingBacklog}}, {{DaysPending}} > 14) */
export function calcChangeRequestsIsMyOverdueBacklog(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_my_blocking_backlog)), erbBool3(erbCmp(vInt(tc.days_pending), ">", vI(14)))));
}

/** Computes the IsImplemented calculated field.
 *  Whether the approved change has actually been applied.
 *  Formula: ={{ImplementedAt}} <> "" */
export function calcChangeRequestsIsImplemented(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.implemented_at)));
}

/** Computes the IsMyDecidedRequest calculated field.
 *  A change request I have personally ruled on.
 *  Formula: =AND({{AuthorityRole}} = "hr-policy-owner", {{IsDecided}}) */
export function calcChangeRequestsIsMyDecidedRequest(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.authority_role)), vS("hr-policy-owner"))), erbBool3(vBool(tc.is_decided))));
}

/** Computes the IsMyDecidedButUnlanded calculated field.
 *  A decision I have made that has not yet been applied — still counted against me by the loop-1 open-request measure.
 *  Formula: =AND({{IsMyDecidedRequest}}, NOT({{IsImplemented}})) */
export function calcChangeRequestsIsMyDecidedButUnlanded(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_my_decided_request)), erbBool3(erbNot(erbBool3(vBool(tc.is_implemented))))));
}

/** Computes the DecisionLatencyDays calculated field.
 *  How many days I took to rule, once ruled. Zero when undecided — read only alongside IsDecided.
 *  Formula: =IF({{IsDecided}}, DATETIME_DIFF({{DecidedAt}}, {{RequestedAt}}, "days"), 0) */
export function calcChangeRequestsDecisionLatencyDays(tc: ChangeRequestsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_decided)), () => erbDatetimeDiff(vStr(tc.decided_at), vStr(tc.requested_at), vS("days")), () => vI(0))));
}

/** Computes the ImplementationLatencyDays calculated field.
 *  How many days elapsed between my ruling and the change actually landing. Zero when not yet implemented.
 *  Formula: =IF({{IsImplemented}}, DATETIME_DIFF({{ImplementedAt}}, {{DecidedAt}}, "days"), 0) */
export function calcChangeRequestsImplementationLatencyDays(tc: ChangeRequestsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_implemented)), () => erbDatetimeDiff(vStr(tc.implemented_at), vStr(tc.decided_at), vS("days")), () => vI(0))));
}

/** Computes the DelayIsDownstreamOfMe calculated field.
 *  A request I decided promptly that is nevertheless still outstanding because nobody has implemented it.
 *  Formula: =AND({{IsMyDecidedButUnlanded}}, {{DecisionLatencyDays}} <= 14) */
export function calcChangeRequestsDelayIsDownstreamOfMe(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_my_decided_but_unlanded)), erbBool3(erbCmp(vInt(tc.decision_latency_days), "<=", vI(14)))));
}

/** Computes the UnlandedVersionKey calculated field.
 *  Composite-key echo: this request's procedure version when I have decided it but it has not landed, blank otherwise.
 *  Formula: =IF({{IsMyDecidedButUnlanded}}, {{ProcedureVersion}}, "") */
export function calcChangeRequestsUnlandedVersionKey(tc: ChangeRequestsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_my_decided_but_unlanded)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsApprovedNotImplemented calculated field.
 *  A change request the authority approved but that has not yet been applied.
 *  Formula: =AND({{Status}} = "Approved", NOT({{IsImplemented}})) */
export function calcChangeRequestsIsApprovedNotImplemented(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbNot(erbBool3(vBool(tc.is_implemented))))));
}

/** Computes the DaysSinceApproval calculated field.
 *  How many days have elapsed since the authority decided this request. Zero when undecided.
 *  Formula: =IF({{IsDecided}}, DATETIME_DIFF({{AsOfInstant}}, {{DecidedAt}}, "days"), 0) */
export function calcChangeRequestsDaysSinceApproval(tc: ChangeRequestsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_decided)), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.decided_at), vS("days")), () => vI(0))));
}

/** Computes the IsStalledImplementation calculated field.
 *  An approved change request that has sat unimplemented for more than two weeks.
 *  Formula: =AND({{IsApprovedNotImplemented}}, {{DaysSinceApproval}} > 14) */
export function calcChangeRequestsIsStalledImplementation(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_approved_not_implemented)), erbBool3(erbCmp(vInt(tc.days_since_approval), ">", vI(14)))));
}

/** Computes the StalledImplementationVersionKey calculated field.
 *  Composite-key echo: this request's procedure version when its implementation is stalled, blank otherwise.
 *  Formula: =IF({{IsStalledImplementation}}, {{ProcedureVersion}}, "") */
export function calcChangeRequestsStalledImplementationVersionKey(tc: ChangeRequestsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_stalled_implementation)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the ApprovedVersionKey calculated field.
 *  Echoes the target version id when this change request was approved.
 *  Formula: =IF({{IsApprovedDecision}}, {{ProcedureVersion}}, "") */
export function calcChangeRequestsApprovedVersionKey(tc: ChangeRequestsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_approved_decision)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the IsApprovedDecision calculated field.
 *  TRUE when this change request was decided in the affirmative.
 *  Formula: ={{Status}} = "Approved" */
export function calcChangeRequestsIsApprovedDecision(tc: ChangeRequestsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.status)), vS("Approved")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeChangeRequests(tc: ChangeRequestsRow): ChangeRequestsRow {
  // Level 1
  calcGuard(tc, changeRequestsFieldTypes, "name", () => { tc.name = calcChangeRequestsName(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_open", () => { tc.is_open = calcChangeRequestsIsOpen(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_decided", () => { tc.is_decided = calcChangeRequestsIsDecided(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "requester_is_authority", () => { tc.requester_is_authority = calcChangeRequestsRequesterIsAuthority(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_implemented", () => { tc.is_implemented = calcChangeRequestsIsImplemented(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_approved_decision", () => { tc.is_approved_decision = calcChangeRequestsIsApprovedDecision(tc); });
  // Level 2
  calcGuard(tc, changeRequestsFieldTypes, "open_change_version_key", () => { tc.open_change_version_key = calcChangeRequestsOpenChangeVersionKey(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "days_pending", () => { tc.days_pending = calcChangeRequestsDaysPending(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_still_pending", () => { tc.is_still_pending = calcChangeRequestsIsStillPending(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "awaits_authority_decision", () => { tc.awaits_authority_decision = calcChangeRequestsAwaitsAuthorityDecision(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_my_decided_request", () => { tc.is_my_decided_request = calcChangeRequestsIsMyDecidedRequest(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "decision_latency_days", () => { tc.decision_latency_days = calcChangeRequestsDecisionLatencyDays(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "implementation_latency_days", () => { tc.implementation_latency_days = calcChangeRequestsImplementationLatencyDays(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_approved_not_implemented", () => { tc.is_approved_not_implemented = calcChangeRequestsIsApprovedNotImplemented(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "days_since_approval", () => { tc.days_since_approval = calcChangeRequestsDaysSinceApproval(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "approved_version_key", () => { tc.approved_version_key = calcChangeRequestsApprovedVersionKey(tc); });
  // Level 3
  calcGuard(tc, changeRequestsFieldTypes, "is_stalled", () => { tc.is_stalled = calcChangeRequestsIsStalled(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_live_decision_backlog", () => { tc.is_live_decision_backlog = calcChangeRequestsIsLiveDecisionBacklog(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_my_pending_decision", () => { tc.is_my_pending_decision = calcChangeRequestsIsMyPendingDecision(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_my_decided_but_unlanded", () => { tc.is_my_decided_but_unlanded = calcChangeRequestsIsMyDecidedButUnlanded(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "is_stalled_implementation", () => { tc.is_stalled_implementation = calcChangeRequestsIsStalledImplementation(tc); });
  // Level 4
  calcGuard(tc, changeRequestsFieldTypes, "blocks_an_open_gap", () => { tc.blocks_an_open_gap = calcChangeRequestsBlocksAnOpenGap(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "backlog_version_key", () => { tc.backlog_version_key = calcChangeRequestsBacklogVersionKey(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "delay_is_downstream_of_me", () => { tc.delay_is_downstream_of_me = calcChangeRequestsDelayIsDownstreamOfMe(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "unlanded_version_key", () => { tc.unlanded_version_key = calcChangeRequestsUnlandedVersionKey(tc); });
  calcGuard(tc, changeRequestsFieldTypes, "stalled_implementation_version_key", () => { tc.stalled_implementation_version_key = calcChangeRequestsStalledImplementationVersionKey(tc); });
  // Level 5
  calcGuard(tc, changeRequestsFieldTypes, "is_my_blocking_backlog", () => { tc.is_my_blocking_backlog = calcChangeRequestsIsMyBlockingBacklog(tc); });
  // Level 6
  calcGuard(tc, changeRequestsFieldTypes, "is_my_overdue_backlog", () => { tc.is_my_overdue_backlog = calcChangeRequestsIsMyOverdueBacklog(tc); });
  return tc;
}

/** Reads ChangeRequests rows from a JSON array file. */
export function loadChangeRequestsRows(file: string): ChangeRequestsRow[] {
  return loadRows(file, { fields: changeRequestsFieldTypes }) as unknown as ChangeRequestsRow[];
}

// =============================================================================
// REVIEWEVENTS TABLE
// Periodic governance reviews that test competency coverage, staleness, and semantic integrity. Explicit ERB-PKO extension represented as prov:Activity.
// =============================================================================

/** A row in the ReviewEvents table. */
export interface ReviewEventsRow {
  /** Stored logical identifier for one ReviewEvents row. */
  review_event_id: string;
  /** Human-readable calculated display alias for the ReviewEvents row. */
  name: string | null;
  /** Reviewed procedure version. */
  procedure_version: string | null;
  /** Review type. */
  review_kind: string | null;
  /** Review timestamp. */
  reviewed_at: string | null;
  /** Reviewing agent. */
  reviewed_by_agent: string | null;
  /** Passed, PassedWithChange, Failed, or Deferred. */
  outcome: string | null;
  /** Change request produced or considered. */
  related_change_request: string | null;
  /** Next required review. */
  next_review_due: string | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** TRUE when review is overdue. */
  is_overdue: boolean | null;
  /** Echoes the ProcedureVersion id only when this review is past its next-due date. */
  overdue_version_key: string | null;
  /** The review cadence promised for this procedure version, resolved via ProcedureVersions because INDEX/MATCH can only match a target table primary key. */
  promised_cadence_days: number | null;
  /** Elapsed days since this review actually happened. */
  days_since_reviewed: number | null;
  /** TRUE when more days have elapsed since this review than the stewardship assignment promised as a cadence. */
  exceeds_promised_cadence: boolean | null;
  /** Signed drift: positive means we are past the promised cadence by this many days; negative means we are still inside it. */
  cadence_drift_days: number | null;
  /** TRUE when the promised cadence has been blown but the hand-entered NextReviewDue still says we are fine. */
  promise_and_behavior_disagree: boolean | null;
  /** Composite-key echo: this review event's procedure version when the promised cadence has been exceeded, blank otherwise. */
  cadence_breach_version_key: string | null;
  /** Class IRI used in projection. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const reviewEventsFieldTypes: Record<string, FieldType> = {
  review_event_id: "string",
  name: "*string",
  procedure_version: "*string",
  review_kind: "*string",
  reviewed_at: "*string",
  reviewed_by_agent: "*string",
  outcome: "*string",
  related_change_request: "*string",
  next_review_due: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  is_overdue: "*bool",
  overdue_version_key: "*string",
  promised_cadence_days: "*int",
  days_since_reviewed: "*int",
  exceeds_promised_cadence: "*bool",
  cadence_drift_days: "*int",
  promise_and_behavior_disagree: "*bool",
  cadence_breach_version_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ReviewEvents row.
 *  Formula: ={{ProcedureVersion}} & " / " & {{ReviewKind}} */
export function calcReviewEventsName(tc: ReviewEventsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_version)), vS(" / "), erbTextOr(vStr(tc.review_kind))));
}

/** Computes the IsOverdue calculated field.
 *  TRUE when review is overdue.
 *  Formula: ={{NextReviewDue}} < {{AsOfInstant}} */
export function calcReviewEventsIsOverdue(tc: ReviewEventsRow): boolean | null {
  return toBoolPtr(erbCmp(erbNullif(vStr(tc.next_review_due)), "<", vStr(tc.as_of_instant)));
}

/** Computes the OverdueVersionKey calculated field.
 *  Echoes the ProcedureVersion id only when this review is past its next-due date.
 *  Formula: =IF({{IsOverdue}}, {{ProcedureVersion}}, "") */
export function calcReviewEventsOverdueVersionKey(tc: ReviewEventsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_overdue)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes the DaysSinceReviewed calculated field.
 *  Elapsed days since this review actually happened.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{ReviewedAt}}, "days") */
export function calcReviewEventsDaysSinceReviewed(tc: ReviewEventsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.reviewed_at), vS("days"))));
}

/** Computes the ExceedsPromisedCadence calculated field.
 *  TRUE when more days have elapsed since this review than the stewardship assignment promised as a cadence.
 *  Formula: ={{DaysSinceReviewed}} > {{PromisedCadenceDays}} */
export function calcReviewEventsExceedsPromisedCadence(tc: ReviewEventsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.days_since_reviewed), ">", vInt(tc.promised_cadence_days)));
}

/** Computes the CadenceDriftDays calculated field.
 *  Signed drift: positive means we are past the promised cadence by this many days; negative means we are still inside it.
 *  Formula: ={{DaysSinceReviewed}} - {{PromisedCadenceDays}} */
export function calcReviewEventsCadenceDriftDays(tc: ReviewEventsRow): number | null {
  return toIntPtr(erbInteger(erbSub(vInt(tc.days_since_reviewed), vInt(tc.promised_cadence_days))));
}

/** Computes the PromiseAndBehaviorDisagree calculated field.
 *  TRUE when the promised cadence has been blown but the hand-entered NextReviewDue still says we are fine.
 *  Formula: =AND({{ExceedsPromisedCadence}}, NOT({{IsOverdue}})) */
export function calcReviewEventsPromiseAndBehaviorDisagree(tc: ReviewEventsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.exceeds_promised_cadence)), erbBool3(erbNot(erbBool3(vBool(tc.is_overdue))))));
}

/** Computes the CadenceBreachVersionKey calculated field.
 *  Composite-key echo: this review event's procedure version when the promised cadence has been exceeded, blank otherwise.
 *  Formula: =IF({{ExceedsPromisedCadence}}, {{ProcedureVersion}}, "") */
export function calcReviewEventsCadenceBreachVersionKey(tc: ReviewEventsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.exceeds_promised_cadence)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeReviewEvents(tc: ReviewEventsRow): ReviewEventsRow {
  // Level 1
  calcGuard(tc, reviewEventsFieldTypes, "name", () => { tc.name = calcReviewEventsName(tc); });
  calcGuard(tc, reviewEventsFieldTypes, "is_overdue", () => { tc.is_overdue = calcReviewEventsIsOverdue(tc); });
  calcGuard(tc, reviewEventsFieldTypes, "days_since_reviewed", () => { tc.days_since_reviewed = calcReviewEventsDaysSinceReviewed(tc); });
  // Level 2
  calcGuard(tc, reviewEventsFieldTypes, "overdue_version_key", () => { tc.overdue_version_key = calcReviewEventsOverdueVersionKey(tc); });
  calcGuard(tc, reviewEventsFieldTypes, "exceeds_promised_cadence", () => { tc.exceeds_promised_cadence = calcReviewEventsExceedsPromisedCadence(tc); });
  calcGuard(tc, reviewEventsFieldTypes, "cadence_drift_days", () => { tc.cadence_drift_days = calcReviewEventsCadenceDriftDays(tc); });
  // Level 3
  calcGuard(tc, reviewEventsFieldTypes, "promise_and_behavior_disagree", () => { tc.promise_and_behavior_disagree = calcReviewEventsPromiseAndBehaviorDisagree(tc); });
  calcGuard(tc, reviewEventsFieldTypes, "cadence_breach_version_key", () => { tc.cadence_breach_version_key = calcReviewEventsCadenceBreachVersionKey(tc); });
  return tc;
}

/** Reads ReviewEvents rows from a JSON array file. */
export function loadReviewEventsRows(file: string): ReviewEventsRow[] {
  return loadRows(file, { fields: reviewEventsFieldTypes }) as unknown as ReviewEventsRow[];
}

// =============================================================================
// LEARNINGACTIVITIES TABLE
// Learning, retrospective, tabletop, and onboarding activities that convert execution experience into maintained knowledge. Explicit ERB-PKO extension represented as prov:Activity.
// =============================================================================

/** A row in the LearningActivities table. */
export interface LearningActivitiesRow {
  /** Stored logical identifier for one LearningActivities row. */
  learning_activity_id: string;
  /** Human-readable calculated display alias for the LearningActivities row. */
  name: string | null;
  /** Community hosting the activity. */
  community_of_practice: string | null;
  /** Procedure version practiced or reviewed. */
  procedure_version: string | null;
  /** Retrospective, TabletopExercise, Onboarding, Drill, or Apprenticeship. */
  activity_kind: string | null;
  /** Activity time. */
  occurred_at: string | null;
  /** Facilitator. */
  facilitator_agent: string | null;
  /** Knowledge or competence produced. */
  outcome: string | null;
  /** Evidence or material produced. */
  evidence_resource: string | null;
  /** Class IRI used in projection. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const learningActivitiesFieldTypes: Record<string, FieldType> = {
  learning_activity_id: "string",
  name: "*string",
  community_of_practice: "*string",
  procedure_version: "*string",
  activity_kind: "*string",
  occurred_at: "*string",
  facilitator_agent: "*string",
  outcome: "*string",
  evidence_resource: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the LearningActivities row.
 *  Formula: ={{ActivityKind}} & " / " & {{OccurredAt}} */
export function calcLearningActivitiesName(tc: LearningActivitiesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.activity_kind)), vS(" / "), erbTimestamptzText(vStr(tc.occurred_at))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeLearningActivities(tc: LearningActivitiesRow): LearningActivitiesRow {
  // Level 1
  calcGuard(tc, learningActivitiesFieldTypes, "name", () => { tc.name = calcLearningActivitiesName(tc); });
  return tc;
}

/** Reads LearningActivities rows from a JSON array file. */
export function loadLearningActivitiesRows(file: string): LearningActivitiesRow[] {
  return loadRows(file, { fields: learningActivitiesFieldTypes }) as unknown as LearningActivitiesRow[];
}

// =============================================================================
// OPERATIONALBINDINGS TABLE
// Live bindings between procedural semantics and operational data/resources. Explicit ERB-PKO extension using DCAT/DCMI/PROV identifiers.
// =============================================================================

/** A row in the OperationalBindings table. */
export interface OperationalBindingsRow {
  /** Stored logical identifier for one OperationalBindings row. */
  operational_binding_id: string;
  /** Human-readable calculated display alias for the OperationalBindings row. */
  name: string | null;
  /** Procedure version using the binding. */
  procedure_version: string | null;
  /** Step using the binding. */
  step: string | null;
  /** Bound operational resource. */
  resource: string | null;
  /** Read, Write, ReadWrite, Subscribe, or Publish. */
  access_mode: string | null;
  /** Operational record, table, event, or schema key. */
  record_or_schema_key: string | null;
  /** Most recent successful observation. */
  last_observed_at: string | null;
  /** Maximum allowed data age. */
  freshness_sla_minutes: number | null;
  /** TRUE when the binding is authoritative for the represented fact. */
  is_authoritative: boolean | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** Current observed age. */
  age_minutes: number | null;
  /** TRUE when within freshness SLA. */
  is_fresh: boolean | null;
  /** Echoes the bound Step id only when the binding is outside its freshness SLA. */
  stale_binding_step_key: string | null;
  /** Echoes the bound Step id only when an AUTHORITATIVE binding is stale. */
  authoritative_stale_step_key: string | null;
  /** TRUE when an authoritative binding has aged past its freshness SLA. */
  is_stale_and_authoritative: boolean | null;
  /** Echoes the step id when this authoritative binding is stale, blank otherwise. */
  step_when_stale: string | null;
  /** Whether the resource behind this binding is an approved source. */
  resource_is_approved: boolean | null;
  /** TRUE when this binding points at an approved source that is still inside its freshness SLA. */
  is_usable_for_drafting: boolean | null;
  /** Echoes the step id when this binding is NOT usable for drafting, blank otherwise. */
  step_when_unusable: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const operationalBindingsFieldTypes: Record<string, FieldType> = {
  operational_binding_id: "string",
  name: "*string",
  procedure_version: "*string",
  step: "*string",
  resource: "*string",
  access_mode: "*string",
  record_or_schema_key: "*string",
  last_observed_at: "*string",
  freshness_sla_minutes: "*int",
  is_authoritative: "*bool",
  evaluation_context: "*string",
  as_of_instant: "*string",
  age_minutes: "*int",
  is_fresh: "*bool",
  stale_binding_step_key: "*string",
  authoritative_stale_step_key: "*string",
  is_stale_and_authoritative: "*bool",
  step_when_stale: "*string",
  resource_is_approved: "*bool",
  is_usable_for_drafting: "*bool",
  step_when_unusable: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the OperationalBindings row.
 *  Formula: ={{Step}} & " / " & {{RecordOrSchemaKey}} */
export function calcOperationalBindingsName(tc: OperationalBindingsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS(" / "), erbTextOr(vStr(tc.record_or_schema_key))));
}

/** Computes the AgeMinutes calculated field.
 *  Current observed age.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{LastObservedAt}}, "minutes") */
export function calcOperationalBindingsAgeMinutes(tc: OperationalBindingsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.last_observed_at), vS("minutes"))));
}

/** Computes the IsFresh calculated field.
 *  TRUE when within freshness SLA.
 *  Formula: ={{AgeMinutes}} <= {{FreshnessSlaMinutes}} */
export function calcOperationalBindingsIsFresh(tc: OperationalBindingsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.age_minutes), "<=", erbNullif(vInt(tc.freshness_sla_minutes))));
}

/** Computes the StaleBindingStepKey calculated field.
 *  Echoes the bound Step id only when the binding is outside its freshness SLA.
 *  Formula: =IF(NOT({{IsFresh}}), {{Step}}, "") */
export function calcOperationalBindingsStaleBindingStepKey(tc: OperationalBindingsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbNot(erbBool3(vBool(tc.is_fresh)))), () => vStr(tc.step), () => vS("")));
}

/** Computes the AuthoritativeStaleStepKey calculated field.
 *  Echoes the bound Step id only when an AUTHORITATIVE binding is stale.
 *  Formula: =IF(AND(NOT({{IsFresh}}), {{IsAuthoritative}}), {{Step}}, "") */
export function calcOperationalBindingsAuthoritativeStaleStepKey(tc: OperationalBindingsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_fresh)))), erbIsTrue(vBool(tc.is_authoritative)))), () => vStr(tc.step), () => vS("")));
}

/** Computes the IsStaleAndAuthoritative calculated field.
 *  TRUE when an authoritative binding has aged past its freshness SLA.
 *  Formula: =AND({{IsAuthoritative}}, NOT({{IsFresh}})) */
export function calcOperationalBindingsIsStaleAndAuthoritative(tc: OperationalBindingsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.is_authoritative)), erbBool3(erbNot(erbBool3(vBool(tc.is_fresh))))));
}

/** Computes the StepWhenStale calculated field.
 *  Echoes the step id when this authoritative binding is stale, blank otherwise.
 *  Formula: =IF({{IsStaleAndAuthoritative}}, {{Step}}, "") */
export function calcOperationalBindingsStepWhenStale(tc: OperationalBindingsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_stale_and_authoritative)), () => vStr(tc.step), () => vS("")));
}

/** Computes the IsUsableForDrafting calculated field.
 *  TRUE when this binding points at an approved source that is still inside its freshness SLA.
 *  Formula: =AND({{ResourceIsApproved}}, {{IsFresh}}) */
export function calcOperationalBindingsIsUsableForDrafting(tc: OperationalBindingsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.resource_is_approved)), erbBool3(vBool(tc.is_fresh))));
}

/** Computes the StepWhenUnusable calculated field.
 *  Echoes the step id when this binding is NOT usable for drafting, blank otherwise.
 *  Formula: =IF({{IsUsableForDrafting}}, "", {{Step}}) */
export function calcOperationalBindingsStepWhenUnusable(tc: OperationalBindingsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_usable_for_drafting)), () => vS(""), () => vStr(tc.step)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeOperationalBindings(tc: OperationalBindingsRow): OperationalBindingsRow {
  // Level 1
  calcGuard(tc, operationalBindingsFieldTypes, "name", () => { tc.name = calcOperationalBindingsName(tc); });
  calcGuard(tc, operationalBindingsFieldTypes, "age_minutes", () => { tc.age_minutes = calcOperationalBindingsAgeMinutes(tc); });
  // Level 2
  calcGuard(tc, operationalBindingsFieldTypes, "is_fresh", () => { tc.is_fresh = calcOperationalBindingsIsFresh(tc); });
  // Level 3
  calcGuard(tc, operationalBindingsFieldTypes, "stale_binding_step_key", () => { tc.stale_binding_step_key = calcOperationalBindingsStaleBindingStepKey(tc); });
  calcGuard(tc, operationalBindingsFieldTypes, "authoritative_stale_step_key", () => { tc.authoritative_stale_step_key = calcOperationalBindingsAuthoritativeStaleStepKey(tc); });
  calcGuard(tc, operationalBindingsFieldTypes, "is_stale_and_authoritative", () => { tc.is_stale_and_authoritative = calcOperationalBindingsIsStaleAndAuthoritative(tc); });
  calcGuard(tc, operationalBindingsFieldTypes, "is_usable_for_drafting", () => { tc.is_usable_for_drafting = calcOperationalBindingsIsUsableForDrafting(tc); });
  // Level 4
  calcGuard(tc, operationalBindingsFieldTypes, "step_when_stale", () => { tc.step_when_stale = calcOperationalBindingsStepWhenStale(tc); });
  calcGuard(tc, operationalBindingsFieldTypes, "step_when_unusable", () => { tc.step_when_unusable = calcOperationalBindingsStepWhenUnusable(tc); });
  return tc;
}

/** Reads OperationalBindings rows from a JSON array file. */
export function loadOperationalBindingsRows(file: string): OperationalBindingsRow[] {
  return loadRows(file, { fields: operationalBindingsFieldTypes }) as unknown as OperationalBindingsRow[];
}

// =============================================================================
// COMMUNICATIONPOLICIES TABLE
// Channel-specific communication policy projected from the same canonical procedure. Uses ODRL-style policy semantics plus ERB-PKO channel constraints.
// =============================================================================

/** A row in the CommunicationPolicies table. */
export interface CommunicationPoliciesRow {
  /** Stored logical identifier for one CommunicationPolicies row. */
  communication_policy_id: string;
  /** Human-readable calculated display alias for the CommunicationPolicies row. */
  name: string | null;
  /** Procedure version governing the channel. */
  procedure_version: string | null;
  /** Email, SMS, Push, Postal, or another declared channel. */
  channel: string | null;
  /** Rule that identifies eligible recipients. */
  audience_rule: string | null;
  /** Whether active consent is required. */
  consent_required: boolean | null;
  /** Recipient-local quiet-hours start. */
  quiet_hours_start: string | null;
  /** Recipient-local quiet-hours end. */
  quiet_hours_end: string | null;
  /** Maximum message length for one unit. */
  max_message_length: number | null;
  /** Maximum number of channel segments. */
  max_segments: number | null;
  /** Retention period for rendered messages and delivery records. */
  retention_days: number | null;
  /** Role approving the channel policy. */
  approval_role: string | null;
  /** Content that every message must include. */
  required_content: string | null;
  /** Which artifact is authoritative. */
  authority_statement: string | null;
  /** Draft, Active, Suspended, or Retired. */
  status: string | null;
  /** Number of consent violations attributable to this channel policy. */
  consent_violation_count: number | null;
  /** The hour (0-23) at which the quiet window opens. Stored rather than parsed from QuietHoursStart: VALUE(LEFT(...)) does not translate — the transpiler casts the "20:00" string to a timestamp and the view errors. The sending system already knows this number, so modeling it as a parse would dress a broken derivation up as a rule. */
  quiet_hours_start_hour: number | null;
  /** Numeric hour 0-23 at which quiet hours end. Seeded 8 for comm-sms-policy, 0 for comm-email-policy. */
  quiet_hours_end_hour: number | null;
  /** Number of transmitted messages that breached this policy's quiet-hours window. */
  quiet_hours_violation_count: number | null;
  /** The exact opt-out phrase every message on this channel must contain. Seeded 'Reply STOP' for comm-sms-policy, empty string for comm-email-policy. */
  required_opt_out_phrase: string | null;
  /** TRUE only when this channel policy is in Active status. */
  is_active_policy: boolean | null;
  /** ODRL Policy class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const communicationPoliciesFieldTypes: Record<string, FieldType> = {
  communication_policy_id: "string",
  name: "*string",
  procedure_version: "*string",
  channel: "*string",
  audience_rule: "*string",
  consent_required: "*bool",
  quiet_hours_start: "*string",
  quiet_hours_end: "*string",
  max_message_length: "*int",
  max_segments: "*int",
  retention_days: "*int",
  approval_role: "*string",
  required_content: "*string",
  authority_statement: "*string",
  status: "*string",
  consent_violation_count: "*float64",
  quiet_hours_start_hour: "*int",
  quiet_hours_end_hour: "*int",
  quiet_hours_violation_count: "*float64",
  required_opt_out_phrase: "*string",
  is_active_policy: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the CommunicationPolicies row.
 *  Formula: ={{Channel}} & " policy / " & {{ProcedureVersion}} */
export function calcCommunicationPoliciesName(tc: CommunicationPoliciesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.channel)), vS(" policy / "), erbTextOr(vStr(tc.procedure_version))));
}

/** Computes the IsActivePolicy calculated field.
 *  TRUE only when this channel policy is in Active status.
 *  Formula: ={{Status}} = "Active" */
export function calcCommunicationPoliciesIsActivePolicy(tc: CommunicationPoliciesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.status)), vS("Active")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeCommunicationPolicies(tc: CommunicationPoliciesRow): CommunicationPoliciesRow {
  // Level 1
  calcGuard(tc, communicationPoliciesFieldTypes, "name", () => { tc.name = calcCommunicationPoliciesName(tc); });
  calcGuard(tc, communicationPoliciesFieldTypes, "is_active_policy", () => { tc.is_active_policy = calcCommunicationPoliciesIsActivePolicy(tc); });
  return tc;
}

/** Reads CommunicationPolicies rows from a JSON array file. */
export function loadCommunicationPoliciesRows(file: string): CommunicationPoliciesRow[] {
  return loadRows(file, { fields: communicationPoliciesFieldTypes }) as unknown as CommunicationPoliciesRow[];
}

// =============================================================================
// MESSAGETEMPLATES TABLE
// Approved channel templates projected from the canonical rulebook without becoming a second source of policy meaning.
// =============================================================================

/** A row in the MessageTemplates table. */
export interface MessageTemplatesRow {
  /** Stored logical identifier for one MessageTemplates row. */
  message_template_id: string;
  /** Human-readable calculated display alias for the MessageTemplates row. */
  name: string | null;
  /** Channel policy governing the template. */
  communication_policy: string | null;
  /** Template resource. */
  resource: string | null;
  /** Subject template where supported. */
  subject_template: string | null;
  /** Body template. */
  body_template: string | null;
  /** Locale or language tag. */
  locale: string | null;
  /** Draft, Approved, Retired, or Superseded. */
  status: string | null;
  /** Per-segment character limit inherited from the governing channel policy. */
  policy_max_message_length: number | null;
  /** Maximum permitted segment count from the governing channel policy. */
  policy_max_segments: number | null;
  /** Character length of the raw template body before variable substitution. */
  body_template_length: number | null;
  /** TRUE when the template body alone already exceeds the single-segment limit before any variables are substituted in. */
  is_template_over_length: boolean | null;
  /** Number of properly-authorized approvals on record for this template. */
  valid_approval_count: number | null;
  /** TRUE when at least one properly-authorized approval exists for this template. */
  has_valid_approval: boolean | null;
  /** TRUE when a template's Status says Approved but no properly-authorized approval record backs it. The phantom-approval witness. */
  is_claiming_unbacked_approval: boolean | null;
  /** Digest of the template's current body text, maintained whenever the body is edited. */
  current_body_hash: string | null;
  /** Digest of the body text as it stood at the most recent valid approval. */
  last_approved_body_hash: string | null;
  /** The TemplateApprovals id of the approval this template is currently sendable under. Deliberately a raw identifier, not a relationship: TemplateApprovals already points at MessageTemplates, so declaring an FK back would make the two tables mutually dependent and the rulebook is required to stay acyclic. The value is still resolved by INDEX/MATCH in LastApprovedBodyHash. */
  last_valid_approval: string | null;
  /** TRUE when the template body no longer matches what was approved. */
  has_body_drifted: boolean | null;
  /** TRUE only when the template is marked Approved, has a properly-authorized approval, AND its body still matches what was approved. */
  is_sendable_under_approval: boolean | null;
  /** How many messages went out from this template while it was not validly sendable. */
  drifted_send_count: number | null;
  /** How many transmitted messages from this template drew no acknowledgement. */
  unanswered_delivery_count: number | null;
  /** How many deliveries using this template were actually transmitted. */
  transmitted_delivery_count: number | null;
  /** TRUE when every transmitted message from this template went unacknowledged. */
  template_draws_no_response: boolean | null;
  /** When this template's currently-governing approval was decided. */
  last_approval_at: string | null;
  /** Resource class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const messageTemplatesFieldTypes: Record<string, FieldType> = {
  message_template_id: "string",
  name: "*string",
  communication_policy: "*string",
  resource: "*string",
  subject_template: "*string",
  body_template: "*string",
  locale: "*string",
  status: "*string",
  policy_max_message_length: "*int",
  policy_max_segments: "*int",
  body_template_length: "*int",
  is_template_over_length: "*bool",
  valid_approval_count: "*float64",
  has_valid_approval: "*bool",
  is_claiming_unbacked_approval: "*bool",
  current_body_hash: "*string",
  last_approved_body_hash: "*string",
  last_valid_approval: "*string",
  has_body_drifted: "*bool",
  is_sendable_under_approval: "*bool",
  drifted_send_count: "*float64",
  unanswered_delivery_count: "*float64",
  transmitted_delivery_count: "*float64",
  template_draws_no_response: "*bool",
  last_approval_at: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the MessageTemplates row.
 *  Formula: ={{CommunicationPolicy}} & " / " & {{Locale}} */
export function calcMessageTemplatesName(tc: MessageTemplatesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.communication_policy)), vS(" / "), erbTextOr(vStr(tc.locale))));
}

/** Computes the BodyTemplateLength calculated field.
 *  Character length of the raw template body before variable substitution.
 *  Formula: =LEN({{BodyTemplate}}) */
export function calcMessageTemplatesBodyTemplateLength(tc: MessageTemplatesRow): number | null {
  return toIntPtr(erbInteger(erbLen(vStr(tc.body_template))));
}

/** Computes the IsTemplateOverLength calculated field.
 *  TRUE when the template body alone already exceeds the single-segment limit before any variables are substituted in.
 *  Formula: ={{BodyTemplateLength}} > {{PolicyMaxMessageLength}} */
export function calcMessageTemplatesIsTemplateOverLength(tc: MessageTemplatesRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.body_template_length), ">", vInt(tc.policy_max_message_length)));
}

/** Computes the HasValidApproval calculated field.
 *  TRUE when at least one properly-authorized approval exists for this template.
 *  Formula: ={{ValidApprovalCount}} > 0 */
export function calcMessageTemplatesHasValidApproval(tc: MessageTemplatesRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.valid_approval_count), ">", vI(0)));
}

/** Computes the IsClaimingUnbackedApproval calculated field.
 *  TRUE when a template's Status says Approved but no properly-authorized approval record backs it. The phantom-approval witness.
 *  Formula: =AND({{Status}} = "Approved", NOT({{HasValidApproval}})) */
export function calcMessageTemplatesIsClaimingUnbackedApproval(tc: MessageTemplatesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbNot(erbBool3(vBool(tc.has_valid_approval))))));
}

/** Computes the HasBodyDrifted calculated field.
 *  TRUE when the template body no longer matches what was approved.
 *  Formula: =AND({{LastApprovedBodyHash}} <> "", {{CurrentBodyHash}} <> {{LastApprovedBodyHash}}) */
export function calcMessageTemplatesHasBodyDrifted(tc: MessageTemplatesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.last_approved_body_hash))), erbBool3(erbNe(erbNullif(vStr(tc.current_body_hash)), vStr(tc.last_approved_body_hash)))));
}

/** Computes the IsSendableUnderApproval calculated field.
 *  TRUE only when the template is marked Approved, has a properly-authorized approval, AND its body still matches what was approved.
 *  Formula: =AND({{Status}} = "Approved", AND({{HasValidApproval}}, NOT({{HasBodyDrifted}}))) */
export function calcMessageTemplatesIsSendableUnderApproval(tc: MessageTemplatesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbAnd(erbBool3(vBool(tc.has_valid_approval)), erbBool3(erbNot(erbBool3(vBool(tc.has_body_drifted))))))));
}

/** Computes the TemplateDrawsNoResponse calculated field.
 *  TRUE when every transmitted message from this template went unacknowledged.
 *  Formula: =AND({{TransmittedDeliveryCount}} > 0, {{UnansweredDeliveryCount}} = {{TransmittedDeliveryCount}}) */
export function calcMessageTemplatesTemplateDrawsNoResponse(tc: MessageTemplatesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.transmitted_delivery_count), ">", vI(0))), erbBool3(erbEq(vNum(tc.unanswered_delivery_count), vNum(tc.transmitted_delivery_count)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeMessageTemplates(tc: MessageTemplatesRow): MessageTemplatesRow {
  // Level 1
  calcGuard(tc, messageTemplatesFieldTypes, "name", () => { tc.name = calcMessageTemplatesName(tc); });
  calcGuard(tc, messageTemplatesFieldTypes, "body_template_length", () => { tc.body_template_length = calcMessageTemplatesBodyTemplateLength(tc); });
  calcGuard(tc, messageTemplatesFieldTypes, "has_valid_approval", () => { tc.has_valid_approval = calcMessageTemplatesHasValidApproval(tc); });
  calcGuard(tc, messageTemplatesFieldTypes, "has_body_drifted", () => { tc.has_body_drifted = calcMessageTemplatesHasBodyDrifted(tc); });
  calcGuard(tc, messageTemplatesFieldTypes, "template_draws_no_response", () => { tc.template_draws_no_response = calcMessageTemplatesTemplateDrawsNoResponse(tc); });
  // Level 2
  calcGuard(tc, messageTemplatesFieldTypes, "is_template_over_length", () => { tc.is_template_over_length = calcMessageTemplatesIsTemplateOverLength(tc); });
  calcGuard(tc, messageTemplatesFieldTypes, "is_claiming_unbacked_approval", () => { tc.is_claiming_unbacked_approval = calcMessageTemplatesIsClaimingUnbackedApproval(tc); });
  calcGuard(tc, messageTemplatesFieldTypes, "is_sendable_under_approval", () => { tc.is_sendable_under_approval = calcMessageTemplatesIsSendableUnderApproval(tc); });
  return tc;
}

/** Reads MessageTemplates rows from a JSON array file. */
export function loadMessageTemplatesRows(file: string): MessageTemplatesRow[] {
  return loadRows(file, { fields: messageTemplatesFieldTypes }) as unknown as MessageTemplatesRow[];
}

// =============================================================================
// SEMANTICMAPPINGS TABLE
// Machine-readable alignment from ERB table/field paths to exact PKO or reused ontology terms. Extension mappings are never presented as native PKO.
// =============================================================================

/** A row in the SemanticMappings table. */
export interface SemanticMappingsRow {
  /** Stored logical identifier for one SemanticMappings row. */
  semantic_mapping_id: string;
  /** Human-readable calculated display alias for the SemanticMappings row. */
  name: string | null;
  /** ERB table, field, or discriminator path. */
  source_path: string | null;
  /** class, objectProperty, datatypeProperty, individual, or rule. */
  mapping_kind: string | null;
  /** Exact target semantic IRI. */
  target_iri: string | null;
  /** exact, aligned, subclass, subproperty, or extension. */
  mapping_relation: string | null;
  /** Versioned ontology profile containing the target term. */
  ontology_profile: string | null;
  /** Mapping semantics and boundaries. */
  notes: string | null;
  _erb_errors?: Record<string, string>;
}

const semanticMappingsFieldTypes: Record<string, FieldType> = {
  semantic_mapping_id: "string",
  name: "*string",
  source_path: "*string",
  mapping_kind: "*string",
  target_iri: "*string",
  mapping_relation: "*string",
  ontology_profile: "*string",
  notes: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the SemanticMappings row.
 *  Formula: ={{SourcePath}} & " -> " & {{TargetIri}} */
export function calcSemanticMappingsName(tc: SemanticMappingsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.source_path)), vS(" -> "), erbTextOr(vStr(tc.target_iri))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeSemanticMappings(tc: SemanticMappingsRow): SemanticMappingsRow {
  // Level 1
  calcGuard(tc, semanticMappingsFieldTypes, "name", () => { tc.name = calcSemanticMappingsName(tc); });
  return tc;
}

/** Reads SemanticMappings rows from a JSON array file. */
export function loadSemanticMappingsRows(file: string): SemanticMappingsRow[] {
  return loadRows(file, { fields: semanticMappingsFieldTypes }) as unknown as SemanticMappingsRow[];
}

// =============================================================================
// WITNESSLOOPS TABLE
// One row per role-question expansion loop. Each loop poses questions that only became askable because of the previous loop's predicates.
// =============================================================================

/** A row in the WitnessLoops table. */
export interface WitnessLoopsRow {
  /** Stored logical identifier for one WitnessLoops row. */
  witness_loop_id: string;
  /** Human-readable calculated display alias for the WitnessLoops row. */
  name: string | null;
  /** Ordinal of this expansion loop. Loop 1 is the founding set of role questions. */
  loop_number: number;
  /** Short title for what this loop set out to make askable. */
  title: string | null;
  /** Why this loop's questions became askable. For loop N>1 this names the loop N-1 predicates that made them possible. */
  premise: string | null;
  /** Time this loop began. */
  started_at: string | null;
  /** Time this loop was committed. Null while in progress. */
  completed_at: string | null;
  /** How many role questions were posed in this loop. */
  question_count: number | null;
  /** TRUE once the loop has been committed. */
  is_complete: boolean | null;
  /** Total fields in the rulebook after this loop completed. */
  fields_after: number | null;
  /** Derived fields after this loop completed. */
  derived_after: number | null;
  /** Fields invented for a role question after this loop completed. */
  witnessed_after: number | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const witnessLoopsFieldTypes: Record<string, FieldType> = {
  witness_loop_id: "string",
  name: "*string",
  loop_number: "float64",
  title: "*string",
  premise: "*string",
  started_at: "*string",
  completed_at: "*string",
  question_count: "*float64",
  is_complete: "*bool",
  fields_after: "*float64",
  derived_after: "*float64",
  witnessed_after: "*float64",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the WitnessLoops row.
 *  Formula: ="Loop " & {{LoopNumber}} & ": " & {{Title}} */
export function calcWitnessLoopsName(tc: WitnessLoopsRow): string | null {
  return toStringPtr(erbConcat(vS("Loop "), erbTextOr(vNumPlain(tc.loop_number)), vS(": "), erbTextOr(vStr(tc.title))));
}

/** Computes the IsComplete calculated field.
 *  TRUE once the loop has been committed.
 *  Formula: ={{CompletedAt}} <> "" */
export function calcWitnessLoopsIsComplete(tc: WitnessLoopsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.completed_at)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeWitnessLoops(tc: WitnessLoopsRow): WitnessLoopsRow {
  // Level 1
  calcGuard(tc, witnessLoopsFieldTypes, "name", () => { tc.name = calcWitnessLoopsName(tc); });
  calcGuard(tc, witnessLoopsFieldTypes, "is_complete", () => { tc.is_complete = calcWitnessLoopsIsComplete(tc); });
  return tc;
}

/** Reads WitnessLoops rows from a JSON array file. */
export function loadWitnessLoopsRows(file: string): WitnessLoopsRow[] {
  return loadRows(file, { fields: witnessLoopsFieldTypes }) as unknown as WitnessLoopsRow[];
}

// =============================================================================
// ROLEQUESTIONS TABLE
// One row per question a named role wants answered. Every invented predicate in this rulebook traces back to one of these.
// =============================================================================

/** A row in the RoleQuestions table. */
export interface RoleQuestionsRow {
  /** Stored logical identifier for one RoleQuestions row. */
  role_question_id: string;
  /** Human-readable calculated display alias for the RoleQuestions row. */
  name: string | null;
  /** The role that wants this question answered. */
  asking_role: string | null;
  /** The expansion loop in which this question was posed. */
  witness_loop: string | null;
  /** The question in the role's own words. */
  question_text: string | null;
  /** What goes wrong in the real world when this question cannot be answered. */
  why_it_matters: string | null;
  /** TRUE if the model could already answer this before its loop ran. FALSE means the loop had to invent predicates for it. */
  answerable_before: boolean | null;
  /** How many fields were invented to answer this question. */
  predicate_count: number | null;
  /** TRUE when at least one predicate exists to answer this question. */
  is_answered: boolean | null;
  /** The current reading of this question's predicates, extracted from the substrate after the loop ran: which witness columns fire, on how many rows out of how many. Written by tools/extract_computed_answers.py from values Postgres computed — never recomputed in Python. This is what lets a later loop plan against materialized answers instead of imagined ones. */
  witnessed_answer: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const roleQuestionsFieldTypes: Record<string, FieldType> = {
  role_question_id: "string",
  name: "*string",
  asking_role: "*string",
  witness_loop: "*string",
  question_text: "*string",
  why_it_matters: "*string",
  answerable_before: "*bool",
  predicate_count: "*float64",
  is_answered: "*bool",
  witnessed_answer: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the RoleQuestions row.
 *  Formula: ={{AskingRole}} & ": " & LEFT({{QuestionText}}, 60) */
export function calcRoleQuestionsName(tc: RoleQuestionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.asking_role)), vS(": "), erbTextNotNull(erbLeft(vStr(tc.question_text), vI(60)))));
}

/** Computes the IsAnswered calculated field.
 *  TRUE when at least one predicate exists to answer this question.
 *  Formula: ={{PredicateCount}} > 0 */
export function calcRoleQuestionsIsAnswered(tc: RoleQuestionsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.predicate_count), ">", vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoleQuestions(tc: RoleQuestionsRow): RoleQuestionsRow {
  // Level 1
  calcGuard(tc, roleQuestionsFieldTypes, "name", () => { tc.name = calcRoleQuestionsName(tc); });
  calcGuard(tc, roleQuestionsFieldTypes, "is_answered", () => { tc.is_answered = calcRoleQuestionsIsAnswered(tc); });
  return tc;
}

/** Reads RoleQuestions rows from a JSON array file. */
export function loadRoleQuestionsRows(file: string): RoleQuestionsRow[] {
  return loadRows(file, { fields: roleQuestionsFieldTypes }) as unknown as RoleQuestionsRow[];
}

// =============================================================================
// RULEBOOKFIELDS TABLE
// A complete census of every field in this rulebook. Reconciled from the real schemas by tools/reconcile_field_catalog.py — never hand-maintained. Fields invented by a witness loop carry an InventedForQuestion FK.
// =============================================================================

/** A row in the RulebookFields table. */
export interface RulebookFieldsRow {
  /** Stored logical identifier for one RulebookFields row. Formed as <TargetTable>.<FieldName>. */
  rulebook_field_id: string;
  /** Human-readable calculated display alias for the RulebookFields row. */
  name: string | null;
  /** The table this field lives on. */
  target_table: string | null;
  /** The field's name within its table. */
  field_name: string | null;
  /** raw, calculated, lookup, relationship, or aggregation. */
  field_type: string | null;
  /** The field's declared datatype. */
  datatype: string | null;
  /** The field's formula when it is derived. Null for raw and relationship fields. */
  formula: string | null;
  /** The role question that motivated this field's existence. Null for fields that predate the witness-loop exercise. */
  invented_for_question: string | null;
  /** TRUE when this field is computed rather than stored. */
  is_derived: boolean | null;
  /** TRUE when this field exists because a role asked a question. These are the fields the witness loops added. */
  is_witness: boolean | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const rulebookFieldsFieldTypes: Record<string, FieldType> = {
  rulebook_field_id: "string",
  name: "*string",
  target_table: "*string",
  field_name: "*string",
  field_type: "*string",
  datatype: "*string",
  formula: "*string",
  invented_for_question: "*string",
  is_derived: "*bool",
  is_witness: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the RulebookFields row.
 *  Formula: ={{TargetTable}} & "." & {{FieldName}} */
export function calcRulebookFieldsName(tc: RulebookFieldsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.target_table)), vS("."), erbTextOr(vStr(tc.field_name))));
}

/** Computes the IsDerived calculated field.
 *  TRUE when this field is computed rather than stored.
 *  Formula: =OR({{FieldType}} = "calculated", {{FieldType}} = "lookup", {{FieldType}} = "aggregation") */
export function calcRulebookFieldsIsDerived(tc: RulebookFieldsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.field_type)), vS("calculated"))), erbBool3(erbEq(erbNullif(vStr(tc.field_type)), vS("lookup"))), erbBool3(erbEq(erbNullif(vStr(tc.field_type)), vS("aggregation")))));
}

/** Computes the IsWitness calculated field.
 *  TRUE when this field exists because a role asked a question. These are the fields the witness loops added.
 *  Formula: ={{InventedForQuestion}} <> "" */
export function calcRulebookFieldsIsWitness(tc: RulebookFieldsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.invented_for_question)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRulebookFields(tc: RulebookFieldsRow): RulebookFieldsRow {
  // Level 1
  calcGuard(tc, rulebookFieldsFieldTypes, "name", () => { tc.name = calcRulebookFieldsName(tc); });
  calcGuard(tc, rulebookFieldsFieldTypes, "is_derived", () => { tc.is_derived = calcRulebookFieldsIsDerived(tc); });
  calcGuard(tc, rulebookFieldsFieldTypes, "is_witness", () => { tc.is_witness = calcRulebookFieldsIsWitness(tc); });
  return tc;
}

/** Reads RulebookFields rows from a JSON array file. */
export function loadRulebookFieldsRows(file: string): RulebookFieldsRow[] {
  return loadRows(file, { fields: rulebookFieldsFieldTypes }) as unknown as RulebookFieldsRow[];
}

// =============================================================================
// TESTSUITES TABLE
// Groups of conformance checks. Rollups here are computed from TestCases, so the board's headline is itself a derived field.
// =============================================================================

/** A row in the TestSuites table. */
export interface TestSuitesRow {
  /** Stored logical identifier for one TestSuites row. */
  test_suite_id: string;
  /** Human-readable calculated display alias for the TestSuites row. */
  name: string | null;
  /** Display name for this suite. */
  label: string | null;
  /** How many checks belong to this suite. */
  test_count: number | null;
  /** How many checks passed on the last run. */
  pass_count: number | null;
  /** How many blocking checks failed. This is the number that must be zero for the board to be green. */
  blocking_fail_count: number | null;
  /** TRUE when no blocking check is failing. Advisory warnings do not break the board. */
  is_green: boolean | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const testSuitesFieldTypes: Record<string, FieldType> = {
  test_suite_id: "string",
  name: "*string",
  label: "*string",
  test_count: "*float64",
  pass_count: "*float64",
  blocking_fail_count: "*float64",
  is_green: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the TestSuites row.
 *  Formula: ={{Label}} */
export function calcTestSuitesName(tc: TestSuitesRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes the IsGreen calculated field.
 *  TRUE when no blocking check is failing. Advisory warnings do not break the board.
 *  Formula: ={{BlockingFailCount}} = 0 */
export function calcTestSuitesIsGreen(tc: TestSuitesRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.blocking_fail_count), vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeTestSuites(tc: TestSuitesRow): TestSuitesRow {
  // Level 1
  calcGuard(tc, testSuitesFieldTypes, "name", () => { tc.name = calcTestSuitesName(tc); });
  calcGuard(tc, testSuitesFieldTypes, "is_green", () => { tc.is_green = calcTestSuitesIsGreen(tc); });
  return tc;
}

/** Reads TestSuites rows from a JSON array file. */
export function loadTestSuitesRows(file: string): TestSuitesRow[] {
  return loadRows(file, { fields: testSuitesFieldTypes }) as unknown as TestSuitesRow[];
}

// =============================================================================
// TESTCASES TABLE
// The conformance suite, as data. One row per check, naming what it checks and — where applicable — the role question whose answer it defends. tools/run_test_suite.py executes these rows and writes the outcome back; it invents no checks of its own.
// =============================================================================

/** A row in the TestCases table. */
export interface TestCasesRow {
  /** Stored logical identifier for one TestCases row. */
  test_case_id: string;
  /** Human-readable calculated display alias for the TestCases row. */
  name: string | null;
  /** What class of check this is. One of: structural (the rulebook's own shape), formula-translates (the transpiler emitted a real function, not a NULL stub), view-loads (the view exists and is queryable), fk-resolves (every FK value names a real row), witness-discriminates (a boolean witness can distinguish cases in this data), witness-fires (a specific witness reads TRUE on at least one row), provenance (every invented field traces to a question), catalog-sync (RulebookFields matches the real schemas), question-answered (a role question has at least one predicate answering it), invariant (a domain rule that must hold), remediation (a seeded violation was resolved in model rather than deleted). */
  test_kind: string | null;
  /** What is under test — a Table.Field, a table, a view, or a question id. */
  subject: string | null;
  /** The table this check reads, when it reads one. */
  target_table: string | null;
  /** The field this check reads, when it reads one. */
  target_field: string | null;
  /** What must be true, stated so a human can judge the verdict without reading code. */
  assertion: string | null;
  /** The role question whose answer this check protects. Null for checks that defend the model's structure rather than a specific question. */
  defends_question: string | null;
  /** The suite this check belongs to. */
  suite: string | null;
  /** blocking — a failure means the model is stating something false; advisory — a failure means the model cannot state something it should be able to. Vacuity is advisory by design: a witness that cannot fire on this seed is not necessarily wrong, and forcing it red would create pressure to fabricate data. */
  severity: string | null;
  /** TRUE when a failure of this check means the model is asserting something false. */
  is_blocking: boolean | null;
  /** PASS, WARN, FAIL, or SKIP from the most recent run. Written by tools/run_test_suite.py from the substrate — never hand-edited. */
  last_outcome: string | null;
  /** The observed reading behind LastOutcome, e.g. a fire count or the error text. */
  last_detail: string | null;
  /** When this check last ran. */
  last_run_at: string | null;
  /** TRUE when the last run passed outright. */
  is_passing: boolean | null;
  /** TRUE when the last run failed. A blocking failure means the model is asserting something false. */
  is_failing: boolean | null;
  /** TRUE when this check failed and its failure means the model is wrong. This is the number that must be zero. */
  needs_attention: boolean | null;
  /** Echoes the suite id only for checks that passed; empty otherwise. Single-criterion COUNTIFS key. */
  passing_suite_key: string | null;
  /** Echoes the suite id only for blocking checks that failed; empty otherwise. Single-criterion COUNTIFS key. */
  needs_attention_suite_key: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const testCasesFieldTypes: Record<string, FieldType> = {
  test_case_id: "string",
  name: "*string",
  test_kind: "*string",
  subject: "*string",
  target_table: "*string",
  target_field: "*string",
  assertion: "*string",
  defends_question: "*string",
  suite: "*string",
  severity: "*string",
  is_blocking: "*bool",
  last_outcome: "*string",
  last_detail: "*string",
  last_run_at: "*string",
  is_passing: "*bool",
  is_failing: "*bool",
  needs_attention: "*bool",
  passing_suite_key: "*string",
  needs_attention_suite_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the TestCases row.
 *  Formula: ={{TestKind}} & ": " & {{Subject}} */
export function calcTestCasesName(tc: TestCasesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.test_kind)), vS(": "), erbTextOr(vStr(tc.subject))));
}

/** Computes the IsBlocking calculated field.
 *  TRUE when a failure of this check means the model is asserting something false.
 *  Formula: ={{Severity}} = "blocking" */
export function calcTestCasesIsBlocking(tc: TestCasesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.severity)), vS("blocking")));
}

/** Computes the IsPassing calculated field.
 *  TRUE when the last run passed outright.
 *  Formula: ={{LastOutcome}} = "PASS" */
export function calcTestCasesIsPassing(tc: TestCasesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.last_outcome)), vS("PASS")));
}

/** Computes the IsFailing calculated field.
 *  TRUE when the last run failed. A blocking failure means the model is asserting something false.
 *  Formula: ={{LastOutcome}} = "FAIL" */
export function calcTestCasesIsFailing(tc: TestCasesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.last_outcome)), vS("FAIL")));
}

/** Computes the NeedsAttention calculated field.
 *  TRUE when this check failed and its failure means the model is wrong. This is the number that must be zero.
 *  Formula: =AND({{IsFailing}}, {{IsBlocking}}) */
export function calcTestCasesNeedsAttention(tc: TestCasesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_failing)), erbBool3(vBool(tc.is_blocking))));
}

/** Computes the PassingSuiteKey calculated field.
 *  Echoes the suite id only for checks that passed; empty otherwise. Single-criterion COUNTIFS key.
 *  Formula: =IF({{IsPassing}}, {{Suite}}, "") */
export function calcTestCasesPassingSuiteKey(tc: TestCasesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_passing)), () => vStr(tc.suite), () => vS("")));
}

/** Computes the NeedsAttentionSuiteKey calculated field.
 *  Echoes the suite id only for blocking checks that failed; empty otherwise. Single-criterion COUNTIFS key.
 *  Formula: =IF({{NeedsAttention}}, {{Suite}}, "") */
export function calcTestCasesNeedsAttentionSuiteKey(tc: TestCasesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.needs_attention)), () => vStr(tc.suite), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeTestCases(tc: TestCasesRow): TestCasesRow {
  // Level 1
  calcGuard(tc, testCasesFieldTypes, "name", () => { tc.name = calcTestCasesName(tc); });
  calcGuard(tc, testCasesFieldTypes, "is_blocking", () => { tc.is_blocking = calcTestCasesIsBlocking(tc); });
  calcGuard(tc, testCasesFieldTypes, "is_passing", () => { tc.is_passing = calcTestCasesIsPassing(tc); });
  calcGuard(tc, testCasesFieldTypes, "is_failing", () => { tc.is_failing = calcTestCasesIsFailing(tc); });
  // Level 2
  calcGuard(tc, testCasesFieldTypes, "needs_attention", () => { tc.needs_attention = calcTestCasesNeedsAttention(tc); });
  calcGuard(tc, testCasesFieldTypes, "passing_suite_key", () => { tc.passing_suite_key = calcTestCasesPassingSuiteKey(tc); });
  // Level 3
  calcGuard(tc, testCasesFieldTypes, "needs_attention_suite_key", () => { tc.needs_attention_suite_key = calcTestCasesNeedsAttentionSuiteKey(tc); });
  return tc;
}

/** Reads TestCases rows from a JSON array file. */
export function loadTestCasesRows(file: string): TestCasesRow[] {
  return loadRows(file, { fields: testCasesFieldTypes }) as unknown as TestCasesRow[];
}

// =============================================================================
// ERBVERSIONS TABLE
// Standard ERB semantic version history.
// =============================================================================

/** A row in the ERBVersions table. */
export interface ERBVersionsRow {
  /** Stored ERB version identifier. */
  erb_version_id: string;
  /** Source base identifier. */
  base_id: string | null;
  /** Version label. */
  name: string | null;
  /** Version message. */
  message: string | null;
  /** Version notes. */
  notes: string | null;
  /** Commit timestamp. */
  commit_date: string | null;
  /** Publication flag. */
  is_published: boolean | null;
  _erb_errors?: Record<string, string>;
}

const eRBVersionsFieldTypes: Record<string, FieldType> = {
  erb_version_id: "string",
  base_id: "*string",
  name: "*string",
  message: "*string",
  notes: "*string",
  commit_date: "*string",
  is_published: "*bool",
};

/** Computes every calculated field of the row in dependency order. */
export function computeERBVersions(tc: ERBVersionsRow): ERBVersionsRow {
  return tc;
}

/** Reads ERBVersions rows from a JSON array file. */
export function loadERBVersionsRows(file: string): ERBVersionsRow[] {
  return loadRows(file, { fields: eRBVersionsFieldTypes }) as unknown as ERBVersionsRow[];
}

// =============================================================================
// ERBCUSTOMIZATIONS TABLE
// Explicit customization seams; empty because the canonical model is expressed in the rulebook.
// =============================================================================

/** A row in the ERBCustomizations table. */
export interface ERBCustomizationsRow {
  /** Customization identifier. */
  erb_customization_id: string;
  /** Customization file name. */
  name: string | null;
  /** Customization title. */
  title: string | null;
  /** Customization SQL. */
  sql_code: string | null;
  /** Target substrate. */
  sql_target: string | null;
  /** Customization category. */
  customization_type: string | null;
  _erb_errors?: Record<string, string>;
}

const eRBCustomizationsFieldTypes: Record<string, FieldType> = {
  erb_customization_id: "string",
  name: "*string",
  title: "*string",
  sql_code: "*string",
  sql_target: "*string",
  customization_type: "*string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeERBCustomizations(tc: ERBCustomizationsRow): ERBCustomizationsRow {
  return tc;
}

/** Reads ERBCustomizations rows from a JSON array file. */
export function loadERBCustomizationsRows(file: string): ERBCustomizationsRow[] {
  return loadRows(file, { fields: eRBCustomizationsFieldTypes }) as unknown as ERBCustomizationsRow[];
}

// =============================================================================
// __META__ TABLE
// Project-level metadata that travels with the rulebook: tagline, motif, narrative descriptions, PKO version contract, substrate list, signature rows, etc. One row per metadata key. Use ValueType to interpret StringValue vs JsonValue.
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

// =============================================================================
// EXCEPTIONINVOCATIONS TABLE
// ExceptionInvocations (added by witness loop 1).
// =============================================================================

/** A row in the ExceptionInvocations table. */
export interface ExceptionInvocationsRow {
  /** Stored logical identifier for one ExceptionInvocations row. */
  exception_invocation_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** The execution during which the exception was invoked. */
  step_execution: string | null;
  /** The specified exception that was invoked. */
  exception: string | null;
  /** Agent who invoked the exception. */
  invoked_by_agent: string | null;
  /** Agent who approved the invocation. Must satisfy the exception's ApprovalRole. */
  approved_by_agent: string | null;
  /** When the exception was invoked. */
  invoked_at: string | null;
  /** What was actually done, in the invoker's words. */
  handling_applied: string | null;
  /** The handling the specification prescribes. */
  expected_handling: string | null;
  /** The role the specification requires to approve this exception. */
  required_approval_role: string | null;
  /** The agent who currently holds the role that must approve this exception. */
  required_approval_role_holder: string | null;
  /** TRUE when the agent who approved is the holder of the role the exception requires. */
  approval_role_matches: boolean | null;
  /** TRUE when an approver was recorded at all. */
  is_approved: boolean | null;
  /** TRUE when an exception was invoked without an approver, or approved by an agent who does not hold the required role. */
  is_improperly_approved: boolean | null;
  /** What kind of agent invoked the exception. */
  invoker_agent_kind: string | null;
  /** Composite execution+approver key, in the same key space as StepExecutions.PreparerAgentKey. */
  invoker_also_prepared_key: string | null;
  /** The procedure execution this invocation belongs to. */
  parent_procedure_execution: string | null;
  /** How many preparation steps in the same execution were run by the agent who approved this exception. */
  approver_prepared_count: number | null;
  /** TRUE when an exception routed approval authority to an agent who prepared work in the same execution. */
  delegated_to_preparer: boolean | null;
  /** TRUE when an exception was invoked without proper role authority, or routed authority to the preparer. */
  is_ungoverned_invocation: boolean | null;
  /** Extension IRI — PKO does not define exception invocation. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const exceptionInvocationsFieldTypes: Record<string, FieldType> = {
  exception_invocation_id: "string",
  name: "*string",
  step_execution: "*string",
  exception: "*string",
  invoked_by_agent: "*string",
  approved_by_agent: "*string",
  invoked_at: "*string",
  handling_applied: "*string",
  expected_handling: "*string",
  required_approval_role: "*string",
  required_approval_role_holder: "*string",
  approval_role_matches: "*bool",
  is_approved: "*bool",
  is_improperly_approved: "*bool",
  invoker_agent_kind: "*string",
  invoker_also_prepared_key: "*string",
  parent_procedure_execution: "*string",
  approver_prepared_count: "*float64",
  delegated_to_preparer: "*bool",
  is_ungoverned_invocation: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{StepExecution}} & " / " & {{Exception}} */
export function calcExceptionInvocationsName(tc: ExceptionInvocationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step_execution)), vS(" / "), erbTextOr(vStr(tc.exception))));
}

/** Computes the ApprovalRoleMatches calculated field.
 *  TRUE when the agent who approved is the holder of the role the exception requires.
 *  Formula: ={{ApprovedByAgent}} = {{RequiredApprovalRoleHolder}} */
export function calcExceptionInvocationsApprovalRoleMatches(tc: ExceptionInvocationsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.approved_by_agent)), vStr(tc.required_approval_role_holder)));
}

/** Computes the IsApproved calculated field.
 *  TRUE when an approver was recorded at all.
 *  Formula: ={{ApprovedByAgent}} <> "" */
export function calcExceptionInvocationsIsApproved(tc: ExceptionInvocationsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.approved_by_agent)));
}

/** Computes the IsImproperlyApproved calculated field.
 *  TRUE when an exception was invoked without an approver, or approved by an agent who does not hold the required role.
 *  Formula: =OR(NOT({{IsApproved}}), NOT({{ApprovalRoleMatches}})) */
export function calcExceptionInvocationsIsImproperlyApproved(tc: ExceptionInvocationsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.is_approved)))), erbBool3(erbNot(erbBool3(vBool(tc.approval_role_matches))))));
}

/** Computes the InvokerAlsoPreparedKey calculated field.
 *  Composite execution+approver key, in the same key space as StepExecutions.PreparerAgentKey.
 *  Formula: ={{ParentProcedureExecution}} & "|" & {{ApprovedByAgent}} */
export function calcExceptionInvocationsInvokerAlsoPreparedKey(tc: ExceptionInvocationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.parent_procedure_execution)), vS("|"), erbTextOr(vStr(tc.approved_by_agent))));
}

/** Computes the DelegatedToPreparer calculated field.
 *  TRUE when an exception routed approval authority to an agent who prepared work in the same execution.
 *  Formula: ={{ApproverPreparedCount}} > 0 */
export function calcExceptionInvocationsDelegatedToPreparer(tc: ExceptionInvocationsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.approver_prepared_count), ">", vI(0)));
}

/** Computes the IsUngovernedInvocation calculated field.
 *  TRUE when an exception was invoked without proper role authority, or routed authority to the preparer.
 *  Formula: =OR({{IsImproperlyApproved}}, {{DelegatedToPreparer}}) */
export function calcExceptionInvocationsIsUngovernedInvocation(tc: ExceptionInvocationsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(vBool(tc.is_improperly_approved)), erbBool3(vBool(tc.delegated_to_preparer))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeExceptionInvocations(tc: ExceptionInvocationsRow): ExceptionInvocationsRow {
  // Level 1
  calcGuard(tc, exceptionInvocationsFieldTypes, "name", () => { tc.name = calcExceptionInvocationsName(tc); });
  calcGuard(tc, exceptionInvocationsFieldTypes, "approval_role_matches", () => { tc.approval_role_matches = calcExceptionInvocationsApprovalRoleMatches(tc); });
  calcGuard(tc, exceptionInvocationsFieldTypes, "is_approved", () => { tc.is_approved = calcExceptionInvocationsIsApproved(tc); });
  calcGuard(tc, exceptionInvocationsFieldTypes, "invoker_also_prepared_key", () => { tc.invoker_also_prepared_key = calcExceptionInvocationsInvokerAlsoPreparedKey(tc); });
  calcGuard(tc, exceptionInvocationsFieldTypes, "delegated_to_preparer", () => { tc.delegated_to_preparer = calcExceptionInvocationsDelegatedToPreparer(tc); });
  // Level 2
  calcGuard(tc, exceptionInvocationsFieldTypes, "is_improperly_approved", () => { tc.is_improperly_approved = calcExceptionInvocationsIsImproperlyApproved(tc); });
  // Level 3
  calcGuard(tc, exceptionInvocationsFieldTypes, "is_ungoverned_invocation", () => { tc.is_ungoverned_invocation = calcExceptionInvocationsIsUngovernedInvocation(tc); });
  return tc;
}

/** Reads ExceptionInvocations rows from a JSON array file. */
export function loadExceptionInvocationsRows(file: string): ExceptionInvocationsRow[] {
  return loadRows(file, { fields: exceptionInvocationsFieldTypes }) as unknown as ExceptionInvocationsRow[];
}

// =============================================================================
// VERIFICATIONOUTCOMES TABLE
// VerificationOutcomes (added by witness loop 1).
// =============================================================================

/** A row in the VerificationOutcomes table. */
export interface VerificationOutcomesRow {
  /** Stored logical identifier for one VerificationOutcomes row. */
  verification_outcome_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** The execution during which the verification was performed. */
  step_execution: string | null;
  /** The declared verification being performed. */
  step_verification: string | null;
  /** The value the signal actually read at verification time. */
  observed_signal_value: string | null;
  /** Agent who observed the signal. */
  observed_by_agent: string | null;
  /** When the signal was observed. */
  observed_at: string | null;
  /** Pointer to the retained artifact backing the observation. */
  evidence_uri: string | null;
  /** The value the specification expects. */
  expected_signal_value: string | null;
  /** Which signal this outcome concerns. */
  signal_identifier: string | null;
  /** TRUE when the observed value equals the expected value. */
  signal_matches_expected: boolean | null;
  /** TRUE when a retained artifact backs this observation. */
  has_evidence: boolean | null;
  /** TRUE when a verification was recorded as matching but no evidence artifact was retained. */
  is_unbacked_observation: boolean | null;
  /** TRUE when the agent who observed the verification signal is the same agent who executed the step being verified. */
  is_self_witnessed: boolean | null;
  /** The agent who executed the step this verification outcome concerns. */
  step_executor_agent: string | null;
  /** Echoes the StepExecution id only for self-witnessed verifications. */
  self_witnessed_step_key: string | null;
  /** Echoes the StepExecution id when a verification matched but retained no evidence. */
  unbacked_step_key: string | null;
  /** TRUE when the executor observed their own passing signal and attached no evidence. */
  is_self_witnessed_and_unbacked: boolean | null;
  /** TRUE when a PASSING signal was self-observed with no evidence behind it. */
  is_uncorroborated_pass: boolean | null;
  /** Echoes the step execution id when this outcome is an uncorroborated pass. */
  uncorroborated_pass_step_key: string | null;
  /** Whether the agent who recorded this observation is non-human. Agents has no PrimaryRole column and an FK from Agents back to Roles would create a table cycle, so the independence question is answered by agent kind rather than by role. */
  observer_is_non_human: boolean | null;
  /** TRUE when someone other than the step's executor recorded the observation. */
  observer_is_independent_of_executor: boolean | null;
  /** TRUE when a human other than the step's executor observed this signal and attached evidence. */
  is_independent_human_observation: boolean | null;
  /** Echoes the parent execution id when this is an independent human observation. */
  independent_observation_execution_key: string | null;
  /** The procedure execution this verification outcome belongs to. */
  parent_procedure_execution_of_outcome: string | null;
  /** Extension IRI — PKO 2.0.0 has no execution-side verification outcome class. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const verificationOutcomesFieldTypes: Record<string, FieldType> = {
  verification_outcome_id: "string",
  name: "*string",
  step_execution: "*string",
  step_verification: "*string",
  observed_signal_value: "*string",
  observed_by_agent: "*string",
  observed_at: "*string",
  evidence_uri: "*string",
  expected_signal_value: "*string",
  signal_identifier: "*string",
  signal_matches_expected: "*bool",
  has_evidence: "*bool",
  is_unbacked_observation: "*bool",
  is_self_witnessed: "*bool",
  step_executor_agent: "*string",
  self_witnessed_step_key: "*string",
  unbacked_step_key: "*string",
  is_self_witnessed_and_unbacked: "*bool",
  is_uncorroborated_pass: "*bool",
  uncorroborated_pass_step_key: "*string",
  observer_is_non_human: "*bool",
  observer_is_independent_of_executor: "*bool",
  is_independent_human_observation: "*bool",
  independent_observation_execution_key: "*string",
  parent_procedure_execution_of_outcome: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{StepExecution}} & " / " & {{StepVerification}} */
export function calcVerificationOutcomesName(tc: VerificationOutcomesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step_execution)), vS(" / "), erbTextOr(vStr(tc.step_verification))));
}

/** Computes the SignalMatchesExpected calculated field.
 *  TRUE when the observed value equals the expected value.
 *  Formula: ={{ObservedSignalValue}} = {{ExpectedSignalValue}} */
export function calcVerificationOutcomesSignalMatchesExpected(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.observed_signal_value)), vStr(tc.expected_signal_value)));
}

/** Computes the HasEvidence calculated field.
 *  TRUE when a retained artifact backs this observation.
 *  Formula: ={{EvidenceUri}} <> "" */
export function calcVerificationOutcomesHasEvidence(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.evidence_uri)));
}

/** Computes the IsUnbackedObservation calculated field.
 *  TRUE when a verification was recorded as matching but no evidence artifact was retained.
 *  Formula: =AND({{SignalMatchesExpected}}, NOT({{HasEvidence}})) */
export function calcVerificationOutcomesIsUnbackedObservation(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.signal_matches_expected)), erbBool3(erbNot(erbBool3(vBool(tc.has_evidence))))));
}

/** Computes the IsSelfWitnessed calculated field.
 *  TRUE when the agent who observed the verification signal is the same agent who executed the step being verified.
 *  Formula: ={{ObservedByAgent}} = {{StepExecutorAgent}} */
export function calcVerificationOutcomesIsSelfWitnessed(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.observed_by_agent)), vStr(tc.step_executor_agent)));
}

/** Computes the SelfWitnessedStepKey calculated field.
 *  Echoes the StepExecution id only for self-witnessed verifications.
 *  Formula: =IF({{IsSelfWitnessed}}, {{StepExecution}}, "") */
export function calcVerificationOutcomesSelfWitnessedStepKey(tc: VerificationOutcomesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_self_witnessed)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the UnbackedStepKey calculated field.
 *  Echoes the StepExecution id when a verification matched but retained no evidence.
 *  Formula: =IF({{IsUnbackedObservation}}, {{StepExecution}}, "") */
export function calcVerificationOutcomesUnbackedStepKey(tc: VerificationOutcomesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unbacked_observation)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the IsSelfWitnessedAndUnbacked calculated field.
 *  TRUE when the executor observed their own passing signal and attached no evidence.
 *  Formula: =AND({{IsSelfWitnessed}}, NOT({{HasEvidence}})) */
export function calcVerificationOutcomesIsSelfWitnessedAndUnbacked(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_self_witnessed)), erbBool3(erbNot(erbBool3(vBool(tc.has_evidence))))));
}

/** Computes the IsUncorroboratedPass calculated field.
 *  TRUE when a PASSING signal was self-observed with no evidence behind it.
 *  Formula: =AND({{SignalMatchesExpected}}, {{IsSelfWitnessedAndUnbacked}}) */
export function calcVerificationOutcomesIsUncorroboratedPass(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.signal_matches_expected)), erbBool3(vBool(tc.is_self_witnessed_and_unbacked))));
}

/** Computes the UncorroboratedPassStepKey calculated field.
 *  Echoes the step execution id when this outcome is an uncorroborated pass.
 *  Formula: =IF({{IsUncorroboratedPass}}, {{StepExecution}}, "") */
export function calcVerificationOutcomesUncorroboratedPassStepKey(tc: VerificationOutcomesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_uncorroborated_pass)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the ObserverIsIndependentOfExecutor calculated field.
 *  TRUE when someone other than the step's executor recorded the observation.
 *  Formula: =NOT({{IsSelfWitnessed}}) */
export function calcVerificationOutcomesObserverIsIndependentOfExecutor(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(vBool(tc.is_self_witnessed))));
}

/** Computes the IsIndependentHumanObservation calculated field.
 *  TRUE when a human other than the step's executor observed this signal and attached evidence.
 *  Formula: =AND(NOT({{ObserverIsNonHuman}}), NOT({{IsSelfWitnessed}}), {{HasEvidence}}) */
export function calcVerificationOutcomesIsIndependentHumanObservation(tc: VerificationOutcomesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.observer_is_non_human)))), erbBool3(erbNot(erbBool3(vBool(tc.is_self_witnessed)))), erbBool3(vBool(tc.has_evidence))));
}

/** Computes the IndependentObservationExecutionKey calculated field.
 *  Echoes the parent execution id when this is an independent human observation.
 *  Formula: =IF({{IsIndependentHumanObservation}}, {{ParentProcedureExecutionOfOutcome}}, "") */
export function calcVerificationOutcomesIndependentObservationExecutionKey(tc: VerificationOutcomesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_independent_human_observation)), () => vStr(tc.parent_procedure_execution_of_outcome), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeVerificationOutcomes(tc: VerificationOutcomesRow): VerificationOutcomesRow {
  // Level 1
  calcGuard(tc, verificationOutcomesFieldTypes, "name", () => { tc.name = calcVerificationOutcomesName(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "signal_matches_expected", () => { tc.signal_matches_expected = calcVerificationOutcomesSignalMatchesExpected(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "has_evidence", () => { tc.has_evidence = calcVerificationOutcomesHasEvidence(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "is_self_witnessed", () => { tc.is_self_witnessed = calcVerificationOutcomesIsSelfWitnessed(tc); });
  // Level 2
  calcGuard(tc, verificationOutcomesFieldTypes, "is_unbacked_observation", () => { tc.is_unbacked_observation = calcVerificationOutcomesIsUnbackedObservation(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "self_witnessed_step_key", () => { tc.self_witnessed_step_key = calcVerificationOutcomesSelfWitnessedStepKey(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "is_self_witnessed_and_unbacked", () => { tc.is_self_witnessed_and_unbacked = calcVerificationOutcomesIsSelfWitnessedAndUnbacked(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "observer_is_independent_of_executor", () => { tc.observer_is_independent_of_executor = calcVerificationOutcomesObserverIsIndependentOfExecutor(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "is_independent_human_observation", () => { tc.is_independent_human_observation = calcVerificationOutcomesIsIndependentHumanObservation(tc); });
  // Level 3
  calcGuard(tc, verificationOutcomesFieldTypes, "unbacked_step_key", () => { tc.unbacked_step_key = calcVerificationOutcomesUnbackedStepKey(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "is_uncorroborated_pass", () => { tc.is_uncorroborated_pass = calcVerificationOutcomesIsUncorroboratedPass(tc); });
  calcGuard(tc, verificationOutcomesFieldTypes, "independent_observation_execution_key", () => { tc.independent_observation_execution_key = calcVerificationOutcomesIndependentObservationExecutionKey(tc); });
  // Level 4
  calcGuard(tc, verificationOutcomesFieldTypes, "uncorroborated_pass_step_key", () => { tc.uncorroborated_pass_step_key = calcVerificationOutcomesUncorroboratedPassStepKey(tc); });
  return tc;
}

/** Reads VerificationOutcomes rows from a JSON array file. */
export function loadVerificationOutcomesRows(file: string): VerificationOutcomesRow[] {
  return loadRows(file, { fields: verificationOutcomesFieldTypes }) as unknown as VerificationOutcomesRow[];
}

// =============================================================================
// OBSERVEDTRANSITIONS TABLE
// The proxy above cannot tell a walked fallback from a happy-path step that happens to share an endpoint. PKO models Transition as a first-class thing; its execution counterpart is missing. Without a table that records WHICH transition a step execution arrived by, 'has this fallback ever been walked' is permanently unanswerable rather than merely unanswered. This is an extension (urn:effortless:pko-extension#ObservedTransition), not a native PKO term.
// =============================================================================

/** A row in the ObservedTransitions table. */
export interface ObservedTransitionsRow {
  /** Stored logical identifier for one ObservedTransitions row. */
  observed_transition_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** The procedure execution during which this transition was traversed. */
  procedure_execution: string;
  /** The specification-level transition that was actually taken. */
  step_transition: string;
  /** The step execution that this traversal produced. */
  arriving_step_execution: string | null;
  /** When the traversal occurred. */
  observed_at: string | null;
  /** Why this path rather than the default was taken. */
  trigger_reason: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const observedTransitionsFieldTypes: Record<string, FieldType> = {
  observed_transition_id: "string",
  name: "*string",
  procedure_execution: "string",
  step_transition: "string",
  arriving_step_execution: "*string",
  observed_at: "*string",
  trigger_reason: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{StepTransition}} & " @ " & {{ObservedAt}} */
export function calcObservedTransitionsName(tc: ObservedTransitionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStrPlain(tc.step_transition)), vS(" @ "), erbTimestamptzText(vStr(tc.observed_at))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeObservedTransitions(tc: ObservedTransitionsRow): ObservedTransitionsRow {
  // Level 1
  calcGuard(tc, observedTransitionsFieldTypes, "name", () => { tc.name = calcObservedTransitionsName(tc); });
  return tc;
}

/** Reads ObservedTransitions rows from a JSON array file. */
export function loadObservedTransitionsRows(file: string): ObservedTransitionsRow[] {
  return loadRows(file, { fields: observedTransitionsFieldTypes }) as unknown as ObservedTransitionsRow[];
}

// =============================================================================
// RECIPIENTS TABLE
// Recipients (added by witness loop 1).
// =============================================================================

/** A row in the Recipients table. */
export interface RecipientsRow {
  /** Stored logical identifier for one Recipients row. */
  recipient_id: string;
  /** Human-readable calculated display alias for the Recipients row. */
  name: string | null;
  /** Employee display name as carried in the consent registry. */
  display_name: string | null;
  /** Organization the recipient belongs to. */
  organization: string | null;
  /** Corporate email address; empty string when the recipient has no valid corporate email. */
  email_address: string | null;
  /** Mobile number in E.164 form; empty string when no mobile number is on file. */
  mobile_number: string | null;
  /** Consent state as observed in the consent registry: Granted, Revoked, or NeverGiven. */
  sms_consent_status: string | null;
  /** Timestamp at which the current SMS consent state took effect. */
  sms_consent_at: string | null;
  /** Operational binding through which this consent state was observed. */
  consent_binding: string | null;
  /** TRUE only when the recipient's SMS consent state is Granted. */
  has_sms_consent: boolean | null;
  /** TRUE when a corporate email address is on file for this recipient. */
  is_email_reachable: boolean | null;
  /** TRUE when a mobile number is on file for this recipient. */
  is_sms_reachable: boolean | null;
  /** TRUE when the recipient has neither an email address nor a mobile number -- the exc-unreachable trigger condition. */
  is_unreachable: boolean | null;
  /** TRUE when every channel we hold for this person is either non-consenting or unreachable, so no lawful route exists at all. */
  is_communicationally_stranded: boolean | null;
  /** Extension IRI; recipients are not a PKO 2.0.0 native class. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const recipientsFieldTypes: Record<string, FieldType> = {
  recipient_id: "string",
  name: "*string",
  display_name: "*string",
  organization: "*string",
  email_address: "*string",
  mobile_number: "*string",
  sms_consent_status: "*string",
  sms_consent_at: "*string",
  consent_binding: "*string",
  has_sms_consent: "*bool",
  is_email_reachable: "*bool",
  is_sms_reachable: "*bool",
  is_unreachable: "*bool",
  is_communicationally_stranded: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Recipients row.
 *  Formula: ={{DisplayName}} */
export function calcRecipientsName(tc: RecipientsRow): string | null {
  return toStringPtr(vStr(tc.display_name));
}

/** Computes the HasSmsConsent calculated field.
 *  TRUE only when the recipient's SMS consent state is Granted.
 *  Formula: ={{SmsConsentStatus}} = "Granted" */
export function calcRecipientsHasSmsConsent(tc: RecipientsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.sms_consent_status)), vS("Granted")));
}

/** Computes the IsEmailReachable calculated field.
 *  TRUE when a corporate email address is on file for this recipient.
 *  Formula: ={{EmailAddress}} <> "" */
export function calcRecipientsIsEmailReachable(tc: RecipientsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.email_address)));
}

/** Computes the IsSmsReachable calculated field.
 *  TRUE when a mobile number is on file for this recipient.
 *  Formula: ={{MobileNumber}} <> "" */
export function calcRecipientsIsSmsReachable(tc: RecipientsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.mobile_number)));
}

/** Computes the IsUnreachable calculated field.
 *  TRUE when the recipient has neither an email address nor a mobile number -- the exc-unreachable trigger condition.
 *  Formula: =AND(NOT({{IsEmailReachable}}), NOT({{IsSmsReachable}})) */
export function calcRecipientsIsUnreachable(tc: RecipientsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_email_reachable)))), erbBool3(erbNot(erbBool3(vBool(tc.is_sms_reachable))))));
}

/** Computes the IsCommunicationallyStranded calculated field.
 *  TRUE when every channel we hold for this person is either non-consenting or unreachable, so no lawful route exists at all.
 *  Formula: =AND(NOT({{IsSmsReachable}}), NOT({{IsEmailReachable}})) */
export function calcRecipientsIsCommunicationallyStranded(tc: RecipientsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_sms_reachable)))), erbBool3(erbNot(erbBool3(vBool(tc.is_email_reachable))))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRecipients(tc: RecipientsRow): RecipientsRow {
  // Level 1
  calcGuard(tc, recipientsFieldTypes, "name", () => { tc.name = calcRecipientsName(tc); });
  calcGuard(tc, recipientsFieldTypes, "has_sms_consent", () => { tc.has_sms_consent = calcRecipientsHasSmsConsent(tc); });
  calcGuard(tc, recipientsFieldTypes, "is_email_reachable", () => { tc.is_email_reachable = calcRecipientsIsEmailReachable(tc); });
  calcGuard(tc, recipientsFieldTypes, "is_sms_reachable", () => { tc.is_sms_reachable = calcRecipientsIsSmsReachable(tc); });
  // Level 2
  calcGuard(tc, recipientsFieldTypes, "is_unreachable", () => { tc.is_unreachable = calcRecipientsIsUnreachable(tc); });
  calcGuard(tc, recipientsFieldTypes, "is_communicationally_stranded", () => { tc.is_communicationally_stranded = calcRecipientsIsCommunicationallyStranded(tc); });
  return tc;
}

/** Reads Recipients rows from a JSON array file. */
export function loadRecipientsRows(file: string): RecipientsRow[] {
  return loadRows(file, { fields: recipientsFieldTypes }) as unknown as RecipientsRow[];
}

// =============================================================================
// MESSAGEDELIVERIES TABLE
// MessageDeliveries (added by witness loop 1).
// =============================================================================

/** A row in the MessageDeliveries table. */
export interface MessageDeliveriesRow {
  /** Stored logical identifier for one MessageDeliveries row. */
  message_delivery_id: string;
  /** Human-readable calculated display alias for the MessageDeliveries row. */
  name: string | null;
  /** The procedure execution this send was performed under. */
  procedure_execution: string | null;
  /** The step execution that performed the send. */
  step_execution: string | null;
  /** Who the message was sent to. */
  recipient: string | null;
  /** Template that was rendered for this send. */
  message_template: string | null;
  /** Agent that performed the send. */
  sent_by_agent: string | null;
  /** The exact body text that was transmitted, after variable substitution. */
  rendered_body: string | null;
  /** Transmission timestamp. */
  sent_at: string | null;
  /** Hour of day 0-23 in the RECIPIENT's local timezone at transmission. Stored raw because recipient-local time is what the quiet-hours rule is written against and it is not derivable from SentAt alone. */
  sent_at_local_hour: number | null;
  /** Sent, Delivered, Failed, Suppressed, or Bounced. */
  delivery_status: string | null;
  /** When DeliveryStatus is Suppressed, why; empty string otherwise. */
  suppression_reason: string | null;
  /** Documented exception invoked for this delivery, when one was. */
  invoked_exception: string | null;
  /** When the recipient acknowledged; null when not acknowledged. */
  acknowledged_at: string | null;
  /** The communication policy governing the template that was sent. */
  policy_channel: string | null;
  /** Channel (Email, SMS) of the policy this delivery went out under. */
  channel_name: string | null;
  /** Whether the governing policy requires active consent for this channel. */
  policy_requires_consent: boolean | null;
  /** Whether the recipient of this delivery had SMS consent on file. */
  recipient_has_sms_consent: boolean | null;
  /** TRUE when the message actually left our systems. Suppressed and Failed messages never reached the carrier. */
  was_actually_transmitted: boolean | null;
  /** TRUE when a message was actually transmitted on a consent-required channel to a recipient without consent. This is the TCPA witness. */
  is_consent_violation: boolean | null;
  /** Carries the policy id only on rows that are consent violations; empty string otherwise. */
  consent_violation_policy_key: string | null;
  /** Quiet-hours start hour (0-23) from the governing policy. */
  policy_quiet_hours_start_hour: number | null;
  /** Quiet-hours end hour (0-23) from the governing policy. */
  policy_quiet_hours_end_hour: number | null;
  /** TRUE when the governing policy actually declares a quiet-hours window. comm-email-policy has 00:00-00:00, meaning no window. */
  policy_has_quiet_hours: boolean | null;
  /** TRUE when the quiet window crosses midnight (20:00 -> 08:00 does). */
  quiet_window_wraps_midnight: boolean | null;
  /** TRUE when the recipient-local send hour falls inside the declared quiet window, handling the midnight wrap correctly. */
  is_inside_quiet_window: boolean | null;
  /** TRUE when a message was actually transmitted inside a declared quiet-hours window. The quiet-hours witness. */
  is_quiet_hours_violation: boolean | null;
  /** Carries the policy id only on quiet-hours violations; empty string otherwise. */
  quiet_hours_violation_policy_key: string | null;
  /** Whether the recipient has neither email nor mobile on file -- the exc-unreachable trigger. */
  recipient_is_unreachable: boolean | null;
  /** TRUE when an acknowledgement timestamp is recorded for this delivery. */
  is_acknowledged: boolean | null;
  /** The documented condition of the exception invoked on this delivery, if any. */
  invoked_exception_condition: string | null;
  /** TRUE when the documented unreachable-recipient exception was formally invoked on this delivery. */
  has_unreachable_exception_invoked: boolean | null;
  /** TRUE when an acknowledgement is recorded for a recipient we could not reach. This is the misrepresentation witness. */
  is_fabricated_acknowledgement: boolean | null;
  /** TRUE when a recipient was unreachable but no documented exception was invoked -- the case fell on the floor silently. */
  is_unhandled_unreachable: boolean | null;
  /** Carries the execution id on any unreachable-handling failure; empty string otherwise. */
  unreachable_failure_key: string | null;
  /** Retention commitment in days inherited from the governing channel policy. */
  policy_retention_days: number | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** Days elapsed since transmission. */
  age_days: number | null;
  /** TRUE while this delivery is still inside its committed retention period. */
  is_within_retention_window: boolean | null;
  /** TRUE when the exact transmitted text is still held on this record. */
  has_rendered_body: boolean | null;
  /** TRUE when we are still obliged to hold this message's text. */
  is_evidence_required: boolean | null;
  /** TRUE when we are obliged to hold the message text and do not. The evidentiary-gap witness. */
  is_retention_breach: boolean | null;
  /** Carries the execution id on retention breaches; empty string otherwise. */
  retention_breach_execution_key: string | null;
  /** Which procedure step this delivery was performed under. */
  sending_step_execution_step: string | null;
  /** Whether legal review had passed for the execution this delivery belongs to. */
  execution_has_cleared_legal_review: boolean | null;
  /** TRUE when a message was transmitted under an execution whose legal review had not passed. The ungated-send witness. */
  is_unreviewed_send: boolean | null;
  /** Character length of the exact text that was actually transmitted. */
  rendered_body_length: number | null;
  /** The per-segment limit that governed this specific send. */
  policy_max_message_length_at_send: number | null;
  /** How many channel segments the transmitted text occupied. */
  segment_count: number | null;
  /** The segment ceiling that governed this specific send. */
  policy_max_segments_at_send: number | null;
  /** TRUE when a transmitted message split into more segments than the policy permits. The oversize witness. */
  is_over_segment_limit: boolean | null;
  /** Whether the template used for this delivery had a properly-authorized approval. */
  template_has_valid_approval: boolean | null;
  /** TRUE when a message was transmitted using a template with no valid approval behind it. */
  is_unapproved_send: boolean | null;
  /** The opt-out phrase required for this delivery's channel. */
  policy_required_opt_out_phrase: string | null;
  /** TRUE when the governing channel policy declares a required opt-out phrase at all. */
  policy_requires_opt_out: boolean | null;
  /** Character position at which the required opt-out phrase appears in the transmitted text; 0 when absent. */
  opt_out_phrase_position: number | null;
  /** TRUE when the required opt-out phrase appears anywhere in the transmitted text. */
  has_opt_out_phrase: boolean | null;
  /** TRUE when the opt-out phrase falls inside the first segment, where it is guaranteed to be read. */
  is_opt_out_in_first_segment: boolean | null;
  /** TRUE when a transmitted message on an opt-out-required channel did not contain the phrase at all. The hard witness. */
  is_missing_required_opt_out: boolean | null;
  /** TRUE when the opt-out phrase is present but sits beyond the first segment, where carrier truncation or out-of-order delivery can hide it. The soft witness. */
  is_opt_out_at_risk_of_truncation: boolean | null;
  /** TRUE when the message left our systems but did not land. */
  is_failed_delivery: boolean | null;
  /** TRUE when we deliberately chose not to transmit. */
  is_suppressed: boolean | null;
  /** TRUE when some documented exception was formally invoked on this delivery -- i.e. a human or a rule picked it up. */
  is_triaged: boolean | null;
  /** TRUE when a delivery failed and no documented exception was invoked. The abandoned-bounce witness. */
  is_abandoned_failure: boolean | null;
  /** Carries the execution id on abandoned failures; empty string otherwise. */
  abandoned_failure_execution_key: string | null;
  /** Carries the execution id on confirmed-delivered messages; empty string otherwise. */
  reached_execution_key: string | null;
  /** Whether the template used for this delivery was fully sendable under a current, matching approval. */
  template_was_sendable: boolean | null;
  /** TRUE when a message was transmitted from a template that was not validly sendable at the time. The drift witness. */
  is_drifted_send: boolean | null;
  /** Carries the template id on drifted sends; empty string otherwise. */
  drifted_send_template_key: string | null;
  /** TRUE when this message landed before 08:00 or after 18:00 in the recipient's local time. */
  was_sent_outside_business_hours: boolean | null;
  /** TRUE when a message actually reached someone and no acknowledgement came back. */
  was_delivered_and_unanswered: boolean | null;
  /** TRUE when an unanswered message was delivered outside business hours -- a timing hypothesis for the silence. */
  is_poorly_timed_unanswered: boolean | null;
  /** TRUE when a message was delivered at a reasonable hour and still drew no response. */
  is_well_timed_unanswered: boolean | null;
  /** The template id when this delivery went unanswered, otherwise empty string. */
  unanswered_template_key: string | null;
  /** The template id when this delivery actually reached someone. */
  transmitted_template_key: string | null;
  /** The agent whose approval authorized this specific transmission, frozen at send time. */
  approving_agent_at_send: string | null;
  /** The role that agent held when they approved, frozen at send time. */
  approving_role_at_send: string | null;
  /** When the relied-upon approval was granted, frozen at send time. */
  approval_decided_at_send: string | null;
  /** TRUE when the approval we relied on was granted before the message went out. */
  approval_preceded_send: boolean | null;
  /** TRUE when this delivery carries a complete frozen record of who authorized it and when. */
  has_frozen_approval_evidence: boolean | null;
  /** TRUE when we have no frozen evidence and the only available answer comes from recomputing against today's template state. */
  provenance_is_live_derived: boolean | null;
  /** The template's most recent approval time as it stands NOW, for comparison against what was approved at send. */
  current_last_approval_at: string | null;
  /** TRUE when the template has picked up a newer approval since this message was transmitted. */
  template_reapproved_since_send: boolean | null;
  /** TRUE when we assert this send was approved, hold no frozen evidence, and the template has been re-approved since. The claim cannot be substantiated from the record. */
  is_unprovable_approval_claim: boolean | null;
  /** When I sent a follow-up reminder about this unacknowledged message. Empty when I never did. */
  reminder_sent_at: string | null;
  /** How many reminders I have sent for this delivery. */
  reminder_count: number | null;
  /** TRUE when at least one reminder went out for this delivery. */
  has_sent_reminder: boolean | null;
  /** TRUE when a message actually reached someone, carried an acknowledgement obligation, and has not been acknowledged. */
  acknowledgement_is_outstanding: boolean | null;
  /** How long this acknowledgement has been outstanding. */
  outstanding_age_days: number | null;
  /** TRUE when an acknowledgement has been outstanding for more than 7 days and I have never sent a reminder. */
  is_unchased_acknowledgement: boolean | null;
  /** TRUE when I have sent three or more reminders and still have no acknowledgement -- the point at which this stops being my work and becomes a human escalation. */
  is_exhausted_follow_up: boolean | null;
  /** TRUE when follow-up is exhausted and no exception has been invoked to close out the obligation. */
  needs_human_escalation: boolean | null;
  /** Extension IRI; per-recipient delivery events are not a PKO 2.0.0 native class. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const messageDeliveriesFieldTypes: Record<string, FieldType> = {
  message_delivery_id: "string",
  name: "*string",
  procedure_execution: "*string",
  step_execution: "*string",
  recipient: "*string",
  message_template: "*string",
  sent_by_agent: "*string",
  rendered_body: "*string",
  sent_at: "*string",
  sent_at_local_hour: "*int",
  delivery_status: "*string",
  suppression_reason: "*string",
  invoked_exception: "*string",
  acknowledged_at: "*string",
  policy_channel: "*string",
  channel_name: "*string",
  policy_requires_consent: "*bool",
  recipient_has_sms_consent: "*bool",
  was_actually_transmitted: "*bool",
  is_consent_violation: "*bool",
  consent_violation_policy_key: "*string",
  policy_quiet_hours_start_hour: "*int",
  policy_quiet_hours_end_hour: "*int",
  policy_has_quiet_hours: "*bool",
  quiet_window_wraps_midnight: "*bool",
  is_inside_quiet_window: "*bool",
  is_quiet_hours_violation: "*bool",
  quiet_hours_violation_policy_key: "*string",
  recipient_is_unreachable: "*bool",
  is_acknowledged: "*bool",
  invoked_exception_condition: "*string",
  has_unreachable_exception_invoked: "*bool",
  is_fabricated_acknowledgement: "*bool",
  is_unhandled_unreachable: "*bool",
  unreachable_failure_key: "*string",
  policy_retention_days: "*int",
  evaluation_context: "*string",
  as_of_instant: "*string",
  age_days: "*int",
  is_within_retention_window: "*bool",
  has_rendered_body: "*bool",
  is_evidence_required: "*bool",
  is_retention_breach: "*bool",
  retention_breach_execution_key: "*string",
  sending_step_execution_step: "*string",
  execution_has_cleared_legal_review: "*bool",
  is_unreviewed_send: "*bool",
  rendered_body_length: "*int",
  policy_max_message_length_at_send: "*int",
  segment_count: "*int",
  policy_max_segments_at_send: "*int",
  is_over_segment_limit: "*bool",
  template_has_valid_approval: "*bool",
  is_unapproved_send: "*bool",
  policy_required_opt_out_phrase: "*string",
  policy_requires_opt_out: "*bool",
  opt_out_phrase_position: "*int",
  has_opt_out_phrase: "*bool",
  is_opt_out_in_first_segment: "*bool",
  is_missing_required_opt_out: "*bool",
  is_opt_out_at_risk_of_truncation: "*bool",
  is_failed_delivery: "*bool",
  is_suppressed: "*bool",
  is_triaged: "*bool",
  is_abandoned_failure: "*bool",
  abandoned_failure_execution_key: "*string",
  reached_execution_key: "*string",
  template_was_sendable: "*bool",
  is_drifted_send: "*bool",
  drifted_send_template_key: "*string",
  was_sent_outside_business_hours: "*bool",
  was_delivered_and_unanswered: "*bool",
  is_poorly_timed_unanswered: "*bool",
  is_well_timed_unanswered: "*bool",
  unanswered_template_key: "*string",
  transmitted_template_key: "*string",
  approving_agent_at_send: "*string",
  approving_role_at_send: "*string",
  approval_decided_at_send: "*string",
  approval_preceded_send: "*bool",
  has_frozen_approval_evidence: "*bool",
  provenance_is_live_derived: "*bool",
  current_last_approval_at: "*string",
  template_reapproved_since_send: "*bool",
  is_unprovable_approval_claim: "*bool",
  reminder_sent_at: "*string",
  reminder_count: "*int",
  has_sent_reminder: "*bool",
  acknowledgement_is_outstanding: "*bool",
  outstanding_age_days: "*int",
  is_unchased_acknowledgement: "*bool",
  is_exhausted_follow_up: "*bool",
  needs_human_escalation: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the MessageDeliveries row.
 *  Formula: ={{Recipient}} & " / " & {{MessageTemplate}} & " / " & {{SentAt}} */
export function calcMessageDeliveriesName(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.recipient)), vS(" / "), erbTextOr(vStr(tc.message_template)), vS(" / "), erbTimestamptzText(vStr(tc.sent_at))));
}

/** Computes the WasActuallyTransmitted calculated field.
 *  TRUE when the message actually left our systems. Suppressed and Failed messages never reached the carrier.
 *  Formula: =OR({{DeliveryStatus}} = "Sent", OR({{DeliveryStatus}} = "Delivered", {{DeliveryStatus}} = "Bounced")) */
export function calcMessageDeliveriesWasActuallyTransmitted(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Sent"))), erbBool3(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Delivered"))), erbBool3(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Bounced")))))));
}

/** Computes the IsConsentViolation calculated field.
 *  TRUE when a message was actually transmitted on a consent-required channel to a recipient without consent. This is the TCPA witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresConsent}}, NOT({{RecipientHasSmsConsent}}))) */
export function calcMessageDeliveriesIsConsentViolation(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbAnd(erbBool3(vBool(tc.policy_requires_consent)), erbBool3(erbNot(erbBool3(vBool(tc.recipient_has_sms_consent))))))));
}

/** Computes the ConsentViolationPolicyKey calculated field.
 *  Carries the policy id only on rows that are consent violations; empty string otherwise.
 *  Formula: =IF({{IsConsentViolation}}, {{PolicyChannel}}, "") */
export function calcMessageDeliveriesConsentViolationPolicyKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_consent_violation)), () => vStr(tc.policy_channel), () => vS("")));
}

/** Computes the PolicyHasQuietHours calculated field.
 *  TRUE when the governing policy actually declares a quiet-hours window. comm-email-policy has 00:00-00:00, meaning no window.
 *  Formula: ={{PolicyQuietHoursStartHour}} <> {{PolicyQuietHoursEndHour}} */
export function calcMessageDeliveriesPolicyHasQuietHours(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbNe(vInt(tc.policy_quiet_hours_start_hour), vInt(tc.policy_quiet_hours_end_hour)));
}

/** Computes the QuietWindowWrapsMidnight calculated field.
 *  TRUE when the quiet window crosses midnight (20:00 -> 08:00 does).
 *  Formula: ={{PolicyQuietHoursStartHour}} > {{PolicyQuietHoursEndHour}} */
export function calcMessageDeliveriesQuietWindowWrapsMidnight(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.policy_quiet_hours_start_hour), ">", vInt(tc.policy_quiet_hours_end_hour)));
}

/** Computes the IsInsideQuietWindow calculated field.
 *  TRUE when the recipient-local send hour falls inside the declared quiet window, handling the midnight wrap correctly.
 *  Formula: =IF({{QuietWindowWrapsMidnight}}, OR({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}}), AND({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}})) */
export function calcMessageDeliveriesIsInsideQuietWindow(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(vBool(tc.quiet_window_wraps_midnight)), () => erbOr(erbBool3(erbCmp(erbNullif(vInt(tc.sent_at_local_hour)), ">=", vInt(tc.policy_quiet_hours_start_hour))), erbBool3(erbCmp(erbNullif(vInt(tc.sent_at_local_hour)), "<", vInt(tc.policy_quiet_hours_end_hour)))), () => erbAnd(erbBool3(erbCmp(erbNullif(vInt(tc.sent_at_local_hour)), ">=", vInt(tc.policy_quiet_hours_start_hour))), erbBool3(erbCmp(erbNullif(vInt(tc.sent_at_local_hour)), "<", vInt(tc.policy_quiet_hours_end_hour))))));
}

/** Computes the IsQuietHoursViolation calculated field.
 *  TRUE when a message was actually transmitted inside a declared quiet-hours window. The quiet-hours witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyHasQuietHours}}, {{IsInsideQuietWindow}})) */
export function calcMessageDeliveriesIsQuietHoursViolation(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbAnd(erbBool3(vBool(tc.policy_has_quiet_hours)), erbBool3(vBool(tc.is_inside_quiet_window))))));
}

/** Computes the QuietHoursViolationPolicyKey calculated field.
 *  Carries the policy id only on quiet-hours violations; empty string otherwise.
 *  Formula: =IF({{IsQuietHoursViolation}}, {{PolicyChannel}}, "") */
export function calcMessageDeliveriesQuietHoursViolationPolicyKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_quiet_hours_violation)), () => vStr(tc.policy_channel), () => vS("")));
}

/** Computes the IsAcknowledged calculated field.
 *  TRUE when an acknowledgement timestamp is recorded for this delivery.
 *  Formula: ={{AcknowledgedAt}} <> "" */
export function calcMessageDeliveriesIsAcknowledged(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.acknowledged_at)));
}

/** Computes the HasUnreachableExceptionInvoked calculated field.
 *  TRUE when the documented unreachable-recipient exception was formally invoked on this delivery.
 *  Formula: ={{InvokedException}} = "exc-unreachable" */
export function calcMessageDeliveriesHasUnreachableExceptionInvoked(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.invoked_exception)), vS("exc-unreachable")));
}

/** Computes the IsFabricatedAcknowledgement calculated field.
 *  TRUE when an acknowledgement is recorded for a recipient we could not reach. This is the misrepresentation witness.
 *  Formula: =AND({{RecipientIsUnreachable}}, {{IsAcknowledged}}) */
export function calcMessageDeliveriesIsFabricatedAcknowledgement(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.recipient_is_unreachable)), erbBool3(vBool(tc.is_acknowledged))));
}

/** Computes the IsUnhandledUnreachable calculated field.
 *  TRUE when a recipient was unreachable but no documented exception was invoked -- the case fell on the floor silently.
 *  Formula: =AND({{RecipientIsUnreachable}}, NOT({{HasUnreachableExceptionInvoked}})) */
export function calcMessageDeliveriesIsUnhandledUnreachable(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.recipient_is_unreachable)), erbBool3(erbNot(erbBool3(vBool(tc.has_unreachable_exception_invoked))))));
}

/** Computes the UnreachableFailureKey calculated field.
 *  Carries the execution id on any unreachable-handling failure; empty string otherwise.
 *  Formula: =IF(OR({{IsFabricatedAcknowledgement}}, {{IsUnhandledUnreachable}}), {{ProcedureExecution}}, "") */
export function calcMessageDeliveriesUnreachableFailureKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbOr(erbBool3(vBool(tc.is_fabricated_acknowledgement)), erbBool3(vBool(tc.is_unhandled_unreachable)))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the AgeDays calculated field.
 *  Days elapsed since transmission.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days") */
export function calcMessageDeliveriesAgeDays(tc: MessageDeliveriesRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.sent_at), vS("days"))));
}

/** Computes the IsWithinRetentionWindow calculated field.
 *  TRUE while this delivery is still inside its committed retention period.
 *  Formula: ={{AgeDays}} <= {{PolicyRetentionDays}} */
export function calcMessageDeliveriesIsWithinRetentionWindow(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.age_days), "<=", vInt(tc.policy_retention_days)));
}

/** Computes the HasRenderedBody calculated field.
 *  TRUE when the exact transmitted text is still held on this record.
 *  Formula: ={{RenderedBody}} <> "" */
export function calcMessageDeliveriesHasRenderedBody(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.rendered_body)));
}

/** Computes the IsEvidenceRequired calculated field.
 *  TRUE when we are still obliged to hold this message's text.
 *  Formula: =AND({{WasActuallyTransmitted}}, {{IsWithinRetentionWindow}}) */
export function calcMessageDeliveriesIsEvidenceRequired(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(vBool(tc.is_within_retention_window))));
}

/** Computes the IsRetentionBreach calculated field.
 *  TRUE when we are obliged to hold the message text and do not. The evidentiary-gap witness.
 *  Formula: =AND({{IsEvidenceRequired}}, NOT({{HasRenderedBody}})) */
export function calcMessageDeliveriesIsRetentionBreach(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_evidence_required)), erbBool3(erbNot(erbBool3(vBool(tc.has_rendered_body))))));
}

/** Computes the RetentionBreachExecutionKey calculated field.
 *  Carries the execution id on retention breaches; empty string otherwise.
 *  Formula: =IF({{IsRetentionBreach}}, {{ProcedureExecution}}, "") */
export function calcMessageDeliveriesRetentionBreachExecutionKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_retention_breach)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the IsUnreviewedSend calculated field.
 *  TRUE when a message was transmitted under an execution whose legal review had not passed. The ungated-send witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, NOT({{ExecutionHasClearedLegalReview}})) */
export function calcMessageDeliveriesIsUnreviewedSend(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbNot(erbBool3(vBool(tc.execution_has_cleared_legal_review))))));
}

/** Computes the RenderedBodyLength calculated field.
 *  Character length of the exact text that was actually transmitted.
 *  Formula: =LEN({{RenderedBody}}) */
export function calcMessageDeliveriesRenderedBodyLength(tc: MessageDeliveriesRow): number | null {
  return toIntPtr(erbInteger(erbLen(vStr(tc.rendered_body))));
}

/** Computes the SegmentCount calculated field.
 *  How many channel segments the transmitted text occupied.
 *  Formula: =IF({{RenderedBodyLength}} = 0, 0, IF({{RenderedBodyLength}} <= {{PolicyMaxMessageLengthAtSend}}, 1, ROUNDUP({{RenderedBodyLength}} / {{PolicyMaxMessageLengthAtSend}}, 0))) */
export function calcMessageDeliveriesSegmentCount(tc: MessageDeliveriesRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbEq(vInt(tc.rendered_body_length), vI(0))), () => vI(0), () => erbIf(erbBool3(erbCmp(vInt(tc.rendered_body_length), "<=", vInt(tc.policy_max_message_length_at_send))), () => vI(1), () => erbRoundup(erbDiv(vInt(tc.rendered_body_length), vInt(tc.policy_max_message_length_at_send)), vI(0))))));
}

/** Computes the IsOverSegmentLimit calculated field.
 *  TRUE when a transmitted message split into more segments than the policy permits. The oversize witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, {{SegmentCount}} > {{PolicyMaxSegmentsAtSend}}) */
export function calcMessageDeliveriesIsOverSegmentLimit(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbCmp(vInt(tc.segment_count), ">", vInt(tc.policy_max_segments_at_send)))));
}

/** Computes the IsUnapprovedSend calculated field.
 *  TRUE when a message was transmitted using a template with no valid approval behind it.
 *  Formula: =AND({{WasActuallyTransmitted}}, NOT({{TemplateHasValidApproval}})) */
export function calcMessageDeliveriesIsUnapprovedSend(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbNot(erbBool3(vBool(tc.template_has_valid_approval))))));
}

/** Computes the PolicyRequiresOptOut calculated field.
 *  TRUE when the governing channel policy declares a required opt-out phrase at all.
 *  Formula: ={{PolicyRequiredOptOutPhrase}} <> "" */
export function calcMessageDeliveriesPolicyRequiresOptOut(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.policy_required_opt_out_phrase)));
}

/** Computes the OptOutPhrasePosition calculated field.
 *  Character position at which the required opt-out phrase appears in the transmitted text; 0 when absent.
 *  Formula: =FIND({{PolicyRequiredOptOutPhrase}}, {{RenderedBody}}) */
export function calcMessageDeliveriesOptOutPhrasePosition(tc: MessageDeliveriesRow): number | null {
  return toIntPtr(erbInteger(erbFind(vStr(tc.policy_required_opt_out_phrase), erbNullif(vStr(tc.rendered_body)))));
}

/** Computes the HasOptOutPhrase calculated field.
 *  TRUE when the required opt-out phrase appears anywhere in the transmitted text.
 *  Formula: ={{OptOutPhrasePosition}} > 0 */
export function calcMessageDeliveriesHasOptOutPhrase(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.opt_out_phrase_position), ">", vI(0)));
}

/** Computes the IsOptOutInFirstSegment calculated field.
 *  TRUE when the opt-out phrase falls inside the first segment, where it is guaranteed to be read.
 *  Formula: =AND({{HasOptOutPhrase}}, {{OptOutPhrasePosition}} <= {{PolicyMaxMessageLengthAtSend}}) */
export function calcMessageDeliveriesIsOptOutInFirstSegment(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_opt_out_phrase)), erbBool3(erbCmp(vInt(tc.opt_out_phrase_position), "<=", vInt(tc.policy_max_message_length_at_send)))));
}

/** Computes the IsMissingRequiredOptOut calculated field.
 *  TRUE when a transmitted message on an opt-out-required channel did not contain the phrase at all. The hard witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, NOT({{HasOptOutPhrase}}))) */
export function calcMessageDeliveriesIsMissingRequiredOptOut(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbAnd(erbBool3(vBool(tc.policy_requires_opt_out)), erbBool3(erbNot(erbBool3(vBool(tc.has_opt_out_phrase))))))));
}

/** Computes the IsOptOutAtRiskOfTruncation calculated field.
 *  TRUE when the opt-out phrase is present but sits beyond the first segment, where carrier truncation or out-of-order delivery can hide it. The soft witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, AND({{HasOptOutPhrase}}, NOT({{IsOptOutInFirstSegment}})))) */
export function calcMessageDeliveriesIsOptOutAtRiskOfTruncation(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbAnd(erbBool3(vBool(tc.policy_requires_opt_out)), erbBool3(erbAnd(erbBool3(vBool(tc.has_opt_out_phrase)), erbBool3(erbNot(erbBool3(vBool(tc.is_opt_out_in_first_segment))))))))));
}

/** Computes the IsFailedDelivery calculated field.
 *  TRUE when the message left our systems but did not land.
 *  Formula: =OR({{DeliveryStatus}} = "Failed", {{DeliveryStatus}} = "Bounced") */
export function calcMessageDeliveriesIsFailedDelivery(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Failed"))), erbBool3(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Bounced")))));
}

/** Computes the IsSuppressed calculated field.
 *  TRUE when we deliberately chose not to transmit.
 *  Formula: ={{DeliveryStatus}} = "Suppressed" */
export function calcMessageDeliveriesIsSuppressed(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Suppressed")));
}

/** Computes the IsTriaged calculated field.
 *  TRUE when some documented exception was formally invoked on this delivery -- i.e. a human or a rule picked it up.
 *  Formula: ={{InvokedException}} <> "" */
export function calcMessageDeliveriesIsTriaged(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.invoked_exception)));
}

/** Computes the IsAbandonedFailure calculated field.
 *  TRUE when a delivery failed and no documented exception was invoked. The abandoned-bounce witness.
 *  Formula: =AND({{IsFailedDelivery}}, NOT({{IsTriaged}})) */
export function calcMessageDeliveriesIsAbandonedFailure(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_failed_delivery)), erbBool3(erbNot(erbBool3(vBool(tc.is_triaged))))));
}

/** Computes the AbandonedFailureExecutionKey calculated field.
 *  Carries the execution id on abandoned failures; empty string otherwise.
 *  Formula: =IF({{IsAbandonedFailure}}, {{ProcedureExecution}}, "") */
export function calcMessageDeliveriesAbandonedFailureExecutionKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_abandoned_failure)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the ReachedExecutionKey calculated field.
 *  Carries the execution id on confirmed-delivered messages; empty string otherwise.
 *  Formula: =IF({{DeliveryStatus}} = "Delivered", {{ProcedureExecution}}, "") */
export function calcMessageDeliveriesReachedExecutionKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(erbNullif(vStr(tc.delivery_status)), vS("Delivered"))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the IsDriftedSend calculated field.
 *  TRUE when a message was transmitted from a template that was not validly sendable at the time. The drift witness.
 *  Formula: =AND({{WasActuallyTransmitted}}, NOT({{TemplateWasSendable}})) */
export function calcMessageDeliveriesIsDriftedSend(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbNot(erbBool3(vBool(tc.template_was_sendable))))));
}

/** Computes the DriftedSendTemplateKey calculated field.
 *  Carries the template id on drifted sends; empty string otherwise.
 *  Formula: =IF({{IsDriftedSend}}, {{MessageTemplate}}, "") */
export function calcMessageDeliveriesDriftedSendTemplateKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_drifted_send)), () => vStr(tc.message_template), () => vS("")));
}

/** Computes the WasSentOutsideBusinessHours calculated field.
 *  TRUE when this message landed before 08:00 or after 18:00 in the recipient's local time.
 *  Formula: =OR({{SentAtLocalHour}} < 8, {{SentAtLocalHour}} > 18) */
export function calcMessageDeliveriesWasSentOutsideBusinessHours(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbCmp(erbNullif(vInt(tc.sent_at_local_hour)), "<", vI(8))), erbBool3(erbCmp(erbNullif(vInt(tc.sent_at_local_hour)), ">", vI(18)))));
}

/** Computes the WasDeliveredAndUnanswered calculated field.
 *  TRUE when a message actually reached someone and no acknowledgement came back.
 *  Formula: =AND({{WasActuallyTransmitted}}, NOT({{IsAcknowledged}})) */
export function calcMessageDeliveriesWasDeliveredAndUnanswered(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbNot(erbBool3(vBool(tc.is_acknowledged))))));
}

/** Computes the IsPoorlyTimedUnanswered calculated field.
 *  TRUE when an unanswered message was delivered outside business hours -- a timing hypothesis for the silence.
 *  Formula: =AND({{WasDeliveredAndUnanswered}}, {{WasSentOutsideBusinessHours}}) */
export function calcMessageDeliveriesIsPoorlyTimedUnanswered(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_delivered_and_unanswered)), erbBool3(vBool(tc.was_sent_outside_business_hours))));
}

/** Computes the IsWellTimedUnanswered calculated field.
 *  TRUE when a message was delivered at a reasonable hour and still drew no response.
 *  Formula: =AND({{WasDeliveredAndUnanswered}}, NOT({{WasSentOutsideBusinessHours}})) */
export function calcMessageDeliveriesIsWellTimedUnanswered(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_delivered_and_unanswered)), erbBool3(erbNot(erbBool3(vBool(tc.was_sent_outside_business_hours))))));
}

/** Computes the UnansweredTemplateKey calculated field.
 *  The template id when this delivery went unanswered, otherwise empty string.
 *  Formula: =IF({{WasDeliveredAndUnanswered}}, {{MessageTemplate}}, "") */
export function calcMessageDeliveriesUnansweredTemplateKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_delivered_and_unanswered)), () => vStr(tc.message_template), () => vS("")));
}

/** Computes the TransmittedTemplateKey calculated field.
 *  The template id when this delivery actually reached someone.
 *  Formula: =IF({{WasActuallyTransmitted}}, {{MessageTemplate}}, "") */
export function calcMessageDeliveriesTransmittedTemplateKey(tc: MessageDeliveriesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_actually_transmitted)), () => vStr(tc.message_template), () => vS("")));
}

/** Computes the ApprovalPrecededSend calculated field.
 *  TRUE when the approval we relied on was granted before the message went out.
 *  Formula: =AND({{ApprovalDecidedAtSend}} <> "", {{SentAt}} > {{ApprovalDecidedAtSend}}) */
export function calcMessageDeliveriesApprovalPrecededSend(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.approval_decided_at_send))), erbBool3(erbCmp(erbNullif(vStr(tc.sent_at)), ">", erbNullif(vStr(tc.approval_decided_at_send))))));
}

/** Computes the HasFrozenApprovalEvidence calculated field.
 *  TRUE when this delivery carries a complete frozen record of who authorized it and when.
 *  Formula: =AND({{ApprovingAgentAtSend}} <> "", {{ApprovalDecidedAtSend}} <> "") */
export function calcMessageDeliveriesHasFrozenApprovalEvidence(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.approving_agent_at_send))), erbBool3(erbIsNotBlank(vStr(tc.approval_decided_at_send)))));
}

/** Computes the ProvenanceIsLiveDerived calculated field.
 *  TRUE when we have no frozen evidence and the only available answer comes from recomputing against today's template state.
 *  Formula: =NOT({{HasFrozenApprovalEvidence}}) */
export function calcMessageDeliveriesProvenanceIsLiveDerived(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(vBool(tc.has_frozen_approval_evidence))));
}

/** Computes the TemplateReapprovedSinceSend calculated field.
 *  TRUE when the template has picked up a newer approval since this message was transmitted.
 *  Formula: =AND({{CurrentLastApprovalAt}} <> "", {{CurrentLastApprovalAt}} > {{SentAt}}) */
export function calcMessageDeliveriesTemplateReapprovedSinceSend(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.current_last_approval_at))), erbBool3(erbCmp(vStr(tc.current_last_approval_at), ">", erbNullif(vStr(tc.sent_at))))));
}

/** Computes the IsUnprovableApprovalClaim calculated field.
 *  TRUE when we assert this send was approved, hold no frozen evidence, and the template has been re-approved since. The claim cannot be substantiated from the record.
 *  Formula: =AND({{ProvenanceIsLiveDerived}}, AND({{TemplateReapprovedSinceSend}}, {{TemplateHasValidApproval}})) */
export function calcMessageDeliveriesIsUnprovableApprovalClaim(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.provenance_is_live_derived)), erbBool3(erbAnd(erbBool3(vBool(tc.template_reapproved_since_send)), erbBool3(vBool(tc.template_has_valid_approval))))));
}

/** Computes the HasSentReminder calculated field.
 *  TRUE when at least one reminder went out for this delivery.
 *  Formula: ={{ReminderCount}} > 0 */
export function calcMessageDeliveriesHasSentReminder(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbCmp(erbNullif(vInt(tc.reminder_count)), ">", vI(0)));
}

/** Computes the AcknowledgementIsOutstanding calculated field.
 *  TRUE when a message actually reached someone, carried an acknowledgement obligation, and has not been acknowledged.
 *  Formula: =AND({{WasActuallyTransmitted}}, AND({{IsEvidenceRequired}}, NOT({{IsAcknowledged}}))) */
export function calcMessageDeliveriesAcknowledgementIsOutstanding(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_actually_transmitted)), erbBool3(erbAnd(erbBool3(vBool(tc.is_evidence_required)), erbBool3(erbNot(erbBool3(vBool(tc.is_acknowledged))))))));
}

/** Computes the OutstandingAgeDays calculated field.
 *  How long this acknowledgement has been outstanding.
 *  Formula: =IF({{AcknowledgementIsOutstanding}}, DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days"), 0) */
export function calcMessageDeliveriesOutstandingAgeDays(tc: MessageDeliveriesRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.acknowledgement_is_outstanding)), () => erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.sent_at), vS("days")), () => vI(0))));
}

/** Computes the IsUnchasedAcknowledgement calculated field.
 *  TRUE when an acknowledgement has been outstanding for more than 7 days and I have never sent a reminder.
 *  Formula: =AND({{AcknowledgementIsOutstanding}}, AND({{OutstandingAgeDays}} > 7, NOT({{HasSentReminder}}))) */
export function calcMessageDeliveriesIsUnchasedAcknowledgement(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.acknowledgement_is_outstanding)), erbBool3(erbAnd(erbBool3(erbCmp(vInt(tc.outstanding_age_days), ">", vI(7))), erbBool3(erbNot(erbBool3(vBool(tc.has_sent_reminder))))))));
}

/** Computes the IsExhaustedFollowUp calculated field.
 *  TRUE when I have sent three or more reminders and still have no acknowledgement -- the point at which this stops being my work and becomes a human escalation.
 *  Formula: =AND({{AcknowledgementIsOutstanding}}, {{ReminderCount}} >= 3) */
export function calcMessageDeliveriesIsExhaustedFollowUp(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.acknowledgement_is_outstanding)), erbBool3(erbCmp(erbNullif(vInt(tc.reminder_count)), ">=", vI(3)))));
}

/** Computes the NeedsHumanEscalation calculated field.
 *  TRUE when follow-up is exhausted and no exception has been invoked to close out the obligation.
 *  Formula: =AND({{IsExhaustedFollowUp}}, NOT({{HasUnreachableExceptionInvoked}})) */
export function calcMessageDeliveriesNeedsHumanEscalation(tc: MessageDeliveriesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_exhausted_follow_up)), erbBool3(erbNot(erbBool3(vBool(tc.has_unreachable_exception_invoked))))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeMessageDeliveries(tc: MessageDeliveriesRow): MessageDeliveriesRow {
  // Level 1
  calcGuard(tc, messageDeliveriesFieldTypes, "name", () => { tc.name = calcMessageDeliveriesName(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "was_actually_transmitted", () => { tc.was_actually_transmitted = calcMessageDeliveriesWasActuallyTransmitted(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "policy_has_quiet_hours", () => { tc.policy_has_quiet_hours = calcMessageDeliveriesPolicyHasQuietHours(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "quiet_window_wraps_midnight", () => { tc.quiet_window_wraps_midnight = calcMessageDeliveriesQuietWindowWrapsMidnight(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_acknowledged", () => { tc.is_acknowledged = calcMessageDeliveriesIsAcknowledged(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "has_unreachable_exception_invoked", () => { tc.has_unreachable_exception_invoked = calcMessageDeliveriesHasUnreachableExceptionInvoked(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "age_days", () => { tc.age_days = calcMessageDeliveriesAgeDays(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "has_rendered_body", () => { tc.has_rendered_body = calcMessageDeliveriesHasRenderedBody(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "rendered_body_length", () => { tc.rendered_body_length = calcMessageDeliveriesRenderedBodyLength(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "policy_requires_opt_out", () => { tc.policy_requires_opt_out = calcMessageDeliveriesPolicyRequiresOptOut(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "opt_out_phrase_position", () => { tc.opt_out_phrase_position = calcMessageDeliveriesOptOutPhrasePosition(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_failed_delivery", () => { tc.is_failed_delivery = calcMessageDeliveriesIsFailedDelivery(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_suppressed", () => { tc.is_suppressed = calcMessageDeliveriesIsSuppressed(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_triaged", () => { tc.is_triaged = calcMessageDeliveriesIsTriaged(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "reached_execution_key", () => { tc.reached_execution_key = calcMessageDeliveriesReachedExecutionKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "was_sent_outside_business_hours", () => { tc.was_sent_outside_business_hours = calcMessageDeliveriesWasSentOutsideBusinessHours(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "approval_preceded_send", () => { tc.approval_preceded_send = calcMessageDeliveriesApprovalPrecededSend(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "has_frozen_approval_evidence", () => { tc.has_frozen_approval_evidence = calcMessageDeliveriesHasFrozenApprovalEvidence(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "template_reapproved_since_send", () => { tc.template_reapproved_since_send = calcMessageDeliveriesTemplateReapprovedSinceSend(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "has_sent_reminder", () => { tc.has_sent_reminder = calcMessageDeliveriesHasSentReminder(tc); });
  // Level 2
  calcGuard(tc, messageDeliveriesFieldTypes, "is_consent_violation", () => { tc.is_consent_violation = calcMessageDeliveriesIsConsentViolation(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_inside_quiet_window", () => { tc.is_inside_quiet_window = calcMessageDeliveriesIsInsideQuietWindow(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_fabricated_acknowledgement", () => { tc.is_fabricated_acknowledgement = calcMessageDeliveriesIsFabricatedAcknowledgement(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_unhandled_unreachable", () => { tc.is_unhandled_unreachable = calcMessageDeliveriesIsUnhandledUnreachable(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_within_retention_window", () => { tc.is_within_retention_window = calcMessageDeliveriesIsWithinRetentionWindow(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_unreviewed_send", () => { tc.is_unreviewed_send = calcMessageDeliveriesIsUnreviewedSend(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "segment_count", () => { tc.segment_count = calcMessageDeliveriesSegmentCount(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_unapproved_send", () => { tc.is_unapproved_send = calcMessageDeliveriesIsUnapprovedSend(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "has_opt_out_phrase", () => { tc.has_opt_out_phrase = calcMessageDeliveriesHasOptOutPhrase(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_abandoned_failure", () => { tc.is_abandoned_failure = calcMessageDeliveriesIsAbandonedFailure(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_drifted_send", () => { tc.is_drifted_send = calcMessageDeliveriesIsDriftedSend(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "was_delivered_and_unanswered", () => { tc.was_delivered_and_unanswered = calcMessageDeliveriesWasDeliveredAndUnanswered(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "transmitted_template_key", () => { tc.transmitted_template_key = calcMessageDeliveriesTransmittedTemplateKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "provenance_is_live_derived", () => { tc.provenance_is_live_derived = calcMessageDeliveriesProvenanceIsLiveDerived(tc); });
  // Level 3
  calcGuard(tc, messageDeliveriesFieldTypes, "consent_violation_policy_key", () => { tc.consent_violation_policy_key = calcMessageDeliveriesConsentViolationPolicyKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_quiet_hours_violation", () => { tc.is_quiet_hours_violation = calcMessageDeliveriesIsQuietHoursViolation(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "unreachable_failure_key", () => { tc.unreachable_failure_key = calcMessageDeliveriesUnreachableFailureKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_evidence_required", () => { tc.is_evidence_required = calcMessageDeliveriesIsEvidenceRequired(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_over_segment_limit", () => { tc.is_over_segment_limit = calcMessageDeliveriesIsOverSegmentLimit(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_opt_out_in_first_segment", () => { tc.is_opt_out_in_first_segment = calcMessageDeliveriesIsOptOutInFirstSegment(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_missing_required_opt_out", () => { tc.is_missing_required_opt_out = calcMessageDeliveriesIsMissingRequiredOptOut(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "abandoned_failure_execution_key", () => { tc.abandoned_failure_execution_key = calcMessageDeliveriesAbandonedFailureExecutionKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "drifted_send_template_key", () => { tc.drifted_send_template_key = calcMessageDeliveriesDriftedSendTemplateKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_poorly_timed_unanswered", () => { tc.is_poorly_timed_unanswered = calcMessageDeliveriesIsPoorlyTimedUnanswered(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_well_timed_unanswered", () => { tc.is_well_timed_unanswered = calcMessageDeliveriesIsWellTimedUnanswered(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "unanswered_template_key", () => { tc.unanswered_template_key = calcMessageDeliveriesUnansweredTemplateKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_unprovable_approval_claim", () => { tc.is_unprovable_approval_claim = calcMessageDeliveriesIsUnprovableApprovalClaim(tc); });
  // Level 4
  calcGuard(tc, messageDeliveriesFieldTypes, "quiet_hours_violation_policy_key", () => { tc.quiet_hours_violation_policy_key = calcMessageDeliveriesQuietHoursViolationPolicyKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_retention_breach", () => { tc.is_retention_breach = calcMessageDeliveriesIsRetentionBreach(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_opt_out_at_risk_of_truncation", () => { tc.is_opt_out_at_risk_of_truncation = calcMessageDeliveriesIsOptOutAtRiskOfTruncation(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "acknowledgement_is_outstanding", () => { tc.acknowledgement_is_outstanding = calcMessageDeliveriesAcknowledgementIsOutstanding(tc); });
  // Level 5
  calcGuard(tc, messageDeliveriesFieldTypes, "retention_breach_execution_key", () => { tc.retention_breach_execution_key = calcMessageDeliveriesRetentionBreachExecutionKey(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "outstanding_age_days", () => { tc.outstanding_age_days = calcMessageDeliveriesOutstandingAgeDays(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "is_exhausted_follow_up", () => { tc.is_exhausted_follow_up = calcMessageDeliveriesIsExhaustedFollowUp(tc); });
  // Level 6
  calcGuard(tc, messageDeliveriesFieldTypes, "is_unchased_acknowledgement", () => { tc.is_unchased_acknowledgement = calcMessageDeliveriesIsUnchasedAcknowledgement(tc); });
  calcGuard(tc, messageDeliveriesFieldTypes, "needs_human_escalation", () => { tc.needs_human_escalation = calcMessageDeliveriesNeedsHumanEscalation(tc); });
  return tc;
}

/** Reads MessageDeliveries rows from a JSON array file. */
export function loadMessageDeliveriesRows(file: string): MessageDeliveriesRow[] {
  return loadRows(file, { fields: messageDeliveriesFieldTypes }) as unknown as MessageDeliveriesRow[];
}

// =============================================================================
// TEMPLATEAPPROVALS TABLE
// TemplateApprovals (added by witness loop 1).
// =============================================================================

/** A row in the TemplateApprovals table. */
export interface TemplateApprovalsRow {
  /** Stored logical identifier for one TemplateApprovals row. */
  template_approval_id: string;
  /** Human-readable calculated display alias for the TemplateApprovals row. */
  name: string | null;
  /** Template this decision applies to. */
  message_template: string | null;
  /** Agent who made the approval decision. */
  decided_by_agent: string | null;
  /** Role the agent was acting in when deciding. */
  decided_in_role: string | null;
  /** Approved, Rejected, or Withdrawn. */
  decision: string | null;
  /** When the decision was recorded. */
  decided_at: string | null;
  /** Digest of the exact template body that was approved, so later edits are detectable. */
  approved_body_hash: string | null;
  /** Approver's rationale or conditions. */
  notes: string | null;
  /** TRUE when this decision row is an approval rather than a rejection or withdrawal. */
  is_approval_decision: boolean | null;
  /** The communication policy governing the template being approved. */
  template_policy: string | null;
  /** The role the governing policy designates as approver -- communications-manager for both current policies. */
  required_approval_role: string | null;
  /** TRUE when the approving role matches the role the policy designates. */
  is_decided_by_required_role: boolean | null;
  /** Carries the template id only on approvals made by the correct role; empty string otherwise. */
  valid_approval_template_key: string | null;
  /** Extension IRI; template approval events are not a PKO 2.0.0 native class. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const templateApprovalsFieldTypes: Record<string, FieldType> = {
  template_approval_id: "string",
  name: "*string",
  message_template: "*string",
  decided_by_agent: "*string",
  decided_in_role: "*string",
  decision: "*string",
  decided_at: "*string",
  approved_body_hash: "*string",
  notes: "*string",
  is_approval_decision: "*bool",
  template_policy: "*string",
  required_approval_role: "*string",
  is_decided_by_required_role: "*bool",
  valid_approval_template_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the TemplateApprovals row.
 *  Formula: ={{MessageTemplate}} & " / " & {{Decision}} & " / " & {{DecidedAt}} */
export function calcTemplateApprovalsName(tc: TemplateApprovalsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.message_template)), vS(" / "), erbTextOr(vStr(tc.decision)), vS(" / "), erbTimestamptzText(vStr(tc.decided_at))));
}

/** Computes the IsApprovalDecision calculated field.
 *  TRUE when this decision row is an approval rather than a rejection or withdrawal.
 *  Formula: ={{Decision}} = "Approved" */
export function calcTemplateApprovalsIsApprovalDecision(tc: TemplateApprovalsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.decision)), vS("Approved")));
}

/** Computes the IsDecidedByRequiredRole calculated field.
 *  TRUE when the approving role matches the role the policy designates.
 *  Formula: ={{DecidedInRole}} = {{RequiredApprovalRole}} */
export function calcTemplateApprovalsIsDecidedByRequiredRole(tc: TemplateApprovalsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.decided_in_role)), vStr(tc.required_approval_role)));
}

/** Computes the ValidApprovalTemplateKey calculated field.
 *  Carries the template id only on approvals made by the correct role; empty string otherwise.
 *  Formula: =IF(AND({{IsApprovalDecision}}, {{IsDecidedByRequiredRole}}), {{MessageTemplate}}, "") */
export function calcTemplateApprovalsValidApprovalTemplateKey(tc: TemplateApprovalsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.is_approval_decision)), erbBool3(vBool(tc.is_decided_by_required_role)))), () => vStr(tc.message_template), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeTemplateApprovals(tc: TemplateApprovalsRow): TemplateApprovalsRow {
  // Level 1
  calcGuard(tc, templateApprovalsFieldTypes, "name", () => { tc.name = calcTemplateApprovalsName(tc); });
  calcGuard(tc, templateApprovalsFieldTypes, "is_approval_decision", () => { tc.is_approval_decision = calcTemplateApprovalsIsApprovalDecision(tc); });
  calcGuard(tc, templateApprovalsFieldTypes, "is_decided_by_required_role", () => { tc.is_decided_by_required_role = calcTemplateApprovalsIsDecidedByRequiredRole(tc); });
  // Level 2
  calcGuard(tc, templateApprovalsFieldTypes, "valid_approval_template_key", () => { tc.valid_approval_template_key = calcTemplateApprovalsValidApprovalTemplateKey(tc); });
  return tc;
}

/** Reads TemplateApprovals rows from a JSON array file. */
export function loadTemplateApprovalsRows(file: string): TemplateApprovalsRow[] {
  return loadRows(file, { fields: templateApprovalsFieldTypes }) as unknown as TemplateApprovalsRow[];
}

// =============================================================================
// SENDINTENTS TABLE
// SendIntents (added by witness loop 1).
// =============================================================================

/** A row in the SendIntents table. */
export interface SendIntentsRow {
  /** Stored logical identifier for one SendIntents row. */
  send_intent_id: string;
  /** Human-readable calculated display alias for the SendIntents row. */
  name: string | null;
  /** Execution this send intent belongs to. */
  procedure_execution: string | null;
  /** Step execution evaluating this intent. */
  step_execution: string | null;
  /** Intended recipient. */
  recipient: string | null;
  /** Template the pipeline intends to render and send. */
  message_template: string | null;
  /** The fully rendered text the pipeline intends to transmit, before transmission. */
  proposed_body: string | null;
  /** Hour 0-23 in the recipient's local timezone at which the pipeline intends to transmit. */
  proposed_send_at_local_hour: number | null;
  /** When the pipeline evaluated the gates for this intent. */
  evaluated_at: string | null;
  /** The delivery record produced from this intent, whether transmitted or suppressed. */
  resulting_delivery: string | null;
  /** The communication policy governing the template this intent would send. */
  intent_policy: string | null;
  /** Channel this intent would transmit on. */
  intent_channel: string | null;
  /** Whether the governing policy is currently Active rather than Draft, Suspended, or Retired. */
  policy_is_active: boolean | null;
  /** Whether the governing policy requires consent for this channel. */
  intent_requires_consent: boolean | null;
  /** Whether the intended recipient has active SMS consent on record. */
  recipient_has_channel_consent: boolean | null;
  /** TRUE when either the channel does not require consent, or the recipient has granted it. */
  consent_gate_passed: boolean | null;
  /** Whether a mobile number is on file for the intended recipient. */
  recipient_is_sms_reachable: boolean | null;
  /** Whether an email address is on file for the intended recipient. */
  recipient_is_email_reachable: boolean | null;
  /** TRUE when the recipient is reachable on the channel this intent would use. */
  reachability_gate_passed: boolean | null;
  /** TRUE when the policy is active, consent is satisfied, and the recipient is reachable on this channel. The channel-permission gate. */
  permission_gate_passed: boolean | null;
  /** Quiet-hours start hour from the governing policy. */
  intent_quiet_start_hour: number | null;
  /** Quiet-hours end hour from the governing policy. */
  intent_quiet_end_hour: number | null;
  /** TRUE when the governing policy declares a real quiet-hours window. */
  intent_policy_has_quiet_hours: boolean | null;
  /** TRUE when the quiet window crosses midnight. */
  intent_quiet_window_wraps: boolean | null;
  /** TRUE when the proposed recipient-local send hour falls inside the forbidden window. */
  intent_is_inside_quiet_window: boolean | null;
  /** TRUE when transmitting now is permitted on timing grounds. The timing gate. */
  timing_gate_passed: boolean | null;
  /** How many hours the pipeline must defer before transmission becomes permitted; 0 when already permitted. */
  hours_until_window_opens: number | null;
  /** Per-segment character limit from the governing policy. */
  intent_max_message_length: number | null;
  /** Segment ceiling from the governing policy. */
  intent_max_segments: number | null;
  /** Character count of ProposedBody, written by the pipeline at render time. */
  proposed_body_length: number | null;
  /** Number of channel segments ProposedBody will occupy, computed by the pipeline at render time. */
  proposed_segment_count: number | null;
  /** TRUE when the proposed text is non-empty and fits within the segment ceiling. */
  length_gate_passed: boolean | null;
  /** Opt-out phrase the governing policy requires in every message on this channel. */
  intent_required_opt_out_phrase: string | null;
  /** Character position of the required opt-out phrase within ProposedBody; 0 when absent. Written by the pipeline at render time. */
  proposed_opt_out_position: number | null;
  /** TRUE when no opt-out is required, or the required phrase is present within the first segment. */
  opt_out_gate_passed: boolean | null;
  /** TRUE when the proposed text satisfies every content rule of the governing channel policy. */
  content_gate_passed: boolean | null;
  /** Whether the template carries a current, properly-authorized, non-drifted approval. */
  template_is_sendable: boolean | null;
  /** Whether legal review has passed for the execution this intent belongs to. */
  execution_has_legal_clearance: boolean | null;
  /** The role the governing policy designates as the approving authority. */
  intent_approval_role: string | null;
  /** Whether the agent currently filling the designated approval role is Human, AIAgent, or AutomatedPipeline. */
  approval_role_agent_kind: string | null;
  /** TRUE when the designated approval role is currently held by a human agent. */
  approval_is_human: boolean | null;
  /** TRUE only when the template is validly approved, legal review has cleared, and the approving role is held by a human. The authorization gate. */
  authorization_gate_passed: boolean | null;
  /** TRUE only when all four gates pass. THE single column the pipeline reads before transmitting. */
  is_cleared_to_send: boolean | null;
  /** Names the first gate that refused, for the suppression reason; empty string when cleared. */
  blocking_gate_name: string | null;
  /** TRUE when this intent produced a delivery record of any status. */
  has_resulting_delivery: boolean | null;
  /** Whether the delivery produced from this intent actually left our systems. */
  resulting_delivery_was_transmitted: boolean | null;
  /** TRUE when a gate refused and the message was transmitted regardless. The override witness. */
  is_overridden_refusal: boolean | null;
  /** TRUE when a gate refused and no delivery record of any kind was produced -- the recipient vanished from the run. */
  is_silently_dropped: boolean | null;
  /** The documented exception invoked on the delivery produced from this intent, if any. */
  resulting_delivery_exception: string | null;
  /** TRUE when the suppression produced by this refusal cited a documented exception. */
  refusal_cited_an_exception: boolean | null;
  /** TRUE when a refusal correctly resulted in a non-transmitted delivery record citing a documented exception. The positive witness. */
  is_properly_handled_refusal: boolean | null;
  /** Carries the execution id on any mishandled refusal; empty string otherwise. */
  refusal_failure_execution_key: string | null;
  /** The procedure execution id for every intent, used as the campaign rollup key. */
  intent_execution_key: string | null;
  /** The execution id when this intent actually reached a person, otherwise empty string. */
  delivered_intent_execution_key: string | null;
  /** The execution id when this intent was refused and left no record at all. */
  dropped_intent_execution_key: string | null;
  /** TRUE when the template carried a valid approval at the moment this intent was evaluated. */
  my_approval_was_in_force: boolean | null;
  /** TRUE when the pipeline refused an intent on content grounds even though the template was approved. */
  refused_on_approved_content: boolean | null;
  /** TRUE when the only content failure was a missing or mispositioned opt-out phrase. */
  refused_on_opt_out_only: boolean | null;
  /** TRUE when the refusal came from a communications rule I own -- content, length, opt-out, quiet hours. */
  refusal_was_on_my_rules: boolean | null;
  /** TRUE when the refusal came from consent, reachability, or authorization -- none of which I can fix by editing a template. */
  refusal_was_outside_my_control: boolean | null;
  /** Whether the approving human was informed that this intent was refused. */
  approver_was_notified: boolean | null;
  /** TRUE when a refusal I own and could have fixed was never surfaced to me. */
  is_unreported_refusal_on_my_rules: boolean | null;
  /** TRUE when the pipeline overrode a valid human approval on content grounds and told nobody. */
  is_approval_overridden_silently: boolean | null;
  /** The follow-up intent raised on a different channel after this one was refused. Stored as a raw identifier rather than an FK: SendIntents pointing at SendIntents would make the table self-referential in a way the DAG contract does not allow here. */
  alternate_channel_intent: string | null;
  /** TRUE when a follow-up intent on another channel was raised for this refused send. */
  has_alternate_channel_attempt: boolean | null;
  /** Whether the follow-up intent itself passed all gates. */
  alternate_attempt_was_cleared: boolean | null;
  /** TRUE when a send was refused and no attempt was ever made on any other channel. */
  is_refused_with_no_alternative: boolean | null;
  /** TRUE when the documented exception for this refusal prescribes a different-channel send as the correct handling. */
  exception_prescribed_an_alternative: boolean | null;
  /** TRUE when the exception prescribed an alternate channel and an alternate intent was actually raised and cleared. */
  prescribed_handling_was_performed: boolean | null;
  /** TRUE when we cited an exception that prescribed an alternate channel and then never performed it. */
  is_suppression_without_remedy: boolean | null;
  /** When this refusal was written to a durable record. Empty when no record was ever emitted. */
  refusal_recorded_at: string | null;
  /** The role informed that this send was refused. */
  refusal_notified_role: string | null;
  /** TRUE when this refusal was written down somewhere a human can find it. */
  has_durable_refusal_record: boolean | null;
  /** TRUE when a specific role was notified of this refusal. */
  refusal_was_escalated: boolean | null;
  /** TRUE when I refused a send and produced no delivery record, no refusal record, and no exception. The refusal left no trace of any kind. */
  is_unrecorded_refusal: boolean | null;
  /** TRUE when a refusal was recorded but no human role was ever told. */
  is_unescalated_refusal: boolean | null;
  /** Echoes the role that should have been told about this refusal, but only when the refusal went unrecorded. Empty otherwise. */
  unescalated_refusal_role_key: string | null;
  /** Echoes the parent procedure execution only for refusals nobody recorded; empty otherwise. */
  unrecorded_refusal_execution_key: string | null;
  /** The intent I raised when the quiet window reopened for this deferred send. Raw identifier rather than an FK: a SendIntents self-reference is not expressible as a relationship without making the table depend on itself. */
  retry_intent: string | null;
  /** TRUE when the timing gate is the reason this intent did not clear, and every other gate passed. */
  was_deferred_on_timing: boolean | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged against. */
  as_of_instant: string | null;
  /** TRUE when enough time has passed since evaluation that the quiet window this intent hit must have closed. */
  window_has_since_reopened: boolean | null;
  /** TRUE when a retry intent was raised for this deferred send. */
  has_retry_attempt: boolean | null;
  /** Whether the retry intent itself passed all gates. */
  retry_was_cleared: boolean | null;
  /** TRUE when a send was deferred for timing, the window has since reopened, and no retry was ever raised. A deferral silently converted into a cancellation. */
  is_abandoned_deferral: boolean | null;
  /** How long this deferred intent has been sitting since evaluation. */
  deferral_age_hours: number | null;
  /** TRUE when a deferred send has been waiting more than 24 hours -- longer than any quiet window can justify. */
  is_stale_deferral: boolean | null;
  /** The RoleAssignments id under which the pipeline evaluated this send intent. Raw rather than an FK because SendIntents already sits downstream of the execution graph and an added edge to RoleAssignments is not needed to resolve it. */
  evaluating_role_assignment: string | null;
  /** Whether the gate decision on this intent was made by an agent holding an unauthorized enforcement assignment. */
  enforced_by_unauthorized_agent: boolean | null;
  /** TRUE when the recipient's consent state was actually retrievable, as opposed to absent and read as a refusal. */
  consent_input_was_resolvable: boolean | null;
  /** The recipient's consent status as a string: Granted, Revoked, NeverGiven, or empty if no consent record exists at all. */
  recipient_consent_status_raw: string | null;
  /** TRUE when a governing communication policy was actually found for this intent. */
  policy_input_was_resolvable: boolean | null;
  /** TRUE when every input my gates depend on was actually retrievable. */
  all_gate_inputs_resolved: boolean | null;
  /** TRUE when I refused a send but at least one gate input could not be resolved -- so I do not actually know whether the rule was violated or merely unreadable. */
  is_unevaluable_refusal: boolean | null;
  /** Whether any agent other than me verified this gate outcome. */
  gate_result_was_independently_confirmed: boolean | null;
  /** TRUE when the entire decision to send or refuse rests solely on my own computation, unconfirmed by anything else. */
  is_self_witnessed_decision: boolean | null;
  /** TRUE when this intent's decision was corroborated by an actual delivery record rather than resting solely on the pipeline's own say-so. */
  is_independently_confirmed: boolean | null;
  /** Echoes the parent execution only for independently confirmed intents; empty otherwise. */
  independently_confirmed_execution_key: string | null;
  /** Extension IRI; pre-send intents are not a PKO 2.0.0 native class. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const sendIntentsFieldTypes: Record<string, FieldType> = {
  send_intent_id: "string",
  name: "*string",
  procedure_execution: "*string",
  step_execution: "*string",
  recipient: "*string",
  message_template: "*string",
  proposed_body: "*string",
  proposed_send_at_local_hour: "*int",
  evaluated_at: "*string",
  resulting_delivery: "*string",
  intent_policy: "*string",
  intent_channel: "*string",
  policy_is_active: "*bool",
  intent_requires_consent: "*bool",
  recipient_has_channel_consent: "*bool",
  consent_gate_passed: "*bool",
  recipient_is_sms_reachable: "*bool",
  recipient_is_email_reachable: "*bool",
  reachability_gate_passed: "*bool",
  permission_gate_passed: "*bool",
  intent_quiet_start_hour: "*int",
  intent_quiet_end_hour: "*int",
  intent_policy_has_quiet_hours: "*bool",
  intent_quiet_window_wraps: "*bool",
  intent_is_inside_quiet_window: "*bool",
  timing_gate_passed: "*bool",
  hours_until_window_opens: "*int",
  intent_max_message_length: "*int",
  intent_max_segments: "*int",
  proposed_body_length: "*int",
  proposed_segment_count: "*int",
  length_gate_passed: "*bool",
  intent_required_opt_out_phrase: "*string",
  proposed_opt_out_position: "*int",
  opt_out_gate_passed: "*bool",
  content_gate_passed: "*bool",
  template_is_sendable: "*bool",
  execution_has_legal_clearance: "*bool",
  intent_approval_role: "*string",
  approval_role_agent_kind: "*string",
  approval_is_human: "*bool",
  authorization_gate_passed: "*bool",
  is_cleared_to_send: "*bool",
  blocking_gate_name: "*string",
  has_resulting_delivery: "*bool",
  resulting_delivery_was_transmitted: "*bool",
  is_overridden_refusal: "*bool",
  is_silently_dropped: "*bool",
  resulting_delivery_exception: "*string",
  refusal_cited_an_exception: "*bool",
  is_properly_handled_refusal: "*bool",
  refusal_failure_execution_key: "*string",
  intent_execution_key: "*string",
  delivered_intent_execution_key: "*string",
  dropped_intent_execution_key: "*string",
  my_approval_was_in_force: "*bool",
  refused_on_approved_content: "*bool",
  refused_on_opt_out_only: "*bool",
  refusal_was_on_my_rules: "*bool",
  refusal_was_outside_my_control: "*bool",
  approver_was_notified: "*bool",
  is_unreported_refusal_on_my_rules: "*bool",
  is_approval_overridden_silently: "*bool",
  alternate_channel_intent: "*string",
  has_alternate_channel_attempt: "*bool",
  alternate_attempt_was_cleared: "*bool",
  is_refused_with_no_alternative: "*bool",
  exception_prescribed_an_alternative: "*bool",
  prescribed_handling_was_performed: "*bool",
  is_suppression_without_remedy: "*bool",
  refusal_recorded_at: "*string",
  refusal_notified_role: "*string",
  has_durable_refusal_record: "*bool",
  refusal_was_escalated: "*bool",
  is_unrecorded_refusal: "*bool",
  is_unescalated_refusal: "*bool",
  unescalated_refusal_role_key: "*string",
  unrecorded_refusal_execution_key: "*string",
  retry_intent: "*string",
  was_deferred_on_timing: "*bool",
  evaluation_context: "*string",
  as_of_instant: "*string",
  window_has_since_reopened: "*bool",
  has_retry_attempt: "*bool",
  retry_was_cleared: "*bool",
  is_abandoned_deferral: "*bool",
  deferral_age_hours: "*int",
  is_stale_deferral: "*bool",
  evaluating_role_assignment: "*string",
  enforced_by_unauthorized_agent: "*bool",
  consent_input_was_resolvable: "*bool",
  recipient_consent_status_raw: "*string",
  policy_input_was_resolvable: "*bool",
  all_gate_inputs_resolved: "*bool",
  is_unevaluable_refusal: "*bool",
  gate_result_was_independently_confirmed: "*bool",
  is_self_witnessed_decision: "*bool",
  is_independently_confirmed: "*bool",
  independently_confirmed_execution_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the SendIntents row.
 *  Formula: ={{Recipient}} & " / " & {{MessageTemplate}} & " / intent" */
export function calcSendIntentsName(tc: SendIntentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.recipient)), vS(" / "), erbTextOr(vStr(tc.message_template)), vS(" / intent")));
}

/** Computes the ConsentGatePassed calculated field.
 *  TRUE when either the channel does not require consent, or the recipient has granted it.
 *  Formula: =OR(NOT({{IntentRequiresConsent}}), {{RecipientHasChannelConsent}}) */
export function calcSendIntentsConsentGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.intent_requires_consent)))), erbBool3(vBool(tc.recipient_has_channel_consent))));
}

/** Computes the ReachabilityGatePassed calculated field.
 *  TRUE when the recipient is reachable on the channel this intent would use.
 *  Formula: =IF({{IntentChannel}} = "SMS", {{RecipientIsSmsReachable}}, {{RecipientIsEmailReachable}}) */
export function calcSendIntentsReachabilityGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbEq(vStr(tc.intent_channel), vS("SMS"))), () => vBool(tc.recipient_is_sms_reachable), () => vBool(tc.recipient_is_email_reachable)));
}

/** Computes the PermissionGatePassed calculated field.
 *  TRUE when the policy is active, consent is satisfied, and the recipient is reachable on this channel. The channel-permission gate.
 *  Formula: =AND({{PolicyIsActive}}, AND({{ConsentGatePassed}}, {{ReachabilityGatePassed}})) */
export function calcSendIntentsPermissionGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.policy_is_active)), erbBool3(erbAnd(erbBool3(vBool(tc.consent_gate_passed)), erbBool3(vBool(tc.reachability_gate_passed))))));
}

/** Computes the IntentPolicyHasQuietHours calculated field.
 *  TRUE when the governing policy declares a real quiet-hours window.
 *  Formula: ={{IntentQuietStartHour}} <> {{IntentQuietEndHour}} */
export function calcSendIntentsIntentPolicyHasQuietHours(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbNe(vInt(tc.intent_quiet_start_hour), vInt(tc.intent_quiet_end_hour)));
}

/** Computes the IntentQuietWindowWraps calculated field.
 *  TRUE when the quiet window crosses midnight.
 *  Formula: ={{IntentQuietStartHour}} > {{IntentQuietEndHour}} */
export function calcSendIntentsIntentQuietWindowWraps(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.intent_quiet_start_hour), ">", vInt(tc.intent_quiet_end_hour)));
}

/** Computes the IntentIsInsideQuietWindow calculated field.
 *  TRUE when the proposed recipient-local send hour falls inside the forbidden window.
 *  Formula: =IF({{IntentQuietWindowWraps}}, OR({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}), AND({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}})) */
export function calcSendIntentsIntentIsInsideQuietWindow(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(vBool(tc.intent_quiet_window_wraps)), () => erbOr(erbBool3(erbCmp(erbNullif(vInt(tc.proposed_send_at_local_hour)), ">=", vInt(tc.intent_quiet_start_hour))), erbBool3(erbCmp(erbNullif(vInt(tc.proposed_send_at_local_hour)), "<", vInt(tc.intent_quiet_end_hour)))), () => erbAnd(erbBool3(erbCmp(erbNullif(vInt(tc.proposed_send_at_local_hour)), ">=", vInt(tc.intent_quiet_start_hour))), erbBool3(erbCmp(erbNullif(vInt(tc.proposed_send_at_local_hour)), "<", vInt(tc.intent_quiet_end_hour))))));
}

/** Computes the TimingGatePassed calculated field.
 *  TRUE when transmitting now is permitted on timing grounds. The timing gate.
 *  Formula: =OR(NOT({{IntentPolicyHasQuietHours}}), NOT({{IntentIsInsideQuietWindow}})) */
export function calcSendIntentsTimingGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.intent_policy_has_quiet_hours)))), erbBool3(erbNot(erbBool3(vBool(tc.intent_is_inside_quiet_window))))));
}

/** Computes the HoursUntilWindowOpens calculated field.
 *  How many hours the pipeline must defer before transmission becomes permitted; 0 when already permitted.
 *  Formula: =IF({{TimingGatePassed}}, 0, IF({{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}, {{IntentQuietEndHour}} - {{ProposedSendAtLocalHour}}, 24 - {{ProposedSendAtLocalHour}} + {{IntentQuietEndHour}})) */
export function calcSendIntentsHoursUntilWindowOpens(tc: SendIntentsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.timing_gate_passed)), () => vI(0), () => erbIf(erbBool3(erbCmp(erbNullif(vInt(tc.proposed_send_at_local_hour)), "<", vInt(tc.intent_quiet_end_hour))), () => erbSub(vInt(tc.intent_quiet_end_hour), vInt(tc.proposed_send_at_local_hour)), () => erbAdd(erbSub(vI(24), vInt(tc.proposed_send_at_local_hour)), vInt(tc.intent_quiet_end_hour))))));
}

/** Computes the LengthGatePassed calculated field.
 *  TRUE when the proposed text is non-empty and fits within the segment ceiling.
 *  Formula: =AND({{ProposedBodyLength}} > 0, {{ProposedSegmentCount}} <= {{IntentMaxSegments}}) */
export function calcSendIntentsLengthGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(erbNullif(vInt(tc.proposed_body_length)), ">", vI(0))), erbBool3(erbCmp(erbNullif(vInt(tc.proposed_segment_count)), "<=", vInt(tc.intent_max_segments)))));
}

/** Computes the OptOutGatePassed calculated field.
 *  TRUE when no opt-out is required, or the required phrase is present within the first segment.
 *  Formula: =OR({{IntentRequiredOptOutPhrase}} = "", AND({{ProposedOptOutPosition}} > 0, {{ProposedOptOutPosition}} <= {{IntentMaxMessageLength}})) */
export function calcSendIntentsOptOutGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbIsBlank(vStr(tc.intent_required_opt_out_phrase))), erbBool3(erbAnd(erbBool3(erbCmp(erbNullif(vInt(tc.proposed_opt_out_position)), ">", vI(0))), erbBool3(erbCmp(erbNullif(vInt(tc.proposed_opt_out_position)), "<=", vInt(tc.intent_max_message_length)))))));
}

/** Computes the ContentGatePassed calculated field.
 *  TRUE when the proposed text satisfies every content rule of the governing channel policy.
 *  Formula: =AND({{LengthGatePassed}}, {{OptOutGatePassed}}) */
export function calcSendIntentsContentGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.length_gate_passed)), erbBool3(vBool(tc.opt_out_gate_passed))));
}

/** Computes the ApprovalIsHuman calculated field.
 *  TRUE when the designated approval role is currently held by a human agent.
 *  Formula: ={{ApprovalRoleAgentKind}} = "Human" */
export function calcSendIntentsApprovalIsHuman(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.approval_role_agent_kind), vS("Human")));
}

/** Computes the AuthorizationGatePassed calculated field.
 *  TRUE only when the template is validly approved, legal review has cleared, and the approving role is held by a human. The authorization gate.
 *  Formula: =AND({{TemplateIsSendable}}, AND({{ExecutionHasLegalClearance}}, {{ApprovalIsHuman}})) */
export function calcSendIntentsAuthorizationGatePassed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.template_is_sendable)), erbBool3(erbAnd(erbBool3(vBool(tc.execution_has_legal_clearance)), erbBool3(vBool(tc.approval_is_human))))));
}

/** Computes the IsClearedToSend calculated field.
 *  TRUE only when all four gates pass. THE single column the pipeline reads before transmitting.
 *  Formula: =AND({{PermissionGatePassed}}, AND({{TimingGatePassed}}, AND({{ContentGatePassed}}, {{AuthorizationGatePassed}}))) */
export function calcSendIntentsIsClearedToSend(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.permission_gate_passed)), erbBool3(erbAnd(erbBool3(vBool(tc.timing_gate_passed)), erbBool3(erbAnd(erbBool3(vBool(tc.content_gate_passed)), erbBool3(vBool(tc.authorization_gate_passed))))))));
}

/** Computes the BlockingGateName calculated field.
 *  Names the first gate that refused, for the suppression reason; empty string when cleared.
 *  Formula: =IF({{IsClearedToSend}}, "", IF(NOT({{PermissionGatePassed}}), "Permission", IF(NOT({{TimingGatePassed}}), "Timing", IF(NOT({{ContentGatePassed}}), "Content", "Authorization")))) */
export function calcSendIntentsBlockingGateName(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_cleared_to_send)), () => vS(""), () => erbIf(erbBool3(erbNot(erbBool3(vBool(tc.permission_gate_passed)))), () => vS("Permission"), () => erbIf(erbBool3(erbNot(erbBool3(vBool(tc.timing_gate_passed)))), () => vS("Timing"), () => erbIf(erbBool3(erbNot(erbBool3(vBool(tc.content_gate_passed)))), () => vS("Content"), () => vS("Authorization"))))));
}

/** Computes the HasResultingDelivery calculated field.
 *  TRUE when this intent produced a delivery record of any status.
 *  Formula: ={{ResultingDelivery}} <> "" */
export function calcSendIntentsHasResultingDelivery(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.resulting_delivery)));
}

/** Computes the IsOverriddenRefusal calculated field.
 *  TRUE when a gate refused and the message was transmitted regardless. The override witness.
 *  Formula: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}})) */
export function calcSendIntentsIsOverriddenRefusal(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbAnd(erbBool3(vBool(tc.has_resulting_delivery)), erbBool3(vBool(tc.resulting_delivery_was_transmitted))))));
}

/** Computes the IsSilentlyDropped calculated field.
 *  TRUE when a gate refused and no delivery record of any kind was produced -- the recipient vanished from the run.
 *  Formula: =AND(NOT({{IsClearedToSend}}), NOT({{HasResultingDelivery}})) */
export function calcSendIntentsIsSilentlyDropped(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbNot(erbBool3(vBool(tc.has_resulting_delivery))))));
}

/** Computes the RefusalCitedAnException calculated field.
 *  TRUE when the suppression produced by this refusal cited a documented exception.
 *  Formula: ={{ResultingDeliveryException}} <> "" */
export function calcSendIntentsRefusalCitedAnException(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.resulting_delivery_exception)));
}

/** Computes the IsProperlyHandledRefusal calculated field.
 *  TRUE when a refusal correctly resulted in a non-transmitted delivery record citing a documented exception. The positive witness.
 *  Formula: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, AND(NOT({{ResultingDeliveryWasTransmitted}}), {{RefusalCitedAnException}}))) */
export function calcSendIntentsIsProperlyHandledRefusal(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbAnd(erbBool3(vBool(tc.has_resulting_delivery)), erbBool3(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.resulting_delivery_was_transmitted)))), erbBool3(vBool(tc.refusal_cited_an_exception))))))));
}

/** Computes the RefusalFailureExecutionKey calculated field.
 *  Carries the execution id on any mishandled refusal; empty string otherwise.
 *  Formula: =IF(OR({{IsOverriddenRefusal}}, {{IsSilentlyDropped}}), {{ProcedureExecution}}, "") */
export function calcSendIntentsRefusalFailureExecutionKey(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbOr(erbBool3(vBool(tc.is_overridden_refusal)), erbBool3(vBool(tc.is_silently_dropped)))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the IntentExecutionKey calculated field.
 *  The procedure execution id for every intent, used as the campaign rollup key.
 *  Formula: ={{ProcedureExecution}} */
export function calcSendIntentsIntentExecutionKey(tc: SendIntentsRow): string | null {
  return toStringPtr(vStr(tc.procedure_execution));
}

/** Computes the DeliveredIntentExecutionKey calculated field.
 *  The execution id when this intent actually reached a person, otherwise empty string.
 *  Formula: =IF(AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}), {{ProcedureExecution}}, "") */
export function calcSendIntentsDeliveredIntentExecutionKey(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.has_resulting_delivery)), erbBool3(vBool(tc.resulting_delivery_was_transmitted)))), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the DroppedIntentExecutionKey calculated field.
 *  The execution id when this intent was refused and left no record at all.
 *  Formula: =IF({{IsSilentlyDropped}}, {{ProcedureExecution}}, "") */
export function calcSendIntentsDroppedIntentExecutionKey(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_silently_dropped)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the MyApprovalWasInForce calculated field.
 *  TRUE when the template carried a valid approval at the moment this intent was evaluated.
 *  Formula: ={{TemplateIsSendable}} */
export function calcSendIntentsMyApprovalWasInForce(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(vBool(tc.template_is_sendable));
}

/** Computes the RefusedOnApprovedContent calculated field.
 *  TRUE when the pipeline refused an intent on content grounds even though the template was approved.
 *  Formula: =AND({{MyApprovalWasInForce}}, NOT({{ContentGatePassed}})) */
export function calcSendIntentsRefusedOnApprovedContent(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.my_approval_was_in_force)), erbBool3(erbNot(erbBool3(vBool(tc.content_gate_passed))))));
}

/** Computes the RefusedOnOptOutOnly calculated field.
 *  TRUE when the only content failure was a missing or mispositioned opt-out phrase.
 *  Formula: =AND(NOT({{OptOutGatePassed}}), {{LengthGatePassed}}) */
export function calcSendIntentsRefusedOnOptOutOnly(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.opt_out_gate_passed)))), erbBool3(vBool(tc.length_gate_passed))));
}

/** Computes the RefusalWasOnMyRules calculated field.
 *  TRUE when the refusal came from a communications rule I own -- content, length, opt-out, quiet hours.
 *  Formula: =AND(NOT({{IsClearedToSend}}), OR(NOT({{ContentGatePassed}}), NOT({{TimingGatePassed}}))) */
export function calcSendIntentsRefusalWasOnMyRules(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.content_gate_passed)))), erbBool3(erbNot(erbBool3(vBool(tc.timing_gate_passed))))))));
}

/** Computes the RefusalWasOutsideMyControl calculated field.
 *  TRUE when the refusal came from consent, reachability, or authorization -- none of which I can fix by editing a template.
 *  Formula: =AND(NOT({{IsClearedToSend}}), OR(NOT({{PermissionGatePassed}}), NOT({{AuthorizationGatePassed}}))) */
export function calcSendIntentsRefusalWasOutsideMyControl(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.permission_gate_passed)))), erbBool3(erbNot(erbBool3(vBool(tc.authorization_gate_passed))))))));
}

/** Computes the IsUnreportedRefusalOnMyRules calculated field.
 *  TRUE when a refusal I own and could have fixed was never surfaced to me.
 *  Formula: =AND({{RefusalWasOnMyRules}}, NOT({{ApproverWasNotified}})) */
export function calcSendIntentsIsUnreportedRefusalOnMyRules(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.refusal_was_on_my_rules)), erbBool3(erbNot(erbIsTrue(vBool(tc.approver_was_notified))))));
}

/** Computes the IsApprovalOverriddenSilently calculated field.
 *  TRUE when the pipeline overrode a valid human approval on content grounds and told nobody.
 *  Formula: =AND({{RefusedOnApprovedContent}}, NOT({{ApproverWasNotified}})) */
export function calcSendIntentsIsApprovalOverriddenSilently(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.refused_on_approved_content)), erbBool3(erbNot(erbIsTrue(vBool(tc.approver_was_notified))))));
}

/** Computes the HasAlternateChannelAttempt calculated field.
 *  TRUE when a follow-up intent on another channel was raised for this refused send.
 *  Formula: ={{AlternateChannelIntent}} <> "" */
export function calcSendIntentsHasAlternateChannelAttempt(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.alternate_channel_intent)));
}

/** Computes the IsRefusedWithNoAlternative calculated field.
 *  TRUE when a send was refused and no attempt was ever made on any other channel.
 *  Formula: =AND(NOT({{IsClearedToSend}}), NOT({{HasAlternateChannelAttempt}})) */
export function calcSendIntentsIsRefusedWithNoAlternative(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbNot(erbBool3(vBool(tc.has_alternate_channel_attempt))))));
}

/** Computes the ExceptionPrescribedAnAlternative calculated field.
 *  TRUE when the documented exception for this refusal prescribes a different-channel send as the correct handling.
 *  Formula: =AND({{RefusalCitedAnException}}, {{ResultingDeliveryException}} <> "") */
export function calcSendIntentsExceptionPrescribedAnAlternative(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.refusal_cited_an_exception)), erbBool3(erbIsNotBlank(vStr(tc.resulting_delivery_exception)))));
}

/** Computes the PrescribedHandlingWasPerformed calculated field.
 *  TRUE when the exception prescribed an alternate channel and an alternate intent was actually raised and cleared.
 *  Formula: =AND({{ExceptionPrescribedAnAlternative}}, AND({{HasAlternateChannelAttempt}}, {{AlternateAttemptWasCleared}})) */
export function calcSendIntentsPrescribedHandlingWasPerformed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.exception_prescribed_an_alternative)), erbBool3(erbAnd(erbBool3(vBool(tc.has_alternate_channel_attempt)), erbBool3(vBool(tc.alternate_attempt_was_cleared))))));
}

/** Computes the IsSuppressionWithoutRemedy calculated field.
 *  TRUE when we cited an exception that prescribed an alternate channel and then never performed it.
 *  Formula: =AND({{ExceptionPrescribedAnAlternative}}, NOT({{PrescribedHandlingWasPerformed}})) */
export function calcSendIntentsIsSuppressionWithoutRemedy(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.exception_prescribed_an_alternative)), erbBool3(erbNot(erbBool3(vBool(tc.prescribed_handling_was_performed))))));
}

/** Computes the HasDurableRefusalRecord calculated field.
 *  TRUE when this refusal was written down somewhere a human can find it.
 *  Formula: ={{RefusalRecordedAt}} <> "" */
export function calcSendIntentsHasDurableRefusalRecord(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.refusal_recorded_at)));
}

/** Computes the RefusalWasEscalated calculated field.
 *  TRUE when a specific role was notified of this refusal.
 *  Formula: ={{RefusalNotifiedRole}} <> "" */
export function calcSendIntentsRefusalWasEscalated(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.refusal_notified_role)));
}

/** Computes the IsUnrecordedRefusal calculated field.
 *  TRUE when I refused a send and produced no delivery record, no refusal record, and no exception. The refusal left no trace of any kind.
 *  Formula: =AND({{IsSilentlyDropped}}, AND(NOT({{HasDurableRefusalRecord}}), NOT({{RefusalCitedAnException}}))) */
export function calcSendIntentsIsUnrecordedRefusal(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_silently_dropped)), erbBool3(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.has_durable_refusal_record)))), erbBool3(erbNot(erbBool3(vBool(tc.refusal_cited_an_exception))))))));
}

/** Computes the IsUnescalatedRefusal calculated field.
 *  TRUE when a refusal was recorded but no human role was ever told.
 *  Formula: =AND(NOT({{IsClearedToSend}}), NOT({{RefusalWasEscalated}})) */
export function calcSendIntentsIsUnescalatedRefusal(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbNot(erbBool3(vBool(tc.refusal_was_escalated))))));
}

/** Computes the UnescalatedRefusalRoleKey calculated field.
 *  Echoes the role that should have been told about this refusal, but only when the refusal went unrecorded. Empty otherwise.
 *  Formula: =IF({{IsUnrecordedRefusal}}, {{RefusalNotifiedRole}}, "") */
export function calcSendIntentsUnescalatedRefusalRoleKey(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unrecorded_refusal)), () => vStr(tc.refusal_notified_role), () => vS("")));
}

/** Computes the UnrecordedRefusalExecutionKey calculated field.
 *  Echoes the parent procedure execution only for refusals nobody recorded; empty otherwise.
 *  Formula: =IF({{IsUnrecordedRefusal}}, {{ProcedureExecution}}, "") */
export function calcSendIntentsUnrecordedRefusalExecutionKey(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unrecorded_refusal)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes the WasDeferredOnTiming calculated field.
 *  TRUE when the timing gate is the reason this intent did not clear, and every other gate passed.
 *  Formula: =AND(NOT({{TimingGatePassed}}), AND({{PermissionGatePassed}}, {{ContentGatePassed}})) */
export function calcSendIntentsWasDeferredOnTiming(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.timing_gate_passed)))), erbBool3(erbAnd(erbBool3(vBool(tc.permission_gate_passed)), erbBool3(vBool(tc.content_gate_passed))))));
}

/** Computes the WindowHasSinceReopened calculated field.
 *  TRUE when enough time has passed since evaluation that the quiet window this intent hit must have closed.
 *  Formula: =AND({{HoursUntilWindowOpens}} > 0, DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours") > {{HoursUntilWindowOpens}}) */
export function calcSendIntentsWindowHasSinceReopened(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vInt(tc.hours_until_window_opens), ">", vI(0))), erbBool3(erbCmp(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.evaluated_at), vS("hours")), ">", vInt(tc.hours_until_window_opens)))));
}

/** Computes the HasRetryAttempt calculated field.
 *  TRUE when a retry intent was raised for this deferred send.
 *  Formula: ={{RetryIntent}} <> "" */
export function calcSendIntentsHasRetryAttempt(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.retry_intent)));
}

/** Computes the IsAbandonedDeferral calculated field.
 *  TRUE when a send was deferred for timing, the window has since reopened, and no retry was ever raised. A deferral silently converted into a cancellation.
 *  Formula: =AND({{WasDeferredOnTiming}}, AND({{WindowHasSinceReopened}}, NOT({{HasRetryAttempt}}))) */
export function calcSendIntentsIsAbandonedDeferral(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_deferred_on_timing)), erbBool3(erbAnd(erbBool3(vBool(tc.window_has_since_reopened)), erbBool3(erbNot(erbBool3(vBool(tc.has_retry_attempt))))))));
}

/** Computes the DeferralAgeHours calculated field.
 *  How long this deferred intent has been sitting since evaluation.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours") */
export function calcSendIntentsDeferralAgeHours(tc: SendIntentsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.evaluated_at), vS("hours"))));
}

/** Computes the IsStaleDeferral calculated field.
 *  TRUE when a deferred send has been waiting more than 24 hours -- longer than any quiet window can justify.
 *  Formula: =AND({{WasDeferredOnTiming}}, {{DeferralAgeHours}} > 24) */
export function calcSendIntentsIsStaleDeferral(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_deferred_on_timing)), erbBool3(erbCmp(vInt(tc.deferral_age_hours), ">", vI(24)))));
}

/** Computes the ConsentInputWasResolvable calculated field.
 *  TRUE when the recipient's consent state was actually retrievable, as opposed to absent and read as a refusal.
 *  Formula: ={{RecipientConsentStatusRaw}} <> "" */
export function calcSendIntentsConsentInputWasResolvable(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.recipient_consent_status_raw)));
}

/** Computes the PolicyInputWasResolvable calculated field.
 *  TRUE when a governing communication policy was actually found for this intent.
 *  Formula: ={{IntentPolicy}} <> "" */
export function calcSendIntentsPolicyInputWasResolvable(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.intent_policy)));
}

/** Computes the AllGateInputsResolved calculated field.
 *  TRUE when every input my gates depend on was actually retrievable.
 *  Formula: =AND({{ConsentInputWasResolvable}}, {{PolicyInputWasResolvable}}) */
export function calcSendIntentsAllGateInputsResolved(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.consent_input_was_resolvable)), erbBool3(vBool(tc.policy_input_was_resolvable))));
}

/** Computes the IsUnevaluableRefusal calculated field.
 *  TRUE when I refused a send but at least one gate input could not be resolved -- so I do not actually know whether the rule was violated or merely unreadable.
 *  Formula: =AND(NOT({{IsClearedToSend}}), NOT({{AllGateInputsResolved}})) */
export function calcSendIntentsIsUnevaluableRefusal(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(vBool(tc.is_cleared_to_send)))), erbBool3(erbNot(erbBool3(vBool(tc.all_gate_inputs_resolved))))));
}

/** Computes the IsSelfWitnessedDecision calculated field.
 *  TRUE when the entire decision to send or refuse rests solely on my own computation, unconfirmed by anything else.
 *  Formula: =NOT({{GateResultWasIndependentlyConfirmed}}) */
export function calcSendIntentsIsSelfWitnessedDecision(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbNot(erbIsTrue(vBool(tc.gate_result_was_independently_confirmed))));
}

/** Computes the IsIndependentlyConfirmed calculated field.
 *  TRUE when this intent's decision was corroborated by an actual delivery record rather than resting solely on the pipeline's own say-so.
 *  Formula: =AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}) */
export function calcSendIntentsIsIndependentlyConfirmed(tc: SendIntentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_resulting_delivery)), erbBool3(vBool(tc.resulting_delivery_was_transmitted))));
}

/** Computes the IndependentlyConfirmedExecutionKey calculated field.
 *  Echoes the parent execution only for independently confirmed intents; empty otherwise.
 *  Formula: =IF({{IsIndependentlyConfirmed}}, {{ProcedureExecution}}, "") */
export function calcSendIntentsIndependentlyConfirmedExecutionKey(tc: SendIntentsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_independently_confirmed)), () => vStr(tc.procedure_execution), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeSendIntents(tc: SendIntentsRow): SendIntentsRow {
  // Level 1
  calcGuard(tc, sendIntentsFieldTypes, "name", () => { tc.name = calcSendIntentsName(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "consent_gate_passed", () => { tc.consent_gate_passed = calcSendIntentsConsentGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "reachability_gate_passed", () => { tc.reachability_gate_passed = calcSendIntentsReachabilityGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "intent_policy_has_quiet_hours", () => { tc.intent_policy_has_quiet_hours = calcSendIntentsIntentPolicyHasQuietHours(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "intent_quiet_window_wraps", () => { tc.intent_quiet_window_wraps = calcSendIntentsIntentQuietWindowWraps(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "length_gate_passed", () => { tc.length_gate_passed = calcSendIntentsLengthGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "opt_out_gate_passed", () => { tc.opt_out_gate_passed = calcSendIntentsOptOutGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "approval_is_human", () => { tc.approval_is_human = calcSendIntentsApprovalIsHuman(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "has_resulting_delivery", () => { tc.has_resulting_delivery = calcSendIntentsHasResultingDelivery(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "refusal_cited_an_exception", () => { tc.refusal_cited_an_exception = calcSendIntentsRefusalCitedAnException(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "intent_execution_key", () => { tc.intent_execution_key = calcSendIntentsIntentExecutionKey(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "my_approval_was_in_force", () => { tc.my_approval_was_in_force = calcSendIntentsMyApprovalWasInForce(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "has_alternate_channel_attempt", () => { tc.has_alternate_channel_attempt = calcSendIntentsHasAlternateChannelAttempt(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "has_durable_refusal_record", () => { tc.has_durable_refusal_record = calcSendIntentsHasDurableRefusalRecord(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "refusal_was_escalated", () => { tc.refusal_was_escalated = calcSendIntentsRefusalWasEscalated(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "has_retry_attempt", () => { tc.has_retry_attempt = calcSendIntentsHasRetryAttempt(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "deferral_age_hours", () => { tc.deferral_age_hours = calcSendIntentsDeferralAgeHours(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "consent_input_was_resolvable", () => { tc.consent_input_was_resolvable = calcSendIntentsConsentInputWasResolvable(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "policy_input_was_resolvable", () => { tc.policy_input_was_resolvable = calcSendIntentsPolicyInputWasResolvable(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_self_witnessed_decision", () => { tc.is_self_witnessed_decision = calcSendIntentsIsSelfWitnessedDecision(tc); });
  // Level 2
  calcGuard(tc, sendIntentsFieldTypes, "permission_gate_passed", () => { tc.permission_gate_passed = calcSendIntentsPermissionGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "intent_is_inside_quiet_window", () => { tc.intent_is_inside_quiet_window = calcSendIntentsIntentIsInsideQuietWindow(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "content_gate_passed", () => { tc.content_gate_passed = calcSendIntentsContentGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "authorization_gate_passed", () => { tc.authorization_gate_passed = calcSendIntentsAuthorizationGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "delivered_intent_execution_key", () => { tc.delivered_intent_execution_key = calcSendIntentsDeliveredIntentExecutionKey(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "refused_on_opt_out_only", () => { tc.refused_on_opt_out_only = calcSendIntentsRefusedOnOptOutOnly(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "exception_prescribed_an_alternative", () => { tc.exception_prescribed_an_alternative = calcSendIntentsExceptionPrescribedAnAlternative(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "all_gate_inputs_resolved", () => { tc.all_gate_inputs_resolved = calcSendIntentsAllGateInputsResolved(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_independently_confirmed", () => { tc.is_independently_confirmed = calcSendIntentsIsIndependentlyConfirmed(tc); });
  // Level 3
  calcGuard(tc, sendIntentsFieldTypes, "timing_gate_passed", () => { tc.timing_gate_passed = calcSendIntentsTimingGatePassed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "refused_on_approved_content", () => { tc.refused_on_approved_content = calcSendIntentsRefusedOnApprovedContent(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "prescribed_handling_was_performed", () => { tc.prescribed_handling_was_performed = calcSendIntentsPrescribedHandlingWasPerformed(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "independently_confirmed_execution_key", () => { tc.independently_confirmed_execution_key = calcSendIntentsIndependentlyConfirmedExecutionKey(tc); });
  // Level 4
  calcGuard(tc, sendIntentsFieldTypes, "hours_until_window_opens", () => { tc.hours_until_window_opens = calcSendIntentsHoursUntilWindowOpens(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_cleared_to_send", () => { tc.is_cleared_to_send = calcSendIntentsIsClearedToSend(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_approval_overridden_silently", () => { tc.is_approval_overridden_silently = calcSendIntentsIsApprovalOverriddenSilently(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_suppression_without_remedy", () => { tc.is_suppression_without_remedy = calcSendIntentsIsSuppressionWithoutRemedy(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "was_deferred_on_timing", () => { tc.was_deferred_on_timing = calcSendIntentsWasDeferredOnTiming(tc); });
  // Level 5
  calcGuard(tc, sendIntentsFieldTypes, "blocking_gate_name", () => { tc.blocking_gate_name = calcSendIntentsBlockingGateName(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_overridden_refusal", () => { tc.is_overridden_refusal = calcSendIntentsIsOverriddenRefusal(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_silently_dropped", () => { tc.is_silently_dropped = calcSendIntentsIsSilentlyDropped(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_properly_handled_refusal", () => { tc.is_properly_handled_refusal = calcSendIntentsIsProperlyHandledRefusal(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "refusal_was_on_my_rules", () => { tc.refusal_was_on_my_rules = calcSendIntentsRefusalWasOnMyRules(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "refusal_was_outside_my_control", () => { tc.refusal_was_outside_my_control = calcSendIntentsRefusalWasOutsideMyControl(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_refused_with_no_alternative", () => { tc.is_refused_with_no_alternative = calcSendIntentsIsRefusedWithNoAlternative(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_unescalated_refusal", () => { tc.is_unescalated_refusal = calcSendIntentsIsUnescalatedRefusal(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "window_has_since_reopened", () => { tc.window_has_since_reopened = calcSendIntentsWindowHasSinceReopened(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_stale_deferral", () => { tc.is_stale_deferral = calcSendIntentsIsStaleDeferral(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_unevaluable_refusal", () => { tc.is_unevaluable_refusal = calcSendIntentsIsUnevaluableRefusal(tc); });
  // Level 6
  calcGuard(tc, sendIntentsFieldTypes, "refusal_failure_execution_key", () => { tc.refusal_failure_execution_key = calcSendIntentsRefusalFailureExecutionKey(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "dropped_intent_execution_key", () => { tc.dropped_intent_execution_key = calcSendIntentsDroppedIntentExecutionKey(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_unreported_refusal_on_my_rules", () => { tc.is_unreported_refusal_on_my_rules = calcSendIntentsIsUnreportedRefusalOnMyRules(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_unrecorded_refusal", () => { tc.is_unrecorded_refusal = calcSendIntentsIsUnrecordedRefusal(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "is_abandoned_deferral", () => { tc.is_abandoned_deferral = calcSendIntentsIsAbandonedDeferral(tc); });
  // Level 7
  calcGuard(tc, sendIntentsFieldTypes, "unescalated_refusal_role_key", () => { tc.unescalated_refusal_role_key = calcSendIntentsUnescalatedRefusalRoleKey(tc); });
  calcGuard(tc, sendIntentsFieldTypes, "unrecorded_refusal_execution_key", () => { tc.unrecorded_refusal_execution_key = calcSendIntentsUnrecordedRefusalExecutionKey(tc); });
  return tc;
}

/** Reads SendIntents rows from a JSON array file. */
export function loadSendIntentsRows(file: string): SendIntentsRow[] {
  return loadRows(file, { fields: sendIntentsFieldTypes }) as unknown as SendIntentsRow[];
}

// =============================================================================
// AGENTDECISIONRECORDS TABLE
// AgentDecisionRecords (added by witness loop 1).
// =============================================================================

/** A row in the AgentDecisionRecords table. */
export interface AgentDecisionRecordsRow {
  /** Stored logical identifier for one AgentDecisionRecords row. */
  agent_decision_record_id: string;
  /** Human-readable calculated display alias for the AgentDecisionRecords row. */
  name: string | null;
  /** Step execution during which the decision was made. */
  step_execution: string | null;
  /** Agent that made the decision. */
  deciding_agent: string | null;
  /** Classification, Prioritization, Draft, Suppression, Escalation, or Posting. */
  decision_kind: string | null;
  /** What the agent decided, in one sentence. */
  decision_summary: string | null;
  /** When the decision was made. */
  decided_at: string | null;
  /** BelowThreshold, Material, or Escalated. */
  materiality_band: string | null;
  /** Accepted, Corrected, Reversed, or NotReviewed. */
  human_disposition: string | null;
  /** Human agent that dispositioned the decision. */
  reviewed_by_agent: string | null;
  /** When a human dispositioned the decision. */
  reviewed_at: string | null;
  /** TRUE when a human corrected or reversed this decision. */
  was_overridden: boolean | null;
  /** TRUE when a human actually dispositioned this decision. */
  was_reviewed: boolean | null;
  /** Whether the deciding agent is Human, AIAgent, or AutomatedPipeline. */
  deciding_agent_kind: string | null;
  /** Echoes the deciding agent id when the decision was overridden, blank otherwise. */
  deciding_agent_when_overridden: string | null;
  /** Role assignment in force when the decision was made; anchors the decision to one side of a handover. */
  under_role_assignment: string | null;
  /** Echoes the governing role assignment when one is recorded, blank otherwise. */
  role_assignment_when_scored: string | null;
  /** Echoes the governing role assignment when the decision was overridden, blank otherwise. */
  role_assignment_when_overridden: string | null;
  /** The specified step at which this decision was made. */
  step_of_decision: string | null;
  /** Composite key of step, deciding agent kind, and decision kind for this decision. */
  boundary_match_key: string | null;
  /** Number of authority boundaries this decision matches. */
  matching_boundary_count: number | null;
  /** TRUE when this decision matches an authority boundary that forbids it. */
  violated_authority_boundary: boolean | null;
  /** Agent kind of the reviewer, if any. */
  reviewer_agent_kind: string | null;
  /** TRUE when a human agent actually dispositioned this decision. */
  has_human_confirmation: boolean | null;
  /** TRUE when a non-human agent made a material or escalated decision. */
  needs_human_confirmation: boolean | null;
  /** TRUE when a material non-human decision was never confirmed by a human. */
  is_unconfirmed_non_human_decision: boolean | null;
  /** Echoes the step-execution id when the decision is an unconfirmed non-human material decision, blank otherwise. */
  step_execution_when_unconfirmed: string | null;
  /** Echoes the deciding agent id when the decision violated a boundary, blank otherwise. */
  agent_when_boundary_violated: string | null;
  /** Minutes between the decision and its human disposition; 0 when never reviewed. Declared number, not integer: DATETIME_DIFF returns a numeric and an integer cast makes the entire view fail on read. */
  review_latency_minutes: number | null;
  /** TRUE when this decision produced text — the drafter's own output class. */
  is_draft_kind: boolean | null;
  /** Echoes the deciding agent id when a drafting decision was overridden, blank otherwise. */
  agent_when_draft_overridden: string | null;
  /** Echoes the deciding agent id when the decision produced text, blank otherwise. */
  agent_when_draft: string | null;
  /** Why the human changed my output: ErrorCorrection, JudgmentReserved, PolicyChange, or empty when not overridden. */
  override_reason_kind: string | null;
  /** TRUE when the override corrected something I got wrong. */
  is_error_correction: boolean | null;
  /** TRUE when the override was a human exercising authority the procedure always reserved to them. */
  is_reserved_judgment_override: boolean | null;
  /** TRUE when an override carries a stated reason. */
  override_reason_is_recorded: boolean | null;
  /** TRUE when my output was changed and nobody recorded why. */
  is_unexplained_override: boolean | null;
  /** The role assignment id when this decision was overridden as an error correction, otherwise empty. */
  error_correction_role_assignment_key: string | null;
  /** Echoes the role assignment this decision was made under, but only when the decision violated an authority boundary. Empty otherwise. */
  boundary_violation_role_assignment_key: string | null;
  /** Extension class IRI for an agent decision record. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const agentDecisionRecordsFieldTypes: Record<string, FieldType> = {
  agent_decision_record_id: "string",
  name: "*string",
  step_execution: "*string",
  deciding_agent: "*string",
  decision_kind: "*string",
  decision_summary: "*string",
  decided_at: "*string",
  materiality_band: "*string",
  human_disposition: "*string",
  reviewed_by_agent: "*string",
  reviewed_at: "*string",
  was_overridden: "*bool",
  was_reviewed: "*bool",
  deciding_agent_kind: "*string",
  deciding_agent_when_overridden: "*string",
  under_role_assignment: "*string",
  role_assignment_when_scored: "*string",
  role_assignment_when_overridden: "*string",
  step_of_decision: "*string",
  boundary_match_key: "*string",
  matching_boundary_count: "*float64",
  violated_authority_boundary: "*bool",
  reviewer_agent_kind: "*string",
  has_human_confirmation: "*bool",
  needs_human_confirmation: "*bool",
  is_unconfirmed_non_human_decision: "*bool",
  step_execution_when_unconfirmed: "*string",
  agent_when_boundary_violated: "*string",
  review_latency_minutes: "*float64",
  is_draft_kind: "*bool",
  agent_when_draft_overridden: "*string",
  agent_when_draft: "*string",
  override_reason_kind: "*string",
  is_error_correction: "*bool",
  is_reserved_judgment_override: "*bool",
  override_reason_is_recorded: "*bool",
  is_unexplained_override: "*bool",
  error_correction_role_assignment_key: "*string",
  boundary_violation_role_assignment_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AgentDecisionRecords row.
 *  Formula: ={{DecidingAgent}} & ": " & LEFT({{DecisionSummary}}, 60) */
export function calcAgentDecisionRecordsName(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.deciding_agent)), vS(": "), erbTextNotNull(erbLeft(vStr(tc.decision_summary), vI(60)))));
}

/** Computes the WasOverridden calculated field.
 *  TRUE when a human corrected or reversed this decision.
 *  Formula: =OR({{HumanDisposition}} = "Corrected", {{HumanDisposition}} = "Reversed") */
export function calcAgentDecisionRecordsWasOverridden(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.human_disposition)), vS("Corrected"))), erbBool3(erbEq(erbNullif(vStr(tc.human_disposition)), vS("Reversed")))));
}

/** Computes the WasReviewed calculated field.
 *  TRUE when a human actually dispositioned this decision.
 *  Formula: =AND({{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed") */
export function calcAgentDecisionRecordsWasReviewed(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.human_disposition))), erbBool3(erbNe(erbNullif(vStr(tc.human_disposition)), vS("NotReviewed")))));
}

/** Computes the DecidingAgentWhenOverridden calculated field.
 *  Echoes the deciding agent id when the decision was overridden, blank otherwise.
 *  Formula: =IF({{WasOverridden}}, {{DecidingAgent}}, "") */
export function calcAgentDecisionRecordsDecidingAgentWhenOverridden(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_overridden)), () => vStr(tc.deciding_agent), () => vS("")));
}

/** Computes the RoleAssignmentWhenScored calculated field.
 *  Echoes the governing role assignment when one is recorded, blank otherwise.
 *  Formula: =IF({{UnderRoleAssignment}} <> "", {{UnderRoleAssignment}}, "") */
export function calcAgentDecisionRecordsRoleAssignmentWhenScored(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbIsNotBlank(vStr(tc.under_role_assignment))), () => vStr(tc.under_role_assignment), () => vS("")));
}

/** Computes the RoleAssignmentWhenOverridden calculated field.
 *  Echoes the governing role assignment when the decision was overridden, blank otherwise.
 *  Formula: =IF({{WasOverridden}}, {{UnderRoleAssignment}}, "") */
export function calcAgentDecisionRecordsRoleAssignmentWhenOverridden(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_overridden)), () => vStr(tc.under_role_assignment), () => vS("")));
}

/** Computes the BoundaryMatchKey calculated field.
 *  Composite key of step, deciding agent kind, and decision kind for this decision.
 *  Formula: ={{StepOfDecision}} & "|" & {{DecidingAgentKind}} & "|" & {{DecisionKind}} */
export function calcAgentDecisionRecordsBoundaryMatchKey(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step_of_decision)), vS("|"), erbTextOr(vStr(tc.deciding_agent_kind)), vS("|"), erbTextOr(vStr(tc.decision_kind))));
}

/** Computes the ViolatedAuthorityBoundary calculated field.
 *  TRUE when this decision matches an authority boundary that forbids it.
 *  Formula: ={{MatchingBoundaryCount}} > 0 */
export function calcAgentDecisionRecordsViolatedAuthorityBoundary(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.matching_boundary_count), ">", vI(0)));
}

/** Computes the HasHumanConfirmation calculated field.
 *  TRUE when a human agent actually dispositioned this decision.
 *  Formula: =AND({{ReviewerAgentKind}} = "Human", {{HumanDisposition}} <> "", {{HumanDisposition}} <> "NotReviewed") */
export function calcAgentDecisionRecordsHasHumanConfirmation(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(vStr(tc.reviewer_agent_kind), vS("Human"))), erbBool3(erbIsNotBlank(vStr(tc.human_disposition))), erbBool3(erbNe(erbNullif(vStr(tc.human_disposition)), vS("NotReviewed")))));
}

/** Computes the NeedsHumanConfirmation calculated field.
 *  TRUE when a non-human agent made a material or escalated decision.
 *  Formula: =AND(NOT({{DecidingAgentKind}} = "Human"), OR({{MaterialityBand}} = "Material", {{MaterialityBand}} = "Escalated")) */
export function calcAgentDecisionRecordsNeedsHumanConfirmation(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbEq(vStr(tc.deciding_agent_kind), vS("Human"))))), erbBool3(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.materiality_band)), vS("Material"))), erbBool3(erbEq(erbNullif(vStr(tc.materiality_band)), vS("Escalated")))))));
}

/** Computes the IsUnconfirmedNonHumanDecision calculated field.
 *  TRUE when a material non-human decision was never confirmed by a human.
 *  Formula: =AND({{NeedsHumanConfirmation}}, NOT({{HasHumanConfirmation}})) */
export function calcAgentDecisionRecordsIsUnconfirmedNonHumanDecision(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.needs_human_confirmation)), erbBool3(erbNot(erbBool3(vBool(tc.has_human_confirmation))))));
}

/** Computes the StepExecutionWhenUnconfirmed calculated field.
 *  Echoes the step-execution id when the decision is an unconfirmed non-human material decision, blank otherwise.
 *  Formula: =IF({{IsUnconfirmedNonHumanDecision}}, {{StepExecution}}, "") */
export function calcAgentDecisionRecordsStepExecutionWhenUnconfirmed(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unconfirmed_non_human_decision)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes the AgentWhenBoundaryViolated calculated field.
 *  Echoes the deciding agent id when the decision violated a boundary, blank otherwise.
 *  Formula: =IF({{ViolatedAuthorityBoundary}}, {{DecidingAgent}}, "") */
export function calcAgentDecisionRecordsAgentWhenBoundaryViolated(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.violated_authority_boundary)), () => vStr(tc.deciding_agent), () => vS("")));
}

/** Computes the ReviewLatencyMinutes calculated field.
 *  Minutes between the decision and its human disposition; 0 when never reviewed. Declared number, not integer: DATETIME_DIFF returns a numeric and an integer cast makes the entire view fail on read.
 *  Formula: =IF({{ReviewedAt}} = "", 0, DATETIME_DIFF({{ReviewedAt}}, {{DecidedAt}}, "minutes")) */
export function calcAgentDecisionRecordsReviewLatencyMinutes(tc: AgentDecisionRecordsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbIsBlank(vStr(tc.reviewed_at))), () => vI(0), () => erbDatetimeDiff(vStr(tc.reviewed_at), vStr(tc.decided_at), vS("minutes"))));
}

/** Computes the IsDraftKind calculated field.
 *  TRUE when this decision produced text — the drafter's own output class.
 *  Formula: =OR({{DecisionKind}} = "Draft", {{DecisionKind}} = "Commitment") */
export function calcAgentDecisionRecordsIsDraftKind(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.decision_kind)), vS("Draft"))), erbBool3(erbEq(erbNullif(vStr(tc.decision_kind)), vS("Commitment")))));
}

/** Computes the AgentWhenDraftOverridden calculated field.
 *  Echoes the deciding agent id when a drafting decision was overridden, blank otherwise.
 *  Formula: =IF(AND({{IsDraftKind}}, {{WasOverridden}}), {{DecidingAgent}}, "") */
export function calcAgentDecisionRecordsAgentWhenDraftOverridden(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.is_draft_kind)), erbBool3(vBool(tc.was_overridden)))), () => vStr(tc.deciding_agent), () => vS("")));
}

/** Computes the AgentWhenDraft calculated field.
 *  Echoes the deciding agent id when the decision produced text, blank otherwise.
 *  Formula: =IF({{IsDraftKind}}, {{DecidingAgent}}, "") */
export function calcAgentDecisionRecordsAgentWhenDraft(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_draft_kind)), () => vStr(tc.deciding_agent), () => vS("")));
}

/** Computes the IsErrorCorrection calculated field.
 *  TRUE when the override corrected something I got wrong.
 *  Formula: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "ErrorCorrection") */
export function calcAgentDecisionRecordsIsErrorCorrection(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_overridden)), erbBool3(erbEq(erbNullif(vStr(tc.override_reason_kind)), vS("ErrorCorrection")))));
}

/** Computes the IsReservedJudgmentOverride calculated field.
 *  TRUE when the override was a human exercising authority the procedure always reserved to them.
 *  Formula: =AND({{WasOverridden}}, {{OverrideReasonKind}} = "JudgmentReserved") */
export function calcAgentDecisionRecordsIsReservedJudgmentOverride(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_overridden)), erbBool3(erbEq(erbNullif(vStr(tc.override_reason_kind)), vS("JudgmentReserved")))));
}

/** Computes the OverrideReasonIsRecorded calculated field.
 *  TRUE when an override carries a stated reason.
 *  Formula: =AND({{WasOverridden}}, {{OverrideReasonKind}} <> "") */
export function calcAgentDecisionRecordsOverrideReasonIsRecorded(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_overridden)), erbBool3(erbIsNotBlank(vStr(tc.override_reason_kind)))));
}

/** Computes the IsUnexplainedOverride calculated field.
 *  TRUE when my output was changed and nobody recorded why.
 *  Formula: =AND({{WasOverridden}}, NOT({{OverrideReasonIsRecorded}})) */
export function calcAgentDecisionRecordsIsUnexplainedOverride(tc: AgentDecisionRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.was_overridden)), erbBool3(erbNot(erbBool3(vBool(tc.override_reason_is_recorded))))));
}

/** Computes the ErrorCorrectionRoleAssignmentKey calculated field.
 *  The role assignment id when this decision was overridden as an error correction, otherwise empty.
 *  Formula: =IF({{IsErrorCorrection}}, {{UnderRoleAssignment}}, "") */
export function calcAgentDecisionRecordsErrorCorrectionRoleAssignmentKey(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_error_correction)), () => vStr(tc.under_role_assignment), () => vS("")));
}

/** Computes the BoundaryViolationRoleAssignmentKey calculated field.
 *  Echoes the role assignment this decision was made under, but only when the decision violated an authority boundary. Empty otherwise.
 *  Formula: =IF({{ViolatedAuthorityBoundary}}, {{UnderRoleAssignment}}, "") */
export function calcAgentDecisionRecordsBoundaryViolationRoleAssignmentKey(tc: AgentDecisionRecordsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.violated_authority_boundary)), () => vStr(tc.under_role_assignment), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAgentDecisionRecords(tc: AgentDecisionRecordsRow): AgentDecisionRecordsRow {
  // Level 1
  calcGuard(tc, agentDecisionRecordsFieldTypes, "name", () => { tc.name = calcAgentDecisionRecordsName(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "was_overridden", () => { tc.was_overridden = calcAgentDecisionRecordsWasOverridden(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "was_reviewed", () => { tc.was_reviewed = calcAgentDecisionRecordsWasReviewed(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "role_assignment_when_scored", () => { tc.role_assignment_when_scored = calcAgentDecisionRecordsRoleAssignmentWhenScored(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "boundary_match_key", () => { tc.boundary_match_key = calcAgentDecisionRecordsBoundaryMatchKey(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "violated_authority_boundary", () => { tc.violated_authority_boundary = calcAgentDecisionRecordsViolatedAuthorityBoundary(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "has_human_confirmation", () => { tc.has_human_confirmation = calcAgentDecisionRecordsHasHumanConfirmation(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "needs_human_confirmation", () => { tc.needs_human_confirmation = calcAgentDecisionRecordsNeedsHumanConfirmation(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "review_latency_minutes", () => { tc.review_latency_minutes = calcAgentDecisionRecordsReviewLatencyMinutes(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "is_draft_kind", () => { tc.is_draft_kind = calcAgentDecisionRecordsIsDraftKind(tc); });
  // Level 2
  calcGuard(tc, agentDecisionRecordsFieldTypes, "deciding_agent_when_overridden", () => { tc.deciding_agent_when_overridden = calcAgentDecisionRecordsDecidingAgentWhenOverridden(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "role_assignment_when_overridden", () => { tc.role_assignment_when_overridden = calcAgentDecisionRecordsRoleAssignmentWhenOverridden(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "is_unconfirmed_non_human_decision", () => { tc.is_unconfirmed_non_human_decision = calcAgentDecisionRecordsIsUnconfirmedNonHumanDecision(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "agent_when_boundary_violated", () => { tc.agent_when_boundary_violated = calcAgentDecisionRecordsAgentWhenBoundaryViolated(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "agent_when_draft_overridden", () => { tc.agent_when_draft_overridden = calcAgentDecisionRecordsAgentWhenDraftOverridden(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "agent_when_draft", () => { tc.agent_when_draft = calcAgentDecisionRecordsAgentWhenDraft(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "is_error_correction", () => { tc.is_error_correction = calcAgentDecisionRecordsIsErrorCorrection(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "is_reserved_judgment_override", () => { tc.is_reserved_judgment_override = calcAgentDecisionRecordsIsReservedJudgmentOverride(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "override_reason_is_recorded", () => { tc.override_reason_is_recorded = calcAgentDecisionRecordsOverrideReasonIsRecorded(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "boundary_violation_role_assignment_key", () => { tc.boundary_violation_role_assignment_key = calcAgentDecisionRecordsBoundaryViolationRoleAssignmentKey(tc); });
  // Level 3
  calcGuard(tc, agentDecisionRecordsFieldTypes, "step_execution_when_unconfirmed", () => { tc.step_execution_when_unconfirmed = calcAgentDecisionRecordsStepExecutionWhenUnconfirmed(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "is_unexplained_override", () => { tc.is_unexplained_override = calcAgentDecisionRecordsIsUnexplainedOverride(tc); });
  calcGuard(tc, agentDecisionRecordsFieldTypes, "error_correction_role_assignment_key", () => { tc.error_correction_role_assignment_key = calcAgentDecisionRecordsErrorCorrectionRoleAssignmentKey(tc); });
  return tc;
}

/** Reads AgentDecisionRecords rows from a JSON array file. */
export function loadAgentDecisionRecordsRows(file: string): AgentDecisionRecordsRow[] {
  return loadRows(file, { fields: agentDecisionRecordsFieldTypes }) as unknown as AgentDecisionRecordsRow[];
}

// =============================================================================
// DELIVEREDCOMMUNICATIONS TABLE
// Everything above is a proxy for the real question, which is instance-level: THIS message, to THIS recipient, rendered from THIS template, authorized by THIS approval. The model has MessageTemplates and CommunicationPolicies as specifications and no record of a single thing ever sent. Without an instance table, 'can I show that what was sent matched what I approved' is permanently unanswerable rather than merely unanswered — and it is the question a disputing employee actually asks. Extension term (urn:effortless:pko-extension#DeliveredCommunication); PKO has no native class for a delivered artifact instance.
// =============================================================================

/** A row in the DeliveredCommunications table. */
export interface DeliveredCommunicationsRow {
  /** Stored logical identifier for one DeliveredCommunications row. */
  delivered_communication_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** The run that produced this delivery. */
  procedure_execution: string;
  /** The policy-07 step execution that sent it. */
  sending_step_execution: string | null;
  /** The human approval gate execution that authorized this content. Null means nobody approved it. */
  authorizing_step_execution: string | null;
  /** The approved template this delivery was rendered from. */
  message_template: string | null;
  /** Email or SMS. */
  channel: string | null;
  /** Opaque recipient identifier; no personal data in the rulebook. */
  recipient_key: string | null;
  /** When the message was dispatched. */
  sent_at: string | null;
  /** Hash of the exact bytes delivered, for comparison against the approved rendering. */
  rendered_content_hash: string | null;
  /** Hash of the content as it stood at the approval gate. */
  approved_content_hash: string | null;
  /** Provider-reported delivery outcome. */
  delivery_status: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  /** TRUE when this delivery names the approval that authorized it. */
  has_authorization: boolean | null;
  /** TRUE when the bytes delivered are the bytes approved. */
  content_matches_approval: boolean | null;
  /** When the authorizing approval completed. */
  authorized_at: string | null;
  /** TRUE when the approval preceded the send. */
  was_approved_before_sending: boolean | null;
  /** TRUE when this delivery can be defended in a dispute: authorized, unaltered, and approved beforehand. */
  is_defensible: boolean | null;
  _erb_errors?: Record<string, string>;
}

const deliveredCommunicationsFieldTypes: Record<string, FieldType> = {
  delivered_communication_id: "string",
  name: "*string",
  procedure_execution: "string",
  sending_step_execution: "*string",
  authorizing_step_execution: "*string",
  message_template: "*string",
  channel: "*string",
  recipient_key: "*string",
  sent_at: "*string",
  rendered_content_hash: "*string",
  approved_content_hash: "*string",
  delivery_status: "*string",
  semantic_type_iri: "*string",
  has_authorization: "*bool",
  content_matches_approval: "*bool",
  authorized_at: "*string",
  was_approved_before_sending: "*bool",
  is_defensible: "*bool",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{Channel}} & " -> " & {{RecipientKey}} & " @ " & {{SentAt}} */
export function calcDeliveredCommunicationsName(tc: DeliveredCommunicationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.channel)), vS(" -> "), erbTextOr(vStr(tc.recipient_key)), vS(" @ "), erbTimestamptzText(vStr(tc.sent_at))));
}

/** Computes the HasAuthorization calculated field.
 *  TRUE when this delivery names the approval that authorized it.
 *  Formula: ={{AuthorizingStepExecution}} <> "" */
export function calcDeliveredCommunicationsHasAuthorization(tc: DeliveredCommunicationsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.authorizing_step_execution)));
}

/** Computes the ContentMatchesApproval calculated field.
 *  TRUE when the bytes delivered are the bytes approved.
 *  Formula: ={{RenderedContentHash}} = {{ApprovedContentHash}} */
export function calcDeliveredCommunicationsContentMatchesApproval(tc: DeliveredCommunicationsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.rendered_content_hash)), erbNullif(vStr(tc.approved_content_hash))));
}

/** Computes the WasApprovedBeforeSending calculated field.
 *  TRUE when the approval preceded the send.
 *  Formula: ={{AuthorizedAt}} <= {{SentAt}} */
export function calcDeliveredCommunicationsWasApprovedBeforeSending(tc: DeliveredCommunicationsRow): boolean | null {
  return toBoolPtr(erbCmp(vStr(tc.authorized_at), "<=", erbNullif(vStr(tc.sent_at))));
}

/** Computes the IsDefensible calculated field.
 *  TRUE when this delivery can be defended in a dispute: authorized, unaltered, and approved beforehand.
 *  Formula: =AND({{HasAuthorization}}, {{ContentMatchesApproval}}, {{WasApprovedBeforeSending}}) */
export function calcDeliveredCommunicationsIsDefensible(tc: DeliveredCommunicationsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_authorization)), erbBool3(vBool(tc.content_matches_approval)), erbBool3(vBool(tc.was_approved_before_sending))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeDeliveredCommunications(tc: DeliveredCommunicationsRow): DeliveredCommunicationsRow {
  // Level 1
  calcGuard(tc, deliveredCommunicationsFieldTypes, "name", () => { tc.name = calcDeliveredCommunicationsName(tc); });
  calcGuard(tc, deliveredCommunicationsFieldTypes, "has_authorization", () => { tc.has_authorization = calcDeliveredCommunicationsHasAuthorization(tc); });
  calcGuard(tc, deliveredCommunicationsFieldTypes, "content_matches_approval", () => { tc.content_matches_approval = calcDeliveredCommunicationsContentMatchesApproval(tc); });
  calcGuard(tc, deliveredCommunicationsFieldTypes, "was_approved_before_sending", () => { tc.was_approved_before_sending = calcDeliveredCommunicationsWasApprovedBeforeSending(tc); });
  // Level 2
  calcGuard(tc, deliveredCommunicationsFieldTypes, "is_defensible", () => { tc.is_defensible = calcDeliveredCommunicationsIsDefensible(tc); });
  return tc;
}

/** Reads DeliveredCommunications rows from a JSON array file. */
export function loadDeliveredCommunicationsRows(file: string): DeliveredCommunicationsRow[] {
  return loadRows(file, { fields: deliveredCommunicationsFieldTypes }) as unknown as DeliveredCommunicationsRow[];
}

// =============================================================================
// AUTHORITYBOUNDARIES TABLE
// AuthorityBoundaries (added by witness loop 1).
// =============================================================================

/** A row in the AuthorityBoundaries table. */
export interface AuthorityBoundariesRow {
  /** Stored logical identifier for one AuthorityBoundaries row. */
  authority_boundary_id: string;
  /** Human-readable calculated display alias for the AuthorityBoundaries row. */
  name: string | null;
  /** Step at which the boundary applies. */
  step: string | null;
  /** Agent kind that may not perform the forbidden decision kind: Human, AIAgent, or AutomatedPipeline. */
  forbidden_agent_kind: string | null;
  /** Decision kind forbidden to that agent kind at this step. */
  forbidden_decision_kind: string | null;
  /** Knowledge fragment that states the boundary. */
  ratified_by_knowledge_fragment: string | null;
  /** Requirement under which the boundary is enforced, if any. */
  enforcing_requirement: string | null;
  /** Role accountable for the boundary. */
  authority_role: string | null;
  /** Start of the boundary's valid-time interval. */
  valid_from: string | null;
  /** End of the boundary's valid-time interval; null means open-ended. */
  valid_to: string | null;
  /** Approved, Proposed, or Retired. */
  status: string | null;
  /** The evaluation context this boundary's currency is judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this boundary is judged against. */
  as_of_instant: string | null;
  /** TRUE when this boundary is approved and inside its valid-time window right now. */
  is_currently_binding: boolean | null;
  /** Whether the knowledge fragment that ratified this boundary is still valid. */
  ratifying_fragment_is_valid: boolean | null;
  /** Echoes the step id when this boundary is currently binding, blank otherwise. */
  step_when_binding: string | null;
  /** Composite key of step, forbidden agent kind, and forbidden decision kind. */
  boundary_match_key: string | null;
  /** Number of recorded decisions that this boundary forbids. */
  violation_count: number | null;
  /** TRUE when a binding boundary has never been triggered by any recorded decision. */
  is_untested: boolean | null;
  /** TRUE when this boundary names a knowledge fragment as its justification. FALSE means the rule constrains behaviour on nobody's recorded authority — strictly worse than resting on an expired claim, and previously invisible because the ratification lookup returned NULL. */
  has_ratifying_fragment: boolean | null;
  /** TRUE when a binding constraint on authority rests on no ratifying claim at all, or on one that is no longer valid. Either way the rule is being enforced without a live justification. */
  is_unwarranted: boolean | null;
  /** Whether the fragment ratifying this boundary is past its review date. */
  ratifying_fragment_is_overdue: boolean | null;
  /** Whether the fragment ratifying this boundary rests on a single witness. */
  ratifying_fragment_is_single_witness: boolean | null;
  /** A binding boundary whose ratifying knowledge is either overdue for review or single-sourced — still valid, but weakly warranted. */
  warrant_is_thin: boolean | null;
  /** A boundary whose ratification has lapsed and which no agent decision has ever been evaluated against — we cannot show it works and we cannot show why it exists. */
  is_unwarranted_and_untested: boolean | null;
  /** Composite-key echo: the step this boundary governs when the boundary is unwarranted, blank otherwise. */
  unwarranted_boundary_step_key: string | null;
  /** Composite-key echo: the fragment ratifying this boundary when the boundary is currently binding, blank otherwise. */
  ratifying_fragment_key: string | null;
  /** The status string of the knowledge fragment that ratifies this boundary: Approved, Reviewed, Draft, or empty. */
  ratifying_fragment_status: string | null;
  /** TRUE when this boundary names a ratifying fragment and that fragment is no longer valid. */
  ratification_lapsed: boolean | null;
  /** TRUE when a boundary is still enforced against agents while the knowledge that authorized it has lapsed. */
  binds_despite_lapsed_ratification: boolean | null;
  /** TRUE when a boundary has lapsed ratification AND has never once been exercised -- so neither its authority nor its operation has ever been demonstrated. */
  is_ungrounded_and_untested: boolean | null;
  /** The role id this boundary constrains, emitted only when the boundary is ungrounded. */
  constrained_role_assignment_key: string | null;
  /** Extension class IRI for an authority boundary. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const authorityBoundariesFieldTypes: Record<string, FieldType> = {
  authority_boundary_id: "string",
  name: "*string",
  step: "*string",
  forbidden_agent_kind: "*string",
  forbidden_decision_kind: "*string",
  ratified_by_knowledge_fragment: "*string",
  enforcing_requirement: "*string",
  authority_role: "*string",
  valid_from: "*string",
  valid_to: "*string",
  status: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  is_currently_binding: "*bool",
  ratifying_fragment_is_valid: "*bool",
  step_when_binding: "*string",
  boundary_match_key: "*string",
  violation_count: "*float64",
  is_untested: "*bool",
  has_ratifying_fragment: "*bool",
  is_unwarranted: "*bool",
  ratifying_fragment_is_overdue: "*bool",
  ratifying_fragment_is_single_witness: "*bool",
  warrant_is_thin: "*bool",
  is_unwarranted_and_untested: "*bool",
  unwarranted_boundary_step_key: "*string",
  ratifying_fragment_key: "*string",
  ratifying_fragment_status: "*string",
  ratification_lapsed: "*bool",
  binds_despite_lapsed_ratification: "*bool",
  is_ungrounded_and_untested: "*bool",
  constrained_role_assignment_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AuthorityBoundaries row.
 *  Formula: ={{ForbiddenAgentKind}} & " may not " & {{ForbiddenDecisionKind}} */
export function calcAuthorityBoundariesName(tc: AuthorityBoundariesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.forbidden_agent_kind)), vS(" may not "), erbTextOr(vStr(tc.forbidden_decision_kind))));
}

/** Computes the IsCurrentlyBinding calculated field.
 *  TRUE when this boundary is approved and inside its valid-time window right now.
 *  Formula: =AND({{Status}} = "Approved", {{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})) */
export function calcAuthorityBoundariesIsCurrentlyBinding(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(erbNullif(vStr(tc.status)), vS("Approved"))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_from)), "<=", vStr(tc.as_of_instant))), erbBool3(erbOr(erbBool3(erbIsBlank(vStr(tc.valid_to))), erbBool3(erbCmp(erbNullif(vStr(tc.valid_to)), ">", vStr(tc.as_of_instant)))))));
}

/** Computes the StepWhenBinding calculated field.
 *  Echoes the step id when this boundary is currently binding, blank otherwise.
 *  Formula: =IF({{IsCurrentlyBinding}}, {{Step}}, "") */
export function calcAuthorityBoundariesStepWhenBinding(tc: AuthorityBoundariesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_currently_binding)), () => vStr(tc.step), () => vS("")));
}

/** Computes the BoundaryMatchKey calculated field.
 *  Composite key of step, forbidden agent kind, and forbidden decision kind.
 *  Formula: ={{Step}} & "|" & {{ForbiddenAgentKind}} & "|" & {{ForbiddenDecisionKind}} */
export function calcAuthorityBoundariesBoundaryMatchKey(tc: AuthorityBoundariesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step)), vS("|"), erbTextOr(vStr(tc.forbidden_agent_kind)), vS("|"), erbTextOr(vStr(tc.forbidden_decision_kind))));
}

/** Computes the IsUntested calculated field.
 *  TRUE when a binding boundary has never been triggered by any recorded decision.
 *  Formula: =AND({{IsCurrentlyBinding}}, {{ViolationCount}} = 0) */
export function calcAuthorityBoundariesIsUntested(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_binding)), erbBool3(erbEq(vNum(tc.violation_count), vI(0)))));
}

/** Computes the HasRatifyingFragment calculated field.
 *  TRUE when this boundary names a knowledge fragment as its justification. FALSE means the rule constrains behaviour on nobody's recorded authority — strictly worse than resting on an expired claim, and previously invisible because the ratification lookup returned NULL.
 *  Formula: ={{RatifiedByKnowledgeFragment}} <> "" */
export function calcAuthorityBoundariesHasRatifyingFragment(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.ratified_by_knowledge_fragment)));
}

/** Computes the IsUnwarranted calculated field.
 *  TRUE when a binding constraint on authority rests on no ratifying claim at all, or on one that is no longer valid. Either way the rule is being enforced without a live justification.
 *  Formula: =AND({{IsCurrentlyBinding}}, OR(NOT({{HasRatifyingFragment}}), NOT({{RatifyingFragmentIsValid}}))) */
export function calcAuthorityBoundariesIsUnwarranted(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_binding)), erbBool3(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.has_ratifying_fragment)))), erbBool3(erbNot(erbBool3(vBool(tc.ratifying_fragment_is_valid))))))));
}

/** Computes the WarrantIsThin calculated field.
 *  A binding boundary whose ratifying knowledge is either overdue for review or single-sourced — still valid, but weakly warranted.
 *  Formula: =AND({{IsCurrentlyBinding}}, OR({{RatifyingFragmentIsOverdue}}, {{RatifyingFragmentIsSingleWitness}})) */
export function calcAuthorityBoundariesWarrantIsThin(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_binding)), erbBool3(erbOr(erbBool3(vBool(tc.ratifying_fragment_is_overdue)), erbBool3(vBool(tc.ratifying_fragment_is_single_witness))))));
}

/** Computes the IsUnwarrantedAndUntested calculated field.
 *  A boundary whose ratification has lapsed and which no agent decision has ever been evaluated against — we cannot show it works and we cannot show why it exists.
 *  Formula: =AND({{IsUnwarranted}}, {{IsUntested}}) */
export function calcAuthorityBoundariesIsUnwarrantedAndUntested(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unwarranted)), erbBool3(vBool(tc.is_untested))));
}

/** Computes the UnwarrantedBoundaryStepKey calculated field.
 *  Composite-key echo: the step this boundary governs when the boundary is unwarranted, blank otherwise.
 *  Formula: =IF({{IsUnwarranted}}, {{Step}}, "") */
export function calcAuthorityBoundariesUnwarrantedBoundaryStepKey(tc: AuthorityBoundariesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_unwarranted)), () => vStr(tc.step), () => vS("")));
}

/** Computes the RatifyingFragmentKey calculated field.
 *  Composite-key echo: the fragment ratifying this boundary when the boundary is currently binding, blank otherwise.
 *  Formula: =IF({{IsCurrentlyBinding}}, {{RatifiedByKnowledgeFragment}}, "") */
export function calcAuthorityBoundariesRatifyingFragmentKey(tc: AuthorityBoundariesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_currently_binding)), () => vStr(tc.ratified_by_knowledge_fragment), () => vS("")));
}

/** Computes the RatificationLapsed calculated field.
 *  TRUE when this boundary names a ratifying fragment and that fragment is no longer valid.
 *  Formula: =AND({{HasRatifyingFragment}}, NOT({{RatifyingFragmentIsValid}})) */
export function calcAuthorityBoundariesRatificationLapsed(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_ratifying_fragment)), erbBool3(erbNot(erbBool3(vBool(tc.ratifying_fragment_is_valid))))));
}

/** Computes the BindsDespiteLapsedRatification calculated field.
 *  TRUE when a boundary is still enforced against agents while the knowledge that authorized it has lapsed.
 *  Formula: =AND({{IsCurrentlyBinding}}, {{RatificationLapsed}}) */
export function calcAuthorityBoundariesBindsDespiteLapsedRatification(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_currently_binding)), erbBool3(vBool(tc.ratification_lapsed))));
}

/** Computes the IsUngroundedAndUntested calculated field.
 *  TRUE when a boundary has lapsed ratification AND has never once been exercised -- so neither its authority nor its operation has ever been demonstrated.
 *  Formula: =AND({{BindsDespiteLapsedRatification}}, {{IsUntested}}) */
export function calcAuthorityBoundariesIsUngroundedAndUntested(tc: AuthorityBoundariesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.binds_despite_lapsed_ratification)), erbBool3(vBool(tc.is_untested))));
}

/** Computes the ConstrainedRoleAssignmentKey calculated field.
 *  The role id this boundary constrains, emitted only when the boundary is ungrounded.
 *  Formula: =IF({{BindsDespiteLapsedRatification}}, {{AuthorityRole}}, "") */
export function calcAuthorityBoundariesConstrainedRoleAssignmentKey(tc: AuthorityBoundariesRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.binds_despite_lapsed_ratification)), () => vStr(tc.authority_role), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAuthorityBoundaries(tc: AuthorityBoundariesRow): AuthorityBoundariesRow {
  // Level 1
  calcGuard(tc, authorityBoundariesFieldTypes, "name", () => { tc.name = calcAuthorityBoundariesName(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "is_currently_binding", () => { tc.is_currently_binding = calcAuthorityBoundariesIsCurrentlyBinding(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "boundary_match_key", () => { tc.boundary_match_key = calcAuthorityBoundariesBoundaryMatchKey(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "has_ratifying_fragment", () => { tc.has_ratifying_fragment = calcAuthorityBoundariesHasRatifyingFragment(tc); });
  // Level 2
  calcGuard(tc, authorityBoundariesFieldTypes, "step_when_binding", () => { tc.step_when_binding = calcAuthorityBoundariesStepWhenBinding(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "is_untested", () => { tc.is_untested = calcAuthorityBoundariesIsUntested(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "is_unwarranted", () => { tc.is_unwarranted = calcAuthorityBoundariesIsUnwarranted(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "warrant_is_thin", () => { tc.warrant_is_thin = calcAuthorityBoundariesWarrantIsThin(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "ratifying_fragment_key", () => { tc.ratifying_fragment_key = calcAuthorityBoundariesRatifyingFragmentKey(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "ratification_lapsed", () => { tc.ratification_lapsed = calcAuthorityBoundariesRatificationLapsed(tc); });
  // Level 3
  calcGuard(tc, authorityBoundariesFieldTypes, "is_unwarranted_and_untested", () => { tc.is_unwarranted_and_untested = calcAuthorityBoundariesIsUnwarrantedAndUntested(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "unwarranted_boundary_step_key", () => { tc.unwarranted_boundary_step_key = calcAuthorityBoundariesUnwarrantedBoundaryStepKey(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "binds_despite_lapsed_ratification", () => { tc.binds_despite_lapsed_ratification = calcAuthorityBoundariesBindsDespiteLapsedRatification(tc); });
  // Level 4
  calcGuard(tc, authorityBoundariesFieldTypes, "is_ungrounded_and_untested", () => { tc.is_ungrounded_and_untested = calcAuthorityBoundariesIsUngroundedAndUntested(tc); });
  calcGuard(tc, authorityBoundariesFieldTypes, "constrained_role_assignment_key", () => { tc.constrained_role_assignment_key = calcAuthorityBoundariesConstrainedRoleAssignmentKey(tc); });
  return tc;
}

/** Reads AuthorityBoundaries rows from a JSON array file. */
export function loadAuthorityBoundariesRows(file: string): AuthorityBoundariesRow[] {
  return loadRows(file, { fields: authorityBoundariesFieldTypes }) as unknown as AuthorityBoundariesRow[];
}

// =============================================================================
// BINDINGOBSERVATIONS TABLE
// BindingObservations (added by witness loop 2).
// =============================================================================

/** A row in the BindingObservations table. */
export interface BindingObservationsRow {
  /** Primary key of an execution-side observation of one operational binding, frozen at the moment a step actually ran. */
  binding_observation_id: string;
  /** Human-readable calculated display alias for the BindingObservations row. */
  name: string | null;
  /** The step execution during which this binding was read. */
  step_execution: string | null;
  /** The spec-side binding this observation instantiates. */
  operational_binding: string | null;
  /** The LastObservedAt value the source actually carried at run time, captured then and never recomputed. */
  observed_source_timestamp: string | null;
  /** When the step execution actually read this binding. */
  read_at: string | null;
  /** The freshness SLA declared for this binding. */
  sla_minutes_at_run: number | null;
  /** How old the source data was at the instant the step read it. */
  age_at_run_minutes: number | null;
  /** TRUE when an authoritative source was already outside its SLA at the moment the step consumed it. */
  was_stale_at_run: boolean | null;
  /** Whether the binding observed is the authoritative source for its record key. */
  is_authoritative_binding: boolean | null;
  /** Echoes the step execution id when the source was stale at run time. */
  stale_at_run_step_key: string | null;
  _erb_errors?: Record<string, string>;
}

const bindingObservationsFieldTypes: Record<string, FieldType> = {
  binding_observation_id: "string",
  name: "*string",
  step_execution: "*string",
  operational_binding: "*string",
  observed_source_timestamp: "*string",
  read_at: "*string",
  sla_minutes_at_run: "*int",
  age_at_run_minutes: "*int",
  was_stale_at_run: "*bool",
  is_authoritative_binding: "*bool",
  stale_at_run_step_key: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the BindingObservations row.
 *  Formula: ={{StepExecution}} & " / " & {{BindingObservationId}} */
export function calcBindingObservationsName(tc: BindingObservationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.step_execution)), vS(" / "), erbTextOr(vStrPlain(tc.binding_observation_id))));
}

/** Computes the AgeAtRunMinutes calculated field.
 *  How old the source data was at the instant the step read it.
 *  Formula: =DATETIME_DIFF({{ReadAt}}, {{ObservedSourceTimestamp}}, "minutes") */
export function calcBindingObservationsAgeAtRunMinutes(tc: BindingObservationsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.read_at), vStr(tc.observed_source_timestamp), vS("minutes"))));
}

/** Computes the WasStaleAtRun calculated field.
 *  TRUE when an authoritative source was already outside its SLA at the moment the step consumed it.
 *  Formula: =AND({{IsAuthoritativeBinding}}, {{AgeAtRunMinutes}} > {{SlaMinutesAtRun}}) */
export function calcBindingObservationsWasStaleAtRun(tc: BindingObservationsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_authoritative_binding)), erbBool3(erbCmp(vInt(tc.age_at_run_minutes), ">", vInt(tc.sla_minutes_at_run)))));
}

/** Computes the StaleAtRunStepKey calculated field.
 *  Echoes the step execution id when the source was stale at run time.
 *  Formula: =IF({{WasStaleAtRun}}, {{StepExecution}}, "") */
export function calcBindingObservationsStaleAtRunStepKey(tc: BindingObservationsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.was_stale_at_run)), () => vStr(tc.step_execution), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeBindingObservations(tc: BindingObservationsRow): BindingObservationsRow {
  // Level 1
  calcGuard(tc, bindingObservationsFieldTypes, "name", () => { tc.name = calcBindingObservationsName(tc); });
  calcGuard(tc, bindingObservationsFieldTypes, "age_at_run_minutes", () => { tc.age_at_run_minutes = calcBindingObservationsAgeAtRunMinutes(tc); });
  // Level 2
  calcGuard(tc, bindingObservationsFieldTypes, "was_stale_at_run", () => { tc.was_stale_at_run = calcBindingObservationsWasStaleAtRun(tc); });
  // Level 3
  calcGuard(tc, bindingObservationsFieldTypes, "stale_at_run_step_key", () => { tc.stale_at_run_step_key = calcBindingObservationsStaleAtRunStepKey(tc); });
  return tc;
}

/** Reads BindingObservations rows from a JSON array file. */
export function loadBindingObservationsRows(file: string): BindingObservationsRow[] {
  return loadRows(file, { fields: bindingObservationsFieldTypes }) as unknown as BindingObservationsRow[];
}

// =============================================================================
// ATTESTATIONS TABLE
// Attestations (added by witness loop 2).
// =============================================================================

/** A row in the Attestations table. */
export interface AttestationsRow {
  /** Primary key of a signature event: one person, one execution, one instant. */
  attestation_id: string;
  /** Human-readable calculated display alias for the Attestations row. */
  name: string | null;
  /** The execution being attested to. */
  procedure_execution: string | null;
  /** The human who signed. */
  signed_by_agent: string | null;
  /** The instant of signature. */
  signed_at: string | null;
  /** The AssuranceGrade string as the model reported it at the moment of signature, captured then and never recomputed. */
  assurance_grade_at_signing: string | null;
  /** Whether the executed procedure version was fit to execute at the moment of signature. */
  version_was_fit_at_signing: boolean | null;
  /** Whether the executed version reads as fit today. */
  version_is_fit_now: boolean | null;
  /** TRUE when the fitness of the signed version reads differently today than it did at signature. */
  fitness_verdict_has_drifted: boolean | null;
  /** The AssuranceGrade the model reports for this execution today. */
  assurance_grade_now: string | null;
  /** TRUE when the assurance behind this signature is described differently now than it was at signature. */
  assurance_grade_has_drifted: boolean | null;
  /** TRUE when re-deriving this attestation today would not reproduce what the model said when it was signed. */
  would_not_survive_restatement: boolean | null;
  _erb_errors?: Record<string, string>;
}

const attestationsFieldTypes: Record<string, FieldType> = {
  attestation_id: "string",
  name: "*string",
  procedure_execution: "*string",
  signed_by_agent: "*string",
  signed_at: "*string",
  assurance_grade_at_signing: "*string",
  version_was_fit_at_signing: "*bool",
  version_is_fit_now: "*bool",
  fitness_verdict_has_drifted: "*bool",
  assurance_grade_now: "*string",
  assurance_grade_has_drifted: "*bool",
  would_not_survive_restatement: "*bool",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Attestations row.
 *  Formula: ={{ProcedureExecution}} & " / " & {{AttestationId}} */
export function calcAttestationsName(tc: AttestationsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.procedure_execution)), vS(" / "), erbTextOr(vStrPlain(tc.attestation_id))));
}

/** Computes the FitnessVerdictHasDrifted calculated field.
 *  TRUE when the fitness of the signed version reads differently today than it did at signature.
 *  Formula: =NOT({{VersionWasFitAtSigning}} = {{VersionIsFitNow}}) */
export function calcAttestationsFitnessVerdictHasDrifted(tc: AttestationsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(erbNullif(vBool(tc.version_was_fit_at_signing)), vBool(tc.version_is_fit_now)))));
}

/** Computes the AssuranceGradeHasDrifted calculated field.
 *  TRUE when the assurance behind this signature is described differently now than it was at signature.
 *  Formula: =NOT({{AssuranceGradeAtSigning}} = {{AssuranceGradeNow}}) */
export function calcAttestationsAssuranceGradeHasDrifted(tc: AttestationsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(erbEq(erbNullif(vStr(tc.assurance_grade_at_signing)), vStr(tc.assurance_grade_now)))));
}

/** Computes the WouldNotSurviveRestatement calculated field.
 *  TRUE when re-deriving this attestation today would not reproduce what the model said when it was signed.
 *  Formula: =OR({{FitnessVerdictHasDrifted}}, {{AssuranceGradeHasDrifted}}) */
export function calcAttestationsWouldNotSurviveRestatement(tc: AttestationsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(vBool(tc.fitness_verdict_has_drifted)), erbBool3(vBool(tc.assurance_grade_has_drifted))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAttestations(tc: AttestationsRow): AttestationsRow {
  // Level 1
  calcGuard(tc, attestationsFieldTypes, "name", () => { tc.name = calcAttestationsName(tc); });
  calcGuard(tc, attestationsFieldTypes, "fitness_verdict_has_drifted", () => { tc.fitness_verdict_has_drifted = calcAttestationsFitnessVerdictHasDrifted(tc); });
  calcGuard(tc, attestationsFieldTypes, "assurance_grade_has_drifted", () => { tc.assurance_grade_has_drifted = calcAttestationsAssuranceGradeHasDrifted(tc); });
  // Level 2
  calcGuard(tc, attestationsFieldTypes, "would_not_survive_restatement", () => { tc.would_not_survive_restatement = calcAttestationsWouldNotSurviveRestatement(tc); });
  return tc;
}

/** Reads Attestations rows from a JSON array file. */
export function loadAttestationsRows(file: string): AttestationsRow[] {
  return loadRows(file, { fields: attestationsFieldTypes }) as unknown as AttestationsRow[];
}

// =============================================================================
// APPROLEPROFILES TABLE
// One row per role, carrying how that role is presented: its accent colour, its 128x128 icon, and the login-card copy. Presentation is data, so the app never hardcodes a colour or a label per role.
// =============================================================================

/** A row in the AppRoleProfiles table. */
export interface AppRoleProfilesRow {
  /** Stored logical identifier for one AppRoleProfiles row. */
  app_role_profile_id: string;
  /** Human-readable calculated display alias for the AppRoleProfiles row. */
  name: string | null;
  /** The role this presentation profile describes. */
  role: string | null;
  /** Label shown on the login card and in the app chrome. */
  display_label: string | null;
  /** human or software. Drives the login-card silhouette and the icon plate shape. */
  role_kind: string | null;
  /** Hex accent colour. Distinctive per role; used for chrome, the left-nav active state, and the icon plate. */
  accent_color: string | null;
  /** Name of the geometric mark drawn on this role's icon. Colour is never the only signal — the mark discriminates in greyscale. */
  icon_mark: string | null;
  /** 128x128 PNG icon for this role, base64-encoded, no data: prefix. Rendered on the login card and beside the role name throughout the app. */
  icon_png_base64: string | null;
  /** One line of login-card copy: what this role opens the app to do, in that role's own terms. */
  pitch: string | null;
  /** Display order on the login screen. Human roles first, then software roles. */
  sort_order: number | null;
  /** How many routes this role has. */
  route_count: number | null;
  /** Semantic type IRI for this row. An extension: PKO does not model application presentation. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const appRoleProfilesFieldTypes: Record<string, FieldType> = {
  app_role_profile_id: "string",
  name: "*string",
  role: "*string",
  display_label: "*string",
  role_kind: "*string",
  accent_color: "*string",
  icon_mark: "*string",
  icon_png_base64: "*string",
  pitch: "*string",
  sort_order: "*float64",
  route_count: "*float64",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AppRoleProfiles row.
 *  Formula: ={{DisplayLabel}} & " (" & {{RoleKind}} & ")" */
export function calcAppRoleProfilesName(tc: AppRoleProfilesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.display_label)), vS(" ("), erbTextOr(vStr(tc.role_kind)), vS(")")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAppRoleProfiles(tc: AppRoleProfilesRow): AppRoleProfilesRow {
  // Level 1
  calcGuard(tc, appRoleProfilesFieldTypes, "name", () => { tc.name = calcAppRoleProfilesName(tc); });
  return tc;
}

/** Reads AppRoleProfiles rows from a JSON array file. */
export function loadAppRoleProfilesRows(file: string): AppRoleProfilesRow[] {
  return loadRows(file, { fields: appRoleProfilesFieldTypes }) as unknown as AppRoleProfilesRow[];
}

// =============================================================================
// APPNAVGROUPS TABLE
// The left-navigation section headers. A route names the group it appears under; the nav renders the groups its active role actually uses.
// =============================================================================

/** A row in the AppNavGroups table. */
export interface AppNavGroupsRow {
  /** Stored logical identifier for one AppNavGroups row. */
  app_nav_group_id: string;
  /** Human-readable calculated display alias for the AppNavGroups row. */
  name: string | null;
  /** Section header text rendered in the left navigation. */
  group_label: string | null;
  /** How many routes sit under this nav group across all roles. */
  route_count: number | null;
  /** Semantic type IRI for this row. An extension: PKO does not model navigation. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const appNavGroupsFieldTypes: Record<string, FieldType> = {
  app_nav_group_id: "string",
  name: "*string",
  group_label: "*string",
  route_count: "*float64",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AppNavGroups row.
 *  Formula: ={{GroupLabel}} */
export function calcAppNavGroupsName(tc: AppNavGroupsRow): string | null {
  return toStringPtr(vStr(tc.group_label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAppNavGroups(tc: AppNavGroupsRow): AppNavGroupsRow {
  // Level 1
  calcGuard(tc, appNavGroupsFieldTypes, "name", () => { tc.name = calcAppNavGroupsName(tc); });
  return tc;
}

/** Reads AppNavGroups rows from a JSON array file. */
export function loadAppNavGroupsRows(file: string): AppNavGroupsRow[] {
  return loadRows(file, { fields: appNavGroupsFieldTypes }) as unknown as AppNavGroupsRow[];
}

// =============================================================================
// APPROUTES TABLE
// One row per screen in the role-navigated application, as /{role}/{dashboard}/{entity...}. Each carries its purpose in the owning role's voice and brief layout hints, so a later build session has the brief without re-deriving it. Shared detail routes have no owning role.
// =============================================================================

/** A row in the AppRoutes table. */
export interface AppRoutesRow {
  /** Stored logical identifier for one AppRoutes row. */
  app_route_id: string;
  /** Human-readable calculated display alias for the AppRoutes row. */
  name: string | null;
  /** The URL path, /{role}/{dashboard}/{entity...}. Shared detail routes live under /shared/ because the same entity serves every role. */
  route_path: string;
  /** Short label. This is the left-nav text. */
  route_name: string | null;
  /** domain | maintainer. Domain routes belong to the twelve accountable Roles. Maintainer routes are the model's own instrumentation (Admin, Explorer) and deliberately have no owning role — 'admin' is not a role anyone is accountable in. */
  surface: string;
  /** The role whose navigation contains this route. Null for shared routes reachable from several roles. */
  owning_role: string | null;
  /** Left-nav section this route appears under. Null for detail routes reached by drill-down rather than from the nav. */
  nav_group: string | null;
  /** Sort order within the nav group. */
  nav_order: number | null;
  /** dashboard | workspace | detail | index | action. Detail routes carry path parameters and are not in the nav. */
  route_kind: string | null;
  /** What the role does here, in that role's own voice. This is the brief for the build session. */
  purpose: string | null;
  /** Brief layout direction: the shape of the screen, what leads, and what must not be collapsed or implied. */
  layout_hints: string | null;
  /** Whether this route appears in the left navigation. Detail routes do not. */
  is_in_nav: boolean | null;
  /** Whether this route is shared across domain roles rather than owned by one. A maintainer route is not shared — it belongs to a different surface entirely. */
  is_shared: boolean | null;
  /** Whether this route is model instrumentation rather than a domain workspace. Maintainer routes are reached from the login page's maintainer section, not from a role card. */
  is_maintainer: boolean | null;
  /** How many role questions this route helps answer. */
  question_count: number | null;
  /** How many other routes this route links to. */
  reference_count: number | null;
  /** A domain route owned by a role that answers no role question. Not automatically wrong, but it should be justified. Maintainer routes are excluded: they answer questions about the model itself, which are not RoleQuestions and must not be fabricated as such. */
  answers_no_question: boolean | null;
  /** Semantic type IRI for this row. An extension: PKO models procedures, not the applications that display them. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const appRoutesFieldTypes: Record<string, FieldType> = {
  app_route_id: "string",
  name: "*string",
  route_path: "string",
  route_name: "*string",
  surface: "string",
  owning_role: "*string",
  nav_group: "*string",
  nav_order: "*float64",
  route_kind: "*string",
  purpose: "*string",
  layout_hints: "*string",
  is_in_nav: "*bool",
  is_shared: "*bool",
  is_maintainer: "*bool",
  question_count: "*float64",
  reference_count: "*float64",
  answers_no_question: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AppRoutes row.
 *  Formula: ={{RouteName}} & " — " & {{RoutePath}} */
export function calcAppRoutesName(tc: AppRoutesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.route_name)), vS(" — "), erbTextOr(vStrPlain(tc.route_path))));
}

/** Computes the IsInNav calculated field.
 *  Whether this route appears in the left navigation. Detail routes do not.
 *  Formula: ={{NavGroup}} <> "" */
export function calcAppRoutesIsInNav(tc: AppRoutesRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.nav_group)));
}

/** Computes the IsShared calculated field.
 *  Whether this route is shared across domain roles rather than owned by one. A maintainer route is not shared — it belongs to a different surface entirely.
 *  Formula: =AND({{OwningRole}} = "", {{Surface}} = "domain") */
export function calcAppRoutesIsShared(tc: AppRoutesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsBlank(vStr(tc.owning_role))), erbBool3(erbEq(erbNullif(vStrPlain(tc.surface)), vS("domain")))));
}

/** Computes the IsMaintainer calculated field.
 *  Whether this route is model instrumentation rather than a domain workspace. Maintainer routes are reached from the login page's maintainer section, not from a role card.
 *  Formula: ={{Surface}} = "maintainer" */
export function calcAppRoutesIsMaintainer(tc: AppRoutesRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStrPlain(tc.surface)), vS("maintainer")));
}

/** Computes the AnswersNoQuestion calculated field.
 *  A domain route owned by a role that answers no role question. Not automatically wrong, but it should be justified. Maintainer routes are excluded: they answer questions about the model itself, which are not RoleQuestions and must not be fabricated as such.
 *  Formula: =AND({{QuestionCount}} = 0, {{IsShared}} = FALSE, {{IsMaintainer}} = FALSE, {{RouteKind}} <> "index") */
export function calcAppRoutesAnswersNoQuestion(tc: AppRoutesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(vNum(tc.question_count), vI(0))), erbBool3(erbEq(vBool(tc.is_shared), vB(false))), erbBool3(erbEq(vBool(tc.is_maintainer), vB(false))), erbBool3(erbNe(erbNullif(vStr(tc.route_kind)), vS("index")))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAppRoutes(tc: AppRoutesRow): AppRoutesRow {
  // Level 1
  calcGuard(tc, appRoutesFieldTypes, "name", () => { tc.name = calcAppRoutesName(tc); });
  calcGuard(tc, appRoutesFieldTypes, "is_in_nav", () => { tc.is_in_nav = calcAppRoutesIsInNav(tc); });
  calcGuard(tc, appRoutesFieldTypes, "is_shared", () => { tc.is_shared = calcAppRoutesIsShared(tc); });
  calcGuard(tc, appRoutesFieldTypes, "is_maintainer", () => { tc.is_maintainer = calcAppRoutesIsMaintainer(tc); });
  // Level 2
  calcGuard(tc, appRoutesFieldTypes, "answers_no_question", () => { tc.answers_no_question = calcAppRoutesAnswersNoQuestion(tc); });
  return tc;
}

/** Reads AppRoutes rows from a JSON array file. */
export function loadAppRoutesRows(file: string): AppRoutesRow[] {
  return loadRows(file, { fields: appRoutesFieldTypes }) as unknown as AppRoutesRow[];
}

// =============================================================================
// APPROUTEQUESTIONS TABLE
// Junction: which RoleQuestions each route helps answer. Deliberately many-to-many — a question is answered across several routes and a route serves several questions. It is not, and should not become, 1:1.
// =============================================================================

/** A row in the AppRouteQuestions table. */
export interface AppRouteQuestionsRow {
  /** Stored logical identifier for one AppRouteQuestions row. */
  app_route_question_id: string;
  /** Human-readable calculated display alias for the AppRouteQuestions row. */
  name: string | null;
  /** The route that helps answer the question. */
  route: string | null;
  /** The role question this route helps answer. */
  question: string | null;
  /** Semantic type IRI for this row. An extension. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const appRouteQuestionsFieldTypes: Record<string, FieldType> = {
  app_route_question_id: "string",
  name: "*string",
  route: "*string",
  question: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AppRouteQuestions row.
 *  Formula: ={{Route}} & " answers " & {{Question}} */
export function calcAppRouteQuestionsName(tc: AppRouteQuestionsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.route)), vS(" answers "), erbTextOr(vStr(tc.question))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAppRouteQuestions(tc: AppRouteQuestionsRow): AppRouteQuestionsRow {
  // Level 1
  calcGuard(tc, appRouteQuestionsFieldTypes, "name", () => { tc.name = calcAppRouteQuestionsName(tc); });
  return tc;
}

/** Reads AppRouteQuestions rows from a JSON array file. */
export function loadAppRouteQuestionsRows(file: string): AppRouteQuestionsRow[] {
  return loadRows(file, { fields: appRouteQuestionsFieldTypes }) as unknown as AppRouteQuestionsRow[];
}

// =============================================================================
// APPROUTEREFERENCES TABLE
// Junction: which other routes a route links to. This is the navigation graph between screens, kept as rows rather than embedded lists so the canonical model stays a DAG.
// =============================================================================

/** A row in the AppRouteReferences table. */
export interface AppRouteReferencesRow {
  /** Stored logical identifier for one AppRouteReferences row. */
  app_route_reference_id: string;
  /** Human-readable calculated display alias for the AppRouteReferences row. */
  name: string | null;
  /** The route that links out. */
  from_route: string | null;
  /** The route being linked to. */
  to_route: string | null;
  /** Semantic type IRI for this row. An extension. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const appRouteReferencesFieldTypes: Record<string, FieldType> = {
  app_route_reference_id: "string",
  name: "*string",
  from_route: "*string",
  to_route: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the AppRouteReferences row.
 *  Formula: ={{FromRoute}} & " -> " & {{ToRoute}} */
export function calcAppRouteReferencesName(tc: AppRouteReferencesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.from_route)), vS(" -> "), erbTextOr(vStr(tc.to_route))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAppRouteReferences(tc: AppRouteReferencesRow): AppRouteReferencesRow {
  // Level 1
  calcGuard(tc, appRouteReferencesFieldTypes, "name", () => { tc.name = calcAppRouteReferencesName(tc); });
  return tc;
}

/** Reads AppRouteReferences rows from a JSON array file. */
export function loadAppRouteReferencesRows(file: string): AppRouteReferencesRow[] {
  return loadRows(file, { fields: appRouteReferencesFieldTypes }) as unknown as AppRouteReferencesRow[];
}

// =============================================================================
// RULEBOOKTABLES TABLE
// Census of every table in this rulebook. The table-level counterpart to RulebookFields, and the anchor every access policy points at. Derived by tools/reconcile_field_catalog.py -- never hand-maintained.
// =============================================================================

/** A row in the RulebookTables table. */
export interface RulebookTablesRow {
  /** Primary key: the rulebook table name, verbatim. rulebook-to-postgres synthesizes a lowercased slug PK for a table with no <Entity>Id and rewrites every FK to it, which broke each formula comparing an FK to TableName. */
  rulebook_table_id: string;
  /** Stored logical identifier: the rulebook table name, e.g. 'Procedures'. */
  table_name: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** snake_case Postgres base table name emitted by rulebook-to-postgres. */
  physical_table: string | null;
  /** snake_case Postgres view name (vw_*) carrying the computed columns. */
  physical_view: string | null;
  /** Coarse grouping used to organise role schemas, e.g. 'execution', 'governance'. */
  subject_area: string | null;
  /** True when this table is an ERB extension rather than a native/aligned PKO term. */
  is_extension: boolean | null;
  /** Number of catalogued fields on this table. */
  field_count: number | null;
  /** Number of access policies targeting this table. */
  policy_count: number | null;
  /** True when RLS is enabled but no policy targets the table, so every principal sees zero rows. A fail-closed table nobody has granted access to. */
  is_unsecured: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const rulebookTablesFieldTypes: Record<string, FieldType> = {
  rulebook_table_id: "string",
  table_name: "string",
  name: "*string",
  physical_table: "*string",
  physical_view: "*string",
  subject_area: "*string",
  is_extension: "*bool",
  field_count: "*float64",
  policy_count: "*float64",
  is_unsecured: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{TableName}} */
export function calcRulebookTablesName(tc: RulebookTablesRow): string | null {
  return toStringPtr(vStrPlain(tc.table_name));
}

/** Computes the IsUnsecured calculated field.
 *  True when RLS is enabled but no policy targets the table, so every principal sees zero rows. A fail-closed table nobody has granted access to.
 *  Formula: ={{PolicyCount}} = 0 */
export function calcRulebookTablesIsUnsecured(tc: RulebookTablesRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.policy_count), vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRulebookTables(tc: RulebookTablesRow): RulebookTablesRow {
  // Level 1
  calcGuard(tc, rulebookTablesFieldTypes, "name", () => { tc.name = calcRulebookTablesName(tc); });
  calcGuard(tc, rulebookTablesFieldTypes, "is_unsecured", () => { tc.is_unsecured = calcRulebookTablesIsUnsecured(tc); });
  return tc;
}

/** Reads RulebookTables rows from a JSON array file. */
export function loadRulebookTablesRows(file: string): RulebookTablesRow[] {
  return loadRows(file, { fields: rulebookTablesFieldTypes }) as unknown as RulebookTablesRow[];
}

// =============================================================================
// ACCESSPRINCIPALS TABLE
// Security principals -- the identities policies attach to. A principal is the console persona a person logs in as; it maps many-to-one onto a domain Role, so 'who may see this row' is expressed once against the domain vocabulary while the UI keeps its own persona names. Each principal owns exactly one Postgres role and one Postgres schema.
// =============================================================================

/** A row in the AccessPrincipals table. */
export interface AccessPrincipalsRow {
  /** Stored logical identifier, e.g. 'principal-controller'. */
  access_principal_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Display label shown in the console role picker. */
  label: string | null;
  /** Domain role whose authority this principal exercises. */
  domain_role: string | null;
  /** Postgres role name this principal authenticates as, e.g. 'pko_controller'. */
  pg_role_name: string | null;
  /** Postgres schema that is this principal's entire visible world, e.g. 'pko_controller'. */
  schema_name: string | null;
  /** True when this principal may read every table and edit access policy. */
  is_administrator: boolean | null;
  /** Organization inherited from the domain role; the default tenancy boundary for row predicates. */
  organization_scope: string | null;
  /** Label of the domain role, for display. */
  role_label: string | null;
  /** Number of row policies granted to this principal. */
  policy_count: number | null;
  /** Number of field grants held by this principal. */
  grant_count: number | null;
  /** Number of tables exposed in this principal's schema. */
  visible_table_count: number | null;
  /** True when the principal holds no policies at all, so its schema is empty and it can read nothing. Fail-closed by construction. */
  has_no_access: boolean | null;
  /** True when a non-administrator principal can reach every table in the rulebook -- an admin-equivalent principal that was never declared as one. */
  is_over_privileged: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const accessPrincipalsFieldTypes: Record<string, FieldType> = {
  access_principal_id: "string",
  name: "*string",
  label: "*string",
  domain_role: "*string",
  pg_role_name: "*string",
  schema_name: "*string",
  is_administrator: "*bool",
  organization_scope: "*string",
  role_label: "*string",
  policy_count: "*float64",
  grant_count: "*float64",
  visible_table_count: "*float64",
  has_no_access: "*bool",
  is_over_privileged: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{Label}} */
export function calcAccessPrincipalsName(tc: AccessPrincipalsRow): string | null {
  return toStringPtr(vStr(tc.label));
}

/** Computes the HasNoAccess calculated field.
 *  True when the principal holds no policies at all, so its schema is empty and it can read nothing. Fail-closed by construction.
 *  Formula: ={{PolicyCount}} = 0 */
export function calcAccessPrincipalsHasNoAccess(tc: AccessPrincipalsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.policy_count), vI(0)));
}

/** Computes the IsOverPrivileged calculated field.
 *  True when a non-administrator principal can reach every table in the rulebook -- an admin-equivalent principal that was never declared as one.
 *  Formula: =AND(NOT({{IsAdministrator}}), {{VisibleTableCount}} >= 74) */
export function calcAccessPrincipalsIsOverPrivileged(tc: AccessPrincipalsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbIsTrue(vBool(tc.is_administrator)))), erbBool3(erbCmp(vNum(tc.visible_table_count), ">=", vI(74)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAccessPrincipals(tc: AccessPrincipalsRow): AccessPrincipalsRow {
  // Level 1
  calcGuard(tc, accessPrincipalsFieldTypes, "name", () => { tc.name = calcAccessPrincipalsName(tc); });
  calcGuard(tc, accessPrincipalsFieldTypes, "has_no_access", () => { tc.has_no_access = calcAccessPrincipalsHasNoAccess(tc); });
  calcGuard(tc, accessPrincipalsFieldTypes, "is_over_privileged", () => { tc.is_over_privileged = calcAccessPrincipalsIsOverPrivileged(tc); });
  return tc;
}

/** Reads AccessPrincipals rows from a JSON array file. */
export function loadAccessPrincipalsRows(file: string): AccessPrincipalsRow[] {
  return loadRows(file, { fields: accessPrincipalsFieldTypes }) as unknown as AccessPrincipalsRow[];
}

// =============================================================================
// ACCESSPOLICIES TABLE
// Row-level security policies: the VERTICAL cut. One row per principal x table x command, carrying the predicate that decides which rows are visible. RowPredicate is emitted verbatim into a Postgres USING clause, so it may call any SECURITY DEFINER calc_* function and therefore reference inference fields many hops down the DAG.
// =============================================================================

/** A row in the AccessPolicies table. */
export interface AccessPoliciesRow {
  /** Stored logical identifier, e.g. 'pol-controller-procedures-select'. */
  access_policy_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Principal this policy grants to. */
  principal: string | null;
  /** Rulebook table this policy guards. */
  target_table: string | null;
  /** SQL command the policy governs: SELECT, INSERT, UPDATE, DELETE or ALL. */
  command: string | null;
  /** SQL boolean expression emitted into USING(...). Empty means all rows of the table. Must not sub-select the guarded table -- Postgres raises infinite recursion; route such predicates through a SECURITY DEFINER function instead. */
  row_predicate: string | null;
  /** SQL boolean expression emitted into WITH CHECK(...) for write commands. Empty reuses RowPredicate. */
  check_predicate: string | null;
  /** Why this principal is entitled to these rows, in the granting authority's words. */
  rationale: string | null;
  /** True when RowPredicate calls a calc_* function, i.e. the cut depends on a derived field rather than a stored column. */
  references_inference: boolean | null;
  /** True when this policy governs a mutating command. */
  is_write_command: boolean | null;
  /** True when the policy carries no predicate, exposing every row of the target table to the principal. */
  is_unrestricted: boolean | null;
  /** Whether the granted principal is an administrator. */
  principal_is_admin: boolean | null;
  /** True when a non-administrator principal is granted an unrestricted policy -- a whole-table exposure that no row predicate narrows. The single highest-signal privilege-escalation witness in the model. */
  is_unrestricted_non_admin_grant: boolean | null;
  /** True when a write policy has no denial test proving it refuses out-of-scope rows. An untested write grant is an assertion, not evidence. */
  is_unwitnessed_write: boolean | null;
  /** Number of denial tests seeded against this policy. */
  denial_test_count: number | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const accessPoliciesFieldTypes: Record<string, FieldType> = {
  access_policy_id: "string",
  name: "*string",
  principal: "*string",
  target_table: "*string",
  command: "*string",
  row_predicate: "*string",
  check_predicate: "*string",
  rationale: "*string",
  references_inference: "*bool",
  is_write_command: "*bool",
  is_unrestricted: "*bool",
  principal_is_admin: "*bool",
  is_unrestricted_non_admin_grant: "*bool",
  is_unwitnessed_write: "*bool",
  denial_test_count: "*float64",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{Principal}} & " " & {{Command}} & " " & {{TargetTable}} */
export function calcAccessPoliciesName(tc: AccessPoliciesRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.principal)), vS(" "), erbTextOr(vStr(tc.command)), vS(" "), erbTextOr(vStr(tc.target_table))));
}

/** Computes the IsWriteCommand calculated field.
 *  True when this policy governs a mutating command.
 *  Formula: =OR({{Command}} = "INSERT", {{Command}} = "UPDATE", {{Command}} = "DELETE", {{Command}} = "ALL") */
export function calcAccessPoliciesIsWriteCommand(tc: AccessPoliciesRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(erbNullif(vStr(tc.command)), vS("INSERT"))), erbBool3(erbEq(erbNullif(vStr(tc.command)), vS("UPDATE"))), erbBool3(erbEq(erbNullif(vStr(tc.command)), vS("DELETE"))), erbBool3(erbEq(erbNullif(vStr(tc.command)), vS("ALL")))));
}

/** Computes the IsUnrestricted calculated field.
 *  True when the policy carries no predicate, exposing every row of the target table to the principal.
 *  Formula: ={{RowPredicate}} = "" */
export function calcAccessPoliciesIsUnrestricted(tc: AccessPoliciesRow): boolean | null {
  return toBoolPtr(erbIsBlank(vStr(tc.row_predicate)));
}

/** Computes the IsUnrestrictedNonAdminGrant calculated field.
 *  True when a non-administrator principal is granted an unrestricted policy -- a whole-table exposure that no row predicate narrows. The single highest-signal privilege-escalation witness in the model.
 *  Formula: =AND({{IsUnrestricted}}, NOT({{PrincipalIsAdmin}})) */
export function calcAccessPoliciesIsUnrestrictedNonAdminGrant(tc: AccessPoliciesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_unrestricted)), erbBool3(erbNot(erbBool3(vBool(tc.principal_is_admin))))));
}

/** Computes the IsUnwitnessedWrite calculated field.
 *  True when a write policy has no denial test proving it refuses out-of-scope rows. An untested write grant is an assertion, not evidence.
 *  Formula: =AND({{IsWriteCommand}}, {{DenialTestCount}} = 0) */
export function calcAccessPoliciesIsUnwitnessedWrite(tc: AccessPoliciesRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_write_command)), erbBool3(erbEq(vNum(tc.denial_test_count), vI(0)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAccessPolicies(tc: AccessPoliciesRow): AccessPoliciesRow {
  // Level 1
  calcGuard(tc, accessPoliciesFieldTypes, "name", () => { tc.name = calcAccessPoliciesName(tc); });
  calcGuard(tc, accessPoliciesFieldTypes, "is_write_command", () => { tc.is_write_command = calcAccessPoliciesIsWriteCommand(tc); });
  calcGuard(tc, accessPoliciesFieldTypes, "is_unrestricted", () => { tc.is_unrestricted = calcAccessPoliciesIsUnrestricted(tc); });
  // Level 2
  calcGuard(tc, accessPoliciesFieldTypes, "is_unrestricted_non_admin_grant", () => { tc.is_unrestricted_non_admin_grant = calcAccessPoliciesIsUnrestrictedNonAdminGrant(tc); });
  calcGuard(tc, accessPoliciesFieldTypes, "is_unwitnessed_write", () => { tc.is_unwitnessed_write = calcAccessPoliciesIsUnwitnessedWrite(tc); });
  return tc;
}

/** Reads AccessPolicies rows from a JSON array file. */
export function loadAccessPoliciesRows(file: string): AccessPoliciesRow[] {
  return loadRows(file, { fields: accessPoliciesFieldTypes }) as unknown as AccessPoliciesRow[];
}

// =============================================================================
// FIELDGRANTS TABLE
// Field-level grants: the HORIZONTAL cut. One row per principal x field. A field with no grant row is not filtered from the principal's view -- it is absent from it, so the column does not exist as far as that principal's SQL is concerned.
// =============================================================================

/** A row in the FieldGrants table. */
export interface FieldGrantsRow {
  /** Stored logical identifier, e.g. 'fg-controller-Procedures.Title'. */
  field_grant_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Principal receiving the grant. */
  principal: string | null;
  /** Catalogued field being exposed. */
  target_field: string | null;
  /** True when the principal may read this field. */
  can_read: boolean | null;
  /** True when the principal may write this field. */
  can_write: boolean | null;
  /** How the value is presented when read: 'plain', 'redacted' or 'hashed'. */
  mask_strategy: string | null;
  /** Table the granted field belongs to. */
  field_table: string | null;
  /** Name of the granted field. */
  field_name: string | null;
  /** Whether the granted field is a derived (calculated/lookup/aggregation) field. */
  field_is_derived: boolean | null;
  /** True when a derived field has been granted write access. Derived fields are computed by the substrate and cannot be written -- such a grant is incoherent and must be corrected. */
  is_writable_derived_field: boolean | null;
  /** True when the value is transformed rather than shown verbatim. */
  is_masked: boolean | null;
  /** Composite echo of principal and table, blank unless readable. Enables single-criterion COUNTIFS rollups of readable columns per principal per table, per the documented multi-criteria COUNTIFS defect. */
  grant_key_when_readable: string | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const fieldGrantsFieldTypes: Record<string, FieldType> = {
  field_grant_id: "string",
  name: "*string",
  principal: "*string",
  target_field: "*string",
  can_read: "*bool",
  can_write: "*bool",
  mask_strategy: "*string",
  field_table: "*string",
  field_name: "*string",
  field_is_derived: "*bool",
  is_writable_derived_field: "*bool",
  is_masked: "*bool",
  grant_key_when_readable: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{Principal}} & " -> " & {{TargetField}} */
export function calcFieldGrantsName(tc: FieldGrantsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.principal)), vS(" -> "), erbTextOr(vStr(tc.target_field))));
}

/** Computes the IsWritableDerivedField calculated field.
 *  True when a derived field has been granted write access. Derived fields are computed by the substrate and cannot be written -- such a grant is incoherent and must be corrected.
 *  Formula: =AND({{CanWrite}}, {{FieldIsDerived}}) */
export function calcFieldGrantsIsWritableDerivedField(tc: FieldGrantsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBool(tc.can_write)), erbBool3(vBool(tc.field_is_derived))));
}

/** Computes the IsMasked calculated field.
 *  True when the value is transformed rather than shown verbatim.
 *  Formula: =AND({{MaskStrategy}} <> "plain", {{MaskStrategy}} <> "") */
export function calcFieldGrantsIsMasked(tc: FieldGrantsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNe(erbNullif(vStr(tc.mask_strategy)), vS("plain"))), erbBool3(erbIsNotBlank(vStr(tc.mask_strategy)))));
}

/** Computes the GrantKeyWhenReadable calculated field.
 *  Composite echo of principal and table, blank unless readable. Enables single-criterion COUNTIFS rollups of readable columns per principal per table, per the documented multi-criteria COUNTIFS defect.
 *  Formula: =IF({{CanRead}}, {{Principal}} & "|" & {{FieldTable}}, "") */
export function calcFieldGrantsGrantKeyWhenReadable(tc: FieldGrantsRow): string | null {
  return toStringPtr(erbIf(erbIsTrue(vBool(tc.can_read)), () => erbConcat(erbTextOr(vStr(tc.principal)), vS("|"), erbTextOr(vStr(tc.field_table))), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeFieldGrants(tc: FieldGrantsRow): FieldGrantsRow {
  // Level 1
  calcGuard(tc, fieldGrantsFieldTypes, "name", () => { tc.name = calcFieldGrantsName(tc); });
  calcGuard(tc, fieldGrantsFieldTypes, "is_writable_derived_field", () => { tc.is_writable_derived_field = calcFieldGrantsIsWritableDerivedField(tc); });
  calcGuard(tc, fieldGrantsFieldTypes, "is_masked", () => { tc.is_masked = calcFieldGrantsIsMasked(tc); });
  calcGuard(tc, fieldGrantsFieldTypes, "grant_key_when_readable", () => { tc.grant_key_when_readable = calcFieldGrantsGrantKeyWhenReadable(tc); });
  return tc;
}

/** Reads FieldGrants rows from a JSON array file. */
export function loadFieldGrantsRows(file: string): FieldGrantsRow[] {
  return loadRows(file, { fields: fieldGrantsFieldTypes }) as unknown as FieldGrantsRow[];
}

// =============================================================================
// ROLESCHEMAS TABLE
// One Postgres schema per principal -- the principal's entire visible world. The schema is the only entry on that principal's search_path, so a table absent from it cannot be named at all.
// =============================================================================

/** A row in the RoleSchemas table. */
export interface RoleSchemasRow {
  /** Stored logical identifier, e.g. 'schema-controller'. */
  role_schema_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Principal that owns this schema. */
  principal: string | null;
  /** Postgres schema name, e.g. 'pko_controller'. */
  schema_name: string | null;
  /** search_path set for this principal's sessions. The principal's own schema only -- public is deliberately excluded so base tables cannot be named. */
  search_path: string | null;
  /** True when the principal may not create objects in its own schema. */
  is_sealed: boolean | null;
  /** Number of views exposed in this schema. */
  view_count: number | null;
  /** True when the schema exposes no views, so the principal can read nothing at all. */
  is_empty_schema: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const roleSchemasFieldTypes: Record<string, FieldType> = {
  role_schema_id: "string",
  name: "*string",
  principal: "*string",
  schema_name: "*string",
  search_path: "*string",
  is_sealed: "*bool",
  view_count: "*float64",
  is_empty_schema: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{SchemaName}} */
export function calcRoleSchemasName(tc: RoleSchemasRow): string | null {
  return toStringPtr(vStr(tc.schema_name));
}

/** Computes the SearchPath calculated field.
 *  search_path set for this principal's sessions. The principal's own schema only -- public is deliberately excluded so base tables cannot be named.
 *  Formula: ={{SchemaName}} */
export function calcRoleSchemasSearchPath(tc: RoleSchemasRow): string | null {
  return toStringPtr(vStr(tc.schema_name));
}

/** Computes the IsEmptySchema calculated field.
 *  True when the schema exposes no views, so the principal can read nothing at all.
 *  Formula: ={{ViewCount}} = 0 */
export function calcRoleSchemasIsEmptySchema(tc: RoleSchemasRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.view_count), vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoleSchemas(tc: RoleSchemasRow): RoleSchemasRow {
  // Level 1
  calcGuard(tc, roleSchemasFieldTypes, "name", () => { tc.name = calcRoleSchemasName(tc); });
  calcGuard(tc, roleSchemasFieldTypes, "search_path", () => { tc.search_path = calcRoleSchemasSearchPath(tc); });
  calcGuard(tc, roleSchemasFieldTypes, "is_empty_schema", () => { tc.is_empty_schema = calcRoleSchemasIsEmptySchema(tc); });
  return tc;
}

/** Reads RoleSchemas rows from a JSON array file. */
export function loadRoleSchemasRows(file: string): RoleSchemasRow[] {
  return loadRows(file, { fields: roleSchemasFieldTypes }) as unknown as RoleSchemasRow[];
}

// =============================================================================
// ROLESCHEMAVIEWS TABLE
// The emitted views: one per principal x table. ColumnList is DERIVED from FieldGrants, so toggling a single grant changes the emitted DDL with no second edit anywhere. This is what makes an admin's save reshape the database without touching UI code.
// =============================================================================

/** A row in the RoleSchemaViews table. */
export interface RoleSchemaViewsRow {
  /** Stored logical identifier, e.g. 'rsv-controller-procedures'. */
  role_schema_view_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Schema this view is emitted into. */
  role_schema: string | null;
  /** Principal that will read this view. */
  principal: string | null;
  /** Rulebook table this view exposes. */
  target_table: string | null;
  /** Unqualified view name inside the principal's schema, e.g. 'procedures'. */
  view_name: string | null;
  /** Schema name, from the owning RoleSchemas row. */
  schema_name: string | null;
  /** Underlying computed view this narrows, e.g. 'vw_procedures'. */
  source_view: string | null;
  /** Composite key matching FieldGrants.GrantKeyWhenReadable, used to roll up this view's readable column count. */
  grant_key: string | null;
  /** Number of columns exposed, derived live from the principal's readable field grants. Change one grant and this view's shape changes. */
  column_count: number | null;
  /** Total catalogued fields on the target table. */
  table_field_count: number | null;
  /** True when every field on the table is exposed, so the horizontal cut removes nothing. */
  is_full_width: boolean | null;
  /** True when the view exposes zero columns -- an emitted view that cannot be selected from. A generator that emits this has produced invalid DDL. */
  is_degenerate_view: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const roleSchemaViewsFieldTypes: Record<string, FieldType> = {
  role_schema_view_id: "string",
  name: "*string",
  role_schema: "*string",
  principal: "*string",
  target_table: "*string",
  view_name: "*string",
  schema_name: "*string",
  source_view: "*string",
  grant_key: "*string",
  column_count: "*float64",
  table_field_count: "*float64",
  is_full_width: "*bool",
  is_degenerate_view: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{SchemaName}} & "." & {{ViewName}} */
export function calcRoleSchemaViewsName(tc: RoleSchemaViewsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.schema_name)), vS("."), erbTextOr(vStr(tc.view_name))));
}

/** Computes the GrantKey calculated field.
 *  Composite key matching FieldGrants.GrantKeyWhenReadable, used to roll up this view's readable column count.
 *  Formula: ={{Principal}} & "|" & {{TargetTable}} */
export function calcRoleSchemaViewsGrantKey(tc: RoleSchemaViewsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.principal)), vS("|"), erbTextOr(vStr(tc.target_table))));
}

/** Computes the IsFullWidth calculated field.
 *  True when every field on the table is exposed, so the horizontal cut removes nothing.
 *  Formula: =AND({{ColumnCount}} > 0, {{ColumnCount}} >= {{TableFieldCount}}) */
export function calcRoleSchemaViewsIsFullWidth(tc: RoleSchemaViewsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbCmp(vNum(tc.column_count), ">", vI(0))), erbBool3(erbCmp(vNum(tc.column_count), ">=", vNum(tc.table_field_count)))));
}

/** Computes the IsDegenerateView calculated field.
 *  True when the view exposes zero columns -- an emitted view that cannot be selected from. A generator that emits this has produced invalid DDL.
 *  Formula: ={{ColumnCount}} = 0 */
export function calcRoleSchemaViewsIsDegenerateView(tc: RoleSchemaViewsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.column_count), vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoleSchemaViews(tc: RoleSchemaViewsRow): RoleSchemaViewsRow {
  // Level 1
  calcGuard(tc, roleSchemaViewsFieldTypes, "name", () => { tc.name = calcRoleSchemaViewsName(tc); });
  calcGuard(tc, roleSchemaViewsFieldTypes, "grant_key", () => { tc.grant_key = calcRoleSchemaViewsGrantKey(tc); });
  calcGuard(tc, roleSchemaViewsFieldTypes, "is_full_width", () => { tc.is_full_width = calcRoleSchemaViewsIsFullWidth(tc); });
  calcGuard(tc, roleSchemaViewsFieldTypes, "is_degenerate_view", () => { tc.is_degenerate_view = calcRoleSchemaViewsIsDegenerateView(tc); });
  return tc;
}

/** Reads RoleSchemaViews rows from a JSON array file. */
export function loadRoleSchemaViewsRows(file: string): RoleSchemaViewsRow[] {
  return loadRows(file, { fields: roleSchemaViewsFieldTypes }) as unknown as RoleSchemaViewsRow[];
}

// =============================================================================
// JWTCLAIMMAPPINGS TABLE
// Maps verified JWT claims onto the SQL accessors row predicates call. Magic-links is the notary: it asserts only that the bearer controls an email address. This table records how that verified email, and any additional claims, become values a policy can test.
// =============================================================================

/** A row in the JwtClaimMappings table. */
export interface JwtClaimMappingsRow {
  /** Stored logical identifier, e.g. 'claim-email'. */
  jwt_claim_mapping_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Claim key as it appears in the verified JWT payload, e.g. 'email'. */
  claim_name: string | null;
  /** SQL function a policy calls to read the claim, e.g. 'app.jwt_email()'. */
  sql_accessor: string | null;
  /** True for claims magic-links controls and an app cannot override: email, iss, iat, nbf, exp, sub, tenant_id. */
  is_reserved_claim: boolean | null;
  /** True when this claim is what resolves the caller to an AccessPrincipals row. */
  maps_to_principal: boolean | null;
  /** What the claim asserts and who vouches for it. */
  description2: string | null;
  /** Number of policies whose predicate calls this accessor. */
  usage_count: number | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const jwtClaimMappingsFieldTypes: Record<string, FieldType> = {
  jwt_claim_mapping_id: "string",
  name: "*string",
  claim_name: "*string",
  sql_accessor: "*string",
  is_reserved_claim: "*bool",
  maps_to_principal: "*bool",
  description2: "*string",
  usage_count: "*float64",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{ClaimName}} & " -> " & {{SqlAccessor}} */
export function calcJwtClaimMappingsName(tc: JwtClaimMappingsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.claim_name)), vS(" -> "), erbTextOr(vStr(tc.sql_accessor))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeJwtClaimMappings(tc: JwtClaimMappingsRow): JwtClaimMappingsRow {
  // Level 1
  calcGuard(tc, jwtClaimMappingsFieldTypes, "name", () => { tc.name = calcJwtClaimMappingsName(tc); });
  return tc;
}

/** Reads JwtClaimMappings rows from a JSON array file. */
export function loadJwtClaimMappingsRows(file: string): JwtClaimMappingsRow[] {
  return loadRows(file, { fields: jwtClaimMappingsFieldTypes }) as unknown as JwtClaimMappingsRow[];
}

// =============================================================================
// ACCESSDENIALTESTS TABLE
// Denial witnesses. A policy with no failing case seeded against it is an assertion, not evidence -- the same acceptance bar the rest of this rulebook holds. Each row names a principal, a query, and the row that MUST NOT come back, so a policy that silently stops enforcing is caught by a red test rather than by an incident.
// =============================================================================

/** A row in the AccessDenialTests table. */
export interface AccessDenialTestsRow {
  /** Stored logical identifier, e.g. 'deny-analyst-other-org-procedures'. */
  access_denial_test_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Policy this test exercises. */
  target_policy: string | null;
  /** Principal the query runs as. */
  principal: string | null;
  /** Table queried. */
  target_table: string | null;
  /** Primary key of the row that must be invisible to this principal. */
  forbidden_row_id: string | null;
  /** False for a denial test: the row must not appear. True asserts a row the principal is entitled to does appear. */
  expected_visible: boolean | null;
  /** What the substrate actually returned on the last run. Written back by the verifier, never hand-set. */
  observed_visible: boolean | null;
  /** When this test was last executed against Postgres. */
  last_run_at: string | null;
  /** True once the test has been executed at least once. */
  has_run: boolean | null;
  /** True when observed visibility matches expectation. */
  is_passing: boolean | null;
  /** True when a row that must be invisible was returned. A confirmed access-control breach. */
  is_leak: boolean | null;
  /** True when the test has never run, so it proves nothing regardless of how it is written. */
  is_unproven: boolean | null;
  /** Why this row must (or must not) be visible, and which predicate it exercises. */
  rationale: string | null;
  /** True when this test asserts a row the principal IS entitled to. A denial suite with no positive controls cannot distinguish a working policy from one that denies everything. */
  is_positive_control: boolean | null;
  /** For a table-absence witness: the table that must NOT exist in this principal's schema. Selecting it must raise 'relation does not exist', not return zero rows. */
  forbidden_table: string | null;
  /** For a column-absence witness: the column that must NOT exist in this principal's view. Selecting it must raise 'column does not exist', not return null. */
  forbidden_column: string | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const accessDenialTestsFieldTypes: Record<string, FieldType> = {
  access_denial_test_id: "string",
  name: "*string",
  target_policy: "*string",
  principal: "*string",
  target_table: "*string",
  forbidden_row_id: "*string",
  expected_visible: "*bool",
  observed_visible: "*bool",
  last_run_at: "*string",
  has_run: "*bool",
  is_passing: "*bool",
  is_leak: "*bool",
  is_unproven: "*bool",
  rationale: "*string",
  is_positive_control: "*bool",
  forbidden_table: "*string",
  forbidden_column: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{Principal}} & " must not see " & {{ForbiddenRowId}} */
export function calcAccessDenialTestsName(tc: AccessDenialTestsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.principal)), vS(" must not see "), erbTextOr(vStr(tc.forbidden_row_id))));
}

/** Computes the HasRun calculated field.
 *  True once the test has been executed at least once.
 *  Formula: ={{LastRunAt}} <> "" */
export function calcAccessDenialTestsHasRun(tc: AccessDenialTestsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.last_run_at)));
}

/** Computes the IsPassing calculated field.
 *  True when observed visibility matches expectation.
 *  Formula: ={{ObservedVisible}} = {{ExpectedVisible}} */
export function calcAccessDenialTestsIsPassing(tc: AccessDenialTestsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vBool(tc.observed_visible)), erbNullif(vBool(tc.expected_visible))));
}

/** Computes the IsLeak calculated field.
 *  True when a row that must be invisible was returned. A confirmed access-control breach.
 *  Formula: =AND(NOT({{ExpectedVisible}}), {{ObservedVisible}}) */
export function calcAccessDenialTestsIsLeak(tc: AccessDenialTestsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbIsTrue(vBool(tc.expected_visible)))), erbIsTrue(vBool(tc.observed_visible))));
}

/** Computes the IsUnproven calculated field.
 *  True when the test has never run, so it proves nothing regardless of how it is written.
 *  Formula: =NOT({{HasRun}}) */
export function calcAccessDenialTestsIsUnproven(tc: AccessDenialTestsRow): boolean | null {
  return toBoolPtr(erbNot(erbBool3(vBool(tc.has_run))));
}

/** Computes the IsPositiveControl calculated field.
 *  True when this test asserts a row the principal IS entitled to. A denial suite with no positive controls cannot distinguish a working policy from one that denies everything.
 *  Formula: ={{ExpectedVisible}} */
export function calcAccessDenialTestsIsPositiveControl(tc: AccessDenialTestsRow): boolean | null {
  return toBoolPtr(vBool(tc.expected_visible));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAccessDenialTests(tc: AccessDenialTestsRow): AccessDenialTestsRow {
  // Level 1
  calcGuard(tc, accessDenialTestsFieldTypes, "name", () => { tc.name = calcAccessDenialTestsName(tc); });
  calcGuard(tc, accessDenialTestsFieldTypes, "has_run", () => { tc.has_run = calcAccessDenialTestsHasRun(tc); });
  calcGuard(tc, accessDenialTestsFieldTypes, "is_passing", () => { tc.is_passing = calcAccessDenialTestsIsPassing(tc); });
  calcGuard(tc, accessDenialTestsFieldTypes, "is_leak", () => { tc.is_leak = calcAccessDenialTestsIsLeak(tc); });
  calcGuard(tc, accessDenialTestsFieldTypes, "is_positive_control", () => { tc.is_positive_control = calcAccessDenialTestsIsPositiveControl(tc); });
  // Level 2
  calcGuard(tc, accessDenialTestsFieldTypes, "is_unproven", () => { tc.is_unproven = calcAccessDenialTestsIsUnproven(tc); });
  return tc;
}

/** Reads AccessDenialTests rows from a JSON array file. */
export function loadAccessDenialTestsRows(file: string): AccessDenialTestsRow[] {
  return loadRows(file, { fields: accessDenialTestsFieldTypes }) as unknown as AccessDenialTestsRow[];
}

// =============================================================================
// APPUSERS TABLE
// Sign-in identities. One row per person or automation that can authenticate. EmailAddress is what a verified token asserts; everything else about the caller is resolved from here inside the database, never trusted from the token.
// =============================================================================

/** A row in the AppUsers table. */
export interface AppUsersRow {
  /** Stored logical identifier, e.g. 'user-maria-chen'. */
  app_user_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Verified email. The one claim magic-links vouches for, and the join key from a token back to this row. */
  email_address: string | null;
  /** Name shown in the console. */
  display_name: string | null;
  /** Domain agent this sign-in identity corresponds to. */
  linked_agent: string | null;
  /** False disables sign-in without deleting the identity or its history. */
  is_enabled: boolean | null;
  /** Whether the linked agent is Human, AIAgent or AutomatedPipeline. */
  agent_kind: string | null;
  /** Organization inherited from the linked agent; the tenancy claim baked into issued tokens. */
  organization: string | null;
  /** Number of principals this user may act as. */
  assignment_count: number | null;
  /** True when the user may act as no principal at all, so a successfully verified token still grants nothing. Authentication without authorization. */
  has_no_principal: boolean | null;
  /** True when the user may act as more than one principal, so the principal cannot be inferred from the email alone and must be chosen explicitly at sign-in. */
  holds_multiple_principals: boolean | null;
  /** True when a non-human agent has a sign-in identity. Pipelines and AI agents authenticate too, and their tokens are scoped exactly like a person's. */
  is_non_human_sign_in: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const appUsersFieldTypes: Record<string, FieldType> = {
  app_user_id: "string",
  name: "*string",
  email_address: "*string",
  display_name: "*string",
  linked_agent: "*string",
  is_enabled: "*bool",
  agent_kind: "*string",
  organization: "*string",
  assignment_count: "*float64",
  has_no_principal: "*bool",
  holds_multiple_principals: "*bool",
  is_non_human_sign_in: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{DisplayName}} */
export function calcAppUsersName(tc: AppUsersRow): string | null {
  return toStringPtr(vStr(tc.display_name));
}

/** Computes the HasNoPrincipal calculated field.
 *  True when the user may act as no principal at all, so a successfully verified token still grants nothing. Authentication without authorization.
 *  Formula: ={{AssignmentCount}} = 0 */
export function calcAppUsersHasNoPrincipal(tc: AppUsersRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.assignment_count), vI(0)));
}

/** Computes the HoldsMultiplePrincipals calculated field.
 *  True when the user may act as more than one principal, so the principal cannot be inferred from the email alone and must be chosen explicitly at sign-in.
 *  Formula: ={{AssignmentCount}} > 1 */
export function calcAppUsersHoldsMultiplePrincipals(tc: AppUsersRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.assignment_count), ">", vI(1)));
}

/** Computes the IsNonHumanSignIn calculated field.
 *  True when a non-human agent has a sign-in identity. Pipelines and AI agents authenticate too, and their tokens are scoped exactly like a person's.
 *  Formula: =OR({{AgentKind}} = "AIAgent", {{AgentKind}} = "AutomatedPipeline") */
export function calcAppUsersIsNonHumanSignIn(tc: AppUsersRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbEq(vStr(tc.agent_kind), vS("AIAgent"))), erbBool3(erbEq(vStr(tc.agent_kind), vS("AutomatedPipeline")))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeAppUsers(tc: AppUsersRow): AppUsersRow {
  // Level 1
  calcGuard(tc, appUsersFieldTypes, "name", () => { tc.name = calcAppUsersName(tc); });
  calcGuard(tc, appUsersFieldTypes, "has_no_principal", () => { tc.has_no_principal = calcAppUsersHasNoPrincipal(tc); });
  calcGuard(tc, appUsersFieldTypes, "holds_multiple_principals", () => { tc.holds_multiple_principals = calcAppUsersHoldsMultiplePrincipals(tc); });
  calcGuard(tc, appUsersFieldTypes, "is_non_human_sign_in", () => { tc.is_non_human_sign_in = calcAppUsersIsNonHumanSignIn(tc); });
  return tc;
}

/** Reads AppUsers rows from a JSON array file. */
export function loadAppUsersRows(file: string): AppUsersRow[] {
  return loadRows(file, { fields: appUsersFieldTypes }) as unknown as AppUsersRow[];
}

// =============================================================================
// PRINCIPALASSIGNMENTS TABLE
// Which principals a user may act as. The authorization half of sign-in: a verified email proves who you are, this table decides what you may become. A user with two assignments picks one at sign-in, and the choice is verified here rather than accepted from the client.
// =============================================================================

/** A row in the PrincipalAssignments table. */
export interface PrincipalAssignmentsRow {
  /** Stored logical identifier, e.g. 'pa-maria-finance-analyst'. */
  principal_assignment_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Sign-in identity being granted. */
  app_user: string | null;
  /** Principal the user may act as. */
  principal: string | null;
  /** True for the principal selected when the user does not name one. */
  is_default: boolean | null;
  /** Why this user may act as this principal. */
  granted_rationale: string | null;
  /** Whether the assigned principal is an administrator. */
  principal_is_admin: boolean | null;
  /** Organization of the signing-in user. */
  user_organization: string | null;
  /** Organization of the principal being assumed. */
  principal_organization: string | null;
  /** True when a user is allowed to act as a principal in a different organization. Legitimate for shared-service roles, but it crosses the tenancy boundary and should be deliberate rather than accidental. */
  is_cross_organization_grant: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const principalAssignmentsFieldTypes: Record<string, FieldType> = {
  principal_assignment_id: "string",
  name: "*string",
  app_user: "*string",
  principal: "*string",
  is_default: "*bool",
  granted_rationale: "*string",
  principal_is_admin: "*bool",
  user_organization: "*string",
  principal_organization: "*string",
  is_cross_organization_grant: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{AppUser}} & " as " & {{Principal}} */
export function calcPrincipalAssignmentsName(tc: PrincipalAssignmentsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.app_user)), vS(" as "), erbTextOr(vStr(tc.principal))));
}

/** Computes the IsCrossOrganizationGrant calculated field.
 *  True when a user is allowed to act as a principal in a different organization. Legitimate for shared-service roles, but it crosses the tenancy boundary and should be deliberate rather than accidental.
 *  Formula: =AND({{UserOrganization}} <> "", {{PrincipalOrganization}} <> "", {{UserOrganization}} <> {{PrincipalOrganization}}) */
export function calcPrincipalAssignmentsIsCrossOrganizationGrant(tc: PrincipalAssignmentsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbIsNotBlank(vStr(tc.user_organization))), erbBool3(erbIsNotBlank(vStr(tc.principal_organization))), erbBool3(erbNe(vStr(tc.user_organization), vStr(tc.principal_organization)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computePrincipalAssignments(tc: PrincipalAssignmentsRow): PrincipalAssignmentsRow {
  // Level 1
  calcGuard(tc, principalAssignmentsFieldTypes, "name", () => { tc.name = calcPrincipalAssignmentsName(tc); });
  calcGuard(tc, principalAssignmentsFieldTypes, "is_cross_organization_grant", () => { tc.is_cross_organization_grant = calcPrincipalAssignmentsIsCrossOrganizationGrant(tc); });
  return tc;
}

/** Reads PrincipalAssignments rows from a JSON array file. */
export function loadPrincipalAssignmentsRows(file: string): PrincipalAssignmentsRow[] {
  return loadRows(file, { fields: principalAssignmentsFieldTypes }) as unknown as PrincipalAssignmentsRow[];
}

// =============================================================================
// ISSUEDTOKENS TABLE
// Audit trail of every token minted. A token records which user signed in, which principal they chose, and the claims that were joined from the database at mint time -- so a later question of 'what could this session see' is answerable from data rather than reconstruction.
// =============================================================================

/** A row in the IssuedTokens table. */
export interface IssuedTokensRow {
  /** Stored logical identifier for one mint event. */
  issued_token_id: string;
  /** Human-readable calculated display alias. */
  name: string | null;
  /** Identity that signed in. */
  app_user: string | null;
  /** Principal the token authorises. */
  principal: string | null;
  /** When the token was minted. */
  issued_at: string | null;
  /** When the token stops being accepted. */
  expires_at: string | null;
  /** Who minted it: 'dev-mint' locally, or the magic-links tenant URL in production. */
  issuer: string | null;
  /** The 'sub' claim: the AppUserId the bearer is asserted to be. */
  subject_claim: string | null;
  /** JSON of the additional claims joined from the database at mint time. */
  claims_snapshot: string | null;
  /** True when issued by the local dev minter rather than a real magic-links tenant. Dev tokens are genuine RS256 tokens with a genuine keypair; they simply skip the email round-trip. */
  is_dev_minted: boolean | null;
  /** Semantic type IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const issuedTokensFieldTypes: Record<string, FieldType> = {
  issued_token_id: "string",
  name: "*string",
  app_user: "*string",
  principal: "*string",
  issued_at: "*string",
  expires_at: "*string",
  issuer: "*string",
  subject_claim: "*string",
  claims_snapshot: "*string",
  is_dev_minted: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias.
 *  Formula: ={{AppUser}} & " as " & {{Principal}} & " @ " & {{IssuedAt}} */
export function calcIssuedTokensName(tc: IssuedTokensRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.app_user)), vS(" as "), erbTextOr(vStr(tc.principal)), vS(" @ "), erbTimestamptzText(vStr(tc.issued_at))));
}

/** Computes the IsDevMinted calculated field.
 *  True when issued by the local dev minter rather than a real magic-links tenant. Dev tokens are genuine RS256 tokens with a genuine keypair; they simply skip the email round-trip.
 *  Formula: ={{Issuer}} = "dev-mint" */
export function calcIssuedTokensIsDevMinted(tc: IssuedTokensRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStr(tc.issuer)), vS("dev-mint")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeIssuedTokens(tc: IssuedTokensRow): IssuedTokensRow {
  // Level 1
  calcGuard(tc, issuedTokensFieldTypes, "name", () => { tc.name = calcIssuedTokensName(tc); });
  calcGuard(tc, issuedTokensFieldTypes, "is_dev_minted", () => { tc.is_dev_minted = calcIssuedTokensIsDevMinted(tc); });
  return tc;
}

/** Reads IssuedTokens rows from a JSON array file. */
export function loadIssuedTokensRows(file: string): IssuedTokensRow[] {
  return loadRows(file, { fields: issuedTokensFieldTypes }) as unknown as IssuedTokensRow[];
}

// =============================================================================
// PROCESSMININGRUNS TABLE
// One conformance-checking run of a mined event log against a documented procedure version — a third kind of evidence, distinct from an elicitation session (someone's account) or a knowledge fragment (a claim): what the system of record actually did, discovered by process mining rather than told to us.
// =============================================================================

/** A row in the ProcessMiningRuns table. */
export interface ProcessMiningRunsRow {
  /** Stored logical identifier for one ProcessMiningRuns row. */
  process_mining_run_id: string;
  /** Human-readable calculated display alias for the ProcessMiningRuns row. */
  name: string | null;
  /** Procedure version this mined event log is checked against. */
  procedure_version: string | null;
  /** System or log the event log was extracted from (e.g. an ERP audit log). */
  event_log_source: string | null;
  /** When this mining/conformance-checking run was performed. */
  mined_at: string | null;
  /** Distinct process variants discovered in the mined event log. */
  discovered_variant_count: number | null;
  /** How many of the discovered variants exactly match a documented path through StepTransitions. */
  conforming_variant_count: number | null;
  /** Narrative of the most significant discovered deviation, if any. */
  deviation_description: string | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged under. */
  as_of_instant: string | null;
  /** Share of discovered variants that conform to the documented procedure. */
  conformance_rate: number | null;
  /** TRUE when at least 80% of what actually happened matches what was documented. */
  is_conformant: boolean | null;
  /** TRUE when less than half of what actually happened matches what was documented. */
  has_major_drift_from_documentation: boolean | null;
  /** Days elapsed since this event log was mined. */
  days_since_mined: number | null;
  /** TRUE when this mining evidence is more than 180 days old. */
  is_stale_mining_evidence: boolean | null;
  /** Whether the procedure version this run was checked against is currently live. */
  procedure_version_is_live: boolean | null;
  /** TRUE when a major, real, mined deviation exists against a procedure version people are actually executing right now. */
  is_drift_on_live_version: boolean | null;
  /** Composite-key echo: this run's procedure version when the run drifted on a live version, else blank. */
  drifted_mining_run_key: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const processMiningRunsFieldTypes: Record<string, FieldType> = {
  process_mining_run_id: "string",
  name: "*string",
  procedure_version: "*string",
  event_log_source: "*string",
  mined_at: "*string",
  discovered_variant_count: "*int",
  conforming_variant_count: "*int",
  deviation_description: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  conformance_rate: "*float64",
  is_conformant: "*bool",
  has_major_drift_from_documentation: "*bool",
  days_since_mined: "*int",
  is_stale_mining_evidence: "*bool",
  procedure_version_is_live: "*bool",
  is_drift_on_live_version: "*bool",
  drifted_mining_run_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the ProcessMiningRuns row.
 *  Formula: ={{EventLogSource}} & " / " & {{MinedAt}} */
export function calcProcessMiningRunsName(tc: ProcessMiningRunsRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.event_log_source)), vS(" / "), erbTimestamptzText(vStr(tc.mined_at))));
}

/** Computes the ConformanceRate calculated field.
 *  Share of discovered variants that conform to the documented procedure.
 *  Formula: =IF({{DiscoveredVariantCount}} = 0, 0, {{ConformingVariantCount}} / {{DiscoveredVariantCount}}) */
export function calcProcessMiningRunsConformanceRate(tc: ProcessMiningRunsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(erbEq(erbNullif(vInt(tc.discovered_variant_count)), vI(0))), () => vI(0), () => erbDiv(vInt(tc.conforming_variant_count), vInt(tc.discovered_variant_count))));
}

/** Computes the IsConformant calculated field.
 *  TRUE when at least 80% of what actually happened matches what was documented.
 *  Formula: ={{ConformanceRate}} >= 0.8 */
export function calcProcessMiningRunsIsConformant(tc: ProcessMiningRunsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.conformance_rate), ">=", vF(0.8)));
}

/** Computes the HasMajorDriftFromDocumentation calculated field.
 *  TRUE when less than half of what actually happened matches what was documented.
 *  Formula: ={{ConformanceRate}} < 0.5 */
export function calcProcessMiningRunsHasMajorDriftFromDocumentation(tc: ProcessMiningRunsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.conformance_rate), "<", vF(0.5)));
}

/** Computes the DaysSinceMined calculated field.
 *  Days elapsed since this event log was mined.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{MinedAt}}, "days") */
export function calcProcessMiningRunsDaysSinceMined(tc: ProcessMiningRunsRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.mined_at), vS("days"))));
}

/** Computes the IsStaleMiningEvidence calculated field.
 *  TRUE when this mining evidence is more than 180 days old.
 *  Formula: ={{DaysSinceMined}} > 180 */
export function calcProcessMiningRunsIsStaleMiningEvidence(tc: ProcessMiningRunsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.days_since_mined), ">", vI(180)));
}

/** Computes the IsDriftOnLiveVersion calculated field.
 *  TRUE when a major, real, mined deviation exists against a procedure version people are actually executing right now.
 *  Formula: =AND({{HasMajorDriftFromDocumentation}}, {{ProcedureVersionIsLive}}) */
export function calcProcessMiningRunsIsDriftOnLiveVersion(tc: ProcessMiningRunsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.has_major_drift_from_documentation)), erbBool3(vBool(tc.procedure_version_is_live))));
}

/** Computes the DriftedMiningRunKey calculated field.
 *  Composite-key echo: this run's procedure version when the run drifted on a live version, else blank.
 *  Formula: =IF({{IsDriftOnLiveVersion}}, {{ProcedureVersion}}, "") */
export function calcProcessMiningRunsDriftedMiningRunKey(tc: ProcessMiningRunsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_drift_on_live_version)), () => vStr(tc.procedure_version), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProcessMiningRuns(tc: ProcessMiningRunsRow): ProcessMiningRunsRow {
  // Level 1
  calcGuard(tc, processMiningRunsFieldTypes, "name", () => { tc.name = calcProcessMiningRunsName(tc); });
  calcGuard(tc, processMiningRunsFieldTypes, "conformance_rate", () => { tc.conformance_rate = calcProcessMiningRunsConformanceRate(tc); });
  calcGuard(tc, processMiningRunsFieldTypes, "days_since_mined", () => { tc.days_since_mined = calcProcessMiningRunsDaysSinceMined(tc); });
  // Level 2
  calcGuard(tc, processMiningRunsFieldTypes, "is_conformant", () => { tc.is_conformant = calcProcessMiningRunsIsConformant(tc); });
  calcGuard(tc, processMiningRunsFieldTypes, "has_major_drift_from_documentation", () => { tc.has_major_drift_from_documentation = calcProcessMiningRunsHasMajorDriftFromDocumentation(tc); });
  calcGuard(tc, processMiningRunsFieldTypes, "is_stale_mining_evidence", () => { tc.is_stale_mining_evidence = calcProcessMiningRunsIsStaleMiningEvidence(tc); });
  // Level 3
  calcGuard(tc, processMiningRunsFieldTypes, "is_drift_on_live_version", () => { tc.is_drift_on_live_version = calcProcessMiningRunsIsDriftOnLiveVersion(tc); });
  // Level 4
  calcGuard(tc, processMiningRunsFieldTypes, "drifted_mining_run_key", () => { tc.drifted_mining_run_key = calcProcessMiningRunsDriftedMiningRunKey(tc); });
  return tc;
}

/** Reads ProcessMiningRuns rows from a JSON array file. */
export function loadProcessMiningRunsRows(file: string): ProcessMiningRunsRow[] {
  return loadRows(file, { fields: processMiningRunsFieldTypes }) as unknown as ProcessMiningRunsRow[];
}

// =============================================================================
// VOCABULARIES TABLE
// A controlled vocabulary / SKOS-style concept scheme — one facet of standardized terminology (e.g. a family of control categories) that Requirements and other rows can point at instead of restating the concept in free text each time.
// =============================================================================

/** A row in the Vocabularies table. */
export interface VocabulariesRow {
  /** Stored logical identifier for one Vocabularies row. */
  vocabulary_id: string;
  /** Human-readable calculated display alias for the Vocabularies row. */
  name: string | null;
  /** Human-facing name of this controlled vocabulary. */
  title: string | null;
  /** SKOS ConceptScheme IRI for this vocabulary. */
  scheme_uri: string | null;
  /** Role accountable for defining and maintaining terms in this vocabulary. */
  governing_role: string | null;
  /** How many terms belong to this vocabulary. */
  term_count: number | null;
  /** How many terms in this vocabulary have never actually been used. */
  orphan_term_count: number | null;
  /** TRUE when this vocabulary has at least one defined-but-unused term. */
  has_orphan_terms: boolean | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const vocabulariesFieldTypes: Record<string, FieldType> = {
  vocabulary_id: "string",
  name: "*string",
  title: "*string",
  scheme_uri: "*string",
  governing_role: "*string",
  term_count: "*float64",
  orphan_term_count: "*float64",
  has_orphan_terms: "*bool",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the Vocabularies row.
 *  Formula: ={{Title}} */
export function calcVocabulariesName(tc: VocabulariesRow): string | null {
  return toStringPtr(vStr(tc.title));
}

/** Computes the HasOrphanTerms calculated field.
 *  TRUE when this vocabulary has at least one defined-but-unused term.
 *  Formula: ={{OrphanTermCount}} > 0 */
export function calcVocabulariesHasOrphanTerms(tc: VocabulariesRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.orphan_term_count), ">", vI(0)));
}

/** Computes every calculated field of the row in dependency order. */
export function computeVocabularies(tc: VocabulariesRow): VocabulariesRow {
  // Level 1
  calcGuard(tc, vocabulariesFieldTypes, "name", () => { tc.name = calcVocabulariesName(tc); });
  calcGuard(tc, vocabulariesFieldTypes, "has_orphan_terms", () => { tc.has_orphan_terms = calcVocabulariesHasOrphanTerms(tc); });
  return tc;
}

/** Reads Vocabularies rows from a JSON array file. */
export function loadVocabulariesRows(file: string): VocabulariesRow[] {
  return loadRows(file, { fields: vocabulariesFieldTypes }) as unknown as VocabulariesRow[];
}

// =============================================================================
// VOCABULARYTERMS TABLE
// One controlled, defined term (a SKOS Concept) within a Vocabulary. Requirements point at a VocabularyTerm instead of free-texting the same concept in a new Statement every time.
// =============================================================================

/** A row in the VocabularyTerms table. */
export interface VocabularyTermsRow {
  /** Stored logical identifier for one VocabularyTerms row. */
  vocabulary_term_id: string;
  /** Human-readable calculated display alias for the VocabularyTerms row. */
  name: string | null;
  /** Vocabulary this term belongs to. */
  vocabulary: string | null;
  /** SKOS preferred label — the controlled term itself. */
  pref_label: string | null;
  /** Comma-separated synonyms this term should also match. */
  alt_labels: string | null;
  /** SKOS definition of what this term means. */
  definition: string | null;
  /** How many Requirements point at this exact term instead of free-texting the concept. */
  usage_count: number | null;
  /** TRUE when this term has been defined but nothing in the model actually uses it yet. */
  is_orphan_term: boolean | null;
  /** TRUE when more than one Requirement shares this exact controlled term. */
  is_widely_adopted_term: boolean | null;
  /** Composite-key echo: this term's vocabulary when the term is an orphan, else blank. */
  orphan_term_vocabulary_key: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const vocabularyTermsFieldTypes: Record<string, FieldType> = {
  vocabulary_term_id: "string",
  name: "*string",
  vocabulary: "*string",
  pref_label: "*string",
  alt_labels: "*string",
  definition: "*string",
  usage_count: "*float64",
  is_orphan_term: "*bool",
  is_widely_adopted_term: "*bool",
  orphan_term_vocabulary_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the VocabularyTerms row.
 *  Formula: ={{PrefLabel}} */
export function calcVocabularyTermsName(tc: VocabularyTermsRow): string | null {
  return toStringPtr(vStr(tc.pref_label));
}

/** Computes the IsOrphanTerm calculated field.
 *  TRUE when this term has been defined but nothing in the model actually uses it yet.
 *  Formula: ={{UsageCount}} = 0 */
export function calcVocabularyTermsIsOrphanTerm(tc: VocabularyTermsRow): boolean | null {
  return toBoolPtr(erbEq(vNum(tc.usage_count), vI(0)));
}

/** Computes the IsWidelyAdoptedTerm calculated field.
 *  TRUE when more than one Requirement shares this exact controlled term.
 *  Formula: ={{UsageCount}} >= 2 */
export function calcVocabularyTermsIsWidelyAdoptedTerm(tc: VocabularyTermsRow): boolean | null {
  return toBoolPtr(erbCmp(vNum(tc.usage_count), ">=", vI(2)));
}

/** Computes the OrphanTermVocabularyKey calculated field.
 *  Composite-key echo: this term's vocabulary when the term is an orphan, else blank.
 *  Formula: =IF({{IsOrphanTerm}}, {{Vocabulary}}, "") */
export function calcVocabularyTermsOrphanTermVocabularyKey(tc: VocabularyTermsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_orphan_term)), () => vStr(tc.vocabulary), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeVocabularyTerms(tc: VocabularyTermsRow): VocabularyTermsRow {
  // Level 1
  calcGuard(tc, vocabularyTermsFieldTypes, "name", () => { tc.name = calcVocabularyTermsName(tc); });
  calcGuard(tc, vocabularyTermsFieldTypes, "is_orphan_term", () => { tc.is_orphan_term = calcVocabularyTermsIsOrphanTerm(tc); });
  calcGuard(tc, vocabularyTermsFieldTypes, "is_widely_adopted_term", () => { tc.is_widely_adopted_term = calcVocabularyTermsIsWidelyAdoptedTerm(tc); });
  // Level 2
  calcGuard(tc, vocabularyTermsFieldTypes, "orphan_term_vocabulary_key", () => { tc.orphan_term_vocabulary_key = calcVocabularyTermsOrphanTermVocabularyKey(tc); });
  return tc;
}

/** Reads VocabularyTerms rows from a JSON array file. */
export function loadVocabularyTermsRows(file: string): VocabularyTermsRow[] {
  return loadRows(file, { fields: vocabularyTermsFieldTypes }) as unknown as VocabularyTermsRow[];
}

// =============================================================================
// KNOWLEDGEBROKERLINKS TABLE
// An informal expertise-network edge: one agent naming another as who they actually go to for a topic, independent of any formal Role or RoleAssignment. RoleAssignments and CommunitiesOfPractice record who is SUPPOSED to know something; this records who people ACTUALLY rely on.
// =============================================================================

/** A row in the KnowledgeBrokerLinks table. */
export interface KnowledgeBrokerLinksRow {
  /** Stored logical identifier for one KnowledgeBrokerLinks row. */
  knowledge_broker_link_id: string;
  /** Human-readable calculated display alias for the KnowledgeBrokerLinks row. */
  name: string | null;
  /** The agent who goes to someone else for this topic. */
  seeker: string | null;
  /** The agent who is actually consulted, whether or not they hold a formal role for it. */
  broker: string | null;
  /** The controlled topic this reliance is about. */
  topic: string | null;
  /** Rarely, Sometimes, or Often. */
  frequency: string | null;
  /** When the seeker last actually consulted the broker on this topic. */
  last_consulted_at: string | null;
  /** The evaluation context this row's time-dependent witnesses are judged under. */
  evaluation_context: string | null;
  /** The evaluation instant this row's time-dependent witnesses are judged under. */
  as_of_instant: string | null;
  /** Days elapsed since the seeker last consulted the broker. */
  days_since_consulted: number | null;
  /** TRUE when this is a live, ongoing informal dependency rather than a one-off or stale contact. */
  is_active_reliance: boolean | null;
  /** Whether the broker being relied on still holds any current role at all. */
  broker_is_still_engaged: boolean | null;
  /** TRUE when someone is actively relying on a broker who has already left every role they held. */
  is_at_risk_reliance: boolean | null;
  /** Composite-key echo: the broker this link names when the reliance is active, else blank. */
  active_reliance_broker_key: string | null;
  /** Composite-key echo: the broker this link names when the reliance is at risk, else blank. */
  at_risk_broker_key: string | null;
  /** Extension class IRI. */
  semantic_type_iri: string | null;
  _erb_errors?: Record<string, string>;
}

const knowledgeBrokerLinksFieldTypes: Record<string, FieldType> = {
  knowledge_broker_link_id: "string",
  name: "*string",
  seeker: "*string",
  broker: "*string",
  topic: "*string",
  frequency: "*string",
  last_consulted_at: "*string",
  evaluation_context: "*string",
  as_of_instant: "*string",
  days_since_consulted: "*int",
  is_active_reliance: "*bool",
  broker_is_still_engaged: "*bool",
  is_at_risk_reliance: "*bool",
  active_reliance_broker_key: "*string",
  at_risk_broker_key: "*string",
  semantic_type_iri: "*string",
};

/** Computes the Name calculated field.
 *  Human-readable calculated display alias for the KnowledgeBrokerLinks row.
 *  Formula: ={{Seeker}} & " -> " & {{Broker}} */
export function calcKnowledgeBrokerLinksName(tc: KnowledgeBrokerLinksRow): string | null {
  return toStringPtr(erbConcat(erbTextOr(vStr(tc.seeker)), vS(" -> "), erbTextOr(vStr(tc.broker))));
}

/** Computes the DaysSinceConsulted calculated field.
 *  Days elapsed since the seeker last consulted the broker.
 *  Formula: =DATETIME_DIFF({{AsOfInstant}}, {{LastConsultedAt}}, "days") */
export function calcKnowledgeBrokerLinksDaysSinceConsulted(tc: KnowledgeBrokerLinksRow): number | null {
  return toIntPtr(erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), vStr(tc.last_consulted_at), vS("days"))));
}

/** Computes the IsActiveReliance calculated field.
 *  TRUE when this is a live, ongoing informal dependency rather than a one-off or stale contact.
 *  Formula: =AND(NOT({{Frequency}} = "Rarely"), {{DaysSinceConsulted}} <= 180) */
export function calcKnowledgeBrokerLinksIsActiveReliance(tc: KnowledgeBrokerLinksRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbNot(erbBool3(erbEq(erbNullif(vStr(tc.frequency)), vS("Rarely"))))), erbBool3(erbCmp(vInt(tc.days_since_consulted), "<=", vI(180)))));
}

/** Computes the IsAtRiskReliance calculated field.
 *  TRUE when someone is actively relying on a broker who has already left every role they held.
 *  Formula: =AND({{IsActiveReliance}}, NOT({{BrokerIsStillEngaged}})) */
export function calcKnowledgeBrokerLinksIsAtRiskReliance(tc: KnowledgeBrokerLinksRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.is_active_reliance)), erbBool3(erbNot(erbBool3(vBool(tc.broker_is_still_engaged))))));
}

/** Computes the ActiveRelianceBrokerKey calculated field.
 *  Composite-key echo: the broker this link names when the reliance is active, else blank.
 *  Formula: =IF({{IsActiveReliance}}, {{Broker}}, "") */
export function calcKnowledgeBrokerLinksActiveRelianceBrokerKey(tc: KnowledgeBrokerLinksRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_active_reliance)), () => vStr(tc.broker), () => vS("")));
}

/** Computes the AtRiskBrokerKey calculated field.
 *  Composite-key echo: the broker this link names when the reliance is at risk, else blank.
 *  Formula: =IF({{IsAtRiskReliance}}, {{Broker}}, "") */
export function calcKnowledgeBrokerLinksAtRiskBrokerKey(tc: KnowledgeBrokerLinksRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_at_risk_reliance)), () => vStr(tc.broker), () => vS("")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeKnowledgeBrokerLinks(tc: KnowledgeBrokerLinksRow): KnowledgeBrokerLinksRow {
  // Level 1
  calcGuard(tc, knowledgeBrokerLinksFieldTypes, "name", () => { tc.name = calcKnowledgeBrokerLinksName(tc); });
  calcGuard(tc, knowledgeBrokerLinksFieldTypes, "days_since_consulted", () => { tc.days_since_consulted = calcKnowledgeBrokerLinksDaysSinceConsulted(tc); });
  // Level 2
  calcGuard(tc, knowledgeBrokerLinksFieldTypes, "is_active_reliance", () => { tc.is_active_reliance = calcKnowledgeBrokerLinksIsActiveReliance(tc); });
  // Level 3
  calcGuard(tc, knowledgeBrokerLinksFieldTypes, "is_at_risk_reliance", () => { tc.is_at_risk_reliance = calcKnowledgeBrokerLinksIsAtRiskReliance(tc); });
  calcGuard(tc, knowledgeBrokerLinksFieldTypes, "active_reliance_broker_key", () => { tc.active_reliance_broker_key = calcKnowledgeBrokerLinksActiveRelianceBrokerKey(tc); });
  // Level 4
  calcGuard(tc, knowledgeBrokerLinksFieldTypes, "at_risk_broker_key", () => { tc.at_risk_broker_key = calcKnowledgeBrokerLinksAtRiskBrokerKey(tc); });
  return tc;
}

/** Reads KnowledgeBrokerLinks rows from a JSON array file. */
export function loadKnowledgeBrokerLinksRows(file: string): KnowledgeBrokerLinksRow[] {
  return loadRows(file, { fields: knowledgeBrokerLinksFieldTypes }) as unknown as KnowledgeBrokerLinksRow[];
}

/** Bounds the runner's passes over the dataset. */
export const calculatedFieldCount = 705;

/** Every table, in rulebook order. */
export const erbTables: TableSpec[] = [
  { name: "RulebookReleases", file: "rulebook_releases", rulebookRows: 1, fields: rulebookReleasesFieldTypes,
    compute: (row: any) => computeRulebookReleases(row as RulebookReleasesRow),
    lookups: [],
    aggregations: [] },
  { name: "OntologyProfiles", file: "ontology_profiles", rulebookRows: 11, fields: ontologyProfilesFieldTypes,
    compute: (row: any) => computeOntologyProfiles(row as OntologyProfilesRow),
    lookups: [],
    aggregations: [] },
  { name: "EvaluationContexts", file: "evaluation_contexts", rulebookRows: 1, fields: evaluationContextsFieldTypes,
    compute: (row: any) => computeEvaluationContexts(row as EvaluationContextsRow),
    lookups: [],
    aggregations: [] },
  { name: "Organizations", file: "organizations", rulebookRows: 4, fields: organizationsFieldTypes,
    compute: (row: any) => computeOrganizations(row as OrganizationsRow),
    lookups: [],
    aggregations: [] },
  { name: "Agents", file: "agents", rulebookRows: 11, fields: agentsFieldTypes,
    compute: (row: any) => computeAgents(row as AgentsRow),
    lookups: [],
    aggregations: [
      { field: "count_of_current_role_assignments", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "current_agent_key", kind: "field", field: "agent_id" }] },
      { field: "decision_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "deciding_agent", kind: "field", field: "agent_id" }] },
      { field: "overridden_decision_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "deciding_agent_when_overridden", kind: "field", field: "agent_id" }] },
      { field: "boundary_violation_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "agent_when_boundary_violated", kind: "field", field: "agent_id" }] },
      { field: "draft_decision_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "agent_when_draft", kind: "field", field: "agent_id" }] },
      { field: "overridden_draft_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "agent_when_draft_overridden", kind: "field", field: "agent_id" }] },
      { field: "times_named_as_broker", op: "COUNTIFS", table: "knowledge_broker_links", criteria: [{ range: "active_reliance_broker_key", kind: "field", field: "agent_id" }] },
      { field: "at_risk_reliance_count", op: "COUNTIFS", table: "knowledge_broker_links", criteria: [{ range: "at_risk_broker_key", kind: "field", field: "agent_id" }] },] },
  { name: "Roles", file: "roles", rulebookRows: 12, fields: rolesFieldTypes,
    compute: (row: any) => computeRoles(row as RolesRow),
    lookups: [
      { field: "current_agent_kind", target: "agents", ret: "agent_kind", key: "current_agent", match: "agent_id" },
      { field: "current_assignment_valid_from", target: "role_assignments", ret: "valid_from", key: "current_assignment", match: "role_assignment_id" },],
    aggregations: [
      { field: "active_assignment_count", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "role", kind: "field", field: "role_id" }] },
      { field: "currently_covered_assignment_count", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "role_when_covering", kind: "field", field: "role_id" }] },
      { field: "count_of_awaited_decisions", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "authority_role", kind: "field", field: "role_id" }] },
      { field: "departed_assignment_count", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "departed_role_key", kind: "field", field: "role_id" }] },
      { field: "ungrounded_boundary_count", op: "COUNTIFS", table: "authority_boundaries", criteria: [{ range: "constrained_role_assignment_key", kind: "field", field: "role_id" }] },
      { field: "unescalated_refusal_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "unescalated_refusal_role_key", kind: "field", field: "role_id" }] },
      { field: "unauthorized_enforcement_assignment_count", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "unauthorized_enforcement_role_key", kind: "field", field: "role_id" }] },] },
  { name: "RoleAssignments", file: "role_assignments", rulebookRows: 13, fields: roleAssignmentsFieldTypes,
    compute: (row: any) => computeRoleAssignments(row as RoleAssignmentsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "agent_kind", target: "agents", ret: "agent_kind", key: "agent", match: "agent_id" },
      { field: "predecessor_agent_kind", target: "role_assignments", ret: "agent_kind", key: "supersedes_assignment", match: "role_assignment_id" },
      { field: "predecessor_override_rate_percent", target: "role_assignments", ret: "override_rate_percent", key: "supersedes_assignment", match: "role_assignment_id" },
      { field: "predecessor_decision_count", target: "role_assignments", ret: "decision_count", key: "supersedes_assignment", match: "role_assignment_id" },
      { field: "has_ungrounded_governing_boundary", target: "roles", ret: "is_governed_by_lapsed_authority", key: "role", match: "role_id" },],
    aggregations: [
      { field: "decision_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "role_assignment_when_scored", kind: "field", field: "role_assignment_id" }] },
      { field: "overridden_decision_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "role_assignment_when_overridden", kind: "field", field: "role_assignment_id" }] },
      { field: "error_correction_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "error_correction_role_assignment_key", kind: "field", field: "role_assignment_id" }] },
      { field: "boundary_violation_count_for_assignment", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "boundary_violation_role_assignment_key", kind: "field", field: "role_assignment_id" }] },] },
  { name: "CommunitiesOfPractice", file: "communities_of_practice", rulebookRows: 2, fields: communitiesOfPracticeFieldTypes,
    compute: (row: any) => computeCommunitiesOfPractice(row as CommunitiesOfPracticeRow),
    lookups: [],
    aggregations: [] },
  { name: "Mentorships", file: "mentorships", rulebookRows: 1, fields: mentorshipsFieldTypes,
    compute: (row: any) => computeMentorships(row as MentorshipsRow),
    lookups: [],
    aggregations: [] },
  { name: "ProcedureTypes", file: "procedure_types", rulebookRows: 2, fields: procedureTypesFieldTypes,
    compute: (row: any) => computeProcedureTypes(row as ProcedureTypesRow),
    lookups: [],
    aggregations: [] },
  { name: "Procedures", file: "procedures", rulebookRows: 2, fields: proceduresFieldTypes,
    compute: (row: any) => computeProcedures(row as ProceduresRow),
    lookups: [],
    aggregations: [] },
  { name: "ProcedureVersions", file: "procedure_versions", rulebookRows: 3, fields: procedureVersionsFieldTypes,
    compute: (row: any) => computeProcedureVersions(row as ProcedureVersionsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "modifier_is_authority", target: "agents", ret: "agent_kind", key: "modified_by_agent", match: "agent_id" },],
    aggregations: [
      { field: "count_of_steps", op: "COUNTIFS", table: "steps", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "count_of_open_knowledge_gaps", op: "COUNTIFS", table: "knowledge_gaps", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }, { range: "status", kind: "literal", literal: vS("Open") }] },
      { field: "specified_step_count", op: "COUNTIFS", table: "steps", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "overdue_review_count", op: "COUNTIFS", table: "review_events", criteria: [{ range: "overdue_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "open_change_request_count", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "open_change_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "open_high_severity_gap_count", op: "COUNTIFS", table: "knowledge_gaps", criteria: [{ range: "open_gap_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "steward_review_cadence_days", op: "SUM", table: "stewardship_assignments", target: "review_cadence_days", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "count_of_stewardship_assignments", op: "COUNTIFS", table: "stewardship_assignments", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "count_of_open_blocking_gaps", op: "COUNTIFS", table: "knowledge_gaps", criteria: [{ range: "is_open_and_blocking", kind: "literal", literal: vB(true) }] },
      { field: "count_of_unapproved_reliance_fragments", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "is_unapproved_but_relied_on", kind: "literal", literal: vB(true) }] },
      { field: "count_of_overdue_gaps", op: "COUNTIFS", table: "knowledge_gaps", criteria: [{ range: "is_overdue_gap", kind: "literal", literal: vB(true) }] },
      { field: "count_of_change_requests", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "count_of_review_events", op: "COUNTIFS", table: "review_events", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "days_since_last_review", op: "COMPOSITE", parts: [{ field: "erb_aggregate0", op: "MAX", table: "review_events", target: "reviewed_at", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] }], scalar: (row: any, aggregates: Record<string, Value>) => { const tc = row as ProcedureVersionsRow; return erbInteger(erbDatetimeDiff(vStr(tc.as_of_instant), (aggregates["erb_aggregate0"] ?? Null), vS("days"))); } },
      { field: "count_of_stale_fragments", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "exceeds_owning_cadence", kind: "literal", literal: vB(true) }] },
      { field: "compound_fragile_fragment_count", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "compound_fragile_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "concentrated_witness_session_count", op: "COUNTIFS", table: "elicitation_sessions", criteria: [{ range: "concentrated_session_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "machine_consumed_unapproved_count", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "machine_consumed_unapproved_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "genuinely_overdue_fragment_count", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "genuinely_overdue_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "awaited_decision_count", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "backlog_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "scoped_open_blocking_gap_count", op: "COUNTIFS", table: "knowledge_gaps", criteria: [{ range: "open_blocking_gap_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "unexercised_human_gate_count", op: "COUNTIFS", table: "steps", criteria: [{ range: "unexercised_gate_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "load_bearing_unapproved_count", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "unapproved_load_bearing_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "unlanded_decision_count", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "unlanded_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "unrehearsed_control_entry_count", op: "COUNTIFS", table: "step_transitions", criteria: [{ range: "unrehearsed_control_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "cadence_breach_count", op: "COUNTIFS", table: "review_events", criteria: [{ range: "cadence_breach_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "valid_fragment_count", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "valid_fragment_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "incoming_supersession_count", op: "COUNTIFS", table: "procedure_version_links", criteria: [{ range: "superseded_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "stalled_implementation_count", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "stalled_implementation_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "undeclared_control_kind_count", op: "COUNTIFS", table: "steps", criteria: [{ range: "undeclared_control_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "approved_change_request_count", op: "COUNTIFS", table: "change_requests", criteria: [{ range: "approved_version_key", kind: "field", field: "procedure_version_id" }] },
      { field: "unwatched_unowned_control_count", op: "COUNTIFS", table: "requirements", criteria: [{ range: "unwatched_unowned_flag", kind: "literal", literal: vS("unwatched-unowned") }] },
      { field: "mining_run_count", op: "COUNTIFS", table: "process_mining_runs", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version_id" }] },
      { field: "drifted_mining_run_count", op: "COUNTIFS", table: "process_mining_runs", criteria: [{ range: "drifted_mining_run_key", kind: "field", field: "procedure_version_id" }] },] },
  { name: "ProcedureVersionLinks", file: "procedure_version_links", rulebookRows: 1, fields: procedureVersionLinksFieldTypes,
    compute: (row: any) => computeProcedureVersionLinks(row as ProcedureVersionLinksRow),
    lookups: [],
    aggregations: [] },
  { name: "ProcedureStatusChanges", file: "procedure_status_changes", rulebookRows: 5, fields: procedureStatusChangesFieldTypes,
    compute: (row: any) => computeProcedureStatusChanges(row as ProcedureStatusChangesRow),
    lookups: [],
    aggregations: [] },
  { name: "Steps", file: "steps", rulebookRows: 17, fields: stepsFieldTypes,
    compute: (row: any) => computeSteps(row as StepsRow),
    lookups: [
      { field: "assigned_role_label", target: "roles", ret: "label", key: "assigned_role", match: "role_id" },
      { field: "assigned_agent_kind", target: "roles", ret: "current_agent_kind", key: "assigned_role", match: "role_id" },
      { field: "assigned_role_is_ungoverned", target: "roles", ret: "is_ungoverned_non_human_role", key: "assigned_role", match: "role_id" },],
    aggregations: [
      { field: "blocking_requirement_count", op: "COUNTIFS", table: "step_requirements", criteria: [{ range: "blocking_step_key", kind: "field", field: "step_id" }] },
      { field: "stale_binding_count", op: "COUNTIFS", table: "operational_bindings", criteria: [{ range: "stale_binding_step_key", kind: "field", field: "step_id" }] },
      { field: "authoritative_stale_count", op: "COUNTIFS", table: "operational_bindings", criteria: [{ range: "authoritative_stale_step_key", kind: "field", field: "step_id" }] },
      { field: "available_exception_count", op: "COUNTIFS", table: "exceptions", criteria: [{ range: "active_exception_step_key", kind: "field", field: "step_id" }] },
      { field: "declared_verification_count", op: "COUNTIFS", table: "step_verifications", criteria: [{ range: "step", kind: "field", field: "step_id" }] },
      { field: "stale_authoritative_binding_count", op: "COUNTIFS", table: "operational_bindings", criteria: [{ range: "step_when_stale", kind: "field", field: "step_id" }] },
      { field: "binding_boundary_count", op: "COUNTIFS", table: "authority_boundaries", criteria: [{ range: "step_when_binding", kind: "field", field: "step_id" }] },
      { field: "unusable_binding_count", op: "COUNTIFS", table: "operational_bindings", criteria: [{ range: "step_when_unusable", kind: "field", field: "step_id" }] },
      { field: "unwarranted_boundary_count", op: "COUNTIFS", table: "authority_boundaries", criteria: [{ range: "unwarranted_boundary_step_key", kind: "field", field: "step_id" }] },
      { field: "software_execution_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "software_execution_step_key", kind: "field", field: "step_id" }] },
      { field: "unwitnessed_blocking_count", op: "COUNTIFS", table: "step_requirements", criteria: [{ range: "unwitnessed_step_key", kind: "field", field: "step_id" }] },] },
  { name: "StepTransitions", file: "step_transitions", rulebookRows: 19, fields: stepTransitionsFieldTypes,
    compute: (row: any) => computeStepTransitions(row as StepTransitionsRow),
    lookups: [
      { field: "target_blocking_requirement_count", target: "steps", ret: "blocking_requirement_count", key: "to_step", match: "step_id" },],
    aggregations: [
      { field: "count_of_from_step_executions", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "step", kind: "field", field: "from_step" }] },
      { field: "count_of_to_step_executions", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "step", kind: "field", field: "to_step" }] },
      { field: "count_of_observed_traversals", op: "COUNTIFS", table: "observed_transitions", criteria: [{ range: "step_transition", kind: "field", field: "step_transition_id" }] },] },
  { name: "Actions", file: "actions", rulebookRows: 9, fields: actionsFieldTypes,
    compute: (row: any) => computeActions(row as ActionsRow),
    lookups: [],
    aggregations: [] },
  { name: "Functions", file: "functions", rulebookRows: 8, fields: functionsFieldTypes,
    compute: (row: any) => computeFunctions(row as FunctionsRow),
    lookups: [],
    aggregations: [] },
  { name: "Tools", file: "tools", rulebookRows: 8, fields: toolsFieldTypes,
    compute: (row: any) => computeTools(row as ToolsRow),
    lookups: [],
    aggregations: [] },
  { name: "StepActions", file: "step_actions", rulebookRows: 9, fields: stepActionsFieldTypes,
    compute: (row: any) => computeStepActions(row as StepActionsRow),
    lookups: [],
    aggregations: [] },
  { name: "StepFunctions", file: "step_functions", rulebookRows: 8, fields: stepFunctionsFieldTypes,
    compute: (row: any) => computeStepFunctions(row as StepFunctionsRow),
    lookups: [],
    aggregations: [] },
  { name: "StepTools", file: "step_tools", rulebookRows: 10, fields: stepToolsFieldTypes,
    compute: (row: any) => computeStepTools(row as StepToolsRow),
    lookups: [],
    aggregations: [] },
  { name: "Requirements", file: "requirements", rulebookRows: 13, fields: requirementsFieldTypes,
    compute: (row: any) => computeRequirements(row as RequirementsRow),
    lookups: [
      { field: "named_witness_field_exists", target: "rulebook_fields", ret: "is_derived", key: "witness_field_name", match: "rulebook_field_id" },
      { field: "accountable_agent", target: "roles", ret: "current_agent", key: "accountable_role", match: "role_id" },],
    aggregations: [
      { field: "satisfaction_record_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "requirement", kind: "field", field: "requirement_id" }] },
      { field: "step_binding_count", op: "COUNTIFS", table: "step_requirements", criteria: [{ range: "requirement", kind: "field", field: "requirement_id" }] },
      { field: "negative_outcome_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "negative_outcome_requirement_key", kind: "field", field: "requirement_id" }] },
      { field: "unexercised_binding_count", op: "COUNTIFS", table: "step_requirements", criteria: [{ range: "unexercised_binding_requirement_key", kind: "field", field: "requirement_id" }] },] },
  { name: "StepRequirements", file: "step_requirements", rulebookRows: 15, fields: stepRequirementsFieldTypes,
    compute: (row: any) => computeStepRequirements(row as StepRequirementsRow),
    lookups: [
      { field: "requirement_is_blocking", target: "requirements", ret: "is_blocking", key: "requirement", match: "requirement_id" },
      { field: "requirement_lacks_witness", target: "requirements", ret: "is_unwitnessed_blocking_control", key: "requirement", match: "requirement_id" },],
    aggregations: [
      { field: "satisfaction_count_for_binding", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "binding_key", kind: "field", field: "step_requirement_id" }] },] },
  { name: "StepVerifications", file: "step_verifications", rulebookRows: 11, fields: stepVerificationsFieldTypes,
    compute: (row: any) => computeStepVerifications(row as StepVerificationsRow),
    lookups: [],
    aggregations: [] },
  { name: "Rationales", file: "rationales", rulebookRows: 4, fields: rationalesFieldTypes,
    compute: (row: any) => computeRationales(row as RationalesRow),
    lookups: [],
    aggregations: [] },
  { name: "Exceptions", file: "exceptions", rulebookRows: 4, fields: exceptionsFieldTypes,
    compute: (row: any) => computeExceptions(row as ExceptionsRow),
    lookups: [],
    aggregations: [] },
  { name: "Resources", file: "resources", rulebookRows: 8, fields: resourcesFieldTypes,
    compute: (row: any) => computeResources(row as ResourcesRow),
    lookups: [],
    aggregations: [] },
  { name: "ProcedureResources", file: "procedure_resources", rulebookRows: 8, fields: procedureResourcesFieldTypes,
    compute: (row: any) => computeProcedureResources(row as ProcedureResourcesRow),
    lookups: [],
    aggregations: [] },
  { name: "ElicitationSessions", file: "elicitation_sessions", rulebookRows: 3, fields: elicitationSessionsFieldTypes,
    compute: (row: any) => computeElicitationSessions(row as ElicitationSessionsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "practitioner_is_still_engaged", target: "agents", ret: "is_still_engaged", key: "practitioner_agent", match: "agent_id" },],
    aggregations: [
      { field: "valid_fragments_produced", op: "COUNTIFS", table: "knowledge_fragments", criteria: [{ range: "valid_fragment_session_key", kind: "field", field: "elicitation_session_id" }] },] },
  { name: "KnowledgeFragments", file: "knowledge_fragments", rulebookRows: 7, fields: knowledgeFragmentsFieldTypes,
    compute: (row: any) => computeKnowledgeFragments(row as KnowledgeFragmentsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "source_agent_is_still_engaged", target: "agents", ret: "is_still_engaged", key: "source_agent", match: "agent_id" },
      { field: "source_agent_kind", target: "agents", ret: "agent_kind", key: "source_agent", match: "agent_id" },
      { field: "step_procedure_version_status", target: "steps", ret: "procedure_version", key: "step", match: "step_id" },
      { field: "is_attached_to_live_version", target: "procedure_versions", ret: "is_live", key: "procedure_version", match: "procedure_version_id" },
      { field: "evidence_age_days", target: "elicitation_sessions", ret: "days_since_elicited", key: "elicitation_session", match: "elicitation_session_id" },
      { field: "is_from_single_witness", target: "elicitation_sessions", ret: "is_single_witness_method", key: "elicitation_session", match: "elicitation_session_id" },
      { field: "owner_agent", target: "roles", ret: "current_agent", key: "owner_role", match: "role_id" },
      { field: "owning_version_cadence_days", target: "procedure_versions", ret: "steward_review_cadence_days", key: "procedure_version", match: "procedure_version_id" },
      { field: "owner_role_agent_kind", target: "roles", ret: "current_agent_kind", key: "owner_role", match: "role_id" },
      { field: "review_cadence_days", target: "procedure_versions", ret: "steward_review_cadence_days", key: "procedure_version", match: "procedure_version_id" },
      { field: "owner_role_assignment_valid_from", target: "roles", ret: "current_assignment_valid_from", key: "owner_role", match: "role_id" },
      { field: "consuming_step_is_software_assigned", target: "steps", ret: "is_software_assigned", key: "step", match: "step_id" },
      { field: "consuming_step_agent_kind", target: "steps", ret: "assigned_agent_kind", key: "step", match: "step_id" },
      { field: "owner_role_is_vacated", target: "roles", ret: "is_vacated_role", key: "owner_role", match: "role_id" },],
    aggregations: [
      { field: "is_invoked_by_an_exception", op: "COUNTIFS", table: "exceptions", criteria: [{ range: "trigger_step", kind: "field", field: "step" }] },
      { field: "ratified_boundary_count", op: "COUNTIFS", table: "authority_boundaries", criteria: [{ range: "ratifying_fragment_key", kind: "field", field: "knowledge_fragment_id" }] },] },
  { name: "KnowledgeGaps", file: "knowledge_gaps", rulebookRows: 8, fields: knowledgeGapsFieldTypes,
    compute: (row: any) => computeKnowledgeGaps(row as KnowledgeGapsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "owner_agent", target: "roles", ret: "current_agent", key: "owner_role", match: "role_id" },
      { field: "owner_is_still_engaged", target: "agents", ret: "is_still_engaged", key: "owner_agent", match: "agent_id" },
      { field: "owner_role_is_vacated", target: "roles", ret: "is_vacated_role", key: "owner_role", match: "role_id" },],
    aggregations: [] },
  { name: "FAQs", file: "fa_qs", rulebookRows: 3, fields: fAQsFieldTypes,
    compute: (row: any) => computeFAQs(row as FAQsRow),
    lookups: [],
    aggregations: [] },
  { name: "Explanations", file: "explanations", rulebookRows: 2, fields: explanationsFieldTypes,
    compute: (row: any) => computeExplanations(row as ExplanationsRow),
    lookups: [],
    aggregations: [] },
  { name: "ProcedureExecutions", file: "procedure_executions", rulebookRows: 2, fields: procedureExecutionsFieldTypes,
    compute: (row: any) => computeProcedureExecutions(row as ProcedureExecutionsRow),
    lookups: [
      { field: "expected_step_count", target: "procedure_versions", ret: "specified_step_count", key: "procedure_version", match: "procedure_version_id" },
      { field: "executed_version_is_fit", target: "procedure_versions", ret: "is_fit_to_execute", key: "procedure_version", match: "procedure_version_id" },],
    aggregations: [
      { field: "completed_step_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "completed_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "control_breach_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "control_breach_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "late_step_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "late_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "unevaluated_blocking_total", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "unevaluated_blocking_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "separation_violation_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "separation_violation_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "asserted_only_control_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "asserted_only_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "unreachable_handling_failure_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "unreachable_failure_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "retention_breach_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "retention_breach_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "cleared_legal_review_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "cleared_legal_review_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "abandoned_failure_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "abandoned_failure_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "delivered_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "reached_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "total_delivery_attempt_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "procedure_execution", kind: "field", field: "procedure_execution_id" }] },
      { field: "mishandled_refusal_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "refusal_failure_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "unclean_step_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "procedure_execution_when_unclean", kind: "field", field: "procedure_execution_id" }] },
      { field: "count_of_approval_executions", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "is_approval_execution", kind: "literal", literal: vB(true) }] },
      { field: "count_of_delivery_executions", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "step", kind: "literal", literal: vS("policy-07") }] },
      { field: "invalid_approval_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "run_when_invalid_approval", kind: "field", field: "procedure_execution_id" }] },
      { field: "vacuously_clean_step_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "vacuously_clean_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "preparation_step_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "preparation_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "approval_step_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "approval_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "ungoverned_divergence_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "ungoverned_divergence_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "computedly_witnessed_control_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "computed_witness_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "interested_party_assertion_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "interested_assertion_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "independent_human_observation_count", op: "COUNTIFS", table: "verification_outcomes", criteria: [{ range: "independent_observation_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "self_attested_approval_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "self_attested_approval_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "latest_attestation_instant", op: "MAX", table: "attestations", target: "signed_at", criteria: [{ range: "procedure_execution", kind: "field", field: "procedure_execution_id" }] },
      { field: "attestation_count", op: "COUNTIFS", table: "attestations", criteria: [{ range: "procedure_execution", kind: "field", field: "procedure_execution_id" }] },
      { field: "post_attestation_score_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "post_attestation_score_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "intended_recipient_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "intent_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "reached_recipient_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "delivered_intent_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "silently_dropped_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "dropped_intent_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "unrecorded_refusal_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "unrecorded_refusal_execution_key", kind: "field", field: "procedure_execution_id" }] },
      { field: "independently_confirmed_intent_count", op: "COUNTIFS", table: "send_intents", criteria: [{ range: "independently_confirmed_execution_key", kind: "field", field: "procedure_execution_id" }] },] },
  { name: "StepExecutions", file: "step_executions", rulebookRows: 12, fields: stepExecutionsFieldTypes,
    compute: (row: any) => computeStepExecutions(row as StepExecutionsRow),
    lookups: [
      { field: "expected_duration_minutes", target: "steps", ret: "expected_duration_minutes", key: "step", match: "step_id" },
      { field: "expected_blocking_count", target: "steps", ret: "blocking_requirement_count", key: "step", match: "step_id" },
      { field: "stale_authoritative_source_count", target: "steps", ret: "authoritative_stale_count", key: "step", match: "step_id" },
      { field: "available_exception_count_for_step", target: "steps", ret: "available_exception_count", key: "step", match: "step_id" },
      { field: "expected_verification_count", target: "steps", ret: "declared_verification_count", key: "step", match: "step_id" },
      { field: "step_is_preparation", target: "steps", ret: "is_preparation_step", key: "step", match: "step_id" },
      { field: "step_is_approval", target: "steps", ret: "is_approval_step", key: "step", match: "step_id" },
      { field: "required_role_for_step", target: "steps", ret: "assigned_role", key: "step", match: "step_id" },
      { field: "executor_agent_kind", target: "agents", ret: "agent_kind", key: "executed_by_agent", match: "agent_id" },
      { field: "step_requires_human_confirmation", target: "steps", ret: "requires_human_confirmation", key: "step", match: "step_id" },
      { field: "assigned_role", target: "steps", ret: "assigned_role", key: "step", match: "step_id" },
      { field: "role_current_agent", target: "roles", ret: "current_agent", key: "assigned_role", match: "role_id" },
      { field: "inputs_were_fresh_at_run", target: "steps", ret: "inputs_are_fresh", key: "step", match: "step_id" },
      { field: "required_blocking_count", target: "steps", ret: "blocking_requirement_count", key: "step", match: "step_id" },
      { field: "executing_agent_kind", target: "agents", ret: "agent_kind", key: "executed_by_agent", match: "agent_id" },
      { field: "step_is_software_assigned", target: "steps", ret: "is_software_assigned", key: "step", match: "step_id" },
      { field: "is_approval_execution", target: "steps", ret: "is_human_approval_gate", key: "step", match: "step_id" },
      { field: "requires_human_confirmation", target: "steps", ret: "requires_human_confirmation", key: "step", match: "step_id" },
      { field: "inputs_were_usable", target: "steps", ret: "all_sources_usable", key: "step", match: "step_id" },
      { field: "step_control_kind", target: "steps", ret: "control_kind", key: "step", match: "step_id" },
      { field: "has_approved_change_coverage", target: "procedure_versions", ret: "has_approved_change_request", key: "version_of_step", match: "procedure_version_id" },
      { field: "version_of_step", target: "steps", ret: "procedure_version", key: "step", match: "step_id" },],
    aggregations: [
      { field: "blocking_unmet_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "step_execution", kind: "field", field: "step_execution_id" }, { range: "is_blocking_and_unmet", kind: "literal", literal: vB(true) }] },
      { field: "blocking_unmet_count_safe", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "blocking_unmet_step_key", kind: "field", field: "step_execution_id" }] },
      { field: "evaluated_blocking_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "blocking_satisfaction_step_key", kind: "field", field: "step_execution_id" }] },
      { field: "performed_verification_count", op: "COUNTIFS", table: "verification_outcomes", criteria: [{ range: "step_execution", kind: "field", field: "step_execution_id" }] },
      { field: "prepared_by_this_agent_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "preparer_agent_key", kind: "field", field: "approver_agent_key" }] },
      { field: "executor_authority_count", op: "COUNTIFS", table: "role_assignments", criteria: [{ range: "agent_role_key", kind: "field", field: "executor_role_key" }] },
      { field: "self_witnessed_verification_count", op: "COUNTIFS", table: "verification_outcomes", criteria: [{ range: "self_witnessed_step_key", kind: "field", field: "step_execution_id" }] },
      { field: "unbacked_verification_count", op: "COUNTIFS", table: "verification_outcomes", criteria: [{ range: "unbacked_step_key", kind: "field", field: "step_execution_id" }] },
      { field: "exception_invocation_count", op: "COUNTIFS", table: "exception_invocations", criteria: [{ range: "step_execution", kind: "field", field: "step_execution_id" }] },
      { field: "unresolved_issue_count", op: "COUNTIFS", table: "issue_occurrences", criteria: [{ range: "step_execution_when_unresolved", kind: "field", field: "step_execution_id" }] },
      { field: "evaluated_requirement_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "step_execution_when_scored", kind: "field", field: "step_execution_id" }] },
      { field: "unconfirmed_non_human_decision_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "step_execution_when_unconfirmed", kind: "field", field: "step_execution_id" }] },
      { field: "unfalsified_clearance_count", op: "COUNTIFS", table: "requirement_satisfactions", criteria: [{ range: "unfalsified_clearance_step_key", kind: "field", field: "step_execution_id" }] },
      { field: "stale_at_run_count", op: "COUNTIFS", table: "binding_observations", criteria: [{ range: "stale_at_run_step_key", kind: "field", field: "step_execution_id" }] },
      { field: "uncorroborated_pass_count", op: "COUNTIFS", table: "verification_outcomes", criteria: [{ range: "uncorroborated_pass_step_key", kind: "field", field: "step_execution_id" }] },] },
  { name: "RequirementSatisfactions", file: "requirement_satisfactions", rulebookRows: 8, fields: requirementSatisfactionsFieldTypes,
    compute: (row: any) => computeRequirementSatisfactions(row as RequirementSatisfactionsRow),
    lookups: [
      { field: "requirement_is_blocking", target: "requirements", ret: "is_blocking", key: "requirement", match: "requirement_id" },
      { field: "evaluator_agent_kind", target: "agents", ret: "agent_kind", key: "evaluated_by_agent", match: "agent_id" },
      { field: "requirement_has_computed_witness", target: "requirements", ret: "has_computed_witness", key: "requirement", match: "requirement_id" },
      { field: "parent_procedure_execution", target: "step_executions", ret: "procedure_execution", key: "step_execution", match: "step_execution_id" },
      { field: "requirement_is_approval_type", target: "requirements", ret: "requirement_type", key: "requirement", match: "requirement_id" },
      { field: "procedure_execution_of_satisfaction", target: "step_executions", ret: "procedure_execution", key: "step_execution", match: "step_execution_id" },
      { field: "requirement_is_unfalsified", target: "requirements", ret: "is_unfalsified_control", key: "requirement", match: "requirement_id" },
      { field: "spec_step_of_execution", target: "step_executions", ret: "step", key: "step_execution", match: "step_execution_id" },
      { field: "binding_key", target: "step_requirements", ret: "step_requirement_id", key: "requirement_satisfaction_id", match: "step_requirement_id" },
      { field: "scored_step_executor_agent", target: "step_executions", ret: "executed_by_agent", key: "step_execution", match: "step_execution_id" },
      { field: "run_owner_agent", target: "procedure_executions", ret: "executed_by_agent", key: "parent_procedure_execution", match: "procedure_execution_id" },
      { field: "step_executor_agent", target: "step_executions", ret: "executed_by_agent", key: "step_execution", match: "step_execution_id" },
      { field: "attestation_instant_for_run", target: "procedure_executions", ret: "latest_attestation_instant", key: "parent_procedure_execution", match: "procedure_execution_id" },],
    aggregations: [] },
  { name: "Errors", file: "errors", rulebookRows: 2, fields: errorsFieldTypes,
    compute: (row: any) => computeErrors(row as ErrorsRow),
    lookups: [],
    aggregations: [] },
  { name: "IssueOccurrences", file: "issue_occurrences", rulebookRows: 2, fields: issueOccurrencesFieldTypes,
    compute: (row: any) => computeIssueOccurrences(row as IssueOccurrencesRow),
    lookups: [],
    aggregations: [] },
  { name: "UserQuestions", file: "user_questions", rulebookRows: 2, fields: userQuestionsFieldTypes,
    compute: (row: any) => computeUserQuestions(row as UserQuestionsRow),
    lookups: [],
    aggregations: [] },
  { name: "UserFeedback", file: "user_feedback", rulebookRows: 2, fields: userFeedbackFieldTypes,
    compute: (row: any) => computeUserFeedback(row as UserFeedbackRow),
    lookups: [],
    aggregations: [] },
  { name: "StewardshipAssignments", file: "stewardship_assignments", rulebookRows: 2, fields: stewardshipAssignmentsFieldTypes,
    compute: (row: any) => computeStewardshipAssignments(row as StewardshipAssignmentsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },],
    aggregations: [
      { field: "count_of_review_events", op: "COUNTIFS", table: "review_events", criteria: [{ range: "procedure_version", kind: "field", field: "procedure_version" }] },] },
  { name: "ChangeRequests", file: "change_requests", rulebookRows: 2, fields: changeRequestsFieldTypes,
    compute: (row: any) => computeChangeRequests(row as ChangeRequestsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "authority_agent", target: "roles", ret: "current_agent", key: "authority_role", match: "role_id" },
      { field: "authority_role_label", target: "roles", ret: "label", key: "authority_role", match: "role_id" },
      { field: "touches_live_version", target: "procedure_versions", ret: "is_live", key: "procedure_version", match: "procedure_version_id" },],
    aggregations: [] },
  { name: "ReviewEvents", file: "review_events", rulebookRows: 3, fields: reviewEventsFieldTypes,
    compute: (row: any) => computeReviewEvents(row as ReviewEventsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "promised_cadence_days", target: "procedure_versions", ret: "steward_review_cadence_days", key: "procedure_version", match: "procedure_version_id" },],
    aggregations: [] },
  { name: "LearningActivities", file: "learning_activities", rulebookRows: 2, fields: learningActivitiesFieldTypes,
    compute: (row: any) => computeLearningActivities(row as LearningActivitiesRow),
    lookups: [],
    aggregations: [] },
  { name: "OperationalBindings", file: "operational_bindings", rulebookRows: 5, fields: operationalBindingsFieldTypes,
    compute: (row: any) => computeOperationalBindings(row as OperationalBindingsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "resource_is_approved", target: "resources", ret: "is_approved_source", key: "resource", match: "resource_id" },],
    aggregations: [] },
  { name: "CommunicationPolicies", file: "communication_policies", rulebookRows: 2, fields: communicationPoliciesFieldTypes,
    compute: (row: any) => computeCommunicationPolicies(row as CommunicationPoliciesRow),
    lookups: [],
    aggregations: [
      { field: "consent_violation_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "policy_channel", kind: "field", field: "communication_policy_id" }, { range: "is_consent_violation", kind: "literal", literal: vB(true) }] },
      { field: "quiet_hours_violation_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "quiet_hours_violation_policy_key", kind: "field", field: "communication_policy_id" }] },] },
  { name: "MessageTemplates", file: "message_templates", rulebookRows: 2, fields: messageTemplatesFieldTypes,
    compute: (row: any) => computeMessageTemplates(row as MessageTemplatesRow),
    lookups: [
      { field: "policy_max_message_length", target: "communication_policies", ret: "max_message_length", key: "communication_policy", match: "communication_policy_id" },
      { field: "policy_max_segments", target: "communication_policies", ret: "max_segments", key: "communication_policy", match: "communication_policy_id" },
      { field: "last_approved_body_hash", target: "template_approvals", ret: "approved_body_hash", key: "last_valid_approval", match: "template_approval_id" },
      { field: "last_approval_at", target: "template_approvals", ret: "decided_at", key: "last_valid_approval", match: "template_approval_id" },],
    aggregations: [
      { field: "valid_approval_count", op: "COUNTIFS", table: "template_approvals", criteria: [{ range: "valid_approval_template_key", kind: "field", field: "message_template_id" }] },
      { field: "drifted_send_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "drifted_send_template_key", kind: "field", field: "message_template_id" }] },
      { field: "unanswered_delivery_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "unanswered_template_key", kind: "field", field: "message_template_id" }] },
      { field: "transmitted_delivery_count", op: "COUNTIFS", table: "message_deliveries", criteria: [{ range: "transmitted_template_key", kind: "field", field: "message_template_id" }] },] },
  { name: "SemanticMappings", file: "semantic_mappings", rulebookRows: 41, fields: semanticMappingsFieldTypes,
    compute: (row: any) => computeSemanticMappings(row as SemanticMappingsRow),
    lookups: [],
    aggregations: [] },
  { name: "WitnessLoops", file: "witness_loops", rulebookRows: 3, fields: witnessLoopsFieldTypes,
    compute: (row: any) => computeWitnessLoops(row as WitnessLoopsRow),
    lookups: [],
    aggregations: [
      { field: "question_count", op: "COUNTIFS", table: "role_questions", criteria: [{ range: "witness_loop", kind: "field", field: "witness_loop_id" }] },] },
  { name: "RoleQuestions", file: "role_questions", rulebookRows: 108, fields: roleQuestionsFieldTypes,
    compute: (row: any) => computeRoleQuestions(row as RoleQuestionsRow),
    lookups: [],
    aggregations: [
      { field: "predicate_count", op: "COUNTIFS", table: "rulebook_fields", criteria: [{ range: "invented_for_question", kind: "field", field: "role_question_id" }] },] },
  { name: "RulebookFields", file: "rulebook_fields", rulebookRows: 1771, fields: rulebookFieldsFieldTypes,
    compute: (row: any) => computeRulebookFields(row as RulebookFieldsRow),
    lookups: [],
    aggregations: [] },
  { name: "TestSuites", file: "test_suites", rulebookRows: 6, fields: testSuitesFieldTypes,
    compute: (row: any) => computeTestSuites(row as TestSuitesRow),
    lookups: [],
    aggregations: [
      { field: "test_count", op: "COUNTIFS", table: "test_cases", criteria: [{ range: "suite", kind: "field", field: "test_suite_id" }] },
      { field: "pass_count", op: "COUNTIFS", table: "test_cases", criteria: [{ range: "passing_suite_key", kind: "field", field: "test_suite_id" }] },
      { field: "blocking_fail_count", op: "COUNTIFS", table: "test_cases", criteria: [{ range: "needs_attention_suite_key", kind: "field", field: "test_suite_id" }] },] },
  { name: "TestCases", file: "test_cases", rulebookRows: 1794, fields: testCasesFieldTypes,
    compute: (row: any) => computeTestCases(row as TestCasesRow),
    lookups: [],
    aggregations: [] },
  { name: "ERBVersions", file: "erb_versions", rulebookRows: 1, fields: eRBVersionsFieldTypes,
    compute: (row: any) => computeERBVersions(row as ERBVersionsRow),
    lookups: [],
    aggregations: [] },
  { name: "ERBCustomizations", file: "erb_customizations", rulebookRows: 0, fields: eRBCustomizationsFieldTypes,
    compute: (row: any) => computeERBCustomizations(row as ERBCustomizationsRow),
    lookups: [],
    aggregations: [] },
  { name: "__meta__", file: "__meta__", rulebookRows: 19, fields: __meta__FieldTypes,
    compute: (row: any) => compute__meta__(row as __meta__Row),
    lookups: [],
    aggregations: [] },
  { name: "ExceptionInvocations", file: "exception_invocations", rulebookRows: 1, fields: exceptionInvocationsFieldTypes,
    compute: (row: any) => computeExceptionInvocations(row as ExceptionInvocationsRow),
    lookups: [
      { field: "expected_handling", target: "exceptions", ret: "handling", key: "exception", match: "exception_id" },
      { field: "required_approval_role", target: "exceptions", ret: "approval_role", key: "exception", match: "exception_id" },
      { field: "required_approval_role_holder", target: "roles", ret: "current_agent", key: "required_approval_role", match: "role_id" },
      { field: "invoker_agent_kind", target: "agents", ret: "agent_kind", key: "invoked_by_agent", match: "agent_id" },
      { field: "parent_procedure_execution", target: "step_executions", ret: "procedure_execution", key: "step_execution", match: "step_execution_id" },],
    aggregations: [
      { field: "approver_prepared_count", op: "COUNTIFS", table: "step_executions", criteria: [{ range: "preparer_agent_key", kind: "field", field: "invoker_also_prepared_key" }] },] },
  { name: "VerificationOutcomes", file: "verification_outcomes", rulebookRows: 4, fields: verificationOutcomesFieldTypes,
    compute: (row: any) => computeVerificationOutcomes(row as VerificationOutcomesRow),
    lookups: [
      { field: "expected_signal_value", target: "step_verifications", ret: "expected_signal_value", key: "step_verification", match: "step_verification_id" },
      { field: "signal_identifier", target: "step_verifications", ret: "signal_identifier", key: "step_verification", match: "step_verification_id" },
      { field: "step_executor_agent", target: "step_executions", ret: "executed_by_agent", key: "step_execution", match: "step_execution_id" },
      { field: "observer_is_non_human", target: "agents", ret: "is_non_human", key: "observed_by_agent", match: "agent_id" },
      { field: "parent_procedure_execution_of_outcome", target: "step_executions", ret: "procedure_execution", key: "step_execution", match: "step_execution_id" },],
    aggregations: [] },
  { name: "ObservedTransitions", file: "observed_transitions", rulebookRows: 10, fields: observedTransitionsFieldTypes,
    compute: (row: any) => computeObservedTransitions(row as ObservedTransitionsRow),
    lookups: [],
    aggregations: [] },
  { name: "Recipients", file: "recipients", rulebookRows: 5, fields: recipientsFieldTypes,
    compute: (row: any) => computeRecipients(row as RecipientsRow),
    lookups: [],
    aggregations: [] },
  { name: "MessageDeliveries", file: "message_deliveries", rulebookRows: 6, fields: messageDeliveriesFieldTypes,
    compute: (row: any) => computeMessageDeliveries(row as MessageDeliveriesRow),
    lookups: [
      { field: "policy_channel", target: "message_templates", ret: "communication_policy", key: "message_template", match: "message_template_id" },
      { field: "channel_name", target: "communication_policies", ret: "channel", key: "policy_channel", match: "communication_policy_id" },
      { field: "policy_requires_consent", target: "communication_policies", ret: "consent_required", key: "policy_channel", match: "communication_policy_id" },
      { field: "recipient_has_sms_consent", target: "recipients", ret: "has_sms_consent", key: "recipient", match: "recipient_id" },
      { field: "policy_quiet_hours_start_hour", target: "communication_policies", ret: "quiet_hours_start_hour", key: "policy_channel", match: "communication_policy_id" },
      { field: "policy_quiet_hours_end_hour", target: "communication_policies", ret: "quiet_hours_end_hour", key: "policy_channel", match: "communication_policy_id" },
      { field: "recipient_is_unreachable", target: "recipients", ret: "is_unreachable", key: "recipient", match: "recipient_id" },
      { field: "invoked_exception_condition", target: "exceptions", ret: "condition", key: "invoked_exception", match: "exception_id" },
      { field: "policy_retention_days", target: "communication_policies", ret: "retention_days", key: "policy_channel", match: "communication_policy_id" },
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "sending_step_execution_step", target: "step_executions", ret: "step", key: "step_execution", match: "step_execution_id" },
      { field: "execution_has_cleared_legal_review", target: "procedure_executions", ret: "has_cleared_legal_review", key: "procedure_execution", match: "procedure_execution_id" },
      { field: "policy_max_message_length_at_send", target: "communication_policies", ret: "max_message_length", key: "policy_channel", match: "communication_policy_id" },
      { field: "policy_max_segments_at_send", target: "communication_policies", ret: "max_segments", key: "policy_channel", match: "communication_policy_id" },
      { field: "template_has_valid_approval", target: "message_templates", ret: "has_valid_approval", key: "message_template", match: "message_template_id" },
      { field: "policy_required_opt_out_phrase", target: "communication_policies", ret: "required_opt_out_phrase", key: "policy_channel", match: "communication_policy_id" },
      { field: "template_was_sendable", target: "message_templates", ret: "is_sendable_under_approval", key: "message_template", match: "message_template_id" },
      { field: "current_last_approval_at", target: "message_templates", ret: "last_approval_at", key: "message_template", match: "message_template_id" },],
    aggregations: [] },
  { name: "TemplateApprovals", file: "template_approvals", rulebookRows: 2, fields: templateApprovalsFieldTypes,
    compute: (row: any) => computeTemplateApprovals(row as TemplateApprovalsRow),
    lookups: [
      { field: "template_policy", target: "message_templates", ret: "communication_policy", key: "message_template", match: "message_template_id" },
      { field: "required_approval_role", target: "communication_policies", ret: "approval_role", key: "template_policy", match: "communication_policy_id" },],
    aggregations: [] },
  { name: "SendIntents", file: "send_intents", rulebookRows: 7, fields: sendIntentsFieldTypes,
    compute: (row: any) => computeSendIntents(row as SendIntentsRow),
    lookups: [
      { field: "intent_policy", target: "message_templates", ret: "communication_policy", key: "message_template", match: "message_template_id" },
      { field: "intent_channel", target: "communication_policies", ret: "channel", key: "intent_policy", match: "communication_policy_id" },
      { field: "policy_is_active", target: "communication_policies", ret: "is_active_policy", key: "intent_policy", match: "communication_policy_id" },
      { field: "intent_requires_consent", target: "communication_policies", ret: "consent_required", key: "intent_policy", match: "communication_policy_id" },
      { field: "recipient_has_channel_consent", target: "recipients", ret: "has_sms_consent", key: "recipient", match: "recipient_id" },
      { field: "recipient_is_sms_reachable", target: "recipients", ret: "is_sms_reachable", key: "recipient", match: "recipient_id" },
      { field: "recipient_is_email_reachable", target: "recipients", ret: "is_email_reachable", key: "recipient", match: "recipient_id" },
      { field: "intent_quiet_start_hour", target: "communication_policies", ret: "quiet_hours_start_hour", key: "intent_policy", match: "communication_policy_id" },
      { field: "intent_quiet_end_hour", target: "communication_policies", ret: "quiet_hours_end_hour", key: "intent_policy", match: "communication_policy_id" },
      { field: "intent_max_message_length", target: "communication_policies", ret: "max_message_length", key: "intent_policy", match: "communication_policy_id" },
      { field: "intent_max_segments", target: "communication_policies", ret: "max_segments", key: "intent_policy", match: "communication_policy_id" },
      { field: "intent_required_opt_out_phrase", target: "communication_policies", ret: "required_opt_out_phrase", key: "intent_policy", match: "communication_policy_id" },
      { field: "template_is_sendable", target: "message_templates", ret: "is_sendable_under_approval", key: "message_template", match: "message_template_id" },
      { field: "execution_has_legal_clearance", target: "procedure_executions", ret: "has_cleared_legal_review", key: "procedure_execution", match: "procedure_execution_id" },
      { field: "intent_approval_role", target: "communication_policies", ret: "approval_role", key: "intent_policy", match: "communication_policy_id" },
      { field: "approval_role_agent_kind", target: "roles", ret: "current_agent_kind", key: "intent_approval_role", match: "role_id" },
      { field: "resulting_delivery_was_transmitted", target: "message_deliveries", ret: "was_actually_transmitted", key: "resulting_delivery", match: "message_delivery_id" },
      { field: "resulting_delivery_exception", target: "message_deliveries", ret: "invoked_exception", key: "resulting_delivery", match: "message_delivery_id" },
      { field: "alternate_attempt_was_cleared", target: "send_intents", ret: "is_cleared_to_send", key: "alternate_channel_intent", match: "send_intent_id" },
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "retry_was_cleared", target: "send_intents", ret: "is_cleared_to_send", key: "retry_intent", match: "send_intent_id" },
      { field: "enforced_by_unauthorized_agent", target: "role_assignments", ret: "is_unauthorized_enforcement_agent", key: "evaluating_role_assignment", match: "role_assignment_id" },
      { field: "recipient_consent_status_raw", target: "recipients", ret: "sms_consent_status", key: "recipient", match: "recipient_id" },],
    aggregations: [] },
  { name: "AgentDecisionRecords", file: "agent_decision_records", rulebookRows: 3, fields: agentDecisionRecordsFieldTypes,
    compute: (row: any) => computeAgentDecisionRecords(row as AgentDecisionRecordsRow),
    lookups: [
      { field: "deciding_agent_kind", target: "agents", ret: "agent_kind", key: "deciding_agent", match: "agent_id" },
      { field: "step_of_decision", target: "step_executions", ret: "step", key: "step_execution", match: "step_execution_id" },
      { field: "reviewer_agent_kind", target: "agents", ret: "agent_kind", key: "reviewed_by_agent", match: "agent_id" },],
    aggregations: [
      { field: "matching_boundary_count", op: "COUNTIFS", table: "authority_boundaries", criteria: [{ range: "boundary_match_key", kind: "field", field: "boundary_match_key" }] },] },
  { name: "DeliveredCommunications", file: "delivered_communications", rulebookRows: 1, fields: deliveredCommunicationsFieldTypes,
    compute: (row: any) => computeDeliveredCommunications(row as DeliveredCommunicationsRow),
    lookups: [
      { field: "authorized_at", target: "step_executions", ret: "ended_at", key: "authorizing_step_execution", match: "step_execution_id" },],
    aggregations: [] },
  { name: "AuthorityBoundaries", file: "authority_boundaries", rulebookRows: 3, fields: authorityBoundariesFieldTypes,
    compute: (row: any) => computeAuthorityBoundaries(row as AuthorityBoundariesRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "ratifying_fragment_is_valid", target: "knowledge_fragments", ret: "is_currently_valid", key: "ratified_by_knowledge_fragment", match: "knowledge_fragment_id" },
      { field: "ratifying_fragment_is_overdue", target: "knowledge_fragments", ret: "is_overdue_for_review", key: "ratified_by_knowledge_fragment", match: "knowledge_fragment_id" },
      { field: "ratifying_fragment_is_single_witness", target: "knowledge_fragments", ret: "is_from_single_witness", key: "ratified_by_knowledge_fragment", match: "knowledge_fragment_id" },
      { field: "ratifying_fragment_status", target: "knowledge_fragments", ret: "status", key: "ratified_by_knowledge_fragment", match: "knowledge_fragment_id" },],
    aggregations: [
      { field: "violation_count", op: "COUNTIFS", table: "agent_decision_records", criteria: [{ range: "boundary_match_key", kind: "field", field: "boundary_match_key" }] },] },
  { name: "BindingObservations", file: "binding_observations", rulebookRows: 0, fields: bindingObservationsFieldTypes,
    compute: (row: any) => computeBindingObservations(row as BindingObservationsRow),
    lookups: [
      { field: "sla_minutes_at_run", target: "operational_bindings", ret: "freshness_sla_minutes", key: "operational_binding", match: "operational_binding_id" },
      { field: "is_authoritative_binding", target: "operational_bindings", ret: "is_authoritative", key: "operational_binding", match: "operational_binding_id" },],
    aggregations: [] },
  { name: "Attestations", file: "attestations", rulebookRows: 0, fields: attestationsFieldTypes,
    compute: (row: any) => computeAttestations(row as AttestationsRow),
    lookups: [
      { field: "version_is_fit_now", target: "procedure_executions", ret: "executed_version_is_fit", key: "procedure_execution", match: "procedure_execution_id" },
      { field: "assurance_grade_now", target: "procedure_executions", ret: "assurance_grade", key: "procedure_execution", match: "procedure_execution_id" },],
    aggregations: [] },
  { name: "AppRoleProfiles", file: "app_role_profiles", rulebookRows: 12, fields: appRoleProfilesFieldTypes,
    compute: (row: any) => computeAppRoleProfiles(row as AppRoleProfilesRow),
    lookups: [],
    aggregations: [
      { field: "route_count", op: "COUNTIFS", table: "app_routes", criteria: [{ range: "owning_role", kind: "field", field: "role" }] },] },
  { name: "AppNavGroups", file: "app_nav_groups", rulebookRows: 23, fields: appNavGroupsFieldTypes,
    compute: (row: any) => computeAppNavGroups(row as AppNavGroupsRow),
    lookups: [],
    aggregations: [
      { field: "route_count", op: "COUNTIFS", table: "app_routes", criteria: [{ range: "nav_group", kind: "field", field: "app_nav_group_id" }] },] },
  { name: "AppRoutes", file: "app_routes", rulebookRows: 149, fields: appRoutesFieldTypes,
    compute: (row: any) => computeAppRoutes(row as AppRoutesRow),
    lookups: [],
    aggregations: [
      { field: "question_count", op: "COUNTIFS", table: "app_route_questions", criteria: [{ range: "route", kind: "field", field: "app_route_id" }] },
      { field: "reference_count", op: "COUNTIFS", table: "app_route_references", criteria: [{ range: "from_route", kind: "field", field: "app_route_id" }] },] },
  { name: "AppRouteQuestions", file: "app_route_questions", rulebookRows: 151, fields: appRouteQuestionsFieldTypes,
    compute: (row: any) => computeAppRouteQuestions(row as AppRouteQuestionsRow),
    lookups: [],
    aggregations: [] },
  { name: "AppRouteReferences", file: "app_route_references", rulebookRows: 315, fields: appRouteReferencesFieldTypes,
    compute: (row: any) => computeAppRouteReferences(row as AppRouteReferencesRow),
    lookups: [],
    aggregations: [] },
  { name: "RulebookTables", file: "rulebook_tables", rulebookRows: 86, fields: rulebookTablesFieldTypes,
    compute: (row: any) => computeRulebookTables(row as RulebookTablesRow),
    lookups: [],
    aggregations: [
      { field: "field_count", op: "COUNTIFS", table: "rulebook_fields", criteria: [{ range: "target_table", kind: "field", field: "rulebook_table_id" }] },
      { field: "policy_count", op: "COUNTIFS", table: "access_policies", criteria: [{ range: "target_table", kind: "field", field: "rulebook_table_id" }] },] },
  { name: "AccessPrincipals", file: "access_principals", rulebookRows: 12, fields: accessPrincipalsFieldTypes,
    compute: (row: any) => computeAccessPrincipals(row as AccessPrincipalsRow),
    lookups: [
      { field: "organization_scope", target: "roles", ret: "organization", key: "domain_role", match: "role_id" },
      { field: "role_label", target: "roles", ret: "label", key: "domain_role", match: "role_id" },],
    aggregations: [
      { field: "policy_count", op: "COUNTIFS", table: "access_policies", criteria: [{ range: "principal", kind: "field", field: "access_principal_id" }] },
      { field: "grant_count", op: "COUNTIFS", table: "field_grants", criteria: [{ range: "principal", kind: "field", field: "access_principal_id" }] },
      { field: "visible_table_count", op: "COUNTIFS", table: "role_schema_views", criteria: [{ range: "principal", kind: "field", field: "access_principal_id" }] },] },
  { name: "AccessPolicies", file: "access_policies", rulebookRows: 202, fields: accessPoliciesFieldTypes,
    compute: (row: any) => computeAccessPolicies(row as AccessPoliciesRow),
    lookups: [
      { field: "principal_is_admin", target: "access_principals", ret: "is_administrator", key: "principal", match: "access_principal_id" },],
    aggregations: [
      { field: "denial_test_count", op: "COUNTIFS", table: "access_denial_tests", criteria: [{ range: "target_policy", kind: "field", field: "access_policy_id" }] },] },
  { name: "FieldGrants", file: "field_grants", rulebookRows: 3639, fields: fieldGrantsFieldTypes,
    compute: (row: any) => computeFieldGrants(row as FieldGrantsRow),
    lookups: [
      { field: "field_table", target: "rulebook_fields", ret: "target_table", key: "target_field", match: "rulebook_field_id" },
      { field: "field_name", target: "rulebook_fields", ret: "field_name", key: "target_field", match: "rulebook_field_id" },
      { field: "field_is_derived", target: "rulebook_fields", ret: "is_derived", key: "target_field", match: "rulebook_field_id" },],
    aggregations: [] },
  { name: "RoleSchemas", file: "role_schemas", rulebookRows: 12, fields: roleSchemasFieldTypes,
    compute: (row: any) => computeRoleSchemas(row as RoleSchemasRow),
    lookups: [],
    aggregations: [
      { field: "view_count", op: "COUNTIFS", table: "role_schema_views", criteria: [{ range: "role_schema", kind: "field", field: "role_schema_id" }] },] },
  { name: "RoleSchemaViews", file: "role_schema_views", rulebookRows: 202, fields: roleSchemaViewsFieldTypes,
    compute: (row: any) => computeRoleSchemaViews(row as RoleSchemaViewsRow),
    lookups: [
      { field: "schema_name", target: "role_schemas", ret: "schema_name", key: "role_schema", match: "role_schema_id" },
      { field: "source_view", target: "rulebook_tables", ret: "physical_view", key: "target_table", match: "rulebook_table_id" },
      { field: "table_field_count", target: "rulebook_tables", ret: "field_count", key: "target_table", match: "rulebook_table_id" },],
    aggregations: [
      { field: "column_count", op: "COUNTIFS", table: "field_grants", criteria: [{ range: "grant_key_when_readable", kind: "field", field: "grant_key" }] },] },
  { name: "JwtClaimMappings", file: "jwt_claim_mappings", rulebookRows: 4, fields: jwtClaimMappingsFieldTypes,
    compute: (row: any) => computeJwtClaimMappings(row as JwtClaimMappingsRow),
    lookups: [],
    aggregations: [
      { field: "usage_count", op: "COUNTIFS", table: "access_policies", criteria: [{ range: "row_predicate", kind: "field", field: "sql_accessor" }] },] },
  { name: "AccessDenialTests", file: "access_denial_tests", rulebookRows: 17, fields: accessDenialTestsFieldTypes,
    compute: (row: any) => computeAccessDenialTests(row as AccessDenialTestsRow),
    lookups: [],
    aggregations: [] },
  { name: "AppUsers", file: "app_users", rulebookRows: 10, fields: appUsersFieldTypes,
    compute: (row: any) => computeAppUsers(row as AppUsersRow),
    lookups: [
      { field: "agent_kind", target: "agents", ret: "agent_kind", key: "linked_agent", match: "agent_id" },
      { field: "organization", target: "agents", ret: "organization", key: "linked_agent", match: "agent_id" },],
    aggregations: [
      { field: "assignment_count", op: "COUNTIFS", table: "principal_assignments", criteria: [{ range: "app_user", kind: "field", field: "app_user_id" }] },] },
  { name: "PrincipalAssignments", file: "principal_assignments", rulebookRows: 12, fields: principalAssignmentsFieldTypes,
    compute: (row: any) => computePrincipalAssignments(row as PrincipalAssignmentsRow),
    lookups: [
      { field: "principal_is_admin", target: "access_principals", ret: "is_administrator", key: "principal", match: "access_principal_id" },
      { field: "user_organization", target: "app_users", ret: "organization", key: "app_user", match: "app_user_id" },
      { field: "principal_organization", target: "access_principals", ret: "organization_scope", key: "principal", match: "access_principal_id" },],
    aggregations: [] },
  { name: "IssuedTokens", file: "issued_tokens", rulebookRows: 0, fields: issuedTokensFieldTypes,
    compute: (row: any) => computeIssuedTokens(row as IssuedTokensRow),
    lookups: [],
    aggregations: [] },
  { name: "ProcessMiningRuns", file: "process_mining_runs", rulebookRows: 4, fields: processMiningRunsFieldTypes,
    compute: (row: any) => computeProcessMiningRuns(row as ProcessMiningRunsRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "procedure_version_is_live", target: "procedure_versions", ret: "is_live", key: "procedure_version", match: "procedure_version_id" },],
    aggregations: [] },
  { name: "Vocabularies", file: "vocabularies", rulebookRows: 2, fields: vocabulariesFieldTypes,
    compute: (row: any) => computeVocabularies(row as VocabulariesRow),
    lookups: [],
    aggregations: [
      { field: "term_count", op: "COUNTIFS", table: "vocabulary_terms", criteria: [{ range: "vocabulary", kind: "field", field: "vocabulary_id" }] },
      { field: "orphan_term_count", op: "COUNTIFS", table: "vocabulary_terms", criteria: [{ range: "orphan_term_vocabulary_key", kind: "field", field: "vocabulary_id" }] },] },
  { name: "VocabularyTerms", file: "vocabulary_terms", rulebookRows: 12, fields: vocabularyTermsFieldTypes,
    compute: (row: any) => computeVocabularyTerms(row as VocabularyTermsRow),
    lookups: [],
    aggregations: [
      { field: "usage_count", op: "COUNTIFS", table: "requirements", criteria: [{ range: "controlled_term", kind: "field", field: "vocabulary_term_id" }] },] },
  { name: "KnowledgeBrokerLinks", file: "knowledge_broker_links", rulebookRows: 5, fields: knowledgeBrokerLinksFieldTypes,
    compute: (row: any) => computeKnowledgeBrokerLinks(row as KnowledgeBrokerLinksRow),
    lookups: [
      { field: "as_of_instant", target: "evaluation_contexts", ret: "as_of_instant", key: "evaluation_context", match: "evaluation_context_id" },
      { field: "broker_is_still_engaged", target: "agents", ret: "is_still_engaged", key: "broker", match: "agent_id" },],
    aggregations: [] },
];

/** Materializes each vw_<entity>_closure view aggregations read. */
export const erbClosures: ClosureSpec[] = [
];
