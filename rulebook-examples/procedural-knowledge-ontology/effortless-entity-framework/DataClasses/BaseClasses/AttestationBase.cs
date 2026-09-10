
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Attestations")]
    public class AttestationBase : SoAEntityBase
    {
        [Key]
        public string AttestationId { get; set; }

        // Formula Name (rulebook: ={{ProcedureExecution}} & " / " & {{AttestationId}})
        public string? Name
        {
            get => this.ProcedureExecution + " / " + this.AttestationId; set { }
        }

        public DateTime? SignedAt { get; set; }
        public string? AssuranceGradeAtSigning { get; set; }
        public bool? VersionWasFitAtSigning { get; set; }
        // Formula VersionIsFitNow (rulebook: =INDEX(ProcedureExecutions!{{ExecutedVersionIsFit}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        public bool? VersionIsFitNow
        {
            get => INDEX(ProcedureExecutions!this.ExecutedVersionIsFit, MATCH(this.ProcedureExecution, ProcedureExecutions!this.ProcedureExecutionId, 0)); set { }
        }

        // Formula FitnessVerdictHasDrifted (rulebook: =NOT({{VersionWasFitAtSigning}} = {{VersionIsFitNow}}))
        public bool? FitnessVerdictHasDrifted
        {
            get => NOT(this.VersionWasFitAtSigning = this.VersionIsFitNow); set { }
        }

        // Formula AssuranceGradeNow (rulebook: =INDEX(ProcedureExecutions!{{AssuranceGrade}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        public string? AssuranceGradeNow
        {
            get => INDEX(ProcedureExecutions!this.AssuranceGrade, MATCH(this.ProcedureExecution, ProcedureExecutions!this.ProcedureExecutionId, 0)); set { }
        }

        // Formula AssuranceGradeHasDrifted (rulebook: =NOT({{AssuranceGradeAtSigning}} = {{AssuranceGradeNow}}))
        public bool? AssuranceGradeHasDrifted
        {
            get => NOT(this.AssuranceGradeAtSigning = this.AssuranceGradeNow); set { }
        }

        // Formula WouldNotSurviveRestatement (rulebook: =OR({{FitnessVerdictHasDrifted}}, {{AssuranceGradeHasDrifted}}))
        public bool? WouldNotSurviveRestatement
        {
            get => OR(this.FitnessVerdictHasDrifted, this.AssuranceGradeHasDrifted); set { }
        }


        public string? ProcedureExecution { get; set; }
        public string? SignedByAgent { get; set; }

        private ProcedureExecution _procedureExecution;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecution
        {
            get
            {
                if (_procedureExecution == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecution - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecution = Context.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecution != null)
                    {
                        Context.Attach(_procedureExecution);
                    }
                }
                return _procedureExecution;
            }
            set
            {
                if (_procedureExecution != value)
                {
                    _procedureExecution = value;
                    ProcedureExecution = _procedureExecution == null ? default : _procedureExecution.ProcedureExecutionId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("SignedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SignedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SignedByAgent: " + SignedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(SignedByAgent);
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
                    SignedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecution;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
