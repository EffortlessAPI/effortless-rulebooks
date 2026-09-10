
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("SendIntents")]
    public class SendIntentBase : SoAEntityBase
    {
        [Key]
        public string SendIntentId { get; set; }

        // Formula Name (rulebook: ={{Recipient}} & " / " & {{MessageTemplate}} & " / intent")
        public string? Name
        {
            get => this.Recipient + " / " + this.MessageTemplate + " / intent"; set { }
        }

        public string? ProposedBody { get; set; }
        public int? ProposedSendAtLocalHour { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        // Formula IntentPolicy (rulebook: =INDEX(MessageTemplates!{{CommunicationPolicy}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public string? IntentPolicy
        {
            get => INDEX(MessageTemplates!this.CommunicationPolicy, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula IntentChannel (rulebook: =INDEX(CommunicationPolicies!{{Channel}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public string? IntentChannel
        {
            get => INDEX(CommunicationPolicies!this.Channel, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula PolicyIsActive (rulebook: =INDEX(CommunicationPolicies!{{IsActivePolicy}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public bool? PolicyIsActive
        {
            get => INDEX(CommunicationPolicies!this.IsActivePolicy, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula IntentRequiresConsent (rulebook: =INDEX(CommunicationPolicies!{{ConsentRequired}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public bool? IntentRequiresConsent
        {
            get => INDEX(CommunicationPolicies!this.ConsentRequired, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula RecipientHasChannelConsent (rulebook: =INDEX(Recipients!{{HasSmsConsent}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        public bool? RecipientHasChannelConsent
        {
            get => INDEX(Recipients!this.HasSmsConsent, MATCH(this.Recipient, Recipients!this.RecipientId, 0)); set { }
        }

        // Formula ConsentGatePassed (rulebook: =OR(NOT({{IntentRequiresConsent}}), {{RecipientHasChannelConsent}}))
        public bool? ConsentGatePassed
        {
            get => OR(NOT(this.IntentRequiresConsent), this.RecipientHasChannelConsent); set { }
        }

        // Formula RecipientIsSmsReachable (rulebook: =INDEX(Recipients!{{IsSmsReachable}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        public bool? RecipientIsSmsReachable
        {
            get => INDEX(Recipients!this.IsSmsReachable, MATCH(this.Recipient, Recipients!this.RecipientId, 0)); set { }
        }

        // Formula RecipientIsEmailReachable (rulebook: =INDEX(Recipients!{{IsEmailReachable}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        public bool? RecipientIsEmailReachable
        {
            get => INDEX(Recipients!this.IsEmailReachable, MATCH(this.Recipient, Recipients!this.RecipientId, 0)); set { }
        }

        // Formula ReachabilityGatePassed (rulebook: =IF({{IntentChannel}} = "SMS", {{RecipientIsSmsReachable}}, {{RecipientIsEmailReachable}}))
        public bool? ReachabilityGatePassed
        {
            get => IF(this.IntentChannel = "SMS", this.RecipientIsSmsReachable, this.RecipientIsEmailReachable); set { }
        }

        // Formula PermissionGatePassed (rulebook: =AND({{PolicyIsActive}}, AND({{ConsentGatePassed}}, {{ReachabilityGatePassed}})))
        public bool? PermissionGatePassed
        {
            get => AND(this.PolicyIsActive, AND(this.ConsentGatePassed, this.ReachabilityGatePassed)); set { }
        }

        // Formula IntentQuietStartHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursStartHour}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? IntentQuietStartHour
        {
            get => INDEX(CommunicationPolicies!this.QuietHoursStartHour, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula IntentQuietEndHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursEndHour}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? IntentQuietEndHour
        {
            get => INDEX(CommunicationPolicies!this.QuietHoursEndHour, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula IntentPolicyHasQuietHours (rulebook: ={{IntentQuietStartHour}} <> {{IntentQuietEndHour}})
        public bool? IntentPolicyHasQuietHours
        {
            get => this.IntentQuietStartHour <> this.IntentQuietEndHour; set { }
        }

        // Formula IntentQuietWindowWraps (rulebook: ={{IntentQuietStartHour}} > {{IntentQuietEndHour}})
        public bool? IntentQuietWindowWraps
        {
            get => this.IntentQuietStartHour > this.IntentQuietEndHour; set { }
        }

        // Formula IntentIsInsideQuietWindow (rulebook: =IF({{IntentQuietWindowWraps}}, OR({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}), AND({{ProposedSendAtLocalHour}} >= {{IntentQuietStartHour}}, {{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}})))
        public bool? IntentIsInsideQuietWindow
        {
            get => IF(this.IntentQuietWindowWraps, OR(this.ProposedSendAtLocalHour >= this.IntentQuietStartHour, this.ProposedSendAtLocalHour < this.IntentQuietEndHour), AND(this.ProposedSendAtLocalHour >= this.IntentQuietStartHour, this.ProposedSendAtLocalHour < this.IntentQuietEndHour)); set { }
        }

        // Formula TimingGatePassed (rulebook: =OR(NOT({{IntentPolicyHasQuietHours}}), NOT({{IntentIsInsideQuietWindow}})))
        public bool? TimingGatePassed
        {
            get => OR(NOT(this.IntentPolicyHasQuietHours), NOT(this.IntentIsInsideQuietWindow)); set { }
        }

        // Formula HoursUntilWindowOpens (rulebook: =IF({{TimingGatePassed}}, 0, IF({{ProposedSendAtLocalHour}} < {{IntentQuietEndHour}}, {{IntentQuietEndHour}} - {{ProposedSendAtLocalHour}}, 24 - {{ProposedSendAtLocalHour}} + {{IntentQuietEndHour}})))
        public int? HoursUntilWindowOpens
        {
            get => IF(this.TimingGatePassed, 0, IF(this.ProposedSendAtLocalHour < this.IntentQuietEndHour, this.IntentQuietEndHour - this.ProposedSendAtLocalHour, 24 - this.ProposedSendAtLocalHour + this.IntentQuietEndHour)); set { }
        }

        // Formula IntentMaxMessageLength (rulebook: =INDEX(CommunicationPolicies!{{MaxMessageLength}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? IntentMaxMessageLength
        {
            get => INDEX(CommunicationPolicies!this.MaxMessageLength, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula IntentMaxSegments (rulebook: =INDEX(CommunicationPolicies!{{MaxSegments}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? IntentMaxSegments
        {
            get => INDEX(CommunicationPolicies!this.MaxSegments, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        public int? ProposedBodyLength { get; set; }
        public int? ProposedSegmentCount { get; set; }
        // Formula LengthGatePassed (rulebook: =AND({{ProposedBodyLength}} > 0, {{ProposedSegmentCount}} <= {{IntentMaxSegments}}))
        public bool? LengthGatePassed
        {
            get => AND(this.ProposedBodyLength > 0, this.ProposedSegmentCount <= this.IntentMaxSegments); set { }
        }

        // Formula IntentRequiredOptOutPhrase (rulebook: =INDEX(CommunicationPolicies!{{RequiredOptOutPhrase}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public string? IntentRequiredOptOutPhrase
        {
            get => INDEX(CommunicationPolicies!this.RequiredOptOutPhrase, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        public int? ProposedOptOutPosition { get; set; }
        // Formula OptOutGatePassed (rulebook: =OR({{IntentRequiredOptOutPhrase}} = "", AND({{ProposedOptOutPosition}} > 0, {{ProposedOptOutPosition}} <= {{IntentMaxMessageLength}})))
        public bool? OptOutGatePassed
        {
            get => OR(this.IntentRequiredOptOutPhrase = "", AND(this.ProposedOptOutPosition > 0, this.ProposedOptOutPosition <= this.IntentMaxMessageLength)); set { }
        }

        // Formula ContentGatePassed (rulebook: =AND({{LengthGatePassed}}, {{OptOutGatePassed}}))
        public bool? ContentGatePassed
        {
            get => AND(this.LengthGatePassed, this.OptOutGatePassed); set { }
        }

        // Formula TemplateIsSendable (rulebook: =INDEX(MessageTemplates!{{IsSendableUnderApproval}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public bool? TemplateIsSendable
        {
            get => INDEX(MessageTemplates!this.IsSendableUnderApproval, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula ExecutionHasLegalClearance (rulebook: =INDEX(ProcedureExecutions!{{HasClearedLegalReview}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        public bool? ExecutionHasLegalClearance
        {
            get => INDEX(ProcedureExecutions!this.HasClearedLegalReview, MATCH(this.ProcedureExecution, ProcedureExecutions!this.ProcedureExecutionId, 0)); set { }
        }

        // Formula IntentApprovalRole (rulebook: =INDEX(CommunicationPolicies!{{ApprovalRole}}, MATCH({{IntentPolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public string? IntentApprovalRole
        {
            get => INDEX(CommunicationPolicies!this.ApprovalRole, MATCH(this.IntentPolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula ApprovalRoleAgentKind (rulebook: =INDEX(Roles!{{CurrentAgentKind}}, MATCH({{IntentApprovalRole}}, Roles!{{RoleId}}, 0)))
        public string? ApprovalRoleAgentKind
        {
            get => INDEX(Roles!this.CurrentAgentKind, MATCH(this.IntentApprovalRole, Roles!this.RoleId, 0)); set { }
        }

        // Formula ApprovalIsHuman (rulebook: ={{ApprovalRoleAgentKind}} = "Human")
        public bool? ApprovalIsHuman
        {
            get => this.ApprovalRoleAgentKind = "Human"; set { }
        }

        // Formula AuthorizationGatePassed (rulebook: =AND({{TemplateIsSendable}}, AND({{ExecutionHasLegalClearance}}, {{ApprovalIsHuman}})))
        public bool? AuthorizationGatePassed
        {
            get => AND(this.TemplateIsSendable, AND(this.ExecutionHasLegalClearance, this.ApprovalIsHuman)); set { }
        }

        // Formula IsClearedToSend (rulebook: =AND({{PermissionGatePassed}}, AND({{TimingGatePassed}}, AND({{ContentGatePassed}}, {{AuthorizationGatePassed}}))))
        public bool? IsClearedToSend
        {
            get => AND(this.PermissionGatePassed, AND(this.TimingGatePassed, AND(this.ContentGatePassed, this.AuthorizationGatePassed))); set { }
        }

        // Formula BlockingGateName (rulebook: =IF({{IsClearedToSend}}, "", IF(NOT({{PermissionGatePassed}}), "Permission", IF(NOT({{TimingGatePassed}}), "Timing", IF(NOT({{ContentGatePassed}}), "Content", "Authorization")))))
        public string? BlockingGateName
        {
            get => IF(this.IsClearedToSend, "", IF(NOT(this.PermissionGatePassed), "Permission", IF(NOT(this.TimingGatePassed), "Timing", IF(NOT(this.ContentGatePassed), "Content", "Authorization")))); set { }
        }

        // Formula HasResultingDelivery (rulebook: ={{ResultingDelivery}} <> "")
        public bool? HasResultingDelivery
        {
            get => this.ResultingDelivery <> ""; set { }
        }

        // Formula ResultingDeliveryWasTransmitted (rulebook: =INDEX(MessageDeliveries!{{WasActuallyTransmitted}}, MATCH({{ResultingDelivery}}, MessageDeliveries!{{MessageDeliveryId}}, 0)))
        public bool? ResultingDeliveryWasTransmitted
        {
            get => INDEX(MessageDeliveries!this.WasActuallyTransmitted, MATCH(this.ResultingDelivery, MessageDeliveries!this.MessageDeliveryId, 0)); set { }
        }

        // Formula IsOverriddenRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}})))
        public bool? IsOverriddenRefusal
        {
            get => AND(NOT(this.IsClearedToSend), AND(this.HasResultingDelivery, this.ResultingDeliveryWasTransmitted)); set { }
        }

        // Formula IsSilentlyDropped (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{HasResultingDelivery}})))
        public bool? IsSilentlyDropped
        {
            get => AND(NOT(this.IsClearedToSend), NOT(this.HasResultingDelivery)); set { }
        }

        // Formula ResultingDeliveryException (rulebook: =INDEX(MessageDeliveries!{{InvokedException}}, MATCH({{ResultingDelivery}}, MessageDeliveries!{{MessageDeliveryId}}, 0)))
        public string? ResultingDeliveryException
        {
            get => INDEX(MessageDeliveries!this.InvokedException, MATCH(this.ResultingDelivery, MessageDeliveries!this.MessageDeliveryId, 0)); set { }
        }

        // Formula RefusalCitedAnException (rulebook: ={{ResultingDeliveryException}} <> "")
        public bool? RefusalCitedAnException
        {
            get => this.ResultingDeliveryException <> ""; set { }
        }

        // Formula IsProperlyHandledRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), AND({{HasResultingDelivery}}, AND(NOT({{ResultingDeliveryWasTransmitted}}), {{RefusalCitedAnException}}))))
        public bool? IsProperlyHandledRefusal
        {
            get => AND(NOT(this.IsClearedToSend), AND(this.HasResultingDelivery, AND(NOT(this.ResultingDeliveryWasTransmitted), this.RefusalCitedAnException))); set { }
        }

        // Formula RefusalFailureExecutionKey (rulebook: =IF(OR({{IsOverriddenRefusal}}, {{IsSilentlyDropped}}), {{ProcedureExecution}}, ""))
        public string? RefusalFailureExecutionKey
        {
            get => IF(OR(this.IsOverriddenRefusal, this.IsSilentlyDropped), this.ProcedureExecution, ""); set { }
        }

        // Formula IntentExecutionKey (rulebook: ={{ProcedureExecution}})
        public string? IntentExecutionKey
        {
            get => this.ProcedureExecution; set { }
        }

        // Formula DeliveredIntentExecutionKey (rulebook: =IF(AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}), {{ProcedureExecution}}, ""))
        public string? DeliveredIntentExecutionKey
        {
            get => IF(AND(this.HasResultingDelivery, this.ResultingDeliveryWasTransmitted), this.ProcedureExecution, ""); set { }
        }

        // Formula DroppedIntentExecutionKey (rulebook: =IF({{IsSilentlyDropped}}, {{ProcedureExecution}}, ""))
        public string? DroppedIntentExecutionKey
        {
            get => IF(this.IsSilentlyDropped, this.ProcedureExecution, ""); set { }
        }

        // Formula MyApprovalWasInForce (rulebook: ={{TemplateIsSendable}})
        public bool? MyApprovalWasInForce
        {
            get => this.TemplateIsSendable; set { }
        }

        // Formula RefusedOnApprovedContent (rulebook: =AND({{MyApprovalWasInForce}}, NOT({{ContentGatePassed}})))
        public bool? RefusedOnApprovedContent
        {
            get => AND(this.MyApprovalWasInForce, NOT(this.ContentGatePassed)); set { }
        }

        // Formula RefusedOnOptOutOnly (rulebook: =AND(NOT({{OptOutGatePassed}}), {{LengthGatePassed}}))
        public bool? RefusedOnOptOutOnly
        {
            get => AND(NOT(this.OptOutGatePassed), this.LengthGatePassed); set { }
        }

        // Formula RefusalWasOnMyRules (rulebook: =AND(NOT({{IsClearedToSend}}), OR(NOT({{ContentGatePassed}}), NOT({{TimingGatePassed}}))))
        public bool? RefusalWasOnMyRules
        {
            get => AND(NOT(this.IsClearedToSend), OR(NOT(this.ContentGatePassed), NOT(this.TimingGatePassed))); set { }
        }

        // Formula RefusalWasOutsideMyControl (rulebook: =AND(NOT({{IsClearedToSend}}), OR(NOT({{PermissionGatePassed}}), NOT({{AuthorizationGatePassed}}))))
        public bool? RefusalWasOutsideMyControl
        {
            get => AND(NOT(this.IsClearedToSend), OR(NOT(this.PermissionGatePassed), NOT(this.AuthorizationGatePassed))); set { }
        }

        public bool? ApproverWasNotified { get; set; }
        // Formula IsUnreportedRefusalOnMyRules (rulebook: =AND({{RefusalWasOnMyRules}}, NOT({{ApproverWasNotified}})))
        public bool? IsUnreportedRefusalOnMyRules
        {
            get => AND(this.RefusalWasOnMyRules, NOT(this.ApproverWasNotified)); set { }
        }

        // Formula IsApprovalOverriddenSilently (rulebook: =AND({{RefusedOnApprovedContent}}, NOT({{ApproverWasNotified}})))
        public bool? IsApprovalOverriddenSilently
        {
            get => AND(this.RefusedOnApprovedContent, NOT(this.ApproverWasNotified)); set { }
        }

        public string? AlternateChannelIntent { get; set; }
        // Formula HasAlternateChannelAttempt (rulebook: ={{AlternateChannelIntent}} <> "")
        public bool? HasAlternateChannelAttempt
        {
            get => this.AlternateChannelIntent <> ""; set { }
        }

        // Formula AlternateAttemptWasCleared (rulebook: =INDEX(SendIntents!{{IsClearedToSend}}, MATCH({{AlternateChannelIntent}}, SendIntents!{{SendIntentId}}, 0)))
        public bool? AlternateAttemptWasCleared
        {
            get => INDEX(SendIntents!this.IsClearedToSend, MATCH(this.AlternateChannelIntent, SendIntents!this.SendIntentId, 0)); set { }
        }

        // Formula IsRefusedWithNoAlternative (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{HasAlternateChannelAttempt}})))
        public bool? IsRefusedWithNoAlternative
        {
            get => AND(NOT(this.IsClearedToSend), NOT(this.HasAlternateChannelAttempt)); set { }
        }

        // Formula ExceptionPrescribedAnAlternative (rulebook: =AND({{RefusalCitedAnException}}, {{ResultingDeliveryException}} <> ""))
        public bool? ExceptionPrescribedAnAlternative
        {
            get => AND(this.RefusalCitedAnException, this.ResultingDeliveryException <> ""); set { }
        }

        // Formula PrescribedHandlingWasPerformed (rulebook: =AND({{ExceptionPrescribedAnAlternative}}, AND({{HasAlternateChannelAttempt}}, {{AlternateAttemptWasCleared}})))
        public bool? PrescribedHandlingWasPerformed
        {
            get => AND(this.ExceptionPrescribedAnAlternative, AND(this.HasAlternateChannelAttempt, this.AlternateAttemptWasCleared)); set { }
        }

        // Formula IsSuppressionWithoutRemedy (rulebook: =AND({{ExceptionPrescribedAnAlternative}}, NOT({{PrescribedHandlingWasPerformed}})))
        public bool? IsSuppressionWithoutRemedy
        {
            get => AND(this.ExceptionPrescribedAnAlternative, NOT(this.PrescribedHandlingWasPerformed)); set { }
        }

        public DateTime? RefusalRecordedAt { get; set; }
        // Formula HasDurableRefusalRecord (rulebook: ={{RefusalRecordedAt}} <> "")
        public bool? HasDurableRefusalRecord
        {
            get => this.RefusalRecordedAt <> ""; set { }
        }

        // Formula RefusalWasEscalated (rulebook: ={{RefusalNotifiedRole}} <> "")
        public bool? RefusalWasEscalated
        {
            get => this.RefusalNotifiedRole <> ""; set { }
        }

        // Formula IsUnrecordedRefusal (rulebook: =AND({{IsSilentlyDropped}}, AND(NOT({{HasDurableRefusalRecord}}), NOT({{RefusalCitedAnException}}))))
        public bool? IsUnrecordedRefusal
        {
            get => AND(this.IsSilentlyDropped, AND(NOT(this.HasDurableRefusalRecord), NOT(this.RefusalCitedAnException))); set { }
        }

        // Formula IsUnescalatedRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{RefusalWasEscalated}})))
        public bool? IsUnescalatedRefusal
        {
            get => AND(NOT(this.IsClearedToSend), NOT(this.RefusalWasEscalated)); set { }
        }

        // Formula UnescalatedRefusalRoleKey (rulebook: =IF({{IsUnrecordedRefusal}}, {{RefusalNotifiedRole}}, ""))
        public string? UnescalatedRefusalRoleKey
        {
            get => IF(this.IsUnrecordedRefusal, this.RefusalNotifiedRole, ""); set { }
        }

        // Formula UnrecordedRefusalExecutionKey (rulebook: =IF({{IsUnrecordedRefusal}}, {{ProcedureExecution}}, ""))
        public string? UnrecordedRefusalExecutionKey
        {
            get => IF(this.IsUnrecordedRefusal, this.ProcedureExecution, ""); set { }
        }

        public string? RetryIntent { get; set; }
        // Formula WasDeferredOnTiming (rulebook: =AND(NOT({{TimingGatePassed}}), AND({{PermissionGatePassed}}, {{ContentGatePassed}})))
        public bool? WasDeferredOnTiming
        {
            get => AND(NOT(this.TimingGatePassed), AND(this.PermissionGatePassed, this.ContentGatePassed)); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula WindowHasSinceReopened (rulebook: =AND({{HoursUntilWindowOpens}} > 0, DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours") > {{HoursUntilWindowOpens}}))
        public bool? WindowHasSinceReopened
        {
            get => AND(this.HoursUntilWindowOpens > 0, DATETIME_DIFF(this.AsOfInstant, this.EvaluatedAt, "hours") > this.HoursUntilWindowOpens); set { }
        }

        // Formula HasRetryAttempt (rulebook: ={{RetryIntent}} <> "")
        public bool? HasRetryAttempt
        {
            get => this.RetryIntent <> ""; set { }
        }

        // Formula RetryWasCleared (rulebook: =INDEX(SendIntents!{{IsClearedToSend}}, MATCH({{RetryIntent}}, SendIntents!{{SendIntentId}}, 0)))
        public bool? RetryWasCleared
        {
            get => INDEX(SendIntents!this.IsClearedToSend, MATCH(this.RetryIntent, SendIntents!this.SendIntentId, 0)); set { }
        }

        // Formula IsAbandonedDeferral (rulebook: =AND({{WasDeferredOnTiming}}, AND({{WindowHasSinceReopened}}, NOT({{HasRetryAttempt}}))))
        public bool? IsAbandonedDeferral
        {
            get => AND(this.WasDeferredOnTiming, AND(this.WindowHasSinceReopened, NOT(this.HasRetryAttempt))); set { }
        }

        // Formula DeferralAgeHours (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{EvaluatedAt}}, "hours"))
        public int? DeferralAgeHours
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.EvaluatedAt, "hours"); set { }
        }

        // Formula IsStaleDeferral (rulebook: =AND({{WasDeferredOnTiming}}, {{DeferralAgeHours}} > 24))
        public bool? IsStaleDeferral
        {
            get => AND(this.WasDeferredOnTiming, this.DeferralAgeHours > 24); set { }
        }

        public string? EvaluatingRoleAssignment { get; set; }
        // Formula EnforcedByUnauthorizedAgent (rulebook: =INDEX(RoleAssignments!{{IsUnauthorizedEnforcementAgent}}, MATCH({{EvaluatingRoleAssignment}}, RoleAssignments!{{RoleAssignmentId}}, 0)))
        public bool? EnforcedByUnauthorizedAgent
        {
            get => INDEX(RoleAssignments!this.IsUnauthorizedEnforcementAgent, MATCH(this.EvaluatingRoleAssignment, RoleAssignments!this.RoleAssignmentId, 0)); set { }
        }

        // Formula ConsentInputWasResolvable (rulebook: ={{RecipientConsentStatusRaw}} <> "")
        public bool? ConsentInputWasResolvable
        {
            get => this.RecipientConsentStatusRaw <> ""; set { }
        }

        // Formula RecipientConsentStatusRaw (rulebook: =INDEX(Recipients!{{SmsConsentStatus}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        public string? RecipientConsentStatusRaw
        {
            get => INDEX(Recipients!this.SmsConsentStatus, MATCH(this.Recipient, Recipients!this.RecipientId, 0)); set { }
        }

        // Formula PolicyInputWasResolvable (rulebook: ={{IntentPolicy}} <> "")
        public bool? PolicyInputWasResolvable
        {
            get => this.IntentPolicy <> ""; set { }
        }

        // Formula AllGateInputsResolved (rulebook: =AND({{ConsentInputWasResolvable}}, {{PolicyInputWasResolvable}}))
        public bool? AllGateInputsResolved
        {
            get => AND(this.ConsentInputWasResolvable, this.PolicyInputWasResolvable); set { }
        }

        // Formula IsUnevaluableRefusal (rulebook: =AND(NOT({{IsClearedToSend}}), NOT({{AllGateInputsResolved}})))
        public bool? IsUnevaluableRefusal
        {
            get => AND(NOT(this.IsClearedToSend), NOT(this.AllGateInputsResolved)); set { }
        }

        public bool? GateResultWasIndependentlyConfirmed { get; set; }
        // Formula IsSelfWitnessedDecision (rulebook: =NOT({{GateResultWasIndependentlyConfirmed}}))
        public bool? IsSelfWitnessedDecision
        {
            get => NOT(this.GateResultWasIndependentlyConfirmed); set { }
        }

        // Formula IsIndependentlyConfirmed (rulebook: =AND({{HasResultingDelivery}}, {{ResultingDeliveryWasTransmitted}}))
        public bool? IsIndependentlyConfirmed
        {
            get => AND(this.HasResultingDelivery, this.ResultingDeliveryWasTransmitted); set { }
        }

        // Formula IndependentlyConfirmedExecutionKey (rulebook: =IF({{IsIndependentlyConfirmed}}, {{ProcedureExecution}}, ""))
        public string? IndependentlyConfirmedExecutionKey
        {
            get => IF(this.IsIndependentlyConfirmed, this.ProcedureExecution, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? StepExecution { get; set; }
        public string? Recipient { get; set; }
        public string? MessageTemplate { get; set; }
        public string? ResultingDelivery { get; set; }
        public string? RefusalNotifiedRole { get; set; }
        public string? EvaluationContext { get; set; }

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

        private StepExecution _stepExecution;

        [ForeignKey("StepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(StepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. StepExecution: " + StepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(StepExecution);
                    if (_stepExecution != null)
                    {
                        Context.Attach(_stepExecution);
                    }
                }
                return _stepExecution;
            }
            set
            {
                if (_stepExecution != value)
                {
                    _stepExecution = value;
                    StepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
                }
            }
        }

        private Recipient _recipient;

        [ForeignKey("Recipient")]
        public virtual Recipient Recipient
        {
            get
            {
                if (_recipient == null && !string.IsNullOrEmpty(Recipient))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Recipient - no database context is set. Recipient: " + Recipient + ".");
                        }
                        return null;
                    }
                    _recipient = Context.Recipients.Find(Recipient);
                    if (_recipient != null)
                    {
                        Context.Attach(_recipient);
                    }
                }
                return _recipient;
            }
            set
            {
                if (_recipient != value)
                {
                    _recipient = value;
                    Recipient = _recipient == null ? default : _recipient.RecipientId;
                }
            }
        }

        private MessageTemplate _messageTemplate;

        [ForeignKey("MessageTemplate")]
        public virtual MessageTemplate MessageTemplate
        {
            get
            {
                if (_messageTemplate == null && !string.IsNullOrEmpty(MessageTemplate))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageTemplate - no database context is set. MessageTemplate: " + MessageTemplate + ".");
                        }
                        return null;
                    }
                    _messageTemplate = Context.MessageTemplates.Find(MessageTemplate);
                    if (_messageTemplate != null)
                    {
                        Context.Attach(_messageTemplate);
                    }
                }
                return _messageTemplate;
            }
            set
            {
                if (_messageTemplate != value)
                {
                    _messageTemplate = value;
                    MessageTemplate = _messageTemplate == null ? default : _messageTemplate.MessageTemplateId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access MessageDelivery - no database context is set. ResultingDelivery: " + ResultingDelivery + ".");
                        }
                        return null;
                    }
                    _messageDelivery = Context.MessageDeliveries.Find(ResultingDelivery);
                    if (_messageDelivery != null)
                    {
                        Context.Attach(_messageDelivery);
                    }
                }
                return _messageDelivery;
            }
            set
            {
                if (_messageDelivery != value)
                {
                    _messageDelivery = value;
                    ResultingDelivery = _messageDelivery == null ? default : _messageDelivery.MessageDeliveryId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. RefusalNotifiedRole: " + RefusalNotifiedRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(RefusalNotifiedRole);
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
                    RefusalNotifiedRole = _role == null ? default : _role.RoleId;
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
            _ = this.ProcedureExecution;
            _ = this.StepExecution;
            _ = this.Recipient;
            _ = this.MessageTemplate;
            _ = this.MessageDelivery;
            _ = this.Role;
            _ = this.EvaluationContext;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
