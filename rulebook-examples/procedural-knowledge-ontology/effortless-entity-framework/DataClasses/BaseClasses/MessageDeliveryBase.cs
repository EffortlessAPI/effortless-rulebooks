
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("MessageDeliveries")]
    public class MessageDeliveryBase : SoAEntityBase
    {
        [Key]
        public string MessageDeliveryId { get; set; }

        // Formula Name (rulebook: ={{Recipient}} & " / " & {{MessageTemplate}} & " / " & {{SentAt}})
        public string? Name
        {
            get => this.Recipient + " / " + this.MessageTemplate + " / " + this.SentAt; set { }
        }

        public string? RenderedBody { get; set; }
        public DateTime? SentAt { get; set; }
        public int? SentAtLocalHour { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? SuppressionReason { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        // Formula PolicyChannel (rulebook: =INDEX(MessageTemplates!{{CommunicationPolicy}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public string? PolicyChannel
        {
            get => INDEX(MessageTemplates!this.CommunicationPolicy, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula ChannelName (rulebook: =INDEX(CommunicationPolicies!{{Channel}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public string? ChannelName
        {
            get => INDEX(CommunicationPolicies!this.Channel, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula PolicyRequiresConsent (rulebook: =INDEX(CommunicationPolicies!{{ConsentRequired}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public bool? PolicyRequiresConsent
        {
            get => INDEX(CommunicationPolicies!this.ConsentRequired, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula RecipientHasSmsConsent (rulebook: =INDEX(Recipients!{{HasSmsConsent}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        public bool? RecipientHasSmsConsent
        {
            get => INDEX(Recipients!this.HasSmsConsent, MATCH(this.Recipient, Recipients!this.RecipientId, 0)); set { }
        }

        // Formula WasActuallyTransmitted (rulebook: =OR({{DeliveryStatus}} = "Sent", OR({{DeliveryStatus}} = "Delivered", {{DeliveryStatus}} = "Bounced")))
        public bool? WasActuallyTransmitted
        {
            get => OR(this.DeliveryStatus = "Sent", OR(this.DeliveryStatus = "Delivered", this.DeliveryStatus = "Bounced")); set { }
        }

        // Formula IsConsentViolation (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresConsent}}, NOT({{RecipientHasSmsConsent}}))))
        public bool? IsConsentViolation
        {
            get => AND(this.WasActuallyTransmitted, AND(this.PolicyRequiresConsent, NOT(this.RecipientHasSmsConsent))); set { }
        }

        // Formula ConsentViolationPolicyKey (rulebook: =IF({{IsConsentViolation}}, {{PolicyChannel}}, ""))
        public string? ConsentViolationPolicyKey
        {
            get => IF(this.IsConsentViolation, this.PolicyChannel, ""); set { }
        }

        // Formula PolicyQuietHoursStartHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursStartHour}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyQuietHoursStartHour
        {
            get => INDEX(CommunicationPolicies!this.QuietHoursStartHour, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula PolicyQuietHoursEndHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursEndHour}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyQuietHoursEndHour
        {
            get => INDEX(CommunicationPolicies!this.QuietHoursEndHour, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula PolicyHasQuietHours (rulebook: ={{PolicyQuietHoursStartHour}} <> {{PolicyQuietHoursEndHour}})
        public bool? PolicyHasQuietHours
        {
            get => this.PolicyQuietHoursStartHour <> this.PolicyQuietHoursEndHour; set { }
        }

        // Formula QuietWindowWrapsMidnight (rulebook: ={{PolicyQuietHoursStartHour}} > {{PolicyQuietHoursEndHour}})
        public bool? QuietWindowWrapsMidnight
        {
            get => this.PolicyQuietHoursStartHour > this.PolicyQuietHoursEndHour; set { }
        }

        // Formula IsInsideQuietWindow (rulebook: =IF({{QuietWindowWrapsMidnight}}, OR({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}}), AND({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}})))
        public bool? IsInsideQuietWindow
        {
            get => IF(this.QuietWindowWrapsMidnight, OR(this.SentAtLocalHour >= this.PolicyQuietHoursStartHour, this.SentAtLocalHour < this.PolicyQuietHoursEndHour), AND(this.SentAtLocalHour >= this.PolicyQuietHoursStartHour, this.SentAtLocalHour < this.PolicyQuietHoursEndHour)); set { }
        }

        // Formula IsQuietHoursViolation (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyHasQuietHours}}, {{IsInsideQuietWindow}})))
        public bool? IsQuietHoursViolation
        {
            get => AND(this.WasActuallyTransmitted, AND(this.PolicyHasQuietHours, this.IsInsideQuietWindow)); set { }
        }

        // Formula QuietHoursViolationPolicyKey (rulebook: =IF({{IsQuietHoursViolation}}, {{PolicyChannel}}, ""))
        public string? QuietHoursViolationPolicyKey
        {
            get => IF(this.IsQuietHoursViolation, this.PolicyChannel, ""); set { }
        }

        // Formula RecipientIsUnreachable (rulebook: =INDEX(Recipients!{{IsUnreachable}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        public bool? RecipientIsUnreachable
        {
            get => INDEX(Recipients!this.IsUnreachable, MATCH(this.Recipient, Recipients!this.RecipientId, 0)); set { }
        }

        // Formula IsAcknowledged (rulebook: ={{AcknowledgedAt}} <> "")
        public bool? IsAcknowledged
        {
            get => this.AcknowledgedAt <> ""; set { }
        }

        // Formula InvokedExceptionCondition (rulebook: =INDEX(Exceptions!{{Condition}}, MATCH({{InvokedException}}, Exceptions!{{ExceptionId}}, 0)))
        public string? InvokedExceptionCondition
        {
            get => INDEX(Exceptions!this.Condition, MATCH(this.InvokedException, Exceptions!this.ExceptionId, 0)); set { }
        }

        // Formula HasUnreachableExceptionInvoked (rulebook: ={{InvokedException}} = "exc-unreachable")
        public bool? HasUnreachableExceptionInvoked
        {
            get => this.InvokedException = "exc-unreachable"; set { }
        }

        // Formula IsFabricatedAcknowledgement (rulebook: =AND({{RecipientIsUnreachable}}, {{IsAcknowledged}}))
        public bool? IsFabricatedAcknowledgement
        {
            get => AND(this.RecipientIsUnreachable, this.IsAcknowledged); set { }
        }

        // Formula IsUnhandledUnreachable (rulebook: =AND({{RecipientIsUnreachable}}, NOT({{HasUnreachableExceptionInvoked}})))
        public bool? IsUnhandledUnreachable
        {
            get => AND(this.RecipientIsUnreachable, NOT(this.HasUnreachableExceptionInvoked)); set { }
        }

        // Formula UnreachableFailureKey (rulebook: =IF(OR({{IsFabricatedAcknowledgement}}, {{IsUnhandledUnreachable}}), {{ProcedureExecution}}, ""))
        public string? UnreachableFailureKey
        {
            get => IF(OR(this.IsFabricatedAcknowledgement, this.IsUnhandledUnreachable), this.ProcedureExecution, ""); set { }
        }

        // Formula PolicyRetentionDays (rulebook: =INDEX(CommunicationPolicies!{{RetentionDays}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyRetentionDays
        {
            get => INDEX(CommunicationPolicies!this.RetentionDays, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        public DateTime? AsOfInstant
        {
            get => INDEX(EvaluationContexts!this.AsOfInstant, MATCH(this.EvaluationContext, EvaluationContexts!this.EvaluationContextId, 0)); set { }
        }

        // Formula AgeDays (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days"))
        public int? AgeDays
        {
            get => DATETIME_DIFF(this.AsOfInstant, this.SentAt, "days"); set { }
        }

        // Formula IsWithinRetentionWindow (rulebook: ={{AgeDays}} <= {{PolicyRetentionDays}})
        public bool? IsWithinRetentionWindow
        {
            get => this.AgeDays <= this.PolicyRetentionDays; set { }
        }

        // Formula HasRenderedBody (rulebook: ={{RenderedBody}} <> "")
        public bool? HasRenderedBody
        {
            get => this.RenderedBody <> ""; set { }
        }

        // Formula IsEvidenceRequired (rulebook: =AND({{WasActuallyTransmitted}}, {{IsWithinRetentionWindow}}))
        public bool? IsEvidenceRequired
        {
            get => AND(this.WasActuallyTransmitted, this.IsWithinRetentionWindow); set { }
        }

        // Formula IsRetentionBreach (rulebook: =AND({{IsEvidenceRequired}}, NOT({{HasRenderedBody}})))
        public bool? IsRetentionBreach
        {
            get => AND(this.IsEvidenceRequired, NOT(this.HasRenderedBody)); set { }
        }

        // Formula RetentionBreachExecutionKey (rulebook: =IF({{IsRetentionBreach}}, {{ProcedureExecution}}, ""))
        public string? RetentionBreachExecutionKey
        {
            get => IF(this.IsRetentionBreach, this.ProcedureExecution, ""); set { }
        }

        // Formula SendingStepExecutionStep (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public string? SendingStepExecutionStep
        {
            get => INDEX(StepExecutions!this.Step, MATCH(this.StepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula ExecutionHasClearedLegalReview (rulebook: =INDEX(ProcedureExecutions!{{HasClearedLegalReview}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        public bool? ExecutionHasClearedLegalReview
        {
            get => INDEX(ProcedureExecutions!this.HasClearedLegalReview, MATCH(this.ProcedureExecution, ProcedureExecutions!this.ProcedureExecutionId, 0)); set { }
        }

        // Formula IsUnreviewedSend (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{ExecutionHasClearedLegalReview}})))
        public bool? IsUnreviewedSend
        {
            get => AND(this.WasActuallyTransmitted, NOT(this.ExecutionHasClearedLegalReview)); set { }
        }

        // Formula RenderedBodyLength (rulebook: =LEN({{RenderedBody}}))
        public int? RenderedBodyLength
        {
            get => LEN(this.RenderedBody); set { }
        }

        // Formula PolicyMaxMessageLengthAtSend (rulebook: =INDEX(CommunicationPolicies!{{MaxMessageLength}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyMaxMessageLengthAtSend
        {
            get => INDEX(CommunicationPolicies!this.MaxMessageLength, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula SegmentCount (rulebook: =IF({{RenderedBodyLength}} = 0, 0, IF({{RenderedBodyLength}} <= {{PolicyMaxMessageLengthAtSend}}, 1, ROUNDUP({{RenderedBodyLength}} / {{PolicyMaxMessageLengthAtSend}}, 0))))
        public int? SegmentCount
        {
            get => IF(this.RenderedBodyLength = 0, 0, IF(this.RenderedBodyLength <= this.PolicyMaxMessageLengthAtSend, 1, ROUNDUP(this.RenderedBodyLength / this.PolicyMaxMessageLengthAtSend, 0))); set { }
        }

        // Formula PolicyMaxSegmentsAtSend (rulebook: =INDEX(CommunicationPolicies!{{MaxSegments}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public int? PolicyMaxSegmentsAtSend
        {
            get => INDEX(CommunicationPolicies!this.MaxSegments, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula IsOverSegmentLimit (rulebook: =AND({{WasActuallyTransmitted}}, {{SegmentCount}} > {{PolicyMaxSegmentsAtSend}}))
        public bool? IsOverSegmentLimit
        {
            get => AND(this.WasActuallyTransmitted, this.SegmentCount > this.PolicyMaxSegmentsAtSend); set { }
        }

        // Formula TemplateHasValidApproval (rulebook: =INDEX(MessageTemplates!{{HasValidApproval}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public bool? TemplateHasValidApproval
        {
            get => INDEX(MessageTemplates!this.HasValidApproval, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula IsUnapprovedSend (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{TemplateHasValidApproval}})))
        public bool? IsUnapprovedSend
        {
            get => AND(this.WasActuallyTransmitted, NOT(this.TemplateHasValidApproval)); set { }
        }

        // Formula PolicyRequiredOptOutPhrase (rulebook: =INDEX(CommunicationPolicies!{{RequiredOptOutPhrase}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public string? PolicyRequiredOptOutPhrase
        {
            get => INDEX(CommunicationPolicies!this.RequiredOptOutPhrase, MATCH(this.PolicyChannel, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula PolicyRequiresOptOut (rulebook: ={{PolicyRequiredOptOutPhrase}} <> "")
        public bool? PolicyRequiresOptOut
        {
            get => this.PolicyRequiredOptOutPhrase <> ""; set { }
        }

        // Formula OptOutPhrasePosition (rulebook: =FIND({{PolicyRequiredOptOutPhrase}}, {{RenderedBody}}))
        public int? OptOutPhrasePosition
        {
            get => FIND(this.PolicyRequiredOptOutPhrase, this.RenderedBody); set { }
        }

        // Formula HasOptOutPhrase (rulebook: ={{OptOutPhrasePosition}} > 0)
        public bool? HasOptOutPhrase
        {
            get => this.OptOutPhrasePosition > 0; set { }
        }

        // Formula IsOptOutInFirstSegment (rulebook: =AND({{HasOptOutPhrase}}, {{OptOutPhrasePosition}} <= {{PolicyMaxMessageLengthAtSend}}))
        public bool? IsOptOutInFirstSegment
        {
            get => AND(this.HasOptOutPhrase, this.OptOutPhrasePosition <= this.PolicyMaxMessageLengthAtSend); set { }
        }

        // Formula IsMissingRequiredOptOut (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, NOT({{HasOptOutPhrase}}))))
        public bool? IsMissingRequiredOptOut
        {
            get => AND(this.WasActuallyTransmitted, AND(this.PolicyRequiresOptOut, NOT(this.HasOptOutPhrase))); set { }
        }

        // Formula IsOptOutAtRiskOfTruncation (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, AND({{HasOptOutPhrase}}, NOT({{IsOptOutInFirstSegment}})))))
        public bool? IsOptOutAtRiskOfTruncation
        {
            get => AND(this.WasActuallyTransmitted, AND(this.PolicyRequiresOptOut, AND(this.HasOptOutPhrase, NOT(this.IsOptOutInFirstSegment)))); set { }
        }

        // Formula IsFailedDelivery (rulebook: =OR({{DeliveryStatus}} = "Failed", {{DeliveryStatus}} = "Bounced"))
        public bool? IsFailedDelivery
        {
            get => OR(this.DeliveryStatus = "Failed", this.DeliveryStatus = "Bounced"); set { }
        }

        // Formula IsSuppressed (rulebook: ={{DeliveryStatus}} = "Suppressed")
        public bool? IsSuppressed
        {
            get => this.DeliveryStatus = "Suppressed"; set { }
        }

        // Formula IsTriaged (rulebook: ={{InvokedException}} <> "")
        public bool? IsTriaged
        {
            get => this.InvokedException <> ""; set { }
        }

        // Formula IsAbandonedFailure (rulebook: =AND({{IsFailedDelivery}}, NOT({{IsTriaged}})))
        public bool? IsAbandonedFailure
        {
            get => AND(this.IsFailedDelivery, NOT(this.IsTriaged)); set { }
        }

        // Formula AbandonedFailureExecutionKey (rulebook: =IF({{IsAbandonedFailure}}, {{ProcedureExecution}}, ""))
        public string? AbandonedFailureExecutionKey
        {
            get => IF(this.IsAbandonedFailure, this.ProcedureExecution, ""); set { }
        }

        // Formula ReachedExecutionKey (rulebook: =IF({{DeliveryStatus}} = "Delivered", {{ProcedureExecution}}, ""))
        public string? ReachedExecutionKey
        {
            get => IF(this.DeliveryStatus = "Delivered", this.ProcedureExecution, ""); set { }
        }

        // Formula TemplateWasSendable (rulebook: =INDEX(MessageTemplates!{{IsSendableUnderApproval}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public bool? TemplateWasSendable
        {
            get => INDEX(MessageTemplates!this.IsSendableUnderApproval, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula IsDriftedSend (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{TemplateWasSendable}})))
        public bool? IsDriftedSend
        {
            get => AND(this.WasActuallyTransmitted, NOT(this.TemplateWasSendable)); set { }
        }

        // Formula DriftedSendTemplateKey (rulebook: =IF({{IsDriftedSend}}, {{MessageTemplate}}, ""))
        public string? DriftedSendTemplateKey
        {
            get => IF(this.IsDriftedSend, this.MessageTemplate, ""); set { }
        }

        // Formula WasSentOutsideBusinessHours (rulebook: =OR({{SentAtLocalHour}} < 8, {{SentAtLocalHour}} > 18))
        public bool? WasSentOutsideBusinessHours
        {
            get => OR(this.SentAtLocalHour < 8, this.SentAtLocalHour > 18); set { }
        }

        // Formula WasDeliveredAndUnanswered (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{IsAcknowledged}})))
        public bool? WasDeliveredAndUnanswered
        {
            get => AND(this.WasActuallyTransmitted, NOT(this.IsAcknowledged)); set { }
        }

        // Formula IsPoorlyTimedUnanswered (rulebook: =AND({{WasDeliveredAndUnanswered}}, {{WasSentOutsideBusinessHours}}))
        public bool? IsPoorlyTimedUnanswered
        {
            get => AND(this.WasDeliveredAndUnanswered, this.WasSentOutsideBusinessHours); set { }
        }

        // Formula IsWellTimedUnanswered (rulebook: =AND({{WasDeliveredAndUnanswered}}, NOT({{WasSentOutsideBusinessHours}})))
        public bool? IsWellTimedUnanswered
        {
            get => AND(this.WasDeliveredAndUnanswered, NOT(this.WasSentOutsideBusinessHours)); set { }
        }

        // Formula UnansweredTemplateKey (rulebook: =IF({{WasDeliveredAndUnanswered}}, {{MessageTemplate}}, ""))
        public string? UnansweredTemplateKey
        {
            get => IF(this.WasDeliveredAndUnanswered, this.MessageTemplate, ""); set { }
        }

        // Formula TransmittedTemplateKey (rulebook: =IF({{WasActuallyTransmitted}}, {{MessageTemplate}}, ""))
        public string? TransmittedTemplateKey
        {
            get => IF(this.WasActuallyTransmitted, this.MessageTemplate, ""); set { }
        }

        public string? ApprovingAgentAtSend { get; set; }
        public string? ApprovingRoleAtSend { get; set; }
        public DateTime? ApprovalDecidedAtSend { get; set; }
        // Formula ApprovalPrecededSend (rulebook: =AND({{ApprovalDecidedAtSend}} <> "", {{SentAt}} > {{ApprovalDecidedAtSend}}))
        public bool? ApprovalPrecededSend
        {
            get => AND(this.ApprovalDecidedAtSend <> "", this.SentAt > this.ApprovalDecidedAtSend); set { }
        }

        // Formula HasFrozenApprovalEvidence (rulebook: =AND({{ApprovingAgentAtSend}} <> "", {{ApprovalDecidedAtSend}} <> ""))
        public bool? HasFrozenApprovalEvidence
        {
            get => AND(this.ApprovingAgentAtSend <> "", this.ApprovalDecidedAtSend <> ""); set { }
        }

        // Formula ProvenanceIsLiveDerived (rulebook: =NOT({{HasFrozenApprovalEvidence}}))
        public bool? ProvenanceIsLiveDerived
        {
            get => NOT(this.HasFrozenApprovalEvidence); set { }
        }

        // Formula CurrentLastApprovalAt (rulebook: =INDEX(MessageTemplates!{{LastApprovalAt}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public DateTime? CurrentLastApprovalAt
        {
            get => INDEX(MessageTemplates!this.LastApprovalAt, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula TemplateReapprovedSinceSend (rulebook: =AND({{CurrentLastApprovalAt}} <> "", {{CurrentLastApprovalAt}} > {{SentAt}}))
        public bool? TemplateReapprovedSinceSend
        {
            get => AND(this.CurrentLastApprovalAt <> "", this.CurrentLastApprovalAt > this.SentAt); set { }
        }

        // Formula IsUnprovableApprovalClaim (rulebook: =AND({{ProvenanceIsLiveDerived}}, AND({{TemplateReapprovedSinceSend}}, {{TemplateHasValidApproval}})))
        public bool? IsUnprovableApprovalClaim
        {
            get => AND(this.ProvenanceIsLiveDerived, AND(this.TemplateReapprovedSinceSend, this.TemplateHasValidApproval)); set { }
        }

        public DateTime? ReminderSentAt { get; set; }
        public int? ReminderCount { get; set; }
        // Formula HasSentReminder (rulebook: ={{ReminderCount}} > 0)
        public bool? HasSentReminder
        {
            get => this.ReminderCount > 0; set { }
        }

        // Formula AcknowledgementIsOutstanding (rulebook: =AND({{WasActuallyTransmitted}}, AND({{IsEvidenceRequired}}, NOT({{IsAcknowledged}}))))
        public bool? AcknowledgementIsOutstanding
        {
            get => AND(this.WasActuallyTransmitted, AND(this.IsEvidenceRequired, NOT(this.IsAcknowledged))); set { }
        }

        // Formula OutstandingAgeDays (rulebook: =IF({{AcknowledgementIsOutstanding}}, DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days"), 0))
        public int? OutstandingAgeDays
        {
            get => IF(this.AcknowledgementIsOutstanding, DATETIME_DIFF(this.AsOfInstant, this.SentAt, "days"), 0); set { }
        }

        // Formula IsUnchasedAcknowledgement (rulebook: =AND({{AcknowledgementIsOutstanding}}, AND({{OutstandingAgeDays}} > 7, NOT({{HasSentReminder}}))))
        public bool? IsUnchasedAcknowledgement
        {
            get => AND(this.AcknowledgementIsOutstanding, AND(this.OutstandingAgeDays > 7, NOT(this.HasSentReminder))); set { }
        }

        // Formula IsExhaustedFollowUp (rulebook: =AND({{AcknowledgementIsOutstanding}}, {{ReminderCount}} >= 3))
        public bool? IsExhaustedFollowUp
        {
            get => AND(this.AcknowledgementIsOutstanding, this.ReminderCount >= 3); set { }
        }

        // Formula NeedsHumanEscalation (rulebook: =AND({{IsExhaustedFollowUp}}, NOT({{HasUnreachableExceptionInvoked}})))
        public bool? NeedsHumanEscalation
        {
            get => AND(this.IsExhaustedFollowUp, NOT(this.HasUnreachableExceptionInvoked)); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? StepExecution { get; set; }
        public string? Recipient { get; set; }
        public string? MessageTemplate { get; set; }
        public string? SentByAgent { get; set; }
        public string? InvokedException { get; set; }
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

        private Agent _agent;

        [ForeignKey("SentByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SentByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SentByAgent: " + SentByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(SentByAgent);
                    if (_agent != null)
                    {
                        Context.Attach(_agent);
                    }
                }
                return _agent;
            }
            set
            {
                if (_agent != value)
                {
                    _agent = value;
                    SentByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Exception _exception;

        [ForeignKey("InvokedException")]
        public virtual Exception Exception
        {
            get
            {
                if (_exception == null && !string.IsNullOrEmpty(InvokedException))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exception - no database context is set. InvokedException: " + InvokedException + ".");
                        }
                        return null;
                    }
                    _exception = Context.Exceptions.Find(InvokedException);
                    if (_exception != null)
                    {
                        Context.Attach(_exception);
                    }
                }
                return _exception;
            }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    InvokedException = _exception == null ? default : _exception.ExceptionId;
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

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("MessageDelivery")]
        public virtual ObservableCollection<SendIntent> SendIntents
        {
            get
            {
                if (_sendIntents == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. MessageDeliveryId: " + this.MessageDeliveryId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = Context.SendIntents.Where(x => x.ResultingDelivery == this.MessageDeliveryId).ToList<SendIntent>();
                        _sendIntents = new ObservableCollection<SendIntent>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _sendIntents.CollectionChanged += SendIntents_CollectionChanged;
                }
                return _sendIntents;
            }
            private set
            {
                if (_sendIntents != null)
                {
                    _sendIntents.CollectionChanged -= SendIntents_CollectionChanged;
                }
                _sendIntents = value;
                if (_sendIntents != null)
                {
                    _sendIntents.CollectionChanged += SendIntents_CollectionChanged;
                }
            }
        }

        private void SendIntents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SendIntent>())
                {
                    item.ResultingDelivery = this.MessageDeliveryId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecution;
            _ = this.StepExecution;
            _ = this.Recipient;
            _ = this.MessageTemplate;
            _ = this.Agent;
            _ = this.Exception;
            _ = this.EvaluationContext;
            _ = this.SendIntents;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
