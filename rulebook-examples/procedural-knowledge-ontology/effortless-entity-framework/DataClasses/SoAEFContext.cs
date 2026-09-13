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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .HasOne(e => e.OrganizationRef)
                .WithMany(f => f.Agents)
                .HasForeignKey(f => f.Organization)
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
            modelBuilder.Entity<ProcedureVersion>()
                .HasOne(e => e.ProcedureRef)
                .WithMany(f => f.ProcedureVersions)
                .HasForeignKey(f => f.Procedure)
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
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProcedureStatusChanges)
                .HasForeignKey(f => f.ChangedByAgent)
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
                .HasOne(e => e.Agent)
                .WithMany(f => f.ProcedureExecutions)
                .HasForeignKey(f => f.ExecutedByAgent)
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
                .WithMany(f => f.StepExecutions)
                .HasForeignKey(f => f.ExecutedByAgent)
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
            modelBuilder.Entity<VocabularyTerm>()
                .HasOne(e => e.VocabularyRef)
                .WithMany(f => f.VocabularyTerms)
                .HasForeignKey(f => f.Vocabulary)
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
