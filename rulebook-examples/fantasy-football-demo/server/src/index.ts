import express, { Request, Response } from 'express'
import { Pool } from 'pg'

const app = express()
const port = 3045

const pool = new Pool({
  user: 'postgres',
  host: 'localhost',
  database: 'erb_fantasy_football_demo',
  port: 5432,
})

// Extend Express Request to include user context
declare global {
  namespace Express {
    interface Request {
      me?: { email: string; role: string; name: string }
    }
  }
}

// Auth middleware: read X-User-Email header
app.use((req, res, next) => {
  const email = req.headers['x-user-email'] as string
  if (email) {
    req.me = { email, role: 'commissioner', name: email }
  }
  next()
})

app.use(express.json())

// Health check
app.get('/healthz', (req: Request, res: Response) => {
  res.json({ ok: true })
})

// Dev users (for login picker)
app.get('/api/dev-users', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT email, role FROM vw_dev_users ORDER BY role')
    res.json(
      result.rows.map((row) => ({
        email: row.email,
        role: row.role,
        name: row.email.split('@')[0],
      }))
    )
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch dev users' })
  }
})

// Get current user
app.get('/api/me', (req: Request, res: Response) => {
  if (!req.me) {
    return res.status(401).json({ error: 'Not authenticated' })
  }
  res.json(req.me)
})

// Players CRUD
app.get('/api/players', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_players ORDER BY name')
    res.json(result.rows)
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch players' })
  }
})

app.get('/api/players/:id', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_players WHERE player_id = $1', [req.params.id])
    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Player not found' })
    }
    res.json(result.rows[0])
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch player' })
  }
})

app.patch('/api/players/:id', async (req: Request, res: Response) => {
  if (!req.me) return res.status(401).json({ error: 'Not authenticated' })
  try {
    const { passing_yards, passing_touchdowns, interceptions, rushing_yards, rushing_touchdowns, receptions, received_yards, received_touchdowns } = req.body
    const updates: string[] = []
    const values: unknown[] = []
    let paramCount = 1

    if (passing_yards !== undefined) {
      updates.push(`passing_yards = $${paramCount++}`)
      values.push(passing_yards)
    }
    if (passing_touchdowns !== undefined) {
      updates.push(`passing_touchdowns = $${paramCount++}`)
      values.push(passing_touchdowns)
    }
    if (interceptions !== undefined) {
      updates.push(`interceptions = $${paramCount++}`)
      values.push(interceptions)
    }
    if (rushing_yards !== undefined) {
      updates.push(`rushing_yards = $${paramCount++}`)
      values.push(rushing_yards)
    }
    if (rushing_touchdowns !== undefined) {
      updates.push(`rushing_touchdowns = $${paramCount++}`)
      values.push(rushing_touchdowns)
    }
    if (receptions !== undefined) {
      updates.push(`receptions = $${paramCount++}`)
      values.push(receptions)
    }
    if (received_yards !== undefined) {
      updates.push(`received_yards = $${paramCount++}`)
      values.push(received_yards)
    }
    if (received_touchdowns !== undefined) {
      updates.push(`received_touchdowns = $${paramCount++}`)
      values.push(received_touchdowns)
    }

    if (updates.length === 0) {
      return res.status(400).json({ error: 'No fields to update' })
    }

    values.push(req.params.id)
    const query = `UPDATE players SET ${updates.join(', ')} WHERE player_id = $${paramCount} RETURNING *`
    const result = await pool.query(query, values)

    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Player not found' })
    }

    res.json(result.rows[0])
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to update player' })
  }
})

// Rosters
app.get('/api/rosters', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_rosters ORDER BY name')
    res.json(result.rows)
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch rosters' })
  }
})

app.get('/api/rosters/:id', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_rosters WHERE roster_id = $1', [req.params.id])
    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Roster not found' })
    }
    res.json(result.rows[0])
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch roster' })
  }
})

// Matchups
app.get('/api/matchups', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_matchups ORDER BY week, matchup_id')
    res.json(result.rows)
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch matchups' })
  }
})

// Standings
app.get('/api/standings', async (req: Request, res: Response) => {
  try {
    const result = await pool.query('SELECT * FROM vw_standings ORDER BY seed_rank')
    res.json(result.rows)
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch standings' })
  }
})

// Rulebook (for explainer DAG)
app.get('/api/rulebook', async (req: Request, res: Response) => {
  try {
    const rulebook = await import('../../../effortless-rulebook/effortless-rulebook.json', {
      assert: { type: 'json' },
    })
    res.json(rulebook.default)
  } catch (err) {
    console.error(err)
    res.status(500).json({ error: 'Failed to fetch rulebook' })
  }
})

app.listen(port, () => {
  console.log(`Server listening on http://localhost:${port}`)
})
