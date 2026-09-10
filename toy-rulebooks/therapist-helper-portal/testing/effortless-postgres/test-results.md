# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 276 |
| Passed | 276 |
| Failed | 0 |
| Score | 100.0% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 93 | 93 | 100.0% |
| Lookup (INDEX/MATCH) | 115 | 115 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 68 | 68 | 100.0% |

## Results by Entity

### users

- Fields: 8/8 (100.0%)
- Computed columns: name, client_count

### clients

- Fields: 36/36 (100.0%)
- Computed columns: therapist_name, name, session_count, avg_mood_rating, goal_count, avg_goal_progress, last_session_label, is_at_risk, status_label

### goals

- Fields: 72/72 (100.0%)
- Computed columns: client_name, client_therapist, name, update_count, avg_score_achieved, latest_score, progress_pct, remaining_gap, is_on_track

### sessions

- Fields: 70/70 (100.0%)
- Computed columns: client_name, client_therapist, name, update_count, avg_score_achieved, is_productive, status_label

### goal_updates

- Fields: 90/90 (100.0%)
- Computed columns: goal_title, goal_client, goal_target_score, goal_client_therapist, session_label, name
