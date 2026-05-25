-- ============================================================================
-- CUSTOMIZE POLICIES - RLS for app_users / customers / statuses
-- ============================================================================
-- Identity comes from auth.begin_session(<jwt>), which sets GUCs read by:
--   auth.email()    -> verified end-user email
--   auth.is_admin() -> true iff vw_app_users.role = 'admin'
-- Policies fail closed when no session has been started.
-- Idempotent: drop-and-recreate so repeated init-db runs work.
-- ============================================================================

-- ---------------- app_users ----------------
DROP POLICY IF EXISTS app_users_self_select ON app_users;
DROP POLICY IF EXISTS app_users_admin_all   ON app_users;

CREATE POLICY app_users_self_select ON app_users
  FOR SELECT
  USING (lower(email_address) = auth.email());

CREATE POLICY app_users_admin_all ON app_users
  FOR ALL
  USING (auth.is_admin())
  WITH CHECK (auth.is_admin());

-- ---------------- customers ----------------
DROP POLICY IF EXISTS customers_owner_select ON customers;
DROP POLICY IF EXISTS customers_owner_modify ON customers;
DROP POLICY IF EXISTS customers_admin_all    ON customers;

CREATE POLICY customers_owner_select ON customers
  FOR SELECT
  USING (lower(owner_email) = auth.email());

CREATE POLICY customers_owner_modify ON customers
  FOR UPDATE
  USING (lower(owner_email) = auth.email())
  WITH CHECK (lower(owner_email) = auth.email());

CREATE POLICY customers_admin_all ON customers
  FOR ALL
  USING (auth.is_admin())
  WITH CHECK (auth.is_admin());

-- ---------------- statuses ----------------
DROP POLICY IF EXISTS statuses_authed_select ON statuses;
DROP POLICY IF EXISTS statuses_admin_all     ON statuses;

CREATE POLICY statuses_authed_select ON statuses
  FOR SELECT
  USING (auth.email() IS NOT NULL);

CREATE POLICY statuses_admin_all ON statuses
  FOR ALL
  USING (auth.is_admin())
  WITH CHECK (auth.is_admin());
