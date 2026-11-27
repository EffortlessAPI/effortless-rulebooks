// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/talisman-s-south-park-ontology-explainer-rulebook.json
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
  erbPart,
  erbConfigure,
  erbNeg,
  erbAdd,
  erbSub,
  erbAddDays,
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
export const erbBuildParameters: Record<string, string> = { "erbDateDiff": "calendar", "erbTimezone": "UTC", "erbDateTimeText": "iso8601", "erbBlankLogic": "coerce", "erbBlankValue": "coerce", "erbWholeNumber": "by-field-type" };
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
// GRAPHS TABLE
// The explainer page as one graph: its wording variables and its totals.
// =============================================================================

/** A row in the Graphs table. */
export interface GraphsRow {
  /** Key of the graph. */
  graph_id: string;
  /** Display name of the graph. */
  label: string;
  /** The quote mark written around a literal value in a fact's wording. */
  literal_quote: string;
  /** The marker written between a literal value and its datatype, as in Turtle. */
  literal_type_marker: string;
  /** Source line shown for a fact the page states as a teaching example. */
  built_in_source_text: string;
  /** Source line shown for a fact the reader stated. */
  user_source_text: string;
  /** Message shown when the selected facts and rules add no conclusion. */
  no_conclusions_text: string;
  /** Order 1. Display name of the graph. */
  name: string | null;
  /** Order 5. Facts that are switched on and in effect. */
  count_of_stated_facts: number | null;
  /** Order 18. Facts that hold only because an enabled rule concluded them. */
  count_of_conclusions: number | null;
  /** Order 18. Disjointness checks that found both links between the same pair. */
  count_of_violations: number | null;
  /** Order 19. True when no enabled disjointness rule is violated. */
  is_consistent: boolean | null;
  /** Order 19. The empty-state message for the conclusions panel, blank when there are conclusions. */
  conclusions_message: string | null;
  /** Order 5. Listed facts that are not in effect; the Add next fact button is enabled while this is above zero. */
  count_of_facts_to_add: number | null;
  /** Order 6. True while some listed fact can still be added. */
  has_facts_to_add: boolean | null;
  /** Order 5. List position of the first listed fact that is not in effect. */
  next_fact_sort_order: number | null;
  /** Order 1. Highest activation number used so far. */
  max_activation_seq: number | null;
  /** Order 2. The activation number the next switched-on fact takes. */
  next_activation_seq: number | null;
  /** Order 2. Highest list position among listed facts. */
  max_listed_sort_order: number | null;
  /** Order 3. The list position a newly added fact takes. */
  next_listed_sort_order: number | null;
  /** Order 1. Highest sort order of any fact, listed or candidate. */
  max_sort_order: number | null;
  /** Order 2. The sort order a new candidate fact, or a fact removed from the list, takes. */
  next_candidate_sort_order: number | null;
  _erb_errors?: Record<string, string>;
}

const graphsFieldTypes: Record<string, FieldType> = {
  graph_id: "string",
  label: "string",
  literal_quote: "string",
  literal_type_marker: "string",
  built_in_source_text: "string",
  user_source_text: "string",
  no_conclusions_text: "string",
  name: "*string",
  count_of_stated_facts: "*int",
  count_of_conclusions: "*int",
  count_of_violations: "*int",
  is_consistent: "*bool",
  conclusions_message: "*string",
  count_of_facts_to_add: "*int",
  has_facts_to_add: "*bool",
  next_fact_sort_order: "*int",
  max_activation_seq: "*int",
  next_activation_seq: "*int",
  max_listed_sort_order: "*int",
  next_listed_sort_order: "*int",
  max_sort_order: "*int",
  next_candidate_sort_order: "*int",
};

/** Computes the Name calculated field.
 *  Order 1. Display name of the graph.
 *  Formula: ={{Label}} */
export function calcGraphsName(tc: GraphsRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes the IsConsistent calculated field.
 *  Order 19. True when no enabled disjointness rule is violated.
 *  Formula: ={{CountOfViolations}} = 0 */
export function calcGraphsIsConsistent(tc: GraphsRow): boolean | null {
  return toBoolPtr(erbEq(vInt(tc.count_of_violations), vI(0)));
}

/** Computes the ConclusionsMessage calculated field.
 *  Order 19. The empty-state message for the conclusions panel, blank when there are conclusions.
 *  Formula: =IF({{CountOfConclusions}} = 0, {{NoConclusionsText}}, "") */
export function calcGraphsConclusionsMessage(tc: GraphsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(vInt(tc.count_of_conclusions), vI(0))), () => vStrPlain(tc.no_conclusions_text), () => vS("")));
}

/** Computes the HasFactsToAdd calculated field.
 *  Order 6. True while some listed fact can still be added.
 *  Formula: ={{CountOfFactsToAdd}} > 0 */
export function calcGraphsHasFactsToAdd(tc: GraphsRow): boolean | null {
  return toBoolPtr(erbCmp(vInt(tc.count_of_facts_to_add), ">", vI(0)));
}

/** Computes the NextActivationSeq calculated field.
 *  Order 2. The activation number the next switched-on fact takes.
 *  Formula: ={{MaxActivationSeq}} + 1 */
export function calcGraphsNextActivationSeq(tc: GraphsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.max_activation_seq), vI(1))));
}

/** Computes the NextListedSortOrder calculated field.
 *  Order 3. The list position a newly added fact takes.
 *  Formula: ={{MaxListedSortOrder}} + 1 */
export function calcGraphsNextListedSortOrder(tc: GraphsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.max_listed_sort_order), vI(1))));
}

/** Computes the NextCandidateSortOrder calculated field.
 *  Order 2. The sort order a new candidate fact, or a fact removed from the list, takes.
 *  Formula: ={{MaxSortOrder}} + 1 */
export function calcGraphsNextCandidateSortOrder(tc: GraphsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.max_sort_order), vI(1))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeGraphs(tc: GraphsRow): GraphsRow {
  // Level 1
  calcGuard(tc, graphsFieldTypes, "name", () => { tc.name = calcGraphsName(tc); });
  calcGuard(tc, graphsFieldTypes, "is_consistent", () => { tc.is_consistent = calcGraphsIsConsistent(tc); });
  calcGuard(tc, graphsFieldTypes, "conclusions_message", () => { tc.conclusions_message = calcGraphsConclusionsMessage(tc); });
  calcGuard(tc, graphsFieldTypes, "has_facts_to_add", () => { tc.has_facts_to_add = calcGraphsHasFactsToAdd(tc); });
  calcGuard(tc, graphsFieldTypes, "next_activation_seq", () => { tc.next_activation_seq = calcGraphsNextActivationSeq(tc); });
  calcGuard(tc, graphsFieldTypes, "next_listed_sort_order", () => { tc.next_listed_sort_order = calcGraphsNextListedSortOrder(tc); });
  calcGuard(tc, graphsFieldTypes, "next_candidate_sort_order", () => { tc.next_candidate_sort_order = calcGraphsNextCandidateSortOrder(tc); });
  return tc;
}

/** Reads Graphs rows from a JSON array file. */
export function loadGraphsRows(file: string): GraphsRow[] {
  return loadRows(file, { fields: graphsFieldTypes }) as unknown as GraphsRow[];
}

// =============================================================================
// RULES TABLE
// The five rule switches on the page.
// =============================================================================

/** A row in the Rules table. */
export interface RulesRow {
  /** Key of the rule. */
  rule_id: string;
  /** The rule's name on the page. */
  label: string;
  /** The rule's one-line explanation on the page. */
  explanation: string;
  /** The rule's checkbox. */
  is_enabled: boolean;
  /** Position in the rule list. */
  sort_order: number;
  /** Order 1. The rule's name. */
  name: string | null;
  /** Order 15. Rule applications that fired, counting every derivation. */
  count_of_firings: number | null;
  _erb_errors?: Record<string, string>;
}

const rulesFieldTypes: Record<string, FieldType> = {
  rule_id: "string",
  label: "string",
  explanation: "string",
  is_enabled: "bool",
  sort_order: "int",
  name: "*string",
  count_of_firings: "*int",
};

/** Computes the Name calculated field.
 *  Order 1. The rule's name.
 *  Formula: ={{Label}} */
export function calcRulesName(tc: RulesRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRules(tc: RulesRow): RulesRow {
  // Level 1
  calcGuard(tc, rulesFieldTypes, "name", () => { tc.name = calcRulesName(tc); });
  return tc;
}

/** Reads Rules rows from a JSON array file. */
export function loadRulesRows(file: string): RulesRow[] {
  return loadRows(file, { fields: rulesFieldTypes }) as unknown as RulesRow[];
}

// =============================================================================
// FIRINGKINDS TABLE
// The ways a rule can apply. Domain and range belong to one rule but are worded apart; 'disjoint' names the rule the disjointness checks answer to.
// =============================================================================

/** A row in the FiringKinds table. */
export interface FiringKindsRow {
  /** Key of the kind. */
  firing_kind_id: string;
  /** The rule switch this kind belongs to. */
  rule: string;
  /** How a conclusion card names the rule, as in 'Worked out by the inverse rule'. */
  wording: string;
  /** Order in which the page's reasoner tries the kinds for one fact. */
  sort_order: number;
  /** Order 1. The kind's wording. */
  name: string | null;
  /** Order 1. Whether the owning rule is switched on. */
  rule_enabled: boolean | null;
  _erb_errors?: Record<string, string>;
}

const firingKindsFieldTypes: Record<string, FieldType> = {
  firing_kind_id: "string",
  rule: "string",
  wording: "string",
  sort_order: "int",
  name: "*string",
  rule_enabled: "*bool",
};

/** Computes the Name calculated field.
 *  Order 1. The kind's wording.
 *  Formula: ={{Wording}} */
export function calcFiringKindsName(tc: FiringKindsRow): string | null {
  return toStringPtr(vStrPlain(tc.wording));
}

/** Computes every calculated field of the row in dependency order. */
export function computeFiringKinds(tc: FiringKindsRow): FiringKindsRow {
  // Level 1
  calcGuard(tc, firingKindsFieldTypes, "name", () => { tc.name = calcFiringKindsName(tc); });
  return tc;
}

/** Reads FiringKinds rows from a JSON array file. */
export function loadFiringKindsRows(file: string): FiringKindsRow[] {
  return loadRows(file, { fields: firingKindsFieldTypes }) as unknown as FiringKindsRow[];
}

// =============================================================================
// THINGS TABLE
// Every individual and class the page can draw.
// =============================================================================

/** A row in the Things table. */
export interface ThingsRow {
  /** Key of the thing; also its local name in the model namespace. */
  thing_id: string;
  /** The name the page shows. */
  label: string;
  /** 'individual' or 'class'. */
  kind: string;
  /** Position in the cast. */
  sort_order: number;
  /** The portrait drawn for this thing; set when its name matches a portrait alias. */
  portrait: string | null;
  /** Order 1. The name on the portrait's Fandom page. */
  portrait_name: string | null;
  /** Order 1. The Fandom page credited for the portrait. */
  portrait_page: string | null;
  /** Order 1. The original Fandom image. */
  portrait_image: string | null;
  /** Order 1. The portrait's crop as an SVG viewBox. */
  portrait_crop: string | null;
  /** Order 1. The original image's width in pixels. */
  portrait_width: number | null;
  /** Order 1. The original image's height in pixels. */
  portrait_height: number | null;
  /** Order 1. The thing's name. */
  name: string | null;
  /** Order 1. The key the graph draws this thing under. */
  node_key: string | null;
  /** Order 17. People this thing is a parent of, stated or concluded. */
  count_of_children: number | null;
  /** Order 17. People this thing is a grandparent of. */
  count_of_grandchildren: number | null;
  /** Order 17. Holding attends facts whose object is this thing. */
  count_as_place: number | null;
  /** Order 17. Holding facts about this thing. */
  count_as_subject: number | null;
  /** Order 17. Holding facts that point at this thing. */
  count_as_object: number | null;
  /** Order 18. True when the graph draws this thing. */
  is_in_graph: boolean | null;
  /** Order 18. How the graph draws the thing: a class, a place someone attends, or a person. */
  node_kind: string | null;
  _erb_errors?: Record<string, string>;
}

const thingsFieldTypes: Record<string, FieldType> = {
  thing_id: "string",
  label: "string",
  kind: "string",
  sort_order: "int",
  portrait: "*string",
  portrait_name: "*string",
  portrait_page: "*string",
  portrait_image: "*string",
  portrait_crop: "*string",
  portrait_width: "*int",
  portrait_height: "*int",
  name: "*string",
  node_key: "*string",
  count_of_children: "*int",
  count_of_grandchildren: "*int",
  count_as_place: "*int",
  count_as_subject: "*int",
  count_as_object: "*int",
  is_in_graph: "*bool",
  node_kind: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The thing's name.
 *  Formula: ={{Label}} */
export function calcThingsName(tc: ThingsRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes the NodeKey calculated field.
 *  Order 1. The key the graph draws this thing under.
 *  Formula: ={{ThingId}} */
export function calcThingsNodeKey(tc: ThingsRow): string | null {
  return toStringPtr(vStrPlain(tc.thing_id));
}

/** Computes the IsInGraph calculated field.
 *  Order 18. True when the graph draws this thing.
 *  Formula: =OR({{CountAsSubject}} > 0, {{CountAsObject}} > 0) */
export function calcThingsIsInGraph(tc: ThingsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbCmp(vInt(tc.count_as_subject), ">", vI(0))), erbBool3(erbCmp(vInt(tc.count_as_object), ">", vI(0)))));
}

/** Computes the NodeKind calculated field.
 *  Order 18. How the graph draws the thing: a class, a place someone attends, or a person.
 *  Formula: =IF({{Kind}} = "", "", IF({{Kind}} = "class", "class", IF({{CountAsPlace}} > 0, "place", "person"))) */
export function calcThingsNodeKind(tc: ThingsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbIsBlank(vStrPlain(tc.kind))), () => vS(""), () => erbIf(erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("class"))), () => vS("class"), () => erbIf(erbBool3(erbCmp(vInt(tc.count_as_place), ">", vI(0))), () => vS("place"), () => vS("person")))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeThings(tc: ThingsRow): ThingsRow {
  // Level 1
  calcGuard(tc, thingsFieldTypes, "name", () => { tc.name = calcThingsName(tc); });
  calcGuard(tc, thingsFieldTypes, "node_key", () => { tc.node_key = calcThingsNodeKey(tc); });
  calcGuard(tc, thingsFieldTypes, "is_in_graph", () => { tc.is_in_graph = calcThingsIsInGraph(tc); });
  calcGuard(tc, thingsFieldTypes, "node_kind", () => { tc.node_kind = calcThingsNodeKind(tc); });
  return tc;
}

/** Reads Things rows from a JSON array file. */
export function loadThingsRows(file: string): ThingsRow[] {
  return loadRows(file, { fields: thingsFieldTypes }) as unknown as ThingsRow[];
}

// =============================================================================
// PORTRAITS TABLE
// The page's Fandom portraits, including those for characters a reader may add.
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
// PORTRAITALIASES TABLE
// The names a portrait answers to. A new thing whose name matches an alias gets that portrait.
// =============================================================================

/** A row in the PortraitAliases table. */
export interface PortraitAliasesRow {
  /** Key: the name in lower case with single spaces. */
  portrait_alias_id: string;
  /** The portrait this name selects. */
  portrait: string;
  /** Order 1. The alias. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const portraitAliasesFieldTypes: Record<string, FieldType> = {
  portrait_alias_id: "string",
  portrait: "string",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The alias.
 *  Formula: ={{PortraitAliasId}} */
export function calcPortraitAliasesName(tc: PortraitAliasesRow): string | null {
  return toStringPtr(vStrPlain(tc.portrait_alias_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computePortraitAliases(tc: PortraitAliasesRow): PortraitAliasesRow {
  // Level 1
  calcGuard(tc, portraitAliasesFieldTypes, "name", () => { tc.name = calcPortraitAliasesName(tc); });
  return tc;
}

/** Reads PortraitAliases rows from a JSON array file. */
export function loadPortraitAliasesRows(file: string): PortraitAliasesRow[] {
  return loadRows(file, { fields: portraitAliasesFieldTypes }) as unknown as PortraitAliasesRow[];
}

// =============================================================================
// PROPERTIES TABLE
// The ten properties, with what the symmetric, domain and range rules read. Axioms that relate one property to another (inverse, chain, disjoint) are rows of PropertyAxioms.
// =============================================================================

/** A row in the Properties table. */
export interface PropertiesRow {
  /** Key of the property; also its local name in the model namespace. */
  property_id: string;
  /** The property's wording on the page. */
  label: string;
  /** 'object' links two things, 'datatype' links a thing to a value, 'type' states class membership. */
  property_kind: string;
  /** The value's XML Schema datatype for a datatype property; blank otherwise. */
  datatype: string | null;
  /** Offered in the page's Add fact form. */
  is_user_addable: boolean;
  /** The page keeps one current value per person: switching a value on replaces the earlier one. */
  has_one_current_value: boolean;
  /** The symmetric rule makes this link true in both directions. */
  is_symmetric: boolean;
  /** The class the domain rule gives this property's subject. */
  domain_class: string | null;
  /** The class the range rule gives this property's object. */
  range_class: string | null;
  /** Position in the Add fact form's property list. */
  sort_order: number;
  /** The small label under a value node for this property: 'role value' or 'number value'. */
  value_node_label: string | null;
  /** How the Add fact form names a replaced value, as in 'Stan now has grade level 5'. */
  current_value_wording: string | null;
  /** How the integer check names this property's value, as in 'for grade level'. */
  input_name: string | null;
  /** The word the current-record summary uses for this property's values: role, grade or school. */
  record_kind: string | null;
  /** Position of this property in the current-record summary. */
  record_order: number | null;
  /** Order 1. The property's wording. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const propertiesFieldTypes: Record<string, FieldType> = {
  property_id: "string",
  label: "string",
  property_kind: "string",
  datatype: "*string",
  is_user_addable: "bool",
  has_one_current_value: "bool",
  is_symmetric: "bool",
  domain_class: "*string",
  range_class: "*string",
  sort_order: "int",
  value_node_label: "*string",
  current_value_wording: "*string",
  input_name: "*string",
  record_kind: "*string",
  record_order: "*int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The property's wording.
 *  Formula: ={{Label}} */
export function calcPropertiesName(tc: PropertiesRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeProperties(tc: PropertiesRow): PropertiesRow {
  // Level 1
  calcGuard(tc, propertiesFieldTypes, "name", () => { tc.name = calcPropertiesName(tc); });
  return tc;
}

/** Reads Properties rows from a JSON array file. */
export function loadPropertiesRows(file: string): PropertiesRow[] {
  return loadRows(file, { fields: propertiesFieldTypes }) as unknown as PropertiesRow[];
}

// =============================================================================
// PROPERTYAXIOMS TABLE
// Axioms that relate one property to others, as OWL states them: an inverse, a two-link chain, or a disjoint pair. The candidate generator reads these rows.
// =============================================================================

/** A row in the PropertyAxioms table. */
export interface PropertyAxiomsRow {
  /** Key: kind:property, with ~other for a disjoint pair. */
  property_axiom_id: string;
  /** inverse, chain or disjoint. */
  kind: string;
  /** The property the axiom is about: the one inverted, the one a chain derives, or the first of a disjoint pair. */
  property: string;
  /** The inverse property, the chain's first link, or the other property of a disjoint pair. */
  first: string;
  /** The chain's second link. */
  second: string | null;
  /** Order 1. The axiom's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const propertyAxiomsFieldTypes: Record<string, FieldType> = {
  property_axiom_id: "string",
  kind: "string",
  property: "string",
  first: "string",
  second: "*string",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The axiom's key.
 *  Formula: ={{PropertyAxiomId}} */
export function calcPropertyAxiomsName(tc: PropertyAxiomsRow): string | null {
  return toStringPtr(vStrPlain(tc.property_axiom_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computePropertyAxioms(tc: PropertyAxiomsRow): PropertyAxiomsRow {
  // Level 1
  calcGuard(tc, propertyAxiomsFieldTypes, "name", () => { tc.name = calcPropertyAxiomsName(tc); });
  return tc;
}

/** Reads PropertyAxioms rows from a JSON array file. */
export function loadPropertyAxiomsRows(file: string): PropertyAxiomsRow[] {
  return loadRows(file, { fields: propertyAxiomsFieldTypes }) as unknown as PropertyAxiomsRow[];
}

// =============================================================================
// STATEMENTS TABLE
// Every fact the page can state or conclude about the cast. A listed fact appears in the page's fact list; the rest are candidates that only a rule can make hold.
// =============================================================================

/** A row in the Statements table. */
export interface StatementsRow {
  /** Key: subject|property|object-or-value. */
  statement_id: string;
  /** The graph the fact belongs to. */
  graph: string;
  /** What the fact describes. */
  subject: string;
  /** How the subject connects to the object. */
  property: string;
  /** The related thing, for an object or type property. */
  object_thing: string | null;
  /** The literal value, for a datatype property. */
  object_value: string | null;
  /** Shown in the page's fact list. */
  is_listed: boolean;
  /** One of the page's teaching facts; the reader cannot remove it. */
  is_built_in: boolean;
  /** One of Stan's detail facts that Show Stan's details switches on. */
  is_detail: boolean;
  /** The fact's checkbox. */
  is_stated: boolean;
  /** When the fact was last switched on, as a running number; decides which value is current. */
  activation_seq: number;
  /** Position in the fact list; candidates follow the listed facts. */
  sort_order: number;
  /** Order 1. The subject's name. */
  subject_label: string | null;
  /** Order 1. The property's wording. */
  property_label: string | null;
  /** Order 1. The object's name, for an object or type property. */
  object_label: string | null;
  /** Order 1. The value's datatype, for a datatype property. */
  property_datatype: string | null;
  /** Order 1. Whether the property keeps one current value per person. */
  property_has_one_current_value: boolean | null;
  /** Order 1. The property's domain class. */
  property_domain_class: string | null;
  /** Order 1. The property's range class. */
  property_range_class: string | null;
  /** Order 1. The graph's literal quote mark. */
  literal_quote: string | null;
  /** Order 1. The graph's datatype marker. */
  literal_type_marker: string | null;
  /** Order 1. The graph's source line for a teaching fact. */
  built_in_source_text: string | null;
  /** Order 1. The graph's source line for a reader's fact. */
  user_source_text: string | null;
  /** Order 6. List position of the next fact to add. */
  next_fact_sort_order: number | null;
  /** Order 1. True when the object is a value rather than a thing. */
  is_literal: boolean | null;
  /** Order 2. The object's name or the value. */
  object_text: string | null;
  /** Order 3. The fact in words. */
  name: string | null;
  /** Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form. */
  words: string | null;
  /** Order 2. Where a listed fact came from. */
  source: string | null;
  /** Order 1. Groups the values one subject has for one property. */
  subject_property_key: string | null;
  /** Order 2. The most recent activation among this subject's values for this property. */
  latest_sibling_activation_seq: number | null;
  /** Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line. */
  is_in_effect: boolean | null;
  /** Order 1. 1 when the fact is in the fact list. */
  listed_unit: number | null;
  /** Order 4. 1 when the fact is in effect. */
  in_effect_unit: number | null;
  /** Order 4. 1 when the fact is listed but not in effect. */
  listed_off_unit: number | null;
  /** Order 7. The fact the Add next fact button switches on. */
  is_next_to_add: boolean | null;
  /** Order 6. Inverse rule applications that conclude this fact. */
  count_of_inverse_firings: number | null;
  /** Order 6. Symmetric rule applications that conclude this fact. */
  count_of_symmetric_firings: number | null;
  /** Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true). */
  holds_before_chain: boolean | null;
  /** Order 10. Property chain applications that conclude this fact. */
  count_of_chain_firings: number | null;
  /** Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true). */
  holds_before_typing: boolean | null;
  /** Order 14. Domain and range applications that conclude this fact. */
  count_of_type_firings: number | null;
  /** Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true). */
  holds: boolean | null;
  /** Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE). */
  is_conclusion: boolean | null;
  /** Order 16. 1 when the fact holds. */
  holds_unit: number | null;
  /** Order 17. 1 when the fact is a conclusion. */
  conclusion_unit: number | null;
  /** Order 16. 1 for a holding parent link. */
  parent_unit: number | null;
  /** Order 16. 1 for a holding grandparent link. */
  grandparent_unit: number | null;
  /** Order 16. 1 for a holding attends link. */
  place_unit: number | null;
  /** Order 7. Order key of the first inverse or symmetric application that concludes this fact. */
  first_pre_chain_order_key: number | null;
  /** Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine. */
  snap_position: number | null;
  /** Order 15. Order key of the first rule application that concludes this fact, in the page reasoner's order. */
  first_fired_order_key: number | null;
  /** Order 20. Rule applications shown as this fact's conclusion card: one for a conclusion, zero otherwise. */
  count_of_chosen_firings: number | null;
  /** Order 1. The graph node this fact starts from. */
  subject_node_key: string | null;
  /** Order 2. The graph node this fact points at: the thing, or one node per value and datatype. */
  object_node_key: string | null;
  /** Order 19. How the graph draws the subject. */
  subject_node_kind: string | null;
  /** Order 19. How the graph draws the object thing. */
  object_thing_node_kind: string | null;
  /** Order 20. How the graph draws the object: a value, a class, a place or a person. */
  object_node_kind: string | null;
  /** Order 1. The small label under a value node. */
  object_value_node_label: string | null;
  /** Order 18. Violated disjointness checks naming this fact first. */
  count_of_violations_as_first: number | null;
  /** Order 18. Violated disjointness checks naming this fact second. */
  count_of_violations_as_second: number | null;
  /** Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour. */
  is_in_contradiction: boolean | null;
  _erb_errors?: Record<string, string>;
}

const statementsFieldTypes: Record<string, FieldType> = {
  statement_id: "string",
  graph: "string",
  subject: "string",
  property: "string",
  object_thing: "*string",
  object_value: "*string",
  is_listed: "bool",
  is_built_in: "bool",
  is_detail: "bool",
  is_stated: "bool",
  activation_seq: "int",
  sort_order: "int",
  subject_label: "*string",
  property_label: "*string",
  object_label: "*string",
  property_datatype: "*string",
  property_has_one_current_value: "*bool",
  property_domain_class: "*string",
  property_range_class: "*string",
  literal_quote: "*string",
  literal_type_marker: "*string",
  built_in_source_text: "*string",
  user_source_text: "*string",
  next_fact_sort_order: "*int",
  is_literal: "*bool",
  object_text: "*string",
  name: "*string",
  words: "*string",
  source: "*string",
  subject_property_key: "*string",
  latest_sibling_activation_seq: "*int",
  is_in_effect: "*bool",
  listed_unit: "*int",
  in_effect_unit: "*int",
  listed_off_unit: "*int",
  is_next_to_add: "*bool",
  count_of_inverse_firings: "*int",
  count_of_symmetric_firings: "*int",
  holds_before_chain: "*bool",
  count_of_chain_firings: "*int",
  holds_before_typing: "*bool",
  count_of_type_firings: "*int",
  holds: "*bool",
  is_conclusion: "*bool",
  holds_unit: "*int",
  conclusion_unit: "*int",
  parent_unit: "*int",
  grandparent_unit: "*int",
  place_unit: "*int",
  first_pre_chain_order_key: "*float64",
  snap_position: "*float64",
  first_fired_order_key: "*float64",
  count_of_chosen_firings: "*int",
  subject_node_key: "*string",
  object_node_key: "*string",
  subject_node_kind: "*string",
  object_thing_node_kind: "*string",
  object_node_kind: "*string",
  object_value_node_label: "*string",
  count_of_violations_as_first: "*int",
  count_of_violations_as_second: "*int",
  is_in_contradiction: "*bool",
};

/** Computes the IsLiteral calculated field.
 *  Order 1. True when the object is a value rather than a thing.
 *  Formula: ={{ObjectThing}} = "" */
export function calcStatementsIsLiteral(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbIsBlank(vStr(tc.object_thing)));
}

/** Computes the ObjectText calculated field.
 *  Order 2. The object's name or the value.
 *  Formula: =IF({{ObjectThing}} = "", {{ObjectValue}}, {{ObjectLabel}}) */
export function calcStatementsObjectText(tc: StatementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbIsBlank(vStr(tc.object_thing))), () => vStr(tc.object_value), () => vStr(tc.object_label)));
}

/** Computes the Name calculated field.
 *  Order 3. The fact in words.
 *  Formula: =CONCAT({{SubjectLabel}}, " ", {{PropertyLabel}}, " ", {{ObjectText}}) */
export function calcStatementsName(tc: StatementsRow): string | null {
  return toStringPtr(erbConcat(erbPart(vStr(tc.subject_label)), vS(" "), erbPart(vStr(tc.property_label)), vS(" "), erbPart(vStr(tc.object_text))));
}

/** Computes the Words calculated field.
 *  Order 2. The fact as a conclusion card quotes it, with a literal written in Turtle form.
 *  Formula: =CONCAT({{SubjectLabel}}, " ", {{PropertyLabel}}, " ", IF({{ObjectThing}} = "", CONCAT({{LiteralQuote}}, {{ObjectValue}}, {{LiteralQuote}}, {{LiteralTypeMarker}}, {{PropertyDatatype}}), {{ObjectLabel}})) */
export function calcStatementsWords(tc: StatementsRow): string | null {
  return toStringPtr(erbConcat(erbPart(vStr(tc.subject_label)), vS(" "), erbPart(vStr(tc.property_label)), vS(" "), erbPart(erbIf(erbBool3(erbIsBlank(vStr(tc.object_thing))), () => erbConcat(erbPart(vStr(tc.literal_quote)), erbPart(vStr(tc.object_value)), erbPart(vStr(tc.literal_quote)), erbPart(vStr(tc.literal_type_marker)), erbPart(vStr(tc.property_datatype))), () => vStr(tc.object_label)))));
}

/** Computes the Source calculated field.
 *  Order 2. Where a listed fact came from.
 *  Formula: =IF({{IsBuiltIn}}, {{BuiltInSourceText}}, {{UserSourceText}}) */
export function calcStatementsSource(tc: StatementsRow): string | null {
  return toStringPtr(erbIf(erbIsTrue(vBoolPlain(tc.is_built_in)), () => vStr(tc.built_in_source_text), () => vStr(tc.user_source_text)));
}

/** Computes the SubjectPropertyKey calculated field.
 *  Order 1. Groups the values one subject has for one property.
 *  Formula: =CONCAT({{Subject}}, "|", {{Property}}) */
export function calcStatementsSubjectPropertyKey(tc: StatementsRow): string | null {
  return toStringPtr(erbConcat(erbPart(vStrPlain(tc.subject)), vS("|"), erbPart(vStrPlain(tc.property))));
}

/** Computes the IsInEffect calculated field.
 *  Order 3. Switched on and, for a one-value property, the most recently switched-on value. Drawn as a solid line.
 *  Formula: =AND({{IsStated}}, OR(NOT({{PropertyHasOneCurrentValue}}), {{ActivationSeq}} = {{LatestSiblingActivationSeq}})) */
export function calcStatementsIsInEffect(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbIsTrue(vBoolPlain(tc.is_stated)), erbBool3(erbOr(erbBool3(erbNot(erbBool3(vBool(tc.property_has_one_current_value)))), erbBool3(erbEq(erbNullif(vIntPlain(tc.activation_seq)), vInt(tc.latest_sibling_activation_seq)))))));
}

/** Computes the ListedUnit calculated field.
 *  Order 1. 1 when the fact is in the fact list.
 *  Formula: =IF({{IsListed}}, 1, 0) */
export function calcStatementsListedUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbIsTrue(vBoolPlain(tc.is_listed)), () => vI(1), () => vI(0))));
}

/** Computes the InEffectUnit calculated field.
 *  Order 4. 1 when the fact is in effect.
 *  Formula: =IF({{IsInEffect}}, 1, 0) */
export function calcStatementsInEffectUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_in_effect)), () => vI(1), () => vI(0))));
}

/** Computes the ListedOffUnit calculated field.
 *  Order 4. 1 when the fact is listed but not in effect.
 *  Formula: =IF(AND({{IsListed}}, NOT({{IsInEffect}})), 1, 0) */
export function calcStatementsListedOffUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbIsTrue(vBoolPlain(tc.is_listed)), erbBool3(erbNot(erbBool3(vBool(tc.is_in_effect)))))), () => vI(1), () => vI(0))));
}

/** Computes the IsNextToAdd calculated field.
 *  Order 7. The fact the Add next fact button switches on.
 *  Formula: =AND({{ListedOffUnit}} = 1, {{SortOrder}} = {{NextFactSortOrder}}) */
export function calcStatementsIsNextToAdd(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(vInt(tc.listed_off_unit), vI(1))), erbBool3(erbEq(erbNullif(vIntPlain(tc.sort_order)), vInt(tc.next_fact_sort_order)))));
}

/** Computes the HoldsBeforeChain calculated field.
 *  Order 7. TRUE when the fact is in effect or the inverse or symmetric rule concludes it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true).
 *  Formula: =IF(OR({{IsInEffect}}, {{CountOfInverseFirings}} > 0, {{CountOfSymmetricFirings}} > 0), TRUE()) */
export function calcStatementsHoldsBeforeChain(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbOr(erbBool3(vBool(tc.is_in_effect)), erbBool3(erbCmp(vInt(tc.count_of_inverse_firings), ">", vI(0))), erbBool3(erbCmp(vInt(tc.count_of_symmetric_firings), ">", vI(0))))), () => vB(true), () => Null));
}

/** Computes the HoldsBeforeTyping calculated field.
 *  Order 11. TRUE when the fact holds before the domain and range rules apply. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true).
 *  Formula: =IF(OR({{HoldsBeforeChain}}, {{CountOfChainFirings}} > 0), TRUE()) */
export function calcStatementsHoldsBeforeTyping(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbOr(erbBool3(vBool(tc.holds_before_chain)), erbBool3(erbCmp(vInt(tc.count_of_chain_firings), ">", vI(0))))), () => vB(true), () => Null));
}

/** Computes the Holds calculated field.
 *  Order 15. TRUE when in effect or concluded by an enabled rule; the graph draws it. Blank when nothing states or concludes it: not known, never FALSE (open world: removing a statement does not say that its opposite is true).
 *  Formula: =IF(OR({{HoldsBeforeTyping}}, {{CountOfTypeFirings}} > 0), TRUE()) */
export function calcStatementsHolds(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbOr(erbBool3(vBool(tc.holds_before_typing)), erbBool3(erbCmp(vInt(tc.count_of_type_firings), ">", vI(0))))), () => vB(true), () => Null));
}

/** Computes the IsConclusion calculated field.
 *  Order 16. TRUE when the fact holds only because a rule concluded it; drawn dashed. Blank otherwise: a stated fact is not a conclusion, and a fact nothing concludes is not known (never FALSE).
 *  Formula: =IF(AND({{Holds}}, NOT({{IsInEffect}})), TRUE()) */
export function calcStatementsIsConclusion(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.holds)), erbBool3(erbNot(erbBool3(vBool(tc.is_in_effect)))))), () => vB(true), () => Null));
}

/** Computes the HoldsUnit calculated field.
 *  Order 16. 1 when the fact holds.
 *  Formula: =IF({{Holds}}, 1, 0) */
export function calcStatementsHoldsUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.holds)), () => vI(1), () => vI(0))));
}

/** Computes the ConclusionUnit calculated field.
 *  Order 17. 1 when the fact is a conclusion.
 *  Formula: =IF({{IsConclusion}}, 1, 0) */
export function calcStatementsConclusionUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_conclusion)), () => vI(1), () => vI(0))));
}

/** Computes the ParentUnit calculated field.
 *  Order 16. 1 for a holding parent link.
 *  Formula: =IF(AND({{Holds}}, {{Property}} = "isParentOf"), 1, 0) */
export function calcStatementsParentUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.holds)), erbBool3(erbEq(erbNullif(vStrPlain(tc.property)), vS("isParentOf"))))), () => vI(1), () => vI(0))));
}

/** Computes the GrandparentUnit calculated field.
 *  Order 16. 1 for a holding grandparent link.
 *  Formula: =IF(AND({{Holds}}, {{Property}} = "isGrandparentOf"), 1, 0) */
export function calcStatementsGrandparentUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.holds)), erbBool3(erbEq(erbNullif(vStrPlain(tc.property)), vS("isGrandparentOf"))))), () => vI(1), () => vI(0))));
}

/** Computes the PlaceUnit calculated field.
 *  Order 16. 1 for a holding attends link.
 *  Formula: =IF(AND({{Holds}}, {{Property}} = "attends"), 1, 0) */
export function calcStatementsPlaceUnit(tc: StatementsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.holds)), erbBool3(erbEq(erbNullif(vStrPlain(tc.property)), vS("attends"))))), () => vI(1), () => vI(0))));
}

/** Computes the SnapPosition calculated field.
 *  Order 8. Where the page's reasoner meets this fact as a premise: a stated fact at its list position, a fact concluded by the inverse or symmetric rule after every stated fact, in the order it was concluded; 0 for a fact that does not hold yet, so a link to no fact reads the same on every engine.
 *  Formula: =IF({{IsInEffect}}, {{SortOrder}}, IF({{HoldsBeforeChain}}, 100000 + {{FirstPreChainOrderKey}}, 0)) */
export function calcStatementsSnapPosition(tc: StatementsRow): number | null {
  return toFloatPtr(erbIf(erbBool3(vBool(tc.is_in_effect)), () => vIntPlain(tc.sort_order), () => erbIf(erbBool3(vBool(tc.holds_before_chain)), () => erbAdd(vI(100000), vNum(tc.first_pre_chain_order_key)), () => vI(0))));
}

/** Computes the SubjectNodeKey calculated field.
 *  Order 1. The graph node this fact starts from.
 *  Formula: ={{Subject}} */
export function calcStatementsSubjectNodeKey(tc: StatementsRow): string | null {
  return toStringPtr(vStrPlain(tc.subject));
}

/** Computes the ObjectNodeKey calculated field.
 *  Order 2. The graph node this fact points at: the thing, or one node per value and datatype.
 *  Formula: =IF({{ObjectThing}} = "", CONCAT("literal|", {{ObjectValue}}, "|", {{PropertyDatatype}}), {{ObjectThing}}) */
export function calcStatementsObjectNodeKey(tc: StatementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbIsBlank(vStr(tc.object_thing))), () => erbConcat(vS("literal|"), erbPart(vStr(tc.object_value)), vS("|"), erbPart(vStr(tc.property_datatype))), () => vStr(tc.object_thing)));
}

/** Computes the ObjectNodeKind calculated field.
 *  Order 20. How the graph draws the object: a value, a class, a place or a person.
 *  Formula: =IF({{ObjectThing}} = "", "value", {{ObjectThingNodeKind}}) */
export function calcStatementsObjectNodeKind(tc: StatementsRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbIsBlank(vStr(tc.object_thing))), () => vS("value"), () => vStr(tc.object_thing_node_kind)));
}

/** Computes the IsInContradiction calculated field.
 *  Order 19. Part of a contradiction the disjoint rule found; drawn in the alarm colour.
 *  Formula: =OR({{CountOfViolationsAsFirst}} > 0, {{CountOfViolationsAsSecond}} > 0) */
export function calcStatementsIsInContradiction(tc: StatementsRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbCmp(vInt(tc.count_of_violations_as_first), ">", vI(0))), erbBool3(erbCmp(vInt(tc.count_of_violations_as_second), ">", vI(0)))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeStatements(tc: StatementsRow): StatementsRow {
  // Level 1
  calcGuard(tc, statementsFieldTypes, "is_literal", () => { tc.is_literal = calcStatementsIsLiteral(tc); });
  calcGuard(tc, statementsFieldTypes, "object_text", () => { tc.object_text = calcStatementsObjectText(tc); });
  calcGuard(tc, statementsFieldTypes, "words", () => { tc.words = calcStatementsWords(tc); });
  calcGuard(tc, statementsFieldTypes, "source", () => { tc.source = calcStatementsSource(tc); });
  calcGuard(tc, statementsFieldTypes, "subject_property_key", () => { tc.subject_property_key = calcStatementsSubjectPropertyKey(tc); });
  calcGuard(tc, statementsFieldTypes, "is_in_effect", () => { tc.is_in_effect = calcStatementsIsInEffect(tc); });
  calcGuard(tc, statementsFieldTypes, "listed_unit", () => { tc.listed_unit = calcStatementsListedUnit(tc); });
  calcGuard(tc, statementsFieldTypes, "subject_node_key", () => { tc.subject_node_key = calcStatementsSubjectNodeKey(tc); });
  calcGuard(tc, statementsFieldTypes, "object_node_key", () => { tc.object_node_key = calcStatementsObjectNodeKey(tc); });
  calcGuard(tc, statementsFieldTypes, "object_node_kind", () => { tc.object_node_kind = calcStatementsObjectNodeKind(tc); });
  calcGuard(tc, statementsFieldTypes, "is_in_contradiction", () => { tc.is_in_contradiction = calcStatementsIsInContradiction(tc); });
  // Level 2
  calcGuard(tc, statementsFieldTypes, "name", () => { tc.name = calcStatementsName(tc); });
  calcGuard(tc, statementsFieldTypes, "in_effect_unit", () => { tc.in_effect_unit = calcStatementsInEffectUnit(tc); });
  calcGuard(tc, statementsFieldTypes, "listed_off_unit", () => { tc.listed_off_unit = calcStatementsListedOffUnit(tc); });
  calcGuard(tc, statementsFieldTypes, "holds_before_chain", () => { tc.holds_before_chain = calcStatementsHoldsBeforeChain(tc); });
  // Level 3
  calcGuard(tc, statementsFieldTypes, "is_next_to_add", () => { tc.is_next_to_add = calcStatementsIsNextToAdd(tc); });
  calcGuard(tc, statementsFieldTypes, "holds_before_typing", () => { tc.holds_before_typing = calcStatementsHoldsBeforeTyping(tc); });
  calcGuard(tc, statementsFieldTypes, "snap_position", () => { tc.snap_position = calcStatementsSnapPosition(tc); });
  // Level 4
  calcGuard(tc, statementsFieldTypes, "holds", () => { tc.holds = calcStatementsHolds(tc); });
  // Level 5
  calcGuard(tc, statementsFieldTypes, "is_conclusion", () => { tc.is_conclusion = calcStatementsIsConclusion(tc); });
  calcGuard(tc, statementsFieldTypes, "holds_unit", () => { tc.holds_unit = calcStatementsHoldsUnit(tc); });
  calcGuard(tc, statementsFieldTypes, "parent_unit", () => { tc.parent_unit = calcStatementsParentUnit(tc); });
  calcGuard(tc, statementsFieldTypes, "grandparent_unit", () => { tc.grandparent_unit = calcStatementsGrandparentUnit(tc); });
  calcGuard(tc, statementsFieldTypes, "place_unit", () => { tc.place_unit = calcStatementsPlaceUnit(tc); });
  // Level 6
  calcGuard(tc, statementsFieldTypes, "conclusion_unit", () => { tc.conclusion_unit = calcStatementsConclusionUnit(tc); });
  return tc;
}

/** Reads Statements rows from a JSON array file. */
export function loadStatementsRows(file: string): StatementsRow[] {
  return loadRows(file, { fields: statementsFieldTypes }) as unknown as StatementsRow[];
}

// =============================================================================
// FIRINGS TABLE
// Every way an enabled rule could conclude a candidate fact from the facts that hold.
// =============================================================================

/** A row in the Firings table. */
export interface FiringsRow {
  /** Key: kind:premise(+premise)>conclusion. */
  firing_id: string;
  /** How the rule applies. */
  kind: string;
  /** The rule switch. */
  rule: string;
  /** The fact this application concludes. */
  target: string;
  /** The premise. */
  first: string;
  /** The second premise, for the property chain. */
  second: string | null;
  /** Order 1. Whether the rule is switched on. */
  rule_enabled: boolean | null;
  /** Order 1. How the card names the rule. */
  kind_wording: string | null;
  /** Order 1. The kind's place in the reasoner's order. */
  kind_sort_order: number | null;
  /** Order 4. The concluded fact in words. */
  target_name: string | null;
  /** Order 2. The concluded fact's subject. */
  target_subject_label: string | null;
  /** Order 2. The concluded fact's property. */
  target_property_label: string | null;
  /** Order 3. The concluded fact's object. */
  target_object_text: string | null;
  /** Order 17. Whether the concluded fact is new. */
  target_is_conclusion: boolean | null;
  /** Order 2. The concluded fact's subject node. */
  target_subject_node_key: string | null;
  /** Order 3. The concluded fact's object node. */
  target_object_node_key: string | null;
  /** Order 16. The order key of the concluded fact's first application. */
  target_first_fired_order_key: number | null;
  /** Order 3. The premise as the card quotes it. */
  first_words: string | null;
  /** Order 3. The second premise as the card quotes it. */
  second_words: string | null;
  /** Order 4. Whether the premise is in effect. */
  first_is_in_effect: boolean | null;
  /** Order 8. Whether the premise holds before the chain. */
  first_holds_before_chain: boolean | null;
  /** Order 8. Whether the second premise holds before the chain. */
  second_holds_before_chain: boolean | null;
  /** Order 12. Whether the premise holds before typing. */
  first_holds_before_typing: boolean | null;
  /** Order 1. The premise's property key, as the card's axiom names it. */
  first_property: string | null;
  /** Order 2. The premise property's domain class. */
  first_property_domain_class: string | null;
  /** Order 2. The premise property's range class. */
  first_property_range_class: string | null;
  /** Order 1. The premise's list position. */
  first_sort_order: number | null;
  /** Order 4. Whether the second premise is in effect. */
  second_is_in_effect: boolean | null;
  /** Order 9. Where the reasoner meets the premise. */
  first_snap_position: number | null;
  /** Order 9. Where the reasoner meets the second premise. */
  second_snap_position: number | null;
  /** Order 5. 1 when the inverse rule fires here. */
  inverse_fired_unit: number | null;
  /** Order 5. 1 when the symmetric rule fires here. */
  symmetric_fired_unit: number | null;
  /** Order 9. 1 when the property chain fires here. */
  chain_fired_unit: number | null;
  /** Order 13. 1 when the domain or range rule fires here. */
  type_fired_unit: number | null;
  /** Order 14. 1 when this application fires. */
  fired_unit: number | null;
  /** Order 6. 1 when this is an inverse or symmetric application that fires. */
  pre_chain_fired_unit: number | null;
  /** Order 2. For an inverse or symmetric application: its place in the reasoner's first round. */
  pre_chain_order_key: number | null;
  /** Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact. */
  round: number | null;
  /** Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise. */
  order_key: number | null;
  /** Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order. */
  is_chosen: boolean | null;
  /** Order 19. 1 when this application is shown. */
  chosen_unit: number | null;
  /** Order 19. The conclusion card's provenance line, in the page's wording. */
  card_text: string | null;
  /** Order 1. The application's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const firingsFieldTypes: Record<string, FieldType> = {
  firing_id: "string",
  kind: "string",
  rule: "string",
  target: "string",
  first: "string",
  second: "*string",
  rule_enabled: "*bool",
  kind_wording: "*string",
  kind_sort_order: "*int",
  target_name: "*string",
  target_subject_label: "*string",
  target_property_label: "*string",
  target_object_text: "*string",
  target_is_conclusion: "*bool",
  target_subject_node_key: "*string",
  target_object_node_key: "*string",
  target_first_fired_order_key: "*float64",
  first_words: "*string",
  second_words: "*string",
  first_is_in_effect: "*bool",
  first_holds_before_chain: "*bool",
  second_holds_before_chain: "*bool",
  first_holds_before_typing: "*bool",
  first_property: "*string",
  first_property_domain_class: "*string",
  first_property_range_class: "*string",
  first_sort_order: "*int",
  second_is_in_effect: "*bool",
  first_snap_position: "*float64",
  second_snap_position: "*float64",
  inverse_fired_unit: "*int",
  symmetric_fired_unit: "*int",
  chain_fired_unit: "*int",
  type_fired_unit: "*int",
  fired_unit: "*int",
  pre_chain_fired_unit: "*int",
  pre_chain_order_key: "*float64",
  round: "*int",
  order_key: "*float64",
  is_chosen: "*bool",
  chosen_unit: "*int",
  card_text: "*string",
  name: "*string",
};

/** Computes the InverseFiredUnit calculated field.
 *  Order 5. 1 when the inverse rule fires here.
 *  Formula: =IF(AND({{RuleEnabled}}, {{Kind}} = "inverse", {{FirstIsInEffect}}), 1, 0) */
export function calcFiringsInverseFiredUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.rule_enabled)), erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("inverse"))), erbBool3(vBool(tc.first_is_in_effect)))), () => vI(1), () => vI(0))));
}

/** Computes the SymmetricFiredUnit calculated field.
 *  Order 5. 1 when the symmetric rule fires here.
 *  Formula: =IF(AND({{RuleEnabled}}, {{Kind}} = "symmetric", {{FirstIsInEffect}}), 1, 0) */
export function calcFiringsSymmetricFiredUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.rule_enabled)), erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("symmetric"))), erbBool3(vBool(tc.first_is_in_effect)))), () => vI(1), () => vI(0))));
}

/** Computes the ChainFiredUnit calculated field.
 *  Order 9. 1 when the property chain fires here.
 *  Formula: =IF(AND({{RuleEnabled}}, {{Kind}} = "chain", {{FirstHoldsBeforeChain}}, {{SecondHoldsBeforeChain}}), 1, 0) */
export function calcFiringsChainFiredUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.rule_enabled)), erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("chain"))), erbBool3(vBool(tc.first_holds_before_chain)), erbBool3(vBool(tc.second_holds_before_chain)))), () => vI(1), () => vI(0))));
}

/** Computes the TypeFiredUnit calculated field.
 *  Order 13. 1 when the domain or range rule fires here.
 *  Formula: =IF(AND({{RuleEnabled}}, OR({{Kind}} = "domain", {{Kind}} = "range"), {{FirstHoldsBeforeTyping}}), 1, 0) */
export function calcFiringsTypeFiredUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.rule_enabled)), erbBool3(erbOr(erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("domain"))), erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("range"))))), erbBool3(vBool(tc.first_holds_before_typing)))), () => vI(1), () => vI(0))));
}

/** Computes the FiredUnit calculated field.
 *  Order 14. 1 when this application fires.
 *  Formula: ={{InverseFiredUnit}} + {{SymmetricFiredUnit}} + {{ChainFiredUnit}} + {{TypeFiredUnit}} */
export function calcFiringsFiredUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(erbAdd(erbAdd(vInt(tc.inverse_fired_unit), vInt(tc.symmetric_fired_unit)), vInt(tc.chain_fired_unit)), vInt(tc.type_fired_unit))));
}

/** Computes the PreChainFiredUnit calculated field.
 *  Order 6. 1 when this is an inverse or symmetric application that fires.
 *  Formula: ={{InverseFiredUnit}} + {{SymmetricFiredUnit}} */
export function calcFiringsPreChainFiredUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbAdd(vInt(tc.inverse_fired_unit), vInt(tc.symmetric_fired_unit))));
}

/** Computes the PreChainOrderKey calculated field.
 *  Order 2. For an inverse or symmetric application: its place in the reasoner's first round.
 *  Formula: =({{FirstSortOrder}} * 10) + {{KindSortOrder}} */
export function calcFiringsPreChainOrderKey(tc: FiringsRow): number | null {
  return toFloatPtr(erbAdd(erbMul(vInt(tc.first_sort_order), vI(10)), vInt(tc.kind_sort_order)));
}

/** Computes the Round calculated field.
 *  Order 5. The reasoner's round: 1 when every premise is a stated fact, 2 when the property chain reads a concluded fact.
 *  Formula: =IF(AND({{Kind}} = "chain", NOT(AND({{FirstIsInEffect}}, {{SecondIsInEffect}}))), 2, 1) */
export function calcFiringsRound(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("chain"))), erbBool3(erbNot(erbBool3(erbAnd(erbBool3(vBool(tc.first_is_in_effect)), erbBool3(vBool(tc.second_is_in_effect)))))))), () => vI(2), () => vI(1))));
}

/** Computes the OrderKey calculated field.
 *  Order 10. The page reasoner's order: by round, then premise, then rule kind, then second premise.
 *  Formula: =({{Round}} * 1000000000000000) + ({{FirstSnapPosition}} * 100000000) + ({{KindSortOrder}} * 10000000) + IF({{Second}} = "", 0, {{SecondSnapPosition}}) */
export function calcFiringsOrderKey(tc: FiringsRow): number | null {
  return toFloatPtr(erbAdd(erbAdd(erbAdd(erbMul(vInt(tc.round), vI(1000000000000000)), erbMul(vNum(tc.first_snap_position), vI(100000000))), erbMul(vInt(tc.kind_sort_order), vI(10000000))), erbIf(erbBool3(erbIsBlank(vStr(tc.second))), () => vI(0), () => vNum(tc.second_snap_position))));
}

/** Computes the IsChosen calculated field.
 *  Order 18. The application the conclusions panel shows for its fact: the first one in the reasoner's order.
 *  Formula: =AND({{FiredUnit}} = 1, {{TargetIsConclusion}}, {{OrderKey}} = {{TargetFirstFiredOrderKey}}) */
export function calcFiringsIsChosen(tc: FiringsRow): boolean | null {
  return toBoolPtr(erbAnd(erbBool3(erbEq(vInt(tc.fired_unit), vI(1))), erbBool3(vBool(tc.target_is_conclusion)), erbBool3(erbEq(vNum(tc.order_key), vNum(tc.target_first_fired_order_key)))));
}

/** Computes the ChosenUnit calculated field.
 *  Order 19. 1 when this application is shown.
 *  Formula: =IF({{IsChosen}}, 1, 0) */
export function calcFiringsChosenUnit(tc: FiringsRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(vBool(tc.is_chosen)), () => vI(1), () => vI(0))));
}

/** Computes the CardText calculated field.
 *  Order 19. The conclusion card's provenance line, in the page's wording.
 *  Formula: =IF({{IsChosen}}, CONCAT("Worked out by the ", {{KindWording}}, " from “", {{FirstWords}}, "”", IF({{Kind}} = "chain", CONCAT(" and “", {{SecondWords}}, "”"), IF({{Kind}} = "domain", CONCAT(" and “", {{FirstProperty}}, " has domain ", {{FirstPropertyDomainClass}}, "”"), IF({{Kind}} = "range", CONCAT(" and “", {{FirstProperty}}, " has range ", {{FirstPropertyRangeClass}}, "”"), ""))), "."), "") */
export function calcFiringsCardText(tc: FiringsRow): string | null {
  return toStringPtr(erbIf(erbBool3(vBool(tc.is_chosen)), () => erbConcat(vS("Worked out by the "), erbPart(vStr(tc.kind_wording)), vS(" from “"), erbPart(vStr(tc.first_words)), vS("”"), erbPart(erbIf(erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("chain"))), () => erbConcat(vS(" and “"), erbPart(vStr(tc.second_words)), vS("”")), () => erbIf(erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("domain"))), () => erbConcat(vS(" and “"), erbPart(vStr(tc.first_property)), vS(" has domain "), erbPart(vStr(tc.first_property_domain_class)), vS("”")), () => erbIf(erbBool3(erbEq(erbNullif(vStrPlain(tc.kind)), vS("range"))), () => erbConcat(vS(" and “"), erbPart(vStr(tc.first_property)), vS(" has range "), erbPart(vStr(tc.first_property_range_class)), vS("”")), () => vS(""))))), vS(".")), () => vS("")));
}

/** Computes the Name calculated field.
 *  Order 1. The application's key.
 *  Formula: ={{FiringId}} */
export function calcFiringsName(tc: FiringsRow): string | null {
  return toStringPtr(vStrPlain(tc.firing_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computeFirings(tc: FiringsRow): FiringsRow {
  // Level 1
  calcGuard(tc, firingsFieldTypes, "inverse_fired_unit", () => { tc.inverse_fired_unit = calcFiringsInverseFiredUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "symmetric_fired_unit", () => { tc.symmetric_fired_unit = calcFiringsSymmetricFiredUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "chain_fired_unit", () => { tc.chain_fired_unit = calcFiringsChainFiredUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "type_fired_unit", () => { tc.type_fired_unit = calcFiringsTypeFiredUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "pre_chain_order_key", () => { tc.pre_chain_order_key = calcFiringsPreChainOrderKey(tc); });
  calcGuard(tc, firingsFieldTypes, "round", () => { tc.round = calcFiringsRound(tc); });
  calcGuard(tc, firingsFieldTypes, "name", () => { tc.name = calcFiringsName(tc); });
  // Level 2
  calcGuard(tc, firingsFieldTypes, "fired_unit", () => { tc.fired_unit = calcFiringsFiredUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "pre_chain_fired_unit", () => { tc.pre_chain_fired_unit = calcFiringsPreChainFiredUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "order_key", () => { tc.order_key = calcFiringsOrderKey(tc); });
  // Level 3
  calcGuard(tc, firingsFieldTypes, "is_chosen", () => { tc.is_chosen = calcFiringsIsChosen(tc); });
  // Level 4
  calcGuard(tc, firingsFieldTypes, "chosen_unit", () => { tc.chosen_unit = calcFiringsChosenUnit(tc); });
  calcGuard(tc, firingsFieldTypes, "card_text", () => { tc.card_text = calcFiringsCardText(tc); });
  return tc;
}

/** Reads Firings rows from a JSON array file. */
export function loadFiringsRows(file: string): FiringsRow[] {
  return loadRows(file, { fields: firingsFieldTypes }) as unknown as FiringsRow[];
}

// =============================================================================
// DISJOINTCHECKS TABLE
// Every pair the disjointness rule watches: the same two people linked by two disjoint properties.
// =============================================================================

/** A row in the DisjointChecks table. */
export interface DisjointChecksRow {
  /** Key: first fact~second property. */
  disjoint_check_id: string;
  /** The graph. */
  graph: string;
  /** The rule switch. */
  rule: string;
  /** The first link. */
  first: string;
  /** The disjoint link between the same pair, in the same direction. */
  second: string;
  /** Order 1. Whether the disjointness rule is on. */
  rule_enabled: boolean | null;
  /** Order 16. Whether the first link holds. */
  first_holds: boolean | null;
  /** Order 16. Whether the second link holds. */
  second_holds: boolean | null;
  /** Order 2. The person recorded with both links. */
  subject_label: string | null;
  /** Order 2. The other person. */
  object_label: string | null;
  /** Order 17. 1 when both links hold. */
  violation_unit: number | null;
  /** Order 18. The page's contradiction alert, blank when there is none. */
  alarm_text: string | null;
  /** Order 1. The check's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const disjointChecksFieldTypes: Record<string, FieldType> = {
  disjoint_check_id: "string",
  graph: "string",
  rule: "string",
  first: "string",
  second: "string",
  rule_enabled: "*bool",
  first_holds: "*bool",
  second_holds: "*bool",
  subject_label: "*string",
  object_label: "*string",
  violation_unit: "*int",
  alarm_text: "*string",
  name: "*string",
};

/** Computes the ViolationUnit calculated field.
 *  Order 17. 1 when both links hold.
 *  Formula: =IF(AND({{RuleEnabled}}, {{FirstHolds}}, {{SecondHolds}}), 1, 0) */
export function calcDisjointChecksViolationUnit(tc: DisjointChecksRow): number | null {
  return toIntPtr(erbInteger(erbIf(erbBool3(erbAnd(erbBool3(vBool(tc.rule_enabled)), erbBool3(vBool(tc.first_holds)), erbBool3(vBool(tc.second_holds)))), () => vI(1), () => vI(0))));
}

/** Computes the AlarmText calculated field.
 *  Order 18. The page's contradiction alert, blank when there is none.
 *  Formula: =IF({{ViolationUnit}} = 1, CONCAT("Contradiction found. ", {{SubjectLabel}}, " is recorded as both parent and spouse of ", {{ObjectLabel}}, ". Check those facts and the selected disjoint rule."), "") */
export function calcDisjointChecksAlarmText(tc: DisjointChecksRow): string | null {
  return toStringPtr(erbIf(erbBool3(erbEq(vInt(tc.violation_unit), vI(1))), () => erbConcat(vS("Contradiction found. "), erbPart(vStr(tc.subject_label)), vS(" is recorded as both parent and spouse of "), erbPart(vStr(tc.object_label)), vS(". Check those facts and the selected disjoint rule.")), () => vS("")));
}

/** Computes the Name calculated field.
 *  Order 1. The check's key.
 *  Formula: ={{DisjointCheckId}} */
export function calcDisjointChecksName(tc: DisjointChecksRow): string | null {
  return toStringPtr(vStrPlain(tc.disjoint_check_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computeDisjointChecks(tc: DisjointChecksRow): DisjointChecksRow {
  // Level 1
  calcGuard(tc, disjointChecksFieldTypes, "violation_unit", () => { tc.violation_unit = calcDisjointChecksViolationUnit(tc); });
  calcGuard(tc, disjointChecksFieldTypes, "name", () => { tc.name = calcDisjointChecksName(tc); });
  // Level 2
  calcGuard(tc, disjointChecksFieldTypes, "alarm_text", () => { tc.alarm_text = calcDisjointChecksAlarmText(tc); });
  return tc;
}

/** Reads DisjointChecks rows from a JSON array file. */
export function loadDisjointChecksRows(file: string): DisjointChecksRow[] {
  return loadRows(file, { fields: disjointChecksFieldTypes }) as unknown as DisjointChecksRow[];
}

// =============================================================================
// SCENARIOS TABLE
// The page's buttons. Each one is a fact baseline, explicit fact changes and rule settings, applied in that order.
// =============================================================================

/** A row in the Scenarios table. */
export interface ScenariosRow {
  /** Key of the button. */
  scenario_id: string;
  /** The graph. */
  graph: string;
  /** The button's text. */
  label: string;
  /** The note shown under the buttons after this one is pressed; blank clears it. */
  note: string | null;
  /** Position among the buttons. */
  sort_order: number;
  /** 'stan' for the two imagined changes, 'facts' for the buttons under the fact list. */
  button_group: string;
  /** Drawn as the filled button. */
  is_primary: boolean;
  /** What happens to every listed fact first: keep, built-in (teaching facts on, others off), built-in-detail, all-off, activate-all (each switched on in list order), or next (the next fact on). */
  fact_baseline: string;
  /** The node layout the graph returns to; blank keeps the current positions. */
  layout: string | null;
  /** Clears the selected node or line. */
  clears_selection: boolean;
  /** Clears the Add fact form's message. */
  clears_form_message: boolean;
  /** Order 1. The button's text. */
  name: string | null;
  /** Order 7. Whether a listed fact can still be added. */
  graph_has_facts_to_add: boolean | null;
  /** Order 8. False only for Add next fact once every listed fact is in effect. */
  is_available: boolean | null;
  _erb_errors?: Record<string, string>;
}

const scenariosFieldTypes: Record<string, FieldType> = {
  scenario_id: "string",
  graph: "string",
  label: "string",
  note: "*string",
  sort_order: "int",
  button_group: "string",
  is_primary: "bool",
  fact_baseline: "string",
  layout: "*string",
  clears_selection: "bool",
  clears_form_message: "bool",
  name: "*string",
  graph_has_facts_to_add: "*bool",
  is_available: "*bool",
};

/** Computes the Name calculated field.
 *  Order 1. The button's text.
 *  Formula: ={{Label}} */
export function calcScenariosName(tc: ScenariosRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes the IsAvailable calculated field.
 *  Order 8. False only for Add next fact once every listed fact is in effect.
 *  Formula: =OR({{FactBaseline}} <> "next", {{GraphHasFactsToAdd}}) */
export function calcScenariosIsAvailable(tc: ScenariosRow): boolean | null {
  return toBoolPtr(erbOr(erbBool3(erbNe(erbNullif(vStrPlain(tc.fact_baseline)), vS("next"))), erbBool3(vBool(tc.graph_has_facts_to_add))));
}

/** Computes every calculated field of the row in dependency order. */
export function computeScenarios(tc: ScenariosRow): ScenariosRow {
  // Level 1
  calcGuard(tc, scenariosFieldTypes, "name", () => { tc.name = calcScenariosName(tc); });
  calcGuard(tc, scenariosFieldTypes, "is_available", () => { tc.is_available = calcScenariosIsAvailable(tc); });
  return tc;
}

/** Reads Scenarios rows from a JSON array file. */
export function loadScenariosRows(file: string): ScenariosRow[] {
  return loadRows(file, { fields: scenariosFieldTypes }) as unknown as ScenariosRow[];
}

// =============================================================================
// SCENARIOFACTCHANGES TABLE
// Fact changes a button makes after its baseline. A blank object key matches every listed value of the property.
// =============================================================================

/** A row in the ScenarioFactChanges table. */
export interface ScenarioFactChangesRow {
  /** Key of the change. */
  scenario_fact_change_id: string;
  /** The button. */
  scenario: string;
  /** The subject whose facts change. */
  subject: string;
  /** The property whose facts change. */
  property: string;
  /** The object thing's key or the literal value; blank for every listed value. */
  object_key: string | null;
  /** 'on' switches the fact on as the newest value; 'off' switches it off. */
  action: string;
  /** Adds the fact to the fact list first when it is not listed. */
  adds_to_list: boolean;
  /** Order of the change within the button. */
  sort_order: number;
  /** Order 1. The change's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const scenarioFactChangesFieldTypes: Record<string, FieldType> = {
  scenario_fact_change_id: "string",
  scenario: "string",
  subject: "string",
  property: "string",
  object_key: "*string",
  action: "string",
  adds_to_list: "bool",
  sort_order: "int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The change's key.
 *  Formula: ={{ScenarioFactChangeId}} */
export function calcScenarioFactChangesName(tc: ScenarioFactChangesRow): string | null {
  return toStringPtr(vStrPlain(tc.scenario_fact_change_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computeScenarioFactChanges(tc: ScenarioFactChangesRow): ScenarioFactChangesRow {
  // Level 1
  calcGuard(tc, scenarioFactChangesFieldTypes, "name", () => { tc.name = calcScenarioFactChangesName(tc); });
  return tc;
}

/** Reads ScenarioFactChanges rows from a JSON array file. */
export function loadScenarioFactChangesRows(file: string): ScenarioFactChangesRow[] {
  return loadRows(file, { fields: scenarioFactChangesFieldTypes }) as unknown as ScenarioFactChangesRow[];
}

// =============================================================================
// SCENARIORULESETTINGS TABLE
// Rule switches a button sets.
// =============================================================================

/** A row in the ScenarioRuleSettings table. */
export interface ScenarioRuleSettingsRow {
  /** Key of the setting. */
  scenario_rule_setting_id: string;
  /** The button. */
  scenario: string;
  /** The rule switch. */
  rule: string;
  /** The state the button puts the switch in. */
  is_enabled: boolean;
  /** Order 1. The setting's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const scenarioRuleSettingsFieldTypes: Record<string, FieldType> = {
  scenario_rule_setting_id: "string",
  scenario: "string",
  rule: "string",
  is_enabled: "bool",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The setting's key.
 *  Formula: ={{ScenarioRuleSettingId}} */
export function calcScenarioRuleSettingsName(tc: ScenarioRuleSettingsRow): string | null {
  return toStringPtr(vStrPlain(tc.scenario_rule_setting_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computeScenarioRuleSettings(tc: ScenarioRuleSettingsRow): ScenarioRuleSettingsRow {
  // Level 1
  calcGuard(tc, scenarioRuleSettingsFieldTypes, "name", () => { tc.name = calcScenarioRuleSettingsName(tc); });
  return tc;
}

/** Reads ScenarioRuleSettings rows from a JSON array file. */
export function loadScenarioRuleSettingsRows(file: string): ScenarioRuleSettingsRow[] {
  return loadRows(file, { fields: scenarioRuleSettingsFieldTypes }) as unknown as ScenarioRuleSettingsRow[];
}

// =============================================================================
// NODELAYOUTS TABLE
// Node positions in the graph's 380 by 480 drawing, one set per layout.
// =============================================================================

/** A row in the NodeLayouts table. */
export interface NodeLayoutsRow {
  /** Key: layout:node. */
  node_layout_id: string;
  /** The layout: 'default' or 'stan-details'. */
  layout: string;
  /** The node: a thing's key, or literal|value|datatype. */
  node_key: string;
  /** Horizontal position. */
  x: number;
  /** Vertical position. */
  y: number;
  /** Order 1. The position's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const nodeLayoutsFieldTypes: Record<string, FieldType> = {
  node_layout_id: "string",
  layout: "string",
  node_key: "string",
  x: "int",
  y: "int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The position's key.
 *  Formula: ={{NodeLayoutId}} */
export function calcNodeLayoutsName(tc: NodeLayoutsRow): string | null {
  return toStringPtr(vStrPlain(tc.node_layout_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computeNodeLayouts(tc: NodeLayoutsRow): NodeLayoutsRow {
  // Level 1
  calcGuard(tc, nodeLayoutsFieldTypes, "name", () => { tc.name = calcNodeLayoutsName(tc); });
  return tc;
}

/** Reads NodeLayouts rows from a JSON array file. */
export function loadNodeLayoutsRows(file: string): NodeLayoutsRow[] {
  return loadRows(file, { fields: nodeLayoutsFieldTypes }) as unknown as NodeLayoutsRow[];
}

// =============================================================================
// LAYOUTSLOTS TABLE
// Places for nodes no layout positions, used in turn.
// =============================================================================

/** A row in the LayoutSlots table. */
export interface LayoutSlotsRow {
  /** Key of the slot. */
  layout_slot_id: string;
  /** Order in which slots are used. */
  sort_order: number;
  /** Horizontal position. */
  x: number;
  /** Vertical position. */
  y: number;
  /** Order 1. The slot's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const layoutSlotsFieldTypes: Record<string, FieldType> = {
  layout_slot_id: "string",
  sort_order: "int",
  x: "int",
  y: "int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The slot's key.
 *  Formula: ={{LayoutSlotId}} */
export function calcLayoutSlotsName(tc: LayoutSlotsRow): string | null {
  return toStringPtr(vStrPlain(tc.layout_slot_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computeLayoutSlots(tc: LayoutSlotsRow): LayoutSlotsRow {
  // Level 1
  calcGuard(tc, layoutSlotsFieldTypes, "name", () => { tc.name = calcLayoutSlotsName(tc); });
  return tc;
}

/** Reads LayoutSlots rows from a JSON array file. */
export function loadLayoutSlotsRows(file: string): LayoutSlotsRow[] {
  return loadRows(file, { fields: layoutSlotsFieldTypes }) as unknown as LayoutSlotsRow[];
}

// =============================================================================
// PAGETEXTS TABLE
// The page's wording, one row per string. {name}-style placeholders are filled by the page.
// =============================================================================

/** A row in the PageTexts table. */
export interface PageTextsRow {
  /** Key of the string. */
  page_text_id: string;
  /** The string. */
  text: string;
  /** Order 1. The string's key. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const pageTextsFieldTypes: Record<string, FieldType> = {
  page_text_id: "string",
  text: "string",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The string's key.
 *  Formula: ={{PageTextId}} */
export function calcPageTextsName(tc: PageTextsRow): string | null {
  return toStringPtr(vStrPlain(tc.page_text_id));
}

/** Computes every calculated field of the row in dependency order. */
export function computePageTexts(tc: PageTextsRow): PageTextsRow {
  // Level 1
  calcGuard(tc, pageTextsFieldTypes, "name", () => { tc.name = calcPageTextsName(tc); });
  return tc;
}

/** Reads PageTexts rows from a JSON array file. */
export function loadPageTextsRows(file: string): PageTextsRow[] {
  return loadRows(file, { fields: pageTextsFieldTypes }) as unknown as PageTextsRow[];
}

// =============================================================================
// ENGINES TABLE
// The execution engines the page can be computed by. Each is a build output of this rulebook.
// =============================================================================

/** A row in the Engines table. */
export interface EnginesRow {
  /** Key of the engine; the page sends it with every request. */
  engine_id: string;
  /** The engine's name in the switcher. */
  label: string;
  /** The conformance harness's name for this substrate. */
  substrate: string;
  /** The engine a new visitor starts on. */
  is_default: boolean;
  /** What computes the page on this engine. */
  description: string;
  /** Position in the switcher. */
  sort_order: number;
  /** Order 1. The engine's name. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const enginesFieldTypes: Record<string, FieldType> = {
  engine_id: "string",
  label: "string",
  substrate: "string",
  is_default: "bool",
  description: "string",
  sort_order: "int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The engine's name.
 *  Formula: ={{Label}} */
export function calcEnginesName(tc: EnginesRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeEngines(tc: EnginesRow): EnginesRow {
  // Level 1
  calcGuard(tc, enginesFieldTypes, "name", () => { tc.name = calcEnginesName(tc); });
  return tc;
}

/** Reads Engines rows from a JSON array file. */
export function loadEnginesRows(file: string): EnginesRow[] {
  return loadRows(file, { fields: enginesFieldTypes }) as unknown as EnginesRow[];
}

// =============================================================================
// RULEBOOKTOOLS TABLE
// What each rulebook tool adds to the original explainer; the tools panel lists them.
// =============================================================================

/** A row in the RulebookTools table. */
export interface RulebookToolsRow {
  /** Key of the tool. */
  rulebook_tool_id: string;
  /** The tool's name. */
  label: string;
  /** What it adds to the original explainer. */
  adds: string;
  /** Where its link goes; 'editor' means the running rulebook editor. */
  href: string | null;
  /** The link's text. */
  link_text: string | null;
  /** Position in the panel. */
  sort_order: number;
  /** Order 1. The tool's name. */
  name: string | null;
  _erb_errors?: Record<string, string>;
}

const rulebookToolsFieldTypes: Record<string, FieldType> = {
  rulebook_tool_id: "string",
  label: "string",
  adds: "string",
  href: "*string",
  link_text: "*string",
  sort_order: "int",
  name: "*string",
};

/** Computes the Name calculated field.
 *  Order 1. The tool's name.
 *  Formula: ={{Label}} */
export function calcRulebookToolsName(tc: RulebookToolsRow): string | null {
  return toStringPtr(vStrPlain(tc.label));
}

/** Computes every calculated field of the row in dependency order. */
export function computeRulebookTools(tc: RulebookToolsRow): RulebookToolsRow {
  // Level 1
  calcGuard(tc, rulebookToolsFieldTypes, "name", () => { tc.name = calcRulebookToolsName(tc); });
  return tc;
}

/** Reads RulebookTools rows from a JSON array file. */
export function loadRulebookToolsRows(file: string): RulebookToolsRow[] {
  return loadRows(file, { fields: rulebookToolsFieldTypes }) as unknown as RulebookToolsRow[];
}

/** Bounds the runner's passes over the dataset. */
export const calculatedFieldCount = 68;

/** Every table, in rulebook order. */
export const erbTables: TableSpec[] = [
  { name: "__meta__", file: "__meta__", rulebookRows: 7, fields: __meta__FieldTypes,
    columns: { meta_key: "MetaKey", name: "Name", value_type: "ValueType", string_value: "StringValue", json_value: "JsonValue" },
    stored: ["meta_key", "value_type", "string_value", "json_value"],
    compute: (row: any) => compute__meta__(row as __meta__Row),
    lookups: [],
    aggregations: [] },
  { name: "Graphs", file: "graphs", rulebookRows: 1, fields: graphsFieldTypes,
    columns: { graph_id: "GraphId", label: "Label", literal_quote: "LiteralQuote", literal_type_marker: "LiteralTypeMarker", built_in_source_text: "BuiltInSourceText", user_source_text: "UserSourceText", no_conclusions_text: "NoConclusionsText", name: "Name", count_of_stated_facts: "CountOfStatedFacts", count_of_conclusions: "CountOfConclusions", count_of_violations: "CountOfViolations", is_consistent: "IsConsistent", conclusions_message: "ConclusionsMessage", count_of_facts_to_add: "CountOfFactsToAdd", has_facts_to_add: "HasFactsToAdd", next_fact_sort_order: "NextFactSortOrder", max_activation_seq: "MaxActivationSeq", next_activation_seq: "NextActivationSeq", max_listed_sort_order: "MaxListedSortOrder", next_listed_sort_order: "NextListedSortOrder", max_sort_order: "MaxSortOrder", next_candidate_sort_order: "NextCandidateSortOrder" },
    stored: ["graph_id", "label", "literal_quote", "literal_type_marker", "built_in_source_text", "user_source_text", "no_conclusions_text"],
    compute: (row: any) => computeGraphs(row as GraphsRow),
    lookups: [],
    aggregations: [
      { field: "count_of_stated_facts", op: "SUM", table: "statements", target: "in_effect_unit", criteria: [{ range: "graph", kind: "field", field: "graph_id" }] },
      { field: "count_of_conclusions", op: "SUM", table: "statements", target: "conclusion_unit", criteria: [{ range: "graph", kind: "field", field: "graph_id" }] },
      { field: "count_of_violations", op: "SUM", table: "disjoint_checks", target: "violation_unit", criteria: [{ range: "graph", kind: "field", field: "graph_id" }] },
      { field: "count_of_facts_to_add", op: "SUM", table: "statements", target: "listed_off_unit", criteria: [{ range: "graph", kind: "field", field: "graph_id" }] },
      { field: "next_fact_sort_order", op: "MIN", table: "statements", target: "sort_order", criteria: [{ range: "graph", kind: "field", field: "graph_id" }, { range: "listed_off_unit", kind: "literal", literal: vI(1) }] },
      { field: "max_activation_seq", op: "MAX", table: "statements", target: "activation_seq", criteria: [{ range: "graph", kind: "field", field: "graph_id" }] },
      { field: "max_listed_sort_order", op: "MAX", table: "statements", target: "sort_order", criteria: [{ range: "graph", kind: "field", field: "graph_id" }, { range: "listed_unit", kind: "literal", literal: vI(1) }] },
      { field: "max_sort_order", op: "MAX", table: "statements", target: "sort_order", criteria: [{ range: "graph", kind: "field", field: "graph_id" }] },] },
  { name: "Rules", file: "rules", rulebookRows: 5, fields: rulesFieldTypes,
    columns: { rule_id: "RuleId", label: "Label", explanation: "Explanation", is_enabled: "IsEnabled", sort_order: "SortOrder", name: "Name", count_of_firings: "CountOfFirings" },
    stored: ["rule_id", "label", "explanation", "is_enabled", "sort_order"],
    compute: (row: any) => computeRules(row as RulesRow),
    lookups: [],
    aggregations: [
      { field: "count_of_firings", op: "SUM", table: "firings", target: "fired_unit", criteria: [{ range: "rule", kind: "field", field: "rule_id" }] },] },
  { name: "FiringKinds", file: "firing_kinds", rulebookRows: 6, fields: firingKindsFieldTypes,
    columns: { firing_kind_id: "FiringKindId", rule: "Rule", wording: "Wording", sort_order: "SortOrder", name: "Name", rule_enabled: "RuleEnabled" },
    stored: ["firing_kind_id", "rule", "wording", "sort_order"],
    compute: (row: any) => computeFiringKinds(row as FiringKindsRow),
    lookups: [
      { field: "rule_enabled", target: "rules", ret: "is_enabled", key: "rule", match: "rule_id" },],
    aggregations: [] },
  { name: "Things", file: "things", rulebookRows: 9, fields: thingsFieldTypes,
    columns: { thing_id: "ThingId", label: "Label", kind: "Kind", sort_order: "SortOrder", portrait: "Portrait", portrait_name: "PortraitName", portrait_page: "PortraitPage", portrait_image: "PortraitImage", portrait_crop: "PortraitCrop", portrait_width: "PortraitWidth", portrait_height: "PortraitHeight", name: "Name", node_key: "NodeKey", count_of_children: "CountOfChildren", count_of_grandchildren: "CountOfGrandchildren", count_as_place: "CountAsPlace", count_as_subject: "CountAsSubject", count_as_object: "CountAsObject", is_in_graph: "IsInGraph", node_kind: "NodeKind" },
    stored: ["thing_id", "label", "kind", "sort_order", "portrait"],
    compute: (row: any) => computeThings(row as ThingsRow),
    lookups: [
      { field: "portrait_name", target: "portraits", ret: "display_name", key: "portrait", match: "portrait_id" },
      { field: "portrait_page", target: "portraits", ret: "page", key: "portrait", match: "portrait_id" },
      { field: "portrait_image", target: "portraits", ret: "image", key: "portrait", match: "portrait_id" },
      { field: "portrait_crop", target: "portraits", ret: "crop", key: "portrait", match: "portrait_id" },
      { field: "portrait_width", target: "portraits", ret: "width", key: "portrait", match: "portrait_id" },
      { field: "portrait_height", target: "portraits", ret: "height", key: "portrait", match: "portrait_id" },],
    aggregations: [
      { field: "count_of_children", op: "SUM", table: "statements", target: "parent_unit", criteria: [{ range: "subject", kind: "field", field: "thing_id" }] },
      { field: "count_of_grandchildren", op: "SUM", table: "statements", target: "grandparent_unit", criteria: [{ range: "subject", kind: "field", field: "thing_id" }] },
      { field: "count_as_place", op: "SUM", table: "statements", target: "place_unit", criteria: [{ range: "object_thing", kind: "field", field: "thing_id" }] },
      { field: "count_as_subject", op: "SUM", table: "statements", target: "holds_unit", criteria: [{ range: "subject", kind: "field", field: "thing_id" }] },
      { field: "count_as_object", op: "SUM", table: "statements", target: "holds_unit", criteria: [{ range: "object_thing", kind: "field", field: "thing_id" }] },] },
  { name: "Portraits", file: "portraits", rulebookRows: 13, fields: portraitsFieldTypes,
    columns: { portrait_id: "PortraitId", display_name: "DisplayName", page: "Page", image: "Image", crop: "Crop", width: "Width", height: "Height", sort_order: "SortOrder", name: "Name" },
    stored: ["portrait_id", "display_name", "page", "image", "crop", "width", "height", "sort_order"],
    compute: (row: any) => computePortraits(row as PortraitsRow),
    lookups: [],
    aggregations: [] },
  { name: "PortraitAliases", file: "portrait_aliases", rulebookRows: 32, fields: portraitAliasesFieldTypes,
    columns: { portrait_alias_id: "PortraitAliasId", portrait: "Portrait", name: "Name" },
    stored: ["portrait_alias_id", "portrait"],
    compute: (row: any) => computePortraitAliases(row as PortraitAliasesRow),
    lookups: [],
    aggregations: [] },
  { name: "Properties", file: "properties", rulebookRows: 10, fields: propertiesFieldTypes,
    columns: { property_id: "PropertyId", label: "Label", property_kind: "PropertyKind", datatype: "Datatype", is_user_addable: "IsUserAddable", has_one_current_value: "HasOneCurrentValue", is_symmetric: "IsSymmetric", domain_class: "DomainClass", range_class: "RangeClass", sort_order: "SortOrder", value_node_label: "ValueNodeLabel", current_value_wording: "CurrentValueWording", input_name: "InputName", record_kind: "RecordKind", record_order: "RecordOrder", name: "Name" },
    stored: ["property_id", "label", "property_kind", "datatype", "is_user_addable", "has_one_current_value", "is_symmetric", "domain_class", "range_class", "sort_order", "value_node_label", "current_value_wording", "input_name", "record_kind", "record_order"],
    compute: (row: any) => computeProperties(row as PropertiesRow),
    lookups: [],
    aggregations: [] },
  { name: "PropertyAxioms", file: "property_axioms", rulebookRows: 4, fields: propertyAxiomsFieldTypes,
    columns: { property_axiom_id: "PropertyAxiomId", kind: "Kind", property: "Property", first: "First", second: "Second", name: "Name" },
    stored: ["property_axiom_id", "kind", "property", "first", "second"],
    compute: (row: any) => computePropertyAxioms(row as PropertyAxiomsRow),
    lookups: [],
    aggregations: [] },
  { name: "Statements", file: "statements", rulebookRows: 270, fields: statementsFieldTypes,
    columns: { statement_id: "StatementId", graph: "Graph", subject: "Subject", property: "Property", object_thing: "ObjectThing", object_value: "ObjectValue", is_listed: "IsListed", is_built_in: "IsBuiltIn", is_detail: "IsDetail", is_stated: "IsStated", activation_seq: "ActivationSeq", sort_order: "SortOrder", subject_label: "SubjectLabel", property_label: "PropertyLabel", object_label: "ObjectLabel", property_datatype: "PropertyDatatype", property_has_one_current_value: "PropertyHasOneCurrentValue", property_domain_class: "PropertyDomainClass", property_range_class: "PropertyRangeClass", literal_quote: "LiteralQuote", literal_type_marker: "LiteralTypeMarker", built_in_source_text: "BuiltInSourceText", user_source_text: "UserSourceText", next_fact_sort_order: "NextFactSortOrder", is_literal: "IsLiteral", object_text: "ObjectText", name: "Name", words: "Words", source: "Source", subject_property_key: "SubjectPropertyKey", latest_sibling_activation_seq: "LatestSiblingActivationSeq", is_in_effect: "IsInEffect", listed_unit: "ListedUnit", in_effect_unit: "InEffectUnit", listed_off_unit: "ListedOffUnit", is_next_to_add: "IsNextToAdd", count_of_inverse_firings: "CountOfInverseFirings", count_of_symmetric_firings: "CountOfSymmetricFirings", holds_before_chain: "HoldsBeforeChain", count_of_chain_firings: "CountOfChainFirings", holds_before_typing: "HoldsBeforeTyping", count_of_type_firings: "CountOfTypeFirings", holds: "Holds", is_conclusion: "IsConclusion", holds_unit: "HoldsUnit", conclusion_unit: "ConclusionUnit", parent_unit: "ParentUnit", grandparent_unit: "GrandparentUnit", place_unit: "PlaceUnit", first_pre_chain_order_key: "FirstPreChainOrderKey", snap_position: "SnapPosition", first_fired_order_key: "FirstFiredOrderKey", count_of_chosen_firings: "CountOfChosenFirings", subject_node_key: "SubjectNodeKey", object_node_key: "ObjectNodeKey", subject_node_kind: "SubjectNodeKind", object_thing_node_kind: "ObjectThingNodeKind", object_node_kind: "ObjectNodeKind", object_value_node_label: "ObjectValueNodeLabel", count_of_violations_as_first: "CountOfViolationsAsFirst", count_of_violations_as_second: "CountOfViolationsAsSecond", is_in_contradiction: "IsInContradiction" },
    stored: ["statement_id", "graph", "subject", "property", "object_thing", "object_value", "is_listed", "is_built_in", "is_detail", "is_stated", "activation_seq", "sort_order"],
    compute: (row: any) => computeStatements(row as StatementsRow),
    lookups: [
      { field: "subject_label", target: "things", ret: "label", key: "subject", match: "thing_id" },
      { field: "property_label", target: "properties", ret: "label", key: "property", match: "property_id" },
      { field: "object_label", target: "things", ret: "label", key: "object_thing", match: "thing_id" },
      { field: "property_datatype", target: "properties", ret: "datatype", key: "property", match: "property_id" },
      { field: "property_has_one_current_value", target: "properties", ret: "has_one_current_value", key: "property", match: "property_id" },
      { field: "property_domain_class", target: "properties", ret: "domain_class", key: "property", match: "property_id" },
      { field: "property_range_class", target: "properties", ret: "range_class", key: "property", match: "property_id" },
      { field: "literal_quote", target: "graphs", ret: "literal_quote", key: "graph", match: "graph_id" },
      { field: "literal_type_marker", target: "graphs", ret: "literal_type_marker", key: "graph", match: "graph_id" },
      { field: "built_in_source_text", target: "graphs", ret: "built_in_source_text", key: "graph", match: "graph_id" },
      { field: "user_source_text", target: "graphs", ret: "user_source_text", key: "graph", match: "graph_id" },
      { field: "next_fact_sort_order", target: "graphs", ret: "next_fact_sort_order", key: "graph", match: "graph_id" },
      { field: "subject_node_kind", target: "things", ret: "node_kind", key: "subject", match: "thing_id" },
      { field: "object_thing_node_kind", target: "things", ret: "node_kind", key: "object_thing", match: "thing_id" },
      { field: "object_value_node_label", target: "properties", ret: "value_node_label", key: "property", match: "property_id" },],
    aggregations: [
      { field: "latest_sibling_activation_seq", op: "MAX", table: "statements", target: "activation_seq", criteria: [{ range: "subject_property_key", kind: "field", field: "subject_property_key" }] },
      { field: "count_of_inverse_firings", op: "SUM", table: "firings", target: "inverse_fired_unit", criteria: [{ range: "target", kind: "field", field: "statement_id" }] },
      { field: "count_of_symmetric_firings", op: "SUM", table: "firings", target: "symmetric_fired_unit", criteria: [{ range: "target", kind: "field", field: "statement_id" }] },
      { field: "count_of_chain_firings", op: "SUM", table: "firings", target: "chain_fired_unit", criteria: [{ range: "target", kind: "field", field: "statement_id" }] },
      { field: "count_of_type_firings", op: "SUM", table: "firings", target: "type_fired_unit", criteria: [{ range: "target", kind: "field", field: "statement_id" }] },
      { field: "first_pre_chain_order_key", op: "MIN", table: "firings", target: "pre_chain_order_key", criteria: [{ range: "target", kind: "field", field: "statement_id" }, { range: "pre_chain_fired_unit", kind: "literal", literal: vI(1) }] },
      { field: "first_fired_order_key", op: "MIN", table: "firings", target: "order_key", criteria: [{ range: "target", kind: "field", field: "statement_id" }, { range: "fired_unit", kind: "literal", literal: vI(1) }] },
      { field: "count_of_chosen_firings", op: "SUM", table: "firings", target: "chosen_unit", criteria: [{ range: "target", kind: "field", field: "statement_id" }] },
      { field: "count_of_violations_as_first", op: "SUM", table: "disjoint_checks", target: "violation_unit", criteria: [{ range: "first", kind: "field", field: "statement_id" }] },
      { field: "count_of_violations_as_second", op: "SUM", table: "disjoint_checks", target: "violation_unit", criteria: [{ range: "second", kind: "field", field: "statement_id" }] },] },
  { name: "Firings", file: "firings", rulebookRows: 465, fields: firingsFieldTypes,
    columns: { firing_id: "FiringId", kind: "Kind", rule: "Rule", target: "Target", first: "First", second: "Second", rule_enabled: "RuleEnabled", kind_wording: "KindWording", kind_sort_order: "KindSortOrder", target_name: "TargetName", target_subject_label: "TargetSubjectLabel", target_property_label: "TargetPropertyLabel", target_object_text: "TargetObjectText", target_is_conclusion: "TargetIsConclusion", target_subject_node_key: "TargetSubjectNodeKey", target_object_node_key: "TargetObjectNodeKey", target_first_fired_order_key: "TargetFirstFiredOrderKey", first_words: "FirstWords", second_words: "SecondWords", first_is_in_effect: "FirstIsInEffect", first_holds_before_chain: "FirstHoldsBeforeChain", second_holds_before_chain: "SecondHoldsBeforeChain", first_holds_before_typing: "FirstHoldsBeforeTyping", first_property: "FirstProperty", first_property_domain_class: "FirstPropertyDomainClass", first_property_range_class: "FirstPropertyRangeClass", first_sort_order: "FirstSortOrder", second_is_in_effect: "SecondIsInEffect", first_snap_position: "FirstSnapPosition", second_snap_position: "SecondSnapPosition", inverse_fired_unit: "InverseFiredUnit", symmetric_fired_unit: "SymmetricFiredUnit", chain_fired_unit: "ChainFiredUnit", type_fired_unit: "TypeFiredUnit", fired_unit: "FiredUnit", pre_chain_fired_unit: "PreChainFiredUnit", pre_chain_order_key: "PreChainOrderKey", round: "Round", order_key: "OrderKey", is_chosen: "IsChosen", chosen_unit: "ChosenUnit", card_text: "CardText", name: "Name" },
    stored: ["firing_id", "kind", "rule", "target", "first", "second"],
    compute: (row: any) => computeFirings(row as FiringsRow),
    lookups: [
      { field: "rule_enabled", target: "rules", ret: "is_enabled", key: "rule", match: "rule_id" },
      { field: "kind_wording", target: "firing_kinds", ret: "wording", key: "kind", match: "firing_kind_id" },
      { field: "kind_sort_order", target: "firing_kinds", ret: "sort_order", key: "kind", match: "firing_kind_id" },
      { field: "target_name", target: "statements", ret: "name", key: "target", match: "statement_id" },
      { field: "target_subject_label", target: "statements", ret: "subject_label", key: "target", match: "statement_id" },
      { field: "target_property_label", target: "statements", ret: "property_label", key: "target", match: "statement_id" },
      { field: "target_object_text", target: "statements", ret: "object_text", key: "target", match: "statement_id" },
      { field: "target_is_conclusion", target: "statements", ret: "is_conclusion", key: "target", match: "statement_id" },
      { field: "target_subject_node_key", target: "statements", ret: "subject_node_key", key: "target", match: "statement_id" },
      { field: "target_object_node_key", target: "statements", ret: "object_node_key", key: "target", match: "statement_id" },
      { field: "target_first_fired_order_key", target: "statements", ret: "first_fired_order_key", key: "target", match: "statement_id" },
      { field: "first_words", target: "statements", ret: "words", key: "first", match: "statement_id" },
      { field: "second_words", target: "statements", ret: "words", key: "second", match: "statement_id" },
      { field: "first_is_in_effect", target: "statements", ret: "is_in_effect", key: "first", match: "statement_id" },
      { field: "first_holds_before_chain", target: "statements", ret: "holds_before_chain", key: "first", match: "statement_id" },
      { field: "second_holds_before_chain", target: "statements", ret: "holds_before_chain", key: "second", match: "statement_id" },
      { field: "first_holds_before_typing", target: "statements", ret: "holds_before_typing", key: "first", match: "statement_id" },
      { field: "first_property", target: "statements", ret: "property", key: "first", match: "statement_id" },
      { field: "first_property_domain_class", target: "statements", ret: "property_domain_class", key: "first", match: "statement_id" },
      { field: "first_property_range_class", target: "statements", ret: "property_range_class", key: "first", match: "statement_id" },
      { field: "first_sort_order", target: "statements", ret: "sort_order", key: "first", match: "statement_id" },
      { field: "second_is_in_effect", target: "statements", ret: "is_in_effect", key: "second", match: "statement_id" },
      { field: "first_snap_position", target: "statements", ret: "snap_position", key: "first", match: "statement_id" },
      { field: "second_snap_position", target: "statements", ret: "snap_position", key: "second", match: "statement_id" },],
    aggregations: [] },
  { name: "DisjointChecks", file: "disjoint_checks", rulebookRows: 42, fields: disjointChecksFieldTypes,
    columns: { disjoint_check_id: "DisjointCheckId", graph: "Graph", rule: "Rule", first: "First", second: "Second", rule_enabled: "RuleEnabled", first_holds: "FirstHolds", second_holds: "SecondHolds", subject_label: "SubjectLabel", object_label: "ObjectLabel", violation_unit: "ViolationUnit", alarm_text: "AlarmText", name: "Name" },
    stored: ["disjoint_check_id", "graph", "rule", "first", "second"],
    compute: (row: any) => computeDisjointChecks(row as DisjointChecksRow),
    lookups: [
      { field: "rule_enabled", target: "rules", ret: "is_enabled", key: "rule", match: "rule_id" },
      { field: "first_holds", target: "statements", ret: "holds", key: "first", match: "statement_id" },
      { field: "second_holds", target: "statements", ret: "holds", key: "second", match: "statement_id" },
      { field: "subject_label", target: "statements", ret: "subject_label", key: "first", match: "statement_id" },
      { field: "object_label", target: "statements", ret: "object_label", key: "first", match: "statement_id" },],
    aggregations: [] },
  { name: "Scenarios", file: "scenarios", rulebookRows: 7, fields: scenariosFieldTypes,
    columns: { scenario_id: "ScenarioId", graph: "Graph", label: "Label", note: "Note", sort_order: "SortOrder", button_group: "ButtonGroup", is_primary: "IsPrimary", fact_baseline: "FactBaseline", layout: "Layout", clears_selection: "ClearsSelection", clears_form_message: "ClearsFormMessage", name: "Name", graph_has_facts_to_add: "GraphHasFactsToAdd", is_available: "IsAvailable" },
    stored: ["scenario_id", "graph", "label", "note", "sort_order", "button_group", "is_primary", "fact_baseline", "layout", "clears_selection", "clears_form_message"],
    compute: (row: any) => computeScenarios(row as ScenariosRow),
    lookups: [
      { field: "graph_has_facts_to_add", target: "graphs", ret: "has_facts_to_add", key: "graph", match: "graph_id" },],
    aggregations: [] },
  { name: "ScenarioFactChanges", file: "scenario_fact_changes", rulebookRows: 6, fields: scenarioFactChangesFieldTypes,
    columns: { scenario_fact_change_id: "ScenarioFactChangeId", scenario: "Scenario", subject: "Subject", property: "Property", object_key: "ObjectKey", action: "Action", adds_to_list: "AddsToList", sort_order: "SortOrder", name: "Name" },
    stored: ["scenario_fact_change_id", "scenario", "subject", "property", "object_key", "action", "adds_to_list", "sort_order"],
    compute: (row: any) => computeScenarioFactChanges(row as ScenarioFactChangesRow),
    lookups: [],
    aggregations: [] },
  { name: "ScenarioRuleSettings", file: "scenario_rule_settings", rulebookRows: 10, fields: scenarioRuleSettingsFieldTypes,
    columns: { scenario_rule_setting_id: "ScenarioRuleSettingId", scenario: "Scenario", rule: "Rule", is_enabled: "IsEnabled", name: "Name" },
    stored: ["scenario_rule_setting_id", "scenario", "rule", "is_enabled"],
    compute: (row: any) => computeScenarioRuleSettings(row as ScenarioRuleSettingsRow),
    lookups: [],
    aggregations: [] },
  { name: "NodeLayouts", file: "node_layouts", rulebookRows: 26, fields: nodeLayoutsFieldTypes,
    columns: { node_layout_id: "NodeLayoutId", layout: "Layout", node_key: "NodeKey", x: "X", y: "Y", name: "Name" },
    stored: ["node_layout_id", "layout", "node_key", "x", "y"],
    compute: (row: any) => computeNodeLayouts(row as NodeLayoutsRow),
    lookups: [],
    aggregations: [] },
  { name: "LayoutSlots", file: "layout_slots", rulebookRows: 8, fields: layoutSlotsFieldTypes,
    columns: { layout_slot_id: "LayoutSlotId", sort_order: "SortOrder", x: "X", y: "Y", name: "Name" },
    stored: ["layout_slot_id", "sort_order", "x", "y"],
    compute: (row: any) => computeLayoutSlots(row as LayoutSlotsRow),
    lookups: [],
    aggregations: [] },
  { name: "PageTexts", file: "page_texts", rulebookRows: 102, fields: pageTextsFieldTypes,
    columns: { page_text_id: "PageTextId", text: "Text", name: "Name" },
    stored: ["page_text_id", "text"],
    compute: (row: any) => computePageTexts(row as PageTextsRow),
    lookups: [],
    aggregations: [] },
  { name: "Engines", file: "engines", rulebookRows: 3, fields: enginesFieldTypes,
    columns: { engine_id: "EngineId", label: "Label", substrate: "Substrate", is_default: "IsDefault", description: "Description", sort_order: "SortOrder", name: "Name" },
    stored: ["engine_id", "label", "substrate", "is_default", "description", "sort_order"],
    compute: (row: any) => computeEngines(row as EnginesRow),
    lookups: [],
    aggregations: [] },
  { name: "RulebookTools", file: "rulebook_tools", rulebookRows: 8, fields: rulebookToolsFieldTypes,
    columns: { rulebook_tool_id: "RulebookToolId", label: "Label", adds: "Adds", href: "Href", link_text: "LinkText", sort_order: "SortOrder", name: "Name" },
    stored: ["rulebook_tool_id", "label", "adds", "href", "link_text", "sort_order"],
    compute: (row: any) => computeRulebookTools(row as RulebookToolsRow),
    lookups: [],
    aggregations: [] },
];

/** Materializes each vw_<entity>_closure view aggregations read. */
export const erbClosures: ClosureSpec[] = [
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
