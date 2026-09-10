
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("ReviewEvents")]
    public class ReviewEventBase : SoAEntityBase
    {
        [Key]
        public string ReviewEventId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " / " & {{ReviewKind}})
        public string? Name
        {
            get => this.ProcedureVersion + " / " + this.ReviewKind; set { }
        }

        public string? ReviewKind { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Outcome { get; set; }
        public DateTime? NextReviewDue { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula IsOverdue (rulebook: ={{NextReviewDue}} < {{AsOfInstant}})
        public bool? IsOverdue
        {
            get => this.NextReviewDue < this.AsOfInstant; set { }
        }

        // Formula OverdueVersionKey (rulebook: =IF({{IsOverdue}}, {{ProcedureVersion}}, ""))
        public string? OverdueVersionKey
        {
            get => IF(this.IsOverdue, this.ProcedureVersion, ""); set { }
        }

        // Formula PromisedCadenceDays (rulebook: =INDEX(ProcedureVersions!{{StewardReviewCadenceDays}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        public int? PromisedCadenceDays
        {
            get => INDEX(ProcedureVersions!this.StewardReviewCadenceDays, MATCH(this.ProcedureVersion, ProcedureVersions!this.ProcedureVersionId, 0)); set { }
        }

        // Formula DaysSinceReviewed (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{ReviewedAt}}, "days"))
        public int? DaysSinceReviewed
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.ReviewedAt, "days"); set { }
        }

        // Formula ExceedsPromisedCadence (rulebook: ={{DaysSinceReviewed}} > {{PromisedCadenceDays}})
        public bool? ExceedsPromisedCadence
        {
            get => this.DaysSinceReviewed > this.PromisedCadenceDays; set { }
        }

        // Formula CadenceDriftDays (rulebook: ={{DaysSinceReviewed}} - {{PromisedCadenceDays}})
        public int? CadenceDriftDays
        {
            get => this.DaysSinceReviewed - this.PromisedCadenceDays; set { }
        }

        // Formula PromiseAndBehaviorDisagree (rulebook: =AND({{ExceedsPromisedCadence}}, NOT({{IsOverdue}})))
        public bool? PromiseAndBehaviorDisagree
        {
            get => AND(this.ExceedsPromisedCadence, NOT(this.IsOverdue)); set { }
        }

        // Formula CadenceBreachVersionKey (rulebook: =IF({{ExceedsPromisedCadence}}, {{ProcedureVersion}}, ""))
        public string? CadenceBreachVersionKey
        {
            get => IF(this.ExceedsPromisedCadence, this.ProcedureVersion, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? ReviewedByAgent { get; set; }
        public string? RelatedChangeRequest { get; set; }
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

        [ForeignKey("ReviewedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ReviewedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ReviewedByAgent: " + ReviewedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(ReviewedByAgent);
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
                    ReviewedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private ChangeRequest _changeRequest;

        [ForeignKey("RelatedChangeRequest")]
        public virtual ChangeRequest ChangeRequest
        {
            get
            {
                if (_changeRequest == null && !string.IsNullOrEmpty(RelatedChangeRequest))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequest - no database context is set. RelatedChangeRequest: " + RelatedChangeRequest + ".");
                        }
                        return null;
                    }
                    _changeRequest = Context.ChangeRequests.Find(RelatedChangeRequest);
                    if (_changeRequest != null)
                    {
                        Context.Attach(_changeRequest);
                    }
                }
                return _changeRequest;
            }
            set
            {
                if (_changeRequest != value)
                {
                    _changeRequest = value;
                    RelatedChangeRequest = _changeRequest == null ? default : _changeRequest.ChangeRequestId;
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
            _ = this.ProcedureVersion;
            _ = this.Agent;
            _ = this.ChangeRequest;
            _ = this.EvaluationContext;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
