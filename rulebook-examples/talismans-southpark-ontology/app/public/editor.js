// Starting a form's rulebook editor. The editor is the generated one, in a container of its
// own; a link to it comes here while it is not running. This page asks the app to start it,
// shows how the launcher is getting on, and goes to the editor as soon as it answers (the
// editor shows its own progress from there).
(function () {
  const $ = (id) => document.getElementById(id);
  const form = new URLSearchParams(location.search).get('form') || '';
  const PAGE = { ontology: '/admin.html', concept: '/concepts.html' };
  let texts = {};
  const T = (key, values) => {
    if (texts[key] == null) throw new Error(`The rulebook has no page text "${key}"`);
    return values ? texts[key].replace(/\{(\w+)\}/g, (_, k) => values[k]) : texts[key];
  };
  async function api(method, url) {
    const res = await fetch(url, { method });
    const json = await res.json();
    if (!res.ok) throw new Error(json.error || `${method} ${url} answered ${res.status}`);
    return json;
  }
  const failed = (reason) => {
    $('status').textContent = T('editor-failed');
    $('status').classList.add('bad');
    $('tail').textContent = reason;
    $('tail').hidden = false;
    $('retry').hidden = false;
  };
  function show(editor) {
    if (editor.state === 'running') { location.replace(editor.url); return true; }
    if (editor.state === 'failed') { failed(editor.reason); return true; }
    $('status').classList.remove('bad');
    $('status').textContent = T('editor-starting', { seconds: editor.seconds ?? 0 });
    $('tail').textContent = (editor.tail || []).join('\n');
    $('tail').hidden = !(editor.tail || []).length;
    $('retry').hidden = true;
    return false;
  }
  async function watch() {
    try {
      if (!show(await api('GET', `/api/editor?form=${encodeURIComponent(form)}`))) setTimeout(watch, 1000);
    } catch (e) { console.error(e); failed(e.message); }
  }
  async function start() {
    try {
      if (!show(await api('POST', `/api/editor/start?form=${encodeURIComponent(form)}`))) setTimeout(watch, 1000);
    } catch (e) { console.error(e); failed(e.message); }
  }
  $('retry').addEventListener('click', start);

  (async () => {
    const live = await api('GET', '/api/rows');
    texts = Object.fromEntries(live.PageTexts.map((x) => [x.PageTextId, x.Text]));
    for (const el of document.querySelectorAll('[data-text]')) el.textContent = T(el.dataset.text);
    if (!PAGE[form]) throw new Error(`There is no rulebook editor for the form ${JSON.stringify(form)}`);
    document.body.classList.toggle('concepts', form === 'concept');
    document.title = $('heading').textContent = T(`editor-heading-${form}`);
    $('back').href = PAGE[form];
    $('back').textContent = T(form === 'concept' ? 'form-switch-concepts' : 'form-switch-ontology');
    window.RulebookAdmin.nameKinds(T, texts);
    await start();
  })().catch((e) => { console.error(e); $('status').textContent = e.message; $('status').classList.add('bad'); });
})();
