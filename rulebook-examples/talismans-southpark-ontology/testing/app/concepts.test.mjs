// The concept form: the same live rows as ordinary tables (a table of individuals, a
// table per class, a table or a column per property, ERBTables and ERBFields), and
// every edit made there written as the matching change of the ontology's rows.
//
// What holds here, on each engine:
// * The concept form is a view: reading it changes nothing, and laying it back over
//   the live rows unchanged changes nothing.
// * It holds concepts, not the page: no Statements, no Properties, and the wiring
//   (symmetric, inverse, chain, disjoint) is cells of ERBTables, not a blob.
// * A new person, a new class, a new relationship to it, a new column, a fact added
//   or removed, a symmetric switch: each shows on the explainer page at once, the
//   rules conclude from it, and the concept form reads back what was put in.
// * What the ontology cannot hold is refused in words, and nothing is changed.
// * Save writes the rulebook file; every engine still agrees on every value.
// * Excel, both ways, on the concept rulebook: one cell edited in the concept
//   workbook comes back as the matching change of the ontology's rows.
//
// The mapping itself (orchestration/ontology_form.py) is tested by
// scripts/tests/test_ontology_form.py; this is the app around it.
//
// Run: node --test testing/app/concepts.test.mjs
// Nothing here writes the project's rulebook: saves go to a temporary copy.
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { spawnSync } from 'node:child_process';
import { PROJECT, startApp } from '../her-app/lib.mjs';

const HUB = path.join(PROJECT, 'effortless-rulebook', 'talismans-southpark-ontology-rulebook.json');
function copyOfHub() {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-concepts-hub-'));
  const file = path.join(dir, 'talismans-southpark-ontology-rulebook.json');
  fs.copyFileSync(HUB, file);
  return file;
}
const rows = (view, table) => view.concepts[table].data;
const row = (view, table, key) => rows(view, table).find((r) => String(Object.values(r)[0]) === key);
const role = (view, table) => view.description.tables.find((t) => t.name === table)?.role;
const facts = (page) => page.facts.filter((f) => f.is_in_effect).map((f) => `${f.subject_label} ${f.property_label} ${f.object_text}`);
const conclusions = (page) => page.conclusions.map((c) => `${c.target_subject_label} ${c.target_property_label} ${c.target_object_text}`).sort();

const PORTS = { typescript: 43281, postgres: 43282, owl: 43283 };
for (const engine of ['typescript', 'postgres', 'owl']) {
  test(`${engine}: the concept form is a view of the same rows, and an edit there shows on the explainer`, async () => {
    const file = copyOfHub();
    const app = await startApp({ engine, port: PORTS[engine], env: { RULEBOOK_PATH: file } });
    try {
      const { api } = app;
      const edit = (...edits) => api('POST', '/api/concepts/edit', edits);

      // 1. A view: concepts, not the page; reading it and laying it back change nothing.
      let view = await api('GET', '/api/concepts');
      assert.equal(view.changes.total, 0);
      assert.deepEqual(Object.keys(view.concepts).filter((k) => view.concepts[k].schema),
        ['__meta__', 'Things', 'Person', 'School', 'isParentOf', 'isChildOf', 'isSpouseOf', 'isFriendOf', 'attends', 'hasAge',
          'isGrandparentOf', 'Portraits', 'ERBTables', 'ERBFields']);
      assert.deepEqual([role(view, 'Things'), role(view, 'Person'), role(view, 'isParentOf'), role(view, 'hasAge'), role(view, 'ERBTables')],
        ['individuals', 'class', 'relationship', 'values', 'attributes']);
      assert.deepEqual(rows(view, 'isParentOf').map((r) => `${r.Subject}>${r.Object}`), ['Marvin>Randy', 'Randy>Stan', 'Randy>Shelly']);
      assert.deepEqual(row(view, 'Person', 'Stan'), { PersonId: 'Stan', Thing: 'Stan', IsAsserted: true, hasGradeLevel: 4, hasRole: 'Student' });
      const parent = row(view, 'ERBTables', 'isParentOf');
      assert.deepEqual([parent.Label, parent.InverseOf, parent.DisjointWith, parent.IsSymmetric], ['is parent of', 'isChildOf', 'isSpouseOf', false]);
      assert.equal(row(view, 'ERBTables', 'isSpouseOf').IsSymmetric, true);
      assert.deepEqual(rows(view, '__meta__').filter((r) => r.MetaKey === 'ontology-form').map((r) => [r.ValueType, r.StringValue, r.JsonValue]),
        [['string', 'concepts-of-an-ontology', null]], 'the one thing __meta__ says of the form is its name');
      const laid = await api('POST', '/api/concepts/rulebook', view.concepts);
      assert.equal(laid.changes.total, 0, 'the concept rulebook laid back over the rows it came from changes nothing');

      // 2. A new person, in a class, with a friend: on the page at once.
      let r = await edit(
        { op: 'insert', table: 'Things', values: { ThingId: 'Butters', Label: 'Butters' } },
        { op: 'insert', table: 'Person', values: { Thing: 'Butters' } },
        { op: 'insert', table: 'isFriendOf', values: { Subject: 'Butters', Object: 'Stan' } });
      assert.deepEqual(r.changes.tables.Things.added, ['Butters']);
      assert.ok(r.candidates.added > 0, 'the new individual got its candidate facts');
      let page = await api('GET', '/api/page');
      assert.ok(page.things.some((t) => t.label === 'Butters' && t.is_in_graph));
      assert.ok(facts(page).includes('Butters is friend of Stan') && facts(page).includes('Butters is a Person'));
      assert.deepEqual(row(r, 'isFriendOf', 'Butters|isFriendOf|Stan'), { isFriendOfId: 'Butters|isFriendOf|Stan', Subject: 'Butters', Object: 'Stan' });
      assert.equal(row(r, 'Person', 'Butters').IsAsserted, true);

      // 3. A different TYPE of thing: a class, a relationship whose object must be one, and a fact.
      r = await edit(
        { op: 'addClass', name: 'Concert', label: 'Concert' },
        { op: 'addProperty', name: 'performsAt', label: 'performs at', object: 'Concert' },
        { op: 'insert', table: 'Things', values: { ThingId: 'Lollapalooza', Label: 'Lollapalooza' } },
        { op: 'insert', table: 'performsAt', values: { Subject: 'Kyle', Object: 'Lollapalooza' } });
      assert.deepEqual([role(r, 'Concert'), role(r, 'performsAt')], ['class', 'relationship']);
      assert.deepEqual(r.description.properties.find((p) => p.id === 'performsAt'),
        { id: 'performsAt', home: 'table', table: 'performsAt', kind: 'object', datatype: null, single: false, domain: null, range: 'Concert', hasAttributes: true });
      // Open world: nobody said Lollapalooza is a Concert; the property's range implies it.
      assert.deepEqual(row(r, 'Concert', 'Lollapalooza'), { ConcertId: 'Lollapalooza', Thing: 'Lollapalooza', IsAsserted: false });
      page = await api('GET', '/api/page');
      assert.ok(page.properties.some((p) => p.label === 'performs at'), 'the Add fact form offers the new property');
      assert.ok(facts(page).includes('Kyle performs at Lollapalooza'));
      assert.ok(!conclusions(page).includes('Lollapalooza is a Concert'), 'with the rule off the page concludes nothing');
      page = (await api('POST', '/api/rules/domain/switch', { on: true })).page;
      assert.ok(conclusions(page).includes('Lollapalooza is a Concert'), 'the domain and range rule reads the new range');
      await api('POST', '/api/rules/domain/switch', { on: false });

      // 4. Symmetric is a cell of ERBTables: a new relationship made symmetric, and the rule reads it.
      r = await edit(
        { op: 'addProperty', name: 'isSiblingOf', label: 'is sibling of' },
        { op: 'patch', table: 'ERBTables', key: 'isSiblingOf', values: { IsSymmetric: true } },
        { op: 'insert', table: 'isSiblingOf', values: { Subject: 'Stan', Object: 'Shelly' } });
      page = (await api('POST', '/api/rules/symmetric/switch', { on: true })).page;
      assert.ok(conclusions(page).includes('Shelly is sibling of Stan'));
      r = await edit({ op: 'patch', table: 'ERBTables', key: 'isSiblingOf', values: { IsSymmetric: false } });
      page = await api('GET', '/api/page');
      assert.ok(!conclusions(page).includes('Shelly is sibling of Stan'), 'no longer symmetric');
      await api('POST', '/api/rules/symmetric/switch', { on: false });

      // 5. A column: another fact about a person, with one current value.
      r = await edit(
        { op: 'addColumn', table: 'Person', name: 'favoriteFood', label: 'favorite food', datatype: 'string' },
        { op: 'patch', table: 'Person', key: 'Stan', values: { favoriteFood: 'Cheesy Poofs' } });
      assert.equal(row(r, 'Person', 'Stan').favoriteFood, 'Cheesy Poofs');
      assert.equal(row(r, 'ERBFields', 'Person.favoriteFood').Label, 'favorite food');
      r = await edit({ op: 'patch', table: 'Person', key: 'Stan', values: { favoriteFood: 'Snacky Cakes' } });
      assert.equal(row(r, 'Person', 'Stan').favoriteFood, 'Snacky Cakes');
      page = await api('GET', '/api/page');
      assert.ok(facts(page).includes('Stan favorite food Snacky Cakes') && !facts(page).includes('Stan favorite food Cheesy Poofs'));

      // 6. A fact removed is switched off; the page keeps it in its list, unticked.
      r = await edit({ op: 'remove', table: 'isParentOf', key: 'Marvin|isParentOf|Randy' });
      assert.deepEqual(rows(r, 'isParentOf').map((x) => `${x.Subject}>${x.Object}`), ['Randy>Stan', 'Randy>Shelly']);
      page = await api('GET', '/api/page');
      const marvin = page.facts.find((f) => f.statement_id === 'Marvin|isParentOf|Randy');
      assert.equal(marvin.is_in_effect, false);
      // ...and put back, it is the same fact again.
      r = await edit({ op: 'insert', table: 'isParentOf', values: { Subject: 'Marvin', Object: 'Randy' } });
      assert.ok(row(r, 'isParentOf', 'Marvin|isParentOf|Randy'));

      // 7. A link pointed at Things: attends no longer needs a school ("or a concert, or other things").
      r = await edit(
        { op: 'retarget', table: 'attends', field: 'Object', relatedTo: 'Things' },
        { op: 'insert', table: 'attends', values: { Subject: 'Butters', Object: 'Lollapalooza' } });
      assert.equal(r.description.properties.find((p) => p.id === 'attends').range, null);
      assert.equal(row(r, 'School', 'Lollapalooza'), undefined, 'attending it does not make it a school any more');

      // 8. What the ontology cannot hold is refused in words, and nothing changes.
      const before = (await api('GET', '/api/admin/changes')).changes.total;
      await assert.rejects(() => edit({ op: 'insert', table: 'isFriendOf', values: { Subject: 'Nobody', Object: 'Stan' } }), /'Nobody', which is not in Things/);
      await assert.rejects(() => edit({ op: 'addClass', name: 'Person' }), /already is a table Person/);
      await assert.rejects(() => edit({ op: 'patch', table: 'ERBTables', key: 'isParentOf', values: { InverseOf: 'isUncleOf' } }), /'isUncleOf', which is not a property/);
      assert.equal((await api('GET', '/api/admin/changes')).changes.total, before);

      // 9. Save writes the rulebook file; the concept form of the saved rows is the same.
      const saved = await api('POST', '/api/admin/save');
      assert.ok(saved.rows > 0);
      const onDisk = JSON.parse(fs.readFileSync(file, 'utf8'));
      assert.ok(onDisk.Things.data.some((t) => t.ThingId === 'Concert' && t.Kind === 'class'));
      assert.ok(onDisk.Properties.data.some((p) => p.PropertyId === 'performsAt' && p.RangeClass === 'Concert'));
      view = await api('GET', '/api/concepts');
      assert.equal(view.changes.total, 0);
      assert.equal(row(view, 'Person', 'Stan').favoriteFood, 'Snacky Cakes');

      // 10. Every available engine still agrees on every computed value.
      const compared = await api('POST', '/api/conformance');
      assert.ok(compared.engines.length >= 2);
      assert.equal(compared.differingCount + compared.missingRowCount, 0, JSON.stringify(compared.differing.slice(0, 5), null, 1));
    } finally {
      app.stop();
    }
  });
}

// The concept rulebook is a rulebook: the catalog's rulebook-to-xlsx and xlsx-to-rulebook take
// it as they take any other. One cell edited in the concept workbook (Stan's role, a cell of
// the Person sheet) comes back as the matching change of the ontology's rows.
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
async function job(base, kind, body) {
  const answer = await fetch(`${base}/api/admin/jobs/${kind}`, { method: 'POST', ...(body ? { body } : { headers: { 'content-type': 'application/json' } }) });
  const started = await answer.json();
  assert.equal(answer.status, 200, started.error);
  for (;;) {
    const now = await (await fetch(`${base}/api/admin/jobs/${kind}`)).json();
    if (!now.running) return now;
    await new Promise((r) => setTimeout(r, 250));
  }
}

test('Excel, both ways, on the concept rulebook: a cell edited in the concept workbook is the matching change of the ontology', async () => {
  const port = 43284;
  const app = await startApp({ port, env: { RULEBOOK_PATH: copyOfHub(), XLSX_IMPORT_TIMEOUT_MS: '90000' } });
  try {
    const base = `http://127.0.0.1:${port}`;
    const exported = await job(base, 'concept-export');
    assert.equal(exported.ok, true, exported.error);
    assert.equal(exported.result.file, 'talismans-southpark-ontology-concepts.xlsx');
    const download = await fetch(`${base}/api/admin/jobs/concept-export/download`);
    assert.equal(download.status, 200, 'the finished export downloads');
    const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-concepts-xlsx-'));
    const original = path.join(dir, 'exported.xlsx'), edited = path.join(dir, 'edited.xlsx');
    fs.writeFileSync(original, Buffer.from(await download.arrayBuffer()));
    const sheets = spawnSync('python3', ['-c', 'import sys, zipfile, re; print(",".join(re.findall(r\'<sheet [^>]*name="([^"]+)"\', zipfile.ZipFile(sys.argv[1]).read("xl/workbook.xml").decode())))', original], { encoding: 'utf8' });
    for (const sheet of ['Things', 'Person', 'isParentOf', 'ERBTables', 'ERBFields']) assert.ok(sheets.stdout.split(',').includes(sheet), `the workbook has a ${sheet} sheet`);
    assert.ok(!sheets.stdout.split(',').includes('Statements'), 'and none of the ontology form\'s');

    const change = spawnSync('python3', ['-c', EDIT_ONE_TEXT, original, edited, 'Student', 'Fourth grader'], { encoding: 'utf8' });
    assert.equal(change.status, 0, change.stderr);
    const asThePageSendsIt = (file) => { const b = fs.readFileSync(file); return b.buffer.slice(b.byteOffset, b.byteOffset + b.byteLength); };

    const imported = await job(base, 'concept-import', asThePageSendsIt(edited));
    assert.equal(imported.ok, true, imported.error);
    const changed = imported.result.changes.tables;
    assert.deepEqual(changed.Statements.added, ['Stan|hasRole|Fourth grader']);
    assert.deepEqual(changed.Statements.changed, [{ key: 'Stan|hasRole|Student', fields: { IsStated: [true, false] } }]);
    assert.deepEqual(Object.keys(changed).sort(), ['Firings', 'Statements'], 'nothing else of the ontology changed');
    const view = await app.api('GET', '/api/concepts');
    assert.equal(row(view, 'Person', 'Stan').hasRole, 'Fourth grader');

    // The same workbook unedited changes nothing.
    await app.api('POST', '/api/admin/discard');
    const same = await job(base, 'concept-import', asThePageSendsIt(original));
    assert.equal(same.ok, true, same.error);
    assert.equal(same.result.changes.total, 0);
  } finally {
    app.stop();
  }
});
