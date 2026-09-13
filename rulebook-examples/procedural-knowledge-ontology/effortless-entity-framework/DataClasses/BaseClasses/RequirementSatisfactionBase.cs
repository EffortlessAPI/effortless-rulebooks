
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
    [Table("RequirementSatisfactions")]
    public class RequirementSatisfactionBase : SoAEntityBase
    {
        [Key]
        public string RequirementSatisfactionId { get; set; }

        // Formula Name (rulebook: ={{Requirement}} & " / " & {{SatisfactionLevel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Requirement)), F.S(" / "), F.TextOr(F.Of(this.SatisfactionLevel))))); set { }
        }

        public string? SatisfactionLevel { get; set; }
        public string? Evidence { get; set; }
        public DateTimeOffset? EvaluatedAt { get; set; }
        // Formula RequirementIsBlocking (rulebook: =INDEX(Requirements!{{IsBlocking}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public bool? RequirementIsBlocking
        {
            get => F.AsBool(F.Memo(this, "RequirementIsBlocking", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.IsBlocking), () => F.Of(new Requirement().IsBlocking)))); set { }
        }

        // Formula IsFullySatisfied (rulebook: ={{SatisfactionLevel}} = "Satisfied")
        [NotMapped]
        public bool? IsFullySatisfied
        {
            get => F.AsBool(F.Memo(this, "IsFullySatisfied", () => F.Eq(F.Nullif(F.Of(this.SatisfactionLevel)), F.S("Satisfied")))); set { }
        }

        // Formula IsBlockingAndUnmet (rulebook: =AND({{RequirementIsBlocking}}, NOT({{IsFullySatisfied}})))
        [NotMapped]
        public bool? IsBlockingAndUnmet
        {
            get => F.AsBool(F.Memo(this, "IsBlockingAndUnmet", () => F.And(F.Bool3(F.Of(this.RequirementIsBlocking)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsFullySatisfied))))))); set { }
        }

        // Formula BlockingUnmetStepKey (rulebook: =IF({{IsBlockingAndUnmet}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? BlockingUnmetStepKey
        {
            get => F.AsString(F.Memo(this, "BlockingUnmetStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsBlockingAndUnmet))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula BlockingSatisfactionStepKey (rulebook: =IF({{RequirementIsBlocking}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? BlockingSatisfactionStepKey
        {
            get => F.AsString(F.Memo(this, "BlockingSatisfactionStepKey", () => (F.Truthy(F.Bool3(F.Of(this.RequirementIsBlocking))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula NegativeOutcomeRequirementKey (rulebook: =IF(NOT({{IsFullySatisfied}}), {{Requirement}}, ""))
        [NotMapped]
        public string? NegativeOutcomeRequirementKey
        {
            get => F.AsString(F.Memo(this, "NegativeOutcomeRequirementKey", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.IsFullySatisfied))))) ? F.Of(this.Requirement) : F.S("")))); set { }
        }

        // Formula EvaluatorAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{EvaluatedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? EvaluatorAgentKind
        {
            get => F.AsString(F.Memo(this, "EvaluatorAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.EvaluatedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula NonHumanEvaluatedHumanControl (rulebook: =AND({{RequirementIsBlocking}}, {{EvaluatorAgentKind}} <> "Human"))
        [NotMapped]
        public bool? NonHumanEvaluatedHumanControl
        {
            get => F.AsBool(F.Memo(this, "NonHumanEvaluatedHumanControl", () => F.And(F.Bool3(F.Of(this.RequirementIsBlocking)), F.Bool3(F.Ne(F.Of(this.EvaluatorAgentKind), F.S("Human")))))); set { }
        }

        // Formula RequirementHasComputedWitness (rulebook: =INDEX(Requirements!{{HasComputedWitness}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public bool? RequirementHasComputedWitness
        {
            get => F.AsBool(F.Memo(this, "RequirementHasComputedWitness", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.HasComputedWitness), () => F.Of(new Requirement().HasComputedWitness)))); set { }
        }

        // Formula IsAssertedOnly (rulebook: =AND({{RequirementIsBlocking}}, {{IsFullySatisfied}}, NOT({{RequirementHasComputedWitness}})))
        [NotMapped]
        public bool? IsAssertedOnly
        {
            get => F.AsBool(F.Memo(this, "IsAssertedOnly", () => F.And(F.Bool3(F.Of(this.RequirementIsBlocking)), F.Bool3(F.Of(this.IsFullySatisfied)), F.Bool3(F.Not(F.Bool3(F.Of(this.RequirementHasComputedWitness))))))); set { }
        }

        // Formula AssertedOnlyExecutionKey (rulebook: =IF({{IsAssertedOnly}}, {{ParentProcedureExecution}}, ""))
        [NotMapped]
        public string? AssertedOnlyExecutionKey
        {
            get => F.AsString(F.Memo(this, "AssertedOnlyExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsAssertedOnly))) ? F.Of(this.ParentProcedureExecution) : F.S("")))); set { }
        }

        // Formula ParentProcedureExecution (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ParentProcedureExecution
        {
            get => F.AsString(F.Memo(this, "ParentProcedureExecution", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ProcedureExecution), () => F.Of(new StepExecution().ProcedureExecution)))); set { }
        }

        // Formula StepExecutionWhenScored (rulebook: =IF({{SatisfactionLevel}} <> "", {{StepExecution}}, ""))
        [NotMapped]
        public string? StepExecutionWhenScored
        {
            get => F.AsString(F.Memo(this, "StepExecutionWhenScored", () => (F.Truthy(F.Bool3(F.IsNotBlank(F.Of(this.SatisfactionLevel)))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula IsHumanEvaluated (rulebook: ={{EvaluatorAgentKind}} = "Human")
        [NotMapped]
        public bool? IsHumanEvaluated
        {
            get => F.AsBool(F.Memo(this, "IsHumanEvaluated", () => F.Eq(F.Of(this.EvaluatorAgentKind), F.S("Human")))); set { }
        }

        // Formula RequirementIsApprovalType (rulebook: =INDEX(Requirements!{{RequirementType}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public string? RequirementIsApprovalType
        {
            get => F.AsString(F.Memo(this, "RequirementIsApprovalType", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.RequirementType), () => F.Of(new Requirement().RequirementType)))); set { }
        }

        // Formula IsInvalidApproval (rulebook: =AND({{RequirementIsApprovalType}} = "Approval", OR(NOT({{IsFullySatisfied}}), NOT({{IsHumanEvaluated}}))))
        [NotMapped]
        public bool? IsInvalidApproval
        {
            get => F.AsBool(F.Memo(this, "IsInvalidApproval", () => F.And(F.Bool3(F.Eq(F.Of(this.RequirementIsApprovalType), F.S("Approval"))), F.Bool3(F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.IsFullySatisfied)))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsHumanEvaluated))))))))); set { }
        }

        // Formula ProcedureExecutionOfSatisfaction (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ProcedureExecutionOfSatisfaction
        {
            get => F.AsString(F.Memo(this, "ProcedureExecutionOfSatisfaction", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ProcedureExecution), () => F.Of(new StepExecution().ProcedureExecution)))); set { }
        }

        // Formula RunWhenInvalidApproval (rulebook: =IF({{IsInvalidApproval}}, {{ProcedureExecutionOfSatisfaction}}, ""))
        [NotMapped]
        public string? RunWhenInvalidApproval
        {
            get => F.AsString(F.Memo(this, "RunWhenInvalidApproval", () => (F.Truthy(F.Bool3(F.Of(this.IsInvalidApproval))) ? F.Of(this.ProcedureExecutionOfSatisfaction) : F.S("")))); set { }
        }

        // Formula RequirementIsUnfalsified (rulebook: =INDEX(Requirements!{{IsUnfalsifiedControl}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        [NotMapped]
        public bool? RequirementIsUnfalsified
        {
            get => F.AsBool(F.Memo(this, "RequirementIsUnfalsified", () => F.Lookup<Requirement>(this, "Requirements", "RequirementId", __c => __c.Requirements, __r => F.Of(__r.RequirementId), F.Of(this.Requirement), __r => F.Of(__r.IsUnfalsifiedControl), () => F.Of(new Requirement().IsUnfalsifiedControl)))); set { }
        }

        // Formula IsClearanceByUnfalsifiedControl (rulebook: =AND({{IsFullySatisfied}}, {{RequirementIsBlocking}}, {{RequirementIsUnfalsified}}))
        [NotMapped]
        public bool? IsClearanceByUnfalsifiedControl
        {
            get => F.AsBool(F.Memo(this, "IsClearanceByUnfalsifiedControl", () => F.And(F.Bool3(F.Of(this.IsFullySatisfied)), F.Bool3(F.Of(this.RequirementIsBlocking)), F.Bool3(F.Of(this.RequirementIsUnfalsified))))); set { }
        }

        // Formula UnfalsifiedClearanceStepKey (rulebook: =IF({{IsClearanceByUnfalsifiedControl}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? UnfalsifiedClearanceStepKey
        {
            get => F.AsString(F.Memo(this, "UnfalsifiedClearanceStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsClearanceByUnfalsifiedControl))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula SpecStepOfExecution (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? SpecStepOfExecution
        {
            get => F.AsString(F.Memo(this, "SpecStepOfExecution", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.Step), () => F.Of(new StepExecution().Step)))); set { }
        }

        // Formula BindingKey (rulebook: =INDEX(StepRequirements!{{StepRequirementId}}, MATCH({{RequirementSatisfactionId}}, StepRequirements!{{StepRequirementId}}, 0)))
        [NotMapped]
        public string? BindingKey
        {
            get => F.AsString(F.Memo(this, "BindingKey", () => F.Lookup<StepRequirement>(this, "StepRequirements", "StepRequirementId", __c => __c.StepRequirements, __r => F.Of(__r.StepRequirementId), F.Of(this.RequirementSatisfactionId), __r => F.Of(__r.StepRequirementId), () => F.Of(new StepRequirement().StepRequirementId)))); set { }
        }

        // Formula ScoredStepExecutorAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ScoredStepExecutorAgent
        {
            get => F.AsString(F.Memo(this, "ScoredStepExecutorAgent", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ExecutedByAgent), () => F.Of(new StepExecution().ExecutedByAgent)))); set { }
        }

        // Formula EvaluatorIsStepExecutor (rulebook: ={{EvaluatedByAgent}} = {{ScoredStepExecutorAgent}})
        [NotMapped]
        public bool? EvaluatorIsStepExecutor
        {
            get => F.AsBool(F.Memo(this, "EvaluatorIsStepExecutor", () => F.Eq(F.Nullif(F.Of(this.EvaluatedByAgent)), F.Of(this.ScoredStepExecutorAgent)))); set { }
        }

        // Formula RunOwnerAgent (rulebook: =INDEX(ProcedureExecutions!{{ExecutedByAgent}}, MATCH({{ParentProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public string? RunOwnerAgent
        {
            get => F.AsString(F.Memo(this, "RunOwnerAgent", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.ParentProcedureExecution), __r => F.Of(__r.ExecutedByAgent), () => F.Of(new ProcedureExecution().ExecutedByAgent)))); set { }
        }

        // Formula EvaluatorOwnsTheRun (rulebook: ={{EvaluatedByAgent}} = {{RunOwnerAgent}})
        [NotMapped]
        public bool? EvaluatorOwnsTheRun
        {
            get => F.AsBool(F.Memo(this, "EvaluatorOwnsTheRun", () => F.Eq(F.Nullif(F.Of(this.EvaluatedByAgent)), F.Of(this.RunOwnerAgent)))); set { }
        }

        // Formula IsInterestedPartyAssertion (rulebook: =AND({{IsAssertedOnly}}, OR({{EvaluatorIsStepExecutor}}, {{EvaluatorOwnsTheRun}})))
        [NotMapped]
        public bool? IsInterestedPartyAssertion
        {
            get => F.AsBool(F.Memo(this, "IsInterestedPartyAssertion", () => F.And(F.Bool3(F.Of(this.IsAssertedOnly)), F.Bool3(F.Or(F.Bool3(F.Of(this.EvaluatorIsStepExecutor)), F.Bool3(F.Of(this.EvaluatorOwnsTheRun))))))); set { }
        }

        // Formula HasWrittenEvidence (rulebook: ={{Evidence}} <> "")
        [NotMapped]
        public bool? HasWrittenEvidence
        {
            get => F.AsBool(F.Memo(this, "HasWrittenEvidence", () => F.IsNotBlank(F.Of(this.Evidence)))); set { }
        }

        // Formula IsBareAssertion (rulebook: =AND({{IsAssertedOnly}}, NOT({{HasWrittenEvidence}})))
        [NotMapped]
        public bool? IsBareAssertion
        {
            get => F.AsBool(F.Memo(this, "IsBareAssertion", () => F.And(F.Bool3(F.Of(this.IsAssertedOnly)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasWrittenEvidence))))))); set { }
        }

        // Formula InterestedAssertionExecutionKey (rulebook: =IF({{IsInterestedPartyAssertion}}, {{ParentProcedureExecution}}, ""))
        [NotMapped]
        public string? InterestedAssertionExecutionKey
        {
            get => F.AsString(F.Memo(this, "InterestedAssertionExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsInterestedPartyAssertion))) ? F.Of(this.ParentProcedureExecution) : F.S("")))); set { }
        }

        // Formula IsComputedlyWitnessed (rulebook: =AND({{RequirementIsBlocking}}, {{RequirementHasComputedWitness}}))
        [NotMapped]
        public bool? IsComputedlyWitnessed
        {
            get => F.AsBool(F.Memo(this, "IsComputedlyWitnessed", () => F.And(F.Bool3(F.Of(this.RequirementIsBlocking)), F.Bool3(F.Of(this.RequirementHasComputedWitness))))); set { }
        }

        // Formula ComputedWitnessExecutionKey (rulebook: =IF({{IsComputedlyWitnessed}}, {{ParentProcedureExecution}}, ""))
        [NotMapped]
        public string? ComputedWitnessExecutionKey
        {
            get => F.AsString(F.Memo(this, "ComputedWitnessExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsComputedlyWitnessed))) ? F.Of(this.ParentProcedureExecution) : F.S("")))); set { }
        }

        // Formula StepExecutorAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? StepExecutorAgent
        {
            get => F.AsString(F.Memo(this, "StepExecutorAgent", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ExecutedByAgent), () => F.Of(new StepExecution().ExecutedByAgent)))); set { }
        }

        // Formula WasScoredAfterAttestation (rulebook: =DATETIME_DIFF({{EvaluatedAt}}, {{AttestationInstantForRun}}, "minutes") > 0)
        [NotMapped]
        public bool? WasScoredAfterAttestation
        {
            get => F.AsBool(F.Memo(this, "WasScoredAfterAttestation", () => F.Cmp(F.DatetimeDiff(F.Of(this.EvaluatedAt), F.Of(this.AttestationInstantForRun), F.S("minutes")), ">", F.I(0)))); set { }
        }

        // Formula AttestationInstantForRun (rulebook: =INDEX(ProcedureExecutions!{{LatestAttestationInstant}}, MATCH({{ParentProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AttestationInstantForRun
        {
            get => F.AsDateTime(F.Memo(this, "AttestationInstantForRun", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.ParentProcedureExecution), __r => F.Of(__r.LatestAttestationInstant), () => F.Of(new ProcedureExecution().LatestAttestationInstant)))); set { }
        }

        // Formula PostAttestationScoreExecutionKey (rulebook: =IF({{WasScoredAfterAttestation}}, {{ParentProcedureExecution}}, ""))
        [NotMapped]
        public string? PostAttestationScoreExecutionKey
        {
            get => F.AsString(F.Memo(this, "PostAttestationScoreExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.WasScoredAfterAttestation))) ? F.Of(this.ParentProcedureExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? Requirement { get; set; }
        public string? EvaluatedByAgent { get; set; }

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

        private Requirement _requirementRef;

        [ForeignKey("Requirement")]
        public virtual Requirement RequirementRef
        {
            get
            {
                if (_requirementRef == null && !string.IsNullOrEmpty(Requirement))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementRef - no database context is set. Requirement: " + Requirement + ".");
                        }
                        return null;
                    }
                    _requirementRef = base.SoAContext.Requirements.Find(Requirement);
                    if (_requirementRef != null)
                    {
                        base.SoAContext.Attach(_requirementRef);
                    }
                }
                return _requirementRef;
            }
            set
            {
                if (_requirementRef != value)
                {
                    _requirementRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_requirementRef != null)
                    {
                        Requirement = _requirementRef.RequirementId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("EvaluatedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(EvaluatedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. EvaluatedByAgent: " + EvaluatedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(EvaluatedByAgent);
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
                        EvaluatedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.RequirementRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
