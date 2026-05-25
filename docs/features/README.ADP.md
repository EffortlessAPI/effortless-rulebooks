# Abstract Derivative Percentage (ADP)

> **A first-class, measurable percentage of any ERB project that is derivative (mechanically rebuildable) vs. hand-written — `effortless -clean` deletes everything derivative, build restores it, and the LOC delta is the ADP.**

ADP draws a bright red line between Derivative Code (rebuildable on demand) and Hand Code (would be permanently lost without backup). Measured as (LOC_before_cleanall - LOC_after_cleanall) / LOC_before_cleanall. A pure scaffold starts at ~100%; a typical ERB project lands at 60-80%.

---

This is a stub README. The formal source of truth for this feature is row `feature-001` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.
