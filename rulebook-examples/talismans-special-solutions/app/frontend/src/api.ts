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

import type {
  Story,
  TriangleResponse,
  DiffResponse,
  ConformanceResponse,
  Scenario,
  StoreId,
  BackendDescriptor,
  ControlEvent,
} from "./types";

const BACKEND_KEY = "erb.backend";

// Read/write the chosen backend (a pair id like "reasoner"/"postgres", or an
// explicit "rules:data" cross-run for the easter-egg 2x2). Defaults to the
// reasoner so a fresh load before login still works.
export function getBackend(): string {
  try {
    return localStorage.getItem(BACKEND_KEY) || "reasoner";
  } catch {
    return "reasoner";
  }
}
export function setBackend(b: string): void {
  try {
    localStorage.setItem(BACKEND_KEY, b);
  } catch {
    /* non-fatal: header just falls back to the default */
  }
}

// fetch() with the backend header attached. All calls below route through this.
function bfetch(url: string, opts: RequestInit = {}): Promise<Response> {
  const headers = { ...(opts.headers || {}), "X-ERB-Backend": getBackend() };
  return fetch(url, { ...opts, headers });
}

async function jsonOrThrow<T = unknown>(res: Response): Promise<T> {
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
  // The login page reads these without caring about the active backend.
  backends: (): Promise<{ default: string; backends: BackendDescriptor[] }> =>
    fetch("/api/backends").then((r) => jsonOrThrow(r)),
  controlActions: (): Promise<unknown> => fetch("/api/control/actions").then((r) => jsonOrThrow(r)),
  // Live drift state across the three raw stores (rulebook head + two engine
  // legs). Computed server-side from canonical hashes; see /api/triangle.
  triangle: (): Promise<TriangleResponse> => fetch("/api/triangle").then((r) => jsonOrThrow<TriangleResponse>(r)),
  // Field-level diff: pick a HEAD store; get every field the other two stores
  // differ on, shown as other-value → HEAD-value. See /api/diff.
  diff: (head: StoreId): Promise<DiffResponse> =>
    fetch(`/api/diff?head=${encodeURIComponent(head)}`).then((r) => jsonOrThrow<DiffResponse>(r)),

  health: (): Promise<unknown> => bfetch("/api/health").then((r) => jsonOrThrow(r)),
  story: (): Promise<Story> => bfetch("/api/story").then((r) => jsonOrThrow<Story>(r)),
  reasoned: (): Promise<unknown> => bfetch("/api/reasoned").then((r) => jsonOrThrow(r)),
  rawDb: (): Promise<unknown> => bfetch("/api/db").then((r) => jsonOrThrow(r)),
  conformance: (): Promise<ConformanceResponse> =>
    bfetch("/api/conformance").then((r) => jsonOrThrow<ConformanceResponse>(r)),
  addRow: (cls: string, row: Record<string, unknown>): Promise<unknown> =>
    bfetch(`/api/individuals/${cls}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(row),
    }).then((r) => jsonOrThrow(r)),
  patchRow: (cls: string, id: string, patch: Record<string, unknown>): Promise<unknown> =>
    bfetch(`/api/individuals/${cls}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(patch),
    }).then((r) => jsonOrThrow(r)),
  deleteRow: (cls: string, id: string): Promise<unknown> =>
    bfetch(`/api/individuals/${cls}/${id}`, { method: "DELETE" }).then((r) => jsonOrThrow(r)),
  scenarios: (): Promise<Scenario[]> => bfetch("/api/scenarios").then((r) => jsonOrThrow<Scenario[]>(r)),
  applyScenario: (name: string): Promise<unknown> =>
    bfetch(`/api/scenario/${name}`, { method: "POST" }).then((r) => jsonOrThrow(r)),
};

// Export the CURRENT live DB state as an Excel workbook (server reads vw_* views,
// runs rulebook-to-xlsx, streams the file). Fetches with the backend header so
// the export reflects the active store, then triggers a browser blob download.
// Surfaces a thrown error verbatim (no silent fallback) for the caller to show.
export async function exportXlsx(): Promise<void> {
  const res = await bfetch("/api/export/xlsx");
  if (!res.ok) {
    let detail = "";
    try {
      detail = (await res.json()).error || "";
    } catch {
      detail = await res.text().catch(() => "");
    }
    throw new Error(`Excel export failed: ${res.status} ${res.statusText}${detail ? ` — ${detail}` : ""}`);
  }
  const blob = await res.blob();
  // Pull the filename from Content-Disposition if present; else a sensible default.
  const cd = res.headers.get("Content-Disposition") || "";
  const m = cd.match(/filename="?([^"]+)"?/);
  const filename = m ? m[1] : "talismans-special-solutions-rulebook.xlsx";
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

// Run a control action (POST) and stream its SSE-formatted body line-by-line.
// `onEvent({type, ...})` is called for each parsed event; resolves when the
// stream ends. Control endpoints are POST (state-mutating), so we can't use the
// browser EventSource (GET-only) — we read the response body as a stream and
// parse the `data: {...}` frames ourselves.
export async function runControl(
  actionPath: string,
  onEvent: (event: ControlEvent) => void,
  body: unknown = null,
): Promise<void> {
  const res = await fetch(actionPath, {
    method: "POST",
    ...(body ? { headers: { "Content-Type": "application/json" }, body: JSON.stringify(body) } : {}),
  });
  if (!res.ok && !res.body) {
    const detail = await res.text().catch(() => "");
    throw new Error(`${res.status} ${res.statusText}${detail ? ` — ${detail}` : ""}`);
  }
  const reader = res.body!.getReader();
  const decoder = new TextDecoder();
  let buf = "";
  for (;;) {
    const { value, done } = await reader.read();
    if (done) break;
    buf += decoder.decode(value, { stream: true });
    // SSE frames are separated by a blank line; each frame has `data: <json>`.
    let idx: number;
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
export function splitRawDerived(
  reasonedRow: Record<string, unknown>,
  rawRow: Record<string, unknown> | null | undefined,
): { raw: Record<string, unknown>; derived: Record<string, unknown> } {
  const rawKeys = new Set(rawRow ? Object.keys(rawRow) : []);
  const raw: Record<string, unknown> = {};
  const derived: Record<string, unknown> = {};
  for (const [k, v] of Object.entries(reasonedRow)) {
    if (k.startsWith("_")) continue; // _iri / _classes are bookkeeping
    if (rawKeys.has(k)) raw[k] = v;
    else derived[k] = v;
  }
  return { raw, derived };
}

export function pkOf(row: Record<string, unknown>): string | undefined {
  for (const [k, v] of Object.entries(row)) {
    if (k.endsWith("Id") && typeof v === "string") return v;
  }
  return row._iri as string | undefined;
}
