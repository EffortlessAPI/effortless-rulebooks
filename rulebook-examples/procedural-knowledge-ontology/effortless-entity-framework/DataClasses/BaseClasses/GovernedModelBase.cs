
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
    [Table("GovernedModels")]
    public class GovernedModelBase : SoAEntityBase
    {
        [Key]
        public string GovernedModelId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? ModelKind { get; set; }
        public int? OrganizationHeadcount { get; set; }
        public string? RequirementsPurpose { get; set; }
        public string? IntendedUsers { get; set; }
        public DateTimeOffset? RegisteredAt { get; set; }
        public DateTimeOffset? FirstControlAdoptedAt { get; set; }
        public string? CurrentCharter { get; set; }
        public string? CurrentRelease { get; set; }
        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysSinceRegistered (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{RegisteredAt}}, "days"))
        [NotMapped]
        public int? DaysSinceRegistered
        {
            get => F.AsInt(F.Memo(this, "DaysSinceRegistered", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.RegisteredAt), F.S("days"))))); set { }
        }

        // Formula CharterCount (rulebook: =COUNTIFS(ModelCharters!{{GovernedModel}}, {{GovernedModelId}}))
        [NotMapped]
        public int? CharterCount
        {
            get => F.AsInt(F.Memo(this, "CharterCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelCharter>(base.SoAContext, "ModelCharters", __c => __c.ModelCharters), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId))))))); set { }
        }

        // Formula CurrentCharterCount (rulebook: =COUNTIFS(ModelCharters!{{GovernedModel}}, {{GovernedModelId}}, ModelCharters!{{IsCurrent}}, TRUE))
        [NotMapped]
        public int? CurrentCharterCount
        {
            get => F.AsInt(F.Memo(this, "CurrentCharterCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelCharter>(base.SoAContext, "ModelCharters", __c => __c.ModelCharters), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.IsCurrent), F.B(true))))))); set { }
        }

        // Formula CurrentStewardRole (rulebook: =INDEX(ModelCharters!{{StewardRole}}, MATCH({{CurrentCharter}}, ModelCharters!{{ModelCharterId}}, 0)))
        [NotMapped]
        public string? CurrentStewardRole
        {
            get => F.AsString(F.Memo(this, "CurrentStewardRole", () => F.Lookup<ModelCharter>(this, "ModelCharters", "ModelCharterId", __c => __c.ModelCharters, __r => F.Of(__r.ModelCharterId), F.Of(this.CurrentCharter), __r => F.Of(__r.StewardRole), () => F.Of(new ModelCharter().StewardRole)))); set { }
        }

        // Formula CurrentStewardAgent (rulebook: =INDEX(ModelCharters!{{StewardAgent}}, MATCH({{CurrentCharter}}, ModelCharters!{{ModelCharterId}}, 0)))
        [NotMapped]
        public string? CurrentStewardAgent
        {
            get => F.AsString(F.Memo(this, "CurrentStewardAgent", () => F.Lookup<ModelCharter>(this, "ModelCharters", "ModelCharterId", __c => __c.ModelCharters, __r => F.Of(__r.ModelCharterId), F.Of(this.CurrentCharter), __r => F.Of(__r.StewardAgent), () => F.Of(new ModelCharter().StewardAgent)))); set { }
        }

        // Formula CurrentAuthorityRole (rulebook: =INDEX(ModelCharters!{{AuthorityRole}}, MATCH({{CurrentCharter}}, ModelCharters!{{ModelCharterId}}, 0)))
        [NotMapped]
        public string? CurrentAuthorityRole
        {
            get => F.AsString(F.Memo(this, "CurrentAuthorityRole", () => F.Lookup<ModelCharter>(this, "ModelCharters", "ModelCharterId", __c => __c.ModelCharters, __r => F.Of(__r.ModelCharterId), F.Of(this.CurrentCharter), __r => F.Of(__r.AuthorityRole), () => F.Of(new ModelCharter().AuthorityRole)))); set { }
        }

        // Formula CurrentAuthorityAgent (rulebook: =INDEX(ModelCharters!{{AuthorityAgent}}, MATCH({{CurrentCharter}}, ModelCharters!{{ModelCharterId}}, 0)))
        [NotMapped]
        public string? CurrentAuthorityAgent
        {
            get => F.AsString(F.Memo(this, "CurrentAuthorityAgent", () => F.Lookup<ModelCharter>(this, "ModelCharters", "ModelCharterId", __c => __c.ModelCharters, __r => F.Of(__r.ModelCharterId), F.Of(this.CurrentCharter), __r => F.Of(__r.AuthorityAgent), () => F.Of(new ModelCharter().AuthorityAgent)))); set { }
        }

        // Formula IsOwnerless (rulebook: ={{CurrentCharterCount}} = 0)
        [NotMapped]
        public bool? IsOwnerless
        {
            get => F.AsBool(F.Memo(this, "IsOwnerless", () => F.Eq(F.Of(this.CurrentCharterCount), F.I(0)))); set { }
        }

        // Formula IsOwnerlessPastAYear (rulebook: =AND({{IsOwnerless}}, {{DaysSinceRegistered}} > 365))
        [NotMapped]
        public bool? IsOwnerlessPastAYear
        {
            get => F.AsBool(F.Memo(this, "IsOwnerlessPastAYear", () => F.And(F.Bool3(F.Of(this.IsOwnerless)), F.Bool3(F.Cmp(F.Of(this.DaysSinceRegistered), ">", F.I(365)))))); set { }
        }

        // Formula GovernanceLapsed (rulebook: =AND({{CharterCount}} > 0, {{CurrentCharterCount}} = 0))
        [NotMapped]
        public bool? GovernanceLapsed
        {
            get => F.AsBool(F.Memo(this, "GovernanceLapsed", () => F.And(F.Bool3(F.Cmp(F.Of(this.CharterCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CurrentCharterCount), F.I(0)))))); set { }
        }

        // Formula HasNoCurrentSteward (rulebook: ={{CurrentStewardAgent}} = "")
        [NotMapped]
        public bool? HasNoCurrentSteward
        {
            get => F.AsBool(F.Memo(this, "HasNoCurrentSteward", () => F.IsBlank(F.Of(this.CurrentStewardAgent)))); set { }
        }

        // Formula HasNoCurrentAuthority (rulebook: ={{CurrentAuthorityAgent}} = "")
        [NotMapped]
        public bool? HasNoCurrentAuthority
        {
            get => F.AsBool(F.Memo(this, "HasNoCurrentAuthority", () => F.IsBlank(F.Of(this.CurrentAuthorityAgent)))); set { }
        }

        // Formula IsProcedureWithoutChangeAuthority (rulebook: =AND({{ModelKind}} = "ProcedureFamily", {{CurrentAuthorityAgent}} = ""))
        [NotMapped]
        public bool? IsProcedureWithoutChangeAuthority
        {
            get => F.AsBool(F.Memo(this, "IsProcedureWithoutChangeAuthority", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ModelKind)), F.S("ProcedureFamily"))), F.Bool3(F.IsBlank(F.Of(this.CurrentAuthorityAgent)))))); set { }
        }

        // Formula LastStewardActivityAt (rulebook: =MAXIFS(StewardActivities!{{PerformedAt}}, StewardActivities!{{ActivityModel}}, {{GovernedModelId}}))
        [NotMapped]
        public DateTimeOffset? LastStewardActivityAt
        {
            get => F.AsDateTime(F.Memo(this, "LastStewardActivityAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<StewardActivity>(base.SoAContext, "StewardActivities", __c => __c.StewardActivities), __r => F.CritField(F.Of(__r.ActivityModel), F.Of(this.GovernedModelId)), __r => F.Of(__r.PerformedAt))))); set { }
        }

        // Formula DaysSinceStewardActivity (rulebook: =IF({{LastStewardActivityAt}} = "", {{DaysSinceRegistered}}, DATETIME_DIFF({{AsOfInstant}}, {{LastStewardActivityAt}}, "days")))
        [NotMapped]
        public int? DaysSinceStewardActivity
        {
            get => F.AsInt(F.Memo(this, "DaysSinceStewardActivity", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastStewardActivityAt)))) ? F.Of(this.DaysSinceRegistered) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastStewardActivityAt), F.S("days")))))); set { }
        }

        // Formula IsUnmaintained (rulebook: ={{DaysSinceStewardActivity}} > 90)
        [NotMapped]
        public bool? IsUnmaintained
        {
            get => F.AsBool(F.Memo(this, "IsUnmaintained", () => F.Cmp(F.Of(this.DaysSinceStewardActivity), ">", F.I(90)))); set { }
        }

        // Formula DocumentsBehindCount (rulebook: =COUNTIFS(ModelDocuments!{{GovernedModel}}, {{GovernedModelId}}, ModelDocuments!{{IsBehindCurrentRelease}}, TRUE))
        [NotMapped]
        public int? DocumentsBehindCount
        {
            get => F.AsInt(F.Memo(this, "DocumentsBehindCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelDocument>(base.SoAContext, "ModelDocuments", __c => __c.ModelDocuments), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.IsBehindCurrentRelease), F.B(true))))))); set { }
        }

        // Formula IsNotKeptCurrent (rulebook: =OR({{IsUnmaintained}}, {{DocumentsBehindCount}} > 0))
        [NotMapped]
        public bool? IsNotKeptCurrent
        {
            get => F.AsBool(F.Memo(this, "IsNotKeptCurrent", () => F.Or(F.Bool3(F.Of(this.IsUnmaintained)), F.Bool3(F.Cmp(F.Of(this.DocumentsBehindCount), ">", F.I(0)))))); set { }
        }

        // Formula OpenPracticeDriftCount (rulebook: =COUNTIFS(DriftObservations!{{GovernedModel}}, {{GovernedModelId}}, DriftObservations!{{IsOpenPracticeMismatch}}, TRUE))
        [NotMapped]
        public int? OpenPracticeDriftCount
        {
            get => F.AsInt(F.Memo(this, "OpenPracticeDriftCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<DriftObservation>(base.SoAContext, "DriftObservations", __c => __c.DriftObservations), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.IsOpenPracticeMismatch), F.B(true))))))); set { }
        }

        // Formula HasOpenPracticeDrift (rulebook: ={{OpenPracticeDriftCount}} > 0)
        [NotMapped]
        public bool? HasOpenPracticeDrift
        {
            get => F.AsBool(F.Memo(this, "HasOpenPracticeDrift", () => F.Cmp(F.Of(this.OpenPracticeDriftCount), ">", F.I(0)))); set { }
        }

        // Formula IsNeglectedAndDrifting (rulebook: =AND({{IsUnmaintained}}, {{OpenPracticeDriftCount}} > 0))
        [NotMapped]
        public bool? IsNeglectedAndDrifting
        {
            get => F.AsBool(F.Memo(this, "IsNeglectedAndDrifting", () => F.And(F.Bool3(F.Of(this.IsUnmaintained)), F.Bool3(F.Cmp(F.Of(this.OpenPracticeDriftCount), ">", F.I(0)))))); set { }
        }

        // Formula ExpertFoundDriftCount (rulebook: =COUNTIFS(DriftObservations!{{GovernedModel}}, {{GovernedModelId}}, DriftObservations!{{DetectedBy}}, "ExpertWrongAnswer"))
        [NotMapped]
        public int? ExpertFoundDriftCount
        {
            get => F.AsInt(F.Memo(this, "ExpertFoundDriftCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<DriftObservation>(base.SoAContext, "DriftObservations", __c => __c.DriftObservations), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.DetectedBy), F.S("ExpertWrongAnswer"))))))); set { }
        }

        // Formula CqReviewCount (rulebook: =COUNTIFS(CompetencyQuestionReviews!{{GovernedModel}}, {{GovernedModelId}}))
        [NotMapped]
        public int? CqReviewCount
        {
            get => F.AsInt(F.Memo(this, "CqReviewCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CompetencyQuestionReview>(base.SoAContext, "CompetencyQuestionReviews", __c => __c.CompetencyQuestionReviews), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId))))))); set { }
        }

        // Formula LastCqReviewAt (rulebook: =MAXIFS(CompetencyQuestionReviews!{{ReviewedAt}}, CompetencyQuestionReviews!{{GovernedModel}}, {{GovernedModelId}}))
        [NotMapped]
        public DateTimeOffset? LastCqReviewAt
        {
            get => F.AsDateTime(F.Memo(this, "LastCqReviewAt", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<CompetencyQuestionReview>(base.SoAContext, "CompetencyQuestionReviews", __c => __c.CompetencyQuestionReviews), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)), __r => F.Of(__r.ReviewedAt))))); set { }
        }

        // Formula DaysSinceCqReview (rulebook: =IF({{LastCqReviewAt}} = "", {{DaysSinceRegistered}}, DATETIME_DIFF({{AsOfInstant}}, {{LastCqReviewAt}}, "days")))
        [NotMapped]
        public int? DaysSinceCqReview
        {
            get => F.AsInt(F.Memo(this, "DaysSinceCqReview", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.LastCqReviewAt)))) ? F.Of(this.DaysSinceRegistered) : F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.LastCqReviewAt), F.S("days")))))); set { }
        }

        // Formula CqReviewOverdue (rulebook: ={{DaysSinceCqReview}} > 92)
        [NotMapped]
        public bool? CqReviewOverdue
        {
            get => F.AsBool(F.Memo(this, "CqReviewOverdue", () => F.Cmp(F.Of(this.DaysSinceCqReview), ">", F.I(92)))); set { }
        }

        // Formula DegradationHiddenUntilWrongAnswer (rulebook: =AND({{ExpertFoundDriftCount}} > 0, {{CqReviewCount}} = 0))
        [NotMapped]
        public bool? DegradationHiddenUntilWrongAnswer
        {
            get => F.AsBool(F.Memo(this, "DegradationHiddenUntilWrongAnswer", () => F.And(F.Bool3(F.Cmp(F.Of(this.ExpertFoundDriftCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CqReviewCount), F.I(0)))))); set { }
        }

        // Formula BaselineQuestionCount (rulebook: =COUNTIFS(CompetencyQuestionSetEntries!{{GovernedModel}}, {{GovernedModelId}}, CompetencyQuestionSetEntries!{{IsOriginalBaseline}}, TRUE))
        [NotMapped]
        public int? BaselineQuestionCount
        {
            get => F.AsInt(F.Memo(this, "BaselineQuestionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CompetencyQuestionSetEntry>(base.SoAContext, "CompetencyQuestionSetEntries", __c => __c.CompetencyQuestionSetEntries), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.IsOriginalBaseline), F.B(true))))))); set { }
        }

        // Formula RequirementsSpecIncomplete (rulebook: =OR({{RequirementsPurpose}} = "", {{IntendedUsers}} = "", {{BaselineQuestionCount}} = 0))
        [NotMapped]
        public bool? RequirementsSpecIncomplete
        {
            get => F.AsBool(F.Memo(this, "RequirementsSpecIncomplete", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.RequirementsPurpose))), F.Bool3(F.IsBlank(F.Of(this.IntendedUsers))), F.Bool3(F.Eq(F.Of(this.BaselineQuestionCount), F.I(0)))))); set { }
        }

        // Formula CollectionControlCount (rulebook: =COUNTIFS(GovernanceStageControls!{{GovernedModel}}, {{GovernedModelId}}, GovernanceStageControls!{{Stage}}, "Collection"))
        [NotMapped]
        public int? CollectionControlCount
        {
            get => F.AsInt(F.Memo(this, "CollectionControlCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<GovernanceStageControl>(base.SoAContext, "GovernanceStageControls", __c => __c.GovernanceStageControls), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.Stage), F.S("Collection"))))))); set { }
        }

        // Formula StewardshipControlCount (rulebook: =COUNTIFS(GovernanceStageControls!{{GovernedModel}}, {{GovernedModelId}}, GovernanceStageControls!{{Stage}}, "Stewardship"))
        [NotMapped]
        public int? StewardshipControlCount
        {
            get => F.AsInt(F.Memo(this, "StewardshipControlCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<GovernanceStageControl>(base.SoAContext, "GovernanceStageControls", __c => __c.GovernanceStageControls), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.Stage), F.S("Stewardship"))))))); set { }
        }

        // Formula RetrievalControlCount (rulebook: =COUNTIFS(GovernanceStageControls!{{GovernedModel}}, {{GovernedModelId}}, GovernanceStageControls!{{Stage}}, "Retrieval"))
        [NotMapped]
        public int? RetrievalControlCount
        {
            get => F.AsInt(F.Memo(this, "RetrievalControlCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<GovernanceStageControl>(base.SoAContext, "GovernanceStageControls", __c => __c.GovernanceStageControls), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.Stage), F.S("Retrieval"))))))); set { }
        }

        // Formula UseControlCount (rulebook: =COUNTIFS(GovernanceStageControls!{{GovernedModel}}, {{GovernedModelId}}, GovernanceStageControls!{{Stage}}, "Use"))
        [NotMapped]
        public int? UseControlCount
        {
            get => F.AsInt(F.Memo(this, "UseControlCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<GovernanceStageControl>(base.SoAContext, "GovernanceStageControls", __c => __c.GovernanceStageControls), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.Stage), F.S("Use"))))))); set { }
        }

        // Formula ContinuousPipelineStageCount (rulebook: =COUNTIFS(GovernanceStageControls!{{GovernedModel}}, {{GovernedModelId}}, GovernanceStageControls!{{IsContinuousPipelineStage}}, TRUE))
        [NotMapped]
        public int? ContinuousPipelineStageCount
        {
            get => F.AsInt(F.Memo(this, "ContinuousPipelineStageCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<GovernanceStageControl>(base.SoAContext, "GovernanceStageControls", __c => __c.GovernanceStageControls), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.IsContinuousPipelineStage), F.B(true))))))); set { }
        }

        // Formula LacksLifecycleStageControl (rulebook: =AND({{ModelKind}} = "ProcedureFamily", OR({{CollectionControlCount}} = 0, {{StewardshipControlCount}} = 0, {{RetrievalControlCount}} = 0, {{UseControlCount}} = 0)))
        [NotMapped]
        public bool? LacksLifecycleStageControl
        {
            get => F.AsBool(F.Memo(this, "LacksLifecycleStageControl", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ModelKind)), F.S("ProcedureFamily"))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.CollectionControlCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.StewardshipControlCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.RetrievalControlCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.UseControlCount), F.I(0)))))))); set { }
        }

        // Formula PipelineNotContinuous (rulebook: =AND({{ModelKind}} = "ProcedureFamily", {{ContinuousPipelineStageCount}} < 3))
        [NotMapped]
        public bool? PipelineNotContinuous
        {
            get => F.AsBool(F.Memo(this, "PipelineNotContinuous", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ModelKind)), F.S("ProcedureFamily"))), F.Bool3(F.Cmp(F.Of(this.ContinuousPipelineStageCount), "<", F.I(3)))))); set { }
        }

        // Formula ProcedureHasNoExplicitSteps (rulebook: =INDEX(Procedures!{{HasNoExplicitSteps}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public bool? ProcedureHasNoExplicitSteps
        {
            get => F.AsBool(F.Memo(this, "ProcedureHasNoExplicitSteps", () => F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.HasNoExplicitSteps), () => F.Of(new Procedure().HasNoExplicitSteps)))); set { }
        }

        // Formula IsUnmanageableUndocumentedWork (rulebook: =AND({{ModelKind}} = "ProcedureFamily", {{Procedure}} <> "", {{ProcedureHasNoExplicitSteps}}))
        [NotMapped]
        public bool? IsUnmanageableUndocumentedWork
        {
            get => F.AsBool(F.Memo(this, "IsUnmanageableUndocumentedWork", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ModelKind)), F.S("ProcedureFamily"))), F.Bool3(F.IsNotBlank(F.Of(this.Procedure))), F.Bool3(F.Of(this.ProcedureHasNoExplicitSteps))))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public string? OriginatingUseCase { get; set; }
        // Formula IsWithoutOriginatingUseCase (rulebook: =AND({{ModelKind}} <> "Vocabulary", {{OriginatingUseCase}} = ""))
        [NotMapped]
        public bool? IsWithoutOriginatingUseCase
        {
            get => F.AsBool(F.Memo(this, "IsWithoutOriginatingUseCase", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.ModelKind)), F.S("Vocabulary"))), F.Bool3(F.IsBlank(F.Of(this.OriginatingUseCase)))))); set { }
        }

        // Formula PilotCount (rulebook: =COUNTIFS(ModelPilots!{{GovernedModel}}, {{GovernedModelId}}))
        [NotMapped]
        public int? PilotCount
        {
            get => F.AsInt(F.Memo(this, "PilotCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelPilot>(base.SoAContext, "ModelPilots", __c => __c.ModelPilots), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId))))))); set { }
        }

        // Formula IsAdoptedWithoutPilot (rulebook: =AND({{ModelKind}} <> "Vocabulary", {{PilotCount}} = 0))
        [NotMapped]
        public bool? IsAdoptedWithoutPilot
        {
            get => F.AsBool(F.Memo(this, "IsAdoptedWithoutPilot", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.ModelKind)), F.S("Vocabulary"))), F.Bool3(F.Eq(F.Of(this.PilotCount), F.I(0)))))); set { }
        }

        // Formula RequirementsExpertCount (rulebook: =COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "RequirementsSpecification"))
        [NotMapped]
        public int? RequirementsExpertCount
        {
            get => F.AsInt(F.Memo(this, "RequirementsExpertCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelActivityExpert>(base.SoAContext, "ModelActivityExperts", __c => __c.ModelActivityExperts), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.LotActivity), F.S("RequirementsSpecification"))))))); set { }
        }

        // Formula ImplementationExpertCount (rulebook: =COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "Implementation"))
        [NotMapped]
        public int? ImplementationExpertCount
        {
            get => F.AsInt(F.Memo(this, "ImplementationExpertCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelActivityExpert>(base.SoAContext, "ModelActivityExperts", __c => __c.ModelActivityExperts), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.LotActivity), F.S("Implementation"))))))); set { }
        }

        // Formula PublicationExpertCount (rulebook: =COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "Publication"))
        [NotMapped]
        public int? PublicationExpertCount
        {
            get => F.AsInt(F.Memo(this, "PublicationExpertCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelActivityExpert>(base.SoAContext, "ModelActivityExperts", __c => __c.ModelActivityExperts), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.LotActivity), F.S("Publication"))))))); set { }
        }

        // Formula MaintenanceExpertCount (rulebook: =COUNTIFS(ModelActivityExperts!{{GovernedModel}}, {{GovernedModelId}}, ModelActivityExperts!{{LotActivity}}, "Maintenance"))
        [NotMapped]
        public int? MaintenanceExpertCount
        {
            get => F.AsInt(F.Memo(this, "MaintenanceExpertCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelActivityExpert>(base.SoAContext, "ModelActivityExperts", __c => __c.ModelActivityExperts), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.LotActivity), F.S("Maintenance"))))))); set { }
        }

        // Formula ExpertsNotInvolvedThroughout (rulebook: =AND({{ModelKind}} <> "Vocabulary", OR({{RequirementsExpertCount}} = 0, {{ImplementationExpertCount}} = 0, {{PublicationExpertCount}} = 0, {{MaintenanceExpertCount}} = 0)))
        [NotMapped]
        public bool? ExpertsNotInvolvedThroughout
        {
            get => F.AsBool(F.Memo(this, "ExpertsNotInvolvedThroughout", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.ModelKind)), F.S("Vocabulary"))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.RequirementsExpertCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.ImplementationExpertCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.PublicationExpertCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.MaintenanceExpertCount), F.I(0)))))))); set { }
        }

        // Formula RealDataMappingRunCount (rulebook: =COUNTIFS(ModelDataMappingRuns!{{GovernedModel}}, {{GovernedModelId}}, ModelDataMappingRuns!{{DataOrigin}}, "Real"))
        [NotMapped]
        public int? RealDataMappingRunCount
        {
            get => F.AsInt(F.Memo(this, "RealDataMappingRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelDataMappingRun>(base.SoAContext, "ModelDataMappingRuns", __c => __c.ModelDataMappingRuns), __r => F.CritField(F.Of(__r.GovernedModel), F.Of(this.GovernedModelId)) && F.CritLiteral(F.Of(__r.DataOrigin), F.S("Real"))))))); set { }
        }

        // Formula IsImplementedWithoutRealData (rulebook: =AND({{ModelKind}} <> "Vocabulary", {{RealDataMappingRunCount}} = 0))
        [NotMapped]
        public bool? IsImplementedWithoutRealData
        {
            get => F.AsBool(F.Memo(this, "IsImplementedWithoutRealData", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.ModelKind)), F.S("Vocabulary"))), F.Bool3(F.Eq(F.Of(this.RealDataMappingRunCount), F.I(0)))))); set { }
        }


        public string? Procedure { get; set; }
        public string? DomainOwningOrganization { get; set; }
        public string? ToolingOwnerRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        private Organization _organization;

        [ForeignKey("DomainOwningOrganization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(DomainOwningOrganization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. DomainOwningOrganization: " + DomainOwningOrganization + ".");
                        }
                        return null;
                    }
                    _organization = base.SoAContext.Organizations.Find(DomainOwningOrganization);
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
                        DomainOwningOrganization = _organization.OrganizationId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("ToolingOwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(ToolingOwnerRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. ToolingOwnerRole: " + ToolingOwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(ToolingOwnerRole);
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
                        ToolingOwnerRole = _role.RoleId;
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

        private ObservableCollection<RulebookRelease> _rulebookReleases;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<RulebookRelease> RulebookReleases
        {
            get
            {
                if (_rulebookReleases == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookReleases - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _rulebookReleases = new ObservableCollection<RulebookRelease>();
                    }
                    else
                    {
                        var items = base.SoAContext.RulebookReleases.Where(x => x.GovernedModel == this.GovernedModelId).ToList<RulebookRelease>();
                        _rulebookReleases = new ObservableCollection<RulebookRelease>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _rulebookReleases.CollectionChanged += RulebookReleases_CollectionChanged;
                }
                return _rulebookReleases;
            }
            private set
            {
                if (_rulebookReleases != null)
                {
                    _rulebookReleases.CollectionChanged -= RulebookReleases_CollectionChanged;
                }
                _rulebookReleases = value;
                if (_rulebookReleases != null)
                {
                    _rulebookReleases.CollectionChanged += RulebookReleases_CollectionChanged;
                }
            }
        }

        private void RulebookReleases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RulebookRelease>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelCharter> _modelCharters;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access ModelCharters - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelCharters = new ObservableCollection<ModelCharter>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelCharters.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelCharter>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelChangeRequest> _modelChangeRequests;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<ModelChangeRequest> ModelChangeRequests
        {
            get
            {
                if (_modelChangeRequests == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelChangeRequests - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelChangeRequests = new ObservableCollection<ModelChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeRequests.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelChangeRequest>();
                        _modelChangeRequests = new ObservableCollection<ModelChangeRequest>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelChangeRequests.CollectionChanged += ModelChangeRequests_CollectionChanged;
                }
                return _modelChangeRequests;
            }
            private set
            {
                if (_modelChangeRequests != null)
                {
                    _modelChangeRequests.CollectionChanged -= ModelChangeRequests_CollectionChanged;
                }
                _modelChangeRequests = value;
                if (_modelChangeRequests != null)
                {
                    _modelChangeRequests.CollectionChanged += ModelChangeRequests_CollectionChanged;
                }
            }
        }

        private void ModelChangeRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelChangeRequest>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ChangeAuthorityRule> _changeAuthorityRules;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<ChangeAuthorityRule> ChangeAuthorityRules
        {
            get
            {
                if (_changeAuthorityRules == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeAuthorityRules - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _changeAuthorityRules = new ObservableCollection<ChangeAuthorityRule>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeAuthorityRules.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ChangeAuthorityRule>();
                        _changeAuthorityRules = new ObservableCollection<ChangeAuthorityRule>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeAuthorityRules.CollectionChanged += ChangeAuthorityRules_CollectionChanged;
                }
                return _changeAuthorityRules;
            }
            private set
            {
                if (_changeAuthorityRules != null)
                {
                    _changeAuthorityRules.CollectionChanged -= ChangeAuthorityRules_CollectionChanged;
                }
                _changeAuthorityRules = value;
                if (_changeAuthorityRules != null)
                {
                    _changeAuthorityRules.CollectionChanged += ChangeAuthorityRules_CollectionChanged;
                }
            }
        }

        private void ChangeAuthorityRules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeAuthorityRule>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelConsumer> _modelConsumers;

        [InverseProperty("GovernedModel")]
        public virtual ObservableCollection<ModelConsumer> ModelConsumers
        {
            get
            {
                if (_modelConsumers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelConsumers - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelConsumers = new ObservableCollection<ModelConsumer>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelConsumers.Where(x => x.DependsOnModel == this.GovernedModelId).ToList<ModelConsumer>();
                        _modelConsumers = new ObservableCollection<ModelConsumer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelConsumers.CollectionChanged += ModelConsumers_CollectionChanged;
                }
                return _modelConsumers;
            }
            private set
            {
                if (_modelConsumers != null)
                {
                    _modelConsumers.CollectionChanged -= ModelConsumers_CollectionChanged;
                }
                _modelConsumers = value;
                if (_modelConsumers != null)
                {
                    _modelConsumers.CollectionChanged += ModelConsumers_CollectionChanged;
                }
            }
        }

        private void ModelConsumers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelConsumer>())
                {
                    item.DependsOnModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelDocument> _modelDocuments;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<ModelDocument> ModelDocuments
        {
            get
            {
                if (_modelDocuments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelDocuments - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelDocuments = new ObservableCollection<ModelDocument>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelDocuments.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelDocument>();
                        _modelDocuments = new ObservableCollection<ModelDocument>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelDocuments.CollectionChanged += ModelDocuments_CollectionChanged;
                }
                return _modelDocuments;
            }
            private set
            {
                if (_modelDocuments != null)
                {
                    _modelDocuments.CollectionChanged -= ModelDocuments_CollectionChanged;
                }
                _modelDocuments = value;
                if (_modelDocuments != null)
                {
                    _modelDocuments.CollectionChanged += ModelDocuments_CollectionChanged;
                }
            }
        }

        private void ModelDocuments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelDocument>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<StalenessQueryRun> _stalenessQueryRuns;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access StalenessQueryRuns - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _stalenessQueryRuns = new ObservableCollection<StalenessQueryRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.StalenessQueryRuns.Where(x => x.GovernedModel == this.GovernedModelId).ToList<StalenessQueryRun>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<StakeholderQuestion> _stakeholderQuestions;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<StakeholderQuestion> StakeholderQuestions
        {
            get
            {
                if (_stakeholderQuestions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderQuestions - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _stakeholderQuestions = new ObservableCollection<StakeholderQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderQuestions.Where(x => x.GovernedModel == this.GovernedModelId).ToList<StakeholderQuestion>();
                        _stakeholderQuestions = new ObservableCollection<StakeholderQuestion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderQuestions.CollectionChanged += StakeholderQuestions_CollectionChanged;
                }
                return _stakeholderQuestions;
            }
            private set
            {
                if (_stakeholderQuestions != null)
                {
                    _stakeholderQuestions.CollectionChanged -= StakeholderQuestions_CollectionChanged;
                }
                _stakeholderQuestions = value;
                if (_stakeholderQuestions != null)
                {
                    _stakeholderQuestions.CollectionChanged += StakeholderQuestions_CollectionChanged;
                }
            }
        }

        private void StakeholderQuestions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderQuestion>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelExpansionRequest> _modelExpansionRequests;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access ModelExpansionRequests - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelExpansionRequests = new ObservableCollection<ModelExpansionRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelExpansionRequests.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelExpansionRequest>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<CompetencyQuestionSetEntry> _competencyQuestionSetEntries;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<CompetencyQuestionSetEntry> CompetencyQuestionSetEntries
        {
            get
            {
                if (_competencyQuestionSetEntries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionSetEntries - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _competencyQuestionSetEntries = new ObservableCollection<CompetencyQuestionSetEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionSetEntries.Where(x => x.GovernedModel == this.GovernedModelId).ToList<CompetencyQuestionSetEntry>();
                        _competencyQuestionSetEntries = new ObservableCollection<CompetencyQuestionSetEntry>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestionSetEntries.CollectionChanged += CompetencyQuestionSetEntries_CollectionChanged;
                }
                return _competencyQuestionSetEntries;
            }
            private set
            {
                if (_competencyQuestionSetEntries != null)
                {
                    _competencyQuestionSetEntries.CollectionChanged -= CompetencyQuestionSetEntries_CollectionChanged;
                }
                _competencyQuestionSetEntries = value;
                if (_competencyQuestionSetEntries != null)
                {
                    _competencyQuestionSetEntries.CollectionChanged += CompetencyQuestionSetEntries_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestionSetEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestionSetEntry>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<CompetencyQuestionReview> _competencyQuestionReviews;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access CompetencyQuestionReviews - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _competencyQuestionReviews = new ObservableCollection<CompetencyQuestionReview>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionReviews.Where(x => x.GovernedModel == this.GovernedModelId).ToList<CompetencyQuestionReview>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelProposal> _modelProposals;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<ModelProposal> ModelProposals
        {
            get
            {
                if (_modelProposals == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelProposals - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelProposal>();
                        _modelProposals = new ObservableCollection<ModelProposal>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelProposals.CollectionChanged += ModelProposals_CollectionChanged;
                }
                return _modelProposals;
            }
            private set
            {
                if (_modelProposals != null)
                {
                    _modelProposals.CollectionChanged -= ModelProposals_CollectionChanged;
                }
                _modelProposals = value;
                if (_modelProposals != null)
                {
                    _modelProposals.CollectionChanged += ModelProposals_CollectionChanged;
                }
            }
        }

        private void ModelProposals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelProposal>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<InstanceDataVersion> _instanceDataVersions;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<InstanceDataVersion> InstanceDataVersions
        {
            get
            {
                if (_instanceDataVersions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InstanceDataVersions - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _instanceDataVersions = new ObservableCollection<InstanceDataVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.InstanceDataVersions.Where(x => x.GovernedModel == this.GovernedModelId).ToList<InstanceDataVersion>();
                        _instanceDataVersions = new ObservableCollection<InstanceDataVersion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _instanceDataVersions.CollectionChanged += InstanceDataVersions_CollectionChanged;
                }
                return _instanceDataVersions;
            }
            private set
            {
                if (_instanceDataVersions != null)
                {
                    _instanceDataVersions.CollectionChanged -= InstanceDataVersions_CollectionChanged;
                }
                _instanceDataVersions = value;
                if (_instanceDataVersions != null)
                {
                    _instanceDataVersions.CollectionChanged += InstanceDataVersions_CollectionChanged;
                }
            }
        }

        private void InstanceDataVersions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<InstanceDataVersion>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<DomainCoverageArea> _domainCoverageAreas;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<DomainCoverageArea> DomainCoverageAreas
        {
            get
            {
                if (_domainCoverageAreas == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DomainCoverageAreas - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _domainCoverageAreas = new ObservableCollection<DomainCoverageArea>();
                    }
                    else
                    {
                        var items = base.SoAContext.DomainCoverageAreas.Where(x => x.GovernedModel == this.GovernedModelId).ToList<DomainCoverageArea>();
                        _domainCoverageAreas = new ObservableCollection<DomainCoverageArea>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _domainCoverageAreas.CollectionChanged += DomainCoverageAreas_CollectionChanged;
                }
                return _domainCoverageAreas;
            }
            private set
            {
                if (_domainCoverageAreas != null)
                {
                    _domainCoverageAreas.CollectionChanged -= DomainCoverageAreas_CollectionChanged;
                }
                _domainCoverageAreas = value;
                if (_domainCoverageAreas != null)
                {
                    _domainCoverageAreas.CollectionChanged += DomainCoverageAreas_CollectionChanged;
                }
            }
        }

        private void DomainCoverageAreas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DomainCoverageArea>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<GovernanceStageControl> _governanceStageControls;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<GovernanceStageControl> GovernanceStageControls
        {
            get
            {
                if (_governanceStageControls == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernanceStageControls - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _governanceStageControls = new ObservableCollection<GovernanceStageControl>();
                    }
                    else
                    {
                        var items = base.SoAContext.GovernanceStageControls.Where(x => x.GovernedModel == this.GovernedModelId).ToList<GovernanceStageControl>();
                        _governanceStageControls = new ObservableCollection<GovernanceStageControl>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _governanceStageControls.CollectionChanged += GovernanceStageControls_CollectionChanged;
                }
                return _governanceStageControls;
            }
            private set
            {
                if (_governanceStageControls != null)
                {
                    _governanceStageControls.CollectionChanged -= GovernanceStageControls_CollectionChanged;
                }
                _governanceStageControls = value;
                if (_governanceStageControls != null)
                {
                    _governanceStageControls.CollectionChanged += GovernanceStageControls_CollectionChanged;
                }
            }
        }

        private void GovernanceStageControls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<GovernanceStageControl>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelChangeLogEntry>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<DriftObservation> _driftObservations;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access DriftObservations - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _driftObservations = new ObservableCollection<DriftObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.DriftObservations.Where(x => x.GovernedModel == this.GovernedModelId).ToList<DriftObservation>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelPilot> _modelPilots;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<ModelPilot> ModelPilots
        {
            get
            {
                if (_modelPilots == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelPilots - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelPilots = new ObservableCollection<ModelPilot>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelPilots.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelPilot>();
                        _modelPilots = new ObservableCollection<ModelPilot>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelPilots.CollectionChanged += ModelPilots_CollectionChanged;
                }
                return _modelPilots;
            }
            private set
            {
                if (_modelPilots != null)
                {
                    _modelPilots.CollectionChanged -= ModelPilots_CollectionChanged;
                }
                _modelPilots = value;
                if (_modelPilots != null)
                {
                    _modelPilots.CollectionChanged += ModelPilots_CollectionChanged;
                }
            }
        }

        private void ModelPilots_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelPilot>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelActivityExpert> _modelActivityExperts;

        [InverseProperty("GovernedModelRef")]
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
                            throw new InvalidOperationException("Cannot access ModelActivityExperts - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelActivityExperts = new ObservableCollection<ModelActivityExpert>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelActivityExperts.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelActivityExpert>();
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
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }

        private ObservableCollection<ModelDataMappingRun> _modelDataMappingRuns;

        [InverseProperty("GovernedModelRef")]
        public virtual ObservableCollection<ModelDataMappingRun> ModelDataMappingRuns
        {
            get
            {
                if (_modelDataMappingRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelDataMappingRuns - no database context is set. GovernedModelId: " + this.GovernedModelId + ".");
                        }
                        _modelDataMappingRuns = new ObservableCollection<ModelDataMappingRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelDataMappingRuns.Where(x => x.GovernedModel == this.GovernedModelId).ToList<ModelDataMappingRun>();
                        _modelDataMappingRuns = new ObservableCollection<ModelDataMappingRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelDataMappingRuns.CollectionChanged += ModelDataMappingRuns_CollectionChanged;
                }
                return _modelDataMappingRuns;
            }
            private set
            {
                if (_modelDataMappingRuns != null)
                {
                    _modelDataMappingRuns.CollectionChanged -= ModelDataMappingRuns_CollectionChanged;
                }
                _modelDataMappingRuns = value;
                if (_modelDataMappingRuns != null)
                {
                    _modelDataMappingRuns.CollectionChanged += ModelDataMappingRuns_CollectionChanged;
                }
            }
        }

        private void ModelDataMappingRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelDataMappingRun>())
                {
                    item.GovernedModel = this.GovernedModelId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureRef;
            _ = this.Organization;
            _ = this.Role;
            _ = this.EvaluationContextRef;
            _ = this.RulebookReleases;
            _ = this.ModelCharters;
            _ = this.ModelChangeRequests;
            _ = this.ChangeAuthorityRules;
            _ = this.ModelConsumers;
            _ = this.ModelDocuments;
            _ = this.StalenessQueryRuns;
            _ = this.StakeholderQuestions;
            _ = this.ModelExpansionRequests;
            _ = this.CompetencyQuestionSetEntries;
            _ = this.CompetencyQuestionReviews;
            _ = this.ModelProposals;
            _ = this.InstanceDataVersions;
            _ = this.DomainCoverageAreas;
            _ = this.GovernanceStageControls;
            _ = this.ModelChangeLogEntries;
            _ = this.DriftObservations;
            _ = this.ModelPilots;
            _ = this.ModelActivityExperts;
            _ = this.ModelDataMappingRuns;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
