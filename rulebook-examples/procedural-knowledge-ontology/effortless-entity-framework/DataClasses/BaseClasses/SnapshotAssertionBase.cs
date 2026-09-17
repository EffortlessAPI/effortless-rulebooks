
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
    [Table("SnapshotAssertions")]
    public class SnapshotAssertionBase : SoAEntityBase
    {
        [Key]
        public string SnapshotAssertionId { get; set; }

        // Formula Name (rulebook: ={{SubjectIdentifier}} & " " & {{Predicate}} & " " & {{ObjectValue}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.SubjectIdentifier)), F.S(" "), F.Text(F.Of(this.Predicate)), F.S(" "), F.Text(F.Of(this.ObjectValue))))); set { }
        }

        public string? SubjectIdentifier { get; set; }
        public string? Predicate { get; set; }
        public string? ObjectValue { get; set; }
        public bool? IsInferred { get; set; }
        public string? ProvenanceUri { get; set; }
        public string? DcTitle { get; set; }
        public string? DcDescription { get; set; }
        public string? PresentedStatus { get; set; }
        // Formula SourceVersionStatus (rulebook: =INDEX(ProcedureVersions!{{Status}}, MATCH({{SourceProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? SourceVersionStatus
        {
            get => F.AsString(F.Memo(this, "SourceVersionStatus", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.SourceProcedureVersion), __r => F.Of(__r.Status), () => F.Of(new ProcedureVersion().Status)))); set { }
        }

        // Formula SourceAssignmentIsCurrent (rulebook: =INDEX(RoleAssignments!{{IsCurrent}}, MATCH({{SourceRoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public bool? SourceAssignmentIsCurrent
        {
            get => F.AsBool(F.Memo(this, "SourceAssignmentIsCurrent", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.SourceRoleAssignment), __r => F.Of(__r.IsCurrent), () => F.Of(new RoleAssignment().IsCurrent)))); set { }
        }

        // Formula SnapshotConsistentRunCount (rulebook: =INDEX(GroundingSnapshots!{{ConsistentReasonerRunCount}}, MATCH({{Snapshot}}, GroundingSnapshots!{{GroundingSnapshotId}}, 0)))
        [NotMapped]
        public int? SnapshotConsistentRunCount
        {
            get => F.AsInt(F.Memo(this, "SnapshotConsistentRunCount", () => F.Integer(F.Lookup<GroundingSnapshot>(this, "GroundingSnapshots", "GroundingSnapshotId", __c => __c.GroundingSnapshots, __r => F.Of(__r.GroundingSnapshotId), F.Of(this.Snapshot), __r => F.Of(__r.ConsistentReasonerRunCount), () => F.Of(new GroundingSnapshot().ConsistentReasonerRunCount))))); set { }
        }

        // Formula SnapshotIsReasoned (rulebook: ={{SnapshotConsistentRunCount}} > 0)
        [NotMapped]
        public bool? SnapshotIsReasoned
        {
            get => F.AsBool(F.Memo(this, "SnapshotIsReasoned", () => F.Cmp(F.Of(this.SnapshotConsistentRunCount), ">", F.I(0)))); set { }
        }

        // Formula IsStaleRoleAssertion (rulebook: =AND({{SourceRoleAssignment}} <> "", {{SourceAssignmentIsCurrent}} = FALSE))
        [NotMapped]
        public bool? IsStaleRoleAssertion
        {
            get => F.AsBool(F.Memo(this, "IsStaleRoleAssertion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SourceRoleAssignment))), F.Bool3(F.Eq(F.Of(this.SourceAssignmentIsCurrent), F.B(false)))))); set { }
        }

        // Formula PresentsDeprecatedAsCurrent (rulebook: =AND(OR({{SourceVersionStatus}} = "Deprecated", {{SourceVersionStatus}} = "Archived"), {{PresentedStatus}} = "Current"))
        [NotMapped]
        public bool? PresentsDeprecatedAsCurrent
        {
            get => F.AsBool(F.Memo(this, "PresentsDeprecatedAsCurrent", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.SourceVersionStatus), F.S("Deprecated"))), F.Bool3(F.Eq(F.Of(this.SourceVersionStatus), F.S("Archived"))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.PresentedStatus)), F.S("Current")))))); set { }
        }

        // Formula LacksProvenance (rulebook: =AND({{ProvenanceUri}} = "", {{SourceProcedureVersion}} = "", {{SourceStep}} = "", {{SourceRoleAssignment}} = ""))
        [NotMapped]
        public bool? LacksProvenance
        {
            get => F.AsBool(F.Memo(this, "LacksProvenance", () => F.And(F.Bool3(F.IsBlank(F.Of(this.ProvenanceUri))), F.Bool3(F.IsBlank(F.Of(this.SourceProcedureVersion))), F.Bool3(F.IsBlank(F.Of(this.SourceStep))), F.Bool3(F.IsBlank(F.Of(this.SourceRoleAssignment)))))); set { }
        }

        // Formula HasOpaqueIdentifier (rulebook: =OR(LEFT({{SubjectIdentifier}}, 9) = "urn:uuid:", LEFT({{SubjectIdentifier}}, 2) = "_:"))
        [NotMapped]
        public bool? HasOpaqueIdentifier
        {
            get => F.AsBool(F.Memo(this, "HasOpaqueIdentifier", () => F.Or(F.Bool3(F.Eq(F.Left(F.Of(this.SubjectIdentifier), F.I(9)), F.S("urn:uuid:"))), F.Bool3(F.Eq(F.Left(F.Of(this.SubjectIdentifier), F.I(2)), F.S("_:")))))); set { }
        }

        // Formula LacksDublinCore (rulebook: =OR({{DcTitle}} = "", {{DcDescription}} = ""))
        [NotMapped]
        public bool? LacksDublinCore
        {
            get => F.AsBool(F.Memo(this, "LacksDublinCore", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.DcTitle))), F.Bool3(F.IsBlank(F.Of(this.DcDescription)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Snapshot { get; set; }
        public string? SourceProcedureVersion { get; set; }
        public string? SourceStep { get; set; }
        public string? SourceRoleAssignment { get; set; }
        public string? AboutAgent { get; set; }

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

        private ProcedureVersion _procedureVersion;

        [ForeignKey("SourceProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(SourceProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. SourceProcedureVersion: " + SourceProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(SourceProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        SourceProcedureVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private Step _step;

        [ForeignKey("SourceStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(SourceStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. SourceStep: " + SourceStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(SourceStep);
                    if (_step != null)
                    {
                        base.SoAContext.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_step != null)
                    {
                        SourceStep = _step.StepId;
                    }
                }
            }
        }

        private RoleAssignment _roleAssignment;

        [ForeignKey("SourceRoleAssignment")]
        public virtual RoleAssignment RoleAssignment
        {
            get
            {
                if (_roleAssignment == null && !string.IsNullOrEmpty(SourceRoleAssignment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. SourceRoleAssignment: " + SourceRoleAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignment = base.SoAContext.RoleAssignments.Find(SourceRoleAssignment);
                    if (_roleAssignment != null)
                    {
                        base.SoAContext.Attach(_roleAssignment);
                    }
                }
                return _roleAssignment;
            }
            set
            {
                if (_roleAssignment != value)
                {
                    _roleAssignment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleAssignment != null)
                    {
                        SourceRoleAssignment = _roleAssignment.RoleAssignmentId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("AboutAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AboutAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AboutAgent: " + AboutAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AboutAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        AboutAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<AnswerGrounding> _answerGroundings;

        [InverseProperty("SnapshotAssertionRef")]
        public virtual ObservableCollection<AnswerGrounding> AnswerGroundings
        {
            get
            {
                if (_answerGroundings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnswerGroundings - no database context is set. SnapshotAssertionId: " + this.SnapshotAssertionId + ".");
                        }
                        _answerGroundings = new ObservableCollection<AnswerGrounding>();
                    }
                    else
                    {
                        var items = base.SoAContext.AnswerGroundings.Where(x => x.SnapshotAssertion == this.SnapshotAssertionId).ToList<AnswerGrounding>();
                        _answerGroundings = new ObservableCollection<AnswerGrounding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answerGroundings.CollectionChanged += AnswerGroundings_CollectionChanged;
                }
                return _answerGroundings;
            }
            private set
            {
                if (_answerGroundings != null)
                {
                    _answerGroundings.CollectionChanged -= AnswerGroundings_CollectionChanged;
                }
                _answerGroundings = value;
                if (_answerGroundings != null)
                {
                    _answerGroundings.CollectionChanged += AnswerGroundings_CollectionChanged;
                }
            }
        }

        private void AnswerGroundings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AnswerGrounding>())
                {
                    item.SnapshotAssertion = this.SnapshotAssertionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GroundingSnapshot;
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.RoleAssignment;
            _ = this.Agent;
            _ = this.AnswerGroundings;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
