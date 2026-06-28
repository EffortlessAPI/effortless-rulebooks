-- Migration 006: v6 — App Users, Client Approvals, Inventory Adjustments
--
-- Adds three new tables that match the latest Airtable schema:
--   * app_users            — internal/external users (Admin / Manager / Customer)
--   * client_approvals     — per-client approval records, linking a client to
--                            the app user who approved them (or null = pending)
--   * inventory_adjustments — stock movements per product. Product.stock_on_hand
--                             is derived from the signed sum of these rows:
--                               Addition  → +quantity
--                               Removal   → −quantity
--                               Correction→ +quantity (audit-style adjustment)

-- ── 1. app_users ────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS app_users (
  id            SERIAL PRIMARY KEY,
  name          TEXT NOT NULL UNIQUE,     -- derived slug (contact-email)
  slug          TEXT NOT NULL UNIQUE,
  contact_name  TEXT NOT NULL DEFAULT '',
  email         TEXT NOT NULL DEFAULT '',
  phone         TEXT NOT NULL DEFAULT '',
  role          TEXT NOT NULL DEFAULT 'Customer'
                  CHECK (role IN ('Admin', 'Manager', 'Customer')),
  notes         TEXT NOT NULL DEFAULT '',
  created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ── 2. client_approvals ─────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS client_approvals (
  id                     SERIAL PRIMARY KEY,
  slug                   TEXT NOT NULL UNIQUE,
  client_id              INTEGER NOT NULL REFERENCES clients(id) ON DELETE CASCADE,
  approved_by_user_id    INTEGER REFERENCES app_users(id) ON DELETE SET NULL,
  notes                  TEXT NOT NULL DEFAULT '',
  created_at             TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_client_approvals_client ON client_approvals(client_id);
CREATE INDEX IF NOT EXISTS idx_client_approvals_user   ON client_approvals(approved_by_user_id);

-- ── 3. inventory_adjustments ────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS inventory_adjustments (
  id               SERIAL PRIMARY KEY,
  slug             TEXT NOT NULL UNIQUE,
  product_id       INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
  adjustment_date  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  adjustment_type  TEXT NOT NULL DEFAULT 'Addition'
                     CHECK (adjustment_type IN ('Addition', 'Removal', 'Correction')),
  quantity         INTEGER NOT NULL DEFAULT 0,
  reason           TEXT NOT NULL DEFAULT 'Other'
                     CHECK (reason IN (
                       'Purchase Order', 'Sales Order', 'Inventory Count',
                       'Damage', 'Return', 'Transfer', 'Other'
                     )),
  adjusted_by      TEXT NOT NULL DEFAULT '',
  notes            TEXT NOT NULL DEFAULT '',
  created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_inventory_adjustments_product ON inventory_adjustments(product_id);
CREATE INDEX IF NOT EXISTS idx_inventory_adjustments_date    ON inventory_adjustments(adjustment_date);

-- ── 4. Seed app_users ───────────────────────────────────────────────────────
INSERT INTO app_users (name, slug, contact_name, email, phone, role, notes)
SELECT * FROM (VALUES
  ('admin-ava-admin',   'admin-ava-admin',   'Ava Admin',     'ava@example.com',   '555-0100', 'Admin',    'Primary platform admin.'),
  ('manager-mia-mgr',   'manager-mia-mgr',   'Mia Manager',   'mia@example.com',   '555-0110', 'Manager',  'Account manager.'),
  ('customer-carl-cust','customer-carl-cust','Carl Customer', 'carl@example.com',  '555-0120', 'Customer', 'Customer portal user.')
) AS v(name, slug, contact_name, email, phone, role, notes)
WHERE NOT EXISTS (SELECT 1 FROM app_users LIMIT 1);

-- ── 5. Seed client_approvals (one per existing client — first gets approved) ─
DO $$
DECLARE
  admin_id   INTEGER;
  manager_id INTEGER;
  c_rec      RECORD;
  counter    INTEGER := 0;
BEGIN
  IF NOT EXISTS (SELECT 1 FROM client_approvals LIMIT 1) THEN
    SELECT id INTO admin_id   FROM app_users WHERE role = 'Admin'   LIMIT 1;
    SELECT id INTO manager_id FROM app_users WHERE role = 'Manager' LIMIT 1;

    FOR c_rec IN SELECT id, slug FROM clients ORDER BY id LOOP
      counter := counter + 1;
      INSERT INTO client_approvals (slug, client_id, approved_by_user_id, notes)
      VALUES (
        c_rec.slug || '-apvl-' || counter,
        c_rec.id,
        CASE
          WHEN counter % 2 = 0 THEN NULL          -- every other approval is pending
          WHEN counter <= 2    THEN admin_id
          ELSE manager_id
        END,
        CASE
          WHEN counter % 2 = 0 THEN 'Pending initial review.'
          ELSE 'Approved after onboarding call.'
        END
      );
    END LOOP;
  END IF;
END $$;

-- ── 6. Seed inventory_adjustments for each existing product ─────────────────
--    Gives every product a realistic opening-stock row, plus a small worked
--    example of a removal / correction on the first two products.
DO $$
DECLARE
  p_rec      RECORD;
  idx        INTEGER := 0;
  initial_qty INTEGER;
BEGIN
  IF NOT EXISTS (SELECT 1 FROM inventory_adjustments LIMIT 1) THEN
    FOR p_rec IN SELECT id, slug, COALESCE(stock_quantity, 0) AS qty FROM products ORDER BY id LOOP
      idx := idx + 1;
      initial_qty := GREATEST(p_rec.qty, 10 + (idx * 5));  -- at least 15, 20, 25, ...
      INSERT INTO inventory_adjustments
        (slug, product_id, adjustment_type, quantity, reason, adjusted_by, notes, adjustment_date)
      VALUES (
        p_rec.slug || '-adj-seed-1',
        p_rec.id, 'Addition', initial_qty, 'Purchase Order',
        'seed', 'Initial opening-stock purchase order.',
        NOW() - INTERVAL '30 days'
      );

      IF idx = 1 THEN
        INSERT INTO inventory_adjustments
          (slug, product_id, adjustment_type, quantity, reason, adjusted_by, notes, adjustment_date)
        VALUES (
          p_rec.slug || '-adj-seed-2',
          p_rec.id, 'Removal', 2, 'Damage',
          'seed', 'Two units damaged in warehouse.',
          NOW() - INTERVAL '10 days'
        );
      ELSIF idx = 2 THEN
        INSERT INTO inventory_adjustments
          (slug, product_id, adjustment_type, quantity, reason, adjusted_by, notes, adjustment_date)
        VALUES (
          p_rec.slug || '-adj-seed-2',
          p_rec.id, 'Correction', 1, 'Inventory Count',
          'seed', 'Cycle-count correction (+1).',
          NOW() - INTERVAL '3 days'
        );
      ELSIF idx = 3 THEN
        INSERT INTO inventory_adjustments
          (slug, product_id, adjustment_type, quantity, reason, adjusted_by, notes, adjustment_date)
        VALUES (
          p_rec.slug || '-adj-seed-2',
          p_rec.id, 'Removal', 3, 'Sales Order',
          'seed', 'Pulled for fulfilment.',
          NOW() - INTERVAL '5 days'
        );
      END IF;
    END LOOP;
  END IF;
END $$;
