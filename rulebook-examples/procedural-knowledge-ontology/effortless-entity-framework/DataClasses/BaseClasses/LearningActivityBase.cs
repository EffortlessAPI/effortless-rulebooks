
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("LearningActivities")]
    public class LearningActivityBase : SoAEntityBase
    {
        [Key]
        public string LearningActivityId { get; set; }

        // Formula Name (rulebook: ={{ActivityKind}} & " / " & {{OccurredAt}})
        public string? Name
        {
            get => this.ActivityKind + " / " + this.OccurredAt; set { }
        }

        public string? ActivityKind { get; set; }
        public DateTime? OccurredAt { get; set; }
        public string? Outcome { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? CommunityOfPractice { get; set; }
        public string? ProcedureVersion { get; set; }
        public string? FacilitatorAgent { get; set; }
        public string? EvidenceResource { get; set; }

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

        private ProcedureVersion _procedureVersion;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersion
        {
            get
            {
                if (_procedureVersion == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersion - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersion = Context.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersion != null)
                    {
                        Context.Attach(_procedureVersion);
                    }
                }
                return _procedureVersion;
            }
            set
            {
                if (_procedureVersion != value)
                {
                    _procedureVersion = value;
                    ProcedureVersion = _procedureVersion == null ? default : _procedureVersion.ProcedureVersionId;
                }
            }
        }

        private Agent _agent;

        [ForeignKey("FacilitatorAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(FacilitatorAgent))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. FacilitatorAgent: " + FacilitatorAgent + ".");
                        }
                        return null;
                    }
                    _agent = Context.Agents.Find(FacilitatorAgent);
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
                    FacilitatorAgent = _agent == null ? default : _agent.AgentId;
                }
            }
        }

        private Resource _resource;

        [ForeignKey("EvidenceResource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(EvidenceResource))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. EvidenceResource: " + EvidenceResource + ".");
                        }
                        return null;
                    }
                    _resource = Context.Resources.Find(EvidenceResource);
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
                    EvidenceResource = _resource == null ? default : _resource.ResourceId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CommunitiesOfPractice;
            _ = this.ProcedureVersion;
            _ = this.Agent;
            _ = this.Resource;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
