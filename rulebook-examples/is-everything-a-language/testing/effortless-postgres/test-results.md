# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 264 |
| Passed | 115 |
| Failed | 149 |
| Score | 43.6% |
| Duration | 4s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 115 | 264 | 43.6% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | — | 0 | n/a |

## Results by Entity

### language_candidates

- Fields: 115/264 (43.6%)
- Computed columns: has_grammar, question, predicted_answer, prediction_predicates, prediction_fail, is_description_of, is_open_closed_world_conflicted, relationship_to_concept

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| a-coffee-mug | question | Is A Coffee Mug a language? | None |
| a-coffee-mug | prediction_predicates | No Syntax & No Parsing Neede & | None |
| a-coffee-mug | relationship_to_concept | IsMirrorOf | None |
| a-game-of-fortnite | question | Is A Game of Fortnite a langua | None |
| a-game-of-fortnite | prediction_predicates | No Syntax & Requires Parsing & | None |
| a-game-of-fortnite | relationship_to_concept | IsMirrorOf | None |
| a-running-app | question | Is A Running App  a language? | None |
| a-running-app | prediction_predicates | No Syntax & Requires Parsing & | None |
| a-running-app | relationship_to_concept | IsMirrorOf | None |
| a-smartphone | question | Is A Smartphone a language? | None |
| a-smartphone | prediction_predicates | No Syntax & No Parsing Neede & | None |
| a-smartphone | relationship_to_concept | IsMirrorOf | None |
| a-thunderstorm | question | Is A Thunderstorm a language? | None |
| a-thunderstorm | prediction_predicates | No Syntax & Requires Parsing & | None |
| a-thunderstorm | relationship_to_concept | IsMirrorOf | None |
| a-uml-file | has_grammar | True | None |
| a-uml-file | question | Is A UML File a language? | None |
| a-uml-file | prediction_predicates | Has Syntax & Requires Parsing  | None |
| a-uml-file | prediction_fail | A UML File Isn't a Family Feud | None |
| a-uml-file | is_description_of | True | None |
| ... | ... | (129 more) | ... |
