-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- ExpenseReports
ALTER TABLE expense_reports DROP CONSTRAINT IF EXISTS fk_expense_reports_employee;
ALTER TABLE expense_reports ADD CONSTRAINT fk_expense_reports_employee
  FOREIGN KEY (employee) REFERENCES employees (employee_id);

-- ExpenseItems
ALTER TABLE expense_items DROP CONSTRAINT IF EXISTS fk_expense_items_expense_report;
ALTER TABLE expense_items ADD CONSTRAINT fk_expense_items_expense_report
  FOREIGN KEY (expense_report) REFERENCES expense_reports (expense_report_id);

-- 2 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
