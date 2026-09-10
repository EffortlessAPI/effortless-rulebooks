
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ObservedTransitions")]
    public class ObservedTransitionBase : SoAEntityBase
    {
        [Key]
        public string ObservedTransitionId { get; set; }

        // Formula Name (rulebook: ={{StepTransition}} & " @ " & {{ObservedAt}})
        public string? Name
        {
            get => this.StepTransition + " @ " + this.ObservedAt; set { }
        }

        public DateTime? ObservedAt { get; set; }
        public string? TriggerReason { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string ProcedureExecution { get; set; }
        public string StepTransition { get; set; }
        public string? ArrivingStepExecution { get; set; }

        private ProcedureExecution _procedureExecution;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecution
        {
            get
            {
                if (_procedureExecution == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecution - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecution = Context.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecution != null)
                    {
                        Context.Attach(_procedureExecution);
                    }
                }
                return _procedureExecution;
            }
            set
            {
                if (_procedureExecution != value)
                {
                    _procedureExecution = value;
                    ProcedureExecution = _procedureExecution == null ? default : _procedureExecution.ProcedureExecutionId;
                }
            }
        }

        private StepTransition _stepTransition;

        [ForeignKey("StepTransition")]
        public virtual StepTransition StepTransition
        {
            get
            {
                if (_stepTransition == null && !string.IsNullOrEmpty(StepTransition))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransition - no database context is set. StepTransition: " + StepTransition + ".");
                        }
                        return null;
                    }
                    _stepTransition = Context.StepTransitions.Find(StepTransition);
                    if (_stepTransition != null)
                    {
                        Context.Attach(_stepTransition);
                    }
                }
                return _stepTransition;
            }
            set
            {
                if (_stepTransition != value)
                {
                    _stepTransition = value;
                    StepTransition = _stepTransition == null ? default : _stepTransition.StepTransitionId;
                }
            }
        }

        private StepExecution _stepExecution;

        [ForeignKey("ArrivingStepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(ArrivingStepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. ArrivingStepExecution: " + ArrivingStepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(ArrivingStepExecution);
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
                    ArrivingStepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecution;
            _ = this.StepTransition;
            _ = this.StepExecution;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
