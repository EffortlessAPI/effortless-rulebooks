
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
    [Table("Agents")]
    public class AgentBase : SoAEntityBase
    {
        [Key]
        public string AgentId { get; set; }

        // Formula Name (rulebook: ={{DisplayName}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.DisplayName))); set { }
        }

        public string? DisplayName { get; set; }
        public string? AgentKind { get; set; }
        public string? ContactAddress { get; set; }
        public string? VersionOrEmploymentKey { get; set; }
        // Formula CountOfCurrentRoleAssignments (rulebook: =COUNTIFS(RoleAssignments!{{CurrentAgentKey}}, {{AgentId}}))
        [NotMapped]
        public int? CountOfCurrentRoleAssignments
        {
            get => F.AsInt(F.Memo(this, "CountOfCurrentRoleAssignments", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.CurrentAgentKey), F.Of(this.AgentId))))))); set { }
        }

        // Formula IsStillEngaged (rulebook: ={{CountOfCurrentRoleAssignments}} > 0)
        [NotMapped]
        public bool? IsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "IsStillEngaged", () => F.Cmp(F.Of(this.CountOfCurrentRoleAssignments), ">", F.I(0)))); set { }
        }

        // Formula DecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{DecidingAgent}}, {{AgentId}}))
        [NotMapped]
        public decimal? DecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "DecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.DecidingAgent), F.Of(this.AgentId)))))); set { }
        }

        // Formula OverriddenDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{DecidingAgentWhenOverridden}}, {{AgentId}}))
        [NotMapped]
        public decimal? OverriddenDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "OverriddenDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.DecidingAgentWhenOverridden), F.Of(this.AgentId)))))); set { }
        }

        // Formula OverrideRatePercent (rulebook: =IF({{DecisionCount}} = 0, 0, ({{OverriddenDecisionCount}} * 100) / {{DecisionCount}}))
        [NotMapped]
        public decimal? OverrideRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "OverrideRatePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.DecisionCount), F.I(0)))) ? F.I(0) : F.Div(F.Mul(F.Of(this.OverriddenDecisionCount), F.I(100)), F.Of(this.DecisionCount))))); set { }
        }

        // Formula IsNonHuman (rulebook: =NOT({{AgentKind}} = "Human"))
        [NotMapped]
        public bool? IsNonHuman
        {
            get => F.AsBool(F.Memo(this, "IsNonHuman", () => F.Not(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("Human")))))); set { }
        }

        // Formula BoundaryViolationCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenBoundaryViolated}}, {{AgentId}}))
        [NotMapped]
        public decimal? BoundaryViolationCount
        {
            get => F.AsDecimal(F.Memo(this, "BoundaryViolationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.AgentWhenBoundaryViolated), F.Of(this.AgentId)))))); set { }
        }

        // Formula IsOperatingOutsideBoundary (rulebook: ={{BoundaryViolationCount}} > 0)
        [NotMapped]
        public bool? IsOperatingOutsideBoundary
        {
            get => F.AsBool(F.Memo(this, "IsOperatingOutsideBoundary", () => F.Cmp(F.Of(this.BoundaryViolationCount), ">", F.I(0)))); set { }
        }

        // Formula DraftDecisionCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenDraft}}, {{AgentId}}))
        [NotMapped]
        public decimal? DraftDecisionCount
        {
            get => F.AsDecimal(F.Memo(this, "DraftDecisionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.AgentWhenDraft), F.Of(this.AgentId)))))); set { }
        }

        // Formula OverriddenDraftCount (rulebook: =COUNTIFS(AgentDecisionRecords!{{AgentWhenDraftOverridden}}, {{AgentId}}))
        [NotMapped]
        public decimal? OverriddenDraftCount
        {
            get => F.AsDecimal(F.Memo(this, "OverriddenDraftCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentDecisionRecord>(base.SoAContext, "AgentDecisionRecords", __c => __c.AgentDecisionRecords), __r => F.CritField(F.Of(__r.AgentWhenDraftOverridden), F.Of(this.AgentId)))))); set { }
        }

        // Formula DraftRewriteRatePercent (rulebook: =IF({{DraftDecisionCount}} = 0, 0, ({{OverriddenDraftCount}} * 100) / {{DraftDecisionCount}}))
        [NotMapped]
        public decimal? DraftRewriteRatePercent
        {
            get => F.AsDecimal(F.Memo(this, "DraftRewriteRatePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.DraftDecisionCount), F.I(0)))) ? F.I(0) : F.Div(F.Mul(F.Of(this.OverriddenDraftCount), F.I(100)), F.Of(this.DraftDecisionCount))))); set { }
        }

        // Formula TimesNamedAsBroker (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{ActiveRelianceBrokerKey}}, {{AgentId}}))
        [NotMapped]
        public decimal? TimesNamedAsBroker
        {
            get => F.AsDecimal(F.Memo(this, "TimesNamedAsBroker", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeBrokerLink>(base.SoAContext, "KnowledgeBrokerLinks", __c => __c.KnowledgeBrokerLinks), __r => F.CritField(F.Of(__r.ActiveRelianceBrokerKey), F.Of(this.AgentId)))))); set { }
        }

        // Formula IsRecognizedBroker (rulebook: ={{TimesNamedAsBroker}} >= 3)
        [NotMapped]
        public bool? IsRecognizedBroker
        {
            get => F.AsBool(F.Memo(this, "IsRecognizedBroker", () => F.Cmp(F.Of(this.TimesNamedAsBroker), ">=", F.I(3)))); set { }
        }

        // Formula AtRiskRelianceCount (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{AtRiskBrokerKey}}, {{AgentId}}))
        [NotMapped]
        public decimal? AtRiskRelianceCount
        {
            get => F.AsDecimal(F.Memo(this, "AtRiskRelianceCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeBrokerLink>(base.SoAContext, "KnowledgeBrokerLinks", __c => __c.KnowledgeBrokerLinks), __r => F.CritField(F.Of(__r.AtRiskBrokerKey), F.Of(this.AgentId)))))); set { }
        }

        // Formula HasAtRiskKnowledgeReliance (rulebook: ={{AtRiskRelianceCount}} > 0)
        [NotMapped]
        public bool? HasAtRiskKnowledgeReliance
        {
            get => F.AsBool(F.Memo(this, "HasAtRiskKnowledgeReliance", () => F.Cmp(F.Of(this.AtRiskRelianceCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        // Formula IsOrganizationAgent (rulebook: ={{RepresentsOrganization}} <> "")
        [NotMapped]
        public bool? IsOrganizationAgent
        {
            get => F.AsBool(F.Memo(this, "IsOrganizationAgent", () => F.IsNotBlank(F.Of(this.RepresentsOrganization)))); set { }
        }

        // Formula AnswerCount (rulebook: =COUNTIFS(AssistantAnswers!{{AnsweringAgent}}, {{AgentId}}))
        [NotMapped]
        public int? AnswerCount
        {
            get => F.AsInt(F.Memo(this, "AnswerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AssistantAnswer>(base.SoAContext, "AssistantAnswers", __c => __c.AssistantAnswers), __r => F.CritField(F.Of(__r.AnsweringAgent), F.Of(this.AgentId))))))); set { }
        }

        // Formula AiTaskCompletedCount (rulebook: =COUNTIFS(AssistantAnswers!{{AnsweringAgent}}, {{AgentId}}, AssistantAnswers!{{TaskOutcome}}, "Completed"))
        [NotMapped]
        public int? AiTaskCompletedCount
        {
            get => F.AsInt(F.Memo(this, "AiTaskCompletedCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AssistantAnswer>(base.SoAContext, "AssistantAnswers", __c => __c.AssistantAnswers), __r => F.CritField(F.Of(__r.AnsweringAgent), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.TaskOutcome), F.S("Completed"))))))); set { }
        }

        // Formula AiTaskCompletionPercent (rulebook: =IF({{AnswerCount}} = 0, 0, ROUND(100 * {{AiTaskCompletedCount}} / {{AnswerCount}}, 1)))
        [NotMapped]
        public decimal? AiTaskCompletionPercent
        {
            get => F.AsDecimal(F.Memo(this, "AiTaskCompletionPercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.AnswerCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.AiTaskCompletedCount)), F.Of(this.AnswerCount)), F.I(1))))); set { }
        }

        // Formula IsBelowTaskCompletionTarget (rulebook: =AND({{AnswerCount}} > 0, {{AiTaskCompletionPercent}} < 80))
        [NotMapped]
        public bool? IsBelowTaskCompletionTarget
        {
            get => F.AsBool(F.Memo(this, "IsBelowTaskCompletionTarget", () => F.And(F.Bool3(F.Cmp(F.Of(this.AnswerCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.AiTaskCompletionPercent), "<", F.I(80)))))); set { }
        }

        // Formula RuntimeIntegrationCount (rulebook: =COUNTIFS(AgentIntegrations!{{Agent}}, {{AgentId}}, AgentIntegrations!{{DeliveryMode}}, "Runtime"))
        [NotMapped]
        public int? RuntimeIntegrationCount
        {
            get => F.AsInt(F.Memo(this, "RuntimeIntegrationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentIntegration>(base.SoAContext, "AgentIntegrations", __c => __c.AgentIntegrations), __r => F.CritField(F.Of(__r.Agent), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.DeliveryMode), F.S("Runtime"))))))); set { }
        }

        // Formula LacksRuntimeKnowledgeIntegration (rulebook: =AND({{AgentKind}} = "AIAgent", {{AnswerCount}} > 0, {{RuntimeIntegrationCount}} = 0))
        [NotMapped]
        public bool? LacksRuntimeKnowledgeIntegration
        {
            get => F.AsBool(F.Memo(this, "LacksRuntimeKnowledgeIntegration", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent"))), F.Bool3(F.Cmp(F.Of(this.AnswerCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.RuntimeIntegrationCount), F.I(0)))))); set { }
        }

        // Formula SearchEventCount (rulebook: =COUNTIFS(KnowledgeSearchEvents!{{SearchedByAgent}}, {{AgentId}}))
        [NotMapped]
        public int? SearchEventCount
        {
            get => F.AsInt(F.Memo(this, "SearchEventCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeSearchEvent>(base.SoAContext, "KnowledgeSearchEvents", __c => __c.KnowledgeSearchEvents), __r => F.CritField(F.Of(__r.SearchedByAgent), F.Of(this.AgentId))))))); set { }
        }

        // Formula IsUntrackedAiConsumer (rulebook: =AND({{AgentKind}} = "AIAgent", {{AnswerCount}} > 0, {{SearchEventCount}} = 0))
        [NotMapped]
        public bool? IsUntrackedAiConsumer
        {
            get => F.AsBool(F.Memo(this, "IsUntrackedAiConsumer", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent"))), F.Bool3(F.Cmp(F.Of(this.AnswerCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.SearchEventCount), F.I(0)))))); set { }
        }

        // Formula AccountabilityAssertionCount (rulebook: =COUNTIFS(SnapshotAssertions!{{AboutAgent}}, {{AgentId}}, SnapshotAssertions!{{IsInferred}}, FALSE))
        [NotMapped]
        public int? AccountabilityAssertionCount
        {
            get => F.AsInt(F.Memo(this, "AccountabilityAssertionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.AboutAgent), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.IsInferred), F.B(false))))))); set { }
        }

        // Formula InferredCategoryCount (rulebook: =COUNTIFS(SnapshotAssertions!{{AboutAgent}}, {{AgentId}}, SnapshotAssertions!{{IsInferred}}, TRUE))
        [NotMapped]
        public int? InferredCategoryCount
        {
            get => F.AsInt(F.Memo(this, "InferredCategoryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SnapshotAssertion>(base.SoAContext, "SnapshotAssertions", __c => __c.SnapshotAssertions), __r => F.CritField(F.Of(__r.AboutAgent), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.IsInferred), F.B(true))))))); set { }
        }

        // Formula CategoryNotAvailableAsInference (rulebook: =AND({{AccountabilityAssertionCount}} > 0, {{InferredCategoryCount}} = 0))
        [NotMapped]
        public bool? CategoryNotAvailableAsInference
        {
            get => F.AsBool(F.Memo(this, "CategoryNotAvailableAsInference", () => F.And(F.Bool3(F.Cmp(F.Of(this.AccountabilityAssertionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.InferredCategoryCount), F.I(0)))))); set { }
        }

        // Formula IsUnclassifiedAgent (rulebook: =AND({{AgentKind}} <> "Human", {{AgentKind}} <> "AIAgent", {{AgentKind}} <> "AutomatedPipeline", {{AgentKind}} <> "Organization"))
        [NotMapped]
        public bool? IsUnclassifiedAgent
        {
            get => F.AsBool(F.Memo(this, "IsUnclassifiedAgent", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.AgentKind)), F.S("Human"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.AgentKind)), F.S("AutomatedPipeline"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.AgentKind)), F.S("Organization")))))); set { }
        }

        // Formula IsAiAgentWithoutModelVersion (rulebook: =AND({{AgentKind}} = "AIAgent", {{VersionOrEmploymentKey}} = ""))
        [NotMapped]
        public bool? IsAiAgentWithoutModelVersion
        {
            get => F.AsBool(F.Memo(this, "IsAiAgentWithoutModelVersion", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent"))), F.Bool3(F.IsBlank(F.Of(this.VersionOrEmploymentKey)))))); set { }
        }

        // Formula AttributedArtifactCount (rulebook: =COUNTIFS(ExecutionEntities!{{AttributedToAgent}}, {{AgentId}}))
        [NotMapped]
        public int? AttributedArtifactCount
        {
            get => F.AsInt(F.Memo(this, "AttributedArtifactCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ExecutionEntity>(base.SoAContext, "ExecutionEntities", __c => __c.ExecutionEntities), __r => F.CritField(F.Of(__r.AttributedToAgent), F.Of(this.AgentId))))))); set { }
        }

        // Formula HasProducedArtifacts (rulebook: ={{AttributedArtifactCount}} > 0)
        [NotMapped]
        public bool? HasProducedArtifacts
        {
            get => F.AsBool(F.Memo(this, "HasProducedArtifacts", () => F.Cmp(F.Of(this.AttributedArtifactCount), ">", F.I(0)))); set { }
        }

        // Formula HasArtifactBlastRadius (rulebook: ={{ArtifactBlastRadiusStepCount}} > 0)
        [NotMapped]
        public bool? HasArtifactBlastRadius
        {
            get => F.AsBool(F.Memo(this, "HasArtifactBlastRadius", () => F.Cmp(F.Of(this.ArtifactBlastRadiusStepCount), ">", F.I(0)))); set { }
        }

        // Formula RegistryVersionMatchCount (rulebook: =COUNTIFS(AiRegistryModelVersions!{{DcIdentifier}}, {{AgentId}}, AiRegistryModelVersions!{{DcHasVersion}}, {{VersionOrEmploymentKey}}))
        [NotMapped]
        public int? RegistryVersionMatchCount
        {
            get => F.AsInt(F.Memo(this, "RegistryVersionMatchCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiRegistryModelVersion>(base.SoAContext, "AiRegistryModelVersions", __c => __c.AiRegistryModelVersions), __r => F.CritField(F.Of(__r.DcIdentifier), F.Of(this.AgentId)) && F.CritField(F.Of(__r.DcHasVersion), F.Of(this.VersionOrEmploymentKey))))))); set { }
        }

        // Formula IsAiAgentNotFilledFromRegistry (rulebook: =AND({{AgentKind}} = "AIAgent", {{RegistryVersionMatchCount}} = 0))
        [NotMapped]
        public bool? IsAiAgentNotFilledFromRegistry
        {
            get => F.AsBool(F.Memo(this, "IsAiAgentNotFilledFromRegistry", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent"))), F.Bool3(F.Eq(F.Of(this.RegistryVersionMatchCount), F.I(0)))))); set { }
        }

        // Formula CurrentAccountableHumanCount (rulebook: =COUNTIFS(AiAgentAccountabilities!{{AiAgent}}, {{AgentId}}, AiAgentAccountabilities!{{IsCurrentHumanAccountability}}, TRUE))
        [NotMapped]
        public int? CurrentAccountableHumanCount
        {
            get => F.AsInt(F.Memo(this, "CurrentAccountableHumanCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiAgentAccountability>(base.SoAContext, "AiAgentAccountabilities", __c => __c.AiAgentAccountabilities), __r => F.CritField(F.Of(__r.AiAgent), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.IsCurrentHumanAccountability), F.B(true))))))); set { }
        }

        // Formula IsAiAgentWithoutAccountableHuman (rulebook: =AND({{AgentKind}} = "AIAgent", {{CountOfCurrentRoleAssignments}} > 0, {{CurrentAccountableHumanCount}} = 0))
        [NotMapped]
        public bool? IsAiAgentWithoutAccountableHuman
        {
            get => F.AsBool(F.Memo(this, "IsAiAgentWithoutAccountableHuman", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent"))), F.Bool3(F.Cmp(F.Of(this.CountOfCurrentRoleAssignments), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CurrentAccountableHumanCount), F.I(0)))))); set { }
        }

        public DateTimeOffset? ServiceStartedAt { get; set; }
        public DateTimeOffset? DepartureAt { get; set; }
        public decimal? ProtectedMentoringHoursPerWeek { get; set; }
        // Formula CommunityCount (rulebook: =COUNTIFS(CommunityMemberships!{{Agent}}, {{AgentId}}))
        [NotMapped]
        public int? CommunityCount
        {
            get => F.AsInt(F.Memo(this, "CommunityCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CommunityMembership>(base.SoAContext, "CommunityMemberships", __c => __c.CommunityMemberships), __r => F.CritField(F.Of(__r.Agent), F.Of(this.AgentId))))))); set { }
        }

        // Formula IsBoundarySpanner (rulebook: ={{CommunityCount}} >= 2)
        [NotMapped]
        public bool? IsBoundarySpanner
        {
            get => F.AsBool(F.Memo(this, "IsBoundarySpanner", () => F.Cmp(F.Of(this.CommunityCount), ">=", F.I(2)))); set { }
        }

        // Formula SnaIdentificationCount (rulebook: =COUNTIFS(MethodApplications!{{IdentifiedBroker}}, {{AgentId}}))
        [NotMapped]
        public int? SnaIdentificationCount
        {
            get => F.AsInt(F.Memo(this, "SnaIdentificationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<MethodApplication>(base.SoAContext, "MethodApplications", __c => __c.MethodApplications), __r => F.CritField(F.Of(__r.IdentifiedBroker), F.Of(this.AgentId))))))); set { }
        }

        // Formula IsUnidentifiedBoundarySpanner (rulebook: =AND({{IsBoundarySpanner}}, {{SnaIdentificationCount}} = 0))
        [NotMapped]
        public bool? IsUnidentifiedBoundarySpanner
        {
            get => F.AsBool(F.Memo(this, "IsUnidentifiedBoundarySpanner", () => F.And(F.Bool3(F.Of(this.IsBoundarySpanner)), F.Bool3(F.Eq(F.Of(this.SnaIdentificationCount), F.I(0)))))); set { }
        }

        // Formula LocatedKnowHowCount (rulebook: =COUNTIFS(KnowledgeBrokerLinks!{{Broker}}, {{AgentId}}, KnowledgeBrokerLinks!{{LocatesOtherHolder}}, TRUE))
        [NotMapped]
        public int? LocatedKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "LocatedKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeBrokerLink>(base.SoAContext, "KnowledgeBrokerLinks", __c => __c.KnowledgeBrokerLinks), __r => F.CritField(F.Of(__r.Broker), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.LocatesOtherHolder), F.B(true))))))); set { }
        }

        // Formula RequiredMentoringHoursPerWeek (rulebook: =SUMIFS(Mentorships!{{ExpectedWeeklyHours}}, Mentorships!{{MentorAgent}}, {{AgentId}}, Mentorships!{{IsActive}}, TRUE))
        [NotMapped]
        public decimal? RequiredMentoringHoursPerWeek
        {
            get => F.AsDecimal(F.Memo(this, "RequiredMentoringHoursPerWeek", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<Mentorship>(base.SoAContext, "Mentorships", __c => __c.Mentorships), __r => F.CritField(F.Of(__r.MentorAgent), F.Of(this.AgentId)) && F.CritLiteral(F.Of(__r.IsActive), F.B(true)), __r => F.Of(__r.ExpectedWeeklyHours), null)))); set { }
        }

        // Formula LacksTimeToMentor (rulebook: =AND({{RequiredMentoringHoursPerWeek}} > 0, {{ProtectedMentoringHoursPerWeek}} < {{RequiredMentoringHoursPerWeek}}))
        [NotMapped]
        public bool? LacksTimeToMentor
        {
            get => F.AsBool(F.Memo(this, "LacksTimeToMentor", () => F.And(F.Bool3(F.Cmp(F.Of(this.RequiredMentoringHoursPerWeek), ">", F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProtectedMentoringHoursPerWeek)), "<", F.Of(this.RequiredMentoringHoursPerWeek)))))); set { }
        }

        // Formula TransfersGivenCount (rulebook: =COUNTIFS(KnowledgeTransfers!{{FromAgent}}, {{AgentId}}))
        [NotMapped]
        public int? TransfersGivenCount
        {
            get => F.AsInt(F.Memo(this, "TransfersGivenCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTransfer>(base.SoAContext, "KnowledgeTransfers", __c => __c.KnowledgeTransfers), __r => F.CritField(F.Of(__r.FromAgent), F.Of(this.AgentId))))))); set { }
        }

        // Formula RecognitionCount (rulebook: =COUNTIFS(SharingRecognitions!{{RecognizedAgent}}, {{AgentId}}))
        [NotMapped]
        public int? RecognitionCount
        {
            get => F.AsInt(F.Memo(this, "RecognitionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SharingRecognition>(base.SoAContext, "SharingRecognitions", __c => __c.SharingRecognitions), __r => F.CritField(F.Of(__r.RecognizedAgent), F.Of(this.AgentId))))))); set { }
        }

        // Formula IsUnrewardedSharer (rulebook: =AND({{TransfersGivenCount}} >= 2, {{RecognitionCount}} = 0))
        [NotMapped]
        public bool? IsUnrewardedSharer
        {
            get => F.AsBool(F.Memo(this, "IsUnrewardedSharer", () => F.And(F.Bool3(F.Cmp(F.Of(this.TransfersGivenCount), ">=", F.I(2))), F.Bool3(F.Eq(F.Of(this.RecognitionCount), F.I(0)))))); set { }
        }

        // Formula DownstreamOfHeldStepsCount (rulebook: =SUMIFS(Steps!{{DownstreamArtifactStepCount}}, Steps!{{AccountableAgent}}, {{AgentId}}))
        [NotMapped]
        public int? DownstreamOfHeldStepsCount
        {
            get => F.AsInt(F.Memo(this, "DownstreamOfHeldStepsCount", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.AccountableAgent), F.Of(this.AgentId)), __r => F.Of(__r.DownstreamArtifactStepCount), null))))); set { }
        }

        // Formula ArtifactBlastRadiusStepCount (rulebook: =IF({{AgentKind}} = "AIAgent", {{DownstreamOfHeldStepsCount}}, 0))
        [NotMapped]
        public int? ArtifactBlastRadiusStepCount
        {
            get => F.AsInt(F.Memo(this, "ArtifactBlastRadiusStepCount", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.AgentKind)), F.S("AIAgent")))) ? F.Of(this.DownstreamOfHeldStepsCount) : F.I(0))))); set { }
        }


        public string? Organization { get; set; }
        public string? RepresentsOrganization { get; set; }

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

        private Organization _organizationRefRef;

        [ForeignKey("RepresentsOrganization")]
        public virtual Organization OrganizationRefRef
        {
            get
            {
                if (_organizationRefRef == null && !string.IsNullOrEmpty(RepresentsOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRefRef - no database context is set. RepresentsOrganization: " + RepresentsOrganization + ".");
                        }
                        return null;
                    }
                    _organizationRefRef = base.SoAContext.Organizations.Find(RepresentsOrganization);
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
                        RepresentsOrganization = _organizationRefRef.OrganizationId;
                    }
                }
            }
        }

        private ObservableCollection<RulebookRelease> _versionDecidedByAgentRulebookReleases;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<RulebookRelease> VersionDecidedByAgentRulebookReleases
        {
            get
            {
                if (_versionDecidedByAgentRulebookReleases == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VersionDecidedByAgentRulebookReleases - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _versionDecidedByAgentRulebookReleases = new ObservableCollection<RulebookRelease>();
                    }
                    else
                    {
                        var items = base.SoAContext.RulebookReleases.Where(x => x.VersionDecidedByAgent == this.AgentId).ToList<RulebookRelease>();
                        _versionDecidedByAgentRulebookReleases = new ObservableCollection<RulebookRelease>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _versionDecidedByAgentRulebookReleases.CollectionChanged += VersionDecidedByAgentRulebookReleases_CollectionChanged;
                }
                return _versionDecidedByAgentRulebookReleases;
            }
            private set
            {
                if (_versionDecidedByAgentRulebookReleases != null)
                {
                    _versionDecidedByAgentRulebookReleases.CollectionChanged -= VersionDecidedByAgentRulebookReleases_CollectionChanged;
                }
                _versionDecidedByAgentRulebookReleases = value;
                if (_versionDecidedByAgentRulebookReleases != null)
                {
                    _versionDecidedByAgentRulebookReleases.CollectionChanged += VersionDecidedByAgentRulebookReleases_CollectionChanged;
                }
            }
        }

        private void VersionDecidedByAgentRulebookReleases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RulebookRelease>())
                {
                    item.VersionDecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RulebookRelease> _approvedByAgentRulebookReleases;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<RulebookRelease> ApprovedByAgentRulebookReleases
        {
            get
            {
                if (_approvedByAgentRulebookReleases == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovedByAgentRulebookReleases - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _approvedByAgentRulebookReleases = new ObservableCollection<RulebookRelease>();
                    }
                    else
                    {
                        var items = base.SoAContext.RulebookReleases.Where(x => x.ApprovedByAgent == this.AgentId).ToList<RulebookRelease>();
                        _approvedByAgentRulebookReleases = new ObservableCollection<RulebookRelease>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvedByAgentRulebookReleases.CollectionChanged += ApprovedByAgentRulebookReleases_CollectionChanged;
                }
                return _approvedByAgentRulebookReleases;
            }
            private set
            {
                if (_approvedByAgentRulebookReleases != null)
                {
                    _approvedByAgentRulebookReleases.CollectionChanged -= ApprovedByAgentRulebookReleases_CollectionChanged;
                }
                _approvedByAgentRulebookReleases = value;
                if (_approvedByAgentRulebookReleases != null)
                {
                    _approvedByAgentRulebookReleases.CollectionChanged += ApprovedByAgentRulebookReleases_CollectionChanged;
                }
            }
        }

        private void ApprovedByAgentRulebookReleases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RulebookRelease>())
                {
                    item.ApprovedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Role> _roles;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access Roles - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roles = new ObservableCollection<Role>();
                    }
                    else
                    {
                        var items = base.SoAContext.Roles.Where(x => x.CurrentAgent == this.AgentId).ToList<Role>();
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
                    item.CurrentAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RoleAssignment> _roleAssignments;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<RoleAssignment> RoleAssignments
        {
            get
            {
                if (_roleAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roleAssignments = new ObservableCollection<RoleAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignments.Where(x => x.Agent == this.AgentId).ToList<RoleAssignment>();
                        _roleAssignments = new ObservableCollection<RoleAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleAssignments.CollectionChanged += RoleAssignments_CollectionChanged;
                }
                return _roleAssignments;
            }
            private set
            {
                if (_roleAssignments != null)
                {
                    _roleAssignments.CollectionChanged -= RoleAssignments_CollectionChanged;
                }
                _roleAssignments = value;
                if (_roleAssignments != null)
                {
                    _roleAssignments.CollectionChanged += RoleAssignments_CollectionChanged;
                }
            }
        }

        private void RoleAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignment>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorAgentMentorships;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Mentorship> MentorAgentMentorships
        {
            get
            {
                if (_mentorAgentMentorships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MentorAgentMentorships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _mentorAgentMentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = base.SoAContext.Mentorships.Where(x => x.MentorAgent == this.AgentId).ToList<Mentorship>();
                        _mentorAgentMentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _mentorAgentMentorships.CollectionChanged += MentorAgentMentorships_CollectionChanged;
                }
                return _mentorAgentMentorships;
            }
            private set
            {
                if (_mentorAgentMentorships != null)
                {
                    _mentorAgentMentorships.CollectionChanged -= MentorAgentMentorships_CollectionChanged;
                }
                _mentorAgentMentorships = value;
                if (_mentorAgentMentorships != null)
                {
                    _mentorAgentMentorships.CollectionChanged += MentorAgentMentorships_CollectionChanged;
                }
            }
        }

        private void MentorAgentMentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.MentorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Mentorship> _learnerAgentMentorships;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<Mentorship> LearnerAgentMentorships
        {
            get
            {
                if (_learnerAgentMentorships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearnerAgentMentorships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _learnerAgentMentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = base.SoAContext.Mentorships.Where(x => x.LearnerAgent == this.AgentId).ToList<Mentorship>();
                        _learnerAgentMentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _learnerAgentMentorships.CollectionChanged += LearnerAgentMentorships_CollectionChanged;
                }
                return _learnerAgentMentorships;
            }
            private set
            {
                if (_learnerAgentMentorships != null)
                {
                    _learnerAgentMentorships.CollectionChanged -= LearnerAgentMentorships_CollectionChanged;
                }
                _learnerAgentMentorships = value;
                if (_learnerAgentMentorships != null)
                {
                    _learnerAgentMentorships.CollectionChanged += LearnerAgentMentorships_CollectionChanged;
                }
            }
        }

        private void LearnerAgentMentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.LearnerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _createdByAgentProcedureVersions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureVersion> CreatedByAgentProcedureVersions
        {
            get
            {
                if (_createdByAgentProcedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CreatedByAgentProcedureVersions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _createdByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.CreatedByAgent == this.AgentId).ToList<ProcedureVersion>();
                        _createdByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _createdByAgentProcedureVersions.CollectionChanged += CreatedByAgentProcedureVersions_CollectionChanged;
                }
                return _createdByAgentProcedureVersions;
            }
            private set
            {
                if (_createdByAgentProcedureVersions != null)
                {
                    _createdByAgentProcedureVersions.CollectionChanged -= CreatedByAgentProcedureVersions_CollectionChanged;
                }
                _createdByAgentProcedureVersions = value;
                if (_createdByAgentProcedureVersions != null)
                {
                    _createdByAgentProcedureVersions.CollectionChanged += CreatedByAgentProcedureVersions_CollectionChanged;
                }
            }
        }

        private void CreatedByAgentProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.CreatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _modifiedByAgentProcedureVersions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ProcedureVersion> ModifiedByAgentProcedureVersions
        {
            get
            {
                if (_modifiedByAgentProcedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModifiedByAgentProcedureVersions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _modifiedByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.ModifiedByAgent == this.AgentId).ToList<ProcedureVersion>();
                        _modifiedByAgentProcedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modifiedByAgentProcedureVersions.CollectionChanged += ModifiedByAgentProcedureVersions_CollectionChanged;
                }
                return _modifiedByAgentProcedureVersions;
            }
            private set
            {
                if (_modifiedByAgentProcedureVersions != null)
                {
                    _modifiedByAgentProcedureVersions.CollectionChanged -= ModifiedByAgentProcedureVersions_CollectionChanged;
                }
                _modifiedByAgentProcedureVersions = value;
                if (_modifiedByAgentProcedureVersions != null)
                {
                    _modifiedByAgentProcedureVersions.CollectionChanged += ModifiedByAgentProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ModifiedByAgentProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.ModifiedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureStatusChange> _procedureStatusChanges;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureStatusChange> ProcedureStatusChanges
        {
            get
            {
                if (_procedureStatusChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureStatusChanges - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureStatusChanges.Where(x => x.ChangedByAgent == this.AgentId).ToList<ProcedureStatusChange>();
                        _procedureStatusChanges = new ObservableCollection<ProcedureStatusChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureStatusChanges.CollectionChanged += ProcedureStatusChanges_CollectionChanged;
                }
                return _procedureStatusChanges;
            }
            private set
            {
                if (_procedureStatusChanges != null)
                {
                    _procedureStatusChanges.CollectionChanged -= ProcedureStatusChanges_CollectionChanged;
                }
                _procedureStatusChanges = value;
                if (_procedureStatusChanges != null)
                {
                    _procedureStatusChanges.CollectionChanged += ProcedureStatusChanges_CollectionChanged;
                }
            }
        }

        private void ProcedureStatusChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureStatusChange>())
                {
                    item.ChangedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Resource> _createdByAgentResources;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Resource> CreatedByAgentResources
        {
            get
            {
                if (_createdByAgentResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CreatedByAgentResources - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _createdByAgentResources = new ObservableCollection<Resource>();
                    }
                    else
                    {
                        var items = base.SoAContext.Resources.Where(x => x.CreatedByAgent == this.AgentId).ToList<Resource>();
                        _createdByAgentResources = new ObservableCollection<Resource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _createdByAgentResources.CollectionChanged += CreatedByAgentResources_CollectionChanged;
                }
                return _createdByAgentResources;
            }
            private set
            {
                if (_createdByAgentResources != null)
                {
                    _createdByAgentResources.CollectionChanged -= CreatedByAgentResources_CollectionChanged;
                }
                _createdByAgentResources = value;
                if (_createdByAgentResources != null)
                {
                    _createdByAgentResources.CollectionChanged += CreatedByAgentResources_CollectionChanged;
                }
            }
        }

        private void CreatedByAgentResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Resource>())
                {
                    item.CreatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Resource> _modifiedByAgentResources;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<Resource> ModifiedByAgentResources
        {
            get
            {
                if (_modifiedByAgentResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModifiedByAgentResources - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _modifiedByAgentResources = new ObservableCollection<Resource>();
                    }
                    else
                    {
                        var items = base.SoAContext.Resources.Where(x => x.ModifiedByAgent == this.AgentId).ToList<Resource>();
                        _modifiedByAgentResources = new ObservableCollection<Resource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modifiedByAgentResources.CollectionChanged += ModifiedByAgentResources_CollectionChanged;
                }
                return _modifiedByAgentResources;
            }
            private set
            {
                if (_modifiedByAgentResources != null)
                {
                    _modifiedByAgentResources.CollectionChanged -= ModifiedByAgentResources_CollectionChanged;
                }
                _modifiedByAgentResources = value;
                if (_modifiedByAgentResources != null)
                {
                    _modifiedByAgentResources.CollectionChanged += ModifiedByAgentResources_CollectionChanged;
                }
            }
        }

        private void ModifiedByAgentResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Resource>())
                {
                    item.ModifiedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _practitionerAgentElicitationSessions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ElicitationSession> PractitionerAgentElicitationSessions
        {
            get
            {
                if (_practitionerAgentElicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PractitionerAgentElicitationSessions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _practitionerAgentElicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.PractitionerAgent == this.AgentId).ToList<ElicitationSession>();
                        _practitionerAgentElicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _practitionerAgentElicitationSessions.CollectionChanged += PractitionerAgentElicitationSessions_CollectionChanged;
                }
                return _practitionerAgentElicitationSessions;
            }
            private set
            {
                if (_practitionerAgentElicitationSessions != null)
                {
                    _practitionerAgentElicitationSessions.CollectionChanged -= PractitionerAgentElicitationSessions_CollectionChanged;
                }
                _practitionerAgentElicitationSessions = value;
                if (_practitionerAgentElicitationSessions != null)
                {
                    _practitionerAgentElicitationSessions.CollectionChanged += PractitionerAgentElicitationSessions_CollectionChanged;
                }
            }
        }

        private void PractitionerAgentElicitationSessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationSession>())
                {
                    item.PractitionerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationSession> _facilitatorAgentElicitationSessions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ElicitationSession> FacilitatorAgentElicitationSessions
        {
            get
            {
                if (_facilitatorAgentElicitationSessions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FacilitatorAgentElicitationSessions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _facilitatorAgentElicitationSessions = new ObservableCollection<ElicitationSession>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationSessions.Where(x => x.FacilitatorAgent == this.AgentId).ToList<ElicitationSession>();
                        _facilitatorAgentElicitationSessions = new ObservableCollection<ElicitationSession>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _facilitatorAgentElicitationSessions.CollectionChanged += FacilitatorAgentElicitationSessions_CollectionChanged;
                }
                return _facilitatorAgentElicitationSessions;
            }
            private set
            {
                if (_facilitatorAgentElicitationSessions != null)
                {
                    _facilitatorAgentElicitationSessions.CollectionChanged -= FacilitatorAgentElicitationSessions_CollectionChanged;
                }
                _facilitatorAgentElicitationSessions = value;
                if (_facilitatorAgentElicitationSessions != null)
                {
                    _facilitatorAgentElicitationSessions.CollectionChanged += FacilitatorAgentElicitationSessions_CollectionChanged;
                }
            }
        }

        private void FacilitatorAgentElicitationSessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationSession>())
                {
                    item.FacilitatorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.SourceAgent == this.AgentId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
                return _knowledgeFragments;
            }
            private set
            {
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged -= KnowledgeFragments_CollectionChanged;
                }
                _knowledgeFragments = value;
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
            }
        }

        private void KnowledgeFragments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeFragment>())
                {
                    item.SourceAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _executedByAgentProcedureExecutions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcedureExecution> ExecutedByAgentProcedureExecutions
        {
            get
            {
                if (_executedByAgentProcedureExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExecutedByAgentProcedureExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _executedByAgentProcedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.ExecutedByAgent == this.AgentId).ToList<ProcedureExecution>();
                        _executedByAgentProcedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _executedByAgentProcedureExecutions.CollectionChanged += ExecutedByAgentProcedureExecutions_CollectionChanged;
                }
                return _executedByAgentProcedureExecutions;
            }
            private set
            {
                if (_executedByAgentProcedureExecutions != null)
                {
                    _executedByAgentProcedureExecutions.CollectionChanged -= ExecutedByAgentProcedureExecutions_CollectionChanged;
                }
                _executedByAgentProcedureExecutions = value;
                if (_executedByAgentProcedureExecutions != null)
                {
                    _executedByAgentProcedureExecutions.CollectionChanged += ExecutedByAgentProcedureExecutions_CollectionChanged;
                }
            }
        }

        private void ExecutedByAgentProcedureExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureExecution>())
                {
                    item.ExecutedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcedureExecution> _confirmedByAgentProcedureExecutions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ProcedureExecution> ConfirmedByAgentProcedureExecutions
        {
            get
            {
                if (_confirmedByAgentProcedureExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConfirmedByAgentProcedureExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _confirmedByAgentProcedureExecutions = new ObservableCollection<ProcedureExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureExecutions.Where(x => x.ConfirmedByAgent == this.AgentId).ToList<ProcedureExecution>();
                        _confirmedByAgentProcedureExecutions = new ObservableCollection<ProcedureExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _confirmedByAgentProcedureExecutions.CollectionChanged += ConfirmedByAgentProcedureExecutions_CollectionChanged;
                }
                return _confirmedByAgentProcedureExecutions;
            }
            private set
            {
                if (_confirmedByAgentProcedureExecutions != null)
                {
                    _confirmedByAgentProcedureExecutions.CollectionChanged -= ConfirmedByAgentProcedureExecutions_CollectionChanged;
                }
                _confirmedByAgentProcedureExecutions = value;
                if (_confirmedByAgentProcedureExecutions != null)
                {
                    _confirmedByAgentProcedureExecutions.CollectionChanged += ConfirmedByAgentProcedureExecutions_CollectionChanged;
                }
            }
        }

        private void ConfirmedByAgentProcedureExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureExecution>())
                {
                    item.ConfirmedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StepExecution> _executedByAgentStepExecutions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<StepExecution> ExecutedByAgentStepExecutions
        {
            get
            {
                if (_executedByAgentStepExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExecutedByAgentStepExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _executedByAgentStepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepExecutions.Where(x => x.ExecutedByAgent == this.AgentId).ToList<StepExecution>();
                        _executedByAgentStepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _executedByAgentStepExecutions.CollectionChanged += ExecutedByAgentStepExecutions_CollectionChanged;
                }
                return _executedByAgentStepExecutions;
            }
            private set
            {
                if (_executedByAgentStepExecutions != null)
                {
                    _executedByAgentStepExecutions.CollectionChanged -= ExecutedByAgentStepExecutions_CollectionChanged;
                }
                _executedByAgentStepExecutions = value;
                if (_executedByAgentStepExecutions != null)
                {
                    _executedByAgentStepExecutions.CollectionChanged += ExecutedByAgentStepExecutions_CollectionChanged;
                }
            }
        }

        private void ExecutedByAgentStepExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepExecution>())
                {
                    item.ExecutedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StepExecution> _confirmedByAgentStepExecutions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<StepExecution> ConfirmedByAgentStepExecutions
        {
            get
            {
                if (_confirmedByAgentStepExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConfirmedByAgentStepExecutions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _confirmedByAgentStepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepExecutions.Where(x => x.ConfirmedByAgent == this.AgentId).ToList<StepExecution>();
                        _confirmedByAgentStepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _confirmedByAgentStepExecutions.CollectionChanged += ConfirmedByAgentStepExecutions_CollectionChanged;
                }
                return _confirmedByAgentStepExecutions;
            }
            private set
            {
                if (_confirmedByAgentStepExecutions != null)
                {
                    _confirmedByAgentStepExecutions.CollectionChanged -= ConfirmedByAgentStepExecutions_CollectionChanged;
                }
                _confirmedByAgentStepExecutions = value;
                if (_confirmedByAgentStepExecutions != null)
                {
                    _confirmedByAgentStepExecutions.CollectionChanged += ConfirmedByAgentStepExecutions_CollectionChanged;
                }
            }
        }

        private void ConfirmedByAgentStepExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepExecution>())
                {
                    item.ConfirmedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RequirementSatisfaction> _requirementSatisfactions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<RequirementSatisfaction> RequirementSatisfactions
        {
            get
            {
                if (_requirementSatisfactions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementSatisfactions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>();
                    }
                    else
                    {
                        var items = base.SoAContext.RequirementSatisfactions.Where(x => x.EvaluatedByAgent == this.AgentId).ToList<RequirementSatisfaction>();
                        _requirementSatisfactions = new ObservableCollection<RequirementSatisfaction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requirementSatisfactions.CollectionChanged += RequirementSatisfactions_CollectionChanged;
                }
                return _requirementSatisfactions;
            }
            private set
            {
                if (_requirementSatisfactions != null)
                {
                    _requirementSatisfactions.CollectionChanged -= RequirementSatisfactions_CollectionChanged;
                }
                _requirementSatisfactions = value;
                if (_requirementSatisfactions != null)
                {
                    _requirementSatisfactions.CollectionChanged += RequirementSatisfactions_CollectionChanged;
                }
            }
        }

        private void RequirementSatisfactions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RequirementSatisfaction>())
                {
                    item.EvaluatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<IssueOccurrence> _issueOccurrences;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<IssueOccurrence> IssueOccurrences
        {
            get
            {
                if (_issueOccurrences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IssueOccurrences - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.IssueOccurrences.Where(x => x.EncounteredByAgent == this.AgentId).ToList<IssueOccurrence>();
                        _issueOccurrences = new ObservableCollection<IssueOccurrence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _issueOccurrences.CollectionChanged += IssueOccurrences_CollectionChanged;
                }
                return _issueOccurrences;
            }
            private set
            {
                if (_issueOccurrences != null)
                {
                    _issueOccurrences.CollectionChanged -= IssueOccurrences_CollectionChanged;
                }
                _issueOccurrences = value;
                if (_issueOccurrences != null)
                {
                    _issueOccurrences.CollectionChanged += IssueOccurrences_CollectionChanged;
                }
            }
        }

        private void IssueOccurrences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<IssueOccurrence>())
                {
                    item.EncounteredByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<UserQuestion> _userQuestions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<UserQuestion> UserQuestions
        {
            get
            {
                if (_userQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserQuestions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _userQuestions = new ObservableCollection<UserQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserQuestions.Where(x => x.AskedByAgent == this.AgentId).ToList<UserQuestion>();
                        _userQuestions = new ObservableCollection<UserQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _userQuestions.CollectionChanged += UserQuestions_CollectionChanged;
                }
                return _userQuestions;
            }
            private set
            {
                if (_userQuestions != null)
                {
                    _userQuestions.CollectionChanged -= UserQuestions_CollectionChanged;
                }
                _userQuestions = value;
                if (_userQuestions != null)
                {
                    _userQuestions.CollectionChanged += UserQuestions_CollectionChanged;
                }
            }
        }

        private void UserQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<UserQuestion>())
                {
                    item.AskedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<UserFeedback> _userFeedback;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<UserFeedback> UserFeedback
        {
            get
            {
                if (_userFeedback == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access UserFeedback - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _userFeedback = new ObservableCollection<UserFeedback>();
                    }
                    else
                    {
                        var items = base.SoAContext.UserFeedback.Where(x => x.ProvidedByAgent == this.AgentId).ToList<UserFeedback>();
                        _userFeedback = new ObservableCollection<UserFeedback>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _userFeedback.CollectionChanged += UserFeedback_CollectionChanged;
                }
                return _userFeedback;
            }
            private set
            {
                if (_userFeedback != null)
                {
                    _userFeedback.CollectionChanged -= UserFeedback_CollectionChanged;
                }
                _userFeedback = value;
                if (_userFeedback != null)
                {
                    _userFeedback.CollectionChanged += UserFeedback_CollectionChanged;
                }
            }
        }

        private void UserFeedback_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<UserFeedback>())
                {
                    item.ProvidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ChangeRequest> _changeRequests;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ChangeRequest> ChangeRequests
        {
            get
            {
                if (_changeRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _changeRequests = new ObservableCollection<ChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeRequests.Where(x => x.RequestedByAgent == this.AgentId).ToList<ChangeRequest>();
                        _changeRequests = new ObservableCollection<ChangeRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeRequests.CollectionChanged += ChangeRequests_CollectionChanged;
                }
                return _changeRequests;
            }
            private set
            {
                if (_changeRequests != null)
                {
                    _changeRequests.CollectionChanged -= ChangeRequests_CollectionChanged;
                }
                _changeRequests = value;
                if (_changeRequests != null)
                {
                    _changeRequests.CollectionChanged += ChangeRequests_CollectionChanged;
                }
            }
        }

        private void ChangeRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeRequest>())
                {
                    item.RequestedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ReviewEvent> _reviewEvents;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ReviewEvent> ReviewEvents
        {
            get
            {
                if (_reviewEvents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewEvents - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _reviewEvents = new ObservableCollection<ReviewEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.ReviewEvents.Where(x => x.ReviewedByAgent == this.AgentId).ToList<ReviewEvent>();
                        _reviewEvents = new ObservableCollection<ReviewEvent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _reviewEvents.CollectionChanged += ReviewEvents_CollectionChanged;
                }
                return _reviewEvents;
            }
            private set
            {
                if (_reviewEvents != null)
                {
                    _reviewEvents.CollectionChanged -= ReviewEvents_CollectionChanged;
                }
                _reviewEvents = value;
                if (_reviewEvents != null)
                {
                    _reviewEvents.CollectionChanged += ReviewEvents_CollectionChanged;
                }
            }
        }

        private void ReviewEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ReviewEvent>())
                {
                    item.ReviewedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<LearningActivity> _learningActivities;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<LearningActivity> LearningActivities
        {
            get
            {
                if (_learningActivities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.LearningActivities.Where(x => x.FacilitatorAgent == this.AgentId).ToList<LearningActivity>();
                        _learningActivities = new ObservableCollection<LearningActivity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _learningActivities.CollectionChanged += LearningActivities_CollectionChanged;
                }
                return _learningActivities;
            }
            private set
            {
                if (_learningActivities != null)
                {
                    _learningActivities.CollectionChanged -= LearningActivities_CollectionChanged;
                }
                _learningActivities = value;
                if (_learningActivities != null)
                {
                    _learningActivities.CollectionChanged += LearningActivities_CollectionChanged;
                }
            }
        }

        private void LearningActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LearningActivity>())
                {
                    item.FacilitatorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _invokedByAgentExceptionInvocations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ExceptionInvocation> InvokedByAgentExceptionInvocations
        {
            get
            {
                if (_invokedByAgentExceptionInvocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InvokedByAgentExceptionInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _invokedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExceptionInvocations.Where(x => x.InvokedByAgent == this.AgentId).ToList<ExceptionInvocation>();
                        _invokedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _invokedByAgentExceptionInvocations.CollectionChanged += InvokedByAgentExceptionInvocations_CollectionChanged;
                }
                return _invokedByAgentExceptionInvocations;
            }
            private set
            {
                if (_invokedByAgentExceptionInvocations != null)
                {
                    _invokedByAgentExceptionInvocations.CollectionChanged -= InvokedByAgentExceptionInvocations_CollectionChanged;
                }
                _invokedByAgentExceptionInvocations = value;
                if (_invokedByAgentExceptionInvocations != null)
                {
                    _invokedByAgentExceptionInvocations.CollectionChanged += InvokedByAgentExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void InvokedByAgentExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.InvokedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExceptionInvocation> _approvedByAgentExceptionInvocations;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ExceptionInvocation> ApprovedByAgentExceptionInvocations
        {
            get
            {
                if (_approvedByAgentExceptionInvocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovedByAgentExceptionInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _approvedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExceptionInvocations.Where(x => x.ApprovedByAgent == this.AgentId).ToList<ExceptionInvocation>();
                        _approvedByAgentExceptionInvocations = new ObservableCollection<ExceptionInvocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvedByAgentExceptionInvocations.CollectionChanged += ApprovedByAgentExceptionInvocations_CollectionChanged;
                }
                return _approvedByAgentExceptionInvocations;
            }
            private set
            {
                if (_approvedByAgentExceptionInvocations != null)
                {
                    _approvedByAgentExceptionInvocations.CollectionChanged -= ApprovedByAgentExceptionInvocations_CollectionChanged;
                }
                _approvedByAgentExceptionInvocations = value;
                if (_approvedByAgentExceptionInvocations != null)
                {
                    _approvedByAgentExceptionInvocations.CollectionChanged += ApprovedByAgentExceptionInvocations_CollectionChanged;
                }
            }
        }

        private void ApprovedByAgentExceptionInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExceptionInvocation>())
                {
                    item.ApprovedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<VerificationOutcome> _verificationOutcomes;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<VerificationOutcome> VerificationOutcomes
        {
            get
            {
                if (_verificationOutcomes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerificationOutcomes - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>();
                    }
                    else
                    {
                        var items = base.SoAContext.VerificationOutcomes.Where(x => x.ObservedByAgent == this.AgentId).ToList<VerificationOutcome>();
                        _verificationOutcomes = new ObservableCollection<VerificationOutcome>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _verificationOutcomes.CollectionChanged += VerificationOutcomes_CollectionChanged;
                }
                return _verificationOutcomes;
            }
            private set
            {
                if (_verificationOutcomes != null)
                {
                    _verificationOutcomes.CollectionChanged -= VerificationOutcomes_CollectionChanged;
                }
                _verificationOutcomes = value;
                if (_verificationOutcomes != null)
                {
                    _verificationOutcomes.CollectionChanged += VerificationOutcomes_CollectionChanged;
                }
            }
        }

        private void VerificationOutcomes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<VerificationOutcome>())
                {
                    item.ObservedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<MessageDelivery> _messageDeliveries;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<MessageDelivery> MessageDeliveries
        {
            get
            {
                if (_messageDeliveries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDeliveries - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _messageDeliveries = new ObservableCollection<MessageDelivery>();
                    }
                    else
                    {
                        var items = base.SoAContext.MessageDeliveries.Where(x => x.SentByAgent == this.AgentId).ToList<MessageDelivery>();
                        _messageDeliveries = new ObservableCollection<MessageDelivery>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _messageDeliveries.CollectionChanged += MessageDeliveries_CollectionChanged;
                }
                return _messageDeliveries;
            }
            private set
            {
                if (_messageDeliveries != null)
                {
                    _messageDeliveries.CollectionChanged -= MessageDeliveries_CollectionChanged;
                }
                _messageDeliveries = value;
                if (_messageDeliveries != null)
                {
                    _messageDeliveries.CollectionChanged += MessageDeliveries_CollectionChanged;
                }
            }
        }

        private void MessageDeliveries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MessageDelivery>())
                {
                    item.SentByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<TemplateApproval> _templateApprovals;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<TemplateApproval> TemplateApprovals
        {
            get
            {
                if (_templateApprovals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TemplateApprovals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _templateApprovals = new ObservableCollection<TemplateApproval>();
                    }
                    else
                    {
                        var items = base.SoAContext.TemplateApprovals.Where(x => x.DecidedByAgent == this.AgentId).ToList<TemplateApproval>();
                        _templateApprovals = new ObservableCollection<TemplateApproval>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _templateApprovals.CollectionChanged += TemplateApprovals_CollectionChanged;
                }
                return _templateApprovals;
            }
            private set
            {
                if (_templateApprovals != null)
                {
                    _templateApprovals.CollectionChanged -= TemplateApprovals_CollectionChanged;
                }
                _templateApprovals = value;
                if (_templateApprovals != null)
                {
                    _templateApprovals.CollectionChanged += TemplateApprovals_CollectionChanged;
                }
            }
        }

        private void TemplateApprovals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TemplateApproval>())
                {
                    item.DecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _decidingAgentAgentDecisionRecords;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AgentDecisionRecord> DecidingAgentAgentDecisionRecords
        {
            get
            {
                if (_decidingAgentAgentDecisionRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DecidingAgentAgentDecisionRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _decidingAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentDecisionRecords.Where(x => x.DecidingAgent == this.AgentId).ToList<AgentDecisionRecord>();
                        _decidingAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _decidingAgentAgentDecisionRecords.CollectionChanged += DecidingAgentAgentDecisionRecords_CollectionChanged;
                }
                return _decidingAgentAgentDecisionRecords;
            }
            private set
            {
                if (_decidingAgentAgentDecisionRecords != null)
                {
                    _decidingAgentAgentDecisionRecords.CollectionChanged -= DecidingAgentAgentDecisionRecords_CollectionChanged;
                }
                _decidingAgentAgentDecisionRecords = value;
                if (_decidingAgentAgentDecisionRecords != null)
                {
                    _decidingAgentAgentDecisionRecords.CollectionChanged += DecidingAgentAgentDecisionRecords_CollectionChanged;
                }
            }
        }

        private void DecidingAgentAgentDecisionRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentDecisionRecord>())
                {
                    item.DecidingAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentDecisionRecord> _reviewedByAgentAgentDecisionRecords;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AgentDecisionRecord> ReviewedByAgentAgentDecisionRecords
        {
            get
            {
                if (_reviewedByAgentAgentDecisionRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewedByAgentAgentDecisionRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _reviewedByAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentDecisionRecords.Where(x => x.ReviewedByAgent == this.AgentId).ToList<AgentDecisionRecord>();
                        _reviewedByAgentAgentDecisionRecords = new ObservableCollection<AgentDecisionRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _reviewedByAgentAgentDecisionRecords.CollectionChanged += ReviewedByAgentAgentDecisionRecords_CollectionChanged;
                }
                return _reviewedByAgentAgentDecisionRecords;
            }
            private set
            {
                if (_reviewedByAgentAgentDecisionRecords != null)
                {
                    _reviewedByAgentAgentDecisionRecords.CollectionChanged -= ReviewedByAgentAgentDecisionRecords_CollectionChanged;
                }
                _reviewedByAgentAgentDecisionRecords = value;
                if (_reviewedByAgentAgentDecisionRecords != null)
                {
                    _reviewedByAgentAgentDecisionRecords.CollectionChanged += ReviewedByAgentAgentDecisionRecords_CollectionChanged;
                }
            }
        }

        private void ReviewedByAgentAgentDecisionRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentDecisionRecord>())
                {
                    item.ReviewedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<Attestation> _attestations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<Attestation> Attestations
        {
            get
            {
                if (_attestations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Attestations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _attestations = new ObservableCollection<Attestation>();
                    }
                    else
                    {
                        var items = base.SoAContext.Attestations.Where(x => x.SignedByAgent == this.AgentId).ToList<Attestation>();
                        _attestations = new ObservableCollection<Attestation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _attestations.CollectionChanged += Attestations_CollectionChanged;
                }
                return _attestations;
            }
            private set
            {
                if (_attestations != null)
                {
                    _attestations.CollectionChanged -= Attestations_CollectionChanged;
                }
                _attestations = value;
                if (_attestations != null)
                {
                    _attestations.CollectionChanged += Attestations_CollectionChanged;
                }
            }
        }

        private void Attestations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Attestation>())
                {
                    item.SignedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AppUser> _appUsers;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AppUser> AppUsers
        {
            get
            {
                if (_appUsers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppUsers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _appUsers = new ObservableCollection<AppUser>();
                    }
                    else
                    {
                        var items = base.SoAContext.AppUsers.Where(x => x.LinkedAgent == this.AgentId).ToList<AppUser>();
                        _appUsers = new ObservableCollection<AppUser>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appUsers.CollectionChanged += AppUsers_CollectionChanged;
                }
                return _appUsers;
            }
            private set
            {
                if (_appUsers != null)
                {
                    _appUsers.CollectionChanged -= AppUsers_CollectionChanged;
                }
                _appUsers = value;
                if (_appUsers != null)
                {
                    _appUsers.CollectionChanged += AppUsers_CollectionChanged;
                }
            }
        }

        private void AppUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AppUser>())
                {
                    item.LinkedAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _seekerKnowledgeBrokerLinks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeBrokerLink> SeekerKnowledgeBrokerLinks
        {
            get
            {
                if (_seekerKnowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SeekerKnowledgeBrokerLinks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _seekerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.Seeker == this.AgentId).ToList<KnowledgeBrokerLink>();
                        _seekerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _seekerKnowledgeBrokerLinks.CollectionChanged += SeekerKnowledgeBrokerLinks_CollectionChanged;
                }
                return _seekerKnowledgeBrokerLinks;
            }
            private set
            {
                if (_seekerKnowledgeBrokerLinks != null)
                {
                    _seekerKnowledgeBrokerLinks.CollectionChanged -= SeekerKnowledgeBrokerLinks_CollectionChanged;
                }
                _seekerKnowledgeBrokerLinks = value;
                if (_seekerKnowledgeBrokerLinks != null)
                {
                    _seekerKnowledgeBrokerLinks.CollectionChanged += SeekerKnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void SeekerKnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.Seeker = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeBrokerLink> _brokerKnowledgeBrokerLinks;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<KnowledgeBrokerLink> BrokerKnowledgeBrokerLinks
        {
            get
            {
                if (_brokerKnowledgeBrokerLinks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access BrokerKnowledgeBrokerLinks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _brokerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeBrokerLinks.Where(x => x.Broker == this.AgentId).ToList<KnowledgeBrokerLink>();
                        _brokerKnowledgeBrokerLinks = new ObservableCollection<KnowledgeBrokerLink>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _brokerKnowledgeBrokerLinks.CollectionChanged += BrokerKnowledgeBrokerLinks_CollectionChanged;
                }
                return _brokerKnowledgeBrokerLinks;
            }
            private set
            {
                if (_brokerKnowledgeBrokerLinks != null)
                {
                    _brokerKnowledgeBrokerLinks.CollectionChanged -= BrokerKnowledgeBrokerLinks_CollectionChanged;
                }
                _brokerKnowledgeBrokerLinks = value;
                if (_brokerKnowledgeBrokerLinks != null)
                {
                    _brokerKnowledgeBrokerLinks.CollectionChanged += BrokerKnowledgeBrokerLinks_CollectionChanged;
                }
            }
        }

        private void BrokerKnowledgeBrokerLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeBrokerLink>())
                {
                    item.Broker = this.AgentId;
                }
            }
        }

        private ObservableCollection<MethodApplication> _appliedByAgentMethodApplications;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<MethodApplication> AppliedByAgentMethodApplications
        {
            get
            {
                if (_appliedByAgentMethodApplications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AppliedByAgentMethodApplications - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _appliedByAgentMethodApplications = new ObservableCollection<MethodApplication>();
                    }
                    else
                    {
                        var items = base.SoAContext.MethodApplications.Where(x => x.AppliedByAgent == this.AgentId).ToList<MethodApplication>();
                        _appliedByAgentMethodApplications = new ObservableCollection<MethodApplication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _appliedByAgentMethodApplications.CollectionChanged += AppliedByAgentMethodApplications_CollectionChanged;
                }
                return _appliedByAgentMethodApplications;
            }
            private set
            {
                if (_appliedByAgentMethodApplications != null)
                {
                    _appliedByAgentMethodApplications.CollectionChanged -= AppliedByAgentMethodApplications_CollectionChanged;
                }
                _appliedByAgentMethodApplications = value;
                if (_appliedByAgentMethodApplications != null)
                {
                    _appliedByAgentMethodApplications.CollectionChanged += AppliedByAgentMethodApplications_CollectionChanged;
                }
            }
        }

        private void AppliedByAgentMethodApplications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MethodApplication>())
                {
                    item.AppliedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<MethodApplication> _identifiedBrokerMethodApplications;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<MethodApplication> IdentifiedBrokerMethodApplications
        {
            get
            {
                if (_identifiedBrokerMethodApplications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IdentifiedBrokerMethodApplications - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _identifiedBrokerMethodApplications = new ObservableCollection<MethodApplication>();
                    }
                    else
                    {
                        var items = base.SoAContext.MethodApplications.Where(x => x.IdentifiedBroker == this.AgentId).ToList<MethodApplication>();
                        _identifiedBrokerMethodApplications = new ObservableCollection<MethodApplication>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _identifiedBrokerMethodApplications.CollectionChanged += IdentifiedBrokerMethodApplications_CollectionChanged;
                }
                return _identifiedBrokerMethodApplications;
            }
            private set
            {
                if (_identifiedBrokerMethodApplications != null)
                {
                    _identifiedBrokerMethodApplications.CollectionChanged -= IdentifiedBrokerMethodApplications_CollectionChanged;
                }
                _identifiedBrokerMethodApplications = value;
                if (_identifiedBrokerMethodApplications != null)
                {
                    _identifiedBrokerMethodApplications.CollectionChanged += IdentifiedBrokerMethodApplications_CollectionChanged;
                }
            }
        }

        private void IdentifiedBrokerMethodApplications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MethodApplication>())
                {
                    item.IdentifiedBroker = this.AgentId;
                }
            }
        }

        private ObservableCollection<ConditionCheck> _conditionChecks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ConditionCheck> ConditionChecks
        {
            get
            {
                if (_conditionChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConditionChecks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _conditionChecks = new ObservableCollection<ConditionCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConditionChecks.Where(x => x.CheckedByAgent == this.AgentId).ToList<ConditionCheck>();
                        _conditionChecks = new ObservableCollection<ConditionCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _conditionChecks.CollectionChanged += ConditionChecks_CollectionChanged;
                }
                return _conditionChecks;
            }
            private set
            {
                if (_conditionChecks != null)
                {
                    _conditionChecks.CollectionChanged -= ConditionChecks_CollectionChanged;
                }
                _conditionChecks = value;
                if (_conditionChecks != null)
                {
                    _conditionChecks.CollectionChanged += ConditionChecks_CollectionChanged;
                }
            }
        }

        private void ConditionChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConditionCheck>())
                {
                    item.CheckedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<CueObservation> _observedByAgentCueObservations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<CueObservation> ObservedByAgentCueObservations
        {
            get
            {
                if (_observedByAgentCueObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedByAgentCueObservations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _observedByAgentCueObservations = new ObservableCollection<CueObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.CueObservations.Where(x => x.ObservedByAgent == this.AgentId).ToList<CueObservation>();
                        _observedByAgentCueObservations = new ObservableCollection<CueObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedByAgentCueObservations.CollectionChanged += ObservedByAgentCueObservations_CollectionChanged;
                }
                return _observedByAgentCueObservations;
            }
            private set
            {
                if (_observedByAgentCueObservations != null)
                {
                    _observedByAgentCueObservations.CollectionChanged -= ObservedByAgentCueObservations_CollectionChanged;
                }
                _observedByAgentCueObservations = value;
                if (_observedByAgentCueObservations != null)
                {
                    _observedByAgentCueObservations.CollectionChanged += ObservedByAgentCueObservations_CollectionChanged;
                }
            }
        }

        private void ObservedByAgentCueObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CueObservation>())
                {
                    item.ObservedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<CueObservation> _escalatedToAgentCueObservations;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<CueObservation> EscalatedToAgentCueObservations
        {
            get
            {
                if (_escalatedToAgentCueObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EscalatedToAgentCueObservations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _escalatedToAgentCueObservations = new ObservableCollection<CueObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.CueObservations.Where(x => x.EscalatedToAgent == this.AgentId).ToList<CueObservation>();
                        _escalatedToAgentCueObservations = new ObservableCollection<CueObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _escalatedToAgentCueObservations.CollectionChanged += EscalatedToAgentCueObservations_CollectionChanged;
                }
                return _escalatedToAgentCueObservations;
            }
            private set
            {
                if (_escalatedToAgentCueObservations != null)
                {
                    _escalatedToAgentCueObservations.CollectionChanged -= EscalatedToAgentCueObservations_CollectionChanged;
                }
                _escalatedToAgentCueObservations = value;
                if (_escalatedToAgentCueObservations != null)
                {
                    _escalatedToAgentCueObservations.CollectionChanged += EscalatedToAgentCueObservations_CollectionChanged;
                }
            }
        }

        private void EscalatedToAgentCueObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CueObservation>())
                {
                    item.EscalatedToAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExecutionParticipant> _executionParticipants;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ExecutionParticipant> ExecutionParticipants
        {
            get
            {
                if (_executionParticipants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExecutionParticipants - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _executionParticipants = new ObservableCollection<ExecutionParticipant>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExecutionParticipants.Where(x => x.Agent == this.AgentId).ToList<ExecutionParticipant>();
                        _executionParticipants = new ObservableCollection<ExecutionParticipant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _executionParticipants.CollectionChanged += ExecutionParticipants_CollectionChanged;
                }
                return _executionParticipants;
            }
            private set
            {
                if (_executionParticipants != null)
                {
                    _executionParticipants.CollectionChanged -= ExecutionParticipants_CollectionChanged;
                }
                _executionParticipants = value;
                if (_executionParticipants != null)
                {
                    _executionParticipants.CollectionChanged += ExecutionParticipants_CollectionChanged;
                }
            }
        }

        private void ExecutionParticipants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExecutionParticipant>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AuthoringSubmission> _authoringSubmissions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AuthoringSubmission> AuthoringSubmissions
        {
            get
            {
                if (_authoringSubmissions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthoringSubmissions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _authoringSubmissions = new ObservableCollection<AuthoringSubmission>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthoringSubmissions.Where(x => x.SubmittedByAgent == this.AgentId).ToList<AuthoringSubmission>();
                        _authoringSubmissions = new ObservableCollection<AuthoringSubmission>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authoringSubmissions.CollectionChanged += AuthoringSubmissions_CollectionChanged;
                }
                return _authoringSubmissions;
            }
            private set
            {
                if (_authoringSubmissions != null)
                {
                    _authoringSubmissions.CollectionChanged -= AuthoringSubmissions_CollectionChanged;
                }
                _authoringSubmissions = value;
                if (_authoringSubmissions != null)
                {
                    _authoringSubmissions.CollectionChanged += AuthoringSubmissions_CollectionChanged;
                }
            }
        }

        private void AuthoringSubmissions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthoringSubmission>())
                {
                    item.SubmittedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<SituationalVariant> _situationalVariants;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<SituationalVariant> SituationalVariants
        {
            get
            {
                if (_situationalVariants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SituationalVariants - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _situationalVariants = new ObservableCollection<SituationalVariant>();
                    }
                    else
                    {
                        var items = base.SoAContext.SituationalVariants.Where(x => x.ExpertAgent == this.AgentId).ToList<SituationalVariant>();
                        _situationalVariants = new ObservableCollection<SituationalVariant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _situationalVariants.CollectionChanged += SituationalVariants_CollectionChanged;
                }
                return _situationalVariants;
            }
            private set
            {
                if (_situationalVariants != null)
                {
                    _situationalVariants.CollectionChanged -= SituationalVariants_CollectionChanged;
                }
                _situationalVariants = value;
                if (_situationalVariants != null)
                {
                    _situationalVariants.CollectionChanged += SituationalVariants_CollectionChanged;
                }
            }
        }

        private void SituationalVariants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SituationalVariant>())
                {
                    item.ExpertAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<CollectedSourceMaterial> CollectedSourceMaterials
        {
            get
            {
                if (_collectedSourceMaterials == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.ContributingExpert == this.AgentId).ToList<CollectedSourceMaterial>();
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
                return _collectedSourceMaterials;
            }
            private set
            {
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged -= CollectedSourceMaterials_CollectionChanged;
                }
                _collectedSourceMaterials = value;
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
            }
        }

        private void CollectedSourceMaterials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectedSourceMaterial>())
                {
                    item.ContributingExpert = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiLabelingRun> _aiLabelingRuns;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AiLabelingRun> AiLabelingRuns
        {
            get
            {
                if (_aiLabelingRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiLabelingRuns - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _aiLabelingRuns = new ObservableCollection<AiLabelingRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiLabelingRuns.Where(x => x.Agent == this.AgentId).ToList<AiLabelingRun>();
                        _aiLabelingRuns = new ObservableCollection<AiLabelingRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiLabelingRuns.CollectionChanged += AiLabelingRuns_CollectionChanged;
                }
                return _aiLabelingRuns;
            }
            private set
            {
                if (_aiLabelingRuns != null)
                {
                    _aiLabelingRuns.CollectionChanged -= AiLabelingRuns_CollectionChanged;
                }
                _aiLabelingRuns = value;
                if (_aiLabelingRuns != null)
                {
                    _aiLabelingRuns.CollectionChanged += AiLabelingRuns_CollectionChanged;
                }
            }
        }

        private void AiLabelingRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiLabelingRun>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<TermMeaningChange> _termMeaningChanges;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<TermMeaningChange> TermMeaningChanges
        {
            get
            {
                if (_termMeaningChanges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TermMeaningChanges - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _termMeaningChanges = new ObservableCollection<TermMeaningChange>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermMeaningChanges.Where(x => x.RecordedByAgent == this.AgentId).ToList<TermMeaningChange>();
                        _termMeaningChanges = new ObservableCollection<TermMeaningChange>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _termMeaningChanges.CollectionChanged += TermMeaningChanges_CollectionChanged;
                }
                return _termMeaningChanges;
            }
            private set
            {
                if (_termMeaningChanges != null)
                {
                    _termMeaningChanges.CollectionChanged -= TermMeaningChanges_CollectionChanged;
                }
                _termMeaningChanges = value;
                if (_termMeaningChanges != null)
                {
                    _termMeaningChanges.CollectionChanged += TermMeaningChanges_CollectionChanged;
                }
            }
        }

        private void TermMeaningChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermMeaningChange>())
                {
                    item.RecordedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentIntegration> _agentIntegrations;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AgentIntegration> AgentIntegrations
        {
            get
            {
                if (_agentIntegrations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentIntegrations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _agentIntegrations = new ObservableCollection<AgentIntegration>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentIntegrations.Where(x => x.Agent == this.AgentId).ToList<AgentIntegration>();
                        _agentIntegrations = new ObservableCollection<AgentIntegration>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _agentIntegrations.CollectionChanged += AgentIntegrations_CollectionChanged;
                }
                return _agentIntegrations;
            }
            private set
            {
                if (_agentIntegrations != null)
                {
                    _agentIntegrations.CollectionChanged -= AgentIntegrations_CollectionChanged;
                }
                _agentIntegrations = value;
                if (_agentIntegrations != null)
                {
                    _agentIntegrations.CollectionChanged += AgentIntegrations_CollectionChanged;
                }
            }
        }

        private void AgentIntegrations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentIntegration>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<SnapshotAssertion> _snapshotAssertions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<SnapshotAssertion> SnapshotAssertions
        {
            get
            {
                if (_snapshotAssertions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SnapshotAssertions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>();
                    }
                    else
                    {
                        var items = base.SoAContext.SnapshotAssertions.Where(x => x.AboutAgent == this.AgentId).ToList<SnapshotAssertion>();
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
                return _snapshotAssertions;
            }
            private set
            {
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged -= SnapshotAssertions_CollectionChanged;
                }
                _snapshotAssertions = value;
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
            }
        }

        private void SnapshotAssertions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SnapshotAssertion>())
                {
                    item.AboutAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _retrievalSegments;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<RetrievalSegment> RetrievalSegments
        {
            get
            {
                if (_retrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.AuthoredByAgent == this.AgentId).ToList<RetrievalSegment>();
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
                return _retrievalSegments;
            }
            private set
            {
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged -= RetrievalSegments_CollectionChanged;
                }
                _retrievalSegments = value;
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
            }
        }

        private void RetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.AuthoredByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _answeringAgentAssistantAnswers;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AssistantAnswer> AnsweringAgentAssistantAnswers
        {
            get
            {
                if (_answeringAgentAssistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnsweringAgentAssistantAnswers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _answeringAgentAssistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.AnsweringAgent == this.AgentId).ToList<AssistantAnswer>();
                        _answeringAgentAssistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answeringAgentAssistantAnswers.CollectionChanged += AnsweringAgentAssistantAnswers_CollectionChanged;
                }
                return _answeringAgentAssistantAnswers;
            }
            private set
            {
                if (_answeringAgentAssistantAnswers != null)
                {
                    _answeringAgentAssistantAnswers.CollectionChanged -= AnsweringAgentAssistantAnswers_CollectionChanged;
                }
                _answeringAgentAssistantAnswers = value;
                if (_answeringAgentAssistantAnswers != null)
                {
                    _answeringAgentAssistantAnswers.CollectionChanged += AnsweringAgentAssistantAnswers_CollectionChanged;
                }
            }
        }

        private void AnsweringAgentAssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.AnsweringAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _askedByAgentAssistantAnswers;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AssistantAnswer> AskedByAgentAssistantAnswers
        {
            get
            {
                if (_askedByAgentAssistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AskedByAgentAssistantAnswers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _askedByAgentAssistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.AskedByAgent == this.AgentId).ToList<AssistantAnswer>();
                        _askedByAgentAssistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _askedByAgentAssistantAnswers.CollectionChanged += AskedByAgentAssistantAnswers_CollectionChanged;
                }
                return _askedByAgentAssistantAnswers;
            }
            private set
            {
                if (_askedByAgentAssistantAnswers != null)
                {
                    _askedByAgentAssistantAnswers.CollectionChanged -= AskedByAgentAssistantAnswers_CollectionChanged;
                }
                _askedByAgentAssistantAnswers = value;
                if (_askedByAgentAssistantAnswers != null)
                {
                    _askedByAgentAssistantAnswers.CollectionChanged += AskedByAgentAssistantAnswers_CollectionChanged;
                }
            }
        }

        private void AskedByAgentAssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.AskedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _humanReviewedByAssistantAnswers;

        [InverseProperty("AgentRefRef")]
        public virtual ObservableCollection<AssistantAnswer> HumanReviewedByAssistantAnswers
        {
            get
            {
                if (_humanReviewedByAssistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HumanReviewedByAssistantAnswers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _humanReviewedByAssistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.HumanReviewedBy == this.AgentId).ToList<AssistantAnswer>();
                        _humanReviewedByAssistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _humanReviewedByAssistantAnswers.CollectionChanged += HumanReviewedByAssistantAnswers_CollectionChanged;
                }
                return _humanReviewedByAssistantAnswers;
            }
            private set
            {
                if (_humanReviewedByAssistantAnswers != null)
                {
                    _humanReviewedByAssistantAnswers.CollectionChanged -= HumanReviewedByAssistantAnswers_CollectionChanged;
                }
                _humanReviewedByAssistantAnswers = value;
                if (_humanReviewedByAssistantAnswers != null)
                {
                    _humanReviewedByAssistantAnswers.CollectionChanged += HumanReviewedByAssistantAnswers_CollectionChanged;
                }
            }
        }

        private void HumanReviewedByAssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.HumanReviewedBy = this.AgentId;
                }
            }
        }

        private ObservableCollection<AnswerRequirementCheck> _answerRequirementChecks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AnswerRequirementCheck> AnswerRequirementChecks
        {
            get
            {
                if (_answerRequirementChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnswerRequirementChecks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _answerRequirementChecks = new ObservableCollection<AnswerRequirementCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.AnswerRequirementChecks.Where(x => x.CheckedByAgent == this.AgentId).ToList<AnswerRequirementCheck>();
                        _answerRequirementChecks = new ObservableCollection<AnswerRequirementCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answerRequirementChecks.CollectionChanged += AnswerRequirementChecks_CollectionChanged;
                }
                return _answerRequirementChecks;
            }
            private set
            {
                if (_answerRequirementChecks != null)
                {
                    _answerRequirementChecks.CollectionChanged -= AnswerRequirementChecks_CollectionChanged;
                }
                _answerRequirementChecks = value;
                if (_answerRequirementChecks != null)
                {
                    _answerRequirementChecks.CollectionChanged += AnswerRequirementChecks_CollectionChanged;
                }
            }
        }

        private void AnswerRequirementChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AnswerRequirementCheck>())
                {
                    item.CheckedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiToolInvocation> _aiToolInvocations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AiToolInvocation> AiToolInvocations
        {
            get
            {
                if (_aiToolInvocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiToolInvocations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _aiToolInvocations = new ObservableCollection<AiToolInvocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiToolInvocations.Where(x => x.InvokingAgent == this.AgentId).ToList<AiToolInvocation>();
                        _aiToolInvocations = new ObservableCollection<AiToolInvocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiToolInvocations.CollectionChanged += AiToolInvocations_CollectionChanged;
                }
                return _aiToolInvocations;
            }
            private set
            {
                if (_aiToolInvocations != null)
                {
                    _aiToolInvocations.CollectionChanged -= AiToolInvocations_CollectionChanged;
                }
                _aiToolInvocations = value;
                if (_aiToolInvocations != null)
                {
                    _aiToolInvocations.CollectionChanged += AiToolInvocations_CollectionChanged;
                }
            }
        }

        private void AiToolInvocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiToolInvocation>())
                {
                    item.InvokingAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<PromptTemplate> _promptTemplates;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<PromptTemplate> PromptTemplates
        {
            get
            {
                if (_promptTemplates == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PromptTemplates - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _promptTemplates = new ObservableCollection<PromptTemplate>();
                    }
                    else
                    {
                        var items = base.SoAContext.PromptTemplates.Where(x => x.UsedByAgent == this.AgentId).ToList<PromptTemplate>();
                        _promptTemplates = new ObservableCollection<PromptTemplate>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
                return _promptTemplates;
            }
            private set
            {
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged -= PromptTemplates_CollectionChanged;
                }
                _promptTemplates = value;
                if (_promptTemplates != null)
                {
                    _promptTemplates.CollectionChanged += PromptTemplates_CollectionChanged;
                }
            }
        }

        private void PromptTemplates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PromptTemplate>())
                {
                    item.UsedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelAnnotation> _modelAnnotations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ModelAnnotation> ModelAnnotations
        {
            get
            {
                if (_modelAnnotations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelAnnotations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelAnnotations.Where(x => x.AnnotatedByAgent == this.AgentId).ToList<ModelAnnotation>();
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
                return _modelAnnotations;
            }
            private set
            {
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged -= ModelAnnotations_CollectionChanged;
                }
                _modelAnnotations = value;
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
            }
        }

        private void ModelAnnotations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelAnnotation>())
                {
                    item.AnnotatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeSearchEvent> _knowledgeSearchEvents;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeSearchEvent> KnowledgeSearchEvents
        {
            get
            {
                if (_knowledgeSearchEvents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeSearchEvents - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeSearchEvents.Where(x => x.SearchedByAgent == this.AgentId).ToList<KnowledgeSearchEvent>();
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
                return _knowledgeSearchEvents;
            }
            private set
            {
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged -= KnowledgeSearchEvents_CollectionChanged;
                }
                _knowledgeSearchEvents = value;
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
            }
        }

        private void KnowledgeSearchEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeSearchEvent>())
                {
                    item.SearchedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiAdoptionInitiatif> _aiAdoptionInitiatives;

        [InverseProperty("AgentRef")]
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
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatives - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAdoptionInitiatives.Where(x => x.Agent == this.AgentId).ToList<AiAdoptionInitiatif>();
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
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiInsightProposal> _proposingAgentAiInsightProposals;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AiInsightProposal> ProposingAgentAiInsightProposals
        {
            get
            {
                if (_proposingAgentAiInsightProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProposingAgentAiInsightProposals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _proposingAgentAiInsightProposals = new ObservableCollection<AiInsightProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiInsightProposals.Where(x => x.ProposingAgent == this.AgentId).ToList<AiInsightProposal>();
                        _proposingAgentAiInsightProposals = new ObservableCollection<AiInsightProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _proposingAgentAiInsightProposals.CollectionChanged += ProposingAgentAiInsightProposals_CollectionChanged;
                }
                return _proposingAgentAiInsightProposals;
            }
            private set
            {
                if (_proposingAgentAiInsightProposals != null)
                {
                    _proposingAgentAiInsightProposals.CollectionChanged -= ProposingAgentAiInsightProposals_CollectionChanged;
                }
                _proposingAgentAiInsightProposals = value;
                if (_proposingAgentAiInsightProposals != null)
                {
                    _proposingAgentAiInsightProposals.CollectionChanged += ProposingAgentAiInsightProposals_CollectionChanged;
                }
            }
        }

        private void ProposingAgentAiInsightProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiInsightProposal>())
                {
                    item.ProposingAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiInsightProposal> _validatedByAgentAiInsightProposals;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AiInsightProposal> ValidatedByAgentAiInsightProposals
        {
            get
            {
                if (_validatedByAgentAiInsightProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ValidatedByAgentAiInsightProposals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _validatedByAgentAiInsightProposals = new ObservableCollection<AiInsightProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiInsightProposals.Where(x => x.ValidatedByAgent == this.AgentId).ToList<AiInsightProposal>();
                        _validatedByAgentAiInsightProposals = new ObservableCollection<AiInsightProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _validatedByAgentAiInsightProposals.CollectionChanged += ValidatedByAgentAiInsightProposals_CollectionChanged;
                }
                return _validatedByAgentAiInsightProposals;
            }
            private set
            {
                if (_validatedByAgentAiInsightProposals != null)
                {
                    _validatedByAgentAiInsightProposals.CollectionChanged -= ValidatedByAgentAiInsightProposals_CollectionChanged;
                }
                _validatedByAgentAiInsightProposals = value;
                if (_validatedByAgentAiInsightProposals != null)
                {
                    _validatedByAgentAiInsightProposals.CollectionChanged += ValidatedByAgentAiInsightProposals_CollectionChanged;
                }
            }
        }

        private void ValidatedByAgentAiInsightProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiInsightProposal>())
                {
                    item.ValidatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AssistantBenchmark> _assistantBenchmarks;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AssistantBenchmark> AssistantBenchmarks
        {
            get
            {
                if (_assistantBenchmarks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssistantBenchmarks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _assistantBenchmarks = new ObservableCollection<AssistantBenchmark>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantBenchmarks.Where(x => x.Agent == this.AgentId).ToList<AssistantBenchmark>();
                        _assistantBenchmarks = new ObservableCollection<AssistantBenchmark>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assistantBenchmarks.CollectionChanged += AssistantBenchmarks_CollectionChanged;
                }
                return _assistantBenchmarks;
            }
            private set
            {
                if (_assistantBenchmarks != null)
                {
                    _assistantBenchmarks.CollectionChanged -= AssistantBenchmarks_CollectionChanged;
                }
                _assistantBenchmarks = value;
                if (_assistantBenchmarks != null)
                {
                    _assistantBenchmarks.CollectionChanged += AssistantBenchmarks_CollectionChanged;
                }
            }
        }

        private void AssistantBenchmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantBenchmark>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StewardActivity> _stewardActivities;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access StewardActivities - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _stewardActivities = new ObservableCollection<StewardActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardActivities.Where(x => x.PerformedByAgent == this.AgentId).ToList<StewardActivity>();
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
                    item.PerformedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelChangeRequest> _requestedByAgentModelChangeRequests;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ModelChangeRequest> RequestedByAgentModelChangeRequests
        {
            get
            {
                if (_requestedByAgentModelChangeRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequestedByAgentModelChangeRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _requestedByAgentModelChangeRequests = new ObservableCollection<ModelChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeRequests.Where(x => x.RequestedByAgent == this.AgentId).ToList<ModelChangeRequest>();
                        _requestedByAgentModelChangeRequests = new ObservableCollection<ModelChangeRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requestedByAgentModelChangeRequests.CollectionChanged += RequestedByAgentModelChangeRequests_CollectionChanged;
                }
                return _requestedByAgentModelChangeRequests;
            }
            private set
            {
                if (_requestedByAgentModelChangeRequests != null)
                {
                    _requestedByAgentModelChangeRequests.CollectionChanged -= RequestedByAgentModelChangeRequests_CollectionChanged;
                }
                _requestedByAgentModelChangeRequests = value;
                if (_requestedByAgentModelChangeRequests != null)
                {
                    _requestedByAgentModelChangeRequests.CollectionChanged += RequestedByAgentModelChangeRequests_CollectionChanged;
                }
            }
        }

        private void RequestedByAgentModelChangeRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeRequest>())
                {
                    item.RequestedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelChangeRequest> _approvedByAgentModelChangeRequests;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ModelChangeRequest> ApprovedByAgentModelChangeRequests
        {
            get
            {
                if (_approvedByAgentModelChangeRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ApprovedByAgentModelChangeRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _approvedByAgentModelChangeRequests = new ObservableCollection<ModelChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeRequests.Where(x => x.ApprovedByAgent == this.AgentId).ToList<ModelChangeRequest>();
                        _approvedByAgentModelChangeRequests = new ObservableCollection<ModelChangeRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _approvedByAgentModelChangeRequests.CollectionChanged += ApprovedByAgentModelChangeRequests_CollectionChanged;
                }
                return _approvedByAgentModelChangeRequests;
            }
            private set
            {
                if (_approvedByAgentModelChangeRequests != null)
                {
                    _approvedByAgentModelChangeRequests.CollectionChanged -= ApprovedByAgentModelChangeRequests_CollectionChanged;
                }
                _approvedByAgentModelChangeRequests = value;
                if (_approvedByAgentModelChangeRequests != null)
                {
                    _approvedByAgentModelChangeRequests.CollectionChanged += ApprovedByAgentModelChangeRequests_CollectionChanged;
                }
            }
        }

        private void ApprovedByAgentModelChangeRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeRequest>())
                {
                    item.ApprovedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelChangeRequest> _placementDecidedByAgentModelChangeRequests;

        [InverseProperty("AgentRefRef")]
        public virtual ObservableCollection<ModelChangeRequest> PlacementDecidedByAgentModelChangeRequests
        {
            get
            {
                if (_placementDecidedByAgentModelChangeRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PlacementDecidedByAgentModelChangeRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _placementDecidedByAgentModelChangeRequests = new ObservableCollection<ModelChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeRequests.Where(x => x.PlacementDecidedByAgent == this.AgentId).ToList<ModelChangeRequest>();
                        _placementDecidedByAgentModelChangeRequests = new ObservableCollection<ModelChangeRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _placementDecidedByAgentModelChangeRequests.CollectionChanged += PlacementDecidedByAgentModelChangeRequests_CollectionChanged;
                }
                return _placementDecidedByAgentModelChangeRequests;
            }
            private set
            {
                if (_placementDecidedByAgentModelChangeRequests != null)
                {
                    _placementDecidedByAgentModelChangeRequests.CollectionChanged -= PlacementDecidedByAgentModelChangeRequests_CollectionChanged;
                }
                _placementDecidedByAgentModelChangeRequests = value;
                if (_placementDecidedByAgentModelChangeRequests != null)
                {
                    _placementDecidedByAgentModelChangeRequests.CollectionChanged += PlacementDecidedByAgentModelChangeRequests_CollectionChanged;
                }
            }
        }

        private void PlacementDecidedByAgentModelChangeRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeRequest>())
                {
                    item.PlacementDecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ChangeImpactFinding> _changeImpactFindings;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ChangeImpactFinding> ChangeImpactFindings
        {
            get
            {
                if (_changeImpactFindings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeImpactFindings - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _changeImpactFindings = new ObservableCollection<ChangeImpactFinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeImpactFindings.Where(x => x.FoundByAgent == this.AgentId).ToList<ChangeImpactFinding>();
                        _changeImpactFindings = new ObservableCollection<ChangeImpactFinding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeImpactFindings.CollectionChanged += ChangeImpactFindings_CollectionChanged;
                }
                return _changeImpactFindings;
            }
            private set
            {
                if (_changeImpactFindings != null)
                {
                    _changeImpactFindings.CollectionChanged -= ChangeImpactFindings_CollectionChanged;
                }
                _changeImpactFindings = value;
                if (_changeImpactFindings != null)
                {
                    _changeImpactFindings.CollectionChanged += ChangeImpactFindings_CollectionChanged;
                }
            }
        }

        private void ChangeImpactFindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeImpactFinding>())
                {
                    item.FoundByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ChangeIntegrityCheck> _changeIntegrityChecks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ChangeIntegrityCheck> ChangeIntegrityChecks
        {
            get
            {
                if (_changeIntegrityChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeIntegrityChecks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _changeIntegrityChecks = new ObservableCollection<ChangeIntegrityCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeIntegrityChecks.Where(x => x.CheckedByAgent == this.AgentId).ToList<ChangeIntegrityCheck>();
                        _changeIntegrityChecks = new ObservableCollection<ChangeIntegrityCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeIntegrityChecks.CollectionChanged += ChangeIntegrityChecks_CollectionChanged;
                }
                return _changeIntegrityChecks;
            }
            private set
            {
                if (_changeIntegrityChecks != null)
                {
                    _changeIntegrityChecks.CollectionChanged -= ChangeIntegrityChecks_CollectionChanged;
                }
                _changeIntegrityChecks = value;
                if (_changeIntegrityChecks != null)
                {
                    _changeIntegrityChecks.CollectionChanged += ChangeIntegrityChecks_CollectionChanged;
                }
            }
        }

        private void ChangeIntegrityChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeIntegrityCheck>())
                {
                    item.CheckedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ChangeObjection> _raisedByAgentChangeObjections;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ChangeObjection> RaisedByAgentChangeObjections
        {
            get
            {
                if (_raisedByAgentChangeObjections == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RaisedByAgentChangeObjections - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _raisedByAgentChangeObjections = new ObservableCollection<ChangeObjection>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeObjections.Where(x => x.RaisedByAgent == this.AgentId).ToList<ChangeObjection>();
                        _raisedByAgentChangeObjections = new ObservableCollection<ChangeObjection>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _raisedByAgentChangeObjections.CollectionChanged += RaisedByAgentChangeObjections_CollectionChanged;
                }
                return _raisedByAgentChangeObjections;
            }
            private set
            {
                if (_raisedByAgentChangeObjections != null)
                {
                    _raisedByAgentChangeObjections.CollectionChanged -= RaisedByAgentChangeObjections_CollectionChanged;
                }
                _raisedByAgentChangeObjections = value;
                if (_raisedByAgentChangeObjections != null)
                {
                    _raisedByAgentChangeObjections.CollectionChanged += RaisedByAgentChangeObjections_CollectionChanged;
                }
            }
        }

        private void RaisedByAgentChangeObjections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeObjection>())
                {
                    item.RaisedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ChangeObjection> _resolvedByAgentChangeObjections;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ChangeObjection> ResolvedByAgentChangeObjections
        {
            get
            {
                if (_resolvedByAgentChangeObjections == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ResolvedByAgentChangeObjections - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _resolvedByAgentChangeObjections = new ObservableCollection<ChangeObjection>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeObjections.Where(x => x.ResolvedByAgent == this.AgentId).ToList<ChangeObjection>();
                        _resolvedByAgentChangeObjections = new ObservableCollection<ChangeObjection>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _resolvedByAgentChangeObjections.CollectionChanged += ResolvedByAgentChangeObjections_CollectionChanged;
                }
                return _resolvedByAgentChangeObjections;
            }
            private set
            {
                if (_resolvedByAgentChangeObjections != null)
                {
                    _resolvedByAgentChangeObjections.CollectionChanged -= ResolvedByAgentChangeObjections_CollectionChanged;
                }
                _resolvedByAgentChangeObjections = value;
                if (_resolvedByAgentChangeObjections != null)
                {
                    _resolvedByAgentChangeObjections.CollectionChanged += ResolvedByAgentChangeObjections_CollectionChanged;
                }
            }
        }

        private void ResolvedByAgentChangeObjections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeObjection>())
                {
                    item.ResolvedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StalenessQueryRun> _stalenessQueryRuns;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<StalenessQueryRun> StalenessQueryRuns
        {
            get
            {
                if (_stalenessQueryRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StalenessQueryRuns - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _stalenessQueryRuns = new ObservableCollection<StalenessQueryRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.StalenessQueryRuns.Where(x => x.RanByAgent == this.AgentId).ToList<StalenessQueryRun>();
                        _stalenessQueryRuns = new ObservableCollection<StalenessQueryRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stalenessQueryRuns.CollectionChanged += StalenessQueryRuns_CollectionChanged;
                }
                return _stalenessQueryRuns;
            }
            private set
            {
                if (_stalenessQueryRuns != null)
                {
                    _stalenessQueryRuns.CollectionChanged -= StalenessQueryRuns_CollectionChanged;
                }
                _stalenessQueryRuns = value;
                if (_stalenessQueryRuns != null)
                {
                    _stalenessQueryRuns.CollectionChanged += StalenessQueryRuns_CollectionChanged;
                }
            }
        }

        private void StalenessQueryRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StalenessQueryRun>())
                {
                    item.RanByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExternalDependencyRevision> _externalDependencyRevisions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ExternalDependencyRevision> ExternalDependencyRevisions
        {
            get
            {
                if (_externalDependencyRevisions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExternalDependencyRevisions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _externalDependencyRevisions = new ObservableCollection<ExternalDependencyRevision>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExternalDependencyRevisions.Where(x => x.TrackedByAgent == this.AgentId).ToList<ExternalDependencyRevision>();
                        _externalDependencyRevisions = new ObservableCollection<ExternalDependencyRevision>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _externalDependencyRevisions.CollectionChanged += ExternalDependencyRevisions_CollectionChanged;
                }
                return _externalDependencyRevisions;
            }
            private set
            {
                if (_externalDependencyRevisions != null)
                {
                    _externalDependencyRevisions.CollectionChanged -= ExternalDependencyRevisions_CollectionChanged;
                }
                _externalDependencyRevisions = value;
                if (_externalDependencyRevisions != null)
                {
                    _externalDependencyRevisions.CollectionChanged += ExternalDependencyRevisions_CollectionChanged;
                }
            }
        }

        private void ExternalDependencyRevisions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExternalDependencyRevision>())
                {
                    item.TrackedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StakeholderQuestion> _askedByAgentStakeholderQuestions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<StakeholderQuestion> AskedByAgentStakeholderQuestions
        {
            get
            {
                if (_askedByAgentStakeholderQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AskedByAgentStakeholderQuestions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _askedByAgentStakeholderQuestions = new ObservableCollection<StakeholderQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderQuestions.Where(x => x.AskedByAgent == this.AgentId).ToList<StakeholderQuestion>();
                        _askedByAgentStakeholderQuestions = new ObservableCollection<StakeholderQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _askedByAgentStakeholderQuestions.CollectionChanged += AskedByAgentStakeholderQuestions_CollectionChanged;
                }
                return _askedByAgentStakeholderQuestions;
            }
            private set
            {
                if (_askedByAgentStakeholderQuestions != null)
                {
                    _askedByAgentStakeholderQuestions.CollectionChanged -= AskedByAgentStakeholderQuestions_CollectionChanged;
                }
                _askedByAgentStakeholderQuestions = value;
                if (_askedByAgentStakeholderQuestions != null)
                {
                    _askedByAgentStakeholderQuestions.CollectionChanged += AskedByAgentStakeholderQuestions_CollectionChanged;
                }
            }
        }

        private void AskedByAgentStakeholderQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderQuestion>())
                {
                    item.AskedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StakeholderQuestion> _answeredByAgentStakeholderQuestions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<StakeholderQuestion> AnsweredByAgentStakeholderQuestions
        {
            get
            {
                if (_answeredByAgentStakeholderQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnsweredByAgentStakeholderQuestions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _answeredByAgentStakeholderQuestions = new ObservableCollection<StakeholderQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderQuestions.Where(x => x.AnsweredByAgent == this.AgentId).ToList<StakeholderQuestion>();
                        _answeredByAgentStakeholderQuestions = new ObservableCollection<StakeholderQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answeredByAgentStakeholderQuestions.CollectionChanged += AnsweredByAgentStakeholderQuestions_CollectionChanged;
                }
                return _answeredByAgentStakeholderQuestions;
            }
            private set
            {
                if (_answeredByAgentStakeholderQuestions != null)
                {
                    _answeredByAgentStakeholderQuestions.CollectionChanged -= AnsweredByAgentStakeholderQuestions_CollectionChanged;
                }
                _answeredByAgentStakeholderQuestions = value;
                if (_answeredByAgentStakeholderQuestions != null)
                {
                    _answeredByAgentStakeholderQuestions.CollectionChanged += AnsweredByAgentStakeholderQuestions_CollectionChanged;
                }
            }
        }

        private void AnsweredByAgentStakeholderQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderQuestion>())
                {
                    item.AnsweredByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<StakeholderQuestion> _triagedByAgentStakeholderQuestions;

        [InverseProperty("AgentRefRef")]
        public virtual ObservableCollection<StakeholderQuestion> TriagedByAgentStakeholderQuestions
        {
            get
            {
                if (_triagedByAgentStakeholderQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TriagedByAgentStakeholderQuestions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _triagedByAgentStakeholderQuestions = new ObservableCollection<StakeholderQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderQuestions.Where(x => x.TriagedByAgent == this.AgentId).ToList<StakeholderQuestion>();
                        _triagedByAgentStakeholderQuestions = new ObservableCollection<StakeholderQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _triagedByAgentStakeholderQuestions.CollectionChanged += TriagedByAgentStakeholderQuestions_CollectionChanged;
                }
                return _triagedByAgentStakeholderQuestions;
            }
            private set
            {
                if (_triagedByAgentStakeholderQuestions != null)
                {
                    _triagedByAgentStakeholderQuestions.CollectionChanged -= TriagedByAgentStakeholderQuestions_CollectionChanged;
                }
                _triagedByAgentStakeholderQuestions = value;
                if (_triagedByAgentStakeholderQuestions != null)
                {
                    _triagedByAgentStakeholderQuestions.CollectionChanged += TriagedByAgentStakeholderQuestions_CollectionChanged;
                }
            }
        }

        private void TriagedByAgentStakeholderQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderQuestion>())
                {
                    item.TriagedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelExpansionRequest> _requestedByAgentModelExpansionRequests;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ModelExpansionRequest> RequestedByAgentModelExpansionRequests
        {
            get
            {
                if (_requestedByAgentModelExpansionRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequestedByAgentModelExpansionRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _requestedByAgentModelExpansionRequests = new ObservableCollection<ModelExpansionRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelExpansionRequests.Where(x => x.RequestedByAgent == this.AgentId).ToList<ModelExpansionRequest>();
                        _requestedByAgentModelExpansionRequests = new ObservableCollection<ModelExpansionRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requestedByAgentModelExpansionRequests.CollectionChanged += RequestedByAgentModelExpansionRequests_CollectionChanged;
                }
                return _requestedByAgentModelExpansionRequests;
            }
            private set
            {
                if (_requestedByAgentModelExpansionRequests != null)
                {
                    _requestedByAgentModelExpansionRequests.CollectionChanged -= RequestedByAgentModelExpansionRequests_CollectionChanged;
                }
                _requestedByAgentModelExpansionRequests = value;
                if (_requestedByAgentModelExpansionRequests != null)
                {
                    _requestedByAgentModelExpansionRequests.CollectionChanged += RequestedByAgentModelExpansionRequests_CollectionChanged;
                }
            }
        }

        private void RequestedByAgentModelExpansionRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelExpansionRequest>())
                {
                    item.RequestedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelExpansionRequest> _decidedByAgentModelExpansionRequests;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ModelExpansionRequest> DecidedByAgentModelExpansionRequests
        {
            get
            {
                if (_decidedByAgentModelExpansionRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DecidedByAgentModelExpansionRequests - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _decidedByAgentModelExpansionRequests = new ObservableCollection<ModelExpansionRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelExpansionRequests.Where(x => x.DecidedByAgent == this.AgentId).ToList<ModelExpansionRequest>();
                        _decidedByAgentModelExpansionRequests = new ObservableCollection<ModelExpansionRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _decidedByAgentModelExpansionRequests.CollectionChanged += DecidedByAgentModelExpansionRequests_CollectionChanged;
                }
                return _decidedByAgentModelExpansionRequests;
            }
            private set
            {
                if (_decidedByAgentModelExpansionRequests != null)
                {
                    _decidedByAgentModelExpansionRequests.CollectionChanged -= DecidedByAgentModelExpansionRequests_CollectionChanged;
                }
                _decidedByAgentModelExpansionRequests = value;
                if (_decidedByAgentModelExpansionRequests != null)
                {
                    _decidedByAgentModelExpansionRequests.CollectionChanged += DecidedByAgentModelExpansionRequests_CollectionChanged;
                }
            }
        }

        private void DecidedByAgentModelExpansionRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelExpansionRequest>())
                {
                    item.DecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<CompetencyQuestionReview> _competencyQuestionReviews;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<CompetencyQuestionReview> CompetencyQuestionReviews
        {
            get
            {
                if (_competencyQuestionReviews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionReviews - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _competencyQuestionReviews = new ObservableCollection<CompetencyQuestionReview>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionReviews.Where(x => x.ReviewedByAgent == this.AgentId).ToList<CompetencyQuestionReview>();
                        _competencyQuestionReviews = new ObservableCollection<CompetencyQuestionReview>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestionReviews.CollectionChanged += CompetencyQuestionReviews_CollectionChanged;
                }
                return _competencyQuestionReviews;
            }
            private set
            {
                if (_competencyQuestionReviews != null)
                {
                    _competencyQuestionReviews.CollectionChanged -= CompetencyQuestionReviews_CollectionChanged;
                }
                _competencyQuestionReviews = value;
                if (_competencyQuestionReviews != null)
                {
                    _competencyQuestionReviews.CollectionChanged += CompetencyQuestionReviews_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestionReviews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestionReview>())
                {
                    item.ReviewedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<QualityAssessment> _qualityAssessments;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<QualityAssessment> QualityAssessments
        {
            get
            {
                if (_qualityAssessments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access QualityAssessments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _qualityAssessments = new ObservableCollection<QualityAssessment>();
                    }
                    else
                    {
                        var items = base.SoAContext.QualityAssessments.Where(x => x.AssessedByAgent == this.AgentId).ToList<QualityAssessment>();
                        _qualityAssessments = new ObservableCollection<QualityAssessment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _qualityAssessments.CollectionChanged += QualityAssessments_CollectionChanged;
                }
                return _qualityAssessments;
            }
            private set
            {
                if (_qualityAssessments != null)
                {
                    _qualityAssessments.CollectionChanged -= QualityAssessments_CollectionChanged;
                }
                _qualityAssessments = value;
                if (_qualityAssessments != null)
                {
                    _qualityAssessments.CollectionChanged += QualityAssessments_CollectionChanged;
                }
            }
        }

        private void QualityAssessments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<QualityAssessment>())
                {
                    item.AssessedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<TermDefinition> _draftedByAgentTermDefinitions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<TermDefinition> DraftedByAgentTermDefinitions
        {
            get
            {
                if (_draftedByAgentTermDefinitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DraftedByAgentTermDefinitions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _draftedByAgentTermDefinitions = new ObservableCollection<TermDefinition>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermDefinitions.Where(x => x.DraftedByAgent == this.AgentId).ToList<TermDefinition>();
                        _draftedByAgentTermDefinitions = new ObservableCollection<TermDefinition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _draftedByAgentTermDefinitions.CollectionChanged += DraftedByAgentTermDefinitions_CollectionChanged;
                }
                return _draftedByAgentTermDefinitions;
            }
            private set
            {
                if (_draftedByAgentTermDefinitions != null)
                {
                    _draftedByAgentTermDefinitions.CollectionChanged -= DraftedByAgentTermDefinitions_CollectionChanged;
                }
                _draftedByAgentTermDefinitions = value;
                if (_draftedByAgentTermDefinitions != null)
                {
                    _draftedByAgentTermDefinitions.CollectionChanged += DraftedByAgentTermDefinitions_CollectionChanged;
                }
            }
        }

        private void DraftedByAgentTermDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermDefinition>())
                {
                    item.DraftedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<TermDefinition> _revisedByAgentTermDefinitions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<TermDefinition> RevisedByAgentTermDefinitions
        {
            get
            {
                if (_revisedByAgentTermDefinitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RevisedByAgentTermDefinitions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _revisedByAgentTermDefinitions = new ObservableCollection<TermDefinition>();
                    }
                    else
                    {
                        var items = base.SoAContext.TermDefinitions.Where(x => x.RevisedByAgent == this.AgentId).ToList<TermDefinition>();
                        _revisedByAgentTermDefinitions = new ObservableCollection<TermDefinition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _revisedByAgentTermDefinitions.CollectionChanged += RevisedByAgentTermDefinitions_CollectionChanged;
                }
                return _revisedByAgentTermDefinitions;
            }
            private set
            {
                if (_revisedByAgentTermDefinitions != null)
                {
                    _revisedByAgentTermDefinitions.CollectionChanged -= RevisedByAgentTermDefinitions_CollectionChanged;
                }
                _revisedByAgentTermDefinitions = value;
                if (_revisedByAgentTermDefinitions != null)
                {
                    _revisedByAgentTermDefinitions.CollectionChanged += RevisedByAgentTermDefinitions_CollectionChanged;
                }
            }
        }

        private void RevisedByAgentTermDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TermDefinition>())
                {
                    item.RevisedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelProposal> _proposedByAgentModelProposals;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ModelProposal> ProposedByAgentModelProposals
        {
            get
            {
                if (_proposedByAgentModelProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProposedByAgentModelProposals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _proposedByAgentModelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.ProposedByAgent == this.AgentId).ToList<ModelProposal>();
                        _proposedByAgentModelProposals = new ObservableCollection<ModelProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _proposedByAgentModelProposals.CollectionChanged += ProposedByAgentModelProposals_CollectionChanged;
                }
                return _proposedByAgentModelProposals;
            }
            private set
            {
                if (_proposedByAgentModelProposals != null)
                {
                    _proposedByAgentModelProposals.CollectionChanged -= ProposedByAgentModelProposals_CollectionChanged;
                }
                _proposedByAgentModelProposals = value;
                if (_proposedByAgentModelProposals != null)
                {
                    _proposedByAgentModelProposals.CollectionChanged += ProposedByAgentModelProposals_CollectionChanged;
                }
            }
        }

        private void ProposedByAgentModelProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelProposal>())
                {
                    item.ProposedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelProposal> _reviewedByAgentModelProposals;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ModelProposal> ReviewedByAgentModelProposals
        {
            get
            {
                if (_reviewedByAgentModelProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ReviewedByAgentModelProposals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _reviewedByAgentModelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.ReviewedByAgent == this.AgentId).ToList<ModelProposal>();
                        _reviewedByAgentModelProposals = new ObservableCollection<ModelProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _reviewedByAgentModelProposals.CollectionChanged += ReviewedByAgentModelProposals_CollectionChanged;
                }
                return _reviewedByAgentModelProposals;
            }
            private set
            {
                if (_reviewedByAgentModelProposals != null)
                {
                    _reviewedByAgentModelProposals.CollectionChanged -= ReviewedByAgentModelProposals_CollectionChanged;
                }
                _reviewedByAgentModelProposals = value;
                if (_reviewedByAgentModelProposals != null)
                {
                    _reviewedByAgentModelProposals.CollectionChanged += ReviewedByAgentModelProposals_CollectionChanged;
                }
            }
        }

        private void ReviewedByAgentModelProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelProposal>())
                {
                    item.ReviewedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelProposal> _committedByAgentModelProposals;

        [InverseProperty("AgentRefRef")]
        public virtual ObservableCollection<ModelProposal> CommittedByAgentModelProposals
        {
            get
            {
                if (_committedByAgentModelProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommittedByAgentModelProposals - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _committedByAgentModelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.CommittedByAgent == this.AgentId).ToList<ModelProposal>();
                        _committedByAgentModelProposals = new ObservableCollection<ModelProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _committedByAgentModelProposals.CollectionChanged += CommittedByAgentModelProposals_CollectionChanged;
                }
                return _committedByAgentModelProposals;
            }
            private set
            {
                if (_committedByAgentModelProposals != null)
                {
                    _committedByAgentModelProposals.CollectionChanged -= CommittedByAgentModelProposals_CollectionChanged;
                }
                _committedByAgentModelProposals = value;
                if (_committedByAgentModelProposals != null)
                {
                    _committedByAgentModelProposals.CollectionChanged += CommittedByAgentModelProposals_CollectionChanged;
                }
            }
        }

        private void CommittedByAgentModelProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelProposal>())
                {
                    item.CommittedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProcessDesignDecision> _processDesignDecisions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProcessDesignDecision> ProcessDesignDecisions
        {
            get
            {
                if (_processDesignDecisions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessDesignDecisions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _processDesignDecisions = new ObservableCollection<ProcessDesignDecision>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessDesignDecisions.Where(x => x.DecidedByAgent == this.AgentId).ToList<ProcessDesignDecision>();
                        _processDesignDecisions = new ObservableCollection<ProcessDesignDecision>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processDesignDecisions.CollectionChanged += ProcessDesignDecisions_CollectionChanged;
                }
                return _processDesignDecisions;
            }
            private set
            {
                if (_processDesignDecisions != null)
                {
                    _processDesignDecisions.CollectionChanged -= ProcessDesignDecisions_CollectionChanged;
                }
                _processDesignDecisions = value;
                if (_processDesignDecisions != null)
                {
                    _processDesignDecisions.CollectionChanged += ProcessDesignDecisions_CollectionChanged;
                }
            }
        }

        private void ProcessDesignDecisions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessDesignDecision>())
                {
                    item.DecidedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ModelChangeLogEntry> ModelChangeLogEntries
        {
            get
            {
                if (_modelChangeLogEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.ChangedByAgent == this.AgentId).ToList<ModelChangeLogEntry>();
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelChangeLogEntries.CollectionChanged += ModelChangeLogEntries_CollectionChanged;
                }
                return _modelChangeLogEntries;
            }
            private set
            {
                if (_modelChangeLogEntries != null)
                {
                    _modelChangeLogEntries.CollectionChanged -= ModelChangeLogEntries_CollectionChanged;
                }
                _modelChangeLogEntries = value;
                if (_modelChangeLogEntries != null)
                {
                    _modelChangeLogEntries.CollectionChanged += ModelChangeLogEntries_CollectionChanged;
                }
            }
        }

        private void ModelChangeLogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeLogEntry>())
                {
                    item.ChangedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<DriftObservation> _driftObservations;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<DriftObservation> DriftObservations
        {
            get
            {
                if (_driftObservations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DriftObservations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _driftObservations = new ObservableCollection<DriftObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.DriftObservations.Where(x => x.ObservedByAgent == this.AgentId).ToList<DriftObservation>();
                        _driftObservations = new ObservableCollection<DriftObservation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _driftObservations.CollectionChanged += DriftObservations_CollectionChanged;
                }
                return _driftObservations;
            }
            private set
            {
                if (_driftObservations != null)
                {
                    _driftObservations.CollectionChanged -= DriftObservations_CollectionChanged;
                }
                _driftObservations = value;
                if (_driftObservations != null)
                {
                    _driftObservations.CollectionChanged += DriftObservations_CollectionChanged;
                }
            }
        }

        private void DriftObservations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DriftObservation>())
                {
                    item.ObservedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeAudit> _knowledgeAudits;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeAudits - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeAudits = new ObservableCollection<KnowledgeAudit>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeAudits.Where(x => x.ConductedByAgent == this.AgentId).ToList<KnowledgeAudit>();
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
                    item.ConductedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeCaptureInitiatif> _knowledgeCaptureInitiatives;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeCaptureInitiatives - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeCaptureInitiatives = new ObservableCollection<KnowledgeCaptureInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeCaptureInitiatives.Where(x => x.LeadAgent == this.AgentId).ToList<KnowledgeCaptureInitiatif>();
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
                    item.LeadAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeWorkforcePosition> _knowledgeWorkforcePositions;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeWorkforcePositions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeWorkforcePositions = new ObservableCollection<KnowledgeWorkforcePosition>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeWorkforcePositions.Where(x => x.FilledByAgent == this.AgentId).ToList<KnowledgeWorkforcePosition>();
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
                    item.FilledByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiAgentAccountability> _aiAgentAiAgentAccountabilities;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AiAgentAccountability> AiAgentAiAgentAccountabilities
        {
            get
            {
                if (_aiAgentAiAgentAccountabilities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAgentAiAgentAccountabilities - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _aiAgentAiAgentAccountabilities = new ObservableCollection<AiAgentAccountability>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAgentAccountabilities.Where(x => x.AiAgent == this.AgentId).ToList<AiAgentAccountability>();
                        _aiAgentAiAgentAccountabilities = new ObservableCollection<AiAgentAccountability>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aiAgentAiAgentAccountabilities.CollectionChanged += AiAgentAiAgentAccountabilities_CollectionChanged;
                }
                return _aiAgentAiAgentAccountabilities;
            }
            private set
            {
                if (_aiAgentAiAgentAccountabilities != null)
                {
                    _aiAgentAiAgentAccountabilities.CollectionChanged -= AiAgentAiAgentAccountabilities_CollectionChanged;
                }
                _aiAgentAiAgentAccountabilities = value;
                if (_aiAgentAiAgentAccountabilities != null)
                {
                    _aiAgentAiAgentAccountabilities.CollectionChanged += AiAgentAiAgentAccountabilities_CollectionChanged;
                }
            }
        }

        private void AiAgentAiAgentAccountabilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAgentAccountability>())
                {
                    item.AiAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AiAgentAccountability> _accountableAgentAiAgentAccountabilities;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AiAgentAccountability> AccountableAgentAiAgentAccountabilities
        {
            get
            {
                if (_accountableAgentAiAgentAccountabilities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AccountableAgentAiAgentAccountabilities - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _accountableAgentAiAgentAccountabilities = new ObservableCollection<AiAgentAccountability>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAgentAccountabilities.Where(x => x.AccountableAgent == this.AgentId).ToList<AiAgentAccountability>();
                        _accountableAgentAiAgentAccountabilities = new ObservableCollection<AiAgentAccountability>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _accountableAgentAiAgentAccountabilities.CollectionChanged += AccountableAgentAiAgentAccountabilities_CollectionChanged;
                }
                return _accountableAgentAiAgentAccountabilities;
            }
            private set
            {
                if (_accountableAgentAiAgentAccountabilities != null)
                {
                    _accountableAgentAiAgentAccountabilities.CollectionChanged -= AccountableAgentAiAgentAccountabilities_CollectionChanged;
                }
                _accountableAgentAiAgentAccountabilities = value;
                if (_accountableAgentAiAgentAccountabilities != null)
                {
                    _accountableAgentAiAgentAccountabilities.CollectionChanged += AccountableAgentAiAgentAccountabilities_CollectionChanged;
                }
            }
        }

        private void AccountableAgentAiAgentAccountabilities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AiAgentAccountability>())
                {
                    item.AccountableAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentUpgradeAssessment> _currentAgentAgentUpgradeAssessments;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AgentUpgradeAssessment> CurrentAgentAgentUpgradeAssessments
        {
            get
            {
                if (_currentAgentAgentUpgradeAssessments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CurrentAgentAgentUpgradeAssessments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _currentAgentAgentUpgradeAssessments = new ObservableCollection<AgentUpgradeAssessment>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentUpgradeAssessments.Where(x => x.CurrentAgent == this.AgentId).ToList<AgentUpgradeAssessment>();
                        _currentAgentAgentUpgradeAssessments = new ObservableCollection<AgentUpgradeAssessment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _currentAgentAgentUpgradeAssessments.CollectionChanged += CurrentAgentAgentUpgradeAssessments_CollectionChanged;
                }
                return _currentAgentAgentUpgradeAssessments;
            }
            private set
            {
                if (_currentAgentAgentUpgradeAssessments != null)
                {
                    _currentAgentAgentUpgradeAssessments.CollectionChanged -= CurrentAgentAgentUpgradeAssessments_CollectionChanged;
                }
                _currentAgentAgentUpgradeAssessments = value;
                if (_currentAgentAgentUpgradeAssessments != null)
                {
                    _currentAgentAgentUpgradeAssessments.CollectionChanged += CurrentAgentAgentUpgradeAssessments_CollectionChanged;
                }
            }
        }

        private void CurrentAgentAgentUpgradeAssessments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentUpgradeAssessment>())
                {
                    item.CurrentAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentUpgradeAssessment> _candidateAgentAgentUpgradeAssessments;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<AgentUpgradeAssessment> CandidateAgentAgentUpgradeAssessments
        {
            get
            {
                if (_candidateAgentAgentUpgradeAssessments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CandidateAgentAgentUpgradeAssessments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _candidateAgentAgentUpgradeAssessments = new ObservableCollection<AgentUpgradeAssessment>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentUpgradeAssessments.Where(x => x.CandidateAgent == this.AgentId).ToList<AgentUpgradeAssessment>();
                        _candidateAgentAgentUpgradeAssessments = new ObservableCollection<AgentUpgradeAssessment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _candidateAgentAgentUpgradeAssessments.CollectionChanged += CandidateAgentAgentUpgradeAssessments_CollectionChanged;
                }
                return _candidateAgentAgentUpgradeAssessments;
            }
            private set
            {
                if (_candidateAgentAgentUpgradeAssessments != null)
                {
                    _candidateAgentAgentUpgradeAssessments.CollectionChanged -= CandidateAgentAgentUpgradeAssessments_CollectionChanged;
                }
                _candidateAgentAgentUpgradeAssessments = value;
                if (_candidateAgentAgentUpgradeAssessments != null)
                {
                    _candidateAgentAgentUpgradeAssessments.CollectionChanged += CandidateAgentAgentUpgradeAssessments_CollectionChanged;
                }
            }
        }

        private void CandidateAgentAgentUpgradeAssessments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentUpgradeAssessment>())
                {
                    item.CandidateAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AgentUpgradeAssessment> _assessedByAgentAgentUpgradeAssessments;

        [InverseProperty("AgentRefRef")]
        public virtual ObservableCollection<AgentUpgradeAssessment> AssessedByAgentAgentUpgradeAssessments
        {
            get
            {
                if (_assessedByAgentAgentUpgradeAssessments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssessedByAgentAgentUpgradeAssessments - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _assessedByAgentAgentUpgradeAssessments = new ObservableCollection<AgentUpgradeAssessment>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentUpgradeAssessments.Where(x => x.AssessedByAgent == this.AgentId).ToList<AgentUpgradeAssessment>();
                        _assessedByAgentAgentUpgradeAssessments = new ObservableCollection<AgentUpgradeAssessment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assessedByAgentAgentUpgradeAssessments.CollectionChanged += AssessedByAgentAgentUpgradeAssessments_CollectionChanged;
                }
                return _assessedByAgentAgentUpgradeAssessments;
            }
            private set
            {
                if (_assessedByAgentAgentUpgradeAssessments != null)
                {
                    _assessedByAgentAgentUpgradeAssessments.CollectionChanged -= AssessedByAgentAgentUpgradeAssessments_CollectionChanged;
                }
                _assessedByAgentAgentUpgradeAssessments = value;
                if (_assessedByAgentAgentUpgradeAssessments != null)
                {
                    _assessedByAgentAgentUpgradeAssessments.CollectionChanged += AssessedByAgentAgentUpgradeAssessments_CollectionChanged;
                }
            }
        }

        private void AssessedByAgentAgentUpgradeAssessments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentUpgradeAssessment>())
                {
                    item.AssessedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RoleAssignmentUpdateTask> _roleAssignmentUpdateTasks;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<RoleAssignmentUpdateTask> RoleAssignmentUpdateTasks
        {
            get
            {
                if (_roleAssignmentUpdateTasks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignmentUpdateTasks - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>();
                    }
                    else
                    {
                        var items = base.SoAContext.RoleAssignmentUpdateTasks.Where(x => x.TriggeredByAgent == this.AgentId).ToList<RoleAssignmentUpdateTask>();
                        _roleAssignmentUpdateTasks = new ObservableCollection<RoleAssignmentUpdateTask>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _roleAssignmentUpdateTasks.CollectionChanged += RoleAssignmentUpdateTasks_CollectionChanged;
                }
                return _roleAssignmentUpdateTasks;
            }
            private set
            {
                if (_roleAssignmentUpdateTasks != null)
                {
                    _roleAssignmentUpdateTasks.CollectionChanged -= RoleAssignmentUpdateTasks_CollectionChanged;
                }
                _roleAssignmentUpdateTasks = value;
                if (_roleAssignmentUpdateTasks != null)
                {
                    _roleAssignmentUpdateTasks.CollectionChanged += RoleAssignmentUpdateTasks_CollectionChanged;
                }
            }
        }

        private void RoleAssignmentUpdateTasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RoleAssignmentUpdateTask>())
                {
                    item.TriggeredByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<AssignmentRoutedNotice> _assignmentRoutedNotices;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<AssignmentRoutedNotice> AssignmentRoutedNotices
        {
            get
            {
                if (_assignmentRoutedNotices == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssignmentRoutedNotices - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _assignmentRoutedNotices = new ObservableCollection<AssignmentRoutedNotice>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssignmentRoutedNotices.Where(x => x.RoutedToAgent == this.AgentId).ToList<AssignmentRoutedNotice>();
                        _assignmentRoutedNotices = new ObservableCollection<AssignmentRoutedNotice>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assignmentRoutedNotices.CollectionChanged += AssignmentRoutedNotices_CollectionChanged;
                }
                return _assignmentRoutedNotices;
            }
            private set
            {
                if (_assignmentRoutedNotices != null)
                {
                    _assignmentRoutedNotices.CollectionChanged -= AssignmentRoutedNotices_CollectionChanged;
                }
                _assignmentRoutedNotices = value;
                if (_assignmentRoutedNotices != null)
                {
                    _assignmentRoutedNotices.CollectionChanged += AssignmentRoutedNotices_CollectionChanged;
                }
            }
        }

        private void AssignmentRoutedNotices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssignmentRoutedNotice>())
                {
                    item.RoutedToAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<PractitionerExpertise> _practitionerExpertise;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<PractitionerExpertise> PractitionerExpertise
        {
            get
            {
                if (_practitionerExpertise == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PractitionerExpertise - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>();
                    }
                    else
                    {
                        var items = base.SoAContext.PractitionerExpertise.Where(x => x.Agent == this.AgentId).ToList<PractitionerExpertise>();
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _practitionerExpertise.CollectionChanged += PractitionerExpertise_CollectionChanged;
                }
                return _practitionerExpertise;
            }
            private set
            {
                if (_practitionerExpertise != null)
                {
                    _practitionerExpertise.CollectionChanged -= PractitionerExpertise_CollectionChanged;
                }
                _practitionerExpertise = value;
                if (_practitionerExpertise != null)
                {
                    _practitionerExpertise.CollectionChanged += PractitionerExpertise_CollectionChanged;
                }
            }
        }

        private void PractitionerExpertise_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PractitionerExpertise>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<CriticalIncident> _criticalIncidents;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<CriticalIncident> CriticalIncidents
        {
            get
            {
                if (_criticalIncidents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CriticalIncidents - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _criticalIncidents = new ObservableCollection<CriticalIncident>();
                    }
                    else
                    {
                        var items = base.SoAContext.CriticalIncidents.Where(x => x.Narrator == this.AgentId).ToList<CriticalIncident>();
                        _criticalIncidents = new ObservableCollection<CriticalIncident>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
                return _criticalIncidents;
            }
            private set
            {
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged -= CriticalIncidents_CollectionChanged;
                }
                _criticalIncidents = value;
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
            }
        }

        private void CriticalIncidents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CriticalIncident>())
                {
                    item.Narrator = this.AgentId;
                }
            }
        }

        private ObservableCollection<ObservedAction> _observedActions;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ObservedAction> ObservedActions
        {
            get
            {
                if (_observedActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedActions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _observedActions = new ObservableCollection<ObservedAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedActions.Where(x => x.Practitioner == this.AgentId).ToList<ObservedAction>();
                        _observedActions = new ObservableCollection<ObservedAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
                return _observedActions;
            }
            private set
            {
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged -= ObservedActions_CollectionChanged;
                }
                _observedActions = value;
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
            }
        }

        private void ObservedActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ObservedAction>())
                {
                    item.Practitioner = this.AgentId;
                }
            }
        }

        private ObservableCollection<ElicitationParticipant> _elicitationParticipants;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ElicitationParticipant> ElicitationParticipants
        {
            get
            {
                if (_elicitationParticipants == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ElicitationParticipants - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _elicitationParticipants = new ObservableCollection<ElicitationParticipant>();
                    }
                    else
                    {
                        var items = base.SoAContext.ElicitationParticipants.Where(x => x.Agent == this.AgentId).ToList<ElicitationParticipant>();
                        _elicitationParticipants = new ObservableCollection<ElicitationParticipant>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _elicitationParticipants.CollectionChanged += ElicitationParticipants_CollectionChanged;
                }
                return _elicitationParticipants;
            }
            private set
            {
                if (_elicitationParticipants != null)
                {
                    _elicitationParticipants.CollectionChanged -= ElicitationParticipants_CollectionChanged;
                }
                _elicitationParticipants = value;
                if (_elicitationParticipants != null)
                {
                    _elicitationParticipants.CollectionChanged += ElicitationParticipants_CollectionChanged;
                }
            }
        }

        private void ElicitationParticipants_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ElicitationParticipant>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RepresentationReview> _representationReviews;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<RepresentationReview> RepresentationReviews
        {
            get
            {
                if (_representationReviews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RepresentationReviews - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _representationReviews = new ObservableCollection<RepresentationReview>();
                    }
                    else
                    {
                        var items = base.SoAContext.RepresentationReviews.Where(x => x.ReviewerAgent == this.AgentId).ToList<RepresentationReview>();
                        _representationReviews = new ObservableCollection<RepresentationReview>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _representationReviews.CollectionChanged += RepresentationReviews_CollectionChanged;
                }
                return _representationReviews;
            }
            private set
            {
                if (_representationReviews != null)
                {
                    _representationReviews.CollectionChanged -= RepresentationReviews_CollectionChanged;
                }
                _representationReviews = value;
                if (_representationReviews != null)
                {
                    _representationReviews.CollectionChanged += RepresentationReviews_CollectionChanged;
                }
            }
        }

        private void RepresentationReviews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RepresentationReview>())
                {
                    item.ReviewerAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<WorkflowViewDivergence> _holderAWorkflowViewDivergences;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<WorkflowViewDivergence> HolderAWorkflowViewDivergences
        {
            get
            {
                if (_holderAWorkflowViewDivergences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HolderAWorkflowViewDivergences - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _holderAWorkflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowViewDivergences.Where(x => x.HolderA == this.AgentId).ToList<WorkflowViewDivergence>();
                        _holderAWorkflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _holderAWorkflowViewDivergences.CollectionChanged += HolderAWorkflowViewDivergences_CollectionChanged;
                }
                return _holderAWorkflowViewDivergences;
            }
            private set
            {
                if (_holderAWorkflowViewDivergences != null)
                {
                    _holderAWorkflowViewDivergences.CollectionChanged -= HolderAWorkflowViewDivergences_CollectionChanged;
                }
                _holderAWorkflowViewDivergences = value;
                if (_holderAWorkflowViewDivergences != null)
                {
                    _holderAWorkflowViewDivergences.CollectionChanged += HolderAWorkflowViewDivergences_CollectionChanged;
                }
            }
        }

        private void HolderAWorkflowViewDivergences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowViewDivergence>())
                {
                    item.HolderA = this.AgentId;
                }
            }
        }

        private ObservableCollection<WorkflowViewDivergence> _holderBWorkflowViewDivergences;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<WorkflowViewDivergence> HolderBWorkflowViewDivergences
        {
            get
            {
                if (_holderBWorkflowViewDivergences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HolderBWorkflowViewDivergences - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _holderBWorkflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowViewDivergences.Where(x => x.HolderB == this.AgentId).ToList<WorkflowViewDivergence>();
                        _holderBWorkflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _holderBWorkflowViewDivergences.CollectionChanged += HolderBWorkflowViewDivergences_CollectionChanged;
                }
                return _holderBWorkflowViewDivergences;
            }
            private set
            {
                if (_holderBWorkflowViewDivergences != null)
                {
                    _holderBWorkflowViewDivergences.CollectionChanged -= HolderBWorkflowViewDivergences_CollectionChanged;
                }
                _holderBWorkflowViewDivergences = value;
                if (_holderBWorkflowViewDivergences != null)
                {
                    _holderBWorkflowViewDivergences.CollectionChanged += HolderBWorkflowViewDivergences_CollectionChanged;
                }
            }
        }

        private void HolderBWorkflowViewDivergences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowViewDivergence>())
                {
                    item.HolderB = this.AgentId;
                }
            }
        }

        private ObservableCollection<ExpertCognition> _expertCognitions;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<ExpertCognition> ExpertCognitions
        {
            get
            {
                if (_expertCognitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpertCognitions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _expertCognitions = new ObservableCollection<ExpertCognition>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpertCognitions.Where(x => x.Agent == this.AgentId).ToList<ExpertCognition>();
                        _expertCognitions = new ObservableCollection<ExpertCognition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expertCognitions.CollectionChanged += ExpertCognitions_CollectionChanged;
                }
                return _expertCognitions;
            }
            private set
            {
                if (_expertCognitions != null)
                {
                    _expertCognitions.CollectionChanged -= ExpertCognitions_CollectionChanged;
                }
                _expertCognitions = value;
                if (_expertCognitions != null)
                {
                    _expertCognitions.CollectionChanged += ExpertCognitions_CollectionChanged;
                }
            }
        }

        private void ExpertCognitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpertCognition>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<RepertoryGridConstruct> _repertoryGridConstructs;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<RepertoryGridConstruct> RepertoryGridConstructs
        {
            get
            {
                if (_repertoryGridConstructs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RepertoryGridConstructs - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _repertoryGridConstructs = new ObservableCollection<RepertoryGridConstruct>();
                    }
                    else
                    {
                        var items = base.SoAContext.RepertoryGridConstructs.Where(x => x.Agent == this.AgentId).ToList<RepertoryGridConstruct>();
                        _repertoryGridConstructs = new ObservableCollection<RepertoryGridConstruct>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _repertoryGridConstructs.CollectionChanged += RepertoryGridConstructs_CollectionChanged;
                }
                return _repertoryGridConstructs;
            }
            private set
            {
                if (_repertoryGridConstructs != null)
                {
                    _repertoryGridConstructs.CollectionChanged -= RepertoryGridConstructs_CollectionChanged;
                }
                _repertoryGridConstructs = value;
                if (_repertoryGridConstructs != null)
                {
                    _repertoryGridConstructs.CollectionChanged += RepertoryGridConstructs_CollectionChanged;
                }
            }
        }

        private void RepertoryGridConstructs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RepertoryGridConstruct>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeHolding> _knowledgeHoldings;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeHolding> KnowledgeHoldings
        {
            get
            {
                if (_knowledgeHoldings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeHoldings - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeHoldings.Where(x => x.HolderAgent == this.AgentId).ToList<KnowledgeHolding>();
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeHoldings.CollectionChanged += KnowledgeHoldings_CollectionChanged;
                }
                return _knowledgeHoldings;
            }
            private set
            {
                if (_knowledgeHoldings != null)
                {
                    _knowledgeHoldings.CollectionChanged -= KnowledgeHoldings_CollectionChanged;
                }
                _knowledgeHoldings = value;
                if (_knowledgeHoldings != null)
                {
                    _knowledgeHoldings.CollectionChanged += KnowledgeHoldings_CollectionChanged;
                }
            }
        }

        private void KnowledgeHoldings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeHolding>())
                {
                    item.HolderAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<FragmentCorroboration> _fragmentCorroborations;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<FragmentCorroboration> FragmentCorroborations
        {
            get
            {
                if (_fragmentCorroborations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FragmentCorroborations - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _fragmentCorroborations = new ObservableCollection<FragmentCorroboration>();
                    }
                    else
                    {
                        var items = base.SoAContext.FragmentCorroborations.Where(x => x.Agent == this.AgentId).ToList<FragmentCorroboration>();
                        _fragmentCorroborations = new ObservableCollection<FragmentCorroboration>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fragmentCorroborations.CollectionChanged += FragmentCorroborations_CollectionChanged;
                }
                return _fragmentCorroborations;
            }
            private set
            {
                if (_fragmentCorroborations != null)
                {
                    _fragmentCorroborations.CollectionChanged -= FragmentCorroborations_CollectionChanged;
                }
                _fragmentCorroborations = value;
                if (_fragmentCorroborations != null)
                {
                    _fragmentCorroborations.CollectionChanged += FragmentCorroborations_CollectionChanged;
                }
            }
        }

        private void FragmentCorroborations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FragmentCorroboration>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.HolderAgent == this.AgentId).ToList<KnowHowCarrier>();
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
                    item.HolderAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeTransfer> _fromAgentKnowledgeTransfers;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeTransfer> FromAgentKnowledgeTransfers
        {
            get
            {
                if (_fromAgentKnowledgeTransfers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromAgentKnowledgeTransfers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _fromAgentKnowledgeTransfers = new ObservableCollection<KnowledgeTransfer>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTransfers.Where(x => x.FromAgent == this.AgentId).ToList<KnowledgeTransfer>();
                        _fromAgentKnowledgeTransfers = new ObservableCollection<KnowledgeTransfer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromAgentKnowledgeTransfers.CollectionChanged += FromAgentKnowledgeTransfers_CollectionChanged;
                }
                return _fromAgentKnowledgeTransfers;
            }
            private set
            {
                if (_fromAgentKnowledgeTransfers != null)
                {
                    _fromAgentKnowledgeTransfers.CollectionChanged -= FromAgentKnowledgeTransfers_CollectionChanged;
                }
                _fromAgentKnowledgeTransfers = value;
                if (_fromAgentKnowledgeTransfers != null)
                {
                    _fromAgentKnowledgeTransfers.CollectionChanged += FromAgentKnowledgeTransfers_CollectionChanged;
                }
            }
        }

        private void FromAgentKnowledgeTransfers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTransfer>())
                {
                    item.FromAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeTransfer> _recipientAgentKnowledgeTransfers;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<KnowledgeTransfer> RecipientAgentKnowledgeTransfers
        {
            get
            {
                if (_recipientAgentKnowledgeTransfers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RecipientAgentKnowledgeTransfers - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _recipientAgentKnowledgeTransfers = new ObservableCollection<KnowledgeTransfer>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTransfers.Where(x => x.RecipientAgent == this.AgentId).ToList<KnowledgeTransfer>();
                        _recipientAgentKnowledgeTransfers = new ObservableCollection<KnowledgeTransfer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _recipientAgentKnowledgeTransfers.CollectionChanged += RecipientAgentKnowledgeTransfers_CollectionChanged;
                }
                return _recipientAgentKnowledgeTransfers;
            }
            private set
            {
                if (_recipientAgentKnowledgeTransfers != null)
                {
                    _recipientAgentKnowledgeTransfers.CollectionChanged -= RecipientAgentKnowledgeTransfers_CollectionChanged;
                }
                _recipientAgentKnowledgeTransfers = value;
                if (_recipientAgentKnowledgeTransfers != null)
                {
                    _recipientAgentKnowledgeTransfers.CollectionChanged += RecipientAgentKnowledgeTransfers_CollectionChanged;
                }
            }
        }

        private void RecipientAgentKnowledgeTransfers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTransfer>())
                {
                    item.RecipientAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeRepositoryEntry> _authorAgentKnowledgeRepositoryEntries;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeRepositoryEntry> AuthorAgentKnowledgeRepositoryEntries
        {
            get
            {
                if (_authorAgentKnowledgeRepositoryEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorAgentKnowledgeRepositoryEntries - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _authorAgentKnowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeRepositoryEntries.Where(x => x.AuthorAgent == this.AgentId).ToList<KnowledgeRepositoryEntry>();
                        _authorAgentKnowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorAgentKnowledgeRepositoryEntries.CollectionChanged += AuthorAgentKnowledgeRepositoryEntries_CollectionChanged;
                }
                return _authorAgentKnowledgeRepositoryEntries;
            }
            private set
            {
                if (_authorAgentKnowledgeRepositoryEntries != null)
                {
                    _authorAgentKnowledgeRepositoryEntries.CollectionChanged -= AuthorAgentKnowledgeRepositoryEntries_CollectionChanged;
                }
                _authorAgentKnowledgeRepositoryEntries = value;
                if (_authorAgentKnowledgeRepositoryEntries != null)
                {
                    _authorAgentKnowledgeRepositoryEntries.CollectionChanged += AuthorAgentKnowledgeRepositoryEntries_CollectionChanged;
                }
            }
        }

        private void AuthorAgentKnowledgeRepositoryEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeRepositoryEntry>())
                {
                    item.AuthorAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeRepositoryEntry> _sourceExpertKnowledgeRepositoryEntries;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<KnowledgeRepositoryEntry> SourceExpertKnowledgeRepositoryEntries
        {
            get
            {
                if (_sourceExpertKnowledgeRepositoryEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourceExpertKnowledgeRepositoryEntries - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _sourceExpertKnowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeRepositoryEntries.Where(x => x.SourceExpert == this.AgentId).ToList<KnowledgeRepositoryEntry>();
                        _sourceExpertKnowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sourceExpertKnowledgeRepositoryEntries.CollectionChanged += SourceExpertKnowledgeRepositoryEntries_CollectionChanged;
                }
                return _sourceExpertKnowledgeRepositoryEntries;
            }
            private set
            {
                if (_sourceExpertKnowledgeRepositoryEntries != null)
                {
                    _sourceExpertKnowledgeRepositoryEntries.CollectionChanged -= SourceExpertKnowledgeRepositoryEntries_CollectionChanged;
                }
                _sourceExpertKnowledgeRepositoryEntries = value;
                if (_sourceExpertKnowledgeRepositoryEntries != null)
                {
                    _sourceExpertKnowledgeRepositoryEntries.CollectionChanged += SourceExpertKnowledgeRepositoryEntries_CollectionChanged;
                }
            }
        }

        private void SourceExpertKnowledgeRepositoryEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeRepositoryEntry>())
                {
                    item.SourceExpert = this.AgentId;
                }
            }
        }

        private ObservableCollection<CommunityMembership> _communityMemberships;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<CommunityMembership> CommunityMemberships
        {
            get
            {
                if (_communityMemberships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunityMemberships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _communityMemberships = new ObservableCollection<CommunityMembership>();
                    }
                    else
                    {
                        var items = base.SoAContext.CommunityMemberships.Where(x => x.Agent == this.AgentId).ToList<CommunityMembership>();
                        _communityMemberships = new ObservableCollection<CommunityMembership>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _communityMemberships.CollectionChanged += CommunityMemberships_CollectionChanged;
                }
                return _communityMemberships;
            }
            private set
            {
                if (_communityMemberships != null)
                {
                    _communityMemberships.CollectionChanged -= CommunityMemberships_CollectionChanged;
                }
                _communityMemberships = value;
                if (_communityMemberships != null)
                {
                    _communityMemberships.CollectionChanged += CommunityMemberships_CollectionChanged;
                }
            }
        }

        private void CommunityMemberships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CommunityMembership>())
                {
                    item.Agent = this.AgentId;
                }
            }
        }

        private ObservableCollection<SourceRelationship> _knowledgeEngineerSourceRelationships;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<SourceRelationship> KnowledgeEngineerSourceRelationships
        {
            get
            {
                if (_knowledgeEngineerSourceRelationships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeEngineerSourceRelationships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _knowledgeEngineerSourceRelationships = new ObservableCollection<SourceRelationship>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceRelationships.Where(x => x.KnowledgeEngineer == this.AgentId).ToList<SourceRelationship>();
                        _knowledgeEngineerSourceRelationships = new ObservableCollection<SourceRelationship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeEngineerSourceRelationships.CollectionChanged += KnowledgeEngineerSourceRelationships_CollectionChanged;
                }
                return _knowledgeEngineerSourceRelationships;
            }
            private set
            {
                if (_knowledgeEngineerSourceRelationships != null)
                {
                    _knowledgeEngineerSourceRelationships.CollectionChanged -= KnowledgeEngineerSourceRelationships_CollectionChanged;
                }
                _knowledgeEngineerSourceRelationships = value;
                if (_knowledgeEngineerSourceRelationships != null)
                {
                    _knowledgeEngineerSourceRelationships.CollectionChanged += KnowledgeEngineerSourceRelationships_CollectionChanged;
                }
            }
        }

        private void KnowledgeEngineerSourceRelationships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourceRelationship>())
                {
                    item.KnowledgeEngineer = this.AgentId;
                }
            }
        }

        private ObservableCollection<SourceRelationship> _sourceAgentSourceRelationships;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<SourceRelationship> SourceAgentSourceRelationships
        {
            get
            {
                if (_sourceAgentSourceRelationships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourceAgentSourceRelationships - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _sourceAgentSourceRelationships = new ObservableCollection<SourceRelationship>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceRelationships.Where(x => x.SourceAgent == this.AgentId).ToList<SourceRelationship>();
                        _sourceAgentSourceRelationships = new ObservableCollection<SourceRelationship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sourceAgentSourceRelationships.CollectionChanged += SourceAgentSourceRelationships_CollectionChanged;
                }
                return _sourceAgentSourceRelationships;
            }
            private set
            {
                if (_sourceAgentSourceRelationships != null)
                {
                    _sourceAgentSourceRelationships.CollectionChanged -= SourceAgentSourceRelationships_CollectionChanged;
                }
                _sourceAgentSourceRelationships = value;
                if (_sourceAgentSourceRelationships != null)
                {
                    _sourceAgentSourceRelationships.CollectionChanged += SourceAgentSourceRelationships_CollectionChanged;
                }
            }
        }

        private void SourceAgentSourceRelationships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourceRelationship>())
                {
                    item.SourceAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<DepartmentProcessAccount> _departmentProcessAccounts;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access DepartmentProcessAccounts - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _departmentProcessAccounts = new ObservableCollection<DepartmentProcessAccount>();
                    }
                    else
                    {
                        var items = base.SoAContext.DepartmentProcessAccounts.Where(x => x.StakeholderAgent == this.AgentId).ToList<DepartmentProcessAccount>();
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
                    item.StakeholderAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<ProblemOccurrence> _problemOccurrences;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ProblemOccurrence> ProblemOccurrences
        {
            get
            {
                if (_problemOccurrences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProblemOccurrences - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProblemOccurrences.Where(x => x.SolvedByAgent == this.AgentId).ToList<ProblemOccurrence>();
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _problemOccurrences.CollectionChanged += ProblemOccurrences_CollectionChanged;
                }
                return _problemOccurrences;
            }
            private set
            {
                if (_problemOccurrences != null)
                {
                    _problemOccurrences.CollectionChanged -= ProblemOccurrences_CollectionChanged;
                }
                _problemOccurrences = value;
                if (_problemOccurrences != null)
                {
                    _problemOccurrences.CollectionChanged += ProblemOccurrences_CollectionChanged;
                }
            }
        }

        private void ProblemOccurrences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProblemOccurrence>())
                {
                    item.SolvedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<OnboardingRecord> _onboardingRecords;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<OnboardingRecord> OnboardingRecords
        {
            get
            {
                if (_onboardingRecords == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OnboardingRecords - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _onboardingRecords = new ObservableCollection<OnboardingRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.OnboardingRecords.Where(x => x.NewStarter == this.AgentId).ToList<OnboardingRecord>();
                        _onboardingRecords = new ObservableCollection<OnboardingRecord>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _onboardingRecords.CollectionChanged += OnboardingRecords_CollectionChanged;
                }
                return _onboardingRecords;
            }
            private set
            {
                if (_onboardingRecords != null)
                {
                    _onboardingRecords.CollectionChanged -= OnboardingRecords_CollectionChanged;
                }
                _onboardingRecords = value;
                if (_onboardingRecords != null)
                {
                    _onboardingRecords.CollectionChanged += OnboardingRecords_CollectionChanged;
                }
            }
        }

        private void OnboardingRecords_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OnboardingRecord>())
                {
                    item.NewStarter = this.AgentId;
                }
            }
        }

        private ObservableCollection<SharingRecognition> _sharingRecognitions;

        [InverseProperty("Agent")]
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
                            throw new InvalidOperationException("Cannot access SharingRecognitions - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _sharingRecognitions = new ObservableCollection<SharingRecognition>();
                    }
                    else
                    {
                        var items = base.SoAContext.SharingRecognitions.Where(x => x.RecognizedAgent == this.AgentId).ToList<SharingRecognition>();
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
                    item.RecognizedAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _derivedByAgentKnowledgeTraces;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<KnowledgeTrace> DerivedByAgentKnowledgeTraces
        {
            get
            {
                if (_derivedByAgentKnowledgeTraces == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DerivedByAgentKnowledgeTraces - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _derivedByAgentKnowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.DerivedByAgent == this.AgentId).ToList<KnowledgeTrace>();
                        _derivedByAgentKnowledgeTraces = new ObservableCollection<KnowledgeTrace>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _derivedByAgentKnowledgeTraces.CollectionChanged += DerivedByAgentKnowledgeTraces_CollectionChanged;
                }
                return _derivedByAgentKnowledgeTraces;
            }
            private set
            {
                if (_derivedByAgentKnowledgeTraces != null)
                {
                    _derivedByAgentKnowledgeTraces.CollectionChanged -= DerivedByAgentKnowledgeTraces_CollectionChanged;
                }
                _derivedByAgentKnowledgeTraces = value;
                if (_derivedByAgentKnowledgeTraces != null)
                {
                    _derivedByAgentKnowledgeTraces.CollectionChanged += DerivedByAgentKnowledgeTraces_CollectionChanged;
                }
            }
        }

        private void DerivedByAgentKnowledgeTraces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTrace>())
                {
                    item.DerivedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _validatedByAgentKnowledgeTraces;

        [InverseProperty("AgentRef")]
        public virtual ObservableCollection<KnowledgeTrace> ValidatedByAgentKnowledgeTraces
        {
            get
            {
                if (_validatedByAgentKnowledgeTraces == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ValidatedByAgentKnowledgeTraces - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _validatedByAgentKnowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.ValidatedByAgent == this.AgentId).ToList<KnowledgeTrace>();
                        _validatedByAgentKnowledgeTraces = new ObservableCollection<KnowledgeTrace>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _validatedByAgentKnowledgeTraces.CollectionChanged += ValidatedByAgentKnowledgeTraces_CollectionChanged;
                }
                return _validatedByAgentKnowledgeTraces;
            }
            private set
            {
                if (_validatedByAgentKnowledgeTraces != null)
                {
                    _validatedByAgentKnowledgeTraces.CollectionChanged -= ValidatedByAgentKnowledgeTraces_CollectionChanged;
                }
                _validatedByAgentKnowledgeTraces = value;
                if (_validatedByAgentKnowledgeTraces != null)
                {
                    _validatedByAgentKnowledgeTraces.CollectionChanged += ValidatedByAgentKnowledgeTraces_CollectionChanged;
                }
            }
        }

        private void ValidatedByAgentKnowledgeTraces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTrace>())
                {
                    item.ValidatedByAgent = this.AgentId;
                }
            }
        }

        private ObservableCollection<MinedFlowEdge> _minedFlowEdges;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<MinedFlowEdge> MinedFlowEdges
        {
            get
            {
                if (_minedFlowEdges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MinedFlowEdges - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _minedFlowEdges = new ObservableCollection<MinedFlowEdge>();
                    }
                    else
                    {
                        var items = base.SoAContext.MinedFlowEdges.Where(x => x.IntentDecisionBy == this.AgentId).ToList<MinedFlowEdge>();
                        _minedFlowEdges = new ObservableCollection<MinedFlowEdge>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _minedFlowEdges.CollectionChanged += MinedFlowEdges_CollectionChanged;
                }
                return _minedFlowEdges;
            }
            private set
            {
                if (_minedFlowEdges != null)
                {
                    _minedFlowEdges.CollectionChanged -= MinedFlowEdges_CollectionChanged;
                }
                _minedFlowEdges = value;
                if (_minedFlowEdges != null)
                {
                    _minedFlowEdges.CollectionChanged += MinedFlowEdges_CollectionChanged;
                }
            }
        }

        private void MinedFlowEdges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MinedFlowEdge>())
                {
                    item.IntentDecisionBy = this.AgentId;
                }
            }
        }

        private ObservableCollection<ModelActivityExpert> _modelActivityExperts;

        [InverseProperty("Agent")]
        public virtual ObservableCollection<ModelActivityExpert> ModelActivityExperts
        {
            get
            {
                if (_modelActivityExperts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelActivityExperts - no database context is set. AgentId: " + this.AgentId + ".");
                        }
                        _modelActivityExperts = new ObservableCollection<ModelActivityExpert>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelActivityExperts.Where(x => x.Expert == this.AgentId).ToList<ModelActivityExpert>();
                        _modelActivityExperts = new ObservableCollection<ModelActivityExpert>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelActivityExperts.CollectionChanged += ModelActivityExperts_CollectionChanged;
                }
                return _modelActivityExperts;
            }
            private set
            {
                if (_modelActivityExperts != null)
                {
                    _modelActivityExperts.CollectionChanged -= ModelActivityExperts_CollectionChanged;
                }
                _modelActivityExperts = value;
                if (_modelActivityExperts != null)
                {
                    _modelActivityExperts.CollectionChanged += ModelActivityExperts_CollectionChanged;
                }
            }
        }

        private void ModelActivityExperts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelActivityExpert>())
                {
                    item.Expert = this.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.OrganizationRefRef;
            _ = this.VersionDecidedByAgentRulebookReleases;
            _ = this.ApprovedByAgentRulebookReleases;
            _ = this.Roles;
            _ = this.RoleAssignments;
            _ = this.MentorAgentMentorships;
            _ = this.LearnerAgentMentorships;
            _ = this.CreatedByAgentProcedureVersions;
            _ = this.ModifiedByAgentProcedureVersions;
            _ = this.ProcedureStatusChanges;
            _ = this.CreatedByAgentResources;
            _ = this.ModifiedByAgentResources;
            _ = this.PractitionerAgentElicitationSessions;
            _ = this.FacilitatorAgentElicitationSessions;
            _ = this.KnowledgeFragments;
            _ = this.ExecutedByAgentProcedureExecutions;
            _ = this.ConfirmedByAgentProcedureExecutions;
            _ = this.ExecutedByAgentStepExecutions;
            _ = this.ConfirmedByAgentStepExecutions;
            _ = this.RequirementSatisfactions;
            _ = this.IssueOccurrences;
            _ = this.UserQuestions;
            _ = this.UserFeedback;
            _ = this.ChangeRequests;
            _ = this.ReviewEvents;
            _ = this.LearningActivities;
            _ = this.InvokedByAgentExceptionInvocations;
            _ = this.ApprovedByAgentExceptionInvocations;
            _ = this.VerificationOutcomes;
            _ = this.MessageDeliveries;
            _ = this.TemplateApprovals;
            _ = this.DecidingAgentAgentDecisionRecords;
            _ = this.ReviewedByAgentAgentDecisionRecords;
            _ = this.Attestations;
            _ = this.AppUsers;
            _ = this.SeekerKnowledgeBrokerLinks;
            _ = this.BrokerKnowledgeBrokerLinks;
            _ = this.AppliedByAgentMethodApplications;
            _ = this.IdentifiedBrokerMethodApplications;
            _ = this.ConditionChecks;
            _ = this.ObservedByAgentCueObservations;
            _ = this.EscalatedToAgentCueObservations;
            _ = this.ExecutionParticipants;
            _ = this.AuthoringSubmissions;
            _ = this.SituationalVariants;
            _ = this.CollectedSourceMaterials;
            _ = this.AiLabelingRuns;
            _ = this.TermMeaningChanges;
            _ = this.AgentIntegrations;
            _ = this.SnapshotAssertions;
            _ = this.RetrievalSegments;
            _ = this.AnsweringAgentAssistantAnswers;
            _ = this.AskedByAgentAssistantAnswers;
            _ = this.HumanReviewedByAssistantAnswers;
            _ = this.AnswerRequirementChecks;
            _ = this.AiToolInvocations;
            _ = this.PromptTemplates;
            _ = this.ModelAnnotations;
            _ = this.KnowledgeSearchEvents;
            _ = this.AiAdoptionInitiatives;
            _ = this.ProposingAgentAiInsightProposals;
            _ = this.ValidatedByAgentAiInsightProposals;
            _ = this.AssistantBenchmarks;
            _ = this.StewardActivities;
            _ = this.RequestedByAgentModelChangeRequests;
            _ = this.ApprovedByAgentModelChangeRequests;
            _ = this.PlacementDecidedByAgentModelChangeRequests;
            _ = this.ChangeImpactFindings;
            _ = this.ChangeIntegrityChecks;
            _ = this.RaisedByAgentChangeObjections;
            _ = this.ResolvedByAgentChangeObjections;
            _ = this.StalenessQueryRuns;
            _ = this.ExternalDependencyRevisions;
            _ = this.AskedByAgentStakeholderQuestions;
            _ = this.AnsweredByAgentStakeholderQuestions;
            _ = this.TriagedByAgentStakeholderQuestions;
            _ = this.RequestedByAgentModelExpansionRequests;
            _ = this.DecidedByAgentModelExpansionRequests;
            _ = this.CompetencyQuestionReviews;
            _ = this.QualityAssessments;
            _ = this.DraftedByAgentTermDefinitions;
            _ = this.RevisedByAgentTermDefinitions;
            _ = this.ProposedByAgentModelProposals;
            _ = this.ReviewedByAgentModelProposals;
            _ = this.CommittedByAgentModelProposals;
            _ = this.ProcessDesignDecisions;
            _ = this.ModelChangeLogEntries;
            _ = this.DriftObservations;
            _ = this.KnowledgeAudits;
            _ = this.KnowledgeCaptureInitiatives;
            _ = this.KnowledgeWorkforcePositions;
            _ = this.AiAgentAiAgentAccountabilities;
            _ = this.AccountableAgentAiAgentAccountabilities;
            _ = this.CurrentAgentAgentUpgradeAssessments;
            _ = this.CandidateAgentAgentUpgradeAssessments;
            _ = this.AssessedByAgentAgentUpgradeAssessments;
            _ = this.RoleAssignmentUpdateTasks;
            _ = this.AssignmentRoutedNotices;
            _ = this.PractitionerExpertise;
            _ = this.CriticalIncidents;
            _ = this.ObservedActions;
            _ = this.ElicitationParticipants;
            _ = this.RepresentationReviews;
            _ = this.HolderAWorkflowViewDivergences;
            _ = this.HolderBWorkflowViewDivergences;
            _ = this.ExpertCognitions;
            _ = this.RepertoryGridConstructs;
            _ = this.KnowledgeHoldings;
            _ = this.FragmentCorroborations;
            _ = this.KnowHowCarriers;
            _ = this.FromAgentKnowledgeTransfers;
            _ = this.RecipientAgentKnowledgeTransfers;
            _ = this.AuthorAgentKnowledgeRepositoryEntries;
            _ = this.SourceExpertKnowledgeRepositoryEntries;
            _ = this.CommunityMemberships;
            _ = this.KnowledgeEngineerSourceRelationships;
            _ = this.SourceAgentSourceRelationships;
            _ = this.DepartmentProcessAccounts;
            _ = this.ProblemOccurrences;
            _ = this.OnboardingRecords;
            _ = this.SharingRecognitions;
            _ = this.DerivedByAgentKnowledgeTraces;
            _ = this.ValidatedByAgentKnowledgeTraces;
            _ = this.MinedFlowEdges;
            _ = this.ModelActivityExperts;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
