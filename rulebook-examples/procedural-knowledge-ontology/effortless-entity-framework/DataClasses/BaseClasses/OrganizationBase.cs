
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
    [Table("Organizations")]
    public class OrganizationBase : SoAEntityBase
    {
        [Key]
        public string OrganizationId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.DisplayName))); set { }
        }

        public string? DisplayName { get; set; }
        public string? OrganizationType { get; set; }
        public string? ExternalIdentifier { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula FailedAiInitiativeCount (rulebook: =COUNTIFS(AiAdoptionInitiatives!{{Organization}}, {{OrganizationId}}, AiAdoptionInitiatives!{{Outcome}}, "Failed"))
        [NotMapped]
        public int? FailedAiInitiativeCount
        {
            get => F.AsInt(F.Memo(this, "FailedAiInitiativeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiAdoptionInitiatif>(base.SoAContext, "AiAdoptionInitiatives", __c => __c.AiAdoptionInitiatives), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.Outcome), F.S("Failed"))))))); set { }
        }

        // Formula OwnedProcedureCount (rulebook: =COUNTIFS(Procedures!{{OwnerOrganization}}, {{OrganizationId}}))
        [NotMapped]
        public int? OwnedProcedureCount
        {
            get => F.AsInt(F.Memo(this, "OwnedProcedureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Procedure>(base.SoAContext, "Procedures", __c => __c.Procedures), __r => F.CritField(F.Of(__r.OwnerOrganization), F.Of(this.OrganizationId))))))); set { }
        }

        // Formula AiFailsForLackOfCapturedKnowledge (rulebook: =AND({{FailedAiInitiativeCount}} > 0, {{OwnedProcedureCount}} = 0))
        [NotMapped]
        public bool? AiFailsForLackOfCapturedKnowledge
        {
            get => F.AsBool(F.Memo(this, "AiFailsForLackOfCapturedKnowledge", () => F.And(F.Bool3(F.Cmp(F.Of(this.FailedAiInitiativeCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.OwnedProcedureCount), F.I(0)))))); set { }
        }

        // Formula ProductDeliveryFunctionCount (rulebook: =COUNTIFS(SourcingFunctions!{{ClientOrganization}}, {{OrganizationId}}, SourcingFunctions!{{DeliversOwnProduct}}, TRUE))
        [NotMapped]
        public int? ProductDeliveryFunctionCount
        {
            get => F.AsInt(F.Memo(this, "ProductDeliveryFunctionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourcingFunction>(base.SoAContext, "SourcingFunctions", __c => __c.SourcingFunctions), __r => F.CritField(F.Of(__r.ClientOrganization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.DeliversOwnProduct), F.B(true))))))); set { }
        }

        // Formula ProviderHeldDeliveryMethodCount (rulebook: =COUNTIFS(SourcingFunctions!{{ClientOrganization}}, {{OrganizationId}}, SourcingFunctions!{{DeliversOwnProduct}}, TRUE, SourcingFunctions!{{IsMethodKnowledgeHeldOutside}}, TRUE))
        [NotMapped]
        public int? ProviderHeldDeliveryMethodCount
        {
            get => F.AsInt(F.Memo(this, "ProviderHeldDeliveryMethodCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourcingFunction>(base.SoAContext, "SourcingFunctions", __c => __c.SourcingFunctions), __r => F.CritField(F.Of(__r.ClientOrganization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.DeliversOwnProduct), F.B(true)) && F.CritLiteral(F.Of(__r.IsMethodKnowledgeHeldOutside), F.B(true))))))); set { }
        }

        // Formula IsHollowedOutFirm (rulebook: =AND({{ProductDeliveryFunctionCount}} > 0, {{ProviderHeldDeliveryMethodCount}} = {{ProductDeliveryFunctionCount}}))
        [NotMapped]
        public bool? IsHollowedOutFirm
        {
            get => F.AsBool(F.Memo(this, "IsHollowedOutFirm", () => F.And(F.Bool3(F.Cmp(F.Of(this.ProductDeliveryFunctionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ProviderHeldDeliveryMethodCount), F.Of(this.ProductDeliveryFunctionCount)))))); set { }
        }

        // Formula AuditFindingCount (rulebook: =COUNTIFS(KnowledgeAuditItems!{{ClientOrganization}}, {{OrganizationId}}, KnowledgeAuditItems!{{HasInternalShortfall}}, TRUE))
        [NotMapped]
        public int? AuditFindingCount
        {
            get => F.AsInt(F.Memo(this, "AuditFindingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeAuditItem>(base.SoAContext, "KnowledgeAuditItems", __c => __c.KnowledgeAuditItems), __r => F.CritField(F.Of(__r.ClientOrganization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.HasInternalShortfall), F.B(true))))))); set { }
        }

        // Formula FilledKnowledgePositionCount (rulebook: =COUNTIFS(KnowledgeWorkforcePositions!{{Organization}}, {{OrganizationId}}, KnowledgeWorkforcePositions!{{Status}}, "Filled"))
        [NotMapped]
        public int? FilledKnowledgePositionCount
        {
            get => F.AsInt(F.Memo(this, "FilledKnowledgePositionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeWorkforcePosition>(base.SoAContext, "KnowledgeWorkforcePositions", __c => __c.KnowledgeWorkforcePositions), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.Status), F.S("Filled"))))))); set { }
        }

        // Formula HasKnowledgeFindingsWithoutKnowledgeStaff (rulebook: =AND({{AuditFindingCount}} > 0, {{FilledKnowledgePositionCount}} = 0))
        [NotMapped]
        public bool? HasKnowledgeFindingsWithoutKnowledgeStaff
        {
            get => F.AsBool(F.Memo(this, "HasKnowledgeFindingsWithoutKnowledgeStaff", () => F.And(F.Bool3(F.Cmp(F.Of(this.AuditFindingCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.FilledKnowledgePositionCount), F.I(0)))))); set { }
        }

        // Formula DepartedHolderKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Organization}}, {{OrganizationId}}, KnowHowCarriers!{{IsHeldByDepartedHolder}}, TRUE))
        [NotMapped]
        public int? DepartedHolderKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "DepartedHolderKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.IsHeldByDepartedHolder), F.B(true))))))); set { }
        }

        // Formula LostDepartedKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Organization}}, {{OrganizationId}}, KnowHowCarriers!{{IsHeldOnlyByDeparted}}, TRUE))
        [NotMapped]
        public int? LostDepartedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "LostDepartedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.IsHeldOnlyByDeparted), F.B(true))))))); set { }
        }

        // Formula RetainedDepartedKnowHowPercent (rulebook: =IF({{DepartedHolderKnowHowCount}} = 0, 100, ROUND(100 * ({{DepartedHolderKnowHowCount}} - {{LostDepartedKnowHowCount}}) / {{DepartedHolderKnowHowCount}}, 0)))
        [NotMapped]
        public decimal? RetainedDepartedKnowHowPercent
        {
            get => F.AsDecimal(F.Memo(this, "RetainedDepartedKnowHowPercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.DepartedHolderKnowHowCount), F.I(0)))) ? F.I(100) : F.Round(F.Div(F.Mul(F.I(100), F.Sub(F.Of(this.DepartedHolderKnowHowCount), F.Of(this.LostDepartedKnowHowCount))), F.Of(this.DepartedHolderKnowHowCount)), F.I(0))))); set { }
        }

        // Formula MemoryLeavesWithStaff (rulebook: =AND({{DepartedHolderKnowHowCount}} > 0, {{RetainedDepartedKnowHowPercent}} < 100))
        [NotMapped]
        public bool? MemoryLeavesWithStaff
        {
            get => F.AsBool(F.Memo(this, "MemoryLeavesWithStaff", () => F.And(F.Bool3(F.Cmp(F.Of(this.DepartedHolderKnowHowCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.RetainedDepartedKnowHowPercent), "<", F.I(100)))))); set { }
        }

        // Formula CapturedOwnKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Organization}}, {{OrganizationId}}, KnowHowCarriers!{{IsCaptured}}, TRUE))
        [NotMapped]
        public int? CapturedOwnKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "CapturedOwnKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.IsCaptured), F.B(true))))))); set { }
        }

        // Formula DocumentationEntryCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{OwnerOrganization}}, {{OrganizationId}}))
        [NotMapped]
        public int? DocumentationEntryCount
        {
            get => F.AsInt(F.Memo(this, "DocumentationEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.OwnerOrganization), F.Of(this.OrganizationId))))))); set { }
        }

        // Formula UnallocatedDocumentationCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{OwnerOrganization}}, {{OrganizationId}}, KnowledgeRepositoryEntries!{{AuthoredOnAllocatedTime}}, FALSE))
        [NotMapped]
        public int? UnallocatedDocumentationCount
        {
            get => F.AsInt(F.Memo(this, "UnallocatedDocumentationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.OwnerOrganization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.AuthoredOnAllocatedTime), F.B(false))))))); set { }
        }

        // Formula TransferGivenCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{FromOrganization}}, {{OrganizationId}}))
        [NotMapped]
        public int? TransferGivenCount
        {
            get => F.AsInt(F.Memo(this, "TransferGivenCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.FromOrganization), F.Of(this.OrganizationId))))))); set { }
        }

        // Formula UnallocatedTransferCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{FromOrganization}}, {{OrganizationId}}, KnowledgeTransfers!{{OnAllocatedTime}}, FALSE))
        [NotMapped]
        public int? UnallocatedTransferCount
        {
            get => F.AsInt(F.Memo(this, "UnallocatedTransferCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.FromOrganization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.OnAllocatedTime), F.B(false))))))); set { }
        }

        // Formula TreatsKnowledgeWorkAsUnvalued (rulebook: =AND({{DocumentationEntryCount}} + {{TransferGivenCount}} > 0, 2 * ({{UnallocatedDocumentationCount}} + {{UnallocatedTransferCount}}) > {{DocumentationEntryCount}} + {{TransferGivenCount}}))
        [NotMapped]
        public bool? TreatsKnowledgeWorkAsUnvalued
        {
            get => F.AsBool(F.Memo(this, "TreatsKnowledgeWorkAsUnvalued", () => F.And(F.Bool3(F.Cmp(F.Add(F.Of(this.DocumentationEntryCount), F.Of(this.TransferGivenCount)), ">", F.I(0))), F.Bool3(F.Cmp(F.Mul(F.I(2), F.Add(F.Of(this.UnallocatedDocumentationCount), F.Of(this.UnallocatedTransferCount))), ">", F.Add(F.Of(this.DocumentationEntryCount), F.Of(this.TransferGivenCount))))))); set { }
        }

        // Formula PersonCarriedKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Organization}}, {{OrganizationId}}, KnowHowCarriers!{{CarrierKind}}, "Person"))
        [NotMapped]
        public int? PersonCarriedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "PersonCarriedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.CarrierKind), F.S("Person"))))))); set { }
        }

        // Formula FacilityCarriedKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Organization}}, {{OrganizationId}}, KnowHowCarriers!{{CarrierKind}}, "Facility"))
        [NotMapped]
        public int? FacilityCarriedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "FacilityCarriedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.CarrierKind), F.S("Facility"))))))); set { }
        }

        // Formula SystemCarriedKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Organization}}, {{OrganizationId}}, KnowHowCarriers!{{CarrierKind}}, "System"))
        [NotMapped]
        public int? SystemCarriedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "SystemCarriedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.CarrierKind), F.S("System"))))))); set { }
        }

        // Formula HoldsKnowHowInPeoplePlantsAndSystems (rulebook: =AND({{PersonCarriedKnowHowCount}} > 0, {{FacilityCarriedKnowHowCount}} > 0, {{SystemCarriedKnowHowCount}} > 0))
        [NotMapped]
        public bool? HoldsKnowHowInPeoplePlantsAndSystems
        {
            get => F.AsBool(F.Memo(this, "HoldsKnowHowInPeoplePlantsAndSystems", () => F.And(F.Bool3(F.Cmp(F.Of(this.PersonCarriedKnowHowCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.FacilityCarriedKnowHowCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.SystemCarriedKnowHowCount), ">", F.I(0)))))); set { }
        }

        // Formula StagedDeclineCount (rulebook: =COUNTIFS(CapabilityDeclines!{{Organization}}, {{OrganizationId}}, CapabilityDeclines!{{FollowsPrecedingStageDecline}}, TRUE))
        [NotMapped]
        public int? StagedDeclineCount
        {
            get => F.AsInt(F.Memo(this, "StagedDeclineCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CapabilityDecline>(base.SoAContext, "CapabilityDeclines", __c => __c.CapabilityDeclines), __r => F.CritField(F.Of(__r.Organization), F.Of(this.OrganizationId)) && F.CritLiteral(F.Of(__r.FollowsPrecedingStageDecline), F.B(true))))))); set { }
        }

        // Formula ErodedInStages (rulebook: ={{StagedDeclineCount}} >= 2)
        [NotMapped]
        public bool? ErodedInStages
        {
            get => F.AsBool(F.Memo(this, "ErodedInStages", () => F.Cmp(F.Of(this.StagedDeclineCount), ">=", F.I(2)))); set { }
        }



        private ObservableCollection<Agent> _organizationAgents;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<Agent> OrganizationAgents
        {
            get
            {
                if (_organizationAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationAgents - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _organizationAgents = new ObservableCollection<Agent>();
                    }
                    else
                    {
                        var items = base.SoAContext.Agents.Where(x => x.Organization == this.OrganizationId).ToList<Agent>();
                        _organizationAgents = new ObservableCollection<Agent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _organizationAgents.CollectionChanged += OrganizationAgents_CollectionChanged;
                }
                return _organizationAgents;
            }
            private set
            {
                if (_organizationAgents != null)
                {
                    _organizationAgents.CollectionChanged -= OrganizationAgents_CollectionChanged;
                }
                _organizationAgents = value;
                if (_organizationAgents != null)
                {
                    _organizationAgents.CollectionChanged += OrganizationAgents_CollectionChanged;
                }
            }
        }

        private void OrganizationAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Agent>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Agent> _representsOrganizationAgents;

        [InverseProperty("OrganizationRefRef")]
        public virtual ObservableCollection<Agent> RepresentsOrganizationAgents
        {
            get
            {
                if (_representsOrganizationAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RepresentsOrganizationAgents - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _representsOrganizationAgents = new ObservableCollection<Agent>();
                    }
                    else
                    {
                        var items = base.SoAContext.Agents.Where(x => x.RepresentsOrganization == this.OrganizationId).ToList<Agent>();
                        _representsOrganizationAgents = new ObservableCollection<Agent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _representsOrganizationAgents.CollectionChanged += RepresentsOrganizationAgents_CollectionChanged;
                }
                return _representsOrganizationAgents;
            }
            private set
            {
                if (_representsOrganizationAgents != null)
                {
                    _representsOrganizationAgents.CollectionChanged -= RepresentsOrganizationAgents_CollectionChanged;
                }
                _representsOrganizationAgents = value;
                if (_representsOrganizationAgents != null)
                {
                    _representsOrganizationAgents.CollectionChanged += RepresentsOrganizationAgents_CollectionChanged;
                }
            }
        }

        private void RepresentsOrganizationAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Agent>())
                {
                    item.RepresentsOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Role> _roles;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.Organization == this.OrganizationId).ToList<Role>();
                        _roles = new ObservableCollection<Role>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
                return _roles;
            }
            private set
            {
                if (_roles != null)
                {
                    _roles.CollectionChanged -= Roles_CollectionChanged;
                }
                _roles = value;
                if (_roles != null)
                {
                    _roles.CollectionChanged += Roles_CollectionChanged;
                }
            }
        }

        private void Roles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Role>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<CommunitiesOfPractice> _communitiesOfPractice;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<CommunitiesOfPractice> CommunitiesOfPractice
        {
            get
            {
                if (_communitiesOfPractice == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunitiesOfPractice.Where(x => x.Organization == this.OrganizationId).ToList<CommunitiesOfPractice>();
                        _communitiesOfPractice = new ObservableCollection<CommunitiesOfPractice>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _communitiesOfPractice.CollectionChanged += CommunitiesOfPractice_CollectionChanged;
                }
                return _communitiesOfPractice;
            }
            private set
            {
                if (_communitiesOfPractice != null)
                {
                    _communitiesOfPractice.CollectionChanged -= CommunitiesOfPractice_CollectionChanged;
                }
                _communitiesOfPractice = value;
                if (_communitiesOfPractice != null)
                {
                    _communitiesOfPractice.CollectionChanged += CommunitiesOfPractice_CollectionChanged;
                }
            }
        }

        private void CommunitiesOfPractice_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CommunitiesOfPractice>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Procedure> _ownerOrganizationProcedures;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Procedure> OwnerOrganizationProcedures
        {
            get
            {
                if (_ownerOrganizationProcedures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OwnerOrganizationProcedures - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _ownerOrganizationProcedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = base.SoAContext.Procedures.Where(x => x.OwnerOrganization == this.OrganizationId).ToList<Procedure>();
                        _ownerOrganizationProcedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _ownerOrganizationProcedures.CollectionChanged += OwnerOrganizationProcedures_CollectionChanged;
                }
                return _ownerOrganizationProcedures;
            }
            private set
            {
                if (_ownerOrganizationProcedures != null)
                {
                    _ownerOrganizationProcedures.CollectionChanged -= OwnerOrganizationProcedures_CollectionChanged;
                }
                _ownerOrganizationProcedures = value;
                if (_ownerOrganizationProcedures != null)
                {
                    _ownerOrganizationProcedures.CollectionChanged += OwnerOrganizationProcedures_CollectionChanged;
                }
            }
        }

        private void OwnerOrganizationProcedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.OwnerOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Procedure> _adoptedByOrganizationProcedures;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<Procedure> AdoptedByOrganizationProcedures
        {
            get
            {
                if (_adoptedByOrganizationProcedures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AdoptedByOrganizationProcedures - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _adoptedByOrganizationProcedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = base.SoAContext.Procedures.Where(x => x.AdoptedByOrganization == this.OrganizationId).ToList<Procedure>();
                        _adoptedByOrganizationProcedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _adoptedByOrganizationProcedures.CollectionChanged += AdoptedByOrganizationProcedures_CollectionChanged;
                }
                return _adoptedByOrganizationProcedures;
            }
            private set
            {
                if (_adoptedByOrganizationProcedures != null)
                {
                    _adoptedByOrganizationProcedures.CollectionChanged -= AdoptedByOrganizationProcedures_CollectionChanged;
                }
                _adoptedByOrganizationProcedures = value;
                if (_adoptedByOrganizationProcedures != null)
                {
                    _adoptedByOrganizationProcedures.CollectionChanged += AdoptedByOrganizationProcedures_CollectionChanged;
                }
            }
        }

        private void AdoptedByOrganizationProcedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.AdoptedByOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Recipient> _recipients;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<Recipient> Recipients
        {
            get
            {
                if (_recipients == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Recipients - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _recipients = new ObservableCollection<Recipient>();
                    }
                    else
                    {
                        var items = base.SoAContext.Recipients.Where(x => x.Organization == this.OrganizationId).ToList<Recipient>();
                        _recipients = new ObservableCollection<Recipient>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _recipients.CollectionChanged += Recipients_CollectionChanged;
                }
                return _recipients;
            }
            private set
            {
                if (_recipients != null)
                {
                    _recipients.CollectionChanged -= Recipients_CollectionChanged;
                }
                _recipients = value;
                if (_recipients != null)
                {
                    _recipients.CollectionChanged += Recipients_CollectionChanged;
                }
            }
        }

        private void Recipients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Recipient>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Facility> _facilities;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<Facility> Facilities
        {
            get
            {
                if (_facilities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Facilities - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _facilities = new ObservableCollection<Facility>();
                    }
                    else
                    {
                        var items = base.SoAContext.Facilities.Where(x => x.Organization == this.OrganizationId).ToList<Facility>();
                        _facilities = new ObservableCollection<Facility>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _facilities.CollectionChanged += Facilities_CollectionChanged;
                }
                return _facilities;
            }
            private set
            {
                if (_facilities != null)
                {
                    _facilities.CollectionChanged -= Facilities_CollectionChanged;
                }
                _facilities = value;
                if (_facilities != null)
                {
                    _facilities.CollectionChanged += Facilities_CollectionChanged;
                }
            }
        }

        private void Facilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Facility>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<Machine> _machines;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<Machine> Machines
        {
            get
            {
                if (_machines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Machines - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _machines = new ObservableCollection<Machine>();
                    }
                    else
                    {
                        var items = base.SoAContext.Machines.Where(x => x.ManufacturedBy == this.OrganizationId).ToList<Machine>();
                        _machines = new ObservableCollection<Machine>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _machines.CollectionChanged += Machines_CollectionChanged;
                }
                return _machines;
            }
            private set
            {
                if (_machines != null)
                {
                    _machines.CollectionChanged -= Machines_CollectionChanged;
                }
                _machines = value;
                if (_machines != null)
                {
                    _machines.CollectionChanged += Machines_CollectionChanged;
                }
            }
        }

        private void Machines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Machine>())
                {
                    item.ManufacturedBy = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<ProcedureAdoption> _procedureAdoptions;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<ProcedureAdoption> ProcedureAdoptions
        {
            get
            {
                if (_procedureAdoptions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureAdoptions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _procedureAdoptions = new ObservableCollection<ProcedureAdoption>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureAdoptions.Where(x => x.Organization == this.OrganizationId).ToList<ProcedureAdoption>();
                        _procedureAdoptions = new ObservableCollection<ProcedureAdoption>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureAdoptions.CollectionChanged += ProcedureAdoptions_CollectionChanged;
                }
                return _procedureAdoptions;
            }
            private set
            {
                if (_procedureAdoptions != null)
                {
                    _procedureAdoptions.CollectionChanged -= ProcedureAdoptions_CollectionChanged;
                }
                _procedureAdoptions = value;
                if (_procedureAdoptions != null)
                {
                    _procedureAdoptions.CollectionChanged += ProcedureAdoptions_CollectionChanged;
                }
            }
        }

        private void ProcedureAdoptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureAdoption>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<ProcessStrategicAlignment> _processStrategicAlignments;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<ProcessStrategicAlignment> ProcessStrategicAlignments
        {
            get
            {
                if (_processStrategicAlignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessStrategicAlignments - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _processStrategicAlignments = new ObservableCollection<ProcessStrategicAlignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessStrategicAlignments.Where(x => x.Organization == this.OrganizationId).ToList<ProcessStrategicAlignment>();
                        _processStrategicAlignments = new ObservableCollection<ProcessStrategicAlignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processStrategicAlignments.CollectionChanged += ProcessStrategicAlignments_CollectionChanged;
                }
                return _processStrategicAlignments;
            }
            private set
            {
                if (_processStrategicAlignments != null)
                {
                    _processStrategicAlignments.CollectionChanged -= ProcessStrategicAlignments_CollectionChanged;
                }
                _processStrategicAlignments = value;
                if (_processStrategicAlignments != null)
                {
                    _processStrategicAlignments.CollectionChanged += ProcessStrategicAlignments_CollectionChanged;
                }
            }
        }

        private void ProcessStrategicAlignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessStrategicAlignment>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<ApplicabilityScope> _applicabilityScopes;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<ApplicabilityScope> ApplicabilityScopes
        {
            get
            {
                if (_applicabilityScopes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApplicabilityScopes - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _applicabilityScopes = new ObservableCollection<ApplicabilityScope>();
                    }
                    else
                    {
                        var items = base.SoAContext.ApplicabilityScopes.Where(x => x.BusinessUnit == this.OrganizationId).ToList<ApplicabilityScope>();
                        _applicabilityScopes = new ObservableCollection<ApplicabilityScope>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _applicabilityScopes.CollectionChanged += ApplicabilityScopes_CollectionChanged;
                }
                return _applicabilityScopes;
            }
            private set
            {
                if (_applicabilityScopes != null)
                {
                    _applicabilityScopes.CollectionChanged -= ApplicabilityScopes_CollectionChanged;
                }
                _applicabilityScopes = value;
                if (_applicabilityScopes != null)
                {
                    _applicabilityScopes.CollectionChanged += ApplicabilityScopes_CollectionChanged;
                }
            }
        }

        private void ApplicabilityScopes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ApplicabilityScope>())
                {
                    item.BusinessUnit = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<KnowledgeConsumerSystem> _knowledgeConsumerSystems;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<KnowledgeConsumerSystem> KnowledgeConsumerSystems
        {
            get
            {
                if (_knowledgeConsumerSystems == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeConsumerSystems - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _knowledgeConsumerSystems = new ObservableCollection<KnowledgeConsumerSystem>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeConsumerSystems.Where(x => x.Organization == this.OrganizationId).ToList<KnowledgeConsumerSystem>();
                        _knowledgeConsumerSystems = new ObservableCollection<KnowledgeConsumerSystem>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeConsumerSystems.CollectionChanged += KnowledgeConsumerSystems_CollectionChanged;
                }
                return _knowledgeConsumerSystems;
            }
            private set
            {
                if (_knowledgeConsumerSystems != null)
                {
                    _knowledgeConsumerSystems.CollectionChanged -= KnowledgeConsumerSystems_CollectionChanged;
                }
                _knowledgeConsumerSystems = value;
                if (_knowledgeConsumerSystems != null)
                {
                    _knowledgeConsumerSystems.CollectionChanged += KnowledgeConsumerSystems_CollectionChanged;
                }
            }
        }

        private void KnowledgeConsumerSystems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeConsumerSystem>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<AiAdoptionInitiatif> _aiAdoptionInitiatives;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<AiAdoptionInitiatif> AiAdoptionInitiatives
        {
            get
            {
                if (_aiAdoptionInitiatives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatives - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAdoptionInitiatives.Where(x => x.Organization == this.OrganizationId).ToList<AiAdoptionInitiatif>();
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
                return _aiAdoptionInitiatives;
            }
            private set
            {
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged -= AiAdoptionInitiatives_CollectionChanged;
                }
                _aiAdoptionInitiatives = value;
                if (_aiAdoptionInitiatives != null)
                {
                    _aiAdoptionInitiatives.CollectionChanged += AiAdoptionInitiatives_CollectionChanged;
                }
            }
        }

        private void AiAdoptionInitiatives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAdoptionInitiatif>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<GovernedModel> _governedModels;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<GovernedModel> GovernedModels
        {
            get
            {
                if (_governedModels == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModels - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _governedModels = new ObservableCollection<GovernedModel>();
                    }
                    else
                    {
                        var items = base.SoAContext.GovernedModels.Where(x => x.DomainOwningOrganization == this.OrganizationId).ToList<GovernedModel>();
                        _governedModels = new ObservableCollection<GovernedModel>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _governedModels.CollectionChanged += GovernedModels_CollectionChanged;
                }
                return _governedModels;
            }
            private set
            {
                if (_governedModels != null)
                {
                    _governedModels.CollectionChanged -= GovernedModels_CollectionChanged;
                }
                _governedModels = value;
                if (_governedModels != null)
                {
                    _governedModels.CollectionChanged += GovernedModels_CollectionChanged;
                }
            }
        }

        private void GovernedModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<GovernedModel>())
                {
                    item.DomainOwningOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<ModelExpansionRequest> _modelExpansionRequests;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<ModelExpansionRequest> ModelExpansionRequests
        {
            get
            {
                if (_modelExpansionRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelExpansionRequests - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _modelExpansionRequests = new ObservableCollection<ModelExpansionRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelExpansionRequests.Where(x => x.RequestingOrganization == this.OrganizationId).ToList<ModelExpansionRequest>();
                        _modelExpansionRequests = new ObservableCollection<ModelExpansionRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelExpansionRequests.CollectionChanged += ModelExpansionRequests_CollectionChanged;
                }
                return _modelExpansionRequests;
            }
            private set
            {
                if (_modelExpansionRequests != null)
                {
                    _modelExpansionRequests.CollectionChanged -= ModelExpansionRequests_CollectionChanged;
                }
                _modelExpansionRequests = value;
                if (_modelExpansionRequests != null)
                {
                    _modelExpansionRequests.CollectionChanged += ModelExpansionRequests_CollectionChanged;
                }
            }
        }

        private void ModelExpansionRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelExpansionRequest>())
                {
                    item.RequestingOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<SourcingFunction> _clientOrganizationSourcingFunctions;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<SourcingFunction> ClientOrganizationSourcingFunctions
        {
            get
            {
                if (_clientOrganizationSourcingFunctions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClientOrganizationSourcingFunctions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _clientOrganizationSourcingFunctions = new ObservableCollection<SourcingFunction>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourcingFunctions.Where(x => x.ClientOrganization == this.OrganizationId).ToList<SourcingFunction>();
                        _clientOrganizationSourcingFunctions = new ObservableCollection<SourcingFunction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _clientOrganizationSourcingFunctions.CollectionChanged += ClientOrganizationSourcingFunctions_CollectionChanged;
                }
                return _clientOrganizationSourcingFunctions;
            }
            private set
            {
                if (_clientOrganizationSourcingFunctions != null)
                {
                    _clientOrganizationSourcingFunctions.CollectionChanged -= ClientOrganizationSourcingFunctions_CollectionChanged;
                }
                _clientOrganizationSourcingFunctions = value;
                if (_clientOrganizationSourcingFunctions != null)
                {
                    _clientOrganizationSourcingFunctions.CollectionChanged += ClientOrganizationSourcingFunctions_CollectionChanged;
                }
            }
        }

        private void ClientOrganizationSourcingFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourcingFunction>())
                {
                    item.ClientOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<SourcingFunction> _executingOrganizationSourcingFunctions;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<SourcingFunction> ExecutingOrganizationSourcingFunctions
        {
            get
            {
                if (_executingOrganizationSourcingFunctions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExecutingOrganizationSourcingFunctions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _executingOrganizationSourcingFunctions = new ObservableCollection<SourcingFunction>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourcingFunctions.Where(x => x.ExecutingOrganization == this.OrganizationId).ToList<SourcingFunction>();
                        _executingOrganizationSourcingFunctions = new ObservableCollection<SourcingFunction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _executingOrganizationSourcingFunctions.CollectionChanged += ExecutingOrganizationSourcingFunctions_CollectionChanged;
                }
                return _executingOrganizationSourcingFunctions;
            }
            private set
            {
                if (_executingOrganizationSourcingFunctions != null)
                {
                    _executingOrganizationSourcingFunctions.CollectionChanged -= ExecutingOrganizationSourcingFunctions_CollectionChanged;
                }
                _executingOrganizationSourcingFunctions = value;
                if (_executingOrganizationSourcingFunctions != null)
                {
                    _executingOrganizationSourcingFunctions.CollectionChanged += ExecutingOrganizationSourcingFunctions_CollectionChanged;
                }
            }
        }

        private void ExecutingOrganizationSourcingFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourcingFunction>())
                {
                    item.ExecutingOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<SourcingFunction> _specificationHolderSourcingFunctions;

        [InverseProperty("OrganizationRefRef")]
        public virtual ObservableCollection<SourcingFunction> SpecificationHolderSourcingFunctions
        {
            get
            {
                if (_specificationHolderSourcingFunctions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SpecificationHolderSourcingFunctions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _specificationHolderSourcingFunctions = new ObservableCollection<SourcingFunction>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourcingFunctions.Where(x => x.SpecificationHolder == this.OrganizationId).ToList<SourcingFunction>();
                        _specificationHolderSourcingFunctions = new ObservableCollection<SourcingFunction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _specificationHolderSourcingFunctions.CollectionChanged += SpecificationHolderSourcingFunctions_CollectionChanged;
                }
                return _specificationHolderSourcingFunctions;
            }
            private set
            {
                if (_specificationHolderSourcingFunctions != null)
                {
                    _specificationHolderSourcingFunctions.CollectionChanged -= SpecificationHolderSourcingFunctions_CollectionChanged;
                }
                _specificationHolderSourcingFunctions = value;
                if (_specificationHolderSourcingFunctions != null)
                {
                    _specificationHolderSourcingFunctions.CollectionChanged += SpecificationHolderSourcingFunctions_CollectionChanged;
                }
            }
        }

        private void SpecificationHolderSourcingFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourcingFunction>())
                {
                    item.SpecificationHolder = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<SourcingFunction> _methodHolderSourcingFunctions;

        [InverseProperty("OrganizationRefRefRef")]
        public virtual ObservableCollection<SourcingFunction> MethodHolderSourcingFunctions
        {
            get
            {
                if (_methodHolderSourcingFunctions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MethodHolderSourcingFunctions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _methodHolderSourcingFunctions = new ObservableCollection<SourcingFunction>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourcingFunctions.Where(x => x.MethodHolder == this.OrganizationId).ToList<SourcingFunction>();
                        _methodHolderSourcingFunctions = new ObservableCollection<SourcingFunction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _methodHolderSourcingFunctions.CollectionChanged += MethodHolderSourcingFunctions_CollectionChanged;
                }
                return _methodHolderSourcingFunctions;
            }
            private set
            {
                if (_methodHolderSourcingFunctions != null)
                {
                    _methodHolderSourcingFunctions.CollectionChanged -= MethodHolderSourcingFunctions_CollectionChanged;
                }
                _methodHolderSourcingFunctions = value;
                if (_methodHolderSourcingFunctions != null)
                {
                    _methodHolderSourcingFunctions.CollectionChanged += MethodHolderSourcingFunctions_CollectionChanged;
                }
            }
        }

        private void MethodHolderSourcingFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourcingFunction>())
                {
                    item.MethodHolder = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<KnowledgeAudit> _knowledgeAudits;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<KnowledgeAudit> KnowledgeAudits
        {
            get
            {
                if (_knowledgeAudits == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeAudits - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _knowledgeAudits = new ObservableCollection<KnowledgeAudit>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeAudits.Where(x => x.Organization == this.OrganizationId).ToList<KnowledgeAudit>();
                        _knowledgeAudits = new ObservableCollection<KnowledgeAudit>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeAudits.CollectionChanged += KnowledgeAudits_CollectionChanged;
                }
                return _knowledgeAudits;
            }
            private set
            {
                if (_knowledgeAudits != null)
                {
                    _knowledgeAudits.CollectionChanged -= KnowledgeAudits_CollectionChanged;
                }
                _knowledgeAudits = value;
                if (_knowledgeAudits != null)
                {
                    _knowledgeAudits.CollectionChanged += KnowledgeAudits_CollectionChanged;
                }
            }
        }

        private void KnowledgeAudits_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeAudit>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<KnowledgeAuditItem> _knowledgeAuditItems;

        [InverseProperty("Organization")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeAuditItems - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _knowledgeAuditItems = new ObservableCollection<KnowledgeAuditItem>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeAuditItems.Where(x => x.ProviderHoldingKnowledge == this.OrganizationId).ToList<KnowledgeAuditItem>();
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
                    item.ProviderHoldingKnowledge = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<KnowledgeWorkforcePosition> _knowledgeWorkforcePositions;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<KnowledgeWorkforcePosition> KnowledgeWorkforcePositions
        {
            get
            {
                if (_knowledgeWorkforcePositions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeWorkforcePositions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _knowledgeWorkforcePositions = new ObservableCollection<KnowledgeWorkforcePosition>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeWorkforcePositions.Where(x => x.Organization == this.OrganizationId).ToList<KnowledgeWorkforcePosition>();
                        _knowledgeWorkforcePositions = new ObservableCollection<KnowledgeWorkforcePosition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeWorkforcePositions.CollectionChanged += KnowledgeWorkforcePositions_CollectionChanged;
                }
                return _knowledgeWorkforcePositions;
            }
            private set
            {
                if (_knowledgeWorkforcePositions != null)
                {
                    _knowledgeWorkforcePositions.CollectionChanged -= KnowledgeWorkforcePositions_CollectionChanged;
                }
                _knowledgeWorkforcePositions = value;
                if (_knowledgeWorkforcePositions != null)
                {
                    _knowledgeWorkforcePositions.CollectionChanged += KnowledgeWorkforcePositions_CollectionChanged;
                }
            }
        }

        private void KnowledgeWorkforcePositions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeWorkforcePosition>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<ProviderEngagement> _clientOrganizationProviderEngagements;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<ProviderEngagement> ClientOrganizationProviderEngagements
        {
            get
            {
                if (_clientOrganizationProviderEngagements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ClientOrganizationProviderEngagements - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _clientOrganizationProviderEngagements = new ObservableCollection<ProviderEngagement>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProviderEngagements.Where(x => x.ClientOrganization == this.OrganizationId).ToList<ProviderEngagement>();
                        _clientOrganizationProviderEngagements = new ObservableCollection<ProviderEngagement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _clientOrganizationProviderEngagements.CollectionChanged += ClientOrganizationProviderEngagements_CollectionChanged;
                }
                return _clientOrganizationProviderEngagements;
            }
            private set
            {
                if (_clientOrganizationProviderEngagements != null)
                {
                    _clientOrganizationProviderEngagements.CollectionChanged -= ClientOrganizationProviderEngagements_CollectionChanged;
                }
                _clientOrganizationProviderEngagements = value;
                if (_clientOrganizationProviderEngagements != null)
                {
                    _clientOrganizationProviderEngagements.CollectionChanged += ClientOrganizationProviderEngagements_CollectionChanged;
                }
            }
        }

        private void ClientOrganizationProviderEngagements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProviderEngagement>())
                {
                    item.ClientOrganization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<ProviderEngagement> _providerProviderEngagements;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<ProviderEngagement> ProviderProviderEngagements
        {
            get
            {
                if (_providerProviderEngagements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProviderProviderEngagements - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _providerProviderEngagements = new ObservableCollection<ProviderEngagement>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProviderEngagements.Where(x => x.Provider == this.OrganizationId).ToList<ProviderEngagement>();
                        _providerProviderEngagements = new ObservableCollection<ProviderEngagement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _providerProviderEngagements.CollectionChanged += ProviderProviderEngagements_CollectionChanged;
                }
                return _providerProviderEngagements;
            }
            private set
            {
                if (_providerProviderEngagements != null)
                {
                    _providerProviderEngagements.CollectionChanged -= ProviderProviderEngagements_CollectionChanged;
                }
                _providerProviderEngagements = value;
                if (_providerProviderEngagements != null)
                {
                    _providerProviderEngagements.CollectionChanged += ProviderProviderEngagements_CollectionChanged;
                }
            }
        }

        private void ProviderProviderEngagements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProviderEngagement>())
                {
                    item.Provider = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<CorporateGovernanceProgram> _corporateGovernancePrograms;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<CorporateGovernanceProgram> CorporateGovernancePrograms
        {
            get
            {
                if (_corporateGovernancePrograms == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CorporateGovernancePrograms - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _corporateGovernancePrograms = new ObservableCollection<CorporateGovernanceProgram>();
                    }
                    else
                    {
                        var items = base.SoAContext.CorporateGovernancePrograms.Where(x => x.Organization == this.OrganizationId).ToList<CorporateGovernanceProgram>();
                        _corporateGovernancePrograms = new ObservableCollection<CorporateGovernanceProgram>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _corporateGovernancePrograms.CollectionChanged += CorporateGovernancePrograms_CollectionChanged;
                }
                return _corporateGovernancePrograms;
            }
            private set
            {
                if (_corporateGovernancePrograms != null)
                {
                    _corporateGovernancePrograms.CollectionChanged -= CorporateGovernancePrograms_CollectionChanged;
                }
                _corporateGovernancePrograms = value;
                if (_corporateGovernancePrograms != null)
                {
                    _corporateGovernancePrograms.CollectionChanged += CorporateGovernancePrograms_CollectionChanged;
                }
            }
        }

        private void CorporateGovernancePrograms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CorporateGovernanceProgram>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<KnowHowCarrier> KnowHowCarriers
        {
            get
            {
                if (_knowHowCarriers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.Organization == this.OrganizationId).ToList<KnowHowCarrier>();
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowHowCarriers.CollectionChanged += KnowHowCarriers_CollectionChanged;
                }
                return _knowHowCarriers;
            }
            private set
            {
                if (_knowHowCarriers != null)
                {
                    _knowHowCarriers.CollectionChanged -= KnowHowCarriers_CollectionChanged;
                }
                _knowHowCarriers = value;
                if (_knowHowCarriers != null)
                {
                    _knowHowCarriers.CollectionChanged += KnowHowCarriers_CollectionChanged;
                }
            }
        }

        private void KnowHowCarriers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowHowCarrier>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<DepartmentProcessAccount> _departmentProcessAccounts;

        [InverseProperty("Organization")]
        public virtual ObservableCollection<DepartmentProcessAccount> DepartmentProcessAccounts
        {
            get
            {
                if (_departmentProcessAccounts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DepartmentProcessAccounts - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _departmentProcessAccounts = new ObservableCollection<DepartmentProcessAccount>();
                    }
                    else
                    {
                        var items = base.SoAContext.DepartmentProcessAccounts.Where(x => x.Department == this.OrganizationId).ToList<DepartmentProcessAccount>();
                        _departmentProcessAccounts = new ObservableCollection<DepartmentProcessAccount>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _departmentProcessAccounts.CollectionChanged += DepartmentProcessAccounts_CollectionChanged;
                }
                return _departmentProcessAccounts;
            }
            private set
            {
                if (_departmentProcessAccounts != null)
                {
                    _departmentProcessAccounts.CollectionChanged -= DepartmentProcessAccounts_CollectionChanged;
                }
                _departmentProcessAccounts = value;
                if (_departmentProcessAccounts != null)
                {
                    _departmentProcessAccounts.CollectionChanged += DepartmentProcessAccounts_CollectionChanged;
                }
            }
        }

        private void DepartmentProcessAccounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DepartmentProcessAccount>())
                {
                    item.Department = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<SharingRecognition> _sharingRecognitions;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<SharingRecognition> SharingRecognitions
        {
            get
            {
                if (_sharingRecognitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SharingRecognitions - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _sharingRecognitions = new ObservableCollection<SharingRecognition>();
                    }
                    else
                    {
                        var items = base.SoAContext.SharingRecognitions.Where(x => x.Organization == this.OrganizationId).ToList<SharingRecognition>();
                        _sharingRecognitions = new ObservableCollection<SharingRecognition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sharingRecognitions.CollectionChanged += SharingRecognitions_CollectionChanged;
                }
                return _sharingRecognitions;
            }
            private set
            {
                if (_sharingRecognitions != null)
                {
                    _sharingRecognitions.CollectionChanged -= SharingRecognitions_CollectionChanged;
                }
                _sharingRecognitions = value;
                if (_sharingRecognitions != null)
                {
                    _sharingRecognitions.CollectionChanged += SharingRecognitions_CollectionChanged;
                }
            }
        }

        private void SharingRecognitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SharingRecognition>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }

        private ObservableCollection<CapabilityDecline> _capabilityDeclines;

        [InverseProperty("OrganizationRef")]
        public virtual ObservableCollection<CapabilityDecline> CapabilityDeclines
        {
            get
            {
                if (_capabilityDeclines == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CapabilityDeclines - no database context is set. OrganizationId: " + this.OrganizationId + ".");
                        }
                        _capabilityDeclines = new ObservableCollection<CapabilityDecline>();
                    }
                    else
                    {
                        var items = base.SoAContext.CapabilityDeclines.Where(x => x.Organization == this.OrganizationId).ToList<CapabilityDecline>();
                        _capabilityDeclines = new ObservableCollection<CapabilityDecline>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _capabilityDeclines.CollectionChanged += CapabilityDeclines_CollectionChanged;
                }
                return _capabilityDeclines;
            }
            private set
            {
                if (_capabilityDeclines != null)
                {
                    _capabilityDeclines.CollectionChanged -= CapabilityDeclines_CollectionChanged;
                }
                _capabilityDeclines = value;
                if (_capabilityDeclines != null)
                {
                    _capabilityDeclines.CollectionChanged += CapabilityDeclines_CollectionChanged;
                }
            }
        }

        private void CapabilityDeclines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CapabilityDecline>())
                {
                    item.Organization = this.OrganizationId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationAgents;
            _ = this.RepresentsOrganizationAgents;
            _ = this.Roles;
            _ = this.CommunitiesOfPractice;
            _ = this.OwnerOrganizationProcedures;
            _ = this.AdoptedByOrganizationProcedures;
            _ = this.Recipients;
            _ = this.Facilities;
            _ = this.Machines;
            _ = this.ProcedureAdoptions;
            _ = this.ProcessStrategicAlignments;
            _ = this.ApplicabilityScopes;
            _ = this.KnowledgeConsumerSystems;
            _ = this.AiAdoptionInitiatives;
            _ = this.GovernedModels;
            _ = this.ModelExpansionRequests;
            _ = this.ClientOrganizationSourcingFunctions;
            _ = this.ExecutingOrganizationSourcingFunctions;
            _ = this.SpecificationHolderSourcingFunctions;
            _ = this.MethodHolderSourcingFunctions;
            _ = this.KnowledgeAudits;
            _ = this.KnowledgeAuditItems;
            _ = this.KnowledgeWorkforcePositions;
            _ = this.ClientOrganizationProviderEngagements;
            _ = this.ProviderProviderEngagements;
            _ = this.CorporateGovernancePrograms;
            _ = this.KnowHowCarriers;
            _ = this.DepartmentProcessAccounts;
            _ = this.SharingRecognitions;
            _ = this.CapabilityDeclines;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
