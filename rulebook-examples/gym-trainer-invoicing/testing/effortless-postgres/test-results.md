# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 204 |
| Passed | 163 |
| Failed | 41 |
| Score | 79.9% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 52 | 93 | 55.9% |
| Lookup (INDEX/MATCH) | 84 | 84 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 27 | 27 | 100.0% |

## Results by Entity

### sessions

- Fields: 77/91 (84.6%)
- Computed columns: client_name, trainer, client_hourly_rate, effective_rate, line_total, invoice_name, is_billed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| s-001 | is_billed | True | None |
| s-002 | is_billed | True | None |
| s-003 | is_billed | True | None |
| s-004 | is_billed | True | None |
| s-005 | effective_rate | 100 | None |
| s-005 | line_total | 200.0 | None |
| s-005 | is_billed | True | None |
| s-006 | is_billed | True | None |
| s-007 | is_billed | True | None |
| s-008 | is_billed | True | None |
| s-010 | is_billed | True | None |
| s-011 | is_billed | True | None |
| s-012 | is_billed | True | None |
| s-013 | is_billed | True | None |

### invoices

- Fields: 52/75 (69.3%)
- Computed columns: client_name, client_email, trainer, trainer_name, subtotal, tax_amount, grace_days, days_since_due_date, days_past_due, late_fee, total, balance, is_paid, is_overdue, status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| inv-001 | grace_days | 45 | None |
| inv-001 | days_since_due_date | 41 | None |
| inv-001 | is_paid | True | None |
| inv-001 | status | Paid | None |
| inv-002 | grace_days | 45 | None |
| inv-002 | days_since_due_date | 11 | None |
| inv-002 | total | -20.0 | None |
| inv-002 | balance | -20.0 | None |
| inv-002 | is_paid | True | None |
| inv-002 | status | Paid | None |
| inv-003 | grace_days | 45 | None |
| inv-003 | days_since_due_date | 1 | None |
| inv-003 | balance | -500.0 | None |
| inv-003 | is_paid | True | None |
| inv-003 | status | Paid | None |
| inv-004 | grace_days | 45 | None |
| inv-004 | days_since_due_date | 26 | None |
| inv-004 | is_paid | True | None |
| inv-004 | status | Paid | None |
| inv-005 | grace_days | 45 | None |
| ... | ... | (3 more) | ... |

### trainers

- Fields: 6/6 (100.0%)
- Computed columns: total_sessions, total_billed, total_outstanding

### clients

- Fields: 28/32 (87.5%)
- Computed columns: trainer_name, trainer_email, trainer_hourly_rate, session_count, total_invoiced, outstanding_balance, overdue_count, status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| c-morgan | status | Paid Up | None |
| c-robin | status | Paid Up | None |
| c-sam | status | Paid Up | None |
| c-taylor | status | Paid Up | None |
