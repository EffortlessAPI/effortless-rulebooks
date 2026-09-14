
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
    [Table("PrincipalAssignments")]
    public class PrincipalAssignmentBase : SoAEntityBase
    {
        [Key]
        public string PrincipalAssignmentId { get; set; }

        // Formula Name (rulebook: ={{AppUser}} & " as " & {{Principal}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AppUser)), F.S(" as "), F.Text(F.Of(this.Principal))))); set { }
        }

        public bool? IsDefault { get; set; }
        public string? GrantedRationale { get; set; }
        // Formula PrincipalIsAdmin (rulebook: =INDEX(AccessPrincipals!{{IsAdministrator}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0)))
        [NotMapped]
        public bool? PrincipalIsAdmin
        {
            get => F.AsBool(F.Memo(this, "PrincipalIsAdmin", () => F.Lookup<AccessPrincipal>(this, "AccessPrincipals", "AccessPrincipalId", __c => __c.AccessPrincipals, __r => F.Of(__r.AccessPrincipalId), F.Of(this.Principal), __r => F.Of(__r.IsAdministrator), () => F.Of(new AccessPrincipal().IsAdministrator)))); set { }
        }

        // Formula UserOrganization (rulebook: =INDEX(AppUsers!{{Organization}}, MATCH({{AppUser}}, AppUsers!{{AppUserId}}, 0)))
        [NotMapped]
        public string? UserOrganization
        {
            get => F.AsString(F.Memo(this, "UserOrganization", () => F.Lookup<AppUser>(this, "AppUsers", "AppUserId", __c => __c.AppUsers, __r => F.Of(__r.AppUserId), F.Of(this.AppUser), __r => F.Of(__r.Organization), () => F.Of(new AppUser().Organization)))); set { }
        }

        // Formula PrincipalOrganization (rulebook: =INDEX(AccessPrincipals!{{OrganizationScope}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0)))
        [NotMapped]
        public string? PrincipalOrganization
        {
            get => F.AsString(F.Memo(this, "PrincipalOrganization", () => F.Lookup<AccessPrincipal>(this, "AccessPrincipals", "AccessPrincipalId", __c => __c.AccessPrincipals, __r => F.Of(__r.AccessPrincipalId), F.Of(this.Principal), __r => F.Of(__r.OrganizationScope), () => F.Of(new AccessPrincipal().OrganizationScope)))); set { }
        }

        // Formula IsCrossOrganizationGrant (rulebook: =AND({{UserOrganization}} <> "", {{PrincipalOrganization}} <> "", {{UserOrganization}} <> {{PrincipalOrganization}}))
        [NotMapped]
        public bool? IsCrossOrganizationGrant
        {
            get => F.AsBool(F.Memo(this, "IsCrossOrganizationGrant", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.UserOrganization))), F.Bool3(F.IsNotBlank(F.Of(this.PrincipalOrganization))), F.Bool3(F.Ne(F.Of(this.UserOrganization), F.Of(this.PrincipalOrganization)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AppUser { get; set; }
        public string? Principal { get; set; }

        private AppUser _appUserRef;

        [ForeignKey("AppUser")]
        public virtual AppUser AppUserRef
        {
            get
            {
                if (_appUserRef == null && !string.IsNullOrEmpty(AppUser))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppUserRef - no database context is set. AppUser: " + AppUser + ".");
                        }
                        return null;
                    }
                    _appUserRef = base.SoAContext.AppUsers.Find(AppUser);
                    if (_appUserRef != null)
                    {
                        base.SoAContext.Attach(_appUserRef);
                    }
                }
                return _appUserRef;
            }
            set
            {
                if (_appUserRef != value)
                {
                    _appUserRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_appUserRef != null)
                    {
                        AppUser = _appUserRef.AppUserId;
                    }
                }
            }
        }

        private AccessPrincipal _accessPrincipal;

        [ForeignKey("Principal")]
        public virtual AccessPrincipal AccessPrincipal
        {
            get
            {
                if (_accessPrincipal == null && !string.IsNullOrEmpty(Principal))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = base.SoAContext.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        base.SoAContext.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_accessPrincipal != null)
                    {
                        Principal = _accessPrincipal.AccessPrincipalId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppUserRef;
            _ = this.AccessPrincipal;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
