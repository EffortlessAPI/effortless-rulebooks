// Airtable -> Postgres one-way sync.
// Pulls every table, unwraps Airtable's link/lookup arrays into scalar FKs,
// and upserts into the matching base table by airtable_id.

import { pool } from './db.js';
import { fetchAllRecords, baseId } from './airtable.js';

// Map: postgres table -> { airtable: <table name>, columns: { pgcol: airtableField | fn } }
//
// A column value can be:
//   - a string (Airtable field name)
//   - { field, type }                        (coerce)
//   - { link, field }                        (Airtable linked-record array -> first id)
//   - (rec) => value                         (custom)
//
const MAP = {
  statuses: {
    airtable: 'Statuses',
    columns: {
      name:        'Name',
      is_blocking: { field: 'IsBlocking', type: 'bool' }
    }
  },
  states: {
    airtable: 'States',
    columns: {
      name:     'Name',
      code:     'Code',
      tax_rate: { field: 'TaxRate', type: 'numeric' }
    }
  },
  types_of_addresses: {
    airtable: 'TypesOfAddresses',
    columns: { name: 'Name' }
  },
  addresses: {
    airtable: 'Addresses',
    columns: {
      name:               'Name',
      street:             'Street',
      city:               'City',
      zip:                'Zip',
      state_id:           { link: 'State' },
      type_of_address_id: { link: 'TypeOfAddress' }
    }
  },
  app_users: {
    airtable: 'AppUsers',
    columns: { name: 'Name', email: 'Email' }
  },
  client_categories: {
    airtable: 'ClientCategories',
    columns: {
      name:     'Name',
      discount: { field: 'Discount', type: 'numeric' }
    }
  },
  clients: {
    airtable: 'Clients',
    columns: {
      name:               'Name',
      email:              'Email',
      phone:              'Phone',
      status_id:          { link: 'Status' },
      client_category_id: { link: 'ClientCategory' },
      address_id:         { link: 'Address' }
    }
  },
  client_approvals: {
    airtable: 'ClientApprovals',
    columns: {
      name:           'Name',
      client_id:      { link: 'Client' },
      approved_by_id: { link: 'ApprovedBy' },
      approval_date:  { field: 'ApprovalDate', type: 'date' },
      notes:          'Notes'
    }
  },
  products: {
    airtable: 'Products',
    columns: {
      name:        'Name',
      sku:         'SKU',
      unit_price:  { field: 'UnitPrice', type: 'numeric' },
      cost:        { field: 'Cost', type: 'numeric' },
      is_active:   { field: 'IsActive', type: 'bool' },
      description: 'Description'
    }
  },
  inventory_adjustments: {
    airtable: 'InventoryAdjustments',
    columns: {
      name:            'Name',
      product_id:      { link: 'Product' },
      adjustment_type: 'AdjustmentType',
      quantity:        { field: 'Quantity', type: 'numeric' },
      adjusted_at:     { field: 'AdjustedAt', type: 'timestamp' },
      notes:           'Notes'
    }
  },
  invoices: {
    airtable: 'Invoices',
    columns: {
      name:         'Name',
      client_id:    { link: 'Client' },
      invoice_date: { field: 'InvoiceDate', type: 'date' },
      tax_rate:     { field: 'TaxRate', type: 'numeric' },
      notes:        'Notes'
    }
  },
  invoice_line_items: {
    airtable: 'InvoiceLineItems',
    columns: {
      name:             'Name',
      invoice_id:       { link: 'Invoice' },
      product_id:       { link: 'Product' },
      quantity:         { field: 'Quantity', type: 'numeric' },
      unit_price:       { field: 'UnitPrice', type: 'numeric' },
      discount_percent: { field: 'DiscountPercent', type: 'numeric' }
    }
  },
  payments: {
    airtable: 'Payments',
    columns: {
      name:           'Name',
      invoice_id:     { link: 'Invoice' },
      amount:         { field: 'Amount', type: 'numeric' },
      payment_status: 'PaymentStatus',
      payment_method: 'PaymentMethod',
      payment_date:   { field: 'PaymentDate', type: 'date' }
    }
  }
};

// Order matters for FKs.
const ORDER = [
  'statuses',
  'states',
  'types_of_addresses',
  'addresses',
  'app_users',
  'client_categories',
  'clients',
  'client_approvals',
  'products',
  'inventory_adjustments',
  'invoices',
  'invoice_line_items',
  'payments'
];

function coerce(v, type) {
  if (v === undefined || v === null || v === '') return null;
  switch (type) {
    case 'numeric':   { const n = Number(v); return Number.isFinite(n) ? n : null; }
    case 'bool':      return v === true || v === 'true' || v === 1 || v === '1';
    case 'date':      return String(v).slice(0, 10);
    case 'timestamp': return new Date(v).toISOString();
    default:          return v;
  }
}

function extract(rec, spec) {
  const f = rec.fields || {};
  if (typeof spec === 'function') return spec(rec);
  if (typeof spec === 'string')   return f[spec] ?? null;
  if (spec.link) {
    const v = f[spec.link];
    if (Array.isArray(v) && v.length) return v[0];
    return null;
  }
  if (spec.field) return coerce(f[spec.field], spec.type);
  return null;
}

async function upsertTable(table) {
  const def = MAP[table];
  console.log(`-- ${table} (${def.airtable})`);
  let records;
  try {
    records = await fetchAllRecords(def.airtable);
  } catch (e) {
    console.warn(`   skipped: ${e.message}`);
    return;
  }
  console.log(`   fetched ${records.length} records`);

  const cols = ['airtable_id', ...Object.keys(def.columns), 'synced_at'];
  const placeholders = cols.map((_, i) => `$${i + 1}`).join(', ');
  const updates = Object.keys(def.columns).map(c => `${c} = EXCLUDED.${c}`).join(', ');
  const sql = `
    INSERT INTO ${table} (${cols.join(', ')})
    VALUES (${placeholders})
    ON CONFLICT (airtable_id) DO UPDATE
       SET ${updates}, synced_at = EXCLUDED.synced_at
  `;

  const now = new Date().toISOString();
  for (const rec of records) {
    const values = [rec.id];
    for (const [, spec] of Object.entries(def.columns)) values.push(extract(rec, spec));
    values.push(now);
    try {
      await pool.query(sql, values);
    } catch (e) {
      console.warn(`   row ${rec.id} failed: ${e.message}`);
    }
  }
}

console.log(`Syncing from Airtable base ${baseId}`);
for (const t of ORDER) await upsertTable(t);
console.log('Sync complete.');
await pool.end();
