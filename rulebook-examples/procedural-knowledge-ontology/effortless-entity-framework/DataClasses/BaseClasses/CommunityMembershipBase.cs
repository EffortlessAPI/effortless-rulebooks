
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
    [Table("CommunityMemberships")]
    public class CommunityMembershipBase : SoAEntityBase
    {
        [Key]
        public string CommunityMembershipId { get; set; }

        // Formula Name (rulebook: ={{Agent}} & " in " & {{CommunityOfPractice}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Agent)), F.S(" in "), F.Text(F.Of(this.CommunityOfPractice))))); set { }
        }

        public bool? IsSpecialist { get; set; }
        public DateTimeOffset? JoinedAt { get; set; }
        // Formula MemberOrganization (rulebook: =INDEX(Agents!{{Organization}}, MATCH({{Agent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? MemberOrganization
        {
            get => F.AsString(F.Memo(this, "MemberOrganization", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.Agent), __r => F.Of(__r.Organization), () => F.Of(new Agent().Organization)))); set { }
        }

        // Formula CommunityOrganization (rulebook: =INDEX(CommunitiesOfPractice!{{Organization}}, MATCH({{CommunityOfPractice}}, CommunitiesOfPractice!{{CommunityOfPracticeId}}, 0)))
        [NotMapped]
        public string? CommunityOrganization
        {
            get => F.AsString(F.Memo(this, "CommunityOrganization", () => F.Lookup<CommunitiesOfPractice>(this, "CommunitiesOfPractice", "CommunityOfPracticeId", __c => __c.CommunitiesOfPractice, __r => F.Of(__r.CommunityOfPracticeId), F.Of(this.CommunityOfPractice), __r => F.Of(__r.Organization), () => F.Of(new CommunitiesOfPractice().Organization)))); set { }
        }

        // Formula IsExternalMember (rulebook: =AND({{MemberOrganization}} <> "", {{MemberOrganization}} <> {{CommunityOrganization}}))
        [NotMapped]
        public bool? IsExternalMember
        {
            get => F.AsBool(F.Memo(this, "IsExternalMember", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.MemberOrganization))), F.Bool3(F.Ne(F.Of(this.MemberOrganization), F.Of(this.CommunityOrganization)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? CommunityOfPractice { get; set; }
        public string? Agent { get; set; }

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

        private Agent _agentRef;

        [ForeignKey("Agent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(Agent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. Agent: " + Agent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(Agent);
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
                        Agent = _agentRef.AgentId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CommunitiesOfPractice;
            _ = this.AgentRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
