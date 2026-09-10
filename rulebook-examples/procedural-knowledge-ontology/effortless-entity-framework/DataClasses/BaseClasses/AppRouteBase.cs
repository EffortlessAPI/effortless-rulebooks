
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("AppRoutes")]
    public class AppRouteBase : SoAEntityBase
    {
        [Key]
        public string AppRouteId { get; set; }

        // Formula Name (rulebook: ={{RouteName}} & " — " & {{RoutePath}})
        public string? Name
        {
            get => this.RouteName + " — " + this.RoutePath; set { }
        }

        public string RoutePath { get; set; }
        public string? RouteName { get; set; }
        public string Surface { get; set; }
        public decimal? NavOrder { get; set; }
        public string? RouteKind { get; set; }
        public string? Purpose { get; set; }
        public string? LayoutHints { get; set; }
        // Formula IsInNav (rulebook: ={{NavGroup}} <> "")
        public bool? IsInNav
        {
            get => this.NavGroup <> ""; set { }
        }

        // Formula IsShared (rulebook: =AND({{OwningRole}} = "", {{Surface}} = "domain"))
        public bool? IsShared
        {
            get => AND(this.OwningRole = "", this.Surface = "domain"); set { }
        }

        // Formula IsMaintainer (rulebook: ={{Surface}} = "maintainer")
        public bool? IsMaintainer
        {
            get => this.Surface = "maintainer"; set { }
        }

        // Formula QuestionCount (rulebook: =COUNTIFS(AppRouteQuestions!{{Route}}, {{AppRouteId}}))
        public decimal? QuestionCount
        {
            get => COUNTIFS(AppRouteQuestions!this.Route, this.AppRouteId); set { }
        }

        // Formula ReferenceCount (rulebook: =COUNTIFS(AppRouteReferences!{{FromRoute}}, {{AppRouteId}}))
        public decimal? ReferenceCount
        {
            get => COUNTIFS(AppRouteReferences!this.FromRoute, this.AppRouteId); set { }
        }

        // Formula AnswersNoQuestion (rulebook: =AND({{QuestionCount}} = 0, {{IsShared}} = FALSE, {{IsMaintainer}} = FALSE, {{RouteKind}} <> "index"))
        public bool? AnswersNoQuestion
        {
            get => AND(this.QuestionCount = 0, this.IsShared = FALSE, this.IsMaintainer = FALSE, this.RouteKind <> "index"); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? OwningRole { get; set; }
        public string? NavGroup { get; set; }

        private Role _role;

        [ForeignKey("OwningRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(OwningRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwningRole: " + OwningRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(OwningRole);
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
                    OwningRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private AppNavGroup _appNavGroup;

        [ForeignKey("NavGroup")]
        public virtual AppNavGroup AppNavGroup
        {
            get
            {
                if (_appNavGroup == null && !string.IsNullOrEmpty(NavGroup))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppNavGroup - no database context is set. NavGroup: " + NavGroup + ".");
                        }
                        return null;
                    }
                    _appNavGroup = Context.AppNavGroups.Find(NavGroup);
                    if (_appNavGroup != null)
                    {
                        Context.Attach(_appNavGroup);
                    }
                }
                return _appNavGroup;
            }
            set
            {
                if (_appNavGroup != value)
                {
                    _appNavGroup = value;
                    NavGroup = _appNavGroup == null ? default : _appNavGroup.AppNavGroupId;
                }
            }
        }

        private ObservableCollection<AppRouteQuestion> _appRouteQuestions;

        [InverseProperty("AppRoute")]
        public virtual ObservableCollection<AppRouteQuestion> AppRouteQuestions
        {
            get
            {
                if (_appRouteQuestions == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteQuestions - no database context is set. AppRouteId: " + this.AppRouteId + ".");
                        }
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>();
                    }
                    else
                    {
                        var items = Context.AppRouteQuestions.Where(x => x.Route == this.AppRouteId).ToList<AppRouteQuestion>();
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _appRouteQuestions.CollectionChanged += AppRouteQuestions_CollectionChanged;
                }
                return _appRouteQuestions;
            }
            private set
            {
                if (_appRouteQuestions != null)
                {
                    _appRouteQuestions.CollectionChanged -= AppRouteQuestions_CollectionChanged;
                }
                _appRouteQuestions = value;
                if (_appRouteQuestions != null)
                {
                    _appRouteQuestions.CollectionChanged += AppRouteQuestions_CollectionChanged;
                }
            }
        }

        private void AppRouteQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRouteQuestion>())
                {
                    item.Route = this.AppRouteId;
                }
            }
        }

        private ObservableCollection<AppRouteReference> _appRouteReferences;

        [InverseProperty("AppRoute")]
        public virtual ObservableCollection<AppRouteReference> AppRouteReferences
        {
            get
            {
                if (_appRouteReferences == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteReferences - no database context is set. AppRouteId: " + this.AppRouteId + ".");
                        }
                        _appRouteReferences = new ObservableCollection<AppRouteReference>();
                    }
                    else
                    {
                        var items = Context.AppRouteReferences.Where(x => x.FromRoute == this.AppRouteId).ToList<AppRouteReference>();
                        _appRouteReferences = new ObservableCollection<AppRouteReference>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _appRouteReferences.CollectionChanged += AppRouteReferences_CollectionChanged;
                }
                return _appRouteReferences;
            }
            private set
            {
                if (_appRouteReferences != null)
                {
                    _appRouteReferences.CollectionChanged -= AppRouteReferences_CollectionChanged;
                }
                _appRouteReferences = value;
                if (_appRouteReferences != null)
                {
                    _appRouteReferences.CollectionChanged += AppRouteReferences_CollectionChanged;
                }
            }
        }

        private void AppRouteReferences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRouteReference>())
                {
                    item.FromRoute = this.AppRouteId;
                }
            }
        }

        private ObservableCollection<AppRouteReference> _appRouteReferences;

        [InverseProperty("AppRoute")]
        public virtual ObservableCollection<AppRouteReference> AppRouteReferences
        {
            get
            {
                if (_appRouteReferences == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteReferences - no database context is set. AppRouteId: " + this.AppRouteId + ".");
                        }
                        _appRouteReferences = new ObservableCollection<AppRouteReference>();
                    }
                    else
                    {
                        var items = Context.AppRouteReferences.Where(x => x.ToRoute == this.AppRouteId).ToList<AppRouteReference>();
                        _appRouteReferences = new ObservableCollection<AppRouteReference>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _appRouteReferences.CollectionChanged += AppRouteReferences_CollectionChanged;
                }
                return _appRouteReferences;
            }
            private set
            {
                if (_appRouteReferences != null)
                {
                    _appRouteReferences.CollectionChanged -= AppRouteReferences_CollectionChanged;
                }
                _appRouteReferences = value;
                if (_appRouteReferences != null)
                {
                    _appRouteReferences.CollectionChanged += AppRouteReferences_CollectionChanged;
                }
            }
        }

        private void AppRouteReferences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRouteReference>())
                {
                    item.ToRoute = this.AppRouteId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.AppNavGroup;
            _ = this.AppRouteQuestions;
            _ = this.AppRouteReferences;
            _ = this.AppRouteReferences;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
