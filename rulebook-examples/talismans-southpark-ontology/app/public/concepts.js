// The concept form: the same ontology as ordinary tables. A table of individuals, a
// table per class, a table or a column per property, and beside each class and
// property the attributes its fields cannot say (label, symmetric, inverse of...).
//
// This page holds no model of its own. The server gives it the concept rulebook of
// the live rows and what each table is; every edit is sent as one of the mapping's
// edits and the server answers with the concept rulebook as it now reads.
(function () {
  const $ = (id) => document.getElementById(id);
  const { esc, changeLines } = window.RulebookAdmin;
  const MAX_ROWS = 300;
  const GROUPS = ['individuals', 'class', 'relationship', 'values', 'attributes', 'support', 'meta'];
  const NEW = { class: 'concepts-new-class', relationship: 'concepts-new-relationship', values: 'concepts-new-values' };
  const PROPERTY_PICKS = new Set(['InverseOf', 'ChainFirst', 'ChainSecond']);
  let texts = {}, engines = [], view = null, current = 'Things', creating = null, armed = null;
  let engineId = localStorage.getItem('erb.backend');
  const T = (key, values) => {
    if (texts[key] == null) throw new Error(`The rulebook has no page text "${key}"`);
    return values ? texts[key].replace(/\{(\w+)\}/g, (_, k) => values[k]) : texts[key];
  };

  async function api(method, url, body, raw) {
    const headers = raw ? {} : { 'content-type': 'application/json' };
    if (engineId) headers['X-ERB-Backend'] = engineId;
    const res = await fetch(url, { method, headers, body: raw || (body ? JSON.stringify(body) : undefined) });
    const json = await res.json();
    if (!res.ok) throw new Error(json.error || `${method} ${url} answered ${res.status}`);
    return json;
  }
  const say = (message, bad) => { const m = $('message'); m.textContent = message || ''; m.classList.toggle('bad', !!bad); };
  const busy = async (fn) => {
    document.body.classList.add('busy');
    try { return await fn(); } catch (e) { console.error(e); say(e.message, true); } finally { document.body.classList.remove('busy'); }
  };

  // ---------------------------------------------------------------- what the server said
  const table = (name) => view.concepts[name];
  const info = (name) => view.description.tables.find((t) => t.name === name);
  const property = (id) => view.description.properties.find((p) => p.id === id);
  const keyOf = (name) => info(name).key;
  const keys = (name) => table(name).data.map((row) => String(row[keyOf(name)]));
  const stored = (name) => table(name).schema.filter((f) => f.type === 'raw' || f.type === 'relationship');
  const isFacts = (role) => role === 'relationship' || role === 'values';
  const attributesOf = (name) => table('ERBTables').data.find((row) => row.ERBTableId === name) || null;
  const targets = () => ['Things', ...view.description.classes];
  const targetName = (t) => (t === 'Things' ? T('concepts-any-thing') : T('concepts-a-member', { class: (attributesOf(t) || {}).Label || t }));

  // ---------------------------------------------------------------- edits
  async function edit(edits, after) {
    say('');
    await busy(async () => {
      const result = await api('POST', '/api/concepts/edit', edits);
      view = result;
      if (after) after();
      if (!table(current)) current = 'Things';
      render();
      const c = result.candidates;
      say(c && (c.added || c.removed) ? T('admin-candidates', c) : T('concepts-applied'));
    });
  }
  // An attribute of a class or a property that is a table: a cell of its ERBTables row (made when it has none).
  const setAttribute = (name, values) => edit([
    ...(attributesOf(name) ? [] : [{ op: 'insert', table: 'ERBTables', values: { ERBTableId: name } }]),
    { op: 'patch', table: 'ERBTables', key: name, values },
  ]);

  // ---------------------------------------------------------------- header
  function renderEngines() {
    const mine = engines.find((e) => e.id === engineId);
    $('engines').innerHTML = engines.map((e) =>
      `<button type="button" data-engine="${esc(e.id)}" aria-pressed="${e.id === engineId}"${e.ok ? '' : ' disabled'} title="${esc(e.ok ? e.description : T('engine-unavailable', { engine: e.label, reason: e.reason }))}">${esc(e.label)}</button>`).join('');
    $('engine-description').textContent = mine ? mine.description : '';
  }
  function renderChanges() {
    const { changes, hubChangedOnDisk } = view;
    const u = $('unsaved');
    u.textContent = changes.total === 1 ? T('unsaved-one') : changes.total ? T('unsaved', { count: changes.total }) : T('unsaved-none');
    u.classList.toggle('dirty', changes.total > 0);
    if (hubChangedOnDisk) say(T('admin-hub-changed'), true);
    $('save').disabled = !changes.total;
    $('discard').disabled = !changes.total && !hubChangedOnDisk;
    $('changes').hidden = !changes.total;
    $('change-list').innerHTML = changeLines(changes).join('');
    // The concept rulebook file: what its rulebook editor saves. Changed on disk, it waits to be loaded.
    $('load-file').disabled = !view.file.exists;
    $('load-file').classList.toggle('primary', view.file.changedOnDisk);
    $('file-note').hidden = !view.file.changedOnDisk;
    $('file-note-text').textContent = view.file.changedOnDisk ? T('concepts-file-changed', { file: view.file.file }) : '';
  }

  // ---------------------------------------------------------------- the list of tables, by what each is
  function renderTables() {
    const html = [];
    for (const role of GROUPS) {
      const mine = view.description.tables.filter((t) => t.role === role);
      if (!mine.length && !NEW[role]) continue;
      html.push(`<div class="group-title">${esc(T(`concepts-group-${role}`))}</div>`);
      for (const t of mine) {
        html.push(`<button type="button" data-table="${esc(t.name)}" aria-pressed="${!creating && current === t.name}">${esc(t.name)} <span>${table(t.name).data.length}</span></button>`);
      }
      if (NEW[role]) html.push(`<button type="button" class="new-concept" data-create="${role}" aria-pressed="${creating === role}">+ ${esc(T(NEW[role]))}</button>`);
    }
    $('tables').innerHTML = html.join('');
  }

  // ---------------------------------------------------------------- what this table is, and what its fields cannot say
  const picker = (attrs, options, value, blank) =>
    `<select ${attrs}>${blank === undefined ? '' : `<option value="">${esc(blank)}</option>`}${options.map(([v, label]) => `<option value="${esc(v)}"${v === (value ?? '') ? ' selected' : ''}>${esc(label)}</option>`).join('')}</select>`;
  const targetOptions = () => targets().map((t) => [t, t === 'Things' ? `Things (${T('concepts-any-thing')})` : t]);
  const propertyOptions = (except) => view.description.properties.filter((p) => p.id !== except).map((p) => [p.id, p.id]);
  const line = (label, control) => `<label class="about-line"><span>${esc(label)}</span>${control}</label>`;
  const remover = (what, label) => `<button type="button" class="x danger" data-remove="${esc(what)}">${esc(armed === what ? T('concepts-remove-confirm') : label)}</button>`;

  function holdsOptions() {
    return [...view.description.datatypes.map((d) => [`value:${d}`, T('concepts-holds-value', { datatype: d })]),
      ...targets().map((t) => [`link:${t}`, T('concepts-holds-link', { table: t })])];
  }
  function columnsPanel(name) {
    const columns = info(name).columns.map((id) => {
      const p = property(id);
      const holds = p.kind === 'object' ? T('concepts-holds-link', { table: p.range || 'Things' }) : T('concepts-holds-value', { datatype: p.datatype });
      return `<li><b>${esc(id)}</b> <span>${esc(holds)}</span> ${remover(`column:${name}:${id}`, T('concepts-remove-column'))}</li>`;
    });
    return `<div class="columns"><h3>${esc(T('concepts-columns'))}</h3>
      ${columns.length ? `<ul class="chips">${columns.join('')}</ul>` : `<p class="hint">${esc(T('concepts-no-columns'))}</p>`}
      <form class="mini" data-form="column">
        <input type="text" name="name" placeholder="${esc(T('concepts-column-name'))}" autocomplete="off" required>
        <input type="text" name="label" placeholder="${esc(T('concepts-column-label'))}" autocomplete="off">
        ${picker('name="holds"', holdsOptions(), 'value:string')}
        <button type="submit">${esc(T('concepts-add-column'))}</button>
      </form></div>`;
  }

  function aboutTable(name) {
    const t = info(name), role = t.role;
    const a = attributesOf(name) || {};
    if (role === 'individuals') return `<p>${esc(T('concepts-role-individuals'))}</p>${columnsPanel(name)}`;
    if (role === 'class') {
      return `<p>${esc(T('concepts-role-class', { label: a.Label || name }))}</p>
        <div class="about-lines">${line(T('concepts-label'), `<input type="text" data-attribute="Label" value="${esc(a.Label)}">`)}</div>
        ${columnsPanel(name)}
        <p class="about-end">${remover(`table:${name}`, T('concepts-remove-class'))}</p>`;
    }
    if (isFacts(role)) {
      const p = property(name);
      const lines = [
        line(T('concepts-label'), `<input type="text" data-attribute="Label" value="${esc(a.Label)}">`),
        line(T('concepts-subject-is'), picker('data-retarget="Subject"', targetOptions(), p.domain || 'Things')),
        role === 'relationship'
          ? line(T('concepts-object-is'), picker('data-retarget="Object"', targetOptions(), p.range || 'Things'))
          : line(T('concepts-value-is'), `<code>${esc(T('concepts-holds-value', { datatype: p.datatype }))}</code>`),
        role === 'relationship' ? line(T('concepts-symmetric'), `<input type="checkbox" data-attribute="IsSymmetric"${a.IsSymmetric ? ' checked' : ''}>`) : '',
        line(T('concepts-inverse-of'), picker('data-attribute="InverseOf"', propertyOptions(name), a.InverseOf, T('concepts-none'))),
        line(T('concepts-chain'), `${picker('data-attribute="ChainFirst"', propertyOptions(), a.ChainFirst, T('concepts-none'))} <i>${esc(T('concepts-chain-then'))}</i> ${picker('data-attribute="ChainSecond"', propertyOptions(), a.ChainSecond, T('concepts-none'))}`),
        line(T('concepts-disjoint-with'), `<input type="text" data-attribute="DisjointWith" value="${esc(a.DisjointWith)}" placeholder="${esc(T('concepts-disjoint-placeholder'))}">`),
        line(T('concepts-user-addable'), `<input type="checkbox" data-attribute="IsUserAddable"${a.IsUserAddable ? ' checked' : ''}>`),
      ];
      const sentence = role === 'relationship'
        ? T('concepts-role-relationship', { subject: targetName(p.domain || 'Things'), label: a.Label || name, object: targetName(p.range || 'Things') })
        : T('concepts-role-values', { subject: targetName(p.domain || 'Things'), label: a.Label || name, datatype: p.datatype });
      return `<p>${esc(sentence)}</p><div class="about-lines">${lines.join('')}</div>
        <p class="about-end">${remover(`table:${name}`, T('concepts-remove-property'))}</p>`;
    }
    return `<p>${esc(T(`concepts-role-${role}`))}</p>`;
  }

  function aboutNew(role) {
    const fields = [
      `<input type="text" name="name" placeholder="${esc(T('concepts-new-name'))}" autocomplete="off" required>`,
      `<input type="text" name="label" placeholder="${esc(T('concepts-new-label'))}" autocomplete="off">`,
    ];
    if (role !== 'class') fields.push(line(T('concepts-subject-is'), picker('name="subject"', targetOptions(), 'Things')));
    if (role === 'relationship') fields.push(line(T('concepts-object-is'), picker('name="object"', targetOptions(), 'Things')));
    if (role === 'values') fields.push(line(T('concepts-value-is'), picker('name="datatype"', view.description.datatypes.map((d) => [d, T('concepts-holds-value', { datatype: d })]), 'string')));
    return `<h2>${esc(T(NEW[role]))}</h2><p>${esc(T(`${NEW[role]}-intro`))}</p>
      <form class="mini tall" data-form="new" data-role="${role}">${fields.join('')}
        <span><button type="submit" class="primary">${esc(T('concepts-add'))}</button> <button type="button" data-cancel>${esc(T('concepts-cancel'))}</button></span>
      </form>`;
  }

  // ---------------------------------------------------------------- the grid
  function editable(name, field) {
    const role = info(name).role;
    if (field.name === keyOf(name)) return false;                            // a key is set once (and derived for a membership or a fact)
    if (role === 'attributes' && view.description.follows.includes(field.name)) return false;   // follows the table or field the row is about
    return true;
  }
  function options(name, field) {
    if (PROPERTY_PICKS.has(field.name) && info(name).role === 'attributes') return view.description.properties.map((p) => p.id);
    if (field.type !== 'relationship') return null;
    // A link to a class's table names an individual: stating the fact is what makes it a member.
    const target = field.RelatedTo === 'Things' || view.description.classes.includes(field.RelatedTo) ? 'Things' : field.RelatedTo;
    return table(target) ? keys(target) : null;
  }
  function cell(name, field, row, key, isNew) {
    const value = row[field.name];
    const attrs = isNew ? `data-new="${esc(field.name)}"` : `data-field="${esc(field.name)}" data-key="${esc(key)}"`;
    const frozen = !isNew && (!editable(name, field) || (name === '__meta__' && row.MetaKey === 'ontology-form'));
    if (frozen) return `<code>${esc(value)}</code>`;
    if (field.datatype === 'boolean') return `<input type="checkbox" ${attrs}${value || (isNew && field.name === 'IsAsserted') ? ' checked' : ''}>`;
    const choices = options(name, field);
    if (choices) return `<select ${attrs}><option value=""></option>${choices.map((o) => `<option${o === String(value ?? '') ? ' selected' : ''}>${esc(o)}</option>`).join('')}</select>`;
    const type = field.datatype === 'integer' || field.datatype === 'number' ? 'number' : 'text';
    return `<input type="${type}" ${attrs} value="${esc(value)}">`;
  }
  // The fields a new row is asked for: not the key of a membership or a fact (made from the row's other values).
  const asked = (name) => stored(name).filter((f) => !(f.name === keyOf(name) && (info(name).role === 'class' || isFacts(info(name).role))));

  function renderGrid() {
    const name = current, t = info(name), fields = stored(name), rows = table(name).data;
    const fixedRows = t.role === 'attributes';
    const needle = $('filter').value.trim().toLowerCase();
    const matching = needle ? rows.filter((row) => fields.some((f) => String(row[f.name] ?? '').toLowerCase().includes(needle))) : rows;
    const shown = matching.slice(0, MAX_ROWS);
    $('row-count').textContent = T('admin-rows', { shown: shown.length, total: rows.length });
    $('add-row').hidden = fixedRows;
    const head = '<thead><tr><th></th>' + fields.map((f) =>
      `<th title="${esc(f.Description)}">${esc(f.name)}<small>${esc(f.type === 'relationship' ? `→ ${f.RelatedTo}` : f.datatype)}</small></th>`).join('') + '</tr></thead>';
    const body = shown.map((row) => {
      const key = String(row[t.key]);
      const first = fixedRows || (name === '__meta__' && row.MetaKey === 'ontology-form') ? '<td></td>'
        : `<td><button type="button" class="x" data-delete="${esc(key)}">${esc(T('admin-delete-row'))}</button></td>`;
      return `<tr>${first}${fields.map((f) => `<td>${cell(name, f, row, key, false)}</td>`).join('')}</tr>`;
    }).join('');
    $('grid').innerHTML = head + '<tbody>' + body + '</tbody>';
  }

  function render() {
    renderChanges();
    renderTables();
    $('about').innerHTML = creating ? aboutNew(creating) : aboutTable(current);
    document.querySelector('.table-card').hidden = !!creating;
    if (creating) return;
    $('table-name').textContent = current;
    $('table-description').textContent = table(current).Description || '';
    renderGrid();
  }

  // ---------------------------------------------------------------- events
  document.addEventListener('click', (e) => {
    const t = e.target.closest('[data-table],[data-create],[data-engine],[data-delete],[data-remove],[data-cancel]');
    if (!t) return;
    if (t.dataset.remove === undefined) armed = null;
    if (t.dataset.table) { current = t.dataset.table; creating = null; render(); }
    else if (t.dataset.create) { creating = t.dataset.create; render(); }
    else if (t.dataset.cancel !== undefined) { creating = null; render(); }
    else if (t.dataset.engine) { engineId = t.dataset.engine; localStorage.setItem('erb.backend', engineId); renderEngines(); busy(load); }
    else if (t.dataset.delete !== undefined) edit([{ op: 'remove', table: current, key: t.dataset.delete }]);
    else if (t.dataset.remove) {
      const what = t.dataset.remove;
      if (armed !== what) { armed = what; render(); return; }     // the second click removes
      armed = null;
      const [kind, tableName, column] = what.split(':');
      edit(kind === 'table' ? [{ op: 'removeTable', table: tableName }] : [{ op: 'removeColumn', table: tableName, name: column }],
        () => { if (kind === 'table') current = 'Things'; });
    }
  });
  $('about').addEventListener('change', (e) => {
    const t = e.target;
    if (t.closest('form')) return;                                 // a form is sent as a whole
    if (t.dataset.attribute) setAttribute(current, { [t.dataset.attribute]: t.type === 'checkbox' ? t.checked : t.value });
    else if (t.dataset.retarget) edit([{ op: 'retarget', table: current, field: t.dataset.retarget, relatedTo: t.value }]);
  });
  $('about').addEventListener('submit', (e) => {
    e.preventDefault();
    const form = e.target, v = Object.fromEntries(new FormData(form));
    if (form.dataset.form === 'column') {
      const [kind, what] = v.holds.split(':');
      edit([{ op: 'addColumn', table: current, name: v.name.trim(), label: v.label.trim(), ...(kind === 'value' ? { datatype: what } : { object: what }) }]);
    } else {
      const role = form.dataset.role, name = v.name.trim(), label = v.label.trim();
      const op = role === 'class' ? { op: 'addClass', name, label }
        : { op: 'addProperty', name, label, subject: v.subject, ...(role === 'values' ? { datatype: v.datatype } : { object: v.object }) };
      edit([op], () => { creating = null; current = name; });
    }
  });
  $('grid').addEventListener('change', (e) => {
    const t = e.target;
    if (t.dataset.field === undefined) return;                     // the new-row line is sent as a whole by its Add button
    edit([{ op: 'patch', table: current, key: t.dataset.key, values: { [t.dataset.field]: t.type === 'checkbox' ? t.checked : t.value } }]);
  });
  $('add-row').addEventListener('click', () => {
    if ($('new-row')) return;
    const name = current, wanted = new Set(asked(name).map((f) => f.name));
    const cells = stored(name).map((f) => `<td>${wanted.has(f.name) ? cell(name, f, {}, '', true) : ''}</td>`).join('');
    $('grid').querySelector('tbody').insertAdjacentHTML('afterbegin', `<tr id="new-row"><td><button type="button" class="primary" id="new-row-add">${esc(T('admin-add-row'))}</button></td>${cells}</tr>`);
    $('new-row-add').addEventListener('click', () => {
      const values = {};
      for (const input of $('new-row').querySelectorAll('[data-new]')) values[input.dataset.new] = input.type === 'checkbox' ? input.checked : input.value;
      edit([{ op: 'insert', table: name, values }]);
    });
  });
  $('filter').addEventListener('input', renderGrid);
  $('save').addEventListener('click', () => busy(async () => { const r = await api('POST', '/api/admin/save'); await load(); say(r.message); }));
  $('discard').addEventListener('click', () => busy(async () => { const r = await api('POST', '/api/admin/discard'); await load(); say(r.message); }));
  for (const [id, url] of [['load-file', '/api/concepts/load-file'], ['write-file', '/api/concepts/write-file']]) {
    $(id).addEventListener('click', () => busy(async () => { const r = await api('POST', url); view = r; if (!table(current)) current = 'Things'; render(); say(r.message); }));
  }

  // The concept rulebook as a workbook, both ways: the same catalog tools as the ontology form's, on the concept rulebook.
  const JOBS = {
    'concept-export': {
      control: 'export', text: 'export', label: 'concepts-export', running: 'admin-export-running',
      async done(job) { say(T('admin-exported', { file: job.result.file })); location.href = '/api/admin/jobs/concept-export/download'; },
      failed: (job) => job.error,
    },
    'concept-import': {
      control: 'import', text: 'import-text', label: 'concepts-import', running: 'admin-import-running',
      async done(job) { await load(); say(job.result.message); },
      failed: (job) => job.error,
    },
  };
  const jobs = window.RulebookAdmin.jobs({ api, say, T, $, kinds: JOBS });
  $('export').addEventListener('click', () => jobs.start('concept-export'));
  $('import').addEventListener('change', async (e) => {
    const file = e.target.files[0];
    if (!file) return;
    const body = await file.arrayBuffer();
    e.target.value = '';
    jobs.start('concept-import', body);
  });

  async function load() {
    view = await api('GET', '/api/concepts');
    if (!table(current)) current = 'Things';
    render();
  }

  (async () => {
    const live = await api('GET', '/api/rows');
    texts = Object.fromEntries(live.PageTexts.map((x) => [x.PageTextId, x.Text]));
    for (const el of document.querySelectorAll('[data-text]')) el.textContent = T(el.dataset.text);
    document.title = T('concepts-title');
    $('filter').placeholder = T('admin-filter');
    const listed = await api('GET', '/api/engines');
    engines = listed.engines;
    const mine = engines.find((e) => e.id === engineId && e.ok) || engines.find((e) => e.id === listed.defaultId && e.ok) || engines.find((e) => e.ok);
    if (!mine) throw new Error(engines.map((e) => `${e.label}: ${e.reason}`).join(' | '));
    engineId = mine.id;
    renderEngines();
    await load();
    const ed = await api('GET', '/api/concepts/editor');
    if (ed.running) $('editor').href = ed.url; else { $('editor').removeAttribute('href'); $('editor').classList.add('off'); $('editor-note').textContent = T('concepts-editor-off'); }
    for (const kind of Object.keys(JOBS)) await jobs.watch(kind, false);
  })().catch((e) => { console.error(e); say(e.message, true); });
})();
