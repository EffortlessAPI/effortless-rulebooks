
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("OntologyProfiles")]
    public class OntologyProfileBase : SoAEntityBase
    {
        [Key]
        public string OntologyProfileId { get; set; }

        // Formula Name (rulebook: ={{Label}} & " " & {{Version}})
        public string? Name
        {
            get => this.Label + " " + this.Version; set { }
        }

        public string? Label { get; set; }
        public string? Version { get; set; }
        public string? VersionIri { get; set; }
        public string? NamespaceIri { get; set; }
        public string? License { get; set; }
        public string? Scope { get; set; }


        private ObservableCollection<SemanticMapping> _semanticMappings;

        [InverseProperty("OntologyProfile")]
        public virtual ObservableCollection<SemanticMapping> SemanticMappings
        {
            get
            {
                if (_semanticMappings == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access SemanticMappings - no database context is set. OntologyProfileId: " + this.OntologyProfileId + ".");
                        }
                        _semanticMappings = new ObservableCollection<SemanticMapping>();
                    }
                    else
                    {
                        var items = Context.SemanticMappings.Where(x => x.OntologyProfile == this.OntologyProfileId).ToList<SemanticMapping>();
                        _semanticMappings = new ObservableCollection<SemanticMapping>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _semanticMappings.CollectionChanged += SemanticMappings_CollectionChanged;
                }
                return _semanticMappings;
            }
            private set
            {
                if (_semanticMappings != null)
                {
                    _semanticMappings.CollectionChanged -= SemanticMappings_CollectionChanged;
                }
                _semanticMappings = value;
                if (_semanticMappings != null)
                {
                    _semanticMappings.CollectionChanged += SemanticMappings_CollectionChanged;
                }
            }
        }

        private void SemanticMappings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<SemanticMapping>())
                {
                    item.OntologyProfile = this.OntologyProfileId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.SemanticMappings;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
