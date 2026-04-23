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

-- Customers
ALTER TABLE customers DROP CONSTRAINT IF EXISTS fk_customers_customer;
ALTER TABLE customers ADD CONSTRAINT fk_customers_customer
  FOREIGN KEY (customer) REFERENCES customers (customer_id);

-- Projects
ALTER TABLE projects DROP CONSTRAINT IF EXISTS fk_projects_project_type;
ALTER TABLE projects ADD CONSTRAINT fk_projects_project_type
  FOREIGN KEY (project_type) REFERENCES types_of_project (types_of_project_id);
ALTER TABLE projects DROP CONSTRAINT IF EXISTS fk_projects_approved_by;
ALTER TABLE projects ADD CONSTRAINT fk_projects_approved_by
  FOREIGN KEY (approved_by) REFERENCES employees (employee_id);

-- Employees
ALTER TABLE employees DROP CONSTRAINT IF EXISTS fk_employees_role;
ALTER TABLE employees ADD CONSTRAINT fk_employees_role
  FOREIGN KEY (role) REFERENCES roles (role_id);
ALTER TABLE employees DROP CONSTRAINT IF EXISTS fk_employees_projects;
ALTER TABLE employees ADD CONSTRAINT fk_employees_projects
  FOREIGN KEY (projects) REFERENCES projects (project_id);

-- Roles
ALTER TABLE roles DROP CONSTRAINT IF EXISTS fk_roles_employees;
ALTER TABLE roles ADD CONSTRAINT fk_roles_employees
  FOREIGN KEY (employees) REFERENCES employees (employee_id);

-- TypesOfProject
ALTER TABLE types_of_project DROP CONSTRAINT IF EXISTS fk_types_of_project_projects;
ALTER TABLE types_of_project ADD CONSTRAINT fk_types_of_project_projects
  FOREIGN KEY (projects) REFERENCES projects (project_id);

-- 7 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
