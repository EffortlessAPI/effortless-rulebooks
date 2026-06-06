-- ============================================================================
-- CUSTOMIZE FUNCTIONS - User-defined functions customizations
-- ============================================================================
-- This file is for YOUR custom changes that should persist across
-- regeneration of the base ERB files.
--
-- IMPORTANT:
--   - This file runs AFTER the main functions script
--   - Define your customizations in the ERBCustomizations table in Airtable
--   - Those changes will appear here after the next build
--
-- ============================================================================

-- Your custom functions changes will appear here:

-- ----------------------------------------------------------------------------
-- calc_prototiles_area  —  regular-polygon area (needs trigonometry)
-- ----------------------------------------------------------------------------
-- Rulebook formula:  =0.25*{{Sides}}*POWER({{EdgeLength}},2)/TAN(PI()/{{Sides}})
-- i.e. area of a regular n-gon with edge s  =  ¼·n·s²·cot(π/n).
-- TAN()/PI() are outside the transpiler's portable formula vocabulary, so the
-- generated 02-create-functions.sql stub returns NULL. This override runs AFTER
-- 02 and CREATE OR REPLACEs that stub with the real implementation using Postgres
-- native tan()/pi(). This is the sanctioned 02b math seam — the rulebook still
-- declares Area as the SSoT calculated field; only its evaluator lives here.
CREATE OR REPLACE FUNCTION calc_prototiles_area(p_prototile_id TEXT)
RETURNS NUMERIC AS $$
  SELECT 0.25 * sides * power(edge_length, 2) / tan(pi() / sides)
  FROM prototiles
  WHERE prototile_id = p_prototile_id;
$$ LANGUAGE sql STABLE;

-- ----------------------------------------------------------------------------
-- calc_tilings_valid_vertex_figure_count  —  two-criterion COUNTIFS
-- ----------------------------------------------------------------------------
-- Rulebook formula:
--   =COUNTIFS(VertexFigures!{{Tiling}}, Tilings!{{TilingId}},
--             VertexFigures!{{IsValid}}, TRUE)
-- The generated 02 stub only emitted the FIRST criterion pair (tiling = id) and
-- silently dropped the second (is_valid = TRUE), so it counted ALL vertex
-- figures, not just the valid ones. That was invisible while every catalogued
-- tiling happened to have only valid vertex figures; the regular-pentagon
-- negative case (vertex figure sums to 324deg, is_valid = false) exposed it.
-- This override runs AFTER 02 and CREATE OR REPLACEs the stub with the real
-- two-criterion count. The rulebook formula remains the SSoT; only its
-- evaluator is corrected here (same sanctioned 02b seam as calc_prototiles_area).
-- IsValid (and therefore AllVerticesValid -> DoesTilePlane) now resolves
-- correctly: the pentagon attempt reports does_tile_plane = false.
CREATE OR REPLACE FUNCTION calc_tilings_valid_vertex_figure_count(p_tiling_id TEXT)
RETURNS INTEGER AS $$
  SELECT COUNT(*)::integer
  FROM vertex_figures vf
  WHERE vf.tiling = p_tiling_id
    AND calc_vertex_figures_is_valid(vf.vertex_figure_id) = TRUE;
$$ LANGUAGE sql STABLE;

