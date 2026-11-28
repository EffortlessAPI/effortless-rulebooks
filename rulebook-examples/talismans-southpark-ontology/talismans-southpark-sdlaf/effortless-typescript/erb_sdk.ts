// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/south-park-ontology-in-sdlaf-rulebook.json
//
// One interface per table, a calc<Table><Field>() function per calculated field,
// the table registry main.ts runs, and evaluate(rows) for an app that computes
// in-process. Formulas compute through erb_runtime.ts; nothing here parses a
// formula, reads the rulebook, or calls a database.

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
  erbDatetimeText,
  erbText,
  erbConcat,
  erbConfigure,
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
  erbSwitch,
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
  erbTextDate,
  erbTextNumber,
  erbDatetimeDiff,
  erbNow,
  calcGuard,
  loadRows,
  erbEvaluate,
} from "./erb_runtime.js";
import type { ClosureSpec, FieldType, TableSpec } from "./erb_runtime.js";

/** The ERB build parameters this SDK was generated under
 *  (docs/ERB-BUILD-PARAMETERS.md). The runtime refuses to run unconfigured. */
export const erbBuildParameters: Record<string, string> = { "erbDateDiff": "calendar", "erbTimezone": "UTC", "erbDateTimeText": "iso8601", "erbBlankLogic": "coerce", "erbWholeNumber": "by-field-type" };
erbConfigure(erbBuildParameters);

// =============================================================================
// __META__ TABLE
// Project-level metadata. Transpiler-ignored name/value rows.
// =============================================================================

/** A row in the __meta__ table. */
export interface __meta__Row {
  /** The metadata key. Unique within the table. */
  meta_key: string;
  /** Order 1. Mirrors MetaKey so the row is addressable by Name. */
  name: string | null;
  /** How to read the value: 'string' uses StringValue, 'object' or 'array' parse JsonValue. */
  value_type: string;
  /** The value when ValueType is 'string'. */
  string_value: string | null;
  /** The value as JSON when ValueType is 'object' or 'array'. */
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
 *  Order 1. Mirrors MetaKey so the row is addressable by Name.
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
// REASONING TABLE
// The one world the rules reason over, and the switch of each of her five rules. Every table links here, so each rule's switch is one lookup away from the field it gates, and every count of the world rolls up here.
// =============================================================================

/** A row in the Reasoning table. */
export interface ReasoningRow {
  /** Key of the world. */
  reasoning_id: string;
  /** Order 1. The world's name. */
  name: string | null;
  /** The world's name. */
  label: string;
  /** Inverse rule: a parent link also gives a child link between the same two people (isChildOf, the inverse of isParentOf). */
  inverse_is_on: boolean;
  /** Symmetric rule: a spouse link or a friend link holds for both people. */
  symmetric_is_on: boolean;
  /** Property chain: a parent of a parent is a grandparent (isParentOf then isParentOf gives isGrandparentOf). */
  chain_is_on: boolean;
  /** Domain and range: whoever attends something, or has a grade level or a role, is a Person; whatever is attended is a School. */
  domain_range_is_on: boolean;
  /** Disjoint: no one is both the parent and the spouse of the same person (isParentOf is disjoint with isSpouseOf). */
  disjoint_is_on: boolean;
  /** Order 1. Every individual of the world. */
  count_of_individuals: number | null;
  /** Order 3. Individuals that are a Person, asserted or concluded. */
  count_of_people: number | null;
  /** Order 3. Individuals that are a School, asserted or concluded. */
  count_of_schools: number | null;
  /** Order 1. Stated isParentOf facts: one per Parenthoods row. */
  count_of_stated_parenthoods: number | null;
  /** Order 1. Stated isSpouseOf facts: one per Marriages row. */
  count_of_stated_marriages: number | null;
  /** Order 1. Stated isFriendOf facts: one per Friendships row. */
  count_of_stated_friendships: number | null;
  /** Order 3. Stated facts held as columns of an individual: attends, has age and is a. */
  count_of_stated_individual_facts: number | null;
  /** Order 3. Grade level facts in effect: each person's current grade. */
  count_of_current_grade_levels: number | null;
  /** Order 3. Role facts in effect: each person's current role. */
  count_of_current_roles: number | null;
  /** Order 4. Every stated fact in effect, wherever it is held. */
  count_of_stated_facts: number | null;
  /** Order 2. isChildOf facts the inverse rule concludes: one per parenthood. */
  count_of_child_of_conclusions: number | null;
  /** Order 2. isSpouseOf facts the symmetric rule concludes: one per marriage, read from its second partner. */
  count_of_mirrored_marriages: number | null;
  /** Order 2. isFriendOf facts the symmetric rule concludes: one per friendship, read from its second friend. */
  count_of_mirrored_friendships: number | null;
  /** Order 4. isGrandparentOf facts the property chain concludes. */
  count_of_grandparent_conclusions: number | null;
  /** Order 3. Individuals the domain rule makes a Person that no one asserted was one. */
  count_of_person_conclusions: number | null;
  /** Order 3. Individuals the range rule makes a School that no one asserted was one. */
  count_of_school_conclusions: number | null;
  /** Order 5. Every fact the rules conclude that no one stated. */
  count_of_conclusions: number | null;
  /** Order 4. Parenthoods the disjoint rule flags: the parent is also the child's spouse. */
  count_of_contradictions: number | null;
  /** Order 5. No contradiction the disjoint rule can see. */
  is_consistent: boolean | null;
  /** Order 6. The world in one line. */
  summary: string | null;
  _erb_errors?: Record<string, string>;
}

const reasoningFieldTypes: Record<string, FieldType> = {
  reasoning_id: "string",
  name: "*string",
  label: "string",
  inverse_is_on: "bool",
  symmetric_is_on: "bool",
  chain_is_on: "bool",
  domain_range_is_on: "bool",
  disjoint_is_on: "bool",
  count_of_individuals: "*int",
  count_of_people: "*int",
  count_of_schools: "*int",
  count_of_stated_parenthoods: "*int",
  count_of_stated_marriages: "*int",
  count_of_stated_friendships: "*int",
  count_of_stated_individual_facts: "*int",
  count_of_current_grade_levels: "*int",
  count_of_current_roles: "*int",
  count_of_stated_facts: "*int",
  count_of_child_of_conclusions: "*int",
  count_of_mirrored_marriages: "*int",
  count_of_mirrored_friendships: "*int",
  count_of_grandparent_conclusions: "*int",
  count_of_person_conclusions: "*int",
  count_of_school_conclusions: "*int",
  count_of_conclusions: "*int",
  count_of_contradictions: "*int",
  is_consistent: "*bool",
  summary: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The world's name.
 *  Formula: ={{Label}} */
export function calcReasoningName(tc: ReasoningRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes the CountOfStatedFacts calculated field.
 *  Order 4. Every stated fact in effect, wherever it is held.
 *  Formula: ={{CountOfStatedParenthoods}} + {{CountOfStatedMarriages}} + {{CountOfStatedFriendships}} + {{CountOfStatedIndividualFacts}} + {{CountOfCurrentGradeLevels}} + {{CountOfCurrentRoles}} */
export function calcReasoningCountOfStatedFacts(tc: ReasoningRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbAdd(erbAdd(erbAdd(erbAdd(vInt(tc.count_of_stated_parenthoods), vInt(tc.count_of_stated_marriages)), vInt(tc.count_of_stated_friendships)), vInt(tc.count_of_stated_individual_facts)), vInt(tc.count_of_current_grade_levels)), vInt(tc.count_of_current_roles))));
}

/** Computes the CountOfConclusions calculated field.
 *  Order 5. Every fact the rules conclude that no one stated.
 *  Formula: ={{CountOfChildOfConclusions}} + {{CountOfMirroredMarriages}} + {{CountOfMirroredFriendships}} + {{CountOfGrandparentConclusions}} + {{CountOfPersonConclusions}} + {{CountOfSchoolConclusions}} */
export function calcReasoningCountOfConclusions(tc: ReasoningRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbAdd(erbAdd(erbAdd(erbAdd(vInt(tc.count_of_child_of_conclusions), vInt(tc.count_of_mirrored_marriages)), vInt(tc.count_of_mirrored_friendships)), vInt(tc.count_of_grandparent_conclusions)), vInt(tc.count_of_person_conclusions)), vInt(tc.count_of_school_conclusions))));
}

/** Computes the IsConsistent calculated field.
 *  Order 5. No contradiction the disjoint rule can see.
 *  Formula: ={{CountOfContradictions}} = 0 */
export function calcReasoningIsConsistent(tc: ReasoningRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.count_of_contradictions), vI(0)));
}

/** Computes the Summary calculated field.
 *  Order 6. The world in one line.
 *  Formula: =CONCAT({{CountOfStatedFacts}}, " stated facts, ", {{CountOfConclusions}}, " conclusions, ", {{CountOfContradictions}}, " contradictions") */
export function calcReasoningSummary(tc: ReasoningRow): string | null {
  return toStringPtr(erbConcat(erbText(vInt(tc.count_of_stated_facts)), vS(" stated facts, "), erbText(vInt(tc.count_of_conclusions)), vS(" conclusions, "), erbText(vInt(tc.count_of_contradictions)), vS(" contradictions")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeReasoning(tc: ReasoningRow): ReasoningRow {
  // Level 1
  calcGuard(tc, reasoningFieldTypes, "name", () => { tc.name = calcReasoningName(tc); });
  calcGuard(tc, reasoningFieldTypes, "count_of_stated_facts", () => { tc.count_of_stated_facts = calcReasoningCountOfStatedFacts(tc); });
  calcGuard(tc, reasoningFieldTypes, "count_of_conclusions", () => { tc.count_of_conclusions = calcReasoningCountOfConclusions(tc); });
  calcGuard(tc, reasoningFieldTypes, "is_consistent", () => { tc.is_consistent = calcReasoningIsConsistent(tc); });
  // Level 2
  calcGuard(tc, reasoningFieldTypes, "summary", () => { tc.summary = calcReasoningSummary(tc); });
  return tc;
}

/** Reads Reasoning rows from a JSON array file. */
export function loadReasoningRows(file: string): ReasoningRow[] {
  return loadRows(file, { fields: reasoningFieldTypes }) as unknown as ReasoningRow[];
}

// =============================================================================
// PORTRAITS TABLE
// The picture of each individual, linked from its Fandom page (never downloaded).
// =============================================================================

/** A row in the Portraits table. */
export interface PortraitsRow {
  /** Key of the portrait: the short name the page files it under. */
  portrait_id: string;
  /** The name on the portrait's Fandom page. */
  display_name: string;
  /** The Fandom page credited for the portrait. */
  page: string;
  /** The original Fandom image. */
  image: string;
  /** The crop as an SVG viewBox: x y width height. */
  crop: string;
  /** The original image's width in pixels. */
  width: number;
  /** The original image's height in pixels. */
  height: number;
  /** Position in the credits. */
  sort_order: number;
  /** Order 1. The portrait's name. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const portraitsFieldTypes: Record<string, FieldType> = {
  portrait_id: "string",
  display_name: "string",
  page: "string",
  image: "string",
  crop: "string",
  width: "int",
  height: "int",
  sort_order: "int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The portrait's name.
 *  Formula: ={{DisplayName}} */
export function calcPortraitsName(tc: PortraitsRow): string | null {
  return toStringPtr(vStrPlain(tc.display_name));
}

/** Computes every calculated field of the row in dependency order. */
export function computePortraits(tc: PortraitsRow): PortraitsRow {
  // Level 1
  calcGuard(tc, portraitsFieldTypes, "name", () => { tc.name = calcPortraitsName(tc); });
  return tc;
}

/** Reads Portraits rows from a JSON array file. */
export function loadPortraitsRows(file: string): PortraitsRow[] {
  return loadRows(file, { fields: portraitsFieldTypes }) as unknown as PortraitsRow[];
}

// =============================================================================
// INDIVIDUALS TABLE
// Every individual of the world (owl:Thing). What is said about one individual alone is a column of its row: its age, the place it attends, whether it is asserted to be a Person or a School. Its current grade level and role come from their records. Class membership is a formula, not a table, because it is concluded: no one stated that South Park Elementary is a School.
// =============================================================================

/** A row in the Individuals table. */
export interface IndividualsRow {
  /** Key of the individual. */
  individual_id: string;
  /** Order 1. The individual's name. */
  name: string | null;
  /** The individual's name. */
  label: string;
  /** Position in the cast. */
  sort_order: number;
  /** The world this individual belongs to. */
  reasoning: string;
  /** The individual's picture. */
  portrait: string | null;
  /** Order 1. The picture's image. */
  portrait_image: string | null;
  /** Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to. */
  symmetric_is_on: boolean | null;
  /** Order 1. The DomainRange rule's switch, read from the reasoning this row belongs to. */
  domain_range_is_on: boolean | null;
  /** Stated: this individual is a Person (isA Person). */
  is_asserted_person: boolean;
  /** Stated: this individual is a School (isA School). */
  is_asserted_school: boolean;
  /** Stated: hasAge. */
  age: number | null;
  /** Stated: the place this individual attends (attends). Its range is School. */
  attends: string | null;
  /** Order 1. The name of the place attended. */
  attends_label: string | null;
  /** Order 1. An age is stated. */
  has_age: boolean | null;
  /** Order 1. A place attended is stated. */
  has_attends: boolean | null;
  /** Order 2. Stated facts held on this row: is a Person, is a School, has age, attends. */
  stated_fact_count: number | null;
  /** Order 1. Grade level records switched on for this individual. */
  count_of_grade_levels_on: number | null;
  /** Order 1. When the latest grade level switched on was stated: the one in effect. */
  latest_grade_level_seq: number | null;
  /** Order 2. A grade level is in effect. */
  has_grade_level: boolean | null;
  /** Order 3. Key of the grade level record in effect. */
  current_grade_level_record: string | null;
  /** Order 4. The grade level in effect: the latest one switched on (hasGradeLevel keeps one current value). */
  grade_level: number | null;
  /** Order 1. Role records switched on for this individual. */
  count_of_roles_on: number | null;
  /** Order 1. When the latest role switched on was stated: the one in effect. */
  latest_role_seq: number | null;
  /** Order 2. A role is in effect. */
  has_role: boolean | null;
  /** Order 3. Key of the role record in effect. */
  current_role_record: string | null;
  /** Order 4. The role in effect: the latest one switched on (hasRole keeps one current value). */
  role: string | null;
  /** Order 3. Something is stated that only a Person can have: a place attended, a grade level or a role (the domain of attends, hasGradeLevel and hasRole). */
  has_person_evidence: boolean | null;
  /** Order 4. The domain rule makes this individual a Person. */
  is_person_by_domain: boolean | null;
  /** Order 5. A Person, asserted or concluded. Open world: true, or not known. */
  is_person: boolean | null;
  /** Order 5. A Person only because the domain rule says so. */
  is_person_concluded: boolean | null;
  /** Order 1. Individuals that attend this one. */
  attended_by_count: number | null;
  /** Order 2. The range rule makes this individual a School: someone attends it. */
  is_school_by_range: boolean | null;
  /** Order 3. A School, asserted or concluded. Open world: true, or not known. */
  is_school: boolean | null;
  /** Order 3. A School only because the range rule says so. */
  is_school_concluded: boolean | null;
  /** Order 1. Children: parenthoods where this individual is the parent (isParentOf). */
  child_count: number | null;
  /** Order 1. Parents: the same parenthoods read from the child's end. The link is there whatever the inverse rule says. */
  parent_count: number | null;
  /** Order 2. isChildOf facts about this individual: its parenthoods read from its end, when the inverse rule is on. */
  child_of_fact_count: number | null;
  /** Order 5. Grandchildren the property chain concludes (isGrandparentOf). */
  grandchild_count: number | null;
  /** Order 5. Grandparents the property chain concludes, read from the grandchild's end. */
  grandparent_count: number | null;
  /** Order 1. Everyone below this individual through one or more parenthoods: the transitive closure, which her page does not draw. */
  descendant_count: number | null;
  /** Order 1. Everyone above this individual through one or more parenthoods. */
  ancestor_count: number | null;
  /** Order 2. Marriages that name this individual first: the stated isSpouseOf. */
  stated_spouse_count: number | null;
  /** Order 3. Marriages that name this individual second, when the symmetric rule reads them both ways. */
  mirrored_spouse_count: number | null;
  /** Order 4. Spouses: isSpouseOf facts about this individual, stated or mirrored. */
  spouse_count: number | null;
  /** Order 2. Friendships that name this individual first: the stated isFriendOf. */
  stated_friend_count: number | null;
  /** Order 3. Friendships that name this individual second, when the symmetric rule reads them both ways. */
  mirrored_friend_count: number | null;
  /** Order 4. Friends: isFriendOf facts about this individual, stated or mirrored. */
  friend_count: number | null;
  _erb_errors?: Record<string, string>;
}

const individualsFieldTypes: Record<string, FieldType> = {
  individual_id: "string",
  name: "*string",
  label: "string",
  sort_order: "int",
  reasoning: "string",
  portrait: "*string",
  portrait_image: "*string",
  symmetric_is_on: "*bool",
  domain_range_is_on: "*bool",
  is_asserted_person: "bool",
  is_asserted_school: "bool",
  age: "*int",
  attends: "*string",
  attends_label: "*string",
  has_age: "*bool",
  has_attends: "*bool",
  stated_fact_count: "*int",
  count_of_grade_levels_on: "*int",
  latest_grade_level_seq: "*int",
  has_grade_level: "*bool",
  current_grade_level_record: "*string",
  grade_level: "*int",
  count_of_roles_on: "*int",
  latest_role_seq: "*int",
  has_role: "*bool",
  current_role_record: "*string",
  role: "*string",
  has_person_evidence: "*bool",
  is_person_by_domain: "*bool",
  is_person: "*bool",
  is_person_concluded: "*bool",
  attended_by_count: "*int",
  is_school_by_range: "*bool",
  is_school: "*bool",
  is_school_concluded: "*bool",
  child_count: "*int",
  parent_count: "*int",
  child_of_fact_count: "*int",
  grandchild_count: "*int",
  grandparent_count: "*int",
  descendant_count: "*int",
  ancestor_count: "*int",
  stated_spouse_count: "*int",
  mirrored_spouse_count: "*int",
  spouse_count: "*int",
  stated_friend_count: "*int",
  mirrored_friend_count: "*int",
  friend_count: "*int",
};

/** Computes the Name calculated field.
 *  Order 1. The individual's name.
 *  Formula: ={{Label}} */
export function calcIndividualsName(tc: IndividualsRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes the HasAge calculated field.
 *  Order 1. An age is stated.
 *  Formula: ={{Age}} <> "" */
export function calcIndividualsHasAge(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vInt(tc.age)));
}

/** Computes the HasAttends calculated field.
 *  Order 1. A place attended is stated.
 *  Formula: ={{Attends}} <> "" */
export function calcIndividualsHasAttends(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbIsNotBlank(vStr(tc.attends)));
}

/** Computes the StatedFactCount calculated field.
 *  Order 2. Stated facts held on this row: is a Person, is a School, has age, attends.
 *  Formula: =IF({{IsAssertedPerson}}, 1, 0) + IF({{IsAssertedSchool}}, 1, 0) + IF({{HasAge}}, 1, 0) + IF({{HasAttends}}, 1, 0) */
export function calcIndividualsStatedFactCount(tc: IndividualsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbAdd(erbAdd(erbIf(erbIsTrue(vBoolPlain(tc.is_asserted_person)), () => vI(1), () => vI(0)), erbIf(erbIsTrue(vBoolPlain(tc.is_asserted_school)), () => vI(1), () => vI(0))), erbIf(erbBool3(vBool(tc.has_age)), () => vI(1), () => vI(0))), erbIf(erbBool3(vBool(tc.has_attends)), () => vI(1), () => vI(0)))));
}

/** Computes the HasGradeLevel calculated field.
 *  Order 2. A grade level is in effect.
 *  Formula: ={{CountOfGradeLevelsOn}} > 0 */
export function calcIndividualsHasGradeLevel(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_grade_levels_on), ">", vI(0)));
}

/** Computes the CurrentGradeLevelRecord calculated field.
 *  Order 3. Key of the grade level record in effect.
 *  Formula: =IF({{HasGradeLevel}}, CONCAT({{IndividualId}}, "|grade|", {{LatestGradeLevelSeq}}), "") */
export function calcIndividualsCurrentGradeLevelRecord(tc: IndividualsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.has_grade_level)), () => erbConcat(erbText(vStrPlain(tc.individual_id)), vS("|grade|"), erbText(vInt(tc.latest_grade_level_seq))), () => vS("")));
}

/** Computes the HasRole calculated field.
 *  Order 2. A role is in effect.
 *  Formula: ={{CountOfRolesOn}} > 0 */
export function calcIndividualsHasRole(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_roles_on), ">", vI(0)));
}

/** Computes the CurrentRoleRecord calculated field.
 *  Order 3. Key of the role record in effect.
 *  Formula: =IF({{HasRole}}, CONCAT({{IndividualId}}, "|role|", {{LatestRoleSeq}}), "") */
export function calcIndividualsCurrentRoleRecord(tc: IndividualsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.has_role)), () => erbConcat(erbText(vStrPlain(tc.individual_id)), vS("|role|"), erbText(vInt(tc.latest_role_seq))), () => vS("")));
}

/** Computes the HasPersonEvidence calculated field.
 *  Order 3. Something is stated that only a Person can have: a place attended, a grade level or a role (the domain of attends, hasGradeLevel and hasRole).
 *  Formula: =OR({{HasAttends}}, {{HasGradeLevel}}, {{HasRole}}) */
export function calcIndividualsHasPersonEvidence(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(vBool(tc.has_attends)), erbBool3(vBool(tc.has_grade_level)), erbBool3(vBool(tc.has_role))));
}

/** Computes the IsPersonByDomain calculated field.
 *  Order 4. The domain rule makes this individual a Person.
 *  Formula: =AND({{DomainRangeIsOn}}, {{HasPersonEvidence}}) */
export function calcIndividualsIsPersonByDomain(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.domain_range_is_on)), erbBool3(vBool(tc.has_person_evidence))));
}

/** Computes the IsPerson calculated field.
 *  Order 5. A Person, asserted or concluded. Open world: true, or not known.
 *  Formula: =IF(OR({{IsAssertedPerson}}, {{IsPersonByDomain}}), TRUE()) */
export function calcIndividualsIsPerson(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbOr(erbIsTrue(vBoolPlain(tc.is_asserted_person)), erbBool3(vBool(tc.is_person_by_domain)))), () => vB(true), () => Null));
}

/** Computes the IsPersonConcluded calculated field.
 *  Order 5. A Person only because the domain rule says so.
 *  Formula: =IF(AND({{IsPersonByDomain}}, NOT({{IsAssertedPerson}})), TRUE()) */
export function calcIndividualsIsPersonConcluded(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.is_person_by_domain)), erbBool3(erbNot(erbIsTrue(vBoolPlain(tc.is_asserted_person)))))), () => vB(true), () => Null));
}

/** Computes the IsSchoolByRange calculated field.
 *  Order 2. The range rule makes this individual a School: someone attends it.
 *  Formula: =AND({{DomainRangeIsOn}}, {{AttendedByCount}} > 0) */
export function calcIndividualsIsSchoolByRange(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(vBool(tc.domain_range_is_on)), erbBool3(erbCmp(vInt(tc.attended_by_count), ">", vI(0)))));
}

/** Computes the IsSchool calculated field.
 *  Order 3. A School, asserted or concluded. Open world: true, or not known.
 *  Formula: =IF(OR({{IsAssertedSchool}}, {{IsSchoolByRange}}), TRUE()) */
export function calcIndividualsIsSchool(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbOr(erbIsTrue(vBoolPlain(tc.is_asserted_school)), erbBool3(vBool(tc.is_school_by_range)))), () => vB(true), () => Null));
}

/** Computes the IsSchoolConcluded calculated field.
 *  Order 3. A School only because the range rule says so.
 *  Formula: =IF(AND({{IsSchoolByRange}}, NOT({{IsAssertedSchool}})), TRUE()) */
export function calcIndividualsIsSchoolConcluded(tc: IndividualsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.is_school_by_range)), erbBool3(erbNot(erbIsTrue(vBoolPlain(tc.is_asserted_school)))))), () => vB(true), () => Null));
}

/** Computes the SpouseCount calculated field.
 *  Order 4. Spouses: isSpouseOf facts about this individual, stated or mirrored.
 *  Formula: ={{StatedSpouseCount}} + {{MirroredSpouseCount}} */
export function calcIndividualsSpouseCount(tc: IndividualsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.stated_spouse_count), vInt(tc.mirrored_spouse_count))));
}

/** Computes the FriendCount calculated field.
 *  Order 4. Friends: isFriendOf facts about this individual, stated or mirrored.
 *  Formula: ={{StatedFriendCount}} + {{MirroredFriendCount}} */
export function calcIndividualsFriendCount(tc: IndividualsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.stated_friend_count), vInt(tc.mirrored_friend_count))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeIndividuals(tc: IndividualsRow): IndividualsRow {
  // Level 1
  calcGuard(tc, individualsFieldTypes, "name", () => { tc.name = calcIndividualsName(tc); });
  calcGuard(tc, individualsFieldTypes, "has_age", () => { tc.has_age = calcIndividualsHasAge(tc); });
  calcGuard(tc, individualsFieldTypes, "has_attends", () => { tc.has_attends = calcIndividualsHasAttends(tc); });
  calcGuard(tc, individualsFieldTypes, "has_grade_level", () => { tc.has_grade_level = calcIndividualsHasGradeLevel(tc); });
  calcGuard(tc, individualsFieldTypes, "has_role", () => { tc.has_role = calcIndividualsHasRole(tc); });
  calcGuard(tc, individualsFieldTypes, "is_school_by_range", () => { tc.is_school_by_range = calcIndividualsIsSchoolByRange(tc); });
  calcGuard(tc, individualsFieldTypes, "spouse_count", () => { tc.spouse_count = calcIndividualsSpouseCount(tc); });
  calcGuard(tc, individualsFieldTypes, "friend_count", () => { tc.friend_count = calcIndividualsFriendCount(tc); });
  // Level 2
  calcGuard(tc, individualsFieldTypes, "stated_fact_count", () => { tc.stated_fact_count = calcIndividualsStatedFactCount(tc); });
  calcGuard(tc, individualsFieldTypes, "current_grade_level_record", () => { tc.current_grade_level_record = calcIndividualsCurrentGradeLevelRecord(tc); });
  calcGuard(tc, individualsFieldTypes, "current_role_record", () => { tc.current_role_record = calcIndividualsCurrentRoleRecord(tc); });
  calcGuard(tc, individualsFieldTypes, "has_person_evidence", () => { tc.has_person_evidence = calcIndividualsHasPersonEvidence(tc); });
  calcGuard(tc, individualsFieldTypes, "is_school", () => { tc.is_school = calcIndividualsIsSchool(tc); });
  calcGuard(tc, individualsFieldTypes, "is_school_concluded", () => { tc.is_school_concluded = calcIndividualsIsSchoolConcluded(tc); });
  // Level 3
  calcGuard(tc, individualsFieldTypes, "is_person_by_domain", () => { tc.is_person_by_domain = calcIndividualsIsPersonByDomain(tc); });
  // Level 4
  calcGuard(tc, individualsFieldTypes, "is_person", () => { tc.is_person = calcIndividualsIsPerson(tc); });
  calcGuard(tc, individualsFieldTypes, "is_person_concluded", () => { tc.is_person_concluded = calcIndividualsIsPersonConcluded(tc); });
  return tc;
}

/** Reads Individuals rows from a JSON array file. */
export function loadIndividualsRows(file: string): IndividualsRow[] {
  return loadRows(file, { fields: individualsFieldTypes }) as unknown as IndividualsRow[];
}

// =============================================================================
// PARENTHOODS TABLE
// One row per stated isParentOf fact. A person has many children and a child more than one parent, so the link is its own entity with two links to Individuals. The inverse rule's isChildOf is not stored: it is this row read from the child's end.
// =============================================================================

/** A row in the Parenthoods table. */
export interface ParenthoodsRow {
  /** Key of the fact: subject|property|object, as her page names it. */
  parenthood_id: string;
  /** Order 2. The stated fact in words. */
  name: string | null;
  /** The world this fact belongs to. */
  reasoning: string;
  /** The parent (the subject of isParentOf). */
  parent: string;
  /** The child (the object of isParentOf). */
  child: string;
  /** Order 1. The parent's name. */
  parent_label: string | null;
  /** Order 1. The child's name. */
  child_label: string | null;
  /** Order 1. The Inverse rule's switch, read from the reasoning this row belongs to. */
  inverse_is_on: boolean | null;
  /** Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to. */
  symmetric_is_on: boolean | null;
  /** Order 1. The Disjoint rule's switch, read from the reasoning this row belongs to. */
  disjoint_is_on: boolean | null;
  /** Order 2. The inverse rule concludes the child is child of the parent. Open world: true, or not known. */
  child_of_holds: boolean | null;
  /** Order 2. The inverse fact in words. */
  child_of_sentence: string | null;
  /** Order 3. Marriages stated with the parent first and the child second. */
  spouse_as_stated_count: number | null;
  /** Order 3. Marriages stated with the child first, which make the parent the child's spouse when the symmetric rule mirrors them. */
  spouse_as_mirrored_count: number | null;
  /** Order 4. The disjoint rule: the parent is also the child's spouse. Open world: true, or not known. */
  is_contradiction: boolean | null;
  /** Transitive closure of isParentOf (an owl:TransitiveProperty reading of it, which her page does not draw): every (ancestor, descendant) pair through one or more parenthoods, materialized as vw_parenthoods_closure. */
  ancestry_closure: unknown;
  _erb_errors?: Record<string, string>;
}

const parenthoodsFieldTypes: Record<string, FieldType> = {
  parenthood_id: "string",
  name: "*string",
  reasoning: "string",
  parent: "string",
  child: "string",
  parent_label: "*string",
  child_label: "*string",
  inverse_is_on: "*bool",
  symmetric_is_on: "*bool",
  disjoint_is_on: "*bool",
  child_of_holds: "*bool",
  child_of_sentence: "*string",
  spouse_as_stated_count: "*int",
  spouse_as_mirrored_count: "*int",
  is_contradiction: "*bool",
  ancestry_closure: "any",
};

/** Computes the Name calculated field.
 *  Order 2. The stated fact in words.
 *  Formula: =CONCAT({{ParentLabel}}, " is parent of ", {{ChildLabel}}) */
export function calcParenthoodsName(tc: ParenthoodsRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.parent_label)), vS(" is parent of "), erbText(vStr(tc.child_label))));
}

/** Computes the ChildOfHolds calculated field.
 *  Order 2. The inverse rule concludes the child is child of the parent. Open world: true, or not known.
 *  Formula: =IF({{InverseIsOn}}, TRUE()) */
export function calcParenthoodsChildOfHolds(tc: ParenthoodsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(vBool(tc.inverse_is_on)), () => vB(true), () => Null));
}

/** Computes the ChildOfSentence calculated field.
 *  Order 2. The inverse fact in words.
 *  Formula: =CONCAT({{ChildLabel}}, " is child of ", {{ParentLabel}}) */
export function calcParenthoodsChildOfSentence(tc: ParenthoodsRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.child_label)), vS(" is child of "), erbText(vStr(tc.parent_label))));
}

/** Computes the IsContradiction calculated field.
 *  Order 4. The disjoint rule: the parent is also the child's spouse. Open world: true, or not known.
 *  Formula: =IF(AND({{DisjointIsOn}}, {{SpouseAsStatedCount}} + {{SpouseAsMirroredCount}} > 0), TRUE()) */
export function calcParenthoodsIsContradiction(tc: ParenthoodsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.disjoint_is_on)), erbBool3(erbCmp(erbAdd(vInt(tc.spouse_as_stated_count), vInt(tc.spouse_as_mirrored_count)), ">", vI(0))))), () => vB(true), () => Null));
}

/** Computes every calculated field of the row in dependency order. */
export function computeParenthoods(tc: ParenthoodsRow): ParenthoodsRow {
  // Level 1
  calcGuard(tc, parenthoodsFieldTypes, "name", () => { tc.name = calcParenthoodsName(tc); });
  calcGuard(tc, parenthoodsFieldTypes, "child_of_holds", () => { tc.child_of_holds = calcParenthoodsChildOfHolds(tc); });
  calcGuard(tc, parenthoodsFieldTypes, "child_of_sentence", () => { tc.child_of_sentence = calcParenthoodsChildOfSentence(tc); });
  calcGuard(tc, parenthoodsFieldTypes, "is_contradiction", () => { tc.is_contradiction = calcParenthoodsIsContradiction(tc); });
  return tc;
}

/** Reads Parenthoods rows from a JSON array file. */
export function loadParenthoodsRows(file: string): ParenthoodsRow[] {
  return loadRows(file, { fields: parenthoodsFieldTypes }) as unknown as ParenthoodsRow[];
}

// =============================================================================
// MARRIAGES TABLE
// One row per stated isSpouseOf fact. A marriage has no direction: the symmetric rule is the same row counted from its second partner's end.
// =============================================================================

/** A row in the Marriages table. */
export interface MarriagesRow {
  /** Key of the fact: subject|property|object, as her page names it. */
  marriage_id: string;
  /** Order 2. The stated fact in words. */
  name: string | null;
  /** The world this fact belongs to. */
  reasoning: string;
  /** The partner the stated fact names first. */
  partner_a: string;
  /** The partner the stated fact names second. */
  partner_b: string;
  /** Order 1. The partnera's name. */
  partner_a_label: string | null;
  /** Order 1. The partnerb's name. */
  partner_b_label: string | null;
  /** Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to. */
  symmetric_is_on: boolean | null;
  /** Order 2. The symmetric rule concludes the second partner is spouse of the first. Open world: true, or not known. */
  mirror_holds: boolean | null;
  /** Order 2. The mirrored fact in words. */
  mirror_sentence: string | null;
  _erb_errors?: Record<string, string>;
}

const marriagesFieldTypes: Record<string, FieldType> = {
  marriage_id: "string",
  name: "*string",
  reasoning: "string",
  partner_a: "string",
  partner_b: "string",
  partner_a_label: "*string",
  partner_b_label: "*string",
  symmetric_is_on: "*bool",
  mirror_holds: "*bool",
  mirror_sentence: "*string",
};

/** Computes the Name calculated field.
 *  Order 2. The stated fact in words.
 *  Formula: =CONCAT({{PartnerALabel}}, " is spouse of ", {{PartnerBLabel}}) */
export function calcMarriagesName(tc: MarriagesRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.partner_a_label)), vS(" is spouse of "), erbText(vStr(tc.partner_b_label))));
}

/** Computes the MirrorHolds calculated field.
 *  Order 2. The symmetric rule concludes the second partner is spouse of the first. Open world: true, or not known.
 *  Formula: =IF({{SymmetricIsOn}}, TRUE()) */
export function calcMarriagesMirrorHolds(tc: MarriagesRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(vBool(tc.symmetric_is_on)), () => vB(true), () => Null));
}

/** Computes the MirrorSentence calculated field.
 *  Order 2. The mirrored fact in words.
 *  Formula: =CONCAT({{PartnerBLabel}}, " is spouse of ", {{PartnerALabel}}) */
export function calcMarriagesMirrorSentence(tc: MarriagesRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.partner_b_label)), vS(" is spouse of "), erbText(vStr(tc.partner_a_label))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeMarriages(tc: MarriagesRow): MarriagesRow {
  // Level 1
  calcGuard(tc, marriagesFieldTypes, "name", () => { tc.name = calcMarriagesName(tc); });
  calcGuard(tc, marriagesFieldTypes, "mirror_holds", () => { tc.mirror_holds = calcMarriagesMirrorHolds(tc); });
  calcGuard(tc, marriagesFieldTypes, "mirror_sentence", () => { tc.mirror_sentence = calcMarriagesMirrorSentence(tc); });
  return tc;
}

/** Reads Marriages rows from a JSON array file. */
export function loadMarriagesRows(file: string): MarriagesRow[] {
  return loadRows(file, { fields: marriagesFieldTypes }) as unknown as MarriagesRow[];
}

// =============================================================================
// FRIENDSHIPS TABLE
// One row per stated isFriendOf fact, read both ways by the symmetric rule, like a marriage.
// =============================================================================

/** A row in the Friendships table. */
export interface FriendshipsRow {
  /** Key of the fact: subject|property|object, as her page names it. */
  friendship_id: string;
  /** Order 2. The stated fact in words. */
  name: string | null;
  /** The world this fact belongs to. */
  reasoning: string;
  /** The friend the stated fact names first. */
  friend_a: string;
  /** The friend the stated fact names second. */
  friend_b: string;
  /** Order 1. The frienda's name. */
  friend_a_label: string | null;
  /** Order 1. The friendb's name. */
  friend_b_label: string | null;
  /** Order 1. The Symmetric rule's switch, read from the reasoning this row belongs to. */
  symmetric_is_on: boolean | null;
  /** Order 2. The symmetric rule concludes the second friend is friend of the first. Open world: true, or not known. */
  mirror_holds: boolean | null;
  /** Order 2. The mirrored fact in words. */
  mirror_sentence: string | null;
  _erb_errors?: Record<string, string>;
}

const friendshipsFieldTypes: Record<string, FieldType> = {
  friendship_id: "string",
  name: "*string",
  reasoning: "string",
  friend_a: "string",
  friend_b: "string",
  friend_a_label: "*string",
  friend_b_label: "*string",
  symmetric_is_on: "*bool",
  mirror_holds: "*bool",
  mirror_sentence: "*string",
};

/** Computes the Name calculated field.
 *  Order 2. The stated fact in words.
 *  Formula: =CONCAT({{FriendALabel}}, " is friend of ", {{FriendBLabel}}) */
export function calcFriendshipsName(tc: FriendshipsRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.friend_a_label)), vS(" is friend of "), erbText(vStr(tc.friend_b_label))));
}

/** Computes the MirrorHolds calculated field.
 *  Order 2. The symmetric rule concludes the second friend is friend of the first. Open world: true, or not known.
 *  Formula: =IF({{SymmetricIsOn}}, TRUE()) */
export function calcFriendshipsMirrorHolds(tc: FriendshipsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(vBool(tc.symmetric_is_on)), () => vB(true), () => Null));
}

/** Computes the MirrorSentence calculated field.
 *  Order 2. The mirrored fact in words.
 *  Formula: =CONCAT({{FriendBLabel}}, " is friend of ", {{FriendALabel}}) */
export function calcFriendshipsMirrorSentence(tc: FriendshipsRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.friend_b_label)), vS(" is friend of "), erbText(vStr(tc.friend_a_label))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeFriendships(tc: FriendshipsRow): FriendshipsRow {
  // Level 1
  calcGuard(tc, friendshipsFieldTypes, "name", () => { tc.name = calcFriendshipsName(tc); });
  calcGuard(tc, friendshipsFieldTypes, "mirror_holds", () => { tc.mirror_holds = calcFriendshipsMirrorHolds(tc); });
  calcGuard(tc, friendshipsFieldTypes, "mirror_sentence", () => { tc.mirror_sentence = calcFriendshipsMirrorSentence(tc); });
  return tc;
}

/** Reads Friendships rows from a JSON array file. */
export function loadFriendshipsRows(file: string): FriendshipsRow[] {
  return loadRows(file, { fields: friendshipsFieldTypes }) as unknown as FriendshipsRow[];
}

// =============================================================================
// GRANDPARENTHOODS TABLE
// The property chain isParentOf then isParentOf, as rows: every ordered pair of parenthoods is a candidate, and it holds when the first one's child is the second one's parent. A formula cannot add a row, so the candidates are data (every pair, decided by nothing about the family) and whether one holds is a formula.
// =============================================================================

/** A row in the Grandparenthoods table. */
export interface GrandparenthoodsRow {
  /** Key: the upper parenthood, then the lower one. */
  grandparenthood_id: string;
  /** Order 4. The concluded fact in words, or why this pair concludes nothing. */
  name: string | null;
  /** The world this candidate belongs to. */
  reasoning: string;
  /** The first link: grandparent to the middle generation. */
  upper_link: string;
  /** The second link: the middle generation to the grandchild. */
  lower_link: string;
  /** Order 1. The upper link's parent. */
  grandparent: string | null;
  /** Order 1. The upper link's child. */
  upper_child: string | null;
  /** Order 1. The lower link's parent. */
  lower_parent: string | null;
  /** Order 1. The lower link's child. */
  grandchild: string | null;
  /** Order 2. The grandparent's name. */
  grandparent_label: string | null;
  /** Order 2. The grandchild's name. */
  grandchild_label: string | null;
  /** Order 2. The upper link's child is the lower link's parent: the two links make a chain. */
  links: boolean | null;
  /** Order 1. The Chain rule's switch, read from the reasoning this row belongs to. */
  chain_is_on: boolean | null;
  /** Order 3. The property chain concludes isGrandparentOf. Open world: true, or not known. */
  holds: boolean | null;
  _erb_errors?: Record<string, string>;
}

const grandparenthoodsFieldTypes: Record<string, FieldType> = {
  grandparenthood_id: "string",
  name: "*string",
  reasoning: "string",
  upper_link: "string",
  lower_link: "string",
  grandparent: "*string",
  upper_child: "*string",
  lower_parent: "*string",
  grandchild: "*string",
  grandparent_label: "*string",
  grandchild_label: "*string",
  links: "*bool",
  chain_is_on: "*bool",
  holds: "*bool",
};

/** Computes the Name calculated field.
 *  Order 4. The concluded fact in words, or why this pair concludes nothing.
 *  Formula: =IF({{Links}}, CONCAT({{GrandparentLabel}}, " is grandparent of ", {{GrandchildLabel}}), CONCAT("(", {{UpperLink}}, " does not lead to ", {{LowerLink}}, ")")) */
export function calcGrandparenthoodsName(tc: GrandparenthoodsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.links)), () => erbConcat(erbText(vStr(tc.grandparent_label)), vS(" is grandparent of "), erbText(vStr(tc.grandchild_label))), () => erbConcat(vS("("), erbText(vStrPlain(tc.upper_link)), vS(" does not lead to "), erbText(vStrPlain(tc.lower_link)), vS(")"))));
}

/** Computes the Links calculated field.
 *  Order 2. The upper link's child is the lower link's parent: the two links make a chain.
 *  Formula: ={{UpperChild}} = {{LowerParent}} */
export function calcGrandparenthoodsLinks(tc: GrandparenthoodsRow): boolean | null {
  return toBoolPtr(erbEq(vStr(tc.upper_child), vStr(tc.lower_parent)));
}

/** Computes the Holds calculated field.
 *  Order 3. The property chain concludes isGrandparentOf. Open world: true, or not known.
 *  Formula: =IF(AND({{Links}}, {{ChainIsOn}}), TRUE()) */
export function calcGrandparenthoodsHolds(tc: GrandparenthoodsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.links)), erbBool3(vBool(tc.chain_is_on)))), () => vB(true), () => Null));
}

/** Computes every calculated field of the row in dependency order. */
export function computeGrandparenthoods(tc: GrandparenthoodsRow): GrandparenthoodsRow {
  // Level 1
  calcGuard(tc, grandparenthoodsFieldTypes, "links", () => { tc.links = calcGrandparenthoodsLinks(tc); });
  // Level 2
  calcGuard(tc, grandparenthoodsFieldTypes, "name", () => { tc.name = calcGrandparenthoodsName(tc); });
  calcGuard(tc, grandparenthoodsFieldTypes, "holds", () => { tc.holds = calcGrandparenthoodsHolds(tc); });
  return tc;
}

/** Reads Grandparenthoods rows from a JSON array file. */
export function loadGrandparenthoodsRows(file: string): GrandparenthoodsRow[] {
  return loadRows(file, { fields: grandparenthoodsFieldTypes }) as unknown as GrandparenthoodsRow[];
}

// =============================================================================
// GRADELEVELRECORDS TABLE
// Every grade level stated for an individual, kept when a later one replaces it: her page keeps one current grade, the latest switched on. The current value is time (which was stated last), not a flag.
// =============================================================================

/** A row in the GradeLevelRecords table. */
export interface GradeLevelRecordsRow {
  /** Key: individual|grade|when it was stated. */
  grade_level_record_id: string;
  /** Order 2. The fact in words. */
  name: string | null;
  /** The world this record belongs to. */
  reasoning: string;
  /** Whose value it is. */
  individual: string;
  /** Order 1. Whose value it is, by name. */
  individual_label: string | null;
  /** The value stated. */
  grade_level: number;
  /** When the value was switched on, in the order facts are stated (transaction time). The latest one switched on is in effect. */
  stated_seq: number;
  /** The value is switched on. A value switched off stays in the record. */
  is_on: boolean;
  /** Order 2. When the individual's latest value switched on was stated. */
  individual_latest_seq: number | null;
  /** Order 3. This value is the one in effect: switched on, and the latest switched on. */
  is_current: boolean | null;
  /** Order 1. The key is individual|grade|StatedSeq, the key the individual's current value is looked up by. */
  key_is_canonical: boolean | null;
  _erb_errors?: Record<string, string>;
}

const gradeLevelRecordsFieldTypes: Record<string, FieldType> = {
  grade_level_record_id: "string",
  name: "*string",
  reasoning: "string",
  individual: "string",
  individual_label: "*string",
  grade_level: "int",
  stated_seq: "int",
  is_on: "bool",
  individual_latest_seq: "*int",
  is_current: "*bool",
  key_is_canonical: "*bool",
};

/** Computes the Name calculated field.
 *  Order 2. The fact in words.
 *  Formula: =CONCAT({{IndividualLabel}}, " has grade level ", {{GradeLevel}}) */
export function calcGradeLevelRecordsName(tc: GradeLevelRecordsRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.individual_label)), vS(" has grade level "), erbText(vIntPlain(tc.grade_level))));
}

/** Computes the IsCurrent calculated field.
 *  Order 3. This value is the one in effect: switched on, and the latest switched on.
 *  Formula: =AND({{IsOn}}, {{StatedSeq}} = {{IndividualLatestSeq}}) */
export function calcGradeLevelRecordsIsCurrent(tc: GradeLevelRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBoolPlain(tc.is_on)), erbBool3(erbEq(erbNullif(vIntPlain(tc.stated_seq)), vInt(tc.individual_latest_seq)))));
}

/** Computes the KeyIsCanonical calculated field.
 *  Order 1. The key is individual|grade|StatedSeq, the key the individual's current value is looked up by.
 *  Formula: ={{GradeLevelRecordId}} = CONCAT({{Individual}}, "|grade|", {{StatedSeq}}) */
export function calcGradeLevelRecordsKeyIsCanonical(tc: GradeLevelRecordsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStrPlain(tc.grade_level_record_id)), erbConcat(erbText(vStrPlain(tc.individual)), vS("|grade|"), erbText(vIntPlain(tc.stated_seq)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeGradeLevelRecords(tc: GradeLevelRecordsRow): GradeLevelRecordsRow {
  // Level 1
  calcGuard(tc, gradeLevelRecordsFieldTypes, "name", () => { tc.name = calcGradeLevelRecordsName(tc); });
  calcGuard(tc, gradeLevelRecordsFieldTypes, "is_current", () => { tc.is_current = calcGradeLevelRecordsIsCurrent(tc); });
  calcGuard(tc, gradeLevelRecordsFieldTypes, "key_is_canonical", () => { tc.key_is_canonical = calcGradeLevelRecordsKeyIsCanonical(tc); });
  return tc;
}

/** Reads GradeLevelRecords rows from a JSON array file. */
export function loadGradeLevelRecordsRows(file: string): GradeLevelRecordsRow[] {
  return loadRows(file, { fields: gradeLevelRecordsFieldTypes }) as unknown as GradeLevelRecordsRow[];
}

// =============================================================================
// ROLERECORDS TABLE
// Every role stated for an individual; the latest one switched on is in effect, like a grade level.
// =============================================================================

/** A row in the RoleRecords table. */
export interface RoleRecordsRow {
  /** Key: individual|role|when it was stated. */
  role_record_id: string;
  /** Order 2. The fact in words. */
  name: string | null;
  /** The world this record belongs to. */
  reasoning: string;
  /** Whose value it is. */
  individual: string;
  /** Order 1. Whose value it is, by name. */
  individual_label: string | null;
  /** The value stated. */
  role: string;
  /** When the value was switched on, in the order facts are stated (transaction time). The latest one switched on is in effect. */
  stated_seq: number;
  /** The value is switched on. A value switched off stays in the record. */
  is_on: boolean;
  /** Order 2. When the individual's latest value switched on was stated. */
  individual_latest_seq: number | null;
  /** Order 3. This value is the one in effect: switched on, and the latest switched on. */
  is_current: boolean | null;
  /** Order 1. The key is individual|role|StatedSeq, the key the individual's current value is looked up by. */
  key_is_canonical: boolean | null;
  _erb_errors?: Record<string, string>;
}

const roleRecordsFieldTypes: Record<string, FieldType> = {
  role_record_id: "string",
  name: "*string",
  reasoning: "string",
  individual: "string",
  individual_label: "*string",
  role: "string",
  stated_seq: "int",
  is_on: "bool",
  individual_latest_seq: "*int",
  is_current: "*bool",
  key_is_canonical: "*bool",
};

/** Computes the Name calculated field.
 *  Order 2. The fact in words.
 *  Formula: =CONCAT({{IndividualLabel}}, " has role ", {{Role}}) */
export function calcRoleRecordsName(tc: RoleRecordsRow): string | null {
  return toStringPtr(erbConcat(erbText(vStr(tc.individual_label)), vS(" has role "), erbText(vStrPlain(tc.role))));
}

/** Computes the IsCurrent calculated field.
 *  Order 3. This value is the one in effect: switched on, and the latest switched on.
 *  Formula: =AND({{IsOn}}, {{StatedSeq}} = {{IndividualLatestSeq}}) */
export function calcRoleRecordsIsCurrent(tc: RoleRecordsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBoolPlain(tc.is_on)), erbBool3(erbEq(erbNullif(vIntPlain(tc.stated_seq)), vInt(tc.individual_latest_seq)))));
}

/** Computes the KeyIsCanonical calculated field.
 *  Order 1. The key is individual|role|StatedSeq, the key the individual's current value is looked up by.
 *  Formula: ={{RoleRecordId}} = CONCAT({{Individual}}, "|role|", {{StatedSeq}}) */
export function calcRoleRecordsKeyIsCanonical(tc: RoleRecordsRow): boolean | null {
  return toBoolPtr(erbEq(erbNullif(vStrPlain(tc.role_record_id)), erbConcat(erbText(vStrPlain(tc.individual)), vS("|role|"), erbText(vIntPlain(tc.stated_seq)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRoleRecords(tc: RoleRecordsRow): RoleRecordsRow {
  // Level 1
  calcGuard(tc, roleRecordsFieldTypes, "name", () => { tc.name = calcRoleRecordsName(tc); });
  calcGuard(tc, roleRecordsFieldTypes, "is_current", () => { tc.is_current = calcRoleRecordsIsCurrent(tc); });
  calcGuard(tc, roleRecordsFieldTypes, "key_is_canonical", () => { tc.key_is_canonical = calcRoleRecordsKeyIsCanonical(tc); });
  return tc;
}

/** Reads RoleRecords rows from a JSON array file. */
export function loadRoleRecordsRows(file: string): RoleRecordsRow[] {
  return loadRows(file, { fields: roleRecordsFieldTypes }) as unknown as RoleRecordsRow[];
}

/** Bounds the runner's passes over the dataset. */
export const calculatedFieldCount = 43;

/** Every table, in rulebook order. */
export const erbTables: TableSpec[] = [
  { name: "__meta__", file: "__meta__", rulebookRows: 8, fields: __meta__FieldTypes,
    columns: { meta_key: "MetaKey", name: "Name", value_type: "ValueType", string_value: "StringValue", json_value: "JsonValue" },
    stored: ["meta_key", "value_type", "string_value", "json_value"],
    compute: (row: any) => compute__meta__(row as __meta__Row),
    lookups: [],
    aggregations: [] },
  { name: "Reasoning", file: "reasoning", rulebookRows: 1, fields: reasoningFieldTypes,
    columns: { reasoning_id: "ReasoningId", name: "Name", label: "Label", inverse_is_on: "InverseIsOn", symmetric_is_on: "SymmetricIsOn", chain_is_on: "ChainIsOn", domain_range_is_on: "DomainRangeIsOn", disjoint_is_on: "DisjointIsOn", count_of_individuals: "CountOfIndividuals", count_of_people: "CountOfPeople", count_of_schools: "CountOfSchools", count_of_stated_parenthoods: "CountOfStatedParenthoods", count_of_stated_marriages: "CountOfStatedMarriages", count_of_stated_friendships: "CountOfStatedFriendships", count_of_stated_individual_facts: "CountOfStatedIndividualFacts", count_of_current_grade_levels: "CountOfCurrentGradeLevels", count_of_current_roles: "CountOfCurrentRoles", count_of_stated_facts: "CountOfStatedFacts", count_of_child_of_conclusions: "CountOfChildOfConclusions", count_of_mirrored_marriages: "CountOfMirroredMarriages", count_of_mirrored_friendships: "CountOfMirroredFriendships", count_of_grandparent_conclusions: "CountOfGrandparentConclusions", count_of_person_conclusions: "CountOfPersonConclusions", count_of_school_conclusions: "CountOfSchoolConclusions", count_of_conclusions: "CountOfConclusions", count_of_contradictions: "CountOfContradictions", is_consistent: "IsConsistent", summary: "Summary" },
    stored: ["reasoning_id", "label", "inverse_is_on", "symmetric_is_on", "chain_is_on", "domain_range_is_on", "disjoint_is_on"],
    compute: (row: any) => computeReasoning(row as ReasoningRow),
    lookups: [],
    aggregations: [
      { field: "count_of_individuals", op: "COUNTIFS", table: "individuals", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }] },
      { field: "count_of_people", op: "COUNTIFS", table: "individuals", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_person", kind: "literal", literal: vB(true) }] },
      { field: "count_of_schools", op: "COUNTIFS", table: "individuals", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_school", kind: "literal", literal: vB(true) }] },
      { field: "count_of_stated_parenthoods", op: "COUNTIFS", table: "parenthoods", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }] },
      { field: "count_of_stated_marriages", op: "COUNTIFS", table: "marriages", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }] },
      { field: "count_of_stated_friendships", op: "COUNTIFS", table: "friendships", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }] },
      { field: "count_of_stated_individual_facts", op: "SUM", table: "individuals", target: "stated_fact_count", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }] },
      { field: "count_of_current_grade_levels", op: "COUNTIFS", table: "grade_level_records", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_current", kind: "literal", literal: vB(true) }] },
      { field: "count_of_current_roles", op: "COUNTIFS", table: "role_records", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_current", kind: "literal", literal: vB(true) }] },
      { field: "count_of_child_of_conclusions", op: "COUNTIFS", table: "parenthoods", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "child_of_holds", kind: "literal", literal: vB(true) }] },
      { field: "count_of_mirrored_marriages", op: "COUNTIFS", table: "marriages", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "mirror_holds", kind: "literal", literal: vB(true) }] },
      { field: "count_of_mirrored_friendships", op: "COUNTIFS", table: "friendships", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "mirror_holds", kind: "literal", literal: vB(true) }] },
      { field: "count_of_grandparent_conclusions", op: "COUNTIFS", table: "grandparenthoods", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "holds", kind: "literal", literal: vB(true) }] },
      { field: "count_of_person_conclusions", op: "COUNTIFS", table: "individuals", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_person_concluded", kind: "literal", literal: vB(true) }] },
      { field: "count_of_school_conclusions", op: "COUNTIFS", table: "individuals", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_school_concluded", kind: "literal", literal: vB(true) }] },
      { field: "count_of_contradictions", op: "COUNTIFS", table: "parenthoods", criteria: [{ range: "reasoning", kind: "field", field: "reasoning_id" }, { range: "is_contradiction", kind: "literal", literal: vB(true) }] },] },
  { name: "Portraits", file: "portraits", rulebookRows: 7, fields: portraitsFieldTypes,
    columns: { portrait_id: "PortraitId", display_name: "DisplayName", page: "Page", image: "Image", crop: "Crop", width: "Width", height: "Height", sort_order: "SortOrder", name: "Name" },
    stored: ["portrait_id", "display_name", "page", "image", "crop", "width", "height", "sort_order"],
    compute: (row: any) => computePortraits(row as PortraitsRow),
    lookups: [],
    aggregations: [] },
  { name: "Individuals", file: "individuals", rulebookRows: 7, fields: individualsFieldTypes,
    columns: { individual_id: "IndividualId", name: "Name", label: "Label", sort_order: "SortOrder", reasoning: "Reasoning", portrait: "Portrait", portrait_image: "PortraitImage", symmetric_is_on: "SymmetricIsOn", domain_range_is_on: "DomainRangeIsOn", is_asserted_person: "IsAssertedPerson", is_asserted_school: "IsAssertedSchool", age: "Age", attends: "Attends", attends_label: "AttendsLabel", has_age: "HasAge", has_attends: "HasAttends", stated_fact_count: "StatedFactCount", count_of_grade_levels_on: "CountOfGradeLevelsOn", latest_grade_level_seq: "LatestGradeLevelSeq", has_grade_level: "HasGradeLevel", current_grade_level_record: "CurrentGradeLevelRecord", grade_level: "GradeLevel", count_of_roles_on: "CountOfRolesOn", latest_role_seq: "LatestRoleSeq", has_role: "HasRole", current_role_record: "CurrentRoleRecord", role: "Role", has_person_evidence: "HasPersonEvidence", is_person_by_domain: "IsPersonByDomain", is_person: "IsPerson", is_person_concluded: "IsPersonConcluded", attended_by_count: "AttendedByCount", is_school_by_range: "IsSchoolByRange", is_school: "IsSchool", is_school_concluded: "IsSchoolConcluded", child_count: "ChildCount", parent_count: "ParentCount", child_of_fact_count: "ChildOfFactCount", grandchild_count: "GrandchildCount", grandparent_count: "GrandparentCount", descendant_count: "DescendantCount", ancestor_count: "AncestorCount", stated_spouse_count: "StatedSpouseCount", mirrored_spouse_count: "MirroredSpouseCount", spouse_count: "SpouseCount", stated_friend_count: "StatedFriendCount", mirrored_friend_count: "MirroredFriendCount", friend_count: "FriendCount" },
    stored: ["individual_id", "label", "sort_order", "reasoning", "portrait", "is_asserted_person", "is_asserted_school", "age", "attends"],
    compute: (row: any) => computeIndividuals(row as IndividualsRow),
    lookups: [
      { field: "portrait_image", target: "portraits", ret: "image", key: "portrait", match: "portrait_id" },
      { field: "symmetric_is_on", target: "reasoning", ret: "symmetric_is_on", key: "reasoning", match: "reasoning_id" },
      { field: "domain_range_is_on", target: "reasoning", ret: "domain_range_is_on", key: "reasoning", match: "reasoning_id" },
      { field: "attends_label", target: "individuals", ret: "label", key: "attends", match: "individual_id" },
      { field: "grade_level", target: "grade_level_records", ret: "grade_level", key: "current_grade_level_record", match: "grade_level_record_id" },
      { field: "role", target: "role_records", ret: "role", key: "current_role_record", match: "role_record_id" },],
    aggregations: [
      { field: "count_of_grade_levels_on", op: "COUNTIFS", table: "grade_level_records", criteria: [{ range: "individual", kind: "field", field: "individual_id" }, { range: "is_on", kind: "literal", literal: vB(true) }] },
      { field: "latest_grade_level_seq", op: "MAX", table: "grade_level_records", target: "stated_seq", criteria: [{ range: "individual", kind: "field", field: "individual_id" }, { range: "is_on", kind: "literal", literal: vB(true) }] },
      { field: "count_of_roles_on", op: "COUNTIFS", table: "role_records", criteria: [{ range: "individual", kind: "field", field: "individual_id" }, { range: "is_on", kind: "literal", literal: vB(true) }] },
      { field: "latest_role_seq", op: "MAX", table: "role_records", target: "stated_seq", criteria: [{ range: "individual", kind: "field", field: "individual_id" }, { range: "is_on", kind: "literal", literal: vB(true) }] },
      { field: "attended_by_count", op: "COUNTIFS", table: "individuals", criteria: [{ range: "attends", kind: "field", field: "individual_id" }] },
      { field: "child_count", op: "COUNTIFS", table: "parenthoods", criteria: [{ range: "parent", kind: "field", field: "individual_id" }] },
      { field: "parent_count", op: "COUNTIFS", table: "parenthoods", criteria: [{ range: "child", kind: "field", field: "individual_id" }] },
      { field: "child_of_fact_count", op: "COUNTIFS", table: "parenthoods", criteria: [{ range: "child", kind: "field", field: "individual_id" }, { range: "child_of_holds", kind: "literal", literal: vB(true) }] },
      { field: "grandchild_count", op: "COUNTIFS", table: "grandparenthoods", criteria: [{ range: "grandparent", kind: "field", field: "individual_id" }, { range: "holds", kind: "literal", literal: vB(true) }] },
      { field: "grandparent_count", op: "COUNTIFS", table: "grandparenthoods", criteria: [{ range: "grandchild", kind: "field", field: "individual_id" }, { range: "holds", kind: "literal", literal: vB(true) }] },
      { field: "descendant_count", op: "COUNTIFS", table: "vw_parenthoods_closure", criteria: [{ range: "from_id", kind: "field", field: "individual_id" }] },
      { field: "ancestor_count", op: "COUNTIFS", table: "vw_parenthoods_closure", criteria: [{ range: "to_id", kind: "field", field: "individual_id" }] },
      { field: "stated_spouse_count", op: "COUNTIFS", table: "marriages", criteria: [{ range: "partner_a", kind: "field", field: "individual_id" }] },
      { field: "mirrored_spouse_count", op: "COUNTIFS", table: "marriages", criteria: [{ range: "partner_b", kind: "field", field: "individual_id" }, { range: "mirror_holds", kind: "literal", literal: vB(true) }] },
      { field: "stated_friend_count", op: "COUNTIFS", table: "friendships", criteria: [{ range: "friend_a", kind: "field", field: "individual_id" }] },
      { field: "mirrored_friend_count", op: "COUNTIFS", table: "friendships", criteria: [{ range: "friend_b", kind: "field", field: "individual_id" }, { range: "mirror_holds", kind: "literal", literal: vB(true) }] },] },
  { name: "Parenthoods", file: "parenthoods", rulebookRows: 3, fields: parenthoodsFieldTypes,
    columns: { parenthood_id: "ParenthoodId", name: "Name", reasoning: "Reasoning", parent: "Parent", child: "Child", parent_label: "ParentLabel", child_label: "ChildLabel", inverse_is_on: "InverseIsOn", symmetric_is_on: "SymmetricIsOn", disjoint_is_on: "DisjointIsOn", child_of_holds: "ChildOfHolds", child_of_sentence: "ChildOfSentence", spouse_as_stated_count: "SpouseAsStatedCount", spouse_as_mirrored_count: "SpouseAsMirroredCount", is_contradiction: "IsContradiction", ancestry_closure: "AncestryClosure" },
    stored: ["parenthood_id", "reasoning", "parent", "child"],
    compute: (row: any) => computeParenthoods(row as ParenthoodsRow),
    lookups: [
      { field: "parent_label", target: "individuals", ret: "label", key: "parent", match: "individual_id" },
      { field: "child_label", target: "individuals", ret: "label", key: "child", match: "individual_id" },
      { field: "inverse_is_on", target: "reasoning", ret: "inverse_is_on", key: "reasoning", match: "reasoning_id" },
      { field: "symmetric_is_on", target: "reasoning", ret: "symmetric_is_on", key: "reasoning", match: "reasoning_id" },
      { field: "disjoint_is_on", target: "reasoning", ret: "disjoint_is_on", key: "reasoning", match: "reasoning_id" },],
    aggregations: [
      { field: "spouse_as_stated_count", op: "COUNTIFS", table: "marriages", criteria: [{ range: "partner_a", kind: "field", field: "parent" }, { range: "partner_b", kind: "field", field: "child" }] },
      { field: "spouse_as_mirrored_count", op: "COUNTIFS", table: "marriages", criteria: [{ range: "partner_a", kind: "field", field: "child" }, { range: "partner_b", kind: "field", field: "parent" }, { range: "mirror_holds", kind: "literal", literal: vB(true) }] },] },
  { name: "Marriages", file: "marriages", rulebookRows: 1, fields: marriagesFieldTypes,
    columns: { marriage_id: "MarriageId", name: "Name", reasoning: "Reasoning", partner_a: "PartnerA", partner_b: "PartnerB", partner_a_label: "PartnerALabel", partner_b_label: "PartnerBLabel", symmetric_is_on: "SymmetricIsOn", mirror_holds: "MirrorHolds", mirror_sentence: "MirrorSentence" },
    stored: ["marriage_id", "reasoning", "partner_a", "partner_b"],
    compute: (row: any) => computeMarriages(row as MarriagesRow),
    lookups: [
      { field: "partner_a_label", target: "individuals", ret: "label", key: "partner_a", match: "individual_id" },
      { field: "partner_b_label", target: "individuals", ret: "label", key: "partner_b", match: "individual_id" },
      { field: "symmetric_is_on", target: "reasoning", ret: "symmetric_is_on", key: "reasoning", match: "reasoning_id" },],
    aggregations: [] },
  { name: "Friendships", file: "friendships", rulebookRows: 1, fields: friendshipsFieldTypes,
    columns: { friendship_id: "FriendshipId", name: "Name", reasoning: "Reasoning", friend_a: "FriendA", friend_b: "FriendB", friend_a_label: "FriendALabel", friend_b_label: "FriendBLabel", symmetric_is_on: "SymmetricIsOn", mirror_holds: "MirrorHolds", mirror_sentence: "MirrorSentence" },
    stored: ["friendship_id", "reasoning", "friend_a", "friend_b"],
    compute: (row: any) => computeFriendships(row as FriendshipsRow),
    lookups: [
      { field: "friend_a_label", target: "individuals", ret: "label", key: "friend_a", match: "individual_id" },
      { field: "friend_b_label", target: "individuals", ret: "label", key: "friend_b", match: "individual_id" },
      { field: "symmetric_is_on", target: "reasoning", ret: "symmetric_is_on", key: "reasoning", match: "reasoning_id" },],
    aggregations: [] },
  { name: "Grandparenthoods", file: "grandparenthoods", rulebookRows: 9, fields: grandparenthoodsFieldTypes,
    columns: { grandparenthood_id: "GrandparenthoodId", name: "Name", reasoning: "Reasoning", upper_link: "UpperLink", lower_link: "LowerLink", grandparent: "Grandparent", upper_child: "UpperChild", lower_parent: "LowerParent", grandchild: "Grandchild", grandparent_label: "GrandparentLabel", grandchild_label: "GrandchildLabel", links: "Links", chain_is_on: "ChainIsOn", holds: "Holds" },
    stored: ["grandparenthood_id", "reasoning", "upper_link", "lower_link"],
    compute: (row: any) => computeGrandparenthoods(row as GrandparenthoodsRow),
    lookups: [
      { field: "grandparent", target: "parenthoods", ret: "parent", key: "upper_link", match: "parenthood_id" },
      { field: "upper_child", target: "parenthoods", ret: "child", key: "upper_link", match: "parenthood_id" },
      { field: "lower_parent", target: "parenthoods", ret: "parent", key: "lower_link", match: "parenthood_id" },
      { field: "grandchild", target: "parenthoods", ret: "child", key: "lower_link", match: "parenthood_id" },
      { field: "grandparent_label", target: "parenthoods", ret: "parent_label", key: "upper_link", match: "parenthood_id" },
      { field: "grandchild_label", target: "parenthoods", ret: "child_label", key: "lower_link", match: "parenthood_id" },
      { field: "chain_is_on", target: "reasoning", ret: "chain_is_on", key: "reasoning", match: "reasoning_id" },],
    aggregations: [] },
  { name: "GradeLevelRecords", file: "grade_level_records", rulebookRows: 1, fields: gradeLevelRecordsFieldTypes,
    columns: { grade_level_record_id: "GradeLevelRecordId", name: "Name", reasoning: "Reasoning", individual: "Individual", individual_label: "IndividualLabel", grade_level: "GradeLevel", stated_seq: "StatedSeq", is_on: "IsOn", individual_latest_seq: "IndividualLatestSeq", is_current: "IsCurrent", key_is_canonical: "KeyIsCanonical" },
    stored: ["grade_level_record_id", "reasoning", "individual", "grade_level", "stated_seq", "is_on"],
    compute: (row: any) => computeGradeLevelRecords(row as GradeLevelRecordsRow),
    lookups: [
      { field: "individual_label", target: "individuals", ret: "label", key: "individual", match: "individual_id" },
      { field: "individual_latest_seq", target: "individuals", ret: "latest_grade_level_seq", key: "individual", match: "individual_id" },],
    aggregations: [] },
  { name: "RoleRecords", file: "role_records", rulebookRows: 1, fields: roleRecordsFieldTypes,
    columns: { role_record_id: "RoleRecordId", name: "Name", reasoning: "Reasoning", individual: "Individual", individual_label: "IndividualLabel", role: "Role", stated_seq: "StatedSeq", is_on: "IsOn", individual_latest_seq: "IndividualLatestSeq", is_current: "IsCurrent", key_is_canonical: "KeyIsCanonical" },
    stored: ["role_record_id", "reasoning", "individual", "role", "stated_seq", "is_on"],
    compute: (row: any) => computeRoleRecords(row as RoleRecordsRow),
    lookups: [
      { field: "individual_label", target: "individuals", ret: "label", key: "individual", match: "individual_id" },
      { field: "individual_latest_seq", target: "individuals", ret: "latest_role_seq", key: "individual", match: "individual_id" },],
    aggregations: [] },
];

/** Materializes each vw_<entity>_closure view aggregations read. */
export const erbClosures: ClosureSpec[] = [
  { view: "vw_parenthoods_closure", source: "parenthoods", from: "parent", to: "child", filter: null },
];

/**
 * Computes every field of every table from the stored (raw and relationship)
 * values given, keyed by the rulebook's own table and field names, and returns
 * every table the same way. Call it again whenever a row changes: a change of
 * data is a new call, never a new SDK. The conformance runner (main.ts) runs
 * the same computation over files. Throws ErbError naming what it cannot do.
 */
export function evaluate(rows: Record<string, Array<Record<string, unknown>>>): Record<string, Array<Record<string, unknown>>> {
  return erbEvaluate(erbTables, erbClosures, calculatedFieldCount, rows);
}
