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

        public DbSet<ProjectMetadata> ProjectMetadata { get; set; }
        public DbSet<ExecutionSubstrate> ExecutionSubstrates { get; set; }
        public DbSet<OrchestrationComponent> OrchestrationComponents { get; set; }
        public DbSet<RulebookSourceSpoke> RulebookSourceSpokes { get; set; }
        public DbSet<SsotmeProxy> SsotmeProxy { get; set; }
        public DbSet<TestingFramework> TestingFramework { get; set; }
        public DbSet<RulebookDomain> RulebookDomains { get; set; }
        public DbSet<CoreDataFlow> CoreDataFlows { get; set; }
        public DbSet<ProjectConfiguration> ProjectConfiguration { get; set; }
        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AppPermission> AppPermissions { get; set; }
        public DbSet<AppNavigation> AppNavigation { get; set; }
        public DbSet<AppScreen> AppScreens { get; set; }
        public DbSet<AppAPI> AppAPIs { get; set; }
        public DbSet<AddToolCatalog> AddToolCatalog { get; set; }
        public DbSet<BuildPipeline> BuildPipeline { get; set; }
        public DbSet<AdminPortalRuntime> AdminPortalRuntime { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
