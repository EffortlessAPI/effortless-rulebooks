
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
    [Table("UserFeedback")]
    public class UserFeedbackBase : SoAEntityBase
    {
        [Key]
        public string UserFeedbackId { get; set; }

        // Formula Name (rulebook: ={{Disposition}} & ": " & LEFT({{FeedbackText}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Disposition)), F.S(": "), F.Text(F.Left(F.Of(this.FeedbackText), F.I(60)))))); set { }
        }

        public DateTimeOffset? ProvidedAt { get; set; }
        public string? FeedbackText { get; set; }
        public string? Disposition { get; set; }
        public string? ChangeRequestKey { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? ProvidedByAgent { get; set; }

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

        private Agent _agent;

        [ForeignKey("ProvidedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ProvidedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ProvidedByAgent: " + ProvidedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ProvidedByAgent);
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
                        ProvidedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
