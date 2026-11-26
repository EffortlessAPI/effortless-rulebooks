# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 3961 |
| Passed | 3961 |
| Failed | 0 |
| Score | 100.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 1011 | 1011 | 100.0% |
| Lookup (INDEX/MATCH) | 2862 | 2862 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 88 | 88 | 100.0% |

## Results by Entity

### series

- Fields: 80/80 (100.0%)
- Computed columns: name, total_seasons, total_episodes, is_long_running, rating_count, rating_total, rating, is_good

### seasons

- Fields: 220/220 (100.0%)
- Computed columns: name, show_title, show_description, show_network, episode_count

### episodes

- Fields: 3572/3572 (100.0%)
- Computed columns: name, season_number, season_title, season_description

### movies

- Fields: 8/8 (100.0%)
- Computed columns: name, crew_count

### crew_assignments

- Fields: 16/16 (100.0%)
- Computed columns: name, person_name, movie_title, crew_type_name

### ratings

- Fields: 65/65 (100.0%)
- Computed columns: name, display_name, series_name, episode_name, episode_season_title
