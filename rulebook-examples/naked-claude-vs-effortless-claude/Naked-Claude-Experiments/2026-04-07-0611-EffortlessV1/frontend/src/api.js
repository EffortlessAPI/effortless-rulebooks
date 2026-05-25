const BASE = '/api';

async function j(res) {
  if (!res.ok) throw new Error((await res.text()) || res.statusText);
  if (res.status === 204) return null;
  return res.json();
}

export const api = {
  list:    ()        => fetch(`${BASE}/customers`).then(j),
  summary: ()        => fetch(`${BASE}/summary`).then(j),
  get:     (id)      => fetch(`${BASE}/customers/${id}`).then(j),
  create:  (body)    => fetch(`${BASE}/customers`,       { method: 'POST',   headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) }).then(j),
  update:  (id,body) => fetch(`${BASE}/customers/${id}`, { method: 'PUT',    headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) }).then(j),
  remove:  (id)      => fetch(`${BASE}/customers/${id}`, { method: 'DELETE' }).then(j),
};
