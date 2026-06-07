#!/usr/bin/env python3
"""
loop-07b-fix-screenlayout-fk.py  (idempotent repair)

Loop 7 left a transpiler FK-inference collision: ScreenLayouts.RouteKey is a
relationship -> PlatformNaviation, and because PlatformNaviation ALSO has columns
named RouteKey / ParentRouteKey, the transpiler emitted BOGUS reverse FKs:

  platform_naviation.route_key        -> screen_layouts(screen_layout_id)
  platform_naviation.parent_route_key -> screen_layouts(screen_layout_id)

Those are wrong (PlatformNaviation.RouteKey/ParentRouteKey are raw self-keys,
not FKs to ScreenLayouts), and they fail to apply at build time because the nav
route keys are not screen_layout_ids.

Fix: rename the ScreenLayouts relationship field RouteKey -> Nav (FK by nav PK)
and repoint every value from a bare RouteKey to the nav PK ("nav-" + dots->dashes).
This removes the route_key name collision, so the bogus reverse FKs are no longer
generated, while keeping a real, correct ScreenLayouts -> PlatformNaviation FK.

Idempotent: if the field is already named Nav and values are nav PKs, 0 changes.

Run:
  python3 scripts/loop-07b-fix-screenlayout-fk.py --dry-run
  python3 scripts/loop-07b-fix-screenlayout-fk.py
"""
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
TTC_ROOT = os.path.dirname(HERE)
RB_PATH = os.path.join(TTC_ROOT, "effortless-rulebook",
                       "traffic-ticket-contest-rulebook.json")
DRY = "--dry-run" in sys.argv


def navpk(routekey):
    return "nav-" + routekey.replace(".", "-")


def main():
    rb = json.load(open(RB_PATH))
    sl = rb["ScreenLayouts"]
    sch = sl["schema"]
    fields = sch.get("fields", sch) if isinstance(sch, dict) else sch
    nav_pks = {r["PlatformNaviationId"] for r in rb["PlatformNaviation"]["data"]}
    nav_by_rk = {r["RouteKey"]: r["PlatformNaviationId"]
                 for r in rb["PlatformNaviation"]["data"]}

    # 1. rename schema field RouteKey -> Nav (if present)
    renamed = False
    for f in fields:
        if f.get("name") == "RouteKey" and f.get("type") == "relationship":
            f["name"] = "Nav"
            f["Description"] = "FK -> PlatformNaviation (by nav PK)."
            f["RelatedTo"] = "PlatformNaviation"
            renamed = True
        # remove any leftover Nav duplicate not needed

    # 2. repoint data: RouteKey -> Nav, value bare-key -> nav PK
    repointed = 0
    unresolved = []
    for row in sl["data"]:
        val = None
        if "RouteKey" in row:
            val = row.pop("RouteKey")
        if "Nav" in row and row["Nav"]:
            val = row["Nav"]
        if val is None:
            continue
        # normalize to nav PK
        if val in nav_pks:
            pk = val
        elif val in nav_by_rk:
            pk = nav_by_rk[val]
        else:
            # try interpreting a bare/dotted key
            cand = navpk(val)
            if cand in nav_pks:
                pk = cand
            else:
                unresolved.append((row.get("ScreenLayoutId"), val))
                continue
        if row.get("Nav") != pk:
            row["Nav"] = pk
            repointed += 1

    # 3. RE-KEY ScreenLayoutId so its values no longer COLLIDE with nav
    #    RouteKey values. The transpiler does VALUE-BASED FK inference: because
    #    every platform_naviation.route_key value is also a screen_layout_id, it
    #    invents platform_naviation.route_key -> screen_layouts FKs. Prefixing the
    #    PK with "sl-" breaks that overlap. ScreenSections.ScreenLayout FKs are
    #    repointed to the new ids.
    rekeyed = 0
    id_map = {}
    for row in sl["data"]:
        old = row.get("ScreenLayoutId")
        if old is None:
            continue
        new = old if old.startswith("sl-") else "sl-" + old
        id_map[old] = new
        if new != old:
            row["ScreenLayoutId"] = new
            rekeyed += 1
    # fix the Name formula reference (it mirrors ScreenLayoutId; fine as-is)
    # repoint ScreenSections.ScreenLayout
    sec_fixed = 0
    for row in rb["ScreenSections"]["data"]:
        cur = row.get("ScreenLayout")
        if cur in id_map and row["ScreenLayout"] != id_map[cur]:
            row["ScreenLayout"] = id_map[cur]
            sec_fixed += 1

    # gate: every Nav value resolves to a nav PK; every section FK resolves
    sl_ids = {r["ScreenLayoutId"] for r in sl["data"]}
    bad = [(r.get("ScreenLayoutId"), r.get("Nav")) for r in sl["data"]
           if r.get("Nav") and r["Nav"] not in nav_pks]
    bad_sec = [(r.get("ScreenSectionId"), r.get("ScreenLayout"))
               for r in rb["ScreenSections"]["data"]
               if r.get("ScreenLayout") and r["ScreenLayout"] not in sl_ids]
    # gate: no ScreenLayoutId may equal a nav RouteKey anymore
    nav_rks = {r["RouteKey"] for r in rb["PlatformNaviation"]["data"]}
    collide = sorted(sl_ids & nav_rks)

    print("=== loop-07b fix-screenlayout-fk ===")
    print("schema field RouteKey->Nav renamed:", renamed)
    print("data values repointed to nav PK   :", repointed)
    print("ScreenLayoutId re-keyed (sl- pfx) :", rekeyed)
    print("ScreenSections.ScreenLayout fixed :", sec_fixed)
    print("unresolved Nav (no matching nav)  :", len(unresolved), unresolved[:8])
    print("--- gates (all 0) ---")
    print("Nav -> bad nav PK                 :", len(bad), bad[:8])
    print("section -> bad ScreenLayout        :", len(bad_sec), bad_sec[:8])
    print("ScreenLayoutId colliding w/ RouteKey:", len(collide), collide[:8])

    if unresolved or bad or bad_sec or collide:
        print("\nABORT: gate(s) failed.")
        sys.exit(1)

    if DRY:
        print("\n[dry-run] no write")
        return
    with open(RB_PATH, "w") as fh:
        fh.write(json.dumps(rb, indent=2, ensure_ascii=False) + "\n")
    print("\nwrote", RB_PATH)


if __name__ == "__main__":
    main()
