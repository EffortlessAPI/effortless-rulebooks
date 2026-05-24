# A Tale of Two Claudes — Predicted Outcomes Across v1, v2, v3

I've read all three specs ([v1](v1_SPECIFICATION.md), [v2](v2_SPECIFICATION.md), [v3](v3_SPECIFICATION.md)), the project [CLAUDE.md](CLAUDE.md) ("DO NOT USE EFFORTLESS SKILLS"), and [disabled_claude.md](disabled_claude.md) — yes, the effortless skills are present in the environment but explicitly disabled for this working directory. The three Airtable bases (`appgjoEcFNxluhbvK`, `app7G5emeY7miM4WN`, `appKLygCIXweUUKtM`) are wired into [effortless.json](effortless.json) so the same experiment can be run end-to-end against three increasingly demanding schemas.

Below is what I'd expect to happen if you actually ran each version through both Claudes.

---

## The setup, restated

- **Naked Claude** gets the spec + Airtable credentials and writes a backend, an API, a frontend, and seed logic by hand. Every rule — "stopped = color is Green," "blocking flag flips stopped for everyone," "order total = sum of line subtotals + tax" — is something Naked Claude has to *implement* in code, somewhere, in a language of its choosing.
- **Effortless Claude** runs four transpilers first:
  - `airtable-to-rulebook` → captures the schema, formulas, lookups, rollups as `effortless-rulebook.json`
  - `rulebook-to-postgres-with-rls` → a real Postgres schema with views, RLS, and the formulas already wired
  - `rulebook-to-docs` → docs derived from the same source
  - `rulebook-to-restapi` → a working REST API on top of the views

  Then it only writes the *thin glue*: a UI, and any rule that the rulebook doesn't already encode.

The crucial asymmetry: if the semantics are modeled in Airtable (formulas, lookups, rollups, blocking flags as data), Effortless Claude inherits them for free. Naked Claude has to re-derive them by reading the spec.

---

## v1 — Customers + a color-driven `stopped` flag

**Spec essence:** 5 customers, a color field, and "stopped iff color == 'Green'" (intentionally inverted from a stoplight, exact-match string).

### Naked Claude
- Picks a stack (likely Node/Express + SQLite or similar), invents a `customers` table, writes CRUD endpoints, writes a frontend.
- Implements `stopped` somewhere — and here's the first fork in the road:
  - **Likely mistake #1:** stores `stopped` as a column and updates it on write. Works on day one, drifts the moment anyone touches the DB directly.
  - **Likely mistake #2:** case-insensitive comparison ("looks more correct"). Spec explicitly says exact "Green" — Naked Claude reads quickly and "improves" it.
  - **Likely mistake #3:** uses traffic-light intuition and inverts the rule (Red = stopped). The spec warns about this *because it's the obvious mistake*.
- Seeds the five customers in code or a migration.
- Final result: a working app, probably correct on the happy path, with a 30–50% chance of one of the three subtle bugs above. Token spend: moderate-to-high — every layer (model, API, UI, seed) is hand-written.

### Effortless Claude
- Runs the four transpilers. `stopped` already exists as a calculated field in Airtable (a formula `{Color} = "Green"`), so it shows up in the rulebook, in the generated Postgres view (`vw_customers.stopped`), and in the REST API automatically.
- Seed data is already *in Airtable* — no seeding code needed.
- Effortless Claude only writes the UI: a list view that reads from `vw_customers`, a detail view, an add/edit form. The "stopped iff Green" rule is never written in code by Claude — it lives in exactly one place (the Airtable formula) and flows through the rulebook to the view.
- Can't get the inversion or the case-sensitivity wrong, because it doesn't write the rule at all.

**v1 verdict:** both finish. Naked Claude's app *probably* works and *might* have a subtle stopped-bug. Effortless Claude's app is structurally incapable of having that bug. Token delta is modest at this scale — maybe 1.5–2x in Naked Claude's favor for sheer volume of generated code, but Effortless Claude spent some tokens running transpilers.

---

## v2 — Customers + Statuses + derived blocking

**Spec essence:** Statuses are their own table with a `blocking` flag. `customer.stopped` is now a *two-hop* derivation: customer → status → blocking. The acceptance test (step 6) is the killer one: **toggle a status's blocking flag and every customer in that status flips, with no per-customer writes.**

### Naked Claude
- This is where the divergence gets sharp. Naked Claude has to decide *where* `stopped` lives.
  - **Tempting wrong answer:** denormalize `is_blocking` onto each customer at write time. Step 6 fails — toggling the status flag doesn't ripple. The spec literally calls this out: *"If that doesn't work, the system isn't actually deriving stopped from the status — it's caching it, and we need to fix that."*
  - **Correct answer:** compute `stopped` in a JOIN (or a view) at read time. Naked Claude *can* get this right, but it requires resisting the natural instinct to "just add a column."
- Status CRUD has to be written: list, add, edit, delete-with-guard ("don't delete a status that has customers in it").
- The dashboard rollups (count by status, count of stopped) are hand-written aggregate queries.
- Seed data: 7 statuses + 5 customers, all in code.
- Token spend: substantially higher than v1. Two tables, two CRUD surfaces, derived state, rollups, referential guards. Probably 3–5x v1.
- **Probability of getting step 6 right on the first try: maybe 50–60%.** It's the kind of thing a careful engineer would catch, but a quickly-generated app would miss.

### Effortless Claude
- The seven statuses are rows in an Airtable table. The blocking flag is a checkbox column. `customer.stopped` is a *lookup* on the customer's linked status, returning that checkbox. The dashboard counts are rollups.
- All of that exists in Airtable before any code is written. The transpilers turn it into:
  - `vw_statuses`, `vw_customers` (with `stopped` as a column already correctly derived)
  - REST endpoints for both
  - A consistent FK relationship enforced in Postgres
- Step 6 — the killer test — passes by construction. There is no cached `stopped` to invalidate. Toggling the blocking checkbox in Airtable (or via the API) updates one cell, and every read of `vw_customers` instantly reflects it for everyone in that status.
- Effortless Claude writes the UI (customers page, statuses page, dashboard) and nothing else. The dangerous-delete guards live in the rulebook / view layer, not in the UI.
- **Probability of getting step 6 right: ~100%**, because the rule is data, not code.

**v2 verdict:** the gap widens decisively. Naked Claude builds *more app* (more code, more places for the bug to live), and the spec's hardest acceptance criterion is exactly the one that's hardest to get right when you write it by hand. Effortless Claude builds *less* code and the load-bearing rule is unwritable-incorrectly. Token delta probably 3–5x in Effortless Claude's favor, and quality delta is much larger than that.

---

## v3 — Customers, Statuses, Products, Orders, Line Items, Payments

**Spec essence:** Six tables. Real referential structure. Money. Computed totals everywhere — `subtotal`, `tax_amount`, `order_total`, `total_paid` (only counting `Completed` payments), `amount_due`, `is_paid_in_full`, `payment_status_label`. Plus the v2 stopped-from-blocking machinery still has to work. Plus delete policies (refuse / cascade / leaf) that differ per table. Plus three explicitly-acknowledged placeholder formulas (avg order value, lifetime margin, days since last order) that are *supposed* to render the literal string "Unable to generate formula."

This is where the experiment really earns its keep.

### Naked Claude

Naked Claude is now writing a small ERP. The things that have to be correct, all of them by hand:

1. Six tables, six CRUD surfaces, foreign keys.
2. **A computation graph.** `line_item.subtotal` → `order.subtotal` → `order.tax_amount` → `order.order_total`. `payment.completed_amount` (conditional on status) → `order.total_paid` → `order.amount_due` → `order.is_paid_in_full` and `payment_status_label`. Every one of these has to be implemented somewhere (DB triggers? application-layer aggregation? recompute-on-read?). Each strategy has tradeoffs and failure modes.
3. **The "only completed payments count" rule.** A test step explicitly checks that a `Pending` or `Failed` payment is *not* in `total_paid`. Easy to miss when writing a `SUM(amount)`.
4. **Per-table delete policies.** Customers refuse-if-orders. Statuses refuse-if-customers. Products refuse-if-line-items. Orders cascade. Line items and payments leaf-delete. Six different rules, six places to forget one.
5. **The placeholder formulas.** The spec wants three calculated fields to literally display "Unable to generate formula." A hand-coding Claude is overwhelmingly likely to either (a) implement them anyway (overshooting the spec) or (b) leave them blank/null instead of showing the sentinel string. *Both* are wrong by the spec.
6. **Round-trip math acceptance tests** (steps 9–14): if you sum line subtotals by hand and add tax, you should get the order total exactly. Floating-point and rounding bugs love this kind of test. Naked Claude needs to be careful about cents-vs-dollars and rounding mode.
7. The v2 stopped-from-blocking ripple still has to work (step 5).
8. Dashboard rollups across orders, payments, outstanding totals.
9. Seed data: 7 statuses + 8 products + 5 customers + 7 orders + 18 line items + 9 payments, all *with internally consistent math* so the day-one acceptance checks pass.

Realistically: Naked Claude can build this. It will take a lot of tokens (estimate 5–10x the v2 effort), it will likely produce a working app, and it will almost certainly have **at least 2–3 subtle bugs** out of: completed-payments rule, the cached-vs-derived stopped issue resurfacing under the new schema, a delete-policy inconsistency, a rounding error in tax/amount-due, or the placeholder-string requirement being silently "fixed."

The seed data alone is a trap — generating 18 line items and 9 payments such that *every* acceptance check (one fully-paid order, one partial, one unpaid, one with a failed payment, totals adding up to the cent) passes is a meaningful piece of arithmetic to get right by hand.

### Effortless Claude

Effortless Claude does almost no new work beyond what it did for v2.

- The whole computation graph (`line.subtotal`, `order.subtotal`, `order.tax_amount`, `order.order_total`, `order.total_paid` with `IF(payment.status="Completed", amount, 0)`, `order.amount_due`, `order.is_paid_in_full`, `order.payment_status_label`) is *Airtable formulas and rollups*. They're authored once, in the base, and the rulebook captures them. The generated Postgres views compute them in SQL the same way. The REST API exposes them. There is no place in Claude-written code where the rule "only completed payments count" can be forgotten — it lives in one cell of one rollup definition.
- The placeholder formulas show up as fields with no formula attached, and the rulebook/transpiler convention is to render them as the literal "Unable to generate formula" — so they're correct *by being unfinished*, which is the only way the spec allows them to be correct.
- Delete policies are encoded in the rulebook's relationship definitions and enforced at the view/RLS layer.
- Seed data lives in Airtable, where the formulas are *already evaluating it as you type*. You don't generate self-consistent seed data; you put the line items in and the totals appear, guaranteed correct.
- The v2 stopped-from-blocking ripple keeps working unchanged, because it's the same lookup it was in v2.
- Effortless Claude writes: customer list/detail, order list/detail (with line items and payments inline), product catalog, status page, dashboard. All read from `vw_*` views. Zero business logic in UI code.

**The killer property:** the v3 spec ends by saying *"those two properties are the whole point. The first is what justifies having statuses as their own table. The second is what justifies having line items and payments as their own tables. If either property fails, the system is just remembering data instead of computing it, and we need to fix that before we trust any of the numbers."* Effortless Claude cannot fail those properties — they're the ground truth its app reads from. Naked Claude must consciously implement them, and the spec's own author clearly expects this to be the most likely failure mode.

**v3 verdict:** the gap is now an order of magnitude. Naked Claude writes a *lot* of code and ships at least a few subtle bugs in the highest-stakes part of the system (the money math). Effortless Claude writes a UI on top of a backend it didn't have to author and ships a system whose correctness is guaranteed by where the rules live.

---

## Token usage, in rough proportions

Holding the spec constant, expect roughly:

| | Naked Claude | Effortless Claude |
|---|---|---|
| v1 | 1.0x | ~0.7x |
| v2 | ~3x | ~1x |
| v3 | ~10x+ | ~1.5x |

The Effortless curve stays nearly flat because the *additional* work between v1 and v3 is almost all schema-and-formulas — exactly what the rulebook absorbs. The Naked curve is super-linear because every new derived field is new code, every new table is new CRUD, and every cross-table formula multiplies the surface area for bugs.

---

## What I'd actually expect if you ran the experiment

1. **v1:** both finish. Naked Claude has a real (~30%) chance of an inverted or case-sensitive `stopped` rule. Effortless Claude can't have either bug. Token delta is modest.

2. **v2:** both finish. Naked Claude has a serious (~40–50%) chance of failing acceptance step 6 — the "toggle blocking, watch the ripple" test — because the natural implementation is to cache `stopped` on the customer. Effortless Claude passes by construction. Token delta becomes meaningful (3–5x).

3. **v3:** both finish, but they finish *very* differently. Naked Claude produces a much larger codebase, takes much longer, and almost certainly has 2+ correctness issues somewhere in the money math, the completed-payments-only rule, the delete policies, the placeholder rendering, or the v2 ripple resurfacing under the new schema. Effortless Claude produces a thin UI over generated infrastructure, with the load-bearing properties guaranteed by the rulebook. Token delta is dramatic (≥10x), and *quality* delta is larger still — Effortless Claude's failure modes at v3 are limited to "the UI is ugly" or "I forgot to wire a button," whereas Naked Claude's failure modes include "the company's outstanding-receivables number is wrong."

---

## The deeper implication

The interesting thing isn't that Effortless Claude is faster or cheaper — it's *what kind of mistakes each Claude is structurally able to make.*

Naked Claude can make any mistake the spec doesn't forbid, plus several the spec explicitly forbids (because the prohibitions are in prose, not in the type system). Its failure surface grows with the size of the app.

Effortless Claude can only make mistakes in the layers it actually authors — and those layers (UI, glue, anything not modeled in Airtable) shrink as a fraction of the total system as the spec gets bigger. The rulebook is a *correctness floor* that rises with the schema. By v3, the floor is high enough that the only mistakes left for Claude to make are cosmetic.

That's the real moral of the tale: the road less travelled isn't a different language or a different framework — it's *moving the rules out of code and into a place where there's nothing for an LLM to misremember.*

And finally, the meta-observation about the two new rules in the original framing ("widgets are safety equipment iff red, safety equipment gets a yellow background") — Naked Claude implements that as `if (widget.color === 'red') ... ` somewhere in render code, where it lives forever as a hardcoded business rule in the UI. Effortless Claude adds an `IsSafetyEquipment` formula field and a `BackgroundColor` lookup in Airtable, the rulebook picks them up, the views expose them, and the UI just reads `widget.background_color`. Tomorrow, when the rule changes to "blue is also safety equipment," Naked Claude needs a code change and a deploy. Effortless Claude needs someone to edit a formula cell.

That's the divergence in one sentence: **with Naked Claude, every rule is a line of code; with Effortless Claude, every rule is a row.**
