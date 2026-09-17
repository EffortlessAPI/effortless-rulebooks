
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
    [Table("UserQuestions")]
    public class UserQuestionBase : SoAEntityBase
    {
        [Key]
        public string UserQuestionId { get; set; }

        // Formula Name (rulebook: =LEFT({{QuestionText}}, 70))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Left(F.Of(this.QuestionText), F.I(70)))); set { }
        }

        public DateTimeOffset? AskedAt { get; set; }
        public string? QuestionText { get; set; }
        public string? Status { get; set; }
        public string? SemanticTypeIri { get; set; }
        // Formula IsUnaddressedQuestion (rulebook: =AND({{AddressedByResource}} = "", {{ResolvedByFaq}} = ""))
        [NotMapped]
        public bool? IsUnaddressedQuestion
        {
            get => F.AsBool(F.Memo(this, "IsUnaddressedQuestion", () => F.And(F.Bool3(F.IsBlank(F.Of(this.AddressedByResource))), F.Bool3(F.IsBlank(F.Of(this.ResolvedByFaq)))))); set { }
        }


        public string? StepExecution { get; set; }
        public string? AskedByAgent { get; set; }
        public string? ResolvedByFaq { get; set; }
        public string? AddressedByResource { get; set; }

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

        private Agent _agent;

        [ForeignKey("AskedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AskedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AskedByAgent: " + AskedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AskedByAgent);
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
                        AskedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private FAQ _fAQ;

        [ForeignKey("ResolvedByFaq")]
        public virtual FAQ FAQ
        {
            get
            {
                if (_fAQ == null && !string.IsNullOrEmpty(ResolvedByFaq))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQ - no database context is set. ResolvedByFaq: " + ResolvedByFaq + ".");
                        }
                        return null;
                    }
                    _fAQ = base.SoAContext.FAQs.Find(ResolvedByFaq);
                    if (_fAQ != null)
                    {
                        base.SoAContext.Attach(_fAQ);
                    }
                }
                return _fAQ;
            }
            set
            {
                if (_fAQ != value)
                {
                    _fAQ = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_fAQ != null)
                    {
                        ResolvedByFaq = _fAQ.FaqId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("AddressedByResource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(AddressedByResource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. AddressedByResource: " + AddressedByResource + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(AddressedByResource);
                    if (_resource != null)
                    {
                        base.SoAContext.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_resource != null)
                    {
                        AddressedByResource = _resource.ResourceId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecutionRef;
            _ = this.Agent;
            _ = this.FAQ;
            _ = this.Resource;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
