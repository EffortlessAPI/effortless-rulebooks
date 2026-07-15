/**
 * bit-calculator app API.
 *
 * Doctrine (see the repo + domain CLAUDE.md): the VIEW is the contract, and the
 * gate netlist is the only oracle for arithmetic. This server NEVER computes an
 * arithmetic answer in JavaScript. Every result comes from the Postgres gate
 * engine:
 *   - GET  /api/flagship  -> SELECT * FROM vw_computation_answer WHERE is_flagship
 *   - POST /api/calc      -> insert an input-bits-only scratch `computations` row,
 *                            let erb_settle / erb_output_value settle it through the
 *                            gates, read the answer back, then delete the row.
 * If the engine can't produce a value we raise — we never paint in a fallback.
 */
import express from "express";
import cors from "cors";
import pg from "pg";

const PORT = Number(process.env.PORT) || 3040;
const DATABASE_URL =
  process.env.DATABASE_URL ||
  `postgresql://postgres@localhost:5432/${process.env.BITCALC_DB || "erb_bit_calculator"}`;

const pool = new pg.Pool({ connectionString: DATABASE_URL });

// op -> (top component, result-bus prefix). These are the fully-driven wrappers.
const OPS = {
  add: { comp: "comp-add4", prefix: "s", symbol: "+" },
  sub: { comp: "comp-sub4", prefix: "s", symbol: "−" },
  mul: { comp: "comp-mul4", prefix: "p", symbol: "×" },
  div: { comp: "comp-div4", prefix: "q", symbol: "÷" },
};
const WIDTH = 4;
const MAX = (1 << WIDTH) - 1;

const app = express();
app.use(cors());
app.use(express.json());

app.get("/healthz", (_req, res) => res.json({ ok: true }));

/** The calculator's honest width — the web app sizes its keypad from this. */
app.get("/api/config", (_req, res) => {
  res.json({ width: WIDTH, maxOperand: MAX, ops: Object.keys(OPS) });
});

/** The four flagship results, straight from the view. */
app.get("/api/flagship", async (_req, res) => {
  try {
    const { rows } = await pool.query(
      `SELECT op, a_value, b_value, result_bits, result_value, expected_value, value_ok
       FROM vw_computation_answer WHERE is_flagship = 1 ORDER BY op`
    );
    res.json(rows);
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  }
});

function toBits(n) {
  return n.toString(2).padStart(WIDTH, "0");
}

/**
 * Compute a OP b through the gate netlist. Body: { op, a, b }.
 * Returns { value, bits, op, a, b } where value/bits come ONLY from the gates.
 */
app.post("/api/calc", async (req, res) => {
  const { op } = req.body || {};
  const a = Number(req.body?.a);
  const b = Number(req.body?.b);
  const spec = OPS[op];

  if (!spec) return res.status(400).json({ error: `unknown op '${op}'` });
  if (!Number.isInteger(a) || !Number.isInteger(b) || a < 0 || b < 0 || a > MAX || b > MAX)
    return res.status(400).json({ error: `operands must be integers 0..${MAX}` });
  if (op === "div" && b === 0)
    return res.status(400).json({ error: "division by zero is undefined" });

  const cid = `scratch--${op}--${a}--${b}`;
  const client = await pool.connect();
  try {
    // 1) lay down an input-bits-only computation row (the Chinese-room input)
    await client.query(
      `INSERT INTO computations
         (computation_id, op, component_id, a_value, b_value, a_bits, b_bits,
          expected_bits, expected_value, is_flagship)
       VALUES ($1,$2,$3,$4,$5,$6,$7,'',0,0)
       ON CONFLICT (computation_id) DO UPDATE SET
         op=EXCLUDED.op, component_id=EXCLUDED.component_id,
         a_value=EXCLUDED.a_value, b_value=EXCLUDED.b_value,
         a_bits=EXCLUDED.a_bits, b_bits=EXCLUDED.b_bits`,
      [cid, op, spec.comp, a, b, toBits(a), toBits(b)]
    );

    // 2) settle it through the gates and read the answer as a WIDTH-bit register.
    //    The displayed value is the low WIDTH bits (true result mod 2^WIDTH); the
    //    gate engine sets `overflow` when the circuit produced more than the
    //    register can hold (dropped carry/borrow, or high product bits). This is
    //    what makes a 4-bit calculator honest: 15+1 wraps to 0, 3*8 wraps to 8.
    const { rows } = await client.query(
      `SELECT bits, val, overflow, full_val FROM erb_result_register($1, $2, $3)`,
      [cid, spec.prefix, WIDTH]
    );
    if (!rows.length || rows[0].val === null)
      throw new Error(`gate engine did not settle ${cid}`);

    res.json({
      op,
      symbol: spec.symbol,
      a,
      b,
      value: Number(rows[0].val),      // the WIDTH-bit register value (wrapped)
      bits: rows[0].bits,              // the WIDTH settled result bits
      overflow: rows[0].overflow,      // did the true result exceed the register?
      fullValue: Number(rows[0].full_val), // the untruncated value the gates produced
    });
  } catch (e) {
    res.status(500).json({ error: String(e.message || e) });
  } finally {
    // 3) remove the scratch row so ad-hoc evals never touch the flagship set
    try {
      await client.query(`DELETE FROM computations WHERE computation_id=$1`, [cid]);
    } catch {
      /* best-effort cleanup; the answer was already returned */
    }
    client.release();
  }
});

app.listen(PORT, () => {
  console.log(`[bitcalc-api] http://localhost:${PORT}  (db: ${DATABASE_URL})`);
});
