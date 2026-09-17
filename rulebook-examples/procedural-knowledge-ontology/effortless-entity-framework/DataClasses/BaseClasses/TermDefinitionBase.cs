
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
    [Table("TermDefinitions")]
    public class TermDefinitionBase : SoAEntityBase
    {
        [Key]
        public string TermDefinitionId { get; set; }

        // Formula Name (rulebook: ={{RulebookTable}} & " definition")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.RulebookTable)), F.S(" definition")))); set { }
        }

        public string? Includes { get; set; }
        public string? Excludes { get; set; }
        public string? NeighboringTerms { get; set; }
        // Formula DrafterKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{DraftedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? DrafterKind
        {
            get => F.AsString(F.Memo(this, "DrafterKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.DraftedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        public DateTimeOffset? DraftedAt { get; set; }
        public DateTimeOffset? RevisedAt { get; set; }
        // Formula AiDraftAdoptedUnrevised (rulebook: =AND({{DrafterKind}} = "AIAgent", {{RevisedByAgent}} = ""))
        [NotMapped]
        public bool? AiDraftAdoptedUnrevised
        {
            get => F.AsBool(F.Memo(this, "AiDraftAdoptedUnrevised", () => F.And(F.Bool3(F.Eq(F.Of(this.DrafterKind), F.S("AIAgent"))), F.Bool3(F.IsBlank(F.Of(this.RevisedByAgent)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? RulebookTable { get; set; }
        public string? DraftedByAgent { get; set; }
        public string? RevisedByAgent { get; set; }

        private RulebookTable _rulebookTableRef;

        [ForeignKey("RulebookTable")]
        public virtual RulebookTable RulebookTableRef
        {
            get
            {
                if (_rulebookTableRef == null && !string.IsNullOrEmpty(RulebookTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTableRef - no database context is set. RulebookTable: " + RulebookTable + ".");
                        }
                        return null;
                    }
                    _rulebookTableRef = base.SoAContext.RulebookTables.Find(RulebookTable);
                    if (_rulebookTableRef != null)
                    {
                        base.SoAContext.Attach(_rulebookTableRef);
                    }
                }
                return _rulebookTableRef;
            }
            set
            {
                if (_rulebookTableRef != value)
                {
                    _rulebookTableRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTableRef != null)
                    {
                        RulebookTable = _rulebookTableRef.RulebookTableId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("DraftedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(DraftedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. DraftedByAgent: " + DraftedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(DraftedByAgent);
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
                        DraftedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("RevisedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(RevisedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. RevisedByAgent: " + RevisedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(RevisedByAgent);
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
                        RevisedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RulebookTableRef;
            _ = this.Agent;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
