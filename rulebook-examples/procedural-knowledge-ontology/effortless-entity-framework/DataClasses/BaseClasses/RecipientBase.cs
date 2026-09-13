
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
    [Table("Recipients")]
    public class RecipientBase : SoAEntityBase
    {
        [Key]
        public string RecipientId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.DisplayName))); set { }
        }

        public string? DisplayName { get; set; }
        public string? EmailAddress { get; set; }
        public string? MobileNumber { get; set; }
        public string? SmsConsentStatus { get; set; }
        public DateTimeOffset? SmsConsentAt { get; set; }
        // Formula HasSmsConsent (rulebook: ={{SmsConsentStatus}} = "Granted")
        [NotMapped]
        public bool? HasSmsConsent
        {
            get => F.AsBool(F.Memo(this, "HasSmsConsent", () => F.Eq(F.Nullif(F.Of(this.SmsConsentStatus)), F.S("Granted")))); set { }
        }

        // Formula IsEmailReachable (rulebook: ={{EmailAddress}} <> "")
        [NotMapped]
        public bool? IsEmailReachable
        {
            get => F.AsBool(F.Memo(this, "IsEmailReachable", () => F.IsNotBlank(F.Of(this.EmailAddress)))); set { }
        }

        // Formula IsSmsReachable (rulebook: ={{MobileNumber}} <> "")
        [NotMapped]
        public bool? IsSmsReachable
        {
            get => F.AsBool(F.Memo(this, "IsSmsReachable", () => F.IsNotBlank(F.Of(this.MobileNumber)))); set { }
        }

        // Formula IsUnreachable (rulebook: =AND(NOT({{IsEmailReachable}}), NOT({{IsSmsReachable}})))
        [NotMapped]
        public bool? IsUnreachable
        {
            get => F.AsBool(F.Memo(this, "IsUnreachable", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsEmailReachable)))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsSmsReachable))))))); set { }
        }

        // Formula IsCommunicationallyStranded (rulebook: =AND(NOT({{IsSmsReachable}}), NOT({{IsEmailReachable}})))
        [NotMapped]
        public bool? IsCommunicationallyStranded
        {
            get => F.AsBool(F.Memo(this, "IsCommunicationallyStranded", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsSmsReachable)))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsEmailReachable))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? ConsentBinding { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private OperationalBinding _operationalBinding;

        [ForeignKey("ConsentBinding")]
        public virtual OperationalBinding OperationalBinding
        {
            get
            {
                if (_operationalBinding == null && !string.IsNullOrEmpty(ConsentBinding))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBinding - no database context is set. ConsentBinding: " + ConsentBinding + ".");
                        }
                        return null;
                    }
                    _operationalBinding = base.SoAContext.OperationalBindings.Find(ConsentBinding);
                    if (_operationalBinding != null)
                    {
                        base.SoAContext.Attach(_operationalBinding);
                    }
                }
                return _operationalBinding;
            }
            set
            {
                if (_operationalBinding != value)
                {
                    _operationalBinding = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_operationalBinding != null)
                    {
                        ConsentBinding = _operationalBinding.OperationalBindingId;
                    }
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("RecipientRef")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. RecipientId: " + this.RecipientId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.Recipient == this.RecipientId).ToList<MessageDelivery>();
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
                    item.Recipient = this.RecipientId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("RecipientRef")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. RecipientId: " + this.RecipientId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.Recipient == this.RecipientId).ToList<SendIntent>();
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
                    item.Recipient = this.RecipientId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.OperationalBinding;
            _ = this.MessageDeliveries;
            _ = this.SendIntents;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
