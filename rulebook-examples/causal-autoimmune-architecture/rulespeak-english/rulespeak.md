# 📘 Causal Autoimmune Architecture Platform — RuleSpeak

_Rulebook for inferring the complete causal architecture of heterogeneous autoimmune disease from multi-omic cohort data, producing falsifiable mechanisms and ancestry-equitable predictions with calibrated uncertainty._

> Declarative business rules rendered from the rulebook. Every statement
> below expresses truth in the business domain — it is neither a procedure
> nor an imperative. The rulebook's formulas are the single source of truth;
> this document is their plain-language reading.

## 1 Business Vocabulary

| Term | Description | Narrative Comment |
|------|-------------|-------------------|
| **Autoimmune Diseas** | Target heterogeneous autoimmune disease definitions tracked across development, aging, tissues, and disease stages. | — |
| Name | Computed as the disease label. | _Display label for the autoimmune disease._ |
| Relative Path | Computed as the literal “/diseases/”, followed by the autoimmune disease ID. | _Canonical path to this AutoimmuneDisease page: /diseases/<slug-or-id>._ |
| Count of Disease Stages | The number of disease stages related to the autoimmune diseas. | _Number of disease stages defined for this autoimmune disease._ |
| Count of Intervention Targets | The number of intervention targets related to the autoimmune diseas. | _Count of validated intervention targets linked to this disease._ |
| **Disease Stage** | Ordered disease stages capturing presymptomatic, active, remission, and treatment-refractory phases along disease progression. | — |
| Name | Computed as the autoimmune disease disease label, followed by the literal “ — ”, followed by the stage label. | _Display label combining disease and stage._ |
| Parent Path | The relative path of the disease stage's autoimmune disease. | _Lookup: AutoimmuneDiseases.RelativePath via AutoimmuneDisease — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/stages/”, followed by the disease stage ID. | _Path to this DiseaseStage page, chained under its AutoimmuneDisease parent._ |
| Autoimmune Disease Disease Label | The disease label of the disease stage's autoimmune disease. | _Lookup of parent disease label._ |
| Is Presymptomatic | True when the stage label is the literal “Presymptomatic”. | _True when stage is presymptomatic diagnosis window._ |
| **Tissue** | Anatomical tissues where omics assays resolve cell-state-specific effects; includes cases of missing tissues. | — |
| Name | Computed as the tissue label. | _Display label for the tissue._ |
| Relative Path | Computed as the literal “/tissues/”, followed by the tissue ID. | _Canonical path to this Tissue page: /tissues/<slug-or-id>._ |
| Count of Omics Assays | The number of omics assays related to the tissue. | _Number of omics assays performed on this tissue._ |
| **Omics Modality** | Registry of omics assay modalities: RNA-seq, ATAC-seq, proteomics, metabolomics, methylomes, chromatin-conformation, immune-receptor repertoires, microbiomes, and long-read genomes. | — |
| Name | Computed as the modality label. | _Display label for the omics modality._ |
| Relative Path | Computed as the literal “/omics-modalities/”, followed by the omics modality ID. | _Canonical path to this OmicsModalitie page: /omics-modalities/<slug-or-id>._ |
| Count of Omics Assays | The number of omics assays related to the omics modality. | _Count of assays using this modality._ |
| **Federated Dataset** | Privacy-preserving federated datasets contributing cohort partitions without centralizing raw genomes. | — |
| Name | Computed as the node label. | _Display label for the federated dataset._ |
| Relative Path | Computed as the literal “/datasets/”, followed by the federated dataset ID. | _Canonical path to this FederatedDataset page: /datasets/<slug-or-id>._ |
| Count of Individuals | The number of individuals related to the federated dataset. | _Individuals enrolled via this federated node._ |
| **Variant Type** | Classification of genomic variant mechanisms: regulatory, coding, repeat expansions, mobile-element insertions, HLA haplotypes, structural variants, de novo mutations, somatic mosaicism, mitochondrial variation. | — |
| Name | Computed as the type label. | _Display label for variant type._ |
| Relative Path | Computed as the literal “/variant-types/”, followed by the variant type ID. | _Canonical path to this VariantType page: /variant-types/<slug-or-id>._ |
| Count of Genomic Variants | The number of genomic variants related to the variant type. | _Variants classified under this type._ |
| **Individual** | Ancestrally diverse cohort participants whose phased genomes, omics profiles, exposures, and clinical phenotypes feed causal architecture inference. | — |
| Name | Computed as the given name, followed by a space, followed by the family name. | _Display name for the individual._ |
| Slug | Computed as the lower-cased family name, followed by a hyphen, followed by the given name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _URL slug for this case, family-given kebab (e.g. reyes-ana). Used in RelativePath._ |
| Relative Path | Computed as the literal “/intake/new-patient/”, followed by the slug. | _Canonical path to this Individual page: /intake/new-patient/<slug-or-id>._ |
| Federated Dataset Node Label | Determined by priority: an empty string if the federated dataset is blank; otherwise the node label of the individual's federated dataset. | _Lookup of federated node label._ |
| Count of Genomic Variants | The number of genomic variants related to the individual. | _Total genomic variants for rare-variant burden assessment._ |
| Count of Causal Mechanisms | The number of causal mechanisms related to the individual. | _Inferred causal mechanisms linked to this individual._ |
| Count of Epistatic Interactions | The number of epistatic interactions related to the individual. | _Higher-order epistasis interactions involving this individual._ |
| Rare Variant Burden Score | Determined by priority: the count of genomic variants divided by the age years if the age years is greater than 0; otherwise 0. | _2nd-order score from rare variant count normalized by age._ |
| Causal Architecture Score | Computed as the count of causal mechanisms times 10 plus the count of epistatic interactions times 5 plus the rare variant burden score. | _3rd-order composite of causal mechanisms and epistasis density._ |
| Is Development Window | True when the age years is at most 25. | _True when age falls in developmental effects window (0-25)._ |
| Is Aging Window | True when the age years is at least 60. | _True when age falls in aging window (60+)._ |
| Count Confirmed Causal Nodes | The number of the individual's causal mechanisms that are causal architecture nodes. | _Count of this individual's confirmed causal-architecture nodes._ |
| Sum Confirmed Causal Confidence | The total causal confidence across the individual's causal mechanisms that are causal architecture nodes. | _Summed confidence of this individual's confirmed causal nodes (derived causal mass)._ |
| Count Cross Ancestry Confirmed Nodes | The number of the individual's causal mechanisms that are ancestry transportables. | _Confirmed nodes that also showed cross-ancestry replication._ |
| Max Severity Score | The largest severity score across the clinical phenotypes related to the individual. | _Highest SeverityScore across this individual's clinical phenotypes (0 if none)._ |
| Count High Severity Phenotypes | The number of the individual's clinical phenotypes that are high severities. | _Count of this individual's high-severity phenotypes (SeverityScore > 7)._ |
| Has High Severity Phenotype | True when the count high severity phenotypes is at least 1. | _True when the individual has at least one high-severity phenotype._ |
| Count Predicted Treatment Responses | The number of the individual's treatments that are treatment response predicted. | _Count of this individual's treatments predicted to respond (effective ∧ mechanism-matched)._ |
| Has Predicted Treatment Response | True when the count predicted treatment responses is at least 1. | _True when the individual has at least one treatment predicted to respond._ |
| Count Serology Panels | The number of serology observations related to the individual. | _Number of serology panels for this individual._ |
| Count Pre Nephritic Signature Panels | The number of the individual's serology observations that are pre nephritic signature panels. | _How many of this individual's panels exhibit the pre-nephritic serology signature (rising anti-dsDNA + falling complement before overt nephritis). The corpus-level roll-up of the per-panel signal; emergent from the raw series._ |
| Is in Pre Nephritic Signature Cluster | True when the count pre nephritic signature panels is at least 1. | _DERIVED cluster membership: TRUE when this individual's own raw serology series ever showed the pre-nephritic signature. The discovery 'this serology signature precedes nephritis' becomes a reproducible, witnessed corpus-level cluster — not a label anyone assigned._ |
| Signature Strength | Determined by priority: 2 if the count pre nephritic signature panels is at least 2; 1 if the count pre nephritic signature panels is at least 1; otherwise 0. | _DERIVED 0/1/2 strength of the pre-nephritic signature: 2 when it appears on >=2 panels (persistent), 1 on exactly one panel, 0 when absent. Drives the emphasis of cluster members on the cohort scatter._ |
| Max Progression State Order | The largest progression state order across the serology observations related to the individual. | _Highest progression-state order reached across this individual's serology panels (worst/current state)._ |
| Latest Sledai Score | The largest sledai score across the serology observations related to the individual. | _Peak SLEDAI activity score across this individual's panels (derived)._ |
| Nephritis Progression State Key | Determined by priority: the literal “BiopsyIndicated” if the max progression state order is at least 5; the literal “RenalFlareRisk” if the max progression state order is at least 4; the literal “EarlyNephritis” if the max progression state order is at least 3; the literal “SerologicActive” if the max progression state order is at least 2; otherwise the literal “PresymptomaticAutoimmunity”. | _DERIVED current disease state for the lupus-nephritis-progression machine: decoded from the highest state order reached. The subject- state column of the state machine; never hand-set._ |
| Activity Tier | Determined by priority: the literal “High / flare” if the latest sledai score is at least 12; the literal “Moderate” if the latest sledai score is at least 6; the literal “Mild” if the latest sledai score is at least 1; otherwise the literal “Quiescent”. | _Derived disease-activity tier from peak SLEDAI score._ |
| Is High Disease Activity | True when the latest sledai score is at least 12. | _TRUE when peak SLEDAI >= 12._ |
| Is Disease Progressing | True when at least one of the following holds: the nephritis progression state key is the literal “EarlyNephritis”; the nephritis progression state key is the literal “RenalFlareRisk”; or the nephritis progression state key is the literal “BiopsyIndicated”. | _TRUE when the disease-state simulator places the patient in an active/worsening renal state (independent of the actionability gate)._ |
| Target Pathway Code | The largest target pathway code across the causal mechanisms related to the individual. | _Resolved pathway code from this individual's causal mechanism (MAXIFS)._ |
| Target Pathway | Determined by priority: the literal “type-I-IFN” if the target pathway code is 1; the literal “B-cell/autoantibody” if the target pathway code is 2; the literal “T-cell-costim” if the target pathway code is 3; the literal “IL-17/23” if the target pathway code is 4; otherwise an empty string. | _Decoded druggable pathway implicated by this individual's mechanism._ |
| Reachable States Ahead | The reachable state count of the individual's current progression state ID. | _How many disease states are still reachable ahead of THIS patient from their current state, via the transition closure (looks up MachineStates.ReachableStateCount for CurrentProgressionStateId). A closure-derived prognostic horizon: Diego at BiopsyIndicated has 0 ahead (terminal); a Presymptomatic patient has 5. NOT derivable from the severity rank alone (the count is non-monotonic in rank because the machine branches into remission)._ |
| **Genomic Variant** | Genomic variant calls per individual spanning regulatory, coding, structural, HLA haplotypes, de novo mutations, somatic mosaicism, and mitochondrial variation. | — |
| Name | Computed as the variant label. | _Display label._ |
| Parent Path | The relative path of the genomic variant's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/variants/”, followed by the genomic variant ID. | _Path to this GenomicVariant page, chained under its Individual parent._ |
| Variant Type Label | The type label of the genomic variant's variant type. | _Lookup of variant type label._ |
| Variant Class is Rare | True when the genomic variant's variant type is a rare variant class. | _Whether this variant's class is a rare-variant class (observed type property)._ |
| Individual Ancestry Label | The ancestry label of the genomic variant's individual. | _Lookup of individual ancestry for stratification checks._ |
| Is Rare Variant | True when the allele frequency is less than 0.01. | _True when allele frequency below 0.01._ |
| Is Causal Candidate | True when all of the following hold: at least one of the following holds: the is rare variant flag is set or the variant class is rare flag is set and the has allele specific expression flag is set. | _Derived: rare (by frequency or class) AND shows allele-specific expression._ |
| **Omics Assay** | Omics assay instances linking individuals to modalities and tissues; captures batch effects, measurement error, and missing tissues. | — |
| Name | Computed as the assay label. | _Display label._ |
| Parent Path | The relative path of the omics assay's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/assays/”, followed by the omics assay ID. | _Path to this OmicsAssay page, chained under its Individual parent._ |
| Modality Label | The modality label of the omics assay's omics modality. | _Lookup of modality label._ |
| Tissue Label | Determined by priority: the literal “Missing Tissue” if the tissue is blank; otherwise the tissue label of the omics assay's tissue. | _Lookup of tissue label._ |
| Has Batch Effect Risk | True when the measurement error score is greater than 0.3. | _True when measurement error exceeds 0.3._ |
| Is High Quality Assay | True when all of the following hold: it is not the case that the has batch effect risk flag is set and the measurement error score is less than 0.15. | _2nd-order quality flag._ |
| **Evidence Item** | One observed support signal for a causal mechanism, measured by one omics assay. Mechanism confidence is an aggregation over these rows. | — |
| Name | Computed as the evidence label. | _Display name._ |
| Parent Path | The relative path of the evidence item's causal mechanism. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/evidence/”, followed by the evidence item ID. | _Path to this EvidenceItem page, chained under its CausalMechanism parent._ |
| Assay is High Quality | True when the evidence item's omics assay is a high quality assay. | _Whether the supporting assay passed quality control._ |
| Z Stat | Determined by priority: the effect size divided by the standard error if the standard error is greater than 0; otherwise 0. | _Derived signal-to-noise ratio (effect / SE), 0 if SE non-positive._ |
| Is Confound Controlled | True when all of the following hold: the is adjusted for ancestry p cs flag is set and the is adjusted for batch flag is set. | _Derived: both ancestry-PC and batch adjustment present._ |
| Is Qualified Evidence | True when all of the following hold: the assay is high quality flag is set; it is not the case that the is negative control arm flag is set; the z stat is at least 2; and the is confound controlled flag is set. | _Derived: clean assay, real support arm, signal-to-noise >= 2, confound-controlled._ |
| **Cohort Replication** | One re-test of a causal mechanism in another federated cohort. Replication and cross-ancestry transport are aggregations over these rows. | — |
| Name | Computed as the replication label. | _Display name._ |
| Parent Path | The relative path of the cohort replication's causal mechanism. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/replications/”, followed by the cohort replication ID. | _Path to this CohortReplication page, chained under its CausalMechanism parent._ |
| Replicated At Nominal Sig | True when all of the following hold: the replication p value is at most 0.05 and the replication effect sign is 1. | _Derived: reproduced the predicted (positive) sign at nominal significance._ |
| Mechanism Primary Ancestry | The individual ancestry label of the cohort replication's causal mechanism. | _Ancestry of the individual the mechanism was discovered in._ |
| Is Different Ancestry Replication | True when it is not the case that the replication ancestry label is the mechanism primary ancestry. | _Derived: the re-test ran in a different ancestry than discovery._ |
| Is Cross Ancestry Concordant | True when all of the following hold: the replicated at nominal sig flag is set and the is different ancestry replication flag is set. | _Derived: replicated at significance AND in a different ancestry (the transportability atom)._ |
| **Negative Control Test** | One stratification / permutation control on a causal mechanism. A true causal signal collapses under the control. | — |
| Name | Computed as the control label. | _Display name._ |
| Parent Path | The relative path of the negative control test's causal mechanism. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/neg-controls/”, followed by the negative control test ID. | _Path to this NegativeControlTest page, chained under its CausalMechanism parent._ |
| Is Survived | True when the permutation effect size is at most the null threshold. | _Derived: signal collapses under the control (within the null band), as a true causal effect should._ |
| **Environmental Exposure** | Longitudinal environmental exposures contributing to gene-environment-microbiome interactions and shifting environments. | — |
| Name | Computed as the exposure label. | _Display label._ |
| Parent Path | The relative path of the environmental exposure's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/exposures/”, followed by the environmental exposure ID. | _Path to this EnvironmentalExposure page, chained under its Individual parent._ |
| Individual Ancestry Label | The ancestry label of the environmental exposure's individual. | _Ancestry lookup for stratification._ |
| Is High Exposure | True when the exposure level is greater than 5. | _True when exposure level exceeds threshold._ |
| **Treatment** | Treatment histories capturing treatment-induced changes, treatment response, and adverse effects. | — |
| Name | Computed as the treatment label. | _Display label._ |
| Parent Path | The relative path of the treatment's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/treatments/”, followed by the treatment ID. | _Path to this Treatment page, chained under its Individual parent._ |
| Autoimmune Disease Label | The disease label of the treatment's autoimmune disease. | _Disease label lookup._ |
| Is Effective Treatment | True when all of the following hold: at least one of the following holds: the treatment response is the literal “Complete” or the treatment response is the literal “Partial” and it is not the case that the has adverse effect flag is set. | _True for Complete or Partial response without adverse effects._ |
| Is Mechanism Matched | False if the targets mechanism is blank, otherwise the is causal architecture node of the treatment's targets mechanism. | _True when the treatment's target mechanism is a CONFIRMED causal-architecture node (empty-guarded). This is the 'mechanism match'._ |
| Is Treatment Response Predicted | True when all of the following hold: the is effective treatment flag is set and the is mechanism matched flag is set. | _Derived: the treatment is effective AND targets a confirmed mechanism. A drug aimed at a debunked mechanism, or one that didn't respond / was adverse, is NOT predicted._ |
| Treatment Response Deciding Factor | Determined by priority: the literal “EffectiveOnConfirmedMechanism” if the is treatment response predicted flag is set; the literal “NoConfirmedMechanism” if it is not the case that the is mechanism matched flag is set; the literal “AdverseEffect” if the has adverse effect flag is set; the literal “NoResponse” if at least one of the following holds: the treatment response is the literal “None” or the treatment response is the literal “Adverse”; otherwise the literal “Undetermined”. | _Why response is/ isn't predicted — the single deciding reason._ |
| **Clinical Phenotype** | Clinical phenotypes including severity, immune dysfunction markers, and feedback from disease progression. | — |
| Name | Computed as the phenotype label. | _Display label._ |
| Parent Path | The relative path of the clinical phenotype's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/phenotypes/”, followed by the clinical phenotype ID. | _Path to this ClinicalPhenotype page, chained under its Individual parent._ |
| Disease Stage Label | Determined by priority: an empty string if the disease stage is blank; otherwise the stage label of the clinical phenotype's disease stage. | _Stage label lookup._ |
| Is High Severity | True when the severity score is greater than 7. | _True when severity exceeds 7._ |
| Is Presymptomatic Phenotype | True when the disease stage label is the literal “Presymptomatic”. | _True when stage is presymptomatic._ |
| **Causal Mechanism** | Inferred causal mechanisms linking variants, exposures, and molecular state to clinical phenotypes; must be experimentally falsifiable. | — |
| Target Pathway Code | Determined by priority: 1 if the target pathway is the literal “type-I-IFN”; 2 if the target pathway is the literal “B-cell/autoantibody”; 3 if the target pathway is the literal “T-cell-costim”; 4 if the target pathway is the literal “IL-17/23”; otherwise 0. | _Numeric encoding of TargetPathway so the individual can resolve it via a MAXIFS aggregation (transpiler-friendly). 1=type-I-IFN 2=B-cell 3=T-cell 4=IL-17/23._ |
| Name | Computed as the mechanism label. | _Display label._ |
| Parent Path | The relative path of the causal mechanism's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/mechanisms/”, followed by the causal mechanism ID. | _Path to this CausalMechanism page, chained under its Individual parent._ |
| Individual Ancestry Label | The ancestry label of the causal mechanism's individual. | _Ancestry lookup._ |
| Count Qualified Evidence | The number of the causal mechanism's evidence items that are qualified evidences. | _Count of qualified evidence items supporting this mechanism._ |
| Count Modalities Supporting | The number of the causal mechanism's evidence items that are cross modalities and are qualified evidences. | _Count of qualified cross-modality evidence items (multi-omic corroboration)._ |
| Count Intervention Targets | The number of intervention targets related to the causal mechanism. | _Count of perturbable intervention targets for this mechanism (falsifiability requires >=1)._ |
| Is Experimentally Falsifiable | True when all of the following hold: the count intervention targets is at least 1 and the count qualified evidence is at least 1. | _Derived: a measurable qualified effect exists AND a named intervention can perturb it._ |
| Count Replications | The number of cohort replications related to the causal mechanism. | _Total cross-cohort re-tests of this mechanism._ |
| Count Concordant Replications | The number of the causal mechanism's cohort replications that are replicated at nominal sig. | _Re-tests reproducing the predicted sign at significance._ |
| Count Cross Ancestry Concordant | The number of the causal mechanism's cohort replications that are cross ancestry concordants. | _Concordant re-tests that ran in a DIFFERENT ancestry (the transportability measurement)._ |
| Replication Fraction | Determined by priority: the count concordant replications divided by the count replications if the count replications is greater than 0; otherwise 0. | _Derived: fraction of re-tests that were concordant (guarded division)._ |
| Replicates Across Cohorts | True when all of the following hold: the count replications is at least 2 and the count concordant replications is at least 2. | _Derived: >=2 independent re-tests and >=2 concordant._ |
| Count Neg Control Tests | The number of negative control tests related to the causal mechanism. | _Negative-control tests run on this mechanism._ |
| Count Neg Control Survived | The number of the causal mechanism's negative control tests that are survived. | _Negative-control tests the mechanism survived (collapsed under the null)._ |
| Survives Negative Controls | True when all of the following hold: the count neg control tests is at least 1 and the count neg control survived is the count neg control tests. | _Derived: at least one control run AND all of them survived._ |
| Is Spurious Derived | True when at least one of the following holds: it is not the case that the replicates across cohorts flag is set; it is not the case that the survives negative controls flag is set; the count modalities supporting is less than 2; or the has pleiotropy flag is set. | _Derived: spurious unless replicated, survives controls, has >=2 modalities, and is not purely pleiotropic._ |
| Causal Confidence | Determined by priority: 1 if 0.30 times 1 if the count qualified evidence is at least 4, otherwise the count qualified evidence divided by 4 plus 0.20 times 1 if the count modalities supporting is at least 3, otherwise the count modalities supporting divided by 3 plus 0.30 times the replication fraction plus 0.20 times 1 if the survives negative controls flag is set, otherwise 0 is greater than 1; otherwise 0.30 times 1 if the count qualified evidence is at least 4, otherwise the count qualified evidence divided by 4 plus 0.20 times 1 if the count modalities supporting is at least 3, otherwise the count modalities supporting divided by 3 plus 0.30 times the replication fraction plus 0.20 times 1 if the survives negative controls flag is set, otherwise 0. | _Derived bounded blend of qualified-evidence count, modality breadth, replication rate, and control survival._ |
| Variant is Causal Candidate | False if the genomic variant is blank, otherwise the is causal candidate of the causal mechanism's genomic variant. | _Whether the linked variant is itself a derived causal candidate (empty-guarded)._ |
| Is Causal Architecture Node | True when all of the following hold: the causal confidence is at least 0.7; the is experimentally falsifiable flag is set; it is not the case that the is spurious derived flag is set; and at least one of the following holds: the variant is causal candidate flag is set or the environmental exposure has a value. | _Derived: a confirmed causal edge - confident, falsifiable, non-spurious, and grounded in a candidate variant or a real exposure._ |
| Is Ancestry Transportable | True when all of the following hold: the is causal architecture node flag is set and the count cross ancestry concordant is at least 1. | _Derived: a confirmed node whose effect replicated in >=1 different ancestry (measured invariance)._ |
| **Epistatic Interaction** | Higher-order epistasis and pleiotropy interactions between genomic variants. | — |
| Name | Computed as the interaction label. | _Display label._ |
| Parent Path | The relative path of the epistatic interaction's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/epistasis/”, followed by the epistatic interaction ID. | _Path to this EpistaticInteraction page, chained under its Individual parent._ |
| Is High Order Epistasis | True when the epistasis score is greater than 0.5. | _True when epistasis score exceeds 0.5._ |
| **Counterfactual Trajectory** | Counterfactual disease trajectories inferred without randomized perturbation data. | — |
| Name | Computed as the trajectory label. | _Display label._ |
| Parent Path | The relative path of the counterfactual trajectory's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/trajectories/”, followed by the counterfactual trajectory ID. | _Path to this CounterfactualTrajectorie page, chained under its Individual parent._ |
| Autoimmune Disease Label | The disease label of the counterfactual trajectory's autoimmune disease. | _Disease label lookup._ |
| Is Worsening Trajectory | True when the projected severity is greater than 7. | _True when projected severity exceeds 7._ |
| **Individual Prediction** | Predictions of disease onset, severity, treatment response, and adverse effects with calibrated uncertainty for ancestry-equitable risk prediction. | — |
| Name | Computed as the prediction label. | _Display label._ |
| Parent Path | The relative path of the individual prediction's individual. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/predictions/”, followed by the individual prediction ID. | _Path to this IndividualPrediction page, chained under its Individual parent._ |
| Individual Ancestry Label | The ancestry label of the individual prediction's individual. | _Ancestry for equity audit._ |
| Is Ancestry Holdout | True when the individual prediction's individual is ancestry absent from training. | _True when individual ancestry absent from training._ |
| Individual Causal Mass | 0 if the individual is blank, otherwise the sum confirmed causal confidence of the individual prediction's individual. | _Summed confirmed causal confidence for this individual (empty-guarded)._ |
| Individual Confirmed Node Count | 0 if the individual is blank, otherwise the count confirmed causal nodes of the individual prediction's individual. | _Count of this individual's confirmed causal nodes (empty-guarded)._ |
| Individual Cross Ancestry Node Count | 0 if the individual is blank, otherwise the count cross ancestry confirmed nodes of the individual prediction's individual. | _Count of this individual's cross-ancestry-replicated confirmed nodes (empty-guarded)._ |
| Individual Has Cryptic Relatedness | False if the individual is blank, otherwise the has cryptic relatedness flag of the individual prediction's individual. | _Whether this individual carries a cryptic-relatedness leakage flag (empty-guarded)._ |
| Individual Max Severity Score | 0 if the individual is blank, otherwise the max severity score of the individual prediction's individual. | _This individual's max clinical SeverityScore (empty-guarded)._ |
| Individual Has High Severity Phenotype | False if the individual is blank, otherwise the has high severity phenotype of the individual prediction's individual. | _Whether this individual has a high-severity phenotype (empty-guarded)._ |
| Individual Has Predicted Treatment Response | False if the individual is blank, otherwise the has predicted treatment response of the individual prediction's individual. | _Whether this individual has a treatment predicted to respond (empty-guarded)._ |
| Predicted Value | Determined by priority: 10 if 2 times the individual causal mass plus 1.5 times the individual confirmed node count is greater than 10; otherwise 2 times the individual causal mass plus 1.5 times the individual confirmed node count. | _Derived risk magnitude (0-10), a monotone function of validated causal mass only - rides mechanism, not ancestry correlation._ |
| Count Bins | The number of calibration bins related to the individual prediction. | _Total reliability bins for this prediction._ |
| Count Well Calibrated Bins | The number of the individual prediction's calibration bins that are well calibrated bins. | _Bins passing coverage and accuracy._ |
| Sum Bin Abs Error | The total bin abs error across the calibration bins related to the individual prediction. | _Summed reliability gap across this prediction's bins._ |
| Mean Bin Abs Error | Determined by priority: the sum bin abs error divided by the count bins if the count bins is greater than 0; otherwise 1. | _Derived mean reliability gap; defaults to 1 (worst) when no bins exist._ |
| Well Calibrated Fraction | Determined by priority: the count well calibrated bins divided by the count bins if the count bins is greater than 0; otherwise 0. | _Derived fraction of trustworthy bins (guarded division)._ |
| Calibrated Uncertainty | Computed as 0 if 1 minus the mean bin abs error is less than 0, otherwise 1 minus the mean bin abs error times the well calibrated fraction. | _Derived reliability (HIGH = trustworthy): (1 - mean gap) scaled by well-covered-bin fraction; 0 for uncovered predictions._ |
| Rests on Confirmed Mechanism | True when the individual confirmed node count is at least 1. | _Derived: grounded in >=1 validated mechanism._ |
| Has Spurious Correlation Flag | True when at least one of the following holds: it is not the case that the rests on confirmed mechanism flag is set or the individual has cryptic relatedness flag is set. | _Derived: spurious if no validated mechanism OR cryptic-relatedness leakage._ |
| Is Falsifiability Backed | True when the individual confirmed node count is at least 1. | _Derived: inherits falsifiability - every confirmed node required IsExperimentallyFalsifiable._ |
| Is Transportable to Absent Ancestry | True when all of the following hold: the is ancestry holdout flag is set; the individual cross ancestry node count is at least 1; and it is not the case that the has spurious correlation flag is set. | _Derived: a holdout individual is transportable only with >=1 cross-ancestry-replicated node and no spurious flag._ |
| Is Ancestry Transport Safe | True when the is transportable to absent ancestry if the is ancestry holdout flag is set, otherwise true. | _Derived: holdout requires measured transport; in-training is vacuously safe._ |
| Transport Gate Status | Determined by priority: the literal “NotApplicable” if it is not the case that the is ancestry holdout flag is set; the literal “PASS-tested” if the is transportable to absent ancestry flag is set; otherwise the literal “FAIL”. | _RENDER ONLY (does NOT feed the keystone): honest three-state view of the transport gate so a vacuous in-training pass is never shown as evidentiary. NotApplicable = in-training ancestry (gate did not bite); PASS-tested = holdout with a confirmed cross-ancestry transport; FAIL = holdout without one. Sits beside IsAncestryTransportSafe (which the keystone still reads) purely to keep the writeup from implying transport evidence it never used._ |
| Is High Confidence Prediction | True when all of the following hold: the calibrated uncertainty is at least 0.7 and it is not the case that the has spurious correlation flag is set. | _Derived: calibrated AND not spurious._ |
| Patient Stratification Tier | Determined by priority: the literal “High-Risk Pathway” if the predicted value is at least 7; the literal “Moderate-Risk Pathway” if the predicted value is at least 4; otherwise the literal “Low-Risk Pathway”. | _Derived risk tier from the derived PredictedValue._ |
| Predicted Severity Value | Computed as the individual max severity score. | _Derived severity prediction grounded in the individual's max clinical SeverityScore._ |
| Severity Tier | Determined by priority: the literal “Severe” if the predicted severity value is greater than 7; the literal “Moderate” if the predicted severity value is at least 4; otherwise the literal “Mild”. | _Derived severity band from the predicted severity value._ |
| Is Severity Actionable | True when all of the following hold: the individual has high severity phenotype flag is set; the rests on confirmed mechanism flag is set; and it is not the case that the has spurious correlation flag is set. | _Derived: a high-severity phenotype on a confirmed, non-spurious mechanism. Chained to the onset gates so severity can never be actionable on a debunked mechanism._ |
| Severity Deciding Factor | Determined by priority: the literal “HighSeverityOnConfirmedMechanism” if the is severity actionable flag is set; the literal “NotHighSeverity” if it is not the case that the individual has high severity phenotype flag is set; the literal “NoValidatedMechanism” if it is not the case that the rests on confirmed mechanism flag is set; the literal “SpuriousFlag” if the has spurious correlation flag is set; otherwise the literal “Undetermined”. | _Why severity is/ isn't actionable — the single deciding reason._ |
| Is Treatment Response Actionable | True when the individual has predicted treatment response flag is set. | _Derived: the individual has a treatment predicted to respond (effective therapy on a confirmed mechanism). The third prediction type — independent of onset/severity._ |
| Treatment Response Deciding Factor | Determined by priority: the literal “EffectiveOnConfirmedMechanism” if the is treatment response actionable flag is set; the literal “NoEffectiveTreatmentOnMechanism” if the rests on confirmed mechanism flag is set; otherwise the literal “NoConfirmedMechanism”. | _Why treatment-response is/ isn't actionable for this individual._ |
| Is Clinically Actionable | True when all of the following hold: the is high confidence prediction flag is set; the is falsifiability backed flag is set; the is ancestry transport safe flag is set; and the predicted value is greater than 0. | _KEYSTONE: TRUE only when the prediction is high-confidence (calibrated + not spurious), falsifiability-backed, ancestry-transport-safe, and rests on a non-null derived magnitude._ |
| Lifecycle State Key | Determined by priority: the literal “Actionable” if all of the following hold: the is high confidence prediction flag is set; the is falsifiability backed flag is set; the is ancestry transport safe flag is set; and the predicted value is greater than 0; the literal “NotActionable” if at least one of the following holds: it is not the case that the rests on confirmed mechanism flag is set or it is not the case that the is falsifiability backed flag is set; the literal “NotActionable” if the individual has cryptic relatedness flag is set; the literal “NotActionable” if the calibrated uncertainty is less than 0.7; the literal “NotActionable” if it is not the case that the is ancestry transport safe flag is set; otherwise the literal “Actionable”. | _DERIVED current lifecycle state (never entered): the single deciding gate determines whether the case lands on Actionable or NotActionable. Subject-state column of the diagnosis-lifecycle machine._ |
| Deciding Gate | Determined by priority: the literal “AllGatesPass” if the is clinically actionable flag is set; the literal “NoValidatedMechanism” if it is not the case that the rests on confirmed mechanism flag is set; the literal “CrypticRelatedness” if the individual has cryptic relatedness flag is set; the literal “Calibration” if the calibrated uncertainty is less than 0.7; the literal “AncestryTransport” if it is not the case that the is ancestry transport safe flag is set; otherwise the literal “Undetermined”. | _DERIVED single primary deciding gate (never entered), named in keystone-AND priority order. 'AllGatesPass' when actionable. When the case rests on no validated mechanism (no confirmed causal node), Falsifiability, Confidence, and Magnitude are one and the same finding, reported as 'NoValidatedMechanism' rather than split across three gates. Otherwise the lone failing gate is named: CrypticRelatedness, Calibration, AncestryTransport._ |
| Individual Target Pathway | The target pathway of the individual prediction's individual. | _TargetPathway resolved on the individual (decoded from the mechanism)._ |
| Individual Progression State Key | The nephritis progression state key of the individual prediction's individual. | _Lookup: the individual's derived disease state._ |
| Individual is Disease Progressing | True when the individual prediction's individual is disease progressing. | _Lookup: is the individual's disease progressing (simulator)._ |
| Recommended Treatment Line | Determined by priority: the literal “No targeted line — mechanism unconfirmed” if it is not the case that the rests on confirmed mechanism flag is set; the literal “Mycophenolate (induction)” if at least one of the following holds: the individual progression state key is the literal “RenalFlareRisk” or the individual progression state key is the literal “BiopsyIndicated”; the literal “Anifrolumab” if the individual target pathway is the literal “type-I-IFN”; the literal “Belimumab” if the individual target pathway is the literal “B-cell/autoantibody”; the literal “Secukinumab” if the individual target pathway is the literal “IL-17/23”; otherwise the literal “Standard-of-care (no mechanism-matched targeted line)”. | _DERIVED treatment-line recommendation from confirmed-mechanism TargetPathway + disease state. The audit's MMF/belimumab/anifrolumab example._ |
| Treatment Line Deciding Factor | Determined by priority: the literal “MechanismUnconfirmed” if it is not the case that the rests on confirmed mechanism flag is set; the literal “ActiveNephritis-Induction” if at least one of the following holds: the individual progression state key is the literal “RenalFlareRisk” or the individual progression state key is the literal “BiopsyIndicated”; the literal “IFNSignature-Anifrolumab” if the individual target pathway is the literal “type-I-IFN”; the literal “AutoantibodyDriven-Belimumab” if the individual target pathway is the literal “B-cell/autoantibody”; the literal “IL17Axis-Secukinumab” if the individual target pathway is the literal “IL-17/23”; otherwise the literal “NoMechanismMatch”. | _The single deciding reason for the recommended line (mirrors DecidingGate style)._ |
| Progression Vs Actionability Disagree | True when all of the following hold: the individual is disease progressing flag is set and it is not the case that the is clinically actionable flag is set. | _THE COUNTER-EXAMPLE FLAG: TRUE when disease is progressing (simulator) but the prediction is NOT clinically actionable (gate). Proves the two layers are independent._ |
| **Calibration Bin** | Per-prediction reliability-curve bins, seeded from the held-out coverage of this individual's own ancestry x disease. Calibration is an aggregation over these rows. | — |
| Name | Computed as the bin label. | _Display name._ |
| Parent Path | The relative path of the calibration bin's individual prediction. | _Lookup: IndividualPredictions.RelativePath via IndividualPrediction — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/bins/”, followed by the calibration bin ID. | _Path to this CalibrationBin page, chained under its IndividualPrediction parent._ |
| Bin Abs Error | Determined by priority: the predicted probability band minus the observed event rate if the predicted probability band is at least the observed event rate; otherwise the observed event rate minus the predicted probability band. | _Derived: absolute gap between predicted band and observed rate._ |
| Is Well Calibrated Bin | True when all of the following hold: the coverage count is at least 20 and the bin abs error is at most 0.1. | _Derived: enough held-out coverage AND small reliability gap._ |
| **Intervention Target** | Validated intervention targets for gene-based and cell-based therapies derived from falsifiable causal mechanisms. | — |
| Name | Computed as the target label. | _Display label._ |
| Parent Path | The relative path of the intervention target's causal mechanism. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Computed as the parent path, followed by the literal “/targets/”, followed by the intervention target ID. | _Path to this InterventionTarget page, chained under its CausalMechanism parent._ |
| Causal Mechanism Label | The mechanism label of the intervention target's causal mechanism. | _Mechanism label lookup._ |
| Is Gene Based Therapy | True when the therapy class is the literal “Gene-based”. | _True when therapy class is gene-based._ |
| Is Cell Based Therapy | True when the therapy class is the literal “Cell-based”. | _True when therapy class is cell-based._ |
| **Axiom** | Non-negotiable invariants the platform must obey. Load-bearing constraints, not per-loop work. Captured from the gauntlet conversation. | — |
| Name | Computed as the statement. | _Display label._ |
| Relative Path | Computed as the literal “/admin/axioms/”, followed by the axiom ID. | _Path to this Axioms entry: /admin/axioms/<id>._ |
| **Tests for Success** | Falsifiable conditions that prove the axioms hold. The human-readable index of what each demonstration shows; many are realized in the witnessed harness. | — |
| Name | Computed as the claim. | _Display label._ |
| Relative Path | Computed as the literal “/admin/tests-for-success/”, followed by the test for success ID. | _Path to this TestsForSuccess entry: /admin/tests-for-success/<id>._ |
| **Feature** | Catalog of buildable capabilities the platform has / allows for. Coarser grain than loops. Each row carries challenge provenance (ChallengeRefs: exact quoted text + file/line/col into THE-ORIGINAL-CHALLENGE.md, for UI hover tips), a Category, a Priority, and a free-form Markdown ChallengeNotes comment. Source of truth for the DERIVED PLATFORM_FEATURES.md. AssignedLoop links a feature to the loop that delivers it (nullable until scheduled). | — |
| Name | Computed as the title. | _Display label._ |
| Relative Path | Computed as the literal “/admin/features/”, followed by the feature ID. | _Path to this Features entry: /admin/features/<id>._ |
| Meta Line | Computed as the literal “**Category:** ”, followed by the category, followed by the literal “ - **Priority:** ”, followed by the priority, followed by the literal “ - **Challenge refs:** ”, followed by the ref count. | _One-line meta summary for the catalog (category - priority - ref count)._ |
| **Inference Kind** | Families of derivation the platform performs - the KINDS of inference in the DAG (lookups, aggregations, higher-order gates, state machines, transitive closure, predicate-gated narrative, cross-substrate conformance), independent of any one feature. Lets PLATFORM_FEATURES.md open with a short overview of the reasoning machinery before the per-feature catalog. Grounded in field types that actually exist in this rulebook; Maturity is Live unless an upstream step is still in flight. | — |
| Name | Computed as the title. | _Display label._ |
| Relative Path | Computed as the literal “/admin/inference-kinds/”, followed by the inference kind ID. | _Path to this entry: /admin/inference-kinds/<id>._ |
| **Open Question** | Decisions still pending, captured so they are not silently re-litigated in a later session. | — |
| Name | Computed as the question. | _Display label._ |
| Relative Path | Computed as the literal “/admin/open-questions/”, followed by the open question ID. | _Path to this OpenQuestions entry: /admin/open-questions/<id>._ |
| **Non Goal** | Explicit out-of-scope statements — the positive twin of the anti-hallucination ledger. Stops scope creep. | — |
| Name | Computed as the statement. | _Display label._ |
| Relative Path | Computed as the literal “/admin/non-goals/”, followed by the non goal ID. | _Path to this NonGoals entry: /admin/non-goals/<id>._ |
| **Glossary Term** | Vocabulary coined in the gauntlet conversation, so the framing is shared and stable across sessions. | — |
| Name | Computed as the term. | _Display label._ |
| Relative Path | Computed as the literal “/admin/glossary/”, followed by the glossary term ID. | _Path to this GlossaryTerms entry: /admin/glossary/<id>._ |
| **Leopold Loop** | The ordered Leopold loops that build this platform, as data. The derived plan (LEOPOLD_LOOPs.md, via json-hbars-transform) is generated from these rows; completed ([DONE]) loops are pruned at publish so only current/roadmap work shows in the plan. | — |
| Name | Computed as the literal “Loop ”, followed by the loop number, followed by the literal “ — ”, followed by the title. | _Display label._ |
| Relative Path | Computed as the literal “/admin/leopold-loops/”, followed by the leopold loop ID. | _Path to this LeopoldLoops entry: /admin/leopold-loops/<id>._ |
| Completedness | Computed as the status. | _Normalized status used by the derived plan to decide placement._ |
| Is in Current Plan | True when it is not the case that the status is the literal “done”. | _TRUE for the current "next" loop and anything still planned/backlog (not done)._ |
| **Routing and Navigation** | Role-based navigation: open-ended parent->child->leaf routes with computed paths. Each route has a template (/intake/case/:caseId); entities carry RelativePath that substitutes their own id/slug. Roles: admin, intake-clinician, diagnosing-doctor, external-llm. | — |
| Name | Computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> | _Slug of the display name._ |
| Admin Can Create | True when the admin CRUD mentions the literal “C”. | _Derived: AdminCRUD contains 'C'._ |
| Admin Can Read | True when the admin CRUD mentions the literal “R”. | _Derived: AdminCRUD contains 'R'._ |
| Admin Can Update | True when the admin CRUD mentions the literal “U”. | _Derived: AdminCRUD contains 'U'._ |
| Admin Can Delete | True when the admin CRUD mentions the literal “D”. | _Derived: AdminCRUD contains 'D'._ |
| Intake Clinician Can Create | True when the intake clinician CRUD mentions the literal “C”. | _Derived: IntakeClinicianCRUD contains 'C'._ |
| Intake Clinician Can Read | True when the intake clinician CRUD mentions the literal “R”. | _Derived: IntakeClinicianCRUD contains 'R'._ |
| Intake Clinician Can Update | True when the intake clinician CRUD mentions the literal “U”. | _Derived: IntakeClinicianCRUD contains 'U'._ |
| Intake Clinician Can Delete | True when the intake clinician CRUD mentions the literal “D”. | _Derived: IntakeClinicianCRUD contains 'D'._ |
| Diagnosing Doctor Can Create | True when the diagnosing doctor CRUD mentions the literal “C”. | _Derived: DiagnosingDoctorCRUD contains 'C'._ |
| Diagnosing Doctor Can Read | True when the diagnosing doctor CRUD mentions the literal “R”. | _Derived: DiagnosingDoctorCRUD contains 'R'._ |
| Diagnosing Doctor Can Update | True when the diagnosing doctor CRUD mentions the literal “U”. | _Derived: DiagnosingDoctorCRUD contains 'U'._ |
| Diagnosing Doctor Can Delete | True when the diagnosing doctor CRUD mentions the literal “D”. | _Derived: DiagnosingDoctorCRUD contains 'D'._ |
| External Llm Can Create | True when the external llm CRUD mentions the literal “C”. | _Derived: ExternalLlmCRUD contains 'C'._ |
| External Llm Can Read | True when the external llm CRUD mentions the literal “R”. | _Derived: ExternalLlmCRUD contains 'R'._ |
| External Llm Can Update | True when the external llm CRUD mentions the literal “U”. | _Derived: ExternalLlmCRUD contains 'U'._ |
| External Llm Can Delete | True when the external llm CRUD mentions the literal “D”. | _Derived: ExternalLlmCRUD contains 'D'._ |
| Depth | Determined by priority: 0 if the parent route key is blank; otherwise the length of the route key minus the length of the route key with every a period replaced by an empty string. ⚠︎ mechanical <!-- rulespeak:reword --> | _Nesting depth: 0 = top-level; otherwise number of dot-segments in RouteKey._ |
| Full Path | Computed as the route. | _Canonical role-agnostic URL path (equals Route)._ |
| Handler Base Name | Computed as the route key with every a period replaced by a space with every a hyphen replaced by a space. ⚠︎ mechanical <!-- rulespeak:reword --> | _Space-delimited form of RouteKey; client PascalCases + prefixes role to derive a handler component._ |
| Relative Path | Computed as the literal “/admin/routing/”, followed by the routing and navigation ID. | _Path to this routing node's own admin page._ |
| **State Machine** | State-machine definitions. | — |
| Relative Path | Computed as the literal “/admin/state-machine/”, followed by the state machine ID. | _Path to this machine's page._ |
| State Count | The number of machine states related to the state machine. | _Count of MachineStates in this machine._ |
| Transition Rule Count | The number of state transition rules related to the state machine. | _Count of StateTransitionRules in this machine._ |
| **Machine State** | Legal states of each machine. | — |
| Relative Path | Computed as the literal “/admin/state-machine/states/”, followed by the machine state ID. | _Path to this state's page._ |
| Reachable State Count | The number of vw state transition rules closure related to the machine state. | _Number of states reachable FROM this state via the transitive closure of the transition edges (rollup over vw_state_transition_rules_closure where from_id = this state). This is a graph-reachability fact the linear severity rank CANNOT reproduce: both Quiescent (the remission sink) and BiopsyIndicated (the terminal progression state) have 0 reachable-ahead, though they sit at opposite ends of the order — the count is non-monotonic in OrderIndex because the machine BRANCHES (remission). The closure is load-bearing here: delete it and this field is uncomputable._ |
| **State Transition Rule** | Legal edges (guards) of each machine. | — |
| Relative Path | Computed as the literal “/admin/state-machine/rules/”, followed by the state transition rule ID. | _Path to this rule's page._ |
| From State Key | The state key of the state transition rule's from state. | _Lookup: FromState.StateKey._ |
| To State Key | The state key of the state transition rule's to state. | _Lookup: ToState.StateKey._ |
| Is Forward Edge | True when it is not the case that the to state key is the from state key. | _TRUE when the edge advances to a different state (all edges here)._ |
| **State Transition** | Instance-level transition log (the witnessed history). | — |
| Relative Path | Computed as the literal “/admin/state-machine/transitions/”, followed by the state transition ID. | _Path to this transition's page._ |
| Is Forward | True when it is not the case that the to state key is the literal “Intake”. | _TRUE when ToStateKey is not the machine's initial state._ |
| **Subject State Instance** | Per-subject state occupancy records; current state has blank ExitedAt. | — |
| Relative Path | Computed as the literal “/admin/state-machine/instances/”, followed by the subject state instance ID. | _Path to this occupancy record._ |
| Is Current | True when the exited at is blank. | _TRUE when ExitedAt IS NULL — the subject's active state._ |
| Has Complete Lineage | True when the sequence index is at least 1. | _TRUE when lineage walks back to SequenceIndex 1._ |
| Is Long Dwell | True when the dwell days is at least 90. | _TRUE when DwellDays >= 90 (a season)._ |
| **Disease Domain Concept** | v2 vocabulary completeness: every disease-domain concept the v1 audit named, with its modeling status and the challenge-stressor TYPE it instantiates. Makes the coverage claim checkable by grep, not by trust. | — |
| Name | Computed as the concept label. | _Echoes ConceptLabel._ |
| Relative Path | Computed as the literal “/admin/disease-concepts/”, followed by the disease domain concept ID. | _Path to this concept's page._ |
| Is Deeply Modeled | True when the modeling status is the literal “deep-dag”. | _TRUE when this concept carries a witnessed inference DAG (status = deep-dag)._ |
| Is Schema Modeled | True when at least one of the following holds: the modeling status is the literal “deep-dag” or the modeling status is the literal “schema”. | _TRUE when first-class schema/data (deep-dag or schema)._ |
| **Serology Observation** | RAW longitudinal serology panels (the lab/LLM layer) + derived trend, SLEDAI sub-scores, and the disease state IMPLIED by each panel. The leaves that drive the lupus-nephritis progression machine. | — |
| Prior Anti Ds Dna IU | The anti ds dna IU of the serology observation's prior observation. | _Prior panel's dsDNA via PriorObservation FK, for trend._ |
| Prior C3 | The complement c3 of the serology observation's prior observation. | _Prior C3 via PriorObservation FK._ |
| Prior C4 | The complement c4 of the serology observation's prior observation. | _Prior C4 via PriorObservation FK._ |
| Anti Ds Dna Trend | Determined by priority: the literal “Stable” if the prior anti ds dna IU is blank; the literal “Rising” if the anti ds dna IU is greater than the prior anti ds dna IU times 1.25; the literal “Falling” if the anti ds dna IU is less than the prior anti ds dna IU times 0.8; otherwise the literal “Stable”. | _Rising/Falling/Stable vs prior panel (derived from raw)._ |
| Complement Trend | Determined by priority: the literal “Stable” if the prior c3 is blank; the literal “Falling” if the complement c3 plus the complement c4 is less than the prior c3 plus the prior c4 times 0.85; the literal “Rising” if the complement c3 plus the complement c4 is greater than the prior c3 plus the prior c4 times 1.15; otherwise the literal “Stable”. | _Rising/Falling/Stable on C3+C4 vs prior (derived)._ |
| Is Pre Nephritic Signature Panel | True when all of the following hold: the anti ds dna trend is the literal “Rising” and the complement trend is the literal “Falling”. | _The pre-nephritic serology signature at THIS panel: rising anti-dsDNA + falling complement (the serological trajectory that precedes/tracks overt renal involvement). Emergent from the raw series, not a label anyone assigned. (Proteinuria is the OUTCOME the signature precedes, so it is deliberately NOT part of the signal.)_ |
| Is Significant Proteinuria | True when the proteinuria g per day is at least 0.5. | _proteinuria >= 0.5 g/day._ |
| Is Nephrotic Range Proteinuria | True when the proteinuria g per day is at least 3.0. | _proteinuria >= 3.0 g/day._ |
| Sledai Renal Points | Determined by priority: 8 if at least one of the following holds: the is nephrotic range proteinuria flag is set or the has active urinary sediment flag is set; 4 if the is significant proteinuria flag is set; otherwise 0. | _SLEDAI-style renal sub-score (0/4/8) from proteinuria + sediment._ |
| Sledai Serology Points | Determined by priority: 4 if all of the following hold: the complement trend is the literal “Falling” and the anti ds dna trend is the literal “Rising”; 2 if at least one of the following holds: the complement trend is the literal “Falling” or the anti ds dna trend is the literal “Rising”; otherwise 0. | _SLEDAI-style serology sub-score (0/2/4) from low-complement + raised dsDNA._ |
| Sledai Score | Computed as the sledai renal points plus the sledai serology points. | _Derived SLEDAI-style activity score = renal + serology points._ |
| Progression State Key | Determined by priority: the literal “BiopsyIndicated” if at least one of the following holds: the is nephrotic range proteinuria flag is set or the has active urinary sediment flag is set; the literal “RenalFlareRisk” if the proteinuria g per day is at least 1.0; the literal “EarlyNephritis” if the is significant proteinuria flag is set; the literal “SerologicActive” if all of the following hold: the anti ds dna trend is the literal “Rising” and the complement trend is the literal “Falling”; otherwise the literal “PresymptomaticAutoimmunity”. | _Disease state IMPLIED by THIS panel (derived purely from raw leaves)._ |
| Progression State Order | Determined by priority: 5 if the progression state key is the literal “BiopsyIndicated”; 4 if the progression state key is the literal “RenalFlareRisk”; 3 if the progression state key is the literal “EarlyNephritis”; 2 if the progression state key is the literal “SerologicActive”; otherwise 1. | _Numeric severity order of THIS panel's implied state (0..5). Lets the individual's current state be a MAXIFS over panels (highest state reached)._ |
| Relative Path | Computed as the literal “/admin/serology/”, followed by the serology observation ID. | _Path._ |
| **Therapy Option** | Therapy lookup for the treatment-line-selection DAG (MMF/belimumab/anifrolumab/secukinumab). | — |
| Name | Computed as the therapy label. | _Echo._ |
| Relative Path | Computed as the literal “/admin/therapy-options/”, followed by the therapy option ID. | _Path._ |

## 2 Fact Types

- a **machine state** references exactly one **state machine**
- a **machine state** may reference one **state transition rule**
- a **state transition rule** references exactly one **state machine**
- a **state transition rule** references exactly one **machine state**
- a **state transition** references exactly one **state machine**
- a **subject state instance** references exactly one **state machine**
- a **subject state instance** may reference one **subject state instance**
- a **subject state instance** may reference one **state transition**
- a **serology observation** references exactly one **individual**
- a **serology observation** may reference one **serology observation**

## 2b Reachability Rules

_A reachability rule is a transitive closure: relationships that hold not only
directly but through any chain of the same relationship. The asserted edges are
the single source of truth; the inferred edges are necessary consequences of them._

- **Progression Closure** — one state transition rule is reachable from another by the **progression** relationship
  when the second can be reached from the first by following one or more **progression** edges
  (from its from state to its to state), whether directly asserted or reached transitively.
  - An edge is **asserted** when it exists directly in the state transition rules; it is **inferred**
    when no direct edge states it but it follows from a chain of asserted edges.
  - The **hop distance** of a reachable pair is the length of the shortest such chain
    (1 for a directly-asserted edge).
  - _Transitive closure of state-machine transitions (an owl:TransitiveProperty). The asserted FromState->ToState edges imply the full reachability ordering of each machine - including never-asserted long-range pairs such as PresymptomaticAutoimmunity -> BiopsyIndicated. Materialized by the transpiler as the cycle-safe recursive view vw_state_transition_rules_closure(from_id, to_id, hop_distance, is_inferred): asserted edges (hop 1) plus inferred reachability rows. The disease trajectory is derived from the transition topology, not hand-asserted._

## 3 Operative Rules

_Operative rules state what the business **obliges**, **prohibits**, or
advises (**should**). Structural rules come from required fields and foreign keys;
semantic rules come from the Constraints table, each keyed on a boolean the rulebook
already computes (cross-referenced as DR-N in the Definitional Rules below)._

### Structural Constraints (from the schema)

- An autoimmune diseas **must** have a disease label, and record whether it is a complex disease.
- A disease stage **must** have a stage label, a sort order, and an autoimmune disease.
- A tissue **must** have a tissue label.
- An omics modality **must** have a modality label, and record whether it is a single cell.
- A federated dataset **must** have a node label and a region, and record whether it is privacy preserving.
- A variant type **must** have a type label, and record whether it is a rare variant class.
- An individual **must** have a given name, a family name, an ancestry label, and an age years, and record whether it is ancestry absent from training and whether it has a cryptic relatedness flag.
- A genomic variant **must** have a variant label, an individual, a variant type, and an allele frequency, and record whether it has an allele specific expression.
- An omics assay **must** have an assay label, an individual, an omics modality, and a measurement error score, and record whether it has a cell state specific effect.
- An evidence item **must** have an evidence label, a causal mechanism, an omics assay, an effect size, and a standard error, and record whether it is a cross modality, whether it is a negative control arm, whether it is an adjusted for ancestry p cs, whether it is an adjusted for batch, and whether it is a synthetic leaf.
- A cohort replication **must** have a replication label, a causal mechanism, a federated dataset, a replication effect sign, a replication p value, and a replication ancestry label.
- A negative control test **must** have a control label, a causal mechanism, a test kind, a permutation effect size, and a null threshold.
- An environmental exposure **must** have an exposure label, an individual, and an exposure level, and record whether it is a maternal effect.
- A treatment **must** have a treatment label, an individual, an autoimmune disease, and a treatment response, and record whether it has a treatment induced change and whether it has an adverse effect.
- A clinical phenotype **must** have a phenotype label, an individual, an autoimmune disease, and a severity score, and record whether it has an immune dysfunction.
- A causal mechanism **must** have a mechanism label, an individual, and a mechanism type, and record whether it has a pleiotropy.
- An epistatic interaction **must** have an interaction label, an individual, a primary variant, a secondary variant, and an epistasis score, and record whether it has a pleiotropy.
- A counterfactual trajectory **must** have a trajectory label, an individual, an autoimmune disease, a projected severity, and a horizon months.
- An individual prediction **must** have a prediction label, an individual, an autoimmune disease, and a prediction type.
- A calibration bin **must** have a bin label, an individual prediction, a predicted probability band, an observed event rate, and a coverage count.
- An intervention target **must** have a target label, a causal mechanism, an autoimmune disease, and a therapy class, and record whether it is validated.
- An axiom **must** have a statement, a rationale, and a category.
- A tests for success **must** have a claim, a how witnessed, and a status.
- A feature **must** have a title, a category, a priority, a description, and a ref count.
- An inference kind **must** have a title, a description, an example field, an evidence count, a maturity, and a sort order.
- An open question **must** have a question and a context, and record whether it is resolved.
- A non goal **must** have a statement and a why excluded.
- A glossary term **must** have a term and a definition.
- A leopold loop **must** have a loop number, a title, a goal, a status, and a sort order.
- A state machine **must** have a subject table name and a subject state column.
- A machine state **must** reference exactly one state machine.
- A machine state **must** have a state key.
- A state transition rule **must** reference exactly one state machine.
- A state transition rule **must** reference exactly one machine state as its from state.
- A state transition rule **must** reference exactly one machine state as its to state.
- A state transition **must** reference exactly one state machine.
- A state transition **must** have a subject table name and a to state key.
- A subject state instance **must** reference exactly one state machine.
- A subject state instance **must** have a subject table name, a state key, and a sequence index.
- A disease domain concept **must** have a concept label and a modeling status.
- A serology observation **must** reference exactly one individual.
- A serology observation **must** have an observed at and a sequence index.
- A therapy option **must** have a therapy label.

## 4 Definitional Rules

_All statements express truth in the business domain; they are neither
procedures nor imperatives. "iff" is avoided in favor of "only if" so a
one-directional necessity is not mistaken for an equivalence. A
**⚠︎ mechanical** chip marks a rule whose deterministic wording is faithful
but clunky — a flag for an optional downstream reword pass, not a defect._

| ID | Declarative rule |
|----|------------------|
| **DR-1 Name** | An autoimmune diseas's name is computed as the disease label. |
| **DR-2 Relative Path** | An autoimmune diseas's relative path is computed as the literal “/diseases/”, followed by the autoimmune disease ID. |
| **DR-3 Count of Disease Stages** | An autoimmune diseas's count of disease stages is the number of disease stages related to the autoimmune diseas. |
| **DR-4 Count of Intervention Targets** | An autoimmune diseas's count of intervention targets is the number of intervention targets related to the autoimmune diseas. |
| **DR-5 Name** | A disease stage's name is computed as the autoimmune disease disease label, followed by the literal “ — ”, followed by the stage label. |
| **DR-6 Parent Path** | A disease stage's parent path is the relative path of the disease stage's autoimmune disease. |
| **DR-7 Relative Path** | A disease stage's relative path is computed as the parent path, followed by the literal “/stages/”, followed by the disease stage ID. |
| **DR-8 Autoimmune Disease Disease Label** | A disease stage's autoimmune disease disease label is the disease label of the disease stage's autoimmune disease. |
| **DR-9 Is Presymptomatic** | A disease stage is considered a presymptomatic if the stage label is the literal “Presymptomatic”. |
| **DR-10 Name** | A tissue's name is computed as the tissue label. |
| **DR-11 Relative Path** | A tissue's relative path is computed as the literal “/tissues/”, followed by the tissue ID. |
| **DR-12 Count of Omics Assays** | A tissue's count of omics assays is the number of omics assays related to the tissue. |
| **DR-13 Name** | An omics modality's name is computed as the modality label. |
| **DR-14 Relative Path** | An omics modality's relative path is computed as the literal “/omics-modalities/”, followed by the omics modality ID. |
| **DR-15 Count of Omics Assays** | An omics modality's count of omics assays is the number of omics assays related to the omics modality. |
| **DR-16 Name** | A federated dataset's name is computed as the node label. |
| **DR-17 Relative Path** | A federated dataset's relative path is computed as the literal “/datasets/”, followed by the federated dataset ID. |
| **DR-18 Count of Individuals** | A federated dataset's count of individuals is the number of individuals related to the federated dataset. |
| **DR-19 Name** | A variant type's name is computed as the type label. |
| **DR-20 Relative Path** | A variant type's relative path is computed as the literal “/variant-types/”, followed by the variant type ID. |
| **DR-21 Count of Genomic Variants** | A variant type's count of genomic variants is the number of genomic variants related to the variant type. |
| **DR-22 Name** | An individual's name is computed as the given name, followed by a space, followed by the family name. |
| **DR-23 Slug** | An individual's slug is computed as the lower-cased family name, followed by a hyphen, followed by the given name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-24 Relative Path** | An individual's relative path is computed as the literal “/intake/new-patient/”, followed by the slug. |
| **DR-25 Federated Dataset Node Label** | The individual's federated dataset node label is determined by the following priority:<br>1. an empty string, if the federated dataset is blank;<br>2. otherwise the node label of the individual's federated dataset. |
| **DR-26 Count of Genomic Variants** | An individual's count of genomic variants is the number of genomic variants related to the individual. |
| **DR-27 Count of Causal Mechanisms** | An individual's count of causal mechanisms is the number of causal mechanisms related to the individual. |
| **DR-28 Count of Epistatic Interactions** | An individual's count of epistatic interactions is the number of epistatic interactions related to the individual. |
| **DR-29 Rare Variant Burden Score** | The individual's rare variant burden score is determined by the following priority:<br>1. the count of genomic variants divided by the age years, if the age years is greater than 0;<br>2. otherwise 0. |
| **DR-30 Causal Architecture Score** | An individual's causal architecture score is computed as the count of causal mechanisms times 10 plus the count of epistatic interactions times 5 plus the rare variant burden score. |
| **DR-31 Is Development Window** | An individual is considered a development window if the age years is at most 25. |
| **DR-32 Is Aging Window** | An individual is considered an aging window if the age years is at least 60. |
| **DR-33 Count Confirmed Causal Nodes** | An individual's count confirmed causal nodes is the number of the individual's causal mechanisms that are causal architecture nodes. |
| **DR-34 Sum Confirmed Causal Confidence** | An individual's sum confirmed causal confidence is the total causal confidence across the individual's causal mechanisms that are causal architecture nodes. |
| **DR-35 Count Cross Ancestry Confirmed Nodes** | An individual's count cross ancestry confirmed nodes is the number of the individual's causal mechanisms that are ancestry transportables. |
| **DR-36 Max Severity Score** | An individual's max severity score is the largest severity score across the clinical phenotypes related to the individual. |
| **DR-37 Count High Severity Phenotypes** | An individual's count high severity phenotypes is the number of the individual's clinical phenotypes that are high severities. |
| **DR-38 Has High Severity Phenotype** | An individual is considered to have a high severity phenotype if the count high severity phenotypes is at least 1. |
| **DR-39 Count Predicted Treatment Responses** | An individual's count predicted treatment responses is the number of the individual's treatments that are treatment response predicted. |
| **DR-40 Has Predicted Treatment Response** | An individual is considered to have a predicted treatment response if the count predicted treatment responses is at least 1. |
| **DR-41 Count Serology Panels** | An individual's count serology panels is the number of serology observations related to the individual. |
| **DR-42 Count Pre Nephritic Signature Panels** | An individual's count pre nephritic signature panels is the number of the individual's serology observations that are pre nephritic signature panels. |
| **DR-43 Is in Pre Nephritic Signature Cluster** | An individual is considered in pre nephritic signature cluster if the count pre nephritic signature panels is at least 1. |
| **DR-44 Signature Strength** | The individual's signature strength is determined by the following priority:<br>1. 2, if the count pre nephritic signature panels is at least 2;<br>2. 1, if the count pre nephritic signature panels is at least 1;<br>3. otherwise 0. |
| **DR-45 Max Progression State Order** | An individual's max progression state order is the largest progression state order across the serology observations related to the individual. |
| **DR-46 Latest Sledai Score** | An individual's latest sledai score is the largest sledai score across the serology observations related to the individual. |
| **DR-47 Nephritis Progression State Key** | The individual's nephritis progression state key is determined by the following priority:<br>1. the literal “BiopsyIndicated”, if the max progression state order is at least 5;<br>2. the literal “RenalFlareRisk”, if the max progression state order is at least 4;<br>3. the literal “EarlyNephritis”, if the max progression state order is at least 3;<br>4. the literal “SerologicActive”, if the max progression state order is at least 2;<br>5. otherwise the literal “PresymptomaticAutoimmunity”. |
| **DR-48 Activity Tier** | The individual's activity tier is determined by the following priority:<br>1. the literal “High / flare”, if the latest sledai score is at least 12;<br>2. the literal “Moderate”, if the latest sledai score is at least 6;<br>3. the literal “Mild”, if the latest sledai score is at least 1;<br>4. otherwise the literal “Quiescent”. |
| **DR-49 Is High Disease Activity** | An individual is considered a high disease activity if the latest sledai score is at least 12. |
| **DR-50 Is Disease Progressing** | An individual is considered disease progressing if at least one of the following holds: the nephritis progression state key is the literal “EarlyNephritis”; the nephritis progression state key is the literal “RenalFlareRisk”; or the nephritis progression state key is the literal “BiopsyIndicated”. |
| **DR-51 Target Pathway Code** | An individual's target pathway code is the largest target pathway code across the causal mechanisms related to the individual. |
| **DR-52 Target Pathway** | The individual's target pathway is determined by the following priority:<br>1. the literal “type-I-IFN”, if the target pathway code is 1;<br>2. the literal “B-cell/autoantibody”, if the target pathway code is 2;<br>3. the literal “T-cell-costim”, if the target pathway code is 3;<br>4. the literal “IL-17/23”, if the target pathway code is 4;<br>5. otherwise an empty string. |
| **DR-53 Reachable States Ahead** | An individual's reachable states ahead is the reachable state count of the individual's current progression state ID. |
| **DR-54 Name** | A genomic variant's name is computed as the variant label. |
| **DR-55 Parent Path** | A genomic variant's parent path is the relative path of the genomic variant's individual. |
| **DR-56 Relative Path** | A genomic variant's relative path is computed as the parent path, followed by the literal “/variants/”, followed by the genomic variant ID. |
| **DR-57 Variant Type Label** | A genomic variant's variant type label is the type label of the genomic variant's variant type. |
| **DR-58 Variant Class is Rare** | A genomic variant's variant class is rare is true when the genomic variant's variant type is a rare variant class. |
| **DR-59 Individual Ancestry Label** | A genomic variant's individual ancestry label is the ancestry label of the genomic variant's individual. |
| **DR-60 Is Rare Variant** | A genomic variant is considered a rare variant if the allele frequency is less than 0.01. |
| **DR-61 Is Causal Candidate** | A genomic variant is considered a causal candidate if all of the following hold: at least one of the following holds: the is rare variant flag is set or the variant class is rare flag is set and the has allele specific expression flag is set. |
| **DR-62 Name** | An omics assay's name is computed as the assay label. |
| **DR-63 Parent Path** | An omics assay's parent path is the relative path of the omics assay's individual. |
| **DR-64 Relative Path** | An omics assay's relative path is computed as the parent path, followed by the literal “/assays/”, followed by the omics assay ID. |
| **DR-65 Modality Label** | An omics assay's modality label is the modality label of the omics assay's omics modality. |
| **DR-66 Tissue Label** | The omics assay's tissue label is determined by the following priority:<br>1. the literal “Missing Tissue”, if the tissue is blank;<br>2. otherwise the tissue label of the omics assay's tissue. |
| **DR-67 Has Batch Effect Risk** | An omics assay is considered to have a batch effect risk if the measurement error score is greater than 0.3. |
| **DR-68 Is High Quality Assay** | An omics assay is considered a high quality assay if all of the following hold: it is not the case that the has batch effect risk flag is set and the measurement error score is less than 0.15. |
| **DR-69 Name** | An evidence item's name is computed as the evidence label. |
| **DR-70 Parent Path** | An evidence item's parent path is the relative path of the evidence item's causal mechanism. |
| **DR-71 Relative Path** | An evidence item's relative path is computed as the parent path, followed by the literal “/evidence/”, followed by the evidence item ID. |
| **DR-72 Assay is High Quality** | An evidence item's assay is high quality is true when the evidence item's omics assay is a high quality assay. |
| **DR-73 Z Stat** | The evidence item's z stat is determined by the following priority:<br>1. the effect size divided by the standard error, if the standard error is greater than 0;<br>2. otherwise 0. |
| **DR-74 Is Confound Controlled** | An evidence item is considered confound controlled if all of the following hold: the is adjusted for ancestry p cs flag is set and the is adjusted for batch flag is set. |
| **DR-75 Is Qualified Evidence** | An evidence item is considered a qualified evidence if all of the following hold: the assay is high quality flag is set; it is not the case that the is negative control arm flag is set; the z stat is at least 2; and the is confound controlled flag is set. |
| **DR-76 Name** | A cohort replication's name is computed as the replication label. |
| **DR-77 Parent Path** | A cohort replication's parent path is the relative path of the cohort replication's causal mechanism. |
| **DR-78 Relative Path** | A cohort replication's relative path is computed as the parent path, followed by the literal “/replications/”, followed by the cohort replication ID. |
| **DR-79 Replicated At Nominal Sig** | A cohort replication is flagged replicated at nominal sig if all of the following hold: the replication p value is at most 0.05 and the replication effect sign is 1. |
| **DR-80 Mechanism Primary Ancestry** | A cohort replication's mechanism primary ancestry is the individual ancestry label of the cohort replication's causal mechanism. |
| **DR-81 Is Different Ancestry Replication** | A cohort replication is considered a different ancestry replication if it is not the case that the replication ancestry label is the mechanism primary ancestry. |
| **DR-82 Is Cross Ancestry Concordant** | A cohort replication is considered a cross ancestry concordant if all of the following hold: the replicated at nominal sig flag is set and the is different ancestry replication flag is set. |
| **DR-83 Name** | A negative control test's name is computed as the control label. |
| **DR-84 Parent Path** | A negative control test's parent path is the relative path of the negative control test's causal mechanism. |
| **DR-85 Relative Path** | A negative control test's relative path is computed as the parent path, followed by the literal “/neg-controls/”, followed by the negative control test ID. |
| **DR-86 Is Survived** | A negative control test is considered survived if the permutation effect size is at most the null threshold. |
| **DR-87 Name** | An environmental exposure's name is computed as the exposure label. |
| **DR-88 Parent Path** | An environmental exposure's parent path is the relative path of the environmental exposure's individual. |
| **DR-89 Relative Path** | An environmental exposure's relative path is computed as the parent path, followed by the literal “/exposures/”, followed by the environmental exposure ID. |
| **DR-90 Individual Ancestry Label** | An environmental exposure's individual ancestry label is the ancestry label of the environmental exposure's individual. |
| **DR-91 Is High Exposure** | An environmental exposure is considered a high exposure if the exposure level is greater than 5. |
| **DR-92 Name** | A treatment's name is computed as the treatment label. |
| **DR-93 Parent Path** | A treatment's parent path is the relative path of the treatment's individual. |
| **DR-94 Relative Path** | A treatment's relative path is computed as the parent path, followed by the literal “/treatments/”, followed by the treatment ID. |
| **DR-95 Autoimmune Disease Label** | A treatment's autoimmune disease label is the disease label of the treatment's autoimmune disease. |
| **DR-96 Is Effective Treatment** | A treatment is considered an effective treatment if all of the following hold: at least one of the following holds: the treatment response is the literal “Complete” or the treatment response is the literal “Partial” and it is not the case that the has adverse effect flag is set. |
| **DR-97 Is Mechanism Matched** | A treatment's is mechanism matched is false if the targets mechanism is blank, otherwise the is causal architecture node of the treatment's targets mechanism. |
| **DR-98 Is Treatment Response Predicted** | A treatment is considered treatment response predicted if all of the following hold: the is effective treatment flag is set and the is mechanism matched flag is set. |
| **DR-99 Treatment Response Deciding Factor** | The treatment's treatment response deciding factor is determined by the following priority:<br>1. the literal “EffectiveOnConfirmedMechanism”, if the is treatment response predicted flag is set;<br>2. the literal “NoConfirmedMechanism”, if it is not the case that the is mechanism matched flag is set;<br>3. the literal “AdverseEffect”, if the has adverse effect flag is set;<br>4. the literal “NoResponse”, if at least one of the following holds: the treatment response is the literal “None” or the treatment response is the literal “Adverse”;<br>5. otherwise the literal “Undetermined”. |
| **DR-100 Name** | A clinical phenotype's name is computed as the phenotype label. |
| **DR-101 Parent Path** | A clinical phenotype's parent path is the relative path of the clinical phenotype's individual. |
| **DR-102 Relative Path** | A clinical phenotype's relative path is computed as the parent path, followed by the literal “/phenotypes/”, followed by the clinical phenotype ID. |
| **DR-103 Disease Stage Label** | The clinical phenotype's disease stage label is determined by the following priority:<br>1. an empty string, if the disease stage is blank;<br>2. otherwise the stage label of the clinical phenotype's disease stage. |
| **DR-104 Is High Severity** | A clinical phenotype is considered a high severity if the severity score is greater than 7. |
| **DR-105 Is Presymptomatic Phenotype** | A clinical phenotype is considered a presymptomatic phenotype if the disease stage label is the literal “Presymptomatic”. |
| **DR-106 Target Pathway Code** | The causal mechanism's target pathway code is determined by the following priority:<br>1. 1, if the target pathway is the literal “type-I-IFN”;<br>2. 2, if the target pathway is the literal “B-cell/autoantibody”;<br>3. 3, if the target pathway is the literal “T-cell-costim”;<br>4. 4, if the target pathway is the literal “IL-17/23”;<br>5. otherwise 0. |
| **DR-107 Name** | A causal mechanism's name is computed as the mechanism label. |
| **DR-108 Parent Path** | A causal mechanism's parent path is the relative path of the causal mechanism's individual. |
| **DR-109 Relative Path** | A causal mechanism's relative path is computed as the parent path, followed by the literal “/mechanisms/”, followed by the causal mechanism ID. |
| **DR-110 Individual Ancestry Label** | A causal mechanism's individual ancestry label is the ancestry label of the causal mechanism's individual. |
| **DR-111 Count Qualified Evidence** | A causal mechanism's count qualified evidence is the number of the causal mechanism's evidence items that are qualified evidences. |
| **DR-112 Count Modalities Supporting** | A causal mechanism's count modalities supporting is the number of the causal mechanism's evidence items that are cross modalities and are qualified evidences. |
| **DR-113 Count Intervention Targets** | A causal mechanism's count intervention targets is the number of intervention targets related to the causal mechanism. |
| **DR-114 Is Experimentally Falsifiable** | A causal mechanism is considered an experimentally falsifiable if all of the following hold: the count intervention targets is at least 1 and the count qualified evidence is at least 1. |
| **DR-115 Count Replications** | A causal mechanism's count replications is the number of cohort replications related to the causal mechanism. |
| **DR-116 Count Concordant Replications** | A causal mechanism's count concordant replications is the number of the causal mechanism's cohort replications that are replicated at nominal sig. |
| **DR-117 Count Cross Ancestry Concordant** | A causal mechanism's count cross ancestry concordant is the number of the causal mechanism's cohort replications that are cross ancestry concordants. |
| **DR-118 Replication Fraction** | The causal mechanism's replication fraction is determined by the following priority:<br>1. the count concordant replications divided by the count replications, if the count replications is greater than 0;<br>2. otherwise 0. |
| **DR-119 Replicates Across Cohorts** | A causal mechanism is considered to replicate across cohorts if all of the following hold: the count replications is at least 2 and the count concordant replications is at least 2. |
| **DR-120 Count Neg Control Tests** | A causal mechanism's count neg control tests is the number of negative control tests related to the causal mechanism. |
| **DR-121 Count Neg Control Survived** | A causal mechanism's count neg control survived is the number of the causal mechanism's negative control tests that are survived. |
| **DR-122 Survives Negative Controls** | A causal mechanism is considered to survive negative controls if all of the following hold: the count neg control tests is at least 1 and the count neg control survived is the count neg control tests. |
| **DR-123 Is Spurious Derived** | A causal mechanism is considered spurious derived if at least one of the following holds: it is not the case that the replicates across cohorts flag is set; it is not the case that the survives negative controls flag is set; the count modalities supporting is less than 2; or the has pleiotropy flag is set. |
| **DR-124 Causal Confidence** | The causal mechanism's causal confidence is determined by the following priority:<br>1. 1, if 0.30 times 1 if the count qualified evidence is at least 4, otherwise the count qualified evidence divided by 4 plus 0.20 times 1 if the count modalities supporting is at least 3, otherwise the count modalities supporting divided by 3 plus 0.30 times the replication fraction plus 0.20 times 1 if the survives negative controls flag is set, otherwise 0 is greater than 1;<br>2. otherwise 0.30 times 1 if the count qualified evidence is at least 4, otherwise the count qualified evidence divided by 4 plus 0.20 times 1 if the count modalities supporting is at least 3, otherwise the count modalities supporting divided by 3 plus 0.30 times the replication fraction plus 0.20 times 1 if the survives negative controls flag is set, otherwise 0. |
| **DR-125 Variant is Causal Candidate** | A causal mechanism's variant is causal candidate is false if the genomic variant is blank, otherwise the is causal candidate of the causal mechanism's genomic variant. |
| **DR-126 Is Causal Architecture Node** | A causal mechanism is considered a causal architecture node if all of the following hold: the causal confidence is at least 0.7; the is experimentally falsifiable flag is set; it is not the case that the is spurious derived flag is set; and at least one of the following holds: the variant is causal candidate flag is set or the environmental exposure has a value. |
| **DR-127 Is Ancestry Transportable** | A causal mechanism is considered an ancestry transportable if all of the following hold: the is causal architecture node flag is set and the count cross ancestry concordant is at least 1. |
| **DR-128 Name** | An epistatic interaction's name is computed as the interaction label. |
| **DR-129 Parent Path** | An epistatic interaction's parent path is the relative path of the epistatic interaction's individual. |
| **DR-130 Relative Path** | An epistatic interaction's relative path is computed as the parent path, followed by the literal “/epistasis/”, followed by the epistatic interaction ID. |
| **DR-131 Is High Order Epistasis** | An epistatic interaction is considered a high order epistasis if the epistasis score is greater than 0.5. |
| **DR-132 Name** | A counterfactual trajectory's name is computed as the trajectory label. |
| **DR-133 Parent Path** | A counterfactual trajectory's parent path is the relative path of the counterfactual trajectory's individual. |
| **DR-134 Relative Path** | A counterfactual trajectory's relative path is computed as the parent path, followed by the literal “/trajectories/”, followed by the counterfactual trajectory ID. |
| **DR-135 Autoimmune Disease Label** | A counterfactual trajectory's autoimmune disease label is the disease label of the counterfactual trajectory's autoimmune disease. |
| **DR-136 Is Worsening Trajectory** | A counterfactual trajectory is considered a worsening trajectory if the projected severity is greater than 7. |
| **DR-137 Name** | An individual prediction's name is computed as the prediction label. |
| **DR-138 Parent Path** | An individual prediction's parent path is the relative path of the individual prediction's individual. |
| **DR-139 Relative Path** | An individual prediction's relative path is computed as the parent path, followed by the literal “/predictions/”, followed by the individual prediction ID. |
| **DR-140 Individual Ancestry Label** | An individual prediction's individual ancestry label is the ancestry label of the individual prediction's individual. |
| **DR-141 Is Ancestry Holdout** | An individual prediction's is ancestry holdout is true when the individual prediction's individual is ancestry absent from training. |
| **DR-142 Individual Causal Mass** | An individual prediction's individual causal mass is 0 if the individual is blank, otherwise the sum confirmed causal confidence of the individual prediction's individual. |
| **DR-143 Individual Confirmed Node Count** | An individual prediction's individual confirmed node count is 0 if the individual is blank, otherwise the count confirmed causal nodes of the individual prediction's individual. |
| **DR-144 Individual Cross Ancestry Node Count** | An individual prediction's individual cross ancestry node count is 0 if the individual is blank, otherwise the count cross ancestry confirmed nodes of the individual prediction's individual. |
| **DR-145 Individual Has Cryptic Relatedness** | An individual prediction's individual has cryptic relatedness is false if the individual is blank, otherwise the has cryptic relatedness flag of the individual prediction's individual. |
| **DR-146 Individual Max Severity Score** | An individual prediction's individual max severity score is 0 if the individual is blank, otherwise the max severity score of the individual prediction's individual. |
| **DR-147 Individual Has High Severity Phenotype** | An individual prediction's individual has high severity phenotype is false if the individual is blank, otherwise the has high severity phenotype of the individual prediction's individual. |
| **DR-148 Individual Has Predicted Treatment Response** | An individual prediction's individual has predicted treatment response is false if the individual is blank, otherwise the has predicted treatment response of the individual prediction's individual. |
| **DR-149 Predicted Value** | The individual prediction's predicted value is determined by the following priority:<br>1. 10, if 2 times the individual causal mass plus 1.5 times the individual confirmed node count is greater than 10;<br>2. otherwise 2 times the individual causal mass plus 1.5 times the individual confirmed node count. |
| **DR-150 Count Bins** | An individual prediction's count bins is the number of calibration bins related to the individual prediction. |
| **DR-151 Count Well Calibrated Bins** | An individual prediction's count well calibrated bins is the number of the individual prediction's calibration bins that are well calibrated bins. |
| **DR-152 Sum Bin Abs Error** | An individual prediction's sum bin abs error is the total bin abs error across the calibration bins related to the individual prediction. |
| **DR-153 Mean Bin Abs Error** | The individual prediction's mean bin abs error is determined by the following priority:<br>1. the sum bin abs error divided by the count bins, if the count bins is greater than 0;<br>2. otherwise 1. |
| **DR-154 Well Calibrated Fraction** | The individual prediction's well calibrated fraction is determined by the following priority:<br>1. the count well calibrated bins divided by the count bins, if the count bins is greater than 0;<br>2. otherwise 0. |
| **DR-155 Calibrated Uncertainty** | An individual prediction's calibrated uncertainty is computed as 0 if 1 minus the mean bin abs error is less than 0, otherwise 1 minus the mean bin abs error times the well calibrated fraction. |
| **DR-156 Rests on Confirmed Mechanism** | An individual prediction is considered to rest on confirmed mechanism if the individual confirmed node count is at least 1. |
| **DR-157 Has Spurious Correlation Flag** | An individual prediction is considered to have a spurious correlation flag if at least one of the following holds: it is not the case that the rests on confirmed mechanism flag is set or the individual has cryptic relatedness flag is set. |
| **DR-158 Is Falsifiability Backed** | An individual prediction is considered falsifiability backed if the individual confirmed node count is at least 1. |
| **DR-159 Is Transportable to Absent Ancestry** | An individual prediction is considered a transportable to absent ancestry if all of the following hold: the is ancestry holdout flag is set; the individual cross ancestry node count is at least 1; and it is not the case that the has spurious correlation flag is set. |
| **DR-160 Is Ancestry Transport Safe** | An individual prediction is considered an ancestry transport safe if the is transportable to absent ancestry if the is ancestry holdout flag is set, otherwise true. |
| **DR-161 Transport Gate Status** | The individual prediction's transport gate status is determined by the following priority:<br>1. the literal “NotApplicable”, if it is not the case that the is ancestry holdout flag is set;<br>2. the literal “PASS-tested”, if the is transportable to absent ancestry flag is set;<br>3. otherwise the literal “FAIL”. |
| **DR-162 Is High Confidence Prediction** | An individual prediction is considered a high confidence prediction if all of the following hold: the calibrated uncertainty is at least 0.7 and it is not the case that the has spurious correlation flag is set. |
| **DR-163 Patient Stratification Tier** | The individual prediction's patient stratification tier is determined by the following priority:<br>1. the literal “High-Risk Pathway”, if the predicted value is at least 7;<br>2. the literal “Moderate-Risk Pathway”, if the predicted value is at least 4;<br>3. otherwise the literal “Low-Risk Pathway”. |
| **DR-164 Predicted Severity Value** | An individual prediction's predicted severity value is computed as the individual max severity score. |
| **DR-165 Severity Tier** | The individual prediction's severity tier is determined by the following priority:<br>1. the literal “Severe”, if the predicted severity value is greater than 7;<br>2. the literal “Moderate”, if the predicted severity value is at least 4;<br>3. otherwise the literal “Mild”. |
| **DR-166 Is Severity Actionable** | An individual prediction is considered a severity actionable if all of the following hold: the individual has high severity phenotype flag is set; the rests on confirmed mechanism flag is set; and it is not the case that the has spurious correlation flag is set. |
| **DR-167 Severity Deciding Factor** | The individual prediction's severity deciding factor is determined by the following priority:<br>1. the literal “HighSeverityOnConfirmedMechanism”, if the is severity actionable flag is set;<br>2. the literal “NotHighSeverity”, if it is not the case that the individual has high severity phenotype flag is set;<br>3. the literal “NoValidatedMechanism”, if it is not the case that the rests on confirmed mechanism flag is set;<br>4. the literal “SpuriousFlag”, if the has spurious correlation flag is set;<br>5. otherwise the literal “Undetermined”. |
| **DR-168 Is Treatment Response Actionable** | An individual prediction is considered a treatment response actionable if the individual has predicted treatment response flag is set. |
| **DR-169 Treatment Response Deciding Factor** | The individual prediction's treatment response deciding factor is determined by the following priority:<br>1. the literal “EffectiveOnConfirmedMechanism”, if the is treatment response actionable flag is set;<br>2. the literal “NoEffectiveTreatmentOnMechanism”, if the rests on confirmed mechanism flag is set;<br>3. otherwise the literal “NoConfirmedMechanism”. |
| **DR-170 Is Clinically Actionable** | An individual prediction is considered a clinically actionable if all of the following hold: the is high confidence prediction flag is set; the is falsifiability backed flag is set; the is ancestry transport safe flag is set; and the predicted value is greater than 0. |
| **DR-171 Lifecycle State Key** | The individual prediction's lifecycle state key is determined by the following priority:<br>1. the literal “Actionable”, if all of the following hold: the is high confidence prediction flag is set; the is falsifiability backed flag is set; the is ancestry transport safe flag is set; and the predicted value is greater than 0;<br>2. the literal “NotActionable”, if at least one of the following holds: it is not the case that the rests on confirmed mechanism flag is set or it is not the case that the is falsifiability backed flag is set;<br>3. the literal “NotActionable”, if the individual has cryptic relatedness flag is set;<br>4. the literal “NotActionable”, if the calibrated uncertainty is less than 0.7;<br>5. the literal “NotActionable”, if it is not the case that the is ancestry transport safe flag is set;<br>6. otherwise the literal “Actionable”. |
| **DR-172 Deciding Gate** | The individual prediction's deciding gate is determined by the following priority:<br>1. the literal “AllGatesPass”, if the is clinically actionable flag is set;<br>2. the literal “NoValidatedMechanism”, if it is not the case that the rests on confirmed mechanism flag is set;<br>3. the literal “CrypticRelatedness”, if the individual has cryptic relatedness flag is set;<br>4. the literal “Calibration”, if the calibrated uncertainty is less than 0.7;<br>5. the literal “AncestryTransport”, if it is not the case that the is ancestry transport safe flag is set;<br>6. otherwise the literal “Undetermined”. |
| **DR-173 Individual Target Pathway** | An individual prediction's individual target pathway is the target pathway of the individual prediction's individual. |
| **DR-174 Individual Progression State Key** | An individual prediction's individual progression state key is the nephritis progression state key of the individual prediction's individual. |
| **DR-175 Individual is Disease Progressing** | An individual prediction's individual is disease progressing is true when the individual prediction's individual is disease progressing. |
| **DR-176 Recommended Treatment Line** | The individual prediction's recommended treatment line is determined by the following priority:<br>1. the literal “No targeted line — mechanism unconfirmed”, if it is not the case that the rests on confirmed mechanism flag is set;<br>2. the literal “Mycophenolate (induction)”, if at least one of the following holds: the individual progression state key is the literal “RenalFlareRisk” or the individual progression state key is the literal “BiopsyIndicated”;<br>3. the literal “Anifrolumab”, if the individual target pathway is the literal “type-I-IFN”;<br>4. the literal “Belimumab”, if the individual target pathway is the literal “B-cell/autoantibody”;<br>5. the literal “Secukinumab”, if the individual target pathway is the literal “IL-17/23”;<br>6. otherwise the literal “Standard-of-care (no mechanism-matched targeted line)”. |
| **DR-177 Treatment Line Deciding Factor** | The individual prediction's treatment line deciding factor is determined by the following priority:<br>1. the literal “MechanismUnconfirmed”, if it is not the case that the rests on confirmed mechanism flag is set;<br>2. the literal “ActiveNephritis-Induction”, if at least one of the following holds: the individual progression state key is the literal “RenalFlareRisk” or the individual progression state key is the literal “BiopsyIndicated”;<br>3. the literal “IFNSignature-Anifrolumab”, if the individual target pathway is the literal “type-I-IFN”;<br>4. the literal “AutoantibodyDriven-Belimumab”, if the individual target pathway is the literal “B-cell/autoantibody”;<br>5. the literal “IL17Axis-Secukinumab”, if the individual target pathway is the literal “IL-17/23”;<br>6. otherwise the literal “NoMechanismMatch”. |
| **DR-178 Progression Vs Actionability Disagree** | An individual prediction is flagged progression vs actionability disagree if all of the following hold: the individual is disease progressing flag is set and it is not the case that the is clinically actionable flag is set. |
| **DR-179 Name** | A calibration bin's name is computed as the bin label. |
| **DR-180 Parent Path** | A calibration bin's parent path is the relative path of the calibration bin's individual prediction. |
| **DR-181 Relative Path** | A calibration bin's relative path is computed as the parent path, followed by the literal “/bins/”, followed by the calibration bin ID. |
| **DR-182 Bin Abs Error** | The calibration bin's bin abs error is determined by the following priority:<br>1. the predicted probability band minus the observed event rate, if the predicted probability band is at least the observed event rate;<br>2. otherwise the observed event rate minus the predicted probability band. |
| **DR-183 Is Well Calibrated Bin** | A calibration bin is considered a well calibrated bin if all of the following hold: the coverage count is at least 20 and the bin abs error is at most 0.1. |
| **DR-184 Name** | An intervention target's name is computed as the target label. |
| **DR-185 Parent Path** | An intervention target's parent path is the relative path of the intervention target's causal mechanism. |
| **DR-186 Relative Path** | An intervention target's relative path is computed as the parent path, followed by the literal “/targets/”, followed by the intervention target ID. |
| **DR-187 Causal Mechanism Label** | An intervention target's causal mechanism label is the mechanism label of the intervention target's causal mechanism. |
| **DR-188 Is Gene Based Therapy** | An intervention target is considered a gene based therapy if the therapy class is the literal “Gene-based”. |
| **DR-189 Is Cell Based Therapy** | An intervention target is considered a cell based therapy if the therapy class is the literal “Cell-based”. |
| **DR-190 Name** | An axiom's name is computed as the statement. |
| **DR-191 Relative Path** | An axiom's relative path is computed as the literal “/admin/axioms/”, followed by the axiom ID. |
| **DR-192 Name** | A tests for success's name is computed as the claim. |
| **DR-193 Relative Path** | A tests for success's relative path is computed as the literal “/admin/tests-for-success/”, followed by the test for success ID. |
| **DR-194 Name** | A feature's name is computed as the title. |
| **DR-195 Relative Path** | A feature's relative path is computed as the literal “/admin/features/”, followed by the feature ID. |
| **DR-196 Meta Line** | A feature's meta line is computed as the literal “**Category:** ”, followed by the category, followed by the literal “ - **Priority:** ”, followed by the priority, followed by the literal “ - **Challenge refs:** ”, followed by the ref count. |
| **DR-197 Name** | An inference kind's name is computed as the title. |
| **DR-198 Relative Path** | An inference kind's relative path is computed as the literal “/admin/inference-kinds/”, followed by the inference kind ID. |
| **DR-199 Name** | An open question's name is computed as the question. |
| **DR-200 Relative Path** | An open question's relative path is computed as the literal “/admin/open-questions/”, followed by the open question ID. |
| **DR-201 Name** | A non goal's name is computed as the statement. |
| **DR-202 Relative Path** | A non goal's relative path is computed as the literal “/admin/non-goals/”, followed by the non goal ID. |
| **DR-203 Name** | A glossary term's name is computed as the term. |
| **DR-204 Relative Path** | A glossary term's relative path is computed as the literal “/admin/glossary/”, followed by the glossary term ID. |
| **DR-205 Name** | A leopold loop's name is computed as the literal “Loop ”, followed by the loop number, followed by the literal “ — ”, followed by the title. |
| **DR-206 Relative Path** | A leopold loop's relative path is computed as the literal “/admin/leopold-loops/”, followed by the leopold loop ID. |
| **DR-207 Completedness** | A leopold loop's completedness is computed as the status. |
| **DR-208 Is in Current Plan** | A leopold loop is considered in current plan if it is not the case that the status is the literal “done”. |
| **DR-209 Name** | A routing and navigation's name is computed as the lower-cased display name with every a space replaced by a hyphen. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-210 Admin Can Create** | A routing and navigation is flagged admin can create if the admin CRUD mentions the literal “C”. |
| **DR-211 Admin Can Read** | A routing and navigation is flagged admin can read if the admin CRUD mentions the literal “R”. |
| **DR-212 Admin Can Update** | A routing and navigation is flagged admin can update if the admin CRUD mentions the literal “U”. |
| **DR-213 Admin Can Delete** | A routing and navigation is flagged admin can delete if the admin CRUD mentions the literal “D”. |
| **DR-214 Intake Clinician Can Create** | A routing and navigation is flagged intake clinician can create if the intake clinician CRUD mentions the literal “C”. |
| **DR-215 Intake Clinician Can Read** | A routing and navigation is flagged intake clinician can read if the intake clinician CRUD mentions the literal “R”. |
| **DR-216 Intake Clinician Can Update** | A routing and navigation is flagged intake clinician can update if the intake clinician CRUD mentions the literal “U”. |
| **DR-217 Intake Clinician Can Delete** | A routing and navigation is flagged intake clinician can delete if the intake clinician CRUD mentions the literal “D”. |
| **DR-218 Diagnosing Doctor Can Create** | A routing and navigation is flagged diagnosing doctor can create if the diagnosing doctor CRUD mentions the literal “C”. |
| **DR-219 Diagnosing Doctor Can Read** | A routing and navigation is flagged diagnosing doctor can read if the diagnosing doctor CRUD mentions the literal “R”. |
| **DR-220 Diagnosing Doctor Can Update** | A routing and navigation is flagged diagnosing doctor can update if the diagnosing doctor CRUD mentions the literal “U”. |
| **DR-221 Diagnosing Doctor Can Delete** | A routing and navigation is flagged diagnosing doctor can delete if the diagnosing doctor CRUD mentions the literal “D”. |
| **DR-222 External Llm Can Create** | A routing and navigation is flagged external llm can create if the external llm CRUD mentions the literal “C”. |
| **DR-223 External Llm Can Read** | A routing and navigation is flagged external llm can read if the external llm CRUD mentions the literal “R”. |
| **DR-224 External Llm Can Update** | A routing and navigation is flagged external llm can update if the external llm CRUD mentions the literal “U”. |
| **DR-225 External Llm Can Delete** | A routing and navigation is flagged external llm can delete if the external llm CRUD mentions the literal “D”. |
| **DR-226 Depth** | The routing and navigation's depth is determined by the following priority:<br>1. 0, if the parent route key is blank;<br>2. otherwise the length of the route key minus the length of the route key with every a period replaced by an empty string. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-227 Full Path** | A routing and navigation's full path is computed as the route. |
| **DR-228 Handler Base Name** | A routing and navigation's handler base name is computed as the route key with every a period replaced by a space with every a hyphen replaced by a space. ⚠︎ mechanical <!-- rulespeak:reword --> |
| **DR-229 Relative Path** | A routing and navigation's relative path is computed as the literal “/admin/routing/”, followed by the routing and navigation ID. |
| **DR-230 Relative Path** | A state machine's relative path is computed as the literal “/admin/state-machine/”, followed by the state machine ID. |
| **DR-231 State Count** | A state machine's state count is the number of machine states related to the state machine. |
| **DR-232 Transition Rule Count** | A state machine's transition rule count is the number of state transition rules related to the state machine. |
| **DR-233 Relative Path** | A machine state's relative path is computed as the literal “/admin/state-machine/states/”, followed by the machine state ID. |
| **DR-234 Reachable State Count** | A machine state's reachable state count is the number of vw state transition rules closure related to the machine state. |
| **DR-235 Relative Path** | A state transition rule's relative path is computed as the literal “/admin/state-machine/rules/”, followed by the state transition rule ID. |
| **DR-236 From State Key** | A state transition rule's from state key is the state key of the state transition rule's from state. |
| **DR-237 To State Key** | A state transition rule's to state key is the state key of the state transition rule's to state. |
| **DR-238 Is Forward Edge** | A state transition rule is considered a forward edge if it is not the case that the to state key is the from state key. |
| **DR-239 Relative Path** | A state transition's relative path is computed as the literal “/admin/state-machine/transitions/”, followed by the state transition ID. |
| **DR-240 Is Forward** | A state transition is considered a forward if it is not the case that the to state key is the literal “Intake”. |
| **DR-241 Relative Path** | A subject state instance's relative path is computed as the literal “/admin/state-machine/instances/”, followed by the subject state instance ID. |
| **DR-242 Is Current** | A subject state instance is considered a current if the exited at is blank. |
| **DR-243 Has Complete Lineage** | A subject state instance is considered to have a complete lineage if the sequence index is at least 1. |
| **DR-244 Is Long Dwell** | A subject state instance is considered a long dwell if the dwell days is at least 90. |
| **DR-245 Name** | A disease domain concept's name is computed as the concept label. |
| **DR-246 Relative Path** | A disease domain concept's relative path is computed as the literal “/admin/disease-concepts/”, followed by the disease domain concept ID. |
| **DR-247 Is Deeply Modeled** | A disease domain concept is considered deeply modeled if the modeling status is the literal “deep-dag”. |
| **DR-248 Is Schema Modeled** | A disease domain concept is considered schema modeled if at least one of the following holds: the modeling status is the literal “deep-dag” or the modeling status is the literal “schema”. |
| **DR-249 Prior Anti Ds Dna IU** | A serology observation's prior anti ds dna IU is the anti ds dna IU of the serology observation's prior observation. |
| **DR-250 Prior C3** | A serology observation's prior c3 is the complement c3 of the serology observation's prior observation. |
| **DR-251 Prior C4** | A serology observation's prior c4 is the complement c4 of the serology observation's prior observation. |
| **DR-252 Anti Ds Dna Trend** | The serology observation's anti ds dna trend is determined by the following priority:<br>1. the literal “Stable”, if the prior anti ds dna IU is blank;<br>2. the literal “Rising”, if the anti ds dna IU is greater than the prior anti ds dna IU times 1.25;<br>3. the literal “Falling”, if the anti ds dna IU is less than the prior anti ds dna IU times 0.8;<br>4. otherwise the literal “Stable”. |
| **DR-253 Complement Trend** | The serology observation's complement trend is determined by the following priority:<br>1. the literal “Stable”, if the prior c3 is blank;<br>2. the literal “Falling”, if the complement c3 plus the complement c4 is less than the prior c3 plus the prior c4 times 0.85;<br>3. the literal “Rising”, if the complement c3 plus the complement c4 is greater than the prior c3 plus the prior c4 times 1.15;<br>4. otherwise the literal “Stable”. |
| **DR-254 Is Pre Nephritic Signature Panel** | A serology observation is considered a pre nephritic signature panel if all of the following hold: the anti ds dna trend is the literal “Rising” and the complement trend is the literal “Falling”. |
| **DR-255 Is Significant Proteinuria** | A serology observation is considered a significant proteinuria if the proteinuria g per day is at least 0.5. |
| **DR-256 Is Nephrotic Range Proteinuria** | A serology observation is considered a nephrotic range proteinuria if the proteinuria g per day is at least 3.0. |
| **DR-257 Sledai Renal Points** | The serology observation's sledai renal points is determined by the following priority:<br>1. 8, if at least one of the following holds: the is nephrotic range proteinuria flag is set or the has active urinary sediment flag is set;<br>2. 4, if the is significant proteinuria flag is set;<br>3. otherwise 0. |
| **DR-258 Sledai Serology Points** | The serology observation's sledai serology points is determined by the following priority:<br>1. 4, if all of the following hold: the complement trend is the literal “Falling” and the anti ds dna trend is the literal “Rising”;<br>2. 2, if at least one of the following holds: the complement trend is the literal “Falling” or the anti ds dna trend is the literal “Rising”;<br>3. otherwise 0. |
| **DR-259 Sledai Score** | A serology observation's sledai score is computed as the sledai renal points plus the sledai serology points. |
| **DR-260 Progression State Key** | The serology observation's progression state key is determined by the following priority:<br>1. the literal “BiopsyIndicated”, if at least one of the following holds: the is nephrotic range proteinuria flag is set or the has active urinary sediment flag is set;<br>2. the literal “RenalFlareRisk”, if the proteinuria g per day is at least 1.0;<br>3. the literal “EarlyNephritis”, if the is significant proteinuria flag is set;<br>4. the literal “SerologicActive”, if all of the following hold: the anti ds dna trend is the literal “Rising” and the complement trend is the literal “Falling”;<br>5. otherwise the literal “PresymptomaticAutoimmunity”. |
| **DR-261 Progression State Order** | The serology observation's progression state order is determined by the following priority:<br>1. 5, if the progression state key is the literal “BiopsyIndicated”;<br>2. 4, if the progression state key is the literal “RenalFlareRisk”;<br>3. 3, if the progression state key is the literal “EarlyNephritis”;<br>4. 2, if the progression state key is the literal “SerologicActive”;<br>5. otherwise 1. |
| **DR-262 Relative Path** | A serology observation's relative path is computed as the literal “/admin/serology/”, followed by the serology observation ID. |
| **DR-263 Name** | A therapy option's name is computed as the therapy label. |
| **DR-264 Relative Path** | A therapy option's relative path is computed as the literal “/admin/therapy-options/”, followed by the therapy option ID. |

## 5 Traceability to Schema

_The expression column is the rule's definition in RuleSpeak notation —
the same logic the rulebook stores, written for a business reader._

| Schema element | Kind | Expression |
|----------------|------|------------|
| **AutoimmuneDiseases.Name** | formula | `DiseaseLabel` |
| **AutoimmuneDiseases.RelativePath** | formula | `"/diseases/" & AutoimmuneDiseaseId` |
| **AutoimmuneDiseases.CountOfDiseaseStages** | rollup | `Count(DiseaseStages via AutoimmuneDisease)` |
| **AutoimmuneDiseases.CountOfInterventionTargets** | rollup | `Count(InterventionTargets via AutoimmuneDisease)` |
| **DiseaseStages.Name** | formula | `AutoimmuneDiseaseDiseaseLabel & " — " & StageLabel` |
| **DiseaseStages.ParentPath** | lookup | `Lookup(AutoimmuneDiseases.RelativePath via AutoimmuneDisease)` |
| **DiseaseStages.RelativePath** | formula | `ParentPath & "/stages/" & DiseaseStageId` |
| **DiseaseStages.AutoimmuneDiseaseDiseaseLabel** | formula | `Lookup(AutoimmuneDiseases.DiseaseLabel via AutoimmuneDisease)` |
| **DiseaseStages.IsPresymptomatic** | formula | `If(StageLabel = "Presymptomatic", True(), False())` |
| **Tissues.Name** | formula | `TissueLabel` |
| **Tissues.RelativePath** | formula | `"/tissues/" & TissueId` |
| **Tissues.CountOfOmicsAssays** | rollup | `Count(OmicsAssays via Tissue)` |
| **OmicsModalities.Name** | formula | `ModalityLabel` |
| **OmicsModalities.RelativePath** | formula | `"/omics-modalities/" & OmicsModalityId` |
| **OmicsModalities.CountOfOmicsAssays** | rollup | `Count(OmicsAssays via OmicsModality)` |
| **FederatedDatasets.Name** | formula | `NodeLabel` |
| **FederatedDatasets.RelativePath** | formula | `"/datasets/" & FederatedDatasetId` |
| **FederatedDatasets.CountOfIndividuals** | rollup | `Count(Individuals via FederatedDataset)` |
| **VariantTypes.Name** | formula | `TypeLabel` |
| **VariantTypes.RelativePath** | formula | `"/variant-types/" & VariantTypeId` |
| **VariantTypes.CountOfGenomicVariants** | rollup | `Count(GenomicVariants via VariantType)` |
| **Individuals.Name** | formula | `GivenName & " " & FamilyName` |
| **Individuals.Slug** | formula | `Replace(Lower(FamilyName & "-" & GivenName), " ", "-")` |
| **Individuals.RelativePath** | formula | `"/intake/new-patient/" & Slug` |
| **Individuals.FederatedDatasetNodeLabel** | formula | `If(FederatedDataset = "", "", Lookup(FederatedDatasets.NodeLabel via FederatedDataset))` |
| **Individuals.CountOfGenomicVariants** | rollup | `Count(GenomicVariants via Individual)` |
| **Individuals.CountOfCausalMechanisms** | rollup | `Count(CausalMechanisms via Individual)` |
| **Individuals.CountOfEpistaticInteractions** | rollup | `Count(EpistaticInteractions via Individual)` |
| **Individuals.RareVariantBurdenScore** | formula | `If(AgeYears > 0, CountOfGenomicVariants / AgeYears, 0)` |
| **Individuals.CausalArchitectureScore** | formula | `CountOfCausalMechanisms * 10 + CountOfEpistaticInteractions * 5 + RareVariantBurdenScore` |
| **Individuals.IsDevelopmentWindow** | formula | `If(AgeYears <= 25, True(), False())` |
| **Individuals.IsAgingWindow** | formula | `If(AgeYears >= 60, True(), False())` |
| **Individuals.CountConfirmedCausalNodes** | rollup | `Count(CausalMechanisms via Individual)` |
| **Individuals.SumConfirmedCausalConfidence** | rollup | `Sum(CausalMechanisms.CausalConfidence via Individual)` |
| **Individuals.CountCrossAncestryConfirmedNodes** | rollup | `Count(CausalMechanisms via Individual)` |
| **Individuals.MaxSeverityScore** | rollup | `Max(ClinicalPhenotypes.SeverityScore via Individual)` |
| **Individuals.CountHighSeverityPhenotypes** | rollup | `Count(ClinicalPhenotypes via Individual)` |
| **Individuals.HasHighSeverityPhenotype** | formula | `If(CountHighSeverityPhenotypes >= 1, True(), False())` |
| **Individuals.CountPredictedTreatmentResponses** | rollup | `Count(Treatments via Individual)` |
| **Individuals.HasPredictedTreatmentResponse** | formula | `If(CountPredictedTreatmentResponses >= 1, True(), False())` |
| **Individuals.CountSerologyPanels** | rollup | `Count(SerologyObservations via Individual)` |
| **Individuals.CountPreNephriticSignaturePanels** | rollup | `Count(SerologyObservations via Individual)` |
| **Individuals.IsInPreNephriticSignatureCluster** | formula | `If(CountPreNephriticSignaturePanels >= 1, True(), False())` |
| **Individuals.SignatureStrength** | formula | `If(CountPreNephriticSignaturePanels >= 2, 2, If(CountPreNephriticSignaturePanels >= 1, 1, 0))` |
| **Individuals.MaxProgressionStateOrder** | rollup | `Max(SerologyObservations.ProgressionStateOrder via Individual)` |
| **Individuals.LatestSledaiScore** | rollup | `Max(SerologyObservations.SledaiScore via Individual)` |
| **Individuals.NephritisProgressionStateKey** | formula | `If(MaxProgressionStateOrder >= 5, "BiopsyIndicated", If(MaxProgressionStateOrder >= 4, "RenalFlareRisk", If(MaxProgressionStateOrder >= 3, "EarlyNephritis", If(MaxProgressionStateOrder >= 2, "SerologicActive", "PresymptomaticAutoimmunity"))))` |
| **Individuals.ActivityTier** | formula | `If(LatestSledaiScore >= 12, "High / flare", If(LatestSledaiScore >= 6, "Moderate", If(LatestSledaiScore >= 1, "Mild", "Quiescent")))` |
| **Individuals.IsHighDiseaseActivity** | formula | `If(LatestSledaiScore >= 12, True(), False())` |
| **Individuals.IsDiseaseProgressing** | formula | `If(Or(NephritisProgressionStateKey = "EarlyNephritis", NephritisProgressionStateKey = "RenalFlareRisk", NephritisProgressionStateKey = "BiopsyIndicated"), True(), False())` |
| **Individuals.TargetPathwayCode** | rollup | `Max(CausalMechanisms.TargetPathwayCode via Individual)` |
| **Individuals.TargetPathway** | formula | `If(TargetPathwayCode = 1, "type-I-IFN", If(TargetPathwayCode = 2, "B-cell/autoantibody", If(TargetPathwayCode = 3, "T-cell-costim", If(TargetPathwayCode = 4, "IL-17/23", ""))))` |
| **Individuals.ReachableStatesAhead** | lookup | `Lookup(MachineStates.ReachableStateCount via CurrentProgressionStateId)` |
| **GenomicVariants.Name** | formula | `VariantLabel` |
| **GenomicVariants.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **GenomicVariants.RelativePath** | formula | `ParentPath & "/variants/" & GenomicVariantId` |
| **GenomicVariants.VariantTypeLabel** | formula | `Lookup(VariantTypes.TypeLabel via VariantType)` |
| **GenomicVariants.VariantClassIsRare** | lookup | `Lookup(VariantTypes.IsRareVariantClass via VariantType)` |
| **GenomicVariants.IndividualAncestryLabel** | formula | `Lookup(Individuals.AncestryLabel via Individual)` |
| **GenomicVariants.IsRareVariant** | formula | `If(AlleleFrequency < 0.01, True(), False())` |
| **GenomicVariants.IsCausalCandidate** | formula | `If(And(Or(IsRareVariant, VariantClassIsRare), HasAlleleSpecificExpression), True(), False())` |
| **OmicsAssays.Name** | formula | `AssayLabel` |
| **OmicsAssays.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **OmicsAssays.RelativePath** | formula | `ParentPath & "/assays/" & OmicsAssayId` |
| **OmicsAssays.ModalityLabel** | formula | `Lookup(OmicsModalities.ModalityLabel via OmicsModality)` |
| **OmicsAssays.TissueLabel** | formula | `If(Tissue = "", "Missing Tissue", Lookup(Tissues.TissueLabel via Tissue))` |
| **OmicsAssays.HasBatchEffectRisk** | formula | `If(MeasurementErrorScore > 0.3, True(), False())` |
| **OmicsAssays.IsHighQualityAssay** | formula | `If(And(Not(HasBatchEffectRisk), MeasurementErrorScore < 0.15), True(), False())` |
| **EvidenceItems.Name** | formula | `EvidenceLabel` |
| **EvidenceItems.ParentPath** | lookup | `Lookup(CausalMechanisms.RelativePath via CausalMechanism)` |
| **EvidenceItems.RelativePath** | formula | `ParentPath & "/evidence/" & EvidenceItemId` |
| **EvidenceItems.AssayIsHighQuality** | lookup | `Lookup(OmicsAssays.IsHighQualityAssay via OmicsAssay)` |
| **EvidenceItems.ZStat** | formula | `If(StandardError > 0, EffectSize / StandardError, 0)` |
| **EvidenceItems.IsConfoundControlled** | formula | `If(And(IsAdjustedForAncestryPCs, IsAdjustedForBatch), True(), False())` |
| **EvidenceItems.IsQualifiedEvidence** | formula | `If(And(AssayIsHighQuality, Not(IsNegativeControlArm), ZStat >= 2, IsConfoundControlled), True(), False())` |
| **CohortReplications.Name** | formula | `ReplicationLabel` |
| **CohortReplications.ParentPath** | lookup | `Lookup(CausalMechanisms.RelativePath via CausalMechanism)` |
| **CohortReplications.RelativePath** | formula | `ParentPath & "/replications/" & CohortReplicationId` |
| **CohortReplications.ReplicatedAtNominalSig** | formula | `If(And(ReplicationPValue <= 0.05, ReplicationEffectSign = 1), True(), False())` |
| **CohortReplications.MechanismPrimaryAncestry** | lookup | `Lookup(CausalMechanisms.IndividualAncestryLabel via CausalMechanism)` |
| **CohortReplications.IsDifferentAncestryReplication** | formula | `If(ReplicationAncestryLabel = MechanismPrimaryAncestry, False(), True())` |
| **CohortReplications.IsCrossAncestryConcordant** | formula | `If(And(ReplicatedAtNominalSig, IsDifferentAncestryReplication), True(), False())` |
| **NegativeControlTests.Name** | formula | `ControlLabel` |
| **NegativeControlTests.ParentPath** | lookup | `Lookup(CausalMechanisms.RelativePath via CausalMechanism)` |
| **NegativeControlTests.RelativePath** | formula | `ParentPath & "/neg-controls/" & NegativeControlTestId` |
| **NegativeControlTests.IsSurvived** | formula | `If(PermutationEffectSize <= NullThreshold, True(), False())` |
| **EnvironmentalExposures.Name** | formula | `ExposureLabel` |
| **EnvironmentalExposures.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **EnvironmentalExposures.RelativePath** | formula | `ParentPath & "/exposures/" & EnvironmentalExposureId` |
| **EnvironmentalExposures.IndividualAncestryLabel** | formula | `Lookup(Individuals.AncestryLabel via Individual)` |
| **EnvironmentalExposures.IsHighExposure** | formula | `If(ExposureLevel > 5, True(), False())` |
| **Treatments.Name** | formula | `TreatmentLabel` |
| **Treatments.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **Treatments.RelativePath** | formula | `ParentPath & "/treatments/" & TreatmentId` |
| **Treatments.AutoimmuneDiseaseLabel** | formula | `Lookup(AutoimmuneDiseases.DiseaseLabel via AutoimmuneDisease)` |
| **Treatments.IsEffectiveTreatment** | formula | `If(And(Or(TreatmentResponse = "Complete", TreatmentResponse = "Partial"), Not(HasAdverseEffect)), True(), False())` |
| **Treatments.IsMechanismMatched** | lookup | `If(TargetsMechanism = "", False(), Lookup(CausalMechanisms.IsCausalArchitectureNode via TargetsMechanism))` |
| **Treatments.IsTreatmentResponsePredicted** | formula | `If(And(IsEffectiveTreatment, IsMechanismMatched), True(), False())` |
| **Treatments.TreatmentResponseDecidingFactor** | formula | `If(IsTreatmentResponsePredicted, "EffectiveOnConfirmedMechanism", If(Not(IsMechanismMatched), "NoConfirmedMechanism", If(HasAdverseEffect, "AdverseEffect", If(Or(TreatmentResponse = "None", TreatmentResponse = "Adverse"), "NoResponse", "Undetermined"))))` |
| **ClinicalPhenotypes.Name** | formula | `PhenotypeLabel` |
| **ClinicalPhenotypes.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **ClinicalPhenotypes.RelativePath** | formula | `ParentPath & "/phenotypes/" & ClinicalPhenotypeId` |
| **ClinicalPhenotypes.DiseaseStageLabel** | formula | `If(DiseaseStage = "", "", Lookup(DiseaseStages.StageLabel via DiseaseStage))` |
| **ClinicalPhenotypes.IsHighSeverity** | formula | `If(SeverityScore > 7, True(), False())` |
| **ClinicalPhenotypes.IsPresymptomaticPhenotype** | formula | `If(DiseaseStageLabel = "Presymptomatic", True(), False())` |
| **CausalMechanisms.TargetPathwayCode** | formula | `If(TargetPathway = "type-I-IFN", 1, If(TargetPathway = "B-cell/autoantibody", 2, If(TargetPathway = "T-cell-costim", 3, If(TargetPathway = "IL-17/23", 4, 0))))` |
| **CausalMechanisms.Name** | formula | `MechanismLabel` |
| **CausalMechanisms.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **CausalMechanisms.RelativePath** | formula | `ParentPath & "/mechanisms/" & CausalMechanismId` |
| **CausalMechanisms.IndividualAncestryLabel** | formula | `Lookup(Individuals.AncestryLabel via Individual)` |
| **CausalMechanisms.CountQualifiedEvidence** | rollup | `Count(EvidenceItems via CausalMechanism)` |
| **CausalMechanisms.CountModalitiesSupporting** | rollup | `Count(EvidenceItems via CausalMechanism)` |
| **CausalMechanisms.CountInterventionTargets** | rollup | `Count(InterventionTargets via CausalMechanism)` |
| **CausalMechanisms.IsExperimentallyFalsifiable** | formula | `If(And(CountInterventionTargets >= 1, CountQualifiedEvidence >= 1), True(), False())` |
| **CausalMechanisms.CountReplications** | rollup | `Count(CohortReplications via CausalMechanism)` |
| **CausalMechanisms.CountConcordantReplications** | rollup | `Count(CohortReplications via CausalMechanism)` |
| **CausalMechanisms.CountCrossAncestryConcordant** | rollup | `Count(CohortReplications via CausalMechanism)` |
| **CausalMechanisms.ReplicationFraction** | formula | `If(CountReplications > 0, CountConcordantReplications / CountReplications, 0)` |
| **CausalMechanisms.ReplicatesAcrossCohorts** | formula | `If(And(CountReplications >= 2, CountConcordantReplications >= 2), True(), False())` |
| **CausalMechanisms.CountNegControlTests** | rollup | `Count(NegativeControlTests via CausalMechanism)` |
| **CausalMechanisms.CountNegControlSurvived** | rollup | `Count(NegativeControlTests via CausalMechanism)` |
| **CausalMechanisms.SurvivesNegativeControls** | formula | `If(And(CountNegControlTests >= 1, CountNegControlSurvived = CountNegControlTests), True(), False())` |
| **CausalMechanisms.IsSpuriousDerived** | formula | `If(Or(Not(ReplicatesAcrossCohorts), Not(SurvivesNegativeControls), CountModalitiesSupporting < 2, HasPleiotropy), True(), False())` |
| **CausalMechanisms.CausalConfidence** | formula | `If(0.30 * If(CountQualifiedEvidence >= 4, 1, CountQualifiedEvidence / 4) + 0.20 * If(CountModalitiesSupporting >= 3, 1, CountModalitiesSupporting / 3) + 0.30 * ReplicationFraction + 0.20 * If(SurvivesNegativeControls, 1, 0) > 1, 1, 0.30 * If(CountQualifiedEvidence >= 4, 1, CountQualifiedEvidence / 4) + 0.20 * If(CountModalitiesSupporting >= 3, 1, CountModalitiesSupporting / 3) + 0.30 * ReplicationFraction + 0.20 * If(SurvivesNegativeControls, 1, 0))` |
| **CausalMechanisms.VariantIsCausalCandidate** | lookup | `If(GenomicVariant = "", False(), Lookup(GenomicVariants.IsCausalCandidate via GenomicVariant))` |
| **CausalMechanisms.IsCausalArchitectureNode** | formula | `If(And(CausalConfidence >= 0.7, IsExperimentallyFalsifiable, Not(IsSpuriousDerived), Or(VariantIsCausalCandidate, EnvironmentalExposure <> "")), True(), False())` |
| **CausalMechanisms.IsAncestryTransportable** | formula | `If(And(IsCausalArchitectureNode, CountCrossAncestryConcordant >= 1), True(), False())` |
| **EpistaticInteractions.Name** | formula | `InteractionLabel` |
| **EpistaticInteractions.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **EpistaticInteractions.RelativePath** | formula | `ParentPath & "/epistasis/" & EpistaticInteractionId` |
| **EpistaticInteractions.IsHighOrderEpistasis** | formula | `If(EpistasisScore > 0.5, True(), False())` |
| **CounterfactualTrajectories.Name** | formula | `TrajectoryLabel` |
| **CounterfactualTrajectories.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **CounterfactualTrajectories.RelativePath** | formula | `ParentPath & "/trajectories/" & CounterfactualTrajectoryId` |
| **CounterfactualTrajectories.AutoimmuneDiseaseLabel** | formula | `Lookup(AutoimmuneDiseases.DiseaseLabel via AutoimmuneDisease)` |
| **CounterfactualTrajectories.IsWorseningTrajectory** | formula | `If(ProjectedSeverity > 7, True(), False())` |
| **IndividualPredictions.Name** | formula | `PredictionLabel` |
| **IndividualPredictions.ParentPath** | lookup | `Lookup(Individuals.RelativePath via Individual)` |
| **IndividualPredictions.RelativePath** | formula | `ParentPath & "/predictions/" & IndividualPredictionId` |
| **IndividualPredictions.IndividualAncestryLabel** | formula | `Lookup(Individuals.AncestryLabel via Individual)` |
| **IndividualPredictions.IsAncestryHoldout** | formula | `Lookup(Individuals.IsAncestryAbsentFromTraining via Individual)` |
| **IndividualPredictions.IndividualCausalMass** | lookup | `If(Individual = "", 0, Lookup(Individuals.SumConfirmedCausalConfidence via Individual))` |
| **IndividualPredictions.IndividualConfirmedNodeCount** | lookup | `If(Individual = "", 0, Lookup(Individuals.CountConfirmedCausalNodes via Individual))` |
| **IndividualPredictions.IndividualCrossAncestryNodeCount** | lookup | `If(Individual = "", 0, Lookup(Individuals.CountCrossAncestryConfirmedNodes via Individual))` |
| **IndividualPredictions.IndividualHasCrypticRelatedness** | lookup | `If(Individual = "", False(), Lookup(Individuals.HasCrypticRelatednessFlag via Individual))` |
| **IndividualPredictions.IndividualMaxSeverityScore** | lookup | `If(Individual = "", 0, Lookup(Individuals.MaxSeverityScore via Individual))` |
| **IndividualPredictions.IndividualHasHighSeverityPhenotype** | lookup | `If(Individual = "", False(), Lookup(Individuals.HasHighSeverityPhenotype via Individual))` |
| **IndividualPredictions.IndividualHasPredictedTreatmentResponse** | lookup | `If(Individual = "", False(), Lookup(Individuals.HasPredictedTreatmentResponse via Individual))` |
| **IndividualPredictions.PredictedValue** | formula | `If(2 * IndividualCausalMass + 1.5 * IndividualConfirmedNodeCount > 10, 10, 2 * IndividualCausalMass + 1.5 * IndividualConfirmedNodeCount)` |
| **IndividualPredictions.CountBins** | rollup | `Count(CalibrationBins via IndividualPrediction)` |
| **IndividualPredictions.CountWellCalibratedBins** | rollup | `Count(CalibrationBins via IndividualPrediction)` |
| **IndividualPredictions.SumBinAbsError** | rollup | `Sum(CalibrationBins.BinAbsError via IndividualPrediction)` |
| **IndividualPredictions.MeanBinAbsError** | formula | `If(CountBins > 0, SumBinAbsError / CountBins, 1)` |
| **IndividualPredictions.WellCalibratedFraction** | formula | `If(CountBins > 0, CountWellCalibratedBins / CountBins, 0)` |
| **IndividualPredictions.CalibratedUncertainty** | formula | `If(1 - MeanBinAbsError < 0, 0, 1 - MeanBinAbsError) * WellCalibratedFraction` |
| **IndividualPredictions.RestsOnConfirmedMechanism** | formula | `If(IndividualConfirmedNodeCount >= 1, True(), False())` |
| **IndividualPredictions.HasSpuriousCorrelationFlag** | formula | `If(Or(Not(RestsOnConfirmedMechanism), IndividualHasCrypticRelatedness), True(), False())` |
| **IndividualPredictions.IsFalsifiabilityBacked** | formula | `If(IndividualConfirmedNodeCount >= 1, True(), False())` |
| **IndividualPredictions.IsTransportableToAbsentAncestry** | formula | `If(And(IsAncestryHoldout, IndividualCrossAncestryNodeCount >= 1, Not(HasSpuriousCorrelationFlag)), True(), False())` |
| **IndividualPredictions.IsAncestryTransportSafe** | formula | `If(IsAncestryHoldout, IsTransportableToAbsentAncestry, True())` |
| **IndividualPredictions.TransportGateStatus** | formula | `If(Not(IsAncestryHoldout), "NotApplicable", If(IsTransportableToAbsentAncestry, "PASS-tested", "FAIL"))` |
| **IndividualPredictions.IsHighConfidencePrediction** | formula | `If(And(CalibratedUncertainty >= 0.7, Not(HasSpuriousCorrelationFlag)), True(), False())` |
| **IndividualPredictions.PatientStratificationTier** | formula | `If(PredictedValue >= 7, "High-Risk Pathway", If(PredictedValue >= 4, "Moderate-Risk Pathway", "Low-Risk Pathway"))` |
| **IndividualPredictions.PredictedSeverityValue** | formula | `IndividualMaxSeverityScore` |
| **IndividualPredictions.SeverityTier** | formula | `If(PredictedSeverityValue > 7, "Severe", If(PredictedSeverityValue >= 4, "Moderate", "Mild"))` |
| **IndividualPredictions.IsSeverityActionable** | formula | `If(And(IndividualHasHighSeverityPhenotype, RestsOnConfirmedMechanism, Not(HasSpuriousCorrelationFlag)), True(), False())` |
| **IndividualPredictions.SeverityDecidingFactor** | formula | `If(IsSeverityActionable, "HighSeverityOnConfirmedMechanism", If(Not(IndividualHasHighSeverityPhenotype), "NotHighSeverity", If(Not(RestsOnConfirmedMechanism), "NoValidatedMechanism", If(HasSpuriousCorrelationFlag, "SpuriousFlag", "Undetermined"))))` |
| **IndividualPredictions.IsTreatmentResponseActionable** | formula | `If(IndividualHasPredictedTreatmentResponse, True(), False())` |
| **IndividualPredictions.TreatmentResponseDecidingFactor** | formula | `If(IsTreatmentResponseActionable, "EffectiveOnConfirmedMechanism", If(RestsOnConfirmedMechanism, "NoEffectiveTreatmentOnMechanism", "NoConfirmedMechanism"))` |
| **IndividualPredictions.IsClinicallyActionable** | formula | `If(And(IsHighConfidencePrediction, IsFalsifiabilityBacked, IsAncestryTransportSafe, PredictedValue > 0), True(), False())` |
| **IndividualPredictions.LifecycleStateKey** | formula | `If(And(IsHighConfidencePrediction, IsFalsifiabilityBacked, IsAncestryTransportSafe, PredictedValue > 0), "Actionable", If(Or(Not(RestsOnConfirmedMechanism), Not(IsFalsifiabilityBacked)), "NotActionable", If(IndividualHasCrypticRelatedness, "NotActionable", If(CalibratedUncertainty < 0.7, "NotActionable", If(Not(IsAncestryTransportSafe), "NotActionable", "Actionable")))))` |
| **IndividualPredictions.DecidingGate** | formula | `If(IsClinicallyActionable, "AllGatesPass", If(Not(RestsOnConfirmedMechanism), "NoValidatedMechanism", If(IndividualHasCrypticRelatedness, "CrypticRelatedness", If(CalibratedUncertainty < 0.7, "Calibration", If(Not(IsAncestryTransportSafe), "AncestryTransport", "Undetermined")))))` |
| **IndividualPredictions.IndividualTargetPathway** | lookup | `Lookup(Individuals.TargetPathway via Individual)` |
| **IndividualPredictions.IndividualProgressionStateKey** | lookup | `Lookup(Individuals.NephritisProgressionStateKey via Individual)` |
| **IndividualPredictions.IndividualIsDiseaseProgressing** | lookup | `Lookup(Individuals.IsDiseaseProgressing via Individual)` |
| **IndividualPredictions.RecommendedTreatmentLine** | formula | `If(Not(RestsOnConfirmedMechanism), "No targeted line — mechanism unconfirmed", If(Or(IndividualProgressionStateKey = "RenalFlareRisk", IndividualProgressionStateKey = "BiopsyIndicated"), "Mycophenolate (induction)", If(IndividualTargetPathway = "type-I-IFN", "Anifrolumab", If(IndividualTargetPathway = "B-cell/autoantibody", "Belimumab", If(IndividualTargetPathway = "IL-17/23", "Secukinumab", "Standard-of-care (no mechanism-matched targeted line)")))))` |
| **IndividualPredictions.TreatmentLineDecidingFactor** | formula | `If(Not(RestsOnConfirmedMechanism), "MechanismUnconfirmed", If(Or(IndividualProgressionStateKey = "RenalFlareRisk", IndividualProgressionStateKey = "BiopsyIndicated"), "ActiveNephritis-Induction", If(IndividualTargetPathway = "type-I-IFN", "IFNSignature-Anifrolumab", If(IndividualTargetPathway = "B-cell/autoantibody", "AutoantibodyDriven-Belimumab", If(IndividualTargetPathway = "IL-17/23", "IL17Axis-Secukinumab", "NoMechanismMatch")))))` |
| **IndividualPredictions.ProgressionVsActionabilityDisagree** | formula | `If(And(IndividualIsDiseaseProgressing, Not(IsClinicallyActionable)), True(), False())` |
| **CalibrationBins.Name** | formula | `BinLabel` |
| **CalibrationBins.ParentPath** | lookup | `Lookup(IndividualPredictions.RelativePath via IndividualPrediction)` |
| **CalibrationBins.RelativePath** | formula | `ParentPath & "/bins/" & CalibrationBinId` |
| **CalibrationBins.BinAbsError** | formula | `If(PredictedProbabilityBand >= ObservedEventRate, PredictedProbabilityBand - ObservedEventRate, ObservedEventRate - PredictedProbabilityBand)` |
| **CalibrationBins.IsWellCalibratedBin** | formula | `If(And(CoverageCount >= 20, BinAbsError <= 0.1), True(), False())` |
| **InterventionTargets.Name** | formula | `TargetLabel` |
| **InterventionTargets.ParentPath** | lookup | `Lookup(CausalMechanisms.RelativePath via CausalMechanism)` |
| **InterventionTargets.RelativePath** | formula | `ParentPath & "/targets/" & InterventionTargetId` |
| **InterventionTargets.CausalMechanismLabel** | formula | `Lookup(CausalMechanisms.MechanismLabel via CausalMechanism)` |
| **InterventionTargets.IsGeneBasedTherapy** | formula | `If(TherapyClass = "Gene-based", True(), False())` |
| **InterventionTargets.IsCellBasedTherapy** | formula | `If(TherapyClass = "Cell-based", True(), False())` |
| **Axioms.Name** | formula | `Statement` |
| **Axioms.RelativePath** | formula | `"/admin/axioms/" & AxiomId` |
| **TestsForSuccess.Name** | formula | `Claim` |
| **TestsForSuccess.RelativePath** | formula | `"/admin/tests-for-success/" & TestForSuccessId` |
| **Features.Name** | formula | `Title` |
| **Features.RelativePath** | formula | `"/admin/features/" & FeatureId` |
| **Features.MetaLine** | formula | `"**Category:** " & Category & " - **Priority:** " & Priority & " - **Challenge refs:** " & RefCount` |
| **InferenceKinds.Name** | formula | `Title` |
| **InferenceKinds.RelativePath** | formula | `"/admin/inference-kinds/" & InferenceKindId` |
| **OpenQuestions.Name** | formula | `Question` |
| **OpenQuestions.RelativePath** | formula | `"/admin/open-questions/" & OpenQuestionId` |
| **NonGoals.Name** | formula | `Statement` |
| **NonGoals.RelativePath** | formula | `"/admin/non-goals/" & NonGoalId` |
| **GlossaryTerms.Name** | formula | `Term` |
| **GlossaryTerms.RelativePath** | formula | `"/admin/glossary/" & GlossaryTermId` |
| **LeopoldLoops.Name** | formula | `Concat("Loop ", LoopNumber, " — ", Title)` |
| **LeopoldLoops.RelativePath** | formula | `"/admin/leopold-loops/" & LeopoldLoopId` |
| **LeopoldLoops.Completedness** | formula | `Status` |
| **LeopoldLoops.IsInCurrentPlan** | formula | `If(Status = "done", FALSE, TRUE)` |
| **RoutingAndNavigation.Name** | formula | `Replace(Lower(DisplayName), " ", "-")` |
| **RoutingAndNavigation.AdminCanCreate** | formula | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("C", AdminCRUD))))` |
| **RoutingAndNavigation.AdminCanRead** | formula | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("R", AdminCRUD))))` |
| **RoutingAndNavigation.AdminCanUpdate** | formula | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("U", AdminCRUD))))` |
| **RoutingAndNavigation.AdminCanDelete** | formula | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("D", AdminCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanCreate** | formula | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("C", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanRead** | formula | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("R", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanUpdate** | formula | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("U", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanDelete** | formula | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("D", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanCreate** | formula | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("C", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanRead** | formula | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("R", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanUpdate** | formula | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("U", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanDelete** | formula | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("D", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanCreate** | formula | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("C", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanRead** | formula | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("R", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanUpdate** | formula | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("U", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanDelete** | formula | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("D", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.Depth** | formula | `If(ParentRouteKey = Blank(), 0, Len(RouteKey) - Len(Replace(RouteKey, ".", "")))` |
| **RoutingAndNavigation.FullPath** | formula | `Route` |
| **RoutingAndNavigation.HandlerBaseName** | formula | `Replace(Replace(RouteKey, ".", " "), "-", " ")` |
| **RoutingAndNavigation.RelativePath** | formula | `"/admin/routing/" & RoutingAndNavigationId` |
| **StateMachines.RelativePath** | formula | `"/admin/state-machine/" & StateMachineId` |
| **StateMachines.StateCount** | rollup | `Count(MachineStates via StateMachine)` |
| **StateMachines.TransitionRuleCount** | rollup | `Count(StateTransitionRules via StateMachine)` |
| **MachineStates.RelativePath** | formula | `"/admin/state-machine/states/" & MachineStateId` |
| **MachineStates.ReachableStateCount** | rollup | `Count(vw_state_transition_rules_closure via FromId)` |
| **StateTransitionRules.RelativePath** | formula | `"/admin/state-machine/rules/" & StateTransitionRuleId` |
| **StateTransitionRules.FromStateKey** | lookup | `Lookup(MachineStates.StateKey via FromState)` |
| **StateTransitionRules.ToStateKey** | lookup | `Lookup(MachineStates.StateKey via ToState)` |
| **StateTransitionRules.IsForwardEdge** | formula | `Not(ToStateKey = FromStateKey)` |
| **StateTransitions.RelativePath** | formula | `"/admin/state-machine/transitions/" & StateTransitionId` |
| **StateTransitions.IsForward** | formula | `Not(ToStateKey = "Intake")` |
| **SubjectStateInstances.RelativePath** | formula | `"/admin/state-machine/instances/" & SubjectStateInstanceId` |
| **SubjectStateInstances.IsCurrent** | formula | `Isblank(ExitedAt)` |
| **SubjectStateInstances.HasCompleteLineage** | formula | `SequenceIndex >= 1` |
| **SubjectStateInstances.IsLongDwell** | formula | `If(DwellDays >= 90, True(), False())` |
| **DiseaseDomainConcepts.Name** | formula | `ConceptLabel` |
| **DiseaseDomainConcepts.RelativePath** | formula | `"/admin/disease-concepts/" & DiseaseDomainConceptId` |
| **DiseaseDomainConcepts.IsDeeplyModeled** | formula | `If(ModelingStatus = "deep-dag", True(), False())` |
| **DiseaseDomainConcepts.IsSchemaModeled** | formula | `If(Or(ModelingStatus = "deep-dag", ModelingStatus = "schema"), True(), False())` |
| **SerologyObservations.PriorAntiDsDnaIU** | lookup | `Lookup(SerologyObservations.AntiDsDnaIU via PriorObservation)` |
| **SerologyObservations.PriorC3** | lookup | `Lookup(SerologyObservations.ComplementC3 via PriorObservation)` |
| **SerologyObservations.PriorC4** | lookup | `Lookup(SerologyObservations.ComplementC4 via PriorObservation)` |
| **SerologyObservations.AntiDsDnaTrend** | formula | `If(Isblank(PriorAntiDsDnaIU), "Stable", If(AntiDsDnaIU > PriorAntiDsDnaIU * 1.25, "Rising", If(AntiDsDnaIU < PriorAntiDsDnaIU * 0.8, "Falling", "Stable")))` |
| **SerologyObservations.ComplementTrend** | formula | `If(Isblank(PriorC3), "Stable", If(ComplementC3 + ComplementC4 < PriorC3 + PriorC4 * 0.85, "Falling", If(ComplementC3 + ComplementC4 > PriorC3 + PriorC4 * 1.15, "Rising", "Stable")))` |
| **SerologyObservations.IsPreNephriticSignaturePanel** | formula | `If(And(AntiDsDnaTrend = "Rising", ComplementTrend = "Falling"), True(), False())` |
| **SerologyObservations.IsSignificantProteinuria** | formula | `If(ProteinuriaGPerDay >= 0.5, True(), False())` |
| **SerologyObservations.IsNephroticRangeProteinuria** | formula | `If(ProteinuriaGPerDay >= 3.0, True(), False())` |
| **SerologyObservations.SledaiRenalPoints** | formula | `If(Or(IsNephroticRangeProteinuria, HasActiveUrinarySediment), 8, If(IsSignificantProteinuria, 4, 0))` |
| **SerologyObservations.SledaiSerologyPoints** | formula | `If(And(ComplementTrend = "Falling", AntiDsDnaTrend = "Rising"), 4, If(Or(ComplementTrend = "Falling", AntiDsDnaTrend = "Rising"), 2, 0))` |
| **SerologyObservations.SledaiScore** | formula | `SledaiRenalPoints + SledaiSerologyPoints` |
| **SerologyObservations.ProgressionStateKey** | formula | `If(Or(IsNephroticRangeProteinuria, HasActiveUrinarySediment), "BiopsyIndicated", If(ProteinuriaGPerDay >= 1.0, "RenalFlareRisk", If(IsSignificantProteinuria, "EarlyNephritis", If(And(AntiDsDnaTrend = "Rising", ComplementTrend = "Falling"), "SerologicActive", "PresymptomaticAutoimmunity"))))` |
| **SerologyObservations.ProgressionStateOrder** | formula | `If(ProgressionStateKey = "BiopsyIndicated", 5, If(ProgressionStateKey = "RenalFlareRisk", 4, If(ProgressionStateKey = "EarlyNephritis", 3, If(ProgressionStateKey = "SerologicActive", 2, 1))))` |
| **SerologyObservations.RelativePath** | formula | `"/admin/serology/" & SerologyObservationId` |
| **TherapyOptions.Name** | formula | `TherapyLabel` |
| **TherapyOptions.RelativePath** | formula | `"/admin/therapy-options/" & TherapyOptionId` |

---

_This document is rendered in **RuleSpeak®**, the declarative business-rule
notation created by **Ronald G. Ross**, and follows the conventions of
**SBVR** (Semantics of Business Vocabulary and Business Rules). With thanks to
Ronald G. Ross for RuleSpeak and his foundational work on business rules —
[www.RonRoss.info](https://www.RonRoss.info)._
