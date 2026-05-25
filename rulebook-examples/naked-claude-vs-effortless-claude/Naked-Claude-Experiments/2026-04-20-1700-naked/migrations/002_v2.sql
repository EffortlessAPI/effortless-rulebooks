-- Migration 002: Add statuses table and update customers for v2

-- 1. Create statuses table
CREATE TABLE IF NOT EXISTS statuses (
  id          SERIAL PRIMARY KEY,
  name        TEXT    NOT NULL,
  slug        TEXT    NOT NULL UNIQUE,
  description TEXT    NOT NULL DEFAULT '',
  is_blocking BOOLEAN NOT NULL DEFAULT FALSE,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 2. Seed statuses if table is empty
INSERT INTO statuses (name, slug, description, is_blocking)
SELECT * FROM (VALUES
  ('New',        'new',        'A newly created customer or request that has not yet been processed.',               false),
  ('Processing', 'processing', 'The customer or request is currently being processed.',                             false),
  ('Delayed',    'delayed',    'The customer or request has been delayed and is not progressing as scheduled.',      false),
  ('Cancelled',  'cancelled',  'The customer or request has been cancelled and will not proceed further.',          false),
  ('In-Review',  'in-review',  'The customer or request is currently under review.',                               true),
  ('Pending',    'pending',    'The customer or request is pending further action or review.',                      true),
  ('On-Hold',    'on-hold',    'The customer or request is on hold and temporarily paused.',                       true)
) AS v(name, slug, description, is_blocking)
WHERE NOT EXISTS (SELECT 1 FROM statuses LIMIT 1);

-- 3. Add status_id to customers (safe to run multiple times)
ALTER TABLE customers ADD COLUMN IF NOT EXISTS status_id INTEGER REFERENCES statuses(id);

-- 4. Drop the v1-specific generated column (is_stopped) and color column if they still exist
DO $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_name = 'customers' AND column_name = 'is_stopped'
  ) THEN
    ALTER TABLE customers DROP COLUMN is_stopped;
  END IF;

  IF EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_name = 'customers' AND column_name = 'color'
  ) THEN
    ALTER TABLE customers DROP COLUMN color;
  END IF;
END $$;

-- 5. Re-seed customers for v2 if they all have null status_id (i.e. migrated from v1)
--    or if customers table is empty
DO $$
DECLARE
  total_cust  INT;
  null_status INT;
BEGIN
  SELECT COUNT(*) INTO total_cust  FROM customers;
  SELECT COUNT(*) INTO null_status FROM customers WHERE status_id IS NULL;

  IF total_cust = 0 OR (total_cust > 0 AND total_cust = null_status) THEN
    -- Clear old v1 data and insert v2 seed
    DELETE FROM customers;

    INSERT INTO customers (name, slug, notes, status_id) VALUES
      ('Bob',          'bob',           'A customer currently in launch.',  (SELECT id FROM statuses WHERE slug = 'on-hold')),
      ('Alice Johnson','alice-johnson', 'Initial mock entry for Alice.',     (SELECT id FROM statuses WHERE slug = 'pending')),
      ('Brian Lee',    'brian-lee',     'Initial mock entry for Brian.',     NULL),
      ('Carla Smith',  'carla-smith',   'Initial mock entry for Carla.',     (SELECT id FROM statuses WHERE slug = 'delayed')),
      ('Caroline',     'caroline',      '',                                  (SELECT id FROM statuses WHERE slug = 'processing'));
  END IF;
END $$;
