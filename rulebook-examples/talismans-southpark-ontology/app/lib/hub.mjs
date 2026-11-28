// The rulebook file (the hub): reading its schema and stored rows, and writing
// stored rows back. The file is the single source of truth; this module never
// computes a value.
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import crypto from 'node:crypto';
import { execFileSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';

const STORED_TYPES = new Set(['raw', 'relationship']);
const hashOf = (text) => crypto.createHash('sha256').update(text).digest('hex');

// A rulebook as the text of its file: standard JSON with single-line leaves (one field or one
// row per line), written by the platform's one layout, scripts/rulebook_layout.py, the same
// text the generated rulebook editor's Save writes. Both of this app's rulebook files go through it.
//
// The document is handed over as a file and the call has a time limit. Save runs this inside the
// server, where it holds up every other request until it ends: on 2026-11-27 the document was
// piped to the script's stdin, twice the script sat waiting for the end of a megabyte that never
// arrived, and the whole app stopped answering for good. Now the worst case is a Save that
// fails and says so. (RULEBOOK_LAYOUT_SCRIPT and RULEBOOK_LAYOUT_LIMIT_MS are for the test of that.)
const LAYOUT = process.env.RULEBOOK_LAYOUT_SCRIPT || path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../../../../scripts/rulebook_layout.py');
const LAYOUT_LIMIT_MS = Number(process.env.RULEBOOK_LAYOUT_LIMIT_MS || 30000);
export function layOut(doc) {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'rulebook-layout-'));
  const file = path.join(dir, 'rulebook.json');
  try {
    fs.writeFileSync(file, JSON.stringify(doc), 'utf8');
    execFileSync('python3', [LAYOUT, file], { stdio: ['ignore', 'ignore', 'pipe'], encoding: 'utf8', timeout: LAYOUT_LIMIT_MS });
    return fs.readFileSync(file, 'utf8');
  } catch (e) {
    const why = e.code === 'ETIMEDOUT' ? `it did not finish within ${LAYOUT_LIMIT_MS / 1000} s` : (e.stderr || e.message).trim().split('\n').slice(-2).join(' ');
    throw new Error(`scripts/rulebook_layout.py could not lay out the rulebook: ${why}`);
  } finally {
    fs.rmSync(dir, { recursive: true, force: true });
  }
}

// The table's key: its first non-nullable field, else its first field (the
// convention the conformance harness's discover_primary_key applies).
function keyField(schema) {
  return (schema.find((f) => f.nullable === false) || schema[0]).name;
}

export function readHub(path) {
  const text = fs.readFileSync(path, 'utf8');
  const doc = JSON.parse(text);
  const tables = {};
  for (const [name, table] of Object.entries(doc)) {
    if (!table || typeof table !== 'object' || !Array.isArray(table.schema)) continue;
    const fields = table.schema.map((f) => ({
      name: f.name, type: f.type || 'raw', datatype: f.datatype || 'string', nullable: f.nullable !== false,
      relatedTo: f.RelatedTo || null, formula: f.formula || null, description: f.Description || '',
      stored: STORED_TYPES.has(f.type || 'raw'),
    }));
    tables[name] = {
      name, description: table.Description || '', key: keyField(table.schema), fields,
      byName: Object.fromEntries(fields.map((f) => [f.name, f])),
      stored: fields.filter((f) => f.stored).map((f) => f.name),
      computed: fields.filter((f) => !f.stored).map((f) => f.name),
    };
  }
  return { path, text, hash: hashOf(text), doc, tables };
}

// The file as it is on disk right now, as a hash of its text: what tells this app that someone
// else has saved into it. Nothing is parsed, so a file caught half written is still answered for.
export const hashOnDisk = (path) => hashOf(fs.readFileSync(path, 'utf8'));

// What the file says, apart from how it is written: every table's fields and its stored rows.
// Computed copies baked into the rows, the indent, and a blank spelled "" or null do not count,
// so a save that only rewrote the file reads the same as before.
export function contentOf(hub) {
  const rows = storedRows(hub);
  for (const name of Object.keys(rows)) for (const row of rows[name]) for (const f of Object.keys(row)) if (row[f] === '') row[f] = null;
  return JSON.stringify([Object.values(hub.tables).map((t) => [t.name, t.description, t.fields]), rows]);
}

// The rows the file stores: stored fields only. Computed values the build baked
// into the rows are copies and are not read.
export function storedRows(hub) {
  const out = {};
  for (const [name, table] of Object.entries(hub.tables)) {
    out[name] = (hub.doc[name].data || []).map((row) => Object.fromEntries(table.stored.map((f) => [f, row[f] ?? null])));
  }
  return out;
}

// Writes stored rows back into the file. Schema, descriptions and table order are
// kept as they are. Computed copies are dropped: they belong to the rows that were
// there before, and the build's compile step bakes them again.
export function writeHub(hub, rows) {
  const onDisk = fs.readFileSync(hub.path, 'utf8');
  if (hashOf(onDisk) !== hub.hash) {
    throw new Error(`${hub.path} changed on disk since this app loaded it. Reload from the rulebook (losing the unsaved changes here) or save elsewhere; this app will not overwrite someone else's edit.`);
  }
  const doc = JSON.parse(onDisk);
  for (const [name, table] of Object.entries(hub.tables)) {
    if (!rows[name]) throw new Error(`No rows given for table ${name}; refusing to blank it in the rulebook`);
    doc[name].data = rows[name].map((row) => Object.fromEntries(table.stored.map((f) => [f, row[f] ?? null])));
  }
  const text = layOut(doc);
  fs.writeFileSync(hub.path, text, 'utf8');
  return readHub(hub.path);
}

// The whole rulebook with the given rows in place of its stored rows: what a
// generator (Excel, OWL...) is handed to build from the live state.
export function rulebookWithRows(hub, rows) {
  const doc = JSON.parse(hub.text);
  for (const [name, table] of Object.entries(hub.tables)) {
    doc[name].data = rows[name].map((row) => Object.fromEntries(table.stored.map((f) => [f, row[f] ?? null])));
  }
  return doc;
}
