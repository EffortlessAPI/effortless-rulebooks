
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("CommunicationPolicies")]
    public class CommunicationPolicyBase : SoAEntityBase
    {
        [Key]
        public string CommunicationPolicyId { get; set; }

        // Formula Name (rulebook: ={{Channel}} & " policy / " & {{ProcedureVersion}})
        public string? Name
        {
            get => this.Channel + " policy / " + this.ProcedureVersion; set { }
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
        public decimal? ConsentViolationCount
        {
            get => COUNTIFS(MessageDeliveries!this.PolicyChannel, this.CommunicationPolicyId, MessageDeliveries!this.IsConsentViolation, TRUE); set { }
        }

        public int? QuietHoursStartHour { get; set; }
        public int? QuietHoursEndHour { get; set; }
        // Formula QuietHoursViolationCount (rulebook: =COUNTIFS(MessageDeliveries!{{QuietHoursViolationPolicyKey}}, {{CommunicationPolicyId}}))
        public decimal? QuietHoursViolationCount
        {
            get => COUNTIFS(MessageDeliveries!this.QuietHoursViolationPolicyKey, this.CommunicationPolicyId); set { }
        }

        public string? RequiredOptOutPhrase { get; set; }
        // Formula IsActivePolicy (rulebook: ={{Status}} = "Active")
        public bool? IsActivePolicy
        {
            get => this.Status = "Active"; set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? ApprovalRole { get; set; }

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

        private ObservableCollection<MessageTemplate> _messageTemplates;

        [InverseProperty("CommunicationPolicy")]
        public virtual ObservableCollection<MessageTemplate> MessageTemplates
        {
            get
            {
                if (_messageTemplates == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageTemplates - no database context is set. CommunicationPolicyId: " + this.CommunicationPolicyId + ".");
                        }
                        _messageTemplates = new ObservableCollection<MessageTemplate>();
                    }
                    else
                    {
                        var items = Context.MessageTemplates.Where(x => x.CommunicationPolicy == this.CommunicationPolicyId).ToList<MessageTemplate>();
                        _messageTemplates = new ObservableCollection<MessageTemplate>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
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
            _ = this.ProcedureVersion;
            _ = this.Role;
            _ = this.MessageTemplates;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
