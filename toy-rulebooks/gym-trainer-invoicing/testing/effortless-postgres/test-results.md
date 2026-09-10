# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 204 |
| Passed | 50 |
| Failed | 154 |
| Score | 24.5% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 48 | 93 | 51.6% |
| Lookup (INDEX/MATCH) | 1 | 84 | 1.2% |
| Aggregation (COUNTIFS/SUMIFS) | 1 | 27 | 3.7% |

## Results by Entity

### trainers

- Fields: 0/6 (0.0%)
- Computed columns: total_sessions, total_billed, total_outstanding

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| t-alex | total_sessions | None | 9 |
| t-alex | total_billed | None | 1061.800 |
| t-alex | total_outstanding | None | 561.800 |
| t-jamie | total_sessions | None | 4 |
| t-jamie | total_billed | None | 476.500 |
| t-jamie | total_outstanding | None | 476.500 |

### clients

- Fields: 2/32 (6.2%)
- Computed columns: trainer_name, trainer_email, trainer_hourly_rate, session_count, total_invoiced, outstanding_balance, overdue_count, status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| c-morgan | trainer_name | None | Jamie Lin |
| c-morgan | trainer_email | None | jamie@gym.test |
| c-morgan | trainer_hourly_rate | None | 95 |
| c-morgan | session_count | None | 2 |
| c-morgan | total_invoiced | None | 205.00 |
| c-morgan | outstanding_balance | None | 205.00 |
| c-morgan | overdue_count | None | 1 |
| c-morgan | status | Paid Up | Overdue |
| c-robin | trainer_name | None | Alex Reyes |
| c-robin | trainer_email | None | alex@gym.test |
| c-robin | trainer_hourly_rate | None | 80 |
| c-robin | session_count | None | 4 |
| c-robin | total_invoiced | None | 447.000 |
| c-robin | outstanding_balance | None | -53.000 |
| c-sam | trainer_name | None | Alex Reyes |
| c-sam | trainer_email | None | alex@gym.test |
| c-sam | trainer_hourly_rate | None | 80 |
| c-sam | session_count | None | 5 |
| c-sam | total_invoiced | None | 614.800 |
| c-sam | outstanding_balance | None | 614.800 |
| ... | ... | (10 more) | ... |

### invoices

- Fields: 32/75 (42.7%)
- Computed columns: client_name, client_email, trainer, trainer_name, subtotal, tax_amount, grace_days, days_since_due_date, days_past_due, late_fee, total, balance, is_paid, is_overdue, status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| inv-001 | client_name | None | Sam Patel |
| inv-001 | client_email | None | sam@example.com |
| inv-001 | trainer | None | t-alex |
| inv-001 | trainer_name | None | Alex Reyes |
| inv-001 | subtotal | None | 280.0 |
| inv-001 | tax_amount | 0.0 | 22.400 |
| inv-001 | total | 15.0 | 317.400 |
| inv-001 | balance | 15.0 | 317.400 |
| inv-002 | client_name | None | Sam Patel |
| inv-002 | client_email | None | sam@example.com |
| inv-002 | trainer | None | t-alex |
| inv-002 | trainer_name | None | Alex Reyes |
| inv-002 | subtotal | None | 280.0 |
| inv-002 | tax_amount | 0.0 | 22.400 |
| inv-002 | total | -5.0 | 297.400 |
| inv-002 | balance | -5.0 | 297.400 |
| inv-002 | is_paid | True | False |
| inv-002 | is_overdue | False | True |
| inv-002 | status | Paid | Overdue |
| inv-003 | client_name | None | Robin Cole |
| ... | ... | (23 more) | ... |

### sessions

- Fields: 16/91 (17.6%)
- Computed columns: client_name, trainer, client_hourly_rate, effective_rate, line_total, invoice_name, is_billed

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| s-001 | client_name | None | Sam Patel |
| s-001 | trainer | None | t-alex |
| s-001 | client_hourly_rate | None | 80 |
| s-001 | effective_rate | None | 80 |
| s-001 | line_total | 0.0 | 80.0 |
| s-001 | invoice_name | None | INV-001 |
| s-002 | client_name | None | Sam Patel |
| s-002 | trainer | None | t-alex |
| s-002 | client_hourly_rate | None | 80 |
| s-002 | effective_rate | None | 80 |
| s-002 | line_total | 0.0 | 120.0 |
| s-002 | invoice_name | None | INV-001 |
| s-003 | client_name | None | Sam Patel |
| s-003 | trainer | None | t-alex |
| s-003 | client_hourly_rate | None | 80 |
| s-003 | effective_rate | None | 80 |
| s-003 | line_total | 0.0 | 80.0 |
| s-003 | invoice_name | None | INV-001 |
| s-004 | client_name | None | Sam Patel |
| s-004 | trainer | None | t-alex |
| ... | ... | (55 more) | ... |
