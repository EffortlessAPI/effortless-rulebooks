-- ============================================================================
-- CUSTOMIZE SCHEMA - magic-links auth + RLS scaffolding
-- ============================================================================
-- auth.trusted_tenants: registry of magic-links tenants whose JWTs we honor.
-- app schema:           home for app.jwt_*() helpers (see 02b).
-- Role grants:          app_user (customer) + app_admin can read views;
--                       app_admin can write base tables.
-- Roles app_user/app_admin/app_anon/app_public are created by the base
-- bootstrap; we only grant here.
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS app;

-- Lock down the auth schema: app roles can USE it (call functions) but
-- cannot CREATE or modify objects in it. The rules layer is owned by postgres.
REVOKE ALL ON SCHEMA auth FROM PUBLIC;
REVOKE CREATE ON SCHEMA auth FROM app_user, app_admin, app_anon, app_public;

CREATE TABLE IF NOT EXISTS auth.trusted_tenants (
  tenant_id      text PRIMARY KEY,
  public_key_pem text NOT NULL,
  is_active      boolean NOT NULL DEFAULT true,
  added_at       timestamptz NOT NULL DEFAULT now()
);

GRANT USAGE  ON SCHEMA auth TO app_user, app_admin;
GRANT SELECT ON auth.trusted_tenants TO app_user, app_admin;

GRANT USAGE ON SCHEMA app TO app_user, app_admin, app_anon, app_public;

-- Views use security_invoker = ON, so RLS on base tables applies through them.
GRANT SELECT ON ALL TABLES IN SCHEMA public TO app_user, app_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT SELECT ON TABLES TO app_user, app_admin;

-- Admins can mutate base tables.
GRANT INSERT, UPDATE, DELETE ON customers, statuses, app_users TO app_admin;

GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO app_user, app_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT USAGE, SELECT ON SEQUENCES TO app_user, app_admin;
