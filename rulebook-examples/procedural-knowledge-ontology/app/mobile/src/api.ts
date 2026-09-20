// Every read is `SELECT * FROM <my schema>.<table>` with equality filters, run AS the signed-in
// principal. There is no formula evaluation, lookup resolution or counting in this app: if a value
// is on screen it is a column of a view. If it looks wrong, the fix is in the rulebook.
export type Row = Record<string, any>;

const TOKEN_KEY = "pko.mobile.token";
export const getToken = () => localStorage.getItem(TOKEN_KEY);
export const setToken = (t: string | null) => (t ? localStorage.setItem(TOKEN_KEY, t) : localStorage.removeItem(TOKEN_KEY));

export class ApiError extends Error {
  constructor(public status: number, public code: string, message: string) { super(message); }
}

async function call<T>(path: string, init: RequestInit = {}): Promise<T> {
  const token = getToken();
  const res = await fetch(path, {
    ...init,
    headers: { "content-type": "application/json", ...(token ? { authorization: `Bearer ${token}` } : {}), ...(init.headers || {}) },
  });
  const body = await res.json().catch(() => ({}));
  if (!res.ok) throw new ApiError(res.status, body.error || "error", body.detail || body.error || res.statusText);
  return body as T;
}

export type SignIn = {
  appUserId: string; principalId: string; displayName: string; organization: string; agentKind: string;
  principalLabel: string; domainRole: string; isAdministrator: boolean; responsibility: string | null;
};
export const signIns = () => call<{ signIns: SignIn[] }>("/api/auth/sign-ins").then((r) => r.signIns);
export const signIn = (appUserId: string, principalId: string) =>
  call<{ token: string }>("/api/auth/sign-in", { method: "POST", body: JSON.stringify({ appUserId, principalId }) });

export type ActionField = { field_name: string; field_label: string; input_kind: string; fixed_value: string; choices_from: string; datatype: string };
export type Action = { app_action_id: string; label: string; description: string; target_table: string; operation: string; watched_field: string; story_episode: number; fields: ActionField[] };
export type Shell = {
  claims: { name: string; agent: string; role: string; organization: string; principal: string; principal_label: string; is_admin: boolean };
  profile: Row | null; actions: Action[]; context: { as_of_instant: string; label: string; evaluation_context_id: string }; tables: string[];
};
export const shell = () => call<Shell>("/api/app/shell");

export function rows(table: string, filters: Record<string, string | boolean | number> = {}, order?: string) {
  const q = new URLSearchParams();
  Object.entries(filters).forEach(([k, v]) => q.set(k, String(v)));
  if (order) q.set("order", order);
  const qs = q.toString();
  return call<{ rows: Row[] }>(`/api/app/rows/${table}${qs ? `?${qs}` : ""}`).then((r) => r.rows);
}
/** Rows plus the name of the key column (role views list columns alphabetically, so it is never "the first"). */
export const keyedRows = (table: string) => call<{ rows: Row[]; pk: string }>(`/api/app/rows/${table}`);

export type Watched = { table: string; field: string; key: string; before: any; after: any; changed: boolean };
export type ActionResult = { action: string; label: string; table: string; key: string; instant: string; watched: Watched | null };
export const act = (id: string, body: { key?: string; context?: Row; values?: Row; watch?: string }) =>
  call<ActionResult>(`/api/app/action/${id}`, { method: "POST", body: JSON.stringify(body) });

// --- provenance: where one value on screen came from -------------------------
// Read from the rulebook's own field census, never assembled here. `column` is
// resolved against information_schema, so a field the catalog claims but the
// substrate does not have comes back unresolved rather than invented.
export type CatalogField = {
  rulebook_field_id: string; target_table: string; field_name: string;
  field_type: "raw" | "calculated" | "lookup" | "aggregation" | "relationship";
  datatype: string; formula: string | null; invented_for_question: string | null;
  is_derived: boolean; is_witness: boolean;
  measured_substantive_count: number; measured_distinct_value_count: number;
  has_measured_data: boolean; is_discriminating: boolean;
  disagreeing_substrate_count: number | string; is_substrate_contested: boolean;
  semantic_type_iri: string;
};
export type FormulaInput = {
  table: string; field: string; key: string; column: string; view: string; isLocal: boolean;
  fieldType: string | null; datatype: string | null; isDerived: boolean | null;
  formula: string | null; exists: boolean;
};
export type Provenance = {
  field: CatalogField; view: string; column: string | null;
  question: Row | null; loop: Row | null; role: Row | null; siblings: CatalogField[];
  table: Row | null; mappings: Row[]; inputs: FormulaInput[];
  reading: { populated: number; total: number } | null;
};
export const provenance = (table: string, column: string) =>
  call<Provenance>(`/api/app/provenance/${table}/${column}`);

export type Grounding = { table: string; key: string; why: string };
export type Answer = { question: string; text: string; topic: string; safety: boolean; next: string | null; groundings: Grounding[]; retrievalMode: string; answeredBy: string };
export const copilotQuestions = (step: string, stepExecution?: string) =>
  call<{ id: string; text: string }[]>(`/api/app/copilot/questions?step=${step}${stepExecution ? `&stepExecution=${stepExecution}` : ""}`);
export const copilotAsk = (body: { questionId: string; stepId: string; stepExecutionId?: string; machineId?: string }) =>
  call<Answer>("/api/app/copilot/ask", { method: "POST", body: JSON.stringify(body) });

export const unsaved = () => call<{ added: Row[]; updated: Row[] }>("/api/admin/story/unsaved");

/** POST to a server-sent-events endpoint and hand each line to `onLine`. */
export async function streamPost(path: string, onLine: (line: string) => void): Promise<boolean> {
  const res = await fetch(path, { method: "POST", headers: { authorization: `Bearer ${getToken()}` } });
  if (!res.ok || !res.body) throw new ApiError(res.status, "stream_failed", res.statusText);
  const reader = res.body.getReader(); const dec = new TextDecoder(); let buf = ""; let ok = false;
  for (;;) {
    const { value, done } = await reader.read(); if (done) break;
    buf += dec.decode(value, { stream: true });
    const parts = buf.split("\n\n"); buf = parts.pop() || "";
    for (const p of parts) {
      const ev = /event: (\w+)/.exec(p)?.[1]; const data = /data: (.*)/.exec(p)?.[1];
      if (!data) continue; const j = JSON.parse(data);
      if (ev === "line") onLine(j.line); if (ev === "done") ok = j.ok;
    }
  }
  return ok;
}
