
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
    [Table("RulebookFields")]
    public class RulebookFieldBase : SoAEntityBase
    {
        [Key]
        public string RulebookFieldId { get; set; }

        // Formula Name (rulebook: ={{TargetTable}} & "." & {{FieldName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.TargetTable)), F.S("."), F.Text(F.Of(this.FieldName))))); set { }
        }

        public string? TargetTable { get; set; }
        public string? FieldName { get; set; }
        public string? FieldType { get; set; }
        public string? Datatype { get; set; }
        public string? Formula { get; set; }
        public string? RelatedTo { get; set; }
        // Formula IsDerived (rulebook: =OR({{FieldType}} = "calculated", {{FieldType}} = "lookup", {{FieldType}} = "aggregation"))
        [NotMapped]
        public bool? IsDerived
        {
            get => F.AsBool(F.Memo(this, "IsDerived", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.FieldType)), F.S("calculated"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.FieldType)), F.S("lookup"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.FieldType)), F.S("aggregation")))))); set { }
        }

        // Formula IsWitness (rulebook: ={{InventedForQuestion}} <> "")
        [NotMapped]
        public bool? IsWitness
        {
            get => F.AsBool(F.Memo(this, "IsWitness", () => F.IsNotBlank(F.Of(this.InventedForQuestion)))); set { }
        }

        // Formula DisagreeingSubstrateCount (rulebook: =COUNTIFS(FieldDisagreements!{{RulebookField}}, {{RulebookFieldId}}))
        [NotMapped]
        public decimal? DisagreeingSubstrateCount
        {
            get => F.AsDecimal(F.Memo(this, "DisagreeingSubstrateCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FieldDisagreement>(base.SoAContext, "FieldDisagreements", __c => __c.FieldDisagreements), __r => F.CritField(F.Of(__r.RulebookField), F.Of(this.RulebookFieldId)))))); set { }
        }

        // Formula IsSubstrateContested (rulebook: ={{DisagreeingSubstrateCount}} > 0)
        [NotMapped]
        public bool? IsSubstrateContested
        {
            get => F.AsBool(F.Memo(this, "IsSubstrateContested", () => F.Cmp(F.Of(this.DisagreeingSubstrateCount), ">", F.I(0)))); set { }
        }

        public int? MeasuredSubstantiveCount { get; set; }
        public int? MeasuredDistinctValueCount { get; set; }
        // Formula HasMeasuredData (rulebook: ={{MeasuredSubstantiveCount}} > 0)
        [NotMapped]
        public bool? HasMeasuredData
        {
            get => F.AsBool(F.Memo(this, "HasMeasuredData", () => F.Cmp(F.Nullif(F.Of(this.MeasuredSubstantiveCount)), ">", F.I(0)))); set { }
        }

        // Formula IsDiscriminating (rulebook: ={{MeasuredDistinctValueCount}} >= 2)
        [NotMapped]
        public bool? IsDiscriminating
        {
            get => F.AsBool(F.Memo(this, "IsDiscriminating", () => F.Cmp(F.Nullif(F.Of(this.MeasuredDistinctValueCount)), ">=", F.I(2)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? InventedForQuestion { get; set; }

        private RoleQuestion _roleQuestion;

        [ForeignKey("InventedForQuestion")]
        public virtual RoleQuestion RoleQuestion
        {
            get
            {
                if (_roleQuestion == null && !string.IsNullOrEmpty(InventedForQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. InventedForQuestion: " + InventedForQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestion = base.SoAContext.RoleQuestions.Find(InventedForQuestion);
                    if (_roleQuestion != null)
                    {
                        base.SoAContext.Attach(_roleQuestion);
                    }
                }
                return _roleQuestion;
            }
            set
            {
                if (_roleQuestion != value)
                {
                    _roleQuestion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleQuestion != null)
                    {
                        InventedForQuestion = _roleQuestion.RoleQuestionId;
                    }
                }
            }
        }

        private ObservableCollection<FieldGrant> _fieldGrants;

        [InverseProperty("RulebookField")]
        public virtual ObservableCollection<FieldGrant> FieldGrants
        {
            get
            {
                if (_fieldGrants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldGrants - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _fieldGrants = new ObservableCollection<FieldGrant>();
                    }
                    else
                    {
                        var items = base.SoAContext.FieldGrants.Where(x => x.TargetField == this.RulebookFieldId).ToList<FieldGrant>();
                        _fieldGrants = new ObservableCollection<FieldGrant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fieldGrants.CollectionChanged += FieldGrants_CollectionChanged;
                }
                return _fieldGrants;
            }
            private set
            {
                if (_fieldGrants != null)
                {
                    _fieldGrants.CollectionChanged -= FieldGrants_CollectionChanged;
                }
                _fieldGrants = value;
                if (_fieldGrants != null)
                {
                    _fieldGrants.CollectionChanged += FieldGrants_CollectionChanged;
                }
            }
        }

        private void FieldGrants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FieldGrant>())
                {
                    item.TargetField = this.RulebookFieldId;
                }
            }
        }

        private ObservableCollection<FieldDisagreement> _fieldDisagreements;

        [InverseProperty("RulebookFieldRef")]
        public virtual ObservableCollection<FieldDisagreement> FieldDisagreements
        {
            get
            {
                if (_fieldDisagreements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FieldDisagreements - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _fieldDisagreements = new ObservableCollection<FieldDisagreement>();
                    }
                    else
                    {
                        var items = base.SoAContext.FieldDisagreements.Where(x => x.RulebookField == this.RulebookFieldId).ToList<FieldDisagreement>();
                        _fieldDisagreements = new ObservableCollection<FieldDisagreement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fieldDisagreements.CollectionChanged += FieldDisagreements_CollectionChanged;
                }
                return _fieldDisagreements;
            }
            private set
            {
                if (_fieldDisagreements != null)
                {
                    _fieldDisagreements.CollectionChanged -= FieldDisagreements_CollectionChanged;
                }
                _fieldDisagreements = value;
                if (_fieldDisagreements != null)
                {
                    _fieldDisagreements.CollectionChanged += FieldDisagreements_CollectionChanged;
                }
            }
        }

        private void FieldDisagreements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FieldDisagreement>())
                {
                    item.RulebookField = this.RulebookFieldId;
                }
            }
        }

        private ObservableCollection<ClaimEvidence> _claimEvidence;

        [InverseProperty("RulebookFieldRef")]
        public virtual ObservableCollection<ClaimEvidence> ClaimEvidence
        {
            get
            {
                if (_claimEvidence == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClaimEvidence - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _claimEvidence = new ObservableCollection<ClaimEvidence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ClaimEvidence.Where(x => x.RulebookField == this.RulebookFieldId).ToList<ClaimEvidence>();
                        _claimEvidence = new ObservableCollection<ClaimEvidence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
                return _claimEvidence;
            }
            private set
            {
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged -= ClaimEvidence_CollectionChanged;
                }
                _claimEvidence = value;
                if (_claimEvidence != null)
                {
                    _claimEvidence.CollectionChanged += ClaimEvidence_CollectionChanged;
                }
            }
        }

        private void ClaimEvidence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ClaimEvidence>())
                {
                    item.RulebookField = this.RulebookFieldId;
                }
            }
        }

        private ObservableCollection<ExpectedInferenceCheck> _expectedInferenceChecks;

        [InverseProperty("RulebookField")]
        public virtual ObservableCollection<ExpectedInferenceCheck> ExpectedInferenceChecks
        {
            get
            {
                if (_expectedInferenceChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpectedInferenceChecks - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _expectedInferenceChecks = new ObservableCollection<ExpectedInferenceCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpectedInferenceChecks.Where(x => x.ExpectedField == this.RulebookFieldId).ToList<ExpectedInferenceCheck>();
                        _expectedInferenceChecks = new ObservableCollection<ExpectedInferenceCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expectedInferenceChecks.CollectionChanged += ExpectedInferenceChecks_CollectionChanged;
                }
                return _expectedInferenceChecks;
            }
            private set
            {
                if (_expectedInferenceChecks != null)
                {
                    _expectedInferenceChecks.CollectionChanged -= ExpectedInferenceChecks_CollectionChanged;
                }
                _expectedInferenceChecks = value;
                if (_expectedInferenceChecks != null)
                {
                    _expectedInferenceChecks.CollectionChanged += ExpectedInferenceChecks_CollectionChanged;
                }
            }
        }

        private void ExpectedInferenceChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpectedInferenceCheck>())
                {
                    item.ExpectedField = this.RulebookFieldId;
                }
            }
        }

        private ObservableCollection<AppAction> _appActions;

        [InverseProperty("RulebookField")]
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
                            throw new InvalidOperationException("Cannot access AppActions - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _appActions = new ObservableCollection<AppAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppActions.Where(x => x.WatchedField == this.RulebookFieldId).ToList<AppAction>();
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
                    item.WatchedField = this.RulebookFieldId;
                }
            }
        }

        private ObservableCollection<AppActionField> _appActionFields;

        [InverseProperty("RulebookField")]
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
                            throw new InvalidOperationException("Cannot access AppActionFields - no database context is set. RulebookFieldId: " + this.RulebookFieldId + ".");
                        }
                        _appActionFields = new ObservableCollection<AppActionField>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppActionFields.Where(x => x.TargetField == this.RulebookFieldId).ToList<AppActionField>();
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
                    item.TargetField = this.RulebookFieldId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleQuestion;
            _ = this.FieldGrants;
            _ = this.FieldDisagreements;
            _ = this.ClaimEvidence;
            _ = this.ExpectedInferenceChecks;
            _ = this.AppActions;
            _ = this.AppActionFields;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
