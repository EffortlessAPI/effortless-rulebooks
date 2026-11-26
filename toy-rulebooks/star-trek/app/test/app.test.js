// The Star Trek explorer, tested against the live erb_star_trek database.
//
//   cd toy-rulebooks/star-trek && effortless build     (resets erb_star_trek)
//   cd app && npm test
//
// Found broken on 2026-11-26: the server read the base tables, so every value the
// rulebook derives (a season's episode count, a series' average rating, IsGood,
// a movie's crew) was missing from the screens; and the router matched every
// /show/... address as the show page, so no season or episode page could open.

const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const { spawn } = require('node:child_process');
const fs = require('node:fs');
const net = require('node:net');
const path = require('node:path');

const APP_DIR = path.resolve(__dirname, '..');
const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/erb_star_trek';

// ---------------------------------------------------------------- the router
test('routes: every page of the app has an address that opens it', () => {
  const { routeFor } = require('../routes.js');
  assert.deepEqual(routeFor('/'), { page: 'home' });
  assert.deepEqual(routeFor('/show/tos'), { page: 'show', show: 'tos' });
  assert.deepEqual(routeFor('/show/tos/season-1'), { page: 'season', show: 'tos', season: 1 });
  assert.deepEqual(routeFor('/show/tos/season-1/episode-7'), { page: 'episode', show: 'tos', season: 1, episode: 7 });
  assert.deepEqual(routeFor('/movie/movie-1-generations'), { page: 'movie', movie: 'movie-1-generations' });
  assert.deepEqual(routeFor('/nowhere'), { page: 'not-found' });
});

// ------------------------------------------------- the view is the contract
test('server: every query reads a vw_* view, never a base table', () => {
  const source = fs.readFileSync(path.join(APP_DIR, 'server.js'), 'utf8');
  const tables = [...source.matchAll(/\bFROM\s+([a-z_]+)/gi)].map(m => m[1]);
  assert.ok(tables.length > 0, 'found no queries in server.js');
  assert.deepEqual(tables.filter(t => !t.startsWith('vw_')), [], 'queries that bypass the views');
});

// ------------------------------------------------------ the API, end to end
let server;
let base;

function freePort() {
  return new Promise((resolve, reject) => {
    const s = net.createServer();
    s.once('error', reject);
    s.listen(0, '127.0.0.1', () => { const { port } = s.address(); s.close(() => resolve(port)); });
  });
}

before(async () => {
  const port = await freePort();
  base = `http://127.0.0.1:${port}`;
  server = spawn(process.execPath, ['server.js'], {
    cwd: APP_DIR, env: { ...process.env, APP_PORT: String(port), DATABASE_URL }, stdio: ['ignore', 'pipe', 'pipe'],
  });
  let output = '';
  server.stdout.on('data', d => { output += d; });
  server.stderr.on('data', d => { output += d; });
  for (let i = 0; i < 80; i++) {
    try { if ((await fetch(`${base}/api/health`)).ok) return; } catch { /* not up yet */ }
    await new Promise(r => setTimeout(r, 100));
  }
  throw new Error(`server.js did not come up on ${base}:\n${output}`);
});

after(() => { if (server) server.kill(); });

async function get(route) {
  const res = await fetch(base + route);
  const body = await res.json();
  assert.equal(res.status, 200, `${route} -> ${res.status} ${JSON.stringify(body)}`);
  return body;
}

test('api: health names the database it reads', async () => {
  const health = await get('/api/health');
  assert.equal(health.status, 'ok');
  assert.equal(health.database, 'erb_star_trek');
});

test('api: a series carries its roll-ups (average rating, IsGood, totals)', async () => {
  const series = await get('/api/series');
  assert.equal(series.length, 10);
  const tos = series.find(s => s.show_code === 'TOS');
  assert.equal(Number(tos.rating_count), 3);
  assert.ok(Math.abs(Number(tos.rating) - 13 / 3) < 1e-9, `TOS rating ${tos.rating}`);
  assert.equal(tos.is_good, false);
  assert.equal(Number(tos.total_episodes), 76);
  assert.equal(tos.is_long_running, true);
  const tng = series.find(s => s.show_code === 'TNG');
  assert.equal(Number(tng.rating), 5);
  assert.equal(tng.is_good, true);
  assert.deepEqual(series.map(s => s.series_number), [...series.map(s => s.series_number)].sort((a, b) => a - b));
});

test('api: one show by its code, with its fan ratings', async () => {
  const show = await get('/api/shows/tos');
  assert.equal(show.serie_id, '1-tos');
  assert.equal(show.ratings.length, 3);
  assert.ok(show.ratings.every(r => r.series === '1-tos'));
});

test('api: a season carries its episode count', async () => {
  const seasons = await get('/api/series/1-tos/seasons');
  assert.deepEqual(seasons.map(s => s.season_number), [1, 2, 3]);
  assert.equal(seasons.reduce((n, s) => n + Number(s.episode_count), 0), 76);
  assert.equal(seasons[0].show_title, 'Star Trek: The Original Series');
});

test('api: an episode carries its season lookups and its ratings', async () => {
  const episodes = await get('/api/seasons/1-tos-season-1/episodes');
  assert.ok(episodes.length > 0);
  assert.deepEqual(episodes.map(e => e.episode_number), [...episodes.map(e => e.episode_number)].sort((a, b) => a - b));
  const one = await get('/api/episodes/1-tos-season-1-episode-07');
  assert.equal(one.season_number, 1);
  assert.equal(one.ratings.length, 1);
  assert.equal(one.ratings[0].users_name, 'Taylor Brooks');
});

test('api: a movie carries its crew', async () => {
  const movies = await get('/api/movies');
  const generations = movies.find(m => m.movie_id === 'movie-1-generations');
  assert.equal(Number(generations.crew_count), 3);
  const movie = await get('/api/movies/movie-1-generations');
  assert.deepEqual(movie.crew.map(c => c.person_name).sort(), ['Gene Roddenberry', 'Patrick Stewart', 'William Shatner']);
});

test('api: an unknown show is a 404, not an empty page', async () => {
  const res = await fetch(`${base}/api/shows/nope`);
  assert.equal(res.status, 404);
});

test('explainer: the generated explainer is served beside the app', async () => {
  const res = await fetch(`${base}/rulebook-explainer-dag/explainer-dag.js`);
  assert.equal(res.status, 200);
  assert.match(res.headers.get('content-type'), /javascript/);
  assert.match(await res.text(), /EffortlessExplainer/);
});
