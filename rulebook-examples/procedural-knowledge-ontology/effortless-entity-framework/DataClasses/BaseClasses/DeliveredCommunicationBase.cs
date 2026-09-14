
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
    [Table("DeliveredCommunications")]
    public class DeliveredCommunicationBase : SoAEntityBase
    {
        [Key]
        public string DeliveredCommunicationId { get; set; }

        // Formula Name (rulebook: ={{Channel}} & " -> " & {{RecipientKey}} & " @ " & {{SentAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Channel)), F.S(" -> "), F.Text(F.Of(this.RecipientKey)), F.S(" @ "), F.DatetimeText(F.Of(this.SentAt))))); set { }
        }

        public string? Channel { get; set; }
        public string? RecipientKey { get; set; }
        public DateTimeOffset? SentAt { get; set; }
        public string? RenderedContentHash { get; set; }
        public string? ApprovedContentHash { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula HasAuthorization (rulebook: ={{AuthorizingStepExecution}} <> "")
        [NotMapped]
        public bool? HasAuthorization
        {
            get => F.AsBool(F.Memo(this, "HasAuthorization", () => F.IsNotBlank(F.Of(this.AuthorizingStepExecution)))); set { }
        }

        // Formula ContentMatchesApproval (rulebook: ={{RenderedContentHash}} = {{ApprovedContentHash}})
        [NotMapped]
        public bool? ContentMatchesApproval
        {
            get => F.AsBool(F.Memo(this, "ContentMatchesApproval", () => F.Eq(F.Nullif(F.Of(this.RenderedContentHash)), F.Nullif(F.Of(this.ApprovedContentHash))))); set { }
        }

        // Formula AuthorizedAt (rulebook: =INDEX(StepExecutions!{{EndedAt}}, MATCH({{AuthorizingStepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        [NotMapped]
        public DateTimeOffset? AuthorizedAt
        {
            get => F.AsDateTime(F.Memo(this, "AuthorizedAt", () => F.Lookup<StepExecution>(this, "StepExecutions", "StepExecutionId", __c => __c.StepExecutions, __r => F.Of(__r.StepExecutionId), F.Of(this.AuthorizingStepExecution), __r => F.Of(__r.EndedAt), () => F.Of(new StepExecution().EndedAt)))); set { }
        }

        // Formula WasApprovedBeforeSending (rulebook: ={{AuthorizedAt}} <= {{SentAt}})
        [NotMapped]
        public bool? WasApprovedBeforeSending
        {
            get => F.AsBool(F.Memo(this, "WasApprovedBeforeSending", () => F.Cmp(F.Of(this.AuthorizedAt), "<=", F.Nullif(F.Of(this.SentAt))))); set { }
        }

        // Formula IsDefensible (rulebook: =AND({{HasAuthorization}}, {{ContentMatchesApproval}}, {{WasApprovedBeforeSending}}))
        [NotMapped]
        public bool? IsDefensible
        {
            get => F.AsBool(F.Memo(this, "IsDefensible", () => F.And(F.Bool3(F.Of(this.HasAuthorization)), F.Bool3(F.Of(this.ContentMatchesApproval)), F.Bool3(F.Of(this.WasApprovedBeforeSending))))); set { }
        }


        public string ProcedureExecution { get; set; }
        public string? SendingStepExecution { get; set; }
        public string? AuthorizingStepExecution { get; set; }
        public string? MessageTemplate { get; set; }

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

        private StepExecution _stepExecution;

        [ForeignKey("SendingStepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(SendingStepExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. SendingStepExecution: " + SendingStepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = base.SoAContext.StepExecutions.Find(SendingStepExecution);
                    if (_stepExecution != null)
                    {
                        base.SoAContext.Attach(_stepExecution);
                    }
                }
                return _stepExecution;
            }
            set
            {
                if (_stepExecution != value)
                {
                    _stepExecution = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepExecution != null)
                    {
                        SendingStepExecution = _stepExecution.StepExecutionId;
                    }
                }
            }
        }

        private StepExecution _stepExecutionRef;

        [ForeignKey("AuthorizingStepExecution")]
        public virtual StepExecution StepExecutionRef
        {
            get
            {
                if (_stepExecutionRef == null && !string.IsNullOrEmpty(AuthorizingStepExecution))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecutionRef - no database context is set. AuthorizingStepExecution: " + AuthorizingStepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecutionRef = base.SoAContext.StepExecutions.Find(AuthorizingStepExecution);
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
                        AuthorizingStepExecution = _stepExecutionRef.StepExecutionId;
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


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecutionRef;
            _ = this.StepExecution;
            _ = this.StepExecutionRef;
            _ = this.MessageTemplateRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
