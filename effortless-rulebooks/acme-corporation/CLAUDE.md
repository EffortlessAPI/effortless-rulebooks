# ACME Corporation — Effortless Project

**A comprehensive business process and inventory management system demonstrating real-world ERB patterns.**

This folder is a **self-contained Effortless Rulebook (ERB) project**. The rulebook is the single source of truth for all business logic.

## What This Demonstrates

- **Business process modeling**: Client → Order → Fulfillment workflows
- **Inventory tracking**: Stock levels, SKU management, supplier relationships
- **Multi-entity aggregations**: Order totals roll up from line items; fulfillment status derives from shipments
- **Temporal reasoning**: Dates, deadlines, fulfillment timelines
- **Status derivations**: Computed states based on related records (Order.Status depends on fulfillment completeness)

## Quick Start

### Pull from Airtable (recommended)

```bash
effortless airtabletorulebook
effortless build
```

This pulls the latest rulebook from the connected Airtable base and regenerates all substrates.

### Or edit directly

```bash
# Edit effortless-rulebook/effortless-rulebook.json with your changes
effortless build
```

## Key Files

- `effortless.json` — Project config with hardcoded base ID
- `effortless-rulebook/effortless-rulebook.json` — The rulebook (SSoT)
- `README.md` — Narrative documentation of the domain
- `execution-substrates/` — Generated code (Postgres, Python, Go, etc.)

## Testing

```bash
./start.sh
# or bash orchestration/orchestrate.sh
```

Runs conformance tests across all substrates; all should produce identical results.

---

**The rulebook is the specification. Everything else is derived.**
