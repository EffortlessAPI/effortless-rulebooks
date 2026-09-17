
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
    [Table("SourcingFunctions")]
    public class SourcingFunctionBase : SoAEntityBase
    {
        [Key]
        public string SourcingFunctionId { get; set; }

        // Formula Name (rulebook: ={{ClientOrganization}} & ": " & {{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ClientOrganization)), F.S(": "), F.Text(F.Of(this.Label))))); set { }
        }

        public string? Label { get; set; }
        public string? SourcingClass { get; set; }
        public string? WorkNature { get; set; }
        public bool? IsStrategicallyVital { get; set; }
        public bool? DeliversOwnProduct { get; set; }
        public DateTimeOffset? SourcedSince { get; set; }
        public string? SourcingRationale { get; set; }
        // Formula IsOutsourced (rulebook: ={{ExecutingOrganization}} <> {{ClientOrganization}})
        [NotMapped]
        public bool? IsOutsourced
        {
            get => F.AsBool(F.Memo(this, "IsOutsourced", () => F.Ne(F.Nullif(F.Of(this.ExecutingOrganization)), F.Nullif(F.Of(this.ClientOrganization))))); set { }
        }

        // Formula IsBusinessProcessOutsourcing (rulebook: =AND({{IsOutsourced}}, {{WorkNature}} = "Transactional"))
        [NotMapped]
        public bool? IsBusinessProcessOutsourcing
        {
            get => F.AsBool(F.Memo(this, "IsBusinessProcessOutsourcing", () => F.And(F.Bool3(F.Of(this.IsOutsourced)), F.Bool3(F.Eq(F.Nullif(F.Of(this.WorkNature)), F.S("Transactional")))))); set { }
        }

        // Formula IsKnowledgeProcessOutsourcing (rulebook: =AND({{IsOutsourced}}, {{WorkNature}} = "ExpertiseHeavy"))
        [NotMapped]
        public bool? IsKnowledgeProcessOutsourcing
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeProcessOutsourcing", () => F.And(F.Bool3(F.Of(this.IsOutsourced)), F.Bool3(F.Eq(F.Nullif(F.Of(this.WorkNature)), F.S("ExpertiseHeavy")))))); set { }
        }

        // Formula IsVitalExpertiseClassedNonCore (rulebook: =AND({{SourcingClass}} = "SentOutExecution", {{IsStrategicallyVital}}, {{WorkNature}} = "ExpertiseHeavy"))
        [NotMapped]
        public bool? IsVitalExpertiseClassedNonCore
        {
            get => F.AsBool(F.Memo(this, "IsVitalExpertiseClassedNonCore", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.SourcingClass)), F.S("SentOutExecution"))), F.IsTrueV(F.Of(this.IsStrategicallyVital)), F.Bool3(F.Eq(F.Nullif(F.Of(this.WorkNature)), F.S("ExpertiseHeavy")))))); set { }
        }

        // Formula IsWhatHowSplit (rulebook: ={{SpecificationHolder}} <> {{MethodHolder}})
        [NotMapped]
        public bool? IsWhatHowSplit
        {
            get => F.AsBool(F.Memo(this, "IsWhatHowSplit", () => F.Ne(F.Nullif(F.Of(this.SpecificationHolder)), F.Nullif(F.Of(this.MethodHolder))))); set { }
        }

        // Formula IsMethodKnowledgeHeldOutside (rulebook: ={{MethodHolder}} <> {{ClientOrganization}})
        [NotMapped]
        public bool? IsMethodKnowledgeHeldOutside
        {
            get => F.AsBool(F.Memo(this, "IsMethodKnowledgeHeldOutside", () => F.Ne(F.Nullif(F.Of(this.MethodHolder)), F.Nullif(F.Of(this.ClientOrganization))))); set { }
        }

        // Formula ClaimsHowWithoutDoing (rulebook: =AND({{MethodHolder}} = {{ClientOrganization}}, {{IsOutsourced}}))
        [NotMapped]
        public bool? ClaimsHowWithoutDoing
        {
            get => F.AsBool(F.Memo(this, "ClaimsHowWithoutDoing", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.MethodHolder)), F.Nullif(F.Of(this.ClientOrganization)))), F.Bool3(F.Of(this.IsOutsourced))))); set { }
        }

        // Formula IsVitalExpertiseProcess (rulebook: =AND({{IsStrategicallyVital}}, {{WorkNature}} = "ExpertiseHeavy"))
        [NotMapped]
        public bool? IsVitalExpertiseProcess
        {
            get => F.AsBool(F.Memo(this, "IsVitalExpertiseProcess", () => F.And(F.IsTrueV(F.Of(this.IsStrategicallyVital)), F.Bool3(F.Eq(F.Nullif(F.Of(this.WorkNature)), F.S("ExpertiseHeavy")))))); set { }
        }

        // Formula IsOutsourcedVitalExpertiseProcess (rulebook: =AND({{IsVitalExpertiseProcess}}, {{IsOutsourced}}))
        [NotMapped]
        public bool? IsOutsourcedVitalExpertiseProcess
        {
            get => F.AsBool(F.Memo(this, "IsOutsourcedVitalExpertiseProcess", () => F.And(F.Bool3(F.Of(this.IsVitalExpertiseProcess)), F.Bool3(F.Of(this.IsOutsourced))))); set { }
        }

        // Formula AuditItemCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{SourcingFunction}}, {{SourcingFunctionId}}))
        [NotMapped]
        public int? AuditItemCount
        {
            get => F.AsInt(F.Memo(this, "AuditItemCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId))))))); set { }
        }

        // Formula DependencyCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{SourcingFunction}}, {{SourcingFunctionId}}, KnowledgeAuditItems!{{IsKnowledgeDependency}}, TRUE))
        [NotMapped]
        public int? DependencyCount
        {
            get => F.AsInt(F.Memo(this, "DependencyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId)) && F.CritLiteral(F.Of(__r.IsKnowledgeDependency), F.B(true))))))); set { }
        }

        // Formula CoverageGapCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{SourcingFunction}}, {{SourcingFunctionId}}, KnowledgeAuditItems!{{IsCoverageGap}}, TRUE))
        [NotMapped]
        public int? CoverageGapCount
        {
            get => F.AsInt(F.Memo(this, "CoverageGapCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId)) && F.CritLiteral(F.Of(__r.IsCoverageGap), F.B(true))))))); set { }
        }

        // Formula SpecificationAuditCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{SourcingFunction}}, {{SourcingFunctionId}}, KnowledgeAuditItems!{{KnowledgeKind}}, "Specification"))
        [NotMapped]
        public int? SpecificationAuditCount
        {
            get => F.AsInt(F.Memo(this, "SpecificationAuditCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId)) && F.CritLiteral(F.Of(__r.KnowledgeKind), F.S("Specification"))))))); set { }
        }

        // Formula SpecificationShortfallCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{SourcingFunction}}, {{SourcingFunctionId}}, KnowledgeAuditItems!{{KnowledgeKind}}, "Specification", KnowledgeAuditItems!{{HasInternalShortfall}}, TRUE))
        [NotMapped]
        public int? SpecificationShortfallCount
        {
            get => F.AsInt(F.Memo(this, "SpecificationShortfallCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId)) && F.CritLiteral(F.Of(__r.KnowledgeKind), F.S("Specification")) && F.CritLiteral(F.Of(__r.HasInternalShortfall), F.B(true))))))); set { }
        }

        // Formula MethodShortfallCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{SourcingFunction}}, {{SourcingFunctionId}}, KnowledgeAuditItems!{{KnowledgeKind}}, "Method", KnowledgeAuditItems!{{HasInternalShortfall}}, TRUE))
        [NotMapped]
        public int? MethodShortfallCount
        {
            get => F.AsInt(F.Memo(this, "MethodShortfallCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId)) && F.CritLiteral(F.Of(__r.KnowledgeKind), F.S("Method")) && F.CritLiteral(F.Of(__r.HasInternalShortfall), F.B(true))))))); set { }
        }

        // Formula ProviderIpDocumentationCount (rulebook: =COUNTIFS(ProviderEngagements!{{SourcingFunction}}, {{SourcingFunctionId}}, ProviderEngagements!{{DocumentationOwnership}}, "ProviderIP"))
        [NotMapped]
        public int? ProviderIpDocumentationCount
        {
            get => F.AsInt(F.Memo(this, "ProviderIpDocumentationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProviderEngagement>(base.SoAContext, "ProviderEngagements", __c => __c.ProviderEngagements), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId)) && F.CritLiteral(F.Of(__r.DocumentationOwnership), F.S("ProviderIP"))))))); set { }
        }

        // Formula DesignsWhatItCannotBuild (rulebook: =AND({{DeliversOwnProduct}}, {{IsOutsourced}}, {{ProviderIpDocumentationCount}} > 0, {{SpecificationAuditCount}} > 0, {{SpecificationShortfallCount}} = 0, {{MethodShortfallCount}} > 0))
        [NotMapped]
        public bool? DesignsWhatItCannotBuild
        {
            get => F.AsBool(F.Memo(this, "DesignsWhatItCannotBuild", () => F.And(F.IsTrueV(F.Of(this.DeliversOwnProduct)), F.Bool3(F.Of(this.IsOutsourced)), F.Bool3(F.Cmp(F.Of(this.ProviderIpDocumentationCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.SpecificationAuditCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.SpecificationShortfallCount), F.I(0))), F.Bool3(F.Cmp(F.Of(this.MethodShortfallCount), ">", F.I(0)))))); set { }
        }

        // Formula IsUnauditedFunction (rulebook: =AND(OR({{IsOutsourced}}, {{IsStrategicallyVital}}), {{AuditItemCount}} = 0))
        [NotMapped]
        public bool? IsUnauditedFunction
        {
            get => F.AsBool(F.Memo(this, "IsUnauditedFunction", () => F.And(F.Bool3(F.Or(F.Bool3(F.Of(this.IsOutsourced)), F.IsTrueV(F.Of(this.IsStrategicallyVital)))), F.Bool3(F.Eq(F.Of(this.AuditItemCount), F.I(0)))))); set { }
        }

        // Formula CaptureInitiativeCount (rulebook: =COUNTIFS(KnowledgeCaptureInitiatives!{{SourcingFunction}}, {{SourcingFunctionId}}))
        [NotMapped]
        public int? CaptureInitiativeCount
        {
            get => F.AsInt(F.Memo(this, "CaptureInitiativeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeCaptureInitiatif>(base.SoAContext, "KnowledgeCaptureInitiatives", __c => __c.KnowledgeCaptureInitiatives), __r => F.CritField(F.Of(__r.SourcingFunction), F.Of(this.SourcingFunctionId))))))); set { }
        }

        // Formula IsUncapturedPriorityProcess (rulebook: =AND({{IsVitalExpertiseProcess}}, OR({{IsOutsourced}}, {{CoverageGapCount}} > 0), {{CaptureInitiativeCount}} = 0))
        [NotMapped]
        public bool? IsUncapturedPriorityProcess
        {
            get => F.AsBool(F.Memo(this, "IsUncapturedPriorityProcess", () => F.And(F.Bool3(F.Of(this.IsVitalExpertiseProcess)), F.Bool3(F.Or(F.Bool3(F.Of(this.IsOutsourced)), F.Bool3(F.Cmp(F.Of(this.CoverageGapCount), ">", F.I(0))))), F.Bool3(F.Eq(F.Of(this.CaptureInitiativeCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ClientOrganization { get; set; }
        public string? Procedure { get; set; }
        public string? ExecutingOrganization { get; set; }
        public string? SpecificationHolder { get; set; }
        public string? MethodHolder { get; set; }

        private Organization _organization;

        [ForeignKey("ClientOrganization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(ClientOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. ClientOrganization: " + ClientOrganization + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(ClientOrganization);
                    if (_organization != null)
                    {
                        base.SoAContext.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organization != null)
                    {
                        ClientOrganization = _organization.OrganizationId;
                    }
                }
            }
        }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }

        private Organization _organizationRef;

        [ForeignKey("ExecutingOrganization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(ExecutingOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. ExecutingOrganization: " + ExecutingOrganization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(ExecutingOrganization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        ExecutingOrganization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private Organization _organizationRefRef;

        [ForeignKey("SpecificationHolder")]
        public virtual Organization OrganizationRefRef
        {
            get
            {
                if (_organizationRefRef == null && !string.IsNullOrEmpty(SpecificationHolder))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRefRef - no database context is set. SpecificationHolder: " + SpecificationHolder + ".");
                        }
                        return null;
                    }
                    _organizationRefRef = base.SoAContext.Organizations.Find(SpecificationHolder);
                    if (_organizationRefRef != null)
                    {
                        base.SoAContext.Attach(_organizationRefRef);
                    }
                }
                return _organizationRefRef;
            }
            set
            {
                if (_organizationRefRef != value)
                {
                    _organizationRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRefRef != null)
                    {
                        SpecificationHolder = _organizationRefRef.OrganizationId;
                    }
                }
            }
        }

        private Organization _organizationRefRefRef;

        [ForeignKey("MethodHolder")]
        public virtual Organization OrganizationRefRefRef
        {
            get
            {
                if (_organizationRefRefRef == null && !string.IsNullOrEmpty(MethodHolder))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRefRefRef - no database context is set. MethodHolder: " + MethodHolder + ".");
                        }
                        return null;
                    }
                    _organizationRefRefRef = base.SoAContext.Organizations.Find(MethodHolder);
                    if (_organizationRefRefRef != null)
                    {
                        base.SoAContext.Attach(_organizationRefRefRef);
                    }
                }
                return _organizationRefRefRef;
            }
            set
            {
                if (_organizationRefRefRef != value)
                {
                    _organizationRefRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRefRefRef != null)
                    {
                        MethodHolder = _organizationRefRefRef.OrganizationId;
                    }
                }
            }
        }

        private ObservableCollection<KnowledgeAuditItem> _knowledgeAuditItems;

        [InverseProperty("SourcingFunctionRef")]
        public virtual ObservableCollection<KnowledgeAuditItem> KnowledgeAuditItems
        {
            get
            {
                if (_knowledgeAuditItems == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeAuditItems - no database context is set. SourcingFunctionId: " + this.SourcingFunctionId + ".");
                        }
                        _knowledgeAuditItems = new ObservableCollection<KnowledgeAuditItem>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeAuditItems.Where(x => x.SourcingFunction == this.SourcingFunctionId).ToList<KnowledgeAuditItem>();
                        _knowledgeAuditItems = new ObservableCollection<KnowledgeAuditItem>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeAuditItems.CollectionChanged += KnowledgeAuditItems_CollectionChanged;
                }
                return _knowledgeAuditItems;
            }
            private set
            {
                if (_knowledgeAuditItems != null)
                {
                    _knowledgeAuditItems.CollectionChanged -= KnowledgeAuditItems_CollectionChanged;
                }
                _knowledgeAuditItems = value;
                if (_knowledgeAuditItems != null)
                {
                    _knowledgeAuditItems.CollectionChanged += KnowledgeAuditItems_CollectionChanged;
                }
            }
        }

        private void KnowledgeAuditItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeAuditItem>())
                {
                    item.SourcingFunction = this.SourcingFunctionId;
                }
            }
        }

        private ObservableCollection<KnowledgeCaptureInitiatif> _knowledgeCaptureInitiatives;

        [InverseProperty("SourcingFunctionRef")]
        public virtual ObservableCollection<KnowledgeCaptureInitiatif> KnowledgeCaptureInitiatives
        {
            get
            {
                if (_knowledgeCaptureInitiatives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeCaptureInitiatives - no database context is set. SourcingFunctionId: " + this.SourcingFunctionId + ".");
                        }
                        _knowledgeCaptureInitiatives = new ObservableCollection<KnowledgeCaptureInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeCaptureInitiatives.Where(x => x.SourcingFunction == this.SourcingFunctionId).ToList<KnowledgeCaptureInitiatif>();
                        _knowledgeCaptureInitiatives = new ObservableCollection<KnowledgeCaptureInitiatif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeCaptureInitiatives.CollectionChanged += KnowledgeCaptureInitiatives_CollectionChanged;
                }
                return _knowledgeCaptureInitiatives;
            }
            private set
            {
                if (_knowledgeCaptureInitiatives != null)
                {
                    _knowledgeCaptureInitiatives.CollectionChanged -= KnowledgeCaptureInitiatives_CollectionChanged;
                }
                _knowledgeCaptureInitiatives = value;
                if (_knowledgeCaptureInitiatives != null)
                {
                    _knowledgeCaptureInitiatives.CollectionChanged += KnowledgeCaptureInitiatives_CollectionChanged;
                }
            }
        }

        private void KnowledgeCaptureInitiatives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeCaptureInitiatif>())
                {
                    item.SourcingFunction = this.SourcingFunctionId;
                }
            }
        }

        private ObservableCollection<ProviderEngagement> _providerEngagements;

        [InverseProperty("SourcingFunctionRef")]
        public virtual ObservableCollection<ProviderEngagement> ProviderEngagements
        {
            get
            {
                if (_providerEngagements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProviderEngagements - no database context is set. SourcingFunctionId: " + this.SourcingFunctionId + ".");
                        }
                        _providerEngagements = new ObservableCollection<ProviderEngagement>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProviderEngagements.Where(x => x.SourcingFunction == this.SourcingFunctionId).ToList<ProviderEngagement>();
                        _providerEngagements = new ObservableCollection<ProviderEngagement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _providerEngagements.CollectionChanged += ProviderEngagements_CollectionChanged;
                }
                return _providerEngagements;
            }
            private set
            {
                if (_providerEngagements != null)
                {
                    _providerEngagements.CollectionChanged -= ProviderEngagements_CollectionChanged;
                }
                _providerEngagements = value;
                if (_providerEngagements != null)
                {
                    _providerEngagements.CollectionChanged += ProviderEngagements_CollectionChanged;
                }
            }
        }

        private void ProviderEngagements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProviderEngagement>())
                {
                    item.SourcingFunction = this.SourcingFunctionId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Organization;
            _ = this.ProcedureRef;
            _ = this.OrganizationRef;
            _ = this.OrganizationRefRef;
            _ = this.OrganizationRefRefRef;
            _ = this.KnowledgeAuditItems;
            _ = this.KnowledgeCaptureInitiatives;
            _ = this.ProviderEngagements;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
