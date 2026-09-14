
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
    [Table("StepTransitions")]
    public class StepTransitionBase : SoAEntityBase
    {
        [Key]
        public string StepTransitionId { get; set; }

        // Formula Name (rulebook: ={{FromStep}} & " -> " & {{ToStep}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.FromStep)), F.S(" -> "), F.Text(F.Of(this.ToStep))))); set { }
        }

        public string? TransitionKind { get; set; }
        public string? Condition { get; set; }
        public int? Priority { get; set; }
        // Formula IsRecoveryPath (rulebook: =OR({{TransitionKind}} = "Fallback", {{TransitionKind}} = "Alternative"))
        [NotMapped]
        public bool? IsRecoveryPath
        {
            get => F.AsBool(F.Memo(this, "IsRecoveryPath", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.TransitionKind)), F.S("Fallback"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.TransitionKind)), F.S("Alternative")))))); set { }
        }

        // Formula CountOfFromStepExecutions (rulebook: =COUNTIFS(StepExecutions!{{Step}}, StepTransitions!{{FromStep}}))
        [NotMapped]
        public int? CountOfFromStepExecutions
        {
            get => F.AsInt(F.Memo(this, "CountOfFromStepExecutions", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.Step), F.Of(this.FromStep))))))); set { }
        }

        // Formula CountOfToStepExecutions (rulebook: =COUNTIFS(StepExecutions!{{Step}}, StepTransitions!{{ToStep}}))
        [NotMapped]
        public int? CountOfToStepExecutions
        {
            get => F.AsInt(F.Memo(this, "CountOfToStepExecutions", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.Step), F.Of(this.ToStep))))))); set { }
        }

        // Formula HasReachableOrigin (rulebook: ={{CountOfFromStepExecutions}} > 0)
        [NotMapped]
        public bool? HasReachableOrigin
        {
            get => F.AsBool(F.Memo(this, "HasReachableOrigin", () => F.Cmp(F.Of(this.CountOfFromStepExecutions), ">", F.I(0)))); set { }
        }

        // Formula HasReachableTarget (rulebook: ={{CountOfToStepExecutions}} > 0)
        [NotMapped]
        public bool? HasReachableTarget
        {
            get => F.AsBool(F.Memo(this, "HasReachableTarget", () => F.Cmp(F.Of(this.CountOfToStepExecutions), ">", F.I(0)))); set { }
        }

        // Formula IsNeverExercised (rulebook: =NOT(AND({{HasReachableOrigin}}, {{HasReachableTarget}})))
        [NotMapped]
        public bool? IsNeverExercised
        {
            get => F.AsBool(F.Memo(this, "IsNeverExercised", () => F.Not(F.Bool3(F.And(F.Bool3(F.Of(this.HasReachableOrigin)), F.Bool3(F.Of(this.HasReachableTarget))))))); set { }
        }

        // Formula IsUntestedRecoveryPath (rulebook: =AND({{IsRecoveryPath}}, {{IsNeverExercised}}))
        [NotMapped]
        public bool? IsUntestedRecoveryPath
        {
            get => F.AsBool(F.Memo(this, "IsUntestedRecoveryPath", () => F.And(F.Bool3(F.Of(this.IsRecoveryPath)), F.Bool3(F.Of(this.IsNeverExercised))))); set { }
        }

        // Formula CountOfObservedTraversals (rulebook: =COUNTIFS(ObservedTransitions!{{StepTransition}}, StepTransitions!{{StepTransitionId}}))
        [NotMapped]
        public int? CountOfObservedTraversals
        {
            get => F.AsInt(F.Memo(this, "CountOfObservedTraversals", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ObservedTransition>(base.SoAContext, "ObservedTransitions", __c => __c.ObservedTransitions), __r => F.CritField(F.Of(__r.StepTransition), F.Of(this.StepTransitionId))))))); set { }
        }

        // Formula HasBeenTraversed (rulebook: ={{CountOfObservedTraversals}} > 0)
        [NotMapped]
        public bool? HasBeenTraversed
        {
            get => F.AsBool(F.Memo(this, "HasBeenTraversed", () => F.Cmp(F.Of(this.CountOfObservedTraversals), ">", F.I(0)))); set { }
        }

        // Formula IsUnwalkedRecoveryPath (rulebook: =AND({{IsRecoveryPath}}, NOT({{HasBeenTraversed}})))
        [NotMapped]
        public bool? IsUnwalkedRecoveryPath
        {
            get => F.AsBool(F.Memo(this, "IsUnwalkedRecoveryPath", () => F.And(F.Bool3(F.Of(this.IsRecoveryPath)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasBeenTraversed))))))); set { }
        }

        // Formula TargetBlockingRequirementCount (rulebook: =INDEX(Steps!{{BlockingRequirementCount}}, MATCH({{ToStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public decimal? TargetBlockingRequirementCount
        {
            get => F.AsDecimal(F.Memo(this, "TargetBlockingRequirementCount", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.ToStep), __r => F.Of(__r.BlockingRequirementCount), () => F.Of(new Step().BlockingRequirementCount)))); set { }
        }

        // Formula TargetCarriesBlockingControl (rulebook: ={{TargetBlockingRequirementCount}} > 0)
        [NotMapped]
        public bool? TargetCarriesBlockingControl
        {
            get => F.AsBool(F.Memo(this, "TargetCarriesBlockingControl", () => F.Cmp(F.Of(this.TargetBlockingRequirementCount), ">", F.I(0)))); set { }
        }

        // Formula IsUnrehearsedControlEntry (rulebook: =AND({{IsUnwalkedRecoveryPath}}, {{TargetCarriesBlockingControl}}))
        [NotMapped]
        public bool? IsUnrehearsedControlEntry
        {
            get => F.AsBool(F.Memo(this, "IsUnrehearsedControlEntry", () => F.And(F.Bool3(F.Of(this.IsUnwalkedRecoveryPath)), F.Bool3(F.Of(this.TargetCarriesBlockingControl))))); set { }
        }

        // Formula UnrehearsedControlVersionKey (rulebook: =IF({{IsUnrehearsedControlEntry}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? UnrehearsedControlVersionKey
        {
            get => F.AsString(F.Memo(this, "UnrehearsedControlVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnrehearsedControlEntry))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        public string? LeadsToClosure { get; set; }
        // Formula FromStepIsHumanApprovalGate (rulebook: =INDEX(Steps!{{IsHumanApprovalGate}}, MATCH({{FromStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? FromStepIsHumanApprovalGate
        {
            get => F.AsBool(F.Memo(this, "FromStepIsHumanApprovalGate", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.FromStep), __r => F.Of(__r.IsHumanApprovalGate), () => F.Of(new Step().IsHumanApprovalGate)))); set { }
        }

        // Formula ToStepIsHumanApprovalGate (rulebook: =INDEX(Steps!{{IsHumanApprovalGate}}, MATCH({{ToStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? ToStepIsHumanApprovalGate
        {
            get => F.AsBool(F.Memo(this, "ToStepIsHumanApprovalGate", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.ToStep), __r => F.Of(__r.IsHumanApprovalGate), () => F.Of(new Step().IsHumanApprovalGate)))); set { }
        }

        // Formula AvoidsHumanApprovalGate (rulebook: =AND(NOT({{FromStepIsHumanApprovalGate}}), NOT({{ToStepIsHumanApprovalGate}})))
        [NotMapped]
        public bool? AvoidsHumanApprovalGate
        {
            get => F.AsBool(F.Memo(this, "AvoidsHumanApprovalGate", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.FromStepIsHumanApprovalGate)))), F.Bool3(F.Not(F.Bool3(F.Of(this.ToStepIsHumanApprovalGate))))))); set { }
        }

        public string? LeadsWithoutHumanGateClosure { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? FromStep { get; set; }
        public string? ToStep { get; set; }

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

        private ObservableCollection<ObservedTransition> _observedTransitions;

        [InverseProperty("StepTransitionRef")]
        public virtual ObservableCollection<ObservedTransition> ObservedTransitions
        {
            get
            {
                if (_observedTransitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedTransitions - no database context is set. StepTransitionId: " + this.StepTransitionId + ".");
                        }
                        _observedTransitions = new ObservableCollection<ObservedTransition>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedTransitions.Where(x => x.StepTransition == this.StepTransitionId).ToList<ObservedTransition>();
                        _observedTransitions = new ObservableCollection<ObservedTransition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedTransitions.CollectionChanged += ObservedTransitions_CollectionChanged;
                }
                return _observedTransitions;
            }
            private set
            {
                if (_observedTransitions != null)
                {
                    _observedTransitions.CollectionChanged -= ObservedTransitions_CollectionChanged;
                }
                _observedTransitions = value;
                if (_observedTransitions != null)
                {
                    _observedTransitions.CollectionChanged += ObservedTransitions_CollectionChanged;
                }
            }
        }

        private void ObservedTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ObservedTransition>())
                {
                    item.StepTransition = this.StepTransitionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Step;
            _ = this.StepRef;
            _ = this.ObservedTransitions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
