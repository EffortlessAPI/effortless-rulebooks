# What we want v4 to do

Based on Airtable base: [appeUOAaOIdoqPSx3](https://airtable.com/appeUOAaOIdoqPSx3)

## The idea

v4 is a **comprehensive rename** of the platform built in v3. The business logic, seed data, formulas, and relationships are all unchanged. What changes is the vocabulary: we are relabeling the domain to better reflect how the business actually talks about these things.

The motivation is simple: we started calling them "customers" and "orders," but the business calls them **clients** and **invoices**. Every time someone reads the screen and sees "Order #1001" they mentally translate it to "Invoice #1001." We want the system to speak the same language the business does, end to end — table names, field names, column headers, URL slugs, labels, navigation, and anything else the user sees or the code touches.

Nothing about the *behavior* of the system changes. The stopped logic still works. The totals still derive from line items. The payment rules still apply. Delete policies are the same. The dashboard shows the same numbers. The only difference is what things are *called*.

---

## The rename map

Here is every rename, organized by table. If a name does not appear here, it stays the same.

### Table renames

| v3 name          | v4 name              |
|------------------|----------------------|
| Customers        | **Clients**          |
| Orders           | **Invoices**         |
| OrderLineItems   | **InvoiceLineItems** |

Statuses, Products, and Payments keep their table names.

### Field renames

#### Clients (was Customers)

| v3 field | v4 field     |
|----------|--------------|
| Orders   | **Invoices** |

All other fields on Clients remain the same: Name, Notes, IsStopped, Status, StatusDisplayName, StatusDescription, Email, Phone, CompanyName, BillingAddress, ShippingAddress, CreatedAt, AverageOrderValue, LifetimeMarginPercent, CustomerSinceDays, DaysSinceLastOrder.

#### Statuses

| v3 field   | v4 field     |
|------------|--------------|
| Customers  | **Clients**  |

All other fields stay the same.

#### Products

| v3 field       | v4 field               |
|----------------|------------------------|
| OrderLineItems | **InvoiceLineItems**   |

All other fields stay the same.

#### Invoices (was Orders)

| v3 field       | v4 field               |
|----------------|------------------------|
| OrderNumber    | **invoiceNumber**      |
| Customer       | **Client**             |
| OrderLineItems | **InvoiceLineItems**   |
| OrderTotal     | **InvoiceTotal**       |

The following field was **removed** in v4:
- **TotalRefunded** — this rollup existed as scaffolding in v3 but was never fully specified. It has been dropped rather than carried forward under a new name.

All other fields stay the same: Name, Payments, OrderDate, OrderStatus, ShippingAddress, BillingAddress, Notes, ItemCount, TotalQuantity, SubTotal, TaxRate, TaxAmount, TotalPaid, AmountDue, IsPaidInFull, PaymentCount, LastPaymentDate, PaymentStatusLabel.

Note: OrderDate and OrderStatus retain their v3 names — the "Order" prefix here refers to the *event* of ordering, not the table, so it still reads naturally on an invoice.

#### InvoiceLineItems (was OrderLineItems)

| v3 field | v4 field      |
|----------|---------------|
| Order    | **Invoice**   |

All other fields stay the same.

#### Payments

| v3 field    | v4 field            |
|-------------|---------------------|
| Order       | **Invoice**         |
| OrderDate   | **InvoiceDate**     |
| OrderNumber | **InvoiceNumber**   |
| OrderStatus | **InvoiceStatus**   |
| OrderTotal  | **InvoiceTotal**    |

All other fields stay the same: Name, PaymentNumber, PaymentDate, PaymentMethod, TransactionId, Notes, Amount, PaymentStatus, IsCompleted, CompletedAmount, OrderAmountDue, OrderIsPaidInFull.

Note: OrderAmountDue and OrderIsPaidInFull retain their v3 names — these are lookups into the parent record's computed fields, and the underlying formula field names (AmountDue, IsPaidInFull) have not changed.

---

## What the rename means in practice

### Database

Every table name, column name, view name, and function name that referenced the old vocabulary must use the new vocabulary. For example:
- Table `customers` becomes `clients`
- Table `orders` becomes `invoices`
- Table `order_line_items` becomes `invoice_line_items`
- Column `orders.customer` becomes `invoices.client`
- Column `orders.order_total` becomes `invoices.invoice_total`
- View `vw_orders` becomes `vw_invoices`
- Foreign key columns that pointed at the old table names update accordingly

### Application code

Every reference in the backend and frontend must update:
- API routes: `/api/customers` → `/api/clients`, `/api/orders` → `/api/invoices`, `/api/order-line-items` → `/api/invoice-line-items`
- Database queries: all SQL referencing old table/column names
- Frontend components: page titles, navigation labels, table headers, form labels, URL paths
- Variable names and types in code that mirror the old vocabulary

### Seed data

The seed data is **identical** to v3 in substance — same five clients, same seven statuses, same eight products, same seven invoices, same eighteen line items, same nine payments. The records, amounts, dates, and relationships are all the same. Only the labels and internal names have changed to match the new vocabulary (e.g., an order's internal name `bob-1001` stays `bob-1001` because it derives from the client id and invoice number, and those values haven't changed).

---

## What has NOT changed

Everything in v3 that was about *behavior* carries over exactly:

- The **stopped** derivation chain: client → status → blocking → stopped.
- The **totals** derivation chain: line items → subtotal → tax → invoice total → payments → amount due.
- Only **completed** payments count toward total paid.
- The **delete policies**: refuse to delete clients with invoices, refuse to delete statuses with clients, refuse to delete products with line items, cascade-delete line items and payments when deleting an invoice.
- The **dashboard** metrics: total clients (and stopped count), total invoices (by status), total products (active count), total payments (by status with completed dollar total), total revenue, outstanding.
- The **three placeholder formulas** on clients: AverageOrderValue, LifetimeMarginPercent, DaysSinceLastOrder still show "Unable to generate formula."
- The **seven statuses**, **eight products**, and all seed data relationships.

---

## How we'll know v4 is right

Run through the exact same acceptance tests as v3, but with the new vocabulary:

1. All tables, columns, views, API endpoints, and UI labels use the v4 names (Clients, Invoices, InvoiceLineItems, etc.).

2. The **stopped** behavior still works: change a client's status and the stopped indicator flips. Change a status's blocking flag and every client in that status flips. (Steps 1–6 from v3 acceptance.)

3. The **totals** still work: invoice totals equal sum of line item subtotals plus tax. Total paid equals sum of completed payment amounts. Amount due equals invoice total minus total paid. (Steps 9–14 from v3 acceptance.)

4. The **delete policies** still work: can't delete a client with invoices, can't delete a status with clients, can't delete a product with line items, deleting an invoice cascades to its line items and payments. (Steps 16–19 from v3 acceptance.)

5. The **dashboard** still works with correct counts and dollar totals. (Step 15 from v3 acceptance.)

6. **No trace of old vocabulary remains.** No API route still says `/customers` or `/orders`. No UI label says "Orders" where it should say "Invoices." No database table is still named `customers` or `orders`. No column is still named `customer` where it should say `client`. This is the new test unique to v4 — the rename must be *complete*.

The point of v4 is to test whether the system can handle a vocabulary change cleanly. If any old name leaks through — a hardcoded string, a missed migration, a stale component name — the rename is incomplete and v4 has failed.
