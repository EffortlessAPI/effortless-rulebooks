import express from 'express';
import cors from 'cors';
import { Pool } from 'pg';

const app = express();
const port = process.env.SERVER_PORT || 3045;

const pool = new Pool({
  connectionString: process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/erb_community_event_planner'
});

app.use(cors());
app.use(express.json());

// Auth middleware — stub auth with X-User-Email header
app.use((req, res, next) => {
  const email = req.headers['x-user-email'] as string || 'admin@events.local';
  let role = 'Attendee';
  if (email === 'admin@events.local') role = 'Admin';
  else if (email === 'organizer@events.local') role = 'Event Organizer';
  else if (email === 'speaker@events.local') role = 'Speaker';
  (req as any).me = { email, role };
  next();
});

// Dev users endpoint
app.get('/api/dev-users', (req, res) => {
  res.json([
    { email: 'admin@events.local', role: 'Admin', description: 'Full access' },
    { email: 'organizer@events.local', role: 'Event Organizer', description: 'Create and manage events' },
    { email: 'speaker@events.local', role: 'Speaker', description: 'View assigned talks' },
    { email: 'attendee@events.local', role: 'Attendee', description: 'Browse and RSVP' }
  ]);
});

// Current user endpoint
app.get('/api/me', (req, res) => {
  const me = (req as any).me;
  res.json(me);
});

// Venues endpoints
app.get('/api/venues', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_venues ORDER BY name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

app.get('/api/venues/:id', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_venues WHERE venue_id = $1', [req.params.id]);
    res.json(result.rows[0]);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

// Events endpoints
app.get('/api/events', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_events ORDER BY event_date');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

app.get('/api/events/:id', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_events WHERE event_id = $1', [req.params.id]);
    res.json(result.rows[0]);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

app.patch('/api/events/:id', async (req, res) => {
  const me = (req as any).me;
  if (me.role !== 'Admin') {
    return res.status(403).json({ error: 'Only admins can edit events' });
  }
  const { duration_minutes, registration_close_days_before_event, category } = req.body;

  // Validate registration_close_days_before_event if provided
  const allowedValues = [
    ...Array.from({ length: 30 }, (_, i) => i + 1), // 1-30
    60, 90, 120, 150, 180, 365
  ];

  if (registration_close_days_before_event !== undefined && !allowedValues.includes(registration_close_days_before_event)) {
    return res.status(400).json({ error: 'Invalid registration_close_days_before_event value' });
  }

  try {
    const updates: string[] = ['duration_minutes = $1'];
    const values: any[] = [duration_minutes];

    if (registration_close_days_before_event !== undefined) {
      updates.push(`registration_close_days_before_event = $${updates.length + 1}`);
      values.push(registration_close_days_before_event);
    }

    if (category !== undefined) {
      updates.push(`category = $${updates.length + 1}`);
      values.push(category);
    }

    values.push(req.params.id);

    const result = await pool.query(
      `UPDATE events SET ${updates.join(', ')} WHERE event_id = $${values.length} RETURNING *`,
      values
    );
    res.json(result.rows[0]);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

// Speakers endpoints
app.get('/api/speakers', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_speakers ORDER BY name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

// Assignments endpoints
app.get('/api/assignments', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_assignments ORDER BY name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

app.get('/api/assignments/:eventId', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_assignments WHERE event_ref = $1', [req.params.eventId]);
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

// Attendees endpoints
app.get('/api/attendees', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_attendees ORDER BY name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

// RSVPs endpoints
app.get('/api/rsvps', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_rsvps ORDER BY name');
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

app.get('/api/rsvps/:eventId', async (req, res) => {
  try {
    const result = await pool.query('SELECT * FROM vw_rsvps WHERE event_ref = $1', [req.params.eventId]);
    res.json(result.rows);
  } catch (error) {
    res.status(500).json({ error: String(error) });
  }
});

app.listen(port, () => {
  console.log(`✓ Server running on http://localhost:${port}`);
});
