
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
    [Table("Attestations")]
    public class AttestationBase : SoAEntityBase
    {
        [Key]
        public string AttestationId { get; set; }

        // Formula Name (rulebook: ={{ProcedureExecution}} & " / " & {{AttestationId}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureExecution)), F.S(" / "), F.Text(F.Of(this.AttestationId))))); set { }
        }

        public DateTimeOffset? SignedAt { get; set; }
        public string? AssuranceGradeAtSigning { get; set; }
        public bool? VersionWasFitAtSigning { get; set; }
        // Formula VersionIsFitNow (rulebook: =INDEX(ProcedureExecutions!{{ExecutedVersionIsFit}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public bool? VersionIsFitNow
        {
            get => F.AsBool(F.Memo(this, "VersionIsFitNow", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.ProcedureExecution), __r => F.Of(__r.ExecutedVersionIsFit), () => F.Of(new ProcedureExecution().ExecutedVersionIsFit)))); set { }
        }

        // Formula FitnessVerdictHasDrifted (rulebook: =NOT({{VersionWasFitAtSigning}} = {{VersionIsFitNow}}))
        [NotMapped]
        public bool? FitnessVerdictHasDrifted
        {
            get => F.AsBool(F.Memo(this, "FitnessVerdictHasDrifted", () => F.Not(F.Bool3(F.Eq(F.Nullif(F.Of(this.VersionWasFitAtSigning)), F.Of(this.VersionIsFitNow)))))); set { }
        }

        // Formula AssuranceGradeNow (rulebook: =INDEX(ProcedureExecutions!{{AssuranceGrade}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public string? AssuranceGradeNow
        {
            get => F.AsString(F.Memo(this, "AssuranceGradeNow", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.ProcedureExecution), __r => F.Of(__r.AssuranceGrade), () => F.Of(new ProcedureExecution().AssuranceGrade)))); set { }
        }

        // Formula AssuranceGradeHasDrifted (rulebook: =NOT({{AssuranceGradeAtSigning}} = {{AssuranceGradeNow}}))
        [NotMapped]
        public bool? AssuranceGradeHasDrifted
        {
            get => F.AsBool(F.Memo(this, "AssuranceGradeHasDrifted", () => F.Not(F.Bool3(F.Eq(F.Nullif(F.Of(this.AssuranceGradeAtSigning)), F.Of(this.AssuranceGradeNow)))))); set { }
        }

        // Formula WouldNotSurviveRestatement (rulebook: =OR({{FitnessVerdictHasDrifted}}, {{AssuranceGradeHasDrifted}}))
        [NotMapped]
        public bool? WouldNotSurviveRestatement
        {
            get => F.AsBool(F.Memo(this, "WouldNotSurviveRestatement", () => F.Or(F.Bool3(F.Of(this.FitnessVerdictHasDrifted)), F.Bool3(F.Of(this.AssuranceGradeHasDrifted))))); set { }
        }


        public string? ProcedureExecution { get; set; }
        public string? SignedByAgent { get; set; }

        private ProcedureExecution _procedureExecutionRef;

        [ForeignKey("ProcedureExecution")]
        public virtual ProcedureExecution ProcedureExecutionRef
        {
            get
            {
                if (_procedureExecutionRef == null && !string.IsNullOrEmpty(ProcedureExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureExecutionRef - no database context is set. ProcedureExecution: " + ProcedureExecution + ".");
                        }
                        return null;
                    }
                    _procedureExecutionRef = base.SoAContext.ProcedureExecutions.Find(ProcedureExecution);
                    if (_procedureExecutionRef != null)
                    {
                        base.SoAContext.Attach(_procedureExecutionRef);
                    }
                }
                return _procedureExecutionRef;
            }
            set
            {
                if (_procedureExecutionRef != value)
                {
                    _procedureExecutionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureExecutionRef != null)
                    {
                        ProcedureExecution = _procedureExecutionRef.ProcedureExecutionId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SignedByAgent: " + SignedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(SignedByAgent);
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
                        SignedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
