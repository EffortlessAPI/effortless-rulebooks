// What the two admin pages share (the ontology form in admin.js, the concept form in
// concepts.js): escaping, the list of unsaved changes, and watching a server job.
window.RulebookAdmin = (function () {
  const esc = (t) => String(t ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

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

  return { esc, changeLines, jobs };
})();
