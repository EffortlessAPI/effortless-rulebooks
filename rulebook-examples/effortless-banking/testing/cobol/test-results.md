# Test Results: cobol

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 390 |
| Passed | 154 |
| Failed | 236 |
| Score | 39.5% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 108 | 231 | 46.8% |
| Lookup (INDEX/MATCH) | 10 | 82 | 12.2% |
| Aggregation (COUNTIFS/SUMIFS) | 36 | 77 | 46.8% |

## Results by Entity

### users

- Fields: 36/40 (90.0%)
- Computed columns: name, is_rm, is_underwriter, is_branch_banker, is_admin, count_of_portfolio_businesses, count_of_originated_loans, count_of_underwritten_loans

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| devon-marshall | count_of_portfolio_businesses | 5 | 0 |
| devon-marshall | count_of_originated_loans | 3 | 0 |
| priya-iyer | count_of_underwritten_loans | 2 | 0 |
| thomas-bell | count_of_underwritten_loans | 1 | 0 |

### businesses

- Fields: 35/90 (38.9%)
- Computed columns: name, relationship_manager_label, is_customer, is_prospect, was_referred, count_of_beneficial_owners, count_of_contacts, count_of_accounts, count_of_loans, count_of_interactions, count_of_documents, total_deposit_balance_usd, total_loan_principal_usd, count_of_classified_loans, has_classified_loan, beneficial_owners_at_cdd_threshold, meets_cdd_rule, portfolio_priority

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| hilltop-bakery-llc | name | hilltop-bakery-llc | None |
| hilltop-bakery-llc | relationship_manager_label | Devon Marshall | None |
| hilltop-bakery-llc | is_prospect | True | None |
| hilltop-bakery-llc | was_referred | True | None |
| hilltop-bakery-llc | portfolio_priority | Medium | None |
| pine-hardware-inc | name | pine-hardware-inc | None |
| pine-hardware-inc | relationship_manager_label | Devon Marshall | None |
| pine-hardware-inc | is_customer | True | None |
| pine-hardware-inc | count_of_beneficial_owners | 2 | None |
| pine-hardware-inc | count_of_contacts | 1 | None |
| pine-hardware-inc | count_of_accounts | 1 | None |
| pine-hardware-inc | count_of_loans | 1 | None |
| pine-hardware-inc | count_of_interactions | 4 | None |
| pine-hardware-inc | count_of_documents | 1 | None |
| pine-hardware-inc | total_deposit_balance_usd | 612430.55 | None |
| pine-hardware-inc | total_loan_principal_usd | 1850000 | None |
| pine-hardware-inc | count_of_classified_loans | 1 | None |
| pine-hardware-inc | has_classified_loan | True | None |
| pine-hardware-inc | beneficial_owners_at_cdd_threshold | 2 | None |
| pine-hardware-inc | meets_cdd_rule | True | None |
| ... | ... | (35 more) | ... |

### beneficial_owners

- Fields: 1/20 (5.0%)
- Computed columns: name, business_label, meets25_percent_threshold, meets_cdd_threshold

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pine-hardware-inc-elena-novak | name | pine-hardware-inc-elena-novak | None |
| pine-hardware-inc-elena-novak | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-elena-novak | meets25_percent_threshold | True | None |
| pine-hardware-inc-elena-novak | meets_cdd_threshold | True | None |
| pine-hardware-inc-robert-novak | name | pine-hardware-inc-robert-novak | None |
| pine-hardware-inc-robert-novak | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-robert-novak | meets25_percent_threshold | True | None |
| pine-hardware-inc-robert-novak | meets_cdd_threshold | True | None |
| ridgeway-consulting-llc-alex-ridgeway | name | ridgeway-consulting-llc-alex-r | None |
| ridgeway-consulting-llc-alex-ridgeway | business_label | Ridgeway Consulting LLC | None |
| ridgeway-consulting-llc-alex-ridgeway | meets_cdd_threshold | True | None |
| stillwater-roasters-llc-jordan-park | name | stillwater-roasters-llc-jordan | None |
| stillwater-roasters-llc-jordan-park | business_label | Stillwater Roasters LLC | None |
| stillwater-roasters-llc-jordan-park | meets25_percent_threshold | True | None |
| stillwater-roasters-llc-jordan-park | meets_cdd_threshold | True | None |
| stillwater-roasters-llc-sam-rivera | name | stillwater-roasters-llc-sam-ri | None |
| stillwater-roasters-llc-sam-rivera | business_label | Stillwater Roasters LLC | None |
| stillwater-roasters-llc-sam-rivera | meets25_percent_threshold | True | None |
| stillwater-roasters-llc-sam-rivera | meets_cdd_threshold | True | None |

### contacts

- Fields: 4/16 (25.0%)
- Computed columns: name, business_label, is_officer, is_ap_clerk

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pine-hardware-inc-marta-keene-ap-clerk | name | pine-hardware-inc-marta-keene- | None |
| pine-hardware-inc-marta-keene-ap-clerk | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-marta-keene-ap-clerk | is_ap_clerk | True | None |
| ridgeway-consulting-llc-alex-ridgeway-ceo | name | ridgeway-consulting-llc-alex-r | None |
| ridgeway-consulting-llc-alex-ridgeway-ceo | business_label | Ridgeway Consulting LLC | None |
| ridgeway-consulting-llc-alex-ridgeway-ceo | is_officer | True | None |
| stillwater-roasters-llc-jordan-park-ceo | name | stillwater-roasters-llc-jordan | None |
| stillwater-roasters-llc-jordan-park-ceo | business_label | Stillwater Roasters LLC | None |
| stillwater-roasters-llc-jordan-park-ceo | is_officer | True | None |
| stillwater-roasters-llc-sam-rivera-cfo | name | stillwater-roasters-llc-sam-ri | None |
| stillwater-roasters-llc-sam-rivera-cfo | business_label | Stillwater Roasters LLC | None |
| stillwater-roasters-llc-sam-rivera-cfo | is_officer | True | None |

### accounts

- Fields: 4/20 (20.0%)
- Computed columns: name, business_label, treasury_service_count, has_any_treasury_service

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pine-hardware-inc-checking-0200 | name | pine-hardware-inc-checking-020 | None |
| pine-hardware-inc-checking-0200 | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-checking-0200 | treasury_service_count | 3 | None |
| pine-hardware-inc-checking-0200 | has_any_treasury_service | True | None |
| ridgeway-consulting-llc-checking-0300 | name | ridgeway-consulting-llc-checki | None |
| ridgeway-consulting-llc-checking-0300 | business_label | Ridgeway Consulting LLC | None |
| ridgeway-consulting-llc-checking-0300 | treasury_service_count | 1 | None |
| ridgeway-consulting-llc-checking-0300 | has_any_treasury_service | True | None |
| stillwater-roasters-llc-checking-0001 | name | stillwater-roasters-llc-checki | None |
| stillwater-roasters-llc-checking-0001 | business_label | Stillwater Roasters LLC | None |
| stillwater-roasters-llc-checking-0001 | treasury_service_count | 2 | None |
| stillwater-roasters-llc-checking-0001 | has_any_treasury_service | True | None |
| stillwater-roasters-llc-savings-0101 | name | stillwater-roasters-llc-saving | None |
| stillwater-roasters-llc-savings-0101 | business_label | Stillwater Roasters LLC | None |
| sunset-auto-body-llc-checking-0400 | name | sunset-auto-body-llc-checking- | None |
| sunset-auto-body-llc-checking-0400 | business_label | Sunset Auto Body LLC | None |

### loans

- Fields: 13/51 (25.5%)
- Computed columns: name, business_label, business_naics_code, originating_rm_label, underwriter_is_admin, is_funded, is_classified_asset, dscr_in_band, ltv_in_band, segregation_of_duties_ok, count_of_covenants, count_of_breached_covenants, count_of_risk_rating_history, count_of_documents, has_breached_covenant, on_watchlist, health_score

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| l-0042 | name | l-0042 | None |
| l-0042 | business_label | Pine Hardware Inc. | None |
| l-0042 | business_naics_code | 444110 | None |
| l-0042 | originating_rm_label | Devon Marshall | None |
| l-0042 | is_funded | True | None |
| l-0042 | is_classified_asset | True | None |
| l-0042 | ltv_in_band | True | None |
| l-0042 | segregation_of_duties_ok | True | None |
| l-0042 | count_of_covenants | 1 | None |
| l-0042 | count_of_breached_covenants | 1 | None |
| l-0042 | count_of_risk_rating_history | 3 | None |
| l-0042 | has_breached_covenant | True | None |
| l-0042 | on_watchlist | True | None |
| l-0042 | health_score | 1 | None |
| l-0051 | name | l-0051 | None |
| l-0051 | business_label | Stillwater Roasters LLC | None |
| l-0051 | business_naics_code | 311920 | None |
| l-0051 | originating_rm_label | Devon Marshall | None |
| l-0051 | underwriter_is_admin | True | None |
| l-0051 | is_funded | True | None |
| ... | ... | (18 more) | ... |

### covenants

- Fields: 6/20 (30.0%)
- Computed columns: name, loan_label, loan_business, is_breached, has_active_waiver

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| l-0042-mindscr | name | l-0042-mindscr | None |
| l-0042-mindscr | loan_label | L-0042 | None |
| l-0042-mindscr | loan_business | pine-hardware-inc | None |
| l-0042-mindscr | is_breached | True | None |
| l-0042-mindscr | has_active_waiver | True | None |
| l-0051-maxleverage | name | l-0051-maxleverage | None |
| l-0051-maxleverage | loan_label | L-0051 | None |
| l-0051-maxleverage | loan_business | stillwater-roasters-llc | None |
| l-0051-mindscr | name | l-0051-mindscr | None |
| l-0051-mindscr | loan_label | L-0051 | None |
| l-0051-mindscr | loan_business | stillwater-roasters-llc | None |
| l-0051-reportingfinancials | name | l-0051-reportingfinancials | None |
| l-0051-reportingfinancials | loan_label | L-0051 | None |
| l-0051-reportingfinancials | loan_business | stillwater-roasters-llc | None |

### risk_rating_history

- Fields: 12/35 (34.3%)
- Computed columns: name, loan_label, changed_by_user_label, grade_delta, is_downgrade, is_initial_rating, crossed_classified_threshold

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| l-0042-2021-05-20-grade-4 | name | l-0042-2021-05-20-grade-4 | None |
| l-0042-2021-05-20-grade-4 | loan_label | L-0042 | None |
| l-0042-2021-05-20-grade-4 | changed_by_user_label | Thomas Bell | None |
| l-0042-2021-05-20-grade-4 | is_initial_rating | True | None |
| l-0042-2025-05-02-grade-6 | name | l-0042-2025-05-02-grade-6 | None |
| l-0042-2025-05-02-grade-6 | loan_label | L-0042 | None |
| l-0042-2025-05-02-grade-6 | changed_by_user_label | Thomas Bell | None |
| l-0042-2025-05-02-grade-6 | grade_delta | 2 | None |
| l-0042-2025-05-02-grade-6 | is_downgrade | True | None |
| l-0042-2025-08-15-grade-7 | name | l-0042-2025-08-15-grade-7 | None |
| l-0042-2025-08-15-grade-7 | loan_label | L-0042 | None |
| l-0042-2025-08-15-grade-7 | changed_by_user_label | Thomas Bell | None |
| l-0042-2025-08-15-grade-7 | grade_delta | 1 | None |
| l-0042-2025-08-15-grade-7 | is_downgrade | True | None |
| l-0042-2025-08-15-grade-7 | crossed_classified_threshold | True | None |
| l-0051-2025-03-01-grade-4 | name | l-0051-2025-03-01-grade-4 | None |
| l-0051-2025-03-01-grade-4 | loan_label | L-0051 | None |
| l-0051-2025-03-01-grade-4 | changed_by_user_label | Priya Iyer | None |
| l-0051-2025-03-01-grade-4 | is_initial_rating | True | None |
| l-0078-2024-08-22-grade-3 | name | l-0078-2024-08-22-grade-3 | None |
| ... | ... | (3 more) | ... |

### documents

- Fields: 10/30 (33.3%)
- Computed columns: name, business_label, loan_label, uploaded_by_user_label, attached_to, from_customer_portal

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| l-0051-promissory-note-pdf | name | l-0051-promissory-note-pdf | None |
| l-0051-promissory-note-pdf | loan_label | L-0051 | None |
| l-0051-promissory-note-pdf | uploaded_by_user_label | Priya Iyer | None |
| l-0051-promissory-note-pdf | attached_to | Loan | None |
| l-0051-security-agreement-pdf | name | l-0051-security-agreement-pdf | None |
| l-0051-security-agreement-pdf | loan_label | L-0051 | None |
| l-0051-security-agreement-pdf | uploaded_by_user_label | Priya Iyer | None |
| l-0051-security-agreement-pdf | attached_to | Loan | None |
| pine-articles-of-incorporation-pdf | name | pine-articles-of-incorporation | None |
| pine-articles-of-incorporation-pdf | business_label | Pine Hardware Inc. | None |
| pine-articles-of-incorporation-pdf | uploaded_by_user_label | Devon Marshall | None |
| pine-articles-of-incorporation-pdf | attached_to | Business | None |
| stillwater-2024-tax-return-pdf | name | stillwater-2024-tax-return-pdf | None |
| stillwater-2024-tax-return-pdf | business_label | Stillwater Roasters LLC | None |
| stillwater-2024-tax-return-pdf | uploaded_by_user_label | Devon Marshall | None |
| stillwater-2024-tax-return-pdf | attached_to | Business | None |
| stillwater-q4-2024-pl-pdf | name | stillwater-q4-2024-pl-pdf | None |
| stillwater-q4-2024-pl-pdf | business_label | Stillwater Roasters LLC | None |
| stillwater-q4-2024-pl-pdf | attached_to | Business | None |
| stillwater-q4-2024-pl-pdf | from_customer_portal | True | None |

### interactions

- Fields: 33/63 (52.4%)
- Computed columns: name, business_label, user_label, is_system_event, is_task, from_customer, is_covenant_event

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| pine-hardware-inc-2025-04-15-systemevent-covenant-test-approaching | name | pine-hardware-inc-2025-04-15-s | None |
| pine-hardware-inc-2025-04-15-systemevent-covenant-test-approaching | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-2025-04-15-systemevent-covenant-test-approaching | is_system_event | True | None |
| pine-hardware-inc-2025-04-15-task-refi-term-sheet | name | pine-hardware-inc-2025-04-15-t | None |
| pine-hardware-inc-2025-04-15-task-refi-term-sheet | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-2025-04-15-task-refi-term-sheet | user_label | Devon Marshall | None |
| pine-hardware-inc-2025-04-15-task-refi-term-sheet | is_task | True | None |
| pine-hardware-inc-2025-04-15-visit-refi-discussion | name | pine-hardware-inc-2025-04-15-v | None |
| pine-hardware-inc-2025-04-15-visit-refi-discussion | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-2025-04-15-visit-refi-discussion | user_label | Devon Marshall | None |
| pine-hardware-inc-2025-05-02-systemevent-covenant-breach-l-0042-dscr | name | pine-hardware-inc-2025-05-02-s | None |
| pine-hardware-inc-2025-05-02-systemevent-covenant-breach-l-0042-dscr | business_label | Pine Hardware Inc. | None |
| pine-hardware-inc-2025-05-02-systemevent-covenant-breach-l-0042-dscr | is_system_event | True | None |
| pine-hardware-inc-2025-05-02-systemevent-covenant-breach-l-0042-dscr | is_covenant_event | True | None |
| ridgeway-consulting-llc-2025-04-22-meeting-annual-review-prep | name | ridgeway-consulting-llc-2025-0 | None |
| ridgeway-consulting-llc-2025-04-22-meeting-annual-review-prep | business_label | Ridgeway Consulting LLC | None |
| ridgeway-consulting-llc-2025-04-22-meeting-annual-review-prep | user_label | Devon Marshall | None |
| stillwater-roasters-llc-2025-01-15-note-customer-portal-message | name | stillwater-roasters-llc-2025-0 | None |
| stillwater-roasters-llc-2025-01-15-note-customer-portal-message | business_label | Stillwater Roasters LLC | None |
| stillwater-roasters-llc-2025-01-15-note-customer-portal-message | from_customer | True | None |
| ... | ... | (10 more) | ... |

### constraints

- Fields: 0/5 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| biz-cdd | name | biz-cdd | None |
| loan-dscr | name | loan-dscr | None |
| loan-ltv | name | loan-ltv | None |
| loan-sod | name | loan-sod | None |
| loan-watchlist | name | loan-watchlist | None |
