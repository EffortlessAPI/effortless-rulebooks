
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
    [Table("CollectionOccasions")]
    public class CollectionOccasionBase : SoAEntityBase
    {
        [Key]
        public string CollectionOccasionId { get; set; }

        // Formula Name (rulebook: ={{OccasionKind}} & ": " & {{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.OccasionKind)), F.S(": "), F.Text(F.Of(this.Label))))); set { }
        }

        public string? Label { get; set; }
        public string? OccasionKind { get; set; }
        public int? CadenceDays { get; set; }
        public DateTimeOffset? LastHeldAt { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceHeld (rulebook: =IF({{LastHeldAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{LastHeldAt}}, "days")))
        [NotMapped]
        public int? DaysSinceHeld
        {
            get => F.AsInt(F.Memo(this, "DaysSinceHeld", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastHeldAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastHeldAt), F.S("days")))))); set { }
        }

        // Formula IsLapsed (rulebook: ={{DaysSinceHeld}} > {{CadenceDays}})
        [NotMapped]
        public bool? IsLapsed
        {
            get => F.AsBool(F.Memo(this, "IsLapsed", () => F.Cmp(F.Of(this.DaysSinceHeld), ">", F.Nullif(F.Of(this.CadenceDays))))); set { }
        }

        // Formula CapturedMaterialCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{CollectedAtOccasion}}, {{CollectionOccasionId}}))
        [NotMapped]
        public int? CapturedMaterialCount
        {
            get => F.AsInt(F.Memo(this, "CapturedMaterialCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.CollectedAtOccasion), F.Of(this.CollectionOccasionId))))))); set { }
        }

        // Formula IsHeldWithoutCapture (rulebook: =AND({{LastHeldAt}} <> "", {{CapturedMaterialCount}} = 0))
        [NotMapped]
        public bool? IsHeldWithoutCapture
        {
            get => F.AsBool(F.Memo(this, "IsHeldWithoutCapture", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.LastHeldAt))), F.Bool3(F.Eq(F.Of(this.CapturedMaterialCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Procedure { get; set; }
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

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("CollectionOccasion")]
        public virtual ObservableCollection<CollectedSourceMaterial> CollectedSourceMaterials
        {
            get
            {
                if (_collectedSourceMaterials == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. CollectionOccasionId: " + this.CollectionOccasionId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.CollectedAtOccasion == this.CollectionOccasionId).ToList<CollectedSourceMaterial>();
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
                return _collectedSourceMaterials;
            }
            private set
            {
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged -= CollectedSourceMaterials_CollectionChanged;
                }
                _collectedSourceMaterials = value;
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
            }
        }

        private void CollectedSourceMaterials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectedSourceMaterial>())
                {
                    item.CollectedAtOccasion = this.CollectionOccasionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.EvaluationContextRef;
            _ = this.CollectedSourceMaterials;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
