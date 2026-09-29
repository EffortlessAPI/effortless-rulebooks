"""Generated transparent substrate for R0 spec 'warehouse-network'.
One function per source component; do not edit by hand."""

def c_pred_Hist_Warehouse(S):
    return S["Hist_Warehouse"]

def c_pred_Hist_Route(S):
    return S["Hist_Route"]

def c_pred_Hist_Shipment(S):
    return S["Hist_Shipment"]

def c_pred_Now(S):
    return S["Now"]

def c_pred_Cmd_AddRoute(S):
    return S["Cmd_AddRoute"]

def c_pred_Cmd_CloseRoute(S):
    return S["Cmd_CloseRoute"]

def c_pred_Cmd_CorrectShipment(S):
    return S["Cmd_CorrectShipment"]

def c_pred_Cmd_Reroute(S):
    return S["Cmd_Reroute"]

def c_pred_Cmd_Observe(S):
    return S["Cmd_Observe"]

def c_rule_tau_Retracted_Warehouse(S):
    out = set()
    for (n, _t1, id, vf, vt, k1) in _dispatch("Hist_Warehouse", S):
        if not (_t1 == 'assert'): continue
        for (m, _t2, _t3, _t4, _t5, k2) in _dispatch("Hist_Warehouse", S):
            if not (_t2 == 'retract'): continue
            if not (_t3 == id): continue
            if not (_t4 == vf): continue
            if not (_t5 == vt): continue
            for (v, k) in _dispatch("Now", S):
                if not (m > n): continue
                if not (k2 <= k): continue
                out.add((n,))
    return out

def c_pred_Retracted_Warehouse(S):
    if "Retracted_Warehouse" not in S:
        S["Retracted_Warehouse"] = set()
        S["Retracted_Warehouse"] |= c_rule_tau_Retracted_Warehouse(S)
    return S["Retracted_Warehouse"]

def c_rule_tau_Warehouse(S):
    out = set()
    for (n, _t6, id, vf, vt, k1) in _dispatch("Hist_Warehouse", S):
        if not (_t6 == 'assert'): continue
        for (v, k) in _dispatch("Now", S):
            if not (k1 <= k): continue
            if not (vf <= v): continue
            if not (v < vt): continue
            if any(True for (_t7,) in _dispatch("Retracted_Warehouse", S) if _t7 == n): continue
            out.add((id,))
    return out

def c_pred_Warehouse(S):
    if "Warehouse" not in S:
        S["Warehouse"] = set()
        S["Warehouse"] |= c_rule_tau_Warehouse(S)
    return S["Warehouse"]

def c_rule_tau_Retracted_Shipment(S):
    out = set()
    for (n, _t8, id, wh, day, vf, vt, k1) in _dispatch("Hist_Shipment", S):
        if not (_t8 == 'assert'): continue
        for (m, _t9, _t10, _t11, _t12, _t13, _t14, k2) in _dispatch("Hist_Shipment", S):
            if not (_t9 == 'retract'): continue
            if not (_t10 == id): continue
            if not (_t11 == wh): continue
            if not (_t12 == day): continue
            if not (_t13 == vf): continue
            if not (_t14 == vt): continue
            for (v, k) in _dispatch("Now", S):
                if not (m > n): continue
                if not (k2 <= k): continue
                out.add((n,))
    return out

def c_pred_Retracted_Shipment(S):
    if "Retracted_Shipment" not in S:
        S["Retracted_Shipment"] = set()
        S["Retracted_Shipment"] |= c_rule_tau_Retracted_Shipment(S)
    return S["Retracted_Shipment"]

def c_rule_tau_Shipment(S):
    out = set()
    for (n, _t15, id, wh, day, vf, vt, k1) in _dispatch("Hist_Shipment", S):
        if not (_t15 == 'assert'): continue
        for (v, k) in _dispatch("Now", S):
            if not (k1 <= k): continue
            if not (vf <= v): continue
            if not (v < vt): continue
            if any(True for (_t16,) in _dispatch("Retracted_Shipment", S) if _t16 == n): continue
            out.add((id, wh, day))
    return out

def c_pred_Shipment(S):
    if "Shipment" not in S:
        S["Shipment"] = set()
        S["Shipment"] |= c_rule_tau_Shipment(S)
    return S["Shipment"]

def c_rule_late(S):
    out = set()
    for (id, _t17, day) in _dispatch("Shipment", S):
        if not (day > 10): continue
        out.add((id,))
    return out

def c_pred_Late(S):
    if "Late" not in S:
        S["Late"] = set()
        S["Late"] |= c_rule_late(S)
    return S["Late"]

def c_rule_known_at(S):
    out = set()
    for (_t18, _t19, id, _t20, day, _t21, _t22, k) in _dispatch("Hist_Shipment", S):
        if not (_t19 == 'assert'): continue
        out.add((id, day, k))
    return out

def c_pred_KnownAt(S):
    if "KnownAt" not in S:
        S["KnownAt"] = set()
        S["KnownAt"] |= c_rule_known_at(S)
    return S["KnownAt"]

def c_rule_tau_Retracted_Route(S):
    out = set()
    for (n, _t23, src, dst, cap, vf, vt, k1) in _dispatch("Hist_Route", S):
        if not (_t23 == 'assert'): continue
        for (m, _t24, _t25, _t26, _t27, _t28, _t29, k2) in _dispatch("Hist_Route", S):
            if not (_t24 == 'retract'): continue
            if not (_t25 == src): continue
            if not (_t26 == dst): continue
            if not (_t27 == cap): continue
            if not (_t28 == vf): continue
            if not (_t29 == vt): continue
            for (v, k) in _dispatch("Now", S):
                if not (m > n): continue
                if not (k2 <= k): continue
                out.add((n,))
    return out

def c_pred_Retracted_Route(S):
    if "Retracted_Route" not in S:
        S["Retracted_Route"] = set()
        S["Retracted_Route"] |= c_rule_tau_Retracted_Route(S)
    return S["Retracted_Route"]

def c_rule_tau_Route(S):
    out = set()
    for (n, _t30, src, dst, cap, vf, vt, k1) in _dispatch("Hist_Route", S):
        if not (_t30 == 'assert'): continue
        for (v, k) in _dispatch("Now", S):
            if not (k1 <= k): continue
            if not (vf <= v): continue
            if not (v < vt): continue
            if any(True for (_t31,) in _dispatch("Retracted_Route", S) if _t31 == n): continue
            out.add((src, dst, cap))
    return out

def c_pred_Route(S):
    if "Route" not in S:
        S["Route"] = set()
        S["Route"] |= c_rule_tau_Route(S)
    return S["Route"]

def c_rule_c_selfroute(S):
    out = set()
    for (x, _t32, _t33) in _dispatch("Route", S):
        if not (_t32 == x): continue
        out.add((x,))
    return out

def c_pred_SelfRoute(S):
    if "SelfRoute" not in S:
        S["SelfRoute"] = set()
        S["SelfRoute"] |= c_rule_c_selfroute(S)
    return S["SelfRoute"]

def c_rule_reach_base(S):
    out = set()
    for (x, y, _t34) in _dispatch("Route", S):
        out.add((x, y))
    return out

def c_rule_reach_step(S):
    out = set()
    for (x, z) in _dispatch("Reachable", S):
        for (_t35, y, _t36) in _dispatch("Route", S):
            if not (_t35 == z): continue
            out.add((x, y))
    return out

def c_mu_Reachable(S):
    if "_mu_Reachable" in S: return
    S["_mu_Reachable"] = "running"
    S["Reachable"] = set()
    changed = True
    while changed:
        changed = False
        _new = c_rule_reach_base(S)
        if not _new <= S["Reachable"]:
            S["Reachable"] |= _new
            changed = True
        _new = c_rule_reach_step(S)
        if not _new <= S["Reachable"]:
            S["Reachable"] |= _new
            changed = True
    S["_mu_Reachable"] = "done"

def c_pred_Reachable(S):
    c_mu_Reachable(S)
    return S["Reachable"]

def c_rule_isolated(S):
    out = set()
    for (x,) in _dispatch("Warehouse", S):
        if not (x != 'hub'): continue
        if any(True for (_t37, _t38) in _dispatch("Reachable", S) if _t37 == 'hub' and _t38 == x): continue
        out.add((x,))
    return out

def c_pred_Isolated(S):
    if "Isolated" not in S:
        S["Isolated"] = set()
        S["Isolated"] |= c_rule_isolated(S)
    return S["Isolated"]

def c_rule_c_negcap(S):
    out = set()
    for (s, d, c) in _dispatch("Route", S):
        if not (c < 0): continue
        out.add((s, d))
    return out

def c_pred_NegativeCap(S):
    if "NegativeCap" not in S:
        S["NegativeCap"] = set()
        S["NegativeCap"] |= c_rule_c_negcap(S)
    return S["NegativeCap"]

def c_rule_maxcap(S):
    out = set()
    for (x,) in _dispatch("Warehouse", S):
        _vals = set()
        for (_t39, _t40, c) in _dispatch("Route", S):
            if not (_t39 == x): continue
            _vals.add(c)
        if _vals:
            out.add((x,) + (max(_vals),))
    return out

def c_pred_MaxCap(S):
    if "MaxCap" not in S:
        S["MaxCap"] = set()
        S["MaxCap"] |= c_rule_maxcap(S)
    return S["MaxCap"]

def c_rule_fanout(S):
    out = set()
    for (x,) in _dispatch("Warehouse", S):
        _vals = set()
        for (_t41, y, _t42) in _dispatch("Route", S):
            if not (_t41 == x): continue
            _vals.add(y)
        out.add((x,) + (len(_vals),))
    return out

def c_pred_Fanout(S):
    if "Fanout" not in S:
        S["Fanout"] = set()
        S["Fanout"] |= c_rule_fanout(S)
    return S["Fanout"]

def c_rule_double(S):
    out = set()
    for (s, d, c) in _dispatch("Route", S):
        c2 = (c * 2)
        out.add((s, d, c2))
    return out

def c_pred_DoubleCap(S):
    if "DoubleCap" not in S:
        S["DoubleCap"] = set()
        S["DoubleCap"] |= c_rule_double(S)
    return S["DoubleCap"]

def c_rule_g_reroute_hub(S):
    out = set()
    for (x,) in _dispatch("Cmd_Reroute", S):
        for (_t43,) in _dispatch("Warehouse", S):
            if not (_t43 == x): continue
            if any(True for (_t44, _t45, _t46) in _dispatch("Route", S) if _t44 == x and _t45 == 'hub'): continue
            out.add((x,))
    return out

def c_pred_Guard_RerouteHub(S):
    if "Guard_RerouteHub" not in S:
        S["Guard_RerouteHub"] = set()
        S["Guard_RerouteHub"] |= c_rule_g_reroute_hub(S)
    return S["Guard_RerouteHub"]

def c_rule_e_reroute_hub(S):
    out = set()
    for (x,) in _dispatch("Guard_RerouteHub", S):
        for (vf, _t47) in _dispatch("Now", S):
            h = 'hub'
            c = 10
            vt = 4611686018427387904
            out.add((x, h, c, vf, vt))
    return out

def c_pred_Eff_RerouteHub(S):
    if "Eff_RerouteHub" not in S:
        S["Eff_RerouteHub"] = set()
        S["Eff_RerouteHub"] |= c_rule_e_reroute_hub(S)
    return S["Eff_RerouteHub"]

def c_rule_g_reroute_depot(S):
    out = set()
    for (x,) in _dispatch("Cmd_Reroute", S):
        for (_t48,) in _dispatch("Warehouse", S):
            if not (_t48 == x): continue
            if any(True for (_t49, _t50, _t51) in _dispatch("Route", S) if _t49 == x and _t50 == 'depot'): continue
            out.add((x,))
    return out

def c_pred_Guard_RerouteDepot(S):
    if "Guard_RerouteDepot" not in S:
        S["Guard_RerouteDepot"] = set()
        S["Guard_RerouteDepot"] |= c_rule_g_reroute_depot(S)
    return S["Guard_RerouteDepot"]

def c_rule_e_reroute_depot(S):
    out = set()
    for (x,) in _dispatch("Guard_RerouteDepot", S):
        for (vf, _t52) in _dispatch("Now", S):
            h = 'depot'
            c = 10
            vt = 4611686018427387904
            out.add((x, h, c, vf, vt))
    return out

def c_pred_Eff_RerouteDepot(S):
    if "Eff_RerouteDepot" not in S:
        S["Eff_RerouteDepot"] = set()
        S["Eff_RerouteDepot"] |= c_rule_e_reroute_depot(S)
    return S["Eff_RerouteDepot"]

def c_rule_g_observe(S):
    out = set()
    for (tag,) in _dispatch("Cmd_Observe", S):
        out.add((tag,))
    return out

def c_pred_Guard_Observe(S):
    if "Guard_Observe" not in S:
        S["Guard_Observe"] = set()
        S["Guard_Observe"] |= c_rule_g_observe(S)
    return S["Guard_Observe"]

def c_rule_g_correct(S):
    out = set()
    for (id, newday, at) in _dispatch("Cmd_CorrectShipment", S):
        for (_t53, _t54, _t55) in _dispatch("Shipment", S):
            if not (_t53 == id): continue
            out.add((id, newday, at))
    return out

def c_pred_Guard_CorrectShipment(S):
    if "Guard_CorrectShipment" not in S:
        S["Guard_CorrectShipment"] = set()
        S["Guard_CorrectShipment"] |= c_rule_g_correct(S)
    return S["Guard_CorrectShipment"]

def c_rule_ev_corrected(S):
    out = set()
    for (id, newday, _t56) in _dispatch("Guard_CorrectShipment", S):
        out.add((id, newday))
    return out

def c_pred_Ev_ShipmentCorrected(S):
    if "Ev_ShipmentCorrected" not in S:
        S["Ev_ShipmentCorrected"] = set()
        S["Ev_ShipmentCorrected"] |= c_rule_ev_corrected(S)
    return S["Ev_ShipmentCorrected"]

def c_rule_e_correct_retract(S):
    out = set()
    for (id, _t57, _t58) in _dispatch("Guard_CorrectShipment", S):
        for (n, _t59, _t60, wh, day, vf, vt, _t61) in _dispatch("Hist_Shipment", S):
            if not (_t59 == 'assert'): continue
            if not (_t60 == id): continue
            if any(True for (_t62,) in _dispatch("Retracted_Shipment", S) if _t62 == n): continue
            out.add((id, wh, day, vf, vt))
    return out

def c_pred_Eff_CorrectRetract(S):
    if "Eff_CorrectRetract" not in S:
        S["Eff_CorrectRetract"] = set()
        S["Eff_CorrectRetract"] |= c_rule_e_correct_retract(S)
    return S["Eff_CorrectRetract"]

def c_rule_e_correct_assert(S):
    out = set()
    for (id, newday, at) in _dispatch("Guard_CorrectShipment", S):
        for (_t63, wh, _t64) in _dispatch("Shipment", S):
            if not (_t63 == id): continue
            vt = 4611686018427387904
            out.add((id, wh, newday, at, vt))
    return out

def c_pred_Eff_CorrectAssert(S):
    if "Eff_CorrectAssert" not in S:
        S["Eff_CorrectAssert"] = set()
        S["Eff_CorrectAssert"] |= c_rule_e_correct_assert(S)
    return S["Eff_CorrectAssert"]

def c_rule_g_closeroute(S):
    out = set()
    for (s, d) in _dispatch("Cmd_CloseRoute", S):
        for (_t65, _t66, _t67) in _dispatch("Route", S):
            if not (_t65 == s): continue
            if not (_t66 == d): continue
            out.add((s, d))
    return out

def c_pred_Guard_CloseRoute(S):
    if "Guard_CloseRoute" not in S:
        S["Guard_CloseRoute"] = set()
        S["Guard_CloseRoute"] |= c_rule_g_closeroute(S)
    return S["Guard_CloseRoute"]

def c_rule_e_closeroute(S):
    out = set()
    for (s, d) in _dispatch("Guard_CloseRoute", S):
        for (n, _t68, _t69, _t70, c, vf, vt, _t71) in _dispatch("Hist_Route", S):
            if not (_t68 == 'assert'): continue
            if not (_t69 == s): continue
            if not (_t70 == d): continue
            if any(True for (_t72,) in _dispatch("Retracted_Route", S) if _t72 == n): continue
            out.add((s, d, c, vf, vt))
    return out

def c_pred_Eff_CloseRoute(S):
    if "Eff_CloseRoute" not in S:
        S["Eff_CloseRoute"] = set()
        S["Eff_CloseRoute"] |= c_rule_e_closeroute(S)
    return S["Eff_CloseRoute"]

def c_rule_g_addroute(S):
    out = set()
    for (s, d, c) in _dispatch("Cmd_AddRoute", S):
        for (_t73,) in _dispatch("Warehouse", S):
            if not (_t73 == s): continue
            for (_t74,) in _dispatch("Warehouse", S):
                if not (_t74 == d): continue
                if any(True for (_t75, _t76, _t77) in _dispatch("Route", S) if _t75 == s and _t76 == d): continue
                out.add((s, d, c))
    return out

def c_pred_Guard_AddRoute(S):
    if "Guard_AddRoute" not in S:
        S["Guard_AddRoute"] = set()
        S["Guard_AddRoute"] |= c_rule_g_addroute(S)
    return S["Guard_AddRoute"]

def c_rule_ev_addroute(S):
    out = set()
    for (s, d, _t78) in _dispatch("Guard_AddRoute", S):
        out.add((s, d))
    return out

def c_pred_Ev_RouteAdded(S):
    if "Ev_RouteAdded" not in S:
        S["Ev_RouteAdded"] = set()
        S["Ev_RouteAdded"] |= c_rule_ev_addroute(S)
    return S["Ev_RouteAdded"]

def c_rule_e_addroute(S):
    out = set()
    for (s, d, c) in _dispatch("Guard_AddRoute", S):
        for (vf, _t79) in _dispatch("Now", S):
            vt = 4611686018427387904
            out.add((s, d, c, vf, vt))
    return out

def c_pred_Eff_AddRoute(S):
    if "Eff_AddRoute" not in S:
        S["Eff_AddRoute"] = set()
        S["Eff_AddRoute"] |= c_rule_e_addroute(S)
    return S["Eff_AddRoute"]

def c_rule_big(S):
    out = set()
    for (x, m) in _dispatch("MaxCap", S):
        if not (m >= 50): continue
        out.add((x,))
    return out

def c_pred_Big(S):
    if "Big" not in S:
        S["Big"] = set()
        S["Big"] |= c_rule_big(S)
    return S["Big"]

def c_t_AddRoute(S):
    return {"guard": c_pred_Guard_AddRoute(S), "effects": [("assert", "Route", c_pred_Eff_AddRoute(S))], "events": {"RouteAdded": c_pred_Ev_RouteAdded(S)}}

def c_t_CloseRoute(S):
    return {"guard": c_pred_Guard_CloseRoute(S), "effects": [("retract", "Route", c_pred_Eff_CloseRoute(S))], "events": {}}

def c_t_CorrectShipment(S):
    return {"guard": c_pred_Guard_CorrectShipment(S), "effects": [("retract", "Shipment", c_pred_Eff_CorrectRetract(S)), ("assert", "Shipment", c_pred_Eff_CorrectAssert(S))], "events": {"ShipmentCorrected": c_pred_Ev_ShipmentCorrected(S)}}

def c_t_RerouteViaHub(S):
    return {"guard": c_pred_Guard_RerouteHub(S), "effects": [("assert", "Route", c_pred_Eff_RerouteHub(S))], "events": {}}

def c_t_RerouteViaDepot(S):
    return {"guard": c_pred_Guard_RerouteDepot(S), "effects": [("assert", "Route", c_pred_Eff_RerouteDepot(S))], "events": {}}

def c_t_Observe(S):
    return {"guard": c_pred_Guard_Observe(S), "effects": [], "events": {}}

def c_constraint_NegativeCap(S):
    return c_pred_NegativeCap(S)

def c_constraint_SelfRoute(S):
    return c_pred_SelfRoute(S)

def c_obs_reachable(S):
    return c_pred_Reachable(S)

def c_obs_isolated(S):
    return c_pred_Isolated(S)

def c_obs_fanout(S):
    return c_pred_Fanout(S)

def c_obs_maxcap(S):
    return c_pred_MaxCap(S)

def c_obs_big(S):
    return c_pred_Big(S)

def c_obs_double(S):
    return c_pred_DoubleCap(S)

def c_obs_shipment(S):
    return c_pred_Shipment(S)

def c_obs_late(S):
    return c_pred_Late(S)

def c_obs_known_at(S):
    return c_pred_KnownAt(S)

MODEL = {
 "name": "warehouse-network",
 "edb": {
  "Warehouse": [
   "id"
  ],
  "Route": [
   "src",
   "dst",
   "cap"
  ],
  "Shipment": [
   "id",
   "wh",
   "day"
  ]
 },
 "commands": {
  "AddRoute": [
   "s",
   "d",
   "c"
  ],
  "CloseRoute": [
   "s",
   "d"
  ],
  "CorrectShipment": [
   "id",
   "newday",
   "at"
  ],
  "Reroute": [
   "x"
  ],
  "Observe": [
   "tag"
  ]
 },
 "transitions": [
  {
   "name": "AddRoute",
   "command": "AddRoute"
  },
  {
   "name": "CloseRoute",
   "command": "CloseRoute"
  },
  {
   "name": "CorrectShipment",
   "command": "CorrectShipment"
  },
  {
   "name": "RerouteViaHub",
   "command": "Reroute"
  },
  {
   "name": "RerouteViaDepot",
   "command": "Reroute"
  },
  {
   "name": "Observe",
   "command": "Observe"
  }
 ],
 "constraints": [
  "NegativeCap",
  "SelfRoute"
 ],
 "observables": [
  "reachable",
  "isolated",
  "fanout",
  "maxcap",
  "big",
  "double",
  "shipment",
  "late",
  "known_at"
 ],
 "h0": [
  {
   "n": 0,
   "op": "assert",
   "rel": "Warehouse",
   "tuple": [
    "hub"
   ],
   "k": 0
  },
  {
   "n": 1,
   "op": "assert",
   "rel": "Warehouse",
   "tuple": [
    "a"
   ],
   "k": 0
  },
  {
   "n": 2,
   "op": "assert",
   "rel": "Warehouse",
   "tuple": [
    "b"
   ],
   "k": 0
  },
  {
   "n": 3,
   "op": "assert",
   "rel": "Warehouse",
   "tuple": [
    "c"
   ],
   "k": 0
  },
  {
   "n": 4,
   "op": "assert",
   "rel": "Warehouse",
   "tuple": [
    "depot"
   ],
   "k": 0
  },
  {
   "n": 5,
   "op": "assert",
   "rel": "Route",
   "tuple": [
    "hub",
    "c",
    30
   ],
   "k": 0
  },
  {
   "n": 6,
   "op": "assert",
   "rel": "Shipment",
   "tuple": [
    "s1",
    "a",
    8
   ],
   "k": 0
  }
 ]
}


def _dispatch(name, S):
    """Universal lookup: every rule's reads go through here."""
    return globals()["c_pred_" + name](S)
