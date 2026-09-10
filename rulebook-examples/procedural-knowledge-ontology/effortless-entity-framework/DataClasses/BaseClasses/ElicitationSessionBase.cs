
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ElicitationSessions")]
    public class ElicitationSessionBase : SoAEntityBase
    {
        [Key]
        public string ElicitationSessionId { get; set; }

        // Formula Name (rulebook: ={{Method}} & " / " & {{StartedAt}})
        public string? Name
        {
            get => this.Method + " / " + this.StartedAt; set { }
        }

        public string? Method { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? Summary { get; set; }
        public string? Status { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula DaysSinceElicited (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{EndedAt}}, "days"))
        public int? DaysSinceElicited
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.EndedAt, "days"); set { }
        }

        // Formula IsSingleWitnessMethod (rulebook: =OR({{Method}} = "Shadowing", {{Method}} = "PractitionerInterview"))
        public bool? IsSingleWitnessMethod
        {
            get => OR(this.Method = "Shadowing", this.Method = "PractitionerInterview"); set { }
        }

        // Formula PractitionerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{PractitionerAgent}}, Agents!{{AgentId}}, 0)))
        public bool? PractitionerIsStillEngaged
        {
            get => INDEX(Agents!this.IsStillEngaged, MATCH(this.PractitionerAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula ValidFragmentsProduced (rulebook: =COUNTIFS(KnowledgeFragments!{{ValidFragmentSessionKey}}, {{ElicitationSessionId}}))
        public decimal? ValidFragmentsProduced
        {
            get => COUNTIFS(KnowledgeFragments!this.ValidFragmentSessionKey, this.ElicitationSessionId); set { }
        }

        // Formula IsHighYieldSession (rulebook: ={{ValidFragmentsProduced}} >= 3)
        public bool? IsHighYieldSession
        {
            get => this.ValidFragmentsProduced >= 3; set { }
        }

        // Formula IsConcentratedSingleWitness (rulebook: =AND({{IsSingleWitnessMethod}}, {{IsHighYieldSession}}))
        public bool? IsConcentratedSingleWitness
        {
            get => AND(this.IsSingleWitnessMethod, this.IsHighYieldSession); set { }
        }

        // Formula IsStaleConcentratedWitness (rulebook: =AND({{IsConcentratedSingleWitness}}, {{DaysSinceElicited}} > 180))
        public bool? IsStaleConcentratedWitness
        {
            get => AND(this.IsConcentratedSingleWitness, this.DaysSinceElicited > 180); set { }
        }

        // Formula ConcentratedSessionVersionKey (rulebook: =IF({{IsConcentratedSingleWitness}}, {{ProcedureVersion}}, ""))
        public string? ConcentratedSessionVersionKey
        {
            get => IF(this.IsConcentratedSingleWitness, this.ProcedureVersion, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? PractitionerAgent { get; set; }
        public string? FacilitatorAgent { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. PractitionerAgent: " + PractitionerAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(PractitionerAgent);
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
                    PractitionerAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("FacilitatorAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(FacilitatorAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. FacilitatorAgent: " + FacilitatorAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(FacilitatorAgent);
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
                    FacilitatorAgent = _agent == null ? default : _agent.AgentId;
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

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("ElicitationSession")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. ElicitationSessionId: " + this.ElicitationSessionId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = Context.KnowledgeFragments.Where(x => x.ElicitationSession == this.ElicitationSessionId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.ProcedureVersion;
            _ = this.Agent;
            _ = this.Agent;
            _ = this.EvaluationContext;
            _ = this.KnowledgeFragments;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
