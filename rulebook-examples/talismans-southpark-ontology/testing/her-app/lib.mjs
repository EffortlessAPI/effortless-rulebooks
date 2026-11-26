// Shared by the tests that compare this project with Jessica Talisman's live page:
// her page (downloaded and run), this project's app (its real server, on its own
// test database), and a seeded random generator.
import vm from 'node:vm';
import crypto from 'node:crypto';
import fs from 'node:fs';
import path from 'node:path';
import { execFileSync, spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';

export const APP_URL = 'https://southpark-ontology-explainer.jessica-tali-7056.chatgpt.site/';
export const PROJECT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..');
const SLUG = path.basename(PROJECT);
// The tests never touch the database the running app uses: they keep their own.
const TEST_DB = `erb_${SLUG.replace(/-/g, '_')}_test`;
export const TEST_DATABASE_URL = `postgresql://postgres@localhost:5432/${TEST_DB}`;

// ------------------------------------------------------------------ her page
export async function downloadHerPage() {
  const res = await fetch(APP_URL);
  if (!res.ok) throw new Error(`Could not download her page: ${APP_URL} answered ${res.status}`);
  const html = await res.text();
  const scripts = [...html.matchAll(/<script(?![^>]*src=)[^>]*>([\s\S]*?)<\/script>/g)].map((m) => m[1]);
  const script = scripts.find((s) => s.includes('function reason()'));
  if (!script) throw new Error('Her page changed: no inline script defines reason()');
  return { html, script };
}

export const unescapeHtml = (s) => s.replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&');

// Her whole page script on a stub document: its own handlers receive the clicks.
export function herPage(script) {
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
  vm.runInNewContext(script, { document, encodeURIComponent, console });
  const el = (id) => document.getElementById(id);
  const fire = (type, target) => (docListeners[type] || []).forEach((fn) => fn({ target, preventDefault() {} }));
  return {
    el,
    toggleFact: (i, checked) => fire('change', { dataset: { fact: String(i) }, checked }),
    toggleRule: (i, checked) => fire('change', { dataset: { rule: String(i) }, checked }),
    remove: (i) => fire('click', { dataset: { remove: String(i) } }),
    button: (id) => { const fns = el(id).listeners.click; if (!fns) throw new Error(`Her page has no button ${id}`); fns.forEach((fn) => fn()); },
    add: (s, p, o) => { el('subj').value = s; el('pred').value = p; el('obj').value = o; el('add').listeners.submit.forEach((fn) => fn({ preventDefault() {} })); },
  };
}

const text = (h) => unescapeHtml(h.replace(/<[^>]+>/g, '')).trim();
const triple = (h) => [...h.matchAll(/<b>([\s\S]*?)<\/b><i>([\s\S]*?)<\/i><b>([\s\S]*?)<\/b>/g)].map((m) => `${unescapeHtml(m[1])} ${unescapeHtml(m[2])} ${unescapeHtml(m[3])}`)[0];

// What her page shows, read out of the HTML it rendered.
export function readHer(h) {
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

// ------------------------------------------------------------------ this project's app
// The test database, loaded from the current build (reloaded when the build's SQL changes).
export function ensureTestDatabase() {
  const sql = ['01-drop-and-create-tables.sql', '02-create-functions.sql', '03-create-views.sql']
    .map((f) => fs.readFileSync(path.join(PROJECT, 'postgres-bootstrap', f), 'utf8')).join('\n');
  const stamp = `build:${crypto.createHash('sha256').update(sql).digest('hex').slice(0, 16)}`;
  const psql = (db, q) => execFileSync('psql', ['-d', db, '-v', 'ON_ERROR_STOP=1', '-qAt', '-c', q], { encoding: 'utf8' }).trim();
  const exists = psql('postgres', `SELECT 1 FROM pg_database WHERE datname = '${TEST_DB}'`) === '1';
  const loaded = exists && psql('postgres', `SELECT shobj_description(oid, 'pg_database') FROM pg_database WHERE datname = '${TEST_DB}'`) === stamp;
  if (loaded) return;
  console.log(`(loading the test database ${TEST_DB} from this build)`);
  if (exists) execFileSync('dropdb', [TEST_DB]);
  execFileSync('createdb', [TEST_DB]);
  execFileSync('bash', [path.join(PROJECT, 'postgres-bootstrap', 'reset-rulebook-db.sh'), TEST_DATABASE_URL], { cwd: PROJECT, stdio: 'ignore' });
  psql('postgres', `COMMENT ON DATABASE ${TEST_DB} IS '${stamp}'`);
}

// The app's real server on a test port, and a client that sends every request to one engine.
export async function startApp({ port = 43298, engine, env = {} }) {
  ensureTestDatabase();
  const base = `http://127.0.0.1:${port}`;
  const server = spawn('node', ['server.js'], { cwd: path.join(PROJECT, 'app'), env: { ...process.env, PORT: String(port), DATABASE_URL: TEST_DATABASE_URL, ...env }, stdio: ['ignore', 'pipe', 'pipe'] });
  let log = '';
  server.stdout.on('data', (d) => { log += d; });
  server.stderr.on('data', (d) => { log += d; });
  const api = async (method, url, body) => {
    const r = await fetch(base + url, { method, headers: { 'content-type': 'application/json', ...(engine ? { 'X-ERB-Backend': engine } : {}) }, body: body ? JSON.stringify(body) : undefined });
    const j = await r.json();
    if (!r.ok) throw new Error(`${method} ${url}: ${j.error}`);
    return j;
  };
  for (let i = 0; ; i++) {
    try { await api('GET', '/api/health'); break; } catch (e) {
      if (i > 60) { server.kill(); throw new Error(`The app did not start:\n${log}`); }
      await new Promise((r) => setTimeout(r, 200));
    }
  }
  const health = await api('GET', '/api/health');
  const mine = engine && health.engines.find((e) => e.id === engine);
  if (engine && !mine) { server.kill(); throw new Error(`The app has no engine ${engine}; it has ${health.engines.map((e) => e.id).join(', ')}`); }
  if (mine && !mine.ok) { server.kill(); throw new Error(`Engine ${engine} is not available: ${mine.reason}`); }
  return { api, stop: () => server.kill(), log: () => log, engines: health.engines };
}

// ------------------------------------------------------------------ a seeded random generator
export function seeded(seed) {
  let state = seed >>> 0;
  const random = () => ((state = (state * 1664525 + 1013904223) >>> 0) / 4294967296);
  return { random, pick: (a) => a[Math.floor(random() * a.length)] };
}
