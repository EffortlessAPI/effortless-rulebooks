// Star Trek explorer, client. Shows what /api returns (the vw_* views) and
// computes nothing: every count, average and verdict is a rulebook rule. Derived
// values on the detail pages are wrapped in data-er-dag so the project's explainer
// can show the rule (not inside links: the explainer adds a link of its own).
(function () {
  const $page = () => document.getElementById('page');
  const $crumbs = () => document.getElementById('breadcrumb');

  const esc = (v) => String(v ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  const date = (v) => (v ? new Date(v).toLocaleDateString(undefined, { timeZone: 'UTC' }) : '—');
  const year = (v) => (v ? new Date(v).getUTCFullYear() : '');
  const money = (v) => (v == null || v === '' ? '—' : `$${(Number(v) / 1e6).toFixed(1)}M`);
  const rating = (v) => (v == null ? '—' : Number(v).toFixed(2).replace(/\.?0+$/, ''));
  const stars = (n) => '★'.repeat(Number(n) || 0) + '☆'.repeat(Math.max(0, 5 - (Number(n) || 0)));
  const pad2 = (n) => String(n).padStart(2, '0');

  // A derived value the explainer can explain: Table.Field on the row with key `row`.
  const dag = (field, row, html) => `<span data-er-dag="${field}" data-er-row="${esc(row)}">${html}</span>`;

  async function api(route) {
    const res = await fetch(`/api${route}`);
    const body = await res.json();
    if (!res.ok) throw new Error(body.error || `${route}: HTTP ${res.status}`);
    return body;
  }

  function crumbs(parts) {
    const all = [['/', 'Home'], ...parts];
    $crumbs().innerHTML = all
      .map(([href, label], i) => (i === all.length - 1 && parts.length ? esc(label) : `<a href="${href}" data-link>${esc(label)}</a>`))
      .join('<span class="sep">/</span>');
  }

  const stat = (label, html) => `<div class="stat"><div class="label">${label}</div><div class="value">${html}</div></div>`;
  const field = (label, html) => `<div class="field"><div class="label">${label}</div><div class="value">${html}</div></div>`;

  function ratingsTable(ratings) {
    if (!ratings.length) return '<p class="muted">No fan ratings yet.</p>';
    return `<table class="ratings">${ratings.map((r) => `
      <tr><td class="stars">${stars(r.rating)}</td><td>${esc(r.users_name)}</td><td class="muted">${date(r.created_at)}</td></tr>`).join('')}</table>`;
  }

  // ------------------------------------------------------------------ pages
  async function home() {
    crumbs([]);
    const [series, movies] = await Promise.all([api('/series'), api('/movies')]);
    return `<div class="content">
      <section class="panel"><h2>Series</h2>${series.map((s) => `
        <a class="item" href="/show/${esc(s.show_code.toLowerCase())}" data-link>
          <div class="item-row">
            <span class="item-name">${esc(s.show_code)}
              ${s.is_good ? '<span class="badge good">good</span>' : ''}
              ${s.is_long_running ? '<span class="badge long">long-running</span>' : ''}</span>
            <span>${Number(s.rating_count) ? `★ ${rating(s.rating)}` : '<span class="muted">unrated</span>'}</span>
          </div>
          <div class="item-subtitle">${esc(s.title)} · ${esc(s.total_episodes)} episodes</div>
        </a>`).join('')}</section>
      <section class="panel"><h2>Movies</h2>${movies.map((m) => `
        <a class="item" href="/movie/${esc(m.movie_id)}" data-link>
          <div class="item-name">${esc(m.title)}</div>
          <div class="item-subtitle">${year(m.release_date)} · ${esc(m.crew_count)} cast &amp; crew</div>
        </a>`).join('')}</section>
    </div>`;
  }

  async function show({ show: code }) {
    const s = await api(`/shows/${encodeURIComponent(code)}`);
    const seasons = await api(`/series/${encodeURIComponent(s.serie_id)}/seasons`);
    crumbs([[`/show/${code}`, s.title]]);
    return `<section class="panel">
      <h2>${esc(s.title)}</h2>
      <div class="stats">
        ${stat('Seasons', dag('Series.TotalSeasons', s.serie_id, esc(s.total_seasons)))}
        ${stat('Episodes', dag('Series.TotalEpisodes', s.serie_id, esc(s.total_episodes)))}
        ${stat('Long-running', dag('Series.IsLongRunning', s.serie_id, s.is_long_running ? 'Yes' : 'No'))}
        ${stat('Fan ratings', dag('Series.RatingCount', s.serie_id, esc(s.rating_count)))}
        ${stat('Average rating', dag('Series.Rating', s.serie_id, Number(s.rating_count) ? `★ ${rating(s.rating)}` : '—'))}
        ${stat('Good?', dag('Series.IsGood', s.serie_id, s.is_good ? 'Yes' : 'No'))}
      </div>
      <div class="details">
        ${field('Network', esc(s.network) || '—')}
        ${field('On the air', `${date(s.premiere_date)} – ${s.finale_date ? date(s.finale_date) : 'present'}`)}
        ${field('About', esc(s.description) || '—')}
      </div>
      <h3>Fan ratings</h3>${ratingsTable(s.ratings)}
    </section>
    <section class="panel"><h2>Seasons</h2>
      ${seasons.length ? `<div class="grid">${seasons.map((se) => `
        <a class="card" href="/show/${esc(code)}/season-${se.season_number}" data-link>
          <div class="num">Season ${se.season_number}</div>
          <div class="title">${esc(se.title)}</div>
          <div class="meta">${esc(se.episode_count)} episodes · ${year(se.start_airdate)}</div>
        </a>`).join('')}</div>` : '<p class="muted">No seasons.</p>'}
    </section>`;
  }

  async function seasonOf(code, number) {
    const s = await api(`/shows/${encodeURIComponent(code)}`);
    const seasons = await api(`/series/${encodeURIComponent(s.serie_id)}/seasons`);
    const se = seasons.find((x) => x.season_number === number);
    if (!se) throw new Error(`${s.title} has no season ${number}`);
    return { s, se };
  }

  async function season({ show: code, season: number }) {
    const { s, se } = await seasonOf(code, number);
    const episodes = await api(`/seasons/${encodeURIComponent(se.season_id)}/episodes`);
    crumbs([[`/show/${code}`, s.title], [`/show/${code}/season-${number}`, `Season ${number}`]]);
    return `<section class="panel">
      <h2>${dag('Seasons.ShowTitle', se.season_id, esc(se.show_title))} — Season ${number}</h2>
      <div class="stats">
        ${stat('Episodes', dag('Seasons.EpisodeCount', se.season_id, esc(se.episode_count)))}
        ${stat('Network', dag('Seasons.ShowNetwork', se.season_id, esc(se.show_network)))}
        ${stat('Aired', `${date(se.start_airdate)} – ${date(se.end_airdate)}`)}
      </div>
      <div class="details">${field('About', esc(se.description) || '—')}</div>
    </section>
    <section class="panel"><h2>Episodes</h2><div class="grid">${episodes.map((e) => `
      <a class="card" href="/show/${esc(code)}/season-${number}/episode-${e.episode_number}" data-link>
        <div class="num">Ep ${e.episode_number}</div>
        <div class="title">${esc(e.one_sentence_summary || e.description || 'Untitled')}</div>
        <div class="meta">${date(e.airdate)}</div>
      </a>`).join('')}</div></section>`;
  }

  async function episode({ show: code, season: number, episode: epNumber }) {
    const { s, se } = await seasonOf(code, number);
    const episodes = await api(`/seasons/${encodeURIComponent(se.season_id)}/episodes`);
    const found = episodes.find((x) => x.episode_number === epNumber);
    if (!found) throw new Error(`Season ${number} has no episode ${epNumber}`);
    const e = await api(`/episodes/${encodeURIComponent(found.episode_id)}`);
    crumbs([[`/show/${code}`, s.title], [`/show/${code}/season-${number}`, `Season ${number}`],
      [`/show/${code}/season-${number}/episode-${epNumber}`, `Episode ${epNumber}`]]);
    return `<section class="panel">
      <h2>${esc(s.title)} — S${dag('Episodes.SeasonNumber', e.episode_id, pad2(e.season_number))}E${pad2(e.episode_number)}</h2>
      <div class="stats">
        ${stat('Season', dag('Episodes.SeasonTitle', e.episode_id, esc(e.season_title)))}
        ${stat('Aired', date(e.airdate))}
        ${stat('Runtime', e.runtime_minutes ? `${esc(e.runtime_minutes)} min` : '—')}
      </div>
      <div class="details">
        ${field('Plot', esc(e.description) || '—')}
        ${field('Written by', esc(e.writers) || '—')}
        ${field('Directed by', esc(e.director) || '—')}
      </div>
      <h3>Fan ratings</h3>${ratingsTable(e.ratings)}
    </section>`;
  }

  async function movie({ movie: id }) {
    const m = await api(`/movies/${encodeURIComponent(id)}`);
    crumbs([[`/movie/${id}`, m.title]]);
    return `<section class="panel">
      <h2>${esc(m.title)}</h2>
      <div class="stats">
        ${stat('Released', date(m.release_date))}
        ${stat('Runtime', m.runtime_minutes ? `${esc(m.runtime_minutes)} min` : '—')}
        ${stat('Budget', money(m.budget))}
        ${stat('Box office', money(m.box_office))}
        ${stat('Cast &amp; crew', dag('Movies.CrewCount', m.movie_id, esc(m.crew_count)))}
      </div>
      <div class="details">${field('Plot', esc(m.description) || '—')}</div>
      <h3>Cast &amp; crew</h3>
      ${m.crew.length ? `<table class="ratings">${m.crew.map((c) => `
        <tr><td>${dag('CrewAssignments.PersonName', c.crew_assignment_id, esc(c.person_name))}</td>
            <td>${dag('CrewAssignments.CrewTypeName', c.crew_assignment_id, esc(c.crew_type_name))}</td>
            <td class="muted">${esc(c.notes)}</td></tr>`).join('')}</table>` : '<p class="muted">No cast or crew recorded.</p>'}
    </section>`;
  }

  async function notFound() {
    crumbs([['', 'Not found']]);
    return '<section class="panel"><h2>Nothing here</h2><p><a href="/" data-link>Back to the bridge</a></p></section>';
  }

  const PAGES = { home, show, season, episode, movie, 'not-found': notFound };

  // ----------------------------------------------------------------- router
  let renderId = 0;
  async function render() {
    const id = ++renderId;
    const route = routeFor(window.location.pathname);
    $page().innerHTML = '<div class="loading">Loading…</div>';
    let html;
    try {
      html = await PAGES[route.page](route);
    } catch (err) {
      html = `<div class="error">${esc(err.message)}</div>`;
    }
    if (id !== renderId) return; // a newer navigation won
    $page().innerHTML = html;
    window.scrollTo(0, 0);
    if (window.EffortlessExplainer) window.EffortlessExplainer.enhanceCells($page());
  }

  document.addEventListener('click', (e) => {
    const a = e.target.closest('a[data-link]');
    if (!a || e.metaKey || e.ctrlKey || e.shiftKey || e.button !== 0) return;
    e.preventDefault();
    if (a.getAttribute('href') !== window.location.pathname) window.history.pushState({}, '', a.getAttribute('href'));
    render();
  });
  window.addEventListener('popstate', render);
  render();
})();
