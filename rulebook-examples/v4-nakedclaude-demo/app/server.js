const path = require('path');
const express = require('express');
const session = require('express-session');
const { pool, newId } = require('./db');
const { TABLES, BY_KEY, labelColFor } = require('./tables');
const shopRouter = require('./shop');

const app = express();
const PORT = process.env.PORT || 3011;

app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));
app.use(express.urlencoded({ extended: true }));
app.use(express.static(path.join(__dirname, 'public')));
app.use(session({
  secret: 'naked-claude-demo',
  resave: false,
  saveUninitialized: true
}));

// expose nav data to all templates
app.use((req, res, next) => {
  res.locals.TABLES = TABLES;
  res.locals.path = req.path;
  res.locals.activeCustomerId = req.session.activeCustomerId || null;
  next();
});

async function fkOptions(viewKey) {
  const t = BY_KEY[viewKey];
  if (!t) return [];
  const labelCol = labelColFor(viewKey);
  const { rows } = await pool.query(
    `SELECT ${t.pk} AS id, ${labelCol} AS label FROM ${viewKey} ORDER BY label NULLS LAST LIMIT 500`
  );
  return rows;
}

// ---- Index: row counts per table ----
app.get('/', async (req, res) => {
  const counts = {};
  for (const t of TABLES) {
    try {
      const { rows } = await pool.query(`SELECT COUNT(*)::int AS n FROM ${t.key}`);
      counts[t.key] = rows[0].n;
    } catch (e) {
      counts[t.key] = '—';
    }
  }
  res.render('index', { counts });
});

// ---- List ----
app.get('/t/:view', async (req, res, next) => {
  try {
    const t = BY_KEY[req.params.view];
    if (!t) return res.status(404).send('Unknown view');
    const { rows, fields } = await pool.query(`SELECT * FROM ${t.key} LIMIT 500`);
    res.render('table', { t, rows, columns: fields.map(f => f.name) });
  } catch (e) { next(e); }
});

// ---- New form ----
app.get('/t/:view/new', async (req, res, next) => {
  try {
    const t = BY_KEY[req.params.view];
    if (!t) return res.status(404).send('Unknown view');
    const fkData = await loadFks(t);
    res.render('form', { t, row: {}, fkData, isNew: true, error: null });
  } catch (e) { next(e); }
});

// ---- Edit form ----
app.get('/t/:view/:id/edit', async (req, res, next) => {
  try {
    const t = BY_KEY[req.params.view];
    if (!t) return res.status(404).send('Unknown view');
    const { rows } = await pool.query(`SELECT * FROM ${t.key} WHERE ${t.pk} = $1`, [req.params.id]);
    if (!rows[0]) return res.status(404).send('Not found');
    const fkData = await loadFks(t);
    res.render('form', { t, row: rows[0], fkData, isNew: false, error: null });
  } catch (e) { next(e); }
});

async function loadFks(t) {
  const fkData = {};
  for (const f of t.fields) {
    if (f.type.startsWith('fk:')) {
      fkData[f.name] = await fkOptions(f.type.slice(3));
    }
  }
  return fkData;
}

function coerce(field, raw) {
  if (raw === undefined) return null;
  if (field.type === 'bool') return raw === 'on' || raw === 'true' || raw === '1';
  if (raw === '') return null;
  if (field.type === 'number') return Number(raw);
  if (field.type === 'datetime') return new Date(raw);
  return raw;
}

// ---- Create ----
app.post('/t/:view/new', async (req, res, next) => {
  try {
    const t = BY_KEY[req.params.view];
    if (!t) return res.status(404).send('Unknown view');
    const id = newId(t.idPrefix);
    const cols = [t.pk];
    const vals = [id];
    for (const f of t.fields) {
      const v = coerce(f, f.type === 'bool' ? (req.body[f.name] ? 'on' : '') : req.body[f.name]);
      cols.push(f.name);
      vals.push(v);
    }
    const placeholders = vals.map((_, i) => `$${i + 1}`).join(', ');
    await pool.query(
      `INSERT INTO ${t.table} (${cols.map(c => `"${c}"`).join(', ')}) VALUES (${placeholders})`,
      vals
    );
    res.redirect(`/t/${t.key}`);
  } catch (e) { next(e); }
});

// ---- Update ----
app.post('/t/:view/:id/edit', async (req, res, next) => {
  try {
    const t = BY_KEY[req.params.view];
    if (!t) return res.status(404).send('Unknown view');
    const sets = [];
    const vals = [];
    for (const f of t.fields) {
      const v = coerce(f, f.type === 'bool' ? (req.body[f.name] ? 'on' : '') : req.body[f.name]);
      sets.push(`"${f.name}" = $${vals.length + 1}`);
      vals.push(v);
    }
    vals.push(req.params.id);
    await pool.query(
      `UPDATE ${t.table} SET ${sets.join(', ')} WHERE ${t.pk} = $${vals.length}`,
      vals
    );
    res.redirect(`/t/${t.key}`);
  } catch (e) { next(e); }
});

// ---- Delete ----
app.post('/t/:view/:id/delete', async (req, res, next) => {
  try {
    const t = BY_KEY[req.params.view];
    if (!t) return res.status(404).send('Unknown view');
    await pool.query(`DELETE FROM ${t.table} WHERE ${t.pk} = $1`, [req.params.id]);
    res.redirect(`/t/${t.key}`);
  } catch (e) { next(e); }
});

app.use('/shop', shopRouter);

app.use((err, req, res, next) => {
  console.error(err);
  res.status(500).send(`<pre>${err.stack || err.message}</pre>`);
});

app.listen(PORT, () => {
  console.log(`v4-nakedclaude-demo listening on http://localhost:${PORT}`);
});
