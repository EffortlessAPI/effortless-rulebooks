/***********************
 * EffortlessAPI Commit Publisher (POC)
 * https://my.effortlessapi.com
 * hello@effortlessapi.com
 ***********************/

// ------------------------------
// Constants
// ------------------------------
const BRIDGE_BASE_URL = "https://airtable-bridge-cmvbd4phczmeg.aws-us-east-2.controlplane.us";
const EFFORTLESS_BASE_URL = "https://bases.effortlessapi.com";

const TABLE_ERB_VERSIONS = "ERBVersions";
const TABLE_ERB_CUSTOMIZATIONS = "ERBCustomizations";

// ERBVersions fields
const FIELD_VERSION_NAME = "Name";
const FIELD_COMMIT_MESSAGE = "Message";
const FIELD_NOTES = "Notes";
const FIELD_COMMIT_DATE = "CommitDate";
const FIELD_IS_PUBLISHED = "IsPublished";

// ERBCustomizations fields
const FIELD_CUST_NAME = "Name";
const FIELD_CUST_TITLE = "Title";
const FIELD_CUST_SQL_CODE = "SQLCode";
const FIELD_CUST_SQL_TARGET = "SQLTarget"; // single select (Postgres)
const FIELD_CUST_TYPE = "CustomizationType"; // single select [Schema, Functions, Views, RLS, Data]

// Polling
const POLL_INTERVAL_MS = 5000;
const POLL_TIMEOUT_MS = 60 * 1000;
// Timeout for synchronous build requests to Effortless Base
const BUILD_TIMEOUT_MS = 5 * 60 * 1000; // 5 minutes

// Error extraction / display
const ERROR_CONTEXT_LINES = 3;      // lines before/after match
const MAX_ERROR_SNIPPETS = 5;       // avoid dumping tons of snippets
const LOG_PAGE_CHARS = 20000;       // paging size for full logs

// Match rules (expand anytime)
const ERROR_LINE_MATCHERS = [
  /(^|\s)ERROR:/,          // your main rule
  /(^|\s)FATAL:/,
  /\bException\b/,
  /\bTraceback\b/,
];

// ------------------------------
// Default customization templates
// ------------------------------
const DEFAULT_CUSTOMIZATIONS = [
  {
    name: "03a-customize-schema.sql",
    title: "Customize Schema",
    type: "Schema",
    sqlCode: `-- ============================================================================
-- CUSTOMIZE SCHEMA - User-defined tables and schema modifications
-- ============================================================================
-- This file is for YOUR custom schema changes that should persist across
-- regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional tables not defined in the rulebook
--   - Extra columns on existing tables (ALTER TABLE)
--   - Custom indexes for performance tuning
--   - Custom constraints or triggers
--
-- IMPORTANT:
--   - This file runs AFTER 01-drop-and-create-tables.sql
--   - The base tables already exist when this runs
--   - This file will NOT be overwritten by ERB regeneration
--
-- EXAMPLES:
--
--   -- Add a custom table
--   CREATE TABLE IF NOT EXISTS audit_log (
--       audit_log_id    TEXT PRIMARY KEY DEFAULT gen_random_uuid()::TEXT,
--       table_name      TEXT NOT NULL,
--       record_id       TEXT NOT NULL,
--       action          TEXT NOT NULL,
--       changed_at      TIMESTAMPTZ DEFAULT NOW()
--   );
--
--   -- Add a column to an existing table
--   ALTER TABLE erb_versions ADD COLUMN IF NOT EXISTS custom_field TEXT;
--
--   -- Add an index for performance
--   CREATE INDEX IF NOT EXISTS idx_custom ON some_table(some_column);
--
-- ============================================================================

-- Your custom schema changes go here:
`,
  },
  {
    name: "03b-customize-functions.sql",
    title: "Customize Functions",
    type: "Functions",
    sqlCode: `-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined calculation functions
-- ============================================================================
-- This file is for YOUR custom PostgreSQL functions that should persist
-- across regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional calc_* functions for computed fields
--   - Helper functions for complex business logic
--   - Utility functions for data transformation
--   - Trigger functions for custom automation
--
-- IMPORTANT:
--   - This file runs AFTER 02-create-functions.sql
--   - Base ERB calc functions already exist when this runs
--   - This file will NOT be overwritten by ERB regeneration
--   - Functions defined here can be used in 03b-customize-views.sql
--
-- ERB FUNCTION NAMING CONVENTIONS:
--   - calc_tablename_fieldname() - Returns a computed value for a view field
--   - Helper functions can use any naming convention
--
-- EXAMPLES:
--
--   -- Custom calculation function for a view field
--   CREATE OR REPLACE FUNCTION calc_orders_total_with_tax(p_order_id TEXT)
--   RETURNS NUMERIC AS $$
--   BEGIN
--       RETURN (SELECT subtotal * 1.08 FROM orders WHERE order_id = p_order_id);
--   END;
--   $$ LANGUAGE plpgsql STABLE;
--
--   -- Utility function
--   CREATE OR REPLACE FUNCTION format_currency(p_amount NUMERIC)
--   RETURNS TEXT AS $$
--   BEGIN
--       RETURN '$' || TO_CHAR(p_amount, 'FM999,999,990.00');
--   END;
--   $$ LANGUAGE plpgsql IMMUTABLE;
--
-- ============================================================================

-- Your custom functions go here:
`,
  },
  {
    name: "03c-customize-views.sql",
    title: "Customize Views",
    type: "Views",
    sqlCode: `-- ============================================================================
-- CUSTOMIZE VIEWS - User-defined views and view extensions
-- ============================================================================
-- This file is for YOUR custom views that should persist across
-- regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional views not defined in the rulebook
--   - Specialized views for reporting or dashboards
--   - Views that combine data from multiple base views
--   - Materialized views for performance optimization
--
-- IMPORTANT:
--   - This file runs AFTER 03-create-views.sql
--   - All base vw_* views already exist when this runs
--   - This file will NOT be overwritten by ERB regeneration
--   - If you need to EXTEND a base view, create a new view that selects from it
--
-- ERB VIEW PATTERN:
--   - Base views (vw_*) contain all denormalized data + calculated fields
--   - READ from views, WRITE to base tables
--   - Never JOIN in application code - extend views instead
--
-- EXAMPLES:
--
--   -- Custom reporting view
--   CREATE OR REPLACE VIEW vw_monthly_summary AS
--   SELECT
--       DATE_TRUNC('month', created_at) AS month,
--       COUNT(*) AS record_count,
--       SUM(amount) AS total_amount
--   FROM vw_orders
--   GROUP BY DATE_TRUNC('month', created_at);
--
--   -- Extended view with additional computed fields
--   CREATE OR REPLACE VIEW vw_orders_extended AS
--   SELECT
--       o.*,
--       calc_orders_total_with_tax(o.order_id) AS total_with_tax
--   FROM vw_orders o;
--
--   -- Materialized view for heavy queries
--   CREATE MATERIALIZED VIEW IF NOT EXISTS mv_dashboard_stats AS
--   SELECT ... FROM vw_orders ...;
--
-- ============================================================================

-- Your custom views go here:
`,
  },
  {
    name: "03d-customize-rls.sql",
    title: "Customize Policies (RLS)",
    type: "RLS",
    sqlCode: `-- ============================================================================
-- CUSTOMIZE POLICIES - User-defined Row Level Security policies
-- ============================================================================
-- This file is for YOUR custom RLS policies that should persist across
-- regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional RLS policies for custom tables
--   - Extended security rules beyond base policies
--   - Role-specific access controls
--   - Custom permission functions
--
-- IMPORTANT:
--   - This file runs AFTER 04-create-policies.sql
--   - Base RLS policies already exist when this runs
--   - This file will NOT be overwritten by ERB regeneration
--   - RLS must be enabled on tables before policies apply
--
-- ROW LEVEL SECURITY BASICS:
--   1. Enable RLS on the table: ALTER TABLE t ENABLE ROW LEVEL SECURITY;
--   2. Create policies for SELECT, INSERT, UPDATE, DELETE
--   3. Policies use current_user or session variables for context
--
-- EXAMPLES:
--
--   -- Enable RLS on a custom table
--   ALTER TABLE audit_log ENABLE ROW LEVEL SECURITY;
--
--   -- Policy: Users can only see their own audit entries
--   CREATE POLICY audit_log_select ON audit_log
--       FOR SELECT
--       USING (user_id = current_setting('app.current_user_id', true));
--
--   -- Policy: Only admins can delete
--   CREATE POLICY audit_log_delete ON audit_log
--       FOR DELETE
--       USING (current_setting('app.is_admin', true)::BOOLEAN = true);
--
--   -- Policy: Allow insert for authenticated users
--   CREATE POLICY audit_log_insert ON audit_log
--       FOR INSERT
--       WITH CHECK (current_setting('app.current_user_id', true) IS NOT NULL);
--
-- ============================================================================

-- Your custom policies go here:
`,
  },
  {
    name: "03e-customize-data.sql",
    title: "Customize Data (Seeds / Migrations)",
    type: "Data",
    sqlCode: `-- ============================================================================
-- CUSTOMIZE DATA - User-defined seed data and data migrations
-- ============================================================================
-- This file is for YOUR custom seed data that should persist across
-- regeneration of the base ERB files.
--
-- USE THIS FILE FOR:
--   - Additional seed data for custom tables
--   - Environment-specific configuration data
--   - Test data for development
--   - Data migrations and transformations
--
-- IMPORTANT:
--   - This file runs AFTER 05-insert-data.sql
--   - Base seed data already exists when this runs
--   - This file will NOT be overwritten by ERB regeneration
--   - Use INSERT ... ON CONFLICT for idempotent inserts
--
-- IDEMPOTENT INSERTS:
--   Use ON CONFLICT to make scripts safe to run multiple times:
--
--   INSERT INTO table (id, field) VALUES ('x', 'y')
--   ON CONFLICT (id) DO UPDATE SET field = EXCLUDED.field;
--
--   Or to skip if exists:
--   INSERT INTO table (id, field) VALUES ('x', 'y')
--   ON CONFLICT (id) DO NOTHING;
--
-- EXAMPLES:
--
--   -- Add custom configuration
--   INSERT INTO settings (setting_id, key, value) VALUES
--       ('cfg-1', 'feature_flag_x', 'enabled'),
--       ('cfg-2', 'max_retries', '3')
--   ON CONFLICT (setting_id) DO UPDATE SET value = EXCLUDED.value;
--
--   -- Add test users for development
--   INSERT INTO users (user_id, email, role) VALUES
--       ('test-admin', 'admin@test.local', 'admin'),
--       ('test-user', 'user@test.local', 'user')
--   ON CONFLICT (user_id) DO NOTHING;
--
--   -- Data migration example
--   UPDATE orders SET status = 'archived'
--   WHERE created_at < NOW() - INTERVAL '1 year';
--
-- ============================================================================

-- Your custom data inserts go here:
`,
  },
];

// ------------------------------
// Helpers
// ------------------------------
function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

// Wrapper around remoteFetchAsync that rejects after `ms` milliseconds.
// Note: remoteFetchAsync cannot be aborted in this environment, so the
// underlying request may still complete; this wrapper simply limits how
// long we wait before giving up in the script.
async function fetchWithTimeout(url, options = {}, ms = BUILD_TIMEOUT_MS) {
  const timeoutPromise = new Promise((_, reject) =>
    setTimeout(() => reject(new Error(`Fetch timeout after ${ms}ms`)), ms)
  );

  const fetchPromise = (async () => {
    return await remoteFetchAsync(url, options);
  })();

  return Promise.race([fetchPromise, timeoutPromise]);
}

function normalizeBaseUrl(url) {
  return (url || "").trim().replace(/\/+$/, "");
}

function truncate(str, max = 4000) {
  if (!str) return "";
  return str.length > max ? str.slice(0, max) + "\n…(truncated)" : str;
}

/**
 * Base64 encode a string for safe JSON/shell transport.
 * This avoids issues with newlines and special characters in SQL code.
 */
function base64Encode(str) {
  if (!str) return "";
  // Use TextEncoder to handle Unicode properly, then base64 encode
  const bytes = new TextEncoder().encode(str);
  let binary = "";
  for (let i = 0; i < bytes.length; i++) {
    binary += String.fromCharCode(bytes[i]);
  }
  return btoa(binary);
}

function pickErrorText(obj) {
  if (!obj) return null;
  if (typeof obj === "string") return obj;

  const candidates = [
    obj.error,
    obj.message,
    obj.stderr,
    obj.stdout,
    obj.stack,
    obj.logs,
    obj.output,
    obj.details,
    obj.reason,
  ];

  for (const c of candidates) {
    if (!c) continue;
    if (typeof c === "string") return c;
    try {
      return JSON.stringify(c, null, 2);
    } catch {}
  }

  if (obj.payload) return pickErrorText(obj.payload);

  try {
    return JSON.stringify(obj, null, 2);
  } catch {
    return String(obj);
  }
}

function getTableIfExists(name) {
  return base.tables.find((t) => t.name === name) || null;
}

async function safeReadText(res) {
  try {
    return await res.text();
  } catch (e) {
    return `<<failed to read body: ${String(e)}>>`;
  }
}

async function safeReadJsonFromText(text) {
  try {
    return { ok: true, json: JSON.parse(text) };
  } catch (e) {
    return { ok: false, error: e };
  }
}

async function createERBVersionsTable() {
  await base.createTableAsync(TABLE_ERB_VERSIONS, [
    { name: FIELD_VERSION_NAME, type: "singleLineText" },
    { name: FIELD_COMMIT_MESSAGE, type: "singleLineText" },
    { name: FIELD_NOTES, type: "multilineText" },
    {
      name: FIELD_COMMIT_DATE,
      type: "date",
      options: { dateFormat: { name: "iso" } },
    },
    {
      name: FIELD_IS_PUBLISHED,
      type: "checkbox",
      options: { icon: "check", color: "greenBright" },
    },
  ]);
  return base.getTable(TABLE_ERB_VERSIONS);
}

async function createERBCustomizationsTable() {
  await base.createTableAsync(TABLE_ERB_CUSTOMIZATIONS, [
    { name: FIELD_CUST_NAME, type: "singleLineText" },
    { name: FIELD_CUST_TITLE, type: "singleLineText" },
    { name: FIELD_CUST_SQL_CODE, type: "multilineText" },
    {
      name: FIELD_CUST_SQL_TARGET,
      type: "singleSelect",
      options: { choices: [{ name: "Postgres" }] },
    },
    {
      name: FIELD_CUST_TYPE,
      type: "singleSelect",
      options: {
        choices: [
          { name: "Schema" },
          { name: "Functions" },
          { name: "Views" },
          { name: "RLS" },
          { name: "Data" },
        ],
      },
    },
  ]);
  return base.getTable(TABLE_ERB_CUSTOMIZATIONS);
}

async function seedDefaultCustomizationsIfEmpty(customizationsTable) {
  if (!customizationsTable) return;

  const q = await customizationsTable.selectRecordsAsync({
    fields: [FIELD_CUST_NAME],
  });
  if (q.records.length > 0) return; // don't duplicate

  await customizationsTable.createRecordsAsync(
    DEFAULT_CUSTOMIZATIONS.map((d) => ({
      fields: {
        [FIELD_CUST_NAME]: d.name,
        [FIELD_CUST_TITLE]: d.title,
        [FIELD_CUST_SQL_CODE]: d.sqlCode,
        [FIELD_CUST_SQL_TARGET]: { name: "Postgres" },
        [FIELD_CUST_TYPE]: { name: d.type },
      },
    }))
  );
}

async function ensureTableWithPrompt({ name, createFn, required }) {
  const existing = getTableIfExists(name);
  if (existing) return existing;

  const choice = await input.buttonsAsync(
    `Table "${name}" does not exist. Create it now?`,
    ["Create", "Skip"]
  );

  if (choice !== "Create") {
    if (required) output.text(`❌ "${name}" is required to proceed.`);
    return null;
  }

  output.markdown(`⚙️ Creating **${name}** table…`);
  const created = await createFn();
  output.markdown(`✅ **${name}** table created.`);
  return created;
}

async function loadCustomizations(customizationsTable) {
  if (!customizationsTable) return [];

  const q = await customizationsTable.selectRecordsAsync();
  return q.records.map((r) => ({
    name: r.getCellValueAsString(FIELD_CUST_NAME),
    title: r.getCellValueAsString(FIELD_CUST_TITLE),
    // Base64 encode sqlCode to avoid JSON/shell escaping issues with newlines
    sqlCode: base64Encode(r.getCellValueAsString(FIELD_CUST_SQL_CODE)),
    sqlCodeEncoding: "base64",  // Signal to receiver that sqlCode is base64 encoded
    sqlTarget: r.getCellValueAsString(FIELD_CUST_SQL_TARGET) || "Postgres",
    customizationType: r.getCellValueAsString(FIELD_CUST_TYPE),
    recordId: r.id,
  }));
}

// ------------------------------
// Improved polling with better error visibility
// ------------------------------
async function pollForResult(resultUrl, { debug = false } = {}) {
  const startedAt = Date.now();
  let attempt = 0;
  let lastSnapshot = null;

  while (Date.now() - startedAt < POLL_TIMEOUT_MS) {
    attempt += 1;

    let res;
    try {
      res = await remoteFetchAsync(resultUrl, { method: "GET" });
    } catch (e) {
      lastSnapshot = { type: "network", error: String(e) };
      if (debug) {
        output.markdown(mdCodeBlock(`⚠️ Poll attempt ${attempt}: network error\n${String(e)}`));
      } else {
        output.text(`⏳ Waiting for build result… (${attempt * (POLL_INTERVAL_MS / 1000)}s)`);
      }
      await sleep(POLL_INTERVAL_MS);
      continue;
    }

    const rawText = await safeReadText(res);

    if (!res.ok) {
      lastSnapshot = {
        type: "http",
        status: res.status,
        statusText: res.statusText,
        body: rawText,
      };
      if (debug) {
        output.markdown(
          `⚠️ Poll attempt ${attempt}: HTTP ${res.status} ${res.statusText}\n${mdCodeBlock(truncate(rawText))}`
        );
      } else {
        output.text(`⏳ Waiting for build result… (${attempt * (POLL_INTERVAL_MS / 1000)}s)`);
      }
      await sleep(POLL_INTERVAL_MS);
      continue;
    }

    const parsed = await safeReadJsonFromText(rawText);
    if (!parsed.ok) {
      lastSnapshot = { type: "parse", error: String(parsed.error), body: rawText };
      if (debug) {
        output.markdown(
          `⚠️ Poll attempt ${attempt}: failed to parse JSON\n${mdCodeBlock(String(parsed.error))}\nRaw:\n${mdCodeBlock(truncate(rawText))}`
        );
      } else {
        output.text(`⏳ Waiting for build result… (${attempt * (POLL_INTERVAL_MS / 1000)}s)`);
      }
      await sleep(POLL_INTERVAL_MS);
      continue;
    }

    const data = parsed.json;

    if (data?.ready) {
      return { timedOut: false, raw: data, payload: data.payload || {}, lastSnapshot };
    }

    lastSnapshot = { type: "not_ready", raw: data };
    output.text(`⏳ Waiting for build result… (${attempt * (POLL_INTERVAL_MS / 1000)}s)`);
    await sleep(POLL_INTERVAL_MS);
  }

  return { timedOut: true, lastSnapshot };
}

function splitLinesPreserve(text) {
  return String(text || "").replace(/\r\n/g, "\n").split("\n");
}

function lineMatchesError(line) {
  return ERROR_LINE_MATCHERS.some((re) => re.test(line));
}

function mergeWindows(windows) {
  if (!windows.length) return [];
  windows.sort((a, b) => a.start - b.start);
  const merged = [windows[0]];
  for (let i = 1; i < windows.length; i++) {
    const prev = merged[merged.length - 1];
    const cur = windows[i];
    if (cur.start <= prev.end + 1) {
      prev.end = Math.max(prev.end, cur.end);
    } else {
      merged.push(cur);
    }
  }
  return merged;
}

function extractErrorWindows(logText, { contextLines = ERROR_CONTEXT_LINES } = {}) {
  const lines = splitLinesPreserve(logText);
  const windows = [];

  for (let i = 0; i < lines.length; i++) {
    if (!lineMatchesError(lines[i])) continue;
    const start = Math.max(0, i - contextLines);
    const end = Math.min(lines.length - 1, i + contextLines);
    windows.push({ start, end, hit: i });
  }

  const merged = mergeWindows(windows);

  // Cap number of snippets shown
  return merged.slice(0, MAX_ERROR_SNIPPETS).map((w) => ({
    ...w,
    lines,
  }));
}

function formatWindow(win) {
  const { start, end, hit, lines } = win;

  // Add line numbers + a pointer line for the hit
  const out = [];
  for (let i = start; i <= end; i++) {
    const n = String(i + 1).padStart(4, " ");
    const prefix = i === hit ? ">>" : "  ";
    out.push(`${prefix} ${n} | ${lines[i]}`);
  }
  return out.join("\n");
}

function getBuildOutput(payloadResult, poll) {
  return (
    payloadResult?.output ||
    payloadResult?.stdout ||
    payloadResult?.logs ||
    pickErrorText(payloadResult) ||
    pickErrorText(poll?.raw) ||
    ""
  );
}

// IMPORTANT: any fenced block must be in a SINGLE output.markdown() call
function mdCodeBlock(body, lang = "") {
  const text = String(body ?? "");
  let fence = "~~~~";
  while (text.includes(fence)) fence += "~";
  const langSuffix = lang ? lang : "";
  return `${fence}${langSuffix}\n${text}\n${fence}`;
}

function extractPrimaryErrorBlocks(logText) {
  const lines = splitLinesPreserve(logText);

  const blocks = [];
  for (let i = 0; i < lines.length; i++) {
    if (!/(^|\s)ERROR:/.test(lines[i])) continue;

    const start = Math.max(0, i - 1);
    let end = i;

    const maxFollow = 8;
    for (let k = 1; k <= maxFollow && i + k < lines.length; k++) {
      const next = lines[i + k];
      if (!next.trim()) break;

      if (
        /^(LINE\s+\d+:|DETAIL:|HINT:|CONTEXT:|STATEMENT:)/.test(next) ||
        /^\s*\^/.test(next) ||
        /^\s+/.test(next)
      ) {
        end = i + k;
        continue;
      }
      break;
    }

    blocks.push({ start, end });
  }

  return mergeWindows(blocks)
    .slice(0, MAX_ERROR_SNIPPETS)
    .map((b) => ({ ...b, lines }));
}

// Make "error lines" visually pop without relying on any special renderer features
function emphasizeErrorText(text) {
  return String(text || "")
    .split("\n")
    .map((l) => `🟥 ${l}`)
    .join("\n");
}

async function showPagedLog(title, text, { pageChars = LOG_PAGE_CHARS } = {}) {
  const t = String(text || "");
  if (!t.trim()) {
    output.markdown(`### ${title}\n(no output)`);
    return;
  }

  let offset = 0;
  let page = 1;

  while (offset < t.length) {
    const chunk = t.slice(offset, offset + pageChars);
    output.markdown(`### ${title} (page ${page})\n${mdCodeBlock(chunk)}`);

    offset += pageChars;
    page += 1;

    if (offset >= t.length) break;

    const choice = await input.buttonsAsync("More output available:", [
      "Next page",
      "Stop",
    ]);
    if (choice !== "Next page") {
      output.text(`(Stopped early. Remaining ~${t.length - offset} chars)`);
      break;
    }
  }
}

// ------------------------------
// Schema Validation
// ------------------------------

/**
 * Check if a name ends with "Id" (case-insensitive for the pattern)
 */
function endsWithId(name) {
  return /Id$/.test(name);
}

/**
 * Check if a name is plural (ends with 's')
 */
function isPlural(name) {
  return name.endsWith("s") || name.endsWith("S");
}

/**
 * Singularize a name (simple version)
 */
function singularize(name) {
  if (name.endsWith("ies")) {
    return name.slice(0, -3) + "y"; // Categories -> Category
  }
  // Only remove "es" for specific patterns where it's clearly the plural form
  if (name.endsWith("ches") || name.endsWith("shes") ||
      name.endsWith("sses") || name.endsWith("xes") || name.endsWith("zes")) {
    return name.slice(0, -2); // Matches -> Match, Boxes -> Box
  }
  if (name.endsWith("s") && !name.endsWith("ss")) {
    return name.slice(0, -1); // Puzzles -> Puzzle, Hunts -> Hunt
  }
  return name;
}

/**
 * Pluralize a name (simple version)
 */
function pluralize(name) {
  if (name.endsWith("y") && !/[aeiou]y$/i.test(name)) {
    return name.slice(0, -1) + "ies";
  }
  if (name.endsWith("s") || name.endsWith("x") || name.endsWith("ch") || name.endsWith("sh")) {
    return name + "es";
  }
  return name + "s";
}

/**
 * Run all schema validations and return a detailed report
 * NOTE: This scans ALL tables and ALL fields in a single pass,
 * so users see every issue at once without needing to re-run.
 */
function validateSchema() {
  const issues = [];
  const tables = base.tables;

  // Track M2M pairs we've already flagged to avoid duplicate errors
  // (since M2M relationships appear on both sides)
  const flaggedM2MPairs = new Set();

  for (const table of tables) {
    const tableName = table.name;

    // Skip ERB system tables
    if (tableName === TABLE_ERB_VERSIONS || tableName === TABLE_ERB_CUSTOMIZATIONS) {
      continue;
    }

    // ----------------------------------------
    // TABLE-LEVEL CHECKS
    // ----------------------------------------

    const fields = table.fields;

    // Check if the PRIMARY field is "Name"
    // table.primaryFieldId is the reliable way to get the primary field ID
    // Do NOT use fields[0] - that's creation order, not the primary field
    const primaryField = fields.find((f) => f.id === table.primaryFieldId);

    if (primaryField && primaryField.name !== "Name") {
      issues.push({
        type: "first_column",
        severity: "error",
        table: tableName,
        field: primaryField.name,
        suggestion: `Rename to "Name"`,
      });
    }

    // ----------------------------------------
    // FIELD-LEVEL CHECKS (runs for EVERY field)
    // ----------------------------------------
    for (const field of fields) {
      const fieldName = field.name;
      const fieldType = field.type;

      // Note: Fields with spaces are now supported - no validation needed

      // Check for FK fields ending in "Id"
      if (fieldType === "foreignKey" || fieldType === "multipleRecordLinks") {
        if (endsWithId(fieldName)) {
          const suggested = fieldName.slice(0, -2); // Remove "Id"
          issues.push({
            type: "fk_naming",
            severity: "error",
            table: tableName,
            field: fieldName,
            suggestion: `Rename to "${suggested}"`,
          });
        }
      }

      // Check for Many-to-Many relationships
      // Use prefersSingleRecordLink to determine relationship type:
      //   - prefersSingleRecordLink: true = "one" side (can only link to 1 record)
      //   - prefersSingleRecordLink: false/undefined = "many" side (can link to multiple)
      //   - BOTH sides allowing multiple = true Many-to-Many (not supported)
      //
      // We use prefersSingleRecordLink as the primary check, with naming convention as fallback
      // when the option isn't set. This handles cases like "Targeted Keywords for SEO" where
      // the field name ends with an acronym, not 's'.
      if (fieldType === "multipleRecordLinks") {
        const linkedTableId = field.options?.linkedTableId;
        if (linkedTableId) {
          const linkedTable = tables.find((t) => t.id === linkedTableId);
          if (linkedTable) {
            const inverseFieldId = field.options?.inverseLinkFieldId;
            if (inverseFieldId) {
              const inverseField = linkedTable.fields.find((f) => f.id === inverseFieldId);
              if (inverseField) {
                // Determine if each side allows multiple records
                // prefersSingleRecordLink: true = "one" side, false/undefined = "many" side
                const thisSidePrefersSingle = field.options?.prefersSingleRecordLink;
                const inverseSidePrefersSingle = inverseField.options?.prefersSingleRecordLink;

                // A side is "many" if prefersSingleRecordLink is explicitly false,
                // OR if it's undefined and the field name is plural (fallback heuristic)
                const thisSideIsMany =
                  thisSidePrefersSingle === false ||
                  (thisSidePrefersSingle === undefined && isPlural(fieldName));
                const inverseSideIsMany =
                  inverseSidePrefersSingle === false ||
                  (inverseSidePrefersSingle === undefined && isPlural(inverseField.name));

                // If BOTH sides allow multiple records, this is a M2M relationship
                if (thisSideIsMany && inverseSideIsMany) {
                  // Create a canonical key to avoid duplicate errors (sort table names)
                  const pairKey = [tableName, linkedTable.name].sort().join("↔");

                  if (!flaggedM2MPairs.has(pairKey)) {
                    flaggedM2MPairs.add(pairKey);

                    const joinTableName = `${singularize(tableName)}${singularize(linkedTable.name)}`;

                    issues.push({
                      type: "many_to_many",
                      severity: "error",
                      table: tableName,
                      field: fieldName,
                      linkedTable: linkedTable.name,
                      inverseField: inverseField.name,
                      joinTableName: joinTableName,
                      suggestion: `Join table "${joinTableName}"`,
                    });
                  }
                }
              }
            }
          }
        }
      }

      // Note: Lookups to the "many" side of relationships are supported - no validation needed

      // Note: Attachments are now supported - no validation needed

      // Note: User columns (createdBy, lastModifiedBy, collaborators) are now supported - no validation needed
    }
  }

  return issues;
}

/**
 * Format the validation report for display
 */
function formatValidationReport(issues) {
  if (issues.length === 0) {
    return {
      hasErrors: false,
      hasWarnings: false,
      markdown: "### ✅ Schema Validation Passed\n\nNo compatibility issues found. Your schema is ready for EffortlessAPI.",
    };
  }

  const errors = issues.filter((i) => i.severity === "error");
  const warnings = issues.filter((i) => i.severity === "warning");

  let md = `### 📋 Schema Compatibility Report\nFound ${errors.length} error(s) and ${warnings.length} warning(s)\n\n`;

  // Group issues by type for better organization
  const issuesByType = {};
  for (const issue of issues) {
    if (!issuesByType[issue.type]) {
      issuesByType[issue.type] = [];
    }
    issuesByType[issue.type].push(issue);
  }

  // Category headers with their one-time explanation
  const typeInfo = {
    first_column: {
      label: "1️⃣ Primary Field Must Be 'Name'",
      explanation: "ERB requires the primary field to be named \"Name\"",
    },
    fk_naming: {
      label: "🔗 FK Naming",
      explanation: "Link fields should not end with \"Id\"",
    },
    many_to_many: {
      label: "🚫 Many-to-Many Relationships",
      explanation: "M2M links are not supported. Create a join table instead.",
    },
  };

  for (const [type, typeIssues] of Object.entries(issuesByType)) {
    const info = typeInfo[type] || { label: type, explanation: "" };

    md += `#### ${info.label}\n`;
    if (info.explanation) {
      md += `${info.explanation}\n\n`;
    }

    for (const issue of typeIssues) {
      const icon = issue.severity === "error" ? "❌" : "⚠️";
      const location = issue.field
        ? `**${issue.table}.${issue.field}**`
        : `**${issue.table}**`;

      md += `- ${icon} ${location} → ${issue.suggestion}\n`;
    }
    md += "\n";
  }

  // Add note about future support
  if (warnings.length > 0) {
    md += "---\n\n";
    md += "💡 **Note:** Features marked with warnings are in development and will be supported in future versions of EffortlessAPI.\n\n";
  }

  return {
    hasErrors: errors.length > 0,
    hasWarnings: warnings.length > 0,
    errorCount: errors.length,
    warningCount: warnings.length,
    markdown: md,
  };
}

// ------------------------------
// Main
// ------------------------------
async function main() {
  output.clear();

  output.markdown(`**🚀 EffortlessAPI Commit Publisher** · [my.effortlessapi.com](https://my.effortlessapi.com) · hello@effortlessapi.com`);

  // ============================================
  // STEP 1: Schema Validation (FIRST!)
  // ============================================
  const validationIssues = validateSchema();
  const report = formatValidationReport(validationIssues);

  // Only show detailed output if there are issues
  if (report.hasErrors || report.hasWarnings) {
    output.markdown(report.markdown);
  } else {
    output.text("✅ Schema validated");
  }

  // If there are any issues, ask if user wants to continue
  if (report.hasErrors || report.hasWarnings) {
    const issuesSummary = [];
    if (report.errorCount > 0) issuesSummary.push(`${report.errorCount} error(s)`);
    if (report.warningCount > 0) issuesSummary.push(`${report.warningCount} warning(s)`);

    const continueChoice = await input.buttonsAsync(
      `Schema has ${issuesSummary.join(" and ")}. Do you want to continue anyway?`,
      [
        { label: "Yes, Continue", variant: "default" },
        { label: "No, Stop", variant: "danger" },
      ]
    );

    if (continueChoice.label === "No, Stop" || continueChoice === "No, Stop") {
      output.markdown("---\n\n❌ **Commit cancelled.** Please fix the schema issues above and try again.");
      return;
    }

    output.markdown("---\n\n⚠️ Continuing with schema issues...\n");
  }

  // ============================================
  // STEP 2: Ask for Commit Message
  // ============================================
  const rawInput = await input.textAsync("Enter commit message");
  if (!rawInput || !rawInput.trim()) {
    output.text("❌ Commit cancelled — input required.");
    return;
  }
  const commitMessage = rawInput.trim();

  // ============================================
  // STEP 3: Get Config Values
  // ============================================
  const config = input.config({
    title: "EffortlessAPI Commit Publisher",
    description:
      "Creates an ERB version record, posts a build job to the bridge, and waits for the result.",
    items: [
      input.config.text("effortlessApiPat", {
        label: "EffortlessAPI PAT",
        description:
          "Note: Script settings are convenient but not a secure secret store.",
      }),
    ],
  });

  const bridgeBaseUrl = BRIDGE_BASE_URL;
  const effortlessBaseUrl = EFFORTLESS_BASE_URL;
  const pat = (config.effortlessApiPat || "").trim();
  const debug = true;

  if (!pat) {
    output.text("❌ Missing PAT in settings. Please configure your EffortlessAPI PAT in the extension settings.");
    return;
  }

  // ============================================
  // STEP 3.5: Choose Route
  // ============================================
  const routeChoice = await input.buttonsAsync(
    "Choose build route:",
    [
      { label: "Bridge Route", variant: "default" },
      { label: "Effortless Base Route", variant: "default" },
    ]
  );

  const useEffortlessBase = routeChoice.label === "Effortless Base Route" || routeChoice === "Effortless Base Route";

  // Optional: request new credentials from Effortless Base
  let forceCreds = false;
  if (useEffortlessBase) {
    const newCredsChoice = await input.buttonsAsync(
      "Request new credentials?",
      [
        { label: "No", variant: "default" },
        { label: "Yes, New Credentials", variant: "primary" },
      ]
    );
    forceCreds = newCredsChoice.label === "Yes, New Credentials" || newCredsChoice === "Yes, New Credentials";
  }

  // ============================================
  // STEP 4: Ensure ERB Tables
  // ============================================
  // Ensure required table (ERBVersions)
  const erbVersions = await ensureTableWithPrompt({
    name: TABLE_ERB_VERSIONS,
    createFn: createERBVersionsTable,
    required: true,
  });
  if (!erbVersions) return;

  // Ensure optional table (ERBCustomizations)
  const erbCustomizations = await ensureTableWithPrompt({
    name: TABLE_ERB_CUSTOMIZATIONS,
    createFn: createERBCustomizationsTable,
    required: false,
  });

  // Seed defaults if empty
  if (erbCustomizations) {
    await seedDefaultCustomizationsIfEmpty(erbCustomizations);
  }

  // Create ERBVersions record
  const commitDate = new Date();
  const versionName = `v${commitDate.toISOString()}`;

  // Create as NOT published; flip to true only on success
  const erbVersionRecordId = await erbVersions.createRecordAsync({
    [FIELD_VERSION_NAME]: versionName,
    [FIELD_COMMIT_MESSAGE]: commitMessage,
    [FIELD_NOTES]: "",
    [FIELD_COMMIT_DATE]: commitDate,
    [FIELD_IS_PUBLISHED]: false,
  });

  // Build payload
  const customizations = await loadCustomizations(erbCustomizations);
  const runId = `${Date.now().toString(36)}-${Math.random().toString(36).slice(2, 8)}`;

  let payloadResult = {};
  let poll = null;

  if (useEffortlessBase) {
    // ============================================
    // EFFORTLESS BASE ROUTE
    // ============================================
    const buildUrl = `${effortlessBaseUrl}/build?baseId=${base.id}${forceCreds ? "&force_creds=true" : ""}`;
    
    output.markdown(`📤 Sending build request to Effortless Base…\n\n\`${buildUrl}\``);

    let buildResp;
    try {
      buildResp = await fetchWithTimeout(
        buildUrl,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${pat}`,
          },
        },
        BUILD_TIMEOUT_MS
      );
    } catch (e) {
      throw new Error(`Build request timed out after ${BUILD_TIMEOUT_MS}ms: ${String(e)}`);
    }

    if (!buildResp.ok) {
      const buildText = await safeReadText(buildResp);
      throw new Error(`Build request failed: HTTP ${buildResp.status}\n${truncate(buildText)}`);
    }

    const buildText = await safeReadText(buildResp);
    const buildJson = await safeReadJsonFromText(buildText);

    if (!buildJson.ok) {
      throw new Error(`Failed to parse build response: ${String(buildJson.error)}\n${truncate(buildText)}`);
    }

    const buildData = buildJson.json;

    // Handle response format
    if (buildData.userCredentials) {
      // Project not initialized - show credentials
      output.markdown(`
***✅ Build Request Successful***

**Message:** ${buildData.message || "Project initialization successful"}

**User Credentials:**
- **Username:** ${buildData.userCredentials.username || "N/A"}
- **Connection String:** ${buildData.userCredentials.connectionString || "N/A"}

Your project has been initialized. Please save these credentials.
`);
      
      // Update the ERB version record with the message
      try {
        await erbVersions.updateRecordAsync(erbVersionRecordId, {
          [FIELD_IS_PUBLISHED]: true,
          [FIELD_NOTES]: `Initialized: ${buildData.message || "Project initialized"}\n\nUsername: ${buildData.userCredentials.username || "N/A"}\nConnection String: ${buildData.userCredentials.connectionString || "N/A"}`,
        });
      } catch (e) {
        output.text(`(Could not update ERB version record: ${String(e)})`);
      }
      
      return;
    } else if (buildData.message) {
      // Project already initialized
      output.markdown(`
***✅ Build Request Successful***

**Message:** ${buildData.message}

Your changes have been published via **EffortlessAPI**.
`);
      
      try {
        await erbVersions.updateRecordAsync(erbVersionRecordId, {
          [FIELD_IS_PUBLISHED]: true,
          [FIELD_NOTES]: buildData.message,
        });
      } catch (e) {
        output.text(`(Could not update ERB version record: ${String(e)})`);
      }
      
      return;
    } else {
      // Unexpected response format
      output.markdown(`
***⚠️ Unexpected Response Format***

Received response but couldn't determine status. Response:
${mdCodeBlock(JSON.stringify(buildData, null, 2))}
`);
      
      try {
        await erbVersions.updateRecordAsync(erbVersionRecordId, {
          [FIELD_NOTES]: `Unexpected response format: ${JSON.stringify(buildData)}`,
        });
      } catch (e) {
        output.text(`(Could not update ERB version record: ${String(e)})`);
      }
      
      return;
    }
  } else {
    // ============================================
    // BRIDGE ROUTE (original implementation)
    // ============================================
    const key = `${base.id}`;
    const markUrl = `${bridgeBaseUrl}/mark/${key}`;
    const resultUrl = `${bridgeBaseUrl}/result/${key}`;

    const payload = {
      baseId: base.id,
      runId,
      commitMessage,
      commitDate: commitDate.toISOString(),
      versionName,
      erbVersionRecordId,
      customizations,
    };

    // POST /mark
    output.markdown(`📤 Sending build job to bridge…\n\n\`${markUrl}\``);

    const markResp = await fetchWithTimeout(markUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${pat}`,
      },
      body: JSON.stringify(payload),
    }, 5 * 60 * 1000); // 5 minute timeout

    if (!markResp.ok) {
      const markText = await safeReadText(markResp);
      throw new Error(`Mark failed: HTTP ${markResp.status}\n${truncate(markText)}`);
    }

    output.markdown("✅ Job queued. Polling for result…");

    poll = await pollForResult(resultUrl, { debug });

    if (poll.timedOut) {
      output.markdown(`***⏱️ Timed out***  
No build result received after 60 seconds.`);
      if (debug && poll.lastSnapshot) {
        output.markdown(`Last snapshot:\n${mdCodeBlock(truncate(JSON.stringify(poll.lastSnapshot, null, 2)))}`);
      }
      return;
    }

    payloadResult = poll.payload || {};

    if (payloadResult.success) {
      await erbVersions.updateRecordAsync(erbVersionRecordId, {
        [FIELD_IS_PUBLISHED]: true,
      });

      output.markdown(`
***✅ Build Succeeded***

Your changes have been published via **EffortlessAPI**.
`);
      return;
    }
  }

  // ------------------------------
  // FAILURE HANDLING (error first)
  // Only applies to bridge route failures
  // ------------------------------
  if (useEffortlessBase) {
    // Error handling for effortless_base route is done above
    return;
  }

  const buildOutput = getBuildOutput(payloadResult, poll);

  // 1) Error block first (visually obvious; no partial markdown fences)
  const primaryBlocks = extractPrimaryErrorBlocks(buildOutput);

  output.markdown(`## 🟥 ERROR (first thing to look at)`);

  if (primaryBlocks.length) {
    for (let i = 0; i < primaryBlocks.length; i++) {
      const b = primaryBlocks[i];
      const snippet = b.lines.slice(b.start, b.end + 1).join("\n");
      output.markdown(`### Error Snippet ${i + 1}\n${mdCodeBlock(emphasizeErrorText(truncate(snippet, 3000)))}`);
    }
  } else {
    output.markdown(`No explicit \`ERROR:\` lines found in output.`);
  }

  // 2) Then context windows
  const windows = extractErrorWindows(buildOutput, { contextLines: ERROR_CONTEXT_LINES });

  output.markdown(`## 📄 Output (context)`);

  if (windows.length) {
    for (let i = 0; i < windows.length; i++) {
      const snippet = formatWindow(windows[i]);
      output.markdown(`### Context ${i + 1}\n${mdCodeBlock(truncate(snippet, 6000))}`);
    }
  } else {
    const lines = splitLinesPreserve(buildOutput);
    const tail = lines.slice(Math.max(0, lines.length - 60)).join("\n");
    output.markdown(mdCodeBlock(truncate(tail, 8000)));
  }

  // 3) Reveal more behind a button
  const revealChoice = await input.buttonsAsync("Show more details?", [
    "Full log",
    "Debug JSON",
    "Done",
  ]);

  if (revealChoice === "Full log") {
    await showPagedLog("Full Build Output", buildOutput);
  } else if (revealChoice === "Debug JSON") {
    const debugObj = {
      resultUrl,
      lastSnapshot: poll.lastSnapshot || null,
      raw: poll.raw || null,
      payload: poll.payload || null,
    };
    await showPagedLog("Debug Result JSON", JSON.stringify(debugObj, null, 2));
  }

  // 4) Persist to Notes
  try {
    const errFirst = primaryBlocks.length
      ? primaryBlocks
          .map(
            (b, idx) =>
              `--- ERROR Snippet ${idx + 1} ---\n${b.lines.slice(b.start, b.end + 1).join("\n")}`
          )
          .join("\n\n")
      : "(No ERROR: lines found)";

    await erbVersions.updateRecordAsync(erbVersionRecordId, {
      [FIELD_NOTES]: truncate(
        `BUILD FAILED\n\nERROR FIRST:\n${errFirst}\n\nFULL OUTPUT (truncated):\n${buildOutput}`,
        90000
      ),
    });
  } catch (e) {
    output.text(`(Could not save failure details to Notes: ${String(e)})`);
  }

  return;
}

await main();
