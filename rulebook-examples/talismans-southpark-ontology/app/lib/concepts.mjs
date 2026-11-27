// The concept form of the live rows.
//
// The rulebook is in the ontology form: things, properties and statements as rows.
// The same ontology has a concept form: a table of individuals, a table per class, a
// table or a column per property, with ERBTables and ERBFields for what structure
// cannot say. The platform's orchestration/ontology_form.py is the one mapping
// between the two; this module only hands it the live rows and loads what it gives
// back. There is one store (the workspace): the concept form is a view of it, and an
// edit made there is written as the matching change of the ontology's rows.
//
// The mapping reads two things an engine computes: which facts are in effect
// (Statements.IsInEffect) and the next numbers a new fact takes (Graphs). They are
// read from the request's engine and handed over with the rows; nothing is
// recomputed here or there.
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { execFile } from 'node:child_process';
import { rulebookWithRows } from './hub.mjs';
import { syncCandidates } from './candidates.mjs';

export function createConcepts({ ws, repoRoot }) {
  const script = path.join(repoRoot, 'orchestration', 'ontology_form.py');

  // One run of the mapping. What it refuses it says on stderr in plain words (exit 1).
  function mapping(args) {
    return new Promise((resolve, reject) => {
      execFile('python3', [script, ...args], { maxBuffer: 256 * 1024 * 1024 }, (error, stdout, stderr) => {
        if (!error) return resolve(stdout);
        if (error.code === 1 && stderr.trim()) return reject(new Error(stderr.trim()));
        reject(new Error(`orchestration/ontology_form.py ${args[0]} failed: ${(stderr || error.message).trim().split('\n').slice(-3).join(' ')}`));
      });
    });
  }
  async function inTemp(fn) {
    const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-concepts-'));
    try { return await fn(dir); } finally { fs.rmSync(dir, { recursive: true, force: true }); }
  }
  const put = (dir, name, value) => { const file = path.join(dir, name); fs.writeFileSync(file, JSON.stringify(value)); return file; };

  // The live ontology, with the computed values the mapping reads.
  async function ontology(engine) {
    const doc = rulebookWithRows(ws.hub, ws.data);
    const inEffect = new Map((await engine.read('Statements', ['StatementId', 'IsInEffect'])).map((r) => [r.StatementId, r.IsInEffect === true]));
    for (const row of doc.Statements.data) {
      if (!inEffect.has(row.StatementId)) throw new Error(`The engine gave no IsInEffect for the fact ${row.StatementId}`);
      row.IsInEffect = inEffect.get(row.StatementId);
    }
    const graphs = await engine.read('Graphs', ['GraphId', 'NextActivationSeq', 'NextListedSortOrder', 'NextCandidateSortOrder']);
    for (const row of doc.Graphs.data) {
      const computed = graphs.find((g) => g.GraphId === row.GraphId);
      if (!computed) throw new Error(`The engine gave no next numbers for the graph ${row.GraphId}`);
      for (const f of ['NextActivationSeq', 'NextListedSortOrder']) row[f] = Number(computed[f]);
    }
    return { doc, nextCandidate: Number(graphs[0].NextCandidateSortOrder) };
  }

  // The ontology the mapping gave back becomes the live rows, then the candidate
  // facts its things and properties imply (the generator's job, as after any edit).
  function load(back, nextCandidate) {
    const rows = {};
    for (const [name, table] of Object.entries(ws.tables)) {
      if (!back[name] || !Array.isArray(back[name].data)) throw new Error(`The ontology that came back has no table ${name}`);
      rows[name] = back[name].data.map((row) => Object.fromEntries(table.stored.map((f) => [f, row[f] ?? null])));
    }
    ws.replaceAll(rows);
    const candidates = syncCandidates(ws, nextCandidate);
    return { candidates: { added: candidates.added, removed: candidates.removed } };
  }

  // { concepts, description }: the concept rulebook of the live rows, and what each of its tables is.
  const view = (engine) => inTemp(async (dir) => JSON.parse(await mapping(['view', put(dir, 'ontology.json', (await ontology(engine)).doc)])));

  return {
    view,

    // Edits made in the concept form ([{ op, ... }], the mapping's own vocabulary), applied to the live rows.
    edit: (engine, edits) => inTemp(async (dir) => {
      if (!Array.isArray(edits) || !edits.length) throw new Error('Send the edits as a list of { op, ... }');
      const { doc, nextCandidate } = await ontology(engine);
      const out = await mapping(['edit', put(dir, 'ontology.json', doc), put(dir, 'edits.json', edits)]);
      return load(JSON.parse(out).ontology, nextCandidate);
    }),

    // A whole concept rulebook from elsewhere (a workbook, the rulebook editor's file) laid over the live rows.
    lay: (engine, concepts) => inTemp(async (dir) => {
      const { doc, nextCandidate } = await ontology(engine);
      const back = path.join(dir, 'back.json');
      await mapping(['inject', put(dir, 'concepts.json', concepts), back, '--base', put(dir, 'ontology.json', doc)]);
      return load(JSON.parse(fs.readFileSync(back, 'utf8')), nextCandidate);
    }),
  };
}
