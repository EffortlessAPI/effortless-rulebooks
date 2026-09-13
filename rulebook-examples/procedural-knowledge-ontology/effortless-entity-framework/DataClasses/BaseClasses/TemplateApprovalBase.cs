
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
    [Table("TemplateApprovals")]
    public class TemplateApprovalBase : SoAEntityBase
    {
        [Key]
        public string TemplateApprovalId { get; set; }

        // Formula Name (rulebook: ={{MessageTemplate}} & " / " & {{Decision}} & " / " & {{DecidedAt}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.TextOr(F.Of(this.MessageTemplate)), F.S(" / "), F.TextOr(F.Of(this.Decision)), F.S(" / "), F.TimestamptzText(F.Of(this.DecidedAt))))); set { }
        }

        public string? Decision { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        public string? ApprovedBodyHash { get; set; }
        public string? Notes { get; set; }
        // Formula IsApprovalDecision (rulebook: ={{Decision}} = "Approved")
        [NotMapped]
        public bool? IsApprovalDecision
        {
            get => F.AsBool(F.Memo(this, "IsApprovalDecision", () => F.Eq(F.Nullif(F.Of(this.Decision)), F.S("Approved")))); set { }
        }

        // Formula TemplatePolicy (rulebook: =INDEX(MessageTemplates!{{CommunicationPolicy}}, MATCH({{MessageTemplate}}, MessageTemplates!{{MessageTemplateId}}, 0)))
        [NotMapped]
        public string? TemplatePolicy
        {
            get => F.AsString(F.Memo(this, "TemplatePolicy", () => F.Lookup<MessageTemplate>(this, "MessageTemplates", "MessageTemplateId", __c => __c.MessageTemplates, __r => F.Of(__r.MessageTemplateId), F.Of(this.MessageTemplate), __r => F.Of(__r.CommunicationPolicy), () => F.Of(new MessageTemplate().CommunicationPolicy)))); set { }
        }

        // Formula RequiredApprovalRole (rulebook: =INDEX(CommunicationPolicies!{{ApprovalRole}}, MATCH({{TemplatePolicy}}, CommunicationPolicies!{{CommunicationPolicyId}}, 0)))
        [NotMapped]
        public string? RequiredApprovalRole
        {
            get => F.AsString(F.Memo(this, "RequiredApprovalRole", () => F.Lookup<CommunicationPolicy>(this, "CommunicationPolicies", "CommunicationPolicyId", __c => __c.CommunicationPolicies, __r => F.Of(__r.CommunicationPolicyId), F.Of(this.TemplatePolicy), __r => F.Of(__r.ApprovalRole), () => F.Of(new CommunicationPolicy().ApprovalRole)))); set { }
        }

        // Formula IsDecidedByRequiredRole (rulebook: ={{DecidedInRole}} = {{RequiredApprovalRole}})
        [NotMapped]
        public bool? IsDecidedByRequiredRole
        {
            get => F.AsBool(F.Memo(this, "IsDecidedByRequiredRole", () => F.Eq(F.Nullif(F.Of(this.DecidedInRole)), F.Of(this.RequiredApprovalRole)))); set { }
        }

        // Formula ValidApprovalTemplateKey (rulebook: =IF(AND({{IsApprovalDecision}}, {{IsDecidedByRequiredRole}}), {{MessageTemplate}}, ""))
        [NotMapped]
        public string? ValidApprovalTemplateKey
        {
            get => F.AsString(F.Memo(this, "ValidApprovalTemplateKey", () => (F.Truthy(F.Bool3(F.And(F.Bool3(F.Of(this.IsApprovalDecision)), F.Bool3(F.Of(this.IsDecidedByRequiredRole))))) ? F.Of(this.MessageTemplate) : F.S("")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? MessageTemplate { get; set; }
        public string? DecidedByAgent { get; set; }
        public string? DecidedInRole { get; set; }

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

        [ForeignKey("DecidedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(DecidedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. DecidedByAgent: " + DecidedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(DecidedByAgent);
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
                        DecidedByAgent = _agent.AgentId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. DecidedInRole: " + DecidedInRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(DecidedInRole);
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
                        DecidedInRole = _role.RoleId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.MessageTemplateRef;
            _ = this.Agent;
            _ = this.Role;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
