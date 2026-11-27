// The explainer page's data and its clicks, on whichever engine the request names.
// Reads go to the engine (computed values) or the workspace (stored values);
// writes go to the workspace. No rule lives here: a conclusion, card, alert,
// count, button state or next number is always a field some engine computed.
import { syncCandidates } from './candidates.mjs';

const snake = (name) => name.replace(/([a-z0-9])([A-Z])/g, '$1_$2').toLowerCase();
const snakeRow = (row) => Object.fromEntries(Object.entries(row).map(([k, v]) => [snake(k), v]));
const fill = (template, values) => template.replace(/\{(\w+)\}/g, (_, k) => {
  if (!(k in values)) throw new Error(`Page text ${JSON.stringify(template)} names {${k}} but no value was given`);
  return values[k];
});
const bySort = (a, b) => a.SortOrder - b.SortOrder;

const FACT_FIELDS = ['StatementId', 'Subject', 'Property', 'SubjectLabel', 'PropertyLabel', 'ObjectText', 'ObjectLabel', 'IsLiteral',
  'PropertyDatatype', 'IsInEffect', 'IsBuiltIn', 'Source', 'SortOrder'];
const EDGE_FIELDS = ['StatementId', 'SubjectNodeKey', 'ObjectNodeKey', 'SubjectLabel', 'PropertyLabel', 'ObjectText', 'IsLiteral', 'Property',
  'PropertyDatatype', 'ObjectNodeKind', 'ObjectValueNodeLabel', 'IsConclusion', 'IsInContradiction', 'Source', 'SortOrder', 'FirstFiredOrderKey'];
const THING_FIELDS = ['ThingId', 'Label', 'Kind', 'NodeKey', 'NodeKind', 'IsInGraph', 'CountOfChildren', 'CountOfGrandchildren',
  'PortraitName', 'PortraitPage', 'PortraitImage', 'PortraitCrop', 'PortraitWidth', 'PortraitHeight', 'SortOrder'];
const CONCLUSION_FIELDS = ['FiringId', 'Target', 'TargetSubjectLabel', 'TargetPropertyLabel', 'TargetObjectText', 'TargetSubjectNodeKey',
  'TargetObjectNodeKey', 'CardText', 'OrderKey'];

export function createExplainer({ ws }) {
  const texts = () => Object.fromEntries(ws.rows('PageTexts').map((r) => [r.PageTextId, r.Text]));

  // The tools of one form of the rulebook (ontology or concept), in the panel's order. The
  // two forms are the same facts, so each has the same tools, made from its own rulebook.
  function tools(form) {
    const all = ws.rows('RulebookTools');
    const mine = all.filter((t) => t.Form === form);
    if (!mine.length) throw new Error(`The rulebook has no form ${JSON.stringify(form)}: its tools are listed for ${[...new Set(all.map((t) => t.Form))].join(' and ')}`);
    return mine.sort(bySort).map(snakeRow);
  }
  const text = (key) => {
    const row = ws.get('PageTexts', key);
    if (!row) throw new Error(`The rulebook has no page text "${key}"`);
    return row.Text;
  };

  // The current-record summary under the Stan buttons, from the listed facts in effect.
  function recordSummary(facts) {
    const subject = ws.rows('Things').find((thing) => thing.Label === text('record-subject'));
    if (!subject) throw new Error(`Page text record-subject names ${JSON.stringify(text('record-subject'))}, which is no thing's label`);
    const kindOf = Object.fromEntries(ws.rows('Properties').map((p) => [p.PropertyId, p.PropertyKind]));
    const mine = facts.filter((f) => f.Subject === subject.ThingId && f.IsInEffect);
    const isPerson = mine.some((f) => kindOf[f.Property] === 'type' && f.ObjectLabel === text('record-class'));
    const sentences = [isPerson ? text('record-person-yes') : text('record-person-no')];
    for (const p of ws.rows('Properties').filter((x) => x.RecordKind).sort((a, b) => a.RecordOrder - b.RecordOrder)) {
      const values = mine.filter((f) => f.Property === p.PropertyId).map((f) => f.ObjectText);
      sentences.push(values.length ? fill(text('record-value'), { kind: p.RecordKind, values: values.join(', ') }) : fill(text('record-none'), { kind: p.RecordKind }));
    }
    return sentences.join(' ');
  }

  async function page(engine) {
    const started = Date.now();
    const [graph] = await engine.read('Graphs', ['GraphId', 'CountOfStatedFacts', 'CountOfConclusions', 'CountOfViolations', 'IsConsistent', 'ConclusionsMessage', 'HasFactsToAdd']);
    const scenarios = (await engine.read('Scenarios', ['ScenarioId', 'Label', 'ButtonGroup', 'IsPrimary', 'IsAvailable', 'SortOrder'])).sort(bySort);
    const things = (await engine.read('Things', THING_FIELDS)).sort(bySort);
    const facts = (await engine.read('Statements', FACT_FIELDS, { IsListed: true })).sort(bySort);
    const edges = (await engine.read('Statements', EDGE_FIELDS, { Holds: true }))
      .sort((a, b) => (a.IsConclusion - b.IsConclusion) || (a.IsConclusion ? a.FirstFiredOrderKey - b.FirstFiredOrderKey : a.SortOrder - b.SortOrder));
    const conclusions = (await engine.read('Firings', CONCLUSION_FIELDS, { IsChosen: true })).sort((a, b) => a.OrderKey - b.OrderKey);
    const alarms = (await engine.read('DisjointChecks', ['DisjointCheckId', 'AlarmText'], { ViolationUnit: 1 }))
      .sort((a, b) => a.DisjointCheckId.localeCompare(b.DisjointCheckId));
    const layouts = {};
    for (const r of ws.rows('NodeLayouts')) (layouts[r.Layout] ||= {})[r.NodeKey] = [r.X, r.Y];
    return {
      texts: texts(),
      graph: snakeRow(graph),
      rules: [...ws.rows('Rules')].sort(bySort).map(snakeRow),
      scenarios: scenarios.map(snakeRow),
      properties: ws.rows('Properties').filter((p) => p.IsUserAddable).sort(bySort).map(snakeRow),
      things: things.map(snakeRow),
      facts: facts.map(snakeRow),
      edges: edges.map(snakeRow),
      conclusions: conclusions.map(snakeRow),
      alarms: alarms.map(snakeRow),
      layouts,
      slots: [...ws.rows('LayoutSlots')].sort(bySort).map((r) => [r.X, r.Y]),
      names: [...new Set([...things.map((thing) => thing.Label), ...[...ws.rows('Portraits')].sort(bySort).map((p) => p.PortraitId)])],
      tools: tools('ontology'),
      record: recordSummary(facts),
      unsaved: ws.changes().total,
      version: ws.rulebookVersion(),
      pageMs: Date.now() - started,
    };
  }

  // The numbers the next write needs, read once before anything changes.
  async function nextNumbers(engine) {
    const [g] = await engine.read('Graphs', ['NextActivationSeq', 'NextListedSortOrder', 'NextCandidateSortOrder']);
    return { seq: g.NextActivationSeq, listed: g.NextListedSortOrder, candidate: g.NextCandidateSortOrder };
  }
  const listedRow = (id) => {
    const row = ws.need('Statements', id);
    if (!row.IsListed) throw new Error(`${id} is not in the fact list`);
    return row;
  };
  const setFact = (id, on, next) => { listedRow(id); ws.patch('Statements', id, on ? { IsStated: true, ActivationSeq: next.seq++ } : { IsStated: false }); };
  const addToList = (id, next) => { if (!ws.need('Statements', id).IsListed) ws.patch('Statements', id, { IsListed: true, SortOrder: next.listed++ }); };

  async function switchFact(engine, id, on) { setFact(id, on, on ? await nextNumbers(engine) : null); }

  function switchRule(id, on) { ws.patch('Rules', id, { IsEnabled: !!on }); }

  async function removeFact(engine, id) {
    const row = listedRow(id);
    if (row.IsBuiltIn) throw new Error(`${id} is one of the page's teaching facts and cannot be removed`);
    ws.patch('Statements', id, { IsListed: false, IsStated: false, SortOrder: (await nextNumbers(engine)).candidate });
  }

  // A button: its fact baseline, then its explicit changes, then its rule settings.
  async function runScenario(engine, id) {
    const sc = ws.need('Scenarios', id);
    const next = await nextNumbers(engine);
    const listed = () => ws.rows('Statements').filter((r) => r.IsListed).sort(bySort);
    switch (sc.FactBaseline) {
      case 'keep': break;
      case 'built-in': for (const r of listed()) setFact(r.StatementId, !!r.IsBuiltIn, next); break;
      case 'built-in-detail': for (const r of listed()) setFact(r.StatementId, !!(r.IsBuiltIn && r.IsDetail), next); break;
      case 'all-off': for (const r of listed()) setFact(r.StatementId, false, next); break;
      case 'activate-all': for (const r of listed()) setFact(r.StatementId, true, next); break;
      case 'next': for (const r of await engine.read('Statements', ['StatementId'], { IsNextToAdd: true })) setFact(r.StatementId, true, next); break;
      default: throw new Error(`Scenario ${id} has an unknown fact baseline: ${sc.FactBaseline}`);
    }
    for (const c of ws.rows('ScenarioFactChanges').filter((x) => x.Scenario === id).sort(bySort)) {
      const exact = c.ObjectKey == null ? null : `${c.Subject}|${c.Property}|${c.ObjectKey}`;
      if (exact && c.AddsToList) addToList(exact, next);
      const matched = listed().filter((r) => r.Subject === c.Subject && r.Property === c.Property && (!exact || r.StatementId === exact));
      for (const r of matched) setFact(r.StatementId, c.Action === 'on', next);
    }
    for (const r of ws.rows('ScenarioRuleSettings').filter((x) => x.Scenario === id)) ws.patch('Rules', r.Rule, { IsEnabled: r.IsEnabled });
    return { note: sc.Note || '', layout: sc.Layout, clearsSelection: !!sc.ClearsSelection, clearsFormMessage: !!sc.ClearsFormMessage };
  }

  function integerValue(value) {   // the original page's own input check, unchanged
    if (!/^[+-]?\d+$/.test(value)) return null;
    const negative = value.charAt(0) === '-';
    const digits = value.replace(/^[+-]/, '').replace(/^0+(?=\d)/, '');
    return (negative && digits !== '0' ? '-' : '') + digits;
  }

  // A thing by the name a reader typed; a new name becomes a new individual,
  // with a portrait when the name is one a portrait answers to.
  function thingNamed(label) {
    const found = ws.rows('Things').find((thing) => thing.Label === label);
    if (found) return { thing: found, created: false };
    const base = label.replace(/[^A-Za-z0-9]+/g, '') || 'Thing';
    let id = base;
    for (let n = 2; ws.get('Things', id); n++) id = `${base}-${n}`;
    const alias = ws.get('PortraitAliases', label.toLowerCase().split(/\s+/).filter(Boolean).join(' '));
    const things = ws.rows('Things');
    const thing = ws.insert('Things', { ThingId: id, Label: label, Kind: 'individual', SortOrder: (things[things.length - 1]?.SortOrder ?? 0) + 1, Portrait: alias ? alias.Portrait : null });
    return { thing, created: true };
  }

  // The Add fact form. Any name and any value: what the rulebook has no row for yet gets its rows.
  async function addFact(engine, { subject, property, object }) {
    const s = String(subject ?? '').trim(), o0 = String(object ?? '').trim();
    const prop = ws.get('Properties', property);
    if (!prop || !prop.IsUserAddable) throw new Error(`Not a property the form offers: ${property}`);
    const literal = prop.PropertyKind === 'datatype';
    if (!s || !o0 || (s === o0 && !literal)) return { added: false, message: null };
    let o = o0;
    if (prop.Datatype === 'xsd:integer') {
      o = integerValue(o0);
      if (o === null) return { added: false, message: fill(text('integer-error'), { field: prop.InputName }) };
    }
    const next = await nextNumbers(engine);
    const subj = thingNamed(s);
    const obj = literal ? null : thingNamed(o);
    const id = `${subj.thing.ThingId}|${prop.PropertyId}|${literal ? o : obj.thing.ThingId}`;
    let candidates = null;
    if (!ws.get('Statements', id)) {
      // No row for this fact yet: a new name, a new literal value, or a link the structural candidates do not cover.
      candidates = syncCandidates(ws, next.candidate);          // a new thing's structural candidates
      if (!ws.get('Statements', id)) {
        ws.insert('Statements', { StatementId: id, Graph: ws.rows('Graphs')[0].GraphId, Subject: subj.thing.ThingId, Property: prop.PropertyId,
          ObjectThing: literal ? null : obj.thing.ThingId, ObjectValue: literal ? o : null, IsListed: false, IsBuiltIn: false, IsDetail: false,
          IsStated: false, ActivationSeq: 0, SortOrder: candidates.nextSortOrder });
        const more = syncCandidates(ws, candidates.nextSortOrder + 1);   // and the ways a rule can read this fact
        candidates = { added: candidates.added + more.added + 1, removed: candidates.removed + more.removed };
      }
    }
    addToList(id, next);
    setFact(id, true, next);
    const message = prop.CurrentValueWording ? fill(text('form-replaced'), { subject: s, kind: prop.CurrentValueWording, value: o }) : '';
    return { added: true, message, candidates };
  }

  return { page, tools, switchFact, switchRule, removeFact, runScenario, addFact, nextNumbers, text };
}
