
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("KnowledgeGaps")]
    public class KnowledgeGapBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeGapId { get; set; }

        // Formula Name (rulebook: ={{Severity}} & ": " & LEFT({{Statement}}, 60))
        public string? Name
        {
            get => this.Severity + ": " + LEFT(this.Statement, 60); set { }
        }

        public string? Statement { get; set; }
        public string? Severity { get; set; }
        public string? BlockingKind { get; set; }
        public string? Status { get; set; }
        public DateTime? IdentifiedAt { get; set; }
        public string? ResolutionPlan { get; set; }
        // Formula IsOpen (rulebook: =OR({{Status}} = "Open", {{Status}} = "Investigating"))
        public bool? IsOpen
        {
            get => OR(this.Status = "Open", this.Status = "Investigating"); set { }
        }

        // Formula OpenGapVersionKey (rulebook: =IF(AND({{IsOpen}}, {{Severity}} = "High"), {{ProcedureVersion}}, ""))
        public string? OpenGapVersionKey
        {
            get => IF(AND(this.IsOpen, this.Severity = "High"), this.ProcedureVersion, ""); set { }
        }

        // Formula IsBlocking (rulebook: ={{BlockingKind}} = "Blocking")
        public bool? IsBlocking
        {
            get => this.BlockingKind = "Blocking"; set { }
        }

        // Formula IsOpenAndBlocking (rulebook: =AND({{IsOpen}}, {{IsBlocking}}))
        public bool? IsOpenAndBlocking
        {
            get => AND(this.IsOpen, this.IsBlocking); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula DaysOpen (rulebook: =IF({{IsOpen}}, DATETIME_DIFF({{AsOfInstant}}, {{IdentifiedAt}}, "days"), 0))
        public int? DaysOpen
        {
            get => IF(this.IsOpen, DATETIME_DIFF(this.AsOfInstant, this.IdentifiedAt, "days"), 0); set { }
        }

        // Formula ToleranceDays (rulebook: =IF({{Severity}} = "High", 30, IF({{Severity}} = "Medium", 90, 180)))
        public int? ToleranceDays
        {
            get => IF(this.Severity = "High", 30, IF(this.Severity = "Medium", 90, 180)); set { }
        }

        // Formula IsOverdueGap (rulebook: ={{DaysOpen}} > {{ToleranceDays}})
        public bool? IsOverdueGap
        {
            get => this.DaysOpen > this.ToleranceDays; set { }
        }

        // Formula OwnerAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        public string? OwnerAgent
        {
            get => INDEX(Roles!this.CurrentAgent, MATCH(this.OwnerRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula OwnerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{OwnerAgent}}, Agents!{{AgentId}}, 0)))
        public bool? OwnerIsStillEngaged
        {
            get => INDEX(Agents!this.IsStillEngaged, MATCH(this.OwnerAgent, Agents!this.AgentId, 0)); set { }
        }

        // Formula HasResolutionPlan (rulebook: ={{ResolutionPlan}} <> "")
        public bool? HasResolutionPlan
        {
            get => this.ResolutionPlan <> ""; set { }
        }

        // Formula IsAbandonedUnknown (rulebook: =AND({{IsOverdueGap}}, OR(NOT({{HasResolutionPlan}}), NOT({{OwnerIsStillEngaged}}))))
        public bool? IsAbandonedUnknown
        {
            get => AND(this.IsOverdueGap, OR(NOT(this.HasResolutionPlan), NOT(this.OwnerIsStillEngaged))); set { }
        }

        // Formula OpenBlockingGapVersionKey (rulebook: =IF({{IsOpenAndBlocking}}, {{ProcedureVersion}}, ""))
        public string? OpenBlockingGapVersionKey
        {
            get => IF(this.IsOpenAndBlocking, this.ProcedureVersion, ""); set { }
        }

        // Formula OwnerRoleIsVacated (rulebook: =INDEX(Roles!{{IsVacatedRole}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        public bool? OwnerRoleIsVacated
        {
            get => INDEX(Roles!this.IsVacatedRole, MATCH(this.OwnerRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula IsOwnerlessOpenGap (rulebook: =AND({{IsOpen}}, {{OwnerRoleIsVacated}}))
        public bool? IsOwnerlessOpenGap
        {
            get => AND(this.IsOpen, this.OwnerRoleIsVacated); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? OwnerRole { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private Step _step;

        [ForeignKey("Step")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(Step))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _step = Context.Steps.Find(Step);
                    if (_step != null)
                    {
                        Context.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    Step = _step == null ? default : _step.StepId;
                }
            }
        }

        private Role _role;

        [ForeignKey("OwnerRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(OwnerRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(OwnerRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OwnerRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private EvaluationContext _evaluationContext;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContext
        {
            get
            {
                if (_evaluationContext == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContext - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContext = Context.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContext != null)
                    {
                        Context.Attach(_evaluationContext);
                    }
                }
                return _evaluationContext;
            }
            set
            {
                if (_evaluationContext != value)
                {
                    _evaluationContext = value;
                    EvaluationContext = _evaluationContext == null ? default : _evaluationContext.EvaluationContextId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersion;
            _ = this.Step;
            _ = this.Role;
            _ = this.EvaluationContext;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
