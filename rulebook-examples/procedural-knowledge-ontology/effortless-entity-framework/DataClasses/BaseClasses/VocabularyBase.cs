
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
    [Table("Vocabularies")]
    public class VocabularyBase : SoAEntityBase
    {
        [Key]
        public string VocabularyId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? SchemeUri { get; set; }
        // Formula TermCount (rulebook: =COUNTIFS(VocabularyTerms!{{Vocabulary}}, {{VocabularyId}}))
        [NotMapped]
        public decimal? TermCount
        {
            get => F.AsDecimal(F.Memo(this, "TermCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VocabularyTerm>(base.SoAContext, "VocabularyTerms", __c => __c.VocabularyTerms), __r => F.CritField(F.Of(__r.Vocabulary), F.Of(this.VocabularyId)))))); set { }
        }

        // Formula OrphanTermCount (rulebook: =COUNTIFS(VocabularyTerms!{{OrphanTermVocabularyKey}}, {{VocabularyId}}))
        [NotMapped]
        public decimal? OrphanTermCount
        {
            get => F.AsDecimal(F.Memo(this, "OrphanTermCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<VocabularyTerm>(base.SoAContext, "VocabularyTerms", __c => __c.VocabularyTerms), __r => F.CritField(F.Of(__r.OrphanTermVocabularyKey), F.Of(this.VocabularyId)))))); set { }
        }

        // Formula HasOrphanTerms (rulebook: ={{OrphanTermCount}} > 0)
        [NotMapped]
        public bool? HasOrphanTerms
        {
            get => F.AsBool(F.Memo(this, "HasOrphanTerms", () => F.Cmp(F.Of(this.OrphanTermCount), ">", F.I(0)))); set { }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. GoverningRole: " + GoverningRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(GoverningRole);
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
                        GoverningRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<VocabularyTerm> _vocabularyTerms;

        [InverseProperty("VocabularyRef")]
        public virtual ObservableCollection<VocabularyTerm> VocabularyTerms
        {
            get
            {
                if (_vocabularyTerms == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerms - no database context is set. VocabularyId: " + this.VocabularyId + ".");
                        }
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.VocabularyTerms.Where(x => x.Vocabulary == this.VocabularyId).ToList<VocabularyTerm>();
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
