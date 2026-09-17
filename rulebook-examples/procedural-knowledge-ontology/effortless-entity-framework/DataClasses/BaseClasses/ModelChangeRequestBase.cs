
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
    [Table("ModelChangeRequests")]
    public class ModelChangeRequestBase : SoAEntityBase
    {
        [Key]
        public string ModelChangeRequestId { get; set; }

        // Formula Name (rulebook: ={{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Title))); set { }
        }

        public string? Title { get; set; }
        public string? StatedNeed { get; set; }
        public DateTimeOffset? RequestedAt { get; set; }
        public string? MotivationKind { get; set; }
        public string? ChangeLayer { get; set; }
        public string? ChangeOperation { get; set; }
        public string? Classification { get; set; }
        public string? Route { get; set; }
        public string? DeclaredScale { get; set; }
        public string? Status { get; set; }
        public string? ImpactAssessment { get; set; }
        public DateTimeOffset? CoverageCheckedAt { get; set; }
        public DateTimeOffset? AuthorityReviewedAt { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        public string? ImplementationPlacement { get; set; }
        public string? ComplianceImpact { get; set; }
        public DateTimeOffset? EffectiveAt { get; set; }
        // Formula StewardAgent (rulebook: =INDEX(GovernedModels!{{CurrentStewardAgent}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? StewardAgent
        {
            get => F.AsString(F.Memo(this, "StewardAgent", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.CurrentStewardAgent), () => F.Of(new GovernedModel().CurrentStewardAgent)))); set { }
        }

        // Formula AuthorityAgent (rulebook: =INDEX(GovernedModels!{{CurrentAuthorityAgent}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? AuthorityAgent
        {
            get => F.AsString(F.Memo(this, "AuthorityAgent", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.CurrentAuthorityAgent), () => F.Of(new GovernedModel().CurrentAuthorityAgent)))); set { }
        }

        // Formula RuleKey (rulebook: ={{GovernedModel}} & "|" & {{ChangeLayer}})
        [NotMapped]
        public string? RuleKey
        {
            get => F.AsString(F.Memo(this, "RuleKey", () => F.Concat(F.Text(F.Of(this.GovernedModel)), F.S("|"), F.Text(F.Of(this.ChangeLayer))))); set { }
        }

        // Formula RequiredRoute (rulebook: =INDEX(ChangeAuthorityRules!{{RequiredRoute}}, MATCH({{RuleKey}}, ChangeAuthorityRules!{{ChangeAuthorityRuleId}}, 0)))
        [NotMapped]
        public string? RequiredRoute
        {
            get => F.AsString(F.Memo(this, "RequiredRoute", () => F.Lookup<ChangeAuthorityRule>(this, "ChangeAuthorityRules", "ChangeAuthorityRuleId", __c => __c.ChangeAuthorityRules, __r => F.Of(__r.ChangeAuthorityRuleId), F.Of(this.RuleKey), __r => F.Of(__r.RequiredRoute), () => F.Of(new ChangeAuthorityRule().RequiredRoute)))); set { }
        }

        // Formula IsAccepted (rulebook: =OR({{Status}} = "Approved", {{Status}} = "Deployed"))
        [NotMapped]
        public bool? IsAccepted
        {
            get => F.AsBool(F.Memo(this, "IsAccepted", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Approved"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Deployed")))))); set { }
        }

        // Formula IsModelingChange (rulebook: ={{Classification}} = "ModelingChange")
        [NotMapped]
        public bool? IsModelingChange
        {
            get => F.AsBool(F.Memo(this, "IsModelingChange", () => F.Eq(F.Nullif(F.Of(this.Classification)), F.S("ModelingChange")))); set { }
        }

        // Formula IsSchemaChange (rulebook: ={{ChangeLayer}} = "Schema")
        [NotMapped]
        public bool? IsSchemaChange
        {
            get => F.AsBool(F.Memo(this, "IsSchemaChange", () => F.Eq(F.Nullif(F.Of(this.ChangeLayer)), F.S("Schema")))); set { }
        }

        // Formula HasAuthorityReview (rulebook: ={{AuthorityReviewedAt}} <> "")
        [NotMapped]
        public bool? HasAuthorityReview
        {
            get => F.AsBool(F.Memo(this, "HasAuthorityReview", () => F.IsNotBlank(F.Of(this.AuthorityReviewedAt)))); set { }
        }

        // Formula RequiresAuthorityReview (rulebook: =OR({{ChangeOperation}} = "RenameClass", {{ChangeOperation}} = "RemoveClass", {{ChangeOperation}} = "ChangeDomainRange", {{ChangeOperation}} = "AddDisjointness", {{ChangeOperation}} = "AddVocabularyTerm", {{ChangeOperation}} = "RetireDefinition", {{DeclaredScale}} = "Major"))
        [NotMapped]
        public bool? RequiresAuthorityReview
        {
            get => F.AsBool(F.Memo(this, "RequiresAuthorityReview", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("RenameClass"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("RemoveClass"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("ChangeDomainRange"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("AddDisjointness"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("AddVocabularyTerm"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("RetireDefinition"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major")))))); set { }
        }

        // Formula IsMinorScope (rulebook: =AND({{RequiresAuthorityReview}} = FALSE, {{DeclaredScale}} <> "Major"))
        [NotMapped]
        public bool? IsMinorScope
        {
            get => F.AsBool(F.Memo(this, "IsMinorScope", () => F.And(F.Bool3(F.Eq(F.Of(this.RequiresAuthorityReview), F.B(false))), F.Bool3(F.Ne(F.Nullif(F.Of(this.DeclaredScale)), F.S("Major")))))); set { }
        }

        // Formula StewardOwnChangeUnreviewed (rulebook: =AND({{StewardAgent}} <> "", {{RequestedByAgent}} = {{StewardAgent}}, {{IsMinorScope}} = FALSE, {{HasAuthorityReview}} = FALSE))
        [NotMapped]
        public bool? StewardOwnChangeUnreviewed
        {
            get => F.AsBool(F.Memo(this, "StewardOwnChangeUnreviewed", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.StewardAgent))), F.Bool3(F.Eq(F.Nullif(F.Of(this.RequestedByAgent)), F.Of(this.StewardAgent))), F.Bool3(F.Eq(F.Of(this.IsMinorScope), F.B(false))), F.Bool3(F.Eq(F.Of(this.HasAuthorityReview), F.B(false)))))); set { }
        }

        // Formula StewardApprovalOutOfBounds (rulebook: =AND({{ApprovedByAgent}} <> "", {{ApprovedByAgent}} = {{StewardAgent}}, {{ApprovedByAgent}} <> {{AuthorityAgent}}, {{IsMinorScope}} = FALSE))
        [NotMapped]
        public bool? StewardApprovalOutOfBounds
        {
            get => F.AsBool(F.Memo(this, "StewardApprovalOutOfBounds", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ApprovedByAgent))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ApprovedByAgent)), F.Of(this.StewardAgent))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ApprovedByAgent)), F.Of(this.AuthorityAgent))), F.Bool3(F.Eq(F.Of(this.IsMinorScope), F.B(false)))))); set { }
        }

        // Formula AuthorityReviewSkipped (rulebook: =AND({{RequiresAuthorityReview}}, {{IsAccepted}}, {{HasAuthorityReview}} = FALSE))
        [NotMapped]
        public bool? AuthorityReviewSkipped
        {
            get => F.AsBool(F.Memo(this, "AuthorityReviewSkipped", () => F.And(F.Bool3(F.Of(this.RequiresAuthorityReview)), F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Eq(F.Of(this.HasAuthorityReview), F.B(false)))))); set { }
        }

        // Formula PlacementNotDecidedByAuthority (rulebook: =AND({{ImplementationPlacement}} <> "", {{PlacementDecidedByAgent}} <> {{AuthorityAgent}}))
        [NotMapped]
        public bool? PlacementNotDecidedByAuthority
        {
            get => F.AsBool(F.Memo(this, "PlacementNotDecidedByAuthority", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ImplementationPlacement))), F.Bool3(F.Ne(F.Nullif(F.Of(this.PlacementDecidedByAgent)), F.Of(this.AuthorityAgent)))))); set { }
        }

        // Formula IsMisrouted (rulebook: =AND({{RequiredRoute}} <> "", {{Route}} <> {{RequiredRoute}}))
        [NotMapped]
        public bool? IsMisrouted
        {
            get => F.AsBool(F.Memo(this, "IsMisrouted", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RequiredRoute))), F.Bool3(F.Ne(F.Nullif(F.Of(this.Route)), F.Of(this.RequiredRoute)))))); set { }
        }

        // Formula LacksMotivatingQuestion (rulebook: ={{MotivatingQuestion}} = "")
        [NotMapped]
        public bool? LacksMotivatingQuestion
        {
            get => F.AsBool(F.Memo(this, "LacksMotivatingQuestion", () => F.IsBlank(F.Of(this.MotivatingQuestion)))); set { }
        }

        // Formula UnmotivatedAndNotReturned (rulebook: =AND({{LacksMotivatingQuestion}}, {{Status}} <> "ReturnedForClarification"))
        [NotMapped]
        public bool? UnmotivatedAndNotReturned
        {
            get => F.AsBool(F.Memo(this, "UnmotivatedAndNotReturned", () => F.And(F.Bool3(F.Of(this.LacksMotivatingQuestion)), F.Bool3(F.Ne(F.Nullif(F.Of(this.Status)), F.S("ReturnedForClarification")))))); set { }
        }

        // Formula MotivatedByFailingQuestion (rulebook: =AND({{MotivatingQuestion}} <> "", {{MotivationKind}} = "ExistingQuestionNowFails"))
        [NotMapped]
        public bool? MotivatedByFailingQuestion
        {
            get => F.AsBool(F.Memo(this, "MotivatedByFailingQuestion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.MotivatingQuestion))), F.Bool3(F.Eq(F.Nullif(F.Of(this.MotivationKind)), F.S("ExistingQuestionNowFails")))))); set { }
        }

        // Formula IsAcceptedWithoutNamedApprover (rulebook: =AND({{IsAccepted}}, {{ApprovedByAgent}} = ""))
        [NotMapped]
        public bool? IsAcceptedWithoutNamedApprover
        {
            get => F.AsBool(F.Memo(this, "IsAcceptedWithoutNamedApprover", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.IsBlank(F.Of(this.ApprovedByAgent)))))); set { }
        }

        // Formula DeployedWithoutTargetRelease (rulebook: =AND({{Status}} = "Deployed", {{IsSchemaChange}}, {{TargetRelease}} = ""))
        [NotMapped]
        public bool? DeployedWithoutTargetRelease
        {
            get => F.AsBool(F.Memo(this, "DeployedWithoutTargetRelease", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Deployed"))), F.Bool3(F.Of(this.IsSchemaChange)), F.Bool3(F.IsBlank(F.Of(this.TargetRelease)))))); set { }
        }

        // Formula IsMisclassifiedAgentSwap (rulebook: =AND({{ChangeOperation}} = "ReassignAgent", {{Classification}} <> "DataOperation"))
        [NotMapped]
        public bool? IsMisclassifiedAgentSwap
        {
            get => F.AsBool(F.Memo(this, "IsMisclassifiedAgentSwap", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("ReassignAgent"))), F.Bool3(F.Ne(F.Nullif(F.Of(this.Classification)), F.S("DataOperation")))))); set { }
        }

        // Formula AiToHumanMoveWithoutComplianceReview (rulebook: =AND({{ChangeOperation}} = "MoveStepToHuman", {{ComplianceImpact}} = ""))
        [NotMapped]
        public bool? AiToHumanMoveWithoutComplianceReview
        {
            get => F.AsBool(F.Memo(this, "AiToHumanMoveWithoutComplianceReview", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("MoveStepToHuman"))), F.Bool3(F.IsBlank(F.Of(this.ComplianceImpact)))))); set { }
        }

        // Formula AiToHumanMoveUnaudited (rulebook: =AND({{ChangeOperation}} = "MoveStepToHuman", OR({{EffectiveAt}} = "", {{StatedNeed}} = "")))
        [NotMapped]
        public bool? AiToHumanMoveUnaudited
        {
            get => F.AsBool(F.Memo(this, "AiToHumanMoveUnaudited", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("MoveStepToHuman"))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.EffectiveAt))), F.Bool3(F.IsBlank(F.Of(this.StatedNeed)))))))); set { }
        }

        // Formula IsAiToHumanMove (rulebook: =AND({{ChangeOperation}} = "MoveStepToHuman", {{EffectiveAt}} <> ""))
        [NotMapped]
        public bool? IsAiToHumanMove
        {
            get => F.AsBool(F.Memo(this, "IsAiToHumanMove", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("MoveStepToHuman"))), F.Bool3(F.IsNotBlank(F.Of(this.EffectiveAt)))))); set { }
        }

        // Formula LifecycleChangeByUnauthorizedAgent (rulebook: =AND(OR({{ChangeOperation}} = "CreateDefinition", {{ChangeOperation}} = "ModifyDefinition", {{ChangeOperation}} = "RetireDefinition"), {{ApprovedByAgent}} <> "", {{ApprovedByAgent}} <> {{StewardAgent}}, {{ApprovedByAgent}} <> {{AuthorityAgent}}))
        [NotMapped]
        public bool? LifecycleChangeByUnauthorizedAgent
        {
            get => F.AsBool(F.Memo(this, "LifecycleChangeByUnauthorizedAgent", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("CreateDefinition"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("ModifyDefinition"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("RetireDefinition"))))), F.Bool3(F.IsNotBlank(F.Of(this.ApprovedByAgent))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ApprovedByAgent)), F.Of(this.StewardAgent))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ApprovedByAgent)), F.Of(this.AuthorityAgent)))))); set { }
        }

        // Formula AssessedInconsistentCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "InconsistentInstance", ChangeImpactFindings!{{FindingStage}}, "Assessment"))
        [NotMapped]
        public int? AssessedInconsistentCount
        {
            get => F.AsInt(F.Memo(this, "AssessedInconsistentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("InconsistentInstance")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("Assessment"))))))); set { }
        }

        // Formula PostDeployInconsistentCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "InconsistentInstance", ChangeImpactFindings!{{FindingStage}}, "PostDeployment"))
        [NotMapped]
        public int? PostDeployInconsistentCount
        {
            get => F.AsInt(F.Memo(this, "PostDeployInconsistentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("InconsistentInstance")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("PostDeployment"))))))); set { }
        }

        // Formula AssessedInferenceCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "AlteredInference", ChangeImpactFindings!{{FindingStage}}, "Assessment"))
        [NotMapped]
        public int? AssessedInferenceCount
        {
            get => F.AsInt(F.Memo(this, "AssessedInferenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("AlteredInference")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("Assessment"))))))); set { }
        }

        // Formula PostDeployInferenceCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "AlteredInference", ChangeImpactFindings!{{FindingStage}}, "PostDeployment"))
        [NotMapped]
        public int? PostDeployInferenceCount
        {
            get => F.AsInt(F.Memo(this, "PostDeployInferenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("AlteredInference")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("PostDeployment"))))))); set { }
        }

        // Formula AssessedQueryResultCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "AlteredQueryResult", ChangeImpactFindings!{{FindingStage}}, "Assessment"))
        [NotMapped]
        public int? AssessedQueryResultCount
        {
            get => F.AsInt(F.Memo(this, "AssessedQueryResultCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("AlteredQueryResult")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("Assessment"))))))); set { }
        }

        // Formula PostDeployQueryResultCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "AlteredQueryResult", ChangeImpactFindings!{{FindingStage}}, "PostDeployment"))
        [NotMapped]
        public int? PostDeployQueryResultCount
        {
            get => F.AsInt(F.Memo(this, "PostDeployQueryResultCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("AlteredQueryResult")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("PostDeployment"))))))); set { }
        }

        // Formula AssessedCoverageGapCount (rulebook: =COUNTIFS(ChangeImpactFindings!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeImpactFindings!{{FindingKind}}, "CoverageGap", ChangeImpactFindings!{{FindingStage}}, "Assessment"))
        [NotMapped]
        public int? AssessedCoverageGapCount
        {
            get => F.AsInt(F.Memo(this, "AssessedCoverageGapCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeImpactFinding>(base.SoAContext, "ChangeImpactFindings", __c => __c.ChangeImpactFindings), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.FindingKind), F.S("CoverageGap")) && F.CritLiteral(F.Of(__r.FindingStage), F.S("Assessment"))))))); set { }
        }

        // Formula WouldMakeInstancesInconsistent (rulebook: ={{AssessedInconsistentCount}} > 0)
        [NotMapped]
        public bool? WouldMakeInstancesInconsistent
        {
            get => F.AsBool(F.Memo(this, "WouldMakeInstancesInconsistent", () => F.Cmp(F.Of(this.AssessedInconsistentCount), ">", F.I(0)))); set { }
        }

        // Formula WouldAlterInferences (rulebook: ={{AssessedInferenceCount}} > 0)
        [NotMapped]
        public bool? WouldAlterInferences
        {
            get => F.AsBool(F.Memo(this, "WouldAlterInferences", () => F.Cmp(F.Of(this.AssessedInferenceCount), ">", F.I(0)))); set { }
        }

        // Formula WouldAlterQueryResults (rulebook: ={{AssessedQueryResultCount}} > 0)
        [NotMapped]
        public bool? WouldAlterQueryResults
        {
            get => F.AsBool(F.Memo(this, "WouldAlterQueryResults", () => F.Cmp(F.Of(this.AssessedQueryResultCount), ">", F.I(0)))); set { }
        }

        // Formula WouldLeaveCoverageIncomplete (rulebook: ={{AssessedCoverageGapCount}} > 0)
        [NotMapped]
        public bool? WouldLeaveCoverageIncomplete
        {
            get => F.AsBool(F.Memo(this, "WouldLeaveCoverageIncomplete", () => F.Cmp(F.Of(this.AssessedCoverageGapCount), ">", F.I(0)))); set { }
        }

        // Formula MissedInconsistentInstances (rulebook: =AND({{PostDeployInconsistentCount}} > 0, {{AssessedInconsistentCount}} = 0))
        [NotMapped]
        public bool? MissedInconsistentInstances
        {
            get => F.AsBool(F.Memo(this, "MissedInconsistentInstances", () => F.And(F.Bool3(F.Cmp(F.Of(this.PostDeployInconsistentCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.AssessedInconsistentCount), F.I(0)))))); set { }
        }

        // Formula MissedAlteredInferences (rulebook: =AND({{PostDeployInferenceCount}} > 0, {{AssessedInferenceCount}} = 0))
        [NotMapped]
        public bool? MissedAlteredInferences
        {
            get => F.AsBool(F.Memo(this, "MissedAlteredInferences", () => F.And(F.Bool3(F.Cmp(F.Of(this.PostDeployInferenceCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.AssessedInferenceCount), F.I(0)))))); set { }
        }

        // Formula MissedAlteredQueryResults (rulebook: =AND({{PostDeployQueryResultCount}} > 0, {{AssessedQueryResultCount}} = 0))
        [NotMapped]
        public bool? MissedAlteredQueryResults
        {
            get => F.AsBool(F.Memo(this, "MissedAlteredQueryResults", () => F.And(F.Bool3(F.Cmp(F.Of(this.PostDeployQueryResultCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.AssessedQueryResultCount), F.I(0)))))); set { }
        }

        // Formula LeavesCoverageUnchecked (rulebook: =AND({{IsAccepted}}, {{IsModelingChange}}, {{CoverageCheckedAt}} = ""))
        [NotMapped]
        public bool? LeavesCoverageUnchecked
        {
            get => F.AsBool(F.Memo(this, "LeavesCoverageUnchecked", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsModelingChange)), F.Bool3(F.IsBlank(F.Of(this.CoverageCheckedAt)))))); set { }
        }

        // Formula DomainChangeSpreadWrongInferences (rulebook: =AND({{ChangeOperation}} = "ChangeDomainRange", {{PostDeployInferenceCount}} > 0))
        [NotMapped]
        public bool? DomainChangeSpreadWrongInferences
        {
            get => F.AsBool(F.Memo(this, "DomainChangeSpreadWrongInferences", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("ChangeDomainRange"))), F.Bool3(F.Cmp(F.Of(this.PostDeployInferenceCount), ">", F.I(0)))))); set { }
        }

        // Formula IntuitiveDisjointnessBrokeIndividuals (rulebook: =AND({{ChangeOperation}} = "AddDisjointness", {{LacksMotivatingQuestion}}, ({{AssessedInconsistentCount}} + {{PostDeployInconsistentCount}}) > 0))
        [NotMapped]
        public bool? IntuitiveDisjointnessBrokeIndividuals
        {
            get => F.AsBool(F.Memo(this, "IntuitiveDisjointnessBrokeIndividuals", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ChangeOperation)), F.S("AddDisjointness"))), F.Bool3(F.Of(this.LacksMotivatingQuestion)), F.Bool3(F.Cmp(F.Add(F.Of(this.AssessedInconsistentCount), F.Of(this.PostDeployInconsistentCount)), ">", F.I(0)))))); set { }
        }

        // Formula ValidationRunCount (rulebook: =COUNTIFS(ChangeValidationRuns!{{ModelChangeRequest}}, {{ModelChangeRequestId}}))
        [NotMapped]
        public int? ValidationRunCount
        {
            get => F.AsInt(F.Memo(this, "ValidationRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId))))))); set { }
        }

        // Formula AcceptanceFailureTotal (rulebook: =SUMIFS(ChangeValidationRuns!{{FailureCount}}, ChangeValidationRuns!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeValidationRuns!{{RunPurpose}}, "Acceptance"))
        [NotMapped]
        public int? AcceptanceFailureTotal
        {
            get => F.AsInt(F.Memo(this, "AcceptanceFailureTotal", () => F.Integer((base.SoAContext == null ? F.Null : F.SumIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.RunPurpose), F.S("Acceptance")), __r => F.Of(__r.FailureCount), null))))); set { }
        }

        // Formula StructuralPassCount (rulebook: =COUNTIFS(ChangeValidationRuns!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeValidationRuns!{{StructuralCheckOutcome}}, "Pass"))
        [NotMapped]
        public int? StructuralPassCount
        {
            get => F.AsInt(F.Memo(this, "StructuralPassCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.StructuralCheckOutcome), F.S("Pass"))))))); set { }
        }

        // Formula VocabularyPassCount (rulebook: =COUNTIFS(ChangeValidationRuns!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeValidationRuns!{{VocabularyCheckOutcome}}, "Pass"))
        [NotMapped]
        public int? VocabularyPassCount
        {
            get => F.AsInt(F.Memo(this, "VocabularyPassCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeValidationRun>(base.SoAContext, "ChangeValidationRuns", __c => __c.ChangeValidationRuns), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.VocabularyCheckOutcome), F.S("Pass"))))))); set { }
        }

        // Formula AcceptedWithoutTestRun (rulebook: =AND({{IsAccepted}}, {{IsModelingChange}}, {{ValidationRunCount}} = 0))
        [NotMapped]
        public bool? AcceptedWithoutTestRun
        {
            get => F.AsBool(F.Memo(this, "AcceptedWithoutTestRun", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsModelingChange)), F.Bool3(F.Eq(F.Of(this.ValidationRunCount), F.I(0)))))); set { }
        }

        // Formula AcceptedWithFailingSuite (rulebook: =AND({{IsAccepted}}, {{AcceptanceFailureTotal}} > 0))
        [NotMapped]
        public bool? AcceptedWithFailingSuite
        {
            get => F.AsBool(F.Memo(this, "AcceptedWithFailingSuite", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Cmp(F.Of(this.AcceptanceFailureTotal), ">", F.I(0)))))); set { }
        }

        // Formula AcceptedWithoutStructuralCheck (rulebook: =AND({{IsAccepted}}, {{IsModelingChange}}, {{StructuralPassCount}} = 0))
        [NotMapped]
        public bool? AcceptedWithoutStructuralCheck
        {
            get => F.AsBool(F.Memo(this, "AcceptedWithoutStructuralCheck", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsModelingChange)), F.Bool3(F.Eq(F.Of(this.StructuralPassCount), F.I(0)))))); set { }
        }

        // Formula AcceptedWithoutVocabularyCheck (rulebook: =AND({{IsAccepted}}, {{IsModelingChange}}, {{VocabularyPassCount}} = 0))
        [NotMapped]
        public bool? AcceptedWithoutVocabularyCheck
        {
            get => F.AsBool(F.Memo(this, "AcceptedWithoutVocabularyCheck", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsModelingChange)), F.Bool3(F.Eq(F.Of(this.VocabularyPassCount), F.I(0)))))); set { }
        }

        // Formula IntegrityCheckCount (rulebook: =COUNTIFS(ChangeIntegrityChecks!{{ModelChangeRequest}}, {{ModelChangeRequestId}}))
        [NotMapped]
        public int? IntegrityCheckCount
        {
            get => F.AsInt(F.Memo(this, "IntegrityCheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeIntegrityCheck>(base.SoAContext, "ChangeIntegrityChecks", __c => __c.ChangeIntegrityChecks), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId))))))); set { }
        }

        // Formula HumanIntegrityCheckCount (rulebook: =COUNTIFS(ChangeIntegrityChecks!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeIntegrityChecks!{{CheckerKind}}, "Human"))
        [NotMapped]
        public int? HumanIntegrityCheckCount
        {
            get => F.AsInt(F.Memo(this, "HumanIntegrityCheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeIntegrityCheck>(base.SoAContext, "ChangeIntegrityChecks", __c => __c.ChangeIntegrityChecks), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.CheckerKind), F.S("Human"))))))); set { }
        }

        // Formula DisjointnessCheckCount (rulebook: =COUNTIFS(ChangeIntegrityChecks!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeIntegrityChecks!{{CheckKind}}, "Disjointness"))
        [NotMapped]
        public int? DisjointnessCheckCount
        {
            get => F.AsInt(F.Memo(this, "DisjointnessCheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeIntegrityCheck>(base.SoAContext, "ChangeIntegrityChecks", __c => __c.ChangeIntegrityChecks), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.CheckKind), F.S("Disjointness"))))))); set { }
        }

        // Formula DomainInferenceCheckCount (rulebook: =COUNTIFS(ChangeIntegrityChecks!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeIntegrityChecks!{{CheckKind}}, "DomainInference"))
        [NotMapped]
        public int? DomainInferenceCheckCount
        {
            get => F.AsInt(F.Memo(this, "DomainInferenceCheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeIntegrityCheck>(base.SoAContext, "ChangeIntegrityChecks", __c => __c.ChangeIntegrityChecks), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.CheckKind), F.S("DomainInference"))))))); set { }
        }

        // Formula RangeConsistencyCheckCount (rulebook: =COUNTIFS(ChangeIntegrityChecks!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeIntegrityChecks!{{CheckKind}}, "RangeConsistency"))
        [NotMapped]
        public int? RangeConsistencyCheckCount
        {
            get => F.AsInt(F.Memo(this, "RangeConsistencyCheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeIntegrityCheck>(base.SoAContext, "ChangeIntegrityChecks", __c => __c.ChangeIntegrityChecks), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.CheckKind), F.S("RangeConsistency"))))))); set { }
        }

        // Formula IntegrityDecidedWithoutHuman (rulebook: =AND({{IsAccepted}}, {{IsSchemaChange}}, {{IntegrityCheckCount}} > 0, {{HumanIntegrityCheckCount}} = 0))
        [NotMapped]
        public bool? IntegrityDecidedWithoutHuman
        {
            get => F.AsBool(F.Memo(this, "IntegrityDecidedWithoutHuman", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsSchemaChange)), F.Bool3(F.Cmp(F.Of(this.IntegrityCheckCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.HumanIntegrityCheckCount), F.I(0)))))); set { }
        }

        // Formula SkippedDisjointnessReview (rulebook: =AND({{IsAccepted}}, {{IsSchemaChange}}, {{DisjointnessCheckCount}} = 0))
        [NotMapped]
        public bool? SkippedDisjointnessReview
        {
            get => F.AsBool(F.Memo(this, "SkippedDisjointnessReview", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsSchemaChange)), F.Bool3(F.Eq(F.Of(this.DisjointnessCheckCount), F.I(0)))))); set { }
        }

        // Formula SkippedDomainInferenceReview (rulebook: =AND({{IsAccepted}}, {{IsSchemaChange}}, {{DomainInferenceCheckCount}} = 0))
        [NotMapped]
        public bool? SkippedDomainInferenceReview
        {
            get => F.AsBool(F.Memo(this, "SkippedDomainInferenceReview", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsSchemaChange)), F.Bool3(F.Eq(F.Of(this.DomainInferenceCheckCount), F.I(0)))))); set { }
        }

        // Formula SkippedRangeReview (rulebook: =AND({{IsAccepted}}, {{IsSchemaChange}}, {{RangeConsistencyCheckCount}} = 0))
        [NotMapped]
        public bool? SkippedRangeReview
        {
            get => F.AsBool(F.Memo(this, "SkippedRangeReview", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Of(this.IsSchemaChange)), F.Bool3(F.Eq(F.Of(this.RangeConsistencyCheckCount), F.I(0)))))); set { }
        }

        // Formula UnresolvedObjectionCount (rulebook: =COUNTIFS(ChangeObjections!{{ModelChangeRequest}}, {{ModelChangeRequestId}}, ChangeObjections!{{IsUnresolved}}, TRUE))
        [NotMapped]
        public int? UnresolvedObjectionCount
        {
            get => F.AsInt(F.Memo(this, "UnresolvedObjectionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ChangeObjection>(base.SoAContext, "ChangeObjections", __c => __c.ChangeObjections), __r => F.CritField(F.Of(__r.ModelChangeRequest), F.Of(this.ModelChangeRequestId)) && F.CritLiteral(F.Of(__r.IsUnresolved), F.B(true))))))); set { }
        }

        // Formula ApprovedOverUnresolvedObjection (rulebook: =AND({{IsAccepted}}, {{UnresolvedObjectionCount}} > 0))
        [NotMapped]
        public bool? ApprovedOverUnresolvedObjection
        {
            get => F.AsBool(F.Memo(this, "ApprovedOverUnresolvedObjection", () => F.And(F.Bool3(F.Of(this.IsAccepted)), F.Bool3(F.Cmp(F.Of(this.UnresolvedObjectionCount), ">", F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? RequestedByAgent { get; set; }
        public string? MotivatingQuestion { get; set; }
        public string? ApprovedByAgent { get; set; }
        public string? PlacementDecidedByAgent { get; set; }
        public string? TargetRelease { get; set; }

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

        private Agent _agent;

        [ForeignKey("RequestedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RequestedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RequestedByAgent: " + RequestedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(RequestedByAgent);
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
                        RequestedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private RoleQuestion _roleQuestion;

        [ForeignKey("MotivatingQuestion")]
        public virtual RoleQuestion RoleQuestion
        {
            get
            {
                if (_roleQuestion == null && !string.IsNullOrEmpty(MotivatingQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestion - no database context is set. MotivatingQuestion: " + MotivatingQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestion = base.SoAContext.RoleQuestions.Find(MotivatingQuestion);
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
                        MotivatingQuestion = _roleQuestion.RoleQuestionId;
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

        private Agent _agentRefRef;

        [ForeignKey("PlacementDecidedByAgent")]
        public virtual Agent AgentRefRef
        {
            get
            {
                if (_agentRefRef == null && !string.IsNullOrEmpty(PlacementDecidedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRefRef - no database context is set. PlacementDecidedByAgent: " + PlacementDecidedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRefRef = base.SoAContext.Agents.Find(PlacementDecidedByAgent);
                    if (_agentRefRef != null)
                    {
                        base.SoAContext.Attach(_agentRefRef);
                    }
                }
                return _agentRefRef;
            }
            set
            {
                if (_agentRefRef != value)
                {
                    _agentRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRefRef != null)
                    {
                        PlacementDecidedByAgent = _agentRefRef.AgentId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("TargetRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(TargetRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. TargetRelease: " + TargetRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(TargetRelease);
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
                        TargetRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }

        private ObservableCollection<ChangeImpactFinding> _changeImpactFindings;

        [InverseProperty("ModelChangeRequestRef")]
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
                            throw new InvalidOperationException("Cannot access ChangeImpactFindings - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _changeImpactFindings = new ObservableCollection<ChangeImpactFinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeImpactFindings.Where(x => x.ModelChangeRequest == this.ModelChangeRequestId).ToList<ChangeImpactFinding>();
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
                    item.ModelChangeRequest = this.ModelChangeRequestId;
                }
            }
        }

        private ObservableCollection<ChangeIntegrityCheck> _changeIntegrityChecks;

        [InverseProperty("ModelChangeRequestRef")]
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
                            throw new InvalidOperationException("Cannot access ChangeIntegrityChecks - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _changeIntegrityChecks = new ObservableCollection<ChangeIntegrityCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeIntegrityChecks.Where(x => x.ModelChangeRequest == this.ModelChangeRequestId).ToList<ChangeIntegrityCheck>();
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
                    item.ModelChangeRequest = this.ModelChangeRequestId;
                }
            }
        }

        private ObservableCollection<ChangeObjection> _changeObjections;

        [InverseProperty("ModelChangeRequestRef")]
        public virtual ObservableCollection<ChangeObjection> ChangeObjections
        {
            get
            {
                if (_changeObjections == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeObjections - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _changeObjections = new ObservableCollection<ChangeObjection>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeObjections.Where(x => x.ModelChangeRequest == this.ModelChangeRequestId).ToList<ChangeObjection>();
                        _changeObjections = new ObservableCollection<ChangeObjection>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeObjections.CollectionChanged += ChangeObjections_CollectionChanged;
                }
                return _changeObjections;
            }
            private set
            {
                if (_changeObjections != null)
                {
                    _changeObjections.CollectionChanged -= ChangeObjections_CollectionChanged;
                }
                _changeObjections = value;
                if (_changeObjections != null)
                {
                    _changeObjections.CollectionChanged += ChangeObjections_CollectionChanged;
                }
            }
        }

        private void ChangeObjections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeObjection>())
                {
                    item.ModelChangeRequest = this.ModelChangeRequestId;
                }
            }
        }

        private ObservableCollection<ChangeValidationRun> _changeValidationRuns;

        [InverseProperty("ModelChangeRequestRef")]
        public virtual ObservableCollection<ChangeValidationRun> ChangeValidationRuns
        {
            get
            {
                if (_changeValidationRuns == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ChangeValidationRuns - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _changeValidationRuns = new ObservableCollection<ChangeValidationRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.ChangeValidationRuns.Where(x => x.ModelChangeRequest == this.ModelChangeRequestId).ToList<ChangeValidationRun>();
                        _changeValidationRuns = new ObservableCollection<ChangeValidationRun>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _changeValidationRuns.CollectionChanged += ChangeValidationRuns_CollectionChanged;
                }
                return _changeValidationRuns;
            }
            private set
            {
                if (_changeValidationRuns != null)
                {
                    _changeValidationRuns.CollectionChanged -= ChangeValidationRuns_CollectionChanged;
                }
                _changeValidationRuns = value;
                if (_changeValidationRuns != null)
                {
                    _changeValidationRuns.CollectionChanged += ChangeValidationRuns_CollectionChanged;
                }
            }
        }

        private void ChangeValidationRuns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ChangeValidationRun>())
                {
                    item.ModelChangeRequest = this.ModelChangeRequestId;
                }
            }
        }

        private ObservableCollection<StakeholderQuestion> _stakeholderQuestions;

        [InverseProperty("ModelChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access StakeholderQuestions - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _stakeholderQuestions = new ObservableCollection<StakeholderQuestion>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderQuestions.Where(x => x.ResultingChangeRequest == this.ModelChangeRequestId).ToList<StakeholderQuestion>();
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
                    item.ResultingChangeRequest = this.ModelChangeRequestId;
                }
            }
        }

        private ObservableCollection<CompetencyQuestionRun> _competencyQuestionRuns;

        [InverseProperty("ModelChangeRequest")]
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
                            throw new InvalidOperationException("Cannot access CompetencyQuestionRuns - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _competencyQuestionRuns = new ObservableCollection<CompetencyQuestionRun>();
                    }
                    else
                    {
                        var items = base.SoAContext.CompetencyQuestionRuns.Where(x => x.DefectChangeRequest == this.ModelChangeRequestId).ToList<CompetencyQuestionRun>();
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
                    item.DefectChangeRequest = this.ModelChangeRequestId;
                }
            }
        }

        private ObservableCollection<ModelChangeLogEntry> _modelChangeLogEntries;

        [InverseProperty("ModelChangeRequestRef")]
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
                            throw new InvalidOperationException("Cannot access ModelChangeLogEntries - no database context is set. ModelChangeRequestId: " + this.ModelChangeRequestId + ".");
                        }
                        _modelChangeLogEntries = new ObservableCollection<ModelChangeLogEntry>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelChangeLogEntries.Where(x => x.ModelChangeRequest == this.ModelChangeRequestId).ToList<ModelChangeLogEntry>();
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
                    item.ModelChangeRequest = this.ModelChangeRequestId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.Agent;
            _ = this.RoleQuestion;
            _ = this.AgentRef;
            _ = this.AgentRefRef;
            _ = this.RulebookRelease;
            _ = this.ChangeImpactFindings;
            _ = this.ChangeIntegrityChecks;
            _ = this.ChangeObjections;
            _ = this.ChangeValidationRuns;
            _ = this.StakeholderQuestions;
            _ = this.CompetencyQuestionRuns;
            _ = this.ModelChangeLogEntries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
