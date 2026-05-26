const email = typeof window !== 'undefined' ? localStorage.getItem('email') : null;

export async function api(url: string, options: RequestInit = {}) {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  if (email) {
    headers['X-User-Email'] = email;
  }

  return fetch(url, {
    ...options,
    headers,
  });
}
