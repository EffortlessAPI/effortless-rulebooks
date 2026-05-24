import express, { Request, Response, NextFunction } from "express";
import cors from "cors";
import pg from "pg";

const PORT = Number(process.env.PORT ?? 3045);
const DATABASE_URL =
  process.env.DATABASE_URL ??
  "postgresql://postgres@localhost:5432/erb_wedding_seating_optimizer";

const pool = new pg.Pool({ connectionString: DATABASE_URL });

const app = express();
app.use(cors());
app.use(express.json());

type Me = { user_id: string; email: string; role: string; display_name: string };
declare global { namespace Express { interface Request { me?: Me } } }

app.get("/healthz", async (_req, res) => {
  const r = await pool.query("SELECT 1 AS ok");
  res.json({ ok: r.rows[0].ok === 1 });
});

app.get("/api/dev-users", async (_req, res) => {
  const r = await pool.query(
    "SELECT user_id, email, role, display_name FROM vw_users ORDER BY role, display_name"
  );
  res.json(r.rows);
});

async function auth(req: Request, res: Response, next: NextFunction) {
  const email = (req.header("x-user-email") ?? "").trim().toLowerCase();
  if (!email) return res.status(401).json({ error: "missing X-User-Email" });
  const u = await pool.query(
    "SELECT user_id, email, role, display_name FROM vw_users WHERE LOWER(email) = $1",
    [email]
  );
  if (u.rowCount === 0) return res.status(401).json({ error: "unknown user" });
  req.me = u.rows[0];
  next();
}

app.get("/api/me", auth, (req, res) => res.json(req.me));

// --- Tables ---
app.get("/api/tables", auth, async (_req, res) => {
  const r = await pool.query("SELECT * FROM vw_tables ORDER BY label");
  res.json(r.rows);
});

app.get("/api/tables/:id", auth, async (req, res) => {
  const t = await pool.query("SELECT * FROM vw_tables WHERE table_id = $1", [req.params.id]);
  if (t.rowCount === 0) return res.status(404).json({ error: "not found" });
  const g = await pool.query(
    "SELECT * FROM vw_guests WHERE assigned_table = $1 ORDER BY full_name",
    [req.params.id]
  );
  res.json({ table: t.rows[0], guests: g.rows });
});

const TABLE_EDITABLE = new Set(["label", "seats"]);
app.patch("/api/tables/:id", auth, async (req, res) => {
  const cols = Object.keys(req.body).filter((k) => TABLE_EDITABLE.has(k));
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields" });
  const sets = cols.map((c, i) => `${c} = $${i + 2}`).join(", ");
  const vals = cols.map((c) => req.body[c]);
  await pool.query(`UPDATE tables SET ${sets} WHERE table_id = $1`, [req.params.id, ...vals]);
  const r = await pool.query("SELECT * FROM vw_tables WHERE table_id = $1", [req.params.id]);
  res.json(r.rows[0]);
});

// --- Guests ---
app.get("/api/guests", auth, async (_req, res) => {
  const r = await pool.query("SELECT * FROM vw_guests ORDER BY full_name");
  res.json(r.rows);
});

app.get("/api/guests/:id", auth, async (req, res) => {
  const r = await pool.query("SELECT * FROM vw_guests WHERE guest_id = $1", [req.params.id]);
  if (r.rowCount === 0) return res.status(404).json({ error: "not found" });
  res.json(r.rows[0]);
});

const GUEST_EDITABLE = new Set([
  "full_name", "side", "dietary", "language", "age_band", "assigned_table",
]);
app.patch("/api/guests/:id", auth, async (req, res) => {
  const cols = Object.keys(req.body).filter((k) => GUEST_EDITABLE.has(k));
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields" });
  const sets = cols.map((c, i) => `${c} = $${i + 2}`).join(", ");
  const vals = cols.map((c) => (req.body[c] === "" ? null : req.body[c]));
  await pool.query(`UPDATE guests SET ${sets} WHERE guest_id = $1`, [req.params.id, ...vals]);
  const r = await pool.query("SELECT * FROM vw_guests WHERE guest_id = $1", [req.params.id]);
  res.json(r.rows[0]);
});

// --- Relationships ---
app.get("/api/relationships", auth, async (_req, res) => {
  const r = await pool.query("SELECT * FROM vw_relationships ORDER BY rel_id");
  res.json(r.rows);
});

const REL_EDITABLE = new Set(["kind", "weight"]);
app.patch("/api/relationships/:id", auth, async (req, res) => {
  const cols = Object.keys(req.body).filter((k) => REL_EDITABLE.has(k));
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields" });
  const sets = cols.map((c, i) => `${c} = $${i + 2}`).join(", ");
  const vals = cols.map((c) => req.body[c]);
  await pool.query(`UPDATE relationships SET ${sets} WHERE rel_id = $1`, [req.params.id, ...vals]);
  const r = await pool.query("SELECT * FROM vw_relationships WHERE rel_id = $1", [req.params.id]);
  res.json(r.rows[0]);
});

// --- Plan score (aggregate over all tables/relationships) ---
app.get("/api/plan-score", auth, async (_req, res) => {
  const r = await pool.query(
    `SELECT
       (SELECT COALESCE(SUM(happiness), 0) FROM vw_tables) AS total_happiness,
       (SELECT COUNT(*) FROM vw_tables WHERE over_capacity) AS over_capacity_count,
       (SELECT COUNT(*) FROM vw_relationships WHERE is_must_not_violation) AS must_not_violations,
       (SELECT COUNT(*) FROM vw_relationships WHERE is_satisfied) AS satisfied_pairs,
       (SELECT COUNT(*) FROM vw_relationships) AS total_pairs,
       (SELECT COUNT(*) FROM vw_guests) AS total_guests,
       (SELECT COUNT(*) FROM vw_tables) AS total_tables`
  );
  res.json(r.rows[0]);
});

app.use((err: Error, _req: Request, res: Response, _next: NextFunction) => {
  console.error(err);
  res.status(500).json({ error: err.message });
});

app.listen(PORT, () => {
  console.log(`wedding-seating-optimizer server listening on http://localhost:${PORT}`);
});
