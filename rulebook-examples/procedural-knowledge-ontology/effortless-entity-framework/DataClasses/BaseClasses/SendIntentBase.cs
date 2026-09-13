
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
    [Table("SendIntents")]
    public class SendIntentBase : SoAEntityBase
    {
        [Key]
        public string SendIntentId { get; set; }

        // Formula Name (rulebook: ={{Recipient}} & " / " & {{MessageTemplate}} & " / intent")
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Recipient)), F.S(" / "), F.TextOr(F.Of(this.MessageTemplate)), F.S(" / intent")))); set { }
        }

        public string? ProposedBody { get; set; }
        public int? ProposedSendAtLocalHour { get; set; }
        public DateTimeOffset? EvaluatedAt { get; set; }
        // Formula IntentPolicy (rulebook: =INDEX(MessageTemplates!{{CommunicationPolicy}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public string? IntentPolicy
        {
            get => F.AsString(F.Memo(this, "IntentPolicy", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.CommunicationPolicy), () => F.Of(new MessageTemplate().CommunicationPolicy)))); set { }
        }

        // Formula IntentChannel (rulebook: =INDEX(CommunicationPolicies!{{Channel}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public string? IntentChannel
        {
            get => F.AsString(F.Memo(this, "IntentChannel", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.Channel), () => F.Of(new CommunicationPolicy().Channel)))); set { }
        }

        // Formula PolicyIsActive (rulebook: =INDEX(CommunicationPolicies!{{IsActivePolicy}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public bool? PolicyIsActive
        {
            get => F.AsBool(F.Memo(this, "PolicyIsActive", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.IsActivePolicy), () => F.Of(new CommunicationPolicy().IsActivePolicy)))); set { }
        }

        // Formula IntentRequiresConsent (rulebook: =INDEX(CommunicationPolicies!{{ConsentRequired}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public bool? IntentRequiresConsent
        {
            get => F.AsBool(F.Memo(this, "IntentRequiresConsent", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.ConsentRequired), () => F.Of(new CommunicationPolicy().ConsentRequired)))); set { }
        }

        // Formula RecipientHasChannelConsent (rulebook: =INDEX(Recipients!{{HasSmsConsent}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        [NotMapped]
        public bool? RecipientHasChannelConsent
        {
            get => F.AsBool(F.Memo(this, "RecipientHasChannelConsent", () => F.Lookup<Recipient>(this, "Recipients", "RecipientId", __c => __c.Recipients, __r => F.Of(__r.RecipientId), F.Of(this.Recipient), __r => F.Of(__r.HasSmsConsent), () => F.Of(new Recipient().HasSmsConsent)))); set { }
        }

        // Formula ConsentGatePassed (rulebook: =OR(NOT({{IntentRequiresConsent}}), {{RecipientHasChannelConsent}}))
        [NotMapped]
        public bool? ConsentGatePassed
        {
            get => F.AsBool(F.Memo(this, "ConsentGatePassed", () => F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.IntentRequiresConsent)))), F.Bool3(F.Of(this.RecipientHasChannelConsent))))); set { }
        }

        // Formula RecipientIsSmsReachable (rulebook: =INDEX(Recipients!{{IsSmsReachable}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        [NotMapped]
        public bool? RecipientIsSmsReachable
        {
            get => F.AsBool(F.Memo(this, "RecipientIsSmsReachable", () => F.Lookup<Recipient>(this, "Recipients", "RecipientId", __c => __c.Recipients, __r => F.Of(__r.RecipientId), F.Of(this.Recipient), __r => F.Of(__r.IsSmsReachable), () => F.Of(new Recipient().IsSmsReachable)))); set { }
        }

        // Formula RecipientIsEmailReachable (rulebook: =INDEX(Recipients!{{IsEmailReachable}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        [NotMapped]
        public bool? RecipientIsEmailReachable
        {
            get => F.AsBool(F.Memo(this, "RecipientIsEmailReachable", () => F.Lookup<Recipient>(this, "Recipients", "RecipientId", __c => __c.Recipients, __r => F.Of(__r.RecipientId), F.Of(this.Recipient), __r => F.Of(__r.IsEmailReachable), () => F.Of(new Recipient().IsEmailReachable)))); set { }
        }

        // Formula ReachabilityGatePassed (rulebook: =IF({{IntentChannel}} = "SMS", {{RecipientIsSmsReachable}}, {{RecipientIsEmailReachable}}))
        [NotMapped]
        public bool? ReachabilityGatePassed
        {
            get => F.AsBool(F.Memo(this, "ReachabilityGatePassed", () => (F.Truthy(F.Bool3(F.Eq(F.Of(this.IntentChannel), F.S("SMS")))) ? F.Of(this.RecipientIsSmsReachable) : F.Of(this.RecipientIsEmailReachable)))); set { }
        }

        // Formula PermissionGatePassed (rulebook: =AND({{PolicyIsActive}}, AND({{ConsentGatePassed}}, {{ReachabilityGatePassed}})))
        [NotMapped]
        public bool? PermissionGatePassed
        {
            get => F.AsBool(F.Memo(this, "PermissionGatePassed", () => F.And(F.Bool3(F.Of(this.PolicyIsActive)), F.Bool3(F.And(F.Bool3(F.Of(this.ConsentGatePassed)), F.Bool3(F.Of(this.ReachabilityGatePassed))))))); set { }
        }

        // Formula IntentQuietStartHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursStartHour}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? IntentQuietStartHour
        {
            get => F.AsInt(F.Memo(this, "IntentQuietStartHour", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.QuietHoursStartHour), () => F.Of(new CommunicationPolicy().QuietHoursStartHour))))); set { }
        }

        // Formula IntentQuietEndHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursEndHour}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? IntentQuietEndHour
        {
            get => F.AsInt(F.Memo(this, "IntentQuietEndHour", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.QuietHoursEndHour), () => F.Of(new CommunicationPolicy().QuietHoursEndHour))))); set { }
        }

        // Formula IntentPolicyHasQuietHours (rulebook: ={{IntentQuietStartHour}} <> {{IntentQuietEndHour}})
        [NotMapped]
        public bool? IntentPolicyHasQuietHours
        {
            get => F.AsBool(F.Memo(this, "IntentPolicyHasQuietHours", () => F.Ne(F.Of(this.IntentQuietStartHour), F.Of(this.IntentQuietEndHour)))); set { }
        }

        // Formula IntentQuietWindowWraps (rulebook: ={{IntentQuietStartHour}} > {{IntentQuietEndHour}})
        [NotMapped]
        public bool? IntentQuietWindowWraps
        {
            get => F.AsBool(F.Memo(this, "IntentQuietWindowWraps", () => F.Cmp(F.Of(this.IntentQuietStartHour), ">", F.Of(this.IntentQuietEndHour)))); set { }
        }

        // Formula IntentIsInsideQuietWindow (rulebook: =IF({{IntentQuietWindowWraps}}, OR({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}), AND({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}})))
        [NotMapped]
        public bool? IntentIsInsideQuietWindow
        {
            get => F.AsBool(F.Memo(this, "IntentIsInsideQuietWindow", () => (F.Truthy(F.Bool3(F.Of(this.IntentQuietWindowWraps))) ? F.Or(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedSendAtLocalHour)), ">=", F.Of(this.IntentQuietStartHour))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedSendAtLocalHour)), "<", F.Of(this.IntentQuietEndHour)))) : F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedSendAtLocalHour)), ">=", F.Of(this.IntentQuietStartHour))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedSendAtLocalHour)), "<", F.Of(this.IntentQuietEndHour))))))); set { }
        }

        // Formula TimingGatePassed (rulebook: =OR(NOT({{IntentPolicyHasQuietHours}}), NOT({{IntentIsInsideQuietWindow}})))
        [NotMapped]
        public bool? TimingGatePassed
        {
            get => F.AsBool(F.Memo(this, "TimingGatePassed", () => F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.IntentPolicyHasQuietHours)))), F.Bool3(F.Not(F.Bool3(F.Of(this.IntentIsInsideQuietWindow))))))); set { }
        }

        // Formula HoursUntilWindowOpens (rulebook: =IF({{TimingGatePassed}}, 0, IF({{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}, {{IntentQuietEndHour}} - {{ProposedSendAtLocalHour}}, 24 - {{ProposedSendAtLocalHour}} + {{IntentQuietEndHour}})))
        [NotMapped]
        public int? HoursUntilWindowOpens
        {
            get => F.AsInt(F.Memo(this, "HoursUntilWindowOpens", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.TimingGatePassed))) ? F.I(0) : (F.Truthy(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedSendAtLocalHour)), "<", F.Of(this.IntentQuietEndHour)))) ? F.Sub(F.Of(this.IntentQuietEndHour), F.Of(this.ProposedSendAtLocalHour)) : F.Add(F.Sub(F.I(24), F.Of(this.ProposedSendAtLocalHour)), F.Of(this.IntentQuietEndHour))))))); set { }
        }

        // Formula IntentMaxMessageLength (rulebook: =INDEX(CommunicationPolicies!{{MaxMessageLength}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? IntentMaxMessageLength
        {
            get => F.AsInt(F.Memo(this, "IntentMaxMessageLength", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.MaxMessageLength), () => F.Of(new CommunicationPolicy().MaxMessageLength))))); set { }
        }

        // Formula IntentMaxSegments (rulebook: =INDEX(CommunicationPolicies!{{MaxSegments}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? IntentMaxSegments
        {
            get => F.AsInt(F.Memo(this, "IntentMaxSegments", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.MaxSegments), () => F.Of(new CommunicationPolicy().MaxSegments))))); set { }
        }

        public int? ProposedBodyLength { get; set; }
        public int? ProposedSegmentCount { get; set; }
        // Formula LengthGatePassed (rulebook: =AND({{ProposedBodyLength}} > 0, {{ProposedSegmentCount}} <= {{IntentMaxSegments}}))
        [NotMapped]
        public bool? LengthGatePassed
        {
            get => F.AsBool(F.Memo(this, "LengthGatePassed", () => F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedBodyLength)), ">", F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedSegmentCount)), "<=", F.Of(this.IntentMaxSegments)))))); set { }
        }

        // Formula IntentRequiredOptOutPhrase (rulebook: =INDEX(CommunicationPolicies!{{RequiredOptOutPhrase}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public string? IntentRequiredOptOutPhrase
        {
            get => F.AsString(F.Memo(this, "IntentRequiredOptOutPhrase", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.RequiredOptOutPhrase), () => F.Of(new CommunicationPolicy().RequiredOptOutPhrase)))); set { }
        }

        public int? ProposedOptOutPosition { get; set; }
        // Formula OptOutGatePassed (rulebook: =OR({{IntentRequiredOptOutPhrase}} = "", AND({{ProposedOptOutPosition}} > 0, {{ProposedOptOutPosition}} <= {{IntentMaxMessageLength}})))
        [NotMapped]
        public bool? OptOutGatePassed
        {
            get => F.AsBool(F.Memo(this, "OptOutGatePassed", () => F.Or(F.Bool3(F.IsBlank(F.Of(this.IntentRequiredOptOutPhrase))), F.Bool3(F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedOptOutPosition)), ">", F.I(0))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ProposedOptOutPosition)), "<=", F.Of(this.IntentMaxMessageLength)))))))); set { }
        }

        // Formula ContentGatePassed (rulebook: =AND({{LengthGatePassed}}, {{OptOutGatePassed}}))
        [NotMapped]
        public bool? ContentGatePassed
        {
            get => F.AsBool(F.Memo(this, "ContentGatePassed", () => F.And(F.Bool3(F.Of(this.LengthGatePassed)), F.Bool3(F.Of(this.OptOutGatePassed))))); set { }
        }

        // Formula TemplateIsSendable (rulebook: =INDEX(MessageTemplates!{{IsSendableUnderApproval}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public bool? TemplateIsSendable
        {
            get => F.AsBool(F.Memo(this, "TemplateIsSendable", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.IsSendableUnderApproval), () => F.Of(new MessageTemplate().IsSendableUnderApproval)))); set { }
        }

        // Formula ExecutionHasLegalClearance (rulebook: =INDEX(ProcedureExecutions!{{HasClearedLegalReview}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public bool? ExecutionHasLegalClearance
        {
            get => F.AsBool(F.Memo(this, "ExecutionHasLegalClearance", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.ProcedureExecution), __r => F.Of(__r.HasClearedLegalReview), () => F.Of(new ProcedureExecution().HasClearedLegalReview)))); set { }
        }

        // Formula IntentApprovalRole (rulebook: =INDEX(CommunicationPolicies!{{ApprovalRole}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public string? IntentApprovalRole
        {
            get => F.AsString(F.Memo(this, "IntentApprovalRole", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.IntentPolicy), __r => F.Of(__r.ApprovalRole), () => F.Of(new CommunicationPolicy().ApprovalRole)))); set { }
        }

        // Formula ApprovalRoleAgentKind (rulebook: =INDEX(Roles!{{CurrentAgentKind}}, MATCH({{IntentApprovalRole}}, Roles!{{RoleId}}, 0)))
        [NotMapped]
        public string? ApprovalRoleAgentKind
        {
            get => F.AsString(F.Memo(this, "ApprovalRoleAgentKind", () => F.Lookup<Role>(this, "Roles", "RoleId", __c => __c.Roles, __r => F.Of(__r.RoleId), F.Of(this.IntentApprovalRole), __r => F.Of(__r.CurrentAgentKind), () => F.Of(new Role().CurrentAgentKind)))); set { }
        }

        // Formula ApprovalIsHuman (rulebook: ={{ApprovalRoleAgentKind}} = "Human")
        [NotMapped]
        public bool? ApprovalIsHuman
        {
            get => F.AsBool(F.Memo(this, "ApprovalIsHuman", () => F.Eq(F.Of(this.ApprovalRoleAgentKind), F.S("Human")))); set { }
        }

        // Formula AuthorizationGatePassed (rulebook: =AND({{TemplateIsSendable}}, AND({{ExecutionHasLegalClearance}}, {{ApprovalIsHuman}})))
        [NotMapped]
        public bool? AuthorizationGatePassed
        {
            get => F.AsBool(F.Memo(this, "AuthorizationGatePassed", () => F.And(F.Bool3(F.Of(this.TemplateIsSendable)), F.Bool3(F.And(F.Bool3(F.Of(this.ExecutionHasLegalClearance)), F.Bool3(F.Of(this.ApprovalIsHuman))))))); set { }
        }

        // Formula IsClearedToSend (rulebook: =AND({{PermissionGatePassed}}, AND({{TimingGatePassed}}, AND({{ContentGatePassed}}, {{AuthorizationGatePassed}}))))
        [NotMapped]
        public bool? IsClearedToSend
        {
            get => F.AsBool(F.Memo(this, "IsClearedToSend", () => F.And(F.Bool3(F.Of(this.PermissionGatePassed)), F.Bool3(F.And(F.Bool3(F.Of(this.TimingGatePassed)), F.Bool3(F.And(F.Bool3(F.Of(this.ContentGatePassed)), F.Bool3(F.Of(this.AuthorizationGatePassed))))))))); set { }
        }

        // Formula BlockingGateName (rulebook: =IF({{IsClearedToSend}}, "", IF(NOT({{PermissionGatePassed}}), "Permission", IF(NOT({{TimingGatePassed}}), "Timing", IF(NOT({{ContentGatePassed}}), "Content", "Authorization")))))
        [NotMapped]
        public string? BlockingGateName
        {
            get => F.AsString(F.Memo(this, "BlockingGateName", () => (F.Truthy(F.Bool3(F.Of(this.IsClearedToSend))) ? F.S("") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.PermissionGatePassed))))) ? F.S("Permission") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.TimingGatePassed))))) ? F.S("Timing") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.Of(this.ContentGatePassed))))) ? F.S("Content") : F.S("Authorization"))))))); set { }
        }

        // Formula HasResultingDelivery (rulebook: ={{ResultingDelivery}} <> "")
        [NotMapped]
        public bool? HasResultingDelivery
        {
            get => F.AsBool(F.Memo(this, "HasResultingDelivery", () => F.IsNotBlank(F.Of(this.ResultingDelivery)))); set { }
        }

        // Formula ResultingDeliveryWasTransmitted (rulebook: =INDEX(MessageDeliveries!{{WasActuallyTransmitted}}, MATCH({{ResultingDelivery}}, MessageDeliveries!{{MessageDeliveryId}}, 0)))
        [NotMapped]
        public bool? ResultingDeliveryWasTransmitted
        {
            get => F.AsBool(F.Memo(this, "ResultingDeliveryWasTransmitted", () => F.Lookup<MessageDelivery>(this, "MessageDeliveries", "MessageDeliveryId", __c => __c.MessageDeliveries, __r => F.Of(__r.MessageDeliveryId), F.Of(this.ResultingDelivery), __r => F.Of(__r.WasActuallyTransmitted), () => F.Of(new MessageDelivery().WasActuallyTransmitted)))); set { }
        }

        // Formula IsOverriddenRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}})))
        [NotMapped]
        public bool? IsOverriddenRefusal
        {
            get => F.AsBool(F.Memo(this, "IsOverriddenRefusal", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.And(F.Bool3(F.Of(this.HasResultingDelivery)), F.Bool3(F.Of(this.ResultingDeliveryWasTransmitted))))))); set { }
        }

        // Formula IsSilentlyDropped (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{HasResultingDelivery}})))
        [NotMapped]
        public bool? IsSilentlyDropped
        {
            get => F.AsBool(F.Memo(this, "IsSilentlyDropped", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasResultingDelivery))))))); set { }
        }

        // Formula ResultingDeliveryException (rulebook: =INDEX(MessageDeliveries!{{InvokedException}}, MATCH({{ResultingDelivery}}, MessageDeliveries!{{MessageDeliveryId}}, 0)))
        [NotMapped]
        public string? ResultingDeliveryException
        {
            get => F.AsString(F.Memo(this, "ResultingDeliveryException", () => F.Lookup<MessageDelivery>(this, "MessageDeliveries", "MessageDeliveryId", __c => __c.MessageDeliveries, __r => F.Of(__r.MessageDeliveryId), F.Of(this.ResultingDelivery), __r => F.Of(__r.InvokedException), () => F.Of(new MessageDelivery().InvokedException)))); set { }
        }

        // Formula RefusalCitedAnException (rulebook: ={{ResultingDeliveryException}} <> "")
        [NotMapped]
        public bool? RefusalCitedAnException
        {
            get => F.AsBool(F.Memo(this, "RefusalCitedAnException", () => F.IsNotBlank(F.Of(this.ResultingDeliveryException)))); set { }
        }

        // Formula IsProperlyHandledRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, AND(NOT({{ResultingDeliveryWasTransmitted}}), {{RefusalCitedAnException}}))))
        [NotMapped]
        public bool? IsProperlyHandledRefusal
        {
            get => F.AsBool(F.Memo(this, "IsProperlyHandledRefusal", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.And(F.Bool3(F.Of(this.HasResultingDelivery)), F.Bool3(F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.ResultingDeliveryWasTransmitted)))), F.Bool3(F.Of(this.RefusalCitedAnException))))))))); set { }
        }

        // Formula RefusalFailureExecutionKey (rulebook: =IF(OR({{IsOverriddenRefusal}}, {{IsSilentlyDropped}}), {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? RefusalFailureExecutionKey
        {
            get => F.AsString(F.Memo(this, "RefusalFailureExecutionKey", () => (F.Truthy(F.Bool3(F.Or(F.Bool3(F.Of(this.IsOverriddenRefusal)), F.Bool3(F.Of(this.IsSilentlyDropped))))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula IntentExecutionKey (rulebook: ={{ProcedureExecution}})
        [NotMapped]
        public string? IntentExecutionKey
        {
            get => F.AsString(F.Memo(this, "IntentExecutionKey", () => F.Of(this.ProcedureExecution))); set { }
        }

        // Formula DeliveredIntentExecutionKey (rulebook: =IF(AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}), {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? DeliveredIntentExecutionKey
        {
            get => F.AsString(F.Memo(this, "DeliveredIntentExecutionKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Of(this.HasResultingDelivery)), F.Bool3(F.Of(this.ResultingDeliveryWasTransmitted))))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula DroppedIntentExecutionKey (rulebook: =IF({{IsSilentlyDropped}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? DroppedIntentExecutionKey
        {
            get => F.AsString(F.Memo(this, "DroppedIntentExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsSilentlyDropped))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula MyApprovalWasInForce (rulebook: ={{TemplateIsSendable}})
        [NotMapped]
        public bool? MyApprovalWasInForce
        {
            get => F.AsBool(F.Memo(this, "MyApprovalWasInForce", () => F.Of(this.TemplateIsSendable))); set { }
        }

        // Formula RefusedOnApprovedContent (rulebook: =AND({{MyApprovalWasInForce}}, NOT({{ContentGatePassed}})))
        [NotMapped]
        public bool? RefusedOnApprovedContent
        {
            get => F.AsBool(F.Memo(this, "RefusedOnApprovedContent", () => F.And(F.Bool3(F.Of(this.MyApprovalWasInForce)), F.Bool3(F.Not(F.Bool3(F.Of(this.ContentGatePassed))))))); set { }
        }

        // Formula RefusedOnOptOutOnly (rulebook: =AND(NOT({{OptOutGatePassed}}), {{LengthGatePassed}}))
        [NotMapped]
        public bool? RefusedOnOptOutOnly
        {
            get => F.AsBool(F.Memo(this, "RefusedOnOptOutOnly", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.OptOutGatePassed)))), F.Bool3(F.Of(this.LengthGatePassed))))); set { }
        }

        // Formula RefusalWasOnMyRules (rulebook: =AND(NOT({{IsClearedToSend}}), OR(NOT({{ContentGatePassed}}), NOT({{TimingGatePassed}}))))
        [NotMapped]
        public bool? RefusalWasOnMyRules
        {
            get => F.AsBool(F.Memo(this, "RefusalWasOnMyRules", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.ContentGatePassed)))), F.Bool3(F.Not(F.Bool3(F.Of(this.TimingGatePassed))))))))); set { }
        }

        // Formula RefusalWasOutsideMyControl (rulebook: =AND(NOT({{IsClearedToSend}}), OR(NOT({{PermissionGatePassed}}), NOT({{AuthorizationGatePassed}}))))
        [NotMapped]
        public bool? RefusalWasOutsideMyControl
        {
            get => F.AsBool(F.Memo(this, "RefusalWasOutsideMyControl", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.Or(F.Bool3(F.Not(F.Bool3(F.Of(this.PermissionGatePassed)))), F.Bool3(F.Not(F.Bool3(F.Of(this.AuthorizationGatePassed))))))))); set { }
        }

        public bool? ApproverWasNotified { get; set; }
        // Formula IsUnreportedRefusalOnMyRules (rulebook: =AND({{RefusalWasOnMyRules}}, NOT({{ApproverWasNotified}})))
        [NotMapped]
        public bool? IsUnreportedRefusalOnMyRules
        {
            get => F.AsBool(F.Memo(this, "IsUnreportedRefusalOnMyRules", () => F.And(F.Bool3(F.Of(this.RefusalWasOnMyRules)), F.Bool3(F.Not(F.IsTrueV(F.Of(this.ApproverWasNotified))))))); set { }
        }

        // Formula IsApprovalOverriddenSilently (rulebook: =AND({{RefusedOnApprovedContent}}, NOT({{ApproverWasNotified}})))
        [NotMapped]
        public bool? IsApprovalOverriddenSilently
        {
            get => F.AsBool(F.Memo(this, "IsApprovalOverriddenSilently", () => F.And(F.Bool3(F.Of(this.RefusedOnApprovedContent)), F.Bool3(F.Not(F.IsTrueV(F.Of(this.ApproverWasNotified))))))); set { }
        }

        public string? AlternateChannelIntent { get; set; }
        // Formula HasAlternateChannelAttempt (rulebook: ={{AlternateChannelIntent}} <> "")
        [NotMapped]
        public bool? HasAlternateChannelAttempt
        {
            get => F.AsBool(F.Memo(this, "HasAlternateChannelAttempt", () => F.IsNotBlank(F.Of(this.AlternateChannelIntent)))); set { }
        }

        // Formula AlternateAttemptWasCleared (rulebook: =INDEX(SendIntents!{{IsClearedToSend}}, MATCH({{AlternateChannelIntent}}, SendIntents!{{SendIntentId}}, 0)))
        [NotMapped]
        public bool? AlternateAttemptWasCleared
        {
            get => F.AsBool(F.Memo(this, "AlternateAttemptWasCleared", () => F.Lookup<SendIntent>(this, "SendIntents", "SendIntentId", __c => __c.SendIntents, __r => F.Of(__r.SendIntentId), F.Of(this.AlternateChannelIntent), __r => F.Of(__r.IsClearedToSend), () => F.Of(new SendIntent().IsClearedToSend)))); set { }
        }

        // Formula IsRefusedWithNoAlternative (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{HasAlternateChannelAttempt}})))
        [NotMapped]
        public bool? IsRefusedWithNoAlternative
        {
            get => F.AsBool(F.Memo(this, "IsRefusedWithNoAlternative", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasAlternateChannelAttempt))))))); set { }
        }

        // Formula ExceptionPrescribedAnAlternative (rulebook: =AND({{RefusalCitedAnException}}, {{ResultingDeliveryException}} <> ""))
        [NotMapped]
        public bool? ExceptionPrescribedAnAlternative
        {
            get => F.AsBool(F.Memo(this, "ExceptionPrescribedAnAlternative", () => F.And(F.Bool3(F.Of(this.RefusalCitedAnException)), F.Bool3(F.IsNotBlank(F.Of(this.ResultingDeliveryException)))))); set { }
        }

        // Formula PrescribedHandlingWasPerformed (rulebook: =AND({{ExceptionPrescribedAnAlternative}}, AND({{HasAlternateChannelAttempt}}, {{AlternateAttemptWasCleared}})))
        [NotMapped]
        public bool? PrescribedHandlingWasPerformed
        {
            get => F.AsBool(F.Memo(this, "PrescribedHandlingWasPerformed", () => F.And(F.Bool3(F.Of(this.ExceptionPrescribedAnAlternative)), F.Bool3(F.And(F.Bool3(F.Of(this.HasAlternateChannelAttempt)), F.Bool3(F.Of(this.AlternateAttemptWasCleared))))))); set { }
        }

        // Formula IsSuppressionWithoutRemedy (rulebook: =AND({{ExceptionPrescribedAnAlternative}}, NOT({{PrescribedHandlingWasPerformed}})))
        [NotMapped]
        public bool? IsSuppressionWithoutRemedy
        {
            get => F.AsBool(F.Memo(this, "IsSuppressionWithoutRemedy", () => F.And(F.Bool3(F.Of(this.ExceptionPrescribedAnAlternative)), F.Bool3(F.Not(F.Bool3(F.Of(this.PrescribedHandlingWasPerformed))))))); set { }
        }

        public DateTimeOffset? RefusalRecordedAt { get; set; }
        // Formula HasDurableRefusalRecord (rulebook: ={{RefusalRecordedAt}} <> "")
        [NotMapped]
        public bool? HasDurableRefusalRecord
        {
            get => F.AsBool(F.Memo(this, "HasDurableRefusalRecord", () => F.IsNotBlank(F.Of(this.RefusalRecordedAt)))); set { }
        }

        // Formula RefusalWasEscalated (rulebook: ={{RefusalNotifiedRole}} <> "")
        [NotMapped]
        public bool? RefusalWasEscalated
        {
            get => F.AsBool(F.Memo(this, "RefusalWasEscalated", () => F.IsNotBlank(F.Of(this.RefusalNotifiedRole)))); set { }
        }

        // Formula IsUnrecordedRefusal (rulebook: =AND({{IsSilentlyDropped}}, AND(NOT({{HasDurableRefusalRecord}}), NOT({{RefusalCitedAnException}}))))
        [NotMapped]
        public bool? IsUnrecordedRefusal
        {
            get => F.AsBool(F.Memo(this, "IsUnrecordedRefusal", () => F.And(F.Bool3(F.Of(this.IsSilentlyDropped)), F.Bool3(F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.HasDurableRefusalRecord)))), F.Bool3(F.Not(F.Bool3(F.Of(this.RefusalCitedAnException))))))))); set { }
        }

        // Formula IsUnescalatedRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{RefusalWasEscalated}})))
        [NotMapped]
        public bool? IsUnescalatedRefusal
        {
            get => F.AsBool(F.Memo(this, "IsUnescalatedRefusal", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.Not(F.Bool3(F.Of(this.RefusalWasEscalated))))))); set { }
        }

        // Formula UnescalatedRefusalRoleKey (rulebook: =IF({{IsUnrecordedRefusal}}, {{RefusalNotifiedRole}}, ""))
        [NotMapped]
        public string? UnescalatedRefusalRoleKey
        {
            get => F.AsString(F.Memo(this, "UnescalatedRefusalRoleKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnrecordedRefusal))) ? F.Of(this.RefusalNotifiedRole) : F.S("")))); set { }
        }

        // Formula UnrecordedRefusalExecutionKey (rulebook: =IF({{IsUnrecordedRefusal}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? UnrecordedRefusalExecutionKey
        {
            get => F.AsString(F.Memo(this, "UnrecordedRefusalExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsUnrecordedRefusal))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        public string? RetryIntent { get; set; }
        // Formula WasDeferredOnTiming (rulebook: =AND(NOT({{TimingGatePassed}}), AND({{PermissionGatePassed}}, {{ContentGatePassed}})))
        [NotMapped]
        public bool? WasDeferredOnTiming
        {
            get => F.AsBool(F.Memo(this, "WasDeferredOnTiming", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.TimingGatePassed)))), F.Bool3(F.And(F.Bool3(F.Of(this.PermissionGatePassed)), F.Bool3(F.Of(this.ContentGatePassed))))))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula WindowHasSinceReopened (rulebook: =AND({{HoursUntilWindowOpens}} > 0, DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours") > {{HoursUntilWindowOpens}}))
        [NotMapped]
        public bool? WindowHasSinceReopened
        {
            get => F.AsBool(F.Memo(this, "WindowHasSinceReopened", () => F.And(F.Bool3(F.Cmp(F.Of(this.HoursUntilWindowOpens), ">", F.I(0))), F.Bool3(F.Cmp(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.EvaluatedAt), F.S("hours")), ">", F.Of(this.HoursUntilWindowOpens)))))); set { }
        }

        // Formula HasRetryAttempt (rulebook: ={{RetryIntent}} <> "")
        [NotMapped]
        public bool? HasRetryAttempt
        {
            get => F.AsBool(F.Memo(this, "HasRetryAttempt", () => F.IsNotBlank(F.Of(this.RetryIntent)))); set { }
        }

        // Formula RetryWasCleared (rulebook: =INDEX(SendIntents!{{IsClearedToSend}}, MATCH({{RetryIntent}}, SendIntents!{{SendIntentId}}, 0)))
        [NotMapped]
        public bool? RetryWasCleared
        {
            get => F.AsBool(F.Memo(this, "RetryWasCleared", () => F.Lookup<SendIntent>(this, "SendIntents", "SendIntentId", __c => __c.SendIntents, __r => F.Of(__r.SendIntentId), F.Of(this.RetryIntent), __r => F.Of(__r.IsClearedToSend), () => F.Of(new SendIntent().IsClearedToSend)))); set { }
        }

        // Formula IsAbandonedDeferral (rulebook: =AND({{WasDeferredOnTiming}}, AND({{WindowHasSinceReopened}}, NOT({{HasRetryAttempt}}))))
        [NotMapped]
        public bool? IsAbandonedDeferral
        {
            get => F.AsBool(F.Memo(this, "IsAbandonedDeferral", () => F.And(F.Bool3(F.Of(this.WasDeferredOnTiming)), F.Bool3(F.And(F.Bool3(F.Of(this.WindowHasSinceReopened)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasRetryAttempt))))))))); set { }
        }

        // Formula DeferralAgeHours (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours"))
        [NotMapped]
        public int? DeferralAgeHours
        {
            get => F.AsInt(F.Memo(this, "DeferralAgeHours", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.EvaluatedAt), F.S("hours"))))); set { }
        }

        // Formula IsStaleDeferral (rulebook: =AND({{WasDeferredOnTiming}}, {{DeferralAgeHours}} > 24))
        [NotMapped]
        public bool? IsStaleDeferral
        {
            get => F.AsBool(F.Memo(this, "IsStaleDeferral", () => F.And(F.Bool3(F.Of(this.WasDeferredOnTiming)), F.Bool3(F.Cmp(F.Of(this.DeferralAgeHours), ">", F.I(24)))))); set { }
        }

        public string? EvaluatingRoleAssignment { get; set; }
        // Formula EnforcedByUnauthorizedAgent (rulebook: =INDEX(RoleAssignments!{{IsUnauthorizedEnforcementAgent}}, MATCH({{EvaluatingRoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        [NotMapped]
        public bool? EnforcedByUnauthorizedAgent
        {
            get => F.AsBool(F.Memo(this, "EnforcedByUnauthorizedAgent", () => F.Lookup<RoleAssignment>(this, "RoleAssignments", "RoleAssignmentId", __c => __c.RoleAssignments, __r => F.Of(__r.RoleAssignmentId), F.Of(this.EvaluatingRoleAssignment), __r => F.Of(__r.IsUnauthorizedEnforcementAgent), () => F.Of(new RoleAssignment().IsUnauthorizedEnforcementAgent)))); set { }
        }

        // Formula ConsentInputWasResolvable (rulebook: ={{RecipientConsentStatusRaw}} <> "")
        [NotMapped]
        public bool? ConsentInputWasResolvable
        {
            get => F.AsBool(F.Memo(this, "ConsentInputWasResolvable", () => F.IsNotBlank(F.Of(this.RecipientConsentStatusRaw)))); set { }
        }

        // Formula RecipientConsentStatusRaw (rulebook: =INDEX(Recipients!{{SmsConsentStatus}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        [NotMapped]
        public string? RecipientConsentStatusRaw
        {
            get => F.AsString(F.Memo(this, "RecipientConsentStatusRaw", () => F.Lookup<Recipient>(this, "Recipients", "RecipientId", __c => __c.Recipients, __r => F.Of(__r.RecipientId), F.Of(this.Recipient), __r => F.Of(__r.SmsConsentStatus), () => F.Of(new Recipient().SmsConsentStatus)))); set { }
        }

        // Formula PolicyInputWasResolvable (rulebook: ={{IntentPolicy}} <> "")
        [NotMapped]
        public bool? PolicyInputWasResolvable
        {
            get => F.AsBool(F.Memo(this, "PolicyInputWasResolvable", () => F.IsNotBlank(F.Of(this.IntentPolicy)))); set { }
        }

        // Formula AllGateInputsResolved (rulebook: =AND({{ConsentInputWasResolvable}}, {{PolicyInputWasResolvable}}))
        [NotMapped]
        public bool? AllGateInputsResolved
        {
            get => F.AsBool(F.Memo(this, "AllGateInputsResolved", () => F.And(F.Bool3(F.Of(this.ConsentInputWasResolvable)), F.Bool3(F.Of(this.PolicyInputWasResolvable))))); set { }
        }

        // Formula IsUnevaluableRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{AllGateInputsResolved}})))
        [NotMapped]
        public bool? IsUnevaluableRefusal
        {
            get => F.AsBool(F.Memo(this, "IsUnevaluableRefusal", () => F.And(F.Bool3(F.Not(F.Bool3(F.Of(this.IsClearedToSend)))), F.Bool3(F.Not(F.Bool3(F.Of(this.AllGateInputsResolved))))))); set { }
        }

        public bool? GateResultWasIndependentlyConfirmed { get; set; }
        // Formula IsSelfWitnessedDecision (rulebook: =NOT({{GateResultWasIndependentlyConfirmed}}))
        [NotMapped]
        public bool? IsSelfWitnessedDecision
        {
            get => F.AsBool(F.Memo(this, "IsSelfWitnessedDecision", () => F.Not(F.IsTrueV(F.Of(this.GateResultWasIndependentlyConfirmed))))); set { }
        }

        // Formula IsIndependentlyConfirmed (rulebook: =AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}))
        [NotMapped]
        public bool? IsIndependentlyConfirmed
        {
            get => F.AsBool(F.Memo(this, "IsIndependentlyConfirmed", () => F.And(F.Bool3(F.Of(this.HasResultingDelivery)), F.Bool3(F.Of(this.ResultingDeliveryWasTransmitted))))); set { }
        }

        // Formula IndependentlyConfirmedExecutionKey (rulebook: =IF({{IsIndependentlyConfirmed}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? IndependentlyConfirmedExecutionKey
        {
            get => F.AsString(F.Memo(this, "IndependentlyConfirmedExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsIndependentlyConfirmed))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? StepExecution { get; set; }
        public string? Recipient { get; set; }
        public string? MessageTemplate { get; set; }
        public string? ResultingDelivery { get; set; }
        public string? RefusalNotifiedRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        private Recipient _recipientRef;

        [ForeignKey("Recipient")]
        public virtual Recipient RecipientRef
        {
            get
            {
                if (_recipientRef == null && !string.IsNullOrEmpty(Recipient))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RecipientRef - no database context is set. Recipient: " + Recipient + ".");
                        }
                        return null;
                    }
                    _recipientRef = base.SoAContext.Recipients.Find(Recipient);
                    if (_recipientRef != null)
                    {
                        base.SoAContext.Attach(_recipientRef);
                    }
                }
                return _recipientRef;
            }
            set
            {
                if (_recipientRef != value)
                {
                    _recipientRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_recipientRef != null)
                    {
                        Recipient = _recipientRef.RecipientId;
                    }
                }
            }
        }

        private MessageTemplate _messageTemplateRef;

        [ForeignKey("MessageTemplate")]
        public virtual MessageTemplate MessageTemplateRef
        {
            get
            {
                if (_messageTemplateRef == null && !string.IsNullOrEmpty(MessageTemplate))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageTemplateRef - no database context is set. MessageTemplate: " + MessageTemplate + ".");
                        }
                        return null;
                    }
                    _messageTemplateRef = base.SoAContext.MessageTemplates.Find(MessageTemplate);
                    if (_messageTemplateRef != null)
                    {
                        base.SoAContext.Attach(_messageTemplateRef);
                    }
                }
                return _messageTemplateRef;
            }
            set
            {
                if (_messageTemplateRef != value)
                {
                    _messageTemplateRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_messageTemplateRef != null)
                    {
                        MessageTemplate = _messageTemplateRef.MessageTemplateId;
                    }
                }
            }
        }

        private MessageDelivery _messageDelivery;

        [ForeignKey("ResultingDelivery")]
        public virtual MessageDelivery MessageDelivery
        {
            get
            {
                if (_messageDelivery == null && !string.IsNullOrEmpty(ResultingDelivery))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDelivery - no database context is set. ResultingDelivery: " + ResultingDelivery + ".");
                        }
                        return null;
                    }
                    _messageDelivery = base.SoAContext.MessageDeliveries.Find(ResultingDelivery);
                    if (_messageDelivery != null)
                    {
                        base.SoAContext.Attach(_messageDelivery);
                    }
                }
                return _messageDelivery;
            }
            set
            {
                if (_messageDelivery != value)
                {
                    _messageDelivery = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_messageDelivery != null)
                    {
                        ResultingDelivery = _messageDelivery.MessageDeliveryId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("RefusalNotifiedRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(RefusalNotifiedRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. RefusalNotifiedRole: " + RefusalNotifiedRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(RefusalNotifiedRole);
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
                        RefusalNotifiedRole = _role.RoleId;
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
            _ = this.ProcedureExecutionRef;
            _ = this.StepExecutionRef;
            _ = this.RecipientRef;
            _ = this.MessageTemplateRef;
            _ = this.MessageDelivery;
            _ = this.Role;
            _ = this.EvaluationContextRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
