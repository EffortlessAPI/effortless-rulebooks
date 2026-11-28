# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 390 |
| Passed | 390 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 231 | 231 | 100.0% |
| Lookup (INDEX/MATCH) | 82 | 82 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 77 | 77 | 100.0% |

## Results by Entity

### users

- Fields: 40/40 (100.0%)
- Computed columns: name, is_rm, is_underwriter, is_branch_banker, is_admin, count_of_portfolio_businesses, count_of_originated_loans, count_of_underwritten_loans

### businesses

- Fields: 90/90 (100.0%)
- Computed columns: name, relationship_manager_label, is_customer, is_prospect, was_referred, count_of_beneficial_owners, count_of_contacts, count_of_accounts, count_of_loans, count_of_interactions, count_of_documents, total_deposit_balance_usd, total_loan_principal_usd, count_of_classified_loans, has_classified_loan, beneficial_owners_at_cdd_threshold, meets_cdd_rule, portfolio_priority

### beneficial_owners

- Fields: 20/20 (100.0%)
- Computed columns: name, business_label, meets25_percent_threshold, meets_cdd_threshold

### contacts

- Fields: 16/16 (100.0%)
- Computed columns: name, business_label, is_officer, is_ap_clerk

### accounts

- Fields: 20/20 (100.0%)
- Computed columns: name, business_label, treasury_service_count, has_any_treasury_service

### loans

- Fields: 51/51 (100.0%)
- Computed columns: name, business_label, business_naics_code, originating_rm_label, underwriter_is_admin, is_funded, is_classified_asset, dscr_in_band, ltv_in_band, segregation_of_duties_ok, count_of_covenants, count_of_breached_covenants, count_of_risk_rating_history, count_of_documents, has_breached_covenant, on_watchlist, health_score

### covenants

- Fields: 20/20 (100.0%)
- Computed columns: name, loan_label, loan_business, is_breached, has_active_waiver

### risk_rating_history

- Fields: 35/35 (100.0%)
- Computed columns: name, loan_label, changed_by_user_label, grade_delta, is_downgrade, is_initial_rating, crossed_classified_threshold

### documents

- Fields: 30/30 (100.0%)
- Computed columns: name, business_label, loan_label, uploaded_by_user_label, attached_to, from_customer_portal

### interactions

- Fields: 63/63 (100.0%)
- Computed columns: name, business_label, user_label, is_system_event, is_task, from_customer, is_covenant_event

### constraints

- Fields: 5/5 (100.0%)
- Computed columns: name
