
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
    [Table("FailureModes")]
    public class FailureModeBase : SoAEntityBase
    {
        [Key]
        public string FailureModeId { get; set; }

        // Formula Name (rulebook: ={{Step}} & ": " & LEFT({{Description}}, 40))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(": "), F.Text(F.Left(F.Of(this.Description), F.I(40)))))); set { }
        }

        public string? Description { get; set; }
        public string? Response { get; set; }
        public bool? RequiresEscalation { get; set; }
        // Formula EscalationRoleHasNoHolder (rulebook: =INDEX(Roles!{{HasNoCurrentHolder}}, MATCH({{EscalateToRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? EscalationRoleHasNoHolder
        {
            get => F.AsBool(F.Memo(this, "EscalationRoleHasNoHolder", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.EscalateToRole), __r => F.Of(__r.HasNoCurrentHolder), () => F.Of(new Role().HasNoCurrentHolder)))); set { }
        }

        // Formula EscalatesToVacantRole (rulebook: =AND({{RequiresEscalation}}, {{EscalationRoleHasNoHolder}}))
        [NotMapped]
        public bool? EscalatesToVacantRole
        {
            get => F.AsBool(F.Memo(this, "EscalatesToVacantRole", () => F.And(F.IsTrueV(F.Of(this.RequiresEscalation)), F.Bool3(F.Of(this.EscalationRoleHasNoHolder))))); set { }
        }

        // Formula HasNoResponse (rulebook: ={{Response}} = "")
        [NotMapped]
        public bool? HasNoResponse
        {
            get => F.AsBool(F.Memo(this, "HasNoResponse", () => F.IsBlank(F.Of(this.Response)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? ProcedureTarget { get; set; }
        public string? EscalateToRole { get; set; }

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

        private ProcedureTarget _procedureTargetRef;

        [ForeignKey("ProcedureTarget")]
        public virtual ProcedureTarget ProcedureTargetRef
        {
            get
            {
                if (_procedureTargetRef == null && !string.IsNullOrEmpty(ProcedureTarget))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureTargetRef - no database context is set. ProcedureTarget: " + ProcedureTarget + ".");
                        }
                        return null;
                    }
                    _procedureTargetRef = base.SoAContext.ProcedureTargets.Find(ProcedureTarget);
                    if (_procedureTargetRef != null)
                    {
                        base.SoAContext.Attach(_procedureTargetRef);
                    }
                }
                return _procedureTargetRef;
            }
            set
            {
                if (_procedureTargetRef != value)
                {
                    _procedureTargetRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureTargetRef != null)
                    {
                        ProcedureTarget = _procedureTargetRef.ProcedureTargetId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("EscalateToRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(EscalateToRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. EscalateToRole: " + EscalateToRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(EscalateToRole);
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
                        EscalateToRole = _role.RoleId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.ProcedureTargetRef;
            _ = this.Role;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
