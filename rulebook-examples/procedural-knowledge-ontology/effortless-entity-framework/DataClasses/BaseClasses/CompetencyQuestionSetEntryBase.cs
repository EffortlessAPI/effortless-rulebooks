
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
    [Table("CompetencyQuestionSetEntries")]
    public class CompetencyQuestionSetEntryBase : SoAEntityBase
    {
        [Key]
        public string CompetencyQuestionSetEntryId { get; set; }

        // Formula Name (rulebook: ={{GovernedModel}} & " / " & {{RoleQuestion}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.GovernedModel)), F.S(" / "), F.Text(F.Of(this.RoleQuestion))))); set { }
        }

        public bool? IsOriginalBaseline { get; set; }
        public DateTimeOffset? AddedAt { get; set; }
        public string? Status { get; set; }
        public string? RelevanceVerdict { get; set; }
        public bool? UsedForScoping { get; set; }
        public bool? UsedAsAcceptanceCriterion { get; set; }
        public bool? UsedAsTestDriver { get; set; }
        public bool? UsedForGovernance { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceAdded (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{AddedAt}}, "days"))
        [NotMapped]
        public int? DaysSinceAdded
        {
            get => F.AsInt(F.Memo(this, "DaysSinceAdded", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.AddedAt), F.S("days"))))); set { }
        }

        // Formula IsOutgrownBaselineQuestion (rulebook: =AND({{IsOriginalBaseline}}, {{DaysSinceAdded}} > 365, {{RelevanceVerdict}} = "NoLongerRelevant"))
        [NotMapped]
        public bool? IsOutgrownBaselineQuestion
        {
            get => F.AsBool(F.Memo(this, "IsOutgrownBaselineQuestion", () => F.And(F.IsTrueV(F.Of(this.IsOriginalBaseline)), F.Bool3(F.Cmp(F.Of(this.DaysSinceAdded), ">", F.I(365))), F.Bool3(F.Eq(F.Nullif(F.Of(this.RelevanceVerdict)), F.S("NoLongerRelevant")))))); set { }
        }

        // Formula IrrelevantButStillActive (rulebook: =AND({{RelevanceVerdict}} = "NoLongerRelevant", {{Status}} = "Active"))
        [NotMapped]
        public bool? IrrelevantButStillActive
        {
            get => F.AsBool(F.Memo(this, "IrrelevantButStillActive", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.RelevanceVerdict)), F.S("NoLongerRelevant"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Active")))))); set { }
        }

        // Formula GovernanceUseCount (rulebook: =IF({{UsedForScoping}}, 1, 0) + IF({{UsedAsAcceptanceCriterion}}, 1, 0) + IF({{UsedAsTestDriver}}, 1, 0) + IF({{UsedForGovernance}}, 1, 0))
        [NotMapped]
        public int? GovernanceUseCount
        {
            get => F.AsInt(F.Memo(this, "GovernanceUseCount", () => F.Integer(F.Add(F.Add(F.Add((F.Truthy(F.IsTrueV(F.Of(this.UsedForScoping))) ? F.I(1) : F.I(0)), (F.Truthy(F.IsTrueV(F.Of(this.UsedAsAcceptanceCriterion))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.UsedAsTestDriver))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.UsedForGovernance))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula ServesEveryGovernanceUse (rulebook: ={{GovernanceUseCount}} = 4)
        [NotMapped]
        public bool? ServesEveryGovernanceUse
        {
            get => F.AsBool(F.Memo(this, "ServesEveryGovernanceUse", () => F.Eq(F.Of(this.GovernanceUseCount), F.I(4)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? RoleQuestion { get; set; }
        public string? EvaluationContext { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private RoleQuestion _roleQuestionRef;

        [ForeignKey("RoleQuestion")]
        public virtual RoleQuestion RoleQuestionRef
        {
            get
            {
                if (_roleQuestionRef == null && !string.IsNullOrEmpty(RoleQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestionRef - no database context is set. RoleQuestion: " + RoleQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestionRef = base.SoAContext.RoleQuestions.Find(RoleQuestion);
                    if (_roleQuestionRef != null)
                    {
                        base.SoAContext.Attach(_roleQuestionRef);
                    }
                }
                return _roleQuestionRef;
            }
            set
            {
                if (_roleQuestionRef != value)
                {
                    _roleQuestionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleQuestionRef != null)
                    {
                        RoleQuestion = _roleQuestionRef.RoleQuestionId;
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

        private ObservableCollection<CompetencyQuestionRun> _competencyQuestionRuns;

        [InverseProperty("CompetencyQuestionSetEntry")]
        public virtual ObservableCollection<CompetencyQuestionRun> CompetencyQuestionRuns
        {
            get
            {
                if (_competencyQuestionRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionRuns - no database context is set. CompetencyQuestionSetEntryId: " + this.CompetencyQuestionSetEntryId + ".");
                        }
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionRuns.Where(x => x.CqSetEntry == this.CompetencyQuestionSetEntryId).ToList<CompetencyQuestionRun>();
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestionRuns.CollectionChanged += CompetencyQuestionRuns_CollectionChanged;
                }
                return _competencyQuestionRuns;
            }
            private set
            {
                if (_competencyQuestionRuns != null)
                {
                    _competencyQuestionRuns.CollectionChanged -= CompetencyQuestionRuns_CollectionChanged;
                }
                _competencyQuestionRuns = value;
                if (_competencyQuestionRuns != null)
                {
                    _competencyQuestionRuns.CollectionChanged += CompetencyQuestionRuns_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestionRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestionRun>())
                {
                    item.CqSetEntry = this.CompetencyQuestionSetEntryId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.RoleQuestionRef;
            _ = this.EvaluationContextRef;
            _ = this.CompetencyQuestionRuns;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
