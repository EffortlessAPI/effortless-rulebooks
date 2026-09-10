
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Organizations")]
    public class OrganizationBase : SoAEntityBase
    {
        [Key]
        public string OrganizationId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        public string? Name
        {
            get => this.DisplayName; set { }
        }

        public string? DisplayName { get; set; }
        public string? OrganizationType { get; set; }
        public string? ExternalIdentifier { get; set; }
        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<Agent> _agents;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Agent> Agents
        {
            get
            {
                if (_agents == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agents - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _agents = new ObservableCollection<Agent>();
                    }
                    else
                    {
                        var items = Context.Agents.Where(x => x.Organization == this.OrganizationId).ToList<Agent>();
                        _agents = new ObservableCollection<Agent>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _agents.CollectionChanged += Agents_CollectionChanged;
                }
                return _agents;
            }
            private set
            {
                if (_agents != null)
                {
                    _agents.CollectionChanged -= Agents_CollectionChanged;
                }
                _agents = value;
                if (_agents != null)
                {
                    _agents.CollectionChanged += Agents_CollectionChanged;
                }
            }
        }

        private void Agents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Agent>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Role> _roles;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = Context.Roles.Where(x => x.Organization == this.OrganizationId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
                return _roles;
            }
            private set
            {
                if (_roles != null)
                {
                    _roles.CollectionChanged -= Roles_CollectionChanged;
                }
                _roles = value;
                if (_roles != null)
                {
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
            }
        }

        private void Roles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<CommunitiesOfPractice> _communitiesOfPractice;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<CommunitiesOfPractice> CommunitiesOfPractice
        {
            get
            {
                if (_communitiesOfPractice == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>();
                    }
                    else
                    {
                        var items = Context.CommunitiesOfPractice.Where(x => x.Organization == this.OrganizationId).ToList<CommunitiesOfPractice>();
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _communitiesOfPractice.CollectionChanged += CommunitiesOfPractice_CollectionChanged;
                }
                return _communitiesOfPractice;
            }
            private set
            {
                if (_communitiesOfPractice != null)
                {
                    _communitiesOfPractice.CollectionChanged -= CommunitiesOfPractice_CollectionChanged;
                }
                _communitiesOfPractice = value;
                if (_communitiesOfPractice != null)
                {
                    _communitiesOfPractice.CollectionChanged += CommunitiesOfPractice_CollectionChanged;
                }
            }
        }

        private void CommunitiesOfPractice_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CommunitiesOfPractice>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Procedure> _procedures;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedures - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _procedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = Context.Procedures.Where(x => x.OwnerOrganization == this.OrganizationId).ToList<Procedure>();
                        _procedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
                return _procedures;
            }
            private set
            {
                if (_procedures != null)
                {
                    _procedures.CollectionChanged -= Procedures_CollectionChanged;
                }
                _procedures = value;
                if (_procedures != null)
                {
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.OwnerOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Procedure> _procedures;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedures - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _procedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = Context.Procedures.Where(x => x.AdoptedByOrganization == this.OrganizationId).ToList<Procedure>();
                        _procedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
                return _procedures;
            }
            private set
            {
                if (_procedures != null)
                {
                    _procedures.CollectionChanged -= Procedures_CollectionChanged;
                }
                _procedures = value;
                if (_procedures != null)
                {
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.AdoptedByOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Recipient> _recipients;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Recipient> Recipients
        {
            get
            {
                if (_recipients == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Recipients - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _recipients = new ObservableCollection<Recipient>();
                    }
                    else
                    {
                        var items = Context.Recipients.Where(x => x.Organization == this.OrganizationId).ToList<Recipient>();
                        _recipients = new ObservableCollection<Recipient>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _recipients.CollectionChanged += Recipients_CollectionChanged;
                }
                return _recipients;
            }
            private set
            {
                if (_recipients != null)
                {
                    _recipients.CollectionChanged -= Recipients_CollectionChanged;
                }
                _recipients = value;
                if (_recipients != null)
                {
                    _recipients.CollectionChanged += Recipients_CollectionChanged;
                }
            }
        }

        private void Recipients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Recipient>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agents;
            _ = this.Roles;
            _ = this.CommunitiesOfPractice;
            _ = this.Procedures;
            _ = this.Procedures;
            _ = this.Recipients;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
