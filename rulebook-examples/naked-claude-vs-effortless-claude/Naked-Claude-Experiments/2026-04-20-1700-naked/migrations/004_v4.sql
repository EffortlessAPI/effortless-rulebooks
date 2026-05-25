-- Migration 004: v4 rename
-- customers → clients, orders → invoices, order_line_items → invoice_line_items

-- 1. Rename tables (idempotent)
DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.tables
             WHERE table_schema = 'public' AND table_name = 'customers') THEN
    ALTER TABLE customers RENAME TO clients;
  END IF;
END $$;

DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.tables
             WHERE table_schema = 'public' AND table_name = 'orders') THEN
    ALTER TABLE orders RENAME TO invoices;
  END IF;
END $$;

DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.tables
             WHERE table_schema = 'public' AND table_name = 'order_line_items') THEN
    ALTER TABLE order_line_items RENAME TO invoice_line_items;
  END IF;
END $$;

-- 2. Rename columns in invoices (was orders)
DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.columns
             WHERE table_name = 'invoices' AND column_name = 'customer_id') THEN
    ALTER TABLE invoices RENAME COLUMN customer_id TO client_id;
  END IF;
END $$;

DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.columns
             WHERE table_name = 'invoices' AND column_name = 'order_number') THEN
    ALTER TABLE invoices RENAME COLUMN order_number TO invoice_number;
  END IF;
END $$;

-- 3. Rename FK column in invoice_line_items (was order_line_items.order_id)
DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.columns
             WHERE table_name = 'invoice_line_items' AND column_name = 'order_id') THEN
    ALTER TABLE invoice_line_items RENAME COLUMN order_id TO invoice_id;
  END IF;
END $$;

-- 4. Rename FK column in payments (was payments.order_id)
DO $$ BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.columns
             WHERE table_name = 'payments' AND column_name = 'order_id') THEN
    ALTER TABLE payments RENAME COLUMN order_id TO invoice_id;
  END IF;
END $$;
