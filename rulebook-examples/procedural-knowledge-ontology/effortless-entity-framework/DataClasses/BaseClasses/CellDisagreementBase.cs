
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
    [Table("CellDisagreements")]
    public class CellDisagreementBase : SoAEntityBase
    {
        [Key]
        public string CellDisagreementId { get; set; }

        // Formula Name (rulebook: =CONCAT({{FieldDisagreement}}, " @ ", {{RecordId}}))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FieldDisagreement)), F.S(" @ "), F.Text(F.Of(this.RecordId))))); set { }
        }

        public string? RecordId { get; set; }
        public string? ExpectedValue { get; set; }
        public string? ActualValue { get; set; }
        public string? Reason { get; set; }
        // Formula Substrate (rulebook: =INDEX(FieldDisagreements!{{Substrate}}, MATCH({{FieldDisagreement}}, FieldDisagreements!{{FieldDisagreementId}}, 0)))
        [NotMapped]
        public string? Substrate
        {
            get => F.AsString(F.Memo(this, "Substrate", () => F.Lookup<FieldDisagreement>(this, "FieldDisagreements", "FieldDisagreementId", __c => __c.FieldDisagreements, __r => F.Of(__r.FieldDisagreementId), F.Of(this.FieldDisagreement), __r => F.Of(__r.Substrate), () => F.Of(new FieldDisagreement().Substrate)))); set { }
        }

        // Formula RulebookField (rulebook: =INDEX(FieldDisagreements!{{RulebookField}}, MATCH({{FieldDisagreement}}, FieldDisagreements!{{FieldDisagreementId}}, 0)))
        [NotMapped]
        public string? RulebookField
        {
            get => F.AsString(F.Memo(this, "RulebookField", () => F.Lookup<FieldDisagreement>(this, "FieldDisagreements", "FieldDisagreementId", __c => __c.FieldDisagreements, __r => F.Of(__r.FieldDisagreementId), F.Of(this.FieldDisagreement), __r => F.Of(__r.RulebookField), () => F.Of(new FieldDisagreement().RulebookField)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? FieldDisagreement { get; set; }

        private FieldDisagreement _fieldDisagreementRef;

        [ForeignKey("FieldDisagreement")]
        public virtual FieldDisagreement FieldDisagreementRef
        {
            get
            {
                if (_fieldDisagreementRef == null && !string.IsNullOrEmpty(FieldDisagreement))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldDisagreementRef - no database context is set. FieldDisagreement: " + FieldDisagreement + ".");
                        }
                        return null;
                    }
                    _fieldDisagreementRef = base.SoAContext.FieldDisagreements.Find(FieldDisagreement);
                    if (_fieldDisagreementRef != null)
                    {
                        base.SoAContext.Attach(_fieldDisagreementRef);
                    }
                }
                return _fieldDisagreementRef;
            }
            set
            {
                if (_fieldDisagreementRef != value)
                {
                    _fieldDisagreementRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_fieldDisagreementRef != null)
                    {
                        FieldDisagreement = _fieldDisagreementRef.FieldDisagreementId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.FieldDisagreementRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
