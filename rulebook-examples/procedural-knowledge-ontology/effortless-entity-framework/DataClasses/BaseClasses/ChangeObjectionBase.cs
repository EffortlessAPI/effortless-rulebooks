
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
    [Table("ChangeObjections")]
    public class ChangeObjectionBase : SoAEntityBase
    {
        [Key]
        public string ChangeObjectionId { get; set; }

        // Formula Name (rulebook: ={{ModelChangeRequest}} & " objection by " & {{RaisedByAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ModelChangeRequest)), F.S(" objection by "), F.Text(F.Of(this.RaisedByAgent))))); set { }
        }

        public DateTimeOffset? RaisedAt { get; set; }
        public string? Objection { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public string? Resolution { get; set; }
        // Formula IsUnresolved (rulebook: ={{ResolvedAt}} = "")
        [NotMapped]
        public bool? IsUnresolved
        {
            get => F.AsBool(F.Memo(this, "IsUnresolved", () => F.IsBlank(F.Of(this.ResolvedAt)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ModelChangeRequest { get; set; }
        public string? RaisedByAgent { get; set; }
        public string? ResolvedByAgent { get; set; }

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

        [ForeignKey("RaisedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RaisedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RaisedByAgent: " + RaisedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(RaisedByAgent);
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
                        RaisedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ResolvedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ResolvedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ResolvedByAgent: " + ResolvedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ResolvedByAgent);
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
                        ResolvedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ModelChangeRequestRef;
            _ = this.Agent;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
