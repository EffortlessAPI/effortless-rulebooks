# Clients & Invoices

A small full-stack CRUD application for managing clients, client categories,
statuses, products, invoices, invoice line items, payments, app users,
client approvals, and inventory adjustments. Built as a "naked"
(no-framework-generator) reference implementation with a plain Express +
Postgres backend and a React + Vite frontend.

The application has grown across six schema versions (v1 → v6); its
current shape is defined by the six SQL migrations in
[migrations/](migrations/).

---

## 1. Narrative summary

### 1.1 What it is
**Clients & Invoices** is a back-office web app for a small B2B operation.
It tracks who you sell to, what you sell, what you've invoiced, what
they've paid, who approved them as a customer, and how much stock is on
the shelf. Everything is CRUD over a shared Postgres database — no
background jobs, no external services.

### 1.2 Core problem it solves
Most small teams run this workflow across a spreadsheet, an accounting
tool, and a warehouse tracker. The app collapses all of that into one
relational model where the numbers reconcile by construction:

- An invoice's totals are derived from its line items in SQL — there is
  no "total" column that can drift.
- A client's per-line-item discount is derived from their category — set
  the category once, every future invoice respects it.
- A product's stock-on-hand is derived from the signed sum of its
  inventory adjustments — there is no editable stock counter to desync
  from the history.

### 1.3 User personas
Three roles are represented by the `app_users.role` field. The role is
recorded but not yet enforced by the app:

| Role     | Seeded user    | Typical responsibility                              |
|----------|----------------|-----------------------------------------------------|
| Admin    | Ava Admin      | Platform configuration, categories, statuses, users |
| Manager  | Mia Manager    | Approves clients, manages invoices and payments     |
| Customer | Carl Customer  | (Future) self-serve portal view of own invoices     |

### 1.4 End-to-end user journey
1. **Prospect lands** — a new client record is created with status *New*
   and (optionally) category *Prospect*.
2. **Approval** — a manager creates a `client_approval` row and assigns
   themselves as `approved_by_user_id`. Until assigned, the approval is
   "pending".
3. **Category assignment** — once approved, the client is moved to an
   *Active* or *Gold* category, which carries the tier discount.
4. **Invoicing** — an invoice is created for the client. Each line item
   automatically inherits the client's category discount.
5. **Fulfilment** — line items reference products; completed orders are
   recorded as *Removal* inventory adjustments with reason *Sales Order*.
6. **Payment** — one or more payments are recorded per invoice.
   Completed payments roll up into `total_paid`, `amount_due`, and the
   `payment_status_label` (Paid / Partial / Unpaid).
7. **Replenishment** — purchase-order receipts are recorded as *Addition*
   adjustments; damage and cycle-count corrections feed the same ledger.

### 1.5 Design philosophy
This repo is deliberately **naked**: no ORM, no code generator, no
backend framework beyond Express. All SQL is hand-written in
[backend/server.js](backend/server.js); all queries for derived fields
(subtotal, tax, amount_due, stock_on_hand, profit, margin) live in a
small set of `SELECT` fragments reused across endpoints. The goal is a
reference implementation that is easy to read end-to-end without
stepping through framework internals.

### 1.6 Version history

| Version | Migration           | What it added                                                        |
|---------|---------------------|----------------------------------------------------------------------|
| v1      | `001_initial.sql`   | `customers` table with a color field and a generated `is_stopped`    |
| v2      | `002_v2.sql`        | `statuses` with `is_blocking`; dropped color/is_stopped; re-seeded    |
| v3      | `003_v3.sql`        | Contact fields, `products`, `orders`, `order_line_items`, `payments` |
| v4      | `004_v4.sql`        | Renamed customers→clients, orders→invoices, `*_line_items`           |
| v5      | `005_v5.sql`        | `client_categories` with `discount_percent`; back-filled discounts    |
| v6      | `006_v6.sql`        | `app_users`, `client_approvals`, `inventory_adjustments`              |

---

## 2. Mock data & scenarios

A fresh migration run leaves the database populated with a small but
fully-worked dataset. Nothing is random — the seeds are chosen to
exercise every derived field and edge case.

### 2.1 Seeded entities

| Entity              | Count | Character of the data                                                                     |
|---------------------|-------|-------------------------------------------------------------------------------------------|
| Client categories   | 3     | **Prospect** (0% disc), **Active** (5% disc), **Gold** (15% disc)                         |
| Statuses            | 7     | New, Processing, Delayed, Cancelled, In-Review, Pending, On-Hold; two are `is_blocking`   |
| Clients             | 5     | Bob, Alice Johnson, Brian Lee, Carla Smith, Caroline — mix of statuses and categories     |
| Products            | 8     | WIDGET-001/002, GADGET-100/200, CABLE-USB-C, HUB-7PORT, STAND-ALU, CASE-LEATHER           |
| Invoices            | 7     | Mix of statuses; tax rates set; line items priced to exercise rounding                    |
| Invoice line items  | many  | Discounts inherited from client category; quantities designed to hit integer edge cases   |
| Payments            | many  | Completed / Pending / Failed — covers Paid, Partial, and Unpaid totals                    |
| App users           | 3     | Ava Admin, Mia Manager, Carl Customer — one per role                                      |
| Client approvals    | 5     | One per client; alternating approved / pending; admin approves first two, manager the rest|
| Inventory adjusts   | 11+   | Opening-stock addition per product + targeted Removal / Correction / Sales rows           |

### 2.2 Worked scenarios

- **Scenario A — Invoice lifecycle.** Pick any seeded invoice with
  `payment_status_label = 'Partial'`. Editing its payments from
  *Pending* → *Completed* or back flips `amount_due` and the label
  without any write to the invoice row itself.
- **Scenario B — Tiered discount.** A Gold-tier client's invoices
  have every line item's `discount_percent` pre-filled at 0.15. Changing
  the client's category triggers
  `recomputeLineDiscountsForClient()` in
  [backend/server.js:303-313](backend/server.js#L303-L313), which
  rewrites every line item on every invoice belonging to that client.
- **Scenario C — Stock derivation.** Each seeded product has at least
  one `Addition` row with reason *Purchase Order*. Product #1 has a
  *Removal*/Damage row (−2), product #2 has a *Correction* (+1), and
  product #3 has a *Removal*/Sales Order (−3). The Products list shows
  `stock_on_hand` = `Σ signed adjustments`; there is no editable stock
  counter.
- **Scenario D — Blocking status.** Statuses `Cancelled` and `On-Hold`
  have `is_blocking = true`. Clients in those statuses are surfaced
  with an `is_stopped` flag and counted separately on the summary.
- **Scenario E — Pending approval.** Approvals with
  `approved_by_user_id = NULL` appear as "Pending" in the Approvals
  list. Assigning a user resolves them to `is_approved = true`. Seeded
  approvals alternate so the list always shows both states.

### 2.3 Edge cases present in seed data

- A product with `stock_quantity = 0` (CASE-LEATHER).
- A client with no status set, exercising the `LEFT JOIN` path.
- An invoice with only *Pending* payments (Unpaid label despite
  payments existing).
- An invoice with payments summing beyond the total (label collapses to
  Paid; `amount_due` goes non-positive).
- Inventory adjustment reasons that cover every allowed CHECK value.

---

## 3. Navigation & architecture

### 3.1 Top-level navigation

The app has a single top nav (see
[frontend/src/App.jsx:25-57](frontend/src/App.jsx#L25-L57)) with one
tab per resource:

```
Clients | Statuses | Categories | Approvals | Products | Inventory | Invoices | Users
```

There is no separate dashboard page in the current UI; `/api/summary`
is exposed for future use and tested via the API.

### 3.2 Page pattern

Every resource follows the same **List → Detail → Form** triad:

- `XxxList.jsx`   — table/grid with links into detail + "New" button
- `XxxDetail.jsx` — read-only view plus related-record sections
- `XxxForm.jsx`   — same component handles both create and edit

Two resources (Approvals, Inventory) skip the detail page — the list
already carries all relevant columns.

### 3.3 Route map

| URL pattern                         | Page                       |
|-------------------------------------|----------------------------|
| `/`                                 | ClientList                 |
| `/clients/new`, `/clients/:id`, `/clients/:id/edit` | Client CRUD  |
| `/statuses`, `/statuses/:id`, …     | Status CRUD                |
| `/client-categories`, …             | Category CRUD              |
| `/client-approvals`, `/client-approvals/new`, `/client-approvals/:id/edit` | Approvals (no detail) |
| `/products`, `/products/:id`, …     | Product CRUD               |
| `/inventory`, `/inventory/new`, `/inventory/:id/edit` | Inventory (no detail) |
| `/invoices`, `/invoices/:id`, …     | Invoice CRUD (detail shows lines + payments) |
| `/app-users`, `/app-users/:id`, …   | App User CRUD              |

The full route table is in
[frontend/src/App.jsx:64-95](frontend/src/App.jsx#L64-L95).

### 3.4 Cross-entity navigation
- **Client detail** lists that client's invoices with totals and payment
  labels. Each row links to the invoice detail.
- **Invoice detail** lists line items (linking to products) and
  payments. The header shows the client and the client's category.
- **Product detail** lists every line item that references the product
  and every inventory adjustment affecting it, newest first.
- **Category detail** lists every client in that category.
- **Status detail** lists every client in that status.
- **App-user detail** lists every approval that user has granted.

### 3.5 Frontend architecture
- **Router shell:** [App.jsx](frontend/src/App.jsx) renders the header
  and all routes. React Router v6 with `NavLink` active-class styling.
- **API wrapper:** [api.js](frontend/src/api.js) exposes one object per
  resource (`clients`, `invoices`, `products`, `inventoryAdjustments`,
  …) with `list`, `get`, `create`, `update`, `remove` methods on top of
  `fetch`.
- **Forms:** plain controlled components; validation is "required
  field" only — the backend is the source of truth for errors and
  returns 400/409 with `{ error: string }`.
- **Styling:** a single stylesheet at
  [frontend/src/index.css](frontend/src/index.css). No component
  library.

### 3.6 Backend architecture
- **Entrypoint:** [backend/server.js](backend/server.js) — every
  endpoint in one file, grouped by resource.
- **DB pool:** [backend/db.js](backend/db.js) exports one `pg.Pool`
  reading `DATABASE_URL`. No ORM.
- **SQL fragments:** `CLIENT_SELECT`, `INVOICE_SELECT`,
  `LINE_ITEM_SELECT`, `PAYMENT_SELECT`, `PRODUCT_SELECT`,
  `APP_USER_SELECT`, `CLIENT_APPROVAL_SELECT`, `INVENTORY_SELECT` — each
  is a reusable `SELECT` that computes all derived fields. List/detail
  endpoints append `WHERE …` and `ORDER BY …`.
- **Derivation helpers:**
  `toSlug` / `uniqueSlug` ([server.js:13-29](backend/server.js#L13-L29)),
  `recomputeLineDiscountsForClient`
  ([server.js:303-313](backend/server.js#L303-L313)),
  `invoiceCategoryDiscount`
  ([server.js:805-815](backend/server.js#L805-L815)).
- **Summary endpoint:** `/api/summary` runs twelve aggregation queries
  in parallel and merges the results
  ([server.js:148-279](backend/server.js#L148-L279)).

### 3.7 Data flow

```
Browser  ──fetch──▶  Vite dev server  ──proxy /api/*──▶  Express (3011)  ──▶  Postgres
```

Vite's dev-server proxy is configured in
[frontend/vite.config.js](frontend/vite.config.js) to forward
`/api/*` to `http://localhost:3011`.

---

## 4. Technical specification details

### 4.1 REST API

All endpoints are JSON. Base URL: `/api`. List endpoints return arrays;
detail endpoints return a single object (or 404). Create returns 201
with the full row; update returns 200; delete returns 204. Referential
integrity errors return 409 with a descriptive `{ error }` message.

| Method | Path                                | Purpose                                  |
|--------|-------------------------------------|------------------------------------------|
| GET    | `/summary`                          | Dashboard aggregates (12 parallel queries)|
| GET/POST/PUT/DELETE | `/clients[/:id]`        | Full CRUD — detail includes invoices     |
| GET/POST/PUT/DELETE | `/client-categories[/:id]` | Full CRUD — detail includes clients    |
| GET/POST/PUT/DELETE | `/statuses[/:id]`        | Full CRUD — detail includes clients      |
| GET/POST/PUT/DELETE | `/products[/:id]`        | Full CRUD — detail includes line items + adjustments |
| GET/POST/PUT/DELETE | `/invoices[/:id]`        | Full CRUD — detail includes line items + payments |
| GET    | `/invoice-line-items?invoice_id=N`  | Filter by invoice                        |
| POST/PUT/DELETE | `/invoice-line-items[/:id]` | Discount is always re-derived from category |
| GET    | `/payments?invoice_id=N`            | Filter by invoice                        |
| POST/PUT/DELETE | `/payments[/:id]`          | Status drives total_paid / amount_due    |
| GET/POST/PUT/DELETE | `/app-users[/:id]`       | Full CRUD — detail lists approvals       |
| GET    | `/client-approvals?client_id=N`     | Filter by client                         |
| POST/PUT/DELETE | `/client-approvals[/:id]`  | Pending = `approved_by_user_id IS NULL` |
| GET    | `/inventory-adjustments?product_id=N` | Filter by product                      |
| POST/PUT/DELETE | `/inventory-adjustments[/:id]` | Signed ledger — no editable stock counter |

Validation rules (enforced in the backend):

- Client `name`, product `sku`, invoice `invoice_number`, status `name`
  are required. 400 otherwise.
- App user requires either `contact_name` or `email`.
- Unique-constraint violations return 409.
- Deleting a row that would orphan children returns 409 with a message
  naming the blocker (e.g. "5 invoice(s) belong to this client").
- Inventory adjustment `quantity` must be a non-negative integer.
  Sign is carried by `adjustment_type`.

### 4.2 Data model

```
client_categories (id, name, slug, notes, discount_percent)
   └──< clients (id, name, slug, notes,
                 status_id?, category_id?,
                 company_name, email, phone,
                 billing_address, shipping_address)
        │
        ├──< invoices (id, invoice_number, slug, client_id,
        │              order_date, order_status,
        │              shipping_address, billing_address,
        │              tax_rate, notes)
        │    ├──< invoice_line_items (id, invoice_id, line_number, slug,
        │    │                        product_id, quantity, unit_price,
        │    │                        discount_percent, notes)
        │    └──< payments (id, invoice_id, payment_number, slug,
        │                   payment_date, amount, payment_method,
        │                   payment_status, transaction_id, notes)
        │
        └──< client_approvals (id, slug, client_id,
                               approved_by_user_id?, notes)

statuses (id, name, slug, description, is_blocking, sort_order)
   └──< clients.status_id

products (id, sku, slug, display_name, description,
          unit_price, cost, stock_quantity, is_active)
   ├──< invoice_line_items.product_id
   └──< inventory_adjustments (id, slug, product_id, adjustment_date,
                               adjustment_type, quantity, reason,
                               adjusted_by, notes)

app_users (id, name, slug, contact_name, email, phone, role, notes)
   └──< client_approvals.approved_by_user_id (nullable = Pending)
```

**FK cascade behaviour**

| Relationship                                   | On delete                    |
|------------------------------------------------|------------------------------|
| `invoice_line_items → invoices`                | CASCADE                      |
| `payments → invoices`                          | CASCADE                      |
| `client_approvals → clients`                   | CASCADE                      |
| `client_approvals → app_users`                 | SET NULL (→ goes Pending)    |
| `inventory_adjustments → products`             | CASCADE                      |
| `clients → statuses`, `clients → categories`   | blocked at API layer (409)   |
| `invoice_line_items → products`                | blocked at API layer (409)   |

**Check constraints**

| Column                              | Allowed values                                                        |
|-------------------------------------|-----------------------------------------------------------------------|
| `app_users.role`                    | Admin, Manager, Customer                                              |
| `inventory_adjustments.type`        | Addition, Removal, Correction                                         |
| `inventory_adjustments.reason`      | Purchase Order, Sales Order, Inventory Count, Damage, Return, Transfer, Other |
| `payments.payment_status`           | Completed, Pending, Failed (enforced in API; DB is TEXT)              |

**Slug algorithm**

Every user-facing row carries a unique `slug`. `toSlug()` lowercases
and strips to `[a-z0-9-]+`; `uniqueSlug()` appends `-1`, `-2`, … if
needed, optionally excluding the row being updated
([server.js:13-29](backend/server.js#L13-L29)). Composite slugs
(invoices, line items, payments, approvals, inventory) are built from
their parent's slug plus a disambiguator — e.g. `alice-johnson-inv-002`,
`alice-johnson-inv-002-line-1`, `widget-001-2026-03-20-qty5`.

### 4.3 Computed fields

**Invoice money math** — computed in `INVOICE_SELECT`
([server.js:51-101](backend/server.js#L51-L101)):

- `subtotal` = `Σ quantity × unit_price × (1 − discount_percent)`
- `tax_amount` = `ROUND(subtotal × tax_rate, 2)`
- `invoice_total` = `subtotal + tax_amount`
- `total_paid` = `Σ amount WHERE payment_status = 'Completed'`
- `amount_due` = `invoice_total − total_paid`
- `is_paid_in_full` = `amount_due ≤ 0`
- `payment_status_label` = `Paid` if paid in full, `Unpaid` if nothing
  completed, else `Partial`.

**Product analytics** — computed in `PRODUCT_SELECT`
([server.js:566-599](backend/server.js#L566-L599)):

- `profit` = `unit_price − cost`
- `margin` = `1 − cost / unit_price` (zero-safe)
- `is_high_margin` = `margin > 0.65`
- `cogs` = `cost × total_sold_qty`
- `stock_on_hand` = `Σ` signed adjustments, where
  `Addition` and `Correction` count positive and `Removal` counts
  negative.

**Inventory signed quantity** — computed in `INVENTORY_SELECT`
([server.js:1114-1128](backend/server.js#L1114-L1128)): each adjustment
row carries a `signed_quantity` that matches the sign convention above,
so stock aggregates can be read straight off the ledger.

### 4.4 Migrations

Migrations are **ordered and idempotent**. They use:

- `CREATE TABLE IF NOT EXISTS`, `ADD COLUMN IF NOT EXISTS`.
- `DO $$ ... $$` blocks to guard renames, drops, and seed loops.
- `WHERE NOT EXISTS (SELECT 1 FROM <table>)` around seed inserts so
  re-running does not double-seed.

Destructive changes in history:

- v2 dropped `customers.color` and the generated `customers.is_stopped`.
- v4 renamed `customers → clients`, `orders → invoices`,
  `order_line_items → invoice_line_items`, and the matching FK
  columns.

### 4.5 Configuration

| Var             | Default                                             |
|-----------------|-----------------------------------------------------|
| `DATABASE_URL`  | `postgresql://localhost/2026-04-20-1700-naked`      |
| `BACKEND_PORT`  | `3011`                                              |
| `FRONTEND_PORT` | `5173`                                              |

`start.sh` applies every migration in order, runs `npm install` in
both `backend/` and `frontend/`, starts the Express API in the
background, waits for `/api/summary` to respond, then hands the
foreground to the Vite dev server. `Ctrl+C` stops the backend too.

### 4.6 Non-functional properties and known limitations

- **No auth.** The `role` column exists but no middleware reads it.
  Any client can call any endpoint.
- **No pagination.** List endpoints return the full table ordered.
- **No soft deletes.** Delete is `DELETE FROM`, guarded only by FK
  reference counts.
- **No audit log** beyond `inventory_adjustments` (which is a ledger).
  Client and invoice history is not retained.
- **No tests.** There is no test suite — manual verification only.
- **No approval state machine.** "Pending vs Approved" is purely a
  function of whether `approved_by_user_id` is NULL; there is no
  rejection, resubmission, or audit trail.

Things that would come next:
1. Role-gated middleware + login (Admin vs Manager vs Customer).
2. Approval workflow states (Pending → Approved / Rejected).
3. Pagination and server-side filtering on list endpoints.
4. An audit table for invoices, payments, and clients.
5. A true dashboard UI consuming `/api/summary`.

### 4.7 Operational

- Fresh DB: `dropdb 2026-04-20-1700-naked && createdb
  2026-04-20-1700-naked && ./start.sh`.
- Backend alone: `cd backend && npm install && DATABASE_URL=… node
  server.js` → `:3011`.
- Frontend alone: `cd frontend && npm install && npm run dev` →
  `:5173`.
- If `BACKEND_PORT` is changed, either update the proxy target in
  [frontend/vite.config.js](frontend/vite.config.js) or export
  `VITE_BACKEND_PORT` the way `start.sh` does.

### 4.8 Glossary

| Term                   | Meaning                                                                 |
|------------------------|-------------------------------------------------------------------------|
| **slug**               | URL-safe `[a-z0-9-]+` identifier, unique per table, auto-derived        |
| **is_blocking**        | A status that halts forward progress; surfaced as `is_stopped` on client |
| **discount_percent**   | Fractional discount (0.15 = 15%); set on category, copied to line items |
| **adjustment_type**    | Addition / Removal / Correction — sign rule for `stock_on_hand`         |
| **role**               | Admin / Manager / Customer — recorded but not enforced                  |
| **payment_status_label** | Derived Paid / Partial / Unpaid; not stored                           |
| **Pending approval**   | `client_approvals` row with `approved_by_user_id IS NULL`               |

---

## Prerequisites

- **Node.js** 18+
- **PostgreSQL** running on `localhost` with your user allowed to create
  databases
- **`psql`** on your `PATH` — the migration step shells out to it

### Create the database

```bash
createdb 2026-04-20-1700-naked
```

Or set `DATABASE_URL` to point at whatever database you want to use.

## Running the app

```bash
./start.sh
```

When it's up:

- UI: <http://localhost:5173>
- API: <http://localhost:3011/api>

## Project layout

```
.
├── start.sh
├── migrations/              # 001..006 — applied in order, idempotent
├── backend/
│   ├── db.js                # pg Pool
│   └── server.js            # All endpoints + SQL
├── frontend/
│   ├── vite.config.js       # /api proxy to :3011
│   └── src/
│       ├── App.jsx          # Router + nav
│       ├── api.js           # Fetch wrapper per resource
│       ├── index.css
│       └── pages/           # List / Detail / Form per resource
└── v1..v4_SPECIFICATION.md  # Historical written specs per version
```

## Historical specifications

The `v1_SPECIFICATION.md` … `v4_SPECIFICATION.md` files record the
written requirements each migration was built against. They are kept
as-is for reference; the current source of truth is the SQL in
[migrations/](migrations/).
