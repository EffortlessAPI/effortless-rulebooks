// Both rulebook files are written in the platform's layout: standard JSON with single-line
// leaves, one field or one row of a table per line (scripts/rulebook_layout.py, the same text
// the generated rulebook editor's Save writes). The app's Save writes the hub through writeHub
// and the concept rulebook file through layOut; both are held here. The committed files are
// held by scripts/tests/test_rulebook_layout.py.
//
// Run: node --test testing/app/rulebook-files.test.mjs   (no server, no database)
import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { execFileSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import { readHub, storedRows, writeHub, layOut } from '../../app/lib/hub.mjs';

const PROJECT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../..');
const LAYOUT = path.resolve(PROJECT, '../../scripts/rulebook_layout.py');
const HUB = path.join(PROJECT, 'effortless-rulebook', 'talismans-southpark-ontology-rulebook.json');
const CONCEPTS = path.join(PROJECT, 'concept-rulebook', 'talismans-southpark-ontology-concepts.json');

const inLayout = (text) => execFileSync('python3', [LAYOUT, '-'], { input: text, encoding: 'utf8', maxBuffer: 256 * 1024 * 1024 }) === text;

test('Save writes the hub with one field or one row per line', () => {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-layout-'));
  const file = path.join(dir, path.basename(HUB));
  // Start from the hub as an older writer left it (one-space indent, every value on its own line).
  fs.writeFileSync(file, JSON.stringify(JSON.parse(fs.readFileSync(HUB, 'utf8')), null, 1) + '\n');
  const hub = readHub(file);
  const rows = storedRows(hub);
  rows.PageTexts[0].Text = `${rows.PageTexts[0].Text} (edited)`;
  writeHub(hub, rows);
  const text = fs.readFileSync(file, 'utf8');
  assert.ok(inLayout(text), 'the saved hub is in the layout');
  const row = text.split('\n').find((line) => line.includes(`"PageTextId": ${JSON.stringify(rows.PageTexts[0].PageTextId)}`));
  assert.ok(row && row.includes('(edited)') && row.trim().startsWith('{') && row.trim().replace(/,$/, '').endsWith('}'), 'the edited row is one line');
  fs.rmSync(dir, { recursive: true, force: true });
});

test('the concept rulebook is written in the same layout', () => {
  const doc = JSON.parse(fs.readFileSync(CONCEPTS, 'utf8'));
  const text = layOut(doc);
  assert.ok(inLayout(text));
  assert.deepEqual(JSON.parse(text), doc);
  assert.ok(text.split('\n').length < JSON.stringify(doc, null, 2).split('\n').length / 5, 'rows are not spread over lines');
});

test('layOut fails loudly on what is not a rulebook', () => {
  assert.throws(() => layOut([1, 2]), /rulebook_layout\.py could not lay out the rulebook/);
});
