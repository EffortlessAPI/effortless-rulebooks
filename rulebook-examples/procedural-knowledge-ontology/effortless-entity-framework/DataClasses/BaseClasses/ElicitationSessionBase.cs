
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
    [Table("ElicitationSessions")]
    public class ElicitationSessionBase : SoAEntityBase
    {
        [Key]
        public string ElicitationSessionId { get; set; }

        // Formula Name (rulebook: ={{Method}} & " / " & {{StartedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Method)), F.S(" / "), F.TimestamptzText(F.Of(this.StartedAt))))); set { }
        }

        public string? Method { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public string? Summary { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceElicited (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{EndedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceElicited
        {
            get => F.AsInt(F.Memo(this, "DaysSinceElicited", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.EndedAt), F.S("days"))))); set { }
        }

        // Formula IsSingleWitnessMethod (rulebook: =OR({{Method}} = "Shadowing", {{Method}} = "PractitionerInterview"))
        [NotMapped]
        public bool? IsSingleWitnessMethod
        {
            get => F.AsBool(F.Memo(this, "IsSingleWitnessMethod", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("Shadowing"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Method)), F.S("PractitionerInterview")))))); set { }
        }

        // Formula PractitionerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{PractitionerAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? PractitionerIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "PractitionerIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.PractitionerAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula ValidFragmentsProduced (rulebook: =COUNTIFS(KnowledgeFragments!{{ValidFragmentSessionKey}}, {{ElicitationSessionId}}))
        [NotMapped]
        public decimal? ValidFragmentsProduced
        {
            get => F.AsDecimal(F.Memo(this, "ValidFragmentsProduced", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.ValidFragmentSessionKey), F.Of(this.ElicitationSessionId)))))); set { }
        }

        // Formula IsHighYieldSession (rulebook: ={{ValidFragmentsProduced}} >= 3)
        [NotMapped]
        public bool? IsHighYieldSession
        {
            get => F.AsBool(F.Memo(this, "IsHighYieldSession", () => F.Cmp(F.Of(this.ValidFragmentsProduced), ">=", F.I(3)))); set { }
        }

        // Formula IsConcentratedSingleWitness (rulebook: =AND({{IsSingleWitnessMethod}}, {{IsHighYieldSession}}))
        [NotMapped]
        public bool? IsConcentratedSingleWitness
        {
            get => F.AsBool(F.Memo(this, "IsConcentratedSingleWitness", () => F.And(F.Bool3(F.Of(this.IsSingleWitnessMethod)), F.Bool3(F.Of(this.IsHighYieldSession))))); set { }
        }

        // Formula IsStaleConcentratedWitness (rulebook: =AND({{IsConcentratedSingleWitness}}, {{DaysSinceElicited}} > 180))
        [NotMapped]
        public bool? IsStaleConcentratedWitness
        {
            get => F.AsBool(F.Memo(this, "IsStaleConcentratedWitness", () => F.And(F.Bool3(F.Of(this.IsConcentratedSingleWitness)), F.Bool3(F.Cmp(F.Of(this.DaysSinceElicited), ">", F.I(180)))))); set { }
        }

        // Formula ConcentratedSessionVersionKey (rulebook: =IF({{IsConcentratedSingleWitness}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? ConcentratedSessionVersionKey
        {
            get => F.AsString(F.Memo(this, "ConcentratedSessionVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsConcentratedSingleWitness))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? PractitionerAgent { get; set; }
        public string? FacilitatorAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("PractitionerAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(PractitionerAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. PractitionerAgent: " + PractitionerAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(PractitionerAgent);
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
                        PractitionerAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("FacilitatorAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(FacilitatorAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. FacilitatorAgent: " + FacilitatorAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(FacilitatorAgent);
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
                        FacilitatorAgent = _agentRef.AgentId;
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

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("ElicitationSessionRef")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
                return _knowledgeFragments;
            }
            private set
            {
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged -= KnowledgeFragments_CollectionChanged;
                }
                _knowledgeFragments = value;
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
            }
        }

        private void KnowledgeFragments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeFragment>())
                {
                    item.ElicitationSession = this.ElicitationSessionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.EvaluationContextRef;
            _ = this.KnowledgeFragments;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
