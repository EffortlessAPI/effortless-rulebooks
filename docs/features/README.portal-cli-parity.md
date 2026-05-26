# Portal/CLI Parity

> **The admin portal and `./start.sh --cli` are peer interfaces to the same `effortless.json` pipeline. Every portal mutation shells out to the same `effortless` CLI command, so the two surfaces cannot drift.**

Two surfaces, one pipeline. If the portal grows logic the CLI doesn't have, that logic moves into the CLI and the portal calls it.

---

This is a stub README. The formal source of truth for this feature is row `feature-008` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Write-through invariant](README.write-through.md) — the complementary guarantee that portal saves also write to JSON
- [Self-hosting platform](README.self-hosting.md) — the portal and CLI are themselves generated from the platform rulebook
- [Admin Portal API Surface (derived)](../derived/api-surface.md) — the 36 API endpoints the portal surfaces
