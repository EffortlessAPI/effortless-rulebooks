"""Evidence for theme C (loop-08): organizing collected knowledge before it is modeled.

(kind, target, justification). Validity is computed by ClaimEvidence.IsValid from Postgres
measurements; nothing here asserts coverage by itself.
"""


def fld(target, why):
    return ("Field", target, why)


def tbl(target, why):
    return ("Table", target, why)


def q(target, why):
    return ("RoleQuestion", target, why)


def prof(target, why):
    return ("OntologyProfile", target, why)


EVIDENCE = {
    # ---------------------------------------------------------------- pkm-1: levels and lenses
    "pkm1-c07": [fld("Steps.StatesOperationalKnowledge",
                     "TRUE for a step that states the three operational-level facts together: it is reached through the transition "
                     "order (what comes before), it names the step it depends on, and it has a precondition that must hold before work "
                     "continues. loto-03, loto-07 and deploy-04 carry all three; steps missing any of them read FALSE.")],
    "pkm1-c08": [tbl("TacticalResourceAllocations",
                     "Each row is tactical knowledge: the capacity allocated to a step's resource, the demand on it, whether that makes a "
                     "bottleneck, and the adjustment decided (order padlocks, fill the night deputy role).")],
    "pkm1-c09": [tbl("ProcessStrategicAlignments",
                     "Each row ties a procedure to the mission of the organization it serves and records its rationale, the value it "
                     "creates and the trade-offs it makes, e.g. lockout's extra hour of downtime per job.")],
    "pkm1-c10": [tbl("ProcessStages",
                     "Stages group a version's steps (Steps.Stage), in sequence, as a product manager runs the release process: build and "
                     "classify, approve, roll out and verify.")],
    "pkm1-c11": [fld("ProcessStages.OwnerRole",
                     "The stakeholder role that owns each stage (release manager owns Approve, site reliability engineer owns Roll out and "
                     "verify); Steps.AssignedRole already names the owner of each step.")],
    "pkm1-c12": [fld("ProcessOutcomeMeasures.ObservedValue",
                     "The measured value of each outcome of a procedure over a period against its target, e.g. change failure rate 33% "
                     "against 5%, zero-energy first-pass rate 60% against 95%.")],
    "pkm1-c13": [tbl("ProcessInterdependencies",
                     "Rows record how one operation requires, helps or hinders another and the mechanism, e.g. deployments during the close "
                     "window restarting the ledger integration: the cross-process dependencies an infrastructure engineer needs.")],
    "pkm1-c14": [fld("ProcessOutcomeMeasures.BusinessOutcome",
                     "Links each process measure to the business outcome it drives (change failure rate to platform availability, owned by "
                     "the VP of engineering), with LinkRationale saying how.")],
    "pkm1-c34": [tbl("StepContextSensitivities",
                     "One row per way a step's significance changes with context, typed by ContextFactor Performer, Resource or "
                     "Circumstance: a deputy verifying on nights, padlocks running short, a press carrying an accumulator retrofit.")],
    "pkm1-p02": [fld("ProcessKnowledgeLevels.LacksCaptureStrategyForEitherForm",
                     "Fires on a level that has no capture strategy for its tacit knowledge or none for its explicit knowledge. The strategic "
                     "level has only strategy memos (explicit) and nothing for the executives' tacit judgment, so it fires; operational and "
                     "tactical have both and do not. That is the prescription that each level needs its own strategy for both forms broken.")],
    "pkm1-p03": [fld("Procedures.PrivilegesSingleStakeholderView",
                     "Fires when a procedure is organized for exactly one stakeholder lens. The press-7 die changeover is organized only as "
                     "the trainer's view, so every other stakeholder (auditor, manager, executive) is unserved; lockout and deployment serve "
                     "several lenses and do not fire. That is one stakeholder view privileged over the rest.")],
    "pkm1-p14": [fld("TermRelations.AssertsIndirectLinkAsDirect",
                     "Fires when a skos:broader link, which must be direct, is asserted from a concept to its grandparent. The SOP index "
                     "linked accumulator bleed-down straight to lockout/tagout although it sits under energy isolation, so it fires; the "
                     "associative relations do not. The rule that a direct relation cannot stand in for an indirect one is enforced.")],
    "pkm1-p15": [fld("ExecutionEntities.ViolatesDeclaredDatatypeOrFormat",
                     "Fires when a recorded value's datatype or media type differs from what its step variable declares. The 2026-07-14 hotfix "
                     "recorded its risk report as a text/plain chat message where deploy-02 declares a RiskReport in application/json, so it "
                     "fires; the conforming PDF verification record and JSON risk report do not. Expected datatype and format are enforced.")],
    "pkm1-p28": [fld("Steps.IsContextSensitiveButUnscoped",
                     "Fires when a step's significance depends on context and a recorded dependency names no applicability scope. loto-03's "
                     "dependence on the press-7 retrofit has no scope, so it fires; the night-shift and EU-freeze dependencies point at one "
                     "compact ApplicabilityScopes row and do not. Context is captured by explicit scoping rather than by forking the procedure.")],
    "pkm1-q05": [q("aq-pkm1-q05",
                   "The operations manager's bottleneck question, answered by TacticalResourceAllocations.IsBottleneck and "
                   "Steps.IsBottleneckStep: north-station padlocks and night verifier hours are demanded beyond capacity.")],
    "pkm1-q06": [q("aq-pkm1-q06",
                   "The VP's question on why each procedure exists and what it trades off, answered by ProcessStrategicAlignments and "
                   "Procedures.HasNoStatedReasonForExisting / ProcessStrategicAlignments.StatesNoTradeOff.")],
    "pkm1-q07": [q("aq-pkm1-q07",
                   "The VP's question on how performance connects to business outcomes, answered by ProcessOutcomeMeasures.BusinessOutcome "
                   "and Procedures.MeasuresPerformanceWithoutBusinessLink (conveyor maintenance is measured but tied to nothing).")],
    "pkm1-s06": [prof("sparql-11-sd",
                      "SPARQL 1.1 Service Description's named graphs: ApplicabilityScopes maps to sd:NamedGraph and NamedGraphIri to sd:name, "
                      "so knowledge scoped to a context is kept in the named graph where it holds.")],
    "pkm1-i04": [fld("LevelCaptureStrategies.ContradictsKnowledgeForm",
                     "Fires when tacit knowledge is to be passed on only as a stored document, or explicit knowledge only in person. The "
                     "shift planners' tacit staffing heuristics were written up once as a wiki page, so it fires; shadowing veterans "
                     "(tacit, in person) and encoding the SOP (explicit, codified) do not. It witnesses the chart's contrast: explicit "
                     "knowledge is codified and stored, tacit knowledge is passed on in person.")],
    "pkm1-i05": [fld("ProcessLevelStatements.IsFiledAtWrongLevel",
                     "Fires when a statement answers a question its level does not answer in the planning pyramid (LevelPyramidQuestions: "
                     "strategic who and why, tactical what, operational how and where). The deployment's reason for gating releases (a why) "
                     "was filed among operational statements, so it fires; the lockout statements filed by the pyramid do not.")],
    "pkm1-p06": [fld("Resources.IsContentWithoutOrganization",
                     "Fires when a resource has content (a description) but lacks structure (no type from the artifact-type scheme), "
                     "findability (no keywords) or linkage (no procedure or step references it). The press-7 changeover PDF has content but "
                     "no keywords, so it fires; the typed, keyworded, referenced 2019 lockout SOP does not. Content alone does not pass.")],
    "pkm1-p40": [fld("Procedures.LacksManagedControlledVocabulary",
                     "Fires when a procedure with specified steps has no controlled vocabulary whose metadata (governing role, "
                     "establishment date) is managed. Conveyor maintenance has only an ungoverned PDF glossary, so it fires; lockout, "
                     "deployment, close and policy each have a governed scheme and do not.")],
    # ---------------------------------------------------------------- pkm-2: from collection to structure
    "pkm2-c40": [fld("CollectedSourceMaterials.MaterialKind",
                     "Each collected item is typed as Transcript, FieldNotes, ProcessMap, MinedEventTrace or DocumentExcerpt, the raw "
                     "material collection produces, with when it was organized and modeled.")],
    "pkm2-c59": [fld("Procedures.ProcedureType",
                     "Each procedure instance is assigned a category of the process taxonomy (safety-procedure, software-release, "
                     "support-runbook, ...); IsInconsistentlyCategorized checks the category is a suitable leaf.")],
    "pkm2-c62": [fld("Vocabularies.SchemeKind",
                     "Distinguishes thesauri (the lockout, release and role schemes, which carry broader and related relations) from plain "
                     "controlled vocabularies; the process taxonomy is ProcedureTypes.")],
    "pkm2-c63": [fld("ApplicabilityScopes.BusinessUnit", "The business unit a scoped piece of knowledge applies in (acme-plant, acme-engineering).")],
    "pkm2-c64": [fld("ApplicabilityScopes.Geography", "The geography a scoped piece of knowledge applies in (US-OH, EU).")],
    "pkm2-c65": [fld("ApplicabilityScopes.CustomerSegment", "The customer segment a scoped piece of knowledge applies to (Enterprise tenants).")],
    "pkm2-c66": [fld("ApplicabilityScopes.RegulatoryRegime", "The regulatory regime a scoped piece of knowledge applies under (hazardous energy control standard, change control policy).")],
    "pkm2-c67": [tbl("SituationalVariants",
                     "Each row is how an expert adapts the standard procedure to a situation: the situation, the adaptation, the expert it "
                     "came from and its scope, e.g. bleeding the press-7 accumulator first.")],
    "pkm2-c68": [fld("ApplicabilityScopes.ExclusionCondition",
                     "A scoping annotation that states where the knowledge stops applying, e.g. the night-shift scope stops applying once "
                     "a second verifier is rostered.")],
    "pkm2-c69": [fld("StakeholderLenses.NeedsStepGuidance", "TRUE for the customer service agent lens (and the trainer): they need step-by-step guidance.")],
    "pkm2-c70": [fld("StakeholderLenses.NeedsMetrics", "TRUE for the operations manager lens (also product manager and executive): they need metrics.")],
    "pkm2-c71": [fld("StakeholderLenses.NeedsExceptionHandling", "TRUE for the operations manager lens: a manager needs exception handling.")],
    "pkm2-c72": [fld("StakeholderLenses.NeedsComplianceEvidence", "TRUE for the compliance auditor lens: an auditor needs compliance evidence.")],
    "pkm2-c73": [fld("StakeholderLenses.NeedsStructuredConstraints", "TRUE for the AI system lens, served a machine-readable step-level view: an AI system needs structured constraints.")],
    "pkm2-c74": [fld("ProcedureTypes.TaxonomyRank",
                     "Marks the top of the process taxonomy as FunctionalDomain nodes (Operations, Finance, Human Resources, Technology, "
                     "Customer Service) with process groups and process types beneath.")],
    "pkm2-c75": [fld("ProcedureTypes.BroaderProcedureType",
                     "Each process group and process type names its broader node, so a functional domain breaks down into narrower levels "
                     "(Operations > Equipment Safety > Equipment Safety Procedure).")],
    "pkm2-c76": [fld("TermLabelVariants.LabelKind",
                     "The activity thesauri record one row per wording as pref (the canonical label, e.g. 'release approval review') or "
                     "alt (variants such as 'sign-off', 'go/no-go').")],
    "pkm2-c77": [fld("Roles.SpecializesRole", "One role specializes another: Senior Maintenance Technician specializes Maintenance Technician; the night safety deputy specializes the plant safety officer.")],
    "pkm2-c78": [fld("Roles.Organization", "Each role sits in an organization, and for departments (ACME Finance, People Operations, Legal) that records which department contains the role.")],
    "pkm2-c79": [fld("Roles.Responsibility", "The responsibility each role carries, recorded on the role (e.g. confirms zero-energy states and receives escalated danger cues).")],
    "pkm2-c80": [fld("VocabularyTerms.Definition", "Every controlled concept carries a definition, mapped to skos:definition.")],
    "pkm2-c81": [fld("VocabularyTerms.ScopeNote", "A SKOS scope note saying how a concept is to be used, e.g. zero-energy verification is for the check after isolation, not the restart try-out.")],
    "pkm2-c82": [fld("VocabularyTerms.BroaderTerm", "The direct broader concept (skos:broader); narrower concepts are its inverse (Lockout/Tagout is broader than Energy Isolation).")],
    "pkm2-c83": [tbl("TermRelations", "Associative relations between concepts (skos:related), e.g. change risk classification related to release approval review.")],
    "pkm2-p19": [fld("CollectedSourceMaterials.IsModeledBeforeOrganized",
                     "Fires when material was encoded into a procedure version before, or without, being organized into a scheme. The "
                     "conveyor manual excerpt was imported into convmaint-v1.0.0 and never organized, so it fires; the lockout transcript and "
                     "SOP excerpt were organized on 10 May and modeled on 12 May and do not.")],
    "pkm2-p20": [fld("Vocabularies.IsSingleKindFrame",
                     "Fires when a scheme organizes several materials but only one kind of knowledge. The conveyor glossary holds two "
                     "document excerpts and nothing else, so it fires; the lockout thesaurus holds transcripts, field notes, a process map "
                     "and SOP excerpts in one frame and does not.")],
    "pkm2-p21": [fld("SituationalVariants.DivergesWithoutStatedConditions",
                     "Fires when a variant departs from the standard procedure and nothing states when it applies or where it stops. The "
                     "hotfix no-canary variant (scope with no conditions) and night self-verification (no scope) fire; the press-7 "
                     "accumulator and EU-freeze variants state their conditions and do not.")],
    "pkm2-p23": [fld("VocabularyTerms.IsOrganizedAroundOfficialTerm",
                     "Fires when practitioners repeatedly use an alternative wording more than the preferred label the scheme is built "
                     "around. Technicians said 'loto' three times and never 'lockout/tagout'; the release manager said 'sign-off' and "
                     "'go/no-go', never 'release approval review'. Zero-energy verification, used in its preferred wording, does not fire.")],
    "pkm2-p24": [fld("ProcedureLensViews.IsDisconnectedSilo",
                     "Fires when a stakeholder view is not projected from the procedure's current version. The lockout executive table was "
                     "built from the deprecated 2019 version and the deployment executive deck is free-standing, so both fire; the trainer, "
                     "auditor and AI views project the current version and do not. Multiple perspectives stay one procedure.")],
    "pkm2-p25": [fld("ProcedureTypes.IsArbitraryGrouping",
                     "Fires when a grouping names no distinguishing facet, or none of its members has the distinction it names. "
                     "'Customer-Facing Support Runbook' claims audience Customers but its one member is for support staff, so it fires; "
                     "Equipment Safety Procedure's members all share hazard HazardousEnergy and it does not.")],
    "pkm2-p26": [fld("Vocabularies.IsFrozenDespiteNewCollection",
                     "Fires when material was collected after a scheme was established and the scheme was never refined. The release "
                     "thesaurus (December 2025) now organizes a July 2026 event trace with no refinement, so it fires; the lockout thesaurus "
                     "was refined after the process map and the night field notes and does not.")],
    "pkm2-p27": [fld("ProcedureLensViews.IsGranularityMisfit",
                     "Fires when a view's granularity differs from what its lens's use requires. The strategic planner was given lockout at "
                     "step level, and the support agent got deployment only at category level, so both fire; the trainer's step view and the "
                     "executive's category view do not.")],
    "pkm2-p28": [fld("ProcedureVersions.MixesGranularityAtOneLevel",
                     "Fires when one level of a version mixes whole activities with single actions. The 2019 lockout checklist puts 'Do the "
                     "maintenance' (an Activity) beside 'Apply locks' and 'Disconnect power' (Actions), so it fires; loto-v2.0.0 keeps its "
                     "actions as sub-steps and does not.")],
    "pkm2-p34": [fld("Vocabularies.OntologyPrecededVocabularyControl",
                     "Fires when ontology modeling for a domain began before its vocabulary control was established. Release modeling "
                     "started 3 November 2025 and the release thesaurus was established 10 December, so it fires; lockout vocabulary (1 May) "
                     "preceded its modeling (12 May) and does not.")],
    "pkm2-p35": [fld("Procedures.IsInconsistentlyCategorized",
                     "Fires when a procedure has no type, is filed at domain or group rank rather than a process type, or its type is "
                     "undefined. The new forklift inspection was filed straight under the Operations domain, so it fires; its peers sit at "
                     "defined process types and do not.")],
    "pkm2-p36": [fld("SourceTermMentions.IsUncontrolledWording",
                     "Fires when a wording found in collected material resolves to no label in its scheme. The runbook's 'release gate "
                     "check' (the same review as sign-off) fires; 'sign-off' and 'go/no-go' resolve through alternative labels to Release "
                     "Approval Review and do not.")],
    "pkm2-p37": [fld("Roles.IsMissedByPhraseQuery",
                     "Fires when a role is mentioned in some process material in words the role vocabulary does not resolve, so a query "
                     "through the vocabulary misses that process. The night field notes say 'the tech', so Maintenance Technician fires; "
                     "'release captain' resolves to Release Manager and 'safety lead' to Plant Safety Officer, which do not.")],
    "pkm2-p38": [fld("ProcedureTypes.IsUnreachableByNavigation",
                     "Fires when navigating down from the functional domains never reaches a taxonomy node. The Metrology group hangs off "
                     "no domain, so it and Instrument Calibration Procedure beneath it fire; every other node is reachable and does not.")],
    "pkm2-q09": [q("aq-pkm2-q09",
                   "The auditor's request, answered by Procedures.InvolvesComplianceReviewRole over steps whose role carries the "
                   "compliance-review capability tag (lockout, deployment, close).")],
    "pkm2-q21": [q("aq-pkm2-q21",
                   "The auditor's question, answered by SituationalVariants through ApplicabilityScopes (business unit, geography, segment, "
                   "regime) with HasNoApplicabilityDimension and DivergesWithoutStatedConditions.")],
    "pkm2-q28": [q("aq-pkm2-q28", "The steward's question, answered by Roles.Organization and OrganizationType, with IsNotHousedInDepartment for roles outside any department.")],
    "pkm2-q29": [q("aq-pkm2-q29", "The steward's question, answered by Roles.SpecializesRole and HasSpecializations, with IsSeniorVariantNotSpecialization.")],
    "pkm2-q30": [q("aq-pkm2-q30", "The product manager's question, answered by ProcedureTypes.BroaderProcedureType, TaxonomyRank and HasNarrowerTypes.")],
    "pkm2-q31": [q("aq-pkm2-q31",
                   "The product manager's question, answered by Procedures.HasStepAndCategoryResolutions and IsMissingDemandedResolution over "
                   "step-level and category-level lens views.")],
    "pkm2-s09": [prof("skos",
                      "SKOS mappings: Vocabularies to skos:ConceptScheme, VocabularyTerms to skos:Concept with prefLabel, altLabel, definition, "
                      "scopeNote, broader and inScheme; TermRelations to skos:related.")],
    "pkm2-s23": [prof("sparql-11-sd",
                      "Named graphs for scoping context: each ApplicabilityScopes row is mapped to sd:NamedGraph and carries its graph IRI "
                      "(sd:name).")],
    "pkm2-s22": [("KnowledgeMethod", "FacetedClassification",
                  "Applied by MethodApplications row ma8-faceted-procedure-types: procedure types are classified along four independent "
                  "facets (ClassificationFacets: functional domain, hazard, audience, change type), each procedure carries a value per "
                  "facet in ProcedureFacetAssignments, and ProcedureTypes.IsArbitraryGrouping checks members against the facet their "
                  "grouping names.")],
    "pkm3-s02": [("KnowledgeMethod", "LayeredVocabularyToOntology",
                  "Applied by MethodApplications row ma8-layered-lockout: the lockout activity thesaurus (Vocabularies, VocabularyTerms with "
                  "SKOS labels and relations) and the process taxonomy (ProcedureTypes) were established before the lockout procedure was "
                  "aligned to the PKO 2.0.0 ontology; Vocabularies.OntologyPrecededVocabularyControl checks that order per domain.")],
    "pkm2-i04": [fld("VocabularyTerms.HasUnreconciledVariantPhrasings",
                     "Fires when one concept appears under several phrasings across sources and some are not reconciled to it. Release "
                     "Approval Review appears as 'sign-off' (transcript), 'go/no-go' (transcript) and 'release gate check' (runbook), the "
                     "last unreconciled, so it fires; Zero-Energy Verification's phrasings all resolve and it does not.")],
    "pkm2-i05": [fld("Roles.IsSeniorVariantNotSpecialization",
                     "Fires when a senior version of a role is not recorded as a specialization of a role in its own family. Lead Release "
                     "Manager specializes nothing, so it fires; Senior Maintenance Technician specializes Maintenance Technician and does not.")],
    "pkm2-i10": [fld("Procedures.IsMissingDemandedResolution",
                     "Fires when a lens needs step-level detail the procedure lacks, or a category it is not classified into. The press-7 "
                     "changeover has a trainer's view but no steps (training needs step detail), and forklift inspection has a strategic "
                     "planner's view but no process-type category; both fire. Lockout serves step and category readers and does not.")],
    "pkm2-i14": [fld("OntologyProfiles.SkipsAdoptionPath",
                     "Fires when a later-stage standard was adopted before the earlier-stage standard it builds on. DMN (decision modeling, "
                     "stage 3) was adopted in May 2025, before PKO (lightweight ontology, stage 2) in September, so it fires; PKO, DCAT, "
                     "P-Plan, FOAF and schema.org follow SKOS and do not. Unadopted standards are not flagged.")],
    # ---------------------------------------------------------------- pkm-3, pkm-4
    "pkm3-c22": [fld("ProcessInterdependencies.Effect",
                     "Records whether one operation Helps, Hinders or Requires another in the value chain (a verified zero-energy record "
                     "helps roller replacement; press-7 lockouts hinder the die changeover), the knowledge lost when parts of the chain go.")],
    "pkm3-q04": [q("aq-pkm3-q04",
                   "The infrastructure engineer's question, answered by ProcessInterdependencies.Effect and IsHinderingDependency and by "
                   "Procedures.IsHinderedByAnotherOperation.")],
    "pkm4-i08": [fld("TermLabelVariants.IsCrossSchemeDuplicatePref",
                     "Fires when the same concern is defined as a separate preferred concept in more than one scheme instead of once and "
                     "shared. 'quality control' is defined both in the shared agent-capability scheme and again in the plant's local quality "
                     "terms, and the maintenance technician is tagged with the plant copy; both labels fire. 'validation', shared by the "
                     "safety officer and the site reliability engineer across plant and engineering, does not. It witnesses that these "
                     "problems cut across disciplines and must not be split by domain.")],
    # ---------------------------------------------------------------- ont-4: drift, deprecation, schemes
    "ont4-c04": [tbl("TermMeaningChanges",
                     "Each row records that the organization's meaning of a controlled term changed (prior and new meaning, when); "
                     "VocabularyTerms.HasStaleDefinition reads it against the term's definition, e.g. 'release' still defined the 2024 way.")],
    "ont4-c08": [fld("VocabularyTerms.NamespaceIri",
                     "The namespace each concept belongs to; re-homed terms such as 'given name' carry the local urn:effortless:pko-extension# "
                     "namespace in place of FOAF's.")],
    "ont4-c09": [fld("VocabularyTerms.ConceptIri", "A globally unique IRI for every controlled concept, mapped to the RDF 1.1 IRI identifier.")],
    "ont4-c10": [fld("OntologyProfiles.Version",
                     "One record per reused external standard (SKOS, FOAF, schema.org, DCAT, PKO, ...) holding the version the model depends "
                     "on, with its version IRI and namespace.")],
    "ont4-c11": [fld("ExternalStandardTerms.DeprecatedBySource",
                     "TRUE for an adopted external term its source standard marks deprecated (foaf:givenname, schema:serviceAudience, "
                     "schema:vendor), with DeprecatedAt and StillResolves.")],
    "ont4-c12": [fld("VocabularyTerms.SameAsIri",
                     "The reconciliation relation: the local concept asserts identity (owl:sameAs) with the deprecated external term, e.g. "
                     "local 'given name' sameAs foaf:givenname.")],
    "ont4-c15": [fld("Vocabularies.ModelLayer",
                     "Marks controlled concept schemes (status, artifact types, capabilities, activities, roles) as the VocabularyLayer, "
                     "held apart from the ontology profiles.")],
    "ont4-c32": [fld("LifecycleStatuses.WorkflowStatusConcept",
                     "Maps every lifecycle status to a concept of the controlled workflow status scheme (active, draft, deprecated).")],
    "ont4-c33": [fld("Resources.ArtifactTypeConcept",
                     "Types each resource by a concept of the controlled artifact-type scheme (standard operating procedure, engineering "
                     "drawing, runbook, training media, register entry).")],
    "ont4-c34": [fld("Vocabularies.GovernedDimension",
                     "Names what each scheme controls; voc-agent-capabilities is the AgentCapability scheme (compliance review, validation, "
                     "risk scoring, automated deployment).")],
    "ont4-c35": [tbl("RoleCapabilityTags",
                     "Roles carry capability tags drawn from the capability scheme (the change risk classifier is tagged risk scoring), with "
                     "IsTagOutsideCapabilityScheme catching a tag from another scheme.")],
    "ont4-p01": [fld("VocabularyTerms.HasStaleDefinition",
                     "Fires when the organization's meaning of a term changed after its definition was last revised. 'release' changed in "
                     "January 2026 to a risk-classified, gated, pipeline rollout while its definition still describes an engineer copying a "
                     "build (revised 2024), so it fires; 'rollback' and 'zero-energy verification' were redefined when their meaning changed "
                     "and do not.")],
    "ont4-p03": [fld("ExternalStandardTerms.IsAlignmentStaleAfterDeprecation",
                     "Fires when the source deprecated a term and our alignment and documentation were not updated afterwards, even though "
                     "the IRI still resolves. schema:serviceAudience (deprecated September 2025, still mapped from StakeholderLenses) fires; "
                     "schema:vendor, whose alignment was updated a month after deprecation, does not.")],
    "ont4-p04": [fld("ExternalStandardTerms.IsNeededDeprecatedTermNotRehomed",
                     "Fires when the model still needs a deprecated external term and has not taken it into the local namespace. "
                     "schema:vendor is still needed for supplier organizations and was not re-homed, so it fires; the FOAF name terms were "
                     "re-homed and serviceAudience is no longer needed, so they do not.")],
    "ont4-p05": [fld("ExternalStandardTerms.IsRehomedWithoutIdentityLink",
                     "Fires when a re-homed local concept does not assert identity with the external original. Local 'surname' carries no "
                     "sameAs to foaf:surname, so it fires; 'given name' asserts sameAs foaf:givenname and does not.")],
    "ont4-p06": [fld("ExternalStandardTerms.IsRehomedWithoutNewRelease",
                     "Fires when a term was re-homed without a model release, issued after the deprecation, that introduces it. The local "
                     "'has version' replacing dct:hasVersion names no release, so it fires; the FOAF name terms arrived in release "
                     "0.9.1 and do not.")],
    "ont4-p07": [fld("ExternalStandardTerms.HasUnpropagatedIdentifierChange",
                     "Fires when a standard changed a term's prefix or identifier and some mapping still uses the old one. schema.org moved "
                     "from http to https; the Procedures mapping still targets http://schema.org/HowTo, so HowTo fires, while HowToStep's "
                     "mapping was updated and does not. A query against the old IRI would silently find nothing.")],
    "ont4-p59": [fld("SourceTermMentions.IsNonCanonicalGeneratedValue",
                     "Fires when an AI-generated value is not a preferred label of the governed scheme. The ungrounded run's 'live', "
                     "'shipped', 'work in progress' and 'retired' fire; the grounded run's 'active', 'draft' and 'deprecated' do not. "
                     "Practitioner and document wordings are never flagged by it.")],
    "ont4-p62": [fld("TermLabelVariants.IsAmbiguousLabel",
                     "Fires when one wording is a label of more than one concept in a scheme, so alternative labels cannot normalize it to "
                     "one canonical concept. 'review' is an alternative label of both Release Approval Review and Change Risk "
                     "Classification, so both fire; 'sign-off' resolves to exactly one concept and does not.")],
    "ont4-q26": [q("aq-ont4-q26",
                   "The knowledge engineer's question, answered by ExternalStandardTerms.IsAdoptedButDeprecated over DeprecatedBySource and "
                   "the mappings and re-homings that adopt each term.")],
    "ont4-s03": [prof("skos",
                      "SKOS concept schemes: mappings for skos:ConceptScheme, skos:Concept, prefLabel, altLabel, definition and inScheme on "
                      "Vocabularies and VocabularyTerms.")],
    "ont4-s08": [prof("foaf-0-99", "FOAF 0.99 is a recorded dependency with a mapping (Agents.DisplayName to foaf:name) and adopted terms in ExternalStandardTerms.")],
    "ont4-s09": [prof("dcat-3", "DCAT 3 is a recorded dependency with semantic mappings (Resources.Keywords to dcat:keyword and others) and its major revision recorded.")],
    "ont4-s10": [prof("schema-org", "Schema.org is a recorded dependency with mappings (ProcedureLensViews to schema:HowTo, Steps to schema:HowToStep) and its deprecations tracked.")],
    "ont4-s11": [prof("rdfs-1-1", "RDF Schema mappings: ProcedureTypes.Label to rdfs:label and ProcedureTypes.Definition to rdfs:comment for term names and definitions.")],
    "ont4-s12": [prof("owl-2", "OWL 2 mapping of VocabularyTerms.SameAsIri to owl:sameAs, the identity assertion between a re-homed local term and the deprecated original.")],
    "ont4-s14": [prof("rdf-1-1", "RDF 1.1 Concepts mapping of VocabularyTerms.ConceptIri: every concept is identified by a globally unique IRI.")],
    "ont4-i09": [fld("ExternalStandardTerms.RehomingKeptExternalNamespace",
                     "Fires when a deprecated FOAF personal-name term said to be re-homed still sits in FOAF's namespace. The local copy of "
                     "foaf:family_name kept http://xmlns.com/foaf/0.1/ as its namespace, so it fires; 'given name' was moved into "
                     "urn:effortless:pko-extension# and linked to foaf:givenname, the illustration done correctly, and does not.")],
    "ont4-i12": [fld("VocabularyTerms.HasStructuralSenseShiftAcrossYears",
                     "Fires when what a term refers to changed shape after holding for about two years or more. 'release' meant an engineer "
                     "copying a tagged build from January 2024 and, since January 2026, a risk-classified change through a human gate and a "
                     "pipeline, so it fires; 'rollback', reworded within a year without structural change, does not.")],
    "ont4-i13": [fld("OntologyProfiles.IsReviewOverdueForChangeRate",
                     "Reads each standard's change rate (ChangeRateProfile: FOAF Dormant since 2014, schema.org FrequentDeprecation with two "
                     "deprecations in two years, DCAT RecentMajorRevision in 2024) and fires when a fast-changing one has not had our "
                     "dependency reviewed in 180 days. schema.org, last reviewed September 2025, fires; DCAT, reviewed June 2026, and "
                     "dormant FOAF do not.")],
    "ont4-i16": [fld("AiLabelingRuns.IsUngroundedSynonymSprawl",
                     "Fires when an ungrounded run produced values outside the canonical scheme. The ungrounded status run returned 'live', "
                     "'shipped', 'work in progress' and 'retired', so it fires; the same notes grounded in the workflow status scheme "
                     "returned only active, draft and deprecated, and do not.")],
    "ont4-i17": [fld("AiLabelingRuns.GroundedInNonMachineReadableScheme",
                     "Fires when a model was grounded in a vocabulary that is not formal and machine-accessible. The support run was "
                     "grounded in a PDF glossary and still produced 'looking into it', so it fires; the run grounded in the SKOS workflow "
                     "status scheme does not. Grounding helps at scale only when the vocabulary is formal.")],
}
