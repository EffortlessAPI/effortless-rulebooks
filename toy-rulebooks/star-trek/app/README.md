# Star Trek Explorer App

Browse the ten Star Trek series, their seasons and episodes, the four movies and their crew. Every count, average and
verdict on screen (a season's episode count, a series' average fan rating, `IsGood`, `IsLongRunning`, a movie's crew
count) is a rule in `../effortless-rulebook/star-trek-rulebook.json`, read from the generated `vw_*` views. The app
computes none of them. Hover a value marked ƒ on a detail page and the project's explainer shows the rule.

## Run it

```bash
cd ..                 # toy-rulebooks/star-trek
effortless build      # regenerates the SQL and resets erb_star_trek from the rulebook
./start.sh            # http://localhost:3140/
```

`DATABASE_URL` overrides the database (default `postgresql://postgres@localhost:5432/erb_star_trek`).

## Pages

| Address | Shows |
|---|---|
| `/` | series (with average rating, long-running and good badges) and movies |
| `/show/<code>` | a series: its roll-ups, fan ratings and seasons |
| `/show/<code>/season-<n>` | a season and its episodes |
| `/show/<code>/season-<n>/episode-<n>` | an episode and its fan ratings |
| `/movie/<movie-id>` | a movie and its cast and crew |

## API

Each one is a `SELECT` from a view.

- `GET /api/health`: the database it reads
- `GET /api/series`: `vw_series`
- `GET /api/shows/:code`: one series by show code, with its `vw_ratings`
- `GET /api/series/:serieId/seasons`: `vw_seasons`
- `GET /api/seasons/:seasonId/episodes`: `vw_episodes`
- `GET /api/episodes/:episodeId`: one episode, with its `vw_ratings`
- `GET /api/movies`, `GET /api/movies/:movieId`: `vw_movies`, the second with its `vw_crew_assignments`
- `GET /api/people`: `vw_people`

`/rulebook-explainer-dag/*` serves `../explainer/rulebook-explainer-dag/`, which the build writes.

## Tests

```bash
npm test   # against the live erb_star_trek: routes, view-only queries, every endpoint
```
