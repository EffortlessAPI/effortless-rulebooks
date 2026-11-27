// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/talisman-s-south-park-ontology-explainer-the-concept-form-rulebook.json
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
// THINGS TABLE
// Every individual of the ontology (owl:Thing). A class's table lists the individuals that belong to it.
// =============================================================================

/** A row in the Things table. */
export interface ThingsRow {
  /** Key of the thing; also its local name in the model namespace. */
  thing_id: string;
  /** The name the page shows. */
  label: string;
  /** Position in the cast. */
  sort_order: number;
  /** The portrait drawn for this thing; set when its name matches a portrait alias. */
  portrait: string | null;
  _erb_errors?: Record<string, string>;
}

const thingsFieldTypes: Record<string, FieldType> = {
  thing_id: "string",
  label: "string",
  sort_order: "int",
  portrait: "*string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeThings(tc: ThingsRow): ThingsRow {
  return tc;
}

/** Reads Things rows from a JSON array file. */
export function loadThingsRows(file: string): ThingsRow[] {
  return loadRows(file, { fields: thingsFieldTypes }) as unknown as ThingsRow[];
}

// =============================================================================
// PERSON TABLE
// The class Person: one row per individual that belongs to it. Its label and place are its ERBTables row.
// =============================================================================

/** A row in the Person table. */
export interface PersonRow {
  /** The individual (its ThingId). */
  person_id: string;
  /** The individual's row in Things. */
  thing: string;
  /** True when a fact states it is a Person. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand. */
  is_asserted: boolean;
  /** has grade level: the one current value. A property of the ontology; its other attributes are its ERBFields row. */
  has_grade_level: number | null;
  /** has role: the one current value. A property of the ontology; its other attributes are its ERBFields row. */
  has_role: string | null;
  _erb_errors?: Record<string, string>;
}

const personFieldTypes: Record<string, FieldType> = {
  person_id: "string",
  thing: "string",
  is_asserted: "bool",
  has_grade_level: "*int",
  has_role: "*string",
};

/** Computes every calculated field of the row in dependency order. */
export function computePerson(tc: PersonRow): PersonRow {
  return tc;
}

/** Reads Person rows from a JSON array file. */
export function loadPersonRows(file: string): PersonRow[] {
  return loadRows(file, { fields: personFieldTypes }) as unknown as PersonRow[];
}

// =============================================================================
// SCHOOL TABLE
// The class School: one row per individual that belongs to it. Its label and place are its ERBTables row.
// =============================================================================

/** A row in the School table. */
export interface SchoolRow {
  /** The individual (its ThingId). */
  school_id: string;
  /** The individual's row in Things. */
  thing: string;
  /** True when a fact states it is a School. False when only a property's domain or range implies it: such a row follows from the facts and is not added by hand. */
  is_asserted: boolean;
  _erb_errors?: Record<string, string>;
}

const schoolFieldTypes: Record<string, FieldType> = {
  school_id: "string",
  thing: "string",
  is_asserted: "bool",
};

/** Computes every calculated field of the row in dependency order. */
export function computeSchool(tc: SchoolRow): SchoolRow {
  return tc;
}

/** Reads School rows from a JSON array file. */
export function loadSchoolRows(file: string): SchoolRow[] {
  return loadRows(file, { fields: schoolFieldTypes }) as unknown as SchoolRow[];
}

// =============================================================================
// ISPARENTOF TABLE
// is parent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the isParentOf table. */
export interface isParentOfRow {
  /** The fact: subject|property|object. */
  is_parent_of_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** What it links to. The table it links to is the property's range: a class's table, or Things for any thing. */
  object: string;
  _erb_errors?: Record<string, string>;
}

const isParentOfFieldTypes: Record<string, FieldType> = {
  is_parent_of_id: "string",
  subject: "string",
  object: "string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeisParentOf(tc: isParentOfRow): isParentOfRow {
  return tc;
}

/** Reads isParentOf rows from a JSON array file. */
export function loadisParentOfRows(file: string): isParentOfRow[] {
  return loadRows(file, { fields: isParentOfFieldTypes }) as unknown as isParentOfRow[];
}

// =============================================================================
// ISCHILDOF TABLE
// is child of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the isChildOf table. */
export interface isChildOfRow {
  /** The fact: subject|property|object. */
  is_child_of_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** What it links to. The table it links to is the property's range: a class's table, or Things for any thing. */
  object: string;
  _erb_errors?: Record<string, string>;
}

const isChildOfFieldTypes: Record<string, FieldType> = {
  is_child_of_id: "string",
  subject: "string",
  object: "string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeisChildOf(tc: isChildOfRow): isChildOfRow {
  return tc;
}

/** Reads isChildOf rows from a JSON array file. */
export function loadisChildOfRows(file: string): isChildOfRow[] {
  return loadRows(file, { fields: isChildOfFieldTypes }) as unknown as isChildOfRow[];
}

// =============================================================================
// ISSPOUSEOF TABLE
// is spouse of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the isSpouseOf table. */
export interface isSpouseOfRow {
  /** The fact: subject|property|object. */
  is_spouse_of_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** What it links to. The table it links to is the property's range: a class's table, or Things for any thing. */
  object: string;
  _erb_errors?: Record<string, string>;
}

const isSpouseOfFieldTypes: Record<string, FieldType> = {
  is_spouse_of_id: "string",
  subject: "string",
  object: "string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeisSpouseOf(tc: isSpouseOfRow): isSpouseOfRow {
  return tc;
}

/** Reads isSpouseOf rows from a JSON array file. */
export function loadisSpouseOfRows(file: string): isSpouseOfRow[] {
  return loadRows(file, { fields: isSpouseOfFieldTypes }) as unknown as isSpouseOfRow[];
}

// =============================================================================
// ISFRIENDOF TABLE
// is friend of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the isFriendOf table. */
export interface isFriendOfRow {
  /** The fact: subject|property|object. */
  is_friend_of_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** What it links to. The table it links to is the property's range: a class's table, or Things for any thing. */
  object: string;
  _erb_errors?: Record<string, string>;
}

const isFriendOfFieldTypes: Record<string, FieldType> = {
  is_friend_of_id: "string",
  subject: "string",
  object: "string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeisFriendOf(tc: isFriendOfRow): isFriendOfRow {
  return tc;
}

/** Reads isFriendOf rows from a JSON array file. */
export function loadisFriendOfRows(file: string): isFriendOfRow[] {
  return loadRows(file, { fields: isFriendOfFieldTypes }) as unknown as isFriendOfRow[];
}

// =============================================================================
// ATTENDS TABLE
// attends: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the attends table. */
export interface attendsRow {
  /** The fact: subject|property|object. */
  attends_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** What it links to. The table it links to is the property's range: a class's table, or Things for any thing. */
  object: string;
  _erb_errors?: Record<string, string>;
}

const attendsFieldTypes: Record<string, FieldType> = {
  attends_id: "string",
  subject: "string",
  object: "string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeattends(tc: attendsRow): attendsRow {
  return tc;
}

/** Reads attends rows from a JSON array file. */
export function loadattendsRows(file: string): attendsRow[] {
  return loadRows(file, { fields: attendsFieldTypes }) as unknown as attendsRow[];
}

// =============================================================================
// HASAGE TABLE
// has age: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the hasAge table. */
export interface hasAgeRow {
  /** The fact: subject|property|object. */
  has_age_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** The value. */
  value: number;
  _erb_errors?: Record<string, string>;
}

const hasAgeFieldTypes: Record<string, FieldType> = {
  has_age_id: "string",
  subject: "string",
  value: "int",
};

/** Computes every calculated field of the row in dependency order. */
export function computehasAge(tc: hasAgeRow): hasAgeRow {
  return tc;
}

/** Reads hasAge rows from a JSON array file. */
export function loadhasAgeRows(file: string): hasAgeRow[] {
  return loadRows(file, { fields: hasAgeFieldTypes }) as unknown as hasAgeRow[];
}

// =============================================================================
// ISGRANDPARENTOF TABLE
// is grandparent of: one row per fact in effect. A property of the ontology; its attributes are its ERBTables row.
// =============================================================================

/** A row in the isGrandparentOf table. */
export interface isGrandparentOfRow {
  /** The fact: subject|property|object. */
  is_grandparent_of_id: string;
  /** Who the fact is about. The table it links to is the property's domain: a class's table, or Things for any thing. */
  subject: string;
  /** What it links to. The table it links to is the property's range: a class's table, or Things for any thing. */
  object: string;
  _erb_errors?: Record<string, string>;
}

const isGrandparentOfFieldTypes: Record<string, FieldType> = {
  is_grandparent_of_id: "string",
  subject: "string",
  object: "string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeisGrandparentOf(tc: isGrandparentOfRow): isGrandparentOfRow {
  return tc;
}

/** Reads isGrandparentOf rows from a JSON array file. */
export function loadisGrandparentOfRows(file: string): isGrandparentOfRow[] {
  return loadRows(file, { fields: isGrandparentOfFieldTypes }) as unknown as isGrandparentOfRow[];
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
// ERBTABLES TABLE
// One row per table of this rulebook (the platform's Schema module), and on it what the table's structure does not say. A class: its label and its place in the cast. A property that is a table: its label, its wording on the page, whether it is symmetric, and its inverse, chain and disjoint axioms. What a table IS is read from its fields: Thing and IsAsserted make a class; Subject with Object or Value make a property, whose domain and range are the tables they link to.
// =============================================================================

/** A row in the ERBTables table. */
export interface ERBTablesRow {
  /** The concept table's name: this row's key. */
  erb_table_id: string;
  /** The table's name again, as a plain column. */
  table_name: string | null;
  /** What the table holds. Follows the table. */
  description: string | null;
  /** True for a table that serves the rulebook itself (ERBTables, ERBFields) and not the ontology. */
  is_meta_table: boolean | null;
  /** The Schema module's: rows belong to one tenant. Not used by the ontology. */
  is_tenant_scoped: boolean | null;
  /** The Schema module's: the table tenancy is reached through. Not used by the ontology. */
  tenant_parent_table: string | null;
  /** The Schema module's: the field that points at TenantParentTable. Not used by the ontology. */
  tenant_parent_field: string | null;
  /** Display order. For a class: Position in the cast. For a property: Position in the Add fact form's property list. */
  sort_order: number | null;
  /** For a class: The name the page shows. For a property: The property's wording on the page. */
  label: string | null;
  /** For a class: The portrait drawn for this thing; set when its name matches a portrait alias. */
  portrait: string | null;
  /** For a property: Offered in the page's Add fact form. */
  is_user_addable: boolean | null;
  /** For a property: The symmetric rule makes this link true in both directions. */
  is_symmetric: boolean | null;
  /** For a property: The small label under a value node for this property: 'role value' or 'number value'. */
  value_node_label: string | null;
  /** For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'. */
  current_value_wording: string | null;
  /** For a property: How the integer check names this property's value, as in 'for grade level'. */
  input_name: string | null;
  /** For a property: The word the current-record summary uses for this property's values: role, grade or school. */
  record_kind: string | null;
  /** For a property: Position of this property in the current-record summary. */
  record_order: number | null;
  /** The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property. */
  inverse_of: string | null;
  /** With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link. */
  chain_first: string | null;
  /** The chain's second link. Set together with ChainFirst. */
  chain_second: string | null;
  /** Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '. */
  disjoint_with: string | null;
  _erb_errors?: Record<string, string>;
}

const eRBTablesFieldTypes: Record<string, FieldType> = {
  erb_table_id: "string",
  table_name: "*string",
  description: "*string",
  is_meta_table: "*bool",
  is_tenant_scoped: "*bool",
  tenant_parent_table: "*string",
  tenant_parent_field: "*string",
  sort_order: "*int",
  label: "*string",
  portrait: "*string",
  is_user_addable: "*bool",
  is_symmetric: "*bool",
  value_node_label: "*string",
  current_value_wording: "*string",
  input_name: "*string",
  record_kind: "*string",
  record_order: "*int",
  inverse_of: "*string",
  chain_first: "*string",
  chain_second: "*string",
  disjoint_with: "*string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeERBTables(tc: ERBTablesRow): ERBTablesRow {
  return tc;
}

/** Reads ERBTables rows from a JSON array file. */
export function loadERBTablesRows(file: string): ERBTablesRow[] {
  return loadRows(file, { fields: eRBTablesFieldTypes }) as unknown as ERBTablesRow[];
}

// =============================================================================
// ERBFIELDS TABLE
// One row per field of every table (the platform's Schema module), and on it the same attributes for a property that is a column: a property with one current value, on its domain's class table (on Things when it has no domain class).
// =============================================================================

/** A row in the ERBFields table. */
export interface ERBFieldsRow {
  /** Table.Field: this row's key. */
  erb_field_id: string;
  /** The table the field is on. */
  erb_table: string | null;
  /** The field's name. For a column that is a property: the property's id. */
  field_name: string | null;
  /** raw or relationship. Follows the field. */
  field_type: string | null;
  /** The rulebook datatype. Follows the field. */
  datatype: string | null;
  /** True for the field that identifies a row of its table. */
  is_primary_key: boolean | null;
  /** What the field means. Follows the field. */
  description: string | null;
  /** For a property: The property's wording on the page. */
  label: string | null;
  /** For a property: Offered in the page's Add fact form. */
  is_user_addable: boolean | null;
  /** For a property: The symmetric rule makes this link true in both directions. */
  is_symmetric: boolean | null;
  /** For a property: Position in the Add fact form's property list. */
  sort_order: number | null;
  /** For a property: The small label under a value node for this property: 'role value' or 'number value'. */
  value_node_label: string | null;
  /** For a property: How the Add fact form names a replaced value, as in 'Stan now has grade level 5'. */
  current_value_wording: string | null;
  /** For a property: How the integer check names this property's value, as in 'for grade level'. */
  input_name: string | null;
  /** For a property: The word the current-record summary uses for this property's values: role, grade or school. */
  record_kind: string | null;
  /** For a property: Position of this property in the current-record summary. */
  record_order: number | null;
  /** The property that reads this one backwards (OWL inverseOf): when A has this link to B, the inverse rule concludes that B has that one to A. Names a property: a property's table, or a column that is a property. */
  inverse_of: string | null;
  /** With ChainSecond, the property chain that concludes this property (OWL propertyChainAxiom): a ChainFirst link followed by a ChainSecond link. */
  chain_first: string | null;
  /** The chain's second link. Set together with ChainFirst. */
  chain_second: string | null;
  /** Properties that never both hold between the same two things (OWL propertyDisjointWith). Several are separated by ', '. */
  disjoint_with: string | null;
  _erb_errors?: Record<string, string>;
}

const eRBFieldsFieldTypes: Record<string, FieldType> = {
  erb_field_id: "string",
  erb_table: "*string",
  field_name: "*string",
  field_type: "*string",
  datatype: "*string",
  is_primary_key: "*bool",
  description: "*string",
  label: "*string",
  is_user_addable: "*bool",
  is_symmetric: "*bool",
  sort_order: "*int",
  value_node_label: "*string",
  current_value_wording: "*string",
  input_name: "*string",
  record_kind: "*string",
  record_order: "*int",
  inverse_of: "*string",
  chain_first: "*string",
  chain_second: "*string",
  disjoint_with: "*string",
};

/** Computes every calculated field of the row in dependency order. */
export function computeERBFields(tc: ERBFieldsRow): ERBFieldsRow {
  return tc;
}

/** Reads ERBFields rows from a JSON array file. */
export function loadERBFieldsRows(file: string): ERBFieldsRow[] {
  return loadRows(file, { fields: eRBFieldsFieldTypes }) as unknown as ERBFieldsRow[];
}

/** Bounds the runner's passes over the dataset. */
export const calculatedFieldCount = 2;

/** Every table, in rulebook order. */
export const erbTables: TableSpec[] = [
  { name: "__meta__", file: "__meta__", rulebookRows: 9, fields: __meta__FieldTypes,
    columns: { meta_key: "MetaKey", name: "Name", value_type: "ValueType", string_value: "StringValue", json_value: "JsonValue" },
    stored: ["meta_key", "value_type", "string_value", "json_value"],
    compute: (row: any) => compute__meta__(row as __meta__Row),
    lookups: [],
    aggregations: [] },
  { name: "Things", file: "things", rulebookRows: 7, fields: thingsFieldTypes,
    columns: { thing_id: "ThingId", label: "Label", sort_order: "SortOrder", portrait: "Portrait" },
    stored: ["thing_id", "label", "sort_order", "portrait"],
    compute: (row: any) => computeThings(row as ThingsRow),
    lookups: [],
    aggregations: [] },
  { name: "Person", file: "person", rulebookRows: 1, fields: personFieldTypes,
    columns: { person_id: "PersonId", thing: "Thing", is_asserted: "IsAsserted", has_grade_level: "hasGradeLevel", has_role: "hasRole" },
    stored: ["person_id", "thing", "is_asserted", "has_grade_level", "has_role"],
    compute: (row: any) => computePerson(row as PersonRow),
    lookups: [],
    aggregations: [] },
  { name: "School", file: "school", rulebookRows: 1, fields: schoolFieldTypes,
    columns: { school_id: "SchoolId", thing: "Thing", is_asserted: "IsAsserted" },
    stored: ["school_id", "thing", "is_asserted"],
    compute: (row: any) => computeSchool(row as SchoolRow),
    lookups: [],
    aggregations: [] },
  { name: "isParentOf", file: "is_parent_of", rulebookRows: 3, fields: isParentOfFieldTypes,
    columns: { is_parent_of_id: "isParentOfId", subject: "Subject", object: "Object" },
    stored: ["is_parent_of_id", "subject", "object"],
    compute: (row: any) => computeisParentOf(row as isParentOfRow),
    lookups: [],
    aggregations: [] },
  { name: "isChildOf", file: "is_child_of", rulebookRows: 0, fields: isChildOfFieldTypes,
    columns: { is_child_of_id: "isChildOfId", subject: "Subject", object: "Object" },
    stored: ["is_child_of_id", "subject", "object"],
    compute: (row: any) => computeisChildOf(row as isChildOfRow),
    lookups: [],
    aggregations: [] },
  { name: "isSpouseOf", file: "is_spouse_of", rulebookRows: 1, fields: isSpouseOfFieldTypes,
    columns: { is_spouse_of_id: "isSpouseOfId", subject: "Subject", object: "Object" },
    stored: ["is_spouse_of_id", "subject", "object"],
    compute: (row: any) => computeisSpouseOf(row as isSpouseOfRow),
    lookups: [],
    aggregations: [] },
  { name: "isFriendOf", file: "is_friend_of", rulebookRows: 1, fields: isFriendOfFieldTypes,
    columns: { is_friend_of_id: "isFriendOfId", subject: "Subject", object: "Object" },
    stored: ["is_friend_of_id", "subject", "object"],
    compute: (row: any) => computeisFriendOf(row as isFriendOfRow),
    lookups: [],
    aggregations: [] },
  { name: "attends", file: "attends", rulebookRows: 1, fields: attendsFieldTypes,
    columns: { attends_id: "attendsId", subject: "Subject", object: "Object" },
    stored: ["attends_id", "subject", "object"],
    compute: (row: any) => computeattends(row as attendsRow),
    lookups: [],
    aggregations: [] },
  { name: "hasAge", file: "has_age", rulebookRows: 1, fields: hasAgeFieldTypes,
    columns: { has_age_id: "hasAgeId", subject: "Subject", value: "Value" },
    stored: ["has_age_id", "subject", "value"],
    compute: (row: any) => computehasAge(row as hasAgeRow),
    lookups: [],
    aggregations: [] },
  { name: "isGrandparentOf", file: "is_grandparent_of", rulebookRows: 0, fields: isGrandparentOfFieldTypes,
    columns: { is_grandparent_of_id: "isGrandparentOfId", subject: "Subject", object: "Object" },
    stored: ["is_grandparent_of_id", "subject", "object"],
    compute: (row: any) => computeisGrandparentOf(row as isGrandparentOfRow),
    lookups: [],
    aggregations: [] },
  { name: "Portraits", file: "portraits", rulebookRows: 13, fields: portraitsFieldTypes,
    columns: { portrait_id: "PortraitId", display_name: "DisplayName", page: "Page", image: "Image", crop: "Crop", width: "Width", height: "Height", sort_order: "SortOrder", name: "Name" },
    stored: ["portrait_id", "display_name", "page", "image", "crop", "width", "height", "sort_order"],
    compute: (row: any) => computePortraits(row as PortraitsRow),
    lookups: [],
    aggregations: [] },
  { name: "ERBTables", file: "erb_tables", rulebookRows: 13, fields: eRBTablesFieldTypes,
    columns: { erb_table_id: "ERBTableId", table_name: "TableName", description: "Description", is_meta_table: "IsMetaTable", is_tenant_scoped: "IsTenantScoped", tenant_parent_table: "TenantParentTable", tenant_parent_field: "TenantParentField", sort_order: "SortOrder", label: "Label", portrait: "Portrait", is_user_addable: "IsUserAddable", is_symmetric: "IsSymmetric", value_node_label: "ValueNodeLabel", current_value_wording: "CurrentValueWording", input_name: "InputName", record_kind: "RecordKind", record_order: "RecordOrder", inverse_of: "InverseOf", chain_first: "ChainFirst", chain_second: "ChainSecond", disjoint_with: "DisjointWith" },
    stored: ["erb_table_id", "table_name", "description", "is_meta_table", "is_tenant_scoped", "tenant_parent_table", "tenant_parent_field", "sort_order", "label", "portrait", "is_user_addable", "is_symmetric", "value_node_label", "current_value_wording", "input_name", "record_kind", "record_order", "inverse_of", "chain_first", "chain_second", "disjoint_with"],
    compute: (row: any) => computeERBTables(row as ERBTablesRow),
    lookups: [],
    aggregations: [] },
  { name: "ERBFields", file: "erb_fields", rulebookRows: 83, fields: eRBFieldsFieldTypes,
    columns: { erb_field_id: "ERBFieldId", erb_table: "ERBTable", field_name: "FieldName", field_type: "FieldType", datatype: "Datatype", is_primary_key: "IsPrimaryKey", description: "Description", label: "Label", is_user_addable: "IsUserAddable", is_symmetric: "IsSymmetric", sort_order: "SortOrder", value_node_label: "ValueNodeLabel", current_value_wording: "CurrentValueWording", input_name: "InputName", record_kind: "RecordKind", record_order: "RecordOrder", inverse_of: "InverseOf", chain_first: "ChainFirst", chain_second: "ChainSecond", disjoint_with: "DisjointWith" },
    stored: ["erb_field_id", "erb_table", "field_name", "field_type", "datatype", "is_primary_key", "description", "label", "is_user_addable", "is_symmetric", "sort_order", "value_node_label", "current_value_wording", "input_name", "record_kind", "record_order", "inverse_of", "chain_first", "chain_second", "disjoint_with"],
    compute: (row: any) => computeERBFields(row as ERBFieldsRow),
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
