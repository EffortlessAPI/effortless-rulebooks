
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
    [Table("ProviderEngagements")]
    public class ProviderEngagementBase : SoAEntityBase
    {
        [Key]
        public string ProviderEngagementId { get; set; }

        // Formula Name (rulebook: ={{ClientOrganization}} & " <- " & {{Provider}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ClientOrganization)), F.S(" <- "), F.Text(F.Of(this.Provider))))); set { }
        }

        public DateTimeOffset? StartedAt { get; set; }
        public int? TermMonths { get; set; }
        public string? Status { get; set; }
        public string? DocumentationOwnership { get; set; }
        public string? KnowledgeDutyTerms { get; set; }
        public bool? ProviderTreatsKnowHowAsDifferentiator { get; set; }
        public bool? HasKnowledgeAccessClause { get; set; }
        public string? KnowledgeReturnPlan { get; set; }
        // Formula IsActive (rulebook: ={{Status}} = "Active")
        [NotMapped]
        public bool? IsActive
        {
            get => F.AsBool(F.Memo(this, "IsActive", () => F.Eq(F.Nullif(F.Of(this.Status)), F.S("Active")))); set { }
        }

        // Formula ReliedDependencyCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{ProviderHoldingKnowledge}}, {{Provider}}, KnowledgeAuditItems!{{ClientOrganization}}, {{ClientOrganization}}, KnowledgeAuditItems!{{IsKnowledgeDependency}}, TRUE))
        [NotMapped]
        public int? ReliedDependencyCount
        {
            get => F.AsInt(F.Memo(this, "ReliedDependencyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.ProviderHoldingKnowledge), F.Of(this.Provider)) && F.CritField(F.Of(__r.ClientOrganization), F.Of(this.ClientOrganization)) && F.CritLiteral(F.Of(__r.IsKnowledgeDependency), F.B(true))))))); set { }
        }

        // Formula IsUnplannedKnowledgeReturn (rulebook: =AND({{ReliedDependencyCount}} > 0, {{DocumentationOwnership}} = "ProviderIP", {{KnowledgeReturnPlan}} = ""))
        [NotMapped]
        public bool? IsUnplannedKnowledgeReturn
        {
            get => F.AsBool(F.Memo(this, "IsUnplannedKnowledgeReturn", () => F.And(F.Bool3(F.Cmp(F.Of(this.ReliedDependencyCount), ">", F.I(0))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DocumentationOwnership)), F.S("ProviderIP"))), F.Bool3(F.IsBlank(F.Of(this.KnowledgeReturnPlan)))))); set { }
        }

        // Formula IsKnowledgeAccessUnsecured (rulebook: =AND({{IsActive}}, {{ReliedDependencyCount}} > 0, {{HasKnowledgeAccessClause}} = FALSE))
        [NotMapped]
        public bool? IsKnowledgeAccessUnsecured
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeAccessUnsecured", () => F.And(F.Bool3(F.Of(this.IsActive)), F.Bool3(F.Cmp(F.Of(this.ReliedDependencyCount), ">", F.I(0))), F.Bool3(F.Eq(F.Nullif(F.Of(this.HasKnowledgeAccessClause)), F.B(false)))))); set { }
        }

        // Formula RequiredDeliverableCount (rulebook: =COUNTIFS(KnowledgeDeliverables!{{ProviderEngagement}}, {{ProviderEngagementId}}))
        [NotMapped]
        public int? RequiredDeliverableCount
        {
            get => F.AsInt(F.Memo(this, "RequiredDeliverableCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeDeliverable>(base.SoAContext, "KnowledgeDeliverables", __c => __c.KnowledgeDeliverables), __r => F.CritField(F.Of(__r.ProviderEngagement), F.Of(this.ProviderEngagementId))))))); set { }
        }

        // Formula ToClientRequiredCount (rulebook: =COUNTIFS(KnowledgeDeliverables!{{ProviderEngagement}}, {{ProviderEngagementId}}, KnowledgeDeliverables!{{Direction}}, "ToClient"))
        [NotMapped]
        public int? ToClientRequiredCount
        {
            get => F.AsInt(F.Memo(this, "ToClientRequiredCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeDeliverable>(base.SoAContext, "KnowledgeDeliverables", __c => __c.KnowledgeDeliverables), __r => F.CritField(F.Of(__r.ProviderEngagement), F.Of(this.ProviderEngagementId)) && F.CritLiteral(F.Of(__r.Direction), F.S("ToClient"))))))); set { }
        }

        // Formula ToClientDeliveredCount (rulebook: =COUNTIFS(KnowledgeDeliverables!{{ProviderEngagement}}, {{ProviderEngagementId}}, KnowledgeDeliverables!{{Direction}}, "ToClient", KnowledgeDeliverables!{{IsDelivered}}, TRUE))
        [NotMapped]
        public int? ToClientDeliveredCount
        {
            get => F.AsInt(F.Memo(this, "ToClientDeliveredCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeDeliverable>(base.SoAContext, "KnowledgeDeliverables", __c => __c.KnowledgeDeliverables), __r => F.CritField(F.Of(__r.ProviderEngagement), F.Of(this.ProviderEngagementId)) && F.CritLiteral(F.Of(__r.Direction), F.S("ToClient")) && F.CritLiteral(F.Of(__r.IsDelivered), F.B(true))))))); set { }
        }

        // Formula ToProviderDeliveredCount (rulebook: =COUNTIFS(KnowledgeDeliverables!{{ProviderEngagement}}, {{ProviderEngagementId}}, KnowledgeDeliverables!{{Direction}}, "ToProvider", KnowledgeDeliverables!{{IsDelivered}}, TRUE))
        [NotMapped]
        public int? ToProviderDeliveredCount
        {
            get => F.AsInt(F.Memo(this, "ToProviderDeliveredCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeDeliverable>(base.SoAContext, "KnowledgeDeliverables", __c => __c.KnowledgeDeliverables), __r => F.CritField(F.Of(__r.ProviderEngagement), F.Of(this.ProviderEngagementId)) && F.CritLiteral(F.Of(__r.Direction), F.S("ToProvider")) && F.CritLiteral(F.Of(__r.IsDelivered), F.B(true))))))); set { }
        }

        // Formula JointDeliverableCount (rulebook: =COUNTIFS(KnowledgeDeliverables!{{ProviderEngagement}}, {{ProviderEngagementId}}, KnowledgeDeliverables!{{Direction}}, "Joint"))
        [NotMapped]
        public int? JointDeliverableCount
        {
            get => F.AsInt(F.Memo(this, "JointDeliverableCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeDeliverable>(base.SoAContext, "KnowledgeDeliverables", __c => __c.KnowledgeDeliverables), __r => F.CritField(F.Of(__r.ProviderEngagement), F.Of(this.ProviderEngagementId)) && F.CritLiteral(F.Of(__r.Direction), F.S("Joint"))))))); set { }
        }

        // Formula LacksKnowledgeDeliverables (rulebook: =AND({{IsActive}}, {{RequiredDeliverableCount}} = 0))
        [NotMapped]
        public bool? LacksKnowledgeDeliverables
        {
            get => F.AsBool(F.Memo(this, "LacksKnowledgeDeliverables", () => F.And(F.Bool3(F.Of(this.IsActive)), F.Bool3(F.Eq(F.Of(this.RequiredDeliverableCount), F.I(0)))))); set { }
        }

        // Formula IsOneWayLearning (rulebook: =AND({{IsActive}}, {{RequiredDeliverableCount}} > 0, OR({{ToClientDeliveredCount}} = 0, {{ToProviderDeliveredCount}} = 0)))
        [NotMapped]
        public bool? IsOneWayLearning
        {
            get => F.AsBool(F.Memo(this, "IsOneWayLearning", () => F.And(F.Bool3(F.Of(this.IsActive)), F.Bool3(F.Cmp(F.Of(this.RequiredDeliverableCount), ">", F.I(0))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.ToClientDeliveredCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ToProviderDeliveredCount), F.I(0)))))))); set { }
        }

        // Formula IsShortTermWithoutJointKnowledge (rulebook: =AND({{IsActive}}, {{TermMonths}} < 36, {{JointDeliverableCount}} = 0))
        [NotMapped]
        public bool? IsShortTermWithoutJointKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsShortTermWithoutJointKnowledge", () => F.And(F.Bool3(F.Of(this.IsActive)), F.Bool3(F.Cmp(F.Nullif(F.Of(this.TermMonths)), "<", F.I(36))), F.Bool3(F.Eq(F.Of(this.JointDeliverableCount), F.I(0)))))); set { }
        }

        // Formula ObligesKnowledgeFlowBack (rulebook: =AND({{ToClientRequiredCount}} > 0, {{KnowledgeReturnPlan}} <> ""))
        [NotMapped]
        public bool? ObligesKnowledgeFlowBack
        {
            get => F.AsBool(F.Memo(this, "ObligesKnowledgeFlowBack", () => F.And(F.Bool3(F.Cmp(F.Of(this.ToClientRequiredCount), ">", F.I(0))), F.Bool3(F.IsNotBlank(F.Of(this.KnowledgeReturnPlan)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ClientOrganization { get; set; }
        public string? Provider { get; set; }
        public string? SourcingFunction { get; set; }

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

        private Organization _organizationRef;

        [ForeignKey("Provider")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Provider))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Provider: " + Provider + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Provider);
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
                        Provider = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private SourcingFunction _sourcingFunctionRef;

        [ForeignKey("SourcingFunction")]
        public virtual SourcingFunction SourcingFunctionRef
        {
            get
            {
                if (_sourcingFunctionRef == null && !string.IsNullOrEmpty(SourcingFunction))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourcingFunctionRef - no database context is set. SourcingFunction: " + SourcingFunction + ".");
                        }
                        return null;
                    }
                    _sourcingFunctionRef = base.SoAContext.SourcingFunctions.Find(SourcingFunction);
                    if (_sourcingFunctionRef != null)
                    {
                        base.SoAContext.Attach(_sourcingFunctionRef);
                    }
                }
                return _sourcingFunctionRef;
            }
            set
            {
                if (_sourcingFunctionRef != value)
                {
                    _sourcingFunctionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_sourcingFunctionRef != null)
                    {
                        SourcingFunction = _sourcingFunctionRef.SourcingFunctionId;
                    }
                }
            }
        }

        private ObservableCollection<KnowledgeDeliverable> _knowledgeDeliverables;

        [InverseProperty("ProviderEngagementRef")]
        public virtual ObservableCollection<KnowledgeDeliverable> KnowledgeDeliverables
        {
            get
            {
                if (_knowledgeDeliverables == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeDeliverables - no database context is set. ProviderEngagementId: " + this.ProviderEngagementId + ".");
                        }
                        _knowledgeDeliverables = new ObservableCollection<KnowledgeDeliverable>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeDeliverables.Where(x => x.ProviderEngagement == this.ProviderEngagementId).ToList<KnowledgeDeliverable>();
                        _knowledgeDeliverables = new ObservableCollection<KnowledgeDeliverable>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeDeliverables.CollectionChanged += KnowledgeDeliverables_CollectionChanged;
                }
                return _knowledgeDeliverables;
            }
            private set
            {
                if (_knowledgeDeliverables != null)
                {
                    _knowledgeDeliverables.CollectionChanged -= KnowledgeDeliverables_CollectionChanged;
                }
                _knowledgeDeliverables = value;
                if (_knowledgeDeliverables != null)
                {
                    _knowledgeDeliverables.CollectionChanged += KnowledgeDeliverables_CollectionChanged;
                }
            }
        }

        private void KnowledgeDeliverables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeDeliverable>())
                {
                    item.ProviderEngagement = this.ProviderEngagementId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Organization;
            _ = this.OrganizationRef;
            _ = this.SourcingFunctionRef;
            _ = this.KnowledgeDeliverables;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
