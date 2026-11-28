# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 734 |
| Passed | 117 |
| Failed | 617 |
| Score | 15.9% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 41 | 291 | 14.1% |
| Lookup (INDEX/MATCH) | 63 | 360 | 17.5% |
| Aggregation (COUNTIFS/SUMIFS) | 13 | 83 | 15.7% |

## Error

```
Script failed: 
```

## Results by Entity

### app_users

- Fields: 0/3 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| customer-ssot-me-customer | name | customer-ssot.me-customer | None |
| ej-ssot-me-admin | name | ej-ssot.me-admin | None |
| manager-ssot-me-manager | name | manager-ssot.me-manager | None |

### clients

- Fields: 18/70 (25.7%)
- Computed columns: category_name, category_discount, is_stopped, status_display_name, status_description, count_of_big_invoices, billing_address_state_tax_rate, billing_address, shipping_address, average_order_value, is_vip, has_recent_invoices, last_invoice, customer_since_days

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson | category_name | Active | None |
| alice-johnson | category_discount | 0.05 | None |
| alice-johnson | is_stopped | True | None |
| alice-johnson | status_display_name | Pending | None |
| alice-johnson | status_description | This status shows the customer | None |
| alice-johnson | count_of_big_invoices | 1 | None |
| alice-johnson | billing_address_state_tax_rate | 0.065 | None |
| alice-johnson | billing_address | 500 Pine St, Suite 200, Seattl | None |
| alice-johnson | average_order_value | 5412.103687499999 | None |
| alice-johnson | last_invoice | 2025-10-05T09:00:00Z | None |
| alice-johnson | customer_since_days | 266 | None |
| bob | category_name | Active | None |
| bob | category_discount | 0.05 | None |
| bob | is_stopped | True | None |
| bob | status_display_name | On-Hold | None |
| bob | status_description | This status indicates the cust | None |
| bob | billing_address_state_tax_rate | 0.0625 | None |
| bob | billing_address | 742 Evergreen Terrace, Springf | None |
| bob | average_order_value | 150.3565 | None |
| bob | last_invoice | 2025-09-15T10:30:00Z | None |
| ... | ... | (32 more) | ... |

### client_approvals

- Fields: 5/30 (16.7%)
- Computed columns: name, approved_by_contact_name, approved_by_email_address, approved_by_phone_number, approved_by_role, is_approved, client_name, client_email, client_phone, client_category

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-apvd | name | Alice Johnson-APVD | None |
| alice-johnson-apvd | approved_by_contact_name | EJ Alexandra | None |
| alice-johnson-apvd | approved_by_email_address | ej@ssot.me | None |
| alice-johnson-apvd | approved_by_phone_number | (555) 555-1111 | None |
| alice-johnson-apvd | approved_by_role | Admin | None |
| alice-johnson-apvd | is_approved | True | None |
| alice-johnson-apvd | client_name | Alice Johnson | None |
| alice-johnson-apvd | client_email | alice.johnson@northwind.exampl | None |
| alice-johnson-apvd | client_phone | +1-206-555-0142 | None |
| alice-johnson-apvd | client_category | active | None |
| brian-lee-apvd | name | Brian Lee-APVD | None |
| brian-lee-apvd | approved_by_contact_name | EJ Alexandra | None |
| brian-lee-apvd | approved_by_email_address | ej@ssot.me | None |
| brian-lee-apvd | approved_by_phone_number | (555) 555-1111 | None |
| brian-lee-apvd | approved_by_role | Admin | None |
| brian-lee-apvd | is_approved | True | None |
| brian-lee-apvd | client_name | Brian Lee | None |
| brian-lee-apvd | client_email | brian.lee@contoso.example | None |
| brian-lee-apvd | client_phone | +1-512-555-0177 | None |
| brian-lee-apvd | client_category | prospect | None |
| ... | ... | (5 more) | ... |

### client_categories

- Fields: 0/3 (0.0%)
- Computed columns: count_of_clients

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| active | count_of_clients | 3 | None |
| gold | count_of_clients | 1 | None |
| prospect | count_of_clients | 1 | None |

### statuses

- Fields: 3/14 (21.4%)
- Computed columns: name, count_of_clients

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cancelled | name | cancelled | None |
| delayed | name | delayed | None |
| delayed | count_of_clients | 2 | None |
| in-review | name | in-review | None |
| new | name | new | None |
| new | count_of_clients | 1 | None |
| on-hold | name | on-hold | None |
| on-hold | count_of_clients | 1 | None |
| pending | name | pending | None |
| pending | count_of_clients | 1 | None |
| processing | name | processing | None |

### products

- Fields: 18/64 (28.1%)
- Computed columns: name, profit, margin, is_high_margin, stock_quantity, cogs, count_of_vip_orders, has_been_ordered_by_vip_customers

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cable-usb-c | name | cable-usb-c | None |
| cable-usb-c | profit | 8.6 | None |
| cable-usb-c | margin | 0.7166666666666667 | None |
| cable-usb-c | is_high_margin | True | None |
| cable-usb-c | stock_quantity | 1240 | None |
| cable-usb-c | cogs | 4216 | None |
| cable-usb-c | count_of_vip_orders | 1 | None |
| cable-usb-c | has_been_ordered_by_vip_customers | True | None |
| case-leather | name | case-leather | None |
| case-leather | profit | 42.5 | None |
| case-leather | margin | 0.6538461538461539 | None |
| case-leather | is_high_margin | True | None |
| gadget-100 | name | gadget-100 | None |
| gadget-100 | profit | 15.4 | None |
| gadget-100 | margin | 0.6285714285714286 | None |
| gadget-100 | stock_quantity | 612 | None |
| gadget-100 | cogs | 5569.2 | None |
| gadget-200 | name | gadget-200 | None |
| gadget-200 | profit | 57 | None |
| gadget-200 | margin | 0.6404494382022472 | None |
| ... | ... | (26 more) | ... |

### invoices

- Fields: 20/147 (13.6%)
- Computed columns: name, client_is_vip, client_hast_recent_invoices, client_email, client_phone, client_company_name, client_category_name, client_category_discount, item_count, total_quantity, sub_total, is_big_order, tax_rate, tax_amount, invoice_total, total_paid, amount_due, is_paid_in_full, payment_count, last_payment_date, payment_status_label

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010 | name | alice-johnson-1010 | None |
| alice-johnson-1010 | client_email | alice.johnson@northwind.exampl | None |
| alice-johnson-1010 | client_phone | +1-206-555-0142 | None |
| alice-johnson-1010 | client_company_name | Northwind Traders | None |
| alice-johnson-1010 | client_category_name | Active | None |
| alice-johnson-1010 | client_category_discount | 0.05 | None |
| alice-johnson-1010 | item_count | 4 | None |
| alice-johnson-1010 | total_quantity | 250 | None |
| alice-johnson-1010 | sub_total | 5081.787499999999 | None |
| alice-johnson-1010 | is_big_order | True | None |
| alice-johnson-1010 | tax_rate | 0.065 | None |
| alice-johnson-1010 | tax_amount | 330.31618749999996 | None |
| alice-johnson-1010 | invoice_total | 5412.103687499999 | None |
| alice-johnson-1010 | total_paid | 4770.4400000000005 | None |
| alice-johnson-1010 | amount_due | 641.66 | None |
| alice-johnson-1010 | payment_count | 2 | None |
| alice-johnson-1010 | last_payment_date | 2025-10-12 | None |
| alice-johnson-1010 | payment_status_label | Partial | None |
| bob-1001 | name | bob-1001 | None |
| bob-1001 | client_email | bob@launchpad.example | None |
| ... | ... | (107 more) | ... |

### invoice_line_items

- Fields: 35/234 (15.0%)
- Computed columns: name, client_is_vip, client_hast_recent_invoices, invoice_number, invoice_total, invoice_total_paid, product_sku, product_display_name, unit_price, pre_discount, discount_percent, discount_amount, sub_total

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010-line-1 | name | alice-johnson-1010-line-1 | None |
| alice-johnson-1010-line-1 | invoice_number | 1010 | None |
| alice-johnson-1010-line-1 | invoice_total | 5412.103687499999 | None |
| alice-johnson-1010-line-1 | invoice_total_paid | 4770.4400000000005 | None |
| alice-johnson-1010-line-1 | product_sku | WIDGET-001 | None |
| alice-johnson-1010-line-1 | product_display_name | Standard Widget | None |
| alice-johnson-1010-line-1 | unit_price | 19.99 | None |
| alice-johnson-1010-line-1 | pre_discount | 999.4999999999999 | None |
| alice-johnson-1010-line-1 | discount_percent | 0.05 | None |
| alice-johnson-1010-line-1 | discount_amount | 49.974999999999994 | None |
| alice-johnson-1010-line-1 | sub_total | 949.5249999999999 | None |
| alice-johnson-1010-line-2 | name | alice-johnson-1010-line-2 | None |
| alice-johnson-1010-line-2 | invoice_number | 1010 | None |
| alice-johnson-1010-line-2 | invoice_total | 5412.103687499999 | None |
| alice-johnson-1010-line-2 | invoice_total_paid | 4770.4400000000005 | None |
| alice-johnson-1010-line-2 | product_sku | WIDGET-002 | None |
| alice-johnson-1010-line-2 | product_display_name | Deluxe Widget | None |
| alice-johnson-1010-line-2 | unit_price | 39.99 | None |
| alice-johnson-1010-line-2 | pre_discount | 999.75 | None |
| alice-johnson-1010-line-2 | discount_percent | 0.05 | None |
| ... | ... | (179 more) | ... |

### inventory_adjustments

- Fields: 0/8 (0.0%)
- Computed columns: adjustment_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| cable-usb-c-2026-04-22-qty1240 | adjustment_name | cable-usb-c-2026-04-22-qty1240 | None |
| case-leather-2026-04-08-qty0 | adjustment_name | case-leather-2026-04-08-qty0 | None |
| gadget-100-2026-04-09-qty612 | adjustment_name | gadget-100-2026-04-09-qty612 | None |
| gadget-200-2026-04-06-qty75 | adjustment_name | gadget-200-2026-04-06-qty75 | None |
| hub-7port-2026-04-25-qty240 | adjustment_name | hub-7port-2026-04-25-qty240 | None |
| stand-alu-2026-04-16-qty92 | adjustment_name | stand-alu-2026-04-16-qty92 | None |
| widget-001-2026-04-01-qty425 | adjustment_name | widget-001-2026-04-01-qty425 | None |
| widget-002-2026-04-01-qty180 | adjustment_name | widget-002-2026-04-01-qty180 | None |

### payments

- Fields: 12/81 (14.8%)
- Computed columns: name, invoice_date, invoice_number, invoice_status, invoice_total, is_completed, completed_amount, order_amount_due, order_is_paid_in_full

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| alice-johnson-1010-pmt-1 | name | alice-johnson-1010-pmt-1 | None |
| alice-johnson-1010-pmt-1 | invoice_date | 2025-10-05 | None |
| alice-johnson-1010-pmt-1 | invoice_number | 1010 | None |
| alice-johnson-1010-pmt-1 | invoice_status | Shipped | None |
| alice-johnson-1010-pmt-1 | invoice_total | 5412.103687499999 | None |
| alice-johnson-1010-pmt-1 | is_completed | True | None |
| alice-johnson-1010-pmt-1 | completed_amount | 1500 | None |
| alice-johnson-1010-pmt-1 | order_amount_due | 641.66 | None |
| alice-johnson-1010-pmt-2 | name | alice-johnson-1010-pmt-2 | None |
| alice-johnson-1010-pmt-2 | invoice_date | 2025-10-05 | None |
| alice-johnson-1010-pmt-2 | invoice_number | 1010 | None |
| alice-johnson-1010-pmt-2 | invoice_status | Shipped | None |
| alice-johnson-1010-pmt-2 | invoice_total | 5412.103687499999 | None |
| alice-johnson-1010-pmt-2 | is_completed | True | None |
| alice-johnson-1010-pmt-2 | completed_amount | 3270.44 | None |
| alice-johnson-1010-pmt-2 | order_amount_due | 641.66 | None |
| bob-1001-pmt-1 | name | bob-1001-pmt-1 | None |
| bob-1001-pmt-1 | invoice_date | 2025-09-15 | None |
| bob-1001-pmt-1 | invoice_number | 1001 | None |
| bob-1001-pmt-1 | invoice_status | Delivered | None |
| ... | ... | (49 more) | ... |

### addresses

- Fields: 6/30 (20.0%)
- Computed columns: name, type_of_address_is_shipping_address, type_of_address_is_billing_address, state_code, state_tax_rate

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1200-ocean-dr-miami-fl-florida-33139 | name | 1200 Ocean Dr, Miami, fl-flori | None |
| 1200-ocean-dr-miami-fl-florida-33139 | type_of_address_is_billing_address | True | None |
| 1200-ocean-dr-miami-fl-florida-33139 | state_code | FL | None |
| 1200-ocean-dr-miami-fl-florida-33139 | state_tax_rate | 0.06 | None |
| 23-hudson-lane-brooklyn-ny-new-york-11201 | name | 23 Hudson Lane, Brooklyn, ny-n | None |
| 23-hudson-lane-brooklyn-ny-new-york-11201 | type_of_address_is_billing_address | True | None |
| 23-hudson-lane-brooklyn-ny-new-york-11201 | state_code | NY | None |
| 23-hudson-lane-brooklyn-ny-new-york-11201 | state_tax_rate | 0.04 | None |
| 500-pine-st-suite-200-seattle-wa-washington-98101 | name | 500 Pine St, Suite 200, Seattl | None |
| 500-pine-st-suite-200-seattle-wa-washington-98101 | type_of_address_is_billing_address | True | None |
| 500-pine-st-suite-200-seattle-wa-washington-98101 | state_code | WA | None |
| 500-pine-st-suite-200-seattle-wa-washington-98101 | state_tax_rate | 0.065 | None |
| 742-evergreen-terrace-springfield-il-illinois-97477 | name | 742 Evergreen Terrace, Springf | None |
| 742-evergreen-terrace-springfield-il-illinois-97477 | type_of_address_is_billing_address | True | None |
| 742-evergreen-terrace-springfield-il-illinois-97477 | state_code | IL | None |
| 742-evergreen-terrace-springfield-il-illinois-97477 | state_tax_rate | 0.0625 | None |
| 88-industrial-way-austin-tx-texas-78701 | name | 88 Industrial Way, Austin, tx- | None |
| 88-industrial-way-austin-tx-texas-78701 | type_of_address_is_billing_address | True | None |
| 88-industrial-way-austin-tx-texas-78701 | state_code | TX | None |
| 88-industrial-way-austin-tx-texas-78701 | state_tax_rate | 0.0625 | None |
| ... | ... | (4 more) | ... |

### states

- Fields: 0/50 (0.0%)
- Computed columns: name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| ak-alaska | name | ak-alaska | None |
| al-alabama | name | al-alabama | None |
| ar-arkansas | name | ar-arkansas | None |
| az-arizona | name | az-arizona | None |
| ca-california | name | ca-california | None |
| co-colorado | name | co-colorado | None |
| ct-connecticut | name | ct-connecticut | None |
| de-delaware | name | de-delaware | None |
| fl-florida | name | fl-florida | None |
| ga-georgia | name | ga-georgia | None |
| hi-hawaii | name | hi-hawaii | None |
| ia-iowa | name | ia-iowa | None |
| id-idaho | name | id-idaho | None |
| il-illinois | name | il-illinois | None |
| in-indiana | name | in-indiana | None |
| ks-kansas | name | ks-kansas | None |
| ky-kentucky | name | ky-kentucky | None |
| la-louisiana | name | la-louisiana | None |
| ma-massachusetts | name | ma-massachusetts | None |
| md-maryland | name | md-maryland | None |
| ... | ... | (30 more) | ... |
