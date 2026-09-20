
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
    [Table("KnowledgeConsumerSystems")]
    public class KnowledgeConsumerSystemBase : SoAEntityBase
    {
        [Key]
        public string KnowledgeConsumerSystemId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? SystemKind { get; set; }
        public string? Audience { get; set; }
        public bool? HoldsProcedureKnowledge { get; set; }
        public bool? ExportsStandardFormat { get; set; }
        public string? ExportFormat { get; set; }
        public bool? HasReasoner { get; set; }
        public bool? HasSemanticStorage { get; set; }
        public bool? HasGraphAlgorithms { get; set; }
        public bool? HasMachineLearning { get; set; }
        public bool? HasTaxonomy { get; set; }
        public bool? HasThesaurus { get; set; }
        public bool? HasOntology { get; set; }
        public bool? HasMetadataSchema { get; set; }
        // Formula ModelSyncCount (rulebook: =COUNTIFS(ConsumerSystemSyncs!{{ConsumerSystem}}, {{KnowledgeConsumerSystemId}}))
        [NotMapped]
        public int? ModelSyncCount
        {
            get => F.AsInt(F.Memo(this, "ModelSyncCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ConsumerSystemSync>(base.SoAContext, "ConsumerSystemSyncs", __c => __c.ConsumerSystemSyncs), __r => F.CritField(F.Of(__r.ConsumerSystem), F.Of(this.KnowledgeConsumerSystemId))))))); set { }
        }

        // Formula IntegrationCount (rulebook: =COUNTIFS(AgentIntegrations!{{KnowledgeSystem}}, {{KnowledgeConsumerSystemId}}))
        [NotMapped]
        public int? IntegrationCount
        {
            get => F.AsInt(F.Memo(this, "IntegrationCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentIntegration>(base.SoAContext, "AgentIntegrations", __c => __c.AgentIntegrations), __r => F.CritField(F.Of(__r.KnowledgeSystem), F.Of(this.KnowledgeConsumerSystemId))))))); set { }
        }

        // Formula IsKnowledgeSilo (rulebook: =AND({{HoldsProcedureKnowledge}}, {{ExportsStandardFormat}} = FALSE, {{ModelSyncCount}} = 0))
        [NotMapped]
        public bool? IsKnowledgeSilo
        {
            get => F.AsBool(F.Memo(this, "IsKnowledgeSilo", () => F.And(F.IsTrueV(F.Of(this.HoldsProcedureKnowledge)), F.Bool3(F.Eq(F.Nullif(F.Of(this.ExportsStandardFormat)), F.B(false))), F.Bool3(F.Eq(F.Of(this.ModelSyncCount), F.I(0)))))); set { }
        }

        // Formula IsUnlinkedToolchainComponent (rulebook: =AND(OR({{SystemKind}} = "ProcessModelingTool", {{SystemKind}} = "SemanticRepository", {{SystemKind}} = "AIPlatform"), ({{ModelSyncCount}} + {{IntegrationCount}}) = 0))
        [NotMapped]
        public bool? IsUnlinkedToolchainComponent
        {
            get => F.AsBool(F.Memo(this, "IsUnlinkedToolchainComponent", () => F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Nullif(F.Of(this.SystemKind)), F.S("ProcessModelingTool"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.SystemKind)), F.S("SemanticRepository"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.SystemKind)), F.S("AIPlatform"))))), F.Bool3(F.Eq(F.Add(F.Of(this.ModelSyncCount), F.Of(this.IntegrationCount)), F.I(0)))))); set { }
        }

        // Formula SemanticLayerComponentCount (rulebook: =IF({{HasTaxonomy}}, 1, 0) + IF({{HasThesaurus}}, 1, 0) + IF({{HasOntology}}, 1, 0) + IF({{HasMetadataSchema}}, 1, 0) + IF({{HasReasoner}}, 1, 0))
        [NotMapped]
        public int? SemanticLayerComponentCount
        {
            get => F.AsInt(F.Memo(this, "SemanticLayerComponentCount", () => F.Integer(F.Add(F.Add(F.Add(F.Add((F.Truthy(F.IsTrueV(F.Of(this.HasTaxonomy))) ? F.I(1) : F.I(0)), (F.Truthy(F.IsTrueV(F.Of(this.HasThesaurus))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.HasOntology))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.HasMetadataSchema))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.HasReasoner))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula PlatformCapabilityCount (rulebook: =IF({{HasReasoner}}, 1, 0) + IF({{HasSemanticStorage}}, 1, 0) + IF({{HasGraphAlgorithms}}, 1, 0) + IF({{HasMachineLearning}}, 1, 0))
        [NotMapped]
        public int? PlatformCapabilityCount
        {
            get => F.AsInt(F.Memo(this, "PlatformCapabilityCount", () => F.Integer(F.Add(F.Add(F.Add((F.Truthy(F.IsTrueV(F.Of(this.HasReasoner))) ? F.I(1) : F.I(0)), (F.Truthy(F.IsTrueV(F.Of(this.HasSemanticStorage))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.HasGraphAlgorithms))) ? F.I(1) : F.I(0))), (F.Truthy(F.IsTrueV(F.Of(this.HasMachineLearning))) ? F.I(1) : F.I(0)))))); set { }
        }

        // Formula IsImmatureGraphPlatform (rulebook: =AND({{SystemKind}} = "KnowledgeGraphPlatform", {{PlatformCapabilityCount}} < 4))
        [NotMapped]
        public bool? IsImmatureGraphPlatform
        {
            get => F.AsBool(F.Memo(this, "IsImmatureGraphPlatform", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.SystemKind)), F.S("KnowledgeGraphPlatform"))), F.Bool3(F.Cmp(F.Of(this.PlatformCapabilityCount), "<", F.I(4)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }
        public bool? IsComputationallyQueryable { get; set; }
        public bool? IsComputationallyValidatable { get; set; }
        // Formula HoldsComputationallyEncodedProcedureKnowledge (rulebook: =AND({{HoldsProcedureKnowledge}}, {{IsComputationallyQueryable}}, {{IsComputationallyValidatable}}, {{HasReasoner}}))
        [NotMapped]
        public bool? HoldsComputationallyEncodedProcedureKnowledge
        {
            get => F.AsBool(F.Memo(this, "HoldsComputationallyEncodedProcedureKnowledge", () => F.And(F.IsTrueV(F.Of(this.HoldsProcedureKnowledge)), F.IsTrueV(F.Of(this.IsComputationallyQueryable)), F.IsTrueV(F.Of(this.IsComputationallyValidatable)), F.IsTrueV(F.Of(this.HasReasoner))))); set { }
        }

        // Formula StoresProcedureKnowledgeWithoutComputationalAccess (rulebook: =AND({{HoldsProcedureKnowledge}}, {{HoldsComputationallyEncodedProcedureKnowledge}} = FALSE))
        [NotMapped]
        public bool? StoresProcedureKnowledgeWithoutComputationalAccess
        {
            get => F.AsBool(F.Memo(this, "StoresProcedureKnowledgeWithoutComputationalAccess", () => F.And(F.IsTrueV(F.Of(this.HoldsProcedureKnowledge)), F.Bool3(F.Eq(F.Of(this.HoldsComputationallyEncodedProcedureKnowledge), F.B(false)))))); set { }
        }


        public string? Organization { get; set; }

        private Organization _organizationRef;

        [ForeignKey("Organization")]
        public virtual Organization OrganizationRef
        {
            get
            {
                if (_organizationRef == null && !string.IsNullOrEmpty(Organization))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OrganizationRef - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organizationRef = base.SoAContext.Organizations.Find(Organization);
                    if (_organizationRef != null)
                    {
                        base.SoAContext.Attach(_organizationRef);
                    }
                }
                return _organizationRef;
            }
            set
            {
                if (_organizationRef != value)
                {
                    _organizationRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_organizationRef != null)
                    {
                        Organization = _organizationRef.OrganizationId;
                    }
                }
            }
        }

        private ObservableCollection<ConsumerSystemSync> _consumerSystemSyncs;

        [InverseProperty("KnowledgeConsumerSystem")]
        public virtual ObservableCollection<ConsumerSystemSync> ConsumerSystemSyncs
        {
            get
            {
                if (_consumerSystemSyncs == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ConsumerSystemSyncs - no database context is set. KnowledgeConsumerSystemId: " + this.KnowledgeConsumerSystemId + ".");
                        }
                        _consumerSystemSyncs = new ObservableCollection<ConsumerSystemSync>();
                    }
                    else
                    {
                        var items = base.SoAContext.ConsumerSystemSyncs.Where(x => x.ConsumerSystem == this.KnowledgeConsumerSystemId).ToList<ConsumerSystemSync>();
                        _consumerSystemSyncs = new ObservableCollection<ConsumerSystemSync>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _consumerSystemSyncs.CollectionChanged += ConsumerSystemSyncs_CollectionChanged;
                }
                return _consumerSystemSyncs;
            }
            private set
            {
                if (_consumerSystemSyncs != null)
                {
                    _consumerSystemSyncs.CollectionChanged -= ConsumerSystemSyncs_CollectionChanged;
                }
                _consumerSystemSyncs = value;
                if (_consumerSystemSyncs != null)
                {
                    _consumerSystemSyncs.CollectionChanged += ConsumerSystemSyncs_CollectionChanged;
                }
            }
        }

        private void ConsumerSystemSyncs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ConsumerSystemSync>())
                {
                    item.ConsumerSystem = this.KnowledgeConsumerSystemId;
                }
            }
        }

        private ObservableCollection<AgentIntegration> _agentIntegrations;

        [InverseProperty("KnowledgeConsumerSystem")]
        public virtual ObservableCollection<AgentIntegration> AgentIntegrations
        {
            get
            {
                if (_agentIntegrations == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentIntegrations - no database context is set. KnowledgeConsumerSystemId: " + this.KnowledgeConsumerSystemId + ".");
                        }
                        _agentIntegrations = new ObservableCollection<AgentIntegration>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentIntegrations.Where(x => x.KnowledgeSystem == this.KnowledgeConsumerSystemId).ToList<AgentIntegration>();
                        _agentIntegrations = new ObservableCollection<AgentIntegration>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _agentIntegrations.CollectionChanged += AgentIntegrations_CollectionChanged;
                }
                return _agentIntegrations;
            }
            private set
            {
                if (_agentIntegrations != null)
                {
                    _agentIntegrations.CollectionChanged -= AgentIntegrations_CollectionChanged;
                }
                _agentIntegrations = value;
                if (_agentIntegrations != null)
                {
                    _agentIntegrations.CollectionChanged += AgentIntegrations_CollectionChanged;
                }
            }
        }

        private void AgentIntegrations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AgentIntegration>())
                {
                    item.KnowledgeSystem = this.KnowledgeConsumerSystemId;
                }
            }
        }

        private ObservableCollection<KnowledgeQuerySource> _knowledgeQuerySources;

        [InverseProperty("KnowledgeConsumerSystem")]
        public virtual ObservableCollection<KnowledgeQuerySource> KnowledgeQuerySources
        {
            get
            {
                if (_knowledgeQuerySources == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeQuerySources - no database context is set. KnowledgeConsumerSystemId: " + this.KnowledgeConsumerSystemId + ".");
                        }
                        _knowledgeQuerySources = new ObservableCollection<KnowledgeQuerySource>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeQuerySources.Where(x => x.ConsumerSystem == this.KnowledgeConsumerSystemId).ToList<KnowledgeQuerySource>();
                        _knowledgeQuerySources = new ObservableCollection<KnowledgeQuerySource>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeQuerySources.CollectionChanged += KnowledgeQuerySources_CollectionChanged;
                }
                return _knowledgeQuerySources;
            }
            private set
            {
                if (_knowledgeQuerySources != null)
                {
                    _knowledgeQuerySources.CollectionChanged -= KnowledgeQuerySources_CollectionChanged;
                }
                _knowledgeQuerySources = value;
                if (_knowledgeQuerySources != null)
                {
                    _knowledgeQuerySources.CollectionChanged += KnowledgeQuerySources_CollectionChanged;
                }
            }
        }

        private void KnowledgeQuerySources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeQuerySource>())
                {
                    item.ConsumerSystem = this.KnowledgeConsumerSystemId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.OrganizationRef;
            _ = this.ConsumerSystemSyncs;
            _ = this.AgentIntegrations;
            _ = this.KnowledgeQuerySources;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
