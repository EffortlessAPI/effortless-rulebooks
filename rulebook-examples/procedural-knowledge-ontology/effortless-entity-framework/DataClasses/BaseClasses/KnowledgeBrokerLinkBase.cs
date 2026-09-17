
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
        // Formula PointedHolder (rulebook: =INDEX(KnowHowCarriers!{{HolderAgent}}, MATCH({{PointsToKnowHow}}, KnowHowCarriers!{{KnowHowCarrierId}}, 0)))
        [NotMapped]
        public string? PointedHolder
        {
            get => F.AsString(F.Memo(this, "PointedHolder", () => F.Lookup<KnowHowCarrier>(this, "KnowHowCarriers", "KnowHowCarrierId", __c => __c.KnowHowCarriers, __r => F.Of(__r.KnowHowCarrierId), F.Of(this.PointsToKnowHow), __r => F.Of(__r.HolderAgent), () => F.Of(new KnowHowCarrier().HolderAgent)))); set { }
        }

        // Formula LocatesOtherHolder (rulebook: =AND({{PointsToKnowHow}} <> "", {{PointedHolder}} <> "", {{PointedHolder}} <> {{Broker}}))
        [NotMapped]
        public bool? LocatesOtherHolder
        {
            get => F.AsBool(F.Memo(this, "LocatesOtherHolder", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PointsToKnowHow))), F.Bool3(F.IsNotBlank(F.Of(this.PointedHolder))), F.Bool3(F.Ne(F.Of(this.PointedHolder), F.Nullif(F.Of(this.Broker))))))); set { }
        }

        public string? SeekerWording { get; set; }
        public string? HolderWording { get; set; }
        // Formula TranslationBetweenVocabularies (rulebook: =IF(AND({{SeekerVocabulary}} <> "", {{HolderVocabulary}} <> "", {{SeekerVocabulary}} <> {{HolderVocabulary}}), CONCAT({{SeekerWording}}, " (", {{SeekerVocabulary}}, ") = ", {{HolderWording}}, " (", {{HolderVocabulary}}, ")"), ""))
        [NotMapped]
        public string? TranslationBetweenVocabularies
        {
            get => F.AsString(F.Memo(this, "TranslationBetweenVocabularies", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.SeekerVocabulary))), F.Bool3(F.IsNotBlank(F.Of(this.HolderVocabulary))), F.Bool3(F.Ne(F.Nullif(F.Of(this.SeekerVocabulary)), F.Nullif(F.Of(this.HolderVocabulary))))))) ? F.Concat(F.Text(F.Of(this.SeekerWording)), F.S(" ("), F.Text(F.Of(this.SeekerVocabulary)), F.S(") = "), F.Text(F.Of(this.HolderWording)), F.S(" ("), F.Text(F.Of(this.HolderVocabulary)), F.S(")")) : F.S("")))); set { }
        }


        public string? Seeker { get; set; }
        public string? Broker { get; set; }
        public string? Topic { get; set; }
        public string? EvaluationContext { get; set; }
        public string? PointsToKnowHow { get; set; }
        public string? SeekerVocabulary { get; set; }
        public string? HolderVocabulary { get; set; }

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

        private KnowHowCarrier _knowHowCarrier;

        [ForeignKey("PointsToKnowHow")]
        public virtual KnowHowCarrier KnowHowCarrier
        {
            get
            {
                if (_knowHowCarrier == null && !string.IsNullOrEmpty(PointsToKnowHow))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarrier - no database context is set. PointsToKnowHow: " + PointsToKnowHow + ".");
                        }
                        return null;
                    }
                    _knowHowCarrier = base.SoAContext.KnowHowCarriers.Find(PointsToKnowHow);
                    if (_knowHowCarrier != null)
                    {
                        base.SoAContext.Attach(_knowHowCarrier);
                    }
                }
                return _knowHowCarrier;
            }
            set
            {
                if (_knowHowCarrier != value)
                {
                    _knowHowCarrier = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowHowCarrier != null)
                    {
                        PointsToKnowHow = _knowHowCarrier.KnowHowCarrierId;
                    }
                }
            }
        }

        private Vocabulary _vocabulary;

        [ForeignKey("SeekerVocabulary")]
        public virtual Vocabulary Vocabulary
        {
            get
            {
                if (_vocabulary == null && !string.IsNullOrEmpty(SeekerVocabulary))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabulary - no database context is set. SeekerVocabulary: " + SeekerVocabulary + ".");
                        }
                        return null;
                    }
                    _vocabulary = base.SoAContext.Vocabularies.Find(SeekerVocabulary);
                    if (_vocabulary != null)
                    {
                        base.SoAContext.Attach(_vocabulary);
                    }
                }
                return _vocabulary;
            }
            set
            {
                if (_vocabulary != value)
                {
                    _vocabulary = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabulary != null)
                    {
                        SeekerVocabulary = _vocabulary.VocabularyId;
                    }
                }
            }
        }

        private Vocabulary _vocabularyRef;

        [ForeignKey("HolderVocabulary")]
        public virtual Vocabulary VocabularyRef
        {
            get
            {
                if (_vocabularyRef == null && !string.IsNullOrEmpty(HolderVocabulary))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyRef - no database context is set. HolderVocabulary: " + HolderVocabulary + ".");
                        }
                        return null;
                    }
                    _vocabularyRef = base.SoAContext.Vocabularies.Find(HolderVocabulary);
                    if (_vocabularyRef != null)
                    {
                        base.SoAContext.Attach(_vocabularyRef);
                    }
                }
                return _vocabularyRef;
            }
            set
            {
                if (_vocabularyRef != value)
                {
                    _vocabularyRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyRef != null)
                    {
                        HolderVocabulary = _vocabularyRef.VocabularyId;
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
            _ = this.KnowHowCarrier;
            _ = this.Vocabulary;
            _ = this.VocabularyRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
