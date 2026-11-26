// The South Park explainer's server. It reads the rulebook's vw_* views and writes
// the rulebook's base tables; it holds no rule of its own. Every conclusion, card,
// alert, count and button state on the page is a column of a view.
import express from 'express';
import pg from 'pg';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const HERE = path.dirname(fileURLToPath(import.meta.url));
const PROJECT = path.resolve(HERE, '..');
const PORT = Number(process.env.PORT || 43210);
// Same database scripts/erb_project.py loads; DATABASE_URL only overrides it.
const DATABASE_URL = process.env.DATABASE_URL || 'postgresql://postgres@localhost:5432/erb_talismans_southpark_ontology';
const pool = new pg.Pool({ connectionString: DATABASE_URL });

async function rows(client, sql, params = []) {
  return (await client.query(sql, params)).rows;
}
async function one(client, sql, params = []) {
  const r = await rows(client, sql, params);
  if (r.length !== 1) throw new Error(`Expected one row from: ${sql} (${JSON.stringify(params)}), got ${r.length}`);
  return r[0];
}
const fill = (template, values) => template.replace(/\{(\w+)\}/g, (_, k) => {
  if (!(k in values)) throw new Error(`Page text ${JSON.stringify(template)} names {${k}} but no value was given`);
  return values[k];
});

async function texts(client) {
  return Object.fromEntries((await rows(client, 'SELECT page_text_id, text FROM vw_page_texts')).map(r => [r.page_text_id, r.text]));
}

// The current-record summary under the Stan buttons, from the facts in effect.
async function recordSummary(client, t) {
  const subject = await one(client, 'SELECT thing_id FROM vw_things WHERE label = $1', [t['record-subject']]);
  const isPerson = (await rows(client,
    `SELECT 1 FROM vw_statements s JOIN vw_properties p ON p.property_id = s.property
      WHERE s.subject = $1 AND p.property_kind = 'type' AND s.object_label = $2 AND s.is_in_effect`, [subject.thing_id, t['record-class']])).length > 0;
  const sentences = [isPerson ? t['record-person-yes'] : t['record-person-no']];
  for (const p of await rows(client, 'SELECT property_id, record_kind FROM vw_properties WHERE record_kind IS NOT NULL ORDER BY record_order')) {
    const values = (await rows(client,
      `SELECT object_text FROM vw_statements WHERE subject = $1 AND property = $2 AND is_in_effect AND is_listed ORDER BY sort_order`,
      [subject.thing_id, p.property_id])).map(r => r.object_text);
    sentences.push(values.length ? fill(t['record-value'], { kind: p.record_kind, values: values.join(', ') }) : fill(t['record-none'], { kind: p.record_kind }));
  }
  return sentences.join(' ');
}

async function page(client) {
  const t = await texts(client);
  const layouts = {};
  for (const r of await rows(client, 'SELECT layout, node_key, x, y FROM vw_node_layouts')) (layouts[r.layout] ||= {})[r.node_key] = [r.x, r.y];
  return {
    texts: t,
    graph: await one(client, `SELECT graph_id, count_of_stated_facts, count_of_conclusions, count_of_violations, is_consistent,
                                     conclusions_message, has_facts_to_add FROM vw_graphs`),
    rules: await rows(client, 'SELECT rule_id, label, explanation, is_enabled FROM vw_rules ORDER BY sort_order'),
    scenarios: await rows(client, `SELECT scenario_id, label, button_group, is_primary, is_available FROM vw_scenarios ORDER BY sort_order`),
    properties: await rows(client, `SELECT property_id, label, datatype, input_name, current_value_wording, has_one_current_value
                                      FROM vw_properties WHERE is_user_addable ORDER BY sort_order`),
    things: await rows(client, `SELECT thing_id, label, kind, node_key, node_kind, is_in_graph, count_of_children, count_of_grandchildren,
                                       portrait_name, portrait_page, portrait_image, portrait_crop, portrait_width, portrait_height
                                  FROM vw_things ORDER BY sort_order`),
    facts: await rows(client, `SELECT statement_id, subject_label, property_label, object_text, is_literal, property_datatype,
                                      is_in_effect, is_built_in, source
                                 FROM vw_statements WHERE is_listed ORDER BY sort_order`),
    edges: await rows(client, `SELECT statement_id, subject_node_key, object_node_key, subject_label, property_label, object_text,
                                      is_literal, property, property_datatype, object_node_kind, object_value_node_label,
                                      is_conclusion, is_in_contradiction, source
                                 FROM vw_statements WHERE holds
                                ORDER BY is_conclusion, CASE WHEN is_conclusion THEN first_fired_order_key ELSE sort_order END`),
    conclusions: await rows(client, `SELECT firing_id, target, target_subject_label, target_property_label, target_object_text,
                                            target_subject_node_key, target_object_node_key, card_text
                                       FROM vw_firings WHERE is_chosen ORDER BY order_key`),
    alarms: await rows(client, `SELECT disjoint_check_id, alarm_text FROM vw_disjoint_checks WHERE violation_unit = 1 ORDER BY disjoint_check_id`),
    layouts,
    slots: (await rows(client, 'SELECT x, y FROM vw_layout_slots ORDER BY sort_order')).map(r => [r.x, r.y]),
    record: await recordSummary(client, t),
  };
}

// Raw rows for the explainer's witness: the base tables, exactly as stored.
const BASE_TABLES = { Graphs: 'graphs', Rules: 'rules', FiringKinds: 'firing_kinds', Things: 'things', Properties: 'properties',
  Statements: 'statements', Firings: 'firings', DisjointChecks: 'disjoint_checks' };

// ------------------------------------------------------------------ writes (base tables only)
async function nextSeq(client) {
  return (await one(client, 'SELECT next_activation_seq FROM vw_graphs')).next_activation_seq;
}
async function switchOn(client, id, seq) {
  const r = await client.query('UPDATE statements SET is_stated = true, activation_seq = $2 WHERE statement_id = $1 AND is_listed', [id, seq]);
  if (r.rowCount !== 1) throw new Error(`No listed fact ${id}`);
}
async function switchOff(client, id) {
  const r = await client.query('UPDATE statements SET is_stated = false WHERE statement_id = $1 AND is_listed', [id]);
  if (r.rowCount !== 1) throw new Error(`No listed fact ${id}`);
}
async function addToList(client, id) {
  const r = await client.query(
    `UPDATE statements SET is_listed = true, sort_order = (SELECT next_listed_sort_order FROM vw_graphs) WHERE statement_id = $1 AND NOT is_listed`, [id]);
  if (r.rowCount === 0 && !(await rows(client, 'SELECT 1 FROM statements WHERE statement_id = $1', [id])).length)
    throw new Error(`The rulebook has no row for ${id}`);
}

// A button: its fact baseline, then its explicit changes, then its rule settings.
async function runScenario(client, id) {
  const sc = await one(client, `SELECT scenario_id, fact_baseline, note, layout, clears_selection, clears_form_message, is_available
                                  FROM vw_scenarios WHERE scenario_id = $1`, [id]);
  let seq = await nextSeq(client);
  const listed = await rows(client, 'SELECT statement_id, is_built_in, is_detail, is_next_to_add FROM vw_statements WHERE is_listed ORDER BY sort_order');
  const set = async (r, on) => (on ? switchOn(client, r.statement_id, seq++) : switchOff(client, r.statement_id));
  switch (sc.fact_baseline) {
    case 'keep': break;
    case 'built-in': for (const r of listed) await set(r, r.is_built_in); break;
    case 'built-in-detail': for (const r of listed) await set(r, r.is_built_in && r.is_detail); break;
    case 'all-off': for (const r of listed) await set(r, false); break;
    case 'activate-all': for (const r of listed) await set(r, true); break;
    case 'next': for (const r of listed.filter(r => r.is_next_to_add)) await set(r, true); break;
    default: throw new Error(`Scenario ${id} has an unknown fact baseline: ${sc.fact_baseline}`);
  }
  for (const c of await rows(client, `SELECT subject, property, object_key, action, adds_to_list FROM vw_scenario_fact_changes
                                       WHERE scenario = $1 ORDER BY sort_order`, [id])) {
    const exact = c.object_key == null ? null : `${c.subject}|${c.property}|${c.object_key}`;
    if (exact && c.adds_to_list) await addToList(client, exact);
    const matched = await rows(client, `SELECT statement_id FROM vw_statements WHERE is_listed AND subject = $1 AND property = $2
                                          AND ($3::text IS NULL OR statement_id = $3) ORDER BY sort_order`, [c.subject, c.property, exact]);
    for (const m of matched) await set(m, c.action === 'on');
  }
  for (const r of await rows(client, 'SELECT rule, is_enabled FROM vw_scenario_rule_settings WHERE scenario = $1', [id]))
    await client.query('UPDATE rules SET is_enabled = $2 WHERE rule_id = $1', [r.rule, r.is_enabled]);
  return { note: sc.note || '', layout: sc.layout, clearsSelection: sc.clears_selection, clearsFormMessage: sc.clears_form_message };
}

function integerValue(value) {  // the page's own input check, unchanged
  if (!/^[+-]?\d+$/.test(value)) return null;
  const negative = value.charAt(0) === '-';
  const digits = value.replace(/^[+-]/, '').replace(/^0+(?=\d)/, '');
  return (negative && digits !== '0' ? '-' : '') + digits;
}

// The Add fact form. Returns { added, message }.
async function addFact(client, { subject, property, object }) {
  const t = await texts(client);
  const s = String(subject || '').trim(), o0 = String(object || '').trim();
  const prop = (await rows(client, 'SELECT property_id, label, datatype, input_name, current_value_wording FROM vw_properties WHERE property_id = $1 AND is_user_addable', [property]))[0];
  if (!prop) throw new Error(`Not a property the form offers: ${property}`);
  const literal = !!prop.datatype;
  if (!s || !o0 || (s === o0 && !literal)) return { added: false, message: null };
  let o = o0;
  if (prop.datatype === 'xsd:integer') {
    o = integerValue(o0);
    if (o === null) return { added: false, message: fill(t['integer-error'], { field: prop.input_name }) };
  }
  const subj = (await rows(client, 'SELECT thing_id FROM vw_things WHERE label = $1', [s]))[0];
  const obj = literal ? { key: o } : (await rows(client, 'SELECT thing_id AS key FROM vw_things WHERE label = $1', [o]))[0];
  const id = subj && obj ? `${subj.thing_id}|${prop.property_id}|${obj.key}` : null;
  if (!id || !(await rows(client, 'SELECT 1 FROM statements WHERE statement_id = $1', [id])).length)
    return { added: false, message: fill(t['not-in-rulebook'], { fact: `${s} ${prop.label} ${o}` }) };
  await addToList(client, id);
  await switchOn(client, id, await nextSeq(client));
  const message = prop.current_value_wording ? fill(t['form-replaced'], { subject: s, kind: prop.current_value_wording, value: o }) : '';
  return { added: true, message };
}

async function removeFact(client, id) {
  const r = await client.query(
    `UPDATE statements SET is_listed = false, is_stated = false, sort_order = 10000 + activation_seq
      WHERE statement_id = $1 AND is_listed AND NOT is_built_in`, [id]);
  if (r.rowCount !== 1) throw new Error(`No removable fact ${id}`);
}

// ------------------------------------------------------------------ HTTP
const app = express();
app.use(express.json());
const tx = (fn) => async (req, res) => {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    const result = await fn(client, req);
    await client.query('COMMIT');
    res.json({ ...result, page: await page(client) });
  } catch (e) {
    await client.query('ROLLBACK');
    console.error(e);
    res.status(400).json({ error: e.message });
  } finally {
    client.release();
  }
};
const read = (fn) => async (req, res) => {
  try { res.json(await fn(pool, req)); } catch (e) { console.error(e); res.status(500).json({ error: e.message }); }
};

app.get('/api/health', read(async (db) => ({ ok: true, views: (await rows(db, "SELECT count(*)::int AS n FROM information_schema.views WHERE table_name LIKE 'vw\\_%'"))[0].n })));
app.get('/api/page', read((db) => page(db)));
app.get('/api/rows', read(async (db) => {
  const out = {};
  for (const [table, base] of Object.entries(BASE_TABLES)) out[table] = await rows(db, `SELECT * FROM ${base}`);
  return out;
}));
app.post('/api/facts/:id/switch', tx(async (c, req) => { req.body.on ? await switchOn(c, req.params.id, await nextSeq(c)) : await switchOff(c, req.params.id); return {}; }));
app.post('/api/facts/:id/remove', tx(async (c, req) => { await removeFact(c, req.params.id); return {}; }));
app.post('/api/facts', tx((c, req) => addFact(c, req.body)));
app.post('/api/rules/:id/switch', tx(async (c, req) => {
  const r = await c.query('UPDATE rules SET is_enabled = $2 WHERE rule_id = $1', [req.params.id, !!req.body.on]);
  if (r.rowCount !== 1) throw new Error(`No rule ${req.params.id}`);
  return {};
}));
app.post('/api/scenarios/:id/run', tx((c, req) => runScenario(c, req.params.id)));

app.use('/rulebook-explainer-dag', express.static(path.join(PROJECT, 'explainer/rulebook-explainer-dag'), { maxAge: 0 }));
app.use('/rulespeak', express.static(path.join(PROJECT, 'rulespeak')));
app.use('/xlsx', express.static(path.join(PROJECT, 'xlsx')));
app.use('/rulebook', express.static(path.join(PROJECT, 'effortless-rulebook')));
app.use('/', express.static(path.join(HERE, 'public')));

app.listen(PORT, '127.0.0.1', () => console.log(`South Park explainer on http://localhost:${PORT} (database ${DATABASE_URL})`));
