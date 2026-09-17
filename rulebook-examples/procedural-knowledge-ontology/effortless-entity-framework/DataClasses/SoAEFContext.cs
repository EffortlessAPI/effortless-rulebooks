using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SqlOnAir.DotNet.Lib.DataClasses.BaseClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses
{
    public class SoAEFContext : DbContext
    {
        /// <summary>
        /// Controls whether missing database contexts should throw errors or return null.
        /// When true (default), navigation properties will throw InvalidOperationException when context is missing.
        /// When false, navigation properties will return null when context is missing.
        /// </summary>
        public static bool ThrowErrorOnContextMissing { get; set; } = true;

        private bool _freezeComputedValues;

        /// <summary>
        /// Declares the tracked rows a read-only snapshot. While true, each computed property is
        /// evaluated once per row and each table's rows are read once, so reading every computed
        /// value is linear in the data instead of re-deriving every dependency on every read.
        /// Setting it (either way) discards the previous snapshot; change detection is off while
        /// frozen, because nothing may change.
        /// </summary>
        public bool FreezeComputedValues
        {
            get => _freezeComputedValues;
            set
            {
                _freezeComputedValues = value;
                ComputedSnapshot = value ? new Formulas.EfFormulaFns.Snapshot() : null;
                ChangeTracker.AutoDetectChangesEnabled = !value;
            }
        }

        public Formulas.EfFormulaFns.Snapshot? ComputedSnapshot { get; private set; }

        public SoAEFContext(DbContextOptions<SoAEFContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            ChangeTracker.Tracked += ChangeTracker_Tracked;
        }

        private void ChangeTracker_Tracked(object? sender, EntityTrackedEventArgs e)
        {
            if (e.Entry.Entity is SoAEntityBase entity)
            {
                entity.SetContext(this);
            }
        }

        public DbSet<RulebookRelease> RulebookReleases { get; set; }
        public DbSet<OntologyProfile> OntologyProfiles { get; set; }
        public DbSet<EvaluationContext> EvaluationContexts { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<CommunitiesOfPractice> CommunitiesOfPractice { get; set; }
        public DbSet<Mentorship> Mentorships { get; set; }
        public DbSet<ProcedureType> ProcedureTypes { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureVersion> ProcedureVersions { get; set; }
        public DbSet<ProcedureVersionLink> ProcedureVersionLinks { get; set; }
        public DbSet<ProcedureStatusChange> ProcedureStatusChanges { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<StepTransition> StepTransitions { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<StepAction> StepActions { get; set; }
        public DbSet<StepFunction> StepFunctions { get; set; }
        public DbSet<StepTool> StepTools { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<StepRequirement> StepRequirements { get; set; }
        public DbSet<StepVerification> StepVerifications { get; set; }
        public DbSet<Rationale> Rationales { get; set; }
        public DbSet<Exception> Exceptions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProcedureResource> ProcedureResources { get; set; }
        public DbSet<ElicitationSession> ElicitationSessions { get; set; }
        public DbSet<KnowledgeFragment> KnowledgeFragments { get; set; }
        public DbSet<KnowledgeGap> KnowledgeGaps { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Explanation> Explanations { get; set; }
        public DbSet<ProcedureExecution> ProcedureExecutions { get; set; }
        public DbSet<StepExecution> StepExecutions { get; set; }
        public DbSet<RequirementSatisfaction> RequirementSatisfactions { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<IssueOccurrence> IssueOccurrences { get; set; }
        public DbSet<UserQuestion> UserQuestions { get; set; }
        public DbSet<UserFeedback> UserFeedback { get; set; }
        public DbSet<StewardshipAssignment> StewardshipAssignments { get; set; }
        public DbSet<ChangeRequest> ChangeRequests { get; set; }
        public DbSet<ReviewEvent> ReviewEvents { get; set; }
        public DbSet<LearningActivity> LearningActivities { get; set; }
        public DbSet<OperationalBinding> OperationalBindings { get; set; }
        public DbSet<CommunicationPolicy> CommunicationPolicies { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<SemanticMapping> SemanticMappings { get; set; }
        public DbSet<WitnessLoop> WitnessLoops { get; set; }
        public DbSet<RoleQuestion> RoleQuestions { get; set; }
        public DbSet<RulebookField> RulebookFields { get; set; }
        public DbSet<TestSuite> TestSuites { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<ERBVersion> ERBVersions { get; set; }
        public DbSet<ERBCustomization> ERBCustomizations { get; set; }
        public DbSet<__meta__> __meta__ { get; set; }
        public DbSet<ExceptionInvocation> ExceptionInvocations { get; set; }
        public DbSet<VerificationOutcome> VerificationOutcomes { get; set; }
        public DbSet<ObservedTransition> ObservedTransitions { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<MessageDelivery> MessageDeliveries { get; set; }
        public DbSet<TemplateApproval> TemplateApprovals { get; set; }
        public DbSet<SendIntent> SendIntents { get; set; }
        public DbSet<AgentDecisionRecord> AgentDecisionRecords { get; set; }
        public DbSet<DeliveredCommunication> DeliveredCommunications { get; set; }
        public DbSet<AuthorityBoundary> AuthorityBoundaries { get; set; }
        public DbSet<BindingObservation> BindingObservations { get; set; }
        public DbSet<Attestation> Attestations { get; set; }
        public DbSet<AppRoleProfile> AppRoleProfiles { get; set; }
        public DbSet<AppNavGroup> AppNavGroups { get; set; }
        public DbSet<AppRoute> AppRoutes { get; set; }
        public DbSet<AppRouteQuestion> AppRouteQuestions { get; set; }
        public DbSet<AppRouteReference> AppRouteReferences { get; set; }
        public DbSet<RulebookTable> RulebookTables { get; set; }
        public DbSet<AccessPrincipal> AccessPrincipals { get; set; }
        public DbSet<AccessPolicy> AccessPolicies { get; set; }
        public DbSet<FieldGrant> FieldGrants { get; set; }
        public DbSet<RoleSchema> RoleSchemas { get; set; }
        public DbSet<RoleSchemaView> RoleSchemaViews { get; set; }
        public DbSet<JwtClaimMapping> JwtClaimMappings { get; set; }
        public DbSet<AccessDenialTest> AccessDenialTests { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<PrincipalAssignment> PrincipalAssignments { get; set; }
        public DbSet<IssuedToken> IssuedTokens { get; set; }
        public DbSet<ProcessMiningRun> ProcessMiningRuns { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<VocabularyTerm> VocabularyTerms { get; set; }
        public DbSet<KnowledgeBrokerLink> KnowledgeBrokerLinks { get; set; }
        public DbSet<ConformanceSubstrate> ConformanceSubstrates { get; set; }
        public DbSet<ConformanceRun> ConformanceRuns { get; set; }
        public DbSet<SubstrateRunScore> SubstrateRunScores { get; set; }
        public DbSet<TableConformance> TableConformance { get; set; }
        public DbSet<FieldDisagreement> FieldDisagreements { get; set; }
        public DbSet<CellDisagreement> CellDisagreements { get; set; }
        public DbSet<KnowledgeMethod> KnowledgeMethods { get; set; }
        public DbSet<SourceArticle> SourceArticles { get; set; }
        public DbSet<ArticleClaim> ArticleClaims { get; set; }
        public DbSet<ClaimEvidence> ClaimEvidence { get; set; }
        public DbSet<MethodApplication> MethodApplications { get; set; }
        public DbSet<LifecycleStatuse> LifecycleStatuses { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<MachineType> MachineTypes { get; set; }
        public DbSet<EnergySource> EnergySources { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineEnergySource> MachineEnergySources { get; set; }
        public DbSet<LockDevice> LockDevices { get; set; }
        public DbSet<ProtectiveEquipment> ProtectiveEquipment { get; set; }
        public DbSet<StepLockRequirement> StepLockRequirements { get; set; }
        public DbSet<StepProtectiveEquipment> StepProtectiveEquipment { get; set; }
        public DbSet<RegulatoryFramework> RegulatoryFrameworks { get; set; }
        public DbSet<ProcedureTarget> ProcedureTargets { get; set; }
        public DbSet<ProcedureAdoption> ProcedureAdoptions { get; set; }
        public DbSet<ProcedureOutcomeCriteria> ProcedureOutcomeCriteria { get; set; }
        public DbSet<RelationType> RelationTypes { get; set; }
        public DbSet<ActivityRelation> ActivityRelations { get; set; }
        public DbSet<StepVariable> StepVariables { get; set; }
        public DbSet<ExecutionEntity> ExecutionEntities { get; set; }
        public DbSet<StepCondition> StepConditions { get; set; }
        public DbSet<ConditionCheck> ConditionChecks { get; set; }
        public DbSet<FailureMode> FailureModes { get; set; }
        public DbSet<StepCue> StepCues { get; set; }
        public DbSet<CueObservation> CueObservations { get; set; }
        public DbSet<DecisionPoint> DecisionPoints { get; set; }
        public DbSet<ExecutionParticipant> ExecutionParticipants { get; set; }
        public DbSet<StepResource> StepResources { get; set; }
        public DbSet<FaqCategory> FaqCategories { get; set; }
        public DbSet<FaqTarget> FaqTargets { get; set; }
        public DbSet<AuthoringSubmission> AuthoringSubmissions { get; set; }
        public DbSet<ProcessKnowledgeLevel> ProcessKnowledgeLevels { get; set; }
        public DbSet<LevelCaptureStrategy> LevelCaptureStrategies { get; set; }
        public DbSet<LevelPyramidQuestion> LevelPyramidQuestions { get; set; }
        public DbSet<ProcessLevelStatement> ProcessLevelStatements { get; set; }
        public DbSet<TacticalResourceAllocation> TacticalResourceAllocations { get; set; }
        public DbSet<ProcessStrategicAlignment> ProcessStrategicAlignments { get; set; }
        public DbSet<BusinessOutcome> BusinessOutcomes { get; set; }
        public DbSet<ProcessOutcomeMeasure> ProcessOutcomeMeasures { get; set; }
        public DbSet<ProcessStage> ProcessStages { get; set; }
        public DbSet<ProcessInterdependency> ProcessInterdependencies { get; set; }
        public DbSet<StakeholderLense> StakeholderLenses { get; set; }
        public DbSet<ProcedureLensView> ProcedureLensViews { get; set; }
        public DbSet<ApplicabilityScope> ApplicabilityScopes { get; set; }
        public DbSet<StepContextSensitivity> StepContextSensitivities { get; set; }
        public DbSet<SituationalVariant> SituationalVariants { get; set; }
        public DbSet<CollectedSourceMaterial> CollectedSourceMaterials { get; set; }
        public DbSet<SchemeRefinement> SchemeRefinements { get; set; }
        public DbSet<TermLabelVariant> TermLabelVariants { get; set; }
        public DbSet<AiLabelingRun> AiLabelingRuns { get; set; }
        public DbSet<SourceTermMention> SourceTermMentions { get; set; }
        public DbSet<TermRelation> TermRelations { get; set; }
        public DbSet<TermMeaningChange> TermMeaningChanges { get; set; }
        public DbSet<ExternalStandardTerm> ExternalStandardTerms { get; set; }
        public DbSet<RoleCapabilityTag> RoleCapabilityTags { get; set; }
        public DbSet<ClassificationFacet> ClassificationFacets { get; set; }
        public DbSet<ProcedureFacetAssignment> ProcedureFacetAssignments { get; set; }
        public DbSet<EncodingLifecycleStage> EncodingLifecycleStages { get; set; }
        public DbSet<KnowledgeConsumerSystem> KnowledgeConsumerSystems { get; set; }
        public DbSet<ConsumerSystemSync> ConsumerSystemSyncs { get; set; }
        public DbSet<IntegrationPathway> IntegrationPathways { get; set; }
        public DbSet<AgentIntegration> AgentIntegrations { get; set; }
        public DbSet<GroundingSnapshot> GroundingSnapshots { get; set; }
        public DbSet<ReasonerRun> ReasonerRuns { get; set; }
        public DbSet<SnapshotAssertion> SnapshotAssertions { get; set; }
        public DbSet<RetrievalSegment> RetrievalSegments { get; set; }
        public DbSet<KnowledgeQueryDefinition> KnowledgeQueryDefinitions { get; set; }
        public DbSet<KnowledgeQuerySource> KnowledgeQuerySources { get; set; }
        public DbSet<AssistantAnswer> AssistantAnswers { get; set; }
        public DbSet<AnswerGrounding> AnswerGroundings { get; set; }
        public DbSet<AnswerRequirementCheck> AnswerRequirementChecks { get; set; }
        public DbSet<AiToolInvocation> AiToolInvocations { get; set; }
        public DbSet<EmbeddingProbe> EmbeddingProbes { get; set; }
        public DbSet<PromptTemplate> PromptTemplates { get; set; }
        public DbSet<KnowledgeProjection> KnowledgeProjections { get; set; }
        public DbSet<ModelAnnotation> ModelAnnotations { get; set; }
        public DbSet<KnowledgeSearchEvent> KnowledgeSearchEvents { get; set; }
        public DbSet<AiAdoptionInitiatif> AiAdoptionInitiatives { get; set; }
        public DbSet<KnowledgeOutcomeMeasurement> KnowledgeOutcomeMeasurements { get; set; }
        public DbSet<AiInsightProposal> AiInsightProposals { get; set; }
        public DbSet<AssistantBenchmark> AssistantBenchmarks { get; set; }
        public DbSet<GovernedModel> GovernedModels { get; set; }
        public DbSet<ModelCharter> ModelCharters { get; set; }
        public DbSet<StewardActivity> StewardActivities { get; set; }
        public DbSet<ModelChangeRequest> ModelChangeRequests { get; set; }
        public DbSet<ChangeAuthorityRule> ChangeAuthorityRules { get; set; }
        public DbSet<ChangeImpactFinding> ChangeImpactFindings { get; set; }
        public DbSet<ChangeIntegrityCheck> ChangeIntegrityChecks { get; set; }
        public DbSet<ChangeObjection> ChangeObjections { get; set; }
        public DbSet<ChangeValidationRun> ChangeValidationRuns { get; set; }
        public DbSet<ExpectedInferenceCheck> ExpectedInferenceChecks { get; set; }
        public DbSet<ModelConsumer> ModelConsumers { get; set; }
        public DbSet<ConsumerRevalidation> ConsumerRevalidations { get; set; }
        public DbSet<ModelDocument> ModelDocuments { get; set; }
        public DbSet<StalenessQueryRun> StalenessQueryRuns { get; set; }
        public DbSet<ExternalDependencyRevision> ExternalDependencyRevisions { get; set; }
        public DbSet<StakeholderQuestion> StakeholderQuestions { get; set; }
        public DbSet<ModelExpansionRequest> ModelExpansionRequests { get; set; }
        public DbSet<ExpansionConceptFit> ExpansionConceptFits { get; set; }
        public DbSet<CompetencyQuestionSetEntry> CompetencyQuestionSetEntries { get; set; }
        public DbSet<CompetencyQuestionRun> CompetencyQuestionRuns { get; set; }
        public DbSet<CompetencyQuestionReview> CompetencyQuestionReviews { get; set; }
        public DbSet<QualityCriteria> QualityCriteria { get; set; }
        public DbSet<QualityAssessment> QualityAssessments { get; set; }
        public DbSet<TermDefinition> TermDefinitions { get; set; }
        public DbSet<ModelProposal> ModelProposals { get; set; }
        public DbSet<AssignmentInstantCheck> AssignmentInstantChecks { get; set; }
        public DbSet<InstanceDataVersion> InstanceDataVersions { get; set; }
        public DbSet<DomainCoverageArea> DomainCoverageAreas { get; set; }
        public DbSet<GovernanceStageControl> GovernanceStageControls { get; set; }
        public DbSet<ProcessDesignDecision> ProcessDesignDecisions { get; set; }
        public DbSet<ModelChangeLogEntry> ModelChangeLogEntries { get; set; }
        public DbSet<DriftObservation> DriftObservations { get; set; }
        public DbSet<SourcingFunction> SourcingFunctions { get; set; }
        public DbSet<KnowledgeAudit> KnowledgeAudits { get; set; }
        public DbSet<KnowledgeAuditItem> KnowledgeAuditItems { get; set; }
        public DbSet<KnowledgeCaptureInitiatif> KnowledgeCaptureInitiatives { get; set; }
        public DbSet<KnowledgeWorkforcePosition> KnowledgeWorkforcePositions { get; set; }
        public DbSet<ProviderEngagement> ProviderEngagements { get; set; }
        public DbSet<KnowledgeDeliverable> KnowledgeDeliverables { get; set; }
        public DbSet<CorporateGovernanceProgram> CorporateGovernancePrograms { get; set; }
        public DbSet<RecordsRetentionPolicy> RecordsRetentionPolicies { get; set; }
        public DbSet<AiRegistryModelVersion> AiRegistryModelVersions { get; set; }
        public DbSet<AiModelDeployment> AiModelDeployments { get; set; }
        public DbSet<AiModelEvaluation> AiModelEvaluations { get; set; }
        public DbSet<AiAgentAccountability> AiAgentAccountabilities { get; set; }
        public DbSet<AgentUpgradeAssessment> AgentUpgradeAssessments { get; set; }
        public DbSet<AssignmentUpdatePolicy> AssignmentUpdatePolicies { get; set; }
        public DbSet<RoleAssignmentUpdateTask> RoleAssignmentUpdateTasks { get; set; }
        public DbSet<AssignmentRoutedNotice> AssignmentRoutedNotices { get; set; }
        public DbSet<PractitionerExpertise> PractitionerExpertise { get; set; }
        public DbSet<CriticalIncident> CriticalIncidents { get; set; }
        public DbSet<InterviewProbe> InterviewProbes { get; set; }
        public DbSet<ObservedAction> ObservedActions { get; set; }
        public DbSet<ElicitationParticipant> ElicitationParticipants { get; set; }
        public DbSet<RepresentationReview> RepresentationReviews { get; set; }
        public DbSet<WorkflowViewDivergence> WorkflowViewDivergences { get; set; }
        public DbSet<ExpertCognition> ExpertCognitions { get; set; }
        public DbSet<ConceptLadderRung> ConceptLadderRungs { get; set; }
        public DbSet<RepertoryGridConstruct> RepertoryGridConstructs { get; set; }
        public DbSet<KnowledgeConversion> KnowledgeConversions { get; set; }
        public DbSet<KnowledgeHolding> KnowledgeHoldings { get; set; }
        public DbSet<FragmentCorroboration> FragmentCorroborations { get; set; }
        public DbSet<KnowledgeTestOutcome> KnowledgeTestOutcomes { get; set; }
        public DbSet<KnowHowCarrier> KnowHowCarriers { get; set; }
        public DbSet<KnowledgeTransfer> KnowledgeTransfers { get; set; }
        public DbSet<KnowledgeRepositoryEntry> KnowledgeRepositoryEntries { get; set; }
        public DbSet<CommunityMembership> CommunityMemberships { get; set; }
        public DbSet<SourceRelationship> SourceRelationships { get; set; }
        public DbSet<DepartmentProcessAccount> DepartmentProcessAccounts { get; set; }
        public DbSet<ProblemOccurrence> ProblemOccurrences { get; set; }
        public DbSet<OnboardingRecord> OnboardingRecords { get; set; }
        public DbSet<SharingRecognition> SharingRecognitions { get; set; }
        public DbSet<CapabilityDecline> CapabilityDeclines { get; set; }
        public DbSet<KnowledgeTrace> KnowledgeTraces { get; set; }
        public DbSet<MinedFlowEdge> MinedFlowEdges { get; set; }
        public DbSet<CollectionOccasion> CollectionOccasions { get; set; }
        public DbSet<StakeholderPerspectif> StakeholderPerspectives { get; set; }
        public DbSet<ModelPilot> ModelPilots { get; set; }
        public DbSet<ModelActivityExpert> ModelActivityExperts { get; set; }
        public DbSet<ModelDataMappingRun> ModelDataMappingRuns { get; set; }
        public DbSet<ArtifactHandoff> ArtifactHandoffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RulebookRelease>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.RulebookReleases)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RulebookRelease>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.RulebookReleases)
                .HasForeignKey(f => f.PreviousRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RulebookRelease>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.VersionDecidedByAgentRulebookReleases)
                .HasForeignKey(f => f.VersionDecidedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RulebookRelease>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ApprovedByAgentRulebookReleases)
                .HasForeignKey(f => f.ApprovedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OntologyProfile>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.OntologyProfiles)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OntologyProfile>()
                .HasOne(e => e.OntologyProfile)
                .WithMany(f => f.OntologyProfiles)
                .HasForeignKey(f => f.PrerequisiteProfile)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Agent>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.OrganizationAgents)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Agent>()
                .HasOne(e => e.OrganizationRefRef)
                .WithMany(f => f.RepresentsOrganizationAgents)
                .HasForeignKey(f => f.RepresentsOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.Roles)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.Roles)
                .HasForeignKey(f => f.CurrentAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.Role)
                .WithMany(f => f.SpecializesRoleRoles)
                .HasForeignKey(f => f.SpecializesRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.EscalationBackupRoleRoles)
                .HasForeignKey(f => f.EscalationBackupRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.RoleRoleAssignments)
                .HasForeignKey(f => f.Role)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.RoleAssignments)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.RoleAssignments)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.RoleAssignments)
                .HasForeignKey(f => f.SupersedesAssignment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.RoleRefRef)
                .WithMany(f => f.ApprovingAuthorityRoleRoleAssignments)
                .HasForeignKey(f => f.ApprovingAuthorityRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.ChangeRequest)
                .WithMany(f => f.RoleAssignments)
                .HasForeignKey(f => f.AuthorizingChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.RoleAssignments)
                .HasForeignKey(f => f.ForProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunitiesOfPractice>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.CommunitiesOfPractice)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunitiesOfPractice>()
                .HasOne(e => e.Role)
                .WithMany(f => f.CommunitiesOfPractice)
                .HasForeignKey(f => f.StewardRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunitiesOfPractice>()
                .HasOne(e => e.Vocabulary)
                .WithMany(f => f.CommunitiesOfPractice)
                .HasForeignKey(f => f.OwnVocabulary)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Mentorship>()
                .HasOne(e => e.CommunitiesOfPractice)
                .WithMany(f => f.Mentorships)
                .HasForeignKey(f => f.CommunityOfPractice)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Mentorship>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.MentorAgentMentorships)
                .HasForeignKey(f => f.MentorAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Mentorship>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.LearnerAgentMentorships)
                .HasForeignKey(f => f.LearnerAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Mentorship>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.Mentorships)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureType>()
                .HasOne(e => e.ProcedureType)
                .WithMany(f => f.ProcedureTypes)
                .HasForeignKey(f => f.BroaderProcedureType)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureType>()
                .HasOne(e => e.ClassificationFacet)
                .WithMany(f => f.ProcedureTypes)
                .HasForeignKey(f => f.DistinguishingFacet)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Procedure>()
                .HasOne(e => e.ProcedureTypeRef)
                .WithMany(f => f.Procedures)
                .HasForeignKey(f => f.ProcedureType)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Procedure>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.OwnerOrganizationProcedures)
                .HasForeignKey(f => f.OwnerOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Procedure>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.AdoptedByOrganizationProcedures)
                .HasForeignKey(f => f.AdoptedByOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Procedure>()
                .HasOne(e => e.Procedure)
                .WithMany(f => f.Procedures)
                .HasForeignKey(f => f.TemplateProcedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Procedure>()
                .HasOne(e => e.RegulatoryFramework)
                .WithMany(f => f.Procedures)
                .HasForeignKey(f => f.RequiredByRegulation)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersion>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureVersions)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersion>()
                .HasOne(e => e.LifecycleStatuse)
                .WithMany(f => f.ProcedureVersions)
                .HasForeignKey(f => f.Status)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersion>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.CreatedByAgentProcedureVersions)
                .HasForeignKey(f => f.CreatedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersion>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ModifiedByAgentProcedureVersions)
                .HasForeignKey(f => f.ModifiedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersion>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ProcedureVersions)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersionLink>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.PreviousProcedureVersionProcedureVersionLinks)
                .HasForeignKey(f => f.PreviousProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureVersionLink>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.NextProcedureVersionProcedureVersionLinks)
                .HasForeignKey(f => f.NextProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureStatusChange>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ProcedureStatusChanges)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureStatusChange>()
                .HasOne(e => e.LifecycleStatuse)
                .WithMany(f => f.FromStatusProcedureStatusChanges)
                .HasForeignKey(f => f.FromStatus)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureStatusChange>()
                .HasOne(e => e.LifecycleStatuseRef)
                .WithMany(f => f.ToStatusProcedureStatusChanges)
                .HasForeignKey(f => f.ToStatus)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureStatusChange>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProcedureStatusChanges)
                .HasForeignKey(f => f.ChangedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureStatusChange>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.ProcedureStatusChanges)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.Steps)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.Role)
                .WithMany(f => f.Steps)
                .HasForeignKey(f => f.AssignedRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.Step)
                .WithMany(f => f.ParentStepSteps)
                .HasForeignKey(f => f.ParentStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.FirstChildStepSteps)
                .HasForeignKey(f => f.FirstChildStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.StepRefRef)
                .WithMany(f => f.VerifiesStepSteps)
                .HasForeignKey(f => f.VerifiesStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.Error)
                .WithMany(f => f.Steps)
                .HasForeignKey(f => f.RemedyForError)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.Procedure)
                .WithMany(f => f.Steps)
                .HasForeignKey(f => f.CallsProcedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.EnergySource)
                .WithMany(f => f.Steps)
                .HasForeignKey(f => f.IsolatesEnergySource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.StepRefRefRef)
                .WithMany(f => f.PrerequisiteStepSteps)
                .HasForeignKey(f => f.PrerequisiteStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Step>()
                .HasOne(e => e.ProcessStage)
                .WithMany(f => f.Steps)
                .HasForeignKey(f => f.Stage)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepTransition>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.StepTransitions)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepTransition>()
                .HasOne(e => e.Step)
                .WithMany(f => f.FromStepStepTransitions)
                .HasForeignKey(f => f.FromStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepTransition>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ToStepStepTransitions)
                .HasForeignKey(f => f.ToStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepAction>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepActions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepAction>()
                .HasOne(e => e.ActionRef)
                .WithMany(f => f.StepActions)
                .HasForeignKey(f => f.Action)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepFunction>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepFunctions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepFunction>()
                .HasOne(e => e.FunctionRef)
                .WithMany(f => f.StepFunctions)
                .HasForeignKey(f => f.Function)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepTool>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepTools)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepTool>()
                .HasOne(e => e.ToolRef)
                .WithMany(f => f.StepTools)
                .HasForeignKey(f => f.Tool)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Requirement>()
                .HasOne(e => e.Role)
                .WithMany(f => f.Requirements)
                .HasForeignKey(f => f.AccountableRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Requirement>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.Requirements)
                .HasForeignKey(f => f.ControlledTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Requirement>()
                .HasOne(e => e.RegulatoryFrameworkRef)
                .WithMany(f => f.Requirements)
                .HasForeignKey(f => f.RegulatoryFramework)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepRequirement>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepRequirements)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepRequirement>()
                .HasOne(e => e.RequirementRef)
                .WithMany(f => f.StepRequirements)
                .HasForeignKey(f => f.Requirement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepVerification>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepVerifications)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rationale>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.Rationales)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rationale>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.Rationales)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rationale>()
                .HasOne(e => e.Role)
                .WithMany(f => f.Rationales)
                .HasForeignKey(f => f.AuthorityRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Exception>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.Exceptions)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Exception>()
                .HasOne(e => e.Step)
                .WithMany(f => f.Exceptions)
                .HasForeignKey(f => f.TriggerStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Exception>()
                .HasOne(e => e.Role)
                .WithMany(f => f.ApprovalRoleExceptions)
                .HasForeignKey(f => f.ApprovalRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Exception>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.FallbackRoleExceptions)
                .HasForeignKey(f => f.FallbackRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Resource>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.CreatedByAgentResources)
                .HasForeignKey(f => f.CreatedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Resource>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ModifiedByAgentResources)
                .HasForeignKey(f => f.ModifiedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Resource>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.Resources)
                .HasForeignKey(f => f.ExtractedFromResource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Resource>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.Resources)
                .HasForeignKey(f => f.ArtifactTypeConcept)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Resource>()
                .HasOne(e => e.Procedure)
                .WithMany(f => f.CatalogEntryForResources)
                .HasForeignKey(f => f.CatalogEntryFor)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Resource>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ComplianceRecordForResources)
                .HasForeignKey(f => f.ComplianceRecordFor)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureResource>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ProcedureResources)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureResource>()
                .HasOne(e => e.ResourceRef)
                .WithMany(f => f.ProcedureResources)
                .HasForeignKey(f => f.Resource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationSession>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ElicitationSessions)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationSession>()
                .HasOne(e => e.KnowledgeMethod)
                .WithMany(f => f.ElicitationSessions)
                .HasForeignKey(f => f.Method)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationSession>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.PractitionerAgentElicitationSessions)
                .HasForeignKey(f => f.PractitionerAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationSession>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.FacilitatorAgentElicitationSessions)
                .HasForeignKey(f => f.FacilitatorAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationSession>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ElicitationSessions)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeFragment>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.KnowledgeFragments)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeFragment>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.KnowledgeFragments)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeFragment>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.KnowledgeFragments)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeFragment>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeFragments)
                .HasForeignKey(f => f.SourceAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeFragment>()
                .HasOne(e => e.Role)
                .WithMany(f => f.KnowledgeFragments)
                .HasForeignKey(f => f.OwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeFragment>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.KnowledgeFragments)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeGap>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.KnowledgeGaps)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeGap>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.KnowledgeGaps)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeGap>()
                .HasOne(e => e.Role)
                .WithMany(f => f.KnowledgeGaps)
                .HasForeignKey(f => f.OwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeGap>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.KnowledgeGaps)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeGap>()
                .HasOne(e => e.ElicitationSession)
                .WithMany(f => f.KnowledgeGaps)
                .HasForeignKey(f => f.DrawnOutBySession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeGap>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.KnowledgeGaps)
                .HasForeignKey(f => f.CodifiedAsFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FAQ>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.FAQs)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FAQ>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.FAQs)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FAQ>()
                .HasOne(e => e.FaqCategory)
                .WithMany(f => f.FAQs)
                .HasForeignKey(f => f.Category)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FAQ>()
                .HasOne(e => e.FaqTarget)
                .WithMany(f => f.FAQs)
                .HasForeignKey(f => f.TargetKind)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FAQ>()
                .HasOne(e => e.ResourceRef)
                .WithMany(f => f.FAQs)
                .HasForeignKey(f => f.Resource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Explanation>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.Explanations)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Explanation>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.Explanations)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureExecution>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ProcedureExecutions)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureExecution>()
                .HasOne(e => e.LifecycleStatuse)
                .WithMany(f => f.ProcedureExecutions)
                .HasForeignKey(f => f.ExecutionStatus)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureExecution>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ExecutedByAgentProcedureExecutions)
                .HasForeignKey(f => f.ExecutedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureExecution>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ConfirmedByAgentProcedureExecutions)
                .HasForeignKey(f => f.ConfirmedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureExecution>()
                .HasOne(e => e.FacilityRef)
                .WithMany(f => f.ProcedureExecutions)
                .HasForeignKey(f => f.Facility)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureExecution>()
                .HasOne(e => e.Machine)
                .WithMany(f => f.ProcedureExecutions)
                .HasForeignKey(f => f.ExecutedOnMachine)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepExecution>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.StepExecutions)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepExecution>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepExecutions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepExecution>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ExecutedByAgentStepExecutions)
                .HasForeignKey(f => f.ExecutedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepExecution>()
                .HasOne(e => e.StepExecution)
                .WithMany(f => f.StepExecutions)
                .HasForeignKey(f => f.PreviousStepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepExecution>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ConfirmedByAgentStepExecutions)
                .HasForeignKey(f => f.ConfirmedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequirementSatisfaction>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.RequirementSatisfactions)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequirementSatisfaction>()
                .HasOne(e => e.RequirementRef)
                .WithMany(f => f.RequirementSatisfactions)
                .HasForeignKey(f => f.Requirement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequirementSatisfaction>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RequirementSatisfactions)
                .HasForeignKey(f => f.EvaluatedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<IssueOccurrence>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.IssueOccurrences)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<IssueOccurrence>()
                .HasOne(e => e.ErrorRef)
                .WithMany(f => f.IssueOccurrences)
                .HasForeignKey(f => f.Error)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<IssueOccurrence>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.IssueOccurrences)
                .HasForeignKey(f => f.EncounteredByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<IssueOccurrence>()
                .HasOne(e => e.ChangeRequest)
                .WithMany(f => f.IssueOccurrences)
                .HasForeignKey(f => f.RedesignChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserQuestion>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.UserQuestions)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserQuestion>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.UserQuestions)
                .HasForeignKey(f => f.AskedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserQuestion>()
                .HasOne(e => e.FAQ)
                .WithMany(f => f.UserQuestions)
                .HasForeignKey(f => f.ResolvedByFaq)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserQuestion>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.UserQuestions)
                .HasForeignKey(f => f.AddressedByResource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserFeedback>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.UserFeedback)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserFeedback>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.UserFeedback)
                .HasForeignKey(f => f.ProvidedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardshipAssignment>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.StewardshipAssignments)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardshipAssignment>()
                .HasOne(e => e.Role)
                .WithMany(f => f.StewardRoleStewardshipAssignments)
                .HasForeignKey(f => f.StewardRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardshipAssignment>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.AuthorityRoleStewardshipAssignments)
                .HasForeignKey(f => f.AuthorityRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardshipAssignment>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.StewardshipAssignments)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeRequest>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ChangeRequests)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeRequest>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ChangeRequests)
                .HasForeignKey(f => f.RequestedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeRequest>()
                .HasOne(e => e.Role)
                .WithMany(f => f.ChangeRequests)
                .HasForeignKey(f => f.AuthorityRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeRequest>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ChangeRequests)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReviewEvent>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ReviewEvents)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReviewEvent>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ReviewEvents)
                .HasForeignKey(f => f.ReviewedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReviewEvent>()
                .HasOne(e => e.ChangeRequest)
                .WithMany(f => f.ReviewEvents)
                .HasForeignKey(f => f.RelatedChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReviewEvent>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ReviewEvents)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LearningActivity>()
                .HasOne(e => e.CommunitiesOfPractice)
                .WithMany(f => f.LearningActivities)
                .HasForeignKey(f => f.CommunityOfPractice)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LearningActivity>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.LearningActivities)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LearningActivity>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.LearningActivities)
                .HasForeignKey(f => f.FacilitatorAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LearningActivity>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.LearningActivities)
                .HasForeignKey(f => f.EvidenceResource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OperationalBinding>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.OperationalBindings)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OperationalBinding>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.OperationalBindings)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OperationalBinding>()
                .HasOne(e => e.ResourceRef)
                .WithMany(f => f.OperationalBindings)
                .HasForeignKey(f => f.Resource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OperationalBinding>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.OperationalBindings)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunicationPolicy>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.CommunicationPolicies)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunicationPolicy>()
                .HasOne(e => e.Role)
                .WithMany(f => f.CommunicationPolicies)
                .HasForeignKey(f => f.ApprovalRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageTemplate>()
                .HasOne(e => e.CommunicationPolicyRef)
                .WithMany(f => f.MessageTemplates)
                .HasForeignKey(f => f.CommunicationPolicy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageTemplate>()
                .HasOne(e => e.ResourceRef)
                .WithMany(f => f.MessageTemplates)
                .HasForeignKey(f => f.Resource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SemanticMapping>()
                .HasOne(e => e.OntologyProfileRef)
                .WithMany(f => f.SemanticMappings)
                .HasForeignKey(f => f.OntologyProfile)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleQuestion>()
                .HasOne(e => e.Role)
                .WithMany(f => f.RoleQuestions)
                .HasForeignKey(f => f.AskingRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleQuestion>()
                .HasOne(e => e.WitnessLoopRef)
                .WithMany(f => f.RoleQuestions)
                .HasForeignKey(f => f.WitnessLoop)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RulebookField>()
                .HasOne(e => e.RoleQuestion)
                .WithMany(f => f.RulebookFields)
                .HasForeignKey(f => f.InventedForQuestion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TestCase>()
                .HasOne(e => e.RoleQuestion)
                .WithMany(f => f.TestCases)
                .HasForeignKey(f => f.DefendsQuestion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TestCase>()
                .HasOne(e => e.TestSuite)
                .WithMany(f => f.TestCases)
                .HasForeignKey(f => f.Suite)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExceptionInvocation>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.ExceptionInvocations)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExceptionInvocation>()
                .HasOne(e => e.ExceptionRef)
                .WithMany(f => f.ExceptionInvocations)
                .HasForeignKey(f => f.Exception)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExceptionInvocation>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.InvokedByAgentExceptionInvocations)
                .HasForeignKey(f => f.InvokedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExceptionInvocation>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ApprovedByAgentExceptionInvocations)
                .HasForeignKey(f => f.ApprovedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VerificationOutcome>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.VerificationOutcomes)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VerificationOutcome>()
                .HasOne(e => e.StepVerificationRef)
                .WithMany(f => f.VerificationOutcomes)
                .HasForeignKey(f => f.StepVerification)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VerificationOutcome>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.VerificationOutcomes)
                .HasForeignKey(f => f.ObservedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedTransition>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.ObservedTransitions)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedTransition>()
                .HasOne(e => e.StepTransitionRef)
                .WithMany(f => f.ObservedTransitions)
                .HasForeignKey(f => f.StepTransition)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedTransition>()
                .HasOne(e => e.StepExecution)
                .WithMany(f => f.ObservedTransitions)
                .HasForeignKey(f => f.ArrivingStepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Recipient>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.Recipients)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Recipient>()
                .HasOne(e => e.OperationalBinding)
                .WithMany(f => f.Recipients)
                .HasForeignKey(f => f.ConsentBinding)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.RecipientRef)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.Recipient)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.MessageTemplateRef)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.MessageTemplate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.SentByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.Exception)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.InvokedException)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MessageDelivery>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.MessageDeliveries)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TemplateApproval>()
                .HasOne(e => e.MessageTemplateRef)
                .WithMany(f => f.TemplateApprovals)
                .HasForeignKey(f => f.MessageTemplate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TemplateApproval>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.TemplateApprovals)
                .HasForeignKey(f => f.DecidedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TemplateApproval>()
                .HasOne(e => e.Role)
                .WithMany(f => f.TemplateApprovals)
                .HasForeignKey(f => f.DecidedInRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.RecipientRef)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.Recipient)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.MessageTemplateRef)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.MessageTemplate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.MessageDelivery)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.ResultingDelivery)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.Role)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.RefusalNotifiedRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SendIntent>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.SendIntents)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentDecisionRecord>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.AgentDecisionRecords)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentDecisionRecord>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.DecidingAgentAgentDecisionRecords)
                .HasForeignKey(f => f.DecidingAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentDecisionRecord>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ReviewedByAgentAgentDecisionRecords)
                .HasForeignKey(f => f.ReviewedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentDecisionRecord>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.AgentDecisionRecords)
                .HasForeignKey(f => f.UnderRoleAssignment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DeliveredCommunication>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.DeliveredCommunications)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DeliveredCommunication>()
                .HasOne(e => e.StepExecution)
                .WithMany(f => f.SendingStepExecutionDeliveredCommunications)
                .HasForeignKey(f => f.SendingStepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DeliveredCommunication>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.AuthorizingStepExecutionDeliveredCommunications)
                .HasForeignKey(f => f.AuthorizingStepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DeliveredCommunication>()
                .HasOne(e => e.MessageTemplateRef)
                .WithMany(f => f.DeliveredCommunications)
                .HasForeignKey(f => f.MessageTemplate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthorityBoundary>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.AuthorityBoundaries)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthorityBoundary>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.AuthorityBoundaries)
                .HasForeignKey(f => f.RatifiedByKnowledgeFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthorityBoundary>()
                .HasOne(e => e.Requirement)
                .WithMany(f => f.AuthorityBoundaries)
                .HasForeignKey(f => f.EnforcingRequirement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthorityBoundary>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AuthorityBoundaries)
                .HasForeignKey(f => f.AuthorityRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthorityBoundary>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.AuthorityBoundaries)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BindingObservation>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.BindingObservations)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BindingObservation>()
                .HasOne(e => e.OperationalBindingRef)
                .WithMany(f => f.BindingObservations)
                .HasForeignKey(f => f.OperationalBinding)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Attestation>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.Attestations)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Attestation>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.Attestations)
                .HasForeignKey(f => f.SignedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRoleProfile>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.AppRoleProfiles)
                .HasForeignKey(f => f.Role)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRoute>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AppRoutes)
                .HasForeignKey(f => f.OwningRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRoute>()
                .HasOne(e => e.AppNavGroup)
                .WithMany(f => f.AppRoutes)
                .HasForeignKey(f => f.NavGroup)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRouteQuestion>()
                .HasOne(e => e.AppRoute)
                .WithMany(f => f.AppRouteQuestions)
                .HasForeignKey(f => f.Route)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRouteQuestion>()
                .HasOne(e => e.RoleQuestion)
                .WithMany(f => f.AppRouteQuestions)
                .HasForeignKey(f => f.Question)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRouteReference>()
                .HasOne(e => e.AppRoute)
                .WithMany(f => f.FromRouteAppRouteReferences)
                .HasForeignKey(f => f.FromRoute)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRouteReference>()
                .HasOne(e => e.AppRouteRef)
                .WithMany(f => f.ToRouteAppRouteReferences)
                .HasForeignKey(f => f.ToRoute)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccessPrincipal>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AccessPrincipals)
                .HasForeignKey(f => f.DomainRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccessPolicy>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.AccessPolicies)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccessPolicy>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.AccessPolicies)
                .HasForeignKey(f => f.TargetTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FieldGrant>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.FieldGrants)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FieldGrant>()
                .HasOne(e => e.RulebookField)
                .WithMany(f => f.FieldGrants)
                .HasForeignKey(f => f.TargetField)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleSchema>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.RoleSchemas)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleSchemaView>()
                .HasOne(e => e.RoleSchemaRef)
                .WithMany(f => f.RoleSchemaViews)
                .HasForeignKey(f => f.RoleSchema)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleSchemaView>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.RoleSchemaViews)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleSchemaView>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.RoleSchemaViews)
                .HasForeignKey(f => f.TargetTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccessDenialTest>()
                .HasOne(e => e.AccessPolicy)
                .WithMany(f => f.AccessDenialTests)
                .HasForeignKey(f => f.TargetPolicy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccessDenialTest>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.AccessDenialTests)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccessDenialTest>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.AccessDenialTests)
                .HasForeignKey(f => f.TargetTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppUser>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AppUsers)
                .HasForeignKey(f => f.LinkedAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PrincipalAssignment>()
                .HasOne(e => e.AppUserRef)
                .WithMany(f => f.PrincipalAssignments)
                .HasForeignKey(f => f.AppUser)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PrincipalAssignment>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.PrincipalAssignments)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<IssuedToken>()
                .HasOne(e => e.AppUserRef)
                .WithMany(f => f.IssuedTokens)
                .HasForeignKey(f => f.AppUser)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<IssuedToken>()
                .HasOne(e => e.AccessPrincipal)
                .WithMany(f => f.IssuedTokens)
                .HasForeignKey(f => f.Principal)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessMiningRun>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ProcessMiningRuns)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessMiningRun>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ProcessMiningRuns)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Vocabulary>()
                .HasOne(e => e.Role)
                .WithMany(f => f.Vocabularies)
                .HasForeignKey(f => f.GoverningRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Vocabulary>()
                .HasOne(e => e.Procedure)
                .WithMany(f => f.Vocabularies)
                .HasForeignKey(f => f.GovernsProcedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VocabularyTerm>()
                .HasOne(e => e.VocabularyRef)
                .WithMany(f => f.VocabularyTerms)
                .HasForeignKey(f => f.Vocabulary)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VocabularyTerm>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.VocabularyTerms)
                .HasForeignKey(f => f.BroaderTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VocabularyTerm>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.VocabularyTerms)
                .HasForeignKey(f => f.IntroducedInRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VocabularyTerm>()
                .HasOne(e => e.Role)
                .WithMany(f => f.VocabularyTerms)
                .HasForeignKey(f => f.RepresentsRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.SeekerKnowledgeBrokerLinks)
                .HasForeignKey(f => f.Seeker)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.BrokerKnowledgeBrokerLinks)
                .HasForeignKey(f => f.Broker)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.KnowledgeBrokerLinks)
                .HasForeignKey(f => f.Topic)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.KnowledgeBrokerLinks)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.KnowHowCarrier)
                .WithMany(f => f.KnowledgeBrokerLinks)
                .HasForeignKey(f => f.PointsToKnowHow)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.Vocabulary)
                .WithMany(f => f.SeekerVocabularyKnowledgeBrokerLinks)
                .HasForeignKey(f => f.SeekerVocabulary)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeBrokerLink>()
                .HasOne(e => e.VocabularyRef)
                .WithMany(f => f.HolderVocabularyKnowledgeBrokerLinks)
                .HasForeignKey(f => f.HolderVocabulary)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConformanceRun>()
                .HasOne(e => e.ConformanceSubstrate)
                .WithMany(f => f.ConformanceRuns)
                .HasForeignKey(f => f.AnswerKeyAuthor)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SubstrateRunScore>()
                .HasOne(e => e.ConformanceRun)
                .WithMany(f => f.SubstrateRunScores)
                .HasForeignKey(f => f.Run)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SubstrateRunScore>()
                .HasOne(e => e.ConformanceSubstrate)
                .WithMany(f => f.SubstrateRunScores)
                .HasForeignKey(f => f.Substrate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableConformance>()
                .HasOne(e => e.ConformanceRun)
                .WithMany(f => f.TableConformance)
                .HasForeignKey(f => f.Run)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableConformance>()
                .HasOne(e => e.ConformanceSubstrate)
                .WithMany(f => f.TableConformance)
                .HasForeignKey(f => f.Substrate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableConformance>()
                .HasOne(e => e.RulebookTableRef)
                .WithMany(f => f.TableConformance)
                .HasForeignKey(f => f.RulebookTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FieldDisagreement>()
                .HasOne(e => e.ConformanceSubstrate)
                .WithMany(f => f.FieldDisagreements)
                .HasForeignKey(f => f.Substrate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FieldDisagreement>()
                .HasOne(e => e.RulebookFieldRef)
                .WithMany(f => f.FieldDisagreements)
                .HasForeignKey(f => f.RulebookField)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FieldDisagreement>()
                .HasOne(e => e.TableConformanceRef)
                .WithMany(f => f.FieldDisagreements)
                .HasForeignKey(f => f.TableConformance)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CellDisagreement>()
                .HasOne(e => e.FieldDisagreementRef)
                .WithMany(f => f.CellDisagreements)
                .HasForeignKey(f => f.FieldDisagreement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArticleClaim>()
                .HasOne(e => e.SourceArticleRef)
                .WithMany(f => f.ArticleClaims)
                .HasForeignKey(f => f.SourceArticle)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.ArticleClaimRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.ArticleClaim)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.RulebookFieldRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.RulebookField)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.RulebookTableRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.RulebookTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.RoleQuestionRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.RoleQuestion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.OntologyProfileRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.OntologyProfile)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.KnowledgeMethodRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.KnowledgeMethod)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClaimEvidence>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ClaimEvidence)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MethodApplication>()
                .HasOne(e => e.KnowledgeMethodRef)
                .WithMany(f => f.MethodApplications)
                .HasForeignKey(f => f.KnowledgeMethod)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MethodApplication>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AppliedByAgentMethodApplications)
                .HasForeignKey(f => f.AppliedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MethodApplication>()
                .HasOne(e => e.GroundingSnapshot)
                .WithMany(f => f.MethodApplications)
                .HasForeignKey(f => f.AppliedToGroundingSnapshot)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MethodApplication>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.IdentifiedBrokerMethodApplications)
                .HasForeignKey(f => f.IdentifiedBroker)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LifecycleStatuse>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.LifecycleStatuses)
                .HasForeignKey(f => f.WorkflowStatusConcept)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Facility>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.Facilities)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Facility>()
                .HasOne(e => e.Facility)
                .WithMany(f => f.Facilities)
                .HasForeignKey(f => f.ParentFacility)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Machine>()
                .HasOne(e => e.MachineTypeRef)
                .WithMany(f => f.Machines)
                .HasForeignKey(f => f.MachineType)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Machine>()
                .HasOne(e => e.FacilityRef)
                .WithMany(f => f.Machines)
                .HasForeignKey(f => f.Facility)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Machine>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.Machines)
                .HasForeignKey(f => f.ManufacturedBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Machine>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.Machines)
                .HasForeignKey(f => f.GoverningProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MachineEnergySource>()
                .HasOne(e => e.MachineRef)
                .WithMany(f => f.MachineEnergySources)
                .HasForeignKey(f => f.Machine)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MachineEnergySource>()
                .HasOne(e => e.EnergySourceRef)
                .WithMany(f => f.MachineEnergySources)
                .HasForeignKey(f => f.EnergySource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepLockRequirement>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepLockRequirements)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepLockRequirement>()
                .HasOne(e => e.LockDeviceRef)
                .WithMany(f => f.StepLockRequirements)
                .HasForeignKey(f => f.LockDevice)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepProtectiveEquipment>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepProtectiveEquipment)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepProtectiveEquipment>()
                .HasOne(e => e.ProtectiveEquipmentRef)
                .WithMany(f => f.StepProtectiveEquipment)
                .HasForeignKey(f => f.ProtectiveEquipment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureTarget>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureTargets)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureTarget>()
                .HasOne(e => e.MachineRef)
                .WithMany(f => f.ProcedureTargets)
                .HasForeignKey(f => f.Machine)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureAdoption>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureAdoptions)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureAdoption>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.ProcedureAdoptions)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureOutcomeCriteria>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureOutcomeCriteria)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ActivityRelation>()
                .HasOne(e => e.Step)
                .WithMany(f => f.FromStepActivityRelations)
                .HasForeignKey(f => f.FromStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ActivityRelation>()
                .HasOne(e => e.RelationTypeRef)
                .WithMany(f => f.ActivityRelations)
                .HasForeignKey(f => f.RelationType)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ActivityRelation>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ToStepActivityRelations)
                .HasForeignKey(f => f.ToStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepVariable>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepVariables)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepVariable>()
                .HasOne(e => e.StepVariable)
                .WithMany(f => f.StepVariables)
                .HasForeignKey(f => f.SourceVariable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExecutionEntity>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.ExecutionEntities)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExecutionEntity>()
                .HasOne(e => e.StepVariableRef)
                .WithMany(f => f.ExecutionEntities)
                .HasForeignKey(f => f.StepVariable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepCondition>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepConditions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConditionCheck>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.ConditionChecks)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConditionCheck>()
                .HasOne(e => e.StepConditionRef)
                .WithMany(f => f.ConditionChecks)
                .HasForeignKey(f => f.StepCondition)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConditionCheck>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ConditionChecks)
                .HasForeignKey(f => f.CheckedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FailureMode>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.FailureModes)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FailureMode>()
                .HasOne(e => e.ProcedureTargetRef)
                .WithMany(f => f.FailureModes)
                .HasForeignKey(f => f.ProcedureTarget)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FailureMode>()
                .HasOne(e => e.Role)
                .WithMany(f => f.FailureModes)
                .HasForeignKey(f => f.EscalateToRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepCue>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepCues)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepCue>()
                .HasOne(e => e.Role)
                .WithMany(f => f.StepCues)
                .HasForeignKey(f => f.EscalateToRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CueObservation>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.CueObservations)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CueObservation>()
                .HasOne(e => e.StepCueRef)
                .WithMany(f => f.CueObservations)
                .HasForeignKey(f => f.StepCue)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CueObservation>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ObservedByAgentCueObservations)
                .HasForeignKey(f => f.ObservedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CueObservation>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.EscalatedToAgentCueObservations)
                .HasForeignKey(f => f.EscalatedToAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DecisionPoint>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.DecisionPoints)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DecisionPoint>()
                .HasOne(e => e.StepTransition)
                .WithMany(f => f.DecisionPoints)
                .HasForeignKey(f => f.GoverningTransition)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExecutionParticipant>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.ExecutionParticipants)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExecutionParticipant>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ExecutionParticipants)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepResource>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepResources)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepResource>()
                .HasOne(e => e.ResourceRef)
                .WithMany(f => f.StepResources)
                .HasForeignKey(f => f.Resource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthoringSubmission>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.AuthoringSubmissions)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthoringSubmission>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AuthoringSubmissions)
                .HasForeignKey(f => f.SubmittedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AuthoringSubmission>()
                .HasOne(e => e.Tool)
                .WithMany(f => f.AuthoringSubmissions)
                .HasForeignKey(f => f.AuthoringTool)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LevelCaptureStrategy>()
                .HasOne(e => e.ProcessKnowledgeLevel)
                .WithMany(f => f.LevelCaptureStrategies)
                .HasForeignKey(f => f.Level)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LevelCaptureStrategy>()
                .HasOne(e => e.KnowledgeMethodRef)
                .WithMany(f => f.LevelCaptureStrategies)
                .HasForeignKey(f => f.KnowledgeMethod)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LevelPyramidQuestion>()
                .HasOne(e => e.ProcessKnowledgeLevel)
                .WithMany(f => f.LevelPyramidQuestions)
                .HasForeignKey(f => f.Level)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessLevelStatement>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcessLevelStatements)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessLevelStatement>()
                .HasOne(e => e.ProcessKnowledgeLevel)
                .WithMany(f => f.ProcessLevelStatements)
                .HasForeignKey(f => f.Level)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TacticalResourceAllocation>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.TacticalResourceAllocations)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TacticalResourceAllocation>()
                .HasOne(e => e.FacilityRef)
                .WithMany(f => f.TacticalResourceAllocations)
                .HasForeignKey(f => f.Facility)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessStrategicAlignment>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcessStrategicAlignments)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessStrategicAlignment>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.ProcessStrategicAlignments)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BusinessOutcome>()
                .HasOne(e => e.Role)
                .WithMany(f => f.BusinessOutcomes)
                .HasForeignKey(f => f.OwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessOutcomeMeasure>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcessOutcomeMeasures)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessOutcomeMeasure>()
                .HasOne(e => e.BusinessOutcomeRef)
                .WithMany(f => f.ProcessOutcomeMeasures)
                .HasForeignKey(f => f.BusinessOutcome)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessStage>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ProcessStages)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessStage>()
                .HasOne(e => e.Role)
                .WithMany(f => f.ProcessStages)
                .HasForeignKey(f => f.OwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessInterdependency>()
                .HasOne(e => e.Procedure)
                .WithMany(f => f.FromProcedureProcessInterdependencies)
                .HasForeignKey(f => f.FromProcedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessInterdependency>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ToProcedureProcessInterdependencies)
                .HasForeignKey(f => f.ToProcedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderLense>()
                .HasOne(e => e.Role)
                .WithMany(f => f.StakeholderLenses)
                .HasForeignKey(f => f.ExemplarRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureLensView>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureLensViews)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureLensView>()
                .HasOne(e => e.StakeholderLense)
                .WithMany(f => f.ProcedureLensViews)
                .HasForeignKey(f => f.StakeholderLens)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureLensView>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.ProcedureLensViews)
                .HasForeignKey(f => f.ProjectsVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ApplicabilityScope>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.ApplicabilityScopes)
                .HasForeignKey(f => f.BusinessUnit)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ApplicabilityScope>()
                .HasOne(e => e.RegulatoryFramework)
                .WithMany(f => f.ApplicabilityScopes)
                .HasForeignKey(f => f.RegulatoryRegime)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepContextSensitivity>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StepContextSensitivities)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepContextSensitivity>()
                .HasOne(e => e.ApplicabilityScopeRef)
                .WithMany(f => f.StepContextSensitivities)
                .HasForeignKey(f => f.ApplicabilityScope)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SituationalVariant>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.SituationalVariants)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SituationalVariant>()
                .HasOne(e => e.ApplicabilityScopeRef)
                .WithMany(f => f.SituationalVariants)
                .HasForeignKey(f => f.ApplicabilityScope)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SituationalVariant>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.SituationalVariants)
                .HasForeignKey(f => f.ExpertAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.Vocabulary)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.OrganizedIntoScheme)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.EncodedIntoVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.SourceDocument)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.ProcessMiningRun)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.ComplementsMiningRun)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.ProcedureExecution)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.CapturedDuringExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.CollectionOccasion)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.CollectedAtOccasion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.UserFeedback)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.PromptedByFeedback)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.MethodApplication)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.ProducedByMethodApplication)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectedSourceMaterial>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.CollectedSourceMaterials)
                .HasForeignKey(f => f.ContributingExpert)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SchemeRefinement>()
                .HasOne(e => e.VocabularyRef)
                .WithMany(f => f.SchemeRefinements)
                .HasForeignKey(f => f.Vocabulary)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SchemeRefinement>()
                .HasOne(e => e.CollectedSourceMaterial)
                .WithMany(f => f.SchemeRefinements)
                .HasForeignKey(f => f.TriggeredByMaterial)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermLabelVariant>()
                .HasOne(e => e.VocabularyTermRef)
                .WithMany(f => f.TermLabelVariants)
                .HasForeignKey(f => f.VocabularyTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiLabelingRun>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AiLabelingRuns)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiLabelingRun>()
                .HasOne(e => e.Vocabulary)
                .WithMany(f => f.AiLabelingRuns)
                .HasForeignKey(f => f.GroundingScheme)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceTermMention>()
                .HasOne(e => e.CollectedSourceMaterial)
                .WithMany(f => f.SourceTermMentions)
                .HasForeignKey(f => f.SourceMaterial)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceTermMention>()
                .HasOne(e => e.AiLabelingRunRef)
                .WithMany(f => f.SourceTermMentions)
                .HasForeignKey(f => f.AiLabelingRun)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceTermMention>()
                .HasOne(e => e.Vocabulary)
                .WithMany(f => f.SourceTermMentions)
                .HasForeignKey(f => f.ConceptScheme)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceTermMention>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.SourceTermMentions)
                .HasForeignKey(f => f.IntendedTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermRelation>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.FromTermTermRelations)
                .HasForeignKey(f => f.FromTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermRelation>()
                .HasOne(e => e.VocabularyTermRef)
                .WithMany(f => f.ToTermTermRelations)
                .HasForeignKey(f => f.ToTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermMeaningChange>()
                .HasOne(e => e.VocabularyTermRef)
                .WithMany(f => f.TermMeaningChanges)
                .HasForeignKey(f => f.VocabularyTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermMeaningChange>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.TermMeaningChanges)
                .HasForeignKey(f => f.RecordedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExternalStandardTerm>()
                .HasOne(e => e.OntologyProfileRef)
                .WithMany(f => f.ExternalStandardTerms)
                .HasForeignKey(f => f.OntologyProfile)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExternalStandardTerm>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ExternalStandardTerms)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExternalStandardTerm>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.ExternalStandardTerms)
                .HasForeignKey(f => f.RehomedAsTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleCapabilityTag>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.RoleCapabilityTags)
                .HasForeignKey(f => f.Role)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleCapabilityTag>()
                .HasOne(e => e.VocabularyTerm)
                .WithMany(f => f.RoleCapabilityTags)
                .HasForeignKey(f => f.CapabilityTerm)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureFacetAssignment>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureFacetAssignments)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcedureFacetAssignment>()
                .HasOne(e => e.ClassificationFacet)
                .WithMany(f => f.ProcedureFacetAssignments)
                .HasForeignKey(f => f.Facet)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeConsumerSystem>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.KnowledgeConsumerSystems)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConsumerSystemSync>()
                .HasOne(e => e.KnowledgeConsumerSystem)
                .WithMany(f => f.ConsumerSystemSyncs)
                .HasForeignKey(f => f.ConsumerSystem)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConsumerSystemSync>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.ConsumerSystemSyncs)
                .HasForeignKey(f => f.LoadedVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConsumerSystemSync>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.ConsumerSystemSyncs)
                .HasForeignKey(f => f.SourceResource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentIntegration>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AgentIntegrations)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentIntegration>()
                .HasOne(e => e.KnowledgeConsumerSystem)
                .WithMany(f => f.AgentIntegrations)
                .HasForeignKey(f => f.KnowledgeSystem)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentIntegration>()
                .HasOne(e => e.IntegrationPathway)
                .WithMany(f => f.AgentIntegrations)
                .HasForeignKey(f => f.Pathway)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentIntegration>()
                .HasOne(e => e.GroundingSnapshot)
                .WithMany(f => f.AgentIntegrations)
                .HasForeignKey(f => f.ServesSnapshot)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GroundingSnapshot>()
                .HasOne(e => e.Role)
                .WithMany(f => f.GroundingSnapshots)
                .HasForeignKey(f => f.StewardRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReasonerRun>()
                .HasOne(e => e.GroundingSnapshot)
                .WithMany(f => f.ReasonerRuns)
                .HasForeignKey(f => f.Snapshot)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SnapshotAssertion>()
                .HasOne(e => e.GroundingSnapshot)
                .WithMany(f => f.SnapshotAssertions)
                .HasForeignKey(f => f.Snapshot)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SnapshotAssertion>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.SnapshotAssertions)
                .HasForeignKey(f => f.SourceProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SnapshotAssertion>()
                .HasOne(e => e.Step)
                .WithMany(f => f.SnapshotAssertions)
                .HasForeignKey(f => f.SourceStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SnapshotAssertion>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.SnapshotAssertions)
                .HasForeignKey(f => f.SourceRoleAssignment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SnapshotAssertion>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.SnapshotAssertions)
                .HasForeignKey(f => f.AboutAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.RetrievalSegments)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.RetrievalSegments)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.DecisionPointRef)
                .WithMany(f => f.RetrievalSegments)
                .HasForeignKey(f => f.DecisionPoint)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.RetrievalSegment)
                .WithMany(f => f.RelatedSegmentRetrievalSegments)
                .HasForeignKey(f => f.RelatedSegment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.RetrievalSegments)
                .HasForeignKey(f => f.SourceResource)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RetrievalSegments)
                .HasForeignKey(f => f.AuthoredByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.Role)
                .WithMany(f => f.RetrievalSegments)
                .HasForeignKey(f => f.AccountableRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RetrievalSegment>()
                .HasOne(e => e.RetrievalSegmentRef)
                .WithMany(f => f.ContradictsSegmentRetrievalSegments)
                .HasForeignKey(f => f.ContradictsSegment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeQueryDefinition>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.KnowledgeQueryDefinitions)
                .HasForeignKey(f => f.TargetProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeQuerySource>()
                .HasOne(e => e.KnowledgeQueryDefinition)
                .WithMany(f => f.KnowledgeQuerySources)
                .HasForeignKey(f => f.QueryDefinition)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeQuerySource>()
                .HasOne(e => e.KnowledgeConsumerSystem)
                .WithMany(f => f.KnowledgeQuerySources)
                .HasForeignKey(f => f.ConsumerSystem)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AnsweringAgentAssistantAnswers)
                .HasForeignKey(f => f.AnsweringAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AskedByAgentAssistantAnswers)
                .HasForeignKey(f => f.AskedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.AgentIntegration)
                .WithMany(f => f.AssistantAnswers)
                .HasForeignKey(f => f.ViaIntegration)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.AssistantAnswers)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.Step)
                .WithMany(f => f.AssumedCurrentStepAssistantAnswers)
                .HasForeignKey(f => f.AssumedCurrentStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.AssertedNextStepAssistantAnswers)
                .HasForeignKey(f => f.AssertedNextStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.StepRefRef)
                .WithMany(f => f.RecommendedStepAssistantAnswers)
                .HasForeignKey(f => f.RecommendedStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.AgentRefRef)
                .WithMany(f => f.HumanReviewedByAssistantAnswers)
                .HasForeignKey(f => f.HumanReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantAnswer>()
                .HasOne(e => e.AiAdoptionInitiatif)
                .WithMany(f => f.AssistantAnswers)
                .HasForeignKey(f => f.ReviewedForInitiative)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnswerGrounding>()
                .HasOne(e => e.AssistantAnswerRef)
                .WithMany(f => f.AnswerGroundings)
                .HasForeignKey(f => f.AssistantAnswer)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnswerGrounding>()
                .HasOne(e => e.SnapshotAssertionRef)
                .WithMany(f => f.AnswerGroundings)
                .HasForeignKey(f => f.SnapshotAssertion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnswerGrounding>()
                .HasOne(e => e.RetrievalSegmentRef)
                .WithMany(f => f.AnswerGroundings)
                .HasForeignKey(f => f.RetrievalSegment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnswerRequirementCheck>()
                .HasOne(e => e.AssistantAnswerRef)
                .WithMany(f => f.AnswerRequirementChecks)
                .HasForeignKey(f => f.AssistantAnswer)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnswerRequirementCheck>()
                .HasOne(e => e.RequirementRef)
                .WithMany(f => f.AnswerRequirementChecks)
                .HasForeignKey(f => f.Requirement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AnswerRequirementCheck>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AnswerRequirementChecks)
                .HasForeignKey(f => f.CheckedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiToolInvocation>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AiToolInvocations)
                .HasForeignKey(f => f.InvokingAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiToolInvocation>()
                .HasOne(e => e.StepExecutionRef)
                .WithMany(f => f.AiToolInvocations)
                .HasForeignKey(f => f.StepExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiToolInvocation>()
                .HasOne(e => e.FunctionRef)
                .WithMany(f => f.AiToolInvocations)
                .HasForeignKey(f => f.Function)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PromptTemplate>()
                .HasOne(e => e.PromptTemplate)
                .WithMany(f => f.PromptTemplates)
                .HasForeignKey(f => f.ParentTemplate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PromptTemplate>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.PromptTemplates)
                .HasForeignKey(f => f.SourceProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PromptTemplate>()
                .HasOne(e => e.Role)
                .WithMany(f => f.PromptTemplates)
                .HasForeignKey(f => f.MaintainedByRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PromptTemplate>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.PromptTemplates)
                .HasForeignKey(f => f.UsedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeProjection>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.KnowledgeProjections)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeProjection>()
                .HasOne(e => e.Role)
                .WithMany(f => f.KnowledgeProjections)
                .HasForeignKey(f => f.AudienceRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelAnnotation>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ModelAnnotations)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelAnnotation>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ModelAnnotations)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelAnnotation>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ModelAnnotations)
                .HasForeignKey(f => f.AnnotatedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelAnnotation>()
                .HasOne(e => e.EncodingLifecycleStage)
                .WithMany(f => f.ModelAnnotations)
                .HasForeignKey(f => f.LifecycleStage)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelAnnotation>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.ModelAnnotations)
                .HasForeignKey(f => f.PromotedToFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelAnnotation>()
                .HasOne(e => e.KnowledgeGap)
                .WithMany(f => f.ModelAnnotations)
                .HasForeignKey(f => f.RaisedKnowledgeGap)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeSearchEvent>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeSearchEvents)
                .HasForeignKey(f => f.SearchedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeSearchEvent>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.KnowledgeSearchEvents)
                .HasForeignKey(f => f.SoughtProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeSearchEvent>()
                .HasOne(e => e.RetrievalSegment)
                .WithMany(f => f.KnowledgeSearchEvents)
                .HasForeignKey(f => f.OpenedSegment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeSearchEvent>()
                .HasOne(e => e.KnowledgeProjection)
                .WithMany(f => f.KnowledgeSearchEvents)
                .HasForeignKey(f => f.OpenedProjection)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeSearchEvent>()
                .HasOne(e => e.KnowledgeGap)
                .WithMany(f => f.KnowledgeSearchEvents)
                .HasForeignKey(f => f.LinkedKnowledgeGap)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAdoptionInitiatif>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.AiAdoptionInitiatives)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAdoptionInitiatif>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AiAdoptionInitiatives)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAdoptionInitiatif>()
                .HasOne(e => e.Procedure)
                .WithMany(f => f.AiAdoptionInitiatives)
                .HasForeignKey(f => f.TargetProcedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAdoptionInitiatif>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.AiAdoptionInitiatives)
                .HasForeignKey(f => f.TargetVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAdoptionInitiatif>()
                .HasOne(e => e.AiAdoptionInitiatif)
                .WithMany(f => f.AiAdoptionInitiatives)
                .HasForeignKey(f => f.PrecedingInitiative)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAdoptionInitiatif>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.AiAdoptionInitiatives)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeOutcomeMeasurement>()
                .HasOne(e => e.FacilityRef)
                .WithMany(f => f.KnowledgeOutcomeMeasurements)
                .HasForeignKey(f => f.Facility)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeOutcomeMeasurement>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.KnowledgeOutcomeMeasurements)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeOutcomeMeasurement>()
                .HasOne(e => e.AiAdoptionInitiatif)
                .WithMany(f => f.KnowledgeOutcomeMeasurements)
                .HasForeignKey(f => f.AiInitiative)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeOutcomeMeasurement>()
                .HasOne(e => e.ChangeRequest)
                .WithMany(f => f.KnowledgeOutcomeMeasurements)
                .HasForeignKey(f => f.InformedChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeOutcomeMeasurement>()
                .HasOne(e => e.KnowledgeOutcomeMeasurement)
                .WithMany(f => f.KnowledgeOutcomeMeasurements)
                .HasForeignKey(f => f.ComparisonBaseline)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiInsightProposal>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProposingAgentAiInsightProposals)
                .HasForeignKey(f => f.ProposingAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiInsightProposal>()
                .HasOne(e => e.AiAdoptionInitiatif)
                .WithMany(f => f.AiInsightProposals)
                .HasForeignKey(f => f.SourceInitiative)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiInsightProposal>()
                .HasOne(e => e.ProcedureVersion)
                .WithMany(f => f.AiInsightProposals)
                .HasForeignKey(f => f.TargetProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiInsightProposal>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ValidatedByAgentAiInsightProposals)
                .HasForeignKey(f => f.ValidatedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiInsightProposal>()
                .HasOne(e => e.ChangeRequest)
                .WithMany(f => f.AiInsightProposals)
                .HasForeignKey(f => f.FoldedIntoChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantBenchmark>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AssistantBenchmarks)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssistantBenchmark>()
                .HasOne(e => e.GroundingSnapshotRef)
                .WithMany(f => f.AssistantBenchmarks)
                .HasForeignKey(f => f.GroundingSnapshot)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GovernedModel>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.GovernedModels)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GovernedModel>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.GovernedModels)
                .HasForeignKey(f => f.DomainOwningOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GovernedModel>()
                .HasOne(e => e.Role)
                .WithMany(f => f.GovernedModels)
                .HasForeignKey(f => f.ToolingOwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GovernedModel>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.GovernedModels)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelCharter>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelCharters)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelCharter>()
                .HasOne(e => e.Role)
                .WithMany(f => f.StewardRoleModelCharters)
                .HasForeignKey(f => f.StewardRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelCharter>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.AuthorityRoleModelCharters)
                .HasForeignKey(f => f.AuthorityRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelCharter>()
                .HasOne(e => e.ModelCharter)
                .WithMany(f => f.ModelCharters)
                .HasForeignKey(f => f.SupersedesCharter)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelCharter>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ModelCharters)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardActivity>()
                .HasOne(e => e.ModelCharterRef)
                .WithMany(f => f.StewardActivities)
                .HasForeignKey(f => f.ModelCharter)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardActivity>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.StewardActivities)
                .HasForeignKey(f => f.PerformedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StewardActivity>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.StewardActivities)
                .HasForeignKey(f => f.Release)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeRequest>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelChangeRequests)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeRequest>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RequestedByAgentModelChangeRequests)
                .HasForeignKey(f => f.RequestedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeRequest>()
                .HasOne(e => e.RoleQuestion)
                .WithMany(f => f.ModelChangeRequests)
                .HasForeignKey(f => f.MotivatingQuestion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeRequest>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ApprovedByAgentModelChangeRequests)
                .HasForeignKey(f => f.ApprovedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeRequest>()
                .HasOne(e => e.AgentRefRef)
                .WithMany(f => f.PlacementDecidedByAgentModelChangeRequests)
                .HasForeignKey(f => f.PlacementDecidedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeRequest>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.ModelChangeRequests)
                .HasForeignKey(f => f.TargetRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeAuthorityRule>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ChangeAuthorityRules)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeAuthorityRule>()
                .HasOne(e => e.Role)
                .WithMany(f => f.PermittedRoleChangeAuthorityRules)
                .HasForeignKey(f => f.PermittedRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeAuthorityRule>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.ApprovalRoleChangeAuthorityRules)
                .HasForeignKey(f => f.ApprovalRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeImpactFinding>()
                .HasOne(e => e.ModelChangeRequestRef)
                .WithMany(f => f.ChangeImpactFindings)
                .HasForeignKey(f => f.ModelChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeImpactFinding>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.ChangeImpactFindings)
                .HasForeignKey(f => f.AffectedTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeImpactFinding>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ChangeImpactFindings)
                .HasForeignKey(f => f.FoundByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeIntegrityCheck>()
                .HasOne(e => e.ModelChangeRequestRef)
                .WithMany(f => f.ChangeIntegrityChecks)
                .HasForeignKey(f => f.ModelChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeIntegrityCheck>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ChangeIntegrityChecks)
                .HasForeignKey(f => f.CheckedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeObjection>()
                .HasOne(e => e.ModelChangeRequestRef)
                .WithMany(f => f.ChangeObjections)
                .HasForeignKey(f => f.ModelChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeObjection>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RaisedByAgentChangeObjections)
                .HasForeignKey(f => f.RaisedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeObjection>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ResolvedByAgentChangeObjections)
                .HasForeignKey(f => f.ResolvedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeValidationRun>()
                .HasOne(e => e.ModelChangeRequestRef)
                .WithMany(f => f.ChangeValidationRuns)
                .HasForeignKey(f => f.ModelChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeValidationRun>()
                .HasOne(e => e.TestSuiteRef)
                .WithMany(f => f.ChangeValidationRuns)
                .HasForeignKey(f => f.TestSuite)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpectedInferenceCheck>()
                .HasOne(e => e.ChangeValidationRunRef)
                .WithMany(f => f.ExpectedInferenceChecks)
                .HasForeignKey(f => f.ChangeValidationRun)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpectedInferenceCheck>()
                .HasOne(e => e.RulebookField)
                .WithMany(f => f.ExpectedInferenceChecks)
                .HasForeignKey(f => f.ExpectedField)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelConsumer>()
                .HasOne(e => e.GovernedModel)
                .WithMany(f => f.ModelConsumers)
                .HasForeignKey(f => f.DependsOnModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelConsumer>()
                .HasOne(e => e.ConformanceSubstrateRef)
                .WithMany(f => f.ModelConsumers)
                .HasForeignKey(f => f.ConformanceSubstrate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelConsumer>()
                .HasOne(e => e.Role)
                .WithMany(f => f.ModelConsumers)
                .HasForeignKey(f => f.OwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConsumerRevalidation>()
                .HasOne(e => e.RulebookReleaseRef)
                .WithMany(f => f.ConsumerRevalidations)
                .HasForeignKey(f => f.RulebookRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConsumerRevalidation>()
                .HasOne(e => e.ModelConsumerRef)
                .WithMany(f => f.ConsumerRevalidations)
                .HasForeignKey(f => f.ModelConsumer)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelDocument>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelDocuments)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelDocument>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.ModelDocuments)
                .HasForeignKey(f => f.DocumentedRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StalenessQueryRun>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.StalenessQueryRuns)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StalenessQueryRun>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.StalenessQueryRuns)
                .HasForeignKey(f => f.RanByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExternalDependencyRevision>()
                .HasOne(e => e.OntologyProfileRef)
                .WithMany(f => f.ExternalDependencyRevisions)
                .HasForeignKey(f => f.OntologyProfile)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExternalDependencyRevision>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ExternalDependencyRevisions)
                .HasForeignKey(f => f.TrackedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExternalDependencyRevision>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.ExternalDependencyRevisions)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.StakeholderQuestions)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AskedByAgentStakeholderQuestions)
                .HasForeignKey(f => f.AskedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.RoleQuestion)
                .WithMany(f => f.StakeholderQuestions)
                .HasForeignKey(f => f.AnsweringRoleQuestion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AnsweredByAgentStakeholderQuestions)
                .HasForeignKey(f => f.AnsweredByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.AgentRefRef)
                .WithMany(f => f.TriagedByAgentStakeholderQuestions)
                .HasForeignKey(f => f.TriagedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.ModelChangeRequest)
                .WithMany(f => f.StakeholderQuestions)
                .HasForeignKey(f => f.ResultingChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderQuestion>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.StakeholderQuestions)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelExpansionRequest>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelExpansionRequests)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelExpansionRequest>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.ModelExpansionRequests)
                .HasForeignKey(f => f.RequestingOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelExpansionRequest>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RequestedByAgentModelExpansionRequests)
                .HasForeignKey(f => f.RequestedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelExpansionRequest>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.DecidedByAgentModelExpansionRequests)
                .HasForeignKey(f => f.DecidedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpansionConceptFit>()
                .HasOne(e => e.ModelExpansionRequestRef)
                .WithMany(f => f.ExpansionConceptFits)
                .HasForeignKey(f => f.ModelExpansionRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpansionConceptFit>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.ExpansionConceptFits)
                .HasForeignKey(f => f.CoveringTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionSetEntry>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.CompetencyQuestionSetEntries)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionSetEntry>()
                .HasOne(e => e.RoleQuestionRef)
                .WithMany(f => f.CompetencyQuestionSetEntries)
                .HasForeignKey(f => f.RoleQuestion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionSetEntry>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.CompetencyQuestionSetEntries)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionRun>()
                .HasOne(e => e.CompetencyQuestionSetEntry)
                .WithMany(f => f.CompetencyQuestionRuns)
                .HasForeignKey(f => f.CqSetEntry)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionRun>()
                .HasOne(e => e.RulebookReleaseRef)
                .WithMany(f => f.CompetencyQuestionRuns)
                .HasForeignKey(f => f.RulebookRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionRun>()
                .HasOne(e => e.CompetencyQuestionRun)
                .WithMany(f => f.CompetencyQuestionRuns)
                .HasForeignKey(f => f.PriorRun)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionRun>()
                .HasOne(e => e.ModelChangeRequest)
                .WithMany(f => f.CompetencyQuestionRuns)
                .HasForeignKey(f => f.DefectChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionReview>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.CompetencyQuestionReviews)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestionReview>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.CompetencyQuestionReviews)
                .HasForeignKey(f => f.ReviewedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<QualityAssessment>()
                .HasOne(e => e.RulebookReleaseRef)
                .WithMany(f => f.QualityAssessments)
                .HasForeignKey(f => f.RulebookRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<QualityAssessment>()
                .HasOne(e => e.QualityCriteria)
                .WithMany(f => f.QualityAssessments)
                .HasForeignKey(f => f.QualityCriterion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<QualityAssessment>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.QualityAssessments)
                .HasForeignKey(f => f.AssessedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermDefinition>()
                .HasOne(e => e.RulebookTableRef)
                .WithMany(f => f.TermDefinitions)
                .HasForeignKey(f => f.RulebookTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermDefinition>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.DraftedByAgentTermDefinitions)
                .HasForeignKey(f => f.DraftedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TermDefinition>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.RevisedByAgentTermDefinitions)
                .HasForeignKey(f => f.RevisedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelProposals)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProposedByAgentModelProposals)
                .HasForeignKey(f => f.ProposedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.ModelProposals)
                .HasForeignKey(f => f.SourceDocument)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ReviewedByAgentModelProposals)
                .HasForeignKey(f => f.ReviewedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.AgentRefRef)
                .WithMany(f => f.CommittedByAgentModelProposals)
                .HasForeignKey(f => f.CommittedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.ModelProposals)
                .HasForeignKey(f => f.AdoptedInRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelProposal>()
                .HasOne(e => e.InstanceDataVersion)
                .WithMany(f => f.ModelProposals)
                .HasForeignKey(f => f.AdoptedInDataVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentInstantCheck>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.AssignmentInstantChecks)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentInstantCheck>()
                .HasOne(e => e.RoleAssignmentRef)
                .WithMany(f => f.AssignmentInstantChecks)
                .HasForeignKey(f => f.RoleAssignment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InstanceDataVersion>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.InstanceDataVersions)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InstanceDataVersion>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.InstanceDataVersions)
                .HasForeignKey(f => f.ConformsToRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DomainCoverageArea>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.DomainCoverageAreas)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DomainCoverageArea>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.DomainCoverageAreas)
                .HasForeignKey(f => f.CoveringTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GovernanceStageControl>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.GovernanceStageControls)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessDesignDecision>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.ProcessDesignDecisions)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessDesignDecision>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ProcessDesignDecisions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProcessDesignDecision>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProcessDesignDecisions)
                .HasForeignKey(f => f.DecidedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.ModelChangeRequestRef)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.ModelChangeRequest)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.Release)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.InstanceDataVersionRef)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.InstanceDataVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.RulebookTable)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.AffectedTable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.ChangedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelChangeLogEntry>()
                .HasOne(e => e.ModelChangeLogEntry)
                .WithMany(f => f.ModelChangeLogEntries)
                .HasForeignKey(f => f.RevertsEntry)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DriftObservation>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.DriftObservations)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DriftObservation>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.DriftObservations)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DriftObservation>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.DriftObservations)
                .HasForeignKey(f => f.ObservedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DriftObservation>()
                .HasOne(e => e.RulebookRelease)
                .WithMany(f => f.DriftObservations)
                .HasForeignKey(f => f.SinceRelease)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourcingFunction>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.ClientOrganizationSourcingFunctions)
                .HasForeignKey(f => f.ClientOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourcingFunction>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.SourcingFunctions)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourcingFunction>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.ExecutingOrganizationSourcingFunctions)
                .HasForeignKey(f => f.ExecutingOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourcingFunction>()
                .HasOne(e => e.OrganizationRefRef)
                .WithMany(f => f.SpecificationHolderSourcingFunctions)
                .HasForeignKey(f => f.SpecificationHolder)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourcingFunction>()
                .HasOne(e => e.OrganizationRefRefRef)
                .WithMany(f => f.MethodHolderSourcingFunctions)
                .HasForeignKey(f => f.MethodHolder)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeAudit>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.KnowledgeAudits)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeAudit>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeAudits)
                .HasForeignKey(f => f.ConductedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeAuditItem>()
                .HasOne(e => e.KnowledgeAuditRef)
                .WithMany(f => f.KnowledgeAuditItems)
                .HasForeignKey(f => f.KnowledgeAudit)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeAuditItem>()
                .HasOne(e => e.SourcingFunctionRef)
                .WithMany(f => f.KnowledgeAuditItems)
                .HasForeignKey(f => f.SourcingFunction)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeAuditItem>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.KnowledgeAuditItems)
                .HasForeignKey(f => f.ProviderHoldingKnowledge)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeAuditItem>()
                .HasOne(e => e.KnowledgeGap)
                .WithMany(f => f.KnowledgeAuditItems)
                .HasForeignKey(f => f.NamedKnowledgeGap)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeCaptureInitiatif>()
                .HasOne(e => e.SourcingFunctionRef)
                .WithMany(f => f.KnowledgeCaptureInitiatives)
                .HasForeignKey(f => f.SourcingFunction)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeCaptureInitiatif>()
                .HasOne(e => e.KnowledgeMethodRef)
                .WithMany(f => f.KnowledgeCaptureInitiatives)
                .HasForeignKey(f => f.KnowledgeMethod)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeCaptureInitiatif>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeCaptureInitiatives)
                .HasForeignKey(f => f.LeadAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeWorkforcePosition>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.KnowledgeWorkforcePositions)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeWorkforcePosition>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.KnowledgeWorkforcePositions)
                .HasForeignKey(f => f.Role)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeWorkforcePosition>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeWorkforcePositions)
                .HasForeignKey(f => f.FilledByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProviderEngagement>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.ClientOrganizationProviderEngagements)
                .HasForeignKey(f => f.ClientOrganization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProviderEngagement>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.ProviderProviderEngagements)
                .HasForeignKey(f => f.Provider)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProviderEngagement>()
                .HasOne(e => e.SourcingFunctionRef)
                .WithMany(f => f.ProviderEngagements)
                .HasForeignKey(f => f.SourcingFunction)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeDeliverable>()
                .HasOne(e => e.ProviderEngagementRef)
                .WithMany(f => f.KnowledgeDeliverables)
                .HasForeignKey(f => f.ProviderEngagement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CorporateGovernanceProgram>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.CorporateGovernancePrograms)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecordsRetentionPolicy>()
                .HasOne(e => e.CorporateGovernanceProgram)
                .WithMany(f => f.RecordsRetentionPolicies)
                .HasForeignKey(f => f.GovernanceProgram)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiModelDeployment>()
                .HasOne(e => e.AiRegistryModelVersion)
                .WithMany(f => f.AiModelDeployments)
                .HasForeignKey(f => f.ModelVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiModelDeployment>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.AiModelDeployments)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiModelEvaluation>()
                .HasOne(e => e.AiRegistryModelVersion)
                .WithMany(f => f.AiModelEvaluations)
                .HasForeignKey(f => f.ModelVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAgentAccountability>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AiAgentAiAgentAccountabilities)
                .HasForeignKey(f => f.AiAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAgentAccountability>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.AccountableAgentAiAgentAccountabilities)
                .HasForeignKey(f => f.AccountableAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AiAgentAccountability>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.AiAgentAccountabilities)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentUpgradeAssessment>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.CurrentAgentAgentUpgradeAssessments)
                .HasForeignKey(f => f.CurrentAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentUpgradeAssessment>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.CandidateAgentAgentUpgradeAssessments)
                .HasForeignKey(f => f.CandidateAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentUpgradeAssessment>()
                .HasOne(e => e.AgentRefRef)
                .WithMany(f => f.AssessedByAgentAgentUpgradeAssessments)
                .HasForeignKey(f => f.AssessedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentUpdatePolicy>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AssignmentUpdatePolicies)
                .HasForeignKey(f => f.TriggerOwnerRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.RoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.Role)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.AssignmentUpdatePolicy)
                .WithMany(f => f.RoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.GoverningPolicy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.TriggeredByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.EndingAssignmentRoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.EndingAssignment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.RoleAssignmentRef)
                .WithMany(f => f.ReplacementAssignmentRoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.ReplacementAssignment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.ProcedureExecution)
                .WithMany(f => f.RoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.DependentExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignmentUpdateTask>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.RoleAssignmentUpdateTasks)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentRoutedNotice>()
                .HasOne(e => e.ProcedureExecutionRef)
                .WithMany(f => f.AssignmentRoutedNotices)
                .HasForeignKey(f => f.ProcedureExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentRoutedNotice>()
                .HasOne(e => e.Step)
                .WithMany(f => f.AssignmentRoutedNotices)
                .HasForeignKey(f => f.NoticeStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentRoutedNotice>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AssignmentRoutedNotices)
                .HasForeignKey(f => f.RoutedToAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PractitionerExpertise>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.PractitionerExpertise)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PractitionerExpertise>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.PractitionerExpertise)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PractitionerExpertise>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.PractitionerExpertise)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PractitionerExpertise>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.PractitionerExpertise)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CriticalIncident>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.CriticalIncidents)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CriticalIncident>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.CriticalIncidents)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CriticalIncident>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.CriticalIncidents)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CriticalIncident>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.CriticalIncidents)
                .HasForeignKey(f => f.Narrator)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CriticalIncident>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.CriticalIncidents)
                .HasForeignKey(f => f.JudgmentFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InterviewProbe>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.InterviewProbes)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InterviewProbe>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.InterviewProbes)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedAction>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.ObservedActions)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedAction>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ObservedActions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedAction>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ObservedActions)
                .HasForeignKey(f => f.Practitioner)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ObservedAction>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.ObservedActions)
                .HasForeignKey(f => f.CapturedAsFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationParticipant>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.ElicitationParticipants)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ElicitationParticipant>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ElicitationParticipants)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RepresentationReview>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.RepresentationReviews)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RepresentationReview>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.RepresentationReviews)
                .HasForeignKey(f => f.ReviewerAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowViewDivergence>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.WorkflowViewDivergences)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowViewDivergence>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.WorkflowViewDivergences)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowViewDivergence>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.WorkflowViewDivergences)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowViewDivergence>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.HolderAWorkflowViewDivergences)
                .HasForeignKey(f => f.HolderA)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowViewDivergence>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.HolderBWorkflowViewDivergences)
                .HasForeignKey(f => f.HolderB)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowViewDivergence>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.WorkflowViewDivergences)
                .HasForeignKey(f => f.ReconciledIntoFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpertCognition>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ExpertCognitions)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpertCognition>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ExpertCognitions)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpertCognition>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.ExpertCognitions)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConceptLadderRung>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.ConceptLadderRungs)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ConceptLadderRung>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ConceptLadderRungs)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RepertoryGridConstruct>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.RepertoryGridConstructs)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RepertoryGridConstruct>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.RepertoryGridConstructs)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeConversion>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.KnowledgeConversions)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeConversion>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.KnowledgeConversions)
                .HasForeignKey(f => f.ResultFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeHolding>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.KnowledgeHoldings)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeHolding>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.KnowledgeHoldings)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeHolding>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeHoldings)
                .HasForeignKey(f => f.HolderAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeHolding>()
                .HasOne(e => e.KnowledgeFragment)
                .WithMany(f => f.KnowledgeHoldings)
                .HasForeignKey(f => f.FormalizedAs)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FragmentCorroboration>()
                .HasOne(e => e.KnowledgeFragmentRef)
                .WithMany(f => f.FragmentCorroborations)
                .HasForeignKey(f => f.KnowledgeFragment)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FragmentCorroboration>()
                .HasOne(e => e.ElicitationSessionRef)
                .WithMany(f => f.FragmentCorroborations)
                .HasForeignKey(f => f.ElicitationSession)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FragmentCorroboration>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.FragmentCorroborations)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTestOutcome>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.KnowledgeTestOutcomes)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.CommunitiesOfPractice)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.CommunityOfPractice)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.HolderAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.Facility)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.HolderFacility)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.KnowHowCarrier)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.BuildsOnKnowHow)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowHowCarrier>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.KnowHowCarriers)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTransfer>()
                .HasOne(e => e.KnowHowCarrier)
                .WithMany(f => f.KnowledgeTransfers)
                .HasForeignKey(f => f.KnowHow)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTransfer>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.FromAgentKnowledgeTransfers)
                .HasForeignKey(f => f.FromAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTransfer>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.RecipientAgentKnowledgeTransfers)
                .HasForeignKey(f => f.RecipientAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTransfer>()
                .HasOne(e => e.CommunitiesOfPractice)
                .WithMany(f => f.KnowledgeTransfers)
                .HasForeignKey(f => f.CommunityOfPractice)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeRepositoryEntry>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.KnowledgeRepositoryEntries)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeRepositoryEntry>()
                .HasOne(e => e.KnowHowCarrier)
                .WithMany(f => f.KnowledgeRepositoryEntries)
                .HasForeignKey(f => f.KnowHow)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeRepositoryEntry>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.AuthorAgentKnowledgeRepositoryEntries)
                .HasForeignKey(f => f.AuthorAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeRepositoryEntry>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.SourceExpertKnowledgeRepositoryEntries)
                .HasForeignKey(f => f.SourceExpert)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeRepositoryEntry>()
                .HasOne(e => e.ProcedureExecution)
                .WithMany(f => f.KnowledgeRepositoryEntries)
                .HasForeignKey(f => f.FedFromExecution)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeRepositoryEntry>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.KnowledgeRepositoryEntries)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunityMembership>()
                .HasOne(e => e.CommunitiesOfPractice)
                .WithMany(f => f.CommunityMemberships)
                .HasForeignKey(f => f.CommunityOfPractice)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommunityMembership>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.CommunityMemberships)
                .HasForeignKey(f => f.Agent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceRelationship>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.KnowledgeEngineerSourceRelationships)
                .HasForeignKey(f => f.KnowledgeEngineer)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceRelationship>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.SourceAgentSourceRelationships)
                .HasForeignKey(f => f.SourceAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SourceRelationship>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.SourceRelationships)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DepartmentProcessAccount>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.DepartmentProcessAccounts)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DepartmentProcessAccount>()
                .HasOne(e => e.Organization)
                .WithMany(f => f.DepartmentProcessAccounts)
                .HasForeignKey(f => f.Department)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DepartmentProcessAccount>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.DepartmentProcessAccounts)
                .HasForeignKey(f => f.StakeholderAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DepartmentProcessAccount>()
                .HasOne(e => e.DepartmentProcessAccount)
                .WithMany(f => f.DepartmentProcessAccounts)
                .HasForeignKey(f => f.ConflictsWithAccount)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProblemOccurrence>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProblemOccurrences)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProblemOccurrence>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProblemOccurrences)
                .HasForeignKey(f => f.SolvedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProblemOccurrence>()
                .HasOne(e => e.KnowledgeRepositoryEntry)
                .WithMany(f => f.ProblemOccurrences)
                .HasForeignKey(f => f.SolutionEntry)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProblemOccurrence>()
                .HasOne(e => e.ProblemOccurrence)
                .WithMany(f => f.ProblemOccurrences)
                .HasForeignKey(f => f.PriorOccurrence)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OnboardingRecord>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.OnboardingRecords)
                .HasForeignKey(f => f.NewStarter)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OnboardingRecord>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.OnboardingRecords)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OnboardingRecord>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.OnboardingRecords)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SharingRecognition>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.SharingRecognitions)
                .HasForeignKey(f => f.RecognizedAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SharingRecognition>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.SharingRecognitions)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CapabilityDecline>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.CapabilityDeclines)
                .HasForeignKey(f => f.Organization)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CapabilityDecline>()
                .HasOne(e => e.CapabilityDecline)
                .WithMany(f => f.CapabilityDeclines)
                .HasForeignKey(f => f.PrecedingStageDecline)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.KnowledgeTraces)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.KnowledgeTraces)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.RequirementRef)
                .WithMany(f => f.KnowledgeTraces)
                .HasForeignKey(f => f.Requirement)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.CollectedSourceMaterial)
                .WithMany(f => f.KnowledgeTraces)
                .HasForeignKey(f => f.SourceMaterial)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.DerivedByAgentKnowledgeTraces)
                .HasForeignKey(f => f.DerivedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.AgentRef)
                .WithMany(f => f.ValidatedByAgentKnowledgeTraces)
                .HasForeignKey(f => f.ValidatedByAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KnowledgeTrace>()
                .HasOne(e => e.Resource)
                .WithMany(f => f.KnowledgeTraces)
                .HasForeignKey(f => f.ContradictedDocument)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MinedFlowEdge>()
                .HasOne(e => e.ProcessMiningRunRef)
                .WithMany(f => f.MinedFlowEdges)
                .HasForeignKey(f => f.ProcessMiningRun)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MinedFlowEdge>()
                .HasOne(e => e.Step)
                .WithMany(f => f.FromStepMinedFlowEdges)
                .HasForeignKey(f => f.FromStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MinedFlowEdge>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ToStepMinedFlowEdges)
                .HasForeignKey(f => f.ToStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MinedFlowEdge>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.MinedFlowEdges)
                .HasForeignKey(f => f.IntentDecisionBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectionOccasion>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.CollectionOccasions)
                .HasForeignKey(f => f.Procedure)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CollectionOccasion>()
                .HasOne(e => e.EvaluationContextRef)
                .WithMany(f => f.CollectionOccasions)
                .HasForeignKey(f => f.EvaluationContext)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderPerspectif>()
                .HasOne(e => e.ProcedureVersionRef)
                .WithMany(f => f.StakeholderPerspectives)
                .HasForeignKey(f => f.ProcedureVersion)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderPerspectif>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.StakeholderPerspectives)
                .HasForeignKey(f => f.Step)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderPerspectif>()
                .HasOne(e => e.Role)
                .WithMany(f => f.StakeholderPerspectives)
                .HasForeignKey(f => f.HolderRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderPerspectif>()
                .HasOne(e => e.CollectedSourceMaterial)
                .WithMany(f => f.StakeholderPerspectives)
                .HasForeignKey(f => f.SourceMaterial)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StakeholderPerspectif>()
                .HasOne(e => e.StakeholderPerspectif)
                .WithMany(f => f.StakeholderPerspectives)
                .HasForeignKey(f => f.ConflictsWithPerspective)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelPilot>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelPilots)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelActivityExpert>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelActivityExperts)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelActivityExpert>()
                .HasOne(e => e.Agent)
                .WithMany(f => f.ModelActivityExperts)
                .HasForeignKey(f => f.Expert)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ModelDataMappingRun>()
                .HasOne(e => e.GovernedModelRef)
                .WithMany(f => f.ModelDataMappingRuns)
                .HasForeignKey(f => f.GovernedModel)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArtifactHandoff>()
                .HasOne(e => e.StepVariableRef)
                .WithMany(f => f.ArtifactHandoffs)
                .HasForeignKey(f => f.StepVariable)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArtifactHandoff>()
                .HasOne(e => e.Step)
                .WithMany(f => f.FromStepArtifactHandoffs)
                .HasForeignKey(f => f.FromStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArtifactHandoff>()
                .HasOne(e => e.StepRef)
                .WithMany(f => f.ToStepArtifactHandoffs)
                .HasForeignKey(f => f.ToStep)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Default configuration - override in your app
                optionsBuilder.UseSqlServer("Server=.,1433;Database=YourDatabase;User ID=sa;Password=YourPassword;Encrypt=false;TrustServerCertificate=true");
            }
        }
    }
}
