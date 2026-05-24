// =============================================================================
// ERB Admin Portal — Express backend
// =============================================================================
// Boots:
//   - localhost:7777 — admin portal API + static frontend host
//   - Connects to Postgres (system pg on 5432 or override via PG* env)
//   - Auto-creates editor database `erb_admin_<project>` on first boot
//   - Auto-migrates editor DB schema from the active rulebook
//   - Enforces the write-through invariant: every mutating endpoint writes to
//     BOTH Postgres AND the rulebook JSON in the same logical transaction
// =============================================================================

import express from "express";
import cors from "cors";
import pg from "pg";
import fs from "node:fs";
import fsp from "node:fs/promises";
import path from "node:path";
import { spawn, exec } from "node:child_process";
import { fileURLToPath } from "node:url";
import http from "node:http";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");
const META_RULEBOOK = path.join(REPO_ROOT, "effortless-rulebook", "effortless-rulebook.json");
const ACTIVE_DOMAIN_FILE = path.join(REPO_ROOT, "orchestration", "active-domain.txt");
const PORTAL_STATE_FILE = path.join(__dirname, ".portal-state.json");
const RULEBOOK_EXAMPLES = path.join(REPO_ROOT, "rulebook-examples");

const PORT = parseInt(process.env.PORTAL_PORT || "7777", 10);
const PROXY_URL = process.env.PROXY_URL || "http://localhost:4242";

const PG_CONFIG = {
  host: process.env.PGHOST || "localhost",
  port: parseInt(process.env.PGPORT || "5432", 10),
  user: process.env.PGUSER || process.env.USER || "postgres",
  password: process.env.PGPASSWORD || "",
};

// ---------------------------------------------------------------------------
// Active project helpers
// ---------------------------------------------------------------------------
function getActiveDomain() {
  // Honor the orchestration active-domain.txt pointer when present, else default.
  try {
    if (fs.existsSync(ACTIVE_DOMAIN_FILE)) {
      const v = fs.readFileSync(ACTIVE_DOMAIN_FILE, "utf8").trim();
      if (v) return v;
    }
  } catch {}
  return "__meta__"; // special sentinel for the top-level meta-rulebook
}

function setActiveDomain(name) {
  fs.writeFileSync(ACTIVE_DOMAIN_FILE, name + "\n");
}

function activeRulebookPath() {
  const domain = getActiveDomain();
  if (domain === "__meta__") return META_RULEBOOK;
  return path.join(RULEBOOK_EXAMPLES, domain, "effortless-rulebook", `${domain}-rulebook.json`);
}

function activeProjectRoot() {
  const domain = getActiveDomain();
  if (domain === "__meta__") return REPO_ROOT;
  return path.join(RULEBOOK_EXAMPLES, domain);
}

function listProjects() {
  const out = [{
    id: "__meta__",
    name: "ERB Meta-Rulebook (this repo)",
    rulebookPath: META_RULEBOOK,
    projectRoot: REPO_ROOT,
    description: "The repo's own meta-rulebook — describes ERB itself.",
  }];
  if (fs.existsSync(RULEBOOK_EXAMPLES)) {
    for (const d of fs.readdirSync(RULEBOOK_EXAMPLES)) {
      const dirPath = path.join(RULEBOOK_EXAMPLES, d);
      if (!fs.statSync(dirPath).isDirectory()) continue;
      const candidate = path.join(dirPath, "effortless-rulebook", `${d}-rulebook.json`);
      if (!fs.existsSync(candidate)) continue;
      let desc = "";
      try {
        const rb = JSON.parse(fs.readFileSync(candidate, "utf8"));
        desc = rb.Description || rb.Name || "";
      } catch {}
      out.push({
        id: d,
        name: d,
        rulebookPath: candidate,
        projectRoot: dirPath,
        description: desc,
      });
    }
  }
  return out;
}

// ---------------------------------------------------------------------------
// Rulebook IO (the write-through SSoT)
// ---------------------------------------------------------------------------
function loadRulebook(p = activeRulebookPath()) {
  return JSON.parse(fs.readFileSync(p, "utf8"));
}

function saveRulebook(rb, p = activeRulebookPath()) {
  fs.writeFileSync(p, JSON.stringify(rb, null, 2) + "\n");
}

// Portal-config entities (driving nav / roles / users) live in the meta-rulebook
// when the active project hasn't defined them. This makes every project get a
// working admin portal out of the box without needing to copy boilerplate.
const PORTAL_FALLBACK_ENTITIES = [
  "UserRoles", "AppUsers", "AppPermissions",
  "AppNavigation", "AppScreens", "AppAPIs",
  "AddToolCatalog", "BuildPipeline", "AdminPortalRuntime",
];

function loadRulebookWithPortalFallback() {
  const project = loadRulebook();
  const activePath = activeRulebookPath();
  if (activePath === META_RULEBOOK) return project;
  let meta;
  try { meta = JSON.parse(fs.readFileSync(META_RULEBOOK, "utf8")); } catch { return project; }
  for (const k of PORTAL_FALLBACK_ENTITIES) {
    if (!project[k] && meta[k]) project[k] = meta[k];
  }
  return project;
}

// ---------------------------------------------------------------------------
// Portal state (per-machine, not committed)
// ---------------------------------------------------------------------------
function loadPortalState() {
  try {
    return JSON.parse(fs.readFileSync(PORTAL_STATE_FILE, "utf8"));
  } catch {
    return { currentUserId: null };
  }
}

function savePortalState(s) {
  fs.writeFileSync(PORTAL_STATE_FILE, JSON.stringify(s, null, 2));
}

// ---------------------------------------------------------------------------
// Editor Postgres database (per-project)
// ---------------------------------------------------------------------------
function editorDbName() {
  const domain = getActiveDomain();
  const safe = domain.replace(/[^a-z0-9_]/gi, "_").toLowerCase();
  return `erb_admin_${safe}`;
}

async function ensureEditorDatabase() {
  const dbName = editorDbName();
  // Connect to default db to issue CREATE DATABASE if needed.
  const root = new pg.Client({ ...PG_CONFIG, database: "postgres" });
  await root.connect();
  try {
    const exists = await root.query("SELECT 1 FROM pg_database WHERE datname = $1", [dbName]);
    if (exists.rowCount === 0) {
      console.log(`[portal] creating editor DB: ${dbName}`);
      await root.query(`CREATE DATABASE "${dbName}"`);
    }
  } finally {
    await root.end();
  }
  return dbName;
}

async function bootstrapEditorSchema(client, rulebook) {
  // Minimal portal-side mirror: one "rulebook entity" generic table that lets
  // the portal back the editor with live rows. Real domain schema lives in the
  // generated postgres substrate; the portal only mirrors what it edits.
  await client.query(`
    CREATE TABLE IF NOT EXISTS portal_rulebook_entities (
      entity_name TEXT PRIMARY KEY,
      schema_json JSONB NOT NULL,
      data_json   JSONB NOT NULL,
      description TEXT,
      updated_at  TIMESTAMPTZ DEFAULT now()
    );
    CREATE TABLE IF NOT EXISTS portal_app_users (
      user_id      TEXT PRIMARY KEY,
      email        TEXT UNIQUE NOT NULL,
      display_name TEXT NOT NULL,
      role_id      TEXT NOT NULL,
      is_default   BOOLEAN DEFAULT FALSE,
      notes        TEXT
    );
    CREATE TABLE IF NOT EXISTS portal_audit_log (
      id BIGSERIAL PRIMARY KEY,
      ts TIMESTAMPTZ DEFAULT now(),
      user_id TEXT,
      action  TEXT NOT NULL,
      target  TEXT,
      payload JSONB
    );
  `);

  // Seed/refresh from rulebook
  await client.query("BEGIN");
  try {
    for (const [name, value] of Object.entries(rulebook)) {
      if (name.startsWith("$") || name.startsWith("_")) continue;
      if (name === "Name" || name === "Description") continue;
      if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
      await client.query(
        `INSERT INTO portal_rulebook_entities (entity_name, schema_json, data_json, description)
         VALUES ($1, $2, $3, $4)
         ON CONFLICT (entity_name) DO UPDATE
           SET schema_json = EXCLUDED.schema_json,
               data_json   = EXCLUDED.data_json,
               description = EXCLUDED.description,
               updated_at  = now()`,
        [name, JSON.stringify(value.schema), JSON.stringify(value.data || []), value.Description || null]
      );
    }
    if (rulebook.AppUsers && Array.isArray(rulebook.AppUsers.data)) {
      for (const u of rulebook.AppUsers.data) {
        await client.query(
          `INSERT INTO portal_app_users (user_id, email, display_name, role_id, is_default, notes)
           VALUES ($1, $2, $3, $4, $5, $6)
           ON CONFLICT (user_id) DO UPDATE
             SET email = EXCLUDED.email,
                 display_name = EXCLUDED.display_name,
                 role_id = EXCLUDED.role_id,
                 is_default = EXCLUDED.is_default,
                 notes = EXCLUDED.notes`,
          [u.UserId, u.Email, u.DisplayName, u.RoleId, u.IsDefault || false, u.Notes || null]
        );
      }
    }
    await client.query("COMMIT");
  } catch (e) {
    await client.query("ROLLBACK");
    throw e;
  }
}

let pool = null;
async function getPool() {
  if (pool) return pool;
  const dbName = await ensureEditorDatabase();
  pool = new pg.Pool({ ...PG_CONFIG, database: dbName });
  const client = await pool.connect();
  try {
    await bootstrapEditorSchema(client, loadRulebook());
  } finally {
    client.release();
  }
  return pool;
}

// ---------------------------------------------------------------------------
// Permissions
// ---------------------------------------------------------------------------
function getCurrentUser() {
  const st = loadPortalState();
  const rb = loadRulebookWithPortalFallback();
  const users = (rb.AppUsers && rb.AppUsers.data) || [];
  const roles = (rb.UserRoles && rb.UserRoles.data) || [];
  let u = users.find((x) => x.UserId === st.currentUserId);
  if (!u) u = users.find((x) => x.IsDefault) || users[0];
  if (!u) return null;
  const role = roles.find((r) => r.RoleId === u.RoleId) || null;
  const perms = ((rb.AppPermissions && rb.AppPermissions.data) || []).filter(
    (p) => p.RoleId === u.RoleId
  );
  return { ...u, role, permissions: perms };
}

function requireDeveloper(req, res, next) {
  const u = getCurrentUser();
  if (!u || !u.role || u.role.AccessLevel !== "full-admin") {
    return res.status(403).json({ error: "Developer role required." });
  }
  req.currentUser = u;
  next();
}

// ---------------------------------------------------------------------------
// Write-through helper
// ---------------------------------------------------------------------------
async function writeThrough({ pgWrite, rulebookMutate }) {
  // pgWrite: async (client) => void  — runs inside a TXN
  // rulebookMutate: (rulebookJson) => rulebookJson — returns new rulebook
  const p = await getPool();
  const client = await p.connect();
  try {
    await client.query("BEGIN");
    await pgWrite(client);

    const rb = loadRulebook();
    const newRb = rulebookMutate(rb);
    // Filesystem first (committed = on disk). If this throws, we ROLLBACK pg.
    saveRulebook(newRb);

    await client.query("COMMIT");
    return newRb;
  } catch (e) {
    await client.query("ROLLBACK").catch(() => {});
    throw e;
  } finally {
    client.release();
  }
}

// ---------------------------------------------------------------------------
// ssotme-proxy client
// ---------------------------------------------------------------------------
function fetchJson(url) {
  return new Promise((resolve, reject) => {
    const req = http.get(url, (res) => {
      let body = "";
      res.on("data", (c) => (body += c));
      res.on("end", () => {
        try {
          resolve({ status: res.statusCode, body: JSON.parse(body || "{}") });
        } catch (e) {
          resolve({ status: res.statusCode, body: { raw: body } });
        }
      });
    });
    req.on("error", reject);
    req.setTimeout(2000, () => req.destroy(new Error("proxy timeout")));
  });
}

// ---------------------------------------------------------------------------
// Express app
// ---------------------------------------------------------------------------
const app = express();
app.use(cors());
app.use(express.json({ limit: "10mb" }));

app.get("/api/health", (req, res) => res.json({ ok: true }));

app.get("/api/me", (req, res) => {
  const u = getCurrentUser();
  res.json(u || { error: "no user" });
});

app.post("/api/me/switch", (req, res) => {
  const { userId } = req.body || {};
  const st = loadPortalState();
  st.currentUserId = userId;
  savePortalState(st);
  res.json(getCurrentUser());
});

// --- projects ---
app.get("/api/projects", (req, res) => {
  res.json({
    active: getActiveDomain(),
    projects: listProjects(),
  });
});

app.post("/api/projects/:id/activate", requireDeveloper, async (req, res) => {
  const id = req.params.id;
  if (!listProjects().find((p) => p.id === id)) {
    return res.status(404).json({ error: "unknown project" });
  }
  setActiveDomain(id);
  pool = null; // force re-init against new editor DB
  await getPool();
  res.json({ active: getActiveDomain() });
});

// --- rulebook (read) ---
app.get("/api/rulebook", (req, res) => res.json(loadRulebookWithPortalFallback()));

app.get("/api/rulebook/entities", (req, res) => {
  const rb = loadRulebook();
  const out = [];
  for (const [name, value] of Object.entries(rb)) {
    if (name.startsWith("$") || name.startsWith("_")) continue;
    if (name === "Name" || name === "Description") continue;
    if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
    out.push({
      name,
      description: value.Description || null,
      fieldCount: value.schema.length,
      rowCount: (value.data || []).length,
    });
  }
  res.json(out);
});

app.get("/api/rulebook/entities/:name", (req, res) => {
  const rb = loadRulebook();
  const e = rb[req.params.name];
  if (!e) return res.status(404).json({ error: "not found" });
  res.json({ name: req.params.name, ...e });
});

// --- rulebook (write-through) ---
app.patch("/api/rulebook/entities/:name", requireDeveloper, async (req, res) => {
  const { description, schema, data } = req.body || {};
  const name = req.params.name;
  try {
    const newRb = await writeThrough({
      pgWrite: async (c) => {
        await c.query(
          `UPDATE portal_rulebook_entities
             SET description = COALESCE($2, description),
                 schema_json = COALESCE($3, schema_json),
                 data_json   = COALESCE($4, data_json),
                 updated_at  = now()
           WHERE entity_name = $1`,
          [name, description ?? null, schema ? JSON.stringify(schema) : null, data ? JSON.stringify(data) : null]
        );
        await c.query(
          `INSERT INTO portal_audit_log (user_id, action, target, payload)
           VALUES ($1, 'entity.update', $2, $3)`,
          [getCurrentUser()?.UserId || null, name, JSON.stringify(req.body)]
        );
      },
      rulebookMutate: (rb) => {
        if (!rb[name]) throw new Error(`Entity ${name} not in rulebook`);
        if (description !== undefined) rb[name].Description = description;
        if (schema !== undefined) rb[name].schema = schema;
        if (data !== undefined) rb[name].data = data;
        return rb;
      },
    });
    res.json({ ok: true, entity: newRb[name] });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.post("/api/rulebook/entities", requireDeveloper, async (req, res) => {
  const { name, description = "", schema = [], data = [] } = req.body || {};
  if (!name) return res.status(400).json({ error: "name required" });
  try {
    const newRb = await writeThrough({
      pgWrite: async (c) => {
        await c.query(
          `INSERT INTO portal_rulebook_entities (entity_name, schema_json, data_json, description)
           VALUES ($1, $2, $3, $4)`,
          [name, JSON.stringify(schema), JSON.stringify(data), description]
        );
      },
      rulebookMutate: (rb) => {
        if (rb[name]) throw new Error(`Entity ${name} already exists`);
        rb[name] = { Description: description, schema, data };
        return rb;
      },
    });
    res.json({ ok: true, entity: newRb[name] });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.delete("/api/rulebook/entities/:name", requireDeveloper, async (req, res) => {
  const name = req.params.name;
  try {
    await writeThrough({
      pgWrite: async (c) =>
        c.query("DELETE FROM portal_rulebook_entities WHERE entity_name = $1", [name]),
      rulebookMutate: (rb) => {
        delete rb[name];
        return rb;
      },
    });
    res.json({ ok: true });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// --- users (write-through to AppUsers) ---
app.get("/api/users", (req, res) => {
  const rb = loadRulebookWithPortalFallback();
  res.json({
    users: (rb.AppUsers && rb.AppUsers.data) || [],
    roles: (rb.UserRoles && rb.UserRoles.data) || [],
  });
});

app.post("/api/users", requireDeveloper, async (req, res) => {
  const { userId, email, displayName, roleId } = req.body || {};
  if (!userId || !email || !roleId) return res.status(400).json({ error: "missing fields" });
  try {
    await writeThrough({
      pgWrite: async (c) =>
        c.query(
          `INSERT INTO portal_app_users (user_id, email, display_name, role_id) VALUES ($1,$2,$3,$4)`,
          [userId, email, displayName || email, roleId]
        ),
      rulebookMutate: (rb) => {
        rb.AppUsers = rb.AppUsers || { Description: "", schema: [], data: [] };
        rb.AppUsers.data = rb.AppUsers.data || [];
        rb.AppUsers.data.push({
          UserId: userId, Email: email, DisplayName: displayName || email,
          RoleId: roleId, IsDefault: false, Notes: null,
        });
        return rb;
      },
    });
    res.json({ ok: true });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// --- substrates ---
app.get("/api/substrates", async (req, res) => {
  const rb = loadRulebook();
  const subs = (rb.ExecutionSubstrates && rb.ExecutionSubstrates.data) || [];
  const root = activeProjectRoot();
  const enriched = subs.map((s) => {
    const rel = s.RelativePath || "";
    const full = path.join(root, rel.replace(/^execution-substrates\//, ""));
    const exists = fs.existsSync(full);
    return { ...s, fullPath: full, exists };
  });
  res.json(enriched);
});

// --- build ---
app.post("/api/build/all", requireDeveloper, (req, res) => {
  const cwd = activeProjectRoot();
  exec("effortless build", { cwd }, (err, stdout, stderr) => {
    res.json({
      ok: !err,
      cwd,
      stdout: stdout?.slice(-8000) || "",
      stderr: stderr?.slice(-8000) || "",
      error: err ? String(err.message || err) : null,
    });
  });
});

// --- tools (Add Tool) ---
app.get("/api/tools/catalog", async (req, res) => {
  const rb = loadRulebookWithPortalFallback();
  const fromRulebook = (rb.AddToolCatalog && rb.AddToolCatalog.data) || [];
  let liveProxy = null;
  try {
    const p = await fetchJson(`${PROXY_URL}/ping`);
    liveProxy = p.body;
  } catch (e) {
    liveProxy = { error: String(e.message || e) };
  }
  res.json({ fromRulebook, liveProxy, proxyUrl: PROXY_URL });
});

app.get("/api/tools/installed", (req, res) => {
  const cwd = activeProjectRoot();
  const f = path.join(cwd, "effortless.json");
  if (!fs.existsSync(f)) return res.json({ projectRoot: cwd, transpilers: [] });
  try {
    const j = JSON.parse(fs.readFileSync(f, "utf8"));
    res.json({ projectRoot: cwd, transpilers: j.ProjectTranspilers || [] });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.post("/api/tools/install", requireDeveloper, (req, res) => {
  const { installUrl, outputPath } = req.body || {};
  if (!installUrl) return res.status(400).json({ error: "installUrl required" });
  const cwd = activeProjectRoot();
  // Resolve rulebook path RELATIVE to the chosen outputPath inside the project,
  // matching how `effortless -install` expects: run from the output dir.
  const outDir = outputPath ? path.join(cwd, outputPath) : cwd;
  fs.mkdirSync(outDir, { recursive: true });
  const rulebookRel = path.relative(outDir, activeRulebookPath());
  const cmd = `effortless -install ${JSON.stringify(installUrl)} -i ${JSON.stringify(rulebookRel)}`;
  exec(cmd, { cwd: outDir }, (err, stdout, stderr) => {
    res.json({
      ok: !err,
      cmd,
      cwd: outDir,
      stdout: stdout?.slice(-8000) || "",
      stderr: stderr?.slice(-8000) || "",
      error: err ? String(err.message || err) : null,
    });
  });
});

// --- tech tools ---
app.get("/api/tech/postgres/tables", requireDeveloper, async (req, res) => {
  try {
    const p = await getPool();
    const r = await p.query(
      `SELECT table_schema, table_name
         FROM information_schema.tables
        WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
        ORDER BY 1, 2`
    );
    res.json({ database: editorDbName(), tables: r.rows });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.post("/api/tech/postgres/query", requireDeveloper, async (req, res) => {
  const { sql } = req.body || {};
  if (!sql) return res.status(400).json({ error: "sql required" });
  try {
    const p = await getPool();
    const r = await p.query(sql);
    res.json({
      command: r.command,
      rowCount: r.rowCount,
      fields: r.fields?.map((f) => f.name) || [],
      rows: Array.isArray(r.rows) ? r.rows.slice(0, 500) : [],
    });
  } catch (e) {
    res.status(400).json({ error: String(e.message || e) });
  }
});

app.get("/api/tech/proxy/status", requireDeveloper, async (req, res) => {
  try {
    const r = await fetchJson(`${PROXY_URL}/ping`);
    res.json({ url: PROXY_URL, ...r });
  } catch (e) {
    res.status(503).json({ url: PROXY_URL, error: String(e.message || e) });
  }
});

app.get("/api/tech/rulebook-json", requireDeveloper, (req, res) => {
  res.type("application/json").send(fs.readFileSync(activeRulebookPath(), "utf8"));
});

app.put("/api/tech/rulebook-json", requireDeveloper, async (req, res) => {
  const text = typeof req.body === "string" ? req.body : JSON.stringify(req.body, null, 2);
  try {
    const parsed = JSON.parse(text);
    saveRulebook(parsed);
    // Force editor pool reseed
    pool = null;
    await getPool();
    res.json({ ok: true });
  } catch (e) {
    res.status(400).json({ error: String(e.message || e) });
  }
});

app.post("/api/tech/reset", requireDeveloper, async (req, res) => {
  // Drop editor DB and rebuild from rulebook JSON
  if (pool) {
    try { await pool.end(); } catch {}
    pool = null;
  }
  const dbName = editorDbName();
  const root = new pg.Client({ ...PG_CONFIG, database: "postgres" });
  await root.connect();
  try {
    await root.query(`SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname=$1 AND pid<>pg_backend_pid()`, [dbName]);
    await root.query(`DROP DATABASE IF EXISTS "${dbName}"`);
  } finally {
    await root.end();
  }
  await getPool();
  res.json({ ok: true, dbName });
});

// --- static frontend ---
app.use("/", express.static(path.join(__dirname, "web")));

// ---------------------------------------------------------------------------
// Boot
// ---------------------------------------------------------------------------
(async () => {
  try {
    await getPool();
    console.log(`[portal] editor DB ready: ${editorDbName()}`);
  } catch (e) {
    console.error(`[portal] WARNING — could not init editor Postgres: ${e.message}`);
    console.error(`[portal] portal will still serve, but DB-backed features will fail.`);
  }
  app.listen(PORT, () => {
    const rb = activeRulebookPath();
    console.log(`[portal] http://localhost:${PORT}`);
    console.log(`[portal] active rulebook: ${rb}`);
    console.log(`[portal] proxy: ${PROXY_URL}`);
  });
})();
