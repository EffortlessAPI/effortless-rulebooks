
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
    [Table("AssignmentInstantChecks")]
    public class AssignmentInstantCheckBase : SoAEntityBase
    {
        [Key]
        public string AssignmentInstantCheckId { get; set; }

        // Formula Name (rulebook: ={{Step}} & " / " & {{RoleAssignment}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Step)), F.S(" / "), F.Text(F.Of(this.RoleAssignment))))); set { }
        }

        public string? AuditQuestion { get; set; }
        public DateTimeOffset? AuditInstant { get; set; }
        // Formula StepRole (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{Step}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? StepRole
        {
            get => F.AsString(F.Memo(this, "StepRole", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.Step), __r => F.Of(__r.AssignedRole), () => F.Of(new Step().AssignedRole)))); set { }
        }

        // Formula AssignmentRole (rulebook: =INDEX(RoleAssignments!{{Role}}, MATCH({{RoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public string? AssignmentRole
        {
            get => F.AsString(F.Memo(this, "AssignmentRole", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.RoleAssignment), __r => F.Of(__r.Role), () => F.Of(new RoleAssignment().Role)))); set { }
        }

        // Formula AssignmentValidFrom (rulebook: =INDEX(RoleAssignments!{{ValidFrom}}, MATCH({{RoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AssignmentValidFrom
        {
            get => F.AsDateTime(F.Memo(this, "AssignmentValidFrom", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.RoleAssignment), __r => F.Of(__r.ValidFrom), () => F.Of(new RoleAssignment().ValidFrom)))); set { }
        }

        // Formula AssignmentValidTo (rulebook: =INDEX(RoleAssignments!{{ValidTo}}, MATCH({{RoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AssignmentValidTo
        {
            get => F.AsDateTime(F.Memo(this, "AssignmentValidTo", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.RoleAssignment), __r => F.Of(__r.ValidTo), () => F.Of(new RoleAssignment().ValidTo)))); set { }
        }

        // Formula AssignmentAgent (rulebook: =INDEX(RoleAssignments!{{Agent}}, MATCH({{RoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public string? AssignmentAgent
        {
            get => F.AsString(F.Memo(this, "AssignmentAgent", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.RoleAssignment), __r => F.Of(__r.Agent), () => F.Of(new RoleAssignment().Agent)))); set { }
        }

        // Formula AssignmentAgentVersion (rulebook: =INDEX(RoleAssignments!{{AgentVersionKey}}, MATCH({{RoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public string? AssignmentAgentVersion
        {
            get => F.AsString(F.Memo(this, "AssignmentAgentVersion", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.RoleAssignment), __r => F.Of(__r.AgentVersionKey), () => F.Of(new RoleAssignment().AgentVersionKey)))); set { }
        }

        // Formula HeldStepAtInstant (rulebook: =AND({{AssignmentRole}} = {{StepRole}}, {{AssignmentValidFrom}} <= {{AuditInstant}}, OR({{AssignmentValidTo}} = "", {{AssignmentValidTo}} > {{AuditInstant}})))
        [NotMapped]
        public bool? HeldStepAtInstant
        {
            get => F.AsBool(F.Memo(this, "HeldStepAtInstant", () => F.And(F.Bool3(F.Eq(F.Of(this.AssignmentRole), F.Of(this.StepRole))), F.Bool3(F.Cmp(F.Of(this.AssignmentValidFrom), "<=", F.Nullif(F.Of(this.AuditInstant)))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.AssignmentValidTo))), F.Bool3(F.Cmp(F.Of(this.AssignmentValidTo), ">", F.Nullif(F.Of(this.AuditInstant))))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Step { get; set; }
        public string? RoleAssignment { get; set; }

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

        private RoleAssignment _roleAssignmentRef;

        [ForeignKey("RoleAssignment")]
        public virtual RoleAssignment RoleAssignmentRef
        {
            get
            {
                if (_roleAssignmentRef == null && !string.IsNullOrEmpty(RoleAssignment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleAssignmentRef - no database context is set. RoleAssignment: " + RoleAssignment + ".");
                        }
                        return null;
                    }
                    _roleAssignmentRef = base.SoAContext.RoleAssignments.Find(RoleAssignment);
                    if (_roleAssignmentRef != null)
                    {
                        base.SoAContext.Attach(_roleAssignmentRef);
                    }
                }
                return _roleAssignmentRef;
            }
            set
            {
                if (_roleAssignmentRef != value)
                {
                    _roleAssignmentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleAssignmentRef != null)
                    {
                        RoleAssignment = _roleAssignmentRef.RoleAssignmentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepRef;
            _ = this.RoleAssignmentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
