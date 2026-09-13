
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
    [Table("BindingObservations")]
    public class BindingObservationBase : SoAEntityBase
    {
        [Key]
        public string BindingObservationId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{BindingObservationId}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.StepExecution)), F.S(" / "), F.TextOr(F.Of(this.BindingObservationId))))); set { }
        }

        public DateTimeOffset? ObservedSourceTimestamp { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        // Formula SlaMinutesAtRun (rulebook: =INDEX(OperationalBindings!{{FreshnessSlaMinutes}}, MATCH({{OperationalBinding}}, OperationalBindings!{{OperationalBindingId}}, 0)))
        [NotMapped]
        public int? SlaMinutesAtRun
        {
            get => F.AsInt(F.Memo(this, "SlaMinutesAtRun", () => F.Integer(F.Lookup<OperationalBinding>(this, "OperationalBindings", "OperationalBindingId", __c => __c.OperationalBindings, __r => F.Of(__r.OperationalBindingId), F.Of(this.OperationalBinding), __r => F.Of(__r.FreshnessSlaMinutes), () => F.Of(new OperationalBinding().FreshnessSlaMinutes))))); set { }
        }

        // Formula AgeAtRunMinutes (rulebook: =DATETIME_DIFF({{ReadAt}}, {{ObservedSourceTimestamp}}, "minutes"))
        [NotMapped]
        public int? AgeAtRunMinutes
        {
            get => F.AsInt(F.Memo(this, "AgeAtRunMinutes", () => F.Integer(F.DatetimeDiff(F.Of(this.ReadAt), F.Of(this.ObservedSourceTimestamp), F.S("minutes"))))); set { }
        }

        // Formula WasStaleAtRun (rulebook: =AND({{IsAuthoritativeBinding}}, {{AgeAtRunMinutes}} > {{SlaMinutesAtRun}}))
        [NotMapped]
        public bool? WasStaleAtRun
        {
            get => F.AsBool(F.Memo(this, "WasStaleAtRun", () => F.And(F.Bool3(F.Of(this.IsAuthoritativeBinding)), F.Bool3(F.Cmp(F.Of(this.AgeAtRunMinutes), ">", F.Of(this.SlaMinutesAtRun)))))); set { }
        }

        // Formula IsAuthoritativeBinding (rulebook: =INDEX(OperationalBindings!{{IsAuthoritative}}, MATCH({{OperationalBinding}}, OperationalBindings!{{OperationalBindingId}}, 0)))
        [NotMapped]
        public bool? IsAuthoritativeBinding
        {
            get => F.AsBool(F.Memo(this, "IsAuthoritativeBinding", () => F.Lookup<OperationalBinding>(this, "OperationalBindings", "OperationalBindingId", __c => __c.OperationalBindings, __r => F.Of(__r.OperationalBindingId), F.Of(this.OperationalBinding), __r => F.Of(__r.IsAuthoritative), () => F.Of(new OperationalBinding().IsAuthoritative)))); set { }
        }

        // Formula StaleAtRunStepKey (rulebook: =IF({{WasStaleAtRun}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? StaleAtRunStepKey
        {
            get => F.AsString(F.Memo(this, "StaleAtRunStepKey", () => (F.Truthy(F.Bool3(F.Of(this.WasStaleAtRun))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }


        public string? StepExecution { get; set; }
        public string? OperationalBinding { get; set; }

        private StepExecution _stepExecutionRef;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecutionRef
        {
            get
            {
                if (_stepExecutionRef == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutionRef - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecutionRef = base.SoAContext.StepExecutions.Find(StepExecution);
                    if (_stepExecutionRef != null)
                    {
                        base.SoAContext.Attach(_stepExecutionRef);
                    }
                }
                return _stepExecutionRef;
            }
            set
            {
                if (_stepExecutionRef != value)
                {
                    _stepExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepExecutionRef != null)
                    {
                        StepExecution = _stepExecutionRef.StepExecutionId;
                    }
                }
            }
        }

        private OperationalBinding _operationalBindingRef;

        [ForeignKey("OperationalBinding")]
        public virtual OperationalBinding OperationalBindingRef
        {
            get
            {
                if (_operationalBindingRef == null && !string.IsNullOrEmpty(OperationalBinding))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindingRef - no database context is set. OperationalBinding: " + OperationalBinding + ".");
                        }
                        return null;
                    }
                    _operationalBindingRef = base.SoAContext.OperationalBindings.Find(OperationalBinding);
                    if (_operationalBindingRef != null)
                    {
                        base.SoAContext.Attach(_operationalBindingRef);
                    }
                }
                return _operationalBindingRef;
            }
            set
            {
                if (_operationalBindingRef != value)
                {
                    _operationalBindingRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_operationalBindingRef != null)
                    {
                        OperationalBinding = _operationalBindingRef.OperationalBindingId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.OperationalBindingRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
