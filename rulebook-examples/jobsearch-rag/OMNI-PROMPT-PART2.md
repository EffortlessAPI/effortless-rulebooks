# OMNI Prompt — Part 2: Lookups & Formulas (Per Table)

> **Instructions for OMNI:** Add the computed fields below to the tables created in Part 1.
> Run this prompt **one table section at a time**. Each section is self-contained.

---

## SearchUrls — Computed Fields

### Lookups

- **JobBoardName** (Lookup): Display name of the parent job board.
  - Through: `JobBoard` -> JobBoards.`Name`

---

## ResumeSections — Computed Fields

### Lookups

- **ResumeName** (Lookup): Display name of the parent resume.
  - Through: `Resume` -> Resumes.`Name`

---

## SearchRuns — Computed Fields

### Lookups

- **ResumeName** (Lookup): Display name of the resume used for this run.
  - Through: `Resume` -> Resumes.`Name`

---

## JobListings — Computed Fields

### Lookups

- **JobBoardName** (Lookup): Display name of the source job board.
  - Through: `JobBoard` -> JobBoards.`Name`

---

## ScoreResults — Computed Fields

### Lookups

- **JobListingName** (Lookup): Title of the scored job listing.
  - Through: `JobListing` -> JobListings.`Name`
- **SearchRunName** (Lookup): Label of the parent search run.
  - Through: `SearchRun` -> SearchRuns.`Name`
- **Company** (Lookup): Company name from the scored listing.
  - Through: `JobListing` -> JobListings.`Company`

### Formulas

- **Name** (Formula): Display label combining job title and search run. Convert the existing Name text field to this formula:
  - `CONCATENATE({JobListingName}, " - ", {SearchRunName})`
- **FinalScore** (Formula): Weighted fusion of all component scores. Zeroed if disqualified.
  - `IF({IsDisqualified}, 0, MAX(0, {FitScore} + {ArchetypeScore} + {HistoryScore} + {CompScore} + {CultureScore} - {NegativeScore}))`

---

## Decisions — Computed Fields

### Lookups

- **JobListingName** (Lookup): Title of the decided job listing.
  - Through: `JobListing` -> JobListings.`Name`
- **Company** (Lookup): Company name from the decided listing.
  - Through: `JobListing` -> JobListings.`Company`
- **Board** (Lookup): Job board the decided listing came from.
  - Through: `JobListing` -> JobListings.`JobBoard`

### Formulas

- **Name** (Formula): Display label combining verdict and job title. Convert the existing Name text field to this formula:
  - `CONCATENATE(UPPER({Verdict}), ": ", {JobListingName})`
- **IsScoringSignal** (Formula): Whether this verdict contributes to history_score. Only "yes" verdicts are scoring signals.
  - `IF({Verdict} = "yes", TRUE(), FALSE())`

---

## Tables With No Part 2 Work

The following tables have no lookup or formula fields — they are complete after Part 1:

- **JobBoards**
- **RoleArchetypes**
- **RubricDimensions**
- **Resumes**

---

## Excluded: MANY-Side Rollups

These aggregation fields from the rulebook cannot be created via OMNI. They require Airtable Rollup fields configured manually through the UI, or app-layer computation.

| Table | Field | Aggregates From | Description |
|-------|-------|-----------------|-------------|
| JobBoards | JobListings | JobListings (count where JobBoard matches) | Count of job listings scraped from this board |
| Resumes | ResumeSections | ResumeSections (count where Resume matches) | Count of sections extracted from this resume |
| Resumes | SearchRuns | SearchRuns (count where Resume matches) | Count of search runs that scored against this resume |
| SearchRuns | ScoreResults | ScoreResults (count where SearchRun matches) | Count of score results produced in this run |
| JobListings | ScoreResults | ScoreResults (count where JobListing matches) | Number of times this listing has been scored |
| JobListings | Decisions | Decisions (count where JobListing matches) | Number of decisions recorded for this listing |
