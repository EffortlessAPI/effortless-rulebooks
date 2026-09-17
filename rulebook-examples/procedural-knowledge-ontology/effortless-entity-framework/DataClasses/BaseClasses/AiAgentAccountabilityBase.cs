
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
    [Table("AiAgentAccountabilities")]
    public class AiAgentAccountabilityBase : SoAEntityBase
    {
        [Key]
        public string AiAgentAccountabilityId { get; set; }

        // Formula Name (rulebook: ={{AiAgent}} & " accountable to " & {{AccountableAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AiAgent)), F.S(" accountable to "), F.Text(F.Of(this.AccountableAgent))))); set { }
        }

        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsCurrent (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsCurrent
        {
            get => F.AsBool(F.Memo(this, "IsCurrent", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula AccountableAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{AccountableAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? AccountableAgentKind
        {
            get => F.AsString(F.Memo(this, "AccountableAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.AccountableAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula IsAccountableToNonPerson (rulebook: ={{AccountableAgentKind}} <> "Human")
        [NotMapped]
        public bool? IsAccountableToNonPerson
        {
            get => F.AsBool(F.Memo(this, "IsAccountableToNonPerson", () => F.Ne(F.Of(this.AccountableAgentKind), F.S("Human")))); set { }
        }

        // Formula IsCurrentHumanAccountability (rulebook: =AND({{IsCurrent}}, {{AccountableAgentKind}} = "Human"))
        [NotMapped]
        public bool? IsCurrentHumanAccountability
        {
            get => F.AsBool(F.Memo(this, "IsCurrentHumanAccountability", () => F.And(F.Bool3(F.Of(this.IsCurrent)), F.Bool3(F.Eq(F.Of(this.AccountableAgentKind), F.S("Human")))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AiAgent { get; set; }
        public string? AccountableAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private Agent _agent;

        [ForeignKey("AiAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AiAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AiAgent: " + AiAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AiAgent);
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
                        AiAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("AccountableAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(AccountableAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. AccountableAgent: " + AccountableAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(AccountableAgent);
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
                        AccountableAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
