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

-- Businesses
ALTER TABLE businesses DROP CONSTRAINT IF EXISTS fk_businesses_relationship_manager;
ALTER TABLE businesses ADD CONSTRAINT fk_businesses_relationship_manager
  FOREIGN KEY (relationship_manager) REFERENCES users (users_id);
ALTER TABLE businesses DROP CONSTRAINT IF EXISTS fk_businesses_referral_source;
ALTER TABLE businesses ADD CONSTRAINT fk_businesses_referral_source
  FOREIGN KEY (referral_source) REFERENCES businesses (businesses_id);

-- BeneficialOwners
ALTER TABLE beneficial_owners DROP CONSTRAINT IF EXISTS fk_beneficial_owners_business;
ALTER TABLE beneficial_owners ADD CONSTRAINT fk_beneficial_owners_business
  FOREIGN KEY (business) REFERENCES businesses (businesses_id);

-- Contacts
ALTER TABLE contacts DROP CONSTRAINT IF EXISTS fk_contacts_business;
ALTER TABLE contacts ADD CONSTRAINT fk_contacts_business
  FOREIGN KEY (business) REFERENCES businesses (businesses_id);

-- Accounts
ALTER TABLE accounts DROP CONSTRAINT IF EXISTS fk_accounts_business;
ALTER TABLE accounts ADD CONSTRAINT fk_accounts_business
  FOREIGN KEY (business) REFERENCES businesses (businesses_id);

-- Loans
ALTER TABLE loans DROP CONSTRAINT IF EXISTS fk_loans_business;
ALTER TABLE loans ADD CONSTRAINT fk_loans_business
  FOREIGN KEY (business) REFERENCES businesses (businesses_id);
ALTER TABLE loans DROP CONSTRAINT IF EXISTS fk_loans_originating_rm;
ALTER TABLE loans ADD CONSTRAINT fk_loans_originating_rm
  FOREIGN KEY (originating_rm) REFERENCES users (users_id);
ALTER TABLE loans DROP CONSTRAINT IF EXISTS fk_loans_underwriter;
ALTER TABLE loans ADD CONSTRAINT fk_loans_underwriter
  FOREIGN KEY (underwriter) REFERENCES users (users_id);

-- Covenants
ALTER TABLE covenants DROP CONSTRAINT IF EXISTS fk_covenants_loan;
ALTER TABLE covenants ADD CONSTRAINT fk_covenants_loan
  FOREIGN KEY (loan) REFERENCES loans (loans_id);

-- RiskRatingHistory
ALTER TABLE risk_rating_history DROP CONSTRAINT IF EXISTS fk_risk_rating_history_loan;
ALTER TABLE risk_rating_history ADD CONSTRAINT fk_risk_rating_history_loan
  FOREIGN KEY (loan) REFERENCES loans (loans_id);
ALTER TABLE risk_rating_history DROP CONSTRAINT IF EXISTS fk_risk_rating_history_changed_by_user;
ALTER TABLE risk_rating_history ADD CONSTRAINT fk_risk_rating_history_changed_by_user
  FOREIGN KEY (changed_by_user) REFERENCES users (users_id);

-- Documents
ALTER TABLE documents DROP CONSTRAINT IF EXISTS fk_documents_business;
ALTER TABLE documents ADD CONSTRAINT fk_documents_business
  FOREIGN KEY (business) REFERENCES businesses (businesses_id);
ALTER TABLE documents DROP CONSTRAINT IF EXISTS fk_documents_loan;
ALTER TABLE documents ADD CONSTRAINT fk_documents_loan
  FOREIGN KEY (loan) REFERENCES loans (loans_id);
ALTER TABLE documents DROP CONSTRAINT IF EXISTS fk_documents_uploaded_by_user;
ALTER TABLE documents ADD CONSTRAINT fk_documents_uploaded_by_user
  FOREIGN KEY (uploaded_by_user) REFERENCES users (users_id);

-- Interactions
ALTER TABLE interactions DROP CONSTRAINT IF EXISTS fk_interactions_business;
ALTER TABLE interactions ADD CONSTRAINT fk_interactions_business
  FOREIGN KEY (business) REFERENCES businesses (businesses_id);
ALTER TABLE interactions DROP CONSTRAINT IF EXISTS fk_interactions_user;
ALTER TABLE interactions ADD CONSTRAINT fk_interactions_user
  FOREIGN KEY ("user") REFERENCES users (users_id);

-- 16 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
