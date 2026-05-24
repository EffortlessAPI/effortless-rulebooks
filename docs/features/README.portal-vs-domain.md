# Admin Portal ≠ a Domain

> **The portal is the app (`erb_admin_portal` DB); a domain is a document the app opens (`erb_<domain>` DB). The name 'admin' never appears with a domain suffix — `erb_admin_<domain>` is a category-error red flag.**

Word's install directory isn't named after any document; a .docx isn't named after Word. Two connections, two purposes, two categories.

---

This is a stub README. The formal source of truth for this feature is row `feature-014` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Project rulebook ≠ demo rulebook](README.project-vs-demo.md) — the parallel category boundary at the file level
- [Write-through invariant](README.write-through.md) — the portal maintains two DB connections: one to `erb_admin_portal`, one to `erb_<domain>`
- [Admin Portal Screens (derived)](../derived/screens.md) — the 20 screens generated from the platform rulebook
