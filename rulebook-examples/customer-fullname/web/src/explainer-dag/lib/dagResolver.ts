// Rulebook → field-level DAG. Reads from the embedded rulebook (baked in at
// transpile time) and resolves upstream/downstream/leaf provenance for any
// field. Pure, synchronous, no I/O.

import { rulebook } from "../embedded-rulebook.ts";
import type {
  FieldType,
  RawField,
  FieldRef,
  FieldNode,
  DagResponse,
} from "./types.ts";

type Rulebook = Record<string, unknown>;

function tablesOf(rb: Rulebook): string[] {
  return Object.keys(rb).filter((k) => {
    if (k.startsWith("$") || k.startsWith("_")) return false;
    if (!/^[A-Z]/.test(k)) return false;
    const v = rb[k];
    return !!v && typeof v === "object" && "schema" in (v as object);
  });
}

function schemaOf(rb: Rulebook, table: string): RawField[] {
  const t = rb[table];
  if (!t || typeof t !== "object" || !("schema" in (t as object))) return [];
  return ((t as { schema?: RawField[] }).schema ?? []) as RawField[];
}

function fieldOf(rb: Rulebook, table: string, field: string): RawField | null {
  return schemaOf(rb, table).find((f) => f.name === field) ?? null;
}

// The table a relationship field points at. Prefer the explicit RelatedTo; when
// it's absent (some reverse links only carry an implied target), fall back to a
// bare `formula` value that names a real table — and lastly the field name
// itself if it matches a table. Returns null only when nothing resolves, so the
// UI can degrade to generic "another table" prose without a dead link.
function relationTarget(rb: Rulebook, f: RawField): string | null {
  const candidate = (v: unknown): string | null => {
    if (typeof v !== "string") return null;
    const name = v.trim();
    if (!name || /[^A-Za-z0-9_]/.test(name)) return null; // a real formula, not a bare table name
    const t = rb[name];
    return t && typeof t === "object" && "schema" in (t as object) ? name : null;
  };
  return f.RelatedTo?.trim() || candidate(f.formula) || candidate(f.name);
}

function toNode(rb: Rulebook, table: string, field: string): FieldNode | null {
  const f = fieldOf(rb, table, field);
  if (!f) return null;
  return {
    table,
    field: f.name,
    datatype: f.datatype ?? "string",
    type: (f.type ?? "raw") as FieldType,
    nullable: f.nullable ?? true,
    formula: f.formula ?? null,
    description: f.Description ?? "",
    relatedTo: f.type === "relationship" ? relationTarget(rb, f) : (f.RelatedTo ?? null),
  };
}

// Parse a formula and return the list of (table, field) refs it touches.
export function extractFormulaRefs(formula: string, sameTable: string): FieldRef[] {
  if (!formula) return [];
  const refs: FieldRef[] = [];
  const re = /(?:([A-Za-z_][A-Za-z0-9_]*)!\s*)?\{\{\s*([A-Za-z_][A-Za-z0-9_]*)\s*\}\}/g;
  let m: RegExpExecArray | null;
  while ((m = re.exec(formula)) !== null) {
    const t = (m[1] ?? sameTable).trim();
    const f = m[2].trim();
    refs.push({ table: t, field: f });
  }
  const seen = new Set<string>();
  return refs.filter((r) => {
    const k = `${r.table}.${r.field}`;
    if (seen.has(k)) return false;
    seen.add(k);
    return true;
  });
}

interface Index {
  upstream: Map<string, FieldRef[]>;
  downstream: Map<string, FieldRef[]>;
  all: Set<string>;
}

let _index: Index | null = null;

function getIndex(rb: Rulebook): Index {
  if (_index) return _index;
  const tables = tablesOf(rb);
  const upstream = new Map<string, FieldRef[]>();
  const downstream = new Map<string, FieldRef[]>();
  const all = new Set<string>();

  for (const t of tables) {
    for (const f of schemaOf(rb, t)) {
      const key = `${t}.${f.name}`;
      all.add(key);
      if (f.formula) {
        upstream.set(key, extractFormulaRefs(f.formula, t));
      } else {
        upstream.set(key, []);
      }
    }
  }
  for (const [child, parents] of upstream) {
    const [ct, cf] = child.split(".");
    for (const p of parents) {
      const pk = `${p.table}.${p.field}`;
      const list = downstream.get(pk) ?? [];
      list.push({ table: ct, field: cf });
      downstream.set(pk, list);
    }
  }
  _index = { upstream, downstream, all };
  return _index;
}

function computeDepth(
  rb: Rulebook,
  index: Index,
  table: string,
  field: string,
  seen: Set<string> = new Set(),
): number {
  const key = `${table}.${field}`;
  if (seen.has(key)) return 0;
  seen.add(key);
  const f = fieldOf(rb, table, field);
  if (!f) return 0;
  if (f.type === "raw" || f.type === "relationship" || !f.formula) return 0;
  const parents = index.upstream.get(key) ?? [];
  if (parents.length === 0) return 0;
  let max = 0;
  for (const p of parents) {
    const d = 1 + computeDepth(rb, index, p.table, p.field, new Set(seen));
    if (d > max) max = d;
  }
  return max;
}

function collectLeaves(
  rb: Rulebook,
  index: Index,
  table: string,
  field: string,
  seen: Set<string> = new Set(),
  acc: Map<string, FieldNode> = new Map(),
  isRoot: boolean = true,
): FieldNode[] {
  const key = `${table}.${field}`;
  if (seen.has(key)) return [...acc.values()];
  seen.add(key);
  const node = toNode(rb, table, field);
  if (!node) return [...acc.values()];
  // A node is a LEAF only when reached by recursion from the field we're
  // explaining — never the field itself. (The root being its own leaf is the
  // self-reference bug: a formula-less aggregation with no traced inputs would
  // otherwise list itself under "ground truth at the leaves".)
  if (!isRoot && (node.type === "raw" || node.type === "relationship")) {
    if (!acc.has(key)) acc.set(key, node);
    return [...acc.values()];
  }
  const parents = index.upstream.get(key) ?? [];
  if (!isRoot && parents.length === 0 && !node.formula) {
    if (!acc.has(key)) acc.set(key, node);
    return [...acc.values()];
  }
  for (const p of parents) collectLeaves(rb, index, p.table, p.field, seen, acc, false);
  return [...acc.values()];
}

function collectTransitiveDownstream(
  index: Index,
  table: string,
  field: string,
): FieldRef[] {
  const out = new Map<string, FieldRef>();
  const queue: FieldRef[] = [{ table, field }];
  const seen = new Set<string>([`${table}.${field}`]);
  while (queue.length) {
    const cur = queue.shift()!;
    const children = index.downstream.get(`${cur.table}.${cur.field}`) ?? [];
    for (const c of children) {
      const k = `${c.table}.${c.field}`;
      if (seen.has(k)) continue;
      seen.add(k);
      out.set(k, c);
      queue.push(c);
    }
  }
  return [...out.values()];
}

export function resolveDag(table: string, field: string): DagResponse | null {
  const rb = rulebook as unknown as Rulebook;
  const index = getIndex(rb);
  const node = toNode(rb, table, field);
  if (!node) return null;
  const key = `${table}.${field}`;
  const upRefs = index.upstream.get(key) ?? [];
  const downRefs = index.downstream.get(key) ?? [];
  const upstream = upRefs
    .map((r) => toNode(rb, r.table, r.field))
    .filter((n): n is FieldNode => n !== null);
  const downstream = downRefs
    .map((r) => toNode(rb, r.table, r.field))
    .filter((n): n is FieldNode => n !== null);
  const leaves = node.type === "raw" || node.type === "relationship"
    ? []
    : collectLeaves(rb, index, table, field);
  const consumerTransitive = collectTransitiveDownstream(index, table, field);
  const depth = computeDepth(rb, index, table, field);

  return {
    ...node,
    depth,
    fanIn: upstream.length,
    fanOut: downstream.length,
    upstream,
    downstream,
    leaves,
    consumerTransitive,
  };
}

export function listTablesAndFields(): { table: string; fields: string[] }[] {
  const rb = rulebook as unknown as Rulebook;
  return tablesOf(rb).map((t) => ({
    table: t,
    fields: schemaOf(rb, t).map((f) => f.name),
  }));
}

// ── Ontology navigation: tables + their columns ────────────────────────────

export interface TableSummary {
  table: string;
  description: string;
  fieldCount: number;
  derivedCount: number;   // calculated / lookup / aggregation
  relationshipCount: number;
}

// Every table in the rulebook with a quick shape summary, for the tables index.
export function listTables(): TableSummary[] {
  const rb = rulebook as unknown as Rulebook;
  return tablesOf(rb).map((t) => {
    const fields = schemaOf(rb, t);
    const derived = fields.filter(
      (f) => f.type === "calculated" || f.type === "lookup" || f.type === "aggregation",
    ).length;
    const rels = fields.filter((f) => f.type === "relationship").length;
    const tv = rb[t] as { Description?: string } | undefined;
    return {
      table: t,
      description: tv?.Description ?? "",
      fieldCount: fields.length,
      derivedCount: derived,
      relationshipCount: rels,
    };
  });
}

// Does this table exist in the rulebook?
export function tableExists(table: string): boolean {
  const rb = rulebook as unknown as Rulebook;
  return tablesOf(rb).includes(table);
}

// One table's description (for the table page header).
export function tableDescription(table: string): string {
  const rb = rulebook as unknown as Rulebook;
  const tv = rb[table] as { Description?: string } | undefined;
  return tv?.Description ?? "";
}

// All columns of a table as full FieldNodes (type/datatype/formula/description),
// in schema order — for the per-table page.
export function tableColumns(table: string): FieldNode[] {
  const rb = rulebook as unknown as Rulebook;
  return schemaOf(rb, table)
    .map((f) => toNode(rb, table, f.name))
    .filter((n): n is FieldNode => n !== null);
}
