
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
    [Table("RulebookReleases")]
    public class RulebookReleaseBase : SoAEntityBase
    {
        [Key]
        public string RulebookReleaseId { get; set; }

        // Formula Name (rulebook: ={{RulebookVersion}} & " / PKO " & {{PkoCoreVersionIri}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.RulebookVersion)), F.S(" / PKO "), F.Text(F.Of(this.PkoCoreVersionIri))))); set { }
        }

        public string? RulebookVersion { get; set; }
        public string? ProfileVersion { get; set; }
        public string? ProfileSchemaPath { get; set; }
        public string? PkoCoreVersionIri { get; set; }
        public string? PkoIndustryVersionIri { get; set; }
        public DateTimeOffset? IssuedAt { get; set; }
        public string? Status { get; set; }
        public string? Changelog { get; set; }
        public bool? IsCurrent { get; set; }
        public string? DeclaredScale { get; set; }
        public int? VersionMajor { get; set; }
        public int? VersionMinor { get; set; }
        public int? VersionPatch { get; set; }
        public string? MigrationPlan { get; set; }
        public string? VersionDecisionRationale { get; set; }
        public string? RulebookCommit { get; set; }
        public string? License { get; set; }
        public string? PermanentIri { get; set; }
        public string? NamespacePrefix { get; set; }
        public string? FundingNote { get; set; }
        // Formula PrevMajor (rulebook: =INDEX(RulebookReleases!{{VersionMajor}}, MATCH({{PreviousRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public int? PrevMajor
        {
            get => F.AsInt(F.Memo(this, "PrevMajor", () => F.Integer(F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.PreviousRelease), __r => F.Of(__r.VersionMajor), () => F.Of(new RulebookRelease().VersionMajor))))); set { }
        }

        // Formula PrevMinor (rulebook: =INDEX(RulebookReleases!{{VersionMinor}}, MATCH({{PreviousRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public int? PrevMinor
        {
            get => F.AsInt(F.Memo(this, "PrevMinor", () => F.Integer(F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.PreviousRelease), __r => F.Of(__r.VersionMinor), () => F.Of(new RulebookRelease().VersionMinor))))); set { }
        }

        // Formula PrevPatch (rulebook: =INDEX(RulebookReleases!{{VersionPatch}}, MATCH({{PreviousRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public int? PrevPatch
        {
            get => F.AsInt(F.Memo(this, "PrevPatch", () => F.Integer(F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.PreviousRelease), __r => F.Of(__r.VersionPatch), () => F.Of(new RulebookRelease().VersionPatch))))); set { }
        }

        // Formula PrevIssuedAt (rulebook: =INDEX(RulebookReleases!{{IssuedAt}}, MATCH({{PreviousRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public DateTimeOffset? PrevIssuedAt
        {
            get => F.AsDateTime(F.Memo(this, "PrevIssuedAt", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.PreviousRelease), __r => F.Of(__r.IssuedAt), () => F.Of(new RulebookRelease().IssuedAt)))); set { }
        }

        // Formula ModelCurrentRelease (rulebook: =INDEX(GovernedModels!{{CurrentRelease}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelCurrentRelease
        {
            get => F.AsString(F.Memo(this, "ModelCurrentRelease", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.CurrentRelease), () => F.Of(new GovernedModel().CurrentRelease)))); set { }
        }

        // Formula ExpectedMajor (rulebook: =IF({{DeclaredScale}} = "Major", {{PrevMajor}} + 1, {{PrevMajor}}))
        [NotMapped]
        public int? ExpectedMajor
        {
            get => F.AsInt(F.Memo(this, "ExpectedMajor", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major")))) ? F.Add(F.Of(this.PrevMajor), F.I(1)) : F.Of(this.PrevMajor))))); set { }
        }

        // Formula ExpectedMinor (rulebook: =IF({{DeclaredScale}} = "Major", 0, IF({{DeclaredScale}} = "Minor", {{PrevMinor}} + 1, {{PrevMinor}})))
        [NotMapped]
        public int? ExpectedMinor
        {
            get => F.AsInt(F.Memo(this, "ExpectedMinor", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major")))) ? F.I(0) : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Minor")))) ? F.Add(F.Of(this.PrevMinor), F.I(1)) : F.Of(this.PrevMinor)))))); set { }
        }

        // Formula ExpectedPatch (rulebook: =IF({{DeclaredScale}} = "Patch", {{PrevPatch}} + 1, 0))
        [NotMapped]
        public int? ExpectedPatch
        {
            get => F.AsInt(F.Memo(this, "ExpectedPatch", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Patch")))) ? F.Add(F.Of(this.PrevPatch), F.I(1)) : F.I(0))))); set { }
        }

        // Formula IsIncrementInconsistentWithScale (rulebook: =AND({{PreviousRelease}} <> "", OR({{VersionMajor}} <> {{ExpectedMajor}}, {{VersionMinor}} <> {{ExpectedMinor}}, {{VersionPatch}} <> {{ExpectedPatch}})))
        [NotMapped]
        public bool? IsIncrementInconsistentWithScale
        {
            get => F.AsBool(F.Memo(this, "IsIncrementInconsistentWithScale", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PreviousRelease))), F.Bool3(F.Or(F.Bool3(F.Ne(F.Nullif(F.Of(this.VersionMajor)), F.Of(this.ExpectedMajor))), F.Bool3(F.Ne(F.Nullif(F.Of(this.VersionMinor)), F.Of(this.ExpectedMinor))), F.Bool3(F.Ne(F.Nullif(F.Of(this.VersionPatch)), F.Of(this.ExpectedPatch)))))))); set { }
        }

        // Formula DaysSincePreviousRelease (rulebook: =IF({{PreviousRelease}} = "", 0, DATETIME_DIFF({{IssuedAt}}, {{PrevIssuedAt}}, "days")))
        [NotMapped]
        public int? DaysSincePreviousRelease
        {
            get => F.AsInt(F.Memo(this, "DaysSincePreviousRelease", () => F.Integer((F.Truthy(F.Bool3(F.IsBlank(F.Of(this.PreviousRelease)))) ? F.I(0) : F.DatetimeDiff(F.Of(this.IssuedAt), F.Of(this.PrevIssuedAt), F.S("days")))))); set { }
        }

        // Formula IsLongReleaseCycle (rulebook: ={{DaysSincePreviousRelease}} > 60)
        [NotMapped]
        public bool? IsLongReleaseCycle
        {
            get => F.AsBool(F.Memo(this, "IsLongReleaseCycle", () => F.Cmp(F.Of(this.DaysSincePreviousRelease), ">", F.I(60)))); set { }
        }

        // Formula IsDeclaredCurrentRelease (rulebook: =AND({{ModelCurrentRelease}} <> "", {{RulebookReleaseId}} = {{ModelCurrentRelease}}))
        [NotMapped]
        public bool? IsDeclaredCurrentRelease
        {
            get => F.AsBool(F.Memo(this, "IsDeclaredCurrentRelease", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ModelCurrentRelease))), F.Bool3(F.Eq(F.Nullif(F.Of(this.RulebookReleaseId)), F.Of(this.ModelCurrentRelease)))))); set { }
        }

        // Formula LogEntryCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}))
        [NotMapped]
        public int? LogEntryCount
        {
            get => F.AsInt(F.Memo(this, "LogEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId))))))); set { }
        }

        // Formula LogicalChangeCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}, ModelChangeLogEntries!{{AltersLogicalModel}}, TRUE))
        [NotMapped]
        public int? LogicalChangeCount
        {
            get => F.AsInt(F.Memo(this, "LogicalChangeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.AltersLogicalModel), F.B(true))))))); set { }
        }

        // Formula NonAdditiveChangeCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}, ModelChangeLogEntries!{{IsBackwardIncompatible}}, TRUE))
        [NotMapped]
        public int? NonAdditiveChangeCount
        {
            get => F.AsInt(F.Memo(this, "NonAdditiveChangeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.IsBackwardIncompatible), F.B(true))))))); set { }
        }

        // Formula ClassRemovalOrRenameCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}, ModelChangeLogEntries!{{IsClassRemovalOrRename}}, TRUE))
        [NotMapped]
        public int? ClassRemovalOrRenameCount
        {
            get => F.AsInt(F.Memo(this, "ClassRemovalOrRenameCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.IsClassRemovalOrRename), F.B(true))))))); set { }
        }

        // Formula InvalidatingDomainRangeCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}, ModelChangeLogEntries!{{IsInvalidatingDomainRangeChange}}, TRUE))
        [NotMapped]
        public int? InvalidatingDomainRangeCount
        {
            get => F.AsInt(F.Memo(this, "InvalidatingDomainRangeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.IsInvalidatingDomainRangeChange), F.B(true))))))); set { }
        }

        // Formula InconsistentDisjointnessCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}, ModelChangeLogEntries!{{IsInconsistentDisjointness}}, TRUE))
        [NotMapped]
        public int? InconsistentDisjointnessCount
        {
            get => F.AsInt(F.Memo(this, "InconsistentDisjointnessCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.IsInconsistentDisjointness), F.B(true))))))); set { }
        }

        // Formula SchemaAdditionCount (rulebook: =COUNTIFS(ModelChangeLogEntries!{{Release}}, {{RulebookReleaseId}}, ModelChangeLogEntries!{{IsAdditiveSchemaChange}}, TRUE))
        [NotMapped]
        public int? SchemaAdditionCount
        {
            get => F.AsInt(F.Memo(this, "SchemaAdditionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeLogEntry>(base.SoAContext, "ModelChangeLogEntries", __c => __c.ModelChangeLogEntries), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.IsAdditiveSchemaChange), F.B(true))))))); set { }
        }

        // Formula SuiteUpdateCount (rulebook: =COUNTIFS(StewardActivities!{{Release}}, {{RulebookReleaseId}}, StewardActivities!{{DutyKind}}, "ValidationSuiteUpdate"))
        [NotMapped]
        public int? SuiteUpdateCount
        {
            get => F.AsInt(F.Memo(this, "SuiteUpdateCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StewardActivity>(base.SoAContext, "StewardActivities", __c => __c.StewardActivities), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.DutyKind), F.S("ValidationSuiteUpdate"))))))); set { }
        }

        // Formula ConsumerCount (rulebook: =COUNTIFS(ModelConsumers!{{DependsOnModel}}, {{GovernedModel}}))
        [NotMapped]
        public int? ConsumerCount
        {
            get => F.AsInt(F.Memo(this, "ConsumerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelConsumer>(base.SoAContext, "ModelConsumers", __c => __c.ModelConsumers), __r => F.CritField(F.Of(__r.DependsOnModel), F.Of(this.GovernedModel))))))); set { }
        }

        // Formula NotifiedConsumerCount (rulebook: =COUNTIFS(ConsumerRevalidations!{{RulebookRelease}}, {{RulebookReleaseId}}, ConsumerRevalidations!{{WasNotified}}, TRUE))
        [NotMapped]
        public int? NotifiedConsumerCount
        {
            get => F.AsInt(F.Memo(this, "NotifiedConsumerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConsumerRevalidation>(base.SoAContext, "ConsumerRevalidations", __c => __c.ConsumerRevalidations), __r => F.CritField(F.Of(__r.RulebookRelease), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.WasNotified), F.B(true))))))); set { }
        }

        // Formula RevalidatedConsumerCount (rulebook: =COUNTIFS(ConsumerRevalidations!{{RulebookRelease}}, {{RulebookReleaseId}}, ConsumerRevalidations!{{PassedRevalidation}}, TRUE))
        [NotMapped]
        public int? RevalidatedConsumerCount
        {
            get => F.AsInt(F.Memo(this, "RevalidatedConsumerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConsumerRevalidation>(base.SoAContext, "ConsumerRevalidations", __c => __c.ConsumerRevalidations), __r => F.CritField(F.Of(__r.RulebookRelease), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.PassedRevalidation), F.B(true))))))); set { }
        }

        // Formula ValidationRunCount (rulebook: =COUNTIFS(ChangeValidationRuns!{{Release}}, {{RulebookReleaseId}}))
        [NotMapped]
        public int? ValidationRunCount
        {
            get => F.AsInt(F.Memo(this, "ValidationRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId))))))); set { }
        }

        // Formula ValidationFailureTotal (rulebook: =SUMIFS(ChangeValidationRuns!{{FailureCount}}, ChangeValidationRuns!{{Release}}, {{RulebookReleaseId}}, ChangeValidationRuns!{{RunPurpose}}, "Acceptance"))
        [NotMapped]
        public int? ValidationFailureTotal
        {
            get => F.AsInt(F.Memo(this, "ValidationFailureTotal", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.RunPurpose), F.S("Acceptance")), __r => F.Of(__r.FailureCount), null))))); set { }
        }

        // Formula ConsistentRunCount (rulebook: =COUNTIFS(ChangeValidationRuns!{{Release}}, {{RulebookReleaseId}}, ChangeValidationRuns!{{ConsistencyCheckOutcome}}, "Consistent"))
        [NotMapped]
        public int? ConsistentRunCount
        {
            get => F.AsInt(F.Memo(this, "ConsistentRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.Release), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.ConsistencyCheckOutcome), F.S("Consistent"))))))); set { }
        }

        // Formula CqRunCount (rulebook: =COUNTIFS(CompetencyQuestionRuns!{{RulebookRelease}}, {{RulebookReleaseId}}))
        [NotMapped]
        public int? CqRunCount
        {
            get => F.AsInt(F.Memo(this, "CqRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CompetencyQuestionRun>(base.SoAContext, "CompetencyQuestionRuns", __c => __c.CompetencyQuestionRuns), __r => F.CritField(F.Of(__r.RulebookRelease), F.Of(this.RulebookReleaseId))))))); set { }
        }

        // Formula AnswerableCqRunCount (rulebook: =COUNTIFS(CompetencyQuestionRuns!{{RulebookRelease}}, {{RulebookReleaseId}}, CompetencyQuestionRuns!{{WasAnswerable}}, TRUE))
        [NotMapped]
        public int? AnswerableCqRunCount
        {
            get => F.AsInt(F.Memo(this, "AnswerableCqRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CompetencyQuestionRun>(base.SoAContext, "CompetencyQuestionRuns", __c => __c.CompetencyQuestionRuns), __r => F.CritField(F.Of(__r.RulebookRelease), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.WasAnswerable), F.B(true))))))); set { }
        }

        // Formula RegressedBaselineCount (rulebook: =COUNTIFS(CompetencyQuestionRuns!{{RulebookRelease}}, {{RulebookReleaseId}}, CompetencyQuestionRuns!{{IsBaselineRegression}}, TRUE))
        [NotMapped]
        public int? RegressedBaselineCount
        {
            get => F.AsInt(F.Memo(this, "RegressedBaselineCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<CompetencyQuestionRun>(base.SoAContext, "CompetencyQuestionRuns", __c => __c.CompetencyQuestionRuns), __r => F.CritField(F.Of(__r.RulebookRelease), F.Of(this.RulebookReleaseId)) && F.CritLiteral(F.Of(__r.IsBaselineRegression), F.B(true))))))); set { }
        }

        // Formula CqCoveragePercent (rulebook: =IF({{CqRunCount}} = 0, 0, ROUND(100 * {{AnswerableCqRunCount}} / {{CqRunCount}}, 1)))
        [NotMapped]
        public decimal? CqCoveragePercent
        {
            get => F.AsDecimal(F.Memo(this, "CqCoveragePercent", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.CqRunCount), F.I(0)))) ? F.I(0) : F.Round(F.Div(F.Mul(F.I(100), F.Of(this.AnswerableCqRunCount)), F.Of(this.CqRunCount)), F.I(1))))); set { }
        }

        // Formula PrevCqCoveragePercent (rulebook: =INDEX(RulebookReleases!{{CqCoveragePercent}}, MATCH({{PreviousRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public decimal? PrevCqCoveragePercent
        {
            get => F.AsDecimal(F.Memo(this, "PrevCqCoveragePercent", () => F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.PreviousRelease), __r => F.Of(__r.CqCoveragePercent), () => F.Of(new RulebookRelease().CqCoveragePercent)))); set { }
        }

        // Formula PrevCqRunCount (rulebook: =INDEX(RulebookReleases!{{CqRunCount}}, MATCH({{PreviousRelease}}, RulebookReleases!{{RulebookReleaseId}}, 0)))
        [NotMapped]
        public int? PrevCqRunCount
        {
            get => F.AsInt(F.Memo(this, "PrevCqRunCount", () => F.Integer(F.Lookup<RulebookRelease>(this, "RulebookReleases", "RulebookReleaseId", __c => __c.RulebookReleases, __r => F.Of(__r.RulebookReleaseId), F.Of(this.PreviousRelease), __r => F.Of(__r.CqRunCount), () => F.Of(new RulebookRelease().CqRunCount))))); set { }
        }

        // Formula ScoredCriterionCount (rulebook: =COUNTIFS(QualityAssessments!{{RulebookRelease}}, {{RulebookReleaseId}}))
        [NotMapped]
        public int? ScoredCriterionCount
        {
            get => F.AsInt(F.Memo(this, "ScoredCriterionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<QualityAssessment>(base.SoAContext, "QualityAssessments", __c => __c.QualityAssessments), __r => F.CritField(F.Of(__r.RulebookRelease), F.Of(this.RulebookReleaseId))))))); set { }
        }

        // Formula StatedCriterionCount (rulebook: =COUNTIFS(QualityCriteria!{{IsStated}}, TRUE))
        [NotMapped]
        public int? StatedCriterionCount
        {
            get => F.AsInt(F.Memo(this, "StatedCriterionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<QualityCriteria>(base.SoAContext, "QualityCriteria", __c => __c.QualityCriteria), __r => F.CritLiteral(F.Of(__r.IsStated), F.B(true))))))); set { }
        }

        // Formula PatchAltersLogicalModel (rulebook: =AND({{DeclaredScale}} = "Patch", {{LogicalChangeCount}} > 0))
        [NotMapped]
        public bool? PatchAltersLogicalModel
        {
            get => F.AsBool(F.Memo(this, "PatchAltersLogicalModel", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Patch"))), F.Bool3(F.Cmp(F.Of(this.LogicalChangeCount), ">", F.I(0)))))); set { }
        }

        // Formula MinorIsNotBackwardCompatible (rulebook: =AND({{DeclaredScale}} = "Minor", {{NonAdditiveChangeCount}} > 0))
        [NotMapped]
        public bool? MinorIsNotBackwardCompatible
        {
            get => F.AsBool(F.Memo(this, "MinorIsNotBackwardCompatible", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Minor"))), F.Bool3(F.Cmp(F.Of(this.NonAdditiveChangeCount), ">", F.I(0)))))); set { }
        }

        // Formula ClassRemovalWithoutMajor (rulebook: =AND({{DeclaredScale}} <> "Major", {{ClassRemovalOrRenameCount}} > 0))
        [NotMapped]
        public bool? ClassRemovalWithoutMajor
        {
            get => F.AsBool(F.Memo(this, "ClassRemovalWithoutMajor", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major"))), F.Bool3(F.Cmp(F.Of(this.ClassRemovalOrRenameCount), ">", F.I(0)))))); set { }
        }

        // Formula InvalidatingDomainRangeWithoutMajor (rulebook: =AND({{DeclaredScale}} <> "Major", {{InvalidatingDomainRangeCount}} > 0))
        [NotMapped]
        public bool? InvalidatingDomainRangeWithoutMajor
        {
            get => F.AsBool(F.Memo(this, "InvalidatingDomainRangeWithoutMajor", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major"))), F.Bool3(F.Cmp(F.Of(this.InvalidatingDomainRangeCount), ">", F.I(0)))))); set { }
        }

        // Formula InconsistentDisjointnessWithoutMajor (rulebook: =AND({{DeclaredScale}} <> "Major", {{InconsistentDisjointnessCount}} > 0))
        [NotMapped]
        public bool? InconsistentDisjointnessWithoutMajor
        {
            get => F.AsBool(F.Memo(this, "InconsistentDisjointnessWithoutMajor", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major"))), F.Bool3(F.Cmp(F.Of(this.InconsistentDisjointnessCount), ">", F.I(0)))))); set { }
        }

        // Formula IsBreakingRelease (rulebook: =OR({{DeclaredScale}} = "Major", {{NonAdditiveChangeCount}} > 0))
        [NotMapped]
        public bool? IsBreakingRelease
        {
            get => F.AsBool(F.Memo(this, "IsBreakingRelease", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major"))), F.Bool3(F.Cmp(F.Of(this.NonAdditiveChangeCount), ">", F.I(0)))))); set { }
        }

        // Formula BreakingReleaseWithUnrevalidatedConsumers (rulebook: =AND({{IsBreakingRelease}}, {{RevalidatedConsumerCount}} < {{ConsumerCount}}))
        [NotMapped]
        public bool? BreakingReleaseWithUnrevalidatedConsumers
        {
            get => F.AsBool(F.Memo(this, "BreakingReleaseWithUnrevalidatedConsumers", () => F.And(F.Bool3(F.Of(this.IsBreakingRelease)), F.Bool3(F.Cmp(F.Of(this.RevalidatedConsumerCount), "<", F.Of(this.ConsumerCount)))))); set { }
        }

        // Formula BreakingReleaseWithoutMigrationPlan (rulebook: =AND({{IsBreakingRelease}}, {{MigrationPlan}} = ""))
        [NotMapped]
        public bool? BreakingReleaseWithoutMigrationPlan
        {
            get => F.AsBool(F.Memo(this, "BreakingReleaseWithoutMigrationPlan", () => F.And(F.Bool3(F.Of(this.IsBreakingRelease)), F.Bool3(F.IsBlank(F.Of(this.MigrationPlan)))))); set { }
        }

        // Formula IsUndocumentedVersionDecision (rulebook: =AND({{GovernedModel}} <> "", OR({{VersionDecisionRationale}} = "", {{VersionDecidedByAgent}} = "")))
        [NotMapped]
        public bool? IsUndocumentedVersionDecision
        {
            get => F.AsBool(F.Memo(this, "IsUndocumentedVersionDecision", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GovernedModel))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.VersionDecisionRationale))), F.Bool3(F.IsBlank(F.Of(this.VersionDecidedByAgent)))))))); set { }
        }

        // Formula IsUnannouncedToDependents (rulebook: ={{NotifiedConsumerCount}} < {{ConsumerCount}})
        [NotMapped]
        public bool? IsUnannouncedToDependents
        {
            get => F.AsBool(F.Memo(this, "IsUnannouncedToDependents", () => F.Cmp(F.Of(this.NotifiedConsumerCount), "<", F.Of(this.ConsumerCount)))); set { }
        }

        // Formula IsReleaseWithoutRecordedChanges (rulebook: =AND({{PreviousRelease}} <> "", {{LogEntryCount}} = 0))
        [NotMapped]
        public bool? IsReleaseWithoutRecordedChanges
        {
            get => F.AsBool(F.Memo(this, "IsReleaseWithoutRecordedChanges", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PreviousRelease))), F.Bool3(F.Eq(F.Of(this.LogEntryCount), F.I(0)))))); set { }
        }

        // Formula SuiteLagsRelease (rulebook: =AND({{SchemaAdditionCount}} > 0, {{SuiteUpdateCount}} = 0))
        [NotMapped]
        public bool? SuiteLagsRelease
        {
            get => F.AsBool(F.Memo(this, "SuiteLagsRelease", () => F.And(F.Bool3(F.Cmp(F.Of(this.SchemaAdditionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.SuiteUpdateCount), F.I(0)))))); set { }
        }

        // Formula IsUntaggedRelease (rulebook: =AND({{GovernedModel}} <> "", {{RulebookCommit}} = ""))
        [NotMapped]
        public bool? IsUntaggedRelease
        {
            get => F.AsBool(F.Memo(this, "IsUntaggedRelease", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GovernedModel))), F.Bool3(F.IsBlank(F.Of(this.RulebookCommit)))))); set { }
        }

        // Formula PublishedWithoutApproval (rulebook: =AND({{Status}} = "Released", {{ApprovedByAgent}} = ""))
        [NotMapped]
        public bool? PublishedWithoutApproval
        {
            get => F.AsBool(F.Memo(this, "PublishedWithoutApproval", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Released"))), F.Bool3(F.IsBlank(F.Of(this.ApprovedByAgent)))))); set { }
        }

        // Formula ReleasedDespiteFailedValidation (rulebook: ={{ValidationFailureTotal}} > 0)
        [NotMapped]
        public bool? ReleasedDespiteFailedValidation
        {
            get => F.AsBool(F.Memo(this, "ReleasedDespiteFailedValidation", () => F.Cmp(F.Of(this.ValidationFailureTotal), ">", F.I(0)))); set { }
        }

        // Formula ReleasedWithoutConsistencyCheck (rulebook: =AND({{GovernedModel}} <> "", {{ConsistentRunCount}} = 0))
        [NotMapped]
        public bool? ReleasedWithoutConsistencyCheck
        {
            get => F.AsBool(F.Memo(this, "ReleasedWithoutConsistencyCheck", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GovernedModel))), F.Bool3(F.Eq(F.Of(this.ConsistentRunCount), F.I(0)))))); set { }
        }

        // Formula PassedValidationAtRelease (rulebook: =AND({{ValidationRunCount}} > 0, {{ValidationFailureTotal}} = 0))
        [NotMapped]
        public bool? PassedValidationAtRelease
        {
            get => F.AsBool(F.Memo(this, "PassedValidationAtRelease", () => F.And(F.Bool3(F.Cmp(F.Of(this.ValidationRunCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ValidationFailureTotal), F.I(0)))))); set { }
        }

        // Formula CqCoverageDeclined (rulebook: =AND({{PreviousRelease}} <> "", {{PrevCqRunCount}} > 0, {{CqRunCount}} > 0, {{CqCoveragePercent}} < {{PrevCqCoveragePercent}}))
        [NotMapped]
        public bool? CqCoverageDeclined
        {
            get => F.AsBool(F.Memo(this, "CqCoverageDeclined", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PreviousRelease))), F.Bool3(F.Cmp(F.Of(this.PrevCqRunCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.CqRunCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.CqCoveragePercent), "<", F.Of(this.PrevCqCoveragePercent)))))); set { }
        }

        // Formula HasBaselineRegression (rulebook: ={{RegressedBaselineCount}} > 0)
        [NotMapped]
        public bool? HasBaselineRegression
        {
            get => F.AsBool(F.Memo(this, "HasBaselineRegression", () => F.Cmp(F.Of(this.RegressedBaselineCount), ">", F.I(0)))); set { }
        }

        // Formula ReleasedWithoutCqTaskTest (rulebook: =AND({{GovernedModel}} <> "", {{CqRunCount}} = 0))
        [NotMapped]
        public bool? ReleasedWithoutCqTaskTest
        {
            get => F.AsBool(F.Memo(this, "ReleasedWithoutCqTaskTest", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GovernedModel))), F.Bool3(F.Eq(F.Of(this.CqRunCount), F.I(0)))))); set { }
        }

        // Formula WellFormedButRequirementsUnshown (rulebook: =AND({{ValidationRunCount}} > 0, {{CqRunCount}} = 0))
        [NotMapped]
        public bool? WellFormedButRequirementsUnshown
        {
            get => F.AsBool(F.Memo(this, "WellFormedButRequirementsUnshown", () => F.And(F.Bool3(F.Cmp(F.Of(this.ValidationRunCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CqRunCount), F.I(0)))))); set { }
        }

        // Formula IsNotScoredAgainstCriteria (rulebook: =AND({{GovernedModel}} <> "", {{ScoredCriterionCount}} < {{StatedCriterionCount}}))
        [NotMapped]
        public bool? IsNotScoredAgainstCriteria
        {
            get => F.AsBool(F.Memo(this, "IsNotScoredAgainstCriteria", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GovernedModel))), F.Bool3(F.Cmp(F.Of(this.ScoredCriterionCount), "<", F.Of(this.StatedCriterionCount)))))); set { }
        }

        // Formula IsReleasedWithoutLicenceOrPermanentId (rulebook: =AND({{GovernedModel}} <> "", OR({{License}} = "", {{PermanentIri}} = "")))
        [NotMapped]
        public bool? IsReleasedWithoutLicenceOrPermanentId
        {
            get => F.AsBool(F.Memo(this, "IsReleasedWithoutLicenceOrPermanentId", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.GovernedModel))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.License))), F.Bool3(F.IsBlank(F.Of(this.PermanentIri)))))))); set { }
        }


        public string? GovernedModel { get; set; }
        public string? PreviousRelease { get; set; }
        public string? VersionDecidedByAgent { get; set; }
        public string? ApprovedByAgent { get; set; }

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

        private RulebookRelease _rulebookRelease;

        [ForeignKey("PreviousRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(PreviousRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. PreviousRelease: " + PreviousRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(PreviousRelease);
                    if (_rulebookRelease != null)
                    {
                        base.SoAContext.Attach(_rulebookRelease);
                    }
                }
                return _rulebookRelease;
            }
            set
            {
                if (_rulebookRelease != value)
                {
                    _rulebookRelease = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookRelease != null)
                    {
                        PreviousRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("VersionDecidedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(VersionDecidedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. VersionDecidedByAgent: " + VersionDecidedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(VersionDecidedByAgent);
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
                        VersionDecidedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ApprovedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ApprovedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ApprovedByAgent: " + ApprovedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ApprovedByAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        ApprovedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<RulebookRelease> _rulebookReleases;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access RulebookReleases - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _rulebookReleases = new ObservableCollection<RulebookRelease>();
                    }
                    else
                    {
                        var items = base.SoAContext.RulebookReleases.Where(x => x.PreviousRelease == this.RulebookReleaseId).ToList<RulebookRelease>();
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
                    item.PreviousRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<VocabularyTerm> _vocabularyTerms;

        [InverseProperty("RulebookRelease")]
        public virtual ObservableCollection<VocabularyTerm> VocabularyTerms
        {
            get
            {
                if (_vocabularyTerms == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerms - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>();
                    }
                    else
                    {
                        var items = base.SoAContext.VocabularyTerms.Where(x => x.IntroducedInRelease == this.RulebookReleaseId).ToList<VocabularyTerm>();
                        _vocabularyTerms = new ObservableCollection<VocabularyTerm>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _vocabularyTerms.CollectionChanged += VocabularyTerms_CollectionChanged;
                }
                return _vocabularyTerms;
            }
            private set
            {
                if (_vocabularyTerms != null)
                {
                    _vocabularyTerms.CollectionChanged -= VocabularyTerms_CollectionChanged;
                }
                _vocabularyTerms = value;
                if (_vocabularyTerms != null)
                {
                    _vocabularyTerms.CollectionChanged += VocabularyTerms_CollectionChanged;
                }
            }
        }

        private void VocabularyTerms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<VocabularyTerm>())
                {
                    item.IntroducedInRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<StewardActivity> _stewardActivities;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access StewardActivities - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _stewardActivities = new ObservableCollection<StewardActivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.StewardActivities.Where(x => x.Release == this.RulebookReleaseId).ToList<StewardActivity>();
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
                    item.Release = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<ModelChangeRequest> _modelChangeRequests;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access ModelChangeRequests - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _modelChangeRequests = new ObservableCollection<ModelChangeRequest>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeRequests.Where(x => x.TargetRelease == this.RulebookReleaseId).ToList<ModelChangeRequest>();
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
                    item.TargetRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<ConsumerRevalidation> _consumerRevalidations;

        [InverseProperty("RulebookReleaseRef")]
        public virtual ObservableCollection<ConsumerRevalidation> ConsumerRevalidations
        {
            get
            {
                if (_consumerRevalidations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConsumerRevalidations - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _consumerRevalidations = new ObservableCollection<ConsumerRevalidation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConsumerRevalidations.Where(x => x.RulebookRelease == this.RulebookReleaseId).ToList<ConsumerRevalidation>();
                        _consumerRevalidations = new ObservableCollection<ConsumerRevalidation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _consumerRevalidations.CollectionChanged += ConsumerRevalidations_CollectionChanged;
                }
                return _consumerRevalidations;
            }
            private set
            {
                if (_consumerRevalidations != null)
                {
                    _consumerRevalidations.CollectionChanged -= ConsumerRevalidations_CollectionChanged;
                }
                _consumerRevalidations = value;
                if (_consumerRevalidations != null)
                {
                    _consumerRevalidations.CollectionChanged += ConsumerRevalidations_CollectionChanged;
                }
            }
        }

        private void ConsumerRevalidations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConsumerRevalidation>())
                {
                    item.RulebookRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<ModelDocument> _modelDocuments;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access ModelDocuments - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _modelDocuments = new ObservableCollection<ModelDocument>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelDocuments.Where(x => x.DocumentedRelease == this.RulebookReleaseId).ToList<ModelDocument>();
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
                    item.DocumentedRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<CompetencyQuestionRun> _competencyQuestionRuns;

        [InverseProperty("RulebookReleaseRef")]
        public virtual ObservableCollection<CompetencyQuestionRun> CompetencyQuestionRuns
        {
            get
            {
                if (_competencyQuestionRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CompetencyQuestionRuns - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionRuns.Where(x => x.RulebookRelease == this.RulebookReleaseId).ToList<CompetencyQuestionRun>();
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _competencyQuestionRuns.CollectionChanged += CompetencyQuestionRuns_CollectionChanged;
                }
                return _competencyQuestionRuns;
            }
            private set
            {
                if (_competencyQuestionRuns != null)
                {
                    _competencyQuestionRuns.CollectionChanged -= CompetencyQuestionRuns_CollectionChanged;
                }
                _competencyQuestionRuns = value;
                if (_competencyQuestionRuns != null)
                {
                    _competencyQuestionRuns.CollectionChanged += CompetencyQuestionRuns_CollectionChanged;
                }
            }
        }

        private void CompetencyQuestionRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CompetencyQuestionRun>())
                {
                    item.RulebookRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<QualityAssessment> _qualityAssessments;

        [InverseProperty("RulebookReleaseRef")]
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
                            throw new InvalidOperationException("Cannot access QualityAssessments - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _qualityAssessments = new ObservableCollection<QualityAssessment>();
                    }
                    else
                    {
                        var items = base.SoAContext.QualityAssessments.Where(x => x.RulebookRelease == this.RulebookReleaseId).ToList<QualityAssessment>();
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
                    item.RulebookRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<ModelProposal> _modelProposals;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access ModelProposals - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _modelProposals = new ObservableCollection<ModelProposal>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelProposals.Where(x => x.AdoptedInRelease == this.RulebookReleaseId).ToList<ModelProposal>();
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
                    item.AdoptedInRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<InstanceDataVersion> _instanceDataVersions;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access InstanceDataVersions - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _instanceDataVersions = new ObservableCollection<InstanceDataVersion>();
                    }
                    else
                    {
                        var items = base.SoAContext.InstanceDataVersions.Where(x => x.ConformsToRelease == this.RulebookReleaseId).ToList<InstanceDataVersion>();
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
                    item.ConformsToRelease = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.Release == this.RulebookReleaseId).ToList<ModelChangeLogEntry>();
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
                    item.Release = this.RulebookReleaseId;
                }
            }
        }

        private ObservableCollection<DriftObservation> _driftObservations;

        [InverseProperty("RulebookRelease")]
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
                            throw new InvalidOperationException("Cannot access DriftObservations - no database context is set. RulebookReleaseId: " + this.RulebookReleaseId + ".");
                        }
                        _driftObservations = new ObservableCollection<DriftObservation>();
                    }
                    else
                    {
                        var items = base.SoAContext.DriftObservations.Where(x => x.SinceRelease == this.RulebookReleaseId).ToList<DriftObservation>();
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
                    item.SinceRelease = this.RulebookReleaseId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.RulebookRelease;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.RulebookReleases;
            _ = this.VocabularyTerms;
            _ = this.StewardActivities;
            _ = this.ModelChangeRequests;
            _ = this.ConsumerRevalidations;
            _ = this.ModelDocuments;
            _ = this.CompetencyQuestionRuns;
            _ = this.QualityAssessments;
            _ = this.ModelProposals;
            _ = this.InstanceDataVersions;
            _ = this.ModelChangeLogEntries;
            _ = this.DriftObservations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
