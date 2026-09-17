
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
    [Table("Machines")]
    public class MachineBase : SoAEntityBase
    {
        [Key]
        public string MachineId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? ConfigurationKind { get; set; }
        // Formula EnergySourceCount (rulebook: =COUNTIFS(MachineEnergySources!{{Machine}}, {{MachineId}}))
        [NotMapped]
        public int? EnergySourceCount
        {
            get => F.AsInt(F.Memo(this, "EnergySourceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MachineEnergySource>(base.SoAContext, "MachineEnergySources", __c => __c.MachineEnergySources), __r => F.CritField(F.Of(__r.Machine), F.Of(this.MachineId))))))); set { }
        }

        // Formula UnisolatedEnergySourceCount (rulebook: =COUNTIFS(MachineEnergySources!{{UnisolatedMachineKey}}, {{MachineId}}))
        [NotMapped]
        public int? UnisolatedEnergySourceCount
        {
            get => F.AsInt(F.Memo(this, "UnisolatedEnergySourceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MachineEnergySource>(base.SoAContext, "MachineEnergySources", __c => __c.MachineEnergySources), __r => F.CritField(F.Of(__r.UnisolatedMachineKey), F.Of(this.MachineId))))))); set { }
        }

        // Formula IsNonStandardConfiguration (rulebook: ={{ConfigurationKind}} = "NonStandard")
        [NotMapped]
        public bool? IsNonStandardConfiguration
        {
            get => F.AsBool(F.Memo(this, "IsNonStandardConfiguration", () => F.Eq(F.Nullif(F.Of(this.ConfigurationKind)), F.S("NonStandard")))); set { }
        }

        // Formula HasUnisolatedEnergySource (rulebook: ={{UnisolatedEnergySourceCount}} > 0)
        [NotMapped]
        public bool? HasUnisolatedEnergySource
        {
            get => F.AsBool(F.Memo(this, "HasUnisolatedEnergySource", () => F.Cmp(F.Of(this.UnisolatedEnergySourceCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? MachineType { get; set; }
        public string? Facility { get; set; }
        public string? ManufacturedBy { get; set; }
        public string? GoverningProcedureVersion { get; set; }

        private MachineType _machineTypeRef;

        [ForeignKey("MachineType")]
        public virtual MachineType MachineTypeRef
        {
            get
            {
                if (_machineTypeRef == null && !string.IsNullOrEmpty(MachineType))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MachineTypeRef - no database context is set. MachineType: " + MachineType + ".");
                        }
                        return null;
                    }
                    _machineTypeRef = base.SoAContext.MachineTypes.Find(MachineType);
                    if (_machineTypeRef != null)
                    {
                        base.SoAContext.Attach(_machineTypeRef);
                    }
                }
                return _machineTypeRef;
            }
            set
            {
                if (_machineTypeRef != value)
                {
                    _machineTypeRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_machineTypeRef != null)
                    {
                        MachineType = _machineTypeRef.MachineTypeId;
                    }
                }
            }
        }

        private Facility _facilityRef;

        [ForeignKey("Facility")]
        public virtual Facility FacilityRef
        {
            get
            {
                if (_facilityRef == null && !string.IsNullOrEmpty(Facility))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FacilityRef - no database context is set. Facility: " + Facility + ".");
                        }
                        return null;
                    }
                    _facilityRef = base.SoAContext.Facilities.Find(Facility);
                    if (_facilityRef != null)
                    {
                        base.SoAContext.Attach(_facilityRef);
                    }
                }
                return _facilityRef;
            }
            set
            {
                if (_facilityRef != value)
                {
                    _facilityRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_facilityRef != null)
                    {
                        Facility = _facilityRef.FacilityId;
                    }
                }
            }
        }

        private Organization _organization;

        [ForeignKey("ManufacturedBy")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(ManufacturedBy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. ManufacturedBy: " + ManufacturedBy + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(ManufacturedBy);
                    if (_organization != null)
                    {
                        base.SoAContext.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organization != null)
                    {
                        ManufacturedBy = _organization.OrganizationId;
                    }
                }
            }
        }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("GoverningProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(GoverningProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. GoverningProcedureVersion: " + GoverningProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = base.SoAContext.ProcedureVersions.Find(GoverningProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        base.SoAContext.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersion != null)
                    {
                        GoverningProcedureVersion = _procedureVersion.ProcedureVersionId;
                    }
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _procedureExecutions;

        [InverseProperty("Machine")]
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
                            throw new InvalidOperationException("Cannot access ProcedureExecutions - no database context is set. MachineId: " + this.MachineId + ".");
                        }
                        _procedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.ExecutedOnMachine == this.MachineId).ToList<ProcedureExecution>();
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
                    item.ExecutedOnMachine = this.MachineId;
                }
            }
        }

        private ObservableCollection<MachineEnergySource> _machineEnergySources;

        [InverseProperty("MachineRef")]
        public virtual ObservableCollection<MachineEnergySource> MachineEnergySources
        {
            get
            {
                if (_machineEnergySources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MachineEnergySources - no database context is set. MachineId: " + this.MachineId + ".");
                        }
                        _machineEnergySources = new ObservableCollection<MachineEnergySource>();
                    }
                    else
                    {
                        var items = base.SoAContext.MachineEnergySources.Where(x => x.Machine == this.MachineId).ToList<MachineEnergySource>();
                        _machineEnergySources = new ObservableCollection<MachineEnergySource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _machineEnergySources.CollectionChanged += MachineEnergySources_CollectionChanged;
                }
                return _machineEnergySources;
            }
            private set
            {
                if (_machineEnergySources != null)
                {
                    _machineEnergySources.CollectionChanged -= MachineEnergySources_CollectionChanged;
                }
                _machineEnergySources = value;
                if (_machineEnergySources != null)
                {
                    _machineEnergySources.CollectionChanged += MachineEnergySources_CollectionChanged;
                }
            }
        }

        private void MachineEnergySources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MachineEnergySource>())
                {
                    item.Machine = this.MachineId;
                }
            }
        }

        private ObservableCollection<ProcedureTarget> _procedureTargets;

        [InverseProperty("MachineRef")]
        public virtual ObservableCollection<ProcedureTarget> ProcedureTargets
        {
            get
            {
                if (_procedureTargets == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureTargets - no database context is set. MachineId: " + this.MachineId + ".");
                        }
                        _procedureTargets = new ObservableCollection<ProcedureTarget>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureTargets.Where(x => x.Machine == this.MachineId).ToList<ProcedureTarget>();
                        _procedureTargets = new ObservableCollection<ProcedureTarget>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureTargets.CollectionChanged += ProcedureTargets_CollectionChanged;
                }
                return _procedureTargets;
            }
            private set
            {
                if (_procedureTargets != null)
                {
                    _procedureTargets.CollectionChanged -= ProcedureTargets_CollectionChanged;
                }
                _procedureTargets = value;
                if (_procedureTargets != null)
                {
                    _procedureTargets.CollectionChanged += ProcedureTargets_CollectionChanged;
                }
            }
        }

        private void ProcedureTargets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureTarget>())
                {
                    item.Machine = this.MachineId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.MachineTypeRef;
            _ = this.FacilityRef;
            _ = this.Organization;
            _ = this.ProcedureVersion;
            _ = this.ProcedureExecutions;
            _ = this.MachineEnergySources;
            _ = this.ProcedureTargets;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
