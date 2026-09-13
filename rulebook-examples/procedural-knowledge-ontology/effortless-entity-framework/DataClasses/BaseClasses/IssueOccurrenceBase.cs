
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
    [Table("IssueOccurrences")]
    public class IssueOccurrenceBase : SoAEntityBase
    {
        [Key]
        public string IssueOccurrenceId { get; set; }

        // Formula Name (rulebook: ={{Error}} & " @ " & {{OccurredAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Error)), F.S(" @ "), F.TimestamptzText(F.Of(this.OccurredAt))))); set { }
        }

        public DateTimeOffset? OccurredAt { get; set; }
        public string? IssueCause { get; set; }
        public string? IssueSolution { get; set; }
        public string? Status { get; set; }
        // Formula IsUnresolved (rulebook: =OR({{Status}} = "Open", {{Status}} = "Investigating", {{Status}} = "Monitoring"))
        [NotMapped]
        public bool? IsUnresolved
        {
            get => F.AsBool(F.Memo(this, "IsUnresolved", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Open"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Investigating"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Monitoring")))))); set { }
        }

        // Formula StepExecutionWhenUnresolved (rulebook: =IF({{IsUnresolved}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? StepExecutionWhenUnresolved
        {
            get => F.AsString(F.Memo(this, "StepExecutionWhenUnresolved", () => (F.Truthy(F.Bool3(F.Of(this.IsUnresolved))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? Error { get; set; }
        public string? EncounteredByAgent { get; set; }

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

        private Error _errorRef;

        [ForeignKey("Error")]
        public virtual Error ErrorRef
        {
            get
            {
                if (_errorRef == null && !string.IsNullOrEmpty(Error))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ErrorRef - no database context is set. Error: " + Error + ".");
                        }
                        return null;
                    }
                    _errorRef = base.SoAContext.Errors.Find(Error);
                    if (_errorRef != null)
                    {
                        base.SoAContext.Attach(_errorRef);
                    }
                }
                return _errorRef;
            }
            set
            {
                if (_errorRef != value)
                {
                    _errorRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_errorRef != null)
                    {
                        Error = _errorRef.ErrorId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("EncounteredByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(EncounteredByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. EncounteredByAgent: " + EncounteredByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(EncounteredByAgent);
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
                        EncounteredByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.ErrorRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
