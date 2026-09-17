
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
    [Table("KnowledgeRepositoryEntries")]
    public class KnowledgeRepositoryEntryBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeRepositoryEntryId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public bool? CreditsSourceExpert { get; set; }
        public string? WrittenForAudience { get; set; }
        public bool? AuthoredOnAllocatedTime { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? LastUpdatedAt { get; set; }
        public int? ReviewIntervalDays { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula AuthorIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{AuthorAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? AuthorIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "AuthorIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.AuthorAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula AuthorAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{AuthorAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? AuthorAgentKind
        {
            get => F.AsString(F.Memo(this, "AuthorAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.AuthorAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula OwnerOrganization (rulebook: =INDEX(Procedures!{{OwnerOrganization}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public string? OwnerOrganization
        {
            get => F.AsString(F.Memo(this, "OwnerOrganization", () => F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.OwnerOrganization), () => F.Of(new Procedure().OwnerOrganization)))); set { }
        }

        // Formula DaysSinceUpdated (rulebook: =IF({{LastUpdatedAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{LastUpdatedAt}}, "days")))
        [NotMapped]
        public int? DaysSinceUpdated
        {
            get => F.AsInt(F.Memo(this, "DaysSinceUpdated", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastUpdatedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastUpdatedAt), F.S("days")))))); set { }
        }

        // Formula IsStale (rulebook: =AND({{ReviewIntervalDays}} > 0, {{DaysSinceUpdated}} > {{ReviewIntervalDays}}))
        [NotMapped]
        public bool? IsStale
        {
            get => F.AsBool(F.Memo(this, "IsStale", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ReviewIntervalDays)), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DaysSinceUpdated), ">", F.Nullif(F.Of(this.ReviewIntervalDays))))))); set { }
        }

        // Formula IsExecutionFeedback (rulebook: ={{FedFromExecution}} <> "")
        [NotMapped]
        public bool? IsExecutionFeedback
        {
            get => F.AsBool(F.Memo(this, "IsExecutionFeedback", () => F.IsNotBlank(F.Of(this.FedFromExecution)))); set { }
        }

        // Formula IsMachineAuthored (rulebook: =AND({{AuthorAgent}} <> "", OR({{AuthorAgentKind}} = "AIAgent", {{AuthorAgentKind}} = "AutomatedPipeline")))
        [NotMapped]
        public bool? IsMachineAuthored
        {
            get => F.AsBool(F.Memo(this, "IsMachineAuthored", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AuthorAgent))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.AuthorAgentKind), F.S("AIAgent"))), F.Bool3(F.Eq(F.Of(this.AuthorAgentKind), F.S("AutomatedPipeline")))))))); set { }
        }

        // Formula OutlivesAuthorTenure (rulebook: =AND({{AuthorAgent}} <> "", {{AuthorIsStillEngaged}} = FALSE, {{IsStale}} = FALSE))
        [NotMapped]
        public bool? OutlivesAuthorTenure
        {
            get => F.AsBool(F.Memo(this, "OutlivesAuthorTenure", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AuthorAgent))), F.Bool3(F.Eq(F.Of(this.AuthorIsStillEngaged), F.B(false))), F.Bool3(F.Eq(F.Of(this.IsStale), F.B(false)))))); set { }
        }

        // Formula IsUncreditedExpertKnowHow (rulebook: =AND({{SourceExpert}} <> "", {{SourceExpert}} <> {{AuthorAgent}}, {{CreditsSourceExpert}} = FALSE))
        [NotMapped]
        public bool? IsUncreditedExpertKnowHow
        {
            get => F.AsBool(F.Memo(this, "IsUncreditedExpertKnowHow", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.SourceExpert))), F.Bool3(F.Ne(F.Nullif(F.Of(this.SourceExpert)), F.Nullif(F.Of(this.AuthorAgent)))), F.Bool3(F.Eq(F.Nullif(F.Of(this.CreditsSourceExpert)), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
        public string? KnowHow { get; set; }
        public string? AuthorAgent { get; set; }
        public string? SourceExpert { get; set; }
        public string? FedFromExecution { get; set; }
        public string? EvaluationContext { get; set; }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private KnowHowCarrier _knowHowCarrier;

        [ForeignKey("KnowHow")]
        public virtual KnowHowCarrier KnowHowCarrier
        {
            get
            {
                if (_knowHowCarrier == null && !string.IsNullOrEmpty(KnowHow))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarrier - no database context is set. KnowHow: " + KnowHow + ".");
                        }
                        return null;
                    }
                    _knowHowCarrier = base.SoAContext.KnowHowCarriers.Find(KnowHow);
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
                        KnowHow = _knowHowCarrier.KnowHowCarrierId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("AuthorAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AuthorAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AuthorAgent: " + AuthorAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AuthorAgent);
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
                        AuthorAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("SourceExpert")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(SourceExpert))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. SourceExpert: " + SourceExpert + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(SourceExpert);
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
                        SourceExpert = _agentRef.AgentId;
                    }
                }
            }
        }

        private ProcedureExecution _procedureExecution;

        [ForeignKey("FedFromExecution")]
        public virtual ProcedureExecution ProcedureExecution
        {
            get
            {
                if (_procedureExecution == null && !string.IsNullOrEmpty(FedFromExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecution - no database context is set. FedFromExecution: " + FedFromExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecution = base.SoAContext.ProcedureExecutions.Find(FedFromExecution);
                    if (_procedureExecution != null)
                    {
                        base.SoAContext.Attach(_procedureExecution);
                    }
                }
                return _procedureExecution;
            }
            set
            {
                if (_procedureExecution != value)
                {
                    _procedureExecution = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecution != null)
                    {
                        FedFromExecution = _procedureExecution.ProcedureExecutionId;
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

        private ObservableCollection<ProblemOccurrence> _problemOccurrences;

        [InverseProperty("KnowledgeRepositoryEntry")]
        public virtual ObservableCollection<ProblemOccurrence> ProblemOccurrences
        {
            get
            {
                if (_problemOccurrences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProblemOccurrences - no database context is set. KnowledgeRepositoryEntryId: " + this.KnowledgeRepositoryEntryId + ".");
                        }
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProblemOccurrences.Where(x => x.SolutionEntry == this.KnowledgeRepositoryEntryId).ToList<ProblemOccurrence>();
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _problemOccurrences.CollectionChanged += ProblemOccurrences_CollectionChanged;
                }
                return _problemOccurrences;
            }
            private set
            {
                if (_problemOccurrences != null)
                {
                    _problemOccurrences.CollectionChanged -= ProblemOccurrences_CollectionChanged;
                }
                _problemOccurrences = value;
                if (_problemOccurrences != null)
                {
                    _problemOccurrences.CollectionChanged += ProblemOccurrences_CollectionChanged;
                }
            }
        }

        private void ProblemOccurrences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProblemOccurrence>())
                {
                    item.SolutionEntry = this.KnowledgeRepositoryEntryId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.KnowHowCarrier;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.ProcedureExecution;
            _ = this.EvaluationContextRef;
            _ = this.ProblemOccurrences;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
