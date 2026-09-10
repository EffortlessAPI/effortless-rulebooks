
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Vocabularies")]
    public class VocabularyBase : SoAEntityBase
    {
        [Key]
        public string VocabularyId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        public string? Name
        {
            get => this.Title; set { }
        }

        public string? Title { get; set; }
        public string? SchemeUri { get; set; }
        // Formula TermCount (rulebook: =COUNTIFS(VocabularyTerms!{{Vocabulary}}, {{VocabularyId}}))
        public decimal? TermCount
        {
            get => COUNTIFS(VocabularyTerms!this.Vocabulary, this.VocabularyId); set { }
        }

        // Formula OrphanTermCount (rulebook: =COUNTIFS(VocabularyTerms!{{OrphanTermVocabularyKey}}, {{VocabularyId}}))
        public decimal? OrphanTermCount
        {
            get => COUNTIFS(VocabularyTerms!this.OrphanTermVocabularyKey, this.VocabularyId); set { }
        }

        // Formula HasOrphanTerms (rulebook: ={{OrphanTermCount}} > 0)
        public bool? HasOrphanTerms
        {
            get => this.OrphanTermCount > 0; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GoverningRole { get; set; }

        private Role _role;

        [ForeignKey("GoverningRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(GoverningRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. GoverningRole: " + GoverningRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(GoverningRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    GoverningRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private ObservableCollection<VocabularyTerm> _vocabularyTerms;

        [InverseProperty("Vocabulary")]
        public virtual ObservableCollection<VocabularyTerm> VocabularyTerms
        {
            get
            {
                if (_vocabularyTerms == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerms - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>();
                    }
                    else
                    {
                        var items = Context.VocabularyTerms.Where(x => x.Vocabulary == this.VocabularyId).ToList<VocabularyTerm>();
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _vocabularyTerms.CollectionChanged += VocabularyTerms_CollectionChanged;
                }
                return _vocabularyTerms;
            }
            private set
            {
                if (_vocabularyTerms != null)
                {
                    _vocabularyTerms.CollectionChanged -= VocabularyTerms_CollectionChanged;
                }
                _vocabularyTerms = value;
                if (_vocabularyTerms != null)
                {
                    _vocabularyTerms.CollectionChanged += VocabularyTerms_CollectionChanged;
                }
            }
        }

        private void VocabularyTerms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<VocabularyTerm>())
                {
                    item.Vocabulary = this.VocabularyId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.VocabularyTerms;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
