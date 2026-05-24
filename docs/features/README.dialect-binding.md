# Per-Rulebook Formula Dialect

> **Each rulebook declares its formula dialect (Excel, Airtable, …). Substrates honor the declared dialect; the conformance claim is 'all substrates compute identically under THIS rulebook's declared dialect.'**

Excel is the current default because domain experts can read it, but it is not invariant. Dialect is metadata on the rulebook, not a property of ERB.

---

This is a stub README. The formal source of truth for this feature is row `feature-012` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [What is non-linguistic?](../what-is-non-linguistic.md) — formula text is a payload in a typed slot; the dialect is metadata on that slot
- [Formula Dialects (derived)](../derived/formula-dialects.md) — the full list of supported dialects
- [Conformance testing](README.conformance.md) — conformance is scoped to the declared dialect, not to ERB globally
