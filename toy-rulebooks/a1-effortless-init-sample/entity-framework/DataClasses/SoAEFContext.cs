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

        public DbSet<HelloWho> HelloWhos { get; set; }

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
