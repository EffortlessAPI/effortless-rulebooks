# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 204 |
| Passed | 204 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 93 | 93 | 100.0% |
| Lookup (INDEX/MATCH) | 84 | 84 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 27 | 27 | 100.0% |

## Results by Entity

### trainers

- Fields: 6/6 (100.0%)
- Computed columns: total_sessions, total_billed, total_outstanding

### clients

- Fields: 32/32 (100.0%)
- Computed columns: trainer_name, trainer_email, trainer_hourly_rate, session_count, total_invoiced, outstanding_balance, overdue_count, status

### invoices

- Fields: 75/75 (100.0%)
- Computed columns: client_name, client_email, trainer, trainer_name, subtotal, tax_amount, grace_days, days_since_due_date, days_past_due, late_fee, total, balance, is_paid, is_overdue, status

### sessions

- Fields: 91/91 (100.0%)
- Computed columns: client_name, trainer, client_hourly_rate, effective_rate, line_total, invoice_name, is_billed
