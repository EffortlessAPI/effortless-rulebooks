
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
    [Table("Datasets")]
    public class DatasetBase : SoAEntityBase
    {
        [Key]
        public string DatasetId { get; set; }

        // Formula RelativePath (rulebook: ="datasets/" & {{DatasetId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("datasets/"), F.TextOr(F.Of(this.DatasetId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string Title { get; set; }
        public string? Identifier { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string? DistributionUrl { get; set; }
        // Formula IsConsumed (rulebook: =NOT(ISBLANK({{ConsumedBySteps}})))
        [NotMapped]
        public bool? IsConsumed
        {
            get => F.AsBool(F.Memo(this, "IsConsumed", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.ConsumedBySteps)))))); set { }
        }


        public string? ConsumedBySteps { get; set; }

        private WorkflowStep _workflowStep;

        [ForeignKey("ConsumedBySteps")]
        public virtual WorkflowStep WorkflowStep
        {
            get
            {
                if (_workflowStep == null && !string.IsNullOrEmpty(ConsumedBySteps))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStep - no database context is set. ConsumedBySteps: " + ConsumedBySteps + ".");
                        }
                        return null;
                    }
                    _workflowStep = base.SoAContext.WorkflowSteps.Find(ConsumedBySteps);
                    if (_workflowStep != null)
                    {
                        base.SoAContext.Attach(_workflowStep);
                    }
                }
                return _workflowStep;
            }
            set
            {
                if (_workflowStep != value)
                {
                    _workflowStep = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowStep != null)
                    {
                        ConsumedBySteps = _workflowStep.WorkflowStepId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowStep> _workflowSteps;

        [InverseProperty("Dataset")]
        public virtual ObservableCollection<WorkflowStep> WorkflowSteps
        {
            get
            {
                if (_workflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowSteps - no database context is set. DatasetId: " + this.DatasetId + ".");
                        }
                        _workflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.ConsumesDataset == this.DatasetId).ToList<WorkflowStep>();
                        _workflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowSteps.CollectionChanged += WorkflowSteps_CollectionChanged;
                }
                return _workflowSteps;
            }
            private set
            {
                if (_workflowSteps != null)
                {
                    _workflowSteps.CollectionChanged -= WorkflowSteps_CollectionChanged;
                }
                _workflowSteps = value;
                if (_workflowSteps != null)
                {
                    _workflowSteps.CollectionChanged += WorkflowSteps_CollectionChanged;
                }
            }
        }

        private void WorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.ConsumesDataset = this.DatasetId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.WorkflowStep;
            _ = this.WorkflowSteps;
        }

        public override string ToString()
        {
            return this.DatasetId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
