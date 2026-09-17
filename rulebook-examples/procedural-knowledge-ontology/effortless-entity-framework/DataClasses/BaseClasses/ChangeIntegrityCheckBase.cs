
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
    [Table("ChangeIntegrityChecks")]
    public class ChangeIntegrityCheckBase : SoAEntityBase
    {
        [Key]
        public string ChangeIntegrityCheckId { get; set; }

        // Formula Name (rulebook: ={{ModelChangeRequest}} & " " & {{CheckKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelChangeRequest)), F.S(" "), F.Text(F.Of(this.CheckKind))))); set { }
        }

        public string? CheckKind { get; set; }
        // Formula CheckerKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{CheckedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? CheckerKind
        {
            get => F.AsString(F.Memo(this, "CheckerKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CheckedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        public DateTimeOffset? CheckedAt { get; set; }
        public string? Result { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ModelChangeRequest { get; set; }
        public string? CheckedByAgent { get; set; }

        private ModelChangeRequest _modelChangeRequestRef;

        [ForeignKey("ModelChangeRequest")]
        public virtual ModelChangeRequest ModelChangeRequestRef
        {
            get
            {
                if (_modelChangeRequestRef == null && !string.IsNullOrEmpty(ModelChangeRequest))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeRequestRef - no database context is set. ModelChangeRequest: " + ModelChangeRequest + ".");
                        }
                        return null;
                    }
                    _modelChangeRequestRef = base.SoAContext.ModelChangeRequests.Find(ModelChangeRequest);
                    if (_modelChangeRequestRef != null)
                    {
                        base.SoAContext.Attach(_modelChangeRequestRef);
                    }
                }
                return _modelChangeRequestRef;
            }
            set
            {
                if (_modelChangeRequestRef != value)
                {
                    _modelChangeRequestRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelChangeRequestRef != null)
                    {
                        ModelChangeRequest = _modelChangeRequestRef.ModelChangeRequestId;
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
            _ = this.ModelChangeRequestRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
