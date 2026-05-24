# Conformance Testing Across Substrates

> **Every build runs every substrate's conformance test against a locally-designated answer key and shows the pass/fail matrix. Substrate agreement is the empirical receipt that the rulebook is the SSoT.**

There is no 'build without testing'. Build = generate + test + regenerate report + open. If a substrate has no take-test, that is a bug to fix, not a case to handle.

---

This is a stub README. The formal source of truth for this feature is row `feature-006` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [ExplainDAG](README.explain-dag.md) — the substrate that produces witnessed derivation proofs; can serve as a substrate-neutral oracle
- [Locally-designated SSoT](README.local-ssot.md) — which substrate is the answer key for a given run
- [No privileged substrate](README.substrate-equivalence.md) — why no substrate is the reference implementation
- [Execution Substrates (derived)](../derived/substrates.md) — full catalog with maturity and answer-key status
- [Substrate Contract (derived)](../derived/substrate-contract.md) — the inject / execute / grade protocol
