
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("OperationalBindings")]
    public class OperationalBindingBase : SoAEntityBase
    {
        [Key]
        public string OperationalBindingId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{RecordOrSchemaKey}})
        public string? Name
        {
            get => this.Step + " / " + this.RecordOrSchemaKey; set { }
        }

        public string? AccessMode { get; set; }
        public string? RecordOrSchemaKey { get; set; }
        public DateTime? LastObservedAt { get; set; }
        public int? FreshnessSlaMinutes { get; set; }
        public bool? IsAuthoritative { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula AgeMinutes (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{LastObservedAt}}, "minutes"))
        public int? AgeMinutes
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.LastObservedAt, "minutes"); set { }
        }

        // Formula IsFresh (rulebook: ={{AgeMinutes}} <= {{FreshnessSlaMinutes}})
        public bool? IsFresh
        {
            get => this.AgeMinutes <= this.FreshnessSlaMinutes; set { }
        }

        // Formula StaleBindingStepKey (rulebook: =IF(NOT({{IsFresh}}), {{Step}}, ""))
        public string? StaleBindingStepKey
        {
            get => IF(NOT(this.IsFresh), this.Step, ""); set { }
        }

        // Formula AuthoritativeStaleStepKey (rulebook: =IF(AND(NOT({{IsFresh}}), {{IsAuthoritative}}), {{Step}}, ""))
        public string? AuthoritativeStaleStepKey
        {
            get => IF(AND(NOT(this.IsFresh), this.IsAuthoritative), this.Step, ""); set { }
        }

        // Formula IsStaleAndAuthoritative (rulebook: =AND({{IsAuthoritative}}, NOT({{IsFresh}})))
        public bool? IsStaleAndAuthoritative
        {
            get => AND(this.IsAuthoritative, NOT(this.IsFresh)); set { }
        }

        // Formula StepWhenStale (rulebook: =IF({{IsStaleAndAuthoritative}}, {{Step}}, ""))
        public string? StepWhenStale
        {
            get => IF(this.IsStaleAndAuthoritative, this.Step, ""); set { }
        }

        // Formula ResourceIsApproved (rulebook: =INDEX(Resources!{{IsApprovedSource}}, MATCH({{Resource}}, Resources!{{ResourceId}}, 0)))
        public bool? ResourceIsApproved
        {
            get => INDEX(Resources!this.IsApprovedSource, MATCH(this.Resource, Resources!this.ResourceId, 0)); set { }
        }

        // Formula IsUsableForDrafting (rulebook: =AND({{ResourceIsApproved}}, {{IsFresh}}))
        public bool? IsUsableForDrafting
        {
            get => AND(this.ResourceIsApproved, this.IsFresh); set { }
        }

        // Formula StepWhenUnusable (rulebook: =IF({{IsUsableForDrafting}}, "", {{Step}}))
        public string? StepWhenUnusable
        {
            get => IF(this.IsUsableForDrafting, "", this.Step); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? Resource { get; set; }
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

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
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
                    Step = _step == null ? default : _step.StepId;
                }
            }
        }

        private Resource _resource;

        [ForeignKey("Resource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(Resource))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resource = Context.Resources.Find(Resource);
                    if (_resource != null)
                    {
                        Context.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    Resource = _resource == null ? default : _resource.ResourceId;
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

        private ObservableCollection<Recipient> _recipients;

        [InverseProperty("OperationalBinding")]
        public virtual ObservableCollection<Recipient> Recipients
        {
            get
            {
                if (_recipients == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Recipients - no database context is set. OperationalBindingId: " + this.OperationalBindingId + ".");
                        }
                        _recipients = new ObservableCollection<Recipient>();
                    }
                    else
                    {
                        var items = Context.Recipients.Where(x => x.ConsentBinding == this.OperationalBindingId).ToList<Recipient>();
                        _recipients = new ObservableCollection<Recipient>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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

        [InverseProperty("OperationalBinding")]
        public virtual ObservableCollection<BindingObservation> BindingObservations
        {
            get
            {
                if (_bindingObservations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BindingObservations - no database context is set. OperationalBindingId: " + this.OperationalBindingId + ".");
                        }
                        _bindingObservations = new ObservableCollection<BindingObservation>();
                    }
                    else
                    {
                        var items = Context.BindingObservations.Where(x => x.OperationalBinding == this.OperationalBindingId).ToList<BindingObservation>();
                        _bindingObservations = new ObservableCollection<BindingObservation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.Resource;
            _ = this.EvaluationContext;
            _ = this.Recipients;
            _ = this.BindingObservations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
