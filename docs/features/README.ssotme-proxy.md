# ssotme-proxy Transpiler Bus

> **A local HTTP server on `localhost:4242` exposes every injector as a first-class `ssotme://` transpiler route, so repo-local transpilers and officially-licensed ones look identical to the CLI.**

POST /rulebook-to-python, POST /rulebook-to-postgres, … Each route runs the corresponding injector. The proxy IS the mechanism, not a wrapper.

---

This is a stub README. The formal source of truth for this feature is row `feature-015` in the `PlatformFeatures` table of [`effortless-platform/effortless-rulebook/effortless-rulebook.json`](../../effortless-platform/effortless-rulebook/effortless-rulebook.json). Edit through the admin portal (Home → Platform Features) or directly in the rulebook; this file MUST conform to that row.

## See also

- [Hub-and-spoke topology](README.hub-and-spoke.md) — the proxy is how new spokes are added without touching the hub
- [Execution Substrates (derived)](../derived/substrates.md) — the substrates currently exposed as proxy routes
- [Substrate Contract (derived)](../derived/substrate-contract.md) — the inject / execute / grade protocol each proxy route must implement
