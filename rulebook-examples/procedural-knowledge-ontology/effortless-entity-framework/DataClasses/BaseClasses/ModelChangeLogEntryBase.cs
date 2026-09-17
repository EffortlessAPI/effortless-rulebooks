
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
    [Table("ModelChangeLogEntries")]
    public class ModelChangeLogEntryBase : SoAEntityBase
    {
        [Key]
        public string ModelChangeLogEntryId { get; set; }

        // Formula Name (rulebook: ={{ChangeOperation}} & ": " & LEFT({{ChangeSummary}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ChangeOperation)), F.S(": "), F.Text(F.Left(F.Of(this.ChangeSummary), F.I(50)))))); set { }
        }

        public string? ChangeLayer { get; set; }
        public string? ChangeOperation { get; set; }
        public string? ChangeSummary { get; set; }
        public string? Rationale { get; set; }
        public string? TermsAffected { get; set; }
        public bool? InvalidatesInstances { get; set; }
        public DateTimeOffset? LoggedAt { get; set; }
        public string? PriorStateCommit { get; set; }
        // Formula MotivatingQuestion (rulebook: =INDEX(ModelChangeRequests!{{MotivatingQuestion}}, MATCH({{ModelChangeRequest}}, ModelChangeRequests!{{ModelChangeRequestId}}, 0)))
        [NotMapped]
        public string? MotivatingQuestion
        {
            get => F.AsString(F.Memo(this, "MotivatingQuestion", () => F.Lookup<ModelChangeRequest>(this, "ModelChangeRequests", "ModelChangeRequestId", __c => __c.ModelChangeRequests, __r => F.Of(__r.ModelChangeRequestId), F.Of(this.ModelChangeRequest), __r => F.Of(__r.MotivatingQuestion), () => F.Of(new ModelChangeRequest().MotivatingQuestion)))); set { }
        }

        // Formula ReleaseVersion (rulebook: =INDEX(RulebookReleases!{{RulebookVersion}}, MATCH({{Release}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public string? ReleaseVersion
        {
            get => F.AsString(F.Memo(this, "ReleaseVersion", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.Release), __r => F.Of(__r.RulebookVersion), () => F.Of(new RulebookRelease().RulebookVersion)))); set { }
        }

        // Formula ReleaseDecisionUndocumented (rulebook: =INDEX(RulebookReleases!{{IsUndocumentedVersionDecision}}, MATCH({{Release}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public bool? ReleaseDecisionUndocumented
        {
            get => F.AsBool(F.Memo(this, "ReleaseDecisionUndocumented", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.Release), __r => F.Of(__r.IsUndocumentedVersionDecision), () => F.Of(new RulebookRelease().IsUndocumentedVersionDecision)))); set { }
        }

        // Formula AltersLogicalModel (rulebook: =OR({{ChangeLayer}} = "Schema", {{ChangeLayer}} = "Vocabulary"))
        [NotMapped]
        public bool? AltersLogicalModel
        {
            get => F.AsBool(F.Memo(this, "AltersLogicalModel", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Schema"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Vocabulary")))))); set { }
        }

        // Formula IsClassRemovalOrRename (rulebook: =OR({{ChangeOperation}} = "RemoveClass", {{ChangeOperation}} = "RenameClass"))
        [NotMapped]
        public bool? IsClassRemovalOrRename
        {
            get => F.AsBool(F.Memo(this, "IsClassRemovalOrRename", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("RemoveClass"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("RenameClass")))))); set { }
        }

        // Formula IsInvalidatingDomainRangeChange (rulebook: =AND({{ChangeOperation}} = "ChangeDomainRange", {{InvalidatesInstances}}))
        [NotMapped]
        public bool? IsInvalidatingDomainRangeChange
        {
            get => F.AsBool(F.Memo(this, "IsInvalidatingDomainRangeChange", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("ChangeDomainRange"))), F.IsTrueV(F.Of(this.InvalidatesInstances))))); set { }
        }

        // Formula IsInconsistentDisjointness (rulebook: =AND({{ChangeOperation}} = "AddDisjointness", {{InvalidatesInstances}}))
        [NotMapped]
        public bool? IsInconsistentDisjointness
        {
            get => F.AsBool(F.Memo(this, "IsInconsistentDisjointness", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("AddDisjointness"))), F.IsTrueV(F.Of(this.InvalidatesInstances))))); set { }
        }

        // Formula IsBackwardIncompatible (rulebook: =OR({{IsClassRemovalOrRename}}, {{InvalidatesInstances}}))
        [NotMapped]
        public bool? IsBackwardIncompatible
        {
            get => F.AsBool(F.Memo(this, "IsBackwardIncompatible", () => F.Or(F.Bool3(F.Of(this.IsClassRemovalOrRename)), F.IsTrueV(F.Of(this.InvalidatesInstances))))); set { }
        }

        // Formula IsAdditiveSchemaChange (rulebook: =AND({{ChangeLayer}} = "Schema", OR({{ChangeOperation}} = "AddClass", {{ChangeOperation}} = "AddProperty")))
        [NotMapped]
        public bool? IsAdditiveSchemaChange
        {
            get => F.AsBool(F.Memo(this, "IsAdditiveSchemaChange", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Schema"))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("AddClass"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("AddProperty")))))))); set { }
        }

        // Formula IsUnexplainedModification (rulebook: =AND(OR({{ChangeLayer}} = "Schema", {{ChangeLayer}} = "Instance"), {{Rationale}} = ""))
        [NotMapped]
        public bool? IsUnexplainedModification
        {
            get => F.AsBool(F.Memo(this, "IsUnexplainedModification", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Schema"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Instance"))))), F.Bool3(F.IsBlank(F.Of(this.Rationale)))))); set { }
        }

        // Formula RationaleWithoutQuestion (rulebook: =AND({{Rationale}} <> "", {{MotivatingQuestion}} = ""))
        [NotMapped]
        public bool? RationaleWithoutQuestion
        {
            get => F.AsBool(F.Memo(this, "RationaleWithoutQuestion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Rationale))), F.Bool3(F.IsBlank(F.Of(this.MotivatingQuestion)))))); set { }
        }

        // Formula IsSchemaChangeWithoutIncrement (rulebook: =AND({{ChangeLayer}} = "Schema", {{Release}} = ""))
        [NotMapped]
        public bool? IsSchemaChangeWithoutIncrement
        {
            get => F.AsBool(F.Memo(this, "IsSchemaChangeWithoutIncrement", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Schema"))), F.Bool3(F.IsBlank(F.Of(this.Release)))))); set { }
        }

        // Formula IsInstanceChangeInSchemaRelease (rulebook: =AND({{ChangeLayer}} = "Instance", {{Release}} <> ""))
        [NotMapped]
        public bool? IsInstanceChangeInSchemaRelease
        {
            get => F.AsBool(F.Memo(this, "IsInstanceChangeInSchemaRelease", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Instance"))), F.Bool3(F.IsNotBlank(F.Of(this.Release)))))); set { }
        }

        // Formula SchemaChangeWithoutRequest (rulebook: =AND({{ChangeLayer}} = "Schema", {{ModelChangeRequest}} = ""))
        [NotMapped]
        public bool? SchemaChangeWithoutRequest
        {
            get => F.AsBool(F.Memo(this, "SchemaChangeWithoutRequest", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Schema"))), F.Bool3(F.IsBlank(F.Of(this.ModelChangeRequest)))))); set { }
        }

        // Formula CannotBeAudited (rulebook: =OR({{ChangedByAgent}} = "", AND({{Release}} = "", {{InstanceDataVersion}} = "")))
        [NotMapped]
        public bool? CannotBeAudited
        {
            get => F.AsBool(F.Memo(this, "CannotBeAudited", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.ChangedByAgent))), F.Bool3(F.And(F.Bool3(F.IsBlank(F.Of(this.Release))), F.Bool3(F.IsBlank(F.Of(this.InstanceDataVersion)))))))); set { }
        }

        // Formula CannotBeRolledBack (rulebook: ={{PriorStateCommit}} = "")
        [NotMapped]
        public bool? CannotBeRolledBack
        {
            get => F.AsBool(F.Memo(this, "CannotBeRolledBack", () => F.IsBlank(F.Of(this.PriorStateCommit)))); set { }
        }

        // Formula IsUntraceableBreakingChange (rulebook: =AND({{IsBackwardIncompatible}}, OR({{Release}} = "", {{ReleaseDecisionUndocumented}})))
        [NotMapped]
        public bool? IsUntraceableBreakingChange
        {
            get => F.AsBool(F.Memo(this, "IsUntraceableBreakingChange", () => F.And(F.Bool3(F.Of(this.IsBackwardIncompatible)), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.Release))), F.Bool3(F.Of(this.ReleaseDecisionUndocumented))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? ModelChangeRequest { get; set; }
        public string? Release { get; set; }
        public string? InstanceDataVersion { get; set; }
        public string? AffectedTable { get; set; }
        public string? ChangedByAgent { get; set; }
        public string? RevertsEntry { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private ModelChangeRequest _modelChangeRequestRef;

        [ForeignKey("ModelChangeRequest")]
        public virtual ModelChangeRequest ModelChangeRequestRef
        {
            get
            {
                if (_modelChangeRequestRef == null && !string.IsNullOrEmpty(ModelChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeRequestRef - no database context is set. ModelChangeRequest: " + ModelChangeRequest + ".");
                        }
                        return null;
                    }
                    _modelChangeRequestRef = base.SoAContext.ModelChangeRequests.Find(ModelChangeRequest);
                    if (_modelChangeRequestRef != null)
                    {
                        base.SoAContext.Attach(_modelChangeRequestRef);
                    }
                }
                return _modelChangeRequestRef;
            }
            set
            {
                if (_modelChangeRequestRef != value)
                {
                    _modelChangeRequestRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelChangeRequestRef != null)
                    {
                        ModelChangeRequest = _modelChangeRequestRef.ModelChangeRequestId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("Release")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(Release))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. Release: " + Release + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(Release);
                    if (_rulebookRelease != null)
                    {
                        base.SoAContext.Attach(_rulebookRelease);
                    }
                }
                return _rulebookRelease;
            }
            set
            {
                if (_rulebookRelease != value)
                {
                    _rulebookRelease = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookRelease != null)
                    {
                        Release = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }

        private InstanceDataVersion _instanceDataVersionRef;

        [ForeignKey("InstanceDataVersion")]
        public virtual InstanceDataVersion InstanceDataVersionRef
        {
            get
            {
                if (_instanceDataVersionRef == null && !string.IsNullOrEmpty(InstanceDataVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InstanceDataVersionRef - no database context is set. InstanceDataVersion: " + InstanceDataVersion + ".");
                        }
                        return null;
                    }
                    _instanceDataVersionRef = base.SoAContext.InstanceDataVersions.Find(InstanceDataVersion);
                    if (_instanceDataVersionRef != null)
                    {
                        base.SoAContext.Attach(_instanceDataVersionRef);
                    }
                }
                return _instanceDataVersionRef;
            }
            set
            {
                if (_instanceDataVersionRef != value)
                {
                    _instanceDataVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_instanceDataVersionRef != null)
                    {
                        InstanceDataVersion = _instanceDataVersionRef.InstanceDataVersionId;
                    }
                }
            }
        }

        private RulebookTable _rulebookTable;

        [ForeignKey("AffectedTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(AffectedTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. AffectedTable: " + AffectedTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(AffectedTable);
                    if (_rulebookTable != null)
                    {
                        base.SoAContext.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTable != null)
                    {
                        AffectedTable = _rulebookTable.RulebookTableId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ChangedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ChangedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ChangedByAgent: " + ChangedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ChangedByAgent);
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
                        ChangedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private ModelChangeLogEntry _modelChangeLogEntry;

        [ForeignKey("RevertsEntry")]
        public virtual ModelChangeLogEntry ModelChangeLogEntry
        {
            get
            {
                if (_modelChangeLogEntry == null && !string.IsNullOrEmpty(RevertsEntry))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntry - no database context is set. RevertsEntry: " + RevertsEntry + ".");
                        }
                        return null;
                    }
                    _modelChangeLogEntry = base.SoAContext.ModelChangeLogEntries.Find(RevertsEntry);
                    if (_modelChangeLogEntry != null)
                    {
                        base.SoAContext.Attach(_modelChangeLogEntry);
                    }
                }
                return _modelChangeLogEntry;
            }
            set
            {
                if (_modelChangeLogEntry != value)
                {
                    _modelChangeLogEntry = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelChangeLogEntry != null)
                    {
                        RevertsEntry = _modelChangeLogEntry.ModelChangeLogEntryId;
                    }
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("ModelChangeLogEntry")]
        public virtual ObservableCollection<ModelChangeLogEntry> ModelChangeLogEntries
        {
            get
            {
                if (_modelChangeLogEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. ModelChangeLogEntryId: " + this.ModelChangeLogEntryId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.RevertsEntry == this.ModelChangeLogEntryId).ToList<ModelChangeLogEntry>();
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelChangeLogEntries.CollectionChanged += ModelChangeLogEntries_CollectionChanged;
                }
                return _modelChangeLogEntries;
            }
            private set
            {
                if (_modelChangeLogEntries != null)
                {
                    _modelChangeLogEntries.CollectionChanged -= ModelChangeLogEntries_CollectionChanged;
                }
                _modelChangeLogEntries = value;
                if (_modelChangeLogEntries != null)
                {
                    _modelChangeLogEntries.CollectionChanged += ModelChangeLogEntries_CollectionChanged;
                }
            }
        }

        private void ModelChangeLogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeLogEntry>())
                {
                    item.RevertsEntry = this.ModelChangeLogEntryId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.ModelChangeRequestRef;
            _ = this.RulebookRelease;
            _ = this.InstanceDataVersionRef;
            _ = this.RulebookTable;
            _ = this.Agent;
            _ = this.ModelChangeLogEntry;
            _ = this.ModelChangeLogEntries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
