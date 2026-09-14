
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
    [Table("CommunicationPolicies")]
    public class CommunicationPolicyBase : SoAEntityBase
    {
        [Key]
        public string CommunicationPolicyId { get; set; }

        // Formula Name (rulebook: ={{Channel}} & " policy / " & {{ProcedureVersion}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Channel)), F.S(" policy / "), F.Text(F.Of(this.ProcedureVersion))))); set { }
        }

        public string? Channel { get; set; }
        public string? AudienceRule { get; set; }
        public bool? ConsentRequired { get; set; }
        public string? QuietHoursStart { get; set; }
        public string? QuietHoursEnd { get; set; }
        public int? MaxMessageLength { get; set; }
        public int? MaxSegments { get; set; }
        public int? RetentionDays { get; set; }
        public string? RequiredContent { get; set; }
        public string? AuthorityStatement { get; set; }
        public string? Status { get; set; }
        // Formula ConsentViolationCount (rulebook: =COUNTIFS(MessageDeliveries!{{PolicyChannel}}, {{CommunicationPolicyId}}, MessageDeliveries!{{IsConsentViolation}}, TRUE))
        [NotMapped]
        public decimal? ConsentViolationCount
        {
            get => F.AsDecimal(F.Memo(this, "ConsentViolationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.PolicyChannel), F.Of(this.CommunicationPolicyId)) && F.CritLiteral(F.Of(__r.IsConsentViolation), F.B(true)))))); set { }
        }

        public int? QuietHoursStartHour { get; set; }
        public int? QuietHoursEndHour { get; set; }
        // Formula QuietHoursViolationCount (rulebook: =COUNTIFS(MessageDeliveries!{{QuietHoursViolationPolicyKey}}, {{CommunicationPolicyId}}))
        [NotMapped]
        public decimal? QuietHoursViolationCount
        {
            get => F.AsDecimal(F.Memo(this, "QuietHoursViolationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.QuietHoursViolationPolicyKey), F.Of(this.CommunicationPolicyId)))))); set { }
        }

        public string? RequiredOptOutPhrase { get; set; }
        // Formula IsActivePolicy (rulebook: ={{Status}} = "Active")
        [NotMapped]
        public bool? IsActivePolicy
        {
            get => F.AsBool(F.Memo(this, "IsActivePolicy", () => F.Eq(F.Nullif(F.Of(this.Status)), F.S("Active")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? ApprovalRole { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. ApprovalRole: " + ApprovalRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(ApprovalRole);
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
                        ApprovalRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<MessageTemplate> _messageTemplates;

        [InverseProperty("CommunicationPolicyRef")]
        public virtual ObservableCollection<MessageTemplate> MessageTemplates
        {
            get
            {
                if (_messageTemplates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageTemplates - no database context is set. CommunicationPolicyId: " + this.CommunicationPolicyId + ".");
                        }
                        _messageTemplates = new ObservableCollection<MessageTemplate>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageTemplates.Where(x => x.CommunicationPolicy == this.CommunicationPolicyId).ToList<MessageTemplate>();
                        _messageTemplates = new ObservableCollection<MessageTemplate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _messageTemplates.CollectionChanged += MessageTemplates_CollectionChanged;
                }
                return _messageTemplates;
            }
            private set
            {
                if (_messageTemplates != null)
                {
                    _messageTemplates.CollectionChanged -= MessageTemplates_CollectionChanged;
                }
                _messageTemplates = value;
                if (_messageTemplates != null)
                {
                    _messageTemplates.CollectionChanged += MessageTemplates_CollectionChanged;
                }
            }
        }

        private void MessageTemplates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MessageTemplate>())
                {
                    item.CommunicationPolicy = this.CommunicationPolicyId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Role;
            _ = this.MessageTemplates;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
