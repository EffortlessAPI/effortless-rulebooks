
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
    [Table("ModelProposals")]
    public class ModelProposalBase : SoAEntityBase
    {
        [Key]
        public string ModelProposalId { get; set; }

        // Formula Name (rulebook: ={{ProposalKind}} & ": " & LEFT({{Content}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ProposalKind)), F.S(": "), F.Text(F.Left(F.Of(this.Content), F.I(50)))))); set { }
        }

        public string? ProposalKind { get; set; }
        public DateTimeOffset? ProposedAt { get; set; }
        public string? Content { get; set; }
        public string? QualityCheckOutcome { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
        public string? ReviewOutcome { get; set; }
        public DateTimeOffset? CommittedAt { get; set; }
        // Formula ProposerKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ProposedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? ProposerKind
        {
            get => F.AsString(F.Memo(this, "ProposerKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ProposedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula ReviewerKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{ReviewedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? ReviewerKind
        {
            get => F.AsString(F.Memo(this, "ReviewerKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.ReviewedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula CommitterKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{CommittedByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? CommitterKind
        {
            get => F.AsString(F.Memo(this, "CommitterKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.CommittedByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula ModelStewardAgent (rulebook: =INDEX(GovernedModels!{{CurrentStewardAgent}}, MATCH({{GovernedModel}}, GovernedModels!{{GovernedModelId}}, 0)))
        [NotMapped]
        public string? ModelStewardAgent
        {
            get => F.AsString(F.Memo(this, "ModelStewardAgent", () => F.Lookup<GovernedModel>(this, "GovernedModels", "GovernedModelId", __c => __c.GovernedModels, __r => F.Of(__r.GovernedModelId), F.Of(this.GovernedModel), __r => F.Of(__r.CurrentStewardAgent), () => F.Of(new GovernedModel().CurrentStewardAgent)))); set { }
        }

        // Formula IsAiCandidate (rulebook: ={{ProposerKind}} = "AIAgent")
        [NotMapped]
        public bool? IsAiCandidate
        {
            get => F.AsBool(F.Memo(this, "IsAiCandidate", () => F.Eq(F.Of(this.ProposerKind), F.S("AIAgent")))); set { }
        }

        // Formula IsOnlyProposed (rulebook: ={{CommittedAt}} = "")
        [NotMapped]
        public bool? IsOnlyProposed
        {
            get => F.AsBool(F.Memo(this, "IsOnlyProposed", () => F.IsBlank(F.Of(this.CommittedAt)))); set { }
        }

        // Formula CommittedWithoutExpertReview (rulebook: =AND({{CommittedAt}} <> "", OR({{ReviewedAt}} = "", {{ReviewerKind}} <> "Human")))
        [NotMapped]
        public bool? CommittedWithoutExpertReview
        {
            get => F.AsBool(F.Memo(this, "CommittedWithoutExpertReview", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Or(F.Bool3(F.IsBlank(F.Of(this.ReviewedAt))), F.Bool3(F.Ne(F.Of(this.ReviewerKind), F.S("Human")))))))); set { }
        }

        // Formula EnteredWithoutQualityCheck (rulebook: =AND({{CommittedAt}} <> "", {{QualityCheckOutcome}} <> "Pass"))
        [NotMapped]
        public bool? EnteredWithoutQualityCheck
        {
            get => F.AsBool(F.Memo(this, "EnteredWithoutQualityCheck", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Ne(F.Nullif(F.Of(this.QualityCheckOutcome)), F.S("Pass")))))); set { }
        }

        // Formula ApproverUnknown (rulebook: =AND({{CommittedAt}} <> "", {{ReviewedByAgent}} = ""))
        [NotMapped]
        public bool? ApproverUnknown
        {
            get => F.AsBool(F.Memo(this, "ApproverUnknown", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.IsBlank(F.Of(this.ReviewedByAgent)))))); set { }
        }

        // Formula CommittedOutsideAnyVersion (rulebook: =AND({{CommittedAt}} <> "", {{AdoptedInRelease}} = "", {{AdoptedInDataVersion}} = ""))
        [NotMapped]
        public bool? CommittedOutsideAnyVersion
        {
            get => F.AsBool(F.Memo(this, "CommittedOutsideAnyVersion", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.IsBlank(F.Of(this.AdoptedInRelease))), F.Bool3(F.IsBlank(F.Of(this.AdoptedInDataVersion)))))); set { }
        }

        // Formula AiQuestionAdoptedUnvetted (rulebook: =AND({{IsAiCandidate}}, {{ProposalKind}} = "CompetencyQuestion", {{CommittedAt}} <> "", {{ReviewedAt}} = ""))
        [NotMapped]
        public bool? AiQuestionAdoptedUnvetted
        {
            get => F.AsBool(F.Memo(this, "AiQuestionAdoptedUnvetted", () => F.And(F.Bool3(F.Of(this.IsAiCandidate)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("CompetencyQuestion"))), F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.IsBlank(F.Of(this.ReviewedAt)))))); set { }
        }

        // Formula AiAlignmentDecidedByAi (rulebook: =AND({{IsAiCandidate}}, {{ProposalKind}} = "Alignment", {{CommittedAt}} <> "", {{CommitterKind}} <> "Human"))
        [NotMapped]
        public bool? AiAlignmentDecidedByAi
        {
            get => F.AsBool(F.Memo(this, "AiAlignmentDecidedByAi", () => F.And(F.Bool3(F.Of(this.IsAiCandidate)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("Alignment"))), F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Ne(F.Of(this.CommitterKind), F.S("Human")))))); set { }
        }

        // Formula AiAxiomWithoutHumanReview (rulebook: =AND({{IsAiCandidate}}, OR({{ProposalKind}} = "Axiom", {{ProposalKind}} = "Disjointness", {{ProposalKind}} = "DomainRange"), {{CommittedAt}} <> "", {{ReviewerKind}} <> "Human"))
        [NotMapped]
        public bool? AiAxiomWithoutHumanReview
        {
            get => F.AsBool(F.Memo(this, "AiAxiomWithoutHumanReview", () => F.And(F.Bool3(F.Of(this.IsAiCandidate)), F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("Axiom"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("Disjointness"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("DomainRange"))))), F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Ne(F.Of(this.ReviewerKind), F.S("Human")))))); set { }
        }

        // Formula AdoptionNotAnsweredByPerson (rulebook: =AND({{IsAiCandidate}}, {{CommittedAt}} <> "", {{CommitterKind}} <> "Human"))
        [NotMapped]
        public bool? AdoptionNotAnsweredByPerson
        {
            get => F.AsBool(F.Memo(this, "AdoptionNotAnsweredByPerson", () => F.And(F.Bool3(F.Of(this.IsAiCandidate)), F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Ne(F.Of(this.CommitterKind), F.S("Human")))))); set { }
        }

        // Formula HasNoHumanTouchpoint (rulebook: =AND({{CommittedAt}} <> "", {{ProposerKind}} <> "Human", {{ReviewerKind}} <> "Human", {{CommitterKind}} <> "Human"))
        [NotMapped]
        public bool? HasNoHumanTouchpoint
        {
            get => F.AsBool(F.Memo(this, "HasNoHumanTouchpoint", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Ne(F.Of(this.ProposerKind), F.S("Human"))), F.Bool3(F.Ne(F.Of(this.ReviewerKind), F.S("Human"))), F.Bool3(F.Ne(F.Of(this.CommitterKind), F.S("Human")))))); set { }
        }

        // Formula AiInstanceDataLoadedWithoutSteward (rulebook: =AND({{IsAiCandidate}}, {{ProposalKind}} = "InstanceData", {{CommittedAt}} <> "", {{ReviewedByAgent}} <> {{ModelStewardAgent}}))
        [NotMapped]
        public bool? AiInstanceDataLoadedWithoutSteward
        {
            get => F.AsBool(F.Memo(this, "AiInstanceDataLoadedWithoutSteward", () => F.And(F.Bool3(F.Of(this.IsAiCandidate)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("InstanceData"))), F.Bool3(F.IsNotBlank(F.Of(this.CommittedAt))), F.Bool3(F.Ne(F.Nullif(F.Of(this.ReviewedByAgent)), F.Of(this.ModelStewardAgent)))))); set { }
        }

        // Formula AwaitsEngineerVetting (rulebook: =AND({{IsAiCandidate}}, {{ProposalKind}} = "CompetencyQuestion", {{SourceDocument}} <> "", {{ReviewedAt}} = "", {{CommittedAt}} = ""))
        [NotMapped]
        public bool? AwaitsEngineerVetting
        {
            get => F.AsBool(F.Memo(this, "AwaitsEngineerVetting", () => F.And(F.Bool3(F.Of(this.IsAiCandidate)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("CompetencyQuestion"))), F.Bool3(F.IsNotBlank(F.Of(this.SourceDocument))), F.Bool3(F.IsBlank(F.Of(this.ReviewedAt))), F.Bool3(F.IsBlank(F.Of(this.CommittedAt)))))); set { }
        }

        // Formula IsPendingAlignmentDecision (rulebook: =AND({{ProposalKind}} = "Alignment", {{ReviewOutcome}} = "", {{CommittedAt}} = ""))
        [NotMapped]
        public bool? IsPendingAlignmentDecision
        {
            get => F.AsBool(F.Memo(this, "IsPendingAlignmentDecision", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("Alignment"))), F.Bool3(F.IsBlank(F.Of(this.ReviewOutcome))), F.Bool3(F.IsBlank(F.Of(this.CommittedAt)))))); set { }
        }

        // Formula AwaitsStewardApproval (rulebook: =AND({{ProposalKind}} = "InstanceData", {{ReviewedAt}} = "", {{CommittedAt}} = ""))
        [NotMapped]
        public bool? AwaitsStewardApproval
        {
            get => F.AsBool(F.Memo(this, "AwaitsStewardApproval", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.ProposalKind)), F.S("InstanceData"))), F.Bool3(F.IsBlank(F.Of(this.ReviewedAt))), F.Bool3(F.IsBlank(F.Of(this.CommittedAt)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? ProposedByAgent { get; set; }
        public string? SourceDocument { get; set; }
        public string? ReviewedByAgent { get; set; }
        public string? CommittedByAgent { get; set; }
        public string? AdoptedInRelease { get; set; }
        public string? AdoptedInDataVersion { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("ProposedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(ProposedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. ProposedByAgent: " + ProposedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(ProposedByAgent);
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
                        ProposedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("SourceDocument")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(SourceDocument))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. SourceDocument: " + SourceDocument + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(SourceDocument);
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
                        SourceDocument = _resource.ResourceId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("ReviewedByAgent")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(ReviewedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. ReviewedByAgent: " + ReviewedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(ReviewedByAgent);
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
                        ReviewedByAgent = _agentRef.AgentId;
                    }
                }
            }
        }

        private Agent _agentRefRef;

        [ForeignKey("CommittedByAgent")]
        public virtual Agent AgentRefRef
        {
            get
            {
                if (_agentRefRef == null && !string.IsNullOrEmpty(CommittedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRefRef - no database context is set. CommittedByAgent: " + CommittedByAgent + ".");
                        }
                        return null;
                    }
                    _agentRefRef = base.SoAContext.Agents.Find(CommittedByAgent);
                    if (_agentRefRef != null)
                    {
                        base.SoAContext.Attach(_agentRefRef);
                    }
                }
                return _agentRefRef;
            }
            set
            {
                if (_agentRefRef != value)
                {
                    _agentRefRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_agentRefRef != null)
                    {
                        CommittedByAgent = _agentRefRef.AgentId;
                    }
                }
            }
        }

        private RulebookRelease _rulebookRelease;

        [ForeignKey("AdoptedInRelease")]
        public virtual RulebookRelease RulebookRelease
        {
            get
            {
                if (_rulebookRelease == null && !string.IsNullOrEmpty(AdoptedInRelease))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookRelease - no database context is set. AdoptedInRelease: " + AdoptedInRelease + ".");
                        }
                        return null;
                    }
                    _rulebookRelease = base.SoAContext.RulebookReleases.Find(AdoptedInRelease);
                    if (_rulebookRelease != null)
                    {
                        base.SoAContext.Attach(_rulebookRelease);
                    }
                }
                return _rulebookRelease;
            }
            set
            {
                if (_rulebookRelease != value)
                {
                    _rulebookRelease = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookRelease != null)
                    {
                        AdoptedInRelease = _rulebookRelease.RulebookReleaseId;
                    }
                }
            }
        }

        private InstanceDataVersion _instanceDataVersion;

        [ForeignKey("AdoptedInDataVersion")]
        public virtual InstanceDataVersion InstanceDataVersion
        {
            get
            {
                if (_instanceDataVersion == null && !string.IsNullOrEmpty(AdoptedInDataVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access InstanceDataVersion - no database context is set. AdoptedInDataVersion: " + AdoptedInDataVersion + ".");
                        }
                        return null;
                    }
                    _instanceDataVersion = base.SoAContext.InstanceDataVersions.Find(AdoptedInDataVersion);
                    if (_instanceDataVersion != null)
                    {
                        base.SoAContext.Attach(_instanceDataVersion);
                    }
                }
                return _instanceDataVersion;
            }
            set
            {
                if (_instanceDataVersion != value)
                {
                    _instanceDataVersion = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_instanceDataVersion != null)
                    {
                        AdoptedInDataVersion = _instanceDataVersion.InstanceDataVersionId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.Agent;
            _ = this.Resource;
            _ = this.AgentRef;
            _ = this.AgentRefRef;
            _ = this.RulebookRelease;
            _ = this.InstanceDataVersion;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
