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

-- SymmetryGroups
ALTER TABLE symmetry_groups DROP CONSTRAINT IF EXISTS fk_symmetry_groups_notation;
ALTER TABLE symmetry_groups ADD CONSTRAINT fk_symmetry_groups_notation
  FOREIGN KEY (notation) REFERENCES symmetry_groups (symmetry_group_id);

-- Tilings
ALTER TABLE tilings DROP CONSTRAINT IF EXISTS fk_tilings_symmetry_group;
ALTER TABLE tilings ADD CONSTRAINT fk_tilings_symmetry_group
  FOREIGN KEY (symmetry_group) REFERENCES symmetry_groups (symmetry_group_id);

-- TilingPrototiles
ALTER TABLE tiling_prototiles DROP CONSTRAINT IF EXISTS fk_tiling_prototiles_tiling;
ALTER TABLE tiling_prototiles ADD CONSTRAINT fk_tiling_prototiles_tiling
  FOREIGN KEY (tiling) REFERENCES tilings (tiling_id);
ALTER TABLE tiling_prototiles DROP CONSTRAINT IF EXISTS fk_tiling_prototiles_prototile;
ALTER TABLE tiling_prototiles ADD CONSTRAINT fk_tiling_prototiles_prototile
  FOREIGN KEY (prototile) REFERENCES prototiles (prototile_id);

-- VertexFigures
ALTER TABLE vertex_figures DROP CONSTRAINT IF EXISTS fk_vertex_figures_tiling;
ALTER TABLE vertex_figures ADD CONSTRAINT fk_vertex_figures_tiling
  FOREIGN KEY (tiling) REFERENCES tilings (tiling_id);

-- Regions
ALTER TABLE regions DROP CONSTRAINT IF EXISTS fk_regions_target_tiling;
ALTER TABLE regions ADD CONSTRAINT fk_regions_target_tiling
  FOREIGN KEY (target_tiling) REFERENCES tilings (tiling_id);

-- Placements
ALTER TABLE placements DROP CONSTRAINT IF EXISTS fk_placements_region;
ALTER TABLE placements ADD CONSTRAINT fk_placements_region
  FOREIGN KEY (region) REFERENCES regions (region_id);
ALTER TABLE placements DROP CONSTRAINT IF EXISTS fk_placements_prototile;
ALTER TABLE placements ADD CONSTRAINT fk_placements_prototile
  FOREIGN KEY (prototile) REFERENCES prototiles (prototile_id);

-- WallpaperDesigns
ALTER TABLE wallpaper_designs DROP CONSTRAINT IF EXISTS fk_wallpaper_designs_symmetry_group;
ALTER TABLE wallpaper_designs ADD CONSTRAINT fk_wallpaper_designs_symmetry_group
  FOREIGN KEY (symmetry_group) REFERENCES symmetry_groups (symmetry_group_id);

-- 9 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
