# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 734 |
| Passed | 690 |
| Failed | 44 |
| Score | 94.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 280 | 291 | 96.2% |
| Lookup (INDEX/MATCH) | 343 | 360 | 95.3% |
| Aggregation (COUNTIFS/SUMIFS) | 67 | 83 | 80.7% |

## Results by Entity

### app_users

- Fields: 3/3 (100.0%)
- Computed columns: name

### clients

- Fields: 50/70 (71.4%)
- Computed columns: category_name, category_discount, is_stopped, status_display_name, status_description, count_of_big_invoices, billing_address_state_tax_rate, billing_address, shipping_address, average_order_value, is_vip, has_recent_invoices, last_invoice, customer_since_days

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson | shipping_address |  | 500 Pine St, Suite 200, Seattl |
| alice-johnson | last_invoice | 2025-10-05T09:00:00Z | 2025-10-05 04:00:00-05:00 |
| alice-johnson | customer_since_days | 266 | 373 |
| bob | count_of_big_invoices | 0 | 1 |
| bob | shipping_address |  | 742 Evergreen Terrace, Springf |
| bob | last_invoice | 2025-09-15T10:30:00Z | 2025-09-15 05:30:00-05:00 |
| bob | customer_since_days | 285 | 392 |
| brian-lee | count_of_big_invoices | 2 | 3 |
| brian-lee | shipping_address |  | 88 Industrial Way, Austin, tx- |
| brian-lee | last_invoice | 2026-02-20T13:15:00Z | 2026-02-20 07:15:00-06:00 |
| brian-lee | customer_since_days | 227 | 334 |
| carla-smith | count_of_big_invoices | 0 | 1 |
| carla-smith | shipping_address |  | 1200 Ocean Dr, Miami, fl-flori |
| carla-smith | has_recent_invoices | True | False |
| carla-smith | last_invoice | 2025-12-01T11:20:00Z | 2025-12-01 05:20:00-06:00 |
| carla-smith | customer_since_days | 185 | 292 |
| caroline | count_of_big_invoices | 0 | 1 |
| caroline | shipping_address |  | 23 Hudson Lane, Brooklyn, ny-n |
| caroline | last_invoice | 2026-03-12T10:00:00Z | 2026-03-12 05:00:00-05:00 |
| caroline | customer_since_days | 138 | 245 |

### client_approvals

- Fields: 27/30 (90.0%)
- Computed columns: name, approved_by_contact_name, approved_by_email_address, approved_by_phone_number, approved_by_role, is_approved, client_name, client_email, client_phone, client_category

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-apvd | name | Alice Johnson-APVD | alice-johnson-APVD |
| brian-lee-apvd | name | Brian Lee-APVD | brian-lee-APVD |
| brian-lee-pend | name | Brian Lee-PEND | brian-lee-PEND |

### client_categories

- Fields: 3/3 (100.0%)
- Computed columns: count_of_clients

### statuses

- Fields: 14/14 (100.0%)
- Computed columns: name, count_of_clients

### products

- Fields: 64/64 (100.0%)
- Computed columns: name, profit, margin, is_high_margin, stock_quantity, cogs, count_of_vip_orders, has_been_ordered_by_vip_customers

### invoices

- Fields: 139/147 (94.6%)
- Computed columns: name, client_is_vip, client_hast_recent_invoices, client_email, client_phone, client_company_name, client_category_name, client_category_discount, item_count, total_quantity, sub_total, is_big_order, tax_rate, tax_amount, invoice_total, total_paid, amount_due, is_paid_in_full, payment_count, last_payment_date, payment_status_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010 | last_payment_date | 2025-10-12 | 2025-10-12 06:30:00-05:00 |
| bob-1001 | last_payment_date | 2025-09-15 | 2025-09-15 05:35:00-05:00 |
| brian-lee-1018 | last_payment_date | 2025-11-15 | 2025-11-15 03:00:00-06:00 |
| brian-lee-1037 | last_payment_date | 2026-01-28 | 2026-01-28 08:05:00-06:00 |
| brian-lee-1042 | last_payment_date | 2026-02-20 | 2026-02-20 07:20:00-06:00 |
| carla-smith-1024 | client_hast_recent_invoices | True | False |
| carla-smith-1024 | last_payment_date | 2025-12-01 | 2025-12-01 05:25:00-06:00 |
| caroline-1045 | last_payment_date | 2026-03-12 | 2026-03-12 06:30:00-05:00 |

### invoice_line_items

- Fields: 230/234 (98.3%)
- Computed columns: name, client_is_vip, client_hast_recent_invoices, invoice_number, invoice_total, invoice_total_paid, product_sku, product_display_name, unit_price, pre_discount, discount_percent, discount_amount, sub_total

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| brian-lee-1018-line-1 | discount_amount | 0 | 0.00 |
| brian-lee-1042-line-2 | discount_amount | 0 | 0.00 |
| carla-smith-1024-line-1 | client_hast_recent_invoices | True | False |
| carla-smith-1024-line-2 | client_hast_recent_invoices | True | False |

### inventory_adjustments

- Fields: 8/8 (100.0%)
- Computed columns: adjustment_name

### payments

- Fields: 72/81 (88.9%)
- Computed columns: name, invoice_date, invoice_number, invoice_status, invoice_total, is_completed, completed_amount, order_amount_due, order_is_paid_in_full

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010-pmt-1 | invoice_date | 2025-10-05 | 2025-10-05 04:00:00-05:00 |
| alice-johnson-1010-pmt-2 | invoice_date | 2025-10-05 | 2025-10-05 04:00:00-05:00 |
| bob-1001-pmt-1 | invoice_date | 2025-09-15 | 2025-09-15 05:30:00-05:00 |
| brian-lee-1018-pmt-1 | invoice_date | 2025-11-12 | 2025-11-12 09:45:00-06:00 |
| brian-lee-1037-pmt-1 | invoice_date | 2026-01-28 | 2026-01-28 08:00:00-06:00 |
| brian-lee-1042-pmt-1 | invoice_date | 2026-02-20 | 2026-02-20 07:15:00-06:00 |
| carla-smith-1024-pmt-1 | invoice_date | 2025-12-01 | 2025-12-01 05:20:00-06:00 |
| caroline-1045-pmt-1 | invoice_date | 2026-03-12 | 2026-03-12 05:00:00-05:00 |
| caroline-1045-pmt-2 | invoice_date | 2026-03-12 | 2026-03-12 05:00:00-05:00 |

### addresses

- Fields: 30/30 (100.0%)
- Computed columns: name, type_of_address_is_shipping_address, type_of_address_is_billing_address, state_code, state_tax_rate

### states

- Fields: 50/50 (100.0%)
- Computed columns: name
