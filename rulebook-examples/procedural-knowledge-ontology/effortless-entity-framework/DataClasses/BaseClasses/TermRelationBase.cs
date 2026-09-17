
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
    [Table("TermRelations")]
    public class TermRelationBase : SoAEntityBase
    {
        [Key]
        public string TermRelationId { get; set; }

        // Formula Name (rulebook: ={{FromTerm}} & " " & {{RelationKind}} & " " & {{ToTerm}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FromTerm)), F.S(" "), F.Text(F.Of(this.RelationKind)), F.S(" "), F.Text(F.Of(this.ToTerm))))); set { }
        }

        public string? RelationKind { get; set; }
        public string? Note { get; set; }
        // Formula FromTermGrandparent (rulebook: =INDEX(VocabularyTerms!{{BroaderTermParent}}, MATCH({{FromTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? FromTermGrandparent
        {
            get => F.AsString(F.Memo(this, "FromTermGrandparent", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.FromTerm), __r => F.Of(__r.BroaderTermParent), () => F.Of(new VocabularyTerm().BroaderTermParent)))); set { }
        }

        // Formula AssertsIndirectLinkAsDirect (rulebook: =AND({{RelationKind}} = "broader", {{ToTerm}} = {{FromTermGrandparent}}))
        [NotMapped]
        public bool? AssertsIndirectLinkAsDirect
        {
            get => F.AsBool(F.Memo(this, "AssertsIndirectLinkAsDirect", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.RelationKind)), F.S("broader"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ToTerm)), F.Of(this.FromTermGrandparent)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? FromTerm { get; set; }
        public string? ToTerm { get; set; }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("FromTerm")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(FromTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. FromTerm: " + FromTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(FromTerm);
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
                        FromTerm = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTermRef;

        [ForeignKey("ToTerm")]
        public virtual VocabularyTerm VocabularyTermRef
        {
            get
            {
                if (_vocabularyTermRef == null && !string.IsNullOrEmpty(ToTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTermRef - no database context is set. ToTerm: " + ToTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTermRef = base.SoAContext.VocabularyTerms.Find(ToTerm);
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
                        ToTerm = _vocabularyTermRef.VocabularyTermId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.VocabularyTerm;
            _ = this.VocabularyTermRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
