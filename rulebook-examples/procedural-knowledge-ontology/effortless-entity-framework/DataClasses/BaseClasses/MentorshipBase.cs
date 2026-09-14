
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
    [Table("Mentorships")]
    public class MentorshipBase : SoAEntityBase
    {
        [Key]
        public string MentorshipId { get; set; }

        // Formula Name (rulebook: ={{MentorAgent}} & " -> " & {{LearnerAgent}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.MentorAgent)), F.S(" -> "), F.Text(F.Of(this.LearnerAgent))))); set { }
        }

        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CommunitiesOfPractice - no database context is set. CommunityOfPractice: " + CommunityOfPractice + ".");
                        }
                        return null;
                    }
                    _communitiesOfPractice = base.SoAContext.CommunitiesOfPractice.Find(CommunityOfPractice);
                    if (_communitiesOfPractice != null)
                    {
                        base.SoAContext.Attach(_communitiesOfPractice);
                    }
                }
                return _communitiesOfPractice;
            }
            set
            {
                if (_communitiesOfPractice != value)
                {
                    _communitiesOfPractice = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_communitiesOfPractice != null)
                    {
                        CommunityOfPractice = _communitiesOfPractice.CommunityOfPracticeId;
                    }
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
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. MentorAgent: " + MentorAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(MentorAgent);
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
                        MentorAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("LearnerAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(LearnerAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. LearnerAgent: " + LearnerAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(LearnerAgent);
                    if (_agentRef != null)
                    {
                        base.SoAContext.Attach(_agentRef);
                    }
                }
                return _agentRef;
            }
            set
            {
                if (_agentRef != value)
                {
                    _agentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRef != null)
                    {
                        LearnerAgent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CommunitiesOfPractice;
            _ = this.Agent;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
