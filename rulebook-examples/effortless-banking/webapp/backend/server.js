// First Valley Bank — admin portal backend.
// Express + pg. Sessions hold the simulated identity {name, role, email}.
// Every DB call uses a per-request transaction that SETs LOCAL session
// vars (app.current_user_name, app.current_user_role) so future real
// RLS policies can pivot on them. Reads always go through vw_* views.

import express from 'express';
import cors from 'cors';
import cookieSession from 'cookie-session';
import pg from 'pg';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const DATABASE_URL =
  process.env.DATABASE_URL ||
  'postgresql://postgres@localhost:5432/erb_effortless_banking_demo';

const pool = new pg.Pool({ connectionString: DATABASE_URL });

// Load rulebook so the API can expose field metadata to the frontend
// (used by the double-click field drawer).
const RULEBOOK_PATH = path.resolve(
  __dirname,
  '../../effortless-rulebook/effortless-rulebook.json',
);
const rulebook = JSON.parse(fs.readFileSync(RULEBOOK_PATH, 'utf8'));

const app = express();
app.use(cors({ origin: true, credentials: true }));
app.use(express.json());
app.use(
  cookieSession({
    name: 'fvb',
    secret: 'demo-only-not-secret',
    httpOnly: true,
    sameSite: 'lax',
    maxAge: 1000 * 60 * 60 * 8,
  }),
);

// ---- helpers --------------------------------------------------------------

async function withUserSession(req, fn) {
  const u = req.session?.user;
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    if (u) {
      await client.query(`SELECT set_config('app.current_user_name', $1, true)`, [u.name]);
      await client.query(`SELECT set_config('app.current_user_role', $1, true)`, [u.role]);
      await client.query(`SELECT set_config('app.current_user_email', $1, true)`, [u.email]);
    }
    const result = await fn(client);
    await client.query('COMMIT');
    return result;
  } catch (err) {
    await client.query('ROLLBACK');
    throw err;
  } finally {
    client.release();
  }
}

function requireAuth(req, res, next) {
  if (!req.session?.user) return res.status(401).json({ error: 'not authenticated' });
  next();
}

function requireRole(...roles) {
  return (req, res, next) => {
    if (!req.session?.user) return res.status(401).json({ error: 'not authenticated' });
    if (!roles.includes(req.session.user.role))
      return res.status(403).json({ error: 'forbidden', need: roles });
    next();
  };
}

// Apply role-based filter to a SELECT against a view. For real RLS this
// would be enforced by the database; here we simulate by appending WHERE
// clauses that match the same intent.
function roleFilter(view, role, userName) {
  switch (view) {
    case 'vw_loans':
      if (role === 'RM') return { sql: ` WHERE originating_rm = $RU`, name: userName };
      if (role === 'Underwriter') return { sql: ` WHERE underwriter = $RU`, name: userName };
      if (role === 'BranchBanker') return { sql: '', name: null }; // sees all funded
      return { sql: '', name: null };
    case 'vw_businesses':
      if (role === 'RM') return { sql: ` WHERE relationship_manager = $RU`, name: userName };
      return { sql: '', name: null };
    default:
      return { sql: '', name: null };
  }
}

// ---- routes ---------------------------------------------------------------

app.get('/api/health', (req, res) => res.json({ ok: true }));

// Login: list seeded demo users so the UI can pick one (no passwords; sim).
app.get('/api/auth/demo-users', async (req, res) => {
  try {
    const r = await withUserSession(req, (c) =>
      c.query(
        `SELECT name, full_name, role, email FROM vw_users ORDER BY role, full_name`,
      ),
    );
    res.json(r.rows);
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.post('/api/auth/login', async (req, res) => {
  const { email } = req.body || {};
  if (!email) return res.status(400).json({ error: 'email required' });
  try {
    const r = await withUserSession(req, (c) =>
      c.query(
        `SELECT name, full_name, role, email FROM vw_users WHERE lower(email) = lower($1) LIMIT 1`,
        [email],
      ),
    );
    if (!r.rows.length) return res.status(404).json({ error: 'unknown user' });
    req.session.user = r.rows[0];
    res.json(req.session.user);
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.post('/api/auth/logout', (req, res) => {
  req.session = null;
  res.json({ ok: true });
});

app.get('/api/auth/me', (req, res) => {
  if (!req.session?.user) return res.status(401).json({ error: 'not authenticated' });
  res.json(req.session.user);
});

// Rulebook metadata (for FieldDrawer / future explainer-DAG).
app.get('/api/rulebook/tables', (req, res) => {
  const tables = Object.keys(rulebook)
    .filter((k) => !['$schema', 'Name', 'Description'].includes(k))
    .map((name) => ({ name, description: rulebook[name].Description || '' }));
  res.json(tables);
});

app.get('/api/rulebook/field/:table/:field', (req, res) => {
  const { table, field } = req.params;
  const t = rulebook[table];
  if (!t) return res.status(404).json({ error: 'unknown table' });
  const schema = t.schema || [];
  const f = schema.find((x) => x.name?.toLowerCase() === field.toLowerCase());
  if (!f) return res.status(404).json({ error: 'unknown field' });
  // Walk dependencies for calculated/aggregation fields.
  const deps = [];
  if (f.formula) {
    const re = /\{\{(\w+)\}\}/g;
    let m;
    while ((m = re.exec(f.formula))) deps.push({ table, name: m[1] });
  }
  res.json({ table, field: f, deps });
});

// Generic list endpoint for a view.
function listEndpoint(view, defaultOrder = 'name') {
  return async (req, res) => {
    try {
      const u = req.session.user;
      const filter = roleFilter(view, u.role, u.name);
      const params = [];
      let sql = `SELECT * FROM ${view}`;
      if (filter.sql) {
        params.push(filter.name);
        sql += filter.sql.replace('$RU', `$${params.length}`);
      }
      sql += ` ORDER BY ${defaultOrder}`;
      const limit = Math.min(parseInt(req.query.limit || '500', 10), 1000);
      sql += ` LIMIT ${limit}`;
      const r = await withUserSession(req, (c) => c.query(sql, params));
      res.json(r.rows);
    } catch (e) {
      res.status(500).json({ error: String(e.message || e) });
    }
  };
}

app.get('/api/users', requireAuth, listEndpoint('vw_users', 'full_name'));
app.get('/api/businesses', requireAuth, listEndpoint('vw_businesses', 'legal_name'));
app.get('/api/beneficial_owners', requireAuth, listEndpoint('vw_beneficial_owners', 'full_name'));
app.get('/api/contacts', requireAuth, listEndpoint('vw_contacts', 'full_name'));
app.get('/api/accounts', requireAuth, listEndpoint('vw_accounts', 'name'));
app.get('/api/loans', requireAuth, listEndpoint('vw_loans', 'originated_at DESC NULLS LAST'));
app.get('/api/covenants', requireAuth, listEndpoint('vw_covenants', 'next_test_date NULLS LAST'));
app.get('/api/risk_rating_history', requireAuth, listEndpoint('vw_risk_rating_history', 'effective_date DESC'));
app.get('/api/documents', requireAuth, listEndpoint('vw_documents', 'name'));
app.get('/api/interactions', requireAuth, listEndpoint('vw_interactions', 'interaction_date DESC NULLS LAST'));

// Single-business detail bundle: business + owners + contacts + accounts + loans.
app.get('/api/businesses/:name/detail', requireAuth, async (req, res) => {
  const name = req.params.name;
  try {
    const r = await withUserSession(req, async (c) => {
      const business = (
        await c.query(`SELECT * FROM vw_businesses WHERE name = $1`, [name])
      ).rows[0];
      if (!business) return null;
      const [owners, contacts, accounts, loans, interactions, documents] = await Promise.all([
        c.query(`SELECT * FROM vw_beneficial_owners WHERE business = $1`, [name]),
        c.query(`SELECT * FROM vw_contacts WHERE business = $1`, [name]),
        c.query(`SELECT * FROM vw_accounts WHERE business = $1`, [name]),
        c.query(`SELECT * FROM vw_loans WHERE business = $1`, [name]),
        c.query(`SELECT * FROM vw_interactions WHERE business = $1 ORDER BY interaction_date DESC NULLS LAST`, [name]),
        c.query(`SELECT * FROM vw_documents WHERE business = $1`, [name]),
      ]);
      return {
        business,
        owners: owners.rows,
        contacts: contacts.rows,
        accounts: accounts.rows,
        loans: loans.rows,
        interactions: interactions.rows,
        documents: documents.rows,
      };
    });
    if (!r) return res.status(404).json({ error: 'not found' });
    res.json(r);
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

app.get('/api/loans/:name/detail', requireAuth, async (req, res) => {
  const name = req.params.name;
  try {
    const r = await withUserSession(req, async (c) => {
      const loan = (await c.query(`SELECT * FROM vw_loans WHERE name = $1`, [name])).rows[0];
      if (!loan) return null;
      const [covenants, history, docs] = await Promise.all([
        c.query(`SELECT * FROM vw_covenants WHERE loan = $1`, [name]),
        c.query(`SELECT * FROM vw_risk_rating_history WHERE loan = $1 ORDER BY effective_date DESC`, [name]),
        c.query(`SELECT * FROM vw_documents WHERE loan = $1`, [name]),
      ]);
      return { loan, covenants: covenants.rows, history: history.rows, documents: docs.rows };
    });
    if (!r) return res.status(404).json({ error: 'not found' });
    res.json(r);
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

// Admin-only KPIs for the dashboard.
app.get('/api/kpis', requireAuth, async (req, res) => {
  try {
    const r = await withUserSession(req, async (c) => {
      const q = (sql) => c.query(sql).then((x) => x.rows[0]);
      const [
        loans,
        funded,
        watchlist,
        breached,
        prospects,
        customers,
        deposits,
        principal,
      ] = await Promise.all([
        q(`SELECT COUNT(*)::int AS n FROM vw_loans`),
        q(`SELECT COUNT(*)::int AS n FROM vw_loans WHERE is_funded`),
        q(`SELECT COUNT(*)::int AS n FROM vw_loans WHERE on_watchlist`),
        q(`SELECT COUNT(*)::int AS n FROM vw_covenants WHERE is_breached`),
        q(`SELECT COUNT(*)::int AS n FROM vw_businesses WHERE is_prospect`),
        q(`SELECT COUNT(*)::int AS n FROM vw_businesses WHERE is_customer`),
        q(`SELECT COALESCE(SUM(current_balance_usd),0)::numeric AS n FROM vw_accounts`),
        q(`SELECT COALESCE(SUM(principal_usd),0)::numeric AS n FROM vw_loans WHERE is_funded`),
      ]);
      const byStage = await c.query(
        `SELECT underwriting_stage, COUNT(*)::int AS n FROM vw_loans GROUP BY 1 ORDER BY 1`,
      );
      const byRisk = await c.query(
        `SELECT risk_rating_label, COUNT(*)::int AS n FROM vw_loans GROUP BY 1 ORDER BY 1`,
      );
      return {
        loans: loans.n,
        funded: funded.n,
        watchlist: watchlist.n,
        breachedCovenants: breached.n,
        prospects: prospects.n,
        customers: customers.n,
        totalDeposits: Number(deposits.n),
        totalPrincipal: Number(principal.n),
        byStage: byStage.rows,
        byRisk: byRisk.rows,
      };
    });
    res.json(r);
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

const PORT = process.env.PORT || 4000;
app.listen(PORT, () => console.log(`[fvb] backend listening on :${PORT}`));
