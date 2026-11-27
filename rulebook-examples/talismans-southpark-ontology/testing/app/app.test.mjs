// The app's own behaviour, beyond matching the original page: the live rows and
// Save, the candidate generator, and admin edits that the original page cannot
// make (a new property, a new name with its portrait, new wording), on more than
// one engine.
//
// Run: node --test testing/app/
// Nothing here writes the project's rulebook: saves go to a temporary copy.
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { spawnSync } from 'node:child_process';
import { Workspace } from '../../app/lib/workspace.mjs';
import { expectedCandidates, syncCandidates } from '../../app/lib/candidates.mjs';
import { PROJECT, startApp } from '../her-app/lib.mjs';

const HUB = path.join(PROJECT, 'effortless-rulebook', 'talismans-southpark-ontology-rulebook.json');
function copyOfHub() {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-hub-'));
  const file = path.join(dir, 'talismans-southpark-ontology-rulebook.json');
  fs.copyFileSync(HUB, file);
  return file;
}
const ids = (rows, key) => rows.map((r) => r[key]).sort();

test('the rulebook file stores exactly the candidates the generator produces', () => {
  const ws = new Workspace(HUB);
  const expected = expectedCandidates(ws);
  for (const [table, key] of [['Statements', 'StatementId'], ['Firings', 'FiringId'], ['DisjointChecks', 'DisjointCheckId']]) {
    assert.deepEqual(ids(ws.rows(table), key), [...expected[table].keys()].sort(), table);
  }
  for (const row of ws.rows('Firings')) assert.deepEqual(row, expected.Firings.get(row.FiringId));
  for (const row of ws.rows('DisjointChecks')) assert.deepEqual(row, expected.DisjointChecks.get(row.DisjointCheckId));
  assert.deepEqual(syncCandidates(ws, 200000), { added: 0, removed: 0, nextSortOrder: 200000 });
  assert.equal(ws.changes().total, 0);
});

test('a new individual gets its candidates, and loses them when it goes', () => {
  const ws = new Workspace(HUB);
  const before = { s: ws.rows('Statements').length, f: ws.rows('Firings').length, d: ws.rows('DisjointChecks').length };
  ws.insert('Things', { ThingId: 'Wendy', Label: 'Wendy', Kind: 'individual', SortOrder: 99, Portrait: null });
  const added = syncCandidates(ws, 500000);
  // 7 individuals before: 6 object properties in both directions with each of them, and membership of 2 classes.
  assert.equal(ws.rows('Statements').length - before.s, 6 * 2 * 7 + 2);
  assert.equal(ws.rows('DisjointChecks').length - before.d, 2 * 7);
  assert.ok(ws.get('Firings', 'chain:Marvin|isParentOf|Randy+Randy|isParentOf|Wendy>Marvin|isGrandparentOf|Wendy'));
  assert.equal(ws.get('Statements', 'Wendy|isFriendOf|Stan').SortOrder >= 500000, true);
  assert.equal(added.removed, 0);
  ws.remove('Things', 'Wendy');
  syncCandidates(ws, 600000);
  assert.deepEqual({ s: ws.rows('Statements').length, f: ws.rows('Firings').length, d: ws.rows('DisjointChecks').length }, before);
  assert.equal(ws.changes().total, 0);
});

test('the workspace refuses what the rulebook does not allow', () => {
  const ws = new Workspace(HUB);
  assert.throws(() => ws.patch('Statements', 'Marvin|isParentOf|Randy', { Holds: true }), /is computed/);
  assert.throws(() => ws.patch('Statements', 'Marvin|isParentOf|Randy', { Subject: 'Nobody' }), /Things has no row "Nobody"/);
  assert.throws(() => ws.patch('Rules', 'inverse', { SortOrder: 'first' }), /whole number/);
  assert.throws(() => ws.patch('Rules', 'inverse', { RuleId: 'other' }), /key .* cannot be changed/);
  assert.throws(() => ws.insert('Rules', { RuleId: 'inverse' }), /already has a row/);
  assert.throws(() => ws.patch('Nope', 'x', {}), /no table Nope/);
});

test('Save writes the stored rows into the file, as UTF-8, and only when nobody else changed it', () => {
  const file = copyOfHub();
  const ws = new Workspace(file);
  ws.patch('Rules', 'inverse', { IsEnabled: true });
  ws.patch('PageTexts', 'title', { Text: 'South Park facts — café edition' });
  assert.equal(ws.changes().total, 2);
  assert.equal(ws.save(), 2);
  assert.equal(ws.changes().total, 0);
  const text = fs.readFileSync(file, 'utf8');
  assert.ok(text.includes('café edition') && !text.includes('\\u00e9'), 'non-ASCII text is written as is, not escaped');
  const doc = JSON.parse(text), original = JSON.parse(fs.readFileSync(HUB, 'utf8'));
  assert.deepEqual(doc.Rules.schema, original.Rules.schema, 'the schema is untouched');
  assert.deepEqual(Object.keys(doc.Statements.data[0]), new Workspace(file).table('Statements').stored, 'rows hold stored fields only');
  const again = new Workspace(file);
  assert.equal(again.get('Rules', 'inverse').IsEnabled, true);
  // Someone else edits the file: this app must not overwrite it.
  ws.patch('Rules', 'symmetric', { IsEnabled: true });
  fs.appendFileSync(file, '\n');
  assert.equal(ws.hubChangedOnDisk(), true);
  assert.throws(() => ws.save(), /changed on disk/);
});

for (const engine of ['typescript', 'owl', 'postgres']) {
  test(`admin edits reach the page on the ${engine} engine: a new property, a new name, new wording`, async () => {
    const file = copyOfHub();
    const app = await startApp({ engine, port: 43296, env: { RULEBOOK_PATH: file } });
    try {
      const { api } = app;
      // 1. A property the original page does not have, wired to the Symmetric rule by a row.
      const made = await api('POST', '/api/admin/tables/Properties/rows', { PropertyId: 'isSiblingOf', Label: 'is sibling of', PropertyKind: 'object',
        IsUserAddable: true, HasOneCurrentValue: false, IsSymmetric: true, SortOrder: 11 });
      assert.equal(made.candidates.added, 42 + 42, 'a candidate fact for every ordered pair, and a symmetric application for each');
      let page = await api('GET', '/api/page');
      assert.ok(page.properties.some((p) => p.label === 'is sibling of'), 'the Add fact form offers it');
      // 2. A fact with it, about a name the rulebook has never seen but a portrait answers to.
      await api('POST', '/api/rules/symmetric/switch', { on: true });
      const added = await api('POST', '/api/facts', { subject: 'Stan', property: 'isSiblingOf', object: 'Kenny McCormick' });
      assert.equal(added.added, true);
      page = added.page;
      const kenny = page.things.find((t) => t.label === 'Kenny McCormick');
      assert.match(kenny.portrait_image, /Kenny-mccormick\.png/, 'the new thing found its portrait by name');
      const cards = page.conclusions.map((c) => `${c.target_subject_label} ${c.target_property_label} ${c.target_object_text} | ${c.card_text}`);
      assert.ok(cards.includes('Kenny McCormick is sibling of Stan | Worked out by the symmetric rule from “Stan is sibling of Kenny McCormick”.'), cards.join('\n'));
      // 3. Wording is a row too.
      await api('PATCH', '/api/admin/tables/FiringKinds/rows/symmetric', { Wording: 'both-ways rule' });
      page = await api('GET', '/api/page');
      assert.ok(page.conclusions.some((c) => c.card_text.startsWith('Worked out by the both-ways rule from')));
      // 4. Save writes it all into the (temporary) rulebook; Discard after a further edit returns to what was saved.
      const saved = await api('POST', '/api/admin/save');
      assert.ok(saved.rows > 100);
      assert.equal(JSON.parse(fs.readFileSync(file, 'utf8')).Properties.data.some((p) => p.PropertyId === 'isSiblingOf'), true);
      await api('POST', '/api/rules/inverse/switch', { on: true });
      assert.equal((await api('GET', '/api/admin/changes')).changes.total, 1);
      await api('POST', '/api/admin/discard');
      page = await api('GET', '/api/page');
      assert.equal(page.unsaved, 0);
      assert.ok(page.properties.some((p) => p.label === 'is sibling of'), 'the saved property is still there');
      assert.equal(page.rules.find((r) => r.rule_id === 'inverse').is_enabled, false);
      // 5. Every available engine still agrees on every computed value, by the harness's rule.
      const compared = await api('POST', '/api/conformance');
      assert.ok(compared.engines.length >= 2);
      assert.equal(compared.differingCount + compared.missingRowCount, 0, JSON.stringify(compared.differing.slice(0, 5), null, 1));
    } finally {
      app.stop();
    }
  });
}

// The TypeScript engine is the generated SDK called inside the app's own process:
// a click is one evaluate() over the live rows. No Python, no second process, no
// files. The server runs here with only node, npm and /bin (npm's shell) on its
// PATH (npm builds the SDK's dist/ if the build left it stale), and a ticked rule
// must still reach the page, quickly. (Until 2026-11-26 every click went
// python3 -> node -> temp files, about 0.5 s each.)
test('the TypeScript engine computes in the app process, with no python3 on the PATH', async () => {
  const bin = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-bin-'));
  for (const tool of ['node', 'npm']) {
    const found = spawnSync('which', [tool], { encoding: 'utf8' }).stdout.trim();
    assert.ok(found, `${tool} is on this machine`);
    fs.symlinkSync(found, path.join(bin, tool));
  }
  const PATH = `${bin}:/bin`;
  assert.notEqual(spawnSync('/usr/bin/which', ['python3'], { env: { PATH } }).status, 0, 'python3 is not reachable on this PATH');
  const app = await startApp({ engine: 'typescript', port: 43294, env: { RULEBOOK_PATH: copyOfHub(), PATH } });
  try {
    const { api } = app;
    assert.deepEqual((await api('GET', '/api/page')).conclusions, [], 'the stored rows have every rule off');
    const timings = [];
    for (const on of [true, false, true]) {
      await api('POST', '/api/rules/inverse/switch', { on });
      const started = Date.now();
      const page = await api('GET', '/api/page');
      timings.push(Date.now() - started);
      const cards = page.conclusions.map((c) => `${c.target_subject_label} ${c.target_property_label} ${c.target_object_text}`).sort();
      assert.deepEqual(cards, on ? ['Randy is child of Marvin', 'Shelly is child of Randy', 'Stan is child of Randy'] : []);
    }
    const ts = (await api('GET', '/api/health')).engines.find((e) => e.id === 'typescript');
    assert.ok(ts.lastMs < 300, `one evaluation took ${ts.lastMs} ms`);
    assert.ok(Math.max(...timings) < 1000, `the page answered in ${timings.join(', ')} ms`);
  } finally {
    app.stop();
  }
});

// "Compare the engines" shows how long each engine took to compute the live rows. An
// engine that already computed them for the page answers the compare from its cache, so
// timing that answer measured a cache read: until 2026-11-26 the page showed
// "Postgres views 6009 ms · TypeScript 8 ms · OWL and SHACL rules 8 ms" when the
// computations had taken 5979, 95 and 1823 ms.
test('Compare the engines reports each engine\'s computation time, not a cache read', async () => {
  const port = 43293;
  const app = await startApp({ port, env: { RULEBOOK_PATH: copyOfHub() } });
  try {
    const { api } = app;
    const available = (await api('GET', '/api/engines')).engines.filter((e) => e.ok);
    assert.ok(available.length >= 2, 'at least two engines to compare');
    for (const e of available) {
      const r = await fetch(`http://127.0.0.1:${port}/api/page?backend=${e.id}`);
      assert.equal(r.status, 200, `the page on ${e.id}`);
    }
    const compared = await api('POST', '/api/conformance');
    const computed = Object.fromEntries((await api('GET', '/api/engines')).engines.map((e) => [e.id, e.lastMs]));
    for (const e of compared.engines) assert.equal(typeof computed[e.id], 'number', `${e.id} computed the rows`);
    const wrong = compared.engines.filter((e) => e.ms !== computed[e.id]).map((e) => `${e.label}: shown ${e.ms} ms, the computation took ${computed[e.id]} ms`);
    assert.deepEqual(wrong, []);
  } finally {
    app.stop();
  }
});

// Excel, both ways, through the catalog's own tools: the app's export with one cell edited
// comes back as exactly that one change. The workbook holds about 31,000 formula cells, each
// saved with its result; the import reads those and computes nothing, so it answers in
// seconds. (Until 2026-11-26 the catalog's xlsx-to-rulebook recalculated the whole workbook
// first and never answered for this one: root finding cr-29-01.)
const EDIT_ONE_TEXT = `
import sys, zipfile
src, dst, old, new = sys.argv[1:5]
zin = zipfile.ZipFile(src)
hits = 0
with zipfile.ZipFile(dst, 'w', zipfile.ZIP_DEFLATED) as zout:
    for info in zin.infolist():
        data = zin.read(info.filename)
        if info.filename == 'xl/sharedStrings.xml':
            text = data.decode('utf-8')
            hits = text.count('>' + old + '<')
            data = text.replace('>' + old + '<', '>' + new + '<').encode('utf-8')
        zout.writestr(info, data)
if hits != 1:
    sys.exit(f'expected the text once in the workbook, found it {hits} times')
`;

test('Excel round trip: the export with one cell edited imports as that one change, in seconds', async () => {
  const file = copyOfHub();
  const port = 43295;
  const app = await startApp({ port, env: { RULEBOOK_PATH: file, XLSX_IMPORT_TIMEOUT_MS: '90000' } });
  try {
    const base = `http://127.0.0.1:${port}`;
    const exported = await fetch(`${base}/api/admin/export.xlsx`);
    assert.equal(exported.status, 200, 'the export answers');
    const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-xlsx-'));
    const original = path.join(dir, 'exported.xlsx'), edited = path.join(dir, 'edited.xlsx');
    fs.writeFileSync(original, Buffer.from(await exported.arrayBuffer()));

    const before = new Workspace(file).get('PageTexts', 'admin-title').Text;
    const after = `${before} (edited in Excel)`;
    const edit = spawnSync('python3', ['-c', EDIT_ONE_TEXT, original, edited, before, after], { encoding: 'utf8' });
    assert.equal(edit.status, 0, edit.stderr);

    const started = Date.now();
    // Sent exactly as the admin page sends it (app/public/admin.js): the file's bytes as an
    // ArrayBuffer, with NO content-type header (a browser adds none for an ArrayBuffer body).
    // The test once sent application/octet-stream, so it passed while the page answered
    // "Send the workbook as the request body" (2026-11-26).
    const asThePageSendsIt = (file) => { const b = fs.readFileSync(file); return b.buffer.slice(b.byteOffset, b.byteOffset + b.byteLength); };
    const answer = await fetch(`${base}/api/admin/import`, { method: 'POST', body: asThePageSendsIt(edited) });
    const body = await answer.json();
    const seconds = (Date.now() - started) / 1000;
    assert.equal(answer.status, 200, body.error);
    assert.ok(seconds < 60, `the import took ${seconds.toFixed(0)} s`);
    assert.deepEqual(body.changes, { total: 1, tables: { PageTexts: { added: [], changed: [{ key: 'admin-title', fields: { Text: [before, after] } }], removed: [] } } });
    assert.deepEqual(body.schemaDiffers, []);
    assert.deepEqual(body.summary.warnings, [], 'the workbook carried its stored results');
    // The same workbook unedited changes nothing.
    await app.api('POST', '/api/admin/discard');
    const again = await fetch(`${base}/api/admin/import`, { method: 'POST', body: asThePageSendsIt(original) });
    const same = await again.json();
    assert.equal(again.status, 200, same.error);
    assert.equal(same.changes.total, 0);
  } finally {
    app.stop();
  }
});
