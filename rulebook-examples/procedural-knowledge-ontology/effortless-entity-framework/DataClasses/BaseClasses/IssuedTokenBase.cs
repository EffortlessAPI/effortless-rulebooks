
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
    [Table("IssuedTokens")]
    public class IssuedTokenBase : SoAEntityBase
    {
        [Key]
        public string IssuedTokenId { get; set; }

        // Formula Name (rulebook: ={{AppUser}} & " as " & {{Principal}} & " @ " & {{IssuedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AppUser)), F.S(" as "), F.Text(F.Of(this.Principal)), F.S(" @ "), F.DatetimeText(F.Of(this.IssuedAt))))); set { }
        }

        public DateTimeOffset? IssuedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string? Issuer { get; set; }
        public string? SubjectClaim { get; set; }
        public string? ClaimsSnapshot { get; set; }
        // Formula IsDevMinted (rulebook: ={{Issuer}} = "dev-mint")
        [NotMapped]
        public bool? IsDevMinted
        {
            get => F.AsBool(F.Memo(this, "IsDevMinted", () => F.Eq(F.Nullif(F.Of(this.Issuer)), F.S("dev-mint")))); set { }
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
