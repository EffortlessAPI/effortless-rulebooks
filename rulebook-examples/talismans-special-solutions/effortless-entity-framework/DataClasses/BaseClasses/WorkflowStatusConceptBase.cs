
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
    [Table("WorkflowStatusConcepts")]
    public class WorkflowStatusConceptBase : SoAEntityBase
    {
        [Key]
        public string ConceptId { get; set; }

        // Formula RelativePath (rulebook: ="concepts/workflow-status/" & {{ConceptId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("concepts/workflow-status/"), F.TextOr(F.Of(this.ConceptId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string PrefLabel { get; set; }
        public string? AltLabel { get; set; }
        public string? Definition { get; set; }
        public string? ScopeNote { get; set; }

        public string? Workflows { get; set; }

        private Workflow _workflow;

        [ForeignKey("Workflows")]
        public virtual Workflow Workflow
        {
            get
            {
                if (_workflow == null && !string.IsNullOrEmpty(Workflows))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Workflow - no database context is set. Workflows: " + Workflows + ".");
                        }
                        return null;
                    }
                    _workflow = base.SoAContext.Workflows.Find(Workflows);
                    if (_workflow != null)
                    {
                        base.SoAContext.Attach(_workflow);
                    }
                }
                return _workflow;
            }
            set
            {
                if (_workflow != value)
                {
                    _workflow = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflow != null)
                    {
                        Workflows = _workflow.WorkflowId;
                    }
                }
            }
        }

        private ObservableCollection<Workflow> _workflowStatusWorkflows;

        [InverseProperty("WorkflowStatusConcept")]
        public virtual ObservableCollection<Workflow> WorkflowStatusWorkflows
        {
            get
            {
                if (_workflowStatusWorkflows == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStatusWorkflows - no database context is set. ConceptId: " + this.ConceptId + ".");
                        }
                        _workflowStatusWorkflows = new ObservableCollection<Workflow>();
                    }
                    else
                    {
                        var items = base.SoAContext.Workflows.Where(x => x.WorkflowStatus == this.ConceptId).ToList<Workflow>();
                        _workflowStatusWorkflows = new ObservableCollection<Workflow>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowStatusWorkflows.CollectionChanged += WorkflowStatusWorkflows_CollectionChanged;
                }
                return _workflowStatusWorkflows;
            }
            private set
            {
                if (_workflowStatusWorkflows != null)
                {
                    _workflowStatusWorkflows.CollectionChanged -= WorkflowStatusWorkflows_CollectionChanged;
                }
                _workflowStatusWorkflows = value;
                if (_workflowStatusWorkflows != null)
                {
                    _workflowStatusWorkflows.CollectionChanged += WorkflowStatusWorkflows_CollectionChanged;
                }
            }
        }

        private void WorkflowStatusWorkflows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Workflow>())
                {
                    item.WorkflowStatus = this.ConceptId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Workflow;
            _ = this.WorkflowStatusWorkflows;
        }

        public override string ToString()
        {
            return this.ConceptId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
