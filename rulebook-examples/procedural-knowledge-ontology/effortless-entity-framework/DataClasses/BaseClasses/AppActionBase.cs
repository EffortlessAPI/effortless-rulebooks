
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
    [Table("AppActions")]
    public class AppActionBase : SoAEntityBase
    {
        [Key]
        public string AppActionId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? RoutePath { get; set; }
        public string? Operation { get; set; }
        public int? StoryEpisode { get; set; }
        public string? Description { get; set; }
        // Formula PolicyCommand (rulebook: =INDEX(AccessPolicies!{{Command}}, MATCH({{Policy}}, AccessPolicies!{{AccessPolicyId}}, 0)))
        [NotMapped]
        public string? PolicyCommand
        {
            get => F.AsString(F.Memo(this, "PolicyCommand", () => F.Lookup<AccessPolicy>(this, "AccessPolicies", "AccessPolicyId", __c => __c.AccessPolicies, __r => F.Of(__r.AccessPolicyId), F.Of(this.Policy), __r => F.Of(__r.Command), () => F.Of(new AccessPolicy().Command)))); set { }
        }

        // Formula PolicyDenialTestCount (rulebook: =INDEX(AccessPolicies!{{DenialTestCount}}, MATCH({{Policy}}, AccessPolicies!{{AccessPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyDenialTestCount
        {
            get => F.AsInt(F.Memo(this, "PolicyDenialTestCount", () => F.Integer(F.Lookup<AccessPolicy>(this, "AccessPolicies", "AccessPolicyId", __c => __c.AccessPolicies, __r => F.Of(__r.AccessPolicyId), F.Of(this.Policy), __r => F.Of(__r.DenialTestCount), () => F.Of(new AccessPolicy().DenialTestCount))))); set { }
        }

        // Formula WatchedFieldIsWitness (rulebook: =INDEX(RulebookFields!{{IsWitness}}, MATCH({{WatchedField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? WatchedFieldIsWitness
        {
            get => F.AsBool(F.Memo(this, "WatchedFieldIsWitness", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.WatchedField), __r => F.Of(__r.IsWitness), () => F.Of(new RulebookField().IsWitness)))); set { }
        }

        // Formula InputFieldCount (rulebook: =COUNTIFS(AppActionFields!{{AppAction}}, {{AppActionId}}))
        [NotMapped]
        public int? InputFieldCount
        {
            get => F.AsInt(F.Memo(this, "InputFieldCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AppActionField>(base.SoAContext, "AppActionFields", __c => __c.AppActionFields), __r => F.CritField(F.Of(__r.AppAction), F.Of(this.AppActionId))))))); set { }
        }

        // Formula IsUnpermitted (rulebook: ={{Policy}} = "")
        [NotMapped]
        public bool? IsUnpermitted
        {
            get => F.AsBool(F.Memo(this, "IsUnpermitted", () => F.IsBlank(F.Of(this.Policy)))); set { }
        }

        // Formula PolicyCommandDisagrees (rulebook: =AND({{Policy}} <> "", {{Operation}} <> {{PolicyCommand}}))
        [NotMapped]
        public bool? PolicyCommandDisagrees
        {
            get => F.AsBool(F.Memo(this, "PolicyCommandDisagrees", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Policy))), F.Bool3(F.Ne(F.Nullif(F.Of(this.Operation)), F.Of(this.PolicyCommand)))))); set { }
        }

        // Formula IsUnprovenWrite (rulebook: =AND({{Policy}} <> "", {{PolicyDenialTestCount}} = 0))
        [NotMapped]
        public bool? IsUnprovenWrite
        {
            get => F.AsBool(F.Memo(this, "IsUnprovenWrite", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.Policy))), F.Bool3(F.Eq(F.Of(this.PolicyDenialTestCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? OwningRole { get; set; }
        public string? TargetTable { get; set; }
        public string? Policy { get; set; }
        public string? WatchedField { get; set; }

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

        private AccessPolicy _accessPolicy;

        [ForeignKey("Policy")]
        public virtual AccessPolicy AccessPolicy
        {
            get
            {
                if (_accessPolicy == null && !string.IsNullOrEmpty(Policy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccessPolicy - no database context is set. Policy: " + Policy + ".");
                        }
                        return null;
                    }
                    _accessPolicy = base.SoAContext.AccessPolicies.Find(Policy);
                    if (_accessPolicy != null)
                    {
                        base.SoAContext.Attach(_accessPolicy);
                    }
                }
                return _accessPolicy;
            }
            set
            {
                if (_accessPolicy != value)
                {
                    _accessPolicy = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_accessPolicy != null)
                    {
                        Policy = _accessPolicy.AccessPolicyId;
                    }
                }
            }
        }

        private RulebookField _rulebookField;

        [ForeignKey("WatchedField")]
        public virtual RulebookField RulebookField
        {
            get
            {
                if (_rulebookField == null && !string.IsNullOrEmpty(WatchedField))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookField - no database context is set. WatchedField: " + WatchedField + ".");
                        }
                        return null;
                    }
                    _rulebookField = base.SoAContext.RulebookFields.Find(WatchedField);
                    if (_rulebookField != null)
                    {
                        base.SoAContext.Attach(_rulebookField);
                    }
                }
                return _rulebookField;
            }
            set
            {
                if (_rulebookField != value)
                {
                    _rulebookField = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookField != null)
                    {
                        WatchedField = _rulebookField.RulebookFieldId;
                    }
                }
            }
        }

        private ObservableCollection<AppActionField> _appActionFields;

        [InverseProperty("AppActionRef")]
        public virtual ObservableCollection<AppActionField> AppActionFields
        {
            get
            {
                if (_appActionFields == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppActionFields - no database context is set. AppActionId: " + this.AppActionId + ".");
                        }
                        _appActionFields = new ObservableCollection<AppActionField>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppActionFields.Where(x => x.AppAction == this.AppActionId).ToList<AppActionField>();
                        _appActionFields = new ObservableCollection<AppActionField>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appActionFields.CollectionChanged += AppActionFields_CollectionChanged;
                }
                return _appActionFields;
            }
            private set
            {
                if (_appActionFields != null)
                {
                    _appActionFields.CollectionChanged -= AppActionFields_CollectionChanged;
                }
                _appActionFields = value;
                if (_appActionFields != null)
                {
                    _appActionFields.CollectionChanged += AppActionFields_CollectionChanged;
                }
            }
        }

        private void AppActionFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppActionField>())
                {
                    item.AppAction = this.AppActionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Role;
            _ = this.RulebookTable;
            _ = this.AccessPolicy;
            _ = this.RulebookField;
            _ = this.AppActionFields;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
