# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 698 |
| Passed | 698 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 242 | 242 | 100.0% |
| Lookup (INDEX/MATCH) | 295 | 295 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 161 | 161 | 100.0% |

## Results by Entity

### users

- Fields: 56/56 (100.0%)
- Computed columns: wind_count, cook_count, completed_cook_count, activity_level, total_recipe_minutes_attempted, avg_recipe_minutes_attempted, cooking_style

### timers

- Fields: 72/72 (100.0%)
- Computed columns: wind_fraction, stored_torque_nm, remaining_minutes, wind_state, is_armed, has_rung, tick_count, ring_count, cook_count, total_recipe_minutes_used, avg_recipe_minutes_used, wind_duration_minutes, expected_ticks_per_minute, expected_total_ticks, ticks_elapsed_expected, tick_efficiency_percent, has_sparse_tick_record, mechanical_phase

### bells

- Fields: 8/8 (100.0%)
- Computed columns: times_rung, has_ever_rung

### winding_knobs

- Fields: 12/12 (100.0%)
- Computed columns: timer_wind_angle, timer_max_wind, scale_reading

### gear_trains

- Fields: 20/20 (100.0%)
- Computed columns: timer_escapement_hz, output_shaft_rpm, gear_count, total_teeth, average_tooth_count

### gears

- Fields: 48/48 (100.0%)
- Computed columns: is_first_in_chain, is_last_in_chain, size_class, on_timer

### output_shafts

- Fields: 4/4 (100.0%)
- Computed columns: current_rpm

### escapements

- Fields: 8/8 (100.0%)
- Computed columns: tick_count, has_ticked

### bell_hammers

- Fields: 8/8 (100.0%)
- Computed columns: timer_has_rung, release_state

### hammer_catches

- Fields: 8/8 (100.0%)
- Computed columns: hammer_release_state, is_engaged

### wind_actions

- Fields: 10/10 (100.0%)
- Computed columns: applied_to_timer, resulted_in_ring

### tick_events

- Fields: 69/69 (100.0%)
- Computed columns: timer_label, on_timer_that_has_rung, on_timer_in_phase

### cooks

- Fields: 234/234 (100.0%)
- Computed columns: timer_has_rung, timer_is_armed, timer_remaining_min, status, recommended_minutes, timer_wind_duration, timer_can_cover_recipe, suitability_verdict, prepared_by_name, prepared_by_activity_level, recipe_duration_category, recipe_temp_profile, timer_mechanical_phase

### recipes

- Fields: 114/114 (100.0%)
- Computed columns: times_used_today, duration_category, cooking_temp_profile, avg_timer_capacity, total_timer_capacity, assignment_verdict

### kitchens

- Fields: 27/27 (100.0%)
- Computed columns: total_timers, total_cases, total_bells, total_users, total_cooks, total_recipes, total_wind_actions, total_tick_events, total_ring_events, timers_freshly_wound, timers_mid_run, timers_nearly_done, timers_rung, cooks_done, cooks_cooking, recipes_unused, recipes_under_provisioned, recipes_well_matched, busy_users, idle_users, total_recipe_minutes, avg_recipe_minutes, min_recipe_minutes, max_recipe_minutes, has_any_timer_rung, has_misprovisioned_recipe, operational_status
