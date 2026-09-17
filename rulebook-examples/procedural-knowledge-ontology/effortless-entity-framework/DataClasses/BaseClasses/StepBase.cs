
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
        public int? MinRepetitions { get; set; }
        public int? MaxRepetitions { get; set; }
        public string? Description { get; set; }
        // Formula ChildStepCount (rulebook: =COUNTIFS(Steps!{{ParentStep}}, Steps!{{StepId}}))
        [NotMapped]
        public int? ChildStepCount
        {
            get => F.AsInt(F.Memo(this, "ChildStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<Step>(base.SoAContext, "Steps", __c => __c.Steps), __r => F.CritField(F.Of(__r.ParentStep), F.Of(this.StepId))))))); set { }
        }

        // Formula ParentStepKind (rulebook: =INDEX(Steps!{{StepKind}}, MATCH({{ParentStep}}, Steps!{{StepId}}, 0)))
        [NotMapped]
        public string? ParentStepKind
        {
            get => F.AsString(F.Memo(this, "ParentStepKind", () => F.Lookup<Step>(this, "Steps", "StepId", __c => __c.Steps, __r => F.Of(__r.StepId), F.Of(this.ParentStep), __r => F.Of(__r.StepKind), () => F.Of(new Step().StepKind)))); set { }
        }

        // Formula IsCompositeWithoutChildren (rulebook: =AND({{StepKind}} = "MultiStep", {{ChildStepCount}} = 0))
        [NotMapped]
        public bool? IsCompositeWithoutChildren
        {
            get => F.AsBool(F.Memo(this, "IsCompositeWithoutChildren", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.StepKind)), F.S("MultiStep"))), F.Bool3(F.Eq(F.Of(this.ChildStepCount), F.I(0)))))); set { }
        }

        // Formula PreconditionCount (rulebook: =COUNTIFS(StepConditions!{{PreconditionStepKey}}, {{StepId}}))
        [NotMapped]
        public int? PreconditionCount
        {
            get => F.AsInt(F.Memo(this, "PreconditionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCondition>(base.SoAContext, "StepConditions", __c => __c.StepConditions), __r => F.CritField(F.Of(__r.PreconditionStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula PostconditionCount (rulebook: =COUNTIFS(StepConditions!{{PostconditionStepKey}}, {{StepId}}))
        [NotMapped]
        public int? PostconditionCount
        {
            get => F.AsInt(F.Memo(this, "PostconditionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCondition>(base.SoAContext, "StepConditions", __c => __c.StepConditions), __r => F.CritField(F.Of(__r.PostconditionStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula InvariantCount (rulebook: =COUNTIFS(StepConditions!{{InvariantStepKey}}, {{StepId}}))
        [NotMapped]
        public int? InvariantCount
        {
            get => F.AsInt(F.Memo(this, "InvariantCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCondition>(base.SoAContext, "StepConditions", __c => __c.StepConditions), __r => F.CritField(F.Of(__r.InvariantStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula SafetyCriticalConditionCount (rulebook: =COUNTIFS(StepConditions!{{SafetyCriticalStepKey}}, {{StepId}}))
        [NotMapped]
        public int? SafetyCriticalConditionCount
        {
            get => F.AsInt(F.Memo(this, "SafetyCriticalConditionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCondition>(base.SoAContext, "StepConditions", __c => __c.StepConditions), __r => F.CritField(F.Of(__r.SafetyCriticalStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula FailureModeCount (rulebook: =COUNTIFS(FailureModes!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? FailureModeCount
        {
            get => F.AsInt(F.Memo(this, "FailureModeCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<FailureMode>(base.SoAContext, "FailureModes", __c => __c.FailureModes), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula CueCount (rulebook: =COUNTIFS(StepCues!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? CueCount
        {
            get => F.AsInt(F.Memo(this, "CueCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCue>(base.SoAContext, "StepCues", __c => __c.StepCues), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula DangerCueCount (rulebook: =COUNTIFS(StepCues!{{DangerCueStepKey}}, {{StepId}}))
        [NotMapped]
        public int? DangerCueCount
        {
            get => F.AsInt(F.Memo(this, "DangerCueCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCue>(base.SoAContext, "StepCues", __c => __c.StepCues), __r => F.CritField(F.Of(__r.DangerCueStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula DecisionPointCount (rulebook: =COUNTIFS(DecisionPoints!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? DecisionPointCount
        {
            get => F.AsInt(F.Memo(this, "DecisionPointCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<DecisionPoint>(base.SoAContext, "DecisionPoints", __c => __c.DecisionPoints), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula KnowledgeFragmentCount (rulebook: =COUNTIFS(KnowledgeFragments!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? KnowledgeFragmentCount
        {
            get => F.AsInt(F.Memo(this, "KnowledgeFragmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeFragment>(base.SoAContext, "KnowledgeFragments", __c => __c.KnowledgeFragments), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula HasInstructionOnly (rulebook: =AND(({{PreconditionCount}} + {{PostconditionCount}} + {{InvariantCount}}) = 0, {{CueCount}} = 0, {{FailureModeCount}} = 0, {{DecisionPointCount}} = 0, {{KnowledgeFragmentCount}} = 0))
        [NotMapped]
        public bool? HasInstructionOnly
        {
            get => F.AsBool(F.Memo(this, "HasInstructionOnly", () => F.And(F.Bool3(F.Eq(F.Add(F.Add(F.Of(this.PreconditionCount), F.Of(this.PostconditionCount)), F.Of(this.InvariantCount)), F.I(0))), F.Bool3(F.Eq(F.Of(this.CueCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.FailureModeCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.DecisionPointCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.KnowledgeFragmentCount), F.I(0)))))); set { }
        }

        // Formula InputVariableCount (rulebook: =COUNTIFS(StepVariables!{{InputStepKey}}, {{StepId}}))
        [NotMapped]
        public int? InputVariableCount
        {
            get => F.AsInt(F.Memo(this, "InputVariableCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepVariable>(base.SoAContext, "StepVariables", __c => __c.StepVariables), __r => F.CritField(F.Of(__r.InputStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula OutputVariableCount (rulebook: =COUNTIFS(StepVariables!{{OutputStepKey}}, {{StepId}}))
        [NotMapped]
        public int? OutputVariableCount
        {
            get => F.AsInt(F.Memo(this, "OutputVariableCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepVariable>(base.SoAContext, "StepVariables", __c => __c.StepVariables), __r => F.CritField(F.Of(__r.OutputStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula RequiredLockCount (rulebook: =COUNTIFS(StepLockRequirements!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? RequiredLockCount
        {
            get => F.AsInt(F.Memo(this, "RequiredLockCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepLockRequirement>(base.SoAContext, "StepLockRequirements", __c => __c.StepLockRequirements), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula RequiredProtectiveEquipmentCount (rulebook: =COUNTIFS(StepProtectiveEquipment!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? RequiredProtectiveEquipmentCount
        {
            get => F.AsInt(F.Memo(this, "RequiredProtectiveEquipmentCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepProtectiveEquipment>(base.SoAContext, "StepProtectiveEquipment", __c => __c.StepProtectiveEquipment), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula IsIsolationWithoutLock (rulebook: =AND({{IsolatesEnergySource}} <> "", {{RequiredLockCount}} = 0))
        [NotMapped]
        public bool? IsIsolationWithoutLock
        {
            get => F.AsBool(F.Memo(this, "IsIsolationWithoutLock", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.IsolatesEnergySource))), F.Bool3(F.Eq(F.Of(this.RequiredLockCount), F.I(0)))))); set { }
        }

        // Formula ReferencedResourceCount (rulebook: =COUNTIFS(StepResources!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? ReferencedResourceCount
        {
            get => F.AsInt(F.Memo(this, "ReferencedResourceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepResource>(base.SoAContext, "StepResources", __c => __c.StepResources), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula UntypedVersionKey (rulebook: =IF({{SemanticTypeIri}} = "", {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? UntypedVersionKey
        {
            get => F.AsString(F.Memo(this, "UntypedVersionKey", () => (F.Truthy(F.Bool3(F.IsBlank(F.Of(this.SemanticTypeIri)))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula IsAccountableToSoftware (rulebook: =AND({{AssignedAgentKind}} <> "", {{AssignedAgentKind}} <> "Human"))
        [NotMapped]
        public bool? IsAccountableToSoftware
        {
            get => F.AsBool(F.Memo(this, "IsAccountableToSoftware", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AssignedAgentKind))), F.Bool3(F.Ne(F.Of(this.AssignedAgentKind), F.S("Human")))))); set { }
        }

        // Formula HasDownstreamSteps (rulebook: ={{ReachableStepCount}} > 0)
        [NotMapped]
        public bool? HasDownstreamSteps
        {
            get => F.AsBool(F.Memo(this, "HasDownstreamSteps", () => F.Cmp(F.Of(this.ReachableStepCount), ">", F.I(0)))); set { }
        }

        // Formula IncompletenessCueCount (rulebook: =COUNTIFS(StepCues!{{IncompleteCueStepKey}}, {{StepId}}))
        [NotMapped]
        public int? IncompletenessCueCount
        {
            get => F.AsInt(F.Memo(this, "IncompletenessCueCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCue>(base.SoAContext, "StepCues", __c => __c.StepCues), __r => F.CritField(F.Of(__r.IncompleteCueStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula HasIncompletenessCue (rulebook: ={{IncompletenessCueCount}} > 0)
        [NotMapped]
        public bool? HasIncompletenessCue
        {
            get => F.AsBool(F.Memo(this, "HasIncompletenessCue", () => F.Cmp(F.Of(this.IncompletenessCueCount), ">", F.I(0)))); set { }
        }

        // Formula HasPostcondition (rulebook: ={{PostconditionCount}} > 0)
        [NotMapped]
        public bool? HasPostcondition
        {
            get => F.AsBool(F.Memo(this, "HasPostcondition", () => F.Cmp(F.Of(this.PostconditionCount), ">", F.I(0)))); set { }
        }

        // Formula AccountableAgent (rulebook: =INDEX(Roles!{{CurrentAgent}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? AccountableAgent
        {
            get => F.AsString(F.Memo(this, "AccountableAgent", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.CurrentAgent), () => F.Of(new Role().CurrentAgent)))); set { }
        }

        // Formula HasNoAccountableAgent (rulebook: ={{AccountableAgent}} = "")
        [NotMapped]
        public bool? HasNoAccountableAgent
        {
            get => F.AsBool(F.Memo(this, "HasNoAccountableAgent", () => F.IsBlank(F.Of(this.AccountableAgent)))); set { }
        }

        // Formula ConditionlessVersionKey (rulebook: =IF({{PreconditionCount}} = 0, {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? ConditionlessVersionKey
        {
            get => F.AsString(F.Memo(this, "ConditionlessVersionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.PreconditionCount), F.I(0)))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula PrerequisiteDownstreamCount (rulebook: =COUNTIFS(vw_step_transitions_closure!{{FromId}}, Steps!{{StepId}}, vw_step_transitions_closure!{{ToId}}, {{PrerequisiteStep}}))
        [NotMapped]
        public int? PrerequisiteDownstreamCount
        {
            get => F.AsInt(F.Memo(this, "PrerequisiteDownstreamCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Closure<StepTransition>(base.SoAContext, "vw_step_transitions_closure", "StepTransitions", __c => __c.StepTransitions, __e => F.Of(__e.FromStep), __e => F.Of(__e.ToStep), __e => true), __r => F.CritField(F.Of(__r.FromId), F.Of(this.StepId)) && F.CritField(F.Of(__r.ToId), F.Of(this.PrerequisiteStep))))))); set { }
        }

        // Formula PrerequisiteIsDownstream (rulebook: =AND({{PrerequisiteStep}} <> "", {{PrerequisiteDownstreamCount}} > 0))
        [NotMapped]
        public bool? PrerequisiteIsDownstream
        {
            get => F.AsBool(F.Memo(this, "PrerequisiteIsDownstream", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PrerequisiteStep))), F.Bool3(F.Cmp(F.Of(this.PrerequisiteDownstreamCount), ">", F.I(0)))))); set { }
        }

        // Formula StatesOperationalKnowledge (rulebook: =AND({{PrerequisiteStep}} <> "", {{PreconditionCount}} > 0, {{ReachedFromStepCount}} > 0))
        [NotMapped]
        public bool? StatesOperationalKnowledge
        {
            get => F.AsBool(F.Memo(this, "StatesOperationalKnowledge", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.PrerequisiteStep))), F.Bool3(F.Cmp(F.Of(this.PreconditionCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.ReachedFromStepCount), ">", F.I(0)))))); set { }
        }

        // Formula BottleneckAllocationCount (rulebook: =COUNTIFS(TacticalResourceAllocations!{{Step}}, {{StepId}}, TacticalResourceAllocations!{{IsBottleneck}}, TRUE))
        [NotMapped]
        public int? BottleneckAllocationCount
        {
            get => F.AsInt(F.Memo(this, "BottleneckAllocationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<TacticalResourceAllocation>(base.SoAContext, "TacticalResourceAllocations", __c => __c.TacticalResourceAllocations), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.IsBottleneck), F.B(true))))))); set { }
        }

        // Formula IsBottleneckStep (rulebook: ={{BottleneckAllocationCount}} > 0)
        [NotMapped]
        public bool? IsBottleneckStep
        {
            get => F.AsBool(F.Memo(this, "IsBottleneckStep", () => F.Cmp(F.Of(this.BottleneckAllocationCount), ">", F.I(0)))); set { }
        }

        public string? DetailLevel { get; set; }
        // Formula CoarseTopLevelVersionKey (rulebook: =IF(AND({{ParentStep}} = "", {{DetailLevel}} = "Activity"), {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? CoarseTopLevelVersionKey
        {
            get => F.AsString(F.Memo(this, "CoarseTopLevelVersionKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.IsBlank(F.Of(this.ParentStep))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DetailLevel)), F.S("Activity")))))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula FineTopLevelVersionKey (rulebook: =IF(AND({{ParentStep}} = "", {{DetailLevel}} = "Action"), {{ProcedureVersion}}, ""))
        [NotMapped]
        public string? FineTopLevelVersionKey
        {
            get => F.AsString(F.Memo(this, "FineTopLevelVersionKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.IsBlank(F.Of(this.ParentStep))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DetailLevel)), F.S("Action")))))) ? F.Of(this.ProcedureVersion) : F.S("")))); set { }
        }

        // Formula ContextSensitivityCount (rulebook: =COUNTIFS(StepContextSensitivities!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? ContextSensitivityCount
        {
            get => F.AsInt(F.Memo(this, "ContextSensitivityCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepContextSensitivity>(base.SoAContext, "StepContextSensitivities", __c => __c.StepContextSensitivities), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula UnscopedSensitivityCount (rulebook: =COUNTIFS(StepContextSensitivities!{{UnscopedStepKey}}, {{StepId}}))
        [NotMapped]
        public int? UnscopedSensitivityCount
        {
            get => F.AsInt(F.Memo(this, "UnscopedSensitivityCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepContextSensitivity>(base.SoAContext, "StepContextSensitivities", __c => __c.StepContextSensitivities), __r => F.CritField(F.Of(__r.UnscopedStepKey), F.Of(this.StepId))))))); set { }
        }

        // Formula IsContextSensitiveButUnscoped (rulebook: =AND({{ContextSensitivityCount}} > 0, {{UnscopedSensitivityCount}} > 0))
        [NotMapped]
        public bool? IsContextSensitiveButUnscoped
        {
            get => F.AsBool(F.Memo(this, "IsContextSensitiveButUnscoped", () => F.And(F.Bool3(F.Cmp(F.Of(this.ContextSensitivityCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.UnscopedSensitivityCount), ">", F.I(0)))))); set { }
        }

        // Formula VersionProcedure (rulebook: =INDEX(ProcedureVersions!{{Procedure}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? VersionProcedure
        {
            get => F.AsString(F.Memo(this, "VersionProcedure", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.Procedure), () => F.Of(new ProcedureVersion().Procedure)))); set { }
        }

        // Formula AssignedRoleDoesComplianceReview (rulebook: =INDEX(Roles!{{HasComplianceReviewCapability}}, MATCH({{AssignedRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public bool? AssignedRoleDoesComplianceReview
        {
            get => F.AsBool(F.Memo(this, "AssignedRoleDoesComplianceReview", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.AssignedRole), __r => F.Of(__r.HasComplianceReviewCapability), () => F.Of(new Role().HasComplianceReviewCapability)))); set { }
        }

        // Formula ComplianceReviewProcedureKey (rulebook: =IF({{AssignedRoleDoesComplianceReview}}, {{VersionProcedure}}, ""))
        [NotMapped]
        public string? ComplianceReviewProcedureKey
        {
            get => F.AsString(F.Memo(this, "ComplianceReviewProcedureKey", () => (F.Truthy(F.Bool3(F.Of(this.AssignedRoleDoesComplianceReview))) ? F.Of(this.VersionProcedure) : F.S("")))); set { }
        }

        // Formula StepProcedureType (rulebook: =INDEX(ProcedureVersions!{{ProcedureTypeOfVersion}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public string? StepProcedureType
        {
            get => F.AsString(F.Memo(this, "StepProcedureType", () => F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.ProcedureTypeOfVersion), () => F.Of(new ProcedureVersion().ProcedureTypeOfVersion)))); set { }
        }

        // Formula IsReleaseApprovalGate (rulebook: =AND({{ControlKind}} = "Approval", {{StepProcedureType}} = "software-release"))
        [NotMapped]
        public bool? IsReleaseApprovalGate
        {
            get => F.AsBool(F.Memo(this, "IsReleaseApprovalGate", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ControlKind)), F.S("Approval"))), F.Bool3(F.Eq(F.Of(this.StepProcedureType), F.S("software-release")))))); set { }
        }

        // Formula RegulatoryRequirementCount (rulebook: =COUNTIFS(StepRequirements!{{Step}}, {{StepId}}, StepRequirements!{{RequirementIsRegulatory}}, TRUE))
        [NotMapped]
        public int? RegulatoryRequirementCount
        {
            get => F.AsInt(F.Memo(this, "RegulatoryRequirementCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepRequirement>(base.SoAContext, "StepRequirements", __c => __c.StepRequirements), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.RequirementIsRegulatory), F.B(true))))))); set { }
        }

        // Formula ToolFunctionCount (rulebook: =COUNTIFS(StepFunctions!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? ToolFunctionCount
        {
            get => F.AsInt(F.Memo(this, "ToolFunctionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepFunction>(base.SoAContext, "StepFunctions", __c => __c.StepFunctions), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula ParseableConditionCount (rulebook: =COUNTIFS(StepConditions!{{Step}}, {{StepId}}, StepConditions!{{IsMachineParseable}}, TRUE))
        [NotMapped]
        public int? ParseableConditionCount
        {
            get => F.AsInt(F.Memo(this, "ParseableConditionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepCondition>(base.SoAContext, "StepConditions", __c => __c.StepConditions), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.IsMachineParseable), F.B(true))))))); set { }
        }

        // Formula DmnDecisionCount (rulebook: =COUNTIFS(DecisionPoints!{{Step}}, {{StepId}}, DecisionPoints!{{IsDmnEncoded}}, TRUE))
        [NotMapped]
        public int? DmnDecisionCount
        {
            get => F.AsInt(F.Memo(this, "DmnDecisionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<DecisionPoint>(base.SoAContext, "DecisionPoints", __c => __c.DecisionPoints), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.IsDmnEncoded), F.B(true))))))); set { }
        }

        // Formula ToolUseRulesOnlyInProse (rulebook: =AND({{ToolFunctionCount}} > 0, {{ParseableConditionCount}} = 0, {{DmnDecisionCount}} = 0))
        [NotMapped]
        public bool? ToolUseRulesOnlyInProse
        {
            get => F.AsBool(F.Memo(this, "ToolUseRulesOnlyInProse", () => F.And(F.Bool3(F.Cmp(F.Of(this.ToolFunctionCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ParseableConditionCount), F.I(0))), F.Bool3(F.Eq(F.Of(this.DmnDecisionCount), F.I(0)))))); set { }
        }

        // Formula OpenOutdatedFlagCount (rulebook: =COUNTIFS(ModelAnnotations!{{Step}}, {{StepId}}, ModelAnnotations!{{IsOpenOutdatedFlag}}, TRUE))
        [NotMapped]
        public int? OpenOutdatedFlagCount
        {
            get => F.AsInt(F.Memo(this, "OpenOutdatedFlagCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelAnnotation>(base.SoAContext, "ModelAnnotations", __c => __c.ModelAnnotations), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.IsOpenOutdatedFlag), F.B(true))))))); set { }
        }

        // Formula HasReportedRealityMismatch (rulebook: ={{OpenOutdatedFlagCount}} > 0)
        [NotMapped]
        public bool? HasReportedRealityMismatch
        {
            get => F.AsBool(F.Memo(this, "HasReportedRealityMismatch", () => F.Cmp(F.Of(this.OpenOutdatedFlagCount), ">", F.I(0)))); set { }
        }

        // Formula DeviatedRunCount (rulebook: =COUNTIFS(StepExecutions!{{Step}}, {{StepId}}, StepExecutions!{{HasDeviation}}, TRUE))
        [NotMapped]
        public int? DeviatedRunCount
        {
            get => F.AsInt(F.Memo(this, "DeviatedRunCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepExecution>(base.SoAContext, "StepExecutions", __c => __c.StepExecutions), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.HasDeviation), F.B(true))))))); set { }
        }

        // Formula IsDriftedFromPractice (rulebook: =OR({{DeviatedRunCount}} >= 2, AND({{DeviatedRunCount}} > 0, {{OpenOutdatedFlagCount}} > 0)))
        [NotMapped]
        public bool? IsDriftedFromPractice
        {
            get => F.AsBool(F.Memo(this, "IsDriftedFromPractice", () => F.Or(F.Bool3(F.Cmp(F.Of(this.DeviatedRunCount), ">=", F.I(2))), F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.DeviatedRunCount), ">", F.I(0))), F.Bool3(F.Cmp(F.Of(this.OpenOutdatedFlagCount), ">", F.I(0)))))))); set { }
        }

        // Formula AiFailureCount (rulebook: =COUNTIFS(AssistantAnswers!{{ContextStep}}, {{StepId}}, AssistantAnswers!{{TaskOutcome}}, "Failed"))
        [NotMapped]
        public int? AiFailureCount
        {
            get => F.AsInt(F.Memo(this, "AiFailureCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AssistantAnswer>(base.SoAContext, "AssistantAnswers", __c => __c.AssistantAnswers), __r => F.CritField(F.Of(__r.ContextStep), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.TaskOutcome), F.S("Failed"))))))); set { }
        }

        // Formula IsAiFailurePoint (rulebook: ={{AiFailureCount}} > 0)
        [NotMapped]
        public bool? IsAiFailurePoint
        {
            get => F.AsBool(F.Memo(this, "IsAiFailurePoint", () => F.Cmp(F.Of(this.AiFailureCount), ">", F.I(0)))); set { }
        }

        // Formula AiArtifactInputCount (rulebook: =COUNTIFS(StepVariables!{{Step}}, {{StepId}}, StepVariables!{{IsInputFromAiArtifact}}, TRUE))
        [NotMapped]
        public int? AiArtifactInputCount
        {
            get => F.AsInt(F.Memo(this, "AiArtifactInputCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<StepVariable>(base.SoAContext, "StepVariables", __c => __c.StepVariables), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.IsInputFromAiArtifact), F.B(true))))))); set { }
        }

        // Formula ConsumesAiAgentArtifact (rulebook: ={{AiArtifactInputCount}} > 0)
        [NotMapped]
        public bool? ConsumesAiAgentArtifact
        {
            get => F.AsBool(F.Memo(this, "ConsumesAiAgentArtifact", () => F.Cmp(F.Of(this.AiArtifactInputCount), ">", F.I(0)))); set { }
        }

        // Formula CollectionEvidenceCount (rulebook: =COUNTIFS(KnowledgeTraces!{{Step}}, {{StepId}}))
        [NotMapped]
        public int? CollectionEvidenceCount
        {
            get => F.AsInt(F.Memo(this, "CollectionEvidenceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId))))))); set { }
        }

        // Formula HasCollectionEvidence (rulebook: ={{CollectionEvidenceCount}} > 0)
        [NotMapped]
        public bool? HasCollectionEvidence
        {
            get => F.AsBool(F.Memo(this, "HasCollectionEvidence", () => F.Cmp(F.Of(this.CollectionEvidenceCount), ">", F.I(0)))); set { }
        }

        // Formula ActivityOriginTraceCount (rulebook: =COUNTIFS(KnowledgeTraces!{{Step}}, {{StepId}}, KnowledgeTraces!{{TargetKind}}, "ActivityDefinition", KnowledgeTraces!{{TraceRole}}, "Origin"))
        [NotMapped]
        public int? ActivityOriginTraceCount
        {
            get => F.AsInt(F.Memo(this, "ActivityOriginTraceCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.TargetKind), F.S("ActivityDefinition")) && F.CritLiteral(F.Of(__r.TraceRole), F.S("Origin"))))))); set { }
        }

        // Formula VersionModelTraceCount (rulebook: =INDEX(ProcedureVersions!{{ProcessModelTraceCount}}, MATCH({{ProcedureVersion}}, ProcedureVersions!{{ProcedureVersionId}}, 0)))
        [NotMapped]
        public int? VersionModelTraceCount
        {
            get => F.AsInt(F.Memo(this, "VersionModelTraceCount", () => F.Integer(F.Lookup<ProcedureVersion>(this, "ProcedureVersions", "ProcedureVersionId", __c => __c.ProcedureVersions, __r => F.Of(__r.ProcedureVersionId), F.Of(this.ProcedureVersion), __r => F.Of(__r.ProcessModelTraceCount), () => F.Of(new ProcedureVersion().ProcessModelTraceCount))))); set { }
        }

        // Formula IsUntracedActivityInTracedModel (rulebook: =AND({{VersionModelTraceCount}} > 0, {{ActivityOriginTraceCount}} = 0))
        [NotMapped]
        public bool? IsUntracedActivityInTracedModel
        {
            get => F.AsBool(F.Memo(this, "IsUntracedActivityInTracedModel", () => F.And(F.Bool3(F.Cmp(F.Of(this.VersionModelTraceCount), ">", F.I(0))), F.Bool3(F.Eq(F.Of(this.ActivityOriginTraceCount), F.I(0)))))); set { }
        }

        // Formula ElicitedValidationCount (rulebook: =COUNTIFS(KnowledgeTraces!{{Step}}, {{StepId}}, KnowledgeTraces!{{TraceRole}}, "Validates", KnowledgeTraces!{{SourceIsPeopleCapture}}, TRUE))
        [NotMapped]
        public int? ElicitedValidationCount
        {
            get => F.AsInt(F.Memo(this, "ElicitedValidationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.TraceRole), F.S("Validates")) && F.CritLiteral(F.Of(__r.SourceIsPeopleCapture), F.B(true))))))); set { }
        }

        // Formula ElicitedExtensionCount (rulebook: =COUNTIFS(KnowledgeTraces!{{Step}}, {{StepId}}, KnowledgeTraces!{{TraceRole}}, "Extends", KnowledgeTraces!{{SourceIsPeopleCapture}}, TRUE))
        [NotMapped]
        public int? ElicitedExtensionCount
        {
            get => F.AsInt(F.Memo(this, "ElicitedExtensionCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<KnowledgeTrace>(base.SoAContext, "KnowledgeTraces", __c => __c.KnowledgeTraces), __r => F.CritField(F.Of(__r.Step), F.Of(this.StepId)) && F.CritLiteral(F.Of(__r.TraceRole), F.S("Extends")) && F.CritLiteral(F.Of(__r.SourceIsPeopleCapture), F.B(true))))))); set { }
        }

        // Formula DownstreamArtifactStepCount (rulebook: =COUNTIFS(vw_artifact_handoffs_closure!{{FromId}}, Steps!{{StepId}}))
        [NotMapped]
        public int? DownstreamArtifactStepCount
        {
            get => F.AsInt(F.Memo(this, "DownstreamArtifactStepCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Closure<ArtifactHandoff>(base.SoAContext, "vw_artifact_handoffs_closure", "ArtifactHandoffs", __c => __c.ArtifactHandoffs, __e => F.Of(__e.FromStep), __e => F.Of(__e.ToStep), __e => true), __r => F.CritField(F.Of(__r.FromId), F.Of(this.StepId))))))); set { }
        }

        public bool? IsDeclaredFirstStep { get; set; }
        public bool? IsDeclaredFallbackStep { get; set; }
        // Formula DeclaredFirstStepKey (rulebook: =IF({{IsDeclaredFirstStep}}, {{StepId}}, ""))
        [NotMapped]
        public string? DeclaredFirstStepKey
        {
            get => F.AsString(F.Memo(this, "DeclaredFirstStepKey", () => (F.Truthy(F.IsTrueV(F.Of(this.IsDeclaredFirstStep))) ? F.Of(this.StepId) : F.S("")))); set { }
        }

        // Formula DeclaredFallbackStepKey (rulebook: =IF({{IsDeclaredFallbackStep}}, {{StepId}}, ""))
        [NotMapped]
        public string? DeclaredFallbackStepKey
        {
            get => F.AsString(F.Memo(this, "DeclaredFallbackStepKey", () => (F.Truthy(F.IsTrueV(F.Of(this.IsDeclaredFallbackStep))) ? F.Of(this.StepId) : F.S("")))); set { }
        }


        public string? ProcedureVersion { get; set; }
        public string? AssignedRole { get; set; }
        public string? ParentStep { get; set; }
        public string? FirstChildStep { get; set; }
        public string? VerifiesStep { get; set; }
        public string? RemedyForError { get; set; }
        public string? CallsProcedure { get; set; }
        public string? IsolatesEnergySource { get; set; }
        public string? PrerequisiteStep { get; set; }
        public string? Stage { get; set; }

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

        private Step _step;

        [ForeignKey("ParentStep")]
        public virtual Step Step
        {
            get
            {
                if (_step == null && !string.IsNullOrEmpty(ParentStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Step - no database context is set. ParentStep: " + ParentStep + ".");
                        }
                        return null;
                    }
                    _step = base.SoAContext.Steps.Find(ParentStep);
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
                        ParentStep = _step.StepId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("FirstChildStep")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(FirstChildStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. FirstChildStep: " + FirstChildStep + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(FirstChildStep);
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
                        FirstChildStep = _stepRef.StepId;
                    }
                }
            }
        }

        private Step _stepRefRef;

        [ForeignKey("VerifiesStep")]
        public virtual Step StepRefRef
        {
            get
            {
                if (_stepRefRef == null && !string.IsNullOrEmpty(VerifiesStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRefRef - no database context is set. VerifiesStep: " + VerifiesStep + ".");
                        }
                        return null;
                    }
                    _stepRefRef = base.SoAContext.Steps.Find(VerifiesStep);
                    if (_stepRefRef != null)
                    {
                        base.SoAContext.Attach(_stepRefRef);
                    }
                }
                return _stepRefRef;
            }
            set
            {
                if (_stepRefRef != value)
                {
                    _stepRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRefRef != null)
                    {
                        VerifiesStep = _stepRefRef.StepId;
                    }
                }
            }
        }

        private Error _error;

        [ForeignKey("RemedyForError")]
        public virtual Error Error
        {
            get
            {
                if (_error == null && !string.IsNullOrEmpty(RemedyForError))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Error - no database context is set. RemedyForError: " + RemedyForError + ".");
                        }
                        return null;
                    }
                    _error = base.SoAContext.Errors.Find(RemedyForError);
                    if (_error != null)
                    {
                        base.SoAContext.Attach(_error);
                    }
                }
                return _error;
            }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_error != null)
                    {
                        RemedyForError = _error.ErrorId;
                    }
                }
            }
        }

        private Procedure _procedure;

        [ForeignKey("CallsProcedure")]
        public virtual Procedure Procedure
        {
            get
            {
                if (_procedure == null && !string.IsNullOrEmpty(CallsProcedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Procedure - no database context is set. CallsProcedure: " + CallsProcedure + ".");
                        }
                        return null;
                    }
                    _procedure = base.SoAContext.Procedures.Find(CallsProcedure);
                    if (_procedure != null)
                    {
                        base.SoAContext.Attach(_procedure);
                    }
                }
                return _procedure;
            }
            set
            {
                if (_procedure != value)
                {
                    _procedure = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedure != null)
                    {
                        CallsProcedure = _procedure.ProcedureId;
                    }
                }
            }
        }

        private EnergySource _energySource;

        [ForeignKey("IsolatesEnergySource")]
        public virtual EnergySource EnergySource
        {
            get
            {
                if (_energySource == null && !string.IsNullOrEmpty(IsolatesEnergySource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access EnergySource - no database context is set. IsolatesEnergySource: " + IsolatesEnergySource + ".");
                        }
                        return null;
                    }
                    _energySource = base.SoAContext.EnergySources.Find(IsolatesEnergySource);
                    if (_energySource != null)
                    {
                        base.SoAContext.Attach(_energySource);
                    }
                }
                return _energySource;
            }
            set
            {
                if (_energySource != value)
                {
                    _energySource = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_energySource != null)
                    {
                        IsolatesEnergySource = _energySource.EnergySourceId;
                    }
                }
            }
        }

        private Step _stepRefRefRef;

        [ForeignKey("PrerequisiteStep")]
        public virtual Step StepRefRefRef
        {
            get
            {
                if (_stepRefRefRef == null && !string.IsNullOrEmpty(PrerequisiteStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRefRefRef - no database context is set. PrerequisiteStep: " + PrerequisiteStep + ".");
                        }
                        return null;
                    }
                    _stepRefRefRef = base.SoAContext.Steps.Find(PrerequisiteStep);
                    if (_stepRefRefRef != null)
                    {
                        base.SoAContext.Attach(_stepRefRefRef);
                    }
                }
                return _stepRefRefRef;
            }
            set
            {
                if (_stepRefRefRef != value)
                {
                    _stepRefRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRefRefRef != null)
                    {
                        PrerequisiteStep = _stepRefRefRef.StepId;
                    }
                }
            }
        }

        private ProcessStage _processStage;

        [ForeignKey("Stage")]
        public virtual ProcessStage ProcessStage
        {
            get
            {
                if (_processStage == null && !string.IsNullOrEmpty(Stage))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessStage - no database context is set. Stage: " + Stage + ".");
                        }
                        return null;
                    }
                    _processStage = base.SoAContext.ProcessStages.Find(Stage);
                    if (_processStage != null)
                    {
                        base.SoAContext.Attach(_processStage);
                    }
                }
                return _processStage;
            }
            set
            {
                if (_processStage != value)
                {
                    _processStage = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_processStage != null)
                    {
                        Stage = _processStage.ProcessStageId;
                    }
                }
            }
        }

        private ObservableCollection<Step> _parentStepSteps;

        [InverseProperty("Step")]
        public virtual ObservableCollection<Step> ParentStepSteps
        {
            get
            {
                if (_parentStepSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ParentStepSteps - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _parentStepSteps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.ParentStep == this.StepId).ToList<Step>();
                        _parentStepSteps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _parentStepSteps.CollectionChanged += ParentStepSteps_CollectionChanged;
                }
                return _parentStepSteps;
            }
            private set
            {
                if (_parentStepSteps != null)
                {
                    _parentStepSteps.CollectionChanged -= ParentStepSteps_CollectionChanged;
                }
                _parentStepSteps = value;
                if (_parentStepSteps != null)
                {
                    _parentStepSteps.CollectionChanged += ParentStepSteps_CollectionChanged;
                }
            }
        }

        private void ParentStepSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.ParentStep = this.StepId;
                }
            }
        }

        private ObservableCollection<Step> _firstChildStepSteps;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<Step> FirstChildStepSteps
        {
            get
            {
                if (_firstChildStepSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FirstChildStepSteps - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _firstChildStepSteps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.FirstChildStep == this.StepId).ToList<Step>();
                        _firstChildStepSteps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _firstChildStepSteps.CollectionChanged += FirstChildStepSteps_CollectionChanged;
                }
                return _firstChildStepSteps;
            }
            private set
            {
                if (_firstChildStepSteps != null)
                {
                    _firstChildStepSteps.CollectionChanged -= FirstChildStepSteps_CollectionChanged;
                }
                _firstChildStepSteps = value;
                if (_firstChildStepSteps != null)
                {
                    _firstChildStepSteps.CollectionChanged += FirstChildStepSteps_CollectionChanged;
                }
            }
        }

        private void FirstChildStepSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.FirstChildStep = this.StepId;
                }
            }
        }

        private ObservableCollection<Step> _verifiesStepSteps;

        [InverseProperty("StepRefRef")]
        public virtual ObservableCollection<Step> VerifiesStepSteps
        {
            get
            {
                if (_verifiesStepSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VerifiesStepSteps - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _verifiesStepSteps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.VerifiesStep == this.StepId).ToList<Step>();
                        _verifiesStepSteps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _verifiesStepSteps.CollectionChanged += VerifiesStepSteps_CollectionChanged;
                }
                return _verifiesStepSteps;
            }
            private set
            {
                if (_verifiesStepSteps != null)
                {
                    _verifiesStepSteps.CollectionChanged -= VerifiesStepSteps_CollectionChanged;
                }
                _verifiesStepSteps = value;
                if (_verifiesStepSteps != null)
                {
                    _verifiesStepSteps.CollectionChanged += VerifiesStepSteps_CollectionChanged;
                }
            }
        }

        private void VerifiesStepSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.VerifiesStep = this.StepId;
                }
            }
        }

        private ObservableCollection<Step> _prerequisiteStepSteps;

        [InverseProperty("StepRefRefRef")]
        public virtual ObservableCollection<Step> PrerequisiteStepSteps
        {
            get
            {
                if (_prerequisiteStepSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PrerequisiteStepSteps - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _prerequisiteStepSteps = new ObservableCollection<Step>();
                    }
                    else
                    {
                        var items = base.SoAContext.Steps.Where(x => x.PrerequisiteStep == this.StepId).ToList<Step>();
                        _prerequisiteStepSteps = new ObservableCollection<Step>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _prerequisiteStepSteps.CollectionChanged += PrerequisiteStepSteps_CollectionChanged;
                }
                return _prerequisiteStepSteps;
            }
            private set
            {
                if (_prerequisiteStepSteps != null)
                {
                    _prerequisiteStepSteps.CollectionChanged -= PrerequisiteStepSteps_CollectionChanged;
                }
                _prerequisiteStepSteps = value;
                if (_prerequisiteStepSteps != null)
                {
                    _prerequisiteStepSteps.CollectionChanged += PrerequisiteStepSteps_CollectionChanged;
                }
            }
        }

        private void PrerequisiteStepSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Step>())
                {
                    item.PrerequisiteStep = this.StepId;
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

        private ObservableCollection<StepLockRequirement> _stepLockRequirements;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepLockRequirement> StepLockRequirements
        {
            get
            {
                if (_stepLockRequirements == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepLockRequirements - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepLockRequirements = new ObservableCollection<StepLockRequirement>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepLockRequirements.Where(x => x.Step == this.StepId).ToList<StepLockRequirement>();
                        _stepLockRequirements = new ObservableCollection<StepLockRequirement>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepLockRequirements.CollectionChanged += StepLockRequirements_CollectionChanged;
                }
                return _stepLockRequirements;
            }
            private set
            {
                if (_stepLockRequirements != null)
                {
                    _stepLockRequirements.CollectionChanged -= StepLockRequirements_CollectionChanged;
                }
                _stepLockRequirements = value;
                if (_stepLockRequirements != null)
                {
                    _stepLockRequirements.CollectionChanged += StepLockRequirements_CollectionChanged;
                }
            }
        }

        private void StepLockRequirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepLockRequirement>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepProtectiveEquipment> _stepProtectiveEquipment;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepProtectiveEquipment> StepProtectiveEquipment
        {
            get
            {
                if (_stepProtectiveEquipment == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepProtectiveEquipment - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepProtectiveEquipment = new ObservableCollection<StepProtectiveEquipment>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepProtectiveEquipment.Where(x => x.Step == this.StepId).ToList<StepProtectiveEquipment>();
                        _stepProtectiveEquipment = new ObservableCollection<StepProtectiveEquipment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepProtectiveEquipment.CollectionChanged += StepProtectiveEquipment_CollectionChanged;
                }
                return _stepProtectiveEquipment;
            }
            private set
            {
                if (_stepProtectiveEquipment != null)
                {
                    _stepProtectiveEquipment.CollectionChanged -= StepProtectiveEquipment_CollectionChanged;
                }
                _stepProtectiveEquipment = value;
                if (_stepProtectiveEquipment != null)
                {
                    _stepProtectiveEquipment.CollectionChanged += StepProtectiveEquipment_CollectionChanged;
                }
            }
        }

        private void StepProtectiveEquipment_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepProtectiveEquipment>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<ActivityRelation> _fromStepActivityRelations;

        [InverseProperty("Step")]
        public virtual ObservableCollection<ActivityRelation> FromStepActivityRelations
        {
            get
            {
                if (_fromStepActivityRelations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromStepActivityRelations - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _fromStepActivityRelations = new ObservableCollection<ActivityRelation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ActivityRelations.Where(x => x.FromStep == this.StepId).ToList<ActivityRelation>();
                        _fromStepActivityRelations = new ObservableCollection<ActivityRelation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromStepActivityRelations.CollectionChanged += FromStepActivityRelations_CollectionChanged;
                }
                return _fromStepActivityRelations;
            }
            private set
            {
                if (_fromStepActivityRelations != null)
                {
                    _fromStepActivityRelations.CollectionChanged -= FromStepActivityRelations_CollectionChanged;
                }
                _fromStepActivityRelations = value;
                if (_fromStepActivityRelations != null)
                {
                    _fromStepActivityRelations.CollectionChanged += FromStepActivityRelations_CollectionChanged;
                }
            }
        }

        private void FromStepActivityRelations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ActivityRelation>())
                {
                    item.FromStep = this.StepId;
                }
            }
        }

        private ObservableCollection<ActivityRelation> _toStepActivityRelations;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ActivityRelation> ToStepActivityRelations
        {
            get
            {
                if (_toStepActivityRelations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToStepActivityRelations - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _toStepActivityRelations = new ObservableCollection<ActivityRelation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ActivityRelations.Where(x => x.ToStep == this.StepId).ToList<ActivityRelation>();
                        _toStepActivityRelations = new ObservableCollection<ActivityRelation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toStepActivityRelations.CollectionChanged += ToStepActivityRelations_CollectionChanged;
                }
                return _toStepActivityRelations;
            }
            private set
            {
                if (_toStepActivityRelations != null)
                {
                    _toStepActivityRelations.CollectionChanged -= ToStepActivityRelations_CollectionChanged;
                }
                _toStepActivityRelations = value;
                if (_toStepActivityRelations != null)
                {
                    _toStepActivityRelations.CollectionChanged += ToStepActivityRelations_CollectionChanged;
                }
            }
        }

        private void ToStepActivityRelations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ActivityRelation>())
                {
                    item.ToStep = this.StepId;
                }
            }
        }

        private ObservableCollection<StepVariable> _stepVariables;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepVariable> StepVariables
        {
            get
            {
                if (_stepVariables == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepVariables - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepVariables = new ObservableCollection<StepVariable>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepVariables.Where(x => x.Step == this.StepId).ToList<StepVariable>();
                        _stepVariables = new ObservableCollection<StepVariable>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepVariables.CollectionChanged += StepVariables_CollectionChanged;
                }
                return _stepVariables;
            }
            private set
            {
                if (_stepVariables != null)
                {
                    _stepVariables.CollectionChanged -= StepVariables_CollectionChanged;
                }
                _stepVariables = value;
                if (_stepVariables != null)
                {
                    _stepVariables.CollectionChanged += StepVariables_CollectionChanged;
                }
            }
        }

        private void StepVariables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepVariable>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepCondition> _stepConditions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepCondition> StepConditions
        {
            get
            {
                if (_stepConditions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepConditions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepConditions = new ObservableCollection<StepCondition>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepConditions.Where(x => x.Step == this.StepId).ToList<StepCondition>();
                        _stepConditions = new ObservableCollection<StepCondition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepConditions.CollectionChanged += StepConditions_CollectionChanged;
                }
                return _stepConditions;
            }
            private set
            {
                if (_stepConditions != null)
                {
                    _stepConditions.CollectionChanged -= StepConditions_CollectionChanged;
                }
                _stepConditions = value;
                if (_stepConditions != null)
                {
                    _stepConditions.CollectionChanged += StepConditions_CollectionChanged;
                }
            }
        }

        private void StepConditions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepCondition>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<FailureMode> _failureModes;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<FailureMode> FailureModes
        {
            get
            {
                if (_failureModes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FailureModes - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _failureModes = new ObservableCollection<FailureMode>();
                    }
                    else
                    {
                        var items = base.SoAContext.FailureModes.Where(x => x.Step == this.StepId).ToList<FailureMode>();
                        _failureModes = new ObservableCollection<FailureMode>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _failureModes.CollectionChanged += FailureModes_CollectionChanged;
                }
                return _failureModes;
            }
            private set
            {
                if (_failureModes != null)
                {
                    _failureModes.CollectionChanged -= FailureModes_CollectionChanged;
                }
                _failureModes = value;
                if (_failureModes != null)
                {
                    _failureModes.CollectionChanged += FailureModes_CollectionChanged;
                }
            }
        }

        private void FailureModes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<FailureMode>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepCue> _stepCues;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepCue> StepCues
        {
            get
            {
                if (_stepCues == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepCues - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepCues = new ObservableCollection<StepCue>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepCues.Where(x => x.Step == this.StepId).ToList<StepCue>();
                        _stepCues = new ObservableCollection<StepCue>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepCues.CollectionChanged += StepCues_CollectionChanged;
                }
                return _stepCues;
            }
            private set
            {
                if (_stepCues != null)
                {
                    _stepCues.CollectionChanged -= StepCues_CollectionChanged;
                }
                _stepCues = value;
                if (_stepCues != null)
                {
                    _stepCues.CollectionChanged += StepCues_CollectionChanged;
                }
            }
        }

        private void StepCues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepCue>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<DecisionPoint> _decisionPoints;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<DecisionPoint> DecisionPoints
        {
            get
            {
                if (_decisionPoints == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DecisionPoints - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _decisionPoints = new ObservableCollection<DecisionPoint>();
                    }
                    else
                    {
                        var items = base.SoAContext.DecisionPoints.Where(x => x.Step == this.StepId).ToList<DecisionPoint>();
                        _decisionPoints = new ObservableCollection<DecisionPoint>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _decisionPoints.CollectionChanged += DecisionPoints_CollectionChanged;
                }
                return _decisionPoints;
            }
            private set
            {
                if (_decisionPoints != null)
                {
                    _decisionPoints.CollectionChanged -= DecisionPoints_CollectionChanged;
                }
                _decisionPoints = value;
                if (_decisionPoints != null)
                {
                    _decisionPoints.CollectionChanged += DecisionPoints_CollectionChanged;
                }
            }
        }

        private void DecisionPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DecisionPoint>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepResource> _stepResources;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepResource> StepResources
        {
            get
            {
                if (_stepResources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepResources - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepResources = new ObservableCollection<StepResource>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepResources.Where(x => x.Step == this.StepId).ToList<StepResource>();
                        _stepResources = new ObservableCollection<StepResource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepResources.CollectionChanged += StepResources_CollectionChanged;
                }
                return _stepResources;
            }
            private set
            {
                if (_stepResources != null)
                {
                    _stepResources.CollectionChanged -= StepResources_CollectionChanged;
                }
                _stepResources = value;
                if (_stepResources != null)
                {
                    _stepResources.CollectionChanged += StepResources_CollectionChanged;
                }
            }
        }

        private void StepResources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepResource>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<TacticalResourceAllocation> _tacticalResourceAllocations;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<TacticalResourceAllocation> TacticalResourceAllocations
        {
            get
            {
                if (_tacticalResourceAllocations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access TacticalResourceAllocations - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _tacticalResourceAllocations = new ObservableCollection<TacticalResourceAllocation>();
                    }
                    else
                    {
                        var items = base.SoAContext.TacticalResourceAllocations.Where(x => x.Step == this.StepId).ToList<TacticalResourceAllocation>();
                        _tacticalResourceAllocations = new ObservableCollection<TacticalResourceAllocation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _tacticalResourceAllocations.CollectionChanged += TacticalResourceAllocations_CollectionChanged;
                }
                return _tacticalResourceAllocations;
            }
            private set
            {
                if (_tacticalResourceAllocations != null)
                {
                    _tacticalResourceAllocations.CollectionChanged -= TacticalResourceAllocations_CollectionChanged;
                }
                _tacticalResourceAllocations = value;
                if (_tacticalResourceAllocations != null)
                {
                    _tacticalResourceAllocations.CollectionChanged += TacticalResourceAllocations_CollectionChanged;
                }
            }
        }

        private void TacticalResourceAllocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<TacticalResourceAllocation>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<StepContextSensitivity> _stepContextSensitivities;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StepContextSensitivity> StepContextSensitivities
        {
            get
            {
                if (_stepContextSensitivities == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepContextSensitivities - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stepContextSensitivities = new ObservableCollection<StepContextSensitivity>();
                    }
                    else
                    {
                        var items = base.SoAContext.StepContextSensitivities.Where(x => x.Step == this.StepId).ToList<StepContextSensitivity>();
                        _stepContextSensitivities = new ObservableCollection<StepContextSensitivity>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stepContextSensitivities.CollectionChanged += StepContextSensitivities_CollectionChanged;
                }
                return _stepContextSensitivities;
            }
            private set
            {
                if (_stepContextSensitivities != null)
                {
                    _stepContextSensitivities.CollectionChanged -= StepContextSensitivities_CollectionChanged;
                }
                _stepContextSensitivities = value;
                if (_stepContextSensitivities != null)
                {
                    _stepContextSensitivities.CollectionChanged += StepContextSensitivities_CollectionChanged;
                }
            }
        }

        private void StepContextSensitivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StepContextSensitivity>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<SnapshotAssertion> _snapshotAssertions;

        [InverseProperty("Step")]
        public virtual ObservableCollection<SnapshotAssertion> SnapshotAssertions
        {
            get
            {
                if (_snapshotAssertions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SnapshotAssertions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>();
                    }
                    else
                    {
                        var items = base.SoAContext.SnapshotAssertions.Where(x => x.SourceStep == this.StepId).ToList<SnapshotAssertion>();
                        _snapshotAssertions = new ObservableCollection<SnapshotAssertion>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
                return _snapshotAssertions;
            }
            private set
            {
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged -= SnapshotAssertions_CollectionChanged;
                }
                _snapshotAssertions = value;
                if (_snapshotAssertions != null)
                {
                    _snapshotAssertions.CollectionChanged += SnapshotAssertions_CollectionChanged;
                }
            }
        }

        private void SnapshotAssertions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SnapshotAssertion>())
                {
                    item.SourceStep = this.StepId;
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _retrievalSegments;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<RetrievalSegment> RetrievalSegments
        {
            get
            {
                if (_retrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegments - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.Step == this.StepId).ToList<RetrievalSegment>();
                        _retrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
                return _retrievalSegments;
            }
            private set
            {
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged -= RetrievalSegments_CollectionChanged;
                }
                _retrievalSegments = value;
                if (_retrievalSegments != null)
                {
                    _retrievalSegments.CollectionChanged += RetrievalSegments_CollectionChanged;
                }
            }
        }

        private void RetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _assumedCurrentStepAssistantAnswers;

        [InverseProperty("Step")]
        public virtual ObservableCollection<AssistantAnswer> AssumedCurrentStepAssistantAnswers
        {
            get
            {
                if (_assumedCurrentStepAssistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssumedCurrentStepAssistantAnswers - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _assumedCurrentStepAssistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.AssumedCurrentStep == this.StepId).ToList<AssistantAnswer>();
                        _assumedCurrentStepAssistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assumedCurrentStepAssistantAnswers.CollectionChanged += AssumedCurrentStepAssistantAnswers_CollectionChanged;
                }
                return _assumedCurrentStepAssistantAnswers;
            }
            private set
            {
                if (_assumedCurrentStepAssistantAnswers != null)
                {
                    _assumedCurrentStepAssistantAnswers.CollectionChanged -= AssumedCurrentStepAssistantAnswers_CollectionChanged;
                }
                _assumedCurrentStepAssistantAnswers = value;
                if (_assumedCurrentStepAssistantAnswers != null)
                {
                    _assumedCurrentStepAssistantAnswers.CollectionChanged += AssumedCurrentStepAssistantAnswers_CollectionChanged;
                }
            }
        }

        private void AssumedCurrentStepAssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.AssumedCurrentStep = this.StepId;
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _assertedNextStepAssistantAnswers;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<AssistantAnswer> AssertedNextStepAssistantAnswers
        {
            get
            {
                if (_assertedNextStepAssistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssertedNextStepAssistantAnswers - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _assertedNextStepAssistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.AssertedNextStep == this.StepId).ToList<AssistantAnswer>();
                        _assertedNextStepAssistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assertedNextStepAssistantAnswers.CollectionChanged += AssertedNextStepAssistantAnswers_CollectionChanged;
                }
                return _assertedNextStepAssistantAnswers;
            }
            private set
            {
                if (_assertedNextStepAssistantAnswers != null)
                {
                    _assertedNextStepAssistantAnswers.CollectionChanged -= AssertedNextStepAssistantAnswers_CollectionChanged;
                }
                _assertedNextStepAssistantAnswers = value;
                if (_assertedNextStepAssistantAnswers != null)
                {
                    _assertedNextStepAssistantAnswers.CollectionChanged += AssertedNextStepAssistantAnswers_CollectionChanged;
                }
            }
        }

        private void AssertedNextStepAssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.AssertedNextStep = this.StepId;
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _recommendedStepAssistantAnswers;

        [InverseProperty("StepRefRef")]
        public virtual ObservableCollection<AssistantAnswer> RecommendedStepAssistantAnswers
        {
            get
            {
                if (_recommendedStepAssistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RecommendedStepAssistantAnswers - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _recommendedStepAssistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.RecommendedStep == this.StepId).ToList<AssistantAnswer>();
                        _recommendedStepAssistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _recommendedStepAssistantAnswers.CollectionChanged += RecommendedStepAssistantAnswers_CollectionChanged;
                }
                return _recommendedStepAssistantAnswers;
            }
            private set
            {
                if (_recommendedStepAssistantAnswers != null)
                {
                    _recommendedStepAssistantAnswers.CollectionChanged -= RecommendedStepAssistantAnswers_CollectionChanged;
                }
                _recommendedStepAssistantAnswers = value;
                if (_recommendedStepAssistantAnswers != null)
                {
                    _recommendedStepAssistantAnswers.CollectionChanged += RecommendedStepAssistantAnswers_CollectionChanged;
                }
            }
        }

        private void RecommendedStepAssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.RecommendedStep = this.StepId;
                }
            }
        }

        private ObservableCollection<ModelAnnotation> _modelAnnotations;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ModelAnnotation> ModelAnnotations
        {
            get
            {
                if (_modelAnnotations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ModelAnnotations - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>();
                    }
                    else
                    {
                        var items = base.SoAContext.ModelAnnotations.Where(x => x.Step == this.StepId).ToList<ModelAnnotation>();
                        _modelAnnotations = new ObservableCollection<ModelAnnotation>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
                return _modelAnnotations;
            }
            private set
            {
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged -= ModelAnnotations_CollectionChanged;
                }
                _modelAnnotations = value;
                if (_modelAnnotations != null)
                {
                    _modelAnnotations.CollectionChanged += ModelAnnotations_CollectionChanged;
                }
            }
        }

        private void ModelAnnotations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ModelAnnotation>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<AssignmentInstantCheck> _assignmentInstantChecks;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<AssignmentInstantCheck> AssignmentInstantChecks
        {
            get
            {
                if (_assignmentInstantChecks == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssignmentInstantChecks - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _assignmentInstantChecks = new ObservableCollection<AssignmentInstantCheck>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssignmentInstantChecks.Where(x => x.Step == this.StepId).ToList<AssignmentInstantCheck>();
                        _assignmentInstantChecks = new ObservableCollection<AssignmentInstantCheck>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assignmentInstantChecks.CollectionChanged += AssignmentInstantChecks_CollectionChanged;
                }
                return _assignmentInstantChecks;
            }
            private set
            {
                if (_assignmentInstantChecks != null)
                {
                    _assignmentInstantChecks.CollectionChanged -= AssignmentInstantChecks_CollectionChanged;
                }
                _assignmentInstantChecks = value;
                if (_assignmentInstantChecks != null)
                {
                    _assignmentInstantChecks.CollectionChanged += AssignmentInstantChecks_CollectionChanged;
                }
            }
        }

        private void AssignmentInstantChecks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssignmentInstantCheck>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<ProcessDesignDecision> _processDesignDecisions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ProcessDesignDecision> ProcessDesignDecisions
        {
            get
            {
                if (_processDesignDecisions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcessDesignDecisions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _processDesignDecisions = new ObservableCollection<ProcessDesignDecision>();
                    }
                    else
                    {
                        var items = base.SoAContext.ProcessDesignDecisions.Where(x => x.Step == this.StepId).ToList<ProcessDesignDecision>();
                        _processDesignDecisions = new ObservableCollection<ProcessDesignDecision>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _processDesignDecisions.CollectionChanged += ProcessDesignDecisions_CollectionChanged;
                }
                return _processDesignDecisions;
            }
            private set
            {
                if (_processDesignDecisions != null)
                {
                    _processDesignDecisions.CollectionChanged -= ProcessDesignDecisions_CollectionChanged;
                }
                _processDesignDecisions = value;
                if (_processDesignDecisions != null)
                {
                    _processDesignDecisions.CollectionChanged += ProcessDesignDecisions_CollectionChanged;
                }
            }
        }

        private void ProcessDesignDecisions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ProcessDesignDecision>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<AssignmentRoutedNotice> _assignmentRoutedNotices;

        [InverseProperty("Step")]
        public virtual ObservableCollection<AssignmentRoutedNotice> AssignmentRoutedNotices
        {
            get
            {
                if (_assignmentRoutedNotices == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssignmentRoutedNotices - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _assignmentRoutedNotices = new ObservableCollection<AssignmentRoutedNotice>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssignmentRoutedNotices.Where(x => x.NoticeStep == this.StepId).ToList<AssignmentRoutedNotice>();
                        _assignmentRoutedNotices = new ObservableCollection<AssignmentRoutedNotice>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assignmentRoutedNotices.CollectionChanged += AssignmentRoutedNotices_CollectionChanged;
                }
                return _assignmentRoutedNotices;
            }
            private set
            {
                if (_assignmentRoutedNotices != null)
                {
                    _assignmentRoutedNotices.CollectionChanged -= AssignmentRoutedNotices_CollectionChanged;
                }
                _assignmentRoutedNotices = value;
                if (_assignmentRoutedNotices != null)
                {
                    _assignmentRoutedNotices.CollectionChanged += AssignmentRoutedNotices_CollectionChanged;
                }
            }
        }

        private void AssignmentRoutedNotices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssignmentRoutedNotice>())
                {
                    item.NoticeStep = this.StepId;
                }
            }
        }

        private ObservableCollection<PractitionerExpertise> _practitionerExpertise;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<PractitionerExpertise> PractitionerExpertise
        {
            get
            {
                if (_practitionerExpertise == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access PractitionerExpertise - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>();
                    }
                    else
                    {
                        var items = base.SoAContext.PractitionerExpertise.Where(x => x.Step == this.StepId).ToList<PractitionerExpertise>();
                        _practitionerExpertise = new ObservableCollection<PractitionerExpertise>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _practitionerExpertise.CollectionChanged += PractitionerExpertise_CollectionChanged;
                }
                return _practitionerExpertise;
            }
            private set
            {
                if (_practitionerExpertise != null)
                {
                    _practitionerExpertise.CollectionChanged -= PractitionerExpertise_CollectionChanged;
                }
                _practitionerExpertise = value;
                if (_practitionerExpertise != null)
                {
                    _practitionerExpertise.CollectionChanged += PractitionerExpertise_CollectionChanged;
                }
            }
        }

        private void PractitionerExpertise_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<PractitionerExpertise>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<CriticalIncident> _criticalIncidents;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<CriticalIncident> CriticalIncidents
        {
            get
            {
                if (_criticalIncidents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CriticalIncidents - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _criticalIncidents = new ObservableCollection<CriticalIncident>();
                    }
                    else
                    {
                        var items = base.SoAContext.CriticalIncidents.Where(x => x.Step == this.StepId).ToList<CriticalIncident>();
                        _criticalIncidents = new ObservableCollection<CriticalIncident>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
                return _criticalIncidents;
            }
            private set
            {
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged -= CriticalIncidents_CollectionChanged;
                }
                _criticalIncidents = value;
                if (_criticalIncidents != null)
                {
                    _criticalIncidents.CollectionChanged += CriticalIncidents_CollectionChanged;
                }
            }
        }

        private void CriticalIncidents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CriticalIncident>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<InterviewProbe> _interviewProbes;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<InterviewProbe> InterviewProbes
        {
            get
            {
                if (_interviewProbes == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InterviewProbes - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _interviewProbes = new ObservableCollection<InterviewProbe>();
                    }
                    else
                    {
                        var items = base.SoAContext.InterviewProbes.Where(x => x.Step == this.StepId).ToList<InterviewProbe>();
                        _interviewProbes = new ObservableCollection<InterviewProbe>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _interviewProbes.CollectionChanged += InterviewProbes_CollectionChanged;
                }
                return _interviewProbes;
            }
            private set
            {
                if (_interviewProbes != null)
                {
                    _interviewProbes.CollectionChanged -= InterviewProbes_CollectionChanged;
                }
                _interviewProbes = value;
                if (_interviewProbes != null)
                {
                    _interviewProbes.CollectionChanged += InterviewProbes_CollectionChanged;
                }
            }
        }

        private void InterviewProbes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<InterviewProbe>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<ObservedAction> _observedActions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ObservedAction> ObservedActions
        {
            get
            {
                if (_observedActions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ObservedActions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _observedActions = new ObservableCollection<ObservedAction>();
                    }
                    else
                    {
                        var items = base.SoAContext.ObservedActions.Where(x => x.Step == this.StepId).ToList<ObservedAction>();
                        _observedActions = new ObservableCollection<ObservedAction>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
                return _observedActions;
            }
            private set
            {
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged -= ObservedActions_CollectionChanged;
                }
                _observedActions = value;
                if (_observedActions != null)
                {
                    _observedActions.CollectionChanged += ObservedActions_CollectionChanged;
                }
            }
        }

        private void ObservedActions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ObservedAction>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<WorkflowViewDivergence> _workflowViewDivergences;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<WorkflowViewDivergence> WorkflowViewDivergences
        {
            get
            {
                if (_workflowViewDivergences == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowViewDivergences - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowViewDivergences.Where(x => x.Step == this.StepId).ToList<WorkflowViewDivergence>();
                        _workflowViewDivergences = new ObservableCollection<WorkflowViewDivergence>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowViewDivergences.CollectionChanged += WorkflowViewDivergences_CollectionChanged;
                }
                return _workflowViewDivergences;
            }
            private set
            {
                if (_workflowViewDivergences != null)
                {
                    _workflowViewDivergences.CollectionChanged -= WorkflowViewDivergences_CollectionChanged;
                }
                _workflowViewDivergences = value;
                if (_workflowViewDivergences != null)
                {
                    _workflowViewDivergences.CollectionChanged += WorkflowViewDivergences_CollectionChanged;
                }
            }
        }

        private void WorkflowViewDivergences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowViewDivergence>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<ExpertCognition> _expertCognitions;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ExpertCognition> ExpertCognitions
        {
            get
            {
                if (_expertCognitions == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ExpertCognitions - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _expertCognitions = new ObservableCollection<ExpertCognition>();
                    }
                    else
                    {
                        var items = base.SoAContext.ExpertCognitions.Where(x => x.Step == this.StepId).ToList<ExpertCognition>();
                        _expertCognitions = new ObservableCollection<ExpertCognition>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _expertCognitions.CollectionChanged += ExpertCognitions_CollectionChanged;
                }
                return _expertCognitions;
            }
            private set
            {
                if (_expertCognitions != null)
                {
                    _expertCognitions.CollectionChanged -= ExpertCognitions_CollectionChanged;
                }
                _expertCognitions = value;
                if (_expertCognitions != null)
                {
                    _expertCognitions.CollectionChanged += ExpertCognitions_CollectionChanged;
                }
            }
        }

        private void ExpertCognitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ExpertCognition>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<ConceptLadderRung> _conceptLadderRungs;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ConceptLadderRung> ConceptLadderRungs
        {
            get
            {
                if (_conceptLadderRungs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConceptLadderRungs - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _conceptLadderRungs = new ObservableCollection<ConceptLadderRung>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConceptLadderRungs.Where(x => x.Step == this.StepId).ToList<ConceptLadderRung>();
                        _conceptLadderRungs = new ObservableCollection<ConceptLadderRung>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _conceptLadderRungs.CollectionChanged += ConceptLadderRungs_CollectionChanged;
                }
                return _conceptLadderRungs;
            }
            private set
            {
                if (_conceptLadderRungs != null)
                {
                    _conceptLadderRungs.CollectionChanged -= ConceptLadderRungs_CollectionChanged;
                }
                _conceptLadderRungs = value;
                if (_conceptLadderRungs != null)
                {
                    _conceptLadderRungs.CollectionChanged += ConceptLadderRungs_CollectionChanged;
                }
            }
        }

        private void ConceptLadderRungs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConceptLadderRung>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<KnowledgeHolding> _knowledgeHoldings;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<KnowledgeHolding> KnowledgeHoldings
        {
            get
            {
                if (_knowledgeHoldings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeHoldings - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeHoldings.Where(x => x.Step == this.StepId).ToList<KnowledgeHolding>();
                        _knowledgeHoldings = new ObservableCollection<KnowledgeHolding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeHoldings.CollectionChanged += KnowledgeHoldings_CollectionChanged;
                }
                return _knowledgeHoldings;
            }
            private set
            {
                if (_knowledgeHoldings != null)
                {
                    _knowledgeHoldings.CollectionChanged -= KnowledgeHoldings_CollectionChanged;
                }
                _knowledgeHoldings = value;
                if (_knowledgeHoldings != null)
                {
                    _knowledgeHoldings.CollectionChanged += KnowledgeHoldings_CollectionChanged;
                }
            }
        }

        private void KnowledgeHoldings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeHolding>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<KnowledgeTrace> _knowledgeTraces;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<KnowledgeTrace> KnowledgeTraces
        {
            get
            {
                if (_knowledgeTraces == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeTraces - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeTraces.Where(x => x.Step == this.StepId).ToList<KnowledgeTrace>();
                        _knowledgeTraces = new ObservableCollection<KnowledgeTrace>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
                return _knowledgeTraces;
            }
            private set
            {
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged -= KnowledgeTraces_CollectionChanged;
                }
                _knowledgeTraces = value;
                if (_knowledgeTraces != null)
                {
                    _knowledgeTraces.CollectionChanged += KnowledgeTraces_CollectionChanged;
                }
            }
        }

        private void KnowledgeTraces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeTrace>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<MinedFlowEdge> _fromStepMinedFlowEdges;

        [InverseProperty("Step")]
        public virtual ObservableCollection<MinedFlowEdge> FromStepMinedFlowEdges
        {
            get
            {
                if (_fromStepMinedFlowEdges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromStepMinedFlowEdges - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _fromStepMinedFlowEdges = new ObservableCollection<MinedFlowEdge>();
                    }
                    else
                    {
                        var items = base.SoAContext.MinedFlowEdges.Where(x => x.FromStep == this.StepId).ToList<MinedFlowEdge>();
                        _fromStepMinedFlowEdges = new ObservableCollection<MinedFlowEdge>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromStepMinedFlowEdges.CollectionChanged += FromStepMinedFlowEdges_CollectionChanged;
                }
                return _fromStepMinedFlowEdges;
            }
            private set
            {
                if (_fromStepMinedFlowEdges != null)
                {
                    _fromStepMinedFlowEdges.CollectionChanged -= FromStepMinedFlowEdges_CollectionChanged;
                }
                _fromStepMinedFlowEdges = value;
                if (_fromStepMinedFlowEdges != null)
                {
                    _fromStepMinedFlowEdges.CollectionChanged += FromStepMinedFlowEdges_CollectionChanged;
                }
            }
        }

        private void FromStepMinedFlowEdges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MinedFlowEdge>())
                {
                    item.FromStep = this.StepId;
                }
            }
        }

        private ObservableCollection<MinedFlowEdge> _toStepMinedFlowEdges;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<MinedFlowEdge> ToStepMinedFlowEdges
        {
            get
            {
                if (_toStepMinedFlowEdges == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToStepMinedFlowEdges - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _toStepMinedFlowEdges = new ObservableCollection<MinedFlowEdge>();
                    }
                    else
                    {
                        var items = base.SoAContext.MinedFlowEdges.Where(x => x.ToStep == this.StepId).ToList<MinedFlowEdge>();
                        _toStepMinedFlowEdges = new ObservableCollection<MinedFlowEdge>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toStepMinedFlowEdges.CollectionChanged += ToStepMinedFlowEdges_CollectionChanged;
                }
                return _toStepMinedFlowEdges;
            }
            private set
            {
                if (_toStepMinedFlowEdges != null)
                {
                    _toStepMinedFlowEdges.CollectionChanged -= ToStepMinedFlowEdges_CollectionChanged;
                }
                _toStepMinedFlowEdges = value;
                if (_toStepMinedFlowEdges != null)
                {
                    _toStepMinedFlowEdges.CollectionChanged += ToStepMinedFlowEdges_CollectionChanged;
                }
            }
        }

        private void ToStepMinedFlowEdges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<MinedFlowEdge>())
                {
                    item.ToStep = this.StepId;
                }
            }
        }

        private ObservableCollection<StakeholderPerspectif> _stakeholderPerspectives;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<StakeholderPerspectif> StakeholderPerspectives
        {
            get
            {
                if (_stakeholderPerspectives == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StakeholderPerspectives - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>();
                    }
                    else
                    {
                        var items = base.SoAContext.StakeholderPerspectives.Where(x => x.Step == this.StepId).ToList<StakeholderPerspectif>();
                        _stakeholderPerspectives = new ObservableCollection<StakeholderPerspectif>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
                return _stakeholderPerspectives;
            }
            private set
            {
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged -= StakeholderPerspectives_CollectionChanged;
                }
                _stakeholderPerspectives = value;
                if (_stakeholderPerspectives != null)
                {
                    _stakeholderPerspectives.CollectionChanged += StakeholderPerspectives_CollectionChanged;
                }
            }
        }

        private void StakeholderPerspectives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<StakeholderPerspectif>())
                {
                    item.Step = this.StepId;
                }
            }
        }

        private ObservableCollection<ArtifactHandoff> _fromStepArtifactHandoffs;

        [InverseProperty("Step")]
        public virtual ObservableCollection<ArtifactHandoff> FromStepArtifactHandoffs
        {
            get
            {
                if (_fromStepArtifactHandoffs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FromStepArtifactHandoffs - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _fromStepArtifactHandoffs = new ObservableCollection<ArtifactHandoff>();
                    }
                    else
                    {
                        var items = base.SoAContext.ArtifactHandoffs.Where(x => x.FromStep == this.StepId).ToList<ArtifactHandoff>();
                        _fromStepArtifactHandoffs = new ObservableCollection<ArtifactHandoff>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _fromStepArtifactHandoffs.CollectionChanged += FromStepArtifactHandoffs_CollectionChanged;
                }
                return _fromStepArtifactHandoffs;
            }
            private set
            {
                if (_fromStepArtifactHandoffs != null)
                {
                    _fromStepArtifactHandoffs.CollectionChanged -= FromStepArtifactHandoffs_CollectionChanged;
                }
                _fromStepArtifactHandoffs = value;
                if (_fromStepArtifactHandoffs != null)
                {
                    _fromStepArtifactHandoffs.CollectionChanged += FromStepArtifactHandoffs_CollectionChanged;
                }
            }
        }

        private void FromStepArtifactHandoffs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ArtifactHandoff>())
                {
                    item.FromStep = this.StepId;
                }
            }
        }

        private ObservableCollection<ArtifactHandoff> _toStepArtifactHandoffs;

        [InverseProperty("StepRef")]
        public virtual ObservableCollection<ArtifactHandoff> ToStepArtifactHandoffs
        {
            get
            {
                if (_toStepArtifactHandoffs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ToStepArtifactHandoffs - no database context is set. StepId: " + this.StepId + ".");
                        }
                        _toStepArtifactHandoffs = new ObservableCollection<ArtifactHandoff>();
                    }
                    else
                    {
                        var items = base.SoAContext.ArtifactHandoffs.Where(x => x.ToStep == this.StepId).ToList<ArtifactHandoff>();
                        _toStepArtifactHandoffs = new ObservableCollection<ArtifactHandoff>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _toStepArtifactHandoffs.CollectionChanged += ToStepArtifactHandoffs_CollectionChanged;
                }
                return _toStepArtifactHandoffs;
            }
            private set
            {
                if (_toStepArtifactHandoffs != null)
                {
                    _toStepArtifactHandoffs.CollectionChanged -= ToStepArtifactHandoffs_CollectionChanged;
                }
                _toStepArtifactHandoffs = value;
                if (_toStepArtifactHandoffs != null)
                {
                    _toStepArtifactHandoffs.CollectionChanged += ToStepArtifactHandoffs_CollectionChanged;
                }
            }
        }

        private void ToStepArtifactHandoffs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ArtifactHandoff>())
                {
                    item.ToStep = this.StepId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.Role;
            _ = this.Step;
            _ = this.StepRef;
            _ = this.StepRefRef;
            _ = this.Error;
            _ = this.Procedure;
            _ = this.EnergySource;
            _ = this.StepRefRefRef;
            _ = this.ProcessStage;
            _ = this.ParentStepSteps;
            _ = this.FirstChildStepSteps;
            _ = this.VerifiesStepSteps;
            _ = this.PrerequisiteStepSteps;
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
            _ = this.StepLockRequirements;
            _ = this.StepProtectiveEquipment;
            _ = this.FromStepActivityRelations;
            _ = this.ToStepActivityRelations;
            _ = this.StepVariables;
            _ = this.StepConditions;
            _ = this.FailureModes;
            _ = this.StepCues;
            _ = this.DecisionPoints;
            _ = this.StepResources;
            _ = this.TacticalResourceAllocations;
            _ = this.StepContextSensitivities;
            _ = this.SnapshotAssertions;
            _ = this.RetrievalSegments;
            _ = this.AssumedCurrentStepAssistantAnswers;
            _ = this.AssertedNextStepAssistantAnswers;
            _ = this.RecommendedStepAssistantAnswers;
            _ = this.ModelAnnotations;
            _ = this.AssignmentInstantChecks;
            _ = this.ProcessDesignDecisions;
            _ = this.AssignmentRoutedNotices;
            _ = this.PractitionerExpertise;
            _ = this.CriticalIncidents;
            _ = this.InterviewProbes;
            _ = this.ObservedActions;
            _ = this.WorkflowViewDivergences;
            _ = this.ExpertCognitions;
            _ = this.ConceptLadderRungs;
            _ = this.KnowledgeHoldings;
            _ = this.KnowledgeTraces;
            _ = this.FromStepMinedFlowEdges;
            _ = this.ToStepMinedFlowEdges;
            _ = this.StakeholderPerspectives;
            _ = this.FromStepArtifactHandoffs;
            _ = this.ToStepArtifactHandoffs;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
