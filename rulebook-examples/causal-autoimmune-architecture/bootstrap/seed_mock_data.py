#!/usr/bin/env python3
"""Loop 0 mock data: 7 named patients (A-G), each exercising exactly one keystone gate.

Replaces the `data` arrays of the spine tables with a coherent end-to-end story.
Each patient is built so the DERIVED IsClinicallyActionable comes out to a known value
for a known reason. See the coverage matrix at the bottom and mock-data/scenarios.md.

Reference reality (bootstrap/clinical-reference.md): real loci/HLA/drugs, gnomAD rarity
cutoff (<0.01), calibration coverage convention, PRS ancestry-portability loss.
"""
import json, collections, os
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
RB = os.path.join(ROOT, "effortless-rulebook", "effortless-rulebook.json")
d = json.load(open(RB), object_pairs_hook=collections.OrderedDict)

def setdata(t, rows): d[t]["data"] = rows

# --- Reference tables (keep, lightly aligned to clinical-reference.md) -------
setdata("AutoimmuneDiseases",[
 {"AutoimmuneDiseaseId":"sle","DiseaseLabel":"Systemic Lupus Erythematosus","IsComplexDisease":True},
 {"AutoimmuneDiseaseId":"ra","DiseaseLabel":"Rheumatoid Arthritis","IsComplexDisease":True},
 {"AutoimmuneDiseaseId":"psa","DiseaseLabel":"Psoriatic Arthritis","IsComplexDisease":True},
])
setdata("DiseaseStages",[
 {"DiseaseStageId":"sle-presymptomatic","StageLabel":"Presymptomatic","SortOrder":1,"AutoimmuneDisease":"sle"},
 {"DiseaseStageId":"sle-active","StageLabel":"Active","SortOrder":2,"AutoimmuneDisease":"sle"},
 {"DiseaseStageId":"sle-remission","StageLabel":"Remission","SortOrder":3,"AutoimmuneDisease":"sle"},
 {"DiseaseStageId":"ra-active","StageLabel":"Active","SortOrder":1,"AutoimmuneDisease":"ra"},
 {"DiseaseStageId":"ra-remission","StageLabel":"Remission","SortOrder":2,"AutoimmuneDisease":"ra"},
 {"DiseaseStageId":"psa-active","StageLabel":"Active","SortOrder":1,"AutoimmuneDisease":"psa"},
])
setdata("Tissues",[
 {"TissueId":"blood","TissueLabel":"Blood"},{"TissueId":"gut","TissueLabel":"Gut"},
 {"TissueId":"synovium","TissueLabel":"Synovium"},{"TissueId":"skin","TissueLabel":"Skin"},
 {"TissueId":"kidney","TissueLabel":"Kidney"},
])
setdata("OmicsModalities",[
 {"OmicsModalityId":"long-read-genomes","ModalityLabel":"Phased Telomere-to-Telomere Long-Read Genomes","IsSingleCell":False},
 {"OmicsModalityId":"rna-seq","ModalityLabel":"RNA-seq","IsSingleCell":False},
 {"OmicsModalityId":"single-cell-rna","ModalityLabel":"Single-Cell RNA-seq","IsSingleCell":True},
 {"OmicsModalityId":"atac-seq","ModalityLabel":"ATAC-seq","IsSingleCell":False},
 {"OmicsModalityId":"proteomics","ModalityLabel":"Proteomics","IsSingleCell":False},
 {"OmicsModalityId":"microbiome","ModalityLabel":"Microbiome","IsSingleCell":False},
 {"OmicsModalityId":"methylome","ModalityLabel":"Methylome (sparse context)","IsSingleCell":False},
 {"OmicsModalityId":"hi-c","ModalityLabel":"Chromatin Conformation Hi-C (sparse context)","IsSingleCell":False},
])
setdata("FederatedDatasets",[
 {"FederatedDatasetId":"fed-europe","NodeLabel":"European Reference Node","Region":"Europe","IsPrivacyPreserving":True},
 {"FederatedDatasetId":"fed-east-asia","NodeLabel":"East Asia Federated Node","Region":"East Asia","IsPrivacyPreserving":True},
 {"FederatedDatasetId":"fed-west-africa","NodeLabel":"West Africa Federated Node","Region":"West Africa","IsPrivacyPreserving":True},
 {"FederatedDatasetId":"fed-amazon","NodeLabel":"Amazon Indigenous Cohort","Region":"Amazon Basin","IsPrivacyPreserving":True},
])
setdata("VariantTypes",[
 {"VariantTypeId":"regulatory","TypeLabel":"Regulatory Variant","IsRareVariantClass":True},
 {"VariantTypeId":"coding","TypeLabel":"Coding Variant","IsRareVariantClass":False},
 {"VariantTypeId":"hla-haplotype","TypeLabel":"HLA Haplotype","IsRareVariantClass":False},
 {"VariantTypeId":"structural","TypeLabel":"Structural Variant","IsRareVariantClass":True},
 {"VariantTypeId":"de-novo","TypeLabel":"De Novo Mutation","IsRareVariantClass":True},
])

# --- Individuals: A-G --------------------------------------------------------
# (givenname/familyname per spec; ancestry + flags chosen to isolate each gate)
setdata("Individuals",[
 {"IndividualId":"ind-a-reyes","GivenName":"Ana","FamilyName":"Reyes","AncestryLabel":"European","AgeYears":34,"IsAncestryAbsentFromTraining":False,"FederatedDataset":"fed-europe","EnrollmentDate":"2023-04-10","HasCrypticRelatednessFlag":False},
 {"IndividualId":"ind-b-okafor","GivenName":"Bili","FamilyName":"Okafor","AncestryLabel":"European","AgeYears":41,"IsAncestryAbsentFromTraining":False,"FederatedDataset":"fed-europe","EnrollmentDate":"2023-05-12","HasCrypticRelatednessFlag":False},
 {"IndividualId":"ind-c-chen","GivenName":"Chen","FamilyName":"Wei","AncestryLabel":"East Asian","AgeYears":47,"IsAncestryAbsentFromTraining":False,"FederatedDataset":"fed-east-asia","EnrollmentDate":"2022-09-01","HasCrypticRelatednessFlag":False},
 {"IndividualId":"ind-d-santos","GivenName":"Diego","FamilyName":"Santos","AncestryLabel":"European","AgeYears":29,"IsAncestryAbsentFromTraining":False,"FederatedDataset":"fed-europe","EnrollmentDate":"2024-01-20","HasCrypticRelatednessFlag":True},
 {"IndividualId":"ind-e-mensah","GivenName":"Esi","FamilyName":"Mensah","AncestryLabel":"African","AgeYears":38,"IsAncestryAbsentFromTraining":False,"FederatedDataset":"fed-west-africa","EnrollmentDate":"2023-07-15","HasCrypticRelatednessFlag":False},
 {"IndividualId":"ind-f-haidar","GivenName":"Faisal","FamilyName":"Haidar","AncestryLabel":"Indigenous American","AgeYears":31,"IsAncestryAbsentFromTraining":True,"FederatedDataset":"fed-amazon","EnrollmentDate":"2025-02-01","HasCrypticRelatednessFlag":False},
 {"IndividualId":"ind-g-lin","GivenName":"Grace","FamilyName":"Lin","AncestryLabel":"Indigenous American","AgeYears":36,"IsAncestryAbsentFromTraining":True,"FederatedDataset":"fed-amazon","EnrollmentDate":"2025-02-03","HasCrypticRelatednessFlag":False},
])

# --- GenomicVariants: one causal-candidate variant per patient (rare + ASE) --
# AlleleFrequency < 0.01 => IsRareVariant; HasAlleleSpecificExpression True => IsCausalCandidate
setdata("GenomicVariants",[
 {"GenomicVariantId":"var-a-irf5","VariantLabel":"IRF5 regulatory (rs2004640)","Individual":"ind-a-reyes","VariantType":"regulatory","AlleleFrequency":0.006,"HasAlleleSpecificExpression":True},
 {"GenomicVariantId":"var-b-irf5","VariantLabel":"IRF5 regulatory (rs2004640)","Individual":"ind-b-okafor","VariantType":"regulatory","AlleleFrequency":0.006,"HasAlleleSpecificExpression":True},
 {"GenomicVariantId":"var-c-stat4","VariantLabel":"STAT4 regulatory (rs7574865)","Individual":"ind-c-chen","VariantType":"regulatory","AlleleFrequency":0.008,"HasAlleleSpecificExpression":True},
 {"GenomicVariantId":"var-d-irf5","VariantLabel":"IRF5 regulatory (rs2004640)","Individual":"ind-d-santos","VariantType":"regulatory","AlleleFrequency":0.006,"HasAlleleSpecificExpression":True},
 {"GenomicVariantId":"var-e-ctla4","VariantLabel":"CTLA4 enhancer (rs3087243)","Individual":"ind-e-mensah","VariantType":"regulatory","AlleleFrequency":0.007,"HasAlleleSpecificExpression":True},
 {"GenomicVariantId":"var-f-il23r","VariantLabel":"IL23R regulatory (rs11209026)","Individual":"ind-f-haidar","VariantType":"regulatory","AlleleFrequency":0.009,"HasAlleleSpecificExpression":True},
 {"GenomicVariantId":"var-g-il23r","VariantLabel":"IL23R regulatory (rs11209026)","Individual":"ind-g-lin","VariantType":"regulatory","AlleleFrequency":0.009,"HasAlleleSpecificExpression":True},
])

# --- OmicsAssays: high-quality (err 0.05) per patient + one extra modality ---
def assay(aid,ind,mod,tis,err):
    return {"OmicsAssayId":aid,"AssayLabel":aid,"Individual":ind,"OmicsModality":mod,"Tissue":tis,"FederatedDataset":"","BatchId":"b1","MeasurementErrorScore":err,"HasCellStateSpecificEffect":True}
assays=[]
for p,ind in [("a","ind-a-reyes"),("b","ind-b-okafor"),("c","ind-c-chen"),("d","ind-d-santos"),("e","ind-e-mensah"),("f","ind-f-haidar"),("g","ind-g-lin")]:
    assays.append(assay(f"assay-{p}-rna","" if False else ind,"rna-seq","blood",0.05))
    assays.append(assay(f"assay-{p}-atac",ind,"atac-seq","blood",0.05))
    assays.append(assay(f"assay-{p}-prot",ind,"proteomics","blood",0.05))
setdata("OmicsAssays",assays)

# --- CausalMechanisms: one per patient, variant-grounded -----------------
def mech(cid,ind,var,label,pleio=False,disease="sle"):
    return {"CausalMechanismId":cid,"MechanismLabel":label,"Individual":ind,"GenomicVariant":var,"EnvironmentalExposure":"","MechanismType":"regulatory","HasPleiotropy":pleio}
setdata("CausalMechanisms",[
 mech("cm-a","ind-a-reyes","var-a-irf5","IRF5 regulatory -> type-I IFN -> SLE risk"),
 mech("cm-b","ind-b-okafor","var-b-irf5","IRF5 regulatory -> type-I IFN -> SLE risk"),
 mech("cm-c","ind-c-chen","var-c-stat4","STAT4 regulatory -> Th1/IFN -> SLE risk"),
 mech("cm-d","ind-d-santos","var-d-irf5","IRF5 regulatory -> type-I IFN -> SLE risk"),
 mech("cm-e","ind-e-mensah","var-e-ctla4","CTLA4 enhancer -> T-cell costimulation -> SLE risk"),
 mech("cm-f","ind-f-haidar","var-f-il23r","IL23R regulatory -> IL-17 axis -> PsA risk",disease="psa"),
 mech("cm-g","ind-g-lin","var-g-il23r","IL23R regulatory -> IL-17 axis -> PsA risk",disease="psa"),
])

# --- EvidenceItems: 3 qualified items per mechanism, >=2 cross-modality ----
# Qualified requires: AssayIsHighQuality (err 0.05 -> IsHighQualityAssay TRUE), not control,
# ZStat = EffectSize/SE >= 2, IsConfoundControlled (both adjust flags TRUE).
def ev(eid,cm,assay,eff,se,cross,neg=False,pcs=True,batch=True):
    return {"EvidenceItemId":eid,"EvidenceLabel":eid,"CausalMechanism":cm,"OmicsAssay":assay,
            "EffectSize":eff,"StandardError":se,"IsCrossModality":cross,"IsNegativeControlArm":neg,
            "IsAdjustedForAncestryPCs":pcs,"IsAdjustedForBatch":batch}
evid=[]
for p in ["a","b","c","d","e","f","g"]:
    cm=f"cm-{p}"
    # 3 strong qualified items, 2 cross-modality -> CountQualifiedEvidence=3, CountModalitiesSupporting=2
    evid.append(ev(f"ev-{p}-1",cm,f"assay-{p}-rna",0.80,0.20,False))   # ZStat 4
    evid.append(ev(f"ev-{p}-2",cm,f"assay-{p}-atac",0.70,0.20,True))    # ZStat 3.5 cross
    evid.append(ev(f"ev-{p}-3",cm,f"assay-{p}-prot",0.60,0.20,True))    # ZStat 3.0 cross
setdata("EvidenceItems",evid)

# --- CohortReplications --------------------------------------------------
# A: 3 reps, 2 concordant (>=2 each) -> ReplicatesAcrossCohorts TRUE; at least one cross-ancestry.
# B: same as A (mechanism confirmed); calibration is what fails for B.
# C: SPURIOUS via sign-flip: signs +1/-1/+1 -> only some concordant; plus neg-control fails (below).
# D: confirmed like A.
# E: confirmed like A.
# F: confirmed BUT all reps SAME ancestry as discovery (Indigenous American) -> no cross-ancestry.
# G: confirmed AND >=1 rep in a DIFFERENT ancestry -> cross-ancestry concordant.
reps=[]
def rep(rid,cm,fed,sign,pval,anc):
    return {"CohortReplicationId":rid,"ReplicationLabel":rid,"CausalMechanism":cm,"FederatedDataset":fed,
            "ReplicationEffectSign":sign,"ReplicationPValue":pval,"ReplicationAncestryLabel":anc}
# Patient A (discovery ancestry European) - reps in EastAsia + WestAfrica (cross) concordant
reps+=[rep("rep-a-1","cm-a","fed-east-asia",1,0.01,"East Asian"),
       rep("rep-a-2","cm-a","fed-west-africa",1,0.02,"African"),
       rep("rep-a-3","cm-a","fed-europe",1,0.20,"European")]  # not sig -> not concordant
# Patient B - identical replication profile (mechanism confirmed); calibration fails later
reps+=[rep("rep-b-1","cm-b","fed-east-asia",1,0.01,"East Asian"),
       rep("rep-b-2","cm-b","fed-west-africa",1,0.02,"African"),
       rep("rep-b-3","cm-b","fed-europe",1,0.20,"European")]
# Patient C - SPURIOUS: sign flips, only 1 concordant -> ReplicatesAcrossCohorts FALSE (needs >=2 concordant)
reps+=[rep("rep-c-1","cm-c","fed-east-asia",1,0.01,"East Asian"),
       rep("rep-c-2","cm-c","fed-west-africa",-1,0.02,"African"),   # opposite sign -> not concordant
       rep("rep-c-3","cm-c","fed-europe",1,0.30,"European")]        # not sig
# Patient D - confirmed mechanism (cryptic-relatedness fails at prediction level)
reps+=[rep("rep-d-1","cm-d","fed-east-asia",1,0.01,"East Asian"),
       rep("rep-d-2","cm-d","fed-west-africa",1,0.02,"African"),
       rep("rep-d-3","cm-d","fed-europe",1,0.20,"European")]
# Patient E - confirmed mechanism (falsifiability fails: no InterventionTarget)
reps+=[rep("rep-e-1","cm-e","fed-east-asia",1,0.01,"East Asian"),
       rep("rep-e-2","cm-e","fed-europe",1,0.02,"European"),
       rep("rep-e-3","cm-e","fed-west-africa",1,0.20,"African")]
# Patient F - confirmed BUT replications ALL in same (Indigenous American) ancestry -> no cross-ancestry
reps+=[rep("rep-f-1","cm-f","fed-amazon",1,0.01,"Indigenous American"),
       rep("rep-f-2","cm-f","fed-amazon",1,0.02,"Indigenous American"),
       rep("rep-f-3","cm-f","fed-amazon",1,0.03,"Indigenous American")]
# Patient G - confirmed AND >=1 cross-ancestry concordant (European + East Asian)
reps+=[rep("rep-g-1","cm-g","fed-europe",1,0.01,"European"),
       rep("rep-g-2","cm-g","fed-east-asia",1,0.02,"East Asian"),
       rep("rep-g-3","cm-g","fed-amazon",1,0.03,"Indigenous American")]
setdata("CohortReplications",reps)

# --- NegativeControlTests ------------------------------------------------
# survive: PermutationEffectSize <= NullThreshold. C must FAIL one (drives IsSpuriousDerived).
ncts=[]
def nct(nid,cm,perm,thr,kind="ancestry-permutation"):
    return {"NegativeControlTestId":nid,"ControlLabel":nid,"CausalMechanism":cm,"TestKind":kind,
            "PermutationEffectSize":perm,"NullThreshold":thr}
for p in ["a","b","d","e","f","g"]:
    ncts.append(nct(f"nct-{p}-1",f"cm-{p}",0.01,0.10))
    ncts.append(nct(f"nct-{p}-2",f"cm-{p}",0.02,0.10))
# C: one control does NOT collapse (0.5 > 0.1) -> SurvivesNegativeControls FALSE
ncts.append(nct("nct-c-1","cm-c",0.01,0.10))
ncts.append(nct("nct-c-2","cm-c",0.50,0.10))
setdata("NegativeControlTests",ncts)

# --- InterventionTargets: one per mechanism EXCEPT Patient E (falsifiability gate) --
setdata("InterventionTargets",[
 {"InterventionTargetId":"it-a","TargetLabel":"IRF5 Regulatory Modulation","CausalMechanism":"cm-a","AutoimmuneDisease":"sle","TherapyClass":"Gene-based","IsValidated":True},
 {"InterventionTargetId":"it-b","TargetLabel":"IRF5 Regulatory Modulation","CausalMechanism":"cm-b","AutoimmuneDisease":"sle","TherapyClass":"Gene-based","IsValidated":True},
 {"InterventionTargetId":"it-c","TargetLabel":"STAT4 Pathway Modulation","CausalMechanism":"cm-c","AutoimmuneDisease":"sle","TherapyClass":"Gene-based","IsValidated":False},
 {"InterventionTargetId":"it-d","TargetLabel":"IRF5 Regulatory Modulation","CausalMechanism":"cm-d","AutoimmuneDisease":"sle","TherapyClass":"Gene-based","IsValidated":True},
 # NO it-e -> Patient E mechanism is not experimentally falsifiable
 {"InterventionTargetId":"it-f","TargetLabel":"IL-17 Pathway Blockade (secukinumab)","CausalMechanism":"cm-f","AutoimmuneDisease":"psa","TherapyClass":"Cell-based","IsValidated":True},
 {"InterventionTargetId":"it-g","TargetLabel":"IL-17 Pathway Blockade (secukinumab)","CausalMechanism":"cm-g","AutoimmuneDisease":"psa","TherapyClass":"Cell-based","IsValidated":True},
])

# --- IndividualPredictions: one per patient ------------------------------
def pred(pid,ind,disease,ptype):
    return {"IndividualPredictionId":pid,"PredictionLabel":pid,"Individual":ind,"AutoimmuneDisease":disease,"PredictionType":ptype}
setdata("IndividualPredictions",[
 pred("pred-a","ind-a-reyes","sle","Onset"),
 pred("pred-b","ind-b-okafor","sle","Onset"),
 pred("pred-c","ind-c-chen","sle","Onset"),
 pred("pred-d","ind-d-santos","sle","Onset"),
 pred("pred-e","ind-e-mensah","sle","Onset"),
 pred("pred-f","ind-f-haidar","psa","Onset"),
 pred("pred-g","ind-g-lin","psa","Onset"),
])

# --- CalibrationBins: well-covered (count 30) for all EXCEPT Patient B -----
# Patient B: CoverageCount 8 (<20) -> no well-calibrated bins -> CalibratedUncertainty 0.
bins=[]
def cb(bid,pred,band,obs,cov):
    return {"CalibrationBinId":bid,"BinLabel":bid,"IndividualPrediction":pred,
            "PredictedProbabilityBand":band,"ObservedEventRate":obs,"CoverageCount":cov}
band_obs=[(0.1,0.08),(0.3,0.31),(0.5,0.52),(0.7,0.68),(0.9,0.92)]  # all |gap|<=0.1
for p in ["a","c","d","e","f","g"]:
    for i,(band,obs) in enumerate(band_obs):
        bins.append(cb(f"cb-{p}-{i}",f"pred-{p}",band,obs,30))
# Patient B: insufficient coverage
for i,(band,obs) in enumerate(band_obs):
    bins.append(cb(f"cb-b-{i}","pred-b",band,obs,8))
setdata("CalibrationBins",bins)

# --- Trim sparse/unused tables to a couple of context rows each -----------
setdata("EnvironmentalExposures",[
 {"EnvironmentalExposureId":"exp-context","ExposureLabel":"Smoking pack-years (context only)","Individual":"ind-a-reyes","ExposureLevel":3.0,"ExposureStartDate":"2015-01-01","IsMaternalEffect":False},
])
setdata("Treatments",[
 {"TreatmentId":"tx-context","TreatmentLabel":"Anifrolumab (context only)","Individual":"ind-a-reyes","AutoimmuneDisease":"sle","TreatmentResponse":"Partial","HasTreatmentInducedChange":True,"HasAdverseEffect":False,"StartDate":"2024-01-01"},
])
setdata("ClinicalPhenotypes",[
 {"ClinicalPhenotypeId":"ph-context","PhenotypeLabel":"ANA elevation (context only)","Individual":"ind-a-reyes","AutoimmuneDisease":"sle","DiseaseStage":"sle-presymptomatic","Tissue":"blood","SeverityScore":4.0,"MeasurementDate":"2024-02-01","HasImmuneDysfunction":True},
])
setdata("EpistaticInteractions",[
 {"EpistaticInteractionId":"epi-context","InteractionLabel":"IRF5 x STAT4 (context only)","Individual":"ind-a-reyes","PrimaryVariant":"var-a-irf5","SecondaryVariant":"var-c-stat4","EpistasisScore":0.4,"HasPleiotropy":False},
])
setdata("CounterfactualTrajectories",[
 {"CounterfactualTrajectoryId":"cf-context","TrajectoryLabel":"Untreated SLE projection (context only)","Individual":"ind-a-reyes","AutoimmuneDisease":"sle","ProjectedSeverity":6.0,"HorizonMonths":12,"InterventionApplied":"none"},
])

json.dump(d, open(RB,"w"), indent=2)
print("seeded mock data. row counts:")
for t in ["Individuals","GenomicVariants","OmicsAssays","EvidenceItems","CohortReplications","NegativeControlTests","CausalMechanisms","InterventionTargets","IndividualPredictions","CalibrationBins"]:
    print(f"  {t}: {len(d[t]['data'])}")
