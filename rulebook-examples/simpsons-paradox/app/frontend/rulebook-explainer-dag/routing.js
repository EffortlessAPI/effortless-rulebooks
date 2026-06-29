/**
 * Routing adapter for portable explainer-dag.
 * Modes: hash (default), path, modal, callback, htmx
 */
(function (global) {
  function encodeSeg(s) {
    return encodeURIComponent(s);
  }

  function fieldPath(table, field, cfg) {
    if (cfg.mode === "path") {
      const tpl = cfg.fieldPath || "/dag/:table/:field";
      return tpl.replace(":table", encodeSeg(table)).replace(":field", encodeSeg(field));
    }
    if (cfg.mode === "hash") {
      const prefix = cfg.hashPrefix || "#/dag/";
      return prefix + encodeSeg(table) + "/" + encodeSeg(field);
    }
    return "/dag/" + encodeSeg(table) + "/" + encodeSeg(field);
  }

  function tablePath(table, cfg) {
    if (cfg.mode === "path") {
      const tpl = cfg.tablePath || "/dag/:table";
      return tpl.replace(":table", encodeSeg(table));
    }
    if (cfg.mode === "hash") {
      const prefix = cfg.hashPrefix || "#/dag/";
      return prefix + encodeSeg(table);
    }
    return "/dag/" + encodeSeg(table);
  }

  function indexPath(cfg) {
    if (cfg.mode === "path") return cfg.indexPath || "/dag";
    if (cfg.mode === "hash") return (cfg.hashPrefix || "#/dag/").replace(/\/$/, "") || "#/dag";
    return "/dag";
  }

  function createRouting(cfg) {
    cfg = cfg || { mode: "hash" };

    function navigate(table, field) {
      if (cfg.routing && cfg.routing.navigate) {
        cfg.routing.navigate(table, field);
        return;
      }
      const dest = fieldPath(table, field, cfg);
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
      if (cfg.mode === "hash") {
        window.location.hash = dest.startsWith("#") ? dest.slice(1) : dest;
        if (global.EffortlessExplainer && global.EffortlessExplainer.renderFromHash) {
          global.EffortlessExplainer.renderFromHash();
        }
        return;
      }
      window.location.href = dest;
    }

    function navigateTable(table) {
      if (cfg.routing && cfg.routing.navigateTable) {
        cfg.routing.navigateTable(table);
        return;
      }
      const dest = tablePath(table, cfg);
      if (cfg.mode === "modal") {
        window.location.href = dest;
        return;
      }
      if (cfg.mode === "hash") {
        window.location.hash = dest.startsWith("#") ? dest.slice(1) : dest;
        if (global.EffortlessExplainer && global.EffortlessExplainer.renderFromHash) {
          global.EffortlessExplainer.renderFromHash();
        }
        return;
      }
      window.location.href = dest;
    }

    function fieldHref(table, field) {
      if (cfg.routing && cfg.routing.fieldHref) return cfg.routing.fieldHref(table, field);
      return fieldPath(table, field, cfg);
    }

    function tableHref(table) {
      if (cfg.routing && cfg.routing.tableHref) return cfg.routing.tableHref(table);
      return tablePath(table, cfg);
    }

    function onBack() {
      if (cfg.routing && cfg.routing.onBack) return cfg.routing.onBack();
      window.history.back();
    }

    function onHome() {
      if (cfg.routing && cfg.routing.onHome) return cfg.routing.onHome();
      if (cfg.mode === "hash") {
        window.location.hash = "#/";
        return;
      }
      window.location.href = cfg.homePath || "/";
    }

    return { navigate, navigateTable, fieldHref, tableHref, onBack, onHome, cfg };
  }

  global.EffortlessExplainerRouting = { createRouting, fieldPath, tablePath, indexPath };
})(typeof window !== "undefined" ? window : globalThis);
