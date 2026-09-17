
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
    [Table("LockDevices")]
    public class LockDeviceBase : SoAEntityBase
    {
        [Key]
        public string LockDeviceId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepLockRequirement> _stepLockRequirements;

        [InverseProperty("LockDeviceRef")]
        public virtual ObservableCollection<StepLockRequirement> StepLockRequirements
        {
            get
            {
                if (_stepLockRequirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepLockRequirements - no database context is set. LockDeviceId: " + this.LockDeviceId + ".");
                        }
                        _stepLockRequirements = new ObservableCollection<StepLockRequirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepLockRequirements.Where(x => x.LockDevice == this.LockDeviceId).ToList<StepLockRequirement>();
                        _stepLockRequirements = new ObservableCollection<StepLockRequirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepLockRequirements.CollectionChanged += StepLockRequirements_CollectionChanged;
                }
                return _stepLockRequirements;
            }
            private set
            {
                if (_stepLockRequirements != null)
                {
                    _stepLockRequirements.CollectionChanged -= StepLockRequirements_CollectionChanged;
                }
                _stepLockRequirements = value;
                if (_stepLockRequirements != null)
                {
                    _stepLockRequirements.CollectionChanged += StepLockRequirements_CollectionChanged;
                }
            }
        }

        private void StepLockRequirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepLockRequirement>())
                {
                    item.LockDevice = this.LockDeviceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepLockRequirements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
