
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("UserQuestions")]
    public class UserQuestionBase : SoAEntityBase
    {
        [Key]
        public string UserQuestionId { get; set; }

        // Formula Name (rulebook: =LEFT({{QuestionText}}, 70))
        public string? Name
        {
            get => LEFT(this.QuestionText, 70); set { }
        }

        public DateTime? AskedAt { get; set; }
        public string? QuestionText { get; set; }
        public string? Status { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? StepExecution { get; set; }
        public string? AskedByAgent { get; set; }
        public string? ResolvedByFaq { get; set; }
        public string? AddressedByResource { get; set; }

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

        private Agent _agent;

        [ForeignKey("AskedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AskedByAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AskedByAgent: " + AskedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(AskedByAgent);
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
                    AskedByAgent = _agent == null ? default : _agent.AgentId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access FAQ - no database context is set. ResolvedByFaq: " + ResolvedByFaq + ".");
                        }
                        return null;
                    }
                    _fAQ = Context.FAQs.Find(ResolvedByFaq);
                    if (_fAQ != null)
                    {
                        Context.Attach(_fAQ);
                    }
                }
                return _fAQ;
            }
            set
            {
                if (_fAQ != value)
                {
                    _fAQ = value;
                    ResolvedByFaq = _fAQ == null ? default : _fAQ.FaqId;
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
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. AddressedByResource: " + AddressedByResource + ".");
                        }
                        return null;
                    }
                    _resource = Context.Resources.Find(AddressedByResource);
                    if (_resource != null)
                    {
                        Context.Attach(_resource);
                    }
                }
                return _resource;
            }
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    AddressedByResource = _resource == null ? default : _resource.ResourceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.StepExecution;
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
