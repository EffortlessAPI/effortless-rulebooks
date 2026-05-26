<!-- DERIVED ARTIFACT — DO NOT EDIT BY HAND -->
<!-- Source: https://raw.githubusercontent.com/effortlessapi/effortless-claude/main/skills/effortless-react-explainer-dag/SKILL.md -->
<!-- Mirrored by: docs/skills/clone-skills.sh -->
<!-- Update: cd effortless-platform && effortless clone-skills -->

---
name: effortless-react-explainer-dag
description: >
  Use when adding the React Explainer DAG to an Effortless project — a
  generated, embedded visualization of the rulebook's calculated-field
  DAG that lets users click any cell/field and see exactly how it was
  derived (raw inputs → lookups → 1st/2nd/3rd-order calcs → aggregations).
  This is the canonical "show the inference graph" UI for ERB demos.

  Triggers: "add the explainer dag", "wire up the explainer", "show the
  DAG in the UI", "rulebook-to-react-explainer-dag", "explain a
  calculated field visually", "add DagCell / DagToggle / FieldDag",
  "make calculated fields clickable".

  **Scope (load gate):** Effortless projects with a Vite + React web app
  (typical for the demo skill). Does not require Airtable.
audience: customer
---

# Effortless React Explainer DAG — generated, embedded inference visualizer

A drop-in module that reads the project's `effortless-rulebook.json`,
embeds it into the web bundle at build time, and renders a clickable
DAG for every calculated/lookup/aggregated field. Users hover a cell to
see the formula, click to drill into the full inference graph for that
field, and navigate hop-by-hop back to raw inputs.

The integration is **purely additive** — it does not touch your
existing pages' data flow. You wrap cells in `<DagCell>`, add one
route, and the explainer reads from its own embedded rulebook copy.

## When to add it

- As the **final step** of the demo-app flow (after server + web are
  working end-to-end). Recommended default.
- **On demand** any time later when the user asks to "show the DAG"
  or "make the calculated fields explainable".

Don't add it before the app boots and reads/writes work — debugging
two things at once wastes time.

## The transpiler

Name: `rulebook-to-react-explainer-dag`

It emits a self-contained module at the configured output path
(typically `web/src/explainer-dag/`) with:

- `embedded-rulebook.ts` — frozen copy of the rulebook
- `lib/` — `dagResolver`, `routingContext`, types
- `components/` — `DagCell`, `DagHoverCard`, `DagToggle`, `FieldChip`,
  `FormulaText`, `TypeBadge`
- `pages/FieldDag.tsx` — the full-screen DAG view
- `dag.css` — styles
- `index.ts` — public barrel

You consume it via `import { ... } from "./explainer-dag"`.

## Add to effortless.json

Append a third transpiler **after** `rulebook-to-postgres` and
`execute ./init-db.sh`:

```json
{
  "IsSSoTTranspiler": false,
  "Name": "rulebooktoreactexplainerdag",
  "RelativePath": "/web/src",
  "CommandLine": "rulebook-to-react-explainer-dag -i ../../effortless-rulebook/effortless-rulebook.json",
  "IsDisabled": false
}
```

If the transpiler isn't installed yet:
```
cd web/src && effortless -install rulebook-to-react-explainer-dag
```

Then `./start.sh build` (or `effortless build`) regenerates the
module. The DB build runs first; this step is purely codegen, no DB
side-effects.

## Wire it into the web app (4 small edits)

### 1. `App.tsx` — RoutingContext + DAG route

```tsx
import { FieldDag, RoutingContext } from "./explainer-dag";
import "./explainer-dag/dag.css";
import { Link, useNavigate, useParams } from "react-router-dom";

function useDagRouting() {
  const navigate = useNavigate();
  return {
    FieldLink: ({ table, field, className, children }: {
      table: string; field: string; className?: string; children: React.ReactNode;
    }) => (
      <Link to={`/dag/${table}/${field}`} className={className}>{children}</Link>
    ),
    onBack: () => navigate(-1),
    navigate: (t: string, f: string) => navigate(`/dag/${t}/${f}`),
  };
}

function DagRoute() {
  const { table = "", field = "" } = useParams();
  return <FieldDag table={table} field={field} routing={useDagRouting()} />;
}
```

Wrap your `<Routes>` in `<RoutingContext.Provider value={useDagRouting()}>`
and add:

```tsx
<Route path="/dag/:table/:field" element={<DagRoute />} />
```

### 2. `Shell.tsx` — global on/off toggle

```tsx
import { DagToggle } from "./explainer-dag";
// ...
<div style={{ display: "flex", justifyContent: "flex-end" }}><DagToggle /></div>
```

`DagToggle` flips a global "explainer mode" — when off, `<DagCell>`
renders children as-is; when on, cells get a clickable hover affordance.

### 3. Pages — wrap calculated/lookup/aggregated cells in `<DagCell>`

For every cell that displays a non-raw value (calc / lookup /
aggregation / Name PK), wrap it:

```tsx
import { DagCell } from "../explainer-dag";

<td><DagCell table="Invoices" field="Name"><Link to={`/invoices/${i.invoice_id}`}>{i.name}</Link></DagCell></td>
<td><DagCell table="Invoices" field="ClientName">{i.client_name}</DagCell></td>
<td><DagCell table="Invoices" field="TotalDue">{money(i.total_due)}</DagCell></td>
```

Rules of thumb:

- Raw fields editable in a form → **don't** wrap (they have no DAG)
- Anything calculated, looked up, or aggregated → **wrap**
- The `Name` PK → wrap (it's a calculated formula)
- `table` is the PascalCase rulebook table name
- `field` is the PascalCase rulebook field name (not the snake_case
  view column)

### 4. (Optional) `FieldChip` on detail pages

For a richer detail page, use `<FieldChip table="X" field="Y" />`
inline — it renders the label + type badge + clickable affordance.

## Verifying it works

After `./start.sh build` + restart web:

1. Page loads; the DAG toggle appears in the top-right of the shell
2. Toggle on → calculated cells get a subtle outline + cursor
3. Click a calculated cell → routed to `/dag/<Table>/<Field>` with the
   full inference graph rendered
4. Click an upstream field in the graph → navigates to its DAG
5. Back button returns to the previous field

If the DAG is empty for a field you expect to be calculated, the
field's rulebook entry is probably typed as a raw field — check
`effortless-rulebook.json`.

## Pitfalls

1. **Stale embedded rulebook.** The module embeds the rulebook at
   build time. After editing the rulebook you MUST `./start.sh build`
   (not just restart the web dev server) — Vite HMR won't refresh
   `embedded-rulebook.ts` on its own.

2. **PascalCase mismatch.** `<DagCell table="invoices" field="totalDue">`
   will silently render nothing. Use the rulebook's exact casing.

3. **Wrapping raw editable fields.** A `<DagCell>` around an `<input>`
   that edits a raw field clutters the form without adding info — raw
   fields have no upstream graph. Skip them.

4. **Route order.** Put `/dag/:table/:field` BEFORE the `*` catch-all
   redirect, or it'll get swallowed.

5. **Multiple `RoutingContext.Provider`s.** One at the App level is
   enough; nesting doesn't compose, it overrides.

## Minimal integration checklist

- [ ] Transpiler installed and added to `effortless.json`
- [ ] `./start.sh build` regenerates `web/src/explainer-dag/`
- [ ] `App.tsx`: import, `useDagRouting`, `DagRoute`, route, provider
- [ ] `Shell.tsx`: `<DagToggle />` mounted somewhere visible
- [ ] At least one page wraps calculated cells in `<DagCell>`
- [ ] Web restarts cleanly; toggle + click + back all work
