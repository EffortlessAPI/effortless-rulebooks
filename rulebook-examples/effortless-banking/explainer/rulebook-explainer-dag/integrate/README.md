# Integrating rulebook-explainer-dag

See the tool README at the repo root. Copy `integrate/snippet.html` into your host layout.

Serve `/rulebook-explainer-dag/*` as static files. That is all the routing there is: with the snippet's `mode: "modal"`
a field opens in an overlay and a table opens the explainer's own page
(`/rulebook-explainer-dag/pages/index.html#/dag/<Table>`), a real file in this folder. Do not add `/dag/*` routes to the
app, and do not choose `mode: "path"` unless the host really serves `/dag`, `/dag/:table` and `/dag/:table/:field` from
`pages/*.html`: in an app without those routes, the app's own router answers them (usually with its home page).

## The corner control

`corner: "bottom-right"` (or `top-left`, `top-right`, `bottom-left`) puts the control in that corner of the window: two
icons, opening on hover to "download the workbook" and "provenance on/off". `xlsx: "/your-project-rulebook.xlsx"` adds the
download (the file `rulebook-to-xlsx` wrote). `provenance: "off"` starts with the badges off; the person's own choice then
wins. To place the control yourself, pass `mountToggle: "#your-element"` instead of `corner`.

## The witness (optional)

A popup or a field's page can show one row's value and the values it came from, down to the stored ones. Every value is
computed in the page by the project's own generated TypeScript, so the project needs `rulebook-to-typescript` too:

1. Add `rulebook-to-typescript` to `effortless.json` (the full form, not `-p simpleSyntax=true`), writing INTO this
   folder as `typescript/`, for example `"RelativePath": "/web/public/rulebook-explainer-dag/typescript"`, listed after
   the explainer's own step. The explainer finds it there by itself. (A project on effortlessapi.com already has this:
   the rulebook editor's build writes it.)
2. Only if it is written somewhere else, name that folder:
   `EffortlessExplainer.init({ ..., typescript: "/rulebook-typescript" })`.
3. Say which row a cell shows: `<span data-er-dag="Table.Field" data-er-row="row-key">value</span>` (the row's key:
   the value of the table's `...Id` field).

The rows are the rulebook's own. For an app's live rows, hand them over: `EffortlessExplainer.setRows({ Table: [row, ...] })`
(or `init({ rows })`), raw values only, keys as the rulebook or the database spells them. `witness.js` and `ts-strip.js`
sit beside `explainer-dag.js` and are fetched on first use; no script tag to add.
