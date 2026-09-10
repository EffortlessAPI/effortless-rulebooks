
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RequirementSatisfactions")]
    public class RequirementSatisfactionBase : SoAEntityBase
    {
        [Key]
        public string RequirementSatisfactionId { get; set; }

        // Formula Name (rulebook: ={{Requirement}} & " / " & {{SatisfactionLevel}})
        public string? Name
        {
            get => this.Requirement + " / " + this.SatisfactionLevel; set { }
        }

        public string? SatisfactionLevel { get; set; }
        public string? Evidence { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        // Formula RequirementIsBlocking (rulebook: =INDEX(Requirements!{{IsBlocking}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        public bool? RequirementIsBlocking
        {
            get => INDEX(Requirements!this.IsBlocking, MATCH(this.Requirement, Requirements!this.RequirementId, 0)); set { }
        }

        // Formula IsFullySatisfied (rulebook: ={{SatisfactionLevel}} = "Satisfied")
        public bool? IsFullySatisfied
        {
            get => this.SatisfactionLevel = "Satisfied"; set { }
        }

        // Formula IsBlockingAndUnmet (rulebook: =AND({{RequirementIsBlocking}}, NOT({{IsFullySatisfied}})))
        public bool? IsBlockingAndUnmet
        {
            get => AND(this.RequirementIsBlocking, NOT(this.IsFullySatisfied)); set { }
        }

        // Formula BlockingUnmetStepKey (rulebook: =IF({{IsBlockingAndUnmet}}, {{StepExecution}}, ""))
        public string? BlockingUnmetStepKey
        {
            get => IF(this.IsBlockingAndUnmet, this.StepExecution, ""); set { }
        }

        // Formula BlockingSatisfactionStepKey (rulebook: =IF({{RequirementIsBlocking}}, {{StepExecution}}, ""))
        public string? BlockingSatisfactionStepKey
        {
            get => IF(this.RequirementIsBlocking, this.StepExecution, ""); set { }
        }

        // Formula NegativeOutcomeRequirementKey (rulebook: =IF(NOT({{IsFullySatisfied}}), {{Requirement}}, ""))
        public string? NegativeOutcomeRequirementKey
        {
            get => IF(NOT(this.IsFullySatisfied), this.Requirement, ""); set { }
        }

        // Formula EvaluatorAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{EvaluatedByAgent}}, Agents!{{AgentId}}, 0)))
        public string? EvaluatorAgentKind
        {
            get => INDEX(Agents!this.AgentKind, MATCH(this.EvaluatedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula NonHumanEvaluatedHumanControl (rulebook: =AND({{RequirementIsBlocking}}, {{EvaluatorAgentKind}} <> "Human"))
        public bool? NonHumanEvaluatedHumanControl
        {
            get => AND(this.RequirementIsBlocking, this.EvaluatorAgentKind <> "Human"); set { }
        }

        // Formula RequirementHasComputedWitness (rulebook: =INDEX(Requirements!{{HasComputedWitness}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        public bool? RequirementHasComputedWitness
        {
            get => INDEX(Requirements!this.HasComputedWitness, MATCH(this.Requirement, Requirements!this.RequirementId, 0)); set { }
        }

        // Formula IsAssertedOnly (rulebook: =AND({{RequirementIsBlocking}}, {{IsFullySatisfied}}, NOT({{RequirementHasComputedWitness}})))
        public bool? IsAssertedOnly
        {
            get => AND(this.RequirementIsBlocking, this.IsFullySatisfied, NOT(this.RequirementHasComputedWitness)); set { }
        }

        // Formula AssertedOnlyExecutionKey (rulebook: =IF({{IsAssertedOnly}}, {{ParentProcedureExecution}}, ""))
        public string? AssertedOnlyExecutionKey
        {
            get => IF(this.IsAssertedOnly, this.ParentProcedureExecution, ""); set { }
        }

        // Formula ParentProcedureExecution (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? ParentProcedureExecution
        {
            get => INDEX(StepExecutions!this.ProcedureExecution, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula StepExecutionWhenScored (rulebook: =IF({{SatisfactionLevel}} <> "", {{StepExecution}}, ""))
        public string? StepExecutionWhenScored
        {
            get => IF(this.SatisfactionLevel <> "", this.StepExecution, ""); set { }
        }

        // Formula IsHumanEvaluated (rulebook: ={{EvaluatorAgentKind}} = "Human")
        public bool? IsHumanEvaluated
        {
            get => this.EvaluatorAgentKind = "Human"; set { }
        }

        // Formula RequirementIsApprovalType (rulebook: =INDEX(Requirements!{{RequirementType}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        public string? RequirementIsApprovalType
        {
            get => INDEX(Requirements!this.RequirementType, MATCH(this.Requirement, Requirements!this.RequirementId, 0)); set { }
        }

        // Formula IsInvalidApproval (rulebook: =AND({{RequirementIsApprovalType}} = "Approval", OR(NOT({{IsFullySatisfied}}), NOT({{IsHumanEvaluated}}))))
        public bool? IsInvalidApproval
        {
            get => AND(this.RequirementIsApprovalType = "Approval", OR(NOT(this.IsFullySatisfied), NOT(this.IsHumanEvaluated))); set { }
        }

        // Formula ProcedureExecutionOfSatisfaction (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? ProcedureExecutionOfSatisfaction
        {
            get => INDEX(StepExecutions!this.ProcedureExecution, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula RunWhenInvalidApproval (rulebook: =IF({{IsInvalidApproval}}, {{ProcedureExecutionOfSatisfaction}}, ""))
        public string? RunWhenInvalidApproval
        {
            get => IF(this.IsInvalidApproval, this.ProcedureExecutionOfSatisfaction, ""); set { }
        }

        // Formula RequirementIsUnfalsified (rulebook: =INDEX(Requirements!{{IsUnfalsifiedControl}}, MATCH({{Requirement}}, Requirements!{{RequirementId}}, 0)))
        public bool? RequirementIsUnfalsified
        {
            get => INDEX(Requirements!this.IsUnfalsifiedControl, MATCH(this.Requirement, Requirements!this.RequirementId, 0)); set { }
        }

        // Formula IsClearanceByUnfalsifiedControl (rulebook: =AND({{IsFullySatisfied}}, {{RequirementIsBlocking}}, {{RequirementIsUnfalsified}}))
        public bool? IsClearanceByUnfalsifiedControl
        {
            get => AND(this.IsFullySatisfied, this.RequirementIsBlocking, this.RequirementIsUnfalsified); set { }
        }

        // Formula UnfalsifiedClearanceStepKey (rulebook: =IF({{IsClearanceByUnfalsifiedControl}}, {{StepExecution}}, ""))
        public string? UnfalsifiedClearanceStepKey
        {
            get => IF(this.IsClearanceByUnfalsifiedControl, this.StepExecution, ""); set { }
        }

        // Formula SpecStepOfExecution (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? SpecStepOfExecution
        {
            get => INDEX(StepExecutions!this.Step, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula BindingKey (rulebook: =INDEX(StepRequirements!{{StepRequirementId}}, MATCH({{RequirementSatisfactionId}}, StepRequirements!{{StepRequirementId}}, 0)))
        public string? BindingKey
        {
            get => INDEX(StepRequirements!this.StepRequirementId, MATCH(this.RequirementSatisfactionId, StepRequirements!this.StepRequirementId, 0)); set { }
        }

        // Formula ScoredStepExecutorAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? ScoredStepExecutorAgent
        {
            get => INDEX(StepExecutions!this.ExecutedByAgent, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula EvaluatorIsStepExecutor (rulebook: ={{EvaluatedByAgent}} = {{ScoredStepExecutorAgent}})
        public bool? EvaluatorIsStepExecutor
        {
            get => this.EvaluatedByAgent = this.ScoredStepExecutorAgent; set { }
        }

        // Formula RunOwnerAgent (rulebook: =INDEX(ProcedureExecutions!{{ExecutedByAgent}}, MATCH({{ParentProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        public string? RunOwnerAgent
        {
            get => INDEX(ProcedureExecutions!this.ExecutedByAgent, MATCH(this.ParentProcedureExecution, ProcedureExecutions!this.ProcedureExecutionId, 0)); set { }
        }

        // Formula EvaluatorOwnsTheRun (rulebook: ={{EvaluatedByAgent}} = {{RunOwnerAgent}})
        public bool? EvaluatorOwnsTheRun
        {
            get => this.EvaluatedByAgent = this.RunOwnerAgent; set { }
        }

        // Formula IsInterestedPartyAssertion (rulebook: =AND({{IsAssertedOnly}}, OR({{EvaluatorIsStepExecutor}}, {{EvaluatorOwnsTheRun}})))
        public bool? IsInterestedPartyAssertion
        {
            get => AND(this.IsAssertedOnly, OR(this.EvaluatorIsStepExecutor, this.EvaluatorOwnsTheRun)); set { }
        }

        // Formula HasWrittenEvidence (rulebook: ={{Evidence}} <> "")
        public bool? HasWrittenEvidence
        {
            get => this.Evidence <> ""; set { }
        }

        // Formula IsBareAssertion (rulebook: =AND({{IsAssertedOnly}}, NOT({{HasWrittenEvidence}})))
        public bool? IsBareAssertion
        {
            get => AND(this.IsAssertedOnly, NOT(this.HasWrittenEvidence)); set { }
        }

        // Formula InterestedAssertionExecutionKey (rulebook: =IF({{IsInterestedPartyAssertion}}, {{ParentProcedureExecution}}, ""))
        public string? InterestedAssertionExecutionKey
        {
            get => IF(this.IsInterestedPartyAssertion, this.ParentProcedureExecution, ""); set { }
        }

        // Formula IsComputedlyWitnessed (rulebook: =AND({{RequirementIsBlocking}}, {{RequirementHasComputedWitness}}))
        public bool? IsComputedlyWitnessed
        {
            get => AND(this.RequirementIsBlocking, this.RequirementHasComputedWitness); set { }
        }

        // Formula ComputedWitnessExecutionKey (rulebook: =IF({{IsComputedlyWitnessed}}, {{ParentProcedureExecution}}, ""))
        public string? ComputedWitnessExecutionKey
        {
            get => IF(this.IsComputedlyWitnessed, this.ParentProcedureExecution, ""); set { }
        }

        // Formula StepExecutorAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? StepExecutorAgent
        {
            get => INDEX(StepExecutions!this.ExecutedByAgent, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula WasScoredAfterAttestation (rulebook: =DATETIME_DIFF({{EvaluatedAt}}, {{AttestationInstantForRun}}, "minutes") > 0)
        public bool? WasScoredAfterAttestation
        {
            get => DATETIME_DIFF(this.EvaluatedAt, this.AttestationInstantForRun, "minutes") > 0; set { }
        }

        // Formula AttestationInstantForRun (rulebook: =INDEX(ProcedureExecutions!{{LatestAttestationInstant}}, MATCH({{ParentProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        public DateTime? AttestationInstantForRun
        {
            get => INDEX(ProcedureExecutions!this.LatestAttestationInstant, MATCH(this.ParentProcedureExecution, ProcedureExecutions!this.ProcedureExecutionId, 0)); set { }
        }

        // Formula PostAttestationScoreExecutionKey (rulebook: =IF({{WasScoredAfterAttestation}}, {{ParentProcedureExecution}}, ""))
        public string? PostAttestationScoreExecutionKey
        {
            get => IF(this.WasScoredAfterAttestation, this.ParentProcedureExecution, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? Requirement { get; set; }
        public string? EvaluatedByAgent { get; set; }

        private StepExecution _stepExecution;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(StepExecution);
                    if (_stepExecution != null)
                    {
                        Context.Attach(_stepExecution);
                    }
                }
                return _stepExecution;
            }
            set
            {
                if (_stepExecution != value)
                {
                    _stepExecution = value;
                    StepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
                }
            }
        }

        private Requirement _requirement;

        [ForeignKey("Requirement")]
        public virtual Requirement Requirement
        {
            get
            {
                if (_requirement == null && !string.IsNullOrEmpty(Requirement))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Requirement - no database context is set. Requirement: " + Requirement + ".");
                        }
                        return null;
                    }
                    _requirement = Context.Requirements.Find(Requirement);
                    if (_requirement != null)
                    {
                        Context.Attach(_requirement);
                    }
                }
                return _requirement;
            }
            set
            {
                if (_requirement != value)
                {
                    _requirement = value;
                    Requirement = _requirement == null ? default : _requirement.RequirementId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. EvaluatedByAgent: " + EvaluatedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(EvaluatedByAgent);
                    if (_agent != null)
                    {
                        Context.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    EvaluatedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
            _ = this.Requirement;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
