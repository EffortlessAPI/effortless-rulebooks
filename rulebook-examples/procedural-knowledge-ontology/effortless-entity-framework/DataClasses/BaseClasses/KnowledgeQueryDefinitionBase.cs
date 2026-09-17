
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
    [Table("KnowledgeQueryDefinitions")]
    public class KnowledgeQueryDefinitionBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeQueryDefinitionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? QueryLanguage { get; set; }
        public string? QueryText { get; set; }
        public bool? NeedsOntologyAndInstances { get; set; }
        public bool? TraversesOntologyLayer { get; set; }
        public bool? TraversesInstanceLayer { get; set; }
        public bool? RanOverConsolidatedCopy { get; set; }
        public DateTimeOffset? LastRunAt { get; set; }
        // Formula SourceSystemCount (rulebook: =COUNTIFS(KnowledgeQuerySources!{{QueryDefinition}}, {{KnowledgeQueryDefinitionId}}))
        [NotMapped]
        public int? SourceSystemCount
        {
            get => F.AsInt(F.Memo(this, "SourceSystemCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeQuerySource>(base.SoAContext, "KnowledgeQuerySources", __c => __c.KnowledgeQuerySources), __r => F.CritField(F.Of(__r.QueryDefinition), F.Of(this.KnowledgeQueryDefinitionId))))))); set { }
        }

        // Formula MissesALayer (rulebook: =AND({{NeedsOntologyAndInstances}}, OR({{TraversesOntologyLayer}} = FALSE, {{TraversesInstanceLayer}} = FALSE)))
        [NotMapped]
        public bool? MissesALayer
        {
            get => F.AsBool(F.Memo(this, "MissesALayer", () => F.And(F.IsTrueV(F.Of(this.NeedsOntologyAndInstances)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.TraversesOntologyLayer)), F.B(false))), F.Bool3(F.Eq(F.Nullif(F.Of(this.TraversesInstanceLayer)), F.B(false)))))))); set { }
        }

        // Formula ConsolidatedDistributedSources (rulebook: =AND({{SourceSystemCount}} > 1, {{RanOverConsolidatedCopy}}))
        [NotMapped]
        public bool? ConsolidatedDistributedSources
        {
            get => F.AsBool(F.Memo(this, "ConsolidatedDistributedSources", () => F.And(F.Bool3(F.Cmp(F.Of(this.SourceSystemCount), ">", F.I(1))), F.IsTrueV(F.Of(this.RanOverConsolidatedCopy))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? TargetProcedureVersion { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("TargetProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(TargetProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. TargetProcedureVersion: " + TargetProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(TargetProcedureVersion);
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
                        TargetProcedureVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private ObservableCollection<KnowledgeQuerySource> _knowledgeQuerySources;

        [InverseProperty("KnowledgeQueryDefinition")]
        public virtual ObservableCollection<KnowledgeQuerySource> KnowledgeQuerySources
        {
            get
            {
                if (_knowledgeQuerySources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeQuerySources - no database context is set. KnowledgeQueryDefinitionId: " + this.KnowledgeQueryDefinitionId + ".");
                        }
                        _knowledgeQuerySources = new ObservableCollection<KnowledgeQuerySource>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeQuerySources.Where(x => x.QueryDefinition == this.KnowledgeQueryDefinitionId).ToList<KnowledgeQuerySource>();
                        _knowledgeQuerySources = new ObservableCollection<KnowledgeQuerySource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeQuerySources.CollectionChanged += KnowledgeQuerySources_CollectionChanged;
                }
                return _knowledgeQuerySources;
            }
            private set
            {
                if (_knowledgeQuerySources != null)
                {
                    _knowledgeQuerySources.CollectionChanged -= KnowledgeQuerySources_CollectionChanged;
                }
                _knowledgeQuerySources = value;
                if (_knowledgeQuerySources != null)
                {
                    _knowledgeQuerySources.CollectionChanged += KnowledgeQuerySources_CollectionChanged;
                }
            }
        }

        private void KnowledgeQuerySources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeQuerySource>())
                {
                    item.QueryDefinition = this.KnowledgeQueryDefinitionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.KnowledgeQuerySources;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
