// The rulebook file (the hub): reading its schema and stored rows, and writing
// stored rows back. The file is the single source of truth; this module never
// computes a value.
import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import { execFileSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';

const STORED_TYPES = new Set(['raw', 'relationship']);
const hashOf = (text) => crypto.createHash('sha256').update(text).digest('hex');

// A rulebook as the text of its file: standard JSON with single-line leaves (one field or one
// row per line), written by the platform's one layout, scripts/rulebook_layout.py, the same
// text the generated rulebook editor's Save writes. Both of this app's rulebook files go through it.
const LAYOUT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '../../../../scripts/rulebook_layout.py');
export function layOut(doc) {
  try {
    return execFileSync('python3', [LAYOUT, '-'], { input: JSON.stringify(doc), encoding: 'utf8', maxBuffer: 256 * 1024 * 1024 });
  } catch (e) {
    throw new Error(`scripts/rulebook_layout.py could not lay out the rulebook: ${(e.stderr || e.message).trim().split('\n').slice(-2).join(' ')}`);
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
