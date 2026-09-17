
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
    [Table("Facilities")]
    public class FacilityBase : SoAEntityBase
    {
        [Key]
        public string FacilityId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? FacilityKind { get; set; }
        // Formula DeviatingRunCount (rulebook: =COUNTIFS(ProcedureExecutions!{{DeviatingFacilityKey}}, {{FacilityId}}))
        [NotMapped]
        public int? DeviatingRunCount
        {
            get => F.AsInt(F.Memo(this, "DeviatingRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.DeviatingFacilityKey), F.Of(this.FacilityId))))))); set { }
        }

        // Formula CleanRunCount (rulebook: =COUNTIFS(ProcedureExecutions!{{CleanFacilityKey}}, {{FacilityId}}))
        [NotMapped]
        public int? CleanRunCount
        {
            get => F.AsInt(F.Memo(this, "CleanRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureExecution>(base.SoAContext, "ProcedureExecutions", __c => __c.ProcedureExecutions), __r => F.CritField(F.Of(__r.CleanFacilityKey), F.Of(this.FacilityId))))))); set { }
        }

        // Formula IsDeviationOnlyFacility (rulebook: =AND({{DeviatingRunCount}} > 0, {{CleanRunCount}} = 0))
        [NotMapped]
        public bool? IsDeviationOnlyFacility
        {
            get => F.AsBool(F.Memo(this, "IsDeviationOnlyFacility", () => F.And(F.Bool3(F.Cmp(F.Of(this.DeviatingRunCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CleanRunCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? ParentFacility { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private Facility _facility;

        [ForeignKey("ParentFacility")]
        public virtual Facility Facility
        {
            get
            {
                if (_facility == null && !string.IsNullOrEmpty(ParentFacility))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Facility - no database context is set. ParentFacility: " + ParentFacility + ".");
                        }
                        return null;
                    }
                    _facility = base.SoAContext.Facilities.Find(ParentFacility);
                    if (_facility != null)
                    {
                        base.SoAContext.Attach(_facility);
                    }
                }
                return _facility;
            }
            set
            {
                if (_facility != value)
                {
                    _facility = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_facility != null)
                    {
                        ParentFacility = _facility.FacilityId;
                    }
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _procedureExecutions;

        [InverseProperty("FacilityRef")]
        public virtual ObservableCollection<ProcedureExecution> ProcedureExecutions
        {
            get
            {
                if (_procedureExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. FacilityId: " + this.FacilityId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.Facility == this.FacilityId).ToList<ProcedureExecution>();
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureExecutions.CollectionChanged += ProcedureExecutions_CollectionChanged;
                }
                return _procedureExecutions;
            }
            private set
            {
                if (_procedureExecutions != null)
                {
                    _procedureExecutions.CollectionChanged -= ProcedureExecutions_CollectionChanged;
                }
                _procedureExecutions = value;
                if (_procedureExecutions != null)
                {
                    _procedureExecutions.CollectionChanged += ProcedureExecutions_CollectionChanged;
                }
            }
        }

        private void ProcedureExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureExecution>())
                {
                    item.Facility = this.FacilityId;
                }
            }
        }

        private ObservableCollection<Facility> _facilities;

        [InverseProperty("Facility")]
        public virtual ObservableCollection<Facility> Facilities
        {
            get
            {
                if (_facilities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Facilities - no database context is set. FacilityId: " + this.FacilityId + ".");
                        }
                        _facilities = new ObservableCollection<Facility>();
                    }
                    else
                    {
                        var items = base.SoAContext.Facilities.Where(x => x.ParentFacility == this.FacilityId).ToList<Facility>();
                        _facilities = new ObservableCollection<Facility>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _facilities.CollectionChanged += Facilities_CollectionChanged;
                }
                return _facilities;
            }
            private set
            {
                if (_facilities != null)
                {
                    _facilities.CollectionChanged -= Facilities_CollectionChanged;
                }
                _facilities = value;
                if (_facilities != null)
                {
                    _facilities.CollectionChanged += Facilities_CollectionChanged;
                }
            }
        }

        private void Facilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Facility>())
                {
                    item.ParentFacility = this.FacilityId;
                }
            }
        }

        private ObservableCollection<Machine> _machines;

        [InverseProperty("FacilityRef")]
        public virtual ObservableCollection<Machine> Machines
        {
            get
            {
                if (_machines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Machines - no database context is set. FacilityId: " + this.FacilityId + ".");
                        }
                        _machines = new ObservableCollection<Machine>();
                    }
                    else
                    {
                        var items = base.SoAContext.Machines.Where(x => x.Facility == this.FacilityId).ToList<Machine>();
                        _machines = new ObservableCollection<Machine>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _machines.CollectionChanged += Machines_CollectionChanged;
                }
                return _machines;
            }
            private set
            {
                if (_machines != null)
                {
                    _machines.CollectionChanged -= Machines_CollectionChanged;
                }
                _machines = value;
                if (_machines != null)
                {
                    _machines.CollectionChanged += Machines_CollectionChanged;
                }
            }
        }

        private void Machines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Machine>())
                {
                    item.Facility = this.FacilityId;
                }
            }
        }

        private ObservableCollection<TacticalResourceAllocation> _tacticalResourceAllocations;

        [InverseProperty("FacilityRef")]
        public virtual ObservableCollection<TacticalResourceAllocation> TacticalResourceAllocations
        {
            get
            {
                if (_tacticalResourceAllocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TacticalResourceAllocations - no database context is set. FacilityId: " + this.FacilityId + ".");
                        }
                        _tacticalResourceAllocations = new ObservableCollection<TacticalResourceAllocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.TacticalResourceAllocations.Where(x => x.Facility == this.FacilityId).ToList<TacticalResourceAllocation>();
                        _tacticalResourceAllocations = new ObservableCollection<TacticalResourceAllocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _tacticalResourceAllocations.CollectionChanged += TacticalResourceAllocations_CollectionChanged;
                }
                return _tacticalResourceAllocations;
            }
            private set
            {
                if (_tacticalResourceAllocations != null)
                {
                    _tacticalResourceAllocations.CollectionChanged -= TacticalResourceAllocations_CollectionChanged;
                }
                _tacticalResourceAllocations = value;
                if (_tacticalResourceAllocations != null)
                {
                    _tacticalResourceAllocations.CollectionChanged += TacticalResourceAllocations_CollectionChanged;
                }
            }
        }

        private void TacticalResourceAllocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TacticalResourceAllocation>())
                {
                    item.Facility = this.FacilityId;
                }
            }
        }

        private ObservableCollection<KnowledgeOutcomeMeasurement> _knowledgeOutcomeMeasurements;

        [InverseProperty("FacilityRef")]
        public virtual ObservableCollection<KnowledgeOutcomeMeasurement> KnowledgeOutcomeMeasurements
        {
            get
            {
                if (_knowledgeOutcomeMeasurements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeOutcomeMeasurements - no database context is set. FacilityId: " + this.FacilityId + ".");
                        }
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeOutcomeMeasurements.Where(x => x.Facility == this.FacilityId).ToList<KnowledgeOutcomeMeasurement>();
                        _knowledgeOutcomeMeasurements = new ObservableCollection<KnowledgeOutcomeMeasurement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                return _knowledgeOutcomeMeasurements;
            }
            private set
            {
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged -= KnowledgeOutcomeMeasurements_CollectionChanged;
                }
                _knowledgeOutcomeMeasurements = value;
                if (_knowledgeOutcomeMeasurements != null)
                {
                    _knowledgeOutcomeMeasurements.CollectionChanged += KnowledgeOutcomeMeasurements_CollectionChanged;
                }
            }
        }

        private void KnowledgeOutcomeMeasurements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeOutcomeMeasurement>())
                {
                    item.Facility = this.FacilityId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("Facility")]
        public virtual ObservableCollection<KnowHowCarrier> KnowHowCarriers
        {
            get
            {
                if (_knowHowCarriers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. FacilityId: " + this.FacilityId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.HolderFacility == this.FacilityId).ToList<KnowHowCarrier>();
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowHowCarriers.CollectionChanged += KnowHowCarriers_CollectionChanged;
                }
                return _knowHowCarriers;
            }
            private set
            {
                if (_knowHowCarriers != null)
                {
                    _knowHowCarriers.CollectionChanged -= KnowHowCarriers_CollectionChanged;
                }
                _knowHowCarriers = value;
                if (_knowHowCarriers != null)
                {
                    _knowHowCarriers.CollectionChanged += KnowHowCarriers_CollectionChanged;
                }
            }
        }

        private void KnowHowCarriers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowHowCarrier>())
                {
                    item.HolderFacility = this.FacilityId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.Facility;
            _ = this.ProcedureExecutions;
            _ = this.Facilities;
            _ = this.Machines;
            _ = this.TacticalResourceAllocations;
            _ = this.KnowledgeOutcomeMeasurements;
            _ = this.KnowHowCarriers;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
