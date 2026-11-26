// The execution engines. Each one computes every field of the rulebook from the
// same live rows (the workspace); the page and the admin read whichever one a
// request names. None of them is hand-written logic: Postgres reads the generated
// vw_* views, and TypeScript and OWL run the generated substrates through the
// platform's orchestration/live_substrates.py, the same way the conformance
// harness runs them.
import pg from 'pg';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
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

function runPython(args, { timeoutMs = 240000 } = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn('python3', args, { stdio: ['ignore', 'pipe', 'pipe'] });
    let out = '', err = '';
    const timer = setTimeout(() => { child.kill('SIGKILL'); reject(new Error(`python3 ${args[0]} did not finish within ${timeoutMs / 1000}s`)); }, timeoutMs);
    child.stdout.on('data', (d) => { out += d; });
    child.stderr.on('data', (d) => { err += d; });
    child.on('error', (e) => { clearTimeout(timer); reject(e); });
    child.on('close', (code) => {
      clearTimeout(timer);
      if (code !== 0) reject(new Error((err || out).trim().split('\n').slice(-6).join('\n') || `python3 exited ${code}`));
      else resolve(out);
    });
  });
}

function select(table, rows, fields, where) {
  let out = rows;
  if (where) out = out.filter((row) => Object.entries(where).every(([f, v]) => normalize(row[f], table.byName[f].datatype) === v));
  const wanted = fields || table.fields.map((f) => f.name);
  for (const f of wanted) if (!table.byName[f]) throw new Error(`${table.name} has no field ${f}`);
  return out.map((row) => Object.fromEntries(wanted.map((f) => [f, normalize(row[f], table.byName[f].datatype)])));
}

// ---------------------------------------------------------------------------
// A generated substrate run on the live rows (TypeScript, OWL)
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
    typescript: () => new LiveSubstrateEngine({ ws, substrate: 'effortless-typescript', projectDir, repoRoot, requires: ['effortless-typescript/erb_sdk.ts'] }),
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
          const t0 = Date.now();
          const file = path.join(dir, `${row.EngineId}.json`);
          fs.writeFileSync(file, JSON.stringify(await engine.answers()));
          used.push({ id: row.EngineId, label: row.Label, ms: Date.now() - t0 });
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
