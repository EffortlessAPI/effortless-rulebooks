
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
    [Table("KnowledgeTransfers")]
    public class KnowledgeTransferBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeTransferId { get; set; }

        // Formula Name (rulebook: ={{KnowHow}} & " via " & {{Channel}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.KnowHow)), F.S(" via "), F.Text(F.Of(this.Channel))))); set { }
        }

        public string? Channel { get; set; }
        public DateTimeOffset? OccurredAt { get; set; }
        public bool? OnAllocatedTime { get; set; }
        // Formula FromOrganization (rulebook: =INDEX(Agents!{{Organization}}, MATCH({{FromAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? FromOrganization
        {
            get => F.AsString(F.Memo(this, "FromOrganization", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.FromAgent), __r => F.Of(__r.Organization), () => F.Of(new Agent().Organization)))); set { }
        }

        // Formula RecipientRoleCount (rulebook: =INDEX(Agents!{{CountOfCurrentRoleAssignments}}, MATCH({{RecipientAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public int? RecipientRoleCount
        {
            get => F.AsInt(F.Memo(this, "RecipientRoleCount", () => F.Integer(F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.RecipientAgent), __r => F.Of(__r.CountOfCurrentRoleAssignments), () => F.Of(new Agent().CountOfCurrentRoleAssignments))))); set { }
        }

        // Formula IsTraditionalChannel (rulebook: =OR({{Channel}} = "Document", {{Channel}} = "TrainingProgram", {{Channel}} = "Mentoring"))
        [NotMapped]
        public bool? IsTraditionalChannel
        {
            get => F.AsBool(F.Memo(this, "IsTraditionalChannel", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("Document"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("TrainingProgram"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("Mentoring")))))); set { }
        }

        // Formula IsSocialNetworkChannel (rulebook: =OR({{Channel}} = "PersonalRelationship", {{Channel}} = "Collaboration", {{Channel}} = "DesignProductionIteration", {{Channel}} = "EmployerChange"))
        [NotMapped]
        public bool? IsSocialNetworkChannel
        {
            get => F.AsBool(F.Memo(this, "IsSocialNetworkChannel", () => F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("PersonalRelationship"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("Collaboration"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("DesignProductionIteration"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("EmployerChange")))))); set { }
        }

        // Formula IsAmbientAbsorptionByNonPractitioner (rulebook: =AND({{RecipientAgent}} <> "", {{Channel}} = "AmbientExposure", {{RecipientRoleCount}} = 0))
        [NotMapped]
        public bool? IsAmbientAbsorptionByNonPractitioner
        {
            get => F.AsBool(F.Memo(this, "IsAmbientAbsorptionByNonPractitioner", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.RecipientAgent))), F.Bool3(F.Eq(F.Nullif(F.Of(this.Channel)), F.S("AmbientExposure"))), F.Bool3(F.Eq(F.Of(this.RecipientRoleCount), F.I(0)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        // Formula KnowHowTopic (rulebook: =INDEX(KnowHowCarriers!{{Topic}}, MATCH({{KnowHow}}, KnowHowCarriers!{{KnowHowCarrierId}}, 0)))
        [NotMapped]
        public string? KnowHowTopic
        {
            get => F.AsString(F.Memo(this, "KnowHowTopic", () => F.Lookup<KnowHowCarrier>(this, "KnowHowCarriers", "KnowHowCarrierId", __c => __c.KnowHowCarriers, __r => F.Of(__r.KnowHowCarrierId), F.Of(this.KnowHow), __r => F.Of(__r.Topic), () => F.Of(new KnowHowCarrier().Topic)))); set { }
        }


        public string? KnowHow { get; set; }
        public string? FromAgent { get; set; }
        public string? RecipientAgent { get; set; }
        public string? CommunityOfPractice { get; set; }

        private KnowHowCarrier _knowHowCarrier;

        [ForeignKey("KnowHow")]
        public virtual KnowHowCarrier KnowHowCarrier
        {
            get
            {
                if (_knowHowCarrier == null && !string.IsNullOrEmpty(KnowHow))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowHowCarrier - no database context is set. KnowHow: " + KnowHow + ".");
                        }
                        return null;
                    }
                    _knowHowCarrier = base.SoAContext.KnowHowCarriers.Find(KnowHow);
                    if (_knowHowCarrier != null)
                    {
                        base.SoAContext.Attach(_knowHowCarrier);
                    }
                }
                return _knowHowCarrier;
            }
            set
            {
                if (_knowHowCarrier != value)
                {
                    _knowHowCarrier = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowHowCarrier != null)
                    {
                        KnowHow = _knowHowCarrier.KnowHowCarrierId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("FromAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(FromAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. FromAgent: " + FromAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(FromAgent);
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
                        FromAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("RecipientAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(RecipientAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. RecipientAgent: " + RecipientAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(RecipientAgent);
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
                        RecipientAgent = _agentRef.AgentId;
                    }
                }
            }
        }

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


        protected override void LazyLoadProperties()
        {
            _ = this.KnowHowCarrier;
            _ = this.Agent;
            _ = this.AgentRef;
            _ = this.CommunitiesOfPractice;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
