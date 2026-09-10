
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
    [Table("WorkflowArtifacts")]
    public class WorkflowArtifactBase : SoAEntityBase
    {
        [Key]
        public string ArtifactId { get; set; }

        // Formula ParentPath (rulebook: =INDEX(WorkflowSteps!{{RelativePath}}, MATCH({{ProducedByStep}}, WorkflowSteps!{{WorkflowStepId}}, 0)))
        [NotMapped]
        public string? ParentPath
        {
            get => F.AsString(F.Memo(this, "ParentPath", () => F.Lookup<WorkflowStep>(this, "WorkflowSteps", "WorkflowStepId", __c => __c.WorkflowSteps, __r => F.Of(__r.WorkflowStepId), F.Of(this.ProducedByStep), __r => F.Of(__r.RelativePath), () => F.Of(new WorkflowStep().RelativePath)))); set { }
        }

        // Formula RelativePath (rulebook: ={{ParentPath}} & "/artifacts/" & {{ArtifactId}})
        [NotMapped]
        public string? RelativePath
        {
            get => F.AsString(F.Memo(this, "RelativePath", () => F.Concat(F.TextOr(F.Of(this.ParentPath)), F.S("/artifacts/"), F.TextOr(F.Of(this.ArtifactId))))); set { }
        }

        // Formula Iri (rulebook: =SUBSTITUTE({{RelativePath}}, "/", "-"))
        [NotMapped]
        public string? Iri
        {
            get => F.AsString(F.Memo(this, "Iri", () => F.Substitute(F.Of(this.RelativePath), F.S("/"), F.S("-")))); set { }
        }

        public string Title { get; set; }
        public string? Identifier { get; set; }
        public DateTimeOffset? Created { get; set; }
        // Formula ProducingAgentType (rulebook: =IF(NOT(ISBLANK({{AttributedToHumanAgent}})), "HumanAgent", IF(NOT(ISBLANK({{AttributedToAIAgent}})), "AIAgent", IF(NOT(ISBLANK({{AttributedToAutomatedPipeline}})), "AutomatedPipeline", ""))))
        [NotMapped]
        public string? ProducingAgentType
        {
            get => F.AsString(F.Memo(this, "ProducingAgentType", () => (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.AttributedToHumanAgent)))))) ? F.S("HumanAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.AttributedToAIAgent)))))) ? F.S("AIAgent") : (F.Truthy(F.Bool3(F.Not(F.Bool3(F.IsBlank(F.Of(this.AttributedToAutomatedPipeline)))))) ? F.S("AutomatedPipeline") : F.S("")))))); set { }
        }

        // Formula HasDerivationParent (rulebook: =NOT(ISBLANK({{DerivedFromArtifact}})))
        [NotMapped]
        public bool? HasDerivationParent
        {
            get => F.AsBool(F.Memo(this, "HasDerivationParent", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.DerivedFromArtifact)))))); set { }
        }

        // Formula ProducedByWorkflow (rulebook: =INDEX(WorkflowSteps!{{Workflow}}, MATCH({{ProducedByStep}}, WorkflowSteps!{{WorkflowStepId}}, 0)))
        [NotMapped]
        public string? ProducedByWorkflow
        {
            get => F.AsString(F.Memo(this, "ProducedByWorkflow", () => F.Lookup<WorkflowStep>(this, "WorkflowSteps", "WorkflowStepId", __c => __c.WorkflowSteps, __r => F.Of(__r.WorkflowStepId), F.Of(this.ProducedByStep), __r => F.Of(__r.Workflow), () => F.Of(new WorkflowStep().Workflow)))); set { }
        }

        // Formula HasProducingWorkflow (rulebook: =NOT(ISBLANK({{ProducedByWorkflow}})))
        [NotMapped]
        public bool? HasProducingWorkflow
        {
            get => F.AsBool(F.Memo(this, "HasProducingWorkflow", () => F.Not(F.Bool3(F.IsBlank(F.Of(this.ProducedByWorkflow)))))); set { }
        }

        public string? DerivationClosure { get; set; }

        public string? ArtifactType { get; set; }
        public string? ProducedByStep { get; set; }
        public string? RequiredBySteps { get; set; }
        public string? DerivedFromArtifact { get; set; }
        public string? AttributedToHumanAgent { get; set; }
        public string? AttributedToAIAgent { get; set; }
        public string? AttributedToAutomatedPipeline { get; set; }

        private ArtifactTypeConcept _artifactTypeConcept;

        [ForeignKey("ArtifactType")]
        public virtual ArtifactTypeConcept ArtifactTypeConcept
        {
            get
            {
                if (_artifactTypeConcept == null && !string.IsNullOrEmpty(ArtifactType))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ArtifactTypeConcept - no database context is set. ArtifactType: " + ArtifactType + ".");
                        }
                        return null;
                    }
                    _artifactTypeConcept = base.SoAContext.ArtifactTypeConcepts.Find(ArtifactType);
                    if (_artifactTypeConcept != null)
                    {
                        base.SoAContext.Attach(_artifactTypeConcept);
                    }
                }
                return _artifactTypeConcept;
            }
            set
            {
                if (_artifactTypeConcept != value)
                {
                    _artifactTypeConcept = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_artifactTypeConcept != null)
                    {
                        ArtifactType = _artifactTypeConcept.ConceptId;
                    }
                }
            }
        }

        private WorkflowStep _workflowStep;

        [ForeignKey("ProducedByStep")]
        public virtual WorkflowStep WorkflowStep
        {
            get
            {
                if (_workflowStep == null && !string.IsNullOrEmpty(ProducedByStep))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStep - no database context is set. ProducedByStep: " + ProducedByStep + ".");
                        }
                        return null;
                    }
                    _workflowStep = base.SoAContext.WorkflowSteps.Find(ProducedByStep);
                    if (_workflowStep != null)
                    {
                        base.SoAContext.Attach(_workflowStep);
                    }
                }
                return _workflowStep;
            }
            set
            {
                if (_workflowStep != value)
                {
                    _workflowStep = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowStep != null)
                    {
                        ProducedByStep = _workflowStep.WorkflowStepId;
                    }
                }
            }
        }

        private WorkflowStep _workflowStepRef;

        [ForeignKey("RequiredBySteps")]
        public virtual WorkflowStep WorkflowStepRef
        {
            get
            {
                if (_workflowStepRef == null && !string.IsNullOrEmpty(RequiredBySteps))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowStepRef - no database context is set. RequiredBySteps: " + RequiredBySteps + ".");
                        }
                        return null;
                    }
                    _workflowStepRef = base.SoAContext.WorkflowSteps.Find(RequiredBySteps);
                    if (_workflowStepRef != null)
                    {
                        base.SoAContext.Attach(_workflowStepRef);
                    }
                }
                return _workflowStepRef;
            }
            set
            {
                if (_workflowStepRef != value)
                {
                    _workflowStepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_workflowStepRef != null)
                    {
                        RequiredBySteps = _workflowStepRef.WorkflowStepId;
                    }
                }
            }
        }

        private WorkflowArtifact _workflowArtifact;

        [ForeignKey("DerivedFromArtifact")]
        public virtual WorkflowArtifact WorkflowArtifact
        {
            get
            {
                if (_workflowArtifact == null && !string.IsNullOrEmpty(DerivedFromArtifact))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifact - no database context is set. DerivedFromArtifact: " + DerivedFromArtifact + ".");
                        }
                        return null;
                    }
                    _workflowArtifact = base.SoAContext.WorkflowArtifacts.Find(DerivedFromArtifact);
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
                        DerivedFromArtifact = _workflowArtifact.ArtifactId;
                    }
                }
            }
        }

        private HumanAgent _humanAgent;

        [ForeignKey("AttributedToHumanAgent")]
        public virtual HumanAgent HumanAgent
        {
            get
            {
                if (_humanAgent == null && !string.IsNullOrEmpty(AttributedToHumanAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access HumanAgent - no database context is set. AttributedToHumanAgent: " + AttributedToHumanAgent + ".");
                        }
                        return null;
                    }
                    _humanAgent = base.SoAContext.HumanAgents.Find(AttributedToHumanAgent);
                    if (_humanAgent != null)
                    {
                        base.SoAContext.Attach(_humanAgent);
                    }
                }
                return _humanAgent;
            }
            set
            {
                if (_humanAgent != value)
                {
                    _humanAgent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_humanAgent != null)
                    {
                        AttributedToHumanAgent = _humanAgent.HumanAgentId;
                    }
                }
            }
        }

        private AIAgent _aIAgent;

        [ForeignKey("AttributedToAIAgent")]
        public virtual AIAgent AIAgent
        {
            get
            {
                if (_aIAgent == null && !string.IsNullOrEmpty(AttributedToAIAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AIAgent - no database context is set. AttributedToAIAgent: " + AttributedToAIAgent + ".");
                        }
                        return null;
                    }
                    _aIAgent = base.SoAContext.AIAgents.Find(AttributedToAIAgent);
                    if (_aIAgent != null)
                    {
                        base.SoAContext.Attach(_aIAgent);
                    }
                }
                return _aIAgent;
            }
            set
            {
                if (_aIAgent != value)
                {
                    _aIAgent = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_aIAgent != null)
                    {
                        AttributedToAIAgent = _aIAgent.AIAgentId;
                    }
                }
            }
        }

        private AutomatedPipeline _automatedPipeline;

        [ForeignKey("AttributedToAutomatedPipeline")]
        public virtual AutomatedPipeline AutomatedPipeline
        {
            get
            {
                if (_automatedPipeline == null && !string.IsNullOrEmpty(AttributedToAutomatedPipeline))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AutomatedPipeline - no database context is set. AttributedToAutomatedPipeline: " + AttributedToAutomatedPipeline + ".");
                        }
                        return null;
                    }
                    _automatedPipeline = base.SoAContext.AutomatedPipelines.Find(AttributedToAutomatedPipeline);
                    if (_automatedPipeline != null)
                    {
                        base.SoAContext.Attach(_automatedPipeline);
                    }
                }
                return _automatedPipeline;
            }
            set
            {
                if (_automatedPipeline != value)
                {
                    _automatedPipeline = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_automatedPipeline != null)
                    {
                        AttributedToAutomatedPipeline = _automatedPipeline.AutomatedPipelineId;
                    }
                }
            }
        }

        private ObservableCollection<WorkflowStep> _producesArtifactsWorkflowSteps;

        [InverseProperty("WorkflowArtifact")]
        public virtual ObservableCollection<WorkflowStep> ProducesArtifactsWorkflowSteps
        {
            get
            {
                if (_producesArtifactsWorkflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProducesArtifactsWorkflowSteps - no database context is set. ArtifactId: " + this.ArtifactId + ".");
                        }
                        _producesArtifactsWorkflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.ProducesArtifacts == this.ArtifactId).ToList<WorkflowStep>();
                        _producesArtifactsWorkflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _producesArtifactsWorkflowSteps.CollectionChanged += ProducesArtifactsWorkflowSteps_CollectionChanged;
                }
                return _producesArtifactsWorkflowSteps;
            }
            private set
            {
                if (_producesArtifactsWorkflowSteps != null)
                {
                    _producesArtifactsWorkflowSteps.CollectionChanged -= ProducesArtifactsWorkflowSteps_CollectionChanged;
                }
                _producesArtifactsWorkflowSteps = value;
                if (_producesArtifactsWorkflowSteps != null)
                {
                    _producesArtifactsWorkflowSteps.CollectionChanged += ProducesArtifactsWorkflowSteps_CollectionChanged;
                }
            }
        }

        private void ProducesArtifactsWorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.ProducesArtifacts = this.ArtifactId;
                }
            }
        }

        private ObservableCollection<WorkflowStep> _requiresArtifactsWorkflowSteps;

        [InverseProperty("WorkflowArtifactRef")]
        public virtual ObservableCollection<WorkflowStep> RequiresArtifactsWorkflowSteps
        {
            get
            {
                if (_requiresArtifactsWorkflowSteps == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RequiresArtifactsWorkflowSteps - no database context is set. ArtifactId: " + this.ArtifactId + ".");
                        }
                        _requiresArtifactsWorkflowSteps = new ObservableCollection<WorkflowStep>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowSteps.Where(x => x.RequiresArtifacts == this.ArtifactId).ToList<WorkflowStep>();
                        _requiresArtifactsWorkflowSteps = new ObservableCollection<WorkflowStep>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _requiresArtifactsWorkflowSteps.CollectionChanged += RequiresArtifactsWorkflowSteps_CollectionChanged;
                }
                return _requiresArtifactsWorkflowSteps;
            }
            private set
            {
                if (_requiresArtifactsWorkflowSteps != null)
                {
                    _requiresArtifactsWorkflowSteps.CollectionChanged -= RequiresArtifactsWorkflowSteps_CollectionChanged;
                }
                _requiresArtifactsWorkflowSteps = value;
                if (_requiresArtifactsWorkflowSteps != null)
                {
                    _requiresArtifactsWorkflowSteps.CollectionChanged += RequiresArtifactsWorkflowSteps_CollectionChanged;
                }
            }
        }

        private void RequiresArtifactsWorkflowSteps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowStep>())
                {
                    item.RequiresArtifacts = this.ArtifactId;
                }
            }
        }

        private ObservableCollection<AIAgent> _aIAgents;

        [InverseProperty("WorkflowArtifact")]
        public virtual ObservableCollection<AIAgent> AIAgents
        {
            get
            {
                if (_aIAgents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AIAgents - no database context is set. ArtifactId: " + this.ArtifactId + ".");
                        }
                        _aIAgents = new ObservableCollection<AIAgent>();
                    }
                    else
                    {
                        var items = base.SoAContext.AIAgents.Where(x => x.AttributedArtifacts == this.ArtifactId).ToList<AIAgent>();
                        _aIAgents = new ObservableCollection<AIAgent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _aIAgents.CollectionChanged += AIAgents_CollectionChanged;
                }
                return _aIAgents;
            }
            private set
            {
                if (_aIAgents != null)
                {
                    _aIAgents.CollectionChanged -= AIAgents_CollectionChanged;
                }
                _aIAgents = value;
                if (_aIAgents != null)
                {
                    _aIAgents.CollectionChanged += AIAgents_CollectionChanged;
                }
            }
        }

        private void AIAgents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AIAgent>())
                {
                    item.AttributedArtifacts = this.ArtifactId;
                }
            }
        }

        private ObservableCollection<ArtifactTypeConcept> _artifactTypeConcepts;

        [InverseProperty("WorkflowArtifact")]
        public virtual ObservableCollection<ArtifactTypeConcept> ArtifactTypeConcepts
        {
            get
            {
                if (_artifactTypeConcepts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ArtifactTypeConcepts - no database context is set. ArtifactId: " + this.ArtifactId + ".");
                        }
                        _artifactTypeConcepts = new ObservableCollection<ArtifactTypeConcept>();
                    }
                    else
                    {
                        var items = base.SoAContext.ArtifactTypeConcepts.Where(x => x.WorkflowArtifacts == this.ArtifactId).ToList<ArtifactTypeConcept>();
                        _artifactTypeConcepts = new ObservableCollection<ArtifactTypeConcept>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _artifactTypeConcepts.CollectionChanged += ArtifactTypeConcepts_CollectionChanged;
                }
                return _artifactTypeConcepts;
            }
            private set
            {
                if (_artifactTypeConcepts != null)
                {
                    _artifactTypeConcepts.CollectionChanged -= ArtifactTypeConcepts_CollectionChanged;
                }
                _artifactTypeConcepts = value;
                if (_artifactTypeConcepts != null)
                {
                    _artifactTypeConcepts.CollectionChanged += ArtifactTypeConcepts_CollectionChanged;
                }
            }
        }

        private void ArtifactTypeConcepts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<ArtifactTypeConcept>())
                {
                    item.WorkflowArtifacts = this.ArtifactId;
                }
            }
        }

        private ObservableCollection<WorkflowArtifact> _workflowArtifacts;

        [InverseProperty("WorkflowArtifact")]
        public virtual ObservableCollection<WorkflowArtifact> WorkflowArtifacts
        {
            get
            {
                if (_workflowArtifacts == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access WorkflowArtifacts - no database context is set. ArtifactId: " + this.ArtifactId + ".");
                        }
                        _workflowArtifacts = new ObservableCollection<WorkflowArtifact>();
                    }
                    else
                    {
                        var items = base.SoAContext.WorkflowArtifacts.Where(x => x.DerivedFromArtifact == this.ArtifactId).ToList<WorkflowArtifact>();
                        _workflowArtifacts = new ObservableCollection<WorkflowArtifact>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _workflowArtifacts.CollectionChanged += WorkflowArtifacts_CollectionChanged;
                }
                return _workflowArtifacts;
            }
            private set
            {
                if (_workflowArtifacts != null)
                {
                    _workflowArtifacts.CollectionChanged -= WorkflowArtifacts_CollectionChanged;
                }
                _workflowArtifacts = value;
                if (_workflowArtifacts != null)
                {
                    _workflowArtifacts.CollectionChanged += WorkflowArtifacts_CollectionChanged;
                }
            }
        }

        private void WorkflowArtifacts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<WorkflowArtifact>())
                {
                    item.DerivedFromArtifact = this.ArtifactId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ArtifactTypeConcept;
            _ = this.WorkflowStep;
            _ = this.WorkflowStepRef;
            _ = this.WorkflowArtifact;
            _ = this.HumanAgent;
            _ = this.AIAgent;
            _ = this.AutomatedPipeline;
            _ = this.ProducesArtifactsWorkflowSteps;
            _ = this.RequiresArtifactsWorkflowSteps;
            _ = this.AIAgents;
            _ = this.ArtifactTypeConcepts;
            _ = this.WorkflowArtifacts;
        }

        public override string ToString()
        {
            return this.ArtifactId?.ToString() ?? base.ToString() ?? "";
        }
    }
}
