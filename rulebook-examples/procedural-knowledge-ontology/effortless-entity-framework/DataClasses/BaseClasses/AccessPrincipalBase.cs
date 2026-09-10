
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AccessPrincipals")]
    public class AccessPrincipalBase : SoAEntityBase
    {
        [Key]
        public string AccessPrincipalId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? PgRoleName { get; set; }
        public string? SchemaName { get; set; }
        public bool? IsAdministrator { get; set; }
        // Formula OrganizationScope (rulebook: =INDEX(Roles!{{Organization}}, MATCH({{DomainRole}}, Roles!{{RoleId}}, 0)))
        public string? OrganizationScope
        {
            get => INDEX(Roles!this.Organization, MATCH(this.DomainRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula RoleLabel (rulebook: =INDEX(Roles!{{Label}}, MATCH({{DomainRole}}, Roles!{{RoleId}}, 0)))
        public string? RoleLabel
        {
            get => INDEX(Roles!this.Label, MATCH(this.DomainRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula PolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{Principal}}, {{AccessPrincipalId}}))
        public decimal? PolicyCount
        {
            get => COUNTIFS(AccessPolicies!this.Principal, this.AccessPrincipalId); set { }
        }

        // Formula GrantCount (rulebook: =COUNTIFS(FieldGrants!{{Principal}}, {{AccessPrincipalId}}))
        public decimal? GrantCount
        {
            get => COUNTIFS(FieldGrants!this.Principal, this.AccessPrincipalId); set { }
        }

        // Formula VisibleTableCount (rulebook: =COUNTIFS(RoleSchemaViews!{{Principal}}, {{AccessPrincipalId}}))
        public decimal? VisibleTableCount
        {
            get => COUNTIFS(RoleSchemaViews!this.Principal, this.AccessPrincipalId); set { }
        }

        // Formula HasNoAccess (rulebook: ={{PolicyCount}} = 0)
        public bool? HasNoAccess
        {
            get => this.PolicyCount = 0; set { }
        }

        // Formula IsOverPrivileged (rulebook: =AND(NOT({{IsAdministrator}}), {{VisibleTableCount}} >= 74))
        public bool? IsOverPrivileged
        {
            get => AND(NOT(this.IsAdministrator), this.VisibleTableCount >= 74); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? DomainRole { get; set; }

        private Role _role;

        [ForeignKey("DomainRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(DomainRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. DomainRole: " + DomainRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(DomainRole);
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
                    DomainRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private ObservableCollection<AccessPolicy> _accessPolicies;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<AccessPolicy> AccessPolicies
        {
            get
            {
                if (_accessPolicies == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicies - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _accessPolicies = new ObservableCollection<AccessPolicy>();
                    }
                    else
                    {
                        var items = Context.AccessPolicies.Where(x => x.Principal == this.AccessPrincipalId).ToList<AccessPolicy>();
                        _accessPolicies = new ObservableCollection<AccessPolicy>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _accessPolicies.CollectionChanged += AccessPolicies_CollectionChanged;
                }
                return _accessPolicies;
            }
            private set
            {
                if (_accessPolicies != null)
                {
                    _accessPolicies.CollectionChanged -= AccessPolicies_CollectionChanged;
                }
                _accessPolicies = value;
                if (_accessPolicies != null)
                {
                    _accessPolicies.CollectionChanged += AccessPolicies_CollectionChanged;
                }
            }
        }

        private void AccessPolicies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessPolicy>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<FieldGrant> _fieldGrants;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<FieldGrant> FieldGrants
        {
            get
            {
                if (_fieldGrants == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldGrants - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _fieldGrants = new ObservableCollection<FieldGrant>();
                    }
                    else
                    {
                        var items = Context.FieldGrants.Where(x => x.Principal == this.AccessPrincipalId).ToList<FieldGrant>();
                        _fieldGrants = new ObservableCollection<FieldGrant>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _fieldGrants.CollectionChanged += FieldGrants_CollectionChanged;
                }
                return _fieldGrants;
            }
            private set
            {
                if (_fieldGrants != null)
                {
                    _fieldGrants.CollectionChanged -= FieldGrants_CollectionChanged;
                }
                _fieldGrants = value;
                if (_fieldGrants != null)
                {
                    _fieldGrants.CollectionChanged += FieldGrants_CollectionChanged;
                }
            }
        }

        private void FieldGrants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FieldGrant>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<RoleSchema> _roleSchemas;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<RoleSchema> RoleSchemas
        {
            get
            {
                if (_roleSchemas == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemas - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _roleSchemas = new ObservableCollection<RoleSchema>();
                    }
                    else
                    {
                        var items = Context.RoleSchemas.Where(x => x.Principal == this.AccessPrincipalId).ToList<RoleSchema>();
                        _roleSchemas = new ObservableCollection<RoleSchema>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _roleSchemas.CollectionChanged += RoleSchemas_CollectionChanged;
                }
                return _roleSchemas;
            }
            private set
            {
                if (_roleSchemas != null)
                {
                    _roleSchemas.CollectionChanged -= RoleSchemas_CollectionChanged;
                }
                _roleSchemas = value;
                if (_roleSchemas != null)
                {
                    _roleSchemas.CollectionChanged += RoleSchemas_CollectionChanged;
                }
            }
        }

        private void RoleSchemas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleSchema>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<RoleSchemaView> _roleSchemaViews;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<RoleSchemaView> RoleSchemaViews
        {
            get
            {
                if (_roleSchemaViews == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = Context.RoleSchemaViews.Where(x => x.Principal == this.AccessPrincipalId).ToList<RoleSchemaView>();
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _roleSchemaViews.CollectionChanged += RoleSchemaViews_CollectionChanged;
                }
                return _roleSchemaViews;
            }
            private set
            {
                if (_roleSchemaViews != null)
                {
                    _roleSchemaViews.CollectionChanged -= RoleSchemaViews_CollectionChanged;
                }
                _roleSchemaViews = value;
                if (_roleSchemaViews != null)
                {
                    _roleSchemaViews.CollectionChanged += RoleSchemaViews_CollectionChanged;
                }
            }
        }

        private void RoleSchemaViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleSchemaView>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<AccessDenialTest> _accessDenialTests;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<AccessDenialTest> AccessDenialTests
        {
            get
            {
                if (_accessDenialTests == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = Context.AccessDenialTests.Where(x => x.Principal == this.AccessPrincipalId).ToList<AccessDenialTest>();
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _accessDenialTests.CollectionChanged += AccessDenialTests_CollectionChanged;
                }
                return _accessDenialTests;
            }
            private set
            {
                if (_accessDenialTests != null)
                {
                    _accessDenialTests.CollectionChanged -= AccessDenialTests_CollectionChanged;
                }
                _accessDenialTests = value;
                if (_accessDenialTests != null)
                {
                    _accessDenialTests.CollectionChanged += AccessDenialTests_CollectionChanged;
                }
            }
        }

        private void AccessDenialTests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AccessDenialTest>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<PrincipalAssignment> _principalAssignments;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<PrincipalAssignment> PrincipalAssignments
        {
            get
            {
                if (_principalAssignments == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrincipalAssignments - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>();
                    }
                    else
                    {
                        var items = Context.PrincipalAssignments.Where(x => x.Principal == this.AccessPrincipalId).ToList<PrincipalAssignment>();
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _principalAssignments.CollectionChanged += PrincipalAssignments_CollectionChanged;
                }
                return _principalAssignments;
            }
            private set
            {
                if (_principalAssignments != null)
                {
                    _principalAssignments.CollectionChanged -= PrincipalAssignments_CollectionChanged;
                }
                _principalAssignments = value;
                if (_principalAssignments != null)
                {
                    _principalAssignments.CollectionChanged += PrincipalAssignments_CollectionChanged;
                }
            }
        }

        private void PrincipalAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PrincipalAssignment>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }

        private ObservableCollection<IssuedToken> _issuedTokens;

        [InverseProperty("AccessPrincipal")]
        public virtual ObservableCollection<IssuedToken> IssuedTokens
        {
            get
            {
                if (_issuedTokens == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssuedTokens - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _issuedTokens = new ObservableCollection<IssuedToken>();
                    }
                    else
                    {
                        var items = Context.IssuedTokens.Where(x => x.Principal == this.AccessPrincipalId).ToList<IssuedToken>();
                        _issuedTokens = new ObservableCollection<IssuedToken>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _issuedTokens.CollectionChanged += IssuedTokens_CollectionChanged;
                }
                return _issuedTokens;
            }
            private set
            {
                if (_issuedTokens != null)
                {
                    _issuedTokens.CollectionChanged -= IssuedTokens_CollectionChanged;
                }
                _issuedTokens = value;
                if (_issuedTokens != null)
                {
                    _issuedTokens.CollectionChanged += IssuedTokens_CollectionChanged;
                }
            }
        }

        private void IssuedTokens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<IssuedToken>())
                {
                    item.Principal = this.AccessPrincipalId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.AccessPolicies;
            _ = this.FieldGrants;
            _ = this.RoleSchemas;
            _ = this.RoleSchemaViews;
            _ = this.AccessDenialTests;
            _ = this.PrincipalAssignments;
            _ = this.IssuedTokens;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
