
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
    [Table("MachineTypes")]
    public class MachineTypeBase : SoAEntityBase
    {
        [Key]
        public string MachineTypeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<Machine> _machines;

        [InverseProperty("MachineTypeRef")]
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
                            throw new InvalidOperationException("Cannot access Machines - no database context is set. MachineTypeId: " + this.MachineTypeId + ".");
                        }
                        _machines = new ObservableCollection<Machine>();
                    }
                    else
                    {
                        var items = base.SoAContext.Machines.Where(x => x.MachineType == this.MachineTypeId).ToList<Machine>();
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
                    item.MachineType = this.MachineTypeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Machines;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
