
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
    [Table("ModelCharters")]
    public class ModelCharterBase : SoAEntityBase
    {
        [Key]
        public string ModelCharterId { get; set; }

        // Formula Name (rulebook: ={{GovernedModel}} & " / " & {{StewardRole}} & " + " & {{AuthorityRole}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.GovernedModel)), F.S(" / "), F.Text(F.Of(this.StewardRole)), F.S(" + "), F.Text(F.Of(this.AuthorityRole))))); set { }
        }

        public string? StewardResponsibilities { get; set; }
        public string? AuthorityApprovalScope { get; set; }
        public string? CharterTemplate { get; set; }
        public string? LocalAdaptation { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula IsCurrent (rulebook: =AND({{ValidFrom}} <= {{AsOfInstant}}, OR({{ValidTo}} = "", {{ValidTo}} > {{AsOfInstant}})))
        [NotMapped]
        public bool? IsCurrent
        {
            get => F.AsBool(F.Memo(this, "IsCurrent", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidFrom)), "<=", F.Of(this.AsOfInstant))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ValidTo))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ValidTo)), ">", F.Of(this.AsOfInstant)))))))); set { }
        }

        // Formula StewardAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{StewardRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? StewardAgent
        {
            get => F.AsString(F.Memo(this, "StewardAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.StewardRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula AuthorityAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AuthorityRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AuthorityAgent
        {
            get => F.AsString(F.Memo(this, "AuthorityAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AuthorityRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula AuthorityOrganization (rulebook: =INDEX(Roles!{{Organization}}, MATCH({{AuthorityRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AuthorityOrganization
        {
            get => F.AsString(F.Memo(this, "AuthorityOrganization", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AuthorityRole), __r => F.Of(__r.Organization), () => F.Of(new Role().Organization)))); set { }
        }

        // Formula ModelKind (rulebook: =INDEX(GovernedModels!{{ModelKind}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelKind
        {
            get => F.AsString(F.Memo(this, "ModelKind", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.ModelKind), () => F.Of(new GovernedModel().ModelKind)))); set { }
        }

        // Formula ModelDomainOwner (rulebook: =INDEX(GovernedModels!{{DomainOwningOrganization}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelDomainOwner
        {
            get => F.AsString(F.Memo(this, "ModelDomainOwner", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.DomainOwningOrganization), () => F.Of(new GovernedModel().DomainOwningOrganization)))); set { }
        }

        // Formula ModelHeadcount (rulebook: =INDEX(GovernedModels!{{OrganizationHeadcount}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public int? ModelHeadcount
        {
            get => F.AsInt(F.Memo(this, "ModelHeadcount", () => F.Integer(F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.OrganizationHeadcount), () => F.Of(new GovernedModel().OrganizationHeadcount))))); set { }
        }

        // Formula ModelToolingOwnerRole (rulebook: =INDEX(GovernedModels!{{ToolingOwnerRole}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelToolingOwnerRole
        {
            get => F.AsString(F.Memo(this, "ModelToolingOwnerRole", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.ToolingOwnerRole), () => F.Of(new GovernedModel().ToolingOwnerRole)))); set { }
        }

        // Formula ModelFirstControlAdoptedAt (rulebook: =INDEX(GovernedModels!{{FirstControlAdoptedAt}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public DateTimeOffset? ModelFirstControlAdoptedAt
        {
            get => F.AsDateTime(F.Memo(this, "ModelFirstControlAdoptedAt", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.FirstControlAdoptedAt), () => F.Of(new GovernedModel().FirstControlAdoptedAt)))); set { }
        }

        // Formula IsStewardUnwritten (rulebook: =OR({{StewardRole}} = "", {{StewardResponsibilities}} = ""))
        [NotMapped]
        public bool? IsStewardUnwritten
        {
            get => F.AsBool(F.Memo(this, "IsStewardUnwritten", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.StewardRole))), F.Bool3(F.IsBlank(F.Of(this.StewardResponsibilities)))))); set { }
        }

        // Formula IsAuthorityScopeUnstated (rulebook: =OR({{AuthorityRole}} = "", {{AuthorityApprovalScope}} = ""))
        [NotMapped]
        public bool? IsAuthorityScopeUnstated
        {
            get => F.AsBool(F.Memo(this, "IsAuthorityScopeUnstated", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.AuthorityRole))), F.Bool3(F.IsBlank(F.Of(this.AuthorityApprovalScope)))))); set { }
        }

        // Formula ConflatesStewardAndAuthority (rulebook: =AND({{StewardRole}} <> "", {{StewardRole}} = {{AuthorityRole}}))
        [NotMapped]
        public bool? ConflatesStewardAndAuthority
        {
            get => F.AsBool(F.Memo(this, "ConflatesStewardAndAuthority", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.StewardRole))), F.Bool3(F.Eq(F.Nullif(F.Of(this.StewardRole)), F.Nullif(F.Of(this.AuthorityRole))))))); set { }
        }

        // Formula IsSanctionedDualHolding (rulebook: =AND({{StewardRole}} <> {{AuthorityRole}}, {{StewardAgent}} <> "", {{StewardAgent}} = {{AuthorityAgent}}, {{ModelHeadcount}} <= 50))
        [NotMapped]
        public bool? IsSanctionedDualHolding
        {
            get => F.AsBool(F.Memo(this, "IsSanctionedDualHolding", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.StewardRole)), F.Nullif(F.Of(this.AuthorityRole)))), F.Bool3(F.IsNotBlank(F.Of(this.StewardAgent))), F.Bool3(F.Eq(F.Of(this.StewardAgent), F.Of(this.AuthorityAgent))), F.Bool3(F.Cmp(F.Of(this.ModelHeadcount), "<=", F.I(50)))))); set { }
        }

        // Formula IsAuthorityOutsideDomainOwner (rulebook: =AND({{AuthorityOrganization}} <> "", {{ModelDomainOwner}} <> "", {{AuthorityOrganization}} <> {{ModelDomainOwner}}))
        [NotMapped]
        public bool? IsAuthorityOutsideDomainOwner
        {
            get => F.AsBool(F.Memo(this, "IsAuthorityOutsideDomainOwner", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AuthorityOrganization))), F.Bool3(F.IsNotBlank(F.Of(this.ModelDomainOwner))), F.Bool3(F.Ne(F.Of(this.AuthorityOrganization), F.Of(this.ModelDomainOwner)))))); set { }
        }

        // Formula ControlsPrecedeOwnership (rulebook: =AND({{SupersedesCharter}} = "", {{ModelFirstControlAdoptedAt}} <> "", {{ModelFirstControlAdoptedAt}} < {{ValidFrom}}))
        [NotMapped]
        public bool? ControlsPrecedeOwnership
        {
            get => F.AsBool(F.Memo(this, "ControlsPrecedeOwnership", () => F.And(F.Bool3(F.IsBlank(F.Of(this.SupersedesCharter))), F.Bool3(F.IsNotBlank(F.Of(this.ModelFirstControlAdoptedAt))), F.Bool3(F.Cmp(F.Of(this.ModelFirstControlAdoptedAt), "<", F.Nullif(F.Of(this.ValidFrom))))))); set { }
        }

        // Formula StewardActivityCount (rulebook: =COUNTIFS(StewardActivities!{{ModelCharter}}, {{ModelCharterId}}))
        [NotMapped]
        public int? StewardActivityCount
        {
            get => F.AsInt(F.Memo(this, "StewardActivityCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StewardActivity>(base.SoAContext, "StewardActivities", __c => __c.StewardActivities), __r => F.CritField(F.Of(__r.ModelCharter), F.Of(this.ModelCharterId))))))); set { }
        }

        // Formula LastDriftWatchAt (rulebook: =MAXIFS(StewardActivities!{{PerformedAt}}, StewardActivities!{{ModelCharter}}, {{ModelCharterId}}, StewardActivities!{{DutyKind}}, "DriftWatch"))
        [NotMapped]
        public DateTimeOffset? LastDriftWatchAt
        {
            get => F.AsDateTime(F.Memo(this, "LastDriftWatchAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<StewardActivity>(base.SoAContext, "StewardActivities", __c => __c.StewardActivities), __r => F.CritField(F.Of(__r.ModelCharter), F.Of(this.ModelCharterId)) && F.CritLiteral(F.Of(__r.DutyKind), F.S("DriftWatch")), __r => F.Of(__r.PerformedAt))))); set { }
        }

        // Formula DaysSinceDriftWatch (rulebook: =IF({{LastDriftWatchAt}} = "", DATETIME_DIFF({{AsOfInstant}}, {{ValidFrom}}, "days"), DATETIME_DIFF({{AsOfInstant}}, {{LastDriftWatchAt}}, "days")))
        [NotMapped]
        public int? DaysSinceDriftWatch
        {
            get => F.AsInt(F.Memo(this, "DaysSinceDriftWatch", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastDriftWatchAt)))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.ValidFrom), F.S("days")) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastDriftWatchAt), F.S("days")))))); set { }
        }

        // Formula IsDriftWatchLapsed (rulebook: =AND({{IsCurrent}}, {{DaysSinceDriftWatch}} > 90))
        [NotMapped]
        public bool? IsDriftWatchLapsed
        {
            get => F.AsBool(F.Memo(this, "IsDriftWatchLapsed", () => F.And(F.Bool3(F.Of(this.IsCurrent)), F.Bool3(F.Cmp(F.Of(this.DaysSinceDriftWatch), ">", F.I(90)))))); set { }
        }

        // Formula ProcedureDecisionCount (rulebook: =COUNTIFS(ChangeRequests!{{AuthorityRole}}, {{AuthorityRole}}, ChangeRequests!{{IsDecided}}, TRUE))
        [NotMapped]
        public int? ProcedureDecisionCount
        {
            get => F.AsInt(F.Memo(this, "ProcedureDecisionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeRequest>(base.SoAContext, "ChangeRequests", __c => __c.ChangeRequests), __r => F.CritField(F.Of(__r.AuthorityRole), F.Of(this.AuthorityRole)) && F.CritLiteral(F.Of(__r.IsDecided), F.B(true))))))); set { }
        }

        // Formula ModelReviewCount (rulebook: =COUNTIFS(ModelChangeRequests!{{GovernedModel}}, {{GovernedModel}}, ModelChangeRequests!{{HasAuthorityReview}}, TRUE))
        [NotMapped]
        public int? ModelReviewCount
        {
            get => F.AsInt(F.Memo(this, "ModelReviewCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeRequest>(base.SoAContext, "ModelChangeRequests", __c => __c.ModelChangeRequests), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModel)) && F.CritLiteral(F.Of(__r.HasAuthorityReview), F.B(true))))))); set { }
        }

        // Formula IsNamedButUnexercised (rulebook: =AND({{IsCurrent}}, OR({{StewardActivityCount}} = 0, ({{ProcedureDecisionCount}} + {{ModelReviewCount}}) = 0)))
        [NotMapped]
        public bool? IsNamedButUnexercised
        {
            get => F.AsBool(F.Memo(this, "IsNamedButUnexercised", () => F.And(F.Bool3(F.Of(this.IsCurrent)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.StewardActivityCount), F.I(0))), F.Bool3(F.Eq(F.Add(F.Of(this.ProcedureDecisionCount), F.Of(this.ModelReviewCount)), F.I(0)))))))); set { }
        }

        // Formula StewardIsOutsideTooling (rulebook: =AND({{ModelKind}} = "Ontology", {{ModelToolingOwnerRole}} <> "", {{StewardRole}} <> {{ModelToolingOwnerRole}}))
        [NotMapped]
        public bool? StewardIsOutsideTooling
        {
            get => F.AsBool(F.Memo(this, "StewardIsOutsideTooling", () => F.And(F.Bool3(F.Eq(F.Of(this.ModelKind), F.S("Ontology"))), F.Bool3(F.IsNotBlank(F.Of(this.ModelToolingOwnerRole))), F.Bool3(F.Ne(F.Nullif(F.Of(this.StewardRole)), F.Of(this.ModelToolingOwnerRole)))))); set { }
        }

        // Formula AdoptedTemplateWithoutAdaptation (rulebook: =AND({{CharterTemplate}} <> "", {{LocalAdaptation}} = ""))
        [NotMapped]
        public bool? AdoptedTemplateWithoutAdaptation
        {
            get => F.AsBool(F.Memo(this, "AdoptedTemplateWithoutAdaptation", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CharterTemplate))), F.Bool3(F.IsBlank(F.Of(this.LocalAdaptation)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? StewardRole { get; set; }
        public string? AuthorityRole { get; set; }
        public string? SupersedesCharter { get; set; }
        public string? EvaluationContext { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("StewardRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(StewardRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. StewardRole: " + StewardRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(StewardRole);
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
                        StewardRole = _role.RoleId;
                    }
                }
            }
        }

        private Role _roleRef;

        [ForeignKey("AuthorityRole")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(AuthorityRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. AuthorityRole: " + AuthorityRole + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(AuthorityRole);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        AuthorityRole = _roleRef.RoleId;
                    }
                }
            }
        }

        private ModelCharter _modelCharter;

        [ForeignKey("SupersedesCharter")]
        public virtual ModelCharter ModelCharter
        {
            get
            {
                if (_modelCharter == null && !string.IsNullOrEmpty(SupersedesCharter))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelCharter - no database context is set. SupersedesCharter: " + SupersedesCharter + ".");
                        }
                        return null;
                    }
                    _modelCharter = base.SoAContext.ModelCharters.Find(SupersedesCharter);
                    if (_modelCharter != null)
                    {
                        base.SoAContext.Attach(_modelCharter);
                    }
                }
                return _modelCharter;
            }
            set
            {
                if (_modelCharter != value)
                {
                    _modelCharter = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_modelCharter != null)
                    {
                        SupersedesCharter = _modelCharter.ModelCharterId;
                    }
                }
            }
        }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }

        private ObservableCollection<ModelCharter> _modelCharters;

        [InverseProperty("ModelCharter")]
        public virtual ObservableCollection<ModelCharter> ModelCharters
        {
            get
            {
                if (_modelCharters == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelCharters - no database context is set. ModelCharterId: " + this.ModelCharterId + ".");
                        }
                        _modelCharters = new ObservableCollection<ModelCharter>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelCharters.Where(x => x.SupersedesCharter == this.ModelCharterId).ToList<ModelCharter>();
                        _modelCharters = new ObservableCollection<ModelCharter>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelCharters.CollectionChanged += ModelCharters_CollectionChanged;
                }
                return _modelCharters;
            }
            private set
            {
                if (_modelCharters != null)
                {
                    _modelCharters.CollectionChanged -= ModelCharters_CollectionChanged;
                }
                _modelCharters = value;
                if (_modelCharters != null)
                {
                    _modelCharters.CollectionChanged += ModelCharters_CollectionChanged;
                }
            }
        }

        private void ModelCharters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelCharter>())
                {
                    item.SupersedesCharter = this.ModelCharterId;
                }
            }
        }

        private ObservableCollection<StewardActivity> _stewardActivities;

        [InverseProperty("ModelCharterRef")]
        public virtual ObservableCollection<StewardActivity> StewardActivities
        {
            get
            {
                if (_stewardActivities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StewardActivities - no database context is set. ModelCharterId: " + this.ModelCharterId + ".");
                        }
                        _stewardActivities = new ObservableCollection<StewardActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardActivities.Where(x => x.ModelCharter == this.ModelCharterId).ToList<StewardActivity>();
                        _stewardActivities = new ObservableCollection<StewardActivity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stewardActivities.CollectionChanged += StewardActivities_CollectionChanged;
                }
                return _stewardActivities;
            }
            private set
            {
                if (_stewardActivities != null)
                {
                    _stewardActivities.CollectionChanged -= StewardActivities_CollectionChanged;
                }
                _stewardActivities = value;
                if (_stewardActivities != null)
                {
                    _stewardActivities.CollectionChanged += StewardActivities_CollectionChanged;
                }
            }
        }

        private void StewardActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StewardActivity>())
                {
                    item.ModelCharter = this.ModelCharterId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.Role;
            _ = this.RoleRef;
            _ = this.ModelCharter;
            _ = this.EvaluationContextRef;
            _ = this.ModelCharters;
            _ = this.StewardActivities;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
