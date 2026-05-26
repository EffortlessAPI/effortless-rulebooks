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

-- Seasons
ALTER TABLE seasons DROP CONSTRAINT IF EXISTS fk_seasons_series;
ALTER TABLE seasons ADD CONSTRAINT fk_seasons_series
  FOREIGN KEY (series) REFERENCES series (serie_id);

-- Episodes
ALTER TABLE episodes DROP CONSTRAINT IF EXISTS fk_episodes_season;
ALTER TABLE episodes ADD CONSTRAINT fk_episodes_season
  FOREIGN KEY (season) REFERENCES seasons (season_id);
ALTER TABLE episodes DROP CONSTRAINT IF EXISTS fk_episodes_ratings;
ALTER TABLE episodes ADD CONSTRAINT fk_episodes_ratings
  FOREIGN KEY (ratings) REFERENCES ratings (rating_id);

-- Ratings
ALTER TABLE ratings DROP CONSTRAINT IF EXISTS fk_ratings_series;
ALTER TABLE ratings ADD CONSTRAINT fk_ratings_series
  FOREIGN KEY (series) REFERENCES series (serie_id);
ALTER TABLE ratings DROP CONSTRAINT IF EXISTS fk_ratings_episode;
ALTER TABLE ratings ADD CONSTRAINT fk_ratings_episode
  FOREIGN KEY (episode) REFERENCES episodes (episode_id);

-- 5 FK constraint(s) declared (off unless EFFORTLESS_ENFORCE_FKS=true).
