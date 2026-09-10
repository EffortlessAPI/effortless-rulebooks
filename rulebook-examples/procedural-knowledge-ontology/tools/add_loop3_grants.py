#!/usr/bin/env python3
"""Grant the two loop-3 asking roles (process-steward, knowledge-authority)
read access to the fields loop-3 added. Without this, the fields exist and
witness cleanly in Postgres, but the role-scoped console for the very roles
that asked the questions still can't see the answer — a real gap for a "full
ride", not a nitpick.

Idempotent: skips any grant that already exists.
"""
from __future__ import annotations

import json
from collections import OrderedDict
from pathlib import Path

RB = Path("effortless-rulebook/procedural-knowledge-ontology-rulebook.json")
EXT = "urn:effortless:pko-extension#"
PRINCIPALS = ["principal-process-steward", "principal-knowledge-authority"]

FIELDS_BY_TABLE = {
    "ProcessMiningRuns": ["ProcessMiningRunId", "Name", "ProcedureVersion", "EventLogSource",
                          "MinedAt", "DiscoveredVariantCount", "ConformingVariantCount",
                          "DeviationDescription", "EvaluationContext", "AsOfInstant",
                          "ConformanceRate", "IsConformant", "HasMajorDriftFromDocumentation",
                          "DaysSinceMined", "IsStaleMiningEvidence", "ProcedureVersionIsLive",
                          "IsDriftOnLiveVersion", "DriftedMiningRunKey", "SemanticTypeIri"],
    "Vocabularies": ["VocabularyId", "Name", "Title", "SchemeUri", "GoverningRole",
                     "TermCount", "OrphanTermCount", "HasOrphanTerms", "SemanticTypeIri"],
    "VocabularyTerms": ["VocabularyTermId", "Name", "Vocabulary", "PrefLabel", "AltLabels",
                        "Definition", "UsageCount", "IsOrphanTerm", "IsWidelyAdoptedTerm",
                        "OrphanTermVocabularyKey", "SemanticTypeIri"],
    "KnowledgeBrokerLinks": ["KnowledgeBrokerLinkId", "Name", "Seeker", "Broker", "Topic",
                            "Frequency", "LastConsultedAt", "EvaluationContext", "AsOfInstant",
                            "DaysSinceConsulted", "IsActiveReliance", "BrokerIsStillEngaged",
                            "IsAtRiskReliance", "ActiveRelianceBrokerKey", "AtRiskBrokerKey",
                            "SemanticTypeIri"],
    "ProcedureVersions": ["MiningRunCount", "DriftedMiningRunCount", "HasUnresolvedMiningDrift"],
    "Requirements": ["ControlledTerm", "UsesControlledVocabulary"],
    "Agents": ["TimesNamedAsBroker", "IsRecognizedBroker", "AtRiskRelianceCount",
               "HasAtRiskKnowledgeReliance"],
}


def main() -> int:
    with RB.open() as fh:
        rb = json.load(fh, object_pairs_hook=OrderedDict)

    grants = rb["FieldGrants"]["data"]
    have = {r["FieldGrantId"] for r in grants}
    added = 0
    for principal in PRINCIPALS:
        short = principal.removeprefix("principal-")
        for table, fields in FIELDS_BY_TABLE.items():
            for field in fields:
                target = f"{table}.{field}"
                gid = f"fg-{short}-{target}"
                if gid in have:
                    continue
                grants.append(OrderedDict([
                    ("FieldGrantId", gid), ("Principal", principal), ("TargetField", target),
                    ("CanRead", True), ("CanWrite", False), ("MaskStrategy", "plain"),
                    ("SemanticTypeIri", f"{EXT}FieldGrant"),
                ]))
                have.add(gid)
                added += 1

    with RB.open("w") as fh:
        json.dump(rb, fh, indent=1, ensure_ascii=False)
        fh.write("\n")
    print(f"added {added} field grants")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
