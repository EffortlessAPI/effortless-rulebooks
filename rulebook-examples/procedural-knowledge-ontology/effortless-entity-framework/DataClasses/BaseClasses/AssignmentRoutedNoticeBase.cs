
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
    [Table("AssignmentRoutedNotices")]
    public class AssignmentRoutedNoticeBase : SoAEntityBase
    {
        [Key]
        public string AssignmentRoutedNoticeId { get; set; }

        // Formula Name (rulebook: ={{ProcedureExecution}} & " / " & {{NoticeStep}} & " -> " & {{RoutedToAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProcedureExecution)), F.S(" / "), F.Text(F.Of(this.NoticeStep)), F.S(" -> "), F.Text(F.Of(this.RoutedToAgent))))); set { }
        }

        public string? NoticeKind { get; set; }
        public DateTimeOffset? SentAt { get; set; }
        public string? RoutingSource { get; set; }
        // Formula NoticeRole (rulebook: =INDEX(Steps!{{AssignedRole}}, MATCH({{NoticeStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? NoticeRole
        {
            get => F.AsString(F.Memo(this, "NoticeRole", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.NoticeStep), __r => F.Of(__r.AssignedRole), () => F.Of(new Step().AssignedRole)))); set { }
        }

        // Formula RecipientRoleKey (rulebook: ={{RoutedToAgent}} & "|" & {{NoticeRole}})
        [NotMapped]
        public string? RecipientRoleKey
        {
            get => F.AsString(F.Memo(this, "RecipientRoleKey", () => F.Concat(F.Text(F.Of(this.RoutedToAgent)), F.S("|"), F.Text(F.Of(this.NoticeRole))))); set { }
        }

        // Formula RecipientPairAssignmentCount (rulebook: =COUNTIFS(RoleAssignments!{{AgentRolePairKey}}, {{RecipientRoleKey}}))
        [NotMapped]
        public int? RecipientPairAssignmentCount
        {
            get => F.AsInt(F.Memo(this, "RecipientPairAssignmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.AgentRolePairKey), F.Of(this.RecipientRoleKey))))))); set { }
        }

        // Formula RecipientValidFrom (rulebook: =MAXIFS(RoleAssignments!{{ValidFrom}}, RoleAssignments!{{AgentRolePairKey}}, {{RecipientRoleKey}}))
        [NotMapped]
        public DateTimeOffset? RecipientValidFrom
        {
            get => F.AsDateTime(F.Memo(this, "RecipientValidFrom", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.AgentRolePairKey), F.Of(this.RecipientRoleKey)), __r => F.Of(__r.ValidFrom))))); set { }
        }

        // Formula RecipientLatestValidTo (rulebook: =MAXIFS(RoleAssignments!{{ValidTo}}, RoleAssignments!{{AgentRolePairKey}}, {{RecipientRoleKey}}))
        [NotMapped]
        public DateTimeOffset? RecipientLatestValidTo
        {
            get => F.AsDateTime(F.Memo(this, "RecipientLatestValidTo", () => (base.SoAContext == null ? F.Null : F.ExtremeIfs(true, F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.AgentRolePairKey), F.Of(this.RecipientRoleKey)), __r => F.Of(__r.ValidTo))))); set { }
        }

        // Formula RecipientOpenEndedCount (rulebook: =COUNTIFS(RoleAssignments!{{AgentRolePairKey}}, {{RecipientRoleKey}}, RoleAssignments!{{IsOpenEnded}}, TRUE))
        [NotMapped]
        public int? RecipientOpenEndedCount
        {
            get => F.AsInt(F.Memo(this, "RecipientOpenEndedCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RoleAssignment>(base.SoAContext, "RoleAssignments", __c => __c.RoleAssignments), __r => F.CritField(F.Of(__r.AgentRolePairKey), F.Of(this.RecipientRoleKey)) && F.CritLiteral(F.Of(__r.IsOpenEnded), F.B(true))))))); set { }
        }

        // Formula RecipientHeldRoleWhenSent (rulebook: =AND({{RoutedToAgent}} <> "", {{RecipientPairAssignmentCount}} > 0, {{RecipientValidFrom}} <= {{SentAt}}, OR({{RecipientOpenEndedCount}} > 0, {{RecipientLatestValidTo}} > {{SentAt}})))
        [NotMapped]
        public bool? RecipientHeldRoleWhenSent
        {
            get => F.AsBool(F.Memo(this, "RecipientHeldRoleWhenSent", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RoutedToAgent))), F.Bool3(F.Cmp(F.Of(this.RecipientPairAssignmentCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.RecipientValidFrom), "<=", F.Nullif(F.Of(this.SentAt)))), F.Bool3(F.Or(F.Bool3(F.Cmp(F.Of(this.RecipientOpenEndedCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.RecipientLatestValidTo), ">", F.Nullif(F.Of(this.SentAt))))))))); set { }
        }

        // Formula ReachedWrongPersonOrNobody (rulebook: =NOT({{RecipientHeldRoleWhenSent}}))
        [NotMapped]
        public bool? ReachedWrongPersonOrNobody
        {
            get => F.AsBool(F.Memo(this, "ReachedWrongPersonOrNobody", () => F.Not(F.Bool3(F.Of(this.RecipientHeldRoleWhenSent))))); set { }
        }

        // Formula RoutedAroundModel (rulebook: =AND({{RoutingSource}} <> "RoleAssignments", NOT({{RecipientHeldRoleWhenSent}})))
        [NotMapped]
        public bool? RoutedAroundModel
        {
            get => F.AsBool(F.Memo(this, "RoutedAroundModel", () => F.And(F.Bool3(F.Ne(F.Nullif(F.Of(this.RoutingSource)), F.S("RoleAssignments"))), F.Bool3(F.Not(F.Bool3(F.Of(this.RecipientHeldRoleWhenSent))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? NoticeStep { get; set; }
        public string? RoutedToAgent { get; set; }

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

        private Step _step;

        [ForeignKey("NoticeStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(NoticeStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. NoticeStep: " + NoticeStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(NoticeStep);
                    if (_step != null)
                    {
                        base.SoAContext.Attach(_step);
                    }
                }
                return _step;
            }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_step != null)
                    {
                        NoticeStep = _step.StepId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("RoutedToAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(RoutedToAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. RoutedToAgent: " + RoutedToAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(RoutedToAgent);
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
                        RoutedToAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.Step;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
