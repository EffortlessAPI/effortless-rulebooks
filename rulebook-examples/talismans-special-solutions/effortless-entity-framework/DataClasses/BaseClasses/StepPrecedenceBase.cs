
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
    [Table("StepPrecedence")]
    public class StepPrecedenceBase : SoAEntityBase
    {
        [Key]
        public string StepPrecedenceId { get; set; }

        // Formula ParentPath (rulebook: =INDEX(WorkflowSteps!{{RelativePath}}, MATCH({{FromStep}}, WorkflowSteps!{{WorkflowStepId}}, 0)))
        [NotMapped]
        public string? ParentPath
        {
            get => F.AsString(F.Memo(this, "ParentPath", () => F.Lookup<WorkflowStep>(this, "WorkflowSteps", "WorkflowStepId", __c => __c.WorkflowSteps, __r => F.Of(__r.WorkflowStepId), F.Of(this.FromStep), __r => F.Of(__r.RelativePath), () => F.Of(new WorkflowStep().RelativePath)))); set { }
        }

        // Formula RelativePath (rulebook: ={{ParentPath}} & "/precedence/" & {{StepPrecedenceId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.TextOr(F.Of(this.ParentPath)), F.S("/precedence/"), F.TextOr(F.Of(this.StepPrecedenceId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: ={{FromStep}} & " -> " & {{ToStep}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.FromStep)), F.S(" -> "), F.TextOr(F.Of(this.ToStep))))); set { }
        }

        public string? PrecedesStepClosure { get; set; }

        public string FromStep { get; set; }
        public string ToStep { get; set; }

        private WorkflowStep _workflowStep;

        [ForeignKey("FromStep")]
        public virtual WorkflowStep WorkflowStep
        {
            get
            {
                if (_workflowStep == null && !string.IsNullOrEmpty(FromStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStep - no database context is set. FromStep: " + FromStep + ".");
                        }
                        return null;
                    }
                    _workflowStep = base.SoAContext.WorkflowSteps.Find(FromStep);
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
                        FromStep = _workflowStep.WorkflowStepId;
                    }
                }
            }
        }

        private WorkflowStep _workflowStepRef;

        [ForeignKey("ToStep")]
        public virtual WorkflowStep WorkflowStepRef
        {
            get
            {
                if (_workflowStepRef == null && !string.IsNullOrEmpty(ToStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStepRef - no database context is set. ToStep: " + ToStep + ".");
                        }
                        return null;
                    }
                    _workflowStepRef = base.SoAContext.WorkflowSteps.Find(ToStep);
                    if (_workflowStepRef != null)
                    {
                        base.SoAContext.Attach(_workflowStepRef);
                    }
                }
                return _workflowStepRef;
            }
            set
            {
                if (_workflowStepRef != value)
                {
                    _workflowStepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowStepRef != null)
                    {
                        ToStep = _workflowStepRef.WorkflowStepId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowStep> _precedesWorkflowSteps;

        [InverseProperty("StepPrecedence")]
        public virtual ObservableCollection<WorkflowStep> PrecedesWorkflowSteps
        {
            get
            {
                if (_precedesWorkflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrecedesWorkflowSteps - no database context is set. StepPrecedenceId: " + this.StepPrecedenceId + ".");
                        }
                        _precedesWorkflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.Precedes == this.StepPrecedenceId).ToList<WorkflowStep>();
                        _precedesWorkflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _precedesWorkflowSteps.CollectionChanged += PrecedesWorkflowSteps_CollectionChanged;
                }
                return _precedesWorkflowSteps;
            }
            private set
            {
                if (_precedesWorkflowSteps != null)
                {
                    _precedesWorkflowSteps.CollectionChanged -= PrecedesWorkflowSteps_CollectionChanged;
                }
                _precedesWorkflowSteps = value;
                if (_precedesWorkflowSteps != null)
                {
                    _precedesWorkflowSteps.CollectionChanged += PrecedesWorkflowSteps_CollectionChanged;
                }
            }
        }

        private void PrecedesWorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.Precedes = this.StepPrecedenceId;
                }
            }
        }

        private ObservableCollection<WorkflowStep> _precededByWorkflowSteps;

        [InverseProperty("StepPrecedenceRef")]
        public virtual ObservableCollection<WorkflowStep> PrecededByWorkflowSteps
        {
            get
            {
                if (_precededByWorkflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrecededByWorkflowSteps - no database context is set. StepPrecedenceId: " + this.StepPrecedenceId + ".");
                        }
                        _precededByWorkflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.PrecededBy == this.StepPrecedenceId).ToList<WorkflowStep>();
                        _precededByWorkflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _precededByWorkflowSteps.CollectionChanged += PrecededByWorkflowSteps_CollectionChanged;
                }
                return _precededByWorkflowSteps;
            }
            private set
            {
                if (_precededByWorkflowSteps != null)
                {
                    _precededByWorkflowSteps.CollectionChanged -= PrecededByWorkflowSteps_CollectionChanged;
                }
                _precededByWorkflowSteps = value;
                if (_precededByWorkflowSteps != null)
                {
                    _precededByWorkflowSteps.CollectionChanged += PrecededByWorkflowSteps_CollectionChanged;
                }
            }
        }

        private void PrecededByWorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.PrecededBy = this.StepPrecedenceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.WorkflowStep;
            _ = this.WorkflowStepRef;
            _ = this.PrecedesWorkflowSteps;
            _ = this.PrecededByWorkflowSteps;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
