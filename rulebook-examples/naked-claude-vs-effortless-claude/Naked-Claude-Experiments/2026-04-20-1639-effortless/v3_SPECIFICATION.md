# What we want v3 to do

Based on Airtable base: [appKLygCIXweUUKtM](https://airtable.com/appKLygCIXweUUKtM)

## The idea

We want a small but real customer-and-orders system. Something we can use to track who our customers are, where they stand with us, what they've bought, what we've shipped, what they've paid us, and what they still owe. Not a full ERP — we're not doing inventory forecasting or accounting periods or returns workflows or any of that. But enough to actually run a tiny business out of: a customer list, a product catalog, the orders that connect them, the line items that make up each order, and the payments that come in against those orders.

The whole thing is built around one principle: **the totals come from the line items.** We don't type "this order is $1,247.83." We type the line items — quantities, prices, discounts — and the system computes the order total. Then payments come in, and the system computes how much is still owed. Everything that can be derived is derived. Anything we type by hand should be the *cause* of something, never the *result*.

## The big picture

There are six things in the system:

1. **Customers** — the people or companies we sell to.
2. **Statuses** — a small managed set of categories that say where each customer stands with us right now (think: New, Pending, On-Hold, etc.). Each customer points at one status.
3. **Products** — our catalog. The things we sell.
4. **Orders** — when a customer places an order, that's an order. Each order belongs to exactly one customer.
5. **Order line items** — the things on an order. Each line item is one product on one order, with a quantity and a price. An order can have many line items.
6. **Payments** — the money coming in. Each payment is applied to one specific order. An order can have many payments (a deposit, a final payment, a refund, etc.).

The relationships, in plain English:

- A customer has many orders.
- An order has many line items, and each line item points at a product.
- A product can appear on many line items across many orders.
- An order has many payments.

That's the whole shape. Everything else is fields and formulas.

---

## Customers

A customer is someone we sell to. For each customer we want to know:

### Identity

- **A name.** Could be a person's name, could be a company. Whatever we want shown in the customer list.
- **A company name.** Optional. If the customer is acting on behalf of a company, this is where we put it. If they're an individual, this is just empty.
- **An internal identifier.** A short slug like `bob` or `alice-johnson` so the system can refer to them unambiguously. We don't look at this much, but it's how the rest of the system points at this customer.

### Contact

- **Email.** Their primary email address.
- **Phone.** Their primary phone number.
- **Billing address.** Where invoices go.
- **Shipping address.** Where physical goods go. Often the same as billing, but not always.

All of these are optional in the sense that we may not have them yet, but we want fields for them so we can fill them in over time. None of them are validated beyond "is it a string" — we're not checking that emails are well-formed or phones are dialable. That's a problem for later.

### Standing

- **Status.** Each customer points at one of the statuses we've defined (see below). A customer can also have *no* status assigned, and that's a meaningful "we haven't categorized them" state.
- **Stopped indicator.** This is *derived* from the customer's status — see the "Stopped" section below. We do not store it as a field. It's computed every time we look at the customer.

### History and metrics

- **Created at.** A timestamp recording when we first added the customer to the system. Set automatically when we create them; not editable afterward.
- **Customer since (days).** A number — how many days ago we created the customer. Recomputed every time we look at it. Today this is `now - created_at` measured in whole days. If `created_at` isn't set, this is blank.
- **Orders.** Not a field we type — it's the natural list of every order this customer has placed. When we look at a customer, we should be able to see their orders right there.
- **Average order value.** We *want* this to be the average dollar value across this customer's orders. It is a placeholder right now — the formula isn't worked out yet — and the system will show "Unable to generate formula" until we sort it out. We're aware of this and we want it written down so it doesn't get quietly forgotten.
- **Lifetime margin percent.** Same situation. We *want* this to be the customer's lifetime gross margin as a percentage (revenue minus cost, divided by revenue, across everything they've ever bought). Same placeholder for now.
- **Days since last order.** Same situation. We want it; the formula's a placeholder.

So three of the customer metrics are aspirational placeholders today. They show up as the literal string "Unable to generate formula" wherever they appear, on purpose, so we can see at a glance which calculated fields we still owe ourselves.

### Notes

- **Notes.** Free-form text. A sentence or two about what's going on with the customer, why we care, anything we want to remember.

### What we can do with customers

- Add a new customer (name is required; everything else is optional).
- Edit any of the editable fields above. Anything derived (the stopped indicator, orders list, customer-since-days, the placeholder metrics) is read-only.
- Delete a customer. We need to think carefully about what this does to their orders — see "Deletes and dependencies" at the end.
- See a list of all customers, with their most important fields: name, company, email/phone, status, stopped, notes.
- Open a customer and see everything: all of their fields, plus the list of their orders inline.

---

## Statuses

A status is a category that says where a customer stands with us right now.

### Fields

- **Display name.** What we read on screen. Like "On-Hold" or "In-Review" or "Pending".
- **Internal name.** The display name in lowercase. We don't type it — the system fills it in automatically. So "On-Hold" becomes `on-hold`. This is the handle the rest of the system uses.
- **Description.** A sentence or two explaining what this status actually means and when to apply it. This matters because it's how a new person learns the vocabulary without having to ask.
- **Blocking flag.** Yes or no. A blocking status means: a customer in this status is stuck, something needs to give before they can move forward. A non-blocking status means: things are progressing, no special attention needed.
- **Sort order.** A number that controls the order statuses appear in lists. We use this so "New" can come first and "Cancelled" can come last regardless of how we typed them in.
- **Customers.** Not a field we type — it's the natural list of every customer currently assigned to this status. When we open a status, we want to see who's in it.
- **An internal identifier.** Same as customers — a short slug the system uses to point at this status.

### The seven statuses we want from day one

| Order | Status        | Blocking? | What it means                                                                            |
|-------|---------------|-----------|------------------------------------------------------------------------------------------|
| 1     | **New**       | no        | A newly created customer or request that has not yet been processed.                     |
| 2     | **Processing**| no        | The customer or request is currently being processed.                                    |
| 3     | **In-Review** | **yes**   | The customer or request is currently under review.                                       |
| 4     | **Pending**   | **yes**   | The customer or request is pending further action or review.                             |
| 5     | **On-Hold**   | **yes**   | The customer or request is on hold and temporarily paused.                              |
| 6     | **Delayed**   | no        | The customer or request has been delayed and is not progressing as scheduled.            |
| 7     | **Cancelled** | no        | The customer or request has been cancelled and will not proceed further.                 |

Three of them — In-Review, Pending, On-Hold — are blocking. The other four are not.

A note on "Delayed": being delayed doesn't make a customer *blocked* in our sense. They're behind schedule, but nothing structural is preventing them from moving forward. Blocking is reserved for situations where there's a deliberate hold, a review gate, or a pause. Delayed is "we're frustrated"; blocking is "someone has to act."

### What we can do with statuses

- Add a new status (display name and blocking flag are the important parts; description and sort order are strongly recommended).
- Edit any field. Editing the **blocking flag** is particularly powerful — see below.
- Delete a status. **Don't allow deleting a status that customers are currently assigned to.** Reassign them first. The system can either prevent the delete or warn loudly enough that nobody accidentally orphans a customer.
- See the list of statuses, sorted by their sort order.
- Open a status and see who's in it.

---

## The "stopped" idea

For every customer, we want to know whether they are currently **stopped**. Stopped means: they're in a status that's blocking, so they can't move forward right now. Stopped customers should look visually distinct in the customer list so we can scan and pick them out without reading.

We never type "stopped" anywhere. The chain is:

1. Each customer points at one status (or none).
2. Each status is either blocking or not.
3. **A customer is stopped if and only if their current status is a blocking one.**

So if we change a customer's status from Pending to Processing, they should immediately stop being stopped — not because we updated a "stopped" field, but because Processing isn't blocking and the system worked it out.

And critically: if we change the *meaning* of a status — say, we decide that Delayed should now be blocking — then **every customer currently in Delayed should become stopped automatically**, with no per-customer updates. The blocking flag lives on the status, not on the customer, so changing it once flips the situation for everybody at once.

A customer with no status is **not** stopped. They're not in a blocking status because they're not in any status at all.

---

## Products

A product is something we sell.

### Fields

- **SKU.** The stock-keeping unit identifier. The thing on a barcode label. Like `WIDGET-001` or `GADGET-200`. SKUs are typed in uppercase by convention. The SKU is the heart of a product's identity.
- **Internal name.** Lowercase, hyphenated version of the SKU, derived automatically. So `WIDGET-001` becomes `widget-001`. This is the handle line items use to point at the product.
- **Display name.** What we want shown in catalogs and on order lines. Like "Standard Widget" or "Pro Gadget" or "USB-C Cable (2m)". This is what humans read.
- **Description.** Long-form description of the product. A paragraph if we want.
- **Unit price.** The default list price per unit. What we charge for one of these.
- **Cost.** What it costs us to acquire one. Internal — not shown on customer-facing surfaces, but used for margin calculations.
- **Stock quantity.** How many we currently have on hand. Just a number.
- **Active flag.** Whether this product is currently sellable. Inactive products stay in the catalog (because old orders still reference them) but shouldn't be selectable on new orders.
- **Order line items.** Not a field we type — the natural reverse list of every line item across every order that references this product. When we open a product, we should be able to see what's been ordered.

### What we can do with products

- Add a new product (SKU is required; everything else is optional but display name, price, and cost are strongly recommended).
- Edit any of the editable fields.
- Delete a product. Same caveat as statuses: a product that's referenced by line items shouldn't be silently deleted, because that would orphan the line items. Mark it inactive instead, or delete the line items first.
- See the catalog as a list, with SKU, display name, price, cost, stock, and active flag.
- Open a product and see everything, including the list of line items that reference it.

### The eight products we want from day one

A small starter catalog:

| SKU          | Display name      | Unit price | Cost   | Stock |
|--------------|-------------------|------------|--------|-------|
| WIDGET-001   | Standard Widget   | $19.99     | $7.50  |       |
| WIDGET-002   | Deluxe Widget     | $39.99     | $14.20 |       |
| GADGET-100   | Pocket Gadget     | $24.50     | $9.10  |       |
| GADGET-200   | Pro Gadget        | $89.00     | $32.00 |       |
| CABLE-USB-C  | USB-C Cable (2m)  | $12.00     | $3.40  |       |
| HUB-7PORT    | 7-Port USB Hub    | $34.99     | $11.80 |       |
| STAND-ALU    | Aluminum Stand    | $45.00     | $18.75 |       |
| CASE-LEATHER | Leather Case      | $65.00     | $22.50 | 0     |

(The Leather Case is intentionally out of stock at zero units. The others have stock counts we'll fill in over time.)

---

## Orders

An order is what happens when a customer buys something. Each order belongs to exactly one customer.

### Identity

- **Order number.** A short identifier we assign — usually a sequential number like `1001`, `1042`. Not globally unique on its own (two customers could each have a "1001"), but unique *per customer*.
- **Internal name.** The customer's id and the order number combined, lowercased and hyphenated. So Bob's order #1001 becomes `bob-1001`, Alice Johnson's order #1010 becomes `alice-johnson-1010`. The system fills this in automatically; we don't type it. This is how the rest of the system points at the order.
- **Customer.** A link to the customer who placed the order. Required.

### When and where

- **Order date.** When the order was placed. A datetime.
- **Order status.** The lifecycle state of the order itself: `New`, `Pending`, `Processing`, `Shipped`, `Delivered`, `Cancelled`, `Returned`. (This is separate from the *customer's* status. They're different things — a customer's status describes our relationship with them, an order's status describes where one specific order is in its lifecycle.) For now this is just a free-text field; we may want to constrain it to a fixed list later.
- **Shipping address.** Where this specific order should ship to. May or may not match the customer's default shipping address — sometimes a customer wants something sent somewhere else.
- **Billing address.** Same idea for billing.

### Money — the part that matters

This is where the system earns its keep. None of these fields are typed by hand. They all come from the line items and the payments.

- **Item count.** How many distinct line items are on this order. (If an order has 3 lines, this is 3, regardless of quantities.)
- **Total quantity.** Sum of the quantities across all line items. (If line 1 is qty 4 and line 2 is qty 2, this is 6.)
- **Subtotal.** Sum of the post-discount line totals across all line items. This is what the order is worth before tax.
- **Tax rate.** A decimal — `0.085` means 8.5%. This is the one money-related field we type by hand on the order itself, because it depends on the jurisdiction and we don't want to compute it. Defaulting to whatever the customer's location implies is a future improvement.
- **Tax amount.** Subtotal times tax rate. Computed.
- **Order total.** Subtotal plus tax amount. Computed. This is the number on the bottom of the invoice. It's the amount the customer owes us for this order.
- **Total paid.** The sum of all *completed* payments against this order. (Pending or failed payments don't count toward total paid until they complete.) Computed.
- **Amount due.** Order total minus total paid, rounded to the nearest cent. Computed. This is the number we care about most — it's what the customer still owes.
- **Is paid in full.** True when amount due is zero or negative. Computed.
- **Payment count.** How many payments have been recorded against this order. Computed.
- **Last payment date.** When the most recent payment came in. Computed.
- **Total refunded.** Sum of refunded amounts across this order's payments. Computed. (We don't refund yet in v3 in any meaningful way — this is here so the field exists when we do.)
- **Payment status label.** A short text label that says what state the order is in payment-wise: `Paid` if fully paid, `Unpaid` if nothing has been paid, `Partial` if some has been paid but not all. Computed.

### Notes and lists

- **Notes.** Free-form text about this order.
- **Order line items.** Not a field — the natural list of the line items belonging to this order.
- **Payments.** Not a field — the natural list of the payments applied to this order.

### What we can do with orders

- Add a new order. Pick a customer (required), pick an order number, pick an order date and status, set the tax rate, optionally type addresses and notes. The order starts empty — no line items yet.
- Edit any of the editable fields.
- Delete an order. Same caveat — line items and payments belong to it, and they shouldn't be silently orphaned.
- See the list of orders, sorted by date with most recent first. The list should show order number, customer, date, status, item count, total, paid, due, and a payment-status label so we can scan for who owes what.
- Open an order and see everything: all the typed fields, all the computed totals, the list of line items inline (with the products they reference), and the list of payments inline.

---

## Order line items

A line item is one product on one order. It's the rows on an invoice.

### Fields

- **Order.** The order this line belongs to. Required.
- **Line number.** Position within the order. Like `1`, `2`, `3`. Combined with the order, this forms the line item's natural key — `bob-1001-line-2`.
- **Internal name.** The order id and the line number combined, lowercased and hyphenated. Filled in automatically. Like `bob-1001-line-2`.
- **Product.** The product being ordered on this line. Required.
- **Quantity.** How many units of this product. An integer.
- **Unit price.** The price we charged for one unit, *captured at the time of ordering*. This may differ from the product's current unit price — we record what we charged then, not what it costs now. Important for historical accuracy.
- **Discount percent.** A decimal — `0.10` means 10% off. Per-line discount. Defaults to 0.
- **Pre-discount.** Quantity times unit price. Computed. This is what the line would cost without the discount.
- **Discount amount.** Pre-discount times discount percent. Computed.
- **Subtotal.** Pre-discount minus discount amount. Computed. This is what the line actually costs after the discount, and this is what flows up into the order's subtotal.
- **Notes.** Free-form text about this specific line.

So when we look at a line item, the four things we type are: the order, the product, the quantity, the unit price (and optionally a discount). Everything else flows from there.

### What we can do with line items

- Add a new line item to an order. Pick the order, pick the product, type a quantity and a unit price. Optionally type a discount and notes.
- Edit any of those typed fields. Everything computed updates immediately, including the parent order's totals.
- Delete a line item. The parent order's totals should drop accordingly.
- See line items as part of an order detail page (the natural place) and as a standalone list for admin purposes.

---

## Payments

A payment is money coming in (or going out, in the refund case) against one specific order.

### Fields

- **Order.** The order this payment is being applied to. Required.
- **Payment number.** Sequential within the order. Like `1`, `2`. Combined with the order, forms the payment's natural key — `bob-1001-pmt-1`.
- **Internal name.** The order id and the payment number combined, lowercased and hyphenated. Filled in automatically.
- **Payment date.** When the payment was received.
- **Amount.** The dollar amount of the payment. Required.
- **Payment method.** How they paid. Free text for now: `CreditCard`, `BankTransfer`, `Cash`, `Check`, `PayPal`, `Other`. We may constrain this to a fixed list later.
- **Payment status.** Where this payment is in its own lifecycle: `Pending`, `Completed`, `Failed`, `Refunded`, `Cancelled`. The two that affect the order's totals are `Completed` (counts toward total paid) and everything else (doesn't count).
- **Transaction id.** External identifier from the payment processor — a Stripe charge id, a wire reference, a check number. Free text.
- **Notes.** Free-form text about this payment.

### Derived fields on a payment

A payment knows things about its parent order, by following the link. We don't copy the order's data onto the payment — we look it up:

- **Is completed.** True when payment status equals `Completed`. Computed.
- **Completed amount.** The payment's amount if it's completed, otherwise zero. Computed. This is what gets summed up into the order's total paid.
- **Order date, order number, order status, order total** — looked up from the linked order, so we can show them on a payment view without having to navigate.
- **Order amount due, order is paid in full** — also looked up. Useful when we're looking at a payment and want to know the state of its parent order without bouncing over.

### What we can do with payments

- Record a new payment against an order. Pick the order, pick the date and method, type the amount and a transaction id, set the status (usually `Completed` for payments that have already cleared, `Pending` for payments we expect to clear soon).
- Edit a payment. Changing its status from `Pending` to `Completed` should immediately roll up into the parent order's total paid, amount due, and paid-in-full state.
- Delete a payment. The parent order's totals should drop accordingly.
- See payments as part of an order detail page, and as a standalone list for admin purposes (sorted by date, most recent first).

---

## What we want to see at the top

When we open the app, before we get to any of the detail views, we want a small dashboard that tells us the temperature of things:

- **Total customers**, and how many are stopped.
- **Total orders**, and a breakdown by order status (how many New, how many Shipped, etc.).
- **Total products**, and how many are active.
- **Total payments**, broken down by payment status, with the dollar total of completed payments shown next to "Completed."
- **Total revenue.** The sum of order totals across all orders.
- **Outstanding.** The sum of amount-due across all orders. The number we lose sleep over.

If "Outstanding" jumps overnight, we want to notice it the moment we open the app.

---

## The starting data we want loaded

We want a realistic-feeling set of seed data so we can use the app from day one and see all the formulas working against actual numbers.

### The seven statuses

The seven statuses listed in the Statuses section, in their sort order: New, Processing, In-Review, Pending, On-Hold, Delayed, Cancelled.

### The eight products

The eight products listed in the Products section.

### Five customers

| Customer        | Status      | Stopped? | Notes                              |
|-----------------|-------------|----------|------------------------------------|
| **Bob**         | On-Hold     | **yes**  | A customer currently in launch.    |
| **Alice Johnson** | Pending   | **yes**  | Initial mock entry for Alice.      |
| **Brian Lee**   | New         | no       | Initial mock entry for Brian.      |
| **Carla Smith** | Delayed     | no       | Initial mock entry for Carla.      |
| **Caroline**    | Processing  | no       | *(none)*                           |

Most of them have plausible-looking email, phone, company, and address fields filled in. Caroline is a solo customer with just an email — no company.

### Seven orders, eighteen line items, nine payments

The seed data should include enough orders, line items, and payments to exercise every formula in the system. The exact composition matters less than the fact that:

- **Multiple customers have multiple orders.** Bob has two orders (one delivered, one pending). Carla has two (one returned, one processing). The other three have one each.
- **Line items use realistic discount percentages** — 0%, 5%, 10%, 15% — so we can verify the discount math works.
- **Payments include all the lifecycle states.** Most are `Completed`. At least one is `Failed` (a credit card decline). At least one is `Pending` (an ACH that hasn't cleared yet). This is important — we want to see the difference between "amount paid" and "completed amount" on day one.
- **At least one order is fully paid in full** (so we see `Paid` in the payment status label).
- **At least one order has partial payment** (so we see `Partial`).
- **At least one order is unpaid** (so we see `Unpaid`).
- **The line item math should actually add up correctly** — if we sum the line subtotals on an order and add tax, we should get exactly the order total. If we sum the completed payment amounts, we should get exactly the total paid. We want to be able to do these checks by hand on day one and have them match.

---

## What we are *not* doing in v3

We want to be clear so nobody runs ahead.

- **No accounting.** We're computing totals and outstanding amounts, but we are not double-entry, we are not booking journal entries, we are not closing periods. This is operational, not financial.
- **No inventory management.** Stock quantity is a number we type. We don't decrement it when an order ships, we don't reorder when it goes low, we don't track lots or serial numbers.
- **No order workflow rules.** Order status is free text (with conventional values). Nothing prevents us from going from "Cancelled" to "Shipped" or any other nonsense. We rely on humans doing the right thing.
- **No payment processing.** We *record* payments after they happen elsewhere. We do not charge cards, we do not initiate wires, we do not connect to Stripe or anything else. The transaction id field exists so we can paste the external reference in.
- **No refunds workflow** — we have a `Refunded` payment status and a `total_refunded` field, but we haven't worked through what they should compute. Treat this as scaffolding.
- **No tax engine.** Tax rate is one number per order, typed by hand. We're not computing tax by jurisdiction or product category.
- **No multi-currency.** Everything is one currency (assume USD). No conversion, no FX rates.
- **No discounts at the order level.** Discounts are per-line only. If we want to discount a whole order, we discount each line.
- **No shipping costs as a separate line.** If we want to charge shipping, we add it as a line item with a "Shipping" product. Crude but works.
- **No history or audit.** We don't record who changed what when. The current state is the only state we know.
- **No users, no logins, no permissions.** Whoever opens the app sees everything and can edit everything.
- **No notifications.** We don't email customers when their order ships. We don't ping ourselves when a payment fails. We don't do reminders.
- **No reports beyond the small dashboard.** No PDF invoices, no CSV exports, no charts, no profit-and-loss.

If any of those sound useful — and several of them obviously do — they're for a later version. v3 is the foundation.

---

## Things we know are imperfect

We want to call these out explicitly so they don't get lost.

1. **Three customer metrics are placeholders.** Average order value, lifetime margin percent, and days since last order all show "Unable to generate formula" as their value. We want them, we know what they should compute, we just haven't worked out the formulas. They are deliberately visible so we can't pretend they're done.

2. **Total refunded is similarly under-specified.** The field exists; what it should sum is a hand-wave for now.

3. **Order status and payment method are free text.** We have conventional values in mind, but the system doesn't enforce them. Typos will silently create new "statuses" that don't match anything.

4. **Sort order on statuses is a number we type.** No drag-and-drop reordering. If we want to insert a new status between Processing and In-Review, we have to renumber by hand.

5. **The system doesn't prevent dependent deletes.** If we delete a status that has customers in it, or a product that has line items referencing it, or an order that has payments against it, the system doesn't currently stop us. We want it to either stop us or warn loudly. This is a top item to fix.

---

## Deletes and dependencies

Because we have a real graph of relationships now, deletes are dangerous. Here's the policy we want, table by table:

- **Delete a customer** → if they have orders, refuse. Reassign or delete the orders first.
- **Delete a status** → if any customers are assigned to it, refuse. Reassign first.
- **Delete a product** → if any line items reference it, refuse. The right move is usually to mark it inactive instead.
- **Delete an order** → cascading delete its line items and payments. The order is the unit; if we're deleting it we mean the whole thing.
- **Delete a line item** → fine. The parent order's totals just drop.
- **Delete a payment** → fine. The parent order's amount-due just goes back up.

The asymmetry here is intentional. Customers, statuses, and products are *referenced* — other things point at them, and we shouldn't yank the rug. Orders are *containers* — they own their lines and payments, so they cascade. Line items and payments are *leaves* — deleting them affects nothing downstream.

---

## How we'll know v3 is right

Open the app cold, with the seed data loaded, and run through this:

### Statuses and stopped behavior

1. The seven statuses are all there with the right display names, descriptions, blocking flags, and sort order. Three blocking, four not.

2. The five customers are loaded, and exactly **two of them — Bob and Alice — are marked as stopped**. The other three (Brian Lee, Carla Smith, Caroline) are not, even though they have non-trivial statuses.

3. Open Bob, change his status from On-Hold to Processing. Bob immediately stops being marked as stopped. The dashboard's stopped count drops from 2 to 1.

4. Change Bob back. The count goes back to 2.

5. Go to the Pending status and uncheck its blocking flag. Without touching any customer, **Alice immediately stops being marked as stopped**. The dashboard count drops to 1.

6. Re-check Pending's blocking flag. Alice is stopped again. Count back to 2.

### Products

7. The eight products are all there with the right SKUs, display names, prices, and costs. Seven are active, one (Leather Case) has stock zero.

8. Open Pro Gadget. The "order line items" list shows every line that references it across all orders.

### Orders, totals, and payments

9. Open Bob's order #1001. Look at the line items table inline. Look at the order's subtotal, tax amount, order total, total paid, amount due, and payment status label. Add the line subtotals up by hand. Add tax. Compare to the order total. They match.

10. Look at the payments inline on the same order. Sum the *completed* payment amounts by hand. Compare to total paid. They match.

11. Find an order with a `Failed` or `Pending` payment. Verify the order's total paid does **not** include that payment's amount. Verify the completed-amount field on the payment is zero, not the amount.

12. Find an order labeled `Paid` in the payment status label, and verify amount due is zero. Find one labeled `Partial` and verify amount due is positive but less than order total. Find one labeled `Unpaid` and verify total paid is zero.

13. On any order, change a line item's quantity. Refresh the order. Subtotal, tax, total, and amount due should all update. The customer who owns the order should see a different total in their orders list.

14. On any order, mark a `Pending` payment as `Completed`. The order's total paid jumps up, amount due drops, and if the payment closes the gap, the payment status label flips to `Paid`.

### Dashboard

15. The dashboard shows correct counts for customers, orders, products, payments. The "Outstanding" number equals the sum of every order's amount due. The "Total revenue" equals the sum of every order's order total. Pick two arbitrary orders, sum their amount-due fields by hand, then check that the "Outstanding" number changes by exactly that amount when those two orders are deleted.

### Deletes

16. Try to delete a status that has customers in it. The system refuses or warns loudly.

17. Try to delete a product that has line items referencing it. Same.

18. Delete an order. Its line items and payments disappear with it. The customer's order count drops by one. The dashboard's outstanding number changes by exactly that order's amount-due.

19. Delete a single line item from an order. The order's subtotal, tax, total, and amount due all drop by the right amount. The product the line item referenced is unaffected.

If all of that works — especially the **stopped flips when a status's blocking flag changes** (step 5) and the **order totals always equal the sum of their parts** (steps 9–14) — then v3 is doing what we asked for.

Those two properties are the whole point. The first is what justifies having statuses as their own table. The second is what justifies having line items and payments as their own tables. If either property fails, the system is just remembering data instead of computing it, and we need to fix that before we trust any of the numbers.
