
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Functions")]
    public class FunctionBase : SoAEntityBase
    {
        [Key]
        public string FunctionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? Definition { get; set; }
        public string? ImplementationKey { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepFunction> _stepFunctions;

        [InverseProperty("Function")]
        public virtual ObservableCollection<StepFunction> StepFunctions
        {
            get
            {
                if (_stepFunctions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepFunctions - no database context is set. FunctionId: " + this.FunctionId + ".");
                        }
                        _stepFunctions = new ObservableCollection<StepFunction>();
                    }
                    else
                    {
                        var items = Context.StepFunctions.Where(x => x.Function == this.FunctionId).ToList<StepFunction>();
                        _stepFunctions = new ObservableCollection<StepFunction>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepFunctions.CollectionChanged += StepFunctions_CollectionChanged;
                }
                return _stepFunctions;
            }
            private set
            {
                if (_stepFunctions != null)
                {
                    _stepFunctions.CollectionChanged -= StepFunctions_CollectionChanged;
                }
                _stepFunctions = value;
                if (_stepFunctions != null)
                {
                    _stepFunctions.CollectionChanged += StepFunctions_CollectionChanged;
                }
            }
        }

        private void StepFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepFunction>())
                {
                    item.Function = this.FunctionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepFunctions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
