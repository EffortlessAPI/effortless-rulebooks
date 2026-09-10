
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
    [Table("ApprovalGates")]
    public class ApprovalGateBase : SoAEntityBase
    {
        [Key]
        public string ApprovalGateId { get; set; }

        // Formula ParentPath (rulebook: =INDEX(WorkflowSteps!{{RelativePath}}, MATCH({{WorkflowStep}}, WorkflowSteps!{{WorkflowStepId}}, 0)))
        [NotMapped]
        public string? ParentPath
        {
            get => F.AsString(F.Memo(this, "ParentPath", () => F.Lookup<WorkflowStep>(this, "WorkflowSteps", "WorkflowStepId", __c => __c.WorkflowSteps, __r => F.Of(__r.WorkflowStepId), F.Of(this.WorkflowStep), __r => F.Of(__r.RelativePath), () => F.Of(new WorkflowStep().RelativePath)))); set { }
        }

        // Formula RelativePath (rulebook: ={{ParentPath}} & "/approval-gates/" & {{ApprovalGateId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.TextOr(F.Of(this.ParentPath)), F.S("/approval-gates/"), F.TextOr(F.Of(this.ApprovalGateId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        // Formula Name (rulebook: =SUBSTITUTE(LOWER({{DisplayName}}), " ", "-"))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Substitute(F.Lower(F.Of(this.DisplayName)), F.S(" "), F.S("-")))); set { }
        }

        public string? DisplayName { get; set; }
        public int? EscalationThresholdHours { get; set; }
        // Formula GateRole (rulebook: =INDEX(WorkflowSteps!{{AssignedRole}}, MATCH({{WorkflowStep}}, WorkflowSteps!{{WorkflowStepId}}, 0)))
        [NotMapped]
        public string? GateRole
        {
            get => F.AsString(F.Memo(this, "GateRole", () => F.Lookup<WorkflowStep>(this, "WorkflowSteps", "WorkflowStepId", __c => __c.WorkflowSteps, __r => F.Of(__r.WorkflowStepId), F.Of(this.WorkflowStep), __r => F.Of(__r.AssignedRole), () => F.Of(new WorkflowStep().AssignedRole)))); set { }
        }

        // Formula GateApproverHuman (rulebook: =INDEX(Roles!{{FilledByHumanAgent}}, MATCH({{GateRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? GateApproverHuman
        {
            get => F.AsString(F.Memo(this, "GateApproverHuman", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.GateRole), __r => F.Of(__r.FilledByHumanAgent), () => F.Of(new Role().FilledByHumanAgent)))); set { }
        }

        // Formula HasHumanApprover (rulebook: =NOT(ISBLANK({{GateApproverHuman}})))
        [NotMapped]
        public bool? HasHumanApprover
        {
            get => F.AsBool(F.Memo(this, "HasHumanApprover", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.GateApproverHuman)))))); set { }
        }


        public string? WorkflowStep { get; set; }

        private WorkflowStep _workflowStepRef;

        [ForeignKey("WorkflowStep")]
        public virtual WorkflowStep WorkflowStepRef
        {
            get
            {
                if (_workflowStepRef == null && !string.IsNullOrEmpty(WorkflowStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStepRef - no database context is set. WorkflowStep: " + WorkflowStep + ".");
                        }
                        return null;
                    }
                    _workflowStepRef = base.SoAContext.WorkflowSteps.Find(WorkflowStep);
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
                        WorkflowStep = _workflowStepRef.WorkflowStepId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowStep> _workflowSteps;

        [InverseProperty("ApprovalGateRef")]
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
                            throw new InvalidOperationException("Cannot access WorkflowSteps - no database context is set. ApprovalGateId: " + this.ApprovalGateId + ".");
                        }
                        _workflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.ApprovalGate == this.ApprovalGateId).ToList<WorkflowStep>();
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
                    item.ApprovalGate = this.ApprovalGateId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.WorkflowStepRef;
            _ = this.WorkflowSteps;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
