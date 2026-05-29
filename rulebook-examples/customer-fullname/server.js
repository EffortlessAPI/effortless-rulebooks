const express = require('express');
const { Pool } = require('pg');

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

// The app READS from the vw_* view (raw + every calculated field, denormalized)
// and WRITES to the base table (raw facts only). It never recomputes business
// logic itself — Name/Initials come from the rulebook-derived view.
const VIEW_COLUMNS =
  'customer_id, name, email_address, initials, first_name, last_name';

// Map a vw_customers row (snake_case) to the API shape the frontend expects.
function toApi(row) {
  return {
    CustomerId: row.customer_id,
    Name: row.name,
    EmailAddress: row.email_address,
    Initials: row.initials,
    FirstName: row.first_name,
    LastName: row.last_name,
  };
}

// Only raw fields are writable. Calculated fields are derived in the view.
const WRITABLE = {
  FirstName: 'first_name',
  LastName: 'last_name',
  EmailAddress: 'email_address',
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
}

// --- API server (odd port): JSON only ---
const apiApp = express();
apiApp.use(express.json());
registerApi(apiApp);

// --- Client server (even port): static UI + same-origin API ---
const clientApp = express();
clientApp.use(express.json());
clientApp.use(express.static(__dirname, { extensions: ['html'] }));
registerApi(clientApp);

apiApp.listen(API_PORT, () => {
  console.log(`[api]    http://localhost:${API_PORT}  (JSON API)`);
});
clientApp.listen(CLIENT_PORT, () => {
  console.log(`[client] http://localhost:${CLIENT_PORT}  (open this in a browser)`);
  console.log(`Connected to ${DATABASE_URL}`);
  console.log(`Press Ctrl+C to stop`);
});
