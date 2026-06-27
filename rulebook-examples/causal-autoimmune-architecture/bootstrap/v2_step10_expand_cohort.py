#!/usr/bin/env python3
"""
v2 Step 10 — expand the cohort to full claim-bearing members.

The v1 cohort is 7 patients (A-G), each a COMPLETE member: variant -> omics ->
mechanism -> evidence/replication/control/intervention -> calibration -> prediction
(keystone) + phenotype + treatment + serology. Step 3 added H & I as serology-only
PROGRESSION demos (no prediction) — so they never appeared as cohort members.

This step promotes H & I to FULL members and adds J, K, L, so the cohort is 12
deep, each with its own keystone claim, treatment line, and disease state — visible
everywhere the original 7 are.

Method: CLONE a known-good template member (structural copy guarantees the keystone
derives correctly), rewriting every member-scoped id by token substitution
(-a -> -h, cm-a -> cm-h, ...) and the cross-reference FK fields. Shared lookups
(AutoimmuneDisease, Tissue, DiseaseStage, FederatedDataset, EnvironmentalExposure)
keep pointing at existing rows. We then overlay a few RAW leaves per member to set
ancestry / disease-state / the one deciding factor.

INVARIANT: the original 7 members are untouched — their keystone verdicts stay
byte-for-byte. New members are purely additive.

Idempotent: re-running removes any previously-added new-member rows first.
"""
import json, os, copy

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RB = os.path.join(ROOT, "effortless-rulebook", "effortless-rulebook.json")

# tables a member spans, and the FK fields that reference member-scoped ids.
# (lookups to SHARED rows are intentionally NOT rewritten.)
MEMBER_TABLES = {
    "Individuals": {"id": "IndividualId", "scope": "ind"},
    "GenomicVariants": {"id": "GenomicVariantId", "fks": {"Individual": "ind"}},
    "OmicsAssays": {"id": "OmicsAssayId", "fks": {"Individual": "ind"}},
    "CausalMechanisms": {"id": "CausalMechanismId", "fks": {"Individual": "ind", "GenomicVariant": "var"}},
    "EvidenceItems": {"id": "EvidenceItemId", "fks": {"CausalMechanism": "cm", "OmicsAssay": "assay"}},
    "CohortReplications": {"id": "CohortReplicationId", "fks": {"CausalMechanism": "cm"}},
    "NegativeControlTests": {"id": "NegativeControlTestId", "fks": {"CausalMechanism": "cm"}},
    "InterventionTargets": {"id": "InterventionTargetId", "fks": {"CausalMechanism": "cm"}},
    "ClinicalPhenotypes": {"id": "ClinicalPhenotypeId", "fks": {"Individual": "ind"}},
    "Treatments": {"id": "TreatmentId", "fks": {"Individual": "ind", "TargetsMechanism": "cm"}},
    "IndividualPredictions": {"id": "IndividualPredictionId", "fks": {"Individual": "ind"}},
    "CalibrationBins": {"id": "CalibrationBinId", "fks": {"IndividualPrediction": "pred"}},
    "SerologyObservations": {"id": "SerologyObservationId", "fks": {"Individual": "ind", "PriorObservation": "self"}},
}

# A member's id "letter token". Templates use single letters a..g. New members get
# h, i, j, k, l. Every member-scoped id embeds "-<letter>" as a path token; assays
# use "assay-<letter>-...", variants "var-<letter>-...", etc. So substituting the
# template's letter token for the new one rewrites ids AND inline references.

# new member -> (new_letter, tmpl_letter, new_ind_id, tmpl_ind_id, overrides).
# We substitute BOTH the full individual-id stem (a-reyes -> h-yamamoto) AND the
# bare letter token (cm-a -> cm-h, ev-a-1 -> ev-h-1, etc.).
NEW_MEMBERS = [
    ("h", "a", "ind-h-yamamoto", "ind-a-reyes", {
        "Individuals": {"GivenName": "Hana", "FamilyName": "Yamamoto", "AncestryLabel": "East Asian",
                        "AgeYears": 33, "IsAncestryAbsentFromTraining": False,
                        "CaseNarrative": "33yo East Asian woman; confirmed IFN-pathway mechanism, well-calibrated; "
                                         "rising anti-dsDNA with falling complement and new significant proteinuria "
                                         "(0.7 g/day) — early lupus nephritis. Actionable AND progressing."},
        "Treatments": {"TreatmentLabel": "Anifrolumab (anti-IFNAR1)"},
    }),
    # I — Ibrahim Conteh: clone D (cryptic-relatedness FALSE). African, renal-flare-risk -> 2nd disagreement.
    ("i", "d", "ind-i-conteh", "ind-d-santos", {
        "Individuals": {"GivenName": "Ibrahim", "FamilyName": "Conteh", "AncestryLabel": "African",
                        "AgeYears": 45, "IsAncestryAbsentFromTraining": False,
                        "HasCrypticRelatednessFlag": True,
                        "CaseNarrative": "45yo African man; brisk serologic activity and proteinuria climbing to "
                                         "1.5 g/day — at renal-flare risk. A cryptic-relatedness flag makes the "
                                         "prediction NOT actionable: a second disease-vs-evidence disagreement."},
    }),
    # J — Jamal Brooks: clone B (calibration fail). Hispanic/Latino, serologically active.
    ("j", "b", "ind-j-brooks", "ind-b-okafor", {
        "Individuals": {"GivenName": "Jamal", "FamilyName": "Brooks", "AncestryLabel": "Hispanic/Latino",
                        "AgeYears": 52, "IsAncestryAbsentFromTraining": False,
                        "CaseNarrative": "52yo Hispanic/Latino man; the targeted mechanism is confirmed but the "
                                         "calibration bins are under-covered — uncertainty collapses, so NOT actionable "
                                         "on the calibration gate."},
    }),
    # K — Kavya Nair: clone F (ancestry-transport fail, holdout). South Asian holdout.
    ("k", "f", "ind-k-nair", "ind-f-haidar", {
        "Individuals": {"GivenName": "Kavya", "FamilyName": "Nair", "AncestryLabel": "South Asian",
                        "AgeYears": 39, "IsAncestryAbsentFromTraining": True,
                        "CaseNarrative": "39yo South Asian woman, ancestry absent from training; confirmed mechanism "
                                         "but all replications ran in one ancestry — fails ancestry transport, NOT actionable."},
    }),
    # L — Lena Brandt: clone G (all-pass actionable, holdout twin). Indigenous-American, actionable.
    ("l", "g", "ind-l-brandt", "ind-g-lin", {
        "Individuals": {"GivenName": "Lena", "FamilyName": "Brandt", "AncestryLabel": "Indigenous American",
                        "AgeYears": 28, "IsAncestryAbsentFromTraining": True,
                        "CaseNarrative": "28yo Indigenous-American woman, holdout ancestry; the positive twin — the "
                                         "IL23R node replicated cross-ancestry, so transport passes and she is actionable."},
    }),
]

# Serology series for each new member (raw leaves), so each lands on an intended
# disease state. (seq, observed_at, dsDNA, C3, C4, proteinuria, eGFR, sediment)
SEROLOGY = {
    "h": [(1, "2025-07-20", 55, 98, 19, 0.3, 90, False), (2, "2025-10-20", 110, 72, 11, 0.7, 85, False)],  # EarlyNephritis
    "i": [(1, "2025-06-15", 70, 90, 16, 0.5, 84, False), (2, "2025-09-15", 150, 55, 8, 1.5, 76, False)],   # RenalFlareRisk
    "j": [(1, "2025-08-01", 48, 100, 20, 0.2, 92, False), (2, "2025-11-01", 115, 70, 10, 0.3, 90, False)], # SerologicActive
    "k": [(1, "2025-07-05", 30, 120, 25, 0.05, 99, False), (2, "2025-10-05", 33, 118, 24, 0.08, 98, False)], # Presymptomatic
    "l": [(1, "2025-09-05", 22, 130, 28, 0.0, 100, False), (2, "2025-12-05", 24, 128, 27, 0.0, 100, False)], # Presymptomatic
}

# pathway per new member's mechanism (raw leaf, mirrors Step 3's TARGET_PATHWAY).
TARGET_PATHWAY = {"h": "type-I-IFN", "i": "B-cell/autoantibody", "j": "type-I-IFN",
                  "k": "IL-17/23", "l": "IL-17/23"}

AUDIT = {"CreatedBy": "v2-step10", "CreatedAt": "2026-06-20T00:00:00Z",
         "ModifiedBy": "v2-step10", "ModifiedAt": "2026-06-20T00:00:00Z",
         "ModifiedByModel": "claude-opus-4-8"}

NEW_LETTERS = [m[0] for m in NEW_MEMBERS]


import re

PATHWAY_CODE = {"type-I-IFN": 1, "B-cell/autoantibody": 2, "T-cell-costim": 3, "IL-17/23": 4}


def letter_token_re(letter):
    """Match a SINGLE member letter as a path token: cm-a, ev-a-1, -a, a- (not 'reyes')."""
    return re.compile(rf"(?<![A-Za-z]){letter}(?=-|$)|(?<=-){letter}(?![A-Za-z0-9])")


def substitute(value, tmpl_letter, new_letter, tmpl_ind, new_ind):
    """Rewrite a string id/ref: full individual stem first, then the bare letter token."""
    if not isinstance(value, str):
        return value
    s = value
    # 1) full individual id (and its stem without the 'ind-' prefix, used in Slug/paths)
    s = s.replace(tmpl_ind, new_ind)
    s = s.replace(tmpl_ind[len("ind-"):], new_ind[len("ind-"):])  # reyes-ana style stems
    # 2) bare member-letter token in scoped ids (cm-a, pred-a, ev-a-1, var-a-irf5, assay-a-rna…)
    s = letter_token_re(tmpl_letter).sub(new_letter, s)
    return s


def clone_member(rb, new_letter, tmpl_letter, new_ind, tmpl_ind, overrides):
    sub = lambda v: substitute(v, tmpl_letter, new_letter, tmpl_ind, new_ind)
    for table, meta in MEMBER_TABLES.items():
        data = rb[table]["data"]
        idf = meta["id"]
        # template rows = rows belonging to the template member. Identify by the
        # template individual id (via its FK) or, for the Individuals table, the PK.
        def belongs(r):
            if table == "Individuals":
                return r.get(idf) == tmpl_ind
            # rows tied directly to the individual:
            if r.get("Individual") == tmpl_ind:
                return True
            # rows tied via mechanism/prediction whose id carries the template letter:
            for fk in meta.get("fks", {}):
                fv = r.get(fk)
                if isinstance(fv, str) and fv and letter_token_re(tmpl_letter).search(fv) \
                        and fv.startswith(("cm-", "pred-", "var-", "assay-")):
                    return True
            return False
        tmpl_rows = [r for r in data if belongs(r)]
        added = []
        for r in tmpl_rows:
            nr = copy.deepcopy(r)
            for k, v in list(nr.items()):
                nr[k] = sub(v)
            nr.update(AUDIT)
            data.append(nr)
            added.append(nr)
        # per-table overrides
        ov = overrides.get(table)
        if ov:
            for nr in added:
                nr.update(ov)


# Members whose transport-fail must be preserved: their cloned replications carry
# the TEMPLATE's ancestry, which becomes "cross-ancestry" relative to the new
# member's (different) ancestry — flipping a transport-FAIL into a pass. Rewrite
# every replication's ancestry to the member's own ancestry so cross-count stays 0.
ANCESTRY_LOCK = {"ind-k-nair": "South Asian"}


def finalize_member(rb, new_letter, new_ind):
    # Individual Name/Slug
    ancestry = None
    for r in rb["Individuals"]["data"]:
        if r["IndividualId"] == new_ind:
            r["Name"] = f'{r["GivenName"]} {r["FamilyName"]}'
            r["Slug"] = new_ind
            ancestry = r.get("AncestryLabel")
    # transport-fail lock: pin this member's replications to its own ancestry
    if new_ind in ANCESTRY_LOCK:
        mech_ids = {r["CausalMechanismId"] for r in rb["CausalMechanisms"]["data"] if r.get("Individual") == new_ind}
        for r in rb["CohortReplications"]["data"]:
            if r.get("CausalMechanism") in mech_ids:
                r["ReplicationAncestryLabel"] = ANCESTRY_LOCK[new_ind]
    # mechanism TargetPathway + code
    pw = TARGET_PATHWAY[new_letter]
    for r in rb["CausalMechanisms"]["data"]:
        if r.get("Individual") == new_ind:
            r["TargetPathway"] = pw
            r["TargetPathwayCode"] = PATHWAY_CODE.get(pw, 0)
    # serology: replace cloned panels with the designed series for this member
    data = rb["SerologyObservations"]["data"]
    data[:] = [r for r in data if r.get("Individual") != new_ind]
    for (seq, at, dsdna, c3, c4, prot, egfr, sed) in SEROLOGY[new_letter]:
        sid = f"sero-{new_ind}-{seq}"
        prior = f"sero-{new_ind}-{seq-1}" if seq > 1 else None
        data.append({
            "SerologyObservationId": sid, "Individual": new_ind, "ObservedAt": at + "T00:00:00Z",
            "SequenceIndex": seq, "AntiDsDnaIU": dsdna, "ComplementC3": c3, "ComplementC4": c4,
            "ProteinuriaGPerDay": prot, "EgfrMlMin": egfr, "HasActiveUrinarySediment": sed,
            "PriorObservation": prior, "Name": sid, "RelativePath": f"/admin/serology/{sid}", **AUDIT,
        })


def purge_prior(rb):
    """Idempotency: drop rows from a prior run of THIS step (v2-step10) AND the
    Step-3 progression-only H/I individuals + their serology (Step 10 owns them now
    as full members). Leave Step-3's state-machine occupancy alone — init-db rebuilds
    it; the rulebook occupancy rows are regenerated by Step 3's builder if re-run."""
    new_inds = {m[2] for m in NEW_MEMBERS}
    for table in MEMBER_TABLES:
        rb[table]["data"][:] = [
            r for r in rb[table]["data"]
            if r.get("CreatedBy") != "v2-step10"
            and not (table in ("Individuals", "SerologyObservations")
                     and (r.get("IndividualId") in new_inds or r.get("Individual") in new_inds))
        ]


SM = "lupus-nephritis-progression"

# diagnosis-lifecycle path derivation (mirrors the oracle's SPINE/DIVERT_AFTER).
DL = "diagnosis-lifecycle"
SPINE = ["Intake", "EvidenceAssessed", "MechanismConfirmed", "CalibrationChecked", "TransportChecked", "Actionable"]
DIVERT_AFTER = {"cryptic-relatedness": "MechanismConfirmed", "calibration": "CalibrationChecked",
                "ancestry-transport": "TransportChecked"}
# new member pred -> (deciding-gate-key, actionable)
LIFECYCLE = {
    "pred-h": ("all-pass", True), "pred-i": ("cryptic-relatedness", False),
    "pred-j": ("calibration", False), "pred-k": ("ancestry-transport", False),
    "pred-l": ("transport-passes", True),
}


def lifecycle_path(gate, actionable):
    if actionable:
        return list(SPINE)
    after = DIVERT_AFTER[gate]
    return SPINE[:SPINE.index(after) + 1] + ["NotActionable"]


def build_diagnosis_lifecycle(rb, pred):
    """Generate diagnosis-lifecycle occupancy + transition rows for a new prediction,
    pathed from its deciding gate (mirrors the oracle's pathFor)."""
    ssi = rb["SubjectStateInstances"]["data"]
    st = rb["StateTransitions"]["data"]
    ssi[:] = [r for r in ssi if not (r.get("StateMachine") == DL and r.get("SubjectId") == pred)]
    st[:] = [r for r in st if not (r.get("StateMachine") == DL and r.get("SubjectId") == pred)]
    gate, actionable = LIFECYCLE[pred]
    path = lifecycle_path(gate, actionable)
    prior_id = None
    base_day = 1
    for idx, s in enumerate(path, start=1):
        is_cur = idx == len(path)
        entered = f"2026-06-{base_day+idx:02d}T09:00:00Z"
        exited = None if is_cur else f"2026-06-{base_day+idx+1:02d}T09:00:00Z"
        occ = f"ssi-{pred}-{s.lower()}-{idx}"
        trans = f"st-{pred}-{s.lower()}-{idx}"
        st.append({"StateTransitionId": trans, "Name": trans,
                   "RelativePath": f"/admin/state-machine/transitions/{trans}", "StateMachine": DL,
                   "SubjectTableName": "IndividualPredictions", "SubjectId": pred,
                   "FromStateKey": (path[idx-2] if idx >= 2 else None), "ToStateKey": s,
                   "TransitionAt": entered, "TriggeredByRole": "system",
                   "Reason": f"Lifecycle transition to {s}", **AUDIT})
        ssi.append({"SubjectStateInstanceId": occ, "Name": occ,
                    "RelativePath": f"/admin/state-machine/instances/{occ}", "StateMachine": DL,
                    "SubjectTableName": "IndividualPredictions", "SubjectId": pred, "StateKey": s,
                    "EnteredAt": entered, "ExitedAt": exited, "SequenceIndex": idx,
                    "PriorInstance": prior_id, "EnteredViaTransition": trans, "DwellDays": 1, **AUDIT})
        prior_id = occ


def implied_state(dt, ct, prot, sed):
    if prot >= 3.0 or sed:
        return "BiopsyIndicated"
    if prot >= 1.0:
        return "RenalFlareRisk"
    if prot >= 0.5:
        return "EarlyNephritis"
    if dt == "Rising" and ct == "Falling":
        return "SerologicActive"
    return "PresymptomaticAutoimmunity"


def build_occupancy(rb, new_ind):
    """Generate the lupus-nephritis-progression occupancy walk + transition log for
    a new member, derived from its serology series (mirrors Step 3's logic). New
    members own these rows (CreatedBy v2-step10), so purge_prior clears prior runs."""
    ssi = rb["SubjectStateInstances"]["data"]
    st = rb["StateTransitions"]["data"]
    # clear any prior rows for this subject in this machine
    ssi[:] = [r for r in ssi if not (r.get("StateMachine") == SM and r.get("SubjectId") == new_ind)]
    st[:] = [r for r in st if not (r.get("StateMachine") == SM and r.get("SubjectId") == new_ind)]
    panels = sorted([r for r in rb["SerologyObservations"]["data"] if r.get("Individual") == new_ind],
                    key=lambda r: r["SequenceIndex"])
    path, pdd, pc3, pc4 = [], None, None, None
    for p in panels:
        dsdna, c3, c4, prot, sed = (p["AntiDsDnaIU"], p["ComplementC3"], p["ComplementC4"],
                                    p["ProteinuriaGPerDay"], p["HasActiveUrinarySediment"])
        dt = "Stable" if pdd is None else ("Rising" if dsdna > pdd * 1.25 else ("Falling" if dsdna < pdd * 0.8 else "Stable"))
        ct = "Stable" if pc3 is None else ("Falling" if (c3 + c4) < (pc3 + pc4) * 0.85 else ("Rising" if (c3 + c4) > (pc3 + pc4) * 1.15 else "Stable"))
        s = implied_state(dt, ct, prot, sed)
        at = str(p["ObservedAt"])[:10]
        if not path or path[-1][0] != s:
            path.append((s, at))
        pdd, pc3, pc4 = dsdna, c3, c4
    prior_id = None
    for idx, (s, at) in enumerate(path, start=1):
        is_cur = idx == len(path)
        entered = at + "T00:00:00Z"
        exited = None if is_cur else path[idx][1] + "T00:00:00Z"
        occ = f"ssi-{new_ind}-{s.lower()}-{idx}"
        trans = f"st-{new_ind}-{s.lower()}-{idx}"
        st.append({"StateTransitionId": trans, "Name": trans,
                   "RelativePath": f"/admin/state-machine/transitions/{trans}", "StateMachine": SM,
                   "SubjectTableName": "Individuals", "SubjectId": new_ind,
                   "FromStateKey": (path[idx-2][0] if idx >= 2 else None), "ToStateKey": s,
                   "TransitionAt": entered, "TriggeredByRole": "system",
                   "Reason": f"Serology-derived transition to {s}", **AUDIT})
        ssi.append({"SubjectStateInstanceId": occ, "Name": occ,
                    "RelativePath": f"/admin/state-machine/instances/{occ}", "StateMachine": SM,
                    "SubjectTableName": "Individuals", "SubjectId": new_ind, "StateKey": s,
                    "EnteredAt": entered, "ExitedAt": exited, "SequenceIndex": idx,
                    "PriorInstance": prior_id, "EnteredViaTransition": trans,
                    "DwellDays": 120 if is_cur else 90, **AUDIT})
        prior_id = occ


def main():
    with open(RB) as f:
        rb = json.load(f)
    purge_prior(rb)
    # also clear any prior step10-owned occupancy/log rows
    for t in ("SubjectStateInstances", "StateTransitions"):
        rb[t]["data"][:] = [r for r in rb[t]["data"] if r.get("CreatedBy") != "v2-step10"]
    for new_letter, tmpl_letter, new_ind, tmpl_ind, overrides in NEW_MEMBERS:
        clone_member(rb, new_letter, tmpl_letter, new_ind, tmpl_ind, overrides)
        finalize_member(rb, new_letter, new_ind)
        build_occupancy(rb, new_ind)               # lupus-nephritis-progression walk
        build_diagnosis_lifecycle(rb, f"pred-{new_letter}")  # diagnosis-lifecycle walk
    with open(RB, "w") as f:
        json.dump(rb, f, indent=2)
    n_ind = len(rb["Individuals"]["data"])
    n_pred = len(rb["IndividualPredictions"]["data"])
    print(f"Individuals: {n_ind} | IndividualPredictions: {n_pred} | new members: {[m[2] for m in NEW_MEMBERS]}")


if __name__ == "__main__":
    main()
