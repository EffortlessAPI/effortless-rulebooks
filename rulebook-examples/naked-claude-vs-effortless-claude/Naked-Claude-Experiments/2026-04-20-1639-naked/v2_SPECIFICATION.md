# What we want v2 to do

Based on Airtable base: [app7G5emeY7miM4WN](https://airtable.com/app7G5emeY7miM4WN)

## The idea

We want a simple way to keep track of our customers and, at a glance, know which ones we should be paying attention to right now and which ones we should leave alone.

For each customer, we want to remember a few basic things — who they are, any notes we've jotted down about them, and most importantly, **what status they're currently in**. The status is the heart of it. It's our shorthand for "where this customer stands today."

We're not building a full CRM. We don't need invoices, orders, contact info, addresses, or any of that. Right now we just want a customer list, a small set of statuses we can apply to them, and a way to immediately tell which customers are blocked from progressing.

## The customers

A customer, for our purposes, is one row in a list. Each customer has:

- **A name.** Whatever we call them — could be a person's name, could be a company. We're not being formal about it.
- **Some notes.** Free-form. A sentence or two about what's going on with them, what they're working on, why we care. This can be empty.
- **A status.** This is the connection to the other half of the system. Each customer points at one of the statuses we've defined (see below). A customer can also have *no* status assigned at all — that's allowed and it's a meaningful state, just means we haven't categorized them yet.

We also need some way of telling customers apart internally — a short identifier, like a slug or a handle. We don't expect to look at it much, but the system needs it so it can tell two customers with similar names apart. Something like `bob` or `alice-johnson` is fine.

## The statuses

A status, in our world, is a category that describes the situation a customer is currently in. We want a small, fixed-feeling set of these — not arbitrary tags, but a handful of meaningful categories that we manage centrally and apply to customers.

Each status has:

- **A display name.** What we actually want to read on screen — capitalized nicely, with hyphens or whatever formatting makes sense. "On-Hold", "In-Review", "Pending", "Processing", and so on.
- **A short internal name.** This is just the display name in lowercase, derived automatically. We don't type it or maintain it — it comes for free from the display name. It's there so the system has a clean, lowercase handle to refer to the status by.
- **A description.** A sentence or two explaining what this status actually *means*, so anyone using the system understands when to apply it. These descriptions are important — they're how a new person learns what each status is for without having to ask.
- **A "blocking" flag.** This is the key piece. Each status is either *blocking* or *not blocking*. A blocking status means: a customer in this status is stuck — there's something preventing them from moving forward, and someone probably needs to do something about it. A non-blocking status means: things are progressing normally, no special attention needed.

Each status also internally needs an identifier (like `on-hold` or `in-review`) so the customer records can point at it. We use the lowercased, hyphenated form of the display name for this and the system fills it in.

We also want each status to know **which customers are currently using it**. So if you open up "On-Hold" you should see Bob there, because Bob's current status is On-Hold. We don't manually maintain this list — it's just the natural mirror of which customers point at which status.

### The seven statuses we want from day one

We have a specific set of seven statuses in mind, and we want them in the system before we add any customers:

| Status        | Blocking? | What it means                                                                                |
|---------------|-----------|----------------------------------------------------------------------------------------------|
| **New**       | no        | A newly created customer or request that has not yet been processed.                         |
| **Processing**| no        | The customer or request is currently being processed.                                        |
| **Delayed**   | no        | The customer or request has been delayed and is not progressing as scheduled.                |
| **Cancelled** | no        | The customer or request has been cancelled and will not proceed further.                    |
| **In-Review** | **yes**   | The customer or request is currently under review.                                           |
| **Pending**   | **yes**   | The customer or request is pending further action or review.                                 |
| **On-Hold**   | **yes**   | The customer or request is on hold and temporarily paused.                                  |

So out of the seven, three are blocking (In-Review, Pending, On-Hold) and four are not (New, Processing, Delayed, Cancelled). The blocking statuses are the ones we need to pay attention to.

A note on "Delayed": being delayed doesn't make a customer *blocked* in our sense — they're behind schedule, but nothing is structurally stopping them. Blocking is reserved for situations where there's a deliberate hold, a review gate, or a pause. Delayed is a state of frustration; blocking is a state of "someone needs to act."

## The "stopped" idea

Here's the part that connects the two halves.

For every customer, we want to know whether they are currently **stopped**. Stopped means: they're in a status that's blocking, so they can't move forward right now. We want this to show up clearly on the customer list — a stopped customer should look visually different from a non-stopped one, so we can scan the list and spot them without reading every row.

We do **not** want to maintain "stopped" by hand. We don't want a separate stopped checkbox on each customer that we have to remember to flip. That's a recipe for the data drifting out of sync. Instead, we want stopped to be **derived from the customer's status**, which is in turn derived from the status's blocking flag. The chain is:

1. Each customer points at one status (or none).
2. Each status is either blocking or not.
3. A customer is stopped if and only if their current status is a blocking one.

So if we change a customer's status from Pending to Processing, they should immediately stop being stopped — not because we updated a "stopped" field, but because Processing isn't blocking and the system worked it out from there.

And critically: if we ever change the *meaning* of a status — say, we decide that Delayed should now be blocking — then **every customer currently set to Delayed should become stopped automatically**, with no per-customer updates. The blocking flag lives on the status, not on the customer, so changing it once flips the situation for everybody at once. This is the whole point of having statuses as their own thing.

### What about customers with no status?

A customer with no status assigned is **not** stopped. They're not in a blocking status because they're not in any status at all. We may or may not want to do something about them eventually (probably we want every customer to have *some* status), but for now, no status means not stopped.

### What we want to display alongside the status

When we look at a customer, we don't just want to see the internal status handle (`on-hold`). We want to see:

- The status's nicely formatted display name ("On-Hold").
- The status's description, ideally on hover or in the detail view, so we don't have to bounce over to the statuses page to remember what it means.
- Whether the status is blocking, indirectly via the stopped indicator.

All of these come "for free" from the link to the status — we shouldn't be copy-pasting them onto the customer record. They should follow the link.

## What we need to be able to do

For customers:

- **See the list.** Show every customer, with their name, status, notes, and whether they're stopped. The stopped ones should be obvious — visually distinct from the rest.
- **Add a customer.** Type a name, optionally jot notes, optionally pick a status from the existing list, save. The stopped indicator updates immediately based on whichever status we picked.
- **Edit a customer.** Change their name, notes, or status. Changing the status is the main thing we'll do day to day.
- **Delete a customer.** When we're done with them, they're gone.
- **Look at one customer in detail.** A page that shows everything we know about them, including the description of whatever status they're in.

For statuses:

- **See the list of statuses.** All seven (or however many we end up with), with their display name, description, and blocking flag.
- **See which customers are in each status.** Open a status, see the list of customers currently assigned to it.
- **Add a new status.** Type a display name, write a description, decide whether it's blocking, save. The internal name fills in automatically.
- **Edit a status.** Change the display name, the description, or the blocking flag. Changing the blocking flag is the powerful one — it flips the stopped state for every customer in that status at once, and we should expect that to ripple through the customer list immediately.
- **Delete a status.** This is the dangerous operation — what happens to customers currently assigned to it? For v2 we'll say: don't delete a status that has customers in it. Reassign them first. The system can either prevent the delete or warn loudly.

What we do **not** need: search, filtering beyond the obvious "show me only stopped customers" view, sorting beyond a sensible default, bulk edit, import/export, history, audit, or anything fancy. Maybe later.

## What we want to see at the top

When we open the app, before we get to the list, we want a small dashboard that tells us the temperature of things at a glance:

- **Total customers.** Just the count.
- **Stopped customers.** How many are currently in a blocking status. This number is the one we care about most.
- **Status distribution.** How many customers are in each status — On-Hold (3), Processing (8), Pending (1), and so on. This tells us where everyone is at a glance.
- **Status counts.** How many statuses we have configured, and how many of them are blocking. (Mostly there for symmetry — the customer counts matter more.)

If "Stopped customers" jumps from 2 to 5 between Monday and Tuesday, that's the kind of thing we want to notice immediately when we open the app.

## The five customers and seven statuses we already know about

We already have a starting set in mind, and we want all of it loaded from day one.

The seven statuses listed above (New, Processing, Delayed, Cancelled, In-Review, Pending, On-Hold) should all exist before any customer is added.

Then five customers, with these starting assignments:

| Customer        | Status      | Notes                              | Stopped? |
|-----------------|-------------|------------------------------------|----------|
| **Bob**         | On-Hold     | A customer currently in launch.    | **yes** (On-Hold is blocking)  |
| **Alice Johnson** | Pending   | Initial mock entry for Alice.      | **yes** (Pending is blocking)  |
| **Brian Lee**   | *(none)*    | Initial mock entry for Brian.      | no (no status)  |
| **Carla Smith** | Delayed     | Initial mock entry for Carla.      | no (Delayed is not blocking) |
| **Caroline**    | Processing  | *(empty)*                          | no (Processing is not blocking) |

So when we first open the app, we should see **five customers, two of them flagged as stopped** (Bob and Alice). The status distribution will look like: On-Hold has Bob, Pending has Alice, Delayed has Carla, Processing has Caroline, and Brian has no status. New, In-Review, and Cancelled all start out empty.

If we open the app for the first time and the counts don't match that, something's wrong.

## What we are *not* doing in v2

We want to be clear about this so nobody runs ahead. v2 includes the customer list, the status list, the link between them, and the derived "stopped" indicator. v2 does **not** include:

- Email addresses, phone numbers, mailing addresses, company names, or any contact details on customers.
- Orders, invoices, payments, line items, products, prices, taxes, or anything money-related.
- Multiple statuses per customer, or status history, or "previous status." A customer has exactly one current status (or none) and we don't remember what it used to be.
- Workflow rules — there's no "you can only go from Pending to Processing", no allowed transitions, no required approvals. Any status can be assigned to any customer at any time.
- Sort order on statuses, color coding on statuses, icons, or any visual customization beyond the display name and the blocking flag.
- Audit history of who changed what and when.
- Users, logins, permissions, multi-tenancy, or sharing. Whoever opens the app sees everything and can edit everything.
- Notifications, reminders, alerts.
- Reports, charts, or analytics beyond the small dashboard at the top.

If any of those sound useful, great — they're for a later version. v2 is deliberately small.

## How we'll know v2 is right

The simplest way to check: open the app cold, with the seed data loaded, and:

1. The seven statuses are all there with the right display names, descriptions, and blocking flags. Three of them — In-Review, Pending, On-Hold — are marked as blocking. The other four are not.

2. The five customers are all there with the right names, notes, and starting statuses. Exactly **two of them — Bob and Alice — are marked as stopped**, and they look visually distinct from the other three. Brian shows no status. Carla shows Delayed but is not stopped. Caroline shows Processing and is not stopped.

3. Open Bob, change his status from On-Hold to Processing, save. Bob should immediately stop being marked as stopped, because Processing is not a blocking status. The "stopped" count on the dashboard should drop from 2 to 1.

4. Change Bob back to On-Hold. He should be stopped again. The dashboard count should go back to 2.

5. Open Brian (who has no status), set his status to In-Review, save. Brian should now be marked as stopped. The dashboard count should go to 3.

6. Now go to the **statuses** page, open Pending, and uncheck its "blocking" flag. Without touching any customer record, **Alice should immediately stop being marked as stopped**, because the rule changed underneath her. The dashboard count should drop to 2 (just Bob and Brian now, since Alice's status — Pending — is no longer blocking).

7. Re-check Pending's blocking flag. Alice should be stopped again. Dashboard back to 3.

8. Add a brand new customer named "Test", assign them the status On-Hold, save. They should appear in the list, marked as stopped. Dashboard goes to 4.

9. Try to delete the On-Hold status while customers are still assigned to it. The system should refuse, or at minimum warn loudly. Bob and Test are still pointing at it.

10. Reassign Bob and Test to a non-blocking status (say, Processing), then delete On-Hold. The status should disappear from the statuses list, and the customers that used to point at it now have a different status.

11. Delete the Test customer. They should disappear and the customer count should drop by one.

If all of that works, v2 is doing what we asked for.

The most important behavior to verify is **step 6** — the one where editing the status flips the stopped state of customers who depend on it. That's the behavior that justifies having statuses as their own thing in the first place. If that doesn't work, the system isn't actually deriving stopped from the status — it's caching it, and we need to fix that.
