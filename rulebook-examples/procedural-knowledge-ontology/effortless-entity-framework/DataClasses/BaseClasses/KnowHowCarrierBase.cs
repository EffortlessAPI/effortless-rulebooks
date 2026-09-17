
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
    [Table("KnowHowCarriers")]
    public class KnowHowCarrierBase : SoAEntityBase
    {
        [Key]
        public string KnowHowCarrierId { get; set; }

        // Formula Name (rulebook: ={{Topic}} & " / " & {{CarrierKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Topic)), F.S(" / "), F.Text(F.Of(this.CarrierKind))))); set { }
        }

        public string? Topic { get; set; }
        public string? CarrierKind { get; set; }
        public string? KnowHowKind { get; set; }
        public bool? IsInWrittenProcedure { get; set; }
        public bool? IsTrainedSkill { get; set; }
        public string? WorkMedium { get; set; }
        public DateTimeOffset? HeldSince { get; set; }
        public bool? IsInPublicReferences { get; set; }
        public string? ReplacementPlan { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula HolderIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{HolderAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? HolderIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "HolderIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.HolderAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula HolderServiceStartedAt (rulebook: =INDEX(Agents!{{ServiceStartedAt}}, MATCH({{HolderAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public DateTimeOffset? HolderServiceStartedAt
        {
            get => F.AsDateTime(F.Memo(this, "HolderServiceStartedAt", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.HolderAgent), __r => F.Of(__r.ServiceStartedAt), () => F.Of(new Agent().ServiceStartedAt)))); set { }
        }

        // Formula HolderDepartureAt (rulebook: =INDEX(Agents!{{DepartureAt}}, MATCH({{HolderAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public DateTimeOffset? HolderDepartureAt
        {
            get => F.AsDateTime(F.Memo(this, "HolderDepartureAt", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.HolderAgent), __r => F.Of(__r.DepartureAt), () => F.Of(new Agent().DepartureAt)))); set { }
        }

        // Formula DaysUntilHolderDeparture (rulebook: =IF({{HolderDepartureAt}} = "", 99999, DATETIME_DIFF({{HolderDepartureAt}}, {{AsOfInstant}}, "days")))
        [NotMapped]
        public int? DaysUntilHolderDeparture
        {
            get => F.AsInt(F.Memo(this, "DaysUntilHolderDeparture", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.HolderDepartureAt)))) ? F.I(99999) : F.DatetimeDiff(F.Of(this.HolderDepartureAt), F.Of(this.AsOfInstant), F.S("days")))))); set { }
        }

        // Formula DaysServedToAsOf (rulebook: =IF({{HolderServiceStartedAt}} = "", 0, DATETIME_DIFF({{AsOfInstant}}, {{HolderServiceStartedAt}}, "days")))
        [NotMapped]
        public int? DaysServedToAsOf
        {
            get => F.AsInt(F.Memo(this, "DaysServedToAsOf", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.HolderServiceStartedAt)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.HolderServiceStartedAt), F.S("days")))))); set { }
        }

        // Formula DaysServedToDeparture (rulebook: =IF(OR({{HolderServiceStartedAt}} = "", {{HolderDepartureAt}} = ""), 0, DATETIME_DIFF({{HolderDepartureAt}}, {{HolderServiceStartedAt}}, "days")))
        [NotMapped]
        public int? DaysServedToDeparture
        {
            get => F.AsInt(F.Memo(this, "DaysServedToDeparture", () => F.Integer((F.Truthy(F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.HolderServiceStartedAt))), F.Bool3(F.IsBlank(F.Of(this.HolderDepartureAt)))))) ? F.I(0) : F.DatetimeDiff(F.Of(this.HolderDepartureAt), F.Of(this.HolderServiceStartedAt), F.S("days")))))); set { }
        }

        // Formula HolderTenureYears (rulebook: =ROUND(IF(AND({{HolderDepartureAt}} <> "", {{DaysUntilHolderDeparture}} < 0), {{DaysServedToDeparture}}, {{DaysServedToAsOf}}) / 365, 1))
        [NotMapped]
        public decimal? HolderTenureYears
        {
            get => F.AsDecimal(F.Memo(this, "HolderTenureYears", () => F.Round(F.Div((F.Truthy(F.Bool3(F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderDepartureAt))), F.Bool3(F.Cmp(F.Of(this.DaysUntilHolderDeparture), "<", F.I(0)))))) ? F.Of(this.DaysServedToDeparture) : F.Of(this.DaysServedToAsOf)), F.I(365)), F.I(1)))); set { }
        }

        // Formula IsVeteranHeld (rulebook: =AND({{HolderAgent}} <> "", {{HolderTenureYears}} >= 10))
        [NotMapped]
        public bool? IsVeteranHeld
        {
            get => F.AsBool(F.Memo(this, "IsVeteranHeld", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderAgent))), F.Bool3(F.Cmp(F.Of(this.HolderTenureYears), ">=", F.I(10)))))); set { }
        }

        // Formula IsHolderLeavingSoon (rulebook: =AND({{HolderDepartureAt}} <> "", {{DaysUntilHolderDeparture}} >= 0, {{DaysUntilHolderDeparture}} <= 120))
        [NotMapped]
        public bool? IsHolderLeavingSoon
        {
            get => F.AsBool(F.Memo(this, "IsHolderLeavingSoon", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderDepartureAt))), F.Bool3(F.Cmp(F.Of(this.DaysUntilHolderDeparture), ">=", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DaysUntilHolderDeparture), "<=", F.I(120)))))); set { }
        }

        // Formula TransferCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{KnowHow}}, {{KnowHowCarrierId}}))
        [NotMapped]
        public int? TransferCount
        {
            get => F.AsInt(F.Memo(this, "TransferCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.KnowHow), F.Of(this.KnowHowCarrierId))))))); set { }
        }

        // Formula RepositoryEntryCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{KnowHow}}, {{KnowHowCarrierId}}))
        [NotMapped]
        public int? RepositoryEntryCount
        {
            get => F.AsInt(F.Memo(this, "RepositoryEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.KnowHow), F.Of(this.KnowHowCarrierId))))))); set { }
        }

        // Formula SourceRelationshipCount (rulebook: =COUNTIFS(SourceRelationships!{{SourceAgent}}, {{HolderAgent}}, SourceRelationships!{{Procedure}}, {{Procedure}}))
        [NotMapped]
        public int? SourceRelationshipCount
        {
            get => F.AsInt(F.Memo(this, "SourceRelationshipCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceRelationship>(base.SoAContext, "SourceRelationships", __c => __c.SourceRelationships), __r => F.CritField(F.Of(__r.SourceAgent), F.Of(this.HolderAgent)) && F.CritField(F.Of(__r.Procedure), F.Of(this.Procedure))))))); set { }
        }

        // Formula IsHeldInBothForms (rulebook: =AND({{IsInWrittenProcedure}}, {{IsTrainedSkill}}))
        [NotMapped]
        public bool? IsHeldInBothForms
        {
            get => F.AsBool(F.Memo(this, "IsHeldInBothForms", () => F.And(F.IsTrueV(F.Of(this.IsInWrittenProcedure)), F.IsTrueV(F.Of(this.IsTrainedSkill))))); set { }
        }

        // Formula IsHeldByCurrentPractitioner (rulebook: =AND({{HolderAgent}} <> "", {{HolderIsStillEngaged}} = TRUE))
        [NotMapped]
        public bool? IsHeldByCurrentPractitioner
        {
            get => F.AsBool(F.Memo(this, "IsHeldByCurrentPractitioner", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderAgent))), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(true)))))); set { }
        }

        // Formula IsUntransferredVeteranKnowHow (rulebook: =AND({{IsVeteranHeld}}, {{HolderIsStillEngaged}} = TRUE, {{TransferCount}} = 0, {{RepositoryEntryCount}} = 0))
        [NotMapped]
        public bool? IsUntransferredVeteranKnowHow
        {
            get => F.AsBool(F.Memo(this, "IsUntransferredVeteranKnowHow", () => F.And(F.Bool3(F.Of(this.IsVeteranHeld)), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(true))), F.Bool3(F.Eq(F.Of(this.TransferCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.RepositoryEntryCount), F.I(0)))))); set { }
        }

        // Formula IsAtRiskOfImminentLoss (rulebook: =AND({{IsUntransferredVeteranKnowHow}}, {{IsHolderLeavingSoon}}))
        [NotMapped]
        public bool? IsAtRiskOfImminentLoss
        {
            get => F.AsBool(F.Memo(this, "IsAtRiskOfImminentLoss", () => F.And(F.Bool3(F.Of(this.IsUntransferredVeteranKnowHow)), F.Bool3(F.Of(this.IsHolderLeavingSoon))))); set { }
        }

        // Formula IsHeldOnlyByDeparted (rulebook: =AND({{HolderAgent}} <> "", {{HolderIsStillEngaged}} = FALSE, {{TransferCount}} = 0, {{RepositoryEntryCount}} = 0))
        [NotMapped]
        public bool? IsHeldOnlyByDeparted
        {
            get => F.AsBool(F.Memo(this, "IsHeldOnlyByDeparted", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderAgent))), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(false))), F.Bool3(F.Eq(F.Of(this.TransferCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.RepositoryEntryCount), F.I(0)))))); set { }
        }

        // Formula IsHeldByDepartedHolder (rulebook: =AND({{HolderAgent}} <> "", {{HolderIsStillEngaged}} = FALSE))
        [NotMapped]
        public bool? IsHeldByDepartedHolder
        {
            get => F.AsBool(F.Memo(this, "IsHeldByDepartedHolder", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderAgent))), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(false)))))); set { }
        }

        // Formula IsCaptured (rulebook: ={{RepositoryEntryCount}} > 0)
        [NotMapped]
        public bool? IsCaptured
        {
            get => F.AsBool(F.Memo(this, "IsCaptured", () => F.Cmp(F.Of(this.RepositoryEntryCount), ">", F.I(0)))); set { }
        }

        // Formula MustBeRelearnedIfHolderLeaves (rulebook: =AND({{HolderAgent}} <> "", {{HolderIsStillEngaged}} = TRUE, {{RepositoryEntryCount}} = 0))
        [NotMapped]
        public bool? MustBeRelearnedIfHolderLeaves
        {
            get => F.AsBool(F.Memo(this, "MustBeRelearnedIfHolderLeaves", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderAgent))), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(true))), F.Bool3(F.Eq(F.Of(this.RepositoryEntryCount), F.I(0)))))); set { }
        }

        // Formula IsOverlookedLivingHolder (rulebook: =AND({{HolderAgent}} <> "", {{HolderIsStillEngaged}} = TRUE, {{RepositoryEntryCount}} = 0, {{SourceRelationshipCount}} = 0))
        [NotMapped]
        public bool? IsOverlookedLivingHolder
        {
            get => F.AsBool(F.Memo(this, "IsOverlookedLivingHolder", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.HolderAgent))), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(true))), F.Bool3(F.Eq(F.Of(this.RepositoryEntryCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.SourceRelationshipCount), F.I(0)))))); set { }
        }

        // Formula TransferStopsWithoutVeteran (rulebook: =AND({{IsVeteranHeld}}, {{HolderIsStillEngaged}} = TRUE, {{TransferCount}} > 0, {{RepositoryEntryCount}} = 0))
        [NotMapped]
        public bool? TransferStopsWithoutVeteran
        {
            get => F.AsBool(F.Memo(this, "TransferStopsWithoutVeteran", () => F.And(F.Bool3(F.Of(this.IsVeteranHeld)), F.Bool3(F.Eq(F.Of(this.HolderIsStillEngaged), F.B(true))), F.Bool3(F.Cmp(F.Of(this.TransferCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.RepositoryEntryCount), F.I(0)))))); set { }
        }

        // Formula DependencyCommunity (rulebook: =INDEX(KnowHowCarriers!{{CommunityOfPractice}}, MATCH({{BuildsOnKnowHow}}, KnowHowCarriers!{{KnowHowCarrierId}}, 0)))
        [NotMapped]
        public string? DependencyCommunity
        {
            get => F.AsString(F.Memo(this, "DependencyCommunity", () => F.Lookup<KnowHowCarrier>(this, "KnowHowCarriers", "KnowHowCarrierId", __c => __c.KnowHowCarriers, __r => F.Of(__r.KnowHowCarrierId), F.Of(this.BuildsOnKnowHow), __r => F.Of(__r.CommunityOfPractice), () => F.Of(new KnowHowCarrier().CommunityOfPractice)))); set { }
        }

        // Formula BuildsOnSameCommunityKnowHow (rulebook: =AND({{BuildsOnKnowHow}} <> "", {{CommunityOfPractice}} <> "", {{DependencyCommunity}} = {{CommunityOfPractice}}))
        [NotMapped]
        public bool? BuildsOnSameCommunityKnowHow
        {
            get => F.AsBool(F.Memo(this, "BuildsOnSameCommunityKnowHow", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.BuildsOnKnowHow))), F.Bool3(F.IsNotBlank(F.Of(this.CommunityOfPractice))), F.Bool3(F.Eq(F.Of(this.DependencyCommunity), F.Nullif(F.Of(this.CommunityOfPractice))))))); set { }
        }

        // Formula LostAccumulationYears (rulebook: =IF({{IsHeldOnlyByDeparted}}, {{HolderTenureYears}}, 0))
        [NotMapped]
        public decimal? LostAccumulationYears
        {
            get => F.AsDecimal(F.Memo(this, "LostAccumulationYears", () => (F.Truthy(F.Bool3(F.Of(this.IsHeldOnlyByDeparted))) ? F.Of(this.HolderTenureYears) : F.I(0)))); set { }
        }

        // Formula IsDelegatedToUnfitSource (rulebook: =AND(OR({{ReplacementPlan}} = "PublicReference", {{ReplacementPlan}} = "AiGeneration"), {{IsInPublicReferences}} = FALSE))
        [NotMapped]
        public bool? IsDelegatedToUnfitSource
        {
            get => F.AsBool(F.Memo(this, "IsDelegatedToUnfitSource", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ReplacementPlan)), F.S("PublicReference"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ReplacementPlan)), F.S("AiGeneration"))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.IsInPublicReferences)), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? Procedure { get; set; }
        public string? CommunityOfPractice { get; set; }
        public string? HolderAgent { get; set; }
        public string? HolderFacility { get; set; }
        public string? BuildsOnKnowHow { get; set; }
        public string? EvaluationContext { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
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
                        Organization = _organizationRef.OrganizationId;
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

        private CommunitiesOfPractice _communitiesOfPractice;

        [ForeignKey("CommunityOfPractice")]
        public virtual CommunitiesOfPractice CommunitiesOfPractice
        {
            get
            {
                if (_communitiesOfPractice == null && !string.IsNullOrEmpty(CommunityOfPractice))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. CommunityOfPractice: " + CommunityOfPractice + ".");
                        }
                        return null;
                    }
                    _communitiesOfPractice = base.SoAContext.CommunitiesOfPractice.Find(CommunityOfPractice);
                    if (_communitiesOfPractice != null)
                    {
                        base.SoAContext.Attach(_communitiesOfPractice);
                    }
                }
                return _communitiesOfPractice;
            }
            set
            {
                if (_communitiesOfPractice != value)
                {
                    _communitiesOfPractice = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_communitiesOfPractice != null)
                    {
                        CommunityOfPractice = _communitiesOfPractice.CommunityOfPracticeId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("HolderAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(HolderAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. HolderAgent: " + HolderAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(HolderAgent);
                    if (_agent != null)
                    {
                        base.SoAContext.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agent != null)
                    {
                        HolderAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Facility _facility;

        [ForeignKey("HolderFacility")]
        public virtual Facility Facility
        {
            get
            {
                if (_facility == null && !string.IsNullOrEmpty(HolderFacility))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Facility - no database context is set. HolderFacility: " + HolderFacility + ".");
                        }
                        return null;
                    }
                    _facility = base.SoAContext.Facilities.Find(HolderFacility);
                    if (_facility != null)
                    {
                        base.SoAContext.Attach(_facility);
                    }
                }
                return _facility;
            }
            set
            {
                if (_facility != value)
                {
                    _facility = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_facility != null)
                    {
                        HolderFacility = _facility.FacilityId;
                    }
                }
            }
        }

        private KnowHowCarrier _knowHowCarrier;

        [ForeignKey("BuildsOnKnowHow")]
        public virtual KnowHowCarrier KnowHowCarrier
        {
            get
            {
                if (_knowHowCarrier == null && !string.IsNullOrEmpty(BuildsOnKnowHow))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarrier - no database context is set. BuildsOnKnowHow: " + BuildsOnKnowHow + ".");
                        }
                        return null;
                    }
                    _knowHowCarrier = base.SoAContext.KnowHowCarriers.Find(BuildsOnKnowHow);
                    if (_knowHowCarrier != null)
                    {
                        base.SoAContext.Attach(_knowHowCarrier);
                    }
                }
                return _knowHowCarrier;
            }
            set
            {
                if (_knowHowCarrier != value)
                {
                    _knowHowCarrier = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowHowCarrier != null)
                    {
                        BuildsOnKnowHow = _knowHowCarrier.KnowHowCarrierId;
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

        private ObservableCollection<KnowledgeBrokerLink> _knowledgeBrokerLinks;

        [InverseProperty("KnowHowCarrier")]
        public virtual ObservableCollection<KnowledgeBrokerLink> KnowledgeBrokerLinks
        {
            get
            {
                if (_knowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeBrokerLinks - no database context is set. KnowHowCarrierId: " + this.KnowHowCarrierId + ".");
                        }
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.PointsToKnowHow == this.KnowHowCarrierId).ToList<KnowledgeBrokerLink>();
                        _knowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
                return _knowledgeBrokerLinks;
            }
            private set
            {
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged -= KnowledgeBrokerLinks_CollectionChanged;
                }
                _knowledgeBrokerLinks = value;
                if (_knowledgeBrokerLinks != null)
                {
                    _knowledgeBrokerLinks.CollectionChanged += KnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void KnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.PointsToKnowHow = this.KnowHowCarrierId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("KnowHowCarrier")]
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
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. KnowHowCarrierId: " + this.KnowHowCarrierId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.BuildsOnKnowHow == this.KnowHowCarrierId).ToList<KnowHowCarrier>();
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
                    item.BuildsOnKnowHow = this.KnowHowCarrierId;
                }
            }
        }

        private ObservableCollection<KnowledgeTransfer> _knowledgeTransfers;

        [InverseProperty("KnowHowCarrier")]
        public virtual ObservableCollection<KnowledgeTransfer> KnowledgeTransfers
        {
            get
            {
                if (_knowledgeTransfers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTransfers - no database context is set. KnowHowCarrierId: " + this.KnowHowCarrierId + ".");
                        }
                        _knowledgeTransfers = new ObservableCollection<KnowledgeTransfer>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTransfers.Where(x => x.KnowHow == this.KnowHowCarrierId).ToList<KnowledgeTransfer>();
                        _knowledgeTransfers = new ObservableCollection<KnowledgeTransfer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTransfers.CollectionChanged += KnowledgeTransfers_CollectionChanged;
                }
                return _knowledgeTransfers;
            }
            private set
            {
                if (_knowledgeTransfers != null)
                {
                    _knowledgeTransfers.CollectionChanged -= KnowledgeTransfers_CollectionChanged;
                }
                _knowledgeTransfers = value;
                if (_knowledgeTransfers != null)
                {
                    _knowledgeTransfers.CollectionChanged += KnowledgeTransfers_CollectionChanged;
                }
            }
        }

        private void KnowledgeTransfers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTransfer>())
                {
                    item.KnowHow = this.KnowHowCarrierId;
                }
            }
        }

        private ObservableCollection<KnowledgeRepositoryEntry> _knowledgeRepositoryEntries;

        [InverseProperty("KnowHowCarrier")]
        public virtual ObservableCollection<KnowledgeRepositoryEntry> KnowledgeRepositoryEntries
        {
            get
            {
                if (_knowledgeRepositoryEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeRepositoryEntries - no database context is set. KnowHowCarrierId: " + this.KnowHowCarrierId + ".");
                        }
                        _knowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeRepositoryEntries.Where(x => x.KnowHow == this.KnowHowCarrierId).ToList<KnowledgeRepositoryEntry>();
                        _knowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeRepositoryEntries.CollectionChanged += KnowledgeRepositoryEntries_CollectionChanged;
                }
                return _knowledgeRepositoryEntries;
            }
            private set
            {
                if (_knowledgeRepositoryEntries != null)
                {
                    _knowledgeRepositoryEntries.CollectionChanged -= KnowledgeRepositoryEntries_CollectionChanged;
                }
                _knowledgeRepositoryEntries = value;
                if (_knowledgeRepositoryEntries != null)
                {
                    _knowledgeRepositoryEntries.CollectionChanged += KnowledgeRepositoryEntries_CollectionChanged;
                }
            }
        }

        private void KnowledgeRepositoryEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeRepositoryEntry>())
                {
                    item.KnowHow = this.KnowHowCarrierId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.ProcedureRef;
            _ = this.CommunitiesOfPractice;
            _ = this.Agent;
            _ = this.Facility;
            _ = this.KnowHowCarrier;
            _ = this.EvaluationContextRef;
            _ = this.KnowledgeBrokerLinks;
            _ = this.KnowHowCarriers;
            _ = this.KnowledgeTransfers;
            _ = this.KnowledgeRepositoryEntries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
