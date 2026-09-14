// Formula runtime for the TypeScript substrate.
//
// HAND-WRITTEN, NOT GENERATED. erb_sdk.ts compiles every formula to calls on
// this file and describes every lookup and aggregation as data this file runs,
// so there is one implementation of the dialect's semantics in TypeScript. It
// ships beside the generated SDK the way erb_runtime.go ships beside the Go SDK,
// and it is a line-for-line port of that file: NULL propagates through
// comparisons and AND/OR/NOT, a raw text column reads through NULLIF(x, ''),
// DATETIME_DIFF counts calendar days, CONCAT renders a datetime as timestamptz
// text. When a rule changes in erb_runtime.go or erb_runtime.py, it changes here.
//
// This file parses no formula, reads no rulebook, and calls no database or other
// tool. Cross-table values come from this run's own rows.
//
// A Value is dynamically typed because a formula's operands are: a lookup can
// return any column, an IF can mix branches, and NULL is a value of every type.
// Row interfaces are the SDK's public shape; Values are what formulas compute
// with in between.

import * as fs from "node:fs";
import * as path from "node:path";

// ─────────────────────────────── values ───────────────────────────────

export const KNull = 0;
export const KBool = 1;
export const KNum = 2;
export const KStr = 3;
export const KTime = 4;

/** A datetime: the wall-clock reading (whole seconds since the epoch, read as
 *  UTC, plus nanoseconds) and the UTC offset that reading is in. */
export interface ErbTime {
  sec: number;
  nsec: number;
  offset: number;
}

/** One formula value. `isInt` lets text rendering match the reference
 *  implementation, which renders an integer (3) differently from a float (3.0). */
export class Value {
  constructor(
    readonly k: number,
    readonly b: boolean = false,
    readonly n: number = 0,
    readonly isInt: boolean = false,
    readonly s: string = "",
    readonly t: ErbTime | null = null,
    readonly zoned: boolean = false,
  ) {}
}

export const Null = new Value(KNull);
const TRUE = new Value(KBool, true);
const FALSE = new Value(KBool, false);
const EMPTY = new Value(KStr, false, 0, false, "");

export function vB(b: boolean): Value { return b ? TRUE : FALSE; }
export function vS(s: string): Value { return s === "" ? EMPTY : new Value(KStr, false, 0, false, s); }
export function vI(n: number): Value { return new Value(KNum, false, Math.trunc(n), true); }
export function vF(n: number): Value { return new Value(KNum, false, n, false); }
function vT(t: ErbTime): Value { return new Value(KTime, false, 0, false, "", t, false); }
function vTZ(t: ErbTime): Value { return new Value(KTime, false, 0, false, "", t, true); }

export class ErbError extends Error {}

function fail(message: string): never {
  throw new ErbError(message);
}

export function vStr(p: string | null | undefined): Value { return p == null ? Null : vS(p); }
export function vStrPlain(s: string): Value { return vS(s); }
export function vBool(p: boolean | null | undefined): Value { return p == null ? Null : vB(p); }
export function vBoolPlain(b: boolean): Value { return vB(b); }
export function vInt(p: number | null | undefined): Value { return p == null ? Null : vI(p); }
export function vIntPlain(n: number): Value { return vI(n); }
export function vNum(p: number | null | undefined): Value { return p == null ? Null : numberValue(p); }
export function vNumPlain(n: number): Value { return numberValue(n); }
export function vAny(p: unknown): Value { return fromJSON(p); }

/** Keeps a whole number that arrived from a number column integral. */
function numberValue(n: number): Value {
  if (Number.isFinite(n) && n === Math.trunc(n) && Math.abs(n) < 1e15) return vI(n);
  return vF(n);
}

/** Converts a decoded JSON value into a Value. */
export function fromJSON(x: unknown): Value {
  if (x === null || x === undefined) return Null;
  if (typeof x === "boolean") return vB(x);
  if (typeof x === "string") return vS(x);
  if (typeof x === "number") return Number.isInteger(x) ? vI(x) : vF(x);
  return fail(`unsupported JSON value ${JSON.stringify(x)} in a record field`);
}

// ───────────────────────────── typed boundary ─────────────────────────────

export function toStringPtr(v: Value): string | null {
  if (v.k === KNull) return null;
  if (v.k === KStr) return v.s;
  return pyStr(v);
}

export function toBoolPtr(v: Value): boolean | null {
  switch (v.k) {
    case KNull: return null;
    case KBool: return v.b;
    case KStr: {
      const s = v.s.trim().toLowerCase();
      if (s === "true") return true;
      if (s === "false") return false;
      if (s === "") return null;
      break;
    }
    case KNum: return v.n !== 0;
  }
  return fail(`cannot store ${pyRepr(v)} in a boolean field`);
}

export function toFloatPtr(v: Value): number | null {
  switch (v.k) {
    case KNull: return null;
    case KNum: return v.n;
    case KBool: return v.b ? 1 : 0;
    case KStr: {
      if (v.s.trim() === "") return null;
      const n = toNumber(v);
      if (n !== null) return n.n;
      break;
    }
  }
  return fail(`cannot store ${pyRepr(v)} in a number field`);
}

export function toIntPtr(v: Value): number | null {
  const f = toFloatPtr(v);
  if (f === null) return null;
  return Math.trunc(roundHalfAway(f, 0));
}

export function strPlain(v: Value): string { return toStringPtr(v) ?? ""; }
export function boolPlain(v: Value): boolean { return toBoolPtr(v) ?? false; }
export function intPlain(v: Value): number { return toIntPtr(v) ?? 0; }
export function floatPlain(v: Value): number { return toFloatPtr(v) ?? 0; }
export function anyPlain(v: Value): unknown { return toJSON(v); }

export function toJSON(v: Value): unknown {
  switch (v.k) {
    case KNull: return null;
    case KBool: return v.b;
    case KNum: return v.isInt ? Math.trunc(v.n) : v.n;
    case KStr: return v.s;
    // A computed datetime is written per erbDateTimeText, as the reference
    // implementation's erb_json_value writes it.
    case KTime: return erbDatetimeText(v).s;
  }
  return null;
}

// ───────────────────────────── text rendering ─────────────────────────────

function pad(n: number, width: number): string {
  const s = String(Math.abs(n));
  return (n < 0 ? "-" : "") + (s.length >= width ? s : "0".repeat(width - s.length) + s);
}

function wallText(t: ErbTime): string {
  const d = new Date(t.sec * 1000);
  return `${pad(d.getUTCFullYear(), 4)}-${pad(d.getUTCMonth() + 1, 2)}-${pad(d.getUTCDate(), 2)} ` +
    `${pad(d.getUTCHours(), 2)}:${pad(d.getUTCMinutes(), 2)}:${pad(d.getUTCSeconds(), 2)}`;
}

/** Renders a value as str() does in the reference implementation. */
export function pyStr(v: Value): string {
  switch (v.k) {
    case KNull: return "None";
    case KBool: return v.b ? "True" : "False";
    case KNum: return v.isInt ? String(Math.trunc(v.n)) : pyFloatRepr(v.n);
    case KStr: return v.s;
    case KTime: {
      const t = v.t as ErbTime;
      let s = wallText(t);
      if (t.nsec !== 0) s += "." + pad(Math.trunc(t.nsec / 1000), 6);
      if (v.zoned) {
        const sign = t.offset < 0 ? "-" : "+";
        const off = Math.abs(t.offset);
        s += `${sign}${pad(Math.trunc(off / 3600), 2)}:${pad(Math.trunc((off % 3600) / 60), 2)}`;
      }
      return s;
    }
  }
  return "";
}

function pyRepr(v: Value): string {
  return v.k === KStr ? JSON.stringify(v.s) : pyStr(v);
}

/** Python's repr() of a float: shortest round-trip digits, exponent notation
 *  below 1e-4 and from 1e16. */
export function pyFloatRepr(f: number): string {
  if (f === Infinity) return "inf";
  if (f === -Infinity) return "-inf";
  if (Number.isNaN(f)) return "nan";
  if (f === 0) return Object.is(f, -0) ? "-0.0" : "0.0";
  const [mantissa, expText] = f.toExponential().split("e");
  const exp = Number(expText);
  if (exp < -4 || exp >= 16) {
    const sign = exp < 0 ? "-" : "+";
    return `${mantissa}e${sign}${pad(Math.abs(exp), 2)}`;
  }
  const s = fixedShortest(f);
  return s.includes(".") ? s : s + ".0";
}

/** The shortest round-trip digits of f in fixed notation (no exponent). */
function fixedShortest(f: number): string {
  const negative = f < 0;
  const [mantissa, expText] = Math.abs(f).toExponential().split("e");
  const exp = Number(expText);
  const digits = mantissa.replace(".", "");
  const point = exp + 1; // decimal point position within digits
  let out: string;
  if (point <= 0) out = "0." + "0".repeat(-point) + digits;
  else if (point >= digits.length) out = digits + "0".repeat(point - digits.length);
  else out = digits.slice(0, point) + "." + digits.slice(point);
  return (negative ? "-" : "") + out;
}

/** Python truthiness, used where compiled code reads `x or ""`. */
function truthy(v: Value): boolean {
  switch (v.k) {
    case KNull: return false;
    case KBool: return v.b;
    case KNum: return v.n !== 0;
    case KStr: return v.s !== "";
  }
  return true;
}

/** `str(x or "")`. */
export function erbTextOr(v: Value): Value { return truthy(v) ? vS(pyStr(v)) : EMPTY; }

/** `str(x if x is not None else "")`. */
export function erbTextNotNull(v: Value): Value { return v.k === KNull ? EMPTY : vS(pyStr(v)); }

export function erbConcat(...parts: Value[]): Value {
  let out = "";
  for (const p of parts) out += p.s;
  return vS(out);
}

// ───────────────────────────── numbers ─────────────────────────────

const INT_PATTERN = /^[+-]?\d+$/;
const FLOAT_PATTERN = /^[+-]?(?:(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?|inf(?:inity)?|nan)$/i;

/** Numbers and numeric strings are numbers; booleans and anything else are not. */
function toNumber(v: Value): Value | null {
  if (v.k === KNum) return v;
  if (v.k === KStr) {
    const s = v.s.trim();
    if (s === "") return null;
    if (INT_PATTERN.test(s)) return vI(Number(s));
    if (FLOAT_PATTERN.test(s)) {
      const lower = s.toLowerCase().replace(/^\+/, "");
      if (lower.includes("inf")) return vF(lower.startsWith("-") ? -Infinity : Infinity);
      if (lower.includes("nan")) return vF(NaN);
      return vF(Number(s));
    }
  }
  return null;
}

function numOrZero(v: Value): Value { return toNumber(v) ?? vI(0); }

function arith(a: Value, b: Value, op: "+" | "-" | "*"): Value {
  const isInt = a.isInt && b.isInt;
  const n = op === "+" ? a.n + b.n : op === "-" ? a.n - b.n : a.n * b.n;
  return new Value(KNum, false, n, isInt);
}

export function erbNeg(v: Value): Value {
  if (!truthy(v)) return vI(0);
  if (v.k !== KNum) fail(`bad operand type for unary -: ${pyRepr(v)}`);
  return new Value(KNum, false, -v.n, v.isInt);
}

export function erbAdd(a: Value, b: Value): Value {
  const at = dateOrNone(a);
  const bt = dateOrNone(b);
  if (at && !bt) return vT(addNanos(at, daysDeltaNanos(b)));
  if (bt && !at) return vT(addNanos(bt, daysDeltaNanos(a)));
  return arith(numOrZero(a), numOrZero(b), "+");
}

export function erbSub(a: Value, b: Value): Value {
  const at = dateOrNone(a);
  if (at && !dateOrNone(b)) return vT(addNanos(at, -daysDeltaNanos(b)));
  return arith(numOrZero(a), numOrZero(b), "-");
}

export function erbMul(a: Value, b: Value): Value { return arith(numOrZero(a), numOrZero(b), "*"); }

export function erbDiv(a: Value, b: Value): Value {
  const d = numOrZero(b);
  if (d.n === 0) return Null;
  return vF(numOrZero(a).n / d.n);
}

/** A day count as nanoseconds, truncated as Go's time.Duration conversion truncates. */
function daysDeltaNanos(v: Value): number {
  let f = 0;
  if (truthy(v)) {
    const n = toNumber(v);
    if (n === null) return 0;
    f = n.n;
  }
  return Math.trunc(f * 86400e9);
}

/** Rounds the shortest decimal representation of f to `digits` places, toward
 *  +inf when `ceiling` is set and half away from zero otherwise —
 *  Decimal(str(x)).quantize(...), so 2.675 rounds as the text "2.675" reads. */
function decimalRound(f: number, digits: number, ceiling: boolean): number {
  let text = fixedShortest(f);
  const negative = text.startsWith("-");
  if (negative) text = text.slice(1);
  const [intPart, frac = ""] = text.split(".");
  let all = intPart + frac;
  let point = intPart.length;
  let keep = point + digits;
  if (keep < 0) {
    all = "0".repeat(-keep) + all;
    point += -keep;
    keep = 0;
  }
  while (all.length < keep) all += "0";
  const kept = all.slice(0, keep);
  const rest = all.slice(keep);
  const roundUp = ceiling
    ? !negative && /[^0]/.test(rest)
    : rest.length > 0 && rest[0] >= "5";
  const result = kept.split("");
  if (roundUp) {
    let i = result.length - 1;
    for (; i >= 0; i--) {
      if (result[i] === "9") { result[i] = "0"; continue; }
      result[i] = String(Number(result[i]) + 1);
      break;
    }
    if (i < 0) { result.unshift("1"); point++; }
  }
  const whole = result.join("");
  let out: string;
  if (point >= whole.length) out = whole + "0".repeat(point - whole.length);
  else if (point <= 0) out = "0." + "0".repeat(-point) + whole;
  else out = whole.slice(0, point) + "." + whole.slice(point);
  if (out === "") out = "0";
  const r = Number(out);
  if (Number.isNaN(r)) fail(`rounding ${f}: ${out} is not a number`);
  return negative ? -r : r;
}

function roundHalfAway(f: number, digits: number): number { return decimalRound(f, digits, false); }

function roundDigits(v: Value): number {
  if (!truthy(v) && v.k !== KNum) return 0;
  const n = toNumber(v);
  if (n === null) fail(`invalid ROUND digits ${pyRepr(v)}`);
  return Math.trunc(n.n);
}

export function erbRound(v: Value, digits: Value): Value {
  if (v.k === KNull || (v.k === KStr && v.s === "")) return Null;
  const n = toNumber(v);
  if (n === null) fail(`cannot ROUND ${pyRepr(v)}`);
  return vF(decimalRound(n.n, roundDigits(digits), false));
}

export function erbRoundup(v: Value, digits: Value): Value {
  if (v.k === KNull || (v.k === KStr && v.s === "")) return Null;
  const n = toNumber(v);
  if (n === null) fail(`cannot ROUNDUP ${pyRepr(v)}`);
  return vF(decimalRound(n.n, roundDigits(digits), true));
}

/** The oracle's ::integer cast on a field declared integer. */
export function erbInteger(v: Value): Value {
  if (v.k === KNull || v.k === KBool) return v;
  const n = toNumber(v);
  if (n === null) return v;
  return vI(roundHalfAway(n.n, 0));
}

function orZero(v: Value): Value {
  if (!truthy(v)) return vI(0);
  if (v.k !== KNum) fail(`expected a number, got ${pyRepr(v)}`);
  return v;
}

export function erbAbs(v: Value): Value {
  const x = orZero(v);
  return new Value(KNum, false, Math.abs(x.n), x.isInt);
}

export function erbPower(b: Value, e: Value): Value {
  const x = orZero(b);
  const y = orZero(e);
  return new Value(KNum, false, Math.pow(x.n, y.n), x.isInt && y.isInt && y.n >= 0);
}

export function erbSqrt(v: Value): Value { return vF(Math.sqrt(orZero(v).n)); }
export function erbTan(v: Value): Value { return vF(Math.tan(orZero(v).n)); }
export function erbLog10(v: Value): Value { return vF(Math.log10(orZero(v).n)); }
export function erbLog(v: Value, base: Value): Value { return vF(Math.log(orZero(v).n) / Math.log(orZero(base).n)); }

export function erbMaxMin(max: boolean, ...args: Value[]): Value {
  let best = orZero(args[0]);
  for (const a of args.slice(1)) {
    const x = orZero(a);
    if ((max && x.n > best.n) || (!max && x.n < best.n)) best = x;
  }
  return best;
}

export function erbSum(...args: Value[]): Value {
  let total = vI(0);
  for (const a of args) total = arith(total, numOrZero(a), "+");
  return total;
}

export const erbPi = Math.PI;

// ───────────────────────────── build parameters ─────────────────────────────
//
// The ERB build parameters (docs/ERB-BUILD-PARAMETERS.md) this SDK was
// generated under. erb_sdk.ts calls erbConfigure() as it loads; every helper
// below reads erbParam(), and nothing runs unconfigured. erb_runtime.py is the
// reference; when a rule changes there, it changes here.

const erbParameterValues: Record<string, string[]> = {
  erbDateDiff: ["calendar", "elapsed"],
  erbTimezone: ["UTC"], // or any IANA zone name
  erbDateTimeText: ["iso8601", "sql"],
  erbBlankLogic: ["coerce", "propagate"],
  erbWholeNumber: ["by-field-type", "integer", "decimal"],
};

let erbParams: Record<string, string> | null = null;
let erbZoneFormatter: Intl.DateTimeFormat | null = null;

export function erbConfigure(params: Record<string, string>): void {
  for (const name of Object.keys(params)) {
    if (!(name in erbParameterValues)) fail(`erbConfigure: unknown ERB build parameter ${JSON.stringify(name)}`);
  }
  for (const [name, allowed] of Object.entries(erbParameterValues)) {
    const value = params[name];
    if (value === undefined) fail(`erbConfigure: missing ERB build parameter ${JSON.stringify(name)}`);
    if (name !== "erbTimezone" && !allowed.includes(value)) {
      fail(`erbConfigure: ${name}=${JSON.stringify(value)} is not one of ${JSON.stringify(allowed)}`);
    }
  }
  if (params.erbWholeNumber !== "by-field-type") {
    // A typed row field is typed by its declared datatype; there is no honest
    // rendering of the other two values here.
    fail(`erbConfigure: the TypeScript substrate honours erbWholeNumber=by-field-type only, not ${JSON.stringify(params.erbWholeNumber)}`);
  }
  const zone = params.erbTimezone;
  if (zone === "UTC") {
    erbZoneFormatter = null;
  } else {
    try {
      erbZoneFormatter = new Intl.DateTimeFormat("en-US", {
        timeZone: zone, hourCycle: "h23", year: "numeric", month: "2-digit", day: "2-digit",
        hour: "2-digit", minute: "2-digit", second: "2-digit",
      });
    } catch (e) {
      fail(`erbConfigure: erbTimezone=${JSON.stringify(zone)} cannot be resolved: ${(e as Error).message}`);
    }
  }
  erbParams = { ...params };
}

function erbParam(name: string): string {
  if (erbParams === null) fail("erb_runtime is not configured: erb_sdk.ts must call erbConfigure() before any formula runs");
  return erbParams[name];
}

function coerceBlanks(): boolean { return erbParam("erbBlankLogic") === "coerce"; }

/** The UTC offset, in seconds, erbTimezone has at the instant `instantSec`. */
function zoneOffsetAt(instantSec: number): number {
  erbParam("erbTimezone");
  if (erbZoneFormatter === null) return 0;
  const parts = erbZoneFormatter.formatToParts(new Date(instantSec * 1000));
  const get = (type: string) => Number(parts.find((p) => p.type === type)?.value ?? "0");
  const wall = Date.UTC(get("year"), get("month") - 1, get("day"), get("hour") % 24, get("minute"), get("second")) / 1000;
  return Math.round(wall - instantSec);
}

/** The instant v names, expressed in erbTimezone (an ErbTime whose wall reading
 *  and offset are the zone's). A value carrying no offset is taken to be in
 *  erbTimezone. */
function inZone(v: Value): ErbTime {
  let t: ErbTime;
  let zoned: boolean;
  if (v.k === KTime) {
    t = v.t as ErbTime;
    zoned = v.zoned;
  } else if (v.k === KStr) {
    const parsed = parseISO(v.s.trim());
    if (parsed) {
      t = parsed.t;
      zoned = parsed.hasZone;
    } else {
      const date = parseDateOnly(v.s.trim().slice(0, 10));
      if (!date) return fail(`cannot read ${pyRepr(v)} as a datetime`);
      t = date;
      zoned = false;
    }
  } else {
    return fail(`cannot read ${pyRepr(v)} as a datetime`);
  }
  // A naive reading is in erbTimezone: find the instant whose zone reading is t.
  const instant = zoned ? t.sec - t.offset : t.sec - zoneOffsetAt(t.sec - zoneOffsetAt(t.sec));
  const offset = zoneOffsetAt(instant);
  return { sec: instant + offset, nsec: t.nsec, offset };
}

// ───────────────────────────── logic ─────────────────────────────

export function erbBool3(v: Value): Value {
  return v.k === KNull || v.k === KBool ? v : FALSE;
}

export function erbIsTrue(v: Value): Value { return vB(v.k === KBool && v.b); }
export function erbHasValue(v: Value): Value { return vB(!isBlank(v)); }

/** AND: under erbBlankLogic=coerce a blank operand is FALSE; under propagate
 *  FALSE if any operand is FALSE, else NULL if any is NULL, else TRUE. */
export function erbAnd(...values: Value[]): Value {
  let sawNull = false;
  for (const v of values) {
    if (v.k === KBool && !v.b) return FALSE;
    if (v.k === KNull) sawNull = true;
  }
  if (sawNull) return coerceBlanks() ? FALSE : Null;
  return TRUE;
}

export function erbOr(...values: Value[]): Value {
  let sawNull = false;
  for (const v of values) {
    if (v.k === KBool && v.b) return TRUE;
    if (v.k === KNull) sawNull = true;
  }
  if (sawNull) return coerceBlanks() ? FALSE : Null;
  return FALSE;
}

/** NOT: coerce makes NOT(blank) TRUE; propagate keeps it NULL. */
export function erbNot(v: Value): Value {
  if (v.k === KNull) return coerceBlanks() ? TRUE : Null;
  return vB(!truthy(v));
}

export function erbIf(cond: Value, then: () => Value, otherwise: () => Value): Value {
  return truthy(cond) ? then() : otherwise();
}

function isBlank(v: Value): boolean { return v.k === KNull || (v.k === KStr && v.s === ""); }

export function erbIsBlank(v: Value): Value { return vB(isBlank(v)); }
export function erbIsNotBlank(v: Value): Value { return vB(!isBlank(v)); }

export function erbNullif(v: Value): Value { return v.k === KStr && v.s === "" ? Null : v; }

/** Numbers and booleans compare numerically under Python ==. */
function numericLike(v: Value): number | null {
  if (v.k === KNum) return v.n;
  if (v.k === KBool) return v.b ? 1 : 0;
  return null;
}

/** Python ==. */
function pyEqual(a: Value, b: Value): boolean {
  const an = numericLike(a);
  const bn = numericLike(b);
  if (an !== null && bn !== null) return an === bn;
  if (a.k !== b.k) return false;
  switch (a.k) {
    case KNull: return true;
    case KStr: return a.s === b.s;
    case KTime: return compareInstants(a.t as ErbTime, b.t as ErbTime) === 0;
  }
  return false;
}

/** Under coerce a blank compared against `other` takes the other side's zero:
 *  FALSE against a boolean, 0 against a number, "" otherwise. */
function blankAsZeroOf(other: Value): Value {
  if (other.k === KBool) return FALSE;
  if (toNumber(other) !== null) return vI(0);
  return EMPTY;
}

/** Resolves blank operands per erbBlankLogic: the pair to compare, or null when
 *  the blank must propagate as NULL. */
function coercedPair(a: Value, b: Value): [Value, Value] | null {
  if (a.k === KNull && b.k === KNull) return coerceBlanks() ? [EMPTY, EMPTY] : null;
  if (a.k === KNull || b.k === KNull) {
    if (!coerceBlanks()) return null;
    return a.k === KNull ? [blankAsZeroOf(b), b] : [a, blankAsZeroOf(a)];
  }
  return [a, b];
}

export function erbEq(a: Value, b: Value): Value {
  const pair = coercedPair(a, b);
  if (pair === null) return Null;
  return vB(pyEqual(pair[0], pair[1]));
}

export function erbNe(a: Value, b: Value): Value {
  const pair = coercedPair(a, b);
  if (pair === null) return Null;
  return vB(!pyEqual(pair[0], pair[1]));
}

function compareStrings(a: string, b: string): number {
  return a < b ? -1 : a > b ? 1 : 0;
}

/** An ordered comparison: numbers and numeric strings as numbers, two strings as
 *  strings, two booleans as booleans; NULL makes it NULL and anything else cannot
 *  be ordered and is FALSE. */
export function erbCmp(a: Value, op: string, b: Value): Value {
  const pair = coercedPair(a, b);
  if (pair === null) return Null;
  [a, b] = pair;
  let c: number;
  const an = toNumber(a);
  const bn = toNumber(b);
  if (an !== null && bn !== null) c = an.n < bn.n ? -1 : an.n > bn.n ? 1 : 0;
  else if (a.k === KStr && b.k === KStr) c = compareStrings(a.s, b.s);
  else if (a.k === KBool && b.k === KBool) c = Number(a.b) - Number(b.b);
  else return FALSE;
  switch (op) {
    case "<": return vB(c < 0);
    case "<=": return vB(c <= 0);
    case ">": return vB(c > 0);
  }
  return vB(c >= 0);
}

export function erbCoalesce(...values: Value[]): Value {
  for (const v of values) if (!isBlank(v)) return v;
  return Null;
}

export function erbTry(main: () => Value, fallback: () => Value): Value {
  try {
    return main();
  } catch {
    return fallback();
  }
}

export function erbIsError(main: () => Value): Value {
  try {
    main();
    return FALSE;
  } catch {
    return TRUE;
  }
}

// ───────────────────────────── strings ─────────────────────────────

function textOperand(v: Value, fn: string): string {
  if (!truthy(v)) return "";
  if (v.k !== KStr) fail(`${fn} expects text, got ${pyRepr(v)}`);
  return v.s;
}

function countOperand(v: Value, fn: string): number {
  if (!truthy(v)) return 0;
  if (v.k !== KNum || !v.isInt) fail(`${fn} expects an integer count, got ${pyRepr(v)}`);
  return Math.trunc(v.n);
}

export function erbLower(v: Value): Value { return vS(textOperand(v, "LOWER").toLowerCase()); }
export function erbUpper(v: Value): Value { return vS(textOperand(v, "UPPER").toUpperCase()); }
export function erbTrim(v: Value): Value { return vS(textOperand(v, "TRIM").replace(/^ +| +$/g, "")); }
export function erbLen(v: Value): Value { return vI(Array.from(textOperand(v, "LEN")).length); }

/** Clamps like a Python slice with non-negative-normalised bounds. */
function pySlice(r: string[], lo: number, hi: number): string[] {
  if (hi < 0) hi = Math.max(hi + r.length, 0);
  if (lo < 0) lo = 0;
  if (hi > r.length) hi = r.length;
  if (lo > hi) return [];
  return r.slice(lo, hi);
}

export function erbLeft(text: Value, count: Value): Value {
  const r = Array.from(textOperand(text, "LEFT"));
  return vS(pySlice(r, 0, countOperand(count, "LEFT")).join(""));
}

export function erbRight(text: Value, count: Value): Value {
  const r = Array.from(textOperand(text, "RIGHT"));
  const n = countOperand(count, "RIGHT");
  if (n <= 0) return EMPTY;
  return vS(pySlice(r, r.length - n, r.length).join(""));
}

export function erbMid(text: Value, start: Value, count: Value): Value {
  const r = Array.from(textOperand(text, "MID"));
  const n = countOperand(count, "MID");
  const s = truthy(start) ? Math.trunc(numOrZero(start).n) : 1;
  const i = Math.max(s - 1, 0);
  if (n <= 0) return EMPTY;
  return vS(pySlice(r, i, i + n).join(""));
}

export function erbSubstitute(text: Value, oldText: Value, newText: Value): Value {
  if (oldText.k !== KStr || newText.k !== KStr) fail("SUBSTITUTE expects text arguments");
  const s = textOperand(text, "SUBSTITUTE");
  if (oldText.s === "") {
    // strings.ReplaceAll inserts the replacement before every character and at the end.
    return vS(newText.s + Array.from(s).map(ch => ch + newText.s).join(""));
  }
  return vS(s.split(oldText.s).join(newText.s));
}

export function erbFind(needle: Value, haystack: Value): Value {
  if (needle.k === KNull || haystack.k === KNull) return Null;
  const h = pyStr(haystack);
  const i = h.indexOf(pyStr(needle));
  if (i < 0) return vI(0);
  return vI(Array.from(h.slice(0, i)).length + 1);
}

export function erbCast(v: Value): Value { return truthy(v) ? vS(pyStr(v)) : EMPTY; }

// ───────────────────────────── dates ─────────────────────────────

const ISO_PATTERN =
  /^(\d{4})-(\d{2})-(\d{2})(?:[T ](\d{2})(?::(\d{2})(?::(\d{2})(?:[.,](\d+))?)?)?)?\s*(Z|[+-]\d{2}(?::?\d{2}(?::?\d{2})?)?)?$/;

/** Builds a time from calendar fields, normalising out-of-range fields the way
 *  Go's time.Date does. */
function makeTime(y: number, mo: number, d: number, h: number, mi: number, s: number, nsec: number, offset: number): ErbTime {
  // setUTCFullYear, unlike Date.UTC, does not read years 0-99 as 1900-1999.
  const date = new Date(0);
  date.setUTCFullYear(y, mo - 1, d);
  date.setUTCHours(h, mi, s, 0);
  return { sec: Math.floor(date.getTime() / 1000), nsec, offset };
}

/** Accepts what datetime.fromisoformat accepts for these values. */
function parseISO(text: string): { t: ErbTime; hasZone: boolean } | null {
  const m = ISO_PATTERN.exec(text.trim());
  if (!m) return null;
  const num = (x: string | undefined) => (x ? Number(x) : 0);
  const nsec = m[7] ? Number((m[7] + "000000000").slice(0, 9)) : 0;
  let offset = 0;
  let hasZone = false;
  if (m[8]) {
    hasZone = true;
    if (m[8] !== "Z") {
      const sign = m[8][0] === "-" ? -1 : 1;
      const digits = m[8].slice(1).replace(/:/g, "");
      const h = num(digits.slice(0, 2));
      const mi = digits.length >= 4 ? num(digits.slice(2, 4)) : 0;
      const sec = digits.length >= 6 ? num(digits.slice(4, 6)) : 0;
      offset = sign * (h * 3600 + mi * 60 + sec);
    }
  }
  return { t: makeTime(num(m[1]), num(m[2]), num(m[3]), num(m[4]), num(m[5]), num(m[6]), nsec, offset), hasZone };
}

/** Drops the zone, keeping the local reading — the reference implementation
 *  strips tzinfo before any date arithmetic. */
function wallClock(t: ErbTime): ErbTime { return { sec: t.sec, nsec: t.nsec, offset: 0 }; }

function addNanos(t: ErbTime, nanos: number): ErbTime {
  const wholeSeconds = Math.trunc(nanos / 1e9);
  let sec = t.sec + wholeSeconds;
  let nsec = t.nsec + Math.round(nanos - wholeSeconds * 1e9);
  if (nsec >= 1e9) { sec += 1; nsec -= 1e9; }
  if (nsec < 0) { sec -= 1; nsec += 1e9; }
  return { sec, nsec, offset: t.offset };
}

function compareInstants(a: ErbTime, b: ErbTime): number {
  const as = a.sec - a.offset;
  const bs = b.sec - b.offset;
  if (as !== bs) return as < bs ? -1 : 1;
  return a.nsec === b.nsec ? 0 : a.nsec < b.nsec ? -1 : 1;
}

/** (a - b) in seconds, as Go's Duration.Seconds() reports it. */
function secondsBetween(a: ErbTime, b: ErbTime): number {
  return (a.sec - a.offset) - (b.sec - b.offset) + (a.nsec - b.nsec) / 1e9;
}

/** A calendar date "YYYY-MM-DD" that time.Parse accepts. */
function parseDateOnly(s: string): ErbTime | null {
  const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(s);
  if (!m) return null;
  const y = Number(m[1]);
  const mo = Number(m[2]);
  const d = Number(m[3]);
  if (mo < 1 || mo > 12 || d < 1) return null;
  const daysInMonth = new Date(Date.UTC(y, mo, 0)).getUTCDate();
  if (d > daysInMonth) return null;
  return makeTime(y, mo, d, 0, 0, 0, 0, 0);
}

const DATE_PREFIX = /^\d{4}-\d{2}-\d{2}/;

/** erb_date_or_none: only text that starts YYYY-MM-DD is a date. */
function dateOrNone(v: Value): ErbTime | null {
  if (v.k !== KStr) return null;
  const s = v.s.trim();
  if (!DATE_PREFIX.test(s)) return null;
  const parsed = parseISO(s);
  if (parsed) return wallClock(parsed.t);
  return parseDateOnly(s.slice(0, 10));
}

/** Whole calendar days since the epoch of t's wall reading (t is in erbTimezone). */
function calendarDays(t: ErbTime): number { return Math.floor(t.sec / 86400); }

/** DATETIME_DIFF(end, start, unit) per erbDateDiff: hour, minute and second are
 *  exact elapsed (possibly fractional) in both modes; month and year are whole
 *  calendar months (AGE()) in both. calendar: day is calendar dates in
 *  erbTimezone subtracted, week is day/7 truncated. elapsed: day is
 *  seconds/86400 truncated, week seconds/604800 truncated. */
export function erbDatetimeDiff(end: Value, start: Value, unitValue: Value): Value {
  let unit = "day";
  if (truthy(unitValue)) unit = pyStr(unitValue).toLowerCase().replace(/s+$/, "");
  if (isBlank(end) || isBlank(start)) return Null;
  const e = inZone(end);
  const s = inZone(start);
  const seconds = secondsBetween(e, s);
  const calendar = erbParam("erbDateDiff") === "calendar";
  const days = () => calendarDays(e) - calendarDays(s);
  switch (unit) {
    case "hour": return vF(seconds / 3600);
    case "minute": return vF(seconds / 60);
    case "second": return vF(seconds);
    case "day": return vI(calendar ? days() : Math.trunc(seconds / 86400));
    case "week": return vI(calendar ? Math.trunc(days() / 7) : Math.trunc(seconds / 604800));
    case "month":
    case "year": {
      const ed = new Date(e.sec * 1000);
      const sd = new Date(s.sec * 1000);
      let months = (ed.getUTCFullYear() - sd.getUTCFullYear()) * 12 + ed.getUTCMonth() - sd.getUTCMonth();
      const eClock = [ed.getUTCDate(), ed.getUTCHours(), ed.getUTCMinutes(), ed.getUTCSeconds()];
      const sClock = [sd.getUTCDate(), sd.getUTCHours(), sd.getUTCMinutes(), sd.getUTCSeconds()];
      for (let i = 0; i < 4; i++) {
        if (eClock[i] !== sClock[i]) {
          if (eClock[i] < sClock[i]) months--;
          break;
        }
      }
      return vI(unit === "month" ? months : Math.trunc(months / 12));
    }
  }
  return fail(`DATETIME_DIFF: unsupported unit ${JSON.stringify(unit)}`);
}

/** NOW()/TODAY(), an instant in erbTimezone. FORMULA_NOW pins it. */
export function erbNow(): Value {
  const override = process.env.FORMULA_NOW;
  if (override) {
    if (!parseISO(override)) fail(`FORMULA_NOW ${JSON.stringify(override)} is not an ISO-8601 datetime`);
    return vTZ(inZone(vS(override)));
  }
  const ms = Date.now();
  const instant = Math.floor(ms / 1000);
  const offset = zoneOffsetAt(instant);
  return vTZ({ sec: instant + offset, nsec: (ms % 1000) * 1e6, offset });
}

const DATE_ONLY_TEXT = /^\d{4}-\d{2}-\d{2}$/;

/** A datetime rendered into text per erbDateTimeText, in erbTimezone:
 *  iso8601 = 2026-04-03T14:00:00+00:00, sql = 2026-04-03 14:00:00+00. A
 *  date-only value renders as YYYY-MM-DD; a blank as "". */
export function erbDatetimeText(v: Value): Value {
  if (isBlank(v)) return EMPTY;
  if (v.k === KStr && DATE_ONLY_TEXT.test(v.s.trim())) return vS(v.s.trim());
  if (v.k !== KTime && v.k !== KStr) return fail(`cannot render ${pyRepr(v)} as a datetime`);
  const t = inZone(v);
  const iso = erbParam("erbDateTimeText") === "iso8601";
  let s = wallText(t);
  if (iso) s = s.replace(" ", "T");
  if (t.nsec !== 0) s += "." + pad(Math.trunc(t.nsec / 1000), 6).replace(/0+$/, "");
  const sign = t.offset < 0 ? "-" : "+";
  const off = Math.abs(t.offset);
  const minutes = Math.trunc((off % 3600) / 60);
  if (iso) {
    s += `${sign}${pad(Math.trunc(off / 3600), 2)}:${pad(minutes, 2)}`;
  } else {
    s += `${sign}${pad(Math.trunc(off / 3600), 2)}`;
    if (minutes !== 0) s += `:${pad(minutes, 2)}`;
  }
  return vS(s);
}

/** A non-datetime CONCAT / `&` operand as text: a blank contributes nothing, a
 *  number renders in its shortest exact form with no trailing .0, a boolean as
 *  true/false, a datetime per erbDateTimeText. */
export function erbText(v: Value): Value {
  switch (v.k) {
    case KNull: return EMPTY;
    case KBool: return vS(v.b ? "true" : "false");
    case KNum:
      if (v.isInt || (Number.isFinite(v.n) && v.n === Math.trunc(v.n) && Math.abs(v.n) < 1e15)) return vS(String(Math.trunc(v.n)));
      return vS(pyFloatRepr(v.n));
    case KTime: return erbDatetimeText(v);
  }
  return vS(v.s);
}

// ───────────────────────────── transitive closure ─────────────────────────────

export interface ClosureRow {
  from_id: string;
  to_id: string;
  hop_distance: number;
  is_inferred: boolean;
}

/** Every reachable pair with its shortest hop distance, flagging pairs no
 *  directly-asserted edge states — vw_<entity>_closure. An empty endpoint is not
 *  an edge. Cyclic edge sets terminate. */
export function computeClosureRelation(edges: Array<[string, string]>): ClosureRow[] {
  const asserted = new Set<string>();
  const adjacency = new Map<string, string[]>();
  const origins: string[] = [];
  for (const [from, to] of edges) {
    if (from === "" || to === "") continue;
    if (!adjacency.has(from)) {
      origins.push(from);
      adjacency.set(from, []);
    }
    asserted.add(JSON.stringify([from, to]));
    (adjacency.get(from) as string[]).push(to);
  }
  if (asserted.size === 0) return [];

  const shortest = new Map<string, { from: string; to: string; hop: number }>();
  for (const origin of origins) {
    // Breadth-first, so the first arrival at a node is the shortest derivation;
    // carrying the visited path makes cyclic edge sets terminate.
    let frontier: Array<{ node: string; path: Set<string> }> = [{ node: origin, path: new Set([origin]) }];
    for (let hop = 1; frontier.length > 0; hop++) {
      const next: Array<{ node: string; path: Set<string> }> = [];
      for (const w of frontier) {
        for (const neighbor of adjacency.get(w.node) ?? []) {
          const key = JSON.stringify([origin, neighbor]);
          if (!shortest.has(key)) shortest.set(key, { from: origin, to: neighbor, hop });
          if (w.path.has(neighbor)) continue;
          const extended = new Set(w.path);
          extended.add(neighbor);
          next.push({ node: neighbor, path: extended });
        }
      }
      frontier = next;
    }
  }

  const rows: ClosureRow[] = [];
  for (const [key, p] of shortest) {
    rows.push({ from_id: p.from, to_id: p.to, hop_distance: p.hop, is_inferred: !asserted.has(key) });
  }
  rows.sort((a, b) => compareStrings(a.from_id, b.from_id) || compareStrings(a.to_id, b.to_id));
  return rows;
}

// ───────────────────────────── records & tables ─────────────────────────────

/** How a field is stored: a nullable (`*`) or plain string/bool/int/float64, or
 *  `any` for a closure column. The names are erb_runtime.go's. */
export type FieldType = "*string" | "string" | "*bool" | "bool" | "*int" | "int" | "*float64" | "float64" | "any";

export type Row = Record<string, unknown> & { _erb_errors?: Record<string, string> };

const READERS: Record<FieldType, (x: any) => Value> = {
  "*string": vStr, "string": vStrPlain, "*bool": vBool, "bool": vBoolPlain,
  "*int": vInt, "int": vIntPlain, "*float64": vNum, "float64": vNumPlain, "any": vAny,
};

const WRITERS: Record<FieldType, (v: Value) => unknown> = {
  "*string": toStringPtr, "string": strPlain, "*bool": toBoolPtr, "bool": boolPlain,
  "*int": toIntPtr, "int": intPlain, "*float64": toFloatPtr, "float64": floatPlain, "any": anyPlain,
};

const ZERO: Record<FieldType, unknown> = {
  "*string": null, "string": "", "*bool": null, "bool": false,
  "*int": null, "int": 0, "*float64": null, "float64": 0, "any": null,
};

/** =INDEX(Target!{{Return}}, MATCH({{Key}}, Target!{{Match}}, 0)) or
 *  =LOOKUP(Target!{{Return}}, {{Key}}, Target!{{Pk}}). */
export interface LookupSpec {
  field: string;
  target?: string;
  ret?: string;
  key?: string;
  match?: string;
  error?: string;
}

/** One (range, criteria) pair of a COUNTIFS-family call. */
export interface Criterion {
  range: string;
  kind: "field" | "literal" | "op";
  field?: string;
  literal?: Value;
  op?: string;
}

/** A COUNTIFS / SUMIFS / AVERAGEIFS / MINIFS / MAXIFS call, a bare
 *  SUM/AVERAGE/COUNT/MIN/MAX over a table, or — with `scalar` set — a scalar
 *  formula wrapped around such calls, computed into placeholders first. */
export interface AggregateSpec {
  field: string;
  op?: "COUNTIFS" | "SUM" | "AVERAGE" | "COUNT" | "MIN" | "MAX" | "COMPOSITE";
  table?: string;
  target?: string;
  criteria?: Criterion[];
  suffix?: (v: Value) => Value;
  parts?: AggregateSpec[];
  scalar?: (row: any, aggregates: Record<string, Value>) => Value;
  error?: string;
}

/** Materializes vw_<entity>_closure. `filter`, when set, names the edge table
 *  field an edge must hold TRUE in to take part (the closure field's
 *  EdgeFilterColumn); the view is then vw_<entity>_closure_where_<filter>. */
export interface ClosureSpec {
  view: string;
  source: string;
  from: string;
  to: string;
  filter: string | null;
}

export interface TableSpec {
  name: string;
  file: string;
  /** Rows the rulebook itself holds for this table. */
  rulebookRows: number;
  fields: Record<string, FieldType>;
  compute: (row: any) => void;
  lookups: LookupSpec[];
  aggregations: AggregateSpec[];
}

export function erbErrors(row: Row): Record<string, string> {
  if (!row._erb_errors) row._erb_errors = {};
  return row._erb_errors;
}

function errorText(e: unknown): string {
  return e instanceof Error ? e.message : String(e);
}

/** Runs one calculated field; a failure nulls that field alone and is recorded
 *  on the row, never swallowed. */
export function calcGuard(row: object, types: Record<string, FieldType>, field: string, compute: () => void): void {
  try {
    compute();
  } catch (e) {
    const r = row as Row;
    r[field] = WRITERS[types[field]](Null);
    erbErrors(r)[field] = errorText(e);
  }
}

/** A row of the table with every field at its zero value. */
export function newRow(spec: { fields: Record<string, FieldType> }): Row {
  const row: Row = {};
  for (const [field, type] of Object.entries(spec.fields)) row[field] = ZERO[type];
  return row;
}

/** Reads rows from a JSON array, keeping only the table's own fields. */
export function loadRows(file: string, spec: { fields: Record<string, FieldType> }): Row[] {
  let raw: unknown;
  try {
    raw = JSON.parse(fs.readFileSync(file, "utf-8"));
  } catch (e) {
    return fail(`reading ${file}: ${errorText(e)}`);
  }
  if (!Array.isArray(raw)) fail(`${file} does not hold a JSON array of rows`);
  return raw.map((m: Record<string, unknown>) => {
    const row = newRow(spec);
    for (const [key, value] of Object.entries(m)) {
      const type = spec.fields[key];
      if (type !== undefined) row[key] = WRITERS[type](fromJSON(value));
    }
    return row;
  });
}

/** The row as JSON writes it: every field in schema order, then any errors. */
function outputRow(row: Row, spec: TableSpec): Record<string, unknown> {
  const out: Record<string, unknown> = {};
  for (const field of Object.keys(spec.fields)) out[field] = row[field] ?? null;
  if (row._erb_errors && Object.keys(row._erb_errors).length > 0) out._erb_errors = row._erb_errors;
  return out;
}

interface TableRows {
  rows: Row[];
  get(row: Row, field: string): Value;
}

class Dataset {
  readonly byFile = new Map<string, TableSpec>();
  readonly rows = new Map<string, Row[]>();
  readonly closures = new Map<string, TableRows>();
  blank = new Map<string, Value>();

  constructor(readonly specs: TableSpec[]) {
    for (const spec of specs) this.byFile.set(spec.file, spec);
  }

  accessor(spec: TableSpec): (row: Row, field: string) => Value {
    return (row, field) => {
      const type = spec.fields[field];
      if (type === undefined) fail(`${spec.name} has no field ${field}`);
      return READERS[type](row[field]);
    };
  }

  tableRows(file: string): TableRows {
    const closure = this.closures.get(file);
    if (closure) return closure;
    const rows = this.rows.get(file);
    const spec = this.byFile.get(file);
    if (!rows) {
      if (spec) {
        fail(`the rulebook holds ${spec.rulebookRows} ${spec.name} rows but blank-tests has no ${file}.json — regenerate the test fixtures`);
      }
      fail(`no table ${JSON.stringify(file)}`);
    }
    return { rows, get: this.accessor(spec as TableSpec) };
  }

  set(spec: TableSpec, row: Row, field: string, v: Value): void {
    const type = spec.fields[field];
    if (type === undefined) fail(`${spec.name} has no field ${field}`);
    row[field] = WRITERS[type](v);
  }

  // ───────────── lookups ─────────────

  computeLookup(lookup: LookupSpec, owner: TableSpec, rows: Row[]): void {
    const failAll = (message: string) => {
      for (const r of rows) {
        this.set(owner, r, lookup.field, Null);
        erbErrors(r)[lookup.field] = message;
      }
    };
    if (lookup.error) {
      failAll(lookup.error);
      return;
    }
    try {
      const target = this.tableRows(lookup.target as string);
      const index = new Map<string, Value>();
      for (const t of target.rows) {
        const pk = target.get(t, lookup.match as string);
        if (pk.k === KNull) continue;
        index.set(lookupKey(pk), target.get(t, lookup.ret as string));
      }
      const noMatch = this.blankJoinValue(lookup.target as string, lookup.ret as string);
      const get = this.accessor(owner);
      for (const r of rows) {
        const key = get(r, lookup.key as string);
        const found = key.k !== KNull ? index.get(lookupKey(key)) : undefined;
        this.set(owner, r, lookup.field, found ?? noMatch);
      }
    } catch (e) {
      failAll(errorText(e));
    }
  }

  /** What a lookup renders when nothing matches: the joined table's field
   *  evaluated against an all-NULL row, as a LEFT JOIN evaluates a calculated
   *  column — a formula built on literal text still yields text. */
  blankJoinValue(file: string, field: string): Value {
    const cacheKey = `${file}.${field}`;
    const cached = this.blank.get(cacheKey);
    if (cached) return cached;
    const spec = this.byFile.get(file);
    if (!spec) {
      this.blank.set(cacheKey, Null);
      return Null;
    }
    const lookup = spec.lookups.find(l => l.field === field);
    if (lookup) {
      if (lookup.error) return Null;
      const v = this.blankJoinValue(lookup.target as string, lookup.ret as string);
      this.blank.set(cacheKey, v);
      return v;
    }
    let result: Value;
    try {
      const blank = newRow(spec);
      spec.compute(blank);
      result = this.accessor(spec)(blank, field);
    } catch {
      result = Null;
    }
    this.blank.set(cacheKey, result);
    return result;
  }

  // ───────────── aggregations ─────────────

  aggregateValue(agg: AggregateSpec, owner: TableSpec, record: Row): Value {
    if (agg.op === "COMPOSITE") {
      const aggregates: Record<string, Value> = {};
      for (const part of agg.parts ?? []) aggregates[part.field] = this.aggregateValue(part, owner, record);
      return (agg.scalar as NonNullable<AggregateSpec["scalar"]>)(record, aggregates);
    }
    const table = this.tableRows(agg.table as string);
    const ownerGet = this.accessor(owner);
    const criteria = agg.criteria ?? [];
    const matching = table.rows.filter(row => criteria.every(c => criterionMatches(c, table.get(row, c.range), () => ownerGet(record, c.field as string))));
    switch (agg.op) {
      case "COUNTIFS":
      case "COUNT":
        return vI(matching.length);
      case "MIN":
      case "MAX": {
        const values = matching.map(row => table.get(row, agg.target as string)).filter(v => !isBlank(v));
        if (values.length === 0) return Null;
        let best = values[0];
        for (const v of values.slice(1)) {
          // min()/max() keep the first of equal extremes.
          if ((agg.op === "MIN" && pyLess(v, best, agg.op)) || (agg.op === "MAX" && pyLess(best, v, agg.op))) best = v;
        }
        return best;
      }
    }
    const numbers: Value[] = [];
    for (const row of matching) {
      const raw = table.get(row, agg.target as string);
      if (isBlank(raw)) continue;
      let value: Value;
      if (raw.k === KBool) value = vF(raw.b ? 1 : 0);
      else {
        const n = toNumber(raw);
        if (n === null) continue;
        value = vF(n.n);
      }
      if (agg.suffix) value = agg.suffix(value);
      numbers.push(value);
    }
    if (agg.op === "SUM") {
      if (numbers.length === 0) return vI(0);
      let total = 0;
      for (const n of numbers) total += n.n;
      return vF(total);
    }
    if (numbers.length === 0) return Null;
    let total = 0;
    for (const n of numbers) total += n.n;
    return vF(total / numbers.length);
  }

  computeAggregation(agg: AggregateSpec, owner: TableSpec, rows: Row[]): void {
    for (const r of rows) {
      if (agg.error) {
        this.set(owner, r, agg.field, Null);
        erbErrors(r)[agg.field] = agg.error;
        continue;
      }
      try {
        this.set(owner, r, agg.field, this.aggregateValue(agg, owner, r));
      } catch (e) {
        this.set(owner, r, agg.field, Null);
        erbErrors(r)[agg.field] = errorText(e);
      }
    }
  }
}

function lookupKey(v: Value): string {
  switch (v.k) {
    case KStr: return "s:" + v.s;
    case KNum: return "n:" + String(v.n);
    case KBool: return v.b ? "n:1" : "n:0";
    case KTime: {
      const t = v.t as ErbTime;
      return `t:${t.sec}:${t.nsec}:${t.offset}`;
    }
  }
  return "";
}

function blankAsEmpty(v: Value): Value { return v.k === KNull ? EMPTY : v; }

function criterionMatches(c: Criterion, cell: Value, recordField: () => Value): boolean {
  switch (c.kind) {
    case "field":
      return pyEqual(blankAsEmpty(cell), blankAsEmpty(recordField()));
    case "op": {
      if (isBlank(cell)) return false;
      let cellNum: number;
      if (cell.k === KBool) cellNum = cell.b ? 1 : 0;
      else {
        const n = toNumber(cell);
        if (n === null) return false;
        cellNum = n.n;
      }
      const target = toNumber(c.literal as Value);
      if (target === null) return false;
      switch (c.op) {
        case ">": return cellNum > target.n;
        case ">=": return cellNum >= target.n;
        case "<": return cellNum < target.n;
        case "<=": return cellNum <= target.n;
        case "<>": return cellNum !== target.n;
      }
      return cellNum === target.n;
    }
  }
  return pyEqual(cell, c.literal as Value);
}

/** Python < for the values MIN/MAX rollups compare. */
function pyLess(a: Value, b: Value, fn: string): boolean {
  const an = numericLike(a);
  const bn = numericLike(b);
  if (an !== null && bn !== null) return an < bn;
  if (a.k === KStr && b.k === KStr) return a.s < b.s;
  if (a.k === KTime && b.k === KTime) return compareInstants(a.t as ErbTime, b.t as ErbTime) < 0;
  return fail(`${fn} over values that cannot be ordered: ${pyRepr(a)} and ${pyRepr(b)}`);
}

function materializeClosure(d: Dataset, spec: ClosureSpec): TableRows {
  const source = d.tableRows(spec.source);
  const edges: Array<[string, string]> = [];
  for (const r of source.rows) {
    // Only boolean TRUE admits an edge; NULL, FALSE and non-booleans exclude it.
    if (spec.filter !== null && source.get(r, spec.filter) !== TRUE) continue;
    const from = source.get(r, spec.from);
    const to = source.get(r, spec.to);
    if (isBlank(from) || isBlank(to)) continue;
    edges.push([pyStr(from), pyStr(to)]);
  }
  const rows = computeClosureRelation(edges).map(p => ({
    from_id: vS(p.from_id), to_id: vS(p.to_id), hop_distance: vI(p.hop_distance), is_inferred: vB(p.is_inferred),
  }) as unknown as Row);
  return { rows, get: (row, field) => (row[field] as Value | undefined) ?? Null };
}

// ───────────────────────────── the run ─────────────────────────────

function fatal(message: string): never {
  process.stderr.write(`FATAL: ${message}\n`);
  process.exit(1);
}

/**
 * Loads every table's blank test rows, computes lookups, aggregations and
 * calculations across the whole dataset until nothing changes, and writes each
 * table's answers.
 *
 * Tables depend on each other in both directions — one table's lookup reads
 * another's aggregation, which counts rows of the first — so no table order
 * works; a field's inputs settle one pass before the field does. The pass bound
 * is the number of computed fields, which a dependency chain cannot exceed
 * without a cycle.
 */
export function erbRun(specs: TableSpec[], closures: ClosureSpec[], calculatedFieldCount: number): void {
  const testing = process.env.ERB_TESTING_DIR;
  if (!testing) fatal("ERB_TESTING_DIR is not set; point it at the domain's testing/ directory.");
  const substrate = process.env.ERB_SUBSTRATE_NAME;
  if (!substrate) fatal("ERB_SUBSTRATE_NAME is not set; the harness must name the substrate whose answers these are.");
  const blankDir = path.join(testing, "blank-tests");
  const answersDir = path.join(testing, substrate, "test-answers");
  fs.mkdirSync(answersDir, { recursive: true });

  const d = new Dataset(specs);
  let computed = 0;
  const graded: string[] = [];
  for (const spec of specs) {
    const file = path.join(blankDir, `${spec.file}.json`);
    if (!fs.existsSync(file)) {
      // No fixture: a table the rulebook declares empty has no rows to test, and
      // reads as empty. One the rulebook holds rows for stays unloaded, so
      // anything that reads it fails naming the gap.
      if (spec.rulebookRows === 0) d.rows.set(spec.file, []);
      continue;
    }
    d.rows.set(spec.file, loadRows(file, spec));
    graded.push(spec.file);
    computed += spec.lookups.length + spec.aggregations.length;
  }
  if (graded.length === 0) fatal(`no blank tests for any table under ${blankDir}`);
  for (const c of closures) {
    if (!d.rows.has(c.source)) fatal(`closure ${c.view} needs ${c.source}.json in ${blankDir}`);
    const closure = materializeClosure(d, c);
    d.closures.set(c.view, closure);
    console.log(`  -> ${c.view}: ${closure.rows.length} pairs`);
  }

  const maxPasses = computed + calculatedFieldCount + 2;
  let previous = "";
  let passes = 0;
  for (;;) {
    passes++;
    if (passes > maxPasses) fatal(`the dataset did not settle within ${maxPasses} passes — the field dependencies are cyclic.`);
    d.blank = new Map();
    // A filtered closure's filter may be a derived field, so it is re-read from
    // the current rows on every pass and settles with them.
    for (const c of closures) {
      if (c.filter !== null) d.closures.set(c.view, materializeClosure(d, c));
    }
    for (const spec of specs) {
      const rows = d.rows.get(spec.file);
      if (!rows) continue;
      for (const r of rows) delete r._erb_errors;
      for (const l of spec.lookups) d.computeLookup(l, spec, rows);
      for (const a of spec.aggregations) d.computeAggregation(a, spec, rows);
      for (const r of rows) spec.compute(r);
    }
    let current = "";
    for (const spec of specs) {
      const rows = d.rows.get(spec.file);
      if (rows) current += JSON.stringify(rows.map(r => outputRow(r, spec)));
    }
    if (current === previous) break;
    previous = current;
  }
  console.log(`TypeScript substrate: dataset settled after ${passes} passes`);

  let total = 0;
  for (const file of [...graded].sort()) {
    const spec = d.byFile.get(file) as TableSpec;
    const rows = d.rows.get(file) as Row[];
    fs.writeFileSync(path.join(answersDir, `${file}.json`), JSON.stringify(rows.map(r => outputRow(r, spec)), null, 2));
    total += rows.length;
    console.log(`  -> ${file}: ${rows.length} records`);
  }
  console.log(`TypeScript substrate: wrote ${total} records across ${graded.length} tables to ${answersDir}`);
}
