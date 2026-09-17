
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
    [Table("MinedFlowEdges")]
    public class MinedFlowEdgeBase : SoAEntityBase
    {
        [Key]
        public string MinedFlowEdgeId { get; set; }

        // Formula Name (rulebook: ={{FromStep}} & " -> " & {{ToStep}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FromStep)), F.S(" -> "), F.Text(F.Of(this.ToStep))))); set { }
        }

        public int? ObservedCaseCount { get; set; }
        public int? MedianWaitMinutes { get; set; }
        public string? RecordedStance { get; set; }
        // Formula DocumentedTransitionCount (rulebook: =COUNTIFS(StepTransitions!{{FromStep}}, {{FromStep}}, StepTransitions!{{ToStep}}, {{ToStep}}))
        [NotMapped]
        public int? DocumentedTransitionCount
        {
            get => F.AsInt(F.Memo(this, "DocumentedTransitionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepTransition>(base.SoAContext, "StepTransitions", __c => __c.StepTransitions), __r => F.CritField(F.Of(__r.FromStep), F.Of(this.FromStep)) && F.CritField(F.Of(__r.ToStep), F.Of(this.ToStep))))))); set { }
        }

        // Formula IsUndocumentedPath (rulebook: ={{DocumentedTransitionCount}} = 0)
        [NotMapped]
        public bool? IsUndocumentedPath
        {
            get => F.AsBool(F.Memo(this, "IsUndocumentedPath", () => F.Eq(F.Of(this.DocumentedTransitionCount), F.I(0)))); set { }
        }

        // Formula ToStepExpectedMinutes (rulebook: =INDEX(Steps!{{ExpectedDurationMinutes}}, MATCH({{ToStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? ToStepExpectedMinutes
        {
            get => F.AsInt(F.Memo(this, "ToStepExpectedMinutes", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.ToStep), __r => F.Of(__r.ExpectedDurationMinutes), () => F.Of(new Step().ExpectedDurationMinutes))))); set { }
        }

        // Formula IsBottleneck (rulebook: =AND({{ToStepExpectedMinutes}} > 0, {{MedianWaitMinutes}} > 4 * {{ToStepExpectedMinutes}}))
        [NotMapped]
        public bool? IsBottleneck
        {
            get => F.AsBool(F.Memo(this, "IsBottleneck", () => F.And(F.Bool3(F.Cmp(F.Of(this.ToStepExpectedMinutes), ">", F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.MedianWaitMinutes)), ">", F.Mul(F.I(4), F.Of(this.ToStepExpectedMinutes))))))); set { }
        }

        // Formula IsMinedPathRecordedAsIntentWithoutDecision (rulebook: =AND({{RecordedStance}} = "Intended", {{IntentDecisionBy}} = ""))
        [NotMapped]
        public bool? IsMinedPathRecordedAsIntentWithoutDecision
        {
            get => F.AsBool(F.Memo(this, "IsMinedPathRecordedAsIntentWithoutDecision", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.RecordedStance)), F.S("Intended"))), F.Bool3(F.IsBlank(F.Of(this.IntentDecisionBy)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcessMiningRun { get; set; }
        public string? FromStep { get; set; }
        public string? ToStep { get; set; }
        public string? IntentDecisionBy { get; set; }

        private ProcessMiningRun _processMiningRunRef;

        [ForeignKey("ProcessMiningRun")]
        public virtual ProcessMiningRun ProcessMiningRunRef
        {
            get
            {
                if (_processMiningRunRef == null && !string.IsNullOrEmpty(ProcessMiningRun))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessMiningRunRef - no database context is set. ProcessMiningRun: " + ProcessMiningRun + ".");
                        }
                        return null;
                    }
                    _processMiningRunRef = base.SoAContext.ProcessMiningRuns.Find(ProcessMiningRun);
                    if (_processMiningRunRef != null)
                    {
                        base.SoAContext.Attach(_processMiningRunRef);
                    }
                }
                return _processMiningRunRef;
            }
            set
            {
                if (_processMiningRunRef != value)
                {
                    _processMiningRunRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_processMiningRunRef != null)
                    {
                        ProcessMiningRun = _processMiningRunRef.ProcessMiningRunId;
                    }
                }
            }
        }

        private Step _step;

        [ForeignKey("FromStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(FromStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. FromStep: " + FromStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(FromStep);
                    if (_step != null)
                    {
                        base.SoAContext.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_step != null)
                    {
                        FromStep = _step.StepId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("ToStep")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(ToStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. ToStep: " + ToStep + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(ToStep);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        ToStep = _stepRef.StepId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("IntentDecisionBy")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(IntentDecisionBy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. IntentDecisionBy: " + IntentDecisionBy + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(IntentDecisionBy);
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
                        IntentDecisionBy = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcessMiningRunRef;
            _ = this.Step;
            _ = this.StepRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
