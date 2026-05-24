# Star Trek Explorer — Quick Start

## Run the App

From the `app/` directory:

```bash
npm start
```

Or use the automated start script from the project root:

```bash
./app/start.sh
```

The app will be available at **http://localhost:3000**

## What You Get

A dark-themed browser UI that lets you:
- Browse all Star Trek TV series (TOS, TNG, DS9, VOY, ENT, DIS, PIC, etc.)
- Click a series to view details (network, premiere date, episode/season counts)
- Click a season to expand and see all episodes with air dates
- See aggregated statistics calculated from the rulebook (total seasons, episodes)

## Under the Hood

- **API**: Express.js server on port 3000
- **Database**: PostgreSQL (`erb_star_trek` database)
- **Data Source**: Star Trek rulebook (`../effortless-rulebook/star-trek-rulebook.json`)
- **Frontend**: Vanilla HTML/CSS/JavaScript (no build step needed)

## API Endpoints

If you want to query the API directly:

```bash
# Get all series
curl http://localhost:3000/api/series

# Get seasons for a series
curl http://localhost:3000/api/series/ser001/seasons

# Get episodes for a season
curl http://localhost:3000/api/seasons/sea001/episodes

# Health check
curl http://localhost:3000/api/health
```

## Database Setup

If the database hasn't been initialized yet, the start script will run it automatically. To manually initialize:

```bash
cd ../postgres-bootstrap
./init-db.sh
```

## Troubleshooting

**Port 3000 already in use?**
```bash
APP_PORT=3001 npm start
```

**Database connection error?**
Ensure PostgreSQL is running and the `erb_star_trek` database exists:
```bash
psql -U postgres -l | grep erb_star_trek
```
