
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
    [Table("SourceTermMentions")]
    public class SourceTermMentionBase : SoAEntityBase
    {
        [Key]
        public string SourceTermMentionId { get; set; }

        // Formula Name (rulebook: ={{ConceptScheme}} & ": " & {{Wording}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ConceptScheme)), F.S(": "), F.Text(F.Of(this.Wording))))); set { }
        }

        public string? Wording { get; set; }
        public string? MentionOrigin { get; set; }
        // Formula WordingKey (rulebook: ={{ConceptScheme}} & "|" & {{Wording}})
        [NotMapped]
        public string? WordingKey
        {
            get => F.AsString(F.Memo(this, "WordingKey", () => F.Concat(F.Text(F.Of(this.ConceptScheme)), F.S("|"), F.Text(F.Of(this.Wording))))); set { }
        }

        // Formula MatchingLabelCount (rulebook: =COUNTIFS(TermLabelVariants!{{WordingKey}}, {{WordingKey}}))
        [NotMapped]
        public int? MatchingLabelCount
        {
            get => F.AsInt(F.Memo(this, "MatchingLabelCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TermLabelVariant>(base.SoAContext, "TermLabelVariants", __c => __c.TermLabelVariants), __r => F.CritField(F.Of(__r.WordingKey), F.Of(this.WordingKey))))))); set { }
        }

        // Formula MatchingPrefLabelCount (rulebook: =COUNTIFS(TermLabelVariants!{{PrefWordingKey}}, {{WordingKey}}))
        [NotMapped]
        public int? MatchingPrefLabelCount
        {
            get => F.AsInt(F.Memo(this, "MatchingPrefLabelCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TermLabelVariant>(base.SoAContext, "TermLabelVariants", __c => __c.TermLabelVariants), __r => F.CritField(F.Of(__r.PrefWordingKey), F.Of(this.WordingKey))))))); set { }
        }

        // Formula IsUncontrolledWording (rulebook: ={{MatchingLabelCount}} = 0)
        [NotMapped]
        public bool? IsUncontrolledWording
        {
            get => F.AsBool(F.Memo(this, "IsUncontrolledWording", () => F.Eq(F.Of(this.MatchingLabelCount), F.I(0)))); set { }
        }

        // Formula IsNonCanonicalGeneratedValue (rulebook: =AND({{MentionOrigin}} = "AIGenerated", {{MatchingPrefLabelCount}} = 0))
        [NotMapped]
        public bool? IsNonCanonicalGeneratedValue
        {
            get => F.AsBool(F.Memo(this, "IsNonCanonicalGeneratedValue", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.MentionOrigin)), F.S("AIGenerated"))), F.Bool3(F.Eq(F.Of(this.MatchingPrefLabelCount), F.I(0)))))); set { }
        }

        // Formula IntendedTermRole (rulebook: =INDEX(VocabularyTerms!{{RepresentsRole}}, MATCH({{IntendedTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? IntendedTermRole
        {
            get => F.AsString(F.Memo(this, "IntendedTermRole", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.IntendedTerm), __r => F.Of(__r.RepresentsRole), () => F.Of(new VocabularyTerm().RepresentsRole)))); set { }
        }

        // Formula UnresolvedIntendedTermKey (rulebook: =IF({{MatchingLabelCount}} = 0, {{IntendedTerm}}, ""))
        [NotMapped]
        public string? UnresolvedIntendedTermKey
        {
            get => F.AsString(F.Memo(this, "UnresolvedIntendedTermKey", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.MatchingLabelCount), F.I(0)))) ? F.Of(this.IntendedTerm) : F.S("")))); set { }
        }

        // Formula UnresolvedRoleKey (rulebook: =IF({{MatchingLabelCount}} = 0, {{IntendedTermRole}}, ""))
        [NotMapped]
        public string? UnresolvedRoleKey
        {
            get => F.AsString(F.Memo(this, "UnresolvedRoleKey", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.MatchingLabelCount), F.I(0)))) ? F.Of(this.IntendedTermRole) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? SourceMaterial { get; set; }
        public string? AiLabelingRun { get; set; }
        public string? ConceptScheme { get; set; }
        public string? IntendedTerm { get; set; }

        private CollectedSourceMaterial _collectedSourceMaterial;

        [ForeignKey("SourceMaterial")]
        public virtual CollectedSourceMaterial CollectedSourceMaterial
        {
            get
            {
                if (_collectedSourceMaterial == null && !string.IsNullOrEmpty(SourceMaterial))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterial - no database context is set. SourceMaterial: " + SourceMaterial + ".");
                        }
                        return null;
                    }
                    _collectedSourceMaterial = base.SoAContext.CollectedSourceMaterials.Find(SourceMaterial);
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
                        SourceMaterial = _collectedSourceMaterial.CollectedSourceMaterialId;
                    }
                }
            }
        }

        private AiLabelingRun _aiLabelingRunRef;

        [ForeignKey("AiLabelingRun")]
        public virtual AiLabelingRun AiLabelingRunRef
        {
            get
            {
                if (_aiLabelingRunRef == null && !string.IsNullOrEmpty(AiLabelingRun))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiLabelingRunRef - no database context is set. AiLabelingRun: " + AiLabelingRun + ".");
                        }
                        return null;
                    }
                    _aiLabelingRunRef = base.SoAContext.AiLabelingRuns.Find(AiLabelingRun);
                    if (_aiLabelingRunRef != null)
                    {
                        base.SoAContext.Attach(_aiLabelingRunRef);
                    }
                }
                return _aiLabelingRunRef;
            }
            set
            {
                if (_aiLabelingRunRef != value)
                {
                    _aiLabelingRunRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aiLabelingRunRef != null)
                    {
                        AiLabelingRun = _aiLabelingRunRef.AiLabelingRunId;
                    }
                }
            }
        }

        private Vocabulary _vocabulary;

        [ForeignKey("ConceptScheme")]
        public virtual Vocabulary Vocabulary
        {
            get
            {
                if (_vocabulary == null && !string.IsNullOrEmpty(ConceptScheme))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabulary - no database context is set. ConceptScheme: " + ConceptScheme + ".");
                        }
                        return null;
                    }
                    _vocabulary = base.SoAContext.Vocabularies.Find(ConceptScheme);
                    if (_vocabulary != null)
                    {
                        base.SoAContext.Attach(_vocabulary);
                    }
                }
                return _vocabulary;
            }
            set
            {
                if (_vocabulary != value)
                {
                    _vocabulary = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabulary != null)
                    {
                        ConceptScheme = _vocabulary.VocabularyId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("IntendedTerm")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(IntendedTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. IntendedTerm: " + IntendedTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(IntendedTerm);
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
                        IntendedTerm = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CollectedSourceMaterial;
            _ = this.AiLabelingRunRef;
            _ = this.Vocabulary;
            _ = this.VocabularyTerm;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
