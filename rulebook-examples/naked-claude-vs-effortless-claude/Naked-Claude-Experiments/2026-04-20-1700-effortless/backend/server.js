const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const app = express();
const PORT = process.env.PORT || 3001;
const DB_URL = process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/2026-04-20-1700-effortless';

const pool = new Pool({ connectionString: DB_URL });

app.use(cors());
app.use(express.json());

function slugify(name) {
  return name
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-');
}

// Sync statuses.clients denorm field after client status changes
async function syncStatusClients(client, statuse_id) {
  if (!statuse_id) return;
  const result = await client.query(
    'SELECT client_id FROM clients WHERE status = $1 ORDER BY client_id',
    [statuse_id]
  );
  const ids = result.rows.map(r => r.client_id).join(', ');
  await client.query(
    'UPDATE statuses SET clients = $1 WHERE statuse_id = $2',
    [ids || null, statuse_id]
  );
}

// ── CLIENTS ────────────────────────────────────────────────────────────────

app.get('/api/clients', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_clients ORDER BY name ASC');
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/clients/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_clients WHERE client_id = $1',
      [req.params.id]
    );
    if (result.rows.length === 0) return res.status(404).json({ error: 'Client not found' });
    const client_rec = result.rows[0];

    // Load invoices for this client
    const invoices = await pool.query(
      'SELECT * FROM vw_invoices WHERE client = $1 ORDER BY order_date DESC',
      [req.params.id]
    );
    client_rec.invoice_list = invoices.rows;

    // Load approvals for this client
    const approvals = await pool.query(
      'SELECT * FROM vw_client_approvals WHERE client = $1 ORDER BY name ASC',
      [req.params.id]
    );
    client_rec.approval_list = approvals.rows;

    res.json(client_rec);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/clients', async (req, res) => {
  const { name, notes, status, category, email, phone, company_name } = req.body;
  if (!name) return res.status(400).json({ error: 'Name is required' });

  const baseSlug = slugify(name);
  let client_id = baseSlug;

  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    // Ensure unique client_id
    const existing = await client.query(
      'SELECT client_id FROM clients WHERE client_id LIKE $1',
      [`${baseSlug}%`]
    );
    if (existing.rows.some(r => r.client_id === client_id)) {
      client_id = `${baseSlug}-${Date.now()}`;
    }

    await client.query(
      `INSERT INTO clients (client_id, name, notes, status, category, email, phone, company_name, created_at)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, NOW())`,
      [client_id, name, notes || null, status || null, category || null,
       email || null, phone || null, company_name || null]
    );

    if (status) await syncStatusClients(client, status);

    await client.query('COMMIT');
    const result = await pool.query('SELECT * FROM vw_clients WHERE client_id = $1', [client_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

app.put('/api/clients/:id', async (req, res) => {
  const { name, notes, status, category, email, phone, company_name } = req.body;
  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    const old = await client.query(
      'SELECT status FROM clients WHERE client_id = $1',
      [req.params.id]
    );
    if (old.rows.length === 0) {
      await client.query('ROLLBACK');
      return res.status(404).json({ error: 'Client not found' });
    }
    const oldStatus = old.rows[0].status;
    const newStatus = status !== undefined ? (status || null) : oldStatus;

    await client.query(
      `UPDATE clients SET
         name         = COALESCE($1, name),
         notes        = $2,
         status       = $3,
         category     = $4,
         email        = $5,
         phone        = $6,
         company_name = $7
       WHERE client_id = $8`,
      [
        name || null,
        notes !== undefined ? (notes || null) : null,
        newStatus,
        category !== undefined ? (category || null) : null,
        email !== undefined ? (email || null) : null,
        phone !== undefined ? (phone || null) : null,
        company_name !== undefined ? (company_name || null) : null,
        req.params.id
      ]
    );

    if (oldStatus !== newStatus) {
      if (oldStatus) await syncStatusClients(client, oldStatus);
      if (newStatus) await syncStatusClients(client, newStatus);
    }

    await client.query('COMMIT');
    const updated = await pool.query('SELECT * FROM vw_clients WHERE client_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

app.delete('/api/clients/:id', async (req, res) => {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    const old = await client.query(
      'SELECT status FROM clients WHERE client_id = $1',
      [req.params.id]
    );
    if (old.rows.length === 0) {
      await client.query('ROLLBACK');
      return res.status(404).json({ error: 'Client not found' });
    }
    const oldStatus = old.rows[0].status;

    // Check for invoices
    const invoiceCheck = await client.query(
      'SELECT COUNT(*) FROM invoices WHERE client = $1',
      [req.params.id]
    );
    const invoiceCount = parseInt(invoiceCheck.rows[0].count);
    if (invoiceCount > 0) {
      await client.query('ROLLBACK');
      return res.status(409).json({
        error: `Cannot delete: ${invoiceCount} invoice(s) belong to this client. Delete or reassign invoices first.`
      });
    }

    await client.query('DELETE FROM clients WHERE client_id = $1', [req.params.id]);

    if (oldStatus) await syncStatusClients(client, oldStatus);

    await client.query('COMMIT');
    res.json({ success: true });
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

// ── CLIENT CATEGORIES ──────────────────────────────────────────────────────

app.get('/api/client-categories', async (req, res) => {
  try {
    const cats = await pool.query('SELECT * FROM vw_client_categories ORDER BY name ASC');
    const counts = await pool.query(
      'SELECT category, COUNT(*)::int AS count FROM clients WHERE category IS NOT NULL GROUP BY category'
    );
    const countMap = {};
    counts.rows.forEach(r => { countMap[r.category] = r.count; });
    res.json(cats.rows.map(c => ({ ...c, client_count: countMap[c.client_categorie_id] || 0 })));
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/client-categories', async (req, res) => {
  const { name, notes, discount } = req.body;
  if (!name) return res.status(400).json({ error: 'name is required' });
  const client_categorie_id = slugify(name);
  try {
    await pool.query(
      'INSERT INTO client_categories (client_categorie_id, name, notes, discount) VALUES ($1, $2, $3, $4)',
      [client_categorie_id, name, notes || null, discount != null ? parseFloat(discount) : 0]
    );
    const result = await pool.query('SELECT * FROM vw_client_categories WHERE client_categorie_id = $1', [client_categorie_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/client-categories/:id', async (req, res) => {
  const { name, notes, discount } = req.body;
  try {
    const result = await pool.query(
      `UPDATE client_categories SET
         name     = COALESCE($1, name),
         notes    = $2,
         discount = $3
       WHERE client_categorie_id = $4`,
      [
        name || null,
        notes !== undefined ? (notes || null) : null,
        discount != null ? parseFloat(discount) : 0,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Category not found' });
    const updated = await pool.query('SELECT * FROM vw_client_categories WHERE client_categorie_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/client-categories/:id', async (req, res) => {
  try {
    const check = await pool.query(
      'SELECT COUNT(*)::int AS count FROM clients WHERE category = $1',
      [req.params.id]
    );
    if (check.rows[0].count > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${check.rows[0].count} client(s) are still assigned to this category. Reassign them first.`
      });
    }
    const result = await pool.query('DELETE FROM client_categories WHERE client_categorie_id = $1', [req.params.id]);
    if (result.rowCount === 0) return res.status(404).json({ error: 'Category not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── STATUSES ───────────────────────────────────────────────────────────────

app.get('/api/statuses', async (req, res) => {
  try {
    const statuses = await pool.query('SELECT * FROM vw_statuses ORDER BY sort_order ASC, display_name ASC');
    // Augment each status with a live client count
    const counts = await pool.query(
      'SELECT status, COUNT(*) as count FROM clients WHERE status IS NOT NULL GROUP BY status'
    );
    const countMap = {};
    counts.rows.forEach(r => { countMap[r.status] = parseInt(r.count); });
    const rows = statuses.rows.map(s => ({
      ...s,
      client_count: countMap[s.statuse_id] || 0
    }));
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/statuses/:id', async (req, res) => {
  try {
    const status = await pool.query(
      'SELECT * FROM vw_statuses WHERE statuse_id = $1',
      [req.params.id]
    );
    if (status.rows.length === 0) return res.status(404).json({ error: 'Status not found' });

    const clients = await pool.query(
      'SELECT * FROM vw_clients WHERE status = $1 ORDER BY name',
      [req.params.id]
    );
    res.json({ ...status.rows[0], client_list: clients.rows });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/statuses', async (req, res) => {
  const { display_name, description, is_blocking, sort_order } = req.body;
  if (!display_name) return res.status(400).json({ error: 'display_name is required' });

  const statuse_id = slugify(display_name);

  try {
    await pool.query(
      'INSERT INTO statuses (statuse_id, display_name, description, is_blocking, sort_order) VALUES ($1, $2, $3, $4, $5)',
      [statuse_id, display_name, description || null, is_blocking === true || is_blocking === 'true', sort_order != null ? parseInt(sort_order) : null]
    );
    const result = await pool.query('SELECT * FROM vw_statuses WHERE statuse_id = $1', [statuse_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/statuses/:id', async (req, res) => {
  const { display_name, description, is_blocking, sort_order } = req.body;
  try {
    const result = await pool.query(
      `UPDATE statuses
         SET display_name = COALESCE($1, display_name),
             description  = $2,
             is_blocking  = $3,
             sort_order   = $4
       WHERE statuse_id = $5`,
      [
        display_name || null,
        description !== undefined ? (description || null) : null,
        is_blocking === true || is_blocking === 'true',
        sort_order != null ? parseInt(sort_order) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Status not found' });
    const updated = await pool.query('SELECT * FROM vw_statuses WHERE statuse_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/statuses/:id', async (req, res) => {
  try {
    const check = await pool.query(
      'SELECT COUNT(*) FROM clients WHERE status = $1',
      [req.params.id]
    );
    const count = parseInt(check.rows[0].count);
    if (count > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${count} client(s) are still assigned to this status. Reassign them first.`
      });
    }
    const result = await pool.query('DELETE FROM statuses WHERE statuse_id = $1', [req.params.id]);
    if (result.rowCount === 0) return res.status(404).json({ error: 'Status not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── PRODUCTS ───────────────────────────────────────────────────────────────

app.get('/api/products', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_products ORDER BY sku ASC');
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/products/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_products WHERE product_id = $1',
      [req.params.id]
    );
    if (result.rows.length === 0) return res.status(404).json({ error: 'Product not found' });
    const product = result.rows[0];

    // Load line items referencing this product
    const lineItems = await pool.query(
      `SELECT li.*, i.invoice_number, i.client, i.order_status
         FROM vw_invoice_line_items li
         JOIN invoices i ON i.invoice_id = li.invoice
        WHERE li.product = $1
        ORDER BY li.invoice, li.line_number`,
      [req.params.id]
    );
    product.line_item_list = lineItems.rows;

    // Load inventory adjustments for this product
    const adjustments = await pool.query(
      'SELECT * FROM vw_inventory_adjustments WHERE product = $1 ORDER BY date DESC NULLS LAST',
      [req.params.id]
    );
    product.adjustment_list = adjustments.rows;

    res.json(product);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/products', async (req, res) => {
  const { sku, display_name, description, unit_price, cost, is_active, stock_quantity } = req.body;
  if (!sku) return res.status(400).json({ error: 'SKU is required' });

  const product_id = slugify(sku);
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    await client.query(
      `INSERT INTO products (product_id, sku, display_name, description, unit_price, cost, is_active)
       VALUES ($1, $2, $3, $4, $5, $6, $7)`,
      [
        product_id, sku.toUpperCase(),
        display_name || null, description || null,
        unit_price != null ? parseFloat(unit_price) : null,
        cost != null ? parseFloat(cost) : null,
        is_active !== false && is_active !== 'false'
      ]
    );

    // Stock is derived from inventory_adjustments (SUM of quantity).
    // If an initial stock is provided, seed an opening adjustment.
    if (stock_quantity != null && stock_quantity !== '') {
      const qty = parseInt(stock_quantity);
      if (qty !== 0) {
        const dateStr = new Date().toISOString().slice(0, 10);
        const adjId = slugify(`${sku.toLowerCase()}-${dateStr}-qty${qty}-opening`);
        await client.query(
          `INSERT INTO inventory_adjustments
             (inventory_adjustment_id, product, date, adjustment_type, quantity, reason, notes)
           VALUES ($1, $2, NOW(), 'Correction', $3, 'Opening balance', 'Initial stock on product creation')`,
          [adjId, product_id, qty]
        );
      }
    }

    await client.query('COMMIT');
    const result = await pool.query('SELECT * FROM vw_products WHERE product_id = $1', [product_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

app.put('/api/products/:id', async (req, res) => {
  const { sku, display_name, description, unit_price, cost, is_active } = req.body;
  try {
    const result = await pool.query(
      `UPDATE products SET
         sku          = COALESCE($1, sku),
         display_name = $2,
         description  = $3,
         unit_price   = $4,
         cost         = $5,
         is_active    = $6
       WHERE product_id = $7`,
      [
        sku ? sku.toUpperCase() : null,
        display_name !== undefined ? (display_name || null) : null,
        description !== undefined ? (description || null) : null,
        unit_price != null ? parseFloat(unit_price) : null,
        cost != null ? parseFloat(cost) : null,
        is_active !== false && is_active !== 'false',
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Product not found' });
    const updated = await pool.query('SELECT * FROM vw_products WHERE product_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/products/:id', async (req, res) => {
  try {
    // Check for line items referencing this product
    const check = await pool.query(
      'SELECT COUNT(*) FROM invoice_line_items WHERE product = $1',
      [req.params.id]
    );
    const count = parseInt(check.rows[0].count);
    if (count > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${count} line item(s) reference this product. Mark it inactive instead.`
      });
    }
    const result = await pool.query('DELETE FROM products WHERE product_id = $1', [req.params.id]);
    if (result.rowCount === 0) return res.status(404).json({ error: 'Product not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── INVOICES ───────────────────────────────────────────────────────────────

app.get('/api/invoices', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT i.*, c.name as client_name FROM vw_invoices i LEFT JOIN clients c ON c.client_id = i.client ORDER BY i.order_date DESC NULLS LAST'
    );
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/invoices/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT i.*, c.name as client_name FROM vw_invoices i LEFT JOIN clients c ON c.client_id = i.client WHERE i.invoice_id = $1',
      [req.params.id]
    );
    if (result.rows.length === 0) return res.status(404).json({ error: 'Invoice not found' });
    const invoice = result.rows[0];

    // Load line items
    const lineItems = await pool.query(
      `SELECT li.*, p.display_name as product_display_name, p.sku as product_sku
         FROM vw_invoice_line_items li
         LEFT JOIN products p ON p.product_id = li.product
        WHERE li.invoice = $1
        ORDER BY li.line_number`,
      [req.params.id]
    );
    invoice.line_item_list = lineItems.rows;

    // Load payments
    const payments = await pool.query(
      'SELECT * FROM vw_payments WHERE invoice = $1 ORDER BY payment_date DESC NULLS LAST',
      [req.params.id]
    );
    invoice.payment_list = payments.rows;

    res.json(invoice);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/invoices', async (req, res) => {
  const { invoice_number, client, order_date, order_status, shipping_address, billing_address, notes, tax_rate } = req.body;
  if (!client) return res.status(400).json({ error: 'client is required' });
  if (!invoice_number) return res.status(400).json({ error: 'invoice_number is required' });

  // Build invoice_id: {client_id}-{invoice_number}
  const invoice_id = `${client}-${invoice_number}`;

  try {
    await pool.query(
      `INSERT INTO invoices (invoice_id, invoice_number, client, order_date, order_status, shipping_address, billing_address, notes, tax_rate)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)`,
      [
        invoice_id, invoice_number, client,
        order_date || null, order_status || 'New',
        shipping_address || null, billing_address || null, notes || null,
        tax_rate != null ? parseFloat(tax_rate) : 0
      ]
    );
    const result = await pool.query(
      'SELECT i.*, c.name as client_name FROM vw_invoices i LEFT JOIN clients c ON c.client_id = i.client WHERE i.invoice_id = $1',
      [invoice_id]
    );
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/invoices/:id', async (req, res) => {
  const { invoice_number, order_date, order_status, shipping_address, billing_address, notes, tax_rate } = req.body;
  try {
    const result = await pool.query(
      `UPDATE invoices SET
         invoice_number   = COALESCE($1, invoice_number),
         order_date       = $2,
         order_status     = $3,
         shipping_address = $4,
         billing_address  = $5,
         notes            = $6,
         tax_rate         = $7
       WHERE invoice_id = $8`,
      [
        invoice_number || null,
        order_date !== undefined ? (order_date || null) : null,
        order_status !== undefined ? (order_status || null) : null,
        shipping_address !== undefined ? (shipping_address || null) : null,
        billing_address !== undefined ? (billing_address || null) : null,
        notes !== undefined ? (notes || null) : null,
        tax_rate != null ? parseFloat(tax_rate) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Invoice not found' });
    const updated = await pool.query(
      'SELECT i.*, c.name as client_name FROM vw_invoices i LEFT JOIN clients c ON c.client_id = i.client WHERE i.invoice_id = $1',
      [req.params.id]
    );
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/invoices/:id', async (req, res) => {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    const check = await client.query('SELECT invoice_id FROM invoices WHERE invoice_id = $1', [req.params.id]);
    if (check.rows.length === 0) {
      await client.query('ROLLBACK');
      return res.status(404).json({ error: 'Invoice not found' });
    }
    // Cascade delete line items and payments
    await client.query('DELETE FROM payments WHERE invoice = $1', [req.params.id]);
    await client.query('DELETE FROM invoice_line_items WHERE invoice = $1', [req.params.id]);
    await client.query('DELETE FROM invoices WHERE invoice_id = $1', [req.params.id]);
    await client.query('COMMIT');
    res.json({ success: true });
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

// ── INVOICE LINE ITEMS ─────────────────────────────────────────────────────

app.get('/api/invoices/:invoiceId/line-items', async (req, res) => {
  try {
    const result = await pool.query(
      `SELECT li.*, p.display_name as product_display_name, p.sku as product_sku
         FROM vw_invoice_line_items li
         LEFT JOIN products p ON p.product_id = li.product
        WHERE li.invoice = $1 ORDER BY li.line_number`,
      [req.params.invoiceId]
    );
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/invoices/:invoiceId/line-items', async (req, res) => {
  const { product, quantity, unit_price, discount_percent, notes } = req.body;
  if (!product) return res.status(400).json({ error: 'product is required' });
  if (quantity == null) return res.status(400).json({ error: 'quantity is required' });

  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    // Get next line number
    const numRes = await client.query(
      'SELECT COUNT(*) FROM invoice_line_items WHERE invoice = $1',
      [req.params.invoiceId]
    );
    const lineNumber = (parseInt(numRes.rows[0].count) + 1).toString();
    const invoice_line_item_id = `${req.params.invoiceId}-line-${lineNumber}`;

    await client.query(
      `INSERT INTO invoice_line_items (invoice_line_item_id, line_number, invoice, product, quantity, unit_price, discount_percent, notes)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8)`,
      [
        invoice_line_item_id, lineNumber, req.params.invoiceId,
        product, parseInt(quantity),
        unit_price != null ? parseFloat(unit_price) : null,
        discount_percent != null ? parseFloat(discount_percent) : 0,
        notes || null
      ]
    );

    await client.query('COMMIT');
    const result = await pool.query(
      `SELECT li.*, p.display_name as product_display_name, p.sku as product_sku
         FROM vw_invoice_line_items li
         LEFT JOIN products p ON p.product_id = li.product
        WHERE li.invoice_line_item_id = $1`,
      [invoice_line_item_id]
    );
    res.status(201).json(result.rows[0]);
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

app.put('/api/line-items/:id', async (req, res) => {
  const { product, quantity, unit_price, discount_percent, notes } = req.body;
  try {
    const result = await pool.query(
      `UPDATE invoice_line_items SET
         product          = COALESCE($1, product),
         quantity         = COALESCE($2, quantity),
         unit_price       = $3,
         discount_percent = $4,
         notes            = $5
       WHERE invoice_line_item_id = $6`,
      [
        product || null,
        quantity != null ? parseInt(quantity) : null,
        unit_price != null ? parseFloat(unit_price) : null,
        discount_percent != null ? parseFloat(discount_percent) : 0,
        notes !== undefined ? (notes || null) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Line item not found' });
    const updated = await pool.query(
      `SELECT li.*, p.display_name as product_display_name, p.sku as product_sku
         FROM vw_invoice_line_items li
         LEFT JOIN products p ON p.product_id = li.product
        WHERE li.invoice_line_item_id = $1`,
      [req.params.id]
    );
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/line-items/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'DELETE FROM invoice_line_items WHERE invoice_line_item_id = $1',
      [req.params.id]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Line item not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── PAYMENTS ───────────────────────────────────────────────────────────────

app.get('/api/payments', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_payments ORDER BY payment_date DESC NULLS LAST'
    );
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/invoices/:invoiceId/payments', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM vw_payments WHERE invoice = $1 ORDER BY payment_date DESC NULLS LAST',
      [req.params.invoiceId]
    );
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/invoices/:invoiceId/payments', async (req, res) => {
  const { payment_date, amount, payment_method, payment_status, transaction_id, notes } = req.body;
  if (amount == null) return res.status(400).json({ error: 'amount is required' });

  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    // Get next payment number
    const numRes = await client.query(
      'SELECT COUNT(*) FROM payments WHERE invoice = $1',
      [req.params.invoiceId]
    );
    const paymentNumber = (parseInt(numRes.rows[0].count) + 1).toString();
    const payment_id = `${req.params.invoiceId}-pmt-${paymentNumber}`;

    await client.query(
      `INSERT INTO payments (payment_id, payment_number, invoice, payment_date, amount, payment_method, payment_status, transaction_id, notes)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)`,
      [
        payment_id, paymentNumber, req.params.invoiceId,
        payment_date || null,
        parseFloat(amount),
        payment_method || null,
        payment_status || 'Pending',
        transaction_id || null,
        notes || null
      ]
    );

    await client.query('COMMIT');
    const result = await pool.query('SELECT * FROM vw_payments WHERE payment_id = $1', [payment_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

app.put('/api/payments/:id', async (req, res) => {
  const { payment_date, amount, payment_method, payment_status, transaction_id, notes } = req.body;
  try {
    const result = await pool.query(
      `UPDATE payments SET
         payment_date   = $1,
         amount         = COALESCE($2, amount),
         payment_method = $3,
         payment_status = $4,
         transaction_id = $5,
         notes          = $6
       WHERE payment_id = $7`,
      [
        payment_date !== undefined ? (payment_date || null) : null,
        amount != null ? parseFloat(amount) : null,
        payment_method !== undefined ? (payment_method || null) : null,
        payment_status !== undefined ? (payment_status || null) : null,
        transaction_id !== undefined ? (transaction_id || null) : null,
        notes !== undefined ? (notes || null) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Payment not found' });
    const updated = await pool.query('SELECT * FROM vw_payments WHERE payment_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/payments/:id', async (req, res) => {
  try {
    const result = await pool.query('DELETE FROM payments WHERE payment_id = $1', [req.params.id]);
    if (result.rowCount === 0) return res.status(404).json({ error: 'Payment not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── INVENTORY ADJUSTMENTS ──────────────────────────────────────────────────

app.get('/api/inventory-adjustments', async (req, res) => {
  try {
    const { product } = req.query;
    const result = product
      ? await pool.query(
          `SELECT a.*, p.display_name as product_display_name, p.sku as product_sku
             FROM vw_inventory_adjustments a
             LEFT JOIN products p ON p.product_id = a.product
            WHERE a.product = $1
            ORDER BY a.date DESC NULLS LAST`,
          [product]
        )
      : await pool.query(
          `SELECT a.*, p.display_name as product_display_name, p.sku as product_sku
             FROM vw_inventory_adjustments a
             LEFT JOIN products p ON p.product_id = a.product
            ORDER BY a.date DESC NULLS LAST`
        );
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/inventory-adjustments', async (req, res) => {
  const { product, date, adjustment_type, quantity, reason, notes, adjusted_by } = req.body;
  if (!product) return res.status(400).json({ error: 'product is required' });
  if (quantity == null) return res.status(400).json({ error: 'quantity is required' });
  if (!adjustment_type) return res.status(400).json({ error: 'adjustment_type is required' });

  const client = await pool.connect();
  try {
    await client.query('BEGIN');

    // Check product exists
    const p = await client.query('SELECT sku FROM products WHERE product_id = $1', [product]);
    if (p.rows.length === 0) {
      await client.query('ROLLBACK');
      return res.status(404).json({ error: 'Product not found' });
    }

    const dateStr = date ? new Date(date).toISOString().slice(0, 10) : new Date().toISOString().slice(0, 10);
    const baseName = `${p.rows[0].sku.toLowerCase()}-${dateStr}-qty${parseInt(quantity)}`;
    let id = slugify(baseName);
    const existing = await client.query(
      'SELECT inventory_adjustment_id FROM inventory_adjustments WHERE inventory_adjustment_id LIKE $1',
      [`${id}%`]
    );
    if (existing.rows.some(r => r.inventory_adjustment_id === id)) {
      id = `${id}-${Date.now()}`;
    }

    await client.query(
      `INSERT INTO inventory_adjustments
         (inventory_adjustment_id, product, date, adjustment_type, quantity, reason, notes, adjusted_by)
       VALUES ($1, $2, $3, $4, $5, $6, $7, $8)`,
      [
        id, product,
        date || new Date().toISOString(),
        adjustment_type,
        parseInt(quantity),
        reason || null,
        notes || null,
        adjusted_by || null
      ]
    );

    // Stock is a derived aggregation from inventory_adjustments — no manual update needed.

    await client.query('COMMIT');
    const result = await pool.query(
      `SELECT a.*, p.display_name as product_display_name, p.sku as product_sku
         FROM vw_inventory_adjustments a
         LEFT JOIN products p ON p.product_id = a.product
        WHERE a.inventory_adjustment_id = $1`,
      [id]
    );
    res.status(201).json(result.rows[0]);
  } catch (err) {
    await client.query('ROLLBACK');
    console.error(err);
    res.status(500).json({ error: err.message });
  } finally {
    client.release();
  }
});

app.put('/api/inventory-adjustments/:id', async (req, res) => {
  const { date, adjustment_type, quantity, reason, notes, adjusted_by } = req.body;
  try {
    const result = await pool.query(
      `UPDATE inventory_adjustments SET
         date            = $1,
         adjustment_type = COALESCE($2, adjustment_type),
         quantity        = COALESCE($3, quantity),
         reason          = $4,
         notes           = $5,
         adjusted_by     = $6
       WHERE inventory_adjustment_id = $7`,
      [
        date !== undefined ? (date || null) : null,
        adjustment_type || null,
        quantity != null ? parseInt(quantity) : null,
        reason !== undefined ? (reason || null) : null,
        notes !== undefined ? (notes || null) : null,
        adjusted_by !== undefined ? (adjusted_by || null) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Adjustment not found' });
    const updated = await pool.query(
      `SELECT a.*, p.display_name as product_display_name, p.sku as product_sku
         FROM vw_inventory_adjustments a
         LEFT JOIN products p ON p.product_id = a.product
        WHERE a.inventory_adjustment_id = $1`,
      [req.params.id]
    );
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/inventory-adjustments/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'DELETE FROM inventory_adjustments WHERE inventory_adjustment_id = $1',
      [req.params.id]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Adjustment not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── APP USERS ──────────────────────────────────────────────────────────────

app.get('/api/app-users', async (req, res) => {
  try {
    const users = await pool.query('SELECT * FROM vw_app_users ORDER BY contact_name ASC, email_address ASC');
    const counts = await pool.query(
      'SELECT approved_by, COUNT(*)::int AS count FROM client_approvals WHERE approved_by IS NOT NULL GROUP BY approved_by'
    );
    const countMap = {};
    counts.rows.forEach(r => { countMap[r.approved_by] = r.count; });
    res.json(users.rows.map(u => ({ ...u, approval_count: countMap[u.app_user_id] || 0 })));
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/app-users/:id', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_app_users WHERE app_user_id = $1', [req.params.id]);
    if (result.rows.length === 0) return res.status(404).json({ error: 'User not found' });
    const user = result.rows[0];
    const approvals = await pool.query(
      'SELECT * FROM vw_client_approvals WHERE approved_by = $1 ORDER BY client_name ASC',
      [req.params.id]
    );
    user.approval_list = approvals.rows;
    res.json(user);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/app-users', async (req, res) => {
  const { contact_name, email_address, phone_number, role, notes } = req.body;
  if (!contact_name) return res.status(400).json({ error: 'contact_name is required' });
  if (!email_address) return res.status(400).json({ error: 'email_address is required' });

  // Airtable's AppUsers.Name formula: SUBSTITUTE(LOWER(EmailAddress & "-" & Role), "@", "-")
  const base = `${email_address}-${role || 'user'}`.toLowerCase().replace(/@/g, '-');
  let app_user_id = slugify(base);

  try {
    const existing = await pool.query('SELECT app_user_id FROM app_users WHERE app_user_id = $1', [app_user_id]);
    if (existing.rows.length > 0) {
      app_user_id = `${app_user_id}-${Date.now()}`;
    }
    await pool.query(
      `INSERT INTO app_users (app_user_id, contact_name, email_address, phone_number, role, notes)
       VALUES ($1, $2, $3, $4, $5, $6)`,
      [app_user_id, contact_name, email_address, phone_number || null, role || null, notes || null]
    );
    const result = await pool.query('SELECT * FROM vw_app_users WHERE app_user_id = $1', [app_user_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/app-users/:id', async (req, res) => {
  const { contact_name, email_address, phone_number, role, notes } = req.body;
  try {
    const result = await pool.query(
      `UPDATE app_users SET
         contact_name  = COALESCE($1, contact_name),
         email_address = COALESCE($2, email_address),
         phone_number  = $3,
         role          = $4,
         notes         = $5
       WHERE app_user_id = $6`,
      [
        contact_name || null,
        email_address || null,
        phone_number !== undefined ? (phone_number || null) : null,
        role !== undefined ? (role || null) : null,
        notes !== undefined ? (notes || null) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'User not found' });
    const updated = await pool.query('SELECT * FROM vw_app_users WHERE app_user_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/app-users/:id', async (req, res) => {
  try {
    const check = await pool.query(
      'SELECT COUNT(*)::int AS count FROM client_approvals WHERE approved_by = $1',
      [req.params.id]
    );
    if (check.rows[0].count > 0) {
      return res.status(409).json({
        error: `Cannot delete: ${check.rows[0].count} approval(s) reference this user. Reassign or delete the approvals first.`
      });
    }
    const result = await pool.query('DELETE FROM app_users WHERE app_user_id = $1', [req.params.id]);
    if (result.rowCount === 0) return res.status(404).json({ error: 'User not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── CLIENT APPROVALS ───────────────────────────────────────────────────────

app.get('/api/approvals', async (req, res) => {
  try {
    const { status, client } = req.query;
    let sql = 'SELECT * FROM vw_client_approvals';
    const wheres = [];
    const params = [];
    if (status === 'pending') wheres.push('is_approved IS NOT TRUE');
    else if (status === 'approved') wheres.push('is_approved = true');
    if (client) { params.push(client); wheres.push(`client = $${params.length}`); }
    if (wheres.length) sql += ' WHERE ' + wheres.join(' AND ');
    sql += ' ORDER BY client_name ASC, name ASC';
    const result = await pool.query(sql, params);
    res.json(result.rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/approvals/:id', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_client_approvals WHERE client_approval_id = $1', [req.params.id]);
    if (result.rows.length === 0) return res.status(404).json({ error: 'Approval not found' });
    res.json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/approvals', async (req, res) => {
  const { client, approved_by, notes } = req.body;
  if (!client) return res.status(400).json({ error: 'client is required' });

  const base = approved_by ? `${client}-${approved_by}` : `${client}-pending-${Date.now()}`;
  let client_approval_id = slugify(base);
  try {
    const existing = await pool.query(
      'SELECT client_approval_id FROM client_approvals WHERE client_approval_id = $1',
      [client_approval_id]
    );
    if (existing.rows.length > 0) client_approval_id = `${client_approval_id}-${Date.now()}`;

    await pool.query(
      `INSERT INTO client_approvals (client_approval_id, client, approved_by, notes)
       VALUES ($1, $2, $3, $4)`,
      [client_approval_id, client, approved_by || null, notes || null]
    );
    const result = await pool.query('SELECT * FROM vw_client_approvals WHERE client_approval_id = $1', [client_approval_id]);
    res.status(201).json(result.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/approvals/:id', async (req, res) => {
  const { approved_by, notes } = req.body;
  try {
    const result = await pool.query(
      `UPDATE client_approvals SET
         approved_by = $1,
         notes       = $2
       WHERE client_approval_id = $3`,
      [
        approved_by !== undefined ? (approved_by || null) : null,
        notes !== undefined ? (notes || null) : null,
        req.params.id
      ]
    );
    if (result.rowCount === 0) return res.status(404).json({ error: 'Approval not found' });
    const updated = await pool.query('SELECT * FROM vw_client_approvals WHERE client_approval_id = $1', [req.params.id]);
    res.json(updated.rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/approvals/:id', async (req, res) => {
  try {
    const result = await pool.query('DELETE FROM client_approvals WHERE client_approval_id = $1', [req.params.id]);
    if (result.rowCount === 0) return res.status(404).json({ error: 'Approval not found' });
    res.json({ success: true });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ── SUMMARY ────────────────────────────────────────────────────────────────

app.get('/api/summary', async (req, res) => {
  try {
    const [
      totalClients, stoppedClients,
      totalStatuses, blockingStatuses,
      totalInvoices, totalProducts, activeProducts,
      totalPayments, totalAdjustments,
      revenue, outstanding,
      inventoryValue, outOfStock, lowStock,
      vipClients, bigOrders, highMarginProducts,
      totalAppUsers, totalApprovals, pendingApprovals,
      statusDist, invoiceStatusDist, paymentStatusDist
    ] = await Promise.all([
      pool.query('SELECT COUNT(*) FROM clients'),
      pool.query('SELECT COUNT(*) FROM vw_clients WHERE is_stopped = true'),
      pool.query('SELECT COUNT(*) FROM statuses'),
      pool.query('SELECT COUNT(*) FROM statuses WHERE is_blocking = true'),
      pool.query('SELECT COUNT(*) FROM invoices'),
      pool.query('SELECT COUNT(*) FROM products'),
      pool.query('SELECT COUNT(*) FROM products WHERE is_active = true'),
      pool.query('SELECT COUNT(*) FROM payments'),
      pool.query('SELECT COUNT(*) FROM inventory_adjustments'),
      pool.query('SELECT COALESCE(SUM(invoice_total), 0) as total FROM vw_invoices'),
      pool.query('SELECT COALESCE(SUM(amount_due), 0) as total FROM vw_invoices'),
      pool.query('SELECT COALESCE(SUM(cogs), 0) as total FROM vw_products'),
      pool.query('SELECT COUNT(*)::int as count FROM vw_products WHERE stock_quantity = 0 AND is_active = true'),
      pool.query('SELECT COUNT(*)::int as count FROM vw_products WHERE stock_quantity > 0 AND stock_quantity < 50 AND is_active = true'),
      pool.query('SELECT COUNT(*)::int as count FROM vw_clients WHERE is_vip = true'),
      pool.query('SELECT COUNT(*)::int as count FROM vw_invoices WHERE is_big_order = true'),
      pool.query('SELECT COUNT(*)::int as count FROM vw_products WHERE is_high_margin = true AND is_active = true'),
      pool.query('SELECT COUNT(*)::int as count FROM app_users'),
      pool.query('SELECT COUNT(*)::int as count FROM client_approvals'),
      pool.query('SELECT COUNT(*)::int as count FROM vw_client_approvals WHERE is_approved IS NOT TRUE'),
      pool.query(`
        SELECT s.statuse_id, s.display_name, s.is_blocking,
               COUNT(c.client_id)::int AS count
          FROM statuses s
          LEFT JOIN clients c ON c.status = s.statuse_id
         GROUP BY s.statuse_id, s.display_name, s.is_blocking
         ORDER BY s.sort_order, s.display_name
      `),
      pool.query(`
        SELECT order_status, COUNT(*)::int as count
          FROM invoices
         GROUP BY order_status
         ORDER BY order_status
      `),
      pool.query(`
        SELECT payment_status,
               COUNT(*)::int as count,
               SUM(amount) as total_amount,
               SUM(completed_amount) as completed_amount
          FROM vw_payments
         GROUP BY payment_status
         ORDER BY payment_status
      `)
    ]);

    res.json({
      total_clients: parseInt(totalClients.rows[0].count),
      stopped_clients: parseInt(stoppedClients.rows[0].count),
      total_statuses: parseInt(totalStatuses.rows[0].count),
      blocking_statuses: parseInt(blockingStatuses.rows[0].count),
      total_invoices: parseInt(totalInvoices.rows[0].count),
      total_products: parseInt(totalProducts.rows[0].count),
      active_products: parseInt(activeProducts.rows[0].count),
      total_payments: parseInt(totalPayments.rows[0].count),
      total_adjustments: parseInt(totalAdjustments.rows[0].count),
      total_revenue: parseFloat(revenue.rows[0].total),
      total_outstanding: parseFloat(outstanding.rows[0].total),
      inventory_value: parseFloat(inventoryValue.rows[0].total),
      out_of_stock_count: outOfStock.rows[0].count,
      low_stock_count: lowStock.rows[0].count,
      vip_clients: vipClients.rows[0].count,
      big_orders: bigOrders.rows[0].count,
      high_margin_products: highMarginProducts.rows[0].count,
      total_app_users: totalAppUsers.rows[0].count,
      total_approvals: totalApprovals.rows[0].count,
      pending_approvals: pendingApprovals.rows[0].count,
      distribution: statusDist.rows,
      invoice_status_distribution: invoiceStatusDist.rows,
      payment_status_distribution: paymentStatusDist.rows
    });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

app.listen(PORT, () => {
  console.log(`Client & Invoice Tracker API running on http://localhost:${PORT}`);
});
