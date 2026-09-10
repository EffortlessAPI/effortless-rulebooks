
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
    [Table("Roles")]
    public class RoleBase : SoAEntityBase
    {
        [Key]
        public string RoleId { get; set; }

        // Formula RelativePath (rulebook: ="roles/" & {{RoleId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("roles/"), F.TextOr(F.Of(this.RoleId))))); set { }
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
        public string? Label { get; set; }
        public string? Comment { get; set; }
        public string? DelegationClosure { get; set; }
        // Formula FilledByArmCount (rulebook: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAIAgent}})), 1, 0) + IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), 1, 0))
        [NotMapped]
        public int? FilledByArmCount
        {
            get => F.AsInt(F.Memo(this, "FilledByArmCount", () => F.Integer(F.Add(F.Add((F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByHumanAgent)))))) ? F.I(1) : F.I(0)), (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByAIAgent)))))) ? F.I(1) : F.I(0))), (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByAutomatedPipeline)))))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula HasExactlyOneFiller (rulebook: ={{FilledByArmCount}} = 1)
        [NotMapped]
        public bool? HasExactlyOneFiller
        {
            get => F.AsBool(F.Memo(this, "HasExactlyOneFiller", () => F.Eq(F.Of(this.FilledByArmCount), F.I(1)))); set { }
        }

        // Formula FillerType (rulebook: =IF(NOT(ISBLANK({{FilledByHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{FilledByAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{FilledByAutomatedPipeline}})), "AutomatedPipeline", ""))))
        [NotMapped]
        public string? FillerType
        {
            get => F.AsString(F.Memo(this, "FillerType", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByHumanAgent)))))) ? F.S("HumanAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByAIAgent)))))) ? F.S("AIAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.FilledByAutomatedPipeline)))))) ? F.S("AutomatedPipeline") : F.S("")))))); set { }
        }

        // Formula FillsApprovalGate (rulebook: =COUNTIFS(WorkflowSteps!{{AssignedRole}}, Roles!{{RoleId}}, WorkflowSteps!{{IsApprovalGate}}, TRUE))
        [NotMapped]
        public int? FillsApprovalGate
        {
            get => F.AsInt(F.Memo(this, "FillsApprovalGate", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowStep>(base.SoAContext, "WorkflowSteps", __c => __c.WorkflowSteps), __r => F.CritField(F.Of(__r.AssignedRole), F.Of(this.RoleId)) && F.CritLiteral(F.Of(__r.IsApprovalGate), F.B(true))))))); set { }
        }

        // Formula EscalationViolation (rulebook: =AND({{FillsApprovalGate}} > 0, ISBLANK({{DelegatesTo}})))
        [NotMapped]
        public bool? EscalationViolation
        {
            get => F.AsBool(F.Memo(this, "EscalationViolation", () => F.And(F.Bool3(F.Cmp(F.Of(this.FillsApprovalGate), ">", F.I(0))), F.Bool3(F.IsBlank(F.Of(this.DelegatesTo)))))); set { }
        }


        public string? HasCapability { get; set; }
        public string? FilledByHumanAgent { get; set; }
        public string? FilledByAIAgent { get; set; }
        public string? FilledByAutomatedPipeline { get; set; }
        public string? OwnedBy { get; set; }
        public string? DelegatesTo { get; set; }
        public string? WorkflowSteps { get; set; }
        public string? FromDelegatesTo { get; set; }
        public string? RoleAssignments { get; set; }

        private AgentCapabilityConcept _agentCapabilityConcept;

        [ForeignKey("HasCapability")]
        public virtual AgentCapabilityConcept AgentCapabilityConcept
        {
            get
            {
                if (_agentCapabilityConcept == null && !string.IsNullOrEmpty(HasCapability))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentCapabilityConcept - no database context is set. HasCapability: " + HasCapability + ".");
                        }
                        return null;
                    }
                    _agentCapabilityConcept = base.SoAContext.AgentCapabilityConcepts.Find(HasCapability);
                    if (_agentCapabilityConcept != null)
                    {
                        base.SoAContext.Attach(_agentCapabilityConcept);
                    }
                }
                return _agentCapabilityConcept;
            }
            set
            {
                if (_agentCapabilityConcept != value)
                {
                    _agentCapabilityConcept = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentCapabilityConcept != null)
                    {
                        HasCapability = _agentCapabilityConcept.ConceptId;
                    }
                }
            }
        }

        private HumanAgent _humanAgent;

        [ForeignKey("FilledByHumanAgent")]
        public virtual HumanAgent HumanAgent
        {
            get
            {
                if (_humanAgent == null && !string.IsNullOrEmpty(FilledByHumanAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HumanAgent - no database context is set. FilledByHumanAgent: " + FilledByHumanAgent + ".");
                        }
                        return null;
                    }
                    _humanAgent = base.SoAContext.HumanAgents.Find(FilledByHumanAgent);
                    if (_humanAgent != null)
                    {
                        base.SoAContext.Attach(_humanAgent);
                    }
                }
                return _humanAgent;
            }
            set
            {
                if (_humanAgent != value)
                {
                    _humanAgent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_humanAgent != null)
                    {
                        FilledByHumanAgent = _humanAgent.HumanAgentId;
                    }
                }
            }
        }

        private AIAgent _aIAgent;

        [ForeignKey("FilledByAIAgent")]
        public virtual AIAgent AIAgent
        {
            get
            {
                if (_aIAgent == null && !string.IsNullOrEmpty(FilledByAIAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AIAgent - no database context is set. FilledByAIAgent: " + FilledByAIAgent + ".");
                        }
                        return null;
                    }
                    _aIAgent = base.SoAContext.AIAgents.Find(FilledByAIAgent);
                    if (_aIAgent != null)
                    {
                        base.SoAContext.Attach(_aIAgent);
                    }
                }
                return _aIAgent;
            }
            set
            {
                if (_aIAgent != value)
                {
                    _aIAgent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aIAgent != null)
                    {
                        FilledByAIAgent = _aIAgent.AIAgentId;
                    }
                }
            }
        }

        private AutomatedPipeline _automatedPipeline;

        [ForeignKey("FilledByAutomatedPipeline")]
        public virtual AutomatedPipeline AutomatedPipeline
        {
            get
            {
                if (_automatedPipeline == null && !string.IsNullOrEmpty(FilledByAutomatedPipeline))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AutomatedPipeline - no database context is set. FilledByAutomatedPipeline: " + FilledByAutomatedPipeline + ".");
                        }
                        return null;
                    }
                    _automatedPipeline = base.SoAContext.AutomatedPipelines.Find(FilledByAutomatedPipeline);
                    if (_automatedPipeline != null)
                    {
                        base.SoAContext.Attach(_automatedPipeline);
                    }
                }
                return _automatedPipeline;
            }
            set
            {
                if (_automatedPipeline != value)
                {
                    _automatedPipeline = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_automatedPipeline != null)
                    {
                        FilledByAutomatedPipeline = _automatedPipeline.AutomatedPipelineId;
                    }
                }
            }
        }

        private Department _department;

        [ForeignKey("OwnedBy")]
        public virtual Department Department
        {
            get
            {
                if (_department == null && !string.IsNullOrEmpty(OwnedBy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Department - no database context is set. OwnedBy: " + OwnedBy + ".");
                        }
                        return null;
                    }
                    _department = base.SoAContext.Departments.Find(OwnedBy);
                    if (_department != null)
                    {
                        base.SoAContext.Attach(_department);
                    }
                }
                return _department;
            }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_department != null)
                    {
                        OwnedBy = _department.DepartmentId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("DelegatesTo")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(DelegatesTo))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. DelegatesTo: " + DelegatesTo + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(DelegatesTo);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        DelegatesTo = _role.RoleId;
                    }
                }
            }
        }

        private WorkflowStep _workflowStep;

        [ForeignKey("WorkflowSteps")]
        public virtual WorkflowStep WorkflowStep
        {
            get
            {
                if (_workflowStep == null && !string.IsNullOrEmpty(WorkflowSteps))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStep - no database context is set. WorkflowSteps: " + WorkflowSteps + ".");
                        }
                        return null;
                    }
                    _workflowStep = base.SoAContext.WorkflowSteps.Find(WorkflowSteps);
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
                        WorkflowSteps = _workflowStep.WorkflowStepId;
                    }
                }
            }
        }

        private Role _roleRef;

        [ForeignKey("FromDelegatesTo")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(FromDelegatesTo))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. FromDelegatesTo: " + FromDelegatesTo + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(FromDelegatesTo);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        FromDelegatesTo = _roleRef.RoleId;
                    }
                }
            }
        }

        private RoleAssignment _roleAssignment;

        [ForeignKey("RoleAssignments")]
        public virtual RoleAssignment RoleAssignment
        {
            get
            {
                if (_roleAssignment == null && !string.IsNullOrEmpty(RoleAssignments))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignment - no database context is set. RoleAssignments: " + RoleAssignments + ".");
                        }
                        return null;
                    }
                    _roleAssignment = base.SoAContext.RoleAssignments.Find(RoleAssignments);
                    if (_roleAssignment != null)
                    {
                        base.SoAContext.Attach(_roleAssignment);
                    }
                }
                return _roleAssignment;
            }
            set
            {
                if (_roleAssignment != value)
                {
                    _roleAssignment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleAssignment != null)
                    {
                        RoleAssignments = _roleAssignment.RoleAssignmentId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowStep> _assignedRoleWorkflowSteps;

        [InverseProperty("Role")]
        public virtual ObservableCollection<WorkflowStep> AssignedRoleWorkflowSteps
        {
            get
            {
                if (_assignedRoleWorkflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssignedRoleWorkflowSteps - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _assignedRoleWorkflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.AssignedRole == this.RoleId).ToList<WorkflowStep>();
                        _assignedRoleWorkflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assignedRoleWorkflowSteps.CollectionChanged += AssignedRoleWorkflowSteps_CollectionChanged;
                }
                return _assignedRoleWorkflowSteps;
            }
            private set
            {
                if (_assignedRoleWorkflowSteps != null)
                {
                    _assignedRoleWorkflowSteps.CollectionChanged -= AssignedRoleWorkflowSteps_CollectionChanged;
                }
                _assignedRoleWorkflowSteps = value;
                if (_assignedRoleWorkflowSteps != null)
                {
                    _assignedRoleWorkflowSteps.CollectionChanged += AssignedRoleWorkflowSteps_CollectionChanged;
                }
            }
        }

        private void AssignedRoleWorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.AssignedRole = this.RoleId;
                }
            }
        }

        private ObservableCollection<Role> _delegatesToRoles;

        [InverseProperty("Role")]
        public virtual ObservableCollection<Role> DelegatesToRoles
        {
            get
            {
                if (_delegatesToRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DelegatesToRoles - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _delegatesToRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.DelegatesTo == this.RoleId).ToList<Role>();
                        _delegatesToRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _delegatesToRoles.CollectionChanged += DelegatesToRoles_CollectionChanged;
                }
                return _delegatesToRoles;
            }
            private set
            {
                if (_delegatesToRoles != null)
                {
                    _delegatesToRoles.CollectionChanged -= DelegatesToRoles_CollectionChanged;
                }
                _delegatesToRoles = value;
                if (_delegatesToRoles != null)
                {
                    _delegatesToRoles.CollectionChanged += DelegatesToRoles_CollectionChanged;
                }
            }
        }

        private void DelegatesToRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.DelegatesTo = this.RoleId;
                }
            }
        }

        private ObservableCollection<Role> _fromDelegatesToRoles;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<Role> FromDelegatesToRoles
        {
            get
            {
                if (_fromDelegatesToRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromDelegatesToRoles - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _fromDelegatesToRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.FromDelegatesTo == this.RoleId).ToList<Role>();
                        _fromDelegatesToRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromDelegatesToRoles.CollectionChanged += FromDelegatesToRoles_CollectionChanged;
                }
                return _fromDelegatesToRoles;
            }
            private set
            {
                if (_fromDelegatesToRoles != null)
                {
                    _fromDelegatesToRoles.CollectionChanged -= FromDelegatesToRoles_CollectionChanged;
                }
                _fromDelegatesToRoles = value;
                if (_fromDelegatesToRoles != null)
                {
                    _fromDelegatesToRoles.CollectionChanged += FromDelegatesToRoles_CollectionChanged;
                }
            }
        }

        private void FromDelegatesToRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.FromDelegatesTo = this.RoleId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleRoleAssignments;

        [InverseProperty("RoleRef")]
        public virtual ObservableCollection<RoleAssignment> RoleRoleAssignments
        {
            get
            {
                if (_roleRoleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRoleAssignments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _roleRoleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.Role == this.RoleId).ToList<RoleAssignment>();
                        _roleRoleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleRoleAssignments.CollectionChanged += RoleRoleAssignments_CollectionChanged;
                }
                return _roleRoleAssignments;
            }
            private set
            {
                if (_roleRoleAssignments != null)
                {
                    _roleRoleAssignments.CollectionChanged -= RoleRoleAssignments_CollectionChanged;
                }
                _roleRoleAssignments = value;
                if (_roleRoleAssignments != null)
                {
                    _roleRoleAssignments.CollectionChanged += RoleRoleAssignments_CollectionChanged;
                }
            }
        }

        private void RoleRoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.Role = this.RoleId;
                }
            }
        }

        private ObservableCollection<Department> _departments;

        [InverseProperty("Role")]
        public virtual ObservableCollection<Department> Departments
        {
            get
            {
                if (_departments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Departments - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _departments = new ObservableCollection<Department>();
                    }
                    else
                    {
                        var items = base.SoAContext.Departments.Where(x => x.Roles == this.RoleId).ToList<Department>();
                        _departments = new ObservableCollection<Department>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _departments.CollectionChanged += Departments_CollectionChanged;
                }
                return _departments;
            }
            private set
            {
                if (_departments != null)
                {
                    _departments.CollectionChanged -= Departments_CollectionChanged;
                }
                _departments = value;
                if (_departments != null)
                {
                    _departments.CollectionChanged += Departments_CollectionChanged;
                }
            }
        }

        private void Departments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Department>())
                {
                    item.Roles = this.RoleId;
                }
            }
        }

        private ObservableCollection<HumanAgent> _humanAgents;

        [InverseProperty("Role")]
        public virtual ObservableCollection<HumanAgent> HumanAgents
        {
            get
            {
                if (_humanAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HumanAgents - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _humanAgents = new ObservableCollection<HumanAgent>();
                    }
                    else
                    {
                        var items = base.SoAContext.HumanAgents.Where(x => x.Roles == this.RoleId).ToList<HumanAgent>();
                        _humanAgents = new ObservableCollection<HumanAgent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _humanAgents.CollectionChanged += HumanAgents_CollectionChanged;
                }
                return _humanAgents;
            }
            private set
            {
                if (_humanAgents != null)
                {
                    _humanAgents.CollectionChanged -= HumanAgents_CollectionChanged;
                }
                _humanAgents = value;
                if (_humanAgents != null)
                {
                    _humanAgents.CollectionChanged += HumanAgents_CollectionChanged;
                }
            }
        }

        private void HumanAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<HumanAgent>())
                {
                    item.Roles = this.RoleId;
                }
            }
        }

        private ObservableCollection<AIAgent> _aIAgents;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AIAgent> AIAgents
        {
            get
            {
                if (_aIAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AIAgents - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _aIAgents = new ObservableCollection<AIAgent>();
                    }
                    else
                    {
                        var items = base.SoAContext.AIAgents.Where(x => x.Roles == this.RoleId).ToList<AIAgent>();
                        _aIAgents = new ObservableCollection<AIAgent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aIAgents.CollectionChanged += AIAgents_CollectionChanged;
                }
                return _aIAgents;
            }
            private set
            {
                if (_aIAgents != null)
                {
                    _aIAgents.CollectionChanged -= AIAgents_CollectionChanged;
                }
                _aIAgents = value;
                if (_aIAgents != null)
                {
                    _aIAgents.CollectionChanged += AIAgents_CollectionChanged;
                }
            }
        }

        private void AIAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AIAgent>())
                {
                    item.Roles = this.RoleId;
                }
            }
        }

        private ObservableCollection<AutomatedPipeline> _automatedPipelines;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AutomatedPipeline> AutomatedPipelines
        {
            get
            {
                if (_automatedPipelines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AutomatedPipelines - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _automatedPipelines = new ObservableCollection<AutomatedPipeline>();
                    }
                    else
                    {
                        var items = base.SoAContext.AutomatedPipelines.Where(x => x.Roles == this.RoleId).ToList<AutomatedPipeline>();
                        _automatedPipelines = new ObservableCollection<AutomatedPipeline>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _automatedPipelines.CollectionChanged += AutomatedPipelines_CollectionChanged;
                }
                return _automatedPipelines;
            }
            private set
            {
                if (_automatedPipelines != null)
                {
                    _automatedPipelines.CollectionChanged -= AutomatedPipelines_CollectionChanged;
                }
                _automatedPipelines = value;
                if (_automatedPipelines != null)
                {
                    _automatedPipelines.CollectionChanged += AutomatedPipelines_CollectionChanged;
                }
            }
        }

        private void AutomatedPipelines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AutomatedPipeline>())
                {
                    item.Roles = this.RoleId;
                }
            }
        }

        private ObservableCollection<AgentCapabilityConcept> _agentCapabilityConcepts;

        [InverseProperty("Role")]
        public virtual ObservableCollection<AgentCapabilityConcept> AgentCapabilityConcepts
        {
            get
            {
                if (_agentCapabilityConcepts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentCapabilityConcepts - no database context is set. RoleId: " + this.RoleId + ".");
                        }
                        _agentCapabilityConcepts = new ObservableCollection<AgentCapabilityConcept>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentCapabilityConcepts.Where(x => x.Roles == this.RoleId).ToList<AgentCapabilityConcept>();
                        _agentCapabilityConcepts = new ObservableCollection<AgentCapabilityConcept>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _agentCapabilityConcepts.CollectionChanged += AgentCapabilityConcepts_CollectionChanged;
                }
                return _agentCapabilityConcepts;
            }
            private set
            {
                if (_agentCapabilityConcepts != null)
                {
                    _agentCapabilityConcepts.CollectionChanged -= AgentCapabilityConcepts_CollectionChanged;
                }
                _agentCapabilityConcepts = value;
                if (_agentCapabilityConcepts != null)
                {
                    _agentCapabilityConcepts.CollectionChanged += AgentCapabilityConcepts_CollectionChanged;
                }
            }
        }

        private void AgentCapabilityConcepts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentCapabilityConcept>())
                {
                    item.Roles = this.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AgentCapabilityConcept;
            _ = this.HumanAgent;
            _ = this.AIAgent;
            _ = this.AutomatedPipeline;
            _ = this.Department;
            _ = this.Role;
            _ = this.WorkflowStep;
            _ = this.RoleRef;
            _ = this.RoleAssignment;
            _ = this.AssignedRoleWorkflowSteps;
            _ = this.DelegatesToRoles;
            _ = this.FromDelegatesToRoles;
            _ = this.RoleRoleAssignments;
            _ = this.Departments;
            _ = this.HumanAgents;
            _ = this.AIAgents;
            _ = this.AutomatedPipelines;
            _ = this.AgentCapabilityConcepts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
