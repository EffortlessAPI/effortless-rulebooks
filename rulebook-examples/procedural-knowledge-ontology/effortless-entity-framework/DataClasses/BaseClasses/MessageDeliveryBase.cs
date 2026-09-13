
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
    [Table("MessageDeliveries")]
    public class MessageDeliveryBase : SoAEntityBase
    {
        [Key]
        public string MessageDeliveryId { get; set; }

        // Formula Name (rulebook: ={{Recipient}} & " / " & {{MessageTemplate}} & " / " & {{SentAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.Recipient)), F.S(" / "), F.TextOr(F.Of(this.MessageTemplate)), F.S(" / "), F.TimestamptzText(F.Of(this.SentAt))))); set { }
        }

        public string? RenderedBody { get; set; }
        public DateTimeOffset? SentAt { get; set; }
        public int? SentAtLocalHour { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? SuppressionReason { get; set; }
        public DateTimeOffset? AcknowledgedAt { get; set; }
        // Formula PolicyChannel (rulebook: =INDEX(MessageTemplates!{{CommunicationPolicy}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public string? PolicyChannel
        {
            get => F.AsString(F.Memo(this, "PolicyChannel", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.CommunicationPolicy), () => F.Of(new MessageTemplate().CommunicationPolicy)))); set { }
        }

        // Formula ChannelName (rulebook: =INDEX(CommunicationPolicies!{{Channel}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public string? ChannelName
        {
            get => F.AsString(F.Memo(this, "ChannelName", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.Channel), () => F.Of(new CommunicationPolicy().Channel)))); set { }
        }

        // Formula PolicyRequiresConsent (rulebook: =INDEX(CommunicationPolicies!{{ConsentRequired}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public bool? PolicyRequiresConsent
        {
            get => F.AsBool(F.Memo(this, "PolicyRequiresConsent", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.ConsentRequired), () => F.Of(new CommunicationPolicy().ConsentRequired)))); set { }
        }

        // Formula RecipientHasSmsConsent (rulebook: =INDEX(Recipients!{{HasSmsConsent}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        [NotMapped]
        public bool? RecipientHasSmsConsent
        {
            get => F.AsBool(F.Memo(this, "RecipientHasSmsConsent", () => F.Lookup<Recipient>(this, "Recipients", "RecipientId", __c => __c.Recipients, __r => F.Of(__r.RecipientId), F.Of(this.Recipient), __r => F.Of(__r.HasSmsConsent), () => F.Of(new Recipient().HasSmsConsent)))); set { }
        }

        // Formula WasActuallyTransmitted (rulebook: =OR({{DeliveryStatus}} = "Sent", OR({{DeliveryStatus}} = "Delivered", {{DeliveryStatus}} = "Bounced")))
        [NotMapped]
        public bool? WasActuallyTransmitted
        {
            get => F.AsBool(F.Memo(this, "WasActuallyTransmitted", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Sent"))), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Delivered"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Bounced")))))))); set { }
        }

        // Formula IsConsentViolation (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresConsent}}, NOT({{RecipientHasSmsConsent}}))))
        [NotMapped]
        public bool? IsConsentViolation
        {
            get => F.AsBool(F.Memo(this, "IsConsentViolation", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.And(F.Bool3(F.Of(this.PolicyRequiresConsent)), F.Bool3(F.Not(F.Bool3(F.Of(this.RecipientHasSmsConsent))))))))); set { }
        }

        // Formula ConsentViolationPolicyKey (rulebook: =IF({{IsConsentViolation}}, {{PolicyChannel}}, ""))
        [NotMapped]
        public string? ConsentViolationPolicyKey
        {
            get => F.AsString(F.Memo(this, "ConsentViolationPolicyKey", () => (F.Truthy(F.Bool3(F.Of(this.IsConsentViolation))) ? F.Of(this.PolicyChannel) : F.S("")))); set { }
        }

        // Formula PolicyQuietHoursStartHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursStartHour}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyQuietHoursStartHour
        {
            get => F.AsInt(F.Memo(this, "PolicyQuietHoursStartHour", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.QuietHoursStartHour), () => F.Of(new CommunicationPolicy().QuietHoursStartHour))))); set { }
        }

        // Formula PolicyQuietHoursEndHour (rulebook: =INDEX(CommunicationPolicies!{{QuietHoursEndHour}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyQuietHoursEndHour
        {
            get => F.AsInt(F.Memo(this, "PolicyQuietHoursEndHour", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.QuietHoursEndHour), () => F.Of(new CommunicationPolicy().QuietHoursEndHour))))); set { }
        }

        // Formula PolicyHasQuietHours (rulebook: ={{PolicyQuietHoursStartHour}} <> {{PolicyQuietHoursEndHour}})
        [NotMapped]
        public bool? PolicyHasQuietHours
        {
            get => F.AsBool(F.Memo(this, "PolicyHasQuietHours", () => F.Ne(F.Of(this.PolicyQuietHoursStartHour), F.Of(this.PolicyQuietHoursEndHour)))); set { }
        }

        // Formula QuietWindowWrapsMidnight (rulebook: ={{PolicyQuietHoursStartHour}} > {{PolicyQuietHoursEndHour}})
        [NotMapped]
        public bool? QuietWindowWrapsMidnight
        {
            get => F.AsBool(F.Memo(this, "QuietWindowWrapsMidnight", () => F.Cmp(F.Of(this.PolicyQuietHoursStartHour), ">", F.Of(this.PolicyQuietHoursEndHour)))); set { }
        }

        // Formula IsInsideQuietWindow (rulebook: =IF({{QuietWindowWrapsMidnight}}, OR({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}}), AND({{SentAtLocalHour}} >= {{PolicyQuietHoursStartHour}}, {{SentAtLocalHour}} < {{PolicyQuietHoursEndHour}})))
        [NotMapped]
        public bool? IsInsideQuietWindow
        {
            get => F.AsBool(F.Memo(this, "IsInsideQuietWindow", () => (F.Truthy(F.Bool3(F.Of(this.QuietWindowWrapsMidnight))) ? F.Or(F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAtLocalHour)), ">=", F.Of(this.PolicyQuietHoursStartHour))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAtLocalHour)), "<", F.Of(this.PolicyQuietHoursEndHour)))) : F.And(F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAtLocalHour)), ">=", F.Of(this.PolicyQuietHoursStartHour))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAtLocalHour)), "<", F.Of(this.PolicyQuietHoursEndHour))))))); set { }
        }

        // Formula IsQuietHoursViolation (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyHasQuietHours}}, {{IsInsideQuietWindow}})))
        [NotMapped]
        public bool? IsQuietHoursViolation
        {
            get => F.AsBool(F.Memo(this, "IsQuietHoursViolation", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.And(F.Bool3(F.Of(this.PolicyHasQuietHours)), F.Bool3(F.Of(this.IsInsideQuietWindow))))))); set { }
        }

        // Formula QuietHoursViolationPolicyKey (rulebook: =IF({{IsQuietHoursViolation}}, {{PolicyChannel}}, ""))
        [NotMapped]
        public string? QuietHoursViolationPolicyKey
        {
            get => F.AsString(F.Memo(this, "QuietHoursViolationPolicyKey", () => (F.Truthy(F.Bool3(F.Of(this.IsQuietHoursViolation))) ? F.Of(this.PolicyChannel) : F.S("")))); set { }
        }

        // Formula RecipientIsUnreachable (rulebook: =INDEX(Recipients!{{IsUnreachable}}, MATCH({{Recipient}}, Recipients!{{RecipientId}}, 0)))
        [NotMapped]
        public bool? RecipientIsUnreachable
        {
            get => F.AsBool(F.Memo(this, "RecipientIsUnreachable", () => F.Lookup<Recipient>(this, "Recipients", "RecipientId", __c => __c.Recipients, __r => F.Of(__r.RecipientId), F.Of(this.Recipient), __r => F.Of(__r.IsUnreachable), () => F.Of(new Recipient().IsUnreachable)))); set { }
        }

        // Formula IsAcknowledged (rulebook: ={{AcknowledgedAt}} <> "")
        [NotMapped]
        public bool? IsAcknowledged
        {
            get => F.AsBool(F.Memo(this, "IsAcknowledged", () => F.IsNotBlank(F.Of(this.AcknowledgedAt)))); set { }
        }

        // Formula InvokedExceptionCondition (rulebook: =INDEX(Exceptions!{{Condition}}, MATCH({{InvokedException}}, Exceptions!{{ExceptionId}}, 0)))
        [NotMapped]
        public string? InvokedExceptionCondition
        {
            get => F.AsString(F.Memo(this, "InvokedExceptionCondition", () => F.Lookup<Exception>(this, "Exceptions", "ExceptionId", __c => __c.Exceptions, __r => F.Of(__r.ExceptionId), F.Of(this.InvokedException), __r => F.Of(__r.Condition), () => F.Of(new Exception().Condition)))); set { }
        }

        // Formula HasUnreachableExceptionInvoked (rulebook: ={{InvokedException}} = "exc-unreachable")
        [NotMapped]
        public bool? HasUnreachableExceptionInvoked
        {
            get => F.AsBool(F.Memo(this, "HasUnreachableExceptionInvoked", () => F.Eq(F.Nullif(F.Of(this.InvokedException)), F.S("exc-unreachable")))); set { }
        }

        // Formula IsFabricatedAcknowledgement (rulebook: =AND({{RecipientIsUnreachable}}, {{IsAcknowledged}}))
        [NotMapped]
        public bool? IsFabricatedAcknowledgement
        {
            get => F.AsBool(F.Memo(this, "IsFabricatedAcknowledgement", () => F.And(F.Bool3(F.Of(this.RecipientIsUnreachable)), F.Bool3(F.Of(this.IsAcknowledged))))); set { }
        }

        // Formula IsUnhandledUnreachable (rulebook: =AND({{RecipientIsUnreachable}}, NOT({{HasUnreachableExceptionInvoked}})))
        [NotMapped]
        public bool? IsUnhandledUnreachable
        {
            get => F.AsBool(F.Memo(this, "IsUnhandledUnreachable", () => F.And(F.Bool3(F.Of(this.RecipientIsUnreachable)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasUnreachableExceptionInvoked))))))); set { }
        }

        // Formula UnreachableFailureKey (rulebook: =IF(OR({{IsFabricatedAcknowledgement}}, {{IsUnhandledUnreachable}}), {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? UnreachableFailureKey
        {
            get => F.AsString(F.Memo(this, "UnreachableFailureKey", () => (F.Truthy(F.Bool3(F.Or(F.Bool3(F.Of(this.IsFabricatedAcknowledgement)), F.Bool3(F.Of(this.IsUnhandledUnreachable))))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula PolicyRetentionDays (rulebook: =INDEX(CommunicationPolicies!{{RetentionDays}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyRetentionDays
        {
            get => F.AsInt(F.Memo(this, "PolicyRetentionDays", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.RetentionDays), () => F.Of(new CommunicationPolicy().RetentionDays))))); set { }
        }

        // Formula AsOfInstant (rulebook: =INDEX(EvaluationContexts!{{AsOfInstant}}, MATCH({{EvaluationContext}}, EvaluationContexts!{{EvaluationContextId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AsOfInstant
        {
            get => F.AsDateTime(F.Memo(this, "AsOfInstant", () => F.Lookup<EvaluationContext>(this, "EvaluationContexts", "EvaluationContextId", __c => __c.EvaluationContexts, __r => F.Of(__r.EvaluationContextId), F.Of(this.EvaluationContext), __r => F.Of(__r.AsOfInstant), () => F.Of(new EvaluationContext().AsOfInstant)))); set { }
        }

        // Formula AgeDays (rulebook: =DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days"))
        [NotMapped]
        public int? AgeDays
        {
            get => F.AsInt(F.Memo(this, "AgeDays", () => F.Integer(F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.SentAt), F.S("days"))))); set { }
        }

        // Formula IsWithinRetentionWindow (rulebook: ={{AgeDays}} <= {{PolicyRetentionDays}})
        [NotMapped]
        public bool? IsWithinRetentionWindow
        {
            get => F.AsBool(F.Memo(this, "IsWithinRetentionWindow", () => F.Cmp(F.Of(this.AgeDays), "<=", F.Of(this.PolicyRetentionDays)))); set { }
        }

        // Formula HasRenderedBody (rulebook: ={{RenderedBody}} <> "")
        [NotMapped]
        public bool? HasRenderedBody
        {
            get => F.AsBool(F.Memo(this, "HasRenderedBody", () => F.IsNotBlank(F.Of(this.RenderedBody)))); set { }
        }

        // Formula IsEvidenceRequired (rulebook: =AND({{WasActuallyTransmitted}}, {{IsWithinRetentionWindow}}))
        [NotMapped]
        public bool? IsEvidenceRequired
        {
            get => F.AsBool(F.Memo(this, "IsEvidenceRequired", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.Of(this.IsWithinRetentionWindow))))); set { }
        }

        // Formula IsRetentionBreach (rulebook: =AND({{IsEvidenceRequired}}, NOT({{HasRenderedBody}})))
        [NotMapped]
        public bool? IsRetentionBreach
        {
            get => F.AsBool(F.Memo(this, "IsRetentionBreach", () => F.And(F.Bool3(F.Of(this.IsEvidenceRequired)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasRenderedBody))))))); set { }
        }

        // Formula RetentionBreachExecutionKey (rulebook: =IF({{IsRetentionBreach}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? RetentionBreachExecutionKey
        {
            get => F.AsString(F.Memo(this, "RetentionBreachExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsRetentionBreach))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula SendingStepExecutionStep (rulebook: =INDEX(StepExecutions!{{Step}}, MATCH({{StepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public string? SendingStepExecutionStep
        {
            get => F.AsString(F.Memo(this, "SendingStepExecutionStep", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.StepExecution), __r => F.Of(__r.Step), () => F.Of(new StepExecution().Step)))); set { }
        }

        // Formula ExecutionHasClearedLegalReview (rulebook: =INDEX(ProcedureExecutions!{{HasClearedLegalReview}}, MATCH({{ProcedureExecution}}, ProcedureExecutions!{{ProcedureExecutionId}}, 0)))
        [NotMapped]
        public bool? ExecutionHasClearedLegalReview
        {
            get => F.AsBool(F.Memo(this, "ExecutionHasClearedLegalReview", () => F.Lookup<ProcedureExecution>(this, "ProcedureExecutions", "ProcedureExecutionId", __c => __c.ProcedureExecutions, __r => F.Of(__r.ProcedureExecutionId), F.Of(this.ProcedureExecution), __r => F.Of(__r.HasClearedLegalReview), () => F.Of(new ProcedureExecution().HasClearedLegalReview)))); set { }
        }

        // Formula IsUnreviewedSend (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{ExecutionHasClearedLegalReview}})))
        [NotMapped]
        public bool? IsUnreviewedSend
        {
            get => F.AsBool(F.Memo(this, "IsUnreviewedSend", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.Not(F.Bool3(F.Of(this.ExecutionHasClearedLegalReview))))))); set { }
        }

        // Formula RenderedBodyLength (rulebook: =LEN({{RenderedBody}}))
        [NotMapped]
        public int? RenderedBodyLength
        {
            get => F.AsInt(F.Memo(this, "RenderedBodyLength", () => F.Integer(F.Len(F.Of(this.RenderedBody))))); set { }
        }

        // Formula PolicyMaxMessageLengthAtSend (rulebook: =INDEX(CommunicationPolicies!{{MaxMessageLength}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyMaxMessageLengthAtSend
        {
            get => F.AsInt(F.Memo(this, "PolicyMaxMessageLengthAtSend", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.MaxMessageLength), () => F.Of(new CommunicationPolicy().MaxMessageLength))))); set { }
        }

        // Formula SegmentCount (rulebook: =IF({{RenderedBodyLength}} = 0, 0, IF({{RenderedBodyLength}} <= {{PolicyMaxMessageLengthAtSend}}, 1, ROUNDUP({{RenderedBodyLength}} / {{PolicyMaxMessageLengthAtSend}}, 0))))
        [NotMapped]
        public int? SegmentCount
        {
            get => F.AsInt(F.Memo(this, "SegmentCount", () => F.Integer((F.Truthy(F.Bool3(F.Eq(F.Of(this.RenderedBodyLength), F.I(0)))) ? F.I(0) : (F.Truthy(F.Bool3(F.Cmp(F.Of(this.RenderedBodyLength), "<=", F.Of(this.PolicyMaxMessageLengthAtSend)))) ? F.I(1) : F.Roundup(F.Div(F.Of(this.RenderedBodyLength), F.Of(this.PolicyMaxMessageLengthAtSend)), F.I(0))))))); set { }
        }

        // Formula PolicyMaxSegmentsAtSend (rulebook: =INDEX(CommunicationPolicies!{{MaxSegments}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public int? PolicyMaxSegmentsAtSend
        {
            get => F.AsInt(F.Memo(this, "PolicyMaxSegmentsAtSend", () => F.Integer(F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.MaxSegments), () => F.Of(new CommunicationPolicy().MaxSegments))))); set { }
        }

        // Formula IsOverSegmentLimit (rulebook: =AND({{WasActuallyTransmitted}}, {{SegmentCount}} > {{PolicyMaxSegmentsAtSend}}))
        [NotMapped]
        public bool? IsOverSegmentLimit
        {
            get => F.AsBool(F.Memo(this, "IsOverSegmentLimit", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.Cmp(F.Of(this.SegmentCount), ">", F.Of(this.PolicyMaxSegmentsAtSend)))))); set { }
        }

        // Formula TemplateHasValidApproval (rulebook: =INDEX(MessageTemplates!{{HasValidApproval}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public bool? TemplateHasValidApproval
        {
            get => F.AsBool(F.Memo(this, "TemplateHasValidApproval", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.HasValidApproval), () => F.Of(new MessageTemplate().HasValidApproval)))); set { }
        }

        // Formula IsUnapprovedSend (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{TemplateHasValidApproval}})))
        [NotMapped]
        public bool? IsUnapprovedSend
        {
            get => F.AsBool(F.Memo(this, "IsUnapprovedSend", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.Not(F.Bool3(F.Of(this.TemplateHasValidApproval))))))); set { }
        }

        // Formula PolicyRequiredOptOutPhrase (rulebook: =INDEX(CommunicationPolicies!{{RequiredOptOutPhrase}}, MATCH({{PolicyChannel}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public string? PolicyRequiredOptOutPhrase
        {
            get => F.AsString(F.Memo(this, "PolicyRequiredOptOutPhrase", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.PolicyChannel), __r => F.Of(__r.RequiredOptOutPhrase), () => F.Of(new CommunicationPolicy().RequiredOptOutPhrase)))); set { }
        }

        // Formula PolicyRequiresOptOut (rulebook: ={{PolicyRequiredOptOutPhrase}} <> "")
        [NotMapped]
        public bool? PolicyRequiresOptOut
        {
            get => F.AsBool(F.Memo(this, "PolicyRequiresOptOut", () => F.IsNotBlank(F.Of(this.PolicyRequiredOptOutPhrase)))); set { }
        }

        // Formula OptOutPhrasePosition (rulebook: =FIND({{PolicyRequiredOptOutPhrase}}, {{RenderedBody}}))
        [NotMapped]
        public int? OptOutPhrasePosition
        {
            get => F.AsInt(F.Memo(this, "OptOutPhrasePosition", () => F.Integer(F.Find(F.Of(this.PolicyRequiredOptOutPhrase), F.Nullif(F.Of(this.RenderedBody)))))); set { }
        }

        // Formula HasOptOutPhrase (rulebook: ={{OptOutPhrasePosition}} > 0)
        [NotMapped]
        public bool? HasOptOutPhrase
        {
            get => F.AsBool(F.Memo(this, "HasOptOutPhrase", () => F.Cmp(F.Of(this.OptOutPhrasePosition), ">", F.I(0)))); set { }
        }

        // Formula IsOptOutInFirstSegment (rulebook: =AND({{HasOptOutPhrase}}, {{OptOutPhrasePosition}} <= {{PolicyMaxMessageLengthAtSend}}))
        [NotMapped]
        public bool? IsOptOutInFirstSegment
        {
            get => F.AsBool(F.Memo(this, "IsOptOutInFirstSegment", () => F.And(F.Bool3(F.Of(this.HasOptOutPhrase)), F.Bool3(F.Cmp(F.Of(this.OptOutPhrasePosition), "<=", F.Of(this.PolicyMaxMessageLengthAtSend)))))); set { }
        }

        // Formula IsMissingRequiredOptOut (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, NOT({{HasOptOutPhrase}}))))
        [NotMapped]
        public bool? IsMissingRequiredOptOut
        {
            get => F.AsBool(F.Memo(this, "IsMissingRequiredOptOut", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.And(F.Bool3(F.Of(this.PolicyRequiresOptOut)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasOptOutPhrase))))))))); set { }
        }

        // Formula IsOptOutAtRiskOfTruncation (rulebook: =AND({{WasActuallyTransmitted}}, AND({{PolicyRequiresOptOut}}, AND({{HasOptOutPhrase}}, NOT({{IsOptOutInFirstSegment}})))))
        [NotMapped]
        public bool? IsOptOutAtRiskOfTruncation
        {
            get => F.AsBool(F.Memo(this, "IsOptOutAtRiskOfTruncation", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.And(F.Bool3(F.Of(this.PolicyRequiresOptOut)), F.Bool3(F.And(F.Bool3(F.Of(this.HasOptOutPhrase)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsOptOutInFirstSegment))))))))))); set { }
        }

        // Formula IsFailedDelivery (rulebook: =OR({{DeliveryStatus}} = "Failed", {{DeliveryStatus}} = "Bounced"))
        [NotMapped]
        public bool? IsFailedDelivery
        {
            get => F.AsBool(F.Memo(this, "IsFailedDelivery", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Failed"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Bounced")))))); set { }
        }

        // Formula IsSuppressed (rulebook: ={{DeliveryStatus}} = "Suppressed")
        [NotMapped]
        public bool? IsSuppressed
        {
            get => F.AsBool(F.Memo(this, "IsSuppressed", () => F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Suppressed")))); set { }
        }

        // Formula IsTriaged (rulebook: ={{InvokedException}} <> "")
        [NotMapped]
        public bool? IsTriaged
        {
            get => F.AsBool(F.Memo(this, "IsTriaged", () => F.IsNotBlank(F.Of(this.InvokedException)))); set { }
        }

        // Formula IsAbandonedFailure (rulebook: =AND({{IsFailedDelivery}}, NOT({{IsTriaged}})))
        [NotMapped]
        public bool? IsAbandonedFailure
        {
            get => F.AsBool(F.Memo(this, "IsAbandonedFailure", () => F.And(F.Bool3(F.Of(this.IsFailedDelivery)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsTriaged))))))); set { }
        }

        // Formula AbandonedFailureExecutionKey (rulebook: =IF({{IsAbandonedFailure}}, {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? AbandonedFailureExecutionKey
        {
            get => F.AsString(F.Memo(this, "AbandonedFailureExecutionKey", () => (F.Truthy(F.Bool3(F.Of(this.IsAbandonedFailure))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula ReachedExecutionKey (rulebook: =IF({{DeliveryStatus}} = "Delivered", {{ProcedureExecution}}, ""))
        [NotMapped]
        public string? ReachedExecutionKey
        {
            get => F.AsString(F.Memo(this, "ReachedExecutionKey", () => (F.Truthy(F.Bool3(F.Eq(F.Nullif(F.Of(this.DeliveryStatus)), F.S("Delivered")))) ? F.Of(this.ProcedureExecution) : F.S("")))); set { }
        }

        // Formula TemplateWasSendable (rulebook: =INDEX(MessageTemplates!{{IsSendableUnderApproval}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public bool? TemplateWasSendable
        {
            get => F.AsBool(F.Memo(this, "TemplateWasSendable", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.IsSendableUnderApproval), () => F.Of(new MessageTemplate().IsSendableUnderApproval)))); set { }
        }

        // Formula IsDriftedSend (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{TemplateWasSendable}})))
        [NotMapped]
        public bool? IsDriftedSend
        {
            get => F.AsBool(F.Memo(this, "IsDriftedSend", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.Not(F.Bool3(F.Of(this.TemplateWasSendable))))))); set { }
        }

        // Formula DriftedSendTemplateKey (rulebook: =IF({{IsDriftedSend}}, {{MessageTemplate}}, ""))
        [NotMapped]
        public string? DriftedSendTemplateKey
        {
            get => F.AsString(F.Memo(this, "DriftedSendTemplateKey", () => (F.Truthy(F.Bool3(F.Of(this.IsDriftedSend))) ? F.Of(this.MessageTemplate) : F.S("")))); set { }
        }

        // Formula WasSentOutsideBusinessHours (rulebook: =OR({{SentAtLocalHour}} < 8, {{SentAtLocalHour}} > 18))
        [NotMapped]
        public bool? WasSentOutsideBusinessHours
        {
            get => F.AsBool(F.Memo(this, "WasSentOutsideBusinessHours", () => F.Or(F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAtLocalHour)), "<", F.I(8))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAtLocalHour)), ">", F.I(18)))))); set { }
        }

        // Formula WasDeliveredAndUnanswered (rulebook: =AND({{WasActuallyTransmitted}}, NOT({{IsAcknowledged}})))
        [NotMapped]
        public bool? WasDeliveredAndUnanswered
        {
            get => F.AsBool(F.Memo(this, "WasDeliveredAndUnanswered", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsAcknowledged))))))); set { }
        }

        // Formula IsPoorlyTimedUnanswered (rulebook: =AND({{WasDeliveredAndUnanswered}}, {{WasSentOutsideBusinessHours}}))
        [NotMapped]
        public bool? IsPoorlyTimedUnanswered
        {
            get => F.AsBool(F.Memo(this, "IsPoorlyTimedUnanswered", () => F.And(F.Bool3(F.Of(this.WasDeliveredAndUnanswered)), F.Bool3(F.Of(this.WasSentOutsideBusinessHours))))); set { }
        }

        // Formula IsWellTimedUnanswered (rulebook: =AND({{WasDeliveredAndUnanswered}}, NOT({{WasSentOutsideBusinessHours}})))
        [NotMapped]
        public bool? IsWellTimedUnanswered
        {
            get => F.AsBool(F.Memo(this, "IsWellTimedUnanswered", () => F.And(F.Bool3(F.Of(this.WasDeliveredAndUnanswered)), F.Bool3(F.Not(F.Bool3(F.Of(this.WasSentOutsideBusinessHours))))))); set { }
        }

        // Formula UnansweredTemplateKey (rulebook: =IF({{WasDeliveredAndUnanswered}}, {{MessageTemplate}}, ""))
        [NotMapped]
        public string? UnansweredTemplateKey
        {
            get => F.AsString(F.Memo(this, "UnansweredTemplateKey", () => (F.Truthy(F.Bool3(F.Of(this.WasDeliveredAndUnanswered))) ? F.Of(this.MessageTemplate) : F.S("")))); set { }
        }

        // Formula TransmittedTemplateKey (rulebook: =IF({{WasActuallyTransmitted}}, {{MessageTemplate}}, ""))
        [NotMapped]
        public string? TransmittedTemplateKey
        {
            get => F.AsString(F.Memo(this, "TransmittedTemplateKey", () => (F.Truthy(F.Bool3(F.Of(this.WasActuallyTransmitted))) ? F.Of(this.MessageTemplate) : F.S("")))); set { }
        }

        public string? ApprovingAgentAtSend { get; set; }
        public string? ApprovingRoleAtSend { get; set; }
        public DateTimeOffset? ApprovalDecidedAtSend { get; set; }
        // Formula ApprovalPrecededSend (rulebook: =AND({{ApprovalDecidedAtSend}} <> "", {{SentAt}} > {{ApprovalDecidedAtSend}}))
        [NotMapped]
        public bool? ApprovalPrecededSend
        {
            get => F.AsBool(F.Memo(this, "ApprovalPrecededSend", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ApprovalDecidedAtSend))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.SentAt)), ">", F.Nullif(F.Of(this.ApprovalDecidedAtSend))))))); set { }
        }

        // Formula HasFrozenApprovalEvidence (rulebook: =AND({{ApprovingAgentAtSend}} <> "", {{ApprovalDecidedAtSend}} <> ""))
        [NotMapped]
        public bool? HasFrozenApprovalEvidence
        {
            get => F.AsBool(F.Memo(this, "HasFrozenApprovalEvidence", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ApprovingAgentAtSend))), F.Bool3(F.IsNotBlank(F.Of(this.ApprovalDecidedAtSend)))))); set { }
        }

        // Formula ProvenanceIsLiveDerived (rulebook: =NOT({{HasFrozenApprovalEvidence}}))
        [NotMapped]
        public bool? ProvenanceIsLiveDerived
        {
            get => F.AsBool(F.Memo(this, "ProvenanceIsLiveDerived", () => F.Not(F.Bool3(F.Of(this.HasFrozenApprovalEvidence))))); set { }
        }

        // Formula CurrentLastApprovalAt (rulebook: =INDEX(MessageTemplates!{{LastApprovalAt}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public DateTimeOffset? CurrentLastApprovalAt
        {
            get => F.AsDateTime(F.Memo(this, "CurrentLastApprovalAt", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.LastApprovalAt), () => F.Of(new MessageTemplate().LastApprovalAt)))); set { }
        }

        // Formula TemplateReapprovedSinceSend (rulebook: =AND({{CurrentLastApprovalAt}} <> "", {{CurrentLastApprovalAt}} > {{SentAt}}))
        [NotMapped]
        public bool? TemplateReapprovedSinceSend
        {
            get => F.AsBool(F.Memo(this, "TemplateReapprovedSinceSend", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CurrentLastApprovalAt))), F.Bool3(F.Cmp(F.Of(this.CurrentLastApprovalAt), ">", F.Nullif(F.Of(this.SentAt))))))); set { }
        }

        // Formula IsUnprovableApprovalClaim (rulebook: =AND({{ProvenanceIsLiveDerived}}, AND({{TemplateReapprovedSinceSend}}, {{TemplateHasValidApproval}})))
        [NotMapped]
        public bool? IsUnprovableApprovalClaim
        {
            get => F.AsBool(F.Memo(this, "IsUnprovableApprovalClaim", () => F.And(F.Bool3(F.Of(this.ProvenanceIsLiveDerived)), F.Bool3(F.And(F.Bool3(F.Of(this.TemplateReapprovedSinceSend)), F.Bool3(F.Of(this.TemplateHasValidApproval))))))); set { }
        }

        public DateTimeOffset? ReminderSentAt { get; set; }
        public int? ReminderCount { get; set; }
        // Formula HasSentReminder (rulebook: ={{ReminderCount}} > 0)
        [NotMapped]
        public bool? HasSentReminder
        {
            get => F.AsBool(F.Memo(this, "HasSentReminder", () => F.Cmp(F.Nullif(F.Of(this.ReminderCount)), ">", F.I(0)))); set { }
        }

        // Formula AcknowledgementIsOutstanding (rulebook: =AND({{WasActuallyTransmitted}}, AND({{IsEvidenceRequired}}, NOT({{IsAcknowledged}}))))
        [NotMapped]
        public bool? AcknowledgementIsOutstanding
        {
            get => F.AsBool(F.Memo(this, "AcknowledgementIsOutstanding", () => F.And(F.Bool3(F.Of(this.WasActuallyTransmitted)), F.Bool3(F.And(F.Bool3(F.Of(this.IsEvidenceRequired)), F.Bool3(F.Not(F.Bool3(F.Of(this.IsAcknowledged))))))))); set { }
        }

        // Formula OutstandingAgeDays (rulebook: =IF({{AcknowledgementIsOutstanding}}, DATETIME_DIFF({{AsOfInstant}}, {{SentAt}}, "days"), 0))
        [NotMapped]
        public int? OutstandingAgeDays
        {
            get => F.AsInt(F.Memo(this, "OutstandingAgeDays", () => F.Integer((F.Truthy(F.Bool3(F.Of(this.AcknowledgementIsOutstanding))) ? F.DatetimeDiff(F.Of(this.AsOfInstant), F.Of(this.SentAt), F.S("days")) : F.I(0))))); set { }
        }

        // Formula IsUnchasedAcknowledgement (rulebook: =AND({{AcknowledgementIsOutstanding}}, AND({{OutstandingAgeDays}} > 7, NOT({{HasSentReminder}}))))
        [NotMapped]
        public bool? IsUnchasedAcknowledgement
        {
            get => F.AsBool(F.Memo(this, "IsUnchasedAcknowledgement", () => F.And(F.Bool3(F.Of(this.AcknowledgementIsOutstanding)), F.Bool3(F.And(F.Bool3(F.Cmp(F.Of(this.OutstandingAgeDays), ">", F.I(7))), F.Bool3(F.Not(F.Bool3(F.Of(this.HasSentReminder))))))))); set { }
        }

        // Formula IsExhaustedFollowUp (rulebook: =AND({{AcknowledgementIsOutstanding}}, {{ReminderCount}} >= 3))
        [NotMapped]
        public bool? IsExhaustedFollowUp
        {
            get => F.AsBool(F.Memo(this, "IsExhaustedFollowUp", () => F.And(F.Bool3(F.Of(this.AcknowledgementIsOutstanding)), F.Bool3(F.Cmp(F.Nullif(F.Of(this.ReminderCount)), ">=", F.I(3)))))); set { }
        }

        // Formula NeedsHumanEscalation (rulebook: =AND({{IsExhaustedFollowUp}}, NOT({{HasUnreachableExceptionInvoked}})))
        [NotMapped]
        public bool? NeedsHumanEscalation
        {
            get => F.AsBool(F.Memo(this, "NeedsHumanEscalation", () => F.And(F.Bool3(F.Of(this.IsExhaustedFollowUp)), F.Bool3(F.Not(F.Bool3(F.Of(this.HasUnreachableExceptionInvoked))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureExecution { get; set; }
        public string? StepExecution { get; set; }
        public string? Recipient { get; set; }
        public string? MessageTemplate { get; set; }
        public string? SentByAgent { get; set; }
        public string? InvokedException { get; set; }
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

        private Agent _agent;

        [ForeignKey("SentByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(SentByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. SentByAgent: " + SentByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(SentByAgent);
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
                        SentByAgent = _agent.AgentId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Exception - no database context is set. InvokedException: " + InvokedException + ".");
                        }
                        return null;
                    }
                    _exception = base.SoAContext.Exceptions.Find(InvokedException);
                    if (_exception != null)
                    {
                        base.SoAContext.Attach(_exception);
                    }
                }
                return _exception;
            }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_exception != null)
                    {
                        InvokedException = _exception.ExceptionId;
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

        private ObservableCollection<SendIntent> _sendIntents;

        [InverseProperty("MessageDelivery")]
        public virtual ObservableCollection<SendIntent> SendIntents
        {
            get
            {
                if (_sendIntents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SendIntents - no database context is set. MessageDeliveryId: " + this.MessageDeliveryId + ".");
                        }
                        _sendIntents = new ObservableCollection<SendIntent>();
                    }
                    else
                    {
                        var items = base.SoAContext.SendIntents.Where(x => x.ResultingDelivery == this.MessageDeliveryId).ToList<SendIntent>();
                        _sendIntents = new ObservableCollection<SendIntent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
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
            _ = this.ProcedureExecutionRef;
            _ = this.StepExecutionRef;
            _ = this.RecipientRef;
            _ = this.MessageTemplateRef;
            _ = this.Agent;
            _ = this.Exception;
            _ = this.EvaluationContextRef;
            _ = this.SendIntents;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
