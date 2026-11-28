# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 399 |
| Passed | 399 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 205 | 205 | 100.0% |
| Lookup (INDEX/MATCH) | 124 | 124 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 70 | 70 | 100.0% |

## Results by Entity

### symmetry_groups

- Fields: 51/51 (100.0%)
- Computed columns: name, tiling_count, wallpaper_design_count

### prototiles

- Fields: 48/48 (100.0%)
- Computed columns: name, area, interior_angle_deg, vertex_angle_turns, is_regular, is_triangle, is_square, used_in_tilings_count

### tilings

- Fields: 77/77 (100.0%)
- Computed columns: name, symmetry_notation, distinct_prototile_count, vertex_figure_count, valid_vertex_figure_count, is_regular_tiling, all_vertices_valid, lattice_determinant, fundamental_domain_area, has_lattice, does_tile_plane

### tiling_prototiles

- Fields: 33/33 (100.0%)
- Computed columns: name, prototile_sides, prototile_interior_angle

### vertex_figures

- Fields: 28/28 (100.0%)
- Computed columns: name, turns_at_vertex, angle_gap_deg, is_valid

### regions

- Fields: 24/24 (100.0%)
- Computed columns: name, area, placement_count, covered_area, overlap_count, coverage_pct, is_fully_covered, is_clean_tiling

### placements

- Fields: 120/120 (100.0%)
- Computed columns: name, tile_area, tile_sides, region_width, region_height, is_inside_region

### wallpaper_designs

- Fields: 18/18 (100.0%)
- Computed columns: name, group_notation, group_orbifold, group_lattice_type, group_rotation_order, group_operation_count
