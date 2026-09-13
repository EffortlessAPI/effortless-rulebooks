
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
    [Table("ObservedTransitions")]
    public class ObservedTransitionBase : SoAEntityBase
    {
        [Key]
        public string ObservedTransitionId { get; set; }

        // Formula Name (rulebook: ={{StepTransition}} & " @ " & {{ObservedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.StepTransition)), F.S(" @ "), F.TimestamptzText(F.Of(this.ObservedAt))))); set { }
        }

        public DateTimeOffset? ObservedAt { get; set; }
        public string? TriggerReason { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string ProcedureExecution { get; set; }
        public string StepTransition { get; set; }
        public string? ArrivingStepExecution { get; set; }

        private ProcedureExecution _procedureExecutionRef;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecutionRef
        {
            get
            {
                if (_procedureExecutionRef == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutionRef - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecutionRef = base.SoAContext.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecutionRef != null)
                    {
                        base.SoAContext.Attach(_procedureExecutionRef);
                    }
                }
                return _procedureExecutionRef;
            }
            set
            {
                if (_procedureExecutionRef != value)
                {
                    _procedureExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecutionRef != null)
                    {
                        ProcedureExecution = _procedureExecutionRef.ProcedureExecutionId;
                    }
                }
            }
        }

        private StepTransition _stepTransitionRef;

        [ForeignKey("StepTransition")]
        public virtual StepTransition StepTransitionRef
        {
            get
            {
                if (_stepTransitionRef == null && !string.IsNullOrEmpty(StepTransition))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTransitionRef - no database context is set. StepTransition: " + StepTransition + ".");
                        }
                        return null;
                    }
                    _stepTransitionRef = base.SoAContext.StepTransitions.Find(StepTransition);
                    if (_stepTransitionRef != null)
                    {
                        base.SoAContext.Attach(_stepTransitionRef);
                    }
                }
                return _stepTransitionRef;
            }
            set
            {
                if (_stepTransitionRef != value)
                {
                    _stepTransitionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepTransitionRef != null)
                    {
                        StepTransition = _stepTransitionRef.StepTransitionId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. ArrivingStepExecution: " + ArrivingStepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = base.SoAContext.StepExecutions.Find(ArrivingStepExecution);
                    if (_stepExecution != null)
                    {
                        base.SoAContext.Attach(_stepExecution);
                    }
                }
                return _stepExecution;
            }
            set
            {
                if (_stepExecution != value)
                {
                    _stepExecution = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepExecution != null)
                    {
                        ArrivingStepExecution = _stepExecution.StepExecutionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.StepTransitionRef;
            _ = this.StepExecution;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
