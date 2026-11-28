// What the pages share (the explainer in app.js, the ontology form in admin.js, the concept
// form in concepts.js): escaping, the rulebook's version, what each kind of control does, the
// link to a form's rulebook editor, noticing a file saved from outside, a form's tools panel,
// Compare the engines, the list of unsaved changes, and watching a server job.
window.RulebookAdmin = (function () {
  const esc = (t) => String(t ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

  // The rulebook file's version, top right on every page. With unsaved changes the page is
  // no longer that version, and the label says so.
  function showVersion(el, T, version, unsaved) {
    el.textContent = T(unsaved ? 'version-label-unsaved' : 'version-label', { number: version });
    el.title = T('version-hint', { number: version });
    el.classList.toggle('dirty', unsaved > 0);
  }

  // Each control says what kind it is (data-kind: page, newtab, download, upload); the
  // stylesheet draws the kind's mark and this gives it the kind's words on hover.
  function nameKinds(T, texts, root = document) {
    for (const el of root.querySelectorAll('[data-kind]')) if (texts[`kind-${el.dataset.kind}`] != null) el.title = T(`kind-${el.dataset.kind}`);
  }

  // A form's rulebook editor (the generated editor, in a container of its own) is always a
  // link: straight to the editor when it is running, otherwise to the page of this app that
  // starts it and goes there when it answers. The note says so while it is not running.
  const editorHref = (editor) => (editor.state === 'running' ? editor.url : `/editor.html?form=${encodeURIComponent(editor.form)}`);
  const editorNote = (editor, T) => (editor.state === 'running' ? '' : T('editor-starts'));
  function editorLink(link, note, editor, T) {
    link.href = editorHref(editor);
    note.textContent = editorNote(editor, T);
  }

  // A file saved from outside this app (a rulebook editor's Save) is taken in by the server when
  // a page asks how things stand. Every page asks every few seconds; when the answer is no longer
  // the one it is showing, it loads what it shows again and says the server's news.
  //   state: what the page loaded ({ version, hubChangedOnDisk, file, news }); reload(): load it again.
  // Returns seen(state), which the page calls with what it loads itself (after its own Save, say),
  // so that it is not loaded a second time. A cell being typed in is not pulled from under the reader.
  function watchFiles({ api, state, reload, say }) {
    const mark = (s) => [s.news?.id ?? 0, s.version, s.hubChangedOnDisk, s.file.changedOnDisk, s.file.why].join('|');
    let shown = mark(state), told = state.news?.id ?? 0;
    // News made while this page was loading (the load itself took the file in) is said now.
    if (state.news && state.news.at >= performance.timeOrigin) say(state.news.message, state.news.bad);
    const typing = () => { const el = document.activeElement; return !!el && !!el.closest('#grid') && /^(INPUT|TEXTAREA)$/.test(el.tagName) && el.type !== 'checkbox'; };
    setInterval(async () => {
      try {
        const now = await api('GET', '/api/admin/changes');
        if (mark(now) === shown || typing()) return;
        shown = mark(now);
        await reload();
        if (now.news && now.news.id !== told) say(now.news.message, now.news.bad);
        told = now.news?.id ?? 0;
      } catch (e) { console.error(e); }
    }, 3000);
    return (s) => { shown = mark(s); };
  }

  // A form's tools panel: what each tool adds, and its link. 'editor' is the form's rulebook
  // editor; a tool that is the page it is listed on says so; a page of this app opens here,
  // anything else (a generated page, a file) opens beside it.
  function toolList(tools, { T, editor }) {
    const here = location.pathname === '/' ? '/index.html' : location.pathname;
    return tools.map((t) => {
      const [page, anchor] = (t.href || '').split('#');
      let link = '';
      if (t.href === 'editor') {
        link = `<a href="${esc(editorHref(editor))}" target="_blank" rel="noopener" data-kind="newtab">${esc(t.link_text)}</a><span class="off">${esc(editorNote(editor, T))}</span>`;
      } else if (page === here) {
        link = anchor ? `<a href="#${esc(anchor)}">${esc(t.link_text)}</a>` : `<span class="off">${esc(T('tool-here'))}</span>`;
      } else if (t.href) {
        link = /^\/(admin|concepts)\.html/.test(t.href) ? `<a href="${esc(t.href)}" data-kind="page">${esc(t.link_text)}</a>`
                                                      : `<a href="${esc(t.href)}" target="_blank" rel="noopener" data-kind="newtab">${esc(t.link_text)}</a>`;
      }
      return `<li data-tool="${esc(t.rulebook_tool_id)}"><b>${esc(t.label)}</b><span>${esc(t.adds)}</span>${link}</li>`;
    }).join('');
  }

  // Compare the engines on the current facts, and say in the page's conformance box how they agree.
  async function compareEngines({ api, T, box, show = 12 }) {
    box.className = 'conformance';
    box.textContent = T('conformance-running');
    try {
      const r = await api('POST', '/api/conformance');
      const agree = r.differingCount === 0 && r.missingRowCount === 0;
      box.className = 'conformance' + (agree ? '' : ' differ');
      box.innerHTML = esc(agree ? T('conformance-agree', { engines: r.engines.length, cells: r.cells })
                                : T('conformance-differ', { count: r.differingCount + r.missingRowCount, cells: r.cells })) +
        `<div class="muted">${esc(r.engines.map((e) => `${e.label} ${e.ms} ms`).join(' \u00b7 '))}</div>` +
        (r.skipped.length ? `<div class="muted">${r.skipped.map((s) => esc(T('engine-unavailable', { engine: s.label, reason: s.reason }))).join('<br>')}</div>` : '') +
        (agree ? '' : '<ul>' + r.differing.slice(0, show).map((d) =>
          `<li>${esc(d.table)}.${esc(d.field)} of ${esc(d.key)}: ${Object.entries(d.values).map(([k, v]) => `${esc(k)} = ${esc(JSON.stringify(v))}`).join(', ')}</li>`).join('') + '</ul>');
    } catch (e) {
      box.className = 'conformance differ';
      box.textContent = e.message;
    }
  }

  // The unsaved changes as list items: what differs from the rulebook file, a few per table.
  function changeLines(changes) {
    const lines = [];
    for (const [name, c] of Object.entries(changes.tables)) {
      for (const k of c.added.slice(0, 15)) lines.push(`<li><b>${esc(name)}</b> + ${esc(k)}</li>`);
      if (c.added.length > 15) lines.push(`<li><b>${esc(name)}</b> + ${c.added.length - 15} more rows</li>`);
      for (const k of c.removed.slice(0, 15)) lines.push(`<li><b>${esc(name)}</b> − ${esc(k)}</li>`);
      if (c.removed.length > 15) lines.push(`<li><b>${esc(name)}</b> − ${c.removed.length - 15} more rows</li>`);
      for (const ch of c.changed.slice(0, 30)) {
        lines.push(`<li><b>${esc(name)}</b> ${esc(ch.key)}: ${Object.entries(ch.fields).map(([f, [a, b]]) => `${esc(f)} ${esc(JSON.stringify(a))} → ${esc(JSON.stringify(b))}`).join(', ')}</li>`);
      }
      if (c.changed.length > 30) lines.push(`<li><b>${esc(name)}</b> ${c.changed.length - 30} more changed rows</li>`);
    }
    return lines;
  }

  // The long actions run on the server as jobs: the page starts one, then polls it, so it
  // stays usable meanwhile. The job's button shows how long it has been running.
  // kinds: { <kind>: { control, text, label, running, render?(job), started?(), done(job), failed(job) } }
  function jobs({ api, say, T, $, kinds }) {
    function render(kind, job) {
      const j = kinds[kind];
      $(j.control).disabled = job.running;
      $(j.text).closest('.buttonish')?.classList.toggle('off', job.running);
      $(j.text).textContent = job.running ? T(j.running, { seconds: job.seconds }) : T(j.label);
      if (j.render) j.render(job);
    }
    // announce: this page started (or saw running) the job, so it reports how it ended.
    async function watch(kind, announce) {
      const job = await api('GET', `/api/admin/jobs/${kind}`);
      render(kind, job);
      if (job.running) { setTimeout(() => watch(kind, true).catch((e) => say(e.message, true)), 1000); return; }
      if (announce && job.finishedAt) job.ok ? await kinds[kind].done(job) : say(kinds[kind].failed(job), true);
    }
    async function start(kind, body) {
      say('');
      try {
        await api('POST', `/api/admin/jobs/${kind}`, null, body);
        if (kinds[kind].started) kinds[kind].started();
        await watch(kind, true);
      } catch (e) { console.error(e); say(e.message, true); }
    }
    return { watch, start };
  }

  return { esc, showVersion, nameKinds, editorLink, watchFiles, toolList, compareEngines, changeLines, jobs };
})();
