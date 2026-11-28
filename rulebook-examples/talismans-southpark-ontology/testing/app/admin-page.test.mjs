// The admin page itself (app/public/admin.js) in a real browser: what a person does with the
// mouse, not just what the API accepts.
//
// Run: node --test testing/app/admin-page.test.mjs
// Nothing here writes the project's rulebook: the app runs on a temporary copy.
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { PROJECT, startApp } from '../her-app/lib.mjs';
import { openPage } from './browser.mjs';

const HUB = path.join(PROJECT, 'effortless-rulebook', 'talismans-southpark-ontology-rulebook.json');
function copyOfHub() {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-admin-page-'));
  const file = path.join(dir, 'talismans-southpark-ontology-rulebook.json');
  fs.copyFileSync(HUB, file);
  return file;
}
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const idle = "document.getElementById('table-name').textContent && !document.body.classList.contains('busy')";

test('Add a row: open the new line, click into it, fill it in, add it', async () => {
  const port = 43284;
  const app = await startApp({ engine: 'typescript', port, env: { RULEBOOK_PATH: copyOfHub() } });
  const page = await openPage(`http://127.0.0.1:${port}/admin.html`);
  try {
    await page.until(idle, 'the admin page, loaded');
    await page.run(`document.querySelector('#tables [data-table="Rules"]').click()`);
    await page.until(`document.getElementById('table-name').textContent === 'Rules' && !document.body.classList.contains('busy')`, 'the Rules table');

    await page.run(`document.getElementById('add-row').click()`);
    assert.ok(await page.run(`!!document.getElementById('new-row')`), 'Add a row opens a new line at the top of the grid');

    // A person clicks into each box of the new line to type in it. The line must stay.
    for (const field of ['RuleId', 'Label', 'IsEnabled']) {
      await page.run(`document.querySelector('#new-row [data-new="${field}"]').click()`);
      await sleep(300);
      await page.until(idle);
      assert.ok(await page.run(`!!document.getElementById('new-row')`), `the new line is still there after clicking into ${field}`);
    }
    await page.run(`(() => {
      const set = (f, v) => { const el = document.querySelector('#new-row [data-new="' + f + '"]'); el.value = v; el.dispatchEvent(new Event('input', { bubbles: true })); el.dispatchEvent(new Event('change', { bubbles: true })); };
      set('RuleId', 'page-test-rule'); set('Label', 'Added from the page'); set('SortOrder', '9');
    })()`);
    await page.run(`document.getElementById('new-row-add').click()`);
    await page.until(`[...document.querySelectorAll('#grid tbody code')].some((c) => c.textContent === 'page-test-rule')`, 'the added row in the grid');
    assert.equal(await page.run(`document.getElementById('message').classList.contains('bad')`), false, 'no error is shown');

    const rules = (await app.api('GET', '/api/admin/tables/Rules')).rows;
    const added = rules.find((r) => r.RuleId === 'page-test-rule');
    assert.ok(added, 'the server has the new row');
    assert.equal(added.Label, 'Added from the page');
    assert.equal(added.SortOrder, 9);
  } finally {
    app.stop();
    await page.close();
  }
});
