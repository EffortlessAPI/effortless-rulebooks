
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
    [Table("Procedures")]
    public class ProcedureBase : SoAEntityBase
    {
        [Key]
        public string ProcedureId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? Purpose { get; set; }
        public string? Target { get; set; }
        public bool? IsTemplate { get; set; }
        public string? CurrentVersionKey { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula ExecutionCount (rulebook: =SUMIFS(ProcedureVersions!{{ExecutionCount}}, ProcedureVersions!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? ExecutionCount
        {
            get => F.AsInt(F.Memo(this, "ExecutionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<ProcedureVersion>(base.SoAContext, "ProcedureVersions", __c => __c.ProcedureVersions), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)), __r => F.Of(__r.ExecutionCount), null))))); set { }
        }

        // Formula IsTemplateInstance (rulebook: ={{TemplateProcedure}} <> "")
        [NotMapped]
        public bool? IsTemplateInstance
        {
            get => F.AsBool(F.Memo(this, "IsTemplateInstance", () => F.IsNotBlank(F.Of(this.TemplateProcedure)))); set { }
        }

        // Formula TargetCount (rulebook: =COUNTIFS(ProcedureTargets!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? TargetCount
        {
            get => F.AsInt(F.Memo(this, "TargetCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureTarget>(base.SoAContext, "ProcedureTargets", __c => __c.ProcedureTargets), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula AdoptionCount (rulebook: =COUNTIFS(ProcedureAdoptions!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? AdoptionCount
        {
            get => F.AsInt(F.Memo(this, "AdoptionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureAdoption>(base.SoAContext, "ProcedureAdoptions", __c => __c.ProcedureAdoptions), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula SpecifiedStepTotal (rulebook: =SUMIFS(ProcedureVersions!{{CountOfSteps}}, ProcedureVersions!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? SpecifiedStepTotal
        {
            get => F.AsInt(F.Memo(this, "SpecifiedStepTotal", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<ProcedureVersion>(base.SoAContext, "ProcedureVersions", __c => __c.ProcedureVersions), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)), __r => F.Of(__r.CountOfSteps), null))))); set { }
        }

        // Formula HasNoExplicitSteps (rulebook: ={{SpecifiedStepTotal}} = 0)
        [NotMapped]
        public bool? HasNoExplicitSteps
        {
            get => F.AsBool(F.Memo(this, "HasNoExplicitSteps", () => F.Eq(F.Of(this.SpecifiedStepTotal), F.I(0)))); set { }
        }

        // Formula CalledByStepCount (rulebook: =COUNTIFS(Steps!{{CallsProcedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? CalledByStepCount
        {
            get => F.AsInt(F.Memo(this, "CalledByStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.CallsProcedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula IsNestedProcedure (rulebook: ={{CalledByStepCount}} > 0)
        [NotMapped]
        public bool? IsNestedProcedure
        {
            get => F.AsBool(F.Memo(this, "IsNestedProcedure", () => F.Cmp(F.Of(this.CalledByStepCount), ">", F.I(0)))); set { }
        }

        // Formula FailureCriterionCount (rulebook: =COUNTIFS(ProcedureOutcomeCriteria!{{FailureProcedureKey}}, {{ProcedureId}}))
        [NotMapped]
        public int? FailureCriterionCount
        {
            get => F.AsInt(F.Memo(this, "FailureCriterionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureOutcomeCriteria>(base.SoAContext, "ProcedureOutcomeCriteria", __c => __c.ProcedureOutcomeCriteria), __r => F.CritField(F.Of(__r.FailureProcedureKey), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula HasNoFailureCriterion (rulebook: ={{FailureCriterionCount}} = 0)
        [NotMapped]
        public bool? HasNoFailureCriterion
        {
            get => F.AsBool(F.Memo(this, "HasNoFailureCriterion", () => F.Eq(F.Of(this.FailureCriterionCount), F.I(0)))); set { }
        }

        // Formula LensViewCount (rulebook: =COUNTIFS(ProcedureLensViews!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? LensViewCount
        {
            get => F.AsInt(F.Memo(this, "LensViewCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureLensView>(base.SoAContext, "ProcedureLensViews", __c => __c.ProcedureLensViews), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula PrivilegesSingleStakeholderView (rulebook: ={{LensViewCount}} = 1)
        [NotMapped]
        public bool? PrivilegesSingleStakeholderView
        {
            get => F.AsBool(F.Memo(this, "PrivilegesSingleStakeholderView", () => F.Eq(F.Of(this.LensViewCount), F.I(1)))); set { }
        }

        // Formula StepLevelViewCount (rulebook: =COUNTIFS(ProcedureLensViews!{{StepLevelProcedureKey}}, {{ProcedureId}}))
        [NotMapped]
        public int? StepLevelViewCount
        {
            get => F.AsInt(F.Memo(this, "StepLevelViewCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureLensView>(base.SoAContext, "ProcedureLensViews", __c => __c.ProcedureLensViews), __r => F.CritField(F.Of(__r.StepLevelProcedureKey), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula CategoryLevelViewCount (rulebook: =COUNTIFS(ProcedureLensViews!{{CategoryLevelProcedureKey}}, {{ProcedureId}}))
        [NotMapped]
        public int? CategoryLevelViewCount
        {
            get => F.AsInt(F.Memo(this, "CategoryLevelViewCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureLensView>(base.SoAContext, "ProcedureLensViews", __c => __c.ProcedureLensViews), __r => F.CritField(F.Of(__r.CategoryLevelProcedureKey), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula ProcedureTypeRank (rulebook: =INDEX(ProcedureTypes!{{TaxonomyRank}}, MATCH({{ProcedureType}}, ProcedureTypes!{{ProcedureTypeId}}, 0)))
        [NotMapped]
        public string? ProcedureTypeRank
        {
            get => F.AsString(F.Memo(this, "ProcedureTypeRank", () => F.Lookup<ProcedureType>(this, "ProcedureTypes", "ProcedureTypeId", __c => __c.ProcedureTypes, __r => F.Of(__r.ProcedureTypeId), F.Of(this.ProcedureType), __r => F.Of(__r.TaxonomyRank), () => F.Of(new ProcedureType().TaxonomyRank)))); set { }
        }

        // Formula ProcedureTypeDefinition (rulebook: =INDEX(ProcedureTypes!{{Definition}}, MATCH({{ProcedureType}}, ProcedureTypes!{{ProcedureTypeId}}, 0)))
        [NotMapped]
        public string? ProcedureTypeDefinition
        {
            get => F.AsString(F.Memo(this, "ProcedureTypeDefinition", () => F.Lookup<ProcedureType>(this, "ProcedureTypes", "ProcedureTypeId", __c => __c.ProcedureTypes, __r => F.Of(__r.ProcedureTypeId), F.Of(this.ProcedureType), __r => F.Of(__r.Definition), () => F.Of(new ProcedureType().Definition)))); set { }
        }

        // Formula HasStepAndCategoryResolutions (rulebook: =AND({{SpecifiedStepTotal}} > 0, {{ProcedureTypeRank}} = "ProcessType"))
        [NotMapped]
        public bool? HasStepAndCategoryResolutions
        {
            get => F.AsBool(F.Memo(this, "HasStepAndCategoryResolutions", () => F.And(F.Bool3(F.Cmp(F.Of(this.SpecifiedStepTotal), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ProcedureTypeRank), F.S("ProcessType")))))); set { }
        }

        // Formula IsMissingDemandedResolution (rulebook: =OR(AND({{StepLevelViewCount}} > 0, {{HasNoExplicitSteps}}), AND({{CategoryLevelViewCount}} > 0, OR({{ProcedureType}} = "", {{ProcedureTypeRank}} <> "ProcessType"))))
        [NotMapped]
        public bool? IsMissingDemandedResolution
        {
            get => F.AsBool(F.Memo(this, "IsMissingDemandedResolution", () => F.Or(F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.StepLevelViewCount), ">", F.I(0))), F.Bool3(F.Of(this.HasNoExplicitSteps)))), F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.CategoryLevelViewCount), ">", F.I(0))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ProcedureType))), F.Bool3(F.Ne(F.Of(this.ProcedureTypeRank), F.S("ProcessType")))))))))); set { }
        }

        // Formula IsInconsistentlyCategorized (rulebook: =OR({{ProcedureType}} = "", {{ProcedureTypeRank}} <> "ProcessType", {{ProcedureTypeDefinition}} = ""))
        [NotMapped]
        public bool? IsInconsistentlyCategorized
        {
            get => F.AsBool(F.Memo(this, "IsInconsistentlyCategorized", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.ProcedureType))), F.Bool3(F.Ne(F.Of(this.ProcedureTypeRank), F.S("ProcessType"))), F.Bool3(F.IsBlank(F.Of(this.ProcedureTypeDefinition)))))); set { }
        }

        // Formula StrategicAlignmentCount (rulebook: =COUNTIFS(ProcessStrategicAlignments!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? StrategicAlignmentCount
        {
            get => F.AsInt(F.Memo(this, "StrategicAlignmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessStrategicAlignment>(base.SoAContext, "ProcessStrategicAlignments", __c => __c.ProcessStrategicAlignments), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula HasNoStatedReasonForExisting (rulebook: ={{StrategicAlignmentCount}} = 0)
        [NotMapped]
        public bool? HasNoStatedReasonForExisting
        {
            get => F.AsBool(F.Memo(this, "HasNoStatedReasonForExisting", () => F.Eq(F.Of(this.StrategicAlignmentCount), F.I(0)))); set { }
        }

        // Formula OutcomeMeasureCount (rulebook: =COUNTIFS(ProcessOutcomeMeasures!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? OutcomeMeasureCount
        {
            get => F.AsInt(F.Memo(this, "OutcomeMeasureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessOutcomeMeasure>(base.SoAContext, "ProcessOutcomeMeasures", __c => __c.ProcessOutcomeMeasures), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula UnlinkedMeasureCount (rulebook: =COUNTIFS(ProcessOutcomeMeasures!{{Procedure}}, {{ProcedureId}}, ProcessOutcomeMeasures!{{IsUnlinkedToBusinessOutcome}}, TRUE))
        [NotMapped]
        public int? UnlinkedMeasureCount
        {
            get => F.AsInt(F.Memo(this, "UnlinkedMeasureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessOutcomeMeasure>(base.SoAContext, "ProcessOutcomeMeasures", __c => __c.ProcessOutcomeMeasures), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsUnlinkedToBusinessOutcome), F.B(true))))))); set { }
        }

        // Formula MeasuresPerformanceWithoutBusinessLink (rulebook: =AND({{OutcomeMeasureCount}} > 0, {{UnlinkedMeasureCount}} = {{OutcomeMeasureCount}}))
        [NotMapped]
        public bool? MeasuresPerformanceWithoutBusinessLink
        {
            get => F.AsBool(F.Memo(this, "MeasuresPerformanceWithoutBusinessLink", () => F.And(F.Bool3(F.Cmp(F.Of(this.OutcomeMeasureCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.UnlinkedMeasureCount), F.Of(this.OutcomeMeasureCount)))))); set { }
        }

        // Formula HinderedByCount (rulebook: =COUNTIFS(ProcessInterdependencies!{{HinderedProcedureKey}}, {{ProcedureId}}))
        [NotMapped]
        public int? HinderedByCount
        {
            get => F.AsInt(F.Memo(this, "HinderedByCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcessInterdependency>(base.SoAContext, "ProcessInterdependencies", __c => __c.ProcessInterdependencies), __r => F.CritField(F.Of(__r.HinderedProcedureKey), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula IsHinderedByAnotherOperation (rulebook: ={{HinderedByCount}} > 0)
        [NotMapped]
        public bool? IsHinderedByAnotherOperation
        {
            get => F.AsBool(F.Memo(this, "IsHinderedByAnotherOperation", () => F.Cmp(F.Of(this.HinderedByCount), ">", F.I(0)))); set { }
        }

        // Formula TypeDistinguishingFacet (rulebook: =INDEX(ProcedureTypes!{{DistinguishingFacet}}, MATCH({{ProcedureType}}, ProcedureTypes!{{ProcedureTypeId}}, 0)))
        [NotMapped]
        public string? TypeDistinguishingFacet
        {
            get => F.AsString(F.Memo(this, "TypeDistinguishingFacet", () => F.Lookup<ProcedureType>(this, "ProcedureTypes", "ProcedureTypeId", __c => __c.ProcedureTypes, __r => F.Of(__r.ProcedureTypeId), F.Of(this.ProcedureType), __r => F.Of(__r.DistinguishingFacet), () => F.Of(new ProcedureType().DistinguishingFacet)))); set { }
        }

        // Formula TypeDistinguishingValue (rulebook: =INDEX(ProcedureTypes!{{DistinguishingValue}}, MATCH({{ProcedureType}}, ProcedureTypes!{{ProcedureTypeId}}, 0)))
        [NotMapped]
        public string? TypeDistinguishingValue
        {
            get => F.AsString(F.Memo(this, "TypeDistinguishingValue", () => F.Lookup<ProcedureType>(this, "ProcedureTypes", "ProcedureTypeId", __c => __c.ProcedureTypes, __r => F.Of(__r.ProcedureTypeId), F.Of(this.ProcedureType), __r => F.Of(__r.DistinguishingValue), () => F.Of(new ProcedureType().DistinguishingValue)))); set { }
        }

        // Formula MatchingDistinctionCount (rulebook: =COUNTIFS(ProcedureFacetAssignments!{{Procedure}}, {{ProcedureId}}, ProcedureFacetAssignments!{{Facet}}, {{TypeDistinguishingFacet}}, ProcedureFacetAssignments!{{FacetValue}}, {{TypeDistinguishingValue}}))
        [NotMapped]
        public int? MatchingDistinctionCount
        {
            get => F.AsInt(F.Memo(this, "MatchingDistinctionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ProcedureFacetAssignment>(base.SoAContext, "ProcedureFacetAssignments", __c => __c.ProcedureFacetAssignments), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritField(F.Of(__r.Facet), F.Of(this.TypeDistinguishingFacet)) && F.CritField(F.Of(__r.FacetValue), F.Of(this.TypeDistinguishingValue))))))); set { }
        }

        // Formula LacksTypeDistinction (rulebook: =AND({{ProcedureType}} <> "", {{MatchingDistinctionCount}} = 0))
        [NotMapped]
        public bool? LacksTypeDistinction
        {
            get => F.AsBool(F.Memo(this, "LacksTypeDistinction", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ProcedureType))), F.Bool3(F.Eq(F.Of(this.MatchingDistinctionCount), F.I(0)))))); set { }
        }

        // Formula LacksDistinctionTypeKey (rulebook: =IF({{LacksTypeDistinction}}, {{ProcedureType}}, ""))
        [NotMapped]
        public string? LacksDistinctionTypeKey
        {
            get => F.AsString(F.Memo(this, "LacksDistinctionTypeKey", () => (F.Truthy(F.Bool3(F.Of(this.LacksTypeDistinction))) ? F.Of(this.ProcedureType) : F.S("")))); set { }
        }

        // Formula ManagedVocabularyCount (rulebook: =COUNTIFS(Vocabularies!{{ManagedSchemeProcedureKey}}, {{ProcedureId}}))
        [NotMapped]
        public int? ManagedVocabularyCount
        {
            get => F.AsInt(F.Memo(this, "ManagedVocabularyCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Vocabulary>(base.SoAContext, "Vocabularies", __c => __c.Vocabularies), __r => F.CritField(F.Of(__r.ManagedSchemeProcedureKey), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula LacksManagedControlledVocabulary (rulebook: =AND({{SpecifiedStepTotal}} > 0, {{ManagedVocabularyCount}} = 0))
        [NotMapped]
        public bool? LacksManagedControlledVocabulary
        {
            get => F.AsBool(F.Memo(this, "LacksManagedControlledVocabulary", () => F.And(F.Bool3(F.Cmp(F.Of(this.SpecifiedStepTotal), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ManagedVocabularyCount), F.I(0)))))); set { }
        }

        // Formula ComplianceReviewStepCount (rulebook: =COUNTIFS(Steps!{{ComplianceReviewProcedureKey}}, {{ProcedureId}}))
        [NotMapped]
        public int? ComplianceReviewStepCount
        {
            get => F.AsInt(F.Memo(this, "ComplianceReviewStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ComplianceReviewProcedureKey), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula InvolvesComplianceReviewRole (rulebook: ={{ComplianceReviewStepCount}} > 0)
        [NotMapped]
        public bool? InvolvesComplianceReviewRole
        {
            get => F.AsBool(F.Memo(this, "InvolvesComplianceReviewRole", () => F.Cmp(F.Of(this.ComplianceReviewStepCount), ">", F.I(0)))); set { }
        }

        // Formula IsRegulatedButUnformalized (rulebook: =AND({{RequiredByRegulation}} <> "", {{HasNoExplicitSteps}}))
        [NotMapped]
        public bool? IsRegulatedButUnformalized
        {
            get => F.AsBool(F.Memo(this, "IsRegulatedButUnformalized", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RequiredByRegulation))), F.Bool3(F.Of(this.HasNoExplicitSteps))))); set { }
        }

        // Formula AgentIntendedCount (rulebook: =COUNTIFS(AiAdoptionInitiatives!{{TargetProcedure}}, {{ProcedureId}}, AiAdoptionInitiatives!{{IsAgentic}}, TRUE))
        [NotMapped]
        public int? AgentIntendedCount
        {
            get => F.AsInt(F.Memo(this, "AgentIntendedCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AiAdoptionInitiatif>(base.SoAContext, "AiAdoptionInitiatives", __c => __c.AiAdoptionInitiatives), __r => F.CritField(F.Of(__r.TargetProcedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsAgentic), F.B(true))))))); set { }
        }

        // Formula IsTacitOnlyAgentTarget (rulebook: =AND({{AgentIntendedCount}} > 0, {{HasNoExplicitSteps}}))
        [NotMapped]
        public bool? IsTacitOnlyAgentTarget
        {
            get => F.AsBool(F.Memo(this, "IsTacitOnlyAgentTarget", () => F.And(F.Bool3(F.Cmp(F.Of(this.AgentIntendedCount), ">", F.I(0))), F.Bool3(F.Of(this.HasNoExplicitSteps))))); set { }
        }

        // Formula RepositoryEntryCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? RepositoryEntryCount
        {
            get => F.AsInt(F.Memo(this, "RepositoryEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula StaleEntryCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{Procedure}}, {{ProcedureId}}, KnowledgeRepositoryEntries!{{IsStale}}, TRUE))
        [NotMapped]
        public int? StaleEntryCount
        {
            get => F.AsInt(F.Memo(this, "StaleEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsStale), F.B(true))))))); set { }
        }

        // Formula DepartedOnlyKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Procedure}}, {{ProcedureId}}, KnowHowCarriers!{{IsHeldOnlyByDeparted}}, TRUE))
        [NotMapped]
        public int? DepartedOnlyKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "DepartedOnlyKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsHeldOnlyByDeparted), F.B(true))))))); set { }
        }

        // Formula HasDecayedTransferChannel (rulebook: =OR({{StaleEntryCount}} > 0, {{DepartedOnlyKnowHowCount}} > 0))
        [NotMapped]
        public bool? HasDecayedTransferChannel
        {
            get => F.AsBool(F.Memo(this, "HasDecayedTransferChannel", () => F.Or(F.Bool3(F.Cmp(F.Of(this.StaleEntryCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.DepartedOnlyKnowHowCount), ">", F.I(0)))))); set { }
        }

        // Formula ExecutionFeedbackEntryCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{Procedure}}, {{ProcedureId}}, KnowledgeRepositoryEntries!{{IsExecutionFeedback}}, TRUE))
        [NotMapped]
        public int? ExecutionFeedbackEntryCount
        {
            get => F.AsInt(F.Memo(this, "ExecutionFeedbackEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsExecutionFeedback), F.B(true))))))); set { }
        }

        // Formula IsExecutedWithoutFeedbackLoop (rulebook: =AND({{ExecutionCount}} > 0, {{ExecutionFeedbackEntryCount}} = 0))
        [NotMapped]
        public bool? IsExecutedWithoutFeedbackLoop
        {
            get => F.AsBool(F.Memo(this, "IsExecutedWithoutFeedbackLoop", () => F.And(F.Bool3(F.Cmp(F.Of(this.ExecutionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ExecutionFeedbackEntryCount), F.I(0)))))); set { }
        }

        // Formula MachineAuthoredEntryCount (rulebook: =COUNTIFS(KnowledgeRepositoryEntries!{{Procedure}}, {{ProcedureId}}, KnowledgeRepositoryEntries!{{IsMachineAuthored}}, TRUE))
        [NotMapped]
        public int? MachineAuthoredEntryCount
        {
            get => F.AsInt(F.Memo(this, "MachineAuthoredEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeRepositoryEntry>(base.SoAContext, "KnowledgeRepositoryEntries", __c => __c.KnowledgeRepositoryEntries), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsMachineAuthored), F.B(true))))))); set { }
        }

        // Formula PractitionerRelationshipCount (rulebook: =COUNTIFS(SourceRelationships!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? PractitionerRelationshipCount
        {
            get => F.AsInt(F.Memo(this, "PractitionerRelationshipCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<SourceRelationship>(base.SoAContext, "SourceRelationships", __c => __c.SourceRelationships), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula IsCapturedByAutomationAlone (rulebook: =AND({{MachineAuthoredEntryCount}} > 0, {{PractitionerRelationshipCount}} = 0))
        [NotMapped]
        public bool? IsCapturedByAutomationAlone
        {
            get => F.AsBool(F.Memo(this, "IsCapturedByAutomationAlone", () => F.And(F.Bool3(F.Cmp(F.Of(this.MachineAuthoredEntryCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.PractitionerRelationshipCount), F.I(0)))))); set { }
        }

        // Formula UnengagedStakeholderCount (rulebook: =COUNTIFS(DepartmentProcessAccounts!{{Procedure}}, {{ProcedureId}}, DepartmentProcessAccounts!{{IsAwaitingEngagement}}, TRUE))
        [NotMapped]
        public int? UnengagedStakeholderCount
        {
            get => F.AsInt(F.Memo(this, "UnengagedStakeholderCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<DepartmentProcessAccount>(base.SoAContext, "DepartmentProcessAccounts", __c => __c.DepartmentProcessAccounts), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsAwaitingEngagement), F.B(true))))))); set { }
        }

        // Formula HasUnengagedStakeholder (rulebook: ={{UnengagedStakeholderCount}} > 0)
        [NotMapped]
        public bool? HasUnengagedStakeholder
        {
            get => F.AsBool(F.Memo(this, "HasUnengagedStakeholder", () => F.Cmp(F.Of(this.UnengagedStakeholderCount), ">", F.I(0)))); set { }
        }

        // Formula RecentStarterCount (rulebook: =COUNTIFS(OnboardingRecords!{{Procedure}}, {{ProcedureId}}, OnboardingRecords!{{IsRecentStart}}, TRUE))
        [NotMapped]
        public int? RecentStarterCount
        {
            get => F.AsInt(F.Memo(this, "RecentStarterCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OnboardingRecord>(base.SoAContext, "OnboardingRecords", __c => __c.OnboardingRecords), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsRecentStart), F.B(true))))))); set { }
        }

        // Formula UntransferredVeteranKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Procedure}}, {{ProcedureId}}, KnowHowCarriers!{{IsUntransferredVeteranKnowHow}}, TRUE))
        [NotMapped]
        public int? UntransferredVeteranKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "UntransferredVeteranKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsUntransferredVeteranKnowHow), F.B(true))))))); set { }
        }

        // Formula DepartingVeteranKnowHowCount (rulebook: =COUNTIFS(KnowHowCarriers!{{Procedure}}, {{ProcedureId}}, KnowHowCarriers!{{IsAtRiskOfImminentLoss}}, TRUE))
        [NotMapped]
        public int? DepartingVeteranKnowHowCount
        {
            get => F.AsInt(F.Memo(this, "DepartingVeteranKnowHowCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowHowCarrier>(base.SoAContext, "KnowHowCarriers", __c => __c.KnowHowCarriers), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsAtRiskOfImminentLoss), F.B(true))))))); set { }
        }

        // Formula HasTransferShortfallExposure (rulebook: =OR(AND({{RecentStarterCount}} >= 3, {{UntransferredVeteranKnowHowCount}} > 0), {{DepartingVeteranKnowHowCount}} > 0))
        [NotMapped]
        public bool? HasTransferShortfallExposure
        {
            get => F.AsBool(F.Memo(this, "HasTransferShortfallExposure", () => F.Or(F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.RecentStarterCount), ">=", F.I(3))), F.Bool3(F.Cmp(F.Of(this.UntransferredVeteranKnowHowCount), ">", F.I(0))))), F.Bool3(F.Cmp(F.Of(this.DepartingVeteranKnowHowCount), ">", F.I(0)))))); set { }
        }

        // Formula ProficientWithCaptureCount (rulebook: =COUNTIFS(OnboardingRecords!{{Procedure}}, {{ProcedureId}}, OnboardingRecords!{{IsProficient}}, TRUE, OnboardingRecords!{{UsedCapturedKnowledge}}, TRUE))
        [NotMapped]
        public int? ProficientWithCaptureCount
        {
            get => F.AsInt(F.Memo(this, "ProficientWithCaptureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OnboardingRecord>(base.SoAContext, "OnboardingRecords", __c => __c.OnboardingRecords), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsProficient), F.B(true)) && F.CritLiteral(F.Of(__r.UsedCapturedKnowledge), F.B(true))))))); set { }
        }

        // Formula ProficientWithoutCaptureCount (rulebook: =COUNTIFS(OnboardingRecords!{{Procedure}}, {{ProcedureId}}, OnboardingRecords!{{IsProficient}}, TRUE, OnboardingRecords!{{UsedCapturedKnowledge}}, FALSE))
        [NotMapped]
        public int? ProficientWithoutCaptureCount
        {
            get => F.AsInt(F.Memo(this, "ProficientWithoutCaptureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OnboardingRecord>(base.SoAContext, "OnboardingRecords", __c => __c.OnboardingRecords), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsProficient), F.B(true)) && F.CritLiteral(F.Of(__r.UsedCapturedKnowledge), F.B(false))))))); set { }
        }

        // Formula DaysWithCaptureTotal (rulebook: =SUMIFS(OnboardingRecords!{{DaysToProficiency}}, OnboardingRecords!{{Procedure}}, {{ProcedureId}}, OnboardingRecords!{{IsProficient}}, TRUE, OnboardingRecords!{{UsedCapturedKnowledge}}, TRUE))
        [NotMapped]
        public int? DaysWithCaptureTotal
        {
            get => F.AsInt(F.Memo(this, "DaysWithCaptureTotal", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<OnboardingRecord>(base.SoAContext, "OnboardingRecords", __c => __c.OnboardingRecords), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsProficient), F.B(true)) && F.CritLiteral(F.Of(__r.UsedCapturedKnowledge), F.B(true)), __r => F.Of(__r.DaysToProficiency), null))))); set { }
        }

        // Formula DaysWithoutCaptureTotal (rulebook: =SUMIFS(OnboardingRecords!{{DaysToProficiency}}, OnboardingRecords!{{Procedure}}, {{ProcedureId}}, OnboardingRecords!{{IsProficient}}, TRUE, OnboardingRecords!{{UsedCapturedKnowledge}}, FALSE))
        [NotMapped]
        public int? DaysWithoutCaptureTotal
        {
            get => F.AsInt(F.Memo(this, "DaysWithoutCaptureTotal", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<OnboardingRecord>(base.SoAContext, "OnboardingRecords", __c => __c.OnboardingRecords), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsProficient), F.B(true)) && F.CritLiteral(F.Of(__r.UsedCapturedKnowledge), F.B(false)), __r => F.Of(__r.DaysToProficiency), null))))); set { }
        }

        // Formula AvgDaysToProficiencyWithCapture (rulebook: =IF({{ProficientWithCaptureCount}} = 0, 0, ROUND({{DaysWithCaptureTotal}} / {{ProficientWithCaptureCount}}, 1)))
        [NotMapped]
        public decimal? AvgDaysToProficiencyWithCapture
        {
            get => F.AsDecimal(F.Memo(this, "AvgDaysToProficiencyWithCapture", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.ProficientWithCaptureCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Of(this.DaysWithCaptureTotal), F.Of(this.ProficientWithCaptureCount)), F.I(1))))); set { }
        }

        // Formula AvgDaysToProficiencyWithoutCapture (rulebook: =IF({{ProficientWithoutCaptureCount}} = 0, 0, ROUND({{DaysWithoutCaptureTotal}} / {{ProficientWithoutCaptureCount}}, 1)))
        [NotMapped]
        public decimal? AvgDaysToProficiencyWithoutCapture
        {
            get => F.AsDecimal(F.Memo(this, "AvgDaysToProficiencyWithoutCapture", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.ProficientWithoutCaptureCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Of(this.DaysWithoutCaptureTotal), F.Of(this.ProficientWithoutCaptureCount)), F.I(1))))); set { }
        }

        // Formula FormalizationDoesNotEaseOnboarding (rulebook: =AND({{ProficientWithCaptureCount}} > 0, {{ProficientWithoutCaptureCount}} > 0, {{AvgDaysToProficiencyWithCapture}} >= {{AvgDaysToProficiencyWithoutCapture}}))
        [NotMapped]
        public bool? FormalizationDoesNotEaseOnboarding
        {
            get => F.AsBool(F.Memo(this, "FormalizationDoesNotEaseOnboarding", () => F.And(F.Bool3(F.Cmp(F.Of(this.ProficientWithCaptureCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ProficientWithoutCaptureCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.AvgDaysToProficiencyWithCapture), ">=", F.Of(this.AvgDaysToProficiencyWithoutCapture)))))); set { }
        }

        // Formula CollectedMaterialCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public int? CollectedMaterialCount
        {
            get => F.AsInt(F.Memo(this, "CollectedMaterialCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId))))))); set { }
        }

        // Formula InWorkCaptureCount (rulebook: =COUNTIFS(CollectedSourceMaterials!{{Procedure}}, {{ProcedureId}}, CollectedSourceMaterials!{{IsCapturedInFlowOfWork}}, TRUE))
        [NotMapped]
        public int? InWorkCaptureCount
        {
            get => F.AsInt(F.Memo(this, "InWorkCaptureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)) && F.CritLiteral(F.Of(__r.IsCapturedInFlowOfWork), F.B(true))))))); set { }
        }

        // Formula IsCaptureSeparateFromWork (rulebook: =AND({{CollectedMaterialCount}} > 0, {{InWorkCaptureCount}} = 0))
        [NotMapped]
        public bool? IsCaptureSeparateFromWork
        {
            get => F.AsBool(F.Memo(this, "IsCaptureSeparateFromWork", () => F.And(F.Bool3(F.Cmp(F.Of(this.CollectedMaterialCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.InWorkCaptureCount), F.I(0)))))); set { }
        }

        // Formula ExpertAcquisitionHours (rulebook: =SUMIFS(CollectedSourceMaterials!{{ExpertEffortHours}}, CollectedSourceMaterials!{{Procedure}}, {{ProcedureId}}))
        [NotMapped]
        public decimal? ExpertAcquisitionHours
        {
            get => F.AsDecimal(F.Memo(this, "ExpertAcquisitionHours", () => (base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<CollectedSourceMaterial>(base.SoAContext, "CollectedSourceMaterials", __c => __c.CollectedSourceMaterials), __r => F.CritField(F.Of(__r.Procedure), F.Of(this.ProcedureId)), __r => F.Of(__r.ExpertEffortHours), null)))); set { }
        }

        // Formula ComplianceDocumentCount (rulebook: =COUNTIFS(Resources!{{ComplianceRecordFor}}, {{ProcedureId}}))
        [NotMapped]
        public int? ComplianceDocumentCount
        {
            get => F.AsInt(F.Memo(this, "ComplianceDocumentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Resource>(base.SoAContext, "Resources", __c => __c.Resources), __r => F.CritField(F.Of(__r.ComplianceRecordFor), F.Of(this.ProcedureId))))))); set { }
        }


        public string? ProcedureType { get; set; }
        public string? OwnerOrganization { get; set; }
        public string? AdoptedByOrganization { get; set; }
        public string? TemplateProcedure { get; set; }
        public string? RequiredByRegulation { get; set; }

        private ProcedureType _procedureTypeRef;

        [ForeignKey("ProcedureType")]
        public virtual ProcedureType ProcedureTypeRef
        {
            get
            {
                if (_procedureTypeRef == null && !string.IsNullOrEmpty(ProcedureType))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureTypeRef - no database context is set. ProcedureType: " + ProcedureType + ".");
                        }
                        return null;
                    }
                    _procedureTypeRef = base.SoAContext.ProcedureTypes.Find(ProcedureType);
                    if (_procedureTypeRef != null)
                    {
                        base.SoAContext.Attach(_procedureTypeRef);
                    }
                }
                return _procedureTypeRef;
            }
            set
            {
                if (_procedureTypeRef != value)
                {
                    _procedureTypeRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureTypeRef != null)
                    {
                        ProcedureType = _procedureTypeRef.ProcedureTypeId;
                    }
                }
            }
        }

        private Organization _organization;

        [ForeignKey("OwnerOrganization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(OwnerOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. OwnerOrganization: " + OwnerOrganization + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(OwnerOrganization);
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
                        OwnerOrganization = _organization.OrganizationId;
                    }
                }
            }
        }

        private Organization _organizationRef;

        [ForeignKey("AdoptedByOrganization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(AdoptedByOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. AdoptedByOrganization: " + AdoptedByOrganization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(AdoptedByOrganization);
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
                        AdoptedByOrganization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private Procedure _procedure;

        [ForeignKey("TemplateProcedure")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(TemplateProcedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. TemplateProcedure: " + TemplateProcedure + ".");
                        }
                        return null;
                    }
                    _procedure = base.SoAContext.Procedures.Find(TemplateProcedure);
                    if (_procedure != null)
                    {
                        base.SoAContext.Attach(_procedure);
                    }
                }
                return _procedure;
            }
            set
            {
                if (_procedure != value)
                {
                    _procedure = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedure != null)
                    {
                        TemplateProcedure = _procedure.ProcedureId;
                    }
                }
            }
        }

        private RegulatoryFramework _regulatoryFramework;

        [ForeignKey("RequiredByRegulation")]
        public virtual RegulatoryFramework RegulatoryFramework
        {
            get
            {
                if (_regulatoryFramework == null && !string.IsNullOrEmpty(RequiredByRegulation))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RegulatoryFramework - no database context is set. RequiredByRegulation: " + RequiredByRegulation + ".");
                        }
                        return null;
                    }
                    _regulatoryFramework = base.SoAContext.RegulatoryFrameworks.Find(RequiredByRegulation);
                    if (_regulatoryFramework != null)
                    {
                        base.SoAContext.Attach(_regulatoryFramework);
                    }
                }
                return _regulatoryFramework;
            }
            set
            {
                if (_regulatoryFramework != value)
                {
                    _regulatoryFramework = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_regulatoryFramework != null)
                    {
                        RequiredByRegulation = _regulatoryFramework.RegulatoryFrameworkId;
                    }
                }
            }
        }

        private ObservableCollection<Procedure> _procedures;

        [InverseProperty("Procedure")]
        public virtual ObservableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedures - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedures = new ObservableCollection<Procedure>();
                    }
                    else
                    {
                        var items = base.SoAContext.Procedures.Where(x => x.TemplateProcedure == this.ProcedureId).ToList<Procedure>();
                        _procedures = new ObservableCollection<Procedure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
                return _procedures;
            }
            private set
            {
                if (_procedures != null)
                {
                    _procedures.CollectionChanged -= Procedures_CollectionChanged;
                }
                _procedures = value;
                if (_procedures != null)
                {
                    _procedures.CollectionChanged += Procedures_CollectionChanged;
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Procedure>())
                {
                    item.TemplateProcedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcedureVersion> _procedureVersions;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcedureVersion> ProcedureVersions
        {
            get
            {
                if (_procedureVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersions - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureVersions = new ObservableCollection<ProcedureVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureVersions.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureVersion>();
                        _procedureVersions = new ObservableCollection<ProcedureVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
                return _procedureVersions;
            }
            private set
            {
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged -= ProcedureVersions_CollectionChanged;
                }
                _procedureVersions = value;
                if (_procedureVersions != null)
                {
                    _procedureVersions.CollectionChanged += ProcedureVersions_CollectionChanged;
                }
            }
        }

        private void ProcedureVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureVersion>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<Step> _steps;

        [InverseProperty("Procedure")]
        public virtual ObservableCollection<Step> Steps
        {
            get
            {
                if (_steps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Steps - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _steps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.CallsProcedure == this.ProcedureId).ToList<Step>();
                        _steps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
                return _steps;
            }
            private set
            {
                if (_steps != null)
                {
                    _steps.CollectionChanged -= Steps_CollectionChanged;
                }
                _steps = value;
                if (_steps != null)
                {
                    _steps.CollectionChanged += Steps_CollectionChanged;
                }
            }
        }

        private void Steps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.CallsProcedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<Resource> _catalogEntryForResources;

        [InverseProperty("Procedure")]
        public virtual ObservableCollection<Resource> CatalogEntryForResources
        {
            get
            {
                if (_catalogEntryForResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CatalogEntryForResources - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _catalogEntryForResources = new ObservableCollection<Resource>();
                    }
                    else
                    {
                        var items = base.SoAContext.Resources.Where(x => x.CatalogEntryFor == this.ProcedureId).ToList<Resource>();
                        _catalogEntryForResources = new ObservableCollection<Resource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _catalogEntryForResources.CollectionChanged += CatalogEntryForResources_CollectionChanged;
                }
                return _catalogEntryForResources;
            }
            private set
            {
                if (_catalogEntryForResources != null)
                {
                    _catalogEntryForResources.CollectionChanged -= CatalogEntryForResources_CollectionChanged;
                }
                _catalogEntryForResources = value;
                if (_catalogEntryForResources != null)
                {
                    _catalogEntryForResources.CollectionChanged += CatalogEntryForResources_CollectionChanged;
                }
            }
        }

        private void CatalogEntryForResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Resource>())
                {
                    item.CatalogEntryFor = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<Resource> _complianceRecordForResources;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<Resource> ComplianceRecordForResources
        {
            get
            {
                if (_complianceRecordForResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ComplianceRecordForResources - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _complianceRecordForResources = new ObservableCollection<Resource>();
                    }
                    else
                    {
                        var items = base.SoAContext.Resources.Where(x => x.ComplianceRecordFor == this.ProcedureId).ToList<Resource>();
                        _complianceRecordForResources = new ObservableCollection<Resource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _complianceRecordForResources.CollectionChanged += ComplianceRecordForResources_CollectionChanged;
                }
                return _complianceRecordForResources;
            }
            private set
            {
                if (_complianceRecordForResources != null)
                {
                    _complianceRecordForResources.CollectionChanged -= ComplianceRecordForResources_CollectionChanged;
                }
                _complianceRecordForResources = value;
                if (_complianceRecordForResources != null)
                {
                    _complianceRecordForResources.CollectionChanged += ComplianceRecordForResources_CollectionChanged;
                }
            }
        }

        private void ComplianceRecordForResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Resource>())
                {
                    item.ComplianceRecordFor = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<Vocabulary> _vocabularies;

        [InverseProperty("Procedure")]
        public virtual ObservableCollection<Vocabulary> Vocabularies
        {
            get
            {
                if (_vocabularies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Vocabularies - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _vocabularies = new ObservableCollection<Vocabulary>();
                    }
                    else
                    {
                        var items = base.SoAContext.Vocabularies.Where(x => x.GovernsProcedure == this.ProcedureId).ToList<Vocabulary>();
                        _vocabularies = new ObservableCollection<Vocabulary>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _vocabularies.CollectionChanged += Vocabularies_CollectionChanged;
                }
                return _vocabularies;
            }
            private set
            {
                if (_vocabularies != null)
                {
                    _vocabularies.CollectionChanged -= Vocabularies_CollectionChanged;
                }
                _vocabularies = value;
                if (_vocabularies != null)
                {
                    _vocabularies.CollectionChanged += Vocabularies_CollectionChanged;
                }
            }
        }

        private void Vocabularies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Vocabulary>())
                {
                    item.GovernsProcedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ClaimEvidence> _claimEvidence;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access ClaimEvidence - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _claimEvidence = new ObservableCollection<ClaimEvidence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ClaimEvidence.Where(x => x.Procedure == this.ProcedureId).ToList<ClaimEvidence>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcedureTarget> _procedureTargets;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcedureTarget> ProcedureTargets
        {
            get
            {
                if (_procedureTargets == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureTargets - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureTargets = new ObservableCollection<ProcedureTarget>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureTargets.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureTarget>();
                        _procedureTargets = new ObservableCollection<ProcedureTarget>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureTargets.CollectionChanged += ProcedureTargets_CollectionChanged;
                }
                return _procedureTargets;
            }
            private set
            {
                if (_procedureTargets != null)
                {
                    _procedureTargets.CollectionChanged -= ProcedureTargets_CollectionChanged;
                }
                _procedureTargets = value;
                if (_procedureTargets != null)
                {
                    _procedureTargets.CollectionChanged += ProcedureTargets_CollectionChanged;
                }
            }
        }

        private void ProcedureTargets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureTarget>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcedureAdoption> _procedureAdoptions;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access ProcedureAdoptions - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureAdoptions = new ObservableCollection<ProcedureAdoption>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureAdoptions.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureAdoption>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcedureOutcomeCriteria> _procedureOutcomeCriteria;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcedureOutcomeCriteria> ProcedureOutcomeCriteria
        {
            get
            {
                if (_procedureOutcomeCriteria == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureOutcomeCriteria - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureOutcomeCriteria = new ObservableCollection<ProcedureOutcomeCriteria>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureOutcomeCriteria.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureOutcomeCriteria>();
                        _procedureOutcomeCriteria = new ObservableCollection<ProcedureOutcomeCriteria>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureOutcomeCriteria.CollectionChanged += ProcedureOutcomeCriteria_CollectionChanged;
                }
                return _procedureOutcomeCriteria;
            }
            private set
            {
                if (_procedureOutcomeCriteria != null)
                {
                    _procedureOutcomeCriteria.CollectionChanged -= ProcedureOutcomeCriteria_CollectionChanged;
                }
                _procedureOutcomeCriteria = value;
                if (_procedureOutcomeCriteria != null)
                {
                    _procedureOutcomeCriteria.CollectionChanged += ProcedureOutcomeCriteria_CollectionChanged;
                }
            }
        }

        private void ProcedureOutcomeCriteria_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureOutcomeCriteria>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcessLevelStatement> _processLevelStatements;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcessLevelStatement> ProcessLevelStatements
        {
            get
            {
                if (_processLevelStatements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessLevelStatements - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _processLevelStatements = new ObservableCollection<ProcessLevelStatement>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessLevelStatements.Where(x => x.Procedure == this.ProcedureId).ToList<ProcessLevelStatement>();
                        _processLevelStatements = new ObservableCollection<ProcessLevelStatement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processLevelStatements.CollectionChanged += ProcessLevelStatements_CollectionChanged;
                }
                return _processLevelStatements;
            }
            private set
            {
                if (_processLevelStatements != null)
                {
                    _processLevelStatements.CollectionChanged -= ProcessLevelStatements_CollectionChanged;
                }
                _processLevelStatements = value;
                if (_processLevelStatements != null)
                {
                    _processLevelStatements.CollectionChanged += ProcessLevelStatements_CollectionChanged;
                }
            }
        }

        private void ProcessLevelStatements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessLevelStatement>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcessStrategicAlignment> _processStrategicAlignments;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access ProcessStrategicAlignments - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _processStrategicAlignments = new ObservableCollection<ProcessStrategicAlignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessStrategicAlignments.Where(x => x.Procedure == this.ProcedureId).ToList<ProcessStrategicAlignment>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcessOutcomeMeasure> _processOutcomeMeasures;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcessOutcomeMeasure> ProcessOutcomeMeasures
        {
            get
            {
                if (_processOutcomeMeasures == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessOutcomeMeasures - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _processOutcomeMeasures = new ObservableCollection<ProcessOutcomeMeasure>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessOutcomeMeasures.Where(x => x.Procedure == this.ProcedureId).ToList<ProcessOutcomeMeasure>();
                        _processOutcomeMeasures = new ObservableCollection<ProcessOutcomeMeasure>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processOutcomeMeasures.CollectionChanged += ProcessOutcomeMeasures_CollectionChanged;
                }
                return _processOutcomeMeasures;
            }
            private set
            {
                if (_processOutcomeMeasures != null)
                {
                    _processOutcomeMeasures.CollectionChanged -= ProcessOutcomeMeasures_CollectionChanged;
                }
                _processOutcomeMeasures = value;
                if (_processOutcomeMeasures != null)
                {
                    _processOutcomeMeasures.CollectionChanged += ProcessOutcomeMeasures_CollectionChanged;
                }
            }
        }

        private void ProcessOutcomeMeasures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessOutcomeMeasure>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcessInterdependency> _fromProcedureProcessInterdependencies;

        [InverseProperty("Procedure")]
        public virtual ObservableCollection<ProcessInterdependency> FromProcedureProcessInterdependencies
        {
            get
            {
                if (_fromProcedureProcessInterdependencies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromProcedureProcessInterdependencies - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _fromProcedureProcessInterdependencies = new ObservableCollection<ProcessInterdependency>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessInterdependencies.Where(x => x.FromProcedure == this.ProcedureId).ToList<ProcessInterdependency>();
                        _fromProcedureProcessInterdependencies = new ObservableCollection<ProcessInterdependency>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromProcedureProcessInterdependencies.CollectionChanged += FromProcedureProcessInterdependencies_CollectionChanged;
                }
                return _fromProcedureProcessInterdependencies;
            }
            private set
            {
                if (_fromProcedureProcessInterdependencies != null)
                {
                    _fromProcedureProcessInterdependencies.CollectionChanged -= FromProcedureProcessInterdependencies_CollectionChanged;
                }
                _fromProcedureProcessInterdependencies = value;
                if (_fromProcedureProcessInterdependencies != null)
                {
                    _fromProcedureProcessInterdependencies.CollectionChanged += FromProcedureProcessInterdependencies_CollectionChanged;
                }
            }
        }

        private void FromProcedureProcessInterdependencies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessInterdependency>())
                {
                    item.FromProcedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcessInterdependency> _toProcedureProcessInterdependencies;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcessInterdependency> ToProcedureProcessInterdependencies
        {
            get
            {
                if (_toProcedureProcessInterdependencies == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToProcedureProcessInterdependencies - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _toProcedureProcessInterdependencies = new ObservableCollection<ProcessInterdependency>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessInterdependencies.Where(x => x.ToProcedure == this.ProcedureId).ToList<ProcessInterdependency>();
                        _toProcedureProcessInterdependencies = new ObservableCollection<ProcessInterdependency>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toProcedureProcessInterdependencies.CollectionChanged += ToProcedureProcessInterdependencies_CollectionChanged;
                }
                return _toProcedureProcessInterdependencies;
            }
            private set
            {
                if (_toProcedureProcessInterdependencies != null)
                {
                    _toProcedureProcessInterdependencies.CollectionChanged -= ToProcedureProcessInterdependencies_CollectionChanged;
                }
                _toProcedureProcessInterdependencies = value;
                if (_toProcedureProcessInterdependencies != null)
                {
                    _toProcedureProcessInterdependencies.CollectionChanged += ToProcedureProcessInterdependencies_CollectionChanged;
                }
            }
        }

        private void ToProcedureProcessInterdependencies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessInterdependency>())
                {
                    item.ToProcedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcedureLensView> _procedureLensViews;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcedureLensView> ProcedureLensViews
        {
            get
            {
                if (_procedureLensViews == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureLensViews - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureLensViews = new ObservableCollection<ProcedureLensView>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureLensViews.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureLensView>();
                        _procedureLensViews = new ObservableCollection<ProcedureLensView>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureLensViews.CollectionChanged += ProcedureLensViews_CollectionChanged;
                }
                return _procedureLensViews;
            }
            private set
            {
                if (_procedureLensViews != null)
                {
                    _procedureLensViews.CollectionChanged -= ProcedureLensViews_CollectionChanged;
                }
                _procedureLensViews = value;
                if (_procedureLensViews != null)
                {
                    _procedureLensViews.CollectionChanged += ProcedureLensViews_CollectionChanged;
                }
            }
        }

        private void ProcedureLensViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureLensView>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<SituationalVariant> _situationalVariants;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access SituationalVariants - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _situationalVariants = new ObservableCollection<SituationalVariant>();
                    }
                    else
                    {
                        var items = base.SoAContext.SituationalVariants.Where(x => x.Procedure == this.ProcedureId).ToList<SituationalVariant>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.Procedure == this.ProcedureId).ToList<CollectedSourceMaterial>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProcedureFacetAssignment> _procedureFacetAssignments;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<ProcedureFacetAssignment> ProcedureFacetAssignments
        {
            get
            {
                if (_procedureFacetAssignments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureFacetAssignments - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _procedureFacetAssignments = new ObservableCollection<ProcedureFacetAssignment>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcedureFacetAssignments.Where(x => x.Procedure == this.ProcedureId).ToList<ProcedureFacetAssignment>();
                        _procedureFacetAssignments = new ObservableCollection<ProcedureFacetAssignment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _procedureFacetAssignments.CollectionChanged += ProcedureFacetAssignments_CollectionChanged;
                }
                return _procedureFacetAssignments;
            }
            private set
            {
                if (_procedureFacetAssignments != null)
                {
                    _procedureFacetAssignments.CollectionChanged -= ProcedureFacetAssignments_CollectionChanged;
                }
                _procedureFacetAssignments = value;
                if (_procedureFacetAssignments != null)
                {
                    _procedureFacetAssignments.CollectionChanged += ProcedureFacetAssignments_CollectionChanged;
                }
            }
        }

        private void ProcedureFacetAssignments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcedureFacetAssignment>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<AiAdoptionInitiatif> _aiAdoptionInitiatives;

        [InverseProperty("Procedure")]
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
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatives - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _aiAdoptionInitiatives = new ObservableCollection<AiAdoptionInitiatif>();
                    }
                    else
                    {
                        var items = base.SoAContext.AiAdoptionInitiatives.Where(x => x.TargetProcedure == this.ProcedureId).ToList<AiAdoptionInitiatif>();
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
                    item.TargetProcedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<GovernedModel> _governedModels;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access GovernedModels - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _governedModels = new ObservableCollection<GovernedModel>();
                    }
                    else
                    {
                        var items = base.SoAContext.GovernedModels.Where(x => x.Procedure == this.ProcedureId).ToList<GovernedModel>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<SourcingFunction> _sourcingFunctions;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<SourcingFunction> SourcingFunctions
        {
            get
            {
                if (_sourcingFunctions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourcingFunctions - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _sourcingFunctions = new ObservableCollection<SourcingFunction>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourcingFunctions.Where(x => x.Procedure == this.ProcedureId).ToList<SourcingFunction>();
                        _sourcingFunctions = new ObservableCollection<SourcingFunction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sourcingFunctions.CollectionChanged += SourcingFunctions_CollectionChanged;
                }
                return _sourcingFunctions;
            }
            private set
            {
                if (_sourcingFunctions != null)
                {
                    _sourcingFunctions.CollectionChanged -= SourcingFunctions_CollectionChanged;
                }
                _sourcingFunctions = value;
                if (_sourcingFunctions != null)
                {
                    _sourcingFunctions.CollectionChanged += SourcingFunctions_CollectionChanged;
                }
            }
        }

        private void SourcingFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourcingFunction>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<PractitionerExpertise> _practitionerExpertise;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access PractitionerExpertise - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>();
                    }
                    else
                    {
                        var items = base.SoAContext.PractitionerExpertise.Where(x => x.Procedure == this.ProcedureId).ToList<PractitionerExpertise>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<KnowledgeTestOutcome> _knowledgeTestOutcomes;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<KnowledgeTestOutcome> KnowledgeTestOutcomes
        {
            get
            {
                if (_knowledgeTestOutcomes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTestOutcomes - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _knowledgeTestOutcomes = new ObservableCollection<KnowledgeTestOutcome>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTestOutcomes.Where(x => x.Procedure == this.ProcedureId).ToList<KnowledgeTestOutcome>();
                        _knowledgeTestOutcomes = new ObservableCollection<KnowledgeTestOutcome>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTestOutcomes.CollectionChanged += KnowledgeTestOutcomes_CollectionChanged;
                }
                return _knowledgeTestOutcomes;
            }
            private set
            {
                if (_knowledgeTestOutcomes != null)
                {
                    _knowledgeTestOutcomes.CollectionChanged -= KnowledgeTestOutcomes_CollectionChanged;
                }
                _knowledgeTestOutcomes = value;
                if (_knowledgeTestOutcomes != null)
                {
                    _knowledgeTestOutcomes.CollectionChanged += KnowledgeTestOutcomes_CollectionChanged;
                }
            }
        }

        private void KnowledgeTestOutcomes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTestOutcome>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<KnowHowCarrier> _knowHowCarriers;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access KnowHowCarriers - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _knowHowCarriers = new ObservableCollection<KnowHowCarrier>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowHowCarriers.Where(x => x.Procedure == this.ProcedureId).ToList<KnowHowCarrier>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<KnowledgeRepositoryEntry> _knowledgeRepositoryEntries;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access KnowledgeRepositoryEntries - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _knowledgeRepositoryEntries = new ObservableCollection<KnowledgeRepositoryEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeRepositoryEntries.Where(x => x.Procedure == this.ProcedureId).ToList<KnowledgeRepositoryEntry>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<SourceRelationship> _sourceRelationships;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<SourceRelationship> SourceRelationships
        {
            get
            {
                if (_sourceRelationships == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SourceRelationships - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _sourceRelationships = new ObservableCollection<SourceRelationship>();
                    }
                    else
                    {
                        var items = base.SoAContext.SourceRelationships.Where(x => x.Procedure == this.ProcedureId).ToList<SourceRelationship>();
                        _sourceRelationships = new ObservableCollection<SourceRelationship>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _sourceRelationships.CollectionChanged += SourceRelationships_CollectionChanged;
                }
                return _sourceRelationships;
            }
            private set
            {
                if (_sourceRelationships != null)
                {
                    _sourceRelationships.CollectionChanged -= SourceRelationships_CollectionChanged;
                }
                _sourceRelationships = value;
                if (_sourceRelationships != null)
                {
                    _sourceRelationships.CollectionChanged += SourceRelationships_CollectionChanged;
                }
            }
        }

        private void SourceRelationships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SourceRelationship>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<DepartmentProcessAccount> _departmentProcessAccounts;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access DepartmentProcessAccounts - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _departmentProcessAccounts = new ObservableCollection<DepartmentProcessAccount>();
                    }
                    else
                    {
                        var items = base.SoAContext.DepartmentProcessAccounts.Where(x => x.Procedure == this.ProcedureId).ToList<DepartmentProcessAccount>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<ProblemOccurrence> _problemOccurrences;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access ProblemOccurrences - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _problemOccurrences = new ObservableCollection<ProblemOccurrence>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProblemOccurrences.Where(x => x.Procedure == this.ProcedureId).ToList<ProblemOccurrence>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<OnboardingRecord> _onboardingRecords;

        [InverseProperty("ProcedureRef")]
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
                            throw new InvalidOperationException("Cannot access OnboardingRecords - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _onboardingRecords = new ObservableCollection<OnboardingRecord>();
                    }
                    else
                    {
                        var items = base.SoAContext.OnboardingRecords.Where(x => x.Procedure == this.ProcedureId).ToList<OnboardingRecord>();
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
                    item.Procedure = this.ProcedureId;
                }
            }
        }

        private ObservableCollection<CollectionOccasion> _collectionOccasions;

        [InverseProperty("ProcedureRef")]
        public virtual ObservableCollection<CollectionOccasion> CollectionOccasions
        {
            get
            {
                if (_collectionOccasions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectionOccasions - no database context is set. ProcedureId: " + this.ProcedureId + ".");
                        }
                        _collectionOccasions = new ObservableCollection<CollectionOccasion>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectionOccasions.Where(x => x.Procedure == this.ProcedureId).ToList<CollectionOccasion>();
                        _collectionOccasions = new ObservableCollection<CollectionOccasion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectionOccasions.CollectionChanged += CollectionOccasions_CollectionChanged;
                }
                return _collectionOccasions;
            }
            private set
            {
                if (_collectionOccasions != null)
                {
                    _collectionOccasions.CollectionChanged -= CollectionOccasions_CollectionChanged;
                }
                _collectionOccasions = value;
                if (_collectionOccasions != null)
                {
                    _collectionOccasions.CollectionChanged += CollectionOccasions_CollectionChanged;
                }
            }
        }

        private void CollectionOccasions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectionOccasion>())
                {
                    item.Procedure = this.ProcedureId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureTypeRef;
            _ = this.Organization;
            _ = this.OrganizationRef;
            _ = this.Procedure;
            _ = this.RegulatoryFramework;
            _ = this.Procedures;
            _ = this.ProcedureVersions;
            _ = this.Steps;
            _ = this.CatalogEntryForResources;
            _ = this.ComplianceRecordForResources;
            _ = this.Vocabularies;
            _ = this.ClaimEvidence;
            _ = this.ProcedureTargets;
            _ = this.ProcedureAdoptions;
            _ = this.ProcedureOutcomeCriteria;
            _ = this.ProcessLevelStatements;
            _ = this.ProcessStrategicAlignments;
            _ = this.ProcessOutcomeMeasures;
            _ = this.FromProcedureProcessInterdependencies;
            _ = this.ToProcedureProcessInterdependencies;
            _ = this.ProcedureLensViews;
            _ = this.SituationalVariants;
            _ = this.CollectedSourceMaterials;
            _ = this.ProcedureFacetAssignments;
            _ = this.AiAdoptionInitiatives;
            _ = this.GovernedModels;
            _ = this.SourcingFunctions;
            _ = this.PractitionerExpertise;
            _ = this.KnowledgeTestOutcomes;
            _ = this.KnowHowCarriers;
            _ = this.KnowledgeRepositoryEntries;
            _ = this.SourceRelationships;
            _ = this.DepartmentProcessAccounts;
            _ = this.ProblemOccurrences;
            _ = this.OnboardingRecords;
            _ = this.CollectionOccasions;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
