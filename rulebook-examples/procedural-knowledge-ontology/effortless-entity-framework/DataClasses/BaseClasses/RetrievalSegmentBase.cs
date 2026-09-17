
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
    [Table("RetrievalSegments")]
    public class RetrievalSegmentBase : SoAEntityBase
    {
        [Key]
        public string RetrievalSegmentId { get; set; }

        // Formula Name (rulebook: ={{RetrievalSegmentId}} & ": " & LEFT({{SegmentText}}, 50))
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.RetrievalSegmentId)), F.S(": "), F.Text(F.Left(F.Of(this.SegmentText), F.I(50)))))); set { }
        }

        public string? BoundaryKind { get; set; }
        public int? CharacterCount { get; set; }
        public string? PositionInProcess { get; set; }
        public string? ApplicabilityCondition { get; set; }
        public string? SegmentText { get; set; }
        public bool? IsIndexed { get; set; }
        // Formula AuthorAgentKind (rulebook: =INDEX(Agents!{{AgentKind}}, MATCH({{AuthoredByAgent}}, Agents!{{AgentId}}, 0)))
        [NotMapped]
        public string? AuthorAgentKind
        {
            get => F.AsString(F.Memo(this, "AuthorAgentKind", () => F.Lookup<Agent>(this, "Agents", "AgentId", __c => __c.Agents, __r => F.Of(__r.AgentId), F.Of(this.AuthoredByAgent), __r => F.Of(__r.AgentKind), () => F.Of(new Agent().AgentKind)))); set { }
        }

        // Formula RelatedFromCount (rulebook: =COUNTIFS(RetrievalSegments!{{RelatedSegment}}, RetrievalSegments!{{RetrievalSegmentId}}))
        [NotMapped]
        public int? RelatedFromCount
        {
            get => F.AsInt(F.Memo(this, "RelatedFromCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<RetrievalSegment>(base.SoAContext, "RetrievalSegments", __c => __c.RetrievalSegments), __r => F.CritField(F.Of(__r.RelatedSegment), F.Of(this.RetrievalSegmentId))))))); set { }
        }

        // Formula IsIsolatedChunk (rulebook: =AND({{PositionInProcess}} = "", {{ApplicabilityCondition}} = "", {{RelatedSegment}} = "", {{RelatedFromCount}} = 0))
        [NotMapped]
        public bool? IsIsolatedChunk
        {
            get => F.AsBool(F.Memo(this, "IsIsolatedChunk", () => F.And(F.Bool3(F.IsBlank(F.Of(this.PositionInProcess))), F.Bool3(F.IsBlank(F.Of(this.ApplicabilityCondition))), F.Bool3(F.IsBlank(F.Of(this.RelatedSegment))), F.Bool3(F.Eq(F.Of(this.RelatedFromCount), F.I(0)))))); set { }
        }

        // Formula ContradictedIsIndexed (rulebook: =INDEX(RetrievalSegments!{{IsIndexed}}, MATCH({{ContradictsSegment}}, RetrievalSegments!{{RetrievalSegmentId}}, 0)))
        [NotMapped]
        public bool? ContradictedIsIndexed
        {
            get => F.AsBool(F.Memo(this, "ContradictedIsIndexed", () => F.Lookup<RetrievalSegment>(this, "RetrievalSegments", "RetrievalSegmentId", __c => __c.RetrievalSegments, __r => F.Of(__r.RetrievalSegmentId), F.Of(this.ContradictsSegment), __r => F.Of(__r.IsIndexed), () => F.Of(new RetrievalSegment().IsIndexed)))); set { }
        }

        // Formula IsInconsistentGrounding (rulebook: =AND({{IsIndexed}}, {{ContradictsSegment}} <> "", {{ContradictedIsIndexed}}))
        [NotMapped]
        public bool? IsInconsistentGrounding
        {
            get => F.AsBool(F.Memo(this, "IsInconsistentGrounding", () => F.And(F.IsTrueV(F.Of(this.IsIndexed)), F.Bool3(F.IsNotBlank(F.Of(this.ContradictsSegment))), F.Bool3(F.Of(this.ContradictedIsIndexed))))); set { }
        }

        // Formula IsMachineHeldKnowledge (rulebook: =AND({{AuthorAgentKind}} <> "", {{AuthorAgentKind}} <> "Human", {{AccountableRole}} = ""))
        [NotMapped]
        public bool? IsMachineHeldKnowledge
        {
            get => F.AsBool(F.Memo(this, "IsMachineHeldKnowledge", () => F.And(F.Bool3(F.IsNotBlank(F.Of(this.AuthorAgentKind))), F.Bool3(F.Ne(F.Of(this.AuthorAgentKind), F.S("Human"))), F.Bool3(F.IsBlank(F.Of(this.AccountableRole)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ProcedureVersion { get; set; }
        public string? Step { get; set; }
        public string? DecisionPoint { get; set; }
        public string? RelatedSegment { get; set; }
        public string? SourceResource { get; set; }
        public string? AuthoredByAgent { get; set; }
        public string? AccountableRole { get; set; }
        public string? ContradictsSegment { get; set; }

        private ProcedureVersion _procedureVersionRef;

        [ForeignKey("ProcedureVersion")]
        public virtual ProcedureVersion ProcedureVersionRef
        {
            get
            {
                if (_procedureVersionRef == null && !string.IsNullOrEmpty(ProcedureVersion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureVersionRef - no database context is set. ProcedureVersion: " + ProcedureVersion + ".");
                        }
                        return null;
                    }
                    _procedureVersionRef = base.SoAContext.ProcedureVersions.Find(ProcedureVersion);
                    if (_procedureVersionRef != null)
                    {
                        base.SoAContext.Attach(_procedureVersionRef);
                    }
                }
                return _procedureVersionRef;
            }
            set
            {
                if (_procedureVersionRef != value)
                {
                    _procedureVersionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureVersionRef != null)
                    {
                        ProcedureVersion = _procedureVersionRef.ProcedureVersionId;
                    }
                }
            }
        }

        private Step _stepRef;

        [ForeignKey("Step")]
        public virtual Step StepRef
        {
            get
            {
                if (_stepRef == null && !string.IsNullOrEmpty(Step))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access StepRef - no database context is set. Step: " + Step + ".");
                        }
                        return null;
                    }
                    _stepRef = base.SoAContext.Steps.Find(Step);
                    if (_stepRef != null)
                    {
                        base.SoAContext.Attach(_stepRef);
                    }
                }
                return _stepRef;
            }
            set
            {
                if (_stepRef != value)
                {
                    _stepRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_stepRef != null)
                    {
                        Step = _stepRef.StepId;
                    }
                }
            }
        }

        private DecisionPoint _decisionPointRef;

        [ForeignKey("DecisionPoint")]
        public virtual DecisionPoint DecisionPointRef
        {
            get
            {
                if (_decisionPointRef == null && !string.IsNullOrEmpty(DecisionPoint))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access DecisionPointRef - no database context is set. DecisionPoint: " + DecisionPoint + ".");
                        }
                        return null;
                    }
                    _decisionPointRef = base.SoAContext.DecisionPoints.Find(DecisionPoint);
                    if (_decisionPointRef != null)
                    {
                        base.SoAContext.Attach(_decisionPointRef);
                    }
                }
                return _decisionPointRef;
            }
            set
            {
                if (_decisionPointRef != value)
                {
                    _decisionPointRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_decisionPointRef != null)
                    {
                        DecisionPoint = _decisionPointRef.DecisionPointId;
                    }
                }
            }
        }

        private RetrievalSegment _retrievalSegment;

        [ForeignKey("RelatedSegment")]
        public virtual RetrievalSegment RetrievalSegment
        {
            get
            {
                if (_retrievalSegment == null && !string.IsNullOrEmpty(RelatedSegment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegment - no database context is set. RelatedSegment: " + RelatedSegment + ".");
                        }
                        return null;
                    }
                    _retrievalSegment = base.SoAContext.RetrievalSegments.Find(RelatedSegment);
                    if (_retrievalSegment != null)
                    {
                        base.SoAContext.Attach(_retrievalSegment);
                    }
                }
                return _retrievalSegment;
            }
            set
            {
                if (_retrievalSegment != value)
                {
                    _retrievalSegment = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_retrievalSegment != null)
                    {
                        RelatedSegment = _retrievalSegment.RetrievalSegmentId;
                    }
                }
            }
        }

        private Resource _resource;

        [ForeignKey("SourceResource")]
        public virtual Resource Resource
        {
            get
            {
                if (_resource == null && !string.IsNullOrEmpty(SourceResource))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Resource - no database context is set. SourceResource: " + SourceResource + ".");
                        }
                        return null;
                    }
                    _resource = base.SoAContext.Resources.Find(SourceResource);
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
                        SourceResource = _resource.ResourceId;
                    }
                }
            }
        }

        private Agent _agent;

        [ForeignKey("AuthoredByAgent")]
        public virtual Agent Agent
        {
            get
            {
                if (_agent == null && !string.IsNullOrEmpty(AuthoredByAgent))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Agent - no database context is set. AuthoredByAgent: " + AuthoredByAgent + ".");
                        }
                        return null;
                    }
                    _agent = base.SoAContext.Agents.Find(AuthoredByAgent);
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
                        AuthoredByAgent = _agent.AgentId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("AccountableRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(AccountableRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. AccountableRole: " + AccountableRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(AccountableRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        AccountableRole = _role.RoleId;
                    }
                }
            }
        }

        private RetrievalSegment _retrievalSegmentRef;

        [ForeignKey("ContradictsSegment")]
        public virtual RetrievalSegment RetrievalSegmentRef
        {
            get
            {
                if (_retrievalSegmentRef == null && !string.IsNullOrEmpty(ContradictsSegment))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RetrievalSegmentRef - no database context is set. ContradictsSegment: " + ContradictsSegment + ".");
                        }
                        return null;
                    }
                    _retrievalSegmentRef = base.SoAContext.RetrievalSegments.Find(ContradictsSegment);
                    if (_retrievalSegmentRef != null)
                    {
                        base.SoAContext.Attach(_retrievalSegmentRef);
                    }
                }
                return _retrievalSegmentRef;
            }
            set
            {
                if (_retrievalSegmentRef != value)
                {
                    _retrievalSegmentRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_retrievalSegmentRef != null)
                    {
                        ContradictsSegment = _retrievalSegmentRef.RetrievalSegmentId;
                    }
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _relatedSegmentRetrievalSegments;

        [InverseProperty("RetrievalSegment")]
        public virtual ObservableCollection<RetrievalSegment> RelatedSegmentRetrievalSegments
        {
            get
            {
                if (_relatedSegmentRetrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RelatedSegmentRetrievalSegments - no database context is set. RetrievalSegmentId: " + this.RetrievalSegmentId + ".");
                        }
                        _relatedSegmentRetrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.RelatedSegment == this.RetrievalSegmentId).ToList<RetrievalSegment>();
                        _relatedSegmentRetrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _relatedSegmentRetrievalSegments.CollectionChanged += RelatedSegmentRetrievalSegments_CollectionChanged;
                }
                return _relatedSegmentRetrievalSegments;
            }
            private set
            {
                if (_relatedSegmentRetrievalSegments != null)
                {
                    _relatedSegmentRetrievalSegments.CollectionChanged -= RelatedSegmentRetrievalSegments_CollectionChanged;
                }
                _relatedSegmentRetrievalSegments = value;
                if (_relatedSegmentRetrievalSegments != null)
                {
                    _relatedSegmentRetrievalSegments.CollectionChanged += RelatedSegmentRetrievalSegments_CollectionChanged;
                }
            }
        }

        private void RelatedSegmentRetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.RelatedSegment = this.RetrievalSegmentId;
                }
            }
        }

        private ObservableCollection<RetrievalSegment> _contradictsSegmentRetrievalSegments;

        [InverseProperty("RetrievalSegmentRef")]
        public virtual ObservableCollection<RetrievalSegment> ContradictsSegmentRetrievalSegments
        {
            get
            {
                if (_contradictsSegmentRetrievalSegments == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ContradictsSegmentRetrievalSegments - no database context is set. RetrievalSegmentId: " + this.RetrievalSegmentId + ".");
                        }
                        _contradictsSegmentRetrievalSegments = new ObservableCollection<RetrievalSegment>();
                    }
                    else
                    {
                        var items = base.SoAContext.RetrievalSegments.Where(x => x.ContradictsSegment == this.RetrievalSegmentId).ToList<RetrievalSegment>();
                        _contradictsSegmentRetrievalSegments = new ObservableCollection<RetrievalSegment>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _contradictsSegmentRetrievalSegments.CollectionChanged += ContradictsSegmentRetrievalSegments_CollectionChanged;
                }
                return _contradictsSegmentRetrievalSegments;
            }
            private set
            {
                if (_contradictsSegmentRetrievalSegments != null)
                {
                    _contradictsSegmentRetrievalSegments.CollectionChanged -= ContradictsSegmentRetrievalSegments_CollectionChanged;
                }
                _contradictsSegmentRetrievalSegments = value;
                if (_contradictsSegmentRetrievalSegments != null)
                {
                    _contradictsSegmentRetrievalSegments.CollectionChanged += ContradictsSegmentRetrievalSegments_CollectionChanged;
                }
            }
        }

        private void ContradictsSegmentRetrievalSegments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<RetrievalSegment>())
                {
                    item.ContradictsSegment = this.RetrievalSegmentId;
                }
            }
        }

        private ObservableCollection<AnswerGrounding> _answerGroundings;

        [InverseProperty("RetrievalSegmentRef")]
        public virtual ObservableCollection<AnswerGrounding> AnswerGroundings
        {
            get
            {
                if (_answerGroundings == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access AnswerGroundings - no database context is set. RetrievalSegmentId: " + this.RetrievalSegmentId + ".");
                        }
                        _answerGroundings = new ObservableCollection<AnswerGrounding>();
                    }
                    else
                    {
                        var items = base.SoAContext.AnswerGroundings.Where(x => x.RetrievalSegment == this.RetrievalSegmentId).ToList<AnswerGrounding>();
                        _answerGroundings = new ObservableCollection<AnswerGrounding>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _answerGroundings.CollectionChanged += AnswerGroundings_CollectionChanged;
                }
                return _answerGroundings;
            }
            private set
            {
                if (_answerGroundings != null)
                {
                    _answerGroundings.CollectionChanged -= AnswerGroundings_CollectionChanged;
                }
                _answerGroundings = value;
                if (_answerGroundings != null)
                {
                    _answerGroundings.CollectionChanged += AnswerGroundings_CollectionChanged;
                }
            }
        }

        private void AnswerGroundings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<AnswerGrounding>())
                {
                    item.RetrievalSegment = this.RetrievalSegmentId;
                }
            }
        }

        private ObservableCollection<KnowledgeSearchEvent> _knowledgeSearchEvents;

        [InverseProperty("RetrievalSegment")]
        public virtual ObservableCollection<KnowledgeSearchEvent> KnowledgeSearchEvents
        {
            get
            {
                if (_knowledgeSearchEvents == null)
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeSearchEvents - no database context is set. RetrievalSegmentId: " + this.RetrievalSegmentId + ".");
                        }
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>();
                    }
                    else
                    {
                        var items = base.SoAContext.KnowledgeSearchEvents.Where(x => x.OpenedSegment == this.RetrievalSegmentId).ToList<KnowledgeSearchEvent>();
                        _knowledgeSearchEvents = new ObservableCollection<KnowledgeSearchEvent>(items);
                        if (items.Any())
                        {
                            base.SoAContext.AttachRange(items);
                        }
                    }
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
                return _knowledgeSearchEvents;
            }
            private set
            {
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged -= KnowledgeSearchEvents_CollectionChanged;
                }
                _knowledgeSearchEvents = value;
                if (_knowledgeSearchEvents != null)
                {
                    _knowledgeSearchEvents.CollectionChanged += KnowledgeSearchEvents_CollectionChanged;
                }
            }
        }

        private void KnowledgeSearchEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<KnowledgeSearchEvent>())
                {
                    item.OpenedSegment = this.RetrievalSegmentId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ProcedureVersionRef;
            _ = this.StepRef;
            _ = this.DecisionPointRef;
            _ = this.RetrievalSegment;
            _ = this.Resource;
            _ = this.Agent;
            _ = this.Role;
            _ = this.RetrievalSegmentRef;
            _ = this.RelatedSegmentRetrievalSegments;
            _ = this.ContradictsSegmentRetrievalSegments;
            _ = this.AnswerGroundings;
            _ = this.KnowledgeSearchEvents;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
