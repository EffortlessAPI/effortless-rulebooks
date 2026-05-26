// =============================================================================
// ERB Admin Portal — Express backend
// =============================================================================
// Boots:
//   - localhost:7777 — admin portal API + static frontend host
//   - Connects to Postgres (system pg on 5432 or override via PG* env)
//   - Auto-creates per-domain document database `erb_<domain>` on first boot
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
const PLATFORM_DIR = path.resolve(__dirname, "..");
const REPO_ROOT = path.resolve(PLATFORM_DIR, "..");
const TOP_RULEBOOK = path.join(PLATFORM_DIR, "effortless-rulebook", "effortless-rulebook.json");
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
  if (!fs.existsSync(ACTIVE_DOMAIN_FILE)) {
    throw new Error(
      `active-domain.txt missing at ${ACTIVE_DOMAIN_FILE}. ` +
      `Write the active project name (e.g. 'acme-llc') or '__top__' to that file.`
    );
  }
  const v = fs.readFileSync(ACTIVE_DOMAIN_FILE, "utf8").trim();
  if (!v) {
    throw new Error(
      `active-domain.txt at ${ACTIVE_DOMAIN_FILE} is empty. ` +
      `Write the active project name or '__top__'.`
    );
  }
  return v;
}

function setActiveDomain(name) {
  fs.writeFileSync(ACTIVE_DOMAIN_FILE, name + "\n");
}

function activeRulebookPath() {
  const domain = getActiveDomain();
  if (domain === "__top__") return TOP_RULEBOOK;
  return path.join(RULEBOOK_EXAMPLES, domain, "effortless-rulebook", `${domain}-rulebook.json`);
}

function activeProjectRoot() {
  const domain = getActiveDomain();
  if (domain === "__top__") return REPO_ROOT;
  return path.join(RULEBOOK_EXAMPLES, domain);
}

// Returns { <slug>: { DisplayName, Tagline, LogoPath } } from the meta-rulebook's
// RulebookFlavors table. Read fresh on every call so portal restarts after
// reconcileFlavors() see the new rows.
function flavorsBySlug() {
  try {
    const proj = loadProjectRulebook();
    const rows = proj?.RulebookFlavors?.data || [];
    const out = {};
    for (const r of rows) {
      if (r.ProjectSlug) out[r.ProjectSlug] = r;
    }
    return out;
  } catch {
    return {};
  }
}

function logoUrlForSlug(slug) {
  const png = path.join(RULEBOOK_EXAMPLES, slug, "effortless-logo.png");
  return fs.existsSync(png) ? `/api/projects/${encodeURIComponent(slug)}/logo.png` : null;
}

// Resolve _meta.signature_rows ([{entity, ids[]}, …]) against the rulebook's
// own data arrays, projecting only each entity's `important_fields` so the
// payload stays cheap. Returns [{entity, fields: [...], rows: [...]}, …].
// Silent fallback would be a sin here — if the entity or ID isn't found, we
// skip that row (the rulebook author asked for something that doesn't exist;
// the picker just won't display it). We do not invent rows.
function resolveSignatureRows(rb) {
  const sig = rb?._meta?.signature_rows;
  if (!Array.isArray(sig)) return [];
  const out = [];
  for (const block of sig) {
    if (!block || typeof block !== "object") continue;
    const entity = block.entity;
    const ids = Array.isArray(block.ids) ? block.ids : [];
    const ent = entity && rb[entity];
    if (!ent || !Array.isArray(ent.data)) continue;
    const fields = Array.isArray(ent.important_fields) && ent.important_fields.length
      ? ent.important_fields
      : (ent.schema || []).slice(0, 3).map((f) => f.name);
    const pkField = (ent.schema || []).find((f) => /Id$/i.test(f.name))?.name;
    const rows = [];
    for (const id of ids) {
      const row = ent.data.find((r) => pkField && r[pkField] === id);
      if (!row) continue;
      const projected = {};
      for (const f of fields) projected[f] = row[f] ?? null;
      if (pkField) projected[pkField] = row[pkField];
      rows.push(projected);
    }
    out.push({ entity, fields, rows });
  }
  return out;
}

function listProjects() {
  const flavors = flavorsBySlug();
  const out = [{
    id: "__top__",
    name: "ERB Orchestration (top-level)",
    displayName: "ERB Orchestration (top-level)",
    tagline: "The repo's top-level rulebook — describes ERB itself.",
    motif: "default",
    motifPalette: null,
    descriptionRich: null,
    journalSeed: null,
    signatureRows: [],
    logoUrl: null,
    rulebookPath: TOP_RULEBOOK,
    projectRoot: REPO_ROOT,
    description: "The repo's top-level rulebook — describes ERB itself.",
  }];
  if (fs.existsSync(RULEBOOK_EXAMPLES)) {
    for (const d of fs.readdirSync(RULEBOOK_EXAMPLES)) {
      const dirPath = path.join(RULEBOOK_EXAMPLES, d);
      if (!fs.statSync(dirPath).isDirectory()) continue;
      const candidate = path.join(dirPath, "effortless-rulebook", `${d}-rulebook.json`);
      if (!fs.existsSync(candidate)) continue;
      const flav = flavors[d];
      // Peek at the demo's own _meta for the experience fields (tagline,
      // motif, signature_rows). Falls through to the RulebookFlavors row
      // when _meta hasn't been authored yet — that's a default, not a
      // fallback (see CLAUDE.md: the SSoT-derived default IS the right
      // answer; we only override when the rulebook explicitly takes a
      // position via _meta).
      let demoRb = null;
      try { demoRb = JSON.parse(fs.readFileSync(candidate, "utf8")); }
      catch (e) { console.warn(`[listProjects] could not parse ${candidate}: ${e.message}`); }
      const meta = demoRb?._meta || {};
      const displayName = flav?.DisplayName || d;
      const tagline = meta.tagline || flav?.Tagline || flav?.LearningFocus || "";
      out.push({
        id: d,
        name: displayName,
        displayName,
        tagline,
        motif: meta.motif || "default",
        motifPalette: meta.motif_palette || null,
        descriptionRich: meta.description_rich || null,
        journalSeed: meta.journal_seed || null,
        signatureRows: resolveSignatureRows(demoRb),
        logoUrl: logoUrlForSlug(d),
        rulebookPath: candidate,
        projectRoot: dirPath,
        description: tagline,
      });
    }
  }
  return out;
}

// ---------------------------------------------------------------------------
// Rulebook IO — TWO categorically different reads:
//   - loadProjectRulebook() : the wrapper/orchestration tool itself.
//     Contains portal config: UserRoles, AppUsers, AppPermissions,
//     AppNavigation, AppScreens, AppAPIs, AddToolCatalog, BuildPipeline,
//     AdminPortalRuntime. PATH IS FIXED: ./effortless-rulebook/effortless-rulebook.json
//   - loadRulebook()        : the active DEMO domain's rulebook.
//     Contains the domain's business tables (Customers, Episodes, etc.).
//     PATH IS DYNAMIC: ./rulebook-examples/<domain>/effortless-rulebook/<domain>-rulebook.json
//
// THESE ARE NEVER MIXED. The project is the PARENT, the demo is a CHILD.
// See CLAUDE.md "THE PROJECT RULEBOOK ≠ A DEMO RULEBOOK".
// ---------------------------------------------------------------------------
function loadProjectRulebook() {
  return JSON.parse(fs.readFileSync(TOP_RULEBOOK, "utf8"));
}

function loadRulebook(p = activeRulebookPath()) {
  return JSON.parse(fs.readFileSync(p, "utf8"));
}

function saveRulebook(rb, p = activeRulebookPath()) {
  fs.writeFileSync(p, JSON.stringify(rb, null, 2) + "\n");
}

function saveProjectRulebook(rb) {
  fs.writeFileSync(TOP_RULEBOOK, JSON.stringify(rb, null, 2) + "\n");
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
// RulebookFlavors reconciliation
//
// The project rulebook carries a `RulebookFlavors` table — one row per demo
// under rulebook-examples/. The UI reads it directly. When new demos get added
// to the filesystem, that table goes stale.
//
// On boot we scan rulebook-examples/, count entities/calc/agg/lookup from each
// demo's rulebook, then:
//   - add rows for demos that are on disk but missing from the table
//   - refresh the COUNT fields on existing rows (so numbers stay live)
//   - leave hand-authored fields (Flavor, Complexity, LearningFocus,
//     GoodAnswerKeyFor, DisplayName) alone once a row exists
//
// Default Flavor/Complexity heuristics fire only when adding a NEW row — so
// the first time a demo shows up it gets a reasonable guess, and after that
// the table is hand-tunable without the reconciler clobbering it.
// ---------------------------------------------------------------------------
function countFieldTypes(rulebook) {
  let entities = 0, calc = 0, agg = 0, lookup = 0;
  for (const [name, value] of Object.entries(rulebook)) {
    if (name.startsWith("$") || name.startsWith("_")) continue;
    if (name === "Name" || name === "Description") continue;
    if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
    entities += 1;
    for (const f of value.schema) {
      if (f.type === "calculated") calc += 1;
      else if (f.type === "aggregation") agg += 1;
      else if (f.type === "lookup") lookup += 1;
    }
  }
  return { entities, calc, agg, lookup };
}

function classifyFlavor({ entities, calc, agg }) {
  if (agg >= 3) return "aggregation-heavy";
  if (calc >= 6 && agg < 3) return "computation-heavy";
  if (entities <= 3 && calc <= 2) return "tutorial-ladder";
  return "crud-template";
}

function classifyComplexity(entities) {
  if (entities <= 3) return "minimal";
  if (entities <= 6) return "basic";
  return "advanced";
}

function scanDemoRulebooks() {
  // Returns array of { slug, rulebookPath, entities, calc, agg, lookup }.
  // Only includes demos that actually have a <slug>-rulebook.json file.
  const out = [];
  if (!fs.existsSync(RULEBOOK_EXAMPLES)) return out;
  for (const slug of fs.readdirSync(RULEBOOK_EXAMPLES).sort()) {
    const dir = path.join(RULEBOOK_EXAMPLES, slug);
    if (!fs.statSync(dir).isDirectory()) continue;
    const rbPath = path.join(dir, "effortless-rulebook", `${slug}-rulebook.json`);
    if (!fs.existsSync(rbPath)) continue;
    let rb;
    try {
      rb = JSON.parse(fs.readFileSync(rbPath, "utf8"));
    } catch (e) {
      console.warn(`[flavors] skipping ${slug}: rulebook unparseable (${e.message})`);
      continue;
    }
    const counts = countFieldTypes(rb);
    if (counts.entities === 0) continue;
    out.push({ slug, rulebookPath: rbPath, ...counts });
  }
  return out;
}

function nextFlavorId(existing) {
  let max = 0;
  for (const r of existing) {
    const m = /^flav-(\d+)$/.exec(r.FlavorId || "");
    if (m) max = Math.max(max, parseInt(m[1], 10));
  }
  return (n) => `flav-${String(max + n).padStart(3, "0")}`;
}

function reconcileFlavors() {
  // Returns { added: [...], updated: [...], unchanged: number }.
  const project = loadProjectRulebook();
  if (!project.RulebookFlavors || !Array.isArray(project.RulebookFlavors.data)) {
    console.warn("[flavors] RulebookFlavors table missing from project rulebook — skipping reconcile");
    return { added: [], updated: [], unchanged: 0 };
  }
  const rows = project.RulebookFlavors.data;
  const onDisk = scanDemoRulebooks();
  const bySlug = new Map(rows.map((r) => [r.ProjectSlug, r]));
  const minted = nextFlavorId(rows);
  const added = [];
  const updated = [];
  let unchanged = 0;
  let newIdx = 1;

  for (const d of onDisk) {
    const existing = bySlug.get(d.slug);
    if (!existing) {
      const flavor = classifyFlavor(d);
      const complexity = classifyComplexity(d.entities);
      const row = {
        FlavorId:         minted(newIdx++),
        ProjectSlug:      d.slug,
        DisplayName:      d.slug,
        Flavor:           flavor,
        Complexity:       complexity,
        EntityCount:      d.entities,
        CalculatedCount:  d.calc,
        AggregationCount: d.agg,
        LookupCount:      d.lookup,
        LearningFocus:    `Auto-discovered demo. Replace this stub with a one-line description of what ${d.slug} is designed to teach.`,
        GoodAnswerKeyFor: null,
      };
      rows.push(row);
      added.push(row);
    } else {
      const drift =
        existing.EntityCount      !== d.entities ||
        existing.CalculatedCount  !== d.calc ||
        existing.AggregationCount !== d.agg ||
        existing.LookupCount      !== d.lookup;
      if (drift) {
        existing.EntityCount      = d.entities;
        existing.CalculatedCount  = d.calc;
        existing.AggregationCount = d.agg;
        existing.LookupCount      = d.lookup;
        updated.push(existing);
      } else {
        unchanged += 1;
      }
    }
  }

  if (added.length || updated.length) {
    saveProjectRulebook(project);
  }
  return { added, updated, unchanged };
}

async function reconcileFlavorsOnBoot() {
  let result;
  try {
    result = reconcileFlavors();
  } catch (e) {
    console.error(`[flavors] reconcile failed: ${e.message}`);
    return;
  }
  const { added, updated, unchanged } = result;
  if (!added.length && !updated.length) {
    console.log(`[flavors] up to date (${unchanged} demos)`);
    return;
  }
  for (const r of added) {
    console.log(`[flavors] + added ${r.FlavorId}: ${r.ProjectSlug} (${r.Flavor}, ${r.Complexity})`);
  }
  for (const r of updated) {
    console.log(`[flavors] ~ refreshed counts: ${r.ProjectSlug} (${r.EntityCount}e/${r.CalculatedCount}c/${r.AggregationCount}a/${r.LookupCount}l)`);
  }

  // Mirror into Postgres so the editor view stays consistent. Only attempt if
  // the active project IS the platform rulebook — otherwise the pool is
  // pointing at a demo DB and PlatformFeatures-style mirroring doesn't apply.
  if (getActiveDomain() === "__top__") {
    try {
      const p = await getPool();
      const project = loadProjectRulebook();
      await p.query(
        `UPDATE portal_rulebook_entities
            SET data_json = $2, updated_at = now()
          WHERE entity_name = 'RulebookFlavors'`,
        ["RulebookFlavors", JSON.stringify(project.RulebookFlavors.data)]
      );
    } catch (e) {
      console.warn(`[flavors] postgres mirror skipped: ${e.message}`);
    }
  }
}

// ---------------------------------------------------------------------------
// Editor Postgres database (per-project)
// ---------------------------------------------------------------------------
function domainDbName() {
  const domain = getActiveDomain();
  const safe = domain.replace(/[^a-z0-9_]/gi, "_").toLowerCase();
  return `erb_${safe}`;
}

async function ensureEditorDatabase() {
  const dbName = domainDbName();
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
  // Users, roles, permissions are PORTAL config — they live in the PROJECT
  // rulebook, never in a demo rulebook. See CLAUDE.md.
  const st = loadPortalState();
  const project = loadProjectRulebook();
  const users = (project.AppUsers && project.AppUsers.data) || [];
  const roles = (project.UserRoles && project.UserRoles.data) || [];
  let u = users.find((x) => x.UserId === st.currentUserId);
  if (!u) u = users.find((x) => x.IsDefault) || users[0];
  if (!u) return null;
  const role = roles.find((r) => r.RoleId === u.RoleId) || null;
  const perms = ((project.AppPermissions && project.AppPermissions.data) || []).filter(
    (p) => p.RoleId === u.RoleId
  );
  return { ...u, role, permissions: perms };
}

// Role/capability gates intentionally disabled here. UI hides items the
// current user shouldn't see, but the server does not refuse any request
// based on role. Re-introduce gates here once the UX is stable.
function attachUser(req, _res, next) {
  req.currentUser = getCurrentUser();
  next();
}
const requireDeveloper   = attachUser;
const requireEditor      = attachUser;
const requireBuilder     = attachUser;
const requireUserManager = attachUser;

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

// Serve a demo's effortless-logo.png. 404 if the demo has no logo file.
// Path is under /api/ so it doesn't collide with the SPA's static asset prefix.
app.get("/api/projects/:id/logo.png", (req, res) => {
  const id = req.params.id;
  // Validate against the live project list — never serve arbitrary file paths.
  if (!listProjects().find((p) => p.id === id)) {
    return res.status(404).end();
  }
  const png = path.join(RULEBOOK_EXAMPLES, id, "effortless-logo.png");
  if (!fs.existsSync(png)) return res.status(404).end();
  res.setHeader("Cache-Control", "public, max-age=300");
  res.setHeader("Content-Type", "image/png");
  fs.createReadStream(png).pipe(res);
});

app.post("/api/projects/:id/activate", async (req, res) => {
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
// Active demo rulebook (domain tables: Customers, Episodes, etc.).
app.get("/api/rulebook", (req, res) => res.json(loadRulebook()));

// Project rulebook (portal config: UserRoles, AppUsers, AppPermissions,
// AppNavigation, AppScreens, AppAPIs, AddToolCatalog, BuildPipeline,
// AdminPortalRuntime). The wrapper, not the demo.
app.get("/api/project-rulebook", (req, res) => res.json(loadProjectRulebook()));

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
app.patch("/api/rulebook/entities/:name", requireEditor, async (req, res) => {
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

app.post("/api/rulebook/entities", requireEditor, async (req, res) => {
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

app.delete("/api/rulebook/entities/:name", requireEditor, async (req, res) => {
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

// --- users (PROJECT-rulebook resource; writes to project rulebook) ---
app.get("/api/users", (req, res) => {
  const project = loadProjectRulebook();
  res.json({
    users: (project.AppUsers && project.AppUsers.data) || [],
    roles: (project.UserRoles && project.UserRoles.data) || [],
  });
});

app.post("/api/users", requireUserManager, async (req, res) => {
  const { userId, email, displayName, roleId } = req.body || {};
  if (!userId || !email || !roleId) return res.status(400).json({ error: "missing fields" });
  try {
    // AppUsers lives in the PROJECT rulebook, not the active demo. Write to
    // Postgres + project rulebook in the same logical transaction.
    const p = await getPool();
    const client = await p.connect();
    try {
      await client.query("BEGIN");
      await client.query(
        `INSERT INTO portal_app_users (user_id, email, display_name, role_id) VALUES ($1,$2,$3,$4)`,
        [userId, email, displayName || email, roleId]
      );
      const project = loadProjectRulebook();
      project.AppUsers = project.AppUsers || { Description: "", schema: [], data: [] };
      project.AppUsers.data = project.AppUsers.data || [];
      project.AppUsers.data.push({
        UserId: userId, Email: email, DisplayName: displayName || email,
        RoleId: roleId, IsDefault: false, Notes: null,
      });
      saveProjectRulebook(project);
      await client.query("COMMIT");
    } catch (e) {
      await client.query("ROLLBACK").catch(() => {});
      throw e;
    } finally {
      client.release();
    }
    res.json({ ok: true });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// --- platform features (PROJECT-rulebook resource) ---
// PlatformFeatures is the formal SSoT for what ERB does. Hand-maintained
// per-feature README files MUST conform to these rows. IsReadmeStub is
// computed from the README's on-disk length (overrides the rulebook's
// optional ReadmeLength when the file exists).
function readmeOnDiskLength(relPath) {
  if (!relPath) return null;
  const abs = path.join(REPO_ROOT, relPath);
  if (!fs.existsSync(abs)) return 0;
  return fs.statSync(abs).size;
}
function decorateFeature(row) {
  const onDisk = readmeOnDiskLength(row.ReadmeFilePath);
  const len = onDisk == null ? row.ReadmeLength : onDisk;
  return {
    ...row,
    ReadmeOnDiskLength: onDisk,
    EffectiveReadmeLength: len,
    IsReadmeStub: len == null || len < 400,
  };
}

app.get("/api/features", (req, res) => {
  const project = loadProjectRulebook();
  const rows = (project.PlatformFeatures && project.PlatformFeatures.data) || [];
  const decorated = rows.map(decorateFeature).sort((a, b) => {
    if (a.Tier !== b.Tier) return a.Tier === "headline" ? -1 : 1;
    return (a.Priority || 0) - (b.Priority || 0);
  });
  res.json({
    headline: decorated.filter((r) => r.Tier === "headline"),
    additional: decorated.filter((r) => r.Tier !== "headline"),
  });
});

app.get("/api/features/:id", (req, res) => {
  const project = loadProjectRulebook();
  const rows = (project.PlatformFeatures && project.PlatformFeatures.data) || [];
  const row = rows.find((r) => r.FeatureId === req.params.id);
  if (!row) return res.status(404).json({ error: "not found" });
  const axiom = row.RelatedAxiomId
    ? (project.OntologyAxioms?.data || []).find((a) => a.AxiomId === row.RelatedAxiomId) || null
    : null;
  // Resolve on-disk README content (best-effort; null if missing).
  let readmeOnDisk = null;
  if (row.ReadmeFilePath) {
    const abs = path.join(REPO_ROOT, row.ReadmeFilePath);
    if (fs.existsSync(abs)) readmeOnDisk = fs.readFileSync(abs, "utf8");
  }
  res.json({ ...decorateFeature(row), Axiom: axiom, ReadmeOnDisk: readmeOnDisk });
});

const FEATURE_PATCHABLE = new Set([
  "Name", "ShortName", "Tier", "Priority", "OneLineSummary",
  "ReadmeFilePath", "ReadmeStubContent", "Status", "RelatedAxiomId",
]);
app.patch("/api/features/:id", requireEditor, async (req, res) => {
  const id = req.params.id;
  const updates = {};
  for (const [k, v] of Object.entries(req.body || {})) {
    if (FEATURE_PATCHABLE.has(k)) updates[k] = v;
  }
  if (!Object.keys(updates).length) {
    return res.status(400).json({ error: "no patchable fields in body" });
  }
  try {
    const p = await getPool();
    const client = await p.connect();
    try {
      await client.query("BEGIN");
      const project = loadProjectRulebook();
      if (!project.PlatformFeatures || !Array.isArray(project.PlatformFeatures.data)) {
        throw new Error("PlatformFeatures table missing from project rulebook");
      }
      const idx = project.PlatformFeatures.data.findIndex((r) => r.FeatureId === id);
      if (idx === -1) throw new Error(`Feature ${id} not found`);
      Object.assign(project.PlatformFeatures.data[idx], updates);
      // Mirror into the portal_rulebook_entities row so the editor view stays consistent.
      await client.query(
        `UPDATE portal_rulebook_entities
            SET data_json = $2, updated_at = now()
          WHERE entity_name = 'PlatformFeatures'`,
        ["PlatformFeatures", JSON.stringify(project.PlatformFeatures.data)]
      );
      await client.query(
        `INSERT INTO portal_audit_log (user_id, action, target, payload)
         VALUES ($1, 'feature.update', $2, $3)`,
        [getCurrentUser()?.UserId || null, id, JSON.stringify(updates)]
      );
      saveProjectRulebook(project);
      await client.query("COMMIT");
      res.json({ ok: true, feature: decorateFeature(project.PlatformFeatures.data[idx]) });
    } catch (e) {
      await client.query("ROLLBACK").catch(() => {});
      throw e;
    } finally {
      client.release();
    }
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// --- substrates (PROJECT-rulebook resource) ---
app.get("/api/substrates", async (req, res) => {
  const project = loadProjectRulebook();
  const subs = (project.ExecutionSubstrates && project.ExecutionSubstrates.data) || [];
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
app.post("/api/build/all", requireBuilder, (req, res) => {
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

// --- tools (Add Tool — PROJECT-rulebook resource: AddToolCatalog) ---
app.get("/api/tools/catalog", async (req, res) => {
  const project = loadProjectRulebook();
  const fromRulebook = (project.AddToolCatalog && project.AddToolCatalog.data) || [];
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

app.post("/api/tools/install", requireBuilder, (req, res) => {
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
    res.json({ database: domainDbName(), tables: r.rows });
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
  const dbName = domainDbName();
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
    console.log(`[portal] domain DB ready: ${domainDbName()}`);
  } catch (e) {
    console.error(`[portal] WARNING — could not init editor Postgres: ${e.message}`);
    console.error(`[portal] portal will still serve, but DB-backed features will fail.`);
  }
  await reconcileFlavorsOnBoot();
  app.listen(PORT, () => {
    const rb = activeRulebookPath();
    console.log(`[portal] http://localhost:${PORT}`);
    console.log(`[portal] active rulebook: ${rb}`);
    console.log(`[portal] proxy: ${PROXY_URL}`);
  });
})();
