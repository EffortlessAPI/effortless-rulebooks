# effortless -clean / implicit clean before build

> **`effortless -clean` removes every file whose OverwriteMode is ALWAYS, plus every NEVER-mode file that still matches the original generator output — both are Derivative Code by definition. Every build runs this implicitly first, so 'following along' is the default.**

Clean is the mechanism that lets ADP be measured. OverwriteMode=ALWAYS files are always derivative; OverwriteMode=NEVER files are derivative as long as their content still matches the generator output. Once a NEVER file is customized, it crosses into Hand Code and clean leaves it alone.

---

This is a stub README. The formal source of truth for this feature is row `feature-002` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Abstract Derivative Percentage (ADP)](README.ADP.md) — what clean enables you to measure
- [Convergent builds](README.convergent-build.md) — clean + rebuild = full convergence
- [Platform Features (derived)](../derived/features.md) — full feature catalog
