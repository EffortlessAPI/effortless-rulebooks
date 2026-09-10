
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("KnowledgeBrokerLinks")]
    public class KnowledgeBrokerLinkBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeBrokerLinkId { get; set; }

        // Formula Name (rulebook: ={{Seeker}} & " -> " & {{Broker}})
        public string? Name
        {
            get => this.Seeker + " -> " + this.Broker; set { }
        }

        public string? Frequency { get; set; }
        public DateTime? LastConsultedAt { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula DaysSinceConsulted (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{LastConsultedAt}}, "days"))
        public int? DaysSinceConsulted
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.LastConsultedAt, "days"); set { }
        }

        // Formula IsActiveReliance (rulebook: =AND(NOT({{Frequency}} = "Rarely"), {{DaysSinceConsulted}} <= 180))
        public bool? IsActiveReliance
        {
            get => AND(NOT(this.Frequency = "Rarely"), this.DaysSinceConsulted <= 180); set { }
        }

        // Formula BrokerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{Broker}}, Agents!{{AgentId}}, 0)))
        public bool? BrokerIsStillEngaged
        {
            get => INDEX(Agents!this.IsStillEngaged, MATCH(this.Broker, Agents!this.AgentId, 0)); set { }
        }

        // Formula IsAtRiskReliance (rulebook: =AND({{IsActiveReliance}}, NOT({{BrokerIsStillEngaged}})))
        public bool? IsAtRiskReliance
        {
            get => AND(this.IsActiveReliance, NOT(this.BrokerIsStillEngaged)); set { }
        }

        // Formula ActiveRelianceBrokerKey (rulebook: =IF({{IsActiveReliance}}, {{Broker}}, ""))
        public string? ActiveRelianceBrokerKey
        {
            get => IF(this.IsActiveReliance, this.Broker, ""); set { }
        }

        // Formula AtRiskBrokerKey (rulebook: =IF({{IsAtRiskReliance}}, {{Broker}}, ""))
        public string? AtRiskBrokerKey
        {
            get => IF(this.IsAtRiskReliance, this.Broker, ""); set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. Seeker: " + Seeker + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(Seeker);
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
                    Seeker = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("Broker")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(Broker))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. Broker: " + Broker + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(Broker);
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
                    Broker = _agent == null ? default : _agent.AgentId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. Topic: " + Topic + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = Context.VocabularyTerms.Find(Topic);
                    if (_vocabularyTerm != null)
                    {
                        Context.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    Topic = _vocabularyTerm == null ? default : _vocabularyTerm.VocabularyTermId;
                }
            }
        }

        private EvaluationContext _evaluationContext;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContext
        {
            get
            {
                if (_evaluationContext == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContext - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContext = Context.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContext != null)
                    {
                        Context.Attach(_evaluationContext);
                    }
                }
                return _evaluationContext;
            }
            set
            {
                if (_evaluationContext != value)
                {
                    _evaluationContext = value;
                    EvaluationContext = _evaluationContext == null ? default : _evaluationContext.EvaluationContextId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.Agent;
            _ = this.VocabularyTerm;
            _ = this.EvaluationContext;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
