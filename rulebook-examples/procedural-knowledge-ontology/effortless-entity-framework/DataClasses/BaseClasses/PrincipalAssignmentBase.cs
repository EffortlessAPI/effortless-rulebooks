
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("PrincipalAssignments")]
    public class PrincipalAssignmentBase : SoAEntityBase
    {
        [Key]
        public string PrincipalAssignmentId { get; set; }

        // Formula Name (rulebook: ={{AppUser}} & " as " & {{Principal}})
        public string? Name
        {
            get => this.AppUser + " as " + this.Principal; set { }
        }

        public bool? IsDefault { get; set; }
        public string? GrantedRationale { get; set; }
        // Formula PrincipalIsAdmin (rulebook: =INDEX(AccessPrincipals!{{IsAdministrator}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0)))
        public bool? PrincipalIsAdmin
        {
            get => INDEX(AccessPrincipals!this.IsAdministrator, MATCH(this.Principal, AccessPrincipals!this.AccessPrincipalId, 0)); set { }
        }

        // Formula UserOrganization (rulebook: =INDEX(AppUsers!{{Organization}}, MATCH({{AppUser}}, AppUsers!{{AppUserId}}, 0)))
        public string? UserOrganization
        {
            get => INDEX(AppUsers!this.Organization, MATCH(this.AppUser, AppUsers!this.AppUserId, 0)); set { }
        }

        // Formula PrincipalOrganization (rulebook: =INDEX(AccessPrincipals!{{OrganizationScope}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0)))
        public string? PrincipalOrganization
        {
            get => INDEX(AccessPrincipals!this.OrganizationScope, MATCH(this.Principal, AccessPrincipals!this.AccessPrincipalId, 0)); set { }
        }

        // Formula IsCrossOrganizationGrant (rulebook: =AND({{UserOrganization}} <> "", {{PrincipalOrganization}} <> "", {{UserOrganization}} <> {{PrincipalOrganization}}))
        public bool? IsCrossOrganizationGrant
        {
            get => AND(this.UserOrganization <> "", this.PrincipalOrganization <> "", this.UserOrganization <> this.PrincipalOrganization); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AppUser { get; set; }
        public string? Principal { get; set; }

        private AppUser _appUser;

        [ForeignKey("AppUser")]
        public virtual AppUser AppUser
        {
            get
            {
                if (_appUser == null && !string.IsNullOrEmpty(AppUser))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppUser - no database context is set. AppUser: " + AppUser + ".");
                        }
                        return null;
                    }
                    _appUser = Context.AppUsers.Find(AppUser);
                    if (_appUser != null)
                    {
                        Context.Attach(_appUser);
                    }
                }
                return _appUser;
            }
            set
            {
                if (_appUser != value)
                {
                    _appUser = value;
                    AppUser = _appUser == null ? default : _appUser.AppUserId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPrincipal - no database context is set. Principal: " + Principal + ".");
                        }
                        return null;
                    }
                    _accessPrincipal = Context.AccessPrincipals.Find(Principal);
                    if (_accessPrincipal != null)
                    {
                        Context.Attach(_accessPrincipal);
                    }
                }
                return _accessPrincipal;
            }
            set
            {
                if (_accessPrincipal != value)
                {
                    _accessPrincipal = value;
                    Principal = _accessPrincipal == null ? default : _accessPrincipal.AccessPrincipalId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppUser;
            _ = this.AccessPrincipal;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
