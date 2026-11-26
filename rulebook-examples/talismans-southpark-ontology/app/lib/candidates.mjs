// Candidate rows. A formula can decide whether a fact holds, but only for a fact
// that is a row. So every fact a rule could conclude over the cast is a row of
// Statements, every way a rule could conclude it is a row of Firings, and every
// pair the disjoint rule watches is a row of DisjointChecks. This module is the
// ONE place those rows come from: the rulebook file's own candidates are what it
// produces for the seed cast (testing/app/app.test.mjs proves it), and the
// app calls it whenever a thing, a property, its wiring or a literal value is
// added or removed.
//
// It reads only stored rows (Things, Properties, PropertyAxioms, FiringKinds,
// Graphs, Statements) and decides nothing about which facts hold.

const statementId = (s, p, o) => `${s}|${p}|${o}`;

export function expectedCandidates(ws) {
  const things = ws.rows('Things');
  const individuals = things.filter((t) => t.Kind === 'individual').map((t) => t.ThingId);
  const classes = things.filter((t) => t.Kind === 'class').map((t) => t.ThingId);
  const thingIds = new Set(things.map((t) => t.ThingId));
  const properties = ws.rows('Properties');
  const prop = new Map(properties.map((p) => [p.PropertyId, p]));
  const kinds = new Map(ws.rows('FiringKinds').map((k) => [k.FiringKindId, k]));
  const graph = ws.rows('Graphs')[0].GraphId;
  const typeProperties = properties.filter((p) => p.PropertyKind === 'type');
  if (typeProperties.length !== 1) throw new Error(`Expected exactly one property of kind 'type' (class membership), found ${typeProperties.length}`);
  const isA = typeProperties[0].PropertyId;
  const axioms = ws.rows('PropertyAxioms').filter((a) => prop.has(a.Property) && prop.has(a.First) && (a.Second == null || prop.has(a.Second)));
  const axiomsOf = (kind) => axioms.filter((a) => a.Kind === kind);

  // --- Statements: structural candidates, plus every stored fact whose parts still exist.
  const statements = new Map();
  const add = (s, p, o, value) => {
    const id = statementId(s, p, o ?? value);
    if (!statements.has(id)) statements.set(id, { StatementId: id, Graph: graph, Subject: s, Property: p, ObjectThing: o ?? null, ObjectValue: o == null ? value : null });
  };
  for (const p of properties.filter((x) => x.PropertyKind === 'object')) {
    for (const s of individuals) for (const o of individuals) if (s !== o) add(s, p.PropertyId, o);
  }
  for (const s of individuals) for (const c of classes) add(s, isA, c);
  for (const row of ws.rows('Statements')) {
    // A literal fact, or a fact a reader stated outside the structural set (say, about a class).
    const alive = prop.has(row.Property) && thingIds.has(row.Subject) && (row.ObjectThing == null || thingIds.has(row.ObjectThing));
    if (alive) add(row.Subject, row.Property, row.ObjectThing, row.ObjectValue);
  }

  // --- Firings: every way the Properties wiring lets a rule conclude a candidate.
  const firings = new Map();
  const fire = (kind, target, first, second) => {
    if (!statements.has(target) || !statements.has(first) || (second && !statements.has(second))) return;
    const k = kinds.get(kind);
    if (!k) throw new Error(`FiringKinds has no row ${kind}`);
    const id = second ? `${kind}:${first}+${second}>${target}` : `${kind}:${first}>${target}`;
    firings.set(id, { FiringId: id, Kind: kind, Rule: k.Rule, Target: target, First: first, Second: second ?? null });
  };
  const inverseOf = new Map();
  for (const a of axiomsOf('inverse')) (inverseOf.get(a.Property) || inverseOf.set(a.Property, []).get(a.Property)).push(a.First);
  for (const r of statements.values()) {
    const p = prop.get(r.Property);
    const s = r.Subject, o = r.ObjectThing;
    if (o) for (const inverse of inverseOf.get(p.PropertyId) || []) fire('inverse', statementId(o, inverse, s), r.StatementId);
    if (p.IsSymmetric && o) fire('symmetric', statementId(o, p.PropertyId, s), r.StatementId);
    if (p.DomainClass) fire('domain', statementId(s, isA, p.DomainClass), r.StatementId);
    if (p.RangeClass && o) fire('range', statementId(o, isA, p.RangeClass), r.StatementId);
  }
  for (const g of axiomsOf('chain').filter((x) => x.Second != null)) {
    for (const a of individuals) for (const b of individuals) for (const c of individuals) {
      if (a !== b && b !== c && a !== c) fire('chain', statementId(a, g.Property, c), statementId(a, g.First, b), statementId(b, g.Second, c));
    }
  }

  // --- DisjointChecks: one per disjoint axiom and ordered pair, in the axiom's direction.
  const checks = new Map();
  for (const a of axiomsOf('disjoint')) {
    const rule = kinds.get('disjoint');
    if (!rule) throw new Error('FiringKinds has no row disjoint (the rule the disjointness checks answer to)');
    for (const s of individuals) for (const o of individuals) {
      if (s === o) continue;
      const first = statementId(s, a.Property, o);
      const id = `${first}~${a.First}`;
      checks.set(id, { DisjointCheckId: id, Graph: graph, Rule: rule.Rule, First: first, Second: statementId(s, a.First, o) });
    }
  }
  return { Statements: statements, Firings: firings, DisjointChecks: checks };
}

// Makes the three tables hold exactly the expected candidates: missing rows are
// added (a new fact starts unlisted and switched off, at the sort order given),
// rows whose things, properties or wiring are gone are removed.
export function syncCandidates(ws, nextSortOrder) {
  if (!Number.isInteger(nextSortOrder)) throw new Error('syncCandidates needs the sort order the next candidate fact takes (Graphs.NextCandidateSortOrder)');
  const expected = expectedCandidates(ws);
  const result = { added: 0, removed: 0, nextSortOrder };
  const keyOf = { Statements: 'StatementId', Firings: 'FiringId', DisjointChecks: 'DisjointCheckId' };
  // Dependents first when removing, dependencies first when adding.
  for (const name of ['Firings', 'DisjointChecks', 'Statements']) {
    const gone = ws.rows(name).map((row) => row[keyOf[name]]).filter((id) => !expected[name].has(id));
    result.removed += ws.removeMany(name, gone);
  }
  for (const name of ['Statements', 'Firings', 'DisjointChecks']) {
    const fresh = [...expected[name].values()].filter((row) => !ws.get(name, row[keyOf[name]]));
    const rows = name === 'Statements'
      ? fresh.map((row) => ({ ...row, IsListed: false, IsBuiltIn: false, IsDetail: false, IsStated: false, ActivationSeq: 0, SortOrder: result.nextSortOrder++ }))
      : fresh;
    result.added += ws.insertMany(name, rows);
  }
  return result;
}
