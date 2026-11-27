// The South Park explainer page. It draws what the server reads from the rulebook's
// views and sends each click back as a write to the rulebook's base tables. It
// decides nothing about facts, rules or conclusions: no reasoner runs here.
(function () {
  const svg = document.getElementById('graph');
  const $ = (id) => document.getElementById(id);
  let P = null;                 // the page as the chosen engine computes it
  let engines = [];             // the execution engines the rulebook lists, with whether each is available
  let editor = { running: false, url: '' };
  // 'original' shows what the original explainer shows; 'tools' adds what the rulebook tools bring.
  let mode = localStorage.getItem('southpark.mode') === 'original' ? 'original' : 'tools';
  let engineId = localStorage.getItem('erb.backend');
  let pos = {};                 // node key -> [x, y]; layout and dragging are the page's own
  let slotIndex = 0, seen = {}, selected = null, selectedEdge = null, drag = null, explainerReady = false;

  const esc = (t) => String(t).replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  const T = (key, values) => {
    const s = P.texts[key];
    if (s == null) throw new Error(`The rulebook has no page text "${key}"`);
    return values ? s.replace(/\{(\w+)\}/g, (_, k) => values[k]) : s;
  };
  // Wraps a derived value for the explainer: hover it for its rule and the values it came from.
  const dag = (spec, row, inner, tag = 'span', cls = '') => {
    if (mode === 'original') return `<${tag}${cls ? ` class="${cls}"` : ''}>${inner}</${tag}>`;
    const classes = [cls, tag === 'span' ? '' : 'dag-cell-block'].filter(Boolean).join(' ');
    return `<${tag} ${classes ? `class="${classes}" ` : ''}data-er-dag="${spec}" data-er-row="${esc(row)}">${inner}</${tag}>`;
  };

  async function api(method, url, body) {
    const headers = { 'content-type': 'application/json' };
    if (engineId) headers['X-ERB-Backend'] = engineId;
    const res = await fetch(url, { method, headers, body: body ? JSON.stringify(body) : undefined });
    const json = await res.json();
    if (!res.ok) throw new Error(json.error || `${method} ${url} answered ${res.status}`);
    return json;
  }
  async function act(method, url, body, after) {
    document.body.classList.add('busy');
    try {
      const res = await api(method, url, body);
      if (res.page) P = res.page;
      if (after) after(res);
      render();
    } catch (e) {
      console.error(e);
      $('form-message').textContent = e.message;
    } finally {
      document.body.classList.remove('busy');
    }
  }

  // ---------------------------------------------------------------- layout
  function applyLayout(name) {
    const layout = P.layouts[name];
    if (!layout) throw new Error(`The rulebook has no layout "${name}"`);
    pos = {};
    for (const [k, xy] of Object.entries(layout)) pos[k] = xy.slice();
    slotIndex = 0;
  }
  function place(key) {
    if (pos[key]) return;
    const s = P.slots[slotIndex % P.slots.length], lap = Math.floor(slotIndex / P.slots.length);
    pos[key] = [Math.min(350, s[0] + lap * 14), Math.min(435, s[1] + lap * 18)];
    slotIndex++;
  }

  // ---------------------------------------------------------------- graph
  const ICONS = {
    person: '<circle class="icon" cx="0" cy="-6" r="5.5"/><path class="icon" d="M-11 12a11 10 0 0 1 22 0z"/>',
    place: '<path class="icon" d="M-13 -2L0 -12L13 -2z"/><path class="icon" fill-rule="evenodd" d="M-10 0h20v12h-20zM-2.5 5v7h5v-7z"/>',
    class: '<circle class="icon" cx="-6" cy="-5" r="4.5"/><rect class="icon" x="2" y="-9.5" width="9" height="9"/><path class="icon" d="M-5 11L0.5 1.5L6 11z"/>',
  };
  function nodes() {
    const things = Object.fromEntries(P.things.map((t) => [t.node_key, t]));
    const info = {};
    for (const e of P.edges) {
      if (!info[e.subject_node_key]) {
        const t = things[e.subject_node_key];
        info[e.subject_node_key] = { name: e.subject_label, kind: t.node_kind, thing: t };
      }
      if (!info[e.object_node_key]) {
        info[e.object_node_key] = e.is_literal
          ? { name: e.object_text, kind: 'value', datatype: e.property_datatype, valueLabel: e.object_value_node_label, roleValue: e.object_value_node_label === 'role value' }
          : { name: e.object_text, kind: things[e.object_node_key].node_kind, thing: things[e.object_node_key] };
      }
    }
    for (const [key, d] of Object.entries(info)) {
      place(key);
      d.picture = d.thing && d.thing.portrait_image ? d.thing : null;
      d.r = d.picture ? 31 : 23;
    }
    return info;
  }
  function edgePoint(c, r, toward) {
    const dx = toward[0] - c[0], dy = toward[1] - c[1];
    if (!dx && !dy) return c.slice();
    const t = 1 / Math.sqrt((dx / r) ** 2 + (dy / r) ** 2);
    return [c[0] + dx * t, c[1] + dy * t];
  }
  function drawGraph() {
    const info = nodes();
    const near = {};
    if (selected && info[selected]) {
      near[selected] = true;
      for (const e of P.edges) {
        if (e.subject_node_key === selected) near[e.object_node_key] = true;
        if (e.object_node_key === selected) near[e.subject_node_key] = true;
      }
    } else selected = null;
    const groups = {};
    for (const e of P.edges) {
      const a = e.subject_node_key, b = e.object_node_key, k = JSON.stringify(a < b ? [a, b] : [b, a]);
      (groups[k] ||= []).push(e);
    }
    let out = '<defs>' +
      '<marker id="ah" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" fill="var(--ink)"/></marker>' +
      '<marker id="ahi" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="6" markerHeight="6" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" fill="var(--flare)"/></marker>' +
      '<marker id="ahb" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="5" markerHeight="5" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" fill="var(--alarm)"/></marker>' +
      Object.entries(info).map(([k, d], i) => {
        d.clip = `clip-${i}`;
        return d.picture ? `<clipPath id="${d.clip}" clipPathUnits="userSpaceOnUse"><circle cx="${pos[k][0]}" cy="${pos[k][1]}" r="${d.r - 1}"/></clipPath>` : '';
      }).join('') + '</defs>';

    for (const [pk, g] of Object.entries(groups)) {
      const [ka, kb] = JSON.parse(pk), A = pos[ka], B = pos[kb];
      const vx = B[0] - A[0], vy = B[1] - A[1], len = Math.hypot(vx, vy) || 1, px = -vy / len, py = vx / len;
      g.forEach((e, i) => {
        const off = (i - (g.length - 1) / 2) * 30;
        const a = pos[e.subject_node_key], b = pos[e.object_node_key];
        const c = [(a[0] + b[0]) / 2 + px * off * 2, (a[1] + b[1]) / 2 + py * off * 2];
        const p1 = edgePoint(a, info[e.subject_node_key].r, c), p2 = edgePoint(b, info[e.object_node_key].r, c);
        const mx = (p1[0] + 2 * c[0] + p2[0]) / 4, my = (p1[1] + 2 * c[1] + p2[1]) / 4;
        let cls = 'edge' + (e.is_conclusion ? ' inf' : '') + (e.is_in_contradiction ? ' bad' : '');
        if (selected && e.subject_node_key !== selected && e.object_node_key !== selected) cls += ' dim';
        if (selectedEdge === e.statement_id) cls += ' sel';
        if (!seen['e' + e.statement_id]) { cls += ' fresh'; seen['e' + e.statement_id] = true; }
        const mark = e.is_in_contradiction ? 'ahb' : (e.is_conclusion ? 'ahi' : 'ah');
        const d = `M${p1[0].toFixed(1)} ${p1[1].toFixed(1)}Q${c[0].toFixed(1)} ${c[1].toFixed(1)} ${p2[0].toFixed(1)} ${p2[1].toFixed(1)}`;
        out += `<g class="${cls}" data-edge="${esc(e.statement_id)}"><path d="${d}" marker-end="url(#${mark})"/><path class="hit" d="${d}"/>` +
          `<text x="${mx.toFixed(1)}" y="${(my + 3.5).toFixed(1)}" text-anchor="middle">${esc(e.property_label)}</text></g>`;
      });
    }

    for (const [k, d] of Object.entries(info)) {
      const p = pos[k];
      let cls = `node ${d.kind}`;
      if (selected === k) cls += ' sel';
      if (selected && !near[k]) cls += ' dim';
      if (!seen['n' + k]) { cls += ' fresh'; seen['n' + k] = true; }
      const iconKind = d.roleValue || (d.kind === 'class' && d.name === 'Person') ? 'person' : d.kind;
      const icon = `<g class="picture-fallback"${d.picture ? ' style="display:none"' : ''} transform="translate(${p[0]} ${p[1]})">${ICONS[iconKind] || ''}</g>`;
      const labelY = p[1] + d.r + 15;
      let label = '';
      if (d.kind !== 'value' || d.roleValue) {
        label = `<text x="${p[0]}" y="${labelY}" text-anchor="middle">` +
          (d.name.split(' ').length > 2 ? twoLines(d.name, p[0]) : esc(d.name)) + '</text>';
      }
      const sub = d.kind === 'value' ? d.valueLabel : (d.kind === 'class' ? T('sublabel-class') : '');
      if (sub) label += `<text class="datatype" x="${p[0]}" y="${labelY + ((d.roleValue || d.kind === 'class') ? 13 : 0)}" text-anchor="middle">${esc(sub)}</text>`;
      let inner;
      if (d.picture) {
        cls += ' has-picture';
        const t = d.picture;
        inner = `<circle class="picture-backdrop" cx="${p[0]}" cy="${p[1]}" r="${d.r}"/>${icon}` +
          `<g class="node-picture" clip-path="url(#${d.clip})" aria-hidden="true"><svg x="${p[0] - d.r}" y="${p[1] - d.r}" width="${d.r * 2}" height="${d.r * 2}" ` +
          `viewBox="${esc(t.portrait_crop)}" preserveAspectRatio="xMidYMid meet"><image href="${esc(t.portrait_image)}" x="0" y="0" width="${t.portrait_width}" height="${t.portrait_height}"/></svg></g>` +
          `<circle class="shape" cx="${p[0]}" cy="${p[1]}" r="${d.r}"/>`;
      } else {
        inner = `<circle class="shape" cx="${p[0]}" cy="${p[1]}" r="${d.r}"/>` +
          (d.kind === 'value' && !d.roleValue ? `<text class="num" x="${p[0]}" y="${p[1] + 6}" text-anchor="middle">${esc(d.name)}</text>` : icon);
      }
      const title = d.name + (d.kind === 'class' ? ` — ${T('title-class')}` : d.kind === 'value'
        ? ` — ${d.roleValue ? d.valueLabel : T('title-literal')}${d.datatype ? ', ' + d.datatype : ''}` : '');
      out += `<g class="${cls}" data-node="${esc(k)}" tabindex="0" role="button" aria-pressed="${selected === k}" aria-label="${esc(d.name)}">` +
        `<title>${esc(title)}</title>${inner}${label}</g>`;
    }
    svg.innerHTML = out;
    return info;
  }
  function twoLines(name, x) {
    const words = name.split(' '), cut = Math.ceil(words.length / 2);
    return `<tspan x="${x}">${esc(words.slice(0, cut).join(' '))}</tspan><tspan x="${x}" dy="14">${esc(words.slice(cut).join(' '))}</tspan>`;
  }

  // ---------------------------------------------------------------- panels
  const tripleHtml = (s, p, o, datatype) => `<span class="triple"><b>${esc(s)}</b><i>${esc(p)}</i><b>${esc(o)}</b>` +
    (datatype ? `<span class="literal-type">${esc(datatype)}</span>` : '') + '</span>';
  const cardFor = (statementId) => P.conclusions.find((c) => c.target === statementId);
  function whyHtml(e) {
    if (!e.is_conclusion) return `<span class="why">${dag('Statements.Source', e.statement_id, esc(e.source))}</span>`;
    const c = cardFor(e.statement_id);
    return c ? `<span class="why">${dag('Firings.CardText', c.firing_id, esc(c.card_text))}</span>` : '';
  }

  function renderPanels(info) {
    for (const el of document.querySelectorAll('[data-text]')) el.textContent = T(el.dataset.text);
    window.RulebookAdmin.nameKinds(T, P.texts);
    document.title = T('page-title');
    $('byline-author').href = T('byline-author-url');
    $('link-article').href = T('link-article-url');
    $('link-original').href = T('link-original-url');

    $('facts').innerHTML = P.facts.map((f) =>
      `<li><label class="row"><input type="checkbox" data-fact="${esc(f.statement_id)}"${f.is_in_effect ? ' checked' : ''}>` +
      dag('Statements.IsInEffect', f.statement_id, tripleHtml(f.subject_label, f.property_label, f.object_text, f.is_literal ? f.property_datatype : '')) +
      '</label>' + (f.is_built_in ? '' : `<button type="button" class="x" data-remove="${esc(f.statement_id)}" aria-label="${esc(T('remove-label'))}">${esc(T('remove'))}</button>`) +
      '</li>').join('');
    $('rules').innerHTML = P.rules.map((r) =>
      `<li><label class="row"><input type="checkbox" data-rule="${esc(r.rule_id)}"${r.is_enabled ? ' checked' : ''}>` +
      `<span><span class="rule-name">${esc(r.label)}</span><span class="rule-desc">${esc(r.explanation)}</span></span></label></li>`).join('');
    $('inferred').innerHTML = P.conclusions.length
      ? '<ul class="rows inferred">' + P.conclusions.map((c) => '<li>' + tripleHtml(c.target_subject_label, c.target_property_label, c.target_object_text) +
          `<span class="why">${dag('Firings.CardText', c.firing_id, esc(c.card_text))}</span>` + '</li>').join('') + '</ul>'
      : dag('Graphs.ConclusionsMessage', P.graph.graph_id, esc(P.graph.conclusions_message), 'p', 'empty');
    $('alarm').innerHTML = P.alarms.map((a) => dag('DisjointChecks.AlarmText', a.disjoint_check_id, esc(a.alarm_text), 'div', 'alarm')).join('');
    for (const [group, el] of [['stan', $('buttons-stan')], ['facts', $('buttons-facts')]]) {
      el.innerHTML = P.scenarios.filter((s) => s.button_group === group).map((s) =>
        `<button type="button" data-scenario="${esc(s.scenario_id)}"${s.is_primary ? ' class="primary"' : ''}${s.is_available ? '' : ' disabled'}>${esc(s.label)}</button>`).join('');
    }
    $('scenario-message').textContent = P.record;
    const pred = $('pred'), keep = pred.value;
    pred.innerHTML = P.properties.map((p) => `<option value="${esc(p.property_id)}">${esc(p.label)}</option>`).join('');
    if (keep) pred.value = keep;
    $('names').innerHTML = P.names.map((name) => `<option value="${esc(name)}">`).join('');
    $('image-credits').innerHTML = P.things.filter((t) => t.portrait_page).map((t) =>
      `<li><a href="${esc(t.portrait_page)}" target="_blank" rel="noopener noreferrer">${esc(t.portrait_name)}</a> ` +
      `<a class="credit-file" href="${esc(t.portrait_image)}" target="_blank" rel="noopener noreferrer">(image)</a></li>`).join('');
    $('hood-counts').innerHTML = [
      ['Graphs.CountOfStatedFacts', P.graph.count_of_stated_facts, T('hood-stated')],
      ['Graphs.CountOfConclusions', P.graph.count_of_conclusions, T('hood-conclusions')],
      ['Graphs.IsConsistent', P.graph.is_consistent ? T('hood-yes') : T('hood-no'), T('hood-consistent')],
    ].map(([spec, v, label]) => `<div class="stat">${dag(spec, P.graph.graph_id, esc(v), 'b')}<span>${esc(label)}</span></div>`).join('');
    renderFocus(info);
  }

  function renderFocus(info) {
    const focus = $('focus');
    let mine = null, head = '';
    if (selectedEdge) {
      mine = P.edges.filter((e) => e.statement_id === selectedEdge);
      if (mine.length) head = `<strong>${esc(T('focus-source'))}</strong>`; else { selectedEdge = null; mine = null; }
    }
    if (!mine && selected && info[selected]) {
      const d = info[selected];
      mine = P.edges.filter((e) => e.subject_node_key === selected || e.object_node_key === selected);
      head = mine.length === 1 ? T('focus-appears-one', { name: `<strong>${esc(d.name)}</strong>` })
                               : T('focus-appears', { name: `<strong>${esc(d.name)}</strong>`, count: mine.length });
      if (mode === 'tools' && d.thing && d.thing.kind === 'individual') {
        head += `<p class="counts effortless-note">${dag('Things.CountOfChildren', d.thing.thing_id, esc(d.thing.count_of_children))} ${esc(T('focus-children'))} · ` +
          `${dag('Things.CountOfGrandchildren', d.thing.thing_id, esc(d.thing.count_of_grandchildren))} ${esc(T('focus-grandchildren'))}</p>`;
      }
      if (d.picture) {
        head += `<div class="image-note"><a href="${esc(d.picture.portrait_page)}" target="_blank" rel="noopener noreferrer">${esc(T('fandom-link', { name: d.picture.portrait_name }))}</a>` +
          ` · <a href="${esc(d.picture.portrait_image)}" target="_blank" rel="noopener noreferrer">${esc(T('original-image'))}</a></div>`;
      }
    }
    focus.innerHTML = mine ? head + '<ul class="rows inferred" style="margin-top:6px">' + mine.map((e) =>
      '<li>' + tripleHtml(e.subject_label, e.property_label, e.object_text, e.is_literal && !e.is_conclusion ? e.property_datatype : '') + whyHtml(e) + '</li>').join('') + '</ul>' : '';
  }

  async function refreshWitnessRows() {
    if (!window.EffortlessExplainer) return;
    try { window.EffortlessExplainer.setRows(await api('GET', '/api/rows')); } catch (e) { console.error(e); }
  }
  // The tools layer: which engine computes the page, how the engines compare, and what each tool adds.
  function renderTools() {
    document.body.dataset.mode = mode;
    for (const b of document.querySelectorAll('[data-mode-choice]')) b.setAttribute('aria-pressed', String(b.dataset.modeChoice === mode));
    const current = engines.find((e) => e.id === engineId);
    $('engines').innerHTML = engines.map((e) =>
      `<button type="button" data-engine="${esc(e.id)}" aria-pressed="${e.id === engineId}"${e.ok ? '' : ' disabled'} title="${esc(e.ok ? e.description : T('engine-unavailable', { engine: e.label, reason: e.reason }))}">${esc(e.label)}</button>`).join('');
    $('engine-description').textContent = current ? current.description : '';
    $('engine-timing').textContent = T('engine-timing', { ms: P.pageMs });
    const unsaved = $('unsaved');
    unsaved.textContent = P.unsaved === 1 ? T('unsaved-one') : P.unsaved ? T('unsaved', { count: P.unsaved }) : T('unsaved-none');
    unsaved.classList.toggle('dirty', P.unsaved > 0);
    window.RulebookAdmin.showVersion($('version'), T, P.version, P.unsaved);
    $('tools').innerHTML = P.tools.map((t) => {
      let link = '';
      if (t.href === 'editor') {
        link = editor.running ? `<a href="${esc(editor.url)}" target="_blank" rel="noopener" data-kind="newtab">${esc(t.link_text)}</a>` : `<span class="off">${esc(T('admin-editor-off'))}</span>`;
      } else if (t.href) {
        // A page of this app opens here; anything else (a generated page, a file) opens beside it.
        link = /^\/(admin|concepts)\./.test(t.href) ? `<a href="${esc(t.href)}" data-kind="page">${esc(t.link_text)}</a>`
                                                    : `<a href="${esc(t.href)}" target="_blank" rel="noopener" data-kind="newtab">${esc(t.link_text)}</a>`;
      }
      return `<li data-tool="${esc(t.rulebook_tool_id)}"><b>${esc(t.label)}</b><span>${esc(t.adds)}</span>${link}</li>`;
    }).join('');
    window.RulebookAdmin.nameKinds(T, P.texts, $('tools'));
  }

  function render(listsToo = true) {
    const info = drawGraph();
    if (!listsToo) return;
    renderPanels(info);
    renderTools();
    // The explainer (hover a value for its rule and witness) is one of the tools: it stays out of the original view.
    if (mode === 'tools' && window.EffortlessExplainer) {
      if (!explainerReady) {
        window.EffortlessExplainer.init({ mode: 'modal', corner: 'bottom-right', xlsx: T('workbook-url') });
        explainerReady = true;
      } else window.EffortlessExplainer.enhanceCells(document);
      refreshWitnessRows();
    }
  }

  async function compareEngines() {
    const box = $('conformance');
    box.className = 'conformance';
    box.textContent = T('conformance-running');
    try {
      const r = await api('POST', '/api/conformance');
      const agree = r.differingCount === 0 && r.missingRowCount === 0;
      box.className = 'conformance' + (agree ? '' : ' differ');
      const timings = r.engines.map((e) => `${e.label} ${e.ms} ms`).join(' \u00b7 ');
      box.innerHTML = esc(agree ? T('conformance-agree', { engines: r.engines.length, cells: r.cells })
                                 : T('conformance-differ', { count: r.differingCount + r.missingRowCount, cells: r.cells })) +
        `<div class="muted">${esc(timings)}</div>` +
        (r.skipped.length ? `<div class="muted">${r.skipped.map((s) => esc(T('engine-unavailable', { engine: s.label, reason: s.reason }))).join('<br>')}</div>` : '') +
        (agree ? '' : '<ul>' + r.differing.slice(0, 12).map((d) =>
          `<li>${esc(d.table)}.${esc(d.field)} of ${esc(d.key)}: ${Object.entries(d.values).map(([k, v]) => `${esc(k)} = ${esc(JSON.stringify(v))}`).join(', ')}</li>`).join('') + '</ul>');
    } catch (e) {
      box.className = 'conformance differ';
      box.textContent = e.message;
    }
  }

  // ---------------------------------------------------------------- events
  document.addEventListener('change', (e) => {
    const t = e.target;
    if (t.dataset.fact !== undefined) {
      $('scenario-note').textContent = '';
      act('POST', `/api/facts/${encodeURIComponent(t.dataset.fact)}/switch`, { on: t.checked });
    }
    if (t.dataset.rule !== undefined) act('POST', `/api/rules/${encodeURIComponent(t.dataset.rule)}/switch`, { on: t.checked });
  });
  document.addEventListener('click', (e) => {
    const t = e.target.closest('[data-remove],[data-scenario]');
    if (!t) return;
    if (t.dataset.remove !== undefined) {
      $('scenario-note').textContent = '';
      act('POST', `/api/facts/${encodeURIComponent(t.dataset.remove)}/remove`);
    } else {
      act('POST', `/api/scenarios/${encodeURIComponent(t.dataset.scenario)}/run`, null, (res) => {
        $('scenario-note').textContent = res.note;
        if (res.layout) applyLayout(res.layout);
        if (res.clearsSelection) { selected = null; selectedEdge = null; seen = {}; }
        if (res.clearsFormMessage) $('form-message').textContent = '';
      });
    }
  });
  $('add').addEventListener('submit', (e) => {
    e.preventDefault();
    $('scenario-note').textContent = '';
    act('POST', '/api/facts', { subject: $('subj').value, property: $('pred').value, object: $('obj').value }, (res) => {
      if (res.message !== null) $('form-message').textContent = res.message;
      if (res.added) { $('subj').value = ''; $('obj').value = ''; }
    });
  });

  function svgPoint(e) {
    const m = svg.getScreenCTM();
    return m ? [(e.clientX - m.e) / m.a, (e.clientY - m.f) / m.d] : [0, 0];
  }
  svg.addEventListener('pointerdown', (e) => {
    const g = e.target.closest('[data-node]');
    if (!g) {
      const eg = e.target.closest('[data-edge]');
      if (eg) { const k = eg.getAttribute('data-edge'); selectedEdge = selectedEdge === k ? null : k; selected = null; render(); }
      return;
    }
    const name = g.getAttribute('data-node'), pt = svgPoint(e);
    drag = { name, r: +g.querySelector('circle.shape').getAttribute('r'), dx: pos[name][0] - pt[0], dy: pos[name][1] - pt[1], sx: e.clientX, sy: e.clientY, moved: false };
    try { svg.setPointerCapture(e.pointerId); } catch (err) { /* capture is optional */ }
    e.preventDefault();
  });
  svg.addEventListener('pointermove', (e) => {
    if (!drag) return;
    if (!drag.moved && Math.abs(e.clientX - drag.sx) + Math.abs(e.clientY - drag.sy) < 5) return;
    drag.moved = true;
    const pt = svgPoint(e), r = drag.r, allowance = 33;
    pos[drag.name] = [Math.max(r + 4, Math.min(380 - r - 4, Math.round(pt[0] + drag.dx))), Math.max(r + 4, Math.min(480 - r - allowance, Math.round(pt[1] + drag.dy)))];
    render(false);
  });
  svg.addEventListener('pointerup', () => {
    if (!drag) return;
    if (!drag.moved) { selected = selected === drag.name ? null : drag.name; selectedEdge = null; }
    drag = null;
    render();
  });
  svg.addEventListener('pointercancel', () => { drag = null; });
  svg.addEventListener('keydown', (e) => {
    const g = e.target.closest('[data-node]');
    if (g && (e.key === 'Enter' || e.key === ' ')) { e.preventDefault(); const n = g.getAttribute('data-node'); selected = selected === n ? null : n; selectedEdge = null; render(); }
  });
  svg.addEventListener('error', (e) => {  // a portrait that fails to load falls back to the icon
    if (!e.target.matches || !e.target.matches('image')) return;
    const node = e.target.closest('[data-node]');
    if (!node) return;
    node.querySelector('.node-picture').style.display = 'none';
    node.querySelector('.picture-fallback').style.display = '';
  }, true);

  document.addEventListener('click', (e) => {
    const m = e.target.closest('[data-mode-choice]');
    if (m) { mode = m.dataset.modeChoice; localStorage.setItem('southpark.mode', mode); render(); return; }
    const b = e.target.closest('[data-engine]');
    if (b && b.dataset.engine !== engineId) {
      engineId = b.dataset.engine;
      localStorage.setItem('erb.backend', engineId);
      $('conformance').textContent = ''; $('conformance').className = '';
      act('GET', '/api/page', null, (page) => { P = page; });   // the same rows, computed by the other engine
    }
  });
  $('compare').addEventListener('click', compareEngines);

  (async () => {
    const listed = await api('GET', '/api/engines');
    engines = listed.engines;
    const mine = engines.find((e) => e.id === engineId && e.ok) || engines.find((e) => e.id === listed.defaultId && e.ok) || engines.find((e) => e.ok);
    if (!mine) throw new Error(engines.map((e) => `${e.label}: ${e.reason}`).join(' | '));
    engineId = mine.id;
    editor = await api('GET', '/api/admin/editor');
    P = await api('GET', '/api/page');
    applyLayout('default');
    render();
  })().catch((e) => { console.error(e); document.body.insertAdjacentHTML('afterbegin', `<p class="alarm">${esc(e.message)}</p>`); });
})();
