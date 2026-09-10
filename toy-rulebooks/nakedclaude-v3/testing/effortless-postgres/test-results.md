# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 309 |
| Passed | 260 |
| Failed | 49 |
| Score | 84.1% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 170 | 193 | 88.1% |
| Lookup (INDEX/MATCH) | 53 | 69 | 76.8% |
| Aggregation (COUNTIFS/SUMIFS) | 37 | 47 | 78.7% |

## Results by Entity

### customers

- Fields: 32/50 (64.0%)
- Computed columns: is_stopped, status_is_blocking, status_display_name, status_description, average_order_value, lifetime_margin_percent, customer_since_days, days_since_last_order, count_of_multiple_payment_orders, has_multi_payment_orders

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson | average_order_value | Unable to generate formula | 4770.44 |
| alice-johnson | customer_since_days | 266 | 373 |
| alice-johnson | days_since_last_order | Unable to generate formula | 340 |
| bobby | average_order_value | Unable to generate formula | 145.63 |
| bobby | customer_since_days | 285 | 392 |
| bobby | days_since_last_order | Unable to generate formula | 202 |
| bobby | count_of_multiple_payment_orders | 0 | 2 |
| brian-lee | average_order_value | Unable to generate formula | 649.51 |
| brian-lee | customer_since_days | 227 | 334 |
| brian-lee | days_since_last_order | Unable to generate formula | 302 |
| brian-lee | count_of_multiple_payment_orders | 0 | 1 |
| carla-smith | average_order_value | Unable to generate formula | 221.70 |
| carla-smith | customer_since_days | 185 | 292 |
| carla-smith | days_since_last_order | Unable to generate formula | 225 |
| carla-smith | count_of_multiple_payment_orders | 0 | 2 |
| caroline | average_order_value | Unable to generate formula | 126.82 |
| caroline | customer_since_days | 138 | 245 |
| caroline | days_since_last_order | Unable to generate formula | 182 |

### statuses

- Fields: 7/7 (100.0%)
- Computed columns: name

### products

- Fields: 8/8 (100.0%)
- Computed columns: name

### orders

- Fields: 79/91 (86.8%)
- Computed columns: name, item_count, total_quantity, sub_total, tax_amount, order_total, total_paid, amount_due, is_paid_in_full, payment_count, is_multi_payment_order, last_payment_date, payment_status_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010 | amount_due | 0 | 0.00 |
| alice-johnson-1010 | last_payment_date | 2025-10-12 | 2025-10-12 06:30:00-05:00 |
| bobby-1001 | amount_due | 0 | 0.00 |
| bobby-1001 | last_payment_date | 2025-09-15 | 2025-09-15 05:35:00-05:00 |
| bobby-1042 | last_payment_date | 2026-02-20 | 2026-02-20 07:20:00-06:00 |
| brian-lee-1018 | amount_due | 0 | 0.00 |
| brian-lee-1018 | last_payment_date | 2025-11-15 | 2025-11-15 03:00:00-06:00 |
| carla-smith-1024 | last_payment_date | 2025-12-01 | 2025-12-01 05:25:00-06:00 |
| carla-smith-1037 | amount_due | 0 | 0.00 |
| carla-smith-1037 | last_payment_date | 2026-01-28 | 2026-01-28 08:05:00-06:00 |
| caroline-1045 | amount_due | 0 | 0.00 |
| caroline-1045 | last_payment_date | 2026-03-12 | 2026-03-12 06:30:00-05:00 |

### order_line_items

- Fields: 69/72 (95.8%)
- Computed columns: name, pre_discount, discount_amount, sub_total

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bobby-1001-line-1 | discount_amount | 0 | 0.00 |
| bobby-1042-line-2 | discount_amount | 0 | 0.00 |
| caroline-1045-line-1 | discount_amount | 0 | 0.00 |

### payments

- Fields: 65/81 (80.2%)
- Computed columns: name, order_date, order_number, order_status, order_total, is_completed, completed_amount, order_amount_due, order_is_paid_in_full

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010-pmt-1 | order_date | 2025-10-05 | 2025-10-05 04:00:00-05:00 |
| alice-johnson-1010-pmt-1 | order_amount_due | 0.0 | 0.00 |
| alice-johnson-1010-pmt-2 | order_date | 2025-10-05 | 2025-10-05 04:00:00-05:00 |
| alice-johnson-1010-pmt-2 | order_amount_due | 0.0 | 0.00 |
| bobby-1001-pmt-1 | order_date | 2025-09-15 | 2025-09-15 05:30:00-05:00 |
| bobby-1001-pmt-1 | order_amount_due | 0.0 | 0.00 |
| bobby-1042-pmt-1 | order_date | 2026-02-20 | 2026-02-20 07:15:00-06:00 |
| brian-lee-1018-pmt-1 | order_date | 2025-11-12 | 2025-11-12 09:45:00-06:00 |
| brian-lee-1018-pmt-1 | order_amount_due | 0.0 | 0.00 |
| carla-smith-1024-pmt-1 | order_date | 2025-12-01 | 2025-12-01 05:20:00-06:00 |
| carla-smith-1037-pmt-1 | order_date | 2026-01-28 | 2026-01-28 08:00:00-06:00 |
| carla-smith-1037-pmt-1 | order_amount_due | 0.0 | 0.00 |
| caroline-1045-pmt-1 | order_date | 2026-03-12 | 2026-03-12 05:00:00-05:00 |
| caroline-1045-pmt-1 | order_amount_due | 0.0 | 0.00 |
| caroline-1045-pmt-2 | order_date | 2026-03-12 | 2026-03-12 05:00:00-05:00 |
| caroline-1045-pmt-2 | order_amount_due | 0.0 | 0.00 |
