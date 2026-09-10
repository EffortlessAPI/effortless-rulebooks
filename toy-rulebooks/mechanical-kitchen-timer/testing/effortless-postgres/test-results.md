# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 698 |
| Passed | 213 |
| Failed | 485 |
| Score | 30.5% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 140 | 242 | 57.9% |
| Lookup (INDEX/MATCH) | 40 | 295 | 13.6% |
| Aggregation (COUNTIFS/SUMIFS) | 33 | 161 | 20.5% |

## Results by Entity

### users

- Fields: 24/56 (42.9%)
- Computed columns: wind_count, cook_count, completed_cook_count, activity_level, total_recipe_minutes_attempted, avg_recipe_minutes_attempted, cooking_style

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Carmen | cook_count | None | 3 |
| Carmen | completed_cook_count | None | 1 |
| Carmen | activity_level | busy | idle |
| Carmen | total_recipe_minutes_attempted | None | 77 |
| Carmen | avg_recipe_minutes_attempted | None | 25.6666666666666667 |
| Carmen | cooking_style | slow-cooking | short-order |
| Diego | cook_count | None | 2 |
| Diego | activity_level | busy | idle |
| Diego | total_recipe_minutes_attempted | None | 30 |
| Diego | avg_recipe_minutes_attempted | None | 15.0000000000000000 |
| Diego | cooking_style | slow-cooking | short-order |
| Eli | activity_level | busy | idle |
| Eli | cooking_style | slow-cooking | not cooking |
| Iris | cook_count | None | 2 |
| Iris | activity_level | busy | idle |
| Iris | total_recipe_minutes_attempted | None | 68 |
| Iris | avg_recipe_minutes_attempted | None | 34.0000000000000000 |
| Lena | wind_count | None | 1 |
| Lena | cook_count | None | 4 |
| Lena | activity_level | busy | casual |
| ... | ... | (12 more) | ... |

### timers

- Fields: 45/72 (62.5%)
- Computed columns: wind_fraction, stored_torque_nm, remaining_minutes, wind_state, is_armed, has_rung, tick_count, ring_count, cook_count, total_recipe_minutes_used, avg_recipe_minutes_used, wind_duration_minutes, expected_ticks_per_minute, expected_total_ticks, ticks_elapsed_expected, tick_efficiency_percent, has_sparse_tick_record, mechanical_phase

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie | wind_fraction | 0.0 | 0E-20 |
| Bessie | stored_torque_nm | 0.0 | 0E-22 |
| Bessie | remaining_minutes | 0.0 | 0E-20 |
| Bessie | tick_count | None | 8 |
| Bessie | ring_count | None | 1 |
| Bessie | cook_count | None | 3 |
| Bessie | total_recipe_minutes_used | None | 19 |
| Bessie | avg_recipe_minutes_used | None | 6.3333333333333333 |
| Bessie | tick_efficiency_percent | 0.0 | 0.05555555555555555600 |
| Bessie | has_sparse_tick_record | False | True |
| Olive | tick_count | None | 8 |
| Olive | cook_count | None | 6 |
| Olive | total_recipe_minutes_used | None | 454 |
| Olive | avg_recipe_minutes_used | None | 75.6666666666666667 |
| Olive | tick_efficiency_percent | 0.0 | 0.08333333333333333300 |
| Olive | has_sparse_tick_record | False | True |
| Rufus | tick_count | None | 4 |
| Rufus | cook_count | None | 7 |
| Rufus | total_recipe_minutes_used | None | 252 |
| Rufus | avg_recipe_minutes_used | None | 36.0000000000000000 |
| ... | ... | (7 more) | ... |

### bells

- Fields: 6/8 (75.0%)
- Computed columns: times_rung, has_ever_rung

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's bell | times_rung | None | 1 |
| Bessie's bell | has_ever_rung | False | True |

### winding_knobs

- Fields: 1/12 (8.3%)
- Computed columns: timer_wind_angle, timer_max_wind, scale_reading

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's knob | timer_max_wind | None | 360 |
| Bessie's knob | scale_reading | 0 | 0E-20 |
| Olive's knob | timer_wind_angle | None | 120 |
| Olive's knob | timer_max_wind | None | 360 |
| Olive's knob | scale_reading | 0 | 19.99999999999999999980 |
| Rufus's knob | timer_wind_angle | None | 360 |
| Rufus's knob | timer_max_wind | None | 360 |
| Rufus's knob | scale_reading | 0 | 60.00000000000000000000 |
| Twigs's knob | timer_wind_angle | None | 30 |
| Twigs's knob | timer_max_wind | None | 360 |
| Twigs's knob | scale_reading | 0 | 4.99999999999999999980 |

### gear_trains

- Fields: 0/20 (0.0%)
- Computed columns: timer_escapement_hz, output_shaft_rpm, gear_count, total_teeth, average_tooth_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's gear train | timer_escapement_hz | None | 4 |
| Bessie's gear train | output_shaft_rpm | 0.0 | 1.00000000000000000000 |
| Bessie's gear train | gear_count | None | 3 |
| Bessie's gear train | total_teeth | None | 108 |
| Bessie's gear train | average_tooth_count | None | 36.0000000000000000 |
| Olive's gear train | timer_escapement_hz | None | 4 |
| Olive's gear train | output_shaft_rpm | 0.0 | 1.00000000000000000000 |
| Olive's gear train | gear_count | None | 3 |
| Olive's gear train | total_teeth | None | 108 |
| Olive's gear train | average_tooth_count | None | 36.0000000000000000 |
| Rufus's gear train | timer_escapement_hz | None | 4 |
| Rufus's gear train | output_shaft_rpm | 0.0 | 1.00000000000000000000 |
| Rufus's gear train | gear_count | None | 3 |
| Rufus's gear train | total_teeth | None | 108 |
| Rufus's gear train | average_tooth_count | None | 36.0000000000000000 |
| Twigs's gear train | timer_escapement_hz | None | 4 |
| Twigs's gear train | output_shaft_rpm | 0.0 | 1.00000000000000000000 |
| Twigs's gear train | gear_count | None | 3 |
| Twigs's gear train | total_teeth | None | 108 |
| Twigs's gear train | average_tooth_count | None | 36.0000000000000000 |

### gears

- Fields: 36/48 (75.0%)
- Computed columns: is_first_in_chain, is_last_in_chain, size_class, on_timer

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's first gear | on_timer | None | Bessie |
| Bessie's second gear | on_timer | None | Bessie |
| Bessie's third gear | on_timer | None | Bessie |
| Olive's first gear | on_timer | None | Olive |
| Olive's second gear | on_timer | None | Olive |
| Olive's third gear | on_timer | None | Olive |
| Rufus's first gear | on_timer | None | Rufus |
| Rufus's second gear | on_timer | None | Rufus |
| Rufus's third gear | on_timer | None | Rufus |
| Twigs's first gear | on_timer | None | Twigs |
| Twigs's second gear | on_timer | None | Twigs |
| Twigs's third gear | on_timer | None | Twigs |

### output_shafts

- Fields: 0/4 (0.0%)
- Computed columns: current_rpm

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's output shaft | current_rpm | None | 1.00000000000000000000 |
| Olive's output shaft | current_rpm | None | 1.00000000000000000000 |
| Rufus's output shaft | current_rpm | None | 1.00000000000000000000 |
| Twigs's output shaft | current_rpm | None | 1.00000000000000000000 |

### escapements

- Fields: 0/8 (0.0%)
- Computed columns: tick_count, has_ticked

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's escapement | tick_count | None | 8 |
| Bessie's escapement | has_ticked | False | True |
| Olive's escapement | tick_count | None | 8 |
| Olive's escapement | has_ticked | False | True |
| Rufus's escapement | tick_count | None | 4 |
| Rufus's escapement | has_ticked | False | True |
| Twigs's escapement | tick_count | None | 3 |
| Twigs's escapement | has_ticked | False | True |

### bell_hammers

- Fields: 6/8 (75.0%)
- Computed columns: timer_has_rung, release_state

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's bell hammer | timer_has_rung | None | True |
| Bessie's bell hammer | release_state | Held | Released |

### hammer_catches

- Fields: 1/8 (12.5%)
- Computed columns: hammer_release_state, is_engaged

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's hammer catch | hammer_release_state | None | Released |
| Olive's hammer catch | hammer_release_state | None | Held |
| Olive's hammer catch | is_engaged | False | True |
| Rufus's hammer catch | hammer_release_state | None | Held |
| Rufus's hammer catch | is_engaged | False | True |
| Twigs's hammer catch | hammer_release_state | None | Held |
| Twigs's hammer catch | is_engaged | False | True |

### wind_actions

- Fields: 3/10 (30.0%)
- Computed columns: applied_to_timer, resulted_in_ring

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Lena wound Olive | applied_to_timer | None | Olive |
| Marcus readjusted Bessie | applied_to_timer | None | Bessie |
| Marcus readjusted Bessie | resulted_in_ring | None | True |
| Marcus wound Bessie | applied_to_timer | None | Bessie |
| Marcus wound Bessie | resulted_in_ring | None | True |
| Marcus wound Rufus | applied_to_timer | None | Rufus |
| Marcus wound Twigs | applied_to_timer | None | Twigs |

### tick_events

- Fields: 15/69 (21.7%)
- Computed columns: timer_label, on_timer_that_has_rung, on_timer_in_phase

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Bessie's tick 1 | timer_label | None | Egg Timer |
| Bessie's tick 1 | on_timer_that_has_rung | None | True |
| Bessie's tick 1 | on_timer_in_phase | None | rung |
| Bessie's tick 2 | timer_label | None | Egg Timer |
| Bessie's tick 2 | on_timer_that_has_rung | None | True |
| Bessie's tick 2 | on_timer_in_phase | None | rung |
| Bessie's tick 3 | timer_label | None | Egg Timer |
| Bessie's tick 3 | on_timer_that_has_rung | None | True |
| Bessie's tick 3 | on_timer_in_phase | None | rung |
| Bessie's tick 4 | timer_label | None | Egg Timer |
| Bessie's tick 4 | on_timer_that_has_rung | None | True |
| Bessie's tick 4 | on_timer_in_phase | None | rung |
| Bessie's tick 5 | timer_label | None | Egg Timer |
| Bessie's tick 5 | on_timer_that_has_rung | None | True |
| Bessie's tick 5 | on_timer_in_phase | None | rung |
| Bessie's tick 6 | timer_label | None | Egg Timer |
| Bessie's tick 6 | on_timer_that_has_rung | None | True |
| Bessie's tick 6 | on_timer_in_phase | None | rung |
| Bessie's tick 7 | timer_label | None | Egg Timer |
| Bessie's tick 7 | on_timer_that_has_rung | None | True |
| ... | ... | (34 more) | ... |

### cooks

- Fields: 26/234 (11.1%)
- Computed columns: timer_has_rung, timer_is_armed, timer_remaining_min, status, recommended_minutes, timer_wind_duration, timer_can_cover_recipe, suitability_verdict, prepared_by_name, prepared_by_activity_level, recipe_duration_category, recipe_temp_profile, timer_mechanical_phase

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| afternoon tea | timer_is_armed | None | True |
| afternoon tea | timer_remaining_min | None | 5.0000000000000000 |
| afternoon tea | status | Pending | Cooking |
| afternoon tea | recommended_minutes | None | 4 |
| afternoon tea | timer_wind_duration | None | 60.0000000000000000 |
| afternoon tea | timer_can_cover_recipe | False | True |
| afternoon tea | suitability_verdict | appropriate | overkill |
| afternoon tea | prepared_by_name | None | Chef |
| afternoon tea | prepared_by_activity_level | None | busy |
| afternoon tea | recipe_duration_category | None | quick |
| afternoon tea | recipe_temp_profile | None | boiling |
| afternoon tea | timer_mechanical_phase | None | nearly-done |
| baker's espresso | timer_is_armed | None | True |
| baker's espresso | timer_remaining_min | None | 20.0000000000000000 |
| baker's espresso | status | Pending | Cooking |
| baker's espresso | recommended_minutes | None | 2 |
| baker's espresso | timer_wind_duration | None | 60.0000000000000000 |
| baker's espresso | timer_can_cover_recipe | False | True |
| baker's espresso | suitability_verdict | appropriate | overkill |
| baker's espresso | prepared_by_name | None | Baker |
| ... | ... | (188 more) | ... |

### recipes

- Fields: 50/114 (43.9%)
- Computed columns: times_used_today, duration_category, cooking_temp_profile, avg_timer_capacity, total_timer_capacity, assignment_verdict

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| broccoli recipe | times_used_today | None | 1 |
| broccoli recipe | avg_timer_capacity | None | 60.0000000000000000 |
| broccoli recipe | total_timer_capacity | None | 60.0000000000000000 |
| cake recipe | times_used_today | None | 1 |
| cake recipe | avg_timer_capacity | None | 60.0000000000000000 |
| cake recipe | total_timer_capacity | None | 60.0000000000000000 |
| creme brulee recipe | assignment_verdict | well-matched | unused |
| croissant recipe | times_used_today | None | 1 |
| croissant recipe | avg_timer_capacity | None | 60.0000000000000000 |
| croissant recipe | total_timer_capacity | None | 60.0000000000000000 |
| croissant recipe | assignment_verdict | well-matched | over-provisioned |
| flan recipe | times_used_today | None | 1 |
| flan recipe | avg_timer_capacity | None | 60.0000000000000000 |
| flan recipe | total_timer_capacity | None | 60.0000000000000000 |
| instant coffee recipe | times_used_today | None | 2 |
| instant coffee recipe | avg_timer_capacity | None | 60.0000000000000000 |
| instant coffee recipe | total_timer_capacity | None | 120.0000000000000000 |
| instant coffee recipe | assignment_verdict | well-matched | over-provisioned |
| paella recipe | times_used_today | None | 1 |
| paella recipe | avg_timer_capacity | None | 60.0000000000000000 |
| ... | ... | (44 more) | ... |

### kitchens

- Fields: 0/27 (0.0%)
- Computed columns: total_timers, total_cases, total_bells, total_users, total_cooks, total_recipes, total_wind_actions, total_tick_events, total_ring_events, timers_freshly_wound, timers_mid_run, timers_nearly_done, timers_rung, cooks_done, cooks_cooking, recipes_unused, recipes_under_provisioned, recipes_well_matched, busy_users, idle_users, total_recipe_minutes, avg_recipe_minutes, min_recipe_minutes, max_recipe_minutes, has_any_timer_rung, has_misprovisioned_recipe, operational_status

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| Main | total_timers | None | 4 |
| Main | total_cases | None | 4 |
| Main | total_bells | None | 4 |
| Main | total_users | None | 8 |
| Main | total_cooks | None | 18 |
| Main | total_recipes | None | 19 |
| Main | total_wind_actions | None | 5 |
| Main | total_tick_events | None | 23 |
| Main | total_ring_events | None | 1 |
| Main | timers_freshly_wound | None | 1 |
| Main | timers_mid_run | None | 1 |
| Main | timers_nearly_done | None | 1 |
| Main | timers_rung | None | 1 |
| Main | cooks_done | None | 3 |
| Main | cooks_cooking | None | 15 |
| Main | recipes_unused | None | 2 |
| Main | recipes_under_provisioned | None | 2 |
| Main | recipes_well_matched | None | 6 |
| Main | busy_users | None | 1 |
| Main | idle_users | None | 6 |
| ... | ... | (7 more) | ... |
