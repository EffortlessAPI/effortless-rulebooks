# Normalized Schema — Causal Autoimmune Architecture Platform

First-pass structural definition derived from glossary, narrative, and mock scenarios.

## Entity Tables

| Table | Purpose | Parent(s) |
|-------|---------|-----------|
| **Individuals** | Cohort participants with ancestry, age, enrollment metadata | — |
| **AutoimmuneDiseases** | Target disease definitions (e.g. lupus, RA) | — |
| **DiseaseStages** | Stage taxonomy per disease | AutoimmuneDiseases |
| **Tissues** | Anatomical assay sites | — |
| **OmicsModalities** | Assay type registry (RNA-seq, ATAC-seq, …) | — |
| **FederatedDatasets** | Remote privacy-preserving data nodes | — |
| **VariantTypes** | Classification of variant mechanisms | — |
| **GenomicVariants** | Variant calls per individual | Individuals, VariantTypes |
| **OmicsAssays** | Assay instances (individual × modality × tissue) | Individuals, OmicsModalities, Tissues, FederatedDatasets |
| **EnvironmentalExposures** | Longitudinal exposure records | Individuals |
| **Treatments** | Treatment history with response flags | Individuals, AutoimmuneDiseases |
| **ClinicalPhenotypes** | Observed phenotype measurements | Individuals, AutoimmuneDiseases, DiseaseStages, Tissues |
| **CausalMechanisms** | Inferred causal edges (variant/exposure → molecular → phenotype) | Individuals, GenomicVariants, EnvironmentalExposures |
| **EpistaticInteractions** | Multi-variant interaction hypotheses | Individuals, GenomicVariants |
| **CounterfactualTrajectories** | Predicted courses under hypothetical interventions | Individuals, AutoimmuneDiseases |
| **IndividualPredictions** | Onset, severity, response, adverse-effect forecasts | Individuals, AutoimmuneDiseases |
| **InterventionTargets** | Falsifiable therapeutic nodes | CausalMechanisms, AutoimmuneDiseases |

## Key Relationships (DAG)

```
AutoimmuneDiseases ──< DiseaseStages
AutoimmuneDiseases ──< Treatments >── Individuals
AutoimmuneDiseases ──< ClinicalPhenotypes >── Individuals
AutoimmuneDiseases ──< CounterfactualTrajectories >── Individuals
AutoimmuneDiseases ──< IndividualPredictions >── Individuals

Individuals ──< GenomicVariants >── VariantTypes
Individuals ──< OmicsAssays >── OmicsModalities
Individuals ──< OmicsAssays >── Tissues
Individuals ──< OmicsAssays >── FederatedDatasets
Individuals ──< EnvironmentalExposures
Individuals ──< CausalMechanisms
Individuals ──< EpistaticInteractions

CausalMechanisms ──< InterventionTargets >── AutoimmuneDiseases
```

## Inference Layers (field DAG)

**1st-order:** Lookups (IndividualAncestry, DiseaseName, VariantTypeLabel, ModalityName, TissueName, StageLabel)

**2nd-order:** Calculated flags (IsRareVariant, IsCausalCandidate, HasBatchEffectRisk, IsFalsifiable, IsAncestryAbsentFromTraining)

**3rd-order:** Aggregations (CountOfVariants, CountOfCausalMechanisms, CountOfEpistaticInteractions)

**4th-order:** Composite scores (CausalArchitectureScore, CalibratedUncertaintyScore, IsHighConfidencePrediction)

## Mock Scenario Coverage

| Scenario | Tables exercised |
|----------|------------------|
| S1 — Presymptomatic lupus with HLA haplotype | Individuals, GenomicVariants, VariantTypes, ClinicalPhenotypes, IndividualPredictions |
| S2 — RA with gene–environment–microbiome interaction | Individuals, EnvironmentalExposures, OmicsAssays, CausalMechanisms |
| S3 — Treatment-induced remission with feedback | Treatments, ClinicalPhenotypes, CausalMechanisms, CounterfactualTrajectories |
| S4 — Federated African-ancestry holdout prediction | FederatedDatasets, Individuals, IndividualPredictions |
| S5 — Higher-order epistasis between regulatory variants | GenomicVariants, EpistaticInteractions, CausalMechanisms |
| S6 — Falsifiable mechanism → intervention target | CausalMechanisms, InterventionTargets |
