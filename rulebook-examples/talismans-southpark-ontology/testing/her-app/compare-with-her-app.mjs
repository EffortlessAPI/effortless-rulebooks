#!/usr/bin/env node
// Compares this project's Postgres views with Jessica Talisman's live South Park
// Ontology Explainer, state by state.
//
// Her page is the reference. This script downloads it, runs the page's own
// reasoner code (sliced verbatim out of the page, not copied into this repo), and
// puts the rulebook's database into the same state: the same facts switched on,
// the same rule switches. It then compares what each side draws:
//   - every fact that holds (the graph's edges),
//   - every conclusion and its provenance line (the conclusions panel),
//   - every contradiction alert (the disjoint rule).
// The fact-list changes follow her page's handlers (Move Stan to grade 5, Stan
// leaves school, Show Stan's details, Clear the graph, and the two facts her page
// suggests trying). Each runs under all 32 combinations of the five rule switches.
//
// The database is the project's own dev database; the seed state is restored at
// the end. Usage: node testing/her-app/compare-with-her-app.mjs
import vm from 'node:vm';
import { execFileSync } from 'node:child_process';

const APP_URL = 'https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/';
const DB = process.env.DATABASE_URL || 'erb_talismans_southpark_ontology';

// ------------------------------------------------------------------ her page
const res = await fetch(APP_URL);
if (!res.ok) throw new Error(`Could not download her page: ${APP_URL} answered ${res.status}`);
const html = await res.text();
function slice(startMarker, endMarker) {
  const a = html.indexOf(startMarker), b = html.indexOf(endMarker, a);
  if (a < 0 || b < 0) throw new Error(`Her page changed: cannot find "${startMarker}" ... "${endMarker}"`);
  return html.slice(a, b);
}
const reasonerCode = slice('var P = {', 'function place(');           // vocabulary, facts, rules, helpers, reason()
const activateCode = slice('function activateFact(f)', "document.addEventListener('change'");
const ensureCode = slice('function ensureStanPerson()', "document.getElementById('stan-grade-five')");
const alarm = html.match(/Contradiction found\. ' \+ esc\(f\.s\) \+ '([^']*)' \+ esc\(f\.o\) \+ '([^'<]*)/);
if (!alarm) throw new Error('Her page changed: cannot find the contradiction alert wording');
const ctx = vm.createContext({ document: { getElementById: () => ({}) } });
vm.runInContext(reasonerCode + activateCode + ensureCode, ctx);
const baseFacts = JSON.parse(JSON.stringify(ctx.facts));
const ruleIds = ctx.rules.map(r => r.id);
const unescape = s => s.replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&');

// Fact-list changes, as her page's handlers make them.
const herStates = {
  'starting facts': () => {},
  'Sharon is parent of Stan (her suggestion)': () => { const f = { s: 'Sharon', p: 'parent', o: 'Stan', on: false, builtin: false }; ctx.facts.push(f); ctx.activateFact(f); },
  'Sharon is parent of Randy (her suggestion)': () => { const f = { s: 'Sharon', p: 'parent', o: 'Randy', on: false, builtin: false }; ctx.facts.push(f); ctx.activateFact(f); },
  "Show Stan's details": () => ctx.facts.forEach(f => { f.on = !!f.builtin && !!f.detail; }),
  'Move Stan to grade 5': () => { ctx.ensureStanPerson(); const g = { s: 'Stan', p: 'hasGradeLevel', o: '5', datatype: 'xsd:integer', on: false, builtin: false }; ctx.facts.push(g); ctx.activateFact(g); },
  'Stan leaves school': () => { ctx.ensureStanPerson(); ctx.facts.forEach(f => { if (f.s === 'Stan' && ['hasRole', 'hasGradeLevel', 'attends'].includes(f.p)) f.on = false; }); },
  'Clear the graph': () => ctx.facts.forEach(f => { f.on = false; }),
};

function runHer(applyFacts, enabled) {
  ctx.facts = JSON.parse(JSON.stringify(baseFacts));
  ctx.rules.forEach(r => { r.on = enabled.includes(r.id); });
  applyFacts();
  const result = ctx.reason();
  const words = f => `${f.s} ${ctx.P[f.p]} ${f.o}`;
  return {
    facts: ctx.facts.map(f => ({ ...f })),
    holds: result.list.map(words).sort(),
    cards: Object.fromEntries(result.list.filter(f => f.inf).map(f => [words(f), unescape(ctx.prov(f))])),
    order: result.list.filter(f => f.inf).map(words),
    alarms: result.clashes.map(f => `Contradiction found. ${f.s}${unescape(alarm[1])}${f.o}${unescape(alarm[2])}`).sort(),
  };
}

// ------------------------------------------------------------------ the rulebook's database
function psql(sql) {
  return execFileSync('psql', ['-d', DB, '-v', 'ON_ERROR_STOP=1', '-q', '-At', '-f', '-'], { input: sql, encoding: 'utf8' }).trim();
}
const q = s => `'${String(s).replace(/'/g, "''")}'`;
const thingByLabel = Object.fromEntries(JSON.parse(psql("SELECT json_agg(json_build_array(label, thing_id)) FROM things")));
const propByLabel = Object.fromEntries(JSON.parse(psql("SELECT json_agg(json_build_array(label, property_id)) FROM properties")));
const seed = JSON.parse(psql('SELECT json_agg(json_build_array(statement_id, is_listed, is_stated, activation_seq, sort_order, is_built_in)) FROM statements'));
const seedRules = JSON.parse(psql('SELECT json_agg(json_build_array(rule_id, is_enabled)) FROM rules'));

function statementId(f) {
  const prop = propByLabel[ctx.P[f.p]];
  const subject = thingByLabel[f.s];
  const object = f.datatype ? f.o : thingByLabel[f.o];
  if (!prop || !subject || object === undefined) throw new Error(`No rulebook row for her fact ${JSON.stringify(f)}`);
  return `${subject}|${prop}|${object}`;
}

function setDatabase(herFacts, enabled) {
  // Her fact list becomes listed rows in her order; a fact that is on outranks its off siblings.
  const listed = new Map(herFacts.map((f, i) => [statementId(f), { on: !!f.on, order: i + 1, builtin: !!f.builtin }]));
  const rows = seed.map(([id, , , , sortOrder]) => {
    const l = listed.get(id);
    return l ? `(${q(id)}, true, ${l.on}, ${l.on ? 1000 + l.order : l.order}, ${l.order}, ${l.builtin})`
             : `(${q(id)}, false, false, 0, ${sortOrder}, false)`;
  });
  for (const id of listed.keys()) if (!seed.some(r => r[0] === id)) throw new Error(`Her fact ${id} has no candidate row`);
  return `UPDATE statements s SET is_listed = v.l, is_stated = v.st, activation_seq = v.seq, sort_order = v.so, is_built_in = v.b
    FROM (VALUES ${rows.join(',')}) AS v(id, l, st, seq, so, b) WHERE s.statement_id = v.id;
    UPDATE rules SET is_enabled = rule_id = ANY(ARRAY[${enabled.map(q).join(',') || "''"}]::text[]);`;
}

function readDatabase() {
  return JSON.parse(psql(`SELECT json_build_object(
    'holds', (SELECT coalesce(json_agg(name ORDER BY name), '[]') FROM vw_statements WHERE holds),
    'cards', (SELECT coalesce(json_object_agg(target_name, card_text), '{}') FROM vw_firings WHERE is_chosen),
    'order', (SELECT coalesce(json_agg(target_name ORDER BY order_key), '[]') FROM vw_firings WHERE is_chosen),
    'alarms', (SELECT coalesce(json_agg(alarm_text ORDER BY alarm_text), '[]') FROM vw_disjoint_checks WHERE violation_unit = 1))`));
}

// ------------------------------------------------------------------ compare
const subsets = [...Array(1 << ruleIds.length).keys()].map(m => ruleIds.filter((_, i) => m & (1 << i)));
let states = 0, failures = 0, orderDiffs = 0;
try {
  for (const [label, apply] of Object.entries(herStates)) {
    let stateFailures = 0;
    for (const enabled of subsets) {
      const her = runHer(apply, enabled);
      psql(setDatabase(her.facts, enabled));
      const ours = readDatabase();
      states++;
      const problems = [];
      const diff = (a, b) => a.filter(x => !b.includes(x));
      if (JSON.stringify(her.holds) !== JSON.stringify(ours.holds))
        problems.push(`holds: only hers ${JSON.stringify(diff(her.holds, ours.holds))}, only ours ${JSON.stringify(diff(ours.holds, her.holds))}`);
      for (const [fact, text] of Object.entries(her.cards))
        if (ours.cards[fact] !== text) problems.push(`card for "${fact}": hers ${JSON.stringify(text)}, ours ${JSON.stringify(ours.cards[fact] ?? null)}`);
      for (const fact of Object.keys(ours.cards)) if (!(fact in her.cards)) problems.push(`card only ours: "${fact}"`);
      if (JSON.stringify(her.alarms) !== JSON.stringify(ours.alarms))
        problems.push(`alarms: hers ${JSON.stringify(her.alarms)}, ours ${JSON.stringify(ours.alarms)}`);
      if (JSON.stringify(her.order) !== JSON.stringify(ours.order)) orderDiffs++;
      if (problems.length) {
        failures++; stateFailures++;
        if (stateFailures <= 3) console.log(`FAIL  ${label} | rules: ${enabled.join(', ') || 'none'}\n      ${problems.join('\n      ')}`);
      }
    }
    console.log(`${stateFailures ? 'FAIL' : 'ok  '}  ${label}: ${subsets.length - stateFailures} of ${subsets.length} rule combinations agree`);
  }
} finally {
  psql(`UPDATE statements s SET is_listed = v.l, is_stated = v.st, activation_seq = v.seq, sort_order = v.so, is_built_in = v.b
    FROM (VALUES ${seed.map(([id, l, st, seq, so, b]) => `(${q(id)}, ${l}, ${st}, ${seq}, ${so}, ${b})`).join(',')}) AS v(id, l, st, seq, so, b)
    WHERE s.statement_id = v.id;
    ${seedRules.map(([id, on]) => `UPDATE rules SET is_enabled = ${on} WHERE rule_id = ${q(id)};`).join('\n')}`);
}
console.log(`\n${states - failures} of ${states} states agree with her page (facts that hold, conclusion cards, contradiction alerts).`);
console.log(`${states - orderDiffs} of ${states} also list the conclusions in her page's order.`);
process.exit(failures ? 1 : 0);
