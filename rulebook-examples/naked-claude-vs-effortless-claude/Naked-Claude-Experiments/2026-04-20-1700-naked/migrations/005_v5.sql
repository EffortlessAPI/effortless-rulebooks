-- Migration 005: v5 — ClientCategories with per-category discount %
--
-- New table ClientCategories (Name, Notes, Discount) linked 1..N to Clients.
-- InvoiceLineItems.DiscountPercent is now driven by the parent invoice's
-- client's category discount — not by a free-form per-line-item value.

-- 1. client_categories table
CREATE TABLE IF NOT EXISTS client_categories (
  id               SERIAL PRIMARY KEY,
  name             TEXT NOT NULL UNIQUE,
  slug             TEXT NOT NULL UNIQUE,
  notes            TEXT NOT NULL DEFAULT '',
  discount_percent NUMERIC(8,6) NOT NULL DEFAULT 0,
  created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 2. category_id on clients (nullable FK)
ALTER TABLE clients
  ADD COLUMN IF NOT EXISTS category_id INTEGER REFERENCES client_categories(id);

-- 3. Seed 3 categories (idempotent)
INSERT INTO client_categories (name, slug, discount_percent, notes)
SELECT * FROM (VALUES
  ('Prospect', 'prospect', 0.000000,  'New lead who has not yet placed a paid order.'),
  ('Active',   'active',   0.050000,  'Active client — 5% standard discount.'),
  ('Gold',     'gold',     0.150000,  'Gold-tier client — 15% discount.')
) AS v(name, slug, discount_percent, notes)
WHERE NOT EXISTS (SELECT 1 FROM client_categories LIMIT 1);

-- 4. Assign seed clients to categories (only first time — don't clobber manual edits)
DO $$
DECLARE
  prospect_id INTEGER;
  active_id   INTEGER;
  gold_id     INTEGER;
BEGIN
  SELECT id INTO prospect_id FROM client_categories WHERE slug = 'prospect';
  SELECT id INTO active_id   FROM client_categories WHERE slug = 'active';
  SELECT id INTO gold_id     FROM client_categories WHERE slug = 'gold';

  UPDATE clients SET category_id = active_id
    WHERE slug IN ('bob', 'alice-johnson', 'carla-smith') AND category_id IS NULL;
  UPDATE clients SET category_id = prospect_id
    WHERE slug = 'brian-lee' AND category_id IS NULL;
  UPDATE clients SET category_id = gold_id
    WHERE slug = 'caroline' AND category_id IS NULL;
END $$;

-- 5. Align existing invoice line items with the new "discount = category discount" rule.
--    Only run this once so re-migrations don't clobber edits.
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                 WHERE table_name = 'client_categories' AND column_name = 'migrated_line_items') THEN
    -- Add a one-shot flag column, then drop it once we've backfilled.
    ALTER TABLE client_categories ADD COLUMN migrated_line_items BOOLEAN NOT NULL DEFAULT FALSE;

    UPDATE invoice_line_items ili
    SET discount_percent = COALESCE(cc.discount_percent, 0)
    FROM invoices inv
    JOIN clients cl ON cl.id = inv.client_id
    LEFT JOIN client_categories cc ON cc.id = cl.category_id
    WHERE ili.invoice_id = inv.id;

    ALTER TABLE client_categories DROP COLUMN migrated_line_items;
  END IF;
END $$;
