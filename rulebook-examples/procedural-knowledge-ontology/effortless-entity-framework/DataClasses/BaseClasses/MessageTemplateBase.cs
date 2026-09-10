
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("MessageTemplates")]
    public class MessageTemplateBase : SoAEntityBase
    {
        [Key]
        public string MessageTemplateId { get; set; }

        // Formula Name (rulebook: ={{CommunicationPolicy}} & " / " & {{Locale}})
        public string? Name
        {
            get => this.CommunicationPolicy + " / " + this.Locale; set { }
        }

        public string? SubjectTemplate { get; set; }
        public string? BodyTemplate { get; set; }
        public string? Locale { get; set; }
        public string? Status { get; set; }
        // Formula PolicyMaxMessageLength (rulebook: =INDEX(CommunicationPolicies!{{MaxMessageLength}}, MATCH({{CommunicationPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyMaxMessageLength
        {
            get => INDEX(CommunicationPolicies!this.MaxMessageLength, MATCH(this.CommunicationPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula PolicyMaxSegments (rulebook: =INDEX(CommunicationPolicies!{{MaxSegments}}, MATCH({{CommunicationPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyMaxSegments
        {
            get => INDEX(CommunicationPolicies!this.MaxSegments, MATCH(this.CommunicationPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula BodyTemplateLength (rulebook: =LEN({{BodyTemplate}}))
        public int? BodyTemplateLength
        {
            get => LEN(this.BodyTemplate); set { }
        }

        // Formula IsTemplateOverLength (rulebook: ={{BodyTemplateLength}} > {{PolicyMaxMessageLength}})
        public bool? IsTemplateOverLength
        {
            get => this.BodyTemplateLength > this.PolicyMaxMessageLength; set { }
        }

        // Formula ValidApprovalCount (rulebook: =COUNTIFS(TemplateApprovals!{{ValidApprovalTemplateKey}}, {{MessageTemplateId}}))
        public decimal? ValidApprovalCount
        {
            get => COUNTIFS(TemplateApprovals!this.ValidApprovalTemplateKey, this.MessageTemplateId); set { }
        }

        // Formula HasValidApproval (rulebook: ={{ValidApprovalCount}} > 0)
        public bool? HasValidApproval
        {
            get => this.ValidApprovalCount > 0; set { }
        }

        // Formula IsClaimingUnbackedApproval (rulebook: =AND({{Status}} = "Approved", NOT({{HasValidApproval}})))
        public bool? IsClaimingUnbackedApproval
        {
            get => AND(this.Status = "Approved", NOT(this.HasValidApproval)); set { }
        }

        public string? CurrentBodyHash { get; set; }
        // Formula LastApprovedBodyHash (rulebook: =INDEX(TemplateApprovals!{{ApprovedBodyHash}}, MATCH({{LastValidApproval}}, TemplateApprovals!{{TemplateApprovalId}}, 0)))
        public string? LastApprovedBodyHash
        {
            get => INDEX(TemplateApprovals!this.ApprovedBodyHash, MATCH(this.LastValidApproval, TemplateApprovals!this.TemplateApprovalId, 0)); set { }
        }

        public string? LastValidApproval { get; set; }
        // Formula HasBodyDrifted (rulebook: =AND({{LastApprovedBodyHash}} <> "", {{CurrentBodyHash}} <> {{LastApprovedBodyHash}}))
        public bool? HasBodyDrifted
        {
            get => AND(this.LastApprovedBodyHash <> "", this.CurrentBodyHash <> this.LastApprovedBodyHash); set { }
        }

        // Formula IsSendableUnderApproval (rulebook: =AND({{Status}} = "Approved", AND({{HasValidApproval}}, NOT({{HasBodyDrifted}}))))
        public bool? IsSendableUnderApproval
        {
            get => AND(this.Status = "Approved", AND(this.HasValidApproval, NOT(this.HasBodyDrifted))); set { }
        }

        // Formula DriftedSendCount (rulebook: =COUNTIFS(MessageDeliveries!{{DriftedSendTemplateKey}}, {{MessageTemplateId}}))
        public decimal? DriftedSendCount
        {
            get => COUNTIFS(MessageDeliveries!this.DriftedSendTemplateKey, this.MessageTemplateId); set { }
        }

        // Formula UnansweredDeliveryCount (rulebook: =COUNTIFS(MessageDeliveries!{{UnansweredTemplateKey}}, {{MessageTemplateId}}))
        public decimal? UnansweredDeliveryCount
        {
            get => COUNTIFS(MessageDeliveries!this.UnansweredTemplateKey, this.MessageTemplateId); set { }
        }

        // Formula TransmittedDeliveryCount (rulebook: =COUNTIFS(MessageDeliveries!{{TransmittedTemplateKey}}, {{MessageTemplateId}}))
        public decimal? TransmittedDeliveryCount
        {
            get => COUNTIFS(MessageDeliveries!this.TransmittedTemplateKey, this.MessageTemplateId); set { }
        }

        // Formula TemplateDrawsNoResponse (rulebook: =AND({{TransmittedDeliveryCount}} > 0, {{UnansweredDeliveryCount}} = {{TransmittedDeliveryCount}}))
        public bool? TemplateDrawsNoResponse
        {
            get => AND(this.TransmittedDeliveryCount > 0, this.UnansweredDeliveryCount = this.TransmittedDeliveryCount); set { }
        }

        // Formula LastApprovalAt (rulebook: =INDEX(TemplateApprovals!{{DecidedAt}}, MATCH({{LastValidApproval}}, TemplateApprovals!{{TemplateApprovalId}}, 0)))
        public DateTime? LastApprovalAt
        {
            get => INDEX(TemplateApprovals!this.DecidedAt, MATCH(this.LastValidApproval, TemplateApprovals!this.TemplateApprovalId, 0)); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? CommunicationPolicy { get; set; }
        public string? Resource { get; set; }

        private CommunicationPolicy _communicationPolicy;

        [ForeignKey("CommunicationPolicy")]
        public virtual CommunicationPolicy CommunicationPolicy
        {
            get
            {
                if (_communicationPolicy == null && !string.IsNullOrEmpty(CommunicationPolicy))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunicationPolicy - no database context is set. CommunicationPolicy: " + CommunicationPolicy + ".");
                        }
                        return null;
                    }
                    _communicationPolicy = Context.CommunicationPolicies.Find(CommunicationPolicy);
                    if (_communicationPolicy != null)
                    {
                        Context.Attach(_communicationPolicy);
                    }
                }
                return _communicationPolicy;
            }
            set
            {
                if (_communicationPolicy != value)
                {
                    _communicationPolicy = value;
                    CommunicationPolicy = _communicationPolicy == null ? default : _communicationPolicy.CommunicationPolicyId;
                }
            }
        }

        private Resource _resource;

        [ForeignKey("Resource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(Resource))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resource = Context.Resources.Find(Resource);
                    if (_resource != null)
                    {
                        Context.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    Resource = _resource == null ? default : _resource.ResourceId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("MessageTemplate")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<MessageDelivery>();
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
                    item.MessageTemplate = this.MessageTemplateId;
                }
            }
        }

        private ObservableCollection<TemplateApproval> _templateApprovals;

        [InverseProperty("MessageTemplate")]
        public virtual ObservableCollection<TemplateApproval> TemplateApprovals
        {
            get
            {
                if (_templateApprovals == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = Context.TemplateApprovals.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<TemplateApproval>();
                        _templateApprovals = new ObservableCollection<TemplateApproval>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _templateApprovals.CollectionChanged += TemplateApprovals_CollectionChanged;
                }
                return _templateApprovals;
            }
            private set
            {
                if (_templateApprovals != null)
                {
                    _templateApprovals.CollectionChanged -= TemplateApprovals_CollectionChanged;
                }
                _templateApprovals = value;
                if (_templateApprovals != null)
                {
                    _templateApprovals.CollectionChanged += TemplateApprovals_CollectionChanged;
                }
            }
        }

        private void TemplateApprovals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TemplateApproval>())
                {
                    item.MessageTemplate = this.MessageTemplateId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("MessageTemplate")]
        public virtual ObservableCollection<SendIntent> SendIntents
        {
            get
            {
                if (_sendIntents == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<SendIntent>();
                        _sendIntents = new ObservableCollection<SendIntent>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _sendIntents.CollectionChanged += SendIntents_CollectionChanged;
                }
                return _sendIntents;
            }
            private set
            {
                if (_sendIntents != null)
                {
                    _sendIntents.CollectionChanged -= SendIntents_CollectionChanged;
                }
                _sendIntents = value;
                if (_sendIntents != null)
                {
                    _sendIntents.CollectionChanged += SendIntents_CollectionChanged;
                }
            }
        }

        private void SendIntents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SendIntent>())
                {
                    item.MessageTemplate = this.MessageTemplateId;
                }
            }
        }

        private ObservableCollection<DeliveredCommunication> _deliveredCommunications;

        [InverseProperty("MessageTemplate")]
        public virtual ObservableCollection<DeliveredCommunication> DeliveredCommunications
        {
            get
            {
                if (_deliveredCommunications == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DeliveredCommunications - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = Context.DeliveredCommunications.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<DeliveredCommunication>();
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _deliveredCommunications.CollectionChanged += DeliveredCommunications_CollectionChanged;
                }
                return _deliveredCommunications;
            }
            private set
            {
                if (_deliveredCommunications != null)
                {
                    _deliveredCommunications.CollectionChanged -= DeliveredCommunications_CollectionChanged;
                }
                _deliveredCommunications = value;
                if (_deliveredCommunications != null)
                {
                    _deliveredCommunications.CollectionChanged += DeliveredCommunications_CollectionChanged;
                }
            }
        }

        private void DeliveredCommunications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DeliveredCommunication>())
                {
                    item.MessageTemplate = this.MessageTemplateId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CommunicationPolicy;
            _ = this.Resource;
            _ = this.MessageDeliveries;
            _ = this.TemplateApprovals;
            _ = this.SendIntents;
            _ = this.DeliveredCommunications;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
