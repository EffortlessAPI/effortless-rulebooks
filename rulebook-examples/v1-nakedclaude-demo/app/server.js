import dotenv from 'dotenv';
import path from 'path';
import { fileURLToPath } from 'url';
const __filename_boot = fileURLToPath(import.meta.url);
dotenv.config({ path: path.join(path.dirname(__filename_boot), '..', '.env') });
import express from 'express';
import cookieParser from 'cookie-parser';
import jwt from 'jsonwebtoken';
import pg from 'pg';

const __dirname = path.dirname(__filename_boot);
const PORT = process.env.PORT || 3011;
const ML_BASE = process.env.MAGICLINK_BASE_URL;
const TENANT_ID = process.env.MAGICLINK_TENANT_ID;

// Pool connects as the admin role (PGUSER from env). Each request switches
// to app_user / app_admin via SET LOCAL ROLE so RLS applies.
const pool = new pg.Pool();

// Cache trusted_tenants lookups so we don't hit the DB for every JWT verify.
const tenantCache = new Map(); // tenant_id -> public_key_pem
async function trustedKey(tenantId) {
  if (tenantCache.has(tenantId)) return tenantCache.get(tenantId);
  const { rows } = await pool.query(
    `SELECT public_key_pem FROM auth.trusted_tenants
      WHERE tenant_id = $1 AND is_active = true`,
    [tenantId]);
  if (!rows[0]) return null;
  tenantCache.set(tenantId, rows[0].public_key_pem);
  return rows[0].public_key_pem;
}

const app = express();
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

function readBearer(req) {
  const h = req.headers.authorization;
  if (h?.startsWith('Bearer ')) return h.slice(7);
  return req.cookies?.jwt || null;
}

// Verify JWT, attach req.user. Does NOT acquire a DB client — withUser does.
async function requireAuth(req, res, next) {
  const token = readBearer(req);
  if (!token) {
    if (req.accepts('html')) return res.redirect('/login');
    return res.status(401).json({ error: 'missing_token' });
  }
  try {
    const unverified = jwt.decode(token);
    if (!unverified?.tenant_id) throw new Error('no_tenant_claim');
    const pem = await trustedKey(unverified.tenant_id);
    if (!pem) throw new Error(`untrusted_tenant: ${unverified.tenant_id}`);
    const decoded = jwt.verify(token, pem, {
      algorithms: ['RS256'],
      issuer: unverified.iss,
    });
    req.user = { email: decoded.email, claims: decoded, token };
    next();
  } catch (e) {
    console.error('[auth] verify failed:', e.message);
    if (req.accepts('html')) return res.redirect('/login');
    res.status(401).json({ error: 'invalid_token', detail: e.message });
  }
}

// Run `fn(client)` in a transaction with role switched + JWT GUCs stamped.
// Role choice depends on req.user.claims, but we don't trust the claim:
// app.jwt_role() inside Postgres reads vw_app_users authoritatively.
// We pick app_admin role only after a separate role lookup; default app_user.
async function withUser(req, fn) {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    // Switch role first; auth.begin_session is SECURITY DEFINER so it
    // still reads trusted_tenants + vw_app_users with postgres privs.
    const role = req.user.claims && req.user._role; // unused; role comes from DB below
    await client.query(`SET LOCAL ROLE app_user`);
    const { rows } = await client.query(`SELECT auth.begin_session($1) AS role`, [req.user.token]);
    req.user.role = rows[0]?.role || 'anon';
    if (req.user.role === 'admin') {
      await client.query(`SET LOCAL ROLE app_admin`);
    }
    const result = await fn(client);
    await client.query('COMMIT');
    return result;
  } catch (e) {
    await client.query('ROLLBACK').catch(() => {});
    throw e;
  } finally {
    client.release();
  }
}

function requireAdmin(req, res, next) {
  if (req.user?.role !== 'admin') {
    if (req.accepts('html')) return res.status(403).render('error', { user: req.user, msg: 'Admin only' });
    return res.status(403).json({ error: 'forbidden' });
  }
  next();
}

// ---- Public routes ----
app.get('/', (req, res) => res.render('home', { user: null }));
app.get('/login', (req, res) => res.render('login', { tenantId: TENANT_ID, mlBase: ML_BASE }));

app.post('/api/auth/request-code', async (req, res) => {
  const { email } = req.body;
  const r = await fetch(`${ML_BASE}/api/tenants/${TENANT_ID}/send-code`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email }),
  });
  res.status(r.status).json(await r.json());
});

app.post('/api/auth/verify-code', async (req, res) => {
  const { email, code } = req.body;
  const r = await fetch(`${ML_BASE}/api/tenants/${TENANT_ID}/verify-code`, {
    method: 'POST', headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, code }),
  });
  const body = await r.json();
  const token = body.jwt || body.token || body.access_token;
  if (r.ok && token) {
    res.cookie('jwt', token, { httpOnly: false, sameSite: 'lax', path: '/', maxAge: 24*3600*1000 });
    return res.status(200).json({ jwt: token });
  }
  res.status(r.status).json(body);
});

app.post('/api/auth/logout', (req, res) => {
  res.clearCookie('jwt', { path: '/' });
  res.json({ ok: true });
});

// withUser wrapper — runs the route's body in an authed DB transaction.
const authed = (handler) => [requireAuth, async (req, res, next) => {
  try { await withUser(req, (db) => handler(req, res, db)); }
  catch (e) { next(e); }
}];
const authedAdmin = (handler) => [requireAuth, async (req, res, next) => {
  try {
    await withUser(req, async (db) => {
      if (req.user.role !== 'admin') {
        if (req.accepts('html')) return res.status(403).render('error', { user: req.user, msg: 'Admin only' });
        return res.status(403).json({ error: 'forbidden' });
      }
      return handler(req, res, db);
    });
  } catch (e) { next(e); }
}];

app.get('/me', ...authed((req, res) => res.json(req.user)));

app.get('/portal', ...authed((req, res) => {
  res.redirect(req.user.role === 'admin' ? '/admin' : '/customer');
}));

// ---- Admin portal ----
app.get('/admin', ...authedAdmin(async (req, res, db) => {
  const counts = await db.query(
    `SELECT status_display_name AS status, count(*)::int AS n,
            sum(CASE WHEN is_stopped THEN 1 ELSE 0 END)::int AS stopped
     FROM vw_customers GROUP BY 1 ORDER BY 1`);
  res.render('admin_dashboard', { user: req.user, counts: counts.rows });
}));

app.get('/admin/customers', ...authedAdmin(async (req, res, db) => {
  const { rows } = await db.query(
    `SELECT customer_id, name, owner_email, status_display_name, is_stopped
     FROM vw_customers ORDER BY name`);
  res.render('admin_customers', { user: req.user, customers: rows });
}));

app.get('/admin/customers/:id', ...authedAdmin(async (req, res, db) => {
  const { rows } = await db.query(`SELECT * FROM vw_customers WHERE customer_id=$1`, [req.params.id]);
  const statuses = await db.query(`SELECT statuse_id, display_name FROM vw_statuses ORDER BY display_name`);
  if (!rows[0]) return res.status(404).render('error', { user: req.user, msg: 'Not found' });
  res.render('admin_customer_detail', { user: req.user, c: rows[0], statuses: statuses.rows });
}));

app.get('/admin/statuses', ...authedAdmin(async (req, res, db) => {
  const { rows } = await db.query(`SELECT * FROM vw_statuses ORDER BY display_name`);
  res.render('admin_statuses', { user: req.user, statuses: rows });
}));

app.get('/admin/users', ...authedAdmin(async (req, res, db) => {
  const { rows } = await db.query(`SELECT * FROM vw_app_users ORDER BY email_address`);
  res.render('admin_users', { user: req.user, users: rows });
}));

app.get('/api/admin/customers', ...authedAdmin(async (req, res, db) => {
  const { rows } = await db.query(`SELECT * FROM vw_customers`);
  res.json(rows);
}));

// ---- Customer portal ----
// RLS now scopes vw_customers automatically, so no WHERE clause needed.
app.get('/customer', ...authed(async (req, res, db) => {
  const { rows } = await db.query(
    `SELECT customer_id, name, status_display_name, is_stopped
     FROM vw_customers ORDER BY name`);
  res.render('customer_dashboard', { user: req.user, customers: rows });
}));

app.get('/customer/customers/:id', ...authed(async (req, res, db) => {
  const { rows } = await db.query(`SELECT * FROM vw_customers WHERE customer_id=$1`, [req.params.id]);
  if (!rows[0]) return res.status(404).render('error', { user: req.user, msg: 'Not found or not yours' });
  res.render('customer_detail', { user: req.user, c: rows[0] });
}));

app.get('/api/customer/customers', ...authed(async (req, res, db) => {
  const { rows } = await db.query(`SELECT * FROM vw_customers`);
  res.json(rows);
}));

app.listen(PORT, () => console.log(`v1-nakedclaude-demo on :${PORT}`));
