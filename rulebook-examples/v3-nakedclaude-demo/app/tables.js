// Generic admin CRUD config: each rulebook table -> view + base table + PK + fields.
// Field types: text, textarea, number, bool, datetime, fk:<table_key>
const TABLES = [
  {
    key: 'vw_clients', label: 'Clients', table: 'clients', pk: 'client_id', idPrefix: 'client',
    fields: [
      { name: 'name', type: 'text' },
      { name: 'email', type: 'text' },
      { name: 'phone', type: 'text' },
      { name: 'company_name', type: 'text' },
      { name: 'category', type: 'fk:vw_client_categories' },
      { name: 'status', type: 'fk:vw_statuses' },
      { name: 'notes', type: 'textarea' },
      { name: 'created_at', type: 'datetime' }
    ]
  },
  {
    key: 'vw_invoices', label: 'Invoices', table: 'invoices', pk: 'invoice_id', idPrefix: 'invoice',
    fields: [
      { name: 'invoice_number', type: 'text' },
      { name: 'client', type: 'fk:vw_clients' },
      { name: 'order_date', type: 'datetime' },
      { name: 'order_status', type: 'text' },
      { name: 'shipping_address', type: 'fk:vw_addresses' },
      { name: 'billing_address', type: 'fk:vw_addresses' },
      { name: 'notes', type: 'textarea' }
    ]
  },
  {
    key: 'vw_invoice_line_items', label: 'Invoice Line Items', table: 'invoice_line_items',
    pk: 'invoice_line_item_id', idPrefix: 'invoice_line_item',
    fields: [
      { name: 'line_number', type: 'text' },
      { name: 'invoice', type: 'fk:vw_invoices' },
      { name: 'product', type: 'fk:vw_products' },
      { name: 'quantity', type: 'number' },
      { name: 'notes', type: 'textarea' }
    ]
  },
  {
    key: 'vw_payments', label: 'Payments', table: 'payments', pk: 'payment_id', idPrefix: 'payment',
    fields: [
      { name: 'payment_number', type: 'text' },
      { name: 'invoice', type: 'fk:vw_invoices' },
      { name: 'payment_date', type: 'datetime' },
      { name: 'payment_method', type: 'text' },
      { name: 'transaction_id', type: 'text' },
      { name: 'amount', type: 'number' },
      { name: 'payment_status', type: 'text' },
      { name: 'notes', type: 'textarea' }
    ]
  },
  {
    key: 'vw_products', label: 'Products', table: 'products', pk: 'product_id', idPrefix: 'product',
    fields: [
      { name: 'sku', type: 'text' },
      { name: 'display_name', type: 'text' },
      { name: 'description', type: 'textarea' },
      { name: 'unit_price', type: 'number' },
      { name: 'cost', type: 'number' },
      { name: 'is_active', type: 'bool' }
    ]
  },
  {
    key: 'vw_addresses', label: 'Addresses', table: 'addresses', pk: 'addresse_id', idPrefix: 'address',
    fields: [
      { name: 'clients', type: 'fk:vw_clients' },
      { name: 'type_of_address', type: 'fk:vw_types_of_addresses' },
      { name: 'attention', type: 'text' },
      { name: 'address1', type: 'text' },
      { name: 'address2', type: 'text' },
      { name: 'city', type: 'text' },
      { name: 'state', type: 'fk:vw_states' },
      { name: 'zip', type: 'text' }
    ]
  },
  {
    key: 'vw_states', label: 'States', table: 'states', pk: 'state_id', idPrefix: 'state',
    fields: [
      { name: 'code', type: 'text' },
      { name: 'text', type: 'text' },
      { name: 'tax_rate', type: 'number' }
    ]
  },
  {
    key: 'vw_client_categories', label: 'Client Categories', table: 'client_categories',
    pk: 'client_categorie_id', idPrefix: 'client_category',
    fields: [
      { name: 'name', type: 'text' },
      { name: 'discount', type: 'number' },
      { name: 'notes', type: 'textarea' }
    ]
  },
  {
    key: 'vw_statuses', label: 'Statuses', table: 'statuses', pk: 'statuse_id', idPrefix: 'status',
    fields: [
      { name: 'display_name', type: 'text' },
      { name: 'description', type: 'textarea' },
      { name: 'is_blocking', type: 'bool' },
      { name: 'sort_order', type: 'number' }
    ]
  },
  {
    key: 'vw_types_of_addresses', label: 'Types of Addresses', table: 'types_of_addresses',
    pk: 'types_of_addresse_id', idPrefix: 'type_of_address',
    fields: [
      { name: 'name', type: 'text' },
      { name: 'is_shipping_address', type: 'bool' },
      { name: 'is_billing_address', type: 'bool' }
    ]
  }
];

const BY_KEY = Object.fromEntries(TABLES.map(t => [t.key, t]));

// Pick a sensible label column for FK option rendering.
function labelColFor(viewKey) {
  const map = {
    vw_clients: 'name',
    vw_products: 'display_name',
    vw_invoices: 'invoice_number',
    vw_addresses: 'name',
    vw_states: 'text',
    vw_client_categories: 'name',
    vw_statuses: 'display_name',
    vw_types_of_addresses: 'name',
    vw_payments: 'payment_number',
    vw_invoice_line_items: 'line_number'
  };
  return map[viewKey] || 'name';
}

module.exports = { TABLES, BY_KEY, labelColFor };
