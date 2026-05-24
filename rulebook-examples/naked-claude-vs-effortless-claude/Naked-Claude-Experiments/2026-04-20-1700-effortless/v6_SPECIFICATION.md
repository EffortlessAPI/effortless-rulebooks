# What we want v6 to do

Based on Airtable base: [appeUOAaOIdoqPSx3](https://airtable.com/appeUOAaOIdoqPSx3)

## The idea

v5 quietly shipped three capabilities that were never formalized in a spec:
**client categories** with discount tiers, **inventory adjustments** as an
append-only audit log, and a scattering of new calculated fields on Clients,
Products, and Invoices. On top of that, the rulebook added two entire
entities — **AppUsers** and **ClientApprovals** — that have no app surface at
all.

v6 is the alignment release. Nothing new gets invented. We catch the app up
to the rulebook:

1. Surface AppUsers and ClientApprovals as first-class pages.
2. Expose the new calculated/aggregation fields that already exist in the views.
3. Extend the dashboard with the new business signals those fields produce.

If it's in the rulebook, it's in the app.

---

## New pages

### `/app-users` — AppUsers list + detail

Full CRUD for system users who perform approvals and inventory adjustments.

- **List:** ContactName, EmailAddress, PhoneNumber, Role, # of approvals.
- **Create/edit form:** ContactName, EmailAddress, PhoneNumber, Role (free
  text), Notes. `app_user_id` is generated from name + email (slugified).
- **Delete:** blocked if the user has any approvals referencing them.
- **Detail:** shows the user's approval history (which clients, when,
  approved vs. pending).

### `/approvals` — ClientApprovals list

Review and manage the approval queue.

- **List:** Client (link), ApprovedBy (link), IsApproved badge (green APVD
  / amber PEND), Notes. Filter toggle: All / Pending / Approved.
- **Create:** pick a Client and an AppUser; `client_approval_id` =
  `{client}-{app_user}`. If ApprovedBy is left blank the approval is created
  in pending state.
- **Edit:** assign / change ApprovedBy, edit notes.
- **Delete:** unconditional (approvals are workflow records, not audit).

### On the Client detail page

A new **Approvals** section (below Invoices) showing that client's
approvals with inline create/delete. Gives users a per-client view of who
signed off.

---

## Surfaced fields (already in DB, not yet in UI)

### Clients
- **IsVIP** — gold star badge next to the name when true.
- **LastInvoice** — shown in the client stat block (next to Avg Invoice Value).
- **CountOfBigInvoices** — shown in the client stat block as "Big Invoices".
- **HastRecentInvoices** — small indicator next to LastInvoice ("recent" if true).
- **CategoryDiscount** — already shown as a `−10%` chip next to category; keep.

Replace the v4-era `days_since_last_order` placeholder (which returns null
now) with `LastInvoice` and `CountOfBigInvoices`.

### Products
- **Profit** — shown in product detail money grid.
- **Margin** — shown as a percent in product detail and list.
- **IsHighMargin** — ⭐ badge on the product card + list row when true.
- **COGS** — shown in product detail money grid (StockQty × Cost).

### Invoices
- **IsBigOrder** — 💰 badge on invoice list row + detail header when true.
- **ClientEmail / ClientPhone / ClientCompanyName** — shown in the invoice
  header next to the client link (so the invoice is self-contained without
  clicking through).

---

## Dashboard extensions

New stat cards in the Summary grid:
- **VIP Clients** (count of `is_vip = true`)
- **Big Orders** (count of `is_big_order = true`)
- **High-Margin Products** (count of `is_high_margin = true`)
- **Pending Approvals** (count of approvals where `is_approved = false`)

---

## Hard rules (unchanged from prior specs)

- Read from `vw_*` views, never base tables. (Writes still go to base tables.)
- No schema changes, no Airtable writes. v6 is app-layer only.
- No backwards-compatibility shims: the app should *speak* v6 vocabulary.
  Field typos (`hast_recent_invoices`, `client_categorie_id`) are preserved
  as-is because they are what the rulebook/DB expose.

---

## Out of scope for v6

- Renaming `HastRecentInvoices` → `HasRecentInvoices` (breaking rulebook change).
- Renaming `ClientCategorieId` → `ClientCategoryId` (breaking FK change).
- AppUser roles as an enum / permissions model (stays free-text for now).
- Approval state beyond APVD/PEND (no rejection, no withdrawals).
