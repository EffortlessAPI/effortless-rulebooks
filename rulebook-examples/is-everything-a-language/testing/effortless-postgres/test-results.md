# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 617 |
| Passed | 511 |
| Failed | 106 |
| Score | 82.8% |
| Duration | < 1s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 445 | 515 | 86.4% |
| Lookup (INDEX/MATCH) | — | 0 | n/a |
| Aggregation (COUNTIFS/SUMIFS) | 66 | 102 | 64.7% |

## Results by Entity

### language_candidates

- Fields: 433/455 (95.2%)
- Computed columns: has_grammar, question, predicted_answer, prediction_predicates, prediction_fail, is_description_of, is_open_closed_world_conflicted, relationship_to_concept, hockett_assessed_count, hockett_yes_count, is_hockett_assessed, hockett_score, hockett_vs_gate

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bonobo-lexigram-keyboard-communication | hockett_assessed_count | 13 | 0 |
| bonobo-lexigram-keyboard-communication | hockett_yes_count | 8 | 0 |
| bonobo-lexigram-keyboard-communication | is_hockett_assessed | True | False |
| bonobo-lexigram-keyboard-communication | hockett_score | 8 of 13 | not assessed |
| english | hockett_assessed_count | 13 | 0 |
| english | hockett_yes_count | 13 | 0 |
| english | is_hockett_assessed | True | False |
| english | hockett_score | 13 of 13 | not assessed |
| honeybee-waggle-dance | hockett_assessed_count | 13 | 0 |
| honeybee-waggle-dance | hockett_yes_count | 4 | 0 |
| honeybee-waggle-dance | is_hockett_assessed | True | False |
| honeybee-waggle-dance | hockett_score | 4 of 13 | not assessed |
| honeybee-waggle-dance | hockett_vs_gate | The eight-clause gate says lan |  |
| sign-language | hockett_assessed_count | 13 | 0 |
| sign-language | hockett_yes_count | 10 | 0 |
| sign-language | is_hockett_assessed | True | False |
| sign-language | hockett_score | 10 of 13 | not assessed |
| sign-language | hockett_vs_gate | The eight-clause gate says lan |  |
| spoken-words | hockett_assessed_count | 13 | 0 |
| spoken-words | hockett_yes_count | 13 | 0 |
| ... | ... | (2 more) | ... |

### hockett_features

- Fields: 6/32 (18.8%)
- Computed columns: assessment_count, yes_count

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| arbitrariness | assessment_count | 5 | 0 |
| arbitrariness | yes_count | 2 | 0 |
| broadcast-transmission-and-directional-reception | assessment_count | 5 | 0 |
| broadcast-transmission-and-directional-reception | yes_count | 5 | 0 |
| discreteness | assessment_count | 5 | 0 |
| discreteness | yes_count | 4 | 0 |
| displacement | assessment_count | 5 | 0 |
| displacement | yes_count | 5 | 0 |
| duality-of-patterning | assessment_count | 5 | 0 |
| duality-of-patterning | yes_count | 4 | 0 |
| interchangeability | assessment_count | 5 | 0 |
| interchangeability | yes_count | 4 | 0 |
| productivity | assessment_count | 5 | 0 |
| productivity | yes_count | 4 | 0 |
| rapid-fading | assessment_count | 5 | 0 |
| rapid-fading | yes_count | 4 | 0 |
| semanticity | assessment_count | 5 | 0 |
| semanticity | yes_count | 5 | 0 |
| specialization | assessment_count | 5 | 0 |
| specialization | yes_count | 4 | 0 |
| ... | ... | (6 more) | ... |

### hockett_assessments

- Fields: 72/130 (55.4%)
- Computed columns: name, is_yes_flag

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| bonobo-lexigram-keyboard-communication--arbitrariness | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--broadcast-transmission-and-directional-reception | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--discreteness | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--displacement | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--duality-of-patterning | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--interchangeability | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--productivity | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--rapid-fading | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--semanticity | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--specialization | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--total-feedback | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--traditional-transmission | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| bonobo-lexigram-keyboard-communication--vocal-auditory-channel | name | Bonobo/Lexigram Keyboard Commu | bonobo-lexigram-keyboard-commu |
| english--broadcast-transmission-and-directional-reception | name | English / Broadcast Transmissi | english / broadcast-transmissi |
| english--duality-of-patterning | name | English / Duality Of Patternin | english / duality-of-patternin |
| english--rapid-fading | name | English / Rapid Fading | english / rapid-fading |
| english--total-feedback | name | English / Total Feedback | english / total-feedback |
| english--traditional-transmission | name | English / Traditional Transmis | english / traditional-transmis |
| english--vocal-auditory-channel | name | English / Vocal-Auditory Chann | english / vocal-auditory-chann |
| honeybee-waggle-dance--arbitrariness | name | Honeybee Waggle Dance / Arbitr | honeybee-waggle-dance / arbitr |
| ... | ... | (38 more) | ... |
