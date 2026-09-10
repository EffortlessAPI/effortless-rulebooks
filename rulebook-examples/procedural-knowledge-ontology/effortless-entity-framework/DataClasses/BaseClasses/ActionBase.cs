
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Actions")]
    public class ActionBase : SoAEntityBase
    {
        [Key]
        public string ActionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? Definition { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepAction> _stepActions;

        [InverseProperty("Action")]
        public virtual ObservableCollection<StepAction> StepActions
        {
            get
            {
                if (_stepActions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepActions - no database context is set. ActionId: " + this.ActionId + ".");
                        }
                        _stepActions = new ObservableCollection<StepAction>();
                    }
                    else
                    {
                        var items = Context.StepActions.Where(x => x.Action == this.ActionId).ToList<StepAction>();
                        _stepActions = new ObservableCollection<StepAction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
