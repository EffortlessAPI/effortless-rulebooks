
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
    [Table("AssignmentUpdatePolicies")]
    public class AssignmentUpdatePolicyBase : SoAEntityBase
    {
        [Key]
        public string AssignmentUpdatePolicyId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public int? UpdateSlaHours { get; set; }
        public bool? ModelDrivesRouting { get; set; }
        public DateTimeOffset? AdoptedAt { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? TriggerOwnerRole { get; set; }

        private Role _role;

        [ForeignKey("TriggerOwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(TriggerOwnerRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. TriggerOwnerRole: " + TriggerOwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(TriggerOwnerRole);
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
                        TriggerOwnerRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<RoleAssignmentUpdateTask> _roleAssignmentUpdateTasks;

        [InverseProperty("AssignmentUpdatePolicy")]
        public virtual ObservableCollection<RoleAssignmentUpdateTask> RoleAssignmentUpdateTasks
        {
            get
            {
                if (_roleAssignmentUpdateTasks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignmentUpdateTasks - no database context is set. AssignmentUpdatePolicyId: " + this.AssignmentUpdatePolicyId + ".");
                        }
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignmentUpdateTasks.Where(x => x.GoverningPolicy == this.AssignmentUpdatePolicyId).ToList<RoleAssignmentUpdateTask>();
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleAssignmentUpdateTasks.CollectionChanged += RoleAssignmentUpdateTasks_CollectionChanged;
                }
                return _roleAssignmentUpdateTasks;
            }
            private set
            {
                if (_roleAssignmentUpdateTasks != null)
                {
                    _roleAssignmentUpdateTasks.CollectionChanged -= RoleAssignmentUpdateTasks_CollectionChanged;
                }
                _roleAssignmentUpdateTasks = value;
                if (_roleAssignmentUpdateTasks != null)
                {
                    _roleAssignmentUpdateTasks.CollectionChanged += RoleAssignmentUpdateTasks_CollectionChanged;
                }
            }
        }

        private void RoleAssignmentUpdateTasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignmentUpdateTask>())
                {
                    item.GoverningPolicy = this.AssignmentUpdatePolicyId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.RoleAssignmentUpdateTasks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
