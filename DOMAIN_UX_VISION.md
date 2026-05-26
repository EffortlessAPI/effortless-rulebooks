# Domain UX Vision

> A standalone design document for the Effortless admin portal's **domain picker** and **per-domain landing experience**. This is the **full v1 spec**. There is no v2. There is no future. Everything described here ships.

---

## 0. Why this document exists

The current Developer → Domain landing page is roughly 40 in² of screen real estate that says, in effect, *"5 tables · Rulebook generated from Airtable base"* and then renders twelve identical clickable cards that duplicate the left navigation. It treats the user like a dev-ops auditor scanning counts, not a human exploring a business.

This document captures the **target experience** for two adjacent surfaces:

1. **The domain picker** — landing on the portal and choosing a domain.
2. **The domain interior** — once you've picked one (`acme-corporation`, `acme-llc`, `star-trek`, `jessica-basic`, etc.), how it feels to live inside it.

The document is deliberately *experience-first*. It does not prescribe components, routes, or data plumbing. Those come after the tone is agreed on.

---

## 1. What ACME Corporation actually is (the data on disk, so future sessions don't re-derive it)

Source: [rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json](rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json)

- **Name (in JSON):** `ACME Corporation (template)`
- **Description (in JSON):** *"Rulebook generated from Airtable base 'ACME Corporation (template)'."* — generic, devops-flavored, needs a better human-facing description either in the rulebook or computed at render time.
- **Five entities:**

  | Entity | Rows | Notable fields | Real sample identifiers |
  |---|---|---|---|
  | `Client` | 3 | ClientId, FirstName, LastName, EmailAddress, FullName | cust0001, cust0002, cust0003 |
  | `Projects` | 3 | ProjectId, Name, IsApproved, ApprovedBy, ProjectTypeName, DueDate | *Internal Tools Dashboard, Customer Portal Redesign, GDPR Compliance Audit* |
  | `Employees` | 3 | EmployeeId, Name, Role, RoleIsManager, EmailAddress | *Sarah Chen, Mike Johnson, Priya Patel* |
  | `Roles` | 3 | RoleId, Name, IsManager, CountOfEmployees | *Developer, ProjectManager, Analyst* |
  | `TypesOfProject` | 4 | TypesOfProjectId, Name, RequiresManagerApproval, CountOfProjects | *Internal, ClientFacing, Research, Compliance* |

- **The business story this models:** a small professional-services shop. Projects have types; some types (Compliance, ClientFacing) require manager sign-off. Employees hold roles; only ProjectManager roles can approve. The rulebook's calculated fields wire that approval workflow together.

That story — the *meaning* of the domain — is currently invisible on the landing page.

---

## 2. Critique of the current page

Current implementation: [effortless-platform/admin-portal/client/src/screens/developer/DeveloperDomainScreen.jsx](effortless-platform/admin-portal/client/src/screens/developer/DeveloperDomainScreen.jsx)

1. **The hero is a single-line banner.** It says the domain name, the table count, and a generic description. That's the *only* place domain content appears on the page.
2. **Twelve identical cards.** *Entities, Formulas, Relationships, Sample Data, Substrates, Builds, Tests, Input Spokes, Explorer, Files, Rulebook JSON, Reset Editor.* Wildly different weights (the schema vs. a destructive reset action) rendered as identical tiles. No visual hierarchy.
3. **Verb-less nouns.** A landing page should *introduce*; navigation belongs in the left rail. The cards literally duplicate the left nav in tile form. That's why the page feels empty — it is empty.
4. **Vertical waste.** 40 in² of screen and the eye has nothing to land on. Cards are below the fold of any normal monitor by the third row.

The failure mode is not aesthetic. It's that the page answers the question *"what can I click?"* before the question *"what is this?"* — which is the wrong order.

---

## 3. The pruning principle (this is the central design rule)

**Generalized apps all feel the same because they render every list as a flat grid of identical tiles.** Every entity gets the same card. Every rule gets the same row. Every substrate gets the same chip. The user reads the screen and learns nothing about what *this particular domain* is, because the layout is making the layout's argument, not the domain's.

The fix is mechanical and ruthless: **every list in this UI is curated by an `important` flag stored in the rulebook.** Important things render large and featured; everything else collapses behind a `…` or moves to a detail/schema page. This applies uniformly to:

- **Entities.** ACME has 5; some demos will have 30. The landing page features the *important* ones (Projects, Employees in ACME) as rich cards with sample rows. The rest collapse to a single "+12 more tables →" link that opens the schema view.
- **Substrates.** Some domains will have 0 substrates (jessica-basic in its current form); ACME may have 15. The hero shows the substrate chips flagged `important` (typically Postgres + one other for contrast). The rest live behind a `…` chip that opens the full substrates panel. **Missing substrates are simply not rendered — no greyed-out chips, no "coming soon" placeholders.** If a domain has zero substrates, the chip row does not appear at all.
- **Rules.** The "rules that matter" panel shows the important rules featured, with the worked example inline. The rest collapse to "+N more calculated fields →".
- **Use cases.** A domain has 0–5 important use cases that anchor the reception desk's narrative. Any others go on a detail page if they exist at all.

The principle, said plainly: **the UI's job is not to be neutral. It is to take a position on what matters in this specific domain.** That position is editable in the portal (see §5: rich-text editing) and lives in the rulebook (see §4: `_meta` extensions).

---

## 4. The rulebook `_meta` extensions (single source of truth for the experience)

All per-domain experience data lives **inside the rulebook JSON**, managed as a set. No second source — not the admin DB, not a sidecar file, not hardcoded in the React app. If we want to change how a domain feels, we edit its rulebook.

These additions slot into the existing `_meta` section (and a small number of per-entity / per-rule flags). They are the *only* fields the experience layer reads beyond the existing schema.

### 4.1 Domain-level (lives in `_meta`)

| Field | Type | Purpose |
|---|---|---|
| `tagline` | string | One-line headline shown on the picker tile and the reception desk. *"A small services shop — clients, projects, approval rules."* |
| `motif` | enum | The visual identity of the domain tile and header band. Initial set: `skyline`, `lcars`, `corkboard`, `legal-pad`, `atlas`, `default`. Hand-curated per domain; managed as data so we can add more without code changes. |
| `motif_palette` | object | Optional color overrides for the motif (`primary`, `accent`, `ink`). Lets two domains share a motif and still feel different. |
| `description_rich` | rich-text | The non-generic human-facing description shown on the reception desk. Replaces the devops-flavored auto-generated one. Markdown-flavored, edited in the portal. |
| `use_cases` | array<rich-text> | 0–5 specific things this domain is *for*. Each is one to three sentences. Not exposition — specifics. ACME's first use case is *"Approve a Compliance project that requires manager sign-off,"* not *"manage a business."* |
| `signature_rows` | array<{entity, ids[]}> | The rows pulled into the picker tile's "trailer." For ACME: three Project names. For Star Trek: three episode titles. Authored, not auto-derived, because the *choice* of which rows is part of the curation. |
| `journal_seed` | string \| null | Optional sentence that anchors the "Welcome back" hero when there's no per-user diff to show yet (first visit). |

### 4.2 Per-entity (lives on each entity object)

| Field | Type | Purpose |
|---|---|---|
| `important` | boolean | Featured on the landing page when true; collapsed into the "+N more" link when false. |
| `summary_rich` | rich-text | A one-paragraph plain-language explanation of what this entity *is* in this domain. Different from the schema's `description` field (which can stay devops-flavored). Edited in the portal. |
| `important_fields` | array<string> | The 2–4 fields shown on the entity card. Everything else is in the schema view. |

### 4.3 Per-rule / per-calculated-field

| Field | Type | Purpose |
|---|---|---|
| `important` | boolean | Featured in the "rules that matter" panel when true. |
| `explanation_rich` | rich-text | Plain-language explanation of *why this rule exists*, with a worked example. Edited in the portal. |

### 4.4 Per-substrate (lives in `_meta.substrates[]`)

| Field | Type | Purpose |
|---|---|---|
| `key` | string | `postgres`, `python`, `owl`, `excel`, etc. |
| `important` | boolean | Rendered as a chip in the hero when true. False (or absent) ⇒ lives behind the `…` overflow. |
| `chip_label` | string | Short label for the chip (`Postgres`, `Python`, `OWL`). |

### 4.5 Rich-text editing in the portal

Every `*_rich` field above is editable in the admin portal via a lightweight rich-text widget (bold, italic, lists, inline code, links — no images, no tables, no nesting). Saving writes back to the rulebook JSON in place. Because the rulebook is the source of truth, the edit propagates everywhere by definition — the picker tile, the reception desk, any tour mode, any export.

There is no "draft" state. Edits are committed directly. Git is the undo history.

---

## 5. The target experience

### Act I — The domain picker

You open the portal and you don't see a list. You see a **gallery** — each domain rendered as a small world, driven entirely by its rulebook's `_meta`.

ACME Corporation appears as a tidy corner-office tile: the `skyline` motif behind the name, the `tagline` rendered large (*"A small services shop — clients, projects, approval rules"*), three avatar bubbles for the Employees `signature_rows`, and a thin status ribbon: *"all green · last built 4m ago."* Next to it, Star Trek wears the `lcars` motif and shows its three signature episode titles. Jessica-Basic uses `corkboard` with chore cards pinned to it. Acme-LLC uses `legal-pad` to distinguish it from its sister ACME Corporation. *Is-Everything-a-Language* uses `atlas`.

Each tile is a **trailer**, not a placeholder. The signature rows are real, pulled from the rulebook by ID, so the picker shows live content the way a streaming poster shows episodes.

Above the gallery, three soft chips:

- **Last visited** — the domain you were in yesterday, surfaced gently.
- **New since you were here** — any rulebook whose JSON changed, or whose tests went red.
- **Bootstrap a new one** — expanding inline into the Shadle-steps flow, not as a separate page.

A persistent Cmd-K bar lives at the top. Type *"gdpr"* and the picker takes you straight to the GDPR Audit row inside ACME — three navigation hops away — in one keystroke.

On tablet and phone, the gallery becomes a vertical feed, but the trailer quality is preserved. Tiles still show the cast, the activity, the status. Just stacked instead of laid out.

### Act II — Inside ACME LLC (or any domain)

You tap the ACME tile and the gallery fades into the domain. The transition matters: the tile expands into the page's hero, and the same motif becomes the header band. You haven't been teleported; you've **walked in**.

#### The reception desk

A name. The rulebook's `description_rich`, rendered as a real paragraph, not a generated one-liner. A live telemetry strip below it: *3 clients · 3 projects · 3 employees · 1 project awaiting manager approval · last built 4m ago · all conformance tests green*. Numbers as nouns in a sentence — not numbers as the whole page.

To the right of the description, the **use cases panel**: the rulebook's `use_cases`, listed as 0–5 specific things you can do here. Each one is a real verb — *"Approve a Compliance project,"* *"Promote an Employee into a ProjectManager role and watch the approval rule fire."* These are authored in the rulebook and editable inline.

And the hero **knows you**. *"Welcome back. Since Tuesday, Mike Johnson was promoted to ProjectManager. That re-fired the approval rule and cleared one of the pending projects."* The page tells you what changed in your absence. Not as a notification — as a **journal entry**. Bitemporal-by-default makes this trivially natural. On first visit, the `journal_seed` from the rulebook serves as a hand-curated stand-in.

#### The cast — a horizontal scroller (filtered by importance)

A strip you swipe horizontally on a phone or scroll-wheel on a desktop. Avatars of every employee from the entity flagged `important: true` (in ACME's case, `Employees`). Each card shows the entity's `important_fields` only — not the full schema.

Tap **Sarah Chen** and a panel slides in: her role, her projects, the rules that name her, the approvals she's authorized to grant. A small sparkline showing her project workload over time. It feels like opening a Contacts app — except every datum is wired to the rulebook's DAG underneath.

Beside the cast strip, a second scroller: **projects in flight** (the second `important` entity). Each card has a status orb (approved / pending / blocked), the responsible employee's avatar, the type, the due date. The *GDPR Audit* card shows a small ⚠ icon and the explanatory whisper from its rule's `explanation_rich`: *"awaiting a manager — only Mike Johnson or another ProjectManager can approve."* That icon is clickable. It opens the rule that makes the statement true.

Other entities — `Client`, `Roles`, `TypesOfProject` — do not get their own scrollers. They appear behind a single **"+3 more tables →"** link that opens the schema view. They exist, they matter to the DAG, but the landing page is not the place to display them.

#### The rules that matter — alive (filtered by importance)

A **receipt printer**, not a static list. Only rules flagged `important: true` appear here. Each one shows when it last fired and what it touched. *"When Mike was promoted, the approval rule fired across 3 projects. 1 changed state."* A faint glow trails on rules that fired in the last 60 seconds. The DAG is breathing.

Click any rule and the page doesn't navigate — it **expands**. A drawer opens with the rule's `explanation_rich`, the DAG of its inputs, and a worked example on a real row. *"For GDPR Audit (type=Compliance), `RequiresApproval = TypesOfProject.RequiresManagerApproval = TRUE`. Approver must be an Employee whose Role.IsManager = TRUE. Currently zero such approvals on record."* The proof is shown, not just claimed.

Below the featured rules, a single quiet link: **"+12 more calculated fields →"** opens the full DAG view.

#### Substrate witnesses (filtered by importance, hidden when empty)

A small horizontal row of chips in the hero, populated *only* from `_meta.substrates[]` entries with `important: true`. ACME may show **Postgres · Python** as featured chips and a third `…` chip that opens a panel listing the other 13. Jessica-Basic, with zero substrates, shows no chip row at all.

Each featured chip is green. Tap Postgres and a panel opens with the *same question* (*"how many projects are awaiting approval?"*) and the *same answer* (*"1"*), rendered in SQL. Tap Python: same question, same answer, as a Python expression. Tap a chip from inside the `…` overflow and the same panel opens for that substrate. The point is felt, not lectured: **this rulebook is portable**.

When the rulebook rebuilds, the chips briefly turn amber, then relight green as each substrate finishes. The rebuild **is** the user experience, not a background process you hope finished.

#### Time travel (read-only in v1, clearly marked)

A timeline ribbon at the bottom of the page, like a video scrubber. Drag it left: the page rewinds. Last Tuesday's state. Last week's state. The cast strip reshuffles to show who held which project. Projects revert to their old approval states. The substrate chips re-light against the historical state.

The instant you leave "now," the page wears its rewind state visibly: a sticky banner across the top reads **`⏰ Viewing state as of Tue Nov 4, 2025 · 14:22 — Return to now →`**, the page's background tints faintly amber, and every editable affordance is disabled. There is no way to mistake the past for the present. The **"Return to now"** button is large, persistent, and one click resets the scrubber to live.

Editing in the past is **not** in v1. The scrubber is strictly a read-only audit lens.

#### Command palette

Cmd-K from anywhere in the domain. Type *"sarah"* — Sarah Chen surfaces with all rows that name her: projects, approvals, rules. Type *"due this week"* — a query runs across every entity and matches return as a grouped list. Type *"requires approval"* — the rule itself is the top result, with the rows it currently flags shown beneath. The palette is the universal navigation; the visual page is the surface for users who prefer to browse.

The palette respects importance: featured entities and important rules sort to the top. The long tail is reachable but not in the way.

#### The footer is a build receipt

A single calm line: *built 4m ago · 12 calculated fields · 47 conformance tests green · 4 substrates in lockstep · view JSON · view DAG · download Excel*. Dev-ops is acknowledged without being foregrounded. A click expands the full build report inline.

#### Tour mode (curated by importance, not heuristic)

A small **"Take the tour"** button on the hero. Hit it and the page auto-narrates itself for 90 seconds: use cases → important cast → important projects → most-important rule firing → the substrate chips agreeing → the timeline showing the same answer six months ago.

There is no "interestingness heuristic." The tour walks the `important: true` items in the order they appear on the page, reading each one's `*_rich` field aloud (or as captions). The curation is the rulebook author's job; the tour is just a renderer.

It's the perfect demo button. It's also the perfect onboarding for a new developer. *Same affordance, two audiences.*

#### Mobile + tablet

The page reads as a vertical stack of horizontal scrollers — the iOS idiom everyone already knows.

- **Phone** (≤ 640px): Single column. Hero card sticky on top. Then **Use cases · Cast · Projects · Rules · Substrates · Timeline**, each as a swipe-strip. Cmd-K hides behind a 🔍 button.
- **Tablet portrait** (641–1024px): Hybrid. Hero spans the width; below it, a two-column layout: left column is **Cast + Projects** stacked; right column is **Use cases + Rules**. Substrate chips and timeline span the full width. Cmd-K visible in the top bar.
- **Desktop** (≥ 1025px): The magazine grid — hero band across the top, multi-column layout below, scrollers running horizontally inside their columns.

**Same content. Three geometries. No 1990s dashboard.**

#### State preservation (per-user, per-domain, full route memory)

The portal remembers where you were. Not just "last visited domain" — the **last route within each domain**. If you were on `acme-corporation/schema/Projects` and you switch to `star-trek`, you land where you last were inside Star Trek (say, `star-trek/rules/WarpCoreApproval`). When you come back to ACME, you're back on `Projects`. No re-navigation.

Persistence layer:

- **Admin-portal DB** (`erb_admin_portal`) stores `user_domain_state` rows: `(user_id, domain, last_route, last_visited_at, last_seen_rulebook_revision)`.
- The diff that powers the "Welcome back" journal entry is computed server-side by comparing `last_seen_rulebook_revision` to the current rulebook JSON.
- Browser local storage is **not** the source of truth — it's only used to avoid a round-trip on the very first paint.

This is also what makes the picker's "Last visited" and "New since you were here" chips real: they read from the same `user_domain_state` table.

---

## 6. What this experience actually is

Four claims about ERB, made **visible** instead of explained:

1. **A rulebook is a business, not a schema.** So the page leads with the cast, the work, and the rules in plain language. The schema is something you can drill into if you want.
2. **A rulebook is a living DAG, not a static spec.** So rules glow when they fire, and the page shows what changed since you were last here.
3. **A rulebook is substrate-independent.** So the substrate chips are always one tap away from showing the same answer in multiple languages.
4. **A rulebook takes a position.** Importance flags, hand-authored taglines, curated signature rows, motifs — the rulebook tells the UI what matters. Two domains never feel the same because no two rulebooks make the same claims.

Once those four claims are *felt* — not read — the user understands what ERB is without anyone having to explain it.

---

## 7. Resolved specification (the v1 build target)

The open questions from earlier drafts are now decided. They are listed here as the spec, not as questions.

1. **Per-domain visual motif → stored in `_meta.motif` in the rulebook.** Hand-curated per domain for now; managed as a typed enum so we can add motifs without touching app code. All other domain-personality data also lives in the rulebook (see §4) so the rulebook is the single source of truth.

2. **"Knows you" persistence → admin-portal DB, full route memory per domain.** Switching domains restores the user to the exact route they last had open inside that domain. Per-user `last_seen_rulebook_revision` is stored so the "Welcome back" journal entry is real, not a heuristic. Browser local storage is a first-paint optimization only.

3. **Substrate witnesses → only render what exists, filtered by `important: true`.** No greyed-out chips, no "coming soon" placeholders. Domains with zero substrates show no chip row at all. Domains with many substrates (ACME could reach 15) feature the important ones as chips; the rest hide behind a `…` overflow that opens the full substrates panel.

4. **Tour-mode curation → driven by `important: true` flags throughout the rulebook, not by a heuristic.** Apply the same pruning pattern to entities, rules, substrates, and use cases. The rulebook author decides what's important; the UI renders that decision. This is the **central design rule** (see §3) and it applies *everywhere* — the problem with generalized apps is that everything looks equally weighted, and importance flags are the cure.

5. **Rich-text editable sections → every `*_rich` field in the rulebook.** Edit in the portal, save writes back to the rulebook JSON. Git is the undo history. No drafts, no separate CMS.

6. **Time travel → read-only rewind in v1.** Clear demarcation: sticky "Viewing past state" banner, amber background tint, disabled edits, persistent "Return to now" button. Editing the past is explicitly out of scope.

7. **Mobile breakpoint behavior → three breakpoints (phone, tablet portrait, desktop).** Tablets get a hybrid two-column layout below the hero; phones get a single vertical stack of scrollers; desktop gets the magazine grid.

8. **Scope → full v1.** Reception desk, cast/projects scrollers, rules-that-matter panel, substrate chips, build-receipt footer, **timeline**, **command palette**, **tour mode**, and **state preservation** all ship together. There is no tight v1 cut and no v2. This is it.

---

## 8. The test

When the user closes the tab, they should be able to describe ACME Corporation to a colleague in one sentence. If they can't, the page failed. If they can, every other affordance on the page has earned the right to be there.

The harder test: if the user opens *any two domains* and they feel the same, the page failed. Each domain's rulebook should put enough of itself forward — through tagline, motif, use cases, important entities, important rules, signature rows — that the two pages read as different small worlds, not as two instances of the same template.
