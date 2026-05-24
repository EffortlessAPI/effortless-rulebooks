-- ============================================================================
-- CUSTOMIZE SCHEMA - User-defined schema customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main schema script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom schema changes will appear here:

-- ============================================================================
-- Resumes & ResumeSections — the central entity the RAG pipeline scores against
-- ============================================================================

CREATE TABLE IF NOT EXISTS resumes (
  resume_id                           TEXT                 PRIMARY KEY,
  name                                TEXT                ,
  description                         TEXT                ,
  file_path                           TEXT                ,
  indexed_at                          TIMESTAMPTZ
);
COMMENT ON TABLE resumes IS 'Table: Resumes - Candidate resumes that anchor the entire RAG scoring pipeline. Every search run scores listings against a specific resume.';
COMMENT ON COLUMN resumes.resume_id IS 'Primary key for resumes.';
COMMENT ON COLUMN resumes.name IS 'Display name for this resume version (e.g., Platform Engineer - 2026Q1).';
COMMENT ON COLUMN resumes.description IS 'Notes about what this resume version emphasizes or targets.';
COMMENT ON COLUMN resumes.file_path IS 'Path to the resume markdown file relative to project root.';
COMMENT ON COLUMN resumes.indexed_at IS 'When this resume was last chunked and embedded into ChromaDB. NULL means not yet indexed.';

CREATE TABLE IF NOT EXISTS resume_sections (
  resume_section_id                   TEXT                 PRIMARY KEY,
  name                                TEXT                ,
  resume                              TEXT                ,
  section_order                       INTEGER             ,
  content                             TEXT                ,
  chroma_doc_id                       TEXT
);
COMMENT ON TABLE resume_sections IS 'Table: ResumeSections - Individual chunks of a resume, split by ## headings. Each section is embedded separately into ChromaDB for granular semantic matching.';
COMMENT ON COLUMN resume_sections.resume_section_id IS 'Primary key. Slug of the section heading.';
COMMENT ON COLUMN resume_sections.name IS 'Section heading text (e.g., Core Strengths, Experience).';
COMMENT ON COLUMN resume_sections.resume IS 'Which resume this section belongs to.';
COMMENT ON COLUMN resume_sections.section_order IS 'Order of this section within the resume (1-based).';
COMMENT ON COLUMN resume_sections.content IS 'Full text of this section including the ## heading. This is the document embedded into ChromaDB.';
COMMENT ON COLUMN resume_sections.chroma_doc_id IS 'Document ID in the ChromaDB resume collection. Used for idempotent upserts.';

-- Add resume FK to search_runs
ALTER TABLE search_runs ADD COLUMN IF NOT EXISTS resume TEXT;

