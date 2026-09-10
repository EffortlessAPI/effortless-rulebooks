
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("BindingObservations")]
    public class BindingObservationBase : SoAEntityBase
    {
        [Key]
        public string BindingObservationId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{BindingObservationId}})
        public string? Name
        {
            get => this.StepExecution + " / " + this.BindingObservationId; set { }
        }

        public DateTime? ObservedSourceTimestamp { get; set; }
        public DateTime? ReadAt { get; set; }
        // Formula SlaMinutesAtRun (rulebook: =INDEX(OperationalBindings!{{FreshnessSlaMinutes}}, MATCH({{OperationalBinding}}, OperationalBindings!{{OperationalBindingId}}, 0)))
        public int? SlaMinutesAtRun
        {
            get => INDEX(OperationalBindings!this.FreshnessSlaMinutes, MATCH(this.OperationalBinding, OperationalBindings!this.OperationalBindingId, 0)); set { }
        }

        // Formula AgeAtRunMinutes (rulebook: =DATETIME_DIFF({{ReadAt}}, {{ObservedSourceTimestamp}}, "minutes"))
        public int? AgeAtRunMinutes
        {
            get => DATETIME_DIFF(this.ReadAt, this.ObservedSourceTimestamp, "minutes"); set { }
        }

        // Formula WasStaleAtRun (rulebook: =AND({{IsAuthoritativeBinding}}, {{AgeAtRunMinutes}} > {{SlaMinutesAtRun}}))
        public bool? WasStaleAtRun
        {
            get => AND(this.IsAuthoritativeBinding, this.AgeAtRunMinutes > this.SlaMinutesAtRun); set { }
        }

        // Formula IsAuthoritativeBinding (rulebook: =INDEX(OperationalBindings!{{IsAuthoritative}}, MATCH({{OperationalBinding}}, OperationalBindings!{{OperationalBindingId}}, 0)))
        public bool? IsAuthoritativeBinding
        {
            get => INDEX(OperationalBindings!this.IsAuthoritative, MATCH(this.OperationalBinding, OperationalBindings!this.OperationalBindingId, 0)); set { }
        }

        // Formula StaleAtRunStepKey (rulebook: =IF({{WasStaleAtRun}}, {{StepExecution}}, ""))
        public string? StaleAtRunStepKey
        {
            get => IF(this.WasStaleAtRun, this.StepExecution, ""); set { }
        }


        public string? StepExecution { get; set; }
        public string? OperationalBinding { get; set; }

        private StepExecution _stepExecution;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(StepExecution);
                    if (_stepExecution != null)
                    {
                        Context.Attach(_stepExecution);
                    }
                }
                return _stepExecution;
            }
            set
            {
                if (_stepExecution != value)
                {
                    _stepExecution = value;
                    StepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
                }
            }
        }

        private OperationalBinding _operationalBinding;

        [ForeignKey("OperationalBinding")]
        public virtual OperationalBinding OperationalBinding
        {
            get
            {
                if (_operationalBinding == null && !string.IsNullOrEmpty(OperationalBinding))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBinding - no database context is set. OperationalBinding: " + OperationalBinding + ".");
                        }
                        return null;
                    }
                    _operationalBinding = Context.OperationalBindings.Find(OperationalBinding);
                    if (_operationalBinding != null)
                    {
                        Context.Attach(_operationalBinding);
                    }
                }
                return _operationalBinding;
            }
            set
            {
                if (_operationalBinding != value)
                {
                    _operationalBinding = value;
                    OperationalBinding = _operationalBinding == null ? default : _operationalBinding.OperationalBindingId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
            _ = this.OperationalBinding;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
