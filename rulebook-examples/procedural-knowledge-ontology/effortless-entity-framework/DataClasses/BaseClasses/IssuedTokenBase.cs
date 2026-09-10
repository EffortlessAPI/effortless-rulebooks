
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("IssuedTokens")]
    public class IssuedTokenBase : SoAEntityBase
    {
        [Key]
        public string IssuedTokenId { get; set; }

        // Formula Name (rulebook: ={{AppUser}} & " as " & {{Principal}} & " @ " & {{IssuedAt}})
        public string? Name
        {
            get => this.AppUser + " as " + this.Principal + " @ " + this.IssuedAt; set { }
        }

        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Issuer { get; set; }
        public string? SubjectClaim { get; set; }
        public string? ClaimsSnapshot { get; set; }
        // Formula IsDevMinted (rulebook: ={{Issuer}} = "dev-mint")
        public bool? IsDevMinted
        {
            get => this.Issuer = "dev-mint"; set { }
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
