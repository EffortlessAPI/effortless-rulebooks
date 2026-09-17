
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
    [Table("LifecycleStatuses")]
    public class LifecycleStatuseBase : SoAEntityBase
    {
        [Key]
        public string LifecycleStatusId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? StatusScheme { get; set; }
        public string? BroaderStatus { get; set; }
        public bool? IsPkoStatus { get; set; }
        public string? PkoIri { get; set; }
        // Formula VersionUseCount (rulebook: =COUNTIFS(ProcedureVersions!{{Status}}, {{LifecycleStatusId}}))
        [NotMapped]
        public int? VersionUseCount
        {
            get => F.AsInt(F.Memo(this, "VersionUseCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureVersion>(base.SoAContext, "ProcedureVersions", __c => __c.ProcedureVersions), __r => F.CritField(F.Of(__r.Status), F.Of(this.LifecycleStatusId))))))); set { }
        }

        // Formula ExecutionUseCount (rulebook: =COUNTIFS(ProcedureExecutions!{{ExecutionStatus}}, {{LifecycleStatusId}}))
        [NotMapped]
        public int? ExecutionUseCount
        {
            get => F.AsInt(F.Memo(this, "ExecutionUseCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.ExecutionStatus), F.Of(this.LifecycleStatusId))))))); set { }
        }

        // Formula IsNonPkoStatusInUse (rulebook: =AND({{IsPkoStatus}} = FALSE, ({{VersionUseCount}} + {{ExecutionUseCount}}) > 0))
        [NotMapped]
        public bool? IsNonPkoStatusInUse
        {
            get => F.AsBool(F.Memo(this, "IsNonPkoStatusInUse", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.IsPkoStatus)), F.B(false))), F.Bool3(F.Cmp(F.Add(F.Of(this.VersionUseCount), F.Of(this.ExecutionUseCount)), ">", F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? WorkflowStatusConcept { get; set; }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("WorkflowStatusConcept")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(WorkflowStatusConcept))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. WorkflowStatusConcept: " + WorkflowStatusConcept + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(WorkflowStatusConcept);
                    if (_vocabularyTerm != null)
                    {
                        base.SoAContext.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTerm != null)
                    {
                        WorkflowStatusConcept = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("LifecycleStatuse")]
        public virtual ObservableCollection<ProcedureVersion> ProcedureVersions
        {
            get
            {
                if (_procedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. LifecycleStatusId: " + this.LifecycleStatusId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.Status == this.LifecycleStatusId).ToList<ProcedureVersion>();
                        _procedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
                return _procedureVersions;
            }
            private set
            {
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged -= ProcedureVersions_CollectionChanged;
                }
                _procedureVersions = value;
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.Status = this.LifecycleStatusId;
                }
            }
        }

        private ObservableCollection<ProcedureStatusChange> _fromStatusProcedureStatusChanges;

        [InverseProperty("LifecycleStatuse")]
        public virtual ObservableCollection<ProcedureStatusChange> FromStatusProcedureStatusChanges
        {
            get
            {
                if (_fromStatusProcedureStatusChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromStatusProcedureStatusChanges - no database context is set. LifecycleStatusId: " + this.LifecycleStatusId + ".");
                        }
                        _fromStatusProcedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureStatusChanges.Where(x => x.FromStatus == this.LifecycleStatusId).ToList<ProcedureStatusChange>();
                        _fromStatusProcedureStatusChanges = new ObservableCollection<ProcedureStatusChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromStatusProcedureStatusChanges.CollectionChanged += FromStatusProcedureStatusChanges_CollectionChanged;
                }
                return _fromStatusProcedureStatusChanges;
            }
            private set
            {
                if (_fromStatusProcedureStatusChanges != null)
                {
                    _fromStatusProcedureStatusChanges.CollectionChanged -= FromStatusProcedureStatusChanges_CollectionChanged;
                }
                _fromStatusProcedureStatusChanges = value;
                if (_fromStatusProcedureStatusChanges != null)
                {
                    _fromStatusProcedureStatusChanges.CollectionChanged += FromStatusProcedureStatusChanges_CollectionChanged;
                }
            }
        }

        private void FromStatusProcedureStatusChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureStatusChange>())
                {
                    item.FromStatus = this.LifecycleStatusId;
                }
            }
        }

        private ObservableCollection<ProcedureStatusChange> _toStatusProcedureStatusChanges;

        [InverseProperty("LifecycleStatuseRef")]
        public virtual ObservableCollection<ProcedureStatusChange> ToStatusProcedureStatusChanges
        {
            get
            {
                if (_toStatusProcedureStatusChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToStatusProcedureStatusChanges - no database context is set. LifecycleStatusId: " + this.LifecycleStatusId + ".");
                        }
                        _toStatusProcedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureStatusChanges.Where(x => x.ToStatus == this.LifecycleStatusId).ToList<ProcedureStatusChange>();
                        _toStatusProcedureStatusChanges = new ObservableCollection<ProcedureStatusChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toStatusProcedureStatusChanges.CollectionChanged += ToStatusProcedureStatusChanges_CollectionChanged;
                }
                return _toStatusProcedureStatusChanges;
            }
            private set
            {
                if (_toStatusProcedureStatusChanges != null)
                {
                    _toStatusProcedureStatusChanges.CollectionChanged -= ToStatusProcedureStatusChanges_CollectionChanged;
                }
                _toStatusProcedureStatusChanges = value;
                if (_toStatusProcedureStatusChanges != null)
                {
                    _toStatusProcedureStatusChanges.CollectionChanged += ToStatusProcedureStatusChanges_CollectionChanged;
                }
            }
        }

        private void ToStatusProcedureStatusChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureStatusChange>())
                {
                    item.ToStatus = this.LifecycleStatusId;
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _procedureExecutions;

        [InverseProperty("LifecycleStatuse")]
        public virtual ObservableCollection<ProcedureExecution> ProcedureExecutions
        {
            get
            {
                if (_procedureExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. LifecycleStatusId: " + this.LifecycleStatusId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.ExecutionStatus == this.LifecycleStatusId).ToList<ProcedureExecution>();
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureExecutions.CollectionChanged += ProcedureExecutions_CollectionChanged;
                }
                return _procedureExecutions;
            }
            private set
            {
                if (_procedureExecutions != null)
                {
                    _procedureExecutions.CollectionChanged -= ProcedureExecutions_CollectionChanged;
                }
                _procedureExecutions = value;
                if (_procedureExecutions != null)
                {
                    _procedureExecutions.CollectionChanged += ProcedureExecutions_CollectionChanged;
                }
            }
        }

        private void ProcedureExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureExecution>())
                {
                    item.ExecutionStatus = this.LifecycleStatusId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyTerm;
            _ = this.ProcedureVersions;
            _ = this.FromStatusProcedureStatusChanges;
            _ = this.ToStatusProcedureStatusChanges;
            _ = this.ProcedureExecutions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
