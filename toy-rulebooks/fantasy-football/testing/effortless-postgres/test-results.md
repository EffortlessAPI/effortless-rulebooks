# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 471 |
| Passed | 471 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 183 | 183 | 100.0% |
| Lookup (INDEX/MATCH) | 258 | 258 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 30 | 30 | 100.0% |

## Results by Entity

### dev_users

- Fields: 3/3 (100.0%)
- Computed columns: name

### players

- Fields: 54/54 (100.0%)
- Computed columns: name, total_touchdowns, projected_score

### rosters

- Fields: 42/42 (100.0%)
- Computed columns: name, total_passing_yards, total_rushing_yards, total_receiving_yards, total_touchdowns, total_interceptions, projected_score

### roster_assignments

- Fields: 234/234 (100.0%)
- Computed columns: name, player_name, position, passing_yards, passing_touchdowns, interceptions, rushing_yards, rushing_touchdowns, receptions, received_yards, received_touchdowns, total_touchdowns, projected_score

### matchups

- Fields: 126/126 (100.0%)
- Computed columns: name, team1_score, team2_score, margin, winner, loser, is_tie_game

### standings

- Fields: 12/12 (100.0%)
- Computed columns: name, owner
