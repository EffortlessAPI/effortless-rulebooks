# Explainer DAG — Live Data Integration

This directory contains two optional integrations that make the embedded
Explainer DAG reflect **live production data** instead of the seed facts
baked into `effortless-rulebook.json`.

## 1. Refresh raw facts from Postgres → sidecar rulebook

If the host project has its data in Postgres, run:

```bash
./refresh-facts-from-postgres.sh \
  --rulebook ../../effortless-rulebook/effortless-rulebook.json \
  --db "$DATABASE_URL"
```

This produces a **sidecar** file next to the SSOT:

```
effortless-rulebook/
  effortless-rulebook.json              ← unchanged (seed data)
  effortless-rulebook.with-facts.json   ← NEW: same schema, live data
```

The sidecar is what you feed to downstream transpilers when you want
generated artifacts to reflect production.

> **Why raw facts only?**
> The DAG recomputes every calculated / lookup / aggregation field
> client-side. Pulling pre-computed values from views would short-circuit
> the provenance story.

## 2. Wire the XLSX download into `effortless.json`

The `<XlsxDownload />` component links to `rulebook.xlsx` under your SPA's
`BASE_URL`. To make that file exist, add `rulebook-to-xlsx` as a chained
transpiler step that writes into the SPA's `public/` dir:

```jsonc
// effortless.json — excerpt
{
  "transpile": [
    {
      "transpiler": "effortless/rulebook-to-react-explainer-dag",
      "input": "effortless-rulebook/effortless-rulebook.with-facts.json",
      "output": "web/src/explainer-dag"
    },
    {
      "transpiler": "effortless/rulebook-to-xlsx",
      "input":  "effortless-rulebook/effortless-rulebook.with-facts.json",
      "output": "web/public/rulebook.xlsx"
    }
  ]
}
```

After `effortless build`, the SPA serves `rulebook.xlsx` from its base URL
and the `<XlsxDownload />` icon link Just Works.

## 3. Drop the component next to your `<DagToggle />`

```tsx
import { DagToggle, XlsxDownload } from "./explainer-dag";

<div className="dag-controls">
  <DagToggle />
  <XlsxDownload />
</div>
```
