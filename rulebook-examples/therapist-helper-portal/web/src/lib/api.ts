const EMAIL_KEY = "thp.email";

export function getEmail(): string | null {
  return localStorage.getItem(EMAIL_KEY);
}
export function setEmail(email: string) {
  localStorage.setItem(EMAIL_KEY, email);
}
export function clearEmail() {
  localStorage.removeItem(EMAIL_KEY);
}

export async function api<T = any>(
  path: string,
  init: RequestInit = {}
): Promise<T> {
  const headers = new Headers(init.headers);
  const email = getEmail();
  if (email) headers.set("X-User-Email", email);
  if (init.body && !headers.has("Content-Type"))
    headers.set("Content-Type", "application/json");
  const res = await fetch(path, { ...init, headers });
  if (res.status === 401) {
    clearEmail();
    throw new Error("unauthenticated");
  }
  if (!res.ok) throw new Error(`${res.status} ${await res.text()}`);
  return res.json();
}
