# naked-claude-demo
$101.92 at this point.

A two-tier demo app that mirrors an Airtable base into Postgres and exposes it through both an internal admin UI and a customer-facing storefront/portal. The Airtable base is treated as the system of record for *base data only*; every derived value (formulas, lookups, rollups) is computed on the Postgres side by `vw_*` views so the read model stays consistent regardless of where edits originate.

---

## 1. Narrative platform description

This is a small B2B order-management platform for a fictional wholesale operation. There are two audiences:

- **Operators / staff** use the admin UI ([public/index.html](public/index.html)) at `/` — a thin, table-by-table CRUD console over every entity in the base. It is intentionally generic: pick a table from the top nav, filter, sort, create/edit/delete rows. Foreign-key columns render as dropdowns populated from the linked table. Reads are against `vw_*` views (so users see the same derived fields Airtable shows); writes go to base tables (so derived values can never drift from their inputs).
- **Customers** use the portal ([public/portal/index.html](public/portal/index.html)) at `/portal/` — a styled single-page app with hash-based routing. A customer signs in by picking themselves from the client list (no real auth — this is a demo), then browses a shop, manages a cart, checks out into a real `invoice` + `invoice_line_items` + `payments` triple, and views their order history with payment status, line items, and outstanding balance.

The same Postgres database backs both UIs. Anything a customer does in the portal is immediately visible to staff in the admin UI, and vice versa. The Airtable sync is one-way (Airtable → Postgres) and is run on demand via `npm run sync`; it is the seed-and-refresh path, not the runtime write path.

### Domain model

The base models a small wholesale business:

- **Clients** belong to a **ClientCategory** (which carries a discount), have a **Status** (some statuses are *blocking* — those clients are flagged `is_stopped` and cannot transact), and link to a primary **Address** (which itself has a **TypeOfAddress** — shipping vs billing — and a **State** which carries the tax rate).
- **Products** have a SKU, unit price, cost, and an active flag. **InventoryAdjustments** roll up into a derived `stock_quantity` (Addition/Correction add, Removal subtracts). Margin, profit, COGS, and `is_high_margin` (margin > 65%) are derived per product.
- **Invoices** belong to a Client and contain **InvoiceLineItems** (qty × product unit_price × (1 − discount_percent)) and **Payments**. The invoice view rolls up sub-total, tax, total, total paid, amount due, and a label of `Paid` / `Partial` / `Unpaid`. `is_big_order` flags sub-totals over $350. The **effective tax rate** on an invoice is the client's *current* billing-address state rate; the rate stored on the invoice row is used only as a fallback when the client has no billing state. This means changing a client's address (or a state's `tax_rate`) re-tags every one of their invoices automatically, and the same rule is used by the client-level `total_revenue` and `average_order_value` rollups so the two stay consistent.
- **ClientApprovals** records who on staff (an **AppUser**) approved a given client, with a derived `is_approved` flag.
- **Statuses** drive client lifecycle. `is_blocking = true` on a status means clients in that status are stopped (`vw_clients.is_stopped`).
- **VIP rule:** a client is `is_vip` when their average order value exceeds $500 *and* they have at least one invoice within the last 220 days. Both conditions must hold — see [src/schema.sql:414-418](src/schema.sql#L414-L418).

### Architecture in one paragraph

Airtable holds the editorial truth of base fields. `npm run sync` pulls every table, unwraps Airtable's link/lookup arrays into scalar FKs, and upserts into Postgres base tables. Postgres `vw_*` views replicate Airtable's formulas/lookups/rollups in SQL, so every read — admin or portal — joins through views and never has to recompute totals in the UI. The Express API ([src/server.js](src/server.js)) is generated from a per-table `SCHEMA` map: GET endpoints query views, POST/PATCH/PUT/DELETE write base tables and return the freshly-computed view row. There is no ORM.

---

## 2. Mock data and scenarios

The seed data lives in Airtable base `appeUOAaOIdoqPSx3` and is pulled via the sync. The shapes/scenarios it is designed to exercise:

- **A blocking-status client.** At least one client has a status with `is_blocking = true`. In `vw_clients` they appear with `is_stopped = true`; the portal refuses to let them check out and the admin UI shows them with a red status pill.
- **A VIP client.** A client with several invoices, at least one inside the last 220 days, and an average order value over $500. Verifies the `is_vip` rollup against the 220-day window.
- **A lapsed high-spender.** A client whose AOV is over $500 but whose last invoice is older than 220 days. Confirms `is_vip` is `false` because both halves of the rule must hold (this case caught a regression — see commit `d7ff4c7`).
- **A category with a discount.** Clients in this category get `category_discount` applied at checkout-suggestion time and via `client_category_discount` on the invoice view.
- **Mixed-state addresses.** Addresses span multiple `states`, each with its own `tax_rate`, so invoice tax math varies by client. Some addresses are shipping type, others billing.
- **A multi-payment invoice.** An invoice with two or more `payments` rows, exercising the `payment_status_label = 'Partial'` branch and the `last_payment_date` rollup. Some payments are `Completed`, others not, so `total_paid` only counts completed amounts.
- **A high-margin and a low-margin product.** `vw_products.is_high_margin` should be `true` only for the product whose margin exceeds 65%.
- **Inventory churn.** A product with a sequence of Addition / Correction / Removal adjustments so `stock_quantity` is non-trivially derived, not just the latest row.
- **Approved vs pending clients.** Some clients have a `client_approvals` row with `approved_by_id` set (`is_approved = true`); others have a row with `approved_by_id = NULL` (pending).
- **Big vs small orders.** At least one invoice has a sub-total above $350 (`is_big_order = true`) and one below.

The customer portal further exercises this dataset by letting you act *as* any of these clients (pick from the customer-switcher in the top bar) and observe how the UI changes — blocking statuses disable checkout, VIP clients see a VIP badge on Account, lifetime totals match the sum of `invoice_total` across their orders.

---

## 3. Technical specification

### Stack

- **Runtime:** Node.js ≥ 18 (ESM, `"type": "module"`).
- **Server:** Express 4 ([src/server.js](src/server.js)).
- **Database:** Postgres via `pg` ([src/db.js](src/db.js)). Default connection string: `postgres:///naked_claude_naked_test` (override with `DATABASE_URL`).
- **Frontend:** Two static HTML files with vanilla JS — no build step, no framework.
  - Admin UI: [public/index.html](public/index.html) — generic table browser with create/edit modal driven by `/api/_schema`.
  - Customer portal: [public/portal/index.html](public/portal/index.html) — hash-routed SPA (`#/`, `#/shop`, `#/orders`, `#/orders/:id`, `#/account`, `#/cart`, `#/checkout`).
- **Source of truth:** Airtable base `appeUOAaOIdoqPSx3` (overridable via `AIRTABLE_BASE_ID`). Auth via `AIRTABLE_API_KEY`.

### Layout

```
src/
  schema.sql       Base tables + vw_* views (the entire data contract)
  init-db.js       Apply schema.sql to the configured database
  airtable.js      Paginated Airtable fetch helper
  sync.js          Airtable -> Postgres upsert for every table
  db.js            Shared pg.Pool
  server.js        Express API (CRUD + a few nested-list endpoints)
public/
  index.html       Admin table browser
  portal/
    index.html     Customer-facing portal SPA
start.sh           Boot two server instances on fixed ports for demo use
```

### Data model

Two parallel layers, defined in [src/schema.sql](src/schema.sql):

1. **Base tables** — exactly the columns a user can edit. Every table has `airtable_id TEXT PRIMARY KEY` and a `synced_at TIMESTAMPTZ` audit column. FKs use `ON DELETE SET NULL` for soft references (e.g. `clients.status_id`) and `ON DELETE CASCADE` for owned children (e.g. `invoice_line_items` → `invoices`, `payments` → `invoices`).
2. **`vw_*` views** — derived fields (lookups, formulas, rollups) computed in SQL so they match what Airtable would show. The application *never* reads from base tables; it reads from views.

Tables: `statuses`, `states`, `types_of_addresses`, `addresses`, `app_users`, `client_categories`, `clients`, `client_approvals`, `products`, `inventory_adjustments`, `invoices`, `invoice_line_items`, `payments`.

Notable derived fields:

| View | Field | Definition |
| --- | --- | --- |
| `vw_products` | `stock_quantity` | sum of inventory_adjustments (Removal subtracts) |
| `vw_products` | `margin`, `is_high_margin`, `cogs`, `profit` | margin = 1 − cost/price; high if > 0.65 |
| `vw_products` | `has_been_ordered_by_vip_customers` | `count_of_vip_orders > 0 AND stock_quantity > 250` |
| `vw_invoices` | `sub_total`, `tax_amount`, `invoice_total`, `amount_due`, `total_paid`, `payment_status_label` | rolled up from line items + completed payments |
| `vw_invoices` | `tax_rate`, `tax_state_code`, `tax_state_rate`, `invoice_stored_tax_rate` | effective rate is `COALESCE(client billing-state rate, invoice.tax_rate)` |
| `vw_invoices` | `is_big_order` | `sub_total > 350` |
| `vw_invoices` | `is_paid_in_full` | `amount_due <= 0` |
| `vw_clients` | `is_stopped` | lookup of `statuses.is_blocking` |
| `vw_clients` | `is_vip` | `avg_order_value > 500 AND last_invoice_date > now − 220d` |
| `vw_clients` | `total_revenue`, `average_order_value` | summed/averaged using the same effective tax rate as `vw_invoices` (client billing-state rate, then stored) |
| `vw_clients` | `state_code`, `state_name`, `state_tax_rate`, `address_formatted` | flattened from the client's primary address + state |
| `vw_clients` | `has_recent_invoices` | last invoice within 220 days |
| `vw_invoice_line_items` | `pre_discount`, `discount_amount`, `sub_total` | qty × unit_price math |
| `vw_payments` | `is_completed`, `completed_amount` | gate by `payment_status = 'Completed'` |

### HTTP API

Mounted in [src/server.js](src/server.js). All responses are JSON.

- `GET /api` — service descriptor and table index.
- `GET /api/_schema` — full editable-column map (used by the admin UI to render forms).
- `GET /api/_schema/:table` — single-table schema.

For each table `T` in the `TABLES` list:

| Method | Path | Behavior |
| --- | --- | --- |
| GET | `/api/T` | `SELECT * FROM vw_T ORDER BY name NULLS LAST` (or `airtable_id` for `inventory_adjustments`). |
| GET | `/api/T/:id` | Single row from `vw_T`. 404 on miss. |
| POST | `/api/T` | Insert into base table. Body fields are filtered through `SCHEMA[T]` and coerced (`int`, `numeric`, `bool`, `timestamp`, `text`, `fk`). Server generates `airtable_id` (`rec` + 16 hex chars) unless one is supplied. Returns the new row from the view. |
| PATCH | `/api/T/:id` | Partial update. Sets `synced_at = NOW()`. 400 if no editable fields. Returns the view row. |
| PUT | `/api/T/:id` | Full replace. Missing fields become `NULL`. Returns the view row. |
| DELETE | `/api/T/:id` | Hard delete. 204 on success. |

Nested convenience endpoints (read-only, view-backed):

- `GET /api/clients/:id/invoices`
- `GET /api/invoices/:id/line-items`
- `GET /api/invoices/:id/payments`
- `GET /api/products/:id/inventory-adjustments`
- `GET /api/clients/:id/approvals`

### Frontend

**Admin UI ([public/index.html](public/index.html))** — fetches `/api/_schema` once, renders a generic table browser with a top nav per table. Filter input does client-side substring match across visible cells; column headers toggle sort. The "+ New" / row "Edit" actions open a modal whose fields are derived from the schema (FK fields render as `<select>` populated by fetching the linked table).

**Customer portal ([public/portal/index.html](public/portal/index.html))** — hash-routed SPA, client-side only. Routes:

- `#/` — Home: KPIs (open balance, lifetime spend, last order date), recent orders table.
- `#/shop` — Product catalog with chip filters and search (`?q=`, `?status=`).
- `#/shop/:productId` — Product detail with add-to-cart.
- `#/cart` — Cart management; preview totals computed in JS at `TAX_RATE = 0.0875`.
- `#/checkout` — Posts a new `invoices` row, then line items, then a `payments` row; redirects to the new order.
- `#/orders` — Filterable order list (`?status=open|paid|all`).
- `#/orders/:id?tab=items|payments` — Order detail with line items and payments tabs.
- `#/account` — Profile, status, VIP badge, address.

The current customer is held in `localStorage` (the `who` chip in the top bar opens a customer-switcher). Cart is also persisted in `localStorage` and is per-customer. Checkout refuses to proceed for `is_stopped` clients.

### Configuration

Environment variables:

- `DATABASE_URL` — Postgres connection string. Default `postgres:///naked_claude_naked_test`.
- `AIRTABLE_API_KEY` — required for `npm run sync`.
- `AIRTABLE_BASE_ID` — defaults to `appeUOAaOIdoqPSx3`.
- `PORT` — HTTP listen port. Default `3000`.

### Running

```
createdb naked_claude_naked_test     # one-time
npm install
npm run init-db                       # apply schema.sql
AIRTABLE_API_KEY=... npm run sync     # pull base data from Airtable
npm start                             # http://localhost:3000
```

For demo presentations, [start.sh](start.sh) launches two server instances on fixed ports `47821` and `47822` (so VS Code can Ctrl-click the URLs) and writes logs/PIDs under `.run/`.

### Conventions and constraints

- **Reads always go through views.** Application code never selects from a base table. This keeps derived values authoritative and lets schema changes (e.g. adding a rollup) be a one-place edit.
- **Writes always go to base tables.** Views are not updatable; the API never tries.
- **No ORM, no migrations framework.** Schema is one file; `init-db` is destructive (`DROP ... CASCADE`) and rebuilds from scratch. Resync from Airtable to repopulate.
- **IDs are Airtable record IDs.** Server-generated IDs follow the same `rec…` shape, so a row created in the app is indistinguishable from one synced from Airtable.
- **No real auth.** The portal's "who am I" picker is a demo affordance, not a security boundary. Do not deploy this as-is.
