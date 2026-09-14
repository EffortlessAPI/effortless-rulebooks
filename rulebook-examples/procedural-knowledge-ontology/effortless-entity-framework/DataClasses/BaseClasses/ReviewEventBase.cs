
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
    [Table("ReviewEvents")]
    public class ReviewEventBase : SoAEntityBase
    {
        [Key]
        public string ReviewEventId { get; set; }

        // Formula Name (rulebook: ={{ProcedureVersion}} & " / " & {{ReviewKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureVersion)), F.S(" / "), F.Text(F.Of(this.ReviewKind))))); set { }
        }

        public string? ReviewKind { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
        public string? Outcome { get; set; }
        public DateTimeOffset? NextReviewDue { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsOverdue (rulebook: ={{NextReviewDue}} < {{AsOfInstant}})
        [NotMapped]
        public bool? IsOverdue
        {
            get => F.AsBool(F.Memo(this, "IsOverdue", () => F.Cmp(F.Nullif(F.Of(this.NextReviewDue)), "<", F.Of(this.AsOfInstant)))); set { }
        }

        // Formula OverdueVersionKey (rulebook: =IF({{IsOverdue}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? OverdueVersionKey
        {
            get => F.AsString(F.Memo(this, "OverdueVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsOverdue))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula PromisedCadenceDays (rulebook: =INDEX(ProcedureVersions!{{StewardReviewCadenceDays}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public int? PromisedCadenceDays
        {
            get => F.AsInt(F.Memo(this, "PromisedCadenceDays", () => F.Integer(F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.StewardReviewCadenceDays), () => F.Of(new ProcedureVersion().StewardReviewCadenceDays))))); set { }
        }

        // Formula DaysSinceReviewed (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{ReviewedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceReviewed
        {
            get => F.AsInt(F.Memo(this, "DaysSinceReviewed", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ReviewedAt), F.S("days"))))); set { }
        }

        // Formula ExceedsPromisedCadence (rulebook: ={{DaysSinceReviewed}} > {{PromisedCadenceDays}})
        [NotMapped]
        public bool? ExceedsPromisedCadence
        {
            get => F.AsBool(F.Memo(this, "ExceedsPromisedCadence", () => F.Cmp(F.Of(this.DaysSinceReviewed), ">", F.Of(this.PromisedCadenceDays)))); set { }
        }

        // Formula CadenceDriftDays (rulebook: ={{DaysSinceReviewed}} - {{PromisedCadenceDays}})
        [NotMapped]
        public int? CadenceDriftDays
        {
            get => F.AsInt(F.Memo(this, "CadenceDriftDays", () => F.Integer(F.Sub(F.Of(this.DaysSinceReviewed), F.Of(this.PromisedCadenceDays))))); set { }
        }

        // Formula PromiseAndBehaviorDisagree (rulebook: =AND({{ExceedsPromisedCadence}}, NOT({{IsOverdue}})))
        [NotMapped]
        public bool? PromiseAndBehaviorDisagree
        {
            get => F.AsBool(F.Memo(this, "PromiseAndBehaviorDisagree", () => F.And(F.Bool3(F.Of(this.ExceedsPromisedCadence)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsOverdue))))))); set { }
        }

        // Formula CadenceBreachVersionKey (rulebook: =IF({{ExceedsPromisedCadence}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? CadenceBreachVersionKey
        {
            get => F.AsString(F.Memo(this, "CadenceBreachVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.ExceedsPromisedCadence))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? ReviewedByAgent { get; set; }
        public string? RelatedChangeRequest { get; set; }
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

        [ForeignKey("ReviewedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ReviewedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ReviewedByAgent: " + ReviewedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ReviewedByAgent);
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
                        ReviewedByAgent = _agent.AgentId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequest - no database context is set. RelatedChangeRequest: " + RelatedChangeRequest + ".");
                        }
                        return null;
                    }
                    _changeRequest = base.SoAContext.ChangeRequests.Find(RelatedChangeRequest);
                    if (_changeRequest != null)
                    {
                        base.SoAContext.Attach(_changeRequest);
                    }
                }
                return _changeRequest;
            }
            set
            {
                if (_changeRequest != value)
                {
                    _changeRequest = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_changeRequest != null)
                    {
                        RelatedChangeRequest = _changeRequest.ChangeRequestId;
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
            _ = this.ProcedureVersionRef;
            _ = this.Agent;
            _ = this.ChangeRequest;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
