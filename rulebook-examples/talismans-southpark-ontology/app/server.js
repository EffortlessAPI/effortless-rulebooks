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
import { rulebookWithRows, readHub, storedRows } from './lib/hub.mjs';

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
// The rulebook editor's pinned ports (./start.sh --editor starts it on these).
const EDITOR_UI_URL = `http://localhost:${process.env.RULEBOOK_EDITOR_UI_PORT || 43612}`;
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
app.get('/api/page', route((req) => explainer.page(engineOf(req))));
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

// What the rows are against the rulebook file: what differs, whether the file changed under the app, and the file's version.
const fileState = () => ({ changes: ws.changes(), hubChangedOnDisk: ws.hubChangedOnDisk(), version: ws.rulebookVersion() });

app.get('/api/admin/tables', route(async () => ({ tables: Object.values(ws.tables).map(tableInfo), ...fileState() })));

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

app.get('/api/admin/changes', route(async () => fileState()));
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
const conceptView = async (engine) => ({ ...(await concepts.view(engine)), ...fileState(), file: conceptFileState() });
app.get('/api/concepts', route((req) => conceptView(engineOf(req))));
app.post('/api/concepts/edit', route((req) => write(async () => {
  const engine = engineOf(req);
  const result = await concepts.edit(engine, req.body);
  return { ...result, ...(await conceptView(engine)) };
})));
// The concept rulebook as a file of its own: what the tools that take a rulebook are handed.
app.get('/api/concepts/rulebook.json', async (req, res) => {
  try {
    const { concepts: document } = await concepts.view(engineOf(req));
    res.type('application/json').send(JSON.stringify(document, null, 1) + '\n');
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
// opened on it (./start.sh --concept-editor) shows what was saved. That editor's own Save
// rewrites the file; "Load the concept rulebook file" lays it over the live rows, as unsaved
// changes of the ontology. A file that changed on disk since this app wrote, loaded or first
// found it is someone's edit: Save leaves it alone until it is loaded or overwritten here.
const hashOfFile = (file) => (fs.existsSync(file) ? crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex') : null);
const conceptFileName = path.relative(PROJECT, CONCEPT_FILE);
let conceptFileSeen = hashOfFile(CONCEPT_FILE);
function conceptFileState() {
  const now = hashOfFile(CONCEPT_FILE);
  return { file: conceptFileName, exists: now !== null, changedOnDisk: now !== conceptFileSeen };
}
async function writeConceptFile(engine) {
  const { concepts: document } = await concepts.view(engine);
  fs.mkdirSync(path.dirname(CONCEPT_FILE), { recursive: true });
  fs.writeFileSync(CONCEPT_FILE, JSON.stringify(document, null, 2) + '\n', 'utf8');
  conceptFileSeen = hashOfFile(CONCEPT_FILE);
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
  return { ...result, message: fill(explainer.text('concepts-file-loaded'), { file: conceptFileName, rows: ws.changes().total }), ...(await conceptView(engine)) };
})));
app.post('/api/concepts/write-file', route((req) => write(async () => {
  const engine = engineOf(req);
  await writeConceptFile(engine);
  return { message: fill(explainer.text('concepts-file-written'), { file: conceptFileName }), ...(await conceptView(engine)) };
})));

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
async function editorAt(url) {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), 1500);
  try {
    const r = await fetch(url, { signal: controller.signal });
    return { running: r.ok, url };
  } catch (e) {
    return { running: false, url };
  } finally {
    clearTimeout(timer);
  }
}
app.get('/api/admin/editor', route(() => editorAt(EDITOR_UI_URL)));            // on the rulebook (the ontology form)
app.get('/api/concepts/editor', route(() => editorAt(CONCEPT_EDITOR_UI_URL)));   // on the concept rulebook file

app.use('/rulebook-explainer-dag', express.static(path.join(PROJECT, 'explainer/rulebook-explainer-dag'), { maxAge: 0 }));
app.use('/rulespeak', express.static(path.join(PROJECT, 'rulespeak')));
app.use('/effortless-xlsx', express.static(path.join(PROJECT, 'effortless-xlsx')));
app.use('/rulebook', express.static(path.join(PROJECT, 'effortless-rulebook')));
// The concept rulebook's folder: its file, its documentation and its explainer, as the build wrote them.
app.use('/concept-rulebook', express.static(path.join(PROJECT, 'concept-rulebook'), { maxAge: 0 }));
app.use('/', express.static(path.join(HERE, 'public')));

app.listen(PORT, '127.0.0.1', () => console.log(`South Park explainer on http://localhost:${PORT} (rulebook ${path.relative(REPO, HUB)}, database ${DATABASE_URL})`));
