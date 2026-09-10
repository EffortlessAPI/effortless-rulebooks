# Test Results: xlsx

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 3941 |
| Passed | 2849 |
| Failed | 1092 |
| Score | 72.3% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 17 | 1001 | 1.7% |
| Lookup (INDEX/MATCH) | 2828 | 2862 | 98.8% |
| Aggregation (COUNTIFS/SUMIFS) | 4 | 78 | 5.1% |

## Results by Entity

### series

- Fields: 17/60 (28.3%)
- Computed columns: name, total_seasons, total_episodes, is_long_running, rating, is_good

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos | name | 1-TOS | None |
| 1-tos | total_seasons | 3 | None |
| 1-tos | total_episodes | 76 | None |
| 1-tos | is_long_running | True | None |
| 1-tos | rating | 4.333333333333333 | None |
| 10-snw | name | 10-SNW | None |
| 10-snw | total_seasons | 2 | None |
| 10-snw | total_episodes | 20 | None |
| 10-snw | rating | {
  "specialValue": "NaN"
} | None |
| 2-tng | name | 2-TNG | None |
| 2-tng | total_seasons | 7 | None |
| 2-tng | total_episodes | 178 | None |
| 2-tng | is_long_running | True | None |
| 2-tng | rating | 5 | None |
| 2-tng | is_good | True | None |
| 3-ds9 | name | 3-DS9 | None |
| 3-ds9 | total_seasons | 7 | None |
| 3-ds9 | total_episodes | 176 | None |
| 3-ds9 | rating | {
  "specialValue": "NaN"
} | None |
| 4-voy | name | 4-VOY | None |
| ... | ... | (23 more) | ... |

### seasons

- Fields: 132/220 (60.0%)
- Computed columns: name, show_title, show_description, show_network, episode_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1 | name | TOS-Season-1 | None |
| 1-tos-season-1 | episode_count | 26 | None |
| 1-tos-season-2 | name | TOS-Season-2 | None |
| 1-tos-season-2 | episode_count | 26 | None |
| 1-tos-season-3 | name | TOS-Season-3 | None |
| 1-tos-season-3 | episode_count | 24 | None |
| 10-snw-season-1 | name | SNW-Season-1 | None |
| 10-snw-season-1 | episode_count | 10 | None |
| 10-snw-season-2 | name | SNW-Season-2 | None |
| 10-snw-season-2 | episode_count | 10 | None |
| 2-tng-season-1 | name | TNG-Season-1 | None |
| 2-tng-season-1 | episode_count | 26 | None |
| 2-tng-season-2 | name | TNG-Season-2 | None |
| 2-tng-season-2 | episode_count | 22 | None |
| 2-tng-season-3 | name | TNG-Season-3 | None |
| 2-tng-season-3 | episode_count | 26 | None |
| 2-tng-season-4 | name | TNG-Season-4 | None |
| 2-tng-season-4 | episode_count | 26 | None |
| 2-tng-season-5 | name | TNG-Season-5 | None |
| 2-tng-season-5 | episode_count | 26 | None |
| ... | ... | (68 more) | ... |

### episodes

- Fields: 2679/3572 (75.0%)
- Computed columns: name, season_number, season_title, season_description

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1-tos-season-1-episode-01 | name | 1-tos-season-1-Episode-01 | None |
| 1-tos-season-1-episode-02 | name | 1-tos-season-1-Episode-02 | None |
| 1-tos-season-1-episode-03 | name | 1-tos-season-1-Episode-03 | None |
| 1-tos-season-1-episode-04 | name | 1-tos-season-1-Episode-04 | None |
| 1-tos-season-1-episode-05 | name | 1-tos-season-1-Episode-05 | None |
| 1-tos-season-1-episode-06 | name | 1-tos-season-1-Episode-06 | None |
| 1-tos-season-1-episode-07 | name | 1-tos-season-1-Episode-07 | None |
| 1-tos-season-1-episode-08 | name | 1-tos-season-1-Episode-08 | None |
| 1-tos-season-1-episode-09 | name | 1-tos-season-1-Episode-09 | None |
| 1-tos-season-1-episode-10 | name | 1-tos-season-1-Episode-10 | None |
| 1-tos-season-1-episode-11 | name | 1-tos-season-1-Episode-11 | None |
| 1-tos-season-1-episode-12 | name | 1-tos-season-1-Episode-12 | None |
| 1-tos-season-1-episode-13 | name | 1-tos-season-1-Episode-13 | None |
| 1-tos-season-1-episode-14 | name | 1-tos-season-1-Episode-14 | None |
| 1-tos-season-1-episode-15 | name | 1-tos-season-1-Episode-15 | None |
| 1-tos-season-1-episode-16 | name | 1-tos-season-1-Episode-16 | None |
| 1-tos-season-1-episode-17 | name | 1-tos-season-1-Episode-17 | None |
| 1-tos-season-1-episode-18 | name | 1-tos-season-1-Episode-18 | None |
| 1-tos-season-1-episode-19 | name | 1-tos-season-1-Episode-19 | None |
| 1-tos-season-1-episode-20 | name | 1-tos-season-1-Episode-20 | None |
| ... | ... | (873 more) | ... |

### movies

- Fields: 4/8 (50.0%)
- Computed columns: name, crew_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| movie-1-generations | name | 1-Star Trek: Generations | None |
| movie-2-first-contact | name | 2-Star Trek: First Contact | None |
| movie-3-insurrection | name | 3-Star Trek: Insurrection | None |
| movie-4-nemesis | name | 4-Star Trek: Nemesis | None |

### crew_assignments

- Fields: 0/16 (0.0%)
- Computed columns: name, person_name, movie_title, crew_type_name

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| crew-roddenberry-generations-producer | name | Gene Roddenberry-Producer-Star | None |
| crew-roddenberry-generations-producer | person_name | Gene Roddenberry | None |
| crew-roddenberry-generations-producer | movie_title | Star Trek: Generations | None |
| crew-roddenberry-generations-producer | crew_type_name | Producer | None |
| crew-shatner-kirk-generations | name | William Shatner-Actor-Star Tre | None |
| crew-shatner-kirk-generations | person_name | William Shatner | None |
| crew-shatner-kirk-generations | movie_title | Star Trek: Generations | None |
| crew-shatner-kirk-generations | crew_type_name | Actor | None |
| crew-stewart-picard-first-contact | name | Patrick Stewart-Actor-Star Tre | None |
| crew-stewart-picard-first-contact | person_name | Patrick Stewart | None |
| crew-stewart-picard-first-contact | movie_title | Star Trek: First Contact | None |
| crew-stewart-picard-first-contact | crew_type_name | Actor | None |
| crew-stewart-picard-generations | name | Patrick Stewart-Actor-Star Tre | None |
| crew-stewart-picard-generations | person_name | Patrick Stewart | None |
| crew-stewart-picard-generations | movie_title | Star Trek: Generations | None |
| crew-stewart-picard-generations | crew_type_name | Actor | None |

### ratings

- Fields: 17/65 (26.2%)
- Computed columns: name, display_name, series_name, episode_name, episode_season_title

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| episode-1-tos-season-1-episode-01-alexis-cruz | name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-01-alexis-cruz | display_name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-01-alexis-cruz | episode_name | 1-TOS-Season-1-Episode-01 | None |
| episode-1-tos-season-1-episode-01-alexis-cruz | episode_season_title | TOS Season 1 | None |
| episode-1-tos-season-1-episode-02-morgan-lee | name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-02-morgan-lee | display_name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-02-morgan-lee | episode_name | 1-TOS-Season-1-Episode-02 | None |
| episode-1-tos-season-1-episode-02-morgan-lee | episode_season_title | TOS Season 1 | None |
| episode-1-tos-season-1-episode-03-jamie-fox | name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-03-jamie-fox | display_name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-03-jamie-fox | episode_name | 1-TOS-Season-1-Episode-03 | None |
| episode-1-tos-season-1-episode-03-jamie-fox | episode_season_title | TOS Season 1 | None |
| episode-1-tos-season-1-episode-07-taylor-brooks | name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-07-taylor-brooks | display_name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-07-taylor-brooks | episode_name | 1-TOS-Season-1-Episode-07 | None |
| episode-1-tos-season-1-episode-07-taylor-brooks | episode_season_title | TOS Season 1 | None |
| episode-1-tos-season-1-episode-08-robin-singh | name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-08-robin-singh | display_name | Episode: 1-TOS-Season-1-Episod | None |
| episode-1-tos-season-1-episode-08-robin-singh | episode_name | 1-TOS-Season-1-Episode-08 | None |
| episode-1-tos-season-1-episode-08-robin-singh | episode_season_title | TOS Season 1 | None |
| ... | ... | (28 more) | ... |
