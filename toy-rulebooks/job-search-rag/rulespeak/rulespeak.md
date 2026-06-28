# 📘 Job Search RAG — RuleSpeak

_Local LLM + RAG pipeline filtering jobs across boards using semantic search._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Meaning (business sense) |
|------|--------------------------|
| **Job Board** | A job board tracked by the business. |
| Job Listings | Count of job listings scraped from this board. |
| **Search Url** | A search URL tracked by the business. |
| Job Board Name | Display name of the parent job board. |
| **Role Archetype** | A role archetype tracked by the business. |
| **Rubric Dimension** | A rubric dimension tracked by the business. |
| **Resume** | A resume tracked by the business. |
| Resume Sections | Count of sections (## heading chunks) extracted from this resume. |
| Search Runs | Count of search runs that scored against this resume. |
| **Resume Section** | A resume section tracked by the business. |
| Resume Name | Display name of the parent resume. |
| **Search Run** | A search run tracked by the business. |
| Resume Name | Display name of the resume used for this run. |
| Score Results | Count of score results produced in this run. |
| **Job Listing** | A job listing tracked by the business. |
| Job Board Name | Display name of the source job board. |
| Score Results | Number of times this listing has been scored across search runs. |
| Decisions | Number of decisions recorded for this listing. |
| **Score Result** | A score result tracked by the business. |
| Final Score | Weighted fusion of all component scores. Zeroed if disqualified. Formula: max(0, positive_sum - negative_weight * negative_score). |
| Job Listing Name | Title of the scored job listing. |
| Search Run Name | Label of the parent search run. |
| Company | Company name from the scored listing. |
| **Decision** | A decision tracked by the business. |
| Is Scoring Signal | Whether this verdict contributes to history_score. Only 'yes' verdicts are scoring signals. |
| Job Listing Name | Title of the decided job listing. |
| Company | Company name from the decided listing. |
| Board | Job board the decided listing came from. |

## 2 Fact Types

- a **search URL** references exactly one **job board**
- a **resume section** references exactly one **resume**
- a **search run** references exactly one **resume**
- a **job listing** references exactly one **job board**
- a **score result** references exactly one **job listing**
- a **score result** references exactly one **search run**
- a **decision** references exactly one **job listing**

## 3 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Job Listings** | A job board's job listings is the number of job listings related to the job board. |
| **DR-2 Job Board Name** | A search URL's job board name is the name of the search URL's job board. |
| **DR-3 Resume Sections** | A resume's resume sections is the number of resume sections related to the resume. |
| **DR-4 Search Runs** | A resume's search runs is the number of search runs related to the resume. |
| **DR-5 Resume Name** | A resume section's resume name is the name of the resume section's resume. |
| **DR-6 Resume Name** | A search run's resume name is the name of the search run's resume. |
| **DR-7 Score Results** | A search run's score results is the number of score results related to the search run. |
| **DR-8 Job Board Name** | A job listing's job board name is the name of the job listing's job board. |
| **DR-9 Score Results** | A job listing's score results is the number of score results related to the job listing. |
| **DR-10 Decisions** | A job listing's decisions is the number of decisions related to the job listing. |
| **DR-11 Final Score** | The score result's final score is determined by the following priority:<br>1. 0, if the is disqualified flag is set;<br>2. otherwise `Max(0, FitScore + ArchetypeScore + HistoryScore + CompScore + CultureScore - NegativeScore)`. |
| **DR-12 Job Listing Name** | A score result's job listing name is the name of the score result's job listing. |
| **DR-13 Search Run Name** | A score result's search run name is the name of the score result's search run. |
| **DR-14 Company** | A score result's company is the company of the score result's job listing. |
| **DR-15 Is Scoring Signal** | A decision is considered a scoring signal if `If(Verdict` is `"yes", True(), False())`. |
| **DR-16 Job Listing Name** | A decision's job listing name is the name of the decision's job listing. |
| **DR-17 Company** | A decision's company is the company of the decision's job listing. |
| **DR-18 Board** | A decision's board is the job board of the decision's job listing. |

## 4 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **JobBoards.JobListings** | rollup | `Count(JobListings via JobBoard)` |
| **SearchUrls.JobBoardName** | lookup | `Lookup(JobBoards.Name via JobBoard)` |
| **Resumes.ResumeSections** | rollup | `Count(ResumeSections via Resume)` |
| **Resumes.SearchRuns** | rollup | `Count(SearchRuns via Resume)` |
| **ResumeSections.ResumeName** | lookup | `Lookup(Resumes.Name via Resume)` |
| **SearchRuns.ResumeName** | lookup | `Lookup(Resumes.Name via Resume)` |
| **SearchRuns.ScoreResults** | rollup | `Count(ScoreResults via SearchRun)` |
| **JobListings.JobBoardName** | lookup | `Lookup(JobBoards.Name via JobBoard)` |
| **JobListings.ScoreResults** | rollup | `Count(ScoreResults via JobListing)` |
| **JobListings.Decisions** | rollup | `Count(Decisions via JobListing)` |
| **ScoreResults.FinalScore** | formula | `If(IsDisqualified, 0, Max(0, FitScore + ArchetypeScore + HistoryScore + CompScore + CultureScore - NegativeScore))` |
| **ScoreResults.JobListingName** | lookup | `Lookup(JobListings.Name via JobListing)` |
| **ScoreResults.SearchRunName** | lookup | `Lookup(SearchRuns.Name via SearchRun)` |
| **ScoreResults.Company** | lookup | `Lookup(JobListings.Company via JobListing)` |
| **Decisions.IsScoringSignal** | formula | `If(Verdict = "yes", True(), False())` |
| **Decisions.JobListingName** | lookup | `Lookup(JobListings.Name via JobListing)` |
| **Decisions.Company** | lookup | `Lookup(JobListings.Company via JobListing)` |
| **Decisions.Board** | lookup | `Lookup(JobListings.JobBoard via JobListing)` |
