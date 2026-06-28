async function req(path, opts = {}) {
  const res = await fetch(path, {
    headers: { 'Content-Type': 'application/json' },
    ...opts
  });
  if (res.status === 204) return null;
  const data = await res.json();
  if (!res.ok) throw new Error(data.error || 'request failed');
  return data;
}

export const api = {
  list: () => req('/api/customers'),
  summary: () => req('/api/summary'),
  get: (id) => req(`/api/customers/${id}`),
  create: (body) => req('/api/customers', { method: 'POST', body: JSON.stringify(body) }),
  update: (id, body) => req(`/api/customers/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  remove: (id) => req(`/api/customers/${id}`, { method: 'DELETE' })
};
