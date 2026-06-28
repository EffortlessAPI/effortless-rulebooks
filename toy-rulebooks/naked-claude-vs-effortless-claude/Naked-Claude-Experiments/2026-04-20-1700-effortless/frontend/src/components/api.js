const BASE = '/api';

async function handle(res) {
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: res.statusText }));
    throw new Error(err.error || 'Request failed');
  }
  return res.json();
}

// ── Clients ────────────────────────────────────────────────────────────────

export async function getClients() {
  return handle(await fetch(`${BASE}/clients`));
}

export async function getClient(id) {
  return handle(await fetch(`${BASE}/clients/${encodeURIComponent(id)}`));
}

export async function createClient(data) {
  return handle(await fetch(`${BASE}/clients`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateClient(id, data) {
  return handle(await fetch(`${BASE}/clients/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteClient(id) {
  return handle(await fetch(`${BASE}/clients/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Client Categories ──────────────────────────────────────────────────────

export async function getClientCategories() {
  return handle(await fetch(`${BASE}/client-categories`));
}

export async function createClientCategory(data) {
  return handle(await fetch(`${BASE}/client-categories`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateClientCategory(id, data) {
  return handle(await fetch(`${BASE}/client-categories/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteClientCategory(id) {
  return handle(await fetch(`${BASE}/client-categories/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Statuses ───────────────────────────────────────────────────────────────

export async function getStatuses() {
  return handle(await fetch(`${BASE}/statuses`));
}

export async function getStatus(id) {
  return handle(await fetch(`${BASE}/statuses/${encodeURIComponent(id)}`));
}

export async function createStatus(data) {
  return handle(await fetch(`${BASE}/statuses`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateStatus(id, data) {
  return handle(await fetch(`${BASE}/statuses/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteStatus(id) {
  return handle(await fetch(`${BASE}/statuses/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Products ───────────────────────────────────────────────────────────────

export async function getProducts() {
  return handle(await fetch(`${BASE}/products`));
}

export async function getProduct(id) {
  return handle(await fetch(`${BASE}/products/${encodeURIComponent(id)}`));
}

export async function createProduct(data) {
  return handle(await fetch(`${BASE}/products`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateProduct(id, data) {
  return handle(await fetch(`${BASE}/products/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteProduct(id) {
  return handle(await fetch(`${BASE}/products/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Invoices ───────────────────────────────────────────────────────────────

export async function getInvoices() {
  return handle(await fetch(`${BASE}/invoices`));
}

export async function getInvoice(id) {
  return handle(await fetch(`${BASE}/invoices/${encodeURIComponent(id)}`));
}

export async function createInvoice(data) {
  return handle(await fetch(`${BASE}/invoices`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateInvoice(id, data) {
  return handle(await fetch(`${BASE}/invoices/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteInvoice(id) {
  return handle(await fetch(`${BASE}/invoices/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Line Items ─────────────────────────────────────────────────────────────

export async function createLineItem(invoiceId, data) {
  return handle(await fetch(`${BASE}/invoices/${encodeURIComponent(invoiceId)}/line-items`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateLineItem(id, data) {
  return handle(await fetch(`${BASE}/line-items/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteLineItem(id) {
  return handle(await fetch(`${BASE}/line-items/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Payments ───────────────────────────────────────────────────────────────

export async function getPayments() {
  return handle(await fetch(`${BASE}/payments`));
}

export async function createPayment(invoiceId, data) {
  return handle(await fetch(`${BASE}/invoices/${encodeURIComponent(invoiceId)}/payments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updatePayment(id, data) {
  return handle(await fetch(`${BASE}/payments/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deletePayment(id) {
  return handle(await fetch(`${BASE}/payments/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Inventory Adjustments ──────────────────────────────────────────────────

export async function getInventoryAdjustments(productId) {
  const url = productId
    ? `${BASE}/inventory-adjustments?product=${encodeURIComponent(productId)}`
    : `${BASE}/inventory-adjustments`;
  return handle(await fetch(url));
}

export async function createInventoryAdjustment(data) {
  return handle(await fetch(`${BASE}/inventory-adjustments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateInventoryAdjustment(id, data) {
  return handle(await fetch(`${BASE}/inventory-adjustments/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteInventoryAdjustment(id) {
  return handle(await fetch(`${BASE}/inventory-adjustments/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── App Users ──────────────────────────────────────────────────────────────

export async function getAppUsers() {
  return handle(await fetch(`${BASE}/app-users`));
}

export async function getAppUser(id) {
  return handle(await fetch(`${BASE}/app-users/${encodeURIComponent(id)}`));
}

export async function createAppUser(data) {
  return handle(await fetch(`${BASE}/app-users`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateAppUser(id, data) {
  return handle(await fetch(`${BASE}/app-users/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteAppUser(id) {
  return handle(await fetch(`${BASE}/app-users/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Client Approvals ───────────────────────────────────────────────────────

export async function getApprovals({ status, client } = {}) {
  const params = new URLSearchParams();
  if (status) params.set('status', status);
  if (client) params.set('client', client);
  const qs = params.toString();
  return handle(await fetch(`${BASE}/approvals${qs ? '?' + qs : ''}`));
}

export async function createApproval(data) {
  return handle(await fetch(`${BASE}/approvals`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function updateApproval(id, data) {
  return handle(await fetch(`${BASE}/approvals/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  }));
}

export async function deleteApproval(id) {
  return handle(await fetch(`${BASE}/approvals/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  }));
}

// ── Summary ────────────────────────────────────────────────────────────────

export async function getSummary() {
  return handle(await fetch(`${BASE}/summary`));
}
