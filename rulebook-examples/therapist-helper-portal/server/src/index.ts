import express, { Request, Response, NextFunction } from "express";
import { Pool } from "pg";

const PORT = Number(process.env.PORT || 3032);
const pool = new Pool({
  connectionString:
    process.env.DATABASE_URL ||
    "postgresql://postgres@localhost:5432/therapist_helper_portal",
});

const app = express();
app.use(express.json());

interface Me { users_id: string; full_name: string; role: string }
declare global { namespace Express { interface Request { me?: Me } } }

app.get("/healthz", (_req, res) => res.json({ ok: true }));

app.get("/api/dev-users", async (_req, res) => {
  const { rows } = await pool.query(
    "SELECT users_id, full_name, role FROM vw_users ORDER BY role, full_name"
  );
  res.json(rows);
});

const auth = async (req: Request, res: Response, next: NextFunction) => {
  const email = req.header("x-user-email");
  if (!email) return res.status(401).json({ error: "no X-User-Email" });
  const { rows } = await pool.query(
    "SELECT users_id, full_name, role FROM vw_users WHERE users_id = $1",
    [email]
  );
  if (!rows[0]) return res.status(401).json({ error: "unknown user" });
  req.me = rows[0];
  next();
};

app.get("/api/me", auth, (req, res) => res.json(req.me));

// Therapist scope: only rows whose therapist lookup matches the caller.
function therapistWhere(me: Me, col: string, params: any[]): string {
  if (me.role !== "therapist") return "";
  params.push(me.users_id);
  return `${col} = $${params.length}`;
}

function buildWhere(parts: string[]): string {
  const live = parts.filter(Boolean);
  return live.length ? ` WHERE ${live.join(" AND ")}` : "";
}

app.get("/api/clients", auth, async (req, res) => {
  const params: any[] = [];
  const where = buildWhere([therapistWhere(req.me!, "therapist", params)]);
  const { rows } = await pool.query(
    `SELECT * FROM vw_clients${where} ORDER BY client_name`,
    params
  );
  res.json(rows);
});

app.get("/api/clients/:id", auth, async (req, res) => {
  const { rows } = await pool.query(
    "SELECT * FROM vw_clients WHERE clients_id = $1",
    [req.params.id]
  );
  if (!rows[0]) return res.status(404).json({ error: "not found" });
  res.json(rows[0]);
});

app.get("/api/goals", auth, async (req, res) => {
  const params: any[] = [];
  const parts: string[] = [];
  const clientId = (req.query.client as string) || null;
  if (clientId) { params.push(clientId); parts.push(`client = $${params.length}`); }
  parts.push(therapistWhere(req.me!, "client_therapist", params));
  const { rows } = await pool.query(`SELECT * FROM vw_goals${buildWhere(parts)} ORDER BY title`, params);
  res.json(rows);
});

app.post("/api/goals", auth, async (req, res) => {
  const client = String(req.body.client || "");
  const title = String(req.body.title || "");
  if (!client || !title) return res.status(400).json({ error: "client and title required" });
  const target = req.body.target_score != null ? Number(req.body.target_score) : null;
  const id = `goal-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
  await pool.query(
    `INSERT INTO goals (goals_id, client, title, target_score) VALUES ($1, $2, $3, $4)`,
    [id, client, title, target]
  );
  const { rows } = await pool.query("SELECT * FROM vw_goals WHERE goals_id = $1", [id]);
  res.status(201).json(rows[0]);
});

app.patch("/api/goals/:id", auth, async (req, res) => {
  const fields: Record<string, any> = {};
  if (req.body.target_score !== undefined) fields.target_score = Number(req.body.target_score);
  if (req.body.title !== undefined) fields.title = String(req.body.title);
  const keys = Object.keys(fields);
  if (!keys.length) return res.status(400).json({ error: "no fields" });
  const sets = keys.map((k, i) => `${k} = $${i + 1}`).join(", ");
  const vals = keys.map((k) => fields[k]);
  vals.push(req.params.id);
  await pool.query(`UPDATE goals SET ${sets} WHERE goals_id = $${vals.length}`, vals);
  const { rows } = await pool.query("SELECT * FROM vw_goals WHERE goals_id = $1", [req.params.id]);
  res.json(rows[0]);
});

app.get("/api/sessions", auth, async (req, res) => {
  const params: any[] = [];
  const parts: string[] = [];
  const clientId = (req.query.client as string) || null;
  if (clientId) { params.push(clientId); parts.push(`client = $${params.length}`); }
  parts.push(therapistWhere(req.me!, "client_therapist", params));
  const { rows } = await pool.query(`SELECT * FROM vw_sessions${buildWhere(parts)} ORDER BY session_label DESC`, params);
  res.json(rows);
});

app.post("/api/sessions", auth, async (req, res) => {
  const client = String(req.body.client || "");
  const session_label = String(req.body.session_label || "");
  if (!client || !session_label) {
    return res.status(400).json({ error: "client and session_label required" });
  }
  const duration = req.body.duration_minutes != null ? Number(req.body.duration_minutes) : null;
  const mood = req.body.mood_rating != null ? Number(req.body.mood_rating) : null;
  const notes = req.body.notes != null ? String(req.body.notes) : null;
  const id = `ses-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
  await pool.query(
    `INSERT INTO sessions (sessions_id, client, session_label, duration_minutes, mood_rating, notes)
     VALUES ($1, $2, $3, $4, $5, $6)`,
    [id, client, session_label, duration, mood, notes]
  );
  const { rows } = await pool.query("SELECT * FROM vw_sessions WHERE sessions_id = $1", [id]);
  res.status(201).json(rows[0]);
});

app.patch("/api/sessions/:id", auth, async (req, res) => {
  const fields: Record<string, any> = {};
  if (req.body.mood_rating !== undefined) fields.mood_rating = Number(req.body.mood_rating);
  if (req.body.duration_minutes !== undefined) fields.duration_minutes = Number(req.body.duration_minutes);
  if (req.body.notes !== undefined) fields.notes = String(req.body.notes);
  const keys = Object.keys(fields);
  if (!keys.length) return res.status(400).json({ error: "no fields" });
  const sets = keys.map((k, i) => `${k} = $${i + 1}`).join(", ");
  const vals = keys.map((k) => fields[k]);
  vals.push(req.params.id);
  await pool.query(`UPDATE sessions SET ${sets} WHERE sessions_id = $${vals.length}`, vals);
  const { rows } = await pool.query("SELECT * FROM vw_sessions WHERE sessions_id = $1", [req.params.id]);
  res.json(rows[0]);
});

app.get("/api/goal-updates", auth, async (req, res) => {
  const params: any[] = [];
  const parts: string[] = [];
  const goalId = (req.query.goal as string) || null;
  const sessionId = (req.query.session as string) || null;
  if (goalId) { params.push(goalId); parts.push(`goal = $${params.length}`); }
  if (sessionId) { params.push(sessionId); parts.push(`session = $${params.length}`); }
  parts.push(therapistWhere(req.me!, "goal_client_therapist", params));
  const { rows } = await pool.query(
    `SELECT * FROM vw_goal_updates${buildWhere(parts)} ORDER BY session_label DESC NULLS LAST, goal_updates_id`,
    params
  );
  res.json(rows);
});

app.post("/api/goal-updates", auth, async (req, res) => {
  const goal = String(req.body.goal || "");
  const session = String(req.body.session || "");
  const score = Number(req.body.score_achieved);
  if (!goal || !session || !Number.isFinite(score)) {
    return res.status(400).json({ error: "goal, session, score_achieved required" });
  }
  const id = `upd-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
  await pool.query(
    `INSERT INTO goal_updates (goal_updates_id, goal, "session", score_achieved)
     VALUES ($1, $2, $3, $4)`,
    [id, goal, session, score]
  );
  const { rows } = await pool.query("SELECT * FROM vw_goal_updates WHERE goal_updates_id = $1", [id]);
  res.status(201).json(rows[0]);
});

app.patch("/api/goal-updates/:id", auth, async (req, res) => {
  const fields: Record<string, any> = {};
  if (req.body.score_achieved !== undefined) fields.score_achieved = Number(req.body.score_achieved);
  const keys = Object.keys(fields);
  if (!keys.length) return res.status(400).json({ error: "no fields" });
  const sets = keys.map((k, i) => `${k} = $${i + 1}`).join(", ");
  const vals = keys.map((k) => fields[k]);
  vals.push(req.params.id);
  await pool.query(`UPDATE goal_updates SET ${sets} WHERE goal_updates_id = $${vals.length}`, vals);
  const { rows } = await pool.query("SELECT * FROM vw_goal_updates WHERE goal_updates_id = $1", [req.params.id]);
  res.json(rows[0]);
});

app.use((err: any, _req: Request, res: Response, _next: NextFunction) => {
  console.error(err);
  res.status(500).json({ error: String(err?.message || err) });
});

app.listen(PORT, () => console.log(`server listening on http://localhost:${PORT}`));
