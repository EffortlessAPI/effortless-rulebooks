# Toy Rulebooks

These are demonstration toys — intentionally small domains whose job is to show the *breadth* of the platform, not the depth of a real domain.

The defining property here is the **substrate matrix**: one rulebook, many runtimes. [acme-llc](acme-llc/) is the canonical example — three tables, six calculated fields — run through all 17 substrates (Postgres, Python, Go, COBOL, Excel, OWL, English, and more), all conformant. The domain is simple by design so that what the demo brings is the *substrate matrix*, not the complexity of the subject matter.

Because this repo is also used as a live demonstration environment, some toy domains may show partially-completed loop steps at any given moment. A full `effortless build` on any domain resets it to its defined state.

## What lives here

| Domain | Purpose |
|---|---|
| [acme-llc](acme-llc/) | Canonical substrate breadth witness — 17 substrates, all conformant |
| [acme-corporation](acme-corporation/) | Five-table variant of the acme theme |
| [customer-fullname](customer-fullname/) | Hello World — single-table string concat formula |
| [nakedclaude-v1](nakedclaude-v1/) through [nakedclaude-v4](nakedclaude-v4/) | Progression showing the same problem solved without ERB (v1–v3) then with ERB (v4) |

The toys open the door. The [domain examples](../rulebook-examples/) show how far it actually goes.
