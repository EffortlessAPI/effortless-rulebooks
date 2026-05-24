-- ============================================================================
-- CUSTOMIZE FUNCTIONS - auth schema: JWT session + RLS helpers
-- ============================================================================
-- All identity logic lives in the `auth` schema, owned by postgres.
-- app_user can EXECUTE these functions (they're SECURITY DEFINER) but
-- cannot redefine them — the schema is locked down in 01b.
--
-- Entry point:
--   SELECT auth.begin_session('<jwt>');   -- returns the role text
--
-- Then RLS policies use:
--   auth.email()      -> verified email (lower-case) or NULL
--   auth.role()       -> role from public.vw_app_users (or 'anon')
--   auth.is_admin()   -> true iff role = 'admin'
--   auth.claims()     -> full claims jsonb
--
-- Signature verification: the JWT signature is *not* verified here. The
-- node/web app verifies RS256 against auth.trusted_tenants.public_key_pem
-- before calling begin_session. For direct psql poking (as a superuser) the
-- decode-only path is acceptable.
-- ============================================================================

-- ----- decode_jwt: parse the payload (base64url) into jsonb ----------------
CREATE OR REPLACE FUNCTION auth.decode_jwt(jwt_text text) RETURNS jsonb
LANGUAGE plpgsql IMMUTABLE AS $$
DECLARE
  payload text := split_part(jwt_text, '.', 2);
  padded  text;
BEGIN
  IF payload = '' THEN RAISE EXCEPTION 'malformed jwt'; END IF;
  payload := translate(payload, '-_', '+/');
  padded  := rpad(payload, ((length(payload) + 3) / 4) * 4, '=');
  RETURN convert_from(decode(padded, 'base64'), 'utf8')::jsonb;
END $$;

-- ----- begin_session: decode + validate + stamp GUCs ----------------------
CREATE OR REPLACE FUNCTION auth.begin_session(jwt_text text) RETURNS text
LANGUAGE plpgsql
SECURITY DEFINER
SET row_security = off
AS $$
DECLARE
  c       jsonb;
  v_email text;
  v_tid   text;
  v_exp   bigint;
  v_role  text;
BEGIN
  c       := auth.decode_jwt(jwt_text);
  v_email := lower(c->>'email');
  v_tid   := c->>'tenant_id';
  v_exp   := (c->>'exp')::bigint;

  IF v_email IS NULL OR v_email = '' THEN
    RAISE EXCEPTION 'jwt missing email claim';
  END IF;
  IF v_exp IS NULL OR v_exp < extract(epoch from now())::bigint THEN
    RAISE EXCEPTION 'jwt expired or missing exp';
  END IF;
  IF NOT EXISTS (
    SELECT 1 FROM auth.trusted_tenants
     WHERE tenant_id = v_tid AND is_active
  ) THEN
    RAISE EXCEPTION 'untrusted tenant: %', v_tid;
  END IF;

  SELECT au.role INTO v_role
    FROM public.vw_app_users au
   WHERE lower(au.email_address) = v_email
   LIMIT 1;
  v_role := COALESCE(v_role, 'anon');

  -- Transaction-local GUCs (cleared at COMMIT/ROLLBACK).
  PERFORM set_config('auth.email',  v_email,  true);
  PERFORM set_config('auth.role',   v_role,   true);
  PERFORM set_config('auth.claims', c::text,  true);

  RETURN v_role;
END $$;

-- ----- accessors used by RLS policies -------------------------------------
CREATE OR REPLACE FUNCTION auth.email() RETURNS text
LANGUAGE sql STABLE AS $$
  SELECT NULLIF(current_setting('auth.email', true), '')
$$;

CREATE OR REPLACE FUNCTION auth.role() RETURNS text
LANGUAGE sql STABLE AS $$
  SELECT COALESCE(NULLIF(current_setting('auth.role', true), ''), 'anon')
$$;

CREATE OR REPLACE FUNCTION auth.is_admin() RETURNS boolean
LANGUAGE sql STABLE AS $$
  SELECT auth.role() = 'admin'
$$;

CREATE OR REPLACE FUNCTION auth.claims() RETURNS jsonb
LANGUAGE sql STABLE AS $$
  SELECT NULLIF(current_setting('auth.claims', true), '')::jsonb
$$;

-- ----- grants -------------------------------------------------------------
GRANT EXECUTE ON FUNCTION
  auth.decode_jwt(text),
  auth.begin_session(text),
  auth.email(),
  auth.role(),
  auth.is_admin(),
  auth.claims()
TO app_user, app_admin, app_anon, app_public;

-- Drop the old app.jwt_* helpers if they still exist from the prior version.
DROP FUNCTION IF EXISTS app.jwt_claims()    CASCADE;
DROP FUNCTION IF EXISTS app.jwt_email()     CASCADE;
DROP FUNCTION IF EXISTS app.jwt_role()      CASCADE;
DROP FUNCTION IF EXISTS app.jwt_is_admin()  CASCADE;
