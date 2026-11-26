// The execution engines. Each one computes every field of the rulebook from the
// same live rows (the workspace); the page and the admin read whichever one a
// request names. None of them is hand-written logic: Postgres reads the generated
// vw_* views; TypeScript is the generated SDK (effortless-typescript/), imported
// into this process and called with the live rows; OWL runs the generated rules
// through the platform's orchestration/live_substrates.py, the same way the
// conformance harness runs them.
import pg from 'pg';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { performance } from 'node:perf_hooks';
import { pathToFileURL } from 'node:url';
import { spawn } from 'node:child_process';

const norm = (name) => name.toLowerCase().replace(/_/g, '');

// One value in the shape the page expects, whatever the engine's own spelling of it.
// A blank is false for a yes/no field and 0 for a number: the project builds every
// substrate with erbBlankLogic=coerce, and the engines spell a blank differently.
const NUMERIC = new Set(['integer', 'number', 'decimal', 'float']);
function normalize(value, datatype) {
  const blank = value === null || value === undefined || value === '';
  if (datatype === 'boolean') return !blank && (value === true || value === 'true' || value === 't');
  if (NUMERIC.has(datatype)) return blank ? 0 : Number(value);
  return blank ? null : String(value);
}

function run(cmd, args, { cwd, timeoutMs = 240000 } = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn(cmd, args, { cwd, stdio: ['ignore', 'pipe', 'pipe'] });
    let out = '', err = '';
    const timer = setTimeout(() => { child.kill('SIGKILL'); reject(new Error(`${cmd} ${args[0]} did not finish within ${timeoutMs / 1000}s`)); }, timeoutMs);
    child.stdout.on('data', (d) => { out += d; });
    child.stderr.on('data', (d) => { err += d; });
    child.on('error', (e) => { clearTimeout(timer); reject(e); });
    child.on('close', (code) => {
      clearTimeout(timer);
      if (code !== 0) reject(new Error((err || out).trim().split('\n').slice(-6).join('\n') || `${cmd} exited ${code}`));
      else resolve(out);
    });
  });
}
const runPython = (args, options) => run('python3', args, options);

function select(table, rows, fields, where) {
  let out = rows;
  if (where) out = out.filter((row) => Object.entries(where).every(([f, v]) => normalize(row[f], table.byName[f].datatype) === v));
  const wanted = fields || table.fields.map((f) => f.name);
  for (const f of wanted) if (!table.byName[f]) throw new Error(`${table.name} has no field ${f}`);
  return out.map((row) => Object.fromEntries(wanted.map((f) => [f, normalize(row[f], table.byName[f].datatype)])));
}

// ---------------------------------------------------------------------------
// TypeScript: the generated SDK, imported into this process. A click is one
// evaluate() call over the live rows; nothing is regenerated, spawned or written.
// ---------------------------------------------------------------------------
class TypeScriptSdkEngine {
  constructor({ ws, projectDir }) {
    this.ws = ws;
    this.projectDir = projectDir;
    this.folder = path.join(projectDir, 'effortless-typescript');
    this.sdk = null;          // the imported dist/erb_sdk.js
    this.loadedFrom = null;   // the newest source mtime that build was made from
    this.loading = null;
    this.cache = null;        // { version, tables }
    this.lastMs = null;
  }

  sources() { return ['erb_sdk.ts', 'erb_runtime.ts'].map((f) => path.join(this.folder, f)); }
  newestSource() { return Math.max(...this.sources().map((f) => fs.statSync(f).mtimeMs)); }

  async status() {
    const missing = this.sources().filter((f) => !fs.existsSync(f));
    if (missing.length) return { ok: false, reason: `${missing.map((f) => path.relative(this.projectDir, f)).join(', ')} is missing; run effortless build in the project` };
    if (this.sdk && this.newestSource() > this.loadedFrom) return { ok: false, reason: 'effortless-typescript was rebuilt after this app loaded it; restart ./start.sh' };
    return { ok: true };
  }

  // The SDK's own package builds it (npm run build); that runs only when the
  // generated sources are newer than dist/, i.e. after an effortless build.
  async load() {
    if (this.sdk) {
      if (this.newestSource() > this.loadedFrom) throw new Error('effortless-typescript was rebuilt after this app loaded it; restart ./start.sh');
      return this.sdk;
    }
    this.loading ||= (async () => {
      const built = path.join(this.folder, 'dist', 'erb_sdk.js');
      const newest = this.newestSource();
      if (!fs.existsSync(built) || fs.statSync(built).mtimeMs < newest) {
        if (!fs.existsSync(path.join(this.folder, 'node_modules'))) await run('npm', ['install', '--no-audit', '--no-fund', '--silent'], { cwd: this.folder });
        await run('npm', ['run', 'build', '--silent'], { cwd: this.folder });
      }
      const sdk = await import(pathToFileURL(built).href);
      if (typeof sdk.evaluate !== 'function') {
        throw new Error(`${path.relative(this.projectDir, built)} has no evaluate(): it was generated by a rulebook-to-typescript older than 2026-11-26; run effortless build in the project`);
      }
      this.sdk = sdk;
      this.loadedFrom = newest;
      return sdk;
    })();
    try {
      return await this.loading;
    } finally {
      this.loading = null;
    }
  }

  async evaluate() {
    if (this.cache && this.cache.version === this.ws.version) return this.cache.tables;
    const sdk = await this.load();
    const version = this.ws.version;
    const started = performance.now();
    const tables = sdk.evaluate(this.ws.data);
    this.lastMs = Math.round(performance.now() - started);
    this.cache = { version, tables };
    return tables;
  }

  async read(tableName, fields, where) {
    const table = this.ws.table(tableName);
    const tables = await this.evaluate();
    return select(table, tables[tableName], fields, where);
  }

  async answers() { return this.evaluate(); }
}

// ---------------------------------------------------------------------------
// OWL: the generated rules run on the live rows by the platform's runner
// ---------------------------------------------------------------------------
class LiveSubstrateEngine {
  constructor({ ws, substrate, projectDir, repoRoot, requires }) {
    Object.assign(this, { ws, substrate, projectDir, repoRoot, requires });
    this.cache = null;       // { version, promise }
    this.lastMs = null;
  }

  async status() {
    const missing = this.requires.filter((f) => !fs.existsSync(path.join(this.projectDir, f)));
    return missing.length ? { ok: false, reason: `${missing.join(', ')} is missing; run effortless build in the project` } : { ok: true };
  }

  evaluate() {
    const version = this.ws.version;
    if (this.cache && this.cache.version === version) return this.cache.promise;
    const promise = (async () => {
      const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-live-'));
      try {
        const rowsFile = path.join(dir, 'rows.json'), outFile = path.join(dir, 'out.json');
        fs.writeFileSync(rowsFile, JSON.stringify(this.ws.data));
        await runPython([path.join(this.repoRoot, 'orchestration/live_substrates.py'), 'evaluate',
          '--project-dir', this.projectDir, '--substrate', this.substrate, '--rows', rowsFile, '--out', outFile]);
        const result = JSON.parse(fs.readFileSync(outFile, 'utf8'));
        this.lastMs = result.elapsedMs;
        return result.tables;
      } finally {
        fs.rmSync(dir, { recursive: true, force: true });
      }
    })();
    this.cache = { version, promise };
    promise.catch(() => { if (this.cache && this.cache.promise === promise) this.cache = null; });
    return promise;
  }

  async read(tableName, fields, where) {
    const table = this.ws.table(tableName);
    const tables = await this.evaluate();
    if (tables[tableName] == null) throw new Error(`The ${this.substrate} build does not compute ${tableName}`);
    return select(table, tables[tableName], fields, where);
  }

  async answers() { return this.evaluate(); }
}

// ---------------------------------------------------------------------------
// Postgres: the live rows mirrored into the base tables, values read from views
// ---------------------------------------------------------------------------
class PostgresEngine {
  constructor({ ws, databaseUrl }) {
    this.ws = ws;
    this.databaseUrl = databaseUrl;
    this.pool = new pg.Pool({ connectionString: databaseUrl, max: 4, connectionTimeoutMillis: 8000 });   // a database that does not answer is reported, never waited on
    this.pool.on('error', () => {});   // a dropped idle connection is reported by the next query, not by crashing
    this.synced = {};                  // table -> the workspace version its base table holds
    this.names = null;                 // table -> { base, view, baseCols: {field: col}, viewCols: {field: col} }
    this.queue = Promise.resolve();
    this.lastMs = null;
  }

  forget() { this.synced = {}; this.names = null; }

  async status() {
    try {
      const r = await this.pool.query("SELECT count(*)::int AS n FROM information_schema.views WHERE table_schema = 'public' AND table_name LIKE 'vw\\_%'");
      if (r.rows[0].n === 0) return { ok: false, reason: `the database at ${this.databaseUrl} has no vw_* views; load it with scripts/erb_project.py talismans-southpark-ontology --reset-db` };
      return { ok: true };
    } catch (e) {
      return { ok: false, reason: `cannot reach ${this.databaseUrl}: ${e.message}` };
    }
  }

  // Which base table, view and columns each rulebook table and field is, read
  // from the database itself (never rebuilt from a naming rule).
  async mapping() {
    if (this.names) return this.names;
    const cols = await this.pool.query("SELECT table_name, column_name FROM information_schema.columns WHERE table_schema = 'public'");
    const byRelation = {};
    for (const { table_name, column_name } of cols.rows) (byRelation[table_name] ||= []).push(column_name);
    const relations = Object.keys(byRelation);
    const names = {};
    for (const [name, table] of Object.entries(this.ws.tables)) {
      const base = relations.find((r) => !r.startsWith('vw_') && norm(r) === norm(name));
      const view = relations.find((r) => r.startsWith('vw_') && norm(r.slice(3)) === norm(name));
      if (!base || !view) { names[name] = null; continue; }   // a table the database build leaves out (__meta__)
      const pick = (relation, fields) => Object.fromEntries(fields.map((f) => {
        const col = byRelation[relation].find((c) => norm(c) === norm(f));
        if (!col) throw new Error(`${relation} has no column for ${name}.${f}: the database is from another build; reload it`);
        return [f, col];
      }));
      names[name] = { base, view, baseCols: pick(base, table.stored), viewCols: pick(view, table.fields.map((f) => f.name)) };
    }
    this.names = names;
    return names;
  }

  // Brings the base tables level with the live rows: every table that changed
  // since it was last written is replaced, in one transaction.
  sync() {
    this.queue = this.queue.catch(() => {}).then(async () => {
      const names = await this.mapping();
      const stale = Object.keys(this.ws.tables).filter((t) => names[t] && this.synced[t] !== this.ws.tableVersion[t]);
      if (!stale.length) return;
      const client = await this.pool.connect();
      try {
        await client.query('BEGIN');
        const versions = {};
        for (const t of stale) {
          const { base, baseCols } = names[t];
          versions[t] = this.ws.tableVersion[t];
          const rows = this.ws.rows(t).map((row) => Object.fromEntries(Object.entries(baseCols).map(([f, c]) => [c, row[f] ?? null])));
          await client.query(`TRUNCATE ${base}`);
          if (rows.length) await client.query(`INSERT INTO ${base} SELECT * FROM json_populate_recordset(null::${base}, $1::json)`, [JSON.stringify(rows)]);
        }
        await client.query('COMMIT');
        Object.assign(this.synced, versions);
      } catch (e) {
        await client.query('ROLLBACK');
        throw e;
      } finally {
        client.release();
      }
    });
    return this.queue;
  }

  async read(tableName, fields, where) {
    const table = this.ws.table(tableName);
    await this.sync();
    const names = (await this.mapping())[tableName];
    if (!names) throw new Error(`The Postgres build has no view for ${tableName}`);
    const wanted = fields || table.fields.map((f) => f.name);
    const col = (f) => { if (!names.viewCols[f]) throw new Error(`${tableName} has no field ${f}`); return names.viewCols[f]; };
    const params = [];
    const conditions = Object.entries(where || {}).map(([f, v]) => { params.push(v); return `${col(f)} = $${params.length}`; });
    const sql = `SELECT ${wanted.map((f) => `${col(f)} AS "${f}"`).join(', ')} FROM ${names.view}${conditions.length ? ` WHERE ${conditions.join(' AND ')}` : ''}`;
    const result = await this.pool.query(sql, params);
    return result.rows.map((row) => Object.fromEntries(wanted.map((f) => [f, normalize(row[f], table.byName[f].datatype)])));
  }

  async answers() {
    await this.sync();
    const names = await this.mapping();
    const started = Date.now();
    const out = {};
    for (const [name, table] of Object.entries(this.ws.tables)) {
      if (!names[name]) { out[name] = null; continue; }
      const cols = table.fields.map((f) => `${names[name].viewCols[f.name]} AS "${f.name}"`).join(', ');
      out[name] = (await this.pool.query(`SELECT ${cols} FROM ${names[name].view}`)).rows;
    }
    this.lastMs = Date.now() - started;
    return out;
  }
}

// ---------------------------------------------------------------------------
export function createEngines({ ws, projectDir, repoRoot, databaseUrl }) {
  const implementations = {
    postgres: () => new PostgresEngine({ ws, databaseUrl }),
    typescript: () => new TypeScriptSdkEngine({ ws, projectDir }),
    owl: () => new LiveSubstrateEngine({ ws, substrate: 'effortless-owl', projectDir, repoRoot, requires: ['effortless-owl/src/ontology.owl', 'effortless-owl/src/rules.shacl.ttl', 'effortless-owl/src/manifest.json'] }),
  };
  const made = {};
  const rowsOf = () => [...ws.rows('Engines')].sort((a, b) => a.SortOrder - b.SortOrder);

  function get(id) {
    const row = rowsOf().find((e) => e.EngineId === id);
    if (!row) throw new Error(`The rulebook's Engines table has no engine ${JSON.stringify(id)}`);
    if (!implementations[id]) throw new Error(`Engine ${id} is in the rulebook's Engines table but this app has no implementation for it`);
    return (made[id] ||= implementations[id]());
  }

  return {
    get,
    defaultId: () => (rowsOf().find((e) => e.IsDefault) || rowsOf()[0]).EngineId,
    async list() {
      return Promise.all(rowsOf().map(async (row) => {
        const status = await get(row.EngineId).status();
        return { id: row.EngineId, label: row.Label, substrate: row.Substrate, description: row.Description, isDefault: !!row.IsDefault, ...status, lastMs: get(row.EngineId).lastMs };
      }));
    },
    forgetSchema() { if (made.postgres) made.postgres.forget(); },

    // Every available engine's answers for the live rows, compared by the
    // conformance harness's own rule (orchestration/live_substrates.py compare).
    async compare() {
      const started = Date.now();
      const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'southpark-compare-'));
      try {
        const args = [path.join(repoRoot, 'orchestration/live_substrates.py'), 'compare', '--project-dir', projectDir, '--answers'];
        const used = [], skipped = [];
        for (const row of rowsOf()) {
          const engine = get(row.EngineId);
          const status = await engine.status();
          if (!status.ok) { skipped.push({ id: row.EngineId, label: row.Label, reason: status.reason }); continue; }
          const file = path.join(dir, `${row.EngineId}.json`);
          fs.writeFileSync(file, JSON.stringify(await engine.answers()));
          // The engine's own time to compute these rows. answers() is often a cache read
          // (the page already computed them), so timing the call here would show that read.
          used.push({ id: row.EngineId, label: row.Label, ms: engine.lastMs });
          args.push(`${row.EngineId}=${file}`);
        }
        if (used.length < 2) throw new Error(`Only ${used.length} engine is available to compare: ${skipped.map((s) => `${s.label}: ${s.reason}`).join('; ')}`);
        const outFile = path.join(dir, 'compare.json');
        await runPython([...args, '--out', outFile]);
        const result = JSON.parse(fs.readFileSync(outFile, 'utf8'));
        return {
          engines: used, skipped, cells: result.cells, differingCount: result.differing.length, missingRowCount: result.missingRows.length,
          differing: result.differing.slice(0, 100), missingRows: result.missingRows.slice(0, 50), elapsedMs: Date.now() - started,
        };
      } finally {
        fs.rmSync(dir, { recursive: true, force: true });
      }
    },
  };
}
