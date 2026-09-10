
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("VocabularyTerms")]
    public class VocabularyTermBase : SoAEntityBase
    {
        [Key]
        public string VocabularyTermId { get; set; }

        // Formula Name (rulebook: ={{PrefLabel}})
        public string? Name
        {
            get => this.PrefLabel; set { }
        }

        public string? PrefLabel { get; set; }
        public string? AltLabels { get; set; }
        public string? Definition { get; set; }
        // Formula UsageCount (rulebook: =COUNTIFS(Requirements!{{ControlledTerm}}, {{VocabularyTermId}}))
        public decimal? UsageCount
        {
            get => COUNTIFS(Requirements!this.ControlledTerm, this.VocabularyTermId); set { }
        }

        // Formula IsOrphanTerm (rulebook: ={{UsageCount}} = 0)
        public bool? IsOrphanTerm
        {
            get => this.UsageCount = 0; set { }
        }

        // Formula IsWidelyAdoptedTerm (rulebook: ={{UsageCount}} >= 2)
        public bool? IsWidelyAdoptedTerm
        {
            get => this.UsageCount >= 2; set { }
        }

        // Formula OrphanTermVocabularyKey (rulebook: =IF({{IsOrphanTerm}}, {{Vocabulary}}, ""))
        public string? OrphanTermVocabularyKey
        {
            get => IF(this.IsOrphanTerm, this.Vocabulary, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Vocabulary { get; set; }

        private Vocabulary _vocabulary;

        [ForeignKey("Vocabulary")]
        public virtual Vocabulary Vocabulary
        {
            get
            {
                if (_vocabulary == null && !string.IsNullOrEmpty(Vocabulary))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabulary - no database context is set. Vocabulary: " + Vocabulary + ".");
                        }
                        return null;
                    }
                    _vocabulary = Context.Vocabularies.Find(Vocabulary);
                    if (_vocabulary != null)
                    {
                        Context.Attach(_vocabulary);
                    }
                }
                return _vocabulary;
            }
            set
            {
                if (_vocabulary != value)
                {
                    _vocabulary = value;
                    Vocabulary = _vocabulary == null ? default : _vocabulary.VocabularyId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirements - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _requirements = new ObservableCollection<Requirement>();
                    }
                    else
                    {
                        var items = Context.Requirements.Where(x => x.ControlledTerm == this.VocabularyTermId).ToList<Requirement>();
                        _requirements = new ObservableCollection<Requirement>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. VocabularyTermId: " + this.VocabularyTermId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = Context.KnowledgeBrokerLinks.Where(x => x.Topic == this.VocabularyTermId).ToList<KnowledgeBrokerLink>();
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.Vocabulary;
            _ = this.Requirements;
            _ = this.KnowledgeBrokerLinks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
