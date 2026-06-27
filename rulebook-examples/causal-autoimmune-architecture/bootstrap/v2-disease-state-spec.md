# v2 Disease-State Spec — the disease-state simulator the audit asked for

> **Demonstration of inference structure, NOT validated clinical decision support.**
> This is the Step-2 design spec (shadle-style process) for the two load-bearing deep DAGs
> of the v2 audit response. Step 3 implements it in the rulebook hub. Every transition
> trigger below bottoms out in a **raw leaf** — the same trust boundary as v1's keystone.
> See `V2-RESPONSE-PROPOSAL.md` for why these two, and `derivation-spec.md` for the v1 chain
> this extends.

---

## 0. The audit sentence we are making computable

> *"This patient is transitioning from presymptomatic autoimmunity to early lupus nephritis,
> has rising anti-dsDNA and falling complement, is at risk of renal flare, should be evaluated
> for biopsy, and would likely respond differently to mycophenolate vs belimumab vs
> anifrolumab."* — the v1 audit, defining the gap between an *evidence gate* and a
> *disease-state simulator*.

Two derivations make this sentence an **output of the model**, not a label:
1. **Lupus-nephritis progression** — a witnessed state machine whose current state is derived
   from raw serology trajectories.
2. **Treatment-line selection** — a derived recommendation (MMF / belimumab / anifrolumab) with
   a single deciding reason, given the patient's confirmed mechanism + current disease state.

Plus the connective tissue the sentence assumes: a **derived disease-activity score**
(SLEDAI-style) and **bitemporal dwell-time** ("has been transitioning", "at risk").

---

## 1. NEW RAW LEAVES (the only hand/LLM-entered inputs) — `SerologyObservations`

One row = one longitudinal serology panel for an individual at a timepoint. This is the
**lab** layer — exactly what v1 lets the LLM/intake produce. Nothing here is derived.

| Field | Type | Raw? | Notes |
|---|---|---|---|
| `SerologyObservationId` | string | raw PK | `sero-{ind}-{seq}` |
| `Individual` | string | raw FK→Individuals | whose panel |
| `ObservedAt` | datetime | raw | **valid-time** of the observation |
| `SequenceIndex` | number | raw | 1-based order within the individual's panel series |
| `AntiDsDnaIU` | number | raw | anti-dsDNA titre (IU/mL). The headline SLE serology. |
| `ComplementC3` | number | raw | C3 (mg/dL). Falls in active nephritis. |
| `ComplementC4` | number | raw | C4 (mg/dL). Falls in active nephritis. |
| `ProteinuriaGPerDay` | number | raw | urine protein g/day. >0.5 = significant; >3 = nephrotic-range. |
| `EgfrMlMin` | number | raw | renal function. |
| `HasActiveUrinarySediment` | boolean | raw | casts/hematuria — active nephritis sign. |
| **derived ↓** | | | |
| `PriorObservation` | relationship (self) | calc | prior panel in this individual's series (SequenceIndex-1) |
| `PriorAntiDsDnaIU` | lookup | calc | prior titre, for trend |
| `PriorC3` / `PriorC4` | lookup | calc | prior complement, for trend |
| `AntiDsDnaTrend` | calc | | `Rising` if `AntiDsDnaIU > PriorAntiDsDnaIU*1.25`; `Falling` if `< *0.8`; else `Stable` |
| `ComplementTrend` | calc | | `Falling` if `(C3+C4) < (PriorC3+PriorC4)*0.85`; `Rising` if `> *1.15`; else `Stable` |
| `IsNephroticRangeProteinuria` | calc | | `ProteinuriaGPerDay >= 3.0` |
| `IsSignificantProteinuria` | calc | | `ProteinuriaGPerDay >= 0.5` |
| `SledaiRenalPoints` | calc | | SLEDAI-style renal sub-score from proteinuria + sediment (0/4/8) |
| `SledaiSerologyPoints` | calc | | from low-complement + raised-dsDNA (0/2/4) |

**Trust-boundary note:** the *trend* fields are derived (they are arithmetic on two raw
numbers), so even "rising/falling" is auditable, not asserted. The LLM emits only the numbers.

---

## 2. NEW DERIVED ACTIVITY SCORE — extend `ClinicalPhenotypes`

The audit named "real disease activity scores". We DERIVE one rather than hand-enter it. Add
to `ClinicalPhenotypes` (or a small `DiseaseActivityAssessments` table; see Step 3 decision):

| Field | Type | Formula sketch |
|---|---|---|
| `LatestSerology` | lookup | the individual's max-SequenceIndex SerologyObservation |
| `SledaiScore` | calc | `SledaiRenalPoints + SledaiSerologyPoints (+ existing organ points)` from latest serology |
| `ActivityTier` | calc | `>=12 → "High / flare", 6..11 → "Moderate", 1..5 → "Mild", 0 → "Quiescent"` |
| `IsHighActivity` | calc | `SledaiScore >= 12` |

DAS28 (RA) is represented symmetrically but seeded sparsely (PoC scope: SLE carries the deep
example; RA gets the schema + one illustrative row).

---

## 3. THE LUPUS-NEPHRITIS PROGRESSION MACHINE (deep DAG #1)

A new state machine on the existing polymorphic substrate. **Subject = Individual**;
**SubjectStateColumn = `NephritisProgressionStateKey`** (a NEW derived field on `Individuals`).

### 3.1 States (`MachineStates`, machine = `lupus-nephritis-progression`)

| StateKey | Title | Order | Initial | Terminal |
|---|---|--:|:--:|:--:|
| `PresymptomaticAutoimmunity` | Presymptomatic autoimmunity (serology only) | 1 | ✓ | |
| `SerologicActive` | Serologically active (rising dsDNA / falling complement) | 2 | | |
| `EarlyNephritis` | Early nephritis (significant proteinuria) | 3 | | |
| `RenalFlareRisk` | At risk of renal flare (nephrotic-range / active sediment) | 4 | | |
| `BiopsyIndicated` | Biopsy indicated | 5 | | ✓ |
| `Quiescent` | Quiescent / in remission | 0 | | ✓ |

### 3.2 The derived current-state formula (`Individuals.NephritisProgressionStateKey`)

Bottoms out ENTIRELY in raw serology leaves (via the individual's latest SerologyObservation):

```
= IF(IsNephroticRangeProteinuria OR HasActiveUrinarySediment, "BiopsyIndicated",
     IF(ProteinuriaGPerDay >= 1.0,                              "RenalFlareRisk",
     IF(IsSignificantProteinuria,                               "EarlyNephritis",
     IF(AntiDsDnaTrend="Rising" AND ComplementTrend="Falling",  "SerologicActive",
        "PresymptomaticAutoimmunity"))))
```

This is the keystone of the simulator: a patient's **disease state is computed from labs**,
never set. Editing a lab nudges the state (spreadsheet-cell property); there is no
"state" knob anyone can flip to fake the trajectory.

### 3.3 Transition rules (`StateTransitionRules`) — each guard cites a raw leaf

| From → To | GuardDescription (raw-leaf trigger) | RuleRefs |
|---|---|---|
| Presymptomatic → SerologicActive | anti-dsDNA Rising AND complement Falling | `SERO-ACTIVE` |
| SerologicActive → EarlyNephritis | proteinuria ≥ 0.5 g/day | `NEPH-ONSET` |
| EarlyNephritis → RenalFlareRisk | proteinuria ≥ 1.0 g/day | `FLARE-RISK` |
| RenalFlareRisk → BiopsyIndicated | nephrotic-range proteinuria OR active urinary sediment | `BIOPSY-IND` |
| (any) → Quiescent | dsDNA Falling AND complement Rising AND proteinuria < 0.5 | `REMISSION` |

### 3.4 Bitemporal dwell-time (`SubjectStateInstances`, machine-scoped)

Add to the existing occupancy table (or compute on it):
- `DwellDays` = `IF(IsCurrent, NOW - EnteredAt, ExitedAt - EnteredAt)`
- `IndividualDaysInCurrentState` (rollup on Individuals) — answers the audit's "how long has
  the patient/disease been in this state".
- Evidence/tests/serology rows are **tied to the occupancy window** by `ObservedAt ∈
  [EnteredAt, ExitedAt)`, so the chart shows what was true *while in that state*.

---

## 4. TREATMENT-LINE SELECTION (deep DAG #2) — extend `Treatments` / `IndividualPredictions`

The audit's second worked example: "would likely respond differently to MMF vs belimumab vs
anifrolumab." We DERIVE a recommended line + single deciding reason from (a) the confirmed
causal **mechanism** and **its target pathway**, and (b) the **disease state** (§3) and
**activity** (§2).

> **Data reality check (done before writing this):** in v1 every mechanism's `MechanismType` is
> the same value (`regulatory`) — it classifies *variant regulatory class*, not *therapeutic
> target*. So the treatment-line example needs a finer, genuinely-varying input. We add ONE new
> **raw leaf** to `CausalMechanisms`: `TargetPathway` ∈ {`type-I-IFN`, `B-cell/autoantibody`,
> `T-cell-costim`, `IL-17/23`}. This is an *observation about the confirmed mechanism* (which
> druggable pathway it implicates) — legitimately a leaf, not a derived value — and it is the
> input that makes MMF vs belimumab vs anifrolumab actually differentiate across patients.
> It rides the trust boundary correctly: raw in, recommendation derived.

### 4.1 Therapy ontology — small `TherapyOptions` lookup

| TherapyOptionId | Label | TargetsMechanismType | LineOrdinal | PreferredWhen |
|---|---|---|--:|---|
| `mmf` | Mycophenolate (MMF) | (induction, organ-level) | 1 | active nephritis (proteinuria-driven) |
| `belimumab` | Belimumab (anti-BLyS) | B-cell / autoantibody | 2 | serologically active, dsDNA-driven |
| `anifrolumab` | Anifrolumab (anti-IFNAR1) | type-I-IFN pathway | 2 | IFN-signature mechanism |

### 4.2 Derived recommendation (`IndividualPredictions.RecommendedTreatmentLine` + `...DecidingFactor`)

```
RecommendedTherapy =
  IF(NOT RestsOnConfirmedMechanism, "No targeted line — mechanism unconfirmed",
  IF(NephritisProgressionStateKey IN ("RenalFlareRisk","BiopsyIndicated"), "Mycophenolate (induction)",
  IF(TargetPathway = "type-I-IFN",                                          "Anifrolumab",
  IF(TargetPathway = "B-cell/autoantibody",                                 "Belimumab",
     "Standard-of-care (no mechanism-matched targeted line)"))))

TreatmentLineDecidingFactor =  -- the single reason, mirrors v1's DecidingGate style
  IF(NOT RestsOnConfirmedMechanism, "MechanismUnconfirmed",
  IF(state in flare-set,            "ActiveNephritis→Induction",
  IF(TargetPathway="type-I-IFN",    "IFNSignature→Anifrolumab",
  IF(TargetPathway="B-cell/autoantibody", "AutoantibodyDriven→Belimumab",
     "NoMechanismMatch"))))
```

**Why this is not laundering:** the recommendation is a pure function of the *already-witnessed*
mechanism node + the *raw-leaf-derived* disease state. Two patients with the same disease but
different confirmed mechanisms get different lines, and the reason is inspectable to the leaf.

---

## 5. THE MINIMUM COUNTER-EXAMPLE (Step 6 target, designed here)

At least one patient must show **disease-state simulator and actionability gate DISAGREE**:

- **Disease IS progressing**: `NephritisProgressionStateKey ∈ {RenalFlareRisk, BiopsyIndicated}`
  and `IsHighActivity = TRUE` (the simulator fires — there is real, worsening disease).
- **Prediction is NOT actionable**: `IsClinicallyActionable = FALSE` on one gate (e.g. the
  prediction's *mechanism* is spurious or fails ancestry transport).

Reading: *"This patient is clearly progressing toward a renal flare and needs clinical
attention — but our **prediction about their causal mechanism** is not trustworthy enough to
act on for targeted therapy."* That sentence is only sayable if the two layers are independent
and both real. It is the single best proof that v2 ≠ v1-relabeled.

Candidate: **Diego Santos (ind-d-santos)** — already has "biopsy-confirmed nephritis" in v1 and
fails the keystone on **cryptic-relatedness**. Give Diego a rising-dsDNA/falling-complement/
nephrotic serology series → simulator says `BiopsyIndicated` + high activity, while the gate
still says NOT actionable (cryptic relatedness). Disagreement achieved without touching his v1
deciding gate.

---

## 6. Implementation order (Step 3)

1. Add `SerologyObservations` table (raw leaves + trend/score calcs).
2. Add `TherapyOptions` lookup.
3. Add derived fields on `Individuals` (`NephritisProgressionStateKey`, dwell rollups) and
   `IndividualPredictions` (`RecommendedTreatmentLine`, `TreatmentLineDecidingFactor`) and
   `ClinicalPhenotypes`/activity (`SledaiScore`, `ActivityTier`).
4. Add `lupus-nephritis-progression` machine: `StateMachines` row + 6 `MachineStates` + 5
   `StateTransitionRules` + per-patient `SubjectStateInstances` occupancy (bitemporal) +
   `StateTransitions` log.
5. Seed serology series for the 7 + 2 new patients so each lands on an intended state; ensure
   Diego is the disagreement case.
6. `effortless build`; bootstrap new tables (01→02→03→05 with ON_ERROR_STOP once); verify
   keystone verdicts UNCHANGED and new derived states correct.

**Invariant to protect:** v1's seven `IsClinicallyActionable` verdicts and each deciding gate
stay byte-for-byte. The disease-state layer is strictly additive.
