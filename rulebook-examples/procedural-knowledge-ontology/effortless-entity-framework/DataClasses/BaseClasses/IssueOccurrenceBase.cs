
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("IssueOccurrences")]
    public class IssueOccurrenceBase : SoAEntityBase
    {
        [Key]
        public string IssueOccurrenceId { get; set; }

        // Formula Name (rulebook: ={{Error}} & " @ " & {{OccurredAt}})
        public string? Name
        {
            get => this.Error + " @ " + this.OccurredAt; set { }
        }

        public DateTime? OccurredAt { get; set; }
        public string? IssueCause { get; set; }
        public string? IssueSolution { get; set; }
        public string? Status { get; set; }
        // Formula IsUnresolved (rulebook: =OR({{Status}} = "Open", {{Status}} = "Investigating", {{Status}} = "Monitoring"))
        public bool? IsUnresolved
        {
            get => OR(this.Status = "Open", this.Status = "Investigating", this.Status = "Monitoring"); set { }
        }

        // Formula StepExecutionWhenUnresolved (rulebook: =IF({{IsUnresolved}}, {{StepExecution}}, ""))
        public string? StepExecutionWhenUnresolved
        {
            get => IF(this.IsUnresolved, this.StepExecution, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? Error { get; set; }
        public string? EncounteredByAgent { get; set; }

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

        private Error _error;

        [ForeignKey("Error")]
        public virtual Error Error
        {
            get
            {
                if (_error == null && !string.IsNullOrEmpty(Error))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Error - no database context is set. Error: " + Error + ".");
                        }
                        return null;
                    }
                    _error = Context.Errors.Find(Error);
                    if (_error != null)
                    {
                        Context.Attach(_error);
                    }
                }
                return _error;
            }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    Error = _error == null ? default : _error.ErrorId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. EncounteredByAgent: " + EncounteredByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(EncounteredByAgent);
                    if (_agent != null)
                    {
                        Context.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    EncounteredByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
            _ = this.Error;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
