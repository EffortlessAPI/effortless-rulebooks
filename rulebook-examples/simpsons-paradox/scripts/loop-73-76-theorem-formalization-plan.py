#!/usr/bin/env python3
"""Register theorem formalization plan: loops 73–76, conclusions conc-28..31, hypotheses, invariants."""

from __future__ import annotations

import importlib.util
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RB_PATH = ROOT / "effortless-rulebook" / "simpsons-paradox-rulebook.json"
MS = "simpsons-paradox-v1"


def load_bulk():
    spec = importlib.util.spec_from_file_location("bulk", ROOT / "scripts" / "bulk-import-candidates.py")
    bulk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(bulk)
    return bulk


def ms(field: str) -> str:
    return f'LOOKUP("{MS}", ModelSummary[ModelSummaryId], ModelSummary[{field}])'


def append_unique(data: list, row: dict, key: str) -> None:
    if not any(r.get(key) == row.get(key) for r in data):
        data.append(row)


def patch_model_summary(rb: dict) -> None:
    schema = rb["ModelSummary"]["schema"]
    names = {f["name"] for f in schema}
    additions = [
        {
            "name": "MaxStudySweepCorrectedGapRange",
            "datatype": "number",
            "type": "aggregation",
            "nullable": True,
            "Description": "MAX(SweepStudySummary.SweepCorrectedGapRange) — must stay below 0.0001 for CorrectedGap invariance theorem.",
            "formula": "=MAX(SweepStudySummary!{{SweepCorrectedGapRange}})",
        },
        {
            "name": "CorrectedGapInvariantFailCount",
            "datatype": "integer",
            "type": "aggregation",
            "nullable": True,
            "Description": "Count of SweepStudySummary rows where SweepCorrectedGapRange >= 0.0001.",
            "formula": "=COUNTIFS(SweepStudySummary!{{SweepCorrectedGapRange}}, \">=0.0001\")",
        },
        {
            "name": "FalsePositiveExplainedCount",
            "datatype": "integer",
            "type": "aggregation",
            "nullable": True,
            "Description": "IsParadoxExplained=TRUE rows that are not confounder manifest sign-flips — must be 0 for explained↔confounder theorem.",
            "formula": (
                "=COUNTIFS(TreatmentRankings!{{IsParadoxExplained}}, TRUE(), "
                "TreatmentRankings!{{IsSignFlip}}, FALSE()) + "
                "COUNTIFS(TreatmentRankings!{{IsParadoxExplained}}, TRUE(), "
                "TreatmentRankings!{{IsSignFlip}}, TRUE(), "
                "TreatmentRankings!{{StratumCausalRole}}, \"<>confounder\")"
            ),
        },
        {
            "name": "UnexplainedConfounderSignFlipCount",
            "datatype": "integer",
            "type": "aggregation",
            "nullable": True,
            "Description": "Confounder sign-flips with IsParadoxExplained=FALSE — must be 0 for explained↔confounder theorem.",
            "formula": (
                "=COUNTIFS(TreatmentRankings!{{IsSignFlip}}, TRUE(), "
                "TreatmentRankings!{{StratumCausalRole}}, \"confounder\", "
                "TreatmentRankings!{{IsParadoxExplained}}, FALSE())"
            ),
        },
        {
            "name": "TheoremCount",
            "datatype": "integer",
            "type": "calculated",
            "nullable": True,
            "Description": "Count of witnessed Conclusions with Category=theorem (portfolio rollup for loop-76).",
            "formula": "=COUNTIFS(Conclusions!{{Category}}, \"theorem\", Conclusions!{{Status}}, \"witnessed\")",
        },
    ]
    for field in additions:
        if field["name"] not in names:
            schema.append(field)


def patch_discovery_formulas(rb: dict) -> None:
    tail_observed = (
        f'IF({{{{HypothesisId}}}} = "H-econ-encoding-selection", CONCAT("EconExpectedAMismatchRate=", {ms("EconomicsExpectedAMismatchRate")}), '
        f'IF({{{{HypothesisId}}}} = "H-domain-profiles-stable", CONCAT("eduLatent=", {ms("EducationLatentFraction")}, " sportsLatent=", {ms("SportsLatentFraction")}, " econFlipRate=", {ms("EconomicsSignFlipRate")}), '
        f'IF({{{{HypothesisId}}}} = "H-corrected-gap-invariant", CONCAT("maxRange=", {ms("MaxStudySweepCorrectedGapRange")}, " fails=", {ms("CorrectedGapInvariantFailCount")}), '
        f'IF({{{{HypothesisId}}}} = "H-explained-bidirectional", CONCAT("explained=", {ms("ExplainedConfounderCount")}, " confFlip=", {ms("ConfounderSignFlipCount")}, " falsePos=", {ms("FalsePositiveExplainedCount")}, " unexplained=", {ms("UnexplainedConfounderSignFlipCount")}), '
        f'IF({{{{HypothesisId}}}} = "H-collider-no-manifest-theorem", CONCAT("collN=", {ms("ColliderSelectionCount")}, " collManifest=", {ms("ColliderSelectionManifestCount")}), '
        f'IF({{{{HypothesisId}}}} = "H-theorem-portfolio", CONCAT("theoremCount=", {ms("TheoremCount")}), ""))))))))))))))))))))'
    )
    tail_confirmed = (
        f'IF({{{{HypothesisId}}}} = "H-econ-encoding-selection", {ms("EconomicsExpectedAMismatchRate")} > 0.5, '
        f'IF({{{{HypothesisId}}}} = "H-domain-profiles-stable", AND({ms("EducationLatentFraction")} > 0.5, {ms("SportsLatentFraction")} > 0.5, {ms("EconomicsSignFlipRate")} < 0.05), '
        f'IF({{{{HypothesisId}}}} = "H-corrected-gap-invariant", AND({ms("MaxStudySweepCorrectedGapRange")} < 0.0001, {ms("CorrectedGapInvariantFailCount")} = 0), '
        f'IF({{{{HypothesisId}}}} = "H-explained-bidirectional", AND({ms("ExplainedConfounderCount")} = {ms("ConfounderSignFlipCount")}, {ms("FalsePositiveExplainedCount")} = 0, {ms("UnexplainedConfounderSignFlipCount")} = 0), '
        f'IF({{{{HypothesisId}}}} = "H-collider-no-manifest-theorem", AND({ms("ColliderSelectionManifestCount")} = 0, {ms("ColliderSelectionCount")} >= 10), '
        f'IF({{{{HypothesisId}}}} = "H-theorem-portfolio", {ms("TheoremCount")} >= 4, FALSE()))))))))))))))))))))'
    )

    for field in rb["DiscoveryFindings"]["schema"]:
        if field["name"] == "ObservedMetric":
            formula = field["formula"]
            marker = 'IF({{HypothesisId}} = "H-ultra-fragile", CONCAT("SweepFragileCount=",'
            idx = formula.find(marker)
            if idx == -1:
                raise RuntimeError("Could not locate H-ultra-fragile branch in ObservedMetric formula")
            field["formula"] = formula[:idx] + tail_observed
        elif field["name"] == "IsConfirmed":
            formula = field["formula"]
            marker = 'IF({{HypothesisId}} = "H-ultra-fragile",'
            idx = formula.find(marker)
            if idx == -1:
                raise RuntimeError("Could not locate H-ultra-fragile branch in IsConfirmed formula")
            field["formula"] = formula[:idx] + tail_confirmed


def patch_invariants(rb: dict) -> None:
    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-corrected-gap-invariant":
            inv["ProtectsConclusion"] = "conc-28-corrected-gap-invariance-theorem"
            inv["NaturalLanguage"] = (
                "CorrectedGap must be constant across all allocation fractions for EVERY study in the sweep. "
                "Build-breaking witness for the CorrectedGap invariance theorem (loop-73)."
            )
        elif inv["InvariantCheckId"] == "inv-explained-sign-flip-confounder":
            inv["ProtectsConclusion"] = "conc-29-explained-confounder-theorem"
            inv["NaturalLanguage"] = (
                "Every IsParadoxExplained=TRUE row must be a confounder manifest sign-flip. "
                "Half of the explained↔confounder biconditional (loop-74)."
            )

    existing = {i["InvariantCheckId"] for i in rb["InvariantChecks"]["data"]}
    if "inv-confounder-signflip-explained" not in existing:
        rb["InvariantChecks"]["data"].append(
            {
                "InvariantCheckId": "inv-confounder-signflip-explained",
                "Name": "inv-confounder-signflip-explained",
                "AlgebraicStatement": "IsSignFlip=TRUE AND StratumCausalRole='confounder' → IsParadoxExplained=TRUE",
                "NaturalLanguage": "Every confounder manifest sign-flip must be marked explained. Other half of the explained↔confounder biconditional (loop-74).",
                "SourceTable": "TreatmentRankings",
                "FilterExpression": "IsSignFlip = TRUE AND StratumCausalRole = 'confounder'",
                "AssertionExpression": "IsParadoxExplained = TRUE",
                "SqlFilter": "is_sign_flip = true AND stratum_causal_role = 'confounder'",
                "SqlAssertion": "is_paradox_explained = true",
                "PassCount": 0,
                "FailCount": 0,
                "Severity": "critical",
                "TraditionId": "tradition-dag",
                "ProtectsConclusion": "conc-29-explained-confounder-theorem",
            }
        )
    if "inv-collider-no-manifest" not in existing:
        rb["InvariantChecks"]["data"].append(
            {
                "InvariantCheckId": "inv-collider-no-manifest",
                "Name": "inv-collider-no-manifest",
                "AlgebraicStatement": "StratumCausalRole ∈ {collider, selection} → IsSignFlip = FALSE",
                "NaturalLanguage": "Collider and selection studies never produce manifest sign-flips at observed allocation — conditional theorem requiring correct causal-role annotation (loop-75).",
                "SourceTable": "TreatmentRankings",
                "FilterExpression": "StratumCausalRole IN ('collider','selection')",
                "AssertionExpression": "IsSignFlip = FALSE",
                "SqlFilter": "stratum_causal_role IN ('collider', 'selection')",
                "SqlAssertion": "(is_sign_flip = false OR is_sign_flip IS NULL)",
                "PassCount": 0,
                "FailCount": 0,
                "Severity": "critical",
                "TraditionId": "tradition-dag",
                "ProtectsConclusion": "conc-30-collider-no-manifest-theorem",
            }
        )


def patch_discovery_rollup(rb: dict) -> None:
    all_hypotheses = [
        "H-latent-d",
        "H-small-effect",
        "H-econ-zero",
        "H-domain-dist",
        "H-causal-manifest",
        "H-causal-latent",
        "H-explained-confounder",
        "H-unexplained-nonconfounder",
        "H-catalog-exact-match",
        "H-catalog-flip-prediction",
        "H-domain-flip-geometry-controlled",
        "H-collider-no-manifest-v2",
        "H-cplus-magnitude",
        "H-ultra-fragile",
        "H-econ-encoding-selection",
        "H-domain-profiles-stable",
        "H-corrected-gap-invariant",
        "H-explained-bidirectional",
        "H-collider-no-manifest-theorem",
    ]
    for inv in rb["InvariantChecks"]["data"]:
        if inv["InvariantCheckId"] == "inv-discovery-all-confirmed":
            inv["SqlFilter"] = "hypothesis_id IN (" + ", ".join(f"'{h}'" for h in all_hypotheses) + ")"
            inv["PassCount"] = len(all_hypotheses)
            inv["NaturalLanguage"] = (
                "All pre-registered corpus and theorem-hardening hypotheses (loops 61–75) must confirm against the live corpus."
            )
            inv["ProtectsConclusion"] = "conc-31-theorem-portfolio-synthesis"


def main() -> None:
    bulk = load_bulk()
    rb = json.loads(RB_PATH.read_text(encoding="utf-8"))

    patch_model_summary(rb)

    # ModelSummary domain latent fractions + economics flip rate for H-domain-profiles-stable
    ms_schema = rb["ModelSummary"]["schema"]
    ms_names = {f["name"] for f in ms_schema}
    domain_metrics = [
        {
            "name": "EducationLatentFraction",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Fraction of education Type-D studies with LatentFlipPotential=TRUE.",
            "formula": (
                "=IF(COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"education\", TreatmentRankings!{{DistortionType}}, \"D\") = 0, \"\", "
                "COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"education\", TreatmentRankings!{{DistortionType}}, \"D\", "
                "TreatmentRankings!{{IsLatentOnlyFlip}}, TRUE()) / "
                "COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"education\", TreatmentRankings!{{DistortionType}}, \"D\"))"
            ),
        },
        {
            "name": "SportsLatentFraction",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Fraction of sports Type-D studies with LatentFlipPotential=TRUE.",
            "formula": (
                "=IF(COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"sports\", TreatmentRankings!{{DistortionType}}, \"D\") = 0, \"\", "
                "COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"sports\", TreatmentRankings!{{DistortionType}}, \"D\", "
                "TreatmentRankings!{{IsLatentOnlyFlip}}, TRUE()) / "
                "COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"sports\", TreatmentRankings!{{DistortionType}}, \"D\"))"
            ),
        },
        {
            "name": "EconomicsSignFlipRate",
            "datatype": "number",
            "type": "calculated",
            "nullable": True,
            "Description": "Manifest sign-flip rate among economics studies.",
            "formula": (
                "=IF(COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"economics\") = 0, \"\", "
                "COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"economics\", TreatmentRankings!{{IsSignFlip}}, TRUE()) / "
                "COUNTIFS(TreatmentRankings!{{StudyDomain}}, \"economics\"))"
            ),
        },
    ]
    for field in domain_metrics:
        if field["name"] not in ms_names:
            ms_schema.append(field)

    new_loops = [
        {
            "LoopId": "loop-73",
            "Title": "Theorem wave 1: CorrectedGap allocation-invariance theorem",
            "Status": "planned",
            "NewConcept": "Promote conc-28 (Category=theorem): CorrectedGap = WeightedStratumGapSum is invariant under allocation reweighting. ModelSummary MaxStudySweepCorrectedGapRange + CorrectedGapInvariantFailCount; DiscoveryHypothesis H-corrected-gap-invariant (consistency-check); re-point inv-corrected-gap-invariant → conc-28; InstrumentSpec theorem entry.",
            "DomainQuestion": "CorrectedGap invariance was witnessed at corpus scale (loops 34/56) but never promoted to theorem. Does inv-corrected-gap-invariant still pass at N>110 with zero failures — justifying Category=theorem alongside SignalPurity?",
            "MockDataNote": "Pending — run after loop-72. Pre-loop: inv-corrected-gap-invariant PASS 104/104; MaxStudySweepCorrectedGapRange≈0. Pass criteria: CorrectedGapInvariantFailCount=0.",
            "NextSuggestion": "loop-74: theorem wave 2 — explained↔confounder biconditional",
            "TraditionId": "tradition-dag",
        },
        {
            "LoopId": "loop-74",
            "Title": "Theorem wave 2: IsParadoxExplained ↔ confounder sign-flip biconditional",
            "Status": "planned",
            "NewConcept": "Promote conc-29 (Category=theorem): IsParadoxExplained=TRUE iff IsSignFlip=TRUE AND StratumCausalRole=confounder. H-explained-bidirectional (consistency-check); inv-confounder-signflip-explained (reverse direction); ModelSummary FalsePositiveExplainedCount + UnexplainedConfounderSignFlipCount; upgrade inv-explained-sign-flip-confounder ProtectsConclusion.",
            "DomainQuestion": "conc-19 and inv-explained-sign-flip-confounder witness the forward direction. Does the reverse direction also hold with zero false positives — making this a true biconditional theorem rather than a one-way law?",
            "MockDataNote": "Pending — run after loop-73. Pre-loop: ExplainedConfounderCount=ConfounderSignFlipCount; ContestedOrMediatorExplainedCount=0. Pass: FalsePositiveExplainedCount=0 AND UnexplainedConfounderSignFlipCount=0.",
            "NextSuggestion": "loop-75: theorem wave 3 — collider no-manifest (conditional, contingent on loop-72)",
            "TraditionId": "tradition-dag",
        },
        {
            "LoopId": "loop-75",
            "Title": "Theorem wave 3: collider/selection no-manifest sign-flip (conditional theorem)",
            "Status": "planned",
            "NewConcept": "Promote conc-30 (Category=theorem): given correct StratumCausalRole annotation, collider/selection → IsSignFlip=FALSE at observed allocation. H-collider-no-manifest-theorem (ColliderSelectionCount>=10, ManifestCount=0); inv-collider-no-manifest; upgrade conc-24 from theorem candidacy to full theorem if loop-72 stress test holds.",
            "DomainQuestion": "At N≥10 collider/selection studies (post loop-71/72 expansion), does zero manifest sign-flips survive — or does a counterexample appear? Falsifier: any collider/selection row with DistortionType A/B.",
            "MockDataNote": "Pending — contingent on loop-72 PASS for H-collider-no-manifest-v2. Pre-loop N=8 collider/selection, collManifest=0. Pass: ColliderSelectionCount>=10 AND ColliderSelectionManifestCount=0.",
            "NextSuggestion": "loop-76: theorem portfolio synthesis + rulespeak export",
            "TraditionId": "tradition-dag",
        },
        {
            "LoopId": "loop-76",
            "Title": "Theorem portfolio synthesis: four theorems + discovery rollup + methods export prep",
            "Status": "planned",
            "NewConcept": "Promote conc-31 synthesis; H-theorem-portfolio (TheoremCount>=4); ModelSummary TheoremCount; extend inv-discovery-all-confirmed to loops 61–75 hypotheses; optional conc-expansion-wave-synthesis; Rulespeak theorem section regen; InstrumentSpec theorem catalog (SignalPurity, CorrectedGap, Explained↔Confounder, Collider-no-manifest).",
            "DomainQuestion": "With four Category=theorem conclusions witnessed and all discovery hypotheses PASS, is the instrument ready for external methods-paper export — and which patterns remain corpus-contingent vs law?",
            "MockDataNote": "Pending — run after loops 73–75. Target: TheoremCount=4 (conc-12/20 SignalPurity + conc-28/29/30). inv-discovery-all-confirmed extended to 19 hypotheses.",
            "NextSuggestion": "loop-77+: clinical-trials expansion wave or external methods-paper draft from InstrumentSpec",
            "TraditionId": "tradition-historical",
        },
    ]

    loop_ids = {r["LoopId"] for r in rb["Loops"]["data"]}
    for row in new_loops:
        if row["LoopId"] not in loop_ids:
            rb["Loops"]["data"].append(row)

    for row in rb["Loops"]["data"]:
        if row["LoopId"] == "loop-72":
            row["NextSuggestion"] = "loop-73: theorem wave 1 — CorrectedGap invariance theorem promotion"
            row["NewConcept"] = (
                row["NewConcept"]
                + " Theorem promotion pipeline loops 73–76 registered: conc-28 CorrectedGap, conc-29 Explained↔Confounder, conc-30 Collider-no-manifest, conc-31 portfolio synthesis."
            )

    new_conclusions = [
        {
            "ConclusionId": "conc-28-corrected-gap-invariance-theorem",
            "Category": "theorem",
            "Status": "planned",
            "Title": "CorrectedGap (= WeightedStratumGapSum) is invariant under allocation reweighting — the conserved quantity",
            "Evidence": "Algebraic: CorrectedGap depends only on stratum rates and study-level weights, not treatment allocation fractions. Empirical: inv-corrected-gap-invariant PASS across full corpus (SweepCorrectedGapRange < 0.0001 per study). Loop-73 promotes from conc-11 instrument finding to Category=theorem.",
            "TargetLoop": "loop-73",
            "TraditionId": "tradition-dag",
        },
        {
            "ConclusionId": "conc-29-explained-confounder-theorem",
            "Category": "theorem",
            "Status": "planned",
            "Title": "IsParadoxExplained ⟺ confounder manifest sign-flip — adjust-resolvable confounding is the unique explanation condition",
            "Evidence": "Forward: inv-explained-sign-flip-confounder (IsParadoxExplained → confounder sign-flip). Reverse: inv-confounder-signflip-explained (confounder sign-flip → IsParadoxExplained). Loop-74 H-explained-bidirectional: FalsePositiveExplainedCount=0 AND UnexplainedConfounderSignFlipCount=0. Upgrades conc-19 from instrument to biconditional theorem.",
            "TargetLoop": "loop-74",
            "TraditionId": "tradition-dag",
        },
        {
            "ConclusionId": "conc-30-collider-no-manifest-theorem",
            "Category": "theorem",
            "Status": "planned",
            "Title": "Given correct causal-role annotation, collider/selection studies produce no manifest sign-flips at observed allocation",
            "Evidence": "Conditional theorem: StratumCausalRole ∈ {collider, selection} → IsSignFlip=FALSE. Loop-75 H-collider-no-manifest-theorem at ColliderSelectionCount≥10. inv-collider-no-manifest build-breaking. Upgrades conc-24 theorem candidacy after loop-72 scale stress test.",
            "TargetLoop": "loop-75",
            "TraditionId": "tradition-dag",
        },
        {
            "ConclusionId": "conc-31-theorem-portfolio-synthesis",
            "Category": "methodology",
            "Status": "planned",
            "Title": "Four witnessed theorems: SignalPurity, CorrectedGap invariance, Explained↔Confounder, Collider-no-manifest — plus corpus patterns that remain non-theorems",
            "Evidence": "Loop-76: TheoremCount≥4; inv-discovery-all-confirmed PASS on loops 61–75 hypotheses. Distinguishes build-breaking theorems from corpus patterns (latent Type-D majority, C+ blind spot, domain profiles, catalog falsification). Methods-export ready.",
            "TargetLoop": "loop-76",
            "TraditionId": "tradition-dag",
        },
        {
            "ConclusionId": "conc-expansion-wave-synthesis",
            "Category": "methodology",
            "Status": "planned",
            "Title": "Expansion waves 1–2 (loops 68–72): adversarial stress tests survived; theorem candidacy patterns held or sharpened",
            "Evidence": "Pending loop-72 LoopReview. Witnesses collider-no-manifest, economics encoding selection, sweep-fragile class, domain profile stability at N>110.",
            "TargetLoop": "loop-72",
            "TraditionId": "tradition-historical",
        },
    ]
    existing_conc = {c["ConclusionId"] for c in rb["Conclusions"]["data"]}
    for row in new_conclusions:
        if row["ConclusionId"] not in existing_conc:
            rb["Conclusions"]["data"].append(row)

    new_hypotheses = [
        {
            "HypothesisId": "H-corrected-gap-invariant",
            "Statement": "CorrectedGap is allocation-invariant for every study: MAX(SweepStudySummary.SweepCorrectedGapRange) < 0.0001 and zero failing studies.",
            "ExpectedOutcome": "MaxStudySweepCorrectedGapRange < 0.0001 AND CorrectedGapInvariantFailCount = 0",
            "RegisteredInLoop": "loop-73",
            "TraditionId": "tradition-dag",
            "EpistemicTier": "consistency-check",
        },
        {
            "HypothesisId": "H-explained-bidirectional",
            "Statement": "IsParadoxExplained ⟺ confounder manifest sign-flip: ExplainedConfounderCount = ConfounderSignFlipCount, no false-positive explained rows, no unexplained confounder sign-flips.",
            "ExpectedOutcome": "ExplainedConfounderCount = ConfounderSignFlipCount AND FalsePositiveExplainedCount = 0 AND UnexplainedConfounderSignFlipCount = 0",
            "RegisteredInLoop": "loop-74",
            "TraditionId": "tradition-dag",
            "EpistemicTier": "consistency-check",
        },
        {
            "HypothesisId": "H-collider-no-manifest-theorem",
            "Statement": "At scale (≥10 collider/selection studies post expansion), zero manifest sign-flips: ColliderSelectionManifestCount = 0.",
            "ExpectedOutcome": "ColliderSelectionManifestCount = 0 AND ColliderSelectionCount >= 10",
            "RegisteredInLoop": "loop-75",
            "TraditionId": "tradition-dag",
            "EpistemicTier": "consistency-check",
        },
        {
            "HypothesisId": "H-theorem-portfolio",
            "Statement": "Four Category=theorem conclusions are witnessed: SignalPurity (conc-12/20) plus three new theorems from loops 73–75.",
            "ExpectedOutcome": "TheoremCount >= 4",
            "RegisteredInLoop": "loop-76",
            "TraditionId": "tradition-historical",
            "EpistemicTier": "consistency-check",
        },
    ]
    for row in new_hypotheses:
        append_unique(rb["DiscoveryHypotheses"]["data"], row, "HypothesisId")

    new_findings = [
        {"FindingId": "find-H-corrected-gap-invariant", "HypothesisId": "H-corrected-gap-invariant", "WitnessedInLoop": "loop-73"},
        {"FindingId": "find-H-explained-bidirectional", "HypothesisId": "H-explained-bidirectional", "WitnessedInLoop": "loop-74"},
        {"FindingId": "find-H-collider-no-manifest-theorem", "HypothesisId": "H-collider-no-manifest-theorem", "WitnessedInLoop": "loop-75"},
        {"FindingId": "find-H-theorem-portfolio", "HypothesisId": "H-theorem-portfolio", "WitnessedInLoop": "loop-76"},
    ]
    for row in new_findings:
        append_unique(rb["DiscoveryFindings"]["data"], row, "FindingId")

    patch_discovery_formulas(rb)
    patch_invariants(rb)
    patch_discovery_rollup(rb)

    bulk.write_rulebook(rb, RB_PATH)
    print(f"Patched {RB_PATH} — loops 73–76, conc-28..31, theorem hardening hypotheses")


if __name__ == "__main__":
    main()
