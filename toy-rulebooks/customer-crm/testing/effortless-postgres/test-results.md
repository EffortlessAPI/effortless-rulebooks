# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 391 |
| Passed | 370 |
| Failed | 21 |
| Score | 94.6% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 122 | 138 | 88.4% |
| Lookup (INDEX/MATCH) | 181 | 181 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 67 | 72 | 93.1% |

## Results by Entity

### users

- Fields: 4/4 (100.0%)
- Computed columns: name, full_name

### customers

- Fields: 23/30 (76.7%)
- Computed columns: name, full_name, lifetime_sales, unpaid_order_count, large_order_count, is_vip

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ada.lovelace@analytical.engine | lifetime_sales | 50.0 | 0 |
| alan.turing@bletchley.uk | lifetime_sales | 99.0 | 0 |
| grace.hopper@nautical.mil | lifetime_sales | 320.5 | 0 |
| katherine.johnson@nasa.gov | lifetime_sales | 1152.0 | 0 |
| katherine.johnson@nasa.gov | is_vip | True | False |
| linus.torvalds@kernel.org | lifetime_sales | 2580.0 | 0 |
| linus.torvalds@kernel.org | is_vip | True | False |

### orders

- Fields: 116/130 (89.2%)
- Computed columns: name, customer_first_name, customer_last_name, customer_full_name, customer_email, amount_paid, balance, is_paid, unpaid_flag, is_recent, recent_total, fcs_subtotal, fcs_unit_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ORD-1004 | is_recent | True | False |
| ORD-1004 | recent_total | 320.5 | 0 |
| ORD-1005 | is_recent | True | False |
| ORD-1005 | recent_total | 99.0 | 0 |
| ORD-1006 | is_recent | True | False |
| ORD-1006 | recent_total | 540.0 | 0 |
| ORD-1007 | is_recent | True | False |
| ORD-1007 | recent_total | 612.0 | 0 |
| ORD-1008 | is_recent | True | False |
| ORD-1008 | recent_total | 2400.0 | 0 |
| ORD-1009 | is_recent | True | False |
| ORD-1009 | recent_total | 180.0 | 0 |
| ORD-1010 | is_recent | True | False |
| ORD-1010 | recent_total | 50.0 | 0 |

### payments

- Fields: 90/90 (100.0%)
- Computed columns: name, order_number, order_date, order_total, order_amount_paid, order_balance, order_is_paid, order_customer, customer_full_name, customer_email

### jet_models

- Fields: 25/25 (100.0%)
- Computed columns: name, is_fifth_gen, fcs_variant_count, total_units_ordered, total_revenue

### flight_control_systems

- Fields: 48/48 (100.0%)
- Computed columns: name, jet_manufacturer, jet_generation, is_triple_redundant, is_quad_redundant, meets_fifth_gen_spec, total_units_ordered, total_revenue

### order_lines

- Fields: 64/64 (100.0%)
- Computed columns: name, fcs_unit_price, fcs_architecture, fcs_jet_model, fcs_meets_fifth_gen_spec, order_number, order_customer, line_total
