
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
    [Table("EnergySources")]
    public class EnergySourceBase : SoAEntityBase
    {
        [Key]
        public string EnergySourceId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? PkoIndividualIri { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<Step> _steps;

        [InverseProperty("EnergySource")]
        public virtual ObservableCollection<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. EnergySourceId: " + this.EnergySourceId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.IsolatesEnergySource == this.EnergySourceId).ToList<Step>();
                        _steps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
                return _steps;
            }
            private set
            {
                if (_steps != null)
                {
                    _steps.CollectionChanged -= Steps_CollectionChanged;
                }
                _steps = value;
                if (_steps != null)
                {
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
            }
        }

        private void Steps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.IsolatesEnergySource = this.EnergySourceId;
                }
            }
        }

        private ObservableCollection<MachineEnergySource> _machineEnergySources;

        [InverseProperty("EnergySourceRef")]
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
                            throw new InvalidOperationException("Cannot access MachineEnergySources - no database context is set. EnergySourceId: " + this.EnergySourceId + ".");
                        }
                        _machineEnergySources = new ObservableCollection<MachineEnergySource>();
                    }
                    else
                    {
                        var items = base.SoAContext.MachineEnergySources.Where(x => x.EnergySource == this.EnergySourceId).ToList<MachineEnergySource>();
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
                    item.EnergySource = this.EnergySourceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Steps;
            _ = this.MachineEnergySources;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
