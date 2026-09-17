
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
    [Table("ExecutionParticipants")]
    public class ExecutionParticipantBase : SoAEntityBase
    {
        [Key]
        public string ExecutionParticipantId { get; set; }

        // Formula Name (rulebook: ={{ProcedureExecution}} & " / " & {{Agent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureExecution)), F.S(" / "), F.Text(F.Of(this.Agent))))); set { }
        }

        public string? ParticipationRole { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? Agent { get; set; }

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

        private Agent _agentRef;

        [ForeignKey("Agent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Agent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Agent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        Agent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
