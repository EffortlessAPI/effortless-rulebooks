-- v1 customers schema
DROP VIEW IF EXISTS vw_customers;
DROP TABLE IF EXISTS customers CASCADE;

CREATE TABLE customers (
  id SERIAL PRIMARY KEY,
  handle TEXT NOT NULL UNIQUE,
  name TEXT NOT NULL,
  notes TEXT NOT NULL DEFAULT '',
  color TEXT NOT NULL DEFAULT '',
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Derived "stopped" view: stopped iff color is exactly 'Green'
CREATE OR REPLACE VIEW vw_customers AS
SELECT
  id,
  handle,
  name,
  notes,
  color,
  (color = 'Green') AS is_stopped,
  created_at
FROM customers;

-- Seed data
INSERT INTO customers (handle, name, notes, color) VALUES
  ('bob',           'Bob',           'currently in launch',  'Green'),
  ('alice-johnson', 'Alice Johnson', 'an early entry',       'Green'),
  ('brian-lee',     'Brian Lee',     'an early entry',       'Blue'),
  ('carla-smith',   'Carla Smith',   'an early entry',       'Yellow'),
  ('caroline',      'Caroline',      '',                     'Red');
