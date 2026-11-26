#!/usr/bin/env node
// The candidate rows an ontology-form rulebook should hold, from the ONE generator
// (rulebook-examples/talismans-southpark-ontology/app/lib/candidates.mjs), printed as
// JSON id lists: {"Statements": [...], "Firings": [...], "DisjointChecks": [...]}.
// Used by scripts/tests/test_ontology_form.py; reads the rulebook path given.
import { readFileSync } from 'node:fs';
import { expectedCandidates } from '../rulebook-examples/talismans-southpark-ontology/app/lib/candidates.mjs';

const path = process.argv[2];
if (!path) throw new Error('usage: ontology-form-candidates.mjs <ontology-rulebook.json>');
const rulebook = JSON.parse(readFileSync(path, 'utf8'));
const ws = {
  rows(name) {
    const table = rulebook[name];
    if (!table || !Array.isArray(table.data)) throw new Error(`The rulebook has no ${name} table`);
    return table.data;
  },
};
const expected = expectedCandidates(ws);
const out = {};
for (const [name, rows] of Object.entries(expected)) out[name] = [...rows.keys()];
process.stdout.write(JSON.stringify(out));
