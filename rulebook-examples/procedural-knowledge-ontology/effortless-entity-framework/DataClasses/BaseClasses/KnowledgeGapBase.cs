
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
    [Table("KnowledgeGaps")]
    public class KnowledgeGapBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeGapId { get; set; }

        // Formula Name (rulebook: ={{Severity}} & ": " & LEFT({{Statement}}, 60))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Severity)), F.S(": "), F.Text(F.Left(F.Of(this.Statement), F.I(60)))))); set { }
        }

        public string? Statement { get; set; }
        public string? Severity { get; set; }
        public string? BlockingKind { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? IdentifiedAt { get; set; }
        public string? ResolutionPlan { get; set; }
        // Formula IsOpen (rulebook: =OR({{Status}} = "Open", {{Status}} = "Investigating"))
        [NotMapped]
        public bool? IsOpen
        {
            get => F.AsBool(F.Memo(this, "IsOpen", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Open"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Status)), F.S("Investigating")))))); set { }
        }

        // Formula OpenGapVersionKey (rulebook: =IF(AND({{IsOpen}}, {{Severity}} = "High"), {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? OpenGapVersionKey
        {
            get => F.AsString(F.Memo(this, "OpenGapVersionKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Of(this.IsOpen)), F.Bool3(F.Eq(F.Nullif(F.Of(this.Severity)), F.S("High")))))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula IsBlocking (rulebook: ={{BlockingKind}} = "Blocking")
        [NotMapped]
        public bool? IsBlocking
        {
            get => F.AsBool(F.Memo(this, "IsBlocking", () => F.Eq(F.Nullif(F.Of(this.BlockingKind)), F.S("Blocking")))); set { }
        }

        // Formula IsOpenAndBlocking (rulebook: =AND({{IsOpen}}, {{IsBlocking}}))
        [NotMapped]
        public bool? IsOpenAndBlocking
        {
            get => F.AsBool(F.Memo(this, "IsOpenAndBlocking", () => F.And(F.Bool3(F.Of(this.IsOpen)), F.Bool3(F.Of(this.IsBlocking))))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula DaysOpen (rulebook: =IF({{IsOpen}}, DATETIME_DIFF({{AsOfInstant}}, {{IdentifiedAt}}, "days"), 0))
        [NotMapped]
        public int? DaysOpen
        {
            get => F.AsInt(F.Memo(this, "DaysOpen", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.IsOpen))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.IdentifiedAt), F.S("days")) : F.I(0))))); set { }
        }

        // Formula ToleranceDays (rulebook: =IF({{Severity}} = "High", 30, IF({{Severity}} = "Medium", 90, 180)))
        [NotMapped]
        public int? ToleranceDays
        {
            get => F.AsInt(F.Memo(this, "ToleranceDays", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Severity)), F.S("High")))) ? F.I(30) : (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.Severity)), F.S("Medium")))) ? F.I(90) : F.I(180)))))); set { }
        }

        // Formula IsOverdueGap (rulebook: ={{DaysOpen}} > {{ToleranceDays}})
        [NotMapped]
        public bool? IsOverdueGap
        {
            get => F.AsBool(F.Memo(this, "IsOverdueGap", () => F.Cmp(F.Of(this.DaysOpen), ">", F.Of(this.ToleranceDays)))); set { }
        }

        // Formula OwnerAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? OwnerAgent
        {
            get => F.AsString(F.Memo(this, "OwnerAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula OwnerIsStillEngaged (rulebook: =INDEX(Agents!{{IsStillEngaged}}, MATCH({{OwnerAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public bool? OwnerIsStillEngaged
        {
            get => F.AsBool(F.Memo(this, "OwnerIsStillEngaged", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.OwnerAgent), __r => F.Of(__r.IsStillEngaged), () => F.Of(new Agent().IsStillEngaged)))); set { }
        }

        // Formula HasResolutionPlan (rulebook: ={{ResolutionPlan}} <> "")
        [NotMapped]
        public bool? HasResolutionPlan
        {
            get => F.AsBool(F.Memo(this, "HasResolutionPlan", () => F.IsNotBlank(F.Of(this.ResolutionPlan)))); set { }
        }

        // Formula IsAbandonedUnknown (rulebook: =AND({{IsOverdueGap}}, OR(NOT({{HasResolutionPlan}}), NOT({{OwnerIsStillEngaged}}))))
        [NotMapped]
        public bool? IsAbandonedUnknown
        {
            get => F.AsBool(F.Memo(this, "IsAbandonedUnknown", () => F.And(F.Bool3(F.Of(this.IsOverdueGap)), F.Bool3(F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.HasResolutionPlan)))), F.Bool3(F.Not(F.Bool3(F.Of(this.OwnerIsStillEngaged))))))))); set { }
        }

        // Formula OpenBlockingGapVersionKey (rulebook: =IF({{IsOpenAndBlocking}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? OpenBlockingGapVersionKey
        {
            get => F.AsString(F.Memo(this, "OpenBlockingGapVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsOpenAndBlocking))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula OwnerRoleIsVacated (rulebook: =INDEX(Roles!{{IsVacatedRole}}, MATCH({{OwnerRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? OwnerRoleIsVacated
        {
            get => F.AsBool(F.Memo(this, "OwnerRoleIsVacated", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.OwnerRole), __r => F.Of(__r.IsVacatedRole), () => F.Of(new Role().IsVacatedRole)))); set { }
        }

        // Formula IsOwnerlessOpenGap (rulebook: =AND({{IsOpen}}, {{OwnerRoleIsVacated}}))
        [NotMapped]
        public bool? IsOwnerlessOpenGap
        {
            get => F.AsBool(F.Memo(this, "IsOwnerlessOpenGap", () => F.And(F.Bool3(F.Of(this.IsOpen)), F.Bool3(F.Of(this.OwnerRoleIsVacated))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? OwnerRole { get; set; }
        public string? EvaluationContext { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
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
                        Step = _stepRef.StepId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. OwnerRole: " + OwnerRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(OwnerRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        OwnerRole = _role.RoleId;
                    }
                }
            }
        }

        private EvaluationContext _evaluationContextRef;

        [ForeignKey("EvaluationContext")]
        public virtual EvaluationContext EvaluationContextRef
        {
            get
            {
                if (_evaluationContextRef == null && !string.IsNullOrEmpty(EvaluationContext))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EvaluationContextRef - no database context is set. EvaluationContext: " + EvaluationContext + ".");
                        }
                        return null;
                    }
                    _evaluationContextRef = base.SoAContext.EvaluationContexts.Find(EvaluationContext);
                    if (_evaluationContextRef != null)
                    {
                        base.SoAContext.Attach(_evaluationContextRef);
                    }
                }
                return _evaluationContextRef;
            }
            set
            {
                if (_evaluationContextRef != value)
                {
                    _evaluationContextRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_evaluationContextRef != null)
                    {
                        EvaluationContext = _evaluationContextRef.EvaluationContextId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.Role;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
