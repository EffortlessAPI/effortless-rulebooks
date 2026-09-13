
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
    [Table("AccessPrincipals")]
    public class AccessPrincipalBase : SoAEntityBase
    {
        [Key]
        public string AccessPrincipalId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? PgRoleName { get; set; }
        public string? SchemaName { get; set; }
        public bool? IsAdministrator { get; set; }
        // Formula OrganizationScope (rulebook: =INDEX(Roles!{{Organization}}, MATCH({{DomainRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? OrganizationScope
        {
            get => F.AsString(F.Memo(this, "OrganizationScope", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.DomainRole), __r => F.Of(__r.Organization), () => F.Of(new Role().Organization)))); set { }
        }

        // Formula RoleLabel (rulebook: =INDEX(Roles!{{Label}}, MATCH({{DomainRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? RoleLabel
        {
            get => F.AsString(F.Memo(this, "RoleLabel", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.DomainRole), __r => F.Of(__r.Label), () => F.Of(new Role().Label)))); set { }
        }

        // Formula PolicyCount (rulebook: =COUNTIFS(AccessPolicies!{{Principal}}, {{AccessPrincipalId}}))
        [NotMapped]
        public decimal? PolicyCount
        {
            get => F.AsDecimal(F.Memo(this, "PolicyCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessPolicy>(base.SoAContext, "AccessPolicies", __c => __c.AccessPolicies), __r => F.CritField(F.Of(__r.Principal), F.Of(this.AccessPrincipalId)))))); set { }
        }

        // Formula GrantCount (rulebook: =COUNTIFS(FieldGrants!{{Principal}}, {{AccessPrincipalId}}))
        [NotMapped]
        public decimal? GrantCount
        {
            get => F.AsDecimal(F.Memo(this, "GrantCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FieldGrant>(base.SoAContext, "FieldGrants", __c => __c.FieldGrants), __r => F.CritField(F.Of(__r.Principal), F.Of(this.AccessPrincipalId)))))); set { }
        }

        // Formula VisibleTableCount (rulebook: =COUNTIFS(RoleSchemaViews!{{Principal}}, {{AccessPrincipalId}}))
        [NotMapped]
        public decimal? VisibleTableCount
        {
            get => F.AsDecimal(F.Memo(this, "VisibleTableCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleSchemaView>(base.SoAContext, "RoleSchemaViews", __c => __c.RoleSchemaViews), __r => F.CritField(F.Of(__r.Principal), F.Of(this.AccessPrincipalId)))))); set { }
        }

        // Formula HasNoAccess (rulebook: ={{PolicyCount}} = 0)
        [NotMapped]
        public bool? HasNoAccess
        {
            get => F.AsBool(F.Memo(this, "HasNoAccess", () => F.Eq(F.Of(this.PolicyCount), F.I(0)))); set { }
        }

        // Formula IsOverPrivileged (rulebook: =AND(NOT({{IsAdministrator}}), {{VisibleTableCount}} >= 74))
        [NotMapped]
        public bool? IsOverPrivileged
        {
            get => F.AsBool(F.Memo(this, "IsOverPrivileged", () => F.And(F.Bool3(F.Not(F.IsTrueV(F.Of(this.IsAdministrator)))), F.Bool3(F.Cmp(F.Of(this.VisibleTableCount), ">=", F.I(74)))))); set { }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. DomainRole: " + DomainRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(DomainRole);
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
                        DomainRole = _role.RoleId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicies - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _accessPolicies = new ObservableCollection<AccessPolicy>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessPolicies.Where(x => x.Principal == this.AccessPrincipalId).ToList<AccessPolicy>();
                        _accessPolicies = new ObservableCollection<AccessPolicy>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldGrants - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _fieldGrants = new ObservableCollection<FieldGrant>();
                    }
                    else
                    {
                        var items = base.SoAContext.FieldGrants.Where(x => x.Principal == this.AccessPrincipalId).ToList<FieldGrant>();
                        _fieldGrants = new ObservableCollection<FieldGrant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemas - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _roleSchemas = new ObservableCollection<RoleSchema>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleSchemas.Where(x => x.Principal == this.AccessPrincipalId).ToList<RoleSchema>();
                        _roleSchemas = new ObservableCollection<RoleSchema>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleSchemaViews - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleSchemaViews.Where(x => x.Principal == this.AccessPrincipalId).ToList<RoleSchemaView>();
                        _roleSchemaViews = new ObservableCollection<RoleSchemaView>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessDenialTests.Where(x => x.Principal == this.AccessPrincipalId).ToList<AccessDenialTest>();
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrincipalAssignments - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.PrincipalAssignments.Where(x => x.Principal == this.AccessPrincipalId).ToList<PrincipalAssignment>();
                        _principalAssignments = new ObservableCollection<PrincipalAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssuedTokens - no database context is set. AccessPrincipalId: " + this.AccessPrincipalId + ".");
                        }
                        _issuedTokens = new ObservableCollection<IssuedToken>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssuedTokens.Where(x => x.Principal == this.AccessPrincipalId).ToList<IssuedToken>();
                        _issuedTokens = new ObservableCollection<IssuedToken>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
