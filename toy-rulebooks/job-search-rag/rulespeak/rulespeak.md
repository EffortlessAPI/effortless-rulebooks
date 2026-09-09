# 📘 Job Search RAG — RuleSpeak®

_Local LLM + RAG pipeline filtering jobs across boards using semantic search._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Job Board** | A job board is identified by its name. | — |
| Name | A defined attribute. | _Display name of the job board (e.g., ZipRecruiter, Indeed, LinkedIn, WeWorkRemotely)._ |
| Description | A defined attribute. | _Notes about this board's behavior, rate limits, or special handling requirements._ |
| Is Enabled | True when an empty string. | _Whether this board is active for search runs._ |
| Max Pages | A defined attribute. | _Maximum number of result pages to crawl per search URL. Default is 3._ |
| Is Headless | True when an empty string. | _Whether to run the Playwright browser in headless mode for this board._ |
| Browser Channel | A defined attribute. | _Optional browser variant to use (e.g., msedge). NULL means default Chromium._ |
| Job Listings | The number of job listings related to the job board. | _Count of job listings scraped from this board._ |
| **Search Url** | A search URL is identified by its name and is related to a job board. | — |
| Name | A defined attribute. | _Descriptive label for this search query (e.g., 'Staff Engineer - Remote')._ |
| Job Board | A defined attribute. | _Which job board this search URL belongs to._ |
| URL | A defined attribute. | _The full search URL to crawl on this board._ |
| Is Enabled | True when an empty string. | _Whether this search URL is active for crawling._ |
| Job Board Name | Taken from the linked job board. | _Display name of the parent job board._ |
| **Role Archetype** | A role archetype is identified by its name. | — |
| Name | A defined attribute. | _Archetype title (e.g., 'Staff Platform Architect', 'AI Systems Engineer')._ |
| Description | A defined attribute. | _Natural language description of this ideal role type. Embedded into ChromaDB for archetype scoring._ |
| Signals Positive | A defined attribute. | _Comma-separated keywords or phrases indicating strong fit for this archetype._ |
| Signals Negative | A defined attribute. | _Comma-separated keywords or phrases indicating misfit for this archetype._ |
| **Rubric Dimension** | A rubric dimension is identified by its name. | — |
| Name | A defined attribute. | _Dimension name (e.g., 'Role Scope', 'Technical Depth')._ |
| Description | A defined attribute. | _What this dimension evaluates and why it matters._ |
| Signals Positive | A defined attribute. | _Comma-separated positive indicators for this dimension. Embedded into global_positive_signals collection._ |
| Signals Negative | A defined attribute. | _Comma-separated red flags for this dimension. Embedded into negative_signals collection._ |
| **Resume** | A resume is identified by its name. | — |
| Name | A defined attribute. | _Display name for this resume version (e.g., 'Platform Engineer - 2026Q1')._ |
| Description | A defined attribute. | _Notes about what this resume version emphasizes or targets._ |
| File Path | A defined attribute. | _Path to the resume markdown file relative to project root (e.g., 'data/resume.md')._ |
| Indexed At | A defined attribute. | _When this resume was last chunked and embedded into ChromaDB. NULL means not yet indexed._ |
| Resume Sections | The number of resume sections related to the resume. | _Count of sections (## heading chunks) extracted from this resume._ |
| Search Runs | The number of search runs related to the resume. | _Count of search runs that scored against this resume._ |
| **Resume Section** | A resume section is identified by its name and is related to a resume. | — |
| Name | A defined attribute. | _Section heading text (e.g., 'Core Strengths', 'Experience')._ |
| Resume | A defined attribute. | _Which resume this section belongs to._ |
| Section Order | A defined attribute. | _Order of this section within the resume (1-based). Preserves document structure._ |
| Content | A defined attribute. | _Full text of this section including the ## heading and all body content up to the next ## heading. This is the document embedded into ChromaDB._ |
| Chroma Doc ID | A defined attribute. | _Document ID in the ChromaDB resume collection (e.g., 'resume-core-strengths'). Used for idempotent upserts._ |
| Resume Name | Taken from the linked resume. | _Display name of the parent resume._ |
| **Search Run** | A search run is identified by its name and is related to a resume. | — |
| Name | A defined attribute. | _Human-readable run label (typically date + time)._ |
| Resume | A defined attribute. | _Which resume was used for fit_score computation in this run. The central link in the RAG pipeline._ |
| Run Date | A defined attribute. | _When this search run was executed._ |
| Archetype Weight | A defined attribute. | _Weight applied to archetype match score (0.0 - 1.0)._ |
| Fit Weight | A defined attribute. | _Weight applied to resume fit score (0.0 - 1.0)._ |
| History Weight | A defined attribute. | _Weight applied to decision history score (0.0 - 1.0)._ |
| Comp Weight | A defined attribute. | _Weight applied to compensation alignment score (0.0 - 1.0)._ |
| Negative Weight | A defined attribute. | _Penalty multiplier for negative signal score (0.0 - 1.0)._ |
| Culture Weight | A defined attribute. | _Weight applied to culture/quality dimension score (0.0 - 1.0)._ |
| Base Salary | A defined attribute. | _Target annual salary for compensation scoring. Default $220,000._ |
| Min Score Threshold | A defined attribute. | _Minimum final score to include in results (0.0 - 1.0). Default 0.45._ |
| Disqualify on Llm Flag | True when an empty string. | _Whether to run the LLM disqualifier pass on listings._ |
| Total Found | A defined attribute. | _Total listings collected from boards in this run._ |
| Total Scored | A defined attribute. | _Total listings that passed through the scorer._ |
| Total Excluded | A defined attribute. | _Listings filtered below min_score_threshold._ |
| Total Deduplicated | A defined attribute. | _Duplicate listings collapsed during this run._ |
| Failed Listings | A defined attribute. | _Count of listings that failed scoring._ |
| Resume Name | Taken from the linked resume. | _Display name of the resume used for this run._ |
| Score Results | The number of score results related to the search run. | _Count of score results produced in this run._ |
| **Job Listing** | A job listing is identified by its name and is related to a job board. | — |
| Name | A defined attribute. | _Job title as listed on the board, sanitized for filesystem safety._ |
| Job Board | A defined attribute. | _Which job board this listing was scraped from._ |
| External ID | A defined attribute. | _The job board's unique identifier for this posting._ |
| Company | A defined attribute. | _Company name, sanitized for filesystem safety._ |
| Location | A defined attribute. | _Job location (city, state, remote, hybrid, etc.)._ |
| URL | A defined attribute. | _Direct URL link to the job posting on the board._ |
| Full Text | A defined attribute. | _Complete job description text. Max 250,000 characters. Embedded into ChromaDB for scoring._ |
| Posted At | A defined attribute. | _When the job was posted on the board._ |
| Comp Min | A defined attribute. | _Minimum compensation in USD, parsed from job description._ |
| Comp Max | A defined attribute. | _Maximum compensation in USD, parsed from job description._ |
| Comp Source | A defined attribute. | _Source of compensation data: 'employer' (from JD) or 'estimated' (board-generated)._ |
| Comp Text | A defined attribute. | _Raw compensation text matched from the job description (e.g., '$180,000 - $220,000/year')._ |
| Job Board Name | Taken from the linked job board. | _Display name of the source job board._ |
| Score Results | The number of score results related to the job listing. | _Number of times this listing has been scored across search runs._ |
| Decisions | The number of decisions related to the job listing. | _Number of decisions recorded for this listing._ |
| **Score Result** | A score result is identified by its name and is related to a job listing and a search run. | — |
| Name | Computed as the job listing name, followed by “ - ”, followed by the search run name. | _Display label combining job title and search run._ |
| Job Listing | A defined attribute. | _The job listing that was scored._ |
| Search Run | A defined attribute. | _The search run during which this score was produced._ |
| Fit Score | A defined attribute. | _Semantic similarity to resume (0.0 - 1.0). Based on cosine distance in resume ChromaDB collection._ |
| Archetype Score | A defined attribute. | _Match to role archetypes (0.0 - 1.0). Based on cosine distance in role_archetypes collection._ |
| History Score | A defined attribute. | _Match to past accepted decisions (0.0 - 1.0). Only 'yes' verdicts contribute._ |
| Comp Score | A defined attribute. | _Compensation alignment with base salary (0.0 - 1.0). Non-linear curve; 0.5 when comp is missing._ |
| Negative Score | A defined attribute. | _Penalty score from negative signal matches (0.0 - 1.0). Subtracted from positive scores._ |
| Culture Score | A defined attribute. | _Match to positive culture/quality dimensions (0.0 - 1.0)._ |
| Is Disqualified | True when an empty string. | _Whether the LLM disqualifier flagged this listing. If true, final score is forced to 0.0._ |
| Disqualifier Reason | A defined attribute. | _Free-text explanation from the LLM disqualifier about why the role was flagged._ |
| Final Score | Determined by priority: 0 if the disqualified flag is set; in all other cases, the largest of 0 and the fit score plus the archetype score plus the history score plus the comp score plus the culture score minus the negative score. | _Weighted fusion of all component scores. Zeroed if disqualified. Formula: max(0, positive_sum - negative_weight * negative_score)._ |
| Duplicate Boards | A defined attribute. | _Comma-separated list of other boards where this same job appeared (cross-board deduplication)._ |
| Job Listing Name | Taken from the linked job listing. | _Title of the scored job listing._ |
| Search Run Name | Taken from the linked search run. | _Label of the parent search run._ |
| Company | Taken from the linked job listing. | _Company name from the scored listing._ |
| **Decision** | A decision is identified by its name and is related to a job listing. | — |
| Name | Computed as the upper-cased verdict, followed by “: ”, followed by the job listing name. ⚠︎ mechanical <!-- rulespeak:reword --> | _Display label combining verdict and job title._ |
| Job Listing | A defined attribute. | _The job listing this decision is about._ |
| Verdict | A defined attribute. | _Operator's judgment: 'yes' (interested), 'no' (rejected), 'maybe' (uncertain), or 'removed' (retracted)._ |
| Reason | A defined attribute. | _Free-text explanation for the verdict. Stored for audit purposes._ |
| Recorded At | A defined attribute. | _Timestamp when this decision was recorded._ |
| Is Scoring Signal | True when the verdict is “yes”. | _Whether this verdict contributes to history_score. Only 'yes' verdicts are scoring signals._ |
| Job Listing Name | Taken from the linked job listing. | _Title of the decided job listing._ |
| Company | Taken from the linked job listing. | _Company name from the decided listing._ |
| Board | The job board of the decision's job listing. | _Job board the decided listing came from._ |

## 2 Fact Types

- a **search URL** references exactly one **job board**
- a **resume section** references exactly one **resume**
- a **search run** references exactly one **resume**
- a **job listing** references exactly one **job board**
- a **score result** references exactly one **job listing**
- a **score result** references exactly one **search run**
- a **decision** references exactly one **job listing**

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- A job board **must** have a name and a max pages, and record whether it is enabled and whether it is a headless.
- A search URL **must** reference exactly one job board.
- A search URL **must** have a name and a URL, and record whether it is enabled.
- A role archetype **must** have a name and a description.
- A rubric dimension **must** have a name.
- A resume **must** have a name and a file path.
- A resume section **must** reference exactly one resume.
- A resume section **must** have a name, a section order, a content, and a chroma doc ID.
- A search run **must** reference exactly one resume.
- A search run **must** have a name, a run date, an archetype weight, a fit weight, a history weight, a comp weight, a negative weight, a culture weight, a base salary, and a min score threshold, and record whether it is disqualify on llm flag.
- A job listing **must** reference exactly one job board.
- A job listing **must** have a name, an external ID, a company, a URL, and a full text.
- A score result **must** reference exactly one job listing.
- A score result **must** reference exactly one search run.
- A score result **must** have a fit score, an archetype score, a history score, a comp score, a negative score, and a culture score, and record whether it is disqualified.
- A decision **must** reference exactly one job listing.
- A decision **must** have a verdict and a recorded at.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Job Listings** | A job board's job listings is the number of job listings related to the job board. |
| **DR-2 Job Board Name** | A search URL's job board name — taken from the linked job board. |
| **DR-3 Resume Sections** | A resume's resume sections is the number of resume sections related to the resume. |
| **DR-4 Search Runs** | A resume's search runs is the number of search runs related to the resume. |
| **DR-5 Resume Name** | A resume section's resume name — taken from the linked resume. |
| **DR-6 Resume Name** | A search run's resume name — taken from the linked resume. |
| **DR-7 Score Results** | A search run's score results is the number of score results related to the search run. |
| **DR-8 Job Board Name** | A job listing's job board name — taken from the linked job board. |
| **DR-9 Score Results** | A job listing's score results is the number of score results related to the job listing. |
| **DR-10 Decisions** | A job listing's decisions is the number of decisions related to the job listing. |
| **DR-11 Name** | A score result's name is computed as the job listing name, followed by “ - ”, followed by the search run name. |
| **DR-12 Final Score** | The score result's final score is determined by the following priority:<br>1. 0, if the disqualified flag is set;<br>2. in all other cases, the largest of 0 and the fit score plus the archetype score plus the history score plus the comp score plus the culture score minus the negative score. |
| **DR-13 Job Listing Name** | A score result's job listing name — taken from the linked job listing. |
| **DR-14 Search Run Name** | A score result's search run name — taken from the linked search run. |
| **DR-15 Company** | A score result's company — taken from the linked job listing. |
| **DR-16 Name** | A decision's name is computed as the upper-cased verdict, followed by “: ”, followed by the job listing name. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-17 Is Scoring Signal** | A decision is considered a scoring signal if the verdict is “yes”. |
| **DR-18 Job Listing Name** | A decision's job listing name — taken from the linked job listing. |
| **DR-19 Company** | A decision's company — taken from the linked job listing. |
| **DR-20 Board** | A decision's board is the job board of the decision's job listing. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak® notation —
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
| **ScoreResults.Name** | formula | `Concat(JobListingName, " - ", SearchRunName)` |
| **ScoreResults.FinalScore** | formula | `If(IsDisqualified, 0, Max(0, FitScore + ArchetypeScore + HistoryScore + CompScore + CultureScore - NegativeScore))` |
| **ScoreResults.JobListingName** | lookup | `Lookup(JobListings.Name via JobListing)` |
| **ScoreResults.SearchRunName** | lookup | `Lookup(SearchRuns.Name via SearchRun)` |
| **ScoreResults.Company** | lookup | `Lookup(JobListings.Company via JobListing)` |
| **Decisions.Name** | formula | `Concat(Upper(Verdict), ": ", JobListingName)` |
| **Decisions.IsScoringSignal** | formula | `If(Verdict = "yes", True(), False())` |
| **Decisions.JobListingName** | lookup | `Lookup(JobListings.Name via JobListing)` |
| **Decisions.Company** | lookup | `Lookup(JobListings.Company via JobListing)` |
| **Decisions.Board** | lookup | `Lookup(JobListings.JobBoard via JobListing)` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak® and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
