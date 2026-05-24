// ============================================================================
// ERB Admin Portal — Frontend
// ----------------------------------------------------------------------------
// The frontend is THIN. It loads the rulebook's AppNavigation, AppScreens,
// UserRoles, and AppPermissions, and renders the UI from them. Each screen
// kind ('dashboard', 'split-detail', 'list', 'grid', 'editor') is one small
// renderer; the rulebook decides which screen renders where, gated by role.
// ============================================================================

import React, { useEffect, useState, useMemo, useCallback } from "react";
import { createRoot } from "react-dom/client";
import htm from "htm";

const html = htm.bind(React.createElement);

// ---------------------------------------------------------------------------
// API helpers
// ---------------------------------------------------------------------------
const api = {
  async get(path) {
    const r = await fetch(path);
    if (!r.ok) throw new Error(`${r.status} ${path}`);
    return r.json();
  },
  async post(path, body) {
    const r = await fetch(path, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body || {}),
    });
    if (!r.ok) throw new Error((await r.text()) || `${r.status} ${path}`);
    return r.json();
  },
  async patch(path, body) {
    const r = await fetch(path, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });
    if (!r.ok) throw new Error((await r.text()) || `${r.status} ${path}`);
    return r.json();
  },
  async del(path) {
    const r = await fetch(path, { method: "DELETE" });
    if (!r.ok) throw new Error((await r.text()) || `${r.status} ${path}`);
    return r.json();
  },
  async put(path, body, asText = false) {
    const r = await fetch(path, {
      method: "PUT",
      headers: { "Content-Type": asText ? "text/plain" : "application/json" },
      body: asText ? body : JSON.stringify(body),
    });
    if (!r.ok) throw new Error((await r.text()) || `${r.status} ${path}`);
    return r.json();
  },
};

// ---------------------------------------------------------------------------
// Toast
// ---------------------------------------------------------------------------
let toastSetter = null;
function toast(message, kind = "ok") {
  if (toastSetter) toastSetter({ message, kind, ts: Date.now() });
  setTimeout(() => toastSetter && toastSetter(null), 4000);
}

function Toast() {
  const [t, setT] = useState(null);
  useEffect(() => { toastSetter = setT; return () => { toastSetter = null; }; }, []);
  if (!t) return null;
  return html`<div class=${"toast " + (t.kind || "")}>${t.message}</div>`;
}

// ---------------------------------------------------------------------------
// App root
// ---------------------------------------------------------------------------
function App() {
  const [me, setMe] = useState(null);
  const [rulebook, setRulebook] = useState(null);
  const [projects, setProjects] = useState({ active: null, projects: [] });
  const [route, setRoute] = useState(location.hash.slice(1) || "/");

  useEffect(() => {
    const onHash = () => setRoute(location.hash.slice(1) || "/");
    window.addEventListener("hashchange", onHash);
    return () => window.removeEventListener("hashchange", onHash);
  }, []);

  const reloadAll = useCallback(async () => {
    try {
      const [m, rb, pj] = await Promise.all([
        api.get("/api/me"),
        api.get("/api/rulebook"),
        api.get("/api/projects"),
      ]);
      setMe(m); setRulebook(rb); setProjects(pj);
    } catch (e) {
      toast("Failed to load portal: " + e.message, "error");
    }
  }, []);

  useEffect(() => { reloadAll(); }, [reloadAll]);

  if (!rulebook || !me) return html`<div style=${{ padding: 30 }}>Loading ERB Admin Portal…</div>`;

  // Diagnose portal-config presence BEFORE rendering the shell. Without these
  // tables the sidebar and router have nothing to drive them and the page
  // appears blank with a useless "No screen for /" message. Show a clear
  // diagnostic instead so the failure is impossible to miss.
  const portalDiag = diagnosePortalConfig({ me, rulebook, projects });
  if (portalDiag.broken) {
    return html`<${PortalSetupNeeded} diag=${portalDiag} projects=${projects} rulebook=${rulebook} reload=${reloadAll} />`;
  }

  return html`
    <${TopBar} me=${me} projects=${projects} reload=${reloadAll} rulebook=${rulebook} />
    <div class="layout">
      <${Sidebar} rulebook=${rulebook} me=${me} route=${route} />
      <div class="main">
        <${Router} route=${route} rulebook=${rulebook} me=${me} projects=${projects} reload=${reloadAll} />
      </div>
    </div>
    <${Toast} />
  `;
}

// ---------------------------------------------------------------------------
// Portal-config diagnostics — surface "the rulebook is missing the tables that
// drive the admin portal itself" loudly, not silently.
// ---------------------------------------------------------------------------
const PORTAL_REQUIRED_TABLES = [
  "UserRoles", "AppUsers", "AppPermissions", "AppNavigation", "AppScreens",
];

function diagnosePortalConfig({ me, rulebook, projects }) {
  const missingTables = PORTAL_REQUIRED_TABLES.filter(
    (t) => !rulebook[t] || !Array.isArray(rulebook[t].data) || rulebook[t].data.length === 0
  );
  const userError = me && me.error ? me.error : null;
  const broken = missingTables.length > 0 || !!userError;
  return { broken, missingTables, userError, activeProject: projects.active };
}

function PortalSetupNeeded({ diag, projects, rulebook, reload }) {
  return html`
    <div style=${{ padding: 32, maxWidth: 820, margin: "40px auto", fontFamily: "system-ui, sans-serif" }}>
      <h1 style=${{ marginTop: 0 }}>⚠ ERB Admin Portal — setup needed</h1>
      <p>
        The portal is data-driven from a set of <strong>portal-config tables</strong> inside the
        active rulebook. The current rulebook does not have what the portal needs to render,
        so the UI is blank.
      </p>

      <div style=${{ background: "#fff8e6", border: "1px solid #f0c36d", padding: 16, borderRadius: 6, margin: "16px 0" }}>
        <div><strong>Active project:</strong> <code>${diag.activeProject}</code></div>
        <div><strong>Active rulebook:</strong> <code>${rulebook.Name}</code></div>
        ${diag.userError ? html`<div style=${{ marginTop: 8, color: "#c0392b" }}>
          <strong>/api/me error:</strong> ${diag.userError}
        </div>` : null}
        ${diag.missingTables.length > 0 ? html`<div style=${{ marginTop: 8 }}>
          <strong>Missing or empty portal-config tables:</strong>
          <ul style=${{ marginTop: 4 }}>
            ${diag.missingTables.map((t) => html`<li key=${t}><code>${t}</code></li>`)}
          </ul>
        </div>` : null}
      </div>

      <h3>How to fix</h3>
      <ol>
        <li>
          Seed the meta-rulebook at
          <code>effortless-rulebook/effortless-rulebook.json</code>
          with the five portal-config tables
          (<code>UserRoles</code>, <code>AppUsers</code>, <code>AppPermissions</code>,
          <code>AppNavigation</code>, <code>AppScreens</code>).
          The server's <code>loadRulebookWithPortalFallback()</code> will splice these
          into every project automatically.
        </li>
        <li>
          Or switch to a project whose rulebook already defines them:
          <select value=${projects.active} onChange=${async (e) => {
            try {
              await api.post(`/api/projects/${encodeURIComponent(e.target.value)}/activate`);
              await reload();
            } catch (err) { toast("Switch failed: " + err.message, "error"); }
          }} style=${{ marginLeft: 8 }}>
            ${projects.projects.map((p) => html`<option key=${p.id} value=${p.id}>${p.name}</option>`)}
          </select>
        </li>
        <li>
          Reload after fixing:
          <button onClick=${() => reload()} style=${{ marginLeft: 8 }}>↻ Reload</button>
        </li>
      </ol>

      <details style=${{ marginTop: 24 }}>
        <summary>Raw diagnostic</summary>
        <pre style=${{ background: "#f5f5f5", padding: 12, borderRadius: 4, overflow: "auto" }}>${JSON.stringify(diag, null, 2)}</pre>
      </details>
    </div>
  `;
}

// ---------------------------------------------------------------------------
// Top bar
// ---------------------------------------------------------------------------
function TopBar({ me, projects, rulebook, reload }) {
  const activeProj = projects.projects.find((p) => p.id === projects.active) || projects.projects[0];
  return html`
    <div class="topbar">
      <div class="brand">ERB Admin</div>
      <div class="crumbs">
        Project: <strong>${activeProj?.name || projects.active}</strong>
        <span class="muted small"> · ${rulebook.Name}</span>
      </div>
      <${ProjectSwitcher} projects=${projects} reload=${reload} />
      <${UserSwitcher} me=${me} rulebook=${rulebook} reload=${reload} />
    </div>
  `;
}

function ProjectSwitcher({ projects, reload }) {
  const onChange = async (e) => {
    try {
      await api.post(`/api/projects/${encodeURIComponent(e.target.value)}/activate`);
      toast("Switched project");
      await reload();
      location.hash = "#/";
    } catch (err) { toast("Switch failed: " + err.message, "error"); }
  };
  return html`
    <select value=${projects.active} onChange=${onChange} style=${{ width: 200 }}>
      ${projects.projects.map((p) => html`<option key=${p.id} value=${p.id}>${p.name}</option>`)}
    </select>
  `;
}

function UserSwitcher({ me, rulebook, reload }) {
  const users = rulebook.AppUsers?.data || [];
  const roles = rulebook.UserRoles?.data || [];
  const role = roles.find((r) => r.RoleId === me.RoleId);
  return html`
    <span class=${"pill " + (role?.Name?.toLowerCase() || "")}>${role?.Name || me.RoleId}</span>
    <select value=${me.UserId} onChange=${async (e) => {
      await api.post("/api/me/switch", { userId: e.target.value });
      toast("Signed in as new user");
      await reload();
    }} style=${{ width: 180 }}>
      ${users.map((u) => html`<option key=${u.UserId} value=${u.UserId}>${u.DisplayName} (${u.Email})</option>`)}
    </select>
  `;
}

// ---------------------------------------------------------------------------
// Sidebar (driven by AppNavigation in the rulebook)
// ---------------------------------------------------------------------------
function roleMeetsMin(myRole, minRoleId, roles) {
  const min = roles.find((r) => r.RoleId === minRoleId);
  if (!min) return true;
  // Developer satisfies all; viewer only satisfies viewer
  if (myRole?.AccessLevel === "full-admin") return true;
  return min.AccessLevel === "read";
}

function Sidebar({ rulebook, me, route }) {
  const nav = (rulebook.AppNavigation?.data || []).slice().sort((a, b) => (a.Order ?? 0) - (b.Order ?? 0));
  const roles = rulebook.UserRoles?.data || [];
  const myRole = roles.find((r) => r.RoleId === me.RoleId);
  const screens = rulebook.AppScreens?.data || [];
  const screenPath = (sid) => screens.find((s) => s.ScreenId === sid)?.Path || "/";
  const topLevel = nav.filter((n) => !n.ParentNavId);

  return html`
    <div class="sidebar">
      ${topLevel.map((n) => {
        if (!roleMeetsMin(myRole, n.MinRoleId, roles)) return null;
        const children = nav.filter((c) => c.ParentNavId === n.NavId)
                            .filter((c) => roleMeetsMin(myRole, c.MinRoleId, roles));
        const target = n.ScreenId ? screenPath(n.ScreenId) : null;
        const active = target === route;
        return html`
          <div key=${n.NavId}>
            <div class=${"nav-item " + (active ? "active" : "")}
                 onClick=${() => { if (target) location.hash = "#" + target; }}>
              <span>${n.Label}</span>
            </div>
            ${children.map((c) => {
              const cp = screenPath(c.ScreenId);
              const cActive = cp === route;
              return html`
                <div key=${c.NavId} class=${"nav-item child " + (cActive ? "active" : "")}
                     onClick=${() => { location.hash = "#" + cp; }}>
                  ${c.Label}
                </div>
              `;
            })}
          </div>
        `;
      })}
      <div style=${{ padding: "16px 14px", marginTop: 12, color: "var(--muted)", fontSize: 11 }}>
        rulebook is SSoT · postgres is editor
      </div>
    </div>
  `;
}

// ---------------------------------------------------------------------------
// Router
// ---------------------------------------------------------------------------
function Router({ route, rulebook, me, projects, reload }) {
  const roles = rulebook.UserRoles?.data || [];
  const myRole = roles.find((r) => r.RoleId === me.RoleId);
  const screens = rulebook.AppScreens?.data || [];
  const screen = screens.find((s) => s.Path === route) || screens.find((s) => s.Path === "/");

  if (!screen) return html`<div>No screen for ${route}</div>`;
  if (!roleMeetsMin(myRole, screen.MinRoleId, roles)) {
    return html`
      <h2 class="h1">Forbidden</h2>
      <p class="subtitle">This screen requires the
        <span class="pill developer">${(roles.find(r=>r.RoleId===screen.MinRoleId)||{}).Name || screen.MinRoleId}</span>
        role.</p>
    `;
  }

  const isDev = myRole?.AccessLevel === "full-admin";
  const props = { screen, rulebook, me, isDev, projects, reload };

  switch (screen.ScreenId) {
    case "screen-home":          return html`<${HomeScreen} ...${props} />`;
    case "screen-entities":      return html`<${EntitiesScreen} ...${props} />`;
    case "screen-formulas":      return html`<${FormulasScreen} ...${props} />`;
    case "screen-relationships": return html`<${RelationshipsScreen} ...${props} />`;
    case "screen-sample-data":   return html`<${SampleDataScreen} ...${props} />`;
    case "screen-substrates":    return html`<${SubstratesScreen} ...${props} />`;
    case "screen-add-tool":      return html`<${AddToolScreen} ...${props} />`;
    case "screen-builds":        return html`<${BuildsScreen} ...${props} />`;
    case "screen-tests":         return html`<${TestsScreen} ...${props} />`;
    case "screen-input-spokes":  return html`<${SpokesScreen} ...${props} />`;
    case "screen-users":         return html`<${UsersScreen} ...${props} />`;
    case "screen-tech-postgres": return html`<${TechPostgresScreen} ...${props} />`;
    case "screen-tech-proxy":    return html`<${TechProxyScreen} ...${props} />`;
    case "screen-tech-files":    return html`<${TechFilesScreen} ...${props} />`;
    case "screen-tech-json":     return html`<${TechJsonScreen} ...${props} />`;
    case "screen-tech-reset":    return html`<${TechResetScreen} ...${props} />`;
    default: return html`<div>No renderer for screen ${screen.ScreenId}</div>`;
  }
}

// ---------------------------------------------------------------------------
// Common: ScreenHeader (renders the story banner from the rulebook)
// ---------------------------------------------------------------------------
function ScreenHeader({ screen }) {
  return html`
    <h2 class="h1">${screen.Title}</h2>
    ${screen.Story ? html`<div class="story-banner">${screen.Story}</div>` : null}
  `;
}

// ---------------------------------------------------------------------------
// Home
// ---------------------------------------------------------------------------
function HomeScreen({ screen, rulebook, projects, me }) {
  const subs = rulebook.ExecutionSubstrates?.data || [];
  const spokes = rulebook.RulebookSourceSpokes?.data || [];
  const flows = rulebook.CoreDataFlows?.data || [];
  const meta = rulebook.ProjectMetadata?.data?.[0] || {};
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="cards">
      <div class="card"><h3>Active project</h3><div class="big">${projects.active}</div><div class="sub">${meta.Architecture || ""}</div></div>
      <div class="card"><h3>Rulebook entities</h3><div class="big">${Object.keys(rulebook).filter(k => !k.startsWith("$") && !k.startsWith("_") && rulebook[k]?.schema).length}</div></div>
      <div class="card"><h3>Substrates</h3><div class="big">${subs.length}</div><div class="sub">${subs.filter(s=>s.IsProduction).length} production</div></div>
      <div class="card"><h3>Input spokes</h3><div class="big">${spokes.length}</div><div class="sub">all peer; rulebook JSON wins</div></div>
      <div class="card"><h3>Core flows</h3><div class="big">${flows.length}</div></div>
      <div class="card"><h3>Signed in as</h3><div class="big">${me.DisplayName}</div><div class="sub">${me.role?.Name}</div></div>
      <div class="card"><h3>Portal URL</h3><div class="big mono">${location.host}</div><div class="sub">${meta.ProxyUrl || ""}</div></div>
      <div class="card"><h3>Repo root</h3><div class="big mono">${meta.RepositoryRoot || ""}</div></div>
    </div>
    <h3 style=${{ marginTop: 28 }}>What you can do here</h3>
    <ul class="muted">
      <li>Click <b>Rulebook → Entities</b> to see (and as Developer, edit) the business semantics of this project.</li>
      <li>Click <b>Substrates</b> to see every output target — Python, Go, Postgres, Excel, OWL, etc.</li>
      <li>Click <b>Substrates → Add Tool</b> to install another transpiler from the catalog of 15+.</li>
      <li>Click <b>Tests</b> to see the conformance matrix proving all substrates compute identically.</li>
      <li>Use <b>Tech Tools</b> only when something looks off — raw Postgres, proxy logs, reset DB.</li>
    </ul>
  `;
}

// ---------------------------------------------------------------------------
// Entities
// ---------------------------------------------------------------------------
function EntitiesScreen({ screen, rulebook, isDev }) {
  const [entities, setEntities] = useState([]);
  const [selected, setSelected] = useState(null);
  const [entity, setEntity] = useState(null);
  const [editingDesc, setEditingDesc] = useState("");

  useEffect(() => {
    api.get("/api/rulebook/entities").then((es) => {
      setEntities(es);
      if (es.length && !selected) setSelected(es[0].name);
    });
  }, [rulebook]);

  useEffect(() => {
    if (!selected) return;
    api.get(`/api/rulebook/entities/${encodeURIComponent(selected)}`).then((e) => {
      setEntity(e); setEditingDesc(e.Description || "");
    });
  }, [selected]);

  const saveDesc = async () => {
    try {
      await api.patch(`/api/rulebook/entities/${encodeURIComponent(selected)}`, { description: editingDesc });
      toast("Saved (Postgres + rulebook JSON).");
    } catch (e) { toast(e.message, "error"); }
  };

  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="split">
      <div class="list-panel">
        ${entities.map((e) => html`
          <div key=${e.name} class=${"list-item " + (e.name === selected ? "active" : "")}
               onClick=${() => setSelected(e.name)}>
            <div class="name">${e.name}</div>
            <div class="meta">${e.fieldCount} fields · ${e.rowCount} rows</div>
          </div>
        `)}
      </div>
      <div class="detail-panel">
        ${entity ? html`
          <h3 class="mono" style=${{ marginTop: 0 }}>${entity.name}</h3>
          <div class="kv">
            <div class="k">Description</div>
            <div class="v">
              <textarea rows="2" disabled=${!isDev} value=${editingDesc}
                onChange=${(e) => setEditingDesc(e.target.value)} />
              ${isDev ? html`<div style=${{ marginTop: 6 }}><button class="btn" onClick=${saveDesc}>Save (write-through)</button></div>` : null}
            </div>
            <div class="k">Fields</div>
            <div class="v">${(entity.schema || []).length}</div>
            <div class="k">Rows</div>
            <div class="v">${(entity.data || []).length}</div>
          </div>
          <h4>Schema</h4>
          <table class="grid">
            <thead><tr><th>name</th><th>type</th><th>datatype</th><th>nullable</th><th>description</th></tr></thead>
            <tbody>
              ${(entity.schema || []).map((f) => html`
                <tr key=${f.name}><td>${f.name}</td><td>${f.type || ""}</td><td>${f.datatype || ""}</td><td>${String(f.nullable ?? "")}</td><td>${f.Description || ""}</td></tr>
              `)}
            </tbody>
          </table>
          ${entity.data?.length ? html`
            <h4>Sample data (${entity.data.length} rows)</h4>
            <div style=${{ overflowX: "auto" }}>
              <table class="grid">
                <thead><tr>${Object.keys(entity.data[0]).map((k) => html`<th key=${k}>${k}</th>`)}</tr></thead>
                <tbody>${entity.data.slice(0, 30).map((row, i) => html`
                  <tr key=${i}>${Object.keys(entity.data[0]).map((k) => html`<td key=${k}>${formatCell(row[k])}</td>`)}</tr>
                `)}</tbody>
              </table>
              ${entity.data.length > 30 ? html`<div class="muted small">...showing 30 of ${entity.data.length}</div>` : null}
            </div>
          ` : null}
        ` : html`<div class="muted">Select an entity</div>`}
      </div>
    </div>
  `;
}
function formatCell(v) {
  if (v === null || v === undefined) return html`<span class="muted">∅</span>`;
  if (typeof v === "boolean") return v ? "✓" : "✗";
  if (typeof v === "object") return JSON.stringify(v);
  return String(v);
}

// ---------------------------------------------------------------------------
// Formulas — list every calculated field in the rulebook
// ---------------------------------------------------------------------------
function FormulasScreen({ screen, rulebook }) {
  const rows = [];
  for (const [name, value] of Object.entries(rulebook)) {
    if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
    for (const f of value.schema) {
      if (f.type && f.type !== "raw") {
        rows.push({ entity: name, field: f.name, type: f.type, datatype: f.datatype, formula: f.formula || f.Formula || "", desc: f.Description || "" });
      }
    }
  }
  return html`
    <${ScreenHeader} screen=${screen} />
    <table class="grid">
      <thead><tr><th>entity</th><th>field</th><th>type</th><th>datatype</th><th>formula</th><th>description</th></tr></thead>
      <tbody>
        ${rows.length === 0 ? html`<tr><td colspan="6" class="muted">No calculated fields in this rulebook.</td></tr>` :
          rows.map((r, i) => html`
            <tr key=${i}><td>${r.entity}</td><td>${r.field}</td><td><span class="tag">${r.type}</span></td><td>${r.datatype}</td><td class="mono">${r.formula}</td><td>${r.desc}</td></tr>
          `)}
      </tbody>
    </table>
  `;
}

// ---------------------------------------------------------------------------
// Relationships — text rendering of FK graph
// ---------------------------------------------------------------------------
function RelationshipsScreen({ screen, rulebook }) {
  const edges = [];
  for (const [name, value] of Object.entries(rulebook)) {
    if (!value || typeof value !== "object" || !Array.isArray(value.schema)) continue;
    for (const f of value.schema) {
      if (f.type === "lookup" && f.Description) {
        const m = f.Description.match(/FK to\s+(\w+)/i);
        if (m) edges.push({ from: name, field: f.name, to: m[1] });
      }
    }
  }
  return html`
    <${ScreenHeader} screen=${screen} />
    <table class="grid">
      <thead><tr><th>from</th><th>field</th><th>to</th></tr></thead>
      <tbody>
        ${edges.length === 0 ? html`<tr><td colspan="3" class="muted">No FK relationships declared via Description hints.</td></tr>` :
          edges.map((e, i) => html`<tr key=${i}><td>${e.from}</td><td class="mono">${e.field}</td><td>${e.to}</td></tr>`)}
      </tbody>
    </table>
  `;
}

// ---------------------------------------------------------------------------
// Sample data — pick an entity and view its data block
// ---------------------------------------------------------------------------
function SampleDataScreen({ screen, rulebook }) {
  const entities = Object.entries(rulebook).filter(([k, v]) =>
    !k.startsWith("$") && !k.startsWith("_") && v && typeof v === "object" && Array.isArray(v.schema)
  );
  const [pick, setPick] = useState(entities[0]?.[0]);
  const ent = pick ? rulebook[pick] : null;
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="flex"><label>Entity:</label>
      <select value=${pick} onChange=${(e) => setPick(e.target.value)} style=${{ width: 280 }}>
        ${entities.map(([k]) => html`<option key=${k} value=${k}>${k}</option>`)}
      </select>
    </div>
    ${ent && ent.data?.length ? html`
      <div style=${{ overflowX: "auto", marginTop: 14 }}>
        <table class="grid">
          <thead><tr>${Object.keys(ent.data[0]).map((k) => html`<th key=${k}>${k}</th>`)}</tr></thead>
          <tbody>${ent.data.map((row, i) => html`
            <tr key=${i}>${Object.keys(ent.data[0]).map((k) => html`<td key=${k}>${formatCell(row[k])}</td>`)}</tr>
          `)}</tbody>
        </table>
      </div>
    ` : html`<div class="muted" style=${{ marginTop: 14 }}>No sample data for this entity.</div>`}
  `;
}

// ---------------------------------------------------------------------------
// Substrates
// ---------------------------------------------------------------------------
function SubstratesScreen({ screen, isDev }) {
  const [subs, setSubs] = useState([]);
  const [selected, setSelected] = useState(null);
  useEffect(() => { api.get("/api/substrates").then(setSubs); }, []);
  const sel = subs.find((s) => s.SubstrateId === selected) || subs[0];
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="flex" style=${{ marginBottom: 12 }}>
      <div class="muted small">${subs.length} substrates registered in this project.</div>
      <div class="spacer" />
      <button class="btn secondary" onClick=${() => { location.hash = "#/tools/add"; }}>+ Add Tool</button>
      ${isDev ? html`<button class="btn" onClick=${async () => {
        toast("Building…");
        const r = await api.post("/api/build/all");
        toast(r.ok ? "Build complete" : "Build failed", r.ok ? "ok" : "error");
      }}>Build all</button>` : null}
    </div>
    <div class="split">
      <div class="list-panel">
        ${subs.map((s) => html`
          <div key=${s.SubstrateId} class=${"list-item " + (s.SubstrateId === sel?.SubstrateId ? "active" : "")}
               onClick=${() => setSelected(s.SubstrateId)}>
            <div class="name">${s.Name} <span class="tag">${s.Technology}</span></div>
            <div class="meta">${s.RelativePath}${s.IsProduction ? html` · <span class="pill good">prod</span>` : ""}${!s.exists ? html` · <span class="pill warn">not built</span>` : ""}</div>
          </div>
        `)}
      </div>
      <div class="detail-panel">
        ${sel ? html`
          <h3 style=${{ marginTop: 0 }}>${sel.Name}</h3>
          <div class="kv">
            <div class="k">Technology</div><div class="v">${sel.Technology}</div>
            <div class="k">Path</div><div class="v">${sel.RelativePath}</div>
            <div class="k">Injector</div><div class="v">${sel.InjectorScript}</div>
            <div class="k">Production</div><div class="v">${sel.IsProduction ? "yes" : "local-only demo"}</div>
            <div class="k">Status</div><div class="v">${sel.Status}</div>
          </div>
          <p>${sel.Description}</p>
        ` : html`<div class="muted">No substrate selected.</div>`}
      </div>
    </div>
  `;
}

// ---------------------------------------------------------------------------
// Add Tool — drives off rulebook AddToolCatalog + live proxy /ping
// ---------------------------------------------------------------------------
function AddToolScreen({ screen, rulebook }) {
  const [catalog, setCatalog] = useState(null);
  const [installed, setInstalled] = useState({ transpilers: [] });
  const [busy, setBusy] = useState(null);

  const load = async () => {
    setCatalog(await api.get("/api/tools/catalog"));
    setInstalled(await api.get("/api/tools/installed"));
  };
  useEffect(() => { load(); }, []);

  const install = async (tool) => {
    setBusy(tool.ToolId);
    try {
      const r = await api.post("/api/tools/install", { installUrl: tool.InstallUrl, outputPath: tool.OutputPath });
      toast(r.ok ? `Installed ${tool.Name}` : "Install failed", r.ok ? "ok" : "error");
      await load();
    } catch (e) { toast(e.message, "error"); }
    setBusy(null);
  };

  if (!catalog) return html`<div>Loading catalog…</div>`;
  const installedNames = new Set((installed.transpilers || []).map((t) => (t.Name || "").toLowerCase()));
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="flex" style=${{ marginBottom: 10 }}>
      <div class="muted small">
        Catalog: <span class="mono">${(catalog.fromRulebook || []).length}</span> tools (rulebook)
        · live proxy: <span class="mono">${catalog.liveProxy?.transpilers ? Object.keys(catalog.liveProxy.transpilers).length : "offline"}</span> routes
        · installed in project: <span class="mono">${(installed.transpilers || []).length}</span>
      </div>
    </div>
    <div class="cards">
      ${(catalog.fromRulebook || []).map((t) => {
        const already = installedNames.has((t.Name || "").replace(/-/g, "").toLowerCase()) ||
                        installedNames.has((t.Name || "").toLowerCase());
        return html`
          <div class="card" key=${t.ToolId}>
            <h3>${t.Category}</h3>
            <div class="big" style=${{ fontSize: 16 }}>${t.Name}</div>
            <div class="sub">${t.Description}</div>
            <div class="muted small mono" style=${{ marginTop: 6, wordBreak: "break-all" }}>${t.InstallUrl}</div>
            <div class="muted small">→ ${t.OutputPath}</div>
            <div style=${{ marginTop: 10 }}>
              ${already
                ? html`<span class="pill good">installed</span>`
                : html`<button class="btn" disabled=${busy===t.ToolId} onClick=${() => install(t)}>${busy===t.ToolId ? "Installing…" : "Install"}</button>`}
            </div>
          </div>
        `;
      })}
    </div>
  `;
}

// ---------------------------------------------------------------------------
// Builds (lightweight — show installed tools + trigger)
// ---------------------------------------------------------------------------
function BuildsScreen({ screen, isDev }) {
  const [installed, setInstalled] = useState({ transpilers: [] });
  const [last, setLast] = useState(null);
  useEffect(() => { api.get("/api/tools/installed").then(setInstalled); }, []);
  const run = async () => {
    toast("Building all…");
    const r = await api.post("/api/build/all");
    setLast(r);
    toast(r.ok ? "Build complete" : "Build failed", r.ok ? "ok" : "error");
  };
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="flex" style=${{ marginBottom: 10 }}>
      <div class="muted small">Installed transpilers: ${(installed.transpilers || []).length}</div>
      <div class="spacer" />
      ${isDev ? html`<button class="btn" onClick=${run}>Trigger build</button>` : null}
    </div>
    <table class="grid">
      <thead><tr><th>name</th><th>path</th><th>disabled</th><th>pinned</th></tr></thead>
      <tbody>${(installed.transpilers || []).map((t, i) => html`
        <tr key=${i}><td>${t.Name}</td><td>${t.RelativePath}</td><td>${String(t.IsDisabled || false)}</td><td>${t.PinnedVersion || ""}</td></tr>
      `)}</tbody>
    </table>
    ${last ? html`
      <h4>Last build</h4>
      <div class="editor">${last.stdout}\n${last.stderr}</div>
    ` : null}
  `;
}

// ---------------------------------------------------------------------------
// Tests (placeholder — wires to /api/tests once tests are formally exposed)
// ---------------------------------------------------------------------------
function TestsScreen({ screen, rulebook }) {
  const framework = rulebook.TestingFramework?.data || [];
  return html`
    <${ScreenHeader} screen=${screen} />
    <p class="muted">Conformance test runner is wired to the substrate take-test.py scripts; surfacing the matrix here is the next iteration.</p>
    <table class="grid">
      <thead><tr><th>id</th><th>name</th><th>scope</th><th>path</th><th>purpose</th></tr></thead>
      <tbody>${framework.map((t) => html`
        <tr key=${t.TestId}><td>${t.TestId}</td><td>${t.Name}</td><td>${t.Scope}</td><td>${t.FilePath}</td><td>${t.Purpose}</td></tr>
      `)}</tbody>
    </table>
  `;
}

// ---------------------------------------------------------------------------
// Input spokes
// ---------------------------------------------------------------------------
function SpokesScreen({ screen, rulebook }) {
  const spokes = rulebook.RulebookSourceSpokes?.data || [];
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="cards">
      ${spokes.map((s) => html`
        <div class="card" key=${s.SpokeId}>
          <h3>${s.Kind} · ${s.Direction}</h3>
          <div class="big" style=${{ fontSize: 16 }}>${s.Name}</div>
          <div class="sub">${s.Purpose}</div>
          <div class="muted small" style=${{ marginTop: 8 }}>${s.Authority || ""}</div>
        </div>
      `)}
    </div>
  `;
}

// ---------------------------------------------------------------------------
// Users (write-through)
// ---------------------------------------------------------------------------
function UsersScreen({ screen, isDev, reload }) {
  const [data, setData] = useState({ users: [], roles: [] });
  const [form, setForm] = useState({ userId: "", email: "", displayName: "", roleId: "role-viewer" });
  const load = () => api.get("/api/users").then(setData);
  useEffect(() => { load(); }, []);
  const add = async () => {
    try {
      await api.post("/api/users", form);
      toast("User added (Postgres + rulebook JSON)");
      setForm({ userId: "", email: "", displayName: "", roleId: "role-viewer" });
      load(); reload();
    } catch (e) { toast(e.message, "error"); }
  };
  return html`
    <${ScreenHeader} screen=${screen} />
    <table class="grid">
      <thead><tr><th>UserId</th><th>Email</th><th>DisplayName</th><th>Role</th><th>Default</th><th>Notes</th></tr></thead>
      <tbody>${data.users.map((u) => {
        const r = data.roles.find((x) => x.RoleId === u.RoleId);
        return html`
          <tr key=${u.UserId}>
            <td>${u.UserId}</td><td>${u.Email}</td><td>${u.DisplayName}</td>
            <td><span class=${"pill " + (r?.Name?.toLowerCase() || "")}>${r?.Name || u.RoleId}</span></td>
            <td>${u.IsDefault ? "✓" : ""}</td><td>${u.Notes || ""}</td>
          </tr>`;
      })}</tbody>
    </table>
    ${isDev ? html`
      <h4 style=${{ marginTop: 20 }}>Add user</h4>
      <div class="cards" style=${{ gridTemplateColumns: "1fr 1fr 1fr 1fr" }}>
        <input placeholder="UserId" value=${form.userId} onChange=${(e) => setForm({ ...form, userId: e.target.value })} />
        <input placeholder="email@…" value=${form.email} onChange=${(e) => setForm({ ...form, email: e.target.value })} />
        <input placeholder="Display name" value=${form.displayName} onChange=${(e) => setForm({ ...form, displayName: e.target.value })} />
        <select value=${form.roleId} onChange=${(e) => setForm({ ...form, roleId: e.target.value })}>
          ${data.roles.map((r) => html`<option key=${r.RoleId} value=${r.RoleId}>${r.Name}</option>`)}
        </select>
      </div>
      <div style=${{ marginTop: 8 }}><button class="btn" onClick=${add}>Add user (write-through)</button></div>
    ` : null}
  `;
}

// ---------------------------------------------------------------------------
// Tech tools
// ---------------------------------------------------------------------------
function TechPostgresScreen({ screen }) {
  const [tables, setTables] = useState([]);
  const [sql, setSql] = useState("SELECT entity_name, jsonb_array_length(data_json) AS rows FROM portal_rulebook_entities ORDER BY 1;");
  const [result, setResult] = useState(null);
  useEffect(() => { api.get("/api/tech/postgres/tables").then((r) => setTables(r.tables || [])); }, []);
  const run = async () => {
    try { setResult(await api.post("/api/tech/postgres/query", { sql })); }
    catch (e) { toast(e.message, "error"); }
  };
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="muted small" style=${{ marginBottom: 8 }}>Editor DB tables: ${tables.length}</div>
    <table class="grid"><thead><tr><th>schema</th><th>table</th></tr></thead>
      <tbody>${tables.map((t, i) => html`<tr key=${i}><td>${t.table_schema}</td><td>${t.table_name}</td></tr>`)}</tbody>
    </table>
    <h4 style=${{ marginTop: 20 }}>SQL</h4>
    <textarea rows="4" value=${sql} onChange=${(e) => setSql(e.target.value)} />
    <div style=${{ marginTop: 8 }}><button class="btn" onClick=${run}>Run query</button></div>
    ${result ? html`
      <h4>Result (${result.rowCount} rows)</h4>
      ${result.rows?.length ? html`
        <table class="grid">
          <thead><tr>${result.fields.map((f) => html`<th key=${f}>${f}</th>`)}</tr></thead>
          <tbody>${result.rows.map((r, i) => html`<tr key=${i}>${result.fields.map((f) => html`<td key=${f}>${formatCell(r[f])}</td>`)}</tr>`)}</tbody>
        </table>
      ` : html`<div class="muted">No rows.</div>`}
    ` : null}
  `;
}
function TechProxyScreen({ screen }) {
  const [status, setStatus] = useState(null);
  useEffect(() => { api.get("/api/tech/proxy/status").then(setStatus).catch((e) => setStatus({ error: e.message })); }, []);
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="editor">${JSON.stringify(status, null, 2)}</div>
  `;
}
function TechFilesScreen({ screen }) {
  return html`
    <${ScreenHeader} screen=${screen} />
    <p class="muted">Filesystem browser is intentionally read-only; consult Substrates → file tree for generated outputs.</p>
  `;
}
function TechJsonScreen({ screen }) {
  const [text, setText] = useState("");
  const load = () => fetch("/api/tech/rulebook-json").then(r => r.text()).then(setText);
  useEffect(() => { load(); }, []);
  const save = async () => {
    try {
      JSON.parse(text);
      await fetch("/api/tech/rulebook-json", { method: "PUT", headers: { "Content-Type": "text/plain" }, body: text });
      toast("Saved rulebook JSON (and rebooted editor DB).");
    } catch (e) { toast("Invalid JSON or save failed: " + e.message, "error"); }
  };
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="flex" style=${{ marginBottom: 8 }}>
      <button class="btn secondary" onClick=${load}>Reload from disk</button>
      <button class="btn" onClick=${save}>Save</button>
    </div>
    <textarea rows="30" value=${text} onChange=${(e) => setText(e.target.value)} />
  `;
}
function TechResetScreen({ screen }) {
  const [busy, setBusy] = useState(false);
  const reset = async () => {
    if (!confirm("Drop the editor Postgres DB and rebuild from rulebook JSON?")) return;
    setBusy(true);
    try {
      await api.post("/api/tech/reset");
      toast("Editor DB rebuilt from rulebook JSON.");
    } catch (e) { toast(e.message, "error"); }
    setBusy(false);
  };
  return html`
    <${ScreenHeader} screen=${screen} />
    <div class="cards" style=${{ gridTemplateColumns: "1fr" }}>
      <div class="card">
        <h3>Safe operation</h3>
        <p>This drops the per-project editor Postgres database and re-bootstraps it from the rulebook JSON on disk. The rulebook JSON is the durable SSoT, so no business data is lost.</p>
        <button class="btn danger" disabled=${busy} onClick=${reset}>${busy ? "Resetting…" : "Reset editor DB now"}</button>
      </div>
    </div>
  `;
}

// ---------------------------------------------------------------------------
// Boot
// ---------------------------------------------------------------------------
createRoot(document.getElementById("root")).render(html`<${App} />`);
