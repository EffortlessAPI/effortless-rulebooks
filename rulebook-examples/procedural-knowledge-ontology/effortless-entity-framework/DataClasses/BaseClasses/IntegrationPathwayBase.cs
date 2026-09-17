
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
    [Table("IntegrationPathways")]
    public class IntegrationPathwayBase : SoAEntityBase
    {
        [Key]
        public string IntegrationPathwayId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Of(this.Label))); set { }
        }

        public string? Label { get; set; }
        public string? Protocol { get; set; }
        // Formula IntegrationCountOnPathway (rulebook: =COUNTIFS(AgentIntegrations!{{Pathway}}, {{IntegrationPathwayId}}))
        [NotMapped]
        public int? IntegrationCountOnPathway
        {
            get => F.AsInt(F.Memo(this, "IntegrationCountOnPathway", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<AgentIntegration>(base.SoAContext, "AgentIntegrations", __c => __c.AgentIntegrations), __r => F.CritField(F.Of(__r.Pathway), F.Of(this.IntegrationPathwayId))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }


        private ObservableCollection<AgentIntegration> _agentIntegrations;

        [InverseProperty("IntegrationPathway")]
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
                            throw new InvalidOperationException("Cannot access AgentIntegrations - no database context is set. IntegrationPathwayId: " + this.IntegrationPathwayId + ".");
                        }
                        _agentIntegrations = new ObservableCollection<AgentIntegration>();
                    }
                    else
                    {
                        var items = base.SoAContext.AgentIntegrations.Where(x => x.Pathway == this.IntegrationPathwayId).ToList<AgentIntegration>();
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
                    item.Pathway = this.IntegrationPathwayId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.AgentIntegrations;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
