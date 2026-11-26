// Star Trek explorer API. Every read is `SELECT ... FROM vw_<entity>`: the views
// carry every value the rulebook derives (roll-ups, lookups, averages), so this
// file computes nothing. A failing query is a 500 with the database's message.
const express = require('express');
const { Pool } = require('pg');
const path = require('path');

const PORT = process.env.APP_PORT || 3140;
const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/erb_star_trek';
const EXPLAINER_DIR = path.join(__dirname, '..', 'explainer', 'rulebook-explainer-dag');

const pool = new Pool({ connectionString: DATABASE_URL });
const app = express();

const rows = async (sql, params = []) => (await pool.query(sql, params)).rows;

// One handler shape: run, 404 when a single row is missing, 500 with the cause.
function route(handler) {
  return async (req, res) => {
    try {
      const body = await handler(req);
      if (body === undefined) return res.status(404).json({ error: `nothing at ${req.path}` });
      res.json(body);
    } catch (err) {
      console.error(`${req.path}:`, err);
      res.status(500).json({ error: err.message });
    }
  };
}

app.get('/api/health', route(async () => {
  const [db] = await rows('SELECT current_database() AS name, count(*)::int AS series FROM vw_series');
  return { status: 'ok', database: db.name, series: db.series };
}));

app.get('/api/series', route(() => rows('SELECT * FROM vw_series ORDER BY series_number')));

app.get('/api/shows/:code', route(async (req) => {
  const [show] = await rows('SELECT * FROM vw_series WHERE lower(show_code) = lower($1)', [req.params.code]);
  if (!show) return undefined;
  show.ratings = await rows('SELECT * FROM vw_ratings WHERE series = $1 ORDER BY created_at, rating_id', [show.serie_id]);
  return show;
}));

app.get('/api/series/:serieId/seasons', route((req) =>
  rows('SELECT * FROM vw_seasons WHERE series = $1 ORDER BY season_number', [req.params.serieId])));

app.get('/api/seasons/:seasonId/episodes', route((req) =>
  rows('SELECT * FROM vw_episodes WHERE season = $1 ORDER BY episode_number', [req.params.seasonId])));

app.get('/api/episodes/:episodeId', route(async (req) => {
  const [episode] = await rows('SELECT * FROM vw_episodes WHERE episode_id = $1', [req.params.episodeId]);
  if (!episode) return undefined;
  episode.ratings = await rows('SELECT * FROM vw_ratings WHERE episode = $1 ORDER BY created_at, rating_id', [episode.episode_id]);
  return episode;
}));

app.get('/api/movies', route(() => rows('SELECT * FROM vw_movies ORDER BY movie_number')));

app.get('/api/movies/:movieId', route(async (req) => {
  const [movie] = await rows('SELECT * FROM vw_movies WHERE movie_id = $1', [req.params.movieId]);
  if (!movie) return undefined;
  movie.crew = await rows('SELECT * FROM vw_crew_assignments WHERE movie = $1 ORDER BY crew_type_name, person_name', [movie.movie_id]);
  return movie;
}));

app.get('/api/people', route(() => rows('SELECT * FROM vw_people ORDER BY name')));

app.use('/api', (req, res) => res.status(404).json({ error: `no API route ${req.path}` }));

// The explainer this project's build writes (rulebook-to-explainer-dag): hover a
// derived value for its rule and the values it came from.
app.use('/rulebook-explainer-dag', express.static(EXPLAINER_DIR, { maxAge: 0 }));
for (const file of ['app.js', 'routes.js']) {
  app.get(`/${file}`, (req, res) => res.sendFile(path.join(__dirname, file)));
}
app.get('*', (req, res) => res.sendFile(path.join(__dirname, 'index.html')));

app.listen(PORT, () => {
  console.log(`Star Trek Explorer running on http://localhost:${PORT} (database ${DATABASE_URL})`);
});
