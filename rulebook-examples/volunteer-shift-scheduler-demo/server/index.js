import express from 'express';
import cors from 'cors';
import { pool, q, q1 } from './db.js';
import { computeShifts, computeEvents, computeVolunteerLoads } from './compute.js';
import { schemaForApi, tables as schemaTables, constants as schemaConstants } from '../schema/scheduler.schema.mjs';
import { buildWorkbook } from './excel.js';

const app = express();
app.use(cors());
app.use(express.json());

const wrap = (fn) => (req, res, next) =>
  Promise.resolve(fn(req, res, next)).catch(next);

// -----------------------------------------------------------------
// Context: load the full lookup pool once per request. The compute
// layer only needs in-memory arrays — it never re-queries.
// -----------------------------------------------------------------
async function loadCtx() {
  const [events, shifts, assignments, volunteersRaw, volunteer_skills, skills] = await Promise.all([
    q('SELECT * FROM events ORDER BY event_date, id'),
    q('SELECT * FROM shifts ORDER BY starts_at, id'),
    q('SELECT * FROM assignments ORDER BY created_at'),
    q('SELECT * FROM volunteers ORDER BY name'),
    q('SELECT * FROM volunteer_skills'),
    q('SELECT * FROM skills ORDER BY name'),
  ]);
  const skillByName = Object.fromEntries(skills.map((s) => [s.name, s]));
  const skillById = Object.fromEntries(skills.map((s) => [s.id, s]));
  // attach skills array per volunteer for downstream UIs
  const volunteers = volunteersRaw.map((v) => ({
    ...v,
    skills: volunteer_skills
      .filter((vs) => vs.volunteer_id === v.id)
      .map((vs) => ({ id: vs.skill_id, name: skillById[vs.skill_id]?.name || '?' }))
      .sort((a, b) => a.name.localeCompare(b.name)),
  }));
  return { events, shifts, assignments, volunteers, volunteer_skills, skills, skillByName, skillById };
}

async function loadFullyComputed() {
  const ctx = await loadCtx();
  const computedShifts = computeShifts(ctx.shifts, ctx);
  ctx.computedShifts = computedShifts;
  const computedEvents = computeEvents(ctx.events, ctx);
  return { ctx, computedShifts, computedEvents };
}

// Stable label helper so explain refs in the UI look like `Ava Chen` instead of `7`.
function attachReadable(shift, ctx) {
  const skillRow = shift.required_skill_id ? ctx.skillById[shift.required_skill_id] : null;
  return { ...shift, required_skill_name: skillRow?.name || null };
}

// -----------------------------------------------------------------
// Identity (dev login)
// -----------------------------------------------------------------
app.get(
  '/api/identities',
  wrap(async (_req, res) => {
    const volunteers = await q('SELECT id, name, email FROM volunteers ORDER BY name');
    res.json({
      coordinator: { kind: 'coordinator', label: 'Coordinator (full edit)' },
      viewer: { kind: 'viewer', label: 'Viewer (read-only)' },
      volunteers: volunteers.map((v) => ({
        kind: 'volunteer',
        id: v.id,
        label: `${v.name} (volunteer)`,
        email: v.email,
      })),
    });
  })
);

// -----------------------------------------------------------------
// Declarative schema → /api/schema (powers DAG page + rules UI)
// -----------------------------------------------------------------
app.get(
  '/api/schema',
  wrap(async (_req, res) => {
    res.json(schemaForApi);
  })
);

// -----------------------------------------------------------------
// Events
// -----------------------------------------------------------------
app.get(
  '/api/events',
  wrap(async (_req, res) => {
    const { computedEvents, computedShifts } = await loadFullyComputed();
    res.json(
      computedEvents.map((ev) => ({
        id: ev.id,
        name: ev.name,
        location: ev.location,
        event_date: ev.event_date,
        target_hours_per_volunteer: ev.target_hours_per_volunteer,
        shift_count: computedShifts.filter((s) => s.event_id === ev.id).length,
        volunteer_count: ev.computed.volunteer_count,
        computed: ev.computed,
      })),
    );
  })
);

app.get(
  '/api/events/:id',
  wrap(async (req, res) => {
    const id = Number(req.params.id);
    const { ctx, computedShifts, computedEvents } = await loadFullyComputed();
    const ev = computedEvents.find((e) => e.id === id);
    if (!ev) return res.status(404).json({ error: 'not found' });

    const shifts = computedShifts
      .filter((s) => s.event_id === id)
      .map((s) => {
        const decorated = attachReadable(s, ctx);
        const assignments = ctx.assignments
          .filter((a) => a.shift_id === s.id)
          .map((a) => {
            const v = ctx.volunteers.find((vv) => vv.id === a.volunteer_id);
            return { ...a, assignment_id: a.id, volunteer: v };
          });
        return { ...decorated, assignments };
      });

    // distinct volunteers in this event, with their loads
    const shiftIds = new Set(shifts.map((s) => s.id));
    const distinctVolIds = [
      ...new Set(ctx.assignments.filter((a) => shiftIds.has(a.shift_id)).map((a) => a.volunteer_id)),
    ];
    const loads = computeVolunteerLoads(
      distinctVolIds.map((vid) => ({ volunteer_id: vid, event_id: id })),
      ctx,
    );
    const volunteer_loads = loads.map((load) => {
      const v = ctx.volunteers.find((vv) => vv.id === load.volunteer_id);
      const mine = shifts.filter((s) => s.assignments.some((a) => a.volunteer_id === v?.id));
      return {
        volunteer: v,
        ...load,
        shifts: mine.map((s) => ({ id: s.id, name: s.name })),
      };
    });

    res.json({ ...ev, shifts, volunteer_loads });
  })
);

app.post(
  '/api/events',
  wrap(async (req, res) => {
    const { name, location = '', event_date, target_hours_per_volunteer = 4 } = req.body;
    if (!name || !event_date) return res.status(400).json({ error: 'name and event_date required' });
    const row = await q1(
      `INSERT INTO events (name, location, event_date, target_hours_per_volunteer)
       VALUES ($1,$2,$3,$4) RETURNING *`,
      [name, location, event_date, target_hours_per_volunteer]
    );
    res.json(row);
  })
);

app.patch(
  '/api/events/:id',
  wrap(async (req, res) => {
    const fields = ['name', 'location', 'event_date', 'target_hours_per_volunteer'];
    const sets = [];
    const params = [];
    for (const f of fields) {
      if (req.body[f] !== undefined) {
        params.push(req.body[f]);
        sets.push(`${f} = $${params.length}`);
      }
    }
    if (!sets.length) return res.status(400).json({ error: 'nothing to update' });
    params.push(req.params.id);
    const row = await q1(
      `UPDATE events SET ${sets.join(', ')} WHERE id = $${params.length} RETURNING *`,
      params
    );
    res.json(row);
  })
);

app.delete(
  '/api/events/:id',
  wrap(async (req, res) => {
    await q('DELETE FROM events WHERE id = $1', [req.params.id]);
    res.json({ ok: true });
  })
);

// -----------------------------------------------------------------
// Shifts
// -----------------------------------------------------------------
app.get(
  '/api/shifts/:id',
  wrap(async (req, res) => {
    const id = Number(req.params.id);
    const { ctx, computedShifts } = await loadFullyComputed();
    const s = computedShifts.find((x) => x.id === id);
    if (!s) return res.status(404).json({ error: 'not found' });
    const decorated = attachReadable(s, ctx);
    const assignments = ctx.assignments
      .filter((a) => a.shift_id === id)
      .map((a) => ({ ...a, assignment_id: a.id, volunteer: ctx.volunteers.find((v) => v.id === a.volunteer_id) }));
    res.json({ ...decorated, assignments });
  })
);

app.post(
  '/api/events/:eventId/shifts',
  wrap(async (req, res) => {
    const { name, starts_at, ends_at, required_count = 1, required_skill_id = null } = req.body;
    if (!name || !starts_at || !ends_at)
      return res.status(400).json({ error: 'name, starts_at, ends_at required' });
    const row = await q1(
      `INSERT INTO shifts (event_id, name, starts_at, ends_at, required_count, required_skill_id)
       VALUES ($1,$2,$3,$4,$5,$6) RETURNING *`,
      [req.params.eventId, name, starts_at, ends_at, required_count, required_skill_id]
    );
    res.json(row);
  })
);

app.patch(
  '/api/shifts/:id',
  wrap(async (req, res) => {
    const fields = ['name', 'starts_at', 'ends_at', 'required_count', 'required_skill_id'];
    const sets = [];
    const params = [];
    for (const f of fields) {
      if (req.body[f] !== undefined) {
        params.push(req.body[f]);
        sets.push(`${f} = $${params.length}`);
      }
    }
    if (!sets.length) return res.status(400).json({ error: 'nothing to update' });
    params.push(req.params.id);
    const row = await q1(
      `UPDATE shifts SET ${sets.join(', ')} WHERE id = $${params.length} RETURNING *`,
      params
    );
    res.json(row);
  })
);

app.delete(
  '/api/shifts/:id',
  wrap(async (req, res) => {
    await q('DELETE FROM shifts WHERE id = $1', [req.params.id]);
    res.json({ ok: true });
  })
);

// -----------------------------------------------------------------
// Assignments
// -----------------------------------------------------------------
app.post(
  '/api/shifts/:shiftId/assignments',
  wrap(async (req, res) => {
    const { volunteer_id } = req.body;
    if (!volunteer_id) return res.status(400).json({ error: 'volunteer_id required' });
    try {
      const row = await q1(
        `INSERT INTO assignments (shift_id, volunteer_id) VALUES ($1,$2) RETURNING *`,
        [req.params.shiftId, volunteer_id]
      );
      res.json(row);
    } catch (e) {
      if (e.code === '23505')
        return res.status(409).json({ error: 'already assigned' });
      throw e;
    }
  })
);

app.delete(
  '/api/assignments/:id',
  wrap(async (req, res) => {
    await q('DELETE FROM assignments WHERE id = $1', [req.params.id]);
    res.json({ ok: true });
  })
);

// -----------------------------------------------------------------
// Volunteers & skills
// -----------------------------------------------------------------
app.get(
  '/api/volunteers',
  wrap(async (_req, res) => {
    const ctx = await loadCtx();
    res.json(ctx.volunteers);
  })
);

app.get(
  '/api/skills',
  wrap(async (_req, res) => {
    const rows = await q('SELECT * FROM skills ORDER BY name');
    res.json(rows);
  })
);

app.post(
  '/api/volunteers',
  wrap(async (req, res) => {
    const { name, email, reliability_score = 0.85, max_hours = 8, skill_ids = [] } = req.body;
    if (!name || !email) return res.status(400).json({ error: 'name and email required' });
    const v = await q1(
      `INSERT INTO volunteers (name, email, reliability_score, max_hours)
       VALUES ($1,$2,$3,$4) RETURNING *`,
      [name, email, reliability_score, max_hours]
    );
    for (const sid of skill_ids) {
      await q(
        'INSERT INTO volunteer_skills (volunteer_id, skill_id) VALUES ($1,$2) ON CONFLICT DO NOTHING',
        [v.id, sid]
      );
    }
    res.json(v);
  })
);

app.patch(
  '/api/volunteers/:id',
  wrap(async (req, res) => {
    const fields = ['name', 'email', 'reliability_score', 'max_hours'];
    const sets = [];
    const params = [];
    for (const f of fields) {
      if (req.body[f] !== undefined) {
        params.push(req.body[f]);
        sets.push(`${f} = $${params.length}`);
      }
    }
    if (sets.length) {
      params.push(req.params.id);
      await q(
        `UPDATE volunteers SET ${sets.join(', ')} WHERE id = $${params.length}`,
        params
      );
    }
    if (Array.isArray(req.body.skill_ids)) {
      await q('DELETE FROM volunteer_skills WHERE volunteer_id = $1', [req.params.id]);
      for (const sid of req.body.skill_ids) {
        await q(
          'INSERT INTO volunteer_skills (volunteer_id, skill_id) VALUES ($1,$2) ON CONFLICT DO NOTHING',
          [req.params.id, sid]
        );
      }
    }
    const ctx = await loadCtx();
    res.json(ctx.volunteers.find((v) => v.id === Number(req.params.id)));
  })
);

app.delete(
  '/api/volunteers/:id',
  wrap(async (req, res) => {
    await q('DELETE FROM volunteers WHERE id = $1', [req.params.id]);
    res.json({ ok: true });
  })
);

// -----------------------------------------------------------------
// Volunteer self-view
// -----------------------------------------------------------------
app.get(
  '/api/volunteers/:id/shifts',
  wrap(async (req, res) => {
    const rows = await q(
      `SELECT sh.*, ev.name AS event_name, ev.event_date, ev.location AS event_location,
              s.name AS required_skill_name
         FROM assignments a
         JOIN shifts sh ON sh.id = a.shift_id
         JOIN events ev ON ev.id = sh.event_id
         LEFT JOIN skills s ON s.id = sh.required_skill_id
        WHERE a.volunteer_id = $1
        ORDER BY sh.starts_at`,
      [req.params.id]
    );
    res.json(rows);
  })
);

// -----------------------------------------------------------------
// Schema → Excel (with per-field rules + working formulas)
// -----------------------------------------------------------------
app.get(
  '/api/schema.xlsx',
  wrap(async (_req, res) => {
    const ctx = await loadCtx();
    const wb = await buildWorkbook(ctx, schemaTables, schemaForApi.computed, schemaConstants);
    res.setHeader(
      'Content-Type',
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    );
    res.setHeader(
      'Content-Disposition',
      'attachment; filename="volunteer-shift-scheduler-schema.xlsx"'
    );
    await wb.xlsx.write(res);
    res.end();
  })
);

// -----------------------------------------------------------------
// Error handling
// -----------------------------------------------------------------
app.use((err, _req, res, _next) => {
  console.error(err);
  res.status(500).json({ error: err.message });
});

const PORT = Number(process.env.PORT) || 4017;
app.listen(PORT, () => {
  console.log(`API listening on http://localhost:${PORT}`);
});

const shutdown = () => {
  pool.end().finally(() => process.exit(0));
};
process.on('SIGINT', shutdown);
process.on('SIGTERM', shutdown);
