const express = require('express');
const { Pool } = require('pg');
const { spawn } = require('child_process');
const fs = require('fs');
const os = require('os');
const path = require('path');
const crypto = require('crypto');

// Fixed, uncommon port pair (picked once) so this demo doesn't fight every
// other app for :3000. ODD = API, the SUBSEQUENT EVEN = client (static UI).
// Override via env if you ever need to.
const API_PORT = Number(process.env.API_PORT) || 7001; // odd
const CLIENT_PORT = Number(process.env.CLIENT_PORT) || 7002; // even (API_PORT + 1)

// DATABASE_URL is the single source of truth. The default is the SSoT-derived
// value for this domain (matches postgres-bootstrap/effortless.env and the
// formula in orchestrate.sh) — NOT a generic fallback. Override via env if needed.
const DATABASE_URL =
  process.env.DATABASE_URL ||
  'postgresql://postgres@localhost:5432/erb_customer_fullname';

const pool = new Pool({ connectionString: DATABASE_URL });

// ===========================================================================
// Export-to-Excel
// ---------------------------------------------------------------------------
// Take a COPY of the SSoT rulebook, clear every table's `data`, then re-fill it
// from the live `vw_<table>` views (raw + already-computed Name/Initials — the
// view IS the contract, so we just SELECT it, no formula re-eval in JS). Write
// that populated copy to a temp `rulebook-to-save.json` and run the
// rulebook-to-xlsx transpiler over it to produce a workbook of current values.
//
// Why we call the injector directly instead of the :4242 ssotme-proxy: the
// proxy hard-resolves its input to the *committed* <domain>-rulebook.json (it
// identifies the project by the CLI's cwd and ignores `-i`), so routing through
// it would export the SEED data, not the live DB. The injector itself honors
// ERB_RULEBOOK_PATH — the exact env var the proxy sets internally (see
// effortless-platform/ssotme-proxy/server.py run_injector). We point that at
// our temp rulebook and run the same script the proxy would. No fallbacks: any
// view-query / temp-write / transpiler failure surfaces to the client.
// ===========================================================================
const REPO_ROOT = path.resolve(__dirname, '..', '..'); // <...>/effortless-rulebooks
const XLSX_INJECTOR = path.join(REPO_ROOT, 'execution-substrates', 'xlsx', 'inject-into-xlsx.py');
const RULEBOOK_PATH = path.join(__dirname, 'effortless-rulebook', 'customer-fullname-rulebook.json');
const PYTHON = process.env.PYTHON || 'python3';
const PROJECT_NAME = process.env.PROJECT_NAME || 'customer-fullname';

// Tables that legitimately have no vw_ view (metadata, not business rows).
const VIEWLESS_TABLES = new Set(['__meta__']);
// Top-level rulebook keys that are NOT tables.
const RESERVED_KEYS = new Set(['$schema', 'Name', 'Description', '_meta']);

// PascalCase / mixed -> snake_case, matching rulebook-to-postgres
// (Customers -> customers, FirstName -> first_name).
function toSnake(name) {
  return name
    .replace(/([a-z0-9])([A-Z])/g, '$1_$2')
    .replace(/([A-Z]+)([A-Z][a-z])/g, '$1_$2')
    .toLowerCase()
    .replace(/^_+/, '');
}

// In-flight exports keyed by a one-time token: the finished file + its scratch
// dir, so the download route can stream then clean up.
const pendingExports = new Map();

// Build the populated rulebook copy from the live views. Calls onStage(label,
// pct) as it progresses. Throws loudly on any DB / mapping failure.
async function buildExportRulebook(onStage) {
  onStage('Reading rulebook schema', 5);
  const rulebook = JSON.parse(fs.readFileSync(RULEBOOK_PATH, 'utf8'));

  const tableNames = Object.keys(rulebook).filter(
    (k) => !RESERVED_KEYS.has(k) && rulebook[k] && Array.isArray(rulebook[k].schema)
  );

  // Match against the REAL view set — never guess a view that doesn't exist.
  onStage('Discovering Postgres views', 12);
  const { rows: viewRows } = await pool.query(
    "SELECT table_name FROM information_schema.views WHERE table_schema = 'public'"
  );
  const liveViews = new Set(viewRows.map((r) => r.table_name));

  const dataTables = tableNames.filter((t) => !VIEWLESS_TABLES.has(t));
  for (const t of tableNames) rulebook[t].data = []; // an export reflects the live DB

  let done = 0;
  for (const table of dataTables) {
    const viewName = `vw_${toSnake(table)}`;
    if (!liveViews.has(viewName)) {
      throw new Error(
        `Expected view ${viewName} for table ${table} but it does not exist. ` +
          `Views present: ${[...liveViews].join(', ') || '(none)'}. ` +
          `Rebuild the rulebook (effortless build) so the view exists.`
      );
    }
    onStage(`Querying ${viewName}`, 18 + Math.round((done / dataTables.length) * 40));

    const { rows } = await pool.query(`SELECT * FROM ${viewName}`);
    const schema = rulebook[table].schema;
    rulebook[table].data = rows.map((row) => {
      const mapped = {};
      for (const field of schema) {
        const snake = toSnake(field.name);
        if (snake in row) mapped[field.name] = row[snake];
      }
      return mapped;
    });
    done += 1;
    onStage(`Loaded ${rows.length} row(s) from ${viewName}`, 18 + Math.round((done / dataTables.length) * 40));
  }
  return rulebook;
}

// Run the xlsx injector over a temp rulebook in an isolated scratch dir.
// Resolves with the absolute path to the generated workbook. Rejects loudly.
function runXlsxTranspiler(scratchDir, rulebookExportPath, onStage) {
  return new Promise((resolve, reject) => {
    onStage('Running rulebook-to-xlsx transpiler', 65);
    // The injector writes `rulebook.xlsx` into its cwd, reads its input from
    // ERB_RULEBOOK_PATH, and needs the repo root on PYTHONPATH for
    // `orchestration.shared`. This mirrors the proxy's run_injector env.
    const env = {
      ...process.env,
      ERB_RULEBOOK_PATH: rulebookExportPath,
      ERB_OUTPUT_DIR: scratchDir,
      PYTHONPATH: REPO_ROOT,
    };
    const proc = spawn(PYTHON, [XLSX_INJECTOR], { cwd: scratchDir, env });
    let stderr = '';
    proc.stderr.on('data', (d) => { stderr += d.toString(); });
    proc.on('error', (e) => reject(new Error(`Failed to launch ${PYTHON} ${XLSX_INJECTOR}: ${e.message}`)));
    proc.on('close', (code) => {
      if (code !== 0) return reject(new Error(`rulebook-to-xlsx exited ${code}.\n${stderr.trim()}`));
      const out = path.join(scratchDir, 'rulebook.xlsx');
      if (!fs.existsSync(out)) {
        return reject(new Error(`Transpiler exited 0 but produced no rulebook.xlsx in ${scratchDir}.\n${stderr.trim()}`));
      }
      resolve(out);
    });
  });
}

function cleanupScratch(dir) {
  try { fs.rmSync(dir, { recursive: true, force: true }); } catch { /* best-effort */ }
}

// Register the export routes on an express app.
function registerExport(app) {
  // --- Streamed export: SSE with real per-stage progress ---
  app.get('/api/export/xlsx/stream', async (req, res) => {
    res.writeHead(200, {
      'Content-Type': 'text/event-stream',
      'Cache-Control': 'no-cache',
      Connection: 'keep-alive',
    });
    const send = (event, data) => {
      res.write(`event: ${event}\n`);
      res.write(`data: ${JSON.stringify(data)}\n\n`);
    };
    const onStage = (label, pct) => send('progress', { label, pct });

    const token = crypto.randomBytes(16).toString('hex');
    const scratchDir = fs.mkdtempSync(path.join(os.tmpdir(), 'erb-xlsx-export-'));
    try {
      onStage('Starting export', 2);
      const rulebook = await buildExportRulebook(onStage);

      onStage('Writing temporary rulebook', 60);
      const exportPath = path.join(scratchDir, 'rulebook-to-save.json');
      fs.writeFileSync(exportPath, JSON.stringify(rulebook, null, 2));

      const xlsxPath = await runXlsxTranspiler(scratchDir, exportPath, onStage);

      onStage('Packaging workbook', 95);
      const filename = `${PROJECT_NAME}-rulebook.xlsx`;
      pendingExports.set(token, { xlsxPath, scratchDir, filename });
      send('done', { token, filename, downloadUrl: `/api/export/xlsx/download/${token}` });
      res.end();
    } catch (err) {
      cleanupScratch(scratchDir);
      send('error', { message: String(err && err.message ? err.message : err) });
      res.end();
    }
  });

  // --- One-time download of a finished export, then clean up. ---
  app.get('/api/export/xlsx/download/:token', (req, res) => {
    const entry = pendingExports.get(req.params.token);
    if (!entry) {
      return res.status(404).json({ error: 'Export not found or already downloaded.' });
    }
    pendingExports.delete(req.params.token);
    res.download(entry.xlsxPath, entry.filename, (err) => {
      cleanupScratch(entry.scratchDir);
      if (err && !res.headersSent) {
        res.status(500).json({ error: String(err.message || err) });
      }
    });
  });
}

// The app READS from the vw_* view (raw + every calculated field, denormalized)
// and WRITES to the base table (raw facts only). It never recomputes business
// logic itself — Name/Initials come from the rulebook-derived view.
const VIEW_COLUMNS =
  'customer_id, name, initials, first_name, last_name';

// Map a vw_customers row (snake_case) to the API shape the frontend expects.
function toApi(row) {
  return {
    CustomerId: row.customer_id,
    Name: row.name,
    Initials: row.initials,
    FirstName: row.first_name,
    LastName: row.last_name,
  };
}

// Only raw fields are writable. Calculated fields are derived in the view.
const WRITABLE = {
  FirstName: 'first_name',
  LastName: 'last_name',
};

// Register the JSON API on any express app (mounted on both the API port and
// the client port so the static page can call /api same-origin, no CORS).
function registerApi(app) {
  app.get('/api/customers', async (req, res) => {
    try {
      const { rows } = await pool.query(
        `SELECT ${VIEW_COLUMNS} FROM vw_customers ORDER BY customer_id`
      );
      res.json(rows.map(toApi));
    } catch (err) {
      res.status(500).json({ error: String(err.message || err) });
    }
  });

  app.patch('/api/customers/:customerId', async (req, res) => {
    const sets = [];
    const values = [];
    for (const [apiField, column] of Object.entries(WRITABLE)) {
      if (req.body[apiField] !== undefined) {
        values.push(req.body[apiField]);
        sets.push(`${column} = $${values.length}`);
      }
    }

    if (sets.length === 0) {
      return res.status(400).json({ error: 'No writable (raw) fields provided' });
    }

    values.push(req.params.customerId);
    const idParam = `$${values.length}`;

    try {
      const update = await pool.query(
        `UPDATE customers SET ${sets.join(', ')} WHERE customer_id = ${idParam}`,
        values
      );
      if (update.rowCount === 0) {
        return res.status(404).json({ error: 'Not found' });
      }

      // Re-read from the view so the response carries the freshly recomputed
      // calculated fields (FullName, Initials, Name) — not anything we guessed.
      const { rows } = await pool.query(
        `SELECT ${VIEW_COLUMNS} FROM vw_customers WHERE customer_id = $1`,
        [req.params.customerId]
      );
      res.json(toApi(rows[0]));
    } catch (err) {
      res.status(500).json({ error: String(err.message || err) });
    }
  });

  // Export-to-Excel: dump the live view rows into a temp rulebook copy and run
  // the rulebook-to-xlsx transpiler over it (streamed progress + download).
  registerExport(app);
}

// --- API server (odd port): JSON only ---
const apiApp = express();
apiApp.use(express.json());
registerApi(apiApp);

// --- Client server (even port): static UI + same-origin API ---
const clientApp = express();
clientApp.use(express.json());
clientApp.use(express.static(require('path').join(__dirname, 'web/dist'), { extensions: ['html'] }));
registerApi(clientApp);

// Fallback to index.html for React Router
clientApp.use((req, res) => {
  res.sendFile(require('path').join(__dirname, 'web/dist/index.html'));
});

apiApp.listen(API_PORT, () => {
  console.log(`[api]    http://localhost:${API_PORT}  (JSON API)`);
});
clientApp.listen(CLIENT_PORT, () => {
  console.log(`[client] http://localhost:${CLIENT_PORT}  (open this in a browser)`);
  console.log(`Connected to ${DATABASE_URL}`);
  console.log(`Press Ctrl+C to stop`);
});
