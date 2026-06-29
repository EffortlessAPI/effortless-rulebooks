const express = require('express');
const { pool, newId } = require('./db');

const router = express.Router();

router.use((req, res, next) => {
  if (!req.session.cart) req.session.cart = [];
  res.locals.cartCount = req.session.cart.reduce((s, x) => s + x.qty, 0);
  res.locals.activeCustomerId = req.session.activeCustomerId || null;
  next();
});

async function hydrateCart(cart) {
  if (!cart.length) return { items: [], subTotal: 0 };
  const ids = cart.map(c => c.productId);
  const { rows } = await pool.query(
    `SELECT product_id, display_name, sku, unit_price FROM vw_products WHERE product_id = ANY($1)`,
    [ids]
  );
  const byId = Object.fromEntries(rows.map(r => [r.product_id, r]));
  const items = cart
    .map(c => {
      const p = byId[c.productId];
      if (!p) return null;
      const lineTotal = Number(p.unit_price) * c.qty;
      return { ...p, qty: c.qty, lineTotal };
    })
    .filter(Boolean);
  const subTotal = items.reduce((s, x) => s + x.lineTotal, 0);
  return { items, subTotal };
}

// ---- Home ----
router.get('/', async (req, res, next) => {
  try {
    const { rows } = await pool.query(
      `SELECT * FROM vw_products WHERE is_active = true AND COALESCE(stock_quantity, 0) > 0
       ORDER BY display_name LIMIT 6`
    );
    res.render('shop/home', { products: rows });
  } catch (e) { next(e); }
});

// ---- Catalog ----
router.get('/products', async (req, res, next) => {
  try {
    const q = (req.query.q || '').trim();
    const inStock = req.query.in_stock === '1';
    const wheres = ['is_active = true'];
    const vals = [];
    if (q) {
      vals.push(`%${q.toLowerCase()}%`);
      wheres.push(`(LOWER(display_name) LIKE $${vals.length} OR LOWER(COALESCE(description,'')) LIKE $${vals.length})`);
    }
    if (inStock) wheres.push(`COALESCE(stock_quantity,0) > 0`);
    const { rows } = await pool.query(
      `SELECT * FROM vw_products WHERE ${wheres.join(' AND ')} ORDER BY display_name LIMIT 200`,
      vals
    );
    res.render('shop/products', { products: rows, q, inStock });
  } catch (e) { next(e); }
});

// ---- Product detail ----
router.get('/products/:id', async (req, res, next) => {
  try {
    const { rows } = await pool.query(`SELECT * FROM vw_products WHERE product_id = $1`, [req.params.id]);
    if (!rows[0]) return res.status(404).send('Product not found');
    res.render('shop/product', { product: rows[0] });
  } catch (e) { next(e); }
});

// ---- Cart ----
router.get('/cart', async (req, res, next) => {
  try {
    const cart = await hydrateCart(req.session.cart);
    res.render('shop/cart', { cart });
  } catch (e) { next(e); }
});

router.post('/cart/add', (req, res) => {
  const productId = req.body.productId;
  const qty = Math.max(1, parseInt(req.body.qty || '1', 10));
  const existing = req.session.cart.find(c => c.productId === productId);
  if (existing) existing.qty += qty;
  else req.session.cart.push({ productId, qty });
  res.redirect('/shop/cart');
});

router.post('/cart/update', (req, res) => {
  const productId = req.body.productId;
  const qty = parseInt(req.body.qty || '0', 10);
  if (qty <= 0) {
    req.session.cart = req.session.cart.filter(c => c.productId !== productId);
  } else {
    const existing = req.session.cart.find(c => c.productId === productId);
    if (existing) existing.qty = qty;
  }
  res.redirect('/shop/cart');
});

router.post('/cart/clear', (req, res) => {
  req.session.cart = [];
  res.redirect('/shop/cart');
});

// ---- Checkout ----
router.get('/checkout', async (req, res, next) => {
  try {
    const cart = await hydrateCart(req.session.cart);
    const { rows: clients } = await pool.query(
      `SELECT client_id, name, email FROM vw_clients ORDER BY name`
    );
    const { rows: addresses } = await pool.query(
      `SELECT addresse_id, name, clients FROM vw_addresses ORDER BY name`
    );
    res.render('shop/checkout', { cart, clients, addresses });
  } catch (e) { next(e); }
});

router.post('/checkout', async (req, res, next) => {
  const cart = await hydrateCart(req.session.cart);
  if (!cart.items.length) return res.redirect('/shop/cart');
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    let clientId;
    if (req.body.customer_mode === 'new') {
      clientId = newId('client');
      await client.query(
        `INSERT INTO clients (client_id, name, email, phone, company_name, created_at)
         VALUES ($1, $2, $3, $4, $5, NOW())`,
        [clientId, req.body.name || 'Guest', req.body.email || null,
         req.body.phone || null, req.body.company_name || null]
      );
    } else {
      clientId = req.body.client_id;
      if (!clientId) throw new Error('Missing client');
    }

    const invoiceId = newId('invoice');
    const invoiceNumber = 'ORD-' + String(Date.now()).slice(-7);
    await client.query(
      `INSERT INTO invoices (invoice_id, invoice_number, client, order_date, order_status,
                             shipping_address, billing_address, notes)
       VALUES ($1, $2, $3, NOW(), $4, $5, $6, $7)`,
      [invoiceId, invoiceNumber, clientId, 'placed',
       req.body.shipping_address || null, req.body.billing_address || null,
       req.body.notes || null]
    );

    let n = 1;
    for (const item of cart.items) {
      await client.query(
        `INSERT INTO invoice_line_items (invoice_line_item_id, line_number, invoice, product, quantity)
         VALUES ($1, $2, $3, $4, $5)`,
        [newId('invoice_line_item'), String(n).padStart(3, '0'), invoiceId, item.product_id, item.qty]
      );
      n++;
    }

    await client.query('COMMIT');
    req.session.cart = [];
    req.session.activeCustomerId = clientId;
    res.redirect(`/shop/account/${clientId}/orders/${invoiceId}`);
  } catch (e) {
    await client.query('ROLLBACK');
    next(e);
  } finally {
    client.release();
  }
});

// ---- Account ----
router.get('/account', async (req, res, next) => {
  try {
    const { rows } = await pool.query(`SELECT * FROM vw_clients ORDER BY name`);
    res.render('shop/account_picker', { clients: rows });
  } catch (e) { next(e); }
});

router.post('/account/switch', (req, res) => {
  req.session.activeCustomerId = req.body.client_id || null;
  if (req.session.activeCustomerId) {
    return res.redirect(`/shop/account/${req.session.activeCustomerId}`);
  }
  res.redirect('/shop/account');
});

async function getClient(cid) {
  const { rows } = await pool.query(`SELECT * FROM vw_clients WHERE client_id = $1`, [cid]);
  return rows[0];
}

router.get('/account/:cid', async (req, res, next) => {
  try {
    const client = await getClient(req.params.cid);
    if (!client) return res.status(404).send('Client not found');
    const { rows: orders } = await pool.query(
      `SELECT * FROM vw_invoices WHERE client = $1 ORDER BY order_date DESC NULLS LAST LIMIT 5`,
      [req.params.cid]
    );
    const { rows: agg } = await pool.query(
      `SELECT COALESCE(SUM(invoice_total),0) AS lifetime,
              COALESCE(SUM(amount_due),0) AS open_balance
         FROM vw_invoices WHERE client = $1`,
      [req.params.cid]
    );
    res.render('shop/account_overview', { customer: client, orders, agg: agg[0] });
  } catch (e) { next(e); }
});

router.get('/account/:cid/orders', async (req, res, next) => {
  try {
    const client = await getClient(req.params.cid);
    if (!client) return res.status(404).send('Client not found');
    const { rows } = await pool.query(
      `SELECT * FROM vw_invoices WHERE client = $1 ORDER BY order_date DESC NULLS LAST`,
      [req.params.cid]
    );
    res.render('shop/orders', { customer: client, orders: rows });
  } catch (e) { next(e); }
});

router.get('/account/:cid/orders/:oid', async (req, res, next) => {
  try {
    const client = await getClient(req.params.cid);
    if (!client) return res.status(404).send('Client not found');
    const { rows: orderRows } = await pool.query(
      `SELECT * FROM vw_invoices WHERE invoice_id = $1`, [req.params.oid]
    );
    const order = orderRows[0];
    if (!order) return res.status(404).send('Order not found');
    const { rows: items } = await pool.query(
      `SELECT * FROM vw_invoice_line_items WHERE invoice = $1 ORDER BY line_number`,
      [req.params.oid]
    );
    const { rows: payments } = await pool.query(
      `SELECT * FROM vw_payments WHERE invoice = $1 ORDER BY payment_date DESC NULLS LAST`,
      [req.params.oid]
    );
    res.render('shop/order_detail', { customer: client, order, items, payments });
  } catch (e) { next(e); }
});

router.post('/account/:cid/orders/:oid/pay', async (req, res, next) => {
  try {
    const amount = Number(req.body.amount);
    if (!(amount > 0)) return res.redirect(`/shop/account/${req.params.cid}/orders/${req.params.oid}`);
    await pool.query(
      `INSERT INTO payments
        (payment_id, payment_number, invoice, payment_date, payment_method,
         amount, payment_status, notes)
       VALUES ($1, $2, $3, NOW(), $4, $5, 'completed', $6)`,
      [newId('payment'), 'PAY-' + String(Date.now()).slice(-7),
       req.params.oid, req.body.payment_method || 'card', amount, req.body.notes || null]
    );
    res.redirect(`/shop/account/${req.params.cid}/orders/${req.params.oid}`);
  } catch (e) { next(e); }
});

router.get('/account/:cid/payments', async (req, res, next) => {
  try {
    const client = await getClient(req.params.cid);
    if (!client) return res.status(404).send('Client not found');
    const { rows } = await pool.query(
      `SELECT p.* FROM vw_payments p
        JOIN vw_invoices i ON i.invoice_id = p.invoice
        WHERE i.client = $1
        ORDER BY p.payment_date DESC NULLS LAST`,
      [req.params.cid]
    );
    res.render('shop/payments', { customer: client, payments: rows });
  } catch (e) { next(e); }
});

router.get('/account/:cid/profile', async (req, res, next) => {
  try {
    const client = await getClient(req.params.cid);
    if (!client) return res.status(404).send('Client not found');
    const { rows: addresses } = await pool.query(
      `SELECT * FROM vw_addresses WHERE clients = $1 ORDER BY name`, [req.params.cid]
    );
    const { rows: states } = await pool.query(`SELECT * FROM vw_states ORDER BY text`);
    const { rows: types } = await pool.query(`SELECT * FROM vw_types_of_addresses ORDER BY name`);
    res.render('shop/profile', { customer: client, addresses, states, types });
  } catch (e) { next(e); }
});

router.post('/account/:cid/profile', async (req, res, next) => {
  try {
    await pool.query(
      `UPDATE clients SET name=$1, email=$2, phone=$3, company_name=$4, notes=$5
        WHERE client_id=$6`,
      [req.body.name, req.body.email || null, req.body.phone || null,
       req.body.company_name || null, req.body.notes || null, req.params.cid]
    );
    res.redirect(`/shop/account/${req.params.cid}/profile`);
  } catch (e) { next(e); }
});

router.post('/account/:cid/addresses', async (req, res, next) => {
  try {
    await pool.query(
      `INSERT INTO addresses
        (addresse_id, clients, type_of_address, attention, address1, address2, city, state, zip)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9)`,
      [newId('address'), req.params.cid, req.body.type_of_address || null,
       req.body.attention || null, req.body.address1 || null, req.body.address2 || null,
       req.body.city || null, req.body.state || null, req.body.zip || null]
    );
    res.redirect(`/shop/account/${req.params.cid}/profile`);
  } catch (e) { next(e); }
});

router.post('/account/:cid/addresses/:aid', async (req, res, next) => {
  try {
    await pool.query(
      `UPDATE addresses SET type_of_address=$1, attention=$2, address1=$3, address2=$4,
         city=$5, state=$6, zip=$7 WHERE addresse_id=$8`,
      [req.body.type_of_address || null, req.body.attention || null,
       req.body.address1 || null, req.body.address2 || null, req.body.city || null,
       req.body.state || null, req.body.zip || null, req.params.aid]
    );
    res.redirect(`/shop/account/${req.params.cid}/profile`);
  } catch (e) { next(e); }
});

router.post('/account/:cid/addresses/:aid/delete', async (req, res, next) => {
  try {
    await pool.query(`DELETE FROM addresses WHERE addresse_id = $1`, [req.params.aid]);
    res.redirect(`/shop/account/${req.params.cid}/profile`);
  } catch (e) { next(e); }
});

// ---- States (tax rates) ----
router.get('/states', async (req, res, next) => {
  try {
    const { rows } = await pool.query(`SELECT * FROM vw_states ORDER BY text`);
    res.render('shop/states', { states: rows });
  } catch (e) { next(e); }
});

router.post('/states/:sid', async (req, res, next) => {
  try {
    await pool.query(
      `UPDATE states SET tax_rate = $1 WHERE state_id = $2`,
      [Number(req.body.tax_rate), req.params.sid]
    );
    res.redirect('/shop/states');
  } catch (e) { next(e); }
});

module.exports = router;
