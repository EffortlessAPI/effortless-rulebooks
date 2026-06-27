// routes/routing.js — RoutingAndNavigation surface.
//
// Reads vw_routing_and_navigation, writes the routing_and_navigation base table
// (project rule: read views, write base tables). Builds the role-filtered nav
// tree from route_key / parent_route_key, exactly like the portal RouteDesigner.
import express from 'express';
import { query } from '../db.js';

export const router = express.Router();

const SELECT = `
  SELECT routing_and_navigation_id, name, display_name, route, description,
         sort_order::float8 AS sort_order, parent_route_key, route_key, nav_level,
         role_visibility, primary_table, primary_view, icon_hint, is_dynamic, pin_to_top,
         depth, full_path, handler_base_name, relative_path,
         admin_crud, intake_clinician_crud, diagnosing_doctor_crud, external_llm_crud,
         admin_can_read, intake_clinician_can_read, diagnosing_doctor_can_read, external_llm_can_read
  FROM vw_routing_and_navigation`;

// Editable base-table columns.
const WRITABLE = new Set([
  'display_name', 'route', 'description', 'sort_order', 'parent_route_key', 'route_key',
  'nav_level', 'role_visibility', 'primary_table', 'primary_view', 'icon_hint',
  'is_dynamic', 'pin_to_top', 'admin_crud', 'intake_clinician_crud',
  'diagnosing_doctor_crud', 'external_llm_crud',
]);

// Whether a row is visible to `role` (RoleVisibility CSV; admin sees all).
const visibleTo = (row, role) => {
  if (!role || role === 'admin') return true;
  const csv = (row.role_visibility || '').toLowerCase();
  return csv.split(',').map((s) => s.trim()).includes(role);
};

// Build a parent/child tree from flat rows via route_key/parent_route_key (cycle-guarded).
function buildTree(rows) {
  const byKey = new Map();
  for (const r of rows) if (r.route_key) byKey.set(r.route_key, { ...r, children: [] });
  const roots = [];
  for (const node of byKey.values()) {
    const pk = node.parent_route_key;
    const parent = pk ? byKey.get(pk) : null;
    if (parent && parent !== node) parent.children.push(node);
    else roots.push(node);
  }
  const sortRec = (nodes) => {
    nodes.sort((a, b) => (a.sort_order ?? 0) - (b.sort_order ?? 0));
    nodes.forEach((n) => sortRec(n.children));
  };
  sortRec(roots);
  return roots;
}

// GET /api/routing — flat list (ordered).
router.get('/', async (_req, res) => {
  try {
    res.json(await query(`${SELECT} ORDER BY sort_order, route_key`));
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// GET /api/routing/tree?role=intake-clinician|diagnosing-doctor|admin
router.get('/tree', async (req, res) => {
  try {
    const role = (req.query.role || 'admin').toString();
    const rows = (await query(`${SELECT} ORDER BY sort_order, route_key`)).filter((r) => visibleTo(r, role));
    res.json({ role, tree: buildTree(rows) });
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// GET /api/routing/:id — one node.
router.get('/:id', async (req, res) => {
  try {
    const rows = await query(`${SELECT} WHERE routing_and_navigation_id = $1`, [req.params.id]);
    if (!rows.length) return res.status(404).json({ error: 'not_found', id: req.params.id });
    res.json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// POST /api/routing — create a node.
router.post('/', async (req, res) => {
  try {
    const b = req.body || {};
    const id = b.routing_and_navigation_id || `nav-${(b.route_key || Date.now()).toString().replace(/\./g, '-')}`;
    const cols = ['routing_and_navigation_id'];
    const vals = [id];
    for (const [k, v] of Object.entries(b)) {
      if (WRITABLE.has(k)) { cols.push(k); vals.push(v); }
    }
    const ph = vals.map((_, i) => `$${i + 1}`).join(', ');
    await query(`INSERT INTO routing_and_navigation (${cols.join(', ')}) VALUES (${ph})`, vals);
    const rows = await query(`${SELECT} WHERE routing_and_navigation_id = $1`, [id]);
    res.status(201).json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// PUT /api/routing/:id — patch editable columns.
router.put('/:id', async (req, res) => {
  try {
    const sets = [], vals = [];
    for (const [k, v] of Object.entries(req.body || {})) {
      if (WRITABLE.has(k)) { vals.push(v); sets.push(`${k} = $${vals.length}`); }
    }
    if (!sets.length) return res.status(400).json({ error: 'no_writable_fields' });
    vals.push(req.params.id);
    await query(`UPDATE routing_and_navigation SET ${sets.join(', ')} WHERE routing_and_navigation_id = $${vals.length}`, vals);
    const rows = await query(`${SELECT} WHERE routing_and_navigation_id = $1`, [req.params.id]);
    if (!rows.length) return res.status(404).json({ error: 'not_found', id: req.params.id });
    res.json(rows[0]);
  } catch (err) { res.status(500).json({ error: String(err) }); }
});

// DELETE /api/routing/:id — delete + re-parent children up one level.
router.delete('/:id', async (req, res) => {
  try {
    const cur = await query('SELECT route_key, parent_route_key FROM routing_and_navigation WHERE routing_and_navigation_id = $1', [req.params.id]);
    if (!cur.length) return res.status(404).json({ error: 'not_found', id: req.params.id });
    const { route_key, parent_route_key } = cur[0];
    let reparented = 0;
    if (route_key) {
      const r = await query('UPDATE routing_and_navigation SET parent_route_key = $1 WHERE parent_route_key = $2', [parent_route_key ?? '', route_key]);
      reparented = r.length ?? 0;
    }
    await query('DELETE FROM routing_and_navigation WHERE routing_and_navigation_id = $1', [req.params.id]);
    res.json({ ok: true, reparented });
  } catch (err) { res.status(500).json({ error: String(err) }); }
});
