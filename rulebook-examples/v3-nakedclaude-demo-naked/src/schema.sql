-- naked-claude-demo schema
-- Base tables hold only editable columns (mirroring Airtable).
-- vw_* views compute every derived field so reads see the same numbers
-- whether the row was edited in Airtable or in the app.

DROP VIEW IF EXISTS vw_payments CASCADE;
DROP VIEW IF EXISTS vw_invoice_line_items CASCADE;
DROP VIEW IF EXISTS vw_invoices CASCADE;
DROP VIEW IF EXISTS vw_inventory_adjustments CASCADE;
DROP VIEW IF EXISTS vw_products CASCADE;
DROP VIEW IF EXISTS vw_client_approvals CASCADE;
DROP VIEW IF EXISTS vw_clients CASCADE;
DROP VIEW IF EXISTS vw_client_categories CASCADE;
DROP VIEW IF EXISTS vw_app_users CASCADE;
DROP VIEW IF EXISTS vw_addresses CASCADE;
DROP VIEW IF EXISTS vw_types_of_addresses CASCADE;
DROP VIEW IF EXISTS vw_states CASCADE;
DROP VIEW IF EXISTS vw_statuses CASCADE;

DROP TABLE IF EXISTS payments CASCADE;
DROP TABLE IF EXISTS invoice_line_items CASCADE;
DROP TABLE IF EXISTS invoices CASCADE;
DROP TABLE IF EXISTS inventory_adjustments CASCADE;
DROP TABLE IF EXISTS products CASCADE;
DROP TABLE IF EXISTS client_approvals CASCADE;
DROP TABLE IF EXISTS clients CASCADE;
DROP TABLE IF EXISTS client_categories CASCADE;
DROP TABLE IF EXISTS app_users CASCADE;
DROP TABLE IF EXISTS addresses CASCADE;
DROP TABLE IF EXISTS types_of_addresses CASCADE;
DROP TABLE IF EXISTS states CASCADE;
DROP TABLE IF EXISTS statuses CASCADE;

-- ----- base tables ---------------------------------------------------------

CREATE TABLE statuses (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  is_blocking BOOLEAN DEFAULT FALSE,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE states (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  code TEXT,
  tax_rate NUMERIC(6,4) DEFAULT 0,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE types_of_addresses (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE addresses (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  street TEXT,
  city TEXT,
  zip TEXT,
  state_id TEXT REFERENCES states(airtable_id) ON DELETE SET NULL,
  type_of_address_id TEXT REFERENCES types_of_addresses(airtable_id) ON DELETE SET NULL,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE app_users (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  email TEXT,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE client_categories (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  discount NUMERIC(6,4) DEFAULT 0,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE clients (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  email TEXT,
  phone TEXT,
  status_id TEXT REFERENCES statuses(airtable_id) ON DELETE SET NULL,
  client_category_id TEXT REFERENCES client_categories(airtable_id) ON DELETE SET NULL,
  address_id TEXT REFERENCES addresses(airtable_id) ON DELETE SET NULL,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE client_approvals (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  client_id TEXT REFERENCES clients(airtable_id) ON DELETE CASCADE,
  approved_by_id TEXT REFERENCES app_users(airtable_id) ON DELETE SET NULL,
  approval_date DATE,
  notes TEXT,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE products (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  sku TEXT,
  unit_price NUMERIC(12,2) DEFAULT 0,
  cost NUMERIC(12,2) DEFAULT 0,
  is_active BOOLEAN DEFAULT TRUE,
  description TEXT,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE inventory_adjustments (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  product_id TEXT REFERENCES products(airtable_id) ON DELETE CASCADE,
  adjustment_type TEXT,                 -- 'Addition' | 'Correction' | 'Removal'
  quantity NUMERIC(14,2) DEFAULT 0,
  adjusted_at TIMESTAMPTZ DEFAULT NOW(),
  notes TEXT,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE invoices (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  client_id TEXT REFERENCES clients(airtable_id) ON DELETE SET NULL,
  invoice_date DATE DEFAULT CURRENT_DATE,
  tax_rate NUMERIC(6,4) DEFAULT 0,        -- stored fallback
  notes TEXT,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE invoice_line_items (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  invoice_id TEXT REFERENCES invoices(airtable_id) ON DELETE CASCADE,
  product_id TEXT REFERENCES products(airtable_id) ON DELETE SET NULL,
  quantity NUMERIC(14,2) DEFAULT 1,
  unit_price NUMERIC(12,2) DEFAULT 0,
  discount_percent NUMERIC(6,4) DEFAULT 0, -- 0..1
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE payments (
  airtable_id TEXT PRIMARY KEY,
  name TEXT,
  invoice_id TEXT REFERENCES invoices(airtable_id) ON DELETE CASCADE,
  amount NUMERIC(12,2) DEFAULT 0,
  payment_status TEXT DEFAULT 'Pending',   -- 'Completed' counts toward total_paid
  payment_method TEXT,
  payment_date DATE DEFAULT CURRENT_DATE,
  synced_at TIMESTAMPTZ DEFAULT NOW()
);

-- ----- views ---------------------------------------------------------------

CREATE VIEW vw_statuses AS SELECT * FROM statuses;
CREATE VIEW vw_states AS SELECT * FROM states;
CREATE VIEW vw_types_of_addresses AS SELECT * FROM types_of_addresses;
CREATE VIEW vw_app_users AS SELECT * FROM app_users;
CREATE VIEW vw_client_categories AS SELECT * FROM client_categories;

CREATE VIEW vw_addresses AS
SELECT
  a.*,
  s.code  AS state_code,
  s.name  AS state_name,
  s.tax_rate AS state_tax_rate,
  t.name  AS type_of_address_name,
  CONCAT_WS(', ',
    NULLIF(a.street, ''),
    NULLIF(a.city, ''),
    NULLIF(CONCAT_WS(' ', s.code, NULLIF(a.zip,'')), '')
  ) AS address_formatted
FROM addresses a
LEFT JOIN states s              ON s.airtable_id = a.state_id
LEFT JOIN types_of_addresses t  ON t.airtable_id = a.type_of_address_id;

CREATE VIEW vw_inventory_adjustments AS
SELECT
  ia.*,
  p.name AS product_name,
  CASE ia.adjustment_type
    WHEN 'Removal' THEN -ia.quantity
    ELSE ia.quantity
  END AS signed_quantity
FROM inventory_adjustments ia
LEFT JOIN products p ON p.airtable_id = ia.product_id;

-- product rollups (stock, margin) + VIP-order flag
CREATE VIEW vw_products AS
WITH stock AS (
  SELECT product_id,
         COALESCE(SUM(CASE WHEN adjustment_type='Removal' THEN -quantity ELSE quantity END),0) AS stock_quantity
  FROM inventory_adjustments
  GROUP BY product_id
),
client_avg AS (
  SELECT
    i.client_id,
    AVG(li_totals.invoice_total) AS avg_order_value,
    MAX(i.invoice_date) AS last_invoice_date
  FROM invoices i
  JOIN (
    SELECT invoice_id, SUM(quantity * unit_price * (1 - COALESCE(discount_percent,0))) AS invoice_total
    FROM invoice_line_items GROUP BY invoice_id
  ) li_totals ON li_totals.invoice_id = i.airtable_id
  GROUP BY i.client_id
),
vip_clients AS (
  SELECT client_id FROM client_avg
  WHERE avg_order_value > 500
    AND last_invoice_date > (CURRENT_DATE - INTERVAL '220 days')
),
vip_orders AS (
  SELECT li.product_id, COUNT(DISTINCT i.airtable_id) AS count_of_vip_orders
  FROM invoice_line_items li
  JOIN invoices i ON i.airtable_id = li.invoice_id
  WHERE i.client_id IN (SELECT client_id FROM vip_clients)
  GROUP BY li.product_id
)
SELECT
  p.*,
  COALESCE(s.stock_quantity, 0)               AS stock_quantity,
  CASE WHEN p.unit_price > 0
       THEN ROUND((1 - p.cost / p.unit_price)::numeric, 4)
       ELSE 0 END                              AS margin,
  (p.unit_price > 0 AND (1 - p.cost / p.unit_price) > 0.65) AS is_high_margin,
  (p.cost)                                    AS cogs,
  (p.unit_price - p.cost)                     AS profit,
  COALESCE(vo.count_of_vip_orders, 0)         AS count_of_vip_orders,
  (COALESCE(vo.count_of_vip_orders,0) > 0 AND COALESCE(s.stock_quantity,0) > 250) AS has_been_ordered_by_vip_customers
FROM products p
LEFT JOIN stock s      ON s.product_id = p.airtable_id
LEFT JOIN vip_orders vo ON vo.product_id = p.airtable_id;

CREATE VIEW vw_invoice_line_items AS
SELECT
  li.*,
  p.name AS product_name,
  p.sku  AS product_sku,
  (li.quantity * li.unit_price)                                              AS pre_discount,
  (li.quantity * li.unit_price * COALESCE(li.discount_percent,0))            AS discount_amount,
  (li.quantity * li.unit_price * (1 - COALESCE(li.discount_percent,0)))      AS sub_total
FROM invoice_line_items li
LEFT JOIN products p ON p.airtable_id = li.product_id;

CREATE VIEW vw_payments AS
SELECT
  p.*,
  (p.payment_status = 'Completed') AS is_completed,
  CASE WHEN p.payment_status = 'Completed' THEN p.amount ELSE 0 END AS completed_amount
FROM payments p;

-- effective per-client billing-state rate: state of the client's primary address
CREATE VIEW vw_invoices AS
WITH li AS (
  SELECT
    invoice_id,
    SUM(quantity * unit_price)                                          AS pre_discount,
    SUM(quantity * unit_price * COALESCE(discount_percent,0))           AS discount_amount,
    SUM(quantity * unit_price * (1 - COALESCE(discount_percent,0)))     AS sub_total,
    COUNT(*)                                                            AS line_item_count
  FROM invoice_line_items
  GROUP BY invoice_id
),
pay AS (
  SELECT invoice_id,
         SUM(CASE WHEN payment_status='Completed' THEN amount ELSE 0 END) AS total_paid,
         MAX(CASE WHEN payment_status='Completed' THEN payment_date ELSE NULL END) AS last_payment_date,
         COUNT(*) AS payment_count
  FROM payments
  GROUP BY invoice_id
),
client_state AS (
  SELECT c.airtable_id AS client_id, s.code AS state_code, s.name AS state_name, s.tax_rate AS state_tax_rate
  FROM clients c
  LEFT JOIN addresses a ON a.airtable_id = c.address_id
  LEFT JOIN states s    ON s.airtable_id = a.state_id
)
SELECT
  i.*,
  c.name AS client_name,
  cc.discount AS client_category_discount,
  cs.state_code AS tax_state_code,
  cs.state_name AS tax_state_name,
  cs.state_tax_rate AS tax_state_rate,
  i.tax_rate AS invoice_stored_tax_rate,
  COALESCE(cs.state_tax_rate, i.tax_rate, 0) AS effective_tax_rate,
  COALESCE(cs.state_tax_rate, i.tax_rate, 0) AS tax_rate_effective,
  COALESCE(li.pre_discount, 0)        AS pre_discount,
  COALESCE(li.discount_amount, 0)     AS discount_amount,
  COALESCE(li.sub_total, 0)           AS sub_total,
  COALESCE(li.line_item_count, 0)     AS line_item_count,
  ROUND( (COALESCE(li.sub_total,0) * COALESCE(cs.state_tax_rate, i.tax_rate, 0))::numeric, 2 ) AS tax_amount,
  ROUND( (COALESCE(li.sub_total,0) * (1 + COALESCE(cs.state_tax_rate, i.tax_rate, 0)))::numeric, 2 ) AS invoice_total,
  COALESCE(pay.total_paid, 0)         AS total_paid,
  pay.last_payment_date,
  COALESCE(pay.payment_count, 0)      AS payment_count,
  ROUND(((COALESCE(li.sub_total,0) * (1 + COALESCE(cs.state_tax_rate, i.tax_rate, 0))) - COALESCE(pay.total_paid,0))::numeric, 2) AS amount_due,
  CASE
    WHEN COALESCE(pay.total_paid,0) <= 0 THEN 'Unpaid'
    WHEN COALESCE(pay.total_paid,0) >= ROUND((COALESCE(li.sub_total,0) * (1 + COALESCE(cs.state_tax_rate, i.tax_rate, 0)))::numeric, 2) THEN 'Paid'
    ELSE 'Partial'
  END AS payment_status_label,
  (COALESCE(li.sub_total,0) > 350) AS is_big_order,
  (COALESCE(pay.total_paid,0) >= ROUND((COALESCE(li.sub_total,0) * (1 + COALESCE(cs.state_tax_rate, i.tax_rate, 0)))::numeric, 2)) AS is_paid_in_full
FROM invoices i
LEFT JOIN li         ON li.invoice_id = i.airtable_id
LEFT JOIN pay        ON pay.invoice_id = i.airtable_id
LEFT JOIN clients c  ON c.airtable_id = i.client_id
LEFT JOIN client_categories cc ON cc.airtable_id = c.client_category_id
LEFT JOIN client_state cs ON cs.client_id = c.airtable_id;

CREATE VIEW vw_clients AS
WITH inv_eff AS (
  SELECT
    vi.client_id,
    vi.invoice_total,
    vi.amount_due,
    vi.invoice_date
  FROM vw_invoices vi
),
client_rollup AS (
  SELECT
    client_id,
    COUNT(*)                                AS invoice_count,
    COALESCE(SUM(invoice_total), 0)         AS total_revenue,
    COALESCE(AVG(invoice_total), 0)         AS average_order_value,
    COALESCE(SUM(amount_due), 0)            AS open_balance,
    MAX(invoice_date)                       AS last_invoice_date
  FROM inv_eff
  GROUP BY client_id
)
SELECT
  c.*,
  s.name AS status_name,
  s.is_blocking AS status_is_blocking,
  cc.name AS client_category_name,
  cc.discount AS category_discount,
  va.address_formatted,
  va.state_code,
  va.state_name,
  va.state_tax_rate,
  va.type_of_address_name,
  COALESCE(s.is_blocking, FALSE) AS is_stopped,
  COALESCE(r.invoice_count, 0)          AS invoice_count,
  COALESCE(r.total_revenue, 0)          AS total_revenue,
  COALESCE(r.average_order_value, 0)    AS average_order_value,
  COALESCE(r.open_balance, 0)           AS open_balance,
  r.last_invoice_date,
  (r.last_invoice_date IS NOT NULL AND r.last_invoice_date > (CURRENT_DATE - INTERVAL '220 days')) AS has_recent_invoices,
  (
    COALESCE(r.average_order_value, 0) > 500
    AND r.last_invoice_date IS NOT NULL
    AND r.last_invoice_date > (CURRENT_DATE - INTERVAL '220 days')
  ) AS is_vip
FROM clients c
LEFT JOIN statuses s             ON s.airtable_id = c.status_id
LEFT JOIN client_categories cc   ON cc.airtable_id = c.client_category_id
LEFT JOIN vw_addresses va        ON va.airtable_id = c.address_id
LEFT JOIN client_rollup r        ON r.client_id = c.airtable_id;

CREATE VIEW vw_client_approvals AS
SELECT
  ca.*,
  c.name AS client_name,
  u.name AS approved_by_name,
  (ca.approved_by_id IS NOT NULL) AS is_approved
FROM client_approvals ca
LEFT JOIN clients c   ON c.airtable_id = ca.client_id
LEFT JOIN app_users u ON u.airtable_id = ca.approved_by_id;
