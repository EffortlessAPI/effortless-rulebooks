# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 69 |
| Passed | 69 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 35 | 35 | 100.0% |
| Lookup (INDEX/MATCH) | 30 | 30 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 4 | 4 | 100.0% |

## Results by Entity

### transaction_types

- Fields: 3/3 (100.0%)
- Computed columns: name

### products

- Fields: 16/16 (100.0%)
- Computed columns: name, current_quantity, is_low_stock, reorder_status

### transactions

- Fields: 50/50 (100.0%)
- Computed columns: name, product_name, product_unit_price, transaction_type_name, amount
