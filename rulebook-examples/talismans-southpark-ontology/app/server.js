// The South Park explainer's server.
//
// One workspace holds the rulebook's stored rows as they are right now. Every
// click and every admin edit writes there. Whichever engine a request names
// (X-ERB-Backend: postgres | typescript | owl) computes everything else from
// those rows. The server holds no rule of its own.
import express from 'express';
import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import { Workspace } from './lib/workspace.mjs';
import { createEngines } from './lib/engines.mjs';
import { createExplainer } from './lib/explainer.mjs';
import { syncCandidates } from './lib/candidates.mjs';
import { createConcepts } from './lib/concepts.mjs';
import { rulebookWithRows, readHub, storedRows, layOut } from './lib/hub.mjs';

const HERE = path.dirname(fileURLToPath(import.meta.url));
const PROJECT = path.resolve(HERE, '..');
const REPO = path.resolve(PROJECT, '../..');
const SLUG = path.basename(PROJECT);
// The rulebook file. RULEBOOK_PATH points the app at another copy of the SAME rulebook
// (the tests save into a temporary copy, never into the project's file).
const HUB = process.env.RULEBOOK_PATH || path.join(PROJECT, 'effortless-rulebook', `${SLUG}-rulebook.json`);
const RUN_DIR = path.join(PROJECT, '.run');
const PORT = Number(process.env.PORT || 43210);
// The database scripts/erb_project.py loads for this project; DATABASE_URL only overrides it.
const DATABASE_URL = process.env.DATABASE_URL || `postgresql://postgres@localhost:5432/erb_${SLUG.replace(/-/g, '_')}`;
// How long a rulebook editor's launcher is given before it is stopped and called failed.
const EDITOR_START_LIMIT_MS = Number(process.env.EDITOR_START_LIMIT_MS || 15 * 60 * 1000);
// The rulebook editor's pinned ports (./start.sh --editor starts it on these).
const EDITOR_UI_URL = `http://localhost:${process.env.RULEBOOK_EDITOR_UI_PORT || 43612}`;
// What starts a rulebook editor: this project's start.sh, with the form's flag.
const START_SCRIPT = process.env.START_SCRIPT || path.join(PROJECT, 'start.sh');
// The concept rulebook as a file: what its own rulebook editor (./start.sh --concept-editor)
// opens and saves. Beside the hub when RULEBOOK_PATH points the app at a copy.
const CONCEPT_FILE = process.env.CONCEPT_RULEBOOK_PATH
  || (process.env.RULEBOOK_PATH ? path.join(path.dirname(HUB), `${SLUG}-concepts.json`) : path.join(PROJECT, 'concept-rulebook', `${SLUG}-concepts.json`));
const CONCEPT_EDITOR_UI_URL = `http://localhost:${process.env.CONCEPT_EDITOR_UI_PORT || 43622}`;

const ws = new Workspace(HUB);
const engines = createEngines({ ws, projectDir: PROJECT, repoRoot: REPO, databaseUrl: DATABASE_URL });
const explainer = createExplainer({ ws });
const concepts = createConcepts({ ws, repoRoot: REPO });

// Tables whose rows decide which candidate facts exist.
const CANDIDATE_SOURCES = new Set(['Things', 'Properties', 'PropertyAxioms', 'FiringKinds', 'Statements']);

const engineOf = (req) => engines.get(req.get('X-ERB-Backend') || req.query.backend || engines.defaultId());
const fill = (template, values) => template.replace(/\{(\w+)\}/g, (_, k) => values[k]);

// Writes run one at a time, and a write that fails is undone.
let writing = Promise.resolve();
function write(fn) {
  const run = writing.catch(() => {}).then(async () => {
    const snapshot = ws.snapshot();
    try { return await fn(); } catch (e) { ws.restore(snapshot); throw e; }
  });
  writing = run;
  return run;
}

const app = express();
app.use(express.json({ limit: '5mb' }));
// Before the first request that reads or changes rows is answered, the concept rulebook file gets its first look (see firstLook).
app.use('/api', (req, res, next) => {
  if (conceptFileSeen !== undefined || req.path === '/health') return next();
  firstLook().then(() => next(), (e) => { console.error(e.message); looked = null; next(); });
});
const route = (fn) => async (req, res) => {
  try { res.json(await fn(req)); } catch (e) { console.error(e.message); res.status(400).json({ error: e.message }); }
};
// A page action: the write, then the page as the request's engine now computes it.
const action = (fn) => route((req) => write(async () => {
  const engine = engineOf(req);
  const result = (await fn(req, engine)) || {};
  return { ...result, page: await explainer.page(engine) };
}));

// ------------------------------------------------------------------ the explainer
app.get('/api/health', route(async () => ({ ok: true, project: SLUG, engines: await engines.list() })));
app.get('/api/page', route(async (req) => { await takeFiles(engineOf(req)); return explainer.page(engineOf(req)); }));
app.get('/api/rows', route(async () => ws.data));        // stored rows, for the explainer's witness
app.get('/api/engines', route(async () => ({ engines: await engines.list(), defaultId: engines.defaultId() })));
app.post('/api/facts/:id/switch', action((req, engine) => explainer.switchFact(engine, req.params.id, !!req.body.on)));
app.post('/api/facts/:id/remove', action((req, engine) => explainer.removeFact(engine, req.params.id)));
app.post('/api/facts', action((req, engine) => explainer.addFact(engine, req.body)));
app.post('/api/rules/:id/switch', action((req) => explainer.switchRule(req.params.id, !!req.body.on)));
app.post('/api/scenarios/:id/run', action((req, engine) => explainer.runScenario(engine, req.params.id)));
app.post('/api/conformance', route(() => engines.compare()));
app.get('/api/tools', route(async (req) => ({ tools: explainer.tools(String(req.query.form ?? '')) })));   // one form's tools panel

// ------------------------------------------------------------------ admin: every table, every row
const tableInfo = (t) => ({ name: t.name, description: t.description, key: t.key, rowCount: ws.rows(t.name).length,
  fields: t.fields.map((f) => ({ name: f.name, type: f.type, datatype: f.datatype, stored: f.stored, relatedTo: f.relatedTo, formula: f.formula, description: f.description })) });

// What the rows are against the rulebook's two files: what differs from the rulebook file, whether
// either file was saved from outside and is waiting, the file's version, and the last thing a
// file saved from outside did here (news), which an open page shows when it sees a new one.
const fileState = () => ({ changes: ws.changes(), hubChangedOnDisk: ws.hubChangedOnDisk(), version: ws.rulebookVersion(), file: conceptFileState(), news });

app.get('/api/admin/tables', route(async (req) => { await takeFiles(engineOf(req)); return { tables: Object.values(ws.tables).map(tableInfo), ...fileState() }; }));

app.get('/api/admin/tables/:table', route(async (req) => {
  const t = ws.table(req.params.table);
  const stored = ws.rows(t.name);
  if (req.query.computed !== '1' || !t.computed.length) return { table: tableInfo(t), rows: stored, computed: false };
  const computed = new Map((await engineOf(req).read(t.name, [t.key, ...t.computed])).map((row) => [String(row[t.key]), row]));
  return { table: tableInfo(t), computed: true, rows: stored.map((row) => ({ ...row, ...(computed.get(String(row[t.key])) || {}) })) };
}));

// An admin edit, followed by the candidate facts it implies when it touches the cast or the wiring.
const adminWrite = (fn) => route((req) => write(async () => {
  const table = ws.table(req.params.table).name;
  const next = CANDIDATE_SOURCES.has(table) ? await explainer.nextNumbers(engineOf(req)) : null;
  const result = (await fn(req, table)) || {};
  const candidates = next ? syncCandidates(ws, next.candidate) : null;
  return { ...result, candidates: candidates && { added: candidates.added, removed: candidates.removed }, changes: ws.changes() };
}));
// Many rows of one table in one write: [{ key, values }] patches, [{ values }] inserts.
app.post('/api/admin/tables/:table/bulk', adminWrite((req, table) => {
  if (!Array.isArray(req.body)) throw new Error('Send an array of { key, values }');
  for (const item of req.body) item.key === undefined ? ws.insert(table, item.values) : ws.patch(table, item.key, item.values);
  return { rows: req.body.length };
}));
app.patch('/api/admin/tables/:table/rows/:key', adminWrite((req, table) => ({ row: ws.patch(table, req.params.key, req.body) })));
app.post('/api/admin/tables/:table/rows', adminWrite((req, table) => ({ row: ws.insert(table, req.body) })));
app.delete('/api/admin/tables/:table/rows/:key', adminWrite((req, table) => { ws.remove(table, req.params.key); }));

// How things stand. The pages ask every few seconds, which is also when a file saved from outside is taken in.
app.get('/api/admin/changes', route(async (req) => { await takeFiles(engineOf(req)); return fileState(); }));
app.post('/api/admin/save', route((req) => write(async () => {
  const rows = ws.save();
  const message = rows ? fill(explainer.text('admin-saved'), { rows, file: path.relative(PROJECT, HUB) }) : explainer.text('admin-nothing-to-save');
  // The saved rows have a concept form too: its file follows the save (see "the concept rulebook as a file").
  const concept = rows || !fs.existsSync(CONCEPT_FILE) ? await saveConceptFile(engineOf(req)) : null;
  return { rows, version: ws.rulebookVersion(), file: path.relative(PROJECT, HUB), message: [message, concept?.message].filter(Boolean).join(' '), concept };
})));
app.post('/api/admin/discard', route(() => write(async () => {
  ws.reload();
  engines.forgetSchema();
  return { message: explainer.text('admin-discarded'), changes: ws.changes() };
})));

// ------------------------------------------------------------------ jobs: the long admin actions, in the background
// Rebuild, Excel export and Excel import take seconds to minutes. Each is a job: its POST
// starts it and answers at once, GET /api/admin/jobs/<kind> says how it is going, and the
// page polls that, so the page (and every other request) stays usable while it runs. One
// job of each kind at a time; the last one's outcome stays readable until the next starts.
const JOB_KINDS = new Set(['rebuild', 'export', 'import', 'concept-export', 'concept-import']);
const jobs = {};
function startJob(kind, run) {
  if (jobs[kind]?.running) throw new Error(`An ${kind} is already running`);
  const job = jobs[kind] = { kind, running: true, startedAt: new Date().toISOString(), finishedAt: null, ok: null, error: null, result: null };
  const finish = (fields) => Object.assign(job, fields, { running: false, finishedAt: new Date().toISOString() });
  Promise.resolve().then(() => run(job)).then(
    (result) => finish({ ok: true, result: result ?? null }),
    (e) => { console.error(`${kind}: ${e.message}`); finish({ ok: false, error: e.message }); });
  return jobView(job);
}
function jobView(job) {
  const { workbook, ...shown } = job;   // server-side paths stay here
  const until = job.finishedAt ? Date.parse(job.finishedAt) : Date.now();
  return { ...shown, seconds: Math.round((until - Date.parse(job.startedAt)) / 1000) };
}
app.get('/api/admin/jobs/:kind', route(async (req) => {
  const kind = req.params.kind;
  if (!JOB_KINDS.has(kind)) throw new Error(`There is no job ${kind}; there are ${[...JOB_KINDS].join(', ')}`);
  const view = jobs[kind] ? jobView(jobs[kind]) : { kind, running: false, startedAt: null, finishedAt: null, ok: null, error: null, result: null, seconds: 0 };
  if (kind === 'rebuild') view.tail = fs.existsSync(BUILD_LOG) ? fs.readFileSync(BUILD_LOG, 'utf8').split('\n').filter((l) => /\*\*\*\*|rror|ailed|\$ |reset\]/.test(l)).slice(-25) : [];
  return view;
}));

function spawnLogged(cmd, args, cwd, logStream) {
  return new Promise((resolve) => {
    logStream.write(`\n$ ${cmd} ${args.join(' ')}   (in ${cwd})\n`);
    const child = spawn(cmd, args, { cwd, stdio: ['ignore', 'pipe', 'pipe'] });
    child.stdout.pipe(logStream, { end: false });
    child.stderr.pipe(logStream, { end: false });
    child.on('error', (e) => { logStream.write(`${e.message}\n`); resolve(1); });
    child.on('close', (code) => resolve(code ?? 1));
  });
}
function effortless(args, cwd, timeoutMs = 420000) {
  return new Promise((resolve, reject) => {
    const child = spawn('effortless', args, { cwd, stdio: ['ignore', 'pipe', 'pipe'] });
    let out = '';
    const timer = setTimeout(() => { child.kill('SIGKILL'); reject(new Error(`effortless ${args[0]} did not finish within ${timeoutMs / 1000}s`)); }, timeoutMs);
    child.stdout.on('data', (d) => { out += d; });
    child.stderr.on('data', (d) => { out += d; });
    child.on('error', (e) => { clearTimeout(timer); reject(e); });
    child.on('close', (code) => { clearTimeout(timer); code === 0 ? resolve(out) : reject(new Error(`effortless ${args.join(' ')} exited ${code}:\n${out.split('\n').slice(-8).join('\n')}`)); });
  });
}
const freshDir = (name) => { const dir = path.join(RUN_DIR, name); fs.rmSync(dir, { recursive: true, force: true }); fs.mkdirSync(dir, { recursive: true }); return dir; };
// The live rows as a rulebook, taken when the job is asked for (not when it gets round to it).
const liveRulebookText = () => JSON.stringify(rulebookWithRows(ws.hub, ws.data), null, 1);

// ------------------------------------------------------------------ rebuild: effortless build, then the database
const BUILD_LOG = path.join(RUN_DIR, 'build.log');
app.post('/api/admin/jobs/rebuild', route(async () => {
  if (ws.changes().total) throw new Error('There are unsaved changes: save them to the rulebook (or discard them) before rebuilding from it');
  fs.mkdirSync(RUN_DIR, { recursive: true });
  return startJob('rebuild', async () => {
    const log = fs.createWriteStream(BUILD_LOG);
    // The build's last step (init-db) fails when the CLI drops the reset script's
    // executable bit (cr-20-02); the database is loaded by the shared runner instead.
    await spawnLogged('effortless', ['build'], PROJECT, log);
    const code = await spawnLogged('python3', [path.join(REPO, 'scripts/erb_project.py'), SLUG, '--reset-db'], REPO, log);
    log.end();
    if (code !== 0) throw new Error(`Loading the database exited ${code}; see ${path.relative(PROJECT, BUILD_LOG)}`);
    await write(async () => { ws.reload(); engines.forgetSchema(); });
  });
}));

// ------------------------------------------------------------------ Excel, both ways (the catalog's own tools)
// Export: the live rows through rulebook-to-xlsx; when the job is done the page downloads the workbook.
app.post('/api/admin/jobs/export', route(async () => {
  const rulebook = liveRulebookText();
  return startJob('export', async (job) => {
    const dir = freshDir('xlsx-export');
    fs.writeFileSync(path.join(dir, 'working-rulebook.json'), rulebook);
    await effortless(['rulebook-to-xlsx', '-i', 'working-rulebook.json'], dir);
    const made = fs.readdirSync(dir).filter((f) => f.endsWith('.xlsx'));
    if (made.length !== 1) throw new Error(`rulebook-to-xlsx left ${made.length} workbooks in ${dir}, expected one`);
    job.workbook = path.join(dir, made[0]);
    return { file: `${SLUG}.xlsx`, bytes: fs.statSync(job.workbook).size };
  });
}));
// The finished workbook of an export job (the ontology form's, or the concept form's).
app.get('/api/admin/jobs/:kind/download', (req, res) => {
  const kind = req.params.kind, job = jobs[kind];
  if (!job || job.running || !job.ok || !job.workbook) return res.status(400).json({ error: job?.running ? 'The export is still running' : `There is no finished export: start one with POST /api/admin/jobs/${kind}` });
  res.download(job.workbook, job.result.file);
});

// Import: the catalog's xlsx-to-rulebook syncs a workbook INTO a rulebook, so it is handed
// the workbook and a copy of the live rulebook; what comes back is loaded as unsaved changes.
// The tool runs outside the write queue, so edits are not held up meanwhile; if the rows
// changed while it ran, loading its answer would undo that change, so the import refuses.
const IMPORT_TIMEOUT_MS = Number(process.env.XLSX_IMPORT_TIMEOUT_MS || 240000);   // the tool answers this rulebook's workbook in about 2 s; a cold start adds up to a minute
// The body is the workbook whatever its content-type, or none: a browser sends an ArrayBuffer
// with NO content-type, and express.raw's '*/*' matches only a request that has one.
app.post('/api/admin/jobs/import', express.raw({ type: () => true, limit: '30mb' }), route(async (req) => {
  if (!Buffer.isBuffer(req.body) || req.body.length === 0) throw new Error('Send the workbook as the request body');
  const workbook = req.body, rulebook = liveRulebookText(), version = ws.version;
  return startJob('import', async () => {
    const dir = freshDir('xlsx-import');
    const target = path.join(dir, 'working-rulebook.json');
    fs.writeFileSync(path.join(dir, 'upload.xlsx'), workbook);
    fs.writeFileSync(target, rulebook);
    await effortless(['xlsx-to-rulebook', '-i', 'upload.xlsx,working-rulebook.json'], dir, IMPORT_TIMEOUT_MS);
    // The tool replaces the rulebook it was handed with the synced one and says what it did
    // in xlsx-sync-summary.json (the same contract the generated rulebook editor's import uses).
    const summaryFile = path.join(dir, 'xlsx-sync-summary.json');
    if (!fs.existsSync(summaryFile)) throw new Error('xlsx-to-rulebook returned no xlsx-sync-summary.json: the installed tool predates workbook-into-rulebook sync');
    const summary = JSON.parse(fs.readFileSync(summaryFile, 'utf8'));
    const imported = readHub(target);
    // Tables, fields and formulas changed in the workbook are reported, not applied: this import loads stored rows.
    const schemaDiffers = [];
    for (const [name, table] of Object.entries(ws.tables)) {
      const other = imported.tables[name];
      if (!other) throw new Error(`The synced rulebook has no table ${name}`);
      for (const f of table.fields) {
        const o = other.byName[f.name];
        if (!o) schemaDiffers.push(`${name}.${f.name} is missing`);
        else if ((o.formula || '') !== (f.formula || '')) schemaDiffers.push(`${name}.${f.name}: formula changed`);
      }
      for (const o of other.fields) if (!table.byName[o.name]) schemaDiffers.push(`${name}.${o.name} is new`);
    }
    return write(async () => {
      if (ws.version !== version) throw new Error(explainer.text('admin-import-stale'));
      ws.replaceAll(storedRows(imported));
      const changes = ws.changes();
      return { message: fill(explainer.text('admin-imported'), { rows: changes.total }), changes, schemaDiffers, summary };
    });
  });
}));

// ------------------------------------------------------------------ the concept form: the same ontology as ordinary tables
// The rulebook is in the ontology form (things, properties, statements). lib/concepts.mjs shows
// the same live rows as their concept rulebook (a table of individuals, a table per class, a
// table or a column per property, ERBTables and ERBFields) and writes an edit made there as the
// matching change of the ontology's rows. One store, two forms of it.
const conceptView = async (engine) => ({ ...(await concepts.view(engine)), ...fileState() });
app.get('/api/concepts', route(async (req) => { await takeFiles(engineOf(req)); return conceptView(engineOf(req)); }));
app.post('/api/concepts/edit', route((req) => write(async () => {
  const engine = engineOf(req);
  const result = await concepts.edit(engine, req.body);
  return { ...result, ...(await conceptView(engine)) };
})));
// The concept rulebook as a file of its own: what the tools that take a rulebook are handed.
app.get('/api/concepts/rulebook.json', async (req, res) => {
  try {
    const { concepts: document } = await concepts.view(engineOf(req));
    res.type('application/json').send(layOut(document));
  } catch (e) {
    console.error(e.message);
    res.status(400).json({ error: e.message });
  }
});
// A whole concept rulebook laid over the live rows (the body is the rulebook).
app.post('/api/concepts/rulebook', route((req) => write(async () => {
  const engine = engineOf(req);
  const result = await concepts.lay(engine, req.body);
  return { ...result, ...(await conceptView(engine)) };
})));

// The concept rulebook as a file. Save writes it from the saved rows, so the rulebook editor
// opened on it shows what was saved. That editor's own Save rewrites the file, and the file is
// then taken in (see "a file saved from outside this app"); "Load the concept rulebook file"
// does the same by hand, as unsaved changes of the ontology. A file that changed on disk since
// this app wrote, loaded or agreed with it is someone's edit: Save leaves it alone until it is
// loaded or overwritten here.
const hashOfFile = (file) => (fs.existsSync(file) ? crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex') : null);
const conceptFileName = path.relative(PROJECT, CONCEPT_FILE);
// The hash of the file as this app last wrote, loaded or agreed with it. Not known until the
// first look (see firstLook): a file that says something other than the rulebook when the app
// starts was saved while the app was not looking, and is not passed over as already seen.
let conceptFileSeen;
// The file as it is now is the reader's to load or overwrite, and why: { hash, why }.
let conceptFileHeld = null;
function conceptFileState() {
  const now = hashOfFile(CONCEPT_FILE);
  return { file: conceptFileName, exists: now !== null, changedOnDisk: now !== null && now !== conceptFileSeen,
    why: conceptFileHeld && conceptFileHeld.hash === now ? conceptFileHeld.why : null };
}
async function writeConceptFile(engine) {
  const { concepts: document } = await concepts.view(engine);
  fs.mkdirSync(path.dirname(CONCEPT_FILE), { recursive: true });
  fs.writeFileSync(CONCEPT_FILE, layOut(document), 'utf8');
  conceptFileSeen = hashOfFile(CONCEPT_FILE);
  conceptFileHeld = null;
}
// What Save does with the file, and says about it. The rows are saved either way.
async function saveConceptFile(engine) {
  if (conceptFileState().changedOnDisk) return { written: false, message: fill(explainer.text('concepts-file-kept'), { file: conceptFileName }) };
  try {
    await writeConceptFile(engine);
    return { written: true, message: fill(explainer.text('concepts-file-written'), { file: conceptFileName }) };
  } catch (e) {
    console.error(`concept rulebook file: ${e.message}`);
    return { written: false, message: fill(explainer.text('concepts-file-failed'), { reason: e.message }) };
  }
}
app.post('/api/concepts/load-file', route((req) => write(async () => {
  if (!fs.existsSync(CONCEPT_FILE)) throw new Error(fill(explainer.text('concepts-file-missing'), { file: conceptFileName }));
  const engine = engineOf(req);
  const seen = hashOfFile(CONCEPT_FILE);
  const result = await concepts.lay(engine, JSON.parse(fs.readFileSync(CONCEPT_FILE, 'utf8')));
  conceptFileSeen = seen;
  conceptFileHeld = null;
  return { ...result, message: fill(explainer.text('concepts-file-loaded'), { file: conceptFileName, rows: ws.changes().total }), ...(await conceptView(engine)) };
})));
app.post('/api/concepts/write-file', route((req) => write(async () => {
  const engine = engineOf(req);
  await writeConceptFile(engine);
  return { message: fill(explainer.text('concepts-file-written'), { file: conceptFileName }), ...(await conceptView(engine)) };
})));

// ------------------------------------------------------------------ a file saved from outside this app
// Each form of the rulebook is a file, and its rulebook editor saves into it. While nothing is
// unsaved here, a file saved from outside (its editor, or by hand) is taken in the next time a
// page asks how things stand:
//   the rulebook file      is reloaded, the candidate facts its things and properties imply are
//                          brought in line, and an edit that left the version number alone is
//                          stamped as the next version; the concept rulebook file follows it.
//   the concept rulebook   is laid over the rows and saved, like any change made in the concept
//                          form: the rulebook's next version, written to both files. Only a file
//                          saved from the version the rulebook is at: laid over a later one it
//                          would undo what was saved since.
// Everything else waits for the reader, and the pages say so once (news): unsaved changes here,
// both files changed at once, a concept file saved from an earlier version, one the mapping
// refuses, and one that already said something else when this app started (there is no telling
// which of the two files is the newer).
let news = null;
const tell = (form, message, bad = false) => { news = { id: (news?.id || 0) + 1, at: Date.now(), form, message, bad }; };
let told = null;                      // what is waiting, as last said: said once, until it changes
let hubRefused = null;                // the hash of a rulebook file that could not be taken in: not tried again
const hubName = path.relative(PROJECT, HUB);
const conceptFileText = () => JSON.parse(fs.readFileSync(CONCEPT_FILE, 'utf8'));
const hold = (hash, why) => { conceptFileHeld = { hash, why }; return why; };
// A file caught while it is being written does not parse yet. It is looked at again at the
// next look, and refused only once it has lain like that for a few seconds.
const stillBeingWritten = (file) => {
  try { JSON.parse(fs.readFileSync(file, 'utf8')); return false; } catch (e) { return Date.now() - fs.statSync(file).mtimeMs < 5000; }
};
const changedText = (why) => [fill(explainer.text('concepts-file-changed'), { file: conceptFileName }), why].join(' ');

// The first look at the concept file, before the first request is answered. A file that says
// what the rulebook says is the one this app would have written, so it counts as seen. Laying
// it over the rows shows whether it does: nothing changes. It takes an engine that can compute
// (whichever the request named); with none, nothing is decided and the next request looks again.
let looked = null;
function firstLook() {
  looked ??= write(async () => {
    const usable = (await engines.list()).find((e) => e.ok);
    if (!usable) { looked = null; return; }
    const engine = engines.get(usable.id);
    const hash = hashOfFile(CONCEPT_FILE);
    conceptFileSeen = null;
    if (hash === null) return;
    const snapshot = ws.snapshot();
    try {
      await concepts.lay(engine, conceptFileText());
      if (ws.changes().total === 0) conceptFileSeen = hash; else hold(hash, explainer.text('file-held-start'));
    } catch (e) {
      console.error(`concept rulebook file: ${e.message}`);
      hold(hash, fill(explainer.text('file-held-refused'), { reason: e.message }));
    } finally {
      ws.restore(snapshot);
    }
  });
  return looked;
}

async function takeFiles(engine) {
  await writing.catch(() => {});
  const waiting = () => {
    const hubHash = ws.hubHashOnDisk(), hash = hashOfFile(CONCEPT_FILE);
    return { hubHash, hash, hub: hubHash !== ws.hub.hash, concept: hash !== null && hash !== conceptFileSeen };
  };
  // What waits for the reader is said once.
  const wait = (w, form, message) => {
    const key = [w.hub && w.hubHash, w.concept && w.hash, message].join('|');
    if (told !== key) { told = key; tell(form, message, true); }
  };
  const first = waiting();
  if (!first.hub && !first.concept) return;
  if (conceptFileSeen === undefined) return;                                   // no engine could take the first look yet
  if (first.hub ? first.hubHash === hubRefused : conceptFileHeld?.hash === first.hash) return;   // already the reader's, and said
  let taking = null;
  try {
    await write(async () => {
      const w = waiting();
      if (!w.hub && !w.concept) return;
      if (w.hub && w.hubHash === hubRefused) return;
      if ((w.hub && stillBeingWritten(HUB)) || (w.concept && stillBeingWritten(CONCEPT_FILE))) return;
      if (w.hub && w.concept) return wait(w, 'ontology', explainer.text('files-both-changed'));
      if (w.hub && ws.changes().total) return wait(w, 'ontology', explainer.text('admin-hub-changed'));
      if (w.hub) {
        taking = { form: 'ontology', hubHash: w.hubHash };
        const { was, edited, undo } = ws.takeFile();
        try {
          engines.forgetSchema();
          syncCandidates(ws, (await explainer.nextNumbers(engine)).candidate);
          const stamp = edited && ws.rulebookVersion() === was;
          if (stamp || ws.changes().total) ws.save({ edited: stamp });
        } catch (e) {
          undo();
          engines.forgetSchema();
          throw e;
        }
        if (!edited) return;                                             // rewritten, saying the same
        const file = await saveConceptFile(engine);
        return tell('ontology', [fill(explainer.text('file-taken-hub'), { file: hubName, number: ws.rulebookVersion() }), file.message].join(' '));
      }
      // The concept rulebook file.
      if (conceptFileHeld?.hash === w.hash) return;                      // already the reader's, and said
      if (ws.changes().total) return wait(w, 'concept', changedText(explainer.text('file-waiting-unsaved')));
      taking = { form: 'concept', hash: w.hash };
      const document = conceptFileText();
      const base = (document.__meta__?.data || []).find((r) => r.MetaKey === 'rulebook-version')?.StringValue;
      if (base == null) throw new Error('its __meta__ has no rulebook-version row, so it does not say which version of the rulebook it was saved from');
      if (String(base) !== String(ws.rulebookVersion())) {
        return wait(w, 'concept', changedText(hold(w.hash, fill(explainer.text('file-held-base'), { base, number: ws.rulebookVersion() }))));
      }
      await concepts.lay(engine, document);
      const rows = ws.changes().total;
      if (!rows) { conceptFileSeen = w.hash; return; }                   // rewritten, saying the same
      ws.save();
      await writeConceptFile(engine);
      tell('concept', fill(explainer.text('file-taken-concept'), { file: conceptFileName, rows, number: ws.rulebookVersion() }));
    });
  } catch (e) {
    // Nothing was taken (the write was undone). The file stays as it is and waits for the reader.
    if (!taking) throw e;
    console.error(`${taking.form} file saved from outside: ${e.message}`);
    if (taking.form === 'concept') tell('concept', changedText(hold(taking.hash, fill(explainer.text('file-held-refused'), { reason: e.message }))), true);
    else { hubRefused = taking.hubHash; tell('ontology', fill(explainer.text('file-refused-hub'), { file: hubName, reason: e.message }), true); }
  }
}

// Excel, both ways, on the concept rulebook: the same catalog tools, handed the concept form.
app.post('/api/admin/jobs/concept-export', route(async (req) => {
  const { concepts: document } = await concepts.view(engineOf(req));
  return startJob('concept-export', async (job) => {
    const dir = freshDir('concept-xlsx-export');
    fs.writeFileSync(path.join(dir, 'working-concepts.json'), JSON.stringify(document, null, 1));
    await effortless(['rulebook-to-xlsx', '-i', 'working-concepts.json'], dir);
    const made = fs.readdirSync(dir).filter((f) => f.endsWith('.xlsx'));
    if (made.length !== 1) throw new Error(`rulebook-to-xlsx left ${made.length} workbooks in ${dir}, expected one`);
    job.workbook = path.join(dir, made[0]);
    return { file: `${SLUG}-concepts.xlsx`, bytes: fs.statSync(job.workbook).size };
  });
}));
app.post('/api/admin/jobs/concept-import', express.raw({ type: () => true, limit: '30mb' }), route(async (req) => {
  if (!Buffer.isBuffer(req.body) || req.body.length === 0) throw new Error('Send the workbook as the request body');
  const engine = engineOf(req);
  const workbook = req.body, { concepts: document } = await concepts.view(engine), version = ws.version;
  return startJob('concept-import', async () => {
    const dir = freshDir('concept-xlsx-import');
    const target = path.join(dir, 'working-concepts.json');
    fs.writeFileSync(path.join(dir, 'upload.xlsx'), workbook);
    fs.writeFileSync(target, JSON.stringify(document, null, 1));
    await effortless(['xlsx-to-rulebook', '-i', 'upload.xlsx,working-concepts.json'], dir, IMPORT_TIMEOUT_MS);
    if (!fs.existsSync(path.join(dir, 'xlsx-sync-summary.json'))) throw new Error('xlsx-to-rulebook returned no xlsx-sync-summary.json: the installed tool predates workbook-into-rulebook sync');
    const synced = JSON.parse(fs.readFileSync(target, 'utf8'));
    return write(async () => {
      if (ws.version !== version) throw new Error(explainer.text('admin-import-stale'));
      const result = await concepts.lay(engine, synced);
      const changes = ws.changes();
      return { ...result, message: fill(explainer.text('admin-imported'), { rows: changes.total }), changes };
    });
  });
}));

// ------------------------------------------------------------------ the rulebook editors (each its own container)
// Each form of the rulebook has the generated rulebook editor, in a container of its own, on
// ports start.sh pins. A page links to it: straight there when it answers, otherwise to
// /editor.html, which asks for it to be started here and goes there when it does. Starting one
// is this project's own `./start.sh --editor` (or --concept-editor): the platform's launcher,
// which brings the editor up to date, starts the container and then follows its log for ever.
// The launcher is let go as soon as the editor answers; the container stays.
const EDITORS = {
  ontology: { form: 'ontology', url: EDITOR_UI_URL, flag: '--editor' },
  concept: { form: 'concept', url: CONCEPT_EDITOR_UI_URL, flag: '--concept-editor' },
};
const editorOf = (req) => {
  const editor = EDITORS[String(req.query.form ?? '')];
  if (!editor) throw new Error(`There is no rulebook editor for the form ${JSON.stringify(req.query.form ?? '')}: the forms are ${Object.keys(EDITORS).join(' and ')}`);
  return editor;
};
async function answers(url) {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), 1500);
  try {
    return (await fetch(url, { signal: controller.signal })).ok;
  } catch (e) {
    return false;
  } finally {
    clearTimeout(timer);
  }
}
// running | starting (its launcher is at work: how long, and its last lines) | failed (the launcher gave up: why) | stopped
async function editorState(editor) {
  const { form, url } = editor;
  if (await answers(url)) return { form, url, state: 'running' };
  if (editor.launcher) return { form, url, state: 'starting', seconds: Math.round((Date.now() - editor.startedAt) / 1000), tail: editor.tail.slice(-4) };
  if (editor.failed) return { form, url, state: 'failed', reason: editor.failed };
  return { form, url, state: 'stopped' };
}
// One launcher at a time: each runs the effortless CLI in this project before it starts its container.
// A launcher has done its work when its editor answers. It is left following the container's log
// (into .run/editor-<form>.log) and goes when this app does. One that neither answers nor ends
// is stopped after EDITOR_START_LIMIT_MS, so the other form's editor is not kept waiting behind it.
let launching = Promise.resolve();
function startEditor(editor) {
  if (editor.launcher) return;
  editor.failed = null;
  editor.tail = [];
  editor.startedAt = Date.now();
  editor.launcher = launching.then(() => new Promise((resolve) => {
    fs.mkdirSync(RUN_DIR, { recursive: true });
    const log = fs.createWriteStream(path.join(RUN_DIR, `editor-${editor.form}.log`));
    const child = spawn('bash', [START_SCRIPT, editor.flag], { cwd: PROJECT, stdio: ['ignore', 'pipe', 'pipe'] });
    editor.child = child;
    const keep = (data) => { log.write(data); editor.tail = [...editor.tail, ...String(data).split('\n').map((l) => l.trim()).filter(Boolean)].slice(-12); };
    child.stdout.on('data', keep);
    child.stderr.on('data', keep);
    let settled = false;
    const settle = (failed) => {
      if (settled) return;
      settled = true;
      clearInterval(watch);
      editor.launcher = null;
      editor.failed = failed;
      resolve();
    };
    const began = Date.now();
    const watch = setInterval(async () => {
      if (await answers(editor.url)) return settle(null);
      if (Date.now() - began < EDITOR_START_LIMIT_MS) return;
      settle([`The editor did not answer at ${editor.url} within ${Math.round(EDITOR_START_LIMIT_MS / 1000)} s, so its launcher was stopped.`, ...editor.tail.slice(-6)].join('\n'));
      child.kill();
    }, 1000);
    const ended = async (why) => {
      log.end();
      editor.child = null;
      settle((await answers(editor.url)) ? null : [why, ...editor.tail.slice(-6)].join('\n'));
    };
    child.on('error', (e) => ended(e.message));
    child.on('close', (code) => ended(`${path.relative(PROJECT, START_SCRIPT)} ${editor.flag} ended (${code ?? 'stopped'}) before the editor answered at ${editor.url}.`));
  }));
  launching = editor.launcher;
}
// The launchers go with the app, however it is stopped.
process.on('exit', () => { for (const editor of Object.values(EDITORS)) editor.child?.kill(); });
for (const signal of ['SIGINT', 'SIGTERM', 'SIGHUP']) process.on(signal, () => process.exit(0));
app.get('/api/editor', route((req) => editorState(editorOf(req))));
app.post('/api/editor/start', route(async (req) => {
  const editor = editorOf(req);
  if (!(await answers(editor.url))) startEditor(editor);
  return editorState(editor);
}));

app.use('/rulebook-explainer-dag', express.static(path.join(PROJECT, 'explainer/rulebook-explainer-dag'), { maxAge: 0 }));
app.use('/rulespeak', express.static(path.join(PROJECT, 'rulespeak')));
app.use('/effortless-xlsx', express.static(path.join(PROJECT, 'effortless-xlsx')));
app.use('/rulebook', express.static(path.join(PROJECT, 'effortless-rulebook')));
// The concept rulebook's folder: its file, its documentation and its explainer, as the build wrote them.
app.use('/concept-rulebook', express.static(path.join(PROJECT, 'concept-rulebook'), { maxAge: 0 }));
app.use('/', express.static(path.join(HERE, 'public')));

app.listen(PORT, '127.0.0.1', () => console.log(`South Park explainer on http://localhost:${PORT} (rulebook ${path.relative(REPO, HUB)}, database ${DATABASE_URL})`));
