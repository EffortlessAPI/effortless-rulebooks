import express from 'express';
import cors from 'cors';
import pool from './db.js';

const app = express();
const PORT = process.env.PORT || 3001;

app.use(cors());
app.use(express.json());

// ── Helpers ───────────────────────────────────────────────────────────────────

function toSlug(name) {
  return name.toLowerCase().trim().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '');
}

async function uniqueSlug(table, base, excludeId = null) {
  let slug = base;
  let i = 1;
  while (true) {
    const q = excludeId
      ? `SELECT id FROM ${table} WHERE slug = $1 AND id != $2`
      : `SELECT id FROM ${table} WHERE slug = $1`;
    const params = excludeId ? [slug, excludeId] : [slug];
    const { rows } = await pool.query(q, params);
    if (rows.length === 0) return slug;
    slug = `${base}-${i++}`;
  }
}

// ── SQL Fragments ─────────────────────────────────────────────────────────────

const CLIENT_SELECT = `
  SELECT
    c.id, c.name, c.slug, c.notes, c.status_id, c.created_at,
    c.company_name, c.email, c.phone, c.billing_address, c.shipping_address,
    c.category_id,
    s.name        AS status_name,
    s.slug        AS status_slug,
    s.description AS status_description,
    s.is_blocking AS status_is_blocking,
    COALESCE(s.is_blocking, false) AS is_stopped,
    cc.name             AS category_name,
    cc.slug             AS category_slug,
    cc.discount_percent AS category_discount
  FROM clients c
  LEFT JOIN statuses s ON c.status_id = s.id
  LEFT JOIN client_categories cc ON c.category_id = cc.id
`;

const INVOICE_SELECT = `
  WITH lt AS (
    SELECT
      invoice_id,
      COUNT(*)::int                                                 AS item_count,
      SUM(quantity)::int                                            AS total_quantity,
      SUM(quantity * unit_price * (1 - discount_percent))           AS subtotal
    FROM invoice_line_items
    GROUP BY invoice_id
  ),
  pt AS (
    SELECT
      invoice_id,
      COUNT(*)::int                                                 AS payment_count,
      COALESCE(SUM(amount) FILTER (WHERE payment_status = 'Completed'), 0)  AS total_paid,
      MAX(payment_date)                                             AS last_payment_date
    FROM payments
    GROUP BY invoice_id
  )
  SELECT
    o.id, o.invoice_number, o.slug, o.client_id,
    o.order_date, o.order_status, o.shipping_address, o.billing_address,
    o.tax_rate, o.notes, o.created_at,
    c.name AS client_name, c.slug AS client_slug,
    c.category_id                       AS client_category_id,
    cc.name                             AS client_category_name,
    cc.discount_percent                 AS client_category_discount,
    COALESCE(lt.item_count, 0)     AS item_count,
    COALESCE(lt.total_quantity, 0) AS total_quantity,
    COALESCE(lt.subtotal, 0)       AS subtotal,
    ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2) AS tax_amount,
    COALESCE(lt.subtotal, 0) + ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2) AS invoice_total,
    COALESCE(pt.total_paid, 0)     AS total_paid,
    COALESCE(lt.subtotal, 0) + ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2)
      - COALESCE(pt.total_paid, 0) AS amount_due,
    (COALESCE(lt.subtotal, 0) + ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2)
      - COALESCE(pt.total_paid, 0)) <= 0 AS is_paid_in_full,
    COALESCE(pt.payment_count, 0)  AS payment_count,
    pt.last_payment_date,
    CASE
      WHEN (COALESCE(lt.subtotal, 0) + ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2)
            - COALESCE(pt.total_paid, 0)) <= 0 THEN 'Paid'
      WHEN COALESCE(pt.total_paid, 0) = 0      THEN 'Unpaid'
      ELSE 'Partial'
    END AS payment_status_label
  FROM invoices o
  JOIN clients c ON c.id = o.client_id
  LEFT JOIN client_categories cc ON cc.id = c.category_id
  LEFT JOIN lt ON lt.invoice_id = o.id
  LEFT JOIN pt ON pt.invoice_id = o.id
`;

const LINE_ITEM_SELECT = `
  SELECT
    ili.id, ili.invoice_id, ili.line_number, ili.slug,
    ili.product_id, ili.quantity, ili.unit_price, ili.discount_percent,
    ili.notes, ili.created_at,
    p.sku          AS product_sku,
    p.display_name AS product_name,
    p.slug         AS product_slug,
    ili.quantity * ili.unit_price                              AS pre_discount,
    ili.quantity * ili.unit_price * ili.discount_percent       AS discount_amount,
    ili.quantity * ili.unit_price * (1 - ili.discount_percent) AS subtotal
  FROM invoice_line_items ili
  JOIN products p ON p.id = ili.product_id
`;

const PAYMENT_SELECT = `
  SELECT
    pay.id, pay.invoice_id, pay.payment_number, pay.slug,
    pay.payment_date, pay.amount, pay.payment_method, pay.payment_status,
    pay.transaction_id, pay.notes, pay.created_at,
    (pay.payment_status = 'Completed') AS is_completed,
    CASE WHEN pay.payment_status = 'Completed' THEN pay.amount ELSE 0 END AS completed_amount,
    o.order_date     AS invoice_date,
    o.invoice_number AS invoice_number,
    o.order_status   AS invoice_status,
    COALESCE(lt.subtotal, 0) + ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2)
      - COALESCE(pt.total_paid, 0) AS order_amount_due,
    (COALESCE(lt.subtotal, 0) + ROUND(COALESCE(lt.subtotal, 0) * o.tax_rate, 2)
      - COALESCE(pt.total_paid, 0)) <= 0 AS order_is_paid_in_full
  FROM payments pay
  JOIN invoices o ON o.id = pay.invoice_id
  LEFT JOIN (
    SELECT invoice_id,
           SUM(quantity * unit_price * (1 - discount_percent)) AS subtotal
    FROM invoice_line_items GROUP BY invoice_id
  ) lt ON lt.invoice_id = o.id
  LEFT JOIN (
    SELECT invoice_id,
           COALESCE(SUM(amount) FILTER (WHERE payment_status = 'Completed'), 0) AS total_paid
    FROM payments GROUP BY invoice_id
  ) pt ON pt.invoice_id = o.id
`;

// ── GET /api/summary ──────────────────────────────────────────────────────────

app.get('/api/summary', async (req, res) => {
  try {
    const [clientR, distR, statR, prodR, invR, payR, revR, outR, catR, usrR, aprR, invyR] = await Promise.all([
      // Clients: total & stopped
      pool.query(`
        SELECT COUNT(c.id)::int AS total,
               COUNT(c.id) FILTER (WHERE s.is_blocking = true)::int AS stopped
        FROM clients c LEFT JOIN statuses s ON c.status_id = s.id
      `),
      // Status distribution
      pool.query(`
        SELECT s.id, s.name, s.slug, s.is_blocking, s.sort_order,
               COUNT(c.id)::int AS client_count
        FROM statuses s LEFT JOIN clients c ON c.status_id = s.id
        GROUP BY s.id, s.name, s.slug, s.is_blocking, s.sort_order
        ORDER BY s.sort_order ASC
      `),
      // Statuses: total & blocking
      pool.query(`
        SELECT COUNT(*)::int AS total_statuses,
               COUNT(*) FILTER (WHERE is_blocking)::int AS blocking_statuses
        FROM statuses
      `),
      // Products: total & active
      pool.query(`
        SELECT COUNT(*)::int AS total_products,
               COUNT(*) FILTER (WHERE is_active)::int AS active_products
        FROM products
      `),
      // Invoices: total & by status
      pool.query(`
        SELECT o.order_status, COUNT(*)::int AS cnt
        FROM invoices o GROUP BY o.order_status ORDER BY o.order_status
      `),
      // Payments: total & by status, with completed dollar total
      pool.query(`
        SELECT payment_status, COUNT(*)::int AS cnt,
               SUM(amount) AS total_amount
        FROM payments GROUP BY payment_status ORDER BY payment_status
      `),
      // Total revenue (sum of all invoice totals)
      pool.query(`
        WITH lt AS (
          SELECT invoice_id, SUM(quantity * unit_price * (1 - discount_percent)) AS subtotal
          FROM invoice_line_items GROUP BY invoice_id
        )
        SELECT COALESCE(SUM(
          COALESCE(lt.subtotal,0) + ROUND(COALESCE(lt.subtotal,0) * o.tax_rate, 2)
        ), 0) AS total_revenue
        FROM invoices o LEFT JOIN lt ON lt.invoice_id = o.id
      `),
      // Outstanding (sum of amount_due across all invoices)
      pool.query(`
        WITH lt AS (
          SELECT invoice_id, SUM(quantity * unit_price * (1 - discount_percent)) AS subtotal
          FROM invoice_line_items GROUP BY invoice_id
        ),
        pt AS (
          SELECT invoice_id,
            COALESCE(SUM(amount) FILTER (WHERE payment_status = 'Completed'), 0) AS total_paid
          FROM payments GROUP BY invoice_id
        )
        SELECT COALESCE(SUM(
          COALESCE(lt.subtotal,0) + ROUND(COALESCE(lt.subtotal,0) * o.tax_rate, 2)
          - COALESCE(pt.total_paid, 0)
        ), 0) AS outstanding
        FROM invoices o
        LEFT JOIN lt ON lt.invoice_id = o.id
        LEFT JOIN pt ON pt.invoice_id = o.id
      `),
      // Client category distribution
      pool.query(`
        SELECT cc.id, cc.name, cc.slug, cc.discount_percent,
               COUNT(c.id)::int AS client_count
        FROM client_categories cc LEFT JOIN clients c ON c.category_id = cc.id
        GROUP BY cc.id, cc.name, cc.slug, cc.discount_percent
        ORDER BY cc.discount_percent ASC, cc.name ASC
      `),
      // App users by role
      pool.query(`
        SELECT role, COUNT(*)::int AS cnt
        FROM app_users GROUP BY role ORDER BY role
      `),
      // Client approvals: approved vs pending
      pool.query(`
        SELECT
          COUNT(*)::int                                           AS total_approvals,
          COUNT(*) FILTER (WHERE approved_by_user_id IS NOT NULL)::int AS approved_count,
          COUNT(*) FILTER (WHERE approved_by_user_id IS NULL)::int     AS pending_count
        FROM client_approvals
      `),
      // Inventory: total adjustments + current on-hand totals per type
      pool.query(`
        SELECT
          COUNT(*)::int AS total_adjustments,
          COALESCE(SUM(CASE WHEN adjustment_type = 'Addition'   THEN quantity ELSE 0 END), 0)::int AS total_added,
          COALESCE(SUM(CASE WHEN adjustment_type = 'Removal'    THEN quantity ELSE 0 END), 0)::int AS total_removed,
          COALESCE(SUM(CASE WHEN adjustment_type = 'Correction' THEN quantity ELSE 0 END), 0)::int AS total_corrected,
          COALESCE(SUM(CASE
                        WHEN adjustment_type = 'Addition'   THEN quantity
                        WHEN adjustment_type = 'Removal'    THEN -quantity
                        WHEN adjustment_type = 'Correction' THEN quantity
                        ELSE 0 END), 0)::int AS total_on_hand
        FROM inventory_adjustments
      `),
    ]);

    const totalInvoices = invR.rows.reduce((s, r) => s + r.cnt, 0);

    res.json({
      ...clientR.rows[0],
      status_distribution: distR.rows,
      category_distribution: catR.rows,
      total_categories: catR.rows.length,
      ...statR.rows[0],
      ...prodR.rows[0],
      total_invoices: totalInvoices,
      invoices_by_status: invR.rows,
      total_payments: payR.rows.reduce((s, r) => s + r.cnt, 0),
      payments_by_status: payR.rows,
      ...revR.rows[0],
      ...outR.rows[0],
      users_by_role: usrR.rows,
      total_users: usrR.rows.reduce((s, r) => s + r.cnt, 0),
      ...aprR.rows[0],
      ...invyR.rows[0],
    });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── Clients ───────────────────────────────────────────────────────────────────

app.get('/api/clients', async (req, res) => {
  try {
    const { rows } = await pool.query(`${CLIENT_SELECT} ORDER BY c.name ASC`);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/clients/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(`${CLIENT_SELECT} WHERE c.id = $1`, [req.params.id]);
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    // Attach invoices
    const { rows: invoices } = await pool.query(
      `${INVOICE_SELECT} WHERE o.client_id = $1 ORDER BY o.order_date DESC`,
      [req.params.id]
    );
    res.json({ ...rows[0], invoices });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

async function recomputeLineDiscountsForClient(clientId) {
  await pool.query(
    `UPDATE invoice_line_items ili
       SET discount_percent = COALESCE(cc.discount_percent, 0)
     FROM invoices inv
     LEFT JOIN clients cl ON cl.id = inv.client_id
     LEFT JOIN client_categories cc ON cc.id = cl.category_id
     WHERE ili.invoice_id = inv.id AND inv.client_id = $1`,
    [clientId]
  );
}

app.post('/api/clients', async (req, res) => {
  try {
    const { name, notes = '', status_id = null, category_id = null,
            company_name = '', email = '',
            phone = '', billing_address = '', shipping_address = '' } = req.body;
    if (!name?.trim()) return res.status(400).json({ error: 'Name is required' });
    const slug = await uniqueSlug('clients', toSlug(name));
    const { rows: ins } = await pool.query(
      `INSERT INTO clients (name, slug, notes, status_id, category_id, company_name, email, phone, billing_address, shipping_address)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10) RETURNING id`,
      [name.trim(), slug, notes, status_id || null, category_id || null,
       company_name, email, phone, billing_address, shipping_address]
    );
    const { rows } = await pool.query(`${CLIENT_SELECT} WHERE c.id = $1`, [ins[0].id]);
    res.status(201).json({ ...rows[0], invoices: [] });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/clients/:id', async (req, res) => {
  try {
    const { name, notes, status_id, category_id = null, company_name = '', email = '',
            phone = '', billing_address = '', shipping_address = '' } = req.body;
    if (!name?.trim()) return res.status(400).json({ error: 'Name is required' });
    const slug = await uniqueSlug('clients', toSlug(name), req.params.id);
    const { rowCount } = await pool.query(
      `UPDATE clients SET name=$1, slug=$2, notes=$3, status_id=$4, category_id=$5,
         company_name=$6, email=$7, phone=$8, billing_address=$9, shipping_address=$10
       WHERE id=$11`,
      [name.trim(), slug, notes ?? '', status_id || null, category_id || null,
       company_name, email, phone, billing_address, shipping_address, req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    // Category change → re-derive discounts for all this client's line items
    await recomputeLineDiscountsForClient(req.params.id);
    const { rows } = await pool.query(`${CLIENT_SELECT} WHERE c.id = $1`, [req.params.id]);
    const { rows: invoices } = await pool.query(
      `${INVOICE_SELECT} WHERE o.client_id = $1 ORDER BY o.order_date DESC`, [req.params.id]
    );
    res.json({ ...rows[0], invoices });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/clients/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT COUNT(*)::int AS cnt FROM invoices WHERE client_id = $1', [req.params.id]
    );
    if (rows[0].cnt > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${rows[0].cnt} invoice(s) belong to this client. Delete or reassign them first.`
      });
    }
    const { rowCount } = await pool.query('DELETE FROM clients WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Client Categories ─────────────────────────────────────────────────────────

const CLIENT_CATEGORY_SELECT = `
  SELECT cc.id, cc.name, cc.slug, cc.notes, cc.discount_percent, cc.created_at,
         COUNT(c.id)::int AS client_count
  FROM client_categories cc
  LEFT JOIN clients c ON c.category_id = cc.id
`;

app.get('/api/client-categories', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${CLIENT_CATEGORY_SELECT} GROUP BY cc.id ORDER BY cc.discount_percent ASC, cc.name ASC`
    );
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/client-categories/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${CLIENT_CATEGORY_SELECT} WHERE cc.id = $1 GROUP BY cc.id`, [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    const { rows: clients } = await pool.query(
      `SELECT id, name, slug, notes, company_name, email
       FROM clients WHERE category_id = $1 ORDER BY name`, [req.params.id]
    );
    res.json({ ...rows[0], clients });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/client-categories', async (req, res) => {
  try {
    const { name, notes = '', discount_percent = 0 } = req.body;
    if (!name?.trim()) return res.status(400).json({ error: 'Name is required' });
    const slug = await uniqueSlug('client_categories', toSlug(name));
    const { rows } = await pool.query(
      `INSERT INTO client_categories (name, slug, notes, discount_percent)
       VALUES ($1,$2,$3,$4) RETURNING *`,
      [name.trim(), slug, notes, discount_percent || 0]
    );
    res.status(201).json({ ...rows[0], client_count: 0, clients: [] });
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A category with that name already exists' });
    console.error(err); res.status(500).json({ error: err.message });
  }
});

app.put('/api/client-categories/:id', async (req, res) => {
  try {
    const { name, notes, discount_percent } = req.body;
    if (!name?.trim()) return res.status(400).json({ error: 'Name is required' });
    const slug = await uniqueSlug('client_categories', toSlug(name), req.params.id);
    const { rowCount } = await pool.query(
      `UPDATE client_categories SET name=$1, slug=$2, notes=$3, discount_percent=$4 WHERE id=$5`,
      [name.trim(), slug, notes ?? '', discount_percent ?? 0, req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    // Re-derive line-item discounts for every client in this category
    await pool.query(
      `UPDATE invoice_line_items ili
         SET discount_percent = COALESCE(cc.discount_percent, 0)
       FROM invoices inv
       JOIN clients cl ON cl.id = inv.client_id
       JOIN client_categories cc ON cc.id = cl.category_id
       WHERE ili.invoice_id = inv.id AND cl.category_id = $1`,
      [req.params.id]
    );
    const { rows } = await pool.query(
      `${CLIENT_CATEGORY_SELECT} WHERE cc.id = $1 GROUP BY cc.id`, [req.params.id]
    );
    const { rows: clients } = await pool.query(
      `SELECT id, name, slug, notes, company_name, email
       FROM clients WHERE category_id = $1 ORDER BY name`, [req.params.id]
    );
    res.json({ ...rows[0], clients });
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A category with that name already exists' });
    console.error(err); res.status(500).json({ error: err.message });
  }
});

app.delete('/api/client-categories/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT COUNT(*)::int AS cnt FROM clients WHERE category_id = $1', [req.params.id]
    );
    if (rows[0].cnt > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${rows[0].cnt} client(s) are assigned to this category. Reassign them first.`
      });
    }
    const { rowCount } = await pool.query('DELETE FROM client_categories WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Statuses ──────────────────────────────────────────────────────────────────

async function fetchStatusRow(id) {
  const { rows } = await pool.query(`
    SELECT s.id, s.name, s.slug, s.description, s.is_blocking, s.sort_order, s.created_at,
           COUNT(c.id)::int AS client_count
    FROM statuses s LEFT JOIN clients c ON c.status_id = s.id
    WHERE s.id = $1
    GROUP BY s.id
  `, [id]);
  return rows[0] || null;
}

app.get('/api/statuses', async (req, res) => {
  try {
    const { rows } = await pool.query(`
      SELECT s.id, s.name, s.slug, s.description, s.is_blocking, s.sort_order, s.created_at,
             COUNT(c.id)::int AS client_count
      FROM statuses s LEFT JOIN clients c ON c.status_id = s.id
      GROUP BY s.id ORDER BY s.sort_order ASC, s.name ASC
    `);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/statuses/:id', async (req, res) => {
  try {
    const sr = await fetchStatusRow(req.params.id);
    if (!sr) return res.status(404).json({ error: 'Not found' });
    const { rows: clients } = await pool.query(
      `SELECT id, name, slug, notes FROM clients WHERE status_id = $1 ORDER BY name`, [req.params.id]
    );
    res.json({ ...sr, clients });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/statuses', async (req, res) => {
  try {
    const { name, description = '', is_blocking = false, sort_order = 0 } = req.body;
    if (!name?.trim()) return res.status(400).json({ error: 'Name is required' });
    const slug = toSlug(name);
    const { rows } = await pool.query(
      `INSERT INTO statuses (name, slug, description, is_blocking, sort_order)
       VALUES ($1,$2,$3,$4,$5) RETURNING *`,
      [name.trim(), slug, description, Boolean(is_blocking), sort_order]
    );
    res.status(201).json({ ...rows[0], client_count: 0, clients: [] });
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A status with that name already exists' });
    console.error(err); res.status(500).json({ error: err.message });
  }
});

app.put('/api/statuses/:id', async (req, res) => {
  try {
    const { name, description, is_blocking, sort_order } = req.body;
    if (!name?.trim()) return res.status(400).json({ error: 'Name is required' });
    const slug = toSlug(name);
    const { rowCount } = await pool.query(
      `UPDATE statuses SET name=$1, slug=$2, description=$3, is_blocking=$4, sort_order=$5 WHERE id=$6`,
      [name.trim(), slug, description ?? '', Boolean(is_blocking), sort_order ?? 0, req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const sr = await fetchStatusRow(req.params.id);
    const { rows: clients } = await pool.query(
      `SELECT id, name, slug, notes FROM clients WHERE status_id = $1 ORDER BY name`, [req.params.id]
    );
    res.json({ ...sr, clients });
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A status with that name already exists' });
    console.error(err); res.status(500).json({ error: err.message });
  }
});

app.delete('/api/statuses/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT COUNT(*)::int AS count FROM clients WHERE status_id = $1', [req.params.id]
    );
    if (rows[0].count > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${rows[0].count} client(s) are assigned to this status. Reassign them first.`
      });
    }
    const { rowCount } = await pool.query('DELETE FROM statuses WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Products ──────────────────────────────────────────────────────────────────

// Extended product select: adds profit / margin / is_high_margin / cogs /
// stock_on_hand (derived from inventory_adjustments) / line_item_count.
const PRODUCT_SELECT = `
  SELECT
    p.*,
    (p.unit_price - p.cost)                                         AS profit,
    CASE WHEN p.unit_price > 0
         THEN (1 - (p.cost::numeric / NULLIF(p.unit_price, 0)))
         ELSE 0
    END                                                             AS margin,
    CASE WHEN p.unit_price > 0
         THEN (1 - (p.cost::numeric / NULLIF(p.unit_price, 0))) > 0.65
         ELSE FALSE
    END                                                             AS is_high_margin,
    COALESCE(ia.stock_on_hand, 0)                                   AS stock_on_hand,
    COALESCE(ia.adjustment_count, 0)                                AS adjustment_count,
    COALESCE(sold.total_sold_qty, 0)                                AS total_sold_qty,
    (p.cost * COALESCE(sold.total_sold_qty, 0))                     AS cogs,
    COUNT(ili.id)::int                                              AS line_item_count
  FROM products p
  LEFT JOIN invoice_line_items ili ON ili.product_id = p.id
  LEFT JOIN (
    SELECT product_id,
           COUNT(*)::int AS adjustment_count,
           SUM(CASE WHEN adjustment_type = 'Addition'   THEN quantity
                    WHEN adjustment_type = 'Removal'    THEN -quantity
                    WHEN adjustment_type = 'Correction' THEN quantity
                    ELSE 0 END)::int AS stock_on_hand
    FROM inventory_adjustments
    GROUP BY product_id
  ) ia ON ia.product_id = p.id
  LEFT JOIN (
    SELECT product_id, SUM(quantity)::int AS total_sold_qty
    FROM invoice_line_items GROUP BY product_id
  ) sold ON sold.product_id = p.id
`;

app.get('/api/products', async (req, res) => {
  try {
    const { rows } = await pool.query(`
      ${PRODUCT_SELECT}
      GROUP BY p.id, ia.stock_on_hand, ia.adjustment_count, sold.total_sold_qty
      ORDER BY p.sku ASC
    `);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/products/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${PRODUCT_SELECT}
       WHERE p.id = $1
       GROUP BY p.id, ia.stock_on_hand, ia.adjustment_count, sold.total_sold_qty`,
      [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    const [{ rows: lineItems }, { rows: adjustments }] = await Promise.all([
      pool.query(`${LINE_ITEM_SELECT} WHERE ili.product_id = $1 ORDER BY ili.id DESC`, [req.params.id]),
      pool.query(
        `SELECT id, slug, product_id, adjustment_date, adjustment_type, quantity,
                reason, adjusted_by, notes, created_at
         FROM inventory_adjustments
         WHERE product_id = $1
         ORDER BY adjustment_date DESC, id DESC`, [req.params.id]
      ),
    ]);
    res.json({ ...rows[0], line_items: lineItems, inventory_adjustments: adjustments });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/products', async (req, res) => {
  try {
    const { sku, display_name = '', description = '', unit_price = 0,
            cost = 0, stock_quantity = null, is_active = true } = req.body;
    if (!sku?.trim()) return res.status(400).json({ error: 'SKU is required' });
    const slug = toSlug(sku);
    const { rows } = await pool.query(
      `INSERT INTO products (sku, slug, display_name, description, unit_price, cost, stock_quantity, is_active)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8) RETURNING *`,
      [sku.trim().toUpperCase(), slug, display_name, description, unit_price, cost,
       stock_quantity === '' ? null : stock_quantity, Boolean(is_active)]
    );
    res.status(201).json({ ...rows[0], line_item_count: 0, line_items: [] });
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A product with that SKU already exists' });
    console.error(err); res.status(500).json({ error: err.message });
  }
});

app.put('/api/products/:id', async (req, res) => {
  try {
    const { sku, display_name, description, unit_price, cost, stock_quantity, is_active } = req.body;
    if (!sku?.trim()) return res.status(400).json({ error: 'SKU is required' });
    const slug = toSlug(sku);
    const { rowCount } = await pool.query(
      `UPDATE products SET sku=$1, slug=$2, display_name=$3, description=$4,
         unit_price=$5, cost=$6, stock_quantity=$7, is_active=$8 WHERE id=$9`,
      [sku.trim().toUpperCase(), slug, display_name ?? '', description ?? '',
       unit_price ?? 0, cost ?? 0,
       stock_quantity === '' || stock_quantity === undefined ? null : stock_quantity,
       Boolean(is_active), req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const { rows } = await pool.query(
      `SELECT p.*, COUNT(ili.id)::int AS line_item_count
       FROM products p LEFT JOIN invoice_line_items ili ON ili.product_id = p.id
       WHERE p.id = $1 GROUP BY p.id`, [req.params.id]
    );
    res.json(rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A product with that SKU already exists' });
    console.error(err); res.status(500).json({ error: err.message });
  }
});

app.delete('/api/products/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      'SELECT COUNT(*)::int AS cnt FROM invoice_line_items WHERE product_id = $1', [req.params.id]
    );
    if (rows[0].cnt > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${rows[0].cnt} line item(s) reference this product. Mark it inactive instead, or delete the line items first.`
      });
    }
    const { rowCount } = await pool.query('DELETE FROM products WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Invoices ──────────────────────────────────────────────────────────────────

app.get('/api/invoices', async (req, res) => {
  try {
    const { rows } = await pool.query(`${INVOICE_SELECT} ORDER BY o.order_date DESC NULLS LAST, o.id DESC`);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/invoices/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(`${INVOICE_SELECT} WHERE o.id = $1`, [req.params.id]);
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    const [{ rows: lineItems }, { rows: payments }] = await Promise.all([
      pool.query(`${LINE_ITEM_SELECT} WHERE ili.invoice_id = $1 ORDER BY ili.line_number`, [req.params.id]),
      pool.query(`${PAYMENT_SELECT} WHERE pay.invoice_id = $1 ORDER BY pay.payment_number`, [req.params.id]),
    ]);
    res.json({ ...rows[0], line_items: lineItems, payments });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/invoices', async (req, res) => {
  try {
    const { invoice_number, client_id, order_date = null, order_status = 'New',
            shipping_address = '', billing_address = '', tax_rate = 0, notes = '' } = req.body;
    if (!client_id) return res.status(400).json({ error: 'Client is required' });
    if (!invoice_number?.trim()) return res.status(400).json({ error: 'Invoice number is required' });
    // Build slug from client slug + invoice number
    const { rows: crows } = await pool.query('SELECT slug FROM clients WHERE id = $1', [client_id]);
    if (!crows.length) return res.status(400).json({ error: 'Client not found' });
    const baseSlug = `${crows[0].slug}-${toSlug(invoice_number)}`;
    const slug = await uniqueSlug('invoices', baseSlug);
    const { rows: ins } = await pool.query(
      `INSERT INTO invoices (invoice_number, slug, client_id, order_date, order_status, shipping_address, billing_address, tax_rate, notes)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9) RETURNING id`,
      [invoice_number.trim(), slug, client_id, order_date || null, order_status,
       shipping_address, billing_address, tax_rate || 0, notes]
    );
    const { rows } = await pool.query(`${INVOICE_SELECT} WHERE o.id = $1`, [ins[0].id]);
    res.status(201).json({ ...rows[0], line_items: [], payments: [] });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/invoices/:id', async (req, res) => {
  try {
    const { invoice_number, client_id, order_date, order_status,
            shipping_address, billing_address, tax_rate, notes } = req.body;
    if (!client_id) return res.status(400).json({ error: 'Client is required' });
    if (!invoice_number?.trim()) return res.status(400).json({ error: 'Invoice number is required' });
    const { rows: crows } = await pool.query('SELECT slug FROM clients WHERE id = $1', [client_id]);
    if (!crows.length) return res.status(400).json({ error: 'Client not found' });
    const baseSlug = `${crows[0].slug}-${toSlug(invoice_number)}`;
    const slug = await uniqueSlug('invoices', baseSlug, req.params.id);
    const { rowCount } = await pool.query(
      `UPDATE invoices SET invoice_number=$1, slug=$2, client_id=$3, order_date=$4, order_status=$5,
         shipping_address=$6, billing_address=$7, tax_rate=$8, notes=$9 WHERE id=$10`,
      [invoice_number.trim(), slug, client_id, order_date || null, order_status ?? 'New',
       shipping_address ?? '', billing_address ?? '', tax_rate ?? 0, notes ?? '', req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    // Client may have changed → re-derive every line item's discount
    const newDiscount = await invoiceCategoryDiscount(req.params.id);
    await pool.query(
      'UPDATE invoice_line_items SET discount_percent = $1 WHERE invoice_id = $2',
      [newDiscount, req.params.id]
    );
    const { rows } = await pool.query(`${INVOICE_SELECT} WHERE o.id = $1`, [req.params.id]);
    const [{ rows: lineItems }, { rows: payments }] = await Promise.all([
      pool.query(`${LINE_ITEM_SELECT} WHERE ili.invoice_id = $1 ORDER BY ili.line_number`, [req.params.id]),
      pool.query(`${PAYMENT_SELECT} WHERE pay.invoice_id = $1 ORDER BY pay.payment_number`, [req.params.id]),
    ]);
    res.json({ ...rows[0], line_items: lineItems, payments });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/invoices/:id', async (req, res) => {
  try {
    // Cascade deletes line items and payments (ON DELETE CASCADE)
    const { rowCount } = await pool.query('DELETE FROM invoices WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Invoice Line Items ────────────────────────────────────────────────────────

app.get('/api/invoice-line-items', async (req, res) => {
  try {
    const { invoice_id } = req.query;
    const q = invoice_id
      ? `${LINE_ITEM_SELECT} WHERE ili.invoice_id = $1 ORDER BY ili.line_number`
      : `${LINE_ITEM_SELECT} ORDER BY ili.invoice_id, ili.line_number`;
    const params = invoice_id ? [invoice_id] : [];
    const { rows } = await pool.query(q, params);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/invoice-line-items/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(`${LINE_ITEM_SELECT} WHERE ili.id = $1`, [req.params.id]);
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// Look up the client-category discount for a given invoice. Used on create/update
// of line items so discount_percent is always synchronised with the category —
// matching Airtable's lookup-only model.
async function invoiceCategoryDiscount(invoiceId) {
  const { rows } = await pool.query(
    `SELECT COALESCE(cc.discount_percent, 0) AS discount
       FROM invoices inv
       LEFT JOIN clients cl ON cl.id = inv.client_id
       LEFT JOIN client_categories cc ON cc.id = cl.category_id
       WHERE inv.id = $1`,
    [invoiceId]
  );
  return rows.length ? Number(rows[0].discount) : 0;
}

app.post('/api/invoice-line-items', async (req, res) => {
  try {
    const { invoice_id, product_id, quantity = 1, unit_price = 0, notes = '' } = req.body;
    if (!invoice_id) return res.status(400).json({ error: 'Invoice is required' });
    if (!product_id) return res.status(400).json({ error: 'Product is required' });
    // Determine next line number
    const { rows: maxR } = await pool.query(
      'SELECT COALESCE(MAX(line_number), 0) + 1 AS next FROM invoice_line_items WHERE invoice_id = $1', [invoice_id]
    );
    const lineNumber = maxR[0].next;
    // Build slug from invoice slug
    const { rows: orows } = await pool.query('SELECT slug FROM invoices WHERE id = $1', [invoice_id]);
    if (!orows.length) return res.status(400).json({ error: 'Invoice not found' });
    const slug = `${orows[0].slug}-line-${lineNumber}`;
    const discountPercent = await invoiceCategoryDiscount(invoice_id);
    const { rows: ins } = await pool.query(
      `INSERT INTO invoice_line_items (invoice_id, line_number, slug, product_id, quantity, unit_price, discount_percent, notes)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8) RETURNING id`,
      [invoice_id, lineNumber, slug, product_id, quantity, unit_price, discountPercent, notes]
    );
    const { rows } = await pool.query(`${LINE_ITEM_SELECT} WHERE ili.id = $1`, [ins[0].id]);
    res.status(201).json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/invoice-line-items/:id', async (req, res) => {
  try {
    const { product_id, quantity, unit_price, notes } = req.body;
    if (!product_id) return res.status(400).json({ error: 'Product is required' });
    // Re-derive discount_percent from the invoice's client's category
    const { rows: liR } = await pool.query(
      'SELECT invoice_id FROM invoice_line_items WHERE id = $1', [req.params.id]
    );
    if (!liR.length) return res.status(404).json({ error: 'Not found' });
    const discountPercent = await invoiceCategoryDiscount(liR[0].invoice_id);
    const { rowCount } = await pool.query(
      `UPDATE invoice_line_items SET product_id=$1, quantity=$2, unit_price=$3, discount_percent=$4, notes=$5 WHERE id=$6`,
      [product_id, quantity ?? 1, unit_price ?? 0, discountPercent, notes ?? '', req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const { rows } = await pool.query(`${LINE_ITEM_SELECT} WHERE ili.id = $1`, [req.params.id]);
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/invoice-line-items/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query('DELETE FROM invoice_line_items WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Payments ──────────────────────────────────────────────────────────────────

app.get('/api/payments', async (req, res) => {
  try {
    const { invoice_id } = req.query;
    const q = invoice_id
      ? `${PAYMENT_SELECT} WHERE pay.invoice_id = $1 ORDER BY pay.payment_number`
      : `${PAYMENT_SELECT} ORDER BY pay.payment_date DESC NULLS LAST, pay.id DESC`;
    const params = invoice_id ? [invoice_id] : [];
    const { rows } = await pool.query(q, params);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/payments/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(`${PAYMENT_SELECT} WHERE pay.id = $1`, [req.params.id]);
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/payments', async (req, res) => {
  try {
    const { invoice_id, payment_date = null, amount = 0, payment_method = '',
            payment_status = 'Pending', transaction_id = '', notes = '' } = req.body;
    if (!invoice_id) return res.status(400).json({ error: 'Invoice is required' });
    if (!amount || parseFloat(amount) === 0) return res.status(400).json({ error: 'Amount is required' });
    const { rows: maxR } = await pool.query(
      'SELECT COALESCE(MAX(payment_number), 0) + 1 AS next FROM payments WHERE invoice_id = $1', [invoice_id]
    );
    const payNumber = maxR[0].next;
    const { rows: orows } = await pool.query('SELECT slug FROM invoices WHERE id = $1', [invoice_id]);
    if (!orows.length) return res.status(400).json({ error: 'Invoice not found' });
    const slug = `${orows[0].slug}-pmt-${payNumber}`;
    const { rows: ins } = await pool.query(
      `INSERT INTO payments (invoice_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id, notes)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9) RETURNING id`,
      [invoice_id, payNumber, slug, payment_date || null, amount, payment_method,
       payment_status, transaction_id, notes]
    );
    const { rows } = await pool.query(`${PAYMENT_SELECT} WHERE pay.id = $1`, [ins[0].id]);
    res.status(201).json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/payments/:id', async (req, res) => {
  try {
    const { payment_date, amount, payment_method, payment_status, transaction_id, notes } = req.body;
    const { rowCount } = await pool.query(
      `UPDATE payments SET payment_date=$1, amount=$2, payment_method=$3, payment_status=$4,
         transaction_id=$5, notes=$6 WHERE id=$7`,
      [payment_date || null, amount ?? 0, payment_method ?? '', payment_status ?? 'Pending',
       transaction_id ?? '', notes ?? '', req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const { rows } = await pool.query(`${PAYMENT_SELECT} WHERE pay.id = $1`, [req.params.id]);
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/payments/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query('DELETE FROM payments WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── App Users ─────────────────────────────────────────────────────────────────

const APP_USER_SELECT = `
  SELECT au.id, au.name, au.slug, au.contact_name, au.email, au.phone,
         au.role, au.notes, au.created_at,
         COUNT(ca.id)::int AS approval_count
  FROM app_users au
  LEFT JOIN client_approvals ca ON ca.approved_by_user_id = au.id
`;

app.get('/api/app-users', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${APP_USER_SELECT} GROUP BY au.id ORDER BY au.role, au.contact_name`
    );
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/app-users/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${APP_USER_SELECT} WHERE au.id = $1 GROUP BY au.id`, [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    const { rows: approvals } = await pool.query(
      `SELECT ca.id, ca.slug, ca.client_id, ca.approved_by_user_id IS NOT NULL AS is_approved,
              ca.notes, ca.created_at, c.name AS client_name, c.slug AS client_slug
       FROM client_approvals ca JOIN clients c ON c.id = ca.client_id
       WHERE ca.approved_by_user_id = $1 ORDER BY ca.created_at DESC`, [req.params.id]
    );
    res.json({ ...rows[0], approvals });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

function buildAppUserSlug(contact_name, email) {
  const base = `${(contact_name || '').toLowerCase()}-${(email || '').toLowerCase().replace('@', '-')}`;
  return toSlug(base) || 'user';
}

app.post('/api/app-users', async (req, res) => {
  try {
    const { contact_name = '', email = '', phone = '', role = 'Customer', notes = '' } = req.body;
    if (!contact_name.trim() && !email.trim()) {
      return res.status(400).json({ error: 'Contact name or email is required' });
    }
    if (!['Admin', 'Manager', 'Customer'].includes(role)) {
      return res.status(400).json({ error: 'Role must be Admin, Manager, or Customer' });
    }
    const base = buildAppUserSlug(contact_name, email);
    const slug = await uniqueSlug('app_users', base);
    const { rows } = await pool.query(
      `INSERT INTO app_users (name, slug, contact_name, email, phone, role, notes)
       VALUES ($1,$1,$2,$3,$4,$5,$6) RETURNING *`,
      [slug, contact_name, email, phone, role, notes]
    );
    res.status(201).json({ ...rows[0], approval_count: 0, approvals: [] });
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/app-users/:id', async (req, res) => {
  try {
    const { contact_name = '', email = '', phone = '', role = 'Customer', notes = '' } = req.body;
    if (!['Admin', 'Manager', 'Customer'].includes(role)) {
      return res.status(400).json({ error: 'Role must be Admin, Manager, or Customer' });
    }
    const base = buildAppUserSlug(contact_name, email);
    const slug = await uniqueSlug('app_users', base, req.params.id);
    const { rowCount } = await pool.query(
      `UPDATE app_users SET name=$1, slug=$1, contact_name=$2, email=$3, phone=$4, role=$5, notes=$6
       WHERE id=$7`,
      [slug, contact_name, email, phone, role, notes, req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const { rows } = await pool.query(
      `${APP_USER_SELECT} WHERE au.id = $1 GROUP BY au.id`, [req.params.id]
    );
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/app-users/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query('DELETE FROM app_users WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Client Approvals ──────────────────────────────────────────────────────────

const CLIENT_APPROVAL_SELECT = `
  SELECT
    ca.id, ca.slug, ca.client_id, ca.approved_by_user_id, ca.notes, ca.created_at,
    (ca.approved_by_user_id IS NOT NULL)  AS is_approved,
    c.name  AS client_name,
    c.slug  AS client_slug,
    c.email AS client_email,
    au.contact_name AS approver_name,
    au.email        AS approver_email,
    au.role         AS approver_role
  FROM client_approvals ca
  JOIN clients  c  ON c.id  = ca.client_id
  LEFT JOIN app_users au ON au.id = ca.approved_by_user_id
`;

app.get('/api/client-approvals', async (req, res) => {
  try {
    const { client_id } = req.query;
    const q = client_id
      ? `${CLIENT_APPROVAL_SELECT} WHERE ca.client_id = $1 ORDER BY ca.created_at DESC`
      : `${CLIENT_APPROVAL_SELECT} ORDER BY ca.created_at DESC`;
    const params = client_id ? [client_id] : [];
    const { rows } = await pool.query(q, params);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/client-approvals/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${CLIENT_APPROVAL_SELECT} WHERE ca.id = $1`, [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/client-approvals', async (req, res) => {
  try {
    const { client_id, approved_by_user_id = null, notes = '' } = req.body;
    if (!client_id) return res.status(400).json({ error: 'Client is required' });
    const { rows: crows } = await pool.query('SELECT slug FROM clients WHERE id = $1', [client_id]);
    if (!crows.length) return res.status(400).json({ error: 'Client not found' });
    const { rows: countR } = await pool.query(
      'SELECT COUNT(*)::int + 1 AS next FROM client_approvals WHERE client_id = $1', [client_id]
    );
    const slug = await uniqueSlug('client_approvals', `${crows[0].slug}-apvl-${countR[0].next}`);
    const { rows: ins } = await pool.query(
      `INSERT INTO client_approvals (slug, client_id, approved_by_user_id, notes)
       VALUES ($1,$2,$3,$4) RETURNING id`,
      [slug, client_id, approved_by_user_id || null, notes]
    );
    const { rows } = await pool.query(
      `${CLIENT_APPROVAL_SELECT} WHERE ca.id = $1`, [ins[0].id]
    );
    res.status(201).json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/client-approvals/:id', async (req, res) => {
  try {
    const { approved_by_user_id = null, notes = '' } = req.body;
    const { rowCount } = await pool.query(
      `UPDATE client_approvals SET approved_by_user_id=$1, notes=$2 WHERE id=$3`,
      [approved_by_user_id || null, notes, req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const { rows } = await pool.query(
      `${CLIENT_APPROVAL_SELECT} WHERE ca.id = $1`, [req.params.id]
    );
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/client-approvals/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query('DELETE FROM client_approvals WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Inventory Adjustments ─────────────────────────────────────────────────────

const INVENTORY_SELECT = `
  SELECT
    ia.id, ia.slug, ia.product_id, ia.adjustment_date, ia.adjustment_type,
    ia.quantity, ia.reason, ia.adjusted_by, ia.notes, ia.created_at,
    p.sku          AS product_sku,
    p.display_name AS product_name,
    p.slug         AS product_slug,
    CASE WHEN ia.adjustment_type = 'Addition'   THEN  ia.quantity
         WHEN ia.adjustment_type = 'Removal'    THEN -ia.quantity
         WHEN ia.adjustment_type = 'Correction' THEN  ia.quantity
         ELSE 0
    END AS signed_quantity
  FROM inventory_adjustments ia
  JOIN products p ON p.id = ia.product_id
`;

app.get('/api/inventory-adjustments', async (req, res) => {
  try {
    const { product_id } = req.query;
    const q = product_id
      ? `${INVENTORY_SELECT} WHERE ia.product_id = $1 ORDER BY ia.adjustment_date DESC, ia.id DESC`
      : `${INVENTORY_SELECT} ORDER BY ia.adjustment_date DESC, ia.id DESC`;
    const params = product_id ? [product_id] : [];
    const { rows } = await pool.query(q, params);
    res.json(rows);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.get('/api/inventory-adjustments/:id', async (req, res) => {
  try {
    const { rows } = await pool.query(
      `${INVENTORY_SELECT} WHERE ia.id = $1`, [req.params.id]
    );
    if (!rows.length) return res.status(404).json({ error: 'Not found' });
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.post('/api/inventory-adjustments', async (req, res) => {
  try {
    const {
      product_id, adjustment_date = null, adjustment_type = 'Addition',
      quantity = 0, reason = 'Other', adjusted_by = '', notes = ''
    } = req.body;
    if (!product_id) return res.status(400).json({ error: 'Product is required' });
    if (!['Addition', 'Removal', 'Correction'].includes(adjustment_type)) {
      return res.status(400).json({ error: 'Adjustment type must be Addition, Removal, or Correction' });
    }
    const validReasons = ['Purchase Order', 'Sales Order', 'Inventory Count', 'Damage', 'Return', 'Transfer', 'Other'];
    if (!validReasons.includes(reason)) {
      return res.status(400).json({ error: 'Invalid reason' });
    }
    const qty = parseInt(quantity, 10);
    if (!Number.isFinite(qty) || qty < 0) {
      return res.status(400).json({ error: 'Quantity must be a non-negative integer' });
    }
    const { rows: prows } = await pool.query('SELECT slug FROM products WHERE id = $1', [product_id]);
    if (!prows.length) return res.status(400).json({ error: 'Product not found' });
    const datePart = (adjustment_date ? new Date(adjustment_date) : new Date())
      .toISOString().slice(0, 10);
    const base = `${prows[0].slug}-${datePart}-qty${qty}`;
    const slug = await uniqueSlug('inventory_adjustments', base);
    const { rows: ins } = await pool.query(
      `INSERT INTO inventory_adjustments
         (slug, product_id, adjustment_date, adjustment_type, quantity, reason, adjusted_by, notes)
       VALUES ($1,$2,COALESCE($3, NOW()),$4,$5,$6,$7,$8) RETURNING id`,
      [slug, product_id, adjustment_date || null, adjustment_type, qty, reason, adjusted_by, notes]
    );
    const { rows } = await pool.query(
      `${INVENTORY_SELECT} WHERE ia.id = $1`, [ins[0].id]
    );
    res.status(201).json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.put('/api/inventory-adjustments/:id', async (req, res) => {
  try {
    const { adjustment_date = null, adjustment_type, quantity, reason, adjusted_by = '', notes = '' } = req.body;
    if (adjustment_type && !['Addition', 'Removal', 'Correction'].includes(adjustment_type)) {
      return res.status(400).json({ error: 'Adjustment type must be Addition, Removal, or Correction' });
    }
    const { rowCount } = await pool.query(
      `UPDATE inventory_adjustments
         SET adjustment_date = COALESCE($1, adjustment_date),
             adjustment_type = $2, quantity = $3, reason = $4,
             adjusted_by = $5, notes = $6
       WHERE id = $7`,
      [adjustment_date || null, adjustment_type ?? 'Addition',
       parseInt(quantity, 10) || 0, reason ?? 'Other',
       adjusted_by, notes, req.params.id]
    );
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    const { rows } = await pool.query(
      `${INVENTORY_SELECT} WHERE ia.id = $1`, [req.params.id]
    );
    res.json(rows[0]);
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

app.delete('/api/inventory-adjustments/:id', async (req, res) => {
  try {
    const { rowCount } = await pool.query('DELETE FROM inventory_adjustments WHERE id = $1', [req.params.id]);
    if (!rowCount) return res.status(404).json({ error: 'Not found' });
    res.status(204).end();
  } catch (err) { console.error(err); res.status(500).json({ error: err.message }); }
});

// ── Start ─────────────────────────────────────────────────────────────────────

app.listen(PORT, () => {
  console.log(`Backend listening on http://localhost:${PORT}`);
});
