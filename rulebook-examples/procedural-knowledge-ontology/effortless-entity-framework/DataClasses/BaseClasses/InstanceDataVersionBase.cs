
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
    [Table("InstanceDataVersions")]
    public class InstanceDataVersionBase : SoAEntityBase
    {
        [Key]
        public string InstanceDataVersionId { get; set; }

        // Formula Name (rulebook: ={{DataVersionLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.DataVersionLabel))); set { }
        }

        public string? DataVersionLabel { get; set; }
        public DateTimeOffset? SnapshotAt { get; set; }
        // Formula LoggedChangeCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{InstanceDataVersion}}, {{InstanceDataVersionId}}))
        [NotMapped]
        public int? LoggedChangeCount
        {
            get => F.AsInt(F.Memo(this, "LoggedChangeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.InstanceDataVersion), F.Of(this.InstanceDataVersionId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? ConformsToRelease { get; set; }

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

        private RulebookRelease _rulebookRelease;

        [ForeignKey("ConformsToRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(ConformsToRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. ConformsToRelease: " + ConformsToRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(ConformsToRelease);
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
                        ConformsToRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }

        private ObservableCollection<ModelProposal> _modelProposals;

        [InverseProperty("InstanceDataVersion")]
        public virtual ObservableCollection<ModelProposal> ModelProposals
        {
            get
            {
                if (_modelProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelProposals - no database context is set. InstanceDataVersionId: " + this.InstanceDataVersionId + ".");
                        }
                        _modelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.AdoptedInDataVersion == this.InstanceDataVersionId).ToList<ModelProposal>();
                        _modelProposals = new ObservableCollection<ModelProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelProposals.CollectionChanged += ModelProposals_CollectionChanged;
                }
                return _modelProposals;
            }
            private set
            {
                if (_modelProposals != null)
                {
                    _modelProposals.CollectionChanged -= ModelProposals_CollectionChanged;
                }
                _modelProposals = value;
                if (_modelProposals != null)
                {
                    _modelProposals.CollectionChanged += ModelProposals_CollectionChanged;
                }
            }
        }

        private void ModelProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelProposal>())
                {
                    item.AdoptedInDataVersion = this.InstanceDataVersionId;
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("InstanceDataVersionRef")]
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
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. InstanceDataVersionId: " + this.InstanceDataVersionId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.InstanceDataVersion == this.InstanceDataVersionId).ToList<ModelChangeLogEntry>();
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
                    item.InstanceDataVersion = this.InstanceDataVersionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.RulebookRelease;
            _ = this.ModelProposals;
            _ = this.ModelChangeLogEntries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
