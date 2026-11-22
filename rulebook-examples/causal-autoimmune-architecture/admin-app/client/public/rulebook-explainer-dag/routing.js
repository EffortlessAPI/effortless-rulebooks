/**
 * Routing adapter for portable explainer-dag.
 * Modes: hash (default), path, modal, page, callback, htmx
 *
 * One rule holds in every mode: a link lands on an explainer page. Where the host
 * page cannot show one itself, the link goes to the explainer's OWN page, a real
 * file beside this one (pages/index.html#/dag/<Table>/<Field>), never to an
 * address only the explainer knows. Until 2026-11-22 a table link in modal mode,
 * and every link in hash mode on a page with no outlet, sent the host to
 * /dag/<Table>; an app with no such route (a React app's router) showed its own
 * home page instead.
 *
 *   hash    the host page has <div id="effortless-explainer-outlet">: pages are
 *           drawn there, by #/dag/... . No outlet: the explainer's own page.
 *   path    the HOST routes /dag, /dag/:table, /dag/:table/:field to pages/*.html.
 *   modal   a field opens in a modal over the host page; a table and the index
 *           open the explainer's own page.
 *   page    every link opens the explainer's own page.
 */
(function (global) {
  function encodeSeg(s) {
    return encodeURIComponent(s);
  }

  // A link that loads another page would drop the language the host passed to
  // init(); carry it on the URL so the next page picks it up. English is what a
  // page speaks when nothing is said, so it is not carried.
  function langQuery(cfg) {
    return cfg.lang && cfg.lang !== "en" ? "lang=" + encodeURIComponent(cfg.lang) : "";
  }
  function withLang(url, cfg) {
    const q = langQuery(cfg);
    if (cfg.mode !== "path" || !q) return url;
    return url + (url.includes("?") ? "&" : "?") + q;
  }

  // The explainer's own page for a table, a field, or (neither) the index.
  function ownPage(table, field, cfg) {
    const base = String(cfg.ownBase || "/rulebook-explainer-dag").replace(/\/$/, "");
    const q = langQuery(cfg);
    return base + "/pages/index.html" + (q ? "?" + q : "") + "#/dag" +
      (table ? "/" + encodeSeg(table) + (field ? "/" + encodeSeg(field) : "") : "");
  }

  // Where the person was in the app when a link took them to the explainer's own page.
  const HOME_KEY = "effortless-explainer-home";
  function rememberedHome() {
    try { return global.sessionStorage.getItem(HOME_KEY) || null; } catch { return null; }
  }

  function hasOutlet() {
    const d = global.document;
    return !!(d && d.getElementById && d.getElementById("effortless-explainer-outlet"));
  }
  // Whether the page this runs in shows explainer pages by hash itself.
  function hostDrawsByHash(cfg) {
    return cfg.mode === "hash" && hasOutlet();
  }
  // Modes in which the explainer's own page is where a link goes.
  function goesToOwnPage(cfg) {
    return cfg.mode === "page" || cfg.mode === "modal" || (cfg.mode === "hash" && !hasOutlet());
  }

  function fieldPath(table, field, cfg) {
    if (cfg.mode === "path") {
      const tpl = cfg.fieldPath || "/dag/:table/:field";
      return withLang(tpl.replace(":table", encodeSeg(table)).replace(":field", encodeSeg(field)), cfg);
    }
    if (hostDrawsByHash(cfg)) {
      const prefix = cfg.hashPrefix || "#/dag/";
      return prefix + encodeSeg(table) + "/" + encodeSeg(field);
    }
    if (goesToOwnPage(cfg)) return ownPage(table, field, cfg);
    return "/dag/" + encodeSeg(table) + "/" + encodeSeg(field);
  }

  function tablePath(table, cfg) {
    if (!table) return indexPath(cfg);
    if (cfg.mode === "path") {
      const tpl = cfg.tablePath || "/dag/:table";
      return withLang(tpl.replace(":table", encodeSeg(table)), cfg);
    }
    if (hostDrawsByHash(cfg)) {
      const prefix = cfg.hashPrefix || "#/dag/";
      return prefix + encodeSeg(table);
    }
    if (goesToOwnPage(cfg)) return ownPage(table, null, cfg);
    return "/dag/" + encodeSeg(table);
  }

  function indexPath(cfg) {
    if (cfg.mode === "path") return withLang(cfg.indexPath || "/dag", cfg);
    if (hostDrawsByHash(cfg)) return (cfg.hashPrefix || "#/dag/").replace(/\/$/, "") || "#/dag";
    if (goesToOwnPage(cfg)) return ownPage(null, null, cfg);
    return "/dag";
  }

  function createRouting(cfg) {
    cfg = cfg || { mode: "hash" };

    // Draw by hash in the page this runs in (it has the outlet).
    function goHash(dest) {
      window.location.hash = dest.startsWith("#") ? dest.slice(1) : dest;
      if (global.EffortlessExplainer && global.EffortlessExplainer.renderFromHash) {
        global.EffortlessExplainer.renderFromHash();
      }
    }

    // Leave the host page for another one. Going to the explainer's own page, the
    // address left behind is kept for that page's Home button (this tab only).
    function leaveFor(dest) {
      if (goesToOwnPage(cfg)) {
        try { global.sessionStorage.setItem(HOME_KEY, window.location.href); } catch { /* no storage: Home is "/" */ }
      }
      window.location.href = dest;
    }

    function navigate(table, field) {
      if (cfg.routing && cfg.routing.navigate) {
        cfg.routing.navigate(table, field);
        return;
      }
      if (cfg.mode === "modal") {
        global.EffortlessExplainer && global.EffortlessExplainer.openModal(table, field);
        return;
      }
      if (cfg.mode === "htmx" && cfg.htmxFieldUrl) {
        window.location.href = cfg.htmxFieldUrl
          .replace(":table", encodeSeg(table))
          .replace(":field", encodeSeg(field));
        return;
      }
      const dest = fieldPath(table, field, cfg);
      if (hostDrawsByHash(cfg)) { goHash(dest); return; }
      leaveFor(dest);
    }

    // A table's page; no table is the index of tables.
    function navigateTable(table) {
      if (cfg.routing && cfg.routing.navigateTable) {
        cfg.routing.navigateTable(table);
        return;
      }
      const dest = tablePath(table, cfg);
      if (hostDrawsByHash(cfg)) { goHash(dest); return; }
      leaveFor(dest);
    }

    function fieldHref(table, field) {
      if (cfg.routing && cfg.routing.fieldHref) return cfg.routing.fieldHref(table, field);
      return fieldPath(table, field, cfg);
    }

    function tableHref(table) {
      if (cfg.routing && cfg.routing.tableHref) return cfg.routing.tableHref(table);
      return tablePath(table, cfg);
    }

    // In a modal the explainer is a layer over the host page: Back and Home act on
    // the layer. They used to walk the host's history and send it to homePath.
    function onBack() {
      if (cfg.routing && cfg.routing.onBack) return cfg.routing.onBack();
      if (cfg.mode === "modal" && global.EffortlessExplainer && global.EffortlessExplainer.modalBack) {
        return global.EffortlessExplainer.modalBack();
      }
      window.history.back();
    }

    function onHome() {
      if (cfg.routing && cfg.routing.onHome) return cfg.routing.onHome();
      if (cfg.mode === "modal" && global.EffortlessExplainer && global.EffortlessExplainer.closeModal) {
        return global.EffortlessExplainer.closeModal();
      }
      // The explainer's own page was reached FROM the app: Home is the way back.
      if (hostDrawsByHash(cfg) && !cfg.ownPage) {
        window.location.hash = "#/";
        return;
      }
      window.location.href = cfg.homePath || "/";
    }

    return { navigate, navigateTable, fieldHref, tableHref, onBack, onHome, cfg };
  }

  global.EffortlessExplainerRouting = { createRouting, fieldPath, tablePath, indexPath, ownPage, rememberedHome };
})(typeof window !== "undefined" ? window : globalThis);
