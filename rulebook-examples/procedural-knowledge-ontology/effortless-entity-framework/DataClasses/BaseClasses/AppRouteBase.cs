
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
    [Table("AppRoutes")]
    public class AppRouteBase : SoAEntityBase
    {
        [Key]
        public string AppRouteId { get; set; }

        // Formula Name (rulebook: ={{RouteName}} & " — " & {{RoutePath}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.RouteName)), F.S(" — "), F.TextOr(F.Of(this.RoutePath))))); set { }
        }

        public string RoutePath { get; set; }
        public string? RouteName { get; set; }
        public string Surface { get; set; }
        public decimal? NavOrder { get; set; }
        public string? RouteKind { get; set; }
        public string? Purpose { get; set; }
        public string? LayoutHints { get; set; }
        // Formula IsInNav (rulebook: ={{NavGroup}} <> "")
        [NotMapped]
        public bool? IsInNav
        {
            get => F.AsBool(F.Memo(this, "IsInNav", () => F.IsNotBlank(F.Of(this.NavGroup)))); set { }
        }

        // Formula IsShared (rulebook: =AND({{OwningRole}} = "", {{Surface}} = "domain"))
        [NotMapped]
        public bool? IsShared
        {
            get => F.AsBool(F.Memo(this, "IsShared", () => F.And(F.Bool3(F.IsBlank(F.Of(this.OwningRole))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Surface)), F.S("domain")))))); set { }
        }

        // Formula IsMaintainer (rulebook: ={{Surface}} = "maintainer")
        [NotMapped]
        public bool? IsMaintainer
        {
            get => F.AsBool(F.Memo(this, "IsMaintainer", () => F.Eq(F.Nullif(F.Of(this.Surface)), F.S("maintainer")))); set { }
        }

        // Formula QuestionCount (rulebook: =COUNTIFS(AppRouteQuestions!{{Route}}, {{AppRouteId}}))
        [NotMapped]
        public decimal? QuestionCount
        {
            get => F.AsDecimal(F.Memo(this, "QuestionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AppRouteQuestion>(base.SoAContext, "AppRouteQuestions", __c => __c.AppRouteQuestions), __r => F.CritField(F.Of(__r.Route), F.Of(this.AppRouteId)))))); set { }
        }

        // Formula ReferenceCount (rulebook: =COUNTIFS(AppRouteReferences!{{FromRoute}}, {{AppRouteId}}))
        [NotMapped]
        public decimal? ReferenceCount
        {
            get => F.AsDecimal(F.Memo(this, "ReferenceCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AppRouteReference>(base.SoAContext, "AppRouteReferences", __c => __c.AppRouteReferences), __r => F.CritField(F.Of(__r.FromRoute), F.Of(this.AppRouteId)))))); set { }
        }

        // Formula AnswersNoQuestion (rulebook: =AND({{QuestionCount}} = 0, {{IsShared}} = FALSE, {{IsMaintainer}} = FALSE, {{RouteKind}} <> "index"))
        [NotMapped]
        public bool? AnswersNoQuestion
        {
            get => F.AsBool(F.Memo(this, "AnswersNoQuestion", () => F.And(F.Bool3(F.Eq(F.Of(this.QuestionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.IsShared), F.B(false))), F.Bool3(F.Eq(F.Of(this.IsMaintainer), F.B(false))), F.Bool3(F.Ne(F.Nullif(F.Of(this.RouteKind)), F.S("index")))))); set { }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwningRole: " + OwningRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(OwningRole);
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
                        OwningRole = _role.RoleId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppNavGroup - no database context is set. NavGroup: " + NavGroup + ".");
                        }
                        return null;
                    }
                    _appNavGroup = base.SoAContext.AppNavGroups.Find(NavGroup);
                    if (_appNavGroup != null)
                    {
                        base.SoAContext.Attach(_appNavGroup);
                    }
                }
                return _appNavGroup;
            }
            set
            {
                if (_appNavGroup != value)
                {
                    _appNavGroup = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_appNavGroup != null)
                    {
                        NavGroup = _appNavGroup.AppNavGroupId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppRouteQuestions - no database context is set. AppRouteId: " + this.AppRouteId + ".");
                        }
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppRouteQuestions.Where(x => x.Route == this.AppRouteId).ToList<AppRouteQuestion>();
                        _appRouteQuestions = new ObservableCollection<AppRouteQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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

        private ObservableCollection<AppRouteReference> _fromRouteAppRouteReferences;

        [InverseProperty("AppRoute")]
        public virtual ObservableCollection<AppRouteReference> FromRouteAppRouteReferences
        {
            get
            {
                if (_fromRouteAppRouteReferences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromRouteAppRouteReferences - no database context is set. AppRouteId: " + this.AppRouteId + ".");
                        }
                        _fromRouteAppRouteReferences = new ObservableCollection<AppRouteReference>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppRouteReferences.Where(x => x.FromRoute == this.AppRouteId).ToList<AppRouteReference>();
                        _fromRouteAppRouteReferences = new ObservableCollection<AppRouteReference>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromRouteAppRouteReferences.CollectionChanged += FromRouteAppRouteReferences_CollectionChanged;
                }
                return _fromRouteAppRouteReferences;
            }
            private set
            {
                if (_fromRouteAppRouteReferences != null)
                {
                    _fromRouteAppRouteReferences.CollectionChanged -= FromRouteAppRouteReferences_CollectionChanged;
                }
                _fromRouteAppRouteReferences = value;
                if (_fromRouteAppRouteReferences != null)
                {
                    _fromRouteAppRouteReferences.CollectionChanged += FromRouteAppRouteReferences_CollectionChanged;
                }
            }
        }

        private void FromRouteAppRouteReferences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppRouteReference>())
                {
                    item.FromRoute = this.AppRouteId;
                }
            }
        }

        private ObservableCollection<AppRouteReference> _toRouteAppRouteReferences;

        [InverseProperty("AppRouteRef")]
        public virtual ObservableCollection<AppRouteReference> ToRouteAppRouteReferences
        {
            get
            {
                if (_toRouteAppRouteReferences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToRouteAppRouteReferences - no database context is set. AppRouteId: " + this.AppRouteId + ".");
                        }
                        _toRouteAppRouteReferences = new ObservableCollection<AppRouteReference>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppRouteReferences.Where(x => x.ToRoute == this.AppRouteId).ToList<AppRouteReference>();
                        _toRouteAppRouteReferences = new ObservableCollection<AppRouteReference>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toRouteAppRouteReferences.CollectionChanged += ToRouteAppRouteReferences_CollectionChanged;
                }
                return _toRouteAppRouteReferences;
            }
            private set
            {
                if (_toRouteAppRouteReferences != null)
                {
                    _toRouteAppRouteReferences.CollectionChanged -= ToRouteAppRouteReferences_CollectionChanged;
                }
                _toRouteAppRouteReferences = value;
                if (_toRouteAppRouteReferences != null)
                {
                    _toRouteAppRouteReferences.CollectionChanged += ToRouteAppRouteReferences_CollectionChanged;
                }
            }
        }

        private void ToRouteAppRouteReferences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            _ = this.FromRouteAppRouteReferences;
            _ = this.ToRouteAppRouteReferences;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
