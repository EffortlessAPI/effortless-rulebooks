
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
    [Table("StepCues")]
    public class StepCueBase : SoAEntityBase
    {
        [Key]
        public string StepCueId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " " & {{CueKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" "), F.Text(F.Of(this.CueKind))))); set { }
        }

        public string? CueKind { get; set; }
        public string? Description { get; set; }
        public bool? SignalsIncompleteStep { get; set; }
        public bool? RequiresEscalation { get; set; }
        // Formula ObservationCount (rulebook: =COUNTIFS(CueObservations!{{StepCue}}, {{StepCueId}}))
        [NotMapped]
        public int? ObservationCount
        {
            get => F.AsInt(F.Memo(this, "ObservationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CueObservation>(base.SoAContext, "CueObservations", __c => __c.CueObservations), __r => F.CritField(F.Of(__r.StepCue), F.Of(this.StepCueId))))))); set { }
        }

        // Formula UnescalatedObservationCount (rulebook: =COUNTIFS(CueObservations!{{UnescalatedCueKey}}, {{StepCueId}}))
        [NotMapped]
        public int? UnescalatedObservationCount
        {
            get => F.AsInt(F.Memo(this, "UnescalatedObservationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CueObservation>(base.SoAContext, "CueObservations", __c => __c.CueObservations), __r => F.CritField(F.Of(__r.UnescalatedCueKey), F.Of(this.StepCueId))))))); set { }
        }

        // Formula DangerCueStepKey (rulebook: =IF({{RequiresEscalation}}, {{Step}}, ""))
        [NotMapped]
        public string? DangerCueStepKey
        {
            get => F.AsString(F.Memo(this, "DangerCueStepKey", () => (F.Truthy(F.IsTrueV(F.Of(this.RequiresEscalation))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula IncompleteCueStepKey (rulebook: =IF({{SignalsIncompleteStep}}, {{Step}}, ""))
        [NotMapped]
        public string? IncompleteCueStepKey
        {
            get => F.AsString(F.Memo(this, "IncompleteCueStepKey", () => (F.Truthy(F.IsTrueV(F.Of(this.SignalsIncompleteStep))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? EscalateToRole { get; set; }

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

        private Role _role;

        [ForeignKey("EscalateToRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(EscalateToRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. EscalateToRole: " + EscalateToRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(EscalateToRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        EscalateToRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<CueObservation> _cueObservations;

        [InverseProperty("StepCueRef")]
        public virtual ObservableCollection<CueObservation> CueObservations
        {
            get
            {
                if (_cueObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CueObservations - no database context is set. StepCueId: " + this.StepCueId + ".");
                        }
                        _cueObservations = new ObservableCollection<CueObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.CueObservations.Where(x => x.StepCue == this.StepCueId).ToList<CueObservation>();
                        _cueObservations = new ObservableCollection<CueObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _cueObservations.CollectionChanged += CueObservations_CollectionChanged;
                }
                return _cueObservations;
            }
            private set
            {
                if (_cueObservations != null)
                {
                    _cueObservations.CollectionChanged -= CueObservations_CollectionChanged;
                }
                _cueObservations = value;
                if (_cueObservations != null)
                {
                    _cueObservations.CollectionChanged += CueObservations_CollectionChanged;
                }
            }
        }

        private void CueObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CueObservation>())
                {
                    item.StepCue = this.StepCueId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.Role;
            _ = this.CueObservations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
