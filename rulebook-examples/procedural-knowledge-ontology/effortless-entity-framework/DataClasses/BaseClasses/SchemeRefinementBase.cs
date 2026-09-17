
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
    [Table("SchemeRefinements")]
    public class SchemeRefinementBase : SoAEntityBase
    {
        [Key]
        public string SchemeRefinementId { get; set; }

        // Formula Name (rulebook: ={{Vocabulary}} & " refined " & {{RefinedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Vocabulary)), F.S(" refined "), F.DatetimeText(F.Of(this.RefinedAt))))); set { }
        }

        public DateTimeOffset? RefinedAt { get; set; }
        public string? ChangeSummary { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? Vocabulary { get; set; }
        public string? TriggeredByMaterial { get; set; }

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

        private CollectedSourceMaterial _collectedSourceMaterial;

        [ForeignKey("TriggeredByMaterial")]
        public virtual CollectedSourceMaterial CollectedSourceMaterial
        {
            get
            {
                if (_collectedSourceMaterial == null && !string.IsNullOrEmpty(TriggeredByMaterial))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterial - no database context is set. TriggeredByMaterial: " + TriggeredByMaterial + ".");
                        }
                        return null;
                    }
                    _collectedSourceMaterial = base.SoAContext.CollectedSourceMaterials.Find(TriggeredByMaterial);
                    if (_collectedSourceMaterial != null)
                    {
                        base.SoAContext.Attach(_collectedSourceMaterial);
                    }
                }
                return _collectedSourceMaterial;
            }
            set
            {
                if (_collectedSourceMaterial != value)
                {
                    _collectedSourceMaterial = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_collectedSourceMaterial != null)
                    {
                        TriggeredByMaterial = _collectedSourceMaterial.CollectedSourceMaterialId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyRef;
            _ = this.CollectedSourceMaterial;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
