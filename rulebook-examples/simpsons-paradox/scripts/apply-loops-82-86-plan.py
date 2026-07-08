#!/usr/bin/env python3
"""Register planned loops 82–86 in the Loops table (identity wave continuation)."""
from __future__ import annotations

import json
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"

PLANNED = [
    {
        "LoopId": "loop-82",
        "Title": "Identity×domain cells — supersede refuted H-severity-vs-age-latent",
        "Status": "planned",
        "NewConcept": (
            "IdentityDomainCells table (one row per ConfounderIdentity × Studies.Domain with n≥2); "
            "archive H-severity-vs-age-latent to corpus-pattern-superseded; register "
            "H-severity-medicine-manifest (severity identity manifest flip ≥45% within medicine) and "
            "H-severity-epi-latent-only (severity in epidemiology: 0 manifest flips); "
            "refresh_identity_domain_cells() customize SQL; ModelSummary domain-cell rollups."
        ),
        "DomainQuestion": (
            "Loop-81 refuted the global severity-vs-age latent comparison (0.917 > 0.813). "
            "Does severity manifest in medicine (53% flip) but stay latent in epidemiology/legal — "
            "requiring identity×domain cells rather than identity-only clusters?"
        ),
        "MockDataNote": "Pending — ad-hoc SQL at N=238: severity×medicine 53.3% flip; severity×epidemiology 0% flip / 100% latent.",
        "NextSuggestion": "loop-83: ontology refinement — drain id-mechanism-other catch-all",
        "TraditionId": "tradition-dag",
    },
    {
        "LoopId": "loop-83",
        "Title": "Ontology refinement — drain id-mechanism-other into accountable sub-identities",
        "Status": "planned",
        "NewConcept": (
            "Split id-mechanism-other (18 studies) into new ConfounderIdentities: id-smoking-behavior, "
            "id-trial-timing-design, id-method-specification, id-exposure-mix; remap StratumVariableIdentityMaps; "
            "fix mis-maps (cancer_stage→severity, passenger_sex→demographic-group); "
            "DiscoveryHypothesis H-mechanism-other-drain (catch-all ≤5 studies post-split)."
        ),
        "DomainQuestion": (
            "Does the 18-study catch-all hide splittable sub-mechanisms with divergent geometry "
            "(flip-prone trial-timing vs latent-only exposure-mix) — and can the ontology absorb them without bloating past ~20 identities?"
        ),
        "MockDataNote": "Pending — mechanism-other currently 18 studies, 27.8% manifest flip, 75pp cross-domain flip spread.",
        "NextSuggestion": "loop-84: pure-latent identity theorem wave (selection + collider + geographic)",
        "TraditionId": "tradition-dag",
    },
    {
        "LoopId": "loop-84",
        "Title": "Pure-latent identity theorem wave — selection, collider, geographic composition",
        "Status": "planned",
        "NewConcept": (
            "Register H-selection-frailty-zero-manifest, H-collider-identity-low-manifest (≤15%), "
            "H-geographic-stable-type-d (stable Type-D fraction ≥25%); MechanismClassSummary rollups or "
            "extend IdentityClusterSummaries; candidate conclusion conc-34 latent-predominant mechanism family."
        ),
        "DomainQuestion": (
            "Do selection-frailty (0% manifest, 100% latent Type-D), collider-proxy (9.1% manifest), "
            "and geographic-composition (29.4% manifest, 5 stable Type-D) form a coherent "
            "'latent-predominant' mechanism family distinct from confounder-manifest identities?"
        ),
        "MockDataNote": "Pending — ad-hoc: selection-frailty 0/5 manifest; collider 1/11 manifest; geographic 5 stable Type-D.",
        "NextSuggestion": "loop-85: identity-conditioned unexplained-flip sink law",
        "TraditionId": "tradition-dag",
    },
    {
        "LoopId": "loop-85",
        "Title": "Identity-conditioned explained theorem — contested-org and collider as sole unexplained sinks",
        "Status": "planned",
        "NewConcept": (
            "DiscoveryHypothesis H-unexplained-flips-only-nonconfounders (all unexplained sign-flips have "
            "ConfounderIdentity ∈ {id-contested-org-choice, id-collider-proxy} OR CausalRole=contested); "
            "InvariantCheck inv-unexplained-identity-sink; promote conc-35 (Category=theorem): "
            "explained↔confounder holds for all confounder identities; violations confined to contested-org + collider."
        ),
        "DomainQuestion": (
            "Loop-74 biconditional holds for every confounder identity with zero unexplained flips. "
            "Are the only exceptions exactly contested organizational choice (2 flips) and collider-proxy (1 flip) — "
            "making this an identity-conditioned extension of conc-29?"
        ),
        "MockDataNote": "Pending — ad-hoc at N=238: 0 unexplained flips outside contested-org (2) and collider (1).",
        "NextSuggestion": "loop-86: identity×domain interaction matrix + methods-export prep",
        "TraditionId": "tradition-dag",
    },
    {
        "LoopId": "loop-86",
        "Title": "Identity×domain interaction matrix — education vs economics institutional split + export prep",
        "Status": "planned",
        "NewConcept": (
            "Populate IdentityDomainCells for all (identity, domain) pairs; register H-education-institutional-flip "
            "(education × id-institutional-unit manifest ≥45%) and H-economics-institutional-latent "
            "(economics × id-institutional-unit latent-only ≥50%); explorer UI slice 'geometry by confounding archetype'; "
            "InstrumentSpec + Rulespeak identity chapter; unblock loop-87 methods-paper export."
        ),
        "DomainQuestion": (
            "Same identity, different domain geometry: institutional-unit is 52.6% manifest in education but "
            "22.2% manifest / 66.7% latent in economics. Can IdentityDomainCells make this modulation first-class — "
            "and is the instrument ready for loop-87 export?"
        ),
        "MockDataNote": "Pending — ad-hoc: institutional×education 52.6% flip; institutional×economics 22.2% flip / 66.7% latent.",
        "NextSuggestion": "loop-87: methods-paper export on pruned instrument with full identity layer",
        "TraditionId": "tradition-dag",
    },
]


import re


def compact_data_rows(rb: dict) -> str:
    """Compact leaf data arrays to one object per line (with valid JSON commas)."""
    pretty = json.dumps(rb, indent=2, ensure_ascii=False)
    lines = pretty.splitlines()
    out: list[str] = []
    in_data = False
    depth = 0
    buf: list[str] = []

    for line in lines:
        if re.match(r'^\s+"data": \[\s*$', line):
            in_data = True
            depth = 0
            buf = []
            out.append(line)
            continue
        if in_data:
            stripped = line.strip()
            if stripped == "{":
                buf = [stripped]
                depth = 1
            elif buf:
                buf.append(stripped)
                depth += stripped.count("{") - stripped.count("}")
                if depth <= 0 and buf:
                    row = " ".join(buf).rstrip(",")
                    out.append("      " + row + ",")
                    buf = []
            elif stripped.startswith("{"):
                row = stripped.rstrip(",")
                out.append("      " + row + ",")
            elif re.match(r"^\s+\],?\s*$", line):
                in_data = False
                if out and out[-1].endswith(","):
                    out[-1] = out[-1][:-1]
                out.append(line)
            continue
        out.append(line)

    text = "\n".join(out) + "\n"
    json.loads(text)
    return text


def main() -> None:
    rb = json.loads(RB_PATH.read_text())
    loops = rb["Loops"]["data"]

    # Insert after loop-81, before loop-87
    anchor = next(i for i, l in enumerate(loops) if l["LoopId"] == "loop-81")
    insert_at = anchor + 1

    existing = {l["LoopId"] for l in loops}
    to_insert = [p for p in PLANNED if p["LoopId"] not in existing]
    if to_insert:
        loops[insert_at:insert_at] = to_insert
    else:
        for planned in PLANNED:
            for i, row in enumerate(loops):
                if row["LoopId"] == planned["LoopId"]:
                    loops[i] = planned
                    break

    for i, row in enumerate(loops):
        if row["LoopId"] == "loop-87":
            row["MockDataNote"] = (
                "Pending — identity wave loops 80–86 (ontology, witness, domain cells, ontology drain, "
                "latent-family theorems, unexplained-sink law, interaction matrix) precede export."
            )
            row["NextSuggestion"] = "Clinical-trials domain encode or peer-review draft after loop-86"
            break

    RB_PATH.write_text(compact_data_rows(rb), encoding="utf-8")
    print(f"Registered loops 82–86 in {RB_PATH}")


if __name__ == "__main__":
    main()
