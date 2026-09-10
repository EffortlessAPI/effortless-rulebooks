
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppRouteReferences")]
    public class AppRouteReferenceBase : SoAEntityBase
    {
        [Key]
        public string AppRouteReferenceId { get; set; }

        // Formula Name (rulebook: ={{FromRoute}} & " -> " & {{ToRoute}})
        public string? Name
        {
            get => this.FromRoute + " -> " + this.ToRoute; set { }
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoute - no database context is set. FromRoute: " + FromRoute + ".");
                        }
                        return null;
                    }
                    _appRoute = Context.AppRoutes.Find(FromRoute);
                    if (_appRoute != null)
                    {
                        Context.Attach(_appRoute);
                    }
                }
                return _appRoute;
            }
            set
            {
                if (_appRoute != value)
                {
                    _appRoute = value;
                    FromRoute = _appRoute == null ? default : _appRoute.AppRouteId;
                }
            }
        }

        private AppRoute _appRoute;

        [ForeignKey("ToRoute")]
        public virtual AppRoute AppRoute
        {
            get
            {
                if (_appRoute == null && !string.IsNullOrEmpty(ToRoute))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRoute - no database context is set. ToRoute: " + ToRoute + ".");
                        }
                        return null;
                    }
                    _appRoute = Context.AppRoutes.Find(ToRoute);
                    if (_appRoute != null)
                    {
                        Context.Attach(_appRoute);
                    }
                }
                return _appRoute;
            }
            set
            {
                if (_appRoute != value)
                {
                    _appRoute = value;
                    ToRoute = _appRoute == null ? default : _appRoute.AppRouteId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AppRoute;
            _ = this.AppRoute;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
