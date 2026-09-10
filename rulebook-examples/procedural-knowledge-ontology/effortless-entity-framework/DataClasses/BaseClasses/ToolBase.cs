
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Tools")]
    public class ToolBase : SoAEntityBase
    {
        [Key]
        public string ToolId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? Purpose { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepTool> _stepTools;

        [InverseProperty("Tool")]
        public virtual ObservableCollection<StepTool> StepTools
        {
            get
            {
                if (_stepTools == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTools - no database context is set. ToolId: " + this.ToolId + ".");
                        }
                        _stepTools = new ObservableCollection<StepTool>();
                    }
                    else
                    {
                        var items = Context.StepTools.Where(x => x.Tool == this.ToolId).ToList<StepTool>();
                        _stepTools = new ObservableCollection<StepTool>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _stepTools.CollectionChanged += StepTools_CollectionChanged;
                }
                return _stepTools;
            }
            private set
            {
                if (_stepTools != null)
                {
                    _stepTools.CollectionChanged -= StepTools_CollectionChanged;
                }
                _stepTools = value;
                if (_stepTools != null)
                {
                    _stepTools.CollectionChanged += StepTools_CollectionChanged;
                }
            }
        }

        private void StepTools_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTool>())
                {
                    item.Tool = this.ToolId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepTools;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
