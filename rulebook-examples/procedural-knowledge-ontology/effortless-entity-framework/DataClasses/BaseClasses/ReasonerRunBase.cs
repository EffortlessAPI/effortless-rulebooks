
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;
using F = SqlOnAir.DotNet.Lib.DataClasses.Formulas.EfFormulaFns;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ReasonerRuns")]
    public class ReasonerRunBase : SoAEntityBase
    {
        [Key]
        public string ReasonerRunId { get; set; }

        // Formula Name (rulebook: ={{Snapshot}} & " / " & {{ReasonerProfile}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Snapshot)), F.S(" / "), F.Text(F.Of(this.ReasonerProfile))))); set { }
        }

        public string? ReasonerProfile { get; set; }
        public DateTimeOffset? RanAt { get; set; }
        public DateTimeOffset? MaterializedAt { get; set; }
        public bool? IsConsistent { get; set; }
        public int? InferredTripleCount { get; set; }
        public decimal? DurationSeconds { get; set; }
        public decimal? TimeBudgetSeconds { get; set; }
        public int? DroppedAxiomCount { get; set; }
        public bool? SchemaValidationPassed { get; set; }
        // Formula IsRichnessTractabilityFailure (rulebook: =OR({{DurationSeconds}} > {{TimeBudgetSeconds}}, {{DroppedAxiomCount}} > 0))
        [NotMapped]
        public bool? IsRichnessTractabilityFailure
        {
            get => F.AsBool(F.Memo(this, "IsRichnessTractabilityFailure", () => F.Or(F.Bool3(F.Cmp(F.Nullif(F.Of(this.DurationSeconds)), ">", F.Nullif(F.Of(this.TimeBudgetSeconds)))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.DroppedAxiomCount)), ">", F.I(0)))))); set { }
        }

        // Formula PassesSchemaButFailsReasoner (rulebook: =AND({{SchemaValidationPassed}}, {{IsConsistent}} = FALSE))
        [NotMapped]
        public bool? PassesSchemaButFailsReasoner
        {
            get => F.AsBool(F.Memo(this, "PassesSchemaButFailsReasoner", () => F.And(F.IsTrueV(F.Of(this.SchemaValidationPassed)), F.Bool3(F.Eq(F.Nullif(F.Of(this.IsConsistent)), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Snapshot { get; set; }

        private GroundingSnapshot _groundingSnapshot;

        [ForeignKey("Snapshot")]
        public virtual GroundingSnapshot GroundingSnapshot
        {
            get
            {
                if (_groundingSnapshot == null && !string.IsNullOrEmpty(Snapshot))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GroundingSnapshot - no database context is set. Snapshot: " + Snapshot + ".");
                        }
                        return null;
                    }
                    _groundingSnapshot = base.SoAContext.GroundingSnapshots.Find(Snapshot);
                    if (_groundingSnapshot != null)
                    {
                        base.SoAContext.Attach(_groundingSnapshot);
                    }
                }
                return _groundingSnapshot;
            }
            set
            {
                if (_groundingSnapshot != value)
                {
                    _groundingSnapshot = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_groundingSnapshot != null)
                    {
                        Snapshot = _groundingSnapshot.GroundingSnapshotId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GroundingSnapshot;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
