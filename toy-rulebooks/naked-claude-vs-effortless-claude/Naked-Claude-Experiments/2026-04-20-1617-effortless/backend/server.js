const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const app = express();
const PORT = process.env.PORT || 3001;

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/naked-effortless-effortless-db'
});

app.use(cors());
app.use(express.json());

// ============================================================
// SUMMARY / DASHBOARD
// ============================================================

app.get('/api/summary', async (req, res) => {
  try {
    const [custR, ordR, ordStatusR, prodR, payR, revenueR] = await Promise.all([
      pool.query(`SELECT COUNT(*)::int AS total, SUM(CASE WHEN is_stopped THEN 1 ELSE 0 END)::int AS stopped FROM vw_customers`),
      pool.query(`SELECT COUNT(*)::int AS total FROM vw_orders`),
      pool.query(`SELECT order_status, COUNT(*)::int AS cnt FROM vw_orders GROUP BY order_status ORDER BY order_status`),
      pool.query(`SELECT COUNT(*)::int AS total, SUM(CASE WHEN is_active THEN 1 ELSE 0 END)::int AS active FROM vw_products`),
      pool.query(`SELECT payment_status, COUNT(*)::int AS cnt, SUM(amount)::numeric AS total_amount, SUM(completed_amount)::numeric AS completed_amount FROM vw_payments GROUP BY payment_status ORDER BY payment_status`),
      pool.query(`SELECT COALESCE(SUM(order_total),0)::numeric AS total_revenue, COALESCE(SUM(amount_due),0)::numeric AS outstanding FROM vw_orders`)
    ]);
    res.json({
      customers: { total: custR.rows[0].total, stopped: custR.rows[0].stopped },
      orders: { total: ordR.rows[0].total, by_status: ordStatusR.rows },
      products: { total: prodR.rows[0].total, active: prodR.rows[0].active },
      payments: { by_status: payR.rows },
      total_revenue: revenueR.rows[0].total_revenue,
      outstanding: revenueR.rows[0].outstanding
    });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
});

// ============================================================
// CUSTOMERS
// ============================================================

app.get('/api/customers', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_customers ORDER BY name ASC');
    res.json(r.rows);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/customers/:id', async (req, res) => {
  try {
    const [custR, ordersR] = await Promise.all([
      pool.query('SELECT * FROM vw_customers WHERE customer_id = $1', [req.params.id]),
      pool.query('SELECT * FROM vw_orders WHERE customer = $1 ORDER BY order_date DESC', [req.params.id])
    ]);
    if (!custR.rows.length) return res.status(404).json({ error: 'Customer not found' });
    res.json({ ...custR.rows[0], order_list: ordersR.rows });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/customers', async (req, res) => {
  const { name, notes, status, email, phone, company_name, billing_address, shipping_address } = req.body;
  if (!name) return res.status(400).json({ error: 'Name is required' });
  const customer_id = name.toLowerCase().trim().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
  try {
    await pool.query(
      `INSERT INTO customers (customer_id, name, notes, status, email, phone, company_name, billing_address, shipping_address, created_at)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,NOW())`,
      [customer_id, name, notes||null, status||null, email||null, phone||null, company_name||null, billing_address||null, shipping_address||null]
    );
    const r = await pool.query('SELECT * FROM vw_customers WHERE customer_id = $1', [customer_id]);
    res.status(201).json(r.rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A customer with that name already exists' });
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/customers/:id', async (req, res) => {
  const { name, notes, status, email, phone, company_name, billing_address, shipping_address } = req.body;
  const { id } = req.params;
  try {
    const ex = await pool.query('SELECT * FROM customers WHERE customer_id = $1', [id]);
    if (!ex.rows.length) return res.status(404).json({ error: 'Customer not found' });
    const e = ex.rows[0];
    await pool.query(
      `UPDATE customers SET name=$1, notes=$2, status=$3, email=$4, phone=$5, company_name=$6, billing_address=$7, shipping_address=$8 WHERE customer_id=$9`,
      [
        name!==undefined ? name : e.name,
        notes!==undefined ? notes : e.notes,
        status!==undefined ? (status||null) : e.status,
        email!==undefined ? email : e.email,
        phone!==undefined ? phone : e.phone,
        company_name!==undefined ? company_name : e.company_name,
        billing_address!==undefined ? billing_address : e.billing_address,
        shipping_address!==undefined ? shipping_address : e.shipping_address,
        id
      ]
    );
    const r = await pool.query('SELECT * FROM vw_customers WHERE customer_id = $1', [id]);
    res.json(r.rows[0]);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.delete('/api/customers/:id', async (req, res) => {
  try {
    const orderCheck = await pool.query('SELECT COUNT(*)::int AS cnt FROM orders WHERE customer = $1', [req.params.id]);
    if (orderCheck.rows[0].cnt > 0)
      return res.status(409).json({ error: `Cannot delete: ${orderCheck.rows[0].cnt} order(s) belong to this customer. Delete or reassign orders first.` });
    const r = await pool.query('DELETE FROM customers WHERE customer_id = $1 RETURNING *', [req.params.id]);
    if (!r.rows.length) return res.status(404).json({ error: 'Customer not found' });
    res.json({ message: 'Customer deleted' });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

// ============================================================
// STATUSES
// ============================================================

app.get('/api/statuses', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_statuses ORDER BY sort_order ASC, display_name ASC');
    res.json(r.rows);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/statuses/:id', async (req, res) => {
  try {
    const [sR, cR] = await Promise.all([
      pool.query('SELECT * FROM vw_statuses WHERE statuse_id = $1', [req.params.id]),
      pool.query('SELECT * FROM vw_customers WHERE status = $1 ORDER BY name ASC', [req.params.id])
    ]);
    if (!sR.rows.length) return res.status(404).json({ error: 'Status not found' });
    res.json({ ...sR.rows[0], customer_list: cR.rows });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/statuses', async (req, res) => {
  const { display_name, description, is_blocking, sort_order } = req.body;
  if (!display_name) return res.status(400).json({ error: 'Display name is required' });
  const statuse_id = display_name.toLowerCase().trim().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
  try {
    await pool.query(
      'INSERT INTO statuses (statuse_id, display_name, description, is_blocking, sort_order) VALUES ($1,$2,$3,$4,$5)',
      [statuse_id, display_name, description||null, is_blocking===true||is_blocking==='true', sort_order||null]
    );
    const r = await pool.query('SELECT * FROM vw_statuses WHERE statuse_id = $1', [statuse_id]);
    res.status(201).json(r.rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A status with that name already exists' });
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/statuses/:id', async (req, res) => {
  const { display_name, description, is_blocking, sort_order } = req.body;
  const { id } = req.params;
  try {
    const ex = await pool.query('SELECT * FROM statuses WHERE statuse_id = $1', [id]);
    if (!ex.rows.length) return res.status(404).json({ error: 'Status not found' });
    const e = ex.rows[0];
    await pool.query(
      'UPDATE statuses SET display_name=$1, description=$2, is_blocking=$3, sort_order=$4 WHERE statuse_id=$5',
      [
        display_name!==undefined ? display_name : e.display_name,
        description!==undefined ? description : e.description,
        is_blocking!==undefined ? (is_blocking===true||is_blocking==='true') : e.is_blocking,
        sort_order!==undefined ? sort_order : e.sort_order,
        id
      ]
    );
    const r = await pool.query('SELECT * FROM vw_statuses WHERE statuse_id = $1', [id]);
    res.json(r.rows[0]);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.delete('/api/statuses/:id', async (req, res) => {
  try {
    const check = await pool.query('SELECT COUNT(*)::int AS cnt FROM customers WHERE status = $1', [req.params.id]);
    if (check.rows[0].cnt > 0)
      return res.status(409).json({ error: `Cannot delete: ${check.rows[0].cnt} customer(s) assigned to this status. Reassign them first.` });
    const r = await pool.query('DELETE FROM statuses WHERE statuse_id = $1 RETURNING *', [req.params.id]);
    if (!r.rows.length) return res.status(404).json({ error: 'Status not found' });
    res.json({ message: 'Status deleted' });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

// ============================================================
// PRODUCTS
// ============================================================

app.get('/api/products', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_products ORDER BY sku ASC');
    res.json(r.rows);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/products/:id', async (req, res) => {
  try {
    const [pR, liR] = await Promise.all([
      pool.query('SELECT * FROM vw_products WHERE product_id = $1', [req.params.id]),
      pool.query(`SELECT oli.*, o.order_number, o.customer, o.order_date 
                  FROM vw_order_line_items oli 
                  JOIN orders o ON o.order_id = oli."order"
                  WHERE oli.product = $1 ORDER BY o.order_date DESC`, [req.params.id])
    ]);
    if (!pR.rows.length) return res.status(404).json({ error: 'Product not found' });
    res.json({ ...pR.rows[0], line_item_list: liR.rows });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/products', async (req, res) => {
  const { sku, display_name, description, unit_price, cost, stock_quantity, is_active } = req.body;
  if (!sku) return res.status(400).json({ error: 'SKU is required' });
  const product_id = sku.toLowerCase().trim().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
  try {
    await pool.query(
      'INSERT INTO products (product_id, sku, display_name, description, unit_price, cost, stock_quantity, is_active) VALUES ($1,$2,$3,$4,$5,$6,$7,$8)',
      [product_id, sku.toUpperCase(), display_name||null, description||null, unit_price||null, cost||null, stock_quantity||null, is_active!==false && is_active!=='false']
    );
    const r = await pool.query('SELECT * FROM vw_products WHERE product_id = $1', [product_id]);
    res.status(201).json(r.rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A product with that SKU already exists' });
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/products/:id', async (req, res) => {
  const { sku, display_name, description, unit_price, cost, stock_quantity, is_active } = req.body;
  const { id } = req.params;
  try {
    const ex = await pool.query('SELECT * FROM products WHERE product_id = $1', [id]);
    if (!ex.rows.length) return res.status(404).json({ error: 'Product not found' });
    const e = ex.rows[0];
    await pool.query(
      'UPDATE products SET sku=$1, display_name=$2, description=$3, unit_price=$4, cost=$5, stock_quantity=$6, is_active=$7 WHERE product_id=$8',
      [
        sku!==undefined ? sku.toUpperCase() : e.sku,
        display_name!==undefined ? display_name : e.display_name,
        description!==undefined ? description : e.description,
        unit_price!==undefined ? unit_price : e.unit_price,
        cost!==undefined ? cost : e.cost,
        stock_quantity!==undefined ? stock_quantity : e.stock_quantity,
        is_active!==undefined ? (is_active===true||is_active==='true') : e.is_active,
        id
      ]
    );
    const r = await pool.query('SELECT * FROM vw_products WHERE product_id = $1', [id]);
    res.json(r.rows[0]);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.delete('/api/products/:id', async (req, res) => {
  try {
    const check = await pool.query('SELECT COUNT(*)::int AS cnt FROM order_line_items WHERE product = $1', [req.params.id]);
    if (check.rows[0].cnt > 0)
      return res.status(409).json({ error: `Cannot delete: ${check.rows[0].cnt} line item(s) reference this product. Mark it inactive instead, or delete the line items first.` });
    const r = await pool.query('DELETE FROM products WHERE product_id = $1 RETURNING *', [req.params.id]);
    if (!r.rows.length) return res.status(404).json({ error: 'Product not found' });
    res.json({ message: 'Product deleted' });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

// ============================================================
// ORDERS
// ============================================================

app.get('/api/orders', async (req, res) => {
  try {
    const r = await pool.query('SELECT * FROM vw_orders ORDER BY order_date DESC NULLS LAST');
    res.json(r.rows);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.get('/api/orders/:id', async (req, res) => {
  try {
    const [oR, liR, pmtR] = await Promise.all([
      pool.query('SELECT * FROM vw_orders WHERE order_id = $1', [req.params.id]),
      pool.query(`SELECT oli.*, p.display_name AS product_display_name, p.sku AS product_sku
                  FROM vw_order_line_items oli
                  LEFT JOIN products p ON p.product_id = oli.product
                  WHERE oli."order" = $1 ORDER BY oli.line_number`, [req.params.id]),
      pool.query('SELECT * FROM vw_payments WHERE "order" = $1 ORDER BY payment_date DESC NULLS LAST', [req.params.id])
    ]);
    if (!oR.rows.length) return res.status(404).json({ error: 'Order not found' });
    res.json({ ...oR.rows[0], line_item_list: liR.rows, payment_list: pmtR.rows });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/orders', async (req, res) => {
  const { order_number, customer, order_date, order_status, shipping_address, billing_address, notes, tax_rate } = req.body;
  if (!customer) return res.status(400).json({ error: 'Customer is required' });
  if (!order_number) return res.status(400).json({ error: 'Order number is required' });
  // Build order_id from customer + order_number
  const order_id = `${customer}-${order_number}`.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
  try {
    await pool.query(
      `INSERT INTO orders (order_id, order_number, customer, order_date, order_status, shipping_address, billing_address, notes, tax_rate)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9)`,
      [order_id, order_number, customer, order_date||null, order_status||'New', shipping_address||null, billing_address||null, notes||null, tax_rate||0]
    );
    const r = await pool.query('SELECT * FROM vw_orders WHERE order_id = $1', [order_id]);
    res.status(201).json(r.rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'An order with that ID already exists' });
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/orders/:id', async (req, res) => {
  const { order_number, customer, order_date, order_status, shipping_address, billing_address, notes, tax_rate } = req.body;
  const { id } = req.params;
  try {
    const ex = await pool.query('SELECT * FROM orders WHERE order_id = $1', [id]);
    if (!ex.rows.length) return res.status(404).json({ error: 'Order not found' });
    const e = ex.rows[0];
    await pool.query(
      `UPDATE orders SET order_number=$1, customer=$2, order_date=$3, order_status=$4, shipping_address=$5, billing_address=$6, notes=$7, tax_rate=$8 WHERE order_id=$9`,
      [
        order_number!==undefined ? order_number : e.order_number,
        customer!==undefined ? customer : e.customer,
        order_date!==undefined ? order_date : e.order_date,
        order_status!==undefined ? order_status : e.order_status,
        shipping_address!==undefined ? shipping_address : e.shipping_address,
        billing_address!==undefined ? billing_address : e.billing_address,
        notes!==undefined ? notes : e.notes,
        tax_rate!==undefined ? tax_rate : e.tax_rate,
        id
      ]
    );
    const r = await pool.query('SELECT * FROM vw_orders WHERE order_id = $1', [id]);
    res.json(r.rows[0]);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.delete('/api/orders/:id', async (req, res) => {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    await client.query('DELETE FROM payments WHERE "order" = $1', [req.params.id]);
    await client.query('DELETE FROM order_line_items WHERE "order" = $1', [req.params.id]);
    const r = await client.query('DELETE FROM orders WHERE order_id = $1 RETURNING *', [req.params.id]);
    if (!r.rows.length) { await client.query('ROLLBACK'); return res.status(404).json({ error: 'Order not found' }); }
    await client.query('COMMIT');
    res.json({ message: 'Order and all its line items and payments deleted' });
  } catch (err) {
    await client.query('ROLLBACK');
    res.status(500).json({ error: err.message });
  } finally { client.release(); }
});

// ============================================================
// ORDER LINE ITEMS
// ============================================================

app.get('/api/order-line-items', async (req, res) => {
  try {
    const { order } = req.query;
    let q = 'SELECT * FROM vw_order_line_items';
    const params = [];
    if (order) { q += ' WHERE "order" = $1'; params.push(order); }
    q += ' ORDER BY "order", line_number';
    const r = await pool.query(q, params);
    res.json(r.rows);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/order-line-items', async (req, res) => {
  const { order, product, quantity, unit_price, discount_percent, notes, line_number } = req.body;
  if (!order) return res.status(400).json({ error: 'Order is required' });
  if (!product) return res.status(400).json({ error: 'Product is required' });
  // Auto-assign line number if not given
  let lineNum = line_number;
  if (!lineNum) {
    const maxR = await pool.query('SELECT COALESCE(MAX(line_number::int),0)+1 AS next_line FROM order_line_items WHERE "order" = $1', [order]);
    lineNum = String(maxR.rows[0].next_line);
  }
  const order_line_item_id = `${order}-line-${lineNum}`;
  try {
    await pool.query(
      'INSERT INTO order_line_items (order_line_item_id, line_number, "order", product, quantity, unit_price, discount_percent, notes) VALUES ($1,$2,$3,$4,$5,$6,$7,$8)',
      [order_line_item_id, String(lineNum), order, product, quantity||1, unit_price||0, discount_percent||0, notes||null]
    );
    const r = await pool.query('SELECT * FROM vw_order_line_items WHERE order_line_item_id = $1', [order_line_item_id]);
    res.status(201).json(r.rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A line item with that ID already exists' });
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/order-line-items/:id', async (req, res) => {
  const { product, quantity, unit_price, discount_percent, notes } = req.body;
  const { id } = req.params;
  try {
    const ex = await pool.query('SELECT * FROM order_line_items WHERE order_line_item_id = $1', [id]);
    if (!ex.rows.length) return res.status(404).json({ error: 'Line item not found' });
    const e = ex.rows[0];
    await pool.query(
      'UPDATE order_line_items SET product=$1, quantity=$2, unit_price=$3, discount_percent=$4, notes=$5 WHERE order_line_item_id=$6',
      [
        product!==undefined ? product : e.product,
        quantity!==undefined ? quantity : e.quantity,
        unit_price!==undefined ? unit_price : e.unit_price,
        discount_percent!==undefined ? discount_percent : e.discount_percent,
        notes!==undefined ? notes : e.notes,
        id
      ]
    );
    const r = await pool.query('SELECT * FROM vw_order_line_items WHERE order_line_item_id = $1', [id]);
    res.json(r.rows[0]);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.delete('/api/order-line-items/:id', async (req, res) => {
  try {
    const r = await pool.query('DELETE FROM order_line_items WHERE order_line_item_id = $1 RETURNING *', [req.params.id]);
    if (!r.rows.length) return res.status(404).json({ error: 'Line item not found' });
    res.json({ message: 'Line item deleted' });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

// ============================================================
// PAYMENTS
// ============================================================

app.get('/api/payments', async (req, res) => {
  try {
    const { order } = req.query;
    let q = 'SELECT * FROM vw_payments';
    const params = [];
    if (order) { q += ' WHERE "order" = $1'; params.push(order); }
    q += ' ORDER BY payment_date DESC NULLS LAST';
    const r = await pool.query(q, params);
    res.json(r.rows);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.post('/api/payments', async (req, res) => {
  const { order, payment_date, amount, payment_method, payment_status, transaction_id, notes, payment_number } = req.body;
  if (!order) return res.status(400).json({ error: 'Order is required' });
  if (!amount) return res.status(400).json({ error: 'Amount is required' });
  let pmtNum = payment_number;
  if (!pmtNum) {
    const maxR = await pool.query('SELECT COALESCE(MAX(payment_number::int),0)+1 AS next_pmt FROM payments WHERE "order" = $1', [order]);
    pmtNum = String(maxR.rows[0].next_pmt);
  }
  const payment_id = `${order}-pmt-${pmtNum}`;
  try {
    await pool.query(
      'INSERT INTO payments (payment_id, payment_number, "order", payment_date, amount, payment_method, payment_status, transaction_id, notes) VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9)',
      [payment_id, String(pmtNum), order, payment_date||null, amount, payment_method||null, payment_status||'Completed', transaction_id||null, notes||null]
    );
    const r = await pool.query('SELECT * FROM vw_payments WHERE payment_id = $1', [payment_id]);
    res.status(201).json(r.rows[0]);
  } catch (err) {
    if (err.code === '23505') return res.status(409).json({ error: 'A payment with that ID already exists' });
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/payments/:id', async (req, res) => {
  const { payment_date, amount, payment_method, payment_status, transaction_id, notes } = req.body;
  const { id } = req.params;
  try {
    const ex = await pool.query('SELECT * FROM payments WHERE payment_id = $1', [id]);
    if (!ex.rows.length) return res.status(404).json({ error: 'Payment not found' });
    const e = ex.rows[0];
    await pool.query(
      'UPDATE payments SET payment_date=$1, amount=$2, payment_method=$3, payment_status=$4, transaction_id=$5, notes=$6 WHERE payment_id=$7',
      [
        payment_date!==undefined ? payment_date : e.payment_date,
        amount!==undefined ? amount : e.amount,
        payment_method!==undefined ? payment_method : e.payment_method,
        payment_status!==undefined ? payment_status : e.payment_status,
        transaction_id!==undefined ? transaction_id : e.transaction_id,
        notes!==undefined ? notes : e.notes,
        id
      ]
    );
    const r = await pool.query('SELECT * FROM vw_payments WHERE payment_id = $1', [id]);
    res.json(r.rows[0]);
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.delete('/api/payments/:id', async (req, res) => {
  try {
    const r = await pool.query('DELETE FROM payments WHERE payment_id = $1 RETURNING *', [req.params.id]);
    if (!r.rows.length) return res.status(404).json({ error: 'Payment not found' });
    res.json({ message: 'Payment deleted' });
  } catch (err) { res.status(500).json({ error: err.message }); }
});

app.listen(PORT, () => {
  console.log(`Backend API running on http://localhost:${PORT}`);
});
