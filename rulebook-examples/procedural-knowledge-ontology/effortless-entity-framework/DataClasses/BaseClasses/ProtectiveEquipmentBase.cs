
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
    [Table("ProtectiveEquipment")]
    public class ProtectiveEquipmentBase : SoAEntityBase
    {
        [Key]
        public string ProtectiveEquipmentId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepProtectiveEquipment> _stepProtectiveEquipment;

        [InverseProperty("ProtectiveEquipmentRef")]
        public virtual ObservableCollection<StepProtectiveEquipment> StepProtectiveEquipment
        {
            get
            {
                if (_stepProtectiveEquipment == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepProtectiveEquipment - no database context is set. ProtectiveEquipmentId: " + this.ProtectiveEquipmentId + ".");
                        }
                        _stepProtectiveEquipment = new ObservableCollection<StepProtectiveEquipment>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepProtectiveEquipment.Where(x => x.ProtectiveEquipment == this.ProtectiveEquipmentId).ToList<StepProtectiveEquipment>();
                        _stepProtectiveEquipment = new ObservableCollection<StepProtectiveEquipment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepProtectiveEquipment.CollectionChanged += StepProtectiveEquipment_CollectionChanged;
                }
                return _stepProtectiveEquipment;
            }
            private set
            {
                if (_stepProtectiveEquipment != null)
                {
                    _stepProtectiveEquipment.CollectionChanged -= StepProtectiveEquipment_CollectionChanged;
                }
                _stepProtectiveEquipment = value;
                if (_stepProtectiveEquipment != null)
                {
                    _stepProtectiveEquipment.CollectionChanged += StepProtectiveEquipment_CollectionChanged;
                }
            }
        }

        private void StepProtectiveEquipment_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepProtectiveEquipment>())
                {
                    item.ProtectiveEquipment = this.ProtectiveEquipmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepProtectiveEquipment;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
