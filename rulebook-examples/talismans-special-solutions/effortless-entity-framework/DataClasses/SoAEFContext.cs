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

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<ApprovalGate> ApprovalGates { get; set; }
        public DbSet<StepPrecedence> StepPrecedence { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<HumanAgent> HumanAgents { get; set; }
        public DbSet<AIAgent> AIAgents { get; set; }
        public DbSet<AutomatedPipeline> AutomatedPipelines { get; set; }
        public DbSet<WorkflowStatusConcept> WorkflowStatusConcepts { get; set; }
        public DbSet<AgentCapabilityConcept> AgentCapabilityConcepts { get; set; }
        public DbSet<ArtifactTypeConcept> ArtifactTypeConcepts { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<WorkflowArtifact> WorkflowArtifacts { get; set; }
        public DbSet<GovernanceRole> GovernanceRoles { get; set; }
        public DbSet<ChangeLog> ChangeLog { get; set; }
        public DbSet<VocabularyReconciliation> VocabularyReconciliations { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<CompetencyQuestion> CompetencyQuestions { get; set; }
        public DbSet<ScenarioCQEffect> ScenarioCQEffects { get; set; }
        public DbSet<ConformanceTest> ConformanceTests { get; set; }
        public DbSet<__meta__> __meta__ { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workflow>()
                .HasOne(e => e.WorkflowStatusConcept)
                .WithMany(f => f.WorkflowStatusWorkflows)
                .HasForeignKey(f => f.WorkflowStatus)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Workflow>()
                .HasOne(e => e.WorkflowStep)
                .WithMany(f => f.Workflows)
                .HasForeignKey(f => f.WorkflowSteps)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.WorkflowRef)
                .WithMany(f => f.WorkflowWorkflowSteps)
                .HasForeignKey(f => f.Workflow)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AssignedRoleWorkflowSteps)
                .HasForeignKey(f => f.AssignedRole)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.Dataset)
                .WithMany(f => f.WorkflowSteps)
                .HasForeignKey(f => f.ConsumesDataset)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.WorkflowArtifact)
                .WithMany(f => f.ProducesArtifactsWorkflowSteps)
                .HasForeignKey(f => f.ProducesArtifacts)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.WorkflowArtifactRef)
                .WithMany(f => f.RequiresArtifactsWorkflowSteps)
                .HasForeignKey(f => f.RequiresArtifacts)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.ApprovalGateRef)
                .WithMany(f => f.WorkflowSteps)
                .HasForeignKey(f => f.ApprovalGate)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.StepPrecedence)
                .WithMany(f => f.PrecedesWorkflowSteps)
                .HasForeignKey(f => f.Precedes)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStep>()
                .HasOne(e => e.StepPrecedenceRef)
                .WithMany(f => f.PrecededByWorkflowSteps)
                .HasForeignKey(f => f.PrecededBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ApprovalGate>()
                .HasOne(e => e.WorkflowStepRef)
                .WithMany(f => f.ApprovalGates)
                .HasForeignKey(f => f.WorkflowStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepPrecedence>()
                .HasOne(e => e.WorkflowStep)
                .WithMany(f => f.FromStepStepPrecedence)
                .HasForeignKey(f => f.FromStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StepPrecedence>()
                .HasOne(e => e.WorkflowStepRef)
                .WithMany(f => f.ToStepStepPrecedence)
                .HasForeignKey(f => f.ToStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.AgentCapabilityConcept)
                .WithMany(f => f.HasCapabilityRoles)
                .HasForeignKey(f => f.HasCapability)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.HumanAgent)
                .WithMany(f => f.FilledByHumanAgentRoles)
                .HasForeignKey(f => f.FilledByHumanAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.AIAgent)
                .WithMany(f => f.FilledByAIAgentRoles)
                .HasForeignKey(f => f.FilledByAIAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.AutomatedPipeline)
                .WithMany(f => f.FilledByAutomatedPipelineRoles)
                .HasForeignKey(f => f.FilledByAutomatedPipeline)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.Department)
                .WithMany(f => f.OwnedByRoles)
                .HasForeignKey(f => f.OwnedBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.Role)
                .WithMany(f => f.DelegatesToRoles)
                .HasForeignKey(f => f.DelegatesTo)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.WorkflowStep)
                .WithMany(f => f.Roles)
                .HasForeignKey(f => f.WorkflowSteps)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.FromDelegatesToRoles)
                .HasForeignKey(f => f.FromDelegatesTo)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.Roles)
                .HasForeignKey(f => f.RoleAssignments)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.RoleRef)
                .WithMany(f => f.RoleRoleAssignments)
                .HasForeignKey(f => f.Role)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.HumanAgent)
                .WithMany(f => f.FilledByHumanAgentRoleAssignments)
                .HasForeignKey(f => f.FilledByHumanAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.AIAgent)
                .WithMany(f => f.FilledByAIAgentRoleAssignments)
                .HasForeignKey(f => f.FilledByAIAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoleAssignment>()
                .HasOne(e => e.AutomatedPipeline)
                .WithMany(f => f.FilledByAutomatedPipelineRoleAssignments)
                .HasForeignKey(f => f.FilledByAutomatedPipeline)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Department>()
                .HasOne(e => e.Role)
                .WithMany(f => f.Departments)
                .HasForeignKey(f => f.Roles)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HumanAgent>()
                .HasOne(e => e.Role)
                .WithMany(f => f.HumanAgents)
                .HasForeignKey(f => f.Roles)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HumanAgent>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.HumanAgents)
                .HasForeignKey(f => f.RoleAssignments)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AIAgent>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AIAgents)
                .HasForeignKey(f => f.Roles)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AIAgent>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.AIAgents)
                .HasForeignKey(f => f.RoleAssignments)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AIAgent>()
                .HasOne(e => e.WorkflowArtifact)
                .WithMany(f => f.AIAgents)
                .HasForeignKey(f => f.AttributedArtifacts)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AutomatedPipeline>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AutomatedPipelines)
                .HasForeignKey(f => f.Roles)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AutomatedPipeline>()
                .HasOne(e => e.RoleAssignment)
                .WithMany(f => f.AutomatedPipelines)
                .HasForeignKey(f => f.RoleAssignments)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowStatusConcept>()
                .HasOne(e => e.Workflow)
                .WithMany(f => f.WorkflowStatusConcepts)
                .HasForeignKey(f => f.Workflows)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AgentCapabilityConcept>()
                .HasOne(e => e.Role)
                .WithMany(f => f.AgentCapabilityConcepts)
                .HasForeignKey(f => f.Roles)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArtifactTypeConcept>()
                .HasOne(e => e.WorkflowArtifact)
                .WithMany(f => f.ArtifactTypeConcepts)
                .HasForeignKey(f => f.WorkflowArtifacts)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Dataset>()
                .HasOne(e => e.WorkflowStep)
                .WithMany(f => f.Datasets)
                .HasForeignKey(f => f.ConsumedBySteps)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.ArtifactTypeConcept)
                .WithMany(f => f.ArtifactTypeWorkflowArtifacts)
                .HasForeignKey(f => f.ArtifactType)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.WorkflowStep)
                .WithMany(f => f.ProducedByStepWorkflowArtifacts)
                .HasForeignKey(f => f.ProducedByStep)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.WorkflowStepRef)
                .WithMany(f => f.RequiredByStepsWorkflowArtifacts)
                .HasForeignKey(f => f.RequiredBySteps)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.WorkflowArtifact)
                .WithMany(f => f.WorkflowArtifacts)
                .HasForeignKey(f => f.DerivedFromArtifact)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.HumanAgent)
                .WithMany(f => f.WorkflowArtifacts)
                .HasForeignKey(f => f.AttributedToHumanAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.AIAgent)
                .WithMany(f => f.WorkflowArtifacts)
                .HasForeignKey(f => f.AttributedToAIAgent)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WorkflowArtifact>()
                .HasOne(e => e.AutomatedPipeline)
                .WithMany(f => f.WorkflowArtifacts)
                .HasForeignKey(f => f.AttributedToAutomatedPipeline)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GovernanceRole>()
                .HasOne(e => e.ChangeLog)
                .WithMany(f => f.GovernanceRoles)
                .HasForeignKey(f => f.ApprovedChanges)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChangeLog>()
                .HasOne(e => e.GovernanceRole)
                .WithMany(f => f.ApprovedByChangeLog)
                .HasForeignKey(f => f.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetencyQuestion>()
                .HasOne(e => e.Scenario)
                .WithMany(f => f.CompetencyQuestions)
                .HasForeignKey(f => f.SimulateScenario)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ScenarioCQEffect>()
                .HasOne(e => e.ScenarioRef)
                .WithMany(f => f.ScenarioCQEffects)
                .HasForeignKey(f => f.Scenario)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ScenarioCQEffect>()
                .HasOne(e => e.CompetencyQuestionRef)
                .WithMany(f => f.ScenarioCQEffects)
                .HasForeignKey(f => f.CompetencyQuestion)
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
