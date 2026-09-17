
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
    [Table("GroundingSnapshots")]
    public class GroundingSnapshotBase : SoAEntityBase
    {
        [Key]
        public string GroundingSnapshotId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public DateTimeOffset? BuiltAt { get; set; }
        public DateTimeOffset? ServedFrom { get; set; }
        public DateTimeOffset? ServedTo { get; set; }
        public DateTimeOffset? GovernanceReviewedAt { get; set; }
        // Formula IsGoverned (rulebook: =AND({{StewardRole}} <> "", {{GovernanceReviewedAt}} <> "", {{GovernanceReviewedAt}} <= {{ServedFrom}}))
        [NotMapped]
        public bool? IsGoverned
        {
            get => F.AsBool(F.Memo(this, "IsGoverned", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.StewardRole))), F.Bool3(F.IsNotBlank(F.Of(this.GovernanceReviewedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.GovernanceReviewedAt)), "<=", F.Nullif(F.Of(this.ServedFrom))))))); set { }
        }

        // Formula ReasonerRunCount (rulebook: =COUNTIFS(ReasonerRuns!{{Snapshot}}, {{GroundingSnapshotId}}))
        [NotMapped]
        public int? ReasonerRunCount
        {
            get => F.AsInt(F.Memo(this, "ReasonerRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ReasonerRun>(base.SoAContext, "ReasonerRuns", __c => __c.ReasonerRuns), __r => F.CritField(F.Of(__r.Snapshot), F.Of(this.GroundingSnapshotId))))))); set { }
        }

        // Formula ConsistentReasonerRunCount (rulebook: =COUNTIFS(ReasonerRuns!{{Snapshot}}, {{GroundingSnapshotId}}, ReasonerRuns!{{IsConsistent}}, TRUE))
        [NotMapped]
        public int? ConsistentReasonerRunCount
        {
            get => F.AsInt(F.Memo(this, "ConsistentReasonerRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ReasonerRun>(base.SoAContext, "ReasonerRuns", __c => __c.ReasonerRuns), __r => F.CritField(F.Of(__r.Snapshot), F.Of(this.GroundingSnapshotId)) && F.CritLiteral(F.Of(__r.IsConsistent), F.B(true))))))); set { }
        }

        // Formula LatestMaterializedAt (rulebook: =MAXIFS(ReasonerRuns!{{MaterializedAt}}, ReasonerRuns!{{Snapshot}}, {{GroundingSnapshotId}}))
        [NotMapped]
        public DateTimeOffset? LatestMaterializedAt
        {
            get => F.AsDateTime(F.Memo(this, "LatestMaterializedAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<ReasonerRun>(base.SoAContext, "ReasonerRuns", __c => __c.ReasonerRuns), __r => F.CritField(F.Of(__r.Snapshot), F.Of(this.GroundingSnapshotId)), __r => F.Of(__r.MaterializedAt))))); set { }
        }

        // Formula ServedWithoutConsistencyCheck (rulebook: =AND({{ServedFrom}} <> "", {{ConsistentReasonerRunCount}} = 0))
        [NotMapped]
        public bool? ServedWithoutConsistencyCheck
        {
            get => F.AsBool(F.Memo(this, "ServedWithoutConsistencyCheck", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ServedFrom))), F.Bool3(F.Eq(F.Of(this.ConsistentReasonerRunCount), F.I(0)))))); set { }
        }

        // Formula ServedBeforeMaterialization (rulebook: =AND({{ServedFrom}} <> "", OR({{LatestMaterializedAt}} = "", {{LatestMaterializedAt}} > {{ServedFrom}})))
        [NotMapped]
        public bool? ServedBeforeMaterialization
        {
            get => F.AsBool(F.Memo(this, "ServedBeforeMaterialization", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ServedFrom))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.LatestMaterializedAt))), F.Bool3(F.Cmp(F.Of(this.LatestMaterializedAt), ">", F.Nullif(F.Of(this.ServedFrom))))))))); set { }
        }

        // Formula AssertionCount (rulebook: =COUNTIFS(SnapshotAssertions!{{Snapshot}}, {{GroundingSnapshotId}}))
        [NotMapped]
        public int? AssertionCount
        {
            get => F.AsInt(F.Memo(this, "AssertionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.Snapshot), F.Of(this.GroundingSnapshotId))))))); set { }
        }

        // Formula StaleAssignmentAssertionCount (rulebook: =COUNTIFS(SnapshotAssertions!{{Snapshot}}, {{GroundingSnapshotId}}, SnapshotAssertions!{{IsStaleRoleAssertion}}, TRUE))
        [NotMapped]
        public int? StaleAssignmentAssertionCount
        {
            get => F.AsInt(F.Memo(this, "StaleAssignmentAssertionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.Snapshot), F.Of(this.GroundingSnapshotId)) && F.CritLiteral(F.Of(__r.IsStaleRoleAssertion), F.B(true))))))); set { }
        }

        // Formula ServesStaleRoleAssignments (rulebook: ={{StaleAssignmentAssertionCount}} > 0)
        [NotMapped]
        public bool? ServesStaleRoleAssignments
        {
            get => F.AsBool(F.Memo(this, "ServesStaleRoleAssignments", () => F.Cmp(F.Of(this.StaleAssignmentAssertionCount), ">", F.I(0)))); set { }
        }

        // Formula DeprecatedAsCurrentCount (rulebook: =COUNTIFS(SnapshotAssertions!{{Snapshot}}, {{GroundingSnapshotId}}, SnapshotAssertions!{{PresentsDeprecatedAsCurrent}}, TRUE))
        [NotMapped]
        public int? DeprecatedAsCurrentCount
        {
            get => F.AsInt(F.Memo(this, "DeprecatedAsCurrentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.Snapshot), F.Of(this.GroundingSnapshotId)) && F.CritLiteral(F.Of(__r.PresentsDeprecatedAsCurrent), F.B(true))))))); set { }
        }

        // Formula ServesDeprecatedAsCurrent (rulebook: ={{DeprecatedAsCurrentCount}} > 0)
        [NotMapped]
        public bool? ServesDeprecatedAsCurrent
        {
            get => F.AsBool(F.Memo(this, "ServesDeprecatedAsCurrent", () => F.Cmp(F.Of(this.DeprecatedAsCurrentCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StewardRole { get; set; }

        private Role _role;

        [ForeignKey("StewardRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(StewardRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. StewardRole: " + StewardRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(StewardRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        StewardRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<MethodApplication> _methodApplications;

        [InverseProperty("GroundingSnapshot")]
        public virtual ObservableCollection<MethodApplication> MethodApplications
        {
            get
            {
                if (_methodApplications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MethodApplications - no database context is set. GroundingSnapshotId: " + this.GroundingSnapshotId + ".");
                        }
                        _methodApplications = new ObservableCollection<MethodApplication>();
                    }
                    else
                    {
                        var items = base.SoAContext.MethodApplications.Where(x => x.AppliedToGroundingSnapshot == this.GroundingSnapshotId).ToList<MethodApplication>();
                        _methodApplications = new ObservableCollection<MethodApplication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _methodApplications.CollectionChanged += MethodApplications_CollectionChanged;
                }
                return _methodApplications;
            }
            private set
            {
                if (_methodApplications != null)
                {
                    _methodApplications.CollectionChanged -= MethodApplications_CollectionChanged;
                }
                _methodApplications = value;
                if (_methodApplications != null)
                {
                    _methodApplications.CollectionChanged += MethodApplications_CollectionChanged;
                }
            }
        }

        private void MethodApplications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MethodApplication>())
                {
                    item.AppliedToGroundingSnapshot = this.GroundingSnapshotId;
                }
            }
        }

        private ObservableCollection<AgentIntegration> _agentIntegrations;

        [InverseProperty("GroundingSnapshot")]
        public virtual ObservableCollection<AgentIntegration> AgentIntegrations
        {
            get
            {
                if (_agentIntegrations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentIntegrations - no database context is set. GroundingSnapshotId: " + this.GroundingSnapshotId + ".");
                        }
                        _agentIntegrations = new ObservableCollection<AgentIntegration>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentIntegrations.Where(x => x.ServesSnapshot == this.GroundingSnapshotId).ToList<AgentIntegration>();
                        _agentIntegrations = new ObservableCollection<AgentIntegration>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _agentIntegrations.CollectionChanged += AgentIntegrations_CollectionChanged;
                }
                return _agentIntegrations;
            }
            private set
            {
                if (_agentIntegrations != null)
                {
                    _agentIntegrations.CollectionChanged -= AgentIntegrations_CollectionChanged;
                }
                _agentIntegrations = value;
                if (_agentIntegrations != null)
                {
                    _agentIntegrations.CollectionChanged += AgentIntegrations_CollectionChanged;
                }
            }
        }

        private void AgentIntegrations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentIntegration>())
                {
                    item.ServesSnapshot = this.GroundingSnapshotId;
                }
            }
        }

        private ObservableCollection<ReasonerRun> _reasonerRuns;

        [InverseProperty("GroundingSnapshot")]
        public virtual ObservableCollection<ReasonerRun> ReasonerRuns
        {
            get
            {
                if (_reasonerRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReasonerRuns - no database context is set. GroundingSnapshotId: " + this.GroundingSnapshotId + ".");
                        }
                        _reasonerRuns = new ObservableCollection<ReasonerRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.ReasonerRuns.Where(x => x.Snapshot == this.GroundingSnapshotId).ToList<ReasonerRun>();
                        _reasonerRuns = new ObservableCollection<ReasonerRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _reasonerRuns.CollectionChanged += ReasonerRuns_CollectionChanged;
                }
                return _reasonerRuns;
            }
            private set
            {
                if (_reasonerRuns != null)
                {
                    _reasonerRuns.CollectionChanged -= ReasonerRuns_CollectionChanged;
                }
                _reasonerRuns = value;
                if (_reasonerRuns != null)
                {
                    _reasonerRuns.CollectionChanged += ReasonerRuns_CollectionChanged;
                }
            }
        }

        private void ReasonerRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ReasonerRun>())
                {
                    item.Snapshot = this.GroundingSnapshotId;
                }
            }
        }

        private ObservableCollection<SnapshotAssertion> _snapshotAssertions;

        [InverseProperty("GroundingSnapshot")]
        public virtual ObservableCollection<SnapshotAssertion> SnapshotAssertions
        {
            get
            {
                if (_snapshotAssertions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SnapshotAssertions - no database context is set. GroundingSnapshotId: " + this.GroundingSnapshotId + ".");
                        }
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>();
                    }
                    else
                    {
                        var items = base.SoAContext.SnapshotAssertions.Where(x => x.Snapshot == this.GroundingSnapshotId).ToList<SnapshotAssertion>();
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
                return _snapshotAssertions;
            }
            private set
            {
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged -= SnapshotAssertions_CollectionChanged;
                }
                _snapshotAssertions = value;
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
            }
        }

        private void SnapshotAssertions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SnapshotAssertion>())
                {
                    item.Snapshot = this.GroundingSnapshotId;
                }
            }
        }

        private ObservableCollection<AssistantBenchmark> _assistantBenchmarks;

        [InverseProperty("GroundingSnapshotRef")]
        public virtual ObservableCollection<AssistantBenchmark> AssistantBenchmarks
        {
            get
            {
                if (_assistantBenchmarks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssistantBenchmarks - no database context is set. GroundingSnapshotId: " + this.GroundingSnapshotId + ".");
                        }
                        _assistantBenchmarks = new ObservableCollection<AssistantBenchmark>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantBenchmarks.Where(x => x.GroundingSnapshot == this.GroundingSnapshotId).ToList<AssistantBenchmark>();
                        _assistantBenchmarks = new ObservableCollection<AssistantBenchmark>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assistantBenchmarks.CollectionChanged += AssistantBenchmarks_CollectionChanged;
                }
                return _assistantBenchmarks;
            }
            private set
            {
                if (_assistantBenchmarks != null)
                {
                    _assistantBenchmarks.CollectionChanged -= AssistantBenchmarks_CollectionChanged;
                }
                _assistantBenchmarks = value;
                if (_assistantBenchmarks != null)
                {
                    _assistantBenchmarks.CollectionChanged += AssistantBenchmarks_CollectionChanged;
                }
            }
        }

        private void AssistantBenchmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantBenchmark>())
                {
                    item.GroundingSnapshot = this.GroundingSnapshotId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.MethodApplications;
            _ = this.AgentIntegrations;
            _ = this.ReasonerRuns;
            _ = this.SnapshotAssertions;
            _ = this.AssistantBenchmarks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
