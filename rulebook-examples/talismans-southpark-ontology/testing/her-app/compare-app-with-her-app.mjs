#!/usr/bin/env node
// Clicks through Jessica Talisman's live explainer and this project's app side by side
// and compares what each would show after every click.
//
// Her page runs whole: its own inline script is downloaded and run against a stub
// document, and its own event handlers receive the clicks (fact and rule
// checkboxes, the seven buttons, Remove, and the Add fact form). This project's app
// runs as its real server against the project database, and receives the same
// clicks through its API. After each click the test compares:
//   - the fact list: each fact's words, whether its box is ticked, whether it has Remove,
//   - the rule switches,
//   - the conclusions panel: each conclusion and its provenance line, in order,
//     or the empty-panel message,
//   - the contradiction alerts,
//   - whether Add next fact is enabled,
//   - the current-record summary, the button note and the form message.
// Facts the form adds are drawn from the facts this rulebook has rows for (see the
// README: a new name or value is not supported yet).
//
// Usage: node testing/her-app/compare-app-with-her-app.mjs [walks] [steps] [seed]
import vm from 'node:vm';
import { execFileSync, spawn } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const APP_URL = 'https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/';
const DB = process.env.DATABASE_URL || 'erb_talismans_southpark_ontology';
const PORT = 43298;
const BASE = `http://127.0.0.1:${PORT}`;
const [WALKS, STEPS, SEED] = [Number(process.argv[2] || 6), Number(process.argv[3] || 40), Number(process.argv[4] || 20261126)];
const PROJECT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..');

// ------------------------------------------------------------------ her page, whole, on a stub document
const res = await fetch(APP_URL);
if (!res.ok) throw new Error(`Could not download her page: ${APP_URL} answered ${res.status}`);
const html = await res.text();
const scripts = [...html.matchAll(/<script(?![^>]*src=)[^>]*>([\s\S]*?)<\/script>/g)].map((m) => m[1]);
const pageScript = scripts.find((s) => s.includes('function reason()'));
if (!pageScript) throw new Error('Her page changed: no inline script defines reason()');

function herPage() {
  const els = {}, docListeners = {};
  const element = (id) => ({
    id, innerHTML: '', textContent: '', value: '', disabled: false, href: '', style: {}, dataset: {}, listeners: {},
    addEventListener(type, fn) { (this.listeners[type] ||= []).push(fn); },
    querySelector: () => element('inner'), setPointerCapture() {}, getScreenCTM: () => null,
  });
  const document = {
    getElementById: (id) => (els[id] ||= element(id)),
    addEventListener: (type, fn) => { (docListeners[type] ||= []).push(fn); },
  };
  vm.runInNewContext(pageScript, { document, encodeURIComponent, console });
  const fire = (type, target) => (docListeners[type] || []).forEach((fn) => fn({ target, preventDefault() {} }));
  return {
    els, el: (id) => document.getElementById(id),
    toggleFact: (i, checked) => fire('change', { dataset: { fact: String(i) }, checked }),
    toggleRule: (i, checked) => fire('change', { dataset: { rule: String(i) }, checked }),
    remove: (i) => fire('click', { dataset: { remove: String(i) } }),
    button: (id) => { const fns = els[id] && els[id].listeners.click; if (!fns) throw new Error(`Her page has no button ${id}`); fns.forEach((fn) => fn()); },
    add: (s, p, o) => {
      const el = (id) => document.getElementById(id);
      el('subj').value = s; el('pred').value = p; el('obj').value = o;
      el('add').listeners.submit.forEach((fn) => fn({ preventDefault() {} }));
    },
  };
}
const unescape = (s) => s.replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&');
const text = (h) => unescape(h.replace(/<[^>]+>/g, '')).trim();
const triple = (h) => [...h.matchAll(/<b>([\s\S]*?)<\/b><i>([\s\S]*?)<\/i><b>([\s\S]*?)<\/b>/g)].map((m) => `${unescape(m[1])} ${unescape(m[2])} ${unescape(m[3])}`)[0];

function readHer(h) {
  const items = (s) => s.split('<li>').slice(1);
  const inferred = h.el('inferred').innerHTML;
  return {
    facts: items(h.el('facts').innerHTML).map((li) => `${/ checked>/.test(li) ? '[x]' : '[ ]'} ${triple(li)}${li.includes('data-remove') ? ' (removable)' : ''}`),
    rules: items(h.el('rules').innerHTML).map((li) => `${/ checked>/.test(li) ? '[x]' : '[ ]'} ${text(li.match(/rule-name">([^<]*)</)[1])}`),
    conclusions: inferred.includes('class="empty"') ? [`(empty) ${text(inferred)}`]
      : items(inferred).map((li) => `${triple(li)} | ${text(li.match(/class="why">([\s\S]*?)<\/span><\/li>/)[1])}`),
    alarms: (h.el('alarm').innerHTML.match(/role="alert">[\s\S]*?<\/div>/g) || []).map((d) => text(d.replace('role="alert">', ''))),
    nextEnabled: !h.el('next').disabled,
    record: h.el('scenario-message').textContent,
    note: h.el('scenario-note').textContent,
    formMessage: h.el('form-message').textContent,
  };
}

// ------------------------------------------------------------------ this project's app, as its real server
function psql(sql) {
  return execFileSync('psql', ['-d', DB, '-v', 'ON_ERROR_STOP=1', '-q', '-At', '-f', '-'], { input: sql, encoding: 'utf8' }).trim();
}
const q = (s) => `'${String(s).replace(/'/g, "''")}'`;
const seedStatements = JSON.parse(psql('SELECT json_agg(json_build_array(statement_id, is_listed, is_stated, activation_seq, sort_order, is_built_in)) FROM statements'));
const seedRules = JSON.parse(psql('SELECT json_agg(json_build_array(rule_id, is_enabled)) FROM rules'));
function restoreSeed() {
  psql(`UPDATE statements s SET is_listed = v.l, is_stated = v.st, activation_seq = v.seq, sort_order = v.so, is_built_in = v.b
    FROM (VALUES ${seedStatements.map(([id, l, st, seq, so, b]) => `(${q(id)}, ${l}, ${st}, ${seq}, ${so}, ${b})`).join(',')}) AS v(id, l, st, seq, so, b)
    WHERE s.statement_id = v.id;
    ${seedRules.map(([id, on]) => `UPDATE rules SET is_enabled = ${on} WHERE rule_id = ${q(id)};`).join('\n')}`);
}

const server = spawn('node', ['server.js'], { cwd: path.join(PROJECT, 'app'), env: { ...process.env, PORT: String(PORT) }, stdio: ['ignore', 'pipe', 'pipe'] });
let serverLog = '';
server.stdout.on('data', (d) => { serverLog += d; });
server.stderr.on('data', (d) => { serverLog += d; });
async function api(method, url, body) {
  const r = await fetch(BASE + url, { method, headers: { 'content-type': 'application/json' }, body: body ? JSON.stringify(body) : undefined });
  const j = await r.json();
  if (!r.ok) throw new Error(`${method} ${url}: ${j.error}`);
  return j;
}
for (let i = 0; ; i++) {
  try { await api('GET', '/api/health'); break; } catch (e) { if (i > 50) throw new Error(`The app did not start:\n${serverLog}`); await new Promise((r) => setTimeout(r, 200)); }
}

// The app's page keeps the button note and form message itself; this mirrors app/public/app.js.
function ourApp() {
  const state = { page: null, note: '', formMessage: '' };
  const apply = (r) => { state.page = r.page; return r; };
  return {
    state,
    async load() { state.page = await api('GET', '/api/page'); },
    async toggleFact(i, on) { state.note = ''; apply(await api('POST', `/api/facts/${encodeURIComponent(state.page.facts[i].statement_id)}/switch`, { on })); },
    async toggleRule(i, on) { apply(await api('POST', `/api/rules/${encodeURIComponent(state.page.rules[i].rule_id)}/switch`, { on })); },
    async remove(i) { state.note = ''; apply(await api('POST', `/api/facts/${encodeURIComponent(state.page.facts[i].statement_id)}/remove`)); },
    async button(id) {
      const r = apply(await api('POST', `/api/scenarios/${id}/run`));
      state.note = r.note;
      if (r.clearsFormMessage) state.formMessage = '';
    },
    async add(s, propertyId, o) {
      state.note = '';
      const r = apply(await api('POST', '/api/facts', { subject: s, property: propertyId, object: o }));
      if (r.message !== null) state.formMessage = r.message;
    },
  };
}
function readOurs(o) {
  const p = o.state.page;
  return {
    facts: p.facts.map((f) => `${f.is_in_effect ? '[x]' : '[ ]'} ${f.subject_label} ${f.property_label} ${f.object_text}${f.is_built_in ? '' : ' (removable)'}`),
    rules: p.rules.map((r) => `${r.is_enabled ? '[x]' : '[ ]'} ${r.label}`),
    conclusions: p.conclusions.length ? p.conclusions.map((c) => `${c.target_subject_label} ${c.target_property_label} ${c.target_object_text} | ${c.card_text}`)
      : [`(empty) ${p.graph.conclusions_message}`],
    alarms: p.alarms.map((a) => a.alarm_text),
    nextEnabled: p.scenarios.find((s) => s.scenario_id === 'next').is_available,
    record: p.record,
    note: o.state.note,
    formMessage: o.state.formMessage,
  };
}

// ------------------------------------------------------------------ random walks
let rnd = SEED >>> 0;
const random = () => ((rnd = (rnd * 1664525 + 1013904223) >>> 0) / 4294967296);
const pick = (a) => a[Math.floor(random() * a.length)];
const BUTTONS = ['stan-grade-five', 'stan-leaves-school', 'next', 'all', 'clear', 'grade-example', 'reset-original'];

let compared = 0, failures = 0;
try {
  const first = ourApp(); restoreSeed(); await first.load();
  const people = first.state.page.things.filter((t) => t.kind === 'individual').map((t) => t.label);
  const herProps = Object.entries(vm.runInNewContext(`(${pageScript.match(/var P = (\{[\s\S]*?\});/)[1]})`));
  const propByLabel = Object.fromEntries(first.state.page.properties.map((p) => [p.label, p.property_id]));
  const literalValues = { 'has age': ['10', '010', 'ten'], 'has grade level': ['4', '5', '05', 'banana'], 'has role': ['Student'] };

  for (let walk = 1; walk <= WALKS; walk++) {
    const her = herPage(), ours = ourApp();
    restoreSeed(); await ours.load();
    const log = [];
    for (let step = 0; step <= STEPS; step++) {
      let action = 'starting state';
      if (step > 0) {
        const facts = ours.state.page.facts, kind = pick(['fact', 'fact', 'rule', 'rule', 'button', 'button', 'add', 'remove']);
        if (kind === 'fact') {
          const i = Math.floor(random() * facts.length), on = !facts[i].is_in_effect;
          action = `${on ? 'tick' : 'untick'} fact ${i + 1}`; her.toggleFact(i, on); await ours.toggleFact(i, on);
        } else if (kind === 'rule') {
          const i = Math.floor(random() * 5), on = !ours.state.page.rules[i].is_enabled;
          action = `${on ? 'tick' : 'untick'} rule ${ours.state.page.rules[i].label}`; her.toggleRule(i, on); await ours.toggleRule(i, on);
        } else if (kind === 'button') {
          const id = pick(BUTTONS);
          if (id === 'next' && !ours.state.page.scenarios.find((s) => s.scenario_id === 'next').is_available) continue;
          action = `press ${id}`; her.button(id); await ours.button(id);
        } else if (kind === 'remove') {
          const removable = facts.map((f, i) => [f, i]).filter(([f]) => !f.is_built_in);
          if (!removable.length) continue;
          const [, i] = pick(removable);
          action = `remove fact ${i + 1}`; her.remove(i); await ours.remove(i);
        } else {
          const [herKey, label] = pick(herProps.filter(([, l]) => propByLabel[l]));
          const literal = literalValues[label];
          const s = literal ? 'Stan' : pick(people), o = literal ? pick(literal) : pick(people);
          action = `add "${s}, ${label}, ${o}"`; her.add(s, herKey, o); await ours.add(s, propByLabel[label], o);
        }
      }
      log.push(action);
      const a = readHer(her), b = readOurs(ours);
      compared++;
      const diffs = Object.keys(a).filter((k) => JSON.stringify(a[k]) !== JSON.stringify(b[k]));
      if (diffs.length) {
        failures++;
        console.log(`FAIL  walk ${walk}, after: ${log.slice(-6).join(' > ')}`);
        for (const k of diffs) console.log(`      ${k}\n        hers: ${JSON.stringify(a[k])}\n        ours: ${JSON.stringify(b[k])}`);
        break;
      }
    }
    console.log(`${failures ? '' : 'ok    '}walk ${walk}: ${log.length - 1} clicks compared`);
    if (failures) break;
  }
} finally {
  server.kill();
  restoreSeed();
}
console.log(`\n${compared - failures} of ${compared} screens agree with her page (seed ${SEED}).`);
process.exit(failures ? 1 : 0);
