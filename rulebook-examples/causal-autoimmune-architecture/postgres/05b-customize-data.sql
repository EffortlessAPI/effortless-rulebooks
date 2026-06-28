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
-- v2 Step 3: force-update CausalMechanisms.TargetPathway (NEW RAW LEAF).
-- The main 05-insert-data.sql uses ON CONFLICT DO NOTHING, so a pre-existing
-- mechanism row keeps its old (NULL) target_pathway on reseed. This seam runs
-- AFTER the main insert and makes the leaf durable across reseeds. (See the
-- reseed-clobber gotcha: editing existing leaf rows in the rulebook is reverted
-- by ON CONFLICT DO NOTHING unless force-applied here.)
-- ============================================================================
UPDATE causal_mechanisms SET target_pathway = CASE causal_mechanism_id
  WHEN 'cm-a' THEN 'type-I-IFN'           WHEN 'cm-b' THEN 'type-I-IFN'
  WHEN 'cm-c' THEN 'B-cell/autoantibody'  WHEN 'cm-d' THEN 'B-cell/autoantibody'
  WHEN 'cm-e' THEN 'T-cell-costim'        WHEN 'cm-f' THEN 'IL-17/23'
  WHEN 'cm-g' THEN 'IL-17/23' END
WHERE causal_mechanism_id IN ('cm-a','cm-b','cm-c','cm-d','cm-e','cm-f','cm-g');

-- ============================================================================
-- v2 UI: promote the Cohort Discovery board up the left nav + rename it.
-- SortOrder/DisplayName are raw columns on a PRE-EXISTING routing row, so the
-- main 05-insert (ON CONFLICT DO NOTHING) keeps the old values on reseed. Force
-- them here (the preserved post-insert seam). Hub is the SSoT (91.5 / 'Cohort
-- discovery'); this just makes a reseeded DB match it.
-- ============================================================================
-- (relative_path is a CALCULATED column — derived from route — so it is not set here.)
UPDATE routing_and_navigation
   SET sort_order = 5, display_name = 'Cohort discovery', parent_route_key = '',
       nav_level = 'top', route = '/cohort',
       role_visibility = 'admin,diagnosing-doctor,intake-clinician'
 WHERE route_key = 'admin.cohort';

-- ============================================================================
-- v2 Step 10: expanded cohort (H..L as full claim-bearing members).
-- ind-h-yamamoto and ind-i-conteh PRE-EXISTED as Step-3 progression-only rows, so
-- the main 05-insert (ON CONFLICT DO NOTHING) keeps their stale values on reseed.
-- Force the raw leaves the clone+overrides set, so a reseed matches the hub:
--   * Ibrahim's cryptic flag (drives his keystone fail / 2nd disagreement)
--   * the new members' names/ancestry
--   * Kavya's replication ancestry pinned to her own (keeps transport FAIL)
--   * new mechanisms' target pathway
-- ============================================================================
UPDATE individuals SET given_name='Hana',    family_name='Yamamoto', ancestry_label='East Asian',       is_ancestry_absent_from_training=FALSE, has_cryptic_relatedness_flag=FALSE WHERE individual_id='ind-h-yamamoto';
UPDATE individuals SET given_name='Ibrahim', family_name='Conteh',   ancestry_label='African',          is_ancestry_absent_from_training=FALSE, has_cryptic_relatedness_flag=TRUE  WHERE individual_id='ind-i-conteh';
UPDATE individuals SET given_name='Jamal',   family_name='Brooks',   ancestry_label='Hispanic/Latino',  is_ancestry_absent_from_training=FALSE WHERE individual_id='ind-j-brooks';
UPDATE individuals SET given_name='Kavya',   family_name='Nair',     ancestry_label='South Asian',      is_ancestry_absent_from_training=TRUE  WHERE individual_id='ind-k-nair';
UPDATE individuals SET given_name='Lena',    family_name='Brandt',   ancestry_label='Indigenous American', is_ancestry_absent_from_training=TRUE WHERE individual_id='ind-l-brandt';

-- Kavya's replications pinned to her own ancestry so cross-ancestry count stays 0 (transport FAIL).
UPDATE cohort_replications SET replication_ancestry_label='South Asian' WHERE causal_mechanism='cm-k';

-- New mechanisms' target pathway (raw leaf).
UPDATE causal_mechanisms SET target_pathway = CASE causal_mechanism_id
  WHEN 'cm-h' THEN 'type-I-IFN' WHEN 'cm-i' THEN 'B-cell/autoantibody' WHEN 'cm-j' THEN 'type-I-IFN'
  WHEN 'cm-k' THEN 'IL-17/23'   WHEN 'cm-l' THEN 'IL-17/23' END
WHERE causal_mechanism_id IN ('cm-h','cm-i','cm-j','cm-k','cm-l');
