
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
    [Table("MethodApplications")]
    public class MethodApplicationBase : SoAEntityBase
    {
        [Key]
        public string MethodApplicationId { get; set; }

        // Formula Name (rulebook: ={{KnowledgeMethod}} & " applied: " & LEFT({{AppliedTo}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.KnowledgeMethod)), F.S(" applied: "), F.Text(F.Left(F.Of(this.AppliedTo), F.I(50)))))); set { }
        }

        public string? AppliedTo { get; set; }
        public DateTimeOffset? AppliedAt { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? KnowledgeMethod { get; set; }
        public string? AppliedByAgent { get; set; }
        public string? AppliedToGroundingSnapshot { get; set; }
        public string? IdentifiedBroker { get; set; }

        private KnowledgeMethod _knowledgeMethodRef;

        [ForeignKey("KnowledgeMethod")]
        public virtual KnowledgeMethod KnowledgeMethodRef
        {
            get
            {
                if (_knowledgeMethodRef == null && !string.IsNullOrEmpty(KnowledgeMethod))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeMethodRef - no database context is set. KnowledgeMethod: " + KnowledgeMethod + ".");
                        }
                        return null;
                    }
                    _knowledgeMethodRef = base.SoAContext.KnowledgeMethods.Find(KnowledgeMethod);
                    if (_knowledgeMethodRef != null)
                    {
                        base.SoAContext.Attach(_knowledgeMethodRef);
                    }
                }
                return _knowledgeMethodRef;
            }
            set
            {
                if (_knowledgeMethodRef != value)
                {
                    _knowledgeMethodRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeMethodRef != null)
                    {
                        KnowledgeMethod = _knowledgeMethodRef.KnowledgeMethodId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("AppliedByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AppliedByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AppliedByAgent: " + AppliedByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AppliedByAgent);
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
                        AppliedByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private GroundingSnapshot _groundingSnapshot;

        [ForeignKey("AppliedToGroundingSnapshot")]
        public virtual GroundingSnapshot GroundingSnapshot
        {
            get
            {
                if (_groundingSnapshot == null && !string.IsNullOrEmpty(AppliedToGroundingSnapshot))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GroundingSnapshot - no database context is set. AppliedToGroundingSnapshot: " + AppliedToGroundingSnapshot + ".");
                        }
                        return null;
                    }
                    _groundingSnapshot = base.SoAContext.GroundingSnapshots.Find(AppliedToGroundingSnapshot);
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
                        AppliedToGroundingSnapshot = _groundingSnapshot.GroundingSnapshotId;
                    }
                }
            }
        }

        private Agent _agentRef;

        [ForeignKey("IdentifiedBroker")]
        public virtual Agent AgentRef
        {
            get
            {
                if (_agentRef == null && !string.IsNullOrEmpty(IdentifiedBroker))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AgentRef - no database context is set. IdentifiedBroker: " + IdentifiedBroker + ".");
                        }
                        return null;
                    }
                    _agentRef = base.SoAContext.Agents.Find(IdentifiedBroker);
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
                        IdentifiedBroker = _agentRef.AgentId;
                    }
                }
            }
        }

        private ObservableCollection<CollectedSourceMaterial> _collectedSourceMaterials;

        [InverseProperty("MethodApplication")]
        public virtual ObservableCollection<CollectedSourceMaterial> CollectedSourceMaterials
        {
            get
            {
                if (_collectedSourceMaterials == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CollectedSourceMaterials - no database context is set. MethodApplicationId: " + this.MethodApplicationId + ".");
                        }
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>();
                    }
                    else
                    {
                        var items = base.SoAContext.CollectedSourceMaterials.Where(x => x.ProducedByMethodApplication == this.MethodApplicationId).ToList<CollectedSourceMaterial>();
                        _collectedSourceMaterials = new ObservableCollection<CollectedSourceMaterial>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
                return _collectedSourceMaterials;
            }
            private set
            {
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged -= CollectedSourceMaterials_CollectionChanged;
                }
                _collectedSourceMaterials = value;
                if (_collectedSourceMaterials != null)
                {
                    _collectedSourceMaterials.CollectionChanged += CollectedSourceMaterials_CollectionChanged;
                }
            }
        }

        private void CollectedSourceMaterials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<CollectedSourceMaterial>())
                {
                    item.ProducedByMethodApplication = this.MethodApplicationId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.KnowledgeMethodRef;
            _ = this.Agent;
            _ = this.GroundingSnapshot;
            _ = this.AgentRef;
            _ = this.CollectedSourceMaterials;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
