
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
    [Table("ProcessOutcomeMeasures")]
    public class ProcessOutcomeMeasureBase : SoAEntityBase
    {
        [Key]
        public string ProcessOutcomeMeasureId { get; set; }

        // Formula Name (rulebook: ={{Procedure}} & ": " & {{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Procedure)), F.S(": "), F.Text(F.Of(this.Label))))); set { }
        }

        public string? Label { get; set; }
        public string? Unit { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ObservedValue { get; set; }
        public string? MeasuredOverPeriod { get; set; }
        public string? LinkRationale { get; set; }
        // Formula IsUnlinkedToBusinessOutcome (rulebook: ={{BusinessOutcome}} = "")
        [NotMapped]
        public bool? IsUnlinkedToBusinessOutcome
        {
            get => F.AsBool(F.Memo(this, "IsUnlinkedToBusinessOutcome", () => F.IsBlank(F.Of(this.BusinessOutcome)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? BusinessOutcome { get; set; }

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

        private BusinessOutcome _businessOutcomeRef;

        [ForeignKey("BusinessOutcome")]
        public virtual BusinessOutcome BusinessOutcomeRef
        {
            get
            {
                if (_businessOutcomeRef == null && !string.IsNullOrEmpty(BusinessOutcome))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BusinessOutcomeRef - no database context is set. BusinessOutcome: " + BusinessOutcome + ".");
                        }
                        return null;
                    }
                    _businessOutcomeRef = base.SoAContext.BusinessOutcomes.Find(BusinessOutcome);
                    if (_businessOutcomeRef != null)
                    {
                        base.SoAContext.Attach(_businessOutcomeRef);
                    }
                }
                return _businessOutcomeRef;
            }
            set
            {
                if (_businessOutcomeRef != value)
                {
                    _businessOutcomeRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_businessOutcomeRef != null)
                    {
                        BusinessOutcome = _businessOutcomeRef.BusinessOutcomeId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.BusinessOutcomeRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
