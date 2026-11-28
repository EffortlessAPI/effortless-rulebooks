// One of the rulebook's two files, shown as it is written and coloured as JSON. Each line of the
// file is one line here (a field or a row of a table, in the single-line-leaves layout), numbered,
// with a link to it (#L<number>), and the list at the top jumps to where each table starts.
// Nothing is parsed or rewritten: the colours come from the file's own text.
(function () {
  const $ = (id) => document.getElementById(id);
  const { esc, showVersion, nameKinds } = window.RulebookAdmin;
  const params = new URLSearchParams(location.search);
  const form = params.get('form') || '';
  const live = params.has('live');
  // The two files, where the app serves them; &live is the concept rulebook of the live rows.
  const FORMS = {
    ontology: { file: '/rulebook/talismans-southpark-ontology-rulebook.json', back: '/admin.html', backText: 'form-switch-ontology' },
    concept: { file: '/concept-rulebook/talismans-southpark-ontology-concepts.json', live: '/api/concepts/rulebook.json', back: '/concepts.html', backText: 'form-switch-concepts' },
  };
  let texts = {};
  const T = (key, values) => {
    if (texts[key] == null) throw new Error(`The rulebook has no page text "${key}"`);
    return values ? texts[key].replace(/\{(\w+)\}/g, (_, k) => values[k]) : texts[key];
  };
  // The live concept rulebook is computed by the engine the other pages chose.
  async function api(url) {
    const engineId = localStorage.getItem('erb.backend');
    const res = await fetch(url, { headers: engineId ? { 'X-ERB-Backend': engineId } : {} });
    if (!res.ok) throw new Error(((await res.json().catch(() => ({}))).error) || `${url} answered ${res.status}`);
    return res;
  }

  // JSON text as coloured HTML, one token at a time: a string followed by a colon is a key.
  const TOKEN = /("(?:[^"\\]|\\.)*")(\s*:)?|(-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)|\b(true|false)\b|\b(null)\b|([{}[\],])/g;
  function colour(line) {
    let out = '', at = 0;
    for (const m of line.matchAll(TOKEN)) {
      out += esc(line.slice(at, m.index));
      if (m[1] !== undefined) out += m[2] !== undefined ? `<span class="j-key">${esc(m[1])}</span>${esc(m[2])}` : `<span class="j-str">${esc(m[1])}</span>`;
      else if (m[3] !== undefined) out += `<span class="j-num">${m[3]}</span>`;
      else if (m[4] !== undefined) out += `<span class="j-bool">${m[4]}</span>`;
      else if (m[5] !== undefined) out += `<span class="j-null">null</span>`;
      else out += `<span class="j-punct">${m[6]}</span>`;
      at = m.index + m[0].length;
    }
    return out + esc(line.slice(at));
  }

  function show(text, src) {
    const lines = text.replace(/\n$/, '').split('\n');
    $('code').innerHTML = lines.map((line, i) =>
      `<div class="j-line" id="L${i + 1}"><a class="j-no" href="#L${i + 1}">${i + 1}</a><code>${colour(line)}</code></div>`).join('');
    // A table starts on a line `  "Name": {` (two spaces in: a key of the rulebook itself).
    const tables = [];
    lines.forEach((line, i) => { const m = /^ {2}"([^"]+)": \{$/.exec(line); if (m) tables.push({ name: m[1], line: i + 1 }); });
    $('jump').innerHTML = `<option value="">${esc(T('jsonfile-jump-choose', { count: tables.length }))}</option>` +
      tables.map((t) => `<option value="${t.line}">${esc(t.name)}</option>`).join('');
    $('status').textContent = T('jsonfile-size', { lines: lines.length.toLocaleString(), kb: Math.round(text.length / 1024).toLocaleString(), file: src });
    if (location.hash) document.getElementById(location.hash.slice(1))?.scrollIntoView({ block: 'center' });
  }

  $('jump').addEventListener('change', (e) => {
    if (!e.target.value) return;
    location.hash = `L${e.target.value}`;
    document.getElementById(`L${e.target.value}`).scrollIntoView({ block: 'start' });
  });
  $('wrap').addEventListener('change', (e) => $('code').classList.toggle('nowrap', !e.target.checked));

  (async () => {
    const rows = await (await api('/api/rows')).json();
    texts = Object.fromEntries(rows.PageTexts.map((x) => [x.PageTextId, x.Text]));
    for (const el of document.querySelectorAll('[data-text]')) el.textContent = T(el.dataset.text);
    const f = FORMS[form];
    if (!f) throw new Error(`There is no rulebook file for the form ${JSON.stringify(form)}`);
    if (live && !f.live) throw new Error(`The ${form} form has no live file`);
    const src = live ? f.live : f.file;
    document.body.classList.toggle('concepts', form === 'concept');
    document.title = $('heading').textContent = T(`jsonfile-heading-${form}${live ? '-live' : ''}`);
    $('about').textContent = T(live ? 'jsonfile-about-live' : 'jsonfile-about');
    $('back').href = f.back;
    $('back').textContent = T(f.backText);
    $('raw').href = src;
    const state = await (await api('/api/admin/changes')).json();
    showVersion($('version'), T, state.version, state.changes.total);
    nameKinds(T, texts);
    show(await (await api(src)).text(), live ? src : src.replace(/^\/(rulebook|concept-rulebook)\//, (_, d) => (d === 'rulebook' ? 'effortless-rulebook/' : 'concept-rulebook/')));
  })().catch((e) => { console.error(e); $('status').textContent = e.message; $('status').classList.add('bad'); });
})();
