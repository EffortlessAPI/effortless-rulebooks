// Formula parser for rulebook one-line formulas. Produces an AST that can be
// rendered either as JSX (with clickable chips for refs) or as a plain-English
// sentence ("True when CumulativeBalance is greater than 25,000.").
//
// Supported grammar:
//   Refs:       {{Name}}                          (same-table)
//               Table!{{Name}}                    (cross-table)
//   Literals:   123  3.14  "string"  TRUE  FALSE
//               TRUE()  FALSE()                   (Airtable-style nullaries)
//   Calls:      FN(arg1, arg2, ...)
//   Ops:        + - * /  =  <  >  <=  >=  <>  &
//   Grouping:   ( expr )
//   Unary:      -expr
//
// The leading "=" in a formula is stripped.

export type Node =
  | { kind: "ref"; table: string | null; field: string }
  | { kind: "num"; value: number }
  | { kind: "str"; value: string }
  | { kind: "bool"; value: boolean }
  | { kind: "unary"; op: "-"; arg: Node }
  | { kind: "binop"; op: BinOp; left: Node; right: Node }
  | { kind: "call"; fn: string; args: Node[] };

export type BinOp = "+" | "-" | "*" | "/" | "&" | "=" | "<>" | "<" | ">" | "<=" | ">=";

interface Tok {
  kind:
    | "ref"
    | "num"
    | "str"
    | "ident"
    | "op"
    | "lparen"
    | "rparen"
    | "comma";
  value: string;
  // For "ref" tokens:
  table?: string | null;
  field?: string;
}

const OP_2CHAR = new Set(["<=", ">=", "<>"]);
const OP_1CHAR = new Set(["+", "-", "*", "/", "&", "=", "<", ">"]);

function tokenize(src: string): Tok[] {
  let s = src.trim();
  if (s.startsWith("=")) s = s.slice(1);
  const toks: Tok[] = [];
  let i = 0;
  while (i < s.length) {
    const c = s[i];
    if (/\s/.test(c)) { i++; continue; }

    // Refs: optional `Table!` + `{{ Name }}`
    if (c === "{" && s[i + 1] === "{") {
      const end = s.indexOf("}}", i + 2);
      if (end < 0) throw new Error("unterminated {{...}} in formula");
      const inner = s.slice(i + 2, end).trim();
      if (!/^[A-Za-z_][A-Za-z0-9_]*$/.test(inner)) {
        throw new Error(`bad ref name: {{${inner}}}`);
      }
      toks.push({ kind: "ref", value: inner, table: null, field: inner });
      i = end + 2;
      continue;
    }

    // Strings (double-quoted, with backslash escapes)
    if (c === '"') {
      let j = i + 1;
      let out = "";
      while (j < s.length && s[j] !== '"') {
        if (s[j] === "\\" && j + 1 < s.length) { out += s[j + 1]; j += 2; continue; }
        out += s[j]; j++;
      }
      if (j >= s.length) throw new Error("unterminated string in formula");
      toks.push({ kind: "str", value: out });
      i = j + 1;
      continue;
    }

    // Numbers
    if (/[0-9]/.test(c) || (c === "." && /[0-9]/.test(s[i + 1] ?? ""))) {
      let j = i;
      while (j < s.length && /[0-9.]/.test(s[j])) j++;
      toks.push({ kind: "num", value: s.slice(i, j) });
      i = j;
      continue;
    }

    // 2-char ops first
    const two = s.slice(i, i + 2);
    if (OP_2CHAR.has(two)) { toks.push({ kind: "op", value: two }); i += 2; continue; }

    // 1-char ops
    if (OP_1CHAR.has(c)) { toks.push({ kind: "op", value: c }); i++; continue; }

    if (c === "(") { toks.push({ kind: "lparen", value: "(" }); i++; continue; }
    if (c === ")") { toks.push({ kind: "rparen", value: ")" }); i++; continue; }
    if (c === ",") { toks.push({ kind: "comma", value: "," }); i++; continue; }

    // Identifier (function name OR Table! prefix of a ref)
    if (/[A-Za-z_]/.test(c)) {
      let j = i;
      while (j < s.length && /[A-Za-z0-9_]/.test(s[j])) j++;
      const name = s.slice(i, j);
      // Look ahead: is this `Table!{{Field}}`?
      let k = j;
      while (k < s.length && /\s/.test(s[k])) k++;
      if (s[k] === "!") {
        let m = k + 1;
        while (m < s.length && /\s/.test(s[m])) m++;
        if (s[m] === "{" && s[m + 1] === "{") {
          const end = s.indexOf("}}", m + 2);
          if (end < 0) throw new Error("unterminated {{...}} after Table!");
          const inner = s.slice(m + 2, end).trim();
          if (!/^[A-Za-z_][A-Za-z0-9_]*$/.test(inner)) {
            throw new Error(`bad cross-table ref: ${name}!{{${inner}}}`);
          }
          toks.push({ kind: "ref", value: `${name}!${inner}`, table: name, field: inner });
          i = end + 2;
          continue;
        }
      }
      toks.push({ kind: "ident", value: name });
      i = j;
      continue;
    }

    throw new Error(`unrecognized character at position ${i}: "${c}"`);
  }
  return toks;
}

// Pratt-ish recursive descent with explicit precedence.
//   parseExpr      → equality / comparison
//   parseAddSub    → + -
//   parseConcat    → &
//   parseMulDiv    → * /
//   parseUnary     → -primary
//   parsePrimary   → ref | num | str | TRUE/FALSE | IDENT(args) | (expr)

interface Cursor { toks: Tok[]; pos: number; }

function peek(c: Cursor): Tok | undefined { return c.toks[c.pos]; }
function consume(c: Cursor): Tok { return c.toks[c.pos++]; }
function expect(c: Cursor, kind: Tok["kind"], value?: string): Tok {
  const t = peek(c);
  if (!t) throw new Error(`expected ${kind}, got EOF`);
  if (t.kind !== kind || (value !== undefined && t.value !== value)) {
    throw new Error(`expected ${kind}${value ? "(" + value + ")" : ""}, got ${t.kind}(${t.value})`);
  }
  return consume(c);
}

function parsePrimary(c: Cursor): Node {
  const t = peek(c);
  if (!t) throw new Error("unexpected EOF in primary");
  if (t.kind === "op" && t.value === "-") {
    consume(c);
    return { kind: "unary", op: "-", arg: parsePrimary(c) };
  }
  if (t.kind === "ref") {
    consume(c);
    return { kind: "ref", table: t.table ?? null, field: t.field! };
  }
  if (t.kind === "num") {
    consume(c);
    return { kind: "num", value: Number(t.value) };
  }
  if (t.kind === "str") {
    consume(c);
    return { kind: "str", value: t.value };
  }
  if (t.kind === "lparen") {
    consume(c);
    const e = parseExpr(c);
    expect(c, "rparen");
    return e;
  }
  if (t.kind === "ident") {
    consume(c);
    const name = t.value;
    // TRUE / FALSE (with or without parens)
    if (name === "TRUE" || name === "FALSE") {
      if (peek(c)?.kind === "lparen") {
        consume(c);
        expect(c, "rparen");
      }
      return { kind: "bool", value: name === "TRUE" };
    }
    // Function call
    if (peek(c)?.kind === "lparen") {
      consume(c);
      const args: Node[] = [];
      if (peek(c)?.kind !== "rparen") {
        args.push(parseExpr(c));
        while (peek(c)?.kind === "comma") {
          consume(c);
          args.push(parseExpr(c));
        }
      }
      expect(c, "rparen");
      return { kind: "call", fn: name.toUpperCase(), args };
    }
    // Bare ident: treat as a 0-arg call (rare; e.g. NOW())
    return { kind: "call", fn: name.toUpperCase(), args: [] };
  }
  throw new Error(`unexpected token ${t.kind}(${t.value}) in primary`);
}

function parseMulDiv(c: Cursor): Node {
  let left = parsePrimary(c);
  while (true) {
    const t = peek(c);
    if (!t || t.kind !== "op" || (t.value !== "*" && t.value !== "/")) break;
    consume(c);
    const right = parsePrimary(c);
    left = { kind: "binop", op: t.value as BinOp, left, right };
  }
  return left;
}

function parseConcat(c: Cursor): Node {
  let left = parseMulDiv(c);
  while (true) {
    const t = peek(c);
    if (!t || t.kind !== "op" || t.value !== "&") break;
    consume(c);
    const right = parseMulDiv(c);
    left = { kind: "binop", op: "&", left, right };
  }
  return left;
}

function parseAddSub(c: Cursor): Node {
  let left = parseConcat(c);
  while (true) {
    const t = peek(c);
    if (!t || t.kind !== "op" || (t.value !== "+" && t.value !== "-")) break;
    consume(c);
    const right = parseConcat(c);
    left = { kind: "binop", op: t.value as BinOp, left, right };
  }
  return left;
}

function parseExpr(c: Cursor): Node {
  let left = parseAddSub(c);
  while (true) {
    const t = peek(c);
    if (!t || t.kind !== "op") break;
    if (!["=", "<>", "<", ">", "<=", ">="].includes(t.value)) break;
    consume(c);
    const right = parseAddSub(c);
    left = { kind: "binop", op: t.value as BinOp, left, right };
  }
  return left;
}

export function parseFormula(formula: string): Node {
  const toks = tokenize(formula);
  const c: Cursor = { toks, pos: 0 };
  const node = parseExpr(c);
  if (c.pos < toks.length) {
    const rest = toks.slice(c.pos).map((t) => t.value).join(" ");
    throw new Error(`unexpected trailing tokens in formula: ${rest}`);
  }
  return node;
}

// Best-effort: never throws. Returns null on parse error so the UI can fall
// back to raw-text rendering.
export function tryParseFormula(formula: string): Node | null {
  try { return parseFormula(formula); } catch { return null; }
}
