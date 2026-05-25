# Convergent Builds (not Additive Codegen)

> **`effortless build` makes downstream artifacts mirror the current rulebook — additions appear, removals disappear, renames propagate. A conventional code generator only adds; ERB conforms, in both directions.**

Conventional codegen is one-way: you remove a field from the spec, the generated column lingers until someone notices. ERB is two-way: removing a field from the rulebook removes it from every substrate on the next build. This is what makes the rulebook stay authoritative.

---

This is a stub README. The formal source of truth for this feature is row `feature-004` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.
