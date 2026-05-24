# Fail Loudly, Never Fall Back

> **When a file or value isn't where it's expected, the code fails with the exact expected path. No silent fallbacks. Defaults derived from the SSoT are fine; defaults that substitute a guess are not.**

Silent fallbacks make conformance run against the wrong data and report 100% pass, masking the real failure. A loud failure produces a fixable stack trace.

---

This is a stub README. The formal source of truth for this feature is row `feature-010` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Conformance testing](README.conformance.md) — the surface where silent fallbacks do the most damage
- [Framing Invariants (derived)](../derived/invariants.md) — the full list of anti-patterns that violate this axiom
- [CLAUDE.md](../../CLAUDE.md) — the project-level rules that enforce fail-loud in every injector script
