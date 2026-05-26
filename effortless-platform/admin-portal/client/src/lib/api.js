const base = async (method, path, body, asText = false) => {
  const opts = {
    method,
    headers: body != null ? { "Content-Type": asText ? "text/plain" : "application/json" } : {},
    body: body != null ? (asText ? body : JSON.stringify(body)) : undefined,
  };
  const r = await fetch(path, opts);
  if (!r.ok) {
    const msg = await r.text().catch(() => r.statusText);
    throw new Error(msg || `${r.status} ${path}`);
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
