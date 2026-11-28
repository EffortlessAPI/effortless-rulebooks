// The rulebook editors, from the app's side. Each form of the rulebook has the generated
// rulebook editor in a container of its own, and each form is a file that editor saves into.
//
// What holds here:
// * An editor is always a link. Running, the app says where it is; not running, the app starts
//   it (this project's own ./start.sh with the form's flag) and says how that is going, and a
//   launcher that gives up is reported with what it said.
// * A file saved from outside this app (an editor's Save, or by hand) while nothing is unsaved
//   here is taken in the next time a page asks how things stand, as the rulebook's next
//   version, in both files; the pages are told so.
// * Nothing is taken that would lose something: not over unsaved changes, not a concept file
//   saved from an earlier version, not a file the mapping refuses, not when both files changed,
//   and not a concept file that already said something else when the app started.
//
// The real editor is a Docker container that takes minutes to start, so the launcher here is a
// stand-in script; the real one was checked by hand (README, "The rulebook editors").
//
// Run: node --test testing/app/editors.test.mjs
// Nothing here writes the project's rulebook: every file is a temporary copy.
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { PROJECT, startApp } from '../her-app/lib.mjs';

const HUB = path.join(PROJECT, 'effortless-rulebook', 'talismans-southpark-ontology-rulebook.json');
function copyOfHub() {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-editors-'));
  const file = path.join(dir, 'talismans-southpark-ontology-rulebook.json');
  fs.copyFileSync(HUB, file);
  return file;
}
const conceptFileOf = (hub) => path.join(path.dirname(hub), 'talismans-southpark-ontology-concepts.json');
const read = (file) => JSON.parse(fs.readFileSync(file, 'utf8'));
const versionIn = (document) => document.__meta__.data.find((r) => r.MetaKey === 'rulebook-version').StringValue;
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const facts = (page) => page.facts.filter((f) => f.is_in_effect).map((f) => `${f.subject_label} ${f.property_label} ${f.object_text}`);
// A rulebook editor's Save rewrites the whole file: its own indent, and a computed copy baked into every row.
const asAnEditorSaves = (file, document) => {
  for (const table of Object.values(document)) {
    if (!table || !Array.isArray(table.schema) || !table.schema.some((f) => f.name === 'Name' && f.type === 'calculated')) continue;
    for (const row of table.data) row.Name = String(Object.values(row)[0]);
  }
  fs.writeFileSync(file, JSON.stringify(document, null, 2));
};

// ------------------------------------------------------------------ an editor is always a link
test('a rulebook editor is started from the app, and one that does not start says why', async () => {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-launcher-'));
  const ports = { ontology: 43290, concept: 43291 };
  // The stand-in for ./start.sh: it notes how it was called; --concept-editor answers on the
  // concept editor's port after a moment and then stays (the real one follows the container's
  // log); --editor gives up the first time, as the real one does when Docker is not running,
  // and the second time neither answers nor ends.
  const launcher = path.join(dir, 'start.sh');
  fs.writeFileSync(launcher, `#!/usr/bin/env bash
echo "$1" >> "${dir}/calls"
if [[ "$1" == "--concept-editor" ]]; then
  echo "Starting the container..."
  sleep 1
  exec node -e "require('http').createServer((q, s) => s.end('the concept rulebook editor')).listen(${ports.concept})"
fi
if [[ "$1" == "--editor" && ! -f "${dir}/tried" ]]; then
  touch "${dir}/tried"
  echo "Cannot connect to the Docker daemon. Is the docker daemon running?" >&2
  exit 3
fi
if [[ "$1" == "--editor" && ! -f "${dir}/tried-twice" ]]; then
  touch "${dir}/tried-twice"
  echo "Waiting for the container..."
  exec sleep 600
fi
exec node -e "require('http').createServer((q, s) => s.end('the rulebook editor')).listen(${ports.ontology})"
`);
  const app = await startApp({ engine: 'typescript', port: 43287, env: {
    RULEBOOK_PATH: copyOfHub(), START_SCRIPT: launcher, EDITOR_START_LIMIT_MS: '2500',
    RULEBOOK_EDITOR_UI_PORT: String(ports.ontology), CONCEPT_EDITOR_UI_PORT: String(ports.concept) } });
  const until = async (form, state) => {
    for (let i = 0; i < 60; i++) {
      const editor = await app.api('GET', `/api/editor?form=${form}`);
      if (editor.state === state) return editor;
      await sleep(250);
    }
    throw new Error(`the ${form} editor never became ${state}: ${JSON.stringify(await app.api('GET', `/api/editor?form=${form}`))}`);
  };
  try {
    // Not running: the app says so, and where it will be.
    let editor = await app.api('GET', '/api/editor?form=concept');
    assert.deepEqual(editor, { form: 'concept', url: `http://localhost:${ports.concept}`, state: 'stopped' });
    await assert.rejects(app.api('GET', '/api/editor?form=accounting'), /no rulebook editor for the form "accounting": the forms are ontology and concept/);

    // Asked for, it is started with this project's own launcher and the form's flag, and it is running when it answers.
    editor = await app.api('POST', '/api/editor/start?form=concept');
    assert.equal(editor.state, 'starting');
    editor = await until('concept', 'running');
    assert.equal(await (await fetch(editor.url)).text(), 'the concept rulebook editor');
    assert.deepEqual(fs.readFileSync(path.join(dir, 'calls'), 'utf8').trim().split('\n'), ['--concept-editor']);
    // Asked again while it runs: nothing is started twice.
    assert.equal((await app.api('POST', '/api/editor/start?form=concept')).state, 'running');
    assert.deepEqual(fs.readFileSync(path.join(dir, 'calls'), 'utf8').trim().split('\n'), ['--concept-editor']);

    // A launcher that gives up: the reason is what it said, and asking again tries again.
    await app.api('POST', '/api/editor/start?form=ontology');
    editor = await until('ontology', 'failed');
    assert.match(editor.reason, /start\.sh --editor ended \(3\) before the editor answered/);
    assert.match(editor.reason, /Cannot connect to the Docker daemon/);
    // A launcher that neither answers nor ends is stopped after its time limit, and said so.
    assert.equal((await app.api('POST', '/api/editor/start?form=ontology')).state, 'starting');
    editor = await until('ontology', 'failed');
    assert.match(editor.reason, /did not answer at http:\/\/localhost:43290 within 3 s, so its launcher was stopped\.\nWaiting for the container\.\.\./);
    await app.api('POST', '/api/editor/start?form=ontology');
    editor = await until('ontology', 'running');
    assert.equal(await (await fetch(editor.url)).text(), 'the rulebook editor');

    // The page a link goes to while its editor is not running is there, for both forms.
    for (const page of ['/editor.html?form=concept', '/editor.js']) assert.equal((await fetch(`http://127.0.0.1:43287${page}`)).status, 200, page);
  } finally {
    app.stop();
    await sleep(300);
    // The launchers go with the app.
    for (const port of Object.values(ports)) await assert.rejects(fetch(`http://127.0.0.1:${port}`), `the stand-in editor on ${port} was left running`);
  }
});

// ------------------------------------------------------------------ the concept rulebook file, saved from outside
test('a concept rulebook saved from its editor is taken in as the next version, and never over something it would lose', async () => {
  const hub = copyOfHub(), conceptFile = conceptFileOf(hub);
  let app = await startApp({ engine: 'typescript', port: 43288, env: { RULEBOOK_PATH: hub } });
  try {
    const { api } = app;
    const start = (await api('GET', '/api/admin/changes')).version;
    await api('POST', '/api/admin/save');                                   // nothing to save: the concept file is written beside the rulebook
    let state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.file.exists, state.file.changedOnDisk, state.news], [start, true, false, null]);

    // 1. Saved from the editor: a new person and a fact about them. Taken in at the next look.
    let document = read(conceptFile);
    document.Things.data.push({ ThingId: 'Butters', Label: 'Butters', SortOrder: 30, Portrait: null });
    document.Person.data.push({ PersonId: 'Butters', Thing: 'Butters', IsAsserted: true, hasGradeLevel: 4, hasRole: null });
    document.isFriendOf.data.push({ isFriendOfId: 'made in the editor', Subject: 'Butters', Object: 'Stan' });
    asAnEditorSaves(conceptFile, document);
    state = await api('GET', '/api/admin/changes');
    assert.equal(state.version, start + 1, 'the editor\'s Save is the rulebook\'s next version');
    assert.equal(state.changes.total, 0, 'and it is saved, not left as unsaved changes');
    assert.deepEqual([state.news.form, state.news.bad, state.file.changedOnDisk], ['concept', false, false]);
    assert.match(state.news.message, new RegExp(`^Rulebook v${start + 1}: .*concepts\\.json was saved from outside this app .* and has been taken into the ontology\\.$`));
    assert.equal(versionIn(read(hub)), String(start + 1), 'the rulebook file holds it');
    assert.ok(read(hub).Statements.data.some((s) => s.StatementId === 'Butters|isFriendOf|Stan' && s.IsStated === true));
    assert.equal(versionIn(read(conceptFile)), String(start + 1), 'and so does the concept rulebook file');
    assert.deepEqual(read(conceptFile), (await api('GET', '/api/concepts')).concepts, 'which is again the concept form of what was saved');
    const page = await api('GET', '/api/page');
    assert.ok(facts(page).includes('Butters is friend of Stan') && page.version === start + 1, 'the explainer page shows it');

    // 2. Saved again saying the same (the editor rewrites the whole file): nothing happens.
    const news = state.news.id;
    asAnEditorSaves(conceptFile, read(conceptFile));
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.news.id, state.file.changedOnDisk, state.changes.total], [start + 1, news, false, 0]);

    // 3. With unsaved changes here it waits, and says so once.
    await api('POST', '/api/rules/inverse/switch', { on: true });
    document = read(conceptFile);
    document.isFriendOf.data.push({ isFriendOfId: 'x', Subject: 'Butters', Object: 'Kyle' });
    asAnEditorSaves(conceptFile, document);
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.file.changedOnDisk, state.news.form, state.news.bad], [start + 1, true, 'concept', true]);
    assert.match(state.news.message, /changed on disk.*There are unsaved changes here, so it was not loaded by itself\./);
    assert.ok(state.changes.total > 0 && !state.changes.tables.Things, 'the unsaved changes are still only the reader\'s own');
    const waiting = state.news.id;
    assert.equal((await api('GET', '/api/admin/changes')).news.id, waiting, 'said once');

    // 4. Saved here meanwhile, the rulebook has moved on: the file was saved from an earlier
    //    version, and laying it over this one would undo the save. It is the reader's to load.
    const saved = await api('POST', '/api/admin/save');
    assert.deepEqual([saved.version, saved.concept.written], [start + 2, false]);
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.changes.total, state.file.changedOnDisk], [start + 2, 0, true]);
    assert.equal(state.file.why, `It was saved from rulebook v${start + 1} and the rulebook is now v${start + 2}, so loading it would undo what was saved since.`);
    assert.match(state.news.message, /saved from rulebook v/);
    assert.ok(read(conceptFile).isFriendOf.data.some((f) => f.Object === 'Kyle'), 'the file is as the editor left it');
    //    Loaded by hand it is unsaved changes, for the reader to look at and save.
    const loaded = await api('POST', '/api/concepts/load-file');
    assert.ok([...loaded.changes.tables.Statements.added, ...loaded.changes.tables.Statements.changed.map((c) => c.key)].includes('Butters|isFriendOf|Kyle'));
    assert.deepEqual([loaded.file.changedOnDisk, loaded.file.why], [false, null]);
    await api('POST', '/api/admin/discard');
    await api('POST', '/api/concepts/write-file');

    // 5. A file the mapping refuses: nothing changes, the reason is said, and it is not tried again.
    fs.writeFileSync(conceptFile, '{"not": "a concept rulebook"}');
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.changes.total, state.file.changedOnDisk, state.news.bad], [start + 2, 0, true, true]);
    assert.match(state.file.why, /^It could not be loaded: /);
    const refused = state.news.id;
    assert.equal((await api('GET', '/api/admin/changes')).news.id, refused, 'said once');
    assert.ok((await api('GET', '/api/page')).facts.length > 0, 'the page still answers');
    const written = await api('POST', '/api/concepts/write-file');
    assert.deepEqual([written.file.changedOnDisk, written.file.why], [false, null]);

    // 6. Edited while the app was not running: when it starts, the file says something else than
    //    the rulebook, and there is no telling which is the newer. It waits for the reader.
    app.stop();
    await sleep(300);
    document = read(conceptFile);
    document.isFriendOf.data.push({ isFriendOfId: 'y', Subject: 'Butters', Object: 'Shelly' });
    asAnEditorSaves(conceptFile, document);
    app = await startApp({ engine: 'typescript', port: 43288, env: { RULEBOOK_PATH: hub } });
    state = await app.api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.changes.total, state.file.changedOnDisk], [start + 2, 0, true]);
    assert.match(state.file.why, /already said something other than the rulebook when this app started/);
    assert.ok(read(conceptFile).isFriendOf.data.some((f) => f.Object === 'Shelly'), 'and it is not overwritten');
    await app.api('POST', '/api/rules/symmetric/switch', { on: true });
    assert.equal((await app.api('POST', '/api/admin/save')).concept.written, false, 'not by Save either');
    //    One that agrees with the rulebook is the app's own: nothing waits.
    await app.api('POST', '/api/concepts/write-file');
    app.stop();
    await sleep(300);
    asAnEditorSaves(conceptFile, read(conceptFile));
    app = await startApp({ engine: 'typescript', port: 43288, env: { RULEBOOK_PATH: hub } });
    state = await app.api('GET', '/api/admin/changes');
    assert.deepEqual([state.file.changedOnDisk, state.file.why, state.news], [false, null, null]);
  } finally {
    app.stop();
  }
});

// ------------------------------------------------------------------ the rulebook file, saved from outside
test('a rulebook saved from its editor is taken in, stamped as the next version, and the concept rulebook follows', async () => {
  const hub = copyOfHub(), conceptFile = conceptFileOf(hub);
  const app = await startApp({ engine: 'typescript', port: 43289, env: { RULEBOOK_PATH: hub } });
  try {
    const { api } = app;
    const start = (await api('GET', '/api/admin/changes')).version;
    await api('POST', '/api/admin/save');

    // 1. Saved from the editor with a label changed and a new thing; the version number left alone.
    let document = read(hub);
    document.Things.data.find((t) => t.ThingId === 'Marvin').Label = 'Grandpa Marvin';
    document.Things.data.push({ ...document.Things.data.find((t) => t.ThingId === 'Stan'), ThingId: 'Butters', Label: 'Butters', SortOrder: 30, Portrait: null });
    asAnEditorSaves(hub, document);
    let state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.changes.total, state.hubChangedOnDisk, state.news.form, state.news.bad], [start + 1, 0, false, 'ontology', false]);
    assert.match(state.news.message, new RegExp(`^Rulebook v${start + 1}: .*rulebook\\.json was saved from outside this app`));
    assert.equal(versionIn(read(hub)), String(start + 1), 'the file is stamped');
    const page = await api('GET', '/api/page');
    assert.ok(page.things.some((t) => t.label === 'Grandpa Marvin') && page.version === start + 1, 'the explainer page shows it');
    //    The candidate facts the new thing implies were made and saved with it.
    assert.ok(read(hub).Statements.data.some((s) => s.StatementId === 'Butters|isFriendOf|Stan'), 'the new thing has its candidate facts');
    //    The concept rulebook follows.
    assert.equal(read(conceptFile).Things.data.find((t) => t.ThingId === 'Marvin').Label, 'Grandpa Marvin');
    assert.deepEqual([state.file.changedOnDisk, versionIn(read(conceptFile))], [false, String(start + 1)]);

    // 2. Saved again saying the same: no new version, nothing said.
    const news = state.news.id;
    asAnEditorSaves(hub, read(hub));
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.news.id, state.hubChangedOnDisk], [start + 1, news, false]);

    // 3. A file that carries another version number is that version: it is not stamped again.
    document = read(hub);
    document.Things.data.find((t) => t.ThingId === 'Marvin').Label = 'Marvin';
    document.__meta__.data.find((r) => r.MetaKey === 'rulebook-version').StringValue = '40';
    asAnEditorSaves(hub, document);
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, versionIn(read(hub)), state.hubChangedOnDisk], [40, '40', false]);

    // 4. Over unsaved changes here it waits; Discard changes takes it.
    await api('POST', '/api/rules/inverse/switch', { on: true });
    document = read(hub);
    document.Things.data.find((t) => t.ThingId === 'Randy').Label = 'Randy M.';
    asAnEditorSaves(hub, document);
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.hubChangedOnDisk, state.news.bad], [40, true, true]);
    assert.match(state.news.message, /The rulebook file changed on disk/);
    assert.ok(state.changes.total > 0);
    await api('POST', '/api/admin/discard');
    assert.ok((await api('GET', '/api/page')).things.some((t) => t.label === 'Randy M.'));

    // 5. A file caught while it is being written does not parse: nothing is said, it is looked at
    //    again. One that has lain like that is refused: nothing changes, it is said once, and the
    //    page still answers.
    const whole = fs.readFileSync(hub, 'utf8'), quiet = (await api('GET', '/api/admin/changes')).news.id;
    fs.writeFileSync(hub, whole.slice(0, 5000));
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.hubChangedOnDisk, state.news.id], [true, quiet], 'half written a moment ago: not a word');
    const aWhileAgo = new Date(Date.now() - 60000);
    fs.utimesSync(hub, aWhileAgo, aWhileAgo);
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.hubChangedOnDisk, state.news.form, state.news.bad], [true, 'ontology', true]);
    assert.match(state.news.message, /changed on disk and could not be loaded: /);
    const refused = state.news.id;
    assert.equal((await api('GET', '/api/admin/changes')).news.id, refused, 'said once');
    assert.ok((await api('GET', '/api/page')).facts.length > 0);
    //    Written out whole, it is taken.
    fs.writeFileSync(hub, whole);
    state = await api('GET', '/api/admin/changes');
    assert.equal(state.hubChangedOnDisk, false);

    // 6. Both files saved from outside at once: neither is taken, and the pages say so.
    const before = state.version;
    document = read(hub);
    document.Things.data.find((t) => t.ThingId === 'Sharon').Label = 'Sharon M.';
    asAnEditorSaves(hub, document);
    const concepts = read(conceptFile);
    concepts.Things.data.find((t) => t.ThingId === 'Stan').Label = 'Stanley';
    asAnEditorSaves(conceptFile, concepts);
    state = await api('GET', '/api/admin/changes');
    assert.deepEqual([state.version, state.hubChangedOnDisk, state.file.changedOnDisk, state.news.bad], [before, true, true, true]);
    assert.match(state.news.message, /Both rulebook files changed on disk/);
  } finally {
    app.stop();
  }
});

// The first look at the concept file lays it over the rows, which takes an engine. The first
// request a page makes names none, and the default engine (Postgres) may not be there: the look
// then takes an engine that is, and the app's own file is not mistaken for someone's edit.
test('the first look at the concept rulebook file does not need the default engine', async () => {
  const hub = copyOfHub(), port = 43292;
  fs.copyFileSync(path.join(PROJECT, 'concept-rulebook', 'talismans-southpark-ontology-concepts.json'), conceptFileOf(hub));
  const app = await startApp({ engine: 'typescript', port, env: { RULEBOOK_PATH: hub, DATABASE_URL: 'postgresql://postgres@localhost:5432/erb_no_such_database' } });
  try {
    assert.equal(app.engines.find((e) => e.id === 'postgres').ok, false, 'Postgres, the default engine, is not there');
    assert.equal((await fetch(`http://127.0.0.1:${port}/api/rows`)).status, 200);          // as a page starts: no engine named
    const state = await app.api('GET', '/api/admin/changes');
    assert.deepEqual([state.file.exists, state.file.changedOnDisk, state.file.why, state.news], [true, false, null, null]);
    await app.api('POST', '/api/rules/inverse/switch', { on: true });
    assert.equal((await app.api('POST', '/api/admin/save')).concept.written, true, 'and Save writes it');
  } finally {
    app.stop();
  }
});
