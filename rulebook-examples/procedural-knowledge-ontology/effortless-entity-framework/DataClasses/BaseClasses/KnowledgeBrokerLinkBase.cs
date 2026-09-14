
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
    [Table("KnowledgeBrokerLinks")]
    public class KnowledgeBrokerLinkBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeBrokerLinkId { get; set; }

        // Formula Name (rulebook: ={{Seeker}} & " -> " & {{Broker}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Seeker)), F.S(" -> "), F.Text(F.Of(this.Broker))))); set { }
        }

        public string? Frequency { get; set; }
        public DateTimeOffset? LastConsultedAt { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceConsulted (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{LastConsultedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceConsulted
        {
            get => F.AsInt(F.Memo(this, "DaysSinceConsulted", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastConsultedAt), F.S("days"))))); set { }
        }

        // Formula IsActiveReliance (rulebook: =AND(NOT({{Frequency}} = "Rarely"), {{DaysSinceConsulted}} <= 180))
        [NotMapped]
        public bool? IsActiveReliance
        {
            get => F.AsBool(F.Memo(this, "IsActiveReliance", () => F.And(F.Bool3(F.Not(F.Bool3(F.Eq(F.Nullif(F.Of(this.Frequency)), F.S("Rarely"))))), F.Bool3(F.Cmp(F.Of(this.DaysSinceConsulted), "<=", F.I(180)))))); set { }
        }

        // Formula BrokerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{Broker}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? BrokerIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "BrokerIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.Broker), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula IsAtRiskReliance (rulebook: =AND({{IsActiveReliance}}, NOT({{BrokerIsStillEngaged}})))
        [NotMapped]
        public bool? IsAtRiskReliance
        {
            get => F.AsBool(F.Memo(this, "IsAtRiskReliance", () => F.And(F.Bool3(F.Of(this.IsActiveReliance)), F.Bool3(F.Not(F.Bool3(F.Of(this.BrokerIsStillEngaged))))))); set { }
        }

        // Formula ActiveRelianceBrokerKey (rulebook: =IF({{IsActiveReliance}}, {{Broker}}, ""))
        [NotMapped]
        public string? ActiveRelianceBrokerKey
        {
            get => F.AsString(F.Memo(this, "ActiveRelianceBrokerKey", () => (F.Truthy(F.Bool3(F.Of(this.IsActiveReliance))) ? F.Of(this.Broker) : F.S("")))); set { }
        }

        // Formula AtRiskBrokerKey (rulebook: =IF({{IsAtRiskReliance}}, {{Broker}}, ""))
        [NotMapped]
        public string? AtRiskBrokerKey
        {
            get => F.AsString(F.Memo(this, "AtRiskBrokerKey", () => (F.Truthy(F.Bool3(F.Of(this.IsAtRiskReliance))) ? F.Of(this.Broker) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Seeker { get; set; }
        public string? Broker { get; set; }
        public string? Topic { get; set; }
        public string? EvaluationContext { get; set; }

        private Agent _agent;

        [ForeignKey("Seeker")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(Seeker))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. Seeker: " + Seeker + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(Seeker);
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
                        Seeker = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("Broker")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Broker))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Broker: " + Broker + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Broker);
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
                        Broker = _agentRef.AgentId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("Topic")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(Topic))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. Topic: " + Topic + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(Topic);
                    if (_vocabularyTerm != null)
                    {
                        base.SoAContext.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTerm != null)
                    {
                        Topic = _vocabularyTerm.VocabularyTermId;
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
            _ = this.VocabularyTerm;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
