
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
    [Table("AssistantAnswers")]
    public class AssistantAnswerBase : SoAEntityBase
    {
        [Key]
        public string AssistantAnswerId { get; set; }

        // Formula Name (rulebook: ={{AnsweringAgent}} & ": " & LEFT({{QuestionText}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AnsweringAgent)), F.S(": "), F.Text(F.Left(F.Of(this.QuestionText), F.I(50)))))); set { }
        }

        public DateTimeOffset? AskedAt { get; set; }
        public DateTimeOffset? AnsweredAt { get; set; }
        public string? AnswerKind { get; set; }
        public string? QuestionTopic { get; set; }
        public string? QuestionText { get; set; }
        public string? AnswerText { get; set; }
        public string? RetrievalMode { get; set; }
        public string? DerivationPerformedBy { get; set; }
        public bool? NeedsInference { get; set; }
        public bool? RaisedSafetyConcern { get; set; }
        public string? DeliveryDisposition { get; set; }
        public bool? WasActedOn { get; set; }
        public bool? WasCorrect { get; set; }
        public string? DocumentedInaccuracy { get; set; }
        public string? TaskOutcome { get; set; }
        // Formula GroundingCount (rulebook: =COUNTIFS(AnswerGroundings!{{AssistantAnswer}}, {{AssistantAnswerId}}))
        [NotMapped]
        public int? GroundingCount
        {
            get => F.AsInt(F.Memo(this, "GroundingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AnswerGrounding>(base.SoAContext, "AnswerGroundings", __c => __c.AnswerGroundings), __r => F.CritField(F.Of(__r.AssistantAnswer), F.Of(this.AssistantAnswerId))))))); set { }
        }

        // Formula OwnKnowledgeGroundingCount (rulebook: =COUNTIFS(AnswerGroundings!{{AssistantAnswer}}, {{AssistantAnswerId}}, AnswerGroundings!{{IsFromOwnKnowledge}}, TRUE))
        [NotMapped]
        public int? OwnKnowledgeGroundingCount
        {
            get => F.AsInt(F.Memo(this, "OwnKnowledgeGroundingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AnswerGrounding>(base.SoAContext, "AnswerGroundings", __c => __c.AnswerGroundings), __r => F.CritField(F.Of(__r.AssistantAnswer), F.Of(this.AssistantAnswerId)) && F.CritLiteral(F.Of(__r.IsFromOwnKnowledge), F.B(true))))))); set { }
        }

        // Formula CitedGroundingCount (rulebook: =COUNTIFS(AnswerGroundings!{{AssistantAnswer}}, {{AssistantAnswerId}}, AnswerGroundings!{{CitedToUser}}, TRUE))
        [NotMapped]
        public int? CitedGroundingCount
        {
            get => F.AsInt(F.Memo(this, "CitedGroundingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AnswerGrounding>(base.SoAContext, "AnswerGroundings", __c => __c.AnswerGroundings), __r => F.CritField(F.Of(__r.AssistantAnswer), F.Of(this.AssistantAnswerId)) && F.CritLiteral(F.Of(__r.CitedToUser), F.B(true))))))); set { }
        }

        // Formula StaleGroundingCount (rulebook: =COUNTIFS(AnswerGroundings!{{AssistantAnswer}}, {{AssistantAnswerId}}, AnswerGroundings!{{GroundsOnStaleAssertion}}, TRUE))
        [NotMapped]
        public int? StaleGroundingCount
        {
            get => F.AsInt(F.Memo(this, "StaleGroundingCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AnswerGrounding>(base.SoAContext, "AnswerGroundings", __c => __c.AnswerGroundings), __r => F.CritField(F.Of(__r.AssistantAnswer), F.Of(this.AssistantAnswerId)) && F.CritLiteral(F.Of(__r.GroundsOnStaleAssertion), F.B(true))))))); set { }
        }

        // Formula IsNotFromOwnKnowledge (rulebook: =AND({{DeliveryDisposition}} = "Delivered", {{OwnKnowledgeGroundingCount}} = 0))
        [NotMapped]
        public bool? IsNotFromOwnKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsNotFromOwnKnowledge", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryDisposition)), F.S("Delivered"))), F.Bool3(F.Eq(F.Of(this.OwnKnowledgeGroundingCount), F.I(0)))))); set { }
        }

        // Formula IsUntraceableToSource (rulebook: =AND({{DeliveryDisposition}} = "Delivered", {{GroundingCount}} > 0, {{CitedGroundingCount}} = 0))
        [NotMapped]
        public bool? IsUntraceableToSource
        {
            get => F.AsBool(F.Memo(this, "IsUntraceableToSource", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryDisposition)), F.S("Delivered"))), F.Bool3(F.Cmp(F.Of(this.GroundingCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.CitedGroundingCount), F.I(0)))))); set { }
        }

        // Formula ContextUnescalatedDangerCueCount (rulebook: =INDEX(StepExecutions!{{UnescalatedDangerCueCount}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public int? ContextUnescalatedDangerCueCount
        {
            get => F.AsInt(F.Memo(this, "ContextUnescalatedDangerCueCount", () => F.Integer(F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.UnescalatedDangerCueCount), () => F.Of(new StepExecution().UnescalatedDangerCueCount))))); set { }
        }

        // Formula StayedSilentOnSafetyProblem (rulebook: =AND({{ContextUnescalatedDangerCueCount}} > 0, {{RaisedSafetyConcern}} = FALSE))
        [NotMapped]
        public bool? StayedSilentOnSafetyProblem
        {
            get => F.AsBool(F.Memo(this, "StayedSilentOnSafetyProblem", () => F.And(F.Bool3(F.Cmp(F.Of(this.ContextUnescalatedDangerCueCount), ">", F.I(0))), F.Bool3(F.Eq(F.Nullif(F.Of(this.RaisedSafetyConcern)), F.B(false)))))); set { }
        }

        // Formula ContextStepEndedAt (rulebook: =INDEX(StepExecutions!{{EndedAt}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? ContextStepEndedAt
        {
            get => F.AsDateTime(F.Memo(this, "ContextStepEndedAt", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.EndedAt), () => F.Of(new StepExecution().EndedAt)))); set { }
        }

        // Formula ArrivedAfterStepEnded (rulebook: =AND({{AnswerKind}} = "Guidance", {{ContextStepEndedAt}} <> "", {{AnsweredAt}} > {{ContextStepEndedAt}}))
        [NotMapped]
        public bool? ArrivedAfterStepEnded
        {
            get => F.AsBool(F.Memo(this, "ArrivedAfterStepEnded", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnswerKind)), F.S("Guidance"))), F.Bool3(F.IsNotBlank(F.Of(this.ContextStepEndedAt))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.AnsweredAt)), ">", F.Of(this.ContextStepEndedAt)))))); set { }
        }

        // Formula ExecutionOfContext (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ExecutionOfContext
        {
            get => F.AsString(F.Memo(this, "ExecutionOfContext", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ProcedureExecution), () => F.Of(new StepExecution().ProcedureExecution)))); set { }
        }

        // Formula ContextStep (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ContextStep
        {
            get => F.AsString(F.Memo(this, "ContextStep", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.Step), () => F.Of(new StepExecution().Step)))); set { }
        }

        // Formula AssumedStepCompletedCount (rulebook: =COUNTIFS(StepExecutions!{{ProcedureExecution}}, {{ExecutionOfContext}}, StepExecutions!{{Step}}, {{AssumedCurrentStep}}, StepExecutions!{{ExecutionStatus}}, "Completed"))
        [NotMapped]
        public int? AssumedStepCompletedCount
        {
            get => F.AsInt(F.Memo(this, "AssumedStepCompletedCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.ProcedureExecution), F.Of(this.ExecutionOfContext)) && F.CritField(F.Of(__r.Step), F.Of(this.AssumedCurrentStep)) && F.CritLiteral(F.Of(__r.ExecutionStatus), F.S("Completed"))))))); set { }
        }

        // Formula LostTrackOfState (rulebook: =AND({{AssumedCurrentStep}} <> "", {{ExecutionOfContext}} <> "", {{AssumedStepCompletedCount}} = 0))
        [NotMapped]
        public bool? LostTrackOfState
        {
            get => F.AsBool(F.Memo(this, "LostTrackOfState", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AssumedCurrentStep))), F.Bool3(F.IsNotBlank(F.Of(this.ExecutionOfContext))), F.Bool3(F.Eq(F.Of(this.AssumedStepCompletedCount), F.I(0)))))); set { }
        }

        // Formula SpecifiedTransitionCount (rulebook: =COUNTIFS(StepTransitions!{{FromStep}}, {{AssumedCurrentStep}}, StepTransitions!{{ToStep}}, {{AssertedNextStep}}))
        [NotMapped]
        public int? SpecifiedTransitionCount
        {
            get => F.AsInt(F.Memo(this, "SpecifiedTransitionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepTransition>(base.SoAContext, "StepTransitions", __c => __c.StepTransitions), __r => F.CritField(F.Of(__r.FromStep), F.Of(this.AssumedCurrentStep)) && F.CritField(F.Of(__r.ToStep), F.Of(this.AssertedNextStep))))))); set { }
        }

        // Formula ContradictsSharedModel (rulebook: =AND({{AssertedNextStep}} <> "", {{SpecifiedTransitionCount}} = 0))
        [NotMapped]
        public bool? ContradictsSharedModel
        {
            get => F.AsBool(F.Memo(this, "ContradictsSharedModel", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AssertedNextStep))), F.Bool3(F.Eq(F.Of(this.SpecifiedTransitionCount), F.I(0)))))); set { }
        }

        // Formula IsExplicitlyGroundedRecommendation (rulebook: =AND({{AnswerKind}} = "Recommendation", {{OwnKnowledgeGroundingCount}} > 0))
        [NotMapped]
        public bool? IsExplicitlyGroundedRecommendation
        {
            get => F.AsBool(F.Memo(this, "IsExplicitlyGroundedRecommendation", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnswerKind)), F.S("Recommendation"))), F.Bool3(F.Cmp(F.Of(this.OwnKnowledgeGroundingCount), ">", F.I(0)))))); set { }
        }

        // Formula RecommendationRestsOnNothingExplicit (rulebook: =AND({{AnswerKind}} = "Recommendation", {{OwnKnowledgeGroundingCount}} = 0))
        [NotMapped]
        public bool? RecommendationRestsOnNothingExplicit
        {
            get => F.AsBool(F.Memo(this, "RecommendationRestsOnNothingExplicit", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnswerKind)), F.S("Recommendation"))), F.Bool3(F.Eq(F.Of(this.OwnKnowledgeGroundingCount), F.I(0)))))); set { }
        }

        // Formula RecommendedStepRegulatoryCount (rulebook: =INDEX(Steps!{{RegulatoryRequirementCount}}, MATCH({{RecommendedStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public int? RecommendedStepRegulatoryCount
        {
            get => F.AsInt(F.Memo(this, "RecommendedStepRegulatoryCount", () => F.Integer(F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.RecommendedStep), __r => F.Of(__r.RegulatoryRequirementCount), () => F.Of(new Step().RegulatoryRequirementCount))))); set { }
        }

        // Formula RequirementCheckCount (rulebook: =COUNTIFS(AnswerRequirementChecks!{{AssistantAnswer}}, {{AssistantAnswerId}}))
        [NotMapped]
        public int? RequirementCheckCount
        {
            get => F.AsInt(F.Memo(this, "RequirementCheckCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AnswerRequirementCheck>(base.SoAContext, "AnswerRequirementChecks", __c => __c.AnswerRequirementChecks), __r => F.CritField(F.Of(__r.AssistantAnswer), F.Of(this.AssistantAnswerId))))))); set { }
        }

        // Formula ConflictCount (rulebook: =COUNTIFS(AnswerRequirementChecks!{{AssistantAnswer}}, {{AssistantAnswerId}}, AnswerRequirementChecks!{{Verdict}}, "Conflicts"))
        [NotMapped]
        public int? ConflictCount
        {
            get => F.AsInt(F.Memo(this, "ConflictCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AnswerRequirementCheck>(base.SoAContext, "AnswerRequirementChecks", __c => __c.AnswerRequirementChecks), __r => F.CritField(F.Of(__r.AssistantAnswer), F.Of(this.AssistantAnswerId)) && F.CritLiteral(F.Of(__r.Verdict), F.S("Conflicts"))))))); set { }
        }

        // Formula ConflictsWithRegulation (rulebook: ={{ConflictCount}} > 0)
        [NotMapped]
        public bool? ConflictsWithRegulation
        {
            get => F.AsBool(F.Memo(this, "ConflictsWithRegulation", () => F.Cmp(F.Of(this.ConflictCount), ">", F.I(0)))); set { }
        }

        // Formula IsUncheckedRegulatedRecommendation (rulebook: =AND({{AnswerKind}} = "Recommendation", {{RecommendedStepRegulatoryCount}} > 0, {{RequirementCheckCount}} = 0))
        [NotMapped]
        public bool? IsUncheckedRegulatedRecommendation
        {
            get => F.AsBool(F.Memo(this, "IsUncheckedRegulatedRecommendation", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.AnswerKind)), F.S("Recommendation"))), F.Bool3(F.Cmp(F.Of(this.RecommendedStepRegulatoryCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.RequirementCheckCount), F.I(0)))))); set { }
        }

        // Formula DeliveredDespiteConflict (rulebook: =AND({{ConflictCount}} > 0, {{DeliveryDisposition}} = "Delivered"))
        [NotMapped]
        public bool? DeliveredDespiteConflict
        {
            get => F.AsBool(F.Memo(this, "DeliveredDespiteConflict", () => F.And(F.Bool3(F.Cmp(F.Of(this.ConflictCount), ">", F.I(0))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryDisposition)), F.S("Delivered")))))); set { }
        }

        // Formula RecommendedStepNeedsHuman (rulebook: =INDEX(Steps!{{RequiresHumanConfirmation}}, MATCH({{RecommendedStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public bool? RecommendedStepNeedsHuman
        {
            get => F.AsBool(F.Memo(this, "RecommendedStepNeedsHuman", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.RecommendedStep), __r => F.Of(__r.RequiresHumanConfirmation), () => F.Of(new Step().RequiresHumanConfirmation)))); set { }
        }

        // Formula ActedOnWithoutHumanJudgment (rulebook: =AND({{RecommendedStepNeedsHuman}}, {{WasActedOn}}, {{HumanReviewedBy}} = ""))
        [NotMapped]
        public bool? ActedOnWithoutHumanJudgment
        {
            get => F.AsBool(F.Memo(this, "ActedOnWithoutHumanJudgment", () => F.And(F.Bool3(F.Of(this.RecommendedStepNeedsHuman)), F.IsTrueV(F.Of(this.WasActedOn)), F.Bool3(F.IsBlank(F.Of(this.HumanReviewedBy)))))); set { }
        }

        // Formula AnsweredComplianceQuestionFromDocuments (rulebook: =AND(OR({{QuestionTopic}} = "Authority", {{QuestionTopic}} = "Safety"), {{RetrievalMode}} = "DocumentPassage"))
        [NotMapped]
        public bool? AnsweredComplianceQuestionFromDocuments
        {
            get => F.AsBool(F.Memo(this, "AnsweredComplianceQuestionFromDocuments", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.QuestionTopic)), F.S("Authority"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.QuestionTopic)), F.S("Safety"))))), F.Bool3(F.Eq(F.Nullif(F.Of(this.RetrievalMode)), F.S("DocumentPassage")))))); set { }
        }

        // Formula DocumentInterpretationErred (rulebook: =AND({{RetrievalMode}} = "DocumentPassage", {{QuestionTopic}} = "Authority", {{WasCorrect}} = FALSE))
        [NotMapped]
        public bool? DocumentInterpretationErred
        {
            get => F.AsBool(F.Memo(this, "DocumentInterpretationErred", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.RetrievalMode)), F.S("DocumentPassage"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.QuestionTopic)), F.S("Authority"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.WasCorrect)), F.B(false)))))); set { }
        }

        // Formula ModelDidTheReasoning (rulebook: =AND({{NeedsInference}}, {{DerivationPerformedBy}} = "LanguageModel"))
        [NotMapped]
        public bool? ModelDidTheReasoning
        {
            get => F.AsBool(F.Memo(this, "ModelDidTheReasoning", () => F.And(F.IsTrueV(F.Of(this.NeedsInference)), F.Bool3(F.Eq(F.Nullif(F.Of(this.DerivationPerformedBy)), F.S("LanguageModel")))))); set { }
        }

        // Formula WrongBecauseGraphWasStale (rulebook: =AND({{RetrievalMode}} = "StructuredQuery", {{WasCorrect}} = FALSE, {{StaleGroundingCount}} > 0))
        [NotMapped]
        public bool? WrongBecauseGraphWasStale
        {
            get => F.AsBool(F.Memo(this, "WrongBecauseGraphWasStale", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.RetrievalMode)), F.S("StructuredQuery"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.WasCorrect)), F.B(false))), F.Bool3(F.Cmp(F.Of(this.StaleGroundingCount), ">", F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? AnsweringAgent { get; set; }
        public string? AskedByAgent { get; set; }
        public string? ViaIntegration { get; set; }
        public string? StepExecution { get; set; }
        public string? AssumedCurrentStep { get; set; }
        public string? AssertedNextStep { get; set; }
        public string? RecommendedStep { get; set; }
        public string? HumanReviewedBy { get; set; }
        public string? ReviewedForInitiative { get; set; }

        private Agent _agent;

        [ForeignKey("AnsweringAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AnsweringAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AnsweringAgent: " + AnsweringAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AnsweringAgent);
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
                        AnsweringAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("AskedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(AskedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. AskedByAgent: " + AskedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(AskedByAgent);
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
                        AskedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private AgentIntegration _agentIntegration;

        [ForeignKey("ViaIntegration")]
        public virtual AgentIntegration AgentIntegration
        {
            get
            {
                if (_agentIntegration == null && !string.IsNullOrEmpty(ViaIntegration))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentIntegration - no database context is set. ViaIntegration: " + ViaIntegration + ".");
                        }
                        return null;
                    }
                    _agentIntegration = base.SoAContext.AgentIntegrations.Find(ViaIntegration);
                    if (_agentIntegration != null)
                    {
                        base.SoAContext.Attach(_agentIntegration);
                    }
                }
                return _agentIntegration;
            }
            set
            {
                if (_agentIntegration != value)
                {
                    _agentIntegration = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentIntegration != null)
                    {
                        ViaIntegration = _agentIntegration.AgentIntegrationId;
                    }
                }
            }
        }

        private StepExecution _stepExecutionRef;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecutionRef
        {
            get
            {
                if (_stepExecutionRef == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutionRef - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecutionRef = base.SoAContext.StepExecutions.Find(StepExecution);
                    if (_stepExecutionRef != null)
                    {
                        base.SoAContext.Attach(_stepExecutionRef);
                    }
                }
                return _stepExecutionRef;
            }
            set
            {
                if (_stepExecutionRef != value)
                {
                    _stepExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepExecutionRef != null)
                    {
                        StepExecution = _stepExecutionRef.StepExecutionId;
                    }
                }
            }
        }

        private Step _step;

        [ForeignKey("AssumedCurrentStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(AssumedCurrentStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. AssumedCurrentStep: " + AssumedCurrentStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(AssumedCurrentStep);
                    if (_step != null)
                    {
                        base.SoAContext.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_step != null)
                    {
                        AssumedCurrentStep = _step.StepId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("AssertedNextStep")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(AssertedNextStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. AssertedNextStep: " + AssertedNextStep + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(AssertedNextStep);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        AssertedNextStep = _stepRef.StepId;
                    }
                }
            }
        }

        private Step _stepRefRef;

        [ForeignKey("RecommendedStep")]
        public virtual Step StepRefRef
        {
            get
            {
                if (_stepRefRef == null && !string.IsNullOrEmpty(RecommendedStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRefRef - no database context is set. RecommendedStep: " + RecommendedStep + ".");
                        }
                        return null;
                    }
                    _stepRefRef = base.SoAContext.Steps.Find(RecommendedStep);
                    if (_stepRefRef != null)
                    {
                        base.SoAContext.Attach(_stepRefRef);
                    }
                }
                return _stepRefRef;
            }
            set
            {
                if (_stepRefRef != value)
                {
                    _stepRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRefRef != null)
                    {
                        RecommendedStep = _stepRefRef.StepId;
                    }
                }
            }
        }

        private Agent _agentRefRef;

        [ForeignKey("HumanReviewedBy")]
        public virtual Agent AgentRefRef
        {
            get
            {
                if (_agentRefRef == null && !string.IsNullOrEmpty(HumanReviewedBy))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRefRef - no database context is set. HumanReviewedBy: " + HumanReviewedBy + ".");
                        }
                        return null;
                    }
                    _agentRefRef = base.SoAContext.Agents.Find(HumanReviewedBy);
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
                        HumanReviewedBy = _agentRefRef.AgentId;
                    }
                }
            }
        }

        private AiAdoptionInitiatif _aiAdoptionInitiatif;

        [ForeignKey("ReviewedForInitiative")]
        public virtual AiAdoptionInitiatif AiAdoptionInitiatif
        {
            get
            {
                if (_aiAdoptionInitiatif == null && !string.IsNullOrEmpty(ReviewedForInitiative))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AiAdoptionInitiatif - no database context is set. ReviewedForInitiative: " + ReviewedForInitiative + ".");
                        }
                        return null;
                    }
                    _aiAdoptionInitiatif = base.SoAContext.AiAdoptionInitiatives.Find(ReviewedForInitiative);
                    if (_aiAdoptionInitiatif != null)
                    {
                        base.SoAContext.Attach(_aiAdoptionInitiatif);
                    }
                }
                return _aiAdoptionInitiatif;
            }
            set
            {
                if (_aiAdoptionInitiatif != value)
                {
                    _aiAdoptionInitiatif = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aiAdoptionInitiatif != null)
                    {
                        ReviewedForInitiative = _aiAdoptionInitiatif.AiAdoptionInitiativeId;
                    }
                }
            }
        }

        private ObservableCollection<AnswerGrounding> _answerGroundings;

        [InverseProperty("AssistantAnswerRef")]
        public virtual ObservableCollection<AnswerGrounding> AnswerGroundings
        {
            get
            {
                if (_answerGroundings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnswerGroundings - no database context is set. AssistantAnswerId: " + this.AssistantAnswerId + ".");
                        }
                        _answerGroundings = new ObservableCollection<AnswerGrounding>();
                    }
                    else
                    {
                        var items = base.SoAContext.AnswerGroundings.Where(x => x.AssistantAnswer == this.AssistantAnswerId).ToList<AnswerGrounding>();
                        _answerGroundings = new ObservableCollection<AnswerGrounding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answerGroundings.CollectionChanged += AnswerGroundings_CollectionChanged;
                }
                return _answerGroundings;
            }
            private set
            {
                if (_answerGroundings != null)
                {
                    _answerGroundings.CollectionChanged -= AnswerGroundings_CollectionChanged;
                }
                _answerGroundings = value;
                if (_answerGroundings != null)
                {
                    _answerGroundings.CollectionChanged += AnswerGroundings_CollectionChanged;
                }
            }
        }

        private void AnswerGroundings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AnswerGrounding>())
                {
                    item.AssistantAnswer = this.AssistantAnswerId;
                }
            }
        }

        private ObservableCollection<AnswerRequirementCheck> _answerRequirementChecks;

        [InverseProperty("AssistantAnswerRef")]
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
                            throw new InvalidOperationException("Cannot access AnswerRequirementChecks - no database context is set. AssistantAnswerId: " + this.AssistantAnswerId + ".");
                        }
                        _answerRequirementChecks = new ObservableCollection<AnswerRequirementCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.AnswerRequirementChecks.Where(x => x.AssistantAnswer == this.AssistantAnswerId).ToList<AnswerRequirementCheck>();
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
                    item.AssistantAnswer = this.AssistantAnswerId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.AgentIntegration;
            _ = this.StepExecutionRef;
            _ = this.Step;
            _ = this.StepRef;
            _ = this.StepRefRef;
            _ = this.AgentRefRef;
            _ = this.AiAdoptionInitiatif;
            _ = this.AnswerGroundings;
            _ = this.AnswerRequirementChecks;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
