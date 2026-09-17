"""Evidence for loop-14: practices and illustrations the earlier themes left uncovered."""

EVIDENCE = {
    "pkm4-p23": [("Field", "GovernedModels.IsWithoutOriginatingUseCase",
                  "Fires for the deployment and press-changeover models, which were modeled with no concrete use case behind them; the lockout model started from a conveyor near miss and reads false.")],
    "pkm4-p43": [("Field", "GovernedModels.ExpertsNotInvolvedThroughout",
                  "Fires for models where some LOT activity ran with no domain expert: the close model's maintenance, the deployment model's requirements and publication, the press model entirely. Lockout had its veteran technician in all four activities and reads false.")],
    "pkm4-p45": [("Field", "GovernedModels.IsAdoptedWithoutPilot",
                  "Fires for the policy, deployment and press models, adopted without ever being tried in a pilot; lockout was piloted in the north hall and changed as a result.")],
    "pkm4-p25": [("Field", "GovernedModels.IsImplementedWithoutRealData",
                  "Fires for models never implemented by mapping the organization's real records into RDF: the policy model has only a synthetic mapping run, deployment and press have none. The lockout, close and register models were mapped from real data.")],
    "ont4-i31": [("Field", "Vocabularies.OntologyPrecededVocabularyControl",
                  "The pipeline's point is that controlled vocabulary comes before ontology; this organizing-theme witness fires where an ontology was built before its vocabulary was controlled.")],
    "pkm4-i06": [("Field", "SemanticMappings.ReinventsStandardTerm",
                  "Building on CIDOC CRM or Transmodel illustrates reuse-first engineering; this witness fires on terms minted although a reused standard already defines them, which is reuse-first being broken.")],
    "ont4-i27": [("Field", "SemanticMappings.ReinventsStandardTerm",
                  "A deliberately small custom model leans on reused standards; this witness fires on custom terms that duplicate a reused standard's term, which is how a custom model grows beyond what it needs.")],
}
