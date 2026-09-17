
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
    [Table("AgentIntegrations")]
    public class AgentIntegrationBase : SoAEntityBase
    {
        [Key]
        public string AgentIntegrationId { get; set; }

        // Formula Name (rulebook: ={{Agent}} & " via " & {{Pathway}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Agent)), F.S(" via "), F.Text(F.Of(this.Pathway))))); set { }
        }

        public string? DeliveryMode { get; set; }
        public string? RegistryEntryKey { get; set; }
        public DateTimeOffset? ConnectedAt { get; set; }
        // Formula PathwayIntegrationCount (rulebook: =INDEX(IntegrationPathways!{{IntegrationCountOnPathway}}, MATCH({{Pathway}}, IntegrationPathways!{{IntegrationPathwayId}}, 0)))
        [NotMapped]
        public int? PathwayIntegrationCount
        {
            get => F.AsInt(F.Memo(this, "PathwayIntegrationCount", () => F.Integer(F.Lookup<IntegrationPathway>(this, "IntegrationPathways", "IntegrationPathwayId", __c => __c.IntegrationPathways, __r => F.Of(__r.IntegrationPathwayId), F.Of(this.Pathway), __r => F.Of(__r.IntegrationCountOnPathway), () => F.Of(new IntegrationPathway().IntegrationCountOnPathway))))); set { }
        }

        // Formula ObservedAnswerCount (rulebook: =COUNTIFS(AssistantAnswers!{{ViaIntegration}}, {{AgentIntegrationId}}))
        [NotMapped]
        public int? ObservedAnswerCount
        {
            get => F.AsInt(F.Memo(this, "ObservedAnswerCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AssistantAnswer>(base.SoAContext, "AssistantAnswers", __c => __c.AssistantAnswers), __r => F.CritField(F.Of(__r.ViaIntegration), F.Of(this.AgentIntegrationId))))))); set { }
        }

        // Formula IsShadowIntegration (rulebook: =AND({{ObservedAnswerCount}} > 0, {{RegistryEntryKey}} = ""))
        [NotMapped]
        public bool? IsShadowIntegration
        {
            get => F.AsBool(F.Memo(this, "IsShadowIntegration", () => F.And(F.Bool3(F.Cmp(F.Of(this.ObservedAnswerCount), ">", F.I(0))), F.Bool3(F.IsBlank(F.Of(this.RegistryEntryKey)))))); set { }
        }

        // Formula IsOneOffConnection (rulebook: ={{PathwayIntegrationCount}} <= 1)
        [NotMapped]
        public bool? IsOneOffConnection
        {
            get => F.AsBool(F.Memo(this, "IsOneOffConnection", () => F.Cmp(F.Of(this.PathwayIntegrationCount), "<=", F.I(1)))); set { }
        }

        // Formula SnapshotIsGoverned (rulebook: =INDEX(GroundingSnapshots!{{IsGoverned}}, MATCH({{ServesSnapshot}}, GroundingSnapshots!{{GroundingSnapshotId}}, 0)))
        [NotMapped]
        public bool? SnapshotIsGoverned
        {
            get => F.AsBool(F.Memo(this, "SnapshotIsGoverned", () => F.Lookup<GroundingSnapshot>(this, "GroundingSnapshots", "GroundingSnapshotId", __c => __c.GroundingSnapshots, __r => F.Of(__r.GroundingSnapshotId), F.Of(this.ServesSnapshot), __r => F.Of(__r.IsGoverned), () => F.Of(new GroundingSnapshot().IsGoverned)))); set { }
        }

        // Formula IsDeployedOnUngovernedGraph (rulebook: =AND({{ServesSnapshot}} <> "", {{SnapshotIsGoverned}} = FALSE))
        [NotMapped]
        public bool? IsDeployedOnUngovernedGraph
        {
            get => F.AsBool(F.Memo(this, "IsDeployedOnUngovernedGraph", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.ServesSnapshot))), F.Bool3(F.Eq(F.Of(this.SnapshotIsGoverned), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Agent { get; set; }
        public string? KnowledgeSystem { get; set; }
        public string? Pathway { get; set; }
        public string? ServesSnapshot { get; set; }

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

        private KnowledgeConsumerSystem _knowledgeConsumerSystem;

        [ForeignKey("KnowledgeSystem")]
        public virtual KnowledgeConsumerSystem KnowledgeConsumerSystem
        {
            get
            {
                if (_knowledgeConsumerSystem == null && !string.IsNullOrEmpty(KnowledgeSystem))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeConsumerSystem - no database context is set. KnowledgeSystem: " + KnowledgeSystem + ".");
                        }
                        return null;
                    }
                    _knowledgeConsumerSystem = base.SoAContext.KnowledgeConsumerSystems.Find(KnowledgeSystem);
                    if (_knowledgeConsumerSystem != null)
                    {
                        base.SoAContext.Attach(_knowledgeConsumerSystem);
                    }
                }
                return _knowledgeConsumerSystem;
            }
            set
            {
                if (_knowledgeConsumerSystem != value)
                {
                    _knowledgeConsumerSystem = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeConsumerSystem != null)
                    {
                        KnowledgeSystem = _knowledgeConsumerSystem.KnowledgeConsumerSystemId;
                    }
                }
            }
        }

        private IntegrationPathway _integrationPathway;

        [ForeignKey("Pathway")]
        public virtual IntegrationPathway IntegrationPathway
        {
            get
            {
                if (_integrationPathway == null && !string.IsNullOrEmpty(Pathway))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access IntegrationPathway - no database context is set. Pathway: " + Pathway + ".");
                        }
                        return null;
                    }
                    _integrationPathway = base.SoAContext.IntegrationPathways.Find(Pathway);
                    if (_integrationPathway != null)
                    {
                        base.SoAContext.Attach(_integrationPathway);
                    }
                }
                return _integrationPathway;
            }
            set
            {
                if (_integrationPathway != value)
                {
                    _integrationPathway = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_integrationPathway != null)
                    {
                        Pathway = _integrationPathway.IntegrationPathwayId;
                    }
                }
            }
        }

        private GroundingSnapshot _groundingSnapshot;

        [ForeignKey("ServesSnapshot")]
        public virtual GroundingSnapshot GroundingSnapshot
        {
            get
            {
                if (_groundingSnapshot == null && !string.IsNullOrEmpty(ServesSnapshot))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GroundingSnapshot - no database context is set. ServesSnapshot: " + ServesSnapshot + ".");
                        }
                        return null;
                    }
                    _groundingSnapshot = base.SoAContext.GroundingSnapshots.Find(ServesSnapshot);
                    if (_groundingSnapshot != null)
                    {
                        base.SoAContext.Attach(_groundingSnapshot);
                    }
                }
                return _groundingSnapshot;
            }
            set
            {
                if (_groundingSnapshot != value)
                {
                    _groundingSnapshot = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_groundingSnapshot != null)
                    {
                        ServesSnapshot = _groundingSnapshot.GroundingSnapshotId;
                    }
                }
            }
        }

        private ObservableCollection<AssistantAnswer> _assistantAnswers;

        [InverseProperty("AgentIntegration")]
        public virtual ObservableCollection<AssistantAnswer> AssistantAnswers
        {
            get
            {
                if (_assistantAnswers == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AssistantAnswers - no database context is set. AgentIntegrationId: " + this.AgentIntegrationId + ".");
                        }
                        _assistantAnswers = new ObservableCollection<AssistantAnswer>();
                    }
                    else
                    {
                        var items = base.SoAContext.AssistantAnswers.Where(x => x.ViaIntegration == this.AgentIntegrationId).ToList<AssistantAnswer>();
                        _assistantAnswers = new ObservableCollection<AssistantAnswer>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _assistantAnswers.CollectionChanged += AssistantAnswers_CollectionChanged;
                }
                return _assistantAnswers;
            }
            private set
            {
                if (_assistantAnswers != null)
                {
                    _assistantAnswers.CollectionChanged -= AssistantAnswers_CollectionChanged;
                }
                _assistantAnswers = value;
                if (_assistantAnswers != null)
                {
                    _assistantAnswers.CollectionChanged += AssistantAnswers_CollectionChanged;
                }
            }
        }

        private void AssistantAnswers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AssistantAnswer>())
                {
                    item.ViaIntegration = this.AgentIntegrationId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AgentRef;
            _ = this.KnowledgeConsumerSystem;
            _ = this.IntegrationPathway;
            _ = this.GroundingSnapshot;
            _ = this.AssistantAnswers;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
