const BASE = '/api';

export async function fetchSummary() {
  const r = await fetch(`${BASE}/summary`);
  if (!r.ok) throw new Error(await r.text());
  return r.json();
}

export async function fetchCustomers() {
  const r = await fetch(`${BASE}/customers`);
  if (!r.ok) throw new Error(await r.text());
  return r.json();
}

export async function fetchCustomer(id) {
  const r = await fetch(`${BASE}/customers/${encodeURIComponent(id)}`);
  if (!r.ok) throw new Error(await r.text());
  return r.json();
}

export async function createCustomer(data) {
  const r = await fetch(`${BASE}/customers`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  if (!r.ok) throw new Error(await r.text());
  return r.json();
}

export async function updateCustomer(id, data) {
  const r = await fetch(`${BASE}/customers/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  if (!r.ok) throw new Error(await r.text());
  return r.json();
}

export async function deleteCustomer(id) {
  const r = await fetch(`${BASE}/customers/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  });
  if (!r.ok) throw new Error(await r.text());
  return r.json();
}
