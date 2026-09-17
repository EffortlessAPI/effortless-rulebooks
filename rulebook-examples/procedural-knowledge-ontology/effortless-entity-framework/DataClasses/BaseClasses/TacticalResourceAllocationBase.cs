
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
    [Table("TacticalResourceAllocations")]
    public class TacticalResourceAllocationBase : SoAEntityBase
    {
        [Key]
        public string TacticalResourceAllocationId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{ResourceLabel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.ResourceLabel))))); set { }
        }

        public string? ResourceLabel { get; set; }
        public string? ResourceKind { get; set; }
        public decimal? AvailableUnitsPerWeek { get; set; }
        public decimal? DemandedUnitsPerWeek { get; set; }
        public string? AdjustmentDecision { get; set; }
        // Formula UtilizationPercent (rulebook: =IF({{AvailableUnitsPerWeek}} = 0, 0, ROUND({{DemandedUnitsPerWeek}} * 100 / {{AvailableUnitsPerWeek}}, 1)))
        [NotMapped]
        public decimal? UtilizationPercent
        {
            get => F.AsDecimal(F.Memo(this, "UtilizationPercent", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.AvailableUnitsPerWeek)), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.Of(this.DemandedUnitsPerWeek), F.I(100)), F.Of(this.AvailableUnitsPerWeek)), F.I(1))))); set { }
        }

        // Formula IsBottleneck (rulebook: ={{DemandedUnitsPerWeek}} > {{AvailableUnitsPerWeek}})
        [NotMapped]
        public bool? IsBottleneck
        {
            get => F.AsBool(F.Memo(this, "IsBottleneck", () => F.Cmp(F.Nullif(F.Of(this.DemandedUnitsPerWeek)), ">", F.Nullif(F.Of(this.AvailableUnitsPerWeek))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? Facility { get; set; }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private Facility _facilityRef;

        [ForeignKey("Facility")]
        public virtual Facility FacilityRef
        {
            get
            {
                if (_facilityRef == null && !string.IsNullOrEmpty(Facility))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FacilityRef - no database context is set. Facility: " + Facility + ".");
                        }
                        return null;
                    }
                    _facilityRef = base.SoAContext.Facilities.Find(Facility);
                    if (_facilityRef != null)
                    {
                        base.SoAContext.Attach(_facilityRef);
                    }
                }
                return _facilityRef;
            }
            set
            {
                if (_facilityRef != value)
                {
                    _facilityRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_facilityRef != null)
                    {
                        Facility = _facilityRef.FacilityId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.FacilityRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
