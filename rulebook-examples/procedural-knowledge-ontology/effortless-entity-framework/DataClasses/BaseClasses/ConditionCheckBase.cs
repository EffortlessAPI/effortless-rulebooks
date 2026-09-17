
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
    [Table("ConditionChecks")]
    public class ConditionCheckBase : SoAEntityBase
    {
        [Key]
        public string ConditionCheckId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{StepCondition}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.StepExecution)), F.S(" / "), F.Text(F.Of(this.StepCondition))))); set { }
        }

        public bool? Held { get; set; }
        public DateTimeOffset? CheckedAt { get; set; }
        // Formula ConditionKind (rulebook: =INDEX(StepConditions!{{ConditionKind}}, MATCH({{StepCondition}}, StepConditions!{{StepConditionId}}, 0)))
        [NotMapped]
        public string? ConditionKind
        {
            get => F.AsString(F.Memo(this, "ConditionKind", () => F.Lookup<StepCondition>(this, "StepConditions", "StepConditionId", __c => __c.StepConditions, __r => F.Of(__r.StepConditionId), F.Of(this.StepCondition), __r => F.Of(__r.ConditionKind), () => F.Of(new StepCondition().ConditionKind)))); set { }
        }

        // Formula IsFailedPrecondition (rulebook: =AND({{ConditionKind}} = "Precondition", {{Held}} = FALSE))
        [NotMapped]
        public bool? IsFailedPrecondition
        {
            get => F.AsBool(F.Memo(this, "IsFailedPrecondition", () => F.And(F.Bool3(F.Eq(F.Of(this.ConditionKind), F.S("Precondition"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Held)), F.B(false)))))); set { }
        }

        // Formula IsViolatedInvariant (rulebook: =AND({{ConditionKind}} = "Invariant", {{Held}} = FALSE))
        [NotMapped]
        public bool? IsViolatedInvariant
        {
            get => F.AsBool(F.Memo(this, "IsViolatedInvariant", () => F.And(F.Bool3(F.Eq(F.Of(this.ConditionKind), F.S("Invariant"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Held)), F.B(false)))))); set { }
        }

        // Formula FailedPreconditionExecutionKey (rulebook: =IF({{IsFailedPrecondition}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? FailedPreconditionExecutionKey
        {
            get => F.AsString(F.Memo(this, "FailedPreconditionExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsFailedPrecondition))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula ViolatedInvariantExecutionKey (rulebook: =IF({{IsViolatedInvariant}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? ViolatedInvariantExecutionKey
        {
            get => F.AsString(F.Memo(this, "ViolatedInvariantExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsViolatedInvariant))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? StepCondition { get; set; }
        public string? CheckedByAgent { get; set; }

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

        private StepCondition _stepConditionRef;

        [ForeignKey("StepCondition")]
        public virtual StepCondition StepConditionRef
        {
            get
            {
                if (_stepConditionRef == null && !string.IsNullOrEmpty(StepCondition))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepConditionRef - no database context is set. StepCondition: " + StepCondition + ".");
                        }
                        return null;
                    }
                    _stepConditionRef = base.SoAContext.StepConditions.Find(StepCondition);
                    if (_stepConditionRef != null)
                    {
                        base.SoAContext.Attach(_stepConditionRef);
                    }
                }
                return _stepConditionRef;
            }
            set
            {
                if (_stepConditionRef != value)
                {
                    _stepConditionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepConditionRef != null)
                    {
                        StepCondition = _stepConditionRef.StepConditionId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("CheckedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(CheckedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CheckedByAgent: " + CheckedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(CheckedByAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        CheckedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.StepConditionRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
