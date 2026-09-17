
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
    [Table("AssistantBenchmarks")]
    public class AssistantBenchmarkBase : SoAEntityBase
    {
        [Key]
        public string AssistantBenchmarkId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? TaskFamily { get; set; }
        public bool? GeneratesDatabaseQueries { get; set; }
        public decimal? UngroundedAccuracyPercent { get; set; }
        public decimal? GraphGroundedAccuracyPercent { get; set; }
        public DateTimeOffset? RanAt { get; set; }
        // Formula AccuracyLiftPoints (rulebook: ={{GraphGroundedAccuracyPercent}} - {{UngroundedAccuracyPercent}})
        [NotMapped]
        public decimal? AccuracyLiftPoints
        {
            get => F.AsDecimal(F.Memo(this, "AccuracyLiftPoints", () => F.Sub(F.Of(this.GraphGroundedAccuracyPercent), F.Of(this.UngroundedAccuracyPercent)))); set { }
        }

        // Formula ShowsSpatialLiftFromGraphQueries (rulebook: =AND({{TaskFamily}} = "Spatial", {{GeneratesDatabaseQueries}}, {{AccuracyLiftPoints}} >= 20))
        [NotMapped]
        public bool? ShowsSpatialLiftFromGraphQueries
        {
            get => F.AsBool(F.Memo(this, "ShowsSpatialLiftFromGraphQueries", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TaskFamily)), F.S("Spatial"))), F.IsTrueV(F.Of(this.GeneratesDatabaseQueries)), F.Bool3(F.Cmp(F.Of(this.AccuracyLiftPoints), ">=", F.I(20)))))); set { }
        }

        // Formula ShowsGeospatialRetrievalLift (rulebook: =AND({{TaskFamily}} = "Geospatial", {{AccuracyLiftPoints}} >= 20))
        [NotMapped]
        public bool? ShowsGeospatialRetrievalLift
        {
            get => F.AsBool(F.Memo(this, "ShowsGeospatialRetrievalLift", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.TaskFamily)), F.S("Geospatial"))), F.Bool3(F.Cmp(F.Of(this.AccuracyLiftPoints), ">=", F.I(20)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Agent { get; set; }
        public string? GroundingSnapshot { get; set; }

        private Agent _agentRef;

        [ForeignKey("Agent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Agent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Agent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        Agent = _agentRef.AgentId;
                    }
                }
            }
        }

        private GroundingSnapshot _groundingSnapshotRef;

        [ForeignKey("GroundingSnapshot")]
        public virtual GroundingSnapshot GroundingSnapshotRef
        {
            get
            {
                if (_groundingSnapshotRef == null && !string.IsNullOrEmpty(GroundingSnapshot))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GroundingSnapshotRef - no database context is set. GroundingSnapshot: " + GroundingSnapshot + ".");
                        }
                        return null;
                    }
                    _groundingSnapshotRef = base.SoAContext.GroundingSnapshots.Find(GroundingSnapshot);
                    if (_groundingSnapshotRef != null)
                    {
                        base.SoAContext.Attach(_groundingSnapshotRef);
                    }
                }
                return _groundingSnapshotRef;
            }
            set
            {
                if (_groundingSnapshotRef != value)
                {
                    _groundingSnapshotRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_groundingSnapshotRef != null)
                    {
                        GroundingSnapshot = _groundingSnapshotRef.GroundingSnapshotId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AgentRef;
            _ = this.GroundingSnapshotRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
