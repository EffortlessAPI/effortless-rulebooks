
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("StepTransitions")]
    public class StepTransitionBase : SoAEntityBase
    {
        [Key]
        public string StepTransitionId { get; set; }

        // Formula Name (rulebook: ={{FromStep}} & " -> " & {{ToStep}})
        public string? Name
        {
            get => this.FromStep + " -> " + this.ToStep; set { }
        }

        public string? TransitionKind { get; set; }
        public string? Condition { get; set; }
        public int? Priority { get; set; }
        // Formula IsRecoveryPath (rulebook: =OR({{TransitionKind}} = "Fallback", {{TransitionKind}} = "Alternative"))
        public bool? IsRecoveryPath
        {
            get => OR(this.TransitionKind = "Fallback", this.TransitionKind = "Alternative"); set { }
        }

        // Formula CountOfFromStepExecutions (rulebook: =COUNTIFS(StepExecutions!{{Step}}, StepTransitions!{{FromStep}}))
        public int? CountOfFromStepExecutions
        {
            get => this.StepExecutions == null ? 0 : this.StepExecutions.Count; set { }
        }

        // Formula CountOfToStepExecutions (rulebook: =COUNTIFS(StepExecutions!{{Step}}, StepTransitions!{{ToStep}}))
        public int? CountOfToStepExecutions
        {
            get => this.StepExecutions == null ? 0 : this.StepExecutions.Count; set { }
        }

        // Formula HasReachableOrigin (rulebook: ={{CountOfFromStepExecutions}} > 0)
        public bool? HasReachableOrigin
        {
            get => this.CountOfFromStepExecutions > 0; set { }
        }

        // Formula HasReachableTarget (rulebook: ={{CountOfToStepExecutions}} > 0)
        public bool? HasReachableTarget
        {
            get => this.CountOfToStepExecutions > 0; set { }
        }

        // Formula IsNeverExercised (rulebook: =NOT(AND({{HasReachableOrigin}}, {{HasReachableTarget}})))
        public bool? IsNeverExercised
        {
            get => NOT(AND(this.HasReachableOrigin, this.HasReachableTarget)); set { }
        }

        // Formula IsUntestedRecoveryPath (rulebook: =AND({{IsRecoveryPath}}, {{IsNeverExercised}}))
        public bool? IsUntestedRecoveryPath
        {
            get => AND(this.IsRecoveryPath, this.IsNeverExercised); set { }
        }

        // Formula CountOfObservedTraversals (rulebook: =COUNTIFS(ObservedTransitions!{{StepTransition}}, StepTransitions!{{StepTransitionId}}))
        public int? CountOfObservedTraversals
        {
            get => this.ObservedTransitions == null ? 0 : this.ObservedTransitions.Count; set { }
        }

        // Formula HasBeenTraversed (rulebook: ={{CountOfObservedTraversals}} > 0)
        public bool? HasBeenTraversed
        {
            get => this.CountOfObservedTraversals > 0; set { }
        }

        // Formula IsUnwalkedRecoveryPath (rulebook: =AND({{IsRecoveryPath}}, NOT({{HasBeenTraversed}})))
        public bool? IsUnwalkedRecoveryPath
        {
            get => AND(this.IsRecoveryPath, NOT(this.HasBeenTraversed)); set { }
        }

        // Formula TargetBlockingRequirementCount (rulebook: =INDEX(Steps!{{BlockingRequirementCount}}, MATCH({{ToStep}}, Steps!{{StepId}}, 0)))
        public decimal? TargetBlockingRequirementCount
        {
            get => INDEX(Steps!this.BlockingRequirementCount, MATCH(this.ToStep, Steps!this.StepId, 0)); set { }
        }

        // Formula TargetCarriesBlockingControl (rulebook: ={{TargetBlockingRequirementCount}} > 0)
        public bool? TargetCarriesBlockingControl
        {
            get => this.TargetBlockingRequirementCount > 0; set { }
        }

        // Formula IsUnrehearsedControlEntry (rulebook: =AND({{IsUnwalkedRecoveryPath}}, {{TargetCarriesBlockingControl}}))
        public bool? IsUnrehearsedControlEntry
        {
            get => AND(this.IsUnwalkedRecoveryPath, this.TargetCarriesBlockingControl); set { }
        }

        // Formula UnrehearsedControlVersionKey (rulebook: =IF({{IsUnrehearsedControlEntry}}, {{ProcedureVersion}}, ""))
        public string? UnrehearsedControlVersionKey
        {
            get => IF(this.IsUnrehearsedControlEntry, this.ProcedureVersion, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? FromStep { get; set; }
        public string? ToStep { get; set; }

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

        private Step _step;

        [ForeignKey("FromStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(FromStep))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. FromStep: " + FromStep + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(FromStep);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    FromStep = _step == null ? default : _step.StepId;
                }
            }
        }

        private Step _step;

        [ForeignKey("ToStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(ToStep))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. ToStep: " + ToStep + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(ToStep);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    ToStep = _step == null ? default : _step.StepId;
                }
            }
        }

        private ObservableCollection<ObservedTransition> _observedTransitions;

        [InverseProperty("StepTransition")]
        public virtual ObservableCollection<ObservedTransition> ObservedTransitions
        {
            get
            {
                if (_observedTransitions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedTransitions - no database context is set. StepTransitionId: " + this.StepTransitionId + ".");
                        }
                        _observedTransitions = new ObservableCollection<ObservedTransition>();
                    }
                    else
                    {
                        var items = Context.ObservedTransitions.Where(x => x.StepTransition == this.StepTransitionId).ToList<ObservedTransition>();
                        _observedTransitions = new ObservableCollection<ObservedTransition>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.Step;
            _ = this.ObservedTransitions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
