# What we want v1 to do

Based on Airtable base: [appgjoEcFNxluhbvK](https://airtable.com/appgjoEcFNxluhbvK)

## The idea

We want a simple way to keep track of our customers and, at a glance, know which ones we should be paying attention to right now.

For each customer, we want to remember a few basic things — who they are, any notes we've jotted down about them, and a color that tells us how things are going with them. The color is the important part. It's our shorthand for "here's the situation."

We're not building a full CRM. We don't need invoices, orders, contact info, addresses, or any of that. Not yet. Right now we just want the list, the colors, and a way to tell at a glance who's stopped.

## The customers

A customer, for our purposes, is one row in a list. Each customer has:

- **A name.** Whatever we call them — could be a person's name, could be a company. We're not being formal about it.
- **Some notes.** Free-form. A sentence or two about what's going on with them, what they're working on, why we care. This can be empty.
- **A current color.** This is the heart of it. Each customer has one color at a time, and that color summarizes how things stand with them right now. We'll mostly use red, green, yellow, and blue, but we want to be able to type in something else if we need to. We change the color whenever the situation changes.

We also need some way of telling customers apart internally — a short identifier, like a slug or a handle. We don't expect to look at it much, but the system needs it so it can tell two customers with similar names apart. Something like `bob` or `alice-johnson` is fine.

## The "stopped" idea

Here's the part that matters most.

Some customers are **stopped**. When a customer is stopped, that means we're not actively moving forward with them right now — they're paused, on hold, parked, whatever you want to call it. We want to be able to spot stopped customers immediately when we look at the list, without having to read every row carefully.

We don't want to *separately* mark a customer as stopped, because then we'd have to remember to update two things every time something changed. Instead, we want **stopped to be derived from the color**. The color is the one source of truth. If we change the color, the stopped flag updates by itself.

The rule is: **a customer is stopped when their current color is Green.**

That's it. Green means stopped. Anything else — red, yellow, blue, no color at all — means they're not stopped.

(Yes, we know that's the opposite of what a stoplight does. We picked Green on purpose because in our world "green" means "they're in a green field, they're parked safely, nothing's happening." If that ever gets confusing we can revisit, but for now Green = stopped, and we want it written down clearly so nobody changes it by accident.)

The important things about this rule:

- It's automatic. Nobody types "stopped: yes" anywhere. The system figures it out from the color.
- It's exact. The color has to be Green. Not "green", not "GREEN", not "Greenish" — exactly "Green" the way we type it in. If we get sloppy with capitalization, the customer will quietly stop being marked as stopped, and that's on us.
- It only depends on the color. Nothing else about the customer affects whether they're stopped.

## What we need to be able to do

We need to be able to:

- **See the list.** Show every customer, with their name, color, notes, and whether they're stopped. The stopped ones should be obvious — visually different from the rest, so we can scan the list and pick them out without reading.
- **Add a new customer.** Type a name, optionally jot some notes, pick (or type) a color, and save. The system should immediately know whether they're stopped based on the color we picked.
- **Edit a customer.** Change their name, change their notes, change their color. When we change the color, the stopped indicator should update right away — no extra step.
- **Delete a customer.** When we're done with them, they're gone. We're not worried about archiving or undo for v1.
- **Look at one customer on their own.** Click into a customer and see all of their information on a page by itself. Useful when we want to focus.

We do *not* need search, filtering, sorting beyond a simple alphabetical default, bulk edit, import, export, or any reporting beyond "how many customers are stopped right now." Maybe later. Not now.

## What we want to see at the top

When we open the app, before we get to the list, we want a small summary. Just enough to tell us the temperature of things:

- How many customers we have in total.
- How many of them are currently stopped.
- A quick breakdown of how the colors are distributed — how many Greens, how many Reds, how many Yellows, how many Blues. We want to see this at a glance because the color mix tells us a lot about where we're putting our attention.

## The five customers we already know about

We already have five customers in mind, and we want them in the system from day one as starting data:

- **Bob** — currently in launch. Color: Green. (So: stopped.)
- **Alice Johnson** — an early entry. Color: Green. (Stopped.)
- **Brian Lee** — an early entry. Color: Blue. (Not stopped.)
- **Carla Smith** — an early entry. Color: Yellow. (Not stopped.)
- **Caroline** — no notes yet. Color: Red. (Not stopped.)

So when we first open the app, we should see five customers, two of them flagged as stopped, with the colors split as 2 Green, 1 Blue, 1 Yellow, 1 Red. If we open it for the first time and we don't see exactly that, something's wrong.

## What we are *not* doing in v1

We want to be clear about this so nobody runs ahead. v1 is just the customer list and the color-driven stopped flag. v1 does **not** include:

- Email addresses, phone numbers, mailing addresses, company names, or any other contact details.
- Orders, invoices, payments, line items, products, prices, taxes, or anything money-related.
- Status workflows beyond the single color field — no "stage", no "pipeline", no multi-step process.
- History — we don't track when the color changed, who changed it, or what it was before. The current color is the only color we know about.
- Users, logins, permissions, or sharing. Whoever opens the app can see and edit everything.
- Notifications, reminders, alerts, or anything that pings us when a customer changes.
- Reports, charts, exports, or analytics beyond the small summary at the top.

If any of those sound useful, great — they're for a later version. v1 is deliberately small so we can start using it immediately and learn what we actually need from there.

## How we'll know v1 is right

The simplest way to check: open the app cold, and:

1. The five customers above are all there with the right names, notes, and colors.
2. Exactly two of them — Bob and Alice — are marked as stopped, and they look visually distinct from the other three.
3. Open Brian Lee, change his color from Blue to Green, save. He should now be marked as stopped, without us having touched anything else.
4. Change him back to Blue. He should stop being marked as stopped.
5. Add a brand new customer named "Test", give them the color Green, save. They should appear in the list, and they should be marked as stopped.
6. Delete the test customer. They should disappear and the count should go back to five.
7. The summary at the top should reflect the right counts at every step.

If all of that works, v1 is doing what we asked for.
