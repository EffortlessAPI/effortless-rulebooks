
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
    [Table("MessageTemplates")]
    public class MessageTemplateBase : SoAEntityBase
    {
        [Key]
        public string MessageTemplateId { get; set; }

        // Formula Name (rulebook: ={{CommunicationPolicy}} & " / " & {{Locale}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.CommunicationPolicy)), F.S(" / "), F.Text(F.Of(this.Locale))))); set { }
        }

        public string? SubjectTemplate { get; set; }
        public string? BodyTemplate { get; set; }
        public string? Locale { get; set; }
        public string? Status { get; set; }
        // Formula PolicyMaxMessageLength (rulebook: =INDEX(CommunicationPolicies!{{MaxMessageLength}}, MATCH({{CommunicationPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyMaxMessageLength
        {
            get => F.AsInt(F.Memo(this, "PolicyMaxMessageLength", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.CommunicationPolicy), __r => F.Of(__r.MaxMessageLength), () => F.Of(new CommunicationPolicy().MaxMessageLength))))); set { }
        }

        // Formula PolicyMaxSegments (rulebook: =INDEX(CommunicationPolicies!{{MaxSegments}}, MATCH({{CommunicationPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyMaxSegments
        {
            get => F.AsInt(F.Memo(this, "PolicyMaxSegments", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.CommunicationPolicy), __r => F.Of(__r.MaxSegments), () => F.Of(new CommunicationPolicy().MaxSegments))))); set { }
        }

        // Formula BodyTemplateLength (rulebook: =LEN({{BodyTemplate}}))
        [NotMapped]
        public int? BodyTemplateLength
        {
            get => F.AsInt(F.Memo(this, "BodyTemplateLength", () => F.Integer(F.Len(F.Of(this.BodyTemplate))))); set { }
        }

        // Formula IsTemplateOverLength (rulebook: ={{BodyTemplateLength}} > {{PolicyMaxMessageLength}})
        [NotMapped]
        public bool? IsTemplateOverLength
        {
            get => F.AsBool(F.Memo(this, "IsTemplateOverLength", () => F.Cmp(F.Of(this.BodyTemplateLength), ">", F.Of(this.PolicyMaxMessageLength)))); set { }
        }

        // Formula ValidApprovalCount (rulebook: =COUNTIFS(TemplateApprovals!{{ValidApprovalTemplateKey}}, {{MessageTemplateId}}))
        [NotMapped]
        public decimal? ValidApprovalCount
        {
            get => F.AsDecimal(F.Memo(this, "ValidApprovalCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TemplateApproval>(base.SoAContext, "TemplateApprovals", __c => __c.TemplateApprovals), __r => F.CritField(F.Of(__r.ValidApprovalTemplateKey), F.Of(this.MessageTemplateId)))))); set { }
        }

        // Formula HasValidApproval (rulebook: ={{ValidApprovalCount}} > 0)
        [NotMapped]
        public bool? HasValidApproval
        {
            get => F.AsBool(F.Memo(this, "HasValidApproval", () => F.Cmp(F.Of(this.ValidApprovalCount), ">", F.I(0)))); set { }
        }

        // Formula IsClaimingUnbackedApproval (rulebook: =AND({{Status}} = "Approved", NOT({{HasValidApproval}})))
        [NotMapped]
        public bool? IsClaimingUnbackedApproval
        {
            get => F.AsBool(F.Memo(this, "IsClaimingUnbackedApproval", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasValidApproval))))))); set { }
        }

        public string? CurrentBodyHash { get; set; }
        // Formula LastApprovedBodyHash (rulebook: =INDEX(TemplateApprovals!{{ApprovedBodyHash}}, MATCH({{LastValidApproval}}, TemplateApprovals!{{TemplateApprovalId}}, 0)))
        [NotMapped]
        public string? LastApprovedBodyHash
        {
            get => F.AsString(F.Memo(this, "LastApprovedBodyHash", () => F.Lookup<TemplateApproval>(this, "TemplateApprovals", "TemplateApprovalId", __c => __c.TemplateApprovals, __r => F.Of(__r.TemplateApprovalId), F.Of(this.LastValidApproval), __r => F.Of(__r.ApprovedBodyHash), () => F.Of(new TemplateApproval().ApprovedBodyHash)))); set { }
        }

        public string? LastValidApproval { get; set; }
        // Formula HasBodyDrifted (rulebook: =AND({{LastApprovedBodyHash}} <> "", {{CurrentBodyHash}} <> {{LastApprovedBodyHash}}))
        [NotMapped]
        public bool? HasBodyDrifted
        {
            get => F.AsBool(F.Memo(this, "HasBodyDrifted", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.LastApprovedBodyHash))), F.Bool3(F.Ne(F.Nullif(F.Of(this.CurrentBodyHash)), F.Of(this.LastApprovedBodyHash)))))); set { }
        }

        // Formula IsSendableUnderApproval (rulebook: =AND({{Status}} = "Approved", AND({{HasValidApproval}}, NOT({{HasBodyDrifted}}))))
        [NotMapped]
        public bool? IsSendableUnderApproval
        {
            get => F.AsBool(F.Memo(this, "IsSendableUnderApproval", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.And(F.Bool3(F.Of(this.HasValidApproval)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasBodyDrifted))))))))); set { }
        }

        // Formula DriftedSendCount (rulebook: =COUNTIFS(MessageDeliveries!{{DriftedSendTemplateKey}}, {{MessageTemplateId}}))
        [NotMapped]
        public decimal? DriftedSendCount
        {
            get => F.AsDecimal(F.Memo(this, "DriftedSendCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.DriftedSendTemplateKey), F.Of(this.MessageTemplateId)))))); set { }
        }

        // Formula UnansweredDeliveryCount (rulebook: =COUNTIFS(MessageDeliveries!{{UnansweredTemplateKey}}, {{MessageTemplateId}}))
        [NotMapped]
        public decimal? UnansweredDeliveryCount
        {
            get => F.AsDecimal(F.Memo(this, "UnansweredDeliveryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.UnansweredTemplateKey), F.Of(this.MessageTemplateId)))))); set { }
        }

        // Formula TransmittedDeliveryCount (rulebook: =COUNTIFS(MessageDeliveries!{{TransmittedTemplateKey}}, {{MessageTemplateId}}))
        [NotMapped]
        public decimal? TransmittedDeliveryCount
        {
            get => F.AsDecimal(F.Memo(this, "TransmittedDeliveryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MessageDelivery>(base.SoAContext, "MessageDeliveries", __c => __c.MessageDeliveries), __r => F.CritField(F.Of(__r.TransmittedTemplateKey), F.Of(this.MessageTemplateId)))))); set { }
        }

        // Formula TemplateDrawsNoResponse (rulebook: =AND({{TransmittedDeliveryCount}} > 0, {{UnansweredDeliveryCount}} = {{TransmittedDeliveryCount}}))
        [NotMapped]
        public bool? TemplateDrawsNoResponse
        {
            get => F.AsBool(F.Memo(this, "TemplateDrawsNoResponse", () => F.And(F.Bool3(F.Cmp(F.Of(this.TransmittedDeliveryCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.UnansweredDeliveryCount), F.Of(this.TransmittedDeliveryCount)))))); set { }
        }

        // Formula LastApprovalAt (rulebook: =INDEX(TemplateApprovals!{{DecidedAt}}, MATCH({{LastValidApproval}}, TemplateApprovals!{{TemplateApprovalId}}, 0)))
        [NotMapped]
        public DateTimeOffset? LastApprovalAt
        {
            get => F.AsDateTime(F.Memo(this, "LastApprovalAt", () => F.Lookup<TemplateApproval>(this, "TemplateApprovals", "TemplateApprovalId", __c => __c.TemplateApprovals, __r => F.Of(__r.TemplateApprovalId), F.Of(this.LastValidApproval), __r => F.Of(__r.DecidedAt), () => F.Of(new TemplateApproval().DecidedAt)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? CommunicationPolicy { get; set; }
        public string? Resource { get; set; }

        private CommunicationPolicy _communicationPolicyRef;

        [ForeignKey("CommunicationPolicy")]
        public virtual CommunicationPolicy CommunicationPolicyRef
        {
            get
            {
                if (_communicationPolicyRef == null && !string.IsNullOrEmpty(CommunicationPolicy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunicationPolicyRef - no database context is set. CommunicationPolicy: " + CommunicationPolicy + ".");
                        }
                        return null;
                    }
                    _communicationPolicyRef = base.SoAContext.CommunicationPolicies.Find(CommunicationPolicy);
                    if (_communicationPolicyRef != null)
                    {
                        base.SoAContext.Attach(_communicationPolicyRef);
                    }
                }
                return _communicationPolicyRef;
            }
            set
            {
                if (_communicationPolicyRef != value)
                {
                    _communicationPolicyRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_communicationPolicyRef != null)
                    {
                        CommunicationPolicy = _communicationPolicyRef.CommunicationPolicyId;
                    }
                }
            }
        }

        private Resource _resourceRef;

        [ForeignKey("Resource")]
        public virtual Resource ResourceRef
        {
            get
            {
                if (_resourceRef == null && !string.IsNullOrEmpty(Resource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ResourceRef - no database context is set. Resource: " + Resource + ".");
                        }
                        return null;
                    }
                    _resourceRef = base.SoAContext.Resources.Find(Resource);
                    if (_resourceRef != null)
                    {
                        base.SoAContext.Attach(_resourceRef);
                    }
                }
                return _resourceRef;
            }
            set
            {
                if (_resourceRef != value)
                {
                    _resourceRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resourceRef != null)
                    {
                        Resource = _resourceRef.ResourceId;
                    }
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("MessageTemplateRef")]
        public virtual ObservableCollection<MessageDelivery> MessageDeliveries
        {
            get
            {
                if (_messageDeliveries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<MessageDelivery>();
                        _messageDeliveries = new ObservableCollection<MessageDelivery>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        [InverseProperty("MessageTemplateRef")]
        public virtual ObservableCollection<TemplateApproval> TemplateApprovals
        {
            get
            {
                if (_templateApprovals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = base.SoAContext.TemplateApprovals.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<TemplateApproval>();
                        _templateApprovals = new ObservableCollection<TemplateApproval>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        [InverseProperty("MessageTemplateRef")]
        public virtual ObservableCollection<SendIntent> SendIntents
        {
            get
            {
                if (_sendIntents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<SendIntent>();
                        _sendIntents = new ObservableCollection<SendIntent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        [InverseProperty("MessageTemplateRef")]
        public virtual ObservableCollection<DeliveredCommunication> DeliveredCommunications
        {
            get
            {
                if (_deliveredCommunications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DeliveredCommunications - no database context is set. MessageTemplateId: " + this.MessageTemplateId + ".");
                        }
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>();
                    }
                    else
                    {
                        var items = base.SoAContext.DeliveredCommunications.Where(x => x.MessageTemplate == this.MessageTemplateId).ToList<DeliveredCommunication>();
                        _deliveredCommunications = new ObservableCollection<DeliveredCommunication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
            _ = this.CommunicationPolicyRef;
            _ = this.ResourceRef;
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
