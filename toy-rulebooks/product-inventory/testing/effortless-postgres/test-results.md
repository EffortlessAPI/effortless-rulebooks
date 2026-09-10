# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 69 |
| Passed | 17 |
| Failed | 52 |
| Score | 24.6% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 17 | 35 | 48.6% |
| Lookup (INDEX/MATCH) | 0 | 30 | 0.0% |
| Aggregation (COUNTIFS/SUMIFS) | 0 | 4 | 0.0% |

## Results by Entity

### transaction_types

- Fields: 3/3 (100.0%)
- Computed columns: name

### products

- Fields: 4/16 (25.0%)
- Computed columns: name, current_quantity, is_low_stock, reorder_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| GADGET-B | current_quantity | None | 15 |
| GADGET-B | is_low_stock | False | True |
| GADGET-B | reorder_status | In Stock | This needs to be reordered IMM |
| PART-C | current_quantity | None | 70 |
| PART-C | is_low_stock | False | True |
| PART-C | reorder_status | In Stock | This needs to be reordered IMM |
| TOOL-D | current_quantity | None | 5 |
| TOOL-D | is_low_stock | False | True |
| TOOL-D | reorder_status | In Stock | This needs to be reordered IMM |
| WIDGET-A | current_quantity | None | 45 |
| WIDGET-A | is_low_stock | False | True |
| WIDGET-A | reorder_status | In Stock | This needs to be reordered IMM |

### transactions

- Fields: 10/50 (20.0%)
- Computed columns: name, product_name, product_unit_price, transaction_type_name, amount

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| TXN-001 | product_name | None | WIDGET-A |
| TXN-001 | product_unit_price | None | 12.99 |
| TXN-001 | transaction_type_name | None | PURCHASE |
| TXN-001 | amount | 0 | 1299.00 |
| TXN-002 | product_name | None | WIDGET-A |
| TXN-002 | product_unit_price | None | 12.99 |
| TXN-002 | transaction_type_name | None | SALE |
| TXN-002 | amount | 0 | -324.75 |
| TXN-003 | product_name | None | WIDGET-A |
| TXN-003 | product_unit_price | None | 12.99 |
| TXN-003 | transaction_type_name | None | SALE |
| TXN-003 | amount | 0 | -389.70 |
| TXN-004 | product_name | None | GADGET-B |
| TXN-004 | product_unit_price | None | 24.5 |
| TXN-004 | transaction_type_name | None | PURCHASE |
| TXN-004 | amount | 0 | 1225.0 |
| TXN-005 | product_name | None | GADGET-B |
| TXN-005 | product_unit_price | None | 24.5 |
| TXN-005 | transaction_type_name | None | SALE |
| TXN-005 | amount | 0 | -857.5 |
| ... | ... | (20 more) | ... |
