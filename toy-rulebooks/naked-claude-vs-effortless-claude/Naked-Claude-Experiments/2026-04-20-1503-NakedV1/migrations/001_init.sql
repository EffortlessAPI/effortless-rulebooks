-- Create customers table for the Customer Tracker app
CREATE TABLE IF NOT EXISTS customers (
  id        SERIAL PRIMARY KEY,
  name      TEXT NOT NULL,
  slug      TEXT NOT NULL UNIQUE,
  notes     TEXT,
  color     TEXT NOT NULL DEFAULT '',
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- stopped is derived: true when color = 'Green'
-- We store it as a generated column for query convenience
-- (Postgres 12+ supports generated columns)
ALTER TABLE customers
  ADD COLUMN IF NOT EXISTS stopped BOOLEAN GENERATED ALWAYS AS (color = 'Green') STORED;

-- Seed data
INSERT INTO customers (name, slug, notes, color) VALUES
  ('Bob',           'bob',           'Currently in launch.',   'Green'),
  ('Alice Johnson', 'alice-johnson', 'An early entry.',        'Green'),
  ('Brian Lee',     'brian-lee',     'An early entry.',        'Blue'),
  ('Carla Smith',   'carla-smith',   'An early entry.',        'Yellow'),
  ('Caroline',      'caroline',      '',                       'Red')
ON CONFLICT (slug) DO NOTHING;
