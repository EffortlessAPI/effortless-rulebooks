
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
    [Table("VocabularyTerms")]
    public class VocabularyTermBase : SoAEntityBase
    {
        [Key]
        public string VocabularyTermId { get; set; }

        // Formula Name (rulebook: ={{PrefLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.PrefLabel))); set { }
        }

        public string? PrefLabel { get; set; }
        public string? AltLabels { get; set; }
        public string? Definition { get; set; }
        // Formula UsageCount (rulebook: =COUNTIFS(Requirements!{{ControlledTerm}}, {{VocabularyTermId}}))
        [NotMapped]
        public decimal? UsageCount
        {
            get => F.AsDecimal(F.Memo(this, "UsageCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Requirement>(base.SoAContext, "Requirements", __c => __c.Requirements), __r => F.CritField(F.Of(__r.ControlledTerm), F.Of(this.VocabularyTermId)))))); set { }
        }

        // Formula IsOrphanTerm (rulebook: ={{UsageCount}} = 0)
        [NotMapped]
        public bool? IsOrphanTerm
        {
            get => F.AsBool(F.Memo(this, "IsOrphanTerm", () => F.Eq(F.Of(this.UsageCount), F.I(0)))); set { }
        }

        // Formula IsWidelyAdoptedTerm (rulebook: ={{UsageCount}} >= 2)
        [NotMapped]
        public bool? IsWidelyAdoptedTerm
        {
            get => F.AsBool(F.Memo(this, "IsWidelyAdoptedTerm", () => F.Cmp(F.Of(this.UsageCount), ">=", F.I(2)))); set { }
        }

        // Formula OrphanTermVocabularyKey (rulebook: =IF({{IsOrphanTerm}}, {{Vocabulary}}, ""))
        [NotMapped]
        public string? OrphanTermVocabularyKey
        {
            get => F.AsString(F.Memo(this, "OrphanTermVocabularyKey", () => (F.Truthy(F.Bool3(F.Of(this.IsOrphanTerm))) ? F.Of(this.Vocabulary) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Vocabulary { get; set; }

        private Vocabulary _vocabularyRef;

        [ForeignKey("Vocabulary")]
        public virtual Vocabulary VocabularyRef
        {
            get
            {
                if (_vocabularyRef == null && !string.IsNullOrEmpty(Vocabulary))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyRef - no database context is set. Vocabulary: " + Vocabulary + ".");
                        }
                        return null;
                    }
                    _vocabularyRef = base.SoAContext.Vocabularies.Find(Vocabulary);
                    if (_vocabularyRef != null)
                    {
                        base.SoAContext.Attach(_vocabularyRef);
                    }
                }
                return _vocabularyRef;
            }
            set
            {
                if (_vocabularyRef != value)
                {
                    _vocabularyRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyRef != null)
                    {
                        Vocabulary = _vocabularyRef.VocabularyId;
                    }
                }
            }
        }

        private ObservableCollection<Requirement> _requirements;

        [InverseProperty("VocabularyTerm")]
        public virtual ObservableCollection<Requirement> Requirements
        {
            get
            {
                if (_requirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirements - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _requirements = new ObservableCollection<Requirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.Requirements.Where(x => x.ControlledTerm == this.VocabularyTermId).ToList<Requirement>();
                        _requirements = new ObservableCollection<Requirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requirements.CollectionChanged += Requirements_CollectionChanged;
                }
                return _requirements;
            }
            private set
            {
                if (_requirements != null)
                {
                    _requirements.CollectionChanged -= Requirements_CollectionChanged;
                }
                _requirements = value;
                if (_requirements != null)
                {
                    _requirements.CollectionChanged += Requirements_CollectionChanged;
                }
            }
        }

        private void Requirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Requirement>())
                {
                    item.ControlledTerm = this.VocabularyTermId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _knowledgeBrokerLinks;

        [InverseProperty("VocabularyTerm")]
        public virtual ObservableCollection<KnowledgeBrokerLink> KnowledgeBrokerLinks
        {
            get
            {
                if (_knowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.Topic == this.VocabularyTermId).ToList<KnowledgeBrokerLink>();
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
                return _knowledgeBrokerLinks;
            }
            private set
            {
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged -= KnowledgeBrokerLinks_CollectionChanged;
                }
                _knowledgeBrokerLinks = value;
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void KnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.Topic = this.VocabularyTermId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyRef;
            _ = this.Requirements;
            _ = this.KnowledgeBrokerLinks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
