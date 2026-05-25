-- Migration 003: v3 — Products, Orders, Line Items, Payments

-- 1. Extend customers with contact / identity fields
ALTER TABLE customers ADD COLUMN IF NOT EXISTS company_name    TEXT NOT NULL DEFAULT '';
ALTER TABLE customers ADD COLUMN IF NOT EXISTS email           TEXT NOT NULL DEFAULT '';
ALTER TABLE customers ADD COLUMN IF NOT EXISTS phone           TEXT NOT NULL DEFAULT '';
ALTER TABLE customers ADD COLUMN IF NOT EXISTS billing_address TEXT NOT NULL DEFAULT '';
ALTER TABLE customers ADD COLUMN IF NOT EXISTS shipping_address TEXT NOT NULL DEFAULT '';

-- 2. Add sort_order to statuses
ALTER TABLE statuses ADD COLUMN IF NOT EXISTS sort_order INTEGER NOT NULL DEFAULT 0;

UPDATE statuses SET sort_order = 1 WHERE slug = 'new';
UPDATE statuses SET sort_order = 2 WHERE slug = 'processing';
UPDATE statuses SET sort_order = 3 WHERE slug = 'in-review';
UPDATE statuses SET sort_order = 4 WHERE slug = 'pending';
UPDATE statuses SET sort_order = 5 WHERE slug = 'on-hold';
UPDATE statuses SET sort_order = 6 WHERE slug = 'delayed';
UPDATE statuses SET sort_order = 7 WHERE slug = 'cancelled';

-- Assign Brian Lee "New" status if he currently has none (v3 spec change from v2)
UPDATE customers
SET status_id = (SELECT id FROM statuses WHERE slug = 'new')
WHERE slug = 'brian-lee' AND status_id IS NULL;

-- 3. Products table
CREATE TABLE IF NOT EXISTS products (
  id             SERIAL PRIMARY KEY,
  sku            TEXT NOT NULL UNIQUE,
  slug           TEXT NOT NULL UNIQUE,
  display_name   TEXT NOT NULL DEFAULT '',
  description    TEXT NOT NULL DEFAULT '',
  unit_price     NUMERIC(12,2) NOT NULL DEFAULT 0,
  cost           NUMERIC(12,2) NOT NULL DEFAULT 0,
  stock_quantity INTEGER,
  is_active      BOOLEAN NOT NULL DEFAULT TRUE,
  created_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

INSERT INTO products (sku, slug, display_name, unit_price, cost, stock_quantity, is_active)
SELECT * FROM (VALUES
  ('WIDGET-001',   'widget-001',   'Standard Widget',    19.99::numeric,  7.50::numeric,  NULL::integer, true),
  ('WIDGET-002',   'widget-002',   'Deluxe Widget',      39.99::numeric, 14.20::numeric,  NULL,           true),
  ('GADGET-100',   'gadget-100',   'Pocket Gadget',      24.50::numeric,  9.10::numeric,  NULL,           true),
  ('GADGET-200',   'gadget-200',   'Pro Gadget',         89.00::numeric, 32.00::numeric,  NULL,           true),
  ('CABLE-USB-C',  'cable-usb-c',  'USB-C Cable (2m)',   12.00::numeric,  3.40::numeric,  NULL,           true),
  ('HUB-7PORT',    'hub-7port',    '7-Port USB Hub',     34.99::numeric, 11.80::numeric,  NULL,           true),
  ('STAND-ALU',    'stand-alu',    'Aluminum Stand',     45.00::numeric, 18.75::numeric,  NULL,           true),
  ('CASE-LEATHER', 'case-leather', 'Leather Case',       65.00::numeric, 22.50::numeric,     0,           true)
) AS v(sku, slug, display_name, unit_price, cost, stock_quantity, is_active)
WHERE NOT EXISTS (SELECT 1 FROM products LIMIT 1);

-- 4. Orders table
CREATE TABLE IF NOT EXISTS orders (
  id               SERIAL PRIMARY KEY,
  order_number     TEXT NOT NULL,
  slug             TEXT NOT NULL UNIQUE,
  customer_id      INTEGER NOT NULL REFERENCES customers(id),
  order_date       TIMESTAMPTZ,
  order_status     TEXT NOT NULL DEFAULT 'New',
  shipping_address TEXT NOT NULL DEFAULT '',
  billing_address  TEXT NOT NULL DEFAULT '',
  tax_rate         NUMERIC(8,6) NOT NULL DEFAULT 0,
  notes            TEXT NOT NULL DEFAULT '',
  created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 5. Order line items table
CREATE TABLE IF NOT EXISTS order_line_items (
  id               SERIAL PRIMARY KEY,
  order_id         INTEGER NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
  line_number      INTEGER NOT NULL,
  slug             TEXT NOT NULL UNIQUE,
  product_id       INTEGER NOT NULL REFERENCES products(id),
  quantity         INTEGER NOT NULL DEFAULT 1,
  unit_price       NUMERIC(12,2) NOT NULL DEFAULT 0,
  discount_percent NUMERIC(8,6) NOT NULL DEFAULT 0,
  notes            TEXT NOT NULL DEFAULT '',
  created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 6. Payments table
CREATE TABLE IF NOT EXISTS payments (
  id              SERIAL PRIMARY KEY,
  order_id        INTEGER NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
  payment_number  INTEGER NOT NULL,
  slug            TEXT NOT NULL UNIQUE,
  payment_date    TIMESTAMPTZ,
  amount          NUMERIC(12,2) NOT NULL DEFAULT 0,
  payment_method  TEXT NOT NULL DEFAULT '',
  payment_status  TEXT NOT NULL DEFAULT 'Pending',
  transaction_id  TEXT NOT NULL DEFAULT '',
  notes           TEXT NOT NULL DEFAULT '',
  created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 7. Seed data (only if orders table is empty)
DO $$
DECLARE
  bob_id          INTEGER;
  alice_id        INTEGER;
  brian_id        INTEGER;
  carla_id        INTEGER;
  caroline_id     INTEGER;

  widget001_id    INTEGER;
  widget002_id    INTEGER;
  gadget100_id    INTEGER;
  gadget200_id    INTEGER;
  cable_id        INTEGER;
  hub_id          INTEGER;
  stand_id        INTEGER;
  leather_id      INTEGER;

  bob1001_id      INTEGER;
  bob1002_id      INTEGER;
  alice2001_id    INTEGER;
  brian3001_id    INTEGER;
  carla4001_id    INTEGER;
  carla4002_id    INTEGER;
  caroline5001_id INTEGER;
BEGIN
  IF EXISTS (SELECT 1 FROM orders LIMIT 1) THEN
    RETURN;
  END IF;

  -- Customer IDs
  SELECT id INTO bob_id      FROM customers WHERE slug = 'bob';
  SELECT id INTO alice_id    FROM customers WHERE slug = 'alice-johnson';
  SELECT id INTO brian_id    FROM customers WHERE slug = 'brian-lee';
  SELECT id INTO carla_id    FROM customers WHERE slug = 'carla-smith';
  SELECT id INTO caroline_id FROM customers WHERE slug = 'caroline';

  -- Product IDs
  SELECT id INTO widget001_id  FROM products WHERE sku = 'WIDGET-001';
  SELECT id INTO widget002_id  FROM products WHERE sku = 'WIDGET-002';
  SELECT id INTO gadget100_id  FROM products WHERE sku = 'GADGET-100';
  SELECT id INTO gadget200_id  FROM products WHERE sku = 'GADGET-200';
  SELECT id INTO cable_id      FROM products WHERE sku = 'CABLE-USB-C';
  SELECT id INTO hub_id        FROM products WHERE sku = 'HUB-7PORT';
  SELECT id INTO stand_id      FROM products WHERE sku = 'STAND-ALU';
  SELECT id INTO leather_id    FROM products WHERE sku = 'CASE-LEATHER';

  -- Update customers with contact info
  UPDATE customers SET
    company_name     = 'Bob''s Widgets Inc.',
    email            = 'bob@example.com',
    phone            = '555-101-2020',
    billing_address  = '123 Main St, Springfield, IL 62701',
    shipping_address = '123 Main St, Springfield, IL 62701'
  WHERE slug = 'bob';

  UPDATE customers SET
    company_name     = 'AJ Consulting',
    email            = 'alice@example.com',
    phone            = '555-202-3030',
    billing_address  = '456 Oak Ave, Chicago, IL 60601',
    shipping_address = '456 Oak Ave, Chicago, IL 60601'
  WHERE slug = 'alice-johnson';

  UPDATE customers SET
    company_name     = 'Brian Lee Designs',
    email            = 'brian@example.com',
    phone            = '555-303-4040',
    billing_address  = '789 Elm St, Peoria, IL 61602',
    shipping_address = '789 Elm St, Peoria, IL 61602'
  WHERE slug = 'brian-lee';

  UPDATE customers SET
    company_name     = 'Carla & Co',
    email            = 'carla@example.com',
    phone            = '555-404-5050',
    billing_address  = '321 Pine Rd, Rockford, IL 61101',
    shipping_address = '321 Pine Rd, Rockford, IL 61101'
  WHERE slug = 'carla-smith';

  UPDATE customers SET
    email = 'caroline@example.com',
    phone = '555-505-6060'
  WHERE slug = 'caroline';

  -- ── Orders ────────────────────────────────────────────────────────────────

  -- Bob #1001 (Delivered) — will be Paid in full
  -- Lines: GADGET-200 x1 @89.00 -10%=80.10, STAND-ALU x2 @45.00=90.00, CABLE-USB-C x5 @12.00=60.00
  -- Subtotal=230.10, Tax8.5%=19.56, Total=249.66
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate, notes)
  VALUES ('1001', 'bob-1001', bob_id, '2026-03-15 10:00:00+00', 'Delivered', 0.085000, 'First order from Bob.')
  RETURNING id INTO bob1001_id;

  -- Bob #1002 (Pending) — Unpaid (Failed + Pending payments)
  -- Lines: WIDGET-001 x3 @19.99=59.97, HUB-7PORT x1 @34.99=34.99
  -- Subtotal=94.96, Tax8.5%=8.07, Total=103.03
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate)
  VALUES ('1002', 'bob-1002', bob_id, '2026-04-01 09:00:00+00', 'Pending', 0.085000)
  RETURNING id INTO bob1002_id;

  -- Alice #2001 (Shipped) — Partial
  -- Lines: GADGET-100 x2 @24.50=49.00, CASE-LEATHER x1 @65.00 -15%=55.25
  -- Subtotal=104.25, Tax0%=0, Total=104.25
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate)
  VALUES ('2001', 'alice-johnson-2001', alice_id, '2026-03-20 14:00:00+00', 'Shipped', 0.000000)
  RETURNING id INTO alice2001_id;

  -- Brian #3001 (Processing) — Unpaid (no payments)
  -- Lines: WIDGET-002 x2 @39.99=79.98, GADGET-100 x1 @24.50=24.50, CABLE-USB-C x4 @12.00=48.00
  -- Subtotal=152.48, Tax8.5%=12.96, Total=165.44
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate)
  VALUES ('3001', 'brian-lee-3001', brian_id, '2026-04-05 11:00:00+00', 'Processing', 0.085000)
  RETURNING id INTO brian3001_id;

  -- Carla #4001 (Returned) — Paid in full
  -- Lines: WIDGET-001 x4 @19.99=79.96, CABLE-USB-C x2 @12.00=24.00
  -- Subtotal=103.96, Tax8.5%=8.84, Total=112.80
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate)
  VALUES ('4001', 'carla-smith-4001', carla_id, '2026-03-10 08:00:00+00', 'Returned', 0.085000)
  RETURNING id INTO carla4001_id;

  -- Carla #4002 (Processing) — Partial
  -- Lines: GADGET-200 x1 @89.00 -15%=75.65, STAND-ALU x1 @45.00=45.00
  -- Subtotal=120.65, Tax8.5%=10.26, Total=130.91
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate)
  VALUES ('4002', 'carla-smith-4002', carla_id, '2026-04-10 13:00:00+00', 'Processing', 0.085000)
  RETURNING id INTO carla4002_id;

  -- Caroline #5001 (New) — Partial
  -- Lines: HUB-7PORT x1 @34.99=34.99, WIDGET-001 x2 @19.99=39.98, STAND-ALU x1 @45.00=45.00, CASE-LEATHER x1 @65.00=65.00
  -- Subtotal=184.97, Tax8.5%=15.72, Total=200.69
  INSERT INTO orders (order_number, slug, customer_id, order_date, order_status, tax_rate)
  VALUES ('5001', 'caroline-5001', caroline_id, '2026-04-15 16:00:00+00', 'New', 0.085000)
  RETURNING id INTO caroline5001_id;

  -- ── Line Items ─────────────────────────────────────────────────────────────

  -- Bob #1001 (3 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (bob1001_id, 1, 'bob-1001-line-1', gadget200_id, 1, 89.00, 0.100000),
    (bob1001_id, 2, 'bob-1001-line-2', stand_id,     2, 45.00, 0.000000),
    (bob1001_id, 3, 'bob-1001-line-3', cable_id,     5, 12.00, 0.000000);

  -- Bob #1002 (2 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (bob1002_id, 1, 'bob-1002-line-1', widget001_id, 3, 19.99, 0.000000),
    (bob1002_id, 2, 'bob-1002-line-2', hub_id,       1, 34.99, 0.000000);

  -- Alice #2001 (2 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (alice2001_id, 1, 'alice-johnson-2001-line-1', gadget100_id, 2, 24.50, 0.000000),
    (alice2001_id, 2, 'alice-johnson-2001-line-2', leather_id,   1, 65.00, 0.150000);

  -- Brian #3001 (3 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (brian3001_id, 1, 'brian-lee-3001-line-1', widget002_id, 2, 39.99, 0.000000),
    (brian3001_id, 2, 'brian-lee-3001-line-2', gadget100_id, 1, 24.50, 0.000000),
    (brian3001_id, 3, 'brian-lee-3001-line-3', cable_id,     4, 12.00, 0.000000);

  -- Carla #4001 (2 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (carla4001_id, 1, 'carla-smith-4001-line-1', widget001_id, 4, 19.99, 0.000000),
    (carla4001_id, 2, 'carla-smith-4001-line-2', cable_id,     2, 12.00, 0.000000);

  -- Carla #4002 (2 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (carla4002_id, 1, 'carla-smith-4002-line-1', gadget200_id, 1, 89.00, 0.150000),
    (carla4002_id, 2, 'carla-smith-4002-line-2', stand_id,     1, 45.00, 0.000000);

  -- Caroline #5001 (4 lines)
  INSERT INTO order_line_items (order_id, line_number, slug, product_id, quantity, unit_price, discount_percent) VALUES
    (caroline5001_id, 1, 'caroline-5001-line-1', hub_id,       1, 34.99, 0.000000),
    (caroline5001_id, 2, 'caroline-5001-line-2', widget001_id, 2, 19.99, 0.000000),
    (caroline5001_id, 3, 'caroline-5001-line-3', stand_id,     1, 45.00, 0.000000),
    (caroline5001_id, 4, 'caroline-5001-line-4', leather_id,   1, 65.00, 0.000000);

  -- ── Payments ───────────────────────────────────────────────────────────────

  -- Bob #1001: 2 Completed → 100.00 + 149.66 = 249.66 (fully paid)
  INSERT INTO payments (order_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id) VALUES
    (bob1001_id, 1, 'bob-1001-pmt-1', '2026-03-16 10:00:00+00', 100.00, 'CreditCard',   'Completed', 'ch_bob1001_deposit'),
    (bob1001_id, 2, 'bob-1001-pmt-2', '2026-03-20 10:00:00+00', 149.66, 'CreditCard',   'Completed', 'ch_bob1001_final');

  -- Bob #1002: 1 Failed + 1 Pending → 0 completed (unpaid)
  INSERT INTO payments (order_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id, notes) VALUES
    (bob1002_id, 1, 'bob-1002-pmt-1', '2026-04-02 09:00:00+00',  50.00, 'CreditCard',   'Failed',    'ch_bob1002_declined', 'Card declined by issuer.'),
    (bob1002_id, 2, 'bob-1002-pmt-2', '2026-04-05 09:00:00+00', 103.03, 'BankTransfer', 'Pending',   'ach_bob1002_pending',  'ACH transfer initiated, awaiting clearance.');

  -- Alice #2001: 1 Completed → 50.00 of 104.25 (partial)
  INSERT INTO payments (order_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id) VALUES
    (alice2001_id, 1, 'alice-johnson-2001-pmt-1', '2026-03-21 14:00:00+00', 50.00, 'CreditCard', 'Completed', 'ch_alice2001_deposit');

  -- Brian #3001: no payments (unpaid)

  -- Carla #4001: 1 Completed → 112.80 (fully paid)
  INSERT INTO payments (order_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id) VALUES
    (carla4001_id, 1, 'carla-smith-4001-pmt-1', '2026-03-11 08:00:00+00', 112.80, 'BankTransfer', 'Completed', 'wire_carla4001_full');

  -- Carla #4002: 1 Completed + 1 Pending → 60.00 of 130.91 (partial)
  INSERT INTO payments (order_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id) VALUES
    (carla4002_id, 1, 'carla-smith-4002-pmt-1', '2026-04-11 13:00:00+00',  60.00, 'CreditCard',   'Completed', 'ch_carla4002_partial'),
    (carla4002_id, 2, 'carla-smith-4002-pmt-2', '2026-04-15 13:00:00+00',  70.91, 'BankTransfer', 'Pending',   'ach_carla4002_balance');

  -- Caroline #5001: 1 Completed → 100.00 of 200.69 (partial)
  INSERT INTO payments (order_id, payment_number, slug, payment_date, amount, payment_method, payment_status, transaction_id) VALUES
    (caroline5001_id, 1, 'caroline-5001-pmt-1', '2026-04-16 16:00:00+00', 100.00, 'Cash', 'Completed', '');

END $$;
