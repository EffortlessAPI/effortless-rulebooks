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
import crypto from "node:crypto";
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

// ---------------------------------------------------------------------------
// __meta__ table helpers — the rulebook's project-level metadata (tagline,
// motif, description_rich, use_cases, signature_rows, substrates, etc.) is
// stored as a typed-row table:
//   { MetaKey, Name, ValueType: 'string'|'object'|'array', StringValue, JsonValue }
// These helpers project it back to / from a flat {key: value} dictionary so
// the rest of the portal code can keep treating it as an object.
// ---------------------------------------------------------------------------
const META_TABLE_NAME = "__meta__";
const META_SCHEMA = [
  { name: "MetaKey",     datatype: "string", type: "raw",        nullable: false,
    Description: "The metadata key (e.g. 'tagline', 'motif_palette', 'substrates'). Unique within the table." },
  { name: "Name",        datatype: "string", type: "calculated", nullable: false, formula: "={{MetaKey}}",
    Description: "Identifier for this metadata entry. Mirrors MetaKey." },
  { name: "ValueType",   datatype: "string", type: "raw",        nullable: false,
    Description: "How to interpret the value columns: 'string' (use StringValue), 'object' or 'array' (parse JsonValue)." },
  { name: "StringValue", datatype: "string", type: "raw",        nullable: true,
    Description: "Plain string value. Populated when ValueType == 'string'; null otherwise." },
  { name: "JsonValue",   datatype: "string", type: "raw",        nullable: true,
    Description: "JSON-encoded value. Populated when ValueType == 'object' or 'array'; null when 'string'." },
];

function metaAsObject(rb) {
  const tbl = rb?.[META_TABLE_NAME];
  if (!tbl || !Array.isArray(tbl.data)) return {};
  const out = {};
  for (const row of tbl.data) {
    if (!row || typeof row !== "object") continue;
    const key = row.MetaKey;
    if (typeof key !== "string" || !key) continue;
    if (row.ValueType === "string") {
      out[key] = row.StringValue ?? "";
    } else if (row.ValueType === "object" || row.ValueType === "array") {
      if (typeof row.JsonValue === "string" && row.JsonValue.length) {
        try { out[key] = JSON.parse(row.JsonValue); }
        catch (e) { console.warn(`[metaAsObject] bad JsonValue for ${key}: ${e.message}`); }
      }
    }
  }
  return out;
}

function valueToMetaRow(key, value) {
  const row = { MetaKey: key, Name: key, ValueType: "string", StringValue: null, JsonValue: null };
  if (typeof value === "string") {
    row.ValueType = "string"; row.StringValue = value; row.JsonValue = null;
  } else if (Array.isArray(value)) {
    row.ValueType = "array"; row.StringValue = null; row.JsonValue = JSON.stringify(value);
  } else if (value && typeof value === "object") {
    row.ValueType = "object"; row.StringValue = null; row.JsonValue = JSON.stringify(value);
  } else {
    row.ValueType = "string"; row.StringValue = value == null ? "" : String(value); row.JsonValue = null;
  }
  return row;
}

function setMetaValue(rb, key, value) {
  let tbl = rb[META_TABLE_NAME];
  if (!tbl || !Array.isArray(tbl.data)) {
    tbl = {
      Description: "Project-level metadata that travels with the rulebook. One row per metadata key.",
      important: false,
      schema: META_SCHEMA,
      data: [],
    };
    rb[META_TABLE_NAME] = tbl;
  }
  const newRow = valueToMetaRow(key, value);
  const i = tbl.data.findIndex((r) => r && r.MetaKey === key);
  if (i >= 0) tbl.data[i] = newRow; else tbl.data.push(newRow);
}

// Resolve __meta__.signature_rows ([{entity, ids[]}, …]) against the rulebook's
// own data arrays, projecting only each entity's `important_fields` so the
// payload stays cheap. Returns [{entity, fields: [...], rows: [...]}, …].
// Silent fallback would be a sin here — if the entity or ID isn't found, we
// skip that row (the rulebook author asked for something that doesn't exist;
// the picker just won't display it). We do not invent rows.
function resolveSignatureRows(rb) {
  const sig = metaAsObject(rb).signature_rows;
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
      // Peek at the demo's own __meta__ table for the experience fields
      // (tagline, motif, signature_rows). Falls through to the
      // RulebookFlavors row when the metadata hasn't been authored yet —
      // that's a default, not a fallback (see CLAUDE.md: the SSoT-derived
      // default IS the right answer; we only override when the rulebook
      // explicitly takes a position via its __meta__ table).
      let demoRb = null;
      try { demoRb = JSON.parse(fs.readFileSync(candidate, "utf8")); }
      catch (e) { console.warn(`[listProjects] could not parse ${candidate}: ${e.message}`); }
      const meta = metaAsObject(demoRb);
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
    -- Per-user, per-domain "where was I" memory. Powers the picker's
    -- "Last visited" / "New since you were here" chips and the
    -- reception desk's "Welcome back" journal diff.
    CREATE TABLE IF NOT EXISTS portal_user_domain_state (
      user_id TEXT NOT NULL,
      domain  TEXT NOT NULL,
      last_route TEXT,
      last_visited_at TIMESTAMPTZ DEFAULT now(),
      last_seen_rulebook_revision TEXT,
      PRIMARY KEY (user_id, domain)
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

// --- per-user, per-domain state (Item 6: state preservation) ---
//
// Computes a stable revision marker for a rulebook by hashing its on-disk
// content. SHA-256 of the bytes — same content = same revision, byte-for-byte.
// Used to drive the "new since you were here" picker chip and the
// reception desk's "Welcome back" diff.
function rulebookRevisionForDomain(domainId) {
  let rbPath;
  if (domainId === "__top__") rbPath = TOP_RULEBOOK;
  else rbPath = path.join(RULEBOOK_EXAMPLES, domainId, "effortless-rulebook", `${domainId}-rulebook.json`);
  if (!fs.existsSync(rbPath)) return null;
  const buf = fs.readFileSync(rbPath);
  return crypto.createHash("sha256").update(buf).digest("hex").slice(0, 16);
}

// GET /api/portal/me/domain-state
// Returns the current user's per-domain memory PLUS each domain's current
// rulebook revision, so the client can compute `changed-since-last-visit`
// locally without an extra fetch per domain.
app.get("/api/portal/me/domain-state", async (req, res) => {
  const u = getCurrentUser();
  if (!u || !u.UserId) return res.json({ states: [], currentRevisions: {} });
  try {
    const p = await getPool();
    const r = await p.query(
      `SELECT domain, last_route, last_visited_at, last_seen_rulebook_revision
         FROM portal_user_domain_state WHERE user_id = $1
        ORDER BY last_visited_at DESC`,
      [u.UserId]
    );
    const currentRevisions = {};
    for (const proj of listProjects()) {
      currentRevisions[proj.id] = rulebookRevisionForDomain(proj.id);
    }
    res.json({ states: r.rows, currentRevisions });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// PUT /api/portal/me/domain-state
// Body: { domain, last_route }
// Upserts the user's "I was here" memory. Also stamps
// last_seen_rulebook_revision to the rulebook's current content hash,
// so the next visit can compute the diff.
app.put("/api/portal/me/domain-state", async (req, res) => {
  const u = getCurrentUser();
  if (!u || !u.UserId) return res.status(401).json({ error: "no user" });
  const { domain, last_route } = req.body || {};
  if (!domain || typeof domain !== "string") return res.status(400).json({ error: "domain required" });
  const rev = rulebookRevisionForDomain(domain);
  try {
    const p = await getPool();
    await p.query(
      `INSERT INTO portal_user_domain_state (user_id, domain, last_route, last_visited_at, last_seen_rulebook_revision)
       VALUES ($1, $2, $3, now(), $4)
       ON CONFLICT (user_id, domain) DO UPDATE
         SET last_route = EXCLUDED.last_route,
             last_visited_at = now(),
             last_seen_rulebook_revision = EXCLUDED.last_seen_rulebook_revision`,
      [u.UserId, domain, last_route || null, rev]
    );
    res.json({ ok: true, revision: rev });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// --- rulebook (read) ---
// Active demo rulebook (domain tables: Customers, Episodes, etc.).
app.get("/api/rulebook", (req, res) => res.json(loadRulebook()));

// --- rulebook history (Item 7: time-travel scrubber) ---
//
// Git is the substitute for bitemporal history in v1. Each commit that
// touched the rulebook JSON becomes a "tick" on the scrubber. The full
// bitemporal vision (drag arbitrary timestamps, see live-fired rules in
// the past) requires a per-cell history that doesn't exist yet — this is
// the honest shortcut: real history granular to commit boundaries.
//
// Endpoints only ever read the active rulebook path — never an arbitrary
// path supplied by the client. (`req.params.sha` IS supplied by the
// client; we validate it as hex before interpolating into the shell.)

function gitLogForFile(absPath) {
  return new Promise((resolve, reject) => {
    const repoRoot = REPO_ROOT;
    const rel = path.relative(repoRoot, absPath);
    exec(
      `git log --pretty=format:"%H|%ct|%an|%s" -- ${JSON.stringify(rel)}`,
      { cwd: repoRoot, maxBuffer: 4 * 1024 * 1024 },
      (err, stdout) => {
        if (err) return reject(err);
        const lines = (stdout || "").split("\n").filter(Boolean);
        const out = lines.map((line) => {
          const [sha, ct, author, ...rest] = line.split("|");
          return {
            sha,
            timestamp: Number(ct) * 1000,
            author,
            subject: rest.join("|"),
          };
        });
        resolve(out);
      }
    );
  });
}

function gitShowFileAtSha(absPath, sha) {
  return new Promise((resolve, reject) => {
    const repoRoot = REPO_ROOT;
    const rel = path.relative(repoRoot, absPath);
    // Validate sha is a hex string before interpolating.
    if (!/^[0-9a-f]{4,40}$/i.test(sha)) return reject(new Error("invalid sha"));
    exec(
      `git show ${sha}:${JSON.stringify(rel)}`,
      { cwd: repoRoot, maxBuffer: 16 * 1024 * 1024 },
      (err, stdout) => {
        if (err) return reject(err);
        resolve(stdout);
      }
    );
  });
}

app.get("/api/rulebook/history", async (req, res) => {
  try {
    const rbPath = activeRulebookPath();
    const log = await gitLogForFile(rbPath);
    res.json({ path: path.relative(REPO_ROOT, rbPath), commits: log });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.get("/api/rulebook/at/:sha", async (req, res) => {
  try {
    const rbPath = activeRulebookPath();
    const content = await gitShowFileAtSha(rbPath, req.params.sha);
    // Parse to ensure it's valid JSON at that point in history — if it
    // isn't, surface the error instead of returning a corrupt blob.
    const parsed = JSON.parse(content);
    res.json(parsed);
  } catch (e) {
    res.status(404).json({ error: String(e.message || e) });
  }
});

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

// --- rulebook text-field patch (rich-text editor for *_rich fields) ---
// Edits a single allow-listed text field within the active demo rulebook by
// path. Used by the inline rich-text editor in the reception desk.
//
// Allowed paths (any other shape is rejected, never silently coerced):
//   ["__meta__", "tagline"]
//   ["__meta__", "description_rich"]
//   ["__meta__", "journal_seed"]
//   ["__meta__", "use_cases", <int>]
//   [<EntityName>, "summary_rich"]
//   [<EntityName>, "schema", <int>, "explanation_rich"]
//
// Legacy clients may still send "_meta" as path[0]; we treat it as a synonym
// for "__meta__" to keep the old REST contract working until the frontend is
// updated. The storage is always the __meta__ table.
//
// All other writes go through the typed entity endpoints above. Body shape:
//   { path: [...], value: "string" }
//
// NOTE: this writes through the existing transactional helper but does NOT
// mirror into portal_rulebook_entities for __meta__ rows — that table only
// knows about business entities and we treat the rulebook JSON as the
// authoritative source for project-level metadata.
function isMetaPathRoot(seg) {
  return seg === META_TABLE_NAME || seg === "_meta";
}

function validateRichPath(path, rb) {
  if (!Array.isArray(path) || path.length < 2) {
    throw new Error("path must be a non-empty array with at least 2 segments");
  }
  if (isMetaPathRoot(path[0])) {
    const second = path[1];
    if (second === "tagline" || second === "description_rich" || second === "journal_seed") {
      if (path.length !== 2) throw new Error(`${path[0]}.${second} takes no further path segments`);
      return;
    }
    if (second === "use_cases") {
      if (path.length !== 3 || !Number.isInteger(path[2]) || path[2] < 0) {
        throw new Error(`${path[0]}.use_cases requires a non-negative integer index`);
      }
      return;
    }
    throw new Error(`${path[0]}.${second} is not an editable text field`);
  }
  // Otherwise path[0] is an entity name.
  const ent = rb[path[0]];
  if (!ent || typeof ent !== "object" || !Array.isArray(ent.schema)) {
    throw new Error(`Entity ${path[0]} not found or has no schema`);
  }
  if (path[1] === "summary_rich") {
    if (path.length !== 2) throw new Error(`${path[0]}.summary_rich takes no further path segments`);
    return;
  }
  if (path[1] === "schema") {
    if (path.length !== 4 || !Number.isInteger(path[2]) || path[3] !== "explanation_rich") {
      throw new Error(`schema rich-text edits require [entity, "schema", index, "explanation_rich"]`);
    }
    if (path[2] < 0 || path[2] >= ent.schema.length) throw new Error("schema index out of bounds");
    return;
  }
  throw new Error(`${path[0]}.${path[1]} is not an editable text field`);
}

function setByPath(obj, path, value) {
  // __meta__ paths land in the typed table, not in a nested object tree.
  if (isMetaPathRoot(path[0])) {
    const metaKey = path[1];
    if (path.length === 2) {
      setMetaValue(obj, metaKey, value);
      return;
    }
    if (metaKey === "use_cases" && path.length === 3 && Number.isInteger(path[2])) {
      const current = metaAsObject(obj).use_cases;
      const arr = Array.isArray(current) ? current.slice() : [];
      while (arr.length <= path[2]) arr.push("");
      arr[path[2]] = value;
      setMetaValue(obj, "use_cases", arr);
      return;
    }
    throw new Error(`unsupported meta path: ${path.join(".")}`);
  }
  let cur = obj;
  for (let i = 0; i < path.length - 1; i++) {
    const seg = path[i];
    if (cur[seg] == null) {
      if (path[i + 1] === 0 || Number.isInteger(path[i + 1])) cur[seg] = [];
      else cur[seg] = {};
    }
    cur = cur[seg];
  }
  cur[path[path.length - 1]] = value;
}

app.patch("/api/rulebook/text", requireEditor, async (req, res) => {
  const { path, value } = req.body || {};
  if (typeof value !== "string") {
    return res.status(400).json({ error: "value must be a string" });
  }
  try {
    const rb = loadRulebook();
    validateRichPath(path, rb);
    const newRb = await writeThrough({
      pgWrite: async (c) => {
        await c.query(
          `INSERT INTO portal_audit_log (user_id, action, target, payload)
           VALUES ($1, 'rulebook.text.update', $2, $3)`,
          [getCurrentUser()?.UserId || null, path.join("."), JSON.stringify({ path, value })]
        );
      },
      rulebookMutate: (rb2) => {
        setByPath(rb2, path, value);
        return rb2;
      },
    });
    res.json({ ok: true, rulebook: newRb });
  } catch (e) {
    res.status(400).json({ error: String(e.message || e) });
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

// =============================================================================
// Effortless Explorer  (DOMAIN_UX_VISION.md §2)
// =============================================================================
// Walks the rulebook's DAG. The tree endpoint returns entity-level shape for
// the left-nav; subsequent /api/explorer/node calls (step 3) drive instance
// drill-down + scoped child lists. "FK" here means the canonical ERB signal:
// a field with type "relationship" carrying a "RelatedTo" target. lookup /
// aggregation fields are derived values, not FKs, and don't count.
// -----------------------------------------------------------------------------

// Iterate the entity tables in a rulebook in source order, skipping JSON
// metadata keys ($schema, _meta) and the rulebook-level Name/Description.
function* iterEntities(rb) {
  for (const [name, value] of Object.entries(rb)) {
    if (name.startsWith("$") || name.startsWith("_")) continue;
    if (name === "Name" || name === "Description") continue;
    if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
    yield [name, value];
  }
}

// ERB relationship fields are bidirectional: every real FK pair shows up as
// TWO type=relationship fields, one on each entity. To get a sensible DAG we
// have to pick exactly one side as the "real" outbound FK. Two signals,
// tried in order:
//
//   1. prefersSingleRecordLink — when explicitly true/false (ACME-style
//      rulebooks). true = the FK / "many" side; false = the inverse /
//      "one-to-collection" side.
//
//   2. Data shape — when the flag is missing (star-trek-style rulebooks).
//      The "many" side stores a single related id ("1-tos"); the inverse
//      side stores a comma-separated list of related ids
//      ("1-tos-season-1, 1-tos-season-2, ..."). First non-empty sample
//      decides.
//
// If neither signal is available (no flag, no data), we keep the field —
// better to show a relationship that turns out to be inverse than to drop a
// real FK silently.
function isInverseRelationshipField(f, entityData) {
  if (f.prefersSingleRecordLink === true)  return false;
  if (f.prefersSingleRecordLink === false) return true;
  if (!Array.isArray(entityData)) return false;
  for (const row of entityData) {
    const v = row[f.name];
    if (v === null || v === undefined || v === "") continue;
    if (typeof v !== "string") return false;
    const parts = v.split(",").map((s) => s.trim()).filter(Boolean);
    return parts.length > 1;
  }
  return false;
}

// For each entity, returns { outbound: [{ field, to }], rowCount, important }.
// Outbound FKs are the forward (non-inverse) relationship fields IN this
// entity pointing AT another. The inverse map (who points AT me — children)
// is derived in the tree handler.
function describeEntities(rb) {
  const out = {};
  for (const [name, value] of iterEntities(rb)) {
    out[name] = {
      rowCount: Array.isArray(value.data) ? value.data.length : 0,
      important: !!value.important,
      outbound: (value.schema || [])
        .filter((f) => f.type === "relationship"
                    && f.RelatedTo
                    && !isInverseRelationshipField(f, value.data))
        .map((f) => ({ field: f.name, to: f.RelatedTo })),
    };
  }
  return out;
}

function loadRulebookForDomain(domain) {
  const rbPath = (domain === "__top__")
    ? TOP_RULEBOOK
    : path.join(RULEBOOK_EXAMPLES, domain, "effortless-rulebook", `${domain}-rulebook.json`);
  if (!fs.existsSync(rbPath)) {
    const err = new Error(`rulebook for domain '${domain}' not found at ${rbPath}`);
    err.statusCode = 404;
    throw err;
  }
  return JSON.parse(fs.readFileSync(rbPath, "utf8"));
}

// Spec §2.6 says URL ids are the entity's Name. But ERB FK *values* in data
// (e.g. Projects.ApprovedBy = "sarah-chen") reference a PK-style field, not
// Name (Name would be "Sarah Chen"). So scoping a child list under a parent
// instance is a two-step lookup: URL.Name → row → row.PK → filter children
// whose FK field matches that PK. This helper finds the PK field name.
function findPkField(entity) {
  const explicit = (entity.schema || []).find((f) => f.isPk === true);
  if (explicit) return explicit.name;
  const idField = (entity.schema || []).find(
    (f) => f.type === "raw" && f.nullable === false && /Id$/.test(f.name)
  );
  if (idField) return idField.name;
  const firstNonNull = (entity.schema || []).find(
    (f) => f.type === "raw" && f.nullable === false
  );
  return firstNonNull ? firstNonNull.name : "Name";
}

function findRowByName(entity, name) {
  return (entity.data || []).find((r) => String(r.Name) === String(name)) || null;
}

// GET /api/explorer/tree?domain=<slug>&maxDepth=<n>
// Returns the entity-level DAG shape. Top-level = every entity in the rulebook
// (unfiltered cross-cut). children[] = entities that have a relationship FK
// pointing AT this one (i.e., "what would hang off an instance of this
// entity"). With maxDepth=0 the children arrays are empty (useful when the
// client only wants entity row counts).
app.get("/api/explorer/tree", (req, res) => {
  const domain = req.query.domain || getActiveDomain();
  const maxDepth = Math.max(0, parseInt(req.query.maxDepth ?? "1", 10));
  try {
    const rb = loadRulebookForDomain(domain);
    const desc = describeEntities(rb);

    // Inverse FK index: for entity E, who has an outbound FK pointing AT E?
    const inbound = {};
    for (const name of Object.keys(desc)) inbound[name] = [];
    for (const [name, info] of Object.entries(desc)) {
      for (const fk of info.outbound) {
        if (!inbound[fk.to]) continue;     // FK target isn't a known entity
        inbound[fk.to].push({ entity: name, viaFk: fk.field });
      }
    }

    const topLevel = Object.entries(desc).map(([name, info]) => ({
      entity: name,
      rowCount: info.rowCount,
      important: info.important,
      children: maxDepth >= 1
        ? inbound[name].map((c) => ({
            entity: c.entity,
            rowCount: desc[c.entity]?.rowCount ?? 0,
            viaFk: c.viaFk,
          }))
        : [],
    }));

    res.json({
      domain,
      rulebookRevision: rulebookRevisionForDomain(domain),
      topLevel,
    });
  } catch (e) {
    res.status(e.statusCode || 500).json({ error: String(e.message || e) });
  }
});

// GET /api/explorer/node?path=<encoded>&page=<n>&pageSize=<n>
// The §2.7 workhorse. `path` segments alternate entity / id / entity / id…
// per §2.6. Odd length = list node; even length = instance node. Schema and
// rows ride in the same payload (§2.3: schema and data are combined metadata,
// not two separate fetches).
app.get("/api/explorer/node", (req, res) => {
  const domain = req.query.domain || getActiveDomain();
  const pathStr = req.query.path || "";
  const page = Math.max(0, parseInt(req.query.page ?? "0", 10));
  const pageSize = Math.max(1, Math.min(500, parseInt(req.query.pageSize ?? "50", 10)));

  try {
    const rb = loadRulebookForDomain(domain);
    const segments = pathStr.split("/").filter(Boolean).map(decodeURIComponent);

    if (segments.length === 0) {
      return res.json({ kind: "root", domain });
    }

    // Walk path pairs to validate every parent entity/id exists and to build
    // the scopedBy ancestry. Even-positioned segments (0, 2, 4, …) are entity
    // names; odd-positioned are instance Names.
    const scope = []; // [{ entity, id, pk, pkValue }]
    for (let i = 0; i + 1 < segments.length; i += 2) {
      const ent = segments[i], id = segments[i + 1];
      const e = rb[ent];
      if (!e || !Array.isArray(e.schema)) {
        return res.status(404).json({ error: `entity '${ent}' not found` });
      }
      const row = findRowByName(e, id);
      if (!row) {
        return res.status(404).json({ error: `${ent} with Name='${id}' not found` });
      }
      const pk = findPkField(e);
      scope.push({ entity: ent, id, pk, pkValue: row[pk] });
    }

    const isInstance = segments.length % 2 === 0;

    if (isInstance) {
      // Final pair IS the instance. Look it up.
      const { entity: entityName, id } = scope[scope.length - 1];
      const entity = rb[entityName];
      const row = findRowByName(entity, id);

      // Tabs: every entity X with a forward FK pointing at this entity. For
      // each, count rows in X.data whose FK field matches this instance's PK.
      const pk = findPkField(entity);
      const pkValue = row[pk];
      const tabs = [];
      for (const [otherName, otherValue] of iterEntities(rb)) {
        if (otherName === entityName) continue;
        for (const f of otherValue.schema) {
          if (f.type !== "relationship" || f.RelatedTo !== entityName) continue;
          if (isInverseRelationshipField(f, otherValue.data)) continue;
          const count = (otherValue.data || []).filter(
            (r) => String(r[f.name] ?? "").trim() === String(pkValue ?? "").trim()
          ).length;
          tabs.push({ entity: otherName, viaFk: f.name, rowCount: count });
        }
      }

      return res.json({
        kind: "instance",
        entity: entityName,
        id,
        pk,
        pkValue,
        scope: scope.slice(0, -1),  // ancestors, not including this instance
        schema: entity.schema,
        row,
        tabs,
      });
    }

    // List node — final unpaired segment is the entity name.
    const entityName = segments[segments.length - 1];
    const entity = rb[entityName];
    if (!entity || !Array.isArray(entity.schema)) {
      return res.status(404).json({ error: `entity '${entityName}' not found` });
    }

    let rows = entity.data || [];
    let scopedBy = null;

    if (scope.length > 0) {
      // Filter rows of `entityName` whose forward FK points at the immediate
      // parent instance. There can be multiple FKs from this entity to the
      // parent (e.g. Projects has both ApprovedBy and RequestedBy pointing
      // at Employees) — for v1 we pick the first matching FK and document
      // the limitation. A future query-param `viaFk=<field>` can disambiguate.
      const parent = scope[scope.length - 1];
      const fk = entity.schema.find(
        (f) => f.type === "relationship"
            && f.RelatedTo === parent.entity
            && !isInverseRelationshipField(f, entity.data)
      );
      if (!fk) {
        return res.status(400).json({
          error: `no forward FK from ${entityName} to ${parent.entity}`,
        });
      }
      rows = rows.filter(
        (r) => String(r[fk.name] ?? "").trim() === String(parent.pkValue ?? "").trim()
      );
      scopedBy = { entity: parent.entity, id: parent.id, viaFk: fk.name };
    }

    const totalCount = rows.length;
    const pageRows = rows.slice(page * pageSize, (page + 1) * pageSize);

    return res.json({
      kind: "list",
      entity: entityName,
      scope,                       // ancestors
      scopedBy,                    // explicit "filtered by parent X via FK Y"
      schema: entity.schema,
      rows: pageRows,
      totalCount,
      page,
      pageSize,
    });
  } catch (e) {
    res.status(e.statusCode || 500).json({ error: String(e.message || e) });
  }
});

// -----------------------------------------------------------------------------
// Explorer mutations — PATCH / POST / DELETE on instance rows.
// -----------------------------------------------------------------------------
// All three reuse the existing writeThrough helper (Postgres + rulebook JSON
// in one transaction). Per §2.8 decision 4 there is NO locking layer on top
// of writeThrough — the effortless CLI handles its own locking; adding more
// here masks the real conflicts CLAUDE.md "No locks, no caches, no fallbacks"
// warns about.
//
// Two Postgres targets per write:
//   1. portal_rulebook_entities (always exists — coarse JSONB mirror of the
//      whole entity's data array). This is the existing portal pattern and
//      is what guarantees the editor DB reflects the edit immediately.
//   2. The actual entity table (employees, projects, …) — only if it exists
//      in the editor DB. The table might not exist if the user hasn't run
//      `effortless build` yet for this domain; in that case we log a hint
//      and continue (the rulebook JSON is still updated, and the next
//      rebuild will populate the table from the JSON data array).

// PascalCase → snake_case. ACME's transpiler produces this from entity and
// field names, and we follow the same mapping to address its tables.
function toSnakeCase(s) {
  if (!s) return s;
  return String(s)
    .replace(/([a-z0-9])([A-Z])/g, "$1_$2")
    .replace(/([A-Z])([A-Z][a-z])/g, "$1_$2")
    .toLowerCase();
}

async function entityTableExists(client, tableName) {
  const r = await client.query(
    `SELECT 1 FROM information_schema.tables
      WHERE table_schema = 'public' AND table_name = $1`,
    [tableName]
  );
  return r.rowCount > 0;
}

// Returns the schema field-defs that PATCH/POST may write. Calculated,
// lookup, aggregation, and inverse-relationship fields are read-only by
// construction — silently filtering them is wrong (the caller probably has
// a bug), so we instead reject the request loudly.
function classifyFieldsForWrite(entity, patch) {
  const schemaByName = Object.fromEntries((entity.schema || []).map((f) => [f.name, f]));
  const accepted = {};
  const rejected = [];
  const unknown = [];
  for (const [field, value] of Object.entries(patch || {})) {
    const f = schemaByName[field];
    if (!f) { unknown.push(field); continue; }
    if (f.type === "calculated" || f.type === "lookup" || f.type === "aggregation") {
      rejected.push({ field, reason: `${f.type} fields are derived, not writable` });
      continue;
    }
    if (f.type === "relationship" && isInverseRelationshipField(f, entity.data)) {
      rejected.push({ field, reason: "inverse-side relationship; edit the FK side instead" });
      continue;
    }
    accepted[field] = value;
  }
  return { accepted, rejected, unknown };
}

// PATCH /api/explorer/instance/:entity/:id
// Body: { fieldName: newValue, … } — raw + forward-FK fields only.
app.patch("/api/explorer/instance/:entity/:id", requireEditor, async (req, res) => {
  const entityName = req.params.entity;
  const id = req.params.id;
  const patch = req.body || {};
  try {
    const rb = loadRulebook();
    const entity = rb[entityName];
    if (!entity || !Array.isArray(entity.schema)) {
      return res.status(404).json({ error: `entity '${entityName}' not found` });
    }
    const rowIdx = (entity.data || []).findIndex((r) => String(r.Name) === String(id));
    if (rowIdx < 0) {
      return res.status(404).json({ error: `${entityName} with Name='${id}' not found` });
    }
    const { accepted, rejected, unknown } = classifyFieldsForWrite(entity, patch);
    if (unknown.length || rejected.length) {
      return res.status(400).json({
        error: "patch contains non-writable fields",
        unknown,
        rejected,
      });
    }
    if (Object.keys(accepted).length === 0) {
      return res.status(400).json({ error: "patch is empty" });
    }

    const pk = findPkField(entity);
    const pkValue = entity.data[rowIdx][pk];
    const tableName = toSnakeCase(entityName);

    const newRb = await writeThrough({
      pgWrite: async (c) => {
        // (1) actual entity table — best-effort; skip if not bootstrapped.
        if (await entityTableExists(c, tableName)) {
          const cols = Object.keys(accepted);
          const setClauses = cols.map((col, i) => `${toSnakeCase(col)} = $${i + 1}`);
          const values = cols.map((col) => accepted[col]);
          values.push(pkValue);
          await c.query(
            `UPDATE ${tableName} SET ${setClauses.join(", ")}
              WHERE ${toSnakeCase(pk)} = $${cols.length + 1}`,
            values
          );
        } else {
          console.warn(`[explorer] table '${tableName}' not in editor DB; rulebook JSON updated only`);
        }
        // (2) portal mirror — always update so subsequent reads see the edit.
        const newData = entity.data.slice();
        newData[rowIdx] = { ...newData[rowIdx], ...accepted };
        await c.query(
          `UPDATE portal_rulebook_entities SET data_json = $1, updated_at = now()
            WHERE entity_name = $2`,
          [JSON.stringify(newData), entityName]
        );
        await c.query(
          `INSERT INTO portal_audit_log (user_id, action, target, payload)
           VALUES ($1, 'instance.patch', $2, $3)`,
          [getCurrentUser()?.UserId || null, `${entityName}/${id}`, JSON.stringify(accepted)]
        );
      },
      rulebookMutate: (rb2) => {
        // Re-find the row inside rb2 (the freshly-loaded rulebook) rather
        // than trusting the rowIdx captured at handler entry — protects
        // against the rulebook changing under us between request and commit.
        const ent = rb2[entityName];
        if (!ent || !Array.isArray(ent.data)) {
          throw new Error(`entity '${entityName}' missing from current rulebook`);
        }
        const idx = ent.data.findIndex((r) => String(r.Name) === String(id));
        if (idx < 0) {
          throw new Error(`${entityName} with Name='${id}' no longer present`);
        }
        ent.data[idx] = { ...ent.data[idx], ...accepted };
        return rb2;
      },
    });
    const idxFresh = newRb[entityName].data.findIndex((r) => String(r.Name) === String(id));
    res.json({ ok: true, row: newRb[entityName].data[idxFresh] });
  } catch (e) {
    res.status(e.statusCode || 500).json({ error: String(e.message || e) });
  }
});

// POST /api/explorer/instance/:entity
// Body: full row payload (raw + forward-FK fields only).
app.post("/api/explorer/instance/:entity", requireEditor, async (req, res) => {
  const entityName = req.params.entity;
  const body = req.body || {};
  try {
    const rb = loadRulebook();
    const entity = rb[entityName];
    if (!entity || !Array.isArray(entity.schema)) {
      return res.status(404).json({ error: `entity '${entityName}' not found` });
    }
    const { accepted, rejected, unknown } = classifyFieldsForWrite(entity, body);
    if (unknown.length || rejected.length) {
      return res.status(400).json({ error: "row contains non-writable fields", unknown, rejected });
    }
    if (!accepted.Name) {
      return res.status(400).json({ error: "Name field is required (used as the URL id per §2.6)" });
    }
    if ((entity.data || []).some((r) => String(r.Name) === String(accepted.Name))) {
      return res.status(409).json({ error: `${entityName} with Name='${accepted.Name}' already exists` });
    }
    const pk = findPkField(entity);
    if (!accepted[pk]) {
      return res.status(400).json({ error: `${pk} (PK) is required` });
    }
    const tableName = toSnakeCase(entityName);

    const newRb = await writeThrough({
      pgWrite: async (c) => {
        if (await entityTableExists(c, tableName)) {
          const cols = Object.keys(accepted);
          const colList = cols.map(toSnakeCase).join(", ");
          const placeholders = cols.map((_, i) => `$${i + 1}`).join(", ");
          const values = cols.map((col) => accepted[col]);
          await c.query(
            `INSERT INTO ${tableName} (${colList}) VALUES (${placeholders})`,
            values
          );
        } else {
          console.warn(`[explorer] table '${tableName}' not in editor DB; rulebook JSON updated only`);
        }
        const newData = [...(entity.data || []), accepted];
        await c.query(
          `UPDATE portal_rulebook_entities SET data_json = $1, updated_at = now()
            WHERE entity_name = $2`,
          [JSON.stringify(newData), entityName]
        );
        await c.query(
          `INSERT INTO portal_audit_log (user_id, action, target, payload)
           VALUES ($1, 'instance.create', $2, $3)`,
          [getCurrentUser()?.UserId || null, `${entityName}/${accepted.Name}`, JSON.stringify(accepted)]
        );
      },
      rulebookMutate: (rb2) => {
        const ent = rb2[entityName];
        if (!ent || !Array.isArray(ent.data)) {
          throw new Error(`entity '${entityName}' missing from current rulebook`);
        }
        // Re-check uniqueness against the fresh rulebook in case another
        // request inserted the same Name between handler entry and commit.
        if (ent.data.some((r) => String(r.Name) === String(accepted.Name))) {
          throw new Error(`${entityName} with Name='${accepted.Name}' already exists`);
        }
        ent.data = [...ent.data, accepted];
        return rb2;
      },
    });
    res.status(201).json({ ok: true, row: accepted });
  } catch (e) {
    res.status(e.statusCode || 500).json({ error: String(e.message || e) });
  }
});

// DELETE /api/explorer/instance/:entity/:id?cascade=<bool>
// With cascade=false (default): if any FK referrers exist, return 409 with
// the referrer list. With cascade=true: walk forward FKs from other entities
// and delete referring rows first. Recursion depth is bounded to prevent
// infinite loops on cyclic schemas.
function findReferrers(rb, entityName, pkValue) {
  const refs = [];
  for (const [otherName, otherValue] of iterEntities(rb)) {
    if (otherName === entityName) continue;
    for (const f of otherValue.schema || []) {
      if (f.type !== "relationship" || f.RelatedTo !== entityName) continue;
      if (isInverseRelationshipField(f, otherValue.data)) continue;
      for (const r of otherValue.data || []) {
        if (String(r[f.name] ?? "").trim() === String(pkValue ?? "").trim()) {
          refs.push({ entity: otherName, fkField: f.name, name: r.Name, pk: r[findPkField(otherValue)] });
        }
      }
    }
  }
  return refs;
}

app.delete("/api/explorer/instance/:entity/:id", requireEditor, async (req, res) => {
  const entityName = req.params.entity;
  const id = req.params.id;
  const cascade = String(req.query.cascade || "").toLowerCase() === "true";
  const MAX_CASCADE_DEPTH = 8;
  try {
    const rb = loadRulebook();
    const entity = rb[entityName];
    if (!entity || !Array.isArray(entity.schema)) {
      return res.status(404).json({ error: `entity '${entityName}' not found` });
    }
    const rowIdx = (entity.data || []).findIndex((r) => String(r.Name) === String(id));
    if (rowIdx < 0) {
      return res.status(404).json({ error: `${entityName} with Name='${id}' not found` });
    }
    const row = entity.data[rowIdx];
    const pk = findPkField(entity);
    const pkValue = row[pk];

    const refs = findReferrers(rb, entityName, pkValue);
    if (refs.length > 0 && !cascade) {
      return res.status(409).json({
        error: `${entityName}/${id} has ${refs.length} FK referrer(s); pass ?cascade=true to cascade`,
        referrers: refs,
      });
    }

    // Build the deletion plan in dependency order (referrers first), bounded.
    // Same logic as findReferrers but accumulating across depth.
    const deletions = []; // { entity, pk, pkValue, rowIdx }
    function plan(targetEntity, targetPkValue, depth) {
      if (depth > MAX_CASCADE_DEPTH) {
        throw new Error(`cascade depth exceeded ${MAX_CASCADE_DEPTH} — cycle in FK graph?`);
      }
      const subRefs = findReferrers(rb, targetEntity, targetPkValue);
      for (const r of subRefs) plan(r.entity, r.pk, depth + 1);
      const ent = rb[targetEntity];
      const idx = (ent.data || []).findIndex(
        (x) => String(x[findPkField(ent)] ?? "").trim() === String(targetPkValue ?? "").trim()
      );
      if (idx < 0) return;
      if (deletions.some((d) => d.entity === targetEntity && d.rowIdx === idx)) return;
      deletions.push({
        entity: targetEntity,
        table: toSnakeCase(targetEntity),
        pk: findPkField(ent),
        pkValue: targetPkValue,
        rowIdx: idx,
      });
    }
    plan(entityName, pkValue, 0);

    const newRb = await writeThrough({
      pgWrite: async (c) => {
        for (const d of deletions) {
          if (await entityTableExists(c, d.table)) {
            await c.query(
              `DELETE FROM ${d.table} WHERE ${toSnakeCase(d.pk)} = $1`,
              [d.pkValue]
            );
          }
          // refresh portal mirror by reading the post-delete data array from
          // the rulebook mutation — done after rulebookMutate runs. We
          // re-load below.
        }
        await c.query(
          `INSERT INTO portal_audit_log (user_id, action, target, payload)
           VALUES ($1, $2, $3, $4)`,
          [
            getCurrentUser()?.UserId || null,
            cascade ? "instance.delete.cascade" : "instance.delete",
            `${entityName}/${id}`,
            JSON.stringify({ deletions: deletions.map(({ entity, pkValue }) => ({ entity, pkValue })) }),
          ]
        );
      },
      rulebookMutate: (rb2) => {
        // Look up each row fresh in rb2 (by PK) rather than reusing the
        // rowIdx captured at handler entry — keeps the splice safe if the
        // rulebook changed under us.
        for (const d of deletions) {
          const ent = rb2[d.entity];
          if (!ent || !Array.isArray(ent.data)) continue;
          const idx = ent.data.findIndex(
            (x) => String(x[d.pk] ?? "").trim() === String(d.pkValue ?? "").trim()
          );
          if (idx >= 0) ent.data.splice(idx, 1);
        }
        return rb2;
      },
    });

    // Sync the portal_rulebook_entities mirrors for every touched entity.
    // Doing it post-write to keep the transaction simple.
    try {
      const p = await getPool();
      for (const ent of new Set(deletions.map((d) => d.entity))) {
        await p.query(
          `UPDATE portal_rulebook_entities SET data_json = $1, updated_at = now()
            WHERE entity_name = $2`,
          [JSON.stringify(newRb[ent].data || []), ent]
        );
      }
    } catch (e) {
      console.warn(`[explorer] portal mirror sync after delete failed: ${e.message}`);
    }

    res.json({ ok: true, deleted: deletions.map(({ entity, pkValue }) => ({ entity, pkValue })) });
  } catch (e) {
    res.status(e.statusCode || 500).json({ error: String(e.message || e) });
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
