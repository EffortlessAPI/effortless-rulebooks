
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
    [Table("KnowledgeTestOutcomes")]
    public class KnowledgeTestOutcomeBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeTestOutcomeId { get; set; }

        // Formula Name (rulebook: ={{Procedure}} & ": " & {{OutcomeArea}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Procedure)), F.S(": "), F.Text(F.Of(this.OutcomeArea))))); set { }
        }

        public string? OutcomeArea { get; set; }
        public string? Measure { get; set; }
        public decimal? BaselineValue { get; set; }
        public decimal? AfterTestValue { get; set; }
        public bool? HigherIsBetter { get; set; }
        // Formula IsImproved (rulebook: =OR(AND({{HigherIsBetter}}, {{AfterTestValue}} > {{BaselineValue}}), AND(NOT({{HigherIsBetter}}), {{AfterTestValue}} < {{BaselineValue}})))
        [NotMapped]
        public bool? IsImproved
        {
            get => F.AsBool(F.Memo(this, "IsImproved", () => F.Or(F.Bool3(F.And(F.IsTrueV(F.Of(this.HigherIsBetter)), F.Bool3(F.Cmp(F.Nullif(F.Of(this.AfterTestValue)), ">", F.Nullif(F.Of(this.BaselineValue)))))), F.Bool3(F.And(F.Bool3(F.Not(F.IsTrueV(F.Of(this.HigherIsBetter)))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.AfterTestValue)), "<", F.Nullif(F.Of(this.BaselineValue))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
