# OMNI Prompt — Part 1: Tables, Raw Fields & Linked Records

> **Instructions for OMNI:** Create each table below with the listed fields.
> Run this prompt **one table at a time**, in the order listed (parent tables first).
> Do NOT create any Formula, Lookup, or Rollup fields in this part.

---

## Table: JobBoards

**Description:** Configured job board sources that can be crawled for listings.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "ZipRecruiter").

### Fields

- **Name** (Single line text): Display name of the job board.
- **Description** (Long text): Notes about this board's behavior, rate limits, or special handling requirements.
- **IsEnabled** (Checkbox): Whether this board is active for search runs.
- **MaxPages** (Number, integer): Maximum number of result pages to crawl per search URL. Default is 3.
- **IsHeadless** (Checkbox): Whether to run the Playwright browser in headless mode for this board.
- **BrowserChannel** (Single line text): Optional browser variant to use (e.g., msedge). Leave blank for default Chromium.

---

## Table: SearchUrls

**Description:** Individual search URLs configured per board, each representing a specific query to crawl.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "Staff Platform Architect - Remote").

### Fields

- **Name** (Single line text): Descriptive label for this search query.
- **JobBoard** (Link to another record -> JobBoards): Which job board this search URL belongs to.
- **Url** (URL): The full search URL to crawl on this board.
- **IsEnabled** (Checkbox): Whether this search URL is active for crawling.

---

## Table: RoleArchetypes

**Description:** Target role descriptions that define ideal job types for semantic matching.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "Staff Platform Architect").

### Fields

- **Name** (Single line text): Archetype title.
- **Description** (Long text): Natural language description of this ideal role type. Embedded into ChromaDB for archetype scoring.
- **SignalsPositive** (Long text): Comma-separated keywords or phrases indicating strong fit for this archetype.
- **SignalsNegative** (Long text): Comma-separated keywords or phrases indicating misfit for this archetype.

---

## Table: RubricDimensions

**Description:** Universal evaluation criteria applied to all listings for culture and quality scoring.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "Role Scope").

### Fields

- **Name** (Single line text): Dimension name.
- **Description** (Long text): What this dimension evaluates and why it matters.
- **SignalsPositive** (Long text): Comma-separated positive indicators for this dimension.
- **SignalsNegative** (Long text): Comma-separated red flags for this dimension.

---

## Table: Resumes

**Description:** Candidate resumes that anchor the entire RAG scoring pipeline. Every search run scores job listings against a specific resume's embeddings.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "Platform Engineer - 2026 Q1").

### Fields

- **Name** (Single line text): Display name for this resume version.
- **Description** (Long text): Notes about what this resume version emphasizes or targets.
- **FilePath** (Single line text): Path to the resume markdown file relative to project root.
- **IndexedAt** (Date, include time): When this resume was last chunked and embedded into ChromaDB. Blank means not yet indexed.

---

## Table: ResumeSections

**Description:** Individual chunks of a resume, split by ## headings. Each section is embedded separately into ChromaDB for granular semantic matching against job descriptions.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "Summary").

### Fields

- **Name** (Single line text): Section heading text.
- **Resume** (Link to another record -> Resumes): Which resume this section belongs to.
- **SectionOrder** (Number, integer): Order of this section within the resume (1-based).
- **Content** (Long text): Full text of this section including the ## heading and all body content.
- **ChromaDocId** (Single line text): Document ID in the ChromaDB resume collection. Used for idempotent upserts.

---

## Table: SearchRuns

**Description:** Individual pipeline execution runs that crawl boards, score listings, and produce ranked results. Each run scores against a specific resume.

**Name formula:** Leave Name as a Single line text field (entered manually, e.g. "2026-04-01 07:45").

### Fields

- **Name** (Single line text): Human-readable run label (typically date + time).
- **Resume** (Link to another record -> Resumes): Which resume was used for fit_score computation in this run.
- **RunDate** (Date, include time): When this search run was executed.
- **ArchetypeWeight** (Number, decimal): Weight applied to archetype match score (0.0 - 1.0).
- **FitWeight** (Number, decimal): Weight applied to resume fit score (0.0 - 1.0).
- **HistoryWeight** (Number, decimal): Weight applied to decision history score (0.0 - 1.0).
- **CompWeight** (Number, decimal): Weight applied to compensation alignment score (0.0 - 1.0).
- **NegativeWeight** (Number, decimal): Penalty multiplier for negative signal score (0.0 - 1.0).
- **CultureWeight** (Number, decimal): Weight applied to culture/quality dimension score (0.0 - 1.0).
- **BaseSalary** (Number, decimal): Target annual salary for compensation scoring.
- **MinScoreThreshold** (Number, decimal): Minimum final score to include in results (0.0 - 1.0).
- **DisqualifyOnLlmFlag** (Checkbox): Whether to run the LLM disqualifier pass on listings.
- **TotalFound** (Number, integer): Total listings collected from boards in this run.
- **TotalScored** (Number, integer): Total listings that passed through the scorer.
- **TotalExcluded** (Number, integer): Listings filtered below min_score_threshold.
- **TotalDeduplicated** (Number, integer): Duplicate listings collapsed during this run.
- **FailedListings** (Number, integer): Count of listings that failed scoring.

---

## Table: JobListings

**Description:** Individual job postings scraped from job boards, normalized to a common data contract.

**Name formula:** Leave Name as a Single line text field (entered manually — the job title as listed on the board).

### Fields

- **Name** (Single line text): Job title as listed on the board.
- **JobBoard** (Link to another record -> JobBoards): Which job board this listing was scraped from.
- **ExternalId** (Single line text): The job board's unique identifier for this posting.
- **Company** (Single line text): Company name.
- **Location** (Single line text): Job location (city, state, remote, hybrid, etc.).
- **Url** (URL): Direct URL link to the job posting on the board.
- **FullText** (Long text): Complete job description text.
- **PostedAt** (Date, include time): When the job was posted on the board.
- **CompMin** (Number, decimal): Minimum compensation in USD, parsed from job description.
- **CompMax** (Number, decimal): Maximum compensation in USD, parsed from job description.
- **CompSource** (Single select: employer, estimated): Source of compensation data.
- **CompText** (Single line text): Raw compensation text matched from the job description.

---

## Table: ScoreResults

**Description:** Component and final scores for a job listing within a specific search run.

**Name formula:** This will be set as a formula in Part 2 (depends on lookups). For now, leave Name as a Single line text field.

### Fields

- **Name** (Single line text): Temporary placeholder — will become a formula in Part 2.
- **JobListing** (Link to another record -> JobListings): The job listing that was scored.
- **SearchRun** (Link to another record -> SearchRuns): The search run during which this score was produced.
- **FitScore** (Number, decimal): Semantic similarity to resume (0.0 - 1.0).
- **ArchetypeScore** (Number, decimal): Match to role archetypes (0.0 - 1.0).
- **HistoryScore** (Number, decimal): Match to past accepted decisions (0.0 - 1.0).
- **CompScore** (Number, decimal): Compensation alignment with base salary (0.0 - 1.0).
- **NegativeScore** (Number, decimal): Penalty score from negative signal matches (0.0 - 1.0).
- **CultureScore** (Number, decimal): Match to positive culture/quality dimensions (0.0 - 1.0).
- **IsDisqualified** (Checkbox): Whether the LLM disqualifier flagged this listing.
- **DisqualifierReason** (Long text): Free-text explanation from the LLM disqualifier.
- **DuplicateBoards** (Single line text): Comma-separated list of other boards where this same job appeared.

---

## Table: Decisions

**Description:** Operator verdicts on reviewed job listings, used as training signal for future scoring.

**Name formula:** This will be set as a formula in Part 2 (depends on lookups). For now, leave Name as a Single line text field.

### Fields

- **Name** (Single line text): Temporary placeholder — will become a formula in Part 2.
- **JobListing** (Link to another record -> JobListings): The job listing this decision is about.
- **Verdict** (Single select: yes, no, maybe, removed): Operator's judgment on this role.
- **Reason** (Long text): Free-text explanation for the verdict.
- **RecordedAt** (Date, include time): Timestamp when this decision was recorded.
