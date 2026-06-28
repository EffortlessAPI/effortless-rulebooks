# CR-2 draft — transitive closure on the disease-progression machine

**Status:** DRAFT for review. Nothing has touched the rulebook hub yet.
**Mechanic: PROVEN.** This project's pinned `rulebook-to-postgres` (v2026.06.14.1635) already
emits `vw_state_transition_rules_closure` (a cycle-safe `WITH RECURSIVE` view with
`hop_distance` / `is_inferred`) from a single declarative `closure` field — verified by a throwaway
build. The published `rulebook-to-owl` (v2026.06.21.0954) is the real tool and projects the model.

---

## What requires transitive closure (the medical story)

`StateTransitionRules` already stores disease progression as **sparsely-asserted `FromState → ToState`
edges** — but every place the model asks a *reachability* question about that progression, it answers
with a hand-typed integer ladder instead of deriving it from the edges. Closure replaces the magic
numbers with inference. Two machines, both edge-sets:

- **`lupus-nephritis-progression`** (5 forward edges): `PresymptomaticAutoimmunity → SerologicActive →
  EarlyNephritis → RenalFlareRisk → BiopsyIndicated` (+ remission edge `SerologicActive → Quiescent`).
- **`diagnosis-lifecycle`** (9 edges): the four-gate DAG converging on `Actionable` / `NotActionable`.

The closure derives the **never-asserted long-range pairs** — e.g. `PresymptomaticAutoimmunity →
BiopsyIndicated` (hop_distance 4, `is_inferred = true`). That is the autoimmune analogue of
Talisman's `step-1 → step-5`: the medical claim "a presymptomatic patient is on the path to
biopsy-indicated nephritis" becomes *inferred from the transition topology*, not asserted by a number.

---

## The change — three parts

### Part A — declare the closure (the Postgres witness)

Add ONE `closure` field to `StateTransitionRules`, mirroring Talisman's `StepPrecedence.PrecedesStepClosure`:

```json
{
  "name": "ProgressionClosure",
  "datatype": "string",
  "type": "closure",
  "nullable": true,
  "important": true,
  "Description": "Transitive closure of state-machine transitions (an owl:TransitiveProperty). The asserted FromState→ToState edges imply the full reachability ordering — including never-asserted long-range pairs like PresymptomaticAutoimmunity → BiopsyIndicated. Materialized by the transpiler as vw_state_transition_rules_closure(from_id, to_id, hop_distance, is_inferred): asserted edges (hop 1) + inferred rows.",
  "EdgeTable": "StateTransitionRules",
  "FromColumn": "FromState",
  "ToColumn": "ToState"
}
```

→ transpiler emits `vw_state_transition_rules_closure`. **Confirmed working.** Closes BOTH machines
(the recursion is per-edge-set, so diagnosis-lifecycle and lupus close independently and for free).

### Part B — make the ordering *read the closure* instead of a hand-typed ladder

This is the "stop hand-typing the magic number" half — the same move as Talisman's
`InferredSequencePosition = PrecedingStepCount + 1`.

Today the rank is duplicated in two places that can silently disagree:
- `MachineStates.OrderIndex` — a raw integer (Quiescent=0 … BiopsyIndicated=5).
- `SerologyObservations.ProgressionStateOrder` — an `IF`-ladder hardcoding the SAME 0..5 mapping.

**Proposed:** derive each state's order from the closure, so the integer ladder becomes a *witnessed
projection of the edges*, not an independent column:

- Add `MachineStates.PrecedingStateCount` (aggregation):
  `=COUNTIFS(vw_state_transition_rules_closure!{{ToId}}, MachineStates!{{MachineStateId}})`
  — how many states transitively precede this one. On the lupus forward chain: 0,1,2,3,4 for
  Presymptomatic…BiopsyIndicated.
- Add `MachineStates.InferredOrderIndex` (calculated): `={{PrecedingStateCount}}` (forward chain ⇒
  this equals the asserted `OrderIndex`; it's now *derived*, not typed).
- Keep `OrderIndex` as the **override slot** (exactly Talisman's `SequencePositionOverride`):
  `ResolvedOrderIndex = IF(OrderIndex<>"", OrderIndex, InferredOrderIndex)`. The remission edge
  (`SerologicActive → Quiescent`) makes Quiescent's pure-inferred rank ambiguous, so Quiescent keeps
  an explicit override — which is *precisely* what the override slot is for, and we say so.

> **Verdict-contract guardrail.** The 7-patient keystone verdicts ride on
> `NephritisProgressionStateKey` thresholds (`MaxProgressionStateOrder >= 4/5`), which bottom out in
> the SAME 0..5 ranks. Because `InferredOrderIndex == OrderIndex` on the forward chain, every
> downstream value is **bit-identical** before and after. Part B is a provenance upgrade (typed →
> derived), not a value change. **I will diff all 12 individuals' `MaxProgressionStateOrder` /
> `NephritisProgressionStateKey` before vs. after and they must be identical**, or Part B is reverted.

*(Conservative option: ship Part A + Part C only, leave the IF-ladder untouched, and add Part B in a
follow-up once the closure view is confirmed live. Part A+C already produce the OWL closure receipt.
Recommend A+C first, B second — flagged below.)*

### Part C — the OWL axioms (the second, independent witness)

The `closure` field already carries the semantic intent ("an `owl:TransitiveProperty`") in its
Description, and `rulebook-to-owl` reads field metadata. To make the OWL substrate **close
independently** (so OWL-RL agrees with the Postgres CTE) and **catch impossible states**:

1. **Transitive successor property.** Project the transition relation as an `owl:TransitiveProperty`
   so OWL-RL closes `Presymptomatic → … → BiopsyIndicated` the way `reason.py` closes `precedesStep`.
2. **Disjoint states.** Declare the `MachineStates` of a machine `owl:disjointWith` so the reasoner
   catches an individual placed in two states at once (e.g. `Quiescent` ∧ `BiopsyIndicated`).

*Open mechanical question to resolve at apply-time:* whether v0954 emits the `owl:TransitiveProperty`
/ `owl:disjointWith` axioms purely from the `closure` field type + a disjointness hint, or whether it
needs an explicit per-field/per-table axiom hint (a `owl` / `characteristics` property). I'll confirm
by projecting after Part A lands and inspecting the TBox; if the axioms aren't emitted, the fix is a
small hint on the field/table, NOT a remodel. **This is the one unverified step** — everything in A
and B is proven.

---

## The conformance receipt (what we run to prove it)

After build:
1. **Postgres closure** — `SELECT * FROM vw_state_transition_rules_closure` shows the asserted +
   inferred pairs (incl. `presymptomatic→biopsyindicated`, `is_inferred=true`).
2. **OWL closure** — run Talisman's `reason.py` (pyshacl + owlrl) over the generated `ontology.owl` +
   `rules.shacl.ttl`; the OWL-RL deductive closure must contain the same reachability pairs.
3. **Referee** — diff the two closures. **Agreement is the receipt.** Ship a coverage note:
   ordering/reachability + disjointness conform exactly (booleans/edges); the numeric keystone still
   conforms only on the shared deterministic subset (100%*).

This deliberately lives in the closure/disjointness layer — **no float-tolerance issue**, because
agreement there is exact (the CR's own guardrail).

---

## Coverage impact (toward >50%)

- Moves disease-progression ordering from "an integer someone typed" to "reachability **derived** from
  asserted edges + cross-substrate **conformance-checked**" — a genuine depth increase on a row the
  challenge cares about (disease trajectory / staging).
- Adds the **disjoint-state invariant** (a new checkable safety property) and a **second independent
  reasoner witness** for the progression layer — the concrete answer to "does OWL add reasoning here?"
- New harness tests: closure cardinality (asserted vs inferred), the long-range inferred pair exists,
  disjointness catches a synthetic double-occupancy, OWL-RL ≡ Postgres on the reachability set.

---

## Apply order (after approval)

1. Part A (closure field) → `effortless build` → confirm `vw_state_transition_rules_closure` + content.
2. Part C (project to OWL) → inspect TBox → add disjoint/transitive hint if needed → reason.py ≡ Postgres.
3. Part B (rewire ordering to closure) → **diff 12 individuals, must be identical** → keep or revert.
4. Harness tests + coverage-map / README update.

Each step is independently revertible; the verdict contract is gated at step 3.
