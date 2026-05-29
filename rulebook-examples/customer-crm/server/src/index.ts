import express, { Request, Response, NextFunction } from "express";
import path from "path";
import { fileURLToPath } from "url";
import { spawn } from "child_process";
import pg from "pg";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const PROJECT_ROOT = path.resolve(__dirname, "../..");

function spawnRefresh() {
  return spawn("node", ["scripts/refresh-rulebook.mjs"], {
    cwd: PROJECT_ROOT,
  });
}

const PORT = Number(process.env.PORT || 3032);
const DATABASE_URL =
  process.env.DATABASE_URL ||
  "postgresql://postgres@localhost:5432/erb_customer_crm";

const pool = new pg.Pool({ connectionString: DATABASE_URL });

const app = express();
app.use(express.json());

// Two-step download so the client can show progress while the (slow) remote
// rulebook-to-xlsx transpiler runs:
//   1) GET /api/rulebook.xlsx/prepare — SSE, streams the refresh script's
//      stdout/stderr line-by-line, ends with event:done or event:fail.
//   2) GET /api/rulebook.xlsx — just serves the (now-fresh) file.
app.get("/api/rulebook.xlsx/prepare", (req, res) => {
  res.setHeader("Content-Type", "text/event-stream");
  res.setHeader("Cache-Control", "no-cache");
  res.setHeader("Connection", "keep-alive");
  res.flushHeaders?.();

  const send = (event: string, data: unknown) => {
    const payload = typeof data === "string" ? data : JSON.stringify(data);
    res.write(`event: ${event}\n`);
    // Multi-line strings need every line prefixed with "data: ".
    for (const line of payload.split(/\r?\n/)) res.write(`data: ${line}\n`);
    res.write("\n");
  };

  send("log", "Starting rulebook refresh…");
  const proc = spawnRefresh();

  const onChunk = (chunk: Buffer) => {
    for (const line of chunk.toString().split(/\r?\n/)) {
      if (line.trim().length > 0) send("log", line);
    }
  };
  proc.stdout.on("data", onChunk);
  proc.stderr.on("data", onChunk);

  proc.on("exit", (code) => {
    if (code === 0) send("done", { ok: true });
    else send("fail", { code, message: `refresh-rulebook exited ${code}` });
    res.end();
  });
  proc.on("error", (err) => {
    send("fail", { message: String(err.message ?? err) });
    res.end();
  });

  req.on("close", () => {
    try { proc.kill(); } catch { /* already gone */ }
  });
});

app.get("/api/rulebook.xlsx", (_req, res) => {
  const file = path.join(PROJECT_ROOT, "xlsx/rulebook.xlsx");
  res.download(file, "customer-crm-rulebook.xlsx", (err) => {
    if (err && !res.headersSent) {
      res
        .status(404)
        .json({ error: "rulebook.xlsx not built yet — hit /api/rulebook.xlsx/prepare first" });
    }
  });
});

interface Me {
  email: string;
  first_name: string | null;
  last_name: string | null;
  full_name: string | null;
  role: string;
}
declare module "express-serve-static-core" {
  interface Request {
    me?: Me;
  }
}

app.get("/healthz", (_req, res) => res.json({ ok: true }));

// Public: dev-login picker source
app.get("/api/dev-users", async (_req, res) => {
  const { rows } = await pool.query(
    "SELECT email, first_name, last_name, full_name, role FROM vw_users ORDER BY role, email"
  );
  res.json(rows);
});

// Auth middleware
app.use(async (req, res, next) => {
  if (req.path === "/api/dev-users" || req.path === "/healthz") return next();
  const email = req.header("X-User-Email");
  if (!email) return res.status(401).json({ error: "no X-User-Email header" });
  const { rows } = await pool.query(
    "SELECT email, first_name, last_name, full_name, role FROM vw_users WHERE email = $1",
    [email]
  );
  if (!rows[0]) return res.status(401).json({ error: "unknown user" });
  req.me = rows[0];
  next();
});

app.get("/api/me", (req, res) => res.json(req.me));

// Customers
const CUSTOMER_COLS =
  "customers_id, name, email, first_name, last_name, full_name, lifetime_sales, unpaid_order_count, is_vip";

app.get("/api/customers", async (_req, res) => {
  const { rows } = await pool.query(
    `SELECT ${CUSTOMER_COLS} FROM vw_customers ORDER BY last_name, first_name`
  );
  res.json(rows);
});

app.get("/api/customers/:id", async (req, res) => {
  const { rows } = await pool.query(
    `SELECT ${CUSTOMER_COLS} FROM vw_customers WHERE customers_id = $1`,
    [req.params.id]
  );
  if (!rows[0]) return res.status(404).json({ error: "not found" });
  res.json(rows[0]);
});

// Orders
// Orders — read straight from vw_orders; customer detail is a rulebook
// lookup field on the view (no JOINs in app code).
app.get("/api/orders", async (req, res) => {
  const { customer } = req.query;
  if (typeof customer === "string" && customer.length > 0) {
    const { rows } = await pool.query(
      `SELECT * FROM vw_orders WHERE customer = $1 ORDER BY order_date DESC`,
      [customer]
    );
    return res.json(rows);
  }
  const { rows } = await pool.query(`SELECT * FROM vw_orders ORDER BY order_date DESC`);
  res.json(rows);
});

app.get("/api/orders/:id", async (req, res) => {
  const { rows } = await pool.query(
    `SELECT * FROM vw_orders WHERE orders_id = $1`,
    [req.params.id]
  );
  if (!rows[0]) return res.status(404).json({ error: "not found" });
  res.json(rows[0]);
});

app.post("/api/orders", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const { order_number, order_date, customer, total } = req.body ?? {};
  if (!order_number || !order_date || !customer || total == null) {
    return res.status(400).json({ error: "order_number, order_date, customer, total required" });
  }
  await pool.query(
    "INSERT INTO orders (orders_id, order_number, order_date, customer, total) VALUES ($1, $1, $2, $3, $4)",
    [order_number, order_date, customer, total]
  );
  const { rows } = await pool.query(`SELECT * FROM vw_orders WHERE orders_id = $1`, [order_number]);
  res.status(201).json(rows[0]);
});

app.patch("/api/orders/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const allowed = ["order_number", "order_date", "customer", "total"] as const;
  const sets: string[] = [];
  const vals: any[] = [];
  for (const k of allowed) {
    if (k in req.body) {
      vals.push(req.body[k]);
      sets.push(`${k} = $${vals.length}`);
    }
  }
  if (!sets.length) return res.status(400).json({ error: "no fields" });
  vals.push(req.params.id);
  await pool.query(
    `UPDATE orders SET ${sets.join(", ")} WHERE orders_id = $${vals.length}`,
    vals
  );
  const { rows } = await pool.query(`SELECT * FROM vw_orders WHERE orders_id = $1`, [req.params.id]);
  res.json(rows[0]);
});

app.delete("/api/orders/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  await pool.query("DELETE FROM orders WHERE orders_id = $1", [req.params.id]);
  res.status(204).end();
});

app.patch("/api/customers/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const allowed = ["email", "first_name", "last_name"] as const;
  const sets: string[] = [];
  const vals: any[] = [];
  for (const k of allowed) {
    if (k in req.body) {
      vals.push(req.body[k]);
      sets.push(`${k} = $${vals.length}`);
    }
  }
  if (!sets.length) return res.status(400).json({ error: "no fields" });
  vals.push(req.params.id);
  await pool.query(
    `UPDATE customers SET ${sets.join(", ")} WHERE customers_id = $${vals.length}`,
    vals
  );
  const { rows } = await pool.query(
    "SELECT customers_id, name, email, first_name, last_name, full_name FROM vw_customers WHERE customers_id = $1",
    [req.params.id]
  );
  res.json(rows[0]);
});

app.post("/api/customers", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const { email, first_name, last_name } = req.body ?? {};
  if (!email) return res.status(400).json({ error: "email required" });
  await pool.query(
    "INSERT INTO customers (customers_id, email, first_name, last_name) VALUES ($1, $1, $2, $3)",
    [email, first_name ?? null, last_name ?? null]
  );
  const { rows } = await pool.query(
    "SELECT customers_id, name, email, first_name, last_name, full_name FROM vw_customers WHERE customers_id = $1",
    [email]
  );
  res.status(201).json(rows[0]);
});

app.delete("/api/customers/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  await pool.query("DELETE FROM customers WHERE customers_id = $1", [req.params.id]);
  res.status(204).end();
});

// Payments — read straight from vw_payments; all order/customer detail
// lives on the view as rulebook lookup fields (no JOINs in app code).
app.get("/api/payments", async (req, res) => {
  const { order_id } = req.query;
  if (typeof order_id === "string" && order_id.length > 0) {
    const { rows } = await pool.query(
      `SELECT * FROM vw_payments WHERE order_id = $1 ORDER BY payment_date, payment_number`,
      [order_id]
    );
    return res.json(rows);
  }
  const { rows } = await pool.query(
    `SELECT * FROM vw_payments ORDER BY payment_date DESC, payment_number DESC`
  );
  res.json(rows);
});

app.get("/api/payments/:id", async (req, res) => {
  const { rows } = await pool.query(
    `SELECT * FROM vw_payments WHERE payments_id = $1`,
    [req.params.id]
  );
  if (!rows[0]) return res.status(404).json({ error: "not found" });
  res.json(rows[0]);
});

app.post("/api/payments", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const { payment_number, order_id, payment_date, amount, method } = req.body ?? {};
  if (!payment_number || !order_id || !payment_date || amount == null || !method) {
    return res
      .status(400)
      .json({ error: "payment_number, order_id, payment_date, amount, method required" });
  }
  await pool.query(
    `INSERT INTO payments (payments_id, payment_number, order_id, payment_date, amount, method)
     VALUES ($1, $1, $2, $3, $4, $5)`,
    [payment_number, order_id, payment_date, amount, method]
  );
  const { rows } = await pool.query(
    `SELECT * FROM vw_payments WHERE payments_id = $1`,
    [payment_number]
  );
  res.status(201).json(rows[0]);
});

app.patch("/api/payments/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const allowed = ["payment_number", "order_id", "payment_date", "amount", "method"] as const;
  const sets: string[] = [];
  const vals: any[] = [];
  for (const k of allowed) {
    if (k in req.body) {
      vals.push(req.body[k]);
      sets.push(`${k} = $${vals.length}`);
    }
  }
  if (!sets.length) return res.status(400).json({ error: "no fields" });
  vals.push(req.params.id);
  await pool.query(
    `UPDATE payments SET ${sets.join(", ")} WHERE payments_id = $${vals.length}`,
    vals
  );
  const { rows } = await pool.query(
    `SELECT * FROM vw_payments WHERE payments_id = $1`,
    [req.params.id]
  );
  res.json(rows[0]);
});

app.delete("/api/payments/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  await pool.query("DELETE FROM payments WHERE payments_id = $1", [req.params.id]);
  res.status(204).end();
});

// Jet models — read-only catalog
app.get("/api/jet-models", async (_req, res) => {
  const { rows } = await pool.query(
    `SELECT * FROM vw_jet_models ORDER BY total_revenue DESC NULLS LAST, name`
  );
  res.json(rows);
});

app.get("/api/jet-models/:id", async (req, res) => {
  const { rows } = await pool.query(
    `SELECT * FROM vw_jet_models WHERE jet_models_id = $1`,
    [req.params.id]
  );
  if (!rows[0]) return res.status(404).json({ error: "not found" });
  res.json(rows[0]);
});

// Flight control systems
app.get("/api/flight-control-systems", async (req, res) => {
  const { jet_model_id } = req.query;
  if (typeof jet_model_id === "string" && jet_model_id.length > 0) {
    const { rows } = await pool.query(
      `SELECT * FROM vw_flight_control_systems WHERE jet_model_id = $1 ORDER BY name`,
      [jet_model_id]
    );
    return res.json(rows);
  }
  const { rows } = await pool.query(
    `SELECT * FROM vw_flight_control_systems ORDER BY total_revenue DESC NULLS LAST, name`
  );
  res.json(rows);
});

app.get("/api/flight-control-systems/:id", async (req, res) => {
  const { rows } = await pool.query(
    `SELECT * FROM vw_flight_control_systems WHERE flight_control_systems_id = $1`,
    [req.params.id]
  );
  if (!rows[0]) return res.status(404).json({ error: "not found" });
  res.json(rows[0]);
});

app.patch("/api/flight-control-systems/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const allowed = ["architecture", "redundancy_channels", "unit_price"] as const;
  const sets: string[] = [];
  const vals: any[] = [];
  for (const k of allowed) {
    if (k in req.body) {
      vals.push(req.body[k]);
      sets.push(`${k} = $${vals.length}`);
    }
  }
  if (!sets.length) return res.status(400).json({ error: "no fields" });
  vals.push(req.params.id);
  await pool.query(
    `UPDATE flight_control_systems SET ${sets.join(", ")} WHERE flight_control_systems_id = $${vals.length}`,
    vals
  );
  const { rows } = await pool.query(
    `SELECT * FROM vw_flight_control_systems WHERE flight_control_systems_id = $1`,
    [req.params.id]
  );
  res.json(rows[0]);
});

// Order lines
app.get("/api/order-lines", async (req, res) => {
  const { order_id } = req.query;
  if (typeof order_id === "string" && order_id.length > 0) {
    const { rows } = await pool.query(
      `SELECT * FROM vw_order_lines WHERE order_id = $1 ORDER BY line_number`,
      [order_id]
    );
    return res.json(rows);
  }
  const { rows } = await pool.query(`SELECT * FROM vw_order_lines ORDER BY line_number`);
  res.json(rows);
});

app.post("/api/order-lines", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const { line_number, order_id, fcs_id, quantity } = req.body ?? {};
  if (!line_number || !order_id || !fcs_id || quantity == null) {
    return res.status(400).json({ error: "line_number, order_id, fcs_id, quantity required" });
  }
  await pool.query(
    `INSERT INTO order_lines (order_lines_id, line_number, order_id, fcs_id, quantity)
     VALUES ($1, $1, $2, $3, $4)`,
    [line_number, order_id, fcs_id, quantity]
  );
  const { rows } = await pool.query(
    `SELECT * FROM vw_order_lines WHERE order_lines_id = $1`,
    [line_number]
  );
  res.status(201).json(rows[0]);
});

app.patch("/api/order-lines/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  const allowed = ["line_number", "order_id", "fcs_id", "quantity"] as const;
  const sets: string[] = [];
  const vals: any[] = [];
  for (const k of allowed) {
    if (k in req.body) {
      vals.push(req.body[k]);
      sets.push(`${k} = $${vals.length}`);
    }
  }
  if (!sets.length) return res.status(400).json({ error: "no fields" });
  vals.push(req.params.id);
  await pool.query(
    `UPDATE order_lines SET ${sets.join(", ")} WHERE order_lines_id = $${vals.length}`,
    vals
  );
  const { rows } = await pool.query(
    `SELECT * FROM vw_order_lines WHERE order_lines_id = $1`,
    [req.params.id]
  );
  res.json(rows[0]);
});

app.delete("/api/order-lines/:id", async (req, res) => {
  if (req.me?.role !== "admin") return res.status(403).json({ error: "admin only" });
  await pool.query("DELETE FROM order_lines WHERE order_lines_id = $1", [req.params.id]);
  res.status(204).end();
});

app.use((err: any, _req: Request, res: Response, _next: NextFunction) => {
  console.error(err);
  res.status(500).json({ error: String(err?.message || err) });
});

app.listen(PORT, () => console.log(`[server] http://localhost:${PORT}`));
