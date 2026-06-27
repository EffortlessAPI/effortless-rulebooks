#!/usr/bin/env python3
"""Make the first three left-nav items: Cohort Discovery, Case, OWL Conformance.

Promotes `diagnosis.case` and `admin.owl` to TOP-LEVEL nav entries (parent='',
nav_level='top') and orders the three first via SortOrder. Their existing children
(case sub-tabs) stay attached by ParentRouteKey, so the case leaves still nest under
Case. Raw fields only — Depth/FullPath re-derive on build. Idempotent.
"""
import json, collections
RB = 'effortless-rulebook/effortless-rulebook.json'
rb = json.load(open(RB), object_pairs_hook=collections.OrderedDict)
data = rb['RoutingAndNavigation']['data']
by_key = {r.get('RouteKey'): r for r in data}

def set_fields(route_key, **kw):
    r = by_key.get(route_key)
    if not r:
        raise SystemExit(f"missing route {route_key}")
    for k, v in kw.items():
        r[k] = v

# 1) Cohort Discovery first.
set_fields('admin.cohort', SortOrder=1, NavLevel='top', ParentRouteKey='')
# 2) Case — promote to top-level, second.
set_fields('diagnosis.case', SortOrder=2, NavLevel='top', ParentRouteKey='')
# 3) OWL Conformance — promote to top-level, third.
set_fields('admin.owl', SortOrder=3, NavLevel='top', ParentRouteKey='')

# Push the remaining top-level groups below the new top three (keep their internal
# order). Intake=10, Diagnosis=20, Admin=90 already > 3, so nothing else to change;
# but bump Diagnosis/Admin a touch is unnecessary — they stay where they are.

with open(RB, 'w') as f:
    json.dump(rb, f, indent=2, ensure_ascii=False)
    f.write('\n')

# Report the new top-level order.
tops = sorted([r for r in data if not r.get('ParentRouteKey')], key=lambda r: r.get('SortOrder', 9999))
print("New top-level nav order:")
for r in tops:
    print(f"  {r.get('SortOrder'):>3}  {r.get('DisplayName')}  ({r.get('RouteKey')})")
