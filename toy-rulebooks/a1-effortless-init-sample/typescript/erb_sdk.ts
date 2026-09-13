// ERB SDK (GENERATED - DO NOT EDIT)
// ===================================
// Generated from: effortless-rulebook/a1-effortless-init-sample-rulebook.json
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
// HELLOWHOS TABLE
// The smallest complete rulebook: an id, a display name, and a rule that derives a greeting from it.
// =============================================================================

/** A row in the HelloWhos table. */
export interface HelloWhosRow {
  hello_who_id: string;
  name: string;
  introduction: string | null;
  _erb_errors?: Record<string, string>;
}

const helloWhosFieldTypes: Record<string, FieldType> = {
  hello_who_id: "string",
  name: "string",
  introduction: "*string",
};

/** Computes the Introduction calculated field.
 *  Formula: ="Hello " & {{Name}} & "!!!" */
export function calcHelloWhosIntroduction(tc: HelloWhosRow): string | null {
  return toStringPtr(erbConcat(vS("Hello "), erbTextOr(vStrPlain(tc.name)), vS("!!!")));
}

/** Computes every calculated field of the row in dependency order. */
export function computeHelloWhos(tc: HelloWhosRow): HelloWhosRow {
  // Level 1
  calcGuard(tc, helloWhosFieldTypes, "introduction", () => { tc.introduction = calcHelloWhosIntroduction(tc); });
  return tc;
}

/** Reads HelloWhos rows from a JSON array file. */
export function loadHelloWhosRows(file: string): HelloWhosRow[] {
  return loadRows(file, { fields: helloWhosFieldTypes }) as unknown as HelloWhosRow[];
}

/** Bounds the runner's passes over the dataset. */
export const calculatedFieldCount = 1;

/** Every table, in rulebook order. */
export const erbTables: TableSpec[] = [
  { name: "HelloWhos", file: "hello_whos", rulebookRows: 3, fields: helloWhosFieldTypes,
    compute: (row: any) => computeHelloWhos(row as HelloWhosRow),
    lookups: [],
    aggregations: [] },
];

/** Materializes each vw_<entity>_closure view aggregations read. */
export const erbClosures: ClosureSpec[] = [
];
