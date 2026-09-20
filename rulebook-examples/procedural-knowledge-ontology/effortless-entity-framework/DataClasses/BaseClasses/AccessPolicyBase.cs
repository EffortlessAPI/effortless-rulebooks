
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
    [Table("AccessPolicies")]
    public class AccessPolicyBase : SoAEntityBase
    {
        [Key]
        public string AccessPolicyId { get; set; }

        // Formula Name (rulebook: ={{Principal}} & " " & {{Command}} & " " & {{TargetTable}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Principal)), F.S(" "), F.Text(F.Of(this.Command)), F.S(" "), F.Text(F.Of(this.TargetTable))))); set { }
        }

        public string? Command { get; set; }
        public string? RowPredicate { get; set; }
        public string? CheckPredicate { get; set; }
        public string? Rationale { get; set; }
        public bool? ReferencesInference { get; set; }
        // Formula IsWriteCommand (rulebook: =OR({{Command}} = "INSERT", {{Command}} = "UPDATE", {{Command}} = "DELETE", {{Command}} = "ALL"))
        [NotMapped]
        public bool? IsWriteCommand
        {
            get => F.AsBool(F.Memo(this, "IsWriteCommand", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Command)), F.S("INSERT"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Command)), F.S("UPDATE"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Command)), F.S("DELETE"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Command)), F.S("ALL")))))); set { }
        }

        // Formula IsUnrestricted (rulebook: ={{RowPredicate}} = "")
        [NotMapped]
        public bool? IsUnrestricted
        {
            get => F.AsBool(F.Memo(this, "IsUnrestricted", () => F.IsBlank(F.Of(this.RowPredicate)))); set { }
        }

        // Formula PrincipalIsAdmin (rulebook: =INDEX(AccessPrincipals!{{IsAdministrator}}, MATCH({{Principal}}, AccessPrincipals!{{AccessPrincipalId}}, 0)))
        [NotMapped]
        public bool? PrincipalIsAdmin
        {
            get => F.AsBool(F.Memo(this, "PrincipalIsAdmin", () => F.Lookup<AccessPrincipal>(this, "AccessPrincipals", "AccessPrincipalId", __c => __c.AccessPrincipals, __r => F.Of(__r.AccessPrincipalId), F.Of(this.Principal), __r => F.Of(__r.IsAdministrator), () => F.Of(new AccessPrincipal().IsAdministrator)))); set { }
        }

        // Formula IsUnrestrictedNonAdminGrant (rulebook: =AND({{IsUnrestricted}}, NOT({{PrincipalIsAdmin}})))
        [NotMapped]
        public bool? IsUnrestrictedNonAdminGrant
        {
            get => F.AsBool(F.Memo(this, "IsUnrestrictedNonAdminGrant", () => F.And(F.Bool3(F.Of(this.IsUnrestricted)), F.Bool3(F.Not(F.Bool3(F.Of(this.PrincipalIsAdmin))))))); set { }
        }

        // Formula IsUnwitnessedWrite (rulebook: =AND({{IsWriteCommand}}, {{DenialTestCount}} = 0))
        [NotMapped]
        public bool? IsUnwitnessedWrite
        {
            get => F.AsBool(F.Memo(this, "IsUnwitnessedWrite", () => F.And(F.Bool3(F.Of(this.IsWriteCommand)), F.Bool3(F.Eq(F.Of(this.DenialTestCount), F.I(0)))))); set { }
        }

        // Formula DenialTestCount (rulebook: =COUNTIFS(AccessDenialTests!{{TargetPolicy}}, {{AccessPolicyId}}))
        [NotMapped]
        public decimal? DenialTestCount
        {
            get => F.AsDecimal(F.Memo(this, "DenialTestCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AccessDenialTest>(base.SoAContext, "AccessDenialTests", __c => __c.AccessDenialTests), __r => F.CritField(F.Of(__r.TargetPolicy), F.Of(this.AccessPolicyId)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Principal { get; set; }
        public string? TargetTable { get; set; }

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

        private RulebookTable _rulebookTable;

        [ForeignKey("TargetTable")]
        public virtual RulebookTable RulebookTable
        {
            get
            {
                if (_rulebookTable == null && !string.IsNullOrEmpty(TargetTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTable - no database context is set. TargetTable: " + TargetTable + ".");
                        }
                        return null;
                    }
                    _rulebookTable = base.SoAContext.RulebookTables.Find(TargetTable);
                    if (_rulebookTable != null)
                    {
                        base.SoAContext.Attach(_rulebookTable);
                    }
                }
                return _rulebookTable;
            }
            set
            {
                if (_rulebookTable != value)
                {
                    _rulebookTable = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTable != null)
                    {
                        TargetTable = _rulebookTable.RulebookTableId;
                    }
                }
            }
        }

        private ObservableCollection<AccessDenialTest> _accessDenialTests;

        [InverseProperty("AccessPolicy")]
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
                            throw new InvalidOperationException("Cannot access AccessDenialTests - no database context is set. AccessPolicyId: " + this.AccessPolicyId + ".");
                        }
                        _accessDenialTests = new ObservableCollection<AccessDenialTest>();
                    }
                    else
                    {
                        var items = base.SoAContext.AccessDenialTests.Where(x => x.TargetPolicy == this.AccessPolicyId).ToList<AccessDenialTest>();
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
                    item.TargetPolicy = this.AccessPolicyId;
                }
            }
        }

        private ObservableCollection<AppAction> _appActions;

        [InverseProperty("AccessPolicy")]
        public virtual ObservableCollection<AppAction> AppActions
        {
            get
            {
                if (_appActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppActions - no database context is set. AccessPolicyId: " + this.AccessPolicyId + ".");
                        }
                        _appActions = new ObservableCollection<AppAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppActions.Where(x => x.Policy == this.AccessPolicyId).ToList<AppAction>();
                        _appActions = new ObservableCollection<AppAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appActions.CollectionChanged += AppActions_CollectionChanged;
                }
                return _appActions;
            }
            private set
            {
                if (_appActions != null)
                {
                    _appActions.CollectionChanged -= AppActions_CollectionChanged;
                }
                _appActions = value;
                if (_appActions != null)
                {
                    _appActions.CollectionChanged += AppActions_CollectionChanged;
                }
            }
        }

        private void AppActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppAction>())
                {
                    item.Policy = this.AccessPolicyId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AccessPrincipal;
            _ = this.RulebookTable;
            _ = this.AccessDenialTests;
            _ = this.AppActions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
