
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
    [Table("Steps")]
    public class StepBase : SoAEntityBase
    {
        [Key]
        public string StepId { get; set; }

        // Formula Name (rulebook: ={{StepNumber}} & ". " & {{Title}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.StepNumber)), F.S(". "), F.Text(F.Of(this.Title))))); set { }
        }

        public string? StepNumber { get; set; }
        public string? Title { get; set; }
        public string? StepKind { get; set; }
        // Formula AssignedRoleLabel (rulebook: =INDEX(Roles!{{Label}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AssignedRoleLabel
        {
            get => F.AsString(F.Memo(this, "AssignedRoleLabel", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.Label), () => F.Of(new Role().Label)))); set { }
        }

        // Formula AssignedAgentKind (rulebook: =INDEX(Roles!{{CurrentAgentKind}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AssignedAgentKind
        {
            get => F.AsString(F.Memo(this, "AssignedAgentKind", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.CurrentAgentKind), () => F.Of(new Role().CurrentAgentKind)))); set { }
        }

        public string? Instruction { get; set; }
        public int? ExpectedDurationMinutes { get; set; }
        public string? ExpertiseLevel { get; set; }
        public bool? RequiresHumanConfirmation { get; set; }
        // Formula BlockingRequirementCount (rulebook: =COUNTIFS(StepRequirements!{{BlockingStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? BlockingRequirementCount
        {
            get => F.AsDecimal(F.Memo(this, "BlockingRequirementCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepRequirement>(base.SoAContext, "StepRequirements", __c => __c.StepRequirements), __r => F.CritField(F.Of(__r.BlockingStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula StaleBindingCount (rulebook: =COUNTIFS(OperationalBindings!{{StaleBindingStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? StaleBindingCount
        {
            get => F.AsDecimal(F.Memo(this, "StaleBindingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OperationalBinding>(base.SoAContext, "OperationalBindings", __c => __c.OperationalBindings), __r => F.CritField(F.Of(__r.StaleBindingStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula AuthoritativeStaleCount (rulebook: =COUNTIFS(OperationalBindings!{{AuthoritativeStaleStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? AuthoritativeStaleCount
        {
            get => F.AsDecimal(F.Memo(this, "AuthoritativeStaleCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OperationalBinding>(base.SoAContext, "OperationalBindings", __c => __c.OperationalBindings), __r => F.CritField(F.Of(__r.AuthoritativeStaleStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula AvailableExceptionCount (rulebook: =COUNTIFS(Exceptions!{{ActiveExceptionStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? AvailableExceptionCount
        {
            get => F.AsDecimal(F.Memo(this, "AvailableExceptionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Exception>(base.SoAContext, "Exceptions", __c => __c.Exceptions), __r => F.CritField(F.Of(__r.ActiveExceptionStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula DeclaredVerificationCount (rulebook: =COUNTIFS(StepVerifications!{{Step}}, {{StepId}}))
        [NotMapped]
        public decimal? DeclaredVerificationCount
        {
            get => F.AsDecimal(F.Memo(this, "DeclaredVerificationCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepVerification>(base.SoAContext, "StepVerifications", __c => __c.StepVerifications), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)))))); set { }
        }

        // Formula IsPreparationStep (rulebook: =OR({{AssignedRole}} = "finance-analyst", {{AssignedRole}} = "variance-review-agent"))
        [NotMapped]
        public bool? IsPreparationStep
        {
            get => F.AsBool(F.Memo(this, "IsPreparationStep", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.AssignedRole)), F.S("finance-analyst"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.AssignedRole)), F.S("variance-review-agent")))))); set { }
        }

        // Formula IsApprovalStep (rulebook: =OR({{AssignedRole}} = "controller", {{AssignedRole}} = "cfo"))
        [NotMapped]
        public bool? IsApprovalStep
        {
            get => F.AsBool(F.Memo(this, "IsApprovalStep", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.AssignedRole)), F.S("controller"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.AssignedRole)), F.S("cfo")))))); set { }
        }

        // Formula StaleAuthoritativeBindingCount (rulebook: =COUNTIFS(OperationalBindings!{{StepWhenStale}}, {{StepId}}))
        [NotMapped]
        public decimal? StaleAuthoritativeBindingCount
        {
            get => F.AsDecimal(F.Memo(this, "StaleAuthoritativeBindingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OperationalBinding>(base.SoAContext, "OperationalBindings", __c => __c.OperationalBindings), __r => F.CritField(F.Of(__r.StepWhenStale), F.Of(this.StepId)))))); set { }
        }

        // Formula InputsAreFresh (rulebook: ={{StaleAuthoritativeBindingCount}} = 0)
        [NotMapped]
        public bool? InputsAreFresh
        {
            get => F.AsBool(F.Memo(this, "InputsAreFresh", () => F.Eq(F.Of(this.StaleAuthoritativeBindingCount), F.I(0)))); set { }
        }

        // Formula IsSoftwareAssigned (rulebook: =OR({{AssignedAgentKind}} = "AIAgent", {{AssignedAgentKind}} = "AutomatedPipeline"))
        [NotMapped]
        public bool? IsSoftwareAssigned
        {
            get => F.AsBool(F.Memo(this, "IsSoftwareAssigned", () => F.Or(F.Bool3(F.Eq(F.Of(this.AssignedAgentKind), F.S("AIAgent"))), F.Bool3(F.Eq(F.Of(this.AssignedAgentKind), F.S("AutomatedPipeline")))))); set { }
        }

        // Formula IsHumanApprovalGate (rulebook: =AND(NOT({{IsSoftwareAssigned}}), OR({{StepId}} = "policy-05", {{StepId}} = "close-06")))
        [NotMapped]
        public bool? IsHumanApprovalGate
        {
            get => F.AsBool(F.Memo(this, "IsHumanApprovalGate", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsSoftwareAssigned)))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.StepId)), F.S("policy-05"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.StepId)), F.S("close-06")))))))); set { }
        }

        // Formula GateHeldByHuman (rulebook: =AND({{IsHumanApprovalGate}}, {{AssignedAgentKind}} = "Human"))
        [NotMapped]
        public bool? GateHeldByHuman
        {
            get => F.AsBool(F.Memo(this, "GateHeldByHuman", () => F.And(F.Bool3(F.Of(this.IsHumanApprovalGate)), F.Bool3(F.Eq(F.Of(this.AssignedAgentKind), F.S("Human")))))); set { }
        }

        // Formula BindingBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{StepWhenBinding}}, {{StepId}}))
        [NotMapped]
        public decimal? BindingBoundaryCount
        {
            get => F.AsDecimal(F.Memo(this, "BindingBoundaryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthorityBoundary>(base.SoAContext, "AuthorityBoundaries", __c => __c.AuthorityBoundaries), __r => F.CritField(F.Of(__r.StepWhenBinding), F.Of(this.StepId)))))); set { }
        }

        // Formula AssignedRoleIsUngoverned (rulebook: =INDEX(Roles!{{IsUngovernedNonHumanRole}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? AssignedRoleIsUngoverned
        {
            get => F.AsBool(F.Memo(this, "AssignedRoleIsUngoverned", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.IsUngovernedNonHumanRole), () => F.Of(new Role().IsUngovernedNonHumanRole)))); set { }
        }

        // Formula UnusableBindingCount (rulebook: =COUNTIFS(OperationalBindings!{{StepWhenUnusable}}, {{StepId}}))
        [NotMapped]
        public decimal? UnusableBindingCount
        {
            get => F.AsDecimal(F.Memo(this, "UnusableBindingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<OperationalBinding>(base.SoAContext, "OperationalBindings", __c => __c.OperationalBindings), __r => F.CritField(F.Of(__r.StepWhenUnusable), F.Of(this.StepId)))))); set { }
        }

        // Formula AllSourcesUsable (rulebook: ={{UnusableBindingCount}} = 0)
        [NotMapped]
        public bool? AllSourcesUsable
        {
            get => F.AsBool(F.Memo(this, "AllSourcesUsable", () => F.Eq(F.Of(this.UnusableBindingCount), F.I(0)))); set { }
        }

        public string? ControlKind { get; set; }
        // Formula UnwarrantedBoundaryCount (rulebook: =COUNTIFS(AuthorityBoundaries!{{UnwarrantedBoundaryStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? UnwarrantedBoundaryCount
        {
            get => F.AsDecimal(F.Memo(this, "UnwarrantedBoundaryCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AuthorityBoundary>(base.SoAContext, "AuthorityBoundaries", __c => __c.AuthorityBoundaries), __r => F.CritField(F.Of(__r.UnwarrantedBoundaryStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula IsGovernedByUnwarrantedBoundary (rulebook: ={{UnwarrantedBoundaryCount}} > 0)
        [NotMapped]
        public bool? IsGovernedByUnwarrantedBoundary
        {
            get => F.AsBool(F.Memo(this, "IsGovernedByUnwarrantedBoundary", () => F.Cmp(F.Of(this.UnwarrantedBoundaryCount), ">", F.I(0)))); set { }
        }

        // Formula SoftwareExecutionCount (rulebook: =COUNTIFS(StepExecutions!{{SoftwareExecutionStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? SoftwareExecutionCount
        {
            get => F.AsDecimal(F.Memo(this, "SoftwareExecutionCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.SoftwareExecutionStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula HasBeenApproachedBySoftware (rulebook: ={{SoftwareExecutionCount}} > 0)
        [NotMapped]
        public bool? HasBeenApproachedBySoftware
        {
            get => F.AsBool(F.Memo(this, "HasBeenApproachedBySoftware", () => F.Cmp(F.Of(this.SoftwareExecutionCount), ">", F.I(0)))); set { }
        }

        // Formula IsUnexercisedHumanGate (rulebook: =AND({{IsHumanApprovalGate}}, NOT({{HasBeenApproachedBySoftware}})))
        [NotMapped]
        public bool? IsUnexercisedHumanGate
        {
            get => F.AsBool(F.Memo(this, "IsUnexercisedHumanGate", () => F.And(F.Bool3(F.Of(this.IsHumanApprovalGate)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasBeenApproachedBySoftware))))))); set { }
        }

        // Formula IsDemonstratedHumanGate (rulebook: =AND({{IsHumanApprovalGate}}, {{HasBeenApproachedBySoftware}}, {{GateHeldByHuman}}))
        [NotMapped]
        public bool? IsDemonstratedHumanGate
        {
            get => F.AsBool(F.Memo(this, "IsDemonstratedHumanGate", () => F.And(F.Bool3(F.Of(this.IsHumanApprovalGate)), F.Bool3(F.Of(this.HasBeenApproachedBySoftware)), F.Bool3(F.Of(this.GateHeldByHuman))))); set { }
        }

        // Formula UnexercisedGateVersionKey (rulebook: =IF({{IsUnexercisedHumanGate}}, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? UnexercisedGateVersionKey
        {
            get => F.AsString(F.Memo(this, "UnexercisedGateVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnexercisedHumanGate))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula HasDeclaredControlKind (rulebook: ={{ControlKind}} <> "")
        [NotMapped]
        public bool? HasDeclaredControlKind
        {
            get => F.AsBool(F.Memo(this, "HasDeclaredControlKind", () => F.IsNotBlank(F.Of(this.ControlKind)))); set { }
        }

        // Formula UndeclaredControlVersionKey (rulebook: =IF({{HasDeclaredControlKind}}, "", {{ProcedureVersion}}))
        [NotMapped]
        public string? UndeclaredControlVersionKey
        {
            get => F.AsString(F.Memo(this, "UndeclaredControlVersionKey", () => (F.Truthy(F.Bool3(F.Of(this.HasDeclaredControlKind))) ? F.S("") : F.Of(this.ProcedureVersion)))); set { }
        }

        // Formula ApprovalStepIsSoftwareAssigned (rulebook: =AND({{ControlKind}} = "Approval", {{IsSoftwareAssigned}}))
        [NotMapped]
        public bool? ApprovalStepIsSoftwareAssigned
        {
            get => F.AsBool(F.Memo(this, "ApprovalStepIsSoftwareAssigned", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ControlKind)), F.S("Approval"))), F.Bool3(F.Of(this.IsSoftwareAssigned))))); set { }
        }

        // Formula UnwitnessedBlockingCount (rulebook: =COUNTIFS(StepRequirements!{{UnwitnessedStepKey}}, {{StepId}}))
        [NotMapped]
        public decimal? UnwitnessedBlockingCount
        {
            get => F.AsDecimal(F.Memo(this, "UnwitnessedBlockingCount", () => (base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepRequirement>(base.SoAContext, "StepRequirements", __c => __c.StepRequirements), __r => F.CritField(F.Of(__r.UnwitnessedStepKey), F.Of(this.StepId)))))); set { }
        }

        // Formula ReachableStepCount (rulebook: =COUNTIFS(vw_step_transitions_closure!{{FromId}}, Steps!{{StepId}}))
        [NotMapped]
        public int? ReachableStepCount
        {
            get => F.AsInt(F.Memo(this, "ReachableStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Closure<StepTransition>(base.SoAContext, "vw_step_transitions_closure", "StepTransitions", __c => __c.StepTransitions, __e => F.Of(__e.FromStep), __e => F.Of(__e.ToStep), __e => true), __r => F.CritField(F.Of(__r.FromId), F.Of(this.StepId))))))); set { }
        }

        // Formula ReachedFromStepCount (rulebook: =COUNTIFS(vw_step_transitions_closure!{{ToId}}, Steps!{{StepId}}))
        [NotMapped]
        public int? ReachedFromStepCount
        {
            get => F.AsInt(F.Memo(this, "ReachedFromStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Closure<StepTransition>(base.SoAContext, "vw_step_transitions_closure", "StepTransitions", __c => __c.StepTransitions, __e => F.Of(__e.FromStep), __e => F.Of(__e.ToStep), __e => true), __r => F.CritField(F.Of(__r.ToId), F.Of(this.StepId))))))); set { }
        }

        // Formula SelfReachCount (rulebook: =COUNTIFS(vw_step_transitions_closure!{{FromId}}, Steps!{{StepId}}, vw_step_transitions_closure!{{ToId}}, Steps!{{StepId}}))
        [NotMapped]
        public int? SelfReachCount
        {
            get => F.AsInt(F.Memo(this, "SelfReachCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Closure<StepTransition>(base.SoAContext, "vw_step_transitions_closure", "StepTransitions", __c => __c.StepTransitions, __e => F.Of(__e.FromStep), __e => F.Of(__e.ToStep), __e => true), __r => F.CritField(F.Of(__r.FromId), F.Of(this.StepId)) && F.CritField(F.Of(__r.ToId), F.Of(this.StepId))))))); set { }
        }

        // Formula IsOnReworkLoop (rulebook: ={{SelfReachCount}} > 0)
        [NotMapped]
        public bool? IsOnReworkLoop
        {
            get => F.AsBool(F.Memo(this, "IsOnReworkLoop", () => F.Cmp(F.Of(this.SelfReachCount), ">", F.I(0)))); set { }
        }

        // Formula IsBlockingControlOnReworkLoop (rulebook: =AND({{IsOnReworkLoop}}, {{BlockingRequirementCount}} > 0))
        [NotMapped]
        public bool? IsBlockingControlOnReworkLoop
        {
            get => F.AsBool(F.Memo(this, "IsBlockingControlOnReworkLoop", () => F.And(F.Bool3(F.Of(this.IsOnReworkLoop)), F.Bool3(F.Cmp(F.Of(this.BlockingRequirementCount), ">", F.I(0)))))); set { }
        }

        // Formula IncomingTransitionCount (rulebook: =COUNTIFS(StepTransitions!{{ToStep}}, Steps!{{StepId}}))
        [NotMapped]
        public int? IncomingTransitionCount
        {
            get => F.AsInt(F.Memo(this, "IncomingTransitionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepTransition>(base.SoAContext, "StepTransitions", __c => __c.StepTransitions), __r => F.CritField(F.Of(__r.ToStep), F.Of(this.StepId))))))); set { }
        }

        // Formula IsEntryStep (rulebook: ={{IncomingTransitionCount}} = 0)
        [NotMapped]
        public bool? IsEntryStep
        {
            get => F.AsBool(F.Memo(this, "IsEntryStep", () => F.Eq(F.Of(this.IncomingTransitionCount), F.I(0)))); set { }
        }

        // Formula EntryStepKey (rulebook: =IF({{IsEntryStep}}, {{StepId}}, ""))
        [NotMapped]
        public string? EntryStepKey
        {
            get => F.AsString(F.Memo(this, "EntryStepKey", () => (F.Truthy(F.Bool3(F.Of(this.IsEntryStep))) ? F.Of(this.StepId) : F.S("")))); set { }
        }

        // Formula VersionEntryStepId (rulebook: =INDEX(ProcedureVersions!{{EntryStepId}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? VersionEntryStepId
        {
            get => F.AsString(F.Memo(this, "VersionEntryStepId", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.EntryStepId), () => F.Of(new ProcedureVersion().EntryStepId)))); set { }
        }

        // Formula GateFreeReachFromEntryCount (rulebook: =COUNTIFS(vw_step_transitions_closure_where_avoids_human_approval_gate!{{FromId}}, {{VersionEntryStepId}}, vw_step_transitions_closure_where_avoids_human_approval_gate!{{ToId}}, Steps!{{StepId}}))
        [NotMapped]
        public int? GateFreeReachFromEntryCount
        {
            get => F.AsInt(F.Memo(this, "GateFreeReachFromEntryCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Closure<StepTransition>(base.SoAContext, "vw_step_transitions_closure_where_avoids_human_approval_gate", "StepTransitions", __c => __c.StepTransitions, __e => F.Of(__e.FromStep), __e => F.Of(__e.ToStep), __e => F.Truthy(F.Of(__e.AvoidsHumanApprovalGate))), __r => F.CritField(F.Of(__r.FromId), F.Of(this.VersionEntryStepId)) && F.CritField(F.Of(__r.ToId), F.Of(this.StepId))))))); set { }
        }

        // Formula IsReachableFromEntryWithoutHumanGate (rulebook: ={{GateFreeReachFromEntryCount}} > 0)
        [NotMapped]
        public bool? IsReachableFromEntryWithoutHumanGate
        {
            get => F.AsBool(F.Memo(this, "IsReachableFromEntryWithoutHumanGate", () => F.Cmp(F.Of(this.GateFreeReachFromEntryCount), ">", F.I(0)))); set { }
        }

        // Formula IsGateBypassedPublication (rulebook: =AND({{ControlKind}} = "Publication", {{IsReachableFromEntryWithoutHumanGate}}))
        [NotMapped]
        public bool? IsGateBypassedPublication
        {
            get => F.AsBool(F.Memo(this, "IsGateBypassedPublication", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ControlKind)), F.S("Publication"))), F.Bool3(F.Of(this.IsReachableFromEntryWithoutHumanGate))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? AssignedRole { get; set; }

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

        private Role _role;

        [ForeignKey("AssignedRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AssignedRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AssignedRole: " + AssignedRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AssignedRole);
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
                        AssignedRole = _role.RoleId;
                    }
                }
            }
        }

        private ObservableCollection<StepTransition> _fromStepStepTransitions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<StepTransition> FromStepStepTransitions
        {
            get
            {
                if (_fromStepStepTransitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromStepStepTransitions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _fromStepStepTransitions = new ObservableCollection<StepTransition>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepTransitions.Where(x => x.FromStep == this.StepId).ToList<StepTransition>();
                        _fromStepStepTransitions = new ObservableCollection<StepTransition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromStepStepTransitions.CollectionChanged += FromStepStepTransitions_CollectionChanged;
                }
                return _fromStepStepTransitions;
            }
            private set
            {
                if (_fromStepStepTransitions != null)
                {
                    _fromStepStepTransitions.CollectionChanged -= FromStepStepTransitions_CollectionChanged;
                }
                _fromStepStepTransitions = value;
                if (_fromStepStepTransitions != null)
                {
                    _fromStepStepTransitions.CollectionChanged += FromStepStepTransitions_CollectionChanged;
                }
            }
        }

        private void FromStepStepTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTransition>())
                {
                    item.FromStep = this.StepId;
                }
            }
        }

        private ObservableCollection<StepTransition> _toStepStepTransitions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepTransition> ToStepStepTransitions
        {
            get
            {
                if (_toStepStepTransitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToStepStepTransitions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _toStepStepTransitions = new ObservableCollection<StepTransition>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepTransitions.Where(x => x.ToStep == this.StepId).ToList<StepTransition>();
                        _toStepStepTransitions = new ObservableCollection<StepTransition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toStepStepTransitions.CollectionChanged += ToStepStepTransitions_CollectionChanged;
                }
                return _toStepStepTransitions;
            }
            private set
            {
                if (_toStepStepTransitions != null)
                {
                    _toStepStepTransitions.CollectionChanged -= ToStepStepTransitions_CollectionChanged;
                }
                _toStepStepTransitions = value;
                if (_toStepStepTransitions != null)
                {
                    _toStepStepTransitions.CollectionChanged += ToStepStepTransitions_CollectionChanged;
                }
            }
        }

        private void ToStepStepTransitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTransition>())
                {
                    item.ToStep = this.StepId;
                }
            }
        }

        private ObservableCollection<StepAction> _stepActions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepAction> StepActions
        {
            get
            {
                if (_stepActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepActions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepActions = new ObservableCollection<StepAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepActions.Where(x => x.Step == this.StepId).ToList<StepAction>();
                        _stepActions = new ObservableCollection<StepAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepActions.CollectionChanged += StepActions_CollectionChanged;
                }
                return _stepActions;
            }
            private set
            {
                if (_stepActions != null)
                {
                    _stepActions.CollectionChanged -= StepActions_CollectionChanged;
                }
                _stepActions = value;
                if (_stepActions != null)
                {
                    _stepActions.CollectionChanged += StepActions_CollectionChanged;
                }
            }
        }

        private void StepActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepAction>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepFunction> _stepFunctions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepFunction> StepFunctions
        {
            get
            {
                if (_stepFunctions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepFunctions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepFunctions = new ObservableCollection<StepFunction>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepFunctions.Where(x => x.Step == this.StepId).ToList<StepFunction>();
                        _stepFunctions = new ObservableCollection<StepFunction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepFunctions.CollectionChanged += StepFunctions_CollectionChanged;
                }
                return _stepFunctions;
            }
            private set
            {
                if (_stepFunctions != null)
                {
                    _stepFunctions.CollectionChanged -= StepFunctions_CollectionChanged;
                }
                _stepFunctions = value;
                if (_stepFunctions != null)
                {
                    _stepFunctions.CollectionChanged += StepFunctions_CollectionChanged;
                }
            }
        }

        private void StepFunctions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepFunction>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepTool> _stepTools;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepTool> StepTools
        {
            get
            {
                if (_stepTools == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepTools - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepTools = new ObservableCollection<StepTool>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepTools.Where(x => x.Step == this.StepId).ToList<StepTool>();
                        _stepTools = new ObservableCollection<StepTool>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepTools.CollectionChanged += StepTools_CollectionChanged;
                }
                return _stepTools;
            }
            private set
            {
                if (_stepTools != null)
                {
                    _stepTools.CollectionChanged -= StepTools_CollectionChanged;
                }
                _stepTools = value;
                if (_stepTools != null)
                {
                    _stepTools.CollectionChanged += StepTools_CollectionChanged;
                }
            }
        }

        private void StepTools_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepTool>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepRequirement> _stepRequirements;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepRequirement> StepRequirements
        {
            get
            {
                if (_stepRequirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRequirements - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepRequirements = new ObservableCollection<StepRequirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepRequirements.Where(x => x.Step == this.StepId).ToList<StepRequirement>();
                        _stepRequirements = new ObservableCollection<StepRequirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepRequirements.CollectionChanged += StepRequirements_CollectionChanged;
                }
                return _stepRequirements;
            }
            private set
            {
                if (_stepRequirements != null)
                {
                    _stepRequirements.CollectionChanged -= StepRequirements_CollectionChanged;
                }
                _stepRequirements = value;
                if (_stepRequirements != null)
                {
                    _stepRequirements.CollectionChanged += StepRequirements_CollectionChanged;
                }
            }
        }

        private void StepRequirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepRequirement>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepVerification> _stepVerifications;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepVerification> StepVerifications
        {
            get
            {
                if (_stepVerifications == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVerifications - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepVerifications = new ObservableCollection<StepVerification>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepVerifications.Where(x => x.Step == this.StepId).ToList<StepVerification>();
                        _stepVerifications = new ObservableCollection<StepVerification>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepVerifications.CollectionChanged += StepVerifications_CollectionChanged;
                }
                return _stepVerifications;
            }
            private set
            {
                if (_stepVerifications != null)
                {
                    _stepVerifications.CollectionChanged -= StepVerifications_CollectionChanged;
                }
                _stepVerifications = value;
                if (_stepVerifications != null)
                {
                    _stepVerifications.CollectionChanged += StepVerifications_CollectionChanged;
                }
            }
        }

        private void StepVerifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepVerification>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<Rationale> _rationales;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<Rationale> Rationales
        {
            get
            {
                if (_rationales == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Rationales - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _rationales = new ObservableCollection<Rationale>();
                    }
                    else
                    {
                        var items = base.SoAContext.Rationales.Where(x => x.Step == this.StepId).ToList<Rationale>();
                        _rationales = new ObservableCollection<Rationale>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _rationales.CollectionChanged += Rationales_CollectionChanged;
                }
                return _rationales;
            }
            private set
            {
                if (_rationales != null)
                {
                    _rationales.CollectionChanged -= Rationales_CollectionChanged;
                }
                _rationales = value;
                if (_rationales != null)
                {
                    _rationales.CollectionChanged += Rationales_CollectionChanged;
                }
            }
        }

        private void Rationales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Rationale>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<Exception> _exceptions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<Exception> Exceptions
        {
            get
            {
                if (_exceptions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exceptions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _exceptions = new ObservableCollection<Exception>();
                    }
                    else
                    {
                        var items = base.SoAContext.Exceptions.Where(x => x.TriggerStep == this.StepId).ToList<Exception>();
                        _exceptions = new ObservableCollection<Exception>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _exceptions.CollectionChanged += Exceptions_CollectionChanged;
                }
                return _exceptions;
            }
            private set
            {
                if (_exceptions != null)
                {
                    _exceptions.CollectionChanged -= Exceptions_CollectionChanged;
                }
                _exceptions = value;
                if (_exceptions != null)
                {
                    _exceptions.CollectionChanged += Exceptions_CollectionChanged;
                }
            }
        }

        private void Exceptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Exception>())
                {
                    item.TriggerStep = this.StepId;
                }
            }
        }

        private ObservableCollection<KnowledgeFragment> _knowledgeFragments;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<KnowledgeFragment> KnowledgeFragments
        {
            get
            {
                if (_knowledgeFragments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeFragments - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeFragments.Where(x => x.Step == this.StepId).ToList<KnowledgeFragment>();
                        _knowledgeFragments = new ObservableCollection<KnowledgeFragment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
                return _knowledgeFragments;
            }
            private set
            {
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged -= KnowledgeFragments_CollectionChanged;
                }
                _knowledgeFragments = value;
                if (_knowledgeFragments != null)
                {
                    _knowledgeFragments.CollectionChanged += KnowledgeFragments_CollectionChanged;
                }
            }
        }

        private void KnowledgeFragments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeFragment>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<KnowledgeGap> _knowledgeGaps;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<KnowledgeGap> KnowledgeGaps
        {
            get
            {
                if (_knowledgeGaps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeGaps - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeGaps.Where(x => x.Step == this.StepId).ToList<KnowledgeGap>();
                        _knowledgeGaps = new ObservableCollection<KnowledgeGap>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
                return _knowledgeGaps;
            }
            private set
            {
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged -= KnowledgeGaps_CollectionChanged;
                }
                _knowledgeGaps = value;
                if (_knowledgeGaps != null)
                {
                    _knowledgeGaps.CollectionChanged += KnowledgeGaps_CollectionChanged;
                }
            }
        }

        private void KnowledgeGaps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeGap>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<FAQ> _fAQs;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<FAQ> FAQs
        {
            get
            {
                if (_fAQs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQs - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _fAQs = new ObservableCollection<FAQ>();
                    }
                    else
                    {
                        var items = base.SoAContext.FAQs.Where(x => x.Step == this.StepId).ToList<FAQ>();
                        _fAQs = new ObservableCollection<FAQ>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
                return _fAQs;
            }
            private set
            {
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged -= FAQs_CollectionChanged;
                }
                _fAQs = value;
                if (_fAQs != null)
                {
                    _fAQs.CollectionChanged += FAQs_CollectionChanged;
                }
            }
        }

        private void FAQs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FAQ>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<Explanation> _explanations;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<Explanation> Explanations
        {
            get
            {
                if (_explanations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Explanations - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _explanations = new ObservableCollection<Explanation>();
                    }
                    else
                    {
                        var items = base.SoAContext.Explanations.Where(x => x.Step == this.StepId).ToList<Explanation>();
                        _explanations = new ObservableCollection<Explanation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _explanations.CollectionChanged += Explanations_CollectionChanged;
                }
                return _explanations;
            }
            private set
            {
                if (_explanations != null)
                {
                    _explanations.CollectionChanged -= Explanations_CollectionChanged;
                }
                _explanations = value;
                if (_explanations != null)
                {
                    _explanations.CollectionChanged += Explanations_CollectionChanged;
                }
            }
        }

        private void Explanations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Explanation>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepExecution> _stepExecutions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepExecution> StepExecutions
        {
            get
            {
                if (_stepExecutions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepExecutions = new ObservableCollection<StepExecution>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepExecutions.Where(x => x.Step == this.StepId).ToList<StepExecution>();
                        _stepExecutions = new ObservableCollection<StepExecution>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepExecutions.CollectionChanged += StepExecutions_CollectionChanged;
                }
                return _stepExecutions;
            }
            private set
            {
                if (_stepExecutions != null)
                {
                    _stepExecutions.CollectionChanged -= StepExecutions_CollectionChanged;
                }
                _stepExecutions = value;
                if (_stepExecutions != null)
                {
                    _stepExecutions.CollectionChanged += StepExecutions_CollectionChanged;
                }
            }
        }

        private void StepExecutions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepExecution>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<OperationalBinding> _operationalBindings;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<OperationalBinding> OperationalBindings
        {
            get
            {
                if (_operationalBindings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OperationalBindings - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _operationalBindings = new ObservableCollection<OperationalBinding>();
                    }
                    else
                    {
                        var items = base.SoAContext.OperationalBindings.Where(x => x.Step == this.StepId).ToList<OperationalBinding>();
                        _operationalBindings = new ObservableCollection<OperationalBinding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _operationalBindings.CollectionChanged += OperationalBindings_CollectionChanged;
                }
                return _operationalBindings;
            }
            private set
            {
                if (_operationalBindings != null)
                {
                    _operationalBindings.CollectionChanged -= OperationalBindings_CollectionChanged;
                }
                _operationalBindings = value;
                if (_operationalBindings != null)
                {
                    _operationalBindings.CollectionChanged += OperationalBindings_CollectionChanged;
                }
            }
        }

        private void OperationalBindings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<OperationalBinding>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<AuthorityBoundary> _authorityBoundaries;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<AuthorityBoundary> AuthorityBoundaries
        {
            get
            {
                if (_authorityBoundaries == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AuthorityBoundaries - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>();
                    }
                    else
                    {
                        var items = base.SoAContext.AuthorityBoundaries.Where(x => x.Step == this.StepId).ToList<AuthorityBoundary>();
                        _authorityBoundaries = new ObservableCollection<AuthorityBoundary>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
                return _authorityBoundaries;
            }
            private set
            {
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged -= AuthorityBoundaries_CollectionChanged;
                }
                _authorityBoundaries = value;
                if (_authorityBoundaries != null)
                {
                    _authorityBoundaries.CollectionChanged += AuthorityBoundaries_CollectionChanged;
                }
            }
        }

        private void AuthorityBoundaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AuthorityBoundary>())
                {
                    item.Step = this.StepId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Role;
            _ = this.FromStepStepTransitions;
            _ = this.ToStepStepTransitions;
            _ = this.StepActions;
            _ = this.StepFunctions;
            _ = this.StepTools;
            _ = this.StepRequirements;
            _ = this.StepVerifications;
            _ = this.Rationales;
            _ = this.Exceptions;
            _ = this.KnowledgeFragments;
            _ = this.KnowledgeGaps;
            _ = this.FAQs;
            _ = this.Explanations;
            _ = this.StepExecutions;
            _ = this.OperationalBindings;
            _ = this.AuthorityBoundaries;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
