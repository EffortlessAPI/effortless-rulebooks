const KEY = 'mad.email';

export function getEmail(): string | null {
  return localStorage.getItem(KEY);
}

export function setEmail(email: string | null) {
  if (email) localStorage.setItem(KEY, email);
  else localStorage.removeItem(KEY);
}

export async function api<T = unknown>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers);
  const email = getEmail();
  if (email) headers.set('X-User-Email', email);
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');
  const res = await fetch(`/api${path.startsWith('/') ? path : '/' + path}`, {
    ...init,
    headers,
  });
  if (res.status === 401) {
    const err: Error & { status?: number } = new Error('unauthorized');
    err.status = 401;
    throw err;
  }
  if (!res.ok) {
    let body: unknown;
    try {
      body = await res.json();
    } catch {
      body = await res.text();
    }
    const err: Error & { status?: number; body?: unknown } = new Error(`API ${res.status}`);
    err.status = res.status;
    err.body = body;
    throw err;
  }
  return res.json();
}
