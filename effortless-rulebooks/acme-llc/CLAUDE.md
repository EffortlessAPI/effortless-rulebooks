# ACME, LLC — Effortless Project

**A lean variant of ACME Corporation—demonstrating essential business rules in a simplified schema.**

This folder is a **self-contained Effortless Rulebook (ERB) project**. It's a minimal version of ACME Corporation, useful for learning or for domains with simpler workflows.

## What This Demonstrates

- **Simplified order-to-fulfillment**: Core workflow without full inventory system
- **Calculated statuses**: Minimal set of business rules
- **Essential aggregations**: Order totals from line items
- **Lean data model**: Proves ERB patterns work at any schema complexity

## Quick Start

### Pull from Airtable

```bash
effortless airtabletorulebook
effortless build
```

### Or edit directly

```bash
effortless build
```

## Key Files

- `effortless.json` — Project config with base ID: `appWrXPvXbkgQGOxt`
- `effortless-rulebook/effortless-rulebook.json` — The rulebook
- `README.md` — Narrative documentation
- `execution-substrates/` — Generated code

## Testing

```bash
./start.sh
```

---

**The rulebook is the specification. Everything else is derived.**
