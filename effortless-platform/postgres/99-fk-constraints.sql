-- ============================================================================
-- 99-fk-constraints.sql — FK CONSTRAINTS (off by default)
-- ============================================================================
-- Demos must never fail on FK violations, so init-db.sh SKIPS this file
-- unless EFFORTLESS_ENFORCE_FKS=true is set in the environment.
--
--   EFFORTLESS_ENFORCE_FKS=true bash init-db.sh    # apply constraints
--   bash init-db.sh                                # leave them documented but unenforced
--
-- The rulebook always documents the FK relationships, and 01-drop-and-create-tables.sql
-- always installs the supporting indexes inline. This file just declares the actual
-- enforcement. Idempotent: every constraint is dropped if present, then added.
-- ============================================================================

-- FlavorTags
ALTER TABLE flavor_tags DROP CONSTRAINT IF EXISTS fk_flavor_tags_flavor;
ALTER TABLE flavor_tags ADD CONSTRAINT fk_flavor_tags_flavor
  FOREIGN KEY (flavor) REFERENCES rulebook_flavors (flavor_id);
ALTER TABLE flavor_tags DROP CONSTRAINT IF EXISTS fk_flavor_tags_tag;
ALTER TABLE flavor_tags ADD CONSTRAINT fk_flavor_tags_tag
  FOREIGN KEY (tag) REFERENCES rulebook_tags (tag_id);

-- 2 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
