# 📘 Causal Autoimmune Architecture Platform — RuleSpeak

_Rulebook for inferring the complete causal architecture of heterogeneous autoimmune disease from multi-omic cohort data, producing falsifiable mechanisms and ancestry-equitable predictions with calibrated uncertainty._

> Deklarative Geschäftsregeln, aus dem Regelbuch gerendert. Jede Aussage
> unten drückt eine Wahrheit der Geschäftsdomäne aus — sie ist weder eine
> Prozedur noch ein Befehl. Die Formeln des Regelbuchs sind die einzige Quelle
> der Wahrheit; dieses Dokument ist ihre klarsprachliche Lesart.

## 1 Geschäftsvokabular

| Begriff | Beschreibung | Erläuternder Kommentar |
|---------|--------------|------------------------|
| **Autoimmune Diseas** | Target heterogeneous autoimmune disease definitions tracked across development, aging, tissues, and disease stages. | — |
| Name | Berechnet als der disease label. | _Display label for the autoimmune disease._ |
| Relative Path | Berechnet als der Wert „/diseases/“, gefolgt von der autoimmune disease ID. | _Canonical path to this AutoimmuneDisease page: /diseases/<slug-or-id>._ |
| Count of Disease Stages | Die Anzahl der mit dem autoimmune diseas verbundenen disease stages. | _Number of disease stages defined for this autoimmune disease._ |
| Count of Intervention Targets | Die Anzahl der mit dem autoimmune diseas verbundenen intervention targets. | _Count of validated intervention targets linked to this disease._ |
| **Disease Stage** | Ordered disease stages capturing presymptomatic, active, remission, and treatment-refractory phases along disease progression. | — |
| Name | Berechnet als der autoimmune disease disease label, gefolgt von der Wert „ — “, gefolgt von der stage label. | _Display label combining disease and stage._ |
| Parent Path | Der relative path des zugehörigen autoimmune disease des disease stage. | _Lookup: AutoimmuneDiseases.RelativePath via AutoimmuneDisease — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/stages/“, gefolgt von der disease stage ID. | _Path to this DiseaseStage page, chained under its AutoimmuneDisease parent._ |
| Autoimmune Disease Disease Label | Der disease label des zugehörigen autoimmune disease des disease stage. | _Lookup of parent disease label._ |
| Is Presymptomatic | Wahr, wenn der stage label ist der Wert „Presymptomatic“. | _True when stage is presymptomatic diagnosis window._ |
| **Tissue** | Anatomical tissues where omics assays resolve cell-state-specific effects; includes cases of missing tissues. | — |
| Name | Berechnet als der tissue label. | _Display label for the tissue._ |
| Relative Path | Berechnet als der Wert „/tissues/“, gefolgt von der tissue ID. | _Canonical path to this Tissue page: /tissues/<slug-or-id>._ |
| Count of Omics Assays | Die Anzahl der mit dem tissue verbundenen omics assays. | _Number of omics assays performed on this tissue._ |
| **Omics Modality** | Registry of omics assay modalities: RNA-seq, ATAC-seq, proteomics, metabolomics, methylomes, chromatin-conformation, immune-receptor repertoires, microbiomes, and long-read genomes. | — |
| Name | Berechnet als der modality label. | _Display label for the omics modality._ |
| Relative Path | Berechnet als der Wert „/omics-modalities/“, gefolgt von der omics modality ID. | _Canonical path to this OmicsModalitie page: /omics-modalities/<slug-or-id>._ |
| Count of Omics Assays | Die Anzahl der mit dem omics modality verbundenen omics assays. | _Count of assays using this modality._ |
| **Federated Dataset** | Privacy-preserving federated datasets contributing cohort partitions without centralizing raw genomes. | — |
| Name | Berechnet als der node label. | _Display label for the federated dataset._ |
| Relative Path | Berechnet als der Wert „/datasets/“, gefolgt von der federated dataset ID. | _Canonical path to this FederatedDataset page: /datasets/<slug-or-id>._ |
| Count of Individuals | Die Anzahl der mit dem federated dataset verbundenen individuals. | _Individuals enrolled via this federated node._ |
| **Variant Type** | Classification of genomic variant mechanisms: regulatory, coding, repeat expansions, mobile-element insertions, HLA haplotypes, structural variants, de novo mutations, somatic mosaicism, mitochondrial variation. | — |
| Name | Berechnet als der type label. | _Display label for variant type._ |
| Relative Path | Berechnet als der Wert „/variant-types/“, gefolgt von der variant type ID. | _Canonical path to this VariantType page: /variant-types/<slug-or-id>._ |
| Count of Genomic Variants | Die Anzahl der mit dem variant type verbundenen genomic variants. | _Variants classified under this type._ |
| **Individual** | Ancestrally diverse cohort participants whose phased genomes, omics profiles, exposures, and clinical phenotypes feed causal architecture inference. | — |
| Name | Berechnet als der given name, gefolgt von ein Leerzeichen, gefolgt von der family name. | _Display name for the individual._ |
| Slug | Berechnet als das kleingeschriebene family name, gefolgt von ein Bindestrich, gefolgt von der given name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _URL slug for this case, family-given kebab (e.g. reyes-ana). Used in RelativePath._ |
| Relative Path | Berechnet als der Wert „/intake/new-patient/“, gefolgt von der slug. | _Canonical path to this Individual page: /intake/new-patient/<slug-or-id>._ |
| Federated Dataset Node Label | Nach Priorität bestimmt: eine leere Zeichenkette, wenn der federated dataset leer ist; andernfalls der node label des zugehörigen federated dataset des individual. | _Lookup of federated node label._ |
| Count of Genomic Variants | Die Anzahl der mit dem individual verbundenen genomic variants. | _Total genomic variants for rare-variant burden assessment._ |
| Count of Causal Mechanisms | Die Anzahl der mit dem individual verbundenen causal mechanisms. | _Inferred causal mechanisms linked to this individual._ |
| Count of Epistatic Interactions | Die Anzahl der mit dem individual verbundenen epistatic interactions. | _Higher-order epistasis interactions involving this individual._ |
| Rare Variant Burden Score | Nach Priorität bestimmt: der count of genomic variants geteilt durch der age years, wenn der age years größer ist als 0; andernfalls 0. | _2nd-order score from rare variant count normalized by age._ |
| Causal Architecture Score | Berechnet als der count of causal mechanisms mal 10 plus der count of epistatic interactions mal 5 plus der rare variant burden score. | _3rd-order composite of causal mechanisms and epistasis density._ |
| Is Development Window | Wahr, wenn der age years höchstens 25 ist. | _True when age falls in developmental effects window (0-25)._ |
| Is Aging Window | Wahr, wenn der age years mindestens 60 ist. | _True when age falls in aging window (60+)._ |
| Count Confirmed Causal Nodes | Die Anzahl der causal mechanisms des individual, die causal architecture node sind. | _Count of this individual's confirmed causal-architecture nodes._ |
| Sum Confirmed Causal Confidence | Die Gesamtsumme von causal confidence über die causal mechanisms des individual, die causal architecture node sind. | _Summed confidence of this individual's confirmed causal nodes (derived causal mass)._ |
| Count Cross Ancestry Confirmed Nodes | Die Anzahl der causal mechanisms des individual, die ancestry transportable sind. | _Confirmed nodes that also showed cross-ancestry replication._ |
| Max Severity Score | Der größte severity score über die mit dem individual verbundenen clinical phenotypes. | _Highest SeverityScore across this individual's clinical phenotypes (0 if none)._ |
| Count High Severity Phenotypes | Die Anzahl der clinical phenotypes des individual, die high severity sind. | _Count of this individual's high-severity phenotypes (SeverityScore > 7)._ |
| Has High Severity Phenotype | Wahr, wenn der count high severity phenotypes mindestens 1 ist. | _True when the individual has at least one high-severity phenotype._ |
| Count Predicted Treatment Responses | Die Anzahl der treatments des individual, die treatment response predicted sind. | _Count of this individual's treatments predicted to respond (effective ∧ mechanism-matched)._ |
| Has Predicted Treatment Response | Wahr, wenn der count predicted treatment responses mindestens 1 ist. | _True when the individual has at least one treatment predicted to respond._ |
| Count Serology Panels | Die Anzahl der mit dem individual verbundenen serology observations. | _Number of serology panels for this individual._ |
| Count Pre Nephritic Signature Panels | Die Anzahl der serology observations des individual, die pre nephritic signature panel sind. | _How many of this individual's panels exhibit the pre-nephritic serology signature (rising anti-dsDNA + falling complement before overt nephritis). The corpus-level roll-up of the per-panel signal; emergent from the raw series._ |
| Is in Pre Nephritic Signature Cluster | Wahr, wenn der count pre nephritic signature panels mindestens 1 ist. | _DERIVED cluster membership: TRUE when this individual's own raw serology series ever showed the pre-nephritic signature. The discovery 'this serology signature precedes nephritis' becomes a reproducible, witnessed corpus-level cluster — not a label anyone assigned._ |
| Signature Strength | Nach Priorität bestimmt: 2, wenn der count pre nephritic signature panels mindestens 2 ist; 1, wenn der count pre nephritic signature panels mindestens 1 ist; andernfalls 0. | _DERIVED 0/1/2 strength of the pre-nephritic signature: 2 when it appears on >=2 panels (persistent), 1 on exactly one panel, 0 when absent. Drives the emphasis of cluster members on the cohort scatter._ |
| Max Progression State Order | Der größte progression state order über die mit dem individual verbundenen serology observations. | _Highest progression-state order reached across this individual's serology panels (worst/current state)._ |
| Latest Sledai Score | Der größte sledai score über die mit dem individual verbundenen serology observations. | _Peak SLEDAI activity score across this individual's panels (derived)._ |
| Nephritis Progression State Key | Nach Priorität bestimmt: der Wert „BiopsyIndicated“, wenn der max progression state order mindestens 5 ist; der Wert „RenalFlareRisk“, wenn der max progression state order mindestens 4 ist; der Wert „EarlyNephritis“, wenn der max progression state order mindestens 3 ist; der Wert „SerologicActive“, wenn der max progression state order mindestens 2 ist; andernfalls der Wert „PresymptomaticAutoimmunity“. | _DERIVED current disease state for the lupus-nephritis-progression machine: decoded from the highest state order reached. The subject- state column of the state machine; never hand-set._ |
| Activity Tier | Nach Priorität bestimmt: der Wert „High / flare“, wenn der latest sledai score mindestens 12 ist; der Wert „Moderate“, wenn der latest sledai score mindestens 6 ist; der Wert „Mild“, wenn der latest sledai score mindestens 1 ist; andernfalls der Wert „Quiescent“. | _Derived disease-activity tier from peak SLEDAI score._ |
| Is High Disease Activity | Wahr, wenn der latest sledai score mindestens 12 ist. | _TRUE when peak SLEDAI >= 12._ |
| Is Disease Progressing | Wahr, wenn mindestens eines des Folgenden gilt: der nephritis progression state key ist der Wert „EarlyNephritis“; der nephritis progression state key ist der Wert „RenalFlareRisk“; oder der nephritis progression state key ist der Wert „BiopsyIndicated“. | _TRUE when the disease-state simulator places the patient in an active/worsening renal state (independent of the actionability gate)._ |
| Target Pathway Code | Der größte target pathway code über die mit dem individual verbundenen causal mechanisms. | _Resolved pathway code from this individual's causal mechanism (MAXIFS)._ |
| Target Pathway | Nach Priorität bestimmt: der Wert „type-I-IFN“, wenn der target pathway code ist 1; der Wert „B-cell/autoantibody“, wenn der target pathway code ist 2; der Wert „T-cell-costim“, wenn der target pathway code ist 3; der Wert „IL-17/23“, wenn der target pathway code ist 4; andernfalls eine leere Zeichenkette. | _Decoded druggable pathway implicated by this individual's mechanism._ |
| Reachable States Ahead | Der reachable state count des zugehörigen current progression state ID des individual. | _How many disease states are still reachable ahead of THIS patient from their current state, via the transition closure (looks up MachineStates.ReachableStateCount for CurrentProgressionStateId). A closure-derived prognostic horizon: Diego at BiopsyIndicated has 0 ahead (terminal); a Presymptomatic patient has 5. NOT derivable from the severity rank alone (the count is non-monotonic in rank because the machine branches into remission)._ |
| **Genomic Variant** | Genomic variant calls per individual spanning regulatory, coding, structural, HLA haplotypes, de novo mutations, somatic mosaicism, and mitochondrial variation. | — |
| Name | Berechnet als der variant label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des genomic variant. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/variants/“, gefolgt von der genomic variant ID. | _Path to this GenomicVariant page, chained under its Individual parent._ |
| Variant Type Label | Der type label des zugehörigen variant type des genomic variant. | _Lookup of variant type label._ |
| Variant Class is Rare | Wahr, wenn der zugehörige variant type des genomic variant ist ein rare variant class. | _Whether this variant's class is a rare-variant class (observed type property)._ |
| Individual Ancestry Label | Der ancestry label des zugehörigen individual des genomic variant. | _Lookup of individual ancestry for stratification checks._ |
| Is Rare Variant | Wahr, wenn der allele frequency kleiner ist als 0.01. | _True when allele frequency below 0.01._ |
| Is Causal Candidate | Wahr, wenn alles Folgende gilt: mindestens eines des Folgenden gilt: das is rare variant-Kennzeichen gesetzt ist oder das variant class is rare-Kennzeichen gesetzt ist und das has allele specific expression-Kennzeichen gesetzt ist. | _Derived: rare (by frequency or class) AND shows allele-specific expression._ |
| **Omics Assay** | Omics assay instances linking individuals to modalities and tissues; captures batch effects, measurement error, and missing tissues. | — |
| Name | Berechnet als der assay label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des omics assay. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/assays/“, gefolgt von der omics assay ID. | _Path to this OmicsAssay page, chained under its Individual parent._ |
| Modality Label | Der modality label des zugehörigen omics modality des omics assay. | _Lookup of modality label._ |
| Tissue Label | Nach Priorität bestimmt: der Wert „Missing Tissue“, wenn der tissue leer ist; andernfalls der tissue label des zugehörigen tissue des omics assay. | _Lookup of tissue label._ |
| Has Batch Effect Risk | Wahr, wenn der measurement error score größer ist als 0.3. | _True when measurement error exceeds 0.3._ |
| Is High Quality Assay | Wahr, wenn alles Folgende gilt: es nicht der Fall ist, dass das has batch effect risk-Kennzeichen gesetzt ist und der measurement error score kleiner ist als 0.15. | _2nd-order quality flag._ |
| **Evidence Item** | One observed support signal for a causal mechanism, measured by one omics assay. Mechanism confidence is an aggregation over these rows. | — |
| Name | Berechnet als der evidence label. | _Display name._ |
| Parent Path | Der relative path des zugehörigen causal mechanism des evidence item. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/evidence/“, gefolgt von der evidence item ID. | _Path to this EvidenceItem page, chained under its CausalMechanism parent._ |
| Assay is High Quality | Wahr, wenn der zugehörige omics assay des evidence item ist ein high quality assay. | _Whether the supporting assay passed quality control._ |
| Z Stat | Nach Priorität bestimmt: der effect size geteilt durch der standard error, wenn der standard error größer ist als 0; andernfalls 0. | _Derived signal-to-noise ratio (effect / SE), 0 if SE non-positive._ |
| Is Confound Controlled | Wahr, wenn alles Folgende gilt: das is adjusted for ancestry p cs-Kennzeichen gesetzt ist und das is adjusted for batch-Kennzeichen gesetzt ist. | _Derived: both ancestry-PC and batch adjustment present._ |
| Is Qualified Evidence | Wahr, wenn alles Folgende gilt: das assay is high quality-Kennzeichen gesetzt ist; es nicht der Fall ist, dass das is negative control arm-Kennzeichen gesetzt ist; der z stat mindestens 2 ist; und das is confound controlled-Kennzeichen gesetzt ist. | _Derived: clean assay, real support arm, signal-to-noise >= 2, confound-controlled._ |
| **Cohort Replication** | One re-test of a causal mechanism in another federated cohort. Replication and cross-ancestry transport are aggregations over these rows. | — |
| Name | Berechnet als der replication label. | _Display name._ |
| Parent Path | Der relative path des zugehörigen causal mechanism des cohort replication. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/replications/“, gefolgt von der cohort replication ID. | _Path to this CohortReplication page, chained under its CausalMechanism parent._ |
| Replicated At Nominal Sig | Wahr, wenn alles Folgende gilt: der replication p value höchstens 0.05 ist und der replication effect sign ist 1. | _Derived: reproduced the predicted (positive) sign at nominal significance._ |
| Mechanism Primary Ancestry | Der individual ancestry label des zugehörigen causal mechanism des cohort replication. | _Ancestry of the individual the mechanism was discovered in._ |
| Is Different Ancestry Replication | Wahr, wenn es nicht der Fall ist, dass der replication ancestry label ist der mechanism primary ancestry. | _Derived: the re-test ran in a different ancestry than discovery._ |
| Is Cross Ancestry Concordant | Wahr, wenn alles Folgende gilt: das replicated at nominal sig-Kennzeichen gesetzt ist und das is different ancestry replication-Kennzeichen gesetzt ist. | _Derived: replicated at significance AND in a different ancestry (the transportability atom)._ |
| **Negative Control Test** | One stratification / permutation control on a causal mechanism. A true causal signal collapses under the control. | — |
| Name | Berechnet als der control label. | _Display name._ |
| Parent Path | Der relative path des zugehörigen causal mechanism des negative control test. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/neg-controls/“, gefolgt von der negative control test ID. | _Path to this NegativeControlTest page, chained under its CausalMechanism parent._ |
| Is Survived | Wahr, wenn der permutation effect size höchstens der null threshold ist. | _Derived: signal collapses under the control (within the null band), as a true causal effect should._ |
| **Environmental Exposure** | Longitudinal environmental exposures contributing to gene-environment-microbiome interactions and shifting environments. | — |
| Name | Berechnet als der exposure label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des environmental exposure. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/exposures/“, gefolgt von der environmental exposure ID. | _Path to this EnvironmentalExposure page, chained under its Individual parent._ |
| Individual Ancestry Label | Der ancestry label des zugehörigen individual des environmental exposure. | _Ancestry lookup for stratification._ |
| Is High Exposure | Wahr, wenn der exposure level größer ist als 5. | _True when exposure level exceeds threshold._ |
| **Treatment** | Treatment histories capturing treatment-induced changes, treatment response, and adverse effects. | — |
| Name | Berechnet als der treatment label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des treatment. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/treatments/“, gefolgt von der treatment ID. | _Path to this Treatment page, chained under its Individual parent._ |
| Autoimmune Disease Label | Der disease label des zugehörigen autoimmune disease des treatment. | _Disease label lookup._ |
| Is Effective Treatment | Wahr, wenn alles Folgende gilt: mindestens eines des Folgenden gilt: der treatment response ist der Wert „Complete“ oder der treatment response ist der Wert „Partial“ und es nicht der Fall ist, dass das has adverse effect-Kennzeichen gesetzt ist. | _True for Complete or Partial response without adverse effects._ |
| Is Mechanism Matched | Falsch, wenn der targets mechanism leer ist, andernfalls der is causal architecture node des zugehörigen targets mechanism des treatment. | _True when the treatment's target mechanism is a CONFIRMED causal-architecture node (empty-guarded). This is the 'mechanism match'._ |
| Is Treatment Response Predicted | Wahr, wenn alles Folgende gilt: das is effective treatment-Kennzeichen gesetzt ist und das is mechanism matched-Kennzeichen gesetzt ist. | _Derived: the treatment is effective AND targets a confirmed mechanism. A drug aimed at a debunked mechanism, or one that didn't respond / was adverse, is NOT predicted._ |
| Treatment Response Deciding Factor | Nach Priorität bestimmt: der Wert „EffectiveOnConfirmedMechanism“, wenn das is treatment response predicted-Kennzeichen gesetzt ist; der Wert „NoConfirmedMechanism“, wenn es nicht der Fall ist, dass das is mechanism matched-Kennzeichen gesetzt ist; der Wert „AdverseEffect“, wenn das has adverse effect-Kennzeichen gesetzt ist; der Wert „NoResponse“, wenn mindestens eines des Folgenden gilt: der treatment response ist der Wert „None“ oder der treatment response ist der Wert „Adverse“; andernfalls der Wert „Undetermined“. | _Why response is/ isn't predicted — the single deciding reason._ |
| **Clinical Phenotype** | Clinical phenotypes including severity, immune dysfunction markers, and feedback from disease progression. | — |
| Name | Berechnet als der phenotype label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des clinical phenotype. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/phenotypes/“, gefolgt von der clinical phenotype ID. | _Path to this ClinicalPhenotype page, chained under its Individual parent._ |
| Disease Stage Label | Nach Priorität bestimmt: eine leere Zeichenkette, wenn der disease stage leer ist; andernfalls der stage label des zugehörigen disease stage des clinical phenotype. | _Stage label lookup._ |
| Is High Severity | Wahr, wenn der severity score größer ist als 7. | _True when severity exceeds 7._ |
| Is Presymptomatic Phenotype | Wahr, wenn der disease stage label ist der Wert „Presymptomatic“. | _True when stage is presymptomatic._ |
| **Causal Mechanism** | Inferred causal mechanisms linking variants, exposures, and molecular state to clinical phenotypes; must be experimentally falsifiable. | — |
| Target Pathway Code | Nach Priorität bestimmt: 1, wenn der target pathway ist der Wert „type-I-IFN“; 2, wenn der target pathway ist der Wert „B-cell/autoantibody“; 3, wenn der target pathway ist der Wert „T-cell-costim“; 4, wenn der target pathway ist der Wert „IL-17/23“; andernfalls 0. | _Numeric encoding of TargetPathway so the individual can resolve it via a MAXIFS aggregation (transpiler-friendly). 1=type-I-IFN 2=B-cell 3=T-cell 4=IL-17/23._ |
| Name | Berechnet als der mechanism label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des causal mechanism. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/mechanisms/“, gefolgt von der causal mechanism ID. | _Path to this CausalMechanism page, chained under its Individual parent._ |
| Individual Ancestry Label | Der ancestry label des zugehörigen individual des causal mechanism. | _Ancestry lookup._ |
| Count Qualified Evidence | Die Anzahl der evidence items des causal mechanism, die qualified evidence sind. | _Count of qualified evidence items supporting this mechanism._ |
| Count Modalities Supporting | Die Anzahl der evidence items des causal mechanism, die cross modality sind und qualified evidence sind. | _Count of qualified cross-modality evidence items (multi-omic corroboration)._ |
| Count Intervention Targets | Die Anzahl der mit dem causal mechanism verbundenen intervention targets. | _Count of perturbable intervention targets for this mechanism (falsifiability requires >=1)._ |
| Is Experimentally Falsifiable | Wahr, wenn alles Folgende gilt: der count intervention targets mindestens 1 ist und der count qualified evidence mindestens 1 ist. | _Derived: a measurable qualified effect exists AND a named intervention can perturb it._ |
| Count Replications | Die Anzahl der mit dem causal mechanism verbundenen cohort replications. | _Total cross-cohort re-tests of this mechanism._ |
| Count Concordant Replications | Die Anzahl der cohort replications des causal mechanism, die replicated at nominal sig sind. | _Re-tests reproducing the predicted sign at significance._ |
| Count Cross Ancestry Concordant | Die Anzahl der cohort replications des causal mechanism, die cross ancestry concordant sind. | _Concordant re-tests that ran in a DIFFERENT ancestry (the transportability measurement)._ |
| Replication Fraction | Nach Priorität bestimmt: der count concordant replications geteilt durch der count replications, wenn der count replications größer ist als 0; andernfalls 0. | _Derived: fraction of re-tests that were concordant (guarded division)._ |
| Replicates Across Cohorts | Wahr, wenn alles Folgende gilt: der count replications mindestens 2 ist und der count concordant replications mindestens 2 ist. | _Derived: >=2 independent re-tests and >=2 concordant._ |
| Count Neg Control Tests | Die Anzahl der mit dem causal mechanism verbundenen negative control tests. | _Negative-control tests run on this mechanism._ |
| Count Neg Control Survived | Die Anzahl der negative control tests des causal mechanism, die survived sind. | _Negative-control tests the mechanism survived (collapsed under the null)._ |
| Survives Negative Controls | Wahr, wenn alles Folgende gilt: der count neg control tests mindestens 1 ist und der count neg control survived ist der count neg control tests. | _Derived: at least one control run AND all of them survived._ |
| Is Spurious Derived | Wahr, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das replicates across cohorts-Kennzeichen gesetzt ist; es nicht der Fall ist, dass das survives negative controls-Kennzeichen gesetzt ist; der count modalities supporting kleiner ist als 2; oder das has pleiotropy-Kennzeichen gesetzt ist. | _Derived: spurious unless replicated, survives controls, has >=2 modalities, and is not purely pleiotropic._ |
| Causal Confidence | Nach Priorität bestimmt: 1, wenn 0.30 mal 1, wenn der count qualified evidence mindestens 4 ist, andernfalls der count qualified evidence geteilt durch 4 plus 0.20 mal 1, wenn der count modalities supporting mindestens 3 ist, andernfalls der count modalities supporting geteilt durch 3 plus 0.30 mal der replication fraction plus 0.20 mal 1, wenn das survives negative controls-Kennzeichen gesetzt ist, andernfalls 0 größer ist als 1; andernfalls 0.30 mal 1, wenn der count qualified evidence mindestens 4 ist, andernfalls der count qualified evidence geteilt durch 4 plus 0.20 mal 1, wenn der count modalities supporting mindestens 3 ist, andernfalls der count modalities supporting geteilt durch 3 plus 0.30 mal der replication fraction plus 0.20 mal 1, wenn das survives negative controls-Kennzeichen gesetzt ist, andernfalls 0. | _Derived bounded blend of qualified-evidence count, modality breadth, replication rate, and control survival._ |
| Variant is Causal Candidate | Falsch, wenn der genomic variant leer ist, andernfalls der is causal candidate des zugehörigen genomic variant des causal mechanism. | _Whether the linked variant is itself a derived causal candidate (empty-guarded)._ |
| Is Causal Architecture Node | Wahr, wenn alles Folgende gilt: der causal confidence mindestens 0.7 ist; das is experimentally falsifiable-Kennzeichen gesetzt ist; es nicht der Fall ist, dass das is spurious derived-Kennzeichen gesetzt ist; und mindestens eines des Folgenden gilt: das variant is causal candidate-Kennzeichen gesetzt ist oder der environmental exposure einen Wert hat. | _Derived: a confirmed causal edge - confident, falsifiable, non-spurious, and grounded in a candidate variant or a real exposure._ |
| Is Ancestry Transportable | Wahr, wenn alles Folgende gilt: das is causal architecture node-Kennzeichen gesetzt ist und der count cross ancestry concordant mindestens 1 ist. | _Derived: a confirmed node whose effect replicated in >=1 different ancestry (measured invariance)._ |
| **Epistatic Interaction** | Higher-order epistasis and pleiotropy interactions between genomic variants. | — |
| Name | Berechnet als der interaction label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des epistatic interaction. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/epistasis/“, gefolgt von der epistatic interaction ID. | _Path to this EpistaticInteraction page, chained under its Individual parent._ |
| Is High Order Epistasis | Wahr, wenn der epistasis score größer ist als 0.5. | _True when epistasis score exceeds 0.5._ |
| **Counterfactual Trajectory** | Counterfactual disease trajectories inferred without randomized perturbation data. | — |
| Name | Berechnet als der trajectory label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des counterfactual trajectory. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/trajectories/“, gefolgt von der counterfactual trajectory ID. | _Path to this CounterfactualTrajectorie page, chained under its Individual parent._ |
| Autoimmune Disease Label | Der disease label des zugehörigen autoimmune disease des counterfactual trajectory. | _Disease label lookup._ |
| Is Worsening Trajectory | Wahr, wenn der projected severity größer ist als 7. | _True when projected severity exceeds 7._ |
| **Individual Prediction** | Predictions of disease onset, severity, treatment response, and adverse effects with calibrated uncertainty for ancestry-equitable risk prediction. | — |
| Name | Berechnet als der prediction label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen individual des individual prediction. | _Lookup: Individuals.RelativePath via Individual — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/predictions/“, gefolgt von der individual prediction ID. | _Path to this IndividualPrediction page, chained under its Individual parent._ |
| Individual Ancestry Label | Der ancestry label des zugehörigen individual des individual prediction. | _Ancestry for equity audit._ |
| Is Ancestry Holdout | Wahr, wenn der zugehörige individual des individual prediction ist ancestry absent from training. | _True when individual ancestry absent from training._ |
| Individual Causal Mass | 0, wenn der individual leer ist, andernfalls der sum confirmed causal confidence des zugehörigen individual des individual prediction. | _Summed confirmed causal confidence for this individual (empty-guarded)._ |
| Individual Confirmed Node Count | 0, wenn der individual leer ist, andernfalls der count confirmed causal nodes des zugehörigen individual des individual prediction. | _Count of this individual's confirmed causal nodes (empty-guarded)._ |
| Individual Cross Ancestry Node Count | 0, wenn der individual leer ist, andernfalls der count cross ancestry confirmed nodes des zugehörigen individual des individual prediction. | _Count of this individual's cross-ancestry-replicated confirmed nodes (empty-guarded)._ |
| Individual Has Cryptic Relatedness | Falsch, wenn der individual leer ist, andernfalls der has cryptic relatedness flag des zugehörigen individual des individual prediction. | _Whether this individual carries a cryptic-relatedness leakage flag (empty-guarded)._ |
| Individual Max Severity Score | 0, wenn der individual leer ist, andernfalls der max severity score des zugehörigen individual des individual prediction. | _This individual's max clinical SeverityScore (empty-guarded)._ |
| Individual Has High Severity Phenotype | Falsch, wenn der individual leer ist, andernfalls der has high severity phenotype des zugehörigen individual des individual prediction. | _Whether this individual has a high-severity phenotype (empty-guarded)._ |
| Individual Has Predicted Treatment Response | Falsch, wenn der individual leer ist, andernfalls der has predicted treatment response des zugehörigen individual des individual prediction. | _Whether this individual has a treatment predicted to respond (empty-guarded)._ |
| Predicted Value | Nach Priorität bestimmt: 10, wenn 2 mal der individual causal mass plus 1.5 mal der individual confirmed node count größer ist als 10; andernfalls 2 mal der individual causal mass plus 1.5 mal der individual confirmed node count. | _Derived risk magnitude (0-10), a monotone function of validated causal mass only - rides mechanism, not ancestry correlation._ |
| Count Bins | Die Anzahl der mit dem individual prediction verbundenen calibration bins. | _Total reliability bins for this prediction._ |
| Count Well Calibrated Bins | Die Anzahl der calibration bins des individual prediction, die well calibrated bin sind. | _Bins passing coverage and accuracy._ |
| Sum Bin Abs Error | Die Gesamtsumme von bin abs error über die mit dem individual prediction verbundenen calibration bins. | _Summed reliability gap across this prediction's bins._ |
| Mean Bin Abs Error | Nach Priorität bestimmt: der sum bin abs error geteilt durch der count bins, wenn der count bins größer ist als 0; andernfalls 1. | _Derived mean reliability gap; defaults to 1 (worst) when no bins exist._ |
| Well Calibrated Fraction | Nach Priorität bestimmt: der count well calibrated bins geteilt durch der count bins, wenn der count bins größer ist als 0; andernfalls 0. | _Derived fraction of trustworthy bins (guarded division)._ |
| Calibrated Uncertainty | Berechnet als 0, wenn 1 minus der mean bin abs error kleiner ist als 0, andernfalls 1 minus der mean bin abs error mal der well calibrated fraction. | _Derived reliability (HIGH = trustworthy): (1 - mean gap) scaled by well-covered-bin fraction; 0 for uncovered predictions._ |
| Rests on Confirmed Mechanism | Wahr, wenn der individual confirmed node count mindestens 1 ist. | _Derived: grounded in >=1 validated mechanism._ |
| Has Spurious Correlation Flag | Wahr, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist oder das individual has cryptic relatedness-Kennzeichen gesetzt ist. | _Derived: spurious if no validated mechanism OR cryptic-relatedness leakage._ |
| Is Falsifiability Backed | Wahr, wenn der individual confirmed node count mindestens 1 ist. | _Derived: inherits falsifiability - every confirmed node required IsExperimentallyFalsifiable._ |
| Is Transportable to Absent Ancestry | Wahr, wenn alles Folgende gilt: das is ancestry holdout-Kennzeichen gesetzt ist; der individual cross ancestry node count mindestens 1 ist; und es nicht der Fall ist, dass das has spurious correlation flag gesetzt ist. | _Derived: a holdout individual is transportable only with >=1 cross-ancestry-replicated node and no spurious flag._ |
| Is Ancestry Transport Safe | Wahr, wenn der is transportable to absent ancestry, wenn das is ancestry holdout-Kennzeichen gesetzt ist, andernfalls wahr. | _Derived: holdout requires measured transport; in-training is vacuously safe._ |
| Transport Gate Status | Nach Priorität bestimmt: der Wert „NotApplicable“, wenn es nicht der Fall ist, dass das is ancestry holdout-Kennzeichen gesetzt ist; der Wert „PASS-tested“, wenn das is transportable to absent ancestry-Kennzeichen gesetzt ist; andernfalls der Wert „FAIL“. | _RENDER ONLY (does NOT feed the keystone): honest three-state view of the transport gate so a vacuous in-training pass is never shown as evidentiary. NotApplicable = in-training ancestry (gate did not bite); PASS-tested = holdout with a confirmed cross-ancestry transport; FAIL = holdout without one. Sits beside IsAncestryTransportSafe (which the keystone still reads) purely to keep the writeup from implying transport evidence it never used._ |
| Is High Confidence Prediction | Wahr, wenn alles Folgende gilt: der calibrated uncertainty mindestens 0.7 ist und es nicht der Fall ist, dass das has spurious correlation flag gesetzt ist. | _Derived: calibrated AND not spurious._ |
| Patient Stratification Tier | Nach Priorität bestimmt: der Wert „High-Risk Pathway“, wenn der predicted value mindestens 7 ist; der Wert „Moderate-Risk Pathway“, wenn der predicted value mindestens 4 ist; andernfalls der Wert „Low-Risk Pathway“. | _Derived risk tier from the derived PredictedValue._ |
| Predicted Severity Value | Berechnet als der individual max severity score. | _Derived severity prediction grounded in the individual's max clinical SeverityScore._ |
| Severity Tier | Nach Priorität bestimmt: der Wert „Severe“, wenn der predicted severity value größer ist als 7; der Wert „Moderate“, wenn der predicted severity value mindestens 4 ist; andernfalls der Wert „Mild“. | _Derived severity band from the predicted severity value._ |
| Is Severity Actionable | Wahr, wenn alles Folgende gilt: das individual has high severity phenotype-Kennzeichen gesetzt ist; das rests on confirmed mechanism-Kennzeichen gesetzt ist; und es nicht der Fall ist, dass das has spurious correlation flag gesetzt ist. | _Derived: a high-severity phenotype on a confirmed, non-spurious mechanism. Chained to the onset gates so severity can never be actionable on a debunked mechanism._ |
| Severity Deciding Factor | Nach Priorität bestimmt: der Wert „HighSeverityOnConfirmedMechanism“, wenn das is severity actionable-Kennzeichen gesetzt ist; der Wert „NotHighSeverity“, wenn es nicht der Fall ist, dass das individual has high severity phenotype-Kennzeichen gesetzt ist; der Wert „NoValidatedMechanism“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist; der Wert „SpuriousFlag“, wenn das has spurious correlation flag gesetzt ist; andernfalls der Wert „Undetermined“. | _Why severity is/ isn't actionable — the single deciding reason._ |
| Is Treatment Response Actionable | Wahr, wenn das individual has predicted treatment response-Kennzeichen gesetzt ist. | _Derived: the individual has a treatment predicted to respond (effective therapy on a confirmed mechanism). The third prediction type — independent of onset/severity._ |
| Treatment Response Deciding Factor | Nach Priorität bestimmt: der Wert „EffectiveOnConfirmedMechanism“, wenn das is treatment response actionable-Kennzeichen gesetzt ist; der Wert „NoEffectiveTreatmentOnMechanism“, wenn das rests on confirmed mechanism-Kennzeichen gesetzt ist; andernfalls der Wert „NoConfirmedMechanism“. | _Why treatment-response is/ isn't actionable for this individual._ |
| Is Clinically Actionable | Wahr, wenn alles Folgende gilt: das is high confidence prediction-Kennzeichen gesetzt ist; das is falsifiability backed-Kennzeichen gesetzt ist; das is ancestry transport safe-Kennzeichen gesetzt ist; und der predicted value größer ist als 0. | _KEYSTONE: TRUE only when the prediction is high-confidence (calibrated + not spurious), falsifiability-backed, ancestry-transport-safe, and rests on a non-null derived magnitude._ |
| Lifecycle State Key | Nach Priorität bestimmt: der Wert „Actionable“, wenn alles Folgende gilt: das is high confidence prediction-Kennzeichen gesetzt ist; das is falsifiability backed-Kennzeichen gesetzt ist; das is ancestry transport safe-Kennzeichen gesetzt ist; und der predicted value größer ist als 0; der Wert „NotActionable“, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist oder es nicht der Fall ist, dass das is falsifiability backed-Kennzeichen gesetzt ist; der Wert „NotActionable“, wenn das individual has cryptic relatedness-Kennzeichen gesetzt ist; der Wert „NotActionable“, wenn der calibrated uncertainty kleiner ist als 0.7; der Wert „NotActionable“, wenn es nicht der Fall ist, dass das is ancestry transport safe-Kennzeichen gesetzt ist; andernfalls der Wert „Actionable“. | _DERIVED current lifecycle state (never entered): the single deciding gate determines whether the case lands on Actionable or NotActionable. Subject-state column of the diagnosis-lifecycle machine._ |
| Deciding Gate | Nach Priorität bestimmt: der Wert „AllGatesPass“, wenn das is clinically actionable-Kennzeichen gesetzt ist; der Wert „NoValidatedMechanism“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist; der Wert „CrypticRelatedness“, wenn das individual has cryptic relatedness-Kennzeichen gesetzt ist; der Wert „Calibration“, wenn der calibrated uncertainty kleiner ist als 0.7; der Wert „AncestryTransport“, wenn es nicht der Fall ist, dass das is ancestry transport safe-Kennzeichen gesetzt ist; andernfalls der Wert „Undetermined“. | _DERIVED single primary deciding gate (never entered), named in keystone-AND priority order. 'AllGatesPass' when actionable. When the case rests on no validated mechanism (no confirmed causal node), Falsifiability, Confidence, and Magnitude are one and the same finding, reported as 'NoValidatedMechanism' rather than split across three gates. Otherwise the lone failing gate is named: CrypticRelatedness, Calibration, AncestryTransport._ |
| Individual Target Pathway | Der target pathway des zugehörigen individual des individual prediction. | _TargetPathway resolved on the individual (decoded from the mechanism)._ |
| Individual Progression State Key | Der nephritis progression state key des zugehörigen individual des individual prediction. | _Lookup: the individual's derived disease state._ |
| Individual is Disease Progressing | Wahr, wenn der zugehörige individual des individual prediction ist disease progressing. | _Lookup: is the individual's disease progressing (simulator)._ |
| Recommended Treatment Line | Nach Priorität bestimmt: der Wert „No targeted line — mechanism unconfirmed“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist; der Wert „Mycophenolate (induction)“, wenn mindestens eines des Folgenden gilt: der individual progression state key ist der Wert „RenalFlareRisk“ oder der individual progression state key ist der Wert „BiopsyIndicated“; der Wert „Anifrolumab“, wenn der individual target pathway ist der Wert „type-I-IFN“; der Wert „Belimumab“, wenn der individual target pathway ist der Wert „B-cell/autoantibody“; der Wert „Secukinumab“, wenn der individual target pathway ist der Wert „IL-17/23“; andernfalls der Wert „Standard-of-care (no mechanism-matched targeted line)“. | _DERIVED treatment-line recommendation from confirmed-mechanism TargetPathway + disease state. The audit's MMF/belimumab/anifrolumab example._ |
| Treatment Line Deciding Factor | Nach Priorität bestimmt: der Wert „MechanismUnconfirmed“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist; der Wert „ActiveNephritis-Induction“, wenn mindestens eines des Folgenden gilt: der individual progression state key ist der Wert „RenalFlareRisk“ oder der individual progression state key ist der Wert „BiopsyIndicated“; der Wert „IFNSignature-Anifrolumab“, wenn der individual target pathway ist der Wert „type-I-IFN“; der Wert „AutoantibodyDriven-Belimumab“, wenn der individual target pathway ist der Wert „B-cell/autoantibody“; der Wert „IL17Axis-Secukinumab“, wenn der individual target pathway ist der Wert „IL-17/23“; andernfalls der Wert „NoMechanismMatch“. | _The single deciding reason for the recommended line (mirrors DecidingGate style)._ |
| Progression Vs Actionability Disagree | Wahr, wenn alles Folgende gilt: das individual is disease progressing-Kennzeichen gesetzt ist und es nicht der Fall ist, dass das is clinically actionable-Kennzeichen gesetzt ist. | _THE COUNTER-EXAMPLE FLAG: TRUE when disease is progressing (simulator) but the prediction is NOT clinically actionable (gate). Proves the two layers are independent._ |
| **Calibration Bin** | Per-prediction reliability-curve bins, seeded from the held-out coverage of this individual's own ancestry x disease. Calibration is an aggregation over these rows. | — |
| Name | Berechnet als der bin label. | _Display name._ |
| Parent Path | Der relative path des zugehörigen individual prediction des calibration bin. | _Lookup: IndividualPredictions.RelativePath via IndividualPrediction — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/bins/“, gefolgt von der calibration bin ID. | _Path to this CalibrationBin page, chained under its IndividualPrediction parent._ |
| Bin Abs Error | Nach Priorität bestimmt: der predicted probability band minus der observed event rate, wenn der predicted probability band mindestens der observed event rate ist; andernfalls der observed event rate minus der predicted probability band. | _Derived: absolute gap between predicted band and observed rate._ |
| Is Well Calibrated Bin | Wahr, wenn alles Folgende gilt: der coverage count mindestens 20 ist und der bin abs error höchstens 0.1 ist. | _Derived: enough held-out coverage AND small reliability gap._ |
| **Intervention Target** | Validated intervention targets for gene-based and cell-based therapies derived from falsifiable causal mechanisms. | — |
| Name | Berechnet als der target label. | _Display label._ |
| Parent Path | Der relative path des zugehörigen causal mechanism des intervention target. | _Lookup: CausalMechanisms.RelativePath via CausalMechanism — used to chain this entity's path under its parent._ |
| Relative Path | Berechnet als der parent path, gefolgt von der Wert „/targets/“, gefolgt von der intervention target ID. | _Path to this InterventionTarget page, chained under its CausalMechanism parent._ |
| Causal Mechanism Label | Der mechanism label des zugehörigen causal mechanism des intervention target. | _Mechanism label lookup._ |
| Is Gene Based Therapy | Wahr, wenn der therapy class ist der Wert „Gene-based“. | _True when therapy class is gene-based._ |
| Is Cell Based Therapy | Wahr, wenn der therapy class ist der Wert „Cell-based“. | _True when therapy class is cell-based._ |
| **Axiom** | Non-negotiable invariants the platform must obey. Load-bearing constraints, not per-loop work. Captured from the gauntlet conversation. | — |
| Name | Berechnet als der statement. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/axioms/“, gefolgt von der axiom ID. | _Path to this Axioms entry: /admin/axioms/<id>._ |
| **Tests for Success** | Falsifiable conditions that prove the axioms hold. The human-readable index of what each demonstration shows; many are realized in the witnessed harness. | — |
| Name | Berechnet als der claim. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/tests-for-success/“, gefolgt von der test for success ID. | _Path to this TestsForSuccess entry: /admin/tests-for-success/<id>._ |
| **Feature** | Catalog of buildable capabilities the platform has / allows for. Coarser grain than loops. Each row carries challenge provenance (ChallengeRefs: exact quoted text + file/line/col into THE-ORIGINAL-CHALLENGE.md, for UI hover tips), a Category, a Priority, and a free-form Markdown ChallengeNotes comment. Source of truth for the DERIVED PLATFORM_FEATURES.md. AssignedLoop links a feature to the loop that delivers it (nullable until scheduled). | — |
| Name | Berechnet als der title. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/features/“, gefolgt von der feature ID. | _Path to this Features entry: /admin/features/<id>._ |
| Meta Line | Berechnet als der Wert „**Category:** “, gefolgt von der category, gefolgt von der Wert „ - **Priority:** “, gefolgt von der priority, gefolgt von der Wert „ - **Challenge refs:** “, gefolgt von der ref count. | _One-line meta summary for the catalog (category - priority - ref count)._ |
| **Inference Kind** | Families of derivation the platform performs - the KINDS of inference in the DAG (lookups, aggregations, higher-order gates, state machines, transitive closure, predicate-gated narrative, cross-substrate conformance), independent of any one feature. Lets PLATFORM_FEATURES.md open with a short overview of the reasoning machinery before the per-feature catalog. Grounded in field types that actually exist in this rulebook; Maturity is Live unless an upstream step is still in flight. | — |
| Name | Berechnet als der title. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/inference-kinds/“, gefolgt von der inference kind ID. | _Path to this entry: /admin/inference-kinds/<id>._ |
| **Open Question** | Decisions still pending, captured so they are not silently re-litigated in a later session. | — |
| Name | Berechnet als der question. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/open-questions/“, gefolgt von der open question ID. | _Path to this OpenQuestions entry: /admin/open-questions/<id>._ |
| **Non Goal** | Explicit out-of-scope statements — the positive twin of the anti-hallucination ledger. Stops scope creep. | — |
| Name | Berechnet als der statement. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/non-goals/“, gefolgt von der non goal ID. | _Path to this NonGoals entry: /admin/non-goals/<id>._ |
| **Glossary Term** | Vocabulary coined in the gauntlet conversation, so the framing is shared and stable across sessions. | — |
| Name | Berechnet als der term. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/glossary/“, gefolgt von der glossary term ID. | _Path to this GlossaryTerms entry: /admin/glossary/<id>._ |
| **Leopold Loop** | The ordered Leopold loops that build this platform, as data. The derived plan (LEOPOLD_LOOPs.md, via json-hbars-transform) is generated from these rows; completed ([DONE]) loops are pruned at publish so only current/roadmap work shows in the plan. | — |
| Name | Berechnet als der Wert „Loop “, gefolgt von der loop number, gefolgt von der Wert „ — “, gefolgt von der title. | _Display label._ |
| Relative Path | Berechnet als der Wert „/admin/leopold-loops/“, gefolgt von der leopold loop ID. | _Path to this LeopoldLoops entry: /admin/leopold-loops/<id>._ |
| Completedness | Berechnet als der status. | _Normalized status used by the derived plan to decide placement._ |
| Is in Current Plan | Wahr, wenn es nicht der Fall ist, dass der status ist der Wert „done“. | _TRUE for the current "next" loop and anything still planned/backlog (not done)._ |
| **Routing and Navigation** | Role-based navigation: open-ended parent->child->leaf routes with computed paths. Each route has a template (/intake/case/:caseId); entities carry RelativePath that substitutes their own id/slug. Roles: admin, intake-clinician, diagnosing-doctor, external-llm. | — |
| Name | Berechnet als das kleingeschriebene display name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Slug of the display name._ |
| Admin Can Create | Wahr, wenn der admin CRUD der Wert „C“ erwähnt. | _Derived: AdminCRUD contains 'C'._ |
| Admin Can Read | Wahr, wenn der admin CRUD der Wert „R“ erwähnt. | _Derived: AdminCRUD contains 'R'._ |
| Admin Can Update | Wahr, wenn der admin CRUD der Wert „U“ erwähnt. | _Derived: AdminCRUD contains 'U'._ |
| Admin Can Delete | Wahr, wenn der admin CRUD der Wert „D“ erwähnt. | _Derived: AdminCRUD contains 'D'._ |
| Intake Clinician Can Create | Wahr, wenn der intake clinician CRUD der Wert „C“ erwähnt. | _Derived: IntakeClinicianCRUD contains 'C'._ |
| Intake Clinician Can Read | Wahr, wenn der intake clinician CRUD der Wert „R“ erwähnt. | _Derived: IntakeClinicianCRUD contains 'R'._ |
| Intake Clinician Can Update | Wahr, wenn der intake clinician CRUD der Wert „U“ erwähnt. | _Derived: IntakeClinicianCRUD contains 'U'._ |
| Intake Clinician Can Delete | Wahr, wenn der intake clinician CRUD der Wert „D“ erwähnt. | _Derived: IntakeClinicianCRUD contains 'D'._ |
| Diagnosing Doctor Can Create | Wahr, wenn der diagnosing doctor CRUD der Wert „C“ erwähnt. | _Derived: DiagnosingDoctorCRUD contains 'C'._ |
| Diagnosing Doctor Can Read | Wahr, wenn der diagnosing doctor CRUD der Wert „R“ erwähnt. | _Derived: DiagnosingDoctorCRUD contains 'R'._ |
| Diagnosing Doctor Can Update | Wahr, wenn der diagnosing doctor CRUD der Wert „U“ erwähnt. | _Derived: DiagnosingDoctorCRUD contains 'U'._ |
| Diagnosing Doctor Can Delete | Wahr, wenn der diagnosing doctor CRUD der Wert „D“ erwähnt. | _Derived: DiagnosingDoctorCRUD contains 'D'._ |
| External Llm Can Create | Wahr, wenn der external llm CRUD der Wert „C“ erwähnt. | _Derived: ExternalLlmCRUD contains 'C'._ |
| External Llm Can Read | Wahr, wenn der external llm CRUD der Wert „R“ erwähnt. | _Derived: ExternalLlmCRUD contains 'R'._ |
| External Llm Can Update | Wahr, wenn der external llm CRUD der Wert „U“ erwähnt. | _Derived: ExternalLlmCRUD contains 'U'._ |
| External Llm Can Delete | Wahr, wenn der external llm CRUD der Wert „D“ erwähnt. | _Derived: ExternalLlmCRUD contains 'D'._ |
| Depth | Nach Priorität bestimmt: 0, wenn der parent route key leer ist; andernfalls die Länge von der route key minus die Länge von der route key, wobei jedes ein Punkt durch eine leere Zeichenkette ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Nesting depth: 0 = top-level; otherwise number of dot-segments in RouteKey._ |
| Full Path | Berechnet als der route. | _Canonical role-agnostic URL path (equals Route)._ |
| Handler Base Name | Berechnet als der route key, wobei jedes ein Punkt durch ein Leerzeichen ersetzt wird, wobei jedes ein Bindestrich durch ein Leerzeichen ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> | _Space-delimited form of RouteKey; client PascalCases + prefixes role to derive a handler component._ |
| Relative Path | Berechnet als der Wert „/admin/routing/“, gefolgt von der routing and navigation ID. | _Path to this routing node's own admin page._ |
| **State Machine** | State-machine definitions. | — |
| Relative Path | Berechnet als der Wert „/admin/state-machine/“, gefolgt von der state machine ID. | _Path to this machine's page._ |
| State Count | Die Anzahl der mit dem state machine verbundenen machine states. | _Count of MachineStates in this machine._ |
| Transition Rule Count | Die Anzahl der mit dem state machine verbundenen state transition rules. | _Count of StateTransitionRules in this machine._ |
| **Machine State** | Legal states of each machine. | — |
| Relative Path | Berechnet als der Wert „/admin/state-machine/states/“, gefolgt von der machine state ID. | _Path to this state's page._ |
| Reachable State Count | Die Anzahl der mit dem machine state verbundenen vw state transition rules closure. | _Number of states reachable FROM this state via the transitive closure of the transition edges (rollup over vw_state_transition_rules_closure where from_id = this state). This is a graph-reachability fact the linear severity rank CANNOT reproduce: both Quiescent (the remission sink) and BiopsyIndicated (the terminal progression state) have 0 reachable-ahead, though they sit at opposite ends of the order — the count is non-monotonic in OrderIndex because the machine BRANCHES (remission). The closure is load-bearing here: delete it and this field is uncomputable._ |
| **State Transition Rule** | Legal edges (guards) of each machine. | — |
| Relative Path | Berechnet als der Wert „/admin/state-machine/rules/“, gefolgt von der state transition rule ID. | _Path to this rule's page._ |
| From State Key | Der state key des zugehörigen from state des state transition rule. | _Lookup: FromState.StateKey._ |
| To State Key | Der state key des zugehörigen to state des state transition rule. | _Lookup: ToState.StateKey._ |
| Is Forward Edge | Wahr, wenn es nicht der Fall ist, dass der to state key ist der from state key. | _TRUE when the edge advances to a different state (all edges here)._ |
| **State Transition** | Instance-level transition log (the witnessed history). | — |
| Relative Path | Berechnet als der Wert „/admin/state-machine/transitions/“, gefolgt von der state transition ID. | _Path to this transition's page._ |
| Is Forward | Wahr, wenn es nicht der Fall ist, dass der to state key ist der Wert „Intake“. | _TRUE when ToStateKey is not the machine's initial state._ |
| **Subject State Instance** | Per-subject state occupancy records; current state has blank ExitedAt. | — |
| Relative Path | Berechnet als der Wert „/admin/state-machine/instances/“, gefolgt von der subject state instance ID. | _Path to this occupancy record._ |
| Is Current | Wahr, wenn der exited at leer ist. | _TRUE when ExitedAt IS NULL — the subject's active state._ |
| Has Complete Lineage | Wahr, wenn der sequence index mindestens 1 ist. | _TRUE when lineage walks back to SequenceIndex 1._ |
| Is Long Dwell | Wahr, wenn der dwell days mindestens 90 ist. | _TRUE when DwellDays >= 90 (a season)._ |
| **Disease Domain Concept** | v2 vocabulary completeness: every disease-domain concept the v1 audit named, with its modeling status and the challenge-stressor TYPE it instantiates. Makes the coverage claim checkable by grep, not by trust. | — |
| Name | Berechnet als der concept label. | _Echoes ConceptLabel._ |
| Relative Path | Berechnet als der Wert „/admin/disease-concepts/“, gefolgt von der disease domain concept ID. | _Path to this concept's page._ |
| Is Deeply Modeled | Wahr, wenn der modeling status ist der Wert „deep-dag“. | _TRUE when this concept carries a witnessed inference DAG (status = deep-dag)._ |
| Is Schema Modeled | Wahr, wenn mindestens eines des Folgenden gilt: der modeling status ist der Wert „deep-dag“ oder der modeling status ist der Wert „schema“. | _TRUE when first-class schema/data (deep-dag or schema)._ |
| **Serology Observation** | RAW longitudinal serology panels (the lab/LLM layer) + derived trend, SLEDAI sub-scores, and the disease state IMPLIED by each panel. The leaves that drive the lupus-nephritis progression machine. | — |
| Prior Anti Ds Dna IU | Der anti ds dna IU des zugehörigen prior observation des serology observation. | _Prior panel's dsDNA via PriorObservation FK, for trend._ |
| Prior C3 | Der complement c3 des zugehörigen prior observation des serology observation. | _Prior C3 via PriorObservation FK._ |
| Prior C4 | Der complement c4 des zugehörigen prior observation des serology observation. | _Prior C4 via PriorObservation FK._ |
| Anti Ds Dna Trend | Nach Priorität bestimmt: der Wert „Stable“, wenn der prior anti ds dna IU leer ist; der Wert „Rising“, wenn der anti ds dna IU größer ist als der prior anti ds dna IU mal 1.25; der Wert „Falling“, wenn der anti ds dna IU kleiner ist als der prior anti ds dna IU mal 0.8; andernfalls der Wert „Stable“. | _Rising/Falling/Stable vs prior panel (derived from raw)._ |
| Complement Trend | Nach Priorität bestimmt: der Wert „Stable“, wenn der prior c3 leer ist; der Wert „Falling“, wenn der complement c3 plus der complement c4 kleiner ist als der prior c3 plus der prior c4 mal 0.85; der Wert „Rising“, wenn der complement c3 plus der complement c4 größer ist als der prior c3 plus der prior c4 mal 1.15; andernfalls der Wert „Stable“. | _Rising/Falling/Stable on C3+C4 vs prior (derived)._ |
| Is Pre Nephritic Signature Panel | Wahr, wenn alles Folgende gilt: der anti ds dna trend ist der Wert „Rising“ und der complement trend ist der Wert „Falling“. | _The pre-nephritic serology signature at THIS panel: rising anti-dsDNA + falling complement (the serological trajectory that precedes/tracks overt renal involvement). Emergent from the raw series, not a label anyone assigned. (Proteinuria is the OUTCOME the signature precedes, so it is deliberately NOT part of the signal.)_ |
| Is Significant Proteinuria | Wahr, wenn der proteinuria g per day mindestens 0.5 ist. | _proteinuria >= 0.5 g/day._ |
| Is Nephrotic Range Proteinuria | Wahr, wenn der proteinuria g per day mindestens 3.0 ist. | _proteinuria >= 3.0 g/day._ |
| Sledai Renal Points | Nach Priorität bestimmt: 8, wenn mindestens eines des Folgenden gilt: das is nephrotic range proteinuria-Kennzeichen gesetzt ist oder das has active urinary sediment-Kennzeichen gesetzt ist; 4, wenn das is significant proteinuria-Kennzeichen gesetzt ist; andernfalls 0. | _SLEDAI-style renal sub-score (0/4/8) from proteinuria + sediment._ |
| Sledai Serology Points | Nach Priorität bestimmt: 4, wenn alles Folgende gilt: der complement trend ist der Wert „Falling“ und der anti ds dna trend ist der Wert „Rising“; 2, wenn mindestens eines des Folgenden gilt: der complement trend ist der Wert „Falling“ oder der anti ds dna trend ist der Wert „Rising“; andernfalls 0. | _SLEDAI-style serology sub-score (0/2/4) from low-complement + raised dsDNA._ |
| Sledai Score | Berechnet als der sledai renal points plus der sledai serology points. | _Derived SLEDAI-style activity score = renal + serology points._ |
| Progression State Key | Nach Priorität bestimmt: der Wert „BiopsyIndicated“, wenn mindestens eines des Folgenden gilt: das is nephrotic range proteinuria-Kennzeichen gesetzt ist oder das has active urinary sediment-Kennzeichen gesetzt ist; der Wert „RenalFlareRisk“, wenn der proteinuria g per day mindestens 1.0 ist; der Wert „EarlyNephritis“, wenn das is significant proteinuria-Kennzeichen gesetzt ist; der Wert „SerologicActive“, wenn alles Folgende gilt: der anti ds dna trend ist der Wert „Rising“ und der complement trend ist der Wert „Falling“; andernfalls der Wert „PresymptomaticAutoimmunity“. | _Disease state IMPLIED by THIS panel (derived purely from raw leaves)._ |
| Progression State Order | Nach Priorität bestimmt: 5, wenn der progression state key ist der Wert „BiopsyIndicated“; 4, wenn der progression state key ist der Wert „RenalFlareRisk“; 3, wenn der progression state key ist der Wert „EarlyNephritis“; 2, wenn der progression state key ist der Wert „SerologicActive“; andernfalls 1. | _Numeric severity order of THIS panel's implied state (0..5). Lets the individual's current state be a MAXIFS over panels (highest state reached)._ |
| Relative Path | Berechnet als der Wert „/admin/serology/“, gefolgt von der serology observation ID. | _Path._ |
| **Therapy Option** | Therapy lookup for the treatment-line-selection DAG (MMF/belimumab/anifrolumab/secukinumab). | — |
| Name | Berechnet als der therapy label. | _Echo._ |
| Relative Path | Berechnet als der Wert „/admin/therapy-options/“, gefolgt von der therapy option ID. | _Path._ |

## 2 Faktentypen

- ein **machine state** verweist auf genau einen **state machine**
- ein **machine state** kann auf einen **state transition rule** verweisen
- ein **state transition rule** verweist auf genau einen **state machine**
- ein **state transition rule** verweist auf genau einen **machine state**
- ein **state transition** verweist auf genau einen **state machine**
- ein **subject state instance** verweist auf genau einen **state machine**
- ein **subject state instance** kann auf einen **subject state instance** verweisen
- ein **subject state instance** kann auf einen **state transition** verweisen
- ein **serology observation** verweist auf genau einen **individual**
- ein **serology observation** kann auf einen **serology observation** verweisen

## 2b Erreichbarkeitsregeln

_Eine Erreichbarkeitsregel ist eine transitive Hülle: Beziehungen, die nicht nur
direkt, sondern über jede Kette derselben Beziehung gelten. Die behaupteten Kanten sind
die einzige Quelle der Wahrheit; die abgeleiteten Kanten sind notwendige Folgen daraus._

- **Progression Closure** — ein state transition rule ist von einem anderen über die Beziehung **progression** erreichbar,
  wenn der zweite vom ersten aus durch das Verfolgen einer oder mehrerer **progression**-Kanten
  (von seiner from state zu seinem to state) erreicht werden kann, ob direkt behauptet oder transitiv erreicht.
  - Eine Kante ist **behauptet**, wenn sie direkt im state transition rules existiert; sie ist **abgeleitet**,
    wenn keine direkte Kante sie aussagt, sie aber aus einer Kette behaupteter Kanten folgt.
  - Die **Sprungdistanz** eines erreichbaren Paares ist die Länge der kürzesten solchen Kette
    (1 für eine direkt behauptete Kante).
  - _Transitive closure of state-machine transitions (an owl:TransitiveProperty). The asserted FromState->ToState edges imply the full reachability ordering of each machine - including never-asserted long-range pairs such as PresymptomaticAutoimmunity -> BiopsyIndicated. Materialized by the transpiler as the cycle-safe recursive view vw_state_transition_rules_closure(from_id, to_id, hop_distance, is_inferred): asserted edges (hop 1) plus inferred reachability rows. The disease trajectory is derived from the transition topology, not hand-asserted._

## 3 Operative Regeln

_Operative Regeln drücken aus, was das Geschäft **verpflichtet**, **untersagt** oder
empfiehlt (**sollte**). Strukturelle Regeln stammen aus Pflichtfeldern und Fremdschlüsseln;
semantische Regeln stammen aus der Constraints-Tabelle und stützen sich jeweils auf einen booleschen
Wert, den das Regelbuch bereits berechnet (unten in den definitorischen Regeln als DR-N referenziert)._

### Strukturelle Einschränkungen (aus dem Schema)

- Ein autoimmune diseas **muss** einen disease label haben und erfassen, ob es ist ein complex disease.
- Ein disease stage **muss** einen stage label, einen sort order und einen autoimmune disease haben.
- Ein tissue **muss** einen tissue label haben.
- Ein omics modality **muss** einen modality label haben und erfassen, ob es ist ein single cell.
- Ein federated dataset **muss** einen node label und eine region haben und erfassen, ob es ist privacy preserving.
- Ein variant type **muss** einen type label haben und erfassen, ob es ist ein rare variant class.
- Ein individual **muss** einen given name, einen family name, einen ancestry label und einen age years haben und erfassen, ob es ist ancestry absent from training und ob es hat einen cryptic relatedness flag.
- Ein genomic variant **muss** einen variant label, einen individual, einen variant type und einen allele frequency haben und erfassen, ob es hat einen allele specific expression.
- Ein omics assay **muss** einen assay label, einen individual, einen omics modality und einen measurement error score haben und erfassen, ob es hat einen cell state specific effect.
- Ein evidence item **muss** einen evidence label, einen causal mechanism, einen omics assay, einen effect size und einen standard error haben und erfassen, ob es ist ein cross modality, ob es ist ein negative control arm, ob es ist ein adjusted for ancestry p cs, ob es ist ein adjusted for batch und ob es ist ein synthetic leaf.
- Ein cohort replication **muss** eine replication label, einen causal mechanism, einen federated dataset, eine replication effect sign, eine replication p value und eine replication ancestry label haben.
- Ein negative control test **muss** einen control label, einen causal mechanism, einen test kind, eine permutation effect size und einen null threshold haben.
- Ein environmental exposure **muss** einen exposure label, einen individual und einen exposure level haben und erfassen, ob es ist ein maternal effect.
- Ein treatment **muss** einen treatment label, einen individual, einen autoimmune disease und einen treatment response haben und erfassen, ob es hat einen treatment induced change und ob es hat einen adverse effect.
- Ein clinical phenotype **muss** einen phenotype label, einen individual, einen autoimmune disease und einen severity score haben und erfassen, ob es hat einen immune dysfunction.
- Ein causal mechanism **muss** einen mechanism label, einen individual und einen mechanism type haben und erfassen, ob es hat einen pleiotropy.
- Ein epistatic interaction **muss** eine interaction label, einen individual, einen primary variant, einen secondary variant und einen epistasis score haben und erfassen, ob es hat einen pleiotropy.
- Ein counterfactual trajectory **muss** einen trajectory label, einen individual, einen autoimmune disease, einen projected severity und einen horizon months haben.
- Ein individual prediction **muss** eine prediction label, einen individual, einen autoimmune disease und eine prediction type haben.
- Eine calibration bin **muss** einen bin label, einen individual prediction, einen predicted probability band, einen observed event rate und einen coverage count haben.
- Eine intervention target **muss** einen target label, einen causal mechanism, einen autoimmune disease und einen therapy class haben und erfassen, ob es ist validated.
- Ein axiom **muss** einen statement, einen rationale und einen category haben.
- Ein tests for success **muss** einen claim, einen how witnessed und einen status haben.
- Ein feature **muss** einen title, einen category, einen priority, eine description und einen ref count haben.
- Ein inference kind **muss** einen title, eine description, einen example field, einen evidence count, einen maturity und einen sort order haben.
- Ein open question **muss** eine question und einen context haben und erfassen, ob es ist resolved.
- Ein non goal **muss** einen statement und einen why excluded haben.
- Ein glossary term **muss** einen term und eine definition haben.
- Ein leopold loop **muss** einen loop number, einen title, einen goal, einen status und einen sort order haben.
- Ein state machine **muss** einen subject table name und einen subject state column haben.
- Ein machine state **muss** auf genau einen state machine verweisen.
- Ein machine state **muss** einen state key haben.
- Ein state transition rule **muss** auf genau einen state machine verweisen.
- Ein state transition rule **muss** auf genau einen machine state als seinen from state verweisen.
- Ein state transition rule **muss** auf genau einen machine state als seinen to state verweisen.
- Ein state transition **muss** auf genau einen state machine verweisen.
- Ein state transition **muss** einen subject table name und einen to state key haben.
- Ein subject state instance **muss** auf genau einen state machine verweisen.
- Ein subject state instance **muss** einen subject table name, einen state key und einen sequence index haben.
- Ein disease domain concept **muss** einen concept label und einen modeling status haben.
- Ein serology observation **muss** auf genau einen individual verweisen.
- Ein serology observation **muss** einen observed at und einen sequence index haben.
- Ein therapy option **muss** einen therapy label haben.

## 4 Definitorische Regeln

_Alle Aussagen drücken eine Wahrheit der Geschäftsdomäne aus; sie sind weder
Prozeduren noch Befehle. "genau dann, wenn" wird zugunsten von "nur dann, wenn"
vermieden, damit eine einseitige Notwendigkeit nicht mit einer Äquivalenz verwechselt
wird. Ein **⚠︎ mechanisch**-Chip kennzeichnet eine Regel, deren deterministische
Formulierung getreu, aber holprig ist — ein Hinweis für eine optionale spätere
Umformulierung, kein Fehler._

| ID | Deklarative Regel |
|----|-------------------|
| **DR-1 Name** | Ein autoimmune diseas: Name wird berechnet als der disease label. |
| **DR-2 Relative Path** | Ein autoimmune diseas: Relative path wird berechnet als der Wert „/diseases/“, gefolgt von der autoimmune disease ID. |
| **DR-3 Count of Disease Stages** | Ein autoimmune diseas: Count of disease stages ist die Anzahl der mit dem autoimmune diseas verbundenen disease stages. |
| **DR-4 Count of Intervention Targets** | Ein autoimmune diseas: Count of intervention targets ist die Anzahl der mit dem autoimmune diseas verbundenen intervention targets. |
| **DR-5 Name** | Ein disease stage: Name wird berechnet als der autoimmune disease disease label, gefolgt von der Wert „ — “, gefolgt von der stage label. |
| **DR-6 Parent Path** | Ein disease stage: Parent path ist der relative path des zugehörigen autoimmune disease des disease stage. |
| **DR-7 Relative Path** | Ein disease stage: Relative path wird berechnet als der parent path, gefolgt von der Wert „/stages/“, gefolgt von der disease stage ID. |
| **DR-8 Autoimmune Disease Disease Label** | Ein disease stage: Autoimmune disease disease label ist der disease label des zugehörigen autoimmune disease des disease stage. |
| **DR-9 Is Presymptomatic** | Ein disease stage gilt als ein presymptomatic, wenn der stage label ist der Wert „Presymptomatic“. |
| **DR-10 Name** | Ein tissue: Name wird berechnet als der tissue label. |
| **DR-11 Relative Path** | Ein tissue: Relative path wird berechnet als der Wert „/tissues/“, gefolgt von der tissue ID. |
| **DR-12 Count of Omics Assays** | Ein tissue: Count of omics assays ist die Anzahl der mit dem tissue verbundenen omics assays. |
| **DR-13 Name** | Ein omics modality: Name wird berechnet als der modality label. |
| **DR-14 Relative Path** | Ein omics modality: Relative path wird berechnet als der Wert „/omics-modalities/“, gefolgt von der omics modality ID. |
| **DR-15 Count of Omics Assays** | Ein omics modality: Count of omics assays ist die Anzahl der mit dem omics modality verbundenen omics assays. |
| **DR-16 Name** | Ein federated dataset: Name wird berechnet als der node label. |
| **DR-17 Relative Path** | Ein federated dataset: Relative path wird berechnet als der Wert „/datasets/“, gefolgt von der federated dataset ID. |
| **DR-18 Count of Individuals** | Ein federated dataset: Count of individuals ist die Anzahl der mit dem federated dataset verbundenen individuals. |
| **DR-19 Name** | Ein variant type: Name wird berechnet als der type label. |
| **DR-20 Relative Path** | Ein variant type: Relative path wird berechnet als der Wert „/variant-types/“, gefolgt von der variant type ID. |
| **DR-21 Count of Genomic Variants** | Ein variant type: Count of genomic variants ist die Anzahl der mit dem variant type verbundenen genomic variants. |
| **DR-22 Name** | Ein individual: Name wird berechnet als der given name, gefolgt von ein Leerzeichen, gefolgt von der family name. |
| **DR-23 Slug** | Ein individual: Slug wird berechnet als das kleingeschriebene family name, gefolgt von ein Bindestrich, gefolgt von der given name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-24 Relative Path** | Ein individual: Relative path wird berechnet als der Wert „/intake/new-patient/“, gefolgt von der slug. |
| **DR-25 Federated Dataset Node Label** | Der federated dataset node label des individual wird nach folgender Priorität bestimmt:<br>1. eine leere Zeichenkette, wenn der federated dataset leer ist;<br>2. andernfalls der node label des zugehörigen federated dataset des individual. |
| **DR-26 Count of Genomic Variants** | Ein individual: Count of genomic variants ist die Anzahl der mit dem individual verbundenen genomic variants. |
| **DR-27 Count of Causal Mechanisms** | Ein individual: Count of causal mechanisms ist die Anzahl der mit dem individual verbundenen causal mechanisms. |
| **DR-28 Count of Epistatic Interactions** | Ein individual: Count of epistatic interactions ist die Anzahl der mit dem individual verbundenen epistatic interactions. |
| **DR-29 Rare Variant Burden Score** | Der rare variant burden score des individual wird nach folgender Priorität bestimmt:<br>1. der count of genomic variants geteilt durch der age years, wenn der age years größer ist als 0;<br>2. andernfalls 0. |
| **DR-30 Causal Architecture Score** | Ein individual: Causal architecture score wird berechnet als der count of causal mechanisms mal 10 plus der count of epistatic interactions mal 5 plus der rare variant burden score. |
| **DR-31 Is Development Window** | Ein individual gilt als ein development window, wenn der age years höchstens 25 ist. |
| **DR-32 Is Aging Window** | Ein individual gilt als ein aging window, wenn der age years mindestens 60 ist. |
| **DR-33 Count Confirmed Causal Nodes** | Ein individual: Count confirmed causal nodes ist die Anzahl der causal mechanisms des individual, die causal architecture node sind. |
| **DR-34 Sum Confirmed Causal Confidence** | Ein individual: Sum confirmed causal confidence ist die Gesamtsumme von causal confidence über die causal mechanisms des individual, die causal architecture node sind. |
| **DR-35 Count Cross Ancestry Confirmed Nodes** | Ein individual: Count cross ancestry confirmed nodes ist die Anzahl der causal mechanisms des individual, die ancestry transportable sind. |
| **DR-36 Max Severity Score** | Ein individual: Max severity score ist der größte severity score über die mit dem individual verbundenen clinical phenotypes. |
| **DR-37 Count High Severity Phenotypes** | Ein individual: Count high severity phenotypes ist die Anzahl der clinical phenotypes des individual, die high severity sind. |
| **DR-38 Has High Severity Phenotype** | Ein individual gilt als über einen high severity phenotype verfügend, wenn der count high severity phenotypes mindestens 1 ist. |
| **DR-39 Count Predicted Treatment Responses** | Ein individual: Count predicted treatment responses ist die Anzahl der treatments des individual, die treatment response predicted sind. |
| **DR-40 Has Predicted Treatment Response** | Ein individual gilt als über einen predicted treatment response verfügend, wenn der count predicted treatment responses mindestens 1 ist. |
| **DR-41 Count Serology Panels** | Ein individual: Count serology panels ist die Anzahl der mit dem individual verbundenen serology observations. |
| **DR-42 Count Pre Nephritic Signature Panels** | Ein individual: Count pre nephritic signature panels ist die Anzahl der serology observations des individual, die pre nephritic signature panel sind. |
| **DR-43 Is in Pre Nephritic Signature Cluster** | Ein individual gilt als in pre nephritic signature cluster, wenn der count pre nephritic signature panels mindestens 1 ist. |
| **DR-44 Signature Strength** | Der signature strength des individual wird nach folgender Priorität bestimmt:<br>1. 2, wenn der count pre nephritic signature panels mindestens 2 ist;<br>2. 1, wenn der count pre nephritic signature panels mindestens 1 ist;<br>3. andernfalls 0. |
| **DR-45 Max Progression State Order** | Ein individual: Max progression state order ist der größte progression state order über die mit dem individual verbundenen serology observations. |
| **DR-46 Latest Sledai Score** | Ein individual: Latest sledai score ist der größte sledai score über die mit dem individual verbundenen serology observations. |
| **DR-47 Nephritis Progression State Key** | Der nephritis progression state key des individual wird nach folgender Priorität bestimmt:<br>1. der Wert „BiopsyIndicated“, wenn der max progression state order mindestens 5 ist;<br>2. der Wert „RenalFlareRisk“, wenn der max progression state order mindestens 4 ist;<br>3. der Wert „EarlyNephritis“, wenn der max progression state order mindestens 3 ist;<br>4. der Wert „SerologicActive“, wenn der max progression state order mindestens 2 ist;<br>5. andernfalls der Wert „PresymptomaticAutoimmunity“. |
| **DR-48 Activity Tier** | Der activity tier des individual wird nach folgender Priorität bestimmt:<br>1. der Wert „High / flare“, wenn der latest sledai score mindestens 12 ist;<br>2. der Wert „Moderate“, wenn der latest sledai score mindestens 6 ist;<br>3. der Wert „Mild“, wenn der latest sledai score mindestens 1 ist;<br>4. andernfalls der Wert „Quiescent“. |
| **DR-49 Is High Disease Activity** | Ein individual gilt als ein high disease activity, wenn der latest sledai score mindestens 12 ist. |
| **DR-50 Is Disease Progressing** | Ein individual gilt als disease progressing, wenn mindestens eines des Folgenden gilt: der nephritis progression state key ist der Wert „EarlyNephritis“; der nephritis progression state key ist der Wert „RenalFlareRisk“; oder der nephritis progression state key ist der Wert „BiopsyIndicated“. |
| **DR-51 Target Pathway Code** | Ein individual: Target pathway code ist der größte target pathway code über die mit dem individual verbundenen causal mechanisms. |
| **DR-52 Target Pathway** | Der target pathway des individual wird nach folgender Priorität bestimmt:<br>1. der Wert „type-I-IFN“, wenn der target pathway code ist 1;<br>2. der Wert „B-cell/autoantibody“, wenn der target pathway code ist 2;<br>3. der Wert „T-cell-costim“, wenn der target pathway code ist 3;<br>4. der Wert „IL-17/23“, wenn der target pathway code ist 4;<br>5. andernfalls eine leere Zeichenkette. |
| **DR-53 Reachable States Ahead** | Ein individual: Reachable states ahead ist der reachable state count des zugehörigen current progression state ID des individual. |
| **DR-54 Name** | Ein genomic variant: Name wird berechnet als der variant label. |
| **DR-55 Parent Path** | Ein genomic variant: Parent path ist der relative path des zugehörigen individual des genomic variant. |
| **DR-56 Relative Path** | Ein genomic variant: Relative path wird berechnet als der parent path, gefolgt von der Wert „/variants/“, gefolgt von der genomic variant ID. |
| **DR-57 Variant Type Label** | Ein genomic variant: Variant type label ist der type label des zugehörigen variant type des genomic variant. |
| **DR-58 Variant Class is Rare** | Ein genomic variant: Variant class is rare ist wahr, wenn der zugehörige variant type des genomic variant ist ein rare variant class. |
| **DR-59 Individual Ancestry Label** | Ein genomic variant: Individual ancestry label ist der ancestry label des zugehörigen individual des genomic variant. |
| **DR-60 Is Rare Variant** | Ein genomic variant gilt als ein rare variant, wenn der allele frequency kleiner ist als 0.01. |
| **DR-61 Is Causal Candidate** | Ein genomic variant gilt als ein causal candidate, wenn alles Folgende gilt: mindestens eines des Folgenden gilt: das is rare variant-Kennzeichen gesetzt ist oder das variant class is rare-Kennzeichen gesetzt ist und das has allele specific expression-Kennzeichen gesetzt ist. |
| **DR-62 Name** | Ein omics assay: Name wird berechnet als der assay label. |
| **DR-63 Parent Path** | Ein omics assay: Parent path ist der relative path des zugehörigen individual des omics assay. |
| **DR-64 Relative Path** | Ein omics assay: Relative path wird berechnet als der parent path, gefolgt von der Wert „/assays/“, gefolgt von der omics assay ID. |
| **DR-65 Modality Label** | Ein omics assay: Modality label ist der modality label des zugehörigen omics modality des omics assay. |
| **DR-66 Tissue Label** | Der tissue label des omics assay wird nach folgender Priorität bestimmt:<br>1. der Wert „Missing Tissue“, wenn der tissue leer ist;<br>2. andernfalls der tissue label des zugehörigen tissue des omics assay. |
| **DR-67 Has Batch Effect Risk** | Ein omics assay gilt als über einen batch effect risk verfügend, wenn der measurement error score größer ist als 0.3. |
| **DR-68 Is High Quality Assay** | Ein omics assay gilt als ein high quality assay, wenn alles Folgende gilt: es nicht der Fall ist, dass das has batch effect risk-Kennzeichen gesetzt ist und der measurement error score kleiner ist als 0.15. |
| **DR-69 Name** | Ein evidence item: Name wird berechnet als der evidence label. |
| **DR-70 Parent Path** | Ein evidence item: Parent path ist der relative path des zugehörigen causal mechanism des evidence item. |
| **DR-71 Relative Path** | Ein evidence item: Relative path wird berechnet als der parent path, gefolgt von der Wert „/evidence/“, gefolgt von der evidence item ID. |
| **DR-72 Assay is High Quality** | Ein evidence item: Assay is high quality ist wahr, wenn der zugehörige omics assay des evidence item ist ein high quality assay. |
| **DR-73 Z Stat** | Der z stat des evidence item wird nach folgender Priorität bestimmt:<br>1. der effect size geteilt durch der standard error, wenn der standard error größer ist als 0;<br>2. andernfalls 0. |
| **DR-74 Is Confound Controlled** | Ein evidence item gilt als confound controlled, wenn alles Folgende gilt: das is adjusted for ancestry p cs-Kennzeichen gesetzt ist und das is adjusted for batch-Kennzeichen gesetzt ist. |
| **DR-75 Is Qualified Evidence** | Ein evidence item gilt als ein qualified evidence, wenn alles Folgende gilt: das assay is high quality-Kennzeichen gesetzt ist; es nicht der Fall ist, dass das is negative control arm-Kennzeichen gesetzt ist; der z stat mindestens 2 ist; und das is confound controlled-Kennzeichen gesetzt ist. |
| **DR-76 Name** | Ein cohort replication: Name wird berechnet als der replication label. |
| **DR-77 Parent Path** | Ein cohort replication: Parent path ist der relative path des zugehörigen causal mechanism des cohort replication. |
| **DR-78 Relative Path** | Ein cohort replication: Relative path wird berechnet als der parent path, gefolgt von der Wert „/replications/“, gefolgt von der cohort replication ID. |
| **DR-79 Replicated At Nominal Sig** | Ein cohort replication ist markiert als replicated at nominal sig, wenn alles Folgende gilt: der replication p value höchstens 0.05 ist und der replication effect sign ist 1. |
| **DR-80 Mechanism Primary Ancestry** | Ein cohort replication: Mechanism primary ancestry ist der individual ancestry label des zugehörigen causal mechanism des cohort replication. |
| **DR-81 Is Different Ancestry Replication** | Ein cohort replication gilt als ein different ancestry replication, wenn es nicht der Fall ist, dass der replication ancestry label ist der mechanism primary ancestry. |
| **DR-82 Is Cross Ancestry Concordant** | Ein cohort replication gilt als ein cross ancestry concordant, wenn alles Folgende gilt: das replicated at nominal sig-Kennzeichen gesetzt ist und das is different ancestry replication-Kennzeichen gesetzt ist. |
| **DR-83 Name** | Ein negative control test: Name wird berechnet als der control label. |
| **DR-84 Parent Path** | Ein negative control test: Parent path ist der relative path des zugehörigen causal mechanism des negative control test. |
| **DR-85 Relative Path** | Ein negative control test: Relative path wird berechnet als der parent path, gefolgt von der Wert „/neg-controls/“, gefolgt von der negative control test ID. |
| **DR-86 Is Survived** | Ein negative control test gilt als survived, wenn der permutation effect size höchstens der null threshold ist. |
| **DR-87 Name** | Ein environmental exposure: Name wird berechnet als der exposure label. |
| **DR-88 Parent Path** | Ein environmental exposure: Parent path ist der relative path des zugehörigen individual des environmental exposure. |
| **DR-89 Relative Path** | Ein environmental exposure: Relative path wird berechnet als der parent path, gefolgt von der Wert „/exposures/“, gefolgt von der environmental exposure ID. |
| **DR-90 Individual Ancestry Label** | Ein environmental exposure: Individual ancestry label ist der ancestry label des zugehörigen individual des environmental exposure. |
| **DR-91 Is High Exposure** | Ein environmental exposure gilt als ein high exposure, wenn der exposure level größer ist als 5. |
| **DR-92 Name** | Ein treatment: Name wird berechnet als der treatment label. |
| **DR-93 Parent Path** | Ein treatment: Parent path ist der relative path des zugehörigen individual des treatment. |
| **DR-94 Relative Path** | Ein treatment: Relative path wird berechnet als der parent path, gefolgt von der Wert „/treatments/“, gefolgt von der treatment ID. |
| **DR-95 Autoimmune Disease Label** | Ein treatment: Autoimmune disease label ist der disease label des zugehörigen autoimmune disease des treatment. |
| **DR-96 Is Effective Treatment** | Ein treatment gilt als ein effective treatment, wenn alles Folgende gilt: mindestens eines des Folgenden gilt: der treatment response ist der Wert „Complete“ oder der treatment response ist der Wert „Partial“ und es nicht der Fall ist, dass das has adverse effect-Kennzeichen gesetzt ist. |
| **DR-97 Is Mechanism Matched** | Ein treatment: Is mechanism matched ist falsch, wenn der targets mechanism leer ist, andernfalls der is causal architecture node des zugehörigen targets mechanism des treatment. |
| **DR-98 Is Treatment Response Predicted** | Ein treatment gilt als treatment response predicted, wenn alles Folgende gilt: das is effective treatment-Kennzeichen gesetzt ist und das is mechanism matched-Kennzeichen gesetzt ist. |
| **DR-99 Treatment Response Deciding Factor** | Der treatment response deciding factor des treatment wird nach folgender Priorität bestimmt:<br>1. der Wert „EffectiveOnConfirmedMechanism“, wenn das is treatment response predicted-Kennzeichen gesetzt ist;<br>2. der Wert „NoConfirmedMechanism“, wenn es nicht der Fall ist, dass das is mechanism matched-Kennzeichen gesetzt ist;<br>3. der Wert „AdverseEffect“, wenn das has adverse effect-Kennzeichen gesetzt ist;<br>4. der Wert „NoResponse“, wenn mindestens eines des Folgenden gilt: der treatment response ist der Wert „None“ oder der treatment response ist der Wert „Adverse“;<br>5. andernfalls der Wert „Undetermined“. |
| **DR-100 Name** | Ein clinical phenotype: Name wird berechnet als der phenotype label. |
| **DR-101 Parent Path** | Ein clinical phenotype: Parent path ist der relative path des zugehörigen individual des clinical phenotype. |
| **DR-102 Relative Path** | Ein clinical phenotype: Relative path wird berechnet als der parent path, gefolgt von der Wert „/phenotypes/“, gefolgt von der clinical phenotype ID. |
| **DR-103 Disease Stage Label** | Der disease stage label des clinical phenotype wird nach folgender Priorität bestimmt:<br>1. eine leere Zeichenkette, wenn der disease stage leer ist;<br>2. andernfalls der stage label des zugehörigen disease stage des clinical phenotype. |
| **DR-104 Is High Severity** | Ein clinical phenotype gilt als ein high severity, wenn der severity score größer ist als 7. |
| **DR-105 Is Presymptomatic Phenotype** | Ein clinical phenotype gilt als ein presymptomatic phenotype, wenn der disease stage label ist der Wert „Presymptomatic“. |
| **DR-106 Target Pathway Code** | Der target pathway code des causal mechanism wird nach folgender Priorität bestimmt:<br>1. 1, wenn der target pathway ist der Wert „type-I-IFN“;<br>2. 2, wenn der target pathway ist der Wert „B-cell/autoantibody“;<br>3. 3, wenn der target pathway ist der Wert „T-cell-costim“;<br>4. 4, wenn der target pathway ist der Wert „IL-17/23“;<br>5. andernfalls 0. |
| **DR-107 Name** | Ein causal mechanism: Name wird berechnet als der mechanism label. |
| **DR-108 Parent Path** | Ein causal mechanism: Parent path ist der relative path des zugehörigen individual des causal mechanism. |
| **DR-109 Relative Path** | Ein causal mechanism: Relative path wird berechnet als der parent path, gefolgt von der Wert „/mechanisms/“, gefolgt von der causal mechanism ID. |
| **DR-110 Individual Ancestry Label** | Ein causal mechanism: Individual ancestry label ist der ancestry label des zugehörigen individual des causal mechanism. |
| **DR-111 Count Qualified Evidence** | Ein causal mechanism: Count qualified evidence ist die Anzahl der evidence items des causal mechanism, die qualified evidence sind. |
| **DR-112 Count Modalities Supporting** | Ein causal mechanism: Count modalities supporting ist die Anzahl der evidence items des causal mechanism, die cross modality sind und qualified evidence sind. |
| **DR-113 Count Intervention Targets** | Ein causal mechanism: Count intervention targets ist die Anzahl der mit dem causal mechanism verbundenen intervention targets. |
| **DR-114 Is Experimentally Falsifiable** | Ein causal mechanism gilt als ein experimentally falsifiable, wenn alles Folgende gilt: der count intervention targets mindestens 1 ist und der count qualified evidence mindestens 1 ist. |
| **DR-115 Count Replications** | Ein causal mechanism: Count replications ist die Anzahl der mit dem causal mechanism verbundenen cohort replications. |
| **DR-116 Count Concordant Replications** | Ein causal mechanism: Count concordant replications ist die Anzahl der cohort replications des causal mechanism, die replicated at nominal sig sind. |
| **DR-117 Count Cross Ancestry Concordant** | Ein causal mechanism: Count cross ancestry concordant ist die Anzahl der cohort replications des causal mechanism, die cross ancestry concordant sind. |
| **DR-118 Replication Fraction** | Der replication fraction des causal mechanism wird nach folgender Priorität bestimmt:<br>1. der count concordant replications geteilt durch der count replications, wenn der count replications größer ist als 0;<br>2. andernfalls 0. |
| **DR-119 Replicates Across Cohorts** | Ein causal mechanism gilt als across cohorts replicates erfüllend, wenn alles Folgende gilt: der count replications mindestens 2 ist und der count concordant replications mindestens 2 ist. |
| **DR-120 Count Neg Control Tests** | Ein causal mechanism: Count neg control tests ist die Anzahl der mit dem causal mechanism verbundenen negative control tests. |
| **DR-121 Count Neg Control Survived** | Ein causal mechanism: Count neg control survived ist die Anzahl der negative control tests des causal mechanism, die survived sind. |
| **DR-122 Survives Negative Controls** | Ein causal mechanism gilt als negative controls survives erfüllend, wenn alles Folgende gilt: der count neg control tests mindestens 1 ist und der count neg control survived ist der count neg control tests. |
| **DR-123 Is Spurious Derived** | Ein causal mechanism gilt als spurious derived, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das replicates across cohorts-Kennzeichen gesetzt ist; es nicht der Fall ist, dass das survives negative controls-Kennzeichen gesetzt ist; der count modalities supporting kleiner ist als 2; oder das has pleiotropy-Kennzeichen gesetzt ist. |
| **DR-124 Causal Confidence** | Der causal confidence des causal mechanism wird nach folgender Priorität bestimmt:<br>1. 1, wenn 0.30 mal 1, wenn der count qualified evidence mindestens 4 ist, andernfalls der count qualified evidence geteilt durch 4 plus 0.20 mal 1, wenn der count modalities supporting mindestens 3 ist, andernfalls der count modalities supporting geteilt durch 3 plus 0.30 mal der replication fraction plus 0.20 mal 1, wenn das survives negative controls-Kennzeichen gesetzt ist, andernfalls 0 größer ist als 1;<br>2. andernfalls 0.30 mal 1, wenn der count qualified evidence mindestens 4 ist, andernfalls der count qualified evidence geteilt durch 4 plus 0.20 mal 1, wenn der count modalities supporting mindestens 3 ist, andernfalls der count modalities supporting geteilt durch 3 plus 0.30 mal der replication fraction plus 0.20 mal 1, wenn das survives negative controls-Kennzeichen gesetzt ist, andernfalls 0. |
| **DR-125 Variant is Causal Candidate** | Ein causal mechanism: Variant is causal candidate ist falsch, wenn der genomic variant leer ist, andernfalls der is causal candidate des zugehörigen genomic variant des causal mechanism. |
| **DR-126 Is Causal Architecture Node** | Ein causal mechanism gilt als ein causal architecture node, wenn alles Folgende gilt: der causal confidence mindestens 0.7 ist; das is experimentally falsifiable-Kennzeichen gesetzt ist; es nicht der Fall ist, dass das is spurious derived-Kennzeichen gesetzt ist; und mindestens eines des Folgenden gilt: das variant is causal candidate-Kennzeichen gesetzt ist oder der environmental exposure einen Wert hat. |
| **DR-127 Is Ancestry Transportable** | Ein causal mechanism gilt als ein ancestry transportable, wenn alles Folgende gilt: das is causal architecture node-Kennzeichen gesetzt ist und der count cross ancestry concordant mindestens 1 ist. |
| **DR-128 Name** | Ein epistatic interaction: Name wird berechnet als der interaction label. |
| **DR-129 Parent Path** | Ein epistatic interaction: Parent path ist der relative path des zugehörigen individual des epistatic interaction. |
| **DR-130 Relative Path** | Ein epistatic interaction: Relative path wird berechnet als der parent path, gefolgt von der Wert „/epistasis/“, gefolgt von der epistatic interaction ID. |
| **DR-131 Is High Order Epistasis** | Ein epistatic interaction gilt als ein high order epistasis, wenn der epistasis score größer ist als 0.5. |
| **DR-132 Name** | Ein counterfactual trajectory: Name wird berechnet als der trajectory label. |
| **DR-133 Parent Path** | Ein counterfactual trajectory: Parent path ist der relative path des zugehörigen individual des counterfactual trajectory. |
| **DR-134 Relative Path** | Ein counterfactual trajectory: Relative path wird berechnet als der parent path, gefolgt von der Wert „/trajectories/“, gefolgt von der counterfactual trajectory ID. |
| **DR-135 Autoimmune Disease Label** | Ein counterfactual trajectory: Autoimmune disease label ist der disease label des zugehörigen autoimmune disease des counterfactual trajectory. |
| **DR-136 Is Worsening Trajectory** | Ein counterfactual trajectory gilt als ein worsening trajectory, wenn der projected severity größer ist als 7. |
| **DR-137 Name** | Ein individual prediction: Name wird berechnet als der prediction label. |
| **DR-138 Parent Path** | Ein individual prediction: Parent path ist der relative path des zugehörigen individual des individual prediction. |
| **DR-139 Relative Path** | Ein individual prediction: Relative path wird berechnet als der parent path, gefolgt von der Wert „/predictions/“, gefolgt von der individual prediction ID. |
| **DR-140 Individual Ancestry Label** | Ein individual prediction: Individual ancestry label ist der ancestry label des zugehörigen individual des individual prediction. |
| **DR-141 Is Ancestry Holdout** | Ein individual prediction: Is ancestry holdout ist wahr, wenn der zugehörige individual des individual prediction ist ancestry absent from training. |
| **DR-142 Individual Causal Mass** | Ein individual prediction: Individual causal mass ist 0, wenn der individual leer ist, andernfalls der sum confirmed causal confidence des zugehörigen individual des individual prediction. |
| **DR-143 Individual Confirmed Node Count** | Ein individual prediction: Individual confirmed node count ist 0, wenn der individual leer ist, andernfalls der count confirmed causal nodes des zugehörigen individual des individual prediction. |
| **DR-144 Individual Cross Ancestry Node Count** | Ein individual prediction: Individual cross ancestry node count ist 0, wenn der individual leer ist, andernfalls der count cross ancestry confirmed nodes des zugehörigen individual des individual prediction. |
| **DR-145 Individual Has Cryptic Relatedness** | Ein individual prediction: Individual has cryptic relatedness ist falsch, wenn der individual leer ist, andernfalls der has cryptic relatedness flag des zugehörigen individual des individual prediction. |
| **DR-146 Individual Max Severity Score** | Ein individual prediction: Individual max severity score ist 0, wenn der individual leer ist, andernfalls der max severity score des zugehörigen individual des individual prediction. |
| **DR-147 Individual Has High Severity Phenotype** | Ein individual prediction: Individual has high severity phenotype ist falsch, wenn der individual leer ist, andernfalls der has high severity phenotype des zugehörigen individual des individual prediction. |
| **DR-148 Individual Has Predicted Treatment Response** | Ein individual prediction: Individual has predicted treatment response ist falsch, wenn der individual leer ist, andernfalls der has predicted treatment response des zugehörigen individual des individual prediction. |
| **DR-149 Predicted Value** | Der predicted value des individual prediction wird nach folgender Priorität bestimmt:<br>1. 10, wenn 2 mal der individual causal mass plus 1.5 mal der individual confirmed node count größer ist als 10;<br>2. andernfalls 2 mal der individual causal mass plus 1.5 mal der individual confirmed node count. |
| **DR-150 Count Bins** | Ein individual prediction: Count bins ist die Anzahl der mit dem individual prediction verbundenen calibration bins. |
| **DR-151 Count Well Calibrated Bins** | Ein individual prediction: Count well calibrated bins ist die Anzahl der calibration bins des individual prediction, die well calibrated bin sind. |
| **DR-152 Sum Bin Abs Error** | Ein individual prediction: Sum bin abs error ist die Gesamtsumme von bin abs error über die mit dem individual prediction verbundenen calibration bins. |
| **DR-153 Mean Bin Abs Error** | Der mean bin abs error des individual prediction wird nach folgender Priorität bestimmt:<br>1. der sum bin abs error geteilt durch der count bins, wenn der count bins größer ist als 0;<br>2. andernfalls 1. |
| **DR-154 Well Calibrated Fraction** | Der well calibrated fraction des individual prediction wird nach folgender Priorität bestimmt:<br>1. der count well calibrated bins geteilt durch der count bins, wenn der count bins größer ist als 0;<br>2. andernfalls 0. |
| **DR-155 Calibrated Uncertainty** | Ein individual prediction: Calibrated uncertainty wird berechnet als 0, wenn 1 minus der mean bin abs error kleiner ist als 0, andernfalls 1 minus der mean bin abs error mal der well calibrated fraction. |
| **DR-156 Rests on Confirmed Mechanism** | Ein individual prediction gilt als on confirmed mechanism rests erfüllend, wenn der individual confirmed node count mindestens 1 ist. |
| **DR-157 Has Spurious Correlation Flag** | Ein individual prediction gilt als über einen spurious correlation flag verfügend, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist oder das individual has cryptic relatedness-Kennzeichen gesetzt ist. |
| **DR-158 Is Falsifiability Backed** | Ein individual prediction gilt als falsifiability backed, wenn der individual confirmed node count mindestens 1 ist. |
| **DR-159 Is Transportable to Absent Ancestry** | Ein individual prediction gilt als ein transportable to absent ancestry, wenn alles Folgende gilt: das is ancestry holdout-Kennzeichen gesetzt ist; der individual cross ancestry node count mindestens 1 ist; und es nicht der Fall ist, dass das has spurious correlation flag gesetzt ist. |
| **DR-160 Is Ancestry Transport Safe** | Ein individual prediction gilt als ein ancestry transport safe, wenn der is transportable to absent ancestry, wenn das is ancestry holdout-Kennzeichen gesetzt ist, andernfalls wahr. |
| **DR-161 Transport Gate Status** | Der transport gate status des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „NotApplicable“, wenn es nicht der Fall ist, dass das is ancestry holdout-Kennzeichen gesetzt ist;<br>2. der Wert „PASS-tested“, wenn das is transportable to absent ancestry-Kennzeichen gesetzt ist;<br>3. andernfalls der Wert „FAIL“. |
| **DR-162 Is High Confidence Prediction** | Ein individual prediction gilt als ein high confidence prediction, wenn alles Folgende gilt: der calibrated uncertainty mindestens 0.7 ist und es nicht der Fall ist, dass das has spurious correlation flag gesetzt ist. |
| **DR-163 Patient Stratification Tier** | Der patient stratification tier des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „High-Risk Pathway“, wenn der predicted value mindestens 7 ist;<br>2. der Wert „Moderate-Risk Pathway“, wenn der predicted value mindestens 4 ist;<br>3. andernfalls der Wert „Low-Risk Pathway“. |
| **DR-164 Predicted Severity Value** | Ein individual prediction: Predicted severity value wird berechnet als der individual max severity score. |
| **DR-165 Severity Tier** | Der severity tier des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „Severe“, wenn der predicted severity value größer ist als 7;<br>2. der Wert „Moderate“, wenn der predicted severity value mindestens 4 ist;<br>3. andernfalls der Wert „Mild“. |
| **DR-166 Is Severity Actionable** | Ein individual prediction gilt als ein severity actionable, wenn alles Folgende gilt: das individual has high severity phenotype-Kennzeichen gesetzt ist; das rests on confirmed mechanism-Kennzeichen gesetzt ist; und es nicht der Fall ist, dass das has spurious correlation flag gesetzt ist. |
| **DR-167 Severity Deciding Factor** | Der severity deciding factor des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „HighSeverityOnConfirmedMechanism“, wenn das is severity actionable-Kennzeichen gesetzt ist;<br>2. der Wert „NotHighSeverity“, wenn es nicht der Fall ist, dass das individual has high severity phenotype-Kennzeichen gesetzt ist;<br>3. der Wert „NoValidatedMechanism“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist;<br>4. der Wert „SpuriousFlag“, wenn das has spurious correlation flag gesetzt ist;<br>5. andernfalls der Wert „Undetermined“. |
| **DR-168 Is Treatment Response Actionable** | Ein individual prediction gilt als ein treatment response actionable, wenn das individual has predicted treatment response-Kennzeichen gesetzt ist. |
| **DR-169 Treatment Response Deciding Factor** | Der treatment response deciding factor des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „EffectiveOnConfirmedMechanism“, wenn das is treatment response actionable-Kennzeichen gesetzt ist;<br>2. der Wert „NoEffectiveTreatmentOnMechanism“, wenn das rests on confirmed mechanism-Kennzeichen gesetzt ist;<br>3. andernfalls der Wert „NoConfirmedMechanism“. |
| **DR-170 Is Clinically Actionable** | Ein individual prediction gilt als ein clinically actionable, wenn alles Folgende gilt: das is high confidence prediction-Kennzeichen gesetzt ist; das is falsifiability backed-Kennzeichen gesetzt ist; das is ancestry transport safe-Kennzeichen gesetzt ist; und der predicted value größer ist als 0. |
| **DR-171 Lifecycle State Key** | Der lifecycle state key des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „Actionable“, wenn alles Folgende gilt: das is high confidence prediction-Kennzeichen gesetzt ist; das is falsifiability backed-Kennzeichen gesetzt ist; das is ancestry transport safe-Kennzeichen gesetzt ist; und der predicted value größer ist als 0;<br>2. der Wert „NotActionable“, wenn mindestens eines des Folgenden gilt: es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist oder es nicht der Fall ist, dass das is falsifiability backed-Kennzeichen gesetzt ist;<br>3. der Wert „NotActionable“, wenn das individual has cryptic relatedness-Kennzeichen gesetzt ist;<br>4. der Wert „NotActionable“, wenn der calibrated uncertainty kleiner ist als 0.7;<br>5. der Wert „NotActionable“, wenn es nicht der Fall ist, dass das is ancestry transport safe-Kennzeichen gesetzt ist;<br>6. andernfalls der Wert „Actionable“. |
| **DR-172 Deciding Gate** | Der deciding gate des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „AllGatesPass“, wenn das is clinically actionable-Kennzeichen gesetzt ist;<br>2. der Wert „NoValidatedMechanism“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist;<br>3. der Wert „CrypticRelatedness“, wenn das individual has cryptic relatedness-Kennzeichen gesetzt ist;<br>4. der Wert „Calibration“, wenn der calibrated uncertainty kleiner ist als 0.7;<br>5. der Wert „AncestryTransport“, wenn es nicht der Fall ist, dass das is ancestry transport safe-Kennzeichen gesetzt ist;<br>6. andernfalls der Wert „Undetermined“. |
| **DR-173 Individual Target Pathway** | Ein individual prediction: Individual target pathway ist der target pathway des zugehörigen individual des individual prediction. |
| **DR-174 Individual Progression State Key** | Ein individual prediction: Individual progression state key ist der nephritis progression state key des zugehörigen individual des individual prediction. |
| **DR-175 Individual is Disease Progressing** | Ein individual prediction: Individual is disease progressing ist wahr, wenn der zugehörige individual des individual prediction ist disease progressing. |
| **DR-176 Recommended Treatment Line** | Der recommended treatment line des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „No targeted line — mechanism unconfirmed“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist;<br>2. der Wert „Mycophenolate (induction)“, wenn mindestens eines des Folgenden gilt: der individual progression state key ist der Wert „RenalFlareRisk“ oder der individual progression state key ist der Wert „BiopsyIndicated“;<br>3. der Wert „Anifrolumab“, wenn der individual target pathway ist der Wert „type-I-IFN“;<br>4. der Wert „Belimumab“, wenn der individual target pathway ist der Wert „B-cell/autoantibody“;<br>5. der Wert „Secukinumab“, wenn der individual target pathway ist der Wert „IL-17/23“;<br>6. andernfalls der Wert „Standard-of-care (no mechanism-matched targeted line)“. |
| **DR-177 Treatment Line Deciding Factor** | Der treatment line deciding factor des individual prediction wird nach folgender Priorität bestimmt:<br>1. der Wert „MechanismUnconfirmed“, wenn es nicht der Fall ist, dass das rests on confirmed mechanism-Kennzeichen gesetzt ist;<br>2. der Wert „ActiveNephritis-Induction“, wenn mindestens eines des Folgenden gilt: der individual progression state key ist der Wert „RenalFlareRisk“ oder der individual progression state key ist der Wert „BiopsyIndicated“;<br>3. der Wert „IFNSignature-Anifrolumab“, wenn der individual target pathway ist der Wert „type-I-IFN“;<br>4. der Wert „AutoantibodyDriven-Belimumab“, wenn der individual target pathway ist der Wert „B-cell/autoantibody“;<br>5. der Wert „IL17Axis-Secukinumab“, wenn der individual target pathway ist der Wert „IL-17/23“;<br>6. andernfalls der Wert „NoMechanismMatch“. |
| **DR-178 Progression Vs Actionability Disagree** | Ein individual prediction ist markiert als progression vs actionability disagree, wenn alles Folgende gilt: das individual is disease progressing-Kennzeichen gesetzt ist und es nicht der Fall ist, dass das is clinically actionable-Kennzeichen gesetzt ist. |
| **DR-179 Name** | Eine calibration bin: Name wird berechnet als der bin label. |
| **DR-180 Parent Path** | Eine calibration bin: Parent path ist der relative path des zugehörigen individual prediction des calibration bin. |
| **DR-181 Relative Path** | Eine calibration bin: Relative path wird berechnet als der parent path, gefolgt von der Wert „/bins/“, gefolgt von der calibration bin ID. |
| **DR-182 Bin Abs Error** | Der bin abs error des calibration bin wird nach folgender Priorität bestimmt:<br>1. der predicted probability band minus der observed event rate, wenn der predicted probability band mindestens der observed event rate ist;<br>2. andernfalls der observed event rate minus der predicted probability band. |
| **DR-183 Is Well Calibrated Bin** | Eine calibration bin gilt als ein well calibrated bin, wenn alles Folgende gilt: der coverage count mindestens 20 ist und der bin abs error höchstens 0.1 ist. |
| **DR-184 Name** | Eine intervention target: Name wird berechnet als der target label. |
| **DR-185 Parent Path** | Eine intervention target: Parent path ist der relative path des zugehörigen causal mechanism des intervention target. |
| **DR-186 Relative Path** | Eine intervention target: Relative path wird berechnet als der parent path, gefolgt von der Wert „/targets/“, gefolgt von der intervention target ID. |
| **DR-187 Causal Mechanism Label** | Eine intervention target: Causal mechanism label ist der mechanism label des zugehörigen causal mechanism des intervention target. |
| **DR-188 Is Gene Based Therapy** | Eine intervention target gilt als ein gene based therapy, wenn der therapy class ist der Wert „Gene-based“. |
| **DR-189 Is Cell Based Therapy** | Eine intervention target gilt als ein cell based therapy, wenn der therapy class ist der Wert „Cell-based“. |
| **DR-190 Name** | Ein axiom: Name wird berechnet als der statement. |
| **DR-191 Relative Path** | Ein axiom: Relative path wird berechnet als der Wert „/admin/axioms/“, gefolgt von der axiom ID. |
| **DR-192 Name** | Ein tests for success: Name wird berechnet als der claim. |
| **DR-193 Relative Path** | Ein tests for success: Relative path wird berechnet als der Wert „/admin/tests-for-success/“, gefolgt von der test for success ID. |
| **DR-194 Name** | Ein feature: Name wird berechnet als der title. |
| **DR-195 Relative Path** | Ein feature: Relative path wird berechnet als der Wert „/admin/features/“, gefolgt von der feature ID. |
| **DR-196 Meta Line** | Ein feature: Meta line wird berechnet als der Wert „**Category:** “, gefolgt von der category, gefolgt von der Wert „ - **Priority:** “, gefolgt von der priority, gefolgt von der Wert „ - **Challenge refs:** “, gefolgt von der ref count. |
| **DR-197 Name** | Ein inference kind: Name wird berechnet als der title. |
| **DR-198 Relative Path** | Ein inference kind: Relative path wird berechnet als der Wert „/admin/inference-kinds/“, gefolgt von der inference kind ID. |
| **DR-199 Name** | Ein open question: Name wird berechnet als der question. |
| **DR-200 Relative Path** | Ein open question: Relative path wird berechnet als der Wert „/admin/open-questions/“, gefolgt von der open question ID. |
| **DR-201 Name** | Ein non goal: Name wird berechnet als der statement. |
| **DR-202 Relative Path** | Ein non goal: Relative path wird berechnet als der Wert „/admin/non-goals/“, gefolgt von der non goal ID. |
| **DR-203 Name** | Ein glossary term: Name wird berechnet als der term. |
| **DR-204 Relative Path** | Ein glossary term: Relative path wird berechnet als der Wert „/admin/glossary/“, gefolgt von der glossary term ID. |
| **DR-205 Name** | Ein leopold loop: Name wird berechnet als der Wert „Loop “, gefolgt von der loop number, gefolgt von der Wert „ — “, gefolgt von der title. |
| **DR-206 Relative Path** | Ein leopold loop: Relative path wird berechnet als der Wert „/admin/leopold-loops/“, gefolgt von der leopold loop ID. |
| **DR-207 Completedness** | Ein leopold loop: Completedness wird berechnet als der status. |
| **DR-208 Is in Current Plan** | Ein leopold loop gilt als in current plan, wenn es nicht der Fall ist, dass der status ist der Wert „done“. |
| **DR-209 Name** | Ein routing and navigation: Name wird berechnet als das kleingeschriebene display name, wobei jedes ein Leerzeichen durch ein Bindestrich ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-210 Admin Can Create** | Ein routing and navigation ist markiert als admin can create, wenn der admin CRUD der Wert „C“ erwähnt. |
| **DR-211 Admin Can Read** | Ein routing and navigation ist markiert als admin can read, wenn der admin CRUD der Wert „R“ erwähnt. |
| **DR-212 Admin Can Update** | Ein routing and navigation ist markiert als admin can update, wenn der admin CRUD der Wert „U“ erwähnt. |
| **DR-213 Admin Can Delete** | Ein routing and navigation ist markiert als admin can delete, wenn der admin CRUD der Wert „D“ erwähnt. |
| **DR-214 Intake Clinician Can Create** | Ein routing and navigation ist markiert als intake clinician can create, wenn der intake clinician CRUD der Wert „C“ erwähnt. |
| **DR-215 Intake Clinician Can Read** | Ein routing and navigation ist markiert als intake clinician can read, wenn der intake clinician CRUD der Wert „R“ erwähnt. |
| **DR-216 Intake Clinician Can Update** | Ein routing and navigation ist markiert als intake clinician can update, wenn der intake clinician CRUD der Wert „U“ erwähnt. |
| **DR-217 Intake Clinician Can Delete** | Ein routing and navigation ist markiert als intake clinician can delete, wenn der intake clinician CRUD der Wert „D“ erwähnt. |
| **DR-218 Diagnosing Doctor Can Create** | Ein routing and navigation ist markiert als diagnosing doctor can create, wenn der diagnosing doctor CRUD der Wert „C“ erwähnt. |
| **DR-219 Diagnosing Doctor Can Read** | Ein routing and navigation ist markiert als diagnosing doctor can read, wenn der diagnosing doctor CRUD der Wert „R“ erwähnt. |
| **DR-220 Diagnosing Doctor Can Update** | Ein routing and navigation ist markiert als diagnosing doctor can update, wenn der diagnosing doctor CRUD der Wert „U“ erwähnt. |
| **DR-221 Diagnosing Doctor Can Delete** | Ein routing and navigation ist markiert als diagnosing doctor can delete, wenn der diagnosing doctor CRUD der Wert „D“ erwähnt. |
| **DR-222 External Llm Can Create** | Ein routing and navigation ist markiert als external llm can create, wenn der external llm CRUD der Wert „C“ erwähnt. |
| **DR-223 External Llm Can Read** | Ein routing and navigation ist markiert als external llm can read, wenn der external llm CRUD der Wert „R“ erwähnt. |
| **DR-224 External Llm Can Update** | Ein routing and navigation ist markiert als external llm can update, wenn der external llm CRUD der Wert „U“ erwähnt. |
| **DR-225 External Llm Can Delete** | Ein routing and navigation ist markiert als external llm can delete, wenn der external llm CRUD der Wert „D“ erwähnt. |
| **DR-226 Depth** | Der depth des routing and navigation wird nach folgender Priorität bestimmt:<br>1. 0, wenn der parent route key leer ist;<br>2. andernfalls die Länge von der route key minus die Länge von der route key, wobei jedes ein Punkt durch eine leere Zeichenkette ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-227 Full Path** | Ein routing and navigation: Full path wird berechnet als der route. |
| **DR-228 Handler Base Name** | Ein routing and navigation: Handler base name wird berechnet als der route key, wobei jedes ein Punkt durch ein Leerzeichen ersetzt wird, wobei jedes ein Bindestrich durch ein Leerzeichen ersetzt wird. ⚠︎ mechanisch <!-- rulespeak:reword --> |
| **DR-229 Relative Path** | Ein routing and navigation: Relative path wird berechnet als der Wert „/admin/routing/“, gefolgt von der routing and navigation ID. |
| **DR-230 Relative Path** | Ein state machine: Relative path wird berechnet als der Wert „/admin/state-machine/“, gefolgt von der state machine ID. |
| **DR-231 State Count** | Ein state machine: State count ist die Anzahl der mit dem state machine verbundenen machine states. |
| **DR-232 Transition Rule Count** | Ein state machine: Transition rule count ist die Anzahl der mit dem state machine verbundenen state transition rules. |
| **DR-233 Relative Path** | Ein machine state: Relative path wird berechnet als der Wert „/admin/state-machine/states/“, gefolgt von der machine state ID. |
| **DR-234 Reachable State Count** | Ein machine state: Reachable state count ist die Anzahl der mit dem machine state verbundenen vw state transition rules closure. |
| **DR-235 Relative Path** | Ein state transition rule: Relative path wird berechnet als der Wert „/admin/state-machine/rules/“, gefolgt von der state transition rule ID. |
| **DR-236 From State Key** | Ein state transition rule: From state key ist der state key des zugehörigen from state des state transition rule. |
| **DR-237 To State Key** | Ein state transition rule: To state key ist der state key des zugehörigen to state des state transition rule. |
| **DR-238 Is Forward Edge** | Ein state transition rule gilt als ein forward edge, wenn es nicht der Fall ist, dass der to state key ist der from state key. |
| **DR-239 Relative Path** | Ein state transition: Relative path wird berechnet als der Wert „/admin/state-machine/transitions/“, gefolgt von der state transition ID. |
| **DR-240 Is Forward** | Ein state transition gilt als ein forward, wenn es nicht der Fall ist, dass der to state key ist der Wert „Intake“. |
| **DR-241 Relative Path** | Ein subject state instance: Relative path wird berechnet als der Wert „/admin/state-machine/instances/“, gefolgt von der subject state instance ID. |
| **DR-242 Is Current** | Ein subject state instance gilt als ein current, wenn der exited at leer ist. |
| **DR-243 Has Complete Lineage** | Ein subject state instance gilt als über einen complete lineage verfügend, wenn der sequence index mindestens 1 ist. |
| **DR-244 Is Long Dwell** | Ein subject state instance gilt als ein long dwell, wenn der dwell days mindestens 90 ist. |
| **DR-245 Name** | Ein disease domain concept: Name wird berechnet als der concept label. |
| **DR-246 Relative Path** | Ein disease domain concept: Relative path wird berechnet als der Wert „/admin/disease-concepts/“, gefolgt von der disease domain concept ID. |
| **DR-247 Is Deeply Modeled** | Ein disease domain concept gilt als deeply modeled, wenn der modeling status ist der Wert „deep-dag“. |
| **DR-248 Is Schema Modeled** | Ein disease domain concept gilt als schema modeled, wenn mindestens eines des Folgenden gilt: der modeling status ist der Wert „deep-dag“ oder der modeling status ist der Wert „schema“. |
| **DR-249 Prior Anti Ds Dna IU** | Ein serology observation: Prior anti ds dna IU ist der anti ds dna IU des zugehörigen prior observation des serology observation. |
| **DR-250 Prior C3** | Ein serology observation: Prior c3 ist der complement c3 des zugehörigen prior observation des serology observation. |
| **DR-251 Prior C4** | Ein serology observation: Prior c4 ist der complement c4 des zugehörigen prior observation des serology observation. |
| **DR-252 Anti Ds Dna Trend** | Der anti ds dna trend des serology observation wird nach folgender Priorität bestimmt:<br>1. der Wert „Stable“, wenn der prior anti ds dna IU leer ist;<br>2. der Wert „Rising“, wenn der anti ds dna IU größer ist als der prior anti ds dna IU mal 1.25;<br>3. der Wert „Falling“, wenn der anti ds dna IU kleiner ist als der prior anti ds dna IU mal 0.8;<br>4. andernfalls der Wert „Stable“. |
| **DR-253 Complement Trend** | Der complement trend des serology observation wird nach folgender Priorität bestimmt:<br>1. der Wert „Stable“, wenn der prior c3 leer ist;<br>2. der Wert „Falling“, wenn der complement c3 plus der complement c4 kleiner ist als der prior c3 plus der prior c4 mal 0.85;<br>3. der Wert „Rising“, wenn der complement c3 plus der complement c4 größer ist als der prior c3 plus der prior c4 mal 1.15;<br>4. andernfalls der Wert „Stable“. |
| **DR-254 Is Pre Nephritic Signature Panel** | Ein serology observation gilt als ein pre nephritic signature panel, wenn alles Folgende gilt: der anti ds dna trend ist der Wert „Rising“ und der complement trend ist der Wert „Falling“. |
| **DR-255 Is Significant Proteinuria** | Ein serology observation gilt als ein significant proteinuria, wenn der proteinuria g per day mindestens 0.5 ist. |
| **DR-256 Is Nephrotic Range Proteinuria** | Ein serology observation gilt als ein nephrotic range proteinuria, wenn der proteinuria g per day mindestens 3.0 ist. |
| **DR-257 Sledai Renal Points** | Der sledai renal points des serology observation wird nach folgender Priorität bestimmt:<br>1. 8, wenn mindestens eines des Folgenden gilt: das is nephrotic range proteinuria-Kennzeichen gesetzt ist oder das has active urinary sediment-Kennzeichen gesetzt ist;<br>2. 4, wenn das is significant proteinuria-Kennzeichen gesetzt ist;<br>3. andernfalls 0. |
| **DR-258 Sledai Serology Points** | Der sledai serology points des serology observation wird nach folgender Priorität bestimmt:<br>1. 4, wenn alles Folgende gilt: der complement trend ist der Wert „Falling“ und der anti ds dna trend ist der Wert „Rising“;<br>2. 2, wenn mindestens eines des Folgenden gilt: der complement trend ist der Wert „Falling“ oder der anti ds dna trend ist der Wert „Rising“;<br>3. andernfalls 0. |
| **DR-259 Sledai Score** | Ein serology observation: Sledai score wird berechnet als der sledai renal points plus der sledai serology points. |
| **DR-260 Progression State Key** | Der progression state key des serology observation wird nach folgender Priorität bestimmt:<br>1. der Wert „BiopsyIndicated“, wenn mindestens eines des Folgenden gilt: das is nephrotic range proteinuria-Kennzeichen gesetzt ist oder das has active urinary sediment-Kennzeichen gesetzt ist;<br>2. der Wert „RenalFlareRisk“, wenn der proteinuria g per day mindestens 1.0 ist;<br>3. der Wert „EarlyNephritis“, wenn das is significant proteinuria-Kennzeichen gesetzt ist;<br>4. der Wert „SerologicActive“, wenn alles Folgende gilt: der anti ds dna trend ist der Wert „Rising“ und der complement trend ist der Wert „Falling“;<br>5. andernfalls der Wert „PresymptomaticAutoimmunity“. |
| **DR-261 Progression State Order** | Der progression state order des serology observation wird nach folgender Priorität bestimmt:<br>1. 5, wenn der progression state key ist der Wert „BiopsyIndicated“;<br>2. 4, wenn der progression state key ist der Wert „RenalFlareRisk“;<br>3. 3, wenn der progression state key ist der Wert „EarlyNephritis“;<br>4. 2, wenn der progression state key ist der Wert „SerologicActive“;<br>5. andernfalls 1. |
| **DR-262 Relative Path** | Ein serology observation: Relative path wird berechnet als der Wert „/admin/serology/“, gefolgt von der serology observation ID. |
| **DR-263 Name** | Ein therapy option: Name wird berechnet als der therapy label. |
| **DR-264 Relative Path** | Ein therapy option: Relative path wird berechnet als der Wert „/admin/therapy-options/“, gefolgt von der therapy option ID. |

## 5 Rückverfolgbarkeit zum Schema

_Die Ausdruck-Spalte ist die Definition der Regel in RuleSpeak-Notation —
dieselbe Logik, die das Regelbuch speichert, für einen Geschäftsleser geschrieben._

| Schema-Element | Art | Ausdruck |
|----------------|-----|----------|
| **AutoimmuneDiseases.Name** | Formel | `DiseaseLabel` |
| **AutoimmuneDiseases.RelativePath** | Formel | `"/diseases/" & AutoimmuneDiseaseId` |
| **AutoimmuneDiseases.CountOfDiseaseStages** | Aggregation | `Anzahl(DiseaseStages über AutoimmuneDisease)` |
| **AutoimmuneDiseases.CountOfInterventionTargets** | Aggregation | `Anzahl(InterventionTargets über AutoimmuneDisease)` |
| **DiseaseStages.Name** | Formel | `AutoimmuneDiseaseDiseaseLabel & " — " & StageLabel` |
| **DiseaseStages.ParentPath** | Verweis | `Verweis(AutoimmuneDiseases.RelativePath über AutoimmuneDisease)` |
| **DiseaseStages.RelativePath** | Formel | `ParentPath & "/stages/" & DiseaseStageId` |
| **DiseaseStages.AutoimmuneDiseaseDiseaseLabel** | Formel | `Verweis(AutoimmuneDiseases.DiseaseLabel über AutoimmuneDisease)` |
| **DiseaseStages.IsPresymptomatic** | Formel | `If(StageLabel = "Presymptomatic", True(), False())` |
| **Tissues.Name** | Formel | `TissueLabel` |
| **Tissues.RelativePath** | Formel | `"/tissues/" & TissueId` |
| **Tissues.CountOfOmicsAssays** | Aggregation | `Anzahl(OmicsAssays über Tissue)` |
| **OmicsModalities.Name** | Formel | `ModalityLabel` |
| **OmicsModalities.RelativePath** | Formel | `"/omics-modalities/" & OmicsModalityId` |
| **OmicsModalities.CountOfOmicsAssays** | Aggregation | `Anzahl(OmicsAssays über OmicsModality)` |
| **FederatedDatasets.Name** | Formel | `NodeLabel` |
| **FederatedDatasets.RelativePath** | Formel | `"/datasets/" & FederatedDatasetId` |
| **FederatedDatasets.CountOfIndividuals** | Aggregation | `Anzahl(Individuals über FederatedDataset)` |
| **VariantTypes.Name** | Formel | `TypeLabel` |
| **VariantTypes.RelativePath** | Formel | `"/variant-types/" & VariantTypeId` |
| **VariantTypes.CountOfGenomicVariants** | Aggregation | `Anzahl(GenomicVariants über VariantType)` |
| **Individuals.Name** | Formel | `GivenName & " " & FamilyName` |
| **Individuals.Slug** | Formel | `Ersetzen(Lower(FamilyName & "-" & GivenName), " ", "-")` |
| **Individuals.RelativePath** | Formel | `"/intake/new-patient/" & Slug` |
| **Individuals.FederatedDatasetNodeLabel** | Formel | `If(FederatedDataset = "", "", Verweis(FederatedDatasets.NodeLabel über FederatedDataset))` |
| **Individuals.CountOfGenomicVariants** | Aggregation | `Anzahl(GenomicVariants über Individual)` |
| **Individuals.CountOfCausalMechanisms** | Aggregation | `Anzahl(CausalMechanisms über Individual)` |
| **Individuals.CountOfEpistaticInteractions** | Aggregation | `Anzahl(EpistaticInteractions über Individual)` |
| **Individuals.RareVariantBurdenScore** | Formel | `If(AgeYears > 0, CountOfGenomicVariants / AgeYears, 0)` |
| **Individuals.CausalArchitectureScore** | Formel | `CountOfCausalMechanisms * 10 + CountOfEpistaticInteractions * 5 + RareVariantBurdenScore` |
| **Individuals.IsDevelopmentWindow** | Formel | `If(AgeYears <= 25, True(), False())` |
| **Individuals.IsAgingWindow** | Formel | `If(AgeYears >= 60, True(), False())` |
| **Individuals.CountConfirmedCausalNodes** | Aggregation | `Anzahl(CausalMechanisms über Individual)` |
| **Individuals.SumConfirmedCausalConfidence** | Aggregation | `Summe(CausalMechanisms.CausalConfidence über Individual)` |
| **Individuals.CountCrossAncestryConfirmedNodes** | Aggregation | `Anzahl(CausalMechanisms über Individual)` |
| **Individuals.MaxSeverityScore** | Aggregation | `Max(ClinicalPhenotypes.SeverityScore über Individual)` |
| **Individuals.CountHighSeverityPhenotypes** | Aggregation | `Anzahl(ClinicalPhenotypes über Individual)` |
| **Individuals.HasHighSeverityPhenotype** | Formel | `If(CountHighSeverityPhenotypes >= 1, True(), False())` |
| **Individuals.CountPredictedTreatmentResponses** | Aggregation | `Anzahl(Treatments über Individual)` |
| **Individuals.HasPredictedTreatmentResponse** | Formel | `If(CountPredictedTreatmentResponses >= 1, True(), False())` |
| **Individuals.CountSerologyPanels** | Aggregation | `Anzahl(SerologyObservations über Individual)` |
| **Individuals.CountPreNephriticSignaturePanels** | Aggregation | `Anzahl(SerologyObservations über Individual)` |
| **Individuals.IsInPreNephriticSignatureCluster** | Formel | `If(CountPreNephriticSignaturePanels >= 1, True(), False())` |
| **Individuals.SignatureStrength** | Formel | `If(CountPreNephriticSignaturePanels >= 2, 2, If(CountPreNephriticSignaturePanels >= 1, 1, 0))` |
| **Individuals.MaxProgressionStateOrder** | Aggregation | `Max(SerologyObservations.ProgressionStateOrder über Individual)` |
| **Individuals.LatestSledaiScore** | Aggregation | `Max(SerologyObservations.SledaiScore über Individual)` |
| **Individuals.NephritisProgressionStateKey** | Formel | `If(MaxProgressionStateOrder >= 5, "BiopsyIndicated", If(MaxProgressionStateOrder >= 4, "RenalFlareRisk", If(MaxProgressionStateOrder >= 3, "EarlyNephritis", If(MaxProgressionStateOrder >= 2, "SerologicActive", "PresymptomaticAutoimmunity"))))` |
| **Individuals.ActivityTier** | Formel | `If(LatestSledaiScore >= 12, "High / flare", If(LatestSledaiScore >= 6, "Moderate", If(LatestSledaiScore >= 1, "Mild", "Quiescent")))` |
| **Individuals.IsHighDiseaseActivity** | Formel | `If(LatestSledaiScore >= 12, True(), False())` |
| **Individuals.IsDiseaseProgressing** | Formel | `If(Or(NephritisProgressionStateKey = "EarlyNephritis", NephritisProgressionStateKey = "RenalFlareRisk", NephritisProgressionStateKey = "BiopsyIndicated"), True(), False())` |
| **Individuals.TargetPathwayCode** | Aggregation | `Max(CausalMechanisms.TargetPathwayCode über Individual)` |
| **Individuals.TargetPathway** | Formel | `If(TargetPathwayCode = 1, "type-I-IFN", If(TargetPathwayCode = 2, "B-cell/autoantibody", If(TargetPathwayCode = 3, "T-cell-costim", If(TargetPathwayCode = 4, "IL-17/23", ""))))` |
| **Individuals.ReachableStatesAhead** | Verweis | `Verweis(MachineStates.ReachableStateCount über CurrentProgressionStateId)` |
| **GenomicVariants.Name** | Formel | `VariantLabel` |
| **GenomicVariants.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **GenomicVariants.RelativePath** | Formel | `ParentPath & "/variants/" & GenomicVariantId` |
| **GenomicVariants.VariantTypeLabel** | Formel | `Verweis(VariantTypes.TypeLabel über VariantType)` |
| **GenomicVariants.VariantClassIsRare** | Verweis | `Verweis(VariantTypes.IsRareVariantClass über VariantType)` |
| **GenomicVariants.IndividualAncestryLabel** | Formel | `Verweis(Individuals.AncestryLabel über Individual)` |
| **GenomicVariants.IsRareVariant** | Formel | `If(AlleleFrequency < 0.01, True(), False())` |
| **GenomicVariants.IsCausalCandidate** | Formel | `If(And(Or(IsRareVariant, VariantClassIsRare), HasAlleleSpecificExpression), True(), False())` |
| **OmicsAssays.Name** | Formel | `AssayLabel` |
| **OmicsAssays.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **OmicsAssays.RelativePath** | Formel | `ParentPath & "/assays/" & OmicsAssayId` |
| **OmicsAssays.ModalityLabel** | Formel | `Verweis(OmicsModalities.ModalityLabel über OmicsModality)` |
| **OmicsAssays.TissueLabel** | Formel | `If(Tissue = "", "Missing Tissue", Verweis(Tissues.TissueLabel über Tissue))` |
| **OmicsAssays.HasBatchEffectRisk** | Formel | `If(MeasurementErrorScore > 0.3, True(), False())` |
| **OmicsAssays.IsHighQualityAssay** | Formel | `If(And(Not(HasBatchEffectRisk), MeasurementErrorScore < 0.15), True(), False())` |
| **EvidenceItems.Name** | Formel | `EvidenceLabel` |
| **EvidenceItems.ParentPath** | Verweis | `Verweis(CausalMechanisms.RelativePath über CausalMechanism)` |
| **EvidenceItems.RelativePath** | Formel | `ParentPath & "/evidence/" & EvidenceItemId` |
| **EvidenceItems.AssayIsHighQuality** | Verweis | `Verweis(OmicsAssays.IsHighQualityAssay über OmicsAssay)` |
| **EvidenceItems.ZStat** | Formel | `If(StandardError > 0, EffectSize / StandardError, 0)` |
| **EvidenceItems.IsConfoundControlled** | Formel | `If(And(IsAdjustedForAncestryPCs, IsAdjustedForBatch), True(), False())` |
| **EvidenceItems.IsQualifiedEvidence** | Formel | `If(And(AssayIsHighQuality, Not(IsNegativeControlArm), ZStat >= 2, IsConfoundControlled), True(), False())` |
| **CohortReplications.Name** | Formel | `ReplicationLabel` |
| **CohortReplications.ParentPath** | Verweis | `Verweis(CausalMechanisms.RelativePath über CausalMechanism)` |
| **CohortReplications.RelativePath** | Formel | `ParentPath & "/replications/" & CohortReplicationId` |
| **CohortReplications.ReplicatedAtNominalSig** | Formel | `If(And(ReplicationPValue <= 0.05, ReplicationEffectSign = 1), True(), False())` |
| **CohortReplications.MechanismPrimaryAncestry** | Verweis | `Verweis(CausalMechanisms.IndividualAncestryLabel über CausalMechanism)` |
| **CohortReplications.IsDifferentAncestryReplication** | Formel | `If(ReplicationAncestryLabel = MechanismPrimaryAncestry, False(), True())` |
| **CohortReplications.IsCrossAncestryConcordant** | Formel | `If(And(ReplicatedAtNominalSig, IsDifferentAncestryReplication), True(), False())` |
| **NegativeControlTests.Name** | Formel | `ControlLabel` |
| **NegativeControlTests.ParentPath** | Verweis | `Verweis(CausalMechanisms.RelativePath über CausalMechanism)` |
| **NegativeControlTests.RelativePath** | Formel | `ParentPath & "/neg-controls/" & NegativeControlTestId` |
| **NegativeControlTests.IsSurvived** | Formel | `If(PermutationEffectSize <= NullThreshold, True(), False())` |
| **EnvironmentalExposures.Name** | Formel | `ExposureLabel` |
| **EnvironmentalExposures.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **EnvironmentalExposures.RelativePath** | Formel | `ParentPath & "/exposures/" & EnvironmentalExposureId` |
| **EnvironmentalExposures.IndividualAncestryLabel** | Formel | `Verweis(Individuals.AncestryLabel über Individual)` |
| **EnvironmentalExposures.IsHighExposure** | Formel | `If(ExposureLevel > 5, True(), False())` |
| **Treatments.Name** | Formel | `TreatmentLabel` |
| **Treatments.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **Treatments.RelativePath** | Formel | `ParentPath & "/treatments/" & TreatmentId` |
| **Treatments.AutoimmuneDiseaseLabel** | Formel | `Verweis(AutoimmuneDiseases.DiseaseLabel über AutoimmuneDisease)` |
| **Treatments.IsEffectiveTreatment** | Formel | `If(And(Or(TreatmentResponse = "Complete", TreatmentResponse = "Partial"), Not(HasAdverseEffect)), True(), False())` |
| **Treatments.IsMechanismMatched** | Verweis | `If(TargetsMechanism = "", False(), Verweis(CausalMechanisms.IsCausalArchitectureNode über TargetsMechanism))` |
| **Treatments.IsTreatmentResponsePredicted** | Formel | `If(And(IsEffectiveTreatment, IsMechanismMatched), True(), False())` |
| **Treatments.TreatmentResponseDecidingFactor** | Formel | `If(IsTreatmentResponsePredicted, "EffectiveOnConfirmedMechanism", If(Not(IsMechanismMatched), "NoConfirmedMechanism", If(HasAdverseEffect, "AdverseEffect", If(Or(TreatmentResponse = "None", TreatmentResponse = "Adverse"), "NoResponse", "Undetermined"))))` |
| **ClinicalPhenotypes.Name** | Formel | `PhenotypeLabel` |
| **ClinicalPhenotypes.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **ClinicalPhenotypes.RelativePath** | Formel | `ParentPath & "/phenotypes/" & ClinicalPhenotypeId` |
| **ClinicalPhenotypes.DiseaseStageLabel** | Formel | `If(DiseaseStage = "", "", Verweis(DiseaseStages.StageLabel über DiseaseStage))` |
| **ClinicalPhenotypes.IsHighSeverity** | Formel | `If(SeverityScore > 7, True(), False())` |
| **ClinicalPhenotypes.IsPresymptomaticPhenotype** | Formel | `If(DiseaseStageLabel = "Presymptomatic", True(), False())` |
| **CausalMechanisms.TargetPathwayCode** | Formel | `If(TargetPathway = "type-I-IFN", 1, If(TargetPathway = "B-cell/autoantibody", 2, If(TargetPathway = "T-cell-costim", 3, If(TargetPathway = "IL-17/23", 4, 0))))` |
| **CausalMechanisms.Name** | Formel | `MechanismLabel` |
| **CausalMechanisms.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **CausalMechanisms.RelativePath** | Formel | `ParentPath & "/mechanisms/" & CausalMechanismId` |
| **CausalMechanisms.IndividualAncestryLabel** | Formel | `Verweis(Individuals.AncestryLabel über Individual)` |
| **CausalMechanisms.CountQualifiedEvidence** | Aggregation | `Anzahl(EvidenceItems über CausalMechanism)` |
| **CausalMechanisms.CountModalitiesSupporting** | Aggregation | `Anzahl(EvidenceItems über CausalMechanism)` |
| **CausalMechanisms.CountInterventionTargets** | Aggregation | `Anzahl(InterventionTargets über CausalMechanism)` |
| **CausalMechanisms.IsExperimentallyFalsifiable** | Formel | `If(And(CountInterventionTargets >= 1, CountQualifiedEvidence >= 1), True(), False())` |
| **CausalMechanisms.CountReplications** | Aggregation | `Anzahl(CohortReplications über CausalMechanism)` |
| **CausalMechanisms.CountConcordantReplications** | Aggregation | `Anzahl(CohortReplications über CausalMechanism)` |
| **CausalMechanisms.CountCrossAncestryConcordant** | Aggregation | `Anzahl(CohortReplications über CausalMechanism)` |
| **CausalMechanisms.ReplicationFraction** | Formel | `If(CountReplications > 0, CountConcordantReplications / CountReplications, 0)` |
| **CausalMechanisms.ReplicatesAcrossCohorts** | Formel | `If(And(CountReplications >= 2, CountConcordantReplications >= 2), True(), False())` |
| **CausalMechanisms.CountNegControlTests** | Aggregation | `Anzahl(NegativeControlTests über CausalMechanism)` |
| **CausalMechanisms.CountNegControlSurvived** | Aggregation | `Anzahl(NegativeControlTests über CausalMechanism)` |
| **CausalMechanisms.SurvivesNegativeControls** | Formel | `If(And(CountNegControlTests >= 1, CountNegControlSurvived = CountNegControlTests), True(), False())` |
| **CausalMechanisms.IsSpuriousDerived** | Formel | `If(Or(Not(ReplicatesAcrossCohorts), Not(SurvivesNegativeControls), CountModalitiesSupporting < 2, HasPleiotropy), True(), False())` |
| **CausalMechanisms.CausalConfidence** | Formel | `If(0.30 * If(CountQualifiedEvidence >= 4, 1, CountQualifiedEvidence / 4) + 0.20 * If(CountModalitiesSupporting >= 3, 1, CountModalitiesSupporting / 3) + 0.30 * ReplicationFraction + 0.20 * If(SurvivesNegativeControls, 1, 0) > 1, 1, 0.30 * If(CountQualifiedEvidence >= 4, 1, CountQualifiedEvidence / 4) + 0.20 * If(CountModalitiesSupporting >= 3, 1, CountModalitiesSupporting / 3) + 0.30 * ReplicationFraction + 0.20 * If(SurvivesNegativeControls, 1, 0))` |
| **CausalMechanisms.VariantIsCausalCandidate** | Verweis | `If(GenomicVariant = "", False(), Verweis(GenomicVariants.IsCausalCandidate über GenomicVariant))` |
| **CausalMechanisms.IsCausalArchitectureNode** | Formel | `If(And(CausalConfidence >= 0.7, IsExperimentallyFalsifiable, Not(IsSpuriousDerived), Or(VariantIsCausalCandidate, EnvironmentalExposure <> "")), True(), False())` |
| **CausalMechanisms.IsAncestryTransportable** | Formel | `If(And(IsCausalArchitectureNode, CountCrossAncestryConcordant >= 1), True(), False())` |
| **EpistaticInteractions.Name** | Formel | `InteractionLabel` |
| **EpistaticInteractions.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **EpistaticInteractions.RelativePath** | Formel | `ParentPath & "/epistasis/" & EpistaticInteractionId` |
| **EpistaticInteractions.IsHighOrderEpistasis** | Formel | `If(EpistasisScore > 0.5, True(), False())` |
| **CounterfactualTrajectories.Name** | Formel | `TrajectoryLabel` |
| **CounterfactualTrajectories.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **CounterfactualTrajectories.RelativePath** | Formel | `ParentPath & "/trajectories/" & CounterfactualTrajectoryId` |
| **CounterfactualTrajectories.AutoimmuneDiseaseLabel** | Formel | `Verweis(AutoimmuneDiseases.DiseaseLabel über AutoimmuneDisease)` |
| **CounterfactualTrajectories.IsWorseningTrajectory** | Formel | `If(ProjectedSeverity > 7, True(), False())` |
| **IndividualPredictions.Name** | Formel | `PredictionLabel` |
| **IndividualPredictions.ParentPath** | Verweis | `Verweis(Individuals.RelativePath über Individual)` |
| **IndividualPredictions.RelativePath** | Formel | `ParentPath & "/predictions/" & IndividualPredictionId` |
| **IndividualPredictions.IndividualAncestryLabel** | Formel | `Verweis(Individuals.AncestryLabel über Individual)` |
| **IndividualPredictions.IsAncestryHoldout** | Formel | `Verweis(Individuals.IsAncestryAbsentFromTraining über Individual)` |
| **IndividualPredictions.IndividualCausalMass** | Verweis | `If(Individual = "", 0, Verweis(Individuals.SumConfirmedCausalConfidence über Individual))` |
| **IndividualPredictions.IndividualConfirmedNodeCount** | Verweis | `If(Individual = "", 0, Verweis(Individuals.CountConfirmedCausalNodes über Individual))` |
| **IndividualPredictions.IndividualCrossAncestryNodeCount** | Verweis | `If(Individual = "", 0, Verweis(Individuals.CountCrossAncestryConfirmedNodes über Individual))` |
| **IndividualPredictions.IndividualHasCrypticRelatedness** | Verweis | `If(Individual = "", False(), Verweis(Individuals.HasCrypticRelatednessFlag über Individual))` |
| **IndividualPredictions.IndividualMaxSeverityScore** | Verweis | `If(Individual = "", 0, Verweis(Individuals.MaxSeverityScore über Individual))` |
| **IndividualPredictions.IndividualHasHighSeverityPhenotype** | Verweis | `If(Individual = "", False(), Verweis(Individuals.HasHighSeverityPhenotype über Individual))` |
| **IndividualPredictions.IndividualHasPredictedTreatmentResponse** | Verweis | `If(Individual = "", False(), Verweis(Individuals.HasPredictedTreatmentResponse über Individual))` |
| **IndividualPredictions.PredictedValue** | Formel | `If(2 * IndividualCausalMass + 1.5 * IndividualConfirmedNodeCount > 10, 10, 2 * IndividualCausalMass + 1.5 * IndividualConfirmedNodeCount)` |
| **IndividualPredictions.CountBins** | Aggregation | `Anzahl(CalibrationBins über IndividualPrediction)` |
| **IndividualPredictions.CountWellCalibratedBins** | Aggregation | `Anzahl(CalibrationBins über IndividualPrediction)` |
| **IndividualPredictions.SumBinAbsError** | Aggregation | `Summe(CalibrationBins.BinAbsError über IndividualPrediction)` |
| **IndividualPredictions.MeanBinAbsError** | Formel | `If(CountBins > 0, SumBinAbsError / CountBins, 1)` |
| **IndividualPredictions.WellCalibratedFraction** | Formel | `If(CountBins > 0, CountWellCalibratedBins / CountBins, 0)` |
| **IndividualPredictions.CalibratedUncertainty** | Formel | `If(1 - MeanBinAbsError < 0, 0, 1 - MeanBinAbsError) * WellCalibratedFraction` |
| **IndividualPredictions.RestsOnConfirmedMechanism** | Formel | `If(IndividualConfirmedNodeCount >= 1, True(), False())` |
| **IndividualPredictions.HasSpuriousCorrelationFlag** | Formel | `If(Or(Not(RestsOnConfirmedMechanism), IndividualHasCrypticRelatedness), True(), False())` |
| **IndividualPredictions.IsFalsifiabilityBacked** | Formel | `If(IndividualConfirmedNodeCount >= 1, True(), False())` |
| **IndividualPredictions.IsTransportableToAbsentAncestry** | Formel | `If(And(IsAncestryHoldout, IndividualCrossAncestryNodeCount >= 1, Not(HasSpuriousCorrelationFlag)), True(), False())` |
| **IndividualPredictions.IsAncestryTransportSafe** | Formel | `If(IsAncestryHoldout, IsTransportableToAbsentAncestry, True())` |
| **IndividualPredictions.TransportGateStatus** | Formel | `If(Not(IsAncestryHoldout), "NotApplicable", If(IsTransportableToAbsentAncestry, "PASS-tested", "FAIL"))` |
| **IndividualPredictions.IsHighConfidencePrediction** | Formel | `If(And(CalibratedUncertainty >= 0.7, Not(HasSpuriousCorrelationFlag)), True(), False())` |
| **IndividualPredictions.PatientStratificationTier** | Formel | `If(PredictedValue >= 7, "High-Risk Pathway", If(PredictedValue >= 4, "Moderate-Risk Pathway", "Low-Risk Pathway"))` |
| **IndividualPredictions.PredictedSeverityValue** | Formel | `IndividualMaxSeverityScore` |
| **IndividualPredictions.SeverityTier** | Formel | `If(PredictedSeverityValue > 7, "Severe", If(PredictedSeverityValue >= 4, "Moderate", "Mild"))` |
| **IndividualPredictions.IsSeverityActionable** | Formel | `If(And(IndividualHasHighSeverityPhenotype, RestsOnConfirmedMechanism, Not(HasSpuriousCorrelationFlag)), True(), False())` |
| **IndividualPredictions.SeverityDecidingFactor** | Formel | `If(IsSeverityActionable, "HighSeverityOnConfirmedMechanism", If(Not(IndividualHasHighSeverityPhenotype), "NotHighSeverity", If(Not(RestsOnConfirmedMechanism), "NoValidatedMechanism", If(HasSpuriousCorrelationFlag, "SpuriousFlag", "Undetermined"))))` |
| **IndividualPredictions.IsTreatmentResponseActionable** | Formel | `If(IndividualHasPredictedTreatmentResponse, True(), False())` |
| **IndividualPredictions.TreatmentResponseDecidingFactor** | Formel | `If(IsTreatmentResponseActionable, "EffectiveOnConfirmedMechanism", If(RestsOnConfirmedMechanism, "NoEffectiveTreatmentOnMechanism", "NoConfirmedMechanism"))` |
| **IndividualPredictions.IsClinicallyActionable** | Formel | `If(And(IsHighConfidencePrediction, IsFalsifiabilityBacked, IsAncestryTransportSafe, PredictedValue > 0), True(), False())` |
| **IndividualPredictions.LifecycleStateKey** | Formel | `If(And(IsHighConfidencePrediction, IsFalsifiabilityBacked, IsAncestryTransportSafe, PredictedValue > 0), "Actionable", If(Or(Not(RestsOnConfirmedMechanism), Not(IsFalsifiabilityBacked)), "NotActionable", If(IndividualHasCrypticRelatedness, "NotActionable", If(CalibratedUncertainty < 0.7, "NotActionable", If(Not(IsAncestryTransportSafe), "NotActionable", "Actionable")))))` |
| **IndividualPredictions.DecidingGate** | Formel | `If(IsClinicallyActionable, "AllGatesPass", If(Not(RestsOnConfirmedMechanism), "NoValidatedMechanism", If(IndividualHasCrypticRelatedness, "CrypticRelatedness", If(CalibratedUncertainty < 0.7, "Calibration", If(Not(IsAncestryTransportSafe), "AncestryTransport", "Undetermined")))))` |
| **IndividualPredictions.IndividualTargetPathway** | Verweis | `Verweis(Individuals.TargetPathway über Individual)` |
| **IndividualPredictions.IndividualProgressionStateKey** | Verweis | `Verweis(Individuals.NephritisProgressionStateKey über Individual)` |
| **IndividualPredictions.IndividualIsDiseaseProgressing** | Verweis | `Verweis(Individuals.IsDiseaseProgressing über Individual)` |
| **IndividualPredictions.RecommendedTreatmentLine** | Formel | `If(Not(RestsOnConfirmedMechanism), "No targeted line — mechanism unconfirmed", If(Or(IndividualProgressionStateKey = "RenalFlareRisk", IndividualProgressionStateKey = "BiopsyIndicated"), "Mycophenolate (induction)", If(IndividualTargetPathway = "type-I-IFN", "Anifrolumab", If(IndividualTargetPathway = "B-cell/autoantibody", "Belimumab", If(IndividualTargetPathway = "IL-17/23", "Secukinumab", "Standard-of-care (no mechanism-matched targeted line)")))))` |
| **IndividualPredictions.TreatmentLineDecidingFactor** | Formel | `If(Not(RestsOnConfirmedMechanism), "MechanismUnconfirmed", If(Or(IndividualProgressionStateKey = "RenalFlareRisk", IndividualProgressionStateKey = "BiopsyIndicated"), "ActiveNephritis-Induction", If(IndividualTargetPathway = "type-I-IFN", "IFNSignature-Anifrolumab", If(IndividualTargetPathway = "B-cell/autoantibody", "AutoantibodyDriven-Belimumab", If(IndividualTargetPathway = "IL-17/23", "IL17Axis-Secukinumab", "NoMechanismMatch")))))` |
| **IndividualPredictions.ProgressionVsActionabilityDisagree** | Formel | `If(And(IndividualIsDiseaseProgressing, Not(IsClinicallyActionable)), True(), False())` |
| **CalibrationBins.Name** | Formel | `BinLabel` |
| **CalibrationBins.ParentPath** | Verweis | `Verweis(IndividualPredictions.RelativePath über IndividualPrediction)` |
| **CalibrationBins.RelativePath** | Formel | `ParentPath & "/bins/" & CalibrationBinId` |
| **CalibrationBins.BinAbsError** | Formel | `If(PredictedProbabilityBand >= ObservedEventRate, PredictedProbabilityBand - ObservedEventRate, ObservedEventRate - PredictedProbabilityBand)` |
| **CalibrationBins.IsWellCalibratedBin** | Formel | `If(And(CoverageCount >= 20, BinAbsError <= 0.1), True(), False())` |
| **InterventionTargets.Name** | Formel | `TargetLabel` |
| **InterventionTargets.ParentPath** | Verweis | `Verweis(CausalMechanisms.RelativePath über CausalMechanism)` |
| **InterventionTargets.RelativePath** | Formel | `ParentPath & "/targets/" & InterventionTargetId` |
| **InterventionTargets.CausalMechanismLabel** | Formel | `Verweis(CausalMechanisms.MechanismLabel über CausalMechanism)` |
| **InterventionTargets.IsGeneBasedTherapy** | Formel | `If(TherapyClass = "Gene-based", True(), False())` |
| **InterventionTargets.IsCellBasedTherapy** | Formel | `If(TherapyClass = "Cell-based", True(), False())` |
| **Axioms.Name** | Formel | `Statement` |
| **Axioms.RelativePath** | Formel | `"/admin/axioms/" & AxiomId` |
| **TestsForSuccess.Name** | Formel | `Claim` |
| **TestsForSuccess.RelativePath** | Formel | `"/admin/tests-for-success/" & TestForSuccessId` |
| **Features.Name** | Formel | `Title` |
| **Features.RelativePath** | Formel | `"/admin/features/" & FeatureId` |
| **Features.MetaLine** | Formel | `"**Category:** " & Category & " - **Priority:** " & Priority & " - **Challenge refs:** " & RefCount` |
| **InferenceKinds.Name** | Formel | `Title` |
| **InferenceKinds.RelativePath** | Formel | `"/admin/inference-kinds/" & InferenceKindId` |
| **OpenQuestions.Name** | Formel | `Question` |
| **OpenQuestions.RelativePath** | Formel | `"/admin/open-questions/" & OpenQuestionId` |
| **NonGoals.Name** | Formel | `Statement` |
| **NonGoals.RelativePath** | Formel | `"/admin/non-goals/" & NonGoalId` |
| **GlossaryTerms.Name** | Formel | `Term` |
| **GlossaryTerms.RelativePath** | Formel | `"/admin/glossary/" & GlossaryTermId` |
| **LeopoldLoops.Name** | Formel | `Verketten("Loop ", LoopNumber, " — ", Title)` |
| **LeopoldLoops.RelativePath** | Formel | `"/admin/leopold-loops/" & LeopoldLoopId` |
| **LeopoldLoops.Completedness** | Formel | `Status` |
| **LeopoldLoops.IsInCurrentPlan** | Formel | `If(Status = "done", FALSE, TRUE)` |
| **RoutingAndNavigation.Name** | Formel | `Ersetzen(Lower(DisplayName), " ", "-")` |
| **RoutingAndNavigation.AdminCanCreate** | Formel | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("C", AdminCRUD))))` |
| **RoutingAndNavigation.AdminCanRead** | Formel | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("R", AdminCRUD))))` |
| **RoutingAndNavigation.AdminCanUpdate** | Formel | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("U", AdminCRUD))))` |
| **RoutingAndNavigation.AdminCanDelete** | Formel | `If(AdminCRUD = Blank(), Blank(), Not(Iserror(Find("D", AdminCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanCreate** | Formel | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("C", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanRead** | Formel | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("R", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanUpdate** | Formel | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("U", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.IntakeClinicianCanDelete** | Formel | `If(IntakeClinicianCRUD = Blank(), Blank(), Not(Iserror(Find("D", IntakeClinicianCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanCreate** | Formel | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("C", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanRead** | Formel | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("R", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanUpdate** | Formel | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("U", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.DiagnosingDoctorCanDelete** | Formel | `If(DiagnosingDoctorCRUD = Blank(), Blank(), Not(Iserror(Find("D", DiagnosingDoctorCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanCreate** | Formel | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("C", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanRead** | Formel | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("R", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanUpdate** | Formel | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("U", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.ExternalLlmCanDelete** | Formel | `If(ExternalLlmCRUD = Blank(), Blank(), Not(Iserror(Find("D", ExternalLlmCRUD))))` |
| **RoutingAndNavigation.Depth** | Formel | `If(ParentRouteKey = Blank(), 0, Len(RouteKey) - Len(Ersetzen(RouteKey, ".", "")))` |
| **RoutingAndNavigation.FullPath** | Formel | `Route` |
| **RoutingAndNavigation.HandlerBaseName** | Formel | `Ersetzen(Ersetzen(RouteKey, ".", " "), "-", " ")` |
| **RoutingAndNavigation.RelativePath** | Formel | `"/admin/routing/" & RoutingAndNavigationId` |
| **StateMachines.RelativePath** | Formel | `"/admin/state-machine/" & StateMachineId` |
| **StateMachines.StateCount** | Aggregation | `Anzahl(MachineStates über StateMachine)` |
| **StateMachines.TransitionRuleCount** | Aggregation | `Anzahl(StateTransitionRules über StateMachine)` |
| **MachineStates.RelativePath** | Formel | `"/admin/state-machine/states/" & MachineStateId` |
| **MachineStates.ReachableStateCount** | Aggregation | `Anzahl(vw_state_transition_rules_closure über FromId)` |
| **StateTransitionRules.RelativePath** | Formel | `"/admin/state-machine/rules/" & StateTransitionRuleId` |
| **StateTransitionRules.FromStateKey** | Verweis | `Verweis(MachineStates.StateKey über FromState)` |
| **StateTransitionRules.ToStateKey** | Verweis | `Verweis(MachineStates.StateKey über ToState)` |
| **StateTransitionRules.IsForwardEdge** | Formel | `Not(ToStateKey = FromStateKey)` |
| **StateTransitions.RelativePath** | Formel | `"/admin/state-machine/transitions/" & StateTransitionId` |
| **StateTransitions.IsForward** | Formel | `Not(ToStateKey = "Intake")` |
| **SubjectStateInstances.RelativePath** | Formel | `"/admin/state-machine/instances/" & SubjectStateInstanceId` |
| **SubjectStateInstances.IsCurrent** | Formel | `Isblank(ExitedAt)` |
| **SubjectStateInstances.HasCompleteLineage** | Formel | `SequenceIndex >= 1` |
| **SubjectStateInstances.IsLongDwell** | Formel | `If(DwellDays >= 90, True(), False())` |
| **DiseaseDomainConcepts.Name** | Formel | `ConceptLabel` |
| **DiseaseDomainConcepts.RelativePath** | Formel | `"/admin/disease-concepts/" & DiseaseDomainConceptId` |
| **DiseaseDomainConcepts.IsDeeplyModeled** | Formel | `If(ModelingStatus = "deep-dag", True(), False())` |
| **DiseaseDomainConcepts.IsSchemaModeled** | Formel | `If(Or(ModelingStatus = "deep-dag", ModelingStatus = "schema"), True(), False())` |
| **SerologyObservations.PriorAntiDsDnaIU** | Verweis | `Verweis(SerologyObservations.AntiDsDnaIU über PriorObservation)` |
| **SerologyObservations.PriorC3** | Verweis | `Verweis(SerologyObservations.ComplementC3 über PriorObservation)` |
| **SerologyObservations.PriorC4** | Verweis | `Verweis(SerologyObservations.ComplementC4 über PriorObservation)` |
| **SerologyObservations.AntiDsDnaTrend** | Formel | `If(Isblank(PriorAntiDsDnaIU), "Stable", If(AntiDsDnaIU > PriorAntiDsDnaIU * 1.25, "Rising", If(AntiDsDnaIU < PriorAntiDsDnaIU * 0.8, "Falling", "Stable")))` |
| **SerologyObservations.ComplementTrend** | Formel | `If(Isblank(PriorC3), "Stable", If(ComplementC3 + ComplementC4 < PriorC3 + PriorC4 * 0.85, "Falling", If(ComplementC3 + ComplementC4 > PriorC3 + PriorC4 * 1.15, "Rising", "Stable")))` |
| **SerologyObservations.IsPreNephriticSignaturePanel** | Formel | `If(And(AntiDsDnaTrend = "Rising", ComplementTrend = "Falling"), True(), False())` |
| **SerologyObservations.IsSignificantProteinuria** | Formel | `If(ProteinuriaGPerDay >= 0.5, True(), False())` |
| **SerologyObservations.IsNephroticRangeProteinuria** | Formel | `If(ProteinuriaGPerDay >= 3.0, True(), False())` |
| **SerologyObservations.SledaiRenalPoints** | Formel | `If(Or(IsNephroticRangeProteinuria, HasActiveUrinarySediment), 8, If(IsSignificantProteinuria, 4, 0))` |
| **SerologyObservations.SledaiSerologyPoints** | Formel | `If(And(ComplementTrend = "Falling", AntiDsDnaTrend = "Rising"), 4, If(Or(ComplementTrend = "Falling", AntiDsDnaTrend = "Rising"), 2, 0))` |
| **SerologyObservations.SledaiScore** | Formel | `SledaiRenalPoints + SledaiSerologyPoints` |
| **SerologyObservations.ProgressionStateKey** | Formel | `If(Or(IsNephroticRangeProteinuria, HasActiveUrinarySediment), "BiopsyIndicated", If(ProteinuriaGPerDay >= 1.0, "RenalFlareRisk", If(IsSignificantProteinuria, "EarlyNephritis", If(And(AntiDsDnaTrend = "Rising", ComplementTrend = "Falling"), "SerologicActive", "PresymptomaticAutoimmunity"))))` |
| **SerologyObservations.ProgressionStateOrder** | Formel | `If(ProgressionStateKey = "BiopsyIndicated", 5, If(ProgressionStateKey = "RenalFlareRisk", 4, If(ProgressionStateKey = "EarlyNephritis", 3, If(ProgressionStateKey = "SerologicActive", 2, 1))))` |
| **SerologyObservations.RelativePath** | Formel | `"/admin/serology/" & SerologyObservationId` |
| **TherapyOptions.Name** | Formel | `TherapyLabel` |
| **TherapyOptions.RelativePath** | Formel | `"/admin/therapy-options/" & TherapyOptionId` |

---

_Dieses Dokument ist in **RuleSpeak®** gerendert, der deklarativen Geschäftsregel-
Notation von **Ronald G. Ross**, und folgt den Konventionen von
**SBVR** (Semantics of Business Vocabulary and Business Rules). Mit Dank an
Ronald G. Ross für RuleSpeak und seine grundlegende Arbeit zu Geschäftsregeln —
[www.RonRoss.info](https://www.RonRoss.info)._
