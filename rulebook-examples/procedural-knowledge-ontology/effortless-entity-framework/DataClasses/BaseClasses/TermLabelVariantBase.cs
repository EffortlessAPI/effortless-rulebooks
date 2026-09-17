
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
    [Table("TermLabelVariants")]
    public class TermLabelVariantBase : SoAEntityBase
    {
        [Key]
        public string TermLabelVariantId { get; set; }

        // Formula Name (rulebook: ={{VocabularyTerm}} & " " & {{LabelKind}} & ": " & {{Wording}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.VocabularyTerm)), F.S(" "), F.Text(F.Of(this.LabelKind)), F.S(": "), F.Text(F.Of(this.Wording))))); set { }
        }

        public string? LabelKind { get; set; }
        public string? Wording { get; set; }
        // Formula TermScheme (rulebook: =INDEX(VocabularyTerms!{{Vocabulary}}, MATCH({{VocabularyTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? TermScheme
        {
            get => F.AsString(F.Memo(this, "TermScheme", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.VocabularyTerm), __r => F.Of(__r.Vocabulary), () => F.Of(new VocabularyTerm().Vocabulary)))); set { }
        }

        // Formula WordingKey (rulebook: ={{TermScheme}} & "|" & {{Wording}})
        [NotMapped]
        public string? WordingKey
        {
            get => F.AsString(F.Memo(this, "WordingKey", () => F.Concat(F.Text(F.Of(this.TermScheme)), F.S("|"), F.Text(F.Of(this.Wording))))); set { }
        }

        // Formula PrefWordingKey (rulebook: =IF({{LabelKind}} = "pref", {{WordingKey}}, ""))
        [NotMapped]
        public string? PrefWordingKey
        {
            get => F.AsString(F.Memo(this, "PrefWordingKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.LabelKind)), F.S("pref")))) ? F.Of(this.WordingKey) : F.S("")))); set { }
        }

        // Formula ConceptsSharingWording (rulebook: =COUNTIFS(TermLabelVariants!{{WordingKey}}, {{WordingKey}}))
        [NotMapped]
        public int? ConceptsSharingWording
        {
            get => F.AsInt(F.Memo(this, "ConceptsSharingWording", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TermLabelVariant>(base.SoAContext, "TermLabelVariants", __c => __c.TermLabelVariants), __r => F.CritField(F.Of(__r.WordingKey), F.Of(this.WordingKey))))))); set { }
        }

        // Formula IsAmbiguousLabel (rulebook: ={{ConceptsSharingWording}} > 1)
        [NotMapped]
        public bool? IsAmbiguousLabel
        {
            get => F.AsBool(F.Memo(this, "IsAmbiguousLabel", () => F.Cmp(F.Of(this.ConceptsSharingWording), ">", F.I(1)))); set { }
        }

        // Formula PractitionerMentionCount (rulebook: =COUNTIFS(SourceTermMentions!{{WordingKey}}, {{WordingKey}}, SourceTermMentions!{{MentionOrigin}}, "Practitioner"))
        [NotMapped]
        public int? PractitionerMentionCount
        {
            get => F.AsInt(F.Memo(this, "PractitionerMentionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceTermMention>(base.SoAContext, "SourceTermMentions", __c => __c.SourceTermMentions), __r => F.CritField(F.Of(__r.WordingKey), F.Of(this.WordingKey)) && F.CritLiteral(F.Of(__r.MentionOrigin), F.S("Practitioner"))))))); set { }
        }

        // Formula PrefWording (rulebook: =IF({{LabelKind}} = "pref", {{Wording}}, ""))
        [NotMapped]
        public string? PrefWording
        {
            get => F.AsString(F.Memo(this, "PrefWording", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.LabelKind)), F.S("pref")))) ? F.Of(this.Wording) : F.S("")))); set { }
        }

        // Formula SamePrefWordingCount (rulebook: =COUNTIFS(TermLabelVariants!{{PrefWording}}, {{PrefWording}}))
        [NotMapped]
        public int? SamePrefWordingCount
        {
            get => F.AsInt(F.Memo(this, "SamePrefWordingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TermLabelVariant>(base.SoAContext, "TermLabelVariants", __c => __c.TermLabelVariants), __r => F.CritField(F.Of(__r.PrefWording), F.Of(this.PrefWording))))))); set { }
        }

        // Formula IsCrossSchemeDuplicatePref (rulebook: =AND({{LabelKind}} = "pref", {{SamePrefWordingCount}} > 1))
        [NotMapped]
        public bool? IsCrossSchemeDuplicatePref
        {
            get => F.AsBool(F.Memo(this, "IsCrossSchemeDuplicatePref", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.LabelKind)), F.S("pref"))), F.Bool3(F.Cmp(F.Of(this.SamePrefWordingCount), ">", F.I(1)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? VocabularyTerm { get; set; }

        private VocabularyTerm _vocabularyTermRef;

        [ForeignKey("VocabularyTerm")]
        public virtual VocabularyTerm VocabularyTermRef
        {
            get
            {
                if (_vocabularyTermRef == null && !string.IsNullOrEmpty(VocabularyTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTermRef - no database context is set. VocabularyTerm: " + VocabularyTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTermRef = base.SoAContext.VocabularyTerms.Find(VocabularyTerm);
                    if (_vocabularyTermRef != null)
                    {
                        base.SoAContext.Attach(_vocabularyTermRef);
                    }
                }
                return _vocabularyTermRef;
            }
            set
            {
                if (_vocabularyTermRef != value)
                {
                    _vocabularyTermRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTermRef != null)
                    {
                        VocabularyTerm = _vocabularyTermRef.VocabularyTermId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyTermRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
