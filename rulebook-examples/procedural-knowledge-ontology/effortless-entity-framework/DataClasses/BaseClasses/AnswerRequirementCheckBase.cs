
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
    [Table("AnswerRequirementChecks")]
    public class AnswerRequirementCheckBase : SoAEntityBase
    {
        [Key]
        public string AnswerRequirementCheckId { get; set; }

        // Formula Name (rulebook: ={{AssistantAnswer}} & " vs " & {{Requirement}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.AssistantAnswer)), F.S(" vs "), F.Text(F.Of(this.Requirement))))); set { }
        }

        public string? Verdict { get; set; }
        public DateTimeOffset? CheckedAt { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? AssistantAnswer { get; set; }
        public string? Requirement { get; set; }
        public string? CheckedByAgent { get; set; }

        private AssistantAnswer _assistantAnswerRef;

        [ForeignKey("AssistantAnswer")]
        public virtual AssistantAnswer AssistantAnswerRef
        {
            get
            {
                if (_assistantAnswerRef == null && !string.IsNullOrEmpty(AssistantAnswer))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssistantAnswerRef - no database context is set. AssistantAnswer: " + AssistantAnswer + ".");
                        }
                        return null;
                    }
                    _assistantAnswerRef = base.SoAContext.AssistantAnswers.Find(AssistantAnswer);
                    if (_assistantAnswerRef != null)
                    {
                        base.SoAContext.Attach(_assistantAnswerRef);
                    }
                }
                return _assistantAnswerRef;
            }
            set
            {
                if (_assistantAnswerRef != value)
                {
                    _assistantAnswerRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_assistantAnswerRef != null)
                    {
                        AssistantAnswer = _assistantAnswerRef.AssistantAnswerId;
                    }
                }
            }
        }

        private Requirement _requirementRef;

        [ForeignKey("Requirement")]
        public virtual Requirement RequirementRef
        {
            get
            {
                if (_requirementRef == null && !string.IsNullOrEmpty(Requirement))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequirementRef - no database context is set. Requirement: " + Requirement + ".");
                        }
                        return null;
                    }
                    _requirementRef = base.SoAContext.Requirements.Find(Requirement);
                    if (_requirementRef != null)
                    {
                        base.SoAContext.Attach(_requirementRef);
                    }
                }
                return _requirementRef;
            }
            set
            {
                if (_requirementRef != value)
                {
                    _requirementRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_requirementRef != null)
                    {
                        Requirement = _requirementRef.RequirementId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("CheckedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(CheckedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. CheckedByAgent: " + CheckedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(CheckedByAgent);
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
                        CheckedByAgent = _agent.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AssistantAnswerRef;
            _ = this.RequirementRef;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
