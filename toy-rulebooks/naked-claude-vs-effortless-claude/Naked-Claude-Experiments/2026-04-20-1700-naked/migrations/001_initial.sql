-- Create customers table (v1 schema — color-based)
CREATE TABLE IF NOT EXISTS customers (
  id         SERIAL PRIMARY KEY,
  name       TEXT    NOT NULL,
  slug       TEXT    NOT NULL UNIQUE,
  notes      TEXT    NOT NULL DEFAULT '',
  color      TEXT    NOT NULL DEFAULT '',
  is_stopped BOOLEAN GENERATED ALWAYS AS (color = 'Green') STORED,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Seed v1 data only if customers table is empty AND still has the color column
DO $$
BEGIN
  IF EXISTS (
    SELECT 1 FROM information_schema.columns
    WHERE table_name = 'customers' AND column_name = 'color'
  ) AND NOT EXISTS (SELECT 1 FROM customers LIMIT 1) THEN
    INSERT INTO customers (name, slug, notes, color) VALUES
      ('Bob',          'bob',           'Currently in launch.', 'Green'),
      ('Alice Johnson','alice-johnson', 'An early entry.',       'Green'),
      ('Brian Lee',    'brian-lee',     'An early entry.',       'Blue'),
      ('Carla Smith',  'carla-smith',   'An early entry.',       'Yellow'),
      ('Caroline',     'caroline',      '',                      'Red');
  END IF;
END $$;
