import express from 'express';
import path from 'node:path';
import crypto from 'node:crypto';
import { fileURLToPath } from 'node:url';
import { pool } from './db.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const PORT = parseInt(process.env.PORT || '3000', 10);

// ----- editable-column schema (drives both validation & the admin UI) ------
//
// type: int | numeric | bool | timestamp | date | text | fk
// fk:   { table: 'states', label: 'name' }
//
const SCHEMA = {
  statuses: {
    label: 'Statuses',
    fields: {
      name:        { type: 'text' },
      is_blocking: { type: 'bool' }
    }
  },
  states: {
    label: 'States',
    fields: {
      name:     { type: 'text' },
      code:     { type: 'text' },
      tax_rate: { type: 'numeric' }
    }
  },
  types_of_addresses: {
    label: 'Types of Addresses',
    fields: { name: { type: 'text' } }
  },
  addresses: {
    label: 'Addresses',
    fields: {
      name:               { type: 'text' },
      street:             { type: 'text' },
      city:               { type: 'text' },
      zip:                { type: 'text' },
      state_id:           { type: 'fk', fk: { table: 'states', label: 'name' } },
      type_of_address_id: { type: 'fk', fk: { table: 'types_of_addresses', label: 'name' } }
    }
  },
  app_users: {
    label: 'App Users',
    fields: {
      name:  { type: 'text' },
      email: { type: 'text' }
    }
  },
  client_categories: {
    label: 'Client Categories',
    fields: {
      name:     { type: 'text' },
      discount: { type: 'numeric' }
    }
  },
  clients: {
    label: 'Clients',
    fields: {
      name:               { type: 'text' },
      email:              { type: 'text' },
      phone:              { type: 'text' },
      status_id:          { type: 'fk', fk: { table: 'statuses', label: 'name' } },
      client_category_id: { type: 'fk', fk: { table: 'client_categories', label: 'name' } },
      address_id:         { type: 'fk', fk: { table: 'addresses', label: 'name' } }
    }
  },
  client_approvals: {
    label: 'Client Approvals',
    fields: {
      name:           { type: 'text' },
      client_id:      { type: 'fk', fk: { table: 'clients', label: 'name' } },
      approved_by_id: { type: 'fk', fk: { table: 'app_users', label: 'name' } },
      approval_date:  { type: 'date' },
      notes:          { type: 'text' }
    }
  },
  products: {
    label: 'Products',
    fields: {
      name:        { type: 'text' },
      sku:         { type: 'text' },
      unit_price:  { type: 'numeric' },
      cost:        { type: 'numeric' },
      is_active:   { type: 'bool' },
      description: { type: 'text' }
    }
  },
  inventory_adjustments: {
    label: 'Inventory Adjustments',
    fields: {
      name:            { type: 'text' },
      product_id:      { type: 'fk', fk: { table: 'products', label: 'name' } },
      adjustment_type: { type: 'text' },     // Addition | Correction | Removal
      quantity:        { type: 'numeric' },
      adjusted_at:     { type: 'timestamp' },
      notes:           { type: 'text' }
    }
  },
  invoices: {
    label: 'Invoices',
    fields: {
      name:         { type: 'text' },
      client_id:    { type: 'fk', fk: { table: 'clients', label: 'name' } },
      invoice_date: { type: 'date' },
      tax_rate:     { type: 'numeric' },
      notes:        { type: 'text' }
    }
  },
  invoice_line_items: {
    label: 'Invoice Line Items',
    fields: {
      name:             { type: 'text' },
      invoice_id:       { type: 'fk', fk: { table: 'invoices', label: 'name' } },
      product_id:       { type: 'fk', fk: { table: 'products', label: 'name' } },
      quantity:         { type: 'numeric' },
      unit_price:       { type: 'numeric' },
      discount_percent: { type: 'numeric' }
    }
  },
  payments: {
    label: 'Payments',
    fields: {
      name:           { type: 'text' },
      invoice_id:     { type: 'fk', fk: { table: 'invoices', label: 'name' } },
      amount:         { type: 'numeric' },
      payment_status: { type: 'text' },
      payment_method: { type: 'text' },
      payment_date:   { type: 'date' }
    }
  }
};

const TABLES = Object.keys(SCHEMA);

// Default order column per table (most have a `name`).
function orderColumn(table) {
  if (table === 'inventory_adjustments') return 'airtable_id';
  return 'name';
}

// ----- coercion ------------------------------------------------------------

function coerce(value, type) {
  if (value === undefined) return undefined;
  if (value === null || value === '') return null;
  switch (type) {
    case 'int':       { const n = parseInt(value, 10); return Number.isFinite(n) ? n : null; }
    case 'numeric':   { const n = Number(value); return Number.isFinite(n) ? n : null; }
    case 'bool':      return value === true || value === 'true' || value === 1 || value === '1' || value === 'on';
    case 'timestamp': return new Date(value).toISOString();
    case 'date':      return String(value).slice(0, 10);
    case 'fk':
    case 'text':
    default:          return String(value);
  }
}

function pickEditable(table, body) {
  const out = {};
  const fields = SCHEMA[table].fields;
  for (const [k, def] of Object.entries(fields)) {
    if (k in body) {
      const v = coerce(body[k], def.type);
      if (v !== undefined) out[k] = v;
    }
  }
  return out;
}

function newAirtableId() {
  return 'rec' + crypto.randomBytes(8).toString('hex');
}

// ----- app -----------------------------------------------------------------

const app = express();
app.use(express.json({ limit: '2mb' }));

app.use((req, _res, next) => {
  console.log(new Date().toISOString(), req.method, req.url);
  next();
});

// service descriptor
app.get('/api', (_req, res) => {
  res.json({
    name: 'naked-claude-demo',
    tables: TABLES.map(t => ({ name: t, label: SCHEMA[t].label }))
  });
});

app.get('/api/_schema', (_req, res) => {
  res.json(SCHEMA);
});

app.get('/api/_schema/:table', (req, res) => {
  const t = req.params.table;
  if (!SCHEMA[t]) return res.status(404).json({ error: 'unknown table' });
  res.json(SCHEMA[t]);
});

// ----- generic CRUD per table ---------------------------------------------

function mountTable(table) {
  const view = `vw_${table}`;
  const order = orderColumn(table);

  app.get(`/api/${table}`, async (_req, res, next) => {
    try {
      const order_clause = order === 'name'
        ? 'ORDER BY name NULLS LAST'
        : `ORDER BY ${order}`;
      const r = await pool.query(`SELECT * FROM ${view} ${order_clause}`);
      res.json(r.rows);
    } catch (e) { next(e); }
  });

  app.get(`/api/${table}/:id`, async (req, res, next) => {
    try {
      const r = await pool.query(`SELECT * FROM ${view} WHERE airtable_id = $1`, [req.params.id]);
      if (!r.rows[0]) return res.status(404).json({ error: 'not found' });
      res.json(r.rows[0]);
    } catch (e) { next(e); }
  });

  app.post(`/api/${table}`, async (req, res, next) => {
    try {
      const body = pickEditable(table, req.body || {});
      const id = req.body?.airtable_id || newAirtableId();
      const cols = ['airtable_id', ...Object.keys(body), 'synced_at'];
      const vals = [id, ...Object.values(body), new Date().toISOString()];
      const placeholders = cols.map((_, i) => `$${i + 1}`).join(', ');
      await pool.query(
        `INSERT INTO ${table} (${cols.join(', ')}) VALUES (${placeholders})`,
        vals
      );
      const r = await pool.query(`SELECT * FROM ${view} WHERE airtable_id = $1`, [id]);
      res.status(201).json(r.rows[0]);
    } catch (e) { next(e); }
  });

  app.patch(`/api/${table}/:id`, async (req, res, next) => {
    try {
      const body = pickEditable(table, req.body || {});
      if (!Object.keys(body).length) return res.status(400).json({ error: 'no editable fields' });
      const sets = Object.keys(body).map((k, i) => `${k} = $${i + 1}`);
      sets.push(`synced_at = NOW()`);
      const vals = [...Object.values(body), req.params.id];
      const sql = `UPDATE ${table} SET ${sets.join(', ')} WHERE airtable_id = $${vals.length}`;
      const r = await pool.query(sql, vals);
      if (!r.rowCount) return res.status(404).json({ error: 'not found' });
      const r2 = await pool.query(`SELECT * FROM ${view} WHERE airtable_id = $1`, [req.params.id]);
      res.json(r2.rows[0]);
    } catch (e) { next(e); }
  });

  app.put(`/api/${table}/:id`, async (req, res, next) => {
    try {
      // full replace: every editable field is set, missing ones become NULL
      const fields = SCHEMA[table].fields;
      const body = req.body || {};
      const cols = Object.keys(fields);
      const vals = cols.map(k => (k in body ? coerce(body[k], fields[k].type) : null));
      const sets = cols.map((k, i) => `${k} = $${i + 1}`);
      sets.push(`synced_at = NOW()`);
      vals.push(req.params.id);
      const sql = `UPDATE ${table} SET ${sets.join(', ')} WHERE airtable_id = $${vals.length}`;
      const r = await pool.query(sql, vals);
      if (!r.rowCount) return res.status(404).json({ error: 'not found' });
      const r2 = await pool.query(`SELECT * FROM ${view} WHERE airtable_id = $1`, [req.params.id]);
      res.json(r2.rows[0]);
    } catch (e) { next(e); }
  });

  app.delete(`/api/${table}/:id`, async (req, res, next) => {
    try {
      const r = await pool.query(`DELETE FROM ${table} WHERE airtable_id = $1`, [req.params.id]);
      if (!r.rowCount) return res.status(404).json({ error: 'not found' });
      res.status(204).end();
    } catch (e) { next(e); }
  });
}

for (const t of TABLES) mountTable(t);

// ----- nested convenience endpoints ---------------------------------------

app.get('/api/clients/:id/invoices', async (req, res, next) => {
  try {
    const r = await pool.query(
      `SELECT * FROM vw_invoices WHERE client_id = $1 ORDER BY invoice_date DESC NULLS LAST, airtable_id`,
      [req.params.id]
    );
    res.json(r.rows);
  } catch (e) { next(e); }
});

app.get('/api/invoices/:id/line-items', async (req, res, next) => {
  try {
    const r = await pool.query(
      `SELECT * FROM vw_invoice_line_items WHERE invoice_id = $1 ORDER BY airtable_id`,
      [req.params.id]
    );
    res.json(r.rows);
  } catch (e) { next(e); }
});

app.get('/api/invoices/:id/payments', async (req, res, next) => {
  try {
    const r = await pool.query(
      `SELECT * FROM vw_payments WHERE invoice_id = $1 ORDER BY payment_date DESC NULLS LAST`,
      [req.params.id]
    );
    res.json(r.rows);
  } catch (e) { next(e); }
});

app.get('/api/products/:id/inventory-adjustments', async (req, res, next) => {
  try {
    const r = await pool.query(
      `SELECT * FROM vw_inventory_adjustments WHERE product_id = $1 ORDER BY adjusted_at DESC NULLS LAST`,
      [req.params.id]
    );
    res.json(r.rows);
  } catch (e) { next(e); }
});

app.get('/api/clients/:id/approvals', async (req, res, next) => {
  try {
    const r = await pool.query(
      `SELECT * FROM vw_client_approvals WHERE client_id = $1 ORDER BY approval_date DESC NULLS LAST`,
      [req.params.id]
    );
    res.json(r.rows);
  } catch (e) { next(e); }
});

// ----- static frontends ----------------------------------------------------

const publicDir = path.join(__dirname, '..', 'public');
app.use(express.static(publicDir));
app.get('/portal', (_req, res) => res.redirect('/portal/'));

// error handler
app.use((err, _req, res, _next) => {
  console.error(err);
  res.status(500).json({ error: err.message || 'internal error' });
});

app.listen(PORT, () => {
  console.log(`naked-claude-demo listening on http://localhost:${PORT}`);
});
