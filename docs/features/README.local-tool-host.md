# Local Transpiler Host

> **The effortless CLI hosts this repo's own transpilers on `127.0.0.1:4242`, so repo-local transpilers and officially-licensed ones look identical to a build.**

Each tool is a folder under `effortless-tools/<name>/` whose `transpiler.py` calls `orchestration/local_tool_shim.py`; the shim hands the injector the rulebook the CLI delivered. `effortless serve -port 4242` from the repo root keeps one host resident, and `GET /` lists every route.

---

This is a stub README. The formal source of truth for this feature is row `feature-015` in the `PlatformFeatures` table of [`effortless-rulebook/effortless-rulebook.json`](../../effortless-rulebook/effortless-rulebook.json); the routes themselves are the `LocalToolRoutes` table. Edit the rulebook directly; this file MUST conform to that row.

## History

Until 2026-09-12 this was `ssotme-proxy`, a hand-rolled 395-line HTTP server. It existed only because the CLI of the day POSTed an empty request body: the proxy had to work out which project was building by finding the CLI process behind the inbound TCP connection (`lsof`) and reading its working directory, which hard-coded the repo's folder layout and ignored the build step's own `-i`. CLI v2026.09.11.1616 hands a local tool its input directly, so the workaround was deleted.

## See also

- [Hub-and-spoke topology](README.hub-and-spoke.md) — the host is how new spokes are added without touching the hub
- [Execution Substrates (derived)](../derived/substrates.md) — the substrates currently exposed as routes
- [Substrate Contract (derived)](../derived/substrate-contract.md) — the inject / execute / grade protocol each route must implement
