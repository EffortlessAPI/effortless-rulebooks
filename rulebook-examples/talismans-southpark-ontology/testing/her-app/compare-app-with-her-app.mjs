#!/usr/bin/env node
// Clicks through Jessica Talisman's live explainer and this project's app side by
// side, on each execution engine, and compares what each would show after every
// click.
//
// Her page runs whole: its own inline script is downloaded and run against a stub
// document, and its own event handlers receive the clicks (fact and rule
// checkboxes, the seven buttons, Remove, and the Add fact form). This project's
// app runs as its real server (on the tests' own database) and receives the same
// clicks through its API, with every request sent to one engine: the Postgres
// views, the generated TypeScript, or the generated OWL and SHACL rules.
//
// After each click the test compares:
//   - the fact list: each fact's words, whether its box is ticked, whether it has Remove,
//   - the rule switches,
//   - the conclusions panel: each conclusion and its provenance line, in order,
//     or the empty-panel message,
//   - the contradiction alerts,
//   - whether Add next fact is enabled,
//   - the current-record summary, the button note and the form message.
// The Add fact form is given names and values the rulebook has never seen
// (Kenny, Mr Garrison, grade 6...) as well as the original cast.
//
// Usage: node testing/her-app/compare-app-with-her-app.mjs [--engine=postgres|typescript|owl|all] [--walks=4] [--steps=40] [--seed=20261126]
import vm from 'node:vm';
import { downloadHerPage, herPage, readHer, startApp, seeded } from './lib.mjs';

const arg = (name, fallback) => (process.argv.find((a) => a.startsWith(`--${name}=`)) || '').split('=')[1] || fallback;
const WALKS = Number(arg('walks', 4)), STEPS = Number(arg('steps', 40)), SEED = Number(arg('seed', 20261126));
const ENGINE = arg('engine', 'all');

const BUTTONS = ['stan-grade-five', 'stan-leaves-school', 'next', 'all', 'clear', 'grade-example', 'reset-original'];
const NEW_NAMES = ['Kenny', 'Butters', 'Mr Garrison'];
const LITERALS = { 'has age': ['10', '010', 'ten', '12'], 'has grade level': ['4', '5', '05', 'banana', '6'], 'has role': ['Student', 'Hall Monitor'] };

// The app's page keeps the button note and form message itself; this mirrors app/public/app.js.
function ourApp(api) {
  const state = { page: null, note: '', formMessage: '' };
  const apply = (r) => { state.page = r.page; return r; };
  return {
    state,
    async reset() { await api('POST', '/api/admin/discard'); state.page = await api('GET', '/api/page'); state.note = ''; state.formMessage = ''; },
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

const { script } = await downloadHerPage();
const herProps = Object.entries(vm.runInNewContext(`(${script.match(/var P = (\{[\s\S]*?\});/)[1]})`));
let totalCompared = 0, totalFailures = 0;
const summary = [];

async function runEngine(engine) {
  const app = await startApp({ engine });
  const { random, pick } = seeded(SEED);
  let compared = 0, failures = 0;
  const started = Date.now();
  try {
    const ours = ourApp(app.api);
    await ours.reset();
    const cast = ours.state.page.things.filter((t) => t.kind === 'individual').map((t) => t.label);
    const people = [...cast, ...NEW_NAMES];
    const propByLabel = Object.fromEntries(ours.state.page.properties.map((p) => [p.label, p.property_id]));
    for (let walk = 1; walk <= WALKS && !failures; walk++) {
      const her = herPage(script);
      await ours.reset();
      const log = [];
      for (let step = 0; step <= STEPS; step++) {
        let action = 'starting state';
        if (step > 0) {
          const facts = ours.state.page.facts, kind = pick(['fact', 'fact', 'rule', 'rule', 'button', 'button', 'add', 'add', 'remove']);
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
            const literal = LITERALS[label];
            const s = literal ? pick(['Stan', 'Stan', 'Kenny']) : pick(people), o = literal ? pick(literal) : pick(people);
            action = `add "${s}, ${label}, ${o}"`; her.add(s, herKey, o); await ours.add(s, propByLabel[label], o);
          }
        }
        log.push(action);
        const a = readHer(her), b = readOurs(ours);
        compared++;
        const diffs = Object.keys(a).filter((k) => JSON.stringify(a[k]) !== JSON.stringify(b[k]));
        if (diffs.length) {
          failures++;
          console.log(`FAIL  ${engine}, walk ${walk}, after: ${log.slice(-8).join(' > ')}`);
          for (const k of diffs) console.log(`      ${k}\n        hers: ${JSON.stringify(a[k])}\n        ours: ${JSON.stringify(b[k])}`);
          break;
        }
      }
      if (!failures) console.log(`ok    ${engine}, walk ${walk}: ${log.length - 1} clicks compared`);
    }
    await ours.reset();
  } finally {
    app.stop();
  }
  summary.push(`${engine}: ${compared - failures} of ${compared} screens agree with her page (${Math.round((Date.now() - started) / compared)} ms a click)`);
  totalCompared += compared; totalFailures += failures;
}

const engines = ENGINE === 'all' ? ['postgres', 'typescript', 'owl'] : [ENGINE];
for (const engine of engines) await runEngine(engine);
console.log(`\n${summary.join('\n')}\n(seed ${SEED}, ${WALKS} walks of up to ${STEPS} clicks per engine)`);
process.exit(totalFailures ? 1 : 0);
