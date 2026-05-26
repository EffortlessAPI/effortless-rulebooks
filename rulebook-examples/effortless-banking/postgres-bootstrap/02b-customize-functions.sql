-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined functions customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main functions script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom functions changes will appear here:

-- Override: generated calc_covenants_has_active_waiver emits
-- COALESCE(<timestamptz>, '') which Postgres rejects (cannot cast '' to
-- timestamptz). Replace with a direct NULL check.
CREATE OR REPLACE FUNCTION calc_covenants_has_active_waiver(p_covenants_id TEXT)
RETURNS BOOLEAN AS $$
  SELECT ((SELECT current_waiver_through FROM covenants WHERE covenants_id = p_covenants_id) IS NOT NULL)::boolean;
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- Override: generated cross-table lookup functions join on `<other>_id = <fk>`
-- but FK columns store the kebab Name, not the UUID. Rewrite each to match on
-- the calculated Name field, which is what FK columns actually contain.
-- ----------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION calc_businesses_relationship_manager_label(p_businesses_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT relationship_manager FROM businesses WHERE businesses_id = p_businesses_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_beneficial_owners_business_label(p_beneficial_owners_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM beneficial_owners WHERE beneficial_owners_id = p_beneficial_owners_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_contacts_business_label(p_contacts_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM contacts WHERE contacts_id = p_contacts_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_accounts_business_label(p_accounts_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM accounts WHERE accounts_id = p_accounts_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_loans_business_label(p_loans_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM loans WHERE loans_id = p_loans_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_loans_business_naics_code(p_loans_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT naics_code::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM loans WHERE loans_id = p_loans_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_loans_originating_rm_label(p_loans_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT originating_rm FROM loans WHERE loans_id = p_loans_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_loans_underwriter_label(p_loans_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT underwriter FROM loans WHERE loans_id = p_loans_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_covenants_loan_label(p_covenants_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT loan_number::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM covenants WHERE covenants_id = p_covenants_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_covenants_loan_business(p_covenants_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT business::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM covenants WHERE covenants_id = p_covenants_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_risk_rating_history_loan_label(p_risk_rating_history_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT loan_number::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_risk_rating_history_changed_by_user_label(p_risk_rating_history_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT changed_by_user FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_documents_business_label(p_documents_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM documents WHERE documents_id = p_documents_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_documents_loan_label(p_documents_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT loan_number::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM documents WHERE documents_id = p_documents_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_documents_uploaded_by_user_label(p_documents_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT uploaded_by_user FROM documents WHERE documents_id = p_documents_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_interactions_business_label(p_interactions_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM interactions WHERE interactions_id = p_interactions_id));
$$ LANGUAGE sql STABLE;

CREATE OR REPLACE FUNCTION calc_interactions_user_label(p_interactions_id TEXT)
RETURNS TEXT AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT "user" FROM interactions WHERE interactions_id = p_interactions_id));
$$ LANGUAGE sql STABLE;

