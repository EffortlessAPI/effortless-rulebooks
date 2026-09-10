
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Recipients")]
    public class RecipientBase : SoAEntityBase
    {
        [Key]
        public string RecipientId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        public string? Name
        {
            get => this.DisplayName; set { }
        }

        public string? DisplayName { get; set; }
        public string? EmailAddress { get; set; }
        public string? MobileNumber { get; set; }
        public string? SmsConsentStatus { get; set; }
        public DateTime? SmsConsentAt { get; set; }
        // Formula HasSmsConsent (rulebook: ={{SmsConsentStatus}} = "Granted")
        public bool? HasSmsConsent
        {
            get => this.SmsConsentStatus = "Granted"; set { }
        }

        // Formula IsEmailReachable (rulebook: ={{EmailAddress}} <> "")
        public bool? IsEmailReachable
        {
            get => this.EmailAddress <> ""; set { }
        }

        // Formula IsSmsReachable (rulebook: ={{MobileNumber}} <> "")
        public bool? IsSmsReachable
        {
            get => this.MobileNumber <> ""; set { }
        }

        // Formula IsUnreachable (rulebook: =AND(NOT({{IsEmailReachable}}), NOT({{IsSmsReachable}})))
        public bool? IsUnreachable
        {
            get => AND(NOT(this.IsEmailReachable), NOT(this.IsSmsReachable)); set { }
        }

        // Formula IsCommunicationallyStranded (rulebook: =AND(NOT({{IsSmsReachable}}), NOT({{IsEmailReachable}})))
        public bool? IsCommunicationallyStranded
        {
            get => AND(NOT(this.IsSmsReachable), NOT(this.IsEmailReachable)); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? ConsentBinding { get; set; }

        private Organization _organization;

        [ForeignKey("Organization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(Organization))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organization = Context.Organizations.Find(Organization);
                    if (_organization != null)
                    {
                        Context.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    Organization = _organization == null ? default : _organization.OrganizationId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBinding - no database context is set. ConsentBinding: " + ConsentBinding + ".");
                        }
                        return null;
                    }
                    _operationalBinding = Context.OperationalBindings.Find(ConsentBinding);
                    if (_operationalBinding != null)
                    {
                        Context.Attach(_operationalBinding);
                    }
                }
                return _operationalBinding;
            }
            set
            {
                if (_operationalBinding != value)
                {
                    _operationalBinding = value;
                    ConsentBinding = _operationalBinding == null ? default : _operationalBinding.OperationalBindingId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("Recipient")]
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
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. RecipientId: " + this.RecipientId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = Context.MessageDeliveries.Where(x => x.Recipient == this.RecipientId).ToList<MessageDelivery>();
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
                    item.Recipient = this.RecipientId;
                }
            }
        }

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("Recipient")]
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
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. RecipientId: " + this.RecipientId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.Recipient == this.RecipientId).ToList<SendIntent>();
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
                    item.Recipient = this.RecipientId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Organization;
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
