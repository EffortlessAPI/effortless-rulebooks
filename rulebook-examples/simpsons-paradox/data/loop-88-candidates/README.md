# Loop-88 Control Corpus Candidates

153 candidate datasets from the `simpsons_paradox_candidate_datasets.csv` file.
Each is evaluated as a potential **control study** (non-Simpson's paradox case) for wave-1 of the control corpus.

## Pipeline

```
CSV row
  → data/loop-88-candidates/<slug>/candidate.json   (intake — auto-generated)
  → data/loop-88-candidates/<slug>/staged-study.json (encoded — per-dataset research)
  → rulebook injection (Studies + Treatments + Strata + CaseCells rows)
```

## staged-study.json schema

Each `staged-study.json` fully describes one control study ready for rulebook injection:

```json
{
  "StudyId": "rdatasets-airlines-2024",
  "Title": "Airline On-Time Performance by Carrier and Airport",
  "Source": "Stat2Data R package — Stat2: Modeling with Regression and ANOVA (2nd ed.)",
  "SourceUrl": "https://vincentarelbundock.github.io/Rdatasets/doc/Stat2Data/Airlines.html",
  "PublicationYear": 2019,
  "Domain": "transportation",
  "IsSynthetic": false,
  "IsControlStudy": true,
  "ControlDomain": "transportation",
  "TraditionId": "tradition-dag",
  "PrimaryResearcherId": "researcher-pearl",

  "confounding_variable": "Airport",
  "causal_role": "allocation",
  "note": "One sentence on why no paradox expected / found.",

  "treatments": [
    {
      "TreatmentLabel": "Alaska",
      "Description": "Alaska Airlines flights",
      "TotalCases": 3775,
      "TotalSuccesses": 3274
    },
    {
      "TreatmentLabel": "America West",
      "Description": "America West Airlines flights",
      "TotalCases": 6558,
      "TotalSuccesses": 5584
    }
  ],
  "strata": [
    {"StratumLabel": "LAX", "Description": "Los Angeles International Airport", "TotalCases": 2800},
    {"StratumLabel": "PHX", "Description": "Phoenix Sky Harbor Airport", "TotalCases": 4840},
    {"StratumLabel": "SAN", "Description": "San Diego International Airport", "TotalCases": 1670},
    {"StratumLabel": "SFO", "Description": "San Francisco International Airport", "TotalCases": 2146},
    {"StratumLabel": "SEA", "Description": "Seattle-Tacoma International Airport", "TotalCases": 877}
  ],
  "cells": [
    {"StratumLabel": "LAX", "TreatmentLabel": "Alaska",       "Cases": 559,  "Successes": 497},
    {"StratumLabel": "LAX", "TreatmentLabel": "America West", "Cases": 2146, "Successes": 1914},
    {"StratumLabel": "PHX", "TreatmentLabel": "Alaska",       "Cases": 233,  "Successes": 212},
    {"StratumLabel": "PHX", "TreatmentLabel": "America West", "Cases": 4840, "Successes": 4416},
    {"StratumLabel": "SAN", "TreatmentLabel": "Alaska",       "Cases": 232,  "Successes": 213},
    {"StratumLabel": "SAN", "TreatmentLabel": "America West", "Cases": 448,  "Successes": 383},
    {"StratumLabel": "SFO", "TreatmentLabel": "Alaska",       "Cases": 605,  "Successes": 503},
    {"StratumLabel": "SFO", "TreatmentLabel": "America West", "Cases": 449,  "Successes": 285},
    {"StratumLabel": "SEA", "TreatmentLabel": "Alaska",       "Cases": 2146, "Successes": 1841},
    {"StratumLabel": "SEA", "TreatmentLabel": "America West", "Cases": 675,  "Successes": 587}
  ]
}
```

## Status legend (in candidate.json)

- `pending`       — not yet evaluated
- `skip`          — not suitable (continuous data, no binary outcome, wrong structure)
- `needs-data`    — structure is right but data must be fetched/computed
- `staged`        — staged-study.json complete, ready for injection
- `injected`      — already in rulebook

## Raw files that are not kept here

Two candidates' raw downloads are 94 and 95 MB each, close to the 100 MB at which GitHub refuses a push.
They were removed from the repository on 2026-11-22 and are ignored if downloaded again:

| Candidate | File | From |
|---|---|---|
| `rdatasets-nflverse-passing` | `pbp_2023.csv` (NFL play-by-play, 2023) | nflverse, https://github.com/nflverse/nflreadr |
| `rdatasets-owid-covid-data` | `data.csv` (Our World in Data COVID-19) | https://covid.ourworldindata.org/data/owid-covid-data.csv |

Nothing reads them after intake: `inject-loop-88.py` reads each candidate's `staged-study.json`.
`rdatasets-nflverse-passing` has its staged study; `rdatasets-owid-covid-data` was never staged, so
encoding it starts with downloading the file again. Older commits still hold both files.
