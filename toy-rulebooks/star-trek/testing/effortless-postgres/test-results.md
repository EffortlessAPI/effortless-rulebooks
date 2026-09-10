# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 3941 |
| Passed | 1047 |
| Failed | 2894 |
| Score | 26.6% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 952 | 1001 | 95.1% |
| Lookup (INDEX/MATCH) | 29 | 2862 | 1.0% |
| Aggregation (COUNTIFS/SUMIFS) | 66 | 78 | 84.6% |

## Results by Entity

### series

- Fields: 45/60 (75.0%)
- Computed columns: name, total_seasons, total_episodes, is_long_running, rating, is_good

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos | rating | 4.333333333333333 | 3 |
| 10-snw | rating | {
  "specialValue": "NaN"
} | 0 |
| 2-tng | rating | 5 | 1 |
| 2-tng | is_good | True | False |
| 3-ds9 | is_long_running | False | True |
| 3-ds9 | rating | {
  "specialValue": "NaN"
} | 0 |
| 4-voy | is_long_running | False | True |
| 4-voy | rating | {
  "specialValue": "NaN"
} | 0 |
| 5-ent | is_long_running | False | True |
| 5-ent | rating | {
  "specialValue": "NaN"
} | 0 |
| 6-dis | is_long_running | False | True |
| 6-dis | rating | {
  "specialValue": "NaN"
} | 0 |
| 7-pic | rating | {
  "specialValue": "NaN"
} | 0 |
| 8-lds | rating | {
  "specialValue": "NaN"
} | 0 |
| 9-pro | rating | {
  "specialValue": "NaN"
} | 0 |

### seasons

- Fields: 44/220 (20.0%)
- Computed columns: name, show_title, show_description, show_network, episode_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1 | name | TOS-Season-1 | 1-tos-Season-1 |
| 1-tos-season-1 | show_title | None | Star Trek: The Original Series |
| 1-tos-season-1 | show_description | None | The voyages of the starship En |
| 1-tos-season-1 | show_network | None | NBC |
| 1-tos-season-2 | name | TOS-Season-2 | 1-tos-Season-2 |
| 1-tos-season-2 | show_title | None | Star Trek: The Original Series |
| 1-tos-season-2 | show_description | None | The voyages of the starship En |
| 1-tos-season-2 | show_network | None | NBC |
| 1-tos-season-3 | name | TOS-Season-3 | 1-tos-Season-3 |
| 1-tos-season-3 | show_title | None | Star Trek: The Original Series |
| 1-tos-season-3 | show_description | None | The voyages of the starship En |
| 1-tos-season-3 | show_network | None | NBC |
| 10-snw-season-1 | name | SNW-Season-1 | 10-snw-Season-1 |
| 10-snw-season-1 | show_title | None | Star Trek: Strange New Worlds |
| 10-snw-season-1 | show_description | None | Captain Pike leads the Enterpr |
| 10-snw-season-1 | show_network | None | Paramount+ |
| 10-snw-season-2 | name | SNW-Season-2 | 10-snw-Season-2 |
| 10-snw-season-2 | show_title | None | Star Trek: Strange New Worlds |
| 10-snw-season-2 | show_description | None | Captain Pike leads the Enterpr |
| 10-snw-season-2 | show_network | None | Paramount+ |
| ... | ... | (156 more) | ... |

### episodes

- Fields: 893/3572 (25.0%)
- Computed columns: name, season_number, season_title, season_description

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1-episode-01 | season_number | None | 1 |
| 1-tos-season-1-episode-01 | season_title | None | The Original Series Season 1 |
| 1-tos-season-1-episode-01 | season_description | None | Season 1 of Star Trek: The Ori |
| 1-tos-season-1-episode-02 | season_number | None | 1 |
| 1-tos-season-1-episode-02 | season_title | None | The Original Series Season 1 |
| 1-tos-season-1-episode-02 | season_description | None | Season 1 of Star Trek: The Ori |
| 1-tos-season-1-episode-03 | season_number | None | 1 |
| 1-tos-season-1-episode-03 | season_title | None | The Original Series Season 1 |
| 1-tos-season-1-episode-03 | season_description | None | Season 1 of Star Trek: The Ori |
| 1-tos-season-1-episode-04 | season_number | None | 1 |
| 1-tos-season-1-episode-04 | season_title | None | The Original Series Season 1 |
| 1-tos-season-1-episode-04 | season_description | None | Season 1 of Star Trek: The Ori |
| 1-tos-season-1-episode-05 | season_number | None | 1 |
| 1-tos-season-1-episode-05 | season_title | None | The Original Series Season 1 |
| 1-tos-season-1-episode-05 | season_description | None | Season 1 of Star Trek: The Ori |
| 1-tos-season-1-episode-06 | season_number | None | 1 |
| 1-tos-season-1-episode-06 | season_title | None | The Original Series Season 1 |
| 1-tos-season-1-episode-06 | season_description | None | Season 1 of Star Trek: The Ori |
| 1-tos-season-1-episode-07 | season_number | None | 1 |
| 1-tos-season-1-episode-07 | season_title | None | The Original Series Season 1 |
| ... | ... | (2659 more) | ... |

### movies

- Fields: 6/8 (75.0%)
- Computed columns: name, crew_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| movie-1-generations | crew_count | 0 | 3 |
| movie-2-first-contact | crew_count | 0 | 1 |

### crew_assignments

- Fields: 16/16 (100.0%)
- Computed columns: name, person_name, movie_title, crew_type_name

### ratings

- Fields: 43/65 (66.2%)
- Computed columns: name, display_name, series_name, episode_name, episode_season_title

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| episode-1-tos-season-1-episode-01-alexis-cruz | series_name |  | - |
| episode-1-tos-season-1-episode-01-alexis-cruz | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-02-morgan-lee | series_name |  | - |
| episode-1-tos-season-1-episode-02-morgan-lee | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-03-jamie-fox | series_name |  | - |
| episode-1-tos-season-1-episode-03-jamie-fox | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-07-taylor-brooks | series_name |  | - |
| episode-1-tos-season-1-episode-07-taylor-brooks | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-08-robin-singh | series_name |  | - |
| episode-1-tos-season-1-episode-08-robin-singh | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-09-jordan-kim | series_name |  | - |
| episode-1-tos-season-1-episode-09-jordan-kim | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-10-dylan-reed | series_name |  | - |
| episode-1-tos-season-1-episode-10-dylan-reed | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-11-casey-morgan | series_name |  | - |
| episode-1-tos-season-1-episode-11-casey-morgan | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| episode-1-tos-season-1-episode-12-sydney-park | series_name |  | - |
| episode-1-tos-season-1-episode-12-sydney-park | episode_season_title | TOS Season 1 | The Original Series Season 1 |
| series-1-tos-alex-mercer | episode_name |  | -Episode- |
| series-1-tos-drew-patel | episode_name |  | -Episode- |
| ... | ... | (2 more) | ... |
