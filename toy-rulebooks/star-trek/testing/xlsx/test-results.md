# Test Results: xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 3961 |
| Passed | 723 |
| Failed | 3238 |
| Score | 18.3% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 612 | 1011 | 60.5% |
| Lookup (INDEX/MATCH) | 29 | 2862 | 1.0% |
| Aggregation (COUNTIFS/SUMIFS) | 82 | 88 | 93.2% |

## Results by Entity

### series

- Fields: 73/80 (91.2%)
- Computed columns: name, total_seasons, total_episodes, is_long_running, rating_count, rating_total, rating, is_good

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos | rating_count | 3 | None |
| 1-tos | rating_total | 13 | None |
| 1-tos | rating | 4.333333333333333 | None |
| 2-tng | rating_count | 1 | None |
| 2-tng | rating_total | 5 | None |
| 2-tng | rating | 5 | None |
| 2-tng | is_good | True | None |

### seasons

- Fields: 88/220 (40.0%)
- Computed columns: name, show_title, show_description, show_network, episode_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1 | show_title | Star Trek: The Original Series | None |
| 1-tos-season-1 | show_description | The voyages of the starship En | None |
| 1-tos-season-1 | show_network | NBC | None |
| 1-tos-season-2 | show_title | Star Trek: The Original Series | None |
| 1-tos-season-2 | show_description | The voyages of the starship En | None |
| 1-tos-season-2 | show_network | NBC | None |
| 1-tos-season-3 | show_title | Star Trek: The Original Series | None |
| 1-tos-season-3 | show_description | The voyages of the starship En | None |
| 1-tos-season-3 | show_network | NBC | None |
| 10-snw-season-1 | show_title | Star Trek: Strange New Worlds | None |
| 10-snw-season-1 | show_description | Captain Pike leads the Enterpr | None |
| 10-snw-season-1 | show_network | Paramount+ | None |
| 10-snw-season-2 | show_title | Star Trek: Strange New Worlds | None |
| 10-snw-season-2 | show_description | Captain Pike leads the Enterpr | None |
| 10-snw-season-2 | show_network | Paramount+ | None |
| 2-tng-season-1 | show_title | Star Trek: The Next Generation | None |
| 2-tng-season-1 | show_description | The next generation of Starfle | None |
| 2-tng-season-1 | show_network | Syndication | None |
| 2-tng-season-2 | show_title | Star Trek: The Next Generation | None |
| 2-tng-season-2 | show_description | The next generation of Starfle | None |
| ... | ... | (112 more) | ... |

### episodes

- Fields: 497/3572 (13.9%)
- Computed columns: name, season_number, season_title, season_description

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1-episode-01 | name | 1-tos-season-1-Episode-01 | 1-tos-season-1-Episode-1 |
| 1-tos-season-1-episode-01 | season_number | 1 | None |
| 1-tos-season-1-episode-01 | season_title | The Original Series Season 1 | None |
| 1-tos-season-1-episode-01 | season_description | Season 1 of Star Trek: The Ori | None |
| 1-tos-season-1-episode-02 | name | 1-tos-season-1-Episode-02 | 1-tos-season-1-Episode-2 |
| 1-tos-season-1-episode-02 | season_number | 1 | None |
| 1-tos-season-1-episode-02 | season_title | The Original Series Season 1 | None |
| 1-tos-season-1-episode-02 | season_description | Season 1 of Star Trek: The Ori | None |
| 1-tos-season-1-episode-03 | name | 1-tos-season-1-Episode-03 | 1-tos-season-1-Episode-3 |
| 1-tos-season-1-episode-03 | season_number | 1 | None |
| 1-tos-season-1-episode-03 | season_title | The Original Series Season 1 | None |
| 1-tos-season-1-episode-03 | season_description | Season 1 of Star Trek: The Ori | None |
| 1-tos-season-1-episode-04 | name | 1-tos-season-1-Episode-04 | 1-tos-season-1-Episode-4 |
| 1-tos-season-1-episode-04 | season_number | 1 | None |
| 1-tos-season-1-episode-04 | season_title | The Original Series Season 1 | None |
| 1-tos-season-1-episode-04 | season_description | Season 1 of Star Trek: The Ori | None |
| 1-tos-season-1-episode-05 | name | 1-tos-season-1-Episode-05 | 1-tos-season-1-Episode-5 |
| 1-tos-season-1-episode-05 | season_number | 1 | None |
| 1-tos-season-1-episode-05 | season_title | The Original Series Season 1 | None |
| 1-tos-season-1-episode-05 | season_description | Season 1 of Star Trek: The Ori | None |
| ... | ... | (3055 more) | ... |

### movies

- Fields: 6/8 (75.0%)
- Computed columns: name, crew_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| movie-1-generations | crew_count | 3 | 0 |
| movie-2-first-contact | crew_count | 1 | 0 |

### crew_assignments

- Fields: 16/16 (100.0%)
- Computed columns: name, person_name, movie_title, crew_type_name

### ratings

- Fields: 43/65 (66.2%)
- Computed columns: name, display_name, series_name, episode_name, episode_season_title

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| episode-1-tos-season-1-episode-01-alexis-cruz | series_name | - | None |
| episode-1-tos-season-1-episode-01-alexis-cruz | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-02-morgan-lee | series_name | - | None |
| episode-1-tos-season-1-episode-02-morgan-lee | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-03-jamie-fox | series_name | - | None |
| episode-1-tos-season-1-episode-03-jamie-fox | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-07-taylor-brooks | series_name | - | None |
| episode-1-tos-season-1-episode-07-taylor-brooks | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-08-robin-singh | series_name | - | None |
| episode-1-tos-season-1-episode-08-robin-singh | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-09-jordan-kim | series_name | - | None |
| episode-1-tos-season-1-episode-09-jordan-kim | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-10-dylan-reed | series_name | - | None |
| episode-1-tos-season-1-episode-10-dylan-reed | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-11-casey-morgan | series_name | - | None |
| episode-1-tos-season-1-episode-11-casey-morgan | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| episode-1-tos-season-1-episode-12-sydney-park | series_name | - | None |
| episode-1-tos-season-1-episode-12-sydney-park | episode_season_title | The Original Series Season 1 | TOS Season 1 |
| series-1-tos-alex-mercer | episode_name | -Episode-0 | None |
| series-1-tos-drew-patel | episode_name | -Episode-0 | None |
| ... | ... | (2 more) | ... |
