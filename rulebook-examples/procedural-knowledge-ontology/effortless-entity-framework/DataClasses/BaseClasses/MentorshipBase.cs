
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("Mentorships")]
    public class MentorshipBase : SoAEntityBase
    {
        [Key]
        public string MentorshipId { get; set; }

        // Formula Name (rulebook: ={{MentorAgent}} & " -> " & {{LearnerAgent}})
        public string? Name
        {
            get => this.MentorAgent + " -> " + this.LearnerAgent; set { }
        }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? LearningObjective { get; set; }
        public string? EvidenceOfCompletion { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? CommunityOfPractice { get; set; }
        public string? MentorAgent { get; set; }
        public string? LearnerAgent { get; set; }

        private CommunitiesOfPractice _communitiesOfPractice;

        [ForeignKey("CommunityOfPractice")]
        public virtual CommunitiesOfPractice CommunitiesOfPractice
        {
            get
            {
                if (_communitiesOfPractice == null && !string.IsNullOrEmpty(CommunityOfPractice))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. CommunityOfPractice: " + CommunityOfPractice + ".");
                        }
                        return null;
                    }
                    _communitiesOfPractice = Context.CommunitiesOfPractice.Find(CommunityOfPractice);
                    if (_communitiesOfPractice != null)
                    {
                        Context.Attach(_communitiesOfPractice);
                    }
                }
                return _communitiesOfPractice;
            }
            set
            {
                if (_communitiesOfPractice != value)
                {
                    _communitiesOfPractice = value;
                    CommunityOfPractice = _communitiesOfPractice == null ? default : _communitiesOfPractice.CommunityOfPracticeId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("MentorAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(MentorAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. MentorAgent: " + MentorAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(MentorAgent);
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
                    MentorAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("LearnerAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(LearnerAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. LearnerAgent: " + LearnerAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(LearnerAgent);
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
                    LearnerAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CommunitiesOfPractice;
            _ = this.Agent;
            _ = this.Agent;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
