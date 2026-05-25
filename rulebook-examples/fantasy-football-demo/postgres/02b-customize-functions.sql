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

-- Override calc_standings_wins to properly count wins
CREATE OR REPLACE FUNCTION calc_standings_wins(p_standing_id TEXT)
RETURNS INTEGER AS $$
  SELECT (
    (SELECT COUNT(*) FROM matchups m
     WHERE (m.team1 = p_standing_id AND calc_matchups_team1_score(m.matchup_id) > calc_matchups_team2_score(m.matchup_id))
        OR (m.team2 = p_standing_id AND calc_matchups_team2_score(m.matchup_id) > calc_matchups_team1_score(m.matchup_id)))
  )::integer;
$$ LANGUAGE sql STABLE;

-- Override calc_standings_losses to properly count losses
CREATE OR REPLACE FUNCTION calc_standings_losses(p_standing_id TEXT)
RETURNS INTEGER AS $$
  SELECT (
    (SELECT COUNT(*) FROM matchups m
     WHERE (m.team1 = p_standing_id AND calc_matchups_team1_score(m.matchup_id) < calc_matchups_team2_score(m.matchup_id))
        OR (m.team2 = p_standing_id AND calc_matchups_team2_score(m.matchup_id) < calc_matchups_team1_score(m.matchup_id)))
  )::integer;
$$ LANGUAGE sql STABLE;

-- Override calc_standings_ties to properly count only actual tie games
CREATE OR REPLACE FUNCTION calc_standings_ties(p_standing_id TEXT)
RETURNS INTEGER AS $$
  SELECT (
    (SELECT COUNT(*) FROM matchups m
     WHERE (m.team1 = p_standing_id OR m.team2 = p_standing_id)
       AND calc_matchups_team1_score(m.matchup_id) = calc_matchups_team2_score(m.matchup_id))
  )::integer;
$$ LANGUAGE sql STABLE;

-- Override calc_standings_seed_rank to properly rank by win_pct (descending)
CREATE OR REPLACE FUNCTION calc_standings_seed_rank(p_standing_id TEXT)
RETURNS INTEGER AS $$
  SELECT RANK() OVER (ORDER BY calc_standings_win_pct(standing_id) DESC)::integer
  FROM standings
  WHERE standing_id = p_standing_id;
$$ LANGUAGE sql STABLE;
