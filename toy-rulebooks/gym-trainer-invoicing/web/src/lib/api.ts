const EMAIL_KEY = "gti.email";

export function getEmail(): string | null {
  return localStorage.getItem(EMAIL_KEY);
}
export function setEmail(email: string) {
  localStorage.setItem(EMAIL_KEY, email);
}
export function clearEmail() {
  localStorage.removeItem(EMAIL_KEY);
}

export async function api<T = any>(path: string, init?: RequestInit): Promise<T> {
  const email = getEmail();
  const headers = new Headers(init?.headers);
  headers.set("Content-Type", "application/json");
  if (email) headers.set("X-User-Email", email);
  const res = await fetch(path, { ...init, headers });
  if (!res.ok) {
    let msg = `${res.status} ${res.statusText}`;
    try {
      const b = await res.json();
      if (b?.error) msg = b.error;
    } catch {}
    throw new Error(msg);
  }
  if (res.status === 204) return undefined as T;
  return res.json();
}
