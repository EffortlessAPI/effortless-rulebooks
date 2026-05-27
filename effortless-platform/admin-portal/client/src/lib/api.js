const base = async (method, path, body, asText = false) => {
  const opts = {
    method,
    headers: body != null ? { "Content-Type": asText ? "text/plain" : "application/json" } : {},
    body: body != null ? (asText ? body : JSON.stringify(body)) : undefined,
  };
  const r = await fetch(path, opts);
  if (!r.ok) {
    const text = await r.text().catch(() => r.statusText);
    // The server returns JSON errors of shape { error, ...extras } (e.g.
    // { error, referrers } for cascade-required deletes). Surface the
    // human-readable `error` as the Error message and hoist extras to
    // top-level properties so callers can branch on them (e.g.
    // `if (e.referrers) ...`).
    let parsed = null;
    try { parsed = JSON.parse(text); } catch {}
    const msg = (parsed && parsed.error) || text || `${r.status} ${path}`;
    const err = new Error(msg);
    err.status = r.status;
    if (parsed && typeof parsed === "object") {
      for (const [k, v] of Object.entries(parsed)) {
        if (k !== "error" && !(k in err)) err[k] = v;
      }
    }
    throw err;
  }
  if (asText) return r.text();
  return r.json();
};

export const api = {
  get:   (path)             => base("GET",    path),
  post:  (path, body)       => base("POST",   path, body),
  patch: (path, body)       => base("PATCH",  path, body),
  put:   (path, body, txt)  => base("PUT",    path, body, txt),
  del:   (path)             => base("DELETE", path),
};
