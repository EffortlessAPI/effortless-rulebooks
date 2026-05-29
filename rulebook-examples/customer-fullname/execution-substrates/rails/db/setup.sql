-- Create Customers table
CREATE TABLE IF NOT EXISTS customers (
  customer_id VARCHAR PRIMARY KEY,
  email_address VARCHAR,
  first_name VARCHAR,
  last_name VARCHAR
);

-- Create view with calculated fields
CREATE OR REPLACE VIEW vw_customers AS
SELECT
  "customer_id",
  "last_name" || ', ' || "first_name" AS "name",
  "email_address",
  SUBSTRING("first_name" FROM 1 FOR 1) || SUBSTRING("last_name" FROM 1 FOR 1) || '.' AS "initials",
  "first_name",
  "last_name"
FROM customers;

-- Insert seed data
INSERT INTO customers (customer_id, email_address, first_name, last_name) 
VALUES 
  ('jane-smith-email-com', 'jane.smith@email.com', 'Jane', 'Smithy'),
  ('john-doe-email-com', 'john.doe@email.com', 'John', 'Doe'),
  ('emily-jones-email-com', 'emily.jones@email.com', 'Emily', 'Jones'),
  ('alice-cooper', 'alice@cooper.com', 'Alice', 'Gutknecht')
ON CONFLICT DO NOTHING;
