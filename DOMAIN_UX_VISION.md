# Domain UX Vision — what's left

> Living plan for the Effortless admin portal. The reception desk, picker,
> rich-text editor, command palette, time-travel scrubber, tour mode, three
> responsive breakpoints, per-user state, and rules-glow shipped in earlier
> sessions. This document is the **remainder** — open work only.
>
> **Core behavior first. UI polish last.**

---

## What already shipped (do not re-plan)

**Picker** — gallery of motif-banded trailers reading the rulebook's
`__meta__` table (rows for `tagline`, `motif`, `motif_palette`,
`signature_rows`); three picker chips above the
gallery (*Last visited* / *New since you were here* / *Bootstrap a new
one*).

**Reception desk** — motif hero band with logo, slug, name, tagline,
substrate witness chips, and the "Take the tour" button. Below: the
`description_rich` paragraph, telemetry strip, `journal_seed` blockquote,
use-cases panel on the right. Horizontal scrollers per `important: true`
entity (only `important_fields` shown). Rules-that-matter panel with
click-to-expand `explanation_rich` + formula + worked example.

**Cross-cutting** — Cmd-K command palette; inline rich-text editor
(pencil-toggle, `PATCH /api/rulebook/text` with strict path allow-list);
time-travel scrubber driven by `git log` of the rulebook JSON, sticky
amber `Viewing state as of …` banner, pencils disabled in past mode;
rules-panel rows pulse green for 60s after the rulebook's most recent
commit; 90-second tour mode walking important items in page order;
three responsive breakpoints (≤640 phone / 641–1024 tablet / ≥1025
desktop). Per-user / per-domain state persists in
`portal_user_domain_state`.

The `__meta__` table is authored for **8 of 28 demo rulebooks** today:
`acme-corporation`, `acme-llc`, `star-trek`, `jessica-basic`,
`jessica-advanced`, `is-everything-a-language`,
`effortless-rulesbooks`. The other 20 fall back to `motif: "default"` —
that's the active in-flight work right now (a parallel-subagent wave is
running this session). `customer-fullname` is blocked separately — it's
`{}` at HEAD and needs a restore-or-retire decision before it can be
authored.

---

## 1. Reference: ACME Corporation data

Source: [rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json](rulebook-examples/acme-corporation/effortless-rulebook/acme-corporation-rulebook.json)

- Five entities: `Client` (3 rows), `Projects` (3), `Employees` (3),
  `Roles` (3), `TypesOfProject` (4).
- Business story: a small professional-services shop. Some project types
  (Compliance, ClientFacing) require manager sign-off; only employees in
  manager roles can approve. The DAG wires that workflow.
- `__meta__` table fully authored — this is the **reference rulebook**. Match
  this shape when authoring `__meta__` rows for other demos.

---

## 2. Top priority — Effortless Explorer (DAG tree on the left, schema+data as combined metadata on the right)

**This is by far the biggest failure right now, and it's what we're doing next.** The behavior described
below is partially, scantily implemented across ~5 different parts of
the app — `/developer/:domain/data` (Sample Data grid),
`/developer/:domain/entities` (schema + data view),
`/developer/:domain/explorer` (SQL + data), and bits of the reception
desk. They are confusingly similar, they each show a different slice of
the same thing, and the reception-desk row clicks all route to `/data`
instead of to the row the user clicked. Consolidate.

**The "Entities" tab is the Effortless Explorer.** Not a table of entities
with their attributes, with sample-data tables stacked underneath.
Instead:

### 2.1 Left nav: the actual DAG, walkable

A tree representing real instances and their relationships — for an
order-processing domain:

```
Businesses                          ← top-level node, all businesses
├ Acme Corp
│  ├ Customers                      ← Acme's customers
│  │  ├ Jane Smith
│  │  │  ├ Orders
│  │  │  │  ├ Order #1042
│  │  │  │  │  └ Invoices
│  │  │  │  │     └ INV-2026-0117
│  │  │  │  └ Order #1051
│  │  │  └ Addresses
│  │  └ Bob Lee
│  │     └ …
│  └ Projects
│     └ …
└ Globex
   └ …
Customers                           ← top-level node, no filter: all customers across all businesses
Invoices                            ← top-level node, no filter: every invoice in the DAG
Orders                              ← same
```

Top-level nodes appear for **every entity** (an unfiltered view of that
entity across the whole DAG), AND as **nested children** wherever the
DAG places them (a business's customers, a customer's orders, an order's
invoices, …). Clicking any node — at any depth — lets the user
interactively explore that slice of the DAG.

> **FUTURE SELF HINT — list-node UX.** When the selected node is a list
> (e.g. *Customers under Acme Corp*, or the top-level unfiltered
> *Customers*), the right pane should eventually grow full
> sort / filter / group / facet behavior — the planned long-term UX for
> any list view in the explorer. Not now. Capturing it here so we don't
> re-discover it later; the minimum to ship the Explorer is a usable
> grid with the schema visible.

### 2.2 Right pane: entity header + DAG tabs

Clicking a **specific entity instance** (e.g. *Jane Smith*, or *Order
#1042*) shows a header for that entity, with **tabs underneath** that
navigate to the different sub-elements in the DAG for this row — Orders,
Invoices, Addresses, etc. The tabs are derived from the rulebook (the
relationships hanging off this entity), not hand-authored.

### 2.3 Schema and data shown as combined metadata

The right pane is not "data grid with a separate schema sidebar." It's a
single surface where the column header **is** the schema entry: hover to
reveal the field's type / formula / description; double-click to expand
the column into a schema-detail strip; click a cell to see provenance
(raw input vs. lookup vs. calculated, and the formula that produced it).
Many UX models can hang off this — the principle is *schema and data
are the same metadata, surfaced together*, not two separate views the
user has to switch between.

### 2.4 Routes & consolidation

- `/developer/:domain/explorer` is the single home. `/data` and
  `/entities` redirect here.
- `/explorer` — root node selected; nothing on the right yet (or the
  domain overview).
- `/explorer/:path` — `path` encodes the DAG walk
  (`businesses/acme-corp/customers/jane-smith/orders/1042`). Both list
  nodes and instance nodes round-trip through the URL so reception-desk
  scrollers and Cmd-K row matches deep-link to the exact node.
- AppNavigation in the project rulebook surfaces a single **Explorer**
  entry; the old "Entities" / "Data" / "Explorer" entries collapse into
  it.

---

## 3. Next-up — rewrite Effortless Tools as a folder/tool tree

The current Effortless Tools page does **three things badly**:

1. **The "Build all" button is one opaque switch.** No live progress, no
   per-tool status, no way to see what's running or what's stuck.
2. **The "installed substrates" list is the same ~10 entries regardless
   of the project.** It does not reflect what's actually in `effortless.json` —
   ACME LLC has all 10 installed and looks the same as a project with
   two installed. The page is lying about installation state.
3. **The tool detail panel summary is generic and unhelpful**, and the
   floating "Available to add" cards at the bottom have no clear
   relationship to anything above them.

**This page should be a projection of the on-disk project, not a
floating dashboard.**

### 3.1 Left rail — folder/tool tree

The left rail mirrors the project's actual folder structure under the
active domain root (`rulebook-examples/<domain>/`, or
`effortless-platform/` for the project rulebook). Folders that contain
Effortless tools list those tools as inline children. Folders that
don't still appear in the tree but have no children.

```
acme-llc/
├ effortless-rulebook/
│  └ • airtable-to-rulebook         (input spoke)
├ execution-substrates/
│  ├ • rulebook-to-postgres
│  ├ • rulebook-to-python
│  ├ • rulebook-to-excel
│  ├ • rulebook-to-owl
│  ├ • rulebook-to-golang
│  ├ • rulebook-to-csv
│  ├ • rulebook-to-uml
│  ├ • rulebook-to-cobol
│  ├ • rulebook-to-binary
│  └ +                              (add another tool here)
├ english/
│  └ • rulebook-to-english
├ testing/
│  └ • rulebook-to-test-suite
└ +                                 (add a tool at the project root)
```

The tools listed inline come from **the `effortless.json` files**
discovered at each level of the tree — `effortless.json` is a hierarchy,
based on which folder it lives in. ACME LLC has 10 substrates because
ACME LLC's `effortless.json` (and its subfolders') list 10 tools. A
project with two tools shows two. Never default to a hard-coded ~10.

### 3.2 `+` to add a tool

Every folder ends with a `+` row. Clicking opens the catalog currently
floating at the bottom of the page (repurposed) — the filtered list of
*tools available to this folder* that aren't yet installed. Choose one,
it gets written into the folder's `effortless.json`, and the tool appears
under that folder in the tree.

### 3.3 Tool detail (main panel)

Clicking a tool node shows:

- **Tool metadata** — name, description, source URL (`ssotme://…` or
  `http://localhost:4242/…`), version, what dialect it emits.
- **Inputs** — the input files it reads, with paths resolved relative
  to the project (typically the rulebook JSON; sometimes testing
  fixtures or templates).
- **Outputs** — the output files it writes, with the actual generated
  paths.
- **Parameters** — exactly the flag set declared in `effortless.json`
  for this entry (`-i`, `-o`, additional `-p` args).
- **▶ Run** — runs this one tool. Live stdout streams below the panel
  while it runs; exit code + duration shown when it finishes.

This replaces the current "shitty summary" with the only summary that
matters: *what does this tool read, what does it write, what flags
control it, and what happens when I run it right now.*

### 3.4 Folder-level Run, root-level Run

Clicking a **folder** (not a tool) shows a folder detail panel with a
single **▶ Run all** button. It runs every tool under that folder in
order, with the same live tail.

Clicking the **project root** shows a project detail panel with **▶ Run
all** that runs everything under the project. This replaces the current
global "Build all" button.

### 3.5 Backend work

- `GET /api/effortless-tools/tree` — walks the active project's folder
  structure, parses each `effortless.json` it encounters, returns
  `{ folders: [{ path, tools: [...] }, ...] }`. Each tool entry has
  `name`, `installUrl`, `inputs`, `outputs`, `params`, plus a stable
  `id` for routing.
- `GET /api/effortless-tools/catalog?folder=<path>` — the "available to
  add" list filtered to tools sensible for that folder.
- `POST /api/effortless-tools/install` — writes a new entry into the
  appropriate `effortless.json`; refreshes the tree.
- `POST /api/effortless-tools/run` — body
  `{ scope: "tool"|"folder"|"root", id: ... }` → spawns the process
  scoped to that subset, returns a `runId`.
- `GET /api/effortless-tools/run/:runId` — Server-Sent-Events stdout
  stream so the UI can tail the build live without polling.

### 3.6 No more separate "Orchestration" tab

This design subsumes the previously planned dedicated Orchestration
tab. Tools have per-tool Run buttons, folders have Run-all buttons, the
project root has Run-everything, and every run streams its output
live. There's no orchestration concept left orphaned without a home.

---

## 4. Other open priorities

Listed core-behavior-first. UI polish is at the bottom (§4.5 onward).

### 4.1 Fix the `last_seen_rulebook_revision` auto-clear bug

Today every domain visit stamps `last_seen_rulebook_revision` to the
current revision immediately. So the moment a user opens a domain whose
rulebook changed, the "New since you were here" picker chip clears
before they've read why they were notified.

Fix: split the visit-stamp from the seen-stamp. Visiting writes only
`last_route` + `last_visited_at`. The revision is stamped when:
- The user clicks "Mark as seen" on the welcome-back diff, OR
- They dismiss the welcome-back banner, OR
- ~30 seconds elapse on the page suggesting they've had time to read.

This is a correctness bug, not a feature.

### 4.2 Substrate witness panel — inline, not navigate-away

The hero substrate chips currently navigate to Effortless Tools. The
vision wants them to **expand inline** into a panel showing the *same
question* answered in each substrate's dialect — *"how many projects
are awaiting approval?"* as a SQL query, a Python expression, an OWL
axiom, all returning the same number. This is where the
"substrate-independent" claim becomes *felt*.

v1: hand-authored `witness_sample` per substrate in the `substrates`
row of the rulebook's `__meta__` table. v2: live-execute against each
substrate.

### 4.3 Welcome-back journal diff

Today `last_seen_rulebook_revision` is recorded but never displayed.
The journal area only shows `journal_seed`.

Target: when the user lands on a domain whose current revision differs
from their last-seen revision, the journal shows the **real diff** —
*"Since your last visit on Tuesday, two new projects were added, the
approval rule's `explanation_rich` was rewritten, and three calculated
fields fired."* Server endpoint `GET /api/rulebook/diff?from=<sha>`
returns a structured diff (entities changed, rows added/removed, rules
touched). Fall back to `journal_seed` only when there's no diff.

### 4.4 customer-fullname — restore or retire

[customer-fullname-rulebook.json](rulebook-examples/customer-fullname/effortless-rulebook/customer-fullname-rulebook.json)
is `{}` (3 bytes) at HEAD. The previous incarnation at commit `2ab5b78`
had a real `Customers` table with the `FirstName / LastName / FullName`
calculated field. Two options:

1. **Restore** from `2ab5b78` (or re-author using ACME's `Client.FullName`
   as reference), then author the `__meta__` table.
2. **Retire** — remove the directory, delist from the picker; the
   minimal-calculated-field demo lives in `acme-corporation`'s
   `Client.FullName` already.

Decision blocks one demo and shouldn't block anything else.

---

### UI polish (below the core-behavior line)

### 4.5 Viewer surface parity

Viewer routes (`/viewer`, `/viewer/domains`, `/viewer/:domain`) still
use the old `.cards` grid and the pre-reception-desk overview screen.
Wire them to use `DomainTile` + `PickerChips` (viewer accent color), and
swap the overview screen for a read-only reception desk
(`canEdit={false}`, no scrubber, no Effortless Tools link in the footer,
comments panel in its place). Components already accept `canEdit`.

### 4.6 Bootstrap inline form (replace the instructions panel)

The picker's "Bootstrap a new one" chip expands today to an instructions
panel. Replace with an **inline form**: name, slug, motif, starter
template, one-line tagline. Submit creates
`rulebook-examples/<slug>/effortless-rulebook/<slug>-rulebook.json`,
registers in `RulebookFlavors`, navigates to the new domain's reception
desk. `POST /api/projects/bootstrap` does the filesystem work; no
`effortless build` runs until the user fires one from the new Effortless
Tools tree.

### 4.7 Tour mode rewind step

Tour mode currently walks use cases → entities → rules → substrates →
closer. Add a step that rewinds the TimeScrubber to the oldest commit,
holds one beat to show the rule resolving against historical state,
then snaps back to "now" before the closer. The time-travel claim
becomes *felt* without the user having to discover the scrubber on
their own.

---

## 5. The test (unchanged)

When the user closes the tab, they should be able to describe ACME
Corporation to a colleague in one sentence. If they can't, the page
failed.

The harder test: if the user opens *any two domains* and they feel the
same, the page failed.

This currently passes for 8 of 28 demo domains. The in-flight
`__meta__`-authoring wave extends it to the rest.

---

## 6. Honest shortcuts already in production

Called out so they aren't filed as bugs:

- **Time-travel scrubber** rewinds by git commit, not arbitrary
  timestamp. No per-cell history exists.
- **Rules glow** marks all important rules with the rulebook's
  most-recent commit timestamp because there's no per-rule fire
  telemetry yet. The pulse is real, the per-rule attribution is
  approximate.

Both are documented at the source. Replacing either with proper
bitemporal / event-stream infrastructure is a future project.
