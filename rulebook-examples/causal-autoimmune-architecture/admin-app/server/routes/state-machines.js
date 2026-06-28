// routes/state-machines.js — the diagnosis-lifecycle state machine surface.
//
// Reads vw_* views; writes machine_states / state_transition_rules base tables.
// Detail returns {machine, states[], rules[], transitions[]} (the portal shape).
import express from 'express';
import { query } from '../db.js';

export const router = express.Router();

const M_SELECT = `SELECT state_machine_id, name, title, description, subject_table_name,
  subject_state_column, state_count, transition_rule_count, relative_path FROM vw_state_machines`;
const S_SELECT = `SELECT machine_state_id, state_machine, state_key, title,
  order_index::int AS order_index, is_initial, is_terminal, relative_path FROM vw_machine_states`;
const R_SELECT = `SELECT state_transition_rule_id, state_machine, from_state, to_state,
  from_state_key, to_state_key, guard_description, rule_refs, trigger_endpoint,
  triggered_by_role, is_forward_edge, relative_path FROM vw_state_transition_rules`;
const T_SELECT = `SELECT state_transition_id, state_machine, subject_table_name, subject_id,
  from_state_key, to_state_key, transition_at, triggered_by_role, reason, is_forward, relative_path
  FROM vw_state_transitions`;
// Per-state cohort occupancy — how many subjects CURRENTLY occupy each state, and
// how many have ever passed through it. Both counts are DERIVED (IsCurrent on the
// bitemporal occupancy chain), never entered. This is the disease-state simulator's
// admin witness: the progression machine's states light up with their live cohort.
const OCC_SELECT = `SELECT state_key,
  count(*) FILTER (WHERE is_current)::int AS current_occupancy,
  count(*)::int AS ever_occupancy
  FROM vw_subject_state_instances`;

const STATE_WRITABLE = new Set(['state_key', 'title', 'order_index', 'is_initial', 'is_terminal']);
const RULE_WRITABLE = new Set(['from_state', 'to_state', 'guard_description', 'rule_refs', 'trigger_endpoint', 'triggered_by_role']);

const slug = (s) => (s || '').toString().toLowerCase().replace(/[^a-z0-9]+/g, '');

// GET /api/state-machines — list.
router.get('/', async (_req, res) => {
  try { res.json(await query(`${M_SELECT} ORDER BY state_machine_id`)); }
  catch (err) { res.status(500).json({ error: String(err) }); }
});

// GET /api/state-machines/:id — machine + states + rules + recent transitions.
router.get('/:id', async (req, res) => {
  try {
    const m = await query(`${M_SELECT} WHERE state_machine_id = $1`, [req.params.id]);
    if (!m.length) return res.status(404).json({ error: 'not_found', id: req.params.id });
    const [states, rules, transitions, occupancy] = await Promise.all([
      query(`${S_SELECT} WHERE state_machine = $1 ORDER BY order_index`, [req.params.id]),
      query(`${R_SELECT} WHERE state_machine = $1`, [req.params.id]),
      query(`${T_SELECT} WHERE state_machine = $1 ORDER BY transition_at NULLS LAST LIMIT 200`, [req.params.id]),
      query(`${OCC_SELECT} WHERE state_machine = $1 GROUP BY state_key`, [req.params.id]),
    ]);
    // graft the derived occupancy onto each state so the client never recomputes it
    const occByKey = Object.fromEntries(occupancy.map((o) => [o.state_key, o]));
    const statesWithOccupancy = states.map((s) => ({
      ...s,
      current_occupancy: occByKey[s.state_key]?.current_occupancy ?? 0,
      ever_occupancy: occByKey[s.state_key]?.ever_occupancy ?? 0,
    }));
    res.json({ machine: m[0], states: statesWithOccupancy, rules, transitions });
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// PUT /api/state-machines/:id — patch machine meta.
router.put('/:id', async (req, res) => {
  try {
    const allow = new Set(['title', 'description', 'subject_table_name', 'subject_state_column']);
    const sets = [], vals = [];
    for (const [k, v] of Object.entries(req.body || {})) if (allow.has(k)) { vals.push(v); sets.push(`${k} = $${vals.length}`); }
    if (!sets.length) return res.status(400).json({ error: 'no_writable_fields' });
    vals.push(req.params.id);
    await query(`UPDATE state_machines SET ${sets.join(', ')} WHERE state_machine_id = $${vals.length}`, vals);
    const rows = await query(`${M_SELECT} WHERE state_machine_id = $1`, [req.params.id]);
    res.json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// POST /api/state-machines/:id/states — add a state.
router.post('/:id/states', async (req, res) => {
  try {
    const b = req.body || {};
    if (!b.state_key) return res.status(400).json({ error: 'state_key_required' });
    const id = `${req.params.id}--${slug(b.state_key)}`;
    const cols = ['machine_state_id', 'state_machine'], vals = [id, req.params.id];
    for (const [k, v] of Object.entries(b)) if (STATE_WRITABLE.has(k)) { cols.push(k); vals.push(v); }
    const ph = vals.map((_, i) => `$${i + 1}`).join(', ');
    await query(`INSERT INTO machine_states (${cols.join(', ')}) VALUES (${ph})`, vals);
    const rows = await query(`${S_SELECT} WHERE machine_state_id = $1`, [id]);
    res.status(201).json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// PUT /api/state-machines/states/:stateId — patch a state.
router.put('/states/:stateId', async (req, res) => {
  try {
    const sets = [], vals = [];
    for (const [k, v] of Object.entries(req.body || {})) if (STATE_WRITABLE.has(k)) { vals.push(v); sets.push(`${k} = $${vals.length}`); }
    if (!sets.length) return res.status(400).json({ error: 'no_writable_fields' });
    vals.push(req.params.stateId);
    await query(`UPDATE machine_states SET ${sets.join(', ')} WHERE machine_state_id = $${vals.length}`, vals);
    const rows = await query(`${S_SELECT} WHERE machine_state_id = $1`, [req.params.stateId]);
    if (!rows.length) return res.status(404).json({ error: 'not_found' });
    res.json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// DELETE /api/state-machines/states/:stateId — refuse if referenced by a rule.
router.delete('/states/:stateId', async (req, res) => {
  try {
    const ref = await query('SELECT count(*)::int AS n FROM state_transition_rules WHERE from_state = $1 OR to_state = $1', [req.params.stateId]);
    if (ref[0].n > 0) return res.status(400).json({ error: 'state_referenced_by_rules', count: ref[0].n });
    await query('DELETE FROM machine_states WHERE machine_state_id = $1', [req.params.stateId]);
    res.json({ ok: true });
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// POST /api/state-machines/:id/rules — add a transition rule.
router.post('/:id/rules', async (req, res) => {
  try {
    const b = req.body || {};
    if (!b.from_state || !b.to_state) return res.status(400).json({ error: 'from_and_to_required' });
    const keys = await query('SELECT machine_state_id, state_key FROM machine_states WHERE machine_state_id IN ($1,$2)', [b.from_state, b.to_state]);
    const keyOf = (mid) => (keys.find((k) => k.machine_state_id === mid) || {}).state_key || slug(mid);
    const id = `${req.params.id}--${keyOf(b.from_state)}->${keyOf(b.to_state)}`.toLowerCase();
    const cols = ['state_transition_rule_id', 'state_machine'], vals = [id, req.params.id];
    for (const [k, v] of Object.entries(b)) if (RULE_WRITABLE.has(k)) { cols.push(k); vals.push(v); }
    const ph = vals.map((_, i) => `$${i + 1}`).join(', ');
    await query(`INSERT INTO state_transition_rules (${cols.join(', ')}) VALUES (${ph})`, vals);
    const rows = await query(`${R_SELECT} WHERE state_transition_rule_id = $1`, [id]);
    res.status(201).json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// PUT /api/state-machines/rules/:ruleId — patch a rule.
router.put('/rules/:ruleId', async (req, res) => {
  try {
    const sets = [], vals = [];
    for (const [k, v] of Object.entries(req.body || {})) if (RULE_WRITABLE.has(k)) { vals.push(v); sets.push(`${k} = $${vals.length}`); }
    if (!sets.length) return res.status(400).json({ error: 'no_writable_fields' });
    vals.push(req.params.ruleId);
    await query(`UPDATE state_transition_rules SET ${sets.join(', ')} WHERE state_transition_rule_id = $${vals.length}`, vals);
    const rows = await query(`${R_SELECT} WHERE state_transition_rule_id = $1`, [req.params.ruleId]);
    if (!rows.length) return res.status(404).json({ error: 'not_found' });
    res.json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// DELETE /api/state-machines/rules/:ruleId
router.delete('/rules/:ruleId', async (req, res) => {
  try {
    await query('DELETE FROM state_transition_rules WHERE state_transition_rule_id = $1', [req.params.ruleId]);
    res.json({ ok: true });
  } catch (err) { res.status(500).json({ error: String(err) }); }
});
