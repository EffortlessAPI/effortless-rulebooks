# Domain UX Vision — what's left

> Living plan for the Effortless admin portal's domain picker and per-domain
> landing experience. The original v1 spec has been partially shipped; this
> document is now the **remainder**. Everything below is open work.

---

## What already shipped (do not re-plan)

The pruning principle, the rulebook `_meta` extensions, the picker trailer,
the reception desk (description, telemetry, use cases, journal seed), the
cast + projects-in-flight scrollers filtered by `important: true`, the
rules-that-matter panel with click-to-expand `explanation_rich` + worked
example, six motif palettes (`skyline`, `lcars`, `corkboard`, `legal-pad`,
`atlas`, `default`), and one mobile breakpoint at 768px. The reception desk
replaced the 12-tile critique page.

Reference implementations live at
[DomainTile.jsx](effortless-platform/admin-portal/client/src/components/DomainTile.jsx),
[DeveloperHomeScreen.jsx](effortless-platform/admin-portal/client/src/screens/developer/DeveloperHomeScreen.jsx),
[DeveloperDomainScreen.jsx](effortless-platform/admin-portal/client/src/screens/developer/DeveloperDomainScreen.jsx),
[RichText.jsx](effortless-platform/admin-portal/client/src/components/RichText.jsx),
and the styles section in
[index.css](effortless-platform/admin-portal/client/src/index.css) labeled
*Domain UX*.

Only one rulebook —
[acme-corporation-rulebook.json](rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json) —
has `_meta` authored. Every other domain falls back to `motif: "default"`
and its `RulebookFlavors` tagline.

---

## 1. What ACME Corporation actually is (reference, do not edit)

Source: [rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json](rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json)

- Five entities: `Client` (3 rows), `Projects` (3), `Employees` (3),
  `Roles` (3), `TypesOfProject` (4).
- Business story: a small professional-services shop. Some project types
  (Compliance, ClientFacing) require manager sign-off; only employees in
  manager roles can approve. The DAG wires that workflow.
- `_meta` is fully authored — this is the **reference rulebook** for what
  the experience should look like when curation is complete. Use it as a
  template when authoring `_meta` for the others.

---

## 2. Open work — in priority order

Each item below is its own deliverable. Ship in order; each unlocks the
next without backtracking.

### 2.1 Propagate `_meta` to the other demo rulebooks

The §8 test (no two domains feel the same) is currently satisfied only for
ACME vs. everything else. The other demos still render `motif-default` and
generic flavor taglines. Author `_meta.tagline`, `motif`, `motif_palette`,
`description_rich`, `use_cases`, `signature_rows`, `journal_seed`,
per-entity `important` / `summary_rich` / `important_fields`, and per-rule
`important` / `explanation_rich` for:

- `acme-llc` (motif: `legal-pad`)
- `star-trek` (motif: `lcars`)
- `jessica-basic` and `jessica-advanced` (motif: `corkboard`)
- `is-everything-a-language` (motif: `atlas`)
- `customer-fullname`, `effortless-rulesbooks`, `acme-corporation` already
  has `_meta`

This is **content work**, not code work. The renderer is finished.

### 2.2 Substrate witness chips in the hero

Today `_meta.substrates[]` is read by no one. Add a chip row to the
reception hero showing only substrates flagged `important: true` from
`_meta.substrates[]`. Each chip:

- Renders green when the substrate's most recent build succeeded.
- Opens a panel asking the same question (e.g. *"how many projects are
  awaiting approval?"*) and showing the same answer rendered in that
  substrate's dialect (SQL, Python expression, OWL axiom, Excel formula).
- During a rebuild, briefly turns amber, then relights green.

Hide the chip row entirely if no substrate is flagged `important`.
Overflow `…` chip opens the full substrates panel for domains with many.

### 2.3 Rich-text editor in the portal for `*_rich` fields

Today `*_rich` fields are read-only — anyone wanting to edit them has to
modify the rulebook JSON by hand. Add a lightweight inline editor (bold,
italic, lists, inline code, links — no images, no tables, no nesting) that
writes back to the rulebook JSON in place. No drafts. Git is the undo
history.

Apply uniformly to: `_meta.description_rich`, each item in
`_meta.use_cases`, each entity's `summary_rich`, each rule's
`explanation_rich`.

### 2.4 Picker overhead chips

Above the gallery on the picker:

- **Last visited** — the domain the current user was in most recently.
- **New since you were here** — any rulebook whose JSON changed since the
  user's `last_seen_rulebook_revision`, or whose tests went red.
- **Bootstrap a new one** — expands inline into the existing Shadle-steps
  flow (not a separate page).

Requires server-side `user_domain_state` table (see §2.6) for the first
two; the third just toggles an inline panel.

### 2.5 Cmd-K command palette

Persistent input at the top of the portal. Type *"sarah"* and Sarah Chen
surfaces with every row that names her. Type *"due this week"* and
matches return as a grouped list. Type *"requires approval"* and the rule
itself is the top result. Featured (`important: true`) entities and rules
sort to the top.

Implementation: client-side index of the active rulebook's data and
schema, refreshed on rulebook reload. No new server endpoints needed for
v1.

### 2.6 State preservation (per-user, per-domain route memory)

Storage:
- `erb_admin_portal` gets a `user_domain_state` table:
  `(user_id, domain, last_route, last_visited_at, last_seen_rulebook_revision)`.
- Diff for the "Welcome back" journal entry is computed server-side by
  comparing `last_seen_rulebook_revision` to the current rulebook JSON.
- Browser local storage may be used only as a first-paint optimization.

UX:
- Switching domains restores the user to the exact route they last had
  open inside that domain.
- The reception desk's journal area shows a real diff entry when one
  exists; falls back to `journal_seed` only on first visit.

### 2.7 Time-travel scrubber (read-only)

A timeline ribbon at the bottom of the domain interior. Drag left and
the page rewinds to that historical state; cast strip reshuffles, project
approval states revert, substrate witness chips re-light against the
historical query.

Demarcation:
- Sticky banner across the top: `⏰ Viewing state as of <date> — Return to now →`
- Page background tints faintly amber.
- Every editable affordance is disabled.
- "Return to now" button is large and persistent.

Editing in the past is explicitly out of scope for v1.

### 2.8 Tour mode

A "Take the tour" button on the reception hero. 90-second auto-narration:
use cases → important cast → important projects → most-important rule
firing → substrate chips agreeing → timeline showing the same answer six
months ago. No interestingness heuristic — walk the `important: true`
items in page order, reading each one's `*_rich` field.

Same affordance, two audiences: demo button + new-developer onboarding.

### 2.9 Three-breakpoint responsive layout

Only one breakpoint exists today (`@media (max-width: 768px)`). Full v1
wants three:

- **Phone** (≤ 640px): single vertical stack of horizontal scrollers.
  Cmd-K hides behind a 🔍 button.
- **Tablet portrait** (641–1024px): hybrid two-column under the hero —
  left column Cast + Projects, right column Use cases + Rules. Substrate
  chips and timeline span full width.
- **Desktop** (≥ 1025px): the magazine grid we already have.

### 2.10 Rules glow + last-fired metadata

The rules-that-matter panel renders today, but each row is static. The
vision asks for a "receipt printer" — a faint glow trails on rules that
fired in the last 60 seconds; each row shows when it last fired and what
it touched. *"When Mike was promoted, the approval rule fired across 3
projects. 1 changed state."*

Requires either rulebook-level bitemporal change history (the
`last_seen_rulebook_revision` infrastructure from §2.6 can power this) or
an event stream the portal subscribes to.

---

## 3. The test (unchanged from the original)

When the user closes the tab, they should be able to describe ACME
Corporation to a colleague in one sentence. If they can't, the page
failed.

The harder test: if the user opens *any two domains* and they feel the
same, the page failed. Each domain's rulebook should put enough of itself
forward — through tagline, motif, use cases, important entities,
important rules, signature rows — that the two pages read as different
small worlds, not as two instances of the same template.

This test is satisfied for ACME today. §2.1 is what extends it to the
rest of the gallery.
