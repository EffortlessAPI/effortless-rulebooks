
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("VerificationOutcomes")]
    public class VerificationOutcomeBase : SoAEntityBase
    {
        [Key]
        public string VerificationOutcomeId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{StepVerification}})
        public string? Name
        {
            get => this.StepExecution + " / " + this.StepVerification; set { }
        }

        public string? ObservedSignalValue { get; set; }
        public DateTime? ObservedAt { get; set; }
        public string? EvidenceUri { get; set; }
        // Formula ExpectedSignalValue (rulebook: =INDEX(StepVerifications!{{ExpectedSignalValue}}, MATCH({{StepVerification}}, StepVerifications!{{StepVerificationId}}, 0)))
        public string? ExpectedSignalValue
        {
            get => INDEX(StepVerifications!this.ExpectedSignalValue, MATCH(this.StepVerification, StepVerifications!this.StepVerificationId, 0)); set { }
        }

        // Formula SignalIdentifier (rulebook: =INDEX(StepVerifications!{{SignalIdentifier}}, MATCH({{StepVerification}}, StepVerifications!{{StepVerificationId}}, 0)))
        public string? SignalIdentifier
        {
            get => INDEX(StepVerifications!this.SignalIdentifier, MATCH(this.StepVerification, StepVerifications!this.StepVerificationId, 0)); set { }
        }

        // Formula SignalMatchesExpected (rulebook: ={{ObservedSignalValue}} = {{ExpectedSignalValue}})
        public bool? SignalMatchesExpected
        {
            get => this.ObservedSignalValue = this.ExpectedSignalValue; set { }
        }

        // Formula HasEvidence (rulebook: ={{EvidenceUri}} <> "")
        public bool? HasEvidence
        {
            get => this.EvidenceUri <> ""; set { }
        }

        // Formula IsUnbackedObservation (rulebook: =AND({{SignalMatchesExpected}}, NOT({{HasEvidence}})))
        public bool? IsUnbackedObservation
        {
            get => AND(this.SignalMatchesExpected, NOT(this.HasEvidence)); set { }
        }

        // Formula IsSelfWitnessed (rulebook: ={{ObservedByAgent}} = {{StepExecutorAgent}})
        public bool? IsSelfWitnessed
        {
            get => this.ObservedByAgent = this.StepExecutorAgent; set { }
        }

        // Formula StepExecutorAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? StepExecutorAgent
        {
            get => INDEX(StepExecutions!this.ExecutedByAgent, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula SelfWitnessedStepKey (rulebook: =IF({{IsSelfWitnessed}}, {{StepExecution}}, ""))
        public string? SelfWitnessedStepKey
        {
            get => IF(this.IsSelfWitnessed, this.StepExecution, ""); set { }
        }

        // Formula UnbackedStepKey (rulebook: =IF({{IsUnbackedObservation}}, {{StepExecution}}, ""))
        public string? UnbackedStepKey
        {
            get => IF(this.IsUnbackedObservation, this.StepExecution, ""); set { }
        }

        // Formula IsSelfWitnessedAndUnbacked (rulebook: =AND({{IsSelfWitnessed}}, NOT({{HasEvidence}})))
        public bool? IsSelfWitnessedAndUnbacked
        {
            get => AND(this.IsSelfWitnessed, NOT(this.HasEvidence)); set { }
        }

        // Formula IsUncorroboratedPass (rulebook: =AND({{SignalMatchesExpected}}, {{IsSelfWitnessedAndUnbacked}}))
        public bool? IsUncorroboratedPass
        {
            get => AND(this.SignalMatchesExpected, this.IsSelfWitnessedAndUnbacked); set { }
        }

        // Formula UncorroboratedPassStepKey (rulebook: =IF({{IsUncorroboratedPass}}, {{StepExecution}}, ""))
        public string? UncorroboratedPassStepKey
        {
            get => IF(this.IsUncorroboratedPass, this.StepExecution, ""); set { }
        }

        // Formula ObserverIsNonHuman (rulebook: =INDEX(Agents!{{IsNonHuman}}, MATCH({{ObservedByAgent}}, Agents!{{AgentId}}, 0)))
        public bool? ObserverIsNonHuman
        {
            get => INDEX(Agents!this.IsNonHuman, MATCH(this.ObservedByAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula ObserverIsIndependentOfExecutor (rulebook: =NOT({{IsSelfWitnessed}}))
        public bool? ObserverIsIndependentOfExecutor
        {
            get => NOT(this.IsSelfWitnessed); set { }
        }

        // Formula IsIndependentHumanObservation (rulebook: =AND(NOT({{ObserverIsNonHuman}}), NOT({{IsSelfWitnessed}}), {{HasEvidence}}))
        public bool? IsIndependentHumanObservation
        {
            get => AND(NOT(this.ObserverIsNonHuman), NOT(this.IsSelfWitnessed), this.HasEvidence); set { }
        }

        // Formula IndependentObservationExecutionKey (rulebook: =IF({{IsIndependentHumanObservation}}, {{ParentProcedureExecutionOfOutcome}}, ""))
        public string? IndependentObservationExecutionKey
        {
            get => IF(this.IsIndependentHumanObservation, this.ParentProcedureExecutionOfOutcome, ""); set { }
        }

        // Formula ParentProcedureExecutionOfOutcome (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? ParentProcedureExecutionOfOutcome
        {
            get => INDEX(StepExecutions!this.ProcedureExecution, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? StepVerification { get; set; }
        public string? ObservedByAgent { get; set; }

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

        private StepVerification _stepVerification;

        [ForeignKey("StepVerification")]
        public virtual StepVerification StepVerification
        {
            get
            {
                if (_stepVerification == null && !string.IsNullOrEmpty(StepVerification))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVerification - no database context is set. StepVerification: " + StepVerification + ".");
                        }
                        return null;
                    }
                    _stepVerification = Context.StepVerifications.Find(StepVerification);
                    if (_stepVerification != null)
                    {
                        Context.Attach(_stepVerification);
                    }
                }
                return _stepVerification;
            }
            set
            {
                if (_stepVerification != value)
                {
                    _stepVerification = value;
                    StepVerification = _stepVerification == null ? default : _stepVerification.StepVerificationId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ObservedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ObservedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ObservedByAgent: " + ObservedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(ObservedByAgent);
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
                    ObservedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
            _ = this.StepVerification;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
