
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
    [Table("AppRouteReferences")]
    public class AppRouteReferenceBase : SoAEntityBase
    {
        [Key]
        public string AppRouteReferenceId { get; set; }

        // Formula Name (rulebook: ={{FromRoute}} & " -> " & {{ToRoute}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.FromRoute)), F.S(" -> "), F.TextOr(F.Of(this.ToRoute))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? FromRoute { get; set; }
        public string? ToRoute { get; set; }

        private AppRoute _appRoute;

        [ForeignKey("FromRoute")]
        public virtual AppRoute AppRoute
        {
            get
            {
                if (_appRoute == null && !string.IsNullOrEmpty(FromRoute))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoute - no database context is set. FromRoute: " + FromRoute + ".");
                        }
                        return null;
                    }
                    _appRoute = base.SoAContext.AppRoutes.Find(FromRoute);
                    if (_appRoute != null)
                    {
                        base.SoAContext.Attach(_appRoute);
                    }
                }
                return _appRoute;
            }
            set
            {
                if (_appRoute != value)
                {
                    _appRoute = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_appRoute != null)
                    {
                        FromRoute = _appRoute.AppRouteId;
                    }
                }
            }
        }

        private AppRoute _appRouteRef;

        [ForeignKey("ToRoute")]
        public virtual AppRoute AppRouteRef
        {
            get
            {
                if (_appRouteRef == null && !string.IsNullOrEmpty(ToRoute))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteRef - no database context is set. ToRoute: " + ToRoute + ".");
                        }
                        return null;
                    }
                    _appRouteRef = base.SoAContext.AppRoutes.Find(ToRoute);
                    if (_appRouteRef != null)
                    {
                        base.SoAContext.Attach(_appRouteRef);
                    }
                }
                return _appRouteRef;
            }
            set
            {
                if (_appRouteRef != value)
                {
                    _appRouteRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_appRouteRef != null)
                    {
                        ToRoute = _appRouteRef.AppRouteId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppRoute;
            _ = this.AppRouteRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
