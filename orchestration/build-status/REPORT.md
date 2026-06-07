# Cross-Domain Build Status

_Generated 2026-05-26 21:34 UTC by `orchestration/build-status-report.py`._

This file is auto-generated. To refresh:

```
bash orchestration/build-all-domains.sh                 # rebuild everything
bash orchestration/build-all-domains.sh --missing       # only new demos
bash orchestration/build-all-domains.sh --failing       # only what's broken
bash orchestration/build-all-domains.sh --domain NAME   # one demo
bash orchestration/build-all-domains.sh --report-only   # re-render from existing logs
```

**Totals:** 1 PASS · 0 PARSER · 0 CONFIG · 0 OTHER · 27 NO_LOG  _(of 28 demos in rulebook-examples/)_

## Status by domain

| Status  | Domain | Root cause | Log |
| ------- | ------ | ---------- | --- |
| NO_LOG  | `acme-corporation` | No log on disk — run build-all-domains.sh --domain acme-corporation | — |
| NO_LOG  | `acme-llc` | No log on disk — run build-all-domains.sh --domain acme-llc | — |
| NO_LOG  | `community-event-planner` | No log on disk — run build-all-domains.sh --domain community-event-planner | — |
| NO_LOG  | `customer-crm` | No log on disk — run build-all-domains.sh --domain customer-crm | — |
| PASS    | `customer-fullname` |  | [log](orchestration/build-status/logs/customer-fullname.log) |
| NO_LOG  | `effortless-banking` | No log on disk — run build-all-domains.sh --domain effortless-banking | — |
| NO_LOG  | `effortless-rulesbooks` | No log on disk — run build-all-domains.sh --domain effortless-rulesbooks | — |
| NO_LOG  | `expense-approval` | No log on disk — run build-all-domains.sh --domain expense-approval | — |
| NO_LOG  | `fantasy-football` | No log on disk — run build-all-domains.sh --domain fantasy-football | — |
| NO_LOG  | `guessing-game` | No log on disk — run build-all-domains.sh --domain guessing-game | — |
| NO_LOG  | `gym-trainer-invoicing` | No log on disk — run build-all-domains.sh --domain gym-trainer-invoicing | — |
| NO_LOG  | `intelligence-taxonomy` | No log on disk — run build-all-domains.sh --domain intelligence-taxonomy | — |
| NO_LOG  | `is-everything-a-language` | No log on disk — run build-all-domains.sh --domain is-everything-a-language | — |
| NO_LOG  | `talisman-advanced` | No log on disk — run build-all-domains.sh --domain talisman-advanced | — |
| NO_LOG  | `talisman-basic` | No log on disk — run build-all-domains.sh --domain talisman-basic | — |
| NO_LOG  | `job-search-rag` | No log on disk — run build-all-domains.sh --domain job-search-rag | — |
| NO_LOG  | `mechanical-kitchen-timer` | No log on disk — run build-all-domains.sh --domain mechanical-kitchen-timer | — |
| NO_LOG  | `naked-claude-vs-effortless-claude` | No log on disk — run build-all-domains.sh --domain naked-claude-vs-effortless-claude | — |
| NO_LOG  | `nakedclaude-v1` | No log on disk — run build-all-domains.sh --domain nakedclaude-v1 | — |
| NO_LOG  | `nakedclaude-v2` | No log on disk — run build-all-domains.sh --domain nakedclaude-v2 | — |
| NO_LOG  | `nakedclaude-v3` | No log on disk — run build-all-domains.sh --domain nakedclaude-v3 | — |
| NO_LOG  | `nakedclaude-v4` | No log on disk — run build-all-domains.sh --domain nakedclaude-v4 | — |
| NO_LOG  | `product-inventory` | No log on disk — run build-all-domains.sh --domain product-inventory | — |
| NO_LOG  | `star-trek` | No log on disk — run build-all-domains.sh --domain star-trek | — |
| NO_LOG  | `therapist-helper-portal` | No log on disk — run build-all-domains.sh --domain therapist-helper-portal | — |
| NO_LOG  | `volunteer-shift-scheduler` | No log on disk — run build-all-domains.sh --domain volunteer-shift-scheduler | — |
| NO_LOG  | `volunteer-shift-scheduler-demo` | No log on disk — run build-all-domains.sh --domain volunteer-shift-scheduler-demo | — |
| NO_LOG  | `wedding-seating-optimizer` | No log on disk — run build-all-domains.sh --domain wedding-seating-optimizer | — |

## Conventions

- `PASS` — orchestrator ran end-to-end with no Traceback / FATAL / RuntimeError / `✗ FAILED` / `Error:` / explicit `raise` in the log. Conformance score may still be < 100% — that's a substrate gap, not a build error.
- `PARSER` — STEP 1 (answer-key generation) raised `Formula recomputation FAILED`. The `Root cause` column shows which operator characters and function names the canonical evaluator did not recognize.
- `CONFIG` — a transpiler was invoked but isn't listed in that demo's `effortless.json` `ProjectTranspilers`.
- `OTHER` — failure pattern outside the categories above; check the log.
- `NO_LOG` — no log on disk yet for this demo.
