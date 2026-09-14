
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
    [Table("OperationalBindings")]
    public class OperationalBindingBase : SoAEntityBase
    {
        [Key]
        public string OperationalBindingId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{RecordOrSchemaKey}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.RecordOrSchemaKey))))); set { }
        }

        public string? AccessMode { get; set; }
        public string? RecordOrSchemaKey { get; set; }
        public DateTimeOffset? LastObservedAt { get; set; }
        public int? FreshnessSlaMinutes { get; set; }
        public bool? IsAuthoritative { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula AgeMinutes (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{LastObservedAt}}, "minutes"))
        [NotMapped]
        public int? AgeMinutes
        {
            get => F.AsInt(F.Memo(this, "AgeMinutes", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastObservedAt), F.S("minutes"))))); set { }
        }

        // Formula IsFresh (rulebook: ={{AgeMinutes}} <= {{FreshnessSlaMinutes}})
        [NotMapped]
        public bool? IsFresh
        {
            get => F.AsBool(F.Memo(this, "IsFresh", () => F.Cmp(F.Of(this.AgeMinutes), "<=", F.Nullif(F.Of(this.FreshnessSlaMinutes))))); set { }
        }

        // Formula StaleBindingStepKey (rulebook: =IF(NOT({{IsFresh}}), {{Step}}, ""))
        [NotMapped]
        public string? StaleBindingStepKey
        {
            get => F.AsString(F.Memo(this, "StaleBindingStepKey", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.IsFresh))))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula AuthoritativeStaleStepKey (rulebook: =IF(AND(NOT({{IsFresh}}), {{IsAuthoritative}}), {{Step}}, ""))
        [NotMapped]
        public string? AuthoritativeStaleStepKey
        {
            get => F.AsString(F.Memo(this, "AuthoritativeStaleStepKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsFresh)))), F.IsTrueV(F.Of(this.IsAuthoritative))))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula IsStaleAndAuthoritative (rulebook: =AND({{IsAuthoritative}}, NOT({{IsFresh}})))
        [NotMapped]
        public bool? IsStaleAndAuthoritative
        {
            get => F.AsBool(F.Memo(this, "IsStaleAndAuthoritative", () => F.And(F.IsTrueV(F.Of(this.IsAuthoritative)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsFresh))))))); set { }
        }

        // Formula StepWhenStale (rulebook: =IF({{IsStaleAndAuthoritative}}, {{Step}}, ""))
        [NotMapped]
        public string? StepWhenStale
        {
            get => F.AsString(F.Memo(this, "StepWhenStale", () => (F.Truthy(F.Bool3(F.Of(this.IsStaleAndAuthoritative))) ? F.Of(this.Step) : F.S("")))); set { }
        }

        // Formula ResourceIsApproved (rulebook: =INDEX(Resources!{{IsApprovedSource}}, MATCH({{Resource}}, Resources!{{ResourceId}}, 0)))
        [NotMapped]
        public bool? ResourceIsApproved
        {
            get => F.AsBool(F.Memo(this, "ResourceIsApproved", () => F.Lookup<Resource>(this, "Resources", "ResourceId", __c => __c.Resources, __r => F.Of(__r.ResourceId), F.Of(this.Resource), __r => F.Of(__r.IsApprovedSource), () => F.Of(new Resource().IsApprovedSource)))); set { }
        }

        // Formula IsUsableForDrafting (rulebook: =AND({{ResourceIsApproved}}, {{IsFresh}}))
        [NotMapped]
        public bool? IsUsableForDrafting
        {
            get => F.AsBool(F.Memo(this, "IsUsableForDrafting", () => F.And(F.Bool3(F.Of(this.ResourceIsApproved)), F.Bool3(F.Of(this.IsFresh))))); set { }
        }

        // Formula StepWhenUnusable (rulebook: =IF({{IsUsableForDrafting}}, "", {{Step}}))
        [NotMapped]
        public string? StepWhenUnusable
        {
            get => F.AsString(F.Memo(this, "StepWhenUnusable", () => (F.Truthy(F.Bool3(F.Of(this.IsUsableForDrafting))) ? F.S("") : F.Of(this.Step)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? Resource { get; set; }
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

        private Resource _resourceRef;

        [ForeignKey("Resource")]
        public virtual Resource ResourceRef
        {
            get
            {
                if (_resourceRef == null && !string.IsNullOrEmpty(Resource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ResourceRef - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resourceRef = base.SoAContext.Resources.Find(Resource);
                    if (_resourceRef != null)
                    {
                        base.SoAContext.Attach(_resourceRef);
                    }
                }
                return _resourceRef;
            }
            set
            {
                if (_resourceRef != value)
                {
                    _resourceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resourceRef != null)
                    {
                        Resource = _resourceRef.ResourceId;
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

        private ObservableCollection<Recipient> _recipients;

        [InverseProperty("OperationalBinding")]
        public virtual ObservableCollection<Recipient> Recipients
        {
            get
            {
                if (_recipients == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Recipients - no database context is set. OperationalBindingId: " + this.OperationalBindingId + ".");
                        }
                        _recipients = new ObservableCollection<Recipient>();
                    }
                    else
                    {
                        var items = base.SoAContext.Recipients.Where(x => x.ConsentBinding == this.OperationalBindingId).ToList<Recipient>();
                        _recipients = new ObservableCollection<Recipient>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _recipients.CollectionChanged += Recipients_CollectionChanged;
                }
                return _recipients;
            }
            private set
            {
                if (_recipients != null)
                {
                    _recipients.CollectionChanged -= Recipients_CollectionChanged;
                }
                _recipients = value;
                if (_recipients != null)
                {
                    _recipients.CollectionChanged += Recipients_CollectionChanged;
                }
            }
        }

        private void Recipients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Recipient>())
                {
                    item.ConsentBinding = this.OperationalBindingId;
                }
            }
        }

        private ObservableCollection<BindingObservation> _bindingObservations;

        [InverseProperty("OperationalBindingRef")]
        public virtual ObservableCollection<BindingObservation> BindingObservations
        {
            get
            {
                if (_bindingObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BindingObservations - no database context is set. OperationalBindingId: " + this.OperationalBindingId + ".");
                        }
                        _bindingObservations = new ObservableCollection<BindingObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.BindingObservations.Where(x => x.OperationalBinding == this.OperationalBindingId).ToList<BindingObservation>();
                        _bindingObservations = new ObservableCollection<BindingObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _bindingObservations.CollectionChanged += BindingObservations_CollectionChanged;
                }
                return _bindingObservations;
            }
            private set
            {
                if (_bindingObservations != null)
                {
                    _bindingObservations.CollectionChanged -= BindingObservations_CollectionChanged;
                }
                _bindingObservations = value;
                if (_bindingObservations != null)
                {
                    _bindingObservations.CollectionChanged += BindingObservations_CollectionChanged;
                }
            }
        }

        private void BindingObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<BindingObservation>())
                {
                    item.OperationalBinding = this.OperationalBindingId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.ResourceRef;
            _ = this.EvaluationContextRef;
            _ = this.Recipients;
            _ = this.BindingObservations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
