
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
    [Table("Actions")]
    public class ActionBase : SoAEntityBase
    {
        [Key]
        public string ActionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Definition { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepAction> _stepActions;

        [InverseProperty("ActionRef")]
        public virtual ObservableCollection<StepAction> StepActions
        {
            get
            {
                if (_stepActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepActions - no database context is set. ActionId: " + this.ActionId + ".");
                        }
                        _stepActions = new ObservableCollection<StepAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepActions.Where(x => x.Action == this.ActionId).ToList<StepAction>();
                        _stepActions = new ObservableCollection<StepAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepActions.CollectionChanged += StepActions_CollectionChanged;
                }
                return _stepActions;
            }
            private set
            {
                if (_stepActions != null)
                {
                    _stepActions.CollectionChanged -= StepActions_CollectionChanged;
                }
                _stepActions = value;
                if (_stepActions != null)
                {
                    _stepActions.CollectionChanged += StepActions_CollectionChanged;
                }
            }
        }

        private void StepActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepAction>())
                {
                    item.Action = this.ActionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepActions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
