
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Exceptions")]
    public class ExceptionBase : SoAEntityBase
    {
        [Key]
        public string ExceptionId { get; set; }

        // Formula Name (rulebook: ={{Condition}})
        public string? Name
        {
            get => this.Condition; set { }
        }

        public string? Condition { get; set; }
        public string? Handling { get; set; }
        public string? Status { get; set; }
        // Formula ActiveExceptionStepKey (rulebook: =IF({{Status}} = "Active", {{TriggerStep}}, ""))
        public string? ActiveExceptionStepKey
        {
            get => IF(this.Status = "Active", this.TriggerStep, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? TriggerStep { get; set; }
        public string? ApprovalRole { get; set; }
        public string? FallbackRole { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private Step _step;

        [ForeignKey("TriggerStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(TriggerStep))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. TriggerStep: " + TriggerStep + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(TriggerStep);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    TriggerStep = _step == null ? default : _step.StepId;
                }
            }
        }

        private Role _role;

        [ForeignKey("ApprovalRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(ApprovalRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. ApprovalRole: " + ApprovalRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(ApprovalRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    ApprovalRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private Role _role;

        [ForeignKey("FallbackRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(FallbackRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. FallbackRole: " + FallbackRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(FallbackRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    FallbackRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _exceptionInvocations;

        [InverseProperty("Exception")]
        public virtual ObservableCollection<ExceptionInvocation> ExceptionInvocations
        {
            get
            {
                if (_exceptionInvocations == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExceptionInvocations - no database context is set. ExceptionId: " + this.ExceptionId + ".");
                        }
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = Context.ExceptionInvocations.Where(x => x.Exception == this.ExceptionId).ToList<ExceptionInvocation>();
                        _exceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
                return _exceptionInvocations;
            }
            private set
            {
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged -= ExceptionInvocations_CollectionChanged;
                }
                _exceptionInvocations = value;
                if (_exceptionInvocations != null)
                {
                    _exceptionInvocations.CollectionChanged += ExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void ExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.Exception = this.ExceptionId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("Exception")]
        public virtual ObservableCollection<MessageDelivery> MessageDeliveries
        {
            get
            {
                if (_messageDeliveries == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. ExceptionId: " + this.ExceptionId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.InvokedException == this.ExceptionId).ToList<MessageDelivery>();
                        _messageDeliveries = new ObservableCollection<MessageDelivery>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _messageDeliveries.CollectionChanged += MessageDeliveries_CollectionChanged;
                }
                return _messageDeliveries;
            }
            private set
            {
                if (_messageDeliveries != null)
                {
                    _messageDeliveries.CollectionChanged -= MessageDeliveries_CollectionChanged;
                }
                _messageDeliveries = value;
                if (_messageDeliveries != null)
                {
                    _messageDeliveries.CollectionChanged += MessageDeliveries_CollectionChanged;
                }
            }
        }

        private void MessageDeliveries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MessageDelivery>())
                {
                    item.InvokedException = this.ExceptionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.Role;
            _ = this.Role;
            _ = this.ExceptionInvocations;
            _ = this.MessageDeliveries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
