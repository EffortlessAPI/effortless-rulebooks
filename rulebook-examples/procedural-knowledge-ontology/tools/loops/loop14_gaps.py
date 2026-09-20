"""Loop 14: the ontology-engineering practices the governance theme left without a witness.

Part IV of the PKM series says ontology work starts from real use cases, keeps domain experts
involved throughout, is refined through pilots, and implements by mapping real data into RDF.
Theme E modeled the governed models and their lifecycle controls but had nothing that could
show any of those four practices being skipped.
"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from rulebook_edit import agg, calc, raw, rel  # noqa: E402

EXT = "https://effortlessapi.github.io/effortless-rulebooks/ns/pko-extension#"

LOOP = {
    "WitnessLoopId": "loop-14",
    "LoopNumber": 14,
    "Title": "Loop 14: use cases, experts, pilots and real data behind each governed model",
    "Premise": "Loops 6 to 13 left a handful of article claims uncovered. Four are practices the Linked Open Terms "
               "methodology prescribes and theme E's governed models could not witness: starting from a real use case, "
               "domain experts in every activity, refinement through pilots, and implementation over real data.",
}


def _key(name):
    return raw(name, "string", "Stored identifier.", nullable=False)


TABLES = [
    ("ModelPilots", "Pilot implementations a governed model was tried out in before or while it was adopted.", "governance", [
        _key("ModelPilotId"),
        calc("Name", "string", "Display name.", '={{GovernedModel}} & " pilot: " & {{Site}}'),
        rel("GovernedModel", "GovernedModels", "Model piloted."),
        raw("Site", "string", "Where the pilot ran."),
        raw("StartedAt", "datetime", "When it started."),
        raw("Finding", "string", "What the pilot changed in the model."),
        raw("SemanticTypeIri", "string", "Semantic type IRI."),
    ]),
    ("ModelActivityExperts", "Domain experts taking part in one LOT activity of a governed model.", "governance", [
        _key("ModelActivityExpertId"),
        calc("Name", "string", "Display name.", '={{GovernedModel}} & " " & {{LotActivity}} & " " & {{Expert}}'),
        rel("GovernedModel", "GovernedModels", "Model."),
        raw("LotActivity", "string", "RequirementsSpecification, Implementation, Publication or Maintenance."),
        rel("Expert", "Agents", "Domain expert taking part."),
        raw("SemanticTypeIri", "string", "Semantic type IRI."),
    ]),
    ("ModelDataMappingRuns", "Runs that map instance data into RDF for a governed model.", "governance", [
        _key("ModelDataMappingRunId"),
        calc("Name", "string", "Display name.", '={{GovernedModel}} & " mapping " & {{DataOrigin}}'),
        rel("GovernedModel", "GovernedModels", "Model implemented."),
        raw("DataOrigin", "string", "Real (the organization's own records) or Synthetic."),
        raw("TriplesProduced", "integer", "Triples the run produced."),
        raw("RanAt", "datetime", "When."),
        raw("SemanticTypeIri", "string", "Semantic type IRI."),
    ]),
]

FIELDS = {
    "GovernedModels": [
        raw("OriginatingUseCase", "string", "The concrete use case the model was started from."),
        calc("IsWithoutOriginatingUseCase", "boolean",
             "TRUE when a procedure family or ontology was modeled with no concrete use case behind it.",
             '=AND({{ModelKind}} <> "Vocabulary", {{OriginatingUseCase}} = "")'),
        agg("PilotCount", "integer", "Pilot implementations of the model.", "=COUNTIFS(ModelPilots!{{GovernedModel}}, {{GovernedModelId}})"),
        calc("IsAdoptedWithoutPilot", "boolean",
             "TRUE when a procedure family or ontology was adopted without ever being refined through a pilot.",
             '=AND({{ModelKind}} <> "Vocabulary", {{PilotCount}} = 0)'),
        agg("RequirementsExpertCount", "integer", "Experts in requirements specification.",
            '=COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "RequirementsSpecification")'),
        agg("ImplementationExpertCount", "integer", "Experts in implementation.",
            '=COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "Implementation")'),
        agg("PublicationExpertCount", "integer", "Experts in publication.",
            '=COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "Publication")'),
        agg("MaintenanceExpertCount", "integer", "Experts in maintenance.",
            '=COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "Maintenance")'),
        calc("ExpertsNotInvolvedThroughout", "boolean",
             "TRUE when some LOT activity of a procedure family or ontology ran with no domain expert taking part.",
             '=AND({{ModelKind}} <> "Vocabulary", OR({{RequirementsExpertCount}} = 0, {{ImplementationExpertCount}} = 0, {{PublicationExpertCount}} = 0, {{MaintenanceExpertCount}} = 0))'),
        agg("RealDataMappingRunCount", "integer", "Runs that mapped the organization's real records into RDF.",
            '=COUNTIFS(ModelDataMappingRuns!{{GovernedModel}}, {{GovernedModelId}}, ModelDataMappingRuns!{{DataOrigin}}, "Real")'),
        calc("IsImplementedWithoutRealData", "boolean",
             "TRUE when a procedure family or ontology was never implemented by mapping the organization's real data into RDF.",
             '=AND({{ModelKind}} <> "Vocabulary", {{RealDataMappingRunCount}} = 0)'),
    ],
}

KE, AUTH = "knowledge-engineer", "ontology-authority"
QUESTIONS = [
    ("q14-authority-use-case", AUTH, "Which models were built without a concrete use case behind them?",
     "A model with no use case has no way to know when it is finished or wrong.",
     ["GovernedModels.OriginatingUseCase", "GovernedModels.IsWithoutOriginatingUseCase"]),
    ("q14-authority-pilots", AUTH, "Which models were adopted without ever being tried out in a pilot?",
     "A pilot is where a model meets practice; skipping it moves the first test into production.",
     ["ModelPilots.GovernedModel", "ModelPilots.Site", "ModelPilots.StartedAt", "ModelPilots.Finding",
      "GovernedModels.PilotCount", "GovernedModels.IsAdoptedWithoutPilot"]),
    ("q14-ke-experts-throughout", KE, "Did domain experts take part in every LOT activity of each model, or only at the start?",
     "Experts who help write requirements but never see the implementation cannot catch it drifting from practice.",
     ["ModelActivityExperts.GovernedModel", "ModelActivityExperts.LotActivity", "ModelActivityExperts.Expert",
      "GovernedModels.RequirementsExpertCount", "GovernedModels.ImplementationExpertCount", "GovernedModels.PublicationExpertCount",
      "GovernedModels.MaintenanceExpertCount", "GovernedModels.ExpertsNotInvolvedThroughout"]),
    ("q14-ke-real-data", KE, "Which models were implemented against the organization's real records, and which only on synthetic data or not at all?",
     "An ontology that has only ever held invented data has never been tested against what the organization actually records.",
     ["ModelDataMappingRuns.GovernedModel", "ModelDataMappingRuns.DataOrigin", "ModelDataMappingRuns.TriplesProduced",
      "ModelDataMappingRuns.RanAt", "GovernedModels.RealDataMappingRunCount", "GovernedModels.IsImplementedWithoutRealData"]),
]


def _t(d):
    return f"{d}T09:00:00-05:00"


ROWS = {
    "KnowledgeMethods": [
        {"KnowledgeMethodId": "ProcessMapping", "Label": "Process mapping", "MethodFamily": "Analysis",
         "Summary": "Draws a process's steps, hand-offs and decisions as participants describe them, before any analysis of waste.",
         "OriginReference": "Business process analysis practice", "SemanticTypeIri": f"{EXT}KnowledgeMethod"},
    ],
    "MethodApplications": [
        {"MethodApplicationId": "ma14-loto-process-map", "KnowledgeMethod": "ProcessMapping",
         "AppliedTo": "Lockout/tagout drawn step by step with technicians and the safety officer in workshop el7-loto-workshop, before value stream analysis.",
         "AppliedAt": _t("2026-04-15"), "AppliedByAgent": "sam-adeyemi", "SemanticTypeIri": f"{EXT}MethodApplication"},
    ],
    "GovernedModels": [
        {"GovernedModelId": "gm-pko-rulebook", "OriginatingUseCase": "Auditors could not see whether quarter-end controls and policy notifications ran as documented."},
        {"GovernedModelId": "gm-lockout-tagout", "OriginatingUseCase": "A near miss on conveyor 3 when stored pneumatic energy was released during maintenance."},
        {"GovernedModelId": "gm-quarter-end-close", "OriginatingUseCase": "Late postings after cutoff were found in the Q4 2024 audit."},
        {"GovernedModelId": "gm-workforce-policy", "OriginatingUseCase": "An SMS policy notice reached employees who had not consented."},
    ],
    "ModelPilots": [
        {"ModelPilotId": "pilot-pko-register", "GovernedModel": "gm-pko-rulebook", "Site": "Finance close team", "StartedAt": _t("2026-02-01"),
         "Finding": "Split witnesses by role after controllers could not find their questions.", "SemanticTypeIri": f"{EXT}ModelPilot"},
        {"ModelPilotId": "pilot-loto-north", "GovernedModel": "gm-lockout-tagout", "Site": "North assembly hall", "StartedAt": _t("2026-05-25"),
         "Finding": "Capped zero-energy verification at two attempts.", "SemanticTypeIri": f"{EXT}ModelPilot"},
        {"ModelPilotId": "pilot-close-q1", "GovernedModel": "gm-quarter-end-close", "Site": "Q1 2025 close", "StartedAt": _t("2025-03-25"),
         "Finding": "Added the feed timestamp check.", "SemanticTypeIri": f"{EXT}ModelPilot"},
    ],
    "ModelActivityExperts": [
        *[{"ModelActivityExpertId": f"mae-loto-{a.lower()}", "GovernedModel": "gm-lockout-tagout", "LotActivity": a, "Expert": "tomas-reyes",
           "SemanticTypeIri": f"{EXT}ModelActivityExpert"} for a in ("RequirementsSpecification", "Implementation", "Publication", "Maintenance")],
        *[{"ModelActivityExpertId": f"mae-pko-{a.lower()}", "GovernedModel": "gm-pko-rulebook", "LotActivity": a, "Expert": "devon-okafor",
           "SemanticTypeIri": f"{EXT}ModelActivityExpert"} for a in ("RequirementsSpecification", "Implementation", "Publication", "Maintenance")],
        *[{"ModelActivityExpertId": f"mae-policy-{a.lower()}", "GovernedModel": "gm-workforce-policy", "LotActivity": a, "Expert": "noah-williams",
           "SemanticTypeIri": f"{EXT}ModelActivityExpert"} for a in ("RequirementsSpecification", "Implementation", "Publication", "Maintenance")],
        *[{"ModelActivityExpertId": f"mae-close-{a.lower()}", "GovernedModel": "gm-quarter-end-close", "LotActivity": a, "Expert": "maria-chen",
           "SemanticTypeIri": f"{EXT}ModelActivityExpert"} for a in ("RequirementsSpecification", "Implementation", "Publication")],
        {"ModelActivityExpertId": "mae-deploy-implementation", "GovernedModel": "gm-production-deployment", "LotActivity": "Implementation",
         "Expert": "grace-holloway", "SemanticTypeIri": f"{EXT}ModelActivityExpert"},
    ],
    "ModelDataMappingRuns": [
        {"ModelDataMappingRunId": "map-pko-2026-07", "GovernedModel": "gm-pko-rulebook", "DataOrigin": "Real", "TriplesProduced": 184000,
         "RanAt": _t("2026-07-15"), "SemanticTypeIri": f"{EXT}ModelDataMappingRun"},
        {"ModelDataMappingRunId": "map-loto-2026-07", "GovernedModel": "gm-lockout-tagout", "DataOrigin": "Real", "TriplesProduced": 9200,
         "RanAt": _t("2026-07-12"), "SemanticTypeIri": f"{EXT}ModelDataMappingRun"},
        {"ModelDataMappingRunId": "map-close-2026-07", "GovernedModel": "gm-quarter-end-close", "DataOrigin": "Real", "TriplesProduced": 15400,
         "RanAt": _t("2026-07-03"), "SemanticTypeIri": f"{EXT}ModelDataMappingRun"},
        {"ModelDataMappingRunId": "map-policy-synthetic", "GovernedModel": "gm-workforce-policy", "DataOrigin": "Synthetic", "TriplesProduced": 3100,
         "RanAt": _t("2026-06-10"), "SemanticTypeIri": f"{EXT}ModelDataMappingRun"},
    ],
}
