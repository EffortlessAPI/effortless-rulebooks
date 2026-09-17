
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
    [Table("StepConditions")]
    public class StepConditionBase : SoAEntityBase
    {
        [Key]
        public string StepConditionId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " " & {{ConditionKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" "), F.Text(F.Of(this.ConditionKind))))); set { }
        }

        public string? ConditionKind { get; set; }
        public string? Statement { get; set; }
        public bool? IsSafetyCritical { get; set; }
        // Formula CheckCount (rulebook: =COUNTIFS(ConditionChecks!{{StepCondition}}, {{StepConditionId}}))
        [NotMapped]
        public int? CheckCount
        {
            get => F.AsInt(F.Memo(this, "CheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConditionCheck>(base.SoAContext, "ConditionChecks", __c => __c.ConditionChecks), __r => F.CritField(F.Of(__r.StepCondition), F.Of(this.StepConditionId))))))); set { }
        }

        // Formula IsNeverChecked (rulebook: ={{CheckCount}} = 0)
        [NotMapped]
        public bool? IsNeverChecked
        {
            get => F.AsBool(F.Memo(this, "IsNeverChecked", () => F.Eq(F.Of(this.CheckCount), F.I(0)))); set { }
        }

        // Formula PreconditionStepKey (rulebook: =IF({{ConditionKind}} = "Precondition", {{Step}}, ""))
        [NotMapped]
        public string? PreconditionStepKey
        {
            get => F.AsString(F.Memo(this, "PreconditionStepKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConditionKind)), F.S("Precondition")))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula PostconditionStepKey (rulebook: =IF({{ConditionKind}} = "Postcondition", {{Step}}, ""))
        [NotMapped]
        public string? PostconditionStepKey
        {
            get => F.AsString(F.Memo(this, "PostconditionStepKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConditionKind)), F.S("Postcondition")))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula InvariantStepKey (rulebook: =IF({{ConditionKind}} = "Invariant", {{Step}}, ""))
        [NotMapped]
        public string? InvariantStepKey
        {
            get => F.AsString(F.Memo(this, "InvariantStepKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.ConditionKind)), F.S("Invariant")))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula SafetyCriticalStepKey (rulebook: =IF({{IsSafetyCritical}}, {{Step}}, ""))
        [NotMapped]
        public string? SafetyCriticalStepKey
        {
            get => F.AsString(F.Memo(this, "SafetyCriticalStepKey", () => (F.Truthy(F.IsTrueV(F.Of(this.IsSafetyCritical))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? MachineExpression { get; set; }
        // Formula IsMachineParseable (rulebook: ={{MachineExpression}} <> "")
        [NotMapped]
        public bool? IsMachineParseable
        {
            get => F.AsBool(F.Memo(this, "IsMachineParseable", () => F.IsNotBlank(F.Of(this.MachineExpression)))); set { }
        }


        public string? Step { get; set; }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
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
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private ObservableCollection<ConditionCheck> _conditionChecks;

        [InverseProperty("StepConditionRef")]
        public virtual ObservableCollection<ConditionCheck> ConditionChecks
        {
            get
            {
                if (_conditionChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConditionChecks - no database context is set. StepConditionId: " + this.StepConditionId + ".");
                        }
                        _conditionChecks = new ObservableCollection<ConditionCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConditionChecks.Where(x => x.StepCondition == this.StepConditionId).ToList<ConditionCheck>();
                        _conditionChecks = new ObservableCollection<ConditionCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _conditionChecks.CollectionChanged += ConditionChecks_CollectionChanged;
                }
                return _conditionChecks;
            }
            private set
            {
                if (_conditionChecks != null)
                {
                    _conditionChecks.CollectionChanged -= ConditionChecks_CollectionChanged;
                }
                _conditionChecks = value;
                if (_conditionChecks != null)
                {
                    _conditionChecks.CollectionChanged += ConditionChecks_CollectionChanged;
                }
            }
        }

        private void ConditionChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConditionCheck>())
                {
                    item.StepCondition = this.StepConditionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.ConditionChecks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
