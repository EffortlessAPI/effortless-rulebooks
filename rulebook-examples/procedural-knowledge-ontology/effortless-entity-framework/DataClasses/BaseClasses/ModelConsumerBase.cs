
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
    [Table("ModelConsumers")]
    public class ModelConsumerBase : SoAEntityBase
    {
        [Key]
        public string ModelConsumerId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? ConsumerKind { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? DependsOnModel { get; set; }
        public string? ConformanceSubstrate { get; set; }
        public string? OwnerRole { get; set; }

        private GovernedModel _governedModel;

        [ForeignKey("DependsOnModel")]
        public virtual GovernedModel GovernedModel
        {
            get
            {
                if (_governedModel == null && !string.IsNullOrEmpty(DependsOnModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModel - no database context is set. DependsOnModel: " + DependsOnModel + ".");
                        }
                        return null;
                    }
                    _governedModel = base.SoAContext.GovernedModels.Find(DependsOnModel);
                    if (_governedModel != null)
                    {
                        base.SoAContext.Attach(_governedModel);
                    }
                }
                return _governedModel;
            }
            set
            {
                if (_governedModel != value)
                {
                    _governedModel = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModel != null)
                    {
                        DependsOnModel = _governedModel.GovernedModelId;
                    }
                }
            }
        }

        private ConformanceSubstrate _conformanceSubstrateRef;

        [ForeignKey("ConformanceSubstrate")]
        public virtual ConformanceSubstrate ConformanceSubstrateRef
        {
            get
            {
                if (_conformanceSubstrateRef == null && !string.IsNullOrEmpty(ConformanceSubstrate))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConformanceSubstrateRef - no database context is set. ConformanceSubstrate: " + ConformanceSubstrate + ".");
                        }
                        return null;
                    }
                    _conformanceSubstrateRef = base.SoAContext.ConformanceSubstrates.Find(ConformanceSubstrate);
                    if (_conformanceSubstrateRef != null)
                    {
                        base.SoAContext.Attach(_conformanceSubstrateRef);
                    }
                }
                return _conformanceSubstrateRef;
            }
            set
            {
                if (_conformanceSubstrateRef != value)
                {
                    _conformanceSubstrateRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_conformanceSubstrateRef != null)
                    {
                        ConformanceSubstrate = _conformanceSubstrateRef.ConformanceSubstrateId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("OwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(OwnerRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(OwnerRole);
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
                        OwnerRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<ConsumerRevalidation> _consumerRevalidations;

        [InverseProperty("ModelConsumerRef")]
        public virtual ObservableCollection<ConsumerRevalidation> ConsumerRevalidations
        {
            get
            {
                if (_consumerRevalidations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConsumerRevalidations - no database context is set. ModelConsumerId: " + this.ModelConsumerId + ".");
                        }
                        _consumerRevalidations = new ObservableCollection<ConsumerRevalidation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConsumerRevalidations.Where(x => x.ModelConsumer == this.ModelConsumerId).ToList<ConsumerRevalidation>();
                        _consumerRevalidations = new ObservableCollection<ConsumerRevalidation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _consumerRevalidations.CollectionChanged += ConsumerRevalidations_CollectionChanged;
                }
                return _consumerRevalidations;
            }
            private set
            {
                if (_consumerRevalidations != null)
                {
                    _consumerRevalidations.CollectionChanged -= ConsumerRevalidations_CollectionChanged;
                }
                _consumerRevalidations = value;
                if (_consumerRevalidations != null)
                {
                    _consumerRevalidations.CollectionChanged += ConsumerRevalidations_CollectionChanged;
                }
            }
        }

        private void ConsumerRevalidations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConsumerRevalidation>())
                {
                    item.ModelConsumer = this.ModelConsumerId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModel;
            _ = this.ConformanceSubstrateRef;
            _ = this.Role;
            _ = this.ConsumerRevalidations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
