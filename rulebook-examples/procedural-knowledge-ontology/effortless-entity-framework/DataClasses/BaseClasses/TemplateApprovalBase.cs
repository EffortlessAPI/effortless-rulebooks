
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("TemplateApprovals")]
    public class TemplateApprovalBase : SoAEntityBase
    {
        [Key]
        public string TemplateApprovalId { get; set; }

        // Formula Name (rulebook: ={{MessageTemplate}} & " / " & {{Decision}} & " / " & {{DecidedAt}})
        public string? Name
        {
            get => this.MessageTemplate + " / " + this.Decision + " / " + this.DecidedAt; set { }
        }

        public string? Decision { get; set; }
        public DateTime? DecidedAt { get; set; }
        public string? ApprovedBodyHash { get; set; }
        public string? Notes { get; set; }
        // Formula IsApprovalDecision (rulebook: ={{Decision}} = "Approved")
        public bool? IsApprovalDecision
        {
            get => this.Decision = "Approved"; set { }
        }

        // Formula TemplatePolicy (rulebook: =INDEX(MessageTemplates!{{CommunicationPolicy}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        public string? TemplatePolicy
        {
            get => INDEX(MessageTemplates!this.CommunicationPolicy, MATCH(this.MessageTemplate, MessageTemplates!this.MessageTemplateId, 0)); set { }
        }

        // Formula RequiredApprovalRole (rulebook: =INDEX(CommunicationPolicies!{{ApprovalRole}}, MATCH({{TemplatePolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        public string? RequiredApprovalRole
        {
            get => INDEX(CommunicationPolicies!this.ApprovalRole, MATCH(this.TemplatePolicy, CommunicationPolicies!this.CommunicationPolicyId, 0)); set { }
        }

        // Formula IsDecidedByRequiredRole (rulebook: ={{DecidedInRole}} = {{RequiredApprovalRole}})
        public bool? IsDecidedByRequiredRole
        {
            get => this.DecidedInRole = this.RequiredApprovalRole; set { }
        }

        // Formula ValidApprovalTemplateKey (rulebook: =IF(AND({{IsApprovalDecision}}, {{IsDecidedByRequiredRole}}), {{MessageTemplate}}, ""))
        public string? ValidApprovalTemplateKey
        {
            get => IF(AND(this.IsApprovalDecision, this.IsDecidedByRequiredRole), this.MessageTemplate, ""); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? MessageTemplate { get; set; }
        public string? DecidedByAgent { get; set; }
        public string? DecidedInRole { get; set; }

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

        [ForeignKey("DecidedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(DecidedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. DecidedByAgent: " + DecidedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(DecidedByAgent);
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
                    DecidedByAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Role _role;

        [ForeignKey("DecidedInRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(DecidedInRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. DecidedInRole: " + DecidedInRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(DecidedInRole);
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
                    DecidedInRole = _role == null ? default : _role.RoleId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.MessageTemplate;
            _ = this.Agent;
            _ = this.Role;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
