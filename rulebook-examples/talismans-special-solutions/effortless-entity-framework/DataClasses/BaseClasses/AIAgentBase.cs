
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
    [Table("AIAgents")]
    public class AIAgentBase : SoAEntityBase
    {
        [Key]
        public string AIAgentId { get; set; }

        // Formula RelativePath (rulebook: ="ai-agents/" & {{AIAgentId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("ai-agents/"), F.TextOr(F.Of(this.AIAgentId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? DisplayName { get; set; }
        public string? ModelVersion { get; set; }
        public DateOnly? DeployedOn { get; set; }
        // Formula CountAttributedArtifacts (rulebook: =COUNTIFS(WorkflowArtifacts!{{AttributedToAIAgent}}, AIAgents!{{AIAgentId}}))
        [NotMapped]
        public int? CountAttributedArtifacts
        {
            get => F.AsInt(F.Memo(this, "CountAttributedArtifacts", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowArtifact>(base.SoAContext, "WorkflowArtifacts", __c => __c.WorkflowArtifacts), __r => F.CritField(F.Of(__r.AttributedToAIAgent), F.Of(this.AIAgentId))))))); set { }
        }

        // Formula CountImpactedWorkflows (rulebook: =COUNTIFS(WorkflowArtifacts!{{AttributedToAIAgent}}, AIAgents!{{AIAgentId}}, WorkflowArtifacts!{{HasProducingWorkflow}}, TRUE))
        [NotMapped]
        public int? CountImpactedWorkflows
        {
            get => F.AsInt(F.Memo(this, "CountImpactedWorkflows", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<WorkflowArtifact>(base.SoAContext, "WorkflowArtifacts", __c => __c.WorkflowArtifacts), __r => F.CritField(F.Of(__r.AttributedToAIAgent), F.Of(this.AIAgentId)) && F.CritLiteral(F.Of(__r.HasProducingWorkflow), F.B(true))))))); set { }
        }


        public string? Roles { get; set; }
        public string? RoleAssignments { get; set; }
        public string? AttributedArtifacts { get; set; }

        private Role _role;

        [ForeignKey("Roles")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(Roles))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. Roles: " + Roles + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(Roles);
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
                        Roles = _role.RoleId;
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

        private WorkflowArtifact _workflowArtifact;

        [ForeignKey("AttributedArtifacts")]
        public virtual WorkflowArtifact WorkflowArtifact
        {
            get
            {
                if (_workflowArtifact == null && !string.IsNullOrEmpty(AttributedArtifacts))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifact - no database context is set. AttributedArtifacts: " + AttributedArtifacts + ".");
                        }
                        return null;
                    }
                    _workflowArtifact = base.SoAContext.WorkflowArtifacts.Find(AttributedArtifacts);
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
                        AttributedArtifacts = _workflowArtifact.ArtifactId;
                    }
                }
            }
        }

        private ObservableCollection<Role> _filledByAIAgentRoles;

        [InverseProperty("AIAgent")]
        public virtual ObservableCollection<Role> FilledByAIAgentRoles
        {
            get
            {
                if (_filledByAIAgentRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FilledByAIAgentRoles - no database context is set. AIAgentId: " + this.AIAgentId + ".");
                        }
                        _filledByAIAgentRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.FilledByAIAgent == this.AIAgentId).ToList<Role>();
                        _filledByAIAgentRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _filledByAIAgentRoles.CollectionChanged += FilledByAIAgentRoles_CollectionChanged;
                }
                return _filledByAIAgentRoles;
            }
            private set
            {
                if (_filledByAIAgentRoles != null)
                {
                    _filledByAIAgentRoles.CollectionChanged -= FilledByAIAgentRoles_CollectionChanged;
                }
                _filledByAIAgentRoles = value;
                if (_filledByAIAgentRoles != null)
                {
                    _filledByAIAgentRoles.CollectionChanged += FilledByAIAgentRoles_CollectionChanged;
                }
            }
        }

        private void FilledByAIAgentRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.FilledByAIAgent = this.AIAgentId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _filledByAIAgentRoleAssignments;

        [InverseProperty("AIAgent")]
        public virtual ObservableCollection<RoleAssignment> FilledByAIAgentRoleAssignments
        {
            get
            {
                if (_filledByAIAgentRoleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FilledByAIAgentRoleAssignments - no database context is set. AIAgentId: " + this.AIAgentId + ".");
                        }
                        _filledByAIAgentRoleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.FilledByAIAgent == this.AIAgentId).ToList<RoleAssignment>();
                        _filledByAIAgentRoleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _filledByAIAgentRoleAssignments.CollectionChanged += FilledByAIAgentRoleAssignments_CollectionChanged;
                }
                return _filledByAIAgentRoleAssignments;
            }
            private set
            {
                if (_filledByAIAgentRoleAssignments != null)
                {
                    _filledByAIAgentRoleAssignments.CollectionChanged -= FilledByAIAgentRoleAssignments_CollectionChanged;
                }
                _filledByAIAgentRoleAssignments = value;
                if (_filledByAIAgentRoleAssignments != null)
                {
                    _filledByAIAgentRoleAssignments.CollectionChanged += FilledByAIAgentRoleAssignments_CollectionChanged;
                }
            }
        }

        private void FilledByAIAgentRoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.FilledByAIAgent = this.AIAgentId;
                }
            }
        }

        private ObservableCollection<WorkflowArtifact> _workflowArtifacts;

        [InverseProperty("AIAgent")]
        public virtual ObservableCollection<WorkflowArtifact> WorkflowArtifacts
        {
            get
            {
                if (_workflowArtifacts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifacts - no database context is set. AIAgentId: " + this.AIAgentId + ".");
                        }
                        _workflowArtifacts = new ObservableCollection<WorkflowArtifact>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowArtifacts.Where(x => x.AttributedToAIAgent == this.AIAgentId).ToList<WorkflowArtifact>();
                        _workflowArtifacts = new ObservableCollection<WorkflowArtifact>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowArtifacts.CollectionChanged += WorkflowArtifacts_CollectionChanged;
                }
                return _workflowArtifacts;
            }
            private set
            {
                if (_workflowArtifacts != null)
                {
                    _workflowArtifacts.CollectionChanged -= WorkflowArtifacts_CollectionChanged;
                }
                _workflowArtifacts = value;
                if (_workflowArtifacts != null)
                {
                    _workflowArtifacts.CollectionChanged += WorkflowArtifacts_CollectionChanged;
                }
            }
        }

        private void WorkflowArtifacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowArtifact>())
                {
                    item.AttributedToAIAgent = this.AIAgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.RoleAssignment;
            _ = this.WorkflowArtifact;
            _ = this.FilledByAIAgentRoles;
            _ = this.FilledByAIAgentRoleAssignments;
            _ = this.WorkflowArtifacts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
