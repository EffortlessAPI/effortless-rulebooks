import express, { Request, Response, NextFunction } from "express";
import cors from "cors";
import pg from "pg";

const PORT = Number(process.env.PORT ?? 3032);
const DATABASE_URL =
  process.env.DATABASE_URL ??
  "postgresql://postgres@localhost:5432/erb_gym_trainer_invoicing";

const pool = new pg.Pool({ connectionString: DATABASE_URL });

const app = express();
app.use(cors());
app.use(express.json());

type Me = { user_id: string; email: string; role: string; display_name: string; trainer_id?: string | null; client_id?: string | null };
declare global { namespace Express { interface Request { me?: Me } } }

app.get("/healthz", async (_req, res) => {
  const r = await pool.query("SELECT 1 AS ok");
  res.json({ ok: r.rows[0].ok === 1 });
});

// Public: list dev login identities
app.get("/api/dev-users", async (_req, res) => {
  const r = await pool.query(
    "SELECT user_id, email, role, display_name FROM vw_users ORDER BY role, display_name"
  );
  res.json(r.rows);
});

// Auth middleware — stub via X-User-Email header
async function auth(req: Request, res: Response, next: NextFunction) {
  const email = (req.header("x-user-email") ?? "").trim().toLowerCase();
  if (!email) return res.status(401).json({ error: "missing X-User-Email" });
  const u = await pool.query(
    "SELECT user_id, email, role, display_name FROM vw_users WHERE LOWER(email) = $1",
    [email]
  );
  if (u.rowCount === 0) return res.status(401).json({ error: "unknown user" });
  const me: Me = u.rows[0];
  if (me.role === "trainer") {
    const t = await pool.query("SELECT trainer_id FROM vw_trainers WHERE LOWER(email) = $1", [email]);
    me.trainer_id = t.rows[0]?.trainer_id ?? null;
  } else if (me.role === "client") {
    const c = await pool.query("SELECT client_id FROM vw_clients WHERE LOWER(email) = $1", [email]);
    me.client_id = c.rows[0]?.client_id ?? null;
  }
  req.me = me;
  next();
}

app.get("/api/me", auth, (req, res) => res.json(req.me));

// --- Trainers (read-only summaries) ---
app.get("/api/trainers", auth, async (req, res) => {
  const r = await pool.query("SELECT * FROM vw_trainers ORDER BY full_name");
  res.json(r.rows);
});

// --- Clients ---
app.get("/api/clients", auth, async (req, res) => {
  const me = req.me!;
  if (me.role === "trainer") {
    const r = await pool.query(
      "SELECT * FROM vw_clients WHERE trainer = $1 ORDER BY full_name",
      [me.trainer_id]
    );
    return res.json(r.rows);
  }
  const r = await pool.query("SELECT * FROM vw_clients ORDER BY full_name");
  res.json(r.rows);
});

app.get("/api/clients/:id", auth, async (req, res) => {
  const r = await pool.query("SELECT * FROM vw_clients WHERE client_id = $1", [req.params.id]);
  if (r.rowCount === 0) return res.status(404).json({ error: "not found" });
  res.json(r.rows[0]);
});

// --- Invoices ---
app.get("/api/invoices", auth, async (req, res) => {
  const me = req.me!;
  if (me.role === "trainer") {
    const r = await pool.query(
      "SELECT * FROM vw_invoices WHERE trainer = $1 ORDER BY issue_date DESC, invoice_id DESC",
      [me.trainer_id]
    );
    return res.json(r.rows);
  }
  const r = await pool.query("SELECT * FROM vw_invoices ORDER BY issue_date DESC, invoice_id DESC");
  res.json(r.rows);
});

app.get("/api/invoices/:id", auth, async (req, res) => {
  const r = await pool.query("SELECT * FROM vw_invoices WHERE invoice_id = $1", [req.params.id]);
  if (r.rowCount === 0) return res.status(404).json({ error: "not found" });
  const s = await pool.query(
    "SELECT * FROM vw_sessions WHERE invoice = $1 ORDER BY session_date",
    [req.params.id]
  );
  res.json({ invoice: r.rows[0], sessions: s.rows });
});

const INVOICE_EDITABLE = new Set(["issue_date", "due_date", "tax_rate", "discount_amount", "paid_amount"]);
app.patch("/api/invoices/:id", auth, async (req, res) => {
  const cols = Object.keys(req.body).filter((k) => INVOICE_EDITABLE.has(k));
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields" });
  const sets = cols.map((c, i) => `${c} = $${i + 2}`).join(", ");
  const vals = cols.map((c) => req.body[c]);
  await pool.query(`UPDATE invoices SET ${sets} WHERE invoice_id = $1`, [req.params.id, ...vals]);
  const r = await pool.query("SELECT * FROM vw_invoices WHERE invoice_id = $1", [req.params.id]);
  res.json(r.rows[0]);
});

// --- Sessions ---
app.get("/api/sessions", auth, async (req, res) => {
  const me = req.me!;
  if (me.role === "trainer") {
    const r = await pool.query(
      "SELECT * FROM vw_sessions WHERE trainer = $1 ORDER BY session_date DESC, session_id DESC",
      [me.trainer_id]
    );
    return res.json(r.rows);
  }
  const r = await pool.query("SELECT * FROM vw_sessions ORDER BY session_date DESC, session_id DESC");
  res.json(r.rows);
});

app.get("/api/sessions/:id", auth, async (req, res) => {
  const r = await pool.query("SELECT * FROM vw_sessions WHERE session_id = $1", [req.params.id]);
  if (r.rowCount === 0) return res.status(404).json({ error: "not found" });
  res.json(r.rows[0]);
});

const SESSION_EDITABLE = new Set(["session_date", "duration_hours", "rate_override", "notes", "invoice", "client"]);
app.patch("/api/sessions/:id", auth, async (req, res) => {
  const cols = Object.keys(req.body).filter((k) => SESSION_EDITABLE.has(k));
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields" });
  const sets = cols.map((c, i) => `${c} = $${i + 2}`).join(", ");
  const vals = cols.map((c) => (req.body[c] === "" ? null : req.body[c]));
  await pool.query(`UPDATE sessions SET ${sets} WHERE session_id = $1`, [req.params.id, ...vals]);
  const r = await pool.query("SELECT * FROM vw_sessions WHERE session_id = $1", [req.params.id]);
  res.json(r.rows[0]);
});

// --- Trainer settings (HourlyRate) ---
app.patch("/api/trainers/:id", auth, async (req, res) => {
  const allowed = ["hourly_rate", "full_name"];
  const cols = Object.keys(req.body).filter((k) => allowed.includes(k));
  if (cols.length === 0) return res.status(400).json({ error: "no editable fields" });
  const sets = cols.map((c, i) => `${c} = $${i + 2}`).join(", ");
  const vals = cols.map((c) => req.body[c]);
  await pool.query(`UPDATE trainers SET ${sets} WHERE trainer_id = $1`, [req.params.id, ...vals]);
  const r = await pool.query("SELECT * FROM vw_trainers WHERE trainer_id = $1", [req.params.id]);
  res.json(r.rows[0]);
});

app.use((err: Error, _req: Request, res: Response, _next: NextFunction) => {
  console.error(err);
  res.status(500).json({ error: err.message });
});

app.listen(PORT, () => {
  console.log(`gym-trainer-invoicing server listening on http://localhost:${PORT}`);
});
