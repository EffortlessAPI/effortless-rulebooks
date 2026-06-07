// Thin fetch wrapper. Every call goes to same-origin /api (Vite proxies it to
// the Express backend in dev; Express serves both in prod). The frontend NEVER
// computes a derived field — it only renders what the chosen engine returns.
// That is the point: the engine already ran; we open the door and walk in.
//
// The ONE thing this wrapper adds for the backend toggle: it attaches the
// user's chosen execution substrate as the X-ERB-Backend header on every call.
// The login page stores the choice; the entire rest of the UI (App.jsx, the
// views) is UNCHANGED and unaware — it just renders whatever the active engine
// computed. Swapping the engine is therefore a header change, nothing more.

const BACKEND_KEY = "erb.backend";

// Read/write the chosen backend (a pair id like "reasoner"/"postgres", or an
// explicit "rules:data" cross-run for the easter-egg 2x2). Defaults to the
// reasoner so a fresh load before login still works.
export function getBackend() {
  try {
    return localStorage.getItem(BACKEND_KEY) || "reasoner";
  } catch {
    return "reasoner";
  }
}
export function setBackend(b) {
  try {
    localStorage.setItem(BACKEND_KEY, b);
  } catch {
    /* non-fatal: header just falls back to the default */
  }
}

// fetch() with the backend header attached. All calls below route through this.
function bfetch(url, opts = {}) {
  const headers = { ...(opts.headers || {}), "X-ERB-Backend": getBackend() };
  return fetch(url, { ...opts, headers });
}

async function jsonOrThrow(res) {
  if (!res.ok) {
    let detail = "";
    try {
      detail = (await res.json()).error || "";
    } catch {
      detail = await res.text();
    }
    throw new Error(`${res.status} ${res.statusText}${detail ? ` — ${detail}` : ""}`);
  }
  return res.json();
}

export const api = {
  // The login page reads these two without caring about the active backend.
  backends: () => fetch("/api/backends").then(jsonOrThrow),
  controlActions: () => fetch("/api/control/actions").then(jsonOrThrow),

  health: () => bfetch("/api/health").then(jsonOrThrow),
  story: () => bfetch("/api/story").then(jsonOrThrow),
  reasoned: () => bfetch("/api/reasoned").then(jsonOrThrow),
  rawDb: () => bfetch("/api/db").then(jsonOrThrow),
  conformance: () => bfetch("/api/conformance").then(jsonOrThrow),
  addRow: (cls, row) =>
    bfetch(`/api/individuals/${cls}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(row),
    }).then(jsonOrThrow),
  patchRow: (cls, id, patch) =>
    bfetch(`/api/individuals/${cls}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(patch),
    }).then(jsonOrThrow),
  deleteRow: (cls, id) =>
    bfetch(`/api/individuals/${cls}/${id}`, { method: "DELETE" }).then(jsonOrThrow),
  scenarios: () => bfetch("/api/scenarios").then(jsonOrThrow),
  applyScenario: (name) =>
    bfetch(`/api/scenario/${name}`, { method: "POST" }).then(jsonOrThrow),
};

// Run a control action (POST) and stream its SSE-formatted body line-by-line.
// `onEvent({type, ...})` is called for each parsed event; resolves when the
// stream ends. Control endpoints are POST (state-mutating), so we can't use the
// browser EventSource (GET-only) — we read the response body as a stream and
// parse the `data: {...}` frames ourselves.
export async function runControl(actionPath, onEvent) {
  const res = await fetch(actionPath, { method: "POST" });
  if (!res.ok && !res.body) {
    const detail = await res.text().catch(() => "");
    throw new Error(`${res.status} ${res.statusText}${detail ? ` — ${detail}` : ""}`);
  }
  const reader = res.body.getReader();
  const decoder = new TextDecoder();
  let buf = "";
  for (;;) {
    const { value, done } = await reader.read();
    if (done) break;
    buf += decoder.decode(value, { stream: true });
    // SSE frames are separated by a blank line; each frame has `data: <json>`.
    let idx;
    while ((idx = buf.indexOf("\n\n")) >= 0) {
      const frame = buf.slice(0, idx);
      buf = buf.slice(idx + 2);
      for (const line of frame.split(/\r?\n/)) {
        const m = line.match(/^data:\s?(.*)$/);
        if (m) {
          try {
            onEvent(JSON.parse(m[1]));
          } catch {
            /* a non-JSON keepalive comment; ignore */
          }
        }
      }
    }
  }
}

// Which keys on a reasoned individual are RAW (came straight from db.json) vs.
// DERIVED (added by SHACL/OWL-RL). We compute this per-class by diffing the
// reasoned row against the raw db row with the same primary key.
export function splitRawDerived(reasonedRow, rawRow) {
  const rawKeys = new Set(rawRow ? Object.keys(rawRow) : []);
  const raw = {};
  const derived = {};
  for (const [k, v] of Object.entries(reasonedRow)) {
    if (k.startsWith("_")) continue; // _iri / _classes are bookkeeping
    if (rawKeys.has(k)) raw[k] = v;
    else derived[k] = v;
  }
  return { raw, derived };
}

export function pkOf(row) {
  for (const [k, v] of Object.entries(row)) {
    if (k.endsWith("Id") && typeof v === "string") return v;
  }
  return row._iri;
}
