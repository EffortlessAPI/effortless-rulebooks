
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
    [Table("ArtifactTypeConcepts")]
    public class ArtifactTypeConceptBase : SoAEntityBase
    {
        [Key]
        public string ConceptId { get; set; }

        // Formula RelativePath (rulebook: ="concepts/artifact-type/" & {{ConceptId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.S("concepts/artifact-type/"), F.TextOr(F.Of(this.ConceptId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string PrefLabel { get; set; }
        public string? AltLabel { get; set; }
        public string? Definition { get; set; }
        public string? ScopeNote { get; set; }

        public string? WorkflowArtifacts { get; set; }

        private WorkflowArtifact _workflowArtifact;

        [ForeignKey("WorkflowArtifacts")]
        public virtual WorkflowArtifact WorkflowArtifact
        {
            get
            {
                if (_workflowArtifact == null && !string.IsNullOrEmpty(WorkflowArtifacts))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifact - no database context is set. WorkflowArtifacts: " + WorkflowArtifacts + ".");
                        }
                        return null;
                    }
                    _workflowArtifact = base.SoAContext.WorkflowArtifacts.Find(WorkflowArtifacts);
                    if (_workflowArtifact != null)
                    {
                        base.SoAContext.Attach(_workflowArtifact);
                    }
                }
                return _workflowArtifact;
            }
            set
            {
                if (_workflowArtifact != value)
                {
                    _workflowArtifact = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowArtifact != null)
                    {
                        WorkflowArtifacts = _workflowArtifact.ArtifactId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowArtifact> _artifactTypeWorkflowArtifacts;

        [InverseProperty("ArtifactTypeConcept")]
        public virtual ObservableCollection<WorkflowArtifact> ArtifactTypeWorkflowArtifacts
        {
            get
            {
                if (_artifactTypeWorkflowArtifacts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ArtifactTypeWorkflowArtifacts - no database context is set. ConceptId: " + this.ConceptId + ".");
                        }
                        _artifactTypeWorkflowArtifacts = new ObservableCollection<WorkflowArtifact>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowArtifacts.Where(x => x.ArtifactType == this.ConceptId).ToList<WorkflowArtifact>();
                        _artifactTypeWorkflowArtifacts = new ObservableCollection<WorkflowArtifact>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _artifactTypeWorkflowArtifacts.CollectionChanged += ArtifactTypeWorkflowArtifacts_CollectionChanged;
                }
                return _artifactTypeWorkflowArtifacts;
            }
            private set
            {
                if (_artifactTypeWorkflowArtifacts != null)
                {
                    _artifactTypeWorkflowArtifacts.CollectionChanged -= ArtifactTypeWorkflowArtifacts_CollectionChanged;
                }
                _artifactTypeWorkflowArtifacts = value;
                if (_artifactTypeWorkflowArtifacts != null)
                {
                    _artifactTypeWorkflowArtifacts.CollectionChanged += ArtifactTypeWorkflowArtifacts_CollectionChanged;
                }
            }
        }

        private void ArtifactTypeWorkflowArtifacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowArtifact>())
                {
                    item.ArtifactType = this.ConceptId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.WorkflowArtifact;
            _ = this.ArtifactTypeWorkflowArtifacts;
        }

        public override string ToString()
        {
            return this.ConceptId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
