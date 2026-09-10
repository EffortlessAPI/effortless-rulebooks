# Test Results: xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 391 |
| Passed | 66 |
| Failed | 325 |
| Score | 16.9% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 36 | 138 | 26.1% |
| Lookup (INDEX/MATCH) | 14 | 181 | 7.7% |
| Aggregation (COUNTIFS/SUMIFS) | 16 | 72 | 22.2% |

## Results by Entity

### users

- Fields: 0/4 (0.0%)
- Computed columns: name, full_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice@example.com | name | alice@example.com | None |
| alice@example.com | full_name | Anderson, Alice | None |
| bob@example.com | name | bob@example.com | None |
| bob@example.com | full_name | Baker, Bob | None |

### customers

- Fields: 6/30 (20.0%)
- Computed columns: name, full_name, lifetime_sales, unpaid_order_count, large_order_count, is_vip

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ada.lovelace@analytical.engine | name | ada.lovelace@analytical.engine | None |
| ada.lovelace@analytical.engine | full_name | Lovelace, Ada | None |
| ada.lovelace@analytical.engine | lifetime_sales | 50.0 | None |
| ada.lovelace@analytical.engine | large_order_count | 1.0 | None |
| alan.turing@bletchley.uk | name | alan.turing@bletchley.uk | None |
| alan.turing@bletchley.uk | full_name | Turing, Alan | None |
| alan.turing@bletchley.uk | lifetime_sales | 99.0 | None |
| grace.hopper@nautical.mil | name | grace.hopper@nautical.mil | None |
| grace.hopper@nautical.mil | full_name | Hopper, Grace | None |
| grace.hopper@nautical.mil | lifetime_sales | 320.5 | None |
| grace.hopper@nautical.mil | unpaid_order_count | 2 | None |
| grace.hopper@nautical.mil | large_order_count | 1.0 | None |
| katherine.johnson@nasa.gov | name | katherine.johnson@nasa.gov | None |
| katherine.johnson@nasa.gov | full_name | Johnson, Katherine | None |
| katherine.johnson@nasa.gov | lifetime_sales | 1152.0 | None |
| katherine.johnson@nasa.gov | unpaid_order_count | 1 | None |
| katherine.johnson@nasa.gov | large_order_count | 2.0 | None |
| katherine.johnson@nasa.gov | is_vip | True | None |
| linus.torvalds@kernel.org | name | linus.torvalds@kernel.org | None |
| linus.torvalds@kernel.org | full_name | Torvalds, Linus | None |
| ... | ... | (4 more) | ... |

### orders

- Fields: 35/130 (26.9%)
- Computed columns: name, customer_first_name, customer_last_name, customer_full_name, customer_email, amount_paid, balance, is_paid, unpaid_flag, is_recent, recent_total, fcs_subtotal, fcs_unit_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ORD-1001 | name | ORD-1001 | None |
| ORD-1001 | customer_first_name | Ada | None |
| ORD-1001 | customer_last_name | Lovelace | None |
| ORD-1001 | customer_full_name | Lovelace, Ada | None |
| ORD-1001 | customer_email | ada.lovelace@analytical.engine | None |
| ORD-1001 | amount_paid | 250.0 | None |
| ORD-1001 | is_paid | True | None |
| ORD-1002 | name | ORD-1002 | None |
| ORD-1002 | customer_first_name | Ada | None |
| ORD-1002 | customer_last_name | Lovelace | None |
| ORD-1002 | customer_full_name | Lovelace, Ada | None |
| ORD-1002 | customer_email | ada.lovelace@analytical.engine | None |
| ORD-1002 | amount_paid | 875.0 | None |
| ORD-1002 | is_paid | True | None |
| ORD-1003 | name | ORD-1003 | None |
| ORD-1003 | customer_first_name | Grace | None |
| ORD-1003 | customer_last_name | Hopper | None |
| ORD-1003 | customer_full_name | Hopper, Grace | None |
| ORD-1003 | customer_email | grace.hopper@nautical.mil | None |
| ORD-1003 | amount_paid | 1000.0 | None |
| ... | ... | (75 more) | ... |

### payments

- Fields: 9/90 (10.0%)
- Computed columns: name, order_number, order_date, order_total, order_amount_paid, order_balance, order_is_paid, order_customer, customer_full_name, customer_email

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| PMT-2001 | name | PMT-2001 | None |
| PMT-2001 | order_number | ORD-1001 | None |
| PMT-2001 | order_date | 2026-01-15 | None |
| PMT-2001 | order_total | 250.0 | None |
| PMT-2001 | order_amount_paid | 250.0 | None |
| PMT-2001 | order_is_paid | True | None |
| PMT-2001 | order_customer | ada.lovelace@analytical.engine | None |
| PMT-2001 | customer_full_name | Lovelace, Ada | None |
| PMT-2001 | customer_email | ada.lovelace@analytical.engine | None |
| PMT-2002 | name | PMT-2002 | None |
| PMT-2002 | order_number | ORD-1002 | None |
| PMT-2002 | order_date | 2026-02-03 | None |
| PMT-2002 | order_total | 875.0 | None |
| PMT-2002 | order_amount_paid | 875.0 | None |
| PMT-2002 | order_is_paid | True | None |
| PMT-2002 | order_customer | ada.lovelace@analytical.engine | None |
| PMT-2002 | customer_full_name | Lovelace, Ada | None |
| PMT-2002 | customer_email | ada.lovelace@analytical.engine | None |
| PMT-2003 | name | PMT-2003 | None |
| PMT-2003 | order_number | ORD-1002 | None |
| ... | ... | (61 more) | ... |

### jet_models

- Fields: 3/25 (12.0%)
- Computed columns: name, is_fifth_gen, fcs_variant_count, total_units_ordered, total_revenue

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| EF-2000 | name | EF-2000 | None |
| EF-2000 | fcs_variant_count | 1.0 | None |
| EF-2000 | total_units_ordered | 5 | None |
| EF-2000 | total_revenue | 46.5 | None |
| F-16C | name | F-16C | None |
| F-16C | fcs_variant_count | 2.0 | None |
| F-16C | total_units_ordered | 9 | None |
| F-16C | total_revenue | 33.6 | None |
| F-18E | name | F-18E | None |
| F-18E | fcs_variant_count | 1.0 | None |
| F-18E | total_units_ordered | 8 | None |
| F-18E | total_revenue | 46.4 | None |
| F-22A | name | F-22A | None |
| F-22A | is_fifth_gen | True | None |
| F-22A | fcs_variant_count | 1.0 | None |
| F-22A | total_units_ordered | 2 | None |
| F-22A | total_revenue | 37.0 | None |
| F-35A | name | F-35A | None |
| F-35A | is_fifth_gen | True | None |
| F-35A | fcs_variant_count | 1.0 | None |
| ... | ... | (2 more) | ... |

### flight_control_systems

- Fields: 8/48 (16.7%)
- Computed columns: name, jet_manufacturer, jet_generation, is_triple_redundant, is_quad_redundant, meets_fifth_gen_spec, total_units_ordered, total_revenue

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| FCS-EF2K-FBW4 | name | FCS-EF2K-FBW4 | None |
| FCS-EF2K-FBW4 | jet_manufacturer | Eurofighter | None |
| FCS-EF2K-FBW4 | jet_generation | 4 | None |
| FCS-EF2K-FBW4 | is_triple_redundant | True | None |
| FCS-EF2K-FBW4 | is_quad_redundant | True | None |
| FCS-EF2K-FBW4 | total_units_ordered | 5 | None |
| FCS-EF2K-FBW4 | total_revenue | 46.5 | None |
| FCS-F16-FBW3 | name | FCS-F16-FBW3 | None |
| FCS-F16-FBW3 | jet_manufacturer | Lockheed Martin | None |
| FCS-F16-FBW3 | jet_generation | 4 | None |
| FCS-F16-FBW3 | is_triple_redundant | True | None |
| FCS-F16-FBW3 | total_units_ordered | 7 | None |
| FCS-F16-FBW3 | total_revenue | 29.4 | None |
| FCS-F16-HYB2 | name | FCS-F16-HYB2 | None |
| FCS-F16-HYB2 | jet_manufacturer | Lockheed Martin | None |
| FCS-F16-HYB2 | jet_generation | 4 | None |
| FCS-F16-HYB2 | total_units_ordered | 2 | None |
| FCS-F16-HYB2 | total_revenue | 4.2 | None |
| FCS-F18-FBW3 | name | FCS-F18-FBW3 | None |
| FCS-F18-FBW3 | jet_manufacturer | Boeing | None |
| ... | ... | (20 more) | ... |

### order_lines

- Fields: 5/64 (7.8%)
- Computed columns: name, fcs_unit_price, fcs_architecture, fcs_jet_model, fcs_meets_fifth_gen_spec, order_number, order_customer, line_total

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| OL-3001 | name | OL-3001 | None |
| OL-3001 | fcs_unit_price | 4.2 | None |
| OL-3001 | fcs_architecture | fly-by-wire | None |
| OL-3001 | fcs_jet_model | F-16C | None |
| OL-3001 | order_number | ORD-1003 | None |
| OL-3001 | order_customer | grace.hopper@nautical.mil | None |
| OL-3001 | line_total | 25.2 | None |
| OL-3002 | name | OL-3002 | None |
| OL-3002 | fcs_unit_price | 2.1 | None |
| OL-3002 | fcs_architecture | analog-hybrid | None |
| OL-3002 | fcs_jet_model | F-16C | None |
| OL-3002 | order_number | ORD-1003 | None |
| OL-3002 | order_customer | grace.hopper@nautical.mil | None |
| OL-3002 | line_total | 4.2 | None |
| OL-3003 | name | OL-3003 | None |
| OL-3003 | fcs_unit_price | 12.0 | None |
| OL-3003 | fcs_architecture | fly-by-wire | None |
| OL-3003 | fcs_jet_model | F-35A | None |
| OL-3003 | fcs_meets_fifth_gen_spec | True | None |
| OL-3003 | order_number | ORD-1006 | None |
| ... | ... | (39 more) | ... |
