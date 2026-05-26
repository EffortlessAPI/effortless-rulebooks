const express = require('express');
const { Pool } = require('pg');
const cors = require('cors');
const path = require('path');
require('dotenv').config({ path: path.join(__dirname, '..', 'postgres-bootstrap', '.env') });

const app = express();
const PORT = process.env.APP_PORT || 3000;

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/erb_star_trek',
});

app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname)));

app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});

app.get('/api/series', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM series ORDER BY serie_id');
    res.json(result.rows);
  } catch (err) {
    console.error('Error fetching series:', err);
    res.status(500).json({ error: 'Failed to fetch series' });
  }
});

app.get('/api/series/:serieId', async (req, res) => {
  try {
    const { serieId } = req.params;
    const result = await pool.query('SELECT * FROM series WHERE serie_id = $1', [serieId]);
    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Series not found' });
    }
    res.json(result.rows[0]);
  } catch (err) {
    console.error('Error fetching series:', err);
    res.status(500).json({ error: 'Failed to fetch series' });
  }
});

app.get('/api/series/:serieId/seasons', async (req, res) => {
  try {
    const { serieId } = req.params;
    const result = await pool.query(
      'SELECT * FROM seasons WHERE series = $1 ORDER BY season_number',
      [serieId]
    );
    res.json(result.rows);
  } catch (err) {
    console.error('Error fetching seasons:', err);
    res.status(500).json({ error: 'Failed to fetch seasons' });
  }
});

app.get('/api/seasons/:seasonId/episodes', async (req, res) => {
  try {
    const { seasonId } = req.params;
    const result = await pool.query(
      'SELECT * FROM episodes WHERE season = $1 ORDER BY episode_number',
      [seasonId]
    );
    res.json(result.rows);
  } catch (err) {
    console.error('Error fetching episodes:', err);
    res.status(500).json({ error: 'Failed to fetch episodes' });
  }
});

app.get('/api/episodes/:episodeId', async (req, res) => {
  try {
    const { episodeId } = req.params;
    const result = await pool.query(
      'SELECT * FROM episodes WHERE episode_id = $1',
      [episodeId]
    );
    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Episode not found' });
    }
    res.json(result.rows[0]);
  } catch (err) {
    console.error('Error fetching episode:', err);
    res.status(500).json({ error: 'Failed to fetch episode' });
  }
});

app.get('/api/movies', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM movies ORDER BY movie_number');
    res.json(result.rows);
  } catch (err) {
    console.error('Error fetching movies:', err);
    res.status(500).json({ error: 'Failed to fetch movies' });
  }
});

app.get('/api/movies/:movieId', async (req, res) => {
  try {
    const { movieId } = req.params;
    const result = await pool.query('SELECT * FROM movies WHERE movie_id = $1', [movieId]);
    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Movie not found' });
    }
    res.json(result.rows[0]);
  } catch (err) {
    console.error('Error fetching movie:', err);
    res.status(500).json({ error: 'Failed to fetch movie' });
  }
});

app.get('/api/people', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM people ORDER BY name LIMIT 50');
    res.json(result.rows);
  } catch (err) {
    console.error('Error fetching people:', err);
    res.status(500).json({ error: 'Failed to fetch people' });
  }
});

app.get('/api/health', (req, res) => {
  res.json({ status: 'ok' });
});

app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});

app.listen(PORT, () => {
  console.log(`Star Trek Explorer API running on http://localhost:${PORT}`);
});
