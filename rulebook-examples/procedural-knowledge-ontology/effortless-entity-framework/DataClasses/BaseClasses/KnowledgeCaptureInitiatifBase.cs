
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
    [Table("KnowledgeCaptureInitiatives")]
    public class KnowledgeCaptureInitiatifBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeCaptureInitiativeId { get; set; }

        // Formula Name (rulebook: ={{SourcingFunction}} & " by " & {{KnowledgeMethod}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.SourcingFunction)), F.S(" by "), F.Text(F.Of(this.KnowledgeMethod))))); set { }
        }

        public DateTimeOffset? StartedAt { get; set; }
        public string? Status { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? SourcingFunction { get; set; }
        public string? KnowledgeMethod { get; set; }
        public string? LeadAgent { get; set; }

        private SourcingFunction _sourcingFunctionRef;

        [ForeignKey("SourcingFunction")]
        public virtual SourcingFunction SourcingFunctionRef
        {
            get
            {
                if (_sourcingFunctionRef == null && !string.IsNullOrEmpty(SourcingFunction))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourcingFunctionRef - no database context is set. SourcingFunction: " + SourcingFunction + ".");
                        }
                        return null;
                    }
                    _sourcingFunctionRef = base.SoAContext.SourcingFunctions.Find(SourcingFunction);
                    if (_sourcingFunctionRef != null)
                    {
                        base.SoAContext.Attach(_sourcingFunctionRef);
                    }
                }
                return _sourcingFunctionRef;
            }
            set
            {
                if (_sourcingFunctionRef != value)
                {
                    _sourcingFunctionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_sourcingFunctionRef != null)
                    {
                        SourcingFunction = _sourcingFunctionRef.SourcingFunctionId;
                    }
                }
            }
        }

        private KnowledgeMethod _knowledgeMethodRef;

        [ForeignKey("KnowledgeMethod")]
        public virtual KnowledgeMethod KnowledgeMethodRef
        {
            get
            {
                if (_knowledgeMethodRef == null && !string.IsNullOrEmpty(KnowledgeMethod))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeMethodRef - no database context is set. KnowledgeMethod: " + KnowledgeMethod + ".");
                        }
                        return null;
                    }
                    _knowledgeMethodRef = base.SoAContext.KnowledgeMethods.Find(KnowledgeMethod);
                    if (_knowledgeMethodRef != null)
                    {
                        base.SoAContext.Attach(_knowledgeMethodRef);
                    }
                }
                return _knowledgeMethodRef;
            }
            set
            {
                if (_knowledgeMethodRef != value)
                {
                    _knowledgeMethodRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeMethodRef != null)
                    {
                        KnowledgeMethod = _knowledgeMethodRef.KnowledgeMethodId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("LeadAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(LeadAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. LeadAgent: " + LeadAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(LeadAgent);
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
                        LeadAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.SourcingFunctionRef;
            _ = this.KnowledgeMethodRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
