
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("DeliveredCommunications")]
    public class DeliveredCommunicationBase : SoAEntityBase
    {
        [Key]
        public string DeliveredCommunicationId { get; set; }

        // Formula Name (rulebook: ={{Channel}} & " -> " & {{RecipientKey}} & " @ " & {{SentAt}})
        public string? Name
        {
            get => this.Channel + " -> " + this.RecipientKey + " @ " + this.SentAt; set { }
        }

        public string? Channel { get; set; }
        public string? RecipientKey { get; set; }
        public DateTime? SentAt { get; set; }
        public string? RenderedContentHash { get; set; }
        public string? ApprovedContentHash { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula HasAuthorization (rulebook: ={{AuthorizingStepExecution}} <> "")
        public bool? HasAuthorization
        {
            get => this.AuthorizingStepExecution <> ""; set { }
        }

        // Formula ContentMatchesApproval (rulebook: ={{RenderedContentHash}} = {{ApprovedContentHash}})
        public bool? ContentMatchesApproval
        {
            get => this.RenderedContentHash = this.ApprovedContentHash; set { }
        }

        // Formula AuthorizedAt (rulebook: =INDEX(StepExecutions!{{EndedAt}}, MATCH({{AuthorizingStepExecution}}, StepExecutions!{{StepExecutionId}}, 0)))
        public DateTime? AuthorizedAt
        {
            get => INDEX(StepExecutions!this.EndedAt, MATCH(this.AuthorizingStepExecution, StepExecutions!this.StepExecutionId, 0)); set { }
        }

        // Formula WasApprovedBeforeSending (rulebook: ={{AuthorizedAt}} <= {{SentAt}})
        public bool? WasApprovedBeforeSending
        {
            get => this.AuthorizedAt <= this.SentAt; set { }
        }

        // Formula IsDefensible (rulebook: =AND({{HasAuthorization}}, {{ContentMatchesApproval}}, {{WasApprovedBeforeSending}}))
        public bool? IsDefensible
        {
            get => AND(this.HasAuthorization, this.ContentMatchesApproval, this.WasApprovedBeforeSending); set { }
        }


        public string ProcedureExecution { get; set; }
        public string? SendingStepExecution { get; set; }
        public string? AuthorizingStepExecution { get; set; }
        public string? MessageTemplate { get; set; }

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

        [ForeignKey("SendingStepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(SendingStepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. SendingStepExecution: " + SendingStepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(SendingStepExecution);
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
                    SendingStepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
                }
            }
        }

        private StepExecution _stepExecution;

        [ForeignKey("AuthorizingStepExecution")]
        public virtual StepExecution StepExecution
        {
            get
            {
                if (_stepExecution == null && !string.IsNullOrEmpty(AuthorizingStepExecution))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepExecution - no database context is set. AuthorizingStepExecution: " + AuthorizingStepExecution + ".");
                        }
                        return null;
                    }
                    _stepExecution = Context.StepExecutions.Find(AuthorizingStepExecution);
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
                    AuthorizingStepExecution = _stepExecution == null ? default : _stepExecution.StepExecutionId;
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


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureExecution;
            _ = this.StepExecution;
            _ = this.StepExecution;
            _ = this.MessageTemplate;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
