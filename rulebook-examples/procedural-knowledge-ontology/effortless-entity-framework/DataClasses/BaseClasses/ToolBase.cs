
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
    [Table("Tools")]
    public class ToolBase : SoAEntityBase
    {
        [Key]
        public string ToolId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Purpose { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<StepTool> _stepTools;

        [InverseProperty("ToolRef")]
        public virtual ObservableCollection<StepTool> StepTools
        {
            get
            {
                if (_stepTools == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTools - no database context is set. ToolId: " + this.ToolId + ".");
                        }
                        _stepTools = new ObservableCollection<StepTool>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepTools.Where(x => x.Tool == this.ToolId).ToList<StepTool>();
                        _stepTools = new ObservableCollection<StepTool>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        private ObservableCollection<AuthoringSubmission> _authoringSubmissions;

        [InverseProperty("Tool")]
        public virtual ObservableCollection<AuthoringSubmission> AuthoringSubmissions
        {
            get
            {
                if (_authoringSubmissions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthoringSubmissions - no database context is set. ToolId: " + this.ToolId + ".");
                        }
                        _authoringSubmissions = new ObservableCollection<AuthoringSubmission>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthoringSubmissions.Where(x => x.AuthoringTool == this.ToolId).ToList<AuthoringSubmission>();
                        _authoringSubmissions = new ObservableCollection<AuthoringSubmission>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authoringSubmissions.CollectionChanged += AuthoringSubmissions_CollectionChanged;
                }
                return _authoringSubmissions;
            }
            private set
            {
                if (_authoringSubmissions != null)
                {
                    _authoringSubmissions.CollectionChanged -= AuthoringSubmissions_CollectionChanged;
                }
                _authoringSubmissions = value;
                if (_authoringSubmissions != null)
                {
                    _authoringSubmissions.CollectionChanged += AuthoringSubmissions_CollectionChanged;
                }
            }
        }

        private void AuthoringSubmissions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthoringSubmission>())
                {
                    item.AuthoringTool = this.ToolId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepTools;
            _ = this.AuthoringSubmissions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
