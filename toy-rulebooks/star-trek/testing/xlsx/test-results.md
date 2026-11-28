# Test Results: xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 3961 |
| Passed | 3562 |
| Failed | 399 |
| Score | 89.9% |
| Duration | 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 612 | 1011 | 60.5% |
| Lookup (INDEX/MATCH) | 2862 | 2862 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 88 | 88 | 100.0% |

## Results by Entity

### series

- Fields: 77/80 (96.2%)
- Computed columns: name, total_seasons, total_episodes, is_long_running, rating_count, rating_total, rating, is_good

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos | rating | 4.333333333333333 | None |
| 2-tng | rating | 5 | None |
| 2-tng | is_good | True | None |

### seasons

- Fields: 220/220 (100.0%)
- Computed columns: name, show_title, show_description, show_network, episode_count

### episodes

- Fields: 3176/3572 (88.9%)
- Computed columns: name, season_number, season_title, season_description

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1-episode-01 | name | 1-tos-season-1-Episode-01 | 1-tos-season-1-Episode-1 |
| 1-tos-season-1-episode-02 | name | 1-tos-season-1-Episode-02 | 1-tos-season-1-Episode-2 |
| 1-tos-season-1-episode-03 | name | 1-tos-season-1-Episode-03 | 1-tos-season-1-Episode-3 |
| 1-tos-season-1-episode-04 | name | 1-tos-season-1-Episode-04 | 1-tos-season-1-Episode-4 |
| 1-tos-season-1-episode-05 | name | 1-tos-season-1-Episode-05 | 1-tos-season-1-Episode-5 |
| 1-tos-season-1-episode-06 | name | 1-tos-season-1-Episode-06 | 1-tos-season-1-Episode-6 |
| 1-tos-season-1-episode-07 | name | 1-tos-season-1-Episode-07 | 1-tos-season-1-Episode-7 |
| 1-tos-season-1-episode-08 | name | 1-tos-season-1-Episode-08 | 1-tos-season-1-Episode-8 |
| 1-tos-season-1-episode-09 | name | 1-tos-season-1-Episode-09 | 1-tos-season-1-Episode-9 |
| 1-tos-season-2-episode-01 | name | 1-tos-season-2-Episode-01 | 1-tos-season-2-Episode-1 |
| 1-tos-season-2-episode-02 | name | 1-tos-season-2-Episode-02 | 1-tos-season-2-Episode-2 |
| 1-tos-season-2-episode-03 | name | 1-tos-season-2-Episode-03 | 1-tos-season-2-Episode-3 |
| 1-tos-season-2-episode-04 | name | 1-tos-season-2-Episode-04 | 1-tos-season-2-Episode-4 |
| 1-tos-season-2-episode-05 | name | 1-tos-season-2-Episode-05 | 1-tos-season-2-Episode-5 |
| 1-tos-season-2-episode-06 | name | 1-tos-season-2-Episode-06 | 1-tos-season-2-Episode-6 |
| 1-tos-season-2-episode-07 | name | 1-tos-season-2-Episode-07 | 1-tos-season-2-Episode-7 |
| 1-tos-season-2-episode-08 | name | 1-tos-season-2-Episode-08 | 1-tos-season-2-Episode-8 |
| 1-tos-season-2-episode-09 | name | 1-tos-season-2-Episode-09 | 1-tos-season-2-Episode-9 |
| 1-tos-season-3-episode-01 | name | 1-tos-season-3-Episode-01 | 1-tos-season-3-Episode-1 |
| 1-tos-season-3-episode-02 | name | 1-tos-season-3-Episode-02 | 1-tos-season-3-Episode-2 |
| ... | ... | (376 more) | ... |

### movies

- Fields: 8/8 (100.0%)
- Computed columns: name, crew_count

### crew_assignments

- Fields: 16/16 (100.0%)
- Computed columns: name, person_name, movie_title, crew_type_name

### ratings

- Fields: 65/65 (100.0%)
- Computed columns: name, display_name, series_name, episode_name, episode_season_title
