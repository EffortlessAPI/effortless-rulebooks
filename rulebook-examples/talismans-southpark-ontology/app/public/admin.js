// The rulebook admin: every table of the rulebook as a grid. Stored values are
// editable and go straight to the live rows; computed values are shown as the
// chosen engine computes them. Save writes the rows back into the rulebook file.
(function () {
  const $ = (id) => document.getElementById(id);
  const { esc, changeLines, showVersion, nameKinds, toolList, compareEngines } = window.RulebookAdmin;   // shared with the other pages
  const MAX_ROWS = 200, MAX_OPTIONS = 80;
  let texts = {}, live = {}, tables = [], current = null, rows = [], engines = [], explainerReady = false, version = null;
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

  // ---------------------------------------------------------------- header: engines, changes
  function renderEngines() {
    const mine = engines.find((e) => e.id === engineId);
    $('engines').innerHTML = engines.map((e) =>
      `<button type="button" data-engine="${esc(e.id)}" aria-pressed="${e.id === engineId}"${e.ok ? '' : ' disabled'} title="${esc(e.ok ? e.description : T('engine-unavailable', { engine: e.label, reason: e.reason }))}">${esc(e.label)}</button>`).join('');
    $('engine-description').textContent = mine ? mine.description : '';
  }
  function renderChanges(changes, hubChangedOnDisk) {
    const u = $('unsaved');
    u.textContent = changes.total === 1 ? T('unsaved-one') : changes.total ? T('unsaved', { count: changes.total }) : T('unsaved-none');
    if (hubChangedOnDisk) say(T('admin-hub-changed'), true);
    u.classList.toggle('dirty', changes.total > 0);
    $('save').disabled = !changes.total;
    $('discard').disabled = !changes.total && !hubChangedOnDisk;
    $('changes').hidden = !changes.total;
    $('change-list').innerHTML = changeLines(changes).join('');
    showVersion($('version'), T, version, changes.total);
  }

  // ---------------------------------------------------------------- tables and the grid
  async function loadTables() {
    const r = await api('GET', '/api/admin/tables');
    tables = r.tables;
    version = r.version;
    live = await api('GET', '/api/rows');
    texts = Object.fromEntries(live.PageTexts.map((x) => [x.PageTextId, x.Text]));
    for (const el of document.querySelectorAll('[data-text]')) el.textContent = T(el.dataset.text);
    nameKinds(T, texts);
    document.title = T('form-switch-ontology');
    $('filter').placeholder = T('admin-filter');
    $('tables').innerHTML = tables.map((t) =>
      `<button type="button" data-table="${esc(t.name)}" aria-pressed="${current === t.name}">${esc(t.name)} <span>${t.rowCount}</span></button>`).join('');
    renderChanges(r.changes, r.hubChangedOnDisk);
    renderEngines();
  }
  async function loadTable(name) {
    current = name;
    const computed = $('show-computed').checked ? '1' : '0';
    const r = await api('GET', `/api/admin/tables/${encodeURIComponent(name)}?computed=${computed}`);
    rows = r.rows;
    for (const b of document.querySelectorAll('[data-table]')) b.setAttribute('aria-pressed', String(b.dataset.table === name));
    $('table-name').textContent = name;
    $('table-description').textContent = r.table.description;
    renderGrid(r.table);
  }

  function editor(table, field, row, key) {
    const value = row[field.name];
    const attrs = `data-field="${esc(field.name)}" data-key="${esc(key)}"`;
    if (field.name === table.key) return `<code>${esc(value)}</code>`;
    if (field.datatype === 'boolean') return `<input type="checkbox" ${attrs}${value ? ' checked' : ''}>`;
    if (field.type === 'relationship') {
      const target = tables.find((t) => t.name === field.relatedTo);
      const options = (live[field.relatedTo] || []).map((r) => String(r[target.key]));
      if (options.length <= MAX_OPTIONS) {
        return `<select ${attrs}><option value=""></option>${options.map((o) => `<option${o === String(value ?? '') ? ' selected' : ''}>${esc(o)}</option>`).join('')}</select>`;
      }
    }
    const type = field.datatype === 'integer' || field.datatype === 'number' ? 'number' : 'text';
    return `<input type="${type}" ${attrs} value="${esc(value)}">`;
  }

  function renderGrid(table) {
    const showComputed = $('show-computed').checked;
    const fields = table.fields.filter((f) => f.stored || showComputed);
    const needle = $('filter').value.trim().toLowerCase();
    const matching = needle ? rows.filter((row) => fields.some((f) => String(row[f.name] ?? '').toLowerCase().includes(needle))) : rows;
    const shown = matching.slice(0, MAX_ROWS);
    $('row-count').textContent = T('admin-rows', { shown: shown.length, total: rows.length });
    const head = '<thead><tr><th></th>' + fields.map((f) =>
      `<th class="${f.stored ? '' : 'computed'}" title="${esc(f.description)}${f.formula ? `\n${esc(f.formula)}` : ''}">${esc(f.name)}<small>${esc(f.stored ? f.datatype : f.type)}</small></th>`).join('') + '</tr></thead>';
    const body = shown.map((row) => {
      const key = String(row[table.key]);
      return `<tr><td><button type="button" class="x" data-delete="${esc(key)}">${esc(T('admin-delete-row'))}</button></td>` + fields.map((f) =>
        f.stored ? `<td>${editor(table, f, row, key)}</td>`
                 : `<td class="computed"><span data-er-dag="${esc(table.name)}.${esc(f.name)}" data-er-row="${esc(key)}">${esc(row[f.name])}</span></td>`).join('') + '</tr>';
    }).join('');
    $('grid').innerHTML = head + '<tbody>' + body + '</tbody>';
    $('grid').dataset.table = table.name;
    if (showComputed && window.EffortlessExplainer) {
      if (!explainerReady) { window.EffortlessExplainer.init({ mode: 'modal', corner: 'bottom-right' }); explainerReady = true; }
      else window.EffortlessExplainer.enhanceCells(document);
      window.EffortlessExplainer.setRows(live);
    }
  }
  const tableNow = () => tables.find((t) => t.name === current);

  async function afterWrite(result) {
    if (result && result.candidates && (result.candidates.added || result.candidates.removed)) say(T('admin-candidates', result.candidates));
    await loadTables();
    await loadTable(current);
  }

  // ---------------------------------------------------------------- events
  document.addEventListener('click', (e) => {
    const t = e.target.closest('[data-table],[data-engine],[data-delete]');
    if (!t) return;
    if (t.dataset.table) busy(() => loadTable(t.dataset.table));
    else if (t.dataset.engine) { engineId = t.dataset.engine; localStorage.setItem('erb.backend', engineId); renderEngines(); if ($('show-computed').checked) busy(() => loadTable(current)); }
    else if (t.dataset.delete !== undefined) busy(async () => afterWrite(await api('DELETE', `/api/admin/tables/${encodeURIComponent(current)}/rows/${encodeURIComponent(t.dataset.delete)}`)));
  });
  $('grid').addEventListener('change', (e) => {
    const t = e.target;
    if (t.dataset.field === undefined) return;
    const value = t.type === 'checkbox' ? t.checked : t.value;
    if (t.dataset.key === '') return;   // the new-row line is sent as a whole by its Add button
    say('');
    busy(async () => afterWrite(await api('PATCH', `/api/admin/tables/${encodeURIComponent(current)}/rows/${encodeURIComponent(t.dataset.key)}`, { [t.dataset.field]: value })));
  });
  $('add-row').addEventListener('click', () => {
    const table = tableNow();
    if ($('new-row')) return;
    const cells = table.fields.filter((f) => f.stored || $('show-computed').checked).map((f) => {
      if (!f.stored) return '<td class="computed"></td>';
      if (f.name === table.key) return `<td><input type="text" data-new="${esc(f.name)}" placeholder="${esc(f.name)}"></td>`;
      return `<td>${editor(table, f, {}, '').replace(/data-field=/g, 'data-new=')}</td>`;
    }).join('');
    $('grid').querySelector('tbody').insertAdjacentHTML('afterbegin', `<tr id="new-row"><td><button type="button" class="primary" id="new-row-add">${esc(T('admin-add-row'))}</button></td>${cells}</tr>`);
    $('new-row-add').addEventListener('click', () => {
      const values = {};
      for (const input of $('new-row').querySelectorAll('[data-new]')) values[input.dataset.new] = input.type === 'checkbox' ? input.checked : input.value;
      busy(async () => afterWrite(await api('POST', `/api/admin/tables/${encodeURIComponent(current)}/rows`, values)));
    });
  });
  $('filter').addEventListener('input', () => renderGrid(tableNow()));
  $('show-computed').addEventListener('change', () => busy(() => loadTable(current)));
  $('save').addEventListener('click', () => busy(async () => { const r = await api('POST', '/api/admin/save'); await loadTables(); say(r.message); }));
  $('discard').addEventListener('click', () => busy(async () => { const r = await api('POST', '/api/admin/discard'); await loadTables(); await loadTable(current); say(r.message); }));
  $('import').addEventListener('change', async (e) => {
    const file = e.target.files[0];
    if (!file) return;
    const body = await file.arrayBuffer();
    e.target.value = '';
    jobs.start('import', body);
  });
  $('export').addEventListener('click', () => jobs.start('export'));
  $('compare').addEventListener('click', () => compareEngines({ api, T, box: $('conformance'), show: 25 }));

  // The long actions run on the server as jobs (RulebookAdmin.jobs starts and watches them).
  const JOBS = {
    rebuild: {
      control: 'rebuild', text: 'rebuild', label: 'admin-rebuild', running: 'admin-rebuild-running',
      render(job) { $('buildlog').hidden = !job.startedAt; $('buildlog').textContent = job.tail.join('\n'); },
      started() { say(T('admin-build-running')); },
      async done() { say(T('admin-build-done')); await loadTables(); await loadTable(current); },
      failed: () => T('admin-build-failed'),
    },
    export: {
      control: 'export', text: 'export', label: 'admin-export', running: 'admin-export-running',
      async done(job) { say(T('admin-exported', { file: job.result.file })); location.href = '/api/admin/jobs/export/download'; },
      failed: (job) => job.error,
    },
    import: {
      control: 'import', text: 'import-text', label: 'admin-import', running: 'admin-import-running',
      async done(job) {
        const r = job.result;
        await loadTables(); await loadTable(current);
        say(r.message + (r.schemaDiffers.length ? ' ' + T('admin-import-schema', { count: r.schemaDiffers.length, examples: r.schemaDiffers.slice(0, 3).join('; ') }) : ''));
      },
      failed: (job) => job.error,
    },
  };
  const jobs = window.RulebookAdmin.jobs({ api, say, T, $, kinds: JOBS });
  $('rebuild').addEventListener('click', () => jobs.start('rebuild'));

  (async () => {
    const listed = await api('GET', '/api/engines');
    engines = listed.engines;
    const mine = engines.find((e) => e.id === engineId && e.ok) || engines.find((e) => e.id === listed.defaultId && e.ok) || engines.find((e) => e.ok);
    if (!mine) throw new Error(engines.map((e) => `${e.label}: ${e.reason}`).join(' | '));
    engineId = mine.id;
    await loadTables();
    await loadTable(tables.find((t) => t.name === 'Statements') ? 'Statements' : tables[0].name);
    const ed = await api('GET', '/api/admin/editor');
    if (ed.running) $('editor').href = ed.url; else { $('editor').removeAttribute('href'); $('editor').removeAttribute('data-kind'); $('editor').classList.add('off'); $('editor-note').textContent = T('admin-editor-off'); }
    // The ontology form's tools, as the explainer page lists them.
    $('tools').innerHTML = toolList((await api('GET', '/api/tools?form=ontology')).tools, { T, editor: ed, editorOff: 'admin-editor-off' });
    nameKinds(T, texts, $('tools'));
    for (const kind of Object.keys(JOBS)) await jobs.watch(kind, false);
  })().catch((e) => { console.error(e); say(e.message, true); });
})();
