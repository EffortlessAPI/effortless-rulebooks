import express, { type Request, type Response, type NextFunction } from 'express';
import path from 'node:path';
import { existsSync } from 'node:fs';
import pg from 'pg';

const PORT = Number(process.env.PORT ?? 3032);
const DATABASE_URL =
  process.env.DATABASE_URL ?? 'postgresql://postgres@localhost:5432/erb_intelligence_taxonomy';
const SERVE_STATIC = process.env.SERVE_STATIC === 'true';
const STATIC_DIR = process.env.STATIC_DIR ?? path.resolve(process.cwd(), '../web/dist');

// SSL only when the URL explicitly opts in. bases.effortlessapi.com:5432
// does NOT terminate TLS as of May 2026 — connecting with SSL fails.
const needsSsl = /sslmode=require/i.test(DATABASE_URL);
const pool = new pg.Pool({
  connectionString: DATABASE_URL,
  ssl: needsSsl ? { rejectUnauthorized: false } : false,
});

// Dev users are stubbed (no Users table in the rulebook for the minimal demo).
// Real auth would replace this with magic-links / Bearer JWT.
const DEV_USERS = [
  {
    email: 'alice@example.com',
    name: 'Alice Researcher',
    role: 'researcher',
    blurb: 'Primary role — can edit Assessments and watch the DAG cascade.',
  },
  {
    email: 'bob@example.com',
    name: 'Bob Reviewer',
    role: 'reviewer',
    blurb: 'Placeholder role — would see read-only review queues.',
  },
  {
    email: 'carol@example.com',
    name: 'Carol Public',
    role: 'public',
    blurb: 'Placeholder role — would see the public-facing taxonomy.',
  },
] as const;

type DevUser = (typeof DEV_USERS)[number];

declare module 'express-serve-static-core' {
  interface Request {
    me?: DevUser;
  }
}

const app = express();
app.use(express.json());

// CORS not needed in dev — Vite proxies /api to here.

app.use((req: Request, _res: Response, next: NextFunction) => {
  const email = req.header('x-user-email');
  if (email) {
    const me = DEV_USERS.find((u) => u.email === email);
    if (me) req.me = me;
  }
  next();
});

function requireAuth(req: Request, res: Response, next: NextFunction) {
  if (!req.me) {
    res.status(401).json({ error: 'not signed in' });
    return;
  }
  next();
}

function requireRole(...roles: string[]) {
  return (req: Request, res: Response, next: NextFunction) => {
    if (!req.me) return res.status(401).json({ error: 'not signed in' });
    if (!roles.includes(req.me.role)) return res.status(403).json({ error: 'forbidden' });
    next();
  };
}

app.get('/healthz', async (_req, res) => {
  try {
    const r = await pool.query('SELECT 1 AS ok');
    res.json({ ok: r.rows[0].ok === 1 });
  } catch (e) {
    res.status(500).json({ ok: false, error: String(e) });
  }
});

app.get('/api/dev-users', (_req, res) => {
  res.json(DEV_USERS);
});

app.get('/api/me', requireAuth, (req, res) => {
  res.json(req.me);
});

// ---------------------------------------------------------------- views

app.get('/api/intelligences', requireAuth, async (_req, res) => {
  const r = await pool.query(
    'SELECT * FROM vw_intelligences ORDER BY total_weighted_score DESC NULLS LAST, name'
  );
  res.json(r.rows);
});

app.get('/api/intelligences/:id', requireAuth, async (req, res) => {
  const r = await pool.query('SELECT * FROM vw_intelligences WHERE intelligences_id = $1', [
    req.params.id,
  ]);
  if (!r.rows.length) return res.status(404).json({ error: 'not found' });
  res.json(r.rows[0]);
});

app.get('/api/capabilities', requireAuth, async (_req, res) => {
  const r = await pool.query('SELECT * FROM vw_capabilities ORDER BY tier, name');
  res.json(r.rows);
});

app.get('/api/capabilities/:id', requireAuth, async (req, res) => {
  const r = await pool.query('SELECT * FROM vw_capabilities WHERE capabilities_id = $1', [
    req.params.id,
  ]);
  if (!r.rows.length) return res.status(404).json({ error: 'not found' });
  res.json(r.rows[0]);
});

app.patch('/api/capabilities/:id', requireRole('researcher'), async (req, res) => {
  const sets: string[] = [];
  const vals: unknown[] = [];
  const allow = ['description', 'tier', 'weight'] as const;
  for (const k of allow) {
    if (k in req.body) {
      vals.push(req.body[k]);
      sets.push(`${k} = $${vals.length}`);
    }
  }
  if (!sets.length) return res.status(400).json({ error: 'no editable fields supplied' });
  vals.push(req.params.id);
  const sql = `UPDATE capabilities SET ${sets.join(', ')} WHERE capabilities_id = $${vals.length} RETURNING capabilities_id`;
  const r = await pool.query(sql, vals);
  if (!r.rows.length) return res.status(404).json({ error: 'not found' });
  const view = await pool.query('SELECT * FROM vw_capabilities WHERE capabilities_id = $1', [
    req.params.id,
  ]);
  res.json(view.rows[0]);
});

app.get('/api/assessments', requireAuth, async (req, res) => {
  const { intelligence, capability } = req.query as { intelligence?: string; capability?: string };
  const where: string[] = [];
  const vals: unknown[] = [];
  if (intelligence) {
    vals.push(intelligence);
    where.push(`intelligence = $${vals.length}`);
  }
  if (capability) {
    vals.push(capability);
    where.push(`capability = $${vals.length}`);
  }
  const sql =
    'SELECT * FROM vw_assessments' +
    (where.length ? ` WHERE ${where.join(' AND ')}` : '') +
    ' ORDER BY intelligence_name, capability_name';
  const r = await pool.query(sql, vals);
  res.json(r.rows);
});

app.get('/api/assessments/:id', requireAuth, async (req, res) => {
  const r = await pool.query('SELECT * FROM vw_assessments WHERE assessments_id = $1', [
    req.params.id,
  ]);
  if (!r.rows.length) return res.status(404).json({ error: 'not found' });
  res.json(r.rows[0]);
});

app.patch('/api/assessments/:id', requireRole('researcher'), async (req, res) => {
  const { raw_score } = req.body as { raw_score?: number };
  if (raw_score === undefined) return res.status(400).json({ error: 'raw_score required' });
  const n = Number(raw_score);
  if (!Number.isFinite(n) || n < 0 || n > 100)
    return res.status(400).json({ error: 'raw_score must be 0..100' });
  const r = await pool.query(
    'UPDATE assessments SET raw_score = $1 WHERE assessments_id = $2 RETURNING assessments_id',
    [n, req.params.id]
  );
  if (!r.rows.length) return res.status(404).json({ error: 'not found' });
  const view = await pool.query('SELECT * FROM vw_assessments WHERE assessments_id = $1', [
    req.params.id,
  ]);
  res.json(view.rows[0]);
});

// ---------------------------------------------------------------- static SPA
// In production the server also serves the built Vite app. Mount AFTER the
// /api routes so they keep taking priority; the wildcard at the end serves
// index.html so deep links work on refresh.
if (SERVE_STATIC && existsSync(STATIC_DIR)) {
  app.use(express.static(STATIC_DIR));
  app.get('*', (req, res, next) => {
    if (req.path.startsWith('/api') || req.path.startsWith('/healthz')) return next();
    res.sendFile(path.join(STATIC_DIR, 'index.html'));
  });
  console.log(`[server] serving SPA from ${STATIC_DIR}`);
}

app.listen(PORT, () => {
  console.log(`[server] listening on http://localhost:${PORT}`);
  console.log(`[server] DB: ${DATABASE_URL.replace(/:\/\/[^@]+@/, '://***@')}`);
});
