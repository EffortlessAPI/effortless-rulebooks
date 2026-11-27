// Open world, as her page puts it: "Removing a statement alone does not say that its
// opposite is true." A fact about the cast is TRUE when it is stated or a rule concludes
// it, and otherwise not known (blank), never FALSE. Statements.Holds, HoldsBeforeChain,
// HoldsBeforeTyping and IsConclusion say so, and every engine computes them the same way
// (2026-11-27). Counts, cards and alarms read a blank as FALSE in a predicate
// (erbBlankLogic=coerce), so a rule fires only from facts that are known and the page is
// unchanged (compare-app-with-her-app.mjs holds that).
//
// Run: node --test testing/app/open-world.test.mjs
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { PROJECT, startApp } from '../her-app/lib.mjs';

const HUB = path.join(PROJECT, 'effortless-rulebook', 'talismans-southpark-ontology-rulebook.json');
function copyOfHub() {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-open-world-'));
  const file = path.join(dir, 'talismans-southpark-ontology-rulebook.json');
  fs.copyFileSync(HUB, file);
  return file;
}
const FIELDS = ['HoldsBeforeChain', 'HoldsBeforeTyping', 'Holds', 'IsConclusion'];

test('the rulebook never states that a fact about the cast is false', () => {
  const book = JSON.parse(fs.readFileSync(HUB, 'utf8'));
  const schema = Object.fromEntries(book.Statements.schema.map((f) => [f.name, f]));
  for (const name of FIELDS) {
    assert.match(schema[name].formula, /^=IF\(.*, TRUE\(\)\)$/, `${name} is TRUE or blank`);
    assert.equal(schema[name].nullable, true, `${name} may be blank`);
  }
  for (const row of book.Statements.data) for (const name of FIELDS) {
    assert.notEqual(row[name], false, `${row.StatementId}.${name} is false in the file`);
  }
});

for (const engine of ['typescript', 'owl', 'postgres']) {
  test(`${engine}: a fact is TRUE when stated or concluded, and otherwise not known`, async () => {
    const app = await startApp({ engine, port: 43297, env: { RULEBOOK_PATH: copyOfHub() } });
    const statements = async () => Object.fromEntries((await app.api('GET', '/api/admin/tables/Statements?computed=1')).rows
      .map((r) => [r.StatementId, r]));
    try {
      let s = await statements();
      // Stated: TRUE, and not a conclusion (blank, not false).
      assert.equal(s['Randy|isParentOf|Stan'].Holds, true);
      assert.equal(s['Randy|isParentOf|Stan'].IsConclusion, null);
      // Nothing states or concludes it: not known.
      for (const name of FIELDS) assert.equal(s['Marvin|isParentOf|Sharon'][name], null, name);
      assert.equal(s['Stan|isChildOf|Randy'].Holds, null, 'the inverse rule is off: not known, not false');
      for (const row of Object.values(s)) for (const name of FIELDS) assert.notEqual(row[name], false, `${row.StatementId}.${name}`);

      // A rule concludes it: TRUE, and a conclusion.
      await app.api('POST', '/api/rules/inverse/switch', { on: true });
      s = await statements();
      assert.equal(s['Stan|isChildOf|Randy'].Holds, true);
      assert.equal(s['Stan|isChildOf|Randy'].IsConclusion, true);

      // Removing a statement does not say that its opposite is true.
      await app.api('POST', '/api/facts/Randy%7CisParentOf%7CStan/switch', { on: false });
      s = await statements();
      assert.equal(s['Randy|isParentOf|Stan'].Holds, null, 'switched off: not known, not false');
      assert.equal(s['Stan|isChildOf|Randy'].Holds, null, 'nothing concludes it any more');
      for (const row of Object.values(s)) for (const name of FIELDS) assert.notEqual(row[name], false, `${row.StatementId}.${name}`);
    } finally {
      app.stop();
    }
  });
}
