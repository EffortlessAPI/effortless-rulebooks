-- ============================================================================
-- CUSTOMIZE DATA - User-defined data customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main data script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom data changes will appear here:

-- ============================================================================
-- MOCK DATA — exercises the main business rules
--
-- Rules covered:
--   1. Disqualified listing → final_score forced to 0.0
--   2. Only "yes" decisions → is_scoring_signal = true
--   3. Comp scoring: above target, below target, missing (neutral 0.5)
--   4. Cross-board deduplication (same job on two boards)
--   5. High negative_score subtracts from positives
--   6. Score threshold: some above, some below 0.45
--   7. Aggregation counts: listings per board, scores per run, decisions per listing
--   8. Resume is the anchor — search runs reference a specific resume
--   9. Resume sections are chunked by ## headings for granular embedding
-- ============================================================================

-- ── Resumes (the central entity — everything scores against this) ───────────

INSERT INTO resumes (resume_id, name, description, file_path, indexed_at) VALUES
  ('resume-platform-2026q1',
   'Platform Engineer - 2026 Q1',
   'Emphasizes distributed systems, platform architecture, and AI/ML infrastructure. Targets staff+ IC and architect roles.',
   'data/resume.md',
   '2026-04-01T07:45:00Z')
ON CONFLICT DO NOTHING;

-- ── ResumeSections (4 chunks split by ## headings — the documents in ChromaDB) ─

INSERT INTO resume_sections (resume_section_id, name, resume, section_order, content, chroma_doc_id) VALUES
  ('rs-summary', 'Summary', 'resume-platform-2026q1', 1,
   '## Summary
Senior platform engineer with 15+ years building distributed systems, data pipelines, and AI/ML infrastructure. Track record of cross-team technical leadership at scale.',
   'resume-summary')
ON CONFLICT DO NOTHING;

INSERT INTO resume_sections (resume_section_id, name, resume, section_order, content, chroma_doc_id) VALUES
  ('rs-core-strengths', 'Core Strengths', 'resume-platform-2026q1', 2,
   '## Core Strengths
- Distributed systems architecture (Kubernetes, gRPC, service mesh)
- ML/AI platform engineering (model serving, training pipelines, vector stores)
- Technical strategy and mentorship across 50+ engineer orgs
- Data infrastructure (Kafka, Spark, Flink, dbt)',
   'resume-core-strengths')
ON CONFLICT DO NOTHING;

INSERT INTO resume_sections (resume_section_id, name, resume, section_order, content, chroma_doc_id) VALUES
  ('rs-experience', 'Experience', 'resume-platform-2026q1', 3,
   '## Experience
### Staff Platform Engineer — CloudScale Inc (2022-2026)
Led platform architecture for ML serving infrastructure. Designed multi-region Kubernetes deployment strategy. Reduced model inference latency 40%.
### Senior Engineer — DataForge (2018-2022)
Built real-time data pipelines processing 2M events/sec. Migrated monolith to event-driven microservices.',
   'resume-experience')
ON CONFLICT DO NOTHING;

INSERT INTO resume_sections (resume_section_id, name, resume, section_order, content, chroma_doc_id) VALUES
  ('rs-earlier-roles', 'Earlier Roles', 'resume-platform-2026q1', 4,
   '## Earlier Roles
### Software Engineer — StartupCo (2014-2018)
Full-stack development, API design, early DevOps adoption.
### Junior Developer — Agency LLC (2011-2014)
Web application development, client projects, learned software engineering fundamentals.',
   'resume-earlier-roles')
ON CONFLICT DO NOTHING;

-- ── SearchUrls (3 URLs across 2 boards) ────────────────────────────────────

INSERT INTO search_urls (search_url_id, name, job_board, url, is_enabled) VALUES
  ('su-zr-staff',    'Staff Engineer - Remote',    'ziprecruiter',  'https://www.ziprecruiter.com/jobs/staff+engineer+remote', TRUE),
  ('su-zr-platform', 'Platform Architect',         'ziprecruiter',  'https://www.ziprecruiter.com/jobs/platform+architect',    TRUE),
  ('su-wwr-senior',  'Senior Engineer - Remote',   'weworkremotely','https://weworkremotely.com/remote-jobs/search?term=senior+engineer', TRUE)
ON CONFLICT DO NOTHING;

-- ── SearchRuns (1 run with standard weights) ────────────────────────────────

INSERT INTO search_runs (
  search_run_id, name, resume, run_date,
  archetype_weight, fit_weight, history_weight, comp_weight, negative_weight, culture_weight,
  base_salary, min_score_threshold, disqualify_on_llm_flag,
  total_found, total_scored, total_excluded, total_deduplicated, failed_listings
) VALUES (
  'run-2026-04-01', '2026-04-01 Morning Run', 'resume-platform-2026q1', '2026-04-01T08:00:00Z',
  0.25, 0.25, 0.15, 0.15, 0.10, 0.10,
  220000, 0.45, TRUE,
  6, 6, 1, 1, 0
)
ON CONFLICT DO NOTHING;

-- ── JobListings (6 listings covering all scoring scenarios) ─────────────────

-- 1. Strong match: high scores, comp at target → top ranked
INSERT INTO job_listings (
  job_listing_id, name, job_board, external_id, company, location, url, full_text,
  posted_at, comp_min, comp_max, comp_source, comp_text
) VALUES (
  'ziprecruiter-a1', 'Staff Platform Engineer', 'ziprecruiter', 'a1',
  'Acme Cloud', 'Remote', 'https://ziprecruiter.com/jobs/a1',
  'Staff Platform Engineer at Acme Cloud. Build distributed systems, design platform architecture, mentor teams. Kubernetes, Go, gRPC. Direct hire, full-time with equity.',
  '2026-03-28', 200000, 240000, 'employer', '$200,000 - $240,000/year'
)
ON CONFLICT DO NOTHING;

-- 2. Disqualified: good component scores but LLM flags it (staffing agency buried in JD)
INSERT INTO job_listings (
  job_listing_id, name, job_board, external_id, company, location, url, full_text,
  posted_at, comp_min, comp_max, comp_source, comp_text
) VALUES (
  'ziprecruiter-b2', 'Principal ML Engineer', 'ziprecruiter', 'b2',
  'TechVentures Staffing', 'New York, NY', 'https://ziprecruiter.com/jobs/b2',
  'Principal ML Engineer. Build training pipelines, model serving infrastructure. W2 through TechVentures Staffing on behalf of unnamed Fortune 500 client. 6-month contract-to-hire.',
  '2026-03-29', 190000, 230000, 'employer', '$190,000 - $230,000/year'
)
ON CONFLICT DO NOTHING;

-- 3. Below threshold: junior role, seniority mismatch, low scores
INSERT INTO job_listings (
  job_listing_id, name, job_board, external_id, company, location, url, full_text,
  posted_at, comp_min, comp_max, comp_source, comp_text
) VALUES (
  'ziprecruiter-c3', 'Junior Frontend Developer', 'ziprecruiter', 'c3',
  'SmallShop Inc', 'Austin, TX', 'https://ziprecruiter.com/jobs/c3',
  'Junior Frontend Developer. Build landing pages with WordPress and React. Entry-level, 0-2 years experience. Basic CRUD applications.',
  '2026-03-30', 55000, 75000, 'employer', '$55,000 - $75,000/year'
)
ON CONFLICT DO NOTHING;

-- 4. Missing comp: good role but no salary → neutral comp_score (0.5)
INSERT INTO job_listings (
  job_listing_id, name, job_board, external_id, company, location, url, full_text,
  posted_at, comp_min, comp_max, comp_source, comp_text
) VALUES (
  'ziprecruiter-d4', 'AI Infrastructure Lead', 'ziprecruiter', 'd4',
  'DeepTech AI', 'San Francisco, CA (Hybrid)', 'https://ziprecruiter.com/jobs/d4',
  'AI Infrastructure Lead at DeepTech AI. Own ML training pipelines, GPU cluster orchestration, model serving platform. Cross-team leadership, distributed systems at scale.',
  '2026-03-30', NULL, NULL, NULL, NULL
)
ON CONFLICT DO NOTHING;

-- 5. Cross-board duplicate: same job appears on WeWorkRemotely (lower score kept for dedup)
INSERT INTO job_listings (
  job_listing_id, name, job_board, external_id, company, location, url, full_text,
  posted_at, comp_min, comp_max, comp_source, comp_text
) VALUES (
  'weworkremotely-e5', 'Staff Platform Engineer', 'weworkremotely', 'e5',
  'Acme Cloud', 'Remote', 'https://weworkremotely.com/jobs/e5',
  'Staff Platform Engineer at Acme Cloud. Build distributed systems, design platform architecture, mentor teams. Kubernetes, Go, gRPC. Direct hire, full-time with equity.',
  '2026-03-27', 200000, 240000, 'estimated', '$200,000 - $240,000/year'
)
ON CONFLICT DO NOTHING;

-- 6. High negative score: adtech + 24/7 on-call → negatives drag down positives
INSERT INTO job_listings (
  job_listing_id, name, job_board, external_id, company, location, url, full_text,
  posted_at, comp_min, comp_max, comp_source, comp_text
) VALUES (
  'ziprecruiter-f6', 'Senior Data Engineer', 'ziprecruiter', 'f6',
  'AdTrack Global', 'Remote', 'https://ziprecruiter.com/jobs/f6',
  'Senior Data Engineer at AdTrack Global. Adtech real-time bidding platform. 24/7 on-call rotation, pager duty primary. Surveillance-grade user tracking. Kafka, Spark, Flink.',
  '2026-03-31', 180000, 210000, 'employer', '$180,000 - $210,000/year'
)
ON CONFLICT DO NOTHING;

-- ── ScoreResults (1 per listing, exercising all scoring rules) ──────────────

-- 1. Strong match: all high scores, not disqualified → high final_score
INSERT INTO score_results (
  score_result_id, job_listing, search_run,
  fit_score, archetype_score, history_score, comp_score, negative_score, culture_score,
  is_disqualified, disqualifier_reason, duplicate_boards
) VALUES (
  'sr-a1', 'ziprecruiter-a1', 'run-2026-04-01',
  0.88, 0.92, 0.75, 0.95, 0.05, 0.85,
  FALSE, NULL, NULL
)
ON CONFLICT DO NOTHING;

-- 2. Disqualified: good scores BUT final_score → 0.0 (RULE: disqualified zeroes score)
INSERT INTO score_results (
  score_result_id, job_listing, search_run,
  fit_score, archetype_score, history_score, comp_score, negative_score, culture_score,
  is_disqualified, disqualifier_reason, duplicate_boards
) VALUES (
  'sr-b2', 'ziprecruiter-b2', 'run-2026-04-01',
  0.82, 0.78, 0.65, 0.85, 0.15, 0.60,
  TRUE, 'Staffing agency (W2 through third party). Contract-to-hire, not direct employment.', NULL
)
ON CONFLICT DO NOTHING;

-- 3. Below threshold: low scores → final_score < 0.45 (RULE: filtered from results)
INSERT INTO score_results (
  score_result_id, job_listing, search_run,
  fit_score, archetype_score, history_score, comp_score, negative_score, culture_score,
  is_disqualified, disqualifier_reason, duplicate_boards
) VALUES (
  'sr-c3', 'ziprecruiter-c3', 'run-2026-04-01',
  0.15, 0.10, 0.05, 0.00, 0.80, 0.10,
  FALSE, NULL, NULL
)
ON CONFLICT DO NOTHING;

-- 4. Missing comp: comp_score = 0.5 neutral (RULE: missing comp → 0.5)
INSERT INTO score_results (
  score_result_id, job_listing, search_run,
  fit_score, archetype_score, history_score, comp_score, negative_score, culture_score,
  is_disqualified, disqualifier_reason, duplicate_boards
) VALUES (
  'sr-d4', 'ziprecruiter-d4', 'run-2026-04-01',
  0.90, 0.85, 0.70, 0.50, 0.08, 0.82,
  FALSE, NULL, NULL
)
ON CONFLICT DO NOTHING;

-- 5. Cross-board dup: same job as a1, marked with duplicate_boards (RULE: dedup keeps highest)
INSERT INTO score_results (
  score_result_id, job_listing, search_run,
  fit_score, archetype_score, history_score, comp_score, negative_score, culture_score,
  is_disqualified, disqualifier_reason, duplicate_boards
) VALUES (
  'sr-e5', 'weworkremotely-e5', 'run-2026-04-01',
  0.87, 0.91, 0.74, 0.90, 0.05, 0.84,
  FALSE, NULL, 'ziprecruiter'
)
ON CONFLICT DO NOTHING;

-- 6. High negative: decent positives but negatives drag it down (RULE: negative subtracted)
INSERT INTO score_results (
  score_result_id, job_listing, search_run,
  fit_score, archetype_score, history_score, comp_score, negative_score, culture_score,
  is_disqualified, disqualifier_reason, duplicate_boards
) VALUES (
  'sr-f6', 'ziprecruiter-f6', 'run-2026-04-01',
  0.65, 0.55, 0.40, 0.80, 0.75, 0.20,
  FALSE, NULL, NULL
)
ON CONFLICT DO NOTHING;

-- ── Decisions (3 verdicts exercising is_scoring_signal rule) ─────────────────

-- "yes" verdict → is_scoring_signal = TRUE (RULE: only yes feeds history_score)
INSERT INTO decisions (decision_id, job_listing, verdict, reason, recorded_at) VALUES
  ('dec-a1-yes', 'ziprecruiter-a1', 'yes',
   'Perfect archetype match. Direct hire, strong comp, platform scope.',
   '2026-04-01T09:15:00Z')
ON CONFLICT DO NOTHING;

-- "no" verdict → is_scoring_signal = FALSE
INSERT INTO decisions (decision_id, job_listing, verdict, reason, recorded_at) VALUES
  ('dec-b2-no', 'ziprecruiter-b2', 'no',
   'Staffing agency. Contract-to-hire is a dealbreaker.',
   '2026-04-01T09:20:00Z')
ON CONFLICT DO NOTHING;

-- "maybe" verdict → is_scoring_signal = FALSE
INSERT INTO decisions (decision_id, job_listing, verdict, reason, recorded_at) VALUES
  ('dec-d4-maybe', 'ziprecruiter-d4', 'maybe',
   'Great role but hybrid in SF. Need to check if fully remote is negotiable.',
   '2026-04-01T09:25:00Z')
ON CONFLICT DO NOTHING;

