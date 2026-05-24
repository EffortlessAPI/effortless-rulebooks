const KEY = "demo:email";

export function getEmail(): string | null {
  return localStorage.getItem(KEY);
}
export function setEmail(email: string | null) {
  if (email) localStorage.setItem(KEY, email);
  else localStorage.removeItem(KEY);
}

export async function api<T = any>(
  path: string,
  init: RequestInit = {}
): Promise<T> {
  const headers = new Headers(init.headers || {});
  headers.set("Content-Type", "application/json");
  const email = getEmail();
  if (email) headers.set("X-User-Email", email);
  const res = await fetch(path, { ...init, headers });
  if (!res.ok) {
    const body = await res.text();
    throw new Error(`${res.status} ${body}`);
  }
  if (res.status === 204) return undefined as T;
  return res.json();
}
