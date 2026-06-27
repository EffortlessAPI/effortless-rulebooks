const BASE = '/api';

async function apiFetch(url, opts = {}) {
  const res = await fetch(BASE + url, {
    headers: opts.body ? { 'Content-Type': 'application/json' } : {},
    ...opts,
    body: opts.body ? JSON.stringify(opts.body) : undefined,
  });
  if (res.status === 204) return null;
  const data = await res.json().catch(() => ({ error: 'Unknown error' }));
  if (!res.ok) throw new Error(data.error || `HTTP ${res.status}`);
  return data;
}

// Summary
export const fetchSummary = () => apiFetch('/summary');

// Clients
export const fetchClients  = ()      => apiFetch('/clients');
export const fetchClient   = (id)    => apiFetch(`/clients/${id}`);
export const createClient  = (data)  => apiFetch('/clients',      { method: 'POST',   body: data });
export const updateClient  = (id, d) => apiFetch(`/clients/${id}`, { method: 'PUT',    body: d });
export const deleteClient  = (id)    => apiFetch(`/clients/${id}`, { method: 'DELETE' });

// Client Categories
export const fetchClientCategories = ()      => apiFetch('/client-categories');
export const fetchClientCategory   = (id)    => apiFetch(`/client-categories/${id}`);
export const createClientCategory  = (data)  => apiFetch('/client-categories',        { method: 'POST',   body: data });
export const updateClientCategory  = (id, d) => apiFetch(`/client-categories/${id}`,  { method: 'PUT',    body: d });
export const deleteClientCategory  = (id)    => apiFetch(`/client-categories/${id}`,  { method: 'DELETE' });

// Statuses
export const fetchStatuses   = ()      => apiFetch('/statuses');
export const fetchStatus     = (id)    => apiFetch(`/statuses/${id}`);
export const createStatus    = (data)  => apiFetch('/statuses',       { method: 'POST',   body: data });
export const updateStatus    = (id, d) => apiFetch(`/statuses/${id}`,  { method: 'PUT',    body: d });
export const deleteStatus    = (id)    => apiFetch(`/statuses/${id}`,  { method: 'DELETE' });

// Products
export const fetchProducts   = ()      => apiFetch('/products');
export const fetchProduct    = (id)    => apiFetch(`/products/${id}`);
export const createProduct   = (data)  => apiFetch('/products',       { method: 'POST',   body: data });
export const updateProduct   = (id, d) => apiFetch(`/products/${id}`,  { method: 'PUT',    body: d });
export const deleteProduct   = (id)    => apiFetch(`/products/${id}`,  { method: 'DELETE' });

// Invoices
export const fetchInvoices   = ()      => apiFetch('/invoices');
export const fetchInvoice    = (id)    => apiFetch(`/invoices/${id}`);
export const createInvoice   = (data)  => apiFetch('/invoices',       { method: 'POST',   body: data });
export const updateInvoice   = (id, d) => apiFetch(`/invoices/${id}`, { method: 'PUT',    body: d });
export const deleteInvoice   = (id)    => apiFetch(`/invoices/${id}`, { method: 'DELETE' });

// Invoice Line Items
export const fetchInvoiceLineItems  = (invoiceId) => apiFetch(`/invoice-line-items${invoiceId ? `?invoice_id=${invoiceId}` : ''}`);
export const createInvoiceLineItem  = (data)      => apiFetch('/invoice-line-items',        { method: 'POST',   body: data });
export const updateInvoiceLineItem  = (id, d)     => apiFetch(`/invoice-line-items/${id}`,  { method: 'PUT',    body: d });
export const deleteInvoiceLineItem  = (id)        => apiFetch(`/invoice-line-items/${id}`,  { method: 'DELETE' });

// Payments
export const fetchPayments   = (invoiceId) => apiFetch(`/payments${invoiceId ? `?invoice_id=${invoiceId}` : ''}`);
export const createPayment   = (data)      => apiFetch('/payments',       { method: 'POST',   body: data });
export const updatePayment   = (id, d)     => apiFetch(`/payments/${id}`, { method: 'PUT',    body: d });
export const deletePayment   = (id)        => apiFetch(`/payments/${id}`, { method: 'DELETE' });

// App Users
export const fetchAppUsers = ()      => apiFetch('/app-users');
export const fetchAppUser  = (id)    => apiFetch(`/app-users/${id}`);
export const createAppUser = (data)  => apiFetch('/app-users',       { method: 'POST',   body: data });
export const updateAppUser = (id, d) => apiFetch(`/app-users/${id}`, { method: 'PUT',    body: d });
export const deleteAppUser = (id)    => apiFetch(`/app-users/${id}`, { method: 'DELETE' });

// Client Approvals
export const fetchClientApprovals = (clientId) =>
  apiFetch(`/client-approvals${clientId ? `?client_id=${clientId}` : ''}`);
export const fetchClientApproval  = (id)    => apiFetch(`/client-approvals/${id}`);
export const createClientApproval = (data)  => apiFetch('/client-approvals',       { method: 'POST',   body: data });
export const updateClientApproval = (id, d) => apiFetch(`/client-approvals/${id}`, { method: 'PUT',    body: d });
export const deleteClientApproval = (id)    => apiFetch(`/client-approvals/${id}`, { method: 'DELETE' });

// Inventory Adjustments
export const fetchInventoryAdjustments = (productId) =>
  apiFetch(`/inventory-adjustments${productId ? `?product_id=${productId}` : ''}`);
export const fetchInventoryAdjustment  = (id)    => apiFetch(`/inventory-adjustments/${id}`);
export const createInventoryAdjustment = (data)  => apiFetch('/inventory-adjustments',       { method: 'POST',   body: data });
export const updateInventoryAdjustment = (id, d) => apiFetch(`/inventory-adjustments/${id}`, { method: 'PUT',    body: d });
export const deleteInventoryAdjustment = (id)    => apiFetch(`/inventory-adjustments/${id}`, { method: 'DELETE' });
