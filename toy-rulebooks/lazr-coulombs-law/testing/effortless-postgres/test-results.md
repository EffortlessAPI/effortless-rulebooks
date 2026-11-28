# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 300 |
| Passed | 300 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 237 | 237 | 100.0% |
| Lookup (INDEX/MATCH) | 17 | 17 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 46 | 46 | 100.0% |

## Results by Entity

### loops

- Fields: 4/4 (100.0%)
- Computed columns: name

### particles

- Fields: 6/6 (100.0%)
- Computed columns: charge_sign

### charges

- Fields: 35/35 (100.0%)
- Computed columns: charge_sign, charge_magnitude, system_name, particle_name, interaction_count

### systems

- Fields: 21/21 (100.0%)
- Computed columns: charge_count, interaction_count, attractive_count, repulsive_count, total_charge, is_neutral, max_force_magnitude

### charge_interactions

- Fields: 65/65 (100.0%)
- Computed columns: name, delta_x, delta_y, delta_z, distance_squared, distance_meters, distance_picometers, charge_product, force_magnitude, is_attractive, is_repulsive, interaction_type, force_in_atto_newtons

### force_vectors

- Fields: 45/45 (100.0%)
- Computed columns: name, unit_x, unit_y, unit_z, sign_factor, fx, fy, fz, force_dir_description

### electric_field_points

- Fields: 77/77 (100.0%)
- Computed columns: name, d_x, d_y, d_z, distance_squared, distance, e_field_magnitude, q_sign, ex, ey, ez

### invariant_checks

- Fields: 8/8 (100.0%)
- Computed columns: name

### system_summary

- Fields: 39/39 (100.0%)
- Computed columns: name, system_name, charge_count, interaction_count, attractive_count, repulsive_count, total_charge, is_neutral, max_force_magnitude, min_distance_meters, all_attractive, all_repulsive, has_mixed_interactions
