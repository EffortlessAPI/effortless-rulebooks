
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
    [Table("VerificationOutcomes")]
    public class VerificationOutcomeBase : SoAEntityBase
    {
        [Key]
        public string VerificationOutcomeId { get; set; }

        // Formula Name (rulebook: ={{StepExecution}} & " / " & {{StepVerification}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.StepExecution)), F.S(" / "), F.Text(F.Of(this.StepVerification))))); set { }
        }

        public string? ObservedSignalValue { get; set; }
        public DateTimeOffset? ObservedAt { get; set; }
        public string? EvidenceUri { get; set; }
        // Formula ExpectedSignalValue (rulebook: =INDEX(StepVerifications!{{ExpectedSignalValue}}, MATCH({{StepVerification}}, StepVerifications!{{StepVerificationId}}, 0)))
        [NotMapped]
        public string? ExpectedSignalValue
        {
            get => F.AsString(F.Memo(this, "ExpectedSignalValue", () => F.Lookup<StepVerification>(this, "StepVerifications", "StepVerificationId", __c => __c.StepVerifications, __r => F.Of(__r.StepVerificationId), F.Of(this.StepVerification), __r => F.Of(__r.ExpectedSignalValue), () => F.Of(new StepVerification().ExpectedSignalValue)))); set { }
        }

        // Formula SignalIdentifier (rulebook: =INDEX(StepVerifications!{{SignalIdentifier}}, MATCH({{StepVerification}}, StepVerifications!{{StepVerificationId}}, 0)))
        [NotMapped]
        public string? SignalIdentifier
        {
            get => F.AsString(F.Memo(this, "SignalIdentifier", () => F.Lookup<StepVerification>(this, "StepVerifications", "StepVerificationId", __c => __c.StepVerifications, __r => F.Of(__r.StepVerificationId), F.Of(this.StepVerification), __r => F.Of(__r.SignalIdentifier), () => F.Of(new StepVerification().SignalIdentifier)))); set { }
        }

        // Formula SignalMatchesExpected (rulebook: ={{ObservedSignalValue}} = {{ExpectedSignalValue}})
        [NotMapped]
        public bool? SignalMatchesExpected
        {
            get => F.AsBool(F.Memo(this, "SignalMatchesExpected", () => F.Eq(F.Nullif(F.Of(this.ObservedSignalValue)), F.Of(this.ExpectedSignalValue)))); set { }
        }

        // Formula HasEvidence (rulebook: ={{EvidenceUri}} <> "")
        [NotMapped]
        public bool? HasEvidence
        {
            get => F.AsBool(F.Memo(this, "HasEvidence", () => F.IsNotBlank(F.Of(this.EvidenceUri)))); set { }
        }

        // Formula IsUnbackedObservation (rulebook: =AND({{SignalMatchesExpected}}, NOT({{HasEvidence}})))
        [NotMapped]
        public bool? IsUnbackedObservation
        {
            get => F.AsBool(F.Memo(this, "IsUnbackedObservation", () => F.And(F.Bool3(F.Of(this.SignalMatchesExpected)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasEvidence))))))); set { }
        }

        // Formula IsSelfWitnessed (rulebook: ={{ObservedByAgent}} = {{StepExecutorAgent}})
        [NotMapped]
        public bool? IsSelfWitnessed
        {
            get => F.AsBool(F.Memo(this, "IsSelfWitnessed", () => F.Eq(F.Nullif(F.Of(this.ObservedByAgent)), F.Of(this.StepExecutorAgent)))); set { }
        }

        // Formula StepExecutorAgent (rulebook: =INDEX(StepExecutions!{{ExecutedByAgent}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? StepExecutorAgent
        {
            get => F.AsString(F.Memo(this, "StepExecutorAgent", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ExecutedByAgent), () => F.Of(new StepExecution().ExecutedByAgent)))); set { }
        }

        // Formula SelfWitnessedStepKey (rulebook: =IF({{IsSelfWitnessed}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? SelfWitnessedStepKey
        {
            get => F.AsString(F.Memo(this, "SelfWitnessedStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsSelfWitnessed))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula UnbackedStepKey (rulebook: =IF({{IsUnbackedObservation}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? UnbackedStepKey
        {
            get => F.AsString(F.Memo(this, "UnbackedStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnbackedObservation))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula IsSelfWitnessedAndUnbacked (rulebook: =AND({{IsSelfWitnessed}}, NOT({{HasEvidence}})))
        [NotMapped]
        public bool? IsSelfWitnessedAndUnbacked
        {
            get => F.AsBool(F.Memo(this, "IsSelfWitnessedAndUnbacked", () => F.And(F.Bool3(F.Of(this.IsSelfWitnessed)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasEvidence))))))); set { }
        }

        // Formula IsUncorroboratedPass (rulebook: =AND({{SignalMatchesExpected}}, {{IsSelfWitnessedAndUnbacked}}))
        [NotMapped]
        public bool? IsUncorroboratedPass
        {
            get => F.AsBool(F.Memo(this, "IsUncorroboratedPass", () => F.And(F.Bool3(F.Of(this.SignalMatchesExpected)), F.Bool3(F.Of(this.IsSelfWitnessedAndUnbacked))))); set { }
        }

        // Formula UncorroboratedPassStepKey (rulebook: =IF({{IsUncorroboratedPass}}, {{StepExecution}}, ""))
        [NotMapped]
        public string? UncorroboratedPassStepKey
        {
            get => F.AsString(F.Memo(this, "UncorroboratedPassStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUncorroboratedPass))) ? F.Of(this.StepExecution) : F.S("")))); set { }
        }

        // Formula ObserverIsNonHuman (rulebook: =INDEX(Agents!{{IsNonHuman}}, MATCH({{ObservedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? ObserverIsNonHuman
        {
            get => F.AsBool(F.Memo(this, "ObserverIsNonHuman", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ObservedByAgent), __r => F.Of(__r.IsNonHuman), () => F.Of(new Agent().IsNonHuman)))); set { }
        }

        // Formula ObserverIsIndependentOfExecutor (rulebook: =NOT({{IsSelfWitnessed}}))
        [NotMapped]
        public bool? ObserverIsIndependentOfExecutor
        {
            get => F.AsBool(F.Memo(this, "ObserverIsIndependentOfExecutor", () => F.Not(F.Bool3(F.Of(this.IsSelfWitnessed))))); set { }
        }

        // Formula IsIndependentHumanObservation (rulebook: =AND(NOT({{ObserverIsNonHuman}}), NOT({{IsSelfWitnessed}}), {{HasEvidence}}))
        [NotMapped]
        public bool? IsIndependentHumanObservation
        {
            get => F.AsBool(F.Memo(this, "IsIndependentHumanObservation", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.ObserverIsNonHuman)))), F.Bool3(F.Not(F.Bool3(F.Of(this.IsSelfWitnessed)))), F.Bool3(F.Of(this.HasEvidence))))); set { }
        }

        // Formula IndependentObservationExecutionKey (rulebook: =IF({{IsIndependentHumanObservation}}, {{ParentProcedureExecutionOfOutcome}}, ""))
        [NotMapped]
        public string? IndependentObservationExecutionKey
        {
            get => F.AsString(F.Memo(this, "IndependentObservationExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsIndependentHumanObservation))) ? F.Of(this.ParentProcedureExecutionOfOutcome) : F.S("")))); set { }
        }

        // Formula ParentProcedureExecutionOfOutcome (rulebook: =INDEX(StepExecutions!{{ProcedureExecution}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? ParentProcedureExecutionOfOutcome
        {
            get => F.AsString(F.Memo(this, "ParentProcedureExecutionOfOutcome", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.ProcedureExecution), () => F.Of(new StepExecution().ProcedureExecution)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? StepVerification { get; set; }
        public string? ObservedByAgent { get; set; }

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

        private StepVerification _stepVerificationRef;

        [ForeignKey("StepVerification")]
        public virtual StepVerification StepVerificationRef
        {
            get
            {
                if (_stepVerificationRef == null && !string.IsNullOrEmpty(StepVerification))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVerificationRef - no database context is set. StepVerification: " + StepVerification + ".");
                        }
                        return null;
                    }
                    _stepVerificationRef = base.SoAContext.StepVerifications.Find(StepVerification);
                    if (_stepVerificationRef != null)
                    {
                        base.SoAContext.Attach(_stepVerificationRef);
                    }
                }
                return _stepVerificationRef;
            }
            set
            {
                if (_stepVerificationRef != value)
                {
                    _stepVerificationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepVerificationRef != null)
                    {
                        StepVerification = _stepVerificationRef.StepVerificationId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ObservedByAgent: " + ObservedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ObservedByAgent);
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
                        ObservedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.StepVerificationRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
