
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
    [Table("WorkflowSteps")]
    public class WorkflowStepBase : SoAEntityBase
    {
        [Key]
        public string WorkflowStepId { get; set; }

        // Formula ParentPath (rulebook: =INDEX(Workflows!{{RelativePath}}, MATCH({{Workflow}}, Workflows!{{WorkflowId}}, 0)))
        [NotMapped]
        public string? ParentPath
        {
            get => F.AsString(F.Memo(this, "ParentPath", () => F.Lookup<Workflow>(this, "Workflows", "WorkflowId", __c => __c.Workflows, __r => F.Of(__r.WorkflowId), F.Of(this.Workflow), __r => F.Of(__r.RelativePath), () => F.Of(new Workflow().RelativePath)))); set { }
        }

        // Formula RelativePath (rulebook: ={{ParentPath}} & "/steps/" & {{WorkflowStepId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.TextOr(F.Of(this.ParentPath)), F.S("/steps/"), F.TextOr(F.Of(this.WorkflowStepId))))); set { }
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
        // Formula PrecedingStepCount (rulebook: =COUNTIFS(vw_step_precedence_closure!{{ToId}}, WorkflowSteps!{{WorkflowStepId}}))
        [NotMapped]
        public int? PrecedingStepCount
        {
            get => throw new System.NotSupportedException("WorkflowSteps.PrecedingStepCount: could not translate formula =COUNTIFS(vw_step_precedence_closure!{{ToId}}, WorkflowSteps!{{WorkflowStepId}}): no table named vw_step_precedence_closure"); set { }
        }

        // Formula InferredSequencePosition (rulebook: ={{PrecedingStepCount}} + 1)
        [NotMapped]
        public int? InferredSequencePosition
        {
            get => F.AsInt(F.Memo(this, "InferredSequencePosition", () => F.Integer(F.Add(F.Of(this.PrecedingStepCount), F.I(1))))); set { }
        }

        public int? SequencePositionOverride { get; set; }
        // Formula SequencePosition (rulebook: =IF({{SequencePositionOverride}} <> "", {{SequencePositionOverride}}, {{InferredSequencePosition}}))
        [NotMapped]
        public int? SequencePosition
        {
            get => F.AsInt(F.Memo(this, "SequencePosition", () => F.Integer((F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.SequencePositionOverride)))) ? F.Of(this.SequencePositionOverride) : F.Of(this.InferredSequencePosition))))); set { }
        }

        public bool? RequiresHumanApproval { get; set; }
        public int? StepDurationMinutes { get; set; }
        // Formula ExecutingHumanAgent (rulebook: =INDEX(Roles!{{FilledByHumanAgent}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? ExecutingHumanAgent
        {
            get => F.AsString(F.Memo(this, "ExecutingHumanAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.FilledByHumanAgent), () => F.Of(new Role().FilledByHumanAgent)))); set { }
        }

        // Formula ExecutingAIAgent (rulebook: =INDEX(Roles!{{FilledByAIAgent}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? ExecutingAIAgent
        {
            get => F.AsString(F.Memo(this, "ExecutingAIAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.FilledByAIAgent), () => F.Of(new Role().FilledByAIAgent)))); set { }
        }

        // Formula ExecutingAutomatedPipeline (rulebook: =INDEX(Roles!{{FilledByAutomatedPipeline}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? ExecutingAutomatedPipeline
        {
            get => F.AsString(F.Memo(this, "ExecutingAutomatedPipeline", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.FilledByAutomatedPipeline), () => F.Of(new Role().FilledByAutomatedPipeline)))); set { }
        }

        // Formula ExecutingAgentType (rulebook: =IF(NOT(ISBLANK({{ExecutingHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{ExecutingAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{ExecutingAutomatedPipeline}})), "AutomatedPipeline", ""))))
        [NotMapped]
        public string? ExecutingAgentType
        {
            get => F.AsString(F.Memo(this, "ExecutingAgentType", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.ExecutingHumanAgent)))))) ? F.S("HumanAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.ExecutingAIAgent)))))) ? F.S("AIAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.ExecutingAutomatedPipeline)))))) ? F.S("AutomatedPipeline") : F.S("")))))); set { }
        }

        // Formula IsExecutedByAI (rulebook: =NOT(ISBLANK({{ExecutingAIAgent}})))
        [NotMapped]
        public bool? IsExecutedByAI
        {
            get => F.AsBool(F.Memo(this, "IsExecutedByAI", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.ExecutingAIAgent)))))); set { }
        }

        // Formula IsExecutedByHuman (rulebook: =NOT(ISBLANK({{ExecutingHumanAgent}})))
        [NotMapped]
        public bool? IsExecutedByHuman
        {
            get => F.AsBool(F.Memo(this, "IsExecutedByHuman", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.ExecutingHumanAgent)))))); set { }
        }

        // Formula IsApprovalGate (rulebook: =NOT(ISBLANK({{ApprovalGate}})))
        [NotMapped]
        public bool? IsApprovalGate
        {
            get => F.AsBool(F.Memo(this, "IsApprovalGate", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.ApprovalGate)))))); set { }
        }

        // Formula ApprovalConsistencyViolation (rulebook: =AND({{RequiresHumanApproval}}, ISBLANK({{ExecutingHumanAgent}})))
        [NotMapped]
        public bool? ApprovalConsistencyViolation
        {
            get => F.AsBool(F.Memo(this, "ApprovalConsistencyViolation", () => F.And(F.IsTrueV(F.Of(this.RequiresHumanApproval)), F.Bool3(F.IsBlank(F.Of(this.ExecutingHumanAgent)))))); set { }
        }

        // Formula ApprovalIsHumanFilled (rulebook: =IF({{RequiresHumanApproval}}, NOT(ISBLANK({{ExecutingHumanAgent}})), TRUE))
        [NotMapped]
        public bool? ApprovalIsHumanFilled
        {
            get => F.AsBool(F.Memo(this, "ApprovalIsHumanFilled", () => (F.Truthy(F.IsTrueV(F.Of(this.RequiresHumanApproval))) ? F.Not(F.Bool3(F.IsBlank(F.Of(this.ExecutingHumanAgent)))) : F.B(true)))); set { }
        }

        // Formula OwningDepartment (rulebook: =INDEX(Roles!{{OwnedBy}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? OwningDepartment
        {
            get => F.AsString(F.Memo(this, "OwningDepartment", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.OwnedBy), () => F.Of(new Role().OwnedBy)))); set { }
        }

        // Formula IsLegalOwned (rulebook: ={{OwningDepartment}} = "ntwf-legal-dept")
        [NotMapped]
        public bool? IsLegalOwned
        {
            get => F.AsBool(F.Memo(this, "IsLegalOwned", () => F.Eq(F.Of(this.OwningDepartment), F.S("ntwf-legal-dept")))); set { }
        }

        // Formula IsEngineeringOwned (rulebook: ={{OwningDepartment}} = "ntwf-engineering")
        [NotMapped]
        public bool? IsEngineeringOwned
        {
            get => F.AsBool(F.Memo(this, "IsEngineeringOwned", () => F.Eq(F.Of(this.OwningDepartment), F.S("ntwf-engineering")))); set { }
        }


        public string? Workflow { get; set; }
        public string? AssignedRole { get; set; }
        public string? ConsumesDataset { get; set; }
        public string? ProducesArtifacts { get; set; }
        public string? RequiresArtifacts { get; set; }
        public string? ApprovalGate { get; set; }
        public string? Precedes { get; set; }
        public string? PrecededBy { get; set; }

        private Workflow _workflowRef;

        [ForeignKey("Workflow")]
        public virtual Workflow WorkflowRef
        {
            get
            {
                if (_workflowRef == null && !string.IsNullOrEmpty(Workflow))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowRef - no database context is set. Workflow: " + Workflow + ".");
                        }
                        return null;
                    }
                    _workflowRef = base.SoAContext.Workflows.Find(Workflow);
                    if (_workflowRef != null)
                    {
                        base.SoAContext.Attach(_workflowRef);
                    }
                }
                return _workflowRef;
            }
            set
            {
                if (_workflowRef != value)
                {
                    _workflowRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowRef != null)
                    {
                        Workflow = _workflowRef.WorkflowId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("AssignedRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AssignedRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AssignedRole: " + AssignedRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AssignedRole);
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
                        AssignedRole = _role.RoleId;
                    }
                }
            }
        }

        private Dataset _dataset;

        [ForeignKey("ConsumesDataset")]
        public virtual Dataset Dataset
        {
            get
            {
                if (_dataset == null && !string.IsNullOrEmpty(ConsumesDataset))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Dataset - no database context is set. ConsumesDataset: " + ConsumesDataset + ".");
                        }
                        return null;
                    }
                    _dataset = base.SoAContext.Datasets.Find(ConsumesDataset);
                    if (_dataset != null)
                    {
                        base.SoAContext.Attach(_dataset);
                    }
                }
                return _dataset;
            }
            set
            {
                if (_dataset != value)
                {
                    _dataset = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_dataset != null)
                    {
                        ConsumesDataset = _dataset.DatasetId;
                    }
                }
            }
        }

        private WorkflowArtifact _workflowArtifact;

        [ForeignKey("ProducesArtifacts")]
        public virtual WorkflowArtifact WorkflowArtifact
        {
            get
            {
                if (_workflowArtifact == null && !string.IsNullOrEmpty(ProducesArtifacts))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifact - no database context is set. ProducesArtifacts: " + ProducesArtifacts + ".");
                        }
                        return null;
                    }
                    _workflowArtifact = base.SoAContext.WorkflowArtifacts.Find(ProducesArtifacts);
                    if (_workflowArtifact != null)
                    {
                        base.SoAContext.Attach(_workflowArtifact);
                    }
                }
                return _workflowArtifact;
            }
            set
            {
                if (_workflowArtifact != value)
                {
                    _workflowArtifact = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowArtifact != null)
                    {
                        ProducesArtifacts = _workflowArtifact.ArtifactId;
                    }
                }
            }
        }

        private WorkflowArtifact _workflowArtifactRef;

        [ForeignKey("RequiresArtifacts")]
        public virtual WorkflowArtifact WorkflowArtifactRef
        {
            get
            {
                if (_workflowArtifactRef == null && !string.IsNullOrEmpty(RequiresArtifacts))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifactRef - no database context is set. RequiresArtifacts: " + RequiresArtifacts + ".");
                        }
                        return null;
                    }
                    _workflowArtifactRef = base.SoAContext.WorkflowArtifacts.Find(RequiresArtifacts);
                    if (_workflowArtifactRef != null)
                    {
                        base.SoAContext.Attach(_workflowArtifactRef);
                    }
                }
                return _workflowArtifactRef;
            }
            set
            {
                if (_workflowArtifactRef != value)
                {
                    _workflowArtifactRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowArtifactRef != null)
                    {
                        RequiresArtifacts = _workflowArtifactRef.ArtifactId;
                    }
                }
            }
        }

        private ApprovalGate _approvalGateRef;

        [ForeignKey("ApprovalGate")]
        public virtual ApprovalGate ApprovalGateRef
        {
            get
            {
                if (_approvalGateRef == null && !string.IsNullOrEmpty(ApprovalGate))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovalGateRef - no database context is set. ApprovalGate: " + ApprovalGate + ".");
                        }
                        return null;
                    }
                    _approvalGateRef = base.SoAContext.ApprovalGates.Find(ApprovalGate);
                    if (_approvalGateRef != null)
                    {
                        base.SoAContext.Attach(_approvalGateRef);
                    }
                }
                return _approvalGateRef;
            }
            set
            {
                if (_approvalGateRef != value)
                {
                    _approvalGateRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_approvalGateRef != null)
                    {
                        ApprovalGate = _approvalGateRef.ApprovalGateId;
                    }
                }
            }
        }

        private StepPrecedence _stepPrecedence;

        [ForeignKey("Precedes")]
        public virtual StepPrecedence StepPrecedence
        {
            get
            {
                if (_stepPrecedence == null && !string.IsNullOrEmpty(Precedes))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepPrecedence - no database context is set. Precedes: " + Precedes + ".");
                        }
                        return null;
                    }
                    _stepPrecedence = base.SoAContext.StepPrecedence.Find(Precedes);
                    if (_stepPrecedence != null)
                    {
                        base.SoAContext.Attach(_stepPrecedence);
                    }
                }
                return _stepPrecedence;
            }
            set
            {
                if (_stepPrecedence != value)
                {
                    _stepPrecedence = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepPrecedence != null)
                    {
                        Precedes = _stepPrecedence.StepPrecedenceId;
                    }
                }
            }
        }

        private StepPrecedence _stepPrecedenceRef;

        [ForeignKey("PrecededBy")]
        public virtual StepPrecedence StepPrecedenceRef
        {
            get
            {
                if (_stepPrecedenceRef == null && !string.IsNullOrEmpty(PrecededBy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepPrecedenceRef - no database context is set. PrecededBy: " + PrecededBy + ".");
                        }
                        return null;
                    }
                    _stepPrecedenceRef = base.SoAContext.StepPrecedence.Find(PrecededBy);
                    if (_stepPrecedenceRef != null)
                    {
                        base.SoAContext.Attach(_stepPrecedenceRef);
                    }
                }
                return _stepPrecedenceRef;
            }
            set
            {
                if (_stepPrecedenceRef != value)
                {
                    _stepPrecedenceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepPrecedenceRef != null)
                    {
                        PrecededBy = _stepPrecedenceRef.StepPrecedenceId;
                    }
                }
            }
        }

        private ObservableCollection<Workflow> _workflows;

        [InverseProperty("WorkflowStep")]
        public virtual ObservableCollection<Workflow> Workflows
        {
            get
            {
                if (_workflows == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Workflows - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _workflows = new ObservableCollection<Workflow>();
                    }
                    else
                    {
                        var items = base.SoAContext.Workflows.Where(x => x.WorkflowSteps == this.WorkflowStepId).ToList<Workflow>();
                        _workflows = new ObservableCollection<Workflow>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflows.CollectionChanged += Workflows_CollectionChanged;
                }
                return _workflows;
            }
            private set
            {
                if (_workflows != null)
                {
                    _workflows.CollectionChanged -= Workflows_CollectionChanged;
                }
                _workflows = value;
                if (_workflows != null)
                {
                    _workflows.CollectionChanged += Workflows_CollectionChanged;
                }
            }
        }

        private void Workflows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Workflow>())
                {
                    item.WorkflowSteps = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<ApprovalGate> _approvalGates;

        [InverseProperty("WorkflowStepRef")]
        public virtual ObservableCollection<ApprovalGate> ApprovalGates
        {
            get
            {
                if (_approvalGates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovalGates - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _approvalGates = new ObservableCollection<ApprovalGate>();
                    }
                    else
                    {
                        var items = base.SoAContext.ApprovalGates.Where(x => x.WorkflowStep == this.WorkflowStepId).ToList<ApprovalGate>();
                        _approvalGates = new ObservableCollection<ApprovalGate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvalGates.CollectionChanged += ApprovalGates_CollectionChanged;
                }
                return _approvalGates;
            }
            private set
            {
                if (_approvalGates != null)
                {
                    _approvalGates.CollectionChanged -= ApprovalGates_CollectionChanged;
                }
                _approvalGates = value;
                if (_approvalGates != null)
                {
                    _approvalGates.CollectionChanged += ApprovalGates_CollectionChanged;
                }
            }
        }

        private void ApprovalGates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ApprovalGate>())
                {
                    item.WorkflowStep = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<StepPrecedence> _fromStepStepPrecedence;

        [InverseProperty("WorkflowStep")]
        public virtual ObservableCollection<StepPrecedence> FromStepStepPrecedence
        {
            get
            {
                if (_fromStepStepPrecedence == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromStepStepPrecedence - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _fromStepStepPrecedence = new ObservableCollection<StepPrecedence>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepPrecedence.Where(x => x.FromStep == this.WorkflowStepId).ToList<StepPrecedence>();
                        _fromStepStepPrecedence = new ObservableCollection<StepPrecedence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromStepStepPrecedence.CollectionChanged += FromStepStepPrecedence_CollectionChanged;
                }
                return _fromStepStepPrecedence;
            }
            private set
            {
                if (_fromStepStepPrecedence != null)
                {
                    _fromStepStepPrecedence.CollectionChanged -= FromStepStepPrecedence_CollectionChanged;
                }
                _fromStepStepPrecedence = value;
                if (_fromStepStepPrecedence != null)
                {
                    _fromStepStepPrecedence.CollectionChanged += FromStepStepPrecedence_CollectionChanged;
                }
            }
        }

        private void FromStepStepPrecedence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepPrecedence>())
                {
                    item.FromStep = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<StepPrecedence> _toStepStepPrecedence;

        [InverseProperty("WorkflowStepRef")]
        public virtual ObservableCollection<StepPrecedence> ToStepStepPrecedence
        {
            get
            {
                if (_toStepStepPrecedence == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToStepStepPrecedence - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _toStepStepPrecedence = new ObservableCollection<StepPrecedence>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepPrecedence.Where(x => x.ToStep == this.WorkflowStepId).ToList<StepPrecedence>();
                        _toStepStepPrecedence = new ObservableCollection<StepPrecedence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toStepStepPrecedence.CollectionChanged += ToStepStepPrecedence_CollectionChanged;
                }
                return _toStepStepPrecedence;
            }
            private set
            {
                if (_toStepStepPrecedence != null)
                {
                    _toStepStepPrecedence.CollectionChanged -= ToStepStepPrecedence_CollectionChanged;
                }
                _toStepStepPrecedence = value;
                if (_toStepStepPrecedence != null)
                {
                    _toStepStepPrecedence.CollectionChanged += ToStepStepPrecedence_CollectionChanged;
                }
            }
        }

        private void ToStepStepPrecedence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepPrecedence>())
                {
                    item.ToStep = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<Role> _roles;

        [InverseProperty("WorkflowStep")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.WorkflowSteps == this.WorkflowStepId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
                return _roles;
            }
            private set
            {
                if (_roles != null)
                {
                    _roles.CollectionChanged -= Roles_CollectionChanged;
                }
                _roles = value;
                if (_roles != null)
                {
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
            }
        }

        private void Roles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.WorkflowSteps = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<Dataset> _datasets;

        [InverseProperty("WorkflowStep")]
        public virtual ObservableCollection<Dataset> Datasets
        {
            get
            {
                if (_datasets == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Datasets - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _datasets = new ObservableCollection<Dataset>();
                    }
                    else
                    {
                        var items = base.SoAContext.Datasets.Where(x => x.ConsumedBySteps == this.WorkflowStepId).ToList<Dataset>();
                        _datasets = new ObservableCollection<Dataset>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _datasets.CollectionChanged += Datasets_CollectionChanged;
                }
                return _datasets;
            }
            private set
            {
                if (_datasets != null)
                {
                    _datasets.CollectionChanged -= Datasets_CollectionChanged;
                }
                _datasets = value;
                if (_datasets != null)
                {
                    _datasets.CollectionChanged += Datasets_CollectionChanged;
                }
            }
        }

        private void Datasets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Dataset>())
                {
                    item.ConsumedBySteps = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<WorkflowArtifact> _producedByStepWorkflowArtifacts;

        [InverseProperty("WorkflowStep")]
        public virtual ObservableCollection<WorkflowArtifact> ProducedByStepWorkflowArtifacts
        {
            get
            {
                if (_producedByStepWorkflowArtifacts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProducedByStepWorkflowArtifacts - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _producedByStepWorkflowArtifacts = new ObservableCollection<WorkflowArtifact>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowArtifacts.Where(x => x.ProducedByStep == this.WorkflowStepId).ToList<WorkflowArtifact>();
                        _producedByStepWorkflowArtifacts = new ObservableCollection<WorkflowArtifact>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _producedByStepWorkflowArtifacts.CollectionChanged += ProducedByStepWorkflowArtifacts_CollectionChanged;
                }
                return _producedByStepWorkflowArtifacts;
            }
            private set
            {
                if (_producedByStepWorkflowArtifacts != null)
                {
                    _producedByStepWorkflowArtifacts.CollectionChanged -= ProducedByStepWorkflowArtifacts_CollectionChanged;
                }
                _producedByStepWorkflowArtifacts = value;
                if (_producedByStepWorkflowArtifacts != null)
                {
                    _producedByStepWorkflowArtifacts.CollectionChanged += ProducedByStepWorkflowArtifacts_CollectionChanged;
                }
            }
        }

        private void ProducedByStepWorkflowArtifacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowArtifact>())
                {
                    item.ProducedByStep = this.WorkflowStepId;
                }
            }
        }

        private ObservableCollection<WorkflowArtifact> _requiredByStepsWorkflowArtifacts;

        [InverseProperty("WorkflowStepRef")]
        public virtual ObservableCollection<WorkflowArtifact> RequiredByStepsWorkflowArtifacts
        {
            get
            {
                if (_requiredByStepsWorkflowArtifacts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequiredByStepsWorkflowArtifacts - no database context is set. WorkflowStepId: " + this.WorkflowStepId + ".");
                        }
                        _requiredByStepsWorkflowArtifacts = new ObservableCollection<WorkflowArtifact>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowArtifacts.Where(x => x.RequiredBySteps == this.WorkflowStepId).ToList<WorkflowArtifact>();
                        _requiredByStepsWorkflowArtifacts = new ObservableCollection<WorkflowArtifact>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requiredByStepsWorkflowArtifacts.CollectionChanged += RequiredByStepsWorkflowArtifacts_CollectionChanged;
                }
                return _requiredByStepsWorkflowArtifacts;
            }
            private set
            {
                if (_requiredByStepsWorkflowArtifacts != null)
                {
                    _requiredByStepsWorkflowArtifacts.CollectionChanged -= RequiredByStepsWorkflowArtifacts_CollectionChanged;
                }
                _requiredByStepsWorkflowArtifacts = value;
                if (_requiredByStepsWorkflowArtifacts != null)
                {
                    _requiredByStepsWorkflowArtifacts.CollectionChanged += RequiredByStepsWorkflowArtifacts_CollectionChanged;
                }
            }
        }

        private void RequiredByStepsWorkflowArtifacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowArtifact>())
                {
                    item.RequiredBySteps = this.WorkflowStepId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.WorkflowRef;
            _ = this.Role;
            _ = this.Dataset;
            _ = this.WorkflowArtifact;
            _ = this.WorkflowArtifactRef;
            _ = this.ApprovalGateRef;
            _ = this.StepPrecedence;
            _ = this.StepPrecedenceRef;
            _ = this.Workflows;
            _ = this.ApprovalGates;
            _ = this.FromStepStepPrecedence;
            _ = this.ToStepStepPrecedence;
            _ = this.Roles;
            _ = this.Datasets;
            _ = this.ProducedByStepWorkflowArtifacts;
            _ = this.RequiredByStepsWorkflowArtifacts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
