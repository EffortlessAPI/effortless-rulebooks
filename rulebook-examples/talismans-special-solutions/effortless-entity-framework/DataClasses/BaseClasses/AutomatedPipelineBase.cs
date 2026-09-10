
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
    [Table("AutomatedPipelines")]
    public class AutomatedPipelineBase : SoAEntityBase
    {
        [Key]
        public string AutomatedPipelineId { get; set; }

        // Formula RelativePath (rulebook: ="automated-pipelines/" & {{AutomatedPipelineId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("automated-pipelines/"), F.TextOr(F.Of(this.AutomatedPipelineId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DisplayName { get; set; }

        public string? Roles { get; set; }
        public string? RoleAssignments { get; set; }

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

        private ObservableCollection<Role> _filledByAutomatedPipelineRoles;

        [InverseProperty("AutomatedPipeline")]
        public virtual ObservableCollection<Role> FilledByAutomatedPipelineRoles
        {
            get
            {
                if (_filledByAutomatedPipelineRoles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FilledByAutomatedPipelineRoles - no database context is set. AutomatedPipelineId: " + this.AutomatedPipelineId + ".");
                        }
                        _filledByAutomatedPipelineRoles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.FilledByAutomatedPipeline == this.AutomatedPipelineId).ToList<Role>();
                        _filledByAutomatedPipelineRoles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _filledByAutomatedPipelineRoles.CollectionChanged += FilledByAutomatedPipelineRoles_CollectionChanged;
                }
                return _filledByAutomatedPipelineRoles;
            }
            private set
            {
                if (_filledByAutomatedPipelineRoles != null)
                {
                    _filledByAutomatedPipelineRoles.CollectionChanged -= FilledByAutomatedPipelineRoles_CollectionChanged;
                }
                _filledByAutomatedPipelineRoles = value;
                if (_filledByAutomatedPipelineRoles != null)
                {
                    _filledByAutomatedPipelineRoles.CollectionChanged += FilledByAutomatedPipelineRoles_CollectionChanged;
                }
            }
        }

        private void FilledByAutomatedPipelineRoles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.FilledByAutomatedPipeline = this.AutomatedPipelineId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _filledByAutomatedPipelineRoleAssignments;

        [InverseProperty("AutomatedPipeline")]
        public virtual ObservableCollection<RoleAssignment> FilledByAutomatedPipelineRoleAssignments
        {
            get
            {
                if (_filledByAutomatedPipelineRoleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FilledByAutomatedPipelineRoleAssignments - no database context is set. AutomatedPipelineId: " + this.AutomatedPipelineId + ".");
                        }
                        _filledByAutomatedPipelineRoleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.FilledByAutomatedPipeline == this.AutomatedPipelineId).ToList<RoleAssignment>();
                        _filledByAutomatedPipelineRoleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _filledByAutomatedPipelineRoleAssignments.CollectionChanged += FilledByAutomatedPipelineRoleAssignments_CollectionChanged;
                }
                return _filledByAutomatedPipelineRoleAssignments;
            }
            private set
            {
                if (_filledByAutomatedPipelineRoleAssignments != null)
                {
                    _filledByAutomatedPipelineRoleAssignments.CollectionChanged -= FilledByAutomatedPipelineRoleAssignments_CollectionChanged;
                }
                _filledByAutomatedPipelineRoleAssignments = value;
                if (_filledByAutomatedPipelineRoleAssignments != null)
                {
                    _filledByAutomatedPipelineRoleAssignments.CollectionChanged += FilledByAutomatedPipelineRoleAssignments_CollectionChanged;
                }
            }
        }

        private void FilledByAutomatedPipelineRoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.FilledByAutomatedPipeline = this.AutomatedPipelineId;
                }
            }
        }

        private ObservableCollection<WorkflowArtifact> _workflowArtifacts;

        [InverseProperty("AutomatedPipeline")]
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
                            throw new InvalidOperationException("Cannot access WorkflowArtifacts - no database context is set. AutomatedPipelineId: " + this.AutomatedPipelineId + ".");
                        }
                        _workflowArtifacts = new ObservableCollection<WorkflowArtifact>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowArtifacts.Where(x => x.AttributedToAutomatedPipeline == this.AutomatedPipelineId).ToList<WorkflowArtifact>();
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
                    item.AttributedToAutomatedPipeline = this.AutomatedPipelineId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.RoleAssignment;
            _ = this.FilledByAutomatedPipelineRoles;
            _ = this.FilledByAutomatedPipelineRoleAssignments;
            _ = this.WorkflowArtifacts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
