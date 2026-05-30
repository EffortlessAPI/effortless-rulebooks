--
-- PostgreSQL database dump
--

\restrict MgRNtj3b5u2lRGgHsfB4CK1pG1HXAknMMSZDFr5trWALjW3WVQ5JmryPdtA4fxF

-- Dumped from database version 18.1 (Postgres.app)
-- Dumped by pg_dump version 18.1 (Homebrew)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: calc_accounts_business_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_accounts_business_label(p_accounts_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM accounts WHERE accounts_id = p_accounts_id));
$$;


--
-- Name: calc_accounts_has_any_treasury_service(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_accounts_has_any_treasury_service(p_accounts_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_accounts_treasury_service_count(p_accounts_id))::NUMERIC > 0)::boolean;
$$;


--
-- Name: calc_accounts_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_accounts_name(p_accounts_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (CONCAT((SELECT NULLIF(business, '') FROM accounts WHERE accounts_id = p_accounts_id), '-', LOWER((SELECT NULLIF(account_type, '') FROM accounts WHERE accounts_id = p_accounts_id)), '-', (SELECT NULLIF(account_number_last4, '') FROM accounts WHERE accounts_id = p_accounts_id)))::text;
$$;


--
-- Name: calc_accounts_treasury_service_count(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_accounts_treasury_service_count(p_accounts_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $_$
  SELECT ((COALESCE(CASE WHEN (CASE WHEN COALESCE((SELECT has_ach FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN COALESCE((SELECT has_ach FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN ((COALESCE(CASE WHEN (CASE WHEN COALESCE((SELECT has_wire FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN COALESCE((SELECT has_wire FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN (CASE WHEN COALESCE((SELECT has_card FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN COALESCE((SELECT has_card FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0)))::text ~ '^-?[0-9]*\.?[0-9]+$' THEN ((COALESCE(CASE WHEN (CASE WHEN COALESCE((SELECT has_wire FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN COALESCE((SELECT has_wire FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN (CASE WHEN COALESCE((SELECT has_card FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN COALESCE((SELECT has_card FROM accounts WHERE accounts_id = p_accounts_id), FALSE) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0)))::numeric ELSE NULL END, 0)))::integer;
$_$;


--
-- Name: calc_beneficial_owners_business_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_beneficial_owners_business_label(p_beneficial_owners_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM beneficial_owners WHERE beneficial_owners_id = p_beneficial_owners_id));
$$;


--
-- Name: calc_beneficial_owners_meets25_percent_threshold(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_beneficial_owners_meets25_percent_threshold(p_beneficial_owners_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT (((SELECT ownership_percentage FROM beneficial_owners WHERE beneficial_owners_id = p_beneficial_owners_id))::NUMERIC >= 25)::boolean;
$$;


--
-- Name: calc_beneficial_owners_meets_cdd_threshold(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_beneficial_owners_meets_cdd_threshold(p_beneficial_owners_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT (((calc_beneficial_owners_meets25_percent_threshold(p_beneficial_owners_id) = 'true') OR COALESCE((SELECT is_control_person FROM beneficial_owners WHERE beneficial_owners_id = p_beneficial_owners_id), FALSE)))::boolean;
$$;


--
-- Name: calc_beneficial_owners_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_beneficial_owners_name(p_beneficial_owners_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (CONCAT((SELECT NULLIF(business, '') FROM beneficial_owners WHERE beneficial_owners_id = p_beneficial_owners_id), '-', REPLACE(LOWER((SELECT NULLIF(full_name, '') FROM beneficial_owners WHERE beneficial_owners_id = p_beneficial_owners_id)), ' ', '-')))::text;
$$;


--
-- Name: calc_businesses_beneficial_owners_at_cdd_threshold(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_beneficial_owners_at_cdd_threshold(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM beneficial_owners WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_accounts(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_accounts(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM accounts WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_beneficial_owners(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_beneficial_owners(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM beneficial_owners WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_classified_loans(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_classified_loans(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM loans WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_contacts(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_contacts(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM contacts WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_documents(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_documents(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM documents WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_interactions(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_interactions(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM interactions WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_count_of_loans(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_count_of_loans(p_businesses_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM loans WHERE business = calc_businesses_name(p_businesses_id)))::integer;
$$;


--
-- Name: calc_businesses_has_classified_loan(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_has_classified_loan(p_businesses_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_businesses_count_of_classified_loans(p_businesses_id))::NUMERIC > 0)::boolean;
$$;


--
-- Name: calc_businesses_is_customer(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_is_customer(p_businesses_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(status, '') FROM businesses WHERE businesses_id = p_businesses_id) = 'Customer')::boolean;
$$;


--
-- Name: calc_businesses_is_prospect(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_is_prospect(p_businesses_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(status, '') FROM businesses WHERE businesses_id = p_businesses_id) = 'Prospect')::boolean;
$$;


--
-- Name: calc_businesses_meets_cdd_rule(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_meets_cdd_rule(p_businesses_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_businesses_beneficial_owners_at_cdd_threshold(p_businesses_id))::NUMERIC > 0)::boolean;
$$;


--
-- Name: calc_businesses_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_name(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (REPLACE(REPLACE(LOWER((SELECT NULLIF(legal_name, '') FROM businesses WHERE businesses_id = p_businesses_id)), ' ', '-'), '.', ''))::text;
$$;


--
-- Name: calc_businesses_portfolio_priority(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_portfolio_priority(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (CASE WHEN calc_businesses_has_classified_loan(p_businesses_id) THEN ('High')::text ELSE (CASE WHEN (NOT (calc_businesses_meets_cdd_rule(p_businesses_id)) OR calc_businesses_is_prospect(p_businesses_id) OR (calc_businesses_count_of_loans(p_businesses_id))::NUMERIC = 0) THEN ('Medium')::text ELSE ('Low')::text END)::text END)::text;
$$;


--
-- Name: calc_businesses_relationship_manager_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_relationship_manager_label(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT relationship_manager FROM businesses WHERE businesses_id = p_businesses_id));
$$;


--
-- Name: calc_businesses_total_deposit_balance_usd(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_total_deposit_balance_usd(p_businesses_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COALESCE(SUM((current_balance_usd)::numeric), 0) FROM accounts WHERE business = calc_businesses_name(p_businesses_id)))::numeric;
$$;


--
-- Name: calc_businesses_total_loan_principal_usd(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_total_loan_principal_usd(p_businesses_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COALESCE(SUM((principal_usd)::numeric), 0) FROM loans WHERE business = calc_businesses_name(p_businesses_id)))::numeric;
$$;


--
-- Name: calc_businesses_was_referred(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_businesses_was_referred(p_businesses_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT (NOT (COALESCE((SELECT NULLIF(referral_source, '') FROM businesses WHERE businesses_id = p_businesses_id), '') IS NULL))::boolean;
$$;


--
-- Name: calc_contacts_business_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_contacts_business_label(p_contacts_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM contacts WHERE contacts_id = p_contacts_id));
$$;


--
-- Name: calc_contacts_is_ap_clerk(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_contacts_is_ap_clerk(p_contacts_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(contact_type, '') FROM contacts WHERE contacts_id = p_contacts_id) = 'APClerk')::boolean;
$$;


--
-- Name: calc_contacts_is_officer(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_contacts_is_officer(p_contacts_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(contact_type, '') FROM contacts WHERE contacts_id = p_contacts_id) = 'Officer')::boolean;
$$;


--
-- Name: calc_contacts_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_contacts_name(p_contacts_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (CONCAT((SELECT NULLIF(business, '') FROM contacts WHERE contacts_id = p_contacts_id), '-', REPLACE(LOWER((SELECT NULLIF(full_name, '') FROM contacts WHERE contacts_id = p_contacts_id)), ' ', '-'), '-', REPLACE(LOWER((SELECT NULLIF(title, '') FROM contacts WHERE contacts_id = p_contacts_id)), ' ', '-')))::text;
$$;


--
-- Name: calc_covenants_has_active_waiver(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_covenants_has_active_waiver(p_covenants_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT current_waiver_through FROM covenants WHERE covenants_id = p_covenants_id) IS NOT NULL)::boolean;
$$;


--
-- Name: calc_covenants_is_breached(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_covenants_is_breached(p_covenants_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(status, '') FROM covenants WHERE covenants_id = p_covenants_id) = 'Breached')::boolean;
$$;


--
-- Name: calc_covenants_loan_business(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_covenants_loan_business(p_covenants_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT business::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM covenants WHERE covenants_id = p_covenants_id));
$$;


--
-- Name: calc_covenants_loan_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_covenants_loan_label(p_covenants_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT loan_number::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM covenants WHERE covenants_id = p_covenants_id));
$$;


--
-- Name: calc_covenants_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_covenants_name(p_covenants_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (CONCAT((SELECT NULLIF(loan, '') FROM covenants WHERE covenants_id = p_covenants_id), '-', REPLACE(LOWER((SELECT NULLIF(covenant_type, '') FROM covenants WHERE covenants_id = p_covenants_id)), ' ', '-')))::text;
$$;


--
-- Name: calc_documents_attached_to(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_documents_attached_to(p_documents_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (CASE WHEN NOT (COALESCE((SELECT NULLIF(loan, '') FROM documents WHERE documents_id = p_documents_id), '') IS NULL) THEN ('Loan')::text ELSE (CASE WHEN NOT (COALESCE((SELECT NULLIF(business, '') FROM documents WHERE documents_id = p_documents_id), '') IS NULL) THEN ('Business')::text ELSE ('Orphan')::text END)::text END)::text;
$$;


--
-- Name: calc_documents_business_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_documents_business_label(p_documents_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM documents WHERE documents_id = p_documents_id));
$$;


--
-- Name: calc_documents_from_customer_portal(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_documents_from_customer_portal(p_documents_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(uploaded_via, '') FROM documents WHERE documents_id = p_documents_id) = 'BusinessClientPortal')::boolean;
$$;


--
-- Name: calc_documents_loan_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_documents_loan_label(p_documents_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT loan_number::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM documents WHERE documents_id = p_documents_id));
$$;


--
-- Name: calc_documents_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_documents_name(p_documents_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (REPLACE(REPLACE(LOWER((SELECT NULLIF(filename, '') FROM documents WHERE documents_id = p_documents_id)), ' ', '-'), '.', '-'))::text;
$$;


--
-- Name: calc_documents_uploaded_by_user_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_documents_uploaded_by_user_label(p_documents_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT uploaded_by_user FROM documents WHERE documents_id = p_documents_id));
$$;


--
-- Name: calc_interactions_business_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_business_label(p_interactions_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM interactions WHERE interactions_id = p_interactions_id));
$$;


--
-- Name: calc_interactions_from_customer(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_from_customer(p_interactions_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(source, '') FROM interactions WHERE interactions_id = p_interactions_id) = 'BusinessClientPortal')::boolean;
$$;


--
-- Name: calc_interactions_is_covenant_event(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_is_covenant_event(p_interactions_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_interactions_is_system_event(p_interactions_id) AND NOT ((COALESCE(POSITION('covenant' IN LOWER((SELECT NULLIF(subject, '') FROM interactions WHERE interactions_id = p_interactions_id))), 0))::NUMERIC = 0)));
$$;


--
-- Name: calc_interactions_is_system_event(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_is_system_event(p_interactions_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(interaction_type, '') FROM interactions WHERE interactions_id = p_interactions_id) = 'SystemEvent')::boolean;
$$;


--
-- Name: calc_interactions_is_task(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_is_task(p_interactions_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(interaction_type, '') FROM interactions WHERE interactions_id = p_interactions_id) = 'Task')::boolean;
$$;


--
-- Name: calc_interactions_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_name(p_interactions_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT /* WARNING: Formula translation failed: Function 'TEXT' is not supported yet
   Original Airtable formula:
   ={{Business}} & "-" & TEXT({{InteractionDate}}, "yyyy-mm-dd") & "-" & LOWER({{InteractionType}}) & "-" & SUBSTITUTE(LOWER({{Subject}}), " ", "-")
*/
NULL::text;
$$;


--
-- Name: calc_interactions_user_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_interactions_user_label(p_interactions_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT "user" FROM interactions WHERE interactions_id = p_interactions_id));
$$;


--
-- Name: calc_loans_business_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_business_label(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM loans WHERE loans_id = p_loans_id));
$$;


--
-- Name: calc_loans_business_naics_code(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_business_naics_code(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT naics_code::text FROM businesses
          WHERE calc_businesses_name(businesses_id) = (SELECT business FROM loans WHERE loans_id = p_loans_id));
$$;


--
-- Name: calc_loans_count_of_breached_covenants(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_count_of_breached_covenants(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM covenants WHERE loan = calc_loans_name(p_loans_id)))::integer;
$$;


--
-- Name: calc_loans_count_of_covenants(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_count_of_covenants(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM covenants WHERE loan = calc_loans_name(p_loans_id)))::integer;
$$;


--
-- Name: calc_loans_count_of_documents(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_count_of_documents(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM documents WHERE loan = calc_loans_name(p_loans_id)))::integer;
$$;


--
-- Name: calc_loans_count_of_risk_rating_history(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_count_of_risk_rating_history(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM risk_rating_history WHERE loan = calc_loans_name(p_loans_id)))::integer;
$$;


--
-- Name: calc_loans_dscr_in_band(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_dscr_in_band(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((COALESCE((SELECT dscr FROM loans WHERE loans_id = p_loans_id), 0))::NUMERIC >= 1.20)::boolean;
$$;


--
-- Name: calc_loans_has_breached_covenant(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_has_breached_covenant(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_loans_count_of_breached_covenants(p_loans_id))::NUMERIC > 0)::boolean;
$$;


--
-- Name: calc_loans_health_score(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_health_score(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $_$
  SELECT ((COALESCE(CASE WHEN (CASE WHEN calc_loans_dscr_in_band(p_loans_id) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_dscr_in_band(p_loans_id) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN ((COALESCE(CASE WHEN (CASE WHEN calc_loans_ltv_in_band(p_loans_id) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_ltv_in_band(p_loans_id) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN ((COALESCE(CASE WHEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0)))::text ~ '^-?[0-9]*\.?[0-9]+$' THEN ((COALESCE(CASE WHEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0)))::numeric ELSE NULL END, 0)))::text ~ '^-?[0-9]*\.?[0-9]+$' THEN ((COALESCE(CASE WHEN (CASE WHEN calc_loans_ltv_in_band(p_loans_id) THEN (1)::text ELSE (0)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_ltv_in_band(p_loans_id) THEN (1)::text ELSE (0)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN ((COALESCE(CASE WHEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0)))::text ~ '^-?[0-9]*\.?[0-9]+$' THEN ((COALESCE(CASE WHEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_is_classified_asset(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0) + COALESCE(CASE WHEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (CASE WHEN calc_loans_has_breached_covenant(p_loans_id) THEN (0)::text ELSE (1)::text END)::numeric ELSE NULL END, 0)))::numeric ELSE NULL END, 0)))::numeric ELSE NULL END, 0)))::integer;
$_$;


--
-- Name: calc_loans_is_classified_asset(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_is_classified_asset(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT (((SELECT risk_rating FROM loans WHERE loans_id = p_loans_id))::NUMERIC >= 7)::boolean;
$$;


--
-- Name: calc_loans_is_funded(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_is_funded(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(underwriting_stage, '') FROM loans WHERE loans_id = p_loans_id) = 'Funded')::boolean;
$$;


--
-- Name: calc_loans_ltv_in_band(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_ltv_in_band(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((COALESCE((SELECT ltv FROM loans WHERE loans_id = p_loans_id), 0))::NUMERIC <= 0.80)::boolean;
$$;


--
-- Name: calc_loans_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_name(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (REPLACE(LOWER((SELECT NULLIF(loan_number, '') FROM loans WHERE loans_id = p_loans_id)), ' ', '-'))::text;
$$;


--
-- Name: calc_loans_on_watchlist(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_on_watchlist(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_loans_is_classified_asset(p_loans_id) OR calc_loans_has_breached_covenant(p_loans_id)))::boolean;
$$;


--
-- Name: calc_loans_originating_rm_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_originating_rm_label(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT originating_rm FROM loans WHERE loans_id = p_loans_id));
$$;


--
-- Name: calc_loans_segregation_of_duties_ok(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_segregation_of_duties_ok(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT (NOT ((SELECT NULLIF(originating_rm, '') FROM loans WHERE loans_id = p_loans_id) = (SELECT NULLIF(underwriter, '') FROM loans WHERE loans_id = p_loans_id)))::boolean;
$$;


--
-- Name: calc_loans_underwriter_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_loans_underwriter_label(p_loans_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT calc_users_is_admin((SELECT underwriter FROM loans WHERE loans_id = p_loans_id));
$$;


--
-- Name: calc_risk_rating_history_changed_by_user_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_changed_by_user_label(p_risk_rating_history_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT full_name::text FROM users
          WHERE calc_users_name(users_id) = (SELECT changed_by_user FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id));
$$;


--
-- Name: calc_risk_rating_history_crossed_classified_threshold(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_crossed_classified_threshold(p_risk_rating_history_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT (((COALESCE((SELECT prior_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id), 0))::NUMERIC < 7 AND ((SELECT new_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id))::NUMERIC >= 7));
$$;


--
-- Name: calc_risk_rating_history_grade_delta(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_grade_delta(p_risk_rating_history_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $_$
  SELECT ((COALESCE(CASE WHEN ((SELECT new_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id))::text ~ '^-?[0-9]*\.?[0-9]+$' THEN ((SELECT new_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id))::numeric ELSE NULL END, 0) - COALESCE(CASE WHEN (COALESCE((SELECT prior_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id), (SELECT new_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id)))::text ~ '^-?[0-9]*\.?[0-9]+$' THEN (COALESCE((SELECT prior_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id), (SELECT new_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id)))::numeric ELSE NULL END, 0)))::integer;
$_$;


--
-- Name: calc_risk_rating_history_is_downgrade(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_is_downgrade(p_risk_rating_history_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((calc_risk_rating_history_grade_delta(p_risk_rating_history_id))::NUMERIC > 0)::boolean;
$$;


--
-- Name: calc_risk_rating_history_is_initial_rating(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_is_initial_rating(p_risk_rating_history_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((COALESCE((SELECT prior_grade FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id), 0))::NUMERIC = 0)::boolean;
$$;


--
-- Name: calc_risk_rating_history_loan_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_loan_label(p_risk_rating_history_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT loan_number::text FROM loans
          WHERE calc_loans_name(loans_id) = (SELECT loan FROM risk_rating_history WHERE risk_rating_history_id = p_risk_rating_history_id));
$$;


--
-- Name: calc_risk_rating_history_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_risk_rating_history_name(p_risk_rating_history_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT /* WARNING: Formula translation failed: Function 'TEXT' is not supported yet
   Original Airtable formula:
   ={{Loan}} & "-" & TEXT({{EffectiveDate}}, "yyyy-mm-dd") & "-grade-" & TEXT({{NewGrade}}, "0")
*/
NULL::text;
$$;


--
-- Name: calc_users_count_of_originated_loans(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_count_of_originated_loans(p_users_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM loans WHERE originating_rm = calc_users_name(p_users_id)))::integer;
$$;


--
-- Name: calc_users_count_of_portfolio_businesses(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_count_of_portfolio_businesses(p_users_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM businesses WHERE relationship_manager = calc_users_name(p_users_id)))::integer;
$$;


--
-- Name: calc_users_count_of_underwritten_loans(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_count_of_underwritten_loans(p_users_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT COUNT(*) FROM loans WHERE underwriter = calc_users_name(p_users_id)))::integer;
$$;


--
-- Name: calc_users_is_admin(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_is_admin(p_users_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(role, '') FROM users WHERE users_id = p_users_id) = 'Admin')::boolean;
$$;


--
-- Name: calc_users_is_branch_banker(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_is_branch_banker(p_users_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(role, '') FROM users WHERE users_id = p_users_id) = 'BranchBanker')::boolean;
$$;


--
-- Name: calc_users_is_rm(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_is_rm(p_users_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(role, '') FROM users WHERE users_id = p_users_id) = 'RM')::boolean;
$$;


--
-- Name: calc_users_is_underwriter(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_is_underwriter(p_users_id text) RETURNS boolean
    LANGUAGE sql STABLE
    AS $$
  SELECT ((SELECT NULLIF(role, '') FROM users WHERE users_id = p_users_id) = 'Underwriter')::boolean;
$$;


--
-- Name: calc_users_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calc_users_name(p_users_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (REPLACE(LOWER((SELECT NULLIF(full_name, '') FROM users WHERE users_id = p_users_id)), ' ', '-'))::text;
$$;


--
-- Name: get_businesses_annual_revenue_usd(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_annual_revenue_usd(p_businesses_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT annual_revenue_usd FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_businesses_business_structure(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_business_structure(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT business_structure FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_businesses_legal_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_legal_name(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT legal_name FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_businesses_naics_code(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_naics_code(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT naics_code FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_businesses_naics_description(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_naics_description(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT naics_description FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_businesses_onboarded_at(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_onboarded_at(p_businesses_id text) RETURNS timestamp with time zone
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT onboarded_at FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_businesses_status(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_businesses_status(p_businesses_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT status FROM businesses WHERE businesses_id = p_businesses_id);
$$;


--
-- Name: get_loans_dscr(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_dscr(p_loans_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT dscr FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_funded_at(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_funded_at(p_loans_id text) RETURNS timestamp with time zone
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT funded_at FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_global_cash_flow_usd(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_global_cash_flow_usd(p_loans_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT global_cash_flow_usd FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_loan_number(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_loan_number(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT loan_number FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_loan_purpose(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_loan_purpose(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT loan_purpose FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_ltv(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_ltv(p_loans_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT ltv FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_originated_at(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_originated_at(p_loans_id text) RETURNS timestamp with time zone
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT originated_at FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_principal_usd(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_principal_usd(p_loans_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT principal_usd FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_rate_pct(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_rate_pct(p_loans_id text) RETURNS numeric
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT rate_pct FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_risk_rating(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_risk_rating(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT risk_rating FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_risk_rating_label(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_risk_rating_label(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT risk_rating_label FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_term_months(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_term_months(p_loans_id text) RETURNS integer
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT term_months FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_loans_underwriting_stage(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_loans_underwriting_stage(p_loans_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT underwriting_stage FROM loans WHERE loans_id = p_loans_id);
$$;


--
-- Name: get_users_email(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_users_email(p_users_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT email FROM users WHERE users_id = p_users_id);
$$;


--
-- Name: get_users_full_name(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_users_full_name(p_users_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT full_name FROM users WHERE users_id = p_users_id);
$$;


--
-- Name: get_users_role(text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_users_role(p_users_id text) RETURNS text
    LANGUAGE sql STABLE
    AS $$
  SELECT (SELECT role FROM users WHERE users_id = p_users_id);
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: accounts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.accounts (
    accounts_id text NOT NULL,
    business text,
    account_type text,
    account_number_last4 text,
    current_balance_usd numeric,
    has_ach boolean,
    has_wire boolean,
    has_card boolean,
    opened_at timestamp with time zone
);


--
-- Name: TABLE accounts; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.accounts IS 'Deposit accounts the Business holds at the bank (checking, savings, money market). Balances feed GlobalCashFlow during credit analysis. TreasuryServices enrollment flags (ACH, Wire, Card) are stored per-account.';


--
-- Name: COLUMN accounts.accounts_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.accounts_id IS 'Primary key (auto-synthesized: Accounts had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN accounts.business; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.business IS 'FK to the parent Business.';


--
-- Name: COLUMN accounts.account_type; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.account_type IS 'Checking, Savings, MoneyMarket, etc.';


--
-- Name: COLUMN accounts.account_number_last4; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.account_number_last4 IS 'Last 4 digits of the account number for display.';


--
-- Name: COLUMN accounts.current_balance_usd; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.current_balance_usd IS 'Current balance in USD; sourced from CoreBanking.';


--
-- Name: COLUMN accounts.has_ach; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.has_ach IS 'ACH treasury service enrolled on this account.';


--
-- Name: COLUMN accounts.has_wire; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.has_wire IS 'Wire transfer treasury service enrolled on this account.';


--
-- Name: COLUMN accounts.has_card; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.has_card IS 'Business card product issued against this account.';


--
-- Name: COLUMN accounts.opened_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.accounts.opened_at IS 'Date the account was opened.';


--
-- Name: beneficial_owners; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.beneficial_owners (
    beneficial_owners_id text NOT NULL,
    business text,
    full_name text,
    date_of_birth_encrypted text,
    ssn_encrypted text,
    address_encrypted text,
    ownership_percentage numeric,
    is_control_person boolean
);


--
-- Name: TABLE beneficial_owners; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.beneficial_owners IS 'Individuals owning 25%+ of a Business plus designated control persons, per FinCEN''s CDD rule. PII (SSN, DOB, address) is stored encrypted at rest via field-level encryption.';


--
-- Name: COLUMN beneficial_owners.beneficial_owners_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.beneficial_owners_id IS 'Primary key (auto-synthesized: BeneficialOwners had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN beneficial_owners.business; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.business IS 'FK to the parent Business.';


--
-- Name: COLUMN beneficial_owners.full_name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.full_name IS 'Owner''s full legal name.';


--
-- Name: COLUMN beneficial_owners.date_of_birth_encrypted; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.date_of_birth_encrypted IS 'Encrypted date of birth (PII, FieldLevelEncryption).';


--
-- Name: COLUMN beneficial_owners.ssn_encrypted; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.ssn_encrypted IS 'Encrypted Social Security Number (PII, FieldLevelEncryption).';


--
-- Name: COLUMN beneficial_owners.address_encrypted; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.address_encrypted IS 'Encrypted home address (PII, FieldLevelEncryption).';


--
-- Name: COLUMN beneficial_owners.ownership_percentage; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.ownership_percentage IS 'Percent of the Business owned by this individual (0-100).';


--
-- Name: COLUMN beneficial_owners.is_control_person; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.beneficial_owners.is_control_person IS 'True when this individual is the designated control person per CDD (one is required when no owner crosses 25%).';


--
-- Name: businesses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.businesses (
    businesses_id text NOT NULL,
    legal_name text,
    business_structure text,
    naics_code text,
    naics_description text,
    annual_revenue_usd numeric,
    status text,
    onboarded_at timestamp with time zone,
    relationship_manager text,
    referral_source text
);


--
-- Name: TABLE businesses; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.businesses IS 'Small-business customers (and prospects) of the bank. Central entity: BeneficialOwners, Contacts, Accounts, Loans, Documents, and Interactions all hang off a Business. BusinessProfile information (legal name, structure, NAICS code) lives directly on this table.';


--
-- Name: COLUMN businesses.businesses_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.businesses_id IS 'Primary key (auto-synthesized: Businesses had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN businesses.legal_name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.legal_name IS 'Registered legal name of the business.';


--
-- Name: COLUMN businesses.business_structure; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.business_structure IS 'Entity structure: LLC, C-Corp, S-Corp, Partnership, Sole Proprietor, etc.';


--
-- Name: COLUMN businesses.naics_code; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.naics_code IS 'North American Industry Classification System code for the business.';


--
-- Name: COLUMN businesses.naics_description; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.naics_description IS 'Human-readable label for the NAICS code.';


--
-- Name: COLUMN businesses.annual_revenue_usd; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.annual_revenue_usd IS 'Most-recent annual revenue, USD.';


--
-- Name: COLUMN businesses.status; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.status IS 'Lifecycle status: Prospect, Customer, or Closed.';


--
-- Name: COLUMN businesses.onboarded_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.onboarded_at IS 'Date onboarding completed (KYB cleared, first account opened).';


--
-- Name: COLUMN businesses.relationship_manager; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.relationship_manager IS 'FK to Users: the RM who owns this business relationship (the SinglePointOfContact).';


--
-- Name: COLUMN businesses.referral_source; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.businesses.referral_source IS 'Optional FK to another Business that referred this prospect/customer in.';


--
-- Name: contacts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.contacts (
    contacts_id text NOT NULL,
    business text,
    full_name text,
    title text,
    email text,
    phone text,
    contact_type text,
    is_authorized_signer boolean
);


--
-- Name: TABLE contacts; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.contacts IS 'Non-owner individuals associated with a Business: officers, AP clerks, authorized signers, additional points of contact. Distinct from BeneficialOwners (which carries PII/CDD weight).';


--
-- Name: COLUMN contacts.contacts_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.contacts_id IS 'Primary key (auto-synthesized: Contacts had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN contacts.business; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.business IS 'FK to the parent Business.';


--
-- Name: COLUMN contacts.full_name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.full_name IS 'Contact''s full name.';


--
-- Name: COLUMN contacts.title; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.title IS 'Job title at the business (CEO, CFO, AP Clerk, etc.).';


--
-- Name: COLUMN contacts.email; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.email IS 'Contact email.';


--
-- Name: COLUMN contacts.phone; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.phone IS 'Contact phone.';


--
-- Name: COLUMN contacts.contact_type; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.contact_type IS 'Discriminator: Officer, APClerk, AuthorizedSigner, Other.';


--
-- Name: COLUMN contacts.is_authorized_signer; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.contacts.is_authorized_signer IS 'True when this contact can sign on the business''s accounts.';


--
-- Name: covenants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.covenants (
    covenants_id text NOT NULL,
    loan text,
    covenant_type text,
    threshold_value numeric,
    test_frequency text,
    next_test_date timestamp with time zone,
    status text,
    current_waiver_through timestamp with time zone
);


--
-- Name: TABLE covenants; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.covenants IS 'Conditions attached to a Loan that must be tested on a recurring schedule (e.g. minimum DSCR each quarter). CovenantMonitoring runs the tickler calendar ahead of NextTestDate; breaches surface as SystemEvent Interactions.';


--
-- Name: COLUMN covenants.covenants_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.covenants_id IS 'Primary key (auto-synthesized: Covenants had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN covenants.loan; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.loan IS 'FK to the parent Loan.';


--
-- Name: COLUMN covenants.covenant_type; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.covenant_type IS 'MinDSCR, MaxLeverage, ReportingFinancials, etc.';


--
-- Name: COLUMN covenants.threshold_value; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.threshold_value IS 'Numeric threshold for the test (e.g. 1.20 for MinDSCR); null for non-numeric covenants.';


--
-- Name: COLUMN covenants.test_frequency; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.test_frequency IS 'Quarterly, Annual, Monthly, etc.';


--
-- Name: COLUMN covenants.next_test_date; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.next_test_date IS 'Date of the next scheduled covenant test.';


--
-- Name: COLUMN covenants.status; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.status IS 'Active, Breached, Waived, Cured, Closed.';


--
-- Name: COLUMN covenants.current_waiver_through; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.covenants.current_waiver_through IS 'Date through which an active CovenantWaiver is in effect; null when no active waiver.';


--
-- Name: documents; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.documents (
    documents_id text NOT NULL,
    business text,
    loan text,
    uploaded_by_user text,
    filename text,
    document_type text,
    ocr_indexed boolean,
    uploaded_via text,
    uploaded_at timestamp with time zone
);


--
-- Name: TABLE documents; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.documents IS 'Files in the DocumentVault. A document attaches to either a Business (tax returns, formation docs) or a Loan (note, security agreement, appraisal). Both FKs are nullable so a document can hang off the appropriate parent.';


--
-- Name: COLUMN documents.documents_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.documents_id IS 'Primary key (auto-synthesized: Documents had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN documents.business; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.business IS 'Optional FK to a Business (nullable for loan-only docs).';


--
-- Name: COLUMN documents.loan; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.loan IS 'Optional FK to a Loan (nullable for business-only docs).';


--
-- Name: COLUMN documents.uploaded_by_user; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.uploaded_by_user IS 'Optional FK to the User who uploaded (null when uploaded via the BusinessClientPortal).';


--
-- Name: COLUMN documents.filename; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.filename IS 'Original filename in the vault.';


--
-- Name: COLUMN documents.document_type; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.document_type IS 'TaxReturn, FormationDoc, FinancialStatement, Note, SecurityAgreement, Appraisal, etc.';


--
-- Name: COLUMN documents.ocr_indexed; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.ocr_indexed IS 'True once OCR has indexed the document into SearchAndIndexing.';


--
-- Name: COLUMN documents.uploaded_via; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.uploaded_via IS 'Source surface: RMDashboard, BranchBankerPortal, BusinessClientPortal, AdminConsole, API.';


--
-- Name: COLUMN documents.uploaded_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.documents.uploaded_at IS 'Upload timestamp.';


--
-- Name: interactions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.interactions (
    interactions_id text NOT NULL,
    business text,
    "user" text,
    interaction_type text,
    subject text,
    body text,
    interaction_date timestamp with time zone,
    due_date timestamp with time zone,
    source text
);


--
-- Name: TABLE interactions; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.interactions IS 'Unified activity-log feed for a Business. An Interaction is intentionally generic: InteractionType discriminates Note, Call, Visit, Task, Meeting, or SystemEvent. Covenant breaches, document requests, and other machine actions are written as SystemEvent interactions so they appear in the same stream as human-logged activity.';


--
-- Name: COLUMN interactions.interactions_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.interactions_id IS 'Primary key (auto-synthesized: Interactions had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN interactions.business; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.business IS 'FK to the parent Business.';


--
-- Name: COLUMN interactions."user"; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions."user" IS 'FK to Users: who logged the interaction (null for system-generated or customer-portal-originated rows).';


--
-- Name: COLUMN interactions.interaction_type; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.interaction_type IS 'Note, Call, Visit, Task, Meeting, or SystemEvent.';


--
-- Name: COLUMN interactions.subject; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.subject IS 'Short subject line (used in compound PK).';


--
-- Name: COLUMN interactions.body; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.body IS 'Long-form body of the interaction.';


--
-- Name: COLUMN interactions.interaction_date; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.interaction_date IS 'Date the interaction occurred or was logged.';


--
-- Name: COLUMN interactions.due_date; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.due_date IS 'For Task-type interactions: due date.';


--
-- Name: COLUMN interactions.source; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.interactions.source IS 'Origin surface: RMDashboard, BusinessClientPortal, BranchBankerPortal, SystemEvent.';


--
-- Name: loans; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.loans (
    loans_id text NOT NULL,
    loan_number text,
    business text,
    originating_rm text,
    underwriter text,
    loan_purpose text,
    principal_usd numeric,
    rate_pct numeric,
    term_months integer,
    underwriting_stage text,
    risk_rating integer,
    risk_rating_label text,
    dscr numeric,
    ltv numeric,
    global_cash_flow_usd numeric,
    originated_at timestamp with time zone,
    funded_at timestamp with time zone
);


--
-- Name: TABLE loans; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.loans IS 'Credit facilities extended to a Business, tracked from inquiry through funding, servicing, and payoff. RiskRating is denormalized here for fast reads; every change is captured in RiskRatingHistory.';


--
-- Name: COLUMN loans.loans_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.loans_id IS 'Primary key (auto-synthesized: Loans had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN loans.loan_number; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.loan_number IS 'Bank-assigned loan number (e.g. ''L-0051'').';


--
-- Name: COLUMN loans.business; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.business IS 'FK to the borrower Business.';


--
-- Name: COLUMN loans.originating_rm; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.originating_rm IS 'FK to Users: the RM who originated this loan.';


--
-- Name: COLUMN loans.underwriter; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.underwriter IS 'FK to Users: the underwriter who underwrote this loan.';


--
-- Name: COLUMN loans.loan_purpose; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.loan_purpose IS 'Equipment, CRE, LOC, WorkingCapital, etc.';


--
-- Name: COLUMN loans.principal_usd; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.principal_usd IS 'Loan principal at origination, USD.';


--
-- Name: COLUMN loans.rate_pct; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.rate_pct IS 'Interest rate, percent (e.g. 7.25).';


--
-- Name: COLUMN loans.term_months; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.term_months IS 'Loan term in months.';


--
-- Name: COLUMN loans.underwriting_stage; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.underwriting_stage IS 'Current pipeline stage: Inquiry, PreQualification, Application, KYBAndBureauPull, Packaging, CreditAnalysis, CommitteeApproval, ClosingPrep, Signing, Funded.';


--
-- Name: COLUMN loans.risk_rating; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.risk_rating IS 'Current risk grade (1=Pass strongest ... 7=Substandard ... 9=Loss). Denormalized; every change is captured in RiskRatingHistory.';


--
-- Name: COLUMN loans.risk_rating_label; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.risk_rating_label IS 'Human-readable label for the current risk grade.';


--
-- Name: COLUMN loans.dscr; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.dscr IS 'Most recent debt service coverage ratio.';


--
-- Name: COLUMN loans.ltv; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.ltv IS 'Most recent loan-to-value ratio (0-1); null for unsecured loans.';


--
-- Name: COLUMN loans.global_cash_flow_usd; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.global_cash_flow_usd IS 'Most recent global cash flow figure across business, owners, and affiliates.';


--
-- Name: COLUMN loans.originated_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.originated_at IS 'Date the application was created.';


--
-- Name: COLUMN loans.funded_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.loans.funded_at IS 'Date the loan funded; null until funding completes.';


--
-- Name: risk_rating_history; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.risk_rating_history (
    risk_rating_history_id text NOT NULL,
    loan text,
    changed_by_user text,
    effective_date timestamp with time zone,
    prior_grade integer,
    new_grade integer,
    reason text
);


--
-- Name: TABLE risk_rating_history; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.risk_rating_history IS 'Time-series of risk-grade changes on a Loan. Regulators audit rating drift, so every migration is captured here. AnnualReview also writes a row even when the grade is reaffirmed unchanged.';


--
-- Name: COLUMN risk_rating_history.risk_rating_history_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.risk_rating_history_id IS 'Primary key (auto-synthesized: RiskRatingHistory had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN risk_rating_history.loan; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.loan IS 'FK to the Loan whose grade changed.';


--
-- Name: COLUMN risk_rating_history.changed_by_user; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.changed_by_user IS 'FK to Users: who recorded the migration.';


--
-- Name: COLUMN risk_rating_history.effective_date; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.effective_date IS 'Date the new grade takes effect.';


--
-- Name: COLUMN risk_rating_history.prior_grade; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.prior_grade IS 'Grade before this change; null on initial rating.';


--
-- Name: COLUMN risk_rating_history.new_grade; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.new_grade IS 'Grade after this change.';


--
-- Name: COLUMN risk_rating_history.reason; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.risk_rating_history.reason IS 'Free-text justification for the migration.';


--
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    users_id text NOT NULL,
    full_name text,
    role text,
    email text
);


--
-- Name: TABLE users; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.users IS 'Bank employees: relationship managers, underwriters, branch bankers, and admins. Used for portfolio assignment, audit trails, and segregation-of-duties enforcement.';


--
-- Name: COLUMN users.users_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.users.users_id IS 'Primary key (auto-synthesized: Users had no field named ''Id'' or ending in ''Id'').';


--
-- Name: COLUMN users.full_name; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.users.full_name IS 'Display label of the bank employee.';


--
-- Name: COLUMN users.role; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.users.role IS 'Role discriminator: RM, Underwriter, BranchBanker, or Admin.';


--
-- Name: COLUMN users.email; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.users.email IS 'Bank email used for SSO and notifications.';


--
-- Name: vw_accounts; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_accounts WITH (security_invoker='on') AS
 SELECT accounts_id,
    public.calc_accounts_name(accounts_id) AS name,
    business,
    account_type,
    account_number_last4,
    current_balance_usd,
    has_ach,
    has_wire,
    has_card,
    opened_at,
    public.calc_accounts_business_label(accounts_id) AS business_label,
    public.calc_accounts_treasury_service_count(accounts_id) AS treasury_service_count,
    public.calc_accounts_has_any_treasury_service(accounts_id) AS has_any_treasury_service
   FROM public.accounts t;


--
-- Name: vw_beneficial_owners; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_beneficial_owners WITH (security_invoker='on') AS
 SELECT beneficial_owners_id,
    public.calc_beneficial_owners_name(beneficial_owners_id) AS name,
    business,
    full_name,
    date_of_birth_encrypted,
    ssn_encrypted,
    address_encrypted,
    ownership_percentage,
    is_control_person,
    public.calc_beneficial_owners_business_label(beneficial_owners_id) AS business_label,
    public.calc_beneficial_owners_meets25_percent_threshold(beneficial_owners_id) AS meets25_percent_threshold,
    public.calc_beneficial_owners_meets_cdd_threshold(beneficial_owners_id) AS meets_cdd_threshold
   FROM public.beneficial_owners t;


--
-- Name: vw_businesses; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_businesses WITH (security_invoker='on') AS
 SELECT businesses_id,
    public.calc_businesses_name(businesses_id) AS name,
    legal_name,
    business_structure,
    naics_code,
    naics_description,
    annual_revenue_usd,
    status,
    onboarded_at,
    relationship_manager,
    referral_source,
    public.calc_businesses_relationship_manager_label(businesses_id) AS relationship_manager_label,
    public.calc_businesses_is_customer(businesses_id) AS is_customer,
    public.calc_businesses_is_prospect(businesses_id) AS is_prospect,
    public.calc_businesses_was_referred(businesses_id) AS was_referred,
    public.calc_businesses_count_of_beneficial_owners(businesses_id) AS count_of_beneficial_owners,
    public.calc_businesses_count_of_contacts(businesses_id) AS count_of_contacts,
    public.calc_businesses_count_of_accounts(businesses_id) AS count_of_accounts,
    public.calc_businesses_count_of_loans(businesses_id) AS count_of_loans,
    public.calc_businesses_count_of_interactions(businesses_id) AS count_of_interactions,
    public.calc_businesses_count_of_documents(businesses_id) AS count_of_documents,
    public.calc_businesses_total_deposit_balance_usd(businesses_id) AS total_deposit_balance_usd,
    public.calc_businesses_total_loan_principal_usd(businesses_id) AS total_loan_principal_usd,
    public.calc_businesses_count_of_classified_loans(businesses_id) AS count_of_classified_loans,
    public.calc_businesses_has_classified_loan(businesses_id) AS has_classified_loan,
    public.calc_businesses_beneficial_owners_at_cdd_threshold(businesses_id) AS beneficial_owners_at_cdd_threshold,
    public.calc_businesses_meets_cdd_rule(businesses_id) AS meets_cdd_rule,
    public.calc_businesses_portfolio_priority(businesses_id) AS portfolio_priority
   FROM public.businesses t;


--
-- Name: vw_contacts; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_contacts WITH (security_invoker='on') AS
 SELECT contacts_id,
    public.calc_contacts_name(contacts_id) AS name,
    business,
    full_name,
    title,
    email,
    phone,
    contact_type,
    is_authorized_signer,
    public.calc_contacts_business_label(contacts_id) AS business_label,
    public.calc_contacts_is_officer(contacts_id) AS is_officer,
    public.calc_contacts_is_ap_clerk(contacts_id) AS is_ap_clerk
   FROM public.contacts t;


--
-- Name: vw_covenants; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_covenants WITH (security_invoker='on') AS
 SELECT covenants_id,
    public.calc_covenants_name(covenants_id) AS name,
    loan,
    covenant_type,
    threshold_value,
    test_frequency,
    next_test_date,
    status,
    current_waiver_through,
    public.calc_covenants_loan_label(covenants_id) AS loan_label,
    public.calc_covenants_loan_business(covenants_id) AS loan_business,
    public.calc_covenants_is_breached(covenants_id) AS is_breached,
    public.calc_covenants_has_active_waiver(covenants_id) AS has_active_waiver
   FROM public.covenants t;


--
-- Name: vw_documents; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_documents WITH (security_invoker='on') AS
 SELECT documents_id,
    public.calc_documents_name(documents_id) AS name,
    business,
    loan,
    uploaded_by_user,
    filename,
    document_type,
    ocr_indexed,
    uploaded_via,
    uploaded_at,
    public.calc_documents_business_label(documents_id) AS business_label,
    public.calc_documents_loan_label(documents_id) AS loan_label,
    public.calc_documents_uploaded_by_user_label(documents_id) AS uploaded_by_user_label,
    public.calc_documents_attached_to(documents_id) AS attached_to,
    public.calc_documents_from_customer_portal(documents_id) AS from_customer_portal
   FROM public.documents t;


--
-- Name: vw_interactions; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_interactions WITH (security_invoker='on') AS
 SELECT interactions_id,
    public.calc_interactions_name(interactions_id) AS name,
    business,
    "user",
    interaction_type,
    subject,
    body,
    interaction_date,
    due_date,
    source,
    public.calc_interactions_business_label(interactions_id) AS business_label,
    public.calc_interactions_user_label(interactions_id) AS user_label,
    public.calc_interactions_is_system_event(interactions_id) AS is_system_event,
    public.calc_interactions_is_task(interactions_id) AS is_task,
    public.calc_interactions_from_customer(interactions_id) AS from_customer,
    public.calc_interactions_is_covenant_event(interactions_id) AS is_covenant_event
   FROM public.interactions t;


--
-- Name: vw_loans; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_loans WITH (security_invoker='on') AS
 SELECT loans_id,
    public.calc_loans_name(loans_id) AS name,
    loan_number,
    business,
    originating_rm,
    underwriter,
    loan_purpose,
    principal_usd,
    rate_pct,
    term_months,
    underwriting_stage,
    risk_rating,
    risk_rating_label,
    dscr,
    ltv,
    global_cash_flow_usd,
    originated_at,
    funded_at,
    public.calc_loans_business_label(loans_id) AS business_label,
    public.calc_loans_business_naics_code(loans_id) AS business_naics_code,
    public.calc_loans_originating_rm_label(loans_id) AS originating_rm_label,
    public.calc_loans_underwriter_label(loans_id) AS underwriter_label,
    public.calc_loans_is_funded(loans_id) AS is_funded,
    public.calc_loans_is_classified_asset(loans_id) AS is_classified_asset,
    public.calc_loans_dscr_in_band(loans_id) AS dscr_in_band,
    public.calc_loans_ltv_in_band(loans_id) AS ltv_in_band,
    public.calc_loans_segregation_of_duties_ok(loans_id) AS segregation_of_duties_ok,
    public.calc_loans_count_of_covenants(loans_id) AS count_of_covenants,
    public.calc_loans_count_of_breached_covenants(loans_id) AS count_of_breached_covenants,
    public.calc_loans_count_of_risk_rating_history(loans_id) AS count_of_risk_rating_history,
    public.calc_loans_count_of_documents(loans_id) AS count_of_documents,
    public.calc_loans_has_breached_covenant(loans_id) AS has_breached_covenant,
    public.calc_loans_on_watchlist(loans_id) AS on_watchlist,
    public.calc_loans_health_score(loans_id) AS health_score
   FROM public.loans t;


--
-- Name: vw_risk_rating_history; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_risk_rating_history WITH (security_invoker='on') AS
 SELECT risk_rating_history_id,
    public.calc_risk_rating_history_name(risk_rating_history_id) AS name,
    loan,
    changed_by_user,
    effective_date,
    prior_grade,
    new_grade,
    reason,
    public.calc_risk_rating_history_loan_label(risk_rating_history_id) AS loan_label,
    public.calc_risk_rating_history_changed_by_user_label(risk_rating_history_id) AS changed_by_user_label,
    public.calc_risk_rating_history_grade_delta(risk_rating_history_id) AS grade_delta,
    public.calc_risk_rating_history_is_downgrade(risk_rating_history_id) AS is_downgrade,
    public.calc_risk_rating_history_is_initial_rating(risk_rating_history_id) AS is_initial_rating,
    public.calc_risk_rating_history_crossed_classified_threshold(risk_rating_history_id) AS crossed_classified_threshold
   FROM public.risk_rating_history t;


--
-- Name: vw_users; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vw_users WITH (security_invoker='on') AS
 SELECT users_id,
    public.calc_users_name(users_id) AS name,
    full_name,
    role,
    email,
    public.calc_users_is_rm(users_id) AS is_rm,
    public.calc_users_is_underwriter(users_id) AS is_underwriter,
    public.calc_users_is_branch_banker(users_id) AS is_branch_banker,
    public.calc_users_is_admin(users_id) AS is_admin,
    public.calc_users_count_of_portfolio_businesses(users_id) AS count_of_portfolio_businesses,
    public.calc_users_count_of_originated_loans(users_id) AS count_of_originated_loans,
    public.calc_users_count_of_underwritten_loans(users_id) AS count_of_underwritten_loans
   FROM public.users t;


--
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.accounts (accounts_id, business, account_type, account_number_last4, current_balance_usd, has_ach, has_wire, has_card, opened_at) FROM stdin;
d77761a2-b2c7-a864-29c8-a2e6f0157814	stillwater-roasters-llc	Checking	0001	184250.13	t	t	f	2024-03-13 00:00:00-05
286ace58-0a28-1ef6-3b7c-f2d4f481b9cd	stillwater-roasters-llc	Savings	0101	75000	f	f	f	2024-03-13 00:00:00-05
5230f51b-f004-2c8d-f808-29af892a328b	pine-hardware-inc	Checking	0200	612430.55	t	t	t	2019-06-22 00:00:00-05
a2f634a6-e8ad-de42-e0b2-373a2fe39ccf	ridgeway-consulting-llc	Checking	0300	92800	t	f	f	2023-09-06 00:00:00-05
e6c0af7e-38b6-204c-9b7e-0402fcaf511a	sunset-auto-body-llc	Checking	0400	41250	f	f	f	2022-11-03 00:00:00-05
\.


--
-- Data for Name: beneficial_owners; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.beneficial_owners (beneficial_owners_id, business, full_name, date_of_birth_encrypted, ssn_encrypted, address_encrypted, ownership_percentage, is_control_person) FROM stdin;
b5ea91e4-e7c0-d966-933c-b30eb8fd5472	stillwater-roasters-llc	Jordan Park	ENC(1981-04-17)	ENC(xxx-xx-1234)	ENC(742 Evergreen Way, Stillwater)	60	t
cf2c611e-cf32-480a-2034-bde5dd340507	stillwater-roasters-llc	Sam Rivera	ENC(1978-11-02)	ENC(xxx-xx-5678)	ENC(15 Hillcrest Rd, Stillwater)	40	f
6ce52c78-4a62-1a60-23f1-55a37e346a21	ridgeway-consulting-llc	Alex Ridgeway	ENC(1972-02-14)	ENC(xxx-xx-9012)	ENC(204 Birch Ln, Ridgeway)	0	t
c419e8e8-4e17-362f-caac-3c21225a224f	pine-hardware-inc	Elena Novak	ENC(1965-08-30)	ENC(xxx-xx-3456)	ENC(99 Oak St, Pinehurst)	51	t
74411093-d89c-5e49-9403-57329d127b2b	pine-hardware-inc	Robert Novak	ENC(1962-01-19)	ENC(xxx-xx-7890)	ENC(99 Oak St, Pinehurst)	49	f
\.


--
-- Data for Name: businesses; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.businesses (businesses_id, legal_name, business_structure, naics_code, naics_description, annual_revenue_usd, status, onboarded_at, relationship_manager, referral_source) FROM stdin;
c3843334-7c20-fb48-b139-c32dfccecdf7	Stillwater Roasters LLC	LLC	311920	Coffee and Tea Manufacturing	4200000	Customer	2024-03-12 00:00:00-05	devon-marshall	\N
853dc5fc-0f5a-c300-7e44-dfb6bbf91b58	Ridgeway Consulting LLC	LLC	541611	Administrative Management Consulting	1800000	Customer	2023-09-05 00:00:00-05	devon-marshall	\N
d198b720-c0e9-2ed2-2070-b010ec7e7142	Pine Hardware Inc.	C-Corp	444110	Home Centers	12500000	Customer	2019-06-21 00:00:00-05	devon-marshall	\N
8d4bb109-2271-b4b9-9f92-45297e292ffc	Sunset Auto Body LLC	LLC	811121	Automotive Body, Paint, and Interior Repair	2300000	Customer	2022-11-02 00:00:00-05	devon-marshall	\N
6a540187-0826-5225-0f56-2128bad9fcbb	Hilltop Bakery LLC	LLC	311811	Retail Bakeries	850000	Prospect	\N	devon-marshall	pine-hardware-inc
\.


--
-- Data for Name: contacts; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.contacts (contacts_id, business, full_name, title, email, phone, contact_type, is_authorized_signer) FROM stdin;
23746565-c5ad-6197-2075-5ae57552fc4c	stillwater-roasters-llc	Jordan Park	CEO	jordan@stillwaterroasters.com	555-0142	Officer	t
0f3c3c40-ed05-4ea6-456e-a6f545881f08	stillwater-roasters-llc	Sam Rivera	CFO	sam@stillwaterroasters.com	555-0143	Officer	t
0e08cc84-87db-8a61-529e-8b6082052d01	pine-hardware-inc	Marta Keene	AP Clerk	ap@pinehardware.com	555-0188	APClerk	f
e99ccba9-0509-d994-85fb-a992e32bb785	ridgeway-consulting-llc	Alex Ridgeway	CEO	alex@ridgeway.co	555-0260	Officer	t
\.


--
-- Data for Name: covenants; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.covenants (covenants_id, loan, covenant_type, threshold_value, test_frequency, next_test_date, status, current_waiver_through) FROM stdin;
d52676e2-acd7-3bac-ec50-989014614af2	l-0051	MinDSCR	1.2	Quarterly	2025-06-30 00:00:00-05	Active	\N
593494f9-1e0d-fa49-059d-a3e531013bf5	l-0051	MaxLeverage	3	Annual	2026-03-01 00:00:00-06	Active	\N
e0d23a01-54b0-493e-9c9f-3977abc432b5	l-0051	ReportingFinancials	\N	Annual	2026-03-31 00:00:00-05	Active	\N
b4669ce3-1970-3fde-4bda-acd37fd619b5	l-0042	MinDSCR	1.2	Quarterly	2026-06-30 00:00:00-05	Breached	2026-06-30 00:00:00-05
\.


--
-- Data for Name: documents; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.documents (documents_id, business, loan, uploaded_by_user, filename, document_type, ocr_indexed, uploaded_via, uploaded_at) FROM stdin;
aa0b473d-66cd-07dc-20bb-9a647d3c4e51	stillwater-roasters-llc	\N	devon-marshall	stillwater-2024-tax-return.pdf	TaxReturn	t	RMDashboard	2025-01-22 00:00:00-06
5ba9121b-1265-19ab-a385-0699d16006d4	\N	l-0051	priya-iyer	l-0051-promissory-note.pdf	Note	t	RMDashboard	2025-02-28 00:00:00-06
4903ebf3-212e-6783-415b-885e7779d32e	\N	l-0051	priya-iyer	l-0051-security-agreement.pdf	SecurityAgreement	t	RMDashboard	2025-02-28 00:00:00-06
acef9966-dcfa-719a-2e0d-fe1f76583bdf	stillwater-roasters-llc	\N	\N	stillwater-q4-2024-pl.pdf	FinancialStatement	t	BusinessClientPortal	2025-01-15 00:00:00-06
1a69d337-2d5b-786f-dc53-b42571e88d0f	pine-hardware-inc	\N	devon-marshall	pine-articles-of-incorporation.pdf	FormationDoc	t	RMDashboard	2019-06-21 00:00:00-05
\.


--
-- Data for Name: interactions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.interactions (interactions_id, business, "user", interaction_type, subject, body, interaction_date, due_date, source) FROM stdin;
54cd2c15-3be6-5864-a45f-07c8ad56071a	stillwater-roasters-llc	devon-marshall	Note	Q2 PnL review	Reviewed Q2 P&L. Margins compressed slightly on green-coffee cost spike but volume up 12% YoY.	2025-04-15 00:00:00-05	\N	RMDashboard
c18c4512-db6c-502f-1543-9510b2478ac9	stillwater-roasters-llc	devon-marshall	Call	Wire fee question	Sam called re: $30 outbound wire fee. Confirmed price; offered ACH-international as alternative.	2025-04-15 00:00:00-05	\N	RMDashboard
0844c210-ec0c-5035-d36e-1005a2744dea	pine-hardware-inc	devon-marshall	Visit	Refi discussion	On-site visit. Discussed potential refinance of L-0042 once classified-asset status resolves.	2025-04-15 00:00:00-05	\N	RMDashboard
576ccd71-2cb0-206c-3fde-bf9c36a958d2	ridgeway-consulting-llc	devon-marshall	Meeting	Annual review prep	Scheduled annual review meeting. Documentation requested in advance.	2025-04-22 00:00:00-05	\N	RMDashboard
2c09192d-0e51-996d-0fe4-31b96be70953	pine-hardware-inc	devon-marshall	Task	Refi term sheet	Draft and send proposed refinance term sheet to Elena Novak.	2025-04-15 00:00:00-05	2025-04-18 00:00:00-05	RMDashboard
d1686983-03b9-9542-1629-5960fc04d7c6	pine-hardware-inc	\N	SystemEvent	Covenant test approaching	Auto-generated by CovenantMonitoring: DSCR test for L-0042 due in 15 days.	2025-04-15 00:00:00-05	\N	SystemEvent
29914a33-55f9-5a30-5b65-106b0a323f03	pine-hardware-inc	\N	SystemEvent	Covenant breach L-0042 DSCR	DSCR test failed on L-0042. Computed 1.08; covenant threshold 1.20. Status: Breached.	2025-05-02 00:00:00-05	\N	SystemEvent
f24a956d-4cc0-137a-f807-a02a8602753d	sunset-auto-body-llc	renee-okafor	SystemEvent	SAR filed BSA AML	Verafin flagged structuring pattern. SAR drafted and filed via RegulatoryReporting to FinCEN.	2025-05-04 00:00:00-05	\N	SystemEvent
85156cc2-261a-187a-3849-c7bafbc96844	stillwater-roasters-llc	\N	Note	Customer portal message	Customer message: 'Uploaded Q4 P&L. Want to discuss a $25K LOC.'	2025-01-15 00:00:00-06	\N	BusinessClientPortal
\.


--
-- Data for Name: loans; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.loans (loans_id, loan_number, business, originating_rm, underwriter, loan_purpose, principal_usd, rate_pct, term_months, underwriting_stage, risk_rating, risk_rating_label, dscr, ltv, global_cash_flow_usd, originated_at, funded_at) FROM stdin;
46fca949-a81c-1590-e136-26f6996616bd	L-0051	stillwater-roasters-llc	devon-marshall	priya-iyer	Equipment	250000	7.25	60	Funded	4	Pass	1.42	0.78	612000	2025-02-10 00:00:00-06	2025-03-01 00:00:00-06
109471e6-b7f8-1cb6-eade-a9a61b3dd316	L-0042	pine-hardware-inc	devon-marshall	thomas-bell	CRE	1850000	6.5	240	Funded	7	Substandard	1.08	0.71	2100000	2021-04-14 00:00:00-05	2021-05-20 00:00:00-05
73633058-2d03-ca45-bfa1-cd3a677cf273	L-0078	ridgeway-consulting-llc	devon-marshall	priya-iyer	LOC	100000	8	12	Funded	3	Pass	1.85	\N	320000	2024-08-01 00:00:00-05	2024-08-22 00:00:00-05
\.


--
-- Data for Name: risk_rating_history; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.risk_rating_history (risk_rating_history_id, loan, changed_by_user, effective_date, prior_grade, new_grade, reason) FROM stdin;
20199086-2a95-e55a-5c00-918d0909d478	l-0042	thomas-bell	2021-05-20 00:00:00-05	\N	4	Initial rating at funding.
51804094-8b97-c951-c3aa-81e188cd7cc1	l-0042	thomas-bell	2025-05-02 00:00:00-05	4	6	DSCR covenant breach; downgrade to Special Mention.
ba8005e1-30f8-e6ec-192f-f8ea525cbab9	l-0042	thomas-bell	2025-08-15 00:00:00-05	6	7	Sustained underperformance; downgrade to Substandard. Added to Watchlist.
34c91435-ddc1-150f-409d-8b3ebecd7108	l-0051	priya-iyer	2025-03-01 00:00:00-06	\N	4	Initial rating at funding.
472687e4-14c1-ecd7-91d8-9e68edbe883f	l-0078	priya-iyer	2024-08-22 00:00:00-05	\N	3	Initial rating at funding; strong cash flow coverage.
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.users (users_id, full_name, role, email) FROM stdin;
ad4be29e-a5bf-580d-389c-5f30eeee46bc	Devon Marshall	RM	devon.marshall@firstvalley.bank
13aa93cc-1635-dbae-adc3-e16c4c8f189a	Priya Iyer	Underwriter	priya.iyer@firstvalley.bank
81d5dd35-7659-baf9-bc72-19c5f7c1df7e	Thomas Bell	Underwriter	thomas.bell@firstvalley.bank
f4ae9be6-f070-5ce7-f784-14b8b5c90777	Maya Chen	BranchBanker	maya.chen@firstvalley.bank
777037ee-5947-c52a-b33c-63ce36b5069b	Renee Okafor	Admin	renee.okafor@firstvalley.bank
\.


--
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (accounts_id);


--
-- Name: beneficial_owners beneficial_owners_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.beneficial_owners
    ADD CONSTRAINT beneficial_owners_pkey PRIMARY KEY (beneficial_owners_id);


--
-- Name: businesses businesses_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.businesses
    ADD CONSTRAINT businesses_pkey PRIMARY KEY (businesses_id);


--
-- Name: contacts contacts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacts
    ADD CONSTRAINT contacts_pkey PRIMARY KEY (contacts_id);


--
-- Name: covenants covenants_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.covenants
    ADD CONSTRAINT covenants_pkey PRIMARY KEY (covenants_id);


--
-- Name: documents documents_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT documents_pkey PRIMARY KEY (documents_id);


--
-- Name: interactions interactions_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.interactions
    ADD CONSTRAINT interactions_pkey PRIMARY KEY (interactions_id);


--
-- Name: loans loans_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.loans
    ADD CONSTRAINT loans_pkey PRIMARY KEY (loans_id);


--
-- Name: risk_rating_history risk_rating_history_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.risk_rating_history
    ADD CONSTRAINT risk_rating_history_pkey PRIMARY KEY (risk_rating_history_id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (users_id);


--
-- Name: idx_accounts_business; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_accounts_business ON public.accounts USING btree (business);


--
-- Name: idx_beneficial_owners_business; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_beneficial_owners_business ON public.beneficial_owners USING btree (business);


--
-- Name: idx_businesses_referral_source; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_businesses_referral_source ON public.businesses USING btree (referral_source);


--
-- Name: idx_businesses_relationship_manager; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_businesses_relationship_manager ON public.businesses USING btree (relationship_manager);


--
-- Name: idx_contacts_business; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_contacts_business ON public.contacts USING btree (business);


--
-- Name: idx_covenants_loan; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_covenants_loan ON public.covenants USING btree (loan);


--
-- Name: idx_documents_business; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_documents_business ON public.documents USING btree (business);


--
-- Name: idx_documents_loan; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_documents_loan ON public.documents USING btree (loan);


--
-- Name: idx_documents_uploaded_by_user; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_documents_uploaded_by_user ON public.documents USING btree (uploaded_by_user);


--
-- Name: idx_interactions_business; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_interactions_business ON public.interactions USING btree (business);


--
-- Name: idx_interactions_user; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_interactions_user ON public.interactions USING btree ("user");


--
-- Name: idx_loans_business; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_loans_business ON public.loans USING btree (business);


--
-- Name: idx_loans_originating_rm; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_loans_originating_rm ON public.loans USING btree (originating_rm);


--
-- Name: idx_loans_underwriter; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_loans_underwriter ON public.loans USING btree (underwriter);


--
-- Name: idx_risk_rating_history_changed_by_user; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_risk_rating_history_changed_by_user ON public.risk_rating_history USING btree (changed_by_user);


--
-- Name: idx_risk_rating_history_loan; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_risk_rating_history_loan ON public.risk_rating_history USING btree (loan);


--
-- Name: accounts; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.accounts ENABLE ROW LEVEL SECURITY;

--
-- Name: beneficial_owners; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.beneficial_owners ENABLE ROW LEVEL SECURITY;

--
-- Name: businesses; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.businesses ENABLE ROW LEVEL SECURITY;

--
-- Name: contacts; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.contacts ENABLE ROW LEVEL SECURITY;

--
-- Name: covenants; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.covenants ENABLE ROW LEVEL SECURITY;

--
-- Name: documents; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.documents ENABLE ROW LEVEL SECURITY;

--
-- Name: interactions; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.interactions ENABLE ROW LEVEL SECURITY;

--
-- Name: loans; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.loans ENABLE ROW LEVEL SECURITY;

--
-- Name: risk_rating_history; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.risk_rating_history ENABLE ROW LEVEL SECURITY;

--
-- Name: users; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;

--
-- PostgreSQL database dump complete
--

\unrestrict MgRNtj3b5u2lRGgHsfB4CK1pG1HXAknMMSZDFr5trWALjW3WVQ5JmryPdtA4fxF

