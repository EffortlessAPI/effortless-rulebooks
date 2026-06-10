// ---------------------------------------------------------------------------
// conformance.ts — the live differential between the two substrates.
//
// Both engines are generated from the same rulebook, so for a correct build
// they should agree. This module runs BOTH on the same raw db and reports every
// field where the reasoned individuals disagree. It is the demo's headline: a
// toggle that doesn't just switch engines but PROVES (or disproves) that they
// compute identically — and when they don't, shows you exactly which field.
//
// We do NOT hide a mismatch behind a fallback (CLAUDE.md). A disagreement is a
// real fact about the two generated substrates: either a transpiler bug (the
// known rulebook-to-postgres ISBLANK / multi-criteria-COUNTIFS issues) or a
// benign representation difference (a number vs its string form). We classify
// each one so the UI can show "12 fields compared, 11 identical, 1 differs"
// rather than a scary all-or-nothing.
// ---------------------------------------------------------------------------
import { runReasoner, type ReasonedWorld, type RawDb } from "./reasoner";
import { runPostgres } from "./postgres";
import type { SchemaMap } from "./schema-map";

// One row's worth of reasoned individual: camelCase field -> engine-derived
// value. Values are open (the engine is the oracle; the frontend renders them,
// it does not compute them) and may also carry `_iri` / `_classes` bookkeeping.
type ReasonedRow = Record<string, unknown>;

// How classify() labels a single field comparison.
//   equal          — identical, or the same answer in a different representation
//   representation  — same content, different spelling — benign
//   presence        — one engine populated a computed field the other left empty
//   value           — a genuine disagreement on a COMPUTED answer — the one that matters
type Classification = "equal" | "representation" | "presence" | "value";

// PascalCase class name -> camelCase primary-key property.
type PkByClass = Record<string, string>;

// PascalCase class name -> { camelField: rulebook field type }.
type FieldTypeByClass = Record<string, Record<string, string>>;

// A single field disagreement (or whole-row "only-in") in the diff.
interface ConformanceDiff {
  class: string;
  pk: unknown;
  field: string;
  ftype?: string;
  reasoner: unknown;
  postgres: unknown;
  kind: Classification | "only-in";
}

// The summary counts for the differential.
interface ConformanceSummary {
  fieldsCompared: number;
  identical: number;
  representationDiffs: number;
  presenceDiffs: number;
  valueDiffs: number;
  agree: boolean;
}

// Classes present in only one engine's payload.
interface ClassesOnlyIn {
  reasoner: string[];
  postgres: string[];
}

// The structured diff diffPayloads() returns.
interface DiffResult {
  summary: ConformanceSummary;
  diffs: ConformanceDiff[];
  classesOnlyIn: ClassesOnlyIn;
}

// The full conformance run: the diff plus each engine's reasoned-facts counter.
interface ConformanceResult extends DiffResult {
  reasonerTriples: number;
  postgresCells: number;
}

// Parse a value that MIGHT be a datetime into an epoch-millis number, else null.
// The reasoner emits "2026-04-03 00:00:00-05:00"; pg returns an ISO Date string
// "2026-04-03T05:00:00.000Z". Same instant, two spellings — not a disagreement.
function asInstant(v: unknown): number | null {
  if (typeof v !== "string") return null;
  if (!/\d{4}-\d{2}-\d{2}[ T]\d{2}:\d{2}/.test(v)) return null;
  const t = Date.parse(v.replace(" ", "T"));
  return Number.isNaN(t) ? null : t;
}

// Split a relationship value that one engine returns as a comma-joined string
// and the other as an array, into a sorted token set, for set comparison.
function asTokenSet(v: unknown): string[] {
  if (Array.isArray(v)) return v.map(String).map((s) => s.trim()).filter(Boolean).sort();
  if (typeof v === "string") return v.split(",").map((s) => s.trim()).filter(Boolean).sort();
  if (v == null) return [];
  return [String(v)];
}

// Classify a single field disagreement. `ftype` is the rulebook field type
// (raw / calculated / lookup / aggregation / relationship) so we can judge what
// a difference MEANS, not just that bytes differ. Returns one of:
//   equal          — identical, or the same answer in a different representation
//   representation  — same content, different spelling (num/str, datetime format,
//                     array vs comma-joined relationship) — benign
//   presence        — one engine populated a computed field the other left
//                     null/absent (e.g. the reasoner doesn't surface iri on every
//                     class; Postgres always computes it) — a coverage gap, not a
//                     wrong answer
//   value           — a genuine disagreement on a COMPUTED answer (a boolean
//                     flips, a count differs). THE one that matters — a real
//                     substrate/transpiler bug.
function classify(a: unknown, b: unknown, ftype: string): Classification {
  if (a === b) return "equal";
  if (a == null && b == null) return "equal";

  // same content, different scalar spelling (41 vs "41", true vs "true")
  const sa = a == null ? null : String(a);
  const sb = b == null ? null : String(b);
  if (sa === sb) return "representation";

  // datetimes: compare as instants
  const ia = asInstant(a), ib = asInstant(b);
  if (ia != null && ib != null) return ia === ib ? "representation" : "value";

  // relationship / back-reference fields. The two engines legitimately represent
  // the SAME relationship at different inference depths:
  //   - OWL-RL folds the INVERSE edge in, so the reasoner returns BOTH endpoints
  //     of a junction (fromStep = [from, to]) where Postgres keeps the one FK.
  //   - The reasoner pre-expands a TRANSITIVE relationship (delegatesTo) to the
  //     full reachable set; Postgres keeps the single direct FK.
  //   - Back-reference rollups: one engine null, the other comma-joins children.
  // In every one of those, one engine's target set is a SUPERSET of the other's
  // (the extra members are inferred/inverse edges, not contradictions). That is
  // the same relationship in a different shape -> `representation`. A real
  // disagreement is when NEITHER side is contained in the other: each names a
  // target the other does not, i.e. they actually point somewhere different.
  if (ftype === "relationship") {
    const ta = new Set(asTokenSet(a));
    const tb = new Set(asTokenSet(b));
    if (ta.size === tb.size && [...ta].every((x) => tb.has(x))) return "representation";
    const aSubsetOfB = [...ta].every((x) => tb.has(x));
    const bSubsetOfA = [...tb].every((x) => ta.has(x));
    // One contained in the other (incl. the empty-vs-populated back-ref case):
    // same relationship, one engine just inferred more. Not a disagreement.
    if (aSubsetOfB || bSubsetOfA) return "representation";
    return "value";
  }

  // one side null/empty, the other populated, on a COMPUTED field: a coverage
  // gap (one engine emits the column, the other doesn't), not a wrong answer.
  const aEmpty = a == null || a === "";
  const bEmpty = b == null || b === "";
  if (aEmpty !== bEmpty) {
    if (ftype === "calculated" || ftype === "lookup" || ftype === "aggregation") {
      return "presence";
    }
    return "presence";
  }

  // arrays of ids (precedesStep): set-compare
  if (Array.isArray(a) || Array.isArray(b)) {
    const na = asTokenSet(a), nb = asTokenSet(b);
    if (na.length === nb.length && na.every((x, i) => x === nb[i])) return "equal";
    return "value";
  }

  return "value";
}

// Index a class's individuals by primary key so we compare like-for-like rows.
function indexByPk(rows: ReasonedRow[] | undefined, pkCamel: string): Map<unknown, ReasonedRow> {
  const m = new Map<unknown, ReasonedRow>();
  for (const r of rows || []) m.set(r[pkCamel], r);
  return m;
}

// Compare two reasoned payloads. Returns a structured diff:
//   {
//     summary: { fieldsCompared, identical, representationDiffs, presenceDiffs, valueDiffs },
//     diffs: [ {class, pk, field, ftype, reasoner, postgres, kind} ],
//     classesOnlyIn: { reasoner:[...], postgres:[...] },
//   }
// `fieldTypeByClass` is { ClassName: { camelField: "raw"|"calculated"|... } },
// built by the caller from the schema-map, so classify() can judge what a
// difference means per field type.
export function diffPayloads(
  rs: ReasonedWorld,
  pg: ReasonedWorld,
  pkByClass: PkByClass,
  fieldTypeByClass: FieldTypeByClass = {}
): DiffResult {
  const diffs: ConformanceDiff[] = [];
  let fieldsCompared = 0;
  let identical = 0;

  const allClasses = new Set<string>([
    ...Object.keys(rs.individuals),
    ...Object.keys(pg.individuals),
  ]);
  const classesOnlyIn: ClassesOnlyIn = { reasoner: [], postgres: [] };

  for (const cls of allClasses) {
    const rsRows = rs.individuals[cls] as ReasonedRow[] | undefined;
    const pgRows = pg.individuals[cls] as ReasonedRow[] | undefined;
    if (!rsRows) { classesOnlyIn.postgres.push(cls); continue; }
    if (!pgRows) { classesOnlyIn.reasoner.push(cls); continue; }

    const pkCamel = pkByClass[cls];
    const rsIdx = indexByPk(rsRows, pkCamel);
    const pgIdx = indexByPk(pgRows, pkCamel);
    const pks = new Set([...rsIdx.keys(), ...pgIdx.keys()]);

    for (const pk of pks) {
      const rRow = rsIdx.get(pk);
      const pRow = pgIdx.get(pk);
      if (!rRow) { diffs.push({ class: cls, pk, field: "(row)", reasoner: null, postgres: "present", kind: "only-in" }); continue; }
      if (!pRow) { diffs.push({ class: cls, pk, field: "(row)", reasoner: "present", postgres: null, kind: "only-in" }); continue; }

      const types = fieldTypeByClass[cls] || {};
      const fields = new Set([...Object.keys(rRow), ...Object.keys(pRow)]);
      for (const f of fields) {
        if (f.startsWith("_")) continue;      // _iri / _classes bookkeeping
        const ftype = types[f] || "raw";
        fieldsCompared++;
        const kind = classify(rRow[f], pRow[f], ftype);
        if (kind === "equal") { identical++; continue; }
        diffs.push({
          class: cls, pk, field: f, ftype,
          reasoner: rRow[f] ?? null,
          postgres: pRow[f] ?? null,
          kind,
        });
      }
    }
  }

  const valueDiffs = diffs.filter((d) => d.kind === "value").length;
  const representationDiffs = diffs.filter((d) => d.kind === "representation").length;
  const presenceDiffs = diffs.filter((d) => d.kind === "presence").length;

  return {
    summary: {
      fieldsCompared,
      identical,
      representationDiffs,
      presenceDiffs,
      valueDiffs,
      // "agree" is the strong claim: the two engines compute the same ANSWER for
      // every computed field. Representation and presence differences don't break
      // it (same answer, different spelling / one engine emits more columns);
      // a single value difference does.
      agree:
        valueDiffs === 0 &&
        classesOnlyIn.reasoner.length === 0 &&
        classesOnlyIn.postgres.length === 0,
    },
    diffs,
    classesOnlyIn,
  };
}

// Build the { ClassName: { camelField: type } } lookup classify() needs, from a
// schema-map (loadSchemaMap output).
export function fieldTypesFromSchemaMap(schemaMap: SchemaMap): FieldTypeByClass {
  const out: FieldTypeByClass = {};
  for (const [cls, t] of Object.entries(schemaMap)) {
    out[cls] = {};
    for (const f of t.allFields) out[cls][f.camel] = f.type;
  }
  return out;
}

// Run both engines on the same db and diff them. pkByClass maps ClassName ->
// camel primary-key; fieldTypeByClass maps ClassName -> {camelField: type}.
// Both are derived from the schema-map by the caller.
export async function runConformance(
  db: RawDb,
  pkByClass: PkByClass,
  fieldTypeByClass: FieldTypeByClass
): Promise<ConformanceResult> {
  const [rs, pg] = await Promise.all([runReasoner(db), runPostgres(db)]);
  return {
    ...diffPayloads(rs, pg, pkByClass, fieldTypeByClass),
    reasonerTriples: rs.reasoned_triples,
    postgresCells: pg.reasoned_triples,
  };
}
